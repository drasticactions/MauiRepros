using System.Net.NetworkInformation;

namespace PingTestMaui;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnCounterClicked(object sender, EventArgs e)
	{
		try
		{
			using (var ping = new Ping())
			{
				var reply = await ping.SendPingAsync("www.google.com");
				if (reply.Status == IPStatus.Success)
				{
					CounterBtn.Text = $"Ping to {reply.Address}: {reply.RoundtripTime} ms";
				}
				else
				{
					CounterBtn.Text = $"Ping failed: {reply.Status}";
				}
			}
		}
		catch (Exception ex)
		{
			CounterBtn.Text = $"Ping failed: {ex.Message}";
		}
	}
}

