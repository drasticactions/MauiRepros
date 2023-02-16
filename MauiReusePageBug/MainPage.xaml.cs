namespace MauiReusePageBug;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        // Switch the page to a blank one...
        Application.Current.MainPage = new ContentPage();

        // Boom!
        Application.Current.MainPage = App._page;
    }
}

