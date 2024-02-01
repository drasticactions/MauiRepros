using Microsoft.Extensions.Logging;

namespace EditorTest;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("SmartQuote", (handler, view) =>
		{
			#if IOS || MACCATALYST
			handler.PlatformView.TintColor = UIKit.UIColor.Green;
			handler.PlatformView.SmartQuotesType = UIKit.UITextSmartQuotesType.No;
			handler.PlatformView.SmartDashesType = UIKit.UITextSmartDashesType.No;
			handler.PlatformView.SmartInsertDeleteType = UIKit.UITextSmartInsertDeleteType.No;
			#endif
		});
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
