namespace MauiApp1;

public partial class MainPage : ContentPage
{
    private ViewModel ViewModel => (ViewModel)BindingContext;

    public MainPage()
    {
        InitializeComponent();

        BindingContext = new ViewModel(collectionView);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        ViewModel.PreSelectUsingBindingCommand.Execute(null);
    }
}
