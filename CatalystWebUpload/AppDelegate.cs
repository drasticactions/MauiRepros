using Drastic.PureLayout;

namespace CatalystWebUpload;

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
		var vc = new TestViewController ();
		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();

		return true;
	}
}

public class TestViewController : UIViewController
{
	private WebKit.WKWebView webview;

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();

		webview = new WebKit.WKWebView (View.Bounds, new WebKit.WKWebViewConfiguration ());
		View.AddSubview (webview);
		webview.TranslatesAutoresizingMaskIntoConstraints = false;
		webview.AutoPinEdgesToSuperviewEdges ();
		var url = NSUrl.FromString ("https://www.w3schools.com/tags/tryit.asp?filename=tryhtml5_input_type_file");
		var request = new NSUrlRequest (url);
		webview.LoadRequest (request);
	}
}
