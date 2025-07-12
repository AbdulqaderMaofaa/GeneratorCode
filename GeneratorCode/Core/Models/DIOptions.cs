namespace GeneratorCode.Core.Models
{
    public class DIOptions
    {
        public bool EnableDI { get; set; }
        public DIContainerType PreferredContainer { get; set; }
        public bool GenerateServiceExtensions { get; set; }
        public bool EnableLogging { get; set; }
        public bool EnableConfiguration { get; set; }
        public bool EnableHealthChecks { get; set; }
        public bool GenerateModuleClasses { get; set; }
        public bool EnableAutoRegistration { get; set; }
        public bool EnableValidation { get; set; }
    }
} 