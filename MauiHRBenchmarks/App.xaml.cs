using Microsoft.Extensions.Logging.Debug;

namespace MauiHRBenchmarks;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}
	
	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new UIToolsWindow(new BlankPage(), new DebugLoggerProvider().CreateLogger("UITools"));
	}
}
