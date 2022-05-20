<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010101020001.aspx.cs" Inherits="P010101020001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <%-- 20201124_Ares_Stanley-修正格式; 20210420_Ares_Stanley-調整學歷欄位字數限制 --%>
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
      
            //*檢核輸入欄位【身分證號碼】是否為空                  
            if(document.getElementById('txtUserId').value.Trim() == "")
            {
                alert('請輸入身分證號碼');
                document.getElementById('txtUserId').focus();
           
                setControlsDisabled('tabNo2');
                document.getElementById('lblUserNameText').innerText = "";
                document.getElementById('dropStudy').disabled = true;
                return false;
            }
                
            if(!checkInputType(id))
            {
                return false;
            } 
            

            
             //*提交按鈕
            if(intType == 1)
            {           
                //*顯示確認提示框                checkInputType('tabNo1');
                if(!confirm('確定是否要異動資料？'))
                {
                    return false;
                }
            }
            return true;
        }
        
        function ChangeEnable()
        {
                
           if (document.getElementById("txtUserId").value.toUpperCase().Trim()!=document.getElementById("txtUserIdHide").value.toUpperCase().Trim())
           {
                setControlsDisabled('tabNo2');
                document.getElementById('lblUserNameText').innerText = "";
                document.getElementById('dropStudy').disabled = true;
            
           }
         
            document.getElementById("txtUserIdHide").value=document.getElementById("txtUserId").value;
        }

        

    </script>

    <style type="text/css">
   .btnHiden
    {display:none; }
    </style>
</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <cust:image runat="server" ID="image1"/>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" style="" id="tabNo1">
                        <tr class="itemTitle">
                            <td colspan="4">
                                <li>
                                    <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" Width="200px" IsColon="False" ShowID="01_01010200_001"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%; height: 25px;" align="right">
                                <cc1:CustLabel ID="lblUserId" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="01_01010200_002"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%; height: 25px;">
                                <cc1:CustTextBox ID="txtUserId" runat="server" MaxLength="12" checktype="ID" onkeydown="entersubmit('btnSelect');"
                                    onkeyup="ChangeEnable();" BoxName="身分證號碼"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtUserIdHide" runat="server" MaxLength="10" CssClass="btnHiden"
                                    Text=""></cc1:CustTextBox>&nbsp;&nbsp;&nbsp;&nbsp;
                                <cc1:CustButton ID="btnSelect" CssClass="smallButton" runat="server" Width="40px"
                                    OnClick="btnSelect_Click" OnClientClick="return checkInputText('tabNo1',0);"
                                    DisabledWhenSubmit="False" onkeydown="setfocuschoice('txtUserId','txtStudy');" />
                            </td>
                            <td align="right" style="width: 15%; height: 25px;">
                                <cc1:CustLabel ID="lblUserName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="01_01010200_004"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%; height: 25px">
                                <cc1:CustLabel ID="lblUserNameText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1">
                        <tr>
                            <td align="right" colspan="4" style="height: 1px">
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo2" style="">
                        <tr class="itemTitle">
                            <td colspan="4">
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%" height="27" align="right">
                                <cc1:CustLabel ID="lblStudy" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="01_01010200_005"></cc1:CustLabel>
                            </td>
                            <td style="width: 85%; position: relative" colspan="3">
                                <div style="position:relative;">
                                    <cc1:CustDropDownList ID="dropStudy" runat="server" Style="left: 0px; top: 0px; clip: rect(0px auto auto 130px);
                                        position: absolute; width: 150px;" onclick="simOptionClick4IE('txtStudy');">
                                    </cc1:CustDropDownList>
                                    <cc1:CustTextBox ID="txtStudy" runat="server" checklength="2" checktype="num"
                                        onkeydown="entersubmit('btnSubmit');" Style="left: 0px; top: 0px; position: relative;
                                        width: 125px; height: 11px; line-height: 11px;" BoxName="學歷"></cc1:CustTextBox>
                                </div>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblYear" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="01_01010200_006"></cc1:CustLabel>
                            </td>
                            <td style="width: 85%" colspan="3">
                                <cc1:CustTextBox ID="txtYear" runat="server" Width="70px" checktype="num" MaxLength="2"
                                    onkeydown="entersubmit('btnSubmit');" BoxName="畢業西元年"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblMonth" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="01_01010200_007"></cc1:CustLabel>
                            </td>
                            <td style="width: 85%" colspan="3">
                                <cc1:CustTextBox ID="txtMonth" runat="server" Width="70px" checktype="num" MaxLength="2"
                                    onkeydown="entersubmit('btnSubmit');" BoxName="月"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 15%; height: 1px" align="right" colspan="4">
                            </td>
                        </tr>
                        <tr class="itemTitle">
                            <td colspan="4" align='center'>
                                <cc1:CustButton ID="btnSubmit" CssClass="smallButton" Width="40px" runat="server"
                                    OnClientClick="return checkInputText('tabNo2',1);" onkeydown="setfocus('txtUserId');"
                                    OnClick="btnSubmit_Click" DisabledWhenSubmit="False" /></td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
            <br />
        </div>
    </form>
</body>
<script type="text/javascript" language="javascript">
//add by Mars 解決IE6以上瀏覽時下拉選單長度不夠被TEXTBOX擋住 2012-12-04
		var isIE6 = navigator.userAgent.search("MSIE 6") > -1;
		function fixwidth()		
		{
              if(!isIE6){
                document.getElementById("dropStudy").style.width = 165;
              }
		}
		fixwidth();
		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(fixwidth);
</script>
</html>
