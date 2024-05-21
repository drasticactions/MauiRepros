using System.Net;
using System.Net.Sockets;
using TestClientCore;

namespace TestClientMaui;

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
        if (!this.client.IsConnected)
        {
            await this.client.ConnectAsync(internalServer.IPAddress, internalServer.Port);
            this.IsConnectedField.Text = IsConnected;
        }
    }


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

                await this.client.ConnectAsync(this.AddressField.Text, int.Parse(this.PortField.Text));

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

    private void Button_Clicked(object sender, EventArgs e)
    {

    }
}

