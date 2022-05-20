<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010801140001.aspx.cs" Inherits="Page_P010801140001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <%-- 2020/11/23_Ares_Stanley-修正格式; 2021/01/21_Ares_Stanley-修正格式; 2021/04/01_Ares_Stanley-調整超過字元數導致無法搜尋問題 --%>
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript"> 
        // 面頁邏輯檢核
        function checkInputText(id, intType) {

            mustKeyIn(intType);
        }

        function mustKeyIn() {
            var isSuccess = true;
        }
    </script> 
   
</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="300">
        </asp:ScriptManager>
        <cust:image runat="server" ID="image1" />
        &nbsp;
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                    <tr class="itemTitle">
                        <td>
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01080114_001"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <%--統一編號--%>
                    <tr>
                        <td style="width: 100%" colspan="1">
                            <table border="0" width="100%" cellspacing="1"> 
                                <tr class="trOdd">
                                    <td align="right" width="8%"><%--統一編號--%>
                                        <cc1:CustLabel ID="lblCardNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01080100_002" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="14%">
                                        <cc1:CustTextBox ID="txtTaxID" runat="server" MaxLength="10" checktype="num" Width="80px" onfocus="allselect(this);"
                                             BoxName="統一編號一"></cc1:CustTextBox>
                                        <%--  <cc1:CustTextBox ID="txtCardNo2" runat="server" MaxLength="4" checktype="num" Width="40px"
                                            onkeydown="entersubmit('btnSelect');" onkeyup="changeStatus();" BoxName="統一編號二"></cc1:CustTextBox>--%>
                                    </td>
                                    <td align="right" width="8%">
                                        <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01080100_003" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="14%">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtProjYearBegin" runat="server" MaxLength="4" Width="55px" onfocus="allselect(this);"
                                                 BoxName="開始年"
                                              Style="left: 0px; top: 0px; position: relative; width: 55px; height: 13px; line-height:13px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropProjYearBegin" kind="select" runat="server" onclick="simOptionClick4IE('txtProjYearBegin');"
                                            Style="left: 5px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 59px;">
                                            </cc1:CustDropDownList><%--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
                                            <asp:Label ID="Label2" runat="server" Text="年"></asp:Label>
                                        </div>
                                    </td>
                                    <td width="21%" colspan="2">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtProjMonthBegin" runat="server" MaxLength="4" Width="55px" onfocus="allselect(this);"
                                                 BoxName="開始月"
                                                Style="left: 0px; top: 0px; position: relative; width: 55px; height: 13px; line-height:13px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropProjMonthBegin" kind="select" runat="server" onclick="simOptionClick4IE('txtProjMonthBegin');"
                                                Style="left: 5px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 59px;">
                                            </cc1:CustDropDownList><%--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
                                            <asp:Label ID="Label3" runat="server" Text="月  ~"></asp:Label>
                                        </div>
                                    </td>
                                    <td width="21%" colspan="2">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtProjYearEnd" runat="server" MaxLength="4" Width="55px" onfocus="allselect(this);"
                                                 BoxName="結束年"
                                                Style="left: 0px; top: 0px; position: relative; width: 55px; height: 13px; line-height:13px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropProjYearEnd" kind="select" runat="server" onclick="simOptionClick4IE('txtProjYearEnd');"
                                                Style="left: 5px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 59px;">
                                            </cc1:CustDropDownList><%--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
                                            <asp:Label ID="Label4" runat="server" Text="年"></asp:Label>
                                        </div>
                                    </td>
                                    <td width="27%" colspan="2">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtProjMonthEnd" runat="server" MaxLength="4" Width="55px" onfocus="allselect(this);"
                                                 BoxName="結束月"
                                                Style="left: 0px; top: 0px; position: relative; width: 55px; height: 11px; line-height:13px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropProjMonthEnd" kind="select" runat="server" onclick="simOptionClick4IE('txtProjMonthEnd');"
                                                Style="left: 5px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 59px;">
                                            </cc1:CustDropDownList><%--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
                                            <asp:Label ID="Label5" runat="server" Text="月"></asp:Label>
                                        </div>
                                    </td>
                                </tr>
                                <tr class="trOdd">
                                    <td align="right" width="8%">
                                        <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01080100_005" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="14%">
                                        <cc1:CustTextBox ID="txtCASE_NO" runat="server" MaxLength="14" checktype="num" Width="120px" onfocus="allselect(this);"
                                            onkeydown="entersubmit('btnSelect');"  BoxName="案件編號"></cc1:CustTextBox>

                                    </td>
                                    <td align="right" width="8%">
                                        <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01080100_006" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="14%">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtCaseOwner_User" runat="server" Width="90px" onfocus="allselect(this);"
                                                 BoxName="處理經辦"
                                                Style="left: 0px; top: 0px; position: absolute; width: 90px; height: 11px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropCaseOwner_User" kind="select" runat="server" onclick="simOptionClick4IE('txtCaseOwner_User');"
                                                Style="left: 0px; top: 0px; clip: rect(0px auto auto 0px); position: relative; width: 100px;">
                                            </cc1:CustDropDownList>
                                        </div>
                                    </td>                                    
                                    <td align="right" width="8%"><%--風險等級--%>
                                        <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01080100_004" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="13%">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtOriginalRiskRanking" runat="server" MaxLength="3" Width="55px" onfocus="allselect(this);"
                                                 BoxName="風險等級"
                                                Style="left: 0px; top: 0px; position: relative; width: 55px; height: 13px; line-height:13px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropOriginalRiskRanking" kind="select" runat="server" onclick="simOptionClick4IE('txtOriginalRiskRanking');"
                                                Style="left: 5px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 59px;">
                                            </cc1:CustDropDownList>
                                        </div>
                                    </td>
                                    <%--20191025--%>
                                    <td align="right" width="8%"><%--排序欄位--%>
                                        <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01080100_007" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="13%">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtSort" runat="server" MaxLength="4" Width="80px" onfocus="allselect(this);"
                                                 BoxName="排序欄位"
                                                Style="left: 0px; top: 0px; position: absolute; width: 80px; height: 11px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropSort" kind="select" runat="server" onclick="simOptionClick4IE('txtSort');"
                                                Style="left: 0px; top: 0px; clip: rect(0px auto auto 0px); position: relative; width: 90px;">
                                            </cc1:CustDropDownList>
                                        </div>
                                    </td>                                    
                                    <td  width="14%">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtAsc" runat="server" MaxLength="3" Width="60px" onfocus="allselect(this);"
                                                 BoxName="排序欄位"
                                                Style="left: 0px; top: 0px; position: absolute; width: 60px; height: 11px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropAsc" kind="select" runat="server" onclick="simOptionClick4IE('txtAsc');"
                                                Style="left: 0px; top: 0px; clip: rect(0px auto auto 0px); position: relative; width: 70px;">
                                            </cc1:CustDropDownList>
                                        </div>
                                    </td>
                                </tr>
                                <%--20191202--%>
                                <tr class="trOdd">
                                    <td align="right" width="8%">
                                        <cc1:CustLabel ID="CustLabel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01080114_A85" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="14%">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtLastUpdateDateBegin" runat="server" MaxLength="8" checktype="num" Width="120px" onfocus="allselect(this);"
                                                onkeydown="entersubmit('btnSelect');"  BoxName="結案日期(起)"></cc1:CustTextBox>
                                        </div>
                                    </td>
                                    <td align="center" width="8%">
                                        <asp:Label ID="Label1" runat="server" Text="~"></asp:Label>
                                    </td>
                                    <td width="14%">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtLastUpdateDateEnd" runat="server" MaxLength="8" checktype="num" Width="120px" onfocus="allselect(this);"
                                                onkeydown="entersubmit('btnSelect');"  BoxName="結案日期(迄)"></cc1:CustTextBox>
                                            </div>
                                    </td>
                                    <td align="right" width="8%">
                                        <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01080114_A82" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="13%"><%--20191025 修改：增加不合作註記的查詢條件 by Peggy--%>
                                        <div style="position: relative">
                                            <div style="position: relative">
                                            <cc1:CustTextBox ID="txtIncorporated" runat="server" MaxLength="3" Width="60px" onfocus="allselect(this);"
                                                 BoxName="不合作註記"
                                                Style="left: 0px; top: 0px; position: absolute; width: 60px; height: 11px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropIncorporated" kind="select" runat="server" onclick="simOptionClick4IE('txtIncorporated');"
                                                Style="left: 0px; top: 0px; clip: rect(0px auto auto 0px); position: relative; width: 70px;">
                                                <asp:ListItem Value="X">請選擇</asp:ListItem>
                                                 <asp:ListItem Value="Y">Y</asp:ListItem>
                                                 <asp:ListItem Value="N">N</asp:ListItem>
                                            </cc1:CustDropDownList>
                                        </div>
                                        </div>
                                    </td>
                                    <td align="right" width="8%"><%--結案原因--%>
                                        <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01080114_A84" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="13%">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtCloseType" runat="server" MaxLength="4" Width="70px" onfocus="allselect(this);"
                                                 BoxName="結案原因"
                                                Style="left: 0px; top: 0px; position: absolute; width: 70px; height: 11px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropCloseType" kind="select" runat="server" onclick="simOptionClick4IE('txtCloseType');"
                                                Style="left: 0px; top: 0px; clip: rect(0px auto auto 0px); position: relative; width: 80px;">
                                            </cc1:CustDropDownList>
                                        </div>
                                    </td>
                                    <td width="14%" >
                                        <cc1:CustButton ID="btnSelect" runat="server" CssClass="smallButton" ShowID="01_01040101_027"
                                            OnClick="btnSelect_Click" OnClientClick="return checkInputText('pnlText', 0);"
                                            DisabledWhenSubmit="False" onkeydown="setfocusmove();" BoxName="查詢" />
                                        <cc1:CustButton ID="btnDownload" runat="server" CssClass="smallButton" ShowID="01_01080114_A83"
                                            OnClick="btnDownload_Click" OnClientClick="return checkInputText('pnlText', 0);"
                                            DisabledWhenSubmit="False" onkeydown="setfocusmove();" BoxName="下載" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr> 
                </table>
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo4" style="">
                        <%--新統一編號--%>
                        <tr class="trEven">

                            <td style="width: 99%">
                                <cc1:CustGridView ID="grvCardData" runat="server" BorderStyle="Solid" EnableModelValidation="True" PagerID="GridPager1" Width="100%" OnRowDataBound="grvCardData_RowDataBound" OnRowCommand="grvCardData_RowCommand">
                                    <AlternatingRowStyle CssClass="Grid_AlternatingItem" />
                                    <Columns>
                                          <asp:BoundField DataField="Row#" HeaderText="序號" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="60px" />
                                        <asp:TemplateField HeaderText="案件編號" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="120px">
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" ID="lnkView" CommandArgument='<%#Eval("ArgNo") %>'
                                                    CommandName="VIEW"><%#Eval("CASE_NO") %></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="HCOP_HEADQUATERS_CORP_NO" HeaderText="統編/自然人ID" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="100px">
                                            <ItemStyle  HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="HCOP_REG_NAME" HeaderText="登記名稱">

                                            <ItemStyle  HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="OriginalRiskRanking" HeaderText="風險等級" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="35px" />
                                        <asp:BoundField DataField="DataDate" HeaderText="建案日期" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="65px"/>
                                        <asp:BoundField DataField="" HeaderText="派案日期" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="65px" />
                                        <asp:BoundField DataField="CaseExpiryDate" HeaderText="到期日" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="65px" />
                                        <asp:BoundField DataField="ReviewCompletedDate" HeaderText="審查完成日" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="70px" />
                                        <asp:BoundField DataField="AddressLabelTwoMonthFlagTime" HeaderText="寄送不合作信函日期" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="110px" />
                                        <%--20191218-RQ-2019-030155-002--%>
                                        <%--<asp:BoundField DataField="IncorporatedDate" HeaderText="下不合作日期" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />--%>
                                        <asp:BoundField DataField="IncorporatedDate" HeaderText="不合作/拒絕提供資訊日期" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"  HeaderStyle-Width="150px" />
                                        <asp:BoundField DataField="CaseOwner_User" HeaderText="經辦人員" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"  HeaderStyle-Width="80px"/>
                                    </Columns>
                                    <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                    <PagerSettings Visible="False" />
                                    <RowStyle CssClass="Grid_Item" />
                                    <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                </cc1:CustGridView>
                                <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right"
                                    OnPageChanged="gpList_PageChanged">
                                </cc1:GridPager>
                            </td>
                        </tr>
                    </table>

                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>