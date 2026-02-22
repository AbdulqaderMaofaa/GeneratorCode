using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.CodeGenerators
{
    public static class AspNetCoreViewGenerator
    {
        public static List<GeneratedFile> Generate(CodeGenerationContext context)
        {
            var files = new List<GeneratedFile>();
            var entityName = context.EntityName ?? "Entity";
            var ns = context.Namespace ?? "GeneratedCode";
            var outputPath = context.OutputPath ?? ".";
            var columns = context.TableInfo?.Columns ?? new List<ColumnInfo>();

            files.Add(CreateFile(outputPath, $"Pages/{entityName}", "Index.cshtml",
                GenerateIndexPage(entityName, ns, columns), "Presentation"));

            files.Add(CreateFile(outputPath, $"Pages/{entityName}", "Index.cshtml.cs",
                GenerateIndexPageModel(entityName, ns), "Presentation"));

            files.Add(CreateFile(outputPath, $"Pages/{entityName}", "Create.cshtml",
                GenerateCreatePage(entityName, ns, columns), "Presentation"));

            files.Add(CreateFile(outputPath, $"Pages/{entityName}", "Create.cshtml.cs",
                GenerateCreatePageModel(entityName, ns, columns), "Presentation"));

            files.Add(CreateFile(outputPath, $"Pages/{entityName}", "Edit.cshtml",
                GenerateEditPage(entityName, ns, columns), "Presentation"));

            files.Add(CreateFile(outputPath, $"Pages/{entityName}", "Edit.cshtml.cs",
                GenerateEditPageModel(entityName, ns, columns), "Presentation"));

            files.Add(CreateFile(outputPath, $"Pages/{entityName}", "Details.cshtml",
                GenerateDetailsPage(entityName, ns, columns), "Presentation"));

            files.Add(CreateFile(outputPath, $"Pages/{entityName}", "Details.cshtml.cs",
                GenerateDetailsPageModel(entityName, ns), "Presentation"));

            files.Add(CreateFile(outputPath, $"Pages/{entityName}", "Delete.cshtml",
                GenerateDeletePage(entityName, ns, columns), "Presentation"));

            files.Add(CreateFile(outputPath, $"Pages/{entityName}", "Delete.cshtml.cs",
                GenerateDeletePageModel(entityName, ns), "Presentation"));

            return files;
        }

        private static string GenerateIndexPage(string entityName, string ns, List<ColumnInfo> columns)
        {
            var displayCols = columns.Take(8).ToList();
            var headers = string.Join("\n            ", displayCols.Select(c => $"<th>@Html.DisplayNameFor(model => model.Items[0].{c.Name})</th>"));
            var cells = string.Join("\n                ", displayCols.Select(c => $"<td>@Html.DisplayFor(modelItem => item.{c.Name})</td>"));
            var pkCol = columns.FirstOrDefault(c => c.IsPrimaryKey);
            var pkName = pkCol?.Name ?? "Id";

            return $@"@page
@model {ns}.Pages.{entityName}.IndexModel
@{{
    ViewData[""Title""] = ""{entityName} List"";
}}

<h1>{entityName}</h1>

<p>
    <a asp-page=""Create"">Create New</a>
</p>
<table class=""table"">
    <thead>
        <tr>
            {headers}
            <th></th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in Model.Items)
        {{
            <tr>
                {cells}
                <td>
                    <a asp-page=""Edit"" asp-route-id=""@item.{pkName}"">Edit</a> |
                    <a asp-page=""Details"" asp-route-id=""@item.{pkName}"">Details</a> |
                    <a asp-page=""Delete"" asp-route-id=""@item.{pkName}"">Delete</a>
                </td>
            </tr>
        }}
    </tbody>
</table>
";
        }

        private static string GenerateIndexPageModel(string entityName, string ns)
        {
            return $@"using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace {ns}.Pages.{entityName}
{{
    public class IndexModel : PageModel
    {{
        private readonly {ns}DbContext _context;

        public IndexModel({ns}DbContext context)
        {{
            _context = context;
        }}

        public IList<{ns}.Models.{entityName}> Items {{ get; set; }} = new List<{ns}.Models.{entityName}>();

        public async Task OnGetAsync()
        {{
            Items = await _context.{entityName}.ToListAsync();
        }}
    }}
}}
";
        }

        private static string GenerateCreatePage(string entityName, string ns, List<ColumnInfo> columns)
        {
            var formFields = columns.Where(c => !c.IsPrimaryKey && !c.IsAutoIncrement).ToList();
            var inputs = string.Join("\n        ", formFields.Select(c =>
                $@"<div class=""mb-3"">
            <label asp-for=""Item.{c.Name}"" class=""form-label""></label>
            <input asp-for=""Item.{c.Name}"" class=""form-control"" />
            <span asp-validation-for=""Item.{c.Name}"" class=""text-danger""></span>
        </div>"));

            return $@"@page
@model {ns}.Pages.{entityName}.CreateModel
@{{
    ViewData[""Title""] = ""Create {entityName}"";
}}

<h1>Create {entityName}</h1>

<form method=""post"">
    <div asp-validation-summary=""ModelOnly"" class=""text-danger""></div>
        {inputs}
    <button type=""submit"" class=""btn btn-primary"">Create</button>
    <a asp-page=""Index"" class=""btn btn-secondary"">Back to List</a>
</form>
";
        }

        private static string GenerateCreatePageModel(string entityName, string ns, List<ColumnInfo> columns)
        {
            return $@"using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace {ns}.Pages.{entityName}
{{
    public class CreateModel : PageModel
    {{
        private readonly {ns}DbContext _context;

        public CreateModel({ns}DbContext context)
        {{
            _context = context;
        }}

        [BindProperty]
        public {ns}.Models.{entityName} Item {{ get; set; }} = default!;

        public IActionResult OnGet()
        {{
            return Page();
        }}

        public async Task<IActionResult> OnPostAsync()
        {{
            if (!ModelState.IsValid)
                return Page();

            _context.{entityName}.Add(Item);
            await _context.SaveChangesAsync();
            return RedirectToPage(""./Index"");
        }}
    }}
}}
";
        }

        private static string GenerateEditPage(string entityName, string ns, List<ColumnInfo> columns)
        {
            var formFields = columns.Where(c => !c.IsAutoIncrement).ToList();
            var pkCol = columns.FirstOrDefault(c => c.IsPrimaryKey);
            var pkName = pkCol?.Name ?? "Id";
            var inputs = string.Join("\n        ", formFields.Select(c =>
            {
                if (c.IsPrimaryKey)
                    return $@"<input type=""hidden"" asp-for=""Item.{c.Name}"" />";
                return $@"<div class=""mb-3"">
            <label asp-for=""Item.{c.Name}"" class=""form-label""></label>
            <input asp-for=""Item.{c.Name}"" class=""form-control"" />
            <span asp-validation-for=""Item.{c.Name}"" class=""text-danger""></span>
        </div>";
            }));

            return $@"@page ""{{id}}""
@model {ns}.Pages.{entityName}.EditModel
@{{
    ViewData[""Title""] = ""Edit {entityName}"";
}}

<h1>Edit {entityName}</h1>

<form method=""post"">
    <div asp-validation-summary=""ModelOnly"" class=""text-danger""></div>
        {inputs}
    <button type=""submit"" class=""btn btn-primary"">Save</button>
    <a asp-page=""Index"" class=""btn btn-secondary"">Back to List</a>
</form>
";
        }

        private static string GenerateEditPageModel(string entityName, string ns, List<ColumnInfo> columns)
        {
            var pkCol = columns.FirstOrDefault(c => c.IsPrimaryKey);
            var pkName = pkCol?.Name ?? "Id";
            var pkType = pkCol?.CSharpType ?? "int";

            return $@"using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace {ns}.Pages.{entityName}
{{
    public class EditModel : PageModel
    {{
        private readonly {ns}DbContext _context;

        public EditModel({ns}DbContext context)
        {{
            _context = context;
        }}

        [BindProperty]
        public {ns}.Models.{entityName} Item {{ get; set; }} = default!;

        public async Task<IActionResult> OnGetAsync({pkType} id)
        {{
            var item = await _context.{entityName}.FindAsync(id);
            if (item == null) return NotFound();
            Item = item;
            return Page();
        }}

        public async Task<IActionResult> OnPostAsync()
        {{
            if (!ModelState.IsValid) return Page();

            _context.Attach(Item).State = EntityState.Modified;
            try
            {{
                await _context.SaveChangesAsync();
            }}
            catch (DbUpdateConcurrencyException)
            {{
                if (!await _context.{entityName}.AnyAsync(e => e.{pkName} == Item.{pkName}))
                    return NotFound();
                throw;
            }}
            return RedirectToPage(""./Index"");
        }}
    }}
}}
";
        }

        private static string GenerateDetailsPage(string entityName, string ns, List<ColumnInfo> columns)
        {
            var rows = string.Join("\n        ", columns.Select(c =>
                $@"<dt class=""col-sm-3"">@Html.DisplayNameFor(model => model.Item.{c.Name})</dt>
        <dd class=""col-sm-9"">@Html.DisplayFor(model => model.Item.{c.Name})</dd>"));
            var pkCol = columns.FirstOrDefault(c => c.IsPrimaryKey);
            var pkName = pkCol?.Name ?? "Id";

            return $@"@page ""{{id}}""
@model {ns}.Pages.{entityName}.DetailsModel
@{{
    ViewData[""Title""] = ""{entityName} Details"";
}}

<h1>{entityName} Details</h1>

<dl class=""row"">
        {rows}
</dl>
<div>
    <a asp-page=""Edit"" asp-route-id=""@Model.Item.{pkName}"">Edit</a> |
    <a asp-page=""Index"">Back to List</a>
</div>
";
        }

        private static string GenerateDetailsPageModel(string entityName, string ns)
        {
            return $@"using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace {ns}.Pages.{entityName}
{{
    public class DetailsModel : PageModel
    {{
        private readonly {ns}DbContext _context;

        public DetailsModel({ns}DbContext context)
        {{
            _context = context;
        }}

        public {ns}.Models.{entityName} Item {{ get; set; }} = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {{
            var item = await _context.{entityName}.FindAsync(id);
            if (item == null) return NotFound();
            Item = item;
            return Page();
        }}
    }}
}}
";
        }

        private static string GenerateDeletePage(string entityName, string ns, List<ColumnInfo> columns)
        {
            var rows = string.Join("\n        ", columns.Take(5).Select(c =>
                $@"<dt class=""col-sm-3"">@Html.DisplayNameFor(model => model.Item.{c.Name})</dt>
        <dd class=""col-sm-9"">@Html.DisplayFor(model => model.Item.{c.Name})</dd>"));
            var pkCol = columns.FirstOrDefault(c => c.IsPrimaryKey);
            var pkName = pkCol?.Name ?? "Id";

            return $@"@page ""{{id}}""
@model {ns}.Pages.{entityName}.DeleteModel
@{{
    ViewData[""Title""] = ""Delete {entityName}"";
}}

<h1>Delete {entityName}</h1>

<h3>Are you sure you want to delete this?</h3>
<dl class=""row"">
        {rows}
</dl>
<form method=""post"">
    <input type=""hidden"" asp-for=""Item.{pkName}"" />
    <button type=""submit"" class=""btn btn-danger"">Delete</button>
    <a asp-page=""Index"" class=""btn btn-secondary"">Back to List</a>
</form>
";
        }

        private static string GenerateDeletePageModel(string entityName, string ns)
        {
            return $@"using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace {ns}.Pages.{entityName}
{{
    public class DeleteModel : PageModel
    {{
        private readonly {ns}DbContext _context;

        public DeleteModel({ns}DbContext context)
        {{
            _context = context;
        }}

        [BindProperty]
        public {ns}.Models.{entityName} Item {{ get; set; }} = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {{
            var item = await _context.{entityName}.FindAsync(id);
            if (item == null) return NotFound();
            Item = item;
            return Page();
        }}

        public async Task<IActionResult> OnPostAsync(int id)
        {{
            var item = await _context.{entityName}.FindAsync(id);
            if (item != null)
            {{
                _context.{entityName}.Remove(item);
                await _context.SaveChangesAsync();
            }}
            return RedirectToPage(""./Index"");
        }}
    }}
}}
";
        }

        private static GeneratedFile CreateFile(string outputPath, string folder, string fileName, string content, string layer)
        {
            var relativePath = System.IO.Path.Combine(folder, fileName);
            var fullPath = System.IO.Path.Combine(outputPath, relativePath);
            var ext = System.IO.Path.GetExtension(fileName).TrimStart('.');
            return new GeneratedFile
            {
                FileName = fileName,
                RelativePath = relativePath,
                FullPath = fullPath,
                Content = content,
                FileType = ext,
                Layer = layer,
                SizeInBytes = Encoding.UTF8.GetByteCount(content ?? ""),
                CreatedDate = DateTime.Now
            };
        }
    }
}
