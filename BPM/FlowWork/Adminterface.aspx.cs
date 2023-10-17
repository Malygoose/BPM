using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using static BPMLib.Class1;
using static BPMLib.Class1.FormInfo;

namespace BPM.FlowWork
{
    public partial class Adminterface : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {           
            if (!IsPostBack) 
            {
                //Session取得登入者名稱
                string strEmployeeName = (string)Session["EmployeeName"];
                lblLoginName.Text = strEmployeeName;

                //ItemType ItemList載入
                ItemTypeFormLoad();
            }
        }


        //按鈕顯示所有人現有項目表
        //protected void btnShowAdminterface_Click(object sender, EventArgs e)
        //{
        //    //管理者看現有項目表
        //    ViewEmployeeHaveItems();
        //}

        //管理者看所有人現有項目表
        //public void ViewEmployeeHaveItems()
        //{            
        //    dbFunction dbFunction = new dbFunction();
        //    //連線
        //    using (SqlConnection conn = dbFunction.sqlHissMingConnection())
        //    {
        //        conn.Open();
        //        string query = "spAdminterfaceViewHaveItems";
        //        using (SqlCommand cmd = new SqlCommand(query, conn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;                   
        //            cmd.Parameters.AddWithValue("@SearchEmpName", "");
        //            //cmd.ExecuteNonQuery();

        //            SqlDataAdapter da = new SqlDataAdapter();
        //            DataSet ds = new DataSet();
        //            da.SelectCommand = cmd;
        //            da.Fill(ds);

        //            DataTable dtAdminterface = ds.Tables[0];
        //            ViewState["dtAdminterface"] = dtAdminterface;

        //            grvAdminterface.DataSource = dtAdminterface;
        //            grvAdminterface.DataBind();                   
        //        }
        //    }
        //}
        //管理者看搜尋的人現有項目表
        public void ViewSearchEmployeeHaveItems()
        {
            string strSearchEmpName = txbSearch.Text;

            dbFunction dbFunction = new dbFunction();
            //連線
            using (SqlConnection conn = dbFunction.sqlHissMingConnection())
            {
                conn.Open();
                string query = "spAdminterfaceViewHaveItems";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SearchEmpName", strSearchEmpName);
                    //cmd.ExecuteNonQuery();

                    SqlDataAdapter da = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    da.SelectCommand = cmd;
                    da.Fill(ds);

                    DataTable dtAdminterface = ds.Tables[0];
                    ViewState["dtAdminterface"] = dtAdminterface;

                    grvAdminterface.DataSource = dtAdminterface;
                    grvAdminterface.DataBind();
                }
            }
        }
        //載入預先設定的ItemType
        public void ItemTypeFormLoad() 
        {
            dbFunction dbFunction = new dbFunction();
            //連線
            using (SqlConnection conn = dbFunction.sqlHissDBtestConnection())
            {
                SqlCommand cmd = new SqlCommand("spflowTestFormLoad", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                da.SelectCommand = cmd;
                conn.Open();
                da.Fill(ds);
                //設定ddlItemType
                ddlItemType.DataSource = ds.Tables[0];
                ddlItemType.DataBind();
                //設定ddlItemList
                ddlItemList.DataSource = ds.Tables[1];
                ddlItemList.DataBind();
            }         
        }

        //首頁
        protected void btnHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("Home.aspx");
        }
        //重新進入該頁面
        //protected void btnRefresh_Click(object sender, EventArgs e)
        //{
        //    Response.Redirect("Adminterface.aspx");
        //}
        //搜尋該員工工號的現有項目
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //取消選取
            grvAdminterface.SelectedIndex = -1;
            //按鈕顯示
            btnEdit.Visible = false;
            btnDelete.Visible = false;
            //顯示尋找的現有項目
            ViewSearchEmployeeHaveItems();
        }
        //選取後txb呈現該資料
        protected void grvAdminterface_SelectedIndexChanged(object sender, EventArgs e)
        {
            //按鈕顯示
            btnEdit.Visible = true;
            btnDelete.Visible = true;

            //利用datakeynames得到資料
            lblGuidKey.Text = grvAdminterface.SelectedDataKey["GuidKey"].ToString();
            //txbEditNobr.Text = grvAdminterface.SelectedDataKey["Nobr"].ToString();
            //txbEditUserName.Text = grvAdminterface.SelectedDataKey["UserName"].ToString();
            txbEmpNumName.Text = grvAdminterface.SelectedDataKey["Nobr"].ToString()+","+grvAdminterface.SelectedDataKey["UserName"].ToString() + "," + grvAdminterface.SelectedDataKey["DeptName"].ToString();
            //txbEditDeptName.Text = grvAdminterface.SelectedDataKey["DeptName"].ToString();
            //txbEditItemType.Text = grvAdminterface.SelectedDataKey["ItemType"].ToString();
            //txbEditItemName.Text = grvAdminterface.SelectedDataKey["ItemName"].ToString();
            txbEditAssetsName.Text = grvAdminterface.SelectedDataKey["AssetsName"].ToString();
            txbEditAssetsCode.Text = grvAdminterface.SelectedDataKey["AssetsCode"].ToString();

            //DataTable dtAdminterface = (DataTable)ViewState["dtAdminterface"];

            //int intSelectedIndex = grvAdminterface.SelectedIndex; //選擇的索引
            //int intPageIndex = grvAdminterface.PageIndex;   //擷取當前頁索引
            //int intPageSize = grvAdminterface.PageSize;      //擷取每頁顯示記錄數
            //int intCurrentSelectedIndex = intSelectedIndex + intPageIndex  * intPageSize; //取得當前頁面的索引

            //DataRow drAdminterface = dtAdminterface.Rows[intCurrentSelectedIndex];

            //txbEditNobr.Text = drAdminterface["Nobr"].ToString();
            //txbEditUserName.Text = drAdminterface["UserName"].ToString();
            //txbEditDeptName.Text = drAdminterface["DeptName"].ToString();
            //txbEditItemType.Text = drAdminterface["ItemType"].ToString();
            //txbEditAssetsCode.Text = drAdminterface["AssetsCode"].ToString();
            //txbEditAssetsName.Text = drAdminterface["AssetsName"].ToString();
            //txbEditItemName.Text = drAdminterface["ItemName"].ToString();

            //lblGuidKey.Text = drAdminterface["GuidKey"].ToString();

        }
        //更改資料
        protected void btnEdit_Click(object sender, EventArgs e)
        {           
            //用逗號做分割
            string[] strEmpNumNameArray = txbEmpNumName.Text.Split(',');
            //取出資料
            string strNobr = strEmpNumNameArray[0];
            string strUserName = strEmpNumNameArray[1];
            string strDeptName = strEmpNumNameArray[2];
            //string strNobr = txbEditNobr.Text;
            //string strUserName = txbEditUserName.Text;
            //string strDeptName = txbEditDeptName.Text;
            //string strItemType = txbEditItemType.Text;
            //string strItemName = txbEditItemName.Text;
            string strItemType = ddlItemType.SelectedItem.Text;
            string strItemName = ddlItemList.SelectedItem.Text; 
            string strAssetsCode = txbEditAssetsCode.Text;
            string strAssetsName = txbEditAssetsName.Text;          
            string strGuidKey = lblGuidKey.Text;

            string strInputEmail = txbEditAssetsName.Text.Trim();
            //判斷信箱是否有效
            if (strItemType == "信箱" && !IsValidEmail(strInputEmail))
            {
                lblErrorMessage.Text = "無效的電子信箱";
                lblErrorMessage.Visible = true;
            }
            else
            {
                lblErrorMessage.Visible = false;

                //用select找guidkey
                DataTable dtAdminterface = (DataTable)ViewState["dtAdminterface"];
                DataRow[] selectedRow = dtAdminterface.Select("GuidKey= '" + strGuidKey + "'");
                //將變更的資料寫入
                selectedRow[0]["Nobr"] = strNobr;
                selectedRow[0]["UserName"] = strUserName;
                selectedRow[0]["DeptName"] = strDeptName;
                selectedRow[0]["ItemType"] = strItemType;
                selectedRow[0]["AssetsCode"] = strAssetsCode;
                selectedRow[0]["AssetsName"] = strAssetsName;
                selectedRow[0]["ItemName"] = strItemName;
                selectedRow[0]["GuidKey"] = strGuidKey;
                //int intSelectedIndex = grvAdminterface.SelectedIndex;   //選擇索引   5   第六筆
                //int intPageIndex = grvAdminterface.PageIndex;           //擷取當前頁索引   1  第二頁
                //int intPageSize = grvAdminterface.PageSize;             //擷取每頁顯示記錄數  20
                //int intCurrentSelectedIndex = intSelectedIndex + intPageIndex * intPageSize; //取得當前頁面選擇的索引  5+20*1=25 第二頁第六筆

                //dtAdminterface.Rows[intCurrentSelectedIndex]["Nobr"] = strNobr;
                //dtAdminterface.Rows[intCurrentSelectedIndex]["UserName"] = strUserName;
                //dtAdminterface.Rows[intCurrentSelectedIndex]["DeptName"] = strDeptName;
                //dtAdminterface.Rows[intCurrentSelectedIndex]["ItemType"] = strItemType;
                //dtAdminterface.Rows[intCurrentSelectedIndex]["AssetsCode"] = strAssetsCode;
                //dtAdminterface.Rows[intCurrentSelectedIndex]["AssetsName"] = strAssetsName;
                //dtAdminterface.Rows[intCurrentSelectedIndex]["ItemName"] = strItemName;
                //dtAdminterface.Rows[intCurrentSelectedIndex]["GuidKey"] = strGuidKey;

                dbFunction dbFunction = new dbFunction();
                //連線
                using (SqlConnection conn = dbFunction.sqlHissMingConnection())
                {
                    conn.Open();
                    string query = "spUpdateIT01EmployeeItems";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Nobr", strNobr);
                        cmd.Parameters.AddWithValue("@UserName", strUserName);
                        cmd.Parameters.AddWithValue("@DeptName", strDeptName);
                        cmd.Parameters.AddWithValue("@ItemType", strItemType);
                        cmd.Parameters.AddWithValue("@AssetsCode", strAssetsCode);
                        cmd.Parameters.AddWithValue("@AssetsName", strAssetsName);
                        cmd.Parameters.AddWithValue("@ItemName", strItemName);
                        cmd.Parameters.AddWithValue("@GuidKey", strGuidKey);

                        cmd.ExecuteNonQuery();
                    }
                }
                ViewState["dtAdminterface"] = dtAdminterface;
                grvAdminterface.DataSource = dtAdminterface;
                grvAdminterface.DataBind();
                //取消選取
                grvAdminterface.SelectedIndex = -1;
                //按鈕顯示
                btnEdit.Visible = false;
                btnDelete.Visible = false;
                //清空
                ClearTxbContent();
            }           
        }

        //刪除
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            //DataTable dtAdminterface = (DataTable)ViewState["dtAdminterface"];

            //int intSelectedIndex = grvAdminterface.SelectedIndex;
            //int intPageIndex = grvAdminterface.PageIndex;   //擷取當前頁索引
            //int intPageSize = grvAdminterface.PageSize;      //擷取每頁顯示記錄數
            //int intCurrentSelectedIndex = intSelectedIndex + intPageIndex * intPageSize; //取得當前頁面選擇的索引

            //DataRow drAdminterface = dtAdminterface.Rows[intCurrentSelectedIndex];

            //string strGuidKey = drAdminterface["GuidKey"].ToString();

            //用DataKeyNames得到Guidkey
            string strGuidKey = grvAdminterface.SelectedDataKey[0].ToString();

            dbFunction dbFunction = new dbFunction();
            //連線
            using (SqlConnection conn = dbFunction.sqlHissMingConnection())
            {
                conn.Open();
                string query = "spDeleteIT01EmployeeItems";
                //做deleteMark = 1
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@GuidKey", strGuidKey);

                    cmd.ExecuteNonQuery();
                }

                //if (!string.IsNullOrEmpty(txbSearch.Text))
                //{
                //    ViewSearchEmployeeHaveItems();
                //}
                //else
                //{
                //    ViewEmployeeHaveItems();
                //}

            }
            //取消選取
            grvAdminterface.SelectedIndex = -1;
            //按鈕顯示
            btnEdit.Visible = false;
            btnDelete.Visible = false;
            //搜尋
            ViewSearchEmployeeHaveItems();
            //清空
            ClearTxbContent() ;
        }

        //protected void grvAdminterface_RowDeleting(object sender, GridViewDeleteEventArgs e)
        //{
        //    DataTable dtAdminterface = (DataTable)ViewState["dtAdminterface"];

        //    int intSelectedIndex = grvAdminterface.SelectedIndex;
        //    int intPageIndex = grvAdminterface.PageIndex;   //擷取當前頁索引
        //    int intPageSize = grvAdminterface.PageSize;      //擷取每頁顯示記錄數
        //    int intCurrentSelectedIndex = intSelectedIndex + intPageIndex * intPageSize; //取得當前頁面選擇的索引

        //    DataRow drAdminterface = dtAdminterface.Rows[intCurrentSelectedIndex];
            
        //    string strGuidKey = drAdminterface["GuidKey"].ToString();

        //    dbFunction dbFunction = new dbFunction();
        //    //連線
        //    using (SqlConnection conn = dbFunction.sqlHissMingConnection())
        //    {
        //        conn.Open();
        //        string query = "spDeleteIT01EmployeeItems";
        //        //做deleteMark = 1
        //        using (SqlCommand cmd = new SqlCommand(query, conn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@GuidKey", strGuidKey);

        //            cmd.ExecuteNonQuery();
        //        }

        //        if (!string.IsNullOrEmpty(txbSearch.Text))
        //        {
        //            ViewSearchEmployeeHaveItems();              
        //        }
        //        else 
        //        {
        //            ViewEmployeeHaveItems();
        //        }
                
        //    }
        //    grvAdminterface.SelectedIndex = -1;
        //}

        protected void grvAdminterface_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //頁面
            grvAdminterface.PageIndex = e.NewPageIndex;

            //選取移除
            grvAdminterface.SelectedIndex = -1;
            //綁定
            DataTable dtAdminterface = (DataTable)ViewState["dtAdminterface"];
            grvAdminterface.DataSource = dtAdminterface;
            grvAdminterface.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string strEmpNumName = txbEmpNumName.Text;
            string[] strEmpNumNameArray = strEmpNumName.Split(',');

            //得到資料
            string strAddNobr = strEmpNumNameArray[0];
            string strAddUserName = strEmpNumNameArray[1];
            string strAddDeptName = strEmpNumNameArray[2];
            //string strAddNobr = txbEditNobr.Text;
            //string strAddUserName = txbEditUserName.Text;
            //string strAddDeptName = txbEditDeptName.Text;
            //string strAddItemType = txbEditItemType.Text;
            //string strAddItemName = txbEditItemName.Text;
            string strAddItemType = ddlItemType.SelectedItem.Text;
            string strAddItemName = ddlItemList.SelectedItem.Text;  
            string strAddAssetsCode = txbEditAssetsCode.Text;
            string strAddAssetsName = txbEditAssetsName.Text;


            string strInputEmail = txbEditAssetsName.Text.Trim();
            //判斷信箱是否有效
            if (strAddItemType == "信箱" && !IsValidEmail(strInputEmail))
            {
                lblErrorMessage.Text = "無效的電子信箱";
                lblErrorMessage.Visible =true;
            }          
            else 
            {
                lblErrorMessage.Visible = false;

                //設定資料欄位
                DataTable dtAddIT01EmployeeItems = new DataTable();
                dtAddIT01EmployeeItems.Columns.Add("Nobr");
                dtAddIT01EmployeeItems.Columns.Add("UserName");
                dtAddIT01EmployeeItems.Columns.Add("DeptName");
                dtAddIT01EmployeeItems.Columns.Add("ItemType");
                dtAddIT01EmployeeItems.Columns.Add("AssetsCode");
                dtAddIT01EmployeeItems.Columns.Add("AssetsName");
                dtAddIT01EmployeeItems.Columns.Add("ItemName");
                //設定欄位相對資料
                DataRow drAddIT01EmployeeItems = dtAddIT01EmployeeItems.NewRow();
                drAddIT01EmployeeItems["Nobr"] = strAddNobr;
                drAddIT01EmployeeItems["UserName"] = strAddUserName;
                drAddIT01EmployeeItems["DeptName"] = strAddDeptName;
                drAddIT01EmployeeItems["ItemType"] = strAddItemType;
                drAddIT01EmployeeItems["AssetsCode"] = strAddAssetsCode;
                drAddIT01EmployeeItems["AssetsName"] = strAddAssetsName;
                drAddIT01EmployeeItems["ItemName"] = strAddItemName;
                dtAddIT01EmployeeItems.Rows.Add(drAddIT01EmployeeItems);

                dbFunction dbFunction = new dbFunction();
                //連線
                using (SqlConnection conn = dbFunction.sqlHissMingConnection())
                {
                    conn.Open();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                    {
                        // 資料上傳
                        bulkCopy.DestinationTableName = "IT01EmployeeItems";
                        bulkCopy.ColumnMappings.Add("Nobr", "Nobr");
                        bulkCopy.ColumnMappings.Add("UserName", "UserName");
                        bulkCopy.ColumnMappings.Add("DeptName", "DeptName");
                        bulkCopy.ColumnMappings.Add("ItemType", "ItemType");
                        bulkCopy.ColumnMappings.Add("AssetsCode", "AssetsCode");
                        bulkCopy.ColumnMappings.Add("AssetsName", "AssetsName");
                        bulkCopy.ColumnMappings.Add("ItemName", "ItemName");
                        bulkCopy.WriteToServer(dtAddIT01EmployeeItems);
                    }
                }
                //取消選取
                grvAdminterface.SelectedIndex = -1;
                //搜尋
                ViewSearchEmployeeHaveItems();
                //清空
                ClearTxbContent();
            }            
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearTxbContent();
        }

        protected void ClearTxbContent()
        {
            //txbEditNobr.Text = "";
            //txbEditUserName.Text = "";
            txbEmpNumName.Text = "";
            //txbEditDeptName.Text = "";
            //txbEditItemType.Text = "";
            //txbEditItemName.Text = "";
            txbEditAssetsCode.Text = "";
            txbEditAssetsName.Text = "";                         
        }
        //依照不同的ItemType得到相對的Itemlist
        protected void ddlItemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //清空
            txbEditAssetsCode.Text = "";
            txbEditAssetsName.Text = "";
            lblErrorMessage.Visible = false;
            //顯示
            switch (ddlItemType.SelectedItem.Text)
            {
                case "硬體":
                    lblAssetsCode.Visible = true;
                    txbEditAssetsCode.Visible = true;
                    lblAssetsName.Visible = true;
                    txbEditAssetsName.Visible = true;
                    break;
                case "信箱":
                    lblAssetsCode.Visible = false;
                    txbEditAssetsCode.Visible = false;
                    lblAssetsName.Visible = true;
                    txbEditAssetsName.Visible = true;
                    break;
                default:
                    lblAssetsCode.Visible = false;
                    txbEditAssetsCode.Visible = false;
                    lblAssetsName.Visible = false;
                    txbEditAssetsName.Visible = false;
                    break;
            }

            dbFunction dbFunction = new dbFunction();

            using (SqlConnection conn = dbFunction.sqlHissDBtestConnection())
            {
                SqlCommand cmd = new SqlCommand("spGetIT01ItemListByTypeID", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strTypeID", ddlItemType.SelectedValue);

                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                da.SelectCommand = cmd;
                conn.Open();
                da.Fill(ds);

                ddlItemList.DataSource = ds.Tables[0];
                ddlItemList.DataBind();
            }
        }

        //判斷是否為有效的信箱
        public bool IsValidEmail(string email) 
        {
            string pattern = @"^\w+([-+.']\w+)*@hiss\.com\.tw$";
            return System.Text.RegularExpressions.Regex.IsMatch(email, pattern);    
        }

        //判斷是否為有效的資產編號
        public bool IsValidAssetsCode(string AssetsCode)
        {
            string pattern = @"^\d{7}$";
            return System.Text.RegularExpressions.Regex.IsMatch(AssetsCode, pattern);
        }

        //資訊設備明細表下載成excel
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Content");

            DataTable dtAdminterface =(DataTable)ViewState["dtAdminterface"];

            ws.Cell(1,1).InsertTable(dtAdminterface);

            //string fileName = "資訊設備明細表.xlsx"; // 文件名
            //string downloadsFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\"+ fileName;
            //wb.SaveAs(downloadsFolder);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                wb.SaveAs(memoryStream);

                //下載
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=資訊設備明細表.xlsx");
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.BinaryWrite(memoryStream.ToArray());
                Response.End();
            }

        }
    }
}

