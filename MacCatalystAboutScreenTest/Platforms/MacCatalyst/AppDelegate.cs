using Foundation;
using ObjCRuntime;
using UIKit;

namespace MacCatalystAboutScreenTest;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	[Export("buildMenuWithBuilder:")]
	public void BuildMenu(IUIMenuBuilder builder)
	{
		if (builder.System == UIMenuSystem.MainSystem)
		{
			var about = UIMenuIdentifier.About.ToString();
			var menu = builder.GetMenu(about);
			builder.ReplaceChildrenOfMenu(about, oldChildren =>
            {
                var menuElement = oldChildren.First();
                if (menuElement is UICommand uiCommand)
                {
                    var aboutUICommand = UIAction.Create(uiCommand.Title, null, null, action =>
                    {
                        // Handle the aboutApp action here
                    });
                    return new[] { aboutUICommand };
                }
                else
                {
                    return oldChildren;
                }
            }); 
			// var aboutCommand = UIAction.Create("About", null, null, (arg) =>
			// {
			// 	var alert = UIAlertController.Create("About", "This is a test", UIAlertControllerStyle.Alert);
			// 	alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
			// 	UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alert, true, null);
			// });
			// builder.Ins(aboutCommand, UIMenuItemIdentifier.Help, UIMenuPosition.After);
		}
	}
	
	private UIMenu CreateMainMenu()
	{
		var aboutCommand = UIAction.Create("About", null, null, (arg) =>
		{
			
		});
		//UIMenuSystem.MainSystem.
		return UIMenu.Create(new UIMenuElement[] { aboutCommand });
	}
}
