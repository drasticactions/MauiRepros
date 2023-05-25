using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.ViewModels;

/// <summary>
/// Represent the "List picker" view-model.
/// </summary>
public partial class ListPickerViewModel : BaseViewModel
{
    public ObservableCollection<ListPickerItemViewModel> AvailableValues { get; } =
        new ObservableCollection<ListPickerItemViewModel>();

    [ObservableProperty]
    private bool _isLoading = true;

    /// <inheritdoc/>
    public override void OnAppearing()
    {
        base.OnAppearing();

        // Run this on the background thread...
        Task.Run(() =>
        {
            for (int i = 0; i < 12; i++)
            {
                AvailableValues.Add(new ListPickerItemViewModel { Value = i, Label = i.ToString(), IsSelected = i == 3 });
            }
        });
    }

    public ListPickerViewModel()
    {
        // Or create it when the view-model is created.
        // for (int i = 0; i < 12; i++)
        // {
        //     AvailableValues.Add(new ListPickerItemViewModel { Value = i, Label = i.ToString(), IsSelected = i == 3 });
        // }
    }

    /// <inheritdoc />
    public override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        IsLoading = false;
    }
}
