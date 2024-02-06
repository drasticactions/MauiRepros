using Foundation;

namespace DisposeTest;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	public static MauiApp CurrentApp { get; private set; }

	protected override MauiApp CreateMauiApp()
	{
		return AppDelegate.CurrentApp = MauiProgram.CreateMauiApp();
	}
}
