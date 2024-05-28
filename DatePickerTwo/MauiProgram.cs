using Microsoft.Extensions.Logging;
#if IOS || MACCATALYST
using UIKit;
#endif


namespace DatePickerTwo;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		#if IOS || MACCATALYST
		Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping("DatePicker", (handler, view) =>
		{
			var picker = ((UIDatePicker)(handler.PlatformView.InputView));
			if (OperatingSystem.IsIOSVersionAtLeast(13, 4))
			{
				picker.PreferredDatePickerStyle = UIDatePickerStyle.Compact;
			}
		});
		#endif
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
