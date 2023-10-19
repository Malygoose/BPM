using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using static BPMLib.Class1;
using static BPMLib.Class1.FormInfo;
using System.Data;
using ClosedXML.Excel;

namespace BPM
{
    /// <summary>
    ///WebService1 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld(string Name)
        {
            return "Hello World " + Name + " !";
        }

        [WebMethod]
        public double GetBMI(double Weight, double Height)
        { 
            return Weight/Math.Pow( Height/100,2);
        }

        [WebMethod]
        public List<string> GetMatchingData(string prefixText)
        {
            List<string> matchingData = new List<string>();

            dbFunction dbFunction = new dbFunction();
            //連線
            using (SqlConnection conn = dbFunction.sqlHissFlowConnection())
            {
                conn.Open();
            
                string query = "spInputEmpNumToSearch";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@prefixText", prefixText + "%");

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            matchingData.Add(reader["EmployeeID"].ToString());
                        }
                    }
                }
            }
            return matchingData;
        }

        [WebMethod]
        public string SendMail()
        {                    
            try
            {
                //創建一個電子郵件
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("hiss.it@hiss.com.tw"); //發
                mail.To.Add("chiawei.chang@hiss.com.tw");//收
                mail.Subject = "(測試)";
                //mail.Body = "(測試)Portal登入網址：<a href='http://192.168.1.26:8084/login'>http://192.168.1.26:8084/login</a><br>" +
                //            "<span style='color:red;'>此信件為系統自動寄送，請勿直接回信，若有疑問請洽MIS，謝謝您！</span>";
                //mail.IsBodyHtml = true;

                mail.IsBodyHtml = true;
                mail.Body = "您有一筆資服單尚未簽核，請到<br>Portal登入網址：<a href='http://192.168.1.26:8084/login'>http://192.168.1.26:8084/login</a>" +
                            "<br>做簽核，謝謝!<br><br><br>" +
                            "<font color='red'>此信件為系統自動寄送，請勿直接回信，若有疑問請洽MIS，謝謝您！</font>";

                //創建一個SMTP客戶端
                SmtpClient smtpClient = new SmtpClient("msa.hinet.net");//mail.hiss.com.tw、msa.hinet.net     SMTP服務器地址
                smtpClient.Port = 587;//110、25                                                               SMTP端口號
                smtpClient.Credentials = new NetworkCredential("hiss.it@hiss.com.tw", "Aa123456");//webMail 地址密碼
                smtpClient.EnableSsl = true; //啟用SSL加密

                smtpClient.Send(mail);//發送

                return "郵件發送成功";
            }
            catch (Exception ex)
            {
                return "郵件發送失敗 " + ex.Message;
            }
        }

        [WebMethod]
        public string EmailCheckFields(string email)
        {
            string pattern = @"^\w+([-+.']\w+)*@hiss\.com\.tw$";
            if (System.Text.RegularExpressions.Regex.IsMatch(email, pattern))
            {
                dbFunction dbFunction = new dbFunction();

                

                using (SqlConnection conn = dbFunction.sqlHissMingConnection())
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("spIT01EmployeeItemsCheckFields", conn);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@strEmail", email);

                    return (string)cmd.ExecuteScalar();
                }
            }
            else
            {
                return "Emiail格式有誤";
            }
        }
    }
}
