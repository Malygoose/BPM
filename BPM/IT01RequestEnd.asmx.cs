using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using static BPMLib.Class1;

namespace BPM
{
    /// <summary>
    ///IT01RequestEnd 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class IT01RequestEnd : System.Web.Services.WebService
    {

        [WebMethod]
        public bool Run(int ID)
        {
            int intApViewID = ID;

            dbFunction dbFunction = new dbFunction();
            SqlConnection sqlcon = dbFunction.sqlHissFlowConnection();
            SqlCommand sqlcmd = new SqlCommand("spBPMRequestStateUpdate", sqlcon);
            sqlcmd.CommandType = System.Data.CommandType.StoredProcedure;
            sqlcmd.Parameters.AddWithValue("@intApViewID", intApViewID);
            sqlcmd.Parameters.AddWithValue("@strState", "3");

            sqlcon.Open();
            sqlcmd.ExecuteNonQuery();

            sqlcmd.Dispose();

            return true;
        }
    }
}
