<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010307000001.aspx.cs" Inherits="P010307000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%-- 2021/02/20_Ares_Stanley-調整版面, 避免查詢資料超出頁面; 2021/03/08_Ares_Stanley-查詢單一日期改為查詢日期區間 --%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript">
        //* 輸入欄位正確性檢查
        function checkInput()
        {
            var txtRDateStart = document.getElementById("txtReceiveDateStart");
            var txtRDateEnd = document.getElementById("txtReceiveDateEnd");
            var txtRNUM = document.getElementById("txtReceiveNumber");
            var txtMER = document.getElementById("txtMERCHANTNO");

            //全部為空
            if ("" == txtReceiveDateStart.value.Trim() && "" == txtReceiveDateEnd.value.Trim() && "" == txtRNUM.value.Trim() && "" == txtMER.value.Trim())
            {
                alert("請至少填寫一個查詢條件！");
                txtReceiveDateStart.focus();
                return false;
            }

            //只填寫收件起日
            if (txtReceiveDateStart.value.Trim() != "" && txtReceiveDateEnd.value.Trim() == "" && txtRNUM.value.Trim() == "" && txtMER.value.Trim() == "") {
                alert("請完整填寫收件起日及迄日！");
                txtReceiveDateStart.focus();
                return false;
            }
            //只填寫收件訖日
            if (txtReceiveDateStart.value.Trim() == "" && txtReceiveDateEnd.value.Trim() != "" && txtRNUM.value.Trim() == "" && txtMER.value.Trim() == "") {
                alert("請完整填寫收件起日及迄日！");
                txtReceiveDateStart.focus();
                return false;
            }
            
            if ("" != txtReceiveDateStart.value.Trim())
            {
                //* 日期格式檢查
                var dtmReceiveDateStart = checkDateSn((parseFloat(txtReceiveDateStart.value.Trim()) - 19110000).toString());
                
                if (-2 == dtmReceiveDateStart)
                {
                    alert("收件起日日期格式不正確，請重新填寫收件日期！");
                    txtReceiveDateStart.focus();
                    return false;
                }
            }
            if ("" != txtReceiveDateStart.value.Trim()) {
                //* 日期格式檢查
                var dtmReceiveDateEnd = checkDateSn((parseFloat(txtReceiveDateEnd.value.Trim()) - 19110000).toString());

                if (-2 == dtmReceiveDateEnd) {
                    alert("收件訖日日期格式不正確，請重新填寫收件日期！");
                    txtReceiveDateEnd.focus();
                    return false;
                }
            }
        }
    </script>

</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <cust:image runat="server" ID="image1" />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="2">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurSymbol="£" ShowID="01_03070000_001"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblReceiveDate" runat="server" CurSymbol="£" IsColon="True" ShowID="01_03070000_005"></cc1:CustLabel></td>
                        <td style="width: 80%">
                            <cc1:CustTextBox ID="txtReceiveDateStart" runat="server" MaxLength="8" onkeydown="nosubmit();" InputType="Int"></cc1:CustTextBox>
                            <span>~</span>
                            <cc1:CustTextBox ID="txtReceiveDateEnd" runat="server" MaxLength="8" onkeydown="nosubmit();" InputType="Int"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblReceiveNumber" runat="server" CurSymbol="£" IsColon="True"
                                ShowID="01_03070000_006"></cc1:CustLabel></td>
                        <td style="width: 80%">
                            <cc1:CustTextBox ID="txtReceiveNumber" runat="server" MaxLength="12" onkeydown="nosubmit();" InputType="LetterAndInt"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblMERCHANTNO" runat="server" CurSymbol="£" IsColon="True" ShowID="01_03070000_007"></cc1:CustLabel></td>
                        <td style="width: 80%">
                            <cc1:CustTextBox ID="txtMERCHANTNO" runat="server" MaxLength="9" onkeydown="nosubmit();" InputType="Int"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td align="center" colspan="2">
                            <cc1:CustButton ID="btnSearch" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClientClick="return checkInput();" Text="" Width="50px" ShowID="01_03060000_003"
                                OnClick="btnSearch_Click" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnPrint" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                Text="" Width="50px" ShowID="01_03060000_004" OnClientClick="return checkInput();"
                                OnClick="btnPrint_Click" />
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td align="left"colspan="2">
                            <cc1:CustLabel ID="lblRowCountInfo" runat="server" CurSymbol="£" IsColon="True"></cc1:CustLabel></td>                    
                    </tr>                    
                </table>
                <asp:Panel ID="scrollPanel" runat="server" ScrollBars="Auto">
                <table width="100%" cellpadding="0" cellspacing="1">
                    <tr>
                        <td>
                            <cc1:CustGridView ID="gvpbRedeem" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid"
                                OnRowDataBound="gvpbRedeem_RowDataBound">
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="RECEIVE_NUMBER">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="FUNCTION_CODE">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="MSG_SEQ">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="MSG_ERR">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="IN_ORG">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="IN_MERCHANT">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="IN_PROD_CODE">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="IN_CARD_TYPE">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="PROGID">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="MER_RATE">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="LIMITR">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CHPOINT">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CHAMT">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="USER_EXIT">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="STARTU">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ENDU">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CYLCO">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="LIMITU">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CHPOINTU">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CHAMTU">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BIRTH">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="STARTB">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ENDB">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="LIMITB">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CHPOINTB">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CHAMTB">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                </Columns>
                            </cc1:CustGridView>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Left"
                                InputBoxStyle="height:15px" OnPageChanged="gpList_PageChanged" PrevPageText="<前一頁"
                                SubmitButtonText="Go">
                            </cc1:GridPager>
                        </td>
                    </tr>
                </table>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
