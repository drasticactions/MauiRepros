using System.Collections.ObjectModel;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
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

    public async Task ConnectAsync(string address, int port, X509Certificate2? cert = null)
    {
        await client.ConnectAsync(address, port);
        networkStream = client.GetStream();
        var sslStream = new SslStream(this.networkStream, true, (a1, a2, a3, a4) => true, (sender, host, certificates, certificate, issuers) => cert);
        X509Certificate2Collection? certificate2Collection = null;
        if (cert != null)
        {
            certificate2Collection = new X509Certificate2Collection(cert);
        }
        
        await sslStream.AuthenticateAsClientAsync("localhost", certificate2Collection, SslProtocols.Tls13 | SslProtocols.Tls12, false);
        stream = sslStream;
    }

    public Task Send(string message)
    {
        return stream?.WriteAsync(Encoding.UTF8.GetBytes(message), 0, message.Length) ?? Task.CompletedTask;
    }
}