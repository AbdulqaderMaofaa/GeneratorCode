using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.ArchitecturePatterns
{
    /// <summary>
    /// نمط معماري بسيط - يناسب التطبيقات الصغيرة
    /// </summary>
    public class SimpleArchitecturePattern : BaseArchitecturePattern
    {
        public override string Name => "Simple Architecture";
        
        public override string Description => "نمط معماري بسيط يتكون من Models و DAL و Business Logic";
        
        public override CodeGenerationResult Generate(CodeGenerationContext context)
        {
            var result = new CodeGenerationResult();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            try
            {
                // توليد Model
                var modelContent = GenerateModel(context);
                result.GeneratedFiles.Add(new GeneratedFile
                {
                    FileName = $"{context.EntityName}.cs",
                    RelativePath = $"Models/{context.EntityName}.cs",
                    FullPath = Path.Combine(context.OutputPath, "Models", $"{context.EntityName}.cs"),
                    Content = modelContent,
                    FileType = "cs",
                    Layer = "Models"
                });
                
                // توليد DAL
                var dalContent = GenerateDAL(context);
                result.GeneratedFiles.Add(new GeneratedFile
                {
                    FileName = $"{context.EntityName}DAL.cs",
                    RelativePath = $"DAL/{context.EntityName}DAL.cs",
                    FullPath = Path.Combine(context.OutputPath, "DAL", $"{context.EntityName}DAL.cs"),
                    Content = dalContent,
                    FileType = "cs",
                    Layer = "DAL"
                });
                
                // توليد Business Logic
                var businessContent = GenerateBusinessLogic(context);
                result.GeneratedFiles.Add(new GeneratedFile
                {
                    FileName = $"{context.EntityName}Business.cs",
                    RelativePath = $"Business/{context.EntityName}Business.cs",
                    FullPath = Path.Combine(context.OutputPath, "Business", $"{context.EntityName}Business.cs"),
                    Content = businessContent,
                    FileType = "cs",
                    Layer = "Business"
                });
                
                result.Success = true;
                result.Message = "تم توليد النمط المعماري البسيط بنجاح";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"خطأ في توليد الكود: {ex.Message}";
                result.Errors.Add(ex.Message);
            }
            finally
            {
                stopwatch.Stop();
                result.GenerationTime = stopwatch.Elapsed;
            }
            
            return result;
        }
        
        public override bool SupportsDatabaseType(DatabaseType databaseType)
        {
            return databaseType == DatabaseType.SqlServer || 
                   databaseType == DatabaseType.MySql || 
                   databaseType == DatabaseType.PostgreSql;
        }
        
        public override List<string> GetRequiredLayers()
        {
            return new List<string>
            {
                "Models",
                "DAL",
                "Business"
            };
        }
        
        public override List<string> GetRequiredDependencies()
        {
            return new List<string>
            {
                "System.Data.SqlClient",
                "System.ComponentModel.DataAnnotations"
            };
        }
        
        private string GenerateModel(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("using System;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations;");
            sb.AppendLine();
            sb.AppendLine($"namespace {context.Namespace}.Models");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {context.EntityName}");
            sb.AppendLine("    {");
            
            // إضافة الخصائص من معلومات الجدول
            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    if (column.IsPrimaryKey)
                    {
                        sb.AppendLine("        [Key]");
                    }
                    
                    if (!column.IsNullable && column.CSharpType == "string")
                    {
                        sb.AppendLine("        [Required]");
                    }
                    
                    if (column.MaxLength.HasValue && column.CSharpType == "string")
                    {
                        sb.AppendLine($"        [StringLength({column.MaxLength.Value})]");
                    }
                    
                    sb.AppendLine($"        public {column.CSharpType} {column.Name} {{ get; set; }}");
                    sb.AppendLine();
                }
            }
            
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
        
        private string GenerateDAL(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Data;");
            sb.AppendLine("using System.Data.SqlClient;");
            sb.AppendLine($"using {context.Namespace}.Models;");
            sb.AppendLine();
            sb.AppendLine($"namespace {context.Namespace}.DAL");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {context.EntityName}DAL");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly string _connectionString;");
            sb.AppendLine();
            sb.AppendLine($"        public {context.EntityName}DAL(string connectionString)");
            sb.AppendLine("        {");
            sb.AppendLine("            _connectionString = connectionString;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public List<{context.EntityName}> GetAll()");
            sb.AppendLine("        {");
            sb.AppendLine($"            var result = new List<{context.EntityName}>();");
            sb.AppendLine($"            var query = \"SELECT * FROM {context.TableName}\";");
            sb.AppendLine();
            sb.AppendLine("            using (var connection = new SqlConnection(_connectionString))");
            sb.AppendLine("            {");
            sb.AppendLine("                connection.Open();");
            sb.AppendLine("                using (var command = new SqlCommand(query, connection))");
            sb.AppendLine("                {");
            sb.AppendLine("                    using (var reader = command.ExecuteReader())");
            sb.AppendLine("                    {");
            sb.AppendLine("                        while (reader.Read())");
            sb.AppendLine("                        {");
            sb.AppendLine($"                            result.Add(Map{context.EntityName}(reader));");
            sb.AppendLine("                        }");
            sb.AppendLine("                    }");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            return result;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public {context.EntityName} GetById(int id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var query = \"SELECT * FROM {context.TableName} WHERE Id = @Id\";");
            sb.AppendLine();
            sb.AppendLine("            using (var connection = new SqlConnection(_connectionString))");
            sb.AppendLine("            {");
            sb.AppendLine("                connection.Open();");
            sb.AppendLine("                using (var command = new SqlCommand(query, connection))");
            sb.AppendLine("                {");
            sb.AppendLine("                    command.Parameters.AddWithValue(\"@Id\", id);");
            sb.AppendLine("                    using (var reader = command.ExecuteReader())");
            sb.AppendLine("                    {");
            sb.AppendLine("                        if (reader.Read())");
            sb.AppendLine("                        {");
            sb.AppendLine($"                            return Map{context.EntityName}(reader);");
            sb.AppendLine("                        }");
            sb.AppendLine("                    }");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            return null;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public int Insert({context.EntityName} entity)");
            sb.AppendLine("        {");
            sb.AppendLine("            // سيتم إكمال هذه الطريقة لاحقاً");
            sb.AppendLine("            return 0;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public bool Update({context.EntityName} entity)");
            sb.AppendLine("        {");
            sb.AppendLine("            // سيتم إكمال هذه الطريقة لاحقاً");
            sb.AppendLine("            return false;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public bool Delete(int id)");
            sb.AppendLine("        {");
            sb.AppendLine("            // سيتم إكمال هذه الطريقة لاحقاً");
            sb.AppendLine("            return false;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        private {context.EntityName} Map{context.EntityName}(IDataReader reader)");
            sb.AppendLine("        {");
            sb.AppendLine($"            return new {context.EntityName}");
            sb.AppendLine("            {");
            
            // إضافة تعيين الأعمدة
            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    sb.AppendLine($"                {column.Name} = reader[\"{column.Name}\"] as {column.CSharpType},");
                }
            }
            
            sb.AppendLine("            };");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
        
        private string GenerateBusinessLogic(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine($"using {context.Namespace}.Models;");
            sb.AppendLine($"using {context.Namespace}.DAL;");
            sb.AppendLine();
            sb.AppendLine($"namespace {context.Namespace}.Business");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {context.EntityName}Business");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly {context.EntityName}DAL _dal;");
            sb.AppendLine();
            sb.AppendLine($"        public {context.EntityName}Business(string connectionString)");
            sb.AppendLine("        {");
            sb.AppendLine($"            _dal = new {context.EntityName}DAL(connectionString);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public List<{context.EntityName}> GetAll()");
            sb.AppendLine("        {");
            sb.AppendLine("            try");
            sb.AppendLine("            {");
            sb.AppendLine("                return _dal.GetAll();");
            sb.AppendLine("            }");
            sb.AppendLine("            catch (Exception ex)");
            sb.AppendLine("            {");
            sb.AppendLine("                // تسجيل الخطأ");
            sb.AppendLine("                throw new Exception($\"خطأ في جلب البيانات: {ex.Message}\", ex);");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public {context.EntityName} GetById(int id)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (id <= 0)");
            sb.AppendLine("                throw new ArgumentException(\"معرف غير صالح\", nameof(id));");
            sb.AppendLine();
            sb.AppendLine("            try");
            sb.AppendLine("            {");
            sb.AppendLine("                return _dal.GetById(id);");
            sb.AppendLine("            }");
            sb.AppendLine("            catch (Exception ex)");
            sb.AppendLine("            {");
            sb.AppendLine("                throw new Exception($\"خطأ في جلب البيانات: {ex.Message}\", ex);");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public int Add({context.EntityName} entity)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (entity == null)");
            sb.AppendLine("                throw new ArgumentNullException(nameof(entity));");
            sb.AppendLine();
            sb.AppendLine("            // إضافة التحقق من صحة البيانات");
            sb.AppendLine("            ValidateEntity(entity);");
            sb.AppendLine();
            sb.AppendLine("            try");
            sb.AppendLine("            {");
            sb.AppendLine("                return _dal.Insert(entity);");
            sb.AppendLine("            }");
            sb.AppendLine("            catch (Exception ex)");
            sb.AppendLine("            {");
            sb.AppendLine("                throw new Exception($\"خطأ في إضافة البيانات: {ex.Message}\", ex);");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public bool Update({context.EntityName} entity)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (entity == null)");
            sb.AppendLine("                throw new ArgumentNullException(nameof(entity));");
            sb.AppendLine();
            sb.AppendLine("            ValidateEntity(entity);");
            sb.AppendLine();
            sb.AppendLine("            try");
            sb.AppendLine("            {");
            sb.AppendLine("                return _dal.Update(entity);");
            sb.AppendLine("            }");
            sb.AppendLine("            catch (Exception ex)");
            sb.AppendLine("            {");
            sb.AppendLine("                throw new Exception($\"خطأ في تحديث البيانات: {ex.Message}\", ex);");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public bool Delete(int id)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (id <= 0)");
            sb.AppendLine("                throw new ArgumentException(\"معرف غير صالح\", nameof(id));");
            sb.AppendLine();
            sb.AppendLine("            try");
            sb.AppendLine("            {");
            sb.AppendLine("                return _dal.Delete(id);");
            sb.AppendLine("            }");
            sb.AppendLine("            catch (Exception ex)");
            sb.AppendLine("            {");
            sb.AppendLine("                throw new Exception($\"خطأ في حذف البيانات: {ex.Message}\", ex);");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine($"        private void ValidateEntity({context.EntityName} entity)");
            sb.AppendLine("        {");
            sb.AppendLine("            // إضافة قواعد التحقق من صحة البيانات");
            sb.AppendLine("            // يمكن استخدام FluentValidation أو Data Annotations");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }

        private void ValidateEntity(CodeGenerationContext context)
        {
            // إضافة قواعد التحقق من صحة البيانات
            // يمكن استخدام FluentValidation أو Data Annotations
        }

        public override List<PreviewFile> GeneratePreview(TableInfo table, CodeGenerationContext context)
        {
            var files = new List<PreviewFile>();
            context.TableInfo = table;
            context.TableName = table.Name;
            context.EntityName = table.Name;

            files.Add(new PreviewFile
            {
                FileName = $"{context.EntityName}.cs",
                Content = GenerateModel(context),
                Language = "csharp"
            });

            files.Add(new PreviewFile
            {
                FileName = $"{context.EntityName}DAL.cs",
                Content = GenerateDAL(context),
                Language = "csharp"
            });

            files.Add(new PreviewFile
            {
                FileName = $"{context.EntityName}Business.cs",
                Content = GenerateBusinessLogic(context),
                Language = "csharp"
            });

            return files;
        }
    }
} 