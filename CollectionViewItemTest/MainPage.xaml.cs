namespace CollectionViewItemTest;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
		var list = new List<string>();
		for(var i = 0; i < 60; i++)
		{
			list.Add($"Test {i}");
		}
		this.TestCollectionView.ItemsSource = list;
	}

	private void Button_OnClicked(object? sender, EventArgs e)
	{
		this.DisplayAlert("VisualDiagnostics", $"CollectionView Visual Children: {this.TestCollectionView.GetVisualTreeDescendants().Count}", "OK");
	}
}

