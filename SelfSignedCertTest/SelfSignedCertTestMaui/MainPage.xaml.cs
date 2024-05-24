using System.Net;

namespace SelfSignedCertTestMaui;

public partial class MainPage : ContentPage
{
	int count = 0;
	private TcpTestClient client = new();
	private TcpTestServer internalServer;
	
	public MainPage()
	{
		InitializeComponent();
		this.IsConnectedField.Text = IsConnected;
        internalServer = new TcpTestServer();
    }
	
	public string IsConnected => client.IsConnected ? "Connected" : "Not Connected";

    private async void Button3_OnClicked(object? sender, EventArgs e)
	{
		internalServer.Start();
    }

    private async void Button4_OnClicked(object? sender, EventArgs e)
    {
	    try
	    {
		    if (!this.client.IsConnected)
		    {
			    await this.client.ConnectAsync(IPAddress.Loopback.ToString(), internalServer.Port, internalServer.Cert);
			    this.IsConnectedField.Text = IsConnected;
		    }
	    }
	    catch (Exception exception)
	    {
		    Console.WriteLine(exception);
		    Console.WriteLine(exception.InnerException);
		    throw;
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

