// See https://aka.ms/new-console-template for more information

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

Console.WriteLine("Hello, World!");

var testServer = new TestServer();
await testServer.RunServer();

// Task.Run(async () => await testServer.RunServer()).FireAndForgetSafeAsync();
//
// Console.WriteLine("Press any key to exit...");
// Console.ReadLine();


class TestServer
{
    public async Task RunServer()
    {
        var generator = new SelfSignedCertGenerator();

        var cert = generator.Generate();
        TcpListener listener = new TcpListener(IPAddress.Any, 0);
        listener.Start();

        Console.WriteLine($"IP Address: {listener.LocalEndpoint}");
        Console.WriteLine($"Port: {((IPEndPoint)listener.LocalEndpoint).Port}");

        Console.WriteLine("Waiting for a client to connect...");
        TcpClient client = await listener.AcceptTcpClientAsync();

        try
        {
            var networkStream = client.GetStream();
            SslStream sslStream =
                new SslStream(networkStream, true, (a1, a2, a3, a4) => true, (b1, b2, b3, b4, b5) => cert);
            sslStream.AuthenticateAsServer(cert, clientCertificateRequired: false, checkCertificateRevocation: false,
                enabledSslProtocols: SslProtocols.Tls13);

            Read(client, sslStream);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Debug.WriteLine(ex.InnerException);
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
}

/// <summary>
/// Generates a self-signed certificate for use in the debug session.
/// This class uses reflection to access APIs that are not available in .NET Standard 2.0.
/// But are in .NET Framework 4.7.2 and .NET.
/// </summary>
internal class SelfSignedCertGenerator
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
            "CN=localServer",
            RSA.Create(),
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
        });

        Collection<X509Extension> extensions =
            (Collection<X509Extension>)CertificateExtensionsProperty!.GetValue(request)!;
        extensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment, true));

        // object? sanBuilder = Activator.CreateInstance(SanBuilderType!);
        // this.AddDnsNameMethod!.Invoke(sanBuilder, new object[] { "localhost" });
        //
        // MethodInfo? buildMethod = SanBuilderType!.GetMethod("Build");
        // X509Extension sanExtension = (X509Extension)buildMethod!.Invoke(sanBuilder, null)!;
        // extensions.Add(sanExtension);

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

        return new X509Certificate2(cert!.Export(X509ContentType.Pfx));
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
            Debug.WriteLine(ex);
        }
    }
}