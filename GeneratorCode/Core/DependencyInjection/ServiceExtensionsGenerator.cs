using System.Text;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.DependencyInjection
{
    /// <summary>
    /// مولد Service Extensions
    /// </summary>
    public class ServiceExtensionsGenerator
    {
        public string Generate(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            
            // إضافة Using statements
            sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
            sb.AppendLine("using Microsoft.Extensions.Configuration;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine($"using {context.Namespace}.Domain.Repositories;");
            sb.AppendLine($"using {context.Namespace}.Infrastructure.Repositories;");
            sb.AppendLine($"using {context.Namespace}.Application.Services;");
            if (context.ArchitecturePattern == "CleanArchitecture" || context.ArchitecturePattern == "CQRS")
            {
                sb.AppendLine("using MediatR;");
            }
            sb.AppendLine();
            
            // بداية الكلاس
            sb.AppendLine($"namespace {context.Namespace}.Extensions");
            sb.AppendLine("{");
            sb.AppendLine($"    public static class {context.EntityName}ServiceExtensions");
            sb.AppendLine("    {");
            sb.AppendLine($"        public static IServiceCollection Add{context.EntityName}Services(this IServiceCollection services, IConfiguration configuration)");
            sb.AppendLine("        {");
            
            // تسجيل DbContext
            GenerateDbContextRegistration(sb, context);
            
            // تسجيل Repositories
            GenerateRepositoryRegistration(sb, context);
            
            // تسجيل Services
            GenerateServiceRegistration(sb, context);
            
            // تسجيل MediatR
            if (context.ArchitecturePattern == "CleanArchitecture" || context.ArchitecturePattern == "CQRS")
            {
                GenerateMediatRRegistration(sb, context);
            }
            
            // تسجيل AutoMapper
            GenerateAutoMapperRegistration(sb, context);
            
            // تسجيل Health Checks
            GenerateHealthChecksRegistration(sb, context);
            
            sb.AppendLine("            return services;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
        
        private void GenerateDbContextRegistration(StringBuilder sb, CodeGenerationContext context)
        {
            sb.AppendLine("            // Database Configuration");
            sb.AppendLine("            services.AddDbContext<ApplicationDbContext>(options =>");
            
            switch (context.DatabaseType)
            {
                case DatabaseType.SqlServer:
                    sb.AppendLine("                options.UseSqlServer(configuration.GetConnectionString(\"DefaultConnection\")));");
                    break;
                case DatabaseType.MySql:
                    sb.AppendLine("                options.UseMySQL(configuration.GetConnectionString(\"DefaultConnection\")));");
                    break;
                case DatabaseType.PostgreSql:
                    sb.AppendLine("                options.UseNpgsql(configuration.GetConnectionString(\"DefaultConnection\")));");
                    break;
                default:
                    sb.AppendLine("                options.UseSqlServer(configuration.GetConnectionString(\"DefaultConnection\")));");
                    break;
            }
            sb.AppendLine();
        }
        
        private void GenerateRepositoryRegistration(StringBuilder sb, CodeGenerationContext context)
        {
            sb.AppendLine("            // Repository Registration");
            sb.AppendLine($"            services.AddScoped<I{context.EntityName}Repository, {context.EntityName}Repository>();");
            sb.AppendLine("            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));");
            sb.AppendLine();
        }
        
        private void GenerateServiceRegistration(StringBuilder sb, CodeGenerationContext context)
        {
            sb.AppendLine("            // Service Registration");
            sb.AppendLine($"            services.AddScoped<I{context.EntityName}Service, {context.EntityName}Service>();");
            sb.AppendLine();
        }
        
        private void GenerateMediatRRegistration(StringBuilder sb, CodeGenerationContext context)
        {
            sb.AppendLine("            // MediatR Registration");
            sb.AppendLine($"            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof({context.EntityName}ServiceExtensions).Assembly));");
            sb.AppendLine();
        }
        
        private void GenerateAutoMapperRegistration(StringBuilder sb, CodeGenerationContext context)
        {
            sb.AppendLine("            // AutoMapper Registration");
            sb.AppendLine($"            services.AddAutoMapper(typeof({context.EntityName}ServiceExtensions).Assembly);");
            sb.AppendLine();
        }
        
        private void GenerateHealthChecksRegistration(StringBuilder sb, CodeGenerationContext context)
        {
            sb.AppendLine("            // Health Checks");
            sb.AppendLine("            services.AddHealthChecks()")
                .AppendLine("                .AddDbContextCheck<ApplicationDbContext>();");
            sb.AppendLine();
        }
    }
} 