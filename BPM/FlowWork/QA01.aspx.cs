using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static BPMLib.Class1;
using static BPMLib.Class1.FormInfo;

namespace BPM.FlowWork
{
    public partial class QA01 : System.Web.UI.Page
    {
        public string strOccureDate;//發生日期

        protected void Page_Load(object sender, EventArgs e)
        {
            FormInfo Forminfo = new FormInfo();

            if (!IsPostBack)
            {
                //  設定表單資訊-------------------------------------              
                lblApplyDate.Text = DateTime.Now.ToString("yyyy/MM/dd");//申請日期
                strOccureDate = DateTime.Now.ToString("yyyy/MM/dd");//發生日期

                rbtnlSelectWorking.SelectedIndex = 0;
                rbtnComplaint.SelectedIndex = 0;

                stuFormInfo stuFormInfo = new stuFormInfo();//結構
                stuFormInfo = Forminfo.GetFormInfoDataTable(stuFormInfo);

                stuFormInfo.strLoginEmployeeID = User.Identity.Name;



                if (!string.IsNullOrEmpty(stuFormInfo.strLoginEmployeeID))
                {
                    //ApView or ApParm or else
                    string strRequestName = Request.QueryString.AllKeys[0].ToString();
                    //ApView or ApParm畫面的顯示
                    switch (strRequestName)
                    {
                        case "ApView":
                            stuFormInfo.intApKey = int.Parse(Request[strRequestName]);//apview or apparm後面的數字
                            stuFormInfo.intProcessID = Forminfo.CheckGetProcessID(stuFormInfo.intApKey, strRequestName); //取得ProcessID
                            stuFormInfo = Forminfo.GetFormView(stuFormInfo);//取得表單資訊
                            stuFormInfo = Forminfo.GetFormQA01View(stuFormInfo);//取得QA01表單資訊

                            //檢視畫面顯示設定
                            ddlSelectStartEmpDept.Visible = false;
                            ddlSelectApplyEmp.Visible = false;
                            txbSignOpinion.Enabled = false;
                            pnlCheck.Visible = true;
                            btnSubmit.Visible = false;
                            btnReject.Visible = false;
                            btnTake.Visible = true;
                            btnSend.Visible = false;
                            pnlFileUpload.Visible = false;
                            grvFileUpload.Columns[3].Visible = false;

                            //申請日期顯示設定
                            lblApplyDate.Text = stuFormInfo.dateApplyDate.ToString("yyyy-MM-dd");


                            //選取的申請人
                            lblSelectApplyEmp.Text = stuFormInfo.strApplyEmployeeName;

                            //  申請人
                            lblApplyEmpName.Text = stuFormInfo.strApplyEmployeeName;
                            lblApplyEmpJobName.Text = stuFormInfo.strApplyEmployeeJobName;
                            lblApplyEmpDeptName.Text = stuFormInfo.strApplyEmployeeDeptName;

                            //  發起人
                            lblStartEmpName.Text = stuFormInfo.strStartEmployeeName;
                            lblStartEmpDeptName.Text = stuFormInfo.strStartEmployeeDeptName;
                            lblStartEmpJobName.Text = stuFormInfo.strStartEmployeeJobName;

                            if (stuFormInfo.dtSingerRoleList.Rows.Count > 0)
                            {
                                lblStatus.Text = "等待 " + stuFormInfo.strSignerEmployeeDeptName + stuFormInfo.strSignerEmployeeJobName + stuFormInfo.strSignerEmployeeName + " 簽核中";
                            }
                            else
                            {
                                lblStatus.Text = "此表單已結束審核";
                            }

                            //  表單序號
                            lblProcessID.Text = stuFormInfo.intProcessID.ToString();
                            lblLoginEmpID.Text = stuFormInfo.strLoginEmployeeID;
                            lblLoginEmpName.Text = stuFormInfo.strSignMEmployeeName;
                            lblFlowName.Text = stuFormInfo.strFormName;
                            //  綁定"附件上傳"
                            grvFileUpload.DataSource = stuFormInfo.dtFileUpload;
                            grvFileUpload.DataBind();

                            //  綁定簽核紀錄
                            grvFormSignM.DataSource = stuFormInfo.dtFormSingMList;
                            grvFormSignM.DataBind();

                            //------矯正處理單檢視資料-------
                            rbtnlSelectWorking.Enabled = false;
                            txbInputProductCode.Enabled = false;
                            btnEnter.Visible = false;
                            btnClearEnter.Visible = false;
                            txbBadQty.Enabled = false;
                            btnBadQty.Visible = false;
                            txbOccureDate.Enabled = false;
                            txbOccurPlace.Enabled = false;
                            txbProblemDescription.Enabled = false;
                            txbMeasureDirection.Enabled = false;
                            rbtnComplaint.Enabled = false;

                            //設定申請樣式的選取按鈕AND顯示設定
                            switch (stuFormInfo.strApplyType) 
                            {
                                case "客訴":
                                    rbtnlSelectWorking.SelectedIndex = 0;
                                    if (stuFormInfo.IsComplaint)
                                    {
                                        rbtnComplaint.SelectedIndex = 0;    
                                    }
                                    else
                                    {
                                        rbtnComplaint.SelectedIndex = 1;
                                    }
                                    break;
                                case "進料抽檢":
                                    rbtnlSelectWorking.SelectedIndex = 1;
                                    lblComplaint.Visible = false;
                                    rbtnComplaint.Visible = false;
                                    break;
                                case "生產製程巡檢":
                                    rbtnlSelectWorking.SelectedIndex = 2;
                                    lblComplaint.Visible = false;
                                    rbtnComplaint.Visible = false;
                                    break;
                                case "成品抽檢":
                                    rbtnlSelectWorking.SelectedIndex = 3;
                                    lblComplaint.Visible = false;
                                    rbtnComplaint.Visible = false;
                                    break;
                            }
                            //設定填寫資料
                            txbInputProductCode.Text = stuFormInfo.strProductCode+","+ stuFormInfo.strProductName;
                            lblEventObjectContent.Text = stuFormInfo.strEventObject;
                            lblShipQtyContent.Text = stuFormInfo.strShipQty;
                            txbBadQty.Text = stuFormInfo.strBadQty;
                            lblBadRateContent.Text = stuFormInfo.strBadRate;
                            txbOccureDate.Text = stuFormInfo.dateOccureDate.ToString();
                            txbOccurPlace.Text = stuFormInfo.strOccurePlace;
                            txbProblemDescription.Text = stuFormInfo.strProblemDescription;
                            txbMeasureDirection.Text = stuFormInfo.strMeasureDirection;

                            

                            break;
                        case "ApParm":
                            stuFormInfo.intApKey = int.Parse(Request[strRequestName]);//apview or apparm後面的數字
                            stuFormInfo.intProcessID = Forminfo.CheckGetProcessID(stuFormInfo.intApKey, strRequestName); //取得ProcessID
                            stuFormInfo = Forminfo.GetFormView(stuFormInfo);//取得表單資訊
                            stuFormInfo = Forminfo.GetFormQA01View(stuFormInfo);//取得QA01表單資訊
                            //簽核畫面顯示設定
                            ddlSelectStartEmpDept.Visible = false;
                            ddlSelectApplyEmp.Visible = false;
                            pnlCheck.Visible = true;
                            btnSubmit.Visible = true;
                            btnReject.Visible = true;
                            btnTake.Visible = false;
                            btnSend.Visible = false;
                            pnlFileUpload.Visible = false;
                            grvFileUpload.Columns[3].Visible = false;

                            //申請日期、需求日期顯示設定
                            lblApplyDate.Text = stuFormInfo.dateApplyDate.ToString("yyyy-MM-dd");

                            //選取的申請人
                            lblSelectApplyEmp.Text = stuFormInfo.strApplyEmployeeName;

                            //  申請人
                            lblApplyEmpName.Text = stuFormInfo.strApplyEmployeeName;
                            lblApplyEmpJobName.Text = stuFormInfo.strApplyEmployeeJobName;
                            lblApplyEmpDeptName.Text = stuFormInfo.strApplyEmployeeDeptName;

                            //  發起人
                            lblStartEmpName.Text = stuFormInfo.strStartEmployeeName;
                            lblStartEmpDeptName.Text = stuFormInfo.strStartEmployeeDeptName;
                            lblStartEmpJobName.Text = stuFormInfo.strStartEmployeeJobName;


                            //發起人為簽核人 但發起人不為申請人
                            if (stuFormInfo.strStartEmployeeID == stuFormInfo.strSignerEmployeeID && stuFormInfo.strStartEmployeeID != stuFormInfo.strApplyEmployeeID)
                            {
                                btnReject.Visible = false;
                                btnTake.Visible = true;
                                grvFileUpload.Columns[3].Visible = true;
                            }

                            //  表單序號
                            lblProcessID.Text = stuFormInfo.intProcessID.ToString();
                            lblLoginEmpID.Text = stuFormInfo.strLoginEmployeeID;
                            lblLoginEmpName.Text = stuFormInfo.strSignMEmployeeName;
                            lblFlowName.Text = stuFormInfo.strFormName;
                            //  綁定"附件上傳"
                            grvFileUpload.DataSource = stuFormInfo.dtFileUpload;
                            grvFileUpload.DataBind();

                            //  綁定簽核紀錄
                            grvFormSignM.DataSource = stuFormInfo.dtFormSingMList;
                            grvFormSignM.DataBind();

                            //----------矯正處理單------------
                            //------矯正處理單檢視資料-------
                            rbtnlSelectWorking.Enabled = false;
                            txbInputProductCode.Enabled = false;
                            btnEnter.Visible = false;
                            btnClearEnter.Visible = false;
                            txbBadQty.Enabled = false;
                            btnBadQty.Visible = false;
                            txbOccureDate.Enabled = false;
                            txbOccurPlace.Enabled = false;
                            txbProblemDescription.Enabled = false;
                            txbMeasureDirection.Enabled = false;
                            rbtnComplaint.Enabled = false;

                            //設定申請樣式的選取按鈕AND顯示設定
                            switch (stuFormInfo.strApplyType)
                            {
                                case "客訴":
                                    rbtnlSelectWorking.SelectedIndex = 0;
                                    if (stuFormInfo.IsComplaint)
                                    {
                                        rbtnComplaint.SelectedIndex = 0;
                                    }
                                    else
                                    {
                                        rbtnComplaint.SelectedIndex = 1;
                                    }
                                    break;
                                case "進料抽檢":
                                    rbtnlSelectWorking.SelectedIndex = 1;
                                    lblComplaint.Visible = false;
                                    rbtnComplaint.Visible = false;
                                    break;
                                case "生產製程巡檢":
                                    rbtnlSelectWorking.SelectedIndex = 2;
                                    lblComplaint.Visible = false;
                                    rbtnComplaint.Visible = false;
                                    break;
                                case "成品抽檢":
                                    rbtnlSelectWorking.SelectedIndex = 3;
                                    lblComplaint.Visible = false;
                                    rbtnComplaint.Visible = false;
                                    break;
                            }
                            //設定填寫資料
                            txbInputProductCode.Text = stuFormInfo.strProductCode + "," + stuFormInfo.strProductName;
                            lblEventObjectContent.Text = stuFormInfo.strEventObject;
                            lblShipQtyContent.Text = stuFormInfo.strShipQty;
                            txbBadQty.Text = stuFormInfo.strBadQty;
                            lblBadRateContent.Text = stuFormInfo.strBadRate;
                            txbOccureDate.Text = stuFormInfo.dateOccureDate.ToString();
                            txbOccurPlace.Text = stuFormInfo.strOccurePlace;
                            txbProblemDescription.Text = stuFormInfo.strProblemDescription;
                            txbMeasureDirection.Text = stuFormInfo.strMeasureDirection;


                            //品保主管節點
                            if (stuFormInfo.strSignOfTargetNodeID == "576")
                            {
                                pnlSelectInvestigator.Visible = true;
                                ddlSelectInvestigator.Enabled = true;
                                QA01SelectInvestigator();
                            }
                            else
                            {
                                ddlSelectInvestigator.Enabled = false;
                            }
                            //品保調查者節點
                            if (stuFormInfo.strSignOfTargetNodeID == "581")
                            {
                                pnlSelectManager.Visible = true;
                                ddlSelectManager.Enabled = true;
                                QA01SelectManager();
                            }
                            else
                            {
                                ddlSelectManager.Enabled = false;
                            }

                            break;
                        default:
                            //根據網址抓FormCode
                            //stuFormInfo.strFormCode = Request.QueryString.AllKeys[0];
                            stuFormInfo.strFormCode = strRequestName;

                            //使用class中方法
                            stuFormInfo = Forminfo.GetStartEmployeeInfo(stuFormInfo);

                            //將被選擇的申請人預設為登入者
                            stuFormInfo.strApplyEmployeeID = stuFormInfo.strLoginEmployeeID;

                            // 申請人資訊
                            stuFormInfo = Forminfo.GetApplyEmployeeInfo(stuFormInfo);

                            //申請人姓名、職稱、部門
                            lblApplyEmpName.Text = stuFormInfo.strApplyEmployeeName;
                            lblApplyEmpJobName.Text = stuFormInfo.strApplyEmployeeJobName;
                            lblApplyEmpDeptName.Text = stuFormInfo.strApplyEmployeeDeptName;

                            //登入者看不到審核畫面
                            pnlCheck.Visible = false;
                            pnlBtn.Visible = false;

                            // dtStartEmpRoleList為發起人的部門清單(role清單)
                            ddlSelectStartEmpDept.DataSource = stuFormInfo.dtStartEmployeeRoleList;
                            ddlSelectStartEmpDept.DataBind();

                            // 預設顯示第一筆結果的職稱、姓名到其他欄位，否則不會有其他欄位不會有值，需要等選擇部門才會顯示對應結果                
                            lblLoginEmpName.Text = stuFormInfo.strStartEmployeeName;
                            lblStartEmpName.Text = stuFormInfo.strStartEmployeeName;
                            lblStartEmpJobName.Text = stuFormInfo.strStartEmployeeJobName;

                            //選擇申請人
                            ddlSelectApplyEmp.DataSource = stuFormInfo.dtApplyEmployeeRoleList;
                            ddlSelectApplyEmp.DataBind();

                            // 將"選擇申請人"預設為登入者(發起人)
                            ddlSelectApplyEmp.SelectedValue = stuFormInfo.strLoginEmployeeID;

                            //顯示電子表單名稱
                            lblFlowName.Text = stuFormInfo.strFormName;

                            // 設定預設值
                            ddlSelectApplyEmp.SelectedValue = stuFormInfo.strLoginEmployeeID;
                            lblLoginEmpID.Text = stuFormInfo.strLoginEmployeeID;

                            break;
                    }
                    ViewState["stuFormInfo"] = stuFormInfo;
                }
            }
            else
            { 
                strOccureDate = txbOccureDate.Text; 
            }
        }

        protected void btnHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("Home.aspx");
        }

        // 發起人選擇"部門"
        protected void ddlSelectStartEmpDept_SelectedIndexChanged(object sender, EventArgs e)
        {
            FormInfo Forminfo = new FormInfo();
            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];

            dbFunction dbFunction = new dbFunction();
            using (SqlConnection conn = dbFunction.sqlHissShareElecFormConnection())
            {
                SqlCommand cmd = new SqlCommand("spFormApplySelectStartEmpDept", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strDepartmentID", ddlSelectStartEmpDept.SelectedValue);
                cmd.Parameters.AddWithValue("@strEmployeeID", stuFormInfo.strLoginEmployeeID);
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                da.SelectCommand = cmd;
                conn.Open();
                da.Fill(ds);

                stuFormInfo.dtApplyEmployeeRoleList = ds.Tables[0];

                //將登入者=發起者第一筆資料寫入
                DataRow drUser = stuFormInfo.dtApplyEmployeeRoleList.Select("EmployeeID='" + stuFormInfo.strLoginEmployeeID + "'")[0];

                //設定發起人資訊
                stuFormInfo.strStartEmployeeRoleID = (string)drUser["RoleID"];
                stuFormInfo.strStartEmployeeID = (string)drUser["EmployeeID"];

                //被選擇的申請人ID
                stuFormInfo.strApplyEmployeeID = stuFormInfo.strLoginEmployeeID;
                //得到登錄者資訊
                stuFormInfo = Forminfo.GetApplyEmployeeInfo(stuFormInfo);

                //設定發起人資訊
                lblStartEmpRoleID.Text = (string)drUser["RoleID"];
                lblStartEmpName.Text = (string)drUser["EmployeeName"];
                lblStartEmpJobName.Text = (string)drUser["JobName"];

                //申請人資訊
                lblApplyEmpName.Text = stuFormInfo.strApplyEmployeeName;
                lblApplyEmpJobName.Text = stuFormInfo.strApplyEmployeeJobName;
                lblApplyEmpDeptName.Text = stuFormInfo.strApplyEmployeeDeptName;

                //選擇申請人ddl
                ddlSelectApplyEmp.SelectedValue = stuFormInfo.strLoginEmployeeID;
                ddlSelectApplyEmp.DataSource = stuFormInfo.dtApplyEmployeeRoleList;
                ddlSelectApplyEmp.DataBind();

                //存回ViewState
                ViewState["stuFormInfo"] = stuFormInfo;
            }
        }

        // 發起人"選擇申請人"
        protected void ddlSelectApplyEmp_SelectedIndexChanged(object sender, EventArgs e)
        {
            FormInfo Forminfo = new FormInfo();
            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];

            stuFormInfo.strApplyEmployeeID = ddlSelectApplyEmp.SelectedValue;
            //得到選擇的人的資訊
            stuFormInfo = Forminfo.GetApplyEmployeeInfo(stuFormInfo);

            lblApplyEmpName.Text = stuFormInfo.strApplyEmployeeName;
            lblApplyEmpJobName.Text = stuFormInfo.strApplyEmployeeJobName;
            lblApplyEmpDeptName.Text = stuFormInfo.strApplyEmployeeDeptName;

            ViewState["stuFormInfo"] = stuFormInfo;
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

                stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];

                DataRow drUpload = stuFormInfo.dtFileUpload.NewRow();
                drUpload["FileName"] = strFileName;
                drUpload["FileType"] = strFileType;
                drUpload["FileSize"] = intFileSize;
                drUpload["FileUploadDate"] = DateTime.Now.ToString("yyyy/MM/dd   hh:mm:ss");
                drUpload["ServerName"] = strFileUploadName;
                stuFormInfo.dtFileUpload.Rows.Add(drUpload);

                ViewState["stuFormInfo"] = stuFormInfo;

                grvFileUpload.DataSource = stuFormInfo.dtFileUpload;
                grvFileUpload.DataBind();
            }
        }

        // "附件上傳"的下載按鈕
        protected void lbtnDownload_Command(object sender, CommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);

            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];

            if (stuFormInfo.dtFileUpload != null && stuFormInfo.dtFileUpload.Rows.Count > 0)
            {
                DataRow drFileUpload = stuFormInfo.dtFileUpload.Rows[rowIndex];

                string strFileName = drFileUpload["FileName"].ToString();
                string strFileUploadName = drFileUpload["ServerName"].ToString();
                string strFileUploadDate = DateTime.Now.ToString("yyyyMM") + "/";

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
            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            stuFormInfo.dtFileUpload.Rows.RemoveAt(rowIndex);
            grvFileUpload.DataSource = stuFormInfo.dtFileUpload;
            grvFileUpload.DataBind();
        }

        // 送出按鈕
        protected void btnSend_Click(object sender, EventArgs e)
        {
            //QA01表單資料設定
            FormQA01DataSet();

            FormInfo formInfo = new FormInfo();
            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];

            try
            {
                dbFunction dbFunction = new dbFunction();
                //呼叫oService
                ezEngineServices.Service oService = new ezEngineServices.Service(dbFunction.sqlHissFlowConnection());

                //  取得表單序號
                //stuFormInfo.intProcessID = oService.GetProcessID();
                stuFormInfo.intProcessID = formInfo.GetSendProcessID();

                //上傳資料到FileUpload
                formInfo.SqlbulkCopyFileUpload(stuFormInfo);

                //上傳資料到EZFlow
                formInfo.SqlBulkCopyToFlow(stuFormInfo);

                //上傳資料到QA01
                formInfo.SqlBulkCopyToFormQA01(stuFormInfo);

                //  判斷發起人與申請人是否相同，不同的話則為代發起
                if (stuFormInfo.strApplyEmployeeRoleID != stuFormInfo.strStartEmployeeRoleID)
                {
                    oService.FlowStart(stuFormInfo.intProcessID, stuFormInfo.strFormID, stuFormInfo.strApplyEmployeeRoleID, stuFormInfo.strApplyEmployeeID, stuFormInfo.strStartEmployeeRoleID, stuFormInfo.strStartEmployeeID);
                }
                else
                {
                    oService.FlowStart(stuFormInfo.intProcessID, stuFormInfo.strFormID, stuFormInfo.strApplyEmployeeRoleID, stuFormInfo.strApplyEmployeeID, stuFormInfo.strApplyEmployeeRoleID, stuFormInfo.strApplyEmployeeID);
                }



                Response.Redirect("Home.aspx");
            }
            catch (Exception)
            {
                Response.Write("<script>alert('" + "傳送失敗! " + "')</script>");
            }

        }

        // 簽核按鈕
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            FormInfo formInfo = new FormInfo();

            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];

            //設定FlowApprove資訊
            stuFormInfo.blnSignState = true;
            stuFormInfo.intState = 1;

            string strProcessID = stuFormInfo.intProcessID.ToString();

            if (strProcessID.Trim().Length > 0 && stuFormInfo.intApKey != 0)
            {
                if (stuFormInfo.blnSignState == false && stuFormInfo.strStartEmployeeID == stuFormInfo.strLoginEmployeeID && btnTake.Visible == true)
                {
                    //設定InsertFormSignM資訊
                    stuFormInfo.strSignMState = "重新送出申請";
                    stuFormInfo.blnSignM = true;
                    stuFormInfo.strSignOpinion = txbSignOpinion.Text;
                    formInfo.InsertFormSignM(stuFormInfo);
                }
                else
                {
                    //設定InsertFormSignM資訊
                    stuFormInfo.strSignMState = "簽核";
                    stuFormInfo.blnSignM = true;
                    stuFormInfo.strSignOpinion = txbSignOpinion.Text;
                    formInfo.InsertFormSignM(stuFormInfo);
                }

                //如果是品保主管簽核且ddlSelectInvestigator為enable=true時寫入調查者、確認者
                if (ddlSelectInvestigator.Enabled == true && stuFormInfo.strLoginEmployeeID == QAManagerEmpID())
                {
                    InvestigatorWriteToDynamic(strProcessID);
                }

                //如果是調查者且ddlSelectManager為enable=true時寫入課長
                if (ddlSelectManager.Enabled == true)
                {
                    ManagerWriteToDynamic(strProcessID);
                }

                //  流程推進
                formInfo.FlowApprove(stuFormInfo);

                if (stuFormInfo.intApKey != 0 && !string.IsNullOrEmpty(strProcessID))
                {
                    // 當簽核人為 發起人/負責人 的時候，編輯上傳內容並更新原本內容
                    if (stuFormInfo.strSignerEmployeeID == stuFormInfo.strStartEmployeeID)
                    {
                        formInfo.SqlbulkCopyFileUpload(stuFormInfo);
                    }

                    Response.Redirect("Home.aspx");
                }
                else
                {
                    Response.Write("<script>alert('" + "簽核失敗!" + "')</script>");
                }

            }
        }

        // 駁回按鈕
        protected void btnReject_Click(object sender, EventArgs e)
        {
            FormInfo formInfo = new FormInfo();
            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];

            //設定FlowApprove資訊
            stuFormInfo.blnSignState = false;
            stuFormInfo.intState = 1;

            //設定InsertFormSignM資訊
            stuFormInfo.strSignMState = "駁回";
            stuFormInfo.blnSignM = false;
            stuFormInfo.strSignOpinion = txbSignOpinion.Text;

            if (stuFormInfo.intProcessID != 0)
            {
                formInfo.FlowApprove(stuFormInfo);

                formInfo.InsertFormSignM(stuFormInfo);

                Response.Redirect("Home.aspx");
            }
        }

        // 取消申請按鈕
        protected void btnTake_Click(object sender, EventArgs e)
        {
            FormInfo formInfo = new FormInfo();

            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];

            //設定FlowApprove資訊
            stuFormInfo.blnSignState = false;
            stuFormInfo.intState = 7;

            //設定InsertFormSignM資訊
            stuFormInfo.strSignMState = "取消申請";
            stuFormInfo.blnSignM = true;
            stuFormInfo.strSignOpinion = txbSignOpinion.Text;

            if (stuFormInfo.intProcessID != 0)
            {
                if (stuFormInfo.strStartEmployeeID == stuFormInfo.strLoginEmployeeID)
                {
                    formInfo.FlowApprove(stuFormInfo);
                    formInfo.InsertFormSignM(stuFormInfo);

                    Response.Write("<script>alert('" + "申請已取消! " + "')</script>");
                    Response.Redirect("Home.aspx");
                }
            }
        }

        //--------------------矯正處理單區----------------------


        /// <summary>
        /// 輸入sap單號
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEnter_Click(object sender, EventArgs e)
        {
            string strInputProductCodeAndName = txbInputProductCode.Text;
            string strInputProductCode= strInputProductCodeAndName.Split(',')[0];   

            dbFunction dbFunction = new dbFunction();

            using (SqlConnection conn = dbFunction.sqlHissSAPHISS_Officail01Connection())
            {
                SqlCommand cmd = new SqlCommand("spGetViewSapB1ProductOrderList", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strInputProductCode", strInputProductCode);

                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                da.SelectCommand = cmd;
                conn.Open();
                da.Fill(ds);

                DataRow drSapB1ProductOrderList = ds.Tables[0].Rows[0];

                lblEventObjectContent.Text = (string)drSapB1ProductOrderList["CustomerNo"] + " " + (string)drSapB1ProductOrderList["CustomerName"];
                //lblProductNameContent.Text = (string)drSapB1ProductOrderList["ProductName"];
                lblShipQtyContent.Text = drSapB1ProductOrderList["Qty"].ToString();
            }
        }


        /// <summary>
        /// 計算不良率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnBadQty_Click(object sender, EventArgs e)
        {
            float intShipQty = lblShipQtyContent.Text.ToInteger();
            float intBadQty = txbBadQty.Text.ToInteger();

            float BadRate = ((intBadQty / intShipQty) * 100);
            float RoundedBadRate = (float)Math.Round(BadRate, 2);
            lblBadRateContent.Text = RoundedBadRate.ToString() + "%";
        }

        /// <summary>
        /// 清空欄位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClearEnter_Click(object sender, EventArgs e)
        {
            txbInputProductCode.Text = "";
            lblEventObjectContent.Text = "";
            //lblProductNameContent.Text = "";
            lblShipQtyContent.Text = "";
            txbBadQty.Text = "";
            lblBadRateContent.Text = "";
        }

        /// <summary>
        /// 矯正處理單-申請類型:客訴(complain)、進料抽檢(IQC)、生產製程巡檢(IPQC)、成品抽檢(OQC)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rbtnlSelectWorking_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (rbtnlSelectWorking.SelectedValue)
            {
                case "complain":
                    pnlComplaint.Visible = true;
                    break;
                case "IQC":
                    pnlComplaint.Visible = false;
                    break;
                case "IPQC":
                    pnlComplaint.Visible = false;
                    break;
                case "OQC":
                    pnlComplaint.Visible = false;
                    break;

            }
        }
        /// <summary>
        /// 選擇調查者
        /// </summary>
        public void QA01SelectInvestigator()
        {
            dbFunction dbFunction = new dbFunction();

            using (SqlConnection conn = dbFunction.sqlHissChiaweiConnection())
            {
                SqlCommand cmd = new SqlCommand("spQA01SelectInvestigator", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                da.SelectCommand = cmd;
                conn.Open();
                da.Fill(ds);

                ddlSelectInvestigator.DataSource = ds.Tables[0];
                ddlSelectInvestigator.DataBind();
            }
        }
        /// <summary>
        /// 調查者選擇課長
        /// </summary>
        public void QA01SelectManager()
        {
            dbFunction dbFunction = new dbFunction();

            using (SqlConnection conn = dbFunction.sqlHissChiaweiConnection())
            {
                SqlCommand cmd = new SqlCommand("spQA01SelectManager", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                da.SelectCommand = cmd;
                conn.Open();
                da.Fill(ds);

                ddlSelectManager.DataSource = ds.Tables[0];
                ddlSelectManager.DataBind();
            }
        }

        /// <summary>
        /// wfDynamic寫入調查者跟確認者
        /// </summary>
        public void InvestigatorWriteToDynamic(string strProcessID)
        {
            string strInvestigatorEmpID;
            string strInvestigatorEmpRoleID;

            string[] strInvestigator = ddlSelectInvestigator.SelectedValue.Split('/');
            strInvestigatorEmpRoleID = strInvestigator[0];
            strInvestigatorEmpID = strInvestigator[1];

            dbFunction dbFunction = new dbFunction();

            using (SqlConnection conn = dbFunction.sqlHissChiaweiConnection())
            {
                SqlCommand cmd = new SqlCommand("spQA01InvestigatorWriteToDynamic", conn);
                conn.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strProcessID", strProcessID);
                cmd.Parameters.AddWithValue("@strInvestigatorEmpID", strInvestigatorEmpID);
                cmd.Parameters.AddWithValue("@strInvestigatorEmpRoleID", strInvestigatorEmpRoleID);

                cmd.ExecuteNonQuery(); // 用於更新資料庫資料             
            }
        }

        public void ManagerWriteToDynamic(string strProcessID)
        {
            string strManagerEmpID;
            string strManagerEmpRoleID;

            string[] strManager = ddlSelectManager.SelectedValue.Split('/');
            strManagerEmpRoleID = strManager[0];
            strManagerEmpID = strManager[1];

            lblApplyDate.Text = strProcessID;//test
            dbFunction dbFunction = new dbFunction();

            using (SqlConnection conn = dbFunction.sqlHissChiaweiConnection())
            {
                SqlCommand cmd = new SqlCommand("spQA01ManagerWriteToDynamic", conn);
                conn.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strProcessID", strProcessID);
                cmd.Parameters.AddWithValue("@strManagerEmpID", strManagerEmpID);
                cmd.Parameters.AddWithValue("@strManagerEmpRoleID", strManagerEmpRoleID);

                cmd.ExecuteNonQuery(); // 用於更新資料庫資料             
            }
        }

        /// <summary>
        /// 取得品保主管的ID
        /// </summary>
        public string QAManagerEmpID()
        {
            string strQAManagerEmpID;

            dbFunction dbFunction = new dbFunction();

            using (SqlConnection conn = dbFunction.sqlHissChiaweiConnection())
            {
                SqlCommand cmd = new SqlCommand("spQA01GetQAManagerEmpID", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                da.SelectCommand = cmd;
                conn.Open();
                da.Fill(ds);

                strQAManagerEmpID = ds.Tables[0].Rows[0]["EmployeeID"].ToString();

            }
            return strQAManagerEmpID;
        }

        /// <summary>
        /// 設定FormQA01資料
        /// </summary>
        /// <param name="stuFormInfo"></param>
        public void FormQA01DataSet()
        {
            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];

            stuFormInfo.strApplyType=rbtnlSelectWorking.SelectedItem.Text;

            string strInputProductCodeAndName = txbInputProductCode.Text;
            stuFormInfo.strProductCode = strInputProductCodeAndName.Split(',')[0];
            stuFormInfo.strProductName = strInputProductCodeAndName.Split(',')[1];
            stuFormInfo.strEventObject = lblEventObjectContent.Text;
            stuFormInfo.strShipQty = lblShipQtyContent.Text;
            stuFormInfo.strBadQty = txbBadQty.Text;
            stuFormInfo.strBadRate = lblBadRateContent.Text;
            stuFormInfo.dateOccureDate= DateTime.Parse(txbOccureDate.Text);
            stuFormInfo.strOccurePlace = txbOccurPlace.Text;

            bool IsComplaint;
            if (rbtnlSelectWorking.SelectedValue== "complain" && rbtnComplaint.SelectedValue == "1")
            {
                IsComplaint=true;
            }
            else
            {
                IsComplaint = false;
            }
            stuFormInfo.IsComplaint = IsComplaint;

            stuFormInfo.strProblemDescription = txbProblemDescription.Text;
            stuFormInfo.strMeasureDirection = txbMeasureDirection.Text;

            ViewState["stuFormInfo"] = stuFormInfo;
        }
    }
}