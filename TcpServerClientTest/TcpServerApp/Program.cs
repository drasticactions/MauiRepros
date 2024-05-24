using System.Collections.ObjectModel;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

Console.WriteLine("Hello, Tcp!");

var server = new TcpTestServer(IPAddress.Loopback);

Console.WriteLine($"Server IP: {server.IPAddress}");
Console.WriteLine($"Server Port: {server.Port}");

Console.WriteLine("With auth? y for yes, any other key for no");
var withAuth = Console.ReadLine().ToLower() == "y";

Task.Run(() => server.Start(withAuth)).FireAndForgetSafeAsync();

Console.WriteLine("Press any key to exit");
Console.ReadKey();

public class TcpTestServer
{
    private TcpListener listener;
    private TcpClient client;
    private NetworkStream networkStream;
    private X509Certificate2 cert;
    public TcpTestServer(IPAddress ip)
    {
        listener = new TcpListener(ip, 0);
        listener.Start();
        cert = new SelfSignedCertGenerator().Generate();
    }

    public string IPAddress => ((System.Net.IPEndPoint)listener.LocalEndpoint).Address.ToString();

    public int Port => ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;

    public void Start(bool withAuth)
    {
        try
        {
            Console.WriteLine("Server started");
            this.client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected");
            networkStream = client.GetStream();
            if (withAuth)
            {
                SslStream sslStream = new SslStream(networkStream, false);
                sslStream.AuthenticateAsServer(cert, false, SslProtocols.Tls12, true);
                Read(client, sslStream);
            }
            else
            {
                Read(client, networkStream);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task StartAsync(bool withAuth)
    {
        try
        {
            Console.WriteLine("Server started");
            this.client = await listener.AcceptTcpClientAsync();
            Console.WriteLine("Client connected");
            networkStream = client.GetStream();
            if (withAuth)
            {
                SslStream sslStream = new SslStream(networkStream, false);
                await sslStream.AuthenticateAsServerAsync(cert, false, SslProtocols.Tls12, true);
                Read(client, sslStream);
            }
            else
            {
                Read(client, networkStream);
            }
            Read(client, networkStream);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    void Read(TcpClient client, Stream stream)
    {
        byte[] buffer = new byte[client.ReceiveBufferSize];
        while (client.Connected)
        {
            int bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);
            Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, bytesRead));
        }
    }
}

/// <summary>
/// Task Utilities.
/// </summary>
public static class TaskUtilities
{
    /// <summary>
    /// Fire and Forget Safe Async.
    /// </summary>
    /// <param name="task">Task to Fire and Forget.</param>
    /// <param name="handler">Error Handler.</param>
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
    public static async void FireAndForgetSafeAsync(this Task task)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}

    /// <summary>
    /// Generates a self-signed certificate for use in the debug session.
    /// This class uses reflection to access APIs that are not available in .NET Standard 2.0.
    /// But are in .NET Framework 4.7.2 and .NET.
    /// </summary>
    public class SelfSignedCertGenerator
    {
        private readonly Type? CertificateRequestType;
        private readonly ConstructorInfo? CertificateRequestConstructor;
        private readonly PropertyInfo? CertificateExtensionsProperty;
        private readonly MethodInfo? CreateSelfSignedMethod;
        private readonly Type? SanBuilderType;
        private readonly MethodInfo? AddDnsNameMethod;

        public SelfSignedCertGenerator()
        {
            // These are all public methods within the System.Security.Cryptography.X509Certificates namespace.
            // They are not available in .NET Standard 2.0, but are in .NET Framework 4.7.2 and .NET.
            this.CertificateRequestType =
                typeof(RSACertificateExtensions).Assembly.GetType(
                    "System.Security.Cryptography.X509Certificates.CertificateRequest");
            this.CertificateRequestConstructor = CertificateRequestType?.GetConstructor(
            [
                typeof(string),
            typeof(RSA),
            typeof(HashAlgorithmName),
            typeof(RSASignaturePadding)
            ]);
            this.CertificateExtensionsProperty = CertificateRequestType?.GetProperty("CertificateExtensions");
            this.CreateSelfSignedMethod = CertificateRequestType?.GetMethod("CreateSelfSigned");
            this.SanBuilderType = typeof(X509Extension).Assembly.GetType("System.Security.Cryptography.X509Certificates.SubjectAlternativeNameBuilder");
            this.AddDnsNameMethod = SanBuilderType?.GetMethod("AddDnsName", new Type[] { typeof(string) });
        }

        public X509Certificate2 Generate()
        {
            if (CertificateRequestType == null)
            {
                throw new InvalidOperationException("CertificateRequest type not found.");
            }

            object request = CertificateRequestConstructor!.Invoke(new object[]
            {
            "CN=testServer",
            RSA.Create(),
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
            });
          
            DateTimeOffset now = DateTimeOffset.UtcNow;
            X509Certificate2? cert = (X509Certificate2?)CreateSelfSignedMethod!.Invoke(
                request,
                new object[]
                {
                now.AddDays(-1),
                now.AddDays(7)
                });

            if (cert is null)
            {
                throw new InvalidOperationException("CreateSelfSignedMethod return null.");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using (cert)
                {
                    return new X509Certificate2(cert.Export(X509ContentType.Pfx), "", X509KeyStorageFlags.UserKeySet);
                }
            }

            var bytes = cert!.Export(X509ContentType.Pfx);
            return new X509Certificate2(bytes);
        }
    }