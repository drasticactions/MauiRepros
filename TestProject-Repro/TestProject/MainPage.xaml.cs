using System.ComponentModel;
using TestProject.ViewModels;

namespace TestProject;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
		BindingContext = new InventoryRoomsOverviewViewModel();
		
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
		this.Navigation.PushAsync(new ListPickerPage());
    }
}


