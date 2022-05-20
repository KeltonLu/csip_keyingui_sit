<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010104010101.aspx.cs" Inherits="P010104010101" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%--20210329_Ares_Stanley-調整半形轉全形失效; 20210408_Ares_Stanley-調整半形轉全形失效; 20210415_Ares_Stanley-調整全半形轉換--%>
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
        //*檢核輸入欄位身份證號碼是否為空
        if(document.getElementById('txtCardNo1').value.Trim() == "" || document.getElementById('txtCardNo2').value.Trim() == "")
        {
            alert('請輸入統一編號! ');
            document.getElementById('txtCardNo1').focus(); 
            return false;
        }
        
        if(document.getElementById('txtCardNo1').value.Trim().length!=8)
        {
            alert('統編請輸入8碼數字! ');
            document.getElementById('txtCardNo1').focus();
            return false;
        }
      
        
         if(!isNum(document.getElementById('txtCardNo1').value.Trim()))
         { 
            alert('統一編號只能輸入數字');
            return false; 
         }
        
        //*新增按鈕檢核
        if(intType == 1)
        {
            var obj1 = document.getElementById('txtNewCardNo1');
            var obj2 = document.getElementById('txtNewCardNo2');
            if(obj1 != null && obj2 != null)
            {
                if(obj1.value.Trim() == "" || obj2.value.Trim() == "")
                {
                    alert('請輸入新統一編號! ');
                    obj1.focus();
                    return false;
                }
                
                if(document.getElementById('txtCardNo1').value.Trim() + document.getElementById('txtCardNo2').value.Trim() == obj1.value.Trim() + obj2.value.Trim() )
                {
                    alert('新統編不可同 統一編號! ');
                    obj1.focus();
                    return false;
                }
            }
            
            //*檢核欄位輸入規則
            if(!checkInputType(id))
            {
                  return false;
            }
                 
            if(!checkLableType())
            {
                  return false;
            } 
                   
            var value = document.getElementById('txtJCIC').value.toUpperCase().Trim();
            if(!(value == "A" ||  value == "B" || value == "C" || value == ""))
            {
                alert('JCIC只能輸入A/B/C/空白');
                document.getElementById('txtJCIC').focus();
                return false;
            }
            
            if(document.getElementById('txtRedeemCycle').value.toUpperCase().Trim() == "D")
            {
                if(!confirm('確定要將紅利週期修改為 D 嗎？'))
                {
                    return false;
                }
            }
            
            //*顯示確認提示框
            if(!confirm('確定是否要新增資料？'))
            {
                return false;
            }
        }     
        return true;
    }
    
    function checkAdd()
    {
        if(!confirm('資料庫無此筆城市資料,是否確定要新增資料?'))
       {
            return false;
       }
      return true; 
    }
 
//*將Lable文本轉為全型    
function changeLableFullType()
{
    //*營業名稱
    getValue('lblBusinessNameText');
   
    //*實際經營者姓名
    getValue('lblOpermanText');
    
    //*實際經營者換證點
    getValue('lblOperAtText');
    
    //*登記地址
    getValue('lblBusinessAddrText1');
    getValue('lblBusinessAddrText2');
    getValue('lblBusinessAddrText3');
}

function getValue(id)
{
    var obj = document.getElementById(id);
    if(obj.innerText.Trim() != "")
    {
         obj.innerText = FullType(obj.innerText.Trim());
    }
}
  
  //*中文、英文、數字檢核
function checkLableType()
{
        if(document.getElementById('chkOper').checked)
        {  
            //*檢核數字                                        
            if(!isNum(document.getElementById('lblOperTelText1').innerText))
            { 
                alert('實際經營者電話一欄位只能輸入數字');
                return false; 
            } 

            //*檢核數字   
            if(!isNum(document.getElementById('lblOperTelText2').innerText))
            { 
                alert('實際經營者電話二欄位只能輸入數字');
                return false; 
            } 

            //*檢核數字   
            if(!isNum(document.getElementById('lblOperTelText3').innerText))
            { 
                alert('實際經營者電話三欄位只能輸入數字');
                return false; 
            } 

            //*檢核數字   
            if(!isNum(document.getElementById('lblOperChangeDateText').innerText))
            { 
                alert('實際經營者領換補日欄位只能輸入數字');
                return false; 
            } 

            //*檢核英文和數字
            if(!ischinese(document.getElementById('lblOperFlagText').innerText))
            { 
                alert('代號欄位只能輸入英文和數字');
                return false; 
            } 

            //*檢核數字
            if(!isNum(document.getElementById('lblOperBirthdayText').innerText))
            { 
                alert('生日欄位只能輸入數字');
                return false; 
            } 

            //*檢核身分證號碼
            var inputText = document.getElementById('lblOperIDText').innerText;
            //*身分證號碼長度不為10
            if(!checkID(inputText))
            {                                              
                return false; 
            }
        }
        return true;
}

//*統一編號(1)、統一編號(2)欄位輸入值改變
  function changeStatus()
  {
        if(document.getElementById('txtCardNo1').value.Trim() != document.getElementById('txtCardNo1Hide').value.Trim() || document.getElementById('txtCardNo2').value.Trim() != document.getElementById('txtCardNo2Hide').value.Trim())
        {

            document.getElementById('lblBusinessNameText').innerText = "";
            document.getElementById('lblOpermanText').innerText = "";
            document.getElementById('lblOperIDText').innerText = "";
            document.getElementById('lblOperTelText1').innerText = "";
            document.getElementById('lblOperTelText2').innerText = "";
            document.getElementById('lblOperTelText3').innerText = "";
            document.getElementById('lblOperChangeDateText').innerText = "";
            document.getElementById('lblOperFlagText').innerText = "";
            document.getElementById('lblOperBirthdayText').innerText = "";
            document.getElementById('lblOperAtText').innerText = "";
        
            document.getElementById('lblBusinessAddrText1').innerText = "";
            document.getElementById('lblBusinessAddrText2').innerText = "";
            document.getElementById('lblBusinessAddrText3').innerText = "";
            
            document.getElementById('lblZipText').innerText = "";
            
            document.getElementById('chkBusinessName').checked = false;
            document.getElementById('chkOper').checked = false;
            document.getElementById('chkAddress').checked = false;
            setControlsDisabled('pnlText');
        }
        document.getElementById('txtCardNo1Hide').value = document.getElementById('txtCardNo1').value;
        document.getElementById('txtCardNo2Hide').value = document.getElementById('txtCardNo2').value;
  }

//*選擇按鈕上按Tab鍵設置焦點,
function setfocusmove()
{
    if(event.keyCode==9)
    {
        event.returnValue=false;
        var obj1 = document.getElementById('txtReceiveNumber');
        var obj2 = document.getElementById('txtCardNo1');
        var obj3 = document.getElementById('txtNewCardNo1');
       if(obj3 != null)
       {       
             if(obj3.disabled == false)
             { 
                obj3.focus();  
            }       
       }
      else
      {
          if(obj1.disabled == false)
          {
             obj1.focus();   
          }
          else
          {
              obj2.focus();   
          }        
      }         
    }
}

//*新增按鈕Tab鍵設置焦點
function movefocus()
{
    if(event.keyCode==9)
    {
        event.returnValue=false;
        var textbox = document.getElementById('txtCardNo1');
        var obj = document.getElementById('btnSelect');
        if(textbox.disabled)
        {
            obj.focus();
        }
        else
        {
            textbox.focus();
        }
    }
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
    &nbsp;
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                    <tr class="itemTitle">
                        <td colspan="9">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01040101_001"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    
                    <tr class="trOdd">
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblCardNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_002" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 89%" colspan="8">
                            <cc1:CustTextBox ID="txtCardNo1" runat="server" MaxLength="8" checktype="num" Width="80px"
                               onkeydown="entersubmit('btnSelect');" onkeyup="changeStatus();" BoxName="統一編號一"></cc1:CustTextBox>                   
                            <cc1:CustTextBox ID="txtCardNo2" runat="server" MaxLength="4" checktype="num" onkeydown="entersubmit('btnSelect');"
                                Width="40px"  onkeyup="changeStatus();"  BoxName="統一編號二"></cc1:CustTextBox>                         
                            <cc1:CustButton ID="btnSelect" runat="server" CssClass="smallButton" ShowID="01_01040101_027"
                            OnClick="btnSelect_Click" OnClientClick="return checkInputText('pnlText', 0);"
                            DisabledWhenSubmit="False" onkeydown="setfocusmove();"/>
                            <cc1:CustTextBox ID="txtCardNo1Hide" runat="server" MaxLength="8" CssClass="btnHiden"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtCardNo2Hide" runat="server" MaxLength="4" CssClass="btnHiden"></cc1:CustTextBox>
                        </td>
                     </tr>
                    </table> 
                    <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo4" style="">
                    <tr class="trEven">
                        <td align="right" style="width: 12%">
                            <cc1:CustLabel ID="lblNewCard" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_029" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 88%" colspan="8">
                            <cc1:CustTextBox ID="txtNewCardNo1" runat="server" MaxLength="8" checktype="num"
                                Width="80px" onkeydown="entersubmit('btnAdd');" BoxName="新統一編號一"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtNewCardNo2" runat="server" MaxLength="4" checktype="num"
                                Width="40px" onkeydown="entersubmit('btnAdd');" BoxName="新統一編號二"></cc1:CustTextBox>
                        </td>
                     </tr>      
                        
                    <tr class="trOdd">                           
                        <td align="right" style="width: 12%">
                            <cc1:CustLabel ID="lblReceiveNumber" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_003"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 44%" colspan="4">
                            <cc1:CustTextBox ID="txtReceiveNumber" runat="server" MaxLength="10" checktype="num"
                                onkeydown="entersubmit('btnAdd');" Width="100px" BoxName="收件編號"></cc1:CustTextBox>
                        </td>
                        
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblCheckMan" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_005" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 33%" colspan="3">
                            <cc1:CustTextBox ID="txtCheckMan" runat="server" MaxLength="4" checktype="num"
                                Width="50px" onkeydown="entersubmit('btnAdd');" BoxName="徵信員"></cc1:CustTextBox>
                        </td>                         
                    </tr>
                    
                    <tr class="trEven">
                        <td rowspan="5" align="right" style="width: 12%">
                            <cc1:CustLabel ID="lblShopData" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_030" StickHeight="False"></cc1:CustLabel>
                        </td>
                        
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblEstablish" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_004" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtEstablish" runat="server" MaxLength="5" checktype="numandletter"
                                Width="50px" onkeydown="entersubmit('btnAdd');" BoxName="設立"></cc1:CustTextBox>
                        </td>
                         
                         <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblCapital" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_006" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtCapital" runat="server" MaxLength="6" checktype="num" Width="70px"
                                onkeydown="entersubmit('btnAdd');" BoxName="資本"></cc1:CustTextBox>
                            &nbsp<cc1:CustLabel ID="lblMessage" runat="server" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_028"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblOrganization" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_008"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtOrganization" runat="server" MaxLength="1" checktype="numandletter"
                                Width="30px" onkeydown="entersubmit('btnAdd');" BoxName="組織"></cc1:CustTextBox>
                        </td>
                        
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblRisk" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_010" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtRisk" runat="server" MaxLength="3" checktype="numandletter"
                                Width="50px" onkeydown="entersubmit('btnAdd');" BoxName="風險"></cc1:CustTextBox>
                        </td>
                    </tr>
                                   
                    <tr class="trEven">
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblRegName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_007" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 77%" colspan="7">
                            <cc1:CustTextBox ID="txtRegName" runat="server" MaxLength="19"
                                Width="260px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="登記名稱" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                        </td>                         
                    </tr>
                    
                    <tr class="trOdd">
                        <td align="left" style="width: 11%" colspan="8">
                            <cc1:CustLabel ID="lblBusinessName" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_009"
                                StickHeight="False"></cc1:CustLabel>
                        </td>                 
                    </tr>
                    
                    <tr class="trOdd">
                        <td style="width: 77%" colspan="8" align="left">
                            <cc1:CustCheckBox  ID="chkBusinessName" runat="server" OnCheckedChanged="chkBusinessName_CheckedChanged" AutoPostBack="True" />
                            <cc1:CustLabel ID="lblBusinessName1" runat="server" CurAlign="left" CurSymbol="£"
                                IsColon="True" FractionalDigit="2" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_057"
                                StickHeight="False" ></cc1:CustLabel> 
                                <cc1:CustLabel ID="lblBusinessNameText" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                StickHeight="False" Width="260px"></cc1:CustLabel>  
                        </td>
                    </tr>
                    
                    <tr class="trOdd">
                        <td align="left" style="width: 11%" colspan="8">
                            <cc1:CustLabel ID="lblRight" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_059"
                                StickHeight="False"></cc1:CustLabel>
                            <cc1:CustTextBox ID="txtBusinessName" runat="server" MaxLength="19" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                Width="260px" BoxName="營業名稱" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                        </td>                 
                    </tr>
                    
                    <tr class="trOdd" >
                        <td align="right" style="width: 12%" rowspan="3">
                            <cc1:CustLabel ID="lblBossData" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2"  IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_032"
                                StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;&nbsp;
                                <cc1:CustLabel ID="lblBossData1" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_065"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblBoss" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_012" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtBoss" runat="server" MaxLength="4" 
                                Width="70px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人姓名" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                        </td>
                        
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblBossID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_013" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtBossID" runat="server" MaxLength="10" checktype="ID"
                                Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="負責人ID"></cc1:CustTextBox>
                        </td>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblBossTel" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_014" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 22%" colspan="3">
                            <cc1:CustTextBox ID="txtBossTel1" runat="server" MaxLength="3" checktype="num" Width="30px"
                                onkeydown="entersubmit('btnAdd');" BoxName="負責人電話一"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtBossTel2" runat="server" MaxLength="8" checktype="num" Width="70px"
                                onkeydown="entersubmit('btnAdd');" BoxName="負責人電話二"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtBossTel3" runat="server" MaxLength="5" checktype="num" Width="40px"
                                onkeydown="entersubmit('btnAdd');" BoxName="負責人電話三"></cc1:CustTextBox>
                        </td>                         
                    </tr>
                    
                    <tr class="trOdd">
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblBossChangeDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                 IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_032" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;&nbsp;
                                <cc1:CustLabel ID="lblBossChangeDate1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                 IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_061" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtBossChangeDate" runat="server" MaxLength="7" checktype="num" Width="50px"
                                onkeydown="entersubmit('btnAdd');" BoxName="負責人領換補日"></cc1:CustTextBox>
                        </td>
                        
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblBossFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_033" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtBossFlag" runat="server" MaxLength="1" checktype="numandletter" Width="20px"
                                onkeydown="entersubmit('btnAdd');" BoxName="代號"></cc1:CustTextBox>
                        </td>
                        
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblBossBirthday" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_034" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtBossBirthday" runat="server" MaxLength="7" checktype="num" Width="50px"
                                onkeydown="entersubmit('btnAdd');" BoxName="生日"></cc1:CustTextBox>
                        </td>
                        
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblBossAt" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                 IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_040" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtBossAt" runat="server" MaxLength="2" Width="40px"
                                onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="換證點" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                        </td>                          
                    </tr>
                    
                    <tr class="trOdd">
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblRegAddress" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_015" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 88%" colspan="8">
                            <cc1:CustTextBox ID="txtRegAddr1" runat="server"  onblur="changeFullType(this);" MaxLength="6"
                                Width="100px" BoxName="戶籍地址一" onpaste="paste();" checktype="fulltype" onkeydown="entersubmit('btnAdd');"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtRegAddr2" runat="server"  onblur="changeFullType(this);" MaxLength="14"
                                Width="220px" BoxName="戶籍地址二" onpaste="paste();" checktype="fulltype" onkeydown="entersubmit('btnAdd');"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtRegAddr3" runat="server"  onblur="changeFullType(this);" MaxLength="7"
                                Width="100px" BoxName="戶籍地址三" onpaste="paste();" checktype="fulltype" onkeydown="entersubmit('btnAdd');"></cc1:CustTextBox>
                        </td>                           
                    </tr>
                
                    <tr class="trEven" >
                        <td align="right" style="width: 12%" rowspan="6">
                            <cc1:CustLabel ID="lblOperData" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;&nbsp;
                                <cc1:CustLabel ID="lblOperData1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_065" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 88%" colspan="8">
                            <cc1:CustCheckBox  ID="chkOper" runat="server" OnCheckedChanged="chkOper_CheckedChanged" AutoPostBack="True" />
                            <cc1:CustLabel ID="lblSameBoss" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                     IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_058" StickHeight="False"></cc1:CustLabel>
                        </td>                       
                    </tr>
                    
                    <tr class="trEven">
                        <td style="width: 11%" align="right">
                            <cc1:CustLabel ID="lblOperman1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                     IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_016" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;
                                        <cc1:CustLabel ID="lblOperman2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                     IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_062" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">            
                             <cc1:CustLabel ID="lblOpermanText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False"  StickHeight="False" Width="60px"></cc1:CustLabel>
                       </td>
                        <td style="width: 11%" align="right">               
                             <cc1:CustLabel ID="lblOperID1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;
                                     <cc1:CustLabel ID="lblOperID2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                     IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_063" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                             <cc1:CustLabel ID="lblOperIDText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False"  StickHeight="False" Width="100px"></cc1:CustLabel>
                         </td>
                         <td style="width: 11%" align="right">        
                              <cc1:CustLabel ID="lblOperTel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;
                                     <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                     IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_064" StickHeight="False"></cc1:CustLabel>
                          </td>
                          <td style="width: 11%" colspan="3" align="left">   
                              <cc1:CustLabel ID="lblOperTelText1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False"  StickHeight="False" Width="30px"></cc1:CustLabel>
                              <cc1:CustLabel ID="lblOperTelText2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False"  StickHeight="False" Width="70px"></cc1:CustLabel>
                              <cc1:CustLabel ID="lblOperTelText3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False"  StickHeight="False" Width="40px"></cc1:CustLabel> 
                         </td>                               
                    </tr>
                    
                    <tr class="trEven">
                        <td style="width: 11%" align="right"> 
                            <cc1:CustLabel ID="lblOperChangeDate1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;
                                     <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                     IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_061" StickHeight="False"></cc1:CustLabel>
                       </td>
                      <td style="width: 11%">  
                            <cc1:CustLabel ID="lblOperChangeDateText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False"  StickHeight="False" Width="50px"></cc1:CustLabel>
                       </td>
                      <td style="width: 11%" align="right">            
                            <cc1:CustLabel ID="lblOperFlag1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_033" StickHeight="False"></cc1:CustLabel>      
                        </td>
                        
                        <td style="width: 11%">    
                            <cc1:CustLabel ID="lblOperFlagText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False"  StickHeight="False" Width="20px"></cc1:CustLabel>
                         </td>
                        <td style="width: 11%" align="right">         
                            <cc1:CustLabel ID="lblOperBirthday1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_034" StickHeight="False"></cc1:CustLabel> 
                          </td>
                          <td style="width: 11%">  
                            <cc1:CustLabel ID="lblOperBirthdayText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False"  StickHeight="False" Width="50px"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" align="right">       
                            <cc1:CustLabel ID="lblOperAt" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_040" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                            <cc1:CustLabel ID="lblOperAtText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False"  StickHeight="False" Width="40px"></cc1:CustLabel>
                        </td>
                    </tr>
                        
                    <tr class="trOdd">
                        <td align="left" colspan="8">
                            <cc1:CustLabel ID="lblDown" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_060" StickHeight="False"></cc1:CustLabel>
                        </td>
                    </tr>
                    
                    <tr class="trOdd" width="88%">
                        <td style="width: 11%" align="right">
                            <cc1:CustLabel ID="lblOperman" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_016" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;
                                     <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                     IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_062" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtOperman" runat="server" MaxLength="4" Width="60px"  onblur="changeFullType(this);" BoxName="實際經營者姓名" onpaste="paste();" checktype="fulltype" onkeydown="entersubmit('btnAdd');"></cc1:CustTextBox>
                        </td>
                        <td style="width: 11%" align="right">
                            <cc1:CustLabel ID="lblOperID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;
                                     <cc1:CustLabel ID="lblOperID3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                     IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False"  ShowID="01_01040101_063" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtOperID" runat="server" MaxLength="10" checktype="ID"
                                Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="實際經營者ID"></cc1:CustTextBox>
                        </td>
                        <td style="width: 11%" align="right">
                            <cc1:CustLabel ID="lblOperTel" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                 IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;
                                     <cc1:CustLabel ID="lblOperTel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                     IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_064" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td colspan="3">
                            <cc1:CustTextBox ID="txtOperTel1" runat="server" MaxLength="3" checktype="num" Width="30px"
                                onkeydown="entersubmit('btnAdd');" BoxName="實際經營者電話一"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtOperTel2" runat="server" MaxLength="8" checktype="num" Width="70px"
                                onkeydown="entersubmit('btnAdd');" BoxName="實際經營者電話二"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtOperTel3" runat="server" MaxLength="5" checktype="num" Width="40px"
                                onkeydown="entersubmit('btnAdd');" BoxName="實際經營者電話三"></cc1:CustTextBox>
                        </td>
                    </tr>                       
                                                            
                    <tr class="trOdd">
                        <td style="width: 11%" align="right">
                            <cc1:CustLabel ID="lblOperChangeDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;
                                     <cc1:CustLabel ID="lblOperChangeDate3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                     IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_061" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtOperChangeDate" runat="server" MaxLength="7" checktype="num" Width="50px"
                                onkeydown="entersubmit('btnAdd');" BoxName="實際經營者領換補日"></cc1:CustTextBox>
                      </td>
                      <td style="width: 11%" align="right">
                            <cc1:CustLabel ID="lblOperFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_033" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtOperFlag" runat="server" MaxLength="1" checktype="numandletter" Width="20px"
                                onkeydown="entersubmit('btnAdd');" BoxName="代號"></cc1:CustTextBox>
                        </td>
                        <td style="width: 11%" align="right">
                            <cc1:CustLabel ID="lblOperBirthday" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_034" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtOperBirthday" runat="server" MaxLength="7" checktype="num" Width="50px"
                                onkeydown="entersubmit('btnAdd');" BoxName="生日"></cc1:CustTextBox>
                        </td>
                        <td style="width: 11%" align="right">
                            <cc1:CustLabel ID="lblOperChangeAdd" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_040" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtOperAt" runat="server" MaxLength="2"  Width="40px"
                                 onblur="changeFullType(this);" BoxName="換證點" onpaste="paste();" checktype="fulltype" onkeydown="entersubmit('btnAdd');"></cc1:CustTextBox>
                        </td>
                    </tr>
                    
                    <tr class="trOdd">
                        <td align="right" style="width: 12%">
                            <cc1:CustLabel ID="lblContactMan" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_017" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblContactManName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_041" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtContactMan" runat="server"  onblur="changeFullType(this);"
                                MaxLength="4" Width="90px" BoxName="聯絡人姓名"  onpaste="paste();" checktype="fulltype" onkeydown="entersubmit('btnAdd');"></cc1:CustTextBox>
                        </td>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblContactManTel" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_050"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 22%" colspan="2">
                            <cc1:CustTextBox ID="txtContactManTel1" runat="server" MaxLength="3" checktype="num"
                                Width="25px" onkeydown="entersubmit('btnAdd');" BoxName="聯絡人電話一"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtContactManTel2" runat="server" MaxLength="8" checktype="num"
                                Width="58px" onkeydown="entersubmit('btnAdd');" BoxName="聯絡人電話二"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtContactManTel3" runat="server" MaxLength="5" checktype="num"
                                Width="36px" onkeydown="entersubmit('btnAdd');" BoxName="聯絡人電話三"></cc1:CustTextBox>
                        </td>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblFax" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_018" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%" colspan="2">
                            <cc1:CustTextBox ID="txtFax1" runat="server" MaxLength="3" checktype="num" Width="30px"
                                onkeydown="entersubmit('btnAdd');" BoxName="聯絡人傳真一"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtFax2" runat="server" MaxLength="8" checktype="num" Width="70px"
                                onkeydown="entersubmit('btnAdd');" BoxName="聯絡人傳真二"></cc1:CustTextBox>
                        </td>
                        
                    </tr>
                    
                    <tr class="trEven">
                        <td align="right" style="width: 12%" rowspan="4">
                            <cc1:CustLabel ID="lblAddData" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_042" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblBookAddress" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_019" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 88%" colspan="7">
                            <cc1:CustTextBox ID="txtBookAddr1" runat="server"  onblur="changeFullType(this);" onkeydown="entersubmit('btnAdd');"
                                MaxLength="6" Width="100px" BoxName="登記地址一" onpaste="paste();" checktype="fulltype"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtBookAddr2" runat="server"  onblur="changeFullType(this);" onkeydown="entersubmit('btnAdd');"
                                MaxLength="14" Width="220px" BoxName="登記地址二" onpaste="paste();" checktype="fulltype"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtBookAddr3" runat="server"  onblur="changeFullType(this);" onkeydown="entersubmit('btnAdd');"
                                MaxLength="7" Width="100px" BoxName="登記地址三" onpaste="paste();" checktype="fulltype"></cc1:CustTextBox>
                        </td>
                    </tr>
                    
                    <tr class="trOdd">
                        <td align="left" style="width: 11%" colspan="8">
                            <cc1:CustLabel ID="lblBusinessAddress" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_020"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                    </tr>
                    
                    <tr class="trOdd">
                        <td colspan="8">
                            <cc1:CustCheckBox ID="chkAddress" runat="server" OnCheckedChanged="chkAddress_CheckedChanged" AutoPostBack="True"/>
                            <cc1:CustLabel ID="lblAdd" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_055"
                                    StickHeight="False"></cc1:CustLabel> 
                            <cc1:CustLabel ID="lblBusinessZipText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                    StickHeight="False" ShowID="" Width="50px"></cc1:CustLabel>
                            <cc1:CustLabel ID="lblBusinessAddrText1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                    StickHeight="False" ShowID="" Width="100px"></cc1:CustLabel>
                             <cc1:CustLabel ID="lblBusinessAddrText2" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                    StickHeight="False" ShowID="" Width="220px"></cc1:CustLabel>
                             <cc1:CustLabel ID="lblBusinessAddrText3" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                    StickHeight="False" ShowID="" Width="100px"></cc1:CustLabel>
                         </td>       
                    </tr>
                    
                    <tr class="trOdd">
                        <td colspan="8">
                            <cc1:CustLabel ID="lblRight2" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_059"
                                    StickHeight="False"></cc1:CustLabel>
                            <cc1:CustLabel ID="lblZipText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                    StickHeight="False" Width="50px"></cc1:CustLabel>       
                            <cc1:CustTextBox ID="txtBusinessAddr4" runat="server"
                                MaxLength="6" Width="100px"  onblur="changeFullType(this);" onkeydown="entersubmit('btnAdd');" BoxName="營業地址一" onpaste="paste();" checktype="fulltype"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtBusinessAddr5" runat="server" 
                                MaxLength="14" Width="220px"  onblur="changeFullType(this);" onkeydown="entersubmit('btnAdd');" BoxName="營業地址二" onpaste="paste();" checktype="fulltype"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtBusinessAddr6" runat="server"
                                MaxLength="7" Width="100px"  onblur="changeFullType(this);" onkeydown="entersubmit('btnAdd');" BoxName="營業地址三" onpaste="paste();" checktype="fulltype"></cc1:CustTextBox>                           
                            <cc1:CustButton ID="btnSearchZip" runat="server" CssClass="smallButton" ShowID="01_01040301_088"                            
                            DisabledWhenSubmit="False" OnClick="btnSearchZip_Click" />
                         </td>       
                    </tr>
                    
                    <tr class="trOdd">
                        <td align="right" style="width: 12%">
                            <cc1:CustLabel ID="lblJCIC" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_043"
                                StickHeight="False" ForeColor="Red"></cc1:CustLabel>
                                
                        </td>
                        <td style="width: 88%" colspan="8">
                            <cc1:CustTextBox ID="txtJCIC" runat="server" MaxLength="1" Width="100px" onkeydown="entersubmit('btnAdd');" checktype="numandletter" BoxName="JCIC查詢"></cc1:CustTextBox>
                                <cc1:CustLabel ID="lblMessage1" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_031"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                    </tr>
                    
                    <tr class="trEven">
                        <td align="right" style="width: 12%" rowspan="2">
                            <cc1:CustLabel ID="lblAccounts" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_044"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblBank" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_021" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 33%" colspan="3">
                            <cc1:CustTextBox ID="txtBank" runat="server" MaxLength="5" 
                                Width="80px"  onblur="changeFullType(this);" onkeydown="entersubmit('btnAdd');" BoxName="銀行(中文)" onpaste="paste();" checktype="fulltype"></cc1:CustTextBox>
                        </td>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblBranchBank" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_022" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 33%" colspan="3">
                            <cc1:CustTextBox ID="txtBranchBank" runat="server" MaxLength="10"
                                Width="140px"  onblur="changeFullType(this);" onkeydown="entersubmit('btnAdd');" BoxName="分行(中文)" onpaste="paste();" checktype="fulltype"></cc1:CustTextBox>
                        </td>
                    </tr>
                    
                    <tr class="trEven">
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_023" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 88%" colspan="8">
                            <cc1:CustTextBox ID="txtName" runat="server" MaxLength="20" 
                                Width="300px"  onblur="changeFullType(this);" onkeydown="entersubmit('btnAdd');" BoxName="戶名" onpaste="paste();" checktype="fulltype"></cc1:CustTextBox>
                        </td>
                    </tr>
                    
                    <tr class="trOdd">
                        <td align="right" style="width: 12%; height: 33px;">
                            <cc1:CustLabel ID="lblPrev" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_045"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        
                        <td align="right" style="width: 11%; height: 33px;">
                            <cc1:CustLabel ID="lblPrevDesc" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_046"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 33%; height: 33px;" colspan="3">
                            <cc1:CustTextBox ID="txtPrevDesc" runat="server" MaxLength="4"
                                Width="100px"  onblur="changeFullType(this);" onkeydown="entersubmit('btnAdd');" BoxName="帳單內容" onpaste="paste();" checktype="fulltype"></cc1:CustTextBox>
                        </td>
                        
                        <td align="right" style="width: 11%; height: 33px;">
                            <cc1:CustLabel ID="lblInvoiceCycle" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_025"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%; height: 33px;">
                            <cc1:CustTextBox ID="txtInvoiceCycle" runat="server" MaxLength="2" checktype="num"
                                Width="20px" onkeydown="entersubmit('btnAdd');" BoxName="發票週期"></cc1:CustTextBox>
                        </td>
                        
                        <td align="right" style="width: 11%; height: 33px;">
                            <cc1:CustLabel ID="lblRedeemCycle" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_047"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%; height: 33px;">
                            <cc1:CustTextBox ID="txtRedeemCycle" runat="server" MaxLength="1" checktype="numandletter"
                                Width="30px" onkeydown="entersubmit('btnAdd');" BoxName="紅利週期(M/D)"></cc1:CustTextBox>
                        </td>
                    </tr>
                    
                    <tr class="trEven">
                        <td align="right" style="width: 12%">
                            <cc1:CustLabel ID="lblPopMan" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_024" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 88%" colspan="8">
                            <cc1:CustTextBox ID="txtPopMan" runat="server"  onblur="changeFullType(this);"  onkeydown="entersubmit('btnAdd');" MaxLength="3"
                                Width="50px" BoxName="推廣員" onpaste="paste();" checktype="fulltype"></cc1:CustTextBox>
                        </td>             
                    </tr>
                    
                    <tr>
                        <td nowrap colspan="6" style="height: 1px">
                        </td>
                    </tr>
                </table>        
            
            <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo3" style="">
                <tr class="itemTitle">
                    <td align="center">
                        <cc1:CustButton ID="btnAdd" runat="server" CssClass="smallButton" ShowID="01_01040101_026"
                            OnClick="btnAdd_Click" OnClientClick="return checkInputText('pnlText', 1);" DisabledWhenSubmit="False" onkeydown="movefocus();"/>
                    </td>
                </tr>
            </table>
            </cc1:CustPanel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <cc1:CustButton ID="btnHiden" OnClick="btnHiden_Click" runat="server" CssClass="btnHiden"
            DisabledWhenSubmit="False"></cc1:CustButton>
            <cc1:CustButton ID="btnAddHiden" runat="server" CssClass="btnHiden"
            DisabledWhenSubmit="False" OnClick="btnAddHiden_Click" OnClientClick="return checkAdd();"></cc1:CustButton>
</form>
</body>
</html>
