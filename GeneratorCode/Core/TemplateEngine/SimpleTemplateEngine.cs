using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;
using GeneratorCode.Core.Logging;

namespace GeneratorCode.Core.TemplateEngine
{
    /// <summary>
    /// محرك قوالب بسيط
    /// </summary>
    public class SimpleTemplateEngine : ITemplateEngine
    {
        private readonly string _templatesPath;
        private readonly ILogger _logger;
        
        public SimpleTemplateEngine(ILogger logger,string templatesPath = null )
        {
            // تعيين _logger أولاً قبل استخدامه
            _logger = logger ?? LoggerFactory.Default;
            
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
                    // استخدام مسار بديل: مجلد التطبيق الحالي
                    var appDataPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "GeneratorCode",
                        "Templates"
                    );
                    _templatesPath = appDataPath;
                }
            }
            else
            {
                _templatesPath = templatesPath;
            }

            // إنشاء المجلد إذا لم يكن موجوداً
            try
            {
                if (!Directory.Exists(_templatesPath))
                {
                    Directory.CreateDirectory(_templatesPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create templates directory at {_templatesPath}", ex, "SimpleTemplateEngine");
                throw new InvalidOperationException(
                    $"فشل في إنشاء مجلد القوالب في المسار: {_templatesPath}. الخطأ: {ex.Message}", ex);
            }
        }

        private static string FindProjectDirectory(string startDirectory)
        {
            if (string.IsNullOrEmpty(startDirectory) || !Directory.Exists(startDirectory))
                return null;
                
            var currentDirectory = startDirectory;
            var maxDepth = 10; // حد أقصى للبحث لتجنب الحلقات اللانهائية
            var depth = 0;
            
            while (currentDirectory != null && depth < maxDepth)
            {
                try
                {
                    // البحث عن ملف .csproj
                    var csprojFiles = Directory.GetFiles(currentDirectory, "*.csproj");
                    if (csprojFiles.Length > 0)
                    {
                        return currentDirectory;
                    }
                    
                    var parent = Directory.GetParent(currentDirectory);
                    currentDirectory = parent?.FullName;
                    depth++;
                }
                catch (Exception)
                {
                    // في حالة حدوث خطأ، التوقف عن البحث
                    break;
                }
            }
            return null;
        }
        
        public async Task<string> LoadTemplateAsync(string templatePath)
        {
            if (string.IsNullOrEmpty(templatePath))
                throw new ArgumentException("Template path cannot be null or empty", nameof(templatePath));
                
            try
            {
                var fullPath = NormalizePath(templatePath);
                if (!File.Exists(fullPath))
                {
                    _logger.LogWarning($"Template not found: {templatePath}. Searched in: {fullPath}", null, "SimpleTemplateEngine.LoadTemplateAsync");
                    throw new FileNotFoundException(
                        $"Template not found: {templatePath}. Searched in: {fullPath}");
                }
                _logger.LogDebug($"Loading template: {templatePath}", "SimpleTemplateEngine.LoadTemplateAsync");
                return await File.ReadAllTextAsync(fullPath);
            }
            catch (Exception ex) when (ex is not FileNotFoundException)
            {
                _logger.LogError($"Error loading template '{templatePath}'", ex, "SimpleTemplateEngine.LoadTemplateAsync");
                throw new InvalidOperationException(
                    $"Error loading template '{templatePath}': {ex.Message}", ex);
            }
        }
        
        public string LoadTemplate(string templatePath)
        {
            if (string.IsNullOrEmpty(templatePath))
                throw new ArgumentException("Template path cannot be null or empty", nameof(templatePath));
                
            try
            {
                var fullPath = NormalizePath(templatePath);
                
                if (!File.Exists(fullPath))
                {
                    _logger.LogWarning($"Template not found: {templatePath}. Searched in: {fullPath}", null, "SimpleTemplateEngine.LoadTemplate");
                    throw new FileNotFoundException(
                        $"Template not found: {templatePath}. Searched in: {fullPath}");
                }
                
                _logger.LogDebug($"Loading template: {templatePath}", "SimpleTemplateEngine.LoadTemplate");
                return File.ReadAllText(fullPath);
            }
            catch (Exception ex) when (ex is not FileNotFoundException)
            {
                _logger.LogError($"Error loading template '{templatePath}'", ex, "SimpleTemplateEngine.LoadTemplate");
                throw new InvalidOperationException(
                    $"Error loading template '{templatePath}': {ex.Message}", ex);
            }
        }
        
        private string NormalizePath(string templatePath)
        {
            if (string.IsNullOrEmpty(templatePath))
                throw new ArgumentException("Template path cannot be null or empty", nameof(templatePath));
                
            try
            {
                // تنظيف المسار من أي أحرف غير صالحة
                var invalidChars = Path.GetInvalidPathChars();
                var cleanPath = string.Join("", templatePath.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
                
                if (string.IsNullOrEmpty(cleanPath))
                    throw new ArgumentException("Template path contains only invalid characters", nameof(templatePath));
                
                // التأكد من استخدام الفاصل المناسب للنظام
                cleanPath = cleanPath.Replace('/', Path.DirectorySeparatorChar)
                                   .Replace('\\', Path.DirectorySeparatorChar);
                
                // دمج المسار مع المسار الأساسي
                var fullPath = Path.Combine(_templatesPath, cleanPath);
                
                // التأكد من أن المسار النهائي داخل مجلد القوالب (أمان)
                var normalizedFullPath = Path.GetFullPath(fullPath);
                var normalizedTemplatesPath = Path.GetFullPath(_templatesPath);
                
                if (!normalizedFullPath.StartsWith(normalizedTemplatesPath, StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnauthorizedAccessException(
                        $"Template path '{templatePath}' is outside the templates directory");
                }
                
                return normalizedFullPath;
            }
            catch (Exception ex) when (!(ex is ArgumentException || ex is UnauthorizedAccessException))
            {
                throw new InvalidOperationException(
                    $"Error normalizing template path '{templatePath}': {ex.Message}", ex);
            }
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

            // التحقق من توازن الأقواس: عدد {{ يساوي عدد }}
            var openCount = Regex.Matches(template, @"\{\{").Count;
            var closeCount = Regex.Matches(template, @"\}\}").Count;
            if (openCount != closeCount)
            {
                result.IsValid = false;
                result.Errors.Add("عدد علامات فتح القوالب {{ لا يتطابق مع عدد إغلاقها }}");
            }
            
            // التحقق من صحة المتغيرات
            var variablePattern = @"\{\{(\w+)\}\}";
            var matches = Regex.Matches(template, variablePattern);
            
            foreach (Match match in matches)
            {
                var variableName = match.Groups[1].Value;
                if (!result.RequiredVariables.Contains(variableName))
                    result.RequiredVariables.Add(variableName);
            }
            
            // التحقق من صحة الحلقات: عدد {{#each يساوي عدد {{/each}}
            var loopPattern = @"\{\{#each\s+(\w+)\}\}(.*?)\{\{/each\}\}";
            if (!IsValidPattern(template, loopPattern))
            {
                result.IsValid = false;
                result.Errors.Add("خطأ في صيغة الحلقات");
            }
            
            // التحقق من صحة الشروط: عدد {{#if يساوي عدد {{/if}}
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
            if (pattern.Contains("#each"))
            {
                var eachOpen = Regex.Matches(template, @"\{\{#each").Count;
                var eachClose = Regex.Matches(template, @"\{\{/each\}\}").Count;
                return eachOpen == eachClose;
            }
            if (pattern.Contains("#if"))
            {
                var ifOpen = Regex.Matches(template, @"\{\{#if").Count;
                var ifClose = Regex.Matches(template, @"\{\{/if\}\}").Count;
                return ifOpen == ifClose;
            }
            return true;
        }

        public string RenderTemplate(string template, object data)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;
            if (data == null) return template; // إرجاع القالب كما هو إذا كانت البيانات فارغة

            var result = template;

            // معالجة Partials {{> partialName}} قبل المتغيرات
            result = ProcessPartials(result, data);

            // معالجة المتغيرات البسيطة {{variable}}
            result = ProcessSimpleVariables(result, data);

            // معالجة المساعدين المدمجين {{uppercase x}}, {{lowercase x}}, إلخ
            result = ProcessHelpers(result, data);

            // معالجة الحلقات {{#each items}} ... {{/each}}
            result = ProcessLoops(result, data);

            // معالجة الشروط {{#if condition}} ... {{/if}}
            result = ProcessConditions(result, data);

            return result;
        }

        private static readonly string[] HelperNames = { "uppercase ", "lowercase ", "pluralize ", "camelCase ", "pascalCase " };

        private string ProcessSimpleVariables(string template, object data)
        {
            if (data == null) return template;

            var pattern = @"\{\{([^{}]+)\}\}";
            return Regex.Replace(template, pattern, match =>
            {
                var propertyPath = match.Groups[1].Value.Trim();
                // ترك استدعاءات المساعدين لـ ProcessHelpers
                var isHelper = false;
                foreach (var h in HelperNames)
                {
                    if (propertyPath.StartsWith(h, StringComparison.OrdinalIgnoreCase)) { isHelper = true; break; }
                }
                if (isHelper) return match.Value;
                var value = GetPropertyValue(data, propertyPath);
                return value?.ToString() ?? string.Empty;
            });
        }

        /// <summary>
        /// معالجة المساعدين المدمجين: uppercase, lowercase, pluralize, camelCase, pascalCase
        /// </summary>
        private string ProcessHelpers(string template, object data)
        {
            if (data == null) return template;

            var helperPattern = @"\{\{(uppercase|lowercase|pluralize|camelCase|pascalCase)\s+([^{}]+)\}\}";
            return Regex.Replace(template, helperPattern, match =>
            {
                var helperName = match.Groups[1].Value.Trim();
                var propertyPath = match.Groups[2].Value.Trim();
                var value = GetPropertyValue(data, propertyPath);
                var str = value?.ToString() ?? string.Empty;

                return helperName.ToLowerInvariant() switch
                {
                    "uppercase" => str.ToUpperInvariant(),
                    "lowercase" => str.ToLowerInvariant(),
                    "pluralize" => str + (str.EndsWith("s", StringComparison.OrdinalIgnoreCase) ? "" : "s"),
                    "camelcase" => ToCamelCase(str),
                    "pascalcase" => ToPascalCase(str),
                    _ => match.Value
                };
            }, RegexOptions.IgnoreCase);
        }

        private static string ToCamelCase(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            var words = SplitIntoWords(value);
            if (words.Count == 0) return value;
            var result = words[0].ToLowerInvariant();
            for (var i = 1; i < words.Count; i++) result += ToPascalCaseWord(words[i]);
            return result;
        }

        private static string ToPascalCase(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            var result = "";
            foreach (var word in SplitIntoWords(value)) result += ToPascalCaseWord(word);
            return result;
        }

        private static List<string> SplitIntoWords(string value)
        {
            var words = new List<string>();
            foreach (var part in value.Split(new[] { ' ', '_', '-', '.' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (string.IsNullOrEmpty(part)) continue;
                var sb = new System.Text.StringBuilder();
                foreach (var c in part)
                {
                    if (char.IsLetterOrDigit(c)) sb.Append(c);
                }
                if (sb.Length > 0) words.Add(sb.ToString());
            }
            return words;
        }

        private static string ToPascalCaseWord(string word)
        {
            if (string.IsNullOrEmpty(word)) return word;
            return char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant();
        }

        private string ProcessLoops(string template, object data)
        {
            if (data == null) return template;

            var pattern = @"\{\{#each\s+([^{}]+)\}\}(.*?)\{\{/each\}\}";
            return Regex.Replace(template, pattern, match =>
            {
                var propertyPath = match.Groups[1].Value.Trim();
                var content = match.Groups[2].Value;

                if (GetPropertyValue(data, propertyPath) is not IEnumerable<object> items)
                {
                    _logger.LogWarning($"تحذير: المصفوفة {propertyPath} غير موجودة أو فارغة", null, "SimpleTemplateEngine.ProcessLoops");
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
                var isTrue = value != null && (value is not bool boolValue || boolValue);

                return isTrue ? RenderTemplate(trueContent, data) : RenderTemplate(falseContent, data);
            }, RegexOptions.Singleline);
        }

        /// <summary>
        /// معالجة Partials: {{> partialName}} — تحميل وعرض قالب فرعي من مجلد القوالب
        /// </summary>
        private string ProcessPartials(string template, object data)
        {
            if (string.IsNullOrEmpty(template) || data == null) return template;

            var partialPattern = @"\{\{>\s*([^{}\s]+)\s*\}\}";
            return Regex.Replace(template, partialPattern, match =>
            {
                var partialName = match.Groups[1].Value.Trim();
                try
                {
                    var partialPath = partialName.Contains(".template") ? partialName : partialName + ".template";
                    var partialContent = LoadTemplate(partialPath);
                    return RenderTemplate(partialContent, data);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Partial '{partialName}' could not be loaded or rendered: {ex.Message}", ex, "SimpleTemplateEngine.ProcessPartials");
                    return match.Value;
                }
            });
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
                    if (dict.TryGetValue(prop, out object val))
                    {
                        value = val;
                        continue;
                    }
                }

                // إذا لم نجد الخاصية
                _logger.LogWarning($"تحذير: الخاصية {prop} غير موجودة في الكائن", null, "SimpleTemplateEngine.GetPropertyValue");
                return null;
            }

            return value;

        }
    }
} 