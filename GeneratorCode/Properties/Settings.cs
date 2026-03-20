using System;
using System.IO;
using System.Text.Json;

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
        public string PostgreSqlDefaultPassword { get; set; } = "";
        public string PostgreSqlDefaultPort { get; set; } = "5432";
        
        // إعدادات SQL Server
        public string SqlServerDefaultUsername { get; set; } = "sa";
        public string SqlServerDefaultPassword { get; set; } = "";
        
        // إعدادات MySQL
        public string MySqlDefaultUsername { get; set; } = "";
        public string MySqlDefaultPassword { get; set; } = "";

        // إعدادات LogViewer
        public string LogViewerDefaultViewMode { get; set; } = "Formatted"; // Formatted or Table
        public string LogViewerFormattedTemplate { get; set; } = "";

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
            var resourcesDir = Path.GetDirectoryName(SettingsPath);
            if (!Directory.Exists(resourcesDir))
            {
                Directory.CreateDirectory(resourcesDir);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(SettingsPath, json);
        }

        public void ExportToFile(string filePath)
        {
            var exportModel = new SettingsExportModel
            {
                DatabaseType = DatabaseType,
                EnableDI = EnableDI,
                EnableValidation = EnableValidation,
                EnableTesting = EnableTesting,
                DefaultNamespace = DefaultNamespace,
                DefaultOutputPath = DefaultOutputPath,
                PostgreSqlDefaultUsername = PostgreSqlDefaultUsername,
                PostgreSqlDefaultPort = PostgreSqlDefaultPort,
                SqlServerDefaultUsername = SqlServerDefaultUsername,
                MySqlDefaultUsername = MySqlDefaultUsername,
                LogViewerDefaultViewMode = LogViewerDefaultViewMode,
                LogViewerFormattedTemplate = LogViewerFormattedTemplate,
                ExportDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                AppVersion = System.Reflection.Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "1.0.0"
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(exportModel, options);
            File.WriteAllText(filePath, json);
        }

        public static Settings ImportFromFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var importModel = JsonSerializer.Deserialize<SettingsExportModel>(json);
            if (importModel == null)
                throw new InvalidOperationException("Invalid settings file format.");

            var current = Default;
            current.DatabaseType = importModel.DatabaseType ?? current.DatabaseType;
            current.EnableDI = importModel.EnableDI;
            current.EnableValidation = importModel.EnableValidation;
            current.EnableTesting = importModel.EnableTesting;
            current.DefaultNamespace = importModel.DefaultNamespace ?? current.DefaultNamespace;
            current.DefaultOutputPath = importModel.DefaultOutputPath ?? current.DefaultOutputPath;
            current.PostgreSqlDefaultUsername = importModel.PostgreSqlDefaultUsername ?? current.PostgreSqlDefaultUsername;
            current.PostgreSqlDefaultPort = importModel.PostgreSqlDefaultPort ?? current.PostgreSqlDefaultPort;
            current.SqlServerDefaultUsername = importModel.SqlServerDefaultUsername ?? current.SqlServerDefaultUsername;
            current.MySqlDefaultUsername = importModel.MySqlDefaultUsername ?? current.MySqlDefaultUsername;
            current.LogViewerDefaultViewMode = importModel.LogViewerDefaultViewMode ?? current.LogViewerDefaultViewMode;
            current.LogViewerFormattedTemplate = importModel.LogViewerFormattedTemplate ?? current.LogViewerFormattedTemplate;

            current.Save();
            defaultInstance = current;
            return current;
        }
    }

    public class SettingsExportModel
    {
        public string DatabaseType { get; set; }
        public bool EnableDI { get; set; }
        public bool EnableValidation { get; set; }
        public bool EnableTesting { get; set; }
        public string DefaultNamespace { get; set; }
        public string DefaultOutputPath { get; set; }
        public string PostgreSqlDefaultUsername { get; set; }
        public string PostgreSqlDefaultPort { get; set; }
        public string SqlServerDefaultUsername { get; set; }
        public string MySqlDefaultUsername { get; set; }
        public string LogViewerDefaultViewMode { get; set; }
        public string LogViewerFormattedTemplate { get; set; }
        public string ExportDate { get; set; }
        public string AppVersion { get; set; }
    }
}