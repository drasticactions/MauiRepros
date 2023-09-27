// See https://aka.ms/new-console-template for more information
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Runtime.InteropServices;
using System.Globalization;
using Microsoft.Win32.SafeHandles;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Net;

Console.WriteLine("Hello, World!");

var stream = new TcpNetworkStream(3333);
Task.Run(async () => { stream.AcceptClientAsync(); });
var bridge = new TcpDataBridge(stream);
var protocol = new ProtocolHandler(bridge, shouldStart: true);

protocol.RegisterMessageObserver<TestObject>(MessageType.Reply, OnTestObject);
var test2 = new TcpDataBridge("localhost", 3333);
var protocol2 = new ProtocolHandler(test2);
//protocol2.MessagePosted += Protocol2_MessagePosted;
protocol2.RegisterMessageObserver<TestObject>(MessageType.Reply, OnTestObject);

void OnTestObject(TestObject message)
{
    Console.WriteLine(message.TestString);
}

while (true)
{
    protocol.PostMessage(MessageType.Reply, new TestObject() { TestString = "Hello 1" });
    protocol2.PostMessage(MessageType.Reply, new TestObject() { TestString = "Hello 2" });
    Console.ReadKey();
}


[Serializable]
public class TestObject
{
    public TestObject()
    {
    }

    public string TestString { get; set; }

    public Exception Exception { get; set; }
}

public class Agent : IDataBridge
{
    public ManualResetEvent ReadyEvent { get; } = new ManualResetEvent(false);
    ManualResetEvent IDataBridge.FirstMessageEvent => ReadyEvent;

    readonly Queue<byte[]> hostMessages = new Queue<byte[]>();

    public void Close()
    {
    }

    public byte[] ReadMessage()
    {
        byte[]? message = null;
        lock (hostMessages)
        {
            while (true)
            {
                if (hostMessages.Count > 0)
                {
                    message = hostMessages.Dequeue();
                    break;
                }
                Monitor.Wait(hostMessages);
            }
        }

        return message;
    }

    public void WriteMessage(byte[] buffer)
    {
        throw new NotImplementedException();
    }
}

public sealed class TcpDataBridge : IDataBridge, IDisposable
{
    private readonly TcpNetworkStream stream;
    private bool isFirstMessage = true;
    private bool closed;

    public TcpDataBridge(int port)
    {
        // Create a TCP server to listen for incoming connections
        this.stream = new TcpNetworkStream(port);
    }

    public TcpDataBridge(string ip, int port)
    {
        // Create a TCP client to connect to the server
        this.stream = new TcpNetworkStream(ip, port);
    }

    public TcpDataBridge(TcpNetworkStream stream)
    {
        this.stream = stream;
    }

    #region IDataBridge implementation

    public ManualResetEvent ReadyEvent { get; } = new ManualResetEvent(true);
    public ManualResetEvent FirstMessageEvent { get; } = new ManualResetEvent(false);

    public void Close()
    {
        this.closed = true;
        this.stream.Close();
    }

    public byte[] ReadMessage()
    {
        // Read length
        const int LengthSize = sizeof(int);
        byte[] lengthBuffer = new byte[LengthSize];
        this.FillBufferFromReadStream(lengthBuffer, 0);
        if (this.closed)
        {
            return null;
        }

        int dataLength = BitConverter.ToInt32(lengthBuffer, 0);

        // Read the message body
        byte[] messageBuffer = new byte[LengthSize + dataLength];
        Array.Copy(lengthBuffer, messageBuffer, LengthSize);
        this.FillBufferFromReadStream(messageBuffer, LengthSize);
        if (this.closed)
        {
            return null;
        }

        if (this.isFirstMessage)
        {
            this.isFirstMessage = false;
            this.FirstMessageEvent.Set();
        }

        return messageBuffer;
    }

    private void FillBufferFromReadStream(byte[] buffer, int startingOffset)
    {
        int offset = startingOffset;
        while (offset < buffer.Length)
        {
            int bytesRead = this.stream.Read(buffer, offset, buffer.Length - offset);
            if (this.closed)
            {
                return;
            }

            offset += bytesRead;
        }
    }

    public void WriteMessage(byte[] buffer)
    {
        this.stream.Write(buffer);
    }

    #endregion

    [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed")]
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public void Dispose()
    {
        this.ReadyEvent.Dispose();
        this.FirstMessageEvent.Dispose();
        this.stream.Dispose();
        GC.SuppressFinalize(this);
    }
}

public sealed class TcpNetworkStream : IDisposable
{
    private bool done;
    private TcpListener? listener;
    private TcpClient? client;
    private NetworkStream? networkStream;
    private int writeBufferSize;
    private ManualResetEvent writeEvent = new ManualResetEvent(false);
    private ManualResetEvent readEvent = new ManualResetEvent(false);

    /// <summary>
    /// Creates a new TCP network stream.
    /// </summary>
    /// <param name="port">The port to listen on for incoming connections.</param>
    public TcpNetworkStream(int port)
    {
        this.listener = new TcpListener(IPAddress.Any, port);
        this.listener.Start();
    }

    public TcpNetworkStream(string ip, int port)
    {
        this.client = new TcpClient(ip, port);
        this.networkStream = this.client.GetStream();
    }

    /// <summary>
    /// Accepts an incoming TCP client connection.
    /// </summary>
    public async Task AcceptClientAsync()
    {
        if (!this.done && this.listener is not null)
        {
            this.client = await this.listener.AcceptTcpClientAsync();
            this.networkStream = this.client.GetStream();
            this.Initialize();
        }
    }

    /// <summary>
    /// Closes the TCP network stream.
    /// </summary>
    public void Close()
    {
        this.done = true;
        this.writeEvent.Set();
        this.readEvent.Set();
        this.networkStream?.Close();
        this.client?.Close();
        this.listener?.Stop();
    }

    /// <summary>
    /// Reads from the network stream into the provided buffer.
    /// </summary>
    /// <param name="buffer">Buffer to write the data read from the stream.</param>
    /// <param name="offset">Offset into the buffer where writing should begin.</param>
    /// <param name="maxBytesToRead">Maximum number of bytes to write into the buffer.</param>
    /// <returns>Number of bytes read from the stream and written into the buffer.</returns>
    public int Read(byte[] buffer, int offset, int maxBytesToRead)
    {
        if (!this.WaitForCondition(this.writeEvent, () =>
        {
            if (this.done)
            {
                return ConditionResult.Shutdown;
            }

            return this.networkStream is not null && this.networkStream.DataAvailable ? ConditionResult.Success : ConditionResult.NotReadyYet;
        }))
        {
            return 0;
        }

        int bytesRead = this.networkStream?.Read(buffer, offset, maxBytesToRead) ?? 0;
        this.readEvent.Set();
        return bytesRead;
    }

    /// <summary>
    /// Writes data in a buffer to the network stream.
    /// </summary>
    /// <param name="buffer">Buffer containing data to write to the stream.</param>
    public void Write(byte[] buffer)
    {
        if (this.done)
        {
            return;
        }

        this.networkStream?.Write(buffer, 0, buffer.Length);
        this.networkStream?.Flush();
        this.writeEvent.Set();
    }

    private void Initialize()
    {
        if (this.client is not null)
        {
            this.writeBufferSize = this.client.SendBufferSize;
            if (this.writeBufferSize == 0)
            {
                // Use a reasonable default value if SendBufferSize is zero.
                this.writeBufferSize = 4096;
            }
        }
    }

    private enum ConditionResult
    {
        Success,
        NotReadyYet,
        Shutdown
    }

    private bool WaitForCondition(ManualResetEvent waitHandle, Func<ConditionResult> condition)
    {
        while (true)
        {
            ConditionResult conditionResult = condition();

            switch (conditionResult)
            {
                case ConditionResult.Success:
                    return true;
                case ConditionResult.NotReadyYet:
                    {
                        if (!waitHandle.WaitOne())
                        {
                            return false;
                        }
                        break;
                    }
                case ConditionResult.Shutdown:
                default:
                    return false;
            }
        }
    }

    public void Dispose()
    {
        this.Close();
        this.writeEvent.Dispose();
        this.readEvent.Dispose();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Encapsulates a unidirectional anonymous pipe.
/// </summary>
public sealed class AnonymousPipe : IDisposable
{
    private bool done;
    private AutoResetEvent readEvent = new AutoResetEvent(false);
    private AnonymousPipeServerStream readPipe;
    private int writeBufferSize;
    private AutoResetEvent writeEvent = new AutoResetEvent(false);
    private AnonymousPipeClientStream writePipe;

    [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool PeekNamedPipe(SafePipeHandle handle, byte[] buffer, int nBufferSize, ref int bytesRead, ref int bytesAvail, ref int bytesLeftThisMessage);

    /// <summary>
    /// Creates a new unidirectional anonymous pipe.
    /// </summary>
    public AnonymousPipe()
    {
        this.readPipe = new AnonymousPipeServerStream(PipeDirection.In);
        this.writePipe = new AnonymousPipeClientStream(PipeDirection.Out, this.readPipe.ClientSafePipeHandle);
        this.Initialize();
    }

    public AnonymousPipe(IntPtr readEvent, IntPtr readPipe, IntPtr writeEvent, IntPtr writePipe)
    {
        this.readEvent.SafeWaitHandle = new SafeWaitHandle(readEvent, true);
        this.writeEvent.SafeWaitHandle = new SafeWaitHandle(writeEvent, true);
        this.readPipe = new AnonymousPipeServerStream(PipeDirection.In, new SafePipeHandle(readPipe, true), new SafePipeHandle(writePipe, true));
        this.writePipe = new AnonymousPipeClientStream(PipeDirection.Out, this.readPipe.ClientSafePipeHandle);
        this.Initialize();
    }

    /// <summary>
    /// Closes the pipe.
    /// </summary>
    public void Close()
    {
        this.done = true;
        this.writeEvent.Set();
        this.readEvent.Set();
    }

    /// <summary>
    /// Creates a string containing duplicated handle ids which can be used by the 
    /// remote endpoint to connect to the pipe.
    /// </summary>
    [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods")]
    public string Serialize(Func<IntPtr, IntPtr> duplicateHandle)
    {
        IntPtr targetReadEvent = duplicateHandle(this.readEvent.SafeWaitHandle.DangerousGetHandle());
        IntPtr targetReadPipe = duplicateHandle(this.readPipe.SafePipeHandle.DangerousGetHandle());
        IntPtr targetWriteEvent = duplicateHandle(this.writeEvent.SafeWaitHandle.DangerousGetHandle());
        IntPtr targetWritePipe = duplicateHandle(this.writePipe.SafePipeHandle.DangerousGetHandle());
        return string.Format(CultureInfo.InvariantCulture, "{0:X} {1:X} {2:X} {3:X} ",
                             targetReadEvent.ToInt64(),
                             targetReadPipe.ToInt64(),
                             targetWriteEvent.ToInt64(),
                             targetWritePipe.ToInt64());
    }

    /// <summary>
    /// Reads from the pipe into the provided buffer.
    /// </summary>
    /// <param name="buffer">Buffer to write the data read from the pipe.</param>
    /// <param name="offset">Offset into the buffer where writing should begin.</param>
    /// <param name="maxBytesToRead">Maximum number of bytes to write into the buffer.</param>
    /// <returns>Number of bytes read from the pipe and written into the buffer.</returns>
    public int Read(byte[] buffer, int offset, int maxBytesToRead)
    {
        int totalBytesAvailable = 0;

        // Wait until there are bytes in the pipe to read
        if (!this.WaitForCondition(this.writeEvent, () =>
        {
            if (this.done)
            {
                return ConditionResult.Shutdown;
            }

            int ignoredBytesRead = 0;
            int ignoredBytesLeftThisMessage = 0;
            if (!AnonymousPipe.PeekNamedPipe(this.readPipe.SafePipeHandle, null, 0, ref ignoredBytesRead, ref totalBytesAvailable, ref ignoredBytesLeftThisMessage))
            {
                return ConditionResult.Shutdown;
            }

            return totalBytesAvailable > 0 ? ConditionResult.Success : ConditionResult.NotReadyYet;
        }))
        {
            return 0;
        }

        // Fill the buffer, but do not exceed the number of available bytes otherwise the call will block
        int countToRead = Math.Min(maxBytesToRead, totalBytesAvailable);
        int bytesRead = this.readPipe.Read(buffer, offset, countToRead);

        this.readEvent.Set();
        return bytesRead;
    }

    /// <summary>
    /// Writes data in a buffer to the pipe.
    /// </summary>
    /// <param name="buffer">Buffer containing data to write to the pipe.</param>
    public void Write(byte[] buffer)
    {
        // Send the message piecemeal in m_writeBufferSize chunks. If we try to send it all at once WriteFile will
        // block until ReadFile has cleared previous buffers. This causes a deadlock because we will never set m_writeEvent.
        // Additionally, we can not block on WriteFile otherwise Close will fail to terminate the SendThread.
        int bytesWritten = 0;
        int numberOfBytesWritten = 0;
        for (; bytesWritten < buffer.Length; bytesWritten += numberOfBytesWritten)
        {
            int bytesToWrite = buffer.Length - bytesWritten;
            if (this.writeBufferSize != 0)
            {
                int totalBytesAvailable = 0;

                if (!this.WaitForCondition(this.readEvent, () =>
                {
                    if (this.done)
                    {
                        return ConditionResult.Shutdown;
                    }

                    int ignoredBytesRead = 0;
                    int ignoredBytesLeftThisMessage = 0;
                    if (!AnonymousPipe.PeekNamedPipe(this.readPipe.SafePipeHandle, null, 0, ref ignoredBytesRead, ref totalBytesAvailable, ref ignoredBytesLeftThisMessage))
                    {
                        return ConditionResult.Shutdown;
                    }

                    return totalBytesAvailable < this.writeBufferSize ? ConditionResult.Success : ConditionResult.NotReadyYet;
                }))
                {
                    return;
                }

                bytesToWrite = Math.Min(this.writeBufferSize - totalBytesAvailable, bytesToWrite);
            }

            this.writePipe.Write(buffer, bytesWritten, bytesToWrite);
            numberOfBytesWritten = bytesToWrite;

            this.writeEvent.Set();
        }
    }

    private void Initialize()
    {
        this.writeBufferSize = this.writePipe.OutBufferSize;
        if (this.writeBufferSize == 0)
        {
            // The writeBufferSize is sometimes zero. We can use the read pipe's input buffer size instead.
            this.writeBufferSize = this.readPipe.InBufferSize;
        }
        Debug.Assert(this.writeBufferSize > 0, "If we can not get the writeBufferSize sending large messages will deadlock");
    }

    private enum ConditionResult
    {
        /// <summary>
        /// The condition that the caller was waiting for has occurred.
        /// </summary>
        Success,
        /// <summary>
        /// The condition that the caller is waiting for has not occurred yet.
        /// </summary>
        NotReadyYet,
        /// <summary>
        /// It's time to shutdown the app.
        /// </summary>
        Shutdown
    }

    /// <summary>
    /// Waits for the specified condition to become true.
    /// </summary>
    /// <param name="waitHandle">Event that signals we should check the condition again.</param>
    /// <param name="condition">Condition we're waiting for.</param>
    /// <returns>A ConditionResult indicating what happened.</returns>
    private bool WaitForCondition(WaitHandle waitHandle, Func<ConditionResult> condition)
    {
        while (true)
        {
            ConditionResult conditionResult = condition();

            switch (conditionResult)
            {
                case ConditionResult.Success:
                    return true;
                case ConditionResult.NotReadyYet:
                    {
                        if (!waitHandle.WaitOne())
                        {
                            return false;
                        }
                        break;
                    }
                case ConditionResult.Shutdown:
                default:
                    return false;
            }
        }
    }

    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public void Dispose()
    {
        this.readEvent.Dispose();
        this.readPipe.Dispose();
        this.writeEvent.Dispose();
        this.writePipe.Dispose();
        GC.SuppressFinalize(this);
    }
}

public sealed class PipeDataBridge : IDataBridge, IDisposable
{
    private readonly AnonymousPipe readPipe;
    private readonly AnonymousPipe writePipe;
    private bool isFirstMessage = true;
    private bool closed;

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, IntPtr hSourceHandle, IntPtr hTargetProcessHandle, out IntPtr lpTargetHandle, uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwOptions);

    public PipeDataBridge()
    {
        this.readPipe = new AnonymousPipe();
        this.writePipe = new AnonymousPipe();
    }

    public PipeDataBridge(string initializationData)
    {
        IntPtr[] handles = initializationData
            .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(value => (IntPtr)ulong.Parse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture))
            .ToArray();
        this.readPipe = new AnonymousPipe(handles[0], handles[1], handles[2], handles[3]);
        this.writePipe = new AnonymousPipe(handles[4], handles[5], handles[6], handles[7]);
    }

    /// <summary>
    /// Creates a string containing duplicated handle ids which can be used by the 
    /// remote endpoint to connect to the pipes.
    /// </summary>
    /// <param name="targetProcessId">Id of the process that will be connecting.</param>
    /// <returns>The serialized pipe info.</returns>
    public string Serialize(int targetProcessId)
    {
        Process sourceProcess = Process.GetCurrentProcess();
        Process targetProcess = Process.GetProcessById(targetProcessId);
        Func<IntPtr, IntPtr> duplicate = handle => PipeDataBridge.DuplicateHandle(sourceProcess.Handle, targetProcess.Handle, handle);
        return this.Serialize(duplicate);
    }

    public string Serialize(Func<IntPtr, IntPtr> duplicateHandle)
    {
        StringBuilder result = new StringBuilder();
        result.Append(this.writePipe.Serialize(duplicateHandle));
        result.Append(this.readPipe.Serialize(duplicateHandle));
        return result.ToString();
    }

    #region IDataBridge implementation

    public ManualResetEvent ReadyEvent { get; } = new ManualResetEvent(true);
    public ManualResetEvent FirstMessageEvent { get; } = new ManualResetEvent(false);

    public void Close()
    {
        this.closed = true;
        this.readPipe.Close();
        this.writePipe.Close();
    }

    public byte[] ReadMessage()
    {
        // read length
        const int LengthSize = sizeof(int);
        byte[] buffer = new byte[LengthSize];
        this.FillBufferFromReadPipe(buffer, 0);
        if (this.closed)
        {
            return null;
        }

        // read body
        int dataLength = BitConverter.ToInt32(buffer, 0);
        Array.Resize(ref buffer, LengthSize + dataLength);
        this.FillBufferFromReadPipe(buffer, LengthSize);
        if (this.closed)
        {
            return null;
        }

        if (this.isFirstMessage)
        {
            this.isFirstMessage = false;
            this.FirstMessageEvent.Set();
        }
        return buffer;
    }

    private void FillBufferFromReadPipe(byte[] buffer, int startingOffset)
    {
        int offset = startingOffset;

        while (offset < buffer.Length)
        {
            int bytesRead = this.readPipe.Read(buffer, offset, buffer.Length - offset);
            if (this.closed)
            {
                return;
            }

            offset += bytesRead;
        }
    }

    public void WriteMessage(byte[] buffer)
    {
        this.writePipe.Write(buffer);
    }

    #endregion

    [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed")]
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public void Dispose()
    {
        this.ReadyEvent.Dispose();
        this.FirstMessageEvent.Dispose();
        this.readPipe.Dispose();
        this.writePipe.Dispose();
        GC.SuppressFinalize(this);
    }

    private static IntPtr DuplicateHandle(IntPtr sourceProcess, IntPtr targetProcess, IntPtr handle)
    {
        const int DUPLICATE_SAME_ACCESS = 2;
        IntPtr duplicate;
        PipeDataBridge.DuplicateHandle(sourceProcess, handle, targetProcess, out duplicate, 0, true, DUPLICATE_SAME_ACCESS);
        return duplicate;
    }
}
/// <summary>
/// Data bridges encapsulate a particular interprocess communication mechanism.
/// They are used by ProtocolHandlers to send/receive messages.
/// </summary>
public interface IDataBridge
{
    /// <summary>
    /// Returns a WaitHandle that will be signalled when the DataBridge
    /// is ready for use.
    /// </summary>
    ManualResetEvent ReadyEvent { get; }

    /// <summary>
    /// Wait handle that will be signaled when first message is read
    /// which usualy indicates that other side is fully initialized.
    /// </summary>
    ManualResetEvent FirstMessageEvent { get; }

    /// <summary>
    /// Returns a buffer containing the next message received from the remote endpoint.
    /// Blocks until a message is received or Close is called.
    /// Read should not be called until the DataBridge is "ready".
    /// </summary>
    /// <returns>
    /// Buffer containing a serialized message or null if the data bridge was closed.
    /// </returns>
    byte[] ReadMessage();

    /// <summary>
    /// Sends the specified buffer of data to the remote endpoint.  Blocks until
    /// the send is complete.  Write should not be called until the DataBridge is
    /// "ready".
    /// </summary>
    void WriteMessage(byte[] buffer);

    /// <summary>
    /// Cancels any pending I/O requests.  The destructor should take care
    /// of freeing any allocated resources.
    /// </summary>
    void Close();
}

/// <summary>
/// Enables sending and receiving messages via an IDataBridge.
/// </summary>
[DebuggerDisplay("send=T-{this.sendThread.ManagedThreadId}, read=T-{this.readThread != null ? this.readThread.ManagedThreadId : 0}," +
                 "process=T-{this.processThread != null ? this.processThread.ManagedThreadId : 0}{IsShutdown ? \", SHUTDOWN\":\"\",nq}")]
public class ProtocolHandler : IProtocolHandler, IDisposable
{
    private delegate void CancellableAction(bool isCancelled);

    private static int NextId;
    private int id = Interlocked.Increment(ref ProtocolHandler.NextId);
    private IDataBridge dataBridge;
    private CancellationTokenSource tokenSource;
    private readonly object shutdownAndPendingRequestsLock = new();
    private Thread readThread;
    private Thread processThread;
    private Thread sendThread;
    private readonly WaitableActionsList inboundActions = new("ProtocolHandler.ProcessThread");
    private readonly WaitableActionsList outboundActions = new("ProtocolHandler.SendThread");
    private readonly Dictionary<int, PendingRequest> pendingRequests = new();
    private readonly Dictionary<int, List<ActionWithId>> messageObservers = new();
    private event EventHandler shutdownStarted;

    /// <summary>
    /// This is a hook to enable tests to inspect the message queue of a fully constructed environment, because it is not clear how to fully
    /// replicate the environment we need in order to test anti-flickering code. Delete this if a simulation can be created.
    /// </summary>
    public event EventHandler MessagePosted;

    public ProtocolHandler(IDataBridge dataBridge, CancellationTokenSource tokenSource = null, bool shouldStart = true)
    {
        Console.WriteLine($"{this.id} New");
        this.dataBridge = dataBridge ?? throw new ArgumentNullException(nameof(dataBridge));
        this.tokenSource = tokenSource ?? new CancellationTokenSource();
        this.sendThread = new Thread(this.ActionThread);
        this.sendThread.IsBackground = true;
        this.sendThread.Start(this.outboundActions);

        if (shouldStart)
        {
            this.Start();
        }
    }

    public event EventHandler<Exception> OnUnhandledException;

    public event EventHandler ShutdownStarted {
        add {
            // If we already shutdown invoke the callback immediately.
            lock (this.shutdownAndPendingRequestsLock)
            {
                if (!this.IsShutdown)
                {
                    this.shutdownStarted += value;
                    return;
                }
            }

            value(this, EventArgs.Empty);
        }

        remove {
            lock (this.shutdownAndPendingRequestsLock)
            {
                this.shutdownStarted -= value;
            }
        }
    }

    public CancellationToken CancellationToken => this.tokenSource.Token;

    /// <summary>
    /// Checks if the IPC connection has been shutdown.  Probably only useful for unit tests.
    /// </summary>
    public bool IsShutdown { get; private set; }

    /// <summary>
    /// Sends a message of the specified type containing the specified jsonObject as its payload.
    /// Use this in fire and forget cases when no reply is expected.
    /// </summary>
    /// <param name="messageType">Type of message to send.</param>
    /// <param name="jsonObject">Object to serialize and use as message payload.</param>
    public void PostMessage(int messageType, object jsonObject)
    {
        Message message = new Message(messageType, jsonObject);
        this.PostMessage(message);
    }

    /// <summary>
    /// Asynchronously sends a message of the specified messageType containing the specified 
    /// jsonObject as its payload and waits for its reply.  The returned task will complete 
    /// when the reply is received.
    /// </summary>
    /// <param name="messageType">Type of message to send.</param>
    /// <param name="jsonObject">Object to serialize and use as message payload.</param>
    /// <returns>A task that will complete when a reply is received for the sent message.</returns>
    /// <typeparam name="TReply">Type to deserialize the reply's payload into.</typeparam>
    /// <remarks>
    /// There's no requirement that SendMessageAsync always be called from the same thread.
    /// The calling code is free to call this method from any thread, even multiple threads.
    /// </remarks>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public Task<TReply> SendMessageAsync<TReply>(int messageType, object jsonObject) where TReply : class
    {
        Message.VerifyTypeIsSerializable(typeof(TReply));

        Message message = new(messageType, jsonObject);

        TaskCompletionSource<TReply> replyCompletionSource = new();

        TReply result = null;
        PendingRequest pendingRequest = new(() => replyCompletionSource.TrySetCanceled(), (reply) =>
        {
            try
            {
                result = reply.ConvertPayloadToModel<TReply>();
            }
            catch (Exception ex)
            {
                replyCompletionSource.TrySetException(ex);
                return;
            }

            if (result == null)
            {
                // If we get a bad reply cancel the task that requested the data
                replyCompletionSource.TrySetCanceled();
            }
            else
            {
                replyCompletionSource.TrySetResult(result);
            }
        });

        bool cancelPendingRequest = false;

        lock (this.shutdownAndPendingRequestsLock)
        {
            if (!this.tokenSource.IsCancellationRequested)
            {
                this.pendingRequests.Add(message.Id, pendingRequest);

                this.PushMessageOnToSendQueue(message);
            }
            else
            {
                cancelPendingRequest = true;
            }
        }

        if (cancelPendingRequest)
        {
            pendingRequest.Cancel();
        }

        return replyCompletionSource.Task;
    }

    public int RegisterMessageObserver<TMessage>(int messageType, Action<TMessage> callback) where TMessage : class
    {
        if (callback == null)
        {
            throw new ArgumentNullException(nameof(callback));
        }

        return RegisterMessageObserver<TMessage, ResponseWithError>(messageType, message =>
        {
            callback(message);
            return null;
        });
    }

    public int RegisterMessageObserver<TMessage, TReply>(int messageType, Func<TMessage, TReply> callback)
        where TMessage : class
        where TReply : class
    {
        Message.VerifyTypeIsSerializable(typeof(TMessage));
        Message.VerifyTypeIsSerializable(typeof(TReply));
        return this.RegisterMessageObserverRaw(messageType, message => this.HandleMessage<TMessage, TReply>(callback, message));
    }

    public int RegisterAsyncMessageObserver<TMessage, TReply>(int messageType, Func<TMessage, Task<TReply>> callback)
        where TMessage : class
        where TReply : class
    {
        Message.VerifyTypeIsSerializable(typeof(TMessage));
        Message.VerifyTypeIsSerializable(typeof(TReply));
        return this.RegisterMessageObserverRaw(messageType, message => this.HandleAsyncMessage<TMessage, TReply>(callback, message));
    }

    private int RegisterMessageObserverRaw(int messageType, Action<Message> observer)
    {
        if (observer == null)
        {
            throw new ArgumentNullException(nameof(observer));
        }

        lock (this.messageObservers)
        {
            List<ActionWithId> existingObservers;
            if (!this.messageObservers.TryGetValue(messageType, out existingObservers))
            {
                existingObservers = new List<ActionWithId>();
                this.messageObservers.Add(messageType, existingObservers);
            }

            int id = Interlocked.Increment(ref ProtocolHandler.NextId);
            existingObservers.Add(new ActionWithId(id, observer));
            Console.WriteLine($"{this.id} Registered observer for {messageType}");

            return id;
        }
    }

    public bool UnregisterMessageObserver(int registrationId)
    {
        lock (this.messageObservers)
        {
            foreach (List<ActionWithId> existingObservers in this.messageObservers.Values)
            {
                int index = existingObservers.FindIndex(a => a.ID == registrationId);
                if (index >= 0)
                {
                    existingObservers.RemoveAt(index);
                    return true;
                }
            }
        }

        return false;
    }

    public Task FlushInboundMessageQueueAsync() => this.inboundActions.FlushMessageQueueAsync();
    public Task FlushOutboundMessageQueueAsync() => this.outboundActions.FlushMessageQueueAsync();

    public void Start()
    {
        lock (this.shutdownAndPendingRequestsLock)
        {
            if (this.tokenSource.IsCancellationRequested || this.readThread != null)
            {
                return;
            }

            this.readThread = new Thread(this.ReadThread);
            this.readThread.IsBackground = true;
            this.readThread.Start();

            this.processThread = new Thread(this.ActionThread);
            this.processThread.IsBackground = true;
            this.processThread.Start(this.inboundActions);
        }
    }

    private void HandleMessage<TMessage, TReply>(Func<TMessage, TReply> callback, Message request)
        where TMessage : class
        where TReply : class
    {
        TMessage parsed = request.ConvertPayloadToModel<TMessage>();
        if (parsed == null)
        {
            return;
        }

        TReply reply;
        try
        {
            reply = callback(parsed);
        }
        catch (TaskCanceledException)
        {
            // TaskCanceled normally means we're shutting down. We could try sending the TaskCanceled exception back
            // to the other side, but we already have special shutdown logic.
            return;
        }

        this.PostReply(reply, request);
    }

    private void HandleAsyncMessage<TMessage, TReply>(Func<TMessage, Task<TReply>> callback, Message request)
    {
        TMessage parsed = request.ConvertPayloadToModel<TMessage>();
        if (parsed == null)
        {
            return;
        }

        Task<TReply> task = callback(parsed);
        task.ContinueWith(_ =>
        {
            this.PostReply(task.Result, request);
        }, this.CancellationToken, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
        task.ContinueWith(_ =>
        {
            this.OnUnhandledException?.Invoke(this, task.Exception);
        }, this.CancellationToken, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
    }

    private void PostReply<TReply>(TReply reply, Message request)
    {
        if (reply == null)
        {
            return;
        }

        Message replyMessage = new(MessageType.Reply, reply, request);
        this.PostMessage(replyMessage);
    }

    private void PostMessage(Message message)
    {
        lock (this.shutdownAndPendingRequestsLock)
        {
            if (!this.IsShutdown)
            {
                this.PushMessageOnToSendQueue(message);
            }
        }
    }

    /// <summary>
    /// Adds the specified message onto the send queue.
    /// </summary>
    /// <param name="message">The message to send.</param>
    private void PushMessageOnToSendQueue(Message message)
    {
        Console.WriteLine(this.GetMessageDescription("Posting", message));
        byte[] data = message.BufferForSend;
        this.outboundActions.AddAndSignal(() =>
        {
            try
            {
                this.dataBridge.WriteMessage(data);
            }
            catch (IOException)
            {
                this.Shutdown();
            }
        });

        // it is unusual to abuse the sender parameter like this, but given that this is for debug purposes only,
        // it should be fine. Add a real EventArgs subclass if this gets used for non-test purposes.
        // No point expanding the footprint otherwise.
        this.MessagePosted?.Invoke(message, EventArgs.Empty);
    }

    public void Shutdown()
    {
        EventHandler shutdownStarted;
        lock (this.shutdownAndPendingRequestsLock)
        {
            // We can't just look at the cancel source here since it might be canceled somewhere else
            // and we still need to shut down properly.
            if (this.IsShutdown)
            {
                return; // already shut down
            }

            this.IsShutdown = true;
            this.tokenSource.Cancel();
            shutdownStarted = this.shutdownStarted;
        }

        shutdownStarted?.Invoke(this, EventArgs.Empty);

        try
        {
            this.dataBridge.ReadyEvent.Set();
            this.dataBridge.Close();
        }
        catch (IOException ex)
        {
            // Some Watson crashes point to issues with closing event handlers.
            // Investigate if we hit this assert.
            Debug.Fail(ex.ToString());
        }

        // Shutdown threads
        this.outboundActions.Shutdown(this.sendThread);
        this.inboundActions.Shutdown(this.processThread);

        // Cancel requests before joining with processThread in case processThread
        // is blocked waiting for a request to complete.
        lock (this.shutdownAndPendingRequestsLock)
        {
            foreach (PendingRequest pendingRequest in this.pendingRequests.Values)
            {
                pendingRequest.Cancel();
            }

            this.pendingRequests.Clear();
        }

        lock (this.messageObservers)
        {
            this.messageObservers.Clear();
        }

        Console.WriteLine($"{this.id} Shutdown complete");
    }

    /// <summary>
    /// Called when a message is received from the data bridge.  Dispatches it to
    /// any relevant observers.
    /// </summary>
    /// <param name="buffer">Byte array containing message received.</param>
    /// <remarks>
    /// The DataBridge implementation determines which thread(s) this method is called on.
    /// ProtocolHandler makes no assumptions about this and assumes that OnMessageReceived
    /// may be called by multiple threads simultaneously.
    /// </remarks>
    private void OnMessageReceived(byte[] buffer)
    {
        Message message = new(buffer);
        Console.WriteLine(this.GetMessageDescription("Received", message));

        PendingRequest pendingRequest = null;
        lock (this.shutdownAndPendingRequestsLock)
        {
            if (this.pendingRequests.TryGetValue(message.RequestId, out pendingRequest))
            {
                this.pendingRequests.Remove(message.RequestId);
            }
        }

        if (pendingRequest != null)
        {
            pendingRequest.OnReplyReceived(message);
            return;
        }

        this.inboundActions.AddAndSignal(() => this.ProcessInboundMessage(message));
    }

    private void ProcessInboundMessage(Message message)
    {
        Console.WriteLine(this.GetMessageDescription("Processing message", message));
        List<ActionWithId> observers = null;
        lock (this.messageObservers)
        {
            if (this.messageObservers.TryGetValue(message.MessageType, out observers))
            {
                // Copy the observers because an observer may want to add/modify observers
                // during processing.
                observers = new List<ActionWithId>(observers);
            }
        }

        if (observers != null)
        {
            foreach (ActionWithId observer in observers)
            {
                observer.Action.Invoke(message);
            }
        }
    }

    private void ReadThread()
    {
        Console.WriteLine($"{this.id} ReadThread started");
        Thread.CurrentThread.Name = "ProtocolHandler.ReadThread";
        this.dataBridge.ReadyEvent.WaitOne();

        while (!this.tokenSource.IsCancellationRequested)
        {
            byte[] buffer;

            try
            {
                buffer = this.dataBridge.ReadMessage();
            }
            catch (IOException)
            {
                this.Shutdown();
                break;
            }

            if (buffer != null)
            {
                int dataLength = BitConverter.ToInt32(buffer, 0);

                if (dataLength < Message.HeaderSizeWithoutLength)
                {
                    Debug.Fail("Received message shorter than min length.  Stream must be corrupt.  Shutting down.");
                    this.Shutdown();
                    break;
                }

                this.OnMessageReceived(buffer);
            }
        }

        Console.WriteLine($"{this.id} ReadThread ended");
    }

    private void ActionThread(object list)
    {
        WaitableActionsList actionList = (WaitableActionsList)list;
        Console.WriteLine($"{this.id} {actionList.ThreadName} started");
        Thread.CurrentThread.Name = actionList.ThreadName;
        this.dataBridge.ReadyEvent.WaitOne();

        while (!this.tokenSource.IsCancellationRequested)
        {
            IEnumerable<CancellableAction> actions = actionList.WaitForData();
            foreach (CancellableAction action in actions)
            {
                action(isCancelled: this.IsShutdown);
            }
        }

        Console.WriteLine($"{this.id} {actionList.ThreadName} ended");
    }

    private FormattableString GetMessageDescription(string tag, Message message)
    {
        if (message == null)
        {
            return $"{this.id} {tag}";
        }

        if (message.RequestId == Message.InvalidId)
        {
            // Example: "SetPanZoomTransformRequest #25, bytes=76"
            return $"{this.id} {tag} {message.MessageType} #{message.Id}, bytes={message.BufferForSend?.Length ?? 0}";
        }
        else
        {
            // Example: "Reply #80 to #25, bytes=15"
            return $"{this.id} {tag} {message.MessageType} #{message.Id} to #{message.RequestId}, bytes={message.BufferForSend?.Length ?? 0}";
        }
    }

    #region IDisposable

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    [SuppressMessage("Design", "CA1063:Implement IDisposable Correctly")]
    private void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            this.Shutdown();
        }
    }

    #endregion

    [DebuggerDisplay("{this.ThreadName,nq}, count={data.Count}, alive={!isShutdown}")]
    private class WaitableActionsList
    {
        private List<CancellableAction> data = new();
        private readonly object syncObject = new();
        private bool isShutdown;

        public WaitableActionsList(string threadName)
        {
            this.ThreadName = threadName;
        }

        public string ThreadName { get; }

        public IEnumerable<CancellableAction> WaitForData()
        {
            lock (this.syncObject)
            {
                while (this.data.Count == 0 && !this.isShutdown)
                {
                    Monitor.Wait(this.syncObject);
                }

                if (this.isShutdown)
                {
                    return Enumerable.Empty<CancellableAction>();
                }

                List<CancellableAction> result = this.data;
                this.data = new List<CancellableAction>();
                return result;
            }
        }

        public void AddAndSignal(Action action)
        {
            this.AddAndSignal(isCancelled =>
            {
                if (!isCancelled)
                {
                    action();
                }
            });
        }

        public Task FlushMessageQueueAsync()
        {
            TaskCompletionSource<bool> taskCompletion = new();
            CancellableAction action = isCancelled =>
            {
                if (isCancelled)
                {
                    taskCompletion.TrySetCanceled();
                }
                else
                {
                    taskCompletion.TrySetResult(true);
                }
            };

            this.AddAndSignal(action);
            return taskCompletion.Task;
        }

        private void AddAndSignal(CancellableAction action)
        {
            lock (this.syncObject)
            {
                if (!this.isShutdown)
                {
                    this.data.Add(action);
                    Monitor.Pulse(this.syncObject);
                }
                else
                {
                    action(isCancelled: true);
                }
            }
        }

        public void Shutdown(Thread processingThread)
        {
            lock (this.syncObject)
            {
                if (this.isShutdown)
                {
                    return;
                }

                this.isShutdown = true;
                Monitor.Pulse(this.syncObject);
            }

            // We ASSUME that shutdown action is "simple" and does not cause
            // reentrancy into this class.
            this.data.ForEach(a => a(isCancelled: true));
            this.data.Clear();
        }
    }

    private struct ActionWithId
    {
        public ActionWithId(int id, Action<Message> action)
        {
            this.ID = id;
            this.Action = action;
        }

        public int ID { get; }
        public Action<Message> Action { get; }
    }
}

/// <summary>
/// Wraps state related to a single pending ProtocolHandler request.
/// </summary>
public class PendingRequest
{
    private readonly Action cancel;
    private readonly Action<Message> replyAction;

    public PendingRequest(Action cancel, Action<Message> replyAction)
    {
        this.cancel = cancel;
        this.replyAction = replyAction;
    }

    /// <summary>
    /// Called when a reply is received for this request.  Passes the response to its handler.
    /// </summary>
    public void OnReplyReceived(Message reply)
    {
        this.replyAction(reply);
    }

    /// <summary>
    /// Cancels the task associated with the pending request.
    /// </summary>
    public void Cancel()
    {
        this.cancel();
    }
}

public interface IProtocolHandler
{
    /// <summary>
    /// Fired if an asynchronous event handler throws an exception.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    event EventHandler<Exception> OnUnhandledException;
    event EventHandler ShutdownStarted;

    /// <summary>
    /// Cancellation token which gets canceled when pipeline shuts down.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Sends a message of the specified type containing the specified jsonObject as its payload.
    /// Use this in fire and forget cases when no reply is expected.
    /// </summary>
    /// <param name="messageType">Type of message to send.</param>
    /// <param name="jsonObject">Object to serialize and use as message payload.</param>
    [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames")]
    void PostMessage(int messageType, object jsonObject);

    /// <summary>
    /// Asynchronously sends a message of the specified messageType containing the specified 
    /// jsonObject as its payload and waits for its reply.  The returned task will complete 
    /// when the reply is received.
    /// </summary>
    /// <param name="messageType">Type of message to send.</param>
    /// <param name="jsonObject">Object to serialize and use as message payload.</param>
    /// <returns>A task that will complete when a reply is received for the sent message.</returns>
    /// <typeparam name="TReply">Type to deserialize the reply's payload into.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames")]
    Task<TReply> SendMessageAsync<TReply>(int messageType, object jsonObject) where TReply : class;

    /// <summary>
    /// Returns task which completes when all inbound messages which were queued
    /// at the moment of calling this method are processed.
    /// </summary>
    Task FlushInboundMessageQueueAsync();

    /// <summary>
    /// Returns task which completes when all inbound messages which were queued
    /// at the moment of calling this method are processed.
    /// </summary>
    Task FlushOutboundMessageQueueAsync();

    /// <summary>
    /// Registers the specified callback to be called whenever a message of the specified messageType is
    /// received. The callback can be called multiple times.
    /// </summary>
    /// <param name="messageType">Type of message that should cause the callback to be called.</param>
    /// <param name="callback">Delegate to call when messages of the specified type are received.</param>
    /// <typeparam name="TMessage">Type to deserialize the message's payload into.</typeparam>
    int RegisterMessageObserver<TMessage>(int messageType, Action<TMessage> callback) where TMessage : class;

    /// <summary>
    /// Registers the specified callback to be called whenever a message of the specified messageType is
    /// received. The callback can be called multiple times.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TReply"></typeparam>
    /// <param name="messageType">Type of message that should cause the callback to be called.</param>
    /// <param name="callback">Function to call when messages of the specified type are received. Returns the MessageType/object for the reply, or null if no reply should be sent.</param>
    int RegisterMessageObserver<TMessage, TReply>(int messageType, Func<TMessage, TReply> callback)
        where TMessage : class
        where TReply : class;

    int RegisterAsyncMessageObserver<TMessage, TReply>(int messageType, Func<TMessage, Task<TReply>> callback)
        where TMessage : class
        where TReply : class;

    /// <summary>
    /// Unregisters message observer.
    /// </summary>
    /// <param name="registrationId">ID returned by one of RegisterXxx methods.</param>
    bool UnregisterMessageObserver(int registrationId);

    /// <summary>
    /// Start processing incoming messages. Indicates that all message observers have been registered.
    /// </summary>
    void Start();
}

/// <summary>
/// A base class for responses that can return error codes.
/// </summary>
[DataContract]
public class ResponseWithError
{
    [DataMember]
    public int HResult;

    [DataMember]
    public string Error;

    /// <summary>
    /// Throws an exception if the response contains an error.
    /// </summary>
    /// <remarks>
    /// We should probably automatically throw on error, but there's too much code that
    /// assumes we don't throw.
    /// </remarks>
    public void ThrowIfFailed()
    {
        if (this.HResult == 0)
        {
            return;
        }

        Exception hrException = Marshal.GetExceptionForHR(this.HResult);
        if (string.IsNullOrEmpty(this.Error))
        {
            throw hrException;
        }

        throw new AggregateException(this.Error, hrException);
    }
}

[DataContract]
public class EmptyRequestInfo { }

[DataContract]
public class EmptyResponse { }

public static class MessageType
{
    /// <summary>
    /// Invalid message type intended for use in unit tests.
    /// </summary>
    public const int Invalid = 0;


    /// <summary>
    /// Indicates the message is a reply to a request.
    /// </summary>
    public const int Reply = 1;
}

public interface IBinarySerialization
{
    int Deserialize(int startingIndex, byte[] data);
    void Serialize(Stream stream);
}

public class Message
{
    /// <summary>
    /// Value which is invalid for the id field.
    /// </summary>
    public const int InvalidId = -1;

    /// <summary>
    /// Size of header in bytes excluding the length field.
    /// </summary>
    public const int HeaderSizeWithoutLength = 12;

    /// <summary>
    /// Size of length field in bytes.
    /// </summary>
    public const int LengthSize = 4;

    /// <summary>
    /// Size of header in bytes including the length field.
    /// </summary>
    public const int HeaderSizeWithLength = Message.HeaderSizeWithoutLength + Message.LengthSize;

    /// <summary>
    /// Zero-based index into byte array where length field begins.
    /// </summary>
    public const int LengthOffset = 0;

    /// <summary>
    /// Zero-based index into byte array where message type field begins.
    /// </summary>
    public const int MessageTypeOffset = 4;

    /// <summary>
    /// Zero-based index into byte array where id field begins.
    /// </summary>
    public const int IdOffset = 8;

    /// <summary>
    /// Zero-based index into byte array where request id field begins.
    /// </summary>
    public const int RequestIdOffset = 12;

    /// <summary>
    /// Zero-based index into byte array where payload begins.
    /// </summary>
    public const int PayloadOffset = Message.HeaderSizeWithLength;

    /// <summary>
    /// Buffer used to initialize a MemoryStream with room for the header.
    /// </summary>
    private static readonly byte[] InitialHeaderBuffer = new byte[Message.HeaderSizeWithLength];

    /// <summary>
    /// Id to assign to next Message constructed by this endpoint.
    /// </summary>
    private static int nextId = 0;

    /// <summary>
    /// Byte array containing serialized message.
    /// </summary>
    private byte[] buffer;

    /// <summary>
    /// Gets a MessageType value that identifies the purpose of the message and indicates how the 
    /// payload should be interpreted.  For example, it may tell the receiver how to deserialize
    /// the payload.
    /// </summary>
    public int MessageType => BitConverter.ToInt32(this.buffer, Message.MessageTypeOffset);

    /// <summary>
    /// Gets a numeric value that identifies the message instance.  The sender is responsible for
    /// setting the id to a "relatively unique" value.
    /// </summary>
    /// <remarks>
    /// It is expected that in long runs of the tool the id could wrap around, but the previous
    /// instance of message id 1 (for example) will have been sent so far in the past that it
    /// will be irrelevant.
    /// 
    /// Message.InvalidId is considered invalid and will never be used in the id field.
    /// </remarks>
    public int Id => BitConverter.ToInt32(this.buffer, Message.IdOffset);

    /// <summary>
    /// Gets the id of the message to which the message is replying.
    /// </summary>
    /// <remarks>
    /// RequestId is only relevant for reply messages.  It enables a node (ex. the tool) to match 
    /// requests and replies -- for example, "I know that I sent a request for Button1's properties 
    /// in message id 100, so I can wait for an incoming message with request id 100 and know that 
    /// it will contain the button's properties."
    /// 
    /// Non-reply messages (ex. requests and events) will set RequestId to Message.InvalidId.
    /// </remarks>
    public int RequestId => BitConverter.ToInt32(this.buffer, Message.RequestIdOffset);

    /// <summary>
    /// Gets serialized representation of the Message in a byte array to be sent to
    /// a remote endpoint.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
    public byte[] BufferForSend => this.buffer;

    /// <summary>
    /// Constructs a new message for sending that is not a reply.
    /// </summary>
    /// <param name="messageType">Type of message to send.</param>
    /// <param name="payload">JSON object to serialize and send as payload.</param>
    public Message(int messageType, object payload)
    {
        this.SerializeToBuffer(messageType, Message.InvalidId, payload);
    }

    /// <summary>
    /// Constructs a new message for sending as a reply to the specified request.
    /// </summary>
    /// <param name="messageType">Type of message to send.</param>
    /// <param name="payload">JSON object to serialize and send as payload.</param>
    /// <param name="request">Message from the remote endpoint that this new message is replying to.</param>
    public Message(int messageType, object payload, Message request)
    {
        this.SerializeToBuffer(messageType, request.Id, payload);
    }

    /// <summary>
    /// Parses the specified buffer of received data into a message.
    /// </summary>
    /// <param name="buffer">Byte array containing a serialized message.</param>
    public Message(byte[] buffer)
    {
        this.buffer = buffer;

#if DEBUG
        int messageLength = BitConverter.ToInt32(this.buffer, Message.LengthOffset);
        Debug.Assert(this.buffer.Length == Message.LengthSize + messageLength);
#endif
    }


    /// <summary>
    ///  Convert the message payload into a model object. The payload can either be 
    ///  JSON or Binary
    /// </summary>
    /// <typeparam name="ModelT"></typeparam>
    /// <returns></returns>
    [SuppressMessage("Microsoft.Naming", "CA1715")]
    public ModelT ConvertPayloadToModel<ModelT>()
    {
        if (typeof(IBinarySerialization).IsAssignableFrom(typeof(ModelT)))
        {
            return this.ConvertBinaryPayloadToModel<ModelT>();
        }
        else
        {
            return this.ConvertJsonPayloadToModel<ModelT>();
        }
    }


    /// <summary>
    /// Convert a binary payload to a model through IBinarySerialization
    /// </summary>
    /// <typeparam name="ModelT"></typeparam>
    /// <returns></returns>
    private ModelT ConvertBinaryPayloadToModel<ModelT>()
    {
        try
        {
            ModelT model = Activator.CreateInstance<ModelT>();
            ((IBinarySerialization)model).Deserialize(Message.PayloadOffset, this.buffer);
            return model;
        }
        catch (SerializationException)
        {
            return default(ModelT);
        }
        catch (ArgumentOutOfRangeException)
        {
            return default(ModelT);
        }
    }

    /// <summary>
    /// Attempts to deserialize the payload of the message into the specified JSON object type.
    /// </summary>
    /// <typeparam name="ModelT">Type to deserialize into.</typeparam>
    /// <returns>New instance of JsonT created by deserializing the message's payload or null if unable to convert the message to the requested type.</returns>
    [SuppressMessage("Microsoft.Naming", "CA1715")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public ModelT ConvertJsonPayloadToModel<ModelT>()
    {
        DataContractJsonSerializer jsonDeserializer = new DataContractJsonSerializer(typeof(ModelT));

        MemoryStream payloadStream = this.HasContent
            ? new MemoryStream(this.buffer, Message.PayloadOffset, this.buffer.Length - Message.PayloadOffset)
            : new MemoryStream(Encoding.Unicode.GetBytes("{}"));
        XmlDictionaryReader jsonReader = JsonReaderWriterFactory.CreateJsonReader(payloadStream, Encoding.Unicode, XmlDictionaryReaderQuotas.Max, null);

        try
        {
            return (ModelT)jsonDeserializer.ReadObject(jsonReader);
        }
        catch
        {
            return default(ModelT);
        }
    }

    /// <summary>
    /// Sets the id that should be assigned to the next outbound message.
    /// The "next id" will continue to increment as new messages are created.
    /// </summary>
    /// <param name="nextId">Id that should be assigned to the next locally created Message.</param>
    public static void SetNextId(int nextId)
    {
        Message.nextId = nextId;
    }

    /// <summary>
    /// Gets a value indicating the existence of a payload.
    /// </summary>
    private bool HasContent {
        get { return this.buffer.Length > Message.HeaderSizeWithLength; }
    }

    /// <summary>
    /// Gets a message id to use for the next outbound message.
    /// </summary>
    /// <returns>Id to use for next outbound message.</returns>
    private static int AllocateMessageId()
    {
        int id = Interlocked.Increment(ref Message.nextId) - 1;

        if (id == Message.InvalidId)
        {
            id = Interlocked.Increment(ref Message.nextId) - 1;
        }

        return id;
    }

    private void SerializeToBuffer(int messageType, int requestId, object payload)
    {
        // We can't use the byte[] constructor, because that constructor creates a non-expandable MemoryStream
        MemoryStream payloadMemoryStream = new MemoryStream();
        payloadMemoryStream.Write(Message.InitialHeaderBuffer, 0, Message.InitialHeaderBuffer.Length);

        if (payload is not IBinarySerialization serializable)
        {
            Type payloadType = payload.GetType();
            Message.VerifyTypeIsSerializable(payloadType);
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(payloadType);
            XmlDictionaryWriter jsonWriter = JsonReaderWriterFactory.CreateJsonWriter(payloadMemoryStream, Encoding.Unicode, ownsStream: false);
            jsonSerializer.WriteObject(jsonWriter, payload);
            jsonWriter.Flush();
        }
        else
        {
            serializable.Serialize(payloadMemoryStream);
        }

        this.buffer = payloadMemoryStream.GetBuffer();
        Array.Resize(ref this.buffer, (int)payloadMemoryStream.Length);

        int messageLength = this.buffer.Length - Message.LengthSize;
        byte[] lengthBytes = BitConverter.GetBytes(messageLength);
        byte[] messageIdBytes = BitConverter.GetBytes(messageType);
        byte[] idBytes = BitConverter.GetBytes(Message.AllocateMessageId());
        byte[] requestIdBytes = BitConverter.GetBytes(requestId);

        Array.Copy(lengthBytes, 0, this.buffer, Message.LengthOffset, lengthBytes.Length);
        Array.Copy(messageIdBytes, 0, this.buffer, Message.MessageTypeOffset, messageIdBytes.Length);
        Array.Copy(idBytes, 0, this.buffer, Message.IdOffset, idBytes.Length);
        Array.Copy(requestIdBytes, 0, this.buffer, Message.RequestIdOffset, requestIdBytes.Length);
    }

    [Conditional("DEBUG")]
    public static void VerifyTypeIsSerializable(Type type)
    {
        Type typeToVerity = type.IsArray ? type.GetElementType() : type;
        if (typeof(IBinarySerialization).IsAssignableFrom(typeToVerity))
        {
            return;
        }

        if (Attribute.GetCustomAttribute(typeToVerity, typeof(DataContractAttribute)) != null || // XET
            Attribute.GetCustomAttribute(typeToVerity, typeof(SerializableAttribute)) != null)   // UITools
        {
            return;
        }

        Debug.Fail($"Message payload {typeToVerity.FullName} does not appear to be serializable.");
    }
}