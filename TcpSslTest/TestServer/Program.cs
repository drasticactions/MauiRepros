// See https://aka.ms/new-console-template for more information

using System.Collections.ObjectModel;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

Console.WriteLine("Hello, World!");

var generator = new SelfSignedCertGenerator();
var cert = generator.Generate();
TcpListener listener = new TcpListener(IPAddress.Any, 12345);
listener.Start();

Console.WriteLine($"IP Address: {listener.LocalEndpoint}");
Console.WriteLine($"Port: {((IPEndPoint)listener.LocalEndpoint).Port}");

Console.WriteLine("Waiting for a client to connect...");
TcpClient client = await listener.AcceptTcpClientAsync();

var networkStream = client.GetStream();
SslStream sslStream = new SslStream(networkStream, true, (a1, a2, a3, a4) => true);
sslStream.AuthenticateAsServer(cert, clientCertificateRequired: false, checkCertificateRevocation: true);

Read(client, sslStream);

void Read(TcpClient client, Stream stream)
{
    byte[] buffer = new byte[client.ReceiveBufferSize];
    while (client.Connected)
    {
        int bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);
        Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, bytesRead));
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

        Collection<X509Extension> extensions =
            (Collection<X509Extension>)CertificateExtensionsProperty!.GetValue(request)!;
        extensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment, true));

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

        return new X509Certificate2(cert!.Export(X509ContentType.Pfx));
    }
}