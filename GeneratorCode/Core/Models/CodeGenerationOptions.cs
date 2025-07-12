namespace GeneratorCode.Core.Models
{
    public class CodeGenerationOptions
    {
        public bool GenerateEntities { get; set; }
        public bool GenerateDTOs { get; set; }
        public bool GenerateRepositories { get; set; }
        public bool GenerateServices { get; set; }
        public bool GenerateControllers { get; set; }
        public bool GenerateUnitTests { get; set; }
        public bool GenerateValidation { get; set; }
        public bool GenerateSwagger { get; set; }
        public bool GenerateDependencyInjection { get; set; }
        public bool GenerateCreate { get; set; }
        public bool GenerateRead { get; set; }
        public bool GenerateUpdate { get; set; }
        public bool GenerateDelete { get; set; }
    }
} 