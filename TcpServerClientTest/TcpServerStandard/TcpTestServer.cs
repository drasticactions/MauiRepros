using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TcpServerStandard;

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
                SslStream sslStream = new SslStream(networkStream, false, (sender, certificate, chain, errors) => true);
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
            
            Collection<X509Extension> extensions =
                (Collection<X509Extension>)CertificateExtensionsProperty!.GetValue(request)!;
            extensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment, true));

            object? sanBuilder2 = Activator.CreateInstance(SanBuilderType!);
            this.AddDnsNameMethod!.Invoke(sanBuilder2, new object[] { "localhost" });
            this.AddDnsNameMethod!.Invoke(sanBuilder2, new object[] { "0.0.0.0" });
            this.AddDnsNameMethod!.Invoke(sanBuilder2, new object[] { IPAddress.Loopback.ToString() });
            foreach(var ip in NetworkUtils.DeviceIps())
            {
                this.AddDnsNameMethod!.Invoke(sanBuilder2, new object[] { ip });
            }
            
            extensions.Add(
                new X509EnhancedKeyUsageExtension(
                    new OidCollection
                    {
                        new Oid("1.3.6.1.5.5.7.3.1")
                    },
                    false));


            MethodInfo? buildMethod = SanBuilderType!.GetMethod("Build");
            X509Extension sanExtension = (X509Extension)buildMethod!.Invoke(sanBuilder2, new object?[] { false })!;
            extensions.Add(sanExtension);
          
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
    
    public static class NetworkUtils {
        public static IEnumerable<string> DeviceIps ()
        {
            return GoodInterfaces ()
                .SelectMany (x =>
                    x.GetIPProperties ().UnicastAddresses
                        .Where (y => y.Address.AddressFamily == AddressFamily.InterNetwork)
                        .Select (y => y.Address.ToString ())).Union(new[] { "127.0.0.1" }).OrderBy(x=> x);
        }

        public static IEnumerable<NetworkInterface> GoodInterfaces ()
        {
            var allInterfaces = NetworkInterface.GetAllNetworkInterfaces ();
            return allInterfaces.Where (x => x.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                                             !x.Name.StartsWith ("pdp_ip", StringComparison.Ordinal) &&
                                             x.OperationalStatus == OperationalStatus.Up);
        }
    }