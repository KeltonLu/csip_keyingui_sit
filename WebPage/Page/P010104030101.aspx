<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010104030101.aspx.cs" Inherits="P010104030101" %>

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
            alert('統編只能輸入數字');
            return false; 
         } 
        
        
        //*新增按鈕檢核
        if(intType == 1)
        {
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
            if(!confirm('確定是否要更新資料？'))
            {
                return false;
            }
        }     
        return true;
    }
    
    function checkAdd()
    {
        if(!confirm('資料庫無此筆城市資料,是否確定要更新資料?'))
       {
            return false;
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
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01040301_001"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    
                    <tr class="trEven">
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblCardNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_002" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 89%" colspan="8">
                            <cc1:CustTextBox ID="txtCardNo1" runat="server" MaxLength="8" checktype="num" Width="80px"
                                 onkeydown="entersubmit('btnSelect');" onkeyup="changeStatus();" BoxName="統一編號一"></cc1:CustTextBox>                   
                            <cc1:CustTextBox ID="txtCardNo2" runat="server" MaxLength="4" checktype="num" onkeydown="entersubmit('btnSelect');"
                                Width="40px" onkeyup="changeStatus();" BoxName="統一編號二"></cc1:CustTextBox>                         
                            <cc1:CustButton ID="btnSelect" runat="server" CssClass="smallButton" ShowID="01_01040301_027"
                            OnClick="btnSelect_Click" OnClientClick="return checkInputText('pnlText', 0);"
                            DisabledWhenSubmit="False" onkeydown="setfocuschoice('txtCardNo1','txtReceiveNumber');"/>
                            <cc1:CustTextBox ID="txtCardNo1Hide" runat="server" MaxLength="8" CssClass="btnHiden"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtCardNo2Hide" runat="server" MaxLength="4" CssClass="btnHiden"></cc1:CustTextBox>
                        </td>
                     </tr>
                     </table>
   
                    <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo4" style="">    
                    <tr class="trOdd">                           
                        <td align="right" style="width: 12%">
                            <cc1:CustLabel ID="lblReceiveNumber" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040301_003"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 44%" colspan="4">
                            <cc1:CustTextBox ID="txtReceiveNumber" runat="server" MaxLength="10" checktype="num"
                                onkeydown="entersubmit('btnUpdate');" Width="100px" BoxName="收件編號"></cc1:CustTextBox>
                        </td>
                        
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblCheckMan" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_005" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 33%" colspan="3">
                            <cc1:CustTextBox ID="txtCheckMan" runat="server" MaxLength="4" checktype="num"
                                Width="50px" onkeydown="entersubmit('btnUpdate');" BoxName="徵信員"></cc1:CustTextBox>
                        </td>                         
                    </tr>
                    
                    <tr class="trEven">
                        <td rowspan="5" align="right" style="width: 12%">
                            <cc1:CustLabel ID="lblShopData" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_030" StickHeight="False"></cc1:CustLabel>
                        </td>
                        
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblEstablish" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_004" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtEstablish" runat="server" MaxLength="5" checktype="numandletter"
                                Width="50px" onkeydown="entersubmit('btnUpdate');" BoxName="設立"></cc1:CustTextBox>
                        </td>
                         
                         <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblCapital" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_006" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtCapital" runat="server" MaxLength="6" checktype="num" Width="70px"
                                onkeydown="entersubmit('btnUpdate');" BoxName="資本"></cc1:CustTextBox>
                            &nbsp<cc1:CustLabel ID="lblMessage" runat="server" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040301_028"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblOrganization" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040301_008"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtOrganization" runat="server" MaxLength="1" checktype="numandletter"
                                Width="30px" onkeydown="entersubmit('btnUpdate');" BoxName="組織"></cc1:CustTextBox>
                        </td>
                        
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblRisk" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_010" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtRisk" runat="server" MaxLength="3" checktype="numandletter"
                                Width="50px" onkeydown="entersubmit('btnUpdate');" BoxName="風險"></cc1:CustTextBox>
                        </td>
                    </tr>
                                   
                    <tr class="trEven">
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblRegName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_007" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 77%" colspan="7">
                            <cc1:CustTextBox ID="txtRegName" runat="server" MaxLength="19"
                                Width="260px" onkeydown="entersubmit('btnUpdate');"  onblur="changeFullType(this);" BoxName="登記名稱" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                        </td>                         
                    </tr>
                    
                    <tr class="trOdd">
                        <td align="left" style="width: 11%" colspan="8">
                            <cc1:CustLabel ID="lblBusinessName" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040301_009"
                                StickHeight="False"></cc1:CustLabel>
                        </td>                 
                    </tr>
                    
                    <tr class="trOdd">
                        <td style="width: 77%" colspan="8" align="left">
                            <cc1:CustCheckBox  ID="chkBusinessName" runat="server" OnCheckedChanged="chkBusinessName_CheckedChanged" AutoPostBack="True" />
                            <cc1:CustLabel ID="lblBusinessName1" runat="server" CurAlign="left" CurSymbol="£"
                                IsColon="True" FractionalDigit="2" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040301_057"
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
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040301_059"
                                StickHeight="False"></cc1:CustLabel>
                            <cc1:CustTextBox ID="txtBusinessName" runat="server" MaxLength="19" onkeydown="entersubmit('btnUpdate');"  onblur="changeFullType(this);"
                                Width="260px" BoxName="營業名稱" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                        </td>                 
                    </tr>
                    
                    <tr class="trOdd" >
                        <td align="right" style="width: 12%" rowspan="3">
                            <cc1:CustLabel ID="lblBossData" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2"  IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040301_032"
                                StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;&nbsp;
                                <cc1:CustLabel ID="lblBossData1" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040301_065"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblBoss" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_012" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtBoss" runat="server" MaxLength="4" 
                                Width="70px" onkeydown="entersubmit('btnUpdate');"  onblur="changeFullType(this);" BoxName="負責人姓名" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                        </td>
                        
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblBossID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_013" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtBossID" runat="server" MaxLength="10" checktype="ID"
                                Width="100px" onkeydown="entersubmit('btnUpdate');" BoxName="負責人ID"></cc1:CustTextBox>
                        </td>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblBossTel" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_014" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 22%" colspan="3">
                            <cc1:CustTextBox ID="txtBossTel1" runat="server" MaxLength="3" checktype="num" Width="30px"
                                onkeydown="entersubmit('btnUpdate');" BoxName="負責人電話一"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtBossTel2" runat="server" MaxLength="8" checktype="num" Width="70px"
                                onkeydown="entersubmit('btnUpdate');" BoxName="負責人電話二"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtBossTel3" runat="server" MaxLength="5" checktype="num" Width="40px"
                                onkeydown="entersubmit('btnUpdate');" BoxName="負責人電話三"></cc1:CustTextBox>
                        </td>                         
                    </tr>
                    
                    <tr class="trOdd">
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblBossChangeDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                 IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_032" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;&nbsp;
                                <cc1:CustLabel ID="lblBossChangeDate1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                 IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_061" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtBossChangeDate" runat="server" MaxLength="7" checktype="num" Width="50px"
                                onkeydown="entersubmit('btnUpdate');" BoxName="負責人領換補日"></cc1:CustTextBox>
                        </td>
                        
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblBossFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_033" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtBossFlag" runat="server" MaxLength="1" checktype="numandletter" Width="20px"
                                onkeydown="entersubmit('btnUpdate');" BoxName="代號"></cc1:CustTextBox>
                        </td>
                        
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblBossBirthday" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_034" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtBossBirthday" runat="server" MaxLength="7" checktype="num" Width="50px"
                                onkeydown="entersubmit('btnUpdate');" BoxName="生日"></cc1:CustTextBox>
                        </td>
                        
                        <td align="right" style="width: 11%">

                            <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                 IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_040" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtBossAt" runat="server" MaxLength="2"  Width="40px"
                                onkeydown="entersubmit('btnUpdate');"  onblur="changeFullType(this);" BoxName="換證點" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                        </td>                          
                    </tr>
                    
                    <tr class="trOdd">
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblRegAddress" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_015" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 88%" colspan="8">
                            <cc1:CustTextBox ID="txtRegAddr1" runat="server" onkeydown="entersubmit('btnUpdate');"  onblur="changeFullType(this);" MaxLength="6"
                                Width="100px" BoxName="戶籍地址一" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtRegAddr2" runat="server" onkeydown="entersubmit('btnUpdate');"  onblur="changeFullType(this);" MaxLength="14"
                                Width="220px" BoxName="戶籍地址二" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtRegAddr3" runat="server" onkeydown="entersubmit('btnUpdate');"  onblur="changeFullType(this);" MaxLength="7"
                                Width="100px" BoxName="戶籍地址三" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                        </td>                           
                    </tr>
                 
                    <tr class="trEven" >
                        <td align="right" style="width: 12%" rowspan="6">
                            <cc1:CustLabel ID="lblOperData" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;&nbsp;
                                <cc1:CustLabel ID="lblOperData1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_065" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 88%" colspan="8">
                            <cc1:CustCheckBox  ID="chkOper" runat="server" OnCheckedChanged="chkOper_CheckedChanged" AutoPostBack="True" />
                            <cc1:CustLabel ID="lblSameBoss" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                     IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040301_058" StickHeight="False"></cc1:CustLabel>
                        </td>                       
                    </tr>
                    
                    <tr class="trEven">
                        <td style="width: 11%" align="right">
                            <cc1:CustLabel ID="lblOperman1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                     IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040301_016" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;
                                        <cc1:CustLabel ID="lblOperman2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                     IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040301_062" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">            
                             <cc1:CustLabel ID="lblOpermanText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False"  StickHeight="False" Width="60px"></cc1:CustLabel>
                       </td>
                        <td style="width: 11%" align="right">               
                             <cc1:CustLabel ID="lblOperID1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040301_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;
                                     <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                     IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040301_063" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                             <cc1:CustLabel ID="lblOperIDText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False"  StickHeight="False" Width="100px"></cc1:CustLabel>
                         </td>
                         <td style="width: 11%" align="right">        
                              <cc1:CustLabel ID="lblOperTel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040301_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;
                                     <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                     IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040301_064" StickHeight="False"></cc1:CustLabel>
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
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;
                                     <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                     IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040301_061" StickHeight="False"></cc1:CustLabel>
                       </td>
                      <td style="width: 11%">  
                            <cc1:CustLabel ID="lblOperChangeDateText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False"  StickHeight="False" Width="50px"></cc1:CustLabel>
                       </td>
                      <td style="width: 11%" align="right">            
                            <cc1:CustLabel ID="lblOperFlag1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_033" StickHeight="False"></cc1:CustLabel>      
                        </td>
                        
                        <td style="width: 11%">    
                            <cc1:CustLabel ID="lblOperFlagText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False"  StickHeight="False" Width="20px"></cc1:CustLabel>
                         </td>
                        <td style="width: 11%" align="right">         
                            <cc1:CustLabel ID="lblOperBirthday1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_034" StickHeight="False"></cc1:CustLabel> 
                          </td>
                          <td style="width: 11%">  
                            <cc1:CustLabel ID="lblOperBirthdayText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False"  StickHeight="False" Width="50px"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" align="right">       
                            <cc1:CustLabel ID="lblOperAt" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_040" StickHeight="False"></cc1:CustLabel>
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
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_060" StickHeight="False"></cc1:CustLabel>
                        </td>
                    </tr>
                    
                    <tr class="trOdd" width="88%">
                        <td style="width: 11%" align="right">
                            <cc1:CustLabel ID="lblOperman" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_016" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;
                                     <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                     IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040301_062" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtOperman" runat="server" MaxLength="4" Width="60px" onkeydown="entersubmit('btnUpdate');"  onblur="changeFullType(this);" BoxName="實際經營者姓名" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                        </td>
                        <td style="width: 11%" align="right">
                            <cc1:CustLabel ID="lblOperID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;
                                     <cc1:CustLabel ID="lblOperID2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                     IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False"  ShowID="01_01040301_063" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtOperID" runat="server" MaxLength="10" checktype="ID"
                                Width="100px" onkeydown="entersubmit('btnUpdate');" BoxName="實際經營者ID"></cc1:CustTextBox>
                        </td>
                        <td style="width: 11%" align="right">
                            <cc1:CustLabel ID="lblOperTel" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                 IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;
                                     <cc1:CustLabel ID="lblOperTel7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                     IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040301_064" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td colspan="3">
                            <cc1:CustTextBox ID="txtOperTel1" runat="server" MaxLength="3" checktype="num" Width="30px"
                                onkeydown="entersubmit('btnUpdate');" BoxName="實際經營者電話一"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtOperTel2" runat="server" MaxLength="8" checktype="num" Width="70px"
                                onkeydown="entersubmit('btnUpdate');" BoxName="實際經營者電話二"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtOperTel3" runat="server" MaxLength="5" checktype="num" Width="40px"
                                onkeydown="entersubmit('btnUpdate');" BoxName="實際經營者電話三"></cc1:CustTextBox>
                        </td>
                    </tr>                       
                                                            
                    <tr class="trOdd">
                        <td style="width: 11%" align="right">
                            <cc1:CustLabel ID="lblOperChangeDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;
                                     <cc1:CustLabel ID="CustLabel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                     IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040301_061" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtOperChangeDate" runat="server" MaxLength="7" checktype="num" Width="50px"
                                onkeydown="entersubmit('btnUpdate');" BoxName="實際經營者領換補日"></cc1:CustTextBox>
                      </td>
                      <td style="width: 11%" align="right">
                            <cc1:CustLabel ID="lblOperFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_033" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtOperFlag" runat="server" MaxLength="1" checktype="numandletter" Width="20px"
                                onkeydown="entersubmit('btnUpdate');" BoxName="代號"></cc1:CustTextBox>
                        </td>
                        <td style="width: 11%" align="right">
                            <cc1:CustLabel ID="lblOperBirthday" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_034" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtOperBirthday" runat="server" MaxLength="7" checktype="num" Width="50px"
                                onkeydown="entersubmit('btnUpdate');" BoxName="生日"></cc1:CustTextBox>
                        </td>
                        <td style="width: 11%" align="right">
                            <cc1:CustLabel ID="lblOperChangeAdd" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_040" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtOperAt" runat="server" MaxLength="2"  Width="40px"
                                onkeydown="entersubmit('btnUpdate');"  onblur="changeFullType(this);" BoxName="換證點" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                        </td>
                    </tr>
                    
                    <tr class="trOdd">
                        <td align="right" style="width: 12%">
                            <cc1:CustLabel ID="lblContactMan" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_017" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblContactManName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_041" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustTextBox ID="txtContactMan" runat="server" onkeydown="entersubmit('btnUpdate');"
                                MaxLength="4" Width="90px"  onblur="changeFullType(this);" BoxName="聯絡人姓名" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                        </td>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblContactManTel" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040301_050"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 22%" colspan="2">
                            <cc1:CustTextBox ID="txtContactManTel1" runat="server" MaxLength="3" checktype="num"
                                Width="25px" onkeydown="entersubmit('btnUpdate');" BoxName="聯絡人電話一"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtContactManTel2" runat="server" MaxLength="8" checktype="num"
                                Width="58px" onkeydown="entersubmit('btnUpdate');" BoxName="聯絡人電話二"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtContactManTel3" runat="server" MaxLength="5" checktype="num"
                                Width="36px" onkeydown="entersubmit('btnUpdate');" BoxName="聯絡人電話三"></cc1:CustTextBox>
                        </td>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblFax" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_018" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%" colspan="2">
                            <cc1:CustTextBox ID="txtFax1" runat="server" MaxLength="3" checktype="num" Width="30px"
                                onkeydown="entersubmit('btnUpdate');" BoxName="聯絡人傳真一"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtFax2" runat="server" MaxLength="8" checktype="num" Width="70px"
                                onkeydown="entersubmit('btnUpdate');" BoxName="聯絡人傳真二"></cc1:CustTextBox>
                        </td>
                        
                    </tr>
                    
                    <tr class="trEven">
                        <td align="right" style="width: 12%" rowspan="4">
                            <cc1:CustLabel ID="lblAddData" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_042" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblBookAddress" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_019" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 88%" colspan="7">
                            <cc1:CustTextBox ID="txtBookAddr1" runat="server" onkeydown="entersubmit('btnUpdate');"  onblur="changeFullType(this);"
                                MaxLength="6" Width="100px" BoxName="登記地址一" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtBookAddr2" runat="server" onkeydown="entersubmit('btnUpdate');"  onblur="changeFullType(this);"
                                MaxLength="14" Width="220px" BoxName="登記地址二" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtBookAddr3" runat="server" onkeydown="entersubmit('btnUpdate');"  onblur="changeFullType(this);"
                                MaxLength="7" Width="100px" BoxName="登記地址三" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                        </td>
                    </tr>
                    
                    <tr class="trOdd">
                        <td align="left" style="width: 11%" colspan="8">
                            <cc1:CustLabel ID="lblBusinessAddress" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040301_020"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                    </tr>
                    
                    <tr class="trOdd">
                        <td colspan="8">
                            <cc1:CustCheckBox ID="chkAddress" runat="server" OnCheckedChanged="chkAddress_CheckedChanged" AutoPostBack="True"/>
                            <cc1:CustLabel ID="lblAdd" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040301_055"
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
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040301_059"
                                    StickHeight="False"></cc1:CustLabel>
                            <cc1:CustLabel ID="lblZipText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                    StickHeight="False" Width="50px"></cc1:CustLabel>        
                            <cc1:CustTextBox ID="txtBusinessAddr4" runat="server"
                                MaxLength="6" Width="100px" onkeydown="entersubmit('btnUpdate');"  onblur="changeFullType(this);" BoxName="營業地址一" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtBusinessAddr5" runat="server" 
                                MaxLength="14" Width="220px" onkeydown="entersubmit('btnUpdate');"  onblur="changeFullType(this);" BoxName="營業地址二" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtBusinessAddr6" runat="server"
                                MaxLength="7" Width="100px" onkeydown="entersubmit('btnUpdate');"  onblur="changeFullType(this);" BoxName="營業地址三" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                            <cc1:CustButton ID="btnSearchZip" runat="server" CssClass="smallButton" ShowID="01_01040301_088"                            
                            DisabledWhenSubmit="False" OnClick="btnSearchZip_Click" />

                         </td>       
                    </tr>
                    
                    <tr class="trOdd">
                        <td align="right" style="width: 12%">
                            <cc1:CustLabel ID="lblJCIC" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040301_043"
                                StickHeight="False" ForeColor="Red"></cc1:CustLabel>
                                
                        </td>
                        <td style="width: 88%" colspan="8">
                            <cc1:CustTextBox ID="txtJCIC" runat="server" MaxLength="1" Width="100px" onkeydown="entersubmit('btnUpdate');" checktype="numandletter" BoxName="JCIC查詢"></cc1:CustTextBox>
                                <cc1:CustLabel ID="lblMessage1" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040301_031"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                    </tr>
                    
                    <tr class="trEven">
                        <td align="right" style="width: 12%" rowspan="2">
                            <cc1:CustLabel ID="lblAccounts" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040301_044"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblBank" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_021" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 33%" colspan="3">
                            <cc1:CustTextBox ID="txtBank" runat="server" MaxLength="5" 
                                Width="80px" onkeydown="entersubmit('btnUpdate');"  onblur="changeFullType(this);" BoxName="銀行(中文)" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                        </td>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblBranchBank" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_022" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 33%" colspan="3">
                            <cc1:CustTextBox ID="txtBranchBank" runat="server" MaxLength="10" 
                                Width="140px" onkeydown="entersubmit('btnUpdate');"  onblur="changeFullType(this);" BoxName="分行(中文)" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                        </td>
                    </tr>
                    
                    <tr class="trEven">
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_023" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 88%" colspan="8">
                            <cc1:CustTextBox ID="txtName" runat="server" MaxLength="20" 
                                Width="300px" onkeydown="entersubmit('btnUpdate');"  onblur="changeFullType(this);" BoxName="戶名" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                        </td>
                    </tr>
                    
                    <tr class="trOdd">
                        <td align="right" style="width: 12%; height: 33px;">
                            <cc1:CustLabel ID="lblPrev" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040301_045"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        
                        <td align="right" style="width: 11%; height: 33px;">
                            <cc1:CustLabel ID="lblPrevDesc" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040301_046"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 33%; height: 33px;" colspan="3">
                            <cc1:CustTextBox ID="txtPrevDesc" runat="server" MaxLength="4" 
                                Width="100px" onkeydown="entersubmit('btnUpdate');"  onblur="changeFullType(this);" BoxName="帳單內容" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                        </td>
                        
                        <td align="right" style="width: 11%; height: 33px;">
                            <cc1:CustLabel ID="lblInvoiceCycle" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040301_025"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%; height: 33px;">
                            <cc1:CustTextBox ID="txtInvoiceCycle" runat="server" MaxLength="2" checktype="num"
                                Width="20px" onkeydown="entersubmit('btnUpdate');" BoxName="發票週期"></cc1:CustTextBox>
                        </td>
                        
                        <td align="right" style="width: 11%; height: 33px;">
                            <cc1:CustLabel ID="lblRedeemCycle" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040301_047"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%; height: 33px;">
                            <cc1:CustTextBox ID="txtRedeemCycle" runat="server" MaxLength="1" checktype="numandletter"
                                Width="30px" onkeydown="entersubmit('btnUpdate');" BoxName="紅利週期(M/D)"></cc1:CustTextBox>
                        </td>
                    </tr>
                    
                    <tr class="trEven">
                        <td align="right" style="width: 12%">
                            <cc1:CustLabel ID="lblPopMan" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_024" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 88%" colspan="8">
                            <cc1:CustTextBox ID="txtPopMan" runat="server" onkeydown="entersubmit('btnUpdate');"  onblur="changeFullType(this);" MaxLength="3"
                                Width="50px" BoxName="推廣員" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
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
                            <cc1:CustButton ID="btnUpdate" runat="server" CssClass="smallButton" ShowID="01_01040301_026"
                                OnClientClick="return checkInputText('pnlText', 1);" DisabledWhenSubmit="False"
                                onkeydown="setfocus('txtCardNo1');" OnClick="btnUpdate_Click" />              
                        </td>
                    </tr>
                </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
            <cc1:CustButton ID="btnUpdateHiden" runat="server" CssClass="btnHiden"
            DisabledWhenSubmit="False" OnClick="btnUpdateHiden_Click" OnClientClick="return checkAdd();"></cc1:CustButton>
        <cc1:CustButton ID="btnForce" runat="server" CssClass="btnHiden" OnClientClick="return checkInputText('tabNo1', 0);"
            DisabledWhenSubmit="False" OnClick="btnForce_Click" />
    </form>
</body>
</html>
