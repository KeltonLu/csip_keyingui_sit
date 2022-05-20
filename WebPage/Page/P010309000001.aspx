<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010309000001.aspx.cs" Inherits="P010309000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%--修改紀錄:2021/02/01_Ares_Stanley-收件日期調整為區間--%>
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
            var txtMER = document.getElementById("txtMERCHANTNO");
            
            if ("" == txtRDate.value.Trim() && "" == txtRNUM.value.Trim() && "" == txtMER.value.Trim())
            {
                alert("請至少填寫一個查詢條件！");
                txtRDate.focus();
                return false;
            }
            
            //if ("" != txtRDate.value.Trim())
            //{
            //    //* 日期格式檢查
            //    var dtmReceiveDate = checkDateSn((parseFloat(txtRDate.value.Trim()) - 19110000).toString());
                
            //    if (-2==dtmReceiveDate)
            //    {
            //        alert("收件日期格式不正確，請重新填寫收件日期！");
            //        txtRDate.focus();
            //        return false;
            //    }
            //}
            //* 區間起
            if (document.getElementById("dtpSearchStart_foo").value.Trim() == "") {
                alert("區間必需輸入資料!");
                document.getElementById("dtpSearchStart_foo").focus();
                return false;
            }

            //* 區間迄
            if (document.getElementById("dtpSearchEnd_foo").value.Trim() == "") {
                alert("區間必需輸入資料!");
                document.getElementById("dtpSearchEnd_foo").focus();
                return false;
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
                                <cc1:CustLabel ID="lblTitle" runat="server" CurSymbol="£" ShowID="01_03090000_001"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblReceiveDate" runat="server" CurSymbol="£" IsColon="True" ShowID="01_03070000_005"></cc1:CustLabel></td>
                        <td style="width: 80%">
                            <%--<cc1:CustTextBox ID="txtReceiveDate" runat="server" MaxLength="8" onkeydown="nosubmit();"
                                InputType="Int"></cc1:CustTextBox>--%>
                            <cc1:DatePicker ID="dtpSearchStart" Text="" MaxLength="10" runat="server"></cc1:DatePicker>
                            <span>~</span>
                            <cc1:DatePicker ID="dtpSearchEnd" Text="" MaxLength="10" runat="server"></cc1:DatePicker>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblReceiveNumber" runat="server" CurSymbol="£" IsColon="True"
                                ShowID="01_03070000_006"></cc1:CustLabel></td>
                        <td style="width: 80%">
                            <cc1:CustTextBox ID="txtReceiveNumber" runat="server" Width="150px" MaxLength="12" onkeydown="nosubmit();"
                                InputType="LetterAndInt"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblMERCHANTNO" runat="server" CurSymbol="£" IsColon="True" ShowID="01_03070000_007"></cc1:CustLabel></td>
                        <td style="width: 80%">
                            <cc1:CustTextBox ID="txtMERCHANTNO" runat="server" Width="150px" MaxLength="9" onkeydown="nosubmit();"
                                InputType="Int"></cc1:CustTextBox>
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
                <table width="100%" cellpadding="0" cellspacing="1">
                    <tr>
                        <td>
                            <cc1:CustGridView ID="gvpbDetail" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid"
                                OnRowDataBound="gvpbDetail_RowDataBound">
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
                                    <asp:BoundField>
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="RTime">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="USER_ID1">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="STime1">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="USER_ID2">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="STime2">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ISSAME">
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
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
