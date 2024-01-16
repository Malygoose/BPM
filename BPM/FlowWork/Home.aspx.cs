using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ezEngineServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;

namespace BPM.FlowWork
{
    public partial class Home : System.Web.UI.Page
    {
        private dcFlowDataContext dcFlow = new dcFlowDataContext();

        public string strDateBegin;
        public string strDateEnd;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string LoginEmpID = User.Identity.Name;
                strDateBegin = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
                strDateEnd = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

                pnlApply.Visible = true;

                ddlStateType.SelectedItem.Value = "1";

                GetGrvFormList(LoginEmpID);
                
                string strEmployeeName = (string)Session["EmployeeName"];
                lblLoginName.Text = strEmployeeName;

                //HttpCookie cookie = Request.Cookies["EmployeeName"];
                //lblLoginName.Text = cookie.Value;

                if (LoginEmpID == "20181104" || LoginEmpID=="20210305" || LoginEmpID == "20230305")
                {                 
                    HLAdminterface.Visible = true;
                }
                else
                {
                    HLAdminterface.Visible = false;
                }
            }
            else
            {
                strDateBegin = txbDateA.Text;
                strDateEnd = txbDateB.Text;
            }
        }

        private void GetGrvFormList(string LoginEmpID)
        {
            DateTime dateBegin = DateTime.Parse(strDateBegin);
            DateTime dateEnd = DateTime.Parse(strDateEnd);

            string strUnSigm;
            string strState = ddlStateType.SelectedItem.Value;
            //string strFormCode = "IT01";

            string connectionString = ConfigurationManager.ConnectionStrings["Flow"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "spFlowIT01FormApView1";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@sNobr", LoginEmpID);
                    cmd.Parameters.AddWithValue("@sState", strState);
                    //cmd.Parameters.AddWithValue("@sFormCode", strFormCode);
                    cmd.Parameters.AddWithValue("@strDateA", dateBegin);
                    cmd.Parameters.AddWithValue("@strDateB", dateEnd);

                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    da.SelectCommand = cmd;
                    da.Fill(ds);

                    DataTable dtFormList = ds.Tables[0];

                    grvFormList.DataSource = dtFormList;
                    grvFormList.DataBind();

                    ViewState["FormList"] = dtFormList;

                    strUnSigm = ds.Tables[1].Rows[0]["未簽核數量"].ToString();

                    //lblLoginName.Text = ds.Tables[2].Rows[0]["EmployeeName"].ToString();

                }

                lblUnSign.Text = "(" + strUnSigm + ")";

                //簽核數量visible
                if (strUnSigm == "0")
                    lblUnSign.Visible = false;
                else
                    lblUnSign.Visible = true;

            }
        }

        private void GetGrvFormSign(string LoginEmpID)
        {

            DateTime dateBegin = DateTime.Parse(strDateBegin);
            DateTime dateEnd = DateTime.Parse(strDateEnd);

            string connectionString = ConfigurationManager.ConnectionStrings["Flow"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spFlowIT01FormSign", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strEmpID", LoginEmpID);
                cmd.Parameters.AddWithValue("@intState", ddlStateType.SelectedValue);
                cmd.Parameters.AddWithValue("@strDateA", dateBegin);
                cmd.Parameters.AddWithValue("@strDateB", dateEnd);

                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                da.SelectCommand = cmd;
                conn.Open();
                da.Fill(ds);

                DataTable dtFormSign = ds.Tables[0];

                grvFormSign.DataSource = dtFormSign;
                grvFormSign.DataBind();

                ViewState["FormSign"] = dtFormSign;

            }
        }

        protected void btnFormSignList_Click(object sender, EventArgs e)
        {
            DateTime dateBegin = DateTime.Parse(strDateBegin);
            DateTime dateEnd = DateTime.Parse(strDateEnd);

            pnlSign.Visible = true;
            pnlApply.Visible = false;

            string connectionString = ConfigurationManager.ConnectionStrings["Flow"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spFlowIT01FormSign", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strEmpID", User.Identity.Name);
                cmd.Parameters.AddWithValue("@intState", ddlStateType.SelectedValue);
                cmd.Parameters.AddWithValue("@strDateA", dateBegin);
                cmd.Parameters.AddWithValue("@strDateB", dateEnd);

                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                da.SelectCommand = cmd;
                da.Fill(ds);

                DataTable dtFormSign = ds.Tables[2];

                grvFormSign.DataSource = dtFormSign;
                grvFormSign.DataBind();

                ViewState["FormSign"] = dtFormSign;
            }
        }

        protected void grvFormList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grvFormList.PageIndex = e.NewPageIndex;
            GetGrvFormList(User.Identity.Name);
        }

        protected void grvFormSign_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grvFormSign.PageIndex = e.NewPageIndex;
            DataTable dtFormSign = (DataTable)ViewState["FormSign"];
            grvFormSign.DataSource = dtFormSign;
            grvFormSign.DataBind();
        }

        protected void btnDelete_Command(object sender, CommandEventArgs e)
        {
            ezEngineServices.Service oService = new ezEngineServices.Service(dcFlow.Connection);
            if (e.CommandName == "ProcessDelete")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                DataTable FormList = (DataTable)ViewState["FormList"];

                bool ProcessSign = true;
                CheckProcessExist(ref ProcessSign, e);

                // 判斷此Process有沒有被主管簽核了
                if (!ProcessSign)
                {
                    FormList.Rows.RemoveAt(rowIndex);
                    string sProcessID = grvFormList.Rows[rowIndex].Cells[2].Text;

                    List<int> list = new List<int>();
                    list.Add(int.Parse(sProcessID));

                    oService.FlowStateSet(list, FlowState.Take, User.Identity.Name);

                    Response.Write("<script>alert('" + "撤回成功" + "')</script>");  //刪除失敗 警告訊息
                }
                else
                    Response.Write("<script>alert('" + "撤回失敗，主管已簽核" + "')</script>");  //刪除失敗 警告訊息

                grvFormList.DataSource = FormList;
                grvFormList.DataBind();
            }
        }

        public void CheckProcessExist(ref bool exists, CommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            string strProcessID = grvFormList.Rows[rowIndex].Cells[2].Text;
            int intProcessID = int.Parse(strProcessID);

            string connectionString = ConfigurationManager.ConnectionStrings["ShareElecForm"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "spFlowTake";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@intProcessID", intProcessID);

                    cmd.ExecuteNonQuery();
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    da.SelectCommand = cmd;
                    da.Fill(ds);

                    int count = ds.Tables[0].Rows.Count;

                    if (count > 0)
                    {
                        string strLoginEmpID = User.Identity.Name;

                        string strSignerEmpID = ds.Tables[0].Rows[0]["SignerEmpID"].ToString();
                        bool blnSign = (bool)ds.Tables[0].Rows[0]["bSign"];

                        if (strSignerEmpID == strLoginEmpID && !blnSign)
                        {
                            exists = false;
                        }
                        else
                        {
                            exists = true;
                        }
                    }
                    else if (count == 0)
                    {
                        exists = false;
                    }
                }
            }
        }


        protected void btnFormSign_Click(object sender, EventArgs e)
        {
            GetGrvFormSign(User.Identity.Name);
            pnlSign.Visible = true;
            pnlApply.Visible = false;

        }

        protected void btnFormList_Click(object sender, EventArgs e)
        {
            GetGrvFormList(User.Identity.Name);
            pnlApply.Visible = true;
            pnlSign.Visible = false;

        }

        protected void btnTestDelete_Click(object sender, EventArgs e)
        {
            ezEngineServices.Service oService = new ezEngineServices.Service(dcFlow.Connection);

            List<int> listDeleteProcessID = new List<int>();
            string strTestDelete = txbTestDelete.Text;
            int DeleteProcessID;
            if (int.TryParse(strTestDelete, out DeleteProcessID))
            {
                // 转换成功，将数字添加到 List<int> 中
                listDeleteProcessID.Add(DeleteProcessID);
            }

            oService.FlowStateSet(listDeleteProcessID, FlowState.Delete);
        }

        protected void btnApplyForm_Click(object sender, EventArgs e)
        {
            Response.Redirect("IT01?IT01=?");
        }

        protected void btnTemplate_Click(object sender, EventArgs e)
        {
            Response.Redirect("Template?IT01=?");
        }
        
    }

}