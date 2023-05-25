using TestProject.ViewModels;

namespace TestProject;

public partial class ListPickerPage : BasePage
{
	public ListPickerPage()
	{
		InitializeComponent();
		BindingContext = new ListPickerViewModel();
	}
}