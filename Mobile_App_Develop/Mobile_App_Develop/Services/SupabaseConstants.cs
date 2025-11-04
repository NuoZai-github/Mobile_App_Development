using System;

namespace Mobile_App_Develop.Services
{
    // 开发阶段的常量配置（请在部署前改为安全存储方案）
    public static class SupabaseConstants
    {
        // 使用提供的 Supabase 项目 URL 与 anon key
        public const string Url = "https://ysioglppitqwjtxuqhud.supabase.co";
        public const string AnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InlzaW9nbHBwaXRxd2p0eHVxaHVkIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjIxOTY0NjksImV4cCI6MjA3Nzc3MjQ2OX0.QhpBFcKS7sGwLHTiQHcAhNJGTDUT4TvEhO0LJyEbseY";

        public static string GetUrl() =>
            Environment.GetEnvironmentVariable("SUPABASE_URL") ?? Url;

        public static string GetAnonKey() =>
            Environment.GetEnvironmentVariable("SUPABASE_ANON_KEY") ?? AnonKey;
    }
}