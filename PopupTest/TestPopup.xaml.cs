using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;

namespace PopupTest;

public partial class TestPopup : Popup
{
    public TestPopup()
    {
        InitializeComponent();
        this.BindingContext = new TestViewModel();
    }
}

public class TestViewModel
{
    public string Title { get; }

    public string SubTitle { get; }

    public IReadOnlyList<TestItem> Items { get; }

    public TestViewModel() 
    {
        Title = "Test popup";
        SubTitle = "with 5 items";
        Items = new List<TestItem>()
        { 
            new TestItem() { Name = "Item1", SomeNumber = 1 },
            new TestItem() { Name = "Item2", SomeNumber = 2 },
            new TestItem() { Name = "Item3", SomeNumber = 3 },
            new TestItem() { Name = "Item4", SomeNumber = 4 },
            new TestItem() { Name = "Item5", SomeNumber = 5 },
        };
    }
}

public class TestItem
{
    public string Name { get; set; }
    public int SomeNumber { get; set; }
}