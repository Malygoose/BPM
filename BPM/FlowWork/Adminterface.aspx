﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Adminterface.aspx.cs" Inherits="BPM.FlowWork.Adminterface" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <style>
        .font-Size {
            font-size: 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager" runat="server"></asp:ScriptManager>
        <table width="100%">
            <tr>
                <td width="85%"></td>

                <td>
                    <asp:Button ID="btnHome" runat="server" Text="首頁" OnClick="btnHome_Click" />
                </td>
                <td>
                    <asp:LoginName ID="LoginID" runat="server" Font-Size="Large" />
                </td>
                <td>
                    <asp:Label ID="lblLoginName" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:LoginStatus ID="LoginStatus1" runat="server" />
                </td>
            </tr>
        </table>

        <asp:Panel ID="pnlSearch" runat="server">

            
            <div class="font-Size">
                
                <asp:Label ID="lblSearchItemType" runat="server" Text="種類:"></asp:Label>
                <asp:DropDownList ID="ddlSearchItemType" runat="server" AutoPostBack="True" DataTextField="TypeName" DataValueField="TypeID" OnSelectedIndexChanged="ddlSearchItemType_SelectedIndexChanged" CssClass="font-Size"></asp:DropDownList>
                <asp:Label ID="lblSearchItemName" runat="server" Text="項目:" Visible="True"></asp:Label>
                <asp:DropDownList ID="ddlSearchItemName" runat="server" AutoPostBack="True" DataTextField="ItemName" DataValueField="ItemID" CssClass="font-Size" Visible="True"></asp:DropDownList>
                <asp:Label ID="lblSearch" runat="server" Text="輸入關鍵字:"></asp:Label>
                <asp:TextBox ID="txbSearch" runat="server" Width="110px" CssClass="font-Size"></asp:TextBox>
                <asp:Button ID="btnSearch" runat="server" Text="搜尋" OnClick="btnSearch_Click" CssClass="font-Size"/>
                <asp:Button ID="btnDownload" runat="server" Text="下載(Excel)" OnClick="btnDownload_Click" Visible="false" CssClass="font-Size"/>
            </div>

        </asp:Panel>
        <br />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Panel ID="pnlEdit" runat="server">
                    <table width="100%">
                        <tr>
                            <td>
                                <asp:Label ID="lblAdd" runat="server" Text="新增/更改資料" Font-Size="20"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td width="25%">工號,姓名,部門:
                                <asp:TextBox ID="txbEmpNumName" runat="server" AutoPostBack="true" Width="230px"></asp:TextBox>
                                <ajaxToolkit:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server"
                                    TargetControlID="txbEmpNumName"
                                    ServiceMethod="GetMatchingData"
                                    MinimumPrefixLength="1"
                                    CompletionInterval="100"
                                    EnableCaching="false"
                                    ServicePath="~/WebService1.asmx">
                                </ajaxToolkit:AutoCompleteExtender>
                            </td>
                            <%--<td>工號:<asp:TextBox ID="txbEditNobr" runat="server"></asp:TextBox></td>
                            <td>姓名:<asp:TextBox ID="txbEditUserName" runat="server"></asp:TextBox></td>--%>
                            <%--<td>部門:<asp:TextBox ID="txbEditDeptName" runat="server"></asp:TextBox></td>--%>
                            <%--<td>種類:<asp:TextBox ID="txbEditItemType" runat="server"></asp:TextBox></td>--%>
                            <%--<td>項目:<asp:TextBox ID="txbEditItemName" runat="server"></asp:TextBox></td>--%>
                            <td width="9%">種類:<asp:DropDownList ID="ddlItemType" runat="server" AutoPostBack="True" DataTextField="TypeName" DataValueField="TypeID" Font-Size="12" OnSelectedIndexChanged="ddlItemType_SelectedIndexChanged"></asp:DropDownList>
                            </td>
                            <td width="10%">項目:<asp:DropDownList ID="ddlItemList" runat="server" AutoPostBack="True" DataTextField="ItemName" DataValueField="ItemID" Font-Size="12"></asp:DropDownList>
                            </td>
                            <td width="23%">
                                <asp:Label ID="lblAssetsName" runat="server" Text="資產名稱:" Visible="false"></asp:Label>
                                <asp:TextBox ID="txbEditAssetsName" runat="server" Width="180px" Visible="false"></asp:TextBox>
                                <asp:Label ID="lblEmailFormate" runat="server" Text="@hiss.com.tw" Visible="false"></asp:Label>
                            </td>
                            <td width="15%">
                                <asp:Label ID="lblAssetsCode" runat="server" Text="資產編號:" Visible="false"></asp:Label>
                                <asp:TextBox ID="txbEditAssetsCode" runat="server" Width="80px" Visible="false"></asp:TextBox>
                            </td>
                            <td width="10%">
                                <asp:Label ID="lblMessage" runat="server" Visible="false" ForeColor="Red"></asp:Label>
                            </td>
                            <td width="15%">
                                <asp:Button ID="btnAdd" runat="server" Text="新增" OnClick="btnAdd_Click" />
                                <asp:Button ID="btnEdit" runat="server" Text="更改" OnClick="btnEdit_Click" OnClientClick="return confirm('確定要更改嗎？');" Visible="false" />
                                <asp:Button ID="btnDelete" runat="server" Text="刪除" OnClick="btnDelete_Click" Visible="false" OnClientClick="return confirm('確定要刪除嗎？');" />
                                <asp:Button ID="btnClear" runat="server" Text="清空" OnClick="btnClear_Click" />
                            </td>
                        </tr>
                    </table>
                    <asp:Label ID="lblGuidKey" runat="server" Visible="false"></asp:Label>
                </asp:Panel>
                <br />

                <asp:Panel ID="pnlAdminterface" runat="server">

                    <%--<asp:Button ID="btnShowAdminterface" runat="server" Text="顯示現有項目表" OnClick="btnShowAdminterface_Click" Font-Size="15" />--%>

                    <asp:GridView ID="grvAdminterface" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" GridLines="None" Width="100%"
                        OnSelectedIndexChanged="grvAdminterface_SelectedIndexChanged" AllowPaging="true" PageSize="20" OnPageIndexChanging="grvAdminterface_PageIndexChanging"
                        DataKeyNames="GuidKey">
                        <Columns>
                            <asp:BoundField DataField="Nobr" HeaderText="工號" />
                            <asp:BoundField DataField="UserName" HeaderText="姓名" />
                            <asp:BoundField DataField="DeptName" HeaderText="部門" />
                            <asp:BoundField DataField="ItemType" HeaderText="種類" />
                            <asp:BoundField DataField="ItemName" HeaderText="項目" />
                            <asp:BoundField DataField="AssetsName" HeaderText="資產名稱" />
                            <asp:BoundField DataField="AssetsCode" HeaderText="資產編號" />



                            <asp:CommandField ShowSelectButton="true"></asp:CommandField>


                        </Columns>
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                        <EditRowStyle BackColor="#999999" />
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                        <SortedAscendingCellStyle BackColor="#E9E7E2" />
                        <SortedAscendingHeaderStyle BackColor="#506C8C" />
                        <SortedDescendingCellStyle BackColor="#FFFDF8" />
                        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />

                    </asp:GridView>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>

    </form>
</body>
</html>
