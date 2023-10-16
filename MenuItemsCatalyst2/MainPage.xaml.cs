namespace MenuItemsCatalyst2;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		MenuBarItem topitem=new MenuBarItem(){Text="TOP MENU 1"};
		for (int i=0;i<1000;i++)
			topitem.Add(new MenuFlyoutItem(){Text="SUB MENU "+i});
		this.MenuBarItems.Add(topitem);
		
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}
}

