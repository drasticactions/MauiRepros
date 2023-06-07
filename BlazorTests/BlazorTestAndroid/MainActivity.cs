using Android.Webkit;

namespace BlazorTestAndroid;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
    WebView web_view;


    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Set our view from the "main" layout resource
        SetContentView(Resource.Layout.activity_main);

        web_view = FindViewById<WebView>(Resource.Id.webview);
        web_view.Settings.JavaScriptEnabled = true;
        web_view.SetWebViewClient(new HelloWebViewClient());
        web_view.LoadUrl("http://192.168.50.199:5001/");
    }

    public class HelloWebViewClient : WebViewClient
    {
        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            view.LoadUrl(request.Url.ToString());
            return false;
        }
    }
}