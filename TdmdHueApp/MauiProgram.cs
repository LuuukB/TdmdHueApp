using AndroidX.Lifecycle;
using Microsoft.Extensions.Logging;

namespace TdmdHueApp
{
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

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<ViewModel>();
            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddSingleton<IPreferences>(p => Preferences.Default);
            builder.Services.AddSingleton<IBridgeConnectorHueLights, BridgeConnector>();

            return builder.Build();
        }
    }
}
