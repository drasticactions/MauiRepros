
using static System.Net.Mime.MediaTypeNames;

namespace MauiDrawString;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
        InitializeComponent();
		this.TestGraphicsView.Drawable = new DrawStringTest();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}

    public class DrawStringTest : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FontSize = 36;
            canvas.FontColor = Colors.White;
            canvas.SetShadow(offset: new SizeF(2, 2), blur: 1, color: Colors.Black);
            canvas.DrawString("Test!", 0, 0, 200, 200, HorizontalAlignment.Center, VerticalAlignment.Center);
            canvas.StrokeColor = Colors.Blue;
            canvas.StrokeSize = 2;
            canvas.DrawRectangle(0, 0, 200, 200);
        }
    }
}

