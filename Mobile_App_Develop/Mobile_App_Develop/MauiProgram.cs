using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Maps;
using Mobile_App_Develop.Services;
using Mobile_App_Develop.Views;
using CommunityToolkit.Maui;
using Plugin.LocalNotification;
using Supabase;

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
            // 注册 Supabase 客户端（从环境变量读取 URL 和 Anon Key）
            builder.Services.AddSingleton<Client>(sp =>
            {
                var url = SupabaseConstants.GetUrl();
                var key = SupabaseConstants.GetAnonKey();
                var options = new SupabaseOptions
                {
                    AutoRefreshToken = true,
                    AutoConnectRealtime = false
                };
                return new Client(url, key, options);
            });

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

            // 构建应用并初始化服务定位器，确保页面的无参构造可用 DI
            var app = builder.Build();
            ServiceHelper.Initialize(app.Services);
            return app;
        }
    }
}
