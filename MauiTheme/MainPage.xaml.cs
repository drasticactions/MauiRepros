namespace MauiTheme;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
		Application.Current.RequestedThemeChanged += (s, a) =>
		{
			this.Label.Text = $"RequestedThemeChanged: {a.RequestedTheme}";
			System.Diagnostics.Debug.WriteLine($"RequestedThemeChanged: {a.RequestedTheme}");
		};
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		this.DisplayAlert("Title", $"You clicked {++count} times", "OK");
	}
	
}

