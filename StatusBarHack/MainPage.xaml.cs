
using Microsoft.Maui.Platform;
#if IOS
using UIKit;
#endif

namespace StatusBarHack;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
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

	protected override void OnHandlerChanged()
	{
		base.OnHandlerChanged();
		
#if IOS
		var window = this.GetParentWindow()?.Handler?.PlatformView as UIWindow;
		if (window is not null)
		{
			var topPadding = window?.SafeAreaInsets.Top ?? 0;

			var statusBar = new UIView(new CoreGraphics.CGRect(0, 0, UIScreen.MainScreen.Bounds.Size.Width, topPadding))
			{
				BackgroundColor = Colors.Green.ToPlatform()
			};
			
			var view = this.Handler?.PlatformView as UIView;
			if (view is not null)
			{
				view?.AddSubview(statusBar);
			}
		}
#endif
	}
}

