using System.Diagnostics;

namespace WindowSizeChanged;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}

	protected override Window CreateWindow(IActivationState activationState)
    {
        Window window = base.CreateWindow(activationState);
        window.SizeChanged += UpdateFlyoutBehaviorIfNeeded;
        return window;
    }

    private const double minPageWidth = 400;
    private void UpdateFlyoutBehaviorIfNeeded(object sender, EventArgs e)
    {
        double currentWidth = ((Window)sender).Width;
		Debug.WriteLine($"currentWidth: {currentWidth}");
        // if (currentWidth <= minPageWidth)
        // {
        //     Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
        // }
        // else if (currentWidth > minPageWidth + Shell.Current.FlyoutWidth)
        // {
        //     Shell.Current.FlyoutBehavior = FlyoutBehavior.Locked;
        // }
    }
}
