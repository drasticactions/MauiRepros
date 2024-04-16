namespace MauiThrowTest
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            if (!this.Working)
            {
                new Thread(this.DoWork).Start();
            }

            this.Working = true;

            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private bool Working = false;

        private void DoWork()
        {
            while (true)
            {
                try
                {
                    throw new Exception("This is a test exception");
                }
                catch (Exception e)
                {
                }
            }
        }
    }

}
