using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GeneratorCode.Core.DependencyInjection
{
    public class PackageVersionResolver
    {
        private readonly Dictionary<string, Dictionary<string, string>> _versionMatrix;
        private static readonly string[] SupportedFrameworks = { "net6.0", "net7.0", "net8.0", "net9.0" };

        public PackageVersionResolver()
        {
            _versionMatrix = LoadVersionMatrix();
        }

        public string GetVersion(string targetFramework, string packageName)
        {
            targetFramework = NormalizeFramework(targetFramework);

            if (_versionMatrix.TryGetValue(targetFramework, out var versions) &&
                versions.TryGetValue(packageName, out var version))
            {
                return version;
            }

            var fallbackFw = GetNearestSupportedFramework(targetFramework);
            if (fallbackFw != null &&
                _versionMatrix.TryGetValue(fallbackFw, out var fallbackVersions) &&
                fallbackVersions.TryGetValue(packageName, out var fallbackVersion))
            {
                return fallbackVersion;
            }

            return "8.0.0";
        }

        public Dictionary<string, string> GetAllVersions(string targetFramework)
        {
            targetFramework = NormalizeFramework(targetFramework);

            if (_versionMatrix.TryGetValue(targetFramework, out var versions))
                return new Dictionary<string, string>(versions);

            var fallbackFw = GetNearestSupportedFramework(targetFramework);
            if (fallbackFw != null && _versionMatrix.TryGetValue(fallbackFw, out var fallbackVersions))
                return new Dictionary<string, string>(fallbackVersions);

            return new Dictionary<string, string>();
        }

        public List<string> GetSupportedFrameworks()
        {
            return _versionMatrix.Keys.ToList();
        }

        private static string NormalizeFramework(string framework)
        {
            if (string.IsNullOrWhiteSpace(framework)) return "net8.0";
            framework = framework.Trim().ToLowerInvariant();
            if (!framework.StartsWith("net")) framework = "net" + framework;
            if (!framework.Contains('.')) framework += ".0";
            return framework;
        }

        private static string GetNearestSupportedFramework(string targetFramework)
        {
            var numericPart = targetFramework.Replace("net", "").Replace(".0", "");
            if (!int.TryParse(numericPart, out var targetVersion)) return "net8.0";

            string nearest = null;
            int minDiff = int.MaxValue;

            foreach (var fw in SupportedFrameworks)
            {
                var fwNum = fw.Replace("net", "").Replace(".0", "");
                if (int.TryParse(fwNum, out var fwVersion))
                {
                    var diff = Math.Abs(fwVersion - targetVersion);
                    if (diff < minDiff || (diff == minDiff && fwVersion <= targetVersion))
                    {
                        minDiff = diff;
                        nearest = fw;
                    }
                }
            }

            return nearest;
        }

        private static Dictionary<string, Dictionary<string, string>> LoadVersionMatrix()
        {
            try
            {
                var configPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Resources", "package-versions.json");

                if (!File.Exists(configPath))
                    return GetDefaultMatrix();

                var json = File.ReadAllText(configPath);

                var multiVersion = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);
                if (multiVersion != null && multiVersion.ContainsKey("net8.0"))
                    return multiVersion;

                var flatVersion = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (flatVersion != null)
                    return new Dictionary<string, Dictionary<string, string>> { ["net8.0"] = flatVersion };
            }
            catch { }

            return GetDefaultMatrix();
        }

        private static Dictionary<string, Dictionary<string, string>> GetDefaultMatrix()
        {
            return new Dictionary<string, Dictionary<string, string>>
            {
                ["net8.0"] = new Dictionary<string, string>
                {
                    ["Microsoft.EntityFrameworkCore"] = "8.0.12",
                    ["Microsoft.EntityFrameworkCore.Tools"] = "8.0.12",
                    ["Microsoft.EntityFrameworkCore.SqlServer"] = "8.0.12",
                    ["Microsoft.EntityFrameworkCore.Design"] = "8.0.12",
                    ["Microsoft.EntityFrameworkCore.Sqlite"] = "8.0.12",
                    ["Npgsql.EntityFrameworkCore.PostgreSQL"] = "8.0.11",
                    ["Pomelo.EntityFrameworkCore.MySql"] = "8.0.2",
                    ["Oracle.EntityFrameworkCore"] = "8.23.60",
                    ["AutoMapper"] = "13.0.1",
                    ["FluentValidation"] = "11.11.0",
                    ["MediatR"] = "12.4.1",
                    ["Swashbuckle.AspNetCore"] = "6.9.0",
                    ["Microsoft.Extensions.DependencyInjection"] = "8.0.1",
                    ["Microsoft.Extensions.Configuration"] = "8.0.0",
                    ["Autofac"] = "8.2.0",
                    ["Autofac.Extensions.DependencyInjection"] = "10.0.0"
                }
            };
        }
    }
}
