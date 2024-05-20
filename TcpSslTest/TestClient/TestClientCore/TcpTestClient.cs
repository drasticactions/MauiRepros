using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;

namespace TestClientCore;

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

    public async Task ConnectAsync(string address, int port)
    {
        await client.ConnectAsync(address, port);
        networkStream = client.GetStream();
        var sslStream = new SslStream(this.networkStream, true, (a1, a2, a3, a4) => true);
        await sslStream.AuthenticateAsClientAsync("testServer");
        stream = sslStream;
    }

    public Task Send(string message)
    {
        return stream?.WriteAsync(Encoding.UTF8.GetBytes(message), 0, message.Length) ?? Task.CompletedTask;
    }
}