using Foundation;
using UIKit;

namespace CollectionViewItemTest;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
	
	public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
	{
		//Drastic.WatchdogInspector.TWWatchdogInspector.Start();
		TWWatchdogInspector.Start();
		return base.FinishedLaunching(application, launchOptions);
	}
}
