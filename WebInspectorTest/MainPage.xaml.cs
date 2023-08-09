#if IOS
using Foundation;
using WebKit;
#endif

namespace WebInspectorTest;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	protected override void OnHandlerChanged()
	{
		base.OnHandlerChanged();
		#if IOS
		if (blazorWebView.Handler?.PlatformView is WebKit.WKWebView webview)
		{
			webview.SetValueForKey(NSObject.FromObject(true), new NSString("inspectable"));
		}
		#endif
	}
}
