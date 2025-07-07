using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
public abstract class MainClass
{
    SqlConnection conn;
    SqlCommand _Com;
    SortedList Para;
    string ProcName;
    public MainClass()
    {
    }
    
    protected int Add(int param, string outname)
    {
        bool Result = false;
        bool Flage = false;
        int Re = 0;
        outname = "@" + outname;
        Load2Prop("", param);
        _Com = new SqlCommand(ProcName, conn);
        _Com.CommandType = CommandType.StoredProcedure;
        _Com.Parameters.Clear();
        for (int i = 0; i < Para.Count; i++)
        {
            if (Para.GetKey(i).ToString() != outname)
            {
                _Com.Parameters.AddWithValue(Para.GetKey(i).ToString(), Para.GetByIndex(i));
            }
            else
            {
                _Com.Parameters.AddWithValue(Para.GetKey(i).ToString(), Para.GetByIndex(i)).Direction = ParameterDirection.InputOutput;
            }
        }
        if (conn.State == ConnectionState.Closed)
        {
            conn.Open();
        }
        try
        {
            if (_Com.ExecuteNonQuery() > 0)
                Result = true;
            try
            {
                Flage = int.TryParse(_Com.Parameters[outname].Value.ToString(), out Re);
            }
            catch (Exception ex)
            { }
        }
        catch (Exception ex)
        {
            Result = false;
        }
        conn.Close();
        if (!Result) return -1;
        if (!Flage || Re == 0) return 0;
        return Re;
    }
    protected bool Add(int param)
    {
        bool Result = false;
        Load2Prop("", param);
        _Com = new SqlCommand(ProcName, conn);
        _Com.CommandType = CommandType.StoredProcedure;
        _Com.Parameters.Clear();
        for (int i = 0; i < Para.Count; i++)
        {
            _Com.Parameters.AddWithValue(Para.GetKey(i).ToString(), Para.GetByIndex(i));
        }
        if (conn.State == ConnectionState.Closed)
        {
            conn.Open();
        }
        try
        {
            if (_Com.ExecuteNonQuery() > 0)
            {
                Result = true;
            }
        }
        catch (Exception ex)
        {
            Result = false;
        }
        return Result;
    }
    protected DataTable Fill_DropDownList(ComboBox ControlID, string Display, string Val, int param, bool addselectoption = false)
    {
        Load2Prop("", param);
        _Com = new SqlCommand(ProcName, conn);
        _Com.CommandType = CommandType.StoredProcedure;
        _Com.Parameters.Clear();
        DataTable Result = new DataTable();
        for (int i = 0; i < Para.Count; i++)
        {
            _Com.Parameters.AddWithValue(Para.GetKey(i).ToString(), Para.GetByIndex(i));
        }
        if (conn.State == ConnectionState.Closed)
        {
            conn.Open();
        }
        SqlDataAdapter da = new SqlDataAdapter(_Com);
        da.Fill(Result);
        conn.Close();
        if (ControlID != null)
        {
            ControlID.DataSource = Result;
            ControlID.DisplayMember = Display;
            ControlID.ValueMember = Val;
        }
        return Result;
    }
    protected DataTable Fill_GridView(DataGridView ControlID, int param)
    {
        Load2Prop("", param);
        _Com = new SqlCommand(ProcName, conn);
        _Com.CommandType = CommandType.StoredProcedure;
        _Com.Parameters.Clear();
        DataTable Result = new DataTable();
        for (int i = 0; i < Para.Count; i++)
        {
            _Com.Parameters.AddWithValue(Para.GetKey(i).ToString(), Para.GetByIndex(i));
        }
        if (conn.State == ConnectionState.Closed)
        {
            conn.Open();
        }
        SqlDataAdapter da = new SqlDataAdapter(_Com);
        da.Fill(Result);
        conn.Close();
        if (ControlID != null)
        {
            ControlID.DataSource = Result;
        }
        return Result;
    }
    protected virtual bool Load2Prop(string TypeOfOpration, int param)
    {
        return true;
    }
    protected DataTable Select_Data(int param)
    {
        Load2Prop("", param);
        _Com = new SqlCommand(ProcName, conn);
        _Com.CommandType = CommandType.StoredProcedure;
        _Com.Parameters.Clear();
        DataTable Result = new DataTable();
        for (int i = 0; i < Para.Count; i++)
        {
            _Com.Parameters.AddWithValue(Para.GetKey(i).ToString(), Para.GetByIndex(i));
        }
        if (conn.State == ConnectionState.Closed)
        {
            conn.Open();
        }
        SqlDataAdapter da = new SqlDataAdapter(_Com);
        da.Fill(Result);
        conn.Close();
        return Result;
    }
    protected DataSet Select_DataSet(int param)
    {
        Load2Prop("", param);
        _Com = new SqlCommand(ProcName, conn);
        _Com.CommandType = CommandType.StoredProcedure;
        _Com.Parameters.Clear();
        DataSet Result = new DataSet();
        for (int i = 0; i < Para.Count; i++)
        {
            _Com.Parameters.AddWithValue(Para.GetKey(i).ToString(), Para.GetByIndex(i));
        }
        if (conn.State == ConnectionState.Closed)
        {
            conn.Open();
        }
        SqlDataAdapter da = new SqlDataAdapter(_Com);
        da.Fill(Result);
        conn.Close();
        return Result;
    }
    public int RunProc(string ProcName1, SortedList Para1, string TypeOfOpration1)
    {
        int RowsCount = 1;
        ProcName = ProcName1;
        Para = Para1;
        return RowsCount;
    }
}
