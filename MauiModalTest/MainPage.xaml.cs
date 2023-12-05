namespace MauiModalTest;

public partial class MainPage : ContentPage
{
	int count = 0;
	
	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		this.Navigation.PushModalAsync(new MainPage(), true);
	}
}

