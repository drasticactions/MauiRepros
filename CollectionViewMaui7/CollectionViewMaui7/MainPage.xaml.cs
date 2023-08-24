namespace CollectionViewMaui7
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            this.CollectionViewCount.Text = $"Logical Children: {this.CollectionView.LogicalChildren.Count()}";
        }
    }

}
