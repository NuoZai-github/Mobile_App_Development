using System;

namespace Mobile_App_Develop.Services
{
    // 开发阶段的常量配置（请在部署前改为安全存储方案）
    public static class SupabaseConstants
    {
        public static string GetUrl()
        {
            var url = Environment.GetEnvironmentVariable("SUPABASE_URL");
            if (string.IsNullOrEmpty(url))
            {
                throw new InvalidOperationException("SUPABASE_URL environment variable not set.");
            }
            return url;
        }

        public static string GetAnonKey()
        {
            var anonKey = Environment.GetEnvironmentVariable("SUPABASE_ANON_KEY");
            if (string.IsNullOrEmpty(anonKey))
            {
                throw new InvalidOperationException("SUPABASE_ANON_KEY environment variable not set.");
            }
            return anonKey;
        }
    }
}