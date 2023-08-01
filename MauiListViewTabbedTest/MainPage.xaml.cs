using System.Collections.ObjectModel;

namespace MauiListViewTabbedTest;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
		this.ListViewTest.ItemsSource = this.Items;
		//this.AddItems();
	}

	public ObservableCollection<string> Items { get; } = new ObservableCollection<string>();

	private void AddItems()
	{
		for (var i = 0; i < 100; i++)
		{
			this.Items.Add($"Item {i}");
		}
	}
	protected override void OnAppearing()
	{
		Task.Run(() => this.AddItems());
		base.OnAppearing();
	}

	private void Button_OnClicked(object sender, EventArgs e)
	{
		this.AddItems();
	}
}

