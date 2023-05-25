namespace TestProject.ViewModels;

public class HeatingPowerItemViewModel : GenericDetailsItemViewModel
{
    private int _powerLevel;

    public int PowerLevel
    {
        get => _powerLevel;
        set
        {
            _powerLevel = value;
            OnPropertyChanged();
            SecondLabel = PowerLevel + "W";
        }
    }
}