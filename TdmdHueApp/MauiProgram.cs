using Microsoft.Extensions.Logging;
using TdmdHueApp.Domain.Model;
using TdmdHueApp.Domain.Services;
using TdmdHueApp.infrastucture;
using TdmdHueApp;

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
            builder.Services.AddTransient<ExtractUsername>();
            builder.Services.AddSingleton<HttpClient>();

            builder.Services.AddSingleton<IPreferences>(p => Preferences.Default);
            builder.Services.AddSingleton<IBridgeConnectorHueLights, BridgeConnector>();

            return builder.Build();
        }
    }
}
