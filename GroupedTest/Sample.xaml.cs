using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Samples;

public class Item : INotifyPropertyChanged
{
    public string Name { get; set; }

    private double size;
    public double Size
    {
        get => size;
        set => SetProperty(ref size, value);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual bool SetProperty<T>(ref T propertyStorage, T value, [CallerMemberName] string propertyName = null)
    {
        bool result = false;

        if (!Equals(propertyStorage, value))
        {
            propertyStorage = value;
            result = true;

            InvokePropertyChanged(propertyName);
        }

        return result;
    }

    public virtual void InvokePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this,
            new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>
/// Use this class when bindings need to respond to collection content changes.
/// </summary>
/// <typeparam name="T">Type of models this collection will contain</typeparam>
public class ObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
{
    #region Data Members
    /// <summary>
    /// Indicates that change detection should be suspended.
    /// </summary>
    private bool suspendCollectionChanged = false;
    #endregion

    #region Constructors
    /// <summary>
    /// Initialize an empty collection.
    /// </summary>
    public ObservableCollection() : base()
    {
    }

    /// <summary>
    /// Initialize collection with provided list.
    /// </summary>
    /// <param name="collection">List of models</param>
    public ObservableCollection(IEnumerable<T> collection) : base(collection)
    {
    }
    #endregion

    #region Methods
    /// <summary>
    /// Calls CollectionChanged event. 
    /// </summary>
    /// <param name="e">NotifyCollectionChangedEventArgs</param>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (!suspendCollectionChanged)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                base.OnCollectionChanged(e);
            });
        }
    }

    public void Rebind()
    {
        OnCollectionChanged(
            new NotifyCollectionChangedEventArgs(
            NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// Added the specified list of models to the collection.
    /// </summary>
    /// <param name="collection">List of Models</param>
    public virtual void AddRange(IEnumerable<T> collection)
    {
        if (collection != null && collection.Any())
        {
            try
            {
                suspendCollectionChanged = true;

                foreach (T item in collection)
                {
                    Add(item);
                }
            }
            finally
            {
                suspendCollectionChanged = false;
            }

            Rebind();
        }
    }

    /// <summary>
    /// Replaces the collection with the specified list of models.
    /// </summary>
    /// <param name="collection">List of models</param>
    public virtual void Replace(IEnumerable<T> collection)
    {
        try
        {
            suspendCollectionChanged = true;

            Clear();
            AddRange(collection);
        }
        finally
        {
            suspendCollectionChanged = false;
        }

        if (collection == null || !collection.Any())
        {
            Rebind();
        }
    }

    /// <summary>
    /// Append or replace with the specified list of models as indicated.
    /// </summary>
    /// <param name="collection">List of models</param>
    /// <param name="clear">True to replace, false to append to the collection</param>
    public virtual void Load(IEnumerable<T> collection, bool clear = true)
    {
        if (clear)
        {
            Replace(collection);
        }
        else
        {
            AddRange(collection);
        }
    }

    #endregion
}

public class Group : ObservableCollection<Item>
{
	public string Name { get; set; }
    public ICommand Toggle => new Command(parameter =>
    {
        if (parameter is Group group)
        {
            foreach(Item item in group)
            {
                if (item.Size == 0)
                {
                    item.Size = 30;
                }
                else
                {
                    item.Size = 0;
                }
            }
        }
    });
}

public partial class Sample : ContentPage
{
    public ObservableCollection<Group> Groups { get; set; } = [];

    public ICommand Load => new Command(parameter =>
    {
        DateTime start = DateTime.Now;
        List<Group> groups = [];

        Group group1 = new Group() { Name = "One" };
        for (var i = 0; i < 1000; i++)
        {
            group1.Add(new Item() { Name = i.ToString(), Size = 30 });
        }
     
        
        groups.Add(group1);

        Group group2 = new Group() { Name = "Two" };
       for (var i = 0; i < 1000; i++)
        {
            group2.Add(new Item() { Name = i.ToString(), Size = 30 });
        }
        groups.Add(group2);

        Groups.Load(groups);
        time.Text = (DateTime.Now - start).ToString();
    });

    public Sample()
	{
        InitializeComponent();

        BindingContext = this;
        Load.Execute(null);
    }
}