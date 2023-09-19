<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Template.aspx.cs" Inherits="BPM.FlowWork.Template" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
    <script>
        // 使用 window.addEventListener 等待頁面載入完成後執行程式碼
        window.addEventListener('DOMContentLoaded', function () {
            // 取得要綁定 Flatpickr 的元素，這裡假設是一個 ASP.NET 的 TextBox 控制項
            var txbRequireDate = document.getElementById('<%= txbRequireDate.ClientID %>');

            // 初始化 Flatpickr
            flatpickr(txbRequireDate, {
                locale: "zh",
                // 在此設定 Flatpickr 的選項
                minDate: new Date().fp_incr(7),
                defaultDate:"<%=strRequireDate%>"
            });
        });
    </script>
    <style type="text/css">
        .center-panel {
            padding: 10px;
            margin: 0 auto;
            width: 1000px;
            box-shadow: 0 0 20px #666;
            background: #fff;
        }

        .title-style {
            font-family: 微軟正黑體;
            font-size: medium;
            font-weight: bold;
        }

        .gray-row {
            background-color: #E0E0E0;
            border: 1px solid #000;
            text-align: center;
            font-family: 微軟正黑體;
            height: 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" class="center-panel">
        <asp:ScriptManager ID="ScriptManager" runat="server"></asp:ScriptManager>
        <asp:Panel ID="pnlBegin" runat="server">
            <table width="100%">
                <tr>
                    <td width="75%">表單序號:<asp:Label ID="lblProcessID" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Button ID="btnHome" runat="server" Text="首頁" CssClass="title-style" OnClick="btnHome_Click" />
                    </td>
                    <td>
                        <asp:Label ID="lblLoginEmpID" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblLoginEmpName" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:LoginStatus ID="LoginStatus1" runat="server" />
                    </td>
                </tr>
            </table>
            <table width="100%">
                <tr>
                    <td style="text-align: center;">
                        <div style="background-color: lightgray;">
                            <asp:Label ID="lblFlowName" runat="server" CssClass="title-style"></asp:Label>
                        </div>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlDate" runat="server">
            <table width="100%">
                <tr>
                    <td>
                        <div class="title-style">
                            ◆ 申請類型
                        </div>
                    </td>
                </tr>
                <tr>
                    <td width="25%">
                        <div style="width: 20px; display: inline-block;"></div>
                            申請日期:
                            <asp:Label ID="lblApplyDate" runat="server"></asp:Label>
                    </td>
                    <td>
                        <div>
                            需求日期:
                            <font color="red">*</font>
                            <asp:TextBox ID="txbRequireDate" runat="server"></asp:TextBox>
                        </div>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlPersonInformation" runat="server">
            <table width="100%">
                <tr>
                    <td>
                        <div class="title-style">
                            ◆ 發起人資訊
                            <asp:Label ID="lblStartEmpRoleID" runat="server" />
                        </div>
                    </td>
                </tr>
                <tr class="gray-row">
                    <td width="25%">姓名</td>
                    <td width="25%">職稱</td>
                    <td width="25%">部門</td>
                    <td width="25%">選擇申請人</td>
                </tr>
                <tr style="text-align: center">
                    <td>
                        <asp:Label ID="lblStartEmpName" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblStartEmpJobName" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlSelectStartEmpDept" runat="server" DataTextField="DeptAndJob" DataValueField="DepartmentID" AutoPostBack="True" Font-Size="Medium" OnSelectedIndexChanged="ddlSelectStartEmpDept_SelectedIndexChanged"></asp:DropDownList>
                        <asp:Label ID="lblStartEmpDeptName" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlSelectApplyEmp" runat="server" DataTextField="EmployeeName" DataValueField="EmployeeID" AutoPostBack="True" Font-Size="Medium" OnSelectedIndexChanged="ddlSelectApplyEmp_SelectedIndexChanged"></asp:DropDownList>
                        <asp:Label ID="lblSelectApplyEmp" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="title-style">
                            ◆ 申請人資訊
                        </div>
                    </td>
                </tr>
                <tr class="gray-row">
                    <td>姓名</td>
                    <td>職稱</td>
                    <td>部門</td>
                    <td></td>
                </tr>
                <tr style="text-align: center">
                    <td>
                        <asp:Label ID="lblApplyEmpName" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblApplyEmpJobName" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblApplyEmpDeptName" runat="server"></asp:Label>
                    </td>
                </tr>

            </table>
        </asp:Panel>
        <asp:Panel ID="pnlApplyReason" runat="server">
            <table width="100%">
                <tr>
                    <td width="15%">
                        <div class="title-style">
                            ◆ 申請原因
                        </div>
                    </td>
                    <td>
                        <asp:TextBox ID="txbApplyReason" runat="server" TextMode="MultiLine" Width="830px" Height="50px"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlFileUpload" runat="server">
            <table>
                <tr>
                    <td>
                        <div class="title-style">
                            ◆ 附件上傳
                        </div>
                    </td>
                </tr>
                <tr>
                    <td width="50%">
                        <asp:FileUpload ID="FileUpload1" runat="server" />
                    </td>
                    <td width="30%">
                        <asp:Button ID="btnFileUpload" runat="server" Text="上傳" OnClick="btnFileUpload_Click" />
                    </td>
                    <td width="20%">
                        <asp:Label ID="lblFileUploadErrMsg" runat="server" ForeColor="Red" Text="檔案大小不可超過20MB"></asp:Label>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:GridView ID="grvFileUpload" runat="server" AutoGenerateColumns="False" Width="100%" BackColor="White" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" ForeColor="Black" GridLines="Vertical">
            <AlternatingRowStyle BackColor="#CCCCCC" />
            <Columns>
                <asp:BoundField DataField="FileName" HeaderText="檔案名稱" />
                <asp:BoundField DataField="FileSize" HeaderText="檔案大小(KB)" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnDownload" runat="server" CommandArgument="<%# Container.DataItemIndex %>" CommandName="FileDownload" OnCommand="lbtnDownload_Command" Text="下載"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnFileDelete" runat="server" CommandArgument="<%# Container.DataItemIndex %>" CommandName="FileDelete" OnClientClick="return confirm('確定刪除附件？');" OnCommand="lbtnFileDelete_Command" Text="刪除"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <FooterStyle BackColor="#CCCCCC" />
            <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#F1F1F1" />
            <SortedAscendingHeaderStyle BackColor="#808080" />
            <SortedDescendingCellStyle BackColor="#CAC9C9" />
            <SortedDescendingHeaderStyle BackColor="#383838" />
        </asp:GridView>

        <br />
        <asp:Panel ID="pnlSend" runat="server">
            <asp:Button ID="btnSend" runat="server" Text="送出" CssClass="title-style" OnClientClick="return confirm('確定要送出申請嗎？');" Visible="true" OnClick="btnSend_Click" />
        </asp:Panel>

        <asp:Panel ID="pnlCheck" runat="server">
            <table width="100%">
                <tr>
                    <td style="text-align: center;">
                        <div style="background-color: lightgray;">
                            <asp:Label ID="lblCheck" runat="server" Text="審核區"></asp:Label>
                        </div>
                    </td>
                </tr>
            </table>
            <table width="100%">
                <tr>
                    <td width="15%">
                        <div class="title-style">
                            ◆ 簽核意見
                        </div>
                    </td>
                    <td>
                        <asp:TextBox ID="txbSignOpinion" runat="server" TextMode="MultiLine" Width="830px" Height="50px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td width="15%">
                        <div class="title-style">
                            ◆ 簽核紀錄
                        </div>
                    </td>
                </tr>
            </table>
            <div style="text-align: center;">
                <asp:Label ID="lblStatus" runat="server" ForeColor="Red" Font-Bold="true" Font-Names="微軟正黑體" Font-Size="Medium"></asp:Label>
            </div>

            <asp:GridView ID="grvFormSignM" runat="server" Width="100%" AutoGenerateColumns="False" BackColor="White" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" ForeColor="Black" GridLines="Vertical">
                <AlternatingRowStyle BackColor="#CCCCCC" />
                <Columns>
                    <asp:BoundField DataField="工號" HeaderText="工號" />
                    <asp:BoundField DataField="姓名" HeaderText="姓名" />
                    <asp:BoundField DataField="部門" HeaderText="部門" />
                    <asp:BoundField DataField="職稱" HeaderText="職稱" />
                    <asp:BoundField DataField="意見" HeaderText="意見" />
                    <asp:BoundField DataField="簽核日期" HeaderText="簽核日期" />
                </Columns>
                <FooterStyle BackColor="#CCCCCC" />
                <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                <SortedAscendingCellStyle BackColor="#F1F1F1" />
                <SortedAscendingHeaderStyle BackColor="#808080" />
                <SortedDescendingCellStyle BackColor="#CAC9C9" />
                <SortedDescendingHeaderStyle BackColor="#383838" />
            </asp:GridView>
        </asp:Panel>
        <asp:Panel ID="pnlBtn" runat="server">
            <div style="text-align: center;">
                <asp:Button ID="btnSubmit" runat="server" Text="簽核" CssClass="title-style" OnClientClick="return confirm('確定簽核？');" OnClick="btnSubmit_Click" />
                <asp:Button ID="btnReject" runat="server" Text="駁回" CssClass="title-style" OnClientClick="return confirm('確定駁回？');" OnClick="btnReject_Click" />
                <asp:Button ID="btnTake" runat="server" Text="取消申請" CssClass="title-style" OnClientClick="return confirm('確定取消申請？');" OnClick="btnTake_Click" />
            </div>

        </asp:Panel>
    </form>
</body>
</html>
