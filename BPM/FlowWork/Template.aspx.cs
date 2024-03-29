﻿using ezEngineServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebGrease.Activities;
using static BPM.FlowWork.IT01;
using static BPM.FlowWork.Template;
using BPMLib;
using static BPMLib.Class1;
using static BPMLib.Class1.FormInfo;
using System.Net.Mail;
using System.Net;

namespace BPM.FlowWork
{
    public partial class Template : System.Web.UI.Page
    {
        public string strRequireDate; //需求日期

        protected void Page_Load(object sender, EventArgs e)
        {
            FormInfo Forminfo = new FormInfo();

            if (!IsPostBack)
            {
                //  設定表單資訊-------------------------------------
                strRequireDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");//需求日期
                lblApplyDate.Text = DateTime.Now.ToString("yyyy/MM/dd");//申請日期

                stuFormInfo stuFormInfo = new stuFormInfo();//結構
                stuFormInfo = Forminfo.GetFormInfoDataTable(stuFormInfo);

                stuFormInfo.strLoginEmployeeID = User.Identity.Name;

                //stuFormInfo.dtFileUpload = Forminfo.GetDtFileUpload();
                //stuFormInfo.dtwfFormAppInfo = Forminfo.GetDtwfFormAppInfo();
                //stuFormInfo.dtwfFormApp = Forminfo.GetDtwfFormApp();
                //stuFormInfo.dtwfFormSignM = Forminfo.getDtwfFormSignM();

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

                            //檢視畫面顯示設定
                            txbRequireDate.Enabled = false;
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

                            //申請日期、需求日期顯示設定
                            lblApplyDate.Text = stuFormInfo.dateApplyDate.ToString("yyyy-MM-dd");
                            strRequireDate = stuFormInfo.dateRequireDate.ToString("yyyy-MM-dd");
                            txbRequireDate.Text = strRequireDate;

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

                            // 申請原因
                            txbApplyReason.Enabled = false;
                            txbApplyReason.Text = stuFormInfo.strApplyReason;
                            if (string.IsNullOrEmpty(txbApplyReason.Text))
                            {
                                txbApplyReason.Text = "未填寫原因";
                            }

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

                            break;
                        case "ApParm":
                            stuFormInfo.intApKey = int.Parse(Request[strRequestName]);//apview or apparm後面的數字
                            stuFormInfo.intProcessID = Forminfo.CheckGetProcessID(stuFormInfo.intApKey, strRequestName); //取得ProcessID
                            stuFormInfo = Forminfo.GetFormView(stuFormInfo);//取得表單資訊

                            //簽核畫面顯示設定
                            txbRequireDate.Enabled = false;
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
                            strRequireDate = stuFormInfo.dateRequireDate.ToString("yyyy-MM-dd");
                            txbRequireDate.Text = strRequireDate;

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

                            // 申請原因
                            txbApplyReason.Enabled = false;
                            txbApplyReason.Text = stuFormInfo.strApplyReason;
                            if (string.IsNullOrEmpty(txbApplyReason.Text))
                            {
                                txbApplyReason.Text = "未填寫原因";
                            }

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
                strRequireDate = txbRequireDate.Text;
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
            FormInfo formInfo = new FormInfo();
            stuFormInfo stuFormInfo = (stuFormInfo)ViewState["stuFormInfo"];
            stuFormInfo.strRequireDate = txbRequireDate.Text;
            stuFormInfo.strApplyReason = txbApplyReason.Text;

            //  檢查有無輸入日期
            if (string.IsNullOrEmpty(stuFormInfo.strRequireDate))
            {
                Response.Write("<script>alert('" + "請選擇需求日期!" + "')</script>");
                return;
            }
          
            try
            {
                //發信
                SendMail();

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

        public void SendMail()
        {
            try 
            {
                //創建一個電子郵件
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("chiawei.chang@hiss.com.tw"); //發
                mail.To.Add("chiawei.chang@hiss.com.tw");//收
                mail.Subject = "(測試)【通知】(20230305)張家瑋 之共通表單";//標題
                //內文
                mail.Body = "(測試)Portal登入網址：http://192.168.1.26:8084/login\r\n" +
                            "工號:20230305\r\n" +
                            "姓名:張家瑋\r\n" +
                            "部門:系統設計課\r\n\r\n\r\n\r\n" +
                            "此信件為系統自動寄送，請勿直接回信，若有疑問請洽MIS，謝謝您！";

                //創建一個SMTP客戶端
                SmtpClient smtpClient = new SmtpClient("msa.hinet.net");//mail.hiss.com.tw、msa.hinet.net     SMTP服務器地址
                smtpClient.Port = 587;//110、25                                                               SMTP端口號
                smtpClient.Credentials = new NetworkCredential("chiawei.chang@hiss.com.tw", "T124382686");//webMail 地址密碼
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