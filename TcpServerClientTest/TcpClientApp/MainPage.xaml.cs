using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TcpClientApp;

public partial class MainPage : ContentPage
{
	int count = 0;
	private TcpTestClient client;
	public MainPage()
	{
		InitializeComponent();
		this.client = new TcpTestClient();
#if ANDROID
		this.AddressField.Text = "10.0.2.2";
#else
		this.AddressField.Text = IPAddress.Loopback.ToString();
#endif
	}
	
	public string IsConnected => client.IsConnected ? "Connected" : "Not Connected";
	
	private async void Button_OnClicked(object? sender, EventArgs e)
	{
		try
		{
			if (!this.client.IsConnected)
			{
				if (string.IsNullOrEmpty(this.PortField.Text))
				{
					this.IsConnectedField.Text = "Port is required";
					return;
				}

				if (string.IsNullOrEmpty(this.AddressField.Text))
				{
					this.IsConnectedField.Text = "Address is required";
					return;
				}

				await this.client.ConnectAsync(this.AddressField.Text, int.Parse(this.PortField.Text), this.AuthSwitch.IsToggled);

				this.IsConnectedField.Text = IsConnected;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			Console.WriteLine(ex.InnerException);
		}
	}

	private async void Button2_OnClicked(object? sender, EventArgs e)
	{
		if (this.client.IsConnected)
		{
			await this.client.Send("Test Message");
		}
	}
	
	
}

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