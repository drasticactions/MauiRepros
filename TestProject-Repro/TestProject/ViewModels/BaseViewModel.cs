using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestProject.ViewModels;

public class BaseViewModel : ObservableObject
{
    //public event PropertyChangedEventHandler PropertyChanged;

    //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    //{
    //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //}

    //protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    //{
    //    if (EqualityComparer<T>.Default.Equals(field, value)) return false;
    //    field = value;
    //    OnPropertyChanged(propertyName);
    //    return true;
    //}

    public virtual void OnAppearing()
    {
        
    }

    public virtual void OnNavigatedTo(NavigatedToEventArgs args)
    {
        
    }
}