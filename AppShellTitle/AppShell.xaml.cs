namespace AppShellTitle;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
	}

	public void UpdateTabTitle()
	{
		this.TestTab.Title = "Test";
	}
}

