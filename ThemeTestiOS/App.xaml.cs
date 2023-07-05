namespace ThemeTestiOS;

public partial class App : Application
{
	private MainPage page;
	public App()
	{
		InitializeComponent();
		
		// This event fires when you press the Home button on iOS
		Current.RequestedThemeChanged += Current_RequestedThemeChanged;

		MainPage = this.page = new MainPage();
	}

	private bool firstTime = true;
	
	private void Current_RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
	{
		// Don't handle events fired for old application instances
		// and clean up the old instance's event handler
		if (Current != this && Current is App app)
		{
			Current.RequestedThemeChanged -= Current_RequestedThemeChanged;
			return;
		}

		var dateTime = DateTime.UtcNow;
		// Shows a different theme
		if (this.firstTime)
		{
			this.page.UpdateLabels();
			this.firstTime = false;
		}
		
		Console.WriteLine($"{dateTime} App Event: {e.RequestedTheme}");
		Console.WriteLine($"{dateTime} Requested Theme: {App.Current.RequestedTheme.ToString()}");
		Console.WriteLine($"{dateTime} Platform Theme: {App.Current.RequestedTheme.ToString()}");
		Console.WriteLine($"{dateTime} User Theme: {App.Current.RequestedTheme.ToString()}");
		// UserAppTheme = e.RequestedTheme;
	}
}
