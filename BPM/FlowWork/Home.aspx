<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="BPM.FlowWork.Home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
    <script src="https://npmcdn.com/flatpickr/dist/l10n/zh.js"></script>
    <script>
        // 使用 window.addEventListener 等待頁面載入完成後執行程式碼
        window.addEventListener('DOMContentLoaded', function () {
            // 取得要綁定 Flatpickr 的元素，這裡假設是一個 ASP.NET 的 TextBox 控制項
            var txbDateA = document.getElementById('<%= txbDateA.ClientID %>');
            var txbDateB = document.getElementById('<%= txbDateB.ClientID %>');

            // 初始化 Flatpickr
            flatpickr(txbDateA, {
                locale: "zh",
                // 在此設定 Flatpickr 的選項
                defaultDate: "<%=strDateBegin%>",
                maxDate: new Date().fp_incr(0)
            });
            flatpickr(txbDateB, {
                locale: "zh",
                // 在此設定 Flatpickr 的選項
                defaultDate: "<%=strDateEnd%>",
            });
        });
    </script>
    <style type="text/css">
        body {
            background-color: #F0F0F0;
        }

        .fontStyle {
            text-align: center;
            font-family: 微軟正黑體;
        }

        .btnQty {
            padding: 2px;
            width: 200px;
            height: 50px;
            font-size: 20px;
            font-weight: bold;
            text-align: center;
            background-color: #F0F0F0;
            color: #000000;
            border: none;
            cursor: pointer;
            border-style: solid;
            border-width: 2px;
            border-color: #ADADAD;
            margin-right: 5px;
            margin-left: 5px;
        }

            .btnQty:hover {
                background-color: #E0E0E0;
            }

        .styleListTitle {
            background-color: #6C6C6C;
            height: 26px;
            color: aliceblue;
            padding: 3px;
            width: 999px;
        }

        .styleCenter {
            text-align: center;
        }


        .style-left {
            text-align: left;
        }

        .center-panel {
            padding: 10px;
            margin: 0 auto;
            width: 800px;
            box-shadow: 0 0 20px #666;
            background: #fff;
        }

        .pager {
            text-align: center;
            margin-top: 10px;
        }

            .pager a,
            .pager span {
                display: inline-block;
                padding: 5px 10px;
                margin: 0 2px;
                background-color: #e9e9e9;
                color: #333;
                text-decoration: none;
                border-radius: 3px;
            }

                .pager a:hover {
                    background-color: #ccc;
                }

            .pager .pager-current {
                font-weight: bold;
                background-color: #333;
                color: #fff;
            }

        .auto-style3 {
            height: 36px;
        }

        .auto-style4 {
            width: 15%;
            height: 59px;
        }

        .auto-style5 {
            height: 59px;
        }

        .no-underline {
            text-decoration: none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel CssClass="center-panel" runat="server" Width="70%">
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:HyperLink ID="HLAdminterface" runat="server" Font-Size="25px" NavigateUrl="~/FlowWork/Adminterface.aspx" CssClass="no-underline">管理者介面</asp:HyperLink>
                        <asp:Button ID="btnApplyForm" runat="server" Text="資訊服務申請單" CssClass="btnQty" OnClick="btnApplyForm_Click" Visible="false"/>
                        <asp:Button ID="btnTemplate" runat="server" Text="OO申請單" CssClass="btnQty" OnClick="btnTemplate_Click" Visible="false"/>
                        
                        <asp:TextBox ID="txbTestDelete" runat="server" Visible="True"></asp:TextBox>
                        <asp:Button ID="btnTestDelete" runat="server" OnClick="btnTestDelete_Click" Text="刪除" Visible="True" />
                    </td>
                    <td style="text-align: right">
                        <asp:LoginName ID="LoginID" runat="server" Font-Size="Large" />
                        <asp:Label ID="lblLoginName" runat="server" ></asp:Label>
                        <asp:Button ID="btnFormSign" runat="server" CssClass="btnQty" Font-Size="12pt" Height="30px" OnClick="btnFormSign_Click" Text="待簽核" Width="126px" />
                        <asp:Label ID="lblUnSign" runat="server" Font-Bold="True" Font-Size="16pt" ForeColor="Red" Text="Label"></asp:Label>

                        <asp:LoginStatus ID="LoginStatus1" runat="server" />

                    </td>
                </tr>
            </table>
            
            <table width="100%">
                <tr>
                    <th class="styleListTitle" font-bold="True" font-names="微軟正黑體" font-size="Medium">表單申請</th>
                </tr>
            </table>

            <ul>
                <li><asp:HyperLink ID="HLIT01ApplyForm" runat="server" Font-Size="25px" NavigateUrl="~/FlowWork/IT01?IT01=?" CssClass="no-underline">資訊服務申請單</asp:HyperLink></li>
                <li><asp:HyperLink ID="HLQA01" runat="server" Font-Size="25px" NavigateUrl="~/FlowWork/QA01?QA01=?" CssClass="no-underline" >矯正預防單</asp:HyperLink></li>
                <%--<li><asp:HyperLink ID="HLTemplate" runat="server" Font-Size="25px" NavigateUrl="~/FlowWork/Template?Template=?" CssClass="no-underline" >(測試)共通表單</asp:HyperLink></li>--%>
            </ul><br />
            

            <table width="100%">
                <tr>
                    <th class="styleListTitle" font-bold="True" font-names="微軟正黑體" font-size="Medium">表單列表</th>
                </tr>
            </table>

            <table width="100%">
                <tr>
                    <td class="auto-style3">
                        <p style="font-family: 微軟正黑體; font-size: medium; font-weight: bold;">
                            ◆ 申請日期：
                        </p>
                    </td>
                    <td class="auto-style3">
                        <asp:TextBox ID="txbDateA" runat="server" class="fontStyle"></asp:TextBox>
                        <font>到</font>
                        <asp:TextBox ID="txbDateB" runat="server" class="fontStyle"></asp:TextBox>
                    </td>
                </tr>

                <tr>
                    <td style="width: 15%">
                        <p style="font-family: 微軟正黑體; font-size: medium; font-weight: bold;">
                            ◆ 表單狀態：
                        </p>
                    </td>
                    <td>
                        <asp:DropDownList Style="width: 150px; height: 24px; text-align: center" ID="ddlStateType" Font-Size="Medium" runat="server" >
                            <asp:ListItem Value="1">進行中</asp:ListItem>
                            <asp:ListItem Value="2">已駁回</asp:ListItem>
                            <asp:ListItem Value="3">已完成</asp:ListItem>
                            <asp:ListItem Value="7">已取消</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style4">
                        <asp:Button ID="btnFormList" runat="server" Text="申請紀錄查詢" CssClass="btnQty" OnClick="btnFormList_Click" Font-Size="12pt" Height="30px" Width="126px" />
                    </td>
                    <td class="auto-style5">
                        <asp:Button ID="btnFormSignList" runat="server" CssClass="btnQty" Font-Size="12pt" Height="30px" OnClick="btnFormSignList_Click" Text="已簽核紀錄查詢" Width="126px" />
                    </td>
                </tr>
            </table>

            <asp:Panel ID="pnlApply" runat="server" Visible="false">
                <div class="gridview-container">
                    <asp:GridView ID="grvFormList" runat="server" Width="100%" AllowPaging="True" OnPageIndexChanging="grvFormList_PageIndexChanging" PagerSettings-Mode="NumericFirstLast" PagerSettings-FirstPageText="第一頁" PagerSettings-LastPageText="最後一頁" AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" GridLines="None">
                        <PagerSettings FirstPageText="第一頁" LastPageText="最後一頁" />
                        <PagerStyle CssClass="pager" BackColor="white" ForeColor="black" HorizontalAlign="Center" />
                        <EditRowStyle BackColor="#999999" />
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#9D9D9D" Font-Bold="True" ForeColor="White" />
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                        <Columns>
                            <asp:TemplateField ItemStyle-Width="60px">
                                <ItemTemplate>
                                    <asp:Button ID="btnDelete" runat="server" CommandArgument="<%# Container.DataItemIndex %>" CommandName="ProcessDelete" OnCommand="btnDelete_Command" Text="取消申請" Visible='<%# Eval("ShowBtnTake") %>'/>
                                </ItemTemplate>
                                <ItemStyle Width="60px" />
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="40px">
                                <ItemTemplate>
                                    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%#Eval("表單代號")+"?ApView="+ Eval("ApKey") %>' Target="_self" Text="檢視"> </asp:HyperLink>
                                </ItemTemplate>
                                <ItemStyle Width="40px" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="表單序號" HeaderText="表單序號" ItemStyle-Width="80px">
                            <ItemStyle Width="80px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="表單名稱" HeaderText="表單名稱" ItemStyle-Width="100px">
                            <ItemStyle Width="100px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="發起人" HeaderText="發起人" ItemStyle-Width="80px">
                            <ItemStyle Width="80px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="申請人" HeaderText="申請人" ItemStyle-Width="80px">
                            <ItemStyle Width="80px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="日期" DataFormatString="{0:d}" HeaderText="申請日期" ItemStyle-Width="100px">
                            <ItemStyle Width="100px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="資訊" HeaderText="資訊" ItemStyle-Width="600px">
                            <ItemStyle Width="600px" />
                            </asp:BoundField>
                        </Columns>
                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                        <SortedAscendingCellStyle BackColor="#E9E7E2" />
                        <SortedAscendingHeaderStyle BackColor="#506C8C" />
                        <SortedDescendingCellStyle BackColor="#FFFDF8" />
                        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                    </asp:GridView>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlSign" runat="server" Visible="false">
                <asp:GridView ID="grvFormSign" runat="server" Width="100%" AutoGenerateColumns="False"  AllowPaging="True" OnPageIndexChanging="grvFormSign_PageIndexChanging" PagerSettings-Mode="NumericFirstLast" PagerSettings-FirstPageText="第一頁" PagerSettings-LastPageText="最後一頁" CellPadding="4" ForeColor="#333333" GridLines="None">
                    <PagerSettings FirstPageText="第一頁" LastPageText="最後一頁" />
                    <PagerStyle CssClass="pager" BackColor="white" ForeColor="black" HorizontalAlign="Center" />
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    <Columns>
                        <asp:TemplateField ItemStyle-Width="40px">
                            <ItemTemplate>
                                <asp:HyperLink ID="HyperLink2" runat="server" Target="_self" Text='檢視' NavigateUrl='<%#Eval("表單代號")+"?ApParm="+ Eval("ApKey") %>'> </asp:HyperLink>
                            </ItemTemplate>
                            <ItemStyle Width="40px" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="表單序號" HeaderText="表單序號">
                            <ItemStyle Width="80px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="表單名稱" HeaderText="表單名稱">
                            <ItemStyle Width="100px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="發起人" HeaderText="發起人">
                            <ItemStyle Width="80px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="申請人" HeaderText="申請人">
                            <ItemStyle Width="80px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="申請日期" HeaderText="申請日期" DataFormatString="{0:d}">
                            <ItemStyle Width="100px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="資訊" HeaderText="資訊">
                            <ItemStyle Width="600px" />
                        </asp:BoundField>
                    </Columns>
                    <EditRowStyle BackColor="#999999" />
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#9D9D9D" Font-Bold="True" ForeColor="White" />
                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                    <SortedAscendingCellStyle BackColor="#E9E7E2" />
                    <SortedAscendingHeaderStyle BackColor="#506C8C" />
                    <SortedDescendingCellStyle BackColor="#FFFDF8" />
                    <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                </asp:GridView>
                <br />
            </asp:Panel>
            <br />
        </asp:Panel>
    </form>
</body>
</html>
