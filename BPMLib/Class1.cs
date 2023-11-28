using ezEngineServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMLib
{
    public class Class1
    {

        public class dbFunction
        {

            public SqlConnection sqlHissFlowConnection()
            {
                //SqlConnection sqlCon = new SqlConnection("server=localhost;database=HERP;User ID=herp;Password=Aa123456;");
                SqlConnection sqlCon = new SqlConnection("server=192.168.1.9;database=ezFlow;User ID=herp;Password=Aa123456;");
                return sqlCon;
            }

            public SqlConnection sqlHissShareElecFormConnection()
            {
                //SqlConnection sqlCon = new SqlConnection("server=localhost;database=HERP;User ID=herp;Password=Aa123456;");
                SqlConnection sqlCon = new SqlConnection("server=192.168.1.26;database=ShareElecForm;User ID=chiawei;Password=Aa123456;");
                return sqlCon;
            }

            public SqlConnection sqlHissMingConnection()
            {
                //SqlConnection sqlCon = new SqlConnection("server=localhost;database=HERP;User ID=herp;Password=Aa123456;");
                SqlConnection sqlCon = new SqlConnection("server=192.168.1.26;database=ming;User ID=chiawei;Password=Aa123456;");
                return sqlCon;
            }

            public SqlConnection sqlHissDBtestConnection()
            {
                SqlConnection sqlCon = new SqlConnection("server=192.168.1.26;database=DatabaseTest;User ID=chiawei;Password=Aa123456;");
                return sqlCon;
            }

            public SqlConnection sqlHissSAPHISS_Officail01Connection()
            {
                SqlConnection sqlCon = new SqlConnection("server=192.168.1.23;database=HISS_Official01;User ID=chiawei;Password=Aa123456;");
                return sqlCon;
            }

            public SqlConnection sqlHissChiaweiConnection()
            {
                SqlConnection sqlCon = new SqlConnection("server=192.168.1.26;database=chiawei;User ID=chiawei;Password=Aa123456;");
                return sqlCon;
            }
        }

        public class FormInfo
        {
            [Serializable]
            public struct stuFormInfo
            {
                //  表單共用變數-------------------------------------
                public string strFormID;                    //表單ID
                public string strFormCode;                  //表單代號 
                public string strFormName;                  //表單名稱 

                public int intProcessID;                    //流程ID
                public int intApKey;                        //流程ApKey   ApParm、ApView後面的數字
                public bool blnSignState;                   //流程目前bSign值 0駁回 1核准   其中取消申請的判斷在這為false，因為連簽都沒簽就取消了
                public int intState;                        //流程狀態 1進行中 3完成 7取消申請

                public DateTime dateApplyDate;              //申請日期
                public DateTime dateRequireDate;            //需求日期
                public string strRequireDate;               //需求日期
               
                //--------申請---------
                public string strApplyEmployeeID;            //申請人工號
                public string strApplyEmployeeName;          //申請人姓名
                public string strApplyEmployeeRoleID;        //申請人RoleID
                public string strApplyEmployeeDeptID;        //申請人部門ID
                public string strApplyEmployeeDeptName;      //申請人部門名稱              
                public int intApplyEmployeeDeptLevel;        //申請人部門Level
                public string strApplyEmployeeJobID;         //申請人職位ID
                public string strApplyEmployeeJobName;       //申請人職位名稱

                public string strApplyReason;                //申請原因

                //--------發起-------------
                public string strStartEmployeeID;              //發起人工號
                public string strStartEmployeeName;            //發起人姓名
                public string strStartEmployeeRoleID;          //發起人RoleID
                public string strStartEmployeeDeptID;          //發起人部門ID
                public string strStartEmployeeDeptName;        //發起人部門名稱
                public int intStartEmployeeDeptLevel;          //發起人部門Level
                public string strStartEmployeeJobID;           //發起人職位ID
                public string strStartEmployeeJobName;         //發起人職稱

                //--------流程中下一位簽核人資訊-------------
                public string strSignerEmployeeID;             //流程中下一位簽核人的工號
                public string strSignerEmployeeName;           //流程中下一位簽核人姓名
                public string strSignerEmployeeRoleID;         //流程中下一位簽核人的RoleID
                public string strSignerEmployeeDeptID;         //流程中下一位簽核人部門ID
                public string strSignerEmployeeDeptName;       //流程中下一位簽核人部門名稱
                public int intSignerEmployeeDeptLevel;         //流程中下一位簽核人部門Level
                public string strSignerEmployeeJobID;          //流程中下一位簽核人職位ID
                public string strSignerEmployeeJobName;        //流程中下一位簽核人職稱
                public string strSignOfTargetNodeID;             //流程中下一位簽核人節點ID
                public string strSignOfTargetNodeName;         //流程中下一位簽核人節點名稱


                //--------登入者---------- 
                public string strLoginEmployeeID;                //登入者工號

                //--------寫入到wfFormSignM--------------------
                public string strSignMEmployeeName;              //寫入到wfFormSignM簽核者姓名
                public string strSignMEmployeeRoleID;            //寫入到wfFormSignM簽核者角色工號
                public string strSignMEmployeeDeptID;            //寫入到wfFormSignM簽核者部門工號
                public string strSignMEmployeeDeptName;          //寫入到wfFormSignM簽核者部門名稱   
                public string strSignMEmployeeJobID;              //寫入到wfFormSignM簽核者職位ID
                public string strSignMEmployeeJobName;           //寫入到wfFormSignM簽核者職位名稱

                public string strSignMState;                    //寫入到wfFormSignM的簽核文字(重新送出申請、簽核、駁回)
                public bool blnSignM;                           //寫入到wfFormSignM的bool  0駁回 1核准
                public string strSignOpinion;                   //寫入到wfFormSignM的簽核意見

                //------DataTable區----------              
                public DataTable dtApplyEmployeeRoleList;       //被申請人角色資訊
                public DataTable dtStartEmployeeRoleList;       //發起者角色資訊
                public DataTable dtFormSingMList;               //簽核歷程資訊
                public DataTable dtSingerRoleList;              //流程中下一位簽核人角色資訊

                public DataTable dtFileUpload;          //寫入ming資料表FileUpload 附件檔案
                public DataTable dtwfFormAppInfo;       //寫入EZFlow wfFormAppInfo
                public DataTable dtwfFormApp;           //寫入EZFlow wfFormApp
                public DataTable dtwfFormSignM;         //寫入EZFlow wfFormSignM

               
            }
            /// <summary>
            /// 設定發起人的資訊
            /// </summary>
            /// <param name="stuFormInfo">Struct結構</param>
            /// <returns>Struct結構</returns>           
            public stuFormInfo GetStartEmployeeInfo(stuFormInfo stuFormInfo)
            {
                dbFunction dbFunction = new dbFunction();
                using (SqlConnection conn = dbFunction.sqlHissShareElecFormConnection())
                {
                    SqlCommand cmd = new SqlCommand("spFormApply", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@strEmployeeID", stuFormInfo.strLoginEmployeeID);
                    cmd.Parameters.AddWithValue("@sFormCode", stuFormInfo.strFormCode);
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    da.SelectCommand = cmd;
                    conn.Open();
                    da.Fill(ds);

                    // 取得登入者資訊
                    DataTable dtStartEmpRoleList = ds.Tables[0];
                    stuFormInfo.dtStartEmployeeRoleList = dtStartEmpRoleList;

                    //登入者做為發起人
                    stuFormInfo.strStartEmployeeRoleID = (string)dtStartEmpRoleList.Rows[0]["RoleID"];
                    stuFormInfo.strStartEmployeeID = (string)dtStartEmpRoleList.Rows[0]["EmployeeID"];
                    //stuFormInfo.strLoginEmployeeName = (string)dtStartEmpRoleList.Rows[0]["EmployeeName"];
                    stuFormInfo.strStartEmployeeName = (string)dtStartEmpRoleList.Rows[0]["EmployeeName"];
                    stuFormInfo.strStartEmployeeJobName = (string)dtStartEmpRoleList.Rows[0]["JobName"];

                    //設定電子表單訊
                    stuFormInfo.strFormName = ds.Tables[2].Rows[0]["sFormName"].ToString();
                    stuFormInfo.strFormID = ds.Tables[2].Rows[0]["sFlowTree"].ToString();

                    // 在SP判斷是否為主管，若為主管，獲得該角色的部門有哪些人可以選擇為申請人            
                    stuFormInfo.dtApplyEmployeeRoleList = ds.Tables[1];
                }
                return stuFormInfo;
            }


            /// <summary>
            /// 表單，取得發起人與申請人資訊，並儲存到Struct的viewState
            /// </summary>
            /// <param name="stuFormInfo">Struct結構</param>
            /// <returns>Struct結構</returns>
            public stuFormInfo GetApplyEmployeeInfo(stuFormInfo stuFormInfo)
            {
                //進入時預設登錄者作為發起人的第一筆相關資料
                DataRow drEmployee = stuFormInfo.dtApplyEmployeeRoleList.Select("EmployeeID='" + stuFormInfo.strApplyEmployeeID + "'")[0];

                if (string.IsNullOrEmpty(stuFormInfo.strSignerEmployeeRoleID))
                    stuFormInfo.strSignerEmployeeRoleID = (string)drEmployee["RoleID"];

                stuFormInfo.strApplyEmployeeName = (string)drEmployee["EmployeeName"];
                stuFormInfo.strApplyEmployeeDeptName = (string)drEmployee["DepartmentName"];
                stuFormInfo.strApplyEmployeeDeptID = (string)drEmployee["DepartmentID"];
                stuFormInfo.strApplyEmployeeJobID = (string)drEmployee["JobID"];
                stuFormInfo.strApplyEmployeeJobName = (string)drEmployee["JobName"];
                stuFormInfo.strApplyEmployeeRoleID = (string)drEmployee["RoleID"];
                stuFormInfo.strApplyEmployeeID = (string)drEmployee["EmployeeID"];
                stuFormInfo.intApplyEmployeeDeptLevel = int.Parse((string)drEmployee["DepartmentLevel"]);

                return stuFormInfo;
            }

            /// <summary>
            /// 用ApView或ApParm取得ProcessID
            /// </summary>
            /// <param name="intApKey">ApView或ApParm</param>
            /// <param name="strRequestName">表單名稱</param>
            /// <returns>Struct結構</returns>
            public int CheckGetProcessID(int intApKey,string strRequestName)
            {
                dbFunction dbFunction = new dbFunction();
                using (SqlConnection conn = dbFunction.sqlHissFlowConnection())
                {
                    conn.Open();
                    string query = "spCheckGetProcessID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@intApKey", intApKey);
                        cmd.Parameters.AddWithValue("@strRequestName", strRequestName);

                        SqlDataAdapter da = new SqlDataAdapter();
                        DataSet ds = new DataSet();
                        da.SelectCommand = cmd;
                        da.Fill(ds);

                        return (int)ds.Tables[0].Rows[0]["ProcessFlow_id"];
                    }
                }
                
            }


            /// <summary>
            /// 判斷並讀取資訊服務單的簽核或檢視內容的view
            /// </summary>
            /// <param name="stuFormInfo">Struct結構</param>
            /// <returns>Struct結構</returns>
            public stuFormInfo GetFormView(stuFormInfo stuFormInfo)
            {
                dbFunction dbFunction = new dbFunction();
                using (SqlConnection conn = dbFunction.sqlHissShareElecFormConnection())
                {
                    conn.Open();
                    string query = "spFormView";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@strProcessID", stuFormInfo.intProcessID);
                        cmd.Parameters.AddWithValue("@strEmpID", stuFormInfo.strLoginEmployeeID);

                        SqlDataAdapter da = new SqlDataAdapter();
                        DataSet ds = new DataSet();
                        da.SelectCommand = cmd;
                        da.Fill(ds);

                        //  流程目前狀態為"簽核"或"駁回"
                        stuFormInfo.blnSignState = (bool)ds.Tables[0].Rows[0]["FlowSign"];
                        stuFormInfo.strFormName = ds.Tables[0].Rows[0]["FormName"].ToString();
                        stuFormInfo.strFormID = ds.Tables[0].Rows[0]["FormID"].ToString();
                        stuFormInfo.strFormCode = ds.Tables[0].Rows[0]["FormCode"].ToString();

                        //取得此表單"申請日期"與"需求日期"並設定
                        stuFormInfo.dateApplyDate = DateTime.Parse(ds.Tables[0].Rows[0]["ApplyDate"].ToString());
                        stuFormInfo.dateRequireDate = DateTime.Parse(ds.Tables[0].Rows[0]["RequireDate"].ToString());

                        //取得表單現在要簽核的的flowID
                        stuFormInfo.strSignOfTargetNodeID = ds.Tables[0].Rows[0]["FlowNode_id"].ToString();

                        //  將需要用到的發起人、申請人資訊儲存到Struct
                        stuFormInfo.strApplyEmployeeID = ds.Tables[0].Rows[0]["ApplyEmpID"].ToString();
                        stuFormInfo.strApplyEmployeeRoleID = ds.Tables[0].Rows[0]["ApplyEmpRoleID"].ToString();
                        stuFormInfo.strApplyEmployeeName = ds.Tables[0].Rows[0]["ApplyEmpName"].ToString();
                        stuFormInfo.strApplyEmployeeJobName = ds.Tables[0].Rows[0]["ApplyEmpJobName"].ToString();
                        stuFormInfo.strApplyEmployeeDeptName = ds.Tables[0].Rows[0]["ApplyEmpDeptName"].ToString();
                        stuFormInfo.intApplyEmployeeDeptLevel = int.Parse(ds.Tables[0].Rows[0]["ApplyEmpDeptLevel"].ToString());

                        //發起人
                        stuFormInfo.strStartEmployeeID = ds.Tables[0].Rows[0]["StartEmpID"].ToString();
                        stuFormInfo.strStartEmployeeRoleID = ds.Tables[0].Rows[0]["StartEmpRoleID"].ToString();
                        stuFormInfo.strStartEmployeeName = ds.Tables[0].Rows[0]["StartEmpName"].ToString();
                        stuFormInfo.strStartEmployeeDeptName = ds.Tables[0].Rows[0]["StartEmpDeptName"].ToString();
                        stuFormInfo.strStartEmployeeJobName = ds.Tables[0].Rows[0]["StartEmpJobName"].ToString();

                        //登入者資訊存到Struct
                        stuFormInfo.strSignMEmployeeName = ds.Tables[4].Rows[0]["EmployeeName"].ToString();
                        stuFormInfo.strSignMEmployeeRoleID = ds.Tables[4].Rows[0]["RoleID"].ToString();
                        stuFormInfo.strSignMEmployeeDeptID = ds.Tables[4].Rows[0]["DepartmentID"].ToString();
                        stuFormInfo.strSignMEmployeeDeptName = ds.Tables[4].Rows[0]["DepartmentName"].ToString();
                        stuFormInfo.strSignMEmployeeJobID = ds.Tables[4].Rows[0]["JobID"].ToString();
                        stuFormInfo.strSignMEmployeeJobName = ds.Tables[4].Rows[0]["JobName"].ToString();

                        // 申請原因
                        stuFormInfo.strApplyReason = ds.Tables[0].Rows[0]["ApplyReason"].ToString();

                        //簽核歷程資訊
                        stuFormInfo.dtFormSingMList = ds.Tables[1];

                        //現在要簽核的簽核者資訊
                        stuFormInfo.dtSingerRoleList = ds.Tables[2];

                        // 上傳檔案 TABLE                 
                        stuFormInfo.dtFileUpload = ds.Tables[3];

                        if (stuFormInfo.dtSingerRoleList.Rows.Count > 0)
                        {
                            // 現在要簽核的人的roleID跟EmpID
                            stuFormInfo.strSignerEmployeeRoleID = ds.Tables[2].Rows[0]["SignerEmpRoleID"].ToString();
                            stuFormInfo.strSignerEmployeeID = ds.Tables[2].Rows[0]["SignerEmpID"].ToString();
                            stuFormInfo.intSignerEmployeeDeptLevel = int.Parse(ds.Tables[2].Rows[0]["SignerEmpDeptLevel"].ToString());

                            //  顯示正在等待誰簽核的Label
                            stuFormInfo.strSignerEmployeeDeptName = ds.Tables[2].Rows[0]["SignerEmpDeptName"].ToString();
                            stuFormInfo.strSignerEmployeeJobName = ds.Tables[2].Rows[0]["SignerEmpJobName"].ToString();
                            stuFormInfo.strSignerEmployeeName = ds.Tables[2].Rows[0]["SignerEmpName"].ToString();

                        }

                    }
                }
                return stuFormInfo;
            }


            /// <summary>
            /// 送出時取得最新一筆ProcessID
            /// </summary>
            /// <returns>最新一筆ProcessID</returns>
            public int GetSendProcessID()
            {
                int intProcessID;
                dbFunction dbFunction = new dbFunction();
                using (SqlConnection connection = dbFunction.sqlHissFlowConnection())
                {
                    connection.Open();
                    string strQuery = "insert into ProcessID DEFAULT VALUES";
                    using (SqlCommand cmd = new SqlCommand(strQuery, connection))
                    {
                        //上傳空值到ProcessID
                        cmd.ExecuteNonQuery();
                    }

                    strQuery = "SELECT TOP (1) value FROM ProcessID order by value desc";
                    using (SqlCommand cmd = new SqlCommand(strQuery, connection))
                    {
                        intProcessID = (int)cmd.ExecuteScalar();
                    }
                }

                return intProcessID;
            }

            //// 建立"附件上傳"的DataTable
            //public DataTable GetDtFileUpload()
            //{
            //    DataTable dtFileUpload = new DataTable();
            //    dtFileUpload.Columns.Add("FormCode");
            //    dtFileUpload.Columns.Add("FormName");
            //    dtFileUpload.Columns.Add("ProcessID");
            //    dtFileUpload.Columns.Add("FileName");       //  檔案名稱
            //    dtFileUpload.Columns.Add("ServerName");
            //    dtFileUpload.Columns.Add("FileType");       // 檔案類型
            //    dtFileUpload.Columns.Add("FileSize");       // 檔案大小
            //    dtFileUpload.Columns.Add("FileUploadDate"); // 上傳日期

            //    return dtFileUpload;
            //}

            // 建立"wfFormAppInfo"的DataTable
            //public DataTable GetDtwfFormAppInfo()
            //{
            //    DataTable dtwfFormAppInfo = new DataTable();
            //    dtwfFormAppInfo.Columns.Add("sProcessID");
            //    dtwfFormAppInfo.Columns.Add("idProcess");
            //    dtwfFormAppInfo.Columns.Add("sNobr");
            //    dtwfFormAppInfo.Columns.Add("sName");
            //    dtwfFormAppInfo.Columns.Add("sState");
            //    dtwfFormAppInfo.Columns.Add("sInfo");
            //    dtwfFormAppInfo.Columns.Add("sGuid");
            //    dtwfFormAppInfo.Columns.Add("dKeyDate");

            //    return dtwfFormAppInfo;
            //}

            // 建立"wfFormApp"的DataTable
            //public DataTable GetDtwfFormApp()
            //{
            //    DataTable DtwfFormApp = new DataTable();
            //    DtwfFormApp.Columns.Add("sFormCode");
            //    DtwfFormApp.Columns.Add("sFormName");
            //    DtwfFormApp.Columns.Add("sProcessID");
            //    DtwfFormApp.Columns.Add("idProcess");
            //    DtwfFormApp.Columns.Add("sNobr");
            //    DtwfFormApp.Columns.Add("sName");
            //    DtwfFormApp.Columns.Add("sDept");
            //    DtwfFormApp.Columns.Add("sDeptName");
            //    DtwfFormApp.Columns.Add("sJob");
            //    DtwfFormApp.Columns.Add("sJobName");
            //    DtwfFormApp.Columns.Add("sRole");
            //    DtwfFormApp.Columns.Add("dDateTimeA");
            //    DtwfFormApp.Columns.Add("dDateTimeD");
            //    DtwfFormApp.Columns.Add("bSign");
            //    DtwfFormApp.Columns.Add("sConditions1");
            //    DtwfFormApp.Columns.Add("iCateOrder");
            //    DtwfFormApp.Columns.Add("sLevel");
            //    DtwfFormApp.Columns.Add("sInfo");
            //    DtwfFormApp.Columns.Add("sNote");
            //    DtwfFormApp.Columns.Add("sState");
            //    DtwfFormApp.Columns.Add("bDelay");
            //    DtwfFormApp.Columns.Add("bAuth");

            //    return DtwfFormApp;
            //}

            // 建立"wfFormSignM"的DataTable
            //public DataTable getDtwfFormSignM()
            //{
            //    DataTable wfFormSignM = new DataTable();
            //    wfFormSignM.Columns.Add("sFormCode");
            //    wfFormSignM.Columns.Add("sFormName");
            //    wfFormSignM.Columns.Add("sProcessID");
            //    wfFormSignM.Columns.Add("idProcess");
            //    wfFormSignM.Columns.Add("sKey");
            //    wfFormSignM.Columns.Add("sNobr");
            //    wfFormSignM.Columns.Add("sName");
            //    wfFormSignM.Columns.Add("sDept");
            //    wfFormSignM.Columns.Add("sDeptName");
            //    wfFormSignM.Columns.Add("sJob");
            //    wfFormSignM.Columns.Add("sJobName");
            //    wfFormSignM.Columns.Add("sRole");
            //    wfFormSignM.Columns.Add("sNote");
            //    wfFormSignM.Columns.Add("bSign");
            //    wfFormSignM.Columns.Add("dKeyDate");

            //    return wfFormSignM;
            //}

            /// <summary>
            /// 建立初始化的DataTable
            /// </summary>
            /// <param name="stuFormInfo"></param>
            /// <returns></returns>
            public stuFormInfo GetFormInfoDataTable(stuFormInfo stuFormInfo)
            {
                // 建立"附件上傳"的DataTable
                DataTable dtFileUpload = new DataTable();
                dtFileUpload.Columns.Add("FormCode");       //表單代碼 Ex:IT01
                dtFileUpload.Columns.Add("FormName");       //表單名稱 Ex:資服單
                dtFileUpload.Columns.Add("ProcessID");      //流程序號
                dtFileUpload.Columns.Add("FileName");       // 檔案名稱
                dtFileUpload.Columns.Add("ServerName");     //伺服器名稱
                dtFileUpload.Columns.Add("FileType");       // 檔案類型
                dtFileUpload.Columns.Add("FileSize");       // 檔案大小
                dtFileUpload.Columns.Add("FileUploadDate"); // 上傳日期
                stuFormInfo.dtFileUpload = dtFileUpload;

                // 建立寫入EzFlow資料庫中"wfFormAppInfo"資料表的DataTable
                DataTable dtwfFormAppInfo = new DataTable();
                dtwfFormAppInfo.Columns.Add("sProcessID");  //流程序號
                dtwfFormAppInfo.Columns.Add("idProcess");   //流程序號
                dtwfFormAppInfo.Columns.Add("sNobr");       //申請人工號
                dtwfFormAppInfo.Columns.Add("sName");       //申請人姓名
                dtwfFormAppInfo.Columns.Add("sState");      //流程進行中狀態
                dtwfFormAppInfo.Columns.Add("sInfo");       //表單資訊
                dtwfFormAppInfo.Columns.Add("sGuid");       //上傳時的GUID
                dtwfFormAppInfo.Columns.Add("dKeyDate");    //建立時間
                stuFormInfo.dtwfFormAppInfo = dtwfFormAppInfo;

                // 建立寫入EzFlow資料庫中"wfFormApp"資料表的DataTable
                DataTable dtwfFormApp = new DataTable();
                dtwfFormApp.Columns.Add("sFormCode");       //表單代碼 Ex:IT01
                dtwfFormApp.Columns.Add("sFormName");       //表單名稱 Ex:資服單
                dtwfFormApp.Columns.Add("sProcessID");      //流程序號
                dtwfFormApp.Columns.Add("idProcess");       //流程序號
                dtwfFormApp.Columns.Add("sNobr");           //申請人工號
                dtwfFormApp.Columns.Add("sName");           //申請人姓名
                dtwfFormApp.Columns.Add("sDept");           //申請人部門ID
                dtwfFormApp.Columns.Add("sDeptName");       //申請人部門名稱
                dtwfFormApp.Columns.Add("sJob");            //申請人職位ID
                dtwfFormApp.Columns.Add("sJobName");        //申請人職位姓名
                dtwfFormApp.Columns.Add("sRole");           //申請人角色ID
                dtwfFormApp.Columns.Add("dDateTimeA");      //申請日期
                dtwfFormApp.Columns.Add("dDateTimeD");      //需求日期
                dtwfFormApp.Columns.Add("bSign");           //簽核狀態 1簽核 0駁回
                dtwfFormApp.Columns.Add("sConditions1");    //簽核人部門Level層級 組長50 課長60 部長70 處長80
                dtwfFormApp.Columns.Add("iCateOrder");      //申請人部門Level層級 組長50 課長60 部長70 處長80 int
                dtwfFormApp.Columns.Add("sLevel");          //申請人部門Level層級 組長50 課長60 部長70 處長80 string
                dtwfFormApp.Columns.Add("sInfo");           //表單資訊
                dtwfFormApp.Columns.Add("sNote");           //申請原因
                dtwfFormApp.Columns.Add("sState");          //流程狀態 1進行中 3完成 7取消申請
                dtwfFormApp.Columns.Add("bDelay");          //是否有延遲 bool
                dtwfFormApp.Columns.Add("bAuth");           //是否有權限 bool
                stuFormInfo.dtwfFormApp = dtwfFormApp;

                // 建立寫入EzFlow資料庫中"wfFormSignM"資料表的DataTable
                DataTable dtwfFormSignM = new DataTable();
                dtwfFormSignM.Columns.Add("sFormCode");     //表單代碼 Ex:IT01
                dtwfFormSignM.Columns.Add("sFormName");     //表單名稱 Ex:資服單
                dtwfFormSignM.Columns.Add("sProcessID");    //流程序號
                dtwfFormSignM.Columns.Add("idProcess");     //流程序號
                dtwfFormSignM.Columns.Add("sKey");          //ApParm的流程序號
                dtwfFormSignM.Columns.Add("sNobr");         //簽核人工號
                dtwfFormSignM.Columns.Add("sName");         //簽核人姓名
                dtwfFormSignM.Columns.Add("sDept");         //簽核人部門ID
                dtwfFormSignM.Columns.Add("sDeptName");     //簽核人部門姓名
                dtwfFormSignM.Columns.Add("sJob");          //簽核人職位ID
                dtwfFormSignM.Columns.Add("sJobName");      //簽核人職位名稱
                dtwfFormSignM.Columns.Add("sRole");         //簽核人角色ID
                dtwfFormSignM.Columns.Add("sNote");         //簽核意見
                dtwfFormSignM.Columns.Add("bSign");         //簽核狀態 0駁回 1核准
                dtwfFormSignM.Columns.Add("dKeyDate");      //建立時間
                stuFormInfo.dtwfFormSignM = dtwfFormSignM;

                return stuFormInfo;
            }

            //public DataTable makeFormDataTable(string strDataTableName)
            //{
            //    DataTable dt = new DataTable();

            //    switch (strDataTableName)
            //    {
            //        case "DtwfFormApp":
            //            dt.Columns.Add("sFormCode");
            //            dt.Columns.Add("sFormName");
            //            dt.Columns.Add("sProcessID");
            //            dt.Columns.Add("idProcess");
            //            dt.Columns.Add("sNobr");
            //            dt.Columns.Add("sName");
            //            dt.Columns.Add("sDept");
            //            dt.Columns.Add("sDeptName");
            //            dt.Columns.Add("sJob");
            //            dt.Columns.Add("sJobName");
            //            dt.Columns.Add("sRole");
            //            dt.Columns.Add("dDateTimeA");
            //            dt.Columns.Add("dDateTimeD");
            //            dt.Columns.Add("bSign");
            //            dt.Columns.Add("sConditions1");
            //            dt.Columns.Add("iCateOrder");
            //            dt.Columns.Add("sLevel");
            //            dt.Columns.Add("sInfo");
            //            dt.Columns.Add("sNote");
            //            dt.Columns.Add("sState");
            //            dt.Columns.Add("bDelay");
            //            dt.Columns.Add("bAuth");
            //            break;
            //        case "wfFormSignM":
            //            dt.Columns.Add("sFormCode");
            //            dt.Columns.Add("sFormName");
            //            dt.Columns.Add("sProcessID");
            //            dt.Columns.Add("idProcess");
            //            dt.Columns.Add("sKey");
            //            dt.Columns.Add("sNobr");
            //            dt.Columns.Add("sName");
            //            dt.Columns.Add("sDept");
            //            dt.Columns.Add("sDeptName");
            //            dt.Columns.Add("sJob");
            //            dt.Columns.Add("sJobName");
            //            dt.Columns.Add("sRole");
            //            dt.Columns.Add("sNote");
            //            dt.Columns.Add("bSign");
            //            dt.Columns.Add("dKeyDate");
            //            break;

            //    }


            //    return dt;
            //}

            // "附件上傳"的SqlbulkCopy上傳資料庫

            public void SqlbulkCopyFileUpload(stuFormInfo stuFormInfo)
            {
                foreach (DataRow drFileUpload in stuFormInfo.dtFileUpload.Rows)
                {
                    drFileUpload["FormCode"] = stuFormInfo.strFormCode;
                    drFileUpload["FormName"] = stuFormInfo.strFormName;
                    drFileUpload["ProcessID"] = stuFormInfo.intProcessID;
                }

                dbFunction dbFunction = new dbFunction();
                using (SqlConnection connection = dbFunction.sqlHissMingConnection())
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
                        bulkCopy.WriteToServer(stuFormInfo.dtFileUpload);
                    }
                }
            }
           
            // "wfFormApp"、"wfFormAppInfo"用SqlbulkCopy上傳資料庫
            public void SqlBulkCopyToFlow(stuFormInfo stuFormInfo)
            {
                DataRow drwfFormAppInfo = stuFormInfo.dtwfFormAppInfo.NewRow();
                drwfFormAppInfo["sProcessID"] = stuFormInfo.intProcessID.ToString();
                drwfFormAppInfo["idProcess"] = stuFormInfo.intProcessID;
                drwfFormAppInfo["sNobr"] = stuFormInfo.strApplyEmployeeID;
                drwfFormAppInfo["sName"] = stuFormInfo.strApplyEmployeeName;
                drwfFormAppInfo["sState"] = "1";
                drwfFormAppInfo["sInfo"] = "(測試)" + stuFormInfo.strApplyEmployeeName+"的矯正預防單"; //+ "，需求日期：" + stuFormInfo.strRequireDate;
                drwfFormAppInfo["sGuid"] = Guid.NewGuid().ToString();
                drwfFormAppInfo["dKeyDate"] = DateTime.Now.ToString();
                stuFormInfo.dtwfFormAppInfo.Rows.Add(drwfFormAppInfo);

                DataRow drwfFormApp = stuFormInfo.dtwfFormApp.NewRow();
                drwfFormApp["sFormCode"] = stuFormInfo.strFormCode;
                drwfFormApp["sFormName"] = stuFormInfo.strFormName;
                drwfFormApp["sProcessID"] = stuFormInfo.intProcessID.ToString();
                drwfFormApp["idProcess"] = stuFormInfo.intProcessID;
                drwfFormApp["sNobr"] = stuFormInfo.strApplyEmployeeID;
                drwfFormApp["sName"] = stuFormInfo.strApplyEmployeeName;
                drwfFormApp["sDept"] = stuFormInfo.strApplyEmployeeDeptID;
                drwfFormApp["sDeptName"] = stuFormInfo.strApplyEmployeeDeptName;
                drwfFormApp["sJob"] = stuFormInfo.strApplyEmployeeJobID;
                drwfFormApp["sJobName"] = stuFormInfo.strApplyEmployeeJobName;
                drwfFormApp["sRole"] = stuFormInfo.strApplyEmployeeRoleID;
                drwfFormApp["dDateTimeA"] = DateTime.Now;
                drwfFormApp["dDateTimeD"] = DateTime.Now;//DateTime.Parse(stuFormInfo.strRequireDate);
                drwfFormApp["bSign"] = true;
                drwfFormApp["sConditions1"] = "60";
                drwfFormApp["iCateOrder"] = stuFormInfo.intApplyEmployeeDeptLevel;
                drwfFormApp["sLevel"] = stuFormInfo.intApplyEmployeeDeptLevel.ToString();
                drwfFormApp["sInfo"] = "(測試)" + stuFormInfo.strApplyEmployeeName+"的矯正預防單";// + "，需求日期：" + stuFormInfo.strRequireDate;
                drwfFormApp["sNote"] = stuFormInfo.strApplyReason;
                drwfFormApp["sState"] = "1";
                drwfFormApp["bDelay"] = false;
                drwfFormApp["bAuth"] = false;
                stuFormInfo.dtwfFormApp.Rows.Add(drwfFormApp);

                dbFunction dbFunction = new dbFunction();
                using (SqlConnection connection = dbFunction.sqlHissFlowConnection())
                {
                    connection.Open();                   
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        // 上傳到wfFormAppInfo
                        bulkCopy.DestinationTableName = "wfFormAppInfo";
                        bulkCopy.ColumnMappings.Add("sProcessID", "sProcessID");
                        bulkCopy.ColumnMappings.Add("idProcess", "idProcess");
                        bulkCopy.ColumnMappings.Add("sNobr", "sNobr");
                        bulkCopy.ColumnMappings.Add("sName", "sName");
                        bulkCopy.ColumnMappings.Add("sState", "sState");
                        bulkCopy.ColumnMappings.Add("sInfo", "sInfo");
                        bulkCopy.ColumnMappings.Add("sGuid", "sGuid");
                        bulkCopy.ColumnMappings.Add("dKeyDate", "dKeyDate");
                        bulkCopy.WriteToServer(stuFormInfo.dtwfFormAppInfo);
                    }
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        // 上傳wfFormApp
                        bulkCopy.DestinationTableName = "wfFormApp";
                        bulkCopy.ColumnMappings.Add("sFormCode", "sFormCode");
                        bulkCopy.ColumnMappings.Add("sFormName", "sFormName");
                        bulkCopy.ColumnMappings.Add("sProcessID", "sProcessID");
                        bulkCopy.ColumnMappings.Add("idProcess", "idProcess");
                        bulkCopy.ColumnMappings.Add("sNobr", "sNobr");
                        bulkCopy.ColumnMappings.Add("sName", "sName");
                        bulkCopy.ColumnMappings.Add("sDept", "sDept");
                        bulkCopy.ColumnMappings.Add("sDeptName", "sDeptName");
                        bulkCopy.ColumnMappings.Add("sJob", "sJob");
                        bulkCopy.ColumnMappings.Add("sJobName", "sJobName");
                        bulkCopy.ColumnMappings.Add("sRole", "sRole");
                        bulkCopy.ColumnMappings.Add("dDateTimeA", "dDateTimeA");
                        bulkCopy.ColumnMappings.Add("dDateTimeD", "dDateTimeD");
                        bulkCopy.ColumnMappings.Add("bSign", "bSign");
                        bulkCopy.ColumnMappings.Add("sConditions1", "sConditions1");
                        bulkCopy.ColumnMappings.Add("iCateOrder", "iCateOrder");
                        bulkCopy.ColumnMappings.Add("sLevel", "sLevel");
                        bulkCopy.ColumnMappings.Add("sInfo", "sInfo");
                        bulkCopy.ColumnMappings.Add("sNote", "sNote");
                        bulkCopy.ColumnMappings.Add("sState", "sState");

                        bulkCopy.ColumnMappings.Add("bDelay", "bDelay");
                        bulkCopy.ColumnMappings.Add("bAuth", "bAuth");

                        bulkCopy.WriteToServer(stuFormInfo.dtwfFormApp);
                    }
                }
            }
            // 簽核程式，BSign為簽核與否，SState為狀態(1進行中，3完成，7取消申請)
            public void FlowApprove(stuFormInfo stuFormInfo)
            {
                dbFunction dbFunction = new dbFunction();
                using (SqlConnection conn = dbFunction.sqlHissShareElecFormConnection())
                {
                    conn.Open();

                    string strQuery = "spFlowApprove";
                    SqlCommand cmd = new SqlCommand(strQuery, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@strProcessID", stuFormInfo.intProcessID.ToString());
                    cmd.Parameters.AddWithValue("@strFormID", stuFormInfo.strFormID);
                    cmd.Parameters.AddWithValue("@bSign", stuFormInfo.blnSignState);
                    cmd.Parameters.AddWithValue("@sState", stuFormInfo.intState);
                    cmd.Parameters.AddWithValue("@strSignEmpRoleID", stuFormInfo.strSignerEmployeeRoleID);
                    cmd.Parameters.AddWithValue("@strSignEmpID", stuFormInfo.strSignerEmployeeID);
                    cmd.Parameters.AddWithValue("@intSignEmpDeptLevel", stuFormInfo.intSignerEmployeeDeptLevel);

                    cmd.ExecuteNonQuery(); // 用於更新資料庫資料
                }

                //WorkFinish
                ezEngineServices.Service oService = new ezEngineServices.Service(dbFunction.sqlHissFlowConnection());
                oService.WorkFinish(stuFormInfo.intApKey);
            }

            //  寫入[wfFormSignM]程式(共用)
            public void InsertFormSignM(stuFormInfo stuFormInfo)
            {
                DataRow drwfFormSignM = stuFormInfo.dtwfFormSignM.NewRow();

                drwfFormSignM["sFormCode"] = stuFormInfo.strFormCode;
                drwfFormSignM["sFormName"] = stuFormInfo.strFormName;
                drwfFormSignM["sProcessID"] = stuFormInfo.intProcessID.ToString();
                drwfFormSignM["idProcess"] = stuFormInfo.intProcessID;
                drwfFormSignM["sKey"] = stuFormInfo.intApKey.ToString();
                drwfFormSignM["sNobr"] = stuFormInfo.strLoginEmployeeID;
                drwfFormSignM["sName"] = stuFormInfo.strSignMEmployeeName;
                drwfFormSignM["sDept"] = stuFormInfo.strSignMEmployeeDeptID;
                drwfFormSignM["sDeptName"] = stuFormInfo.strSignMEmployeeDeptName;
                drwfFormSignM["sJob"] = stuFormInfo.strSignMEmployeeJobID;
                drwfFormSignM["sJobName"] = stuFormInfo.strSignMEmployeeJobName;
                drwfFormSignM["sRole"] = stuFormInfo.strSignMEmployeeRoleID;
                drwfFormSignM["sNote"] = "(" + stuFormInfo.strSignMState + ")" + stuFormInfo.strSignOpinion;
                drwfFormSignM["bSign"] = stuFormInfo.blnSignM;
                drwfFormSignM["dKeyDate"] = DateTime.Now;

                stuFormInfo.dtwfFormSignM.Rows.Add(drwfFormSignM);

                dbFunction dbFunction = new dbFunction();
                using (SqlConnection connection = dbFunction.sqlHissFlowConnection())
                {
                    connection.Open();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        // 上傳
                        bulkCopy.DestinationTableName = "wfFormSignM";
                        bulkCopy.ColumnMappings.Add("sFormCode", "sFormCode");
                        bulkCopy.ColumnMappings.Add("sFormName", "sFormName");
                        bulkCopy.ColumnMappings.Add("sProcessID", "sProcessID");
                        bulkCopy.ColumnMappings.Add("idProcess", "idProcess");
                        bulkCopy.ColumnMappings.Add("sKey", "sKey");
                        bulkCopy.ColumnMappings.Add("sNobr", "sNobr");
                        bulkCopy.ColumnMappings.Add("sName", "sName");
                        bulkCopy.ColumnMappings.Add("sDept", "sDept");
                        bulkCopy.ColumnMappings.Add("sDeptName", "sDeptName");
                        bulkCopy.ColumnMappings.Add("sJob", "sJob");
                        bulkCopy.ColumnMappings.Add("sJobName", "sJobName");
                        bulkCopy.ColumnMappings.Add("sRole", "sRole");
                        bulkCopy.ColumnMappings.Add("sNote", "sNote");
                        bulkCopy.ColumnMappings.Add("bSign", "bSign");
                        bulkCopy.ColumnMappings.Add("dKeyDate", "dKeyDate");

                        bulkCopy.WriteToServer(stuFormInfo.dtwfFormSignM);
                    }
                }
            }
        }
    }
}
