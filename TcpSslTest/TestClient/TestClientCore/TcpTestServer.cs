using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.Security;
using System.Runtime.ConstrainedExecution;
using System.Security.Authentication;

namespace TestClientCore
{
    public  class TcpTestServer
    {
        private TcpListener listener;
        private TcpClient client;
        private NetworkStream networkStream;
        private X509Certificate2 cert = new SelfSignedCertGenerator().Generate();

        public TcpTestServer()
        {
            listener = new TcpListener(System.Net.IPAddress.Any, 0);
        }

        public string IPAddress => ((System.Net.IPEndPoint)listener.LocalEndpoint).Address.ToString();

        public int Port => ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;

        public void Start()
        {
            try
            {
                Console.WriteLine("Server started");
                listener.Start();
                Task.Run(async () =>
                {
                    this.client = await listener.AcceptTcpClientAsync();
                    Console.WriteLine("Client connected");
                    networkStream = client.GetStream();
                    SslStream sslStream = new SslStream(networkStream, true, (a1, a2, a3, a4) => true);
                    await sslStream.AuthenticateAsServerAsync(cert, clientCertificateRequired: false,
                        checkCertificateRevocation: true);
                    Read(client, sslStream);
                });
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

            //object? sanBuilder = Activator.CreateInstance(SanBuilderType!);
            //this.AddDnsNameMethod!.Invoke(sanBuilder, new object[] { "localhost" });

            //MethodInfo? buildMethod = SanBuilderType!.GetMethod("Build");
            //X509Extension sanExtension = (X509Extension)buildMethod!.Invoke(sanBuilder, null)!;
            //extensions.Add(sanExtension);

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
}
