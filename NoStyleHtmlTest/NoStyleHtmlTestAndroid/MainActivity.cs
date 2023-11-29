using Android.Webkit;

namespace NoStyleHtmlTestAndroid;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Set our view from the "main" layout resource
        SetContentView(Resource.Layout.activity_main);

        var web_view = FindViewById<WebView>(Resource.Id.webview);
        string htmlContent = "<html><body><p>.NET MAUI test test parapgraph</p></body></html>";
        web_view.LoadData(htmlContent, "text/html", "UTF-8");
    }
}