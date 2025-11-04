using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Maui.Storage;

namespace Mobile_App_Develop.Services
{
    // 开发阶段的常量配置（请在部署前改为安全存储方案）
    public static class SupabaseConstants
    {
        private static Dictionary<string, string>? _config;

        private static void LoadConfigIfNeeded()
        {
            if (_config != null) return;
            try
            {
                using var stream = FileSystem.OpenAppPackageFileAsync("supabase.json").GetAwaiter().GetResult();
                using var reader = new StreamReader(stream);
                var json = reader.ReadToEnd();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                string? url = null;
                string? anonKey = null;

                if (root.TryGetProperty("supabase", out var supabase))
                {
                    if (supabase.TryGetProperty("url", out var urlProp)) url = urlProp.GetString();
                    if (supabase.TryGetProperty("anonKey", out var keyProp)) anonKey = keyProp.GetString();
                }
                else
                {
                    if (root.TryGetProperty("SUPABASE_URL", out var urlProp)) url = urlProp.GetString();
                    if (root.TryGetProperty("SUPABASE_ANON_KEY", out var keyProp)) anonKey = keyProp.GetString();
                }

                _config = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (!string.IsNullOrWhiteSpace(url)) _config["SUPABASE_URL"] = url!;
                if (!string.IsNullOrWhiteSpace(anonKey)) _config["SUPABASE_ANON_KEY"] = anonKey!;
            }
            catch
            {
                _config = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
        }

        public static string GetUrl()
        {
            var url = Environment.GetEnvironmentVariable("SUPABASE_URL");
            if (string.IsNullOrEmpty(url))
            {
                LoadConfigIfNeeded();
                if (_config!.TryGetValue("SUPABASE_URL", out var v) && !string.IsNullOrWhiteSpace(v))
                    return v;
                throw new InvalidOperationException("SUPABASE_URL not configured. Set environment variable or provide Resources/Raw/supabase.json.");
            }
            return url;
        }

        public static string GetAnonKey()
        {
            var anonKey = Environment.GetEnvironmentVariable("SUPABASE_ANON_KEY");
            if (string.IsNullOrEmpty(anonKey))
            {
                LoadConfigIfNeeded();
                if (_config!.TryGetValue("SUPABASE_ANON_KEY", out var v) && !string.IsNullOrWhiteSpace(v))
                    return v;
                throw new InvalidOperationException("SUPABASE_ANON_KEY not configured. Set environment variable or provide Resources/Raw/supabase.json.");
            }
            return anonKey;
        }
    }
}