using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Platform;
using System.Collections.ObjectModel;
using System.Reflection;

namespace ListViewMaui7;

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

    private void TemplateItemsCount_Clicked(object sender, EventArgs e)
    {
        var item = (ITemplatedItemsView<Cell>)((ListViewRenderer)this.ListView.Handler).Element;

        this.TemplateItems.Text = "Count: " + item.TemplatedItems.Count;
        FieldInfo fieldInfo = typeof(Microsoft.Maui.Controls.Internals.TemplatedItemsList<ItemsView<Cell>, Cell>).GetField("_templatedObjects", BindingFlags.Instance | BindingFlags.NonPublic);
        var templatedObjects = (List<object>)fieldInfo.GetValue(item);
    }
}

///// <summary>
///// Enumerator for ItemsView-derived classes.
///// </summary>
//public class FormsItemsViewEnumerator<T> : ReadOnlyListBase where T : BindableObject
//{
//    private IReadOnlyList<T> templatedItems;
//    public FormsItemsViewEnumerator(ItemsView<T> itemsView) => this.templatedItems = itemsView.TemplatedItems;
//    public override int Count => this.templatedItems.Count;
//    public override object this[int index] => this.templatedItems[index];
//}