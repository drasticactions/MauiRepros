using AVFoundation;

namespace CatalystAudio;

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

        var testButton = new UIButton(Window!.Frame)
        { AutoresizingMask = UIViewAutoresizing.All};
        testButton.SetTitle("Test", UIControlState.Normal);
        testButton.TouchDown += TestButton_TouchDown;
		vc.View!.AddSubview (testButton);
		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();

		return true;
	}

    private void TestButton_TouchDown(object? sender, EventArgs e)
    {
        this.InvokeOnMainThread(() => this.AudioTest());
    }

    public void AudioTest()
    {
        AVAudioSession audioSession = AVAudioSession.SharedInstance();
        audioSession.RequestRecordPermission((bool granted) =>
        {
            if (granted)
            {
                NSError error;

                audioSession.SetCategory(AVAudioSessionCategory.PlayAndRecord);
                double preferredSampleRate = 44100;
                audioSession.SetPreferredSampleRate(preferredSampleRate, out error);
                AVAudioSessionPortDescription[] inputs = audioSession.AvailableInputs;
                if (inputs.Length > 0)
                {
                    AVAudioSessionPortDescription input = inputs[0];
                    audioSession.SetPreferredInput(input, out error);
                }
                audioSession.SetActive(true, out error);
            }
            else
            {
                // Handle permission denied...
            }
        });
    }
}
