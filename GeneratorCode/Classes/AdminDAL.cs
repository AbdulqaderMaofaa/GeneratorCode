using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorCode.Classes
{
    public class AdminDAL
    {
        SqlConnection conn;
        SqlCommand _Com;
        public AdminDAL(string connectionString)
        {
            conn = new SqlConnection(connectionString);
            _Com = new SqlCommand();
            _Com.Connection = conn;
            _Com.CommandType = CommandType.StoredProcedure;
            _Com.CommandTimeout = 50000;
        }


        public DataSet DSDoQuery(string StoredProcedure, Dictionary<string, object> Para)
        {
            _Com = new SqlCommand();
            SqlDataAdapter Adapter;
            _Com.Connection = conn;
            _Com.CommandType = CommandType.StoredProcedure;
            _Com.CommandText = StoredProcedure;
            _Com.CommandTimeout = 50000;
            if (Para != null)
            {
                foreach (KeyValuePair<string, object> Current in Para)
                { _Com.Parameters.Add(new SqlParameter(Current.Key, Current.Value)); }
            }
            Adapter = new SqlDataAdapter(_Com);
            DataSet _ds = new DataSet();
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }
            Adapter.Fill(_ds);
            conn.Close();
            return _ds;
        }
        public DataTable DoQuery(string StoredProcedure, Dictionary<string, object> Para)
        {
            _Com = new SqlCommand();
            SqlDataAdapter Adapter;
            _Com.Connection = conn;
            _Com.CommandType = CommandType.StoredProcedure;
            _Com.CommandText = StoredProcedure;
            _Com.CommandTimeout = 50000;
            if (Para != null)
            {
                foreach (KeyValuePair<string, object> Current in Para)
                { _Com.Parameters.Add(new SqlParameter(Current.Key, Current.Value)); }
            }
            Adapter = new SqlDataAdapter(_Com);
            DataTable Result = new DataTable();
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }
            Adapter.Fill(Result);
            conn.Close();
            return Result;
        }
        public int DoUpdate(string StoredProcedure, Dictionary<string, object> Para)
        {
            int Result = 0;
            _Com = new SqlCommand();
            SqlDataAdapter Adapter;
            _Com.Connection = conn;
            _Com.CommandType = CommandType.StoredProcedure;
            _Com.CommandText = StoredProcedure;
            _Com.CommandTimeout = 50000;
            if (Para != null)
            {
                foreach (KeyValuePair<string, object> Current in Para)
                { _Com.Parameters.Add(new SqlParameter(Current.Key, Current.Value)); }
            }
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }
            Result = _Com.ExecuteNonQuery();
            conn.Close();
            return Result;
        }






    }
}
