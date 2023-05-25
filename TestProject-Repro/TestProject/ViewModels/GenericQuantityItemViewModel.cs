namespace TestProject.ViewModels;

public class GenericQuantityItemViewModel : BaseViewModel
{
    private string _label;
    private int _quantity;
    private int _minimumQuantity = 0;
    private int _maximumQuantity = int.MaxValue;
    private bool _isInformationVisible;

    public string Label
    {
        get => _label;
        set
        {
            _label = value;
            OnPropertyChanged();
        }
    }

    public bool IsInformationVisible
    {
        get => _isInformationVisible;
        set
        {
            _isInformationVisible = value;
            OnPropertyChanged();
        }
    }

    public int Quantity
    {
        get => _quantity;
        set
        {
            _quantity = value;
            OnPropertyChanged();
        }
    }

    public int MinimumQuantity
    {
        get => _minimumQuantity;
        set
        {
            _minimumQuantity = value;
            OnPropertyChanged();
        }
    }

    public int MaximumQuantity
    {
        get => _maximumQuantity;
        set
        {
            _maximumQuantity = value;
            OnPropertyChanged();
        }
    }

    public void SetQuantitySilently(int quantity)
    {
        _quantity = quantity;
        OnPropertyChanged(nameof(Quantity));
    }
}