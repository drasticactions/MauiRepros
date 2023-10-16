using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListViewTestNet8;

public partial class ImageCellExample : ContentPage
{
    private List<string> _items = new List<string>();
    public ImageCellExample()
    {
        InitializeComponent();
        for(var i = 0; i < 100; i++)
        {
            _items.Add($"Item {i}");
        }
		
        this.ListViewExample.ItemsSource = _items;
    }
}