<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010101050001.aspx.cs" Inherits="P010101050001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <%-- 2020/11/19_Ares_Stanley-修正格式; 2020/12/11_Ares_Stanley-修正格式; 2021/02/08_Ares_Stanley-調整下拉選單; 2021/03/04_Ares_Stanley-調整版面; 2021/09/01_Ares_Stanley-調整TAB順序; 2021/09/24_Ares_Stanley-調整查詢按enter沒有反應; 2021/10/08_Ares_Stanley-EMAIL、電子帳單反灰不能修改 --%>
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript">
                    
    var flagChange=0;
    
        function checkInputText(id,intType)
        {
            //*檢核輸入欄位【收件編號】是否為空 
            if(document.getElementById('txtReceiveNumber').value.Trim() == "")
            {
                alert('請輸入收件編號');
                document.getElementById('txtReceiveNumber').focus();
                setControlsDisabled('tabNo2');
                document.getElementById('lblUserNameText').innerText = "";
                document.getElementById('lblLongBankNo').innerText = "";
                document.getElementById('lblLongBankName').innerText = "";
                document.getElementById('lbBuildDateText').innerText = "";
                document.getElementById('lbNoteText').innerText = "";
                document.getElementById('lbReturnDate').innerText = "";
                document.getElementById('lbAchText').innerText = "";
                document.getElementById('lbApplyCodeText').innerText = "";
                
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
             
                 if(document.getElementById("txtApplyType").value.toUpperCase().Trim()=="A")
                 {
                    document.getElementById("txtTranCode").value="851";
                 }
                 

                
                 
                //*扣繳帳號(銀行3碼)不為空且不為“042”和“701”，則輸入錯誤

               if((document.getElementById('txtAccNoBank').value.Trim()== "042" || document.getElementById('txtAccNoBank').value.Trim()== "701" || document.getElementById('txtAccNoBank').value.Trim()==""))
               {
                   alert('銀行代號錯誤');
                   document.getElementById('txtAccNoBank').focus();
                   document.getElementById('lblLongBankNo').innerText = "";
                   document.getElementById('lblLongBankName').innerText = "";
                   return false;
               }
              
              //*輸入的帳戶ID>10碼~~則出錯誤訊息"帳戶ID>10碼"
               if(document.getElementById('txtAccID').value.Trim().length>10)
               {
                   alert('輸入錯誤,帳戶ID>10碼');
                   return false;
               }
                     
               if(document.getElementById('txtAccID').value.Trim()!= "" && ( document.getElementById('txtUserId').value.Trim().toUpperCase()!=document.getElementById('txtAccID').value.Trim().toUpperCase()) ) 
               {
                   if(!confirm('身分證號碼<>帳戶ID，是否要繼續？'))
                    {
                         
                         document.getElementById('txtAccID').focus();
                         return false;
                    }
               }
               
               //*E-MAIL欄位必須要有[@]
               if(document.getElementById('txtEmail').value.Trim()!= "" &&   document.getElementById('txtEmail').value.Trim().indexOf('@')<0) 
               {
                   alert('E-MAIL欄位必須要有[@]');
                   document.getElementById('txtEmail').focus();
                   return false;
               }
            
               if(document.getElementById('txtApplyType').value.Trim()== "" ) 
               {

                     alert('申請類別不能為空');
                     document.getElementById('txtApplyType').focus();
                     return false;

               }
               
               if(!(document.getElementById('txtApplyType').value.toUpperCase().Trim()== "A" || document.getElementById('txtApplyType').value.toUpperCase().Trim()== "D" || document.getElementById('txtApplyType').value.toUpperCase().Trim()== "O" )) 
               {

                     alert('申請類別不正確');
                     document.getElementById('txtApplyType').focus();
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
  
                                 
                //*顯示確認提示框

                if(!confirm('確定是否要異動資料？'))
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
                    url: "P010101050001.aspx/funGetDataCnt",
                    data: "{'KeyValue':'" + ID + "'}",
                    dataType: "json",
                    success: function (data) 
                    {
                        if (data > 0)
                        {
                            if(!confirm('近60天內自扣已建檔過，是否繼續KeyIn？'))
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
              document.getElementById('lblLongBankNo').innerText = "";
              document.getElementById('lblLongBankName').innerText = "";
              document.getElementById('lbNoteText').innerText = "";
              document.getElementById('lbReturnDate').innerText = "";
              document.getElementById('lbAchText').innerText = "";
              document.getElementById('lbApplyCodeText').innerText = "";
              document.getElementById('lbBuildDateText').innerText = "";
              
            
           }
         
            document.getElementById("txtReceiveNumberH").value=document.getElementById("txtReceiveNumber").value;
        }
        
        function ChangeEnableI()
        {
        
   
        
           if (document.getElementById("txtUserId").value.toUpperCase().Trim()!=document.getElementById("txtUserIdH").value.toUpperCase().Trim())
           {
             

              setControlsDisabled('tabNo2');
              document.getElementById('lblUserNameText').innerText = "";          
              document.getElementById('lblLongBankNo').innerText = "";            
              document.getElementById('lblLongBankName').innerText = "";            
              document.getElementById('lbNoteText').innerText = "";          
              document.getElementById('lbReturnDate').innerText = "";             
              document.getElementById('lbAchText').innerText = "";             
              document.getElementById('lbApplyCodeText').innerText  = "";              
              document.getElementById('lbBuildDateText').innerText  = "";

              
            
           }
         
            document.getElementById("txtUserIdH").value=document.getElementById("txtUserId").value;
        }
        
       
        //*onkeypress事件      
        function   keypressbank(id) 
        {  
         
           
            //*按enter提交事件
            
           
            flagChange=0;
            if (event.keyCode==13 )
            {
                 
                  //event.keyCode=9;
                  flagChange=1;
                  document.getElementById("txtAccNoBankH").value=document.getElementById("txtAccNoBank").value;
                  event.returnValue=false;
                  document.getElementById(id).click(); 
                    
            } 
            flagChange=0;
      } 
      
       
      
       function   keypressbankC(id) 
        {  
         
     
             document.getElementById("txtHiden").value="";
            
             fid= document.activeElement.id;
       
             
            
             try
             {
                 if( (document.getElementById("txtAccNoBankH").value.Trim()!=document.getElementById("txtAccNoBank").value.Trim() ) &&  (document.getElementById(fid).tagName=="INPUT" &&  !(fid=="btnCheck" || fid=="btnSubmit" ))) 
                 {
                   document.getElementById("txtHiden").value=fid;
                   document.getElementById("txtAccNoBankH").value=document.getElementById("txtAccNoBank").value;
                   document.getElementById(id).click(); 
                   
                 }
                 else if (fid=="btnSubmit" )
                 {
                 
                   document.getElementById("txtHiden").value=fid;
                   document.getElementById("txtAccNoBankH").value=document.getElementById("txtAccNoBank").value;
                   if(checkInputText('tabNo2',1)==false)
                   {
                     return false;
                   }
                   
                   
                   
                 }
                 else if (fid=="btnCheck" )
                 {
                   
                 }
             }
             catch(e)
             {
             
             }
             finally
             {

             }
             
           
       }
       
        function btnBankCheckClick()
        {
            document.getElementById("txtAccNoBankH").value = document.getElementById("txtAccNoBank").value;
        }

      
      function bankclick()
       {
          simOptionClick4IE('txtAccNoBank');
          document.getElementById("txtAccNoBankH").value=document.getElementById("txtAccNoBank").value;
         
          
       }  
       
       function applyclick()
       {
          simOptionClick4IES('txtApplyType');
          document.getElementById("txtApplyTypeH").value=document.getElementById("txtApplyType").value;
          
       } 
          
       

       
      function simOptionClick4IES(id)//*傳入id為TexyBox ID
      {     
         var evt=window.event   ;      
         var selectObj=evt?evt.srcElement:null;     
     
          // IE Only     
          if (evt && selectObj &&   evt.offsetY && evt.button!=2     
     
             && (evt.offsetY > selectObj.offsetHeight || evt.offsetY<0 ) ) 
             {                      
                //2021/04/01_Ares_Stanley-下拉選單資料取得失敗
                  //* 记录原先的选中项     
                  var option;
                  if (selectObj.index != undefined) {
                      option = selectObj.parentNode[selectObj.index];
                  }
                  else {
                      option = selectObj.options[selectObj.selectedIndex];
                  }  
                  document.getElementById(id).value=option.innerText; 
                  if(option.innerText=="A")
                  {
                      
                      document.getElementById("txtTranCode").value="851";                      
                      
                  }
                  
                  if(document.getElementById(id).value.Trim()!="")
                  {
                     document.getElementById(id).focus();
                   
                  }
     
           }     
      }
         
        //*Tab鍵設置焦點

        function setfocus(id) {

            if (event.keyCode == 9) {
                event.returnValue = false;
                setTimeout(
                    function () {
                        var el = document.getElementById(id);
                        (el != null) ? el.focus() : setTimeout(arguments.callee, 10);

                    }
                    , 10);
                document.getElementById(id).focus();
            }
        }
    </script>

    <style type="text/css">
   .btnHiden
    {display:none; }
    </style>
</head>
<body class="workingArea">
    <form id="form1" runat="server" defaultbutton="btnEventPrevent">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <cust:image runat="server" ID="image1" />
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                        <tr class="itemTitle">
                            <td colspan="4">
                                <li>
                                    <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" Width="200px" IsColon="False" ShowID="01_01010500_001"></cc1:CustLabel></li></td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblReceiveNumber" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01010500_002"
                                    StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 85%" colspan="3">
                                <cc1:CustTextBox ID="txtReceiveNumber" runat="server" MaxLength="13" checktype="num"
                                    onkeyup="ChangeEnableR();" onkeydown="keypressstop();" BoxName="收件編號"></cc1:CustTextBox><cc1:CustTextBox
                                        ID="txtReceiveNumberH" runat="server" MaxLength="13" checktype="num" CssClass="btnHiden"
                                        Text=""></cc1:CustTextBox></td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblUserId" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_003" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtUserId" runat="server" MaxLength="12" checktype="ID" onkeydown="entersubmit('btnSelect');"
                                    onkeyup="ChangeEnableI();" BoxName="身分證號碼"></cc1:CustTextBox><cc1:CustTextBox ID="txtUserIdH"
                                        runat="server" MaxLength="12" checktype="numandletter" CssClass="btnHiden" Text=""></cc1:CustTextBox>&nbsp;&nbsp;&nbsp;&nbsp;
                                <cc1:CustButton ID="btnSelect" CssClass="smallButton" runat="server" Width="40px"
                                    OnClientClick="return checkInputText('tabNo1',0);"
                                    DisabledWhenSubmit="False" onkeydown="setfocuschoice('txtReceiveNumber','txtAccNoBank');" onkeypress="entersubmit('btnSelect');"
                                    ShowID="01_01010500_022" />
                                <%--20160606 (U) by Tank--%>
                                <asp:Button runat="server" ID="btnSelect2" OnClick="btnSelect_Click" style="display:none"/>
                            </td>
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblUserName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_004" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 35%">
                                <cc1:CustLabel ID="lblUserNameText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table2">
                        <tr>
                            <td align="right" style="height: 1px">
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo2" style="">
                        <tr class="itemTitle">
                            <td colspan="4">
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%; height: 25px;" align="right">
                                <cc1:CustLabel ID="lblAccNoBank" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_005" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 35%; height: 25px;">
                                <div style="position: relative">
                                    <cc1:CustDropDownList ID="dropAccNo" kind="select" runat="server" Style="left: 0px;
                                        top: 0px; clip: rect(0px auto auto 130px); position: relative; width: 150px;"
                                        onclick="bankclick();" AutoPostBack="True" OnTextChanged="dropAccNo_TextChanged">
                                    </cc1:CustDropDownList>
                                    <cc1:CustTextBox ID="txtAccNoBank" runat="server" MaxLength="30" checklength="3"
                                        onkeydown="keypressbank('btnHiden');" Style="left: 0px; top: 2px; position: absolute;
                                        width: 125px; height: 11px;" AutoPostBack="False" BoxName="銀行代號" onfocus="allselect(this);" TabIndex="1"></cc1:CustTextBox>
                                    <cc1:CustButton ID="btnBankCheck" CssClass="smallButton" runat="server" Style="left: 0px;
                                        top: 0px; position: relative; width: 90px;" OnClick="btnBankCheck_Click" onkeydown="setfocuschoice('txtAccNo', 'txtAccNo'); " OnClientClick="btnBankCheckClick()"
                                        DisabledWhenSubmit="False" ShowID="01_01010600_026" />
                                    <cc1:CustTextBox ID="txtAccNoBankH" runat="server" MaxLength="30" CssClass="btnHiden" Width="125px" Height="11px"
                                        Text=""></cc1:CustTextBox>
                                </div>
                            </td>
                            <td style="width: 15%; height: 25px;" align="right">
                                <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_008" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 35%; height: 25px;">
                                <cc1:CustTextBox ID="txtAccNo" runat="server" Width="190px" checktype="num" MaxLength="26"
                                    onkeydown="entersubmit('btnSubmit');" BoxName="銀行帳號" onfocus="allselect(this);" TabIndex="2"></cc1:CustTextBox></td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_006" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 85%" colspan="3">
                                <cc1:CustLabel ID="lblLongBankNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%; height: 25px;" align="right">
                                <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_007" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 85%; height: 25px;" colspan="3">
                                <cc1:CustLabel ID="lblLongBankName" runat="server" Width="190px" MaxLength="26"></cc1:CustLabel></td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lbPayWay" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_009" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtPayWay" runat="server" Width="120px" checktype="num" MaxLength="1"
                                    onkeydown="entersubmit('btnSubmit');" BoxName="扣繳方式" TabIndex="3"></cc1:CustTextBox></td>
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblAccID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_024" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtAccID" runat="server" Width="130px" checktype="numandletter"
                                    MaxLength="18" onkeydown="setfocus('txtBcycleCode'); keystoke('btnSubmit','txtBcycleCode');" BoxName="帳戶ID" TabIndex="4"></cc1:CustTextBox></td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblBcycleCode" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_011" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 35%; float:left;" valign="middle">
                                <cc1:CustTextBox ID="txtBcycleCodeText" runat="server" Width="120px" checktype="num"
                                    MaxLength="2" onkeypress="keypress('btnSubmit',false);" onkeydown="setfocuschoicedrop('txtBcycleCode');"
                                    Enabled="false" BoxName="帳單週期" style="height:11px; margin-top:2px; position:absolute;" ></cc1:CustTextBox>
                                <div style="position: absolute; vertical-align: middle; margin-left:130px;">
                                    <cc1:CustDropDownList ID="dropBcycleCode" kind="select" runat="server" Style="left: 0px;
                                        top: 2px; clip: rect(0px auto auto 130px); position: absolute; width: 150px;
                                        vertical-align: middle;" onclick="simOptionClick4IE('txtBcycleCode');">
                                    </cc1:CustDropDownList>
                                    <cc1:CustTextBox ID="txtBcycleCode" runat="server" MaxLength="2" checktype="num"
                                        onkeydown="entersubmit('btnSubmit');" Style="left: 0px; top: 2px; position: absolute;
                                        width: 125px; height: 11px; vertical-align: middle;" BoxName="帳單週期" onfocus="allselect(this);" TabIndex="5"></cc1:CustTextBox>
                                  </div>
                            </td>
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_012" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtMobilePhone" runat="server" Width="190px" checktype="num"
                                    MaxLength="10" onkeydown="entersubmit('btnSubmit');" BoxName="行動電話" TabIndex="6"></cc1:CustTextBox></td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%" height="27" align="right">
                                <cc1:CustLabel ID="lblEmail" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_013" StickHeight="False"></cc1:CustLabel></td>
                            <td width="35%">
                                <cc1:CustTextBox ID="txtEmail" runat="server" Width="335px" checktype="email" MaxLength="50"
                                    onkeydown="entersubmit('btnSubmit');" BoxName="E-MAIL" TabIndex="7" ReadOnly="true" BackColor="LightGray"></cc1:CustTextBox></td>
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblEBill" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_014" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtEBill" runat="server" Width="70px" checktype="num" MaxLength="1"
                                    onkeydown="setfocus('txtApplyType'); keystoke('btnSubmit','txtApplyType');" BoxName="電子帳單" TabIndex="8" ReadOnly="true" BackColor="LightGray"></cc1:CustTextBox>
                                &nbsp;
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_016" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 35%">
                                <div style="position: relative">
                                    <cc1:CustDropDownList ID="dropApplyType" kind="select" runat="server" Style="left: 0px;
                                        top: 0px; clip: rect(0px auto auto 130px); position: absolute; width: 150px;"
                                        onclick="applyclick();" AutoPostBack="True" OnTextChanged="dropApplyType_TextChanged">
                                    </cc1:CustDropDownList>
                                    <cc1:CustTextBox ID="txtApplyType" runat="server" MaxLength="1" checktype="numandletter"
                                        Style="left: -1px; top: 0px; position: relative; width: 125px; height: 11px;"
                                        onkeydown="setfocus('txtTranCode'); keystoke('btnSubmit','txtTranCode');" BoxName="申請類別" onfocus="allselect(this);" TabIndex="9"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtApplyTypeH" runat="server" MaxLength="1" CssClass="btnHiden"
                                        Text=""></cc1:CustTextBox>
                                </div>
                            </td>
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_017" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 35%">
                                <div style="position: relative">
                                    <cc1:CustDropDownList ID="dropTranCode" kind="select" runat="server" Style="left: 0px;
                                        top: 0px; clip: rect(0px auto auto 130px); position: absolute; width: 150px;"
                                        onclick="simOptionClick4IE('txtTranCode');">
                                    </cc1:CustDropDownList>
                                    <cc1:CustTextBox ID="txtTranCode" runat="server" MaxLength="3" checktype="num" onkeydown="entersubmit('btnSubmit');"
                                        Style="left: 0px; top: 0px; position: relative; width: 125px; height: 11px;"
                                        BoxName="交易代號" onfocus="allselect(this);" TabIndex="10"></cc1:CustTextBox>
                                    <cc1:CustButton ID="btnCheck" CssClass="smallButton" runat="server" Style="left: 160px;
                                        top: 0px; position: absolute; width: 80px;" OnClick="btnCheck_Click" DisabledWhenSubmit="False"
                                        ShowID="01_01010500_025" />
                                </div>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_015" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 35%">
                                <cc1:CustLabel ID="lbBuildDateText" runat="server" Width="70px" checktype="num" MaxLength="8"></cc1:CustLabel></td>
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="CustLabel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_018" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 35%">
                                <cc1:CustLabel ID="lbNoteText" runat="server" Width="70px"></cc1:CustLabel>
                                &nbsp;
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="CustLabel9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_019" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 35%">
                                <cc1:CustLabel ID="lbReturnDate" runat="server" Width="70px"></cc1:CustLabel></td>
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="CustLabel10" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_020" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 35%">
                                <cc1:CustLabel ID="lbAchText" runat="server" Width="150px"></cc1:CustLabel>
                                &nbsp;
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="CustLabel11" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_021" StickHeight="False"></cc1:CustLabel></td>
                                <td colspan="3">
                                <cc1:CustLabel ID="lbApplyCodeText" runat="server" Width="70px"></cc1:CustLabel>
                                &nbsp;
                            </td>
                            </tr>
                         <tr class="trOdd">
                            <td style="width: 15%; height: 25px;" align="right">
                                <cc1:CustLabel ID="lblPopulNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_027" StickHeight="False"></cc1:CustLabel>
                            </td>
                             <td colspan="3">
                                <cc1:CustTextBox ID="txtPopulNo" runat="server" Width="190px" checktype="num" MaxLength="5"
                                    onkeydown="entersubmit('btnSubmit');" BoxName="推廣代號" onfocus="allselect(this);" TabIndex="11"></cc1:CustTextBox>
                            </td>
                       <tr class="trEven">

                            <td style="width: 15%; height: 25px;" align="right">
                                <cc1:CustLabel ID="lblPopulEmpNO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_028" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3">
                                <cc1:CustTextBox ID="txtPopulEmpNO" runat="server" Width="190px" checktype="num"
                                    MaxLength="8" onkeydown="entersubmit('btnSubmit');" BoxName="推廣員編" onfocus="allselect(this);" TabIndex="12"></cc1:CustTextBox>
                            </td>
                            </tr>
                                          <tr class="trOdd">
                            <td style="width: 15%; height: 25px;" align="right">
                                <cc1:CustLabel ID="lbCaseClass" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010500_029" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3">
                                <div style="position: relative">
                                    <cc1:CustDropDownList ID="dropCaseClass" kind="select" runat="server" Style="left: 0px;
                                        top: 0px; clip: rect(0px auto auto 130px); position: absolute; width: 205px;"
                                        onclick="simOptionClick4IE('txtCaseClass');" Enabled="false">
                                    </cc1:CustDropDownList>
                                    <cc1:CustTextBox ID="txtCaseClass" runat="server" MaxLength="50" checktype="" onkeydown="entersubmit('btnSubmit');"
                                        Style="left: 0px; top: 0px; position: relative; width: 180px; height: 11px;"
                                        AutoPostBack="False" BoxName="案件類別" onfocus="allselect(this);" Enabled="false"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtbCaseClassH" runat="server" MaxLength="30" CssClass="btnHiden"
                                        Text=""></cc1:CustTextBox>
                                </div>
                            </td>
                        </tr>
                        <tr class="itemTitle">
                            <td colspan="4" align="center">
                                <asp:Button runat="server" ID="btnEventPrevent" OnClientClick="return false" style="display:none" UseSubmitBehavior="true"/>
                                <cc1:CustButton ID="btnSubmit" CssClass="smallButton" Width="40px" runat="server"
                                    OnClientClick="return checkInputText('tabNo2',1);" onkeydown="setfocus('txtReceiveNumber');"
                                    OnClick="btnSubmit_Click" DisabledWhenSubmit="False" ShowID="01_01010500_023" />
                                <cc1:CustTextBox ID="txtHiden" runat="server" MaxLength="100" CssClass="btnHiden"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtHidenA" runat="server" MaxLength="100" CssClass="btnHiden"></cc1:CustTextBox>
                                <cc1:CustButton ID="btnHidenC" OnClick="btnHidenC_Click" runat="server" CssClass="btnHiden"
                                    DisabledWhenSubmit="False"></cc1:CustButton>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
            <br />
            <cc1:CustButton ID="btnHiden" OnClick="btnHiden_Click" runat="server" CssClass="btnHiden"
                DisabledWhenSubmit="False"></cc1:CustButton>
            <cc1:CustButton ID="btnHidenD" OnClick="btnHidenD_Click" runat="server" CssClass="btnHiden"
                DisabledWhenSubmit="False"></cc1:CustButton>
        </div>
    </form>
</body>

<script type="text/javascript" language="javascript">
//add by Mars 解決IE6以上瀏覽時下拉選單長度不夠被TEXTBOX擋住 2012-12-04
		var isIE6 = navigator.userAgent.search("MSIE 6") > -1;
		function fixwidth()		
		{
              if(!isIE6){
                document.getElementById("dropAccNo").style.width = 165;
				document.getElementById("btnBankCheck").style.left = 175;				
				document.getElementById("dropBcycleCode").style.width = 165;
				document.getElementById("dropApplyType").style.width = 165;
				document.getElementById("dropTranCode").style.width = 165;
				document.getElementById("btnCheck").style.left = 175;
				document.getElementById("dropCaseClass").style.width = 165;
              }
		}
		fixwidth();
		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(fixwidth);
</script>

</html>
