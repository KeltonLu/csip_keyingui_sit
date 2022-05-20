<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010103020001.aspx.cs" Inherits="P010103020001" %>

<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <%-- 20201123_Ares_Stanley-修正格式; 20210329_Ares_Stanley-調整半形轉全形失效; 20210331_Ares_Stanley-調整Disabled input背景色; 20210408_Ares_Stanley-調整半形轉全形失效; 20210415_Ares_Stanley-調整全半形轉換; 20210902_Ares_Stanley:移除Email前30後19長度限制, 改為總長度50 --%>
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript">

        //*特店資料異動一KEY提交事件檢核
        function checktextbox(str, index) {
            if (!checkShopId()) {
                return false;
            }

            //*查詢事件檢核
            if (index != 0) {
                //*檢核查詢部分欄位輸入規則
                if (!checkNewInputType('tabMostly')) {
                    return false;
                }
            }

            //*中文、英文、數字檢核
            if (!checkNewInputType(str)) {
                return false;
            }

            //*檢核費率查詢部分商店代號的第三碼為5
            if (document.getElementById('pnlFee') != null) {
                //*檢核若費率異動 (作業畫面: P4A)部分商店代號的第三碼為5
                if (document.getElementById('txtShopId').value.Trim().charAt(2) == "5") {
                    alert('分期商店代號有誤無法異動(第三碼為5)');
                    document.getElementById('txtShopId').focus();
                    return false;
                }
            }

            //*檢核機器查詢部分商店代號的第三碼為5
            if (document.getElementById('pnlMachineData') != null) {
                //*檢核若費率異動 (作業畫面: P4A)部分商店代號的第三碼為5
                if (document.getElementById('txtShopId').value.Trim().charAt(2) == "5") {
                    alert('分期商店代號有誤無法異動(第三碼為5)');
                    document.getElementById('txtShopId').focus();
                    return false;
                }
            }

            //*基本資料部分提交檢核
            if (index == 1) {
                if (!checkbox()) {
                    return false;
                }

                if (!checkBasic()) {
                    return false;
                }
            }

            //*費率部分提交檢核
            if (index == 2) {
                if (!checkP4A()) {
                    return false;
                }

                if (!checkFee()) {
                    return false;
                }
            }

            //*機器資料部分提交檢核
            if (index == 3) {
                if (!checkP4A()) {
                    return false;
                }

                if (!checkMachine()) {
                    return false;
                }
            }

            //*帳號、解約部分提交檢核
            if (index == 4 || index == 5) {
                if (!checkbox()) {
                    return false;
                }

                //*帳號部分
                if (index == 4) {
                    if (!checkBank()) {
                        return false;
                    }
                }
            }

            if (index != 0) {
                if (!confirm('確定是否要異動資料？')) {
                    return false;
                }
            }

            return true;
        }

        //*檢核基本資料異動部分
        function checkBasic() {
            //*檢核商店代號不可為空白
            if (!checkShopId()) {
                return false;
            }

            if ((document.getElementById('txtUndertakerTel1').value.Trim() != "" || document.getElementById('txtUndertakerTel2').value.Trim() != "") && document.getElementById('txtUndertakerTel3').value.Trim() == "") {
                alert('負責人電話分機空欄請補X');
                document.getElementById('txtUndertakerTel3').focus();
                return false;
            }

            if (document.getElementById('txtUndertakerTel1').value.Trim() != "" && (document.getElementById('txtUndertakerTel2').value.Trim() == "" || document.getElementById('txtUndertakerTel3').value.Trim() == "")) {
                alert('負責人電話欄位必須同時輸入');
                document.getElementById('txtJunctionPersonTel1').focus();
                return false;
            }

            if (document.getElementById('txtUndertakerTel2').value.Trim() != "" && (document.getElementById('txtUndertakerTel1').value.Trim() == "" || document.getElementById('txtUndertakerTel3').value.Trim() == "")) {
                alert('負責人電話欄位必須同時輸入');
                document.getElementById('txtUndertakerTel1').focus();
                return false;
            }

            if (document.getElementById('txtUndertakerTel3').value.Trim() != "" && (document.getElementById('txtUndertakerTel2').value.Trim() == "" || document.getElementById('txtUndertakerTel1').value.Trim() == "")) {
                alert('負責人電話欄位必須同時輸入');
                document.getElementById('txtJunctionPersonTel1').focus();
                return false;
            }

            if ((document.getElementById('txtRealPersonTel1').value.Trim() != "" || document.getElementById('txtRealPersonTel2').value.Trim() != "") && document.getElementById('txtRealPersonTel3').value.Trim() == "") {
                alert('實際經營人電話分機空欄請補X');
                document.getElementById('txtJunctionPersonTel3').focus();
                return false;
            }

            if (document.getElementById('txtRealPersonTel1').value.Trim() != "" && (document.getElementById('txtRealPersonTel2').value.Trim() == "" || document.getElementById('txtRealPersonTel3').value.Trim() == "")) {
                alert('實際經營人電話欄位必須同時輸入');
                document.getElementById('txtJunctionPersonTel1').focus();
                return false;
            }

            if (document.getElementById('txtRealPersonTel2').value.Trim() != "" && (document.getElementById('txtRealPersonTel1').value.Trim() == "" || document.getElementById('txtRealPersonTel3').value.Trim() == "")) {
                alert('實際經營人電話欄位必須同時輸入');
                document.getElementById('txtJunctionPersonTel1').focus();
                return false;
            }

            if (document.getElementById('txtRealPersonTel3').value.Trim() != "" && (document.getElementById('txtRealPersonTel2').value.Trim() == "" || document.getElementById('txtRealPersonTel1').value.Trim() == "")) {
                alert('實際經營人電話欄位必須同時輸入');
                document.getElementById('txtJunctionPersonTel1').focus();
                return false;
            }

            if ((document.getElementById('txtJunctionPersonTel1').value.Trim() != "" || document.getElementById('txtJunctionPersonTel2').value.Trim() != "") && document.getElementById('txtJunctionPersonTel3').value.Trim() == "") {
                alert('聯絡人電話分機空欄請補X');
                document.getElementById('txtJunctionPersonTel3').focus();
                return false;
            }

            if (document.getElementById('txtJunctionPersonTel1').value.Trim() != "" && (document.getElementById('txtJunctionPersonTel2').value.Trim() == "" || document.getElementById('txtJunctionPersonTel3').value.Trim() == "")) {
                alert('聯絡人電話欄位必須同時輸入');
                document.getElementById('txtJunctionPersonTel1').focus();
                return false;
            }

            if (document.getElementById('txtJunctionPersonTel2').value.Trim() != "" && (document.getElementById('txtJunctionPersonTel1').value.Trim() == "" || document.getElementById('txtJunctionPersonTel3').value.Trim() == "")) {
                alert('聯絡人電話欄位必須同時輸入');
                document.getElementById('txtJunctionPersonTel1').focus();
                return false;
            }

            if (document.getElementById('txtJunctionPersonTel3').value.Trim() != "" && (document.getElementById('txtJunctionPersonTel2').value.Trim() == "" || document.getElementById('txtJunctionPersonTel1').value.Trim() == "")) {
                alert('聯絡人電話欄位必須同時輸入');
                document.getElementById('txtJunctionPersonTel1').focus();
                return false;
            }

            if (document.getElementById('txtJunctionPersonFax1').value.Trim() != "" && document.getElementById('txtJunctionPersonFax2').value.Trim() == "") {
                alert('聯絡人傳真欄位必須同時輸入');
                document.getElementById('txtJunctionPersonFax1').focus();
                return false;
            }

            if (document.getElementById('txtJunctionPersonFax1').value.Trim() == "" && document.getElementById('txtJunctionPersonFax2').value.Trim() != "") {
                alert('聯絡人傳真欄位必須同時輸入');
                document.getElementById('txtJunctionPersonFax1').focus();
                return false;
            }

            //*檢核HOLD STMT欄位只能輸入0與9
            if (document.getElementById('txtHOLDSTMT').value != "" && document.getElementById('txtHOLDSTMT').value != "0" && document.getElementById('txtHOLDSTMT').value != "9") {
                alert('HOLD STMT只能輸入0與9');
                document.getElementById('txtHOLDSTMT').focus();
                return false;
            }

            //*20130117 Yucheng 素娟同意取消加批欄位不可空白限制,改為可允許空白,但若填寫僅能Y與N
            //*if((document.getElementById('txtReqAppro').value.Trim()=="")||(document.getElementById('txtReqAppro').value.Trim()!=""&&document.getElementById('txtReqAppro').value.Trim()!="Y"&&document.getElementById('txtReqAppro').value.Trim()!="N"))
            if ((document.getElementById('txtReqAppro').value.Trim() != "" && document.getElementById('txtReqAppro').value.Trim() != "Y" && document.getElementById('txtReqAppro').value.Trim() != "N")) {
                alert("請款加批只能輸入Y與N");
                document.getElementById('txtReqAppro').focus();
                return false;
            }

            // 總公司統一編號
            var uniNoLen = document.getElementById('txtUniNo').value.toUpperCase().Trim();
            if (uniNoLen.length > 0) {
                if (uniNoLen.length != 8) {
                    alert('總公司統一編號請輸入8碼數字!');
                    document.getElementById('txtUniNo').focus();
                    return false;
                }
            }

            // AML行業編號
            var amlcc = document.getElementById('txtAMLCC').value.Trim();
            if (amlcc.length > 0) {
                if (amlcc.length != 7) {
                    alert('AML行業編號請輸入7碼數字!');
                    document.getElementById('txtAMLCC').focus();
                    return false;
                }
                else {
                    var allAMLCC = document.getElementById('hidAMLCC').value.Trim();
                    if (allAMLCC.indexOf(amlcc) == -1) {
                        alert('AML行業編號不存在!');
                        document.getElementById('txtAMLCC').focus();
                        return false;
                    }
                }
            }

            // 設立
            var establish = document.getElementById('txtEstablish').value.Trim();
            if (establish.length > 0) {
                if (establish.length != 7) {
                    alert('設立請輸入7碼數字!');
                    document.getElementById('txtEstablish').focus();
                    return false;
                }
            }

            // 法律形式
            var allOrganization = document.getElementById('hidOrganization').value.Trim();
            var organization = document.getElementById('txtOrganization').value.Trim();
            if (organization.length > 0) {
                if (organization.length != 2) {
                    alert('法律形式輸入2碼數字!');
                    document.getElementById('txtOrganization').focus();
                    return false;
                }

                if (allOrganization.indexOf(organization) == -1) {
                    alert('法律形式不存在!');
                    document.getElementById('txtOrganization').focus();
                    return false;
                }
            }

            // 國籍、護照號碼、居留證號
            var countryCode = document.getElementById('txtCountryCode').value.toUpperCase().Trim();
            var passportNo = document.getElementById('txtPassportNo').value.toUpperCase().Trim();
            var residentNo = document.getElementById('txtResidentNo').value.toUpperCase().Trim();
            var passportExpdt = document.getElementById('txtPassportExpdt').value.toUpperCase().Trim();
            var residentExpdt = document.getElementById('txtResidentExpdt').value.toUpperCase().Trim();
            var alertMag = '國籍、護照號碼、護照效期、統一證號、統一證號效期 為一組，必須同時為空或同時有值!';//20200410-RQ-2019-030155-005-居留證號更名為統一證號
            // 檢查是否全部有值
            if (countryCode != "" || passportNo != "" || residentNo != "" || passportExpdt != "" || residentExpdt != "") {
               if (countryCode == "" || passportNo == "" || residentNo == "" || passportExpdt == "" || residentExpdt == "") {
                 //if (countryCode == "" || passportNo == "" || residentNo == "") {
                    if (countryCode == "") {
                        document.getElementById('txtCountryCode').focus();
                    } else if (passportNo == "") {
                        document.getElementById('txtPassportNo').focus();
                    }
                        else if (passportExpdt == "") {
                            document.getElementById('txtPassportExpdt').focus();
                        }
                    else if (residentNo == "") {
                        document.getElementById('txtResidentNo').focus();
                    }
                    else if (residentExpdt == "") {
                        document.getElementById('txtResidentExpdt').focus();
                    }

                    alert(alertMag);
                    return false;
                }
            }

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

            document.getElementById('txtCountryCode').value = countryCode;

            // 國籍
            if (countryCode.length > 0) {
                if (countryCode.length > 1) {
                    var allCountryCode = document.getElementById('hidCountryCode').value.Trim();
                    if (allCountryCode.indexOf(countryCode) == -1) {
                        alert('國籍不存在!');
                        document.getElementById('txtCountryCode').focus();
                        return false;
                    }
                }
                else {
                    if (countryCode != "X") {
                        alert('國籍輸入錯誤!');
                        document.getElementById('txtCountryCode').focus();
                        return false;
                    }
                }
            }

            // E-Mail
            var emailF = document.getElementById('txtEmailFront').value.Trim();
            if (emailF.length > 0) {
                if (document.getElementById('radGmail').checked || document.getElementById('radYahoo').checked ||
                   document.getElementById('radHotmail').checked || document.getElementById('radOther').checked) {
                    var emailB = '';
                    if (document.getElementById('radGmail').checked) {
                        emailB = 'gmail.com';
                    }
                    else if (document.getElementById('radYahoo').checked) {
                        emailB = 'yahoo.com.tw';
                    }
                    else if (document.getElementById('radHotmail').checked) {
                        emailB = 'hotmail.com';
                    }
                    else {
                        emailB = document.getElementById('txtEmailOther').value.Trim();
                    }

                    var email = emailF + '@' + emailB;
                    if (email.length > 1) {
                        var emailRule = new RegExp(/^[+a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/i);
                        if (!emailRule.test(email)) {
                            alert('E-Mail錯誤!');
                            document.getElementById('txtEmailFront').focus();
                            return false;
                        }
                    }

                    // 完整E-MAIL填入HiddenField
                    var emailControl = document.getElementById('hidEmailFall');
                    emailControl.value = email;
                }
                else {
                    alert('請選擇E-Mail!');
                    return false;
                }
            }

            return true;
        }

        //*檢核若費率異動
        function checkFee() {
            //*檢核商店代號不可為空白
            if (!checkShopId()) {
                return false;
            }

            //*檢核若費率異動 (作業畫面: P4A)部分商店代號的第三碼為5
            if (document.getElementById('txtShopId').value.Trim().charAt(2) == "5") {
                alert('分期商店代號有誤無法異動(第三碼為5)');
                document.getElementById('txtShopId').focus();
                return false;
            }

            //*檢核 費率異動部分 【新增JCB(全)】
            if (document.getElementById('txtAddjcb').value != "") {
                if (document.getElementById('txtAddjcbUs').value != "" || document.getElementById('txtAddjcbNotus').value != "") {
                    alert('已輸入新增JCB(全)，新增JCB(本行)及新增JCB(他行)不必輸入');
                    document.getElementById('txtAddjcbUs').value = "";
                    document.getElementById('txtAddjcbNotus').value = "";
                    document.getElementById('txtAddjcb').focus();
                    return false;
                }
            }

            //*檢核 費率異動部分 【全卡(1-8)】
            if (document.getElementById('txtPage3').value != "") {
                if (document.getElementById('txtPage1').value != "" || document.getElementById('txtPage2').value != "" || document.getElementById('txtPage4').value != "") {
                    alert('已輸入全卡(1-8)因此01~08不必輸入');
                    document.getElementById('txtPage1').value = "";
                    document.getElementById('txtPage2').value = "";
                    document.getElementById('txtPage4').value = "";
                    document.getElementById('txtPage3').focus();
                    return false;
                }

                //*檢核費率異動02~08的欄位不為空
                if (!CheckFeeNotNull('txtRate02N')) {
                    return false;
                }
                if (!CheckFeeNotNull('txtRate03V')) {
                    return false;
                }
                if (!CheckFeeNotNull('txtRate04V')) {
                    return false;
                }
                if (!CheckFeeNotNull('txtRate05M')) {
                    return false;
                }
                if (!CheckFeeNotNull('txtRate06M')) {
                    return false;
                }
                if (!CheckFeeNotNull('txtRate07J')) {
                    return false;
                }
                if (!CheckFeeNotNull('txtRate08J')) {
                    return false;
                }
            }

            //*檢核 費率異動部分 【本行卡(1.3.5.7)】
            if (document.getElementById('txtPage1').value != "") {
                if (document.getElementById('txtPage4').value != "" || document.getElementById('txtRate03V').value != "" || document.getElementById('txtRate05M').value != "" || document.getElementById('txtRate07J').value != "") {
                    alert('已輸入本行卡(1.3.5.7)，因此01.03.05.07不必輸入');
                    document.getElementById('txtPage4').value = "";
                    document.getElementById('txtRate03V').value = "";
                    document.getElementById('txtRate05M').value = "";
                    document.getElementById('txtRate07J').value = "";
                    document.getElementById('txtPage1').focus();
                    return false;
                }
            }

            //*檢核 費率異動部分 【他行卡(2.4.6.8)】
            if (document.getElementById('txtPage2').value != "") {
                if (document.getElementById('txtRate02N').value != "" || document.getElementById('txtRate04V').value != "" || document.getElementById('txtRate06M').value != "" || document.getElementById('txtRate08J').value != "") {
                    alert('已輸入他行卡(2.4.6.8)，因此02.04.06.08不必輸入');
                    document.getElementById('txtRate02N').value = "";
                    document.getElementById('txtRate04V').value = "";
                    document.getElementById('txtRate06M').value = "";
                    document.getElementById('txtRate08J').value = "";
                    document.getElementById('txtPage2').focus();
                    return false;
                }
            }

            //*銀聯卡費率STATUS檢核
            if (!checkFeeStatus()) {
                return false;
            }

            return true;
        }

        //*銀聯卡費率STATUS檢核
        function checkFeeStatus() {
            var obj = new Array("txtStatus1", "txtStatus2", "txtStatus3", "txtStatus4", "txtStatus5", "txtStatus6", "txtStatus7", "txtStatus8", "txtStatus9", "txtStatus10");

            for (var i = 0; i < obj.length; i++) {
                if (document.getElementById(obj[i]).value != "" && document.getElementById(obj[i]).value != "1" && document.getElementById(obj[i]).value != "8") {
                    alert('費率STATUS只能輸入"1"或"8"');
                    document.getElementById(obj[i]).focus();
                    return false;
                }
            }

            return true;
        }

        //*檢核商店代號不可為空白
        function checkShopId() {
            if (document.getElementById('txtShopId').value.Trim() == "") {
                alert('商店代碼不可為空白');
                setControlsDisabled('pnlText');
                document.getElementById('lblRecordNameText').innerText = "";
                document.getElementById('lblMerchantNameText').innerText = "";
                document.getElementById('lblBusinessNameText').innerText = "";
                document.getElementById('chboxP4A').checked = false;

                if (document.getElementById('chboxP4') != null) {
                    document.getElementById('chboxP4').checked = false;
                }

                document.getElementById('txtShopId').focus();
                return false;
            }

            return true;
        }

        //*檢核作業畫面
        function checkbox() {
            if (document.getElementById('chboxP4A').checked == false && document.getElementById('chboxP4').checked == false) {
                alert('作業畫面請至少點選一項......!');
                return false;
            }

            return true;
        }

        //*檢核P4A作業畫面
        function checkP4A() {
            if (document.getElementById('chboxP4A').checked == false) {
                alert('作業畫面請點選一項......!');
                return false;
            }

            return true;
        }

        //*根據商店代號3碼選擇checkbox
        function selectCheckBox() {
            document.getElementById('chboxP4A').checked = false;
            document.getElementById('chboxP4').checked = false;

            if (document.getElementById('txtShopId').value.charAt(2) == "5") {
                document.getElementById('chboxP4A').checked = true;
                document.getElementById('chboxP4').checked = true;
            }
            else {
                document.getElementById('chboxP4').checked = true;
            }
        }

        //*檢核機器資料部分
        function checkMachine() {
            if (!checkShopId()) {
                return false;
            }

            //*檢核 機器資料 部分 【EDC編號】機型
            if (document.getElementById('txtEdcType').value == "0" || document.getElementById('txtEdcType').value == "1") {
                alert('此欄位不可鍵入0 或 1');
                document.getElementById('txtEdcType').focus();
                return false;
            }

            return true;
        }

        //*檢核費率異動02~08的某個欄位不為空
        function CheckFeeNotNull(id) {
            if (document.getElementById(id).value != "") {
                alert('已輸入全卡(1-8)因此01~08不必輸入');
                document.getElementById(id).value = "";
                document.getElementById('txtPage3').focus();
                return false;
            }
            else {
                return true;
            }
        }

        //*選擇按鈕上按Tab鍵設置焦點,異動區域欄位可用，設置異動區域第一個為焦點，否則設置商店代號欄位為焦點
        function setfocuschoice() {
            if (event.keyCode == 9) {
                event.returnValue = false;

                if (document.getElementById('txtRecordNameTwo') != null) {
                    //*基本資料異動部分，登記名稱欄位可用，設置為焦點
                    if (document.getElementById('txtRecordNameTwo').disabled == false) {
                        document.getElementById('txtRecordNameTwo').focus();
                        return;
                    }
                }

                if (document.getElementById('txtAddjcb') != null) {
                    //*費率異動部分，新增JCB（全）欄位可用，設置為焦點
                    if (document.getElementById('txtAddjcb').disabled == false) {
                        document.getElementById('txtAddjcb').focus();
                        return;
                    }
                }

                if (document.getElementById('txtBankName') != null) {
                    //*帳號異動部分，銀行名稱欄位可用，設置為焦點
                    if (document.getElementById('txtBankName').disabled == false) {
                        document.getElementById('txtBankName').focus();
                        return;
                    }
                }

                if (document.getElementById('txtCancelCode1') != null) {
                    //*解約作業部分，解約代號欄位可用，設置為焦點
                    if (document.getElementById('txtCancelCode1').disabled == false) {
                        document.getElementById('txtCancelCode1').focus();
                        return;
                    }
                }

                if (document.getElementById('txtImp1Type') != null) {
                    //*機器資料部分，IMP1欄位可用，設置為焦點
                    if (document.getElementById('txtImp1Type').disabled == false) {
                        document.getElementById('txtImp1Type').focus();
                        return;
                    }
                }

                //*異動區域都不可用，設置商店代號欄位為焦點
                document.getElementById('txtShopId').focus();
            }
        }

        function ChangeEnable() {
            if (document.getElementById("txtShopId").value.toUpperCase().Trim() != document.getElementById("txtShopIdH").value.toUpperCase().Trim()) {
                setControlsDisabled('pnlText');
                document.getElementById('lblRecordNameText').innerText = "";
                document.getElementById('lblMerchantNameText').innerText = "";
                document.getElementById('lblBusinessNameText').innerText = "";
                document.getElementById('chboxP4A').checked = false;

                if (document.getElementById('chboxP4') != null) {
                    document.getElementById('chboxP4').checked = false;
                }
            }

            document.getElementById("txtShopIdH").value = document.getElementById("txtShopId").value;
        }

        //*輸入新增JCB(全)時 , 新增JCB(本行) 及新增JCB(他行)欄位 disabled;
        //*若將新增JCB(全)欄位清空時 , 新增JCB(本行) 及新增JCB(他行)欄位 enabled
        function changeFeeStatus() {
            if (document.getElementById("txtAddjcb").value.length > 0) {
                document.getElementById('txtAddjcbUs').disabled = true;
                document.getElementById('txtAddjcbNotus').disabled = true;
            }
            else {
                document.getElementById('txtAddjcbUs').disabled = false;
                document.getElementById('txtAddjcbNotus').disabled = false;
            }
        }

        //*若本行卡(1.3.5.7)欄位有填值時,下方01N(本行) ,03V(本行),05M(本行),07J(本行) 欄位 disabled,反之, enabled
        function changeFeeUsStatus() {
            if (document.getElementById('txtPage1').value.length > 0) {
                document.getElementById('txtPage4').disabled = true;
                document.getElementById('txtRate03V').disabled = true;
                document.getElementById('txtRate05M').disabled = true;
                document.getElementById('txtRate07J').disabled = true;
            }
            else {
                document.getElementById('txtPage4').disabled = false;
                document.getElementById('txtRate03V').disabled = false;
                document.getElementById('txtRate05M').disabled = false;
                document.getElementById('txtRate07J').disabled = false;
            }
        }

        //*若他行卡(2.4.6.8)欄位有填值時 , 下方02N(他行),04V(他行),06M(他行),08J(他行)欄位 disabled,反之 , enabled
        function changeFeeOtherStatus() {
            if (document.getElementById('txtPage2').value.length > 0) {
                document.getElementById('txtRate02N').disabled = true;
                document.getElementById('txtRate04V').disabled = true;
                document.getElementById('txtRate06M').disabled = true;
                document.getElementById('txtRate08J').disabled = true;
            }
            else {
                document.getElementById('txtRate02N').disabled = false;
                document.getElementById('txtRate04V').disabled = false;
                document.getElementById('txtRate06M').disabled = false;
                document.getElementById('txtRate08J').disabled = false;
            }
        }

        //*若全卡(1-8)欄位有填值時 , 本行卡(1.3.5.7) , 他行卡(2.4.6.8) , 以及 01N(本行)~ 08J(他行)欄位皆 disabled ,反之 , enabled
        function changeFeeAllStatus() {
            if (document.getElementById('txtPage3').value.length > 0) {
                document.getElementById('txtPage1').disabled = true;
                document.getElementById('txtPage2').disabled = true;

                document.getElementById('txtPage4').disabled = true;
                document.getElementById('txtRate03V').disabled = true;
                document.getElementById('txtRate05M').disabled = true;
                document.getElementById('txtRate07J').disabled = true;

                document.getElementById('txtRate02N').disabled = true;
                document.getElementById('txtRate04V').disabled = true;
                document.getElementById('txtRate06M').disabled = true;
                document.getElementById('txtRate08J').disabled = true;
            }
            else {
                document.getElementById('txtPage1').disabled = false;
                document.getElementById('txtPage2').disabled = false;

                document.getElementById('txtPage4').disabled = false;
                document.getElementById('txtRate03V').disabled = false;
                document.getElementById('txtRate05M').disabled = false;
                document.getElementById('txtRate07J').disabled = false;

                document.getElementById('txtRate02N').disabled = false;
                document.getElementById('txtRate04V').disabled = false;
                document.getElementById('txtRate06M').disabled = false;
                document.getElementById('txtRate08J').disabled = false;
            }
        }

        //*檢核銀行名稱、分行名稱
        function checkBank() {
            if (document.getElementById('txtBankName').value.Trim() == "" && document.getElementById('txtBranchName').value.Trim() != "") {
                alert('銀行名稱不可空白');
                document.getElementById('txtBankName').focus();
                return false;
            }

            if (document.getElementById('txtBankName').value.Trim() != "" && document.getElementById('txtBranchName').value.Trim() == "") {
                alert('分行名稱不可空白');
                document.getElementById('txtBranchName').focus();
                return false;
            }

            return true;
        }

        function upperCase() {
            var reqApproValue = document.getElementById('txtReqAppro').value.Trim();
            if (reqApproValue == "y" || reqApproValue == "n") {
                document.getElementById('txtReqAppro').value = reqApproValue.toUpperCase();
            }
        }

        //*檢核基本資料異動部分
        function SubmitCheck(str, index) {
            //*提交事件檢核
            if (index != 0) {
                //*檢核查詢部分欄位輸入規則
                if (!checkNewInputType('tabBasicByTaxno')) {
                    return false;
                }
            }

            //*中文、英文、數字檢核
            if (!checkNewInputType(str)) {
                return false;
            }
            if (!checkBasicByTaxno()) {
                return false;
            }
            if (!confirm('確定是否要異動資料？')) {
                return false;
            }
        }

        function checkBasicByTaxno() {
            //*檢核商店代號不可為空白
            if (document.getElementById('txtUNI_NO1').value.Trim() == "") {
                alert('統一編號不可為空白');
                setControlsDisabled('pnlText');
                document.getElementById('lblREG_NAME').innerText = "";
                document.getElementById('lblOWNER_CHINESE_NAME').innerText = "";

                document.getElementById('txtUNI_NO1').focus();

                return false;
            }

            if ((document.getElementById('txtCORP_TEL1').value.Trim() != "" || document.getElementById('txtCORP_TEL2').value.Trim() != "") && document.getElementById('txtCORP_TEL3').value.Trim() == "") {
                //20191015 Talas 依心武需求調整
                //alert('總公司電話分機空欄請補X');
                alert('登記電話分機空欄請補X');
                document.getElementById('txtCORP_TEL3').focus();
                return false;
            }

            if (document.getElementById('txtCORP_TEL1').value.Trim() != "" && (document.getElementById('txtCORP_TEL2').value.Trim() == "" || document.getElementById('txtCORP_TEL3').value.Trim() == "")) {
                //20191015 Talas 依心武需求調整
                // alert('總公司電話欄位必須同時輸入');
                alert('登記電話欄位必須同時輸入');
                document.getElementById('txtCORP_TEL2').focus();
                return false;
            }

            if (document.getElementById('txtCORP_TEL2').value.Trim() != "" && (document.getElementById('txtCORP_TEL1').value.Trim() == "" || document.getElementById('txtCORP_TEL3').value.Trim() == "")) {
                //20191015 Talas 依心武需求調整
                // alert('總公司電話欄位必須同時輸入');
                alert('登記電話欄位必須同時輸入');
                document.getElementById('txtCORP_TEL1').focus();
                return false;
            }

            if (document.getElementById('txtCORP_TEL3').value.Trim() != "" && (document.getElementById('txtCORP_TEL2').value.Trim() == "" || document.getElementById('txtCORP_TEL1').value.Trim() == "")) {
                //20191015 Talas 依心武需求調整
                // alert('總公司電話欄位必須同時輸入');
                alert('登記電話欄位必須同時輸入');
                document.getElementById('txtCORP_TEL1').focus();
                return false;
            }

            if ((document.getElementById('txtPrincipal_TEL1').value.Trim() != "" || document.getElementById('txtPrincipal_TEL2').value.Trim() != "") && document.getElementById('txtPrincipal_TEL3').value.Trim() == "") {
                alert('負責人聯絡電話分機空欄請補X');
                document.getElementById('txtPrincipal_TEL3').focus();
                return false;
            }

            if (document.getElementById('txtPrincipal_TEL1').value.Trim() != "" && (document.getElementById('txtPrincipal_TEL2').value.Trim() == "" || document.getElementById('txtPrincipal_TEL3').value.Trim() == "")) {
                alert('負責人聯絡電話欄位必須同時輸入');
                document.getElementById('txtPrincipal_TEL1').focus();
                return false;
            }

            if (document.getElementById('txtPrincipal_TEL2').value.Trim() != "" && (document.getElementById('txtPrincipal_TEL1').value.Trim() == "" || document.getElementById('txtPrincipal_TEL3').value.Trim() == "")) {
                alert('負責人聯絡電話欄位必須同時輸入');
                document.getElementById('txtPrincipal_TEL1').focus();
                return false;
            }

            if (document.getElementById('txtPrincipal_TEL3').value.Trim() != "" && (document.getElementById('txtPrincipal_TEL2').value.Trim() == "" || document.getElementById('txtPrincipal_TEL1').value.Trim() == "")) {
                alert('負責人聯絡電話欄位必須同時輸入');
                document.getElementById('txtPrincipal_TEL1').focus();
                return false;
            }
            //20191015 Talas 依心武需求調整 追加商店登記地址檢核\ 
            if ((document.getElementById('txtREG_CITY').value.Trim() != "" || document.getElementById('txtREG_ADDR1').value.Trim() != "") && document.getElementById('txtREG_ADDR2').value.Trim() == "") {

                alert('登記地址空欄請補X');
                document.getElementById('txtREG_ADDR2').focus();
                return false;
            }

            if (document.getElementById('txtREG_CITY').value.Trim() != "" && (document.getElementById('txtREG_ADDR1').value.Trim() == "" || document.getElementById('txtREG_ADDR2').value.Trim() == "")) {
                alert('登記地址欄位必須同時輸入');
                document.getElementById('txtREG_ADDR1').focus();
                return false;
            }

            if (document.getElementById('txtREG_ADDR1').value.Trim() != "" && (document.getElementById('txtREG_CITY').value.Trim() == "" || document.getElementById('txtREG_ADDR2').value.Trim() == "")) {
                alert('登記地址欄位必須同時輸入');
                document.getElementById('txtREG_CITY').focus();
                return false;
            }

            if (document.getElementById('txtREG_ADDR2').value.Trim() != "" && (document.getElementById('txtREG_ADDR1').value.Trim() == "" || document.getElementById('txtREG_CITY').value.Trim() == "")) {
                alert('登記地址欄位必須同時輸入');
                document.getElementById('txtREG_CITY').focus();
                return false;
            }
            //20191015 Talas 依心武需求調整 追加負責人戶籍地址地址檢核
            if ((document.getElementById('txtHouseholdCITY').value.Trim() != "" || document.getElementById('txtHouseholdADDR1').value.Trim() != "") && document.getElementById('txtHouseholdADDR2').value.Trim() == "") {

                alert('負責人戶籍地址空欄請補X');
                document.getElementById('txtHouseholdADDR2').focus();
                return false;
            }

            if (document.getElementById('txtHouseholdCITY').value.Trim() != "" && (document.getElementById('txtHouseholdADDR1').value.Trim() == "" || document.getElementById('txtHouseholdADDR2').value.Trim() == "")) {
                alert('負責人戶籍地址欄位必須同時輸入');
                document.getElementById('txtHouseholdADDR1').focus();
                return false;
            }

            if (document.getElementById('txtHouseholdADDR1').value.Trim() != "" && (document.getElementById('txtHouseholdCITY').value.Trim() == "" || document.getElementById('txtHouseholdADDR2').value.Trim() == "")) {
                alert('負責人戶籍地址欄位必須同時輸入');
                document.getElementById('txtHouseholdCITY').focus();
                return false;
            }

            if (document.getElementById('txtHouseholdADDR2').value.Trim() != "" && (document.getElementById('txtHouseholdADDR1').value.Trim() == "" || document.getElementById('txtHouseholdCITY').value.Trim() == "")) {
                alert('負責人戶籍地址欄位必須同時輸入');
                document.getElementById('txtHouseholdCITY').focus();
                return false;
            }

            //20191017州別
            if (document.getElementById('txtCORP_CountryCode').value.Trim() == "US"
                && document.getElementById('txtCORP_CountryStateCode').value.Trim() == "") {
                alert('註冊國籍為美國者，請勾選州別');
                document.getElementById('txtCORP_CountryStateCode').focus();
                return false;
            }

            // 總公司統一編號
            var uniNoLen = document.getElementById('txtCORP_NO').value.toUpperCase().Trim();
            if (uniNoLen.length > 0) {
                if (uniNoLen.length != 8) {
                    alert('總公司統一編號請輸入8碼數字!');
                    document.getElementById('txtUniNo').focus();
                    return false;
                }
            }

            // AML行業編號
            var amlcc = document.getElementById('txtCORP_MCC').value.Trim();
            if (amlcc.length > 0) {
                if (amlcc.length != 7) {
                    alert('AML行業編號請輸入7碼數字!');
                    document.getElementById('txtCORP_MCC').focus();
                    return false;
                }
            }

            // 設立
            var establish = document.getElementById('txtCORP_ESTABLISH').value.Trim();
            if (establish.length > 0) {
                if (establish.length != 7) {
                    alert('設立請輸入7碼數字!');
                    document.getElementById('txtCORP_ESTABLISH').focus();
                    return false;
                }
            }

            // 法律形式
            var organization = document.getElementById('txtCORP_Organization').value.Trim();
            if (organization.length > 0) {
                if (organization.length != 2) {
                    alert('法律形式輸入2碼數字!');
                    document.getElementById('txtCORP_Organization').focus();
                    return false;
                }
            }

            //if (document.getElementById('txtBANK_NAME').value.Trim() == "" && document.getElementById('txtBANK_BRANCH').value.Trim() != "") {
            //    alert('銀行名稱不可空白');
            //    document.getElementById('txtBANK_NAME').focus();
            //    return false;
            //}

            //if (document.getElementById('txtBANK_NAME').value.Trim() != "" && document.getElementById('txtBANK_BRANCH').value.Trim() == "") {
            //    alert('分行名稱不可空白');
            //    document.getElementById('txtBANK_BRANCH').focus();
            //    return false;
            //}

            // 國籍、護照號碼、居留證號
            var countryCode2 = document.getElementById('txtPrincipalCountryCode').value.toUpperCase().Trim();
            var passportNo2 = document.getElementById('txtPrincipalPassportNo').value.toUpperCase().Trim();
            var residentNo2 = document.getElementById('txtPrincipalResidentNo').value.toUpperCase().Trim();
            var passportExpdt2 = document.getElementById('txtPrincipalPassportExpdt').value.toUpperCase().Trim();
            var residentExpdt2 = document.getElementById('txtPrincipalResidentExpdt').value.toUpperCase().Trim();
            var alertMag2 = '國籍、護照號碼、護照效期、統一證號、統一證號效期 為一組，必須同時為空或同時有值!';//20200410-RQ-2019-030155-005-居留證號更名為統一證號
            // 檢查是否全部有值
            //if (countryCode != "" || passportNo != "" || residentNo != "") {
            //    if (countryCode == "" || passportNo == "" || residentNo == "") {
            if (countryCode2 != "" || passportNo2 != "" || residentNo2 != "" || passportExpdt2 != "" || residentExpdt2 != "") {
                if (countryCode2 == "" || passportNo2 == "" || residentNo2 == "" || passportExpdt2 == "" || residentExpdt2 == "") {
                    if (countryCode2 == "") {
                        document.getElementById('txtPrincipalCountryCode').focus();
                    } else if (passportNo2 == "") {
                        document.getElementById('txtPrincipalPassportNo').focus();
                    }
                    else if (passportExpdt2 == "") {
                        document.getElementById('txtPrincipalPassportExpdt').focus();
                    }
                    else if (residentNo2 == "") {
                        document.getElementById('txtPrincipalResidentNo').focus();
                    }
                    else if (residentExpdt2 == "") {
                        document.getElementById('txtPrincipalResidentExpdt').focus();
                    }

                    alert(alertMag2);
                    return false;
                }
            }

            if (passportExpdt2 != "" && passportExpdt2.length != 8 && passportExpdt2 != "X") {
                alert("護照效期請輸入8碼!");
                document.getElementById('txtPrincipalPassportExpdt').focus();
                return false;
            }

            if (residentExpdt2 != "" && residentExpdt2.length != 8 && residentExpdt2 != "X") {
                alert("統一證號效期請輸入8碼!");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
                document.getElementById('txtPrincipalResidentExpdt').focus();
                return false;
            }

            document.getElementById('txtPrincipalCountryCode').value = countryCode2;

            return true;
        }
    </script>

    <style type="text/css">
        .btnHiden {
            display: none;
        }
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
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" style="height: 50px"
                    id="TABLE2">
                    <tr class="itemTitle1">
                        <td>
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="181px" IsColon="False" ShowID="01_01030200_087"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="itemTitle1">
                        <td align="center">
                            <%--基本資料異動--%>
                            <cc1:CustButton ID="btnBasicData" Text="" runat="server" CssClass="smallButton" OnClick="btnBasicData_Click"
                                Width="100px" UseSubmitBehavior="False" DisabledWhenSubmit="False" ShowID="01_01030200_007" />
                            <%--費率異動作業畫面：P4A--%>
                            <cc1:CustButton ID="btnFee" runat="server" Text="" CssClass="smallButton" OnClick="btnFee_Click"
                                Width="150px" UseSubmitBehavior="False" DisabledWhenSubmit="False" ShowID="01_01030200_003" />
                            <%--帳號異動(作業畫面：P4/P4A)--%>
                            <cc1:CustButton ID="btnAccounts" Text="" runat="server" CssClass="smallButton" OnClick="btnAccounts_Click"
                                Width="170px" UseSubmitBehavior="False" DisabledWhenSubmit="False" ShowID="01_01030200_004" />
                            <%--解約作業(作業畫面：P4/P4A)--%>
                            <cc1:CustButton ID="btnUnchainTask" Text="" runat="server" CssClass="smallButton"
                                OnClick="btnUnchainTask_Click" Width="170px" UseSubmitBehavior="False" DisabledWhenSubmit="False"
                                ShowID="01_01030200_005" />
                            <%--機器資料(作業畫面：P4A)--%>
                            <cc1:CustButton ID="btnMachineData" Text="" runat="server" CssClass="smallButton"
                                OnClick="btnMachineData_Click" Width="150px" UseSubmitBehavior="False" DisabledWhenSubmit="False"
                                ShowID="01_01030200_006" />
                            <%--基本資料異動(By 統編)--%>
                            <cc1:CustButton ID="btnBasicDataByTaxno" Text="" runat="server" CssClass="smallButton"
                                OnClick="btnBasicDataByTaxno_Click" Width="150px" UseSubmitBehavior="False" DisabledWhenSubmit="False"
                                ShowID="01_01030100_104" />
                        </td>
                    </tr>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" style="height: 92px" id="tabMostly" runat="server">
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblShopId" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01030200_082" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtShopId" runat="server" Width="143px" MaxLength="9" onkeydown="entersubmit('btnSelect');"
                                checktype="numandletter" onfocus="allselect(this);" onkeyup="ChangeEnable();"
                                BoxName="商店代號"></cc1:CustTextBox><cc1:CustTextBox ID="txtShopIdH" runat="server"
                                    Width="143px" MaxLength="9" CssClass="btnHiden" Text=""></cc1:CustTextBox>&nbsp;&nbsp;&nbsp;&nbsp<cc1:CustButton
                                        ID="btnSelect" Text="" runat="server" Width="64px" CssClass="smallButton" OnClick="btnSelect_Click"
                                        OnClientClick=" return checktextbox('tabMostly',0);" DisabledWhenSubmit="False"
                                        onkeydown="setfocuschoice();" ShowID="01_01030200_001" /></td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblTask" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01030200_083" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 37%">
                            <cc1:CustCheckBox ID="chboxP4A" runat="server" Text="" />
                            &nbsp;&nbsp;<cc1:CustCheckBox ID="chboxP4" runat="server" Text="" /></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblRecordName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01030200_008" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <%--<cc1:CustTextBox ID="txtRecordName" runat="server" Width="240px" Enabled="False"></cc1:CustTextBox>--%>
                            <cc1:CustLabel ID="lblRecordNameText" runat="server" Width="240px" CurAlign="left"></cc1:CustLabel>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblMerchantName" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_084"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 37%">
                            <%--<cc1:CustTextBox ID="txtMerchantName" runat="server" Width="240px" Enabled="False"></cc1:CustTextBox>--%>
                            <cc1:CustLabel ID="lblMerchantNameText" runat="server" Width="240px" CurAlign="left"></cc1:CustLabel>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%;">
                            <cc1:CustLabel ID="lblBusinessName" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_085"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%;">
                            <%--<cc1:CustTextBox ID="txtBusinessName" runat="server" Width="240px" Enabled="False"></cc1:CustTextBox>--%>
                            <cc1:CustLabel ID="lblBusinessNameText" runat="server" Width="240px" CurAlign="left"></cc1:CustLabel>
                            <cc1:CustLabel ID="lblCORP_SEQ" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                StickHeight="False" Visible="false"></cc1:CustLabel>
                        </td>
                        <td colspan="2"></td>
                    </tr>
                    <tr>
                        <td nowrap colspan="4" style="height: 1px"></td>
                    </tr>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" style="height: 60px" id="tabBasicData" runat="server" visible="false">
                    <tr class="trOdd">
                        <%--分公司統編--%>
                        <td align="right" style="width: 15%; height: 27px;">
                            <cc1:CustLabel ID="CustLabel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01040301_002" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%; height: 27px;">
                            <cc1:CustTextBox ID="txtUNI_NO1" runat="server" MaxLength="8" checktype="num" Width="80px"
                                onkeydown="entersubmit('btnInQuery');" BoxName="統一編號一"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtUNI_NO2" runat="server" MaxLength="4" checktype="num" onkeydown="entersubmit('btnInQuery');"
                                Width="40px" BoxName="統一編號二"></cc1:CustTextBox>
                            &nbsp;&nbsp;&nbsp;&nbsp
                            <%--查詢--%>
                            <cc1:CustButton ID="btnInQuery" Text="" runat="server" Width="64px" CssClass="smallButton"
                                OnClick="btnInQuery_Click"
                                DisabledWhenSubmit="False" onkeydown="setfocuschoice();" ShowID="01_01030100_001" />
                        </td>
                        <%--作業畫面--%>
                        <td align="right" style="width: 15%; height: 27px;"></td>
                        <td style="width: 37%; height: 27px;"></td>
                    </tr>
                    <tr class="trEven">
                        <%--登記名稱--%>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel12" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01030100_008" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <cc1:CustLabel ID="lblREG_NAME" runat="server" Width="240px" CurAlign="left"></cc1:CustLabel>
                        </td>
                        <%--負責人姓名--%>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel14" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040301_012"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 37%">
                            <cc1:CustLabel ID="lblOWNER_CHINESE_NAME" runat="server" Width="240px" CurAlign="left"></cc1:CustLabel>
                        </td>
                    </tr>
                </table>
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <cc1:CustPanel ID="pnlBasic" runat="server" Height="363px" Width="100%">
                        <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabBasic" style="">
                            <tr class="itemTitle1">
                                <td colspan="4" align="left">
                                    <li>
                                        <cc1:CustLabel ID="lblBasic" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                            SetOmit="False" StickHeight="False" Width="232px" IsColon="False" ShowID="01_01030200_007"></cc1:CustLabel></li>
                            </tr>
                            <tr class="trOdd">
                                <%--總公司統一編號--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblUniNo" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_111"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtUniNo" runat="server" Width="140px" MaxLength="8"
                                        onkeydown="entersubmit('btnBasicSubmit');" checktype="num" BoxName="總公司統一編號"></cc1:CustTextBox>
                                </td>
                                <%--AML行業編號--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblAMLCC" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_110"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtAMLCC" runat="server" Width="80px" MaxLength="7"
                                        onkeydown="entersubmit('btnBasicSubmit');" BoxName="AML行業編號"></cc1:CustTextBox>
                                    <cc1:CustHiddenField ID="hidAMLCC" runat="server" />
                                </td>
                            </tr>
                            <tr class="trEven">
                                <%--設立--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblEstablish" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_098"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtEstablish" runat="server" Width="60px" MaxLength="7"
                                        onkeydown="entersubmit('btnBasicSubmit');" checktype="num" BoxName="設立"></cc1:CustTextBox>
                                </td>
                                <%--法律形式--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_099"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <div style="position: relative">
                                        <cc1:CustTextBox ID="txtOrganization" runat="server" MaxLength="2" Width="30px" onkeydown="entersubmit('btnBasicSubmit');" BoxName="法律形式"
                                            Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px;"></cc1:CustTextBox>
                                        <cc1:CustDropDownList ID="dropOrganization" kind="select" runat="server" onclick="simOptionClick4IE('txtOrganization');"
                                            Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                        </cc1:CustDropDownList>
                                        <cc1:CustHiddenField ID="hidOrganization" runat="server" />
                                    </div>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td style="width: 15%;" align="right">
                                    <cc1:CustLabel ID="lblRecordNameTwo" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_008"
                                        StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 35%;">
                                    <cc1:CustTextBox ID="txtRecordNameTwo" runat="server" Width="240px" IsValEmpty="False"
                                        ValEmptyMsg="" MaxLength="19" onkeydown="entersubmit('btnBasicSubmit');"  onblur="changeFullType(this);"
                                        onpaste="paste();" onfocus="allselect(this);" BoxName="登記名稱" checktype="fulltype"></cc1:CustTextBox></td>
                                <td align="right" style="width: 15%;">
                                    <cc1:CustLabel ID="lblBusinessNameTwo" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_009"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%;">
                                    <cc1:CustTextBox ID="txtBusinessNameTwo" runat="server" Width="240px" MaxLength="19"
                                        onkeydown="entersubmit('btnBasicSubmit');"  onblur="changeFullType(this);" onpaste="paste();"
                                        onfocus="allselect(this);" BoxName="營業名稱" checktype="fulltype"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="trEven">
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblMerchantNameTwo" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_010"
                                        StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtMerchantNameTwo" runat="server" Width="240px" MaxLength="14"
                                        onkeydown="entersubmit('btnBasicSubmit');"  onblur="changeFullType(this);" onpaste="paste();"
                                        onfocus="allselect(this);" BoxName="帳單列示名稱" checktype="fulltype"></cc1:CustTextBox></td>
                                <td align="right" style="width: 15%">&nbsp;<cc1:CustLabel ID="lblEnglishName" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_011"
                                    StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtEnglishName" runat="server" Width="240px" MaxLength="24"
                                        onkeydown="entersubmit('btnBasicSubmit');"  onfocus="allselect(this);"
                                        BoxName="商店英文名稱"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblUndertaker" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_012" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtUndertaker" runat="server" Width="130px" MaxLength="4" onkeydown="entersubmit('btnBasicSubmit');"
                                         onblur="changeFullType(this);" onpaste="paste();" onfocus="allselect(this);"
                                        BoxName="負責人" checktype="fulltype"></cc1:CustTextBox></td>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblUndertakerID" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_013"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtUndertakerID" runat="server" Width="140px" MaxLength="10"
                                        onkeydown="entersubmit('btnBasicSubmit');" checktype="ID" onfocus="allselect(this);"
                                        BoxName="負責人ID" InputType="LetterAndInt"></cc1:CustTextBox></td>
                            </tr>
                            <%--20190730 長姓名需求-負責人部份--%>
                            <tr class="trOdd">
                                <%--中文長姓名--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030100_102" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustCheckBox ID="chkisLongName" runat="server" OnCheckedChanged="CheckBox_CheckedChanged"
                                        AutoPostBack="True" BoxName="長姓名" />
                                    <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtUndertaker_L" runat="server" MaxLength="50" checktype="fulltype" Width="260px"
                                        onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人長姓名"
                                        onpaste="paste();" AutoPostBack="true" OnTextChanged="TextBox_TextChanged"></cc1:CustTextBox>
                                </td>
                                <%--羅馬拼音--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtUndertaker_Pinyin" runat="server" MaxLength="50" checktype="fulltype" Width="260px"
                                        onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人羅馬拼音"
                                        onpaste="paste();"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <%--20190730 長姓名需求-負責人部份--%>
                            <tr class="trEven">
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblUndertakerEngName" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_014"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtUndertakerEngName" runat="server" Width="240px" MaxLength="25"
                                        onkeydown="entersubmit('btnBasicSubmit');" onfocus="allselect(this);"
                                        BoxName="負責人英文名"></cc1:CustTextBox>
                                </td>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblUndertakerTel" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_015"
                                        StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtUndertakerTel1" runat="server" Width="40px" MaxLength="3"
                                        onkeydown="entersubmit('btnBasicSubmit');" checktype="num" onfocus="allselect(this);"
                                        BoxName="負責人電話一"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtUndertakerTel2" runat="server" Width="100px" MaxLength="8"
                                        onkeydown="entersubmit('btnBasicSubmit');" checktype="num" onfocus="allselect(this);"
                                        BoxName="負責人電話二"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtUndertakerTel3" runat="server" Width="40px" MaxLength="5"
                                        onkeydown="entersubmit('btnBasicSubmit');" checktype="num" onfocus="allselect(this);"
                                        BoxName="負責人電話三"></cc1:CustTextBox></td>
                            </tr>

                            <tr class="trOdd">
                                <%--國籍--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblCountryCode" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_106" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%" colspan="3">
                                    <div style="position: relative">
                                        <cc1:CustTextBox ID="txtCountryCode" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                            onkeydown="entersubmit('btnBasicSubmit');" BoxName="國籍"
                                            Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px;"></cc1:CustTextBox>
                                        <cc1:CustDropDownList ID="dropCountryCode" kind="select" runat="server" onclick="simOptionClick4IE('txtCountryCode');"
                                            Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                        </cc1:CustDropDownList>
                                        <cc1:CustHiddenField ID="hidCountryCode" runat="server" />
                                    </div>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <%--護照號碼--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblPassportNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_107" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtPassportNo" runat="server" MaxLength="22" Width="200px"
                                        onkeydown="entersubmit('btnBasicSubmit');" BoxName="護照號碼" InputType="LetterAndInt"></cc1:CustTextBox>
                                </td>
                                <%--護照效期--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblPassportExpdt" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_114" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtPassportExpdt" runat="server" MaxLength="8" Width="200px"
                                        onkeydown="entersubmit('btnBasicSubmit');" BoxName="護照效期" InputType="LetterAndInt"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <%--居留證號--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblResidentNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_108" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtResidentNo" runat="server" MaxLength="22" Width="200px"
                                        onkeydown="entersubmit('btnBasicSubmit');" BoxName="居留證號"></cc1:CustTextBox>
                                </td>
                                <%--居留效期--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblResidentExpdt" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_115" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtResidentExpdt" runat="server" MaxLength="8" Width="200px"
                                        onkeydown="entersubmit('btnBasicSubmit');" BoxName="居留效期"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblUndertakerAdd" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_016"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td colspan="3" style="width: 85%">
                                    <cc1:CustTextBox ID="txtUndertakerAdd1" runat="server" Width="130px" MaxLength="6"
                                        onkeydown="entersubmit('btnBasicSubmit');"  onblur="changeFullType(this);" onpaste="paste();"
                                        onfocus="allselect(this);" BoxName="負責人戶籍地址一" checktype="fulltype"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtUndertakerAdd2" runat="server" Width="280px" MaxLength="14"
                                        onkeydown="entersubmit('btnBasicSubmit');"  onblur="changeFullType(this);" onpaste="paste();"
                                        onfocus="allselect(this);" BoxName="負責人戶籍地址二" checktype="fulltype"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtUndertakerAdd3" runat="server" Width="140px" MaxLength="7"
                                        onkeydown="entersubmit('btnBasicSubmit');"  onblur="changeFullType(this);" onpaste="paste();"
                                        onfocus="allselect(this);" BoxName="負責人戶籍地址三" checktype="fulltype"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="trOdd" style="display: none">
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblRealPerson" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_017" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtRealPerson" runat="server" Width="130px" IsValEmpty="False"
                                        ValEmptyMsg="" MaxLength="4" onkeydown="entersubmit('btnBasicSubmit');"  onblur="changeFullType(this);"
                                        onpaste="paste();" onfocus="allselect(this);" BoxName="實際經營人" checktype="fulltype"></cc1:CustTextBox></td>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblRealPersonID" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_018"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtRealPersonID" runat="server" Width="140px" MaxLength="10"
                                        onkeydown="entersubmit('btnBasicSubmit');" checktype="ID" onfocus="allselect(this);"
                                        BoxName="實際經營人ID" InputType="LetterAndInt"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="trOdd">
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_020"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%" colspan="3">
                                    <cc1:CustTextBox ID="txtJunctionPerson" runat="server" Width="140px" MaxLength="4"
                                        onkeydown="entersubmit('btnBasicSubmit');"  onblur="changeFullType(this);" onpaste="paste();"
                                        onfocus="allselect(this);" BoxName="聯絡人" checktype="fulltype"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="trEven" style="display: none">
                                <td align="right" style="width: 15%">&nbsp;&nbsp;
                                    <cc1:CustLabel ID="lblRealPersonTel" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_019"
                                        StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtRealPersonTel1" runat="server" Width="36px" MaxLength="3"
                                        onkeydown="entersubmit('btnBasicSubmit');" checktype="num" onfocus="allselect(this);"
                                        BoxName="實際經營人電話一"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtRealPersonTel2" runat="server" Width="110px" MaxLength="8"
                                        onkeydown="entersubmit('btnBasicSubmit');" checktype="num" onfocus="allselect(this);"
                                        BoxName="實際經營人電話二"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtRealPersonTel3" runat="server" Width="50px" MaxLength="5"
                                        onkeydown="entersubmit('btnBasicSubmit');" checktype="num" onfocus="allselect(this);"
                                        BoxName="實際經營人電話三"></cc1:CustTextBox></td>
                            </tr>
                            <%--20190730 長姓名需求-聯絡人部份--%>
                            <tr class="trOdd">
                                <%--中文長姓名--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030100_103" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustCheckBox ID="chkisLongName_c" runat="server" OnCheckedChanged="CheckBox_CheckedChanged" AutoPostBack="True" BoxName="長姓名" />
                                    <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtJunctionPerson_L" runat="server" MaxLength="50" checktype="fulltype" Width="260px"
                                        onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人長姓名"
                                        onpaste="paste();" AutoPostBack="true" OnTextChanged="TextBox_TextChanged"></cc1:CustTextBox>
                                </td>
                                <%--羅馬拼音--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtJunctionPerson_Pinyin" runat="server" MaxLength="50" checktype="fulltype" Width="260px"
                                        onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);" BoxName="負責人羅馬拼音"
                                        onpaste="paste();"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <%--20190730 長姓名需求-聯絡人部份--%>
                            <tr class="trEven">
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblJunctionPersonTel" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_021"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtJunctionPersonTel1" runat="server" Width="36px" IsValEmpty="False"
                                        ValEmptyMsg="" MaxLength="3" onkeydown="entersubmit('btnBasicSubmit');" checktype="num"
                                        onfocus="allselect(this);" BoxName="聯絡人電話一"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtJunctionPersonTel2" runat="server" Width="110px" MaxLength="8"
                                        onkeydown="entersubmit('btnBasicSubmit');" checktype="num" onfocus="allselect(this);"
                                        BoxName="聯絡人電話二"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtJunctionPersonTel3" runat="server" Width="50px" MaxLength="5"
                                        onkeydown="entersubmit('btnBasicSubmit');" checktype="num" onfocus="allselect(this);"
                                        BoxName="聯絡人電話三"></cc1:CustTextBox></td>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblJunctionPersonFax" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_022"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtJunctionPersonFax1" runat="server" Width="36px" MaxLength="3"
                                        onkeydown="entersubmit('btnBasicSubmit');" checktype="num" onfocus="allselect(this);"
                                        BoxName="聯絡人傳真一"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtJunctionPersonFax2" runat="server" Width="110px" MaxLength="8"
                                        onkeydown="entersubmit('btnBasicSubmit');" checktype="num" onfocus="allselect(this);"
                                        BoxName="聯絡人傳真二"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="trEven" style="display: none">
                                <td align="right" style="width: 15%; height: 15px;">
                                    <cc1:CustLabel ID="lblRealPersonAdd" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_023"
                                        StickHeight="False"></cc1:CustLabel></td>
                                <td colspan="3" style="width: 85%; height: 15px;">
                                    <cc1:CustTextBox ID="txtRealPersonAdd1" runat="server" Width="130px" MaxLength="6"
                                        onkeydown="entersubmit('btnBasicSubmit');"  onblur="changeFullType(this);" onpaste="paste();"
                                        onfocus="allselect(this);" BoxName="實際經營人戶籍一" checktype="fulltype"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtRealPersonAdd2" runat="server" Width="280px" MaxLength="14"
                                        onkeydown="entersubmit('btnBasicSubmit');"  onblur="changeFullType(this);" onpaste="paste();"
                                        onfocus="allselect(this);" BoxName="實際經營人戶籍二" checktype="fulltype"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtRealPersonAdd3" runat="server" Width="140px" MaxLength="7"
                                        onkeydown="entersubmit('btnBasicSubmit');"  onblur="changeFullType(this);" onpaste="paste();"
                                        onfocus="allselect(this);" BoxName="實際經營人戶籍三" checktype="fulltype"></cc1:CustTextBox></td>
                            </tr>

                            <%--E-MAIL--%>
                            <tr class="trOdd">
                                <td align="right" style="width: 12%">
                                    <cc1:CustLabel ID="lblEmail" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_109" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td align="left" colspan="3">
                                    <cc1:CustTextBox ID="txtEmailFront" runat="server" Width="200px"
                                        onkeydown="entersubmit('btnBasicSubmit');" BoxName="E-MAIL"></cc1:CustTextBox>
                                    @
                                    <cc1:CustRadioButton ID="radGmail" runat="server" AutoPostBack="False" GroupName="email" Text="gmail.com" Checked="true" />
                                    <cc1:CustRadioButton ID="radYahoo" runat="server" AutoPostBack="False" GroupName="email" Text="yahoo.com.tw" />
                                    <cc1:CustRadioButton ID="radHotmail" runat="server" AutoPostBack="False" GroupName="email" Text="hotmail.com" />
                                    <cc1:CustRadioButton ID="radOther" runat="server" AutoPostBack="False" GroupName="email" Text="其他：" />
                                    <cc1:CustTextBox ID="txtEmailOther" runat="server" Width="200px"
                                        onkeydown="entersubmit('btnBasicSubmit');" BoxName="E-MAIL"></cc1:CustTextBox>
                                    <cc1:CustHiddenField ID="hidEmailFall" runat="server" />
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblJunctionPersonRecadd" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_024"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td colspan="3" style="width: 85%">
                                    <cc1:CustTextBox ID="txtREG_ZIP_CODE" runat="server" Width="130px" MaxLength="7"
                                        onkeydown="entersubmit('btnBasicSubmit');"
                                        BoxName="登記郵遞區號" checktype="fulltype"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtJunctionPersonRecadd1" runat="server" Width="130px" MaxLength="6"
                                        onkeydown="entersubmit('btnBasicSubmit');"  onblur="changeFullType(this);" onpaste="paste();"
                                        onfocus="allselect(this);" BoxName="商店登記地址一" checktype="fulltype" AutoPostBack="true" OnTextChanged="TextBox_AddrChanged"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtJunctionPersonRecadd2" runat="server" Width="280px" MaxLength="14"
                                        onkeydown="entersubmit('btnBasicSubmit');"  onblur="changeFullType(this);" onpaste="paste();"
                                        onfocus="allselect(this);" BoxName="商店登記地址二" checktype="fulltype"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtJunctionPersonRecadd3" runat="server" Width="140px" MaxLength="7"
                                        onkeydown="entersubmit('btnBasicSubmit');"  onblur="changeFullType(this);" onpaste="paste();"
                                        onfocus="allselect(this);" BoxName="商店登記地址三" checktype="fulltype"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="trOdd">
                                <td align="right" style="width: 15%">&nbsp;&nbsp;
                                    <cc1:CustLabel ID="lblJunctionPersonRealadd" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_025"
                                        StickHeight="False"></cc1:CustLabel></td>
                                <td colspan="3" style="width: 85%">
                                    <cc1:CustTextBox ID="txtRealaddZip" runat="server" Width="60px" MaxLength="3" onkeydown="entersubmit('btnBasicSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="商店營業地址一"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtJunctionPersonRealadd1" runat="server" Width="130px" MaxLength="6"
                                        onkeydown="entersubmit('btnBasicSubmit');"  onblur="changeFullType(this);" onpaste="paste();"
                                        onfocus="allselect(this);" BoxName="商店營業地址二" checktype="fulltype"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtJunctionPersonRealadd2" runat="server" Width="280px" MaxLength="14"
                                        onkeydown="entersubmit('btnBasicSubmit');"  onblur="changeFullType(this);" onpaste="paste();"
                                        onfocus="allselect(this);" BoxName="商店營業地址三" checktype="fulltype"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtJunctionPersonRealadd3" runat="server" Width="140px" MaxLength="7"
                                        onkeydown="entersubmit('btnBasicSubmit');"  onblur="changeFullType(this);" onpaste="paste();"
                                        onfocus="allselect(this);" BoxName="商店營業地址四" checktype="fulltype"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="trEven">
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblCommadd" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_026" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td colspan="3" style="width: 85%">
                                    <cc1:CustTextBox ID="txtZip" runat="server" Width="60px" MaxLength="3" onkeydown="entersubmit('btnBasicSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="帳單地址一"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtCommadd1" runat="server" Width="130px" MaxLength="14" onkeydown="entersubmit('btnBasicSubmit');"
                                         onblur="changeFullType(this);" onpaste="paste();" onfocus="allselect(this);"
                                        BoxName="帳單地址二" checktype="fulltype"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtComaddr2" runat="server" Width="280px" MaxLength="14" onkeydown="entersubmit('btnBasicSubmit');"
                                         onblur="changeFullType(this);" onpaste="paste();" onfocus="allselect(this);"
                                        BoxName="帳單地址三" checktype="fulltype"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="trOdd">
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblIntroduces" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_027" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtIntroduces" runat="server" Width="120px" MaxLength="3" onkeydown="entersubmit('btnBasicSubmit');"
                                         onblur="changeFullType(this);" onpaste="paste();" onfocus="allselect(this);"
                                        BoxName="推廣員" checktype="fulltype"></cc1:CustTextBox></td>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblIntroduceFlag" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_028"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtIntroduceFlag" runat="server" Width="140px" MaxLength="5"
                                        onkeydown="entersubmit('btnBasicSubmit');" checktype="num" onfocus="allselect(this);"
                                        BoxName="推廣代號"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="trEven">
                                <td align="right" style="width: 15%; height: 13px;">
                                    <cc1:CustLabel ID="lblInvoiceCycle" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_029"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%; height: 13px;">
                                    <cc1:CustTextBox ID="txtInvoiceCycle" runat="server" Width="60px" MaxLength="2" onkeydown="entersubmit('btnBasicSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="發票週期"></cc1:CustTextBox></td>
                                <td align="right" style="width: 15%; height: 13px;">&nbsp;<cc1:CustLabel ID="lblHOLDSTMT" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030200_030"
                                    StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 35%; height: 13px;">
                                    <cc1:CustTextBox ID="txtHOLDSTMT" runat="server" Width="50px" MaxLength="1" onkeydown="entersubmit('btnBasicSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="HOLD STMT"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="trOdd">
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblReqAppro" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_105" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 85%" colspan="3">
                                    <cc1:CustTextBox ID="txtReqAppro" runat="server" Width="30px" MaxLength="1" onkeydown="entersubmit('btnBasicSubmit');"
                                        checktype="letter" BoxName="請款加批" onblur="upperCase()"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblBlack" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_031" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtBlack" runat="server" Width="30px" MaxLength="1" onkeydown="entersubmit('btnBasicSubmit');"
                                        checktype="numandletter" onfocus="allselect(this);" BoxName="黑名單"></cc1:CustTextBox></td>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblClass" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_032" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtClass" runat="server" Width="60px" MaxLength="1" onkeydown="entersubmit('btnBasicSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="共用類別"></cc1:CustTextBox></td>
                            </tr>
                            <%--資料最後異動--%>
                            <tr class="trOdd">                                  
                                
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
                                <td style="width: 18%; height: 33px;" >
                                    <cc1:CustTextBox ID="txtLAST_UPD_CHECKER" runat="server" MaxLength="9" checktype="numandletter"
                                        Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動CHECKER" onfocus="allselect(this);"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <%--資料最後異動分行--%>
                                <td align="right" style="width: 11%; height: 33px;">
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
                            <tr class="itemTitle1">
                                <td colspan="4" align="center">&nbsp; &nbsp;<cc1:CustButton ID="btnBasicSubmit" Text="" runat="server" Width="57px"
                                    CssClass="smallButton" Enabled="False" OnClick="btnSubmitClick" OnClientClick="return checktextbox('tabBasic',1);"
                                    onkeydown="setfocus('btnBasicData');" DisabledWhenSubmit="False" ShowID="01_01030200_002" />
                                </td>
                            </tr>
                        </table>
                    </cc1:CustPanel>
                    <cc1:CustPanel ID="pnlFee" runat="server" Height="348px" Width="100%" Visible="False">
                        <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabFee" style="">
                            <tr class="itemTitle1">
                                <td colspan="4" style="width: 70%">
                                    <li>
                                        <cc1:CustLabel ID="lblFee" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                            SetOmit="False" StickHeight="False" Width="218px" IsColon="False" ShowID="01_01030200_033"></cc1:CustLabel></li>
                                </td>
                                <td colspan="2" style="width: 30%">
                                    <li>
                                        <cc1:CustLabel ID="lblCup" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                            SetOmit="False" StickHeight="False" Width="240px" IsColon="False" ShowID="01_01030200_104"></cc1:CustLabel></li>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblAddjcb" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_034" StickHeight="False"></cc1:CustLabel></td>
                                <td colspan="3" style="width: 55%">
                                    <cc1:CustTextBox ID="txtAddjcb" runat="server" MaxLength="1" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" onkeyup="changeFeeStatus();" BoxName="新增JCB(全)"
                                        Width="70px" TabIndex="1"></cc1:CustTextBox>&nbsp;&nbsp;<cc1:CustLabel ID="lblMessage2"
                                            runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2" IsColon="False"
                                            IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                            SetOmit="False" ShowID="01_01030200_035" StickHeight="False" ForeColor="Red"></cc1:CustLabel></td>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblTag1" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 15%">
                                    <cc1:CustTextBox ID="txtStatus1" runat="server" MaxLength="1" checktype="num" onkeydown="entersubmit('btnFeeSubmit');"
                                        Width="31px" onfocus="allselect(this);" BoxName="舊制外加費率一STATUS" TabIndex="23"></cc1:CustTextBox>
                                    &nbsp;&nbsp;<cc1:CustTextBox ID="txtFee1" runat="server" MaxLength="5" checktype="num"
                                        onkeydown="entersubmit('btnFeeSubmit');" Width="60px" onfocus="allselect(this);"
                                        BoxName="舊制外加費率一" TabIndex="24"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblAddjcbUs" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_036" StickHeight="False"></cc1:CustLabel></td>
                                <td colspan="3" style="width: 55%">
                                    <cc1:CustTextBox ID="txtAddjcbUs" runat="server" MaxLength="1" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="新增JCB(本行)" Width="70px" TabIndex="2"></cc1:CustTextBox></td>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblTag2" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 15%">
                                    <cc1:CustTextBox ID="txtStatus2" runat="server" MaxLength="1" checktype="num" onkeydown="entersubmit('btnFeeSubmit');"
                                        Width="31px" onfocus="allselect(this);" BoxName="舊制外加費率二STATUS" TabIndex="25"></cc1:CustTextBox>
                                    &nbsp;&nbsp;<cc1:CustTextBox ID="txtFee2" runat="server" MaxLength="5" checktype="num"
                                        onkeydown="entersubmit('btnFeeSubmit');" Width="60px" onfocus="allselect(this);"
                                        BoxName="舊制外加費率二" TabIndex="26"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblAddjcbNotus" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_037" StickHeight="False"></cc1:CustLabel></td>
                                <td colspan="3" style="width: 55%">
                                    <cc1:CustTextBox ID="txtAddjcbNotus" runat="server" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" MaxLength="1" BoxName="新增JCB(他行)"
                                        Width="70px" TabIndex="3"></cc1:CustTextBox></td>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblTag3" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 15%">
                                    <cc1:CustTextBox ID="txtStatus3" runat="server" MaxLength="1" checktype="num" onkeydown="entersubmit('btnFeeSubmit');"
                                        Width="31px" onfocus="allselect(this);" BoxName="舊制外加費率三STATUS" TabIndex="27"></cc1:CustTextBox>
                                    &nbsp;&nbsp;<cc1:CustTextBox ID="txtFee3" runat="server" MaxLength="5" checktype="num"
                                        onkeydown="entersubmit('btnFeeSubmit');" Width="60px" onfocus="allselect(this);"
                                        BoxName="舊制外加費率三" TabIndex="28"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblModdate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_038" StickHeight="False"></cc1:CustLabel></td>
                                <td colspan="3" style="width: 55%">
                                    <cc1:CustTextBox ID="txtModdate" runat="server" MaxLength="6" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="費率異動日" Width="70px" TabIndex="4"></cc1:CustTextBox><span
                                            style="color: Red">&nbsp;&nbsp;<cc1:CustLabel ID="lblMessage3" runat="server" CurAlign="left"
                                                CurSymbol="£" FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0"
                                                NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False" ForeColor="Red"
                                                IsColon="False" ShowID="01_01030200_039"></cc1:CustLabel></td>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblTag4" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 15%">
                                    <cc1:CustTextBox ID="txtStatus4" runat="server" MaxLength="1" checktype="num" onkeydown="entersubmit('btnFeeSubmit');"
                                        Width="31px" onfocus="allselect(this);" BoxName="舊制外加費率四STATUS" TabIndex="29"></cc1:CustTextBox>
                                    &nbsp;&nbsp;<cc1:CustTextBox ID="txtFee4" runat="server" MaxLength="5" checktype="num"
                                        onkeydown="entersubmit('btnFeeSubmit');" Width="60px" onfocus="allselect(this);"
                                        BoxName="舊制外加費率四" TabIndex="30"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblPage1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_040" StickHeight="False"></cc1:CustLabel></td>
                                <td colspan="3" style="width: 55%">
                                    <cc1:CustTextBox ID="txtPage1" runat="server" MaxLength="5" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" onkeyup="changeFeeUsStatus();" BoxName="本行卡(1.3.5.7)"
                                        Width="70px" TabIndex="5"></cc1:CustTextBox>&nbsp;&nbsp;<cc1:CustLabel ID="lblMessage1"
                                            runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2" IsColon="False"
                                            IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                            SetOmit="False" ShowID="01_01030200_041" StickHeight="False" ForeColor="Red"></cc1:CustLabel></td>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblTag5" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 15%">
                                    <cc1:CustTextBox ID="txtStatus5" runat="server" MaxLength="1" checktype="num" onkeydown="entersubmit('btnFeeSubmit');"
                                        Width="31px" onfocus="allselect(this);" BoxName="舊制外加費率五STATUS" TabIndex="31"></cc1:CustTextBox>
                                    &nbsp;&nbsp;<cc1:CustTextBox ID="txtFee5" runat="server" MaxLength="5" checktype="num"
                                        onkeydown="entersubmit('btnFeeSubmit');" Width="60px" onfocus="allselect(this);"
                                        BoxName="舊制外加費率五" TabIndex="32"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblPage2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_042" StickHeight="False"></cc1:CustLabel></td>
                                <td colspan="3" style="width: 55%">
                                    <cc1:CustTextBox ID="txtPage2" runat="server" MaxLength="5" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" onkeyup="changeFeeOtherStatus();"
                                        BoxName="他行卡(2.4.6.8)" Width="70px" TabIndex="6"></cc1:CustTextBox></td>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblTag6" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 15%">
                                    <cc1:CustTextBox ID="txtStatus6" runat="server" MaxLength="1" checktype="num" onkeydown="entersubmit('btnFeeSubmit');"
                                        Width="31px" onfocus="allselect(this);" BoxName="舊制外加費率六STATUS" TabIndex="33"></cc1:CustTextBox>
                                    &nbsp;&nbsp;<cc1:CustTextBox ID="txtFee6" runat="server" MaxLength="5" checktype="num"
                                        onkeydown="entersubmit('btnFeeSubmit');" Width="60px" onfocus="allselect(this);"
                                        BoxName="舊制外加費率六" TabIndex="34"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblPage3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_043" StickHeight="False"></cc1:CustLabel></td>
                                <td colspan="3" style="width: 55%">
                                    <cc1:CustTextBox ID="txtPage3" runat="server" MaxLength="5" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" onkeyup="changeFeeAllStatus();" BoxName="全卡(1-8)"
                                        Width="70px" TabIndex="7"></cc1:CustTextBox></td>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblTag7" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 15%">
                                    <cc1:CustTextBox ID="txtStatus7" runat="server" MaxLength="1" checktype="num" onkeydown="entersubmit('btnFeeSubmit');"
                                        Width="31px" onfocus="allselect(this);" BoxName="舊制外加費率七STATUS" TabIndex="35"></cc1:CustTextBox>
                                    &nbsp;&nbsp;<cc1:CustTextBox ID="txtFee7" runat="server" MaxLength="5" checktype="num"
                                        onkeydown="entersubmit('btnFeeSubmit');" Width="60px" onfocus="allselect(this);"
                                        BoxName="舊制外加費率七" TabIndex="36"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td style="width: 15%; height: 25px;" align="right">
                                    <cc1:CustLabel ID="lblPage4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_044" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 20%; height: 25px;">
                                    <cc1:CustTextBox ID="txtPage4" runat="server" Width="60px" MaxLength="5" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="01N(本行)" TabIndex="8"></cc1:CustTextBox></td>
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblRate09" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_052" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 20%">
                                    <cc1:CustTextBox ID="txtRate09" runat="server" Width="60px" MaxLength="5" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="09" TabIndex="16"></cc1:CustTextBox></td>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblTag8" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 15%">
                                    <cc1:CustTextBox ID="txtStatus8" runat="server" MaxLength="1" checktype="num" onkeydown="entersubmit('btnFeeSubmit');"
                                        Width="31px" onfocus="allselect(this);" BoxName="舊制外加費率八STATUS" TabIndex="37"></cc1:CustTextBox>
                                    &nbsp;&nbsp;<cc1:CustTextBox ID="txtFee8" runat="server" MaxLength="5" checktype="num"
                                        onkeydown="entersubmit('btnFeeSubmit');" Width="60px" onfocus="allselect(this);"
                                        BoxName="舊制外加費率八" TabIndex="38"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td style="width: 15%; height: 25px;" align="right">
                                    <cc1:CustLabel ID="lblRate02N" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_045" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 20%; height: 25px;">
                                    <cc1:CustTextBox ID="txtRate02N" runat="server" Width="60px" MaxLength="5" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="02N(他行)" TabIndex="9"></cc1:CustTextBox></td>
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblRate10" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_053" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 20%">
                                    <cc1:CustTextBox ID="txtRate10" runat="server" Width="60px" MaxLength="5" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="10" TabIndex="17"></cc1:CustTextBox></td>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblTag9" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 15%">
                                    <cc1:CustTextBox ID="txtStatus9" runat="server" MaxLength="1" checktype="num" onkeydown="entersubmit('btnFeeSubmit');"
                                        Width="31px" onfocus="allselect(this);" BoxName="舊制外加費率九STATUS" TabIndex="39"></cc1:CustTextBox>
                                    &nbsp;&nbsp;<cc1:CustTextBox ID="txtFee9" runat="server" MaxLength="5" checktype="num"
                                        onkeydown="entersubmit('btnFeeSubmit');" Width="60px" onfocus="allselect(this);"
                                        BoxName="舊制外加費率九" TabIndex="40"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td style="width: 15%; height: 25px;" align="right">
                                    <cc1:CustLabel ID="lblRate03V" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_046" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 20%; height: 25px;">
                                    <cc1:CustTextBox ID="txtRate03V" runat="server" Width="60px" MaxLength="5" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="03V(本行)" TabIndex="10"></cc1:CustTextBox></td>
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblRate11" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_054" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 20%">
                                    <cc1:CustTextBox ID="txtRate11" runat="server" Width="60px" MaxLength="5" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="11" TabIndex="18"></cc1:CustTextBox></td>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblTag10" runat="server" CurAlign="left" CurSymbol="￡" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 15%">
                                    <cc1:CustTextBox ID="txtStatus10" runat="server" MaxLength="1" checktype="num" onkeydown="entersubmit('btnFeeSubmit');"
                                        Width="31px" onfocus="allselect(this);" BoxName="舊制外加費率十STATUS" TabIndex="41"></cc1:CustTextBox>
                                    &nbsp;&nbsp;<cc1:CustTextBox ID="txtFee10" runat="server" MaxLength="5" checktype="num"
                                        onkeydown="entersubmit('btnFeeSubmit');" Width="60px" onfocus="allselect(this);"
                                        BoxName="舊制外加費率十" TabIndex="42"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td style="width: 15%; height: 25px;" align="right">
                                    <cc1:CustLabel ID="lblRate04V" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_047" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 20%; height: 25px;">
                                    <cc1:CustTextBox ID="txtRate04V" runat="server" Width="60px" MaxLength="5" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="04V(他行)" TabIndex="11"></cc1:CustTextBox></td>
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblRate12" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_055" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 20%">
                                    <cc1:CustTextBox ID="txtRate12" runat="server" Width="60px" MaxLength="5" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="12" TabIndex="19"></cc1:CustTextBox></td>
                                <td colspan="2" style="width: 30%"></td>
                            </tr>
                            <tr class="trEven">
                                <td style="width: 15%; height: 25px;" align="right">
                                    <cc1:CustLabel ID="lblRate05M" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_048" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 20%; height: 25px;">
                                    <cc1:CustTextBox ID="txtRate05M" runat="server" Width="60px" MaxLength="5" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="05M(本行)" TabIndex="12"></cc1:CustTextBox></td>
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblRate13" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_056" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 20%">
                                    <cc1:CustTextBox ID="txtRate13" runat="server" Width="60px" MaxLength="5" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="13" TabIndex="20"></cc1:CustTextBox></td>
                                <td colspan="2" style="width: 30%"></td>
                            </tr>
                            <tr class="trOdd">
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblRate06M" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_049" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 20%">
                                    <cc1:CustTextBox ID="txtRate06M" runat="server" Width="60px" MaxLength="5" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="06M(他行)" TabIndex="13"></cc1:CustTextBox></td>
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblRate14" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_057" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 20%">
                                    <cc1:CustTextBox ID="txtRate14" runat="server" Width="60px" MaxLength="5" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="14" TabIndex="21"></cc1:CustTextBox></td>
                                <td colspan="2" style="width: 30%"></td>
                            </tr>
                            <tr class="trEven">
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblRate07J" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_050" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 20%">
                                    <cc1:CustTextBox ID="txtRate07J" runat="server" Width="60px" MaxLength="5" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="07J(本行)" TabIndex="14"></cc1:CustTextBox></td>
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblRate15" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_058" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 20%">
                                    <cc1:CustTextBox ID="txtRate15" runat="server" Width="60px" MaxLength="5" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="15" TabIndex="22"></cc1:CustTextBox></td>
                                <td colspan="2" style="width: 30%"></td>
                            </tr>
                            <tr class="trOdd">
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblRate08J" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_051" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 20%">
                                    <cc1:CustTextBox ID="txtRate08J" runat="server" Width="60px" MaxLength="5" onkeydown="entersubmit('btnFeeSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="08J(他行)" TabIndex="15"></cc1:CustTextBox></td>
                                <td colspan="2" style="width: 35%"></td>
                                <td colspan="2" style="width: 30%"></td>
                            </tr>
                            <tr class="itemTitle1">
                                <td colspan="6" align="center">
                                    <cc1:CustButton ID="btnFeeSubmit" Text="" runat="server" Width="57px" CssClass="smallButton"
                                        Enabled="False" OnClick="btnFeeSubmit_Click" OnClientClick="return checktextbox('pnlFee',2);"
                                        onkeydown="setfocus('btnBasicData');" DisabledWhenSubmit="False" ShowID="01_01030200_002"
                                        TabIndex="43" />
                                </td>
                            </tr>
                        </table>
                    </cc1:CustPanel>
                    <cc1:CustPanel ID="pnlAccount" Width="100%" runat="server" Visible="False">
                        <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabAccount" style="">
                            <tr class="itemTitle1">
                                <td colspan="2">
                                    <li>
                                        <cc1:CustLabel ID="lblAccount" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                            SetOmit="False" StickHeight="False" Width="253px" IsColon="False" ShowID="01_01030200_059"></cc1:CustLabel></li>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblBankName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_060" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 85%">
                                    <cc1:CustTextBox ID="txtBankName" runat="server" MaxLength="5" onkeydown="entersubmit('btnAccountSubmit');"
                                         onblur="changeFullType(this);" onpaste="paste();" onfocus="allselect(this);"
                                        BoxName="銀行名稱" checktype="fulltype"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="trEven">
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblBranchName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_061" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 85%">
                                    <cc1:CustTextBox ID="txtBranchName" runat="server" MaxLength="10" onkeydown="entersubmit('btnAccountSubmit');"
                                         onblur="changeFullType(this);" onpaste="paste();" onfocus="allselect(this);"
                                        BoxName="分行名稱" checktype="fulltype"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="trOdd">
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblAccountName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_062" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 85%">
                                    <cc1:CustTextBox ID="txtAccount" runat="server" MaxLength="20" onkeydown="entersubmit('btnAccountSubmit');"
                                         onblur="changeFullType(this);" onpaste="paste();" onfocus="allselect(this);"
                                        BoxName="戶名" checktype="fulltype"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="trEven">
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblCheckNum" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_063" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 85%">
                                    <cc1:CustTextBox ID="txtCheckNum" runat="server" MaxLength="4" checktype="num" onkeydown="entersubmit('btnAccountSubmit');"
                                        onpaste="paste();" onfocus="allselect(this);" Width="96px" BoxName="檢碼"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="trOdd">
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblAccountID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_064" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 85%">
                                    <cc1:CustTextBox ID="txtAccount1" runat="server" Width="96px" MaxLength="3" onkeydown="entersubmit('btnAccountSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="帳號一"></cc1:CustTextBox>&nbsp;<cc1:CustTextBox
                                            ID="txtAccount2" runat="server" MaxLength="14" onkeydown="entersubmit('btnAccountSubmit');"
                                            checktype="num" onfocus="allselect(this);" BoxName="帳號二"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="itemTitle1">
                                <td colspan="2" align="center">
                                    <cc1:CustButton ID="btnAccountSubmit" Text="" runat="server" Width="57px" CssClass="smallButton"
                                        Enabled="False" OnClick="btnAccountSubmit_Click" OnClientClick="return checktextbox('tabAccount',4);"
                                        onkeydown="setfocus('btnBasicData');" DisabledWhenSubmit="False" ShowID="01_01030200_002" />
                                </td>
                            </tr>
                        </table>
                    </cc1:CustPanel>
                    <cc1:CustPanel ID="pnlCancelTask" runat="server" Width="100%" Visible="False">
                        <table border="0" cellpadding="0" cellspacing="1" width="100%" id="tabCancelCode">
                            <tr class="itemTitle1">
                                <td colspan="2" style="height: 25px">
                                    <li>
                                        <cc1:CustLabel ID="lblCancelCode" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                            SetOmit="False" StickHeight="False" Width="249px" IsColon="False" ShowID="01_01030200_065"></cc1:CustLabel></li>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblCancelCode1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_066" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 85%">
                                    <cc1:CustTextBox ID="txtCancelCode1" runat="server" Width="90px" MaxLength="1" checktype="num"
                                        onkeydown="entersubmit('btnCancelTaskSubmit');" onfocus="allselect(this);"
                                        BoxName="解約代號"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="trEven">
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblCancelDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_067" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 85%">
                                    <cc1:CustTextBox ID="txtCancelDate" runat="server" Width="90px" MaxLength="6" onkeydown="entersubmit('btnCancelTaskSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="解約日期"></cc1:CustTextBox>&nbsp;&nbsp;<cc1:CustLabel
                                            ID="lblMessageDate" runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01030200_068" StickHeight="False"></cc1:CustLabel></td>
                            </tr>
                            <tr class="trOdd">
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lblCancelCode2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_069" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 85%">
                                    <cc1:CustTextBox ID="txtCancelCode2" runat="server" Width="90px" MaxLength="2" onkeydown="entersubmit('btnCancelTaskSubmit');"
                                        checktype="numandletter" onfocus="allselect(this);" BoxName="解約原因碼"></cc1:CustTextBox></td>
                            </tr>
                            <%--分隔行--%>
                            <tr class="itemTitle1">
                                <td colspan="2"></td>
                            </tr>
                            <tr class="trEven">
                                <td></td>
                                <td>
                                    <cc1:CustCheckBox ID="chkCancelRevert" runat="server" 
                                        AutoPostBack="True" BoxName="解約還原(CheckBox)" />
                                    <cc1:CustLabel ID="lblCancelRevert" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_130"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <%--分隔行--%>
                            <tr class="itemTitle1">
                                <td colspan="2"></td>
                            </tr>                           
                            <tr class="trOdd">
                                <%--資料最後異動MAKER--%>
                                <td align="right" style="width: 18%; height: 33px;" colspan="1">
                                    <cc1:CustLabel ID="lbLAST_UPD_MAKER2" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_086"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 16%; height: 33px;" colspan="1">
                                    <cc1:CustTextBox ID="txtLAST_UPD_MAKER2" runat="server" MaxLength="9" checktype="numandletter"
                                        Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動MAKER" onfocus="allselect(this);"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <%--資料最後異動CHECKER--%>
                                <td align="right" style="width: 20%; height: 33px;">
                                    <cc1:CustLabel ID="lbLAST_UPD_CHECKER2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_087" StickHeight="False"
                                        ></cc1:CustLabel>
                                </td>
                                <td style="width: 18%; height: 33px;" >
                                    <cc1:CustTextBox ID="txtLAST_UPD_CHECKER2" runat="server" MaxLength="9" checktype="numandletter"
                                        Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動CHECKER" onfocus="allselect(this);"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <%--資料最後異動--%>
                            <tr class="trOdd">                            
                                <%--資料最後異動分行--%>
                                <td align="right" style="width: 11%; height: 33px;">
                                    <cc1:CustLabel ID="lbLAST_UPD_BRANCH2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_085" StickHeight="False"
                                        ></cc1:CustLabel>
                                </td>
                                <td style="width: 16%; height: 33px;">
                                    <cc1:CustTextBox ID="txtLAST_UPD_BRANCH2" runat="server" MaxLength="4" Width="100px"
                                        onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動分行" checktype="numandletter" 
                                        onfocus="allselect(this);"></cc1:CustTextBox>
                                </td>                                                                
                            </tr>
                            <tr class="itemTitle1">
                                <td colspan="2" align="center">
                                    <cc1:CustButton ID="btnCancelTaskSubmit" Text="" runat="server" Width="57px" CssClass="smallButton"
                                        Enabled="False" OnClick="btnCancelTaskSubmit_Click" OnClientClick="return checktextbox('tabCancelCode',5);"
                                        onkeydown="setfocus('btnBasicData');" DisabledWhenSubmit="False" ShowID="01_01030200_002" />
                                </td>
                            </tr>
                        </table>
                    </cc1:CustPanel>
                    <cc1:CustPanel ID="pnlMachineData" Width="100%" runat="server" Visible="False">
                        <table border="0" cellpadding="0" cellspacing="1" width="100%" id="tabMachine">
                            <tr class="itemTitle1">
                                <td colspan="6">
                                    <li>
                                        <cc1:CustLabel ID="lblMachineData" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                            SetOmit="False" StickHeight="False" Width="250px" IsColon="False" ShowID="01_01030200_070"></cc1:CustLabel></li>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td style="width: 10%" align="right"></td>
                                <td style="width: 13%" align="left">&nbsp;&nbsp;&nbsp;
                                    <cc1:CustLabel ID="lblType1" runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_080" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<cc1:CustLabel
                                            ID="lblCount1" runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01030200_081" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 10%" align="right"></td>
                                <td style="width: 13%" align="left">&nbsp;&nbsp;&nbsp;
                                    <cc1:CustLabel ID="lblType2" runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_080" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<cc1:CustLabel
                                            ID="lblCount2" runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01030200_081" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 10%" align="right"></td>
                                <td style="width: 13%" align="left"></td>
                            </tr>
                            <tr class="trEven">
                                <td style="width: 10%" align="right">
                                    <cc1:CustLabel ID="lblImp1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_071" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 13%" align="left">
                                    <cc1:CustTextBox ID="txtImp1Type" runat="server" Width="50px" MaxLength="4" onkeydown="entersubmit('btnMaSubmit');"
                                        checktype="numandletter" onfocus="allselect(this);" BoxName="IMP1機型"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtImp1" runat="server" Width="50px" MaxLength="3" onkeydown="entersubmit('btnMaSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="IMP1數量"></cc1:CustTextBox></td>
                                <td style="width: 10%" align="right">
                                    <cc1:CustLabel ID="lblImp2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_072" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 13%" align="left">
                                    <cc1:CustTextBox ID="txtImp2Type" runat="server" Width="50px" MaxLength="4" onkeydown="entersubmit('btnMaSubmit');"
                                        checktype="numandletter" onfocus="allselect(this);" BoxName="IMP2機型"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtImp2" runat="server" Width="50px" MaxLength="3" onkeydown="entersubmit('btnMaSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="IMP2數量"></cc1:CustTextBox></td>
                                <td style="width: 10%" align="right">
                                    <cc1:CustLabel ID="lblImpMoney" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_073" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 13%" align="left">
                                    <cc1:CustTextBox ID="txtImpMoney" runat="server" Width="50px" MaxLength="5" onkeydown="entersubmit('btnMaSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="保證金1"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="trOdd">
                                <td style="width: 10%" align="right">
                                    <cc1:CustLabel ID="lblPos1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_074" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 13%" align="left">
                                    <cc1:CustTextBox ID="txtPos1Type" runat="server" Width="50px" MaxLength="4" onkeydown="entersubmit('btnMaSubmit');"
                                        checktype="numandletter" onfocus="allselect(this);" BoxName="POS1機型"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtPos1" runat="server" Width="50px" MaxLength="3" onkeydown="entersubmit('btnMaSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="POS1數量"></cc1:CustTextBox></td>
                                <td style="width: 10%" align="right">
                                    <cc1:CustLabel ID="lblPos2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_075" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 13%" align="left">
                                    <cc1:CustTextBox ID="txtPos2Type" runat="server" Width="50px" MaxLength="4" onkeydown="entersubmit('btnMaSubmit');"
                                        checktype="numandletter" onfocus="allselect(this);" BoxName="POS2機型"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtPos2" runat="server" Width="50px" MaxLength="3" onkeydown="entersubmit('btnMaSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="POS2數量"></cc1:CustTextBox></td>
                                <td style="width: 10%" align="right">
                                    <cc1:CustLabel ID="lblPosMoney" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_076" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 13%" align="left">
                                    <cc1:CustTextBox ID="txtPosMoney" runat="server" Width="50px" MaxLength="5" onkeydown="entersubmit('btnMaSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="保證金2"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="trEven">
                                <td style="width: 10%" align="right">
                                    <cc1:CustLabel ID="lblEdc1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_077" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 13%" align="left">
                                    <cc1:CustTextBox ID="txtEdc1Type" runat="server" Width="50px" MaxLength="4" onkeydown="entersubmit('btnMaSubmit');"
                                        checktype="numandletter" onfocus="allselect(this);" BoxName="EDC1機型"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtEdc1" runat="server" Width="50px" MaxLength="3" onkeydown="entersubmit('btnMaSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="EDC1數量"></cc1:CustTextBox></td>
                                <td style="width: 10%" align="right">
                                    <cc1:CustLabel ID="lblEdcnum" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030200_078" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 13%" align="left">
                                    <cc1:CustTextBox ID="txtEdcType" runat="server" Width="50px" MaxLength="4" onkeydown="entersubmit('btnMaSubmit');"
                                        checktype="numandletter" onfocus="allselect(this);" BoxName="EDC編號機型"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtEdc" runat="server" Width="50px" MaxLength="3" onkeydown="entersubmit('btnMaSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="EDC編號數量"></cc1:CustTextBox></td>
                                <td style="width: 10%" align="right">
                                    <cc1:CustLabel ID="lblEdcMoney" runat="server" CurSymbol="£" FractionalDigit="0"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="2" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="True" ShowID="01_01030200_079" CurAlign="left"></cc1:CustLabel></td>
                                <td style="width: 13%" align="left">
                                    <cc1:CustTextBox ID="txtEdcMoney" runat="server" Width="50px" MaxLength="5" onkeydown="entersubmit('btnMaSubmit');"
                                        checktype="num" onfocus="allselect(this);" BoxName="保證金3"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="itemTitle1">
                                <td colspan="6" align="center">
                                    <cc1:CustButton ID="btnMaSubmit" Text="" runat="server" Width="57px" CssClass="smallButton"
                                        Enabled="False" OnClick="btnMaSubmit_Click" OnClientClick="return checktextbox('tabMachine',3);"
                                        onkeydown="setfocus('btnBasicData');" DisabledWhenSubmit="False" ShowID="01_01030200_002" />
                                </td>
                            </tr>
                        </table>
                    </cc1:CustPanel>
                    <cc1:CustPanel ID="pnlBasicByTaxno" runat="server" Height="363px" Width="100%" Visible="false">
                        <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabBasicByTaxno" style="">
                            <tr class="itemTitle1">
                                <%--基本資料異動--%>
                                <td colspan="5" align="left">
                                    <li>
                                        <cc1:CustLabel ID="CustLabel16" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                            SetOmit="False" StickHeight="False" Width="232px" IsColon="False" ShowID="01_01030100_104"></cc1:CustLabel>
                                    </li>
                            </tr>
                            <tr class="trEven">
                                <td rowspan="7" align="right" style="width: 7%">
                                    <cc1:CustLabel ID="CustLabel44" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030100_107" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <%--總公司統一編號--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel17" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_097"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtCORP_NO" runat="server" Width="140px" MaxLength="8"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" checktype="num" BoxName="總公司統一編號"  TabIndex="1"></cc1:CustTextBox>
                                </td>
                                <%--AML行業編號--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel43" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_096"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtCORP_MCC" runat="server" Width="80px" MaxLength="7"   TabIndex="2"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" BoxName="AML行業編號" AutoPostBack="true" OnTextChanged="txtCodeType_TextChanged"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <%--設立--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel19" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_098"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtCORP_ESTABLISH" runat="server" Width="60px" MaxLength="7"  TabIndex="3"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" checktype="num" BoxName="設立"></cc1:CustTextBox>
                                </td>
                                <%--法律形式--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel20" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_099"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <div style="position: relative">
                                        <cc1:CustTextBox ID="txtCORP_Organization" runat="server" MaxLength="2" Width="30px" onkeydown="entersubmit('btnSubmitByTaxno');" BoxName="法律形式"  TabIndex="4"
                                            Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px;" AutoPostBack="true" OnTextChanged="txtCodeType_TextChanged"></cc1:CustTextBox>
                                        <cc1:CustDropDownList ID="ddlCORP_Organization" kind="select" runat="server" onclick="simOptionClick4IE('txtCORP_Organization');"
                                            Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                        </cc1:CustDropDownList>
                                    </div>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <%--註冊國籍--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel32" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050100_013" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <div style="position: relative">
                                        <cc1:CustTextBox ID="txtCORP_CountryCode" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                            onkeydown="entersubmit('btnSubmitByTaxno');" BoxName="國籍"  TabIndex="5"
                                            Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px;" AutoPostBack="true" OnTextChanged="txtCodeType_TextChanged"></cc1:CustTextBox>
                                        <cc1:CustDropDownList ID="ddlCORP_CountryCode" kind="select" runat="server" onclick="simOptionClick4IE('txtCORP_CountryCode');"
                                            Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                        </cc1:CustDropDownList>
                                    </div>
                                </td>
                                <%--註冊國為美國者，請勾選州別--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblCountryStateCode" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050100_014"
                                        StickHeight="False" Style="color: Red"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <div style="position: relative">
                                        <cc1:CustTextBox ID="txtCORP_CountryStateCode" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                            onkeydown="entersubmit('btnSubmitByTaxno');" BoxName="註冊國為美國者，請勾選州別"  TabIndex="6"
                                            Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px;"></cc1:CustTextBox>
                                        <cc1:CustDropDownList ID="ddlCORP_CountryStateCode" kind="select" runat="server" onclick="simOptionClick4IE('txtCORP_CountryStateCode');"
                                            Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                        </cc1:CustDropDownList>
                                    </div>
                                </td>
                            </tr>
                            <%--登記名稱--%>
                            <tr class="trOdd">
                                <%--中文登記名稱--%>
                                <td style="width: 15%;" align="right">
                                    <cc1:CustLabel ID="CustLabel21" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_121"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%;" colspan="3">
                                    <cc1:CustTextBox ID="txtREG_NAME_CH" runat="server" Width="240px" IsValEmpty="False"
                                        ValEmptyMsg="" MaxLength="19" onkeydown="entersubmit('btnSubmitByTaxno');"  onblur="changeFullType(this);"
                                        onpaste="paste();" BoxName="登記名稱" checktype="fulltype"  TabIndex="7"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <%--商店英文名稱--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel24" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_122"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%" colspan="3">
                                    <cc1:CustTextBox ID="txtREG_NAME_EN" runat="server" Width="240px" MaxLength="40"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" BoxName="商店英文名稱"  TabIndex="8"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <%--登記地址--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel49" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_024"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 85%" colspan="3">
                                    <cc1:CustTextBox ID="txtREG_ZIP_CODE2" runat="server" Width="130px" MaxLength="7"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" 
                                        BoxName="登記郵遞區號" checktype="fulltype" TabIndex="9"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtREG_CITY" runat="server" Width="130px" MaxLength="6"
                                        onkeydown="entersubmit('btnSubmitByTaxno');"  onblur="changeFullType(this);" onpaste="paste();"
                                        BoxName="商店登記地址一" checktype="fulltype"  TabIndex="9" AutoPostBack="true" OnTextChanged="TextBox_AddrChanged2"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtREG_ADDR1" runat="server" Width="280px" MaxLength="14"
                                        onkeydown="entersubmit('btnSubmitByTaxno');"  onblur="changeFullType(this);" onpaste="paste();"
                                        BoxName="商店登記地址二" checktype="fulltype"  TabIndex="10"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtREG_ADDR2" runat="server" Width="140px" MaxLength="7"
                                        onkeydown="entersubmit('btnSubmitByTaxno');"  onblur="changeFullType(this);" onpaste="paste();"
                                        BoxName="商店登記地址三" checktype="fulltype"  TabIndex="11"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="trEven">
                                <%--登記電話--%>
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="CustLabel45" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_123"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%" colspan="3">
                                    <cc1:CustTextBox ID="txtCORP_TEL1" runat="server" Width="36px" IsValEmpty="False"
                                        ValEmptyMsg="" MaxLength="3" onkeydown="entersubmit('btnSubmitByTaxno');" checktype="num"
                                        BoxName="登記電話一"  TabIndex="12"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtCORP_TEL2" runat="server" Width="110px" MaxLength="8"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" checktype="num" BoxName="登記電話二"  TabIndex="13"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtCORP_TEL3" runat="server" Width="50px" MaxLength="5"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" checktype="num" BoxName="登記電話三"  TabIndex="14"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <%--負責人--%>
                            <tr class="trOdd">
                                <td rowspan="11" align="right" style="width: 7%">
                                    <cc1:CustLabel ID="lblShopData" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030100_108" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <%--負責人--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel25" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030100_012" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%" colspan="3">
                                    <cc1:CustTextBox ID="txtPrincipalNameCH" runat="server" Width="130px" MaxLength="4" onkeydown="entersubmit('btnSubmitByTaxno');"
                                         onblur="changeFullType(this);" onpaste="paste();" BoxName="負責人" checktype="fulltype"  TabIndex="15"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="trEven">
                                <%--中文長姓名--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel27" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030100_102" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustCheckBox ID="chkisLongName2" runat="server" AutoPostBack="True" BoxName="長姓名" OnCheckedChanged="CheckBox_CheckedChanged" />
                                    <cc1:CustLabel ID="CustLabel28" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtPrincipalName_L" runat="server" MaxLength="50" checktype="fulltype" Width="260px"
                                        onkeydown="entersubmit('btnSubmitByTaxno');"  onblur="changeFullType(this);" BoxName="負責人長姓名"
                                        onpaste="paste();" AutoPostBack="true" OnTextChanged="TextBox_TextChanged"></cc1:CustTextBox>
                                </td>
                                <%--羅馬拼音--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel29" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtPrincipalName_PINYIN" runat="server" MaxLength="50" checktype="fulltype" Width="260px"
                                        onkeydown="entersubmit('btnSubmitByTaxno');"  onblur="changeFullType(this);" BoxName="負責人羅馬拼音"
                                        onpaste="paste();"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <%--負責人英文名--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel30" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_014"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%" colspan="3">
                                    <cc1:CustTextBox ID="txtPrincipalNameEN" runat="server" Width="240px" MaxLength="40"  TabIndex="16"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" BoxName="負責人英文名" ></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <%--負責人ID--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel23" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_013"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%" colspan="3">
                                    <cc1:CustTextBox ID="txtPrincipalIDNo" runat="server" Width="140px" MaxLength="10"  TabIndex="17"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" checktype="ID" BoxName="負責人ID" InputType="LetterAndInt"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <%--生日--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblBossBirthday" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_034"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtPrincipalBirth" runat="server" MaxLength="7" checktype="num"  TabIndex="18"
                                        Width="50px" onkeydown="entersubmit('btnSubmitByTaxno');" BoxName="生日"></cc1:CustTextBox>
                                </td>
                                <%--發證日期--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel46" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050100_026"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtPrincipalIssueDate" runat="server" MaxLength="7" checktype="num"  TabIndex="19"
                                        Width="50px" onkeydown="entersubmit('btnSubmitByTaxno');" BoxName="發證日期"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <%--發證地點--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel38" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050100_027" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtPrincipalIssuePlace" runat="server" MaxLength="3" checktype="fulltype"
                                        Width="40px" onkeydown="entersubmit('btnSubmitByTaxno');"  onblur="changeFullType(this);"
                                        BoxName="換證點" InputType="ChineseLanguage" onpaste="paste();"  TabIndex="20"></cc1:CustTextBox>
                                </td>
                                <%--領補換類別--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel39" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050100_028"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtReplaceType" runat="server" Width="40px" MaxLength="1"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" checktype="num" BoxName="換證點" ToolTip="1.初 2.補 3.換"  TabIndex="21"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <%--國籍--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel22" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_071" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%" colspan="3">
                                    <div style="position: relative">
                                        <cc1:CustTextBox ID="txtPrincipalCountryCode" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                            onkeydown="entersubmit('btnAdd');" BoxName="國籍"  TabIndex="22"
                                            Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px;" AutoPostBack="true" OnTextChanged="txtCodeType_TextChanged"></cc1:CustTextBox>
                                        <cc1:CustDropDownList ID="ddlPrincipalCountryCode" kind="select" runat="server" onclick="simOptionClick4IE('txtPrincipalCountryCode');"
                                            Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                        </cc1:CustDropDownList>
                                    </div>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <%--護照號碼--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel33" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030100_093" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtPrincipalPassportNo" runat="server" MaxLength="22" Width="200px"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" BoxName="護照號碼" InputType="LetterAndInt"  TabIndex="23"></cc1:CustTextBox>
                                </td>
                                <%--護照效期--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel34" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030100_100" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtPrincipalPassportExpdt" runat="server" MaxLength="8" Width="200px"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" BoxName="護照效期" InputType="LetterAndInt"  TabIndex="24"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <%--居留證號--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel35" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030100_094" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtPrincipalResidentNo" runat="server" MaxLength="22" Width="200px"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" BoxName="居留證號"  TabIndex="25"></cc1:CustTextBox>
                                </td>
                                <%--居留效期--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel36" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030100_101" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtPrincipalResidentExpdt" runat="server" MaxLength="8" Width="200px"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" BoxName="居留效期"  TabIndex="26"></cc1:CustTextBox>
                                </td>
                            </tr>

                            <%--負責人電話--%>
                            <tr class="trEven">
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel31" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_015"
                                        StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 35%" colspan="3">
                                    <cc1:CustTextBox ID="txtPrincipal_TEL1" runat="server" Width="40px" MaxLength="3"  TabIndex="27"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" checktype="num" BoxName="負責人電話一"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtPrincipal_TEL2" runat="server" Width="100px" MaxLength="8"  TabIndex="28"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" checktype="num" BoxName="負責人電話二"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtPrincipal_TEL3" runat="server" Width="40px" MaxLength="5"  TabIndex="29"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" checktype="num" BoxName="負責人電話三"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <%--負責人戶籍地址--%>
                            <tr class="trOdd">
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel37" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_016"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%" colspan="3">
                                    <cc1:CustTextBox ID="txtHouseholdCITY" runat="server" Width="130px" MaxLength="6"
                                        onkeydown="entersubmit('btnSubmitByTaxno');"  onblur="changeFullType(this);" onpaste="paste();"
                                        BoxName="負責人戶籍地址一" checktype="fulltype"  TabIndex="30"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtHouseholdADDR1" runat="server" Width="280px" MaxLength="14"
                                        onkeydown="entersubmit('btnSubmitByTaxno');"  onblur="changeFullType(this);" onpaste="paste();"
                                        BoxName="負責人戶籍地址二" checktype="fulltype"  TabIndex="31"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtHouseholdADDR2" runat="server" Width="140px" MaxLength="7"
                                        onkeydown="entersubmit('btnSubmitByTaxno');"  onblur="changeFullType(this);" onpaste="paste();"
                                        BoxName="負責人戶籍地址三" checktype="fulltype"  TabIndex="32"></cc1:CustTextBox></td>
                            </tr>
                            <tr class="trEven" runat="server" visible="false">
                                <td rowspan="5" align="right" style="width: 7%">
                                    <%--帳號資料--%>
                                    <cc1:CustLabel ID="CustLabel26" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030100_109" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <%--銀行名稱--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel47" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_110"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%" colspan="3">
                                    <cc1:CustTextBox ID="txtBANK_NAME" runat="server" Width="140px" MaxLength="8"  TabIndex="33"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" checktype="fulltype" BoxName="銀行名稱"  onblur="changeFullType(this);" onpaste="paste();"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd" runat="server" visible="false">
                                <%--分行名稱--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel50" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_111"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%" colspan="3">
                                    <cc1:CustTextBox ID="txtBANK_BRANCH" runat="server" Width="140px" MaxLength="8"  TabIndex="34"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" checktype="fulltype" BoxName="分行名稱"  onblur="changeFullType(this);" onpaste="paste();"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven" runat="server" visible="false">
                                <%--戶名--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel48" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_112"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%" colspan="3">
                                    <cc1:CustTextBox ID="txtBANK_ACCT_NAME" runat="server" Width="140px" MaxLength="8"  TabIndex="35"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" checktype="fulltype" BoxName="戶名"  onblur="changeFullType(this);" onpaste="paste();"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd" runat="server" visible="false">
                                <%--檢碼--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel15" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_116"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%" colspan="3">
                                    <cc1:CustTextBox ID="txtCHECK_CODE" runat="server" Width="140px" MaxLength="4"  TabIndex="36"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" checktype="num" BoxName="檢碼"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven" runat="server" visible="false">
                                <%--帳號--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel52" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_114"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%" colspan="3">
                                    <cc1:CustTextBox ID="txtBANK_ACCT1" runat="server" Width="96px" MaxLength="3" onkeydown="entersubmit('btnSubmitByTaxno');"
                                        checktype="num" BoxName="帳號一"  TabIndex="37"></cc1:CustTextBox>&nbsp;
                                    <cc1:CustTextBox ID="txtBANK_ACCT2" runat="server" MaxLength="14" onkeydown="entersubmit('btnSubmitByTaxno');" checktype="num"
                                        BoxName="帳號二"  TabIndex="38"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td rowspan="1" align="right" style="width: 7%">
                                    <%--歸檔編號--%>
                                    <cc1:CustLabel ID="CustLabel11" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030100_124" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel13" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_125"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%" colspan="3">
                                    <cc1:CustTextBox ID="txtARCHIVE_NO" runat="server" Width="140px" MaxLength="9"
                                        onkeydown="entersubmit('btnSubmitByTaxno');" checktype="num" BoxName="歸檔編號"  TabIndex="39"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtDOC_ID" runat="server" Width="140px" MaxLength="14" BoxName="案件編號" Visible="false"></cc1:CustTextBox>
                                </td>
                            </tr>                            
                            <%--審核確認--%>
                            <tr class="trOdd">
                                <td>
                                    <cc1:CustLabel ID="CustLabel62" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050100_062" Style="color: Red"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td colspan="4">
                                    <cc1:CustCheckBox ID="chkCheckSum" runat="server"
                                        AutoPostBack="false" BoxName="以上資料確認無誤" TabIndex="40" />
                                    <cc1:CustLabel ID="CustLabel63" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050100_063"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <%--資料最後異動--%>
                            <tr class="trOdd">
                                <%--資料最後異動MAKER--%>
                                <td align="right" style="width: 18%; height: 33px;"colspan="2">
                                    <cc1:CustLabel ID="lbLAST_UPD_MAKER3" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_086"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 16%; height: 33px;">
                                    <cc1:CustTextBox ID="txtLAST_UPD_MAKER3" runat="server" MaxLength="9" checktype="numandletter"
                                        Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動MAKER" onfocus="allselect(this);" TabIndex="41"></cc1:CustTextBox>
                                </td>
                                <%--資料最後異動CHECKER--%>
                                <td align="right" style="width: 20%; height: 33px;" >
                                    <cc1:CustLabel ID="lbLAST_UPD_CHECKER3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_087" StickHeight="False"
                                        ></cc1:CustLabel>
                                </td>
                                <td style="width: 18%; height: 33px;" >
                                    <cc1:CustTextBox ID="txtLAST_UPD_CHECKER3" runat="server" MaxLength="9" checktype="numandletter"
                                        Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動CHECKER" onfocus="allselect(this);" TabIndex="42"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <%--資料最後異動分行--%>
                                <td align="right" style="width: 11%; height: 33px;" colspan="2">
                                    <cc1:CustLabel ID="lbLAST_UPD_BRANCH3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_085" StickHeight="False"
                                        ></cc1:CustLabel>
                                </td>
                                <td style="width: 16%; height: 33px;"colspan="3">
                                    <cc1:CustTextBox ID="txtLAST_UPD_BRANCH3" runat="server" MaxLength="4" Width="100px"
                                        onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動分行" checktype="numandletter" 
                                        onfocus="allselect(this);" TabIndex="43"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="itemTitle1">
                                <%--提交--%>
                                <td colspan="5" align="center">&nbsp;<cc1:CustButton ID="btnSubmitByTaxno" Text="" runat="server" Width="57px" CssClass="smallButton"
                                    Enabled="False" OnClick="btnSubmitByTaxno_Click" OnClientClick="return SubmitCheck('tabBasicByTaxno','6');"
                                    onkeydown="setfocus('btnBasicDataByTaxno');" DisabledWhenSubmit="False" ShowID="01_01030100_002" TabIndex="44" />
                                </td>
                            </tr>
                        </table>
                    </cc1:CustPanel>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <cc1:CustButton ID="btnHiden" OnClick="btnHiden_Click" runat="server" CssClass="btnHiden"
            DisabledWhenSubmit="False"></cc1:CustButton>
    </form>
</body>
</html>
