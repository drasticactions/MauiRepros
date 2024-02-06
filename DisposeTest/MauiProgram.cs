using Microsoft.Extensions.Logging;

namespace DisposeTest;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		
		builder.Services.AddSingleton<SingletonServiceThatRequiresDispose>();
#if DEBUG
		builder.Logging.AddDebug();
#endif

		var mauiApp = builder.Build();

		var service = mauiApp.Services.GetRequiredService<SingletonServiceThatRequiresDispose>();

		return mauiApp;
	}
}

internal class SingletonServiceThatRequiresDispose : IDisposable
{
	public void Dispose()
	{
		Console.WriteLine("I am never called!");
	}
}
