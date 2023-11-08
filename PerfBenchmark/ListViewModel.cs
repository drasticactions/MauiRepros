using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PerfBenchmark;

public class ListViewModel : INotifyPropertyChanged
{
    
    
    
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
        Microsoft.Maui.Controls.Application.Current!.Dispatcher.Dispatch(() =>
        {
            var changed = this.PropertyChanged;
            if (changed == null)
            {
                return;
            }

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        });
    }
}

public class ListItemImage
{
    public string Url { get; set; } = string.Empty;
}