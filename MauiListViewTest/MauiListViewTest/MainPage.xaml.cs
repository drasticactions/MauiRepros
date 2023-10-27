using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiListViewTest
{
    public partial class MainPage : ContentPage
    {
        MainPageViewModel vm;
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = vm = new MainPageViewModel();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var random = new Random();
            this.vm.StringList.Insert(0, new TestItem() { Name = "Test", Color = Color.FromRgba(random.Next(255), random.Next(255), random.Next(255), 255), Height = random.Next(100, 200), Width = random.Next(100, 200) });
            //this.vm.Update();
            //   this.OnPropertyChanged(nameof(this.Strings));
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            var children = (ViewCell)this.NewList.TemplatedItems[0];
        }
    }

    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<TestItem> StringList { get; set; } = new ObservableCollection<TestItem> { 
            new TestItem() { Name = "Base One", Color = Color.FromRgba(255, 255, 0, 255), Height = 500, Width = 100 },
            new TestItem() { Name = "Base Two", Color = Color.FromRgba(0, 255, 255, 255), Height = 100, Width = 100 },
        };

        public void Update()
        {
            this.OnPropertyChanged(nameof(this.StringList));
        }

        /// <summary>
        /// On Property Changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            Microsoft.Maui.Controls.Application.Current.Dispatcher?.Dispatch(() =>
            {
                var changed = this.PropertyChanged;
                if (changed == null)
                {
                    return;
                }

                changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }
    }

    public class TestItem
    {
        public string Name { get; set; }
        public Color Color { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}
