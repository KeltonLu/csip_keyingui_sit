<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010101130001.aspx.cs" Inherits="P010101130001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%--2021/04/09_Ares_Stanley-調整在正卡人ID及收件編號以外的輸入框按下ENTER時觸發提交按鈕--%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<title></title>

<script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

<script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

<script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

<script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

<script type="text/javascript" src="../Common/Script/JQuery/json3-3.3.2.min.js"></script>

<link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

<script type="text/javascript" language="javascript">

$(document).ready(function() {
    document.getElementById("ckEtagFlag").focus();
});

// 初始欄位
function initialText(){
    document.getElementById("ckEtagFlag").checked = false;
    document.getElementById("ckTemplateParkFlag").checked = false;
    document.getElementById("ckMonthlyParkFlag").checked = false;
    document.getElementById("txtPrimaryCardID").value = "";
    document.getElementById("txtName").value = "";
    document.getElementById("txtReceiveNumber").value = "";
    document.getElementById("txtMobilePhone").value = "";
    document.getElementById("txtApplyType").value = "";
    document.getElementById("txtPlateNO1_1").value = "";
    document.getElementById("txtPlateNO1_2").value = "";
    document.getElementById("txtOwnersID").value = "";
    document.getElementById("txtMonthlyParkingNO").value = "";
    document.getElementById("txtPopluNO").value = "";
    document.getElementById("txtPopulEmpNO").value = "";
    document.getElementById("txtIntroducerCardID").value = "";
    document.getElementById("txtPlateNO2_1").value = "";
    document.getElementById("txtPlateNO2_2").value = "";
    document.getElementById("txtPlateNO3_1").value = "";
    document.getElementById("txtPlateNO3_2").value = "";
    document.getElementById("txtPlateNO4_1").value = "";
    document.getElementById("txtPlateNO4_2").value = "";
}

function hideText(){
    $("#txtReceiveNumber").addClass("btnHiden");
    $("#txtMobilePhone").addClass("btnHiden");
    $("#txtApplyType").addClass("btnHiden");
    $("#txtPlateNO1_1").addClass("btnHiden");
    $("#txtPlateNO1_2").addClass("btnHiden");
    $("#txtOwnersID").addClass("btnHiden");
    $("#txtMonthlyParkingNO").addClass("btnHiden");
    $("#txtPopluNO").addClass("btnHiden");
    $("#txtPopulEmpNO").addClass("btnHiden");
    $("#txtPlateNO2_1").addClass("btnHiden");
    $("#txtPlateNO2_2").addClass("btnHiden");
    $("#txtPlateNO3_1").addClass("btnHiden");
    $("#txtPlateNO3_2").addClass("btnHiden");
    $("#txtPlateNO4_1").addClass("btnHiden");
    $("#txtPlateNO4_2").addClass("btnHiden");
}

// 勾選 月租停車費代繳
function setMonthlyFee(){
    var isMonthlyParkFlagChecked = document.getElementById("ckMonthlyParkFlag").checked;
    document.getElementById("CustLabel8").style.color = 'black';
   
    if(isMonthlyParkFlagChecked){
        document.getElementById("CustLabel8").style.color = 'red';
    }
}

function WordUpperCase(obj){
    obj.value =obj.value.toUpperCase();
}

// 提交 欄位 檢核
function checkInputText()
{
    var etagFlag = document.getElementById("ckEtagFlag");                       // ETAG儲值
    var templateParkFlag = document.getElementById("ckTemplateParkFlag");       // 臨時停車
    var monthlyParkFlag = document.getElementById("ckMonthlyParkFlag");         // 月租停車
    var monthlyParkingNO = document.getElementById("txtMonthlyParkingNO").value.Trim();
    
    if(!etagFlag.checked && !templateParkFlag.checked && !monthlyParkFlag.checked){
        alert('至少選取一種業務類別');
        etagFlag.focus();
        return false;
    }
    
    // 正卡人ID 檢核
    if(!checkEmptyORLength('txtPrimaryCardID', '正卡人ID', 10, true)){
        return false;
    }

    // 收件編號 檢核
    if(!checkEmptyORLength('txtReceiveNumber', '收件編號', 13, true)){
        return false;
    }
    
    // 申請別 檢核
    if(!checkApplyType()){
        return false;
    }
    
    // 車牌號碼 檢核
    if(!checkPlateNO('txtPlateNO1_1', 'txtPlateNO1_2', '車牌號碼(前半)', '車牌號碼(後半)', true)){
        return false;
    }
    
    // 車主ID或統編 檢核
    if(!checkEmptyORLength('txtOwnersID', '車主ID或統編', 8, true)){
        return false;
    }
    
    if(monthlyParkingNO != ""){
        if(!checkEmptyORLength('txtMonthlyParkingNO', '停車場代碼', 8, true)){
            return false;
        }
    }
    
    // 月租停車廠代碼 檢核
    if(monthlyParkFlag.checked){
        if(!checkEmptyORLength('txtMonthlyParkingNO', '停車場代碼', 8, true)){
            return false;
        }
    }
    
    // 車牌號碼2 檢核
    if(!checkPlateNO('txtPlateNO2_1', 'txtPlateNO2_2', '車牌號碼2(前半)', '車牌號碼2(後半)', false)){
        return false;
    }
    
    // 車牌號碼3 檢核
    if(!checkPlateNO('txtPlateNO3_1', 'txtPlateNO3_2', '車牌號碼3(前半)', '車牌號碼3(後半)', false)){
        return false;
    }
    
    // 車牌號碼4 檢核
    if(!checkPlateNO('txtPlateNO4_1', 'txtPlateNO4_2', '車牌號碼4(前半)', '車牌號碼4(後半)', false)){
        return false;
    }


    if(document.getElementById("txtMonthlyParkingNO").value.Trim() != "" && !monthlyParkFlag.checked){
        alert("月租停車選項不勾選，則停車場代碼須為空");
        return false;
    }

    for(i=1;i<=4;i++){
　      
　      for(j=1;j<=4;j++){
　          
　          if(i == j){
　              continue;
　          }
　      
　          var no1 = document.getElementById("txtPlateNO" + i + "_1").value.Trim() + document.getElementById("txtPlateNO" + i + "_2").value.Trim();
　          var no2 = document.getElementById("txtPlateNO" + j + "_1").value.Trim() + document.getElementById("txtPlateNO" + j + "_2").value.Trim();
　          
　          if(no1 != "" && no2 != "" && no1 == no2){
　              
　              alert("車牌號碼" + i + "和車牌號碼" + j + "相同");
                return false;
　          }
　          　      
　      }
　        
    }

    return true;
}

// 檢核必填及長度
function checkEmptyORLength(domID, txtID, txtLength, mustKeyIn){
    var d = document.getElementById(domID);
    
    if(mustKeyIn){
        if(d.value.Trim() == ""){
            alert(txtID + ' 必填');
            d.focus();
            return false;
        }
    }
    
    if(d.value.Trim() != ""){
        if(d.value.Trim().length < txtLength){
            alert(txtID + ' 格式不正確');
            d.focus();
            return false;
        }
    }
    
    return true;
}

// 申請別 檢核
function checkApplyType(){
    var applyType = document.getElementById("txtApplyType");    // 申請別(1.申請 2.終止)
    
    if(applyType.value.Trim() == ""){
        alert('申請別 必填');
        applyType.focus();
        return false;
    }
    
    if(applyType.value.Trim() != "1" && applyType.value.Trim() != "2"){
        alert('申請別 只能輸入 1 及 2');
        applyType.focus();
        return false;
    }
    
    return true;
}

// 車牌號碼 檢核
function checkPlateNO(domID1, domID2, txtID1, txtID2, mustKeyIn){
    
    var plateNO1 = document.getElementById(domID1);
    var plateNO2 = document.getElementById(domID2);
    
    if(mustKeyIn){
        if(plateNO1.value.Trim().length < 1){
            alert(txtID1 + ' 格式不正確');
            plateNO1.focus();
            return false;
        }
        
        if(plateNO2.value.Trim().length < 1){
            alert(txtID2 + ' 格式不正確');
            plateNO2.focus();
            return false;
        }
    } else {
         if(plateNO1.value.Trim() != '' || plateNO2.value.Trim() != ''){
            if(plateNO1.value.Trim().length < 1){
                alert(txtID1 + ' 格式不正確');
                plateNO1.focus();
                return false;
            }
        
            if(plateNO2.value.Trim().length < 1){
                alert(txtID2 + ' 格式不正確');
                plateNO2.focus();
                return false;
            }
        }
    }
    
    return true;
}
    // 在正卡人ID及收件編號以外的輸入框按下ENTER時觸發提交按鈕
    function otherEnter() {
        if (event.keyCode == 13) {
            document.getElementById('btnSubmit').click();
            event.preventDefault();
        }
    }
</script>

<style type="text/css">
    .btnHiden { display:none; }
</style>
</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <cust:image runat="server" ID="image1" />
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1">
                        <tr class="itemTitle1">
                            <td colspan="2">
                                <li>
                                    <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" Width="200px" ShowID="01_01011300_001" />
                                </li>
                            </td>
                        </tr>
                        <%--業務類別--%>
                        <tr class="trOdd">
                            <td style="width: 20%" align="right">
                                <cc1:CustLabel ID="lblFormCategory" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False"
                                    ShowID="01_01011300_002" />
                            </td>
                            <td style="width: 80%">
                                <cc1:CustCheckBox ID="ckEtagFlag" runat="server" Text="ETAG儲值" Checked="true" />&nbsp;&nbsp;
                                <cc1:CustCheckBox ID="ckTemplateParkFlag" runat="server" Text="臨時停車" />
                                <cc1:CustCheckBox ID="ckMonthlyParkFlag" runat="server" Text="月租停車" onclick="setMonthlyFee();" />
                            </td>
                        </tr>
                        <%--正卡人ID--%>
                        <tr class="trEven">
                            <td style="width: 20%" align="right">
                                <cc1:CustLabel ID="lblBusinessCategory" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False"
                                    ShowID="01_01011300_007" Style="color: Red" />
                            </td>
                            <td style="width: 80%">
                                <cc1:CustTextBox ID="txtPrimaryCardID" runat="server" MaxLength="10" checktype="ID" onkeydown="entersubmit('btnSelect');" BoxName="正卡人ID" onkeypress="WordUpperCase(this);" />
                                <cc1:CustButton ID="btnSelect" CssClass="smallButton" Width="40px" runat="server" ShowID="01_01011300_006" DisabledWhenSubmit="False" 
                                    OnClick="btnSelect_Click" style="display:none" />
                            </td>
                        </tr>
                        <%--姓名--%>
                        <tr class="trOdd">
                            <td style="width: 20%" align="right">
                                <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False"
                                    ShowID="01_01011300_008" Style="color: Red" />
                            </td>
                            <td style="width: 80%">
                                <cc1:CustTextBox ID="txtName" runat="server" MaxLength="1" checktype="num" BoxName="姓名" disabled></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--收件編號--%>
                        <tr class="trEven">
                            <td style="width: 20%" align="right">
                                <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False"
                                    ShowID="01_01011300_009" Style="color: Red" />
                            </td>
                            <td style="width: 80%">
                                <cc1:CustTextBox ID="txtReceiveNumber" runat="server" MaxLength="13" checktype="num" onkeydown="entersubmit('btnSelect');" BoxName="收件編號"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--申請別(1.申請 2.終止)--%>
                        <tr class="trOdd">
                            <td style="width: 20%" align="right">
                                <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False"
                                    ShowID="01_01011300_011" Style="color: Red" />
                            </td>
                            <td style="width: 80%">
                                <cc1:CustTextBox ID="txtApplyType" runat="server" MaxLength="1" checktype="num" BoxName="申請別" onkeypress="otherEnter();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--車牌號碼--%>
                        <tr class="trEven">
                            <td style="width: 20%" align="right">
                                <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False"
                                    ShowID="01_01011300_012" Style="color: Red" />
                            </td>
                            <td style="width: 80%">
                                <cc1:CustTextBox ID="txtPlateNO1_1" runat="server" MaxLength="4" BoxName="車牌號碼" onkeypress="otherEnter();"></cc1:CustTextBox>-
                                <cc1:CustTextBox ID="txtPlateNO1_2" runat="server" MaxLength="4" BoxName="車牌號碼" onkeypress="otherEnter();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--車主ID或統編--%>
                        <tr class="trOdd">
                            <td style="width: 20%" align="right">
                                <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False"
                                    ShowID="01_01011300_013" Style="color: Red" />
                            </td>
                            <td style="width: 80%">
                                <cc1:CustTextBox ID="txtOwnersID" runat="server" MaxLength="10" BoxName="車主ID或統編" onkeypress="otherEnter();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--推廣代號--%>
                        <tr class="trEven">
                            <td style="width: 20%" align="right">
                                <cc1:CustLabel ID="CustLabel9" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False"
                                    ShowID="01_01011300_015" />
                            </td>
                            <td style="width: 80%">
                                <cc1:CustTextBox ID="txtPopluNO" runat="server" MaxLength="5" checktype="num" BoxName="推廣代號" onkeypress="otherEnter();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--推廣員編--%>
                        <tr class="trOdd">
                            <td style="width: 20%" align="right">
                                <cc1:CustLabel ID="CustLabel10" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False"
                                    ShowID="01_01011300_016" />
                            </td>
                            <td style="width: 80%">
                                <cc1:CustTextBox ID="txtPopulEmpNO" runat="server" MaxLength="10" BoxName="推廣員編" onkeypress="otherEnter();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--種子--%>
                        <tr class="trEven">
                            <td style="width: 20%" align="right">
                                <cc1:CustLabel ID="CustLabel11" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False"
                                    ShowID="01_01011300_017" />
                            </td>
                            <td style="width: 80%">
                                <cc1:CustTextBox ID="txtIntroducerCardID" runat="server" MaxLength="16" BoxName="種子" onkeypress="otherEnter();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--停車場代碼--%>
                        <tr class="trOdd">
                            <td style="width: 20%" align="right">
                                <cc1:CustLabel ID="CustLabel8" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False"
                                    ShowID="01_01011300_014" />
                            </td>
                            <td style="width: 80%">
                                <cc1:CustTextBox ID="txtMonthlyParkingNO" runat="server" MaxLength="8" BoxName="停車場代碼" onkeypress="otherEnter();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--車牌號碼2--%>
                        <tr class="trEven">
                            <td style="width: 20%" align="right">
                                <cc1:CustLabel ID="CustLabel12" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False"
                                    ShowID="01_01011300_018" />
                            </td>
                            <td style="width: 80%">
                                <cc1:CustTextBox ID="txtPlateNO2_1" runat="server" MaxLength="4" BoxName="車牌號碼2" onkeypress="otherEnter();"></cc1:CustTextBox>-
                                <cc1:CustTextBox ID="txtPlateNO2_2" runat="server" MaxLength="4" BoxName="車牌號碼2" onkeypress="otherEnter();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--車牌號碼3--%>
                        <tr class="trOdd">
                            <td style="width: 20%" align="right">
                                <cc1:CustLabel ID="CustLabel13" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False"
                                    ShowID="01_01011300_019" />
                            </td>
                            <td style="width: 80%">
                                <cc1:CustTextBox ID="txtPlateNO3_1" runat="server" MaxLength="4" BoxName="車牌號碼3" onkeypress="otherEnter();"></cc1:CustTextBox>-
                                <cc1:CustTextBox ID="txtPlateNO3_2" runat="server" MaxLength="4" BoxName="車牌號碼3" onkeypress="otherEnter();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--車牌號碼4--%>
                        <tr class="trEven">
                            <td style="width: 20%" align="right">
                                <cc1:CustLabel ID="CustLabel14" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False"
                                    ShowID="01_01011300_020" />
                            </td>
                            <td style="width: 80%">
                                <cc1:CustTextBox ID="txtPlateNO4_1" runat="server" MaxLength="4" BoxName="車牌號碼4" onkeypress="otherEnter();"></cc1:CustTextBox>-
                                <cc1:CustTextBox ID="txtPlateNO4_2" runat="server" MaxLength="4" BoxName="車牌號碼4" onkeypress="otherEnter();"></cc1:CustTextBox>
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1">
                        <tr class="itemTitle1">
                            <td align="center">
                                <cc1:CustButton ID="btnSubmit" CssClass="smallButton" Width="40px" runat="server" DisabledWhenSubmit="False" ShowID="01_01011300_006" Enabled="false" 
                                    OnClick="btnSubmit_Click" OnClientClick="return checkInputText();"/>
                                    
                                <cc1:CustButton ID="btnCancel" CssClass="smallButton" Width="40px" runat="server" DisabledWhenSubmit="False" ShowID="01_01011300_021" OnClick="btnCancel_Click" 
                                     />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
</body>
</html>
