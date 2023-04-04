using System.Collections.ObjectModel;

namespace ListViewTest;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
		this.TestView.ItemsSource = this.StringList;
		for(var i = 0; i < 30000; i++)
		{
			this.StringList.Add(i.ToString());
		}
	}

	public ObservableCollection<string> StringList = new ObservableCollection<string>();

    private void Button_Clicked(object sender, EventArgs e)
    {
        var children = ((IVisualTreeElement)this.TestView).GetVisualChildren();
    }
}

