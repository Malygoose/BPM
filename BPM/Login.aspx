<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="BPM.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
     <style type="text/css">
         body, form {
             background-color: white;
             margin: 0;
             padding: 0;
         }

         .auto-style1 {
             vertical-align: middle;
         }

         .center-panel {
             margin: 0 auto;
             width: 800px;
             background: #fff;
         }
     </style>
</head>
<body>
    <form id="form1" runat="server">
         <asp:Panel CssClass="center-panel" runat="server" Width="70%">
            <table style="width: 100%">
                <tr>
                    <td>
                        <div style="text-align: center; background-color: #FFFFFF; box-shadow: 0 0 20px #666;">
                            <img src="https://i.imgur.com/bcB5axe.png" alt="Main Title" width:"100%" />
                            <br />
                            <br />
                            <br />
                            <div style="display: flex; justify-content: center; align-items: center; height: 100%;">
                                <asp:Login ID="Login1" runat="server" OnAuthenticate="Login1_Authenticate" DestinationPageUrl="~/FlowWork/Home.aspx" UserNameLabelText="帳號:" DisplayRememberMe="False" TitleText=""></asp:Login>
                            </div>
                            <br />
                            <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
                            <br />
                        </div>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </form>
</body>
</html>
