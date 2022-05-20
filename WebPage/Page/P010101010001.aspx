<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010101010001.aspx.cs" Inherits="P010101010001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%--20210329_Ares_Stanley-調整半形轉全形失效; 20210401_Ares_Stanley-管理郵區, 郵遞區號; 20210408_Ares_Stanley-調整半形轉全形失效; 20210415_Ares_Stanley-調整全半形轉換---%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />
    <style type="text/css">
   .btnHiden
    {display:none; }
    </style>

    <script type="text/javascript" language="javascript">
        
        //*客戶端檢核
        function checkInputText(id,intType)
        {
            //*檢核輸入欄位身份證號碼是否為空
             if(document.getElementById('txtUserId').value.Trim() == "")
            {
                alert('請輸入身份證號碼后點選查詢按鈕');
                setControlsDisabled('pnlText');
                document.getElementById('lblNameText').innerText="";
                document.getElementById('txtUserId').focus();
                return false;
            }
            
             if(!checkInputType(id))
            {
                return false;
            } 
                     
            if(intType == 1)
            {
               //*檢核查詢部分欄位輸入規則
               if(!checkInputType('tabNo1'))
               {
                    return false;
               }           
                
                //*檢核住家電話是否為空
                if(document.getElementById('txtHomeTel1').value != "" && document.getElementById('txtHomeTel2').value == "")
                {
                    alert('（住家電話）電話號碼不能為空白');
                    document.getElementById('txtHomeTel2').focus();
                    return false;
                }
                if(document.getElementById('txtHomeTel1').value == "" && document.getElementById('txtHomeTel2').value != "")
                {
                    alert('（住家電話）區碼不能為空白');
                    document.getElementById('txtHomeTel1').focus();
                    return false;
                }
                
                //*檢核公司電話是否為空
                if(document.getElementById('txtCompanyTel1').value != "" && document.getElementById('txtCompanyTel2').value == "")
                {
                    alert('（公司電話）電話號碼不能為空白');
                    document.getElementById('txtHomeTel2').focus();
                    return false;
                }
                if(document.getElementById('txtCompanyTel1').value == "" && document.getElementById('txtCompanyTel2').value != "")
                {
                    alert('（公司電話）區碼不能為空白');
                    document.getElementById('txtCompanyTel1').focus();
                    return false;
                }
                
                //*檢核住家電話規則
                if(!checkTelphone('txtHomeTel1','txtHomeTel2'))
                {
                    return false;
                }

                //*檢核TempEmail 20211022 By Ares Stanley
                var email = document.getElementById('txtTempEmail').value;
                if (email.length > 1) {
                    var emailRule = new RegExp(/^[+a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/i);
                    if (!emailRule.test(email)) {
                        alert('E-Mail錯誤!');
                        document.getElementById('txtTempEmail').focus();
                        return false;
                    }
                }
                
                //*檢核公司電話規則
//                if(!checkTelphone('txtCompanyTel1','txtCompanyTel2'))
//                {
//                    return false;
//                }
                
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
               setControlsDisabled('pnlText');
               document.getElementById('lblNameText').innerText = "";
               document.getElementById('lblTempEmailStatus').innerText = "";
            
           }
         
            document.getElementById("txtUserIdHide").value=document.getElementById("txtUserId").value;
        }
        
         //*檢核電話輸入規則
        function checkTelphone(id1,id2)
        {
             switch(document.getElementById(id1).value)
                {
                    case "02":
                        if(document.getElementById(id2).value.length != 8)
                        {
                            alert('區碼02，電話號碼必須有八碼');
                            document.getElementById(id1).focus();
                            return false;
                        }
                        break;
                    case "070":
                    if(document.getElementById(id2).value.length != 8)
                    {
                        alert('區碼070，電話號碼必須有八碼');
                        document.getElementById(id1).focus();
                        return false;
                    }
                    break;
                    case "037":
                    case "049":
                    case "089":
                        if(document.getElementById(id2).value.length != 6)
                        {
                            alert('區碼037、049、089，電話號碼必須有六碼');
                            document.getElementById(id1).focus();
                            return false;
                        }
                        break;
                        
                    case "083":
                         if(document.getElementById(id2).value.length != 6)
                        {
                            alert('區碼083，電話號碼必須有六碼');
                            document.getElementById(id1).focus();
                            return false;
                        }
                        if(document.getElementById(id2).value.substring(0,1) != "6")
                        {
                             alert('區碼083，電話號碼第一個字必須是6');
                             document.getElementById(id1).focus();
                             return false;
                        }
                        break;
                    case "082":
                        if(document.getElementById(id2).value.length != 6)
                        {
                            alert('區碼082，電話號碼必須有六碼');
                            document.getElementById(id1).focus();
                            return false;
                        }
                        if(!(document.getElementById(id2).value.substring(0,1) == "3" || document.getElementById(id2).value.substring(0,1) == "6" || document.getElementById(id2).value.substring(0,1) == "7"))
                        {
                            alert('區碼082，電話號碼第一個字必須是3、6、7');
                            document.getElementById(id1).focus();
                            return false;
                        }
                        break;
                        /*
                     default:
                        if(document.getElementById(id2).value.length != 7)
                        {
                            alert('電話號碼必須有七碼');
                            document.getElementById(id1).focus();
                            return false;
                        }
                        break;  
                        */               
                }
                return true;
        }

        function openDialog() {
            $("#dialog-confirm").dialog("open");
        }

        $(function () {
            $("#dialog-confirm").dialog({
                autoOpen: false,
                resizable: false,
                draggable: false,
                closeOnEscape: false,
                height: "auto",
                width: 400,
                position: { at: "top" },
                modal: true,
                buttons: {
                    "是": function () {
                        document.getElementById("hidSendMailVerifyFlag").value = "T";
                        $("#btnHiden").click();
                    },
                    "否": function () {
                        document.getElementById("hidSendMailVerifyFlag").value = "F";
                        $("#btnHiden").click();
                    },
                    "取消": function () {
                        document.getElementById("hidSendMailVerifyFlag").value = "";
                        $(this).dialog("close");
                    }
                }
            });
        });

    </script>

    <style type="text/css">
   .btnHiden
    {display:none; }

   .ui-dialog
   {
       background-color:white;
        box-shadow:0px 0px 2px 1px rgb(0 0 0 / 20%);
        border-radius:3px;
        padding:1%;
   }
   .ui-dialog-buttonpane
   {
       padding-left:30%;
   }
   .ui-dialog-buttonpane button
   {
       margin-right:5%;
   }
   .ui-dialog-titlebar-close
   {
       display:none;
   }
    </style>
</head>
<body class="workingArea">
    <div id="dialog-confirm" title="請問是否發給客戶進行EMAIL驗證">
  <p><span class="ui-icon ui-icon-alert" style="float:left; margin:12px 12px 20px 0;"></span></p>
</div>
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
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010100_001" StickHeight="False"
                                    Width="240px"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="width: 15%" align="right">
                            <cc1:CustLabel ID="lblUserId" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01010100_002" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtUserId" runat="server" MaxLength="12" checktype="ID" onkeydown="entersubmit('btnSelect');"
                                onkeyup="ChangeEnable();" BoxName="身分證號碼"></cc1:CustTextBox><cc1:CustTextBox ID="txtUserIdHide" runat="server"
                                    MaxLength="10" CssClass="btnHiden" Text=""></cc1:CustTextBox>&nbsp;&nbsp;&nbsp;&nbsp;
                            <cc1:CustButton ID="btnSelect" CssClass="smallButton" runat="server" Width="40px"
                                OnClick="btnSelect_Click" OnClientClick="return checkInputText('tabNo1',0);"
                                DisabledWhenSubmit="False" ShowID="01_01010100_003" onkeydown="setfocuschoice('txtUserId','txtPostalDistrict');" />
                            <asp:Button ID="btnCheckEsbHtgStatus" runat="server" OnClick="btnCheckEsbHtgStatus_Click" CssClass="btnHiden"/>
                        </td>
                        <td style="width: 15%" align="right">
                            <cc1:CustLabel ID="lblName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01010100_004" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            &nbsp;&nbsp;<cc1:CustLabel ID="lblNameText" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False" Width="160px" IsColon="False"
                                ShowID=""></cc1:CustLabel>
                        </td>
                    </tr>
                    <tr>
                        <td nowrap colspan="4" style="height: 1px">
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td colspan="4">
                        </td>
                    </tr>
                </table>
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo2" style="">
                        <tr class="trOdd">
                            <td style="width: 15%;" align="right">
                                <cc1:CustLabel ID="lblPostalDistrict" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01010100_011"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3">
                                <cc1:CustTextBox ID="txtPostalDistrict" runat="server" checktype="num" onkeydown="entersubmit('btnSubmit');"
                                    Width="200px" MaxLength="3" BoxName="管理郵區" ReadOnly="true"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%;" align="right">
                                <cc1:CustLabel ID="lblZipCode" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010100_012" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3">
                                <cc1:CustTextBox ID="txtZipCode" runat="server" checktype="num" onkeydown="entersubmit('btnSubmit');"
                                    MaxLength="3" Width="200px" BoxName="郵遞區號" ReadOnly="true"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%;" align="right">
                                <cc1:CustLabel ID="lblCityName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010100_005" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3">
                                <cc1:CustTextBox ID="txtCityName" runat="server" onkeydown="entersubmit('btnSubmit');" onblur="changeFullType(this);" onpaste="paste();"
                                    Width="200px" MaxLength="6" checktype="fulltype" BoxName="城市名"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%;" align="right">
                                <cc1:CustLabel ID="lblMerchantAddOne" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01010100_006"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%;">
                                <cc1:CustTextBox ID="txtMerchantAddOne" runat="server" onkeydown="entersubmit('btnSubmit');" onblur="changeFullType(this);" onpaste="paste();"
                                    Width="200px" MaxLength="14" checktype="fulltype" BoxName="帳單地址一"></cc1:CustTextBox>
                            </td>
                            <td style="width: 15%;" align="right">
                                <cc1:CustLabel ID="lblMerchantAddTwo" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01010100_007"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%;">
                                <cc1:CustTextBox ID="txtMerchantAddTwo" runat="server" onkeydown="entersubmit('btnSubmit');" onblur="changeFullType(this);" onpaste="paste();"
                                    Width="200px" MaxLength="14" checktype="fulltype" BoxName="帳單地址二"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%;" align="right">
                                <cc1:CustLabel ID="lblCompanyName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010100_008" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3">
                                <cc1:CustTextBox ID="txtCompanyName" runat="server" onkeydown="entersubmit('btnSubmit');" onblur="changeFullType(this);" onpaste="paste();"
                                    Width="200px" MaxLength="14" checktype="fulltype" BoxName="公司名稱"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%;" align="right">
                                <cc1:CustLabel ID="lblHomeTel" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010100_009" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%;">
                                <cc1:CustTextBox ID="txtHomeTel1" runat="server" checktype="num" onkeydown="entersubmit('btnSubmit');"
                                    Width="50px" MaxLength="3" BoxName="住家電話一"></cc1:CustTextBox>
                                &nbsp;<cc1:CustTextBox ID="txtHomeTel2" runat="server" checktype="telphone" onkeydown="entersubmit('btnSubmit');"
                                    Width="150px" MaxLength="14" BoxName="住家電話二"></cc1:CustTextBox>
                            </td>
                            <td style="width: 15%;" align="right">
                                <cc1:CustLabel ID="lblCompanyTel" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010100_010" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%;">
                                <cc1:CustTextBox ID="txtCompanyTel1" runat="server" checktype="num" onkeydown="entersubmit('btnSubmit');"
                                    Width="50px" MaxLength="3" BoxName="公司電話一"></cc1:CustTextBox>
                                &nbsp;<cc1:CustTextBox ID="txtCompanyTel2" runat="server" checktype="telphone" onkeydown="entersubmit('btnSubmit');"
                                    Width="150px" MaxLength="14" BoxName="公司電話二"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%;" align="right">
                                <cc1:CustLabel ID="lblRegTel" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010100_016" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%;">
                                <cc1:CustTextBox ID="txtRegTel1" runat="server" checktype="num" onkeydown="entersubmit('btnSubmit');"
                                    Width="50px" MaxLength="3" BoxName="戶籍電話一"></cc1:CustTextBox>
                                &nbsp;<cc1:CustTextBox ID="txtRegTel2" runat="server" checktype="telphone" onkeydown="entersubmit('btnSubmit');"
                                    Width="150px" MaxLength="14" BoxName="戶籍電話二"></cc1:CustTextBox>
                            </td>
                            <td style="width: 15%;" align="right">
                                <cc1:CustLabel ID="lblMobile" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010100_017" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%;">
                                <cc1:CustTextBox ID="txtMobile" runat="server" checktype="num" onkeydown="entersubmit('btnSubmit');"
                                    Width="200px" MaxLength="10" BoxName="行動電話"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%;" align="right">
                                <cc1:CustLabel ID="lblEmail" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010100_018" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width:35%;">
                                <cc1:CustTextBox ID="txtEmail" runat="server" checktype="email" onkeydown="entersubmit('btnSubmit');"
                                    Width="200px" MaxLength="50" BoxName="E-MAIL" ReadOnly="true" BackColor="LightGray"></cc1:CustTextBox>
                            </td>
                            <td style="width:15%;" align="right">
                                <cc1:CustLabel ID="lblTempEmail" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010100_022" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width:35%;">
                                <cc1:CustTextBox ID="txtTempEmail" runat="server" checktype="email" onkeydown="entersubmit('btnSubmit');"
                                    Width="200px" MaxLength="50" BoxName="待驗證E-MAIL"></cc1:CustTextBox>
                                <cc1:CustLabel ID="lblTempEmailStatus" runat="server" ForeColor="Red"></cc1:CustLabel>
                                <cc1:CustTextBox ID="hidSendMailVerifyFlag" runat="server" CssClass="btnHiden"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%;" align="right">
                                <cc1:CustLabel ID="lblBill" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010100_019" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3">
                                <cc1:CustTextBox ID="txtBill" runat="server" checktype="num" onkeydown="entersubmit('btnSubmit');"
                                    Width="200px" MaxLength="1" BoxName="電子帳單"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%;" align="right">
                                <cc1:CustLabel ID="lblRemarkOne" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010100_013" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3">
                                <cc1:CustTextBox ID="txtRemarkOne" runat="server" onkeydown="entersubmit('btnSubmit');"
                                    Width="300px" MaxLength="26" BoxName="注記一"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%;" align="right">
                                <cc1:CustLabel ID="lblRemarkTwo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010100_014" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3">
                                <cc1:CustTextBox ID="txtRemarkTwo" runat="server" onkeydown="entersubmit('btnSubmit');"
                                    Width="300px" MaxLength="26" BoxName="注記二"></cc1:CustTextBox>
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo4" style="">
                        <tr>
                            <td nowrap colspan="4" style="height: 1px">
                            </td>
                        </tr>
                        <tr class="itemTitle">
                            <td colspan="4" align="center">
                                <cc1:CustButton ID="btnSubmit" CssClass="smallButton" runat="server" Width="40px"
                                    OnClick="btnSubmit_Click" OnClientClick="return checkInputText('tabNo2',1);"
                                    onkeydown="setfocus('txtUserId');" DisabledWhenSubmit="False" ShowID="01_01010100_015" />
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <cc1:CustButton ID="btnHiden" OnClick="btnHiden_Click" runat="server" CssClass="btnHiden"
            DisabledWhenSubmit="False"></cc1:CustButton>
    </form>
</body>
</html>
