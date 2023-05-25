using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.ViewModels;

/// <summary>
/// Class representing List Picker Value Model.
/// </summary>
public partial class ListPickerItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int _value;

    [ObservableProperty]
    private string _label;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private bool _isSection;

    [ObservableProperty]
    private string _errorMessageIfNotChoosable;

    [ObservableProperty]
    private bool _isPossibleToChoose = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsPickerValueWithIcon))]
    private string _iconSelectedFileName;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsPickerValueWithIcon))]
    private string _iconNotSelectedFileName;

    [ObservableProperty]
    private bool _isPickerValueWithCheckBox;

    /// <summary>
    /// Gets a value indication wheter the picker value contains an icon.
    /// </summary>
    public bool IsPickerValueWithIcon => !String.IsNullOrEmpty(IconSelectedFileName) && !String.IsNullOrEmpty(IconNotSelectedFileName);
}

