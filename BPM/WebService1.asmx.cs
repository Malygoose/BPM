using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Services;

namespace BPM
{
    /// <summary>
    ///WebService1 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
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
        public string SendMail()
        {                    
            try
            {
                //創建一個電子郵件
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("chiawei.chang@hiss.com.tw"); //發
                mail.To.Add("chiawei.chang@hiss.com.tw");//收
                mail.Subject = "(測試)【通知】(20230305)張家瑋 之共通表單";
                mail.Body = "(測試)Portal登入網址：http://192.168.1.26:8084/login\r\n" +
                            "工號:20230305\r\n" +
                            "姓名:張家瑋\r\n" +
                            "部門:系統設計課\r\n";

                //創建一個SMTP客戶端
                SmtpClient smtpClient = new SmtpClient("msa.hinet.net");//mail.hiss.com.tw、msa.hinet.net     SMTP服務器地址
                smtpClient.Port = 587;//110、25                                                               SMTP端口號
                smtpClient.Credentials = new NetworkCredential("chiawei.chang@hiss.com.tw", "T124382686");//webMail 地址密碼
                smtpClient.EnableSsl = true; //啟用SSL加密

                smtpClient.Send(mail);//發送

                return "郵件發送成功";
            }
            catch (Exception ex)
            {
                return "郵件發送失敗 " + ex.Message;
            }
        }
    }
}
