<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010105010001.aspx.cs" Inherits="Page_P010105010001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <%-- 20201123_Ares_Stanley-修正格式, 20210201_Ares_Stanley-調整版面; 20210329_Ares_Stanley-調整半形轉全形失效; 20210401_Ares_Stanley-調整下拉選單文字顯示; 20210408_Ares_Stanley-調整半形轉全形失效; 20210415_Ares_Stanley-調整全半形轉換; 20210902_Ares_Stanley:移除Email前30後19長度限制, 改為總長度50 --%>
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript">
         /*設置延時取得焦點 預設延 500毫秒 */
        function setfocus(elmid) {
            setTimeout(function () {
                //your code to be executed after 500 minisecond
                document.getElementById(elmid).focus();
            }, 500);

        }

        $(document).ready(function () {
        });

        function checkTaxData() {
            var taxID = document.getElementById('txtTaxID').value;
            if (taxID == "") {
                alert("請輸入 總公司/總店統編");
                document.getElementById('txtTaxID').focus();
                return false;
            }

            return true;
        }

        // 面頁邏輯檢核
        function checkInputText(id, intType) {

            mustKeyIn(intType);

            // 20210913_Ares_Stanley-增加Email檢核
            var emailF = document.getElementById('txtBasicOfficeEmail').value.Trim();
            if (emailF.length > 0) {
                if (document.getElementById('radBasicGmail').checked || document.getElementById('radBasicYahoo').checked ||
                    document.getElementById('radBasicHotmail').checked || document.getElementById('radBasicOther').checked) {
                    var emailB = '';
                    if (document.getElementById('radBasicGmail').checked) {
                        emailB = 'gmail.com';
                    }
                    else if (document.getElementById('radBasicYahoo').checked) {
                        emailB = 'yahoo.com.tw';
                    }
                    else if (document.getElementById('radBasicHotmail').checked) {
                        emailB = 'hotmail.com';
                    }
                    else {
                        emailB = document.getElementById('txtBasicOfficeEmailOther').value.Trim();
                    }

                    var email = emailF + '@' + emailB;
                    if (email.length > 1) {
                        var emailRule = new RegExp(/^[+a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/i);
                        if (!emailRule.test(email)) {
                            alert('E-Mail錯誤!');
                            document.getElementById('txtBasicOfficeEmail').focus();
                            return false;
                        }
                    }
                }
                else {
                    alert('請選擇E-Mail!');
                    return false;
                }
            }
        }

        function mustKeyIn(intType) {
            
            var isSuccess = true;

            //20210527 EOS_AML(NOVA) 增加欄位 by Ares Dennis
            //*資料最後異動分行不為空
            //if (document.getElementById('txtLAST_UPD_BRANCH').value.Trim() == "") {
            //    alert('資料最後異動分行不為空! ');
            //    document.getElementById('txtLAST_UPD_BRANCH').focus();
            //    return false;
            //}

            //*資料最後異動MAKER不為空
            //if (document.getElementById('txtLAST_UPD_MAKER').value.Trim() == "") {
            //    alert('資料最後異動MAKER不為空! ');
            //    document.getElementById('txtLAST_UPD_MAKER').focus();
            //    return false;
            //}

            //*資料最後異動CHECKER不為空
            //if (document.getElementById('txtLAST_UPD_CHECKER').value.Trim() == "") {
            //    alert('資料最後異動CHECKER不為空! ');
            //    document.getElementById('txtLAST_UPD_CHECKER').focus();
            //    return false;
            //}

          //  var begistyName = document.getElementById('txtRegistyName');
//            var basicTaxID = document.getElementById('txtBasicTaxID');
//            var basicRegistyNameCH = document.getElementById('txtBasicRegistyNameCH');
//            var basicRegistyNameEN = document.getElementById('txtBasicRegistyNameEN');
//            var basicAMLCC = document.getElementById('txtBasicAMLCC');
//            var basicAMLCCNameCH = document.getElementById('txtBasicAMLCCNameCH');
//            var basicEstablish = document.getElementById('txtBasicEstablish');
//            var basicCountryCode = document.getElementById('txtBasicCountryCode');
//            var basicCountryStateCode = document.getElementById('txtBasicCountryStateCode');
//            var basicBookAddr1 = document.getElementById('txtBasicBookAddr1');
//            var basicBookAddr2 = document.getElementById('txtBasicBookAddr2');
//            var basicBookAddr3 = document.getElementById('txtBasicBookAddr3');
//            var basicOfficePhone1 = document.getElementById('txtBasicOfficePhone1');
//            var basicOfficePhone2 = document.getElementById('txtBasicOfficePhone2');
//            var basicOfficePhone3 = document.getElementById('txtBasicOfficePhone3');
//            var bbasicContactMan = document.getElementById('txtBasicContactMan');
//            var basicContactAddr1 = document.getElementById('txtBasicContactAddr1');
//            var basicContactAddr2 = document.getElementById('txtBasicContactAddr2');
//            var basicContactAddr3 = document.getElementById('txtBasicContactAddr3');

//            if (isSuccess) {
//                isSuccess = validateIsEmpty(begistyName, "");
//            }

//            if (isSuccess) {
//                isSuccess = validateIsEmpty(basicTaxID, "");
//            }

//            if (isSuccess) {
//                isSuccess = validateIsEmpty(basicRegistyNameCH, "");
//            }

//            if (isSuccess) {
//                isSuccess = validateIsEmpty(basicRegistyNameEN, "");
//            }

//            if (isSuccess) {
//                isSuccess = validateIsEmpty(basicAMLCC, "");
//            }

//            if (isSuccess) {
//                isSuccess = validateIsEmpty(basicAMLCCNameCH, "");
//            }

//            if (isSuccess) {
//                isSuccess = validateIsEmpty(basicCountryStateCode, "");
//            }

//            if (isSuccess) {
//                isSuccess = validateIsEmpty(basicRegistyNameEN, "");
//            }

//            if (isSuccess) {
//                isSuccess = validateIsEmpty(basicRegistyNameEN, "");
//            }

//            if (isSuccess) {
//                isSuccess = validateIsEmpty(basicRegistyNameEN, "");
//            }

//            if (isSuccess) {
//                isSuccess = validateIsEmpty(basicRegistyNameEN, "");
//            }

//            if (isSuccess) {
//                isSuccess = validateIsEmpty(basicRegistyNameEN, "");
//            }

//            if (isSuccess) {
//                isSuccess = validateIsEmpty(basicRegistyNameEN, "");
//            }

        }

//        function validateIsEmpty(obj, msg) {
//            if (obj.value == "") {
//                alert
//            }
//        }
    </script>
    <style type="text/css">
        .btnHiden {
            display: none;
        }

        .auto-style1 {
            width: 100%;
            height: 25px;
        }
    </style>
</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <cust:image runat="server" ID="image1" />
        &nbsp;
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                    <tr class="itemTitle">
                        <td colspan="5">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01050100_001"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <%--總公司/總店統編--%>
                    <tr class="trOdd">
                        <%--總公司/總店統編--%>
                        <td align="right" style="width: 10%">
                            <cc1:CustLabel ID="lblTaxID1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01050100_002" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 10%">
                            <cc1:CustTextBox ID="txtTaxID" runat="server" MaxLength="8" checktype="num" Width="70px"
                                onkeydown="entersubmit('btnSelect');" BoxName="總公司/總店統編"></cc1:CustTextBox>
                        </td>
                        <%--分公司統編--%>
                        <%--<td align="right" style="width: 10%">
                            <cc1:CustLabel ID="CustLabel57" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01050100_062" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 10%">
                            <cc1:CustTextBox ID="txtStoreTaxID1" runat="server" MaxLength="8" checktype="num" Width="70px"
                                onkeydown="entersubmit('btnSelect');" BoxName="分公司統編"></cc1:CustTextBox>
                        </td>--%>
                        <%--序號--%>
                        <td align="right" style="width: 10%">
                            <cc1:CustLabel ID="lblTaxNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01050100_003" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 10%">
                            <cc1:CustTextBox ID="txtTaxNo" runat="server" MaxLength="4" checktype="num" Width="40px"
                                onkeydown="entersubmit('btnSelect');" BoxName="序號"></cc1:CustTextBox>
                        </td>

                        <%--查詢--%>
                        <td style="width: 60%">
                            <cc1:CustButton ID="btnSelect" runat="server" CssClass="smallButton" ShowID="01_01050100_004"
                                OnClientClick="return checkTaxData();" DisabledWhenSubmit="False" OnClick="btnSelect_Click" />
                             <%--20190614 Talas 增加提示此作業為新增，需檢核--%>
                             <cc1:CustLabel ID="lblNoticeNew" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" ForeColor ="Red"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                    </tr>
                </table>
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo4" style="">
                        <%--登記名稱--%>
                        <tr class="trEven">
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel58" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_005" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 90%" colspan="9">
                                <cc1:CustLabel ID="lblRegistyName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--總公司基本資料 必key--%>
                        <tr class="trOdd">
                            <%--總公司基本資料--%>
                            <td rowspan="9" align="right" style="width: 10%">
                                <cc1:CustLabel ID="lblName1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_006" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--統一編號--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="lblTaxID" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050100_007"
                                    StickHeight="False" Style="color: Red"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%">
                                <cc1:CustTextBox ID="txtBasicTaxID" runat="server" MaxLength="8" checktype="num"
                                    onkeydown="entersubmit('btnAdd');" Width="70px" BoxName="統一編號"></cc1:CustTextBox>
                            </td>
                            <%--登記名稱(中文)--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="lblRegistyNameCH" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050100_008"
                                    StickHeight="False" Style="color: Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 10%" colspan="6">
                                <cc1:CustTextBox ID="txtBasicRegistyNameCH" runat="server"  onblur="changeFullType(this);"
                                    onkeydown="entersubmit('btnAdd');" Width="200px" BoxName="登記名稱(中文)"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--登記名稱(英文)--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="lblRegistyNameEN" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_009"
                                    StickHeight="False" Style="color: Red"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%" colspan="8">
                                <cc1:CustTextBox ID="txtBasicRegistyNameEN" runat="server"
                                    onkeydown="entersubmit('btnAdd');" Width="200px" BoxName="登記名稱(英文)"></cc1:CustTextBox>
                            </td> 
                        </tr>
                        <tr class="trOdd">
                            <%--行業編號--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="lblAMLCC" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_010"
                                    StickHeight="False" Style="color: Red"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%">
                            
                                <cc1:CustHiddenField ID="hidCC" runat="server" />
                            
                                <cc1:CustTextBox ID="txtBasicAMLCC" runat="server"
                                    onkeydown="entersubmit('btnAdd');" MaxLength="7" Width="60px" BoxName="行業編號" AutoPostBack="true" OnTextChanged="txtBasicAMLCC_TextChanged"></cc1:CustTextBox>
                            </td>
                            <%--行業別中文名稱--%>
                            <td align="right" style="width: 10%" colspan="2">
                                <cc1:CustLabel ID="lblAMLCCName" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050100_011"
                                    StickHeight="False" Style="color: Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 10%" colspan="2">
                                <cc1:CustLabel ID="HQlblHCOP_CC_Cname" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--設立日期--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="lblEstablish" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050100_012"
                                    StickHeight="False" Style="color: Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 10%" colspan="2">
                                <cc1:CustTextBox ID="txtBasicEstablish" runat="server" MaxLength="7" checktype="num"
                                    onkeydown="entersubmit('btnAdd');" Width="60px" BoxName="設立日期"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--註冊國籍--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="lblCountryCode" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_013"
                                    StickHeight="False" Style="color: Red"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%">
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtBasicCountryCode" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                        onkeydown="entersubmit('btnAdd');" BoxName="註冊國籍"
                                        Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px; line-height:11px;" AutoPostBack="true" OnTextChanged="txtCodeType_TextChanged"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropBasicCountryCode" kind="select" runat="server" onclick="simOptionClick4IE('txtBasicCountryCode');"
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                    </cc1:CustDropDownList>
                                </div>
                            </td>
                            <%--註冊國為美國者，請勾選州別--%>
                            <td align="right" style="width: 10%" colspan="2">
                                <cc1:CustLabel ID="lblCountryStateCode" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050100_014"
                                    StickHeight="False" Style="color: Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 10%" colspan="5">
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtBasicCountryStateCode" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                        onkeydown="entersubmit('btnAdd');" BoxName="註冊國為美國者，請勾選州別"
                                        Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px; line-height:11px;"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropBasicCountryStateCode" kind="select" runat="server" onclick="simOptionClick4IE('txtBasicCountryStateCode');"
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                    </cc1:CustDropDownList>
                                </div>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--登記地址--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_015"
                                    StickHeight="False" Style="color: Red"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%" colspan="8">
                                <cc1:CustTextBox ID="txtREG_ZIP_CODE" runat="server" checktype="fulltype" MaxLength="7"
                                    Width="80px" onkeydown="entersubmit('btnAdd');"
                                    BoxName="登記地址郵遞區號"  Enabled="false" BackColor="LightGray" ></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtBasicBookAddr1" runat="server" checktype="fulltype" MaxLength="6"
                                    Width="100px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="登記地址一" onpaste="paste();" AutoPostBack="true" OnTextChanged="TextBox_AddrChanged"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtBasicBookAddr2" runat="server" checktype="fulltype" MaxLength="14"
                                    Width="220px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="登記地址二" onpaste="paste();"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtBasicBookAddr3" runat="server" checktype="fulltype" MaxLength="7"
                                    Width="100px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="登記地址三" onpaste="paste();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--公司電話--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050100_016"
                                    StickHeight="False" Style="color: Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 10%" colspan="2">
                                <cc1:CustTextBox ID="txtBasicOfficePhone1" runat="server" MaxLength="3" checktype="num"
                                    Width="25px" onkeydown="entersubmit('btnAdd');" BoxName="公司電話一"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtBasicOfficePhone2" runat="server" MaxLength="8" checktype="num"
                                    Width="58px" onkeydown="entersubmit('btnAdd');" BoxName="公司電話二"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtBasicOfficePhone3" runat="server" MaxLength="5" checktype="numx"
                                    Width="36px" onkeydown="entersubmit('btnAdd');" BoxName="公司電話三"></cc1:CustTextBox>
                            </td>
                            <%--聯絡人--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050100_017"
                                    StickHeight="False" Style="color: Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 10%" colspan="5">
                                <cc1:CustTextBox ID="txtBasicContactMan" runat="server" MaxLength="4" checktype="fulltype"
                                    Width="90px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="聯絡人"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--20190730 長姓名需求-聯絡人部份--%>
                        <tr class="trOdd">
                            <%--中文長姓名--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel57" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01030100_103" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 10%" colspan="4">
                                <cc1:CustCheckBox ID="chkisLongName_c" runat="server" OnCheckedChanged="CheckBox_CheckedChanged" AutoPostBack="True" BoxName="長姓名" />
                                <cc1:CustLabel ID="CustLabel60" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtBasicContactMan_L" runat="server" MaxLength="50" checktype="fulltype" Width="260px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="聯絡人長姓名"
                                    onpaste="paste();" AutoPostBack="true" OnTextChanged="TextBox_TextChanged"></cc1:CustTextBox>
                            </td>
                            <%--羅馬拼音--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel59" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 10%" colspan="3">
                                <cc1:CustTextBox ID="txtBasicContactMan_Pinyin" runat="server" MaxLength="50" checktype="fulltype" Width="260px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="聯絡人羅馬拼音"
                                    onpaste="paste();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--20190730 長姓名需求-聯絡人部份--%>
                        <tr class="trOdd">
                            <%--通訊地址--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_018"
                                    StickHeight="False" Style="color: Red"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%" colspan="8">
                                <cc1:CustTextBox ID="txtBasicContactAddr1" runat="server" checktype="fulltype" MaxLength="6"
                                    Width="100px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="通訊地址一" onpaste="paste();"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtBasicContactAddr2" runat="server" checktype="fulltype" MaxLength="14"
                                    Width="220px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="通訊地址二" onpaste="paste();"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtBasicContactAddr3" runat="server" checktype="fulltype" MaxLength="7"
                                    Width="100px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="通訊地址三" onpaste="paste();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--公司email--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_019"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%" colspan="8">
                                <cc1:CustTextBox ID="txtBasicOfficeEmail" runat="server" Width="200px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="E-MAIL"></cc1:CustTextBox>
                                @
                                <cc1:CustRadioButton ID="radBasicGmail" runat="server" AutoPostBack="False" GroupName="email" Text="gmail.com" />
                                <cc1:CustRadioButton ID="radBasicYahoo" runat="server" AutoPostBack="False" GroupName="email" Text="yahoo.com.tw" />
                                <cc1:CustRadioButton ID="radBasicHotmail" runat="server" AutoPostBack="False" GroupName="email" Text="hotmail.com" />
                                <cc1:CustRadioButton ID="radBasicOther" runat="server" AutoPostBack="False" GroupName="email" Text="其他：" />
                                <cc1:CustTextBox ID="txtBasicOfficeEmailOther" runat="server" Width="200px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="E-MAIL"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--負責人資料--%>
                        <tr class="trEven">
                            <%--負責人資料--%>
                            <td rowspan="6" align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel10" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_020" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--中文姓名--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_021"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%">
                                <cc1:CustTextBox ID="txtPrincipalNameCH" runat="server" MaxLength="4"  onblur="changeFullType(this);"
                                    Width="90px" onkeydown="entersubmit('btnAdd');" BoxName="中文姓名"></cc1:CustTextBox>
                            </td>
                            <%--英文姓名--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_022"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%">
                                <cc1:CustTextBox ID="txtPrincipalNameEn" runat="server" MaxLength="40"
                                    Width="90px" onkeydown="entersubmit('btnAdd');" BoxName="英文姓名"></cc1:CustTextBox>
                            </td>
                            <%--生日--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_023"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%">
                                <cc1:CustTextBox ID="txtPrincipalBirth" runat="server" MaxLength="7" checktype="num"
                                    onkeydown="entersubmit('btnAdd');" Width="60px" BoxName="生日"></cc1:CustTextBox>
                            </td>
                            <%--負責人國籍--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_024"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%" colspan="2">
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtPrincipalCountryCode" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                        onkeydown="entersubmit('btnAdd');" BoxName="負責人國籍"
                                        Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px; line-height:11px;"  AutoPostBack="true" OnTextChanged="txtCodeType_TextChanged"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropPrincipalCountryCode" kind="select" runat="server" onclick="simOptionClick4IE('txtPrincipalCountryCode');"
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                    </cc1:CustDropDownList>
                                </div>
                            </td>
                        </tr>
                        <%--20190730 長姓名需求-負責人部份--%>
                        <tr class="trEven">
                            <%--負責人資料--%>
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="CustLabel61" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01030100_102" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%" colspan="4">
                                     <cc1:CustCheckBox ID="chkisLongName" runat="server" OnCheckedChanged="CheckBox_CheckedChanged"
                                    AutoPostBack="True" BoxName="長姓名" />
                                <cc1:CustLabel ID="CustLabel62" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtPrincipalNameCH_L" runat="server" MaxLength="50" checktype="fulltype" Width="260px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人長姓名"
                                    onpaste="paste();" AutoPostBack="true" OnTextChanged="TextBox_TextChanged"></cc1:CustTextBox>
                                </td>
                            <%--羅馬拼音--%>
                            <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel63" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False"></cc1:CustLabel>
                                </td>
                            <td align="left" style="width: 10%" colspan="3">
                                <cc1:CustTextBox ID="txtPrincipalNameCH_Pinyin" runat="server" MaxLength="50" checktype="fulltype" Width="260px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人羅馬拼音"
                                    onpaste="paste();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--20190730 長姓名需求-負責人部份--%>
                        <tr class="trEven">
                            <%--身分證字號--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel12" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_025"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%">
                                <cc1:CustTextBox ID="txtPrincipalIDNo" runat="server" MaxLength="10"
                                    onkeydown="entersubmit('btnAdd');" Width="90px" BoxName="身分證字號"></cc1:CustTextBox>
                            </td>
                            <%--發證日期--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel13" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_026"
                                    StickHeight="False"></cc1:CustLabel>

                            </td>
                            <td align="left" style="width: 10%">
                                <cc1:CustTextBox ID="txtPrincipalIssueDate" runat="server" MaxLength="7" checktype="num"
                                    onkeydown="entersubmit('btnAdd');" Width="60px" BoxName="發證日期"></cc1:CustTextBox>
                            </td>
                            <%--發證地點--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel14" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_027"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%">
                                <cc1:CustTextBox ID="txtPrincipalIssuePlace" runat="server"  onblur="changeFullType(this);" MaxLength="3"
                                    onkeydown="entersubmit('btnAdd');" Width="60px" BoxName="發證地點"></cc1:CustTextBox>
                            </td>
                            <%--領補換類別--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel15" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_028"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%">
                                <cc1:CustTextBox ID="txtPrincipalRedemption" runat="server"
                                    onkeydown="entersubmit('btnAdd');" Width="60px" BoxName="領補換類別" MaxLength="1"></cc1:CustTextBox>
                            </td>
                            <%--有無照片--%>
                            <td align="left" style="width: 20%">
                                <cc1:CustLabel ID="CustLabel19" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_029"
                                    StickHeight="False"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtPrincipalHasPic" runat="server"
                                    onkeydown="entersubmit('btnAdd');" Width="30px" BoxName="有無照片" MaxLength="1"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--戶籍地址--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel11" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_030"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%" colspan="8">
                                <cc1:CustTextBox ID="txtPrincipalBookAddr1" runat="server" checktype="fulltype" MaxLength="6"
                                    Width="100px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="戶籍地址一" onpaste="paste();"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtPrincipalBookAddr2" runat="server" checktype="fulltype" MaxLength="14"
                                    Width="220px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="戶籍地址二" onpaste="paste();"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtPrincipalBookAddr3" runat="server" checktype="fulltype" MaxLength="7"
                                    Width="100px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="戶籍地址三" onpaste="paste();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--護照號碼--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel16" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_031"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%" colspan="2">
                                <cc1:CustTextBox ID="txtPrincipalPassportNo" runat="server" MaxLength="22" Width="200px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="護照號碼"></cc1:CustTextBox>
                            </td>
                            <%--護照效期--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel17" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_032"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%" colspan="5">
                                <cc1:CustTextBox ID="txtPrincipalPassportExpdt" runat="server" MaxLength="8" Width="60px" InputType="Int"
                                    onkeydown="entersubmit('btnAdd');" BoxName="護照效期"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven" style="display:none">
                            <%--通訊地址--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel18" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_033"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%" colspan="8">
                                <cc1:CustTextBox ID="txtPrincipalContactAddr1" runat="server" checktype="fulltype" MaxLength="6"
                                    Width="100px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="通訊地址一" onpaste="paste();"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtPrincipalContactAddr2" runat="server" checktype="fulltype" MaxLength="14"
                                    Width="220px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="通訊地址二" onpaste="paste();"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtPrincipalContactAddr3" runat="server" checktype="fulltype" MaxLength="7"
                                    Width="100px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="通訊地址三" onpaste="paste();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--居留證號--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel20" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_034"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%" colspan="2">
                                <cc1:CustTextBox ID="txtPrincipalResidentNo" runat="server" MaxLength="22" Width="200px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="居留證號"></cc1:CustTextBox>
                            </td>
                            <%--居留效期--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel21" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_035"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%" colspan="5">
                                <cc1:CustTextBox ID="txtPrincipalResidentExpdt" runat="server" MaxLength="8" Width="60px" InputType="Int"
                                    onkeydown="entersubmit('btnAdd');" BoxName="居留效期"></cc1:CustTextBox>
                            </td>
                        </tr> 
                        <%--SCDD表--%>
                        <tr class="trOdd">
                            <%--SCDD表--%>
                            <td rowspan="7" align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel22" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_036" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--法律形式--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel23" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_037"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%" colspan="8">
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtSCCDOrganization" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                        onkeydown="entersubmit('btnAdd');" BoxName="法律形式"
                                        Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px; line-height:11px;" AutoPostBack="true" OnTextChanged="txtCodeType_TextChanged"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropSCCDOrganization" kind="select" runat="server" onclick="simOptionClick4IE('txtSCCDOrganization');"
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                    </cc1:CustDropDownList>
                                </div>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--註冊國籍--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel28" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_038"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%">
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtSCCDCountryCode" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                        onkeydown="entersubmit('btnAdd');" BoxName="註冊國籍"
                                        Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px; line-height:11px;" AutoPostBack="true" OnTextChanged="txtCodeType_TextChanged"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropSCCDCountryCode" kind="select" runat="server" onclick="simOptionClick4IE('txtSCCDCountryCode');"
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                    </cc1:CustDropDownList>
                                </div>
                            </td>
                            <%--註冊國為美國者，請勾選州別--%>
                            <td align="right" style="width: 10%" colspan="2">
                                <cc1:CustLabel ID="CustLabel29" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_039"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%" colspan="5">
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtSCCDCountryStateCode" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                        onkeydown="entersubmit('btnAdd');" BoxName="註冊國為美國者，請勾選州別"
                                        Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px; line-height:11px;"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropSCCDCountryStateCode" kind="select" runat="server" onclick="simOptionClick4IE('txtSCCDCountryStateCode');"
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                    </cc1:CustDropDownList>
                                </div>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--僑外資/外商--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel24" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_040"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%">
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtSCCDForeign" runat="server" MaxLength="1" Width="50px" onfocus="allselect(this);"
                                        onkeydown="entersubmit('btnAdd');" BoxName="僑外資/外商"
                                        Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px; line-height:11px;"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropSCCDForeign" kind="select" runat="server" onclick="simOptionClick4IE('txtSCCDForeign');"
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                    </cc1:CustDropDownList>
                                </div>
                            </td>
                            <%--僑外資/外商母公司國別--%>
                            <td align="right" style="width: 10%" colspan="2">
                                <cc1:CustLabel ID="CustLabel25" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_041"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%" colspan="5">
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtSCCDForeignCountryStateCode" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                        onkeydown="entersubmit('btnAdd');" BoxName="僑外資/外商母公司國別"
                                        Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px; line-height:11px;" AutoPostBack="true" OnTextChanged="txtCodeType_TextChanged"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropSCCDForeignCountryStateCode" kind="select" runat="server" onclick="simOptionClick4IE('txtSCCDForeignCountryStateCode');"
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                    </cc1:CustDropDownList>
                                </div>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--台灣以外主要之營業處所地址--%>
                            <td align="right" style="width: 10%;display:none" colspan="2"  >
                                <cc1:CustLabel ID="CustLabel26" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_042"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%;display:none" colspan="4"  >
                                <cc1:CustTextBox ID="txtSCCDOtherOfficeAddr1" runat="server" checktype="fulltype" MaxLength="6"
                                    Width="100px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="台灣以外主要之營業處所地址一" onpaste="paste();"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtSCCDOtherOfficeAddr2" runat="server" checktype="fulltype" MaxLength="14"
                                    Width="220px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="台灣以外主要之營業處所地址二" onpaste="paste();"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtSCCDOtherOfficeAddr3" runat="server" checktype="fulltype" MaxLength="7"
                                    Width="100px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="台灣以外主要之營業處所地址三" onpaste="paste();"></cc1:CustTextBox>
                            </td>
                            <%--主要之營業處所國別--%>
                            <td align="right" style="width: 10%" colspan="1">
                                <cc1:CustLabel ID="CustLabel27" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_043"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%" colspan="8">
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtSCCDOtherCountryCode" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                        onkeydown="entersubmit('btnAdd');" BoxName="主要之營業處所國別"
                                        Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px; line-height:11px;"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropSCCDOtherCountryCode" kind="select" runat="server" onclick="simOptionClick4IE('txtSCCDOtherCountryCode');"
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                    </cc1:CustDropDownList>
                                </div>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--營業處所是否在高風險或制裁國家--%>
                            <td align="right" style="width: 10%" colspan="2">
                                <cc1:CustLabel ID="CustLabel30" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_044"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%">
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtSCCDIsSanction" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                        onkeydown="entersubmit('btnAdd');" BoxName="營業處所是否在高風險或制裁國家"
                                        Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px; line-height:11px;"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropSCCDIsSanction" kind="select" runat="server" onclick="simOptionClick4IE('txtSCCDIsSanction');"
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                    </cc1:CustDropDownList>
                                </div>
                            </td>
                            <%--國家--%>
                            <td align="right" style="width: 10%" colspan="2">
                                <cc1:CustLabel ID="CustLabel31" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_045"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%" colspan="4">
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtSCCDIsSanctionCountryCode1" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                        onkeydown="entersubmit('btnAdd');" BoxName="國家1"
                                        Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px; line-height:11px;" AutoPostBack="true" OnTextChanged="txtCodeType_TextChanged"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropSCCDIsSanctionCountryCode1" kind="select" runat="server" onclick="simOptionClick4IE('txtSCCDIsSanctionCountryCode1');"
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                    </cc1:CustDropDownList>
                                    <cc1:CustTextBox ID="txtSCCDIsSanctionCountryCode2" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                        onkeydown="entersubmit('btnAdd');" BoxName="國家2"
                                        Style="left: 70px; top: 0px; position: absolute; width: 32px; height: 11px; line-height:11px;" AutoPostBack="true" OnTextChanged="txtCodeType_TextChanged"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropSCCDIsSanctionCountryCode2" kind="select" runat="server" onclick="simOptionClick4IE('txtSCCDIsSanctionCountryCode2');"
                                        Style="left: 70px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                    </cc1:CustDropDownList>
                                    <cc1:CustTextBox ID="txtSCCDIsSanctionCountryCode3" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                        onkeydown="entersubmit('btnAdd');" BoxName="國家3"
                                        Style="left: 140px; top: 0px; position: absolute; width: 32px; height: 11px; line-height:11px;" AutoPostBack="true" OnTextChanged="txtCodeType_TextChanged"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropSCCDIsSanctionCountryCode3" kind="select" runat="server" onclick="simOptionClick4IE('txtSCCDIsSanctionCountryCode3');"
                                        Style="left: 140px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                    </cc1:CustDropDownList>
                                    <cc1:CustTextBox ID="txtSCCDIsSanctionCountryCode4" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                        onkeydown="entersubmit('btnAdd');" BoxName="國家4"
                                        Style="left: 210px; top: 0px; position: absolute; width: 32px; height: 11px; line-height:11px;" AutoPostBack="true" OnTextChanged="txtCodeType_TextChanged"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropSCCDIsSanctionCountryCode4" kind="select" runat="server" onclick="simOptionClick4IE('txtSCCDIsSanctionCountryCode4');"
                                        Style="left: 210px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                    </cc1:CustDropDownList>
                                    <cc1:CustTextBox ID="txtSCCDIsSanctionCountryCode5" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                        onkeydown="entersubmit('btnAdd');" BoxName="國家5"
                                        Style="left: 280px; top: 0px; position: absolute; width: 32px; height: 11px; line-height:11px;" AutoPostBack="true" OnTextChanged="txtCodeType_TextChanged"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropSCCDIsSanctionCountryCode5" kind="select" runat="server" onclick="simOptionClick4IE('txtSCCDIsSanctionCountryCode5');"
                                        Style="left: 280px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                    </cc1:CustDropDownList>
                                </div>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--複雜股權結構--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel32" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_046"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%">
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtSCCDEquity" runat="server" MaxLength="1" Width="50px" onfocus="allselect(this);"
                                        onkeydown="entersubmit('btnAdd');" BoxName="複雜股權結構"
                                        Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px; line-height:11px;"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropSCCDEquity" kind="select" runat="server" onclick="simOptionClick4IE('txtSCCDEquity');"
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                    </cc1:CustDropDownList>
                                </div>
                            </td>
                            <%--是否可發行無記名股票--%>
                            <td align="right" style="width: 10%" colspan="2">
                                <cc1:CustLabel ID="CustLabel33" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_047"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%">
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtSCCDCanBearerStock" runat="server" MaxLength="1" Width="50px" onfocus="allselect(this);"
                                        onkeydown="entersubmit('btnAdd');" BoxName="是否可發行無記名股票"
                                        Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px; line-height:11px;"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropSCCDCanBearerStock" kind="select" runat="server" onclick="simOptionClick4IE('txtSCCDCanBearerStock');"
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                    </cc1:CustDropDownList>
                                </div>
                            </td>
                            <%--是否已發行無記名股票--%>
                            <td align="right" style="width: 10%" colspan="2">
                                <cc1:CustLabel ID="CustLabel34" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_048"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%" colspan="2">
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtSCCDAlreadyBearerStock" runat="server" MaxLength="1" Width="50px" onfocus="allselect(this);"
                                        onkeydown="entersubmit('btnAdd');" BoxName="是否已發行無記名股票"
                                        Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px; line-height:11px;"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropSCCDAlreadyBearerStock" kind="select" runat="server" onclick="simOptionClick4IE('txtSCCDAlreadyBearerStock');"
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                    </cc1:CustDropDownList>
                                </div>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--是否已完成SCCD表--%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel35" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_049"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 10%" colspan="8">
                                <cc1:CustCheckBox ID="chkSCCD" runat="server"
                                    AutoPostBack="True" BoxName="是否已完成SCCD表" />
                                <cc1:CustLabel ID="lblBusinessName1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050100_061"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo3" style="">
                        <%--資料最後異動--%>
                            <tr class="trEven">                                  
                                <%--資料最後異動MAKER--%>
                                <td align="right" style="width: 18%; height: 33px;">
                                    <cc1:CustLabel ID="lbLAST_UPD_MAKER" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_086"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 16%; height: 33px;">
                                    <cc1:CustTextBox ID="txtLAST_UPD_MAKER" runat="server" MaxLength="9" checktype="numandletter"
                                        Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動MAKER" onfocus="allselect(this);"></cc1:CustTextBox>
                                </td>
                                <%--資料最後異動CHECKER--%>
                                <td align="right" style="width: 20%; height: 33px;">
                                    <cc1:CustLabel ID="lbLAST_UPD_CHECKER" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_087" StickHeight="False"
                                        ></cc1:CustLabel>
                                </td>
                                <td style="width: 18%; height: 33px;">
                                    <cc1:CustTextBox ID="txtLAST_UPD_CHECKER" runat="server" MaxLength="9" checktype="numandletter"
                                        Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動CHECKER" onfocus="allselect(this);"></cc1:CustTextBox>
                                </td>
                                <%--資料最後異動分行--%>
                                <td align="right" style="width: 11%; height: 33px;">
                                    <cc1:CustLabel ID="lbLAST_UPD_BRANCH" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_085" StickHeight="False"
                                        ></cc1:CustLabel>
                                </td>
                                <td style="width: 16%; height: 33px;">
                                    <cc1:CustTextBox ID="txtLAST_UPD_BRANCH" runat="server" MaxLength="4" Width="100px"
                                        onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動分行" checktype="numandletter"
                                        onfocus="allselect(this);"></cc1:CustTextBox>
                                </td>
                            </tr>                            
                    </table>
                    <%--高階管理人暨實質受益人資料明細表--%>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table1" style="">
                        <%--高階管理人暨實質受益人資料明細表--%>
                        <tr class="trEven">
                            <td rowspan="15" align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel36" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_050" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--高階管理人暨實質受益人資料明細表--%>
                        <tr class="trEven">
                            <%--總公司基本資料--%>
                            <td align="center" colspan="8" class="auto-style1">
                                <cc1:CustLabel ID="CustLabel37" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_051" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--刪除--%>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel40" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_052" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--姓    名--%>
                            <td align="center" style="width: 43%">
                                <cc1:CustLabel ID="CustLabel38" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_053" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--出生日期(西元)--%>
                            <td align="center" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel39" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_054" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--國籍--%>
                            <td align="center" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel41" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_055" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--身分證件號碼--%>
                            <td align="center" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel42" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_056" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--身分證件類型--%>
                            <td align="center" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel43" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_057" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--身分類型--%>
                            <td align="center" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel44" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_058" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--護照效期/居留期限(西元)--%>
                            <td align="center"  style="width: 10%">
                                <cc1:CustLabel ID="CustLabel45" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_059" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--1--%>
                            <td align="center" style="width: 7%">
                                <cc1:CustCheckBox ID="chkSeniorManagerDelete_1" runat="server"
                                    BoxName="刪除(CheckBox)" AutoPostBack="False" />
                                <cc1:CustLabel ID="lblAdd" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_060" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblID_1" runat="server" Visible="false"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 33%">
								<cc1:CustLabel ID="lblEnNotice_1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtSeniorManagerName_1" runat="server" MaxLength="40" Width="350px"  
                                    onkeydown="entersubmit('btnAdd');" BoxName="姓名"  onblur="changeFullType(this);"></cc1:CustTextBox></br>
                                <%--20190730 長姓名需求-負責人部份--%>
                                <cc1:CustCheckBox ID="chkisLongName_gdv_1" runat="server" Text="長姓名Flag" AutoPostBack="true" OnCheckedChanged="CheckBox_CheckedChanged" /><br />
                                <cc1:CustLabel ID="lblName_L_1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_L_1" runat="server" MaxLength="50" Width="280px" checktype="fulltype"  
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="姓名" AutoPostBack="true" OnTextChanged="TextBox_TextChanged" Visible="false" ></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblName_Pinyin_1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_Pinyin_1" runat="server" MaxLength="50" checktype="fulltype" Width="280px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人羅馬拼音"
                                    onpaste="paste();" Visible="false"></cc1:CustTextBox>
                                <%--20190730 長姓名需求-負責人部份--%>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerBirth_1" runat="server" MaxLength="8" checktype="num" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerCountryCode_1" runat="server" MaxLength="2" Width="70px"
                                    OnTextChanged="txtHCOP_BENE_NATION_TextChanged"   AutoPostBack="true" onkeydown="entersubmit('btnAdd');" BoxName="國籍"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNo_1" runat="server" MaxLength="22" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNoType_1" runat="server" MaxLength="1" checktype="num" Width="30px" 
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件類型"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIdentity_1" runat="server" MaxLength="11" checktype="num" Width="120px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分類型"></cc1:CustTextBox>
                            </td>
                            <td align="center"  style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerExpdt_1" runat="server" MaxLength="8" checktype="num" Width="100px" InputType="Int"
                                    onkeydown="entersubmit('btnAdd');" BoxName="護照效期/居留期限(西元)"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--2--%>
                            <td align="center" style="width: 7%">
                                <cc1:CustCheckBox ID="chkSeniorManagerDelete_2" runat="server"
                                    BoxName="刪除(CheckBox)" AutoPostBack="False" />
                                <cc1:CustLabel ID="CustLabel46" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_060" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblID_2" runat="server" Visible="false"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 33%">
								<cc1:CustLabel ID="lblEnNotice_2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtSeniorManagerName_2" runat="server" MaxLength="40" Width="350px"  
                                    onkeydown="entersubmit('btnAdd');" BoxName="姓名"  onblur="changeFullType(this);"></cc1:CustTextBox></br>
                                <%--20190730 長姓名需求-負責人部份--%>
                                <cc1:CustCheckBox ID="chkisLongName_gdv_2" runat="server" Text="長姓名Flag" AutoPostBack="true" OnCheckedChanged="CheckBox_CheckedChanged" /><br />
                                <cc1:CustLabel ID="lblName_L_2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_L_2" runat="server" MaxLength="50" Width="280px" checktype="fulltype"  
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="姓名" AutoPostBack="true" OnTextChanged="TextBox_TextChanged" Visible="false"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblName_Pinyin_2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_Pinyin_2" runat="server" MaxLength="50" checktype="fulltype" Width="280px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人羅馬拼音"
                                    onpaste="paste();" Visible="false"></cc1:CustTextBox>
                                <%--20190730 長姓名需求-負責人部份--%>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerBirth_2" runat="server" MaxLength="8" checktype="num" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerCountryCode_2" runat="server" MaxLength="2" Width="70px"
                                OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true" onkeydown="entersubmit('btnAdd');" BoxName="國籍"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNo_2" runat="server" MaxLength="22" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNoType_2" runat="server" MaxLength="1" checktype="num" Width="30px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件類型"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIdentity_2" runat="server" MaxLength="11" checktype="num" Width="120px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分類型"></cc1:CustTextBox>
                            </td>
                            <td align="center"  style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerExpdt_2" runat="server" MaxLength="8" checktype="num" Width="100px" InputType="Int"
                                    onkeydown="entersubmit('btnAdd');" BoxName="護照效期/居留期限(西元)"></cc1:CustTextBox>
                                <cc1:CustLabel ID="lblLineID_2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Visible="false"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--3--%>
                            <td align="center" style="width: 7%">
                                <cc1:CustCheckBox ID="chkSeniorManagerDelete_3" runat="server"
                                    BoxName="刪除(CheckBox)" AutoPostBack="False" />
                                <cc1:CustLabel ID="CustLabel47" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_060" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblID_3" runat="server" Visible="false"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 33%">
								<cc1:CustLabel ID="lblEnNotice_3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtSeniorManagerName_3" runat="server" MaxLength="40" Width="350px"  
                                    onkeydown="entersubmit('btnAdd');" BoxName="姓名"  onblur="changeFullType(this);"></cc1:CustTextBox></br>
                                <%--20190730 長姓名需求-負責人部份--%>
                                <cc1:CustCheckBox ID="chkisLongName_gdv_3" runat="server" Text="長姓名Flag" AutoPostBack="true" OnCheckedChanged="CheckBox_CheckedChanged" /><br />
                                <cc1:CustLabel ID="lblName_L_3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_L_3" runat="server" MaxLength="50" Width="280px" checktype="fulltype"  
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="姓名" AutoPostBack="true" OnTextChanged="TextBox_TextChanged" Visible="false"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblName_Pinyin_3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_Pinyin_3" runat="server" MaxLength="50" checktype="fulltype" Width="280px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人羅馬拼音"
                                    onpaste="paste();" Visible="false"></cc1:CustTextBox>
                                <%--20190730 長姓名需求-負責人部份--%>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerBirth_3" runat="server" MaxLength="8" checktype="num" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerCountryCode_3" runat="server" MaxLength="2" Width="70px"
                                OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"  onkeydown="entersubmit('btnAdd');" BoxName="國籍"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNo_3" runat="server" MaxLength="22" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNoType_3" runat="server" MaxLength="1" checktype="num" Width="30px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件類型"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIdentity_3" runat="server" MaxLength="11" checktype="num" Width="120px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分類型"></cc1:CustTextBox>
                            </td>
                            <td align="center"  style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerExpdt_3" runat="server" MaxLength="8" checktype="num" Width="100px" InputType="Int"
                                    onkeydown="entersubmit('btnAdd');" BoxName="護照效期/居留期限(西元)"></cc1:CustTextBox>
                                <cc1:CustLabel ID="lblLineID_3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Visible="false"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--4--%>
                            <td align="center" style="width: 7%">
                                <cc1:CustCheckBox ID="chkSeniorManagerDelete_4" runat="server"
                                    BoxName="刪除(CheckBox)" AutoPostBack="False" />
                                <cc1:CustLabel ID="CustLabel48" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_060" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblID_4" runat="server" Visible="false"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 33%">
								<cc1:CustLabel ID="lblEnNotice_4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel> 
                                <cc1:CustTextBox ID="txtSeniorManagerName_4" runat="server" MaxLength="40" Width="350px"  
                                    onkeydown="entersubmit('btnAdd');" BoxName="姓名"  onblur="changeFullType(this);"></cc1:CustTextBox></br>
                                <%--20190730 長姓名需求-負責人部份--%>
                                <cc1:CustCheckBox ID="chkisLongName_gdv_4" runat="server" Text="長姓名Flag" AutoPostBack="true" OnCheckedChanged="CheckBox_CheckedChanged" /><br />
                                <cc1:CustLabel ID="lblName_L_4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_L_4" runat="server" MaxLength="50" Width="280px" checktype="fulltype"  
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="姓名" AutoPostBack="true" OnTextChanged="TextBox_TextChanged" Visible="false"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblName_Pinyin_4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_Pinyin_4" runat="server" MaxLength="50" checktype="fulltype" Width="280px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人羅馬拼音"
                                    onpaste="paste();" Visible="false"></cc1:CustTextBox>
                                <%--20190730 長姓名需求-負責人部份--%>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerBirth_4" runat="server" MaxLength="8" checktype="num" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerCountryCode_4" runat="server" MaxLength="2" Width="70px"
                                OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"    onkeydown="entersubmit('btnAdd');" BoxName="國籍"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNo_4" runat="server" MaxLength="22" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNoType_4" runat="server" MaxLength="1" checktype="num" Width="30px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件類型"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIdentity_4" runat="server" MaxLength="11" checktype="num" Width="120px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分類型"></cc1:CustTextBox>
                            </td>
                            <td align="center"  style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerExpdt_4" runat="server" MaxLength="8" checktype="num" Width="100px" InputType="Int"
                                    onkeydown="entersubmit('btnAdd');" BoxName="護照效期/居留期限(西元)"></cc1:CustTextBox>
                                <cc1:CustLabel ID="lblLineID_4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Visible="false"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--5--%>
                            <td align="center" style="width: 7%">
                                <cc1:CustCheckBox ID="chkSeniorManagerDelete_5" runat="server"
                                    BoxName="刪除(CheckBox)" AutoPostBack="False" />
                                <cc1:CustLabel ID="CustLabel49" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_060" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblID_5" runat="server" Visible="false"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 33%">
								<cc1:CustLabel ID="lblEnNotice_5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel> 
                                <cc1:CustTextBox ID="txtSeniorManagerName_5" runat="server" MaxLength="40" Width="350px"  
                                    onkeydown="entersubmit('btnAdd');" BoxName="姓名"  onblur="changeFullType(this);"></cc1:CustTextBox></br>
                                <%--20190730 長姓名需求-負責人部份--%>
                                <cc1:CustCheckBox ID="chkisLongName_gdv_5" runat="server" Text="長姓名Flag" AutoPostBack="true" OnCheckedChanged="CheckBox_CheckedChanged" /><br />
                                <cc1:CustLabel ID="lblName_L_5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_L_5" runat="server" MaxLength="50" Width="280px" checktype="fulltype"  
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="姓名" AutoPostBack="true" OnTextChanged="TextBox_TextChanged" Visible="false"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblName_Pinyin_5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_Pinyin_5" runat="server" MaxLength="50" checktype="fulltype" Width="280px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人羅馬拼音"
                                    onpaste="paste();" Visible="false"></cc1:CustTextBox>
                                <%--20190730 長姓名需求-負責人部份--%>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerBirth_5" runat="server" MaxLength="8" checktype="num" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerCountryCode_5" runat="server" MaxLength="2" Width="70px"
                                OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"    onkeydown="entersubmit('btnAdd');" BoxName="國籍"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNo_5" runat="server" MaxLength="22" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNoType_5" runat="server" MaxLength="1" checktype="num" Width="30px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件類型"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIdentity_5" runat="server" MaxLength="11" checktype="num" Width="120px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分類型"></cc1:CustTextBox>
                            </td>
                            <td align="center"  style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerExpdt_5" runat="server" MaxLength="8" checktype="num" Width="100px" InputType="Int"
                                    onkeydown="entersubmit('btnAdd');" BoxName="護照效期/居留期限(西元)"></cc1:CustTextBox>
                                <cc1:CustLabel ID="lblLineID_5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Visible="false"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--6--%>
                            <td align="center" style="width: 7%">
                                <cc1:CustCheckBox ID="chkSeniorManagerDelete_6" runat="server"
                                    BoxName="刪除(CheckBox)" AutoPostBack="False" />
                                <cc1:CustLabel ID="CustLabel50" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_060" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblID_6" runat="server" Visible="false"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 33%">
								<cc1:CustLabel ID="lblEnNotice_6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtSeniorManagerName_6" runat="server" MaxLength="40" Width="350px"  
                                    onkeydown="entersubmit('btnAdd');" BoxName="姓名"  onblur="changeFullType(this);"></cc1:CustTextBox><br />
                                <%--20190730 長姓名需求-負責人部份--%>
                                <cc1:CustCheckBox ID="chkisLongName_gdv_6" runat="server" Text="長姓名Flag" AutoPostBack="true" OnCheckedChanged="CheckBox_CheckedChanged" /><br />
                                <cc1:CustLabel ID="lblName_L_6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_L_6" runat="server" MaxLength="50" Width="280px" checktype="fulltype"  
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="姓名" AutoPostBack="true" OnTextChanged="TextBox_TextChanged" Visible="false"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblName_Pinyin_6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_Pinyin_6" runat="server" MaxLength="50" checktype="fulltype" Width="280px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人羅馬拼音"
                                    onpaste="paste();" Visible="false"></cc1:CustTextBox>
                                <%--20190730 長姓名需求-負責人部份--%>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerBirth_6" runat="server" MaxLength="8" checktype="num" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerCountryCode_6" runat="server" MaxLength="2" Width="70px"
                                 OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"   onkeydown="entersubmit('btnAdd');" BoxName="國籍"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNo_6" runat="server" MaxLength="22" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNoType_6" runat="server" MaxLength="1" checktype="num" Width="30px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件類型"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIdentity_6" runat="server" MaxLength="11" checktype="num" Width="120px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分類型"></cc1:CustTextBox>
                            </td>
                            <td align="center"  style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerExpdt_6" runat="server" MaxLength="8" checktype="num" Width="100px" InputType="Int"
                                    onkeydown="entersubmit('btnAdd');" BoxName="護照效期/居留期限(西元)"></cc1:CustTextBox>
                                <cc1:CustLabel ID="lblLineID_6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Visible="false"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--7--%>
                            <td align="center" style="width: 7%">
                                <cc1:CustCheckBox ID="chkSeniorManagerDelete_7" runat="server"
                                    BoxName="刪除(CheckBox)" AutoPostBack="False" />
                                <cc1:CustLabel ID="CustLabel51" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_060" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblID_7" runat="server" Visible="false"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 33%">
								<cc1:CustLabel ID="lblEnNotice_7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel>

                                <cc1:CustTextBox ID="txtSeniorManagerName_7" runat="server" MaxLength="40" Width="350px"  
                                    onkeydown="entersubmit('btnAdd');" BoxName="姓名"  onblur="changeFullType(this);"></cc1:CustTextBox></br>
                                <%--20190730 長姓名需求-負責人部份--%>
                                <cc1:CustCheckBox ID="chkisLongName_gdv_7" runat="server" Text="長姓名Flag" AutoPostBack="true" OnCheckedChanged="CheckBox_CheckedChanged" /><br />
                                <cc1:CustLabel ID="lblName_L_7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_L_7" runat="server" MaxLength="50" Width="280px" checktype="fulltype"  
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="姓名" AutoPostBack="true" OnTextChanged="TextBox_TextChanged" Visible="false"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblName_Pinyin_7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_Pinyin_7" runat="server" MaxLength="50" checktype="fulltype" Width="280px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人羅馬拼音"
                                    onpaste="paste();" Visible="false"></cc1:CustTextBox>
                                <%--20190730 長姓名需求-負責人部份--%>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerBirth_7" runat="server" MaxLength="8" checktype="num" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerCountryCode_7" runat="server" MaxLength="2" Width="70px"
                              OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"      onkeydown="entersubmit('btnAdd');" BoxName="國籍"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNo_7" runat="server" MaxLength="22" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNoType_7" runat="server" MaxLength="1" checktype="num" Width="30px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件類型"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIdentity_7" runat="server" MaxLength="11" checktype="num" Width="120px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分類型"></cc1:CustTextBox>
                            </td>
                            <td align="center"  style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerExpdt_7" runat="server" MaxLength="8" checktype="num" Width="100px" InputType="Int"
                                    onkeydown="entersubmit('btnAdd');" BoxName="護照效期/居留期限(西元)"></cc1:CustTextBox>
                                <cc1:CustLabel ID="lblLineID_7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Visible="false"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--8--%>
                            <td align="center" style="width: 7%">
                                <cc1:CustCheckBox ID="chkSeniorManagerDelete_8" runat="server"
                                    BoxName="刪除(CheckBox)" AutoPostBack="False" />
                                <cc1:CustLabel ID="CustLabel52" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_060" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblID_8" runat="server" Visible="false"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 33%">
								<cc1:CustLabel ID="lblEnNotice_8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel>

                                <cc1:CustTextBox ID="txtSeniorManagerName_8" runat="server" MaxLength="40" Width="350px"  
                                    onkeydown="entersubmit('btnAdd');" BoxName="姓名"  onblur="changeFullType(this);"></cc1:CustTextBox></br>
                                <%--20190730 長姓名需求-負責人部份--%>
                                <cc1:CustCheckBox ID="chkisLongName_gdv_8" runat="server" Text="長姓名Flag" AutoPostBack="true" OnCheckedChanged="CheckBox_CheckedChanged" /><br />
                                <cc1:CustLabel ID="lblName_L_8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_L_8" runat="server" MaxLength="50" Width="280px" checktype="fulltype"  
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="姓名" AutoPostBack="true" OnTextChanged="TextBox_TextChanged" Visible="false"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblName_Pinyin_8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_Pinyin_8" runat="server" MaxLength="50" checktype="fulltype" Width="280px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人羅馬拼音"
                                    onpaste="paste();" Visible="false"></cc1:CustTextBox>
                                <%--20190730 長姓名需求-負責人部份--%>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerBirth_8" runat="server" MaxLength="8" checktype="num" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerCountryCode_8" runat="server" MaxLength="2" Width="70px"
                               OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"     onkeydown="entersubmit('btnAdd');" BoxName="國籍"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNo_8" runat="server" MaxLength="22" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNoType_8" runat="server" MaxLength="1" checktype="num" Width="30px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件類型"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIdentity_8" runat="server" MaxLength="11" checktype="num" Width="120px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分類型"></cc1:CustTextBox>
                            </td>
                            <td align="center"  style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerExpdt_8" runat="server" MaxLength="8" checktype="num" Width="100px" InputType="Int"
                                    onkeydown="entersubmit('btnAdd');" BoxName="護照效期/居留期限(西元)"></cc1:CustTextBox>
                                <cc1:CustLabel ID="lblLineID_8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Visible="false"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--9--%>
                            <td align="center" style="width: 7%">
                                <cc1:CustCheckBox ID="chkSeniorManagerDelete_9" runat="server"
                                    BoxName="刪除(CheckBox)" AutoPostBack="False" />
                                <cc1:CustLabel ID="CustLabel53" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_060" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblID_9" runat="server" Visible="false"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 33%">
								<cc1:CustLabel ID="lblEnNotice_9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel>

                                <cc1:CustTextBox ID="txtSeniorManagerName_9" runat="server" MaxLength="40" Width="350px"  
                                    onkeydown="entersubmit('btnAdd');" BoxName="姓名"  onblur="changeFullType(this);"></cc1:CustTextBox></br>
                                <%--20190730 長姓名需求-負責人部份--%>
                                <cc1:CustCheckBox ID="chkisLongName_gdv_9" runat="server" Text="長姓名Flag" AutoPostBack="true" OnCheckedChanged="CheckBox_CheckedChanged" /><br />
                                <cc1:CustLabel ID="lblName_L_9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_L_9" runat="server" MaxLength="50" Width="280px" checktype="fulltype"  
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="姓名" AutoPostBack="true" OnTextChanged="TextBox_TextChanged" Visible="false"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblName_Pinyin_9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_Pinyin_9" runat="server" MaxLength="50" checktype="fulltype" Width="280px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人羅馬拼音"
                                    onpaste="paste();" Visible="false"></cc1:CustTextBox>
                                <%--20190730 長姓名需求-負責人部份--%>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerBirth_9" runat="server" MaxLength="8" checktype="num" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerCountryCode_9" runat="server" MaxLength="2" Width="70px"
                               OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"     onkeydown="entersubmit('btnAdd');" BoxName="國籍"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNo_9" runat="server" MaxLength="22" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNoType_9" runat="server" MaxLength="1" checktype="num" Width="30px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件類型"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIdentity_9" runat="server" MaxLength="11" checktype="num" Width="120px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分類型"></cc1:CustTextBox>
                            </td>
                            <td align="center"  style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerExpdt_9" runat="server" MaxLength="8" checktype="num" Width="100px" InputType="Int"
                                    onkeydown="entersubmit('btnAdd');" BoxName="護照效期/居留期限(西元)"></cc1:CustTextBox>
                                <cc1:CustLabel ID="lblLINEID_9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Visible="false"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--10--%>
                            <td align="center" style="width: 7%">
                                <cc1:CustCheckBox ID="chkSeniorManagerDelete_10" runat="server"
                                    BoxName="刪除(CheckBox)" AutoPostBack="False" />
                                <cc1:CustLabel ID="CustLabel54" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_060" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblID_10" runat="server" Visible="false"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 33%">
								<cc1:CustLabel ID="lblEnNotice_10" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel>

                                <cc1:CustTextBox ID="txtSeniorManagerName_10" runat="server" MaxLength="40" Width="350px"  
                                    onkeydown="entersubmit('btnAdd');" BoxName="姓名"  onblur="changeFullType(this);"></cc1:CustTextBox></br>
                                <%--20190730 長姓名需求-負責人部份--%>
                                <cc1:CustCheckBox ID="chkisLongName_gdv_10" runat="server" Text="長姓名Flag" AutoPostBack="true" OnCheckedChanged="CheckBox_CheckedChanged" /><br />
                                <cc1:CustLabel ID="lblName_L_10" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_L_10" runat="server" MaxLength="50" Width="280px" checktype="fulltype"  
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="姓名" AutoPostBack="true" OnTextChanged="TextBox_TextChanged" Visible="false"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblName_Pinyin_10" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_Pinyin_10" runat="server" MaxLength="50" checktype="fulltype" Width="280px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人羅馬拼音"
                                    onpaste="paste();" Visible="false"></cc1:CustTextBox>
                                <%--20190730 長姓名需求-負責人部份--%>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerBirth_10" runat="server" MaxLength="8" checktype="num" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerCountryCode_10" runat="server" MaxLength="2" Width="70px"
                               OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"     onkeydown="entersubmit('btnAdd');" BoxName="國籍"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNo_10" runat="server" MaxLength="22" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNoType_10" runat="server" MaxLength="1" checktype="num" Width="30px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件類型"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIdentity_10" runat="server" MaxLength="11" checktype="num" Width="120px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分類型"></cc1:CustTextBox>
                            </td>
                            <td align="center"  style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerExpdt_10" runat="server" MaxLength="8" checktype="num" Width="100px" InputType="Int"
                                    onkeydown="entersubmit('btnAdd');" BoxName="護照效期/居留期限(西元)"></cc1:CustTextBox>
                                <cc1:CustLabel ID="lblLineID_10" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Visible="false"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--11--%>
                            <td align="center" style="width: 7%">
                                <cc1:CustCheckBox ID="chkSeniorManagerDelete_11" runat="server"
                                    BoxName="刪除(CheckBox)" AutoPostBack="False" />
                                <cc1:CustLabel ID="CustLabel55" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_060" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblID_11" runat="server" Visible="false"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 33%">
								<cc1:CustLabel ID="lblEnNotice_11" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel>

                                <cc1:CustTextBox ID="txtSeniorManagerName_11" runat="server" MaxLength="40" Width="350px"  
                                    onkeydown="entersubmit('btnAdd');" BoxName="姓名"  onblur="changeFullType(this);"></cc1:CustTextBox></br>
                                <%--20190730 長姓名需求-負責人部份--%>
                                <cc1:CustCheckBox ID="chkisLongName_gdv_11" runat="server" Text="長姓名Flag" AutoPostBack="true" OnCheckedChanged="CheckBox_CheckedChanged" /><br />
                                <cc1:CustLabel ID="lblName_L_11" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_L_11" runat="server" MaxLength="50" Width="280px" checktype="fulltype"  
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="姓名" AutoPostBack="true" OnTextChanged="TextBox_TextChanged" Visible="false"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblName_Pinyin_11" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_Pinyin_11" runat="server" MaxLength="50" checktype="fulltype" Width="280px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人羅馬拼音"
                                    onpaste="paste();" Visible="false"></cc1:CustTextBox>
                                <%--20190730 長姓名需求-負責人部份--%>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerBirth_11" runat="server" MaxLength="8" checktype="num" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerCountryCode_11" runat="server" MaxLength="2" Width="70px"
                                OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"    onkeydown="entersubmit('btnAdd');" BoxName="國籍"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNo_11" runat="server" MaxLength="22" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNoType_11" runat="server" MaxLength="1" checktype="num" Width="30px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件類型"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIdentity_11" runat="server" MaxLength="11" checktype="num" Width="120px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分類型"></cc1:CustTextBox>
                            </td>
                            <td align="center"  style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerExpdt_11" runat="server" MaxLength="8" checktype="num" Width="100px" InputType="Int"
                                    onkeydown="entersubmit('btnAdd');" BoxName="護照效期/居留期限(西元)"></cc1:CustTextBox>
                                <cc1:CustLabel ID="lblLineID_11" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Visible="false"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--12--%>
                            <td align="center" style="width: 7%">
                                <cc1:CustCheckBox ID="chkSeniorManagerDelete_12" runat="server"
                                    BoxName="刪除(CheckBox)" AutoPostBack="False" />
                                <cc1:CustLabel ID="CustLabel56" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_060" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblID_12" runat="server" Visible="false"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 33%">
								<cc1:CustLabel ID="lblEnNotice_12" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel> 
                                <cc1:CustTextBox ID="txtSeniorManagerName_12" runat="server" MaxLength="40" Width="350px"  
                                    onkeydown="entersubmit('btnAdd');" BoxName="姓名"  onblur="changeFullType(this);"></cc1:CustTextBox></br>
                                <%--20190730 長姓名需求-負責人部份--%>
                                <cc1:CustCheckBox ID="chkisLongName_gdv_12" runat="server" Text="長姓名Flag" AutoPostBack="true" OnCheckedChanged="CheckBox_CheckedChanged" /><br />
                                <cc1:CustLabel ID="lblName_L_12" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_L_12" runat="server" MaxLength="50" Width="280px" checktype="fulltype"  
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="姓名" AutoPostBack="true" OnTextChanged="TextBox_TextChanged" Visible="false"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblName_Pinyin_12" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False" Width="80px" Visible="false"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtName_Pinyin_12" runat="server" MaxLength="50" checktype="fulltype" Width="280px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人羅馬拼音"
                                    onpaste="paste();" Visible="false"></cc1:CustTextBox>
                                <%--20190730 長姓名需求-負責人部份--%>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerBirth_12" runat="server" MaxLength="8" checktype="num" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerCountryCode_12" runat="server" MaxLength="2" Width="70px"
                               OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"     onkeydown="entersubmit('btnAdd');" BoxName="國籍"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNo_12" runat="server" MaxLength="22" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIDNoType_12" runat="server" MaxLength="1" checktype="num" Width="30px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證件類型"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerIdentity_12" runat="server" MaxLength="11" checktype="num" Width="120px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分類型"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtSeniorManagerExpdt_12" runat="server" MaxLength="8" checktype="num" Width="100px" InputType="Int"
                                    onkeydown="entersubmit('btnAdd');" BoxName="護照效期/居留期限(西元)"></cc1:CustTextBox>
                                <cc1:CustLabel ID="lblLineID_12" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Visible="false"></cc1:CustLabel>
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo4" style="">
                        <tr class="itemTitle">
                            <td align="center">
                                <cc1:CustButton ID="btnAdd" runat="server" CssClass="smallButton" ShowID="01_01040101_026"
                                    OnClientClick="return checkInputText('pnlText', 1);" DisabledWhenSubmit="False"
                                    onkeydown="movefocus();" OnClick="btnAdd_Click" />
                            </td>
                        </tr>
                    </table>
                    <%--行業編號--%>
                    <cc1:CustHiddenField ID="hidAMLCC" runat="server" />
                    <%--國籍--%>
                    <cc1:CustHiddenField ID="hidCountryCode" runat="server" />
                    <%--州別--%>
                    <cc1:CustHiddenField ID="hidStateCode" runat="server" />
                    <%--法律形式--%>
                    <cc1:CustHiddenField ID="hidOrganization" runat="server" />
                    <%--是、否、空白--%>
                    <cc1:CustHiddenField ID="hidYNEmpty" runat="server" />
                    <%--數值--%>
                    <cc1:CustHiddenField ID="hidNumber" runat="server" />
                    <%--MainSn--%>
                   
                    <cc1:CustHiddenField ID="hidisNew" runat="server" />
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
