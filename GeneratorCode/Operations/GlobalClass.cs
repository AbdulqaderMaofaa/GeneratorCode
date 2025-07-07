using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace GeneratorCode.Operations
{
    public class GlobalClass
    {
        public string SName { get; set; }
        public string EName { get; set; }
        public string GlobalConString { get; set; }
        public string TName { get; set; }
        public string BaseName { get; set; }
        public DataTable Table { get; set; }
        public DataTable ColsTable { get; set; }
        readonly List<string> ExcludeFields = new List<string>();
        //{
        //"AddUserId"
        //,"UpdateUserId"
        //,"DateCreated"
        //,"DateUpdated"
        //,"EstNo"
        //,"OrderDisplay"
        //,"IsDeleted"
        // };
        public DataTable GetPrimaryKeyColumn(string connectionString)
        {

            string query = "SELECT distinct ColumnName = col.column_name ,\r\n" +
                "case when tc.Constraint_Type = 'Primary Key' then CAST(1 as bit) " +
                " else CAST(0 as bit) end as IsKey,c.DATA_TYPE  \r\n" +
                " FROM information_schema.table_constraints tc \r\n" +
                " INNER JOIN information_schema.key_column_usage col " +
                " ON col.Constraint_Name = tc.Constraint_Name AND \r\n" +
                " col.Constraint_schema = tc.Constraint_schema " +
                " inner join INFORMATION_SCHEMA.COLUMNS c on c.COLUMN_NAME=col.COLUMN_NAME" +
                " WHERE  tc.Constraint_Type = 'Primary Key' AND " +
                " col.Table_name = '" + BaseName + "'"; ;
            DataTable cols = GetTableSchema(query, connectionString);

            return cols;
        }
        private bool IsPrimaryKey(string colName)
        {
            string query = "SELECT distinct ColumnName = col.column_name ,\r\n" +
                "case when tc.Constraint_Type = 'Primary Key' then CAST(1 as bit) else CAST(0 as bit) end as IsKey\r\n" +
                "FROM information_schema.table_constraints tc \r\n" +
                "INNER JOIN information_schema.key_column_usage col ON col.Constraint_Name = tc.Constraint_Name AND \r\n" +
                "col.Constraint_schema = tc.Constraint_schema " +
                " WHERE  col.Table_name = '" + BaseName + "'"; ;
            ColsTable = GetTableSchema(query, GlobalConString);
            foreach (DataRow row in ColsTable.Rows)
            {
                string col = row["ColumnName"].ToString();
                if (col == colName)
                {
                    string isKey = row["IsKey"].ToString();
                    _ = bool.TryParse(isKey, out bool IsKey);
                    return IsKey;
                }
            }
            return false;
        }
        public static DataTable GetTableSchema(string query, string conString)
        {
            using var cn = new SqlConnection(conString);
            using var cmd = new SqlCommand { Connection = cn, CommandText = query };

            cn.Open();
            var dt = new DataTable();

            dt.Load(cmd.ExecuteReader());

            return dt;

        }

        public void GeneratingCode(string connectionString, string tableName, string entityName, string className, string Namespace, string FilePath)
        {
            DataTable mTable = new DataTable();
            SName = Namespace;
            EName = className;
            GlobalConString = connectionString;
            TName = entityName;
            mTable.Columns.Add("Column_name");
            mTable.Columns.Add("Type");
            mTable.Columns.Add("ISNULLABLE", typeof(bool));
            mTable.Columns.Add("IsKey", typeof(bool));

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            DataTable schemaTable = connection.GetSchema("Columns", new string[] { null, null, tableName });
            foreach (DataRow row in schemaTable.Rows)
            {
                string columnName = row["COLUMN_NAME"].ToString();
                string dataType = row["DATA_TYPE"].ToString();
                string IS_NULLABLE = row["IS_NULLABLE"].ToString();
                if (!ExcludeFields.Contains(columnName))
                {
                    DataRow r = mTable.NewRow();
                    string colName = columnName;
                    bool isNull = IS_NULLABLE.ToLower() == "yes";
                    bool IsKey = IsPrimaryKey(colName);
                    r["Column_name"] = colName;
                    r["Type"] = dataType;
                    r["ISNULLABLE"] = isNull;
                    r["IsKey"] = IsKey;
                    mTable.Rows.Add(r);
                }


            }
            Table = mTable;
            connection.Close();
            CoreCleanArchitecture(entityName, className, FilePath, Namespace, mTable);
        }
        public void CoreCleanArchitecture(string entityName, string TableName, string FilePath, string Namespace, DataTable schemaTable)
        {
            _ = FilePath.Split(".");

            string fName1 = "Get" + TableName + "ByIdQuery";
            string fName2 = "Get" + TableName + "ListQuery";
            string fName3 = "Get" + TableName + "PaginatedListQuery";
            string[] filesName = { fName1, fName2, fName3 };
            string Add = "Add" + TableName + "";
            string Edit = "Edit" + TableName + "";
            string Delete = "Delete" + TableName + "";
            string[] fCommands = { "Add{table}Command", "Edit{table}Command", "Delete{table}Command" };
            string[] fValidators = { "Add{table}Validator", "Edit{table}Validator" };


            string[] coreFolders = { "Commands", "Queries", "Mapping" };
            string[] foldersQueries = { "Handlers", "Models", "Results" };
            string[] foldersCommands = { "Handlers", "Models", "Validators" };
            string mainPath = FilePath + @"\" + Namespace;
            StringBuilder sb = new StringBuilder();
            StringBuilder sbc = new StringBuilder();

            string responseClass = CreateResponseClass(schemaTable);

            string queryByIdClass = CreateSingleQueryClass(TableName);
            string queryListClass = CreateListQueryClass(TableName, "ListQuery");
            string queryPaginationListClass = CreatePaginationListQueryClass(TableName, "PaginatedListQuery");
            string handlerQueryClass = CreateHandlerQueryClass(TableName, entityName);

            string addModelCommandClass = CreateAddEditCommandClass(schemaTable, TableName, Add + "Command", "Add");
            string editModelCommandClass = CreateAddEditCommandClass(schemaTable, TableName, Edit + "Command", "Edit");
            string deleteModelCommandClass = CreateAddEditCommandClass(schemaTable, TableName, Delete + "Command", "Delete");

            string addValidatorCommandClass = CreateValidatorsCommandClass(TableName, "Add");
            string editValidatorCommandClass = CreateValidatorsCommandClass(TableName, "Edit");
            string handlerCommandClass = CreateHandlerCommandClass(TableName, entityName);
            _ = new List<string>();

            if (!Directory.Exists(mainPath))
            {
                Directory.CreateDirectory(mainPath);
                string subFolder = mainPath + @"\" + Namespace;
                if (!Directory.Exists(subFolder))
                {
                    Directory.CreateDirectory(subFolder);
                }
                foreach (var folder in coreFolders)
                {
                    if (folder == "Queries")
                    {
                        sb.Clear();
                        foreach (var f in foldersQueries)
                        {
                            sb.Clear();
                            string tPath = subFolder + @"\" + folder + @"\" + f;

                            if (!Directory.Exists(tPath))
                            {
                                Directory.CreateDirectory(tPath);
                                if (f == "Results")
                                {
                                    string fName = "Get" + TableName + "Response.cs";
                                    sb.AppendLine(responseClass);
                                    string filePath = tPath + @"\" + fName;
                                    SaveFiles(sb, filePath, TableName, entityName);
                                }
                                else if (f == "Models")
                                {

                                    foreach (var item in filesName)
                                    {
                                        sb.Clear();
                                        string fName = item + ".cs";
                                        if (item == fName1)
                                        {
                                            sb.AppendLine(queryByIdClass);
                                        }
                                        else if (item == fName2)
                                        {
                                            sb.AppendLine(queryListClass);
                                        }
                                        else if (item == fName3)
                                        {
                                            sb.AppendLine(queryPaginationListClass);
                                        }
                                        string filePath = tPath + @"\" + fName;
                                        SaveFiles(sb, filePath, TableName, entityName);

                                    }
                                }
                                else if (f == "Handlers")
                                {
                                    sb.AppendLine(handlerQueryClass);
                                    string filePath = tPath + @"\" + TableName + "QueryHandler.cs";
                                    SaveFiles(sb, filePath, TableName, entityName);
                                }
                               
                            }


                        }

                    }
                    else if (folder == "Commands")
                    {
                        sb.Clear();
                        sbc.Clear();
                        foreach (var f in foldersCommands)
                        {
                            sb.Clear();
                            sbc.Clear();
                            string tPath = subFolder + @"\" + folder + @"\" + f;

                            if (!Directory.Exists(tPath))
                            {
                                Directory.CreateDirectory(tPath);
                                if (f == "Validators")
                                {
                                    foreach (var item in fValidators)
                                    {
                                        sb.Clear();
                                        string fName = item.Replace("{table}", TableName) + ".cs";
                                        if (item.StartsWith("Add"))
                                        {
                                            sb.AppendLine(addValidatorCommandClass);
                                            string filePath1 = tPath + @"\" + fName;

                                            SaveFiles(sb, filePath1, TableName, entityName);

                                        }
                                        else if (item.StartsWith("Edit"))
                                        {
                                            sb.AppendLine(editValidatorCommandClass);
                                            string filePath1 = tPath + @"\" + fName;
                                            SaveFiles(sb, filePath1, TableName, entityName);
                                        }
                                    }

                                }
                                else if (f == "Models")
                                {

                                    foreach (var item in fCommands)
                                    {
                                        sb.Clear();
                                        string fName = item.Replace("{table}", TableName) + ".cs";
                                        if (item.StartsWith("Add"))
                                        {
                                            sb.AppendLine(addModelCommandClass);
                                        }
                                        else if (item.StartsWith("Edit"))
                                        {
                                            sb.AppendLine(editModelCommandClass);
                                        }
                                        else if (item.StartsWith("Delete"))
                                        {
                                            sb.AppendLine(deleteModelCommandClass);
                                        }
                                        string filePath = tPath + @"\" + fName;
                                        SaveFiles(sb, filePath, TableName, entityName);

                                    }
                                }
                                else if (f == "Handlers")
                                {
                                    sb.AppendLine(handlerCommandClass);
                                    string filePath = tPath + @"\" + TableName + "CommandHandler.cs";
                                    SaveFiles(sb, filePath, TableName, entityName);
                                }
                            }


                        }

                    }

                }
                sb.Clear();
                string mapPath = mainPath + @"\" + "Mapping";
                if (!Directory.Exists(mapPath))
                {
                    Directory.CreateDirectory(mapPath);
                    string fMPath = mapPath + @"\" + Namespace;
                    if (!Directory.Exists(fMPath))
                    {
                        Directory.CreateDirectory(fMPath);
                    }
                    string mapFile = fMPath + @"\" + EName + "Profile.cs";

                    Dictionary<string, string> dic = new Dictionary<string, string>
                    {
                        { "Get" + TableName + "Response", "1" },
                        { Add + "Command", "2" },
                        { Edit + "Command", "2" }
                    };
                    //dic.Add(Edit + "Command", "2");
                    string mppClass = CreateMappingClass(SName, EName, entityName, dic);
                    sb.AppendLine(mppClass);

                    SaveFiles(sb, mapFile, TableName, entityName);
                }
                sb.Clear();

                string apiControllerPath = mainPath + @"\" + "apiControllers";

                if (!Directory.Exists(apiControllerPath))
                {
                    Directory.CreateDirectory(apiControllerPath);
                    string countFile = apiControllerPath + @"\" + EName + "Controller.cs";
                    string controllerClass = CreateApiController(EName, SName);
                    sb.AppendLine(controllerClass);
                    SaveFiles(sb, countFile, TableName, entityName);

                }
                sb.Clear();
                string routingPath = mainPath + @"\" + "apiRouting";
                if (!Directory.Exists(routingPath))
                {
                    Directory.CreateDirectory(routingPath);
                    sb.Clear();
                    string routerClass = AddRouter(EName);
                    sb.AppendLine(routerClass);
                    string routerFile = routingPath + @"\" + EName + "Routing.cs";
                    SaveFiles(sb, routerFile, TableName, entityName);
                }
                sb.Clear();
                string webControllerPath = mainPath + @"\" + "webControllers";
                if (!Directory.Exists(webControllerPath))
                {
                    Directory.CreateDirectory(webControllerPath);
                    string countFile = webControllerPath + @"\" + EName + "Controller.cs";
                    string controllerClass = CreateWebController(EName, SName, "Get" + TableName + "Response");
                    sb.AppendLine(controllerClass);
                    SaveFiles(sb, countFile, TableName, entityName);

                }
                string viewPath = mainPath + @"\" + "Views";

                sb.Clear();
                if (!Directory.Exists(viewPath))
                {
                    Directory.CreateDirectory(viewPath);
                    string vPath = viewPath + @"\" + EName;
                    if (!Directory.Exists(vPath))
                    {
                        Directory.CreateDirectory(vPath);
                        var index = CreateViews("Index", EName, SName, "Get" + TableName + "Response", schemaTable);
                        sb.Append(index);
                        string indexPath = vPath + @"\Index.cshtml";
                        SaveFiles(sb, indexPath, TableName, entityName);


                        var viewAll = CreateViews("_ViewAll", EName, SName, "Get" + TableName + "Response", schemaTable);
                        sb.Clear();
                        sb.Append(viewAll);
                        string viewAllPath = vPath + @"\_ViewAll.cshtml";
                        SaveFiles(sb, viewAllPath, TableName, entityName);

                        var addOrEdit = CreateViews("AddOrEdit", EName, SName, "Get" + TableName + "Response", schemaTable);
                        string addOrEditPath = vPath + @"\AddOrEdit.cshtml";
                        sb.Clear();
                        sb.Append(addOrEdit);
                        SaveFiles(sb, addOrEditPath, TableName, entityName);

                        var delete = CreateViews("Delete", EName, SName, "Get" + TableName + "Response", schemaTable);
                        string deletePath = vPath + @"\Delete.cshtml";
                        sb.Clear();
                        sb.Append(delete);
                        SaveFiles(sb, deletePath, TableName, entityName);

                        var details = CreateViews("Details", EName, SName, "Get" + TableName + "Response", schemaTable);
                        string detailsPath = vPath + @"\Details.cshtml";
                        sb.Clear();
                        sb.Append(details);
                        SaveFiles(sb, detailsPath, TableName, entityName);

                    }


                }
            }




        }
        private string CreateResponseClass(DataTable schemaTable)
        {
            string _params = "namespace Core.Features." + SName + ".Queries.Results\r\n" +
                "{\r\n" + "public class Get" + EName + "Response \r\n" +
                "{\n";
            _params += GetProperties(schemaTable, true);
            return _params + "}\r\n}";
        }
        private static string GetProperties(DataTable schemaTable, bool SkipEn = false, bool SkipKey = false)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataRow _row in schemaTable.Rows)
            {
                string _columnName = _row["Column_name"].ToString();
                string _type = _row["Type"].ToString().ToLower();
                string NULLABLE = _row["ISNULLABLE"].ToString();
                string isKey = _row["IsKey"].ToString();
                _ = bool.TryParse(isKey, out bool IsKey);
                _ = bool.TryParse(NULLABLE, out bool isNull);
                string _codeType;
                switch (_type)
                {
                    case "varchar":
                    case "nvarchar":
                    case "char":
                    case "nchar":
                        {
                            _codeType = "string" + (isNull ? "?" : "");
                            break;
                        }
                    case "bit":
                        {
                            _codeType = "bool" + (isNull ? "?" : "");
                            break;
                        }
                    case "bigint":
                        {
                            _codeType = "long" + (isNull ? "?" : "");
                            break;
                        }
                    case "money":
                        {
                            _codeType = "decimal" + (isNull ? "?" : "");
                            break;
                        }
                    case "datetime":
                    case "datetime2":
                        {
                            _codeType = "DateTime" + (isNull ? "?" : "");
                            break;
                        }

                    default:
                        {
                            _codeType = _type + (isNull ? "?" : "");
                            break;
                        }
                }

                string colName = _columnName;
                if (SkipKey)
                {
                    if (!IsKey)
                    {
                        if (SkipEn)
                        {
                            colName = _columnName.EndsWith("Ar") ? _columnName[..^2] : _columnName;
                            if (!_columnName.EndsWith("En"))
                            {
                                sb.AppendLine("public " + _codeType + " " + colName + "{get;set;}");
                            }

                        }
                        else
                        {
                            sb.AppendLine("public " + _codeType + " " + colName + "{get;set;}");
                        }

                    }
                }
                else
                {
                    if (SkipEn)
                    {
                        colName = _columnName.EndsWith("Ar") ? _columnName[..^2] : _columnName;
                        if (!_columnName.EndsWith("En"))
                        {
                            sb.AppendLine("public " + _codeType + " " + colName + "{get;set;}");
                        }

                    }
                    else
                    {
                        sb.AppendLine("public " + _codeType + " " + colName + "{get;set;}");
                    }

                }
            }
            return sb.ToString();
        }
        private string CreateSingleQueryClass(string TableName)
        {
            DataTable KeyCol = GetPrimaryKeyColumn(GlobalConString);

            string Key = "Id";
            string KeyType = "int ";
            if (KeyCol.Rows.Count > 0)
            {
                Key = KeyCol.Rows[0]["ColumnName"].ToString();
                KeyType = KeyCol.Rows[0]["DATA_TYPE"].ToString();
            }
            string key = Key.ToLower();

            string txt = "using MediatR;\n" +
                "using Core.Bases;\n" +
                "using Core.Features." + SName + ".Queries.Results;\n" +
                "namespace Core.Features." + SName + ".Queries.Models\r\n" +
                "{\r\n" +
                "public class Get{table}ByIdQuery : EntityBase, IRequest<Response<Get{table}Response>>\n" +
                "{\n" +
                "public " + KeyType + " " + Key + "{ get; set; }\n" +
                "public Get{table}ByIdQuery(" + KeyType + " " + key + ")\n" +
                "{\n" +
                "" + Key + "=" + key + ";\n" +
                "}\n" +
                "}\n" +
                "}\n";
            string result = txt.Replace("{table}", TableName);
            return result;
        }
        private string CreateListQueryClass(string TableName, string className)
        {
            string txt = "using MediatR;\n" +
                "using Core.Bases;\n" +
                "using Core.Features." + SName + ".Queries.Results;\n" +
                "namespace Core.Features." + SName + ".Queries.Models\n" +
                "{\n" +
                "public class Get{table}{query} :  EntityBase, IRequest<Response<List<Get{table}Response>>>\n" +
                "{\n" +

                "}\n" +
                "}\n";
            string result = txt.Replace("{table}", TableName).Replace("{query}", className);
            return result;
        }
        private string CreatePaginationListQueryClass(string TableName, string className)
        {
            string txt = "using MediatR;\n" +
                "using Core.Wrappers;\n" +
                "using Data.Helpers;\n" +
                "using Core.Features." + SName + ".Queries.Results;\n" +
                "namespace Core.Features." + SName + ".Queries.Models\n" +
                "{\n" +
                "public class Get{table}{query} :  Pager, IRequest<PaginatedResult<Get{table}Response>>\n" +
                "{\n" +

                "}\n" +
                "}\n";
            string result = txt.Replace("{table}", TableName).Replace("{query}", className);
            return result;
        }

        private string CreateHandlerQueryClass(string TableName,string entityName)
        {
            string Key = "Id";
            foreach (DataRow row in Table.Rows)
            {
                string isKey = row["IsKey"].ToString();
                _ = bool.TryParse(isKey, out bool IsKey);
                if (IsKey)
                {
                    Key = row["Column_name"].ToString();
                    string KeyType = row["Type"].ToString();
                    break;
                }


            }

            _ = Key.ToLower();
            string className = "using AutoMapper;\r\n" +
                "using Core.Bases;\r\n" +
                "using Core.Features." + SName + ".Queries.Models;\r\n" +
                "using Core.Features." + SName + ".Queries.Results;\r\n" +
                "using Core.Resources;\r\n" +
                "using Core.Wrappers;\r\n" +
                "using Data.Entities;\r\n" +
                "using MediatR;\r\n" +
                "using Microsoft.Extensions.Localization;\r\n" +
                "using Service.ServiceBases;" +
                "namespace Core.Features." + SName + ".Queries.Handlers\n" +
                "{\n" +
                "public class {table}QueryHandler : ResponseHandler,\n" +
                "IRequestHandler<Get{table}ListQuery, Response<List<Get{table}Response>>>,\n" +
                "IRequestHandler<Get{table}ByIdQuery, Response<Get{table}Response>>,\n" +
                "IRequestHandler<Get{table}PaginatedListQuery, PaginatedResult<Get{table}Response>>\r\n" +
                "{\r\n" +
                "#region Fields\n" +
                "private readonly IGenericServiceAsync<{entity}> _service;\n" +
                "private readonly IMapper _mapper;\n" +
                "private readonly IStringLocalizer<SharedResources> _stringLocalizer;\n" +
                "#endregion\n" +
                "#region Constructors\n" +
                "public {table}QueryHandler(IStringLocalizer<SharedResources> stringLocalizer,\n" +
                "IMapper mapper, IGenericServiceAsync<{entity}> service) : base(stringLocalizer)\n" +
                "{\n" +
                "_mapper = mapper;\r\n" +
                "_service = service;\r\n" +
                "_stringLocalizer = stringLocalizer;\r\n" +
                "}\n" +
                "#endregion\r\n" +
                "#region Handle Functions\r\n" +
                "public async Task<Response<List<Get{table}Response>>> Handle(Get{table}ListQuery request, CancellationToken cancellationToken)\r\n" +
                "{\r\n" +
                "var resultList = await _service.GetListAsync();\r\n" +
                "var resultWhere = resultList.ToList();\r\n" +
                "var listMapper = _mapper.Map<List<Get{table}Response>>(resultWhere);\r\n" +
                "var result = Success(listMapper);\r\n" +
               
                "return result;\r\n" +
                "}\r\n" +
                "public async Task<Response<Get{table}Response>> Handle(Get{table}ByIdQuery request, CancellationToken cancellationToken)\r\n" +
                "{\r\n" +
                "var resultRow = await _service.GetByIdAsync(request." + Key + ");\r\n" +
                "if (resultRow == null) return NotFound<Get{table}Response>(_stringLocalizer[SharedResourcesKeys.NotFound]);\r\n" +
                "var resultMapper = _mapper.Map<Get{table}Response>(resultRow);\r\n" +
                "var result = Success(resultMapper);\r\n" +
                
                "return result;" +
                "}\r\n" +
                "public async Task<PaginatedResult<Get{table}Response>> Handle(Get{table}PaginatedListQuery request, CancellationToken cancellationToken)\r\n" +
                "{\r\n" +
                "var FilterQuery = _service.FilterPaginatedQueryable(request.OrderBy, request.Search??string.Empty);\r\n" +
                "var resultWhere = FilterQuery;\r\n" +
                "var PaginatedList = await _mapper.ProjectTo<Get{table}Response>(resultWhere).ToPaginatedListAsync(request.PageNumber, request.PageSize);\r\n" +
                
                "return PaginatedList;\r\n" +
                "}\r\n" +
                "#endregion\r\n" +
                "}\r\n" +
                "}";

            return className.Replace("{table}", TableName).Replace("{entity}", entityName);
        }

        private string CreateAddEditCommandClass(DataTable schemaTable, string TableName, string className, string op)
        {
            string _params;
            string Key = "Id";
            string KeyType = "int ";
            if (schemaTable.Rows.Count > 0)
            {
                foreach (DataRow row in schemaTable.Rows)
                {
                    string isKey = row["IsKey"].ToString();
                    _ = bool.TryParse(isKey, out bool IsKey);
                    if (IsKey)
                    {
                        Key = row["Column_name"].ToString();
                        KeyType = row["Type"].ToString();
                        break;
                    }


                }

            }
            string key = Key.ToLower();
            if (op == "Delete")
            {
                _params = "using Core.Bases;\r\n" +
                "using MediatR;\r\n" +
                "namespace Core.Features." + SName + ".Commands.Models\r\n" +
                "{\r\n" +
                "public class " + className + " : EntityBase, IRequest<Response<string>>\r\n" +
                "{\r\n" +
                "public " + KeyType + " " + Key + "{ get; set; }\r\n" +
                "public " + className + "(" + KeyType + " " + key + ")\r\n" +
                "{\r\n" +
                "" + Key + "=" + key + ";\r\n" +
                "}\r\n" +
                "";

            }
            else
            {
                _params = "using Core.Bases;\r\n" +
                "using MediatR;\r\n" +
                "namespace Core.Features." + SName + ".Commands.Models\r\n" +
                "{\r\n" +
                "public class " + className + " : EntityBase, IRequest<Response<string>>\r\n" +
                "{\r\n" +
                "";
                _params += GetProperties(schemaTable, SkipKey: op == "Add");
            }
            return _params.Replace("{table}", TableName) + "}\r\n}";

        }

        private string CreateHandlerCommandClass(string TableName,string entityName)
        {

            string className = "using AutoMapper;\r\n" +
                "using Core.Bases;\r\n" +
                "using Core.Features." + SName + ".Commands.Models;\r\n" +
                "using Core.Resources;\r\n" +
                "using Data.Entities;\r\n" +
                "using MediatR;\r\n" +
                "using Microsoft.Extensions.Localization;\r\n" +
                "using Service.ServiceBases;\r\n" +
                "namespace Core.Features." + SName + ".Commands.Handlers\r\n" +
                "{\r\n" +
                "" +
                "public class {table}CommandHandler : ResponseHandler,\r\n" +
                "IRequestHandler<Add{table}Command, Response<string>>,\r\n" +
                "IRequestHandler<Edit{table}Command, Response<string>>,\r\n" +
                "IRequestHandler<Delete{table}Command, Response<string>>\r\n" +
                "{\r\n" +
                "#region Fields\r\n" +
                "private readonly IGenericServiceAsync<{entity}> _service;\r\n" +
                "private readonly IMapper _mapper;\r\n" +
                "private readonly IStringLocalizer<SharedResources> _localizer;\r\n" +
                "#endregion\r\n" +
                "#region Constructors\r\n" +
                "public {table}CommandHandler(IGenericServiceAsync<{entity}> service, IMapper mapper,\r\n" +

                "IStringLocalizer<SharedResources> localizer) : base(localizer)\r\n" +
                "{\r\n" +
                "_service = service;\r\n" +
                "_mapper = mapper;\r\n" +

                "_localizer = localizer;\r\n" +
                "}\r\n" +
                "#endregion\r\n" +
                "#region Handle Functions\r\n\r\n" +
                "public async Task<Response<string>> Handle(Add{table}Command request, CancellationToken cancellationToken)\r\n" +
                "{\r\n//mapping Between request and {entity}\r\n" +
                "var requestMapper = _mapper.Map<{entity}>(request);\r\n" +
                "//add\r\n" +
                "var user = request.AddUserId;\r\n" +
                "requestMapper.AddUserId = user;\r\n" +
                "requestMapper.DateCreated = DateTime.Now;\r\n\r\n" +
                "var result = await _service.AddNewAsync(requestMapper);\r\n" +
                "//return response\r\n" +
                "if (result == \"Success\") return Created(\"\");\r\n" +
                "else return BadRequest<string>();\r\n" +
                "}\r\n\r\n" +
                "public async Task<Response<string>> Handle(Edit{table}Command request, CancellationToken cancellationToken)\r\n        {\r\n" +
                "//Check if the Id is Exist Or not\r\n" +
                "var resultRow = await _service.GetByIdAsync(request.Id);\r\n" +
                "//return {entity} NotFound\r\n" +
                "if (resultRow == null) return NotFound<string>();\r\n" +
                "//mapping Between request and {entity}\r\n" +
                "var resultMapper = _mapper.Map(request, resultRow);\r\n" +
                "//Call service that make Edit\r\n" +
                "var user = request.AddUserId;\r\n" +
                "resultMapper.UpdateUserId = user;\r\n" +
                "resultMapper.DateUpdated = DateTime.Now;\r\n" +
                "var result = await _service.EditAsync(resultMapper);\r\n" +
                "//return response\r\n" +
                "if (result == \"Success\") return Success((string)_localizer[SharedResourcesKeys.Updated]);\r\n" +
                "else return BadRequest<string>();\r\n" +
                "}\r\n\r\n" +
                "public async Task<Response<string>> Handle(Delete{table}Command request, CancellationToken cancellationToken)\r\n" +
                "{\r\n" +
                "//Check if the Id is Exist Or not\r\n" +
                "var resultRow = await _service.GetByIdAsync(request.Id);\r\n" +
                "//{table} NotFound\r\n" +
                "if (resultRow == null) return NotFound<string>();\r\n" +
                "//Call service that make Delete\r\n" +
                "var result = await _service.RemoveAsync(resultRow);\r\n" +
                "if (result == \"Success\") return Deleted<string>();\r\n" +
                "else return BadRequest<string>();\r\n" +
                "}\r\n" +
                "#endregion\r\n" +
                " }\r\n" +
                "}\r\n";

            return className.Replace("{table}", TableName).Replace("{entity}", entityName);
        }

        private string CreateValidatorsCommandClass(string tableName, string op)
        {

            string className = "";
            if (op == "Add")
            {
                className = "using Core.Features." + SName + ".Commands.Models;\r\n" +
                "using Core.Resources;\r\n" +
                "using FluentValidation;\r\n" +
                "using Microsoft.Extensions.Localization;\r\n" +
                "namespace Core.Features." + SName + ".Commands.Validators\r\n" +
                "{\r\n" +
                " public class {op}{table}Validator : AbstractValidator<{op}{table}Command>\r\n" +
                "{\r\n" +
                "#region Fields\r\n" +
                "private readonly IStringLocalizer<SharedResources> _localizer;\r\n" +
                "#endregion\r\n" +
                "#region Constructors\r\n" +
                "public {op}{table}Validator(IStringLocalizer<SharedResources> localizer)\r\n" +
                "{\r\n" +
                "// _service = service;\r\n" +
                "_localizer = localizer;\r\n" +
                "ApplyValidationsRules();\r\n" +
                "// ApplyCustomValidationsRules();\r\n" +
                "// _departmentService = departmentService;\r\n" +
                "}\r\n" +
                "#endregion\r\n" +
                "#region Actions\r\n" +
                    "public void ApplyValidationsRules()\r\n" +
                    "{\r\n" +
                    "/*RuleFor(x => x.NewsTitleAr)\r\n" +
                    ".NotEmpty().WithMessage(_localizer[SharedResourcesKeys.NotEmpty])\r\n" +
                    ".NotNull().WithMessage(_localizer[SharedResourcesKeys.Required])\r\n" +
                    ".MaximumLength(100).WithMessage(_localizer[SharedResourcesKeys.MaxLengthis100]);\r\n\r\n" +
                    "RuleFor(x => x.NewsContentAr)\r\n" +
                    ".NotEmpty().WithMessage(_localizer[SharedResourcesKeys.NotEmpty])\r\n" +
                    ".NotNull().WithMessage(_localizer[SharedResourcesKeys.Required])\r\n" +
                    ".MaximumLength(100).WithMessage(_localizer[SharedResourcesKeys.MaxLengthis100]);\r\n\r\n" +
                    "*/\r\n" +
                    "}\r\n\r\n\r\n" +
                    "#endregion\r\n" +
                    "}\r\n" +
                    "}\r\n";
            }
            else if (op == "Edit")
            {
                className = "using Core.Features." + SName + ".Commands.Models;\r\n" +
                    "using Core.Resources;\r\n" +
                    "using FluentValidation;\r\n" +
                    "using Microsoft.Extensions.Localization;\r\n\r\n" +
                    "namespace Core.Features." + SName + ".Commands.Validators\r\n{" +
                    "\r\n" +
                    "public class {op}{table}Validator : AbstractValidator<{op}{table}Command>\r\n" +
                    "{\r\n" +
                    "#region Fields\r\n" +
                    "//private readonly I{cName}Service _service;\r\n" +
                    "private readonly IStringLocalizer<SharedResources> _localizer;\r\n" +
                    "//private readonly IDepartmentService _departmentService;\r\n" +
                    "#endregion\r\n\r\n" +
                    "#region Constructors\r\n" +
                    "public {op}{table}Validator(IStringLocalizer<SharedResources> localizer)\r\n" +
                    "{\r\n" +
                    "// _service = service;\r\n" +
                    "_localizer = localizer;\r\n" +
                    "ApplyValidationsRules();\r\n" +
                    "// ApplyCustomValidationsRules();\r\n" +
                    "// _departmentService = departmentService;\r\n" +
                    "}\r\n" +
                    "#endregion\r\n\r\n" +
                    "#region Actions\r\n" +
                    "public void ApplyValidationsRules()\r\n" +
                    "{\r\n" +
                     "/*RuleFor(x => x.NewsTitleAr)\r\n" +
                    ".NotEmpty().WithMessage(_localizer[SharedResourcesKeys.NotEmpty])\r\n" +
                    ".NotNull().WithMessage(_localizer[SharedResourcesKeys.Required])\r\n" +
                    ".MaximumLength(100).WithMessage(_localizer[SharedResourcesKeys.MaxLengthis100]);\r\n\r\n" +
                    "RuleFor(x => x.NewsContentAr)\r\n" +
                    ".NotEmpty().WithMessage(_localizer[SharedResourcesKeys.NotEmpty])\r\n" +
                    ".NotNull().WithMessage(_localizer[SharedResourcesKeys.Required])\r\n" +
                    ".MaximumLength(100).WithMessage(_localizer[SharedResourcesKeys.MaxLengthis100]);\r\n\r\n" +
                    "*/\r\n" +
                    "}\r\n\r\n\r\n" +
                    "#endregion\r\n\r\n" +
                    "}\r\n" +
                    "}";
            }

            return className.Replace("{table}", tableName).Replace("{op}", op);
        }


        private static string CreateMappingClass(string spName, string cName,string entityName, Dictionary<string, string> dic)
        {
            string className = "using AutoMapper;\r\n" +
                "using Core.Features." + spName + ".Queries.Results;\r\n" +
                "using Core.Features." + spName + ".Commands.Models;\r\n" +
                "using Data.Entities;\r\n" +
                "namespace Core.Mapping." + spName + "\r\n" +
                "{\r\n" +
                "public class " + cName + "Profile : Profile\r\n" +
                "{\r\n" +
                "public " + cName + "Profile()\r\n" +
                "{\r\n" +
                " ";
            foreach (var item in dic)
            {
                if (item.Value == "1")
                {
                    className += $"CreateMap<{entityName}, " + item.Key + ">().ReverseMap();\r\n";

                }
                else if (item.Value == "2")
                {
                    className += $"CreateMap<{item.Key},{entityName}>().ReverseMap();\r\n";
                }
            }
            className += "}\r\n" +
                "\r\n}" +
                "}\r\n";
            return className.Replace("{entity}", entityName);
        }
        private static void SaveFiles(StringBuilder sb, string filePath,string cName,string entityName)
        {

            filePath = filePath.Replace("{entity}", entityName).Replace("{table}", cName).Replace("{op}", "");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            sb = sb.Replace("{entity}", entityName).Replace("{table}", cName).Replace("{op}", "");
            using StreamWriter w = new StreamWriter(filePath, true);
            string[] deLines = { Environment.NewLine, "\n" }; // "\n" added in case you manually appended a newline
            string[] lines = sb.ToString().Split(deLines, StringSplitOptions.None);
            foreach (string item in lines)
            {
                var txtLine = item.Replace("{entity}", entityName).Replace("{table}", cName).Replace("{op}", "");
                w.WriteLine(item);

            }

            // File.WriteAllText(fileName, code);
        }

        private static string CreateApiController(string ClassName, string spName)
        {

            string controllerName = "using Core.Features.{spName}.Commands.Models;\r\n" +
                "using Core.Features.{spName}.Queries.Models;\r\n" +
                "using Microsoft.AspNetCore.Authorization;\r\n" +
                "using Data.AppMetaData;\r\n" +
                "using Microsoft.AspNetCore.Mvc;\r\n" +
                "using NGWebAPI.Controllers.Base;\r\n" +
                "namespace NGWebAPI.Controllers\r\n" +
                "{\r\n" +
                " [ApiController]\r\n" +
                " public class {cName}Controller : AppControllerBase\r\n" +
                "{\r\n" +
                "[HttpGet(Router.{cName}Routing.List)]\r\n" +
                "[AllowAnonymous]\r\n" +
                "public async Task<IActionResult> Get{cName}List()\r\n" +
                "{\r\n" +
                "var response = await Mediator.Send(new Get{cName}ListQuery());\r\n" +

                "return Ok(response);\r\n" +
                "}\r\n" +
                "[AllowAnonymous]\r\n" +
                "[HttpGet(Router.{cName}Routing.Paginated)]\r\n" +
                "public async Task<IActionResult> Paginated([FromQuery] Get{cName}PaginatedListQuery query)\r\n" +
                "{\r\n" +
                "\r\n" +
                "var response = await Mediator.Send(query);\r\n" +
                "return Ok(response);\r\n" +
                "}\r\n" +
                "[HttpGet(Router.{cName}Routing.GetByID)]\r\n" +
                "public async Task<IActionResult> Get{cName}ByID([FromRoute] int id)\r\n" +
                "{\r\n" +
                "return NewResult(await Mediator.Send(new Get{cName}ByIdQuery(id)));\r\n" +
                "}\r\n" +
                "//[Authorize(Policy = \"Create{cName}\")]\r\n" +
                "[HttpPost(Router.{cName}Routing.Create)]\r\n" +
                "public async Task<IActionResult> Create([FromBody] Add{cName}Command command)\r\n" +
                "{\r\n" +
                "\r\n" +
                "var response = await Mediator.Send(command);\r\n" +
                "return NewResult(response);\r\n" +
                "}\r\n" +
                "//[Authorize(Policy = \"Edit{cName}\")]\r\n" +
                "[HttpPut(Router.{cName}Routing.Edit)]\r\n" +
                "public async Task<IActionResult> Edit([FromBody] Edit{cName}Command command)\r\n" +
                "{\r\n" +
                "\r\n" +
                "var response = await Mediator.Send(command);\r\n" +
                "return NewResult(response);\r\n" +
                "}\r\n" +
                "//[Authorize(Policy = \"Delete{cName}\")]\r\n" +
                "[HttpDelete(Router.{cName}Routing.Delete)]\r\n" +
                "public async Task<IActionResult> Delete([FromRoute] int id)\r\n" +
                "{\r\n" +
                "return NewResult(await Mediator.Send(new Delete{cName}Command(id)));\r\n" +
                "}\r\n" +
                "}\r\n" +
                "}";

            return controllerName.Replace("{cName}", ClassName).Replace("{spName}", spName);
        }
        private static string CreateWebController(string ClassName, string spName, string rName)
        {

            string controllerName = "using Core.Features.{spName}.Queries.Results;\r\n" +
                "using Microsoft.AspNetCore.Mvc;\r\n" +
                "using Newtonsoft.Json;\r\n" +
                "using ngWebApp.Contracts;\r\n" +
                "using ngWebApp.Controllers.Base;\r\n" +
                "using ngWebApp.Helpers;" +
                "namespace ngWebApp.Controllers\r\n{\r\n" +
                "public class {cName}Controller : AppController\r\n" +
                "{\r\n" +

                "private readonly IRestClient<string> _commandService;\r\n" +
                "private readonly IRestClient<{rName}> _queryService;\r\n\r\n" +

                "public {cName}Controller(IRestClient<string> commandService,\r\n" +
                "IRestClient<{rName}> queryService)\r\n" +
                "{\r\n" +
                "\r\n" +
                "            _commandService = commandService;\r\n" +
                "            _queryService = queryService;\r\n" +
                "        }\r\n\r\n" +

                "        public async Task<IActionResult> Index()\r\n" +
                "        {\r\n" +
                "            var resultList = await GetList();\r\n" +
                "            return View(resultList);\r\n" +
                "        }\r\n" +

                "        [HttpGet]\r\n" +
                "        public async Task<IActionResult> AddOrEdit(int id = 0)\r\n" +
                "        {\r\n" +
                "            var model = new {rName}();\r\n" +
                "            if (id == 0)\r\n" +
                "                return View(model);\r\n" +
                "            else\r\n" +
                "            {\r\n" +
                "                var url = \"{cName}/\" + id;\r\n" +
                "                var result = await _queryService.GetAsyncObject(\"{cName}/List\");\r\n" +
                "                if (result is not null)\r\n" +
                "                {\r\n" +
                "                    if (result.Data is not null)\r\n" +
                "                    {\r\n" +
                "                        var json = JsonConvert.SerializeObject(result!.Data, Formatting.Indented,\r\n" +
                "                            new JsonSerializerSettings\r\n" +
                "                            {\r\n" +
                "                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,\r\n " +
                "                               PreserveReferencesHandling = PreserveReferencesHandling.Objects\r\n" +
                "                            });\r\n" +
                "                        var resultRow = JsonConvert.DeserializeObject<{rName}>(json);\r\n" +
                "                        if (resultRow != null)\r\n" +
                "                        {\r\n" +
                "                            return View(resultRow);\r\n" +
                "                        }\r\n" +
                "                    }\r\n" +
                "                    string errors = string.Join(\",\", result!.Errors);\r\n" +
                "                    ModelState.AddModelError(result.Message, errors);\r\n" +
                "                    return View(model);\r\n" +
                "                }\r\n" +
                "                return View(model);\r\n" +
                "            }\r\n\r\n" +
                "        }\r\n\r\n" +
                "        [HttpPost]\r\n" +
                "        [ValidateAntiForgeryToken]\r\n" +
                "        public async Task<IActionResult> AddOrEdit({rName} model)\r\n" +
                "        {\r\n" +
                "            if (ModelState.IsValid)\r\n" +
                "            {\r\n" +
                "                //Insert\r\n" +
                "                try\r\n" +
                "                {\r\n" +
                "                    var json = JsonConvert.SerializeObject(model);\r\n\r\n" +
                "                    var result =await _commandService.PostAsyncVar(\"{cName}/Create\", json);\r\n" +
                "                    if (result.Succeeded)\r\n" +
                "                    {\r\n" +
                "                        return View(nameof(Index));\r\n" +
                "                    }\r\n" +
                "                    else\r\n" +
                "                    {\r\n" +
                "                        return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, \"AddOrEdit\", model) });\r\n\r\n" +
                "                    }\r\n                    //var resultList = await GetList();\r\n\r\n" +
                "                }\r\n                catch\r\n" +
                "                {\r\n                    return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, \"AddOrEdit\", model) });\r\n" +
                "" +
                "                }\r\n\r\n            }\r\n" +
                "            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, \"AddOrEdit\", model) });\r\n" +
                "        }\r\n\r\n" +
                "        private async Task<List<{rName}>> GetList()\r\n" +
                "        {\r\n" +
                "            var model = new List<{rName}>();\r\n" +
                "            var result = await _queryService.GetAsyncList(\"{cName}/List\");\r\n" +
                "            if (result != null)\r\n" +
                "            {\r\n" +
                "                return result.Data;\r\n" +
                "            }\r\n" +
                "            return model;\r\n" +
                "        }\r\n" +
                "        public async Task<IActionResult> Delete(int id)\r\n" +
                "        {\r\n" +
                "            if (id == 0)\r\n" +
                "            {\r\n" +
                "                return NotFound();\r\n" +
                "            }\r\n" +
                "            var result = await _queryService.GetAsyncList(\"{cName}/\" + id);\r\n" +
                "            if (result==null)\r\n" +
                "            {\r\n" +
                "                return NotFound();\r\n" +
                "            }\r\n\r\n" +
                "            return View(result);\r\n" +
                "        }\r\n" +
                "        [HttpPost, ActionName(\"Delete\")]\r\n" +
                "        [ValidateAntiForgeryToken]\r\n" +
                "        public async Task<IActionResult> DeleteConfirmed(int id)\r\n" +
                "        {\r\n" +
                "            var url = \"{cName}/\" + id;\r\n\r\n" +
                "            var request = await _commandService.DeleteAsync(url, id);\r\n" +
                "            if (request != null)\r\n" +
                "            {\r\n" +
                "                var result = await GetList();\r\n" +
                "                return Json(new { html = Helper.RenderRazorViewToString(this, \"_ViewAll\", result) });\r\n\r\n" +
                "            }\r\n" +
                "            return View(nameof(Index));\r\n" +
                "        }\r\n" +
                "        public async Task<bool> IsExists(int id)\r\n" +
                "        {\r\n\r\n" +
                "            var result = await _queryService.GetAsyncList(\"{cName}/\" + id);\r\n" +
                "            if (result != null)\r\n" +
                "            {\r\n" +
                "                var json = JsonConvert.SerializeObject(result.Data);\r\n" +
                "                var resultList = JsonConvert.DeserializeObject<{rName}>(json);\r\n" +
                "                if (resultList != null)\r\n" +
                "                {\r\n" +
                "                    return true;\r\n" +
                "                }\r\n" +
                "            }\r\n            return false;\r\n" +
                "        }\r\n\r\n" +
                "    }\r\n" +
                "}\r\n";

            return controllerName.Replace("{cName}", ClassName).Replace("{spName}", spName).Replace("{rName}", rName);
        }
        private static string AddRouter(string className)
        {
            string router = "\r\n\r\n\r\n" +
                "public static class {cName}Routing\r\n" +
                "{\r\n" +
                "public const string Prefix = Rule + \"{cName}\";\r\n" +
                "public const string List = Prefix + \"/List\";\r\n" +
                "public const string GetByID = Prefix + SignleRoute;\r\n" +
                "public const string Create = Prefix + \"/Create\";\r\n" +
                "public const string Edit = Prefix + \"/Edit\";\r\n" +
                "public const string Delete = Prefix + \"/{id}\";\r\n" +
                "public const string Paginated = Prefix + \"/Paginated\";\r\n\r\n" +
                "}\r\n";
            return router.Replace("{cName}", className);
        }

        private static string CreateViews(string vName, string cName, string spName, string rName, DataTable schemaTable)
        {
            string Key = "Id";
            foreach (DataRow row in schemaTable.Rows)
            {
                string isKey = row["IsKey"].ToString();
                _ = bool.TryParse(isKey, out bool IsKey);
                if (IsKey)
                {
                    Key = row["Column_name"].ToString();
                    string KeyType = row["Type"].ToString();
                    break;
                }


            }
            string view = "";
            if (vName == "Index")
            {
                view = "@using Core.Features.{spName}.Queries.Results\r\n" +
                    "@model IEnumerable<{rName}>\r\n\r\n" +
                    "@{\r\n" +
                    "ViewData[\"Title\"] = \"Home\";\r\n}\r\n\r\n" +
                    "<div id=\"view-all\">\r\n" +
                    "    @await Html.PartialAsync(\"_ViewAll\", Model)\r\n" +
                    "</div>";


            }
            else if (vName == "_ViewAll")
            {
                StringBuilder sbCols = new StringBuilder();
                StringBuilder sbRows = new StringBuilder();
                StringBuilder sbView = new StringBuilder();
                view = "@using Core.Features.{spName}.Queries.Results\r\n" +
                    "@model IEnumerable<{rName}>\r\n\r\n" +
                    "@{\r\n" +
                    "    ViewData[\"Title\"] = \"_ViewAllStruct\";\r\n" +
                    "    Layout = null;\r\n" +
                    "}\r\n\r\n\r\n" +
                    "<p>\r\n" +
                    "    <a onclick=\"showInPopup('@Url.Action(\"AddOrEdit\",\"{cName}\",null,Context.Request.Scheme)','اضافة')\"\r\n" +
                    "       class=\"btn btn-success text-white\"><i class=\"fas fa-random\"></i> اضافة </a>\r\n" +
                    "</p>\r\n\r\n\r\n" +
                    "\r\n\r\n\r\n" +
                    "<table class=\"table\">\r\n";
                sbView.AppendLine(view);

                sbCols.AppendLine(" <thead>\r\n        <tr>");
                sbRows.AppendLine("<tbody>");
                sbRows.AppendLine("@foreach (var item in Model)");
                sbRows.AppendLine("{<tr>");

                foreach (DataRow _row in schemaTable.Rows)
                {
                    string _columnName = _row["Column_name"].ToString();
                    string colName = _columnName.EndsWith("Ar") ? _columnName[..^2] : _columnName;

                    if (!_columnName.EndsWith("En"))
                    {
                        sbCols.AppendLine("<th>\r\n" +
                    "@Html.DisplayNameFor(model => model." + colName + ")\r\n" +
                    "            </th>");
                        sbRows.AppendLine("<td>\r\n" +
                            "                    @Html.DisplayFor(modelItem => item." + colName + ")\r\n" +
                            "                </td>");
                    }
                }
                sbCols.AppendLine(" <th></th>\r\n" +
                    "        </tr>\r\n" +
                    "    </thead>");
                sbRows.AppendLine("<td>\r\n" +
                    "                    <div>\r\n" +
                    "                        <a onclick=\"showInPopup('@Url.Action(\"AddOrEdit\",\"{cName}\",new {id=item." + Key + "},Context.Request.Scheme)','تعديل ')\" " +
                    " class=\"btn btn-info text-white\"><i class=\"fas fa-pencil-alt\"></i> تعديل</a>\r\n" +
                    "                        <form asp-action=\"Delete\" asp-route-id=\"@item." + Key + "\" onsubmit=\"return jQueryAjaxDelete(this)\" class=\"d-inline\">\r\n" +
                    "                            <input type=\"hidden\" asp-for=\"@item." + Key + "\" />\r\n" +
                    "                            <input type=\"submit\" value=\"Delete\" class=\"btn btn-danger\" />\r\n" +
                    "                        </form>\r\n" +
                    "                    </div>\r\n\r\n" +
                    "                    \r\n" +
                    "                </td>");
                sbRows.AppendLine("</tr>");
                sbRows.AppendLine("}");
                sbRows.AppendLine("</tbody>");
                sbRows.AppendLine("</table>\r\n");
                sbView.AppendLine(sbCols.ToString());
                sbView.AppendLine(sbRows.ToString());
                view = sbView.ToString();
            }

            else if (vName == "AddOrEdit")
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("@model Core.Features.{spName}.Queries.Results.{rName}\r\n");
                sb.AppendLine("@{\r\n" +
                    "    ViewData[\"Title\"] = \"AddOrEdit\";\r\n" +
                    "    Layout = null;\r\n" +
                    "}\r\n");

                sb.AppendLine("<h1>AddOrEdit</h1>\r\n\r\n<h4>{rName}</h4>\r\n<hr />");
                sb.AppendLine("<div class=\"row\">\r\n" +
                    "    <div class=\"col-md-4\">");
                sb.AppendLine("<form asp-action=\"AddOrEdit\">");
                sb.AppendLine("<div asp-validation-summary=\"ModelOnly\" class=\"text-danger\"></div>");
                foreach (DataRow _row in schemaTable.Rows)
                {
                    string _columnName = _row["Column_name"].ToString();
                    string colName = _columnName.EndsWith("Ar") ? _columnName[..^2] : _columnName;
                    if (!_columnName.EndsWith("En"))
                    {
                        sb.AppendLine("<div class=\"form-group\">\r\n" +
                "                <label asp-for=\"" + colName + "\" class=\"control-label\"></label>\r\n" +
                "                <input asp-for=\"" + colName + "\" class=\"form-control\" />\r\n" +
                "                <span asp-validation-for=\"" + colName + "\" class=\"text-danger\"></span>\r\n" +
                "            </div>");
                    }
                }
                sb.AppendLine("<div class=\"form-group\">\r\n" +
                    "                <input type=\"submit\" value=\"Create\" class=\"btn btn-primary\" />\r\n" +
                    "            </div>");
                sb.AppendLine("</form>\r\n");
                sb.AppendLine("    </div>\r\n</div>\r\n\r\n<div>\r\n" +
                    "    <a asp-action=\"Index\">Back to List</a>\r\n</div>\r\n");
                view = sb.ToString();
            }
            else if (vName == "Delete")
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("@model Core.Features.{spName}.Queries.Results.{rName}\r\n" +
                    "\r\n" +
                    "@{\r\n" +
                    "    ViewData[\"Title\"] = \"Delete\";\r\n" +
                    "}\r\n\r\n" +
                    "<h1>Delete</h1>\r\n\r\n" +
                    "<h3>Are you sure you want to delete this?</h3>\r\n<div>\r\n" +
                    "    <h4>{rName}</h4>\r\n" +
                    "    <hr />\r\n" +
                    "    <dl class=\"row\">\r\n");

                foreach (DataRow _row in schemaTable.Rows)
                {
                    string _columnName = _row["Column_name"].ToString();
                    string colName = _columnName.EndsWith("Ar") ? _columnName[..^2] : _columnName;

                    if (!_columnName.EndsWith("En"))
                    {
                        sb.AppendLine("<dt class = \"col-sm-2\">\r\n" +
                        "            @Html.DisplayNameFor(model => model." + colName + ")\r\n" +
                        "        </dt>");
                    }
                }
                sb.AppendLine("</dl>\r\n" +
                    "    \r\n" +
                    "    <form asp-action=\"Delete\">\r\n" +
                    "        <input type=\"submit\" value=\"Delete\" class=\"btn btn-danger\" /> |\r\n" +
                    "        <a asp-action=\"Index\">Back to List</a>\r\n" +
                    "    </form>\r\n</div>");
                view = sb.ToString();
            }
            else if (vName == "Details")
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<div>");
                sb.AppendLine("@model Core.Features.{spName}.Queries.Results.{rName}\r\n" +
                    "\r\n" +
                    "@{\r\n" +
                    "    ViewData[\"Title\"] = \"Details\";\r\n" +
                    "}\r\n\r\n" +
                    "<h1>Details</h1>\r\n\r\n" +

                    "    <h4>{rName}</h4>\r\n" +
                    "    <hr />\r\n" +
                    "    <dl class=\"row\">\r\n");

                foreach (DataRow _row in schemaTable.Rows)
                {
                    string _columnName = _row["Column_name"].ToString();
                    string colName = _columnName.EndsWith("Ar") ? _columnName[..^2] : _columnName;
                    if (!_columnName.EndsWith("En"))
                    {

                        sb.AppendLine("<dt class = \"col-sm-2\">\r\n" +
                      "            @Html.DisplayNameFor(model => model." + colName + ")\r\n" +
                      "        </dt>");
                    }

                }
                sb.AppendLine("</dl>");
                sb.AppendLine("</div>\r\n<div>\r\n" +
                    "< a onclick =\"showInPopup('@Url.Action(\"AddOrEdit\",\"{cName}\",new {id=Model." + Key + "},Context.Request.Scheme)','تعديل ')\" " +
                    " class=\"btn btn-info text-white\"><i class=\"fas fa-pencil-alt\"></i> تعديل</a>\r\n" +
                    "    <a asp-action=\"Index\">Back to List</a>\r\n</div>");
                view = sb.ToString();
            }
            return view.Replace("{cName}", cName).Replace("{spName}", spName).Replace("{rName}", rName); ;
        }
    }
}
