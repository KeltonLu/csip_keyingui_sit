<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010311040001.aspx.cs" Inherits="P010311040001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
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
    
    // 輸入欄位正確性檢查
    function checkInput()
    {
        // 拋檔日填寫不完整
        if (document.getElementById("txtSendFileDate").value.Trim() == "")
        {
            alert("請填寫拋檔日！");
            document.getElementById("txtSendFileDate").focus();
            return false;
        }
        else
        {
            var dtmSendFileDate = checkDateReport(document.getElementById("txtSendFileDate"));
            if (-2==dtmSendFileDate)
            {
                alert("日期格式不正確，請重新填寫日期！");
                document.getElementById("txtSendFileDate").focus();
                return false;
            }
        }
        
        return true;
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
                        <td colspan="4">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_03110400_001" StickHeight="False"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblSendFileDate" runat="server" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_03110400_002"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 30%">
                            <cc1:CustTextBox ID="txtSendFileDate" runat="server" MaxLength="8" onkeydown="nosubmit();"></cc1:CustTextBox>
                        </td>
                        <td align="right" style="width: 20%">
                        </td>
                        <td style="width: 30%">
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 20%">
                            &nbsp;&nbsp;
                        </td>
                        <td colspan="3" style="width: 80%" align="left">
                            <cc1:CustRadioButton ID="radBatchStatics" runat="server" AutoPostBack="True" OnCheckedChanged="radBatchStatics_CheckedChanged" />&nbsp;&nbsp;&nbsp;&nbsp;
                            <cc1:CustButton ID="btnPrint" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClientClick="return checkInput();" Text="" Width="50px" ShowID="01_03110400_005"
                                OnClick="btnPrint_Click" /></td>
                    </tr>
                    <tr class="trOdd">
                        <td style="width: 20%">
                            &nbsp;&nbsp;
                        </td>
                        <td style="width: 80%" align="left" colspan="3">
                            <cc1:CustRadioButton ID="radBatchResult" runat="server" AutoPostBack="True" OnCheckedChanged="radBatchResult_CheckedChanged" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <cc1:CustButton ID="btnPrintSuccess" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClientClick="return checkInput();" Text="" Width="80px" ShowID="01_03110400_007"
                                OnClick="btnPrintSuccess_Click" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnPrintFault" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                Text="" Width="80px" ShowID="01_03110400_008" OnClientClick="return checkInput();"
                                OnClick="btnPrintFault_Click" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
