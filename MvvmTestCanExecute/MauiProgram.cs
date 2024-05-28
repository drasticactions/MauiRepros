using Drastic.Services;
using Microsoft.Extensions.Logging;

namespace MvvmTestCanExecute;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder.Services.AddSingleton<IAppDispatcher, MauiAppDispatcher>();
		builder.Services.AddSingleton<IErrorHandlerService, MauiErrorHandler>();
		builder.Services.AddSingleton<MainPageViewModel>();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
