namespace TestProject.ViewModels;

public class GenericDetailsItemViewModel : BaseViewModel
{
    private string _label;
    private string _secondLabel;
    private string _thirdLabel;
    private string _fourthLabel;
    private string _fifthLabel;

    public string Label
    {
        get => _label;
        set
        {
            _label = value;
            OnPropertyChanged();
        }
    }

    public string SecondLabel
    {
        get => _secondLabel;
        set
        {
            _secondLabel = value;
            OnPropertyChanged();
        }
    }

    public string ThirdLabel
    {
        get => _thirdLabel;
        set
        {
            _thirdLabel = value;
            OnPropertyChanged();
        }
    }

    public string FourthLabel
    {
        get => _fourthLabel;
        set
        {
            _fourthLabel = value;
            OnPropertyChanged();
        }
    }

    public string FifthLabel
    {
        get => _fifthLabel;
        set
        {
            _fifthLabel = value;
            OnPropertyChanged();
        }
    }
}