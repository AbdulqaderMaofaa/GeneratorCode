using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace GeneratorCode.Classes
{

    public class CreateClassByDataTable
    {
        private string _connectionString;
        private string _tableName;
        private string _className;
        private bool _withMain;
        public bool cbxWinApp;
        private string _mainClass;
        private string _filePath;

        public string filePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
            }
        }
        public string className
        {
            get
            {
                return _className;
            }
            set
            {
                _className = value;
            }
        }
        public string mainClass
        {
            get
            {
                return _mainClass;
            }
            set
            {
                _mainClass = value;
            }
        }
        public string tableName
        {
            get
            {
                return _tableName;
            }
            set
            {
                _tableName = value;
            }
        }
        public string connectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }
        public bool withMain
        {
            get
            {
                return _withMain;
            }
            set
            {
                _withMain = value;
            }
        }
        public CreateClassByDataTable()
        {
            //string className = tableName;

        }
        public void CreateClass()
        {
            bool createMain = GenerateMainClass();


            if (createMain)
            {
                List<string> Columns = new List<string>();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable schemaTable = connection.GetSchema("Columns", new string[] { null, null, tableName });
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(setUsing());
                    // Generate the C# class from the table schema
                    sb.AppendLine("public class " + _className + " :MainClass");
                    // _className += ":" + _mainClass;
                    sb.AppendLine("{");
                    //string classCode = "public class " + _className + "\n{\n";
                    foreach (DataRow row in schemaTable.Rows)
                    {
                        string columnName = row["COLUMN_NAME"].ToString();
                        string dataType = row["DATA_TYPE"].ToString();
                        string classCode = "public " + GetDataType(dataType) + " " + columnName + " { get; set; }";
                        sb.AppendLine(classCode);
                        Columns.Add(columnName);

                    }
                    sb.AppendLine(Environment.NewLine);
                    string proc = "usp_" + _className;
                    string funs = AddFunctions(Columns, proc);
                    sb.AppendLine(Environment.NewLine);
                    sb.AppendLine(funs);
                    sb.AppendLine(Environment.NewLine);
                    sb.AppendLine("}");
                    // Save the generated class code to a text file
                    string fileName = _className + "min" + ".cs";

                    string nfilePath = _filePath + @"\" + fileName;
                    SaveFiles(sb, nfilePath);
                }

            }



        }
        public void CreateAdminClass()
        {
            bool createAdmin = GenerateAdminDAL();
            if (createAdmin)
            {
                List<string> Columns = new List<string>();
                List<string> Params = new List<string>();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable schemaTable = connection.GetSchema("Columns", new string[] { null, null, tableName });
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(setUsing());
                    // Generate the C# class from the table schema
                    sb.AppendLine("public class " + _className + "");
                    sb.AppendLine("{");
                    sb.AppendLine("public string conString =\"@\"" + "\"" + _connectionString + "\"\"" + ";");

                    //string classCode = "public class " + _className + "\n{\n";
                    foreach (DataRow row in schemaTable.Rows)
                    {
                        string columnName = row["COLUMN_NAME"].ToString();
                        string dataType = row["DATA_TYPE"].ToString();
                        string classCode = "public " + GetDataType(dataType) + " " + columnName + " { get; set; }";
                        sb.AppendLine(classCode);
                        Columns.Add(columnName);
                        Params.Add("@" + columnName);
                    }
                    sb.AppendLine("public void SetParamsNull()");
                    sb.AppendLine("{");
                    foreach (string item in Columns)
                    {
                        sb.AppendLine(item + " = null;");
                    }
                    sb.AppendLine("}");
                    sb.AppendLine(Environment.NewLine);

                    sb.AppendLine("public Dictionary<string, object> GetParams(int Process)");
                    sb.AppendLine("{");
                    sb.AppendLine("Dictionary<string, object> Result = new Dictionary<string, object>();");

                    foreach (string item in Columns)
                    {
                        string xtx = "@" + item;
                        sb.AppendLine("Result.Add(\"" + xtx + "\"" + ", " + item + "); ");
                    }
                    string proc = "usp_" + _tableName + "_opreations";
                    sb.AppendLine("Result.Add(\"@W\", Process);");
                    sb.AppendLine("return Result;");
                    sb.AppendLine("}");
                    sb.AppendLine(Environment.NewLine);
                    sb.AppendLine("public DataTable GetData(int Process)");
                    sb.AppendLine("{");
                    sb.AppendLine("Dictionary<string, object> Params = GetParams(Process);");
                    sb.AppendLine("AdminDAL DAL = new AdminDAL(conString);");
                    sb.AppendLine("DataTable Result = DAL.DoQuery(\"" + proc + "\", Params);");
                    sb.AppendLine("return Result;");
                    sb.AppendLine("}");

                    sb.AppendLine(Environment.NewLine);
                    sb.AppendLine("public DataSet GetDataSet(int Process)");
                    sb.AppendLine("{");
                    sb.AppendLine("Dictionary<string, object> Params = GetParams(Process);");
                    sb.AppendLine("AdminDAL DAL = new AdminDAL(conString);");
                    sb.AppendLine("DataSet Result = DAL.DSDoQuery(\"" + proc + "\", Params);");
                    sb.AppendLine("return Result;");
                    sb.AppendLine("}");

                    sb.AppendLine(Environment.NewLine);
                    sb.AppendLine("public int SetData(int Process)");
                    sb.AppendLine("{");
                    sb.AppendLine("Dictionary<string, object> Params = GetParams(Process);");
                    sb.AppendLine("AdminDAL DAL = new AdminDAL(conString);");
                    sb.AppendLine("int Result = DAL.DoUpdate(\"" + proc + "\", Params);");
                    sb.AppendLine("return Result;");
                    sb.AppendLine("}");


                    sb.AppendLine(Environment.NewLine);
                    sb.AppendLine("public bool IsSetData(int Process)");
                    sb.AppendLine("{");
                    sb.AppendLine("Dictionary<string, object> Params = GetParams(Process);");
                    sb.AppendLine("AdminDAL DAL = new AdminDAL(conString);");
                    sb.AppendLine("int Result = DAL.DoUpdate(\"" + proc + "\", Params);");
                    sb.AppendLine("return Result > 0 ? true : false;");
                    sb.AppendLine("}");

                    sb.AppendLine(Environment.NewLine);
                    sb.AppendLine("}");
                    // Save the generated class code to a text file

                    string fileName = _className + "admin" + ".cs";

                    string nfilePath = _filePath + @"\" + fileName;
                    SaveFiles(sb, nfilePath);
                }

            }
        }
        private string AddFunctions(List<string> Columns, string proc)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("public void SetParamsNull()");
            sb.AppendLine("{");
            foreach (string item in Columns)
            {

                string classCode = item + "=null;";
                sb.AppendLine(classCode);

            }
            sb.AppendLine(Environment.NewLine);
            sb.AppendLine("}");


            sb.AppendLine("protected override bool Load2Prop(string TypeOfOpration, int param)");
            sb.AppendLine("{");
            sb.AppendLine("SortedList sl = new SortedList();");

            foreach (string item in Columns)
            {
                string xtx = "@" + item;
                sb.AppendLine("sl.Add(\"" + xtx + "\"" + ", " + item + "); ");
            }
            sb.AppendLine("sl.Add(\"@w\", param);");
            sb.AppendLine("if (RunProc(\"" + proc + "\", sl, TypeOfOpration) > 0)");
            sb.AppendLine("{");
            sb.AppendLine("return true;");
            sb.AppendLine("}");
            sb.AppendLine("else");
            sb.AppendLine("{");
            sb.AppendLine("return false;");
            sb.AppendLine("}");
            sb.AppendLine(Environment.NewLine);
            sb.AppendLine("}");

            sb.AppendLine("public bool Execute(int p)");
            sb.AppendLine("{");
            sb.AppendLine("if (Add(p))");
            sb.AppendLine("{");
            sb.AppendLine("return true;");
            sb.AppendLine("}");
            sb.AppendLine("return false;");
            sb.AppendLine("}");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("public DataTable GetData(int p)");
            sb.AppendLine("{");
            sb.AppendLine("DataTable Result = Select_Data(p);");
            sb.AppendLine("return Result;");
            sb.AppendLine("}");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("public DataSet GetDataSet(int p)");
            sb.AppendLine("{");
            sb.AppendLine("DataSet Result = Select_DataSet(p);");
            sb.AppendLine("return Result;");
            sb.AppendLine("}");
            sb.AppendLine(Environment.NewLine);



            return sb.ToString();
        }

        private string setUsing()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Data;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Text;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using System.Collections;");
            return sb.ToString();
        }

        private void SaveFiles(StringBuilder sb, string nfilePath)
        {

            //filePath = _filePath + @"\" + _className;
            if (File.Exists(nfilePath))
            {
                File.Delete(nfilePath);
            }
            using (StreamWriter w = new StreamWriter(nfilePath, true))
            {
                string[] delim = { Environment.NewLine, "\n" }; // "\n" added in case you manually appended a newline
                string[] lines = sb.ToString().Split(delim, StringSplitOptions.None);
                foreach (string item in lines)
                {
                    w.WriteLine(item);

                }

                // File.WriteAllText(fileName, code);
            }
        }
        public static void DataTableToTextFile(DataTable dtToText, string filePath)
        {
            int i = 0;
            StreamWriter sw = null;

            sw = new StreamWriter(filePath, false); /*For ColumnName's */
                for (i = 0; i < dtToText.Columns.Count - 1; i++)
                {
                    sw.Write(dtToText.Columns[i].ColumnName + '~');
                }
                sw.Write(dtToText.Columns[i].ColumnName + '~');
                sw.WriteLine(); /*For Data in the Rows*/

                foreach (DataRow row in dtToText.Rows)
                {
                    object[] array = row.ItemArray;
                    for (i = 0; i < array.Length - 1; i++)
                    {
                        sw.Write(array[i].ToString() + '~');
                    }
                    sw.Write(array[i].ToString() + '~');
                    sw.WriteLine();
                }
                sw.Close();
        }
        string GetDataType(string dataType)
        {
            //switch (dataType)
            //{
            //    case "int":
            //    case "bigint":
            //    case "smallint":
            //        return "int";
            //    case "bit":
            //        return "bool";
            //    case "datetime":
            //    case "smalldatetime":
            //        return "DateTime";
            //    case "decimal":
            //    case "money":
            //    case "numeric":
            //    case "smallmoney":
            //        return "decimal";
            //    case "float":
            //        return "double";
            //    case "real":
            //        return "float";
            //    case "char":
            //    case "nchar":
            //    case "nvarchar":
            //    case "text":
            //    case "ntext":
            //    case "varchar":
            //        return "string";
            //    case "binary":
            //    case "image":
            //    case "varbinary":
            //        return "byte[]";
            //    default:
            //        return "object";
            //}
            return "object";
        }
        public bool GenerateMainClass()
        {
            string fileName = "MainClass.cs";

            // string FilePath = @"C:\Users\Abdulqader\Documents\GenerateorCodes/TableName";

            string nfilePath = _filePath + @"\" + fileName;
            string controlDDl = "DropDownList";
            string controlgrv = "GridView";
            if (cbxWinApp)
            {
                controlDDl = "ComboBox";
                controlgrv = "DataGridView";
            }
            string TableName = _tableName;

            if (File.Exists(nfilePath))
            {
                File.Delete(nfilePath);
            }
            List<string> Columns = new List<string>();
            List<string> Params = new List<string>();
            using (StreamWriter w = new StreamWriter(nfilePath, true))
                {
                    #region المكتبات
                    w.WriteLine("using System;");
                    w.WriteLine("using System.Collections.Generic;");
                    w.WriteLine("using System.Data;");
                    w.WriteLine("using System.Linq;");
                    w.WriteLine("using System.Text;");
                    w.WriteLine("using System.Threading.Tasks;");
                    w.WriteLine("using System.Data.SqlClient;");
                    w.WriteLine("using System.Web;");
                    w.WriteLine("using System.Collections;");
                    if (cbxWinApp)
                    {
                        w.WriteLine("using System.Windows.Forms;");
                    }
                    else
                    {
                        w.WriteLine("using System.Web.UI.WebControls;");
                        w.WriteLine("using DevExpress.Web;");

                    }
                    #endregion

                    #region دالة البناء
                    w.WriteLine("public abstract class MainClass");
                    w.WriteLine("{");
                    w.WriteLine("SqlConnection conn;");
                    w.WriteLine("SqlCommand _Com;");
                    w.WriteLine("SortedList Para;");
                    w.WriteLine("string ProcName;");
                    w.WriteLine("public MainClass()");
                    w.WriteLine("{");
                    w.WriteLine("}");
                    #endregion

                    #region دالة  int Add(int param, string outname)
                    w.WriteLine("protected int Add(int param, string outname)");
                    w.WriteLine("{");
                    w.WriteLine("bool Result = false;");
                    w.WriteLine("bool Flage = false;");
                    w.WriteLine("int Re = 0;");
                    w.WriteLine("outname = \"@\" + outname;");
                    InitialFun(w);
                    w.WriteLine("for (int i = 0; i < Para.Count; i++)");
                    w.WriteLine("{");
                    w.WriteLine("if (Para.GetKey(i).ToString() != outname)");
                    w.WriteLine("{");
                    w.WriteLine(" _Com.Parameters.AddWithValue(Para.GetKey(i).ToString(), Para.GetByIndex(i));");
                    w.WriteLine("}");
                    w.WriteLine("else");
                    w.WriteLine("{");
                    w.WriteLine(" _Com.Parameters.AddWithValue(Para.GetKey(i).ToString(), Para.GetByIndex(i)).Direction = ParameterDirection.InputOutput;");
                    w.WriteLine("}");
                    w.WriteLine("}");
                    w.WriteLine("if (conn.State == ConnectionState.Closed)");
                    w.WriteLine("{");
                    w.WriteLine("conn.Open();");
                    w.WriteLine("}");
                    w.WriteLine("try");
                    w.WriteLine("{");
                    w.WriteLine("if (_Com.ExecuteNonQuery() > 0)");
                    w.WriteLine("Result = true;");

                    w.WriteLine("try{" + Environment.NewLine +
                        "Flage = int.TryParse(_Com.Parameters[outname].Value.ToString(), out Re);" +
                       Environment.NewLine + "}" + Environment.NewLine +
                       "catch(Exception ex)" + Environment.NewLine +
                       "{" +
                       "}");
                    w.WriteLine("}");
                    w.WriteLine("catch (Exception ex)");
                    w.WriteLine("{");
                    w.WriteLine("Result = false;");
                    w.WriteLine("}");
                    w.WriteLine("conn.Close();");
                    w.WriteLine("if (!Result) return -1;");
                    w.WriteLine("if (!Flage || Re == 0) return 0;");
                    w.WriteLine("return Re;");
                    w.WriteLine("}");
                    #endregion

                    #region دالة  bool Add(int param)
                    w.WriteLine("protected bool Add(int param)");
                    w.WriteLine("{");
                    w.WriteLine("bool Result = false;");
                    InitialFun(w);
                    BindPararm(w);
                    w.WriteLine("try");
                    w.WriteLine("{");
                    w.WriteLine("if (_Com.ExecuteNonQuery() > 0)");
                    w.WriteLine("{");
                    w.WriteLine("Result = true;");
                    w.WriteLine("}");
                    w.WriteLine("}");
                    w.WriteLine("catch (Exception ex)");
                    w.WriteLine("{");
                    w.WriteLine("Result = false;");
                    w.WriteLine("}");
                    w.WriteLine("return Result;");
                    w.WriteLine("}");
                    #endregion


                    #region دالة protected virtual bool Load2Prop(string TypeOfOpration, int param)
                    w.WriteLine("protected virtual bool Load2Prop(string TypeOfOpration, int param)");
                    w.WriteLine("{");
                    w.WriteLine("return true;");
                    w.WriteLine("}");
                    #endregion

                    #region دالة protected DataTable Select_Data(int param)
                    w.WriteLine("protected DataTable Select_Data(int param)");
                    w.WriteLine("{");
                    InitialFun(w);
                    w.WriteLine("DataTable Result = new DataTable();");
                    BindPararm(w);
                    w.WriteLine("SqlDataAdapter da = new SqlDataAdapter(_Com);");
                    w.WriteLine("da.Fill(Result);");
                    w.WriteLine("conn.Close();");
                    w.WriteLine("return Result;");
                    w.WriteLine("}");
                    #endregion

                    #region دالة protected DataSet Select_DataSet(int param)
                    w.WriteLine("protected DataSet Select_DataSet(int param)");
                    w.WriteLine("{");
                    InitialFun(w);
                    w.WriteLine("DataSet Result = new DataSet();");
                    BindPararm(w);
                    w.WriteLine("SqlDataAdapter da = new SqlDataAdapter(_Com);");
                    w.WriteLine("da.Fill(Result);");
                    w.WriteLine("conn.Close();");
                    w.WriteLine("return Result;");
                    w.WriteLine("}");
                    #endregion

                    #region دالة public int RunProc(string ProcName1, SortedList Para1, string TypeOfOpration1)
                    w.WriteLine("public int RunProc(string ProcName1, SortedList Para1, string TypeOfOpration1)");
                    w.WriteLine("{");
                    w.WriteLine("int RowsCount = 1;");
                    w.WriteLine("ProcName = ProcName1;");
                    w.WriteLine("Para = Para1;");
                    w.WriteLine("return RowsCount;");
                    w.WriteLine("}");
                    #endregion

                    w.WriteLine("}");

                    return true;
                }

            return false;
        }
        private void BindPararm(StreamWriter w)
        {
            w.WriteLine("for (int i = 0; i < Para.Count; i++)");
            w.WriteLine("{");
            w.WriteLine("_Com.Parameters.AddWithValue(Para.GetKey(i).ToString(), Para.GetByIndex(i));");
            w.WriteLine("}");
            w.WriteLine("if (conn.State == ConnectionState.Closed)");
            w.WriteLine("{");
            w.WriteLine("conn.Open();");
            w.WriteLine("}");
        }
        private void InitialFun(StreamWriter w)
        {
            w.WriteLine("Load2Prop(\"\", param);");
            w.WriteLine("_Com = new SqlCommand(ProcName, conn);");
            w.WriteLine("_Com.CommandType = CommandType.StoredProcedure;");
            w.WriteLine("_Com.Parameters.Clear();");
        }
        public bool GenerateAdminDAL()
        {

            string FileName = "AdminDAL.cs";
            // string FilePath = @"C:\Users\Abdulqader\Documents\GenerateorCodes/TableName";
            string nfilePath = _filePath + @"\" + FileName;

            if (File.Exists(nfilePath))
            {
                File.Delete(nfilePath);
            }
            List<string> Columns = new List<string>();
            List<string> Params = new List<string>();
            using (StreamWriter w = new StreamWriter(nfilePath, true))
            {

                w.WriteLine("using System;");
                w.WriteLine("using System.Collections.Generic;");
                w.WriteLine("using System.Data;");
                w.WriteLine("using System.Linq;");
                w.WriteLine("using System.Text;");
                w.WriteLine("using System.Threading.Tasks;");

                w.WriteLine("public class" + "  AdminDAL ");
                w.WriteLine("{");
                w.WriteLine("SqlConnection conn;");
                w.WriteLine("SqlCommand _Com;");
                w.WriteLine("public AdminDAL(string connectionString)");
                w.WriteLine("{");
                w.WriteLine("conn= new SqlConnection(connectionString);");
                w.WriteLine(" _Com = new SqlCommand();");
                w.WriteLine("_Com.Connection = conn;");
                w.WriteLine("_Com.CommandType = CommandType.StoredProcedure;");
                w.WriteLine("_Com.CommandTimeout = 50000;");
                w.WriteLine("}");
                w.WriteLine(Environment.NewLine);

                #region DSDoQuery
                w.WriteLine("public DataSet DSDoQuery(string StoredProcedure, Dictionary<string, object> Para)");
                w.WriteLine("{");
                w.WriteLine("_Com = new SqlCommand();");
                w.WriteLine("SqlDataAdapter Adapter;");
                w.WriteLine("_Com.Connection = conn;");
                w.WriteLine("_Com.CommandType = CommandType.StoredProcedure;");
                w.WriteLine("_Com.CommandText = StoredProcedure;");
                w.WriteLine("_Com.CommandTimeout = 50000;");
                w.WriteLine("if (Para != null)");
                w.WriteLine("{");
                w.WriteLine("foreach (KeyValuePair<string, object> Current in Para)");
                w.WriteLine("{_Com.Parameters.Add(new SqlParameter(Current.Key, Current.Value));}");
                w.WriteLine("}");
                w.WriteLine("Adapter = new SqlDataAdapter(_Com);");
                w.WriteLine("DataSet _ds = new DataSet();");
                w.WriteLine("if (conn.State != System.Data.ConnectionState.Open)");
                w.WriteLine("{");
                w.WriteLine("conn.Open();");
                w.WriteLine("}");
                w.WriteLine("Adapter.Fill(_ds);");
                w.WriteLine("conn.Close();");
                w.WriteLine("return _ds;");
                w.WriteLine("}");
                #endregion
                #region   DoQuery
                w.WriteLine("public DataTable DoQuery(string StoredProcedure, Dictionary<string, object> Para)");
                w.WriteLine("{");
                w.WriteLine("_Com = new SqlCommand();");
                w.WriteLine("SqlDataAdapter Adapter;");
                w.WriteLine("_Com.Connection = conn;");
                w.WriteLine("_Com.CommandType = CommandType.StoredProcedure;");
                w.WriteLine("_Com.CommandText = StoredProcedure;");
                w.WriteLine("_Com.CommandTimeout = 50000;");
                w.WriteLine("if (Para != null)");
                w.WriteLine("{");
                w.WriteLine("foreach (KeyValuePair<string, object> Current in Para)");
                w.WriteLine("{_Com.Parameters.Add(new SqlParameter(Current.Key, Current.Value));}");
                w.WriteLine("}");
                w.WriteLine("Adapter = new SqlDataAdapter(_Com);");
                w.WriteLine("DataTable Result = new DataTable();");
                w.WriteLine("if (conn.State != System.Data.ConnectionState.Open)");
                w.WriteLine("{");
                w.WriteLine("conn.Open();");
                w.WriteLine("}");
                w.WriteLine("Adapter.Fill(Result);");
                w.WriteLine("conn.Close();");
                w.WriteLine("return Result;");
                w.WriteLine("}");
                #endregion
                #region DoUpdate
                w.WriteLine("public int DoUpdate(string StoredProcedure, Dictionary<string, object> Para)");
                w.WriteLine("{");
                w.WriteLine(" int Result = 0;");
                w.WriteLine("_Com = new SqlCommand();");
                w.WriteLine("SqlDataAdapter Adapter;");
                w.WriteLine("_Com.Connection = conn;");
                w.WriteLine("_Com.CommandType = CommandType.StoredProcedure;");
                w.WriteLine("_Com.CommandText = StoredProcedure;");
                w.WriteLine("_Com.CommandTimeout = 50000;");
                w.WriteLine("if (Para != null)");
                w.WriteLine("{");
                w.WriteLine("foreach (KeyValuePair<string, object> Current in Para)");
                w.WriteLine("{_Com.Parameters.Add(new SqlParameter(Current.Key, Current.Value));}");
                w.WriteLine("}");

                w.WriteLine("if (conn.State != System.Data.ConnectionState.Open)");
                w.WriteLine("{");
                w.WriteLine("conn.Open();");
                w.WriteLine("}");
                w.WriteLine(" Result = _Com.ExecuteNonQuery();");
                w.WriteLine("conn.Close();");
                w.WriteLine("return Result;");
                w.WriteLine("}");
                #endregion



                w.WriteLine(Environment.NewLine);
                w.WriteLine(Environment.NewLine);
                w.WriteLine(Environment.NewLine);
                w.WriteLine("}");
                return true;
            }
        }

    }
}
