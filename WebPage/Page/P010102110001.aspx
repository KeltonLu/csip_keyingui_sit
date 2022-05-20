<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010102110001.aspx.cs" Inherits="P010102110001" %>

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
             //*客戶端檢核
        function checkInputText(id)
        {         
             //*檢核輸入欄位【卡號】是否為空
             if(document.getElementById('txtCardNo').value.Trim() == "")
            {
                alert('請輸入卡號');
                document.getElementById('lblBlkCode').innerText="";
                document.getElementById('txtCardNo').focus();
                return false;
            }
            
            if(!checkInputType(id))
            {
                return false;
            } 
            
            //*顯示確認提示框
            if(!confirm('確定是否要異動資料？'))
            {
                return false;
            } 
            return true;
        }
    </script>
    
    <style type="text/css">
   .btnHiden
    {display:none; }
    </style>
</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <cust:image runat="server" ID="image1"/>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1">
                    <tr class="itemTitle1">
                        <td colspan="4">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="275px" IsColon="False" ShowID="01_01021100_001"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr>
                        <td nowrap colspan="4" style="height: 1px">
                        </td>
                    </tr>
                    <tr class="itemTitle1">
                        <td colspan="4">
                            <li>
                                <cc1:CustLabel ID="lblHead" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="160px" IsColon="False" ShowID="01_01021100_002"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblCardNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01021100_003" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtCardNo" runat="server" MaxLength="16" checktype="num" onkeydown="entersubmit('btnSubmit');" BoxName="卡號" onfocus="allselect(this);"></cc1:CustTextBox>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblOldCode" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False" ShowID="01_01021100_004"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            &nbsp;&nbsp<cc1:CustLabel ID="lblBlkCode" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False" Width="180px"></cc1:CustLabel>
                        </td>
                    </tr>
                </table>
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo2">
                        <tr>
                            <td nowrap colspan="4" style="height: 1px">
                            </td>
                        </tr>
                        <tr class="itemTitle1">
                            <td align="center" colspan="6">
                                <cc1:CustButton ID="btnSubmit" runat="server" CssClass="smallButton" Width="40px"
                                    OnClick="btnSubmit_Click" OnClientClick="return checkInputText('tabNo1');" onkeydown="setfocus('txtCardNo');"
                                    DisabledWhenSubmit="False" ShowID="01_01021100_005" />
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <cc1:CustButton id="btnHiden" onclick="btnHiden_Click" runat="server" CssClass= "btnHiden" DisabledWhenSubmit="False" ></cc1:CustButton>
    </form>
</body>
</html>
