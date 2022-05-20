<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010305010001.aspx.cs" Inherits="P010305010001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
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
        //* 查詢條件都未填寫
        if (document.getElementById("txtBuildDateStart").value.Trim() == "" &&
            document.getElementById("txtBuildDateEnd").value.Trim() == "" && 
            document.getElementById("txtInputDateStart").value.Trim() == "" && 
            document.getElementById("txtInputDateEnd").value.Trim() == "" &&
            document.getElementById("txtBank_Code").value.Trim() == "")
        {
            alert("請填寫查詢或列印條件！");
            document.getElementById("txtBuildDateStart").focus();
            return false;
        }
        
        //* 請將鍵檔日起迄日期資料填寫完整
        if (document.getElementById("txtBuildDateStart").value.Trim() == "" && 
                document.getElementById("txtBuildDateEnd").value.Trim() !="" )
        {
            alert("請將鍵檔日起迄日期資料填寫完整！");
            document.getElementById("txtBuildDateStart").focus();
            return false;
        }
        
        //* 請將鍵檔日起迄日期資料填寫完整
        if (document.getElementById("txtBuildDateStart").value.Trim() != "" && 
                document.getElementById("txtBuildDateEnd").value.Trim() == "" )
        {
            alert("請將鍵檔日起迄日期資料填寫完整！");
            document.getElementById("txtBuildDateEnd").focus();
            return false;
        }
        
        //* 鍵檔日不為空
        if (document.getElementById("txtBuildDateStart").value.Trim() != "")
        {
            var dtmBuildDateStart = checkDateReport(document.getElementById("txtBuildDateStart"));
            if (-2==dtmBuildDateStart)
            {
                alert("日期格式不正確，請重新填寫日期！");
                document.getElementById("txtBuildDateStart").focus();
                return false;
            }
            
            var dtmBuildDateEnd = checkDateReport(document.getElementById("txtBuildDateEnd"));
            if (-2==dtmBuildDateEnd)
            {
                alert("日期格式不正確，請重新填寫日期！");
                document.getElementById("txtBuildDateEnd").focus();
                return false;
            }
            
            //* 鍵檔起迄日 > 鍵檔起迄日，提示“鍵檔日迄日必須不小於起日！”
            if (dtmBuildDateStart > dtmBuildDateEnd)
            {
                alert("鍵檔日迄日必須不小於起日！");
                document.getElementById("dtmBuildDateStart").focus();
                return false;
            }
        }
        
        //* 請將首錄日起迄日期資料填寫完整！
        if (document.getElementById("txtInputDateStart").value.Trim() == "" && 
                document.getElementById("txtInputDateEnd").value.Trim() !="" )
        {
            alert("請將鍵檔日起迄日期資料填寫完整！");
            document.getElementById("txtInputDateStart").focus();
            return false;
        }
        
        //* 請將首錄日起迄日期資料填寫完整！
        if (document.getElementById("txtInputDateStart").value.Trim() != "" && 
                document.getElementById("txtInputDateEnd").value.Trim() == "")
        {
            alert("請將鍵檔日起迄日期資料填寫完整！");
            document.getElementById("txtInputDateEnd").focus();
            return false;
        }
        
        if (document.getElementById("txtInputDateStart").value.Trim()!="")
        {
            var dtmInputDateStart = checkDateReport(document.getElementById("txtInputDateStart"));
            if (-2==dtmInputDateStart)
            {
                alert("日期格式不正確，請重新填寫日期！");
                document.getElementById("txtInputDateStart").focus();
                return false;
            }
            
            var dtmInputDateEnd = checkDateReport(document.getElementById("txtInputDateEnd"));
            if (-2==dtmInputDateEnd)
            {
                alert("日期格式不正確，請重新填寫日期！");
                document.getElementById("txtInputDateEnd").focus();
                return false;
            }
            
            //* 鍵檔起迄日 > 鍵檔起迄日，提示“鍵檔日迄日必須不小於起日！”
            if (dtmInputDateStart > dtmInputDateEnd)
            {
                alert("首錄日迄日必須不小於起日！");
                document.getElementById("dtmBuildDateStart").focus();
                return false;
            }
        }
        
        var patten=new RegExp(/^[0-9]*$/); 
        if (!patten.test(document.getElementById("txtBank_Code").value.Trim()))
        {
            alert("行庫必須是數字型！");
            document.getElementById("txtBank_Code").focus();
            return false;
        }
        
        return true;
    }
    
    </script>

</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <cust:image runat="server" ID="image1"/>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="2">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_03050100_001" StickHeight="False"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblBuild_Date" runat="server" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_03050100_002"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 80%">
                            <cc1:CustTextBox ID="txtBuildDateStart" runat="server" MaxLength="8" onkeydown="nosubmit();"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtBuildDateEnd" runat="server" MaxLength="8" onkeydown="nosubmit();"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblInputDate" runat="server" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_03050100_003"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 80%">
                            <cc1:CustTextBox ID="txtInputDateStart" runat="server" MaxLength="8" onkeydown="nosubmit();"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtInputDateEnd" runat="server" MaxLength="8" onkeydown="nosubmit();"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblBank_Code" runat="server" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_03050100_004"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 80%">
                            <cc1:CustTextBox ID="txtBank_Code" runat="server" MaxLength="3" onkeydown="nosubmit();"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td colspan="2" align="center">
                            <cc1:CustButton ID="btnSearch" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClientClick="return checkInput();" Text="" Width="50px" ShowID="01_03050100_005"
                                OnClick="btnSearch_Click" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnPrint" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                Text="" Width="50px" ShowID="01_03050100_007" OnClientClick="return checkInput();"
                                OnClick="btnPrint_Click" />
                        </td>
                    </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <cc1:CustGridView ID="gvpbACHRecord" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid"
                                OnRowDataBound="gvpbACHRecord_RowDataBound">
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField>
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Receive_Number">
                                        <itemstyle width="13%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Other_Bank_Code_L">
                                        <itemstyle width="8%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Other_Bank_Cus_ID">
                                        <itemstyle width="15%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Other_Bank_Acc_No">
                                        <itemstyle width="13%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Apply_Type">
                                        <itemstyle width="8%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="P02_flag">
                                        <itemstyle width="8%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="R02DateReceive">
                                        <itemstyle width="9%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ACH_Return_Code">
                                        <itemstyle width="9%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Pcmc_Return_Code">
                                        <itemstyle width="12%" horizontalalign="Center" />
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
