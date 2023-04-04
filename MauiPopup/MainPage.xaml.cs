using CommunityToolkit.Maui.Views;

namespace MauiPopup;

public partial class MainPage : ContentPage
{
	int count = 0;
	SimplePopup popup = new SimplePopup();
	public MainPage()
	{
		InitializeComponent();
        this.popup.Closed += Popup_Closed;
	}

    private void Popup_Closed(object sender, CommunityToolkit.Maui.Core.PopupClosedEventArgs e)
    {
        var visualElements = (this.GetVisualTreeDescendants()).ToList();
        this.OnChildRemoved(this.popup, this.LogicalChildren.IndexOf(this.popup));
    }

    private void OnCounterClicked(object sender, EventArgs e)
	{
		this.ShowPopup(popup);
        this.OnChildAdded(this.popup);
    }
}

