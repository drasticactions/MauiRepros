using CoreAnimation;

namespace UIKitTextBlurTestMac;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
	public override UIWindow? Window {
		get;
		set;
	}

	public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
	{
		// create a new window instance based on the screen size
		Window = new UIWindow (UIScreen.MainScreen.Bounds);

		// create a UIViewController with a single UILabel
		var vc = new TextLayerViewController ();

		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();

		return true;
	}
}

public class TextLayerViewController : UIViewController
{
	public override void ViewDidLoad()
	{
		base.ViewDidLoad();

		var textLayer = new CATextLayer
		{
			String = "Hello CATextLayer",
			ForegroundColor = UIColor.White.CGColor,
			FontSize = 40,
			TextAlignmentMode = CATextLayerAlignmentMode.Center,
			Frame = new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height / 2)
		};
		
		var textLayer2 = new CATextLayer
		{
			String = "Hello CATextLayer",
			ForegroundColor = UIColor.White.CGColor,
			FontSize = 40,
			ContentsScale = 2.0f,
			TextAlignmentMode = CATextLayerAlignmentMode.Center,
			Frame = new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height / 4)
		};

		textLayer.Position = new CGPoint(View.Bounds.GetMidX(), View.Bounds.GetMidY());
		textLayer2.Position = new CGPoint(View.Bounds.GetMidX(), View.Bounds.GetMidY());
		
		View.Layer.AddSublayer(textLayer);
		View.Layer.AddSublayer(textLayer2);
	}
}