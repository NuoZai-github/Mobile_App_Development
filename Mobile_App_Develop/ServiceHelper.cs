using System;
using Microsoft.Extensions.DependencyInjection;

namespace Mobile_App_Develop
{
    // 简单的服务定位器，用于在 XAML 实例化页面时获取已注册的服务
    public static class ServiceHelper
    {
        private static IServiceProvider? _services;

        public static void Initialize(IServiceProvider services)
        {
            _services = services;
        }

        public static T GetService<T>() where T : notnull
        {
            if (_services == null)
            {
                throw new InvalidOperationException("ServiceHelper is not initialized.");
            }
            return _services.GetRequiredService<T>();
        }
    }
}