using System.Collections.ObjectModel;

namespace CollectionViewVirt
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();
        }
    }

    public class MainPageViewModel
    {
        public MainPageViewModel()
        {
            var items = new List<string>();
            for (int i = 0; i < 100000; i++)
            {
                items.Add(i.ToString());
            }
            this.Items = new ObservableCollection<string>(items);
        }

        public ObservableCollection<string> Items { get; set; } = new ObservableCollection<string>();
    }
}
