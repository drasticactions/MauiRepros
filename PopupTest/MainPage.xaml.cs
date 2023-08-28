using CommunityToolkit.Maui.Views;

namespace PopupTest;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		var testPopup = new TestPopup();
		this.ShowPopup(testPopup);
	}
}

