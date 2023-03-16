#if MACCATALYST || IOS
using AVFoundation;
using Foundation;
using static System.Runtime.InteropServices.JavaScript.JSType;
#endif

namespace AudioTest;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
#if MACCATALYST || IOS
        this.AudioTest();
#endif
	}

#if MACCATALYST || IOS
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
#endif
}

