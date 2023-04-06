using ObjCRuntime;
using Masonry;
namespace MacOSWebviewTest;

public partial class ViewController : NSViewController {

	private NSView webviewHolder;
	private NSButton button;
	private WebKit.WKWebView webview;

	protected ViewController (NativeHandle handle) : base (handle)
	{
		// This constructor is required if the view controller is loaded from a xib or a storyboard.
		// Do not put any initialization here, use ViewDidLoad instead.
	}

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();

		this.webviewHolder = new NSView();
		this.webviewHolder.Layer = new CoreAnimation.CALayer() { BackgroundColor = CGColor.CreateSrgb(225, 225, 225, 1) };

		this.button = new NSButton() { Title = "Execute JS in WebView", BezelStyle = NSBezelStyle.Rounded };
        this.button.Activated += Button_Activated;

        this.View!.AddSubview(webviewHolder);
        this.View!.AddSubview(this.button);

        var configuration = new WebKit.WKWebViewConfiguration();
        configuration.Preferences.SetValueForKey(NSNumber.FromBoolean(true), new NSString("developerExtrasEnabled"));
        this.webviewHolder.MakeConstraints(make => {
            make.Top.And.Left.And.Right.EqualTo(this.View!);
            make.Bottom.EqualTo(this.View).With.Offset(-200);
        });

        webview = new WebKit.WKWebView(this.webviewHolder.Bounds, configuration);

        this.webviewHolder.AddSubview(webview);

        webview.MakeConstraints(make => make.Top.And.Bottom.And.Left.And.Right.EqualTo(this.webviewHolder!));

        this.button.MakeConstraints(make => {
            make.Left.And.Right.EqualTo(this.View!);
            make.Bottom.EqualTo(this.View!).With.Offset(-50);
        });

        webview.LoadRequest(new NSUrlRequest(new NSUrl("https://digital.fidelity.com/stgw/digital/fidchart/consumers/atp/0.0.4/atp-chart.html")));
    }

    private async void Button_Activated(object? sender, EventArgs e)
    {
        string chartOptions = """ { "header":false,"isAuthenticated":true,"symbol":"AAPL","isPartial":false,"displays":{ "default":"candle" },"timeframe":{ "default":"TODAY","items":["TODAY","2D","5D","10D","1M","3M","6M","YTD","1Y","2Y","5Y","10Y","MAX"] },"frequency":{ "default":"5m","items":["1m","5m","10m","15m","30m","1H","4H","1D","1W","1M","1Q","1Y"] },"study":false,"toggle":{ "savedChartsMenu":true,"patternEventsMenu":false,"openOrders":false }} """;
        string datasourceOptions = """ { "env":"dev", "productid":"chartplatform", "realtime":false, "uuid":"d94167ee-1b27-11dc-ab78-ac196951aa77" } """;

        var result = await this.webview.EvaluateJavaScriptAsync(new NSString($"displayInitialChart({chartOptions},{datasourceOptions})"));
    }

    public override NSObject RepresentedObject {
		get => base.RepresentedObject;
		set {
			base.RepresentedObject = value;

			// Update the view, if already loaded.
		}
	}
}
