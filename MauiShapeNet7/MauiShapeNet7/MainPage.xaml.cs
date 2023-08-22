using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Platform;

namespace MauiShapeNet7
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
            var shape = this.BorderTest.StrokeShape as RoundRectangle;
            if (shape != null)
            {
                try
                {
                    var rawShape = shape.Handler;
                    if (rawShape != null)
                    {
                        this.CounterBtn.Text = "Border Shape Not Null!";
                    }
                    else
                    {
                        this.CounterBtn.Text = "Border Shape Null!";
                    }
                }
                catch (Exception ex)
                {
                    this.CounterBtn.Text = $"Error: {ex.Message}";
                }
            }
        }
    }

}
