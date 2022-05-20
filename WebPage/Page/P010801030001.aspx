<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010801030001.aspx.cs" Inherits="Page_P010801030001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%--20210126_Ares_Stanley-調整版面; 移除法律形式, 註冊為美國者州別最大字元限制; 20210329_Ares_Stanley-調整半形轉全形失效; 20210408_Ares_Stanley-調整半形轉全形失效; 20210413_Ares_Stanley-版面調整; 20210415_Ares_Stanley-調整全半形轉換--%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript">
        function resetManager(sid) {

            document.getElementById('txtHCOP_BENE_NAME_' + sid).value = "";
            document.getElementById('txtHCOP_BENE_BIRTH_DATE_' + sid).value = "";
            document.getElementById('txtHCOP_BENE_NATION_' + sid).value = "";
            document.getElementById('txtHCOP_BENE_ID_' + sid).value = "";
            //document.getElementById('txtHCOP_BENE_PASSPORT_' + sid).value = "";
            //顯示欄位調整，清空也需要調整
            //document.getElementById('txtHCOP_BENE_PASSPORT_EXP_' + sid).value = "";
            //document.getElementById('txtHCOP_BENE_RESIDENT_NO_' + sid).value = "";
            //document.getElementById('txtHCOP_BENE_RESIDENT_EXP_' + sid).value = "";
            //document.getElementById('txtHCOP_BENE_OTH_CERT_' + sid).value = "";
            document.getElementById('chkHCOP_BENE_JOB_TYPE_' + sid).checked = false;
            document.getElementById('chkHCOP_BENE_JOB_TYPE_2_' + sid).checked = false;
            document.getElementById('chkHCOP_BENE_JOB_TYPE_3_' + sid).checked = false;
            document.getElementById('chkHCOP_BENE_JOB_TYPE_4_' + sid).checked = false;
            document.getElementById('chkHCOP_BENE_JOB_TYPE_5_' + sid).checked = false;
            document.getElementById('chkHCOP_BENE_JOB_TYPE_6_' + sid).checked = false;
            document.getElementById('chkIDType1_' + sid).checked = false;
            document.getElementById('chkIDType3_' + sid).checked = false;
            document.getElementById('chkIDType4_' + sid).checked = false;
            //2020.06.24 Ray Make 修改成chkIDType7_
            //document.getElementById('chkIDType5_' + sid).checked = false;
            document.getElementById('chkIDType7_' + sid).checked = false;
            

            document.getElementById('chkBENE_LNAM_FLAG_' + sid).checked = false;
            document.getElementById('txtHCOP_BENE_LNAME_' + sid).value = "";
            document.getElementById('txtHCOP_BENE_ROMA_' + sid).value = "";
            //增加反灰停用
            document.getElementById('txtHCOP_BENE_LNAME_' + sid).disabled = true;
            document.getElementById('txtHCOP_BENE_ROMA_' + sid).disabled = true;

        }

        /*設置延時取得焦點 預設延 500毫秒 */
        function setfocus(elmid) {
            setTimeout(function () {
                //your code to be executed after 500 minisecond
                document.getElementById(elmid).focus();
            }, 500);

        }
        function CheckLen(obj, objlen) {
            var strObj = obj.value;

            if (strObj != null && strObj != "undefined") {
                if (strObj.length > objlen) {
                    obj.style.backgroundColor = "red";
                }
                else {
                    obj.style.backgroundColor = "white";
                }
            }
        }

        //*客戶端檢核

        function checkInputText(id, intType) {
            try {
                ////*檢核輸入欄位身分證號碼是否為空
                //if (document.getElementById('txtUserId').value.Trim() == "") {
                //    alert('請輸入身分證號碼後,點選查詢按鈕');
                //    setControlsDisabled('pnlText');
                //    document.getElementById('txtUserId').focus();
                //    return false;
                //}

                if (!checkInputType(id)) {
                    return false;
                }

                if (intType == 1) {
                    //*檢核查詢部分欄位輸入規則
                    if (!checkInputType('tabNo1')) {
                        return false;
                    }

                    //*顯示確認提示框

                    if (!confirm('確定是否要異動資料？')) {
                        return false;
                    }
                }
                return true;
            } catch (e) {

            }
            return;
        }

        // 2020.04.28 Ray Add NameCheck 發電文等待畫面 
        // 顯示讀取遮罩
        function ShowProgressBar() {
            displayProgress();
            displayMaskFrame();
        }

        // 隱藏讀取遮罩
        function HideProgressBar() {
            var progress = $('#divProgress');
            var maskFrame = $("#divMaskFrame");
            progress.hide();
            maskFrame.hide();
        }
        // 顯示讀取畫面
        function displayProgress() {
            var w = $(document).width();
            var h = $(window).height();
            var progress = $('#divProgress');
            progress.css({ "z-index": 999999, "top": (h / 2) - (progress.height() / 2), "left": (w / 2) - (progress.width() / 2) });
            progress.show();
        }
        // 顯示遮罩畫面
        function displayMaskFrame() {
            var w = $(window).width();
            var h = $(document).height();
            var maskFrame = $("#divMaskFrame");
            maskFrame.css({ "z-index": 999998, "opacity": 0.7, "width": w, "height": h });
            maskFrame.show();
        }

    </script>
    <style type="text/css">
        .btnHiden {
            display: none;
        }

        .auto-style1 {
            width: 12%;
            height: 25px;
        }

        .auto-style2 {
            width: 13%;
            height: 25px;
        }
    </style>
</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <!--2020.04.28 Ray Add NameCheck 發電文等待畫面 -->
        <div id="divProgress" style="text-align: center; display: none; position: fixed; top: 50%; left: 50%;">
            <img alt="Please Wait..." src="../Common/images/Waiting.gif" />
            <br />
            <font color="#1B3563" size="4px">正在進行中，請稍後</font>
        </div>
        <div id="divMaskFrame" style="background-color: #F2F4F7; display: none; left: 0px; position: absolute; top: 0px;">
        </div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <cust:image runat="server" ID="image1" />
        &nbsp;
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <Triggers>
                <asp:PostBackTrigger ControlID="btnESB_NameCheckAdd" />
            </Triggers>
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                    <tr class="itemTitle">
                        <%--<td colspan="2">--%<%-->20200423--%>
                        <td>
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01080103_001"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                    <tr class="trOdd">
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblCardNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080103_040" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 12%">
                            <cc1:CustLabel ID="hlblCaseNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="CustLabel28" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080103_041" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 12%">
                            <cc1:CustLabel ID="hlblHCOP_HEADQUATERS_CORP_NO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="CustLabel29" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080103_042" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 12%">
                            <cc1:CustLabel ID="hlblHCOP_REG_NAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                    </tr>
                </table>
                <br />
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo2">
                        <tr class="itemTitle">
                            <td colspan="6"><%--<td colspan="8">20200423--%>
                                <li>
                                    <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01080103_002"></cc1:CustLabel>
                                </li>
                            </td>
                        </tr>
                        <%--公司基本資料 L1 --%>
                        <tr class="trOdd">
                            <td align="right" class="auto-style1">
                                <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_003" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td class="auto-style2">
                                <cc1:CustLabel ID="HQlblHCOP_STATUS" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" class="auto-style1">
                                <cc1:CustLabel ID="CustLabel11" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_006" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td class="auto-style2">
                                <cc1:CustTextBox ID="txtHCOP_BUILD_DATE" runat="server" MaxLength="8" checktype="num" Width="70px"
                                    BoxName="設立日期"></cc1:CustTextBox>
                            </td>
                            <td align="right" class="auto-style1">
                                <cc1:CustLabel ID="CustLabel17" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_015" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td class="auto-style2"><%--<td colspan="3" class="auto-style2">20200423--%>
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtHCOP_BUSINESS_ORGAN_TYPE" runat="server" Width="50px" onfocus="allselect(this);"
                                        BoxName="法律形式" OnTextChanged="txtHCOP_BUSINESS_ORGAN_TYPE_TextChanged" AutoPostBack="true"
                                        Style="left: 0px; top: 0px; position: relative; width: 250px; height: 11px; line-height: 11px;"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropSCCDOrganization" kind="select" runat="server" onclick="simOptionClick4IE('txtHCOP_BUSINESS_ORGAN_TYPE');"
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 240px); position: absolute; width: 260px;"
                                        AutoPostBack="true" OnSelectedIndexChanged="txtHCOP_BUSINESS_ORGAN_TYPE_TextChanged">
                                    </cc1:CustDropDownList>
                                </div>
                            </td>
                        </tr>
                        <%--公司基本資料 L2 --%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel10" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_008" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustTextBox ID="txtHCOP_REGISTER_NATION" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                    BoxName="註冊國籍" OnTextChanged="txtHCOP_REGISTER_NATION_TextChanged" AutoPostBack="true" Style="width: 50px; height: 14px;"></cc1:CustTextBox>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel15" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_009" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td width="25%" colspan="2">
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtHCOP_REGISTER_US_STATE" runat="server" Width="50px" onfocus="allselect(this);"
                                        BoxName="註冊國為美國者，請勾選州別"
                                        Style="left: 0px; top: 0px; position: absolute; width: 90px; height: 11px;"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropBasicCountryStateCode" kind="select" runat="server" onclick="simOptionClick4IE('txtHCOP_REGISTER_US_STATE');"
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 80px); position: relative; width: 135px;">
                                    </cc1:CustDropDownList>
                                </div>
                            </td>
                            <td width="26%"></td>
                        </tr>
                        <%--公司基本資料 L3 --%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_010" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustTextBox ID="txtAMLCC" runat="server"
                                    MaxLength="7" Width="60px" BoxName="行業編號" AutoPostBack="true" OnTextChanged="txtAMLCC_TextChanged"></cc1:CustTextBox>

                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel16" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_011" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3" style="width: 63%">
                                <cc1:CustLabel ID="lblCCName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>

                        </tr>
                        <%--公司基本資料 L4 --%>

                        <%--公司基本資料 L5 --%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel18" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_013" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="5" style="width: 88%">
                                <cc1:CustTextBox ID="txtHCOP_MAILING_CITY" runat="server" checktype="fulltype" MaxLength="6"
                                    Width="100px"  onblur="changeFullType(this);" onkeyup="CheckLen(this,12);"
                                    BoxName="通訊地址_1" onpaste="paste();"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtHCOP_MAILING_ADDR1" runat="server" checktype="fulltype" MaxLength="14"
                                    Width="300px"  onblur="changeFullType(this);" onkeyup="CheckLen(this,15);"
                                    BoxName="通訊地址_2" onpaste="paste();"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtHCOP_MAILING_ADDR2" runat="server" checktype="fulltype" MaxLength="14"
                                    Width="260px"  onblur="changeFullType(this);" onkeyup="CheckLen(this,14);"
                                    BoxName="通訊地址_3" onpaste="paste();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--公司基本資料 L6 --%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel86" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_024" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 38%" colspan="3">
                                <cc1:CustTextBox ID="txtHCOP_COMP_TEL" runat="server" MaxLength="20" Width="80px" onfocus="allselect(this);"
                                    BoxName="公司電話" Style="width: 120px; height: 14px;"></cc1:CustTextBox>
                            </td>
                            <%--20191224-原法律形式的位置--%>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel20" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_014" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 38%">
                                <cc1:CustTextBox ID="txtHCOP_EMAIL" runat="server" MaxLength="48" Width="200px"
                                    BoxName="E-MAIL"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--公司基本資料 L7 --%>
                        <tr class="trOdd">

                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel21" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_016" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <%--    <cc1:CustLabel ID="HQlblHCOP_COMPLEX_STR_CODE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>--%>
                                <cc1:CustRadioButton ID="radHCOP_COMPLEX_STR_CODEY" runat="server" AutoPostBack="False" GroupName="HCOP_COMPLEX_STR_CODE" Text="是" Checked="true" />
                                <cc1:CustRadioButton ID="radHCOP_COMPLEX_STR_CODEN" runat="server" AutoPostBack="False" GroupName="HCOP_COMPLEX_STR_CODE" Text="否" />
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel23" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_017" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <%--      <cc1:CustLabel ID="HQlblHCOP_ALLOW_ISSUE_STOCK_FLAG" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>--%>
                                <cc1:CustRadioButton ID="radHCOP_ALLOW_ISSUE_STOCK_FLAGY" runat="server" AutoPostBack="False" GroupName="HCOP_ALLOW_ISSUE_STOCK_FLAG" Text="是" Checked="true" />
                                <cc1:CustRadioButton ID="radHCOP_ALLOW_ISSUE_STOCK_FLAGN" runat="server" AutoPostBack="False" GroupName="HCOP_ALLOW_ISSUE_STOCK_FLAG" Text="否" />
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel25" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_018" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustRadioButton ID="radHCOP_ISSUE_STOCK_FLAGY" runat="server" AutoPostBack="False" GroupName="HCOP_ISSUE_STOCK_FLAG" Text="是" Checked="true" />
                                <cc1:CustRadioButton ID="radHCOP_ISSUE_STOCK_FLAGN" runat="server" AutoPostBack="False" GroupName="HCOP_ISSUE_STOCK_FLAG" Text="否" />
                            </td>
                        </tr>
                        <%--公司基本資料 L8 --%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel19" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_019" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustRadioButton ID="radHCOP_OVERSEAS_FOREIGNY" runat="server" AutoPostBack="true" GroupName="HCOP_OVERSEAS_FOREIGN" Text="是" OnCheckedChanged="radHCOP_OVERSEAS_FOREIGNY_CheckedChanged" />
                                <cc1:CustRadioButton ID="radHCOP_OVERSEAS_FOREIGNN" runat="server" AutoPostBack="true" GroupName="HCOP_OVERSEAS_FOREIGN" Text="否" OnCheckedChanged="radHCOP_OVERSEAS_FOREIGNY_CheckedChanged" />
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel24" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_020" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustTextBox ID="txtHCOP_OVERSEAS_FOREIGN_COUNTRY" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                    BoxName="母公司國別" Style="width: 50px; height: 14px;"
                                    OnTextChanged="txtHCOP_OVERSEAS_FOREIGN_COUNTRY_TextChanged" AutoPostBack="true"></cc1:CustTextBox>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel27" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_021" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 38%">
                                <cc1:CustTextBox ID="txtHCOP_PRIMARY_BUSI_COUNTRY" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                    BoxName="主要營業處所國別" Style="width: 50px; height: 14px;"
                                    OnTextChanged="txtHCOP_PRIMARY_BUSI_COUNTRY_TextChanged" AutoPostBack="true"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="itemTitle">
                            <td colspan="6">
                                <li>
                                    <cc1:CustLabel ID="CustLabel32" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01080103_022"></cc1:CustLabel>
                                </li>
                            </td>
                        </tr>
                        <%--公司基本資料 L12 --%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel37" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_023" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustTextBox ID="txtHCOP_CONTACT_NAME" runat="server" MaxLength="20" Width="50px" onfocus="allselect(this);"
                                    BoxName="聯絡人姓名" Style="width: 50px; height: 14px;"></cc1:CustTextBox>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel41" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_059" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustRadioButton ID="radHCOP_CONTACT_LNAMEY" runat="server" AutoPostBack="true" GroupName="LNAMEoption" Text="是" OnCheckedChanged="radHCOP_CONTACT_CheckedChanged" />
                                <cc1:CustRadioButton ID="radHCOP_CONTACT_LNAMEN" runat="server" AutoPostBack="true" GroupName="LNAMEoption" Text="否" OnCheckedChanged="radHCOP_CONTACT_CheckedChanged" />
                            </td>
                            <td align="right" style="width: 12%"><%--聯絡電話--%>
                                <cc1:CustLabel ID="CustLabel9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_062" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--RQ-2019-030155-002--%>
                            <td style="width: 63%">
                                <cc1:CustTextBox ID="txtHCOP_MOBILE" runat="server" MaxLength="20" Width="80px" onfocus="allselect(this);"
                                    BoxName="聯絡電話" Style="width: 120px; height: 14px;"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%--公司基本資料 L13(長姓名) --%>
                        <tr class="trOdd" style="display: none;" runat="server" id="cmpLname_contact">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel36" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_059" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="5">
                                <cc1:CustTextBox ID="HQlblHCOP_CONTACT_LNAME" runat="server" MaxLength="50" onfocus="allselect(this);"
                                     onblur="changeFullType(this);" checktype="fulltype" BoxName="長姓名" Style="width: 700px; height: 14px;"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd" style="display: none;" runat="server" id="cmpRname_contact">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel59" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_060" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="5">
                                <cc1:CustTextBox ID="HQlblHCOP_CONTACT_ROMA" runat="server" MaxLength="50" onfocus="allselect(this);"
                                     onblur="changeFullType(this);" checktype="fulltype" BoxName="羅馬拼音" Style="width: 700px; height: 14px;"></cc1:CustTextBox>
                            </td>
                        </tr>
                    </table>
                    <%--SCDD情報 --%>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1">
                        <tr class="itemTitle">
                            <td colspan="2" align="center">
                                <div style="display: inline;">
                                    <cc1:CustLabel ID="CustLabel70" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01080105_004"></cc1:CustLabel>
                                    <%--NameCheck Ray 20200414--%>
                                    <div style="text-align: right; display: inline;">
                                        <cc1:CustButton ID="btnESB_NameCheck" runat="server" CssClass="smallButton" ShowID="01_01080101_086"
                                            DisabledWhenSubmit="False" Enabled="true" OnClick="btnESB_NameCheck_Click" />
                                        &nbsp;&nbsp;
                                         <cc1:CustButton ID="btnESB_NameCheck_Detail" runat="server" CssClass="smallButton" ShowID="01_01080101_088"
                                            DisabledWhenSubmit="False" Enabled="true" OnClick="btnESB_NameCheck_Detail_Click" />
                                        <%--NameCheck_Add Ray 20200422 btnESB_NameCheck confirm 後跑btnESB_NameCheckAdd 按鈕隱藏--%>
                                        <div style="display:none">
                                            <cc1:CustButton ID="btnESB_NameCheckAdd" runat="server" CssClass="smallButton" ShowID="01_01080101_087"
                                                DisabledWhenSubmit="False" Enabled="true" OnClick="btnESB_NameCheckAdd_Click" OnClientClick="ShowProgressBar();" />
                                        </div>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 40%" bgcolor="#FF9900">
                                <cc1:CustLabel ID="CustLabel66" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080105_005" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 60%" bgcolor="#FF9900">
                                <cc1:CustLabel ID="CustLabel67" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080105_006" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 40%">
                                <%--RQ-2018-015749-002-擴案件編號欄位至70 20191105 modify by Peggy--%>
                                <%--<cc1:CustTextBox ID="txtNameCheck_No" runat="server" MaxLength="50"--%>
                                    <cc1:CustTextBox ID="txtNameCheck_No" runat="server" MaxLength="70"
                                    Width="420px" BoxName="名單掃描案件編號"></cc1:CustTextBox>
                            </td>
                            <td style="width: 60%">
                                <cc1:CustDropDownList ID="dropNameCheck_Item" kind="select" runat="server"
                                    Style="width: 440px;">
                                </cc1:CustDropDownList>
                                <br>
                                <cc1:CustTextBox ID="txtNameCheck_Note" runat="server" MaxLength="400"
                                    Width="400px" BoxName="名單掃描案件編號Note"></cc1:CustTextBox>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <%--組織情報 --%>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo2">
                        <tr class="trOdd">
                            <td align="right" style="width: 20%">
                                <cc1:CustLabel ID="CustLabel22" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080105_007" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 80%" colspan="3">
                                <cc1:CustTextBox ID="txtBusinessForeignAddress" runat="server" MaxLength="72"
                                    Width="400px" BoxName="台灣以外主要之營業處所地址"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 20%">
                                <cc1:CustLabel ID="CustLabel30" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080105_008" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 80%" colspan="3">
                                <cc1:CustTextBox ID="txtRiskObject" runat="server" MaxLength="98"
                                    Width="300px" BoxName="中/高風險客戶交易往來對象"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--營業處所是否在高風險或制裁國家--%>
                            <td align="left" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel31" runat="server" CurAlign="right" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080105_011"
                                    StickHeight="False"></cc1:CustLabel>

                            </td>
                            <%--國家--%>
                            <td align="left" style="width: 10%">
                                <cc1:CustDropDownList ID="dropSCCDIsSanction" kind="select" runat="server" OnSelectedIndexChanged="dropSCCDIsSanction_SelectedIndexChanged" AutoPostBack="true"
                                    Style="width: 80px;">
                                </cc1:CustDropDownList>
                            </td>
                            <td style="width: 10%">
                                <cc1:CustLabel ID="CustLabel33" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080105_012"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td>1<cc1:CustTextBox ID="txtIsSanctionCountryCode1" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                BoxName="國家1" OnTextChanged="txtIsSanctionCountryCode1_TextChanged" AutoPostBack="true"
                                Style="width: 50px; height: 14px;"></cc1:CustTextBox>
                                &nbsp;&nbsp;
                                     2<cc1:CustTextBox ID="txtIsSanctionCountryCode2" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                         BoxName="國家2" OnTextChanged="txtIsSanctionCountryCode1_TextChanged" AutoPostBack="true"
                                         Style="width: 50px; height: 14px;"></cc1:CustTextBox>
                                &nbsp;&nbsp;
                                      3<cc1:CustTextBox ID="txtIsSanctionCountryCode3" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                          BoxName="國家3" OnTextChanged="txtIsSanctionCountryCode1_TextChanged" AutoPostBack="true"
                                          Style="width: 50px; height: 14px;"></cc1:CustTextBox>
                                &nbsp;&nbsp;
                                         4<cc1:CustTextBox ID="txtIsSanctionCountryCode4" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                             BoxName="國家4" OnTextChanged="txtIsSanctionCountryCode1_TextChanged" AutoPostBack="true"
                                             Style="width: 50px; height: 14px;"></cc1:CustTextBox>
                                &nbsp;&nbsp;
                                         5<cc1:CustTextBox ID="txtIsSanctionCountryCode5" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);"
                                             BoxName="國家5" OnTextChanged="txtIsSanctionCountryCode1_TextChanged" AutoPostBack="true"
                                             Style="width: 50px; height: 14px;"></cc1:CustTextBox>
                                &nbsp;
                           
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 20%">
                                <cc1:CustLabel ID="CustLabel34" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080105_009" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 80%" colspan="3">
                                <cc1:CustDropDownList ID="dropOrganization_Item" kind="select" runat="server"
                                    Style="width: 380px;">
                                </cc1:CustDropDownList>
                                <br>
                                <cc1:CustTextBox ID="txtOrganization_Note" runat="server" MaxLength="80" Width="120px" BoxName="組織運作Note"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 20%">
                                <cc1:CustLabel ID="CustLabel35" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080105_010" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 80%" colspan="3">
                                <cc1:CustDropDownList ID="dropProof_Item" kind="select" runat="server"
                                    Style="width: 340px;">
                                </cc1:CustDropDownList>

                            </td>
                        </tr>
                    </table>

                    <%--公司負責人資料資料  --%>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo3" style="">
                        <tr class="itemTitle">
                            <td colspan="14">
                                <li>
                                    <cc1:CustLabel ID="CustLabel39" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01080103_025"></cc1:CustLabel>
                                </li>
                            </td>
                        </tr>
                        <%--公司負責人資料資料 HEADERLINE --%>
                        <tr class="trOdd">
                            <td align="center" style="width: 5%">
                                <cc1:CustLabel ID="CustLabel42" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_026" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 8%">
                                <cc1:CustLabel ID="CustLabel43" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_027" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 8%">
                                <cc1:CustLabel ID="CustLabel44" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_028" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel45" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_029" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 8%">
                                <cc1:CustLabel ID="CustLabel46" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_030" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel47" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_031" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel48" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_032" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel49" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_033" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--    <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel50" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_034" StickHeight="False"></cc1:CustLabel>
                            </td>--%>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel51" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_035" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel52" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_036" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel53" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_037" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel54" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_038" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel55" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_039" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel14" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_056" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--公司負責人資料資料 DataRow --%>
                        <tr class="trOdd">
                            <td align="left" style="width: 5%">
                                <cc1:CustLabel ID="HQlblHCOP_OWNER_CHINESE_NAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>

                            </td>
                            <td align="left" style="width: 8%">
                                <cc1:CustLabel ID="HQlblHCOP_OWNER_ENGLISH_NAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>

                            </td>
                            <td align="left" style="width: 8%">
                                <%--<cc1:CustLabel ID="HQlblHCOP_OWNER_BIRTH_DATE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>--%>
                                <%--RQ-2018-015749-002--%>
                                <cc1:CustTextBox ID="txtHCOP_OWNER_BIRTH_DATE" runat="server" MaxLength="8" Width="65px" onfocus="allselect(this);"  checktype="num"
                                    BoxName="出生日期(西元)" Style="width: 65px; height: 14px;"></cc1:CustTextBox>
                            </td>
                            <td align="left" style="width: 7%">
                                <cc1:CustLabel ID="HQlblHCOP_OWNER_ID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 8%">

                                <cc1:CustTextBox ID="txtHCOP_OWNER_ID_ISSUE_DATE" runat="server" MaxLength="8" onfocus="allselect(this);"
                                    BoxName="身分證發證日期" Style="width: 55px; height: 14px;"></cc1:CustTextBox>
                            </td>
                            <td align="left" style="width: 7%">
                                <cc1:CustTextBox ID="txtHCOP_OWNER_ID_ISSUE_PLACE" runat="server" MaxLength="20" Width="40px" BoxName="發證地點"></cc1:CustTextBox>
                            </td>
                            <td align="left" style="width: 7%">
                                <cc1:CustDropDownList ID="dropHCOP_OWNER_ID_REPLACE_TYPE" kind="select" runat="server"
                                    Style="width: 40px;">
                                </cc1:CustDropDownList>
                            </td>
                            <td align="left" style="width: 7%">
                                <cc1:CustRadioButton ID="radHasPhoto" runat="server" AutoPostBack="False" GroupName="Photo" Text="有" Checked="true" /><br />
                                <cc1:CustRadioButton ID="radNoPhoto" runat="server" AutoPostBack="False" GroupName="Photo" Text="無" />
                            </td>
                            <%--  <td align="left" style="width: 7%">
                                <cc1:CustTextBox ID="txtHCOP_OWNER_ID_Type" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustTextBox>
                            </td>--%>
                            <td align="left" style="width: 7%">

                                <cc1:CustTextBox ID="txtHCOP_OWNER_NATION" runat="server" MaxLength="2"
                                    Width="20px" BoxName="國籍" AutoPostBack="true" OnTextChanged="txtHCOP_OWNER_NATION_TextChanged"></cc1:CustTextBox>
                            </td>
                            <td align="left" style="width: 7%">
                                <cc1:CustLabel ID="HQlblHCOP_PASSPORT" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 7%">

                                <cc1:CustTextBox ID="txtHCOP_PASSPORT_EXP_DATE" runat="server" MaxLength="8" checktype="num"
                                    Width="70px" BoxName="護照效期"></cc1:CustTextBox>
                            </td>
                            <td align="left" style="width: 7%">
                                <cc1:CustLabel ID="HQlblHCOP_RESIDENT_NO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 7%">

                                <cc1:CustTextBox ID="txtHCOP_RESIDENT_EXP_DATE" runat="server" MaxLength="8" checktype="num"
                                    Width="70px" BoxName="居留證效期"></cc1:CustTextBox>
                            </td>
                            <td align="left" style="width: 7%">
                                <cc1:CustCheckBox ID="ChkOWNER_ID_SreachStatusY" runat="server" AutoPostBack="true" Text="適用" OnCheckedChanged="ChkOWNER_ID_SreachStatusY_CheckedChanged" />
                                <br>
                                <cc1:CustCheckBox ID="ChkOWNER_ID_SreachStatusN" runat="server" AutoPostBack="true" Text="不適用" OnCheckedChanged="ChkOWNER_ID_SreachStatusN_CheckedChanged" />

                            </td>
                        </tr>
                        <tr class="trOdd" style="display: none;" runat="server" id="cmpLname_0">
                            <td align="left" colspan="2">
                                <cc1:CustLabel ID="CustLabel57" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_059" StickHeight="False"></cc1:CustLabel>

                            </td>
                            <td align="left" style="width: 100%" colspan="12">
                                <cc1:CustLabel ID="HQlblHCOP_OWNER_CHINESE_LNAME" runat="server" CurAlign="Right" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd" style="display: none;" runat="server" id="cmpRname_0">
                            <td align="left" colspan="2">
                                <cc1:CustLabel ID="CustLabel62" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_060" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 100%" colspan="12">
                                <cc1:CustLabel ID="HQlblHCOP_OWNER_ROMA" runat="server" CurAlign="Right" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                    </table>
                    <%--高階管理人暨實質受益人資料明細表--%>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table1" style="">
                        <%--高階管理人暨實質受益人資料明細表--%>
                        <tr class="trEven">
                            <%--總公司基本資料--%>
                            <td align="center" colspan="7" class="auto-style1">
                                <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_051" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--姓    名--%>
                            <td align="center" style="width: 26%">
                                <cc1:CustLabel ID="CustLabel38" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_043" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--出生日期(西元)--%>
                            <td align="center" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_044" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--國籍--%>
                            <td align="center" style="width: 4%">
                                <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_046" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--身分證件類型--%>
                            <td align="center" style="width: 13%">
                                <cc1:CustLabel ID="CustLabel50" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_034" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--身分證件號碼--%>
                            <td align="center" style="width: 13%">
                                <cc1:CustLabel ID="CustLabel12" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050100_056" StickHeight="False"></cc1:CustLabel>
                            </td>                            
                            
                            <%--護照號碼--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustLabel ID="CustLabel26" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_047" StickHeight="False"></cc1:CustLabel>
                            </td>--%>
                            <%--護照效期--%>
                            <%-- <td align="center" style="width: 8%">
                                <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_048" StickHeight="False"></cc1:CustLabel>
                            </td>--%>
                            <%--居留證號碼--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_049" StickHeight="False"></cc1:CustLabel>
                            </td>--%>
                            <%--居留證效期--%>
                            <%-- <td align="center" style="width: 8%">
                                <cc1:CustLabel ID="CustLabel9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_050" StickHeight="False"></cc1:CustLabel>
                            </td>--%>
                            <%--其他證號--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_061" StickHeight="False"></cc1:CustLabel>
                            </td>--%>
                            <%--身分類型--%>
                            <td align="center" style="width: 27%">
                                <cc1:CustLabel ID="CustLabel13" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_051" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--清空--%>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel40" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_052" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--1--%><%--20200522-原系統：MaxLength="30"--%>
                            <td align="center" style="width: 26%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NAME_1" runat="server" MaxLength="40" Width="390px"
                                    BoxName="姓名"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblEnNotice_1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel>
                                <div>
                                    <cc1:CustCheckBox ID="chkBENE_LNAM_FLAG_1" runat="server" AutoPostBack="true" OnCheckedChanged="chkBENE_LNAM_FLAG_CheckedChanged" CssClass="tooglelnameCk" Text="長姓名" />
                                </div>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_BIRTH_DATE_1" runat="server" MaxLength="8" checktype="num" Width="65px"
                                    BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 4%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NATION_1" runat="server" MaxLength="2" Width="25px"
                                    BoxName="國籍" OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 13%"><%--身分證件類型--%>
                                <cc1:CustCheckBox ID="chkIDType1_1" runat="server" BoxName="1身分證" Text="1身分證" />
                                <cc1:CustCheckBox ID="chkIDType3_1" runat="server" BoxName="3護照" Text="3護照" /><br />
                                <cc1:CustCheckBox ID="chkIDType4_1" runat="server" BoxName="4統一證號" Text="4統一證號" />
                                <cc1:CustCheckBox ID="chkIDType7_1" runat="server" BoxName="7其他" Text="7其他" />
                            </td>
                            <td align="center" style="width: 13%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ID_1" runat="server" MaxLength="22" Width="100px"
                                    BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_1" runat="server" MaxLength="22" Width="100px"
                                    BoxName="護照號碼"></cc1:CustTextBox>
                            </td>--%>
                            <%-- <td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_EXP_1" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="護照效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_NO_1" runat="server" MaxLength="22" Width="100px"
                                    BoxName="居留證號碼"></cc1:CustTextBox>
                            </td>--%>
                            <%--  <td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_EXP_1" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="居留證效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_OTH_CERT_1" runat="server" MaxLength="22" Width="100px"
                                    BoxName="其他證件號碼"></cc1:CustTextBox>
                            </td>--%>
                            <td align="center" style="width: 27%">
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_1" runat="server"
                                    AutoPostBack="False" BoxName="1董/理事、監事/監察人，" Text="1董/理事、監事/監察人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_2_1" runat="server"
                                    AutoPostBack="False" BoxName="2總經理，" Text="2總經理，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_3_1" runat="server"
                                    AutoPostBack="False" BoxName="3財務長，" Text="3財務長，" /><br />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_4_1" runat="server"
                                    AutoPostBack="False" BoxName="4有權簽章人，" Text="4有權簽章人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_5_1" runat="server"
                                    AutoPostBack="False" BoxName="5合夥人、實質受益人，" Text="5合夥人、實質受益人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_6_1" runat="server"
                                    AutoPostBack="False" BoxName="6其他關聯人" Text="6其他關聯人" />
                            </td>
                            <td align="center" style="width: 7%">
                                <button value="清空" onclick="return resetManager('1');">清空</button>
                            </td>
                        </tr>
                        <%--1 長姓名區--%>
                        <tr class="trEven" style="display: none;" runat="server" id="cmpLname_1">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel56" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_059" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_LNAME_1" runat="server" MaxLength="50" Width="700px"
                                    BoxName="長姓名"  onblur="changeFullType(this);" checktype="fulltype"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven" style="display: none;" runat="server" id="cmpRname_1">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel60" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_060" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ROMA_1" runat="server" MaxLength="50" checktype="fulltype" Width="700px"
                                    BoxName="羅馬拼音"  onblur="changeFullType(this);"></cc1:CustTextBox>
                            </td>
                        </tr>

                        <tr class="trOdd">
                            <%--2--%>
                            <td align="center" style="width: 26%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NAME_2" runat="server" MaxLength="40" Width="390px"
                                    BoxName="姓名"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblEnNotice_2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel>
                                <div>
                                    <cc1:CustCheckBox ID="chkBENE_LNAM_FLAG_2" runat="server" AutoPostBack="true" OnCheckedChanged="chkBENE_LNAM_FLAG_CheckedChanged" CssClass="tooglelnameCk" Text="長姓名" />
                                </div>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_BIRTH_DATE_2" runat="server" MaxLength="8" checktype="num" Width="65px"
                                    BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 4%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NATION_2" runat="server" MaxLength="2" Width="25px"
                                    BoxName="國籍" OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 13%"><%--身分證件類型--%>
                                <cc1:CustCheckBox ID="chkIDType1_2" runat="server" BoxName="1身分證" Text="1身分證" />
                                <cc1:CustCheckBox ID="chkIDType3_2" runat="server" BoxName="3護照" Text="3護照" /><br />
                                <cc1:CustCheckBox ID="chkIDType4_2" runat="server" BoxName="4統一證號" Text="4統一證號" />
                                <cc1:CustCheckBox ID="chkIDType7_2" runat="server" BoxName="7其他" Text="7其他" />
                            </td>
                            <td align="center" style="width: 13%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ID_2" runat="server" MaxLength="22" Width="100px"
                                    BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>                            
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_2" runat="server" MaxLength="22" Width="100px"
                                    BoxName="護照號碼"></cc1:CustTextBox>
                            </td>--%>
                            <%-- <td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_EXP_2" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="護照效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_NO_2" runat="server" MaxLength="22" Width="100px"
                                    BoxName="居留證號碼"></cc1:CustTextBox>
                            </td>--%>
                            <%-- <td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_EXP_2" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="居留證效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_OTH_CERT_2" runat="server" MaxLength="22" Width="100px"
                                    BoxName="其他證件號碼"></cc1:CustTextBox>
                            </td>--%>
                            <td align="center" style="width: 27%">
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_2" runat="server"
                                    AutoPostBack="False" BoxName="1董/理事、監事/監察人，" Text="1董/理事、監事/監察人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_2_2" runat="server"
                                    AutoPostBack="False" BoxName="2總經理，" Text="2總經理，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_3_2" runat="server"
                                    AutoPostBack="False" BoxName="3財務長，" Text="3財務長，" /><br />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_4_2" runat="server"
                                    AutoPostBack="False" BoxName="4有權簽章人，" Text="4有權簽章人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_5_2" runat="server"
                                    AutoPostBack="False" BoxName="5合夥人、實質受益人，" Text="5合夥人、實質受益人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_6_2" runat="server"
                                    AutoPostBack="False" BoxName="6其他關聯人" Text="6其他關聯人" />
                            </td>
                            <td align="center" style="width: 7%">
                                <button value="清空" onclick="return resetManager('2');">清空</button>
                            </td>
                        </tr>
                        <%--長姓名區--%>
                        <tr class="trOdd" style="display: none" runat="server" id="cmpLname_2">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel84" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_059" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_LNAME_2" runat="server" MaxLength="50" Width="700px"
                                    BoxName="長姓名"  onblur="changeFullType(this);" checktype="fulltype"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd" style="display: none" runat="server" id="cmpRname_2">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel85" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_060" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ROMA_2" runat="server" MaxLength="50" checktype="fulltype" Width="700px"
                                    BoxName="羅馬拼音"  onblur="changeFullType(this);"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--3--%>
                            <td align="center" style="width: 26%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NAME_3" runat="server" MaxLength="40" Width="390px"
                                    BoxName="姓名"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblEnNotice_3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel>
                                <div>
                                    <cc1:CustCheckBox ID="chkBENE_LNAM_FLAG_3" runat="server" AutoPostBack="true" OnCheckedChanged="chkBENE_LNAM_FLAG_CheckedChanged" CssClass="tooglelnameCk" Text="長姓名" />
                                </div>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_BIRTH_DATE_3" runat="server" MaxLength="8" checktype="num" Width="65px"
                                    BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 4%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NATION_3" runat="server" MaxLength="2" Width="25px"
                                    BoxName="國籍" OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 13%"><%--身分證件類型--%>
                                <cc1:CustCheckBox ID="chkIDType1_3" runat="server" BoxName="1身分證" Text="1身分證" />
                                <cc1:CustCheckBox ID="chkIDType3_3" runat="server" BoxName="3護照" Text="3護照" /><br />
                                <cc1:CustCheckBox ID="chkIDType4_3" runat="server" BoxName="4統一證號" Text="4統一證號" />
                                <cc1:CustCheckBox ID="chkIDType7_3" runat="server" BoxName="7其他" Text="7其他" />
                            </td>
                            <td align="center" style="width: 13%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ID_3" runat="server" MaxLength="22" Width="100px"
                                    BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_3" runat="server" MaxLength="22" Width="100px"
                                    BoxName="護照號碼"></cc1:CustTextBox>
                            </td>--%>
                            <%-- <td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_EXP_3" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="護照效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_NO_3" runat="server" MaxLength="22" Width="100px"
                                    BoxName="居留證號碼"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_EXP_3" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="居留證效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_OTH_CERT_3" runat="server" MaxLength="22" Width="100px"
                                    BoxName="其他證件號碼"></cc1:CustTextBox>
                            </td>--%>
                            <td align="center" style="width: 27%">
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_3" runat="server"
                                    AutoPostBack="False" BoxName="1董/理事、監事/監察人，" Text="1董/理事、監事/監察人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_2_3" runat="server"
                                    AutoPostBack="False" BoxName="2總經理，" Text="2總經理，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_3_3" runat="server"
                                    AutoPostBack="False" BoxName="3財務長，" Text="3財務長，" /><br />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_4_3" runat="server"
                                    AutoPostBack="False" BoxName="4有權簽章人，" Text="4有權簽章人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_5_3" runat="server"
                                    AutoPostBack="False" BoxName="5合夥人、實質受益人，" Text="5合夥人、實質受益人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_6_3" runat="server"
                                    AutoPostBack="False" BoxName="6其他關聯人" Text="6其他關聯人" />
                            </td>
                            <td align="center" style="width: 7%">
                                <button value="清空" onclick="return resetManager('3');">清空</button>
                            </td>
                        </tr>
                        <%--3 長姓名區--%>
                        <tr class="trEven" style="display: none" runat="server" id="cmpLname_3">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel61" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_059" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_LNAME_3" runat="server" MaxLength="50" Width="700px"
                                    BoxName="長姓名"  onblur="changeFullType(this);" checktype="fulltype"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven" style="display: none" runat="server" id="cmpRname_3">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel58" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_060" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ROMA_3" runat="server" MaxLength="50" checktype="fulltype" Width="700px"
                                    BoxName="羅馬拼音"  onblur="changeFullType(this);"></cc1:CustTextBox>
                            </td>
                        </tr>

                        <tr class="trOdd">
                            <%--4--%>
                            <td align="center" style="width: 26%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NAME_4" runat="server" MaxLength="40" Width="390px"
                                    BoxName="姓名"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblEnNotice_4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel>
                                <div>
                                    <cc1:CustCheckBox ID="chkBENE_LNAM_FLAG_4" runat="server" AutoPostBack="true" OnCheckedChanged="chkBENE_LNAM_FLAG_CheckedChanged" CssClass="tooglelnameCk" Text="長姓名" />
                                </div>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_BIRTH_DATE_4" runat="server" MaxLength="8" checktype="num" Width="65px"
                                    BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 4%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NATION_4" runat="server" MaxLength="2" Width="25px"
                                    BoxName="國籍" OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 13%"><%--身分證件類型--%>
                                <cc1:CustCheckBox ID="chkIDType1_4" runat="server" BoxName="1身分證" Text="1身分證" />
                                <cc1:CustCheckBox ID="chkIDType3_4" runat="server" BoxName="3護照" Text="3護照" /><br />
                                <cc1:CustCheckBox ID="chkIDType4_4" runat="server" BoxName="4統一證號" Text="4統一證號" />
                                <cc1:CustCheckBox ID="chkIDType7_4" runat="server" BoxName="7其他" Text="7其他" />
                            </td>
                            <td align="center" style="width: 13%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ID_4" runat="server" MaxLength="22" Width="100px"
                                    BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>                            
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_4" runat="server" MaxLength="22" Width="100px"
                                    BoxName="護照號碼"></cc1:CustTextBox>
                            </td>--%>
                            <%--   <td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_EXP_4" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="護照效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_NO_4" runat="server" MaxLength="22" Width="100px"
                                    BoxName="居留證號碼"></cc1:CustTextBox>
                            </td>--%>
                            <%-- <td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_EXP_4" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="居留證效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_OTH_CERT_4" runat="server" MaxLength="22" Width="100px"
                                    BoxName="其他證件號碼"></cc1:CustTextBox>
                            </td>--%>
                            <td align="center" style="width: 27%">
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_4" runat="server"
                                    AutoPostBack="False" BoxName="1董/理事、監事/監察人，" Text="1董/理事、監事/監察人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_2_4" runat="server"
                                    AutoPostBack="False" BoxName="2總經理，" Text="2總經理，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_3_4" runat="server"
                                    AutoPostBack="False" BoxName="3財務長，" Text="3財務長，" /><br />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_4_4" runat="server"
                                    AutoPostBack="False" BoxName="4有權簽章人，" Text="4有權簽章人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_5_4" runat="server"
                                    AutoPostBack="False" BoxName="5合夥人、實質受益人，" Text="5合夥人、實質受益人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_6_4" runat="server"
                                    AutoPostBack="False" BoxName="6其他關聯人" Text="6其他關聯人" />
                            </td>
                            <td align="center" style="width: 7%">
                                <button value="清空" onclick="return resetManager('4');">清空</button>
                            </td>
                        </tr>
                        <%--長姓名區--%>
                        <tr class="trOdd" style="display: none" runat="server" id="cmpLname_4">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel63" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_059" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_LNAME_4" runat="server" MaxLength="50" Width="700px"
                                    BoxName="長姓名"  onblur="changeFullType(this);" checktype="fulltype"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd" style="display: none" runat="server" id="cmpRname_4">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel64" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_060" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ROMA_4" runat="server" MaxLength="50" checktype="fulltype" Width="700px"
                                    BoxName="羅馬拼音"  onblur="changeFullType(this);"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--5--%>
                            <td align="center" style="width: 26%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NAME_5" runat="server" MaxLength="40" Width="390px"
                                    BoxName="姓名"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblEnNotice_5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel>
                                <div>
                                    <cc1:CustCheckBox ID="chkBENE_LNAM_FLAG_5" runat="server" AutoPostBack="true" OnCheckedChanged="chkBENE_LNAM_FLAG_CheckedChanged" CssClass="tooglelnameCk" Text="長姓名" />
                                </div>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_BIRTH_DATE_5" runat="server" MaxLength="8" checktype="num" Width="65px"
                                    BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 4%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NATION_5" runat="server" MaxLength="2" Width="25px"
                                    BoxName="國籍" OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 13%"><%--身分證件類型--%>
                                <cc1:CustCheckBox ID="chkIDType1_5" runat="server" BoxName="1身分證" Text="1身分證" />
                                <cc1:CustCheckBox ID="chkIDType3_5" runat="server" BoxName="3護照" Text="3護照" /><br />
                                <cc1:CustCheckBox ID="chkIDType4_5" runat="server" BoxName="4統一證號" Text="4統一證號" />
                                <cc1:CustCheckBox ID="chkIDType7_5" runat="server" BoxName="7其他" Text="7其他" />
                            </td>
                            <td align="center" style="width: 13%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ID_5" runat="server" MaxLength="22" Width="100px"
                                    BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_5" runat="server" MaxLength="22" Width="100px"
                                    BoxName="護照號碼"></cc1:CustTextBox>
                            </td>--%>
                            <%--  <td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_EXP_5" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="護照效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_NO_5" runat="server" MaxLength="22" Width="100px"
                                    BoxName="居留證號碼"></cc1:CustTextBox>
                            </td>--%>
                            <%-- <td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_EXP_5" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="居留證效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_OTH_CERT_5" runat="server" MaxLength="22" Width="100px"
                                    BoxName="其他證件號碼"></cc1:CustTextBox>
                            </td>--%>
                            <td align="center" style="width: 27%">
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_5" runat="server"
                                    AutoPostBack="False" BoxName="1董/理事、監事/監察人，" Text="1董/理事、監事/監察人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_2_5" runat="server"
                                    AutoPostBack="False" BoxName="2總經理，" Text="2總經理，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_3_5" runat="server"
                                    AutoPostBack="False" BoxName="3財務長，" Text="3財務長，" /><br />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_4_5" runat="server"
                                    AutoPostBack="False" BoxName="4有權簽章人，" Text="4有權簽章人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_5_5" runat="server"
                                    AutoPostBack="False" BoxName="5合夥人、實質受益人，" Text="5合夥人、實質受益人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_6_5" runat="server"
                                    AutoPostBack="False" BoxName="6其他關聯人" Text="6其他關聯人" />
                            </td>
                            <td align="center" style="width: 7%">
                                <button value="清空" onclick="return resetManager('5');">清空</button>
                            </td>
                        </tr>
                        <%--長姓名區--%>
                        <tr class="trEven" style="display: none" runat="server" id="cmpLname_5">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel65" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_059" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_LNAME_5" runat="server" MaxLength="50" Width="700px"
                                    BoxName="長姓名"  onblur="changeFullType(this);" checktype="fulltype"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven" style="display: none" runat="server" id="cmpRname_5">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel68" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_060" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ROMA_5" runat="server" MaxLength="50" checktype="fulltype" Width="700px"
                                    BoxName="羅馬拼音"  onblur="changeFullType(this);"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--6--%>
                            <td align="center" style="width: 26%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NAME_6" runat="server" MaxLength="40" Width="390px"
                                    BoxName="姓名"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblEnNotice_6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel>
                                <div>
                                    <cc1:CustCheckBox ID="chkBENE_LNAM_FLAG_6" runat="server" AutoPostBack="true" OnCheckedChanged="chkBENE_LNAM_FLAG_CheckedChanged" CssClass="tooglelnameCk" Text="長姓名" />
                                </div>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_BIRTH_DATE_6" runat="server" MaxLength="8" checktype="num" Width="65px"
                                    BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 4%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NATION_6" runat="server" MaxLength="2" Width="25px"
                                    BoxName="國籍" OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 13%"><%--身分證件類型--%>
                                <cc1:CustCheckBox ID="chkIDType1_6" runat="server" BoxName="1身分證" Text="1身分證" />
                                <cc1:CustCheckBox ID="chkIDType3_6" runat="server" BoxName="3護照" Text="3護照" /><br />
                                <cc1:CustCheckBox ID="chkIDType4_6" runat="server" BoxName="4統一證號" Text="4統一證號" />
                                <cc1:CustCheckBox ID="chkIDType7_6" runat="server" BoxName="7其他" Text="7其他" />
                            </td>
                            <td align="center" style="width: 13%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ID_6" runat="server" MaxLength="22" Width="100px"
                                    BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_6" runat="server" MaxLength="22" Width="100px"
                                    BoxName="護照號碼"></cc1:CustTextBox>
                            </td>--%>
                            <%-- <td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_EXP_6" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="護照效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_NO_6" runat="server" MaxLength="22" Width="100px"
                                    BoxName="居留證號碼"></cc1:CustTextBox>
                            </td>--%>
                            <%--  <td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_EXP_6" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="居留證效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_OTH_CERT_6" runat="server" MaxLength="22" Width="100px"
                                    BoxName="其他證件號碼"></cc1:CustTextBox>
                            </td>--%>
                            <td align="center" style="width: 27%">
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_6" runat="server"
                                    AutoPostBack="False" BoxName="1董/理事、監事/監察人，" Text="1董/理事、監事/監察人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_2_6" runat="server"
                                    AutoPostBack="False" BoxName="2總經理，" Text="2總經理，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_3_6" runat="server"
                                    AutoPostBack="False" BoxName="3財務長，" Text="3財務長，" /><br />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_4_6" runat="server"
                                    AutoPostBack="False" BoxName="4有權簽章人，" Text="4有權簽章人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_5_6" runat="server"
                                    AutoPostBack="False" BoxName="5合夥人、實質受益人，" Text="5合夥人、實質受益人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_6_6" runat="server"
                                    AutoPostBack="False" BoxName="6其他關聯人" Text="6其他關聯人" />
                            </td>
                            <td align="center" style="width: 7%">
                                <button value="清空" onclick="return resetManager('6');">清空</button>
                            </td>
                        </tr>
                        <%--長姓名區--%>
                        <tr class="trOdd" style="display: none" runat="server" id="cmpLname_6">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel69" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_059" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_LNAME_6" runat="server" MaxLength="50" Width="700px"
                                    BoxName="長姓名"  onblur="changeFullType(this);" checktype="fulltype"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd" style="display: none" runat="server" id="cmpRname_6">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel71" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_060" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ROMA_6" runat="server" MaxLength="50" checktype="fulltype" Width="700px"
                                    BoxName="羅馬拼音"  onblur="changeFullType(this);"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--7--%>
                            <td align="center" style="width: 26%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NAME_7" runat="server" MaxLength="40" Width="390px"
                                    BoxName="姓名"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblEnNotice_7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel>
                                <div>
                                    <cc1:CustCheckBox ID="chkBENE_LNAM_FLAG_7" runat="server" AutoPostBack="true" OnCheckedChanged="chkBENE_LNAM_FLAG_CheckedChanged" CssClass="tooglelnameCk" Text="長姓名" />
                                </div>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_BIRTH_DATE_7" runat="server" MaxLength="8" checktype="num" Width="65px"
                                    BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 4%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NATION_7" runat="server" MaxLength="2" Width="25px"
                                    BoxName="國籍" OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 13%"><%--身分證件類型--%>
                                <cc1:CustCheckBox ID="chkIDType1_7" runat="server" BoxName="1身分證" Text="1身分證" />
                                <cc1:CustCheckBox ID="chkIDType3_7" runat="server" BoxName="3護照" Text="3護照" /><br />
                                <cc1:CustCheckBox ID="chkIDType4_7" runat="server" BoxName="4統一證號" Text="4統一證號" />
                                <cc1:CustCheckBox ID="chkIDType7_7" runat="server" BoxName="7其他" Text="7其他" />
                            </td>
                            <td align="center" style="width: 13%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ID_7" runat="server" MaxLength="22" Width="100px"
                                    BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_7" runat="server" MaxLength="22" Width="100px"
                                    BoxName="護照號碼"></cc1:CustTextBox>
                            </td>--%>
                            <%-- <td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_EXP_7" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="護照效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_NO_7" runat="server" MaxLength="22" Width="100px"
                                    BoxName="居留證號碼"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_EXP_7" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="居留證效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_OTH_CERT_7" runat="server" MaxLength="22" Width="100px"
                                    BoxName="其他證件號碼"></cc1:CustTextBox>
                            </td>--%>
                            <td align="center" style="width: 27%">
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_7" runat="server"
                                    AutoPostBack="False" BoxName="1董/理事、監事/監察人，" Text="1董/理事、監事/監察人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_2_7" runat="server"
                                    AutoPostBack="False" BoxName="2總經理，" Text="2總經理，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_3_7" runat="server"
                                    AutoPostBack="False" BoxName="3財務長，" Text="3財務長，" /><br />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_4_7" runat="server"
                                    AutoPostBack="False" BoxName="4有權簽章人，" Text="4有權簽章人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_5_7" runat="server"
                                    AutoPostBack="False" BoxName="5合夥人、實質受益人，" Text="5合夥人、實質受益人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_6_7" runat="server"
                                    AutoPostBack="False" BoxName="6其他關聯人" Text="6其他關聯人" />
                            </td>
                            <td align="center" style="width: 7%">
                                <button value="清空" onclick="return resetManager('7');">清空</button>
                            </td>
                        </tr>
                        <%--長姓名區--%>
                        <tr class="trEven" style="display: none" runat="server" id="cmpLname_7">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel72" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_059" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_LNAME_7" runat="server" MaxLength="50" Width="700px"
                                    BoxName="長姓名"  onblur="changeFullType(this);" checktype="fulltype"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven" style="display: none" runat="server" id="cmpRname_7">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel73" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_060" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ROMA_7" runat="server" MaxLength="50" checktype="fulltype" Width="700px"
                                    BoxName="羅馬拼音"  onblur="changeFullType(this);"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--8--%>
                            <td align="center" style="width: 26%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NAME_8" runat="server" MaxLength="40" Width="390px"
                                    BoxName="姓名"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblEnNotice_8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel>
                                <div>
                                    <cc1:CustCheckBox ID="chkBENE_LNAM_FLAG_8" runat="server" AutoPostBack="true" OnCheckedChanged="chkBENE_LNAM_FLAG_CheckedChanged" CssClass="tooglelnameCk" Text="長姓名" />
                                </div>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_BIRTH_DATE_8" runat="server" MaxLength="8" checktype="num" Width="65px"
                                    BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 4%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NATION_8" runat="server" MaxLength="2" Width="25px"
                                    BoxName="國籍" OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 13%"><%--身分證件類型--%>
                                <cc1:CustCheckBox ID="chkIDType1_8" runat="server" BoxName="1身分證" Text="1身分證" />
                                <cc1:CustCheckBox ID="chkIDType3_8" runat="server" BoxName="3護照" Text="3護照" /><br />
                                <cc1:CustCheckBox ID="chkIDType4_8" runat="server" BoxName="4統一證號" Text="4統一證號" />
                                <cc1:CustCheckBox ID="chkIDType7_8" runat="server" BoxName="7其他" Text="7其他" />
                            </td>
                            <td align="center" style="width: 13%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ID_8" runat="server" MaxLength="22" Width="100px"
                                    BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_8" runat="server" MaxLength="22" Width="100px"
                                    BoxName="護照號碼"></cc1:CustTextBox>
                            </td>--%>
                            <%-- <td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_EXP_8" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="護照效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_NO_8" runat="server" MaxLength="22" Width="100px"
                                    BoxName="居留證號碼"></cc1:CustTextBox>
                            </td>--%>
                            <%-- <td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_EXP_8" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="居留證效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_OTH_CERT_8" runat="server" MaxLength="22" Width="100px"
                                    BoxName="其他證件號碼"></cc1:CustTextBox>
                            </td>--%>
                            <td align="center" style="width: 27%">
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_8" runat="server"
                                    AutoPostBack="False" BoxName="1董/理事、監事/監察人，" Text="1董/理事、監事/監察人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_2_8" runat="server"
                                    AutoPostBack="False" BoxName="2總經理，" Text="2總經理，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_3_8" runat="server"
                                    AutoPostBack="False" BoxName="3財務長，" Text="3財務長，" /><br />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_4_8" runat="server"
                                    AutoPostBack="False" BoxName="4有權簽章人，" Text="4有權簽章人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_5_8" runat="server"
                                    AutoPostBack="False" BoxName="5合夥人、實質受益人，" Text="5合夥人、實質受益人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_6_8" runat="server"
                                    AutoPostBack="False" BoxName="6其他關聯人" Text="6其他關聯人" />
                            </td>
                            <td align="center" style="width: 7%">
                                <button value="清空" onclick="return resetManager('8');">清空</button>
                            </td>
                        </tr>
                        <%--長姓名區--%>
                        <tr class="trOdd" style="display: none" runat="server" id="cmpLname_8">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel74" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_059" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_LNAME_8" runat="server" MaxLength="50" Width="700px"
                                    BoxName="長姓名"  onblur="changeFullType(this);" checktype="fulltype"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd" style="display: none" runat="server" id="cmpRname_8">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel75" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_060" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ROMA_8" runat="server" MaxLength="50" checktype="fulltype" Width="700px"
                                    BoxName="羅馬拼音"  onblur="changeFullType(this);"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--9--%>
                            <td align="center" style="width: 26%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NAME_9" runat="server" MaxLength="40" Width="390px"
                                    BoxName="姓名"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblEnNotice_9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel>
                                <div>
                                    <cc1:CustCheckBox ID="chkBENE_LNAM_FLAG_9" runat="server" AutoPostBack="true" OnCheckedChanged="chkBENE_LNAM_FLAG_CheckedChanged" CssClass="tooglelnameCk" Text="長姓名" />
                                </div>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_BIRTH_DATE_9" runat="server" MaxLength="8" checktype="num" Width="65px"
                                    BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 4%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NATION_9" runat="server" MaxLength="2" Width="25px"
                                    BoxName="國籍" OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 13%"><%--身分證件類型--%>
                                <cc1:CustCheckBox ID="chkIDType1_9" runat="server" BoxName="1身分證" Text="1身分證" />
                                <cc1:CustCheckBox ID="chkIDType3_9" runat="server" BoxName="3護照" Text="3護照" /><br />
                                <cc1:CustCheckBox ID="chkIDType4_9" runat="server" BoxName="4統一證號" Text="4統一證號" />
                                <cc1:CustCheckBox ID="chkIDType7_9" runat="server" BoxName="7其他" Text="7其他" />
                            </td>
                            <td align="center" style="width: 13%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ID_9" runat="server" MaxLength="22" Width="100px"
                                    BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_9" runat="server" MaxLength="22" Width="100px"
                                    BoxName="護照號碼"></cc1:CustTextBox>
                            </td>--%>
                            <%-- <td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_EXP_9" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="護照效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_NO_9" runat="server" MaxLength="22" Width="100px"
                                    BoxName="居留證號碼"></cc1:CustTextBox>
                            </td>--%>
                            <%-- <td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_EXP_9" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="居留證效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_OTH_CERT_9" runat="server" MaxLength="22"  Width="100px"
                                    BoxName="其他證件號碼"></cc1:CustTextBox>
                            </td>--%>
                            <td align="center" style="width: 27%">
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_9" runat="server"
                                    AutoPostBack="False" BoxName="1董/理事、監事/監察人，" Text="1董/理事、監事/監察人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_2_9" runat="server"
                                    AutoPostBack="False" BoxName="2總經理，" Text="2總經理，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_3_9" runat="server"
                                    AutoPostBack="False" BoxName="3財務長，" Text="3財務長，" /><br />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_4_9" runat="server"
                                    AutoPostBack="False" BoxName="4有權簽章人，" Text="4有權簽章人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_5_9" runat="server"
                                    AutoPostBack="False" BoxName="5合夥人、實質受益人，" Text="5合夥人、實質受益人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_6_9" runat="server"
                                    AutoPostBack="False" BoxName="6其他關聯人" Text="6其他關聯人" />
                            </td>
                            <td align="center" style="width: 7%">
                                <button value="清空" onclick="return resetManager('9');">清空</button>
                            </td>
                        </tr>
                        <%--長姓名區--%>
                        <tr class="trEven" style="display: none" runat="server" id="cmpLname_9">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel76" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_059" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_LNAME_9" runat="server" MaxLength="50" Width="700px"
                                    BoxName="長姓名"  onblur="changeFullType(this);" checktype="fulltype"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven" style="display: none" runat="server" id="cmpRname_9">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel77" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_060" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ROMA_9" runat="server" MaxLength="50" checktype="fulltype" Width="700px"
                                    BoxName="羅馬拼音"  onblur="changeFullType(this);"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--10--%>
                            <td align="center" style="width: 26%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NAME_10" runat="server" MaxLength="40" Width="390px"
                                    BoxName="姓名"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblEnNotice_10" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel>
                                <div>
                                    <cc1:CustCheckBox ID="chkBENE_LNAM_FLAG_10" runat="server" AutoPostBack="true" OnCheckedChanged="chkBENE_LNAM_FLAG_CheckedChanged" CssClass="tooglelnameCk" Text="長姓名" />
                                </div>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_BIRTH_DATE_10" runat="server" MaxLength="8" checktype="num" Width="65px"
                                    BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 4%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NATION_10" runat="server" MaxLength="2" Width="25px"
                                    BoxName="國籍" OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 13%"><%--身分證件類型--%>
                                <cc1:CustCheckBox ID="chkIDType1_10" runat="server" BoxName="1身分證" Text="1身分證" />
                                <cc1:CustCheckBox ID="chkIDType3_10" runat="server" BoxName="3護照" Text="3護照" /><br />
                                <cc1:CustCheckBox ID="chkIDType4_10" runat="server" BoxName="4統一證號" Text="4統一證號" />
                                <cc1:CustCheckBox ID="chkIDType7_10" runat="server" BoxName="7其他" Text="7其他" />
                            </td>
                            <td align="center" style="width: 13%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ID_10" runat="server" MaxLength="22" Width="100px"
                                    BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_10" runat="server" MaxLength="22" Width="100px"
                                    BoxName="護照號碼"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_EXP_10" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="護照效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_NO_10" runat="server" MaxLength="22" Width="100px"
                                    BoxName="居留證號碼"></cc1:CustTextBox>
                            </td>--%>
                           <%-- <td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_EXP_10" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="居留證效期"></cc1:CustTextBox>
                            </td>--%>
                                <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_OTH_CERT_10" runat="server" MaxLength="22" Width="100px"
                                    BoxName="其他證件號碼"></cc1:CustTextBox>
                            </td>--%>
                            <td align="center" style="width: 27%">
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_10" runat="server"
                                    AutoPostBack="False" BoxName="1董/理事、監事/監察人，" Text="1董/理事、監事/監察人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_2_10" runat="server"
                                    AutoPostBack="False" BoxName="2總經理，" Text="2總經理，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_3_10" runat="server"
                                    AutoPostBack="False" BoxName="3財務長，" Text="3財務長，" /><br />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_4_10" runat="server"
                                    AutoPostBack="False" BoxName="4有權簽章人，" Text="4有權簽章人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_5_10" runat="server"
                                    AutoPostBack="False" BoxName="5合夥人、實質受益人，" Text="5合夥人、實質受益人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_6_10" runat="server"
                                    AutoPostBack="False" BoxName="6其他關聯人" Text="6其他關聯人" />
                            </td>
                            <td align="center" style="width: 7%">
                                <button value="清空" onclick="return resetManager('10');">清空</button>
                            </td>
                        </tr>
                        <%--長姓名區--%>
                        <tr class="trOdd" style="display: none" runat="server" id="cmpLname_10">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel78" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_059" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_LNAME_10" runat="server" MaxLength="50" Width="700px"
                                    BoxName="長姓名"  onblur="changeFullType(this);" checktype="fulltype"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd" style="display: none" runat="server" id="cmpRname_10">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel79" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_060" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ROMA_10" runat="server" MaxLength="50" checktype="fulltype" Width="700px"
                                    BoxName="羅馬拼音"  onblur="changeFullType(this);"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--11--%>
                            <td align="center" style="width: 26%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NAME_11" runat="server" MaxLength="40" Width="390px"
                                    BoxName="姓名"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblEnNotice_11" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel>
                                <div>
                                    <cc1:CustCheckBox ID="chkBENE_LNAM_FLAG_11" runat="server" AutoPostBack="true" OnCheckedChanged="chkBENE_LNAM_FLAG_CheckedChanged" CssClass="tooglelnameCk" Text="長姓名" />
                                </div>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_BIRTH_DATE_11" runat="server" MaxLength="8" checktype="num" Width="65px"
                                    BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 4%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NATION_11" runat="server" MaxLength="2" Width="25px"
                                    BoxName="國籍" OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 13%"><%--身分證件類型--%>
                                <cc1:CustCheckBox ID="chkIDType1_11" runat="server" BoxName="1身分證" Text="1身分證" />
                                <cc1:CustCheckBox ID="chkIDType3_11" runat="server" BoxName="3護照" Text="3護照" /><br />
                                <cc1:CustCheckBox ID="chkIDType4_11" runat="server" BoxName="4統一證號" Text="4統一證號" />
                                <cc1:CustCheckBox ID="chkIDType7_11" runat="server" BoxName="7其他" Text="7其他" />
                            </td>
                            <td align="center" style="width: 13%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ID_11" runat="server" MaxLength="22" Width="100px"
                                    BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            <%--<td align="center" style="width: 12%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_11" runat="server" MaxLength="22" Width="100px"
                                    BoxName="護照號碼"></cc1:CustTextBox>
                            </td>--%>
                           <%-- <td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_EXP_11" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="護照效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_NO_11" runat="server" MaxLength="22" Width="100px"
                                    BoxName="居留證號碼"></cc1:CustTextBox>
                            </td>--%>
                           <%-- <td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_EXP_11" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="居留證效期"></cc1:CustTextBox>
                            </td>--%>
                                <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_OTH_CERT_11" runat="server" MaxLength="22" Width="100px"
                                    BoxName="其他證件號碼"></cc1:CustTextBox>
                            </td>--%>
                            <td align="center" style="width: 27%">
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_11" runat="server"
                                    AutoPostBack="False" BoxName="1董/理事、監事/監察人，" Text="1董/理事、監事/監察人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_2_11" runat="server"
                                    AutoPostBack="False" BoxName="2總經理，" Text="2總經理，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_3_11" runat="server"
                                    AutoPostBack="False" BoxName="3財務長，" Text="3財務長，" /><br />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_4_11" runat="server"
                                    AutoPostBack="False" BoxName="4有權簽章人，" Text="4有權簽章人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_5_11" runat="server"
                                    AutoPostBack="False" BoxName="5合夥人、實質受益人，" Text="5合夥人、實質受益人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_6_11" runat="server"
                                    AutoPostBack="False" BoxName="6其他關聯人" Text="6其他關聯人" />
                            </td>
                            <td align="center" style="width: 7%">
                                <button value="清空" onclick="return resetManager('11');">清空</button>
                            </td>
                        </tr>
                        <%--長姓名區--%>
                        <tr class="trEven" style="display: none" runat="server" id="cmpLname_11">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel80" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_059" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_LNAME_11" runat="server" MaxLength="50" Width="700px"
                                    BoxName="長姓名"  onblur="changeFullType(this);" checktype="fulltype"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven" style="display: none" runat="server" id="cmpRname_11">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel81" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_060" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ROMA_11" runat="server" MaxLength="50" checktype="fulltype" Width="700px"
                                    BoxName="羅馬拼音"  onblur="changeFullType(this);"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--12--%>
                            <td align="center" style="width: 26%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NAME_12" runat="server" MaxLength="40" Width="390px"
                                    BoxName="姓名"></cc1:CustTextBox><br />
                                <cc1:CustLabel ID="lblEnNotice_12" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" Visible="false" ForeColor="Red"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_057" StickHeight="False"></cc1:CustLabel>
                                <div>
                                    <cc1:CustCheckBox ID="chkBENE_LNAM_FLAG_12" runat="server" AutoPostBack="true" OnCheckedChanged="chkBENE_LNAM_FLAG_CheckedChanged" Text="長姓名" />
                                </div>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_BIRTH_DATE_12" runat="server" MaxLength="8" checktype="num" Width="65px"
                                    BoxName="出生日期(西元)"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 4%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_NATION_12" runat="server" MaxLength="2" Width="25px"
                                    BoxName="國籍" OnTextChanged="txtHCOP_BENE_NATION_TextChanged" AutoPostBack="true"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 13%"><%--身分證件類型--%>
                                <cc1:CustCheckBox ID="chkIDType1_12" runat="server" BoxName="1身分證" Text="1身分證" />
                                <cc1:CustCheckBox ID="chkIDType3_12" runat="server" BoxName="3護照" Text="3護照" /><br />
                                <cc1:CustCheckBox ID="chkIDType4_12" runat="server" BoxName="4統一證號" Text="4統一證號" />
                                <cc1:CustCheckBox ID="chkIDType7_12" runat="server" BoxName="7其他" Text="7其他" />
                            </td>
                            <td align="center" style="width: 13%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ID_12" runat="server" MaxLength="22" Width="100px"
                                    BoxName="身分證件號碼"></cc1:CustTextBox>
                            </td>
                            <%--<td align="center" style="width: 12%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_12" runat="server" MaxLength="22" Width="100px"
                                    BoxName="護照號碼"></cc1:CustTextBox>--%>
                            </td>
                           <%-- <td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_PASSPORT_EXP_12" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="護照效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_NO_12" runat="server" MaxLength="22" Width="100px"
                                    BoxName="居留證號碼"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_RESIDENT_EXP_12" runat="server" MaxLength="8" checktype="num" Width="60px"
                                    BoxName="居留證效期"></cc1:CustTextBox>
                            </td>--%>
                            <%--<td align="center" style="width: 8%">
                                <cc1:CustTextBox ID="txtHCOP_BENE_OTH_CERT_12" runat="server" MaxLength="22" Width="100px"
                                    BoxName="其他證件號碼"></cc1:CustTextBox>
                            </td>--%>
                            <td align="center" style="width: 27%">
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_12" runat="server"
                                    AutoPostBack="False" BoxName="1董/理事、監事/監察人，" Text="1董/理事、監事/監察人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_2_12" runat="server"
                                    AutoPostBack="False" BoxName="2總經理，" Text="2總經理，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_3_12" runat="server"
                                    AutoPostBack="False" BoxName="3財務長，" Text="3財務長，" /><br />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_4_12" runat="server"
                                    AutoPostBack="False" BoxName="4有權簽章人，" Text="4有權簽章人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_5_12" runat="server"
                                    AutoPostBack="False" BoxName="5合夥人、實質受益人，" Text="5合夥人、實質受益人，" />
                                <cc1:CustCheckBox ID="chkHCOP_BENE_JOB_TYPE_6_12" runat="server"
                                    AutoPostBack="False" BoxName="6其他關聯人" Text="6其他關聯人" />
                            </td>
                            <td align="center" style="width: 7%">
                                <button value="清空" onclick="return resetManager('12');">清空</button>
                            </td>
                        </tr>
                        <%--長姓名區--%>
                        <tr class="trOdd" style="display: none" runat="server" id="cmpLname_12">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel82" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_059" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_LNAME_12" runat="server" MaxLength="50" Width="700px"
                                    BoxName="長姓名"  onblur="changeFullType(this);" checktype="fulltype"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd" style="display: none" runat="server" id="cmpRname_12">
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel83" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_060" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="6">
                                <cc1:CustTextBox ID="txtHCOP_BENE_ROMA_12" runat="server" MaxLength="50" checktype="fulltype" Width="700px"
                                    BoxName="羅馬拼音"  onblur="changeFullType(this);"></cc1:CustTextBox>
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo9" style="">
                        <tr class="itemTitle">
                            <td align="center">
                                <cc1:CustButton ID="btnAdd" runat="server" CssClass="smallButton" ShowID="01_01080103_053"
                                    OnClientClick="return checkInputText('pnlText', 1);" DisabledWhenSubmit="False"
                                    onkeydown="movefocus();" OnClick="btnAdd_Click" />
                                &nbsp;     &nbsp;     &nbsp;     &nbsp;
                                <cc1:CustButton ID="btnCancel" runat="server" CssClass="smallButton" ShowID="01_01080103_054"
                                    DisabledWhenSubmit="False"
                                    onkeydown="movefocus();" OnClick="btnCancel_Click" />
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
                    <cc1:CustHiddenField ID="hidHCOP_CORP_TYPE" runat="server" />
                    <cc1:CustHiddenField ID="hidHCOP_CORP_REG_ENG_NAME" runat="server" />
                    <cc1:CustHiddenField ID="hidHCOP_REG_NAME" runat="server" />
                    <cc1:CustHiddenField ID="hidHCOP_REG_CITY" runat="server" />
                    <cc1:CustHiddenField ID="hidHCOP_REG_ADDR1" runat="server" />
                    <cc1:CustHiddenField ID="hidHCOP_REG_ADDR2" runat="server" />
                    <cc1:CustHiddenField ID="hidHCOP_NP_COMPANY_NAME" runat="server" />
                    <cc1:CustHiddenField ID="hidHCOP_OWNER_CITY" runat="server" />
                    <cc1:CustHiddenField ID="hidHCOP_OWNER_ADDR1" runat="server" />
                    <cc1:CustHiddenField ID="hidHCOP_OWNER_ADDR2" runat="server" />
                    <cc1:CustHiddenField ID="hidHCOP_LAST_UPD_MALER" runat="server" />
                    <cc1:CustHiddenField ID="hidHCOP_LAST_UPD_CHECKER" runat="server" />
                    <cc1:CustHiddenField ID="hidHCOP_LAST_UPD_BRANCH" runat="server" />
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
