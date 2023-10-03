using Microsoft.Extensions.Logging;

namespace MauiEditor;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		#if IOS
		Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("EditorChange", (handler, view) =>
		{
			handler.PlatformView.InputAccessoryView = null;
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
