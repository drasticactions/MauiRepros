namespace MauiReusePageBug;

public partial class App : Application
{
	public static Page _page;

	public App()
	{
		InitializeComponent();

		MainPage = _page = new MainPage();
	}
}
