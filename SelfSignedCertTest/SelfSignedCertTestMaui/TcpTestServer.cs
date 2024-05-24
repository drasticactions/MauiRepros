using System.Collections.ObjectModel;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SelfSignedCertTestMaui;

public  class TcpTestServer
    {
        private TcpListener listener;
        private TcpClient client;
        private NetworkStream networkStream;
        private X509Certificate2 cert;

        public TcpTestServer()
        {
            listener = new TcpListener(System.Net.IPAddress.Any, 0);
            cert = new CertGenerator().GenerateCert();
        }
        
        public X509Certificate2 Cert => cert;

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
                    LocalCertificateSelectionCallback localCertificateSelectionCallback = (a1, a2, a3, a4, a5) =>
                    {
                        return cert;
                    };
                    SslStream sslStream = new SslStream(networkStream, true, (a1, a2, a3, a4) =>
                    {
                        if (a3 is not null)
                        {
                            foreach(var item in a3.ChainElements)
                            {
                                Console.WriteLine($"Certificate Server: {item.ChainElementStatus[0].Status}");
                                Console.WriteLine($"Certificate Server: {item.ChainElementStatus[0].StatusInformation}");
                            }
                        }
            
                        if (a4 != SslPolicyErrors.None)
                        {
                            Console.WriteLine($"SSL Policy Errors Server: {a4}");
                            return true;
                        }
                        return true;
                    }, localCertificateSelectionCallback);
                    
                    
                    await sslStream.AuthenticateAsServerAsync(cert, clientCertificateRequired: false,
                        checkCertificateRevocation: false);
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