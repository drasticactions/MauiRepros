namespace BugCollectionViewScrollDemo;

public partial class MainPage : ContentPage
{
    MainPageViewModel mainPageViewModel;

    public MainPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        mainPageViewModel = this?.BindingContext as MainPageViewModel;
    }
}


