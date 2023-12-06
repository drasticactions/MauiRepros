using Masonry;
using WebKit;

namespace WKWebViewRedirectTestShared;

public class TestWebViewController : UIViewController
{
    private UIView webviewHolder;
    private WebKit.WKWebView webview;

    public TestWebViewController()
    {
        this.webviewHolder = new UIView();
        this.webviewHolder.BackgroundColor = UIColor.Green;
        this.View!.Add(webviewHolder);

        var configuration = new WebKit.WKWebViewConfiguration();
        // configuration.Preferences.SetValueForKey(NSNumber.FromBoolean(true), new NSString("developerExtrasEnabled"));
        this.webviewHolder.MakeConstraints(make => {
            make.Top.And.Left.And.Right.And.Bottom.EqualTo(this.View!);
            //make.Bottom.EqualTo(this.View).With.Offset(-200);
        });

        webview = new WebKit.WKWebView(this.webviewHolder.Bounds, configuration);
        webview.NavigationDelegate = new CustomNavigationDelegate();
        this.webviewHolder.Add(webview);

        webview.MakeConstraints(make => make.Top.And.Bottom.And.Left.And.Right.EqualTo(this.webviewHolder!));
        webview.LoadRequest(new NSUrlRequest(new NSUrl("https://archiverse.guide")));

    }
    
    public class CustomNavigationDelegate : WKNavigationDelegate
    {
        public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            var currentUrl = webView.Url;
            System.Diagnostics.Debug.WriteLine($"URL changed to: {currentUrl}");
        }
    }
}