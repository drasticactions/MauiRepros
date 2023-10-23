using Microsoft.Extensions.Logging;

namespace MauiWebviewInspect;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
       #if MACCATALYST
       Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("Inspect", (handler, view) =>
	       {
		       if (OperatingSystem.IsMacCatalystVersionAtLeast(16, 4))
		       {
			       handler.PlatformView.Inspectable = true;
			       // For older versions that don't include the Inspectable field.
			       // handler.PlatformView.SetValueForKey(Foundation.NSObject.FromObject(true), new Foundation.NSString("inspectable"));
		       }
       });
       #endif
		var builder = MauiApp.CreateBuilder();
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
