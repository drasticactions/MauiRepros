using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiListView;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
		this.BindingContext = this;
		for (var  i = 0; i < 10; i++)
		{
			this.Items.Add(new TestClass());
		}
	}

	public ObservableCollection<TestClass> Items { get; } = new ObservableCollection<TestClass>();

    private void Button_Clicked(object sender, EventArgs e)
    {
		foreach(var item in this.Items)
		{
			item.Name = "Test 2";
		}
    }
}

public class TestClass : INotifyPropertyChanged
{
    private string name = "Test";

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name {
        get { return this.name; }
        set { this.SetProperty(ref this.name, value); }
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

#pragma warning disable SA1600 // Elements should be documented
    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action? onChanged = null)
#pragma warning restore SA1600 // Elements should be documented
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
        {
            return false;
        }

        backingStore = value;
        onChanged?.Invoke();
        this.OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// On Property Changed.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        var changed = this.PropertyChanged;
        if (changed == null)
        {
            return;
        }

        changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

