using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;

namespace MauiAnimationTest;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		
	}
	
	private int boxCount = 0;
	private void OnAddBoxClicked(object sender, EventArgs e)
	{
		var border = new Border() { WidthRequest = 50, HeightRequest = 50, BackgroundColor = Color.FromHex("#00FF80"), Stroke = Color.FromHex("#194D33")};
		border.StrokeShape = new RoundRectangle() { CornerRadius = 5};
		border.Shadow = new Shadow()  { Opacity = .3f, Offset = new Point(40, 40), Radius = 5, Brush = new SolidColorBrush(Color.FromRgb(0, 0, 0))};
		// Position the new box randomly within the AbsoluteLayout
		AbsoluteLayout.SetLayoutBounds(border, new Rect(
			new Random().NextDouble(), // X-coordinate (random)
			new Random().NextDouble(), // Y-coordinate (random)
			0.2, // Width
			0.2 // Height
		));

		AbsoluteLayout.SetLayoutFlags(border, AbsoluteLayoutFlags.All);

		AbsoluteLayoutView.Children.Add(border);

		boxCount++;
		
		RotateBox(border);
	}
	
	private async void RotateBox(View element)
	{
		while (true)
		{
			await AnimationHelper.RotateElement(element, 2.0, 1); // Rotate 360 degrees in 2 seconds
			element.Rotation = 0; // Reset rotation to 0 degrees
		}
	}

	public static class AnimationHelper
	{
		public static async Task RotateElement(View element, double durationSeconds, double rotations)
		{
			await element.RotateTo(360 * rotations, (uint)(durationSeconds * 1000));
		}
	}
	
}

