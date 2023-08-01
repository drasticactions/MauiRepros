namespace MauiListViewTabbedTest;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new MauiTabbedPage();
	}
}

public class MauiTabbedPage : TabbedPage
{
	public MauiTabbedPage()
	{
		this.Children.Add(new MainPage());
		this.Children.Add(new MainPage());
	}
}
