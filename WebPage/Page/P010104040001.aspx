<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010104040001.aspx.cs" Inherits="P010104040001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%-- 20201231_Ares_Stanley-調整排版, 20210201_Ares_Stanley-調整版面; 20210329_Ares_Stanley-調整半形轉全形失效; 20210331_Ares_Stanley-調整Disabled input背景色; 20210408_Ares_Stanley-調整半形轉全形失效; 20210409_Ares_Stanley-修正onblur alert死循環; 20210415_Ares_Stanley-調整全半形轉換 --%>
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
            //*新增按鈕檢核
            if(intType != 0)
            {
                //*檢核欄位輸入規則
                if(!checkInputType(id))
                {
                      return false;
                }
                 
                if(intType == 1)//*新增
                {       
                    if(document.getElementById('radRegName').checked == false && document.getElementById('radBusinessName').checked == false && document.getElementById('radMerchantName').checked == false)
                    {
                        alert('請選擇一項帳列名稱');
                        return false;
                    }
                    
                                    
                   //*帳單地址一不能輸入空
                   if(document.getElementById('txtAddress1').value.Trim() == "")
                  {
                       alert('帳單地址一不能輸入空! ');
                       document.getElementById('txtAddress1').focus(); 
                       return false;
                  }
                  
                   //*帳單地址二不能輸入空
                   if(document.getElementById('txtAddress2').value.Trim() == "")
                  {
                       alert('帳單地址二不能輸入空! ');
                       document.getElementById('txtAddress2').focus(); 
                       return false;
                  }
                  
                   //*推廣代碼不能輸入空
                   if(document.getElementById('txtAgentBank').value.Trim() == "")
                  {
                       alert('推廣代碼不能輸入空! ');
                       document.getElementById('txtAgentBank').focus(); 
                       return false;
                  }
                  
             
                   //*TYPE不能輸入空
                   if(document.getElementById('txtType').value.Trim() == "")
                  {
                       alert('TYPE不能輸入空! ');
                       document.getElementById('txtType').focus(); 
                       return false;
                  }
                  
                    //*STORE不能輸入空
                   if(document.getElementById('txtStore').value.Trim() == "")
                  {
                       alert('STORE不能輸入空! ');
                       document.getElementById('txtStore').focus(); 
                       return false;
                  }
                  
                  //*MERCH不能輸入空
                   if(document.getElementById('txtMerch').value.Trim() == "")
                  {
                       alert('MERCH不能輸入空! ');
                       document.getElementById('txtMerch').focus(); 
                       return false;
                  }
                  
                   //*LEVEL不能輸入空
                   if(document.getElementById('txtLevel').value.Trim() == "")
                  {
                       alert('LEVEL不能輸入空! ');
                       document.getElementById('txtLevel').focus(); 
                       return false;
                  }
                  
                    //*SOURCE不能輸入空
                   if(document.getElementById('txtSource').value.Trim() == "")
                  {
                       alert('SOURCE不能輸入空! ');
                       document.getElementById('txtSource').focus(); 
                       return false;
                  }
                  
                   //*MCC不能輸入空
                   if(document.getElementById('txtMcc').value.Trim() == "")
                  {
                       alert('MCC不能輸入空! ');
                       document.getElementById('txtMcc').focus(); 
                       return false;
                  }
                  
                  //*AVE不能輸入空
                   if(document.getElementById('txtAve').value.Trim() == "")
                  {
                       alert('AVE不能輸入空! ');
                       document.getElementById('txtAve').focus(); 
                       return false;
                  }
                  
                    //*MONTH不能輸入空
                   if(document.getElementById('txtMonth').value.Trim() == "")
                  {
                       alert('MONTH不能輸入空! ');
                       document.getElementById('txtMonth').focus(); 
                       return false;
                  }
                  
                   //*BRANCH不能輸入空
                   if(document.getElementById('txtBranch').value.Trim() == "")
                  {
                       alert('BRANCH不能輸入空! ');
                       document.getElementById('txtBranch').focus(); 
                       return false;
                  }
                  
                    //*歸檔編號不能輸入空
                   if(document.getElementById('txtFileNo').value.Trim() == "")
                  {
                       alert('歸檔編號不能輸入空! ');
                       document.getElementById('txtFileNo').focus(); 
                       return false;
                  }
                  
                    //*HOLD STMT不能輸入空
                   if(document.getElementById('txtHOLDSTMT').value.Trim() == "")
                  {
                       alert('HOLD STMT不能輸入空! ');
                       document.getElementById('txtHOLDSTMT').focus(); 
                       return false;
                  }
                  
                   //*帳號一不能輸入空
                   if(document.getElementById('txtDdaNo1').value.Trim() == "")
                  {
                       alert('帳號一不能輸入空! ');
                       document.getElementById('txtDdaNo1').focus(); 
                       return false;
                  }
                  
                   //*帳號二不能輸入空
                   if(document.getElementById('txtDdaNo2').value.Trim() == "")
                  {
                       alert('帳號二不能輸入空! ');
                       document.getElementById('txtDdaNo2').focus(); 
                       return false;
                  }
                    
                    //*顯示確認提示框
                    if(!confirm('確定是否要新增資料？'))
                    {
                        return false;
                    }

                    //20210527 EOS_AML(NOVA) 增加欄位 by Ares Dennis
                    //*資料最後異動分行不為空
                    if (document.getElementById('txtLAST_UPD_BRANCH').value.Trim() == "") {
                        alert('資料最後異動分行不為空! ');
                        document.getElementById('txtLAST_UPD_BRANCH').focus();
                        return false;
                    }

                    //*資料最後異動MAKER不為空
                    if (document.getElementById('txtLAST_UPD_MAKER').value.Trim() == "") {
                        alert('資料最後異動MAKER不為空! ');
                        document.getElementById('txtLAST_UPD_MAKER').focus();
                        return false;
                    }

                    //*資料最後異動CHECKER不為空
                    if (document.getElementById('txtLAST_UPD_CHECKER').value.Trim() == "") {
                        alert('資料最後異動CHECKER不為空! ');
                        document.getElementById('txtLAST_UPD_CHECKER').focus();
                        return false;
                    }
                }
                
                 if(intType == 2)//*強制
                {
                    //*顯示確認提示框
                    if(!confirm('確定是否要強制執行？'))
                    {
                        return false;
                    }
                }
            }
            else
            {
                //*檢核輸入欄位身分證號碼是否為空
                if(document.getElementById('txtShopId').value.Trim() == "")
                {
                    alert('請輸入商店代號! ');
                    document.getElementById('txtShopId').focus();
                    return false;
                }
                
                //*檢核輸入欄位統編序號Corp一是否為空
                if(document.getElementById('txtCardNo1').value.Trim() == "")
                {
                    alert('請輸入統編序號Corp');
                    document.getElementById('txtCardNo1').focus();
                    return false;
                }
                
                //*檢核輸入欄位統編序號Corp二是否為空
                if(document.getElementById('txtCardNo2').value.Trim() == "")
                {
                    alert('請輸入統編序號Corp');
                    document.getElementById('txtCardNo2').focus();
                    return false;
                }
            } 
            return true;
        }
        
           //*商店代號欄位輸入值改變
      function changeStatus()
      {
            if(document.getElementById('txtShopId').value.Trim() != document.getElementById('txtShopIdHide').value.Trim())
            {
                var value = document.getElementById('txtShopId').value.Trim();
                //clearControls('pnlText');
                 setControlsDisabled('pnlText');
                document.getElementById('lblRegNameText').innerText = "";
                document.getElementById('lblBusinessNameText').innerText = "";  
            }
            document.getElementById('txtShopIdHide').value = document.getElementById('txtShopId').value;
            
      }
      
         //*統編序號Corp一欄位輸入值改變
      function changeCorpStatusOne()
      {
            if(document.getElementById('txtCardNo1').value.Trim() != document.getElementById('txtCardNo1Hide').value.Trim())
            {
                var value = document.getElementById('txtCardNo1').value.Trim();
                //clearControls('pnlText');
                 setControlsDisabled('pnlText');
                document.getElementById('lblRegNameText').innerText = "";
                document.getElementById('lblBusinessNameText').innerText = "";  
            }
            document.getElementById('txtCardNo1Hide').value = document.getElementById('txtCardNo1').value;
            
      }
      
        //*統編序號Corp二欄位輸入值改變
      function changeCorpStatusTwo()
      {
            if(document.getElementById('txtCardNo2').value.Trim() != document.getElementById('txtCardNo2Hide').value.Trim())
            {
                var value = document.getElementById('txtCardNo2').value.Trim();
                //clearControls('pnlText');
                 setControlsDisabled('pnlText');
                document.getElementById('lblRegNameText').innerText = "";
                document.getElementById('lblBusinessNameText').innerText = ""; 
            }
            document.getElementById('txtCardNo2Hide').value = document.getElementById('txtCardNo2').value;
            
      }
      
      //*檢核聯絡電話
      function checkTelLength(type)
      {
            if(type == 1)//*聯絡電話(1)
            {
                if(document.getElementById('txtPhone1').value.Trim() != "" && document.getElementById('txtPhone1').value.length < 2)
                {
                    alert('電話區碼錯誤');
                    setTimeout(function () {
                        document.getElementById("txtPhone1").focus();
                    }, 10);
                }
            }//*聯絡電話(2)
            else
            {
                if((document.getElementById('txtPhone1').value.trim() == "02" || document.getElementById('txtPhone1').value.trim() == "2") && document.getElementById('txtPhone2').value.Trim() != "" && document.getElementById('txtPhone2').value.length < 8)
                {
                     alert('電話號碼錯誤');
                     if(document.getElementById('txtPhone1').value.Trim() != "" && document.getElementById('txtPhone1').value.length < 2)
                    {
                         setTimeout(function () {
                             document.getElementById("txtPhone1").focus();
                         }, 10);
                    } 
                    else
                    {
                         setTimeout(function () {
                             document.getElementById("txtPhone2").focus();
                         }, 10);
                    }
                }
            }
      }
      
      
            //*Status欄位失去焦點
      function lostFocus(obj,id)
      {
            if(obj.value > 0)
            {
                document.getElementById(id).disabled = false;
            }
            else
            {
                document.getElementById(id).disabled = true;
            }           
      }
      
          //*選擇按鈕上按Tab鍵設置焦點,異動區域欄位可用，設置異動區域第一個為焦點，否則設置商店代號欄位為焦點
    function setfocuschoice()
    {
        if(event.keyCode==9)
        {
            event.returnValue=false;
            if(document.getElementById('txtMerchantName') != null)
            {
                //*基本資料異動部分，登記名稱欄位可用，設置為焦點
                if(document.getElementById('txtMerchantName').disabled == false)
                {
                    document.getElementById('txtMerchantName').focus();
                    return;
                }
            }
            //*異動區域都不可用，設置商店代號欄位為焦點
            document.getElementById('txtShopId').focus();
        }
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
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                 <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                        <tr class="itemTitle1">
                            <td colspan="4">
                                <li>
                                    <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01040202_001"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="lblShopId" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_002" StickHeight="False"></cc1:CustLabel><br /><br />
                                    <cc1:CustLabel ID="lblCardNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_004" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3" style="width: 85%">
                                <cc1:CustTextBox ID="txtShopId" runat="server" MaxLength="10" checktype="numandletter"
                                    Width="100px"  onkeydown="entersubmit('btnSelect');" TabIndex="1" BoxName="商店代號" onkeyup="changeStatus();"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtShopIdHide" runat="server" MaxLength="10" CssClass="btnHiden"></cc1:CustTextBox><br />
                                <cc1:CustTextBox ID="txtCardNo1" runat="server" MaxLength="8" checktype="numandletter"
                                    Width="100px" onkeydown="entersubmit('btnSelect');" TabIndex="2" BoxName="統編序號Corp一" onkeyup="changeCorpStatusOne();" onfocus="allselect(this);"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtCardNo2" runat="server" MaxLength="4" checktype="numandletter"
                                    Width="60px" onkeydown="entersubmit('btnSelect');" TabIndex="3" BoxName="統編序號Corp二" onkeyup="changeCorpStatusTwo();" onfocus="allselect(this);"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtCardNo1Hide" runat="server" MaxLength="8" CssClass="btnHiden"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtCardNo2Hide" runat="server" MaxLength="4" CssClass="btnHiden"></cc1:CustTextBox>
                                    <cc1:CustButton ID="btnSelect" runat="server" CssClass="smallButton" ShowID="01_01040202_057"
                                OnClick="btnSelect_Click" OnClientClick="return checkInputText('tabNo1', 0);" onkeydown="setfocuschoice();"
                                DisabledWhenSubmit="False"  TabIndex="4" />
                            </td>
                        </tr>
                    </table>
                    
                     <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo2" style="">
                            <tr class="trEven">
                             <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="lblMerchantName" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040202_003"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3" style="width: 85%">
                                 <cc1:CustRadioButton  ID="radRegName" runat="server" GroupName="1"/>
                                <cc1:CustLabel ID="lblRegName" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040202_060"
                                    StickHeight="False"> </cc1:CustLabel>
                                    <cc1:CustLabel ID="lblRegNameText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" 
                                    StickHeight="False"></cc1:CustLabel> <br />
                                
                                <cc1:CustRadioButton  ID="radBusinessName" runat="server" GroupName="1"/>
                                <cc1:CustLabel ID="lblBusinessName" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040202_061"
                                    StickHeight="False"> </cc1:CustLabel>
                                    <cc1:CustLabel ID="lblBusinessNameText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                    StickHeight="False"></cc1:CustLabel> <br />    
                                
                                <cc1:CustRadioButton  ID="radMerchantName" runat="server" GroupName="1"/>
                                <cc1:CustLabel ID="lblMerchantNameRight" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040202_062"
                                    StickHeight="False"> </cc1:CustLabel>&nbsp;&nbsp     
                                <cc1:CustTextBox ID="txtMerchantName" runat="server" MaxLength="14" Width="300px"  onblur="changeFullType(this);"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="5" BoxName="帳列名稱" checktype="fulltype" onpaste="paste();" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                        </tr>
                        
                        <tr class="trOdd">
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="lblEngName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_005" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35% ">
                                <cc1:CustTextBox ID="txtEngName" runat="server" MaxLength="25" 
                                    Width="200px" onkeydown="entersubmit('btnAdd');" TabIndex="6" BoxName="英文名稱" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="lblDbaCity" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_006" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtDbaCity" runat="server" MaxLength="14" checktype="numandletter"
                                    Width="260px" onkeydown="entersubmit('btnAdd');" TabIndex="7" BoxName="英文CITY" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                                              

                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="lblOwner" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_009" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtOwner" runat="server" MaxLength="25" checktype="undefined"
                                    Width="260px" onkeydown="entersubmit('btnAdd');" TabIndex="8" BoxName="負責人英文名" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="lblPhone" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_010" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtPhone1" runat="server" MaxLength="3" checktype="numandletter" Width="60px"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="9" onblur="checkTelLength(1);" BoxName="連絡電話一" onfocus="allselect(this);"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtPhone2" runat="server" MaxLength="8" checktype="numandletter" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="10" onblur="checkTelLength(2);" BoxName="連絡電話二" onfocus="allselect(this);"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtPhone3" runat="server" MaxLength="5" checktype="numandletter" Width="60px"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="11"  BoxName="連絡電話三" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                        </tr>   
                        <tr class="trOdd">
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="lblAddress" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_008" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 85%" colspan="3">
                                <cc1:CustTextBox ID="txtZip" runat="server" MaxLength="3" checktype="numandletter"
                                    Width="60px" onkeydown="entersubmit('btnAdd');" TabIndex="12" BoxName="區域號碼" onfocus="allselect(this);"></cc1:CustTextBox>
                                <cc1:CustButton ID="btnCheck" runat="server" CssClass="smallButton" ShowID="01_01040202_059"
                                OnClick="btnCheck_Click" DisabledWhenSubmit="False" Width="80px" TabIndex="13"/>
                                <cc1:CustTextBox ID="txtAddress1" runat="server" MaxLength="14"  onblur="changeFullType(this);"
                                    Width="220px" onkeydown="entersubmit('btnAdd');" TabIndex="14" BoxName="帳單地址一" checktype="fulltype" onpaste="paste();" onfocus="allselect(this);"></cc1:CustTextBox>                               
                                <cc1:CustTextBox ID="txtAddress2" runat="server" MaxLength="14"  onblur="changeFullType(this);"
                                    Width="220px" onkeydown="entersubmit('btnAdd');" TabIndex="15" BoxName="帳單地址二" checktype="fulltype" onpaste="paste();" onfocus="allselect(this);"></cc1:CustTextBox>
                             <%--   <cc1:CustTextBox ID="txtAddress3" runat="server" MaxLength="13"  onblur="changeFullType(this);"
                                    Width="220px" onkeypress="entersubmit('btnAdd');" TabIndex="15" BoxName="帳單地址三" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>--%>
                            </td>
                        </tr>

                        <tr class="itemTitle1">
                            <td colspan="4">
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo3" style="">
                        <tr class="trOdd">
                            <td style="width: 10%" align="right">
                                <cc1:CustLabel ID="lblImp1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_011" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustTextBox ID="txtImp1" runat="server" Width="50px" MaxLength="3" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="15" BoxName="IMP1" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td style="width: 10%" align="right">
                                <cc1:CustLabel ID="lblPos1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_012" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustTextBox ID="txtPos1" runat="server" Width="51px" MaxLength="3" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="16" BoxName="POS1" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td style="width: 10%" align="right">
                                <cc1:CustLabel ID="lblEdc1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_013" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustTextBox ID="txtEdc1" runat="server" Width="50px" MaxLength="3" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="17" BoxName="EDC1" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 10%" align="right">
                                <cc1:CustLabel ID="lblImp2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_014" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustTextBox ID="txtImp2" runat="server" Width="50px" MaxLength="3" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="18" BoxName="IMP2" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td style="width: 10%" align="right">
                                <cc1:CustLabel ID="lblPos2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_015" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustTextBox ID="txtPos2" runat="server" Width="50px" MaxLength="3" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="19" BoxName="POS2" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td style="width: 10%" align="right">
                                <cc1:CustLabel ID="lblEdc2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_016" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustTextBox ID="txtEdc2" runat="server" Width="50px" MaxLength="3" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="20" BoxName="EDC2" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="itemTitle1">
                            <td colspan="6">
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo4" style="">
                        <tr class="trOdd">
                            <td align="right" style="width: 15%; height: 25px;">
                                <cc1:CustLabel ID="lblSalesName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_017" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%; height: 25px;">
                                <cc1:CustTextBox ID="txtSalesName" runat="server" MaxLength="6" Width="120px"  onblur="changeFullType(this);"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="21" BoxName="推廣姓名" checktype="fulltype" onpaste="paste();" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td align="right" style="width: 15%; height: 25px;">
                                <cc1:CustLabel ID="lblAgentBank" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_018" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%; height: 25px;">
                                <cc1:CustTextBox ID="txtAgentBank" runat="server" MaxLength="5" Width="120px" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="22" BoxName="推廣代碼" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="itemTitle1">
                            <td colspan="4">
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo5" style="">
                        <tr class="trOdd">
                            <td style="width:15%; text-align:right;">
                                <cc1:CustLabel ID="lblType" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_019" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width:7%;">
                                <cc1:CustTextBox ID="txtType" runat="server" MaxLength="2" Width="30px" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="23" BoxName="TYPE" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td style="width:7%; text-align:right;">
                                <cc1:CustLabel ID="lblStore" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_020" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width:7%;">
                                <cc1:CustTextBox ID="txtStore" runat="server" MaxLength="9" Width="80px" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="24" BoxName="STORE" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td style="width:7%; text-align:right;">
                                <cc1:CustLabel ID="lblMerch" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_021" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width:7%;">
                                <cc1:CustTextBox ID="txtMerch" runat="server" MaxLength="9" Width="80px" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="25" BoxName="MERCH" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td style="width:7%; text-align:right;">
                                <cc1:CustLabel ID="lblLevel" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_022" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width:7%;">
                                <cc1:CustTextBox ID="txtLevel" runat="server" MaxLength="1" Width="30px" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="26" BoxName="LEVEL" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td style="width:7%; text-align:right;">
                                <cc1:CustLabel ID="lblSource" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_023" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width:7%;">
                                <cc1:CustTextBox ID="txtSource" runat="server" MaxLength="3" Width="40px" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="27" BoxName="SOURCE" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td colspan="2"></td>
                        </tr>
                        <tr class="trEven">
                            <td style="width:15%; text-align:right;">
                                <cc1:CustLabel ID="lblMcc" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_024" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width:7%;">
                                <cc1:CustTextBox ID="txtMcc" runat="server" MaxLength="4" Width="50px" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="28" BoxName="MCC" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td style="width:7%; text-align:right;">
                                <cc1:CustLabel ID="lblAve" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_025" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width:7%;">
                                <cc1:CustTextBox ID="txtAve" runat="server" MaxLength="7" Width="80px" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="29" BoxName="AVE" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td style="width:7%; text-align:right;">
                                <cc1:CustLabel ID="lblMonth" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_026" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width:7%;">
                                <cc1:CustTextBox ID="txtMonth" runat="server" MaxLength="5" Width="120px" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="30" BoxName="MONTH" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td style="width:7%; text-align:right;">
                                <cc1:CustLabel ID="lblBranch" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_027" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width:7%;">
                                <cc1:CustTextBox ID="txtBranch" runat="server" MaxLength="5" Width="70px" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="31" BoxName="BRANCH" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td> 
                            <td colspan="4"></td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width:15%; text-align:right;">
                                <cc1:CustLabel ID="lblInt" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_028" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width:7%;">
                                <cc1:CustTextBox ID="txtInt" runat="server" MaxLength="1" Width="30px" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="32" BoxName="INT" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td style="width:7%; text-align:right;">
                                <cc1:CustLabel ID="lblMail" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_029" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width:7%;">
                                <cc1:CustTextBox ID="txtMail" runat="server" MaxLength="1" Width="30px" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="33" BoxName="MAIL" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td style="width:7%; text-align:right;">
                                <cc1:CustLabel ID="lblPosCa" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_030" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width:7%;">
                                <cc1:CustTextBox ID="txtPosCa" runat="server" MaxLength="1" Width="30px" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="34" BoxName="POS CA" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td style="width:7%; text-align:right;">
                                <cc1:CustLabel ID="lblPosMo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_031" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width:7%;">
                                <cc1:CustTextBox ID="txtPosMo" runat="server" MaxLength="2" Width="30px" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="35" BoxName="POS MO" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td style="width:7%; text-align:right;">
                                <cc1:CustLabel ID="lblCH" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_032" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width:7%;">
                                <cc1:CustTextBox ID="txtCH" runat="server" MaxLength="1" Width="30px" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="36" BoxName="C/H" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td style="width:7%; text-align:right;">
                                <cc1:CustLabel ID="lblMC" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_033" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="">
                                <cc1:CustTextBox ID="txtMC" runat="server" MaxLength="1" Width="30px" checktype="numandletter"
                                    onkeydown="entersubmit('btnAdd');" TabIndex="37" BoxName="M/C" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="itemTitle1">
                            <td colspan="12">
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo6" style="">
                        <tr class="trOdd">
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="lblFileNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_034" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtFileNo" runat="server" MaxLength="9" checktype="numandletter"
                                    Width="140px" onkeydown="entersubmit('btnAdd');" TabIndex="38" BoxName="歸檔編號" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="lblHOLDSTMT" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_035" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtHOLDSTMT" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="30px" onkeydown="entersubmit('btnAdd');" TabIndex="39" BoxName="HOLD STMT" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="lblDdaNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_036" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 85%" colspan="3">
                                <cc1:CustTextBox ID="txtDdaNo1" runat="server" MaxLength="3" checktype="numandletter"
                                    Width="60px" onkeydown="entersubmit('btnAdd');" TabIndex="40" BoxName="帳號一" onfocus="allselect(this);"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtDdaNo2" runat="server" MaxLength="16" checktype="numandletter"
                                    Width="380px" onkeydown="entersubmit('btnAdd');" TabIndex="41" BoxName="帳號二" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="itemTitle1">
                            <td colspan="4">
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo7" style="">
                        <tr class="trOdd">
                            <td align="center" style="width: 34%" colspan="2">
                                <cc1:CustLabel ID="lblStatus" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_037" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 23%">
                                <cc1:CustLabel ID="lblDesc" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_047" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 43%">
                                <cc1:CustLabel ID="lblFee" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_038" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 17%">
                                <cc1:CustLabel ID="lblStatus1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_039" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%">
                                <cc1:CustTextBox ID="txtStatus1" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="30px" onkeydown="entersubmit('btnAdd');" TabIndex="42" onblur="lostFocus(this,'txtDisRate1');" BoxName="01" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 23%">
                                <cc1:CustLabel ID="lblNcccUs" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_048" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 43%">
                                <cc1:CustTextBox ID="txtDisRate1" runat="server" MaxLength="5" checktype="num"
                                    Width="80px" onkeydown="entersubmit('btnAdd');" TabIndex="57" BoxName="費率01" onfocus="allselect(this);"></cc1:CustTextBox>%
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 17%">
                                <cc1:CustLabel ID="lblStatus2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_040" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%">
                                <cc1:CustTextBox ID="txtStatus2" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="30px" onkeydown="entersubmit('btnAdd');" TabIndex="43" onblur="lostFocus(this,'txtDisRate2');" BoxName="02" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 23%">
                                <cc1:CustLabel ID="lblNcccNotUs" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_049" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 43%">
                                <cc1:CustTextBox ID="txtDisRate2" runat="server" MaxLength="5" checktype="num"
                                    Width="80px" onkeydown="entersubmit('btnAdd');" TabIndex="58" BoxName="費率02" onfocus="allselect(this);"></cc1:CustTextBox>%
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 17%">
                                <cc1:CustLabel ID="lblStatus3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_041" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%">
                                <cc1:CustTextBox ID="txtStatus3" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="30px" onkeydown="entersubmit('btnAdd');" TabIndex="44" onblur="lostFocus(this,'txtDisRate3');" BoxName="03" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 23%">
                                <cc1:CustLabel ID="lblVisaUs" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_050" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 43%">
                                <cc1:CustTextBox ID="txtDisRate3" runat="server" MaxLength="5" checktype="num"
                                    Width="80px" onkeydown="entersubmit('btnAdd');" TabIndex="59" BoxName="費率03" onfocus="allselect(this);"></cc1:CustTextBox>%
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 17%">
                                <cc1:CustLabel ID="lblStatus4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_042" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%">
                                <cc1:CustTextBox ID="txtStatus4" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="30px" onkeydown="entersubmit('btnAdd');" TabIndex="45" onblur="lostFocus(this,'txtDisRate4');" BoxName="04" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 23%">
                                <cc1:CustLabel ID="lblVisaNotUs" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_051" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 43%">
                                <cc1:CustTextBox ID="txtDisRate4" runat="server" MaxLength="5" checktype="num"
                                    Width="80px" onkeydown="entersubmit('btnAdd');" TabIndex="60" BoxName="費率04" onfocus="allselect(this);"></cc1:CustTextBox>%
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 17%">
                                <cc1:CustLabel ID="lblStatus5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_043" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%">
                                <cc1:CustTextBox ID="txtStatus5" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="30px" onkeydown="entersubmit('btnAdd');" TabIndex="46" onblur="lostFocus(this,'txtDisRate5');" BoxName="05" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 23%">
                                <cc1:CustLabel ID="lblMasterUs" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_052" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 43%">
                                <cc1:CustTextBox ID="txtDisRate5" runat="server" MaxLength="5" checktype="num"
                                    Width="80px" onkeydown="entersubmit('btnAdd');" TabIndex="61" BoxName="費率05" onfocus="allselect(this);"></cc1:CustTextBox>%
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 17%">
                                <cc1:CustLabel ID="lblStatus6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_044" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%">
                                <cc1:CustTextBox ID="txtStatus6" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="30px" onkeydown="entersubmit('btnAdd');" TabIndex="47" onblur="lostFocus(this,'txtDisRate6');" BoxName="06" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 23%">
                                <cc1:CustLabel ID="lblMasterNotUs" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_053" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 43%">
                                <cc1:CustTextBox ID="txtDisRate6" runat="server" MaxLength="5" checktype="num"
                                    Width="80px" onkeydown="entersubmit('btnAdd');" TabIndex="62" BoxName="費率06" onfocus="allselect(this);"></cc1:CustTextBox>%
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 17%">
                                <cc1:CustLabel ID="lblStatus7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_045" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%">
                                <cc1:CustTextBox ID="txtStatus7" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="30px" onkeydown="entersubmit('btnAdd');" TabIndex="48" onblur="lostFocus(this,'txtDisRate7');" BoxName="07" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 23%">
                                <cc1:CustLabel ID="lblJcbUs" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_054" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 43%">
                                <cc1:CustTextBox ID="txtDisRate7" runat="server" MaxLength="5" checktype="num"
                                    Width="80px" onkeydown="entersubmit('btnAdd');" TabIndex="63" BoxName="費率07" onfocus="allselect(this);"></cc1:CustTextBox>%
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 17%">
                                <cc1:CustLabel ID="lblStatus8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_046" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%">
                                <cc1:CustTextBox ID="txtStatus8" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="30px"  onkeydown="entersubmit('btnAdd');" TabIndex="49" onblur="lostFocus(this,'txtDisRate8');" BoxName="08" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 23%">
                                <cc1:CustLabel ID="lblJcbNotUs" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_055" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 43%">
                                <cc1:CustTextBox ID="txtDisRate8" runat="server" MaxLength="5" checktype="num"
                                    Width="80px" onkeydown="entersubmit('btnAdd');" TabIndex="64" BoxName="費率08" onfocus="allselect(this);"></cc1:CustTextBox>%
                            </td>
                        </tr>
                        
                        <tr class="trEven">
                            <td align="right" style="width: 17%">
                                <cc1:CustLabel ID="lblStatus9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_063" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%">
                                <cc1:CustTextBox ID="txtStatus9" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="30px"  onkeydown="entersubmit('btnAdd');" TabIndex="50" onblur="lostFocus(this,'txtDisRate9');" BoxName="09" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 23%">
                                <cc1:CustLabel ID="lblDistriCardUs" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_070" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 43%">
                                <cc1:CustTextBox ID="txtDisRate9" runat="server" MaxLength="5" checktype="num"
                                    Width="80px" onkeydown="entersubmit('btnAdd');" TabIndex="65" BoxName="費率09" onfocus="allselect(this);"></cc1:CustTextBox>%
                            </td>
                        </tr>
                        
                        <tr class="trOdd">
                            <td align="right" style="width: 17%">
                                <cc1:CustLabel ID="lblStatus10" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_064" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%">
                                <cc1:CustTextBox ID="txtStatus10" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="30px"  onkeydown="entersubmit('btnAdd');" TabIndex="51" onblur="lostFocus(this,'txtDisRate10');" BoxName="10" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 23%">
                                <!--20160525 (U) by Tank-->
                                <cc1:CustLabel ID="lblMasterUs2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040102_070" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 43%">
                                <cc1:CustTextBox ID="txtDisRate10" runat="server" MaxLength="5" checktype="num"
                                    Width="80px" onkeydown="entersubmit('btnAdd');" TabIndex="66" BoxName="費率10" onfocus="allselect(this);"></cc1:CustTextBox>%
                            </td>
                        </tr>
                        
                        <tr class="trEven">
                            <td align="right" style="width: 17%">
                                <cc1:CustLabel ID="lblStatus11" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_065" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%">
                                <cc1:CustTextBox ID="txtStatus11" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="30px"  onkeydown="entersubmit('btnAdd');" TabIndex="52" onblur="lostFocus(this,'txtDisRate11');" BoxName="11" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 23%">
                                <!--20160525 (U) by Tank-->
                                <cc1:CustLabel ID="lblMasterNotUs2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040102_071" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 43%">
                                <cc1:CustTextBox ID="txtDisRate11" runat="server" MaxLength="5" checktype="num"
                                    Width="80px" onkeydown="entersubmit('btnAdd');" TabIndex="67" BoxName="費率11" onfocus="allselect(this);"></cc1:CustTextBox>%
                            </td>
                        </tr>
                        
                         <tr class="trOdd">
                            <td align="right" style="width: 17%">
                                <cc1:CustLabel ID="lblStatus12" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_066" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%">
                                <cc1:CustTextBox ID="txtStatus12" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="30px"  onkeydown="entersubmit('btnAdd');" TabIndex="53" onblur="lostFocus(this,'txtDisRate12');" BoxName="12" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 23%">
                                
                            </td>
                            <td align="center" style="width: 43%">
                                <cc1:CustTextBox ID="txtDisRate12" runat="server" MaxLength="5" checktype="num"
                                    Width="80px" onkeydown="entersubmit('btnAdd');" TabIndex="68" BoxName="費率12" onfocus="allselect(this);"></cc1:CustTextBox>%
                            </td>
                        </tr>
                        
                        <tr class="trEven">
                            <td align="right" style="width: 17%">
                                <cc1:CustLabel ID="lblStatus13" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_067" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%">
                                <cc1:CustTextBox ID="txtStatus13" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="30px"  onkeydown="entersubmit('btnAdd');" TabIndex="54" onblur="lostFocus(this,'txtDisRate13');" BoxName="13" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 23%">
                                
                            </td>
                            <td align="center" style="width: 43%">
                                <cc1:CustTextBox ID="txtDisRate13" runat="server" MaxLength="5" checktype="num"
                                    Width="80px" onkeydown="entersubmit('btnAdd');" TabIndex="69" BoxName="費率13" onfocus="allselect(this);"></cc1:CustTextBox>%
                            </td>
                        </tr>
                        
                        <tr class="trOdd">
                            <td align="right" style="width: 17%">
                                <cc1:CustLabel ID="lblStatus14" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_068" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%">
                                <cc1:CustTextBox ID="txtStatus14" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="30px"  onkeydown="entersubmit('btnAdd');" TabIndex="55" onblur="lostFocus(this,'txtDisRate14');" BoxName="14" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 23%">
                                
                            </td>
                            <td align="center" style="width: 43%">
                                <cc1:CustTextBox ID="txtDisRate14" runat="server" MaxLength="5" checktype="num"
                                    Width="80px" onkeydown="entersubmit('btnAdd');" TabIndex="70" BoxName="費率14" onfocus="allselect(this);"></cc1:CustTextBox>%
                            </td>
                        </tr>
                        
                        <tr class="trEven">
                            <td align="right" style="width: 17%">
                                <cc1:CustLabel ID="lblStatus15" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040202_069" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%">
                                <cc1:CustTextBox ID="txtStatus15" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="30px"  onkeydown="entersubmit('btnAdd');" TabIndex="56" onblur="lostFocus(this,'txtDisRate15');" BoxName="15" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 23%">
                                
                            </td>
                            <td align="center" style="width: 43%">
                                <cc1:CustTextBox ID="txtDisRate15" runat="server" MaxLength="5" checktype="num"
                                    Width="80px" onkeydown="entersubmit('btnAdd');" TabIndex="71" BoxName="費率15" onfocus="allselect(this);"></cc1:CustTextBox>%
                            </td>
                        </tr>
                        <tr class="itemTitle1">
                            <td colspan="12"></td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo8" style="">
                        <%--資料最後異動--%>
                        <tr class="trOdd">                            
                            <%--資料最後異動MAKER--%>
                            <td align="right" style="width: 10%; height: 33px;">
                                <cc1:CustLabel ID="lbLAST_UPD_MAKER" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_086"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%; height: 33px;">
                                <cc1:CustTextBox ID="txtLAST_UPD_MAKER" runat="server" MaxLength="9" checktype="numandletter"
                                    Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動MAKER" onfocus="allselect(this);" TabIndex="72"></cc1:CustTextBox>
                            </td>
                            <%--資料最後異動CHECKER--%>
                            <td align="right" style="width: 10%; height: 33px;">
                                <cc1:CustLabel ID="lbLAST_UPD_CHECKER" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_087" StickHeight="False"
                                    ></cc1:CustLabel>
                            </td>
                            <td style="width: 13%; height: 33px;" >
                                <cc1:CustTextBox ID="txtLAST_UPD_CHECKER" runat="server" MaxLength="9" checktype="numandletter"
                                    Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動CHECKER" onfocus="allselect(this);" TabIndex="73"></cc1:CustTextBox>
                            </td>
                            <%--資料最後異動分行--%>
                            <td align="right" style="width: 10%; height: 33px;">
                                <cc1:CustLabel ID="lbLAST_UPD_BRANCH" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_085" StickHeight="False"
                                    ></cc1:CustLabel>
                            </td>
                            <td style="width: 13%; height: 33px;">
                                <cc1:CustTextBox ID="txtLAST_UPD_BRANCH" runat="server" MaxLength="4" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動分行" checktype="numandletter" 
                                    onfocus="allselect(this);" TabIndex="74"></cc1:CustTextBox>
                            </td>
                        </tr>
                    </table> 
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo9" style="">
                        <tr class="itemTitle1">
                            <td align="center">
                                <cc1:CustButton ID="btnAdd" runat="server" CssClass="smallButton" ShowID="01_01040202_056"
                                    OnClick="btnAdd_Click" OnClientClick="return checkInputText('pnlText', 1);" DisabledWhenSubmit="False" TabIndex="75" onkeydown="setfocus('txtShopId');"/>&nbsp;&nbsp
                                <cc1:CustButton ID="btnForce" runat="server" CssClass="smallButton" ShowID="01_01040202_058"
                                    OnClick="btnForce_Click" OnClientClick="return checkInputText('pnlText', 2);"
                                    DisabledWhenSubmit="False" onkeydown="setfocus('txtShopId');"  Visible="False" />
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
