using System.Collections;
using System.Data;
using System.Windows.Forms;
public class Documents_mn : MainClass
{
    public object ID { get; set; }
    public object Filename { get; set; }
    public object Title { get; set; }
    public object Author { get; set; }
    public object YearPublished { get; set; }
    public object DateUploaded { get; set; }
    public object Status { get; set; }
    public object DateUpdated { get; set; }
    public object Remarks { get; set; }
    public object SoftCat_ID { get; set; }
    public object student_ID { get; set; }
    public object Group_ID { get; set; }
    public void SetParamsNull()
    {
        ID = null;
        Filename = null;
        Title = null;
        Author = null;
        YearPublished = null;
        DateUploaded = null;
        Status = null;
        DateUpdated = null;
        Remarks = null;
        SoftCat_ID = null;
        student_ID = null;
        Group_ID = null;
    }
    protected override bool Load2Prop(string TypeOfOpration, int param)
    {
        SortedList sl = new SortedList();
        sl.Add("@ID", ID);
        sl.Add("@Filename", Filename);
        sl.Add("@Title", Title);
        sl.Add("@Author", Author);
        sl.Add("@YearPublished", YearPublished);
        sl.Add("@DateUploaded", DateUploaded);
        sl.Add("@Status", Status);
        sl.Add("@DateUpdated", DateUpdated);
        sl.Add("@Remarks", Remarks);
        sl.Add("@SoftCat_ID", SoftCat_ID);
        sl.Add("@student_ID", student_ID);
        sl.Add("@Group_ID", Group_ID);
        sl.Add("@w", param);
        if (RunProc("usp_Documents_opreations", sl, TypeOfOpration) > 0)
        {
            return true;
        }
        return false;
    }
    public bool Execute(int p)
    {
        if (Add(p))
        {
            return true;
        }
        return false;
    }
    public DataTable GetData(int p)
    {
        DataTable Result = Select_Data(p);
        return Result;
    }
    public DataSet GetDataSet(int p)
    {
        DataSet Result = Select_DataSet(p);
        return Result;
    }
    public DataTable dropdown(ComboBox d, string txt, string val, int p)
    {
        return Fill_DropDownList(d, txt, val, p);
    }
    public DataTable gridview(DataGridView d, string txt, string val, int p)
    {
        return Fill_GridView(d, p);
    }
}
