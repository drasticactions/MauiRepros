namespace TestProject.ViewModels;

public class GenericSectionItemViewModel : BaseViewModel
{
    private string _title;

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }
}