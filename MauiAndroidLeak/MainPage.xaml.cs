using System.Collections.ObjectModel;
using System.Timers;
using Timer = System.Timers.Timer;
namespace MauiAndroidLeak;

public partial class MainPage : ContentPage
{
	public ObservableCollection<Item> ItemList { get; } = new();
	private System.Timers.Timer _timer;
	public MainPage()
	{
		InitializeComponent();
		this.UpdateList();
		this.Collection.ItemsSource = this.ItemList;
		_timer = new Timer(2000);
		_timer.Elapsed += OnTimerElapsed;
		_timer.Start();
	}

	private void OnTimerElapsed(object sender, ElapsedEventArgs e)
	{
		this.UpdateList();
	}

	private void UpdateList()
	{
		lock (this)
		{
			// UI Thread.
			this.ItemList.Clear();
		
			for (int i = 0; i < 50; i++)
			{
				// UI Thread.
				ItemList.Add(new ());
			}
		}
		
		System.Diagnostics.Debug.WriteLine($"Total Managed Memory: {GC.GetTotalMemory(forceFullCollection: false) / (1024 * 1024)} MB");
	}
}

public class Item
{
	public string Name { get; set; } = "Item";
	public bool IsEnable { get; set; }
	public string Title { get; set; } = "Title";

	public ObservableCollection<SubItem> SubitemList { get; } = new ObservableCollection<SubItem> { new(), new(), new(), new(), new() };
}

public class SubItem
{
	public string Name { get; set; } = "SubItem";
	public string Description { get; set; } = "Description";
}