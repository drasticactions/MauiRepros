using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SelfSignedCertTestStandard;

public class TcpTestClient
{
    private TcpClient client;
    private NetworkStream? networkStream;
    private Stream? stream;
    
    public TcpTestClient()
    {
        client = new TcpClient();
    }
    
    public TcpClient Client => client;
    
    public bool IsConnected => client.Connected;

    public async Task ConnectAsync(string address, int port, X509Certificate2? cert = null)
    {
        await client.ConnectAsync(address, port);
        networkStream = client.GetStream();
        var sslStream = new SslStream(this.networkStream, true, (a1, a2, a3, a4) =>
        {
            if (a4 != SslPolicyErrors.None)
            {
                Console.WriteLine($"SSL Policy Errors: {a4}");
                return true;
            }
            return true;
        });
        X509Certificate2Collection? certificate2Collection = null;
        if (cert != null)
        {
            certificate2Collection = new X509Certificate2Collection(cert);
        }
        
        // var options = new SslClientAuthenticationOptions()
        // {
        //     TargetHost = "localhost",
        //     EnabledSslProtocols = SslProtocols.Tls12,
        //     AllowRenegotiation = false,
        //     CertificateRevocationCheckMode = X509RevocationMode.NoCheck,
        //     ClientCertificates = certificate2Collection
        // };
        
        //await sslStream.AuthenticateAsClientAsync(options);
        await sslStream.AuthenticateAsClientAsync("testServer", certificate2Collection, SslProtocols.Tls12, true);
        stream = sslStream;
    }

    public Task Send(string message)
    {
        return stream?.WriteAsync(Encoding.UTF8.GetBytes(message), 0, message.Length) ?? Task.CompletedTask;
    }
}