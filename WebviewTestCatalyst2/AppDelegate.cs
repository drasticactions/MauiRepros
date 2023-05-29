namespace WebviewTestCatalyst;

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
		vc.View!.AddSubview (webview);
		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();

		webview.LoadRequest(new NSUrlRequest(new NSUrl("https://file-examples.com/index.php/sample-documents-download/sample-pdf-download/")));
		return true;
	}
}
