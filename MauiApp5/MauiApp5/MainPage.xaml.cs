namespace MauiApp5;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		if (this.CounterBtn.ImageSource == null)
		{
			this.CounterBtn.ImageSource = "dotnet_bot.png";
		}
		else
		{
			this.CounterBtn.ImageSource = null;
		}

		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}
}

