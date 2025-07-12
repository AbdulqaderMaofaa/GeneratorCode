using System;
using System.IO;
using System.Text.Json;
using System.Reflection;

namespace GeneratorCode.Properties
{
    public class Settings
    {
        private static Settings defaultInstance;
        private static readonly string SettingsPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Resources",
            "settings.json"
        );

        public static Settings Default
        {
            get
            {
                defaultInstance ??= Load();
                return defaultInstance;
            }
        }

        // نوع قاعدة البيانات الافتراضي
        public string DatabaseType { get; set; } = "PostgreSQL";

        public bool EnableDI { get; set; }
        public bool EnableValidation { get; set; }
        public bool EnableTesting { get; set; }
        public string DefaultNamespace { get; set; }
        public string DefaultOutputPath { get; set; }

        // إعدادات PostgreSQL
        public string PostgreSqlDefaultUsername { get; set; } = "postgres";
        public string PostgreSqlDefaultPassword { get; set; } = "4oh70*w8QT";
        public string PostgreSqlDefaultPort { get; set; } = "5432";
        
        // إعدادات SQL Server
        public string SqlServerDefaultUsername { get; set; } = "sa";
        public string SqlServerDefaultPassword { get; set; } = "CX_cxAdm0n";
        
        // إعدادات MySQL
        public string MySqlDefaultUsername { get; set; } = "";
        public string MySqlDefaultPassword { get; set; } = "";

        private static Settings Load()
        {
            // محاولة تحميل الإعدادات من ملف Resources
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                var settings = JsonSerializer.Deserialize<Settings>(json);
                if (settings != null)
                {
                    return settings;
                }
            }

            // إذا لم يوجد الملف أو حدث خطأ، نعيد نسخة جديدة مع القيم الافتراضية
            return new Settings();
        }

        public void Save()
        {
            // إنشاء مجلد Resources إذا لم يكن موجوداً
            var resourcesDir = Path.GetDirectoryName(SettingsPath);
            if (!Directory.Exists(resourcesDir))
            {
                Directory.CreateDirectory(resourcesDir);
            }

            // حفظ الإعدادات كملف JSON
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(SettingsPath, json);
        }
    }
}