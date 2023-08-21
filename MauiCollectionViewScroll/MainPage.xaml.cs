namespace MauiCollectionViewScroll;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		this.Navigation.PushAsync(new StaticHeightPage());
	}

	private void OnScrollClicked(object sender, EventArgs e)
	{
		this.Navigation.PushAsync(new DynamicHeightPage());
	}
}

