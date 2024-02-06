namespace DisposeTest;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		#if MACCATALYST
		AppDelegate.CurrentApp.Dispose();
		#endif
	}
}

