
using Microsoft.Extensions.Logging;

namespace SliderTest
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

#if ANDROID
            Microsoft.Maui.Handlers.SliderHandler.Mapper.AppendToMapping("SliderChange", (handler, view) =>
            {
                var test = handler.PlatformView;
                var bounds = test.ProgressDrawable.Bounds;
                var rect = new Android.Graphics.Rect(0, 0, bounds.Width(), bounds.Height() + 100);
                test.ProgressDrawable.Bounds = rect;
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
}
