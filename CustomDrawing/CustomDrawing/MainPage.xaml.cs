namespace CustomDrawing
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }


        private void Button_OnClicked(object? sender, EventArgs e)
        {
            this.ButtonColor.Text = ((SolidColorBrush)BoxDrawing.ContainerBackground!).Color.ToString();
        }
    }
}
