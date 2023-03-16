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

    AVAudioEngine engine;
    AVAudioMixerNode mixerNode;

    private void SetupEngine()
    {
        engine = new AVAudioEngine();
        mixerNode = new AVAudioMixerNode();

        // Set volume to 0 to avoid audio feedback while recording.
        mixerNode.Volume = 0;

        engine.AttachNode(mixerNode);

        MakeConnections();

        // Prepare the engine in advance, in order for the system to allocate the necessary resources.
        engine.Prepare();
    }

    private void MakeConnections()
    {
        var inputNode = engine.InputNode;
        var inputFormat = inputNode.GetBusOutputFormat(0);
        engine.Connect(inputNode, mixerNode, inputFormat);

        var mainMixerNode = engine.MainMixerNode;
        var mixerFormat = new AVAudioFormat(AVAudioCommonFormat.PCMFloat32, inputFormat.SampleRate, 1, false);
        engine.Connect(mixerNode, mainMixerNode, mixerFormat);
    }

    public void StartRecording()
    {
        var tapNode = mixerNode;
        var format = tapNode.GetBusOutputFormat(0);

        tapNode.InstallTapOnBus(0, 4096, format, (buffer, time) => {
            System.Diagnostics.Debug.WriteLine(buffer.FrameLength);
        });

        engine.StartAndReturnError(out NSError error);
        if (error != null)
        {
            throw new Exception(error.LocalizedDescription);
        }
    }

    public void ResumeRecording()
    {
        engine.StartAndReturnError(out NSError error);
        if (error != null)
        {
            throw new Exception(error.LocalizedDescription);
        }
       // state = RecordingState.Recording;
    }

    public void PauseRecording()
    {
        engine.Pause();
      //  state = RecordingState.Paused;
    }

    public void StopRecording()
    {
        // Remove existing taps on nodes
        mixerNode.RemoveTapOnBus(0);

        engine.Stop();
      //  state = RecordingState.Stopped;
    }

    public void AudioTest()
	{
        AVAudioSession audioSession = AVAudioSession.SharedInstance();
        audioSession.RequestRecordPermission((bool granted) =>
        {
            if (granted)
            {
                this.SetupEngine();

                this.StartRecording();
            }
            else
            {
                // Handle permission denied...
            }
        });
    }
#endif
}

