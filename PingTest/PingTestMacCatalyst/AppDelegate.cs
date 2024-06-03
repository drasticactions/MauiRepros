using System.Net.NetworkInformation;

namespace PingTestMacCatalyst;

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
	UIButton pingButton;
	UILabel resultLabel;

	public override void ViewDidLoad()
	{
		base.ViewDidLoad();
		this.View.BackgroundColor = UIColor.White;
		// Set up the button
		pingButton = UIButton.FromType(UIButtonType.System);
		pingButton.Frame = new CoreGraphics.CGRect(50, 100, 200, 50);
		pingButton.SetTitle("Ping", UIControlState.Normal);
		pingButton.TouchUpInside += PingButton_TouchUpInside;

		// Set up the label to show results
		resultLabel = new UILabel(new CoreGraphics.CGRect(50, 200, 300, 50));
		resultLabel.TextColor = UIColor.Black;

		// Add the button and label to the view
		View.AddSubview(pingButton);
		View.AddSubview(resultLabel);
	}

	async void PingButton_TouchUpInside(object sender, EventArgs e)
	{
		try
		{
			using (var ping = new Ping())
			{
				var reply = await ping.SendPingAsync("www.google.com");
				if (reply.Status == IPStatus.Success)
				{
					resultLabel.Text = $"Ping to {reply.Address}: {reply.RoundtripTime} ms";
				}
				else
				{
					resultLabel.Text = $"Ping failed: {reply.Status}";
				}
			}
		}
		catch (Exception ex)
		{
			resultLabel.Text = $"Ping failed: {ex.Message}";
		}
	}
}
