<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010102090001.aspx.cs" Inherits="P010102090001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%--2021/03/31_Ares_Stanley-調整Disabled input背景色--%>
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
        function checkInputText(id,intType)
        {
             //*檢核輸入欄位【卡號】是否為空
             if(document.getElementById('txtCardNo').value.Trim() == "")
            {
                alert('請輸入卡號');
                setControlsDisabled('tabNo2');
                document.getElementById('lblBlkCode').innerText="";
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
                
                //*檢核【新BLK CODE】輸入欄為是否為空
                if(document.getElementById('txtNewBlkCode').value.Trim() == "")
                {
                    alert('必須鍵入BLOCK CODE');
                    document.getElementById('txtNewBlkCode').focus();
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
                document.getElementById('lblBlkCode').innerText="";
           }
            document.getElementById("txtCardNoH").value=document.getElementById("txtCardNo").value;
        }
        
    </script>

    <style type="text/css">
   .btnHiden
    {display:none; }
        input:disabled{
            background:white;
            border-width: thin;
        }
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
                                    SetOmit="False" StickHeight="False" Width="275px" IsColon="False" ShowID="01_01020900_001"></cc1:CustLabel></li>
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
                                    SetOmit="False" StickHeight="False" Width="160px" IsColon="False" ShowID="01_01020900_002"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblCardNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01020900_003" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtCardNo" runat="server" MaxLength="16" checktype="num" onkeydown="entersubmit('btnSelect');"
                                onkeyup="ChangeEnable();" BoxName="卡號"></cc1:CustTextBox><cc1:CustTextBox ID="txtCardNoH" runat="server"
                                    MaxLength="16" checktype="num" CssClass="btnHiden" Text=""></cc1:CustTextBox>
                            &nbsp;&nbsp<cc1:CustButton ID="btnSelect" runat="server" CssClass="smallButton" Width="40px"
                                OnClick="btnSelect_Click" OnClientClick="return checkInputText('tabNo1',0);"
                                DisabledWhenSubmit="False" onkeydown="setfocuschoice('txtCardNo','txtNewBlkCode');"
                                ShowID="01_01020900_011" />
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblOldCode" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False" ShowID="01_01020900_004"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            &nbsp;&nbsp<cc1:CustLabel ID="lblBlkCode" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False" Width="180px"></cc1:CustLabel>
                        </td>
                    </tr>
                    <tr>
                        <td nowrap colspan="4" style="height: 1px">
                        </td>
                    </tr>
                    <tr class="itemTitle1">
                        <td colspan="4">
                        </td>
                    </tr>
                </table>
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo2">
                        <tr class="trOdd">
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="lblNewCode" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01020900_005" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3">
                                <cc1:CustTextBox ID="txtNewBlkCode" runat="server" checktype="numandletter" onkeydown="entersubmit('btnSubmit');"
                                    MaxLength="1" Width="50px" onfocus="allselect(this);" BoxName="新BLK CODE"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblPurgeDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01020900_006" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtPurgeDate" runat="server" checktype="num" onkeydown="entersubmit('btnSubmit');"
                                    MaxLength="4" Width="100px" onfocus="allselect(this);" BoxName="PURGE DATE"></cc1:CustTextBox>
                            </td>
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblMemo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01020900_007" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtMemo" runat="server" onkeydown="entersubmit('btnSubmit');"
                                    MaxLength="20" Width="250px" onfocus="allselect(this);" BoxName="MEMO"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="lblReasonCode" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01020900_008" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtReasonCode" runat="server" checktype="numandletter" onkeydown="entersubmit('btnSubmit');"
                                    MaxLength="1" Width="50px" onfocus="allselect(this);" BoxName="REASON CODE"></cc1:CustTextBox>
                            </td>
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="lblActionCode" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01020900_009" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtActionCode" runat="server" checktype="numandletter" onkeydown="entersubmit('btnSubmit');"
                                    MaxLength="2" Width="50px" onfocus="allselect(this);" BoxName="ACTION CODE"></cc1:CustTextBox>
                            </td>                          
                        </tr>
                        <tr>
                            <td nowrap colspan="4" style="height: 1px">
                            </td>
                        </tr>
                        <tr class="itemTitle1">
                            <td align="center" colspan="4">
                                <cc1:CustButton ID="btnSubmit" runat="server" CssClass="smallButton" Width="40px"
                                    OnClick="btnSubmit_Click" OnClientClick="return checkInputText('tabNo2',1);"
                                    onkeydown="setfocus('txtCardNo');" DisabledWhenSubmit="False" ShowID="01_01020900_012" />
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
