using System.Collections.ObjectModel;

namespace ListViewMaui8;

public partial class MainPage : ContentPage
{
    public ObservableCollection<string> Items { get; set; }
    public MainPage()
    {
        InitializeComponent();
        this.Items = new ObservableCollection<string>();
        this.ListView.ItemsSource = this.Items;
    }

    private void RemoveButton_Clicked(object sender, EventArgs e)
    {
        if (this.Items.Count > 0)
        {
            //this.Items.RemoveAt(this.Items.Count - 1);
            this.Items.RemoveAt(0);
        }
    }

    private void AddButton_Clicked(object sender, EventArgs e)
    {
        this.Items.Add("Item");
    }
}

