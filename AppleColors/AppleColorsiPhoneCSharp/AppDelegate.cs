namespace AppleColorsiPhoneCSharp;

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
		var vc = new ViewController ();
		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();

		return true;
	}
}

public class ViewController : UIViewController
{
	public override void ViewDidLoad()
	{
		base.ViewDidLoad();
		this.View!.BackgroundColor = UIColor.SystemBackground;
		
		// Set up the enabled button
		UIButton enabledButton = new UIButton(UIButtonType.System);
		enabledButton.SetTitle("⬛︎⬛︎⬛︎⬛︎⬛︎ Enabled C# ⬛︎⬛︎⬛︎⬛︎⬛︎⬛︎", UIControlState.Normal);
		enabledButton.TouchUpInside += EnabledButtonTapped;
		enabledButton.TranslatesAutoresizingMaskIntoConstraints = false;
		View.AddSubview(enabledButton);
            
		// Set up the disabled button
		UIButton disabledButton = new UIButton(UIButtonType.System);
		disabledButton.SetTitle("⬛︎⬛︎⬛︎⬛︎⬛︎ Disabled C# ⬛︎⬛︎⬛︎⬛︎⬛︎⬛︎", UIControlState.Normal);
		disabledButton.Enabled = false;
		disabledButton.TranslatesAutoresizingMaskIntoConstraints = false;
		View.AddSubview(disabledButton);
            
		// Set up constraints
		NSLayoutConstraint.ActivateConstraints(new[]
		{
			enabledButton.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor),
			enabledButton.CenterYAnchor.ConstraintEqualTo(View.CenterYAnchor, -20),
                
			disabledButton.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor),
			disabledButton.TopAnchor.ConstraintEqualTo(enabledButton.BottomAnchor, 20)
		});
	}
        
	private void EnabledButtonTapped(object sender, EventArgs e)
	{
		Console.WriteLine("Enabled button tapped");
	}
}
