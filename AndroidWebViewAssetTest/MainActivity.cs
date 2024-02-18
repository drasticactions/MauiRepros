using Android.Content;
using Android.OS;
using Android.Util;
using Android.Webkit;
using AndroidX.WebKit;
using Java.Lang;

namespace AndroidWebViewAssetTest;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
        {
            Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
        }

        SetContentView(Resource.Layout.activity_main);

        var webView = FindViewById<DrasticAndroidWebView>(Resource.Id.myWebView);

        webView.LoadUrl("https://webapp/index.html");
    }
}

public class DrasticAndroidWebView : WebView
{

    public DrasticAndroidWebView(Context context, IAttributeSet? attrs) : base(context, attrs)
    {
        this.Settings.JavaScriptEnabled = true;
        this.SetWebViewClient(new AndroidWebViewClient(context));
    }
}

public class AndroidWebViewClient : WebViewClientCompat
{
    readonly WebViewAssetLoader loader;

    public AndroidWebViewClient(Context ctx)
    {
        this.loader = new WebViewAssetLoader.Builder()
            .SetDomain("webapp")
            .AddPathHandler("/", new WebViewAssetLoader.AssetsPathHandler(ctx))
            .Build();
    }

    [Override]
    public override WebResourceResponse? ShouldInterceptRequest(Android.Webkit.WebView? view, IWebResourceRequest? request)
    {
        var url = request?.Url;
        if (url is null) 
        { 
            return null;
        }

        var result = this.loader.ShouldInterceptRequest(request!.Url!);
        return result;
    }
}