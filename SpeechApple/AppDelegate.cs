using Microsoft.CognitiveServices.Speech;
using static System.Net.Mime.MediaTypeNames;

namespace SpeechApple;

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
		var button = new UIButton(Window!.Frame) { AutoresizingMask = UIViewAutoresizing.All };
		button.SetTitle("Startup Service", UIControlState.Normal);
        button.TouchDown += Button_TouchDown;
		var vc = new UIViewController ();
		vc.View!.AddSubview (button);
		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();

		return true;
	}

    private async void Button_TouchDown(object? sender, EventArgs e)
    {
        string subscriptionKey = "****";
        string subscriptionRegion = "***";

        var config = SpeechConfig.FromSubscription(subscriptionKey, subscriptionRegion);
        config.SpeechSynthesisVoiceName = "zh-CN-YunxiNeural";

        // use the default speaker as audio output.
        using (var synthesizer = new SpeechSynthesizer(config))
        {
            using (var result = await synthesizer.SpeakTextAsync("小心踩到客厅里的玻璃碎片，但是房东已经打扫过了"))
            {
                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                }
                else
                {
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                }
            }
        }
    }
}
