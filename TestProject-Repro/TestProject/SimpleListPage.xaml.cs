using System.ComponentModel;
using TestProject.ViewModels;

namespace TestProject;

public partial class SimpleListPage : ContentPage
{
	public SimpleListPage()
	{
		InitializeComponent();
		BindingContext = new SimpleListViewModel();
		
	}
}


