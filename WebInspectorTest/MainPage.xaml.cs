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
#if IOS || MACCATALYST
		var device = UIKit.UIDevice.CurrentDevice;
		var isiPhone = device.SystemName.ToLowerInvariant().Contains("iphone");
		System.Diagnostics.Debug.WriteLine(device.SystemName.ToLowerInvariant());
#endif
	}

	protected override void OnHandlerChanged()
	{
		base.OnHandlerChanged();
		#if IOS
		if (blazorWebView.Handler?.PlatformView is WebKit.WKWebView webview)
		{
			if (OperatingSystem.IsIOSVersionAtLeast(16, 4) || OperatingSystem.IsMacCatalystVersionAtLeast(13, 1)) 
			{
				// Enable Developer Extras for Catalyst/iOS builds for 16.4+
				webview.SetValueForKey(NSObject.FromObject(true), new NSString("inspectable"));
			}
			//webview.SetValueForKey(NSObject.FromObject(true), new NSString("inspectable"));
		}
		#endif
	}
}
