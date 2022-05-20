<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010107020001.aspx.cs" Inherits="P010107020001" %>

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

       
    

    function checkInput()
    {
        var strDateStart=document.getElementById("dpBatchDate_foo").value.Trim();
        var strReceiveBatch = document.getElementById("txtReceiveBatch").value.Trim();
        if (strDateStart == "")
        {
            alert("請選擇編批日期.");
            return false;
        }
        if (strReceiveBatch == "")
        {
            alert("請輸入收件批次.");
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
<body class="workingArea" >
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <cust:image runat="server" ID="image1"/>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="4">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01070200_001" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01070200_002" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 85%">
                            <cc1:DatePicker ID="dpBatchDate" runat="server"></cc1:DatePicker>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01070200_003" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 85%">
                            <cc1:CustTextBox ID="txtReceiveBatch" runat="server" MaxLength="1" InputType="Int" Style="ime-mode: disabled;text-align: left"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01070200_004"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 80%">
                            <cc1:CustDropDownList ID="dropSignType" runat="server">
                            </cc1:CustDropDownList>
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td align="center" colspan="4" >
                                <cc1:CustButton ID="btnExport" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClick="btnExport_Click" OnClientClick="return checkInput();" Text="" Width="150px"
                                ShowID="01_01070200_005" />  &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;
                            
                                <cc1:CustButton ID="btnExportFTP" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClick="btnExportFTP_Click" OnClientClick="return checkInput();" Text="" Width="150px"
                                ShowID="01_01070200_007" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <cc1:CustButton ID="btnHidExportFTP" runat="server" CssClass="btnHiden" DisabledWhenSubmit="False" OnClick="btn_ExportFTP" />
    </form>
</body>
</html>
