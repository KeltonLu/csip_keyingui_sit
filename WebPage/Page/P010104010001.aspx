<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010104010001.aspx.cs" Inherits="P010104010001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <%-- 20201123_Ares_Stanley-修正格式; 20210125_Ares_Stanley-修正格式; 20210302_Ares_Stanley-調整顯示內容; 20210329_Ares_Stanley-調整半形轉全形失效; 20210408_Ares_Stanley-調整半形轉全形失效; 20210415_Ares_Stanley-調整全半形轉換; 20210902_Ares_Stanley:移除Email前30後19長度限制, 改為總長度50 --%>
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript">
        /*20200219-RQ-2019-030155-003 移除3.獨立店選項
    $(document).ready(function() {
        
        $('#radSingleMerchant3').live('click', function(){
            if (document.getElementById('radSingleMerchant3').checked)
                document.getElementById('txtUniNo').value = document.getElementById('txtCardNo1').value.Trim();
        });
    });
    */
    //*客戶端檢核
    function checkInputText(id, intType)
    {
        //*統一編號不能輸入空
        if(document.getElementById('txtCardNo1').value.Trim() == "")
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
            alert('統一編號只能輸入數字');
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
        
        // 總公司已往來或總公司未往來，需手動填入統一編號
        //20200219-RQ-2019-030155-003 5.分期平台時，總公司統編也視為必輸欄位
        if (document.getElementById('radSingleMerchant1').checked || document.getElementById('radSingleMerchant2').checked || document.getElementById('radSingleMerchant5').checked || document.getElementById('radSingleMerchant6').checked) {
            var uniNoLen = document.getElementById('txtUniNo').value.Trim().length;
            if (uniNoLen == 0) {
                alert('請輸入總公司統一編號!');
                document.getElementById('txtUniNo').focus();
                return false;
            }
            else {
                if (uniNoLen != 8) {
                    alert('統編請輸入8碼數字!');
                    document.getElementById('txtUniNo').focus();
                    return false;
                }
            }
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
                
                if(document.getElementById('txtCardNo1').value.Trim() + document.getElementById('txtCardNo2').value.Trim() == obj1.value.Trim() + obj2.value.Trim())
                {
                    alert('新統編不可同 統一編號! ');
                    obj1.focus();
                    return false;
                }
            }
            
            //*收件編號不為空
            if (document.getElementById('txtReceiveNumber').value.Trim() == "") {
                alert('收件編號不為空! ');
                document.getElementById('txtReceiveNumber').focus();
                return false;
            }                        
            
            //*收件編號需輸入7碼
            if(document.getElementById('txtReceiveNumber').value.Trim().length < 7)
            {
                alert('收件編號前7碼應與系統日相同');
                document.getElementById('txtReceiveNumber').focus();
                return false;
            }
            
            if(!isNum(document.getElementById('txtReceiveNumber').value.Trim()))
            {
                alert('收件編號前7碼應與系統日相同');
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
            
            // 商店別標識(1.總公司已往來 2.總公司未往來 3.獨立店 4.海外公司)
            //20200219-RQ-2019-030155-003 移除3.獨立店 4.海外公司，新增5.分期平台
            //20210906 自然人收單 新增分期平台選項
            //            if (!document.getElementById('radSingleMerchant1').checked && !document.getElementById('radSingleMerchant2').checked && !document.getElementById('radSingleMerchant3').checked && !document.getElementById('radSingleMerchant4').checked)
            if (!document.getElementById('radSingleMerchant1').checked && !document.getElementById('radSingleMerchant2').checked && !document.getElementById('radSingleMerchant5').checked && !document.getElementById('radSingleMerchant6').checked)
            {
                //alert('請選擇總公司已(未)往來或獨立店!');
                alert('請選擇總公司已(未)往來或分期平台!');
                return false;
            }
            
            // 總公司已往來或總公司未往來，需手動填入統一編號
            //20200219-RQ-2019-030155-003 5.分期平台時，總公司統編也視為必輸欄位
            //if (document.getElementById('radSingleMerchant1').checked || document.getElementById('radSingleMerchant2').checked)
            if (document.getElementById('radSingleMerchant1').checked || document.getElementById('radSingleMerchant2').checked || document.getElementById('radSingleMerchant5').checked || document.getElementById('radSingleMerchant6').checked)
            {
                var uniNoLen = document.getElementById('txtUniNo').value.Trim().length;
                if (uniNoLen == 0)
                {
                    alert('請輸入統一編號!');
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
            
            // 法律形式
            var allOrganization = document.getElementById('hidOrganization').value.Trim();
            var organization = document.getElementById('txtOrganization').value.Trim();
            if (organization.length > 0)
            {
                if (organization.length != 2) {
                    alert('法律形式輸入2碼數字!');
                    document.getElementById('txtOrganization').focus();
                    return false;
                }
                
                if (allOrganization.indexOf(organization) == -1)
                {
                    alert('法律形式不存在!');
                    document.getElementById('txtOrganization').focus();
                    return false;
                }
            }
            
            // 國籍
            var allCountryCode = document.getElementById('hidCountryCode').value.Trim();
            var countryCode = document.getElementById('txtCountryCode').value.toUpperCase().Trim();
            if (countryCode.length > 0) {
                if (allCountryCode.indexOf(countryCode) == -1) {
                    alert('國籍不存在!');
                    document.getElementById('txtCountryCode').focus();
                    return false;
                }
            }
            else {
                alert('國籍欄位不得為空，請輸入國籍!');
                document.getElementById('txtCountryCode').focus();
                return false;
            }
            
            document.getElementById('txtCountryCode').value = countryCode;
            
            //20191001 10月需求-效期無需檢核↓
            // 護照號碼、護照效期
            //var passportNo = document.getElementById('txtPassportNo').value.Trim();
            var passportExpdt = document.getElementById('txtPassportExpdt').value.Trim();

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
            var residentExpdt = document.getElementById('txtResidentExpdt').value.Trim();
            
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
            
            // 護照號碼、居留證號
            //if(countryCode != "TW"){
            //    if ((passportExpdt.length == 0) && (residentExpdt.length == 0)){
            //        alert('請輸入護照號碼 或 居留證號!');
            //        document.getElementById('txtPassportNo').focus();
            //        return false;
            //    }
            //}
            
            if (passportExpdt != "" && passportExpdt.length != 8 && passportExpdt != "X") {
                alert("護照效期請輸入8碼!");
                document.getElementById('txtPassportExpdt').focus();
                return false;
            }
        
            if (residentExpdt != "" && residentExpdt.length != 8 && residentExpdt != "X") {
                alert("統一證號效期請輸入8碼!");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
                document.getElementById('txtResidentExpdt').focus();
                return false;
            }
            
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
            
            // 2015/04/21 by Eric
            if ($("#txtEstablish").val().length < 7)
            {
                alert('設立輸入值為民國年YYYMMDD共7碼數字');
                $("#txtEstablish").focus();
                return false;
            }

            //*當設立不為空時，需有7位
            if(document.getElementById('txtEstablish').value.Trim().length != 0 && document.getElementById('txtEstablish').value.Trim().length != 7)
            {
                alert('設立請輸入7碼數字! ');
                document.getElementById('txtEstablish').focus();
                return false;
            }
            
            //*當設立不為空時，第4位只能為0,1
            if(document.getElementById('txtEstablish').value.Trim().length != 0 && !(document.getElementById('txtEstablish').value.substring(3,4) == "0" || 
                                                                                     document.getElementById('txtEstablish').value.substring(3,4) == "1"))
            {
                alert('設立第4位只能為0,1! ');
                document.getElementById('txtEstablish').focus();
                return false;
            }
            
            // 2015/4/21 by Eric ===>
            if(document.getElementById('txtEstablish').value.substring(3,4) == "1" && document.getElementById('txtEstablish').value.substring(4,5) > 2)
            {
                alert('設立輸入的月份錯誤!');
                document.getElementById('txtEstablish').focus();
                return false;
            }
            
            var yyy = $("#txtEstablish").val().substring(0,3);
            var mm = $("#txtEstablish").val().substring(3,5);
            var dd = $("#txtEstablish").val().substring(5,7);
            var date = new Date();
            var txtEstablish = new Date(parseInt(yyy) + 1911, parseInt(mm) - 1, date.getDate());
            
            if (yyy <= 0 || mm <= 0 || dd <= 0)
            {
                alert('設立輸入格式錯誤');
                return false;
            }
            
            if (txtEstablish > date)
            {
                alert('設立輸入值不可為未來日');
                return false;
            }
            
            // <============
            if(document.getElementById('txtCheckMan').value.Trim().length != 4)
            {
                  alert('徵信員欄位請輸4碼.');
                  document.getElementById('txtCheckMan').focus();
                  return false;
            }
            
            //*商店登記名稱不為空
            if(document.getElementById('txtRegName').value.Trim().length == 0)
            {
                alert('商店登記名稱不為空! ');
                document.getElementById('txtRegName').focus();
                return false;
            }
            
            //*商店營業名稱不能為空
            if (!document.getElementById('chkBusinessName').checked) {
                if (document.getElementById('txtBusinessName').value.Trim().length == 0) {
                    alert('商店營業名稱不能為空');
                    return false;
                }
            }

            //*風險不為空
            if(document.getElementById('txtRisk').value.Trim().length == 0)
            {
                alert('風險不為空! ');
                document.getElementById('txtRisk').focus();
                return false;
            }
            
            //*風險為3碼字符
            if(!(document.getElementById('txtRisk').value.Trim().length == 3))
            {
                alert('風險請輸入3碼字符,第一個byte為A~Z,后兩個byte為02~98! ');
                document.getElementById('txtRisk').focus();
                return false;
            }
            
            //*風險第一個byte為A~Z
            if(!(document.getElementById('txtRisk').value.Trim().charCodeAt(0) >= 65 && document.getElementById('txtRisk').value.Trim().charCodeAt(0) <= 90))
            {
                alert('風險第一個byte請輸入A~Z! ');
                document.getElementById('txtRisk').focus();
                return false;
            }
            
            //*風險后兩個byte為數字
            if(!isNum(document.getElementById('txtRisk').value.substring(1,3)))
            {
                alert('風險后兩個byte為數字! ');
                document.getElementById('txtRisk').focus();
                return false;
            }
            
            //*風險后兩個byte為02~98
            if(!(Number(document.getElementById('txtRisk').value.substring(1,3)) >= 2 && Number(document.getElementById('txtRisk').value.substring(1,3)) <= 98))
            {
                alert('風險后兩個byte為02~98! ');
                document.getElementById('txtRisk').focus();
                return false;
            }
            
            //*負責人姓名不為空
            if(document.getElementById('txtBoss').value.Trim().length == 0)
            {
                alert('負責人姓名不為空! ');
                document.getElementById('txtBoss').focus();
                return false;
            }
            
            //*負責人ID不為空
            if(document.getElementById('txtBossID').value.Trim().length == 0)
            {
                alert('負責人ID不為空! ');
                document.getElementById('txtBossID').focus();
                return false;
            }
            
            if(document.getElementById('txtBossTel1').value.Trim() != "" && document.getElementById('txtBossTel2').value.Trim() == "")
            {
                alert('負責人電話欄位必須同時輸入');
                document.getElementById('txtBossTel2').focus();
                return false;
            }
            
            if(document.getElementById('txtBossTel2').value.Trim() != "" && document.getElementById('txtBossTel1').value.Trim() == "")
            {
                alert('負責人電話欄位必須同時輸入');
                document.getElementById('txtBossTel1').focus();
                return false;
            }
            
            if(document.getElementById('txtBossTel3').value.Trim() != "" && (document.getElementById('txtBossTel1').value.Trim() == "" || document.getElementById('txtBossTel2').value.Trim() == ""))
            {
                alert('負責人電話欄位必須同時輸入');
                document.getElementById('txtBossTel1').focus();
                return false;
            }
            
            //*戶籍地址一不為空
            if(document.getElementById('txtRegAddr1').value.Trim().length == 0)
            {
                alert('戶籍地址一不為空! ');
                document.getElementById('txtRegAddr1').focus();
                return false;
            }
            
            if(document.getElementById('txtOperTel1').disabled == false)
            {
                if(document.getElementById('txtOperTel1').value.Trim() != "" && document.getElementById('txtOperTel2').value.Trim() == "")
                {
                    alert('實際經營人電話欄位必須同時輸入');
                    document.getElementById('txtOperTel2').focus();
                    return false;
                }
                
                if(document.getElementById('txtOperTel2').value.Trim() != "" && document.getElementById('txtOperTel1').value.Trim() == "")
                {
                    alert('實際經營人電話欄位必須同時輸入');
                    document.getElementById('txtOperTel1').focus();
                    return false;
                }
                
                if(document.getElementById('txtOperTel3').value.Trim() != "" && (document.getElementById('txtOperTel1').value.Trim() == "" || document.getElementById('txtOperTel2').value.Trim() == ""))
                {
                    alert('實際經營人電話欄位必須同時輸入');
                    document.getElementById('txtOperTel1').focus();
                    return false;
                }
            }
            
            if(document.getElementById('txtContactManTel1').value.Trim() != "" && document.getElementById('txtContactManTel2').value.Trim() == "")
            {
                alert('聯絡人電話欄位必須同時輸入');
                document.getElementById('txtContactManTel2').focus();
                return false;
            }
            
            if(document.getElementById('txtContactManTel2').value.Trim() != "" && document.getElementById('txtContactManTel1').value.Trim() == "")
            {
                alert('聯絡人電話欄位必須同時輸入');
                document.getElementById('txtContactManTel1').focus();
                return false;
            }
            
            if(document.getElementById('txtContactManTel3').value.Trim() != "" && (document.getElementById('txtContactManTel1').value.Trim() == "" || document.getElementById('txtContactManTel2').value.Trim() == ""))
            {
                alert('聯絡人電話欄位必須同時輸入');
                document.getElementById('txtContactManTel1').focus();
                return false;
            }
            
            //*銀行別不為空
            if(document.getElementById('txtBank').value.Trim().length == 0)
            {
                alert('銀行不為空! ');
                document.getElementById('txtBank').focus();
                return false;
            }
            
            //*分行別不為空
            if(document.getElementById('txtBranchBank').value.Trim().length == 0)
            {
                alert('分行不為空! ');
                document.getElementById('txtBranchBank').focus();
                return false;
            }
            
            //*戶名不為空
            if(document.getElementById('txtName').value.Trim().length == 0)
            {
                alert('戶名不為空! ');
                document.getElementById('txtName').focus();
                return false;
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
            
            // 2015/1/22 刪除右下角紅利週期欄位 by Eric
            //if(document.getElementById('txtRedeemCycle').value.toUpperCase().Trim() == "D")
            //{
            //	if(!confirm('確定要將紅利週期修改為 D 嗎？'))
            //	{
            //		return false;
            //	}
            //}

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
            
            //Add By CTCB-Carolyn
            var today = new Date();
            var sYear = (today.getFullYear()-1911).toString();
            var sMonth = (today.getMonth()+1).toString();
            var sDate = today.getDate().toString();
            if(sYear.length<3){
                sYear = "0" + sYear;
            }
            if(sDate.length<2){
                sDate = "0" + sDate;
            }
            if(sMonth.length<2){
                sMonth = "0" + sMonth;
            }
            if(sDate.length<2){
                sDate = "0" + sDate;
            }
            
            var sSysDate = sYear + sMonth + sDate;
            var sReceiveNumber = document.getElementById('txtReceiveNumber').value.Trim();
            
            if(sReceiveNumber.substr(0,7) != sSysDate)
            {
                alert('收件編號前7碼應與系統日相同');
                document.getElementById('txtReceiveNumber').focus();
                return false;
            }
            
            //20190805 modify by Peggy
            if (document.getElementById('chkisLongName').checked && document.getElementById('txtboss_1_L').value.Trim().length == 0)
            {
                alert('負責人中文長姓名不可為空');
                document.getElementById('txtboss_1_L').focus();
                return false;
            }
            if (document.getElementById('chkisLongName_c').checked && document.getElementById('txtcontact_man_L').value.Trim().length == 0)
            {
                alert('聯絡人中文長姓名不可為空');
                document.getElementById('txtcontact_man_L').focus();
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

    //*中文、英文、數字檢核
    function checkLableType()
    {

        
        if(document.getElementById('chkOper').checked)
        {
            //if(document.getElementById('lblOperTelText1').innerText.Trim() != "" && document.getElementById('lblOperTelText2').innerText.Trim() == "" )
            //{
            //    alert('實際經營人電話欄位必須同時輸入');
            //    return false;
            //}
            
            //if(document.getElementById('lblOperTelText2').innerText.Trim() != "" && document.getElementById('lblOperTelText1').innerText.Trim() == "" )
            //{
            //    alert('實際經營人電話欄位必須同時輸入');
            //    return false;
            //}
            
            //if(document.getElementById('lblOperTelText3').innerText.Trim() != "" && (document.getElementById('lblOperTelText1').innerText.Trim() == "" || document.getElementById('lblOperTelText2').innerText.Trim() == ""))
            //{
            //    alert('實際經營人電話欄位必須同時輸入');
            //    return false;
            //}
            
            //*檢核數字
            //if(!isNum(document.getElementById('lblOperTelText1').innerText))
            //{
            //    alert('實際經營者電話一欄位只能輸入數字');
            //    return false; 
            //}
            
            //*檢核數字
            //if(!isNum(document.getElementById('lblOperTelText2').innerText))
            //{
            //    alert('實際經營者電話二欄位只能輸入數字');
            //    return false;
            //}
            
            //*檢核數字
            //if(!isNum(document.getElementById('lblOperTelText3').innerText)&& document.getElementById('lblOperTelText3').innerText.toUpperCase() != "X")
            //{
            //    alert('實際經營者電話三欄位只能輸入數字');
            //    return false;
            //}
            
            //*檢核數字
            //if(!isNum(document.getElementById('lblOperChangeDateText').innerText))
            //{
            //    alert('實際經營者領換補日欄位只能輸入數字');
            //    return false;
            //}
            
            //*檢核英文和數字
            //if(!ischinese(document.getElementById('lblOperFlagText').innerText))
            //{
            //    alert('代號欄位只能輸入英文和數字');
            //    return false;
            //}
            
            //*檢核數字
            //if(!isNum(document.getElementById('lblOperBirthdayText').innerText))
            //{
            //    alert('生日欄位只能輸入數字');
            //    return false;
            //}
            
            //*檢核身分證號碼
            //var inputText = document.getElementById('lblOperIDText').innerText;
            
            //*身分證號碼長度不為10
            //if(!checkID(inputText))
            //{
            //    return false;
            //}
        }
        
        return true;
    }

    //*統一編號(1)、統一編號(2)欄位輸入值改變
    function changeStatus()
    {
        if(document.getElementById('txtCardNo1').value.Trim() != document.getElementById('txtCardNo1Hide').value.Trim() || document.getElementById('txtCardNo2').value.Trim() != document.getElementById('txtCardNo2Hide').value.Trim())
        {
            document.getElementById('lblOpermanText').innerText = "";
            document.getElementById('lblOperIDText').innerText = "";
            document.getElementById('lblOperTelText1').innerText = "";
            document.getElementById('lblOperTelText2').innerText = "";
            document.getElementById('lblOperTelText3').innerText = "";
            document.getElementById('lblOperChangeDateText').innerText = "";
            document.getElementById('lblOperFlagText').innerText = "";
            document.getElementById('lblOperBirthdayText').innerText = "";
            document.getElementById('lblOperAtText').innerText = "";

            
            document.getElementById('lblZipText').innerText = "";
            
            document.getElementById('chkBusinessName').checked = false;
            document.getElementById('chkOper').checked = false;
            document.getElementById('chkAddress').checked = false;
            setControlsDisabled('pnlText');
        }
        
        document.getElementById('txtCardNo1Hide').value = document.getElementById('txtCardNo1').value;
        document.getElementById('txtCardNo2Hide').value = document.getElementById('txtCardNo2').value;
    }

    //*選擇按鈕上按Tab鍵設置焦點
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

    function setfocusmove1()
    {
        if(event.keyCode==9)
        {
            event.returnValue=false;
            var obj1 = document.getElementById('txtAMLCC');
            obj1.focus();
        }
    }
    //*新增按鈕Tab鍵設置焦點
    function movefocus()
    {
        if(event.keyCode==9)
        {
            event.returnValue = false;
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
        <cust:image runat="server" ID="image1" />
        &nbsp;
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                    <tr class="itemTitle">
                        <td colspan="2">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01040101_001"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <%--統一編號--%>
                    <tr class="trOdd">
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblCardNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040101_002" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 89%">
                            <cc1:CustTextBox ID="txtCardNo1" runat="server" MaxLength="8" checktype="num" Width="80px"
                                onkeydown="entersubmit('btnSelect');" onkeyup="changeStatus()" BoxName="統一編號一"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtCardNo2" runat="server" MaxLength="4" checktype="num" Width="40px"
                                onkeydown="entersubmit('btnSelect');"  onkeyup="changeStatus()" BoxName="統一編號二"></cc1:CustTextBox>
                            <cc1:CustButton ID="btnSelect" runat="server" CssClass="smallButton" ShowID="01_01040101_027"
                                OnClick="btnSelect_Click"  OnClientClick="return checkInputText('pnlText', 0);"
                                DisabledWhenSubmit="False" onkeydown="setfocusmove();" BoxName="查詢" />
                            <cc1:CustTextBox ID="txtCardNo1Hide" runat="server" MaxLength="8" CssClass="btnHiden"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtCardNo2Hide" runat="server" MaxLength="4" CssClass="btnHiden"></cc1:CustTextBox>
                        </td>
                    </tr>
                </table>
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo4" style="">
                        <%--新統一編號--%>
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
                            <%--收件編號--%>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="lblReceiveNumber" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_003"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 22%" colspan="2">
                                <cc1:CustTextBox ID="txtReceiveNumber" runat="server" MaxLength="10" checktype="num"
                                    onkeydown="entersubmit('btnAdd');" Width="100px" BoxName="收件編號"></cc1:CustTextBox>
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
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_005" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 33%" colspan="3">
                                <cc1:CustTextBox ID="txtCheckMan" runat="server" MaxLength="4" checktype="num" Width="50px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="徵信員"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--商店資料--%>
                        <tr class="trEven">
                            <td rowspan="7" align="right" style="width: 12%">
                                <cc1:CustLabel ID="lblShopData" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_030" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--商店別標識(1.總公司已往來 2.總公司未往來 3.獨立店)--%>
                            <td align="left" style="width: 11%" colspan="4">
                                <cc1:CustRadioButton ID="radSingleMerchant1" runat="server" GroupName="shopType" AutoPostBack="True" OnCheckedChanged="radSingleMerchant1_CheckedChanged" Text="總公司已往來" Checked="true" />
                                <cc1:CustRadioButton ID="radSingleMerchant2" runat="server" GroupName="shopType" AutoPostBack="True" OnCheckedChanged="radSingleMerchant2_CheckedChanged" Text="總公司未往來" />
                                <%--<cc1:CustRadioButton ID="radSingleMerchant3" runat="server" GroupName="shopType" AutoPostBack="True" OnCheckedChanged="radSingleMerchant3_CheckedChanged" Text="獨立店" />
                                <cc1:CustRadioButton ID="radSingleMerchant4" runat="server" GroupName="shopType" AutoPostBack="True" OnCheckedChanged="radSingleMerchant4_CheckedChanged" Text="海外公司" />--%><%--20200213-RQ-2019-030155-003-刪除海外公司選項--%>
                                <cc1:CustRadioButton ID="radSingleMerchant5" runat="server" GroupName="shopType" AutoPostBack="false" OnCheckedChanged="radSingleMerchant5_CheckedChanged" Text="分期平台" /><%--20200213-RQ-2019-030155-003-新增分期平台選項--%>
                                <cc1:CustRadioButton ID="radSingleMerchant6" runat="server" GroupName="shopType" AutoPostBack="false" OnCheckedChanged="radSingleMerchant6_CheckedChanged" Text="自然人收單" /><%--20210906-新增分期平台選項--%>

                            </td>
                            <td align="left" style="width: 11%" colspan="4"><%--20200812-RQ-2020-021027-001-新增獨資合夥異動負責人重簽。pass不合作檢核--%>
                                <cc1:CustCheckBox ID="chkUnsightCooperation" runat="server" Text="獨資合夥異動負責人重簽或不合作客戶新開店" BoxName="獨資合夥異動負責人重簽或不合作客戶新開店" />
                                </td>
                        </tr>
                        <%--統一編號 AML行業編號--%>
                        <tr class="trEven">
                            <td align="left" style="width: 11%" colspan="8">
                                &nbsp;
                                <cc1:CustLabel ID="lblUniNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_076" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtUniNo" runat="server" MaxLength="8" checktype="num" Width="80px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="總公司統一編號"></cc1:CustTextBox>
                                <cc1:CustButton ID="btnSelectShopInfo" runat="server" CssClass="smallButton" ShowID="01_01040101_079"
                                    OnClick="btnSelect_ShopInfo" OnClientClick="return checkInputText('pnlText', 0);"
                                    DisabledWhenSubmit="False" onkeydown="setfocusmove1();" BoxName="查詢商店資料" />
                                <cc1:CustLabel ID="lblAMLCC" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_075" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtAMLCC" runat="server" MaxLength="7" Width="60px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="AML行業編號"></cc1:CustTextBox>
                                <cc1:CustHiddenField ID="hidAMLCC" runat="server" />
                                
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--設立--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblEstablish" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_004" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtEstablish" runat="server" MaxLength="7" checktype="num" Width="60px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="設立"></cc1:CustTextBox>
                            </td>
                            <%--資本--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblCapital" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_006" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtCapital" runat="server" MaxLength="6" checktype="num" Width="70px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="資本"></cc1:CustTextBox>&nbsp
                                <cc1:CustLabel ID="lblMessage" runat="server" CurAlign="left" CurSymbol="&#163;"
                                    FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_028"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--法律形式--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblOrganization" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_008"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtOrganization" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);" 
                                        onkeydown="entersubmit('btnAdd');" BoxName="法律形式" 
                                        Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px;"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropOrganization" kind="select" runat="server" onclick="simOptionClick4IE('txtOrganization');" 
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;"></cc1:CustDropDownList>
                                    <cc1:CustHiddenField ID="hidOrganization" runat="server" />
                                </div>
                            </td>
                            <%--風險--%>
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
                            <%--登記名稱--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblRegName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_007" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 77%" colspan="7">
                                <cc1:CustTextBox ID="txtRegName" runat="server" MaxLength="19" Width="260px" checktype="fulltype"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="登記名稱"
                                    onpaste="paste();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--營業名稱--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="CustLabel10" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_009"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 11%" colspan="7">
                                <cc1:CustLabel ID="lblBusinessName" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--同登記名稱--%>
                            <td style="width: 77%" colspan="8" align="left">
                                <cc1:CustCheckBox ID="chkBusinessName" runat="server" OnCheckedChanged="chkBusinessName_CheckedChanged"
                                    AutoPostBack="True" BoxName="同登記名稱(CheckBox)" />
                                <cc1:CustLabel ID="lblBusinessName1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_057"
                                    StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBusinessNameText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="260px" BoxName="同登記名稱(Label)" Visible="false"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--另列如右--%>
                            <td align="left" style="width: 11%" colspan="8">
                                <cc1:CustLabel ID="lblRight" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_059" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtBusinessName" runat="server" MaxLength="19" checktype="fulltype"
                                    Width="260px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="營業名稱" onpaste="paste();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--負責人相關資料--%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%" rowspan="7">
                                <cc1:CustLabel ID="lblBossData" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040101_032" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="lblBossData1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_065" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--負責人姓名--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBoss" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_012" StickHeight="False"
                                    ForeColor="Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtBoss" runat="server" MaxLength="4" checktype="fulltype" Width="70px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人姓名"
                                    onpaste="paste();"></cc1:CustTextBox>
                            </td>
                            <%--負責人ID--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBossID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_013" StickHeight="False"
                                    ForeColor="Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtBossID" runat="server" MaxLength="10" checktype="ID" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="負責人ID" InputType="LetterAndInt"></cc1:CustTextBox>
                            </td>
                            <%--負責人電話--%>
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
                                <cc1:CustTextBox ID="txtBossTel3" runat="server" MaxLength="5" checktype="numx" Width="40px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="負責人電話三"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd"><%--20190730 長姓名需求-負責人部份--%>
                            <%--負責人長姓名--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_080" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="2">                                
                                <cc1:CustCheckBox ID="chkisLongName" runat="server"
                                    AutoPostBack="True" BoxName="長姓名" OnCheckedChanged="CheckBox_CheckedChanged" />
                                <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtboss_1_L" runat="server" MaxLength="50" checktype="fulltype" Width="260px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人長姓名"
                                    onpaste="paste();" AutoPostBack="true" OnTextChanged="TextBox_TextChanged"></cc1:CustTextBox>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False"></cc1:CustLabel>
                                </td>
                            <td style="width: 22%" colspan="4">
                                <cc1:CustTextBox ID="txtboss_1_Pinyin" runat="server" MaxLength="50" checktype="fulltype" Width="260px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人羅馬拼音"
                                    onpaste="paste();"></cc1:CustTextBox>
                                </td>
                        </tr><%--20190730 長姓名需求-負責人部份--%>
                        <tr class="trOdd">
                            <%--國籍--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblCountryCode" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_071" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="7">
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtCountryCode" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);" 
                                        onkeydown="entersubmit('btnAdd');" BoxName="國籍" 
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
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_072" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtPassportNo" runat="server" MaxLength="22" Width="200px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="護照號碼"></cc1:CustTextBox>
                            </td>
                            <%--護照效期--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblPassportExpdt" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_077" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="5">
                                <cc1:CustTextBox ID="txtPassportExpdt" runat="server" MaxLength="8" Width="200px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="護照效期"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--居留證號--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblResidentNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_073" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 22%">
                                <cc1:CustTextBox ID="txtResidentNo" runat="server" MaxLength="22" Width="200px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="居留證號"></cc1:CustTextBox>
                            </td>
                            <%--居留效期--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblResidentExpdt" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_078" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="5">
                                <cc1:CustTextBox ID="txtResidentExpdt" runat="server" MaxLength="8" Width="200px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="居留效期"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--負責人 領換補日--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBossChangeDate" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_032" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="lblBossChangeDate1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_061"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtBossChangeDate" runat="server" MaxLength="7" checktype="num"
                                    Width="60px" onkeydown="entersubmit('btnAdd');" BoxName="負責人領換補日"></cc1:CustTextBox>
                            </td>
                            <%--代號--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBossFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_033" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtBossFlag" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="20px" onkeydown="entersubmit('btnAdd');" BoxName="代號"></cc1:CustTextBox>
                            </td>
                            <%--生日--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBossBirthday" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_034"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtBossBirthday" runat="server" MaxLength="7" checktype="num"
                                    Width="60px" onkeydown="entersubmit('btnAdd');" BoxName="生日"></cc1:CustTextBox>
                            </td>
                            <%--換證點--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBossAt" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_040" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtBossAt" runat="server" MaxLength="3" checktype="fulltype"
                                    Width="40px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="換證點" InputType="ChineseLanguage" onpaste="paste();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--戶籍地址--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblRegAddress" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_015" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 88%" colspan="7">
                                <cc1:CustTextBox ID="txtRegAddr1" runat="server"  onblur="changeFullType(this);" MaxLength="6"
                                    checktype="fulltype" Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="戶籍地址一"
                                    onpaste="paste();"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtRegAddr2" runat="server"  onblur="changeFullType(this);" MaxLength="14"
                                    checktype="fulltype" Width="220px" onkeydown="entersubmit('btnAdd');" BoxName="戶籍地址二"
                                    onpaste="paste();"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtRegAddr3" runat="server"  onblur="changeFullType(this);" MaxLength="7"
                                    checktype="fulltype" Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="戶籍地址三"
                                    onpaste="paste();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--實際經營者相關資料--%>
                        <tr class="trEven" style="display:none">
                            <td align="right" style="width: 12%" rowspan="6">
                                <cc1:CustLabel ID="lblOperData" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040101_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="lblOperData1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_065" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--同負責人相關資料--%>
                            <td style="width: 88%" colspan="8">
                                <cc1:CustCheckBox ID="chkOper" runat="server" OnCheckedChanged="chkOper_CheckedChanged"
                                    AutoPostBack="True" BoxName="同負責人相關資料(CheckBox)" />
                                <cc1:CustLabel ID="lblSameBoss" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_058" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven" style="display:none">
                            <%--實際經營者 姓名--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperman1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040101_016" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="lblOperman2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_062" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOpermanText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="60px" BoxName="實際經營者姓名"></cc1:CustLabel>
                            </td>
                            <%--實際經營者 ID--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperID1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040101_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="lblOperID2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_063" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOperIDText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="100px" BoxName="實際經營者ID"></cc1:CustLabel>
                            </td>
                            <%--實際經營者 電話--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperTel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040101_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_064" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="3" align="left">
                                <cc1:CustLabel ID="lblOperTelText1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="30px" BoxName="實際經營者電話一"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblOperTelText2" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="70px" BoxName="實際經營者電話二"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblOperTelText3" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="40px" BoxName="實際經營者電話三"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven" style="display:none">
                            <%--實際經營者領換補日--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperChangeDate1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_061" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOperChangeDateText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="50px" BoxName="實際經營者領換補日"></cc1:CustLabel>
                            </td>
                            <%--代號--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperFlag1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_033" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOperFlagText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="20px" BoxName="代號"></cc1:CustLabel>
                            </td>
                            <%--生日--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperBirthday1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_034"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOperBirthdayText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="50px" BoxName="生日"></cc1:CustLabel>
                            </td>
                            <%--換證點--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperAt" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_040" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOperAtText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="40px" BoxName="換證點"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven" style="display:none">
                            <%--另列如下--%>
                            <td align="left" colspan="8">
                                <cc1:CustLabel ID="lblDown" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_060" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven" width="88%" style="display:none">
                            <%--實際經營者 姓名--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperman" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040101_016" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_062" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtOperman" runat="server" checktype="fulltype" MaxLength="4"
                                    Width="60px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="實際經營者姓名" onpaste="paste();"></cc1:CustTextBox>
                            </td>
                            <%--實際經營者 ID--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040101_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="lblOperID3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_063" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtOperID" runat="server" MaxLength="10" checktype="ID" Width="100px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="實際經營者ID" InputType="LetterAndInt"></cc1:CustTextBox>
                            </td>
                            <%--實際經營者 電話--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperTel" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040101_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="lblOperTel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_064" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3">
                                <cc1:CustTextBox ID="txtOperTel1" runat="server" MaxLength="3" checktype="num" Width="30px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="實際經營者電話一"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtOperTel2" runat="server" MaxLength="8" checktype="num" Width="70px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="實際經營者電話二"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtOperTel3" runat="server" MaxLength="5" checktype="numx" Width="40px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="實際經營者電話三"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven" style="display:none">
                            <%--實際經營者 領換補日--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperChangeDate" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;<br>
                                <cc1:CustLabel ID="lblOperChangeDate3" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_061"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtOperChangeDate" runat="server" MaxLength="7" checktype="num"
                                    Width="50px" onkeydown="entersubmit('btnAdd');" BoxName="實際經營者領換補日"></cc1:CustTextBox>
                            </td>
                            <%--代號--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_033" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtOperFlag" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="20px" onkeydown="entersubmit('btnAdd');" BoxName="代號"></cc1:CustTextBox>
                            </td>
                            <%--生日--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperBirthday" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_034"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtOperBirthday" runat="server" MaxLength="7" checktype="num"
                                    Width="50px" onkeydown="entersubmit('btnAdd');" BoxName="生日"></cc1:CustTextBox>
                            </td>
                            <%--換證點--%>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperChangeAdd" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_040"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtOperAt" runat="server" MaxLength="3" checktype="fulltype"
                                    Width="40px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="換證點" onpaste="paste();" InputType="ChineseLanguage"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--聯絡人--%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%" rowspan="2">
                                <cc1:CustLabel ID="lblContactMan" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_017" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--聯絡人姓名--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblContactManName" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_041"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtContactMan" runat="server" MaxLength="4" checktype="fulltype"
                                    Width="90px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="聯絡人姓名" onpaste="paste();"></cc1:CustTextBox>
                            </td>
                            <%--聯絡人電話--%>
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
                                <cc1:CustTextBox ID="txtContactManTel3" runat="server" MaxLength="5" checktype="numx"
                                    Width="36px" onkeydown="entersubmit('btnAdd');" BoxName="聯絡人電話三"></cc1:CustTextBox>
                            </td>
                            <%--聯絡人傳真--%>
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
                        <tr class="trOdd"><%--20190730 長姓名需求-聯絡人部份--%>
                            <%--聯絡人長姓名--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_080" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="2">                                
                                <cc1:CustCheckBox ID="chkisLongName_c" runat="server"
                                    AutoPostBack="True" BoxName="長姓名" OnCheckedChanged="CheckBox_CheckedChanged" />
                                <cc1:CustLabel ID="CustLabel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_083" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtcontact_man_L" runat="server" MaxLength="50" checktype="fulltype" Width="260px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="聯絡人長姓名"
                                    onpaste="paste();" AutoPostBack="true" OnTextChanged="TextBox_TextChanged"></cc1:CustTextBox>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="CustLabel9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_084" StickHeight="False"></cc1:CustLabel>
                                </td>
                            <td style="width: 22%" colspan="4">
                                <cc1:CustTextBox ID="txtcontact_man_Pinyin" runat="server" MaxLength="50" checktype="fulltype" Width="260px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="聯絡人羅馬拼音"
                                    onpaste="paste();"></cc1:CustTextBox>
                                </td>
                        </tr><%--20190730 長姓名需求-聯絡人部份--%>
                        <%--E-MAIL--%>
                        <tr class="trEven">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="lblEmail" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_074" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="8">
                                <cc1:CustTextBox ID="txtEmailFront" runat="server" Width="200px" 
                                    onkeydown="entersubmit('btnAdd');" BoxName="E-MAIL"></cc1:CustTextBox>
                                    @
                                <cc1:CustRadioButton ID="radGmail" runat="server" AutoPostBack="False" GroupName="email" Text="gmail.com" Checked="true" />
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
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_042" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--登記地址--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBookAddress" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_019" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 88%" colspan="7">
                                <cc1:CustTextBox ID="txtREG_ZIP_CODE" runat="server" checktype="fulltype" MaxLength="7"
                                    Width="80px" onkeydown="entersubmit('btnAdd');"
                                    BoxName="登記地址郵遞區號"  Enabled="false" BackColor="LightGray"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtBookAddr1" runat="server" checktype="fulltype" MaxLength="6"
                                    Width="100px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="登記地址一" onpaste="paste();" AutoPostBack="true" OnTextChanged="TextBox_AddrChanged"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtBookAddr2" runat="server" checktype="fulltype" MaxLength="14"
                                    Width="220px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="登記地址二" onpaste="paste();"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtBookAddr3" runat="server" checktype="fulltype" MaxLength="7"
                                    Width="100px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="登記地址三" onpaste="paste();"></cc1:CustTextBox>                               
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--營業地址--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="CustLabel11" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_020"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 11%" colspan="7">
                                <cc1:CustLabel ID="lblBusinessAddress" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--同登記地址--%>
                            <td colspan="8">
                                <cc1:CustCheckBox ID="chkAddress" runat="server" OnCheckedChanged="chkAddress_CheckedChanged"
                                    BoxName="同登記地址(CheckBox)" AutoPostBack="True" />
                                <cc1:CustLabel ID="lblAdd" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_055" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBusinessZipText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="" Width="50px" BoxName="營業地郵遞區號" Visible="false"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBusinessAddrText1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="" Width="100px"
                                    BoxName="登記地址一" Visible="false"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBusinessAddrText2" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="" Width="220px"
                                    BoxName="登記地址二" Visible="false"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBusinessAddrText3" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="" Width="100px"
                                    BoxName="登記地址三" Visible="false"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--營業地址--%>
                        <tr class="trOdd">
                            <td colspan="8">
                                <cc1:CustLabel ID="lblRight2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_059" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblZipText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="50px"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtBusinessAddr4" runat="server" MaxLength="6" checktype="fulltype"
                                    Width="100px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="營業地址一" onpaste="paste();"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtBusinessAddr5" runat="server" MaxLength="14" checktype="fulltype"
                                    Width="220px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="營業地址二" onpaste="paste();"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtBusinessAddr6" runat="server" MaxLength="7" checktype="fulltype"
                                    Width="100px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="營業地址三" onpaste="paste();"></cc1:CustTextBox>
                                <cc1:CustButton ID="btnSearchZip" runat="server" CssClass="smallButton" ShowID="01_01040301_088"
                                    DisabledWhenSubmit="False" OnClick="btnSearchZip_Click" BoxName="查詢郵區" />
                            </td>
                        </tr>
                        <%--JCIC--%>
                        <tr class="trEven">
                            <%--JCIC查詢--%>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="lblJCIC" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_043" StickHeight="False"
                                    ForeColor="Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 16%" colspan="2">
                                <cc1:CustTextBox ID="txtJCIC" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="JCIC查詢"></cc1:CustTextBox>
                                <cc1:CustLabel ID="lblMessage1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040101_031" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--Y_特店跨行匯費(6116)--%>
                            <td align="right" style="width: 18%" colspan="1">
                                <cc1:CustLabel ID="lblGrantFeeFlag" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_067"
                                    StickHeight="False" ForeColor="Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 16%" colspan="1">
                                <cc1:CustTextBox ID="txtGrantFeeFlag" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="Y_特店跨行匯費(6116)"></cc1:CustTextBox>
                            </td>
                            <%--Y_MPOS特店系統服務費免收註記(6086)F001--%>
                            <td align="right" style="width: 20%" colspan="3">
                                <cc1:CustLabel ID="lblMposFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_068" StickHeight="False"
                                    ForeColor="Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 18%" colspan="1">
                                <cc1:CustTextBox ID="txtMposFlag" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="Y_MPOS特店系統服務費免收註記(6086)F001"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--帳戶資料--%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%" rowspan="2">
                                <cc1:CustLabel ID="lblAccounts" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_044" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--銀行(中文)--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBank" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_021" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 33%" colspan="3">
                                <cc1:CustTextBox ID="txtBank" runat="server" MaxLength="5" checktype="fulltype" Width="80px"
                                    onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="銀行(中文)"
                                    onpaste="paste();" InputType="ChineseLanguage"></cc1:CustTextBox>
                            </td>
                            <%--分行(中文)--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBranchBank" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_022" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 33%" colspan="3">
                                <cc1:CustTextBox ID="txtBranchBank" runat="server" checktype="fulltype" MaxLength="10"
                                    Width="140px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="分行(中文)" onpaste="paste();" InputType="ChineseLanguage"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--戶名--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_023" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 88%" colspan="7">
                                <cc1:CustTextBox ID="txtName" runat="server" MaxLength="20" checktype="fulltype"
                                    Width="300px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="戶名" onpaste="paste();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--帳單資料--%>
                        <tr class="trEven">
                            <td align="right" style="width: 12%; height: 33px;">
                                <cc1:CustLabel ID="lblPrev" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_045" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--帳單內容--%>
                            <td align="right" style="width: 11%; height: 33px;">
                                <cc1:CustLabel ID="lblPrevDesc" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_046" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 33%; height: 33px;" colspan="3">
                                <cc1:CustTextBox ID="txtPrevDesc" runat="server" MaxLength="4" checktype="fulltype"
                                    Width="100px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="帳單內容" onpaste="paste();"></cc1:CustTextBox>
                            </td>
                            <%--發票週期--%>
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
                            <%--紅利週期(M/D)--%>
                            <td align="right" style="width: 11%; height: 33px;">
                                <cc1:CustLabel ID="lblRedeemCycle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_047" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%; height: 33px;">
                                <cc1:CustTextBox ID="txtRedeemCycle" runat="server" MaxLength="1" checktype="numandletter"
                                    Width="30px" onkeydown="entersubmit('btnAdd');" BoxName="紅利週期(M/D)"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--推廣員--%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="lblPopMan" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_024" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 88%" colspan="8">
                                <cc1:CustTextBox ID="txtPopMan" runat="server" MaxLength="3" checktype="fulltype"
                                    Width="50px"  onblur="changeFullType(this);" onkeydown="entersubmit('btnAdd');"
                                    BoxName="推廣員" onpaste="paste();"></cc1:CustTextBox>
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
                            <td style="width: 16%" colspan="2">
                                <cc1:CustTextBox ID="txtLAST_UPD_MAKER" runat="server" MaxLength="9" checktype="numandletter"
                                    Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動MAKER"></cc1:CustTextBox>
                            </td>
                            <%--資料最後異動CHECKER--%>
                            <td align="right">
                                <cc1:CustLabel ID="lbLAST_UPD_CHECKER" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_087" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 18%" colspan="2">
                                <cc1:CustTextBox ID="txtLAST_UPD_CHECKER" runat="server" MaxLength="9" checktype="numandletter"
                                    Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動CHECKER"></cc1:CustTextBox>
                            </td>
                            <%--資料最後異動分行--%>
                            <td align="right">
                                <cc1:CustLabel ID="lbLAST_UPD_BRANCH" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_085" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 16%" colspan="2">
                                <cc1:CustTextBox ID="txtLAST_UPD_BRANCH" runat="server" MaxLength="4" checktype="numandletter"
                                    Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動分行"></cc1:CustTextBox>                                
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
                                    OnClick="btnAdd_Click" OnClientClick="return checkInputText('pnlText', 1);" DisabledWhenSubmit="False"
                                    onkeydown="movefocus();" />
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
        
        <cc1:CustButton ID="btn007008Hiden" OnClick="btn007008Hiden_Click" runat="server" CssClass="btnHiden"
            DisabledWhenSubmit="False"></cc1:CustButton>
        
        <cc1:CustButton ID="btnHiden" OnClick="btnHiden_Click" runat="server" CssClass="btnHiden"
            DisabledWhenSubmit="False"></cc1:CustButton>
        <cc1:CustButton ID="btnAddHiden" runat="server" CssClass="btnHiden" DisabledWhenSubmit="False"
            OnClick="btnAddHiden_Click" OnClientClick="return checkAdd();"></cc1:CustButton>
            
        <cc1:CustButton ID="btnAddHidenDataConfirm" runat="server" CssClass="btnHiden" DisabledWhenSubmit="False"
            OnClick="btnAddHiden_Click"></cc1:CustButton>    
            
        <cc1:CustButton ID="btnAddUpdateHiden" runat="server" CssClass="btnHiden" DisabledWhenSubmit="False"
            OnClick="btnAddUpdateHiden_Click"></cc1:CustButton>
    </form>
</body>
</html>
