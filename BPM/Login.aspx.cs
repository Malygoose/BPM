using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BPM
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                string ReturnUrl = Convert.ToString(Request.QueryString["ReturnUrl"]);
                if (!string.IsNullOrEmpty(ReturnUrl))
                {
                    Response.Redirect(ReturnUrl);
                }
            }
        }

        private bool ValidateCredentials(string username, string password)
        {
            // 在此撰寫帳號密碼驗證的邏輯，例如查詢資料庫中的 user 資料表

            // 假設資料庫中有一個 user 資料表，包含 username 和 password 欄位
            string connectionString = ConfigurationManager.ConnectionStrings["Flow"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Emp WHERE id = @Username ";//AND pw = @pw";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                //cmd.Parameters.AddWithValue("@pw", password);
                //cmd.Parameters.AddWithValue("@Password", password);
                conn.Open();
                int result = (int)cmd.ExecuteScalar();
                return result > 0;
            }
        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            if (ValidateCredentials(Login1.UserName, Login1.Password))
            {
                string strEmployeeName = GetEmployeeName();
                //Session
                Session["EmployeeName"] = strEmployeeName;

                //Cookies
                //HttpCookie cookie = new HttpCookie("EmployeeName");
                //cookie.Value = strEmployeeName;
                //Response.Cookies.Add(cookie);

                e.Authenticated = true;
            }
            else
            {
                e.Authenticated = false;
                // 驗證失敗，顯示錯誤訊息
                lblError.Text = "帳號或密碼錯誤";
            }
        }

        private string GetEmployeeName()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Flow"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "select EmployeeName from ViewEmployeeRoleList where EmployeeID = @sNobr";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@sNobr", Login1.UserName);

                    conn.Open();
                    string strEmployeeName = cmd.ExecuteScalar().ToString();

                    return strEmployeeName;
                }
            }
        }
    }
}