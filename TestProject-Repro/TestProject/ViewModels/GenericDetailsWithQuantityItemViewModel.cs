namespace TestProject.ViewModels;

public class GenericDetailsWithQuantityItemViewModel : GenericDetailsItemViewModel
{
    private int _quantity;

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public int Quantity
    {
        get => _quantity;
        set
        {
            _quantity = value;
            OnPropertyChanged();
        }
    }
}