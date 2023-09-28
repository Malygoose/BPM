using ezEngineServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Antlr.Runtime.Tree;

namespace BPM.FlowWork
{
    public partial class IT01 : System.Web.UI.Page
    {
        private dcHRDataContext dcHR = new dcHRDataContext();
        private dcFormDataContext dcForm = new dcFormDataContext();
        private dcFlowDataContext dcFlow = new dcFlowDataContext();

        [Serializable]
        private struct stuFormInfo
        {
            //  表單共用變數-------------------------------------
            public string strFormID;                //表單ID
            public string strFormCode;              //表單代號 
            public string strFormName;              //表單名稱 

            public int intProcessID;                //流程ID
            public int intApKey;                    //流程ApKey
            public bool blnSign;                    //流程目前bSign值

            public string strApplyEmpName;          //申請人姓名
            public string strApplyEmpID;            //申請人工號
            public string strApplyEmpRoleID;        //申請人RoleID
            public string strApplyEmpDeptName;      //申請人部門名稱
            public string strApplyEmpDeptID;         //申請人部門ID
            public int intApplyEmpDeptLevel;        //申請人部門Level
            public string strApplyEmpJobID;         //申請人職位ID
            public string strApplyEmpJobName;       //申請人職位名稱

            public string strStartEmpID;              //發起人工號
            public string strStartEmpRoleID;          //發起人RoleID

            public string strSignerEmpRoleID;         //簽核人的RoleID
            public string strSignerEmpID;             //簽核人的工號
            public int intSignerEmpDeptLevel;         //簽核人部門Level

            public string strSignOfTargetNodeName;  //簽核後下個節點的名稱

            public DateTime dateApplyDate;          //申請日期

            public string strLoginEmpID;            //登入者工號

            ///////////////////////////////////////////////////////////////////////////////////////
            ///

            public string strITManagerEmpID;          //資訊主管ID
            public string strRequestName;          //ApView or ApParm
            public bool blnCheckExecute;            //判斷現在是否為負責人

        }

        [Serializable]
        private struct stuDataTableList
        {
            public DataTable dtFileUpload;
            public DataTable dtEmployeeRoleList;
            public DataTable dtApplyEmpDevice;
            public DataTable dtDeviceList;
            public DataTable dtApplyContent;
            public DataTable dtItemList;
            public DataTable dtSqlBulkApplyContent;
            public DataTable AddToEmployeeItems;

            public DataTable dtHWInventory;
            public DataTable dtNewEmployee;
            public DataTable dtSqlBulkNewEmployee;
        }

        //  資服單用，DDlServiceType選項
        public enum ServiceType
        {
            新增,
            硬體維修,
            軟體維護,
            繳回設備,
            關閉權限
        }

        public string strRequireDate; //需求日期

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //  設定表單資訊-------------------------------------
                lblApplyDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
                strRequireDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
                
                stuFormInfo stuFormInfo = new stuFormInfo();

                stuFormInfo.strLoginEmpID = User.Identity.Name;

                ViewState["stuFormInfo"] = stuFormInfo;

                stuDataTableList stuDataTableList = new stuDataTableList();

                stuDataTableList.dtFileUpload = GetDtFileUpload();                      //  建立"附件上傳"的 TABLE
                stuDataTableList.dtApplyContent = GetDtApplyContent();                  //  建立"申請內容"的 TABLE
                stuDataTableList.dtDeviceList = GetDtDeviceList();                      //  建立 > 儲存現有設備整理出來的DataTable給"項目"的選單 TABLE
                stuDataTableList.dtItemList = GetDtItemList();                          //  建立"新增"的項目清單
                stuDataTableList.dtSqlBulkApplyContent = GetDtSqlBulkApplyContent();    // 建立要SqlBulk上傳"申請內容"的DataTable

                stuDataTableList.dtSqlBulkNewEmployee = GetSqlBulkNewEmployee();        //建立新人使用者資訊

                ViewState["stuDataTableList"] = stuDataTableList;

                if (!string.IsNullOrEmpty(stuFormInfo.strLoginEmpID))
                {
                    string strRequestName = Request.QueryString.AllKeys[0].ToString();
                    //  如果有apView或apParm則顯示為簽核畫面
                    if (strRequestName == "ApView" || strRequestName == "ApParm")
                    {
                        GetPnlCheck();
                        GetIT01SignInfo();
                    }

                    //  如果沒有，則顯示申請介面
                    else
                    {

                        //  取得發起人與申請人資訊
                        GetPersonInfo(stuFormInfo.strLoginEmpID);
                        //  取得資服單資訊
                        GetIT01ApplyInfo(true);


                        // 設定預設值
                        ddlSelectApplyEmp.SelectedValue = stuFormInfo.strLoginEmpID;
                        lblLoginEmpID.Text = stuFormInfo.strLoginEmpID;

                    }

                }


            }
            else
            {
                strRequireDate = txbRequireDate.Text;
            }
        }
        private DataTable GetSqlBulkNewEmployee()
        { 
            DataTable dtNewEmployee = new DataTable();
            dtNewEmployee.Columns.Add("ProcessID");
            dtNewEmployee.Columns.Add("UserID");
            dtNewEmployee.Columns.Add("UserName");

            return dtNewEmployee;
        }

        // 建立"附件上傳"的DataTable
        private DataTable GetDtFileUpload()
        {
            DataTable dtFileUpload = new DataTable();
            dtFileUpload.Columns.Add("FormCode");
            dtFileUpload.Columns.Add("FormName");
            dtFileUpload.Columns.Add("ProcessID");
            dtFileUpload.Columns.Add("FileName");       //  檔案名稱
            dtFileUpload.Columns.Add("ServerName");
            dtFileUpload.Columns.Add("FileType");       // 檔案類型
            dtFileUpload.Columns.Add("FileSize");       // 檔案大小
            dtFileUpload.Columns.Add("FileUploadDate"); // 上傳日期

            return dtFileUpload;
        }

        // 判斷並讀取簽核或檢視內容的view
        public void GetPnlCheck()
        {
            //  表單共用--------------------------------------------------------------------------

            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];

            ezEngineServices.Service oService = new ezEngineServices.Service(dcFlow.Connection);

            stuFormInfo.strRequestName = Request.QueryString.AllKeys[0];
            stuFormInfo.intApKey = int.Parse(Request[stuFormInfo.strRequestName]);

            switch (stuFormInfo.strRequestName)
            {
                case "ApView":
                    var rGetApView = oService.GetApView(stuFormInfo.intApKey);
                    if (rGetApView.ProcessFlow_id != 0)
                        stuFormInfo.intProcessID = rGetApView.ProcessFlow_id;

                    break;

                case "ApParm":

                    var rGetApParm = oService.GetApParm(stuFormInfo.intApKey);
                    if (rGetApParm.ProcessFlow_id != 0)
                        stuFormInfo.intProcessID = rGetApParm.ProcessFlow_id;

                    break;

                default:
                    break;
            }

            ///////////////////////////////////////////////////////////////

            //用ProcessID找出應該要簽核的人 並比對是否與登入的人一致
            var rProcessCheck = (from pn in dcFlow.ProcessNode
                                 join pc in dcFlow.ProcessCheck on pn.auto equals pc.ProcessNode_auto
                                 where pn.ProcessFlow_id == stuFormInfo.intProcessID
                                 && !pn.isFinish.GetValueOrDefault(true)
                                 orderby pn.adate descending
                                 select pc).FirstOrDefault();

            if (rProcessCheck != null)
            {

                var rRole = (from r in dcFlow.Role
                             join d in dcFlow.Dept on r.Dept_id equals d.id
                             where r.id == rProcessCheck.Role_idDefault
                             && r.Emp_id == rProcessCheck.Emp_idDefault
                             select new
                             {
                                 RoleId = r.id,
                                 EmpNobr = r.Emp_id,
                                 DeptTree = d.DeptLevel_id,
                             }).FirstOrDefault();

                if (rRole != null)
                {
                    if (stuFormInfo.intApKey != 0)
                    {
                        //寫回ApParm
                        var rProcessApParm = (from c in dcFlow.ProcessApParm
                                              where c.auto == stuFormInfo.intApKey
                                              && c.ProcessFlow_id == stuFormInfo.intProcessID
                                              && c.ProcessCheck_auto == rProcessCheck.auto
                                              && c.ProcessNode_auto == rProcessCheck.ProcessNode_auto
                                              select c).FirstOrDefault();

                        if (rProcessApParm != null && stuFormInfo.strRequestName == "ApParm")
                        {
                            rProcessApParm.Role_id = rProcessCheck.Role_idDefault;
                            rProcessApParm.Emp_id = rProcessCheck.Emp_idDefault;

                            dcFlow.SubmitChanges();
                           
                        }
                    }

                }
            }

            ///////////////////////////////////////////////////////////////

            txbRequireDate.Enabled = false;
            ddlSelectStartEmpDept.Visible = false;
            ddlSelectApplyEmp.Visible = false;
            pnlCheck.Visible = true;

            btnSend.Visible = false;
            btnSubmit.Visible = false;  //簽核按鈕
            btnReject.Visible = false;  //駁回按鈕


            string strWaitForSignerDept = "";       //等待簽核的人的部門
            string strWaitForSignerJobName = "";    //等待簽核的人的職稱
            string strWaitForSignerName = "";    //等待簽核的人的姓名

            //  StoredProcedure ---------------------------------------------
            string connectionString = ConfigurationManager.ConnectionStrings["ShareElecForm"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "spFormView";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@strProcessID", stuFormInfo.intProcessID);
                    cmd.Parameters.AddWithValue("@strEmpID", stuFormInfo.strLoginEmpID);

                    SqlDataAdapter da = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    da.SelectCommand = cmd;
                    da.Fill(ds);

                    //  表單共用部分------------------------------------------------------------

                    //  流程目前狀態為"簽核"或"駁回"
                    stuFormInfo.blnSign = (bool)ds.Tables[0].Rows[0]["FlowSign"];

                    //流程表單的資訊
                    stuFormInfo.strFormName = ds.Tables[0].Rows[0]["FormName"].ToString();
                    stuFormInfo.strFormID = ds.Tables[0].Rows[0]["FormID"].ToString();
                    stuFormInfo.strFormCode = ds.Tables[0].Rows[0]["FormCode"].ToString();

                    //取得此表單"申請日期"與"需求日期"並設定
                    DateTime dateApplyDate = DateTime.Parse(ds.Tables[0].Rows[0]["ApplyDate"].ToString());
                    lblApplyDate.Text = dateApplyDate.ToString("yyyy-MM-dd");
                    DateTime dateRequireDate = DateTime.Parse(ds.Tables[0].Rows[0]["RequireDate"].ToString());
                    strRequireDate = dateRequireDate.ToString("yyyy-MM-dd");
                    txbRequireDate.Text = strRequireDate;

                    //  發起人
                    lblStartEmpName.Text = ds.Tables[0].Rows[0]["StartEmpName"].ToString();
                    lblStartEmpDeptName.Text = ds.Tables[0].Rows[0]["StartEmpDeptName"].ToString();
                    lblStartEmpJobName.Text = ds.Tables[0].Rows[0]["StartEmpJobName"].ToString();

                    //選取的申請人
                    lblSelectApplyEmp.Text = ds.Tables[0].Rows[0]["ApplyEmpName"].ToString();

                    //  申請人
                    lblApplyEmpName.Text = ds.Tables[0].Rows[0]["ApplyEmpName"].ToString();
                    lblApplyEmpJobName.Text = ds.Tables[0].Rows[0]["ApplyEmpJobName"].ToString();
                    lblApplyEmpDeptName.Text = ds.Tables[0].Rows[0]["ApplyEmpDeptName"].ToString();

                    //到職日期
                    DateTime EmployeeStartDate = (DateTime)ds.Tables[0].Rows[0]["EmployeeStartDate"];
                    lblApplyEmpStartDate.Text = EmployeeStartDate.ToString("yyyy-MM-dd");


                    //  將需要用到的發起人、申請人資訊儲存到Struct
                    stuFormInfo.strApplyEmpID = ds.Tables[0].Rows[0]["ApplyEmpID"].ToString();
                    stuFormInfo.strApplyEmpRoleID = ds.Tables[0].Rows[0]["ApplyEmpRoleID"].ToString();
                    stuFormInfo.strApplyEmpName = ds.Tables[0].Rows[0]["ApplyEmpName"].ToString();
                    stuFormInfo.strApplyEmpJobName = ds.Tables[0].Rows[0]["ApplyEmpJobName"].ToString();
                    stuFormInfo.strApplyEmpDeptName = ds.Tables[0].Rows[0]["ApplyEmpDeptName"].ToString();
                    stuFormInfo.intApplyEmpDeptLevel = int.Parse(ds.Tables[0].Rows[0]["ApplyEmpDeptLevel"].ToString());

                    stuFormInfo.strStartEmpID = ds.Tables[0].Rows[0]["StartEmpID"].ToString();
                    stuFormInfo.strStartEmpRoleID = ds.Tables[0].Rows[0]["StartEmpRoleID"].ToString();

                    // 申請原因
                    txbApplyReason.Enabled = false;
                    txbApplyReason.Text = ds.Tables[0].Rows[0]["ApplyReason"].ToString();
                    if (string.IsNullOrEmpty(txbApplyReason.Text))
                        txbApplyReason.Text = "未填寫原因";
                   
                    if (ds.Tables[2].Rows.Count > 0)
                    {
                        // 現在要簽核的人的roleID跟EmpID
                        stuFormInfo.strSignerEmpRoleID = ds.Tables[2].Rows[0]["SignerEmpRoleID"].ToString();
                        stuFormInfo.strSignerEmpID = ds.Tables[2].Rows[0]["SignerEmpID"].ToString();
                        stuFormInfo.intSignerEmpDeptLevel = int.Parse(ds.Tables[2].Rows[0]["SignerEmpDeptLevel"].ToString());

                        //  顯示正在等待誰簽核的Label
                        strWaitForSignerDept = ds.Tables[2].Rows[0]["SignerEmpDeptName"].ToString();
                        strWaitForSignerJobName = ds.Tables[2].Rows[0]["SignerEmpJobName"].ToString();
                        strWaitForSignerName = ds.Tables[2].Rows[0]["SignerEmpName"].ToString();

                        lblStatus.Text = "等待 " + strWaitForSignerDept + strWaitForSignerJobName + strWaitForSignerName + " 簽核中";
                    }
                    else
                    {
                        lblStatus.Text = "此表單已結束審核";
                    }

                    // 上傳檔案 TABLE
                    stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];
                    stuDataTableList.dtFileUpload = ds.Tables[3];

                    //庫存硬體資料
                    stuDataTableList.dtHWInventory = ds.Tables[5];

                    //存回ViewState
                    ViewState["stuDataTableList"] = stuDataTableList;
                    ViewState["stuFormInfo"] = stuFormInfo;

                    //簽核者==登錄者 且 為ApPram時
                    if (stuFormInfo.strSignerEmpID == stuFormInfo.strLoginEmpID && stuFormInfo.strRequestName == "ApParm")
                    {
                        btnSubmit.Visible = true;
                        btnReject.Visible = true;
                    }

                    //  表單序號
                    lblProcessID.Text = stuFormInfo.intProcessID.ToString();
                    lblLoginEmpID.Text = stuFormInfo.strLoginEmpID;
                    lblLoginEmpName.Text = ds.Tables[4].Rows[0]["EmployeeName"].ToString();
                    lblElecForm.Text = stuFormInfo.strFormName;

                    //  綁定"附件上傳"
                    grvFileUpload.DataSource = stuDataTableList.dtFileUpload;
                    grvFileUpload.DataBind();

                    //  綁定簽核紀錄
                    grvFormSignM.DataSource = ds.Tables[1];
                    grvFormSignM.DataBind();

                    //綁定庫存硬體
                    ddlHWInventory.DataSource = stuDataTableList.dtHWInventory;
                    ddlHWInventory.DataBind();

                    
                }
            }
        }

        // 取得登入者的資訊
        protected void GetPersonInfo(string strLoginEmpID)
        {
            pnlCheck.Visible = false;
            pnlBtn.Visible = false;

            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];
            stuFormInfo.strFormCode = Request.QueryString.AllKeys[0];  //根據網址抓FormCode

            string connectionString = ConfigurationManager.ConnectionStrings["ShareElecForm"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spFormApply", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strEmployeeID", strLoginEmpID);
                cmd.Parameters.AddWithValue("@sFormCode", stuFormInfo.strFormCode);
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                da.SelectCommand = cmd;
                conn.Open();
                da.Fill(ds);

                // 取得登入者的RoleID有哪些
                DataTable dtStartEmpRoleList = ds.Tables[0];
                // dtStartEmpRoleList為發起人的部門清單(role清單)
                ddlSelectStartEmpDept.DataSource = dtStartEmpRoleList;
                ddlSelectStartEmpDept.DataBind();

                // 預設顯示第一筆結果的職稱、姓名到其他欄位，否則不會有其他欄位不會有值，需要等選擇部門才會顯示對應結果
                DataRow drUser = dtStartEmpRoleList.Rows[0];
                lblLoginEmpName.Text = (string)drUser["EmployeeName"];
                lblStartEmpName.Text = (string)drUser["EmployeeName"];
                lblStartEmpJobName.Text = (string)drUser["JobName"];


                stuFormInfo.strStartEmpRoleID = (string)drUser["RoleID"];
                stuFormInfo.strStartEmpID = (string)drUser["EmployeeID"];


                // 在SP判斷是否為主管，若為主管，獲得該角色的部門有哪些人可以選擇為申請人
                stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];
                stuDataTableList.dtEmployeeRoleList = ds.Tables[1];
                
                ddlSelectApplyEmp.DataSource = stuDataTableList.dtEmployeeRoleList;
                ddlSelectApplyEmp.DataBind();

                // 將"選擇申請人"預設為登入者(發起人)
                ddlSelectApplyEmp.SelectedValue = strLoginEmpID;


                stuFormInfo.strFormName = ds.Tables[2].Rows[0]["sFormName"].ToString();
                stuFormInfo.strFormID = ds.Tables[2].Rows[0]["sFlowTree"].ToString();
                lblElecForm.Text = stuFormInfo.strFormName;

                ViewState["stuFormInfo"] = stuFormInfo;

                //取得新進人員資訊
                stuDataTableList.dtNewEmployee = ds.Tables[3];
                ddlSelectUser.DataSource = stuDataTableList.dtNewEmployee;
                ddlSelectUser.DataBind();
                ViewState["stuDataTableList"] = stuDataTableList;

                // 依申請人工號，取得其姓名、職稱、部門
                getEmployeeInfo(stuDataTableList.dtEmployeeRoleList, strLoginEmpID);

                ds.Dispose();
                conn.Dispose();
                conn.Close();

            }


        }


        //  表單，取得發起人與申請人資訊，並儲存到Struct的viewState
        protected void getEmployeeInfo(DataTable dtEmployeeRoleList, string EmployeeID)
        {

            //DataRow drEmployee = dtEmployeeRoleList.Rows[ddlUserSelect.SelectedIndex];
            DataRow drEmployee = dtEmployeeRoleList.Select("EmployeeID='" + EmployeeID + "'")[0];

            lblApplyEmpName.Text = (string)drEmployee["EmployeeName"];
            lblApplyEmpJobName.Text = (string)drEmployee["JobName"];
            lblApplyEmpDeptName.Text = (string)drEmployee["DepartmentName"];

            DateTime EmployeeStartDate = (DateTime)drEmployee["EmployeeStartDate"];
            lblApplyEmpStartDate.Text = EmployeeStartDate.ToString("yyyy-MM-dd");

            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];

            if (string.IsNullOrEmpty(stuFormInfo.strSignerEmpRoleID))
                stuFormInfo.strSignerEmpRoleID = (string)drEmployee["RoleID"];

            stuFormInfo.strApplyEmpName = (string)drEmployee["EmployeeName"];
            stuFormInfo.strApplyEmpDeptName = (string)drEmployee["DepartmentName"];
            stuFormInfo.strApplyEmpDeptID = (string)drEmployee["DepartmentID"];
            stuFormInfo.strApplyEmpJobID = (string)drEmployee["JobID"];
            stuFormInfo.strApplyEmpJobName = (string)drEmployee["JobName"];
            stuFormInfo.strApplyEmpRoleID = (string)drEmployee["RoleID"];
            stuFormInfo.strApplyEmpID = (string)drEmployee["EmployeeID"];
            stuFormInfo.intApplyEmpDeptLevel = int.Parse((string)drEmployee["DeptLevel"]);

            ViewState["stuFormInfo"] = stuFormInfo;
        }

        protected void btnHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("Home.aspx");
        }

        // 發起人選擇"部門"
        protected void ddlSelectStartEmpDept_SelectedIndexChanged(object sender, EventArgs e)
        {
            string startEmpID = User.Identity.Name;
            string startEmpDept = ddlSelectStartEmpDept.SelectedValue;

            string connectionString = ConfigurationManager.ConnectionStrings["ming"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spFlowIT01FormApplyDDLDept", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strDepartmentID", startEmpDept);
                cmd.Parameters.AddWithValue("@strEmployeeID", startEmpID);
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                da.SelectCommand = cmd;
                conn.Open();
                da.Fill(ds);

                stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];
                stuDataTableList.dtEmployeeRoleList = ds.Tables[0];
                ViewState["stuDataTableList"] = stuDataTableList;
              
                DataRow drUser = stuDataTableList.dtEmployeeRoleList.Select("EmployeeID='" + startEmpID + "'")[0];

                lblStartEmpRoleID.Text = (string)drUser["RoleID"];
                lblStartEmpName.Text = (string)drUser["EmployeeName"];
                lblStartEmpJobName.Text = (string)drUser["JobName"];

                stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];
                stuFormInfo.strStartEmpRoleID = (string)drUser["RoleID"];
                stuFormInfo.strStartEmpID = (string)drUser["EmployeeID"];
                ViewState["stuFormInfo"] = stuFormInfo;

                ddlSelectApplyEmp.SelectedValue = startEmpID;
                getEmployeeInfo(stuDataTableList.dtEmployeeRoleList, startEmpID);

                ddlSelectApplyEmp.DataSource = stuDataTableList.dtEmployeeRoleList;
                ddlSelectApplyEmp.DataBind();
                //ddlUserSelect.Items.Insert(0, "請選擇對象");

            }

            //選擇新人使用者的綁定
            GetSelectUser(startEmpID, startEmpDept);
            //申請內容綁定
            GetApplyEmpDevice(ddlSelectApplyEmp.SelectedValue);

            if (rbtnlSelectWorking.SelectedValue == "Quit")
                SetResignGrvUserDevice("離職");
            else if (rbtnlSelectWorking.SelectedValue == "Transfer")
                SetResignGrvUserDevice("調職");

        }

        // 發起人"選擇申請人"
        protected void ddlSelectApplyEmp_SelectedIndexChanged(object sender, EventArgs e)
        {
            stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];
            getEmployeeInfo(stuDataTableList.dtEmployeeRoleList, ddlSelectApplyEmp.SelectedValue);

            if (rbtnlSelectWorking.SelectedValue == "Quit")
                SetResignGrvUserDevice("離職");
            else if (rbtnlSelectWorking.SelectedValue == "Transfer")
                SetResignGrvUserDevice("調職");
            else
            {
                GetApplyEmpDevice(ddlSelectApplyEmp.SelectedValue);

                stuDataTableList.dtApplyContent.Rows.Clear();
                grvApplyContent.DataSource = stuDataTableList.dtApplyContent;
                grvApplyContent.DataBind();
            }

            ddlServiceType.SelectedValue = ((int)ServiceType.新增).ToString(); ;
            ddlItemType.SelectedValue = "Mail";
            pnlItemList.Visible = false;
            ddlItemType.Visible = true;
            pnlMail.Visible = true;

        }

        protected void GetSelectUser(string startEmpID, string startEmpDept)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ShareElecForm"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spFormApplySelectUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strDepartmentID", startEmpDept);
                cmd.Parameters.AddWithValue("@strEmployeeID", startEmpID);
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                da.SelectCommand = cmd;
                conn.Open();
                da.Fill(ds);

                ddlSelectUser.DataSource = ds.Tables[0];
                ddlSelectUser.DataBind();
               
            }
        }

        // "附件上傳"的上傳按鈕
        protected void btnFileUpload_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                FileInfo uploadFile = new FileInfo(FileUpload1.PostedFile.FileName);
                int intFileSize = (FileUpload1.PostedFile.ContentLength / 1024);
                string strFileType = FileUpload1.PostedFile.ContentType.ToString();
                string strFileName = uploadFile.Name.ToString();
                string strFileUploadName = Guid.NewGuid().ToString() + uploadFile.Extension;

                // 將檔案上傳到伺服器
                string strPath = DateTime.Now.ToString("yyyyMM") + "/";
                string strServerPath = Server.MapPath("~/Upload/" + strPath);
                string strServerFilePath = Server.MapPath("~/Upload/" + strPath + strFileUploadName);

                if (!Directory.Exists(strServerPath))
                {
                    Directory.CreateDirectory(strServerPath);
                    FileUpload1.SaveAs(strServerFilePath);
                }
                else
                {
                    FileUpload1.SaveAs(strServerFilePath);
                }

                stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];

                DataRow drUpload;

                drUpload = stuDataTableList.dtFileUpload.NewRow();
                drUpload["FileName"] = strFileName;
                drUpload["FileType"] = strFileType;
                drUpload["FileSize"] = intFileSize;
                drUpload["FileUploadDate"] = DateTime.Now.ToString("yyyy/MM/dd   hh:mm:ss");
                drUpload["ServerName"] = strFileUploadName;
                stuDataTableList.dtFileUpload.Rows.Add(drUpload);

                ViewState["stuDataTableList"] = stuDataTableList;

                grvFileUpload.DataSource = stuDataTableList.dtFileUpload;
                grvFileUpload.DataBind();
            }
        }

        // "附件上傳"的下載按鈕
        protected void lbtnDownload_Command(object sender, CommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);

            stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];

            if (stuDataTableList.dtFileUpload != null && stuDataTableList.dtFileUpload.Rows.Count > 0)
            {
                DataRow row = stuDataTableList.dtFileUpload.Rows[rowIndex];

                string strFileName = row["FileName"].ToString();
                string strFileUploadName = row["ServerName"].ToString();
                string strFileUploadDate = row["FileUploadDate"].ToString();
                strFileUploadDate = DateTime.Now.ToString("yyyyMM") + "/";

                string strServerFilePath = Server.MapPath("~/Upload/" + strFileUploadDate + strFileUploadName);

                // 下載檔案
                Response.Clear();
                Response.ContentType = "application/octet-stream";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + strFileName);
                Response.WriteFile(strServerFilePath);
                Response.Flush();
                Response.End();
            }

        }


        // "附件上傳"的刪除按鈕
        protected void lbtnFileDelete_Command(object sender, CommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];

            stuDataTableList.dtFileUpload.Rows.RemoveAt(rowIndex);
            grvFileUpload.DataSource = stuDataTableList.dtFileUpload;
            grvFileUpload.DataBind();
        }

        // 送出按鈕
        protected void btnSend_Click(object sender, EventArgs e)
        {
            //  檢查有無輸入日期
            if (string.IsNullOrEmpty(txbRequireDate.Text))
            {
                Response.Write("<script>alert('" + "請選擇需求日期!" + "')</script>");
                return;
            }

            //  檢查有無申請內容
            if (grvApplyContent.Rows.Count<1)
            {
                Response.Write("<script>alert('" + "申請內容不能為空!" + "')</script>");
                return;
            }

            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];
         
            //  取得表單序號
            ezEngineServices.Service oService = new ezEngineServices.Service(dcFlow.Connection);
            stuFormInfo.intProcessID = oService.GetProcessID();
          
            SqlbulkCopyFileUpload(stuFormInfo.intProcessID);
            SqlBulkCopyApplyContent(stuFormInfo.intProcessID);
            SqlBulkCopyNemEmployee(stuFormInfo.intProcessID);

            var rFormAppInfo = new wfFormAppInfo();
            rFormAppInfo.idProcess = stuFormInfo.intProcessID;
            rFormAppInfo.sProcessID = rFormAppInfo.idProcess.ToString();
            rFormAppInfo.sNobr = stuFormInfo.strApplyEmpID;
            rFormAppInfo.sName = stuFormInfo.strApplyEmpName;
            rFormAppInfo.sState = "1";
            rFormAppInfo.sInfo =  stuFormInfo.strApplyEmpName + "，需求日期：" + txbRequireDate.Text;
            rFormAppInfo.sGuid = Guid.NewGuid().ToString();
            //lsFormAppInfo.Add(rFormAppInfo);
            dcFlow.wfFormAppInfo.InsertOnSubmit(rFormAppInfo);


            var rAppM = new wfFormApp();
            rAppM.sFormCode = stuFormInfo.strFormCode;
            rAppM.sFormName = "資服單";
            rAppM.sProcessID = stuFormInfo.intProcessID.ToString();
            rAppM.idProcess = stuFormInfo.intProcessID;
            rAppM.sNobr = stuFormInfo.strApplyEmpID;
            rAppM.sName = stuFormInfo.strApplyEmpName;
            rAppM.sDept = stuFormInfo.strApplyEmpDeptID;
            rAppM.sDeptName = stuFormInfo.strApplyEmpDeptName;
            rAppM.sJob = stuFormInfo.strApplyEmpJobID;
            rAppM.sJobName = stuFormInfo.strApplyEmpJobName;
            rAppM.sRole = stuFormInfo.strApplyEmpRoleID;
            rAppM.dDateTimeA = DateTime.Now;
            rAppM.dDateTimeD = DateTime.Parse(txbRequireDate.Text);
            rAppM.bSign = true;

            rAppM.sConditions1 = "60";
            rAppM.iCateOrder = stuFormInfo.intApplyEmpDeptLevel;
            rAppM.sLevel = stuFormInfo.intApplyEmpDeptLevel.ToString();

            rAppM.sInfo =  stuFormInfo.strApplyEmpName + "，需求日期：" + txbRequireDate.Text;
            rAppM.sNote = txbApplyReason.Text;
            rAppM.sState = "1";

            dcFlow.wfFormApp.InsertOnSubmit(rAppM);
            //dcForm.SubmitChanges();
            dcFlow.SubmitChanges();

            
            try
            {
                //  判斷發起人與申請人是否相同，不同的話則為代發起

                if (stuFormInfo.strApplyEmpRoleID != stuFormInfo.strStartEmpRoleID)
                {
                    oService.FlowStart(stuFormInfo.intProcessID, stuFormInfo.strFormID, stuFormInfo.strApplyEmpRoleID, stuFormInfo.strApplyEmpID, stuFormInfo.strStartEmpRoleID, stuFormInfo.strStartEmpID);
                }
                else
                {
                    oService.FlowStart(stuFormInfo.intProcessID, stuFormInfo.strFormID, stuFormInfo.strApplyEmpRoleID, stuFormInfo.strApplyEmpID, stuFormInfo.strApplyEmpRoleID, stuFormInfo.strApplyEmpID);
                }

                //寄信通知簽核者
                SendMail(stuFormInfo);

                Response.Redirect("Home.aspx");

            }
            catch (Exception )
            {
                Response.Write("<script>alert('" + "傳送失敗! " + "')</script>");
            }

        }

        //上傳新人資訊
        private void SqlBulkCopyNemEmployee(int intProcessID)
        {
            stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];

            DataRow drSqlBulkNewEmployee = stuDataTableList.dtSqlBulkNewEmployee.NewRow();

            drSqlBulkNewEmployee["ProcessID"] = intProcessID;
            drSqlBulkNewEmployee["UserID"] = ddlSelectUser.SelectedValue;
            drSqlBulkNewEmployee["UserName"] =ddlSelectUser.SelectedItem.Text;
            stuDataTableList.dtSqlBulkNewEmployee.Rows.Add(drSqlBulkNewEmployee);

            string connectionString = ConfigurationManager.ConnectionStrings["ShareElecForm"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    
                    bulkCopy.DestinationTableName = "IT01SelectedUser";
                    bulkCopy.ColumnMappings.Add("ProcessID", "iProcessID");
                    bulkCopy.ColumnMappings.Add("UserID", "sUserID");
                    bulkCopy.ColumnMappings.Add("UserName", "sUserName");

                    bulkCopy.WriteToServer(stuDataTableList.dtSqlBulkNewEmployee);
                }
            }
        }

        // "附件上傳"的SqlbulkCopy上傳資料庫
        private void SqlbulkCopyFileUpload(int intProcessID)
        {
            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];
            stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];

            foreach (DataRow drFileUpload in stuDataTableList.dtFileUpload.Rows)
            {
                drFileUpload["FormCode"] = stuFormInfo.strFormCode;
                drFileUpload["FormName"] = stuFormInfo.strFormName;
                drFileUpload["ProcessID"] = intProcessID;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["ming"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    // 檔案上傳
                    bulkCopy.DestinationTableName = "FileUpload";
                    bulkCopy.ColumnMappings.Add("FormCode", "FormCode");
                    bulkCopy.ColumnMappings.Add("FormName", "FormName");
                    bulkCopy.ColumnMappings.Add("ProcessID", "ProcessID");

                    bulkCopy.ColumnMappings.Add("FileName", "FileName");
                    bulkCopy.ColumnMappings.Add("FileSize", "Size");
                    bulkCopy.ColumnMappings.Add("FileUploadDate", "KeyDate");
                    bulkCopy.ColumnMappings.Add("ServerName", "ServerName");
                    bulkCopy.ColumnMappings.Add("FileType", "Type");
                    bulkCopy.WriteToServer(stuDataTableList.dtFileUpload);
                }
            }
        }


        // 簽核按鈕
        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            //  資服單條件------------------------------------
            if (ddlSelectExecuteEmp.Enabled == true && ddlSelectExecuteEmp.SelectedValue == "請選擇負責人")
            {
                lblError.Text = "簽核失敗，未指定負責人！";
                Response.Write("<script>alert('" + "簽核失敗，未指定負責人！" + "')</script>");

                return;
            }
            else if (txbTotalCost.Enabled == true && string.IsNullOrEmpty(txbTotalCost.Text))
            {
                Response.Write("<script>alert('" + "簽核失敗，未填寫預估費用！" + "')</script>");

                lblError.Text = "簽核失敗，未填寫預估費用！";
                return;
            }

            //  表單共用簽核按鈕部分------------------------------------
            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];
            string strProcessID = stuFormInfo.intProcessID.ToString();

            var rAppM = (from c in dcFlow.wfFormApp
                         where c.sProcessID == strProcessID
                         select c).FirstOrDefault();



            if (strProcessID.Trim().Length > 0 && stuFormInfo.intApKey != 0)
            {
                ezEngineServices.Service oService = new ezEngineServices.Service(dcFlow.Connection);

                if (stuFormInfo.intApKey != 0)
                {

                    rAppM.sNote = txbApplyReason.Text;
                    rAppM.bSign = true;
                    rAppM.sConditions1 = stuFormInfo.intSignerEmpDeptLevel.ToString();
                    rAppM.sState = !rAppM.bSign ? "2" : rAppM.sState;
                    //rAppM.dDateTimeD = DateTime.Now;
                    if (!string.IsNullOrEmpty(txbTotalCost.Text))
                        rAppM.sConditions2 = txbTotalCost.Text;

                    if (stuFormInfo.blnSign == false && stuFormInfo.strStartEmpID == User.Identity.Name && btnTake.Visible == true)
                        InsertFormSignM("重新送出申請", true);
                    else
                        InsertFormSignM("簽核", true);

                    //  寫入負責人
                    FlowApproveIT01();
                    //  流程推進
                    FlowApprove(true, 1);


                    if (stuFormInfo.intApKey != 0 && !string.IsNullOrEmpty(strProcessID))
                    {
                        //  資服單部分------------------------------------------------------------------------

                        // 當簽核人為 發起人/負責人 的時候，編輯申請內容並更新原本內容
                        if (stuFormInfo.strSignerEmpID == stuFormInfo.strStartEmpID || stuFormInfo.blnCheckExecute || stuFormInfo.strSignerEmpID == stuFormInfo.strITManagerEmpID)
                        {
                            // 將原先"申請內容"的table資料，deleteMark設為1
                            updatePreviousForm();

                            SqlbulkCopyFileUpload(stuFormInfo.intProcessID);
                            SqlBulkCopyApplyContent(stuFormInfo.intProcessID);


                            //  整理成要上傳"附件上傳"的table並儲存到要上傳的DataTable
                            //toBulkCopyFileUpload(formInfo.intRequestID);
                            ////  上傳整理好的"附件上傳"的DataTable到資料庫
                            //SqlbulkCopyFileUpload();
                            // 將"申請內容"的table資料寫到要sqlbulk的datatable
                            //toBulkCopyItemAdd(formInfo.intRequestID);
                            // 重新上傳"申請內容"的table資料，deleteMark預設為0
                            //SqlbulkCopyItemAdd();

                        }


                        // 當負責人已指定，且簽核者為申請人，將審核完的申請內容更新到現有設備
                        if (ddlSelectExecuteEmp.Enabled == false && stuFormInfo.strApplyEmpID == stuFormInfo.strSignerEmpID)
                        {
                            FinishToEmployeeItems();
                        }

                        //寄信通知簽核者
                        SendMail(stuFormInfo);

                        Response.Redirect("Home.aspx");
                    }
                    else
                        Response.Write("<script>alert('" + "簽核失敗!" + "')</script>");


                }
            }


        }

        //  寫入[wfFormSignM]程式(共用)
        public void InsertFormSignM(string strStateName, bool blnBSign)
        {
            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];

            var rEmpM = (from role in dcFlow.Role
                         join emp in dcFlow.Emp on role.Emp_id equals emp.id
                         join dept in dcFlow.Dept on role.Dept_id equals dept.id
                         join pos in dcFlow.Pos on role.Pos_id equals pos.id
                         where role.Emp_id == User.Identity.Name
                         select new
                         {
                             RoleId = role.id,
                             EmpNobr = emp.id,
                             EmpName = emp.name,
                             DeptCode = dept.id,
                             DeptName = dept.name,
                             JobCode = pos.id,
                             JobName = pos.name,
                             Auth = role.deptMg.Value,
                         }).FirstOrDefault();

            if (rEmpM != null)
            {
                var rSignM = (from c in dcFlow.wfFormSignM
                              where c.sProcessID == stuFormInfo.intProcessID.ToString()
                              && c.sKey == stuFormInfo.intApKey.ToString()
                              && c.sNobr == lblLoginEmpID.Text
                              select c).FirstOrDefault();

                if (rSignM == null)
                {
                    rSignM = new wfFormSignM();
                    dcFlow.wfFormSignM.InsertOnSubmit(rSignM);
                }

                rSignM.sFormCode = stuFormInfo.strFormCode;
                rSignM.sFormName = stuFormInfo.strFormName;
                rSignM.sKey = stuFormInfo.intApKey.ToString();
                rSignM.sProcessID = stuFormInfo.intProcessID.ToString();
                rSignM.idProcess = Convert.ToInt32(rSignM.sProcessID);
                rSignM.sNobr = rEmpM.EmpNobr;
                rSignM.sName = rEmpM.EmpName;
                rSignM.sRole = rEmpM.RoleId;
                rSignM.sDept = rEmpM.DeptCode;
                rSignM.sDeptName = rEmpM.DeptName;
                rSignM.sJob = rEmpM.JobCode;
                rSignM.sJobName = rEmpM.JobName;
                rSignM.sNote = "(" + strStateName + ")" + txbSignOpinion.Text;
                rSignM.bSign = blnBSign;
                rSignM.dKeyDate = DateTime.Now;

            }
            dcFlow.SubmitChanges();
            dcForm.SubmitChanges();
        }

        // 簽核程式，BSign為簽核與否，SState為狀態(1進行中，2駁回，7取消申請)
        private void FlowApprove(bool bSign, int intSState)
        {

            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];


            string connectionString = ConfigurationManager.ConnectionStrings["ShareElecForm"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string strQuery = "spFlowApprove";

                SqlCommand cmd = new SqlCommand(strQuery, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strProcessID", stuFormInfo.intProcessID.ToString());
                cmd.Parameters.AddWithValue("@strFormID", stuFormInfo.strFormID);
                cmd.Parameters.AddWithValue("@bSign", bSign);
                cmd.Parameters.AddWithValue("@sState", intSState);
                cmd.Parameters.AddWithValue("@strSignEmpRoleID", stuFormInfo.strSignerEmpRoleID);
                cmd.Parameters.AddWithValue("@strSignEmpID", stuFormInfo.strSignerEmpID);
                cmd.Parameters.AddWithValue("@intSignEmpDeptLevel", stuFormInfo.intSignerEmpDeptLevel);
                cmd.ExecuteNonQuery(); // 用於更新資料庫資料
                conn.Close();
            }

            ezEngineServices.Service oService = new ezEngineServices.Service(dcFlow.Connection);
            oService.WorkFinish(stuFormInfo.intApKey);
        }

        // 後續IT01所需寫入的流程內容
        private void FlowApproveIT01()
        {
            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];
            string strExecuteEmpID = "";
            string strExecuteEmpRoleID = "";

            if (ddlSelectExecuteEmp.Enabled == true && stuFormInfo.blnSign == true)
            {
                string[] strEmpIDandRoleID = ddlSelectExecuteEmp.SelectedValue.Split('/');
                strExecuteEmpID = strEmpIDandRoleID[0];
                strExecuteEmpRoleID = strEmpIDandRoleID[1];
            }

            string connectionString = ConfigurationManager.ConnectionStrings["ming"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string strQuery = "spFlowApproveIT01";

                SqlCommand cmd = new SqlCommand(strQuery, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strProcessID", stuFormInfo.intProcessID.ToString());
                cmd.Parameters.AddWithValue("@strFormID", stuFormInfo.strFormID);
                cmd.Parameters.AddWithValue("@blnSaveExecuteEmp", ddlSelectExecuteEmp.Enabled && stuFormInfo.blnSign == true);
                cmd.Parameters.AddWithValue("@strExecuteEmpID", strExecuteEmpID);
                cmd.Parameters.AddWithValue("@strExecuteEmpRoleID", strExecuteEmpRoleID);

                cmd.ExecuteNonQuery(); // 用於更新資料庫資料
                conn.Close();
            }
        }


        // 駁回按鈕
        protected void btnReject_Click(object sender, EventArgs e)
        {
            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];
            string strProcessID = stuFormInfo.intProcessID.ToString();
            if (stuFormInfo.intProcessID != 0)
            {

                FlowApprove(false, 1);
                InsertFormSignM("駁回", false);

                Response.Redirect("Home.aspx");
            }
        }

        // 取消申請按鈕
        protected void btnTake_Click(object sender, EventArgs e)
        {
            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];
            string strProcessID = stuFormInfo.intProcessID.ToString();
            if (stuFormInfo.intProcessID != 0)
            {
                ezEngineServices.Service oService = new ezEngineServices.Service(dcFlow.Connection);
                if (stuFormInfo.strStartEmpID == User.Identity.Name)
                {
                    FlowApprove(false, 7);
                    InsertFormSignM("取消申請", true);

                    Response.Write("<script>alert('" + "申請已取消! " + "')</script>");
                    Response.Redirect("Home.aspx");
                }
            }
        }



        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///

        //  取得資服單申請介面資訊
        protected void GetIT01ApplyInfo(bool blApplyBegin)
        {
            rbtnlSelectWorking.SelectedValue = "AtWork";

            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];
            stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];

            string connectionString = ConfigurationManager.ConnectionStrings["ming"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "spFlowIT01FormApply";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@strEmployeeID", stuFormInfo.strLoginEmpID);
                    cmd.ExecuteNonQuery();
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    da.SelectCommand = cmd;
                    da.Fill(ds);

                    // 是否為申請介面
                    if (blApplyBegin)
                    {
                        //  將申請人的現有設備表存到DataTabel與ViewState
                        stuDataTableList.dtApplyEmpDevice = ds.Tables[3];
                        //  綁定現有設備表
                        grvApplyEmpDevice.DataSource = stuDataTableList.dtApplyEmpDevice;
                        grvApplyEmpDevice.DataBind();

                        getApplyEmpDeviceItemName(stuDataTableList.dtApplyEmpDevice);

                    }

                    //  "新增"的設備類型清單
                    ddlItemType.DataSource = ds.Tables[2];
                    ddlItemType.DataBind();

                    //  "新增"的項目清單
                    ddlItemList.DataSource = ds.Tables[4];
                    ddlItemList.DataBind();


                }
            }
        }

        //  取得資服單審核介面資訊
        protected void GetIT01SignInfo()
        {
            pnlItemAdd.Visible = false;         //"申請內容"的PANEL
            grvFileUpload.Columns[3].Visible = false;   // 隱藏"附件上傳"的刪除
            pnlFileUpload.Visible = false;  //"附件上傳"


            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];
            stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];

            //  StoredProcedure ---------------------------------------------
            string connectionString = ConfigurationManager.ConnectionStrings["ming"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "spFlowIT01FormView";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@intProcessID", stuFormInfo.intProcessID);
                    cmd.Parameters.AddWithValue("@strEmpID", stuFormInfo.strLoginEmpID);
                    cmd.ExecuteNonQuery();
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    da.SelectCommand = cmd;
                    da.Fill(ds);

                    //  資服單部分------------------------------------------------------------

                    //  儲存資訊主管EmpID
                    stuFormInfo.strITManagerEmpID = ds.Tables[4].Rows[0]["EmployeeID"].ToString();

                    //  獲取這張表單的申請類型(在職申請/申請離職/調職)，並重新設定
                    string strApplyWorkType = ds.Tables[3].Rows[0]["ApplyWorkType"].ToString();
                    if (!string.IsNullOrEmpty(strApplyWorkType))
                        rbtnlSelectWorking.SelectedValue = strApplyWorkType;

                    rbtnlSelectWorking.Enabled = false;


                    stuDataTableList.dtApplyEmpDevice = ds.Tables[2];         //  將申請人的現有設備表存到DataTabel
                    stuDataTableList.dtApplyContent = ds.Tables[3];           //  讀取"申請內容"的DataTable
                    stuFormInfo.blnCheckExecute = (bool)ds.Tables[3].Rows[0]["ShowEditButton"];

                   
                    ViewState["stuDataTableList"] = stuDataTableList;

                    getApplyEmpDeviceItemName(stuDataTableList.dtApplyEmpDevice);

                    //選者使用者
                    ddlSelectUser.Visible = false;
                    lblSelectUser.Text = ds.Tables[0].Rows[0]["選擇使用者"].ToString();

                    //  顯示預估金額
                    txbTotalCost.Text = ds.Tables[0].Rows[0]["預估金額"].ToString();

                    //  判斷登入者 = 資訊主管 = 現在要簽核的人，顯示負責人選單並綁定
                    if (stuFormInfo.strITManagerEmpID == stuFormInfo.strLoginEmpID && stuFormInfo.strLoginEmpID == stuFormInfo.strSignerEmpID)
                    {
                        if (stuFormInfo.strRequestName == "ApParm")
                        {
                            if (ds.Tables[6].Rows.Count == 0)
                            {
                                pnlITManager.Visible = true;
                            }
                            pnlFileUpload.Visible = true;
                            txbTotalCost.Enabled = true;
                            ddlSelectExecuteEmp.Enabled = true;
                            ddlSelectExecuteEmp.DataSource = ds.Tables[5];
                            ddlSelectExecuteEmp.DataBind();
                        }
                        else 
                        {
                            if (ds.Tables[6].Rows.Count == 0)
                            {
                                pnlITManager.Visible = true;
                            }
                            txbTotalCost.Enabled = false;
                            ddlSelectExecuteEmp.Enabled = false;
                            txbSignOpinion.Enabled = false;
                            
                        }
                        
                    }

                    //  預設選單第一項為"請選擇負責人"
                    ddlSelectExecuteEmp.Items.Insert(0, "請選擇負責人");

                    //  當負責人已指定，獲取負責人"EmpIDandRoleID"後重新設定負責人選單並綁定，然後顯示為不可選
                    if (ds.Tables[6].Rows.Count > 0)
                    {
                        pnlITManager.Visible = true;
                        grvApplyContent.Columns[3].HeaderStyle.Width = new Unit("8%");
                        grvApplyContent.Columns[6].Visible = true;
                        grvApplyContent.Columns[7].Visible = true;

                        if (stuFormInfo.strSignerEmpID == stuFormInfo.strLoginEmpID)
                        {
                            ddlSelectExecuteEmp.Enabled = false;
                        }

                        ddlSelectExecuteEmp.DataSource = ds.Tables[5];
                        ddlSelectExecuteEmp.DataBind();
                        ddlSelectExecuteEmp.SelectedValue = ds.Tables[6].Rows[0]["EmpIDandRoleID"].ToString();
                    }

                    string flowNodeID = ds.Tables[0].Rows[0]["FlowNode_id"].ToString();

                    //  讀取負責人是否駁回，若負責人駁回則隱藏簽核按鈕
                    if (stuFormInfo.strSignerEmpID == stuFormInfo.strLoginEmpID && stuFormInfo.strRequestName == "ApParm")
                    {
                        btnSubmit.Visible = (bool)ds.Tables[10].Rows[0]["ExecuteEmpBSign"];

                        
                        if (stuFormInfo.strSignerEmpID == stuFormInfo.strStartEmpID && flowNodeID != "565" && flowNodeID != "557")
                        {
                            btnTake.Visible = true;
                            btnReject.Visible = false;
                        }
                        else 
                        {
                            btnTake.Visible = false;
                            btnReject.Visible = true;
                        }

                    }


                    // 如果 登入者 = 要簽核的人 = 發起人，且還沒指定負責人則可以編輯申請內容
                    if (stuFormInfo.strStartEmpID == stuFormInfo.strLoginEmpID && stuFormInfo.strSignerEmpID == stuFormInfo.strLoginEmpID && stuFormInfo.strRequestName == "ApParm" && flowNodeID != "565" && flowNodeID != "557")
                    {
                        grvFileUpload.Columns[3].Visible = true;   
                        pnlFileUpload.Visible = true;
                        grvApplyContent.Columns[7].Visible = true;


                        btnTake.Visible = true;
                        btnReject.Visible = false;

                        if (stuFormInfo.blnSign == false)
                            btnSubmit.Text = "重新送出申請";

                        if (strApplyWorkType == "AtWork")
                        {
                            pnlItemAdd.Visible = true;

                            if (btnSubmit.Visible)
                                grvApplyContent.Columns[1].Visible = true;
                            else
                            {
                                pnlItemAdd.Visible = false;
                                grvApplyContent.Columns[1].Visible = false;
                            }


                            GetIT01ApplyInfo(false);

                            for (int i = 0; i <= stuDataTableList.dtApplyContent.Rows.Count - 1; i++)
                            {
                                DataRow row = stuDataTableList.dtApplyContent.Rows[i];
                                removeItemList(row["ItemGuidKey"].ToString());
                            }

                        }
                    }

                    ViewState["stuFormInfo"] = stuFormInfo;

                    //  綁定現有設備表
                    grvApplyEmpDevice.DataSource = stuDataTableList.dtApplyEmpDevice;
                    grvApplyEmpDevice.DataBind();

                    //  綁定"申請內容"
                    grvApplyContent.DataSource = stuDataTableList.dtApplyContent;
                    grvApplyContent.DataBind();

                    ds.Dispose();
                    conn.Dispose();
                    conn.Close();
                }

            }
        }

        // 取得申請人現有項目表
        private void GetApplyEmpDevice(string strEmployeeID)
        {
            stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];


            string connectionString = ConfigurationManager.ConnectionStrings["ming"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spFlowIT01ApplyEmpDevice", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strEmployeeID", strEmployeeID);
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                da.SelectCommand = cmd;
                conn.Open();
                da.Fill(ds);

                stuDataTableList.dtApplyEmpDevice = ds.Tables[0];
                grvApplyEmpDevice.DataSource = stuDataTableList.dtApplyEmpDevice;
                grvApplyEmpDevice.DataBind();

                ViewState["stuDataTableList"] = stuDataTableList;

                //ddlItemType.Items.Insert(0, "請選擇種類");

                ds.Dispose();
                conn.Dispose();
                conn.Close();
            }

            //getItemListDataTable();
            //getGrvUserDeviceItemName();

            getApplyEmpDeviceItemName(stuDataTableList.dtApplyEmpDevice);
        }


        // 依據現有項目表，整理成"服務種類"DropDownList需要的DataTable
        private void getApplyEmpDeviceItemName(DataTable dtUserDevice)
        {
            stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];

            if (stuDataTableList.dtDeviceList.Rows.Count > 0)
                stuDataTableList.dtDeviceList.Rows.Clear();

            foreach (DataRow rowUserDevice in dtUserDevice.Rows)
            {
                DataRow drItem = stuDataTableList.dtDeviceList.NewRow();
                drItem["種類"] = rowUserDevice["種類"].ToString();
                drItem["項目"] = rowUserDevice["AssetsCode"].ToString() + "/" + rowUserDevice["種類"].ToString() + "/" + rowUserDevice["項目"].ToString() + "/" + rowUserDevice["名稱"].ToString();
                drItem["ItemGuidKey"] = rowUserDevice["GuidKey"].ToString();
                stuDataTableList.dtDeviceList.Rows.Add(drItem);
            }

            ViewState["stuDataTableList"] = stuDataTableList;

            initialItemList("新增");

        }

        // 初始化ddlServiceType，strServiceType為當下ddlServiceType的選項字串
        public void initialItemList(string strServiceType)
        {
            // 讀取從現有設備表整理出來的 DataTable > ViewState["stuDataTableList"]
            stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];

            DataTable dtItemList = (DataTable)ViewState["dtDeviceList"];

            ddlServiceType.Items.Clear();

            ddlServiceType.Items.Add(new ListItem(ServiceType.新增.ToString(), ((int)ServiceType.新增).ToString()));

            // 如果 dtItemList 裡種類有"硬體"的項目
            DataRow[] drItemType = stuDataTableList.dtDeviceList.Select(" 種類 = '硬體' ");
            if (drItemType.Count() > 0)
            {
                // ddlServiceType新增"硬體維修"、"繳回設備"這兩個選項
                ddlServiceType.Items.Add(new ListItem(ServiceType.硬體維修.ToString(), ((int)ServiceType.硬體維修).ToString()));
                ddlServiceType.Items.Add(new ListItem(ServiceType.繳回設備.ToString(), ((int)ServiceType.繳回設備).ToString()));
            }

            // 如果 dtItemList 裡種類有"軟體"的項目，ddlServiceType新增"軟體維護"選項
            drItemType = stuDataTableList.dtDeviceList.Select(" 種類 = '軟體' ");
            if (drItemType.Count() > 0)
                ddlServiceType.Items.Add(new ListItem(ServiceType.軟體維護.ToString(), ((int)ServiceType.軟體維護).ToString()));

            // 如果 dtItemList 裡種類有"系統權限"的項目，ddlServiceType新增"關閉權限"選項
            drItemType = stuDataTableList.dtDeviceList.Select(" 種類 = '系統權限' ");
            if (drItemType.Count() > 0)
                ddlServiceType.Items.Add(new ListItem(ServiceType.關閉權限.ToString(), ((int)ServiceType.關閉權限).ToString()));




            // 使用 Enum.Parse 方法將 字串strServiceType 轉換成 ServiceType 列舉型別的值
            ServiceType serviceType = (ServiceType)Enum.Parse(typeof(ServiceType), strServiceType, true);

            //  檢查ddlServiceType初始化後還有沒有目前選擇的該選項，false = 沒有項目
            bool itemExists = false;

            HashSet<string> serviceTypeValues = new HashSet<string>(ddlServiceType.Items.Cast<ListItem>().Select(item => item.Value));
            itemExists = serviceTypeValues.Contains(((int)serviceType).ToString());

            // 有的話則維持選擇該選項，沒有的話則變為預設選項"新增"
            if (!itemExists)
                ddlServiceType.SelectedValue = ((int)ServiceType.新增).ToString();
            else
                ddlServiceType.SelectedValue = ((int)serviceType).ToString();

            ServiceType st = (ServiceType)int.Parse(ddlServiceType.SelectedValue);

            // 當選項是"新增"的時候，顯示應該出現的Panel與DDL
            if (st == ServiceType.新增)
            {
                ddlItemType.SelectedValue = "Mail";
                pnlMail.Visible = true;
                ddlItemType.Visible = true;
                pnlItemList.Visible = false;
            }
        }

        //  建立"申請內容"的   TABLE
        protected DataTable GetDtApplyContent()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ServiceType");
            dt.Columns.Add("ItemType");
            dt.Columns.Add("ItemTypeName");
            dt.Columns.Add("ItemName");
            dt.Columns.Add("ItemGuidKey");
            dt.Columns.Add("AssetsCode");

            dt.Columns.Add("Description");
            dt.Columns.Add("ItemProcessing");

            dt.Columns.Add("ShowDeleteButton", typeof(bool), "1");
            dt.Columns.Add("ShowEditButton", typeof(bool), "0");

            return dt;

        }

        //  建立"項目"的選單，儲存現有設備整理出來的DataTable給"項目"的選單
        protected DataTable GetDtDeviceList()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("種類");
            dt.Columns.Add("項目");
            dt.Columns.Add("ItemGuidKey");

            return dt;
        }

        //  取得"新增"服務，對應種類產生項目清單的dataTable
        private DataTable GetDtItemList()
        {
            DataTable dt = new DataTable();

            string connectionString = ConfigurationManager.ConnectionStrings["ming"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spFlowIT01ItemList", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                da.SelectCommand = cmd;
                conn.Open();
                da.Fill(ds);

                dt = ds.Tables[0];

                ds.Dispose();
                conn.Dispose();
                conn.Close();

            }

            return dt;
        }

        // 建立要SqlBulk上傳"申請內容"的DataTable
        public DataTable GetDtSqlBulkApplyContent()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FormCode");
            dt.Columns.Add("ProcessID");
            dt.Columns.Add("sProcessID");
            dt.Columns.Add("ApplyDate");
            dt.Columns.Add("RequireDate");

            dt.Columns.Add("ApplyEmpID");
            dt.Columns.Add("ApplyEmpName");
            dt.Columns.Add("ApplyEmpDeptName");
            dt.Columns.Add("ApplyEmpJobName");
            dt.Columns.Add("Description");
            dt.Columns.Add("ServiceType");
            dt.Columns.Add("ItemType");
            dt.Columns.Add("ItemTypeName");
            dt.Columns.Add("ItemName");
            dt.Columns.Add("ItemGuidKey");
            dt.Columns.Add("AssetsCode");
            dt.Columns.Add("ItemProcessing");

            dt.Columns.Add("isFixed");
            dt.Columns.Add("isClose");
            dt.Columns.Add("isReturn");
            dt.Columns.Add("ApplyWorkType");

            dt.Columns.Add("StartEmpRoleID");
            dt.Columns.Add("ApplyEmpRoleID");

            dt.Columns.Add("sState");
            return dt;
        }


        //  表單，申請類型(含資服單)
        protected void rbtnlSelectWorking_SelectedIndexChanged(object sender, EventArgs e)
        {
            stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];

            if (rbtnlSelectWorking.SelectedValue == "AtWork")
            {
                grvApplyContent.ForeColor = System.Drawing.Color.Black;
                pnlItemAdd.Visible = true;
                txbApplyReason.Text = string.Empty;
                pnlApplyEmpDevice.Visible = true;

                GetApplyEmpDevice(ddlSelectApplyEmp.SelectedValue);

                stuDataTableList.dtApplyContent.Rows.Clear();
                grvApplyContent.DataSource = stuDataTableList.dtApplyContent;
                grvApplyContent.DataBind();

            }
            else if (rbtnlSelectWorking.SelectedValue == "Quit")
            {
                lblApplyContent.Text = "◆ 繳回申請人現有項目";
                pnlItemAdd.Visible = false;
                pnlApplyEmpDevice.Visible = false;
                SetResignGrvUserDevice("離職");
            }
            else if (rbtnlSelectWorking.SelectedValue == "Transfer")
            {
                lblApplyContent.Text = "◆ 繳回申請人現有項目";
                pnlItemAdd.Visible = false;
                pnlApplyEmpDevice.Visible = false;
                SetResignGrvUserDevice("調職");

            }
        }

        protected void ddlServiceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlItemList.Items.Clear();

            stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];

            pnlItemList.Visible = true;
            ddlItemType.Visible = false;
            pnlMail.Visible = false;

            string strSelectType = "";

            ServiceType st = (ServiceType)int.Parse(ddlServiceType.SelectedValue);
            switch (st)
            {
                case ServiceType.新增:
                    ddlItemType.SelectedValue = "Mail";
                    pnlItemList.Visible = false;
                    ddlItemType.Visible = true;
                    pnlMail.Visible = true;
                    break;
                case ServiceType.硬體維修:
                case ServiceType.繳回設備:
                    strSelectType = "硬體";
                    break;

                case ServiceType.關閉權限:
                    strSelectType = "系統權限";
                    break;
                case ServiceType.軟體維護:
                    strSelectType = "軟體";
                    break;
            }

            foreach (DataRow dr in stuDataTableList.dtDeviceList.Select("種類 = '" + strSelectType + "'"))
            {
                ddlItemList.Items.Add(new ListItem(dr["項目"].ToString(), dr["ItemGuidKey"].ToString()));
            }

        }

        //  "申請內容"裡的ddlItemType
        protected void ddlItemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlItemList.Items.Clear();

            if (ddlItemType.SelectedValue == "Mail")
            {
                pnlMail.Visible = true;
                pnlItemList.Visible = false;
            }
            else
            {
                pnlMail.Visible = false;
                pnlItemList.Visible = true;
            }

            stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];
            //DataView sortedView = new DataView(dtAddItemList);
            //sortedView.Sort = "ItemName ASC"; // Replace ColumnName with the actual column name you want to sort by

            //DataRow[] rowAddItemList = sortedView.ToTable().Select(" TypeID = '" + ddlItemType.SelectedValue + "'"); // Convert the sorted DataView back to DataRow[]

            DataRow[] rowAddItemList = stuDataTableList.dtItemList.Select(" TypeID = '" + ddlItemType.SelectedValue + "'");

            for (int i = 0; i < rowAddItemList.Length; i++)
            {
                DataRow rowItemList = rowAddItemList[i];
                ddlItemList.Items.Add(new ListItem(rowItemList["ItemName"].ToString(), rowItemList["ItemID"].ToString()));
            }

        }

        //  "申請內容"的項目選單
        protected void ddlItemList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlServiceType.SelectedValue == "新增" && ddlItemType.SelectedValue == "Mail")
                pnlMail.Visible = true;
            else
                pnlMail.Visible = false;

        }

        // 加入"申請內容"項目的按鈕
        protected void btnItemAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string strItemListText = "";
                if (ddlItemList.SelectedItem != null)
                    strItemListText = ddlItemList.SelectedItem.Text;
                string strServiceTypeText = ddlServiceType.SelectedItem.Text;


                // 除"新增"以外的ddlitemlist皆有斜線"/"，需取出值分別放在各個欄位裡

                // 寫入ItemAddDataTable
                stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];

                //判斷有無重複
                bool bolCheckItem = false;

                foreach (DataRow rowItemAdd in stuDataTableList.dtApplyContent.Rows)
                {
                    string strAddMail = txbApplyMail.Text + lblApplyMail.Text;
                    string strApplyMail = rowItemAdd["ItemName"].ToString();
                    string strAddItemType = ddlItemType.SelectedValue;

                    if (rowItemAdd["ItemName"].ToString() == strAddMail)
                    {
                        // false表示清單內無重複項目
                        if (pnlMail.Visible && strAddMail != strApplyMail)
                            bolCheckItem = false;
                        else if (pnlMail.Visible && strAddMail == strApplyMail)
                            bolCheckItem = true;
                    }
                }

                if (ddlServiceType.SelectedValue == ((int)ServiceType.新增).ToString() && pnlMail.Visible)
                {
                    if (string.IsNullOrEmpty(txbApplyMail.Text))
                    {
                        bolCheckItem = true;
                    }
                }
                else if (ddlServiceType.SelectedValue != ((int)ServiceType.新增).ToString() && string.IsNullOrEmpty(ddlItemList.SelectedValue))
                    bolCheckItem = true;

                if (!bolCheckItem)
                {

                    DataRow row = stuDataTableList.dtApplyContent.NewRow();

                    row["ServiceType"] = strServiceTypeText;
                    row["ItemType"] = ddlItemType.SelectedItem.Text;
                    row["ItemTypeName"] = strItemListText;
                    row["ItemGuidKey"] = Guid.NewGuid().ToString();

                    row["Description"] = txbItemAddReason.Text;
                    txbItemAddReason.Text = string.Empty;

                    row["ShowDeleteButton"] = true;
                    row["ShowEditButton"] = false;


                    if (ddlServiceType.SelectedValue != ((int)ServiceType.新增).ToString())
                    {
                        string[] strItemListArray = strItemListText.Split('/');

                        string strAssetsCode = strItemListArray[0];     //資產編號
                        string strItemType = strItemListArray[1];       //種類
                        string strItemTypeName = strItemListArray[2];   //項目
                        string strItemName = strItemListArray[3];       //名稱

                        row["AssetsCode"] = strAssetsCode;
                        row["ItemType"] = strItemType;
                        row["ItemTypeName"] = strItemTypeName;
                        row["ItemName"] = strItemName;
                        row["ItemGuidKey"] = ddlItemList.SelectedValue;
                    }

                    else if (ddlServiceType.SelectedValue == ((int)ServiceType.新增).ToString() && ddlItemType.SelectedItem.Text == "信箱")
                    {
                        string strAddMail = txbApplyMail.Text + lblApplyMail.Text;
                        if (!string.IsNullOrEmpty(txbApplyMail.Text))
                        {
                            //if (!blCheckMail(strAddMail))
                            //{
                                row["ItemTypeName"] = "Email";

                                row["ItemName"] = strAddMail;
                                lblMailCheck.Visible = false;
                            //}

                            //else
                            //{
                            //    lblMailCheck.Visible = true;
                            //    return;
                            //}
                        }

                    }


                    stuDataTableList.dtApplyContent.Rows.Add(row);
                }

                if (stuDataTableList.dtApplyContent != null)
                    grvApplyContent.Visible = true;

                stuDataTableList.dtApplyContent.DefaultView.Sort = "ServiceType,ItemType,ItemName desc";
                stuDataTableList.dtApplyContent = stuDataTableList.dtApplyContent.DefaultView.ToTable();
                ViewState["stuDataTableList"] = stuDataTableList;

                grvApplyContent.DataSource = stuDataTableList.dtApplyContent;
                grvApplyContent.DataBind();

                // 移除ddlItemList裡已新增的項目
                if (!string.IsNullOrEmpty(strItemListText) && strServiceTypeText != "新增")
                {
                    removeItemList(ddlItemList.SelectedValue);
                }

                txbApplyMail.Text = string.Empty;

                initialItemList(strServiceTypeText);
            }

        }

        //  檢查ddlItemList項目選單裡是否有跟"申請內容"一樣的項目，有則移除該選項
        public void removeItemList(string strGuidKey)
        {
            ddlItemList.DataValueField = "ItemGuidKey";
            ddlItemList.Items.Remove(ddlItemList.SelectedItem);

            stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];

            for (int i = stuDataTableList.dtDeviceList.Rows.Count - 1; i > -1; i--)
            {
                DataRow dr = stuDataTableList.dtDeviceList.Rows[i];
                if (dr["ItemGuidKey"].ToString() == strGuidKey)
                    stuDataTableList.dtDeviceList.Rows.RemoveAt(i);
            }

            ViewState["stuDataTableList"] = stuDataTableList;
        }

        // 自動帶出繳回設備TABLE
        private void SetResignGrvUserDevice(string strDescription)
        {
            GetApplyEmpDevice(ddlSelectApplyEmp.SelectedValue);
            grvApplyContent.ForeColor = System.Drawing.Color.Red;

            stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];
            stuDataTableList.dtApplyContent.Rows.Clear();

            foreach (DataRow rowUserDevice in stuDataTableList.dtApplyEmpDevice.Rows)
            {
                string strItemType = rowUserDevice["種類"].ToString();
                string strItemTypeName = rowUserDevice["項目"].ToString();
                string strItemName = rowUserDevice["名稱"].ToString();
                string strGuidKey = rowUserDevice["GuidKey"].ToString();
                string strAssetsCode = rowUserDevice["AssetsCode"].ToString();

                DataRow row = stuDataTableList.dtApplyContent.NewRow();

                row["ItemTypeName"] = strItemTypeName;
                row["ItemType"] = strItemType;
                row["ItemName"] = strItemName;
                row["ItemGuidKey"] = strGuidKey;
                row["AssetsCode"] = strAssetsCode;
                row["Description"] = strDescription;

                row["ShowDeleteButton"] = false;
                row["ShowEditButton"] = false;

                if (strItemType != "硬體")
                    row["ServiceType"] = "關閉權限";
                else
                    row["ServiceType"] = "繳回設備";

                stuDataTableList.dtApplyContent.Rows.Add(row);
            }

            stuDataTableList.dtApplyContent.DefaultView.Sort = "ServiceType,ItemType,ItemName desc";
            grvApplyContent.DataSource = stuDataTableList.dtApplyContent;
            grvApplyContent.DataBind();
            ViewState["stuDataTableList"] = stuDataTableList;

        }


        // "申請內容"裡，確認信箱是否已被申請
        public bool blCheckMail(string strAddMail)
        {
            bool blCheckMail = false;
            int intCountMail;

            string connectionString = ConfigurationManager.ConnectionStrings["Flow"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spFlowIT01FormCheckEmail", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strAddEmail", strAddMail);
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                da.SelectCommand = cmd;
                conn.Open();
                da.Fill(ds);

                intCountMail = (int)ds.Tables[0].Rows[0]["CountMail"];

                if (intCountMail > 0)
                    blCheckMail = true;
                else
                    blCheckMail = false;
            }

            return blCheckMail;
        }

        //將庫存內容寫入
        protected void btnHWInventory_Click(object sender, EventArgs e)
        {
            string HWInventory = ddlHWInventory.SelectedItem.Text.ToString();
            string[] Inventory = HWInventory.Split('/');
            txbEditAssetCode.Text = Inventory[0];
            txbEditItemName.Text = Inventory[1];
            
            
        }

        //編輯內容
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            int selectedIndex = grvApplyContent.SelectedIndex;

            if (selectedIndex > -1)
            {
                pnlHWInventory.Visible = false;
                pnlItemEdit.Visible = false;

                string strAssetsCode = txbEditAssetCode.Text;
                string strItemName = txbEditItemName.Text;
                string strProcessing = txbEditProcessing.Text;

                GridViewRow selectedRow = grvApplyContent.Rows[selectedIndex];

                stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];

                if (!string.IsNullOrEmpty(txbEditItemName.Text))
                    stuDataTableList.dtApplyContent.Rows[selectedIndex]["ItemName"] = txbEditItemName.Text;

                if (!string.IsNullOrEmpty(txbEditAssetCode.Text))
                    stuDataTableList.dtApplyContent.Rows[selectedIndex]["AssetsCode"] = txbEditAssetCode.Text;

                if (!string.IsNullOrEmpty(txbEditProcessing.Text))
                    stuDataTableList.dtApplyContent.Rows[selectedIndex]["ItemProcessing"] = txbEditProcessing.Text;

                string strGuidKey = stuDataTableList.dtApplyContent.Rows[selectedIndex]["ItemGuidKey"].ToString();


                ViewState["stuDataTableList"] = stuDataTableList;

                stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];

                string connectionString = ConfigurationManager.ConnectionStrings["ming"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spFlowIT01UpdateContent", conn);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@strGuidKey", strGuidKey);
                    cmd.Parameters.AddWithValue("@strAssetsCode", strAssetsCode);
                    cmd.Parameters.AddWithValue("@strItemName", strItemName);
                    cmd.Parameters.AddWithValue("@strItemProcessing", strProcessing);
                    cmd.Parameters.AddWithValue("@intProcessID", stuFormInfo.intProcessID);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    SqlDataAdapter da = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    da.SelectCommand = cmd;
                    da.Fill(ds);

                    stuDataTableList.dtApplyContent = ds.Tables[0];
                    ViewState["stuDataTableList"] = stuDataTableList;

                    grvApplyContent.DataSource = stuDataTableList.dtApplyContent;
                    grvApplyContent.DataBind();

                }
            }
        }

        // "申請內容"的刪除按鈕
        protected void lbtnDelete_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "ItemDelete")
            {
                string strServiceType = "";
                string strItemType = "";
                string strItemTypeName = "";
                string strItemName = "";
                string strDDLItemName = "";
                string strItemGuidKey = "";
                string strAssetsCode = "";


                int rowIndex = Convert.ToInt32(e.CommandArgument);

                stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];

                DataRow deletedRow = stuDataTableList.dtApplyContent.Rows[rowIndex];

                strServiceType = deletedRow["ServiceType"].ToString();
                strItemType = deletedRow["ItemType"].ToString();
                strItemTypeName = deletedRow["ItemTypeName"].ToString();
                strItemName = deletedRow["ItemName"].ToString();
                strItemGuidKey = deletedRow["ItemGuidKey"].ToString();
                strAssetsCode = deletedRow["AssetsCode"].ToString();

                strDDLItemName = strAssetsCode + "/" + strItemType + "/" + strItemTypeName + "/" + strItemName;


                bool blnCheckItem = false;

                if (strServiceType != "新增")
                {
                    for (int i = stuDataTableList.dtDeviceList.Rows.Count - 1; i > -1; i--)
                    {
                        DataRow dr = stuDataTableList.dtDeviceList.Rows[i];
                        if (dr["ItemGuidKey"].ToString() == strItemGuidKey)
                            blnCheckItem = true;
                    }

                    if (blnCheckItem == false)
                    {
                        DataRow drItem = stuDataTableList.dtDeviceList.NewRow();
                        drItem["種類"] = strItemType;
                        drItem["項目"] = strDDLItemName;
                        drItem["ItemGuidKey"] = strItemGuidKey;
                        stuDataTableList.dtDeviceList.Rows.Add(drItem);
                    }

                    ViewState["stuDataTableList"] = stuDataTableList;

                    if (ddlServiceType.SelectedValue != ServiceType.新增.ToString())
                    {
                        // 當不是移除"新增"服務時，則重新加入此項目到DDL選項中(dtItemList)
                        ddlItemList.Items.Clear();
                        foreach (DataRow dr in stuDataTableList.dtDeviceList.Select("種類 = '" + strItemType + "'"))
                        {
                            ddlItemList.Items.Add(new ListItem(dr["項目"].ToString(), dr["ItemGuidKey"].ToString()));
                        }

                        //根據移除的服務種類，自動變更ddl的選項
                        initialItemList(strServiceType);
                        if (strServiceType != "新增")
                        {
                            pnlMail.Visible = false;
                            ddlItemType.Visible = false;
                            pnlItemList.Visible = true;
                        }

                    }
                }

                stuDataTableList.dtApplyContent.Rows.RemoveAt(rowIndex);
                grvApplyContent.DataSource = stuDataTableList.dtApplyContent;
                grvApplyContent.DataBind();

            }
        }

        //  "申請內容"，負責人編輯按鈕
        protected void lnkSelect_Click(object sender, EventArgs e)
        {
            LinkButton lnkSelect = (LinkButton)sender;
            GridViewRow gridViewRow = (GridViewRow)lnkSelect.NamingContainer;

            foreach (GridViewRow row in grvApplyContent.Rows)
            {
                row.ForeColor = System.Drawing.Color.Black;
            }

            gridViewRow.ForeColor = System.Drawing.Color.Red;


            // 確認選取的是資料列
            if (gridViewRow.RowType == DataControlRowType.DataRow)
            {
                pnlHWInventory.Visible = true;
                pnlItemEdit.Visible = true;

                // 獲取選取的資料列的索引
                int rowIndex = gridViewRow.RowIndex;
                string strServiceType, strItemType, strItemName, strAssetsCode, strItemProcessing;

                // 獲取 GridView 的資料來源 DataTable
                stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];

                DataRow selectedRow = stuDataTableList.dtApplyContent.Rows[rowIndex];

                // 獲取選取的資料列
                strServiceType = selectedRow["ServiceType"].ToString();
                strItemType = selectedRow["ItemType"].ToString();

                strItemName = selectedRow["ItemName"].ToString();
                strAssetsCode = selectedRow["AssetsCode"].ToString();
                strItemProcessing = selectedRow["ItemProcessing"].ToString();


                txbEditItemName.Text = strItemName;
                txbEditAssetCode.Text = strAssetsCode;
                txbEditProcessing.Text = strItemProcessing;
            }
        }

        // "申請內容"Gridview的GridViewRowEventArgs
        protected void grvItemAdd_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // 获取当前行的数据项
                LinkButton lnkSelect = (LinkButton)e.Row.FindControl("lnkSelect");
                DataRowView rowView = (DataRowView)e.Row.DataItem;
                lnkSelect.CommandArgument = rowView["ItemName"].ToString();

                if (rbtnlSelectWorking.SelectedValue == "Quit" || rbtnlSelectWorking.SelectedValue == "Transfer")
                {
                    LinkButton lbtnDelete = (LinkButton)e.Row.FindControl("lbtnDelete");
                    lbtnDelete.Visible = false;
                    System.Web.UI.WebControls.Label lblItemAddReason = e.Row.FindControl("lblItemAddReason") as System.Web.UI.WebControls.Label;
                }
            }
        }


        //  將原本的"申請內容"DataTable整理成上傳需要的格式，並儲存到DataTable
        public void SqlBulkCopyApplyContent(int intRequestID)
        {
            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];

            stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];

            //  寫入要上傳到資料庫"申請內容" TABLE 所需要的資訊
            foreach (DataRow rowdtItemAdd in stuDataTableList.dtApplyContent.Rows)
            {
                string strServiceType = rowdtItemAdd["ServiceType"].ToString();
                string strItemType = rowdtItemAdd["ItemType"].ToString();
                string strItemTypeName = rowdtItemAdd["ItemTypeName"].ToString();
                string strItemName = rowdtItemAdd["ItemName"].ToString();
                string strDescription = rowdtItemAdd["Description"].ToString();
                string strItemGuidKey = rowdtItemAdd["ItemGuidKey"].ToString();
                string strAssetsCode = rowdtItemAdd["AssetsCode"].ToString();
                string strItemProcessing = rowdtItemAdd["ItemProcessing"].ToString();

                DataRow newRow = stuDataTableList.dtSqlBulkApplyContent.NewRow();
                newRow["FormCode"] = stuFormInfo.strFormCode;
                newRow["ProcessID"] = intRequestID;
                newRow["sProcessID"] = intRequestID;

                if (!string.IsNullOrEmpty(txbRequireDate.Text))
                    newRow["RequireDate"] = txbRequireDate.Text;
                else
                    newRow["RequireDate"] = txbRequireDate.Text;

                newRow["StartEmpRoleID"] = stuFormInfo.strStartEmpRoleID;

                newRow["ApplyEmpID"] = stuFormInfo.strApplyEmpID;
                newRow["ApplyEmpName"] = stuFormInfo.strApplyEmpName;
                newRow["ApplyEmpDeptName"] = stuFormInfo.strApplyEmpDeptName;
                newRow["ApplyEmpJobName"] = stuFormInfo.strApplyEmpJobName;
                newRow["ApplyEmpRoleID"] = stuFormInfo.strApplyEmpRoleID;
                newRow["ApplyWorkType"] = rbtnlSelectWorking.SelectedValue;

                newRow["sState"] = "1";
                newRow["ServiceType"] = strServiceType;
                newRow["ItemType"] = strItemType;
                newRow["ItemTypeName"] = strItemTypeName;
                newRow["ItemName"] = strItemName.Trim();
                newRow["ItemGuidKey"] = strItemGuidKey;
                newRow["AssetsCode"] = strAssetsCode;
                newRow["Description"] = strDescription;
                newRow["ItemProcessing"] = strItemProcessing;


                if (!string.IsNullOrEmpty(stuFormInfo.strApplyEmpRoleID) && !string.IsNullOrEmpty(stuFormInfo.strStartEmpRoleID))
                {
                    newRow["StartEmpRoleID"] = stuFormInfo.strStartEmpRoleID;
                    newRow["ApplyEmpRoleID"] = stuFormInfo.strApplyEmpRoleID;
                }

                stuDataTableList.dtSqlBulkApplyContent.Rows.Add(newRow);
            }


            //  上傳資料庫
            string connectionString = ConfigurationManager.ConnectionStrings["ming"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {

                    bulkCopy.DestinationTableName = "formIT01";

                    bulkCopy.ColumnMappings.Add("FormCode", "FormCode");
                    bulkCopy.ColumnMappings.Add("ProcessID", "ProcessID");
                    bulkCopy.ColumnMappings.Add("sProcessID", "sProcessID");

                    bulkCopy.ColumnMappings.Add("ApplyEmpID", "ApplyEmpID");
                    bulkCopy.ColumnMappings.Add("ApplyEmpName", "ApplyEmpName");
                    bulkCopy.ColumnMappings.Add("ApplyEmpDeptName", "ApplyEmpDeptName");
                    bulkCopy.ColumnMappings.Add("ApplyEmpJobName", "ApplyEmpJobName");
                    bulkCopy.ColumnMappings.Add("ApplyEmpRoleID", "ApplyEmpRoleID");
                    bulkCopy.ColumnMappings.Add("StartEmpRoleID", "StartEmpRoleID");

                    bulkCopy.ColumnMappings.Add("ApplyDate", "ApplyDate");
                    bulkCopy.ColumnMappings.Add("RequireDate", "RequireDate");

                    bulkCopy.ColumnMappings.Add("Description", "Description");
                    bulkCopy.ColumnMappings.Add("sState", "sState");

                    bulkCopy.ColumnMappings.Add("ServiceType", "ServiceType");
                    bulkCopy.ColumnMappings.Add("ItemType", "ItemType");
                    bulkCopy.ColumnMappings.Add("ItemTypeName", "ItemTypeName");
                    bulkCopy.ColumnMappings.Add("ItemName", "ItemName");
                    bulkCopy.ColumnMappings.Add("ItemGuidKey", "ItemGuidKey");
                    bulkCopy.ColumnMappings.Add("AssetsCode", "AssetsCode");
                    bulkCopy.ColumnMappings.Add("ItemProcessing", "ItemProcessing");

                    bulkCopy.ColumnMappings.Add("ApplyWorkType", "ApplyWorkType");

                    bulkCopy.WriteToServer(stuDataTableList.dtSqlBulkApplyContent);
                }
            }
        }

        //  發起人重新編輯"申請內容"送出時，update資料庫裡之前紀錄資服單"申請內容"的DeleteMark程式
        public void updatePreviousForm()
        {
            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];

            string connectionString = ConfigurationManager.ConnectionStrings["ming"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "update formIT01 set [DeleteMark] = @DeleteMark where sProcessID = @sProcessID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DeleteMark", 1);
                    cmd.Parameters.AddWithValue("@sProcessID", stuFormInfo.intProcessID);
                    cmd.ExecuteNonQuery(); // 用於更新資料庫資料
                }

                query = "UPDATE FileUpload SET [DeleteMark] = @DeleteMark WHERE ProcessID = @ProcessID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Clear(); // 清除之前的參數

                    cmd.Parameters.AddWithValue("@DeleteMark", 1);
                    cmd.Parameters.AddWithValue("@ProcessID", stuFormInfo.intProcessID);

                    cmd.ExecuteNonQuery(); // 更新資料庫資料
                }
                conn.Dispose();
                conn.Close();
            }

        }


        //  資服單，當流程結束，更新"申請內容"申請項目到現有設備表
        public void FinishToEmployeeItems()
        {
            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];
            stuDataTableList stuDataTableList = (stuDataTableList)ViewState["stuDataTableList"];

            stuDataTableList.AddToEmployeeItems = stuDataTableList.dtApplyContent.Clone();

            //用list存取庫存硬體的資產編號
            List<string> assetsCodeList = new List<string>(); 
            foreach (DataRow drHWInventory in stuDataTableList.dtHWInventory.Rows)
            {
                string assetsCode = drHWInventory["AssetsCode"].ToString();
                assetsCodeList.Add(assetsCode);
            }


            string connectionString = ConfigurationManager.ConnectionStrings["ming"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            foreach (DataRow drSqlBulkApplyContent in stuDataTableList.dtApplyContent.Rows)
            {
                string strServiceType = drSqlBulkApplyContent["ServiceType"].ToString();
                string strItemGuidKey = drSqlBulkApplyContent["ItemGuidKey"].ToString();

                string strAssetsCode = drSqlBulkApplyContent["AssetsCode"].ToString();
                string strApplyEmpName = drSqlBulkApplyContent["ApplyEmpName"].ToString();
                string strApplyEmpDeptName = drSqlBulkApplyContent["ApplyEmpDeptName"].ToString();
                string strApplyEmpID = drSqlBulkApplyContent["ApplyEmpID"].ToString();
                string strItemType = drSqlBulkApplyContent["ItemType"].ToString();
                string strItemTypeName = drSqlBulkApplyContent["ItemTypeName"].ToString();
                string strItemName = drSqlBulkApplyContent["ItemName"].ToString();

                SqlCommand cmd = new SqlCommand("", conn); // 初始化 SqlCommand 物件

                switch (strServiceType)
                {
                    case "新增":
                        if (strItemTypeName != "電腦周邊")
                        {
                            DataRow newRow = stuDataTableList.AddToEmployeeItems.NewRow();
                            newRow["AssetsCode"] = strAssetsCode;
                            newRow["ApplyEmpName"] = strApplyEmpName;
                            newRow["ApplyEmpDeptName"] = strApplyEmpDeptName;
                            newRow["ApplyEmpID"] = strApplyEmpID;
                            newRow["ItemType"] = strItemType;
                            newRow["ItemTypeName"] = strItemTypeName;
                            newRow["ItemName"] = strItemName;

                            stuDataTableList.AddToEmployeeItems.Rows.Add(newRow);

                            //利用資產編號比對新增的硬體是不是庫存，是的話就把該筆資料做deletMark
                            bool isExists = assetsCodeList.Contains(strAssetsCode);
                            if (isExists) 
                            {
                                cmd.CommandText = "update IT01EmployeeItems set deleteMark = @deleteMark where  AssetsCode = @AssetsCode";
                                cmd.Parameters.AddWithValue("@deleteMark", true);
                                cmd.Parameters.AddWithValue("@AssetsCode", strAssetsCode);
                                cmd.ExecuteNonQuery(); // 用於更新資料庫資料
                            }
                        }
                        break;

                    case "繳回設備":
                    case "關閉權限":
                        cmd.CommandText = "update IT01EmployeeItems set isReturn = @isReturn where GuidKey = @GuidKey";
                        cmd.Parameters.AddWithValue("@isReturn", true);
                        cmd.Parameters.AddWithValue("@GuidKey", strItemGuidKey);
                        cmd.ExecuteNonQuery(); // 用於更新資料庫資料
                        break;

                    case "硬體維修":
                        foreach (DataRow drUserDevice in stuDataTableList.dtApplyEmpDevice.Select("GuidKey ='" + strItemGuidKey + "'"))
                        {
                            string strUserDeviceAssetsCode = drUserDevice["AssetsCode"].ToString();

                            if (strUserDeviceAssetsCode != strAssetsCode)
                            {
                                cmd.CommandText = "update IT01EmployeeItems set AssetsCode = @AssetsCode,AssetsName = @AssetsName where GuidKey = @GuidKey";
                                cmd.Parameters.AddWithValue("@AssetsCode", strAssetsCode);
                                cmd.Parameters.AddWithValue("@AssetsName", strItemName);
                                cmd.Parameters.AddWithValue("@GuidKey", strItemGuidKey);
                                cmd.ExecuteNonQuery(); // 用於更新資料庫資料
                            }
                        }
                        break;

                    case "軟體維護":
                        foreach (DataRow drUserDevice in stuDataTableList.dtApplyEmpDevice.Select("GuidKey ='" + strItemGuidKey + "'"))
                        {
                            string strUserDeviceItemName = drUserDevice["名稱"].ToString();

                            if (strUserDeviceItemName != strItemName)
                            {
                                cmd.CommandText = "update IT01EmployeeItems set AssetsName = @AssetsName where GuidKey = @GuidKey";
                                cmd.Parameters.AddWithValue("@AssetsName", strItemName);
                                cmd.Parameters.AddWithValue("@GuidKey", strItemGuidKey);
                                cmd.ExecuteNonQuery(); // 用於更新資料庫資料
                            }
                        }
                        break;
                }
            }

            SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString);
            // 硬體設備單
            bulkCopy.DestinationTableName = "IT01EmployeeItems"; // 替換成目標資料表的名稱

            bulkCopy.ColumnMappings.Add("AssetsCode", "AssetsCode");
            bulkCopy.ColumnMappings.Add("ApplyEmpName", "UserName");
            bulkCopy.ColumnMappings.Add("ApplyEmpDeptName", "DeptName");
            bulkCopy.ColumnMappings.Add("ApplyEmpID", "Nobr");
            bulkCopy.ColumnMappings.Add("ItemType", "ItemType");
            bulkCopy.ColumnMappings.Add("ItemTypeName", "ItemName");
            bulkCopy.ColumnMappings.Add("ItemName", "AssetsName");

            bulkCopy.WriteToServer(stuDataTableList.AddToEmployeeItems);

            conn.Dispose();
            conn.Close();
        }

        //寄email
        private void SendMail(stuFormInfo stuFormInfo)
        {           
            try
            {
                string strSingerEmail;//簽核者信箱

                string connectionString = ConfigurationManager.ConnectionStrings["ShareElecForm"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "spFormView";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@strProcessID", stuFormInfo.intProcessID);
                        cmd.Parameters.AddWithValue("@strEmpID", stuFormInfo.strLoginEmpID);

                        SqlDataAdapter da = new SqlDataAdapter();
                        DataSet ds = new DataSet();
                        da.SelectCommand = cmd;
                        da.Fill(ds);
                        
                        if (string.IsNullOrEmpty(ds.Tables[2].Rows[0]["SignerEmail"].ToString()))//如果為空值的話代表流程即將結束，不會有下一個簽核者信箱
                        {
                            strSingerEmail = "hiss.it@hiss.com.tw";//因為strSingerEmail不能為空，所以預設hiss.it@hiss.com.tw
                        }
                        else 
                        {
                            strSingerEmail = ds.Tables[2].Rows[0]["SignerEmail"].ToString(); //下一個簽核者信箱
                        }
                    }
                }

                //創建一個電子郵件
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("hiss.it@hiss.com.tw"); //發
                mail.To.Add(strSingerEmail);//收
                mail.Subject = "【通知】("+ stuFormInfo.strApplyEmpID + ")"+stuFormInfo.strApplyEmpName+"之"+stuFormInfo.strFormName;//標題
                //內文
                mail.Body = "您有一筆"+ stuFormInfo.strFormName+"尚未簽核，請到\r\nPortal登入網址：http://192.168.1.26:8084/login\r\n做簽核，謝謝!\r\n\r\n\r\n" +                           
                            "此信件為系統自動寄送，請勿直接回信，若有疑問請洽MIS，謝謝您！";
                
                //創建一個SMTP客戶端
                SmtpClient smtpClient = new SmtpClient("msa.hinet.net");//mail.hiss.com.tw、msa.hinet.net     SMTP服務器地址
                smtpClient.Port = 587;//110、25                                                               SMTP端口號
                smtpClient.Credentials = new NetworkCredential("hiss.it@hiss.com.tw", "Aa123456");//webMail 地址密碼
                smtpClient.EnableSsl = true; //啟用SSL加密

                smtpClient.Send(mail);//發送
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "')</script>");
            }
        }

    }
}