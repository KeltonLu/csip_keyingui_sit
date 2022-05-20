<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010104020001.aspx.cs" Inherits="P010104020001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <%-- 20201123_Ares_Stanley-修正格式; 20210125_Ares_Stanley-修正格式; 20210304_Ares_Stanley-調整版面; 20210329_Ares_Stanley-調整半形轉全形失效; 20210331_Ares_Stanley-調整統編靠左對齊, 調整Disabled input背景色; 20210408_Ares_Stanley-調整半形轉全形失效; 20210415_Ares_Stanley-調整全半形轉換; 20210902_Ares_Stanley:移除Email前30後19長度限制, 改為總長度50 --%>
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript">
        /*//20200219-RQ-2019-030155-003 移除獨立店選項
    $(document).ready(function() {
        
        $('#radSingleMerchant3').live('click', function(){
            if (document.getElementById('radSingleMerchant3').checked)
                document.getElementById('txtUniNo').value = document.getElementById('txtCardNo1').value.Trim();
        });
    });
    */
    //*客戶端檢核
    function checkInputText(id,intType)
    {        
        //*檢核輸入欄位統一編號是否為空
        if(document.getElementById('txtCardNo1').value.Trim() == "" )
        {
            alert('請輸入統一編號! ');
            document.getElementById('txtCardNo1').focus();
            return false;
        }

        //*序號不能輸入空
        if(document.getElementById('txtCardNo2').value.Trim() == "")
        {
            alert('請輸入序號! ');
            document.getElementById('txtCardNo2').focus();
            return false;
        }

        if(document.getElementById('txtCardNo1').value.Trim().length != 8)
        {
            alert('統編請輸入8碼數字! ');
            document.getElementById('txtCardNo1').focus();
            return false;
        }

        //*統一編號不為0
        if(document.getElementById('txtCardNo1').value.Trim() == "00000000")
        {
            alert('統一編號不能輸入0! ');
            document.getElementById('txtCardNo1').focus();
            return false;
        }

        if(!isNum(document.getElementById('txtCardNo1').value.Trim()))
        { 
            alert('統編只能輸入數字');
            document.getElementById('txtCardNo1').focus();
            return false; 
        }

        //*序號不為非數字
        if(!isNum(document.getElementById('txtCardNo2').value.Trim()))
        { 
            alert('序號只能輸入數字');
            document.getElementById('txtCardNo2').focus();
            return false;
        }

        //*新增按鈕檢核
        if(intType == 1)
        {
            //*收件編號不為空
            if(document.getElementById('txtReceiveNumber').value.Trim() == "")
            {
                alert('收件編號不為空! ');
                document.getElementById('txtReceiveNumber').focus();
                return false;
            }

            //*收件編號需輸入7碼
            if(document.getElementById('txtReceiveNumber').value.Trim().length < 7)
            {
                alert('收件編號輸入不正確');
                document.getElementById('txtReceiveNumber').focus();
                return false;
            }

            if(!isNum(document.getElementById('txtReceiveNumber').value.Trim()))
            {
                alert('收件編號輸入不正確');
                document.getElementById('txtReceiveNumber').focus();
                return false;
            }

            //收件編號日期月份<=12
            if(Number(document.getElementById('txtReceiveNumber').value.substring(3,5)) > 12)
            {
                alert('收件編號日期月份需<=12! ');
                document.getElementById('txtReceiveNumber').focus();
                return false;
            }

            if(Number(document.getElementById('txtReceiveNumber').value.substring(5,7)) > 31)
            {
                alert('收件編號日期日需<=31! ');
                document.getElementById('txtReceiveNumber').focus();
                return false;
            }
            
            // 商店別標識(1.總公司已往來 2.總公司未往來 3.獨立店)
            //20200219-RQ-2019-030155-003 移除3.獨立店 4.海外公司，新增5.分期平台
            //20210906 自然人收單 新增分期平台選項
            //            if (!document.getElementById('radSingleMerchant1').checked && !document.getElementById('radSingleMerchant2').checked && !document.getElementById('radSingleMerchant3').checked && !document.getElementById('radSingleMerchant4').checked)
            if (!document.getElementById('radSingleMerchant1').checked && !document.getElementById('radSingleMerchant2').checked && !document.getElementById('radSingleMerchant5').checked && !document.getElementById('radSingleMerchant6').checked)
            {
                //alert('請選擇總公司已(未)往來或獨立店或海外店!');
                alert('請選擇總公司已(未)往來或分期平台!');
                return false;
            }
            
            // 總公司已往來或總公司未往來，需手動填入統一編號
            //20200219-RQ-2019-030155-003 5.分期平台時，總公司統編也視為必輸欄位
            if (document.getElementById('radSingleMerchant1').checked || document.getElementById('radSingleMerchant2').checked || document.getElementById('radSingleMerchant5').checked || document.getElementById('radSingleMerchant6').checked)
            {
                var uniNoLen = document.getElementById('txtUniNo').value.Trim().length;
                if (uniNoLen == 0)
                {
                    alert('請輸入總公司統一編號!');
                    document.getElementById('txtUniNo').focus();
                    return false;
                }
                else
                {
                    if (uniNoLen != 8)
                    {
                        alert('統編請輸入8碼數字!');
                        document.getElementById('txtUniNo').focus();
                        return false;
                    }
                }
            }
            
            // AML行業編號
            var amlcc = document.getElementById('txtAMLCC').value.Trim();
            if (amlcc.length != 7)
            {
                alert('AML行業編號請輸入7碼數字!');
                document.getElementById('txtAMLCC').focus();
                return false;
            }
            else
            {
                var allAMLCC = document.getElementById('hidAMLCC').value.Trim();
                if (allAMLCC.indexOf(amlcc) == -1)
                {
                    alert('AML行業編號不存在!');
                    document.getElementById('txtAMLCC').focus();
                    return false;
                }
            }
            
            // 國籍
            var allCountryCode = document.getElementById('hidCountryCode').value.Trim();
            var countryCode = document.getElementById('txtCountryCode').value.toUpperCase().Trim();
            if (countryCode.length > 0)
            {
                if (allCountryCode.indexOf(countryCode) == -1)
                {
                    alert('國籍不存在!');
                    document.getElementById('txtCountryCode').focus();
                    return false;
                }
            }
            else {
                alert('國籍欄位不得為空，請輸入國籍!');
                return false;
            }

            document.getElementById('txtCountryCode').value = countryCode;
            
            //20191001 10月需求-效期無需檢核↓
            // 護照號碼、護照效期
            //var passportNo = document.getElementById('txtPassportNo').value.Trim();
            //var passportExpdt = document.getElementById('txtPassportExpdt').value.Trim();
            //if (passportNo.length > 0)
            //{
            //    if (passportExpdt.length == 0)
            //    {
            //        alert('請輸入護照效期!');
            //        document.getElementById('txtPassportExpdt').focus();
            //        return false;
            //    }
            //}
            
            // 居留證號、居留效期
            //var residentNo = document.getElementById('txtResidentNo').value.Trim();
            //var residentExpdt = document.getElementById('txtResidentExpdt').value.Trim();
            //if (residentNo.length > 0)
            //{
            //    if (residentExpdt.length == 0)
            //    {
            //        alert('請輸入居留效期!');
            //        document.getElementById('txtResidentExpdt').focus();
            //        return false;
            //    }
            //}
            //20191001 10月需求-效期無需檢核↑

            // E-Mail
            var emailF = document.getElementById('txtEmailFront').value.Trim();
            if(emailF.length > 0)
            {
                if(document.getElementById('radGmail').checked || document.getElementById('radYahoo').checked || 
                   document.getElementById('radHotmail').checked || document.getElementById('radOther').checked)
                {
                    var emailB = '';
                    if (document.getElementById('radGmail').checked)
                    {
                        emailB = 'gmail.com';
                    }
                    else if (document.getElementById('radYahoo').checked)
                    {
                        emailB = 'yahoo.com.tw';
                    }
                    else if (document.getElementById('radHotmail').checked)
                    {
                        emailB = 'hotmail.com';
                    }
                    else
                    {
                        emailB = document.getElementById('txtEmailOther').value.Trim();
                    }
                    
                    var email = emailF + '@' + emailB;
                    if (email.length > 1)
                    {
                        var emailRule = new RegExp(/^[+a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/i);
                        if (!emailRule.test(email))
                        {
                            alert('E-Mail錯誤!');
                            document.getElementById('txtEmailFront').focus();
                            return false;
                        }
                    }
                    
                    // 完整E-MAIL填入HiddenField
                    var emailControl = document.getElementById('hidEmailFall');
                    emailControl.value = email;
                }
                else
                {
                    alert('請選擇E-Mail!');
                    return false;
                }
            }

            //負責人ID
            if(!checkID(document.getElementById('txtBossID').value.Trim()))
            {
                return false;
            }
            
            // 因表單已刪除實際經營者欄位
            /*if(document.getElementById('chkOper').checked==false)
            {
                //*實際經營者 ID , 身分證號碼長度不為10
                if(!checkID(document.getElementById('txtOperID').value.Trim()))
                {
                    return false;
                }
            }*/

            var value = document.getElementById('txtJCIC').value.toUpperCase().Trim();
            if(!(value == "A" ||  value == "B" || value == "C" || value == ""))
            {
                alert('JCIC只能輸入A/B/C/空白');
                document.getElementById('txtJCIC').focus();
                return false;
            }
                       
            // 2015/1/30 by Eric
            $("#txtJCIC").val($("#txtJCIC").val().toUpperCase());
            
            if ($("#cddl_MemberService").val().toUpperCase() != "Y" && $("#txtJCIC").val() == "A")
            {
                if (!confirm('自動給號設定為\' ' + $("#cddl_MemberService").val() + ' \'，是否確定送出？'))
                {
                    $("#txtJCIC").focus();
                    return false;
                }
            }
            
            var value = document.getElementById('txtGrantFeeFlag').value.toUpperCase().Trim();
            if(!(value == "Y" || value == ""))
            {
                alert('Y_特店跨行匯費只能輸入Y/空白');
                document.getElementById('txtGrantFeeFlag').focus();
                return false;
            }
            else if(value == "Y")
            {
                if(!confirm('Y_特店跨行匯費為Y，是否確定要新增？'))
                {
                    return false;
                }
            }
            
            var value = document.getElementById('txtMposFlag').value.toUpperCase().Trim();
            if(!(value == "Y" || value == ""))
            {
                alert('Y_MPOS特店系統服務費免收註記只能輸入Y/空白');
                document.getElementById('txtMposFlag').focus();
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
       
    //*統一編號(1)、統一編號(2)欄位輸入值改變
    function changeStatus()
    {
        if(document.getElementById('txtCardNo1').value.Trim() != document.getElementById('txtCardNo1Hide').value.Trim() || 
           document.getElementById('txtCardNo2').value.Trim() != document.getElementById('txtCardNo2Hide').value.Trim())
        {
            document.getElementById('lblCheckManText').innerText = "";
            document.getElementById('lblEstablishText').innerText = "";
            document.getElementById('lblCapitalText').innerText = "";
            document.getElementById('lblOrganizationText').innerText = "";
            document.getElementById('lblRiskText').innerText = "";
            document.getElementById('lblRegNameText').innerText = "";

            document.getElementById('lblBusinessNameText_Other').innerText = "";
            document.getElementById('lblBossText').innerText = "";
            document.getElementById('lblBossTel1Text').innerText = "";
            document.getElementById('lblBossTel2Text').innerText = "";
            document.getElementById('lblBossTel3Text').innerText = "";
            document.getElementById('lblBossChangeDateText').innerText = "";
            document.getElementById('lblBossFlagText').innerText = "";
            document.getElementById('lblBossBirthdayText').innerText = "";
            document.getElementById('lblBossAtText').innerText = "";
            document.getElementById('lblBossRegAddr1Text').innerText = "";
            document.getElementById('lblBossRegAddr2Text').innerText = "";
            document.getElementById('lblBossRegAddr3Text').innerText = "";
            document.getElementById('lblOpermanText').innerText = "";
            document.getElementById('lblOperIDText').innerText = "";
            document.getElementById('lblOperTelText1').innerText = "";
            document.getElementById('lblOperTelText2').innerText = "";
            document.getElementById('lblOperTelText3').innerText = "";
            document.getElementById('lblOperChangeDateText').innerText = "";
            document.getElementById('lblOperFlagText').innerText = "";
            document.getElementById('lblOperBirthdayText').innerText = "";
            document.getElementById('lblOperAtText').innerText = "";
            document.getElementById('lblOpermanText_Other').innerText = "";
            document.getElementById('lblOperTelText1_Other').innerText = "";
            document.getElementById('lblOperTelText2_Other').innerText = "";
            document.getElementById('lblOperTelText3_Other').innerText = "";
            document.getElementById('lblOperChangeDateText_Other').innerText = "";
            document.getElementById('lblOperFlagText_Other').innerText = "";
            document.getElementById('lblOperBirthdayText_Other').innerText = "";
            document.getElementById('lblOperAtText_Other').innerText = "";
            document.getElementById('lblContactManText').innerText = "";
            document.getElementById('lblContactManTel1Text').innerText = "";
            document.getElementById('lblContactManTel2Text').innerText = "";
            document.getElementById('lblContactManTel3Text').innerText = "";
            document.getElementById('lblFax1Text').innerText = "";
            document.getElementById('lblFax2Text').innerText = "";
            document.getElementById('lblBookAddr1Text').innerText = "";
            document.getElementById('lblBookAddr2Text').innerText = "";
            document.getElementById('lblBookAddr3Text').innerText = "";

            document.getElementById('lblZipText').innerText = "";
            document.getElementById('lblBusinessAddrText1_Other').innerText = "";
            document.getElementById('lblBusinessAddrText2_Other').innerText = "";
            document.getElementById('lblBusinessAddrText3_Other').innerText = "";
            document.getElementById('lblBankText').innerText = "";
            document.getElementById('lblBranchBankText').innerText = "";
            document.getElementById('lblNameText').innerText = "";
            document.getElementById('lblPopManText').innerText = "";
            // 表單新增欄位
//            document.getElementById('lblIsContactUniNoText').innerText = "";
//            document.getElementById('lblCountryCodeText').innerText = "";
//            document.getElementById('lblPassportNoText').innerText = "";
//            document.getElementById('lblResidentNoText').innerText = "";
//            document.getElementById('lblEmailText').innerText = "";

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
        //getValue('lblOpermanText');

        //*實際經營者換證點
        //getValue('lblOperAtText');

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
        <cust:image runat="server" ID="image1" />
        &nbsp;
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                    <tr class="itemTitle1">
                        <td colspan="2">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="250px" IsColon="False" ShowID="01_01040201_001"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <%--統一編號--%>
                    <tr class="trEven">
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblCardNo" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040201_002" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 89%">
                            <cc1:CustTextBox ID="txtCardNo1" runat="server" MaxLength="8" checktype="num" Width="80px"
                                onkeydown="entersubmit('btnSelect');"  onkeyup="changeStatus()" BoxName="統一編號一"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtCardNo2" runat="server" MaxLength="4" checktype="num" onkeydown="entersubmit('btnSelect');"
                                Width="40px"  onkeyup="changeStatus()" BoxName="統一編號二"></cc1:CustTextBox>
                            <cc1:CustButton ID="btnSelect" runat="server" CssClass="smallButton" ShowID="01_01040201_027"
                                OnClick="btnSelect_Click" OnClientClick="return checkInputText('pnlText', 0);"
                                DisabledWhenSubmit="False" onkeydown="setfocuschoice('txtCardNo1','txtReceiveNumber');" />
                            <cc1:CustTextBox ID="txtCardNo1Hide" runat="server" MaxLength="8" CssClass="btnHiden"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtCardNo2Hide" runat="server" MaxLength="4" CssClass="btnHiden"></cc1:CustTextBox>
                        </td>
                    </tr>
                </table>
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo4" style="">
                        <tr class="trOdd">
                            <%--收件編號--%>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="lblReceiveNumber" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040201_003"
                                    StickHeight="False" ForeColor="Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 44%" colspan="4">
                                <cc1:CustTextBox ID="txtReceiveNumber" runat="server" MaxLength="10" checktype="num"
                                    onkeydown="entersubmit('btnAdd');" Width="100px" BoxName="收件編號" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <%--自動給號--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="clbl_MemberService" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_066"
                                    StickHeight="False" Style="color: Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustDropDownList ID="cddl_MemberService" runat="server" BoxName="自動給號">
                                </cc1:CustDropDownList>
                            </td>
                            <%--徵信員--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblCheckMan" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_005" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 33%" colspan="2">
                                <cc1:CustLabel ID="lblCheckManText" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="150px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--商店資料--%>
                        <tr class="trEven">
                            <td rowspan="7" align="right" style="width: 12%">
                                <cc1:CustLabel ID="lblShopData" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_030" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--商店別標識(1.總公司已往來 2.總公司未往來 3.獨立店 4.海外公司)--%>
                            <td align="left" style="width: 11%" colspan="9">
                                <cc1:CustRadioButton ID="radSingleMerchant1" runat="server" GroupName="shopType" AutoPostBack="False" Text="總公司已往來" Checked="true" />
                                <cc1:CustRadioButton ID="radSingleMerchant2" runat="server" GroupName="shopType" AutoPostBack="False" Text="總公司未往來" />
                                <%--<cc1:CustRadioButton ID="radSingleMerchant3" runat="server" GroupName="shopType" AutoPostBack="False" Text="獨立店" />
                                <cc1:CustRadioButton ID="radSingleMerchant4" runat="server" GroupName="shopType" AutoPostBack="False" Text="海外公司" />--%><%--20200213-RQ-2019-030155-003-刪除獨立店/海外公司選項--%>
                                <cc1:CustRadioButton ID="radSingleMerchant5" runat="server" GroupName="shopType" AutoPostBack="False"  Text="分期平台" /><%--20200213-RQ-2019-030155-003-新增分期平台選項--%>
                                <cc1:CustRadioButton ID="radSingleMerchant6" runat="server" GroupName="shopType" AutoPostBack="False"  Text="自然人收單" /><%--20210906 自然人收單 新增分期平台選項--%>
                            </td>
                            
                        </tr>
                        <%--統一編號 AML行業編號--%>
                        <tr class="trEven">
                            <td align="left" style="width: 11%" colspan="10">
                             &nbsp;
                                <cc1:CustLabel ID="lblUniNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_073" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtUniNo" runat="server" MaxLength="8" checktype="num" Width="80px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="總公司統一編號"></cc1:CustTextBox>
                                <cc1:CustLabel ID="lblAMLCC" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_072" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtAMLCC" runat="server" MaxLength="7" Width="60px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="AML行業編號"></cc1:CustTextBox>
                                <cc1:CustHiddenField ID="hidAMLCC" runat="server" />
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--商店資料 設立--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblEstablish" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_004" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblEstablishText" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="60px"></cc1:CustLabel>
                            </td>
                            <%--商店資料 資本--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblCapital" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_006" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblCapitalText" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="70px"></cc1:CustLabel>&nbsp
                                <cc1:CustLabel ID="lblMessage" runat="server" CurAlign="left" CurSymbol="&#163;"
                                    FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040201_028"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--商店資料 法律形式--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblOrganization" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040201_008"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOrganizationText" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="30px"></cc1:CustLabel>
                            </td>
                            <%--商店資料 風險--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblRisk" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_010" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="3">
                                <cc1:CustLabel ID="lblRiskText" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="50px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--商店資料 登記名稱--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblRegName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_007" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 88%" colspan="9">
                                <cc1:CustLabel ID="lblRegNameText" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--商店資料 營業名稱--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_009" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 11%" colspan="9">
                                <cc1:CustLabel ID="lblBusinessName" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 77%" colspan="10" align="left">
                                <cc1:CustCheckBox ID="chkBusinessName" runat="server" Enabled="false" AutoPostBack="True" />
                                <cc1:CustLabel ID="lblBusinessName1" runat="server" CurAlign="left" CurSymbol="£"
                                    IsColon="False" FractionalDigit="2" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040201_057"
                                    StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBusinessNameText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="260px" Visible="false"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--另列如右--%>
                            <td align="left" style="width: 11%" colspan="10">
                                <cc1:CustLabel ID="lblRight" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_059" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBusinessNameText_Other" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--負責人 相關資料--%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%" rowspan="6">
                                <cc1:CustLabel ID="lblBossData" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040201_032" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="lblBossData1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_065" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--負責人姓名--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBoss" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_012" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblBossText" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="70px"></cc1:CustLabel>
                            </td>
                            <%--負責人ID--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBossID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_013" StickHeight="False"
                                    ForeColor="Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtBossID" runat="server" MaxLength="10" checktype="ID" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="負責人ID" onfocus="allselect(this);"
                                    InputType="LetterAndInt"></cc1:CustTextBox>
                            </td>
                            <%--負責人電話--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBossTel" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_014" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 22%" colspan="4">
                                <cc1:CustLabel ID="lblBossTel1Text" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="30px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBossTel2Text" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="70px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBossTel3Text" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="40px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd"><%--20190730 長姓名需求-負責人部份--%>
                            <%--負責人姓名--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_080" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="2">                                
                                <cc1:CustCheckBox ID="chkisLongName" runat="server"
                                    AutoPostBack="True" BoxName="長姓名"/>
                                <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblboss_1_L" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="6">
                                <cc1:CustLabel ID="lblboss_1_Pinyin" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                            </td>
                        </tr><%--20190730 長姓名需求-負責人部份--%>
                        <tr class="trOdd">
                            <%--國籍--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblCountryCode" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_068" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="9">
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtCountryCode" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);" 
                                        onkeydown="entersubmit('btnAdd');"  BoxName="國籍" 
                                        Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px;"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropCountryCode" kind="select" runat="server" onclick="simOptionClick4IE('txtCountryCode');" 
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;"></cc1:CustDropDownList>
                                    <cc1:CustHiddenField ID="hidCountryCode" runat="server" />
                                </div>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--護照號碼--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblPassportNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_069" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtPassportNo" runat="server" MaxLength="22" Width="200px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="護照號碼"></cc1:CustTextBox>
                            </td>
                            <%--護照效期--%>
                             <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblPassportExpdt" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_074" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="7">
                                <cc1:CustTextBox ID="txtPassportExpdt" runat="server" MaxLength="8" Width="200px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="護照效期"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--居留證號--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblResidentNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_070" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 22%">
                                <cc1:CustTextBox ID="txtResidentNo" runat="server" MaxLength="22" Width="200px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="居留證號"></cc1:CustTextBox>
                            </td>
                             <%--居留效期--%>
                             <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblResidentExpdt" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_075" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="7">
                                <cc1:CustTextBox ID="txtResidentExpdt" runat="server" MaxLength="8" Width="200px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="居留效期"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--負責人 領換補日--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBossChangeDate" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_032" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="lblBossChangeDate1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040201_061"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblBossChangeDateText" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="60px"></cc1:CustLabel>
                            </td>
                            <%--代號--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBossFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_033" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblBossFlagText" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="20px"></cc1:CustLabel>
                            </td>
                            <%--生日--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBossBirthday" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040201_034"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblBossBirthdayText" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="60px"></cc1:CustLabel>
                            </td>
                            <%--換證點--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBossAt" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_040" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="3">
                                <cc1:CustLabel ID="lblBossAtText" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="60px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--戶籍地址--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblRegAddress" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_015" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 88%" colspan="9">
                                <cc1:CustLabel ID="lblBossRegAddr1Text" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="100px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBossRegAddr2Text" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="220px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBossRegAddr3Text" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="100px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--實際經營者 相關資料--%>
                        <tr class="trEven" style="display:none">
                            <td align="right" style="width: 12%" rowspan="6">
                                <cc1:CustLabel ID="lblOperData" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040201_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="lblOperData1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_065" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--同負責人相關資料--%>
                            <td style="width: 88%" colspan="9">
                                <cc1:CustCheckBox ID="chkOper" runat="server" Enabled="false" AutoPostBack="True" />
                                <cc1:CustLabel ID="lblSameBoss" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_058" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven" style="display:none">
                            <%--實際經營者 姓名--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperman1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040201_016" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="lblOperman2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_062" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOpermanText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="60px"></cc1:CustLabel>
                            </td>
                            <%--實際經營者 ID--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperID1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040201_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_063" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOperIDText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="100px"></cc1:CustLabel>
                            </td>
                            <%--實際經營者 電話--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperTel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040201_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_064" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="5" align="left">
                                <cc1:CustLabel ID="lblOperTelText1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="30px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblOperTelText2" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="70px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblOperTelText3" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="40px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven" style="display:none">
                            <%--實際經營者 領換補日--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperChangeDate1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="lblOperChangeDate2" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040201_061"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOperChangeDateText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="50px"></cc1:CustLabel>
                            </td>
                            <%--代號--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperFlag1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_033" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOperFlagText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="20px"></cc1:CustLabel>
                            </td>
                            <%--生日--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperBirthday1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040201_034"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOperBirthdayText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="50px"></cc1:CustLabel>
                            </td>
                            <%--換證點--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperAt" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_040" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="3">
                                <cc1:CustLabel ID="lblOperAtText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="40px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--另列如下--%>
                        <tr class="trEven" style="display:none">
                            <td align="left" colspan="10">
                                <cc1:CustLabel ID="lblDown" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_060" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven" width="88%" style="display:none">
                            <%--實際經營者姓名--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperman" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040201_016" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;
                                <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_062" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOpermanText_Other" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="60px"></cc1:CustLabel>
                            </td>
                            <%--實際經營者 ID--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040201_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;
                                <cc1:CustLabel ID="lblOperID2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_063" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtOperID" runat="server" MaxLength="10" checktype="ID" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="實際經營者ID" onfocus="allselect(this);"
                                    InputType="LetterAndInt"></cc1:CustTextBox>
                            </td>
                            <%--實際經營者 電話--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperTel" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040201_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;
                                <cc1:CustLabel ID="lblOperTel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_064" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="5">
                                <cc1:CustLabel ID="lblOperTelText1_Other" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="30px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblOperTelText2_Other" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="70px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblOperTelText3_Other" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="40px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--實際經營者 領換補日--%>
                        <tr class="trEven" style="display:none">
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperChangeDate" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;
                                <cc1:CustLabel ID="lblOperChangeDate6" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040201_061"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOperChangeDateText_Other" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="50px"></cc1:CustLabel>
                            </td>
                            <%--代號--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_033" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOperFlagText_Other" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="20px"></cc1:CustLabel>
                            </td>
                            <%--生日--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperBirthday" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040201_034"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOperBirthdayText_Other" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="50px"></cc1:CustLabel>
                            </td>
                            <%--換證點--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperChangeAdd" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040201_040"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="3">
                                <cc1:CustLabel ID="lblOperAtText_Other" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="40px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--聯絡人--%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%" rowspan="2">
                                <cc1:CustLabel ID="lblContactMan" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_017" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--聯絡人姓名--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblContactManName" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040201_041"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblContactManText" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="90px"></cc1:CustLabel>
                            </td>
                            <%--聯絡人電話--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblContactManTel" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040201_050"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 22%" colspan="2">
                                <cc1:CustLabel ID="lblContactManTel1Text" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="25px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblContactManTel2Text" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="58px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblContactManTel3Text" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="36px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblFax" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_018" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--聯絡人傳真--%>
                            <td style="width: 11%" colspan="3">
                                <cc1:CustLabel ID="lblFax1Text" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="30px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblFax2Text" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="70px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd"><%--20190730 長姓名需求-聯絡人部份--%>
                            <%--聯絡人姓名--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="CustLabel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_080" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="2">                                
                                <cc1:CustCheckBox ID="chkisLongName_c" runat="server"
                                    AutoPostBack="True" BoxName="長姓名"/>
                                <cc1:CustLabel ID="CustLabel9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_083" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblcontact_man_L" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="CustLabel10" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_084" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="6">
                                <cc1:CustLabel ID="lblcontact_man_Pinyin" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                            </td>
                        </tr><%--20190730 長姓名需求-聯絡人部份--%>
                        <%--E-MAIL--%>
                        <tr class="trEven">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="lblEmail" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_071" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="9">
                                <cc1:CustTextBox ID="txtEmailFront" runat="server" Width="200px" 
                                    onkeydown="entersubmit('btnAdd');" BoxName="E-MAIL"></cc1:CustTextBox>
                                    @
                                <cc1:CustRadioButton ID="radGmail" runat="server" AutoPostBack="False" GroupName="email" Text="gmail.com" />
                                <cc1:CustRadioButton ID="radYahoo" runat="server" AutoPostBack="False" GroupName="email" Text="yahoo.com.tw" />
                                <cc1:CustRadioButton ID="radHotmail" runat="server" AutoPostBack="False" GroupName="email" Text="hotmail.com" />
                                <cc1:CustRadioButton ID="radOther" runat="server" AutoPostBack="False" GroupName="email" Text="其他：" />
                                <cc1:CustTextBox ID="txtEmailOther" runat="server" Width="200px" 
                                    onkeydown="entersubmit('btnAdd');" BoxName="E-MAIL"></cc1:CustTextBox>
                                <cc1:CustHiddenField ID="hidEmailFall" runat="server" />
                            </td>
                        </tr>
                        <%--地址資料--%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%" rowspan="4">
                                <cc1:CustLabel ID="lblAddData" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_042" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--登記地址--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBookAddress" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_019" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 88%" colspan="8">
                                <cc1:CustLabel ID="lblREG_ZIP_CODE" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="80px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBookAddr1Text" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="100px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBookAddr2Text" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="220px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBookAddr3Text" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="100px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--營業地址--%>
                        <tr class="trOdd">
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="CustLabel11" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040201_020"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 11%" colspan="9">
                                <cc1:CustLabel ID="lblBusinessAddress" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--同登記地址--%>
                            <td colspan="10">
                                <cc1:CustCheckBox ID="chkAddress" runat="server" Enabled="false" AutoPostBack="True" />
                                <cc1:CustLabel ID="lblAdd" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_055" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBusinessZipText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="" Width="50px" Visible="false"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBusinessAddrText1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="" Width="100px" Visible="false"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBusinessAddrText2" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="" Width="220px" Visible="false"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBusinessAddrText3" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="" Width="100px" Visible="false"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--另列如右--%>
                            <td colspan="10">
                                <cc1:CustLabel ID="lblRight2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_059" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblZipText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="50px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBusinessAddrText1_Other" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="" Width="100px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBusinessAddrText2_Other" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="" Width="220px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBusinessAddrText3_Other" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="" Width="100px"></cc1:CustLabel>
                                <cc1:CustButton ID="btnSearchZip" Enabled="false" runat="server" CssClass="smallButton"
                                    ShowID="01_01040301_088" DisabledWhenSubmit="False" />
                            </td>
                        </tr>
                        <%--JCIC--%>
                        <tr class="trEven">
                            <%--JCJC查詢--%>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="lblJCIC" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_043" StickHeight="False"
                                    ForeColor="Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 16%" colspan="2">
                                <cc1:CustTextBox ID="txtJCIC" runat="server" MaxLength="1" Width="100px" onkeydown="entersubmit('btnAdd');"
                                    checktype="numandletter" BoxName="JCIC查詢" onfocus="allselect(this);"></cc1:CustTextBox>
                                <cc1:CustLabel ID="lblMessage1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040201_031" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--Y_特店跨行匯費(6116)--%>
                            <td align="right" style="width: 18%" colspan="1">
                                <cc1:CustLabel ID="lblGrantFeeFlag" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_067"
                                    StickHeight="False" ForeColor="Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 16%" colspan="1">
                                <cc1:CustTextBox ID="txtGrantFeeFlag" runat="server" MaxLength="1" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" checktype="numandletter" BoxName="Y_特店跨行匯費(6116)"></cc1:CustTextBox>
                            </td>
                            <%--Y_MPOS特店系統服務費免收註記(6086)F001--%>
                            <td align="right" style="width: 20%" colspan="4">
                                <cc1:CustLabel ID="lblMposFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_068" StickHeight="False"
                                    ForeColor="Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 18%">
                                <cc1:CustTextBox ID="txtMposFlag" runat="server" MaxLength="1" Width="100px" onkeydown="entersubmit('btnAdd');"
                                    checktype="numandletter" BoxName="Y_MPOS特店系統服務費免收註記(6086)F001"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--帳戶資料--%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%" rowspan="2">
                                <cc1:CustLabel ID="lblAccounts" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_044" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--銀行(中文)--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBank" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_021" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 33%" colspan="3">
                                <cc1:CustLabel ID="lblBankText" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="80px"></cc1:CustLabel>
                            </td>
                            <%--分行(中文)--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBranchBank" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_022" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 33%" colspan="4">
                                <cc1:CustLabel ID="lblBranchBankText" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="140px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--戶名--%>
                        <tr class="trOdd">
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_023" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 88%" colspan="9">
                                <cc1:CustLabel ID="lblNameText" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="300px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--帳單資料--%>
                        <tr class="trEven">
                            <td align="right" style="width: 12%; height: 33px;">
                                <cc1:CustLabel ID="lblPrev" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_045" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--帳單內容--%>
                            <td align="right" style="width: 11%; height: 33px;">
                                <cc1:CustLabel ID="lblPrevDesc" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_046" StickHeight="False"
                                    ForeColor="Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 33%; height: 33px;" colspan="3">
                                <cc1:CustTextBox ID="txtPrevDesc" runat="server" MaxLength="4" Width="100px"  onblur="changeFullType(this);"
                                    onkeydown="entersubmit('btnAdd');" BoxName="帳單內容" checktype="fulltype" onpaste="paste();"
                                    onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <%--發票週期--%>
                            <td align="right" style="width: 11%; height: 33px;">
                                <cc1:CustLabel ID="lblInvoiceCycle" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040201_025"
                                    StickHeight="False" ForeColor="Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%; height: 33px;">
                                <cc1:CustTextBox ID="txtInvoiceCycle" runat="server" MaxLength="2" checktype="num"
                                    Width="20px" onkeydown="entersubmit('btnAdd');" BoxName="發票週期" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <%--紅利週期(M/D)--%>
                            <td align="right" style="width: 11%; height: 33px;"colspan="2">
                                <cc1:CustLabel ID="lblRedeemCycle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_047" StickHeight="False"
                                    ForeColor="Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%; height: 33px;" >
                                <cc1:CustTextBox ID="txtRedeemCycle" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="30px" onkeydown="entersubmit('btnAdd');" BoxName="紅利週期(M/D)" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--推廣員--%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="lblPopMan" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040201_024" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 88%" colspan="9">
                                <cc1:CustLabel ID="lblPopManText" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="50px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--資料最後異動--%>
                        <tr class="trEven">                            
                            <%--資料最後異動MAKER--%>
                            <td align="right" >
                                <cc1:CustLabel ID="lbLAST_UPD_MAKER" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_086"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 16%; height: 33px;" colspan="2">
                                <cc1:CustTextBox ID="txtLAST_UPD_MAKER" runat="server" MaxLength="9" checktype="numandletter"
                                    Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動MAKER" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <%--資料最後異動CHECKER--%>
                            <td align="right" >
                                <cc1:CustLabel ID="lbLAST_UPD_CHECKER" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_087" StickHeight="False"
                                    ></cc1:CustLabel>
                            </td>
                            <td style="width: 18%; height: 33px;"colspan="2" >
                                <cc1:CustTextBox ID="txtLAST_UPD_CHECKER" runat="server" MaxLength="9" checktype="numandletter"
                                    Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動CHECKER" onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                            <%--資料最後異動分行--%>
                            <td align="right" >
                                <cc1:CustLabel ID="lbLAST_UPD_BRANCH" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_085" StickHeight="False"
                                    ></cc1:CustLabel>
                            </td>
                            <td style="width: 16%; height: 33px;" colspan="3">
                                <cc1:CustTextBox ID="txtLAST_UPD_BRANCH" runat="server" MaxLength="4" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動分行" checktype="numandletter"
                                    onfocus="allselect(this);"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td nowrap colspan="6" style="height: 1px">
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo3" style="">
                        <tr class="itemTitle1">
                            <td align="center">
                                <cc1:CustButton ID="btnAdd" runat="server" CssClass="smallButton" ShowID="01_01040201_026"
                                    OnClick="btnAdd_Click" OnClientClick="return checkInputText('pnlText', 1);" DisabledWhenSubmit="False"
                                    onkeydown="setfocus('txtCardNo1');" />&nbsp;&nbsp
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <cc1:CustButton ID="btnHiden" OnClick="btnHiden_Click" runat="server" CssClass="btnHiden"
            DisabledWhenSubmit="False"></cc1:CustButton>
        <cc1:CustButton ID="btnAddHiden" runat="server" CssClass="btnHiden" DisabledWhenSubmit="False"
            OnClick="btnAddHiden_Click" OnClientClick="return checkAdd();"></cc1:CustButton>
    </form>
</body>
</html>