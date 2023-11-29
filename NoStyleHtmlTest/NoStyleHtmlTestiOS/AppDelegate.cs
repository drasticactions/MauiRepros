namespace NoStyleHtmlTestiOS;

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
		var vc = new UIViewController ();
		var webview = new WebKit.WKWebView(Window!.Frame, new WebKit.WKWebViewConfiguration())
		{
			AutoresizingMask = UIViewAutoresizing.All
		};
		webview.AccessibilityActivate();
		vc.View!.AddSubview (webview);
		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();
		string htmlContent = "<html><body><p>.NET MAUI test test parapgraph</p></body></html>";
		webview.LoadHtmlString(htmlContent, null);
		return true;
	}
}
