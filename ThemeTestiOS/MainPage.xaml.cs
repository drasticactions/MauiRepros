namespace ThemeTestiOS;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
		// This event fires when you press the Home button on iOS
		// Application.Current.RequestedThemeChanged += Current_RequestedThemeChanged;
		this.UpdateLabels();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		this.UpdateLabels();
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}
	
	private void Current_RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
	{
		this.UpdateLabels();
	}

	public void UpdateLabels()
	{
		this.RequestedThemeLabel.Text = $"Requested Theme: {App.Current.RequestedTheme.ToString()}";
		this.PlatformThemeLabel.Text = $"Platform Theme: {App.Current.RequestedTheme.ToString()}";
		this.UserThemeLabel.Text = $"User Theme: {App.Current.RequestedTheme.ToString()}";
	}
}

