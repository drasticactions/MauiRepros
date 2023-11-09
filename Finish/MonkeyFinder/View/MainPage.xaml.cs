using Microsoft.Maui.Platform;

namespace MonkeyFinder.View;

public partial class MainPage : ContentPage
{
    public MainPage(MonkeysViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        VisualDiagnostics.VisualTreeChanged += (sender, e) =>
        {
            if (e.ChangeType == VisualTreeChangeType.Add)
            {
                Debug.WriteLine($"Added {e.Child.GetType().Name} to {e.Parent.GetType().Name} with Child Index {e.ChildIndex}");
            }

            if (e.ChangeType == VisualTreeChangeType.Remove)
            {
               Debug.WriteLine($"Removed {e.Child.GetType().Name} from {e.Parent.GetType().Name} with Child Index {e.ChildIndex}");
            }
        };
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        this.TestView.ItemsSource = new ObservableCollection<Monkey>();
    }
}

