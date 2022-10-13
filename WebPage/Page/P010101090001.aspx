<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010101090001.aspx.cs" Inherits="P010101090001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <%-- 2020/11/19_Ares_Stanley-修正格式; 2020/12/11_Ares_Stanley-修正格式; 2021/10/08_Ares_Stanley-EMAIL、電子帳單反灰不能修改 --%>
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
      
           
            //*檢核輸入欄位【收件編號】是否為空 
            if(document.getElementById('txtReceiveNumber').value.Trim() == "")
            {

                document.getElementById('txtReceiveNumber').focus();
                alert('請輸入收件編號');
                setControlsDisabled('tabNo2');
                document.getElementById('lblUserNameText').innerText = "";
                
                return false;
            }
            
            if (checkDateSn(document.getElementById("txtReceiveNumber").value.Trim().substring(0,8))==-2)
            {
                alert("收件編號格式不對！");
                document.getElementById('txtReceiveNumber').focus();
                setControlsDisabled('tabNo2');
                document.getElementById('lblUserNameText').innerText = "";
                return false;
            }
            
            //*檢核輸入欄位【身分證號碼】是否為空
             if(document.getElementById('txtUserId').value.Trim()== "")
            {
                alert('請輸入身分證號碼');
                document.getElementById('txtUserId').focus();
                setControlsDisabled('tabNo2');
                document.getElementById('lblUserNameText').innerText = "";
                
                return false;
            }
            
            //*檢核收件編號是否輸入正確
            if(document.getElementById('txtReceiveNumber').value.Trim().length!=13)
            {
                alert('收件編號格式不對！');
                document.getElementById('txtReceiveNumber').focus();
                setControlsDisabled('tabNo2');
                document.getElementById('lblUserNameText').innerText = "";
               
                return false;
               
            }
            
            
         
            
             if(!checkInputType(id))
            {
                return false;
            } 
            

            
             //*提交按鈕
            if(intType == 1)
            {  
                 
                if((document.getElementById('txtAccID').value.Trim()!="") && (document.getElementById('txtUserId').value.Trim().toUpperCase()!=document.getElementById('txtAccID').value.Trim().toUpperCase()))
                {
                   if(!confirm('身分證號碼<>帳戶ID，是否要繼續？'))
                   {
                        document.getElementById('txtAccID').focus();
                        ClientMsgShow("身分證號碼<>帳戶ID");
                        return false;
                   }
                }
                

                
                //*扣繳帳號(銀行3碼)不為空且不為“042”和“701”，則輸入錯誤
               if((document.getElementById('txtAccNoBank').value.Trim()== "042" || document.getElementById('txtAccNoBank').value.Trim()== "701" ))
               {
                   alert('代碼不能輸入[701] 或 [042]');
                   document.getElementById('txtAccNoBank').focus();
                   return false;
               }
              
               

               
               //*E-MAIL欄位必須要有[@]
               if(document.getElementById('txtEmail').value.Trim()!= "" &&   document.getElementById('txtEmail').value.Trim().indexOf('@')<0) 
               {
                   alert('E-MAIL欄位必須要有[@]');
                   ClientMsgShow("E-MAIL欄位必須要有[@]");
                   document.getElementById('txtEmail').focus();
                   return false;
               }
               
               //*檢核扣繳帳號輸入是否符合規則
               if(document.getElementById('txtAccNoBank').value.Trim()== "" &&  document.getElementById('txtAccNo').value.Trim()!= "" && document.getElementById('txtAccID').value.Trim()!= "")
               {
                   alert('扣繳銀行不可為空白');
                   document.getElementById('txtAccNoBank').focus();
                   return false;
               }
               if(document.getElementById('txtAccNoBank').value.Trim()!= "" &&  document.getElementById('txtAccNo').value.Trim()== "" && document.getElementById('txtAccID').value.Trim()!= "")
               {
                   alert('扣繳帳號不可為空白');
                   document.getElementById('txtAccNo').focus();
                   return false;
               }
               if(document.getElementById('txtAccNoBank').value.Trim()!= "" &&  document.getElementById('txtAccNo').value.Trim()!= "" && document.getElementById('txtAccID').value.Trim()== "")
               {
                   alert('帳號ID不可為空白');
                   document.getElementById('txtAccID').focus();
                   return false;
               }
               

                if(!confirm('請確認是否要提交資料'))
                {
                    return false;
                }
                return true;
            }
            else
            {
                //20160606 (U) by Tank
                var ID = $('#txtUserId').val();                
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    async: false,
                    url: "P010101070001.aspx/funGetDataCnt",
                    data: "{'KeyValue':'" + ID + "'}",
                    dataType: "json",
                    success: function (data) 
                    {
                        if (data > 0)
                        {
                            if(!confirm('近60天內已鍵終止件，是否繼續KEYIN?？'))
                            {
                                return false;
                            }
                            else 
                            {
                                $('#btnSelect2').click();
                                return true;
                            }
                        }
                        else 
                        {
                            $('#btnSelect2').click();
                            return true;
                        }
                    },
                    error: function (result) 
                    {
                        alert("funGetDataCnt Error");
                        return false;
                    }                  
                });
            return false;
            }
            
        }
        
          function ChangeEnableR()
        {
        
   
        
           if (document.getElementById("txtReceiveNumber").value.toUpperCase().Trim()!=document.getElementById("txtReceiveNumberH").value.toUpperCase().Trim())
           {
               setControlsDisabled('tabNo2');
              document.getElementById('lblUserNameText').innerText = "";
               
            
           }
         
            document.getElementById("txtReceiveNumberH").value=document.getElementById("txtReceiveNumber").value;
        }
        
        function ChangeEnableI()
        {
        
   
        
           if (document.getElementById("txtUserId").value.toUpperCase().Trim()!=document.getElementById("txtUserIdH").value.toUpperCase().Trim())
           {
               setControlsDisabled('tabNo2');
              document.getElementById('lblUserNameText').innerText = "";
             
            
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
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <cust:image runat="server" ID="image1"/>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                        <tr class="itemTitle">
                            <td colspan="4">
                                <li>
                                    <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" Width="200px" IsColon="False" ShowID="01_01010900_001"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblReceiveNumber" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01010900_002"
                                    StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 85%" colspan="3">
                                <cc1:CustTextBox ID="txtReceiveNumber" runat="server" MaxLength="13" checktype="num"
                                    onkeyup="ChangeEnableR();" onkeydown="keypressstop();" BoxName="收件編號"></cc1:CustTextBox><cc1:CustTextBox ID="txtReceiveNumberH"
                                        runat="server" MaxLength="13" checktype="num" CssClass="btnHiden" Text=""></cc1:CustTextBox></td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblUserId" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010900_003" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtUserId" runat="server" MaxLength="20" checktype="ID" onkeydown="entersubmit('btnSelect');"
                                    onkeyup="ChangeEnableI();" BoxName="身分證號碼"></cc1:CustTextBox><cc1:CustTextBox ID="txtUserIdH" runat="server"
                                        MaxLength="20" checktype="numandletter" CssClass="btnHiden" Text=""></cc1:CustTextBox>&nbsp;&nbsp;&nbsp;&nbsp;
                                <cc1:CustButton ID="btnSelect" CssClass="smallButton" runat="server" Width="40px"
                                    OnClientClick="return checkInputText('tabNo1',0);"
                                    DisabledWhenSubmit="False" onkeydown="setfocuschoice('txtReceiveNumber','txtAccNoBank');" />
                                <%--20160606 (U) by Tank--%>
                                <asp:Button runat="server" ID="btnSelect2" OnClick="btnSelect_Click" style="display:none"/>
                            </td>
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblUserName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010900_004" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 35%">
                                <cc1:CustLabel ID="lblUserNameText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table2">
                        <tr>
                            <td align="right" colspan="4" style="height: 1px">
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo2" style="">
                        <tr class="itemTitle">
                            <td colspan="2">
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblAccNoBank" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010900_006" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 85%">
                                <div style="position: relative">
                                    <cc1:CustDropDownList ID="dropAccNo" kind="select" runat="server" Style="left: 0px;
                                        top: 0px; clip: rect(0px auto auto 130px); position: absolute; width: 150px;"
                                        onclick="simOptionClick4IE('txtAccNoBank');">
                                    </cc1:CustDropDownList>
                                    <cc1:CustTextBox ID="txtAccNoBank" runat="server" MaxLength="30" checklength="3"
                                        onkeydown="entersubmit('btnSubmit');" Style="left: 0px; top: 0px; position: relative;
                                        width: 125px; height: 11px;" BoxName="扣繳帳號" onfocus="allselect(this);"></cc1:CustTextBox>
                                </div>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblAccNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010900_007" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 85%">
                                <cc1:CustTextBox ID="txtAccNo" runat="server" Width="190px" checktype="num" MaxLength="26"
                                    onkeydown="entersubmit('btnSubmit');" BoxName="銀行帳號"></cc1:CustTextBox></td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lnlPayWay" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010900_008" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 85%">
                                <cc1:CustTextBox ID="txtPayWay" runat="server" Width="120px" checktype="num" MaxLength="1"
                                    onkeydown="entersubmit('btnSubmit');" BoxName="扣繳方式"></cc1:CustTextBox></td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblAccID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010900_009" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 85%">
                                <cc1:CustTextBox ID="txtAccID" runat="server" Width="130px" checktype="numandletter"
                                    MaxLength="18" onkeydown="keystoke('btnSubmit','txtBcycleCode');" BoxName="帳戶ID"></cc1:CustTextBox></td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblBcycleCode" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010900_010" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 85%; float:left; ">
                                <cc1:CustTextBox ID="txtBcycleCodeText" runat="server" Width="120px" style="height:11px; margin-top:2px; position:absolute;" checktype="num"
                                    MaxLength="2" onkeypress="keypress('btnSubmit',false);" onkeydown="setfocuschoicedrop('txtBcycleCode');"
                                    Enabled="false" BoxName="帳單週期"></cc1:CustTextBox>
                                <div style="position: absolute; margin-left:130px;">
                                    <cc1:CustDropDownList ID="dropBcycleCode" kind="select" runat="server" Style="left: 0px;
                                        top: 2px; clip: rect(0px auto auto 130px); position: absolute; width: 150px;"
                                        onclick="simOptionClick4IE('txtBcycleCode');" Enabled="false">
                                    </cc1:CustDropDownList>
                                    <cc1:CustTextBox ID="txtBcycleCode" runat="server" MaxLength="2" checktype="num"
                                        onkeydown="entersubmit('btnSubmit');" Style="left: 0px; top: 2px; position: absolute;
                                        width: 125px; height: 11px;" BoxName="帳單週期" onfocus="allselect(this);" Enabled="false"></cc1:CustTextBox>
                                </div>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblMobilePhone" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010900_011" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 85%">
                                <cc1:CustTextBox ID="txtMobilePhone" runat="server" Width="120px" checktype="num"
                                    MaxLength="10" onkeydown="entersubmit('btnSubmit');" BoxName="行動電話"></cc1:CustTextBox></td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%" height="27" align="right">
                                <cc1:CustLabel ID="lblEmail" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010900_012" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 85%">
                                <cc1:CustTextBox ID="txtEmail" runat="server" Width="360px" checktype="email" MaxLength="50"
                                    onkeydown="entersubmit('btnSubmit');" BoxName="E-MAIL" ReadOnly="true" BackColor="LightGray"></cc1:CustTextBox></td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblEBill" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010900_013" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 85%">
                                <cc1:CustTextBox ID="txtEBill" runat="server" Width="70px" checktype="num" MaxLength="1"
                                    onkeydown="entersubmit('btnSubmit');" BoxName="電子帳單" ReadOnly="true" BackColor="LightGray"></cc1:CustTextBox>
                                &nbsp;
                            </td>
                        </tr>
                        
                         <tr class="trOdd">
                          <td style="width: 15%; height: 25px;" align="right">
                                <cc1:CustLabel ID="lblPopulNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010900_015" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%; height: 25px;">
                                <cc1:CustTextBox ID="txtPopulNo" runat="server" Width="190px" checktype="num" MaxLength="5"
                                    onkeydown="entersubmit('btnSubmit');" BoxName="推廣代號" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                        
                        </tr>
                        <tr class="trEven">
                        <td style="width: 15%; height: 25px;" align="right">
                                <cc1:CustLabel ID="lblPopulEmpNO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010900_016" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%; height: 25px;">
                                <cc1:CustTextBox ID="txtPopulEmpNO" runat="server" Width="190px" checktype="num" MaxLength="8"
                                    onkeydown="entersubmit('btnSubmit');" BoxName="推廣員編" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            
                        </tr>
                        <tr class="trOdd">
                        <td style="width: 15%; height: 25px;" align="right">
                                <cc1:CustLabel ID="lbCaseClass" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010900_017" StickHeight="False"></cc1:CustLabel>
                            </td>
                        <td style="width: 35%; height: 25px;">
                                <div style="position: relative">
                                    <cc1:CustDropDownList ID="dropCaseClass" kind="select" runat="server" Style="left: 0px;
                                        top: 0px; clip: rect(0px auto auto 130px); position: absolute; width: 150px;"
                                        onclick="simOptionClick4IE('txtCaseClass');" Enabled="false" >
                                    </cc1:CustDropDownList>
                                    <cc1:CustTextBox ID="txtCaseClass" runat="server" MaxLength="50" checktype=""
                                         onkeydown="entersubmit('btnSubmit');"
                                        Style="left: 0px; top: 0px; position: relative; width: 125px; height: 11px;"
                                        AutoPostBack="False" BoxName="案件類別" onfocus="allselect(this);" Enabled="false"></cc1:CustTextBox>
                                    
                                    <cc1:CustTextBox ID="txtbCaseClassH" runat="server" MaxLength="30" CssClass="btnHiden"
                                        Text=""></cc1:CustTextBox>
                                </div>
                            </td>
                        </tr>
                        <tr class="itemTitle">
                            <td colspan="4" align="center">
                                <cc1:CustButton ID="btnSubmit" CssClass="smallButton" Width="40px" runat="server"
                                    OnClientClick="return checkInputText('tabNo2',1);" onkeydown="setfocus('txtReceiveNumber');"
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
                document.getElementById("dropBcycleCode").style.width = 165;
				document.getElementById("dropAccNo").style.width = 165;
				document.getElementById("dropCaseClass").style.width = 165;
              }
		}
		fixwidth();
		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(fixwidth);
</script>
</html>
