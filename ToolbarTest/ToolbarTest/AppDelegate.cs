using AppKit;

namespace ToolbarTest;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
	public override UIWindow? Window {
		get;
		set;
	}

	public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
	{
		// create a new window instance based on the screen size
		Window = new UIWindow (UIScreen.MainScreen.Bounds);

		// create a UIViewController with a single UILabel
		var vc = new UIViewController ();
		vc.View!.AddSubview (new UILabel (Window!.Frame) {
			BackgroundColor = UIColor.SystemBackground,
			TextAlignment = UITextAlignment.Center,
			Text = "Hello, Mac Catalyst!",
			AutoresizingMask = UIViewAutoresizing.All,
		});

		var toolbar = new NSToolbar();
		Window.WindowScene!.Titlebar!.Toolbar = toolbar;
		toolbar.Delegate = new ToolbarDelegate();

		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();

		return true;
	}
}

public sealed class TestView : UIView
{
	public TestView()
	{
		this.AddSubview(new UILabel(new CGRect(0, 0, 0, 0))
		{
			BackgroundColor = UIColor.Blue,
			TextAlignment = UITextAlignment.Center,
			Text = "Hello, Mac Catalyst!",
			AutoresizingMask = UIViewAutoresizing.All,
		});
	}
}

public class ToolbarDelegate : NSToolbarDelegate
{
private const string Settings = "Settings";

    /// <inheritdoc/>
    public override NSToolbarItem WillInsertItem(NSToolbar toolbar, string itemIdentifier, bool willBeInserted)
    {
        var toolbarItem = new NSUIViewToolbarItem(new NSString(itemIdentifier), new TestView());
        if (itemIdentifier == Settings)
        {
            toolbarItem.Action = new ObjCRuntime.Selector("buttonClickAction:");
            toolbarItem.Target = this;
            toolbarItem.Enabled = true;
        }

        return toolbarItem;
    }

    /// <inheritdoc/>
    public override string[] AllowedItemIdentifiers(NSToolbar toolbar)
    {
        return new string[]
        {
            Settings,
        };
    }

    /// <inheritdoc/>
    public override string[] DefaultItemIdentifiers(NSToolbar toolbar)
    {
        return new string[]
        {
            Settings,
        };
    }

    [Export("buttonClickAction:")]
    public async void ButtonClickAction(NSObject sender)
    {
    }
}
