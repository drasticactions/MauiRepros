using System.Reflection;
using Masonry;
using WebKit;

namespace WKWebViewShellApp;

public class SampleWebViewController : UIViewController
{
    private UIView webviewHolder;
    private WebKit.WKWebView webview;
    
    public SampleWebViewController()
    {
        this.webviewHolder = new UIView();
        this.webviewHolder.BackgroundColor = UIColor.Green;
        this.View!.Add(webviewHolder);

        var configuration = new WebKit.WKWebViewConfiguration();
        configuration.DefaultWebpagePreferences.AllowsContentJavaScript = true;
        this.webviewHolder.MakeConstraints(make => {
            make.Top.And.Left.And.Right.And.Bottom.EqualTo(this.View!);
            //make.Bottom.EqualTo(this.View).With.Offset(-200);
        });

        webview = new CustomWebView(this.webviewHolder.Bounds, configuration);
        webview.Inspectable = true;
        this.webviewHolder.Add(webview);

        webview.MakeConstraints(make => make.Top.And.Bottom.And.Left.And.Right.EqualTo(this.webviewHolder!));
        var htmlFile = GetResourceFileContent("Samples.test.html");
        
        if (htmlFile is not null)
        {
            var test = new StreamReader(htmlFile).ReadToEnd();
            var html = new NSString(test);
            webview.LoadHtmlString(html, null);
        }
    }
    
    /// <summary>
    /// Get Resource File Content via FileName.
    /// </summary>
    /// <param name="fileName">Filename.</param>
    /// <returns>Stream.</returns>
    public static Stream? GetResourceFileContent(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "WKWebViewShellApp." + fileName;
        if (assembly is null)
        {
            return null;
        }

        return assembly.GetManifestResourceStream(resourceName);
    }
}

public class CustomWebView : WKWebView
{
    public CustomWebView(CGRect url, WKWebViewConfiguration configuration) : base(url, configuration)
    {
    }

    public override void BuildMenu(IUIMenuBuilder builder)
    {
        base.BuildMenu(builder);
    }
}