namespace MauiApp3;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();

        Accelerometer.Default.ShakeDetected += Accelerometer_ShakeDetected;
        Accelerometer.Default.Start(SensorSpeed.Game);
    }

    private void Accelerometer_ShakeDetected(object sender, EventArgs e)
    {
    }
}
