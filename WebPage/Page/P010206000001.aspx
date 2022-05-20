<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010206000001.aspx.cs" Inherits="P010206000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>
    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>
    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>
    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>
    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />
    
    <style type="text/css" >
   .btnHiden
    {display:none; }
    </style>
    
    <script type="text/javascript" language="javascript">
        
        //*客戶端檢核
       
        function checkInputText(id)
        {     
             //*檢核收件編號輸入欄位
             var obj =  document.getElementById('txtReceiveNumber');
             obj.style.color="black"; 
             //if(obj.value.Trim().length != 12)
             if(obj.value.Trim().length != 13)
            {
                clearText();
                alert('收件編號格式不對！');
                obj.style.color="red";
                obj.focus();
                return false;
            }  
            
            //if (checkDateSn(document.getElementById("txtReceiveNumber").value.Trim().substring(0,8))==-2)
            if (checkDateSn(document.getElementById("txtReceiveNumber").value.Trim().substring(0,8))==-2)
            {
                clearText();
                alert('收件編號格式不對！');
                obj.style.color="red";
                obj.focus();
                return false;
            }
                             
             if(!checkInputType(id))
            {
                return false;
            }  
            

                            
            return true;
        }
        
        function clearText()
        {    
            document.getElementById('lblBankCodeText').innerText = "";
            document.getElementById('lblBankAccNoText').innerText = "";
            document.getElementById('lblBankPayWayText').innerText = "";
            document.getElementById('lblBankCusIDText').innerText = "";
            document.getElementById('lblBcycleCodeText').innerText = "";
            document.getElementById('lblMobilePhoneText').innerText = "";
            document.getElementById('lblEMailText').innerText = "";
            document.getElementById('lblEBillText').innerText = "";
            document.getElementById('lblBuildDateText').innerText = "";
            document.getElementById('lblApplyTypeText').innerText = "";
            document.getElementById('lblDealNoText').innerText = "";
            document.getElementById('lblACHReturnCodeText').innerText = "";
            document.getElementById('lblRtnDateText').innerText = "";
            document.getElementById('lblRemarkText').innerText = "";
            document.getElementById('lblCodeText').innerText = "";
            document.getElementById('lblBankCodeLText').innerText = "";
            document.getElementById('lblBankNameText').innerText = "";     
        }       
        
         function ChangeEnableR()
        {
        
   
        
           if (document.getElementById("txtReceiveNumber").value.toUpperCase().Trim()!=document.getElementById("txtReceiveNumberH").value.toUpperCase().Trim())
           {
               document.getElementById("btnDelete").disabled = true;
               clearText();
            
           }
         
            document.getElementById("txtReceiveNumberH").value=document.getElementById("txtReceiveNumber").value;
        }
        
        function ChangeEnableI()
        {
        
   
        
           if (document.getElementById("txtUserId").value.toUpperCase().Trim()!=document.getElementById("txtUserIdH").value.toUpperCase().Trim())
           {
              document.getElementById("btnDelete").disabled = true;
               clearText();
           }
         
            document.getElementById("txtUserIdH").value=document.getElementById("txtUserId").value;
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
		            <td colspan="2"><li><cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04040000_001" StickHeight="False" Width="240px"></cc1:CustLabel></li></td>
	            </tr>
	            <tr class="trOdd">	
                    <td style="width: 20%" align="right" >
                        <cc1:CustLabel ID="lblReceiveNumber" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04040000_002" StickHeight="False"></cc1:CustLabel>
                    </td>
                    <td style="width: 80%">                        
                        <cc1:CustTextBox ID="txtReceiveNumber" runat="server" MaxLength="13" checktype="num" onkeydown="keypressstop();" onkeyup="ChangeEnableR();"></cc1:CustTextBox>
                        <cc1:CustTextBox ID="txtReceiveNumberH" runat="server" MaxLength="13" checktype="num" CssClass="btnHiden" Text=""></cc1:CustTextBox>
                     </td> 
	            </tr>
	            <tr class="trEven">
	                <td style="width: 20%" align="right" >
                        <cc1:CustLabel ID="lblUserId" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04040000_003" StickHeight="False"></cc1:CustLabel>
                    </td>
                    <td style="width: 80%">
                        <cc1:CustTextBox ID="txtUserId" runat="server" MaxLength="20" checktype="ID" onkeydown="entersubmit('btnSelect');" onkeyup="ChangeEnableI();"></cc1:CustTextBox>
                        <cc1:CustTextBox ID="txtUserIdH" runat="server" MaxLength="20" checktype="numandletter" CssClass="btnHiden" Text=""></cc1:CustTextBox>
                    </td>
	            </tr>
	            <tr class="itemTitle">
	                <td style="width: 20%">	                    
	                </td>
		            <td style="width: 80%">
		                <cc1:CustButton ID="btnSelect" CssClass="smallButton" runat="server" Width="40px" OnClick="btnSelect_Click" OnClientClick="return checkInputText('tabNo1',0);" DisabledWhenSubmit="False" ShowID="01_04040000_004" onkeydown="setfocuschoice('txtReceiveNumber','btnDelete');"/>&nbsp;&nbsp;&nbsp;&nbsp; 
		                <cc1:CustButton ID="btnDelete" CssClass="smallButton" runat="server" Width="40px" OnClick="btnDelete_Click" OnClientClick="return checkInputText('tabNo1',0);" DisabledWhenSubmit="False" ShowID="01_04040000_005" onkeydown="setfocus('txtReceiveNumber');"/>
		            </td>
	            </tr>
	            <tr>
		            <td nowrap colspan="4" style="height:1px"></td>
	            </tr>
	        </table>
	        
	        <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
	            <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo2" style="">
	                <tr class="trOdd">
	                    <td style="width: 20%; " align="right">
	                        <cc1:CustLabel ID="lblBankCode" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04040000_006" StickHeight="False"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 30%; ">
	                        <cc1:CustLabel ID="lblBankCodeText" runat="server"  Width="50px"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 20%; " align="right">
	                        <cc1:CustLabel ID="lblBankCodeL" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04040000_007" StickHeight="False"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 30%; ">
	                        <cc1:CustLabel ID="lblBankCodeLText" runat="server"  Width="90px"></cc1:CustLabel>
	                    </td>
	                </tr>
	                <tr class="trEven">
	                    <td style="width: 20%; " align="right">
	                        <cc1:CustLabel ID="lblBankName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04040000_008" StickHeight="False"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 30%; ">
	                        <cc1:CustLabel ID="lblBankNameText" runat="server"  Width="200px"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 20%; " align="right">
	                        <cc1:CustLabel ID="lblBankAccNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04040000_009" StickHeight="False"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 30%; ">
	                        <cc1:CustLabel ID="lblBankAccNoText" runat="server"   Width="200px" ></cc1:CustLabel>
	                    </td>
	                </tr>
	                   <tr class="trOdd">
	                    <td style="width: 20%; " align="right">
	                        <cc1:CustLabel ID="lblBankPayWay" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04040000_010" StickHeight="False"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 30%; ">
	                        <cc1:CustLabel ID="lblBankPayWayText" runat="server"  Width="90px"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 20%; " align="right">
	                        <cc1:CustLabel ID="lblBankCusID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04040000_011" StickHeight="False"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 30%; ">
	                        <cc1:CustLabel ID="lblBankCusIDText" runat="server"  Width="150px"></cc1:CustLabel>
	                    </td>
	                </tr>
	                <tr class="trEven">
	                    <td style="width: 20%; " align="right">
	                        <cc1:CustLabel ID="lblBcycleCode" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04040000_012" StickHeight="False"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 30%; ">
	                        <cc1:CustLabel ID="lblBcycleCodeText" runat="server" Width="50px"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 20%; " align="right">
	                        <cc1:CustLabel ID="lblMobilePhone" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04040000_013" StickHeight="False"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 30%; ">
	                        <cc1:CustLabel ID="lblMobilePhoneText" runat="server"  Width="150px"></cc1:CustLabel>
	                    </td>
	                </tr>
	                <tr class="trOdd">
	                    <td style="width: 20%; " align="right">
	                        <cc1:CustLabel ID="lblEMail" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04040000_014" StickHeight="False"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 30%; ">
	                        <cc1:CustLabel ID="lblEMailText" runat="server"  Width="200px"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 20%; " align="right">
	                        <cc1:CustLabel ID="lblEBill" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04040000_015" StickHeight="False"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 30%; ">
	                        <cc1:CustLabel ID="lblEBillText" runat="server"  Width="90px"></cc1:CustLabel>
	                    </td>
	                </tr>
	                <tr class="trEven">
	                    <td style="width: 20%; " align="right">
	                        <cc1:CustLabel ID="lblBuildDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04040000_016" StickHeight="False"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 30%; ">
	                        <cc1:CustLabel ID="lblBuildDateText" runat="server"  Width="90px"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 20%; " align="right">
	                        <cc1:CustLabel ID="lblApplyType" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04040000_017" StickHeight="False"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 30%; ">
	                        <cc1:CustLabel ID="lblApplyTypeText" runat="server"  Width="50px"></cc1:CustLabel>
	                    </td>
	                </tr>
	                <tr class="trOdd">
	                    <td style="width: 20%; " align="right">
	                        <cc1:CustLabel ID="lblDealNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04040000_018" StickHeight="False"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 30%; ">
	                        <cc1:CustLabel ID="lblDealNoText" runat="server"  Width="90px"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 20%; " align="right">
	                        <cc1:CustLabel ID="lblRemark" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04040000_019" StickHeight="False"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 30%; ">
	                        <cc1:CustLabel ID="lblRemarkText" runat="server"  Width="50px"></cc1:CustLabel>
	                    </td>
	                </tr>
	                <tr class="trEven">
	                    <td style="width: 20%; " align="right">
	                        <cc1:CustLabel ID="lblRtnDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04040000_020" StickHeight="False"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 30%; ">
	                        <cc1:CustLabel ID="lblRtnDateText" runat="server"  Width="90px"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 20%; " align="right">
	                        <cc1:CustLabel ID="lblACHReturnCode" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04040000_021" StickHeight="False"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 30%; ">
	                        <cc1:CustLabel ID="lblACHReturnCodeText" runat="server"  Width="200px"></cc1:CustLabel>
	                    </td>
	                </tr>
	                <tr class="trOdd">
	                    <td style="width: 20%; " align="right">
	                        <cc1:CustLabel ID="lblCode" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04040000_022" StickHeight="False"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 30%; ">
	                        <cc1:CustLabel ID="lblCodeText" runat="server"  Width="90px"></cc1:CustLabel>
	                    </td>
	                    <td style="width: 20%; " align="right">
	                    </td>
	                    <td style="width: 30%; ">
	                    </td>
	                </tr>
	                <tr>
		                <td nowrap colspan="4" style="height:1px"></td>
	                </tr>
	                <tr class="itemTitle">
	                    <td nowrap colspan="4"></td>
	                </tr>
	            </table>
            </cc1:CustPanel>
</ContentTemplate>
</asp:UpdatePanel>
<cc1:CustButton id="btnHiden" onclick="btnHiden_Click" runat="server" CssClass= "btnHiden" DisabledWhenSubmit="False" ></cc1:CustButton>
<cc1:CustButton id="btnHiden1" onclick="btnHiden1_Click" runat="server" CssClass= "btnHiden" DisabledWhenSubmit="False" ></cc1:CustButton>
    </form>
</body>
</html>

