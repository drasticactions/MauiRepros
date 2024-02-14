namespace MauiStackResize
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            DeviceDisplay.Current.MainDisplayInfoChanged += Current_MainDisplayInfoChanged;

            InitializeComponent();

            BindingContext = this;
        }


        private void Button_Clicked(object sender, EventArgs e)
        {
            if (this.StackLayoutOrientation == StackOrientation.Vertical)
            {
                this.StackLayoutOrientation = StackOrientation.Horizontal;
            }
            else
            {
                this.StackLayoutOrientation = StackOrientation.Vertical;
            }

            this.OrientationName.Text = this.StackLayoutOrientation.ToString();
        }

        private StackOrientation _stackLayoutOrientation;
        public StackOrientation StackLayoutOrientation
        {
            get { return _stackLayoutOrientation; }
            set
            {
                if (_stackLayoutOrientation != value)
                {
                    _stackLayoutOrientation = value;
                    OnPropertyChanged();
                }
            }
        }

        private void Current_MainDisplayInfoChanged(object? sender, DisplayInfoChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Width: {e.DisplayInfo.Width}");
            System.Diagnostics.Debug.WriteLine($"Height: {e.DisplayInfo.Width}");
            System.Diagnostics.Debug.WriteLine($"Orientation: {e.DisplayInfo.Orientation}");
            if (e.DisplayInfo.Orientation == DisplayOrientation.Landscape)
            {
                StackLayoutOrientation = StackOrientation.Horizontal;
            }
            else
            {
                StackLayoutOrientation = StackOrientation.Vertical;
            }

            this.OrientationName.Text = this.StackLayoutOrientation.ToString();
        }
    }

}
