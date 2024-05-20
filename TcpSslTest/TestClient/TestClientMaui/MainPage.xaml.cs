using TestClientCore;

namespace TestClientMaui;

public partial class MainPage : ContentPage
{
	int count = 0;
	private TcpTestClient client = new();
	
	public MainPage()
	{
		InitializeComponent();
		this.IsConnectedField.Text = IsConnected;
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
}

