namespace GeneratorCode.Core.Models
{
    /// <summary>
    /// خيارات التوليد
    /// </summary>
    public class GenerationOptions
    {
        /// <summary>
        /// توليد Controllers
        /// </summary>
        public bool GenerateControllers { get; set; } = true;
        
        /// <summary>
        /// توليد Services
        /// </summary>
        public bool GenerateServices { get; set; } = true;
        
        /// <summary>
        /// توليد Repositories
        /// </summary>
        public bool GenerateRepositories { get; set; } = true;
        
        /// <summary>
        /// توليد Models
        /// </summary>
        public bool GenerateModels { get; set; } = true;
        
        /// <summary>
        /// توليد DTOs
        /// </summary>
        public bool GenerateDTOs { get; set; } = true;
        
        /// <summary>
        /// توليد Validators
        /// </summary>
        public bool GenerateValidators { get; set; } = true;
        
        /// <summary>
        /// توليد Unit Tests
        /// </summary>
        public bool GenerateUnitTests { get; set; } = false;
        
        /// <summary>
        /// توليد API Documentation
        /// </summary>
        public bool GenerateApiDocs { get; set; } = false;
        
        /// <summary>
        /// دعم Dependency Injection
        /// </summary>
        public bool EnableDependencyInjection { get; set; } = true;
        
        /// <summary>
        /// دعم العمليات غير المتزامنة
        /// </summary>
        public bool EnableAsyncOperations { get; set; } = true;
        
        /// <summary>
        /// دعم التسجيل
        /// </summary>
        public bool EnableLogging { get; set; } = true;

        public bool GenerateEntities { get; set; }
        public bool GenerateValidation { get; set; }
        public bool GenerateSwagger { get; set; }
        public bool GenerateDependencyInjection { get; set; }
        public bool GenerateCreate { get; set; }
        public bool GenerateRead { get; set; }
        public bool GenerateUpdate { get; set; }
        public bool GenerateDelete { get; set; }

        // Project Structure
        public bool GenerateStartupClass { get; set; } = true;
        public bool GenerateProgram { get; set; } = true;
        public bool GenerateGitignore { get; set; } = true;
        public bool GenerateReadme { get; set; } = true;
        public bool GenerateSolutionFile { get; set; } = true;

        // Architecture Components
        public bool GenerateInfrastructureLayer { get; set; } = true;
        public bool GenerateApplicationLayer { get; set; } = true;
        public bool GenerateDomainLayer { get; set; } = true;
        public bool GeneratePresentationLayer { get; set; } = true;
    }
} 