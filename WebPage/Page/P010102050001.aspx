<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010102050001.aspx.cs" Inherits="P010102050001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript">
        function checkInputText(id,intType)
        {
             //*檢核輸入欄位【卡號】是否為空
             if(document.getElementById('txtCardNo').value.Trim() == "")
            {
                alert('請輸入卡號');
                setControlsDisabled('tabNo2');
                document.getElementById('lblNameText').innerText="";
                document.getElementById('txtCardNo').focus();
                return false;
            }
            
            if(!checkInputType(id))
            {
                return false;
            } 
            
             //*提交按鈕
            if(intType == 1)
            {         
                //*檢核查詢部分
                if(!checkInputType('tabNo1'))
                {
                      return false;
                }
                
                //*顯示確認提示框
                if(!confirm('確定是否要異動資料？'))
                {
                    return false;
                }
            }
            return true;
        }
        
        function ChangeEnable()
        {
           if (document.getElementById("txtCardNo").value.toUpperCase().Trim()!=document.getElementById("txtCardNoH").value.toUpperCase().Trim())
           {
                setControlsDisabled('tabNo2');
                document.getElementById('lblNameText').innerText="";
           }
            document.getElementById("txtCardNoH").value=document.getElementById("txtCardNo").value;
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
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                    <tr class="itemTitle">
                        <td colspan="5">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="200px" IsColon="False" ShowID="01_01020500_001"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="width: 15%" align="right">
                            <cc1:CustLabel ID="lblCardNo" runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01020500_002" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtCardNo" runat="server" Width="190px" MaxLength="16" checktype="num"
                                onkeydown="entersubmit('btnSelect');" onkeyup="ChangeEnable();" BoxName="卡號"></cc1:CustTextBox><cc1:CustTextBox
                                    ID="txtCardNoH" runat="server" MaxLength="16" checktype="num" CssClass="btnHiden"
                                    Text=""></cc1:CustTextBox>
                            &nbsp;&nbsp;&nbsp;<cc1:CustButton ID="btnSelect" runat="server" CssClass="smallButton"
                                Width="40px" OnClientClick="return checkInputText('tabNo1',0);" OnClick="btnSelect_Click"
                                DisabledWhenSubmit="False" onkeydown="setfocuschoice('txtCardNo','txtFavourableCode');"
                                ShowID="01_01020500_006" />
                        </td>
                        <td style="width: 15%" align="right">
                            <cc1:CustLabel ID="lblName" runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01020500_003" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <cc1:CustLabel ID="lblNameText" runat="server" Width="160px" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel>
                        </td>
                    </tr>
                    <tr>
                        <td nowrap colspan="4" style="height: 1px">
                        </td>
                    </tr>
                </table>
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo2" style="">
                        <tr class="itemTitle">
                            <td colspan="4">
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblFavourableCode" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01020500_004"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtFavourableCode" runat="server" Width="190px" checktype="num"
                                    onkeydown="entersubmit('btnSubmit');" MaxLength="2" BoxName="優惠碼"></cc1:CustTextBox>
                            </td>
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblCardType" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01020500_005" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtCardType" runat="server" Width="190px" checktype="num" onkeydown="entersubmit('btnSubmit');"
                                    MaxLength="2" BoxName="卡人類別"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td nowrap colspan="4" style="height: 1px">
                            </td>
                        </tr>
                        <tr class="itemTitle">
                            <td colspan="4" align="center">
                                <cc1:CustButton ID="btnSubmit" CssClass="smallButton" Width="40px" runat="server"
                                    OnClientClick="return checkInputText('tabNo2',1);" OnClick="btnSubmit_Click"
                                    onkeydown="setfocus('txtCardNo');" DisabledWhenSubmit="False" ShowID="01_01020500_007" />
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
