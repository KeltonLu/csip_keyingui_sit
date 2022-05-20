<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010102060001.aspx.cs" Inherits="P010102060001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <%-- 20201119,24_Ares_Stanley-修正格式; 20210420_Ares_Stanley-修正取卡方式限制一字元問題 --%>
<head id="Head1" runat="server">
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
                document.getElementById('lblPA1').innerText="";
                document.getElementById('lblPA2').innerText="";
                document.getElementById('lblChineseNameText').innerText="";
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
                
                //*檢核【取卡方式】輸入是否為空
                if(document.getElementById('txtCardFashion').value.Trim() == "")
                {
                    alert('毀損補卡必須選擇取卡方式');
                    document.getElementById('txtCardFashion').focus();
                    return false;
                }
            
//                //*檢核【會員編號】輸入是否符合規范
//                if(document.getElementById('txtMemberNo').value.length > 0 && document.getElementById('txtMemberNo').value.length < 11)
//                {
//                    alert('會員編號有誤,請重新輸入會員編號');
//                    document.getElementById('txtMemberNo').focus();
//                    return false;
//                }
                
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
                document.getElementById('lblPA1').innerText="";
                document.getElementById('lblPA2').innerText="";
                document.getElementById('lblChineseNameText').innerText="";
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
                        <td colspan="4">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="200px" IsColon="False" ShowID="01_01020600_001"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="width: 15%" align="right">
                            <cc1:CustLabel ID="lblCardNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01020600_002" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td colspan="3">
                            <cc1:CustTextBox ID="txtCardNo" runat="server" Width="190px" MaxLength="16" checktype="num"
                                onkeydown="entersubmit('btnSelect');" onkeyup="ChangeEnable();" BoxName="卡號"></cc1:CustTextBox><cc1:CustTextBox
                                    ID="txtCardNoH" runat="server" MaxLength="16" checktype="num" CssClass="btnHiden"
                                    Text=""></cc1:CustTextBox>
                            &nbsp;&nbsp;&nbsp;<cc1:CustButton ID="btnSelect" runat="server" CssClass="smallButton"
                                Width="40px" OnClick="btnSelect_Click" OnClientClick="return checkInputText('tabNo1',0);"
                                DisabledWhenSubmit="False" onkeydown="setfocuschoice('txtCardNo','txtEnglishName');"
                                ShowID="01_01020600_009" />
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
                                <cc1:CustLabel ID="lblChineseName" runat="server" CurSymbol="£"
                                    IsColon="True" ShowID="01_01020600_004" ForeColor="#FF0066"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%">
                                <cc1:CustLabel ID="lblChineseNameText" runat="server" Width="190px"></cc1:CustLabel>
                            </td>
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblCardPattern" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01020600_007" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtCardPattern" runat="server" Width="70px" checktype="num"
                                    MaxLength="2" onkeydown="entersubmit('btnSubmit');" BoxName="製卡式樣"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td align="right">
                                <cc1:CustLabel ID="lblPA" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01020600_003" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td  colspan="3">
                                <cc1:CustLabel ID="lblPA1" runat="server" Width="80px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblPA2" runat="server" Width="80px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblEnglishName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01020600_005" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%" colspan="3">
                                <cc1:CustTextBox ID="txtEnglishName" runat="server" Width="190px" checktype=""
                                    onkeydown="keystoke('btnSubmit','txtCardFashion');" MaxLength="19" BoxName="製卡英文姓名"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="lblCardFashion" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01020600_006" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%; position: relative" colspan="3">
                                <div style="position:relative;">
                                    <cc1:CustDropDownList ID="dropCardFashion" kind="select" runat="server" Style="left: 0px;
                                        top: 0px; clip: rect(0px auto auto 130px); position: absolute; width: 150px;"
                                        onclick="simOptionClick4IE('txtCardFashion');">
                                    </cc1:CustDropDownList>
                                    <cc1:CustTextBox ID="txtCardFashion" runat="server" checktype="num"
                                        checklength="1" onkeydown="entersubmit('btnSubmit');" Style="left: 0px; top: 0px;
                                        position: relative; width: 125px; height: 11px; line-height: 11px;" BoxName="取卡方式"></cc1:CustTextBox>
                                </div>
                            </td>                        
                        </tr>
                        <tr class="trOdd">
                                                    
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblMemberNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01020600_008" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3">
                                <cc1:CustTextBox ID="txtMemberNo" runat="server" Width="190px" checktype="" MaxLength="11"
                                    onkeydown="entersubmit('btnSubmit');" BoxName="會員編號"></cc1:CustTextBox>
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
                                    onkeydown="setfocus('txtCardNo');" DisabledWhenSubmit="False" ShowID="01_01020600_010" />
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
