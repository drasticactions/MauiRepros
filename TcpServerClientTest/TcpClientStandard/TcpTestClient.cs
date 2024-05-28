using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TcpClientStandard;

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

    public async Task ConnectAsync(string address, int port, bool authEnabled)
    {
        await client.ConnectAsync(address, port);
        stream = networkStream = client.GetStream();
        if (authEnabled)
        {
            SslStream sslStream = new SslStream(networkStream, false, (sender, certificate, chain, errors) => true);
            await sslStream.AuthenticateAsClientAsync("localhost", new X509Certificate2Collection(), SslProtocols.Tls12, true);
            stream = sslStream;
        }
    }

    public Task Send(string message)
    {
        return stream?.WriteAsync(Encoding.UTF8.GetBytes(message), 0, message.Length) ?? Task.CompletedTask;
    }
}
