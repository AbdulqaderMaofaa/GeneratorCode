using System;
using System.Collections.Generic;
using System.Text;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.DependencyInjection
{
    /// <summary>
    /// موفر Autofac Dependency Injection
    /// </summary>
    public class AutofacProvider : IDependencyInjectionProvider
    {
        public string Name => "Autofac";
        public DIContainerType ContainerType => DIContainerType.Autofac;
        
        public DIConfigurationResult GenerateConfiguration(CodeGenerationContext context)
        {
            var result = new DIConfigurationResult { Success = true };
            
            // توليد Autofac Module
                var autofacModule = GenerateAutofacModule(context);
                result.ConfigurationFiles.Add(new GeneratedFile
                {
                    FileName = $"{context.EntityName}Module.cs",
                    RelativePath = $"Modules/{context.EntityName}Module.cs",
                    FullPath = System.IO.Path.Combine(context.OutputPath, "Modules", $"{context.EntityName}Module.cs"),
                    Content = autofacModule,
                    FileType = "cs",
                    Layer = "Infrastructure"
                });
                
                // توليد Startup Configuration
                var startupConfig = GenerateStartupConfiguration(context);
                result.ConfigurationFiles.Add(new GeneratedFile
                {
                    FileName = "Startup.cs",
                    RelativePath = "Startup.cs",
                    FullPath = System.IO.Path.Combine(context.OutputPath, "Startup.cs"),
                    Content = startupConfig,
                    FileType = "cs",
                    Layer = "Presentation"
                });
                
                result.Message = "تم توليد تكوين Autofac بنجاح";
            
            return result;
        }
        
        public string GenerateServiceExtensions(CodeGenerationContext context)
        {
            // Autofac يستخدم Modules بدلاً من Service Extensions
            return "";
        }
        
        public string GenerateStartupConfiguration(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("using Autofac;");
            sb.AppendLine("using Autofac.Extensions.DependencyInjection;");
            sb.AppendLine("using Microsoft.AspNetCore.Builder;");
            sb.AppendLine("using Microsoft.AspNetCore.Hosting;");
            sb.AppendLine("using Microsoft.Extensions.Configuration;");
            sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
            sb.AppendLine("using Microsoft.Extensions.Hosting;");
            sb.AppendLine($"using {context.Namespace}.Modules;");
            sb.AppendLine();
            
            sb.AppendLine($"namespace {context.Namespace}");
            sb.AppendLine("{");
            sb.AppendLine("    public class Startup");
            sb.AppendLine("    {");
            sb.AppendLine("        public Startup(IConfiguration configuration)");
            sb.AppendLine("        {");
            sb.AppendLine("            Configuration = configuration;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public IConfiguration Configuration { get; }");
            sb.AppendLine();
            sb.AppendLine("        public void ConfigureServices(IServiceCollection services)");
            sb.AppendLine("        {");
            sb.AppendLine("            services.AddControllers();");
            sb.AppendLine("            services.AddSwaggerGen();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public void ConfigureContainer(ContainerBuilder builder)");
            sb.AppendLine("        {");
            sb.AppendLine($"            builder.RegisterModule<{context.EntityName}Module>();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (env.IsDevelopment())");
            sb.AppendLine("            {");
            sb.AppendLine("                app.UseDeveloperExceptionPage();");
            sb.AppendLine("                app.UseSwagger();");
            sb.AppendLine("                app.UseSwaggerUI();");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            app.UseRouting();");
            sb.AppendLine("            app.UseEndpoints(endpoints => endpoints.MapControllers());");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
        
        private string GenerateAutofacModule(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("using Autofac;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine($"using {context.Namespace}.Domain.Repositories;");
            sb.AppendLine($"using {context.Namespace}.Infrastructure.Repositories;");
            sb.AppendLine($"using {context.Namespace}.Application.Services;");
            sb.AppendLine();
            
            sb.AppendLine($"namespace {context.Namespace}.Modules");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {context.EntityName}Module : Module");
            sb.AppendLine("    {");
            sb.AppendLine("        protected override void Load(ContainerBuilder builder)");
            sb.AppendLine("        {");
            sb.AppendLine("            // Repository Registration");
            sb.AppendLine($"            builder.RegisterType<{context.EntityName}Repository>()");
            sb.AppendLine($"                   .As<I{context.EntityName}Repository>()");
            sb.AppendLine("                   .InstancePerLifetimeScope();");
            sb.AppendLine();
            sb.AppendLine("            // Service Registration");
            sb.AppendLine($"            builder.RegisterType<{context.EntityName}Service>()");
            sb.AppendLine($"                   .As<I{context.EntityName}Service>()");
            sb.AppendLine("                   .InstancePerLifetimeScope();");
            sb.AppendLine();
            sb.AppendLine("            // Generic Repository Registration");
            sb.AppendLine("            builder.RegisterGeneric(typeof(Repository<>))");
            sb.AppendLine("                   .As(typeof(IRepository<>))");
            sb.AppendLine("                   .InstancePerLifetimeScope();");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
        
        public List<string> GetRequiredPackages()
        {
            return new List<string>
            {
                "Autofac",
                "Autofac.Extensions.DependencyInjection",
                "Microsoft.EntityFrameworkCore"
            };
        }
        
        public List<string> GetRequiredUsings()
        {
            return new List<string>
            {
                "Autofac",
                "Autofac.Extensions.DependencyInjection"
            };
        }
    }
} 