namespace TestAppAgain;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
        #if IOS || MACCATALYST
        		var device = UIKit.UIDevice.CurrentDevice;
        		var isiOS = device.Model.ToLowerInvariant().Contains("iphone") ||  device.Model.ToLowerInvariant().Contains("ipad");
		        var isMac = device.Model.ToLowerInvariant().Contains("mac");
		        var test = device.CheckSystemVersion(16, 4);
		        this.CounterBtn.Text = test.ToString();
#endif
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}
}

