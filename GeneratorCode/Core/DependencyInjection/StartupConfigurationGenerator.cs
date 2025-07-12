using System.Text;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.DependencyInjection
{
    /// <summary>
    /// مولد Startup Configuration
    /// </summary>
    public class StartupConfigurationGenerator
    {
        public string Generate(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            
            // Using statements
            sb.AppendLine("using Microsoft.AspNetCore.Builder;");
            sb.AppendLine("using Microsoft.AspNetCore.Hosting;");
            sb.AppendLine("using Microsoft.Extensions.Configuration;");
            sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
            sb.AppendLine("using Microsoft.Extensions.Hosting;");
            sb.AppendLine($"using {context.Namespace}.Extensions;");
            sb.AppendLine();
            
            // Startup class
            sb.AppendLine($"namespace {context.Namespace}");
            sb.AppendLine("{");
            sb.AppendLine("    public class Startup");
            sb.AppendLine("    {");
            
            // Constructor
            sb.AppendLine("        public Startup(IConfiguration configuration)");
            sb.AppendLine("        {");
            sb.AppendLine("            Configuration = configuration;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public IConfiguration Configuration { get; }");
            sb.AppendLine();
            
            // ConfigureServices method
            GenerateConfigureServices(sb, context);
            
            // Configure method
            GenerateConfigure(sb, context);
            
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
        
        private void GenerateConfigureServices(StringBuilder sb, CodeGenerationContext context)
        {
            sb.AppendLine("        public void ConfigureServices(IServiceCollection services)");
            sb.AppendLine("        {");
            sb.AppendLine("            services.AddControllers();");
            sb.AppendLine();
            
            // Swagger
            sb.AppendLine("            // Swagger/OpenAPI");
            sb.AppendLine("            services.AddEndpointsApiExplorer();");
            sb.AppendLine("            services.AddSwaggerGen();");
            sb.AppendLine();
            
            // CORS
            sb.AppendLine("            // CORS");
            sb.AppendLine("            services.AddCors(options =>");
            sb.AppendLine("            {");
            sb.AppendLine("                options.AddDefaultPolicy(builder =>");
            sb.AppendLine("                {");
            sb.AppendLine("                    builder.AllowAnyOrigin()");
            sb.AppendLine("                           .AllowAnyMethod()");
            sb.AppendLine("                           .AllowAnyHeader();");
            sb.AppendLine("                });");
            sb.AppendLine("            });");
            sb.AppendLine();
            
            // Application services
            sb.AppendLine($"            // {context.EntityName} Services");
            sb.AppendLine($"            services.Add{context.EntityName}Services(Configuration);");
            sb.AppendLine("        }");
            sb.AppendLine();
        }
        
        private void GenerateConfigure(StringBuilder sb, CodeGenerationContext context)
        {
            sb.AppendLine("        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (env.IsDevelopment())");
            sb.AppendLine("            {");
            sb.AppendLine("                app.UseDeveloperExceptionPage();");
            sb.AppendLine("                app.UseSwagger();");
            sb.AppendLine("                app.UseSwaggerUI();");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            app.UseHttpsRedirection();");
            sb.AppendLine("            app.UseRouting();");
            sb.AppendLine("            app.UseCors();");
            sb.AppendLine("            app.UseAuthorization();");
            sb.AppendLine();
            sb.AppendLine("            app.UseEndpoints(endpoints =>");
            sb.AppendLine("            {");
            sb.AppendLine("                endpoints.MapControllers();");
            sb.AppendLine("                endpoints.MapHealthChecks(\"/health\");");
            sb.AppendLine("            });");
            sb.AppendLine("        }");
        }
    }
} 