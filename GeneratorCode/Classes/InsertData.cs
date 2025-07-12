using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorCode.Classes
{
    public class InsertData
    {
        public string conString = @"server=DESKTOP-1J9IHO5\MSSQLSERVER2019; uid=sa;pwd=CX_cxAdm0n; database=e_edu_org_mali";
        public object ID { get; set; }
        public object MethodText { get; set; }
        public object MethodCode { get; set; }
        public object MethodType { get; set; }
        public object EstNo { get; set; }
        public object IsDelete { get; set; }
        public object MethodState { get; set; }
        public object SortedBy { get; set; }
        public void SetParamsNull()
        {
            ID = null;
            MethodText = null;
            MethodCode = null;
            MethodType = null;
            EstNo = null;
            IsDelete = null;
            MethodState = null;
            SortedBy = null;
        }




        public Dictionary<string, object> GetParams(int Process)
        {
            Dictionary<string, object> Result = new Dictionary<string, object>();
            Result.Add("@ID", ID);
            Result.Add("@MethodText", MethodText);
            Result.Add("@MethodCode", MethodCode);
            Result.Add("@MethodType", MethodType);
            Result.Add("@EstNo", EstNo);
            Result.Add("@IsDelete", IsDelete);
            Result.Add("@MethodState", MethodState);
            Result.Add("@SortedBy", SortedBy);
            Result.Add("@W", Process);
            return Result;
        }




        public DataTable GetData(int Process)
        {
            Dictionary<string, object> Params = GetParams(Process);
            AdminDAL DAL = new AdminDAL(conString);
            DataTable Result = DAL.DoQuery("usp_INV_Methods_opreations", Params);
            return Result;
        }




        public DataSet GetDataSet(int Process)
        {
            Dictionary<string, object> Params = GetParams(Process);
            AdminDAL DAL = new AdminDAL(conString);
            DataSet Result = DAL.DSDoQuery("usp_INV_Methods_opreations", Params);
            return Result;
        }




        public int SetData(int Process)
        {
            Dictionary<string, object> Params = GetParams(Process);
            AdminDAL DAL = new AdminDAL(conString);
            int Result = DAL.DoUpdate("usp_INV_Methods_opreations", Params);
            return Result;
        }




        bool IsSetData(int Process)
        {
            Dictionary<string, object> Params = GetParams(Process);
            AdminDAL DAL = new AdminDAL(conString);
            int Result = DAL.DoUpdate("usp_INV_Methods_opreations", Params);
            return Result > 0 ? true : false;
        }



    }
}
