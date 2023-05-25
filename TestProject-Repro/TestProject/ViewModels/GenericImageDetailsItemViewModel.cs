namespace TestProject.ViewModels;

public class GenericImageDetailsItemViewModel : BaseViewModel
{
    private string _firstLabel;
    private string _secondLabel;
    private string _firstImageFilePath;
    private string _secondImageFilePath;
    private bool _hasInfoSign;

    /// <summary>
    /// Gets or sets the first label.
    /// </summary>
    public string FirstLabel
    {
        get => _firstLabel;
        set
        {
            _firstLabel = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the second label.
    /// </summary>
    public string SecondLabel
    {
        get => _secondLabel;
        set
        {
            _secondLabel = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the first image file path.
    /// </summary>
    public string FirstImageFilePath
    {
        get => _firstImageFilePath;
        set
        {
            _firstImageFilePath = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the second image file path.
    /// </summary>
    public string SecondImageFilePath
    {
        get => _secondImageFilePath;
        set
        {
            _secondImageFilePath = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the Info sign is visible.
    /// </summary>
    public bool HasInfoSign
    {
        get => _hasInfoSign;
        set
        {
            _hasInfoSign = value;
            OnPropertyChanged();
        }
    }
}