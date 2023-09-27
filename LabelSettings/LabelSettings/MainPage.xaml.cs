namespace LabelSettings
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            this.LabelOptions2.Text = this.LayoutOptionsToString(this.LabelOptions.HorizontalOptions);
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {

            //count++;

            //if (count == 1)
            //    CounterBtn.Text = $"Clicked {count} time";
            //else
            //    CounterBtn.Text = $"Clicked {count} times";

            //SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (this.LabelOptions.HorizontalOptions == LayoutOptions.Start)
            {
                this.LabelOptions.HorizontalOptions = LayoutOptions.End;
            }
            else
            {
                this.LabelOptions.HorizontalOptions = LayoutOptions.Start;
            }

            this.LabelOptions2.Text = this.LayoutOptionsToString(this.LabelOptions.HorizontalOptions);
        }

        private string LayoutOptionsToString(LayoutOptions options)
        {
           if (options == LayoutOptions.Start)
            {
                return "Start";
            }
            else if (options == LayoutOptions.Center)
            {
                return "Center";
            }
            else if (options == LayoutOptions.End)
            {
                return "End";
            }
            else if (options == LayoutOptions.Fill)
            {
                return "Fill";
            }
            else if (options == LayoutOptions.FillAndExpand)
            {
                return "FillAndExpand";
            }
            else
            {
                return "Default";
            }
        }
    }
}
