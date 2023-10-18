
namespace MauiSceneDelegateTest;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		// MainPage = new AppShell();
	}

    protected override Window CreateWindow(IActivationState activationState)
    {
		// Workaround for https://github.com/dotnet/maui/issues/18093
		// Create a Window by overriding CreateWindow and don't use the Single MainPage instance.
        return new Window(new AppShell());
    }
}
