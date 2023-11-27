namespace AppTrackingTransparencyMaui;

public partial class MainPage : ContentPage
{
	#if IOS
	AppTrackingTransparencyService appTrackingTransparencyService = new();
	#endif
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnCounterClicked(object sender, EventArgs e)
	{
#if IOS
		var result = await appTrackingTransparencyService.RequestAsync();
		this.CounterBtn.Text = $"{result}";
#else
		this.CounterBtn.Text = $"Not iOS";
#endif
	}
}

