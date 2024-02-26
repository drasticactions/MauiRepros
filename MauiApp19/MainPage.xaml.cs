using System.Collections.ObjectModel;

namespace MauiApp19;

public partial class MainPage : ContentPage
{
	int count = 0;
	private TestViewModel _viewModel;


	public MainPage()
	{
		InitializeComponent();
		this.BindingContext = _viewModel = new TestViewModel();
	}



	private void Button_Clicked(object sender, EventArgs e)
	{
		this._viewModel.AddItem();
	}

	private void Button_Clicked_1(object sender, EventArgs e)
	{
		this._viewModel.RemoveItem();
	}
}

public enum ItemType
{
	Red,
	Blue,
	Green
}

//public record Item(ItemType Type, string Text);

public class Item
{
	public ItemType Type { get; }

	public string Text { get; }

	public Item(ItemType type, string text)
	{
		Type = type;
		Text = text;
	}

}

public class TestViewModel
{
	public ObservableCollection<Item> Items { get; }

	public TestViewModel()
	{
		var list = new List<Item>();
		for (var i = 0; i < 100; i++)
		{
			// alternate between the different ItemType values
			list.Add(new Item((ItemType)(i % 3), $"Item {i}"));
		}

		Items = new ObservableCollection<Item>(list);
	}

	public void AddItem()
	{
		var itemType = (ItemType)(Items.Count % 3);
		Items.Add(new Item(itemType, "New Item"));
	}

	public void RemoveItem()
	{
		if (Items.Count > 0)
		{
			Items.RemoveAt(Items.Count - 1);
		}
	}

}

public class TestDataTemplateSelector : DataTemplateSelector
{
	public DataTemplate? RedItem { get; set; }
	public DataTemplate? BlueItem { get; set; }

	public DataTemplate? GreenItem { get; set; }

	protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
	{
		switch ((item as Item)?.Type)
		{
			case ItemType.Red:
				return RedItem;
			case ItemType.Blue:
				return BlueItem;
			case ItemType.Green:
				return GreenItem;
		}

		throw new InvalidOperationException("Unknown item type");
	}
}


