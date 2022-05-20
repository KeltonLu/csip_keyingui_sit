<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010308000001.aspx.cs" Inherits="P010308000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%--2021/02/20_Ares_Stanley-調整版面, 避免查詢資料超出頁面--%>
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
            var txtRDate = document.getElementById("txtReceiveDate");
            var txtRNUM = document.getElementById("txtReceiveNumber");            
            
            if ("" == txtRDate.value.Trim() && "" == txtRNUM.value.Trim())
            {
                alert("請至少填寫一個查詢條件！");
                txtRDate.focus();
                return false;
            }
            
            if ("" != txtRDate.value.Trim())
            {
                //* 日期格式檢查
                var dtmReceiveDate = checkDateSn((parseFloat(txtRDate.value.Trim()) - 19110000).toString());
                
                if (-2==dtmReceiveDate)
                {
                    alert("收件日期格式不正確，請重新填寫收件日期！");
                    txtRDate.focus();
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
                                <cc1:CustLabel ID="lblTitle" runat="server" CurSymbol="£" ShowID="01_03070000_057"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblReceiveDate" runat="server" CurSymbol="£" IsColon="True" ShowID="01_03070000_005"></cc1:CustLabel></td>
                        <td style="width: 80%">
                            <cc1:CustTextBox ID="txtReceiveDate" runat="server" MaxLength="8" onkeydown="nosubmit();" InputType="Int"></cc1:CustTextBox>
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
                    <tr class="itemTitle">
                        <td colspan="2" align="center">
                            <cc1:CustButton ID="btnSearch" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClientClick="return checkInput();" Text="" Width="50px" ShowID="01_03060000_003"
                                OnClick="btnSearch_Click" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnPrint" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                Text="" Width="50px" ShowID="01_03060000_004" OnClientClick="return checkInput();"
                                OnClick="btnPrint_Click" />
                        </td>
                    </tr>
                </table>
                <asp:Panel ID="scrollPanel" runat="server" ScrollBars="Auto">
                <table width="100%" cellpadding="0" cellspacing="1">
                    <tr>
                        <td>
                            <cc1:CustGridView ID="gvpbAward" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid"
                                OnRowDataBound="gvpbAward_RowDataBound">
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
                                    <asp:BoundField DataField="MSG_TYPE">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ORG">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="PROG_ID">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="PARTNER_ID">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ACCUMULATION_TYPE">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="TC_CODE1">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="TC_CODE2">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="MCC_FROM_1">
                                        <itemstyle horizontalalign="Center" wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="MCC_FROM_2">
                                        <itemstyle horizontalalign="Center" wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="MCC_FROM_3">
                                        <itemstyle horizontalalign="Center" wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="MCC_FROM_4">
                                        <itemstyle horizontalalign="Center" wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="MCC_FROM_5">
                                        <itemstyle horizontalalign="Center" wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="MCC_FROM_6">
                                        <itemstyle horizontalalign="Center" wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="MCC_FROM_7">
                                        <itemstyle horizontalalign="Center" wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="MCC_FROM_8">
                                        <itemstyle horizontalalign="Center" wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="MCC_FROM_9">
                                        <itemstyle horizontalalign="Center" wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="MCC_FROM_10">
                                        <itemstyle horizontalalign="Center" wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="COUNTRY_CODE_IND">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="COUNTRY_CODE1">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="COUNTRY_CODE2">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="COUNTRY_CODE3">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="COUNTRY_CODE4">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="COUNTRY_CODE5">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="COUNTRY_CODE6">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="COUNTRY_CODE7">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="COUNTRY_CODE8">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="COUNTRY_CODE9">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="COUNTRY_CODE10">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BASIC_CALC_IND">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BASIC_TIER_AMT1">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BASIC_TIER_RATE1">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BASIC_TIER_AMT2">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BASIC_TIER_RATE2">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BASIC_TIER_AMT3">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BASIC_TIER_RATE3">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BASIC_TIER_AMT4">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BASIC_TIER_RATE4">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SUPP_BASIC_CAL_IND">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SUPP_BASIC_TIER_AMT1">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SUPP_BASIC_TIER_RATE1">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SUPP_BASIC_TIER_AMT2">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SUPP_BASIC_TIER_RATE2">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SUPP_BASIC_TIER_AMT3">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SUPP_BASIC_TIER_RATE3">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SUPP_BASIC_TIER_AMT4">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SUPP_BASIC_TIER_RATE4">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="PROMO_START_DTE">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="PROMO_END_DTE">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="PROMO_CALC_IND">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="PROMO_TIER_AMT1">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="PROMO_TIER_RATE1">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="PROMO_TIER_AMT2">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="PROMO_TIER_RATE2">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="PROMO_TIER_AMT3">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="PROMO_TIER_RATE3">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="PROMO_TIER_AMT4">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="PROMO_TIER_RATE4">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BTHDTE_START">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BTHDTE_END">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BTHDTE_CALC_IND">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BTHDTE_TIER_AMT1">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BTHDTE_TIER_RATE1">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BTHDTE_TIER_AMT2">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BTHDTE_TIER_RATE2">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BTHDTE_TIER_AMT3">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BTHDTE_TIER_RATE3">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BTHDTE_TIER_AMT4">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BTHDTE_TIER_RATE4">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                </Columns>
                            </cc1:CustGridView>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right"
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
