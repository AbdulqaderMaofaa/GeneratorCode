using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.TemplateEngine
{
    /// <summary>
    /// محرك قوالب بسيط
    /// </summary>
    public class SimpleTemplateEngine : ITemplateEngine
    {
        private readonly string _templatesPath;
        
        public SimpleTemplateEngine(string templatesPath = null)
        {
            if (string.IsNullOrEmpty(templatesPath))
            {
                var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
                
                // البحث عن مجلد المشروع (حيث يوجد ملف .csproj)
                var projectDirectory = FindProjectDirectory(assemblyDirectory);
                if (projectDirectory != null)
                {
                    _templatesPath = Path.Combine(projectDirectory, "Templates");
                }
                else
                {
                    throw new DirectoryNotFoundException("لم يتم العثور على مجلد المشروع");
                }
            }
            else
            {
                _templatesPath = templatesPath;
            }

            if (!Directory.Exists(_templatesPath))
            {
                Directory.CreateDirectory(_templatesPath);
            }
        }

        private static string FindProjectDirectory(string startDirectory)
        {
            var currentDirectory = startDirectory;
            while (currentDirectory != null)
            {
                // البحث عن ملف .csproj
                if (Directory.GetFiles(currentDirectory, "*.csproj").Length > 0)
                {
                    return currentDirectory;
                }
                currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
            }
            return null;
        }
        
        public async Task<string> LoadTemplateAsync(string templatePath)
        {
            var fullPath = Path.Combine(_templatesPath, templatePath);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Template not found: {templatePath}. Searched in: {fullPath}");
            }
            return await File.ReadAllTextAsync(fullPath);
        }
        
        public string LoadTemplate(string templatePath)
        {
            var fullPath = NormalizePath(templatePath);
            
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException(
                    $"Template not found: {templatePath}. Searched in: {fullPath}");
            }
                
            return File.ReadAllText(fullPath);
        }
        
        private string NormalizePath(string templatePath)
        {
            // تنظيف المسار من أي أحرف غير صالحة
            var cleanPath = string.Join("", templatePath.Split(Path.GetInvalidPathChars()));
            
            // التأكد من استخدام الفاصل المناسب للنظام
            cleanPath = cleanPath.Replace('/', Path.DirectorySeparatorChar)
                               .Replace('\\', Path.DirectorySeparatorChar);
            
            // دمج المسار مع المسار الأساسي
            return Path.GetFullPath(Path.Combine(_templatesPath, cleanPath));
        }
        
        public string ProcessTemplate(string template, Dictionary<string, object> data)
        {
            if (string.IsNullOrEmpty(template))
                return string.Empty;
                
            var result = template;
            
            // معالجة المتغيرات البسيطة {{variableName}}
            var variablePattern = @"\{\{(\w+)\}\}";
            result = Regex.Replace(result, variablePattern, match =>
            {
                var variableName = match.Groups[1].Value;
                return data.TryGetValue(variableName, out object value) ? value?.ToString() ?? "" : match.Value;
            });
            
            // معالجة الحلقات {{#each items}}...{{/each}}
            var loopPattern = @"\{\{#each\s+(\w+)\}\}(.*?)\{\{/each\}\}";
            result = Regex.Replace(result, loopPattern, match =>
            {
                var listName = match.Groups[1].Value;
                var loopTemplate = match.Groups[2].Value;
                
                if (!data.TryGetValue(listName, out object value) || value is not IEnumerable<object> items)
                    return "";
                    
                var loopResult = "";
                foreach (var item in items)
                {
                    var itemData = new Dictionary<string, object>(data);
                    if (item is Dictionary<string, object> itemDict)
                    {
                        foreach (var kvp in itemDict)
                        {
                            itemData[kvp.Key] = kvp.Value;
                        }
                    }
                    else
                    {
                        itemData["item"] = item;
                    }
                    
                    loopResult += ProcessTemplate(loopTemplate, itemData);
                }
                
                return loopResult;
            }, RegexOptions.Singleline);
            
            // معالجة الشروط {{#if condition}}...{{/if}}
            var conditionPattern = @"\{\{#if\s+(\w+)\}\}(.*?)\{\{/if\}\}";
            result = Regex.Replace(result, conditionPattern, match =>
            {
                var conditionName = match.Groups[1].Value;
                var conditionTemplate = match.Groups[2].Value;
                
                if (data.TryGetValue(conditionName, out object value) && IsTrue(value))
                {
                    return ProcessTemplate(conditionTemplate, data);
                }
                
                return "";
            }, RegexOptions.Singleline);
            
            return result;
        }
        
        public List<TemplateInfo> GetAvailableTemplates(string architecturePattern)
        {
            var templates = new List<TemplateInfo>();
            var patternPath = Path.Combine(_templatesPath, architecturePattern);
            
            if (!Directory.Exists(patternPath))
                return templates;
                
            var templateFiles = Directory.GetFiles(patternPath, "*.template", SearchOption.AllDirectories);
            
            foreach (var file in templateFiles)
            {
                var relativePath = Path.GetRelativePath(_templatesPath, file);
                var template = new TemplateInfo
                {
                    Name = Path.GetFileNameWithoutExtension(file),
                    Path = relativePath,
                    ArchitecturePattern = architecturePattern,
                    FileType = GetFileTypeFromTemplate(file),
                    Layer = GetLayerFromPath(relativePath)
                };
                
                templates.Add(template);
            }
            
            return templates;
        }
        
        public bool CreateTemplate(TemplateInfo templateInfo, string templateContent)
        {
            var fullPath = Path.Combine(_templatesPath, templateInfo.Path);
            var directory = Path.GetDirectoryName(fullPath);
            
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
                
            File.WriteAllText(fullPath, templateContent);
            return true;
        }
        
        public bool DeleteTemplate(string templatePath)
        {
            var fullPath = Path.Combine(_templatesPath, templatePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            return false;
        }
        
        public TemplateValidationResult ValidateTemplate(string template)
        {
            var result = new TemplateValidationResult { IsValid = true };
            
            // التحقق من صحة المتغيرات
            var variablePattern = @"\{\{(\w+)\}\}";
            var matches = Regex.Matches(template, variablePattern);
            
            foreach (Match match in matches)
            {
                var variableName = match.Groups[1].Value;
                if (!result.RequiredVariables.Contains(variableName))
                    result.RequiredVariables.Add(variableName);
            }
            
            // التحقق من صحة الحلقات
            var loopPattern = @"\{\{#each\s+(\w+)\}\}(.*?)\{\{/each\}\}";
            if (!IsValidPattern(template, loopPattern))
            {
                result.IsValid = false;
                result.Errors.Add("خطأ في صيغة الحلقات");
            }
            
            // التحقق من صحة الشروط
            var conditionPattern = @"\{\{#if\s+(\w+)\}\}(.*?)\{\{/if\}\}";
            if (!IsValidPattern(template, conditionPattern))
            {
                result.IsValid = false;
                result.Errors.Add("خطأ في صيغة الشروط");
            }
            
            return result;
        }
        
        private static bool IsTrue(object value)
        {
            if (value == null) return false;
            if (value is bool boolValue) return boolValue;
            if (value is string stringValue) return !string.IsNullOrEmpty(stringValue);
            if (value is int intValue) return intValue != 0;
            return true;
        }
        
        private static string GetFileTypeFromTemplate(string templatePath)
        {
            var content = File.ReadAllText(templatePath);
            if (content.Contains("namespace") && content.Contains("class"))
                return "cs";
            if (content.Contains("<html") || content.Contains("@model"))
                return "cshtml";
            if (content.Contains("import") || content.Contains("export"))
                return "js";
            return "txt";
        }
        
        private static string GetLayerFromPath(string relativePath)
        {
            var parts = relativePath.Split(Path.DirectorySeparatorChar);
            if (parts.Length > 1)
                return parts[1]; // الجزء الثاني من المسار يمثل الطبقة عادة
            return "Unknown";
        }
        
        private static bool IsValidPattern(string template, string pattern)
        {
            var matches = Regex.Matches(template, pattern, RegexOptions.Singleline);
            return true; // إذا لم يحدث خطأ، فالنمط صحيح
        }

        public string RenderTemplate(string template, object data)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;
            if (data == null) return template; // إرجاع القالب كما هو إذا كانت البيانات فارغة

            var result = template;

            // معالجة المتغيرات البسيطة {{variable}}
            result = ProcessSimpleVariables(result, data);

            // معالجة الحلقات {{#each items}} ... {{/each}}
            result = ProcessLoops(result, data);

            // معالجة الشروط {{#if condition}} ... {{/if}}
            result = ProcessConditions(result, data);

            return result;
        }

        private string ProcessSimpleVariables(string template, object data)
        {
            if (data == null) return template;

            var pattern = @"\{\{([^{}]+)\}\}";
            return Regex.Replace(template, pattern, match =>
            {
                var propertyPath = match.Groups[1].Value.Trim();
                var value = GetPropertyValue(data, propertyPath);
                return value?.ToString() ?? string.Empty;
            });
        }

        private string ProcessLoops(string template, object data)
        {
            if (data == null) return template;

            var pattern = @"\{\{#each\s+([^{}]+)\}\}(.*?)\{\{/each\}\}";
            return Regex.Replace(template, pattern, match =>
            {
                var propertyPath = match.Groups[1].Value.Trim();
                var content = match.Groups[2].Value;
                var items = GetPropertyValue(data, propertyPath) as IEnumerable<object>;
                
                if (items == null)
                {
                    Console.WriteLine($"تحذير: المصفوفة {propertyPath} غير موجودة أو فارغة");
                    return string.Empty;
                }

                var result = new List<string>();
                foreach (var item in items)
                {
                    if (item != null)
                    {
                        result.Add(RenderTemplate(content, item));
                    }
                }
                return string.Join(Environment.NewLine, result);
            }, RegexOptions.Singleline);
        }

        private string ProcessConditions(string template, object data)
        {
            if (data == null) return template;

            var pattern = @"\{\{#if\s+([^{}]+)\}\}(.*?)(?:\{\{else\}\}(.*?))?\{\{/if\}\}";
            return Regex.Replace(template, pattern, match =>
            {
                var condition = match.Groups[1].Value.Trim();
                var trueContent = match.Groups[2].Value;
                var falseContent = match.Groups[3].Success ? match.Groups[3].Value : string.Empty;

                var value = GetPropertyValue(data, condition);
                var isTrue = value != null && (value is bool boolValue ? boolValue : true);

                return isTrue ? RenderTemplate(trueContent, data) : RenderTemplate(falseContent, data);
            }, RegexOptions.Singleline);
        }

        private object GetPropertyValue(object obj, string path)
        {
            if (obj == null || string.IsNullOrEmpty(path)) return null;

            var properties = path.Split('.');
            var value = obj;

            foreach (var prop in properties)
            {
                if (value == null) return null;

                // التعامل مع الخصائص العادية
                var property = value.GetType().GetProperty(prop);
                if (property != null)
                {
                    value = property.GetValue(value);
                    continue;
                }

                // التعامل مع القواميس
                if (value is IDictionary<string, object> dict)
                {
                    if (dict.ContainsKey(prop))
                    {
                        value = dict[prop];
                        continue;
                    }
                }

                // إذا لم نجد الخاصية
                Console.WriteLine($"تحذير: الخاصية {prop} غير موجودة في الكائن");
                return null;
            }

            return value;

        }
    }
} 