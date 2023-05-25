namespace AppThemeColorBug;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
		
	}

	protected override void OnHandlerChanged()
	{
		base.OnHandlerChanged();
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

	private void ThemeSwitch_OnToggled(object sender, ToggledEventArgs e)
	{
		if (!e.Value)
			Application.Current.UserAppTheme = AppTheme.Dark;
		else
			Application.Current.UserAppTheme = AppTheme.Light;
	}
}

