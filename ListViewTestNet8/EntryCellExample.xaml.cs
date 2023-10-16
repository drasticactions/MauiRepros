using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListViewTestNet8;

public partial class EntryCellExample : ContentPage
{
    private List<string> _items = new List<string>();
    public EntryCellExample()
    {
        InitializeComponent();
        for(var i = 0; i < 100; i++)
        {
            _items.Add($"Item {i}");
        }
		
        this.ListViewExample.ItemsSource = _items;
    }
}