namespace CatalystMultiWindowTest;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
	public override UIWindow? Window {
		get;
		set;
	}

	public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
	{
		return true;
	}

	/// <inheritdoc/>
        public override UISceneConfiguration GetConfiguration(UIApplication application, UISceneSession connectingSceneSession, UISceneConnectionOptions options)
        {
            return new UISceneConfiguration("SceneDelegate", connectingSceneSession.Role);
        }

		public override bool ShouldSaveApplicationState (UIApplication application, NSCoder coder)
		{
			return false;
		}

		public override bool ShouldRestoreApplicationState (UIApplication application, NSCoder coder)
		{
			return false;
		}
}
