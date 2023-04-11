using System.Collections.ObjectModel;

namespace MauiCollectionView;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
		this.Items = new ObservableCollection<string>() { "Test1", "Hoge", "Foobar" };
        VisualDiagnostics.VisualTreeChanged += VisualDiagnostics_VisualTreeChanged;
		this.TestCollectionView.ItemsSource = this.Items;
	}

    private void VisualDiagnostics_VisualTreeChanged(object sender, VisualTreeChangeEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"{e.ChangeType}: Child: {e.Child?.GetType()} Parent: {e.Parent?.GetType()}");
    }

    public ObservableCollection<string> Items { get; set; }

    void Button_Clicked(System.Object sender, System.EventArgs e)
    {
		var test = ((IVisualTreeElement)this.TestCollectionView).GetVisualChildren();
        System.Diagnostics.Debug.WriteLine(test.Count());
        foreach (var item in test)
        {
            if (item is Label label) {
                System.Diagnostics.Debug.WriteLine(label.Text);
            }
        }
    }
}

