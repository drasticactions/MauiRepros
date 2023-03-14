using ServiceReference1;
using MauiApp3.ViewModels;

namespace MauiApp3;

public partial class MainPage : ContentPage
{
    public MainPage()
	{
		InitializeComponent();
        Loaded += MainPage_Loaded;
        BindingContext = new ProductsViewModel();
    }

    private void MainPage_Loaded(object sender, EventArgs e)
    {

    }
}

