using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Maps;
using Mobile_App_Develop.Services;
using Mobile_App_Develop.Views;
using CommunityToolkit.Maui;
using Plugin.LocalNotification;

namespace Mobile_App_Develop
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseLocalNotification()
                .UseMauiMaps()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // 注册服务
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IBusService, BusService>();
            builder.Services.AddSingleton<Mobile_App_Develop.Services.INotificationService, NotificationService>();

            // 注册Shell和页面
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<MapPage>();
            builder.Services.AddTransient<NotificationsPage>();
            builder.Services.AddTransient<ProfilePage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
