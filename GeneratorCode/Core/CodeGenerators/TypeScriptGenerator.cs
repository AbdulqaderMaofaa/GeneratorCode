using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.CodeGenerators
{
    public static class TypeScriptGenerator
    {
        public static List<GeneratedFile> Generate(CodeGenerationContext context)
        {
            var files = new List<GeneratedFile>();
            var entityName = context.EntityName ?? "Entity";
            var outputPath = context.OutputPath ?? ".";
            var columns = context.TableInfo?.Columns ?? new List<ColumnInfo>();

            files.Add(CreateFile(outputPath, "models", $"{CamelCase(entityName)}.model.ts",
                GenerateModel(entityName, columns), "Models"));

            files.Add(CreateFile(outputPath, "models", $"create-{KebabCase(entityName)}.dto.ts",
                GenerateCreateDto(entityName, columns), "Models"));

            files.Add(CreateFile(outputPath, "models", $"update-{KebabCase(entityName)}.dto.ts",
                GenerateUpdateDto(entityName, columns), "Models"));

            files.Add(CreateFile(outputPath, "services", $"{CamelCase(entityName)}.service.ts",
                GenerateAngularService(entityName), "Services"));

            files.Add(CreateFile(outputPath, "services", $"{CamelCase(entityName)}.api.ts",
                GenerateFetchApi(entityName), "Services"));

            files.Add(CreateFile(outputPath, "components", $"{entityName}List.tsx",
                GenerateReactListComponent(entityName, columns), "Presentation"));

            files.Add(CreateFile(outputPath, "components", $"{entityName}Form.tsx",
                GenerateReactFormComponent(entityName, columns), "Presentation"));

            files.Add(CreateFile(outputPath, "", "index.ts",
                GenerateIndexExports(entityName), "Root"));

            return files;
        }

        private static string GenerateModel(string entityName, List<ColumnInfo> columns)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"export interface {entityName} {{");
            foreach (var col in columns)
            {
                var tsType = MapToTypeScript(col.CSharpType, col.IsNullable);
                sb.AppendLine($"  {CamelCase(col.Name)}{(col.IsNullable && !col.IsPrimaryKey ? "?" : "")}: {tsType};");
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateCreateDto(string entityName, List<ColumnInfo> columns)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"export interface Create{entityName}Dto {{");
            foreach (var col in columns.Where(c => !c.IsPrimaryKey && !c.IsAutoIncrement))
            {
                var tsType = MapToTypeScript(col.CSharpType, col.IsNullable);
                sb.AppendLine($"  {CamelCase(col.Name)}{(col.IsNullable ? "?" : "")}: {tsType};");
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateUpdateDto(string entityName, List<ColumnInfo> columns)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"export interface Update{entityName}Dto {{");
            foreach (var col in columns.Where(c => !c.IsAutoIncrement))
            {
                var tsType = MapToTypeScript(col.CSharpType, col.IsNullable);
                sb.AppendLine($"  {CamelCase(col.Name)}{(col.IsNullable && !col.IsPrimaryKey ? "?" : "")}: {tsType};");
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateAngularService(string entityName)
        {
            var camel = CamelCase(entityName);
            var kebab = KebabCase(entityName);
            return $@"import {{ Injectable }} from '@angular/core';
import {{ HttpClient }} from '@angular/common/http';
import {{ Observable }} from 'rxjs';
import {{ {entityName} }} from '../models/{camel}.model';
import {{ Create{entityName}Dto }} from '../models/create-{kebab}.dto';
import {{ Update{entityName}Dto }} from '../models/update-{kebab}.dto';

@Injectable({{
  providedIn: 'root'
}})
export class {entityName}Service {{
  private readonly apiUrl = '/api/{camel}';

  constructor(private http: HttpClient) {{}}

  getAll(): Observable<{entityName}[]> {{
    return this.http.get<{entityName}[]>(this.apiUrl);
  }}

  getById(id: number | string): Observable<{entityName}> {{
    return this.http.get<{entityName}>(`${{this.apiUrl}}/${{id}}`);
  }}

  create(dto: Create{entityName}Dto): Observable<{entityName}> {{
    return this.http.post<{entityName}>(this.apiUrl, dto);
  }}

  update(id: number | string, dto: Update{entityName}Dto): Observable<{entityName}> {{
    return this.http.put<{entityName}>(`${{this.apiUrl}}/${{id}}`, dto);
  }}

  delete(id: number | string): Observable<void> {{
    return this.http.delete<void>(`${{this.apiUrl}}/${{id}}`);
  }}
}}
";
        }

        private static string GenerateFetchApi(string entityName)
        {
            var camel = CamelCase(entityName);
            var kebab = KebabCase(entityName);
            return $@"import {{ {entityName} }} from '../models/{camel}.model';
import {{ Create{entityName}Dto }} from '../models/create-{kebab}.dto';
import {{ Update{entityName}Dto }} from '../models/update-{kebab}.dto';

const API_URL = '/api/{camel}';

async function handleResponse<T>(response: Response): Promise<T> {{
  if (!response.ok) {{
    const error = await response.text();
    throw new Error(error || response.statusText);
  }}
  return response.json();
}}

export const {camel}Api = {{
  async getAll(): Promise<{entityName}[]> {{
    const res = await fetch(API_URL);
    return handleResponse<{entityName}[]>(res);
  }},

  async getById(id: number | string): Promise<{entityName}> {{
    const res = await fetch(`${{API_URL}}/${{id}}`);
    return handleResponse<{entityName}>(res);
  }},

  async create(dto: Create{entityName}Dto): Promise<{entityName}> {{
    const res = await fetch(API_URL, {{
      method: 'POST',
      headers: {{ 'Content-Type': 'application/json' }},
      body: JSON.stringify(dto),
    }});
    return handleResponse<{entityName}>(res);
  }},

  async update(id: number | string, dto: Update{entityName}Dto): Promise<{entityName}> {{
    const res = await fetch(`${{API_URL}}/${{id}}`, {{
      method: 'PUT',
      headers: {{ 'Content-Type': 'application/json' }},
      body: JSON.stringify(dto),
    }});
    return handleResponse<{entityName}>(res);
  }},

  async delete(id: number | string): Promise<void> {{
    const res = await fetch(`${{API_URL}}/${{id}}`, {{ method: 'DELETE' }});
    if (!res.ok) throw new Error(await res.text());
  }},
}};
";
        }

        private static string GenerateReactListComponent(string entityName, List<ColumnInfo> columns)
        {
            var camel = CamelCase(entityName);
            var kebab = KebabCase(entityName);
            var displayCols = columns.Take(6).ToList();
            var headers = string.Join("\n        ", displayCols.Select(c => $"<th>{c.Name}</th>"));
            var cells = string.Join("\n          ", displayCols.Select(c => $"<td>{{item.{CamelCase(c.Name)}}}</td>"));
            var pkCol = columns.FirstOrDefault(c => c.IsPrimaryKey);
            var pkProp = pkCol != null ? CamelCase(pkCol.Name) : "id";

            return $@"import React, {{ useEffect, useState }} from 'react';
import {{ {entityName} }} from '../models/{camel}.model';
import {{ {camel}Api }} from '../services/{camel}.api';

const {entityName}List: React.FC = () => {{
  const [items, setItems] = useState<{entityName}[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {{
    {camel}Api.getAll()
      .then(setItems)
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false));
  }}, []);

  const handleDelete = async ({pkProp}: number | string) => {{
    if (!window.confirm('Are you sure?')) return;
    try {{
      await {camel}Api.delete({pkProp});
      setItems((prev) => prev.filter((i) => i.{pkProp} !== {pkProp}));
    }} catch (e: any) {{
      setError(e.message);
    }}
  }};

  if (loading) return <div>Loading...</div>;
  if (error) return <div className=""error"">{{error}}</div>;

  return (
    <div>
      <h2>{entityName} List</h2>
      <table>
        <thead>
          <tr>
        {headers}
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {{items.map((item) => (
            <tr key={{String(item.{pkProp})}}>
          {cells}
              <td>
                <button onClick={{() => handleDelete(item.{pkProp})}}>Delete</button>
              </td>
            </tr>
          ))}}
        </tbody>
      </table>
    </div>
  );
}};

export default {entityName}List;
";
        }

        private static string GenerateReactFormComponent(string entityName, List<ColumnInfo> columns)
        {
            var camel = CamelCase(entityName);
            var kebab = KebabCase(entityName);
            var formFields = columns.Where(c => !c.IsPrimaryKey && !c.IsAutoIncrement).ToList();
            var stateInit = string.Join(", ", formFields.Select(c =>
                $"{CamelCase(c.Name)}: {GetTsDefault(c.CSharpType)}"));
            var inputs = string.Join("\n      ", formFields.Select(c =>
            {
                var prop = CamelCase(c.Name);
                var inputType = GetHtmlInputType(c.CSharpType);
                return $@"<div>
        <label>{c.Name}</label>
        <input type=""{inputType}"" value={{formData.{prop}}} onChange={{(e) => setFormData({{ ...formData, {prop}: e.target.value }})}} {(c.IsNullable ? "" : "required ")}/>
      </div>";
            }));

            return $@"import React, {{ useState }} from 'react';
import {{ Create{entityName}Dto }} from '../models/create-{kebab}.dto';
import {{ {camel}Api }} from '../services/{camel}.api';

interface Props {{
  onSuccess?: () => void;
}}

const {entityName}Form: React.FC<Props> = ({{ onSuccess }}) => {{
  const [formData, setFormData] = useState<Create{entityName}Dto>({{ {stateInit} }});
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {{
    e.preventDefault();
    setSubmitting(true);
    setError(null);
    try {{
      await {camel}Api.create(formData);
      onSuccess?.();
    }} catch (err: any) {{
      setError(err.message);
    }} finally {{
      setSubmitting(false);
    }}
  }};

  return (
    <form onSubmit={{handleSubmit}}>
      <h2>Create {entityName}</h2>
      {{error && <div className=""error"">{{error}}</div>}}
      {inputs}
      <button type=""submit"" disabled={{submitting}}>
        {{submitting ? 'Saving...' : 'Save'}}
      </button>
    </form>
  );
}};

export default {entityName}Form;
";
        }

        private static string GenerateIndexExports(string entityName)
        {
            var camel = CamelCase(entityName);
            var kebab = KebabCase(entityName);
            return $@"export {{ type {entityName} }} from './models/{camel}.model';
export {{ type Create{entityName}Dto }} from './models/create-{kebab}.dto';
export {{ type Update{entityName}Dto }} from './models/update-{kebab}.dto';
export {{ {camel}Api }} from './services/{camel}.api';
";
        }

        private static string MapToTypeScript(string csharpType, bool isNullable)
        {
            var baseType = (csharpType ?? "string").TrimEnd('?');
            var tsType = baseType switch
            {
                "int" or "long" or "short" or "byte" or "decimal" or "float" or "double" => "number",
                "bool" or "boolean" => "boolean",
                "DateTime" or "DateTimeOffset" or "DateOnly" or "TimeOnly" => "string",
                "Guid" => "string",
                "byte[]" => "string",
                _ => "string"
            };
            return isNullable ? $"{tsType} | null" : tsType;
        }

        private static string GetTsDefault(string csharpType)
        {
            var baseType = (csharpType ?? "string").TrimEnd('?');
            return baseType switch
            {
                "int" or "long" or "short" or "byte" or "decimal" or "float" or "double" => "0",
                "bool" or "boolean" => "false",
                _ => "''"
            };
        }

        private static string GetHtmlInputType(string csharpType)
        {
            var baseType = (csharpType ?? "string").TrimEnd('?');
            return baseType switch
            {
                "int" or "long" or "short" or "byte" or "decimal" or "float" or "double" => "number",
                "bool" or "boolean" => "checkbox",
                "DateTime" or "DateTimeOffset" => "datetime-local",
                "DateOnly" => "date",
                "TimeOnly" => "time",
                _ => "text"
            };
        }

        private static string CamelCase(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }

        private static string KebabCase(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            var sb = new StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]) && i > 0)
                    sb.Append('-');
                sb.Append(char.ToLowerInvariant(name[i]));
            }
            return sb.ToString();
        }

        private static GeneratedFile CreateFile(string outputPath, string folder, string fileName, string content, string layer)
        {
            var relativePath = string.IsNullOrEmpty(folder) ? fileName : System.IO.Path.Combine(folder, fileName);
            var fullPath = System.IO.Path.Combine(outputPath, relativePath);
            return new GeneratedFile
            {
                FileName = fileName,
                RelativePath = relativePath,
                FullPath = fullPath,
                Content = content,
                FileType = "ts",
                Layer = layer,
                SizeInBytes = Encoding.UTF8.GetByteCount(content ?? ""),
                CreatedDate = DateTime.Now
            };
        }
    }
}
