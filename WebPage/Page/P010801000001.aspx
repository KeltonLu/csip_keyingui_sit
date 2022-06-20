<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010801000001.aspx.cs" Inherits="Page_P010801000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <%-- 2020/11/23_Ares_Stanley-修正格式; 2021/01/21_Ares_Stanley-調整版面; 2021/03/10_Ares_Stanley-調整查詢條件textbox長度; 2021/03/26_Ares_Stanley-調整版面 --%>
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
        <asp:ScriptManager ID="ScriptManager1" runat="server">
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
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01080100_001"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <%--統一編號--%>
                    <tr>
                        <td style="width: 100%" colspan="1">
                            <table border="0" width="100%" cellspacing="1">
                                <tr class="trOdd">
                                    <td align="right" width="4%">
                                        <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01080100_008" StickHeight="False"></cc1:CustLabel>

                                    </td>
                                    <td colspan="12">
                                        <cc1:CustRadioButton ID="radBasicNewproj" runat="server" AutoPostBack="False" GroupName="select" Text="新案" Checked="true" /><label id="cntNew" runat="server"   style="color:red;" ></label>
                                        <cc1:CustRadioButton ID="radBasicProcing" runat="server" AutoPostBack="False" GroupName="select" Text="經辦處理" /><label id="cntprocing" runat="server"   style="color:red;" ></label>
                                        <cc1:CustRadioButton ID="radBasicMaster" runat="server" AutoPostBack="False" GroupName="select" Text="主管放行" /><label id="cntMaster" runat="server"   style="color:red;" ></label>
                                        <cc1:CustRadioButton ID="radBasicReject" runat="server" AutoPostBack="False" GroupName="select" Text="送審退件" /><label id="cntReject" runat="server"   style="color:red;" ></label>
                                        <cc1:CustRadioButton ID="radBasicALL" runat="server" AutoPostBack="False" GroupName="select" Text="全部" /><label id="cntALL" runat="server"    style="color:red;" ></label>
                                    </td>
                                </tr>
                                <tr class="trOdd">
                                    <td align="right" width="4%">
                                        <cc1:CustLabel ID="lblCardNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01080100_002" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="12%">
                                        <cc1:CustTextBox ID="txtTaxID" runat="server" MaxLength="10" checktype="num" Width="80px" onfocus="allselect(this);"
                                             BoxName="統一編號一"></cc1:CustTextBox>
                                        <%--  <cc1:CustTextBox ID="txtCardNo2" runat="server" MaxLength="4" checktype="num" Width="40px"
                                            onkeydown="entersubmit('btnSelect');" onkeyup="changeStatus();" BoxName="統一編號二"></cc1:CustTextBox>--%>
                                    </td>
                                    <td width="6%">
                                        <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01080100_003" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="4%">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtProjYearBegin" runat="server" MaxLength="4" Width="50px" onfocus="allselect(this);"
                                                 BoxName="開始年"
                                              Style="left: 0px; top: 0px; position: relative; width: 50px; height: 11px; line-height:11px"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropProjYearBegin" kind="select" runat="server" onclick="simOptionClick4IE('txtProjYearBegin');"
                                            Style="left: 5px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 59px;">
                                            </cc1:CustDropDownList>
                                        </div>
                                    </td>
                                    <td width="4%">年</td>
                                    <td width="4%">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtProjMonthBegin" runat="server" MaxLength="4" Width="50px" onfocus="allselect(this);"
                                                 BoxName="開始月"
                                                Style="left: 0px; top: 0px; position: relative; width: 50px; height: 11px; line-height:11px"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropProjMonthBegin" kind="select" runat="server" onclick="simOptionClick4IE('txtProjMonthBegin');"
                                                Style="left: 5px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 59px;">
                                            </cc1:CustDropDownList>
                                        </div>
                                    </td>
                                    <td width="4%">月  ~ </td>
                                    <td width="4%">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtProjYearEnd" runat="server" MaxLength="4" Width="50px" onfocus="allselect(this);"
                                                 BoxName="結束年"
                                                Style="left: 0px; top: 0px; position: relative; width: 50px; height: 11px; line-height:11px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropProjYearEnd" kind="select" runat="server" onclick="simOptionClick4IE('txtProjYearEnd');"
                                                Style="left: 5px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 59px;">
                                            </cc1:CustDropDownList>
                                        </div>
                                    </td>
                                    <td width="2%">年</td>
                                    <td width="4%">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtProjMonthEnd" runat="server" MaxLength="4" Width="50px" onfocus="allselect(this);"
                                                 BoxName="結束月"
                                                Style="left: 0px; top: 0px; position: relative; width: 50px; height: 11px; line-height:11px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropProjMonthEnd" kind="select" runat="server" onclick="simOptionClick4IE('txtProjMonthEnd');"
                                                Style="left: 5px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 59px;">
                                            </cc1:CustDropDownList>
                                        </div>
                                    </td>
                                    <td width="2%">月  </td>

                                    <td width="4%">
                                        <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01080100_004" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="4%">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtOriginalRiskRanking" runat="server" MaxLength="3" Width="50px" onfocus="allselect(this);"
                                                 BoxName="風險等級"
                                                Style="left: 0px; top: 0px; position: relative; width: 50px; height: 11px; line-height:11px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropOriginalRiskRanking" kind="select" runat="server" onclick="simOptionClick4IE('txtOriginalRiskRanking');"
                                                Style="left: 5px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 59px;">
                                            </cc1:CustDropDownList>
                                        </div>
                                    </td>
                                </tr>
                                <tr class="trOdd">
                                    <td align="right" width="8%">
                                        <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01080100_005" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="8%">
                                        <cc1:CustTextBox ID="txtCASE_NO" runat="server" MaxLength="14" checktype="num" Width="120px" onfocus="allselect(this);"
                                            onkeydown="entersubmit('btnSelect');"  BoxName="案件編號"></cc1:CustTextBox>

                                    </td>
                                    <td width="6%">
                                        <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01080100_006" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="8%">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtCaseOwner_User" runat="server" Width="90px" onfocus="allselect(this);"
                                                 BoxName="處理經辦"
                                                Style="left: 0px; top: 0px; position: relative; width: 90px; height: 11px; line-height:11px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropCaseOwner_User" kind="select" runat="server" onclick="simOptionClick4IE('txtCaseOwner_User');"
                                                Style="left: 0px; top: 0px; clip: rect(0px auto auto 80px); position: absolute; width: 100px;">
                                            </cc1:CustDropDownList>
                                        </div>
                                    </td>
                                    <%--20191025 修改：增加不合作註記的查詢條件 by Peggy--%>
                                    <td width="6%">
                                        <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01080100_010" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="8%">
                                        <div style="position: relative">
                                            <div style="position: relative">
                                            <cc1:CustTextBox ID="txtIncorporated" runat="server" MaxLength="3" Width="90px" onfocus="allselect(this);"
                                                 BoxName="不合作註記"
                                                Style="left: 0px; top: 0px; position: relative; width: 90px; height: 11px; line-height:11px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropIncorporated" kind="select" runat="server" onclick="simOptionClick4IE('txtIncorporated');"
                                                Style="left: 0px; top: 0px; clip: rect(0px auto auto 80px); position: absolute; width: 100px;">
                                                <asp:ListItem Value="X">請選擇</asp:ListItem>
                                                 <asp:ListItem Value="Y">Y</asp:ListItem>
                                                 <asp:ListItem Value="N">N</asp:ListItem>
                                            </cc1:CustDropDownList>
                                        </div>
                                        </div>
                                    </td>
                                    <%--20191025--%>
                                    <td width="4%"><%--排序欄位--%>
                                        <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01080100_007" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="7%">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtSort" runat="server" MaxLength="5" Width="80px" onfocus="allselect(this);"
                                                 BoxName="排序欄位"
                                                Style="left: 0px; top: 0px; position: relative; width: 80px; height: 11px; line-height:11px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropSort" kind="select" runat="server" onclick="simOptionClick4IE('txtSort');"
                                                Style="left: 0px; top: 0px; clip: rect(0px auto auto 70px); position: absolute; width: 90px;">
                                            </cc1:CustDropDownList>
                                        </div>
                                    </td>
                                    <td width="4%">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtAsc" runat="server" MaxLength="3" Width="50px" onfocus="allselect(this);"
                                                 BoxName="排序欄位"
                                                Style="left: 0px; top: 0px; position: relative; width: 50px; height: 11px; line-height:11px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropAsc" kind="select" runat="server" onclick="simOptionClick4IE('txtAsc');"
                                                Style="left: 5px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 59px;">
                                            </cc1:CustDropDownList>

                                        </div>
                                    </td>
                                    <td width="4%"><%--排序欄位--%>
                                        <cc1:CustLabel ID="CustLabel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01080100_011" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="4%">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtStatus" runat="server" MaxLength="3" Width="90px" onfocus="allselect(this);"
                                                 BoxName="呈核狀態"
                                                Style="left: 0px; top: 0px; position: relative; width: 90px; height: 11px; line-height:11px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropStatus" kind="select" runat="server" onclick="simOptionClick4IE('txtStatus');"
                                                Style="left: 0px; top: 0px; clip: rect(0px auto auto 80px); position: absolute; width: 100px;">
                                                <asp:ListItem Value="">請選擇</asp:ListItem>
                                                 <asp:ListItem Value="C1">一階主管</asp:ListItem>
                                                 <asp:ListItem Value="C2">二階主管</asp:ListItem>
                                            </cc1:CustDropDownList>
                                        </div>
                                    </td>
                                    <%--20191025-RQ-2018-015749-002 modify by Peggy--%>
                                    <%--<td colspan="6">--%>
                                    <td colspan="4">
                                        <cc1:CustButton ID="btnSelect" runat="server" CssClass="smallButton" ShowID="01_01040101_027"
                                            OnClick="btnSelect_Click" OnClientClick="return checkInputText('pnlText', 0);"
                                            DisabledWhenSubmit="False" onkeydown="setfocusmove();" BoxName="查詢" />
                                    </td>
                                </tr>
                                <tr class="itemTitle">
                                    <td colspan="13" align="right">
                                    <cc1:CustButton ID="btnJob" runat="server" CssClass="smallButton" ShowID="01_01080100_009"
                                            OnClick="btnJob_Click" OnClientClick="if(!confirm('是否手動執行寄送不合作信函排程')) return false; "
                                            DisabledWhenSubmit="False" BoxName="手動執行寄送不合作信函排程" />
                                            
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
                                <cc1:CustGridView ID="grvCardData" runat="server" BorderStyle="Solid" EnableModelValidation="True" PagerID="GridPager1" Width="100%" OnRowDataBound="grvCardData_RowDataBound" OnRowCommand="grvCardData_RowCommand" CssClass="longTableGridView">
                                    <AlternatingRowStyle CssClass="Grid_AlternatingItem" />
                                    <Columns>
                                          <asp:BoundField DataField="Row#" HeaderText="序號" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                        <asp:TemplateField HeaderText="案件編號" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" ID="lnkView" CommandArgument='<%#Eval("ArgNo") %>'
                                                    CommandName="VIEW"><%#Eval("CASE_NO") %></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="HCOP_HEADQUATERS_CORP_NO" HeaderText="統編/自然人ID" HeaderStyle-HorizontalAlign="Center">
                                            <ItemStyle Width="120px" HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="HCOP_REG_NAME" HeaderText="登記名稱">

                                            <ItemStyle Width="120px" HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="OriginalRiskRanking" HeaderText="風險等級" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="DataDate" HeaderText="建案日期" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="" HeaderText="派案日期" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="CaseExpiryDate" HeaderText="到期日" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="ReviewCompletedDate" HeaderText="審查完成日" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="Status" HeaderText="呈核狀態" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="AddressLabelTwoMonthFlagTime" HeaderText="寄送不合作信函日期" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                        <%--<asp:BoundField DataField="IncorporatedDate" HeaderText="下不合作日期" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />--%>
                                        <asp:BoundField DataField="IncorporatedDate" HeaderText="不合作/拒絕提供資訊日期" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="CaseOwner_User" HeaderText="經辦人員" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
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
