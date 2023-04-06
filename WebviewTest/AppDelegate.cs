using Masonry;
using UIKit;

namespace WebviewTest;

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
		var vc = new TestViewController();
		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();

        
        return true;
	}
}


public class TestViewController : UIViewController
{
	private UIView webviewHolder;
	private UIButton button;
	private WebKit.WKWebView webview;

	public TestViewController()
	{
		this.button = new UIButton();
		this.button.SetTitle("Execute JS in WebView", UIControlState.Normal);
        this.button.PrimaryActionTriggered += Button_PrimaryActionTriggered;
		this.webviewHolder = new UIView();
		this.webviewHolder.BackgroundColor = UIColor.Green;
		this.View!.Add(webviewHolder);
		this.View!.Add(this.button);

        var configuration = new WebKit.WKWebViewConfiguration();
		// configuration.Preferences.SetValueForKey(NSNumber.FromBoolean(true), new NSString("developerExtrasEnabled"));
		this.webviewHolder.MakeConstraints(make => {
			make.Top.And.Left.And.Right.EqualTo(this.View!);
			make.Bottom.EqualTo(this.View).With.Offset(-200);
        });

        webview = new WebKit.WKWebView(this.webviewHolder.Bounds, configuration);

        this.webviewHolder.Add(webview);

        webview.MakeConstraints(make => make.Top.And.Bottom.And.Left.And.Right.EqualTo(this.webviewHolder!));

		this.button.MakeConstraints(make => {
			make.Left.And.Right.EqualTo(this.View!);
			make.Bottom.EqualTo(this.View!).With.Offset(-50);
		});

        webview.LoadRequest(new NSUrlRequest(new NSUrl("https://digital.fidelity.com/stgw/digital/fidchart/consumers/atp/0.0.4/atp-chart.html")));
    }

    private async void Button_PrimaryActionTriggered(object? sender, EventArgs e)
    {
        string chartOptions = """ { "header":false,"isAuthenticated":true,"symbol":"AAPL","isPartial":false,"displays":{ "default":"candle" },"timeframe":{ "default":"TODAY","items":["TODAY","2D","5D","10D","1M","3M","6M","YTD","1Y","2Y","5Y","10Y","MAX"] },"frequency":{ "default":"5m","items":["1m","5m","10m","15m","30m","1H","4H","1D","1W","1M","1Q","1Y"] },"study":false,"toggle":{ "savedChartsMenu":true,"patternEventsMenu":false,"openOrders":false }} """;
        string datasourceOptions = """ { "env":"dev", "productid":"chartplatform", "realtime":false, "uuid":"d94167ee-1b27-11dc-ab78-ac196951aa77" } """;

		var result = await this.webview.EvaluateJavaScriptAsync(new NSString($"displayInitialChart({chartOptions},{datasourceOptions})"));
    }
}