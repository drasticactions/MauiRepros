using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ListViewMVVM;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}
}

public class MainPageModel
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}

public class MainPageViewModel 
{
    public ICommand AddItemCommand { get; private set; }

    private ObservableCollection<MainPageModel> myItems;
    public ObservableCollection<MainPageModel> MyItems
    {
        get { return myItems; }
        set
        {
            if (myItems == value) return;

            myItems = value;
        }
    }

    public MainPageViewModel()
    {
        myItems = new()
        {
            new MainPageModel { Name = "Item 1", Description = "Description 1" },
            new MainPageModel { Name = "Item 2", Description = "Description 2" },
            new MainPageModel { Name = "Item 3", Description = "Description 3" }
        };

        AddItemCommand = new Command(AddRecord);
    }

    public void AddRecord()
    {
        myItems.Add(new MainPageModel
        {
            Name = "Item" + (myItems.Count + 1).ToString(), Description = "Description " + (myItems.Count + 1).ToString()
        }) ;

    }
}