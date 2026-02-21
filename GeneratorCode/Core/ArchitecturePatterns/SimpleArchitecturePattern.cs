using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        
        public override Task<CodeGenerationResult> Generate(CodeGenerationContext context)
        {
            var result = new CodeGenerationResult();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
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
                result.Message = $"حدث خطأ أثناء توليد الكود: {ex.Message}";
                result.Errors.Add(ex.ToString());
            }

            stopwatch.Stop();
            result.GenerationTime = stopwatch.Elapsed;

            return Task.FromResult(result);
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
                "Microsoft.Data.SqlClient",
                "MySql.Data",
                "Npgsql",
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
                    
                    var csharpType = column.CSharpType ?? "object";
                    
                    if (!column.IsNullable && csharpType == "string")
                    {
                        sb.AppendLine("        [Required]");
                    }
                    
                    if (column.MaxLength.HasValue && csharpType == "string")
                    {
                        sb.AppendLine($"        [StringLength({column.MaxLength.Value})]");
                    }
                    
                    sb.AppendLine($"        public {csharpType} {column.Name} {{ get; set; }}");
                    sb.AppendLine();
                }
            }
            
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
        
        private string GenerateDAL(CodeGenerationContext context)
        {
            var dbType = context.DatabaseType;
            var columns = context.TableInfo?.Columns ?? new List<ColumnInfo>();
            var pkColumn = columns.FirstOrDefault(c => c.IsPrimaryKey) ?? columns.FirstOrDefault();
            var pkName = pkColumn?.Name ?? "Id";
            var pkCSharpType = pkColumn?.CSharpType ?? "int";

            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Data;");
            if (dbType == DatabaseType.SqlServer)
                sb.AppendLine("using Microsoft.Data.SqlClient;");
            else if (dbType == DatabaseType.MySql)
                sb.AppendLine("using MySql.Data.MySqlClient;");
            else if (dbType == DatabaseType.PostgreSql)
                sb.AppendLine("using Npgsql;");
            sb.AppendLine($"using {context.Namespace}.Models;");
            sb.AppendLine();

            string connectionType, commandType, parameterType;
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                    connectionType = "SqlConnection";
                    commandType = "SqlCommand";
                    parameterType = "SqlParameter";
                    break;
                case DatabaseType.MySql:
                    connectionType = "MySqlConnection";
                    commandType = "MySqlCommand";
                    parameterType = "MySqlParameter";
                    break;
                case DatabaseType.PostgreSql:
                    connectionType = "NpgsqlConnection";
                    commandType = "NpgsqlCommand";
                    parameterType = "NpgsqlParameter";
                    break;
                default:
                    connectionType = "SqlConnection";
                    commandType = "SqlCommand";
                    parameterType = "SqlParameter";
                    break;
            }

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
            sb.AppendLine($"            var query = \"SELECT * FROM {EscapeTableName(context.TableName, dbType)}\";");
            sb.AppendLine();
            sb.AppendLine($"            using (var connection = new {connectionType}(_connectionString))");
            sb.AppendLine("            {");
            sb.AppendLine("                connection.Open();");
            sb.AppendLine($"                using (var command = new {commandType}(query, connection))");
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
            sb.AppendLine($"        public {context.EntityName} GetById({pkCSharpType} id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var query = \"SELECT * FROM {EscapeTableName(context.TableName, dbType)} WHERE {EscapeColumnName(pkName, dbType)} = @{pkName}\";");
            sb.AppendLine();
            sb.AppendLine($"            using (var connection = new {connectionType}(_connectionString))");
            sb.AppendLine("            {");
            sb.AppendLine("                connection.Open();");
            sb.AppendLine($"                using (var command = new {commandType}(query, connection))");
            sb.AppendLine("                {");
            sb.AppendLine($"                    command.Parameters.Add(new {parameterType}(\"@{pkName}\", id));");
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

            var insertColumns = columns.Where(c => !c.IsAutoIncrement).ToList();
            var insertColumnNames = string.Join(", ", insertColumns.Select(c => EscapeColumnName(c.Name, dbType)));
            var insertParams = string.Join(", ", insertColumns.Select(c => "@" + c.Name));
            var insertSql = $"INSERT INTO {EscapeTableName(context.TableName, dbType)} ({insertColumnNames}) VALUES ({insertParams})";
            var useReturning = dbType == DatabaseType.PostgreSql && pkColumn != null;
            if (useReturning)
                insertSql += $" RETURNING {EscapeColumnName(pkName, dbType)}";

            sb.AppendLine($"        public int Insert({context.EntityName} entity)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var query = \"{EscapeQuotedString(insertSql)}\";");
            if (!useReturning && (dbType == DatabaseType.SqlServer || dbType == DatabaseType.MySql))
                sb.AppendLine($"            var selectIdQuery = \"{(dbType == DatabaseType.SqlServer ? "SELECT CAST(SCOPE_IDENTITY() AS INT);" : "SELECT LAST_INSERT_ID();")}\";");
            sb.AppendLine();
            sb.AppendLine($"            using (var connection = new {connectionType}(_connectionString))");
            sb.AppendLine("            {");
            sb.AppendLine("                connection.Open();");
            sb.AppendLine($"                using (var command = new {commandType}(query, connection))");
            sb.AppendLine("                {");
            foreach (var col in insertColumns)
                sb.AppendLine($"                    command.Parameters.Add(new {parameterType}(\"@{col.Name}\", (object)entity.{col.Name} ?? DBNull.Value));");
            if (useReturning)
            {
                sb.AppendLine("                    var result = command.ExecuteScalar();");
                sb.AppendLine("                    return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;");
            }
            else
            {
                sb.AppendLine("                    command.ExecuteNonQuery();");
                if (dbType == DatabaseType.SqlServer || dbType == DatabaseType.MySql)
                {
                    sb.AppendLine($"                    using (var cmdId = new {commandType}(selectIdQuery, connection))");
                    sb.AppendLine("                    {");
                    sb.AppendLine("                        var result = cmdId.ExecuteScalar();");
                    sb.AppendLine("                        return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;");
                    sb.AppendLine("                    }");
                }
                else
                {
                    sb.AppendLine("                    return 0;");
                }
            }
            sb.AppendLine("                }");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();

            var updateColumns = columns.Where(c => !c.IsPrimaryKey).ToList();
            var setClause = string.Join(", ", updateColumns.Select(c => $"{EscapeColumnName(c.Name, dbType)} = @{c.Name}"));
            var updateSql = $"UPDATE {EscapeTableName(context.TableName, dbType)} SET {setClause} WHERE {EscapeColumnName(pkName, dbType)} = @{pkName}";

            sb.AppendLine($"        public bool Update({context.EntityName} entity)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var query = \"{EscapeQuotedString(updateSql)}\";");
            sb.AppendLine();
            sb.AppendLine($"            using (var connection = new {connectionType}(_connectionString))");
            sb.AppendLine("            {");
            sb.AppendLine("                connection.Open();");
            sb.AppendLine($"                using (var command = new {commandType}(query, connection))");
            sb.AppendLine("                {");
            foreach (var col in updateColumns)
                sb.AppendLine($"                    command.Parameters.Add(new {parameterType}(\"@{col.Name}\", (object)entity.{col.Name} ?? DBNull.Value));");
            sb.AppendLine($"                    command.Parameters.Add(new {parameterType}(\"@{pkName}\", entity.{pkName}));");
            sb.AppendLine("                    var affected = command.ExecuteNonQuery();");
            sb.AppendLine("                    return affected > 0;");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();

            var deleteSql = $"DELETE FROM {EscapeTableName(context.TableName, dbType)} WHERE {EscapeColumnName(pkName, dbType)} = @{pkName}";
            sb.AppendLine($"        public bool Delete({pkCSharpType} id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var query = \"{EscapeQuotedString(deleteSql)}\";");
            sb.AppendLine();
            sb.AppendLine($"            using (var connection = new {connectionType}(_connectionString))");
            sb.AppendLine("            {");
            sb.AppendLine("                connection.Open();");
            sb.AppendLine($"                using (var command = new {commandType}(query, connection))");
            sb.AppendLine("                {");
            sb.AppendLine($"                    command.Parameters.Add(new {parameterType}(\"@{pkName}\", id));");
            sb.AppendLine("                    var affected = command.ExecuteNonQuery();");
            sb.AppendLine("                    return affected > 0;");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();

            sb.AppendLine($"        private {context.EntityName} Map{context.EntityName}(IDataReader reader)");
            sb.AppendLine("        {");
            sb.AppendLine($"            return new {context.EntityName}");
            sb.AppendLine("            {");
            foreach (var column in columns)
                sb.AppendLine($"                {column.Name} = {GetMapExpression(column)},");
            sb.AppendLine("            };");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private static string EscapeTableName(string name, DatabaseType dbType)
        {
            if (string.IsNullOrEmpty(name)) return name;
            if (dbType == DatabaseType.SqlServer) return $"[{name}]";
            if (dbType == DatabaseType.MySql) return $"`{name}`";
            if (dbType == DatabaseType.PostgreSql) return $"\"{name}\"";
            return name;
        }

        private static string EscapeColumnName(string name, DatabaseType dbType)
        {
            if (string.IsNullOrEmpty(name)) return name;
            if (dbType == DatabaseType.SqlServer) return $"[{name}]";
            if (dbType == DatabaseType.MySql) return $"`{name}`";
            if (dbType == DatabaseType.PostgreSql) return $"\"{name}\"";
            return name;
        }

        private static string EscapeQuotedString(string s)
        {
            return s?.Replace("\\", "\\\\").Replace("\"", "\\\"") ?? string.Empty;
        }

        private static string GetMapExpression(ColumnInfo column)
        {
            var csharpType = (column.CSharpType ?? "object").Trim();
            var colName = column.Name;
            if (csharpType == "string")
                return $"reader[\"{colName}\"] == DBNull.Value || reader[\"{colName}\"] == null ? null : (string)reader[\"{colName}\"]";
            if (csharpType == "int")
                return $"reader[\"{colName}\"] == DBNull.Value || reader[\"{colName}\"] == null ? 0 : Convert.ToInt32(reader[\"{colName}\"])";
            if (csharpType == "int?")
                return $"reader[\"{colName}\"] == DBNull.Value || reader[\"{colName}\"] == null ? (int?)null : Convert.ToInt32(reader[\"{colName}\"])";
            if (csharpType == "long")
                return $"reader[\"{colName}\"] == DBNull.Value || reader[\"{colName}\"] == null ? 0L : Convert.ToInt64(reader[\"{colName}\"])";
            if (csharpType == "long?")
                return $"reader[\"{colName}\"] == DBNull.Value || reader[\"{colName}\"] == null ? (long?)null : Convert.ToInt64(reader[\"{colName}\"])";
            if (csharpType == "decimal")
                return $"reader[\"{colName}\"] == DBNull.Value || reader[\"{colName}\"] == null ? 0m : Convert.ToDecimal(reader[\"{colName}\"])";
            if (csharpType == "decimal?")
                return $"reader[\"{colName}\"] == DBNull.Value || reader[\"{colName}\"] == null ? (decimal?)null : Convert.ToDecimal(reader[\"{colName}\"])";
            if (csharpType == "bool")
                return $"reader[\"{colName}\"] == DBNull.Value || reader[\"{colName}\"] == null ? false : Convert.ToBoolean(reader[\"{colName}\"])";
            if (csharpType == "bool?")
                return $"reader[\"{colName}\"] == DBNull.Value || reader[\"{colName}\"] == null ? (bool?)null : Convert.ToBoolean(reader[\"{colName}\"])";
            if (csharpType == "DateTime")
                return $"reader[\"{colName}\"] == DBNull.Value || reader[\"{colName}\"] == null ? DateTime.MinValue : Convert.ToDateTime(reader[\"{colName}\"])";
            if (csharpType == "DateTime?")
                return $"reader[\"{colName}\"] == DBNull.Value || reader[\"{colName}\"] == null ? (DateTime?)null : Convert.ToDateTime(reader[\"{colName}\"])";
            if (csharpType == "byte[]")
                return $"reader[\"{colName}\"] == DBNull.Value || reader[\"{colName}\"] == null ? null : (byte[])reader[\"{colName}\"]";
            if (csharpType == "Guid")
                return $"reader[\"{colName}\"] == DBNull.Value || reader[\"{colName}\"] == null ? Guid.Empty : (Guid)reader[\"{colName}\"]";
            if (csharpType == "Guid?")
                return $"reader[\"{colName}\"] == DBNull.Value || reader[\"{colName}\"] == null ? (Guid?)null : (Guid)reader[\"{colName}\"]";
            return $"reader[\"{colName}\"] == DBNull.Value || reader[\"{colName}\"] == null ? null : reader[\"{colName}\"]";
        }
        
        private string GenerateBusinessLogic(CodeGenerationContext context)
        {
            var columns = context.TableInfo?.Columns ?? new List<ColumnInfo>();
            var pkColumn = columns.FirstOrDefault(c => c.IsPrimaryKey) ?? columns.FirstOrDefault();
            var pkCSharpType = pkColumn?.CSharpType ?? "int";

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
            sb.AppendLine("                throw new Exception($\"خطأ في جلب البيانات: {ex.Message}\", ex);");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public {context.EntityName} GetById({pkCSharpType} id)");
            sb.AppendLine("        {");
            sb.AppendLine(GetIdValidationLine(pkCSharpType));
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
            sb.AppendLine($"        public bool Delete({pkCSharpType} id)");
            sb.AppendLine("        {");
            sb.AppendLine(GetIdValidationLine(pkCSharpType));
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
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        private void ValidateEntity({context.EntityName} entity)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (entity == null)");
            sb.AppendLine("                throw new ArgumentNullException(nameof(entity));");
            sb.AppendLine();
            foreach (var column in columns)
            {
                var csharpType = column.CSharpType ?? "object";
                if (csharpType == "string" && !column.IsPrimaryKey)
                {
                    sb.AppendLine($"            if (string.IsNullOrWhiteSpace(entity.{column.Name}))");
                    sb.AppendLine($"                throw new ArgumentException(\"حقل '{column.Name}' مطلوب ولا يمكن أن يكون فارغاً.\", nameof(entity));");
                    if (column.MaxLength.HasValue)
                    {
                        sb.AppendLine($"            if (entity.{column.Name} != null && entity.{column.Name}.Length > {column.MaxLength.Value})");
                        sb.AppendLine($"                throw new ArgumentException($\"حقل '{column.Name}' يجب ألا يتجاوز {column.MaxLength.Value} حرفاً.\", nameof(entity));");
                    }
                    sb.AppendLine();
                }
            }
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private static string GetIdValidationLine(string pkCSharpType)
        {
            if (pkCSharpType == "int" || pkCSharpType == "long")
                return "            if (id <= 0)";
            if (pkCSharpType == "int?" || pkCSharpType == "long?")
                return "            if (!id.HasValue || id.Value <= 0)";
            if (pkCSharpType == "string")
                return "            if (string.IsNullOrEmpty(id))";
            return "            if (id == null)";
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