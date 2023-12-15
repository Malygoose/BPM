<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IT01.aspx.cs" Inherits="BPM.FlowWork.IT01" %>

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
                minDate: new Date().fp_incr(0),
                defaultDate: "<%=strRequireDate%>"
            });
        });
    </script>
    <style type="text/css">
        .center-panel {
            padding: 0px;
            margin: 0 auto;
            width: 1000px;
            box-shadow: 0 0 20px #666;
            background: #fff;
        }

        .title-style {
            font-family: 微軟正黑體;
            font-size: 18px;
            font-weight: bold;
        }

        .gray-row {
            background-color: #E0E0E0;
            border: 1px solid #000;
            text-align: center;
            font-family: 微軟正黑體;
            height: 20px;
        }

        .center-gridview {
            text-align: center;
        }

        .font-size {
            font-size: 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" class="center-panel">
        <asp:ScriptManager ID="ScriptManager" runat="server"></asp:ScriptManager>
        <asp:Panel ID="pnlBegin" runat="server">
            <table width="100%" class="font-size">
                <tr>
                    <td width="70%">表單序號:<asp:Label ID="lblProcessID" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Button ID="btnHide" runat="server" Text="隱藏" CssClass="title-style" OnClick="btnHide_Click" />
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
                        <div style="background-color: #7D7D7D; height: 25px; padding: 3px">
                            <asp:Label ID="lblElecForm" runat="server" CssClass="title-style" ForeColor="white"></asp:Label>
                        </div>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlDate" runat="server">
            <table width="100%">
                <tr>
                    <td width="25%">
                        <%--<div style="width: 20px; display: inline-block;"></div>--%>
                        <div class="font-size">
                            <asp:Label ID="label1" runat="server" Text="申請日期:" />
                            <asp:Label ID="lblApplyDate" runat="server"></asp:Label>
                        </div>
                    </td>
                    <td>
                        <div class="font-size">
                            <asp:Label ID="label2" runat="server" Text="需求日期:" />
                            <font color="red">*</font>
                            <asp:TextBox ID="txbRequireDate" runat="server" Font-Size="20px"></asp:TextBox>
                        </div>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table width="100%">
            <tr>
                <td>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>

                            <asp:Panel ID="pnlPersonInfo" runat="server">
                                <table width="100%" class="font-size">
                                    <asp:Panel ID="pnlStartEmp" runat="server">
                                        <tr>
                                            <td>
                                                <div class="title-style">
                                                    <asp:Label ID="Label4" runat="server" Text="◆ 發起人資訊"></asp:Label>
                                                    <asp:Label ID="lblStartEmpRoleID" runat="server" Visible="false" />
                                                </div>
                                            </td>
                                        </tr>
                                        <tr class="gray-row">
                                            <td width="20%">姓名</td>
                                            <td width="20%">職稱</td>
                                            <td width="20%">部門</td>
                                            <td width="20%">選擇申請人</td>
                                            <td width="20%">選擇使用人</td>
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
                                            <td>
                                                <asp:DropDownList ID="ddlSelectUser" runat="server" AutoPostBack="True" Font-Size="Medium" DataTextField="EmployeeName" DataValueField="EmployeeID"></asp:DropDownList>
                                                <asp:Label ID="lblSelectUser" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </asp:Panel>
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
                                        <td>到職日期</td>
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
                                        <td>
                                            <asp:Label ID="lblApplyEmpStartDate" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>

                            <asp:Panel ID="pnlChooseApply" runat="server">
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <div class="title-style">
                                                ◆ 申請類型
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:RadioButtonList ID="rbtnlSelectWorking" runat="server" Font-Names="微軟正黑體" OnSelectedIndexChanged="rbtnlSelectWorking_SelectedIndexChanged" RepeatDirection="Horizontal" AutoPostBack="True" Font-Size="20px">
                                                <asp:ListItem Value="AtWork">申請在職變更(新增軟硬體/變更權限)</asp:ListItem>
                                                <asp:ListItem Value="Quit">申請離職(繳回全部設備)</asp:ListItem>
                                                <asp:ListItem Value="Transfer">申請調職(繳回全部設備)</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>

                            <asp:Panel runat="server" ID="pnlApplyEmpDevice" CssClass="font-size">
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <div width="100%" style="font-family: 微軟正黑體; font-size: 20px; font-weight: bold;">
                                                <asp:Label ID="lblApplyEmpHaveItem" runat="server" Text="◆ 申請人現有項目"></asp:Label>
                                            </div>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td>
                                            <asp:GridView ID="grvApplyEmpDevice" runat="server" CssClass="center-gridview" Width="100%" AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" GridLines="None">
                                                <AlternatingRowStyle BackColor="White" />
                                                <Columns>
                                                    <%--                                        <asp:BoundField DataField="姓名" HeaderText="姓名" />
                                        <asp:BoundField DataField="部門" HeaderText="部門" />--%>
                                                    <asp:BoundField DataField="種類" HeaderText="種類" />
                                                    <asp:BoundField DataField="項目" HeaderText="項目" />
                                                    <asp:BoundField DataField="名稱" HeaderText="名稱" />
                                                    <asp:BoundField DataField="AssetsCode" HeaderText="資產編號" />
                                                </Columns>
                                                <EditRowStyle BackColor="#7C6F57" />
                                                <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                                <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                                <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                                                <RowStyle BackColor="#E3EAEB" />
                                                <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                                                <SortedAscendingCellStyle BackColor="#F8FAFA" />
                                                <SortedAscendingHeaderStyle BackColor="#246B61" />
                                                <SortedDescendingCellStyle BackColor="#D4DFE1" />
                                                <SortedDescendingHeaderStyle BackColor="#15524A" />
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>

                            <table width="100%">
                                <tr>
                                    <td>
                                        <div width="100%" style="font-family: 微軟正黑體; font-size: medium; font-weight: bold;">
                                            <asp:Label ID="lblApplyContent" runat="server" Text="◆ 申請內容" Font-Names="微軟正黑體" Font-Size="20px" Font-Bold="true"></asp:Label>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <asp:Panel runat="server" ID="pnlItemAdd" Visible="true" CssClass="font-size">
                                <table width="100%">

                                    <tr>
                                        <td>
                                            <font width="10%">服務：</font>

                                            <div style="width: 10px; display: inline-block;">
                                            </div>
                                            <asp:DropDownList ID="ddlServiceType" runat="server" AutoPostBack="True" Font-Size="12" OnSelectedIndexChanged="ddlServiceType_SelectedIndexChanged" Width="120px">
                                                <%--                                                    <asp:ListItem>新增</asp:ListItem>
                                                    <asp:ListItem>硬體維修</asp:ListItem>
                                                    <asp:ListItem>軟體維護</asp:ListItem>
                                                    <asp:ListItem>繳回設備</asp:ListItem>
                                                    <asp:ListItem>關閉權限</asp:ListItem>--%>
                                            </asp:DropDownList>
                                            <div style="width: 10px; display: inline-block;">
                                            </div>
                                            <asp:DropDownList ID="ddlItemType" runat="server" AutoPostBack="True" DataTextField="TypeName" DataValueField="TypeID" Font-Size="12" OnSelectedIndexChanged="ddlItemType_SelectedIndexChanged" Width="120px">
                                            </asp:DropDownList>
                                            <div style="width: 10px; display: inline-block;">
                                            </div>
                                            <asp:Panel ID="pnlItemList" runat="server" Visible="false">
                                                <font width="10%">項目：</font>
                                                <div style="width: 10px; display: inline-block;">
                                                </div>
                                                <asp:DropDownList ID="ddlItemList" runat="server" AutoPostBack="True" DataTextField="ItemName" DataValueField="ItemID" Font-Size="12" OnSelectedIndexChanged="ddlItemList_SelectedIndexChanged" Width="260px">
                                                </asp:DropDownList>
                                            </asp:Panel>
                                            <asp:Panel ID="pnlMail" runat="server" Visible="false">
                                                <font width="10%">信箱：</font>
                                                <div style="width: 10px; display: inline-block;">
                                                </div>
                                                <asp:TextBox ID="txbApplyMail" runat="server" Width="255px" Font-Size="12"></asp:TextBox>
                                                <asp:Label ID="lblApplyMail" runat="server" Text="@hiss.com.tw"></asp:Label>
                                                <asp:Label ID="lblMailCheck" runat="server" ForeColor="Red" Text="此信箱已被申請！" Visible="False"></asp:Label>
                                            </asp:Panel>
                                            <asp:Panel runat="server">
                                                說明<font width="10%">：</font>
                                                <asp:RequiredFieldValidator ID="rfvItemAddReason" runat="server" ControlToValidate="txbItemAddReason" ErrorMessage="請輸入原因" ForeColor="Red" Text="*" ValidationGroup="MyValidationGroup" Width="10px" />
                                                <asp:TextBox ID="txbItemAddReason" runat="server" Font-Size="Medium" Placeholder="請描述原因或維修事宜等" Visible="true" Width="450px"></asp:TextBox>
                                                <asp:Button ID="btnItemAdd" runat="server" Font-Size="10" OnClick="btnItemAdd_Click" Text="加入" Width="150px" />
                                            </asp:Panel>
                            </asp:Panel>

                            <table width="100%">
                                <tr>
                                    <td>
                                        <asp:Panel ID="pnlHWInventory" runat="server" Visible="false">
                                            <asp:Label ID="lblHWInventory" runat="server" Text="庫存硬體:"></asp:Label>
                                            <asp:DropDownList ID="ddlHWInventory" runat="server" DataTextField="HWInventory" DataValueField="AssetsCode"></asp:DropDownList>
                                            <asp:Button ID="btnHWInventory" runat="server" Text="寫入" OnClick="btnHWInventory_Click" />
                                        </asp:Panel>
                                    </td>
                                    <td>
                                        <asp:Panel ID="pnlItemEdit" runat="server" Visible="false">
                                            <table>
                                                <tr>
                                                    <asp:Panel runat="server" ID="pnlEditName" Visible="true">
                                                        <td width="120px" style="text-align: right">名稱：
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txbEditItemName" runat="server" Visible="true"></asp:TextBox>
                                                        </td>

                                                    </asp:Panel>
                                                </tr>
                                                <tr>
                                                    <asp:Panel runat="server" ID="pnlEditAssetCode" Visible="true">
                                                        <td width="120px" style="text-align: right">資產編號：
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txbEditAssetCode" runat="server" Visible="true"></asp:TextBox>
                                                        </td>
                                                    </asp:Panel>
                                                </tr>
                                                <tr>
                                                    <td width="120px" style="text-align: right">處理進度說明：
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txbEditProcessing" runat="server"></asp:TextBox>
                                                        <div style="width: 20px; height: 10px; display: inline-block"></div>
                                                        <asp:Button ID="btnEdit" runat="server" Text="儲存" OnClick="btnEdit_Click" />
                                                    </td>

                                                </tr>
                                            </table>
                                        </asp:Panel>

                                    </td>
                                </tr>
                            </table>

                            <div class="gridViewContainer">
                                <asp:GridView ID="grvApplyContent" runat="server" AutoGenerateColumns="False" CssClass="center-gridview" OnRowDataBound="grvItemAdd_RowDataBound" Width="100%" CellPadding="4" ForeColor="#333333" GridLines="None" Font-Size="20px">
                                    <PagerStyle CssClass="pager" BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                                    <EditRowStyle BackColor="#7C6F57" />
                                    <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                    <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                    <AlternatingRowStyle BackColor="White" />
                                    <Columns>
                                        <asp:BoundField DataField="ServiceType" HeaderText="服務">
                                            <HeaderStyle Width="10%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="ItemType" HeaderText="種類">
                                            <HeaderStyle Width="10%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="ItemTypeName" HeaderText="項目">
                                            <HeaderStyle Width="12%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="ItemName" HeaderText="名稱">
                                            <HeaderStyle Width="30%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="AssetsCode" HeaderText="資產編號">
                                            <HeaderStyle Width="10%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Description" HeaderText="說明">
                                            <HeaderStyle Width="20%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="ItemProcessing" HeaderText="處理進度" Visible="false">
                                            <HeaderStyle Width="10%" />
                                        </asp:BoundField>
                                        <asp:TemplateField Visible="true">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lbtnDelete" runat="server" CommandArgument="<%# Container.DataItemIndex %>" CommandName="ItemDelete" OnCommand="lbtnDelete_Command" Text="刪除" Visible='<%# Eval("ShowDeleteButton") %>'></asp:LinkButton>
                                                <br />
                                                <asp:LinkButton ID="lnkSelect" runat="server" CommandName="Select" CssClass="lnkSelect" OnClick="lnkSelect_Click" Text="負責人編輯" Visible='<%# Eval("ShowEditButton") %>'></asp:LinkButton>
                                            </ItemTemplate>
                                            <HeaderStyle Width="13%" />
                                        </asp:TemplateField>
                                    </Columns>
                                    <RowStyle BackColor="#E3EAEB" />
                                    <SelectedRowStyle BackColor="#D0D0D0" ForeColor="#333333" />
                                    <SortedAscendingCellStyle BackColor="#F8FAFA" />
                                    <SortedAscendingHeaderStyle BackColor="#246B61" />
                                    <SortedDescendingCellStyle BackColor="#D4DFE1" />
                                    <SortedDescendingHeaderStyle BackColor="#15524A" />
                                </asp:GridView>

                                <br />

                            </div>
                            </td>
                        </tr>

                        </td>
                        </tr>
                    </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
        <asp:Panel runat="server" ID="pnlITManager" Visible="false" CssClass="font-size">
            <table width="100%">
                <tr>
                    <td class="auto-style5" width="15%">
                        <div width="100%" style="font-family: 微軟正黑體; font-weight: bold; text-align: left;">
                            ◆ 負責人：
                        </div>
                    </td>
                    <td width="85%">
                        <asp:Label ID="lblStarSeleckWorkEmp" runat="server" ForeColor="Red" Text="*"></asp:Label>
                        <asp:DropDownList ID="ddlSelectExecuteEmp" CssClass="styleCenter" runat="server" DataTextField="EmpName" DataValueField="EmpIDandRoleID" Width="200px" Enabled="false" Font-Size="20px"></asp:DropDownList>
                        <asp:Label ID="lblStarSeleckWorkEmp0" runat="server" ForeColor="Red" Text="*"></asp:Label>
                        <asp:Label ID="Label3" runat="server" Text="預估金額：$"></asp:Label>
                        <asp:TextBox ID="txbTotalCost" runat="server" Style="text-align: right" Width="60px" ControlToValidate="txtNumber" ValidationExpression="^\d+$" ErrorMessage="只能輸入數字" Enabled="false" oninput="validateIntegerInput(this)" Font-Size="20px"></asp:TextBox>
                        <script>
                            function validateIntegerInput(inputElement) {
                                // 使用正則表達式檢查輸入是否只包含數字
                                var validInput = /^\d*$/.test(inputElement.value);

                                if (!validInput) {
                                    // 如果輸入不是數字，清除輸入並取消事件的預設行為
                                    inputElement.value = "";
                                    preventDefault();
                                }
                            }
                        </script>

                        <br />
                        <asp:Label ID="lblError" runat="server" Font-Bold="true" Font-Size="20px" ForeColor="Red"></asp:Label>

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
                        <asp:TextBox ID="txbApplyReason" runat="server" TextMode="MultiLine" Width="830px" Height="50px" Font-Size="20px"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <table width="100%" class="font-size">
            <tr>
                <td>
                    <div class="title-style">
                        <asp:Label ID="lblFileUpload" runat="server" Text="◆ 附件上傳"></asp:Label>
                    </div>
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlFileUpload" runat="server" CssClass="font-size">

            <table width="100%">
                <tr>
                    <td width="25%">
                        <asp:FileUpload ID="FileUpload1" runat="server" />
                    </td>
                    <td width="75%">
                        <asp:Button ID="btnFileUpload" runat="server" Text="上傳" OnClick="btnFileUpload_Click" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblFileUploadErrMsg" runat="server" ForeColor="Red" Text="檔案大小不可超過20MB"></asp:Label>
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <asp:GridView ID="grvFileUpload" runat="server" CssClass="center-gridview" AutoGenerateColumns="False" Width="100%" CellPadding="4" ForeColor="#333333" GridLines="None" Font-Size="20px">
            <AlternatingRowStyle BackColor="White" />
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
            <EditRowStyle BackColor="#7C6F57" />
            <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#E3EAEB" />
            <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#F8FAFA" />
            <SortedAscendingHeaderStyle BackColor="#246B61" />
            <SortedDescendingCellStyle BackColor="#D4DFE1" />
            <SortedDescendingHeaderStyle BackColor="#15524A" />
        </asp:GridView>
        <br />
        <asp:Panel ID="pnlSend" runat="server" CssClass="font-size">
            <asp:Button ID="btnSend" runat="server" Text="送出" CssClass="title-style" OnClientClick="return confirm('確定要送出申請嗎？');" Visible="true" OnClick="btnSend_Click" />
            <div style="display: inline-block; width: 15px">
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlCheck" runat="server" CssClass="font-size">
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
                            <asp:Label ID="lblSignOpinion" runat="server" Text="◆ 簽核意見"></asp:Label>
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

            <asp:GridView ID="grvFormSignM" runat="server" Width="100%" AutoGenerateColumns="False" BackColor="White" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" ForeColor="Black" GridLines="Vertical" Font-Size="18px">
                <AlternatingRowStyle BackColor="#CCCCCC" />
                <Columns>
                    <asp:BoundField DataField="工號" HeaderText="工號" HeaderStyle-Width="5%" />
                    <asp:BoundField DataField="姓名" HeaderText="姓名" HeaderStyle-Width="6%" />
                    <asp:BoundField DataField="部門" HeaderText="部門" HeaderStyle-Width="10%" />
                    <asp:BoundField DataField="職稱" HeaderText="職稱" HeaderStyle-Width="8%" />
                    <asp:BoundField DataField="意見" HeaderText="意見" HeaderStyle-Width="44%" />
                    <asp:BoundField DataField="簽核日期" HeaderText="簽核日期" HeaderStyle-Width="11%" />
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
        <asp:Panel ID="pnlBtn" runat="server" CssClass="font-size">
            <div style="text-align: center;">
                <asp:Button ID="btnSubmit" runat="server" Text="簽核" CssClass="title-style" OnClientClick="return confirm('確定簽核？');" OnClick="btnSubmit_Click" />
                <asp:Button ID="btnReject" runat="server" Text="駁回" CssClass="title-style" OnClientClick="return confirm('確定駁回？');" OnClick="btnReject_Click" />
                <asp:Button ID="btnTake" runat="server" Text="取消申請" CssClass="title-style" OnClientClick="return confirm('確定取消申請？');" OnClick="btnTake_Click" Visible="false" />
                <br />
            </div>

        </asp:Panel>

        <asp:Panel ID="pnlVersion" runat="server">
            <div style="text-align: right">
                <asp:Label ID="lblVersion" runat="server" Text="QP-M-20-1 版次：5"></asp:Label>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
