<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QA01.aspx.cs" Inherits="BPM.FlowWork.QA01" %>

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
            var txbOccureDate = document.getElementById('<%= txbOccureDate.ClientID %>');
            var txbImplementDay = document.getElementById('<%= txbImplementDay.ClientID %>');
            // 初始化 Flatpickr
            flatpickr(txbOccureDate, {
                locale: "zh",
                // 在此設定 Flatpickr 的選項
                maxDate: new Date().fp_incr(0),
                defaultDate: "<%=strOccureDate%>"
            });

            flatpickr(txbImplementDay, {
                locale: "zh",
                // 在此設定 Flatpickr 的選項
                minDate: new Date().fp_incr(30),
                defaultDate: "<%=strImplementDay%>"
            });
        });

        //計算不良率
        function AutoBadRate() {
            var validInput = /^\d*$/.test(document.getElementById('<%= txbBadQty.ClientID %>').value);
            // 如果輸入不是數字，清除輸入並取消事件的預設行為
            if (!validInput) {
                document.getElementById('<%= txbBadQty.ClientID %>').value = "";
                preventDefault();
            }

            //不良樘數不能超過工單樘數
            var txbBadQtyValue = parseFloat(document.getElementById('<%= txbBadQty.ClientID %>').value);
            var lblShipQtyValue = parseFloat(document.getElementById('<%= lblShipQtyContent.ClientID %>').innerText);
            if (txbBadQtyValue > lblShipQtyValue) {
                document.getElementById('<%= txbBadQty.ClientID %>').value = lblShipQtyValue;
            }

            //計算
            var shipQty = parseFloat(document.getElementById('<%= lblShipQtyContent.ClientID %>').innerText);
            var badQty = parseFloat(document.getElementById('<%= txbBadQty.ClientID %>').value);
            var BadRate = (badQty / shipQty) * 100;
            document.getElementById('<%= lblBadRateContent.ClientID %>').innerText = BadRate.toFixed(2) + '%';
        }
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

        <asp:Panel ID="pnlPersonInformation" runat="server">
            <asp:Panel ID="pnlstartInfo" runat="server" Visible="false">
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
                </table>
            </asp:Panel>

            <table width="100%">
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
                    <td width="50%">
                        <asp:RadioButtonList ID="rbtnlSelectWorking" runat="server" Font-Names="微軟正黑體" RepeatDirection="Horizontal" AutoPostBack="True" OnSelectedIndexChanged="rbtnlSelectWorking_SelectedIndexChanged">
                            <asp:ListItem Value="complain" Text="客訴"></asp:ListItem>
                            <asp:ListItem Value="IQC" Text="進料抽檢"></asp:ListItem>
                            <asp:ListItem Value="IPQC" Text="生產製程巡檢"></asp:ListItem>
                            <asp:ListItem Value="OQC" Text="成品抽檢"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>

                    <td>
                        <div style="width: 20px; display: inline-block;"></div>
                        申請日期:
                            <asp:Label ID="lblApplyDate" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <asp:Panel ID="pnlApplyDepiction" runat="server">
            <div class="title-style">
                ◆ 現況現場現物敘述
            </div>
            <table width="100%">
                <tr>
                    <td>
                        <asp:Label ID="lblInputProductCode" runat="server" Text="成品料號,成品料名:"></asp:Label>
                        <asp:TextBox ID="txbInputProductCode" runat="server" Width="750px" placeholder="(請輸入成品料號或成品料名)"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server"
                            TargetControlID="txbInputProductCode"
                            ServiceMethod="QA01GetProductMatchingData"
                            MinimumPrefixLength="1"
                            CompletionInterval="100"
                            EnableCaching="false"
                            ServicePath="~/WebService1.asmx">
                        </ajaxToolkit:AutoCompleteExtender>
                        <asp:Button ID="btnEnter" runat="server" Text="確定" OnClick="btnEnter_Click" />
                        <asp:Button ID="btnClearEnter" runat="server" Text="清空" OnClick="btnClearEnter_Click" />
                        <asp:Label ID="lblErrorInputSAPNumber" runat="server" Text="錯誤的料號" Visible="false" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
            </table>
            <table width="100%">
                <tr>
                    <td width="50%">
                        <asp:Label ID="lblEventObject" runat="server" Text="事件對象:"></asp:Label>
                        <asp:Label ID="lblEventObjectContent" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblProductName" runat="server" Text="品名:" Visible="false"></asp:Label>
                        <asp:Label ID="lblProductNameContent" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
            <table width="100%">
                <tr>
                    <td width="25%">
                        <asp:Label ID="lblShipQty" runat="server" Text="工單樘數:"></asp:Label>
                        <asp:Label ID="lblShipQtyContent" runat="server"></asp:Label>
                    </td>
                    <td width="25%">
                        <asp:Label ID="lblBadQty" runat="server" Text="不良樘數:"></asp:Label>
                        <asp:TextBox ID="txbBadQty" runat="server" Width="90px" oninput="AutoBadRate()" placeholder="(請輸入數字)" TextMode="Number" Enabled="false"></asp:TextBox>
                        <%--<asp:Button ID="btnBadQty" runat="server" Text="計算不良率" OnClick="btnBadQty_Click" />--%>
                    </td>
                    <td width="50%">
                        <asp:Label ID="lblBadRate" runat="server" Text="不良率:"></asp:Label>
                        <asp:Label ID="lblBadRateContent" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
            <table width="100%">
                <tr>
                    <td width="50%">
                        <asp:Label ID="lblOccurDate" runat="server" Text="發生日期:"></asp:Label>
                        <asp:TextBox ID="txbOccureDate" runat="server" Enabled="false"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label ID="lblOccurPlace" runat="server" Text="發生地點:"></asp:Label>
                        <asp:TextBox ID="txbOccurPlace" runat="server" placeholder="(請輸入發生地點)" Enabled="false"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <asp:Panel ID="pnlComplaint" runat="server">
                <table width="100%">
                    <tr>
                        <td width="50%">
                            <asp:Label ID="lblProblemDescription" runat="server" Text="問題描述:"></asp:Label>
                            <asp:TextBox ID="txbProblemDescription" runat="server" Width="95%" Height="70px" TextMode="MultiLine" placeholder="(請輸入問題描述)" Enabled="false"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblMeasureDirection" runat="server" Text="已採措施說明:"></asp:Label>
                            <asp:TextBox ID="txbMeasureDirection" runat="server" Width="95%" Height="70px" TextMode="MultiLine" placeholder="(請輸入已採措施說明)" Enabled="false"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblComplaint" runat="server" Text="客訴內容是否明確為我司責任"></asp:Label>
                            <asp:RadioButtonList ID="rbtnComplaint" runat="server" RepeatDirection="Horizontal" AutoPostBack="True">
                                <asp:ListItem Value="1" Text="是"></asp:ListItem>
                                <asp:ListItem Value="0" Text="否"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="pnlIQC" runat="server">
            </asp:Panel>
            <asp:Panel ID="pnlIPQC" runat="server">
            </asp:Panel>
            <asp:Panel ID="pnlOQC" runat="server">
            </asp:Panel>
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
                    <td width="40%">
                        <asp:FileUpload ID="FileUpload1" runat="server" />
                    </td>
                    <td width="30%">
                        <asp:Button ID="btnFileUpload" runat="server" Text="上傳" OnClick="btnFileUpload_Click" />
                    </td>
                    <td width="30%">
                        <asp:Label ID="lblFileUploadErrMsg" runat="server" ForeColor="Red" Text="請上傳PDF檔案且大小不可超過5MB"></asp:Label>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:GridView ID="grvFileUpload" runat="server" AutoGenerateColumns="False" Width="100%" CellPadding="4" ForeColor="#333333" GridLines="None">
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
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

        <asp:Panel ID="pnlSend" runat="server">
            <asp:Button ID="btnSend" runat="server" Text="送出" CssClass="title-style" OnClientClick="return confirm('確定要送出申請嗎？');" Visible="true" OnClick="btnSend_Click" />
        </asp:Panel>

        <asp:Panel ID="pnlInvestigation" runat="server" Visible="false" Enabled="false">
            <asp:Label ID="lblInvestigation" runat="server" Text="◆原因調查:" CssClass="title-style"></asp:Label>
            <asp:TextBox ID="txbInvestigation" runat="server" Width="95%" Height="70px" TextMode="MultiLine" placeholder="(限品保調查員填入)"></asp:TextBox>
        </asp:Panel>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
            </ContentTemplate>
        </asp:UpdatePanel>

        <asp:Panel ID="pnlTarget" runat="server" Visible="false" Enabled="false">
            <asp:Label ID="lblRedStart" runat="server" Text="*" ForeColor="Red"></asp:Label>
            <asp:Label ID="lblTarget" runat="server" Text="是否為改善對象(非改善對象請填簽核意見後駁回):" ></asp:Label>
            <asp:CheckBox ID="chbTarget" runat="server" AutoPostBack="true" OnCheckedChanged="chbTarget_CheckedChanged" />
        </asp:Panel>

        <asp:Panel ID="pnlAnalyze" runat="server" Visible="false" Enabled="false">
            <asp:Label ID="lblAnalyze" runat="server" Text="◆真因分析:" CssClass="title-style"></asp:Label>
            <asp:TextBox ID="txbAnalyze" runat="server" Width="95%" Height="70px" TextMode="MultiLine" placeholder="(限改善對象勾選後填入)"></asp:TextBox>
        </asp:Panel>

        <asp:Panel ID="pnlCountermeasures" runat="server" Visible="false" Enabled="false">
            <asp:Label ID="lblCountermeasures" runat="server" Text="◆對策擬定:" CssClass="title-style"></asp:Label><br />
            <asp:Label ID="lblImplementDay" runat="server" Text="選擇實施日:"></asp:Label>
            <asp:TextBox ID="txbImplementDay" runat="server"></asp:TextBox>
            <asp:TextBox ID="txbCountermeasures" runat="server" Width="95%" Height="70px" TextMode="MultiLine" placeholder="(限改善對象勾選後填入)"></asp:TextBox>
        </asp:Panel>

        <asp:Panel ID="pnlQAConfirm" runat="server" Visible="false" Enabled="false">
            <asp:Label ID="lblQAConfirm" runat="server" Text="◆品保確認:" CssClass="title-style"></asp:Label><br />
            <asp:Label ID="lblClassification" runat="server" Text="分類:"></asp:Label>
            <asp:DropDownList ID="ddlLargeClassification" runat="server"></asp:DropDownList>
            <asp:DropDownList ID="ddlMediumClassification" runat="server"></asp:DropDownList>
            <asp:DropDownList ID="ddlSmallClassification" runat="server"></asp:DropDownList>
            <br />
            <asp:Label ID="lblDirections" runat="server" Text="說明:"></asp:Label>
            <asp:TextBox ID="txbDirections" runat="server"></asp:TextBox>
            <asp:Button ID="btnAdd" runat="server" Text="加入" />
            <asp:GridView ID="grdQAConfirm" runat="server"></asp:GridView>
        </asp:Panel>

        <asp:Panel ID="pnlEffectConfirm" runat="server" Visible="false" Enabled="false">
            <asp:Label ID="lblEffectConfirm" runat="server" Text="◆效果確認:" CssClass="title-style"></asp:Label><br />
            <asp:TextBox ID="txbEffectConfirm" runat="server" Width="95%" Height="70px" TextMode="MultiLine" placeholder="(限品保調查者填入)"></asp:TextBox>
        </asp:Panel>

        <asp:Panel ID="pnlQAManager" runat="server" Visible="false" Enabled="false">
            <asp:Label ID="lblQAManager" runat="server" Text="◆品保主管審核:" CssClass="title-style"></asp:Label><br />
            <asp:TextBox ID="txbQAManager" runat="server" Width="95%" Height="70px" TextMode="MultiLine" placeholder="(限品保課長填入)"></asp:TextBox>
        </asp:Panel>

        <asp:Panel ID="pnlSelectInvestigator" runat="server" Visible="false">
            <asp:Label ID="lblSelectInvestigator" runat="server" Text="選擇調查者:"></asp:Label>
            <asp:DropDownList ID="ddlSelectInvestigator" runat="server" DataTextField="DeptNameEmpName" DataValueField="RoleIDEmpID"></asp:DropDownList>
        </asp:Panel>

        <asp:Panel ID="pnlSelectManager" runat="server" Visible="false">
            <asp:Label ID="lblSelectManager" runat="server" Text="選擇改善對象:"></asp:Label>
            <asp:DropDownList ID="ddlSelectManager" runat="server" DataTextField="DeptNameEmpName" DataValueField="RoleIDEmpID"></asp:DropDownList>
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
                        <asp:TextBox ID="txbSignOpinion" runat="server" TextMode="MultiLine" Width="830px" Height="50px" Enabled="false"></asp:TextBox>
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

            <asp:GridView ID="grvFormSignM" runat="server" Width="100%" AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" GridLines="None">
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:BoundField DataField="工號" HeaderText="工號" />
                    <asp:BoundField DataField="姓名" HeaderText="姓名" />
                    <asp:BoundField DataField="部門" HeaderText="部門" />
                    <asp:BoundField DataField="職稱" HeaderText="職稱" />
                    <asp:BoundField DataField="意見" HeaderText="意見" />
                    <asp:BoundField DataField="簽核日期" HeaderText="簽核日期" />
                </Columns>
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
        <asp:Panel ID="pnlBtn" runat="server" Visible="false">
            <div style="text-align: center;">
                <asp:Button ID="btnSubmit" runat="server" Text="簽核" CssClass="title-style" OnClientClick="return confirm('確定簽核？');" OnClick="btnSubmit_Click" />
                <asp:Button ID="btnReject" runat="server" Text="駁回" CssClass="title-style" OnClientClick="return confirm('確定駁回？');" OnClick="btnReject_Click" />
                <asp:Button ID="btnTake" runat="server" Text="取消申請" CssClass="title-style" OnClientClick="return confirm('確定取消申請？');" OnClick="btnTake_Click" />
                <asp:Button ID="btnInvalid" runat="server" Text="客訴不成立" CssClass="title-style" OnClientClick="return confirm('確定不成立？');" OnClick="btnInvalid_Click" />
            </div>

        </asp:Panel>
        <asp:Panel ID="pnlVersion" runat="server">
            <div style="text-align: right">
                <asp:Label ID="lblVersion" runat="server" Text="QP-Q-10-1 版次：6"></asp:Label>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
