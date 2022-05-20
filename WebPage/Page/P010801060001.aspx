<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010801060001.aspx.cs" Inherits="P010801060001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%-- 修改紀錄:2021/01/25_Ares_Stanley-調整頁面資料避免按鈕失效, 調整版面; 2021/01/26_Ares_Stanley-調整版面 --%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript">

        //20191202-RQ-2018-015749-002 mark by Peggy
        //function HideOrShowTextBox(sender) {
        //    if (sender.checked) {
        //        document.getElementById("txtSR_RiskNote").style.display = '';
        //    }
        //    else {
        //        document.getElementById("txtSR_RiskNote").style.display = 'none';
        //        document.getElementById("txtSR_RiskNote").value = "";
        //    }
        //}

        window.onload = function () {
            document.getElementById('hidPersonnelHTML').value = escape(document.getElementById('hidPersonnelHTML').value);
        };

        function CBL_SingleChoice(sender) {
            var listObjGroup = document.getElementById("cbCalculatingRiskLevel");
            var listChxGroup = document.getElementsByTagName("input");
            var tempStatus = sender.checked;

            for (var i = 0; i < listChxGroup.length; i++) {
                if (listChxGroup[i].type == 'checkbox') {
                    //alert(listChxGroup[i].id + ':' + listChxGroup[i].id.indexOf("RiskLevel"));
                    if (listChxGroup[i].id.indexOf("RiskLevel") > 0)
                        listChxGroup[i].checked = false;

                    //20191204-RQ-2018-015749-002-計算風險等級或異常結案的結果為高風險時，不能自動將EDD已完成打勾 by Peggy
                    //20190614 (U) by Talas
                    //if (sender.id.indexOf("CalculatingRiskLevel") > 0) {
                    //    if ($("#" + sender.id).next("label").html() == "高風險") {
                    //        if (listChxGroup[i].id.indexOf("SR_EDD_Status") > 0)
                    //            listChxGroup[i].checked = true;
                    //    }
                    //    else {
                    //          listChxGroup[i].checked = false;
                    //    }
                    //}
                    
                }
            }
            sender.checked = tempStatus;
        }

    </script>

    <style type="text/css">
        .btnHiden {
            display: none;
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

                <%--                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="CaseInfo" style="">
                    <tr class="trOdd">
                        <td align="right" style="width: 10%">
                            <cc1:CustLabel ID="clblbAML_HQ_Work_CASE_NO" runat="server" CurAlign="right" ShowID="01_08010600_002" ></cc1:CustLabel>
                        </td>
                        
                        <td style="width: 20%">    
                            <asp:Label ID="lbAML_HQ_Work_CASE_NO" runat="server" Text=""></asp:Label>
                            <asp:HiddenField ID="hidAML_HQ_Work_CASE_NO" runat="server" />
                        </td>                        
                        <td align="right" style="width: 10%">   
                            <cc1:CustLabel ID="clbAML_HQ_Work_HCOP_HEADQUATERS_CORP_NO" runat="server" CurAlign="right" ShowID="01_08010600_003" ></cc1:CustLabel>
                        </td>
                        <td style="width: 20%">                                
                            <asp:Label ID="lbAML_HQ_Work_HCOP_HEADQUATERS_CORP_NO" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="right" style="width: 10%">  
                            <cc1:CustLabel ID="clbAML_HQ_Work_HCOP_REG_NAME" runat="server" CurAlign="right" ShowID="01_08010600_004" ></cc1:CustLabel>
                        </td>
                        <td style="width: 20%">                        
                            <asp:Label ID="lbAML_HQ_Work_HCOP_REG_NAME" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                </table>--%>

                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="table-layout:fixed">
                    <tr class="trOdd">
                        <td align="left" style="width: 185px">
                            <%--統一編號--%>
                            <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="right" ShowID="01_08010600_005"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 115px">
                            <asp:Label ID="lbAML_HQ_Work_HCOP_HEADQUATERS_CORP_NO" runat="server" Text=""></asp:Label>
                            <asp:HiddenField ID="hidAML_HQ_Work_CASE_NO" runat="server" />
                        </td>
                        <td align="left" style="width: 130px">
                            <%--登記名稱--%>
                            <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="right" ShowID="01_08010600_006"></cc1:CustLabel>
                        </td>
                        <td align="left" style="">
                            <asp:Label ID="lbAML_HQ_Work_HCOP_REG_NAME" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left" style="">
                            <%--登記名稱(英文)--%>
                            <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="right" ShowID="01_08010600_007"></cc1:CustLabel>
                        </td>
                        <td align="left" style="word-break:break-all">
                            <asp:Label ID="lbAML_HQ_Work_HCOP_CORP_REG_ENG_NAME" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left" style="">
                            <%--設立日期--%>
                            <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="right" ShowID="01_08010600_008"></cc1:CustLabel>
                        </td>
                        <td align="left" style="word-break:break-all">
                            <asp:Label ID="lbAML_HQ_Work_HCOP_BUILD_DATE" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="left">
                            <%--商店狀態--%>
                            <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="right" ShowID="01_08010600_009"></cc1:CustLabel>
                        </td>
                        <td align="left">
                            <asp:Label ID="lbAML_HQ_Work_HCOP_STATUS" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left" style="word-break:break-all">
                            <%--註冊國籍--%>
                            <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="right" ShowID="01_08010600_010"></cc1:CustLabel>
                        </td>
                        <td align="left">
                            <asp:Label ID="lbAML_HQ_Work_HCOP_REGISTER_NATION" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left">
                            <%--美國州別--%>
                            <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="right" ShowID="01_08010600_011"></cc1:CustLabel>
                        </td>
                        <td align="left" colspan="3">
                            <asp:Label ID="lbAML_HQ_Work_HCOP_REGISTER_US_STATE" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="left">
                            <%--行業別--%>
                            <cc1:CustLabel ID="CustLabel9" runat="server" CurAlign="right" ShowID="01_08010600_012"></cc1:CustLabel>
                        </td>
                        <td align="left">
                            <asp:Label ID="lbAML_HQ_Work_HCOP_CC" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left">
                            <%--行業別中文名稱--%>
                            <cc1:CustLabel ID="CustLabel10" runat="server" CurAlign="right" ShowID="01_08010600_013"></cc1:CustLabel>
                        </td>
                        <td align="left" colspan="5">
                            <asp:Label ID="lbAML_HQ_Work_HCOP_CC_Name" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="left">
                            <%--法律形式--%>
                            <cc1:CustLabel ID="CustLabel13" runat="server" CurAlign="right" ShowID="01_08010600_014"></cc1:CustLabel>
                        </td>
                        <td align="left">
                            <asp:Label ID="lbAML_HQ_Work_HCOP_BUSINESS_ORGAN_TYPE" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left">
                            <%--複雜股權結構--%>
                            <cc1:CustLabel ID="CustLabel14" runat="server" CurAlign="right" ShowID="01_08010600_015"></cc1:CustLabel>
                        </td>
                        <td align="left">
                            <asp:Label ID="lbAML_HQ_Work_HCOP_COMPLEX_STR_CODE" runat="server" Text=""></asp:Label>

                        </td>
                        <td align="left">
                            <%--是否可發行無記名股票--%>
                            <cc1:CustLabel ID="CustLabel15" runat="server" CurAlign="right" ShowID="01_08010600_016"></cc1:CustLabel>
                        </td>
                        <td align="left">
                            <asp:Label ID="lbAML_HQ_Work_HCOP_ALLOW_ISSUE_STOCK_FLAG" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left">
                            <%--是否已發行無記名股票--%>
                            <cc1:CustLabel ID="CustLabel16" runat="server" CurAlign="right" ShowID="01_08010600_017"></cc1:CustLabel>
                        </td>
                        <td align="left">
                            <asp:Label ID="lbAML_HQ_Work_HCOP_ISSUE_STOCK_FLAG" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="left">
                            <%--組織運作--%>
                            <cc1:CustLabel ID="CustLabel17" runat="server" CurAlign="right" ShowID="01_08010600_018"></cc1:CustLabel>
                        </td>
                        <td align="left" colspan="7">
                            <asp:Label ID="lbHQ_SCDD_Organization_Item" runat="server" Text=""></asp:Label>
                            <br />
                            <asp:Label ID="lbHQ_SCDD_Organization_Note" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="left">
                            <%--存在證明--%>
                            <cc1:CustLabel ID="CustLabel21" runat="server" CurAlign="right" ShowID="01_08010600_019"></cc1:CustLabel>
                        </td>
                        <td align="left" colspan="7">
                            <asp:Label ID="lbHQ_SCDD_Proof_Item" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="left">
                            <%--台灣以外主要之營業處所地址--%>
                            <cc1:CustLabel ID="CustLabel25" runat="server" CurAlign="right" ShowID="01_08010600_020"></cc1:CustLabel>
                        </td>
                        <td align="left" colspan="5">
                            <asp:Label ID="lbHQ_SCDD_BusinessForeignAddress" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left">
                            <%--主要之營業處所國別--%>
                            <cc1:CustLabel ID="CustLabel26" runat="server" CurAlign="right" ShowID="01_08010600_021"></cc1:CustLabel>
                        </td>
                        <td align="left">
                            <asp:Label ID="lbAML_HQ_Work_HCOP_PRIMARY_BUSI_COUNTRY" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="left">
                            <%--營業處所是否在高風險或制裁國家--%>
                            <cc1:CustLabel ID="CustLabel29" runat="server" CurAlign="right" ShowID="01_08010600_022"></cc1:CustLabel>
                        </td>
                        <td align="left" colspan="7">
                            <asp:Label ID="lbIsSanction" runat="server" Text=""></asp:Label>
                            <br />
                            <asp:Label ID="lbIsSanctionCountryCode" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="left">
                            <%--業務往來目的--%>
                            <cc1:CustLabel ID="CustLabel33" runat="server" CurAlign="right" ShowID="01_08010600_023"></cc1:CustLabel>
                        </td>
                        <td align="left" colspan="7">
                            <cc1:CustLabel ID="CustLabel34" runat="server" CurAlign="right" ShowID="01_08010600_054"></cc1:CustLabel>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="left">
                            <%--中/高風險客戶交易往來對象(請客戶提供主要客戶或供應商、往來金額前五大公司)--%>
                            <cc1:CustLabel ID="CustLabel37" runat="server" CurAlign="right" ShowID="01_08010600_024"></cc1:CustLabel>
                        </td>
                        <td align="left" colspan="7">
                            <cc1:CustLabel ID="CustLabel30" runat="server" CurAlign="right" ShowID="01_08010600_055"></cc1:CustLabel>
                            <br />
                            <asp:Label ID="lbHQ_SCDD_RiskObject" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="left">
                            <%--僑外資/外商--%>
                            <cc1:CustLabel ID="CustLabel38" runat="server" CurAlign="right" ShowID="01_08010600_025"></cc1:CustLabel>
                        </td>
                        <td align="left">
                            <asp:Label ID="lbAML_HQ_Work_HCOP_OVERSEAS_FOREIGN" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left">
                            <%--僑外資/外商母公司國別--%>
                            <cc1:CustLabel ID="CustLabel39" runat="server" CurAlign="right" ShowID="01_08010600_026"></cc1:CustLabel>
                        </td>
                        <td align="left" colspan="5">
                            <asp:Label ID="lbAML_HQ_Work_HCOP_OVERSEAS_FOREIGN_COUNTRY" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="left">
                            <%--警示帳戶或衍生警示帳戶--%>
                            <cc1:CustLabel ID="CustLabel45" runat="server" CurAlign="right" ShowID="01_08010600_027"></cc1:CustLabel>
                        </td>
                        <td align="left">
                            <asp:Label ID="lbAML_Cdata_Work_WarningFlag" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left">
                            <%--曾被申報過疑似洗錢Filed SAR註記--%>
                            <cc1:CustLabel ID="CustLabel46" runat="server" CurAlign="right" ShowID="01_08010600_028"></cc1:CustLabel>
                        </td>
                        <td align="left">
                            <asp:Label ID="lbAML_Cdata_Work_FiledSAR" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left">
                            <%--不合作/拒絕提供資訊--%>
                            <cc1:CustLabel ID="CustLabel47" runat="server" CurAlign="right" ShowID="01_08010600_029"></cc1:CustLabel>
                        </td>
                        <td align="left" colspan="3">
                            <asp:Label ID="lbAML_Cdata_Work_Incorporated" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="left" style="width: 10%">
                            <%--AML名單掃描結果-案件編號--%><%--20191107-RQ-2018-015749-002--%>
                            <%--<cc1:CustLabel ID="CustLabel49" runat="server" CurAlign="right" ShowID="01_08010600_030"></cc1:CustLabel>--%>
                            <cc1:CustLabel ID="CustLabel11" runat="server" CurAlign="right" ShowID="01_01080101_065"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 20%; word-break:break-all">
                            <%--AML名單掃描結果-案件編號--%>
                            <asp:Label ID="lblHQ_SCDD_NameCheck_No" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left" style="width: 10%">
                            <%--AML名單掃描結果--%>
                            <cc1:CustLabel ID="CustLabel49" runat="server" CurAlign="right" ShowID="01_08010600_030"></cc1:CustLabel>
                        </td>
                        <td align="left" colspan="5" style="width: 65%"><%--<td align="left" colspan="7">--%>
                            <asp:Label ID="lbHQ_SCDD_NameCheck_Item" runat="server" Text=""></asp:Label>
                            <asp:Label ID="lbHQ_SCDD_NameCheck_Note" runat="server" Text=""></asp:Label>
                            <asp:HiddenField ID="hidHQ_SCDD_NameCheck_RiskRanking" runat="server" />
                            <asp:HiddenField ID="hidHQ_SCDD_NameCheck_Item" runat="server" />
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="left">
                            <%--負責人姓名--%>
                            <cc1:CustLabel ID="CustLabel53" runat="server" CurAlign="right" ShowID="01_08010600_031"></cc1:CustLabel>
                        </td>
                        <td align="left">
                            <asp:Label ID="lbAML_HQ_Work_HCOP_OWNER_CHINESE_NAME" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left">
                            <%--證件類型/號碼--%>
                            <cc1:CustLabel ID="CustLabel54" runat="server" CurAlign="right" ShowID="01_08010600_032"></cc1:CustLabel>
                        </td>
                        <td align="left" style="word-break:break-all">
                            <%--身分證--%>
                            <cc1:CustLabel ID="CustLabel50" runat="server" CurAlign="right" ShowID="01_08010600_033"></cc1:CustLabel>
                            <asp:Label ID="lbAML_HQ_Work_HCOP_OWNER_ID" runat="server" Text=""></asp:Label>
                            <br />
                            <%--護照--%>
                            <cc1:CustLabel ID="CustLabel51" runat="server" CurAlign="right" ShowID="01_08010600_034"></cc1:CustLabel>
                            <asp:Label ID="lbAML_HQ_Work_HCOP_PASSPORT" runat="server" Text=""></asp:Label>
                            <br />
                            <%--居留證--%>
                            <cc1:CustLabel ID="CustLabel52" runat="server" CurAlign="right" ShowID="01_08010600_035"></cc1:CustLabel>
                            <asp:Label ID="lbAML_HQ_Work_HCOP_RESIDENT_NO" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left" style="word-break:break-all">
                            <%--負責人生日--%>
                            <cc1:CustLabel ID="CustLabel55" runat="server" CurAlign="right" ShowID="01_08010600_036"></cc1:CustLabel>
                        </td>
                        <td align="left" style="word-break:break-all">
                            <asp:Label ID="lbAML_HQ_Work_HCOP_OWNER_BIRTH_DATE" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left">
                            <%--負責人國籍--%>
                            <cc1:CustLabel ID="CustLabel56" runat="server" CurAlign="right" ShowID="01_08010600_037"></cc1:CustLabel>
                        </td>
                        <td align="left">
                            <asp:Label ID="lbAML_HQ_Work_HCOP_OWNER_NATION" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="left" style="word-break:break-all">
                            <%--負責人英文姓名--%>
                            <cc1:CustLabel ID="CustLabel57" runat="server" CurAlign="right" ShowID="01_08010600_038"></cc1:CustLabel>
                        </td>
                        <td align="left">
                            <asp:Label ID="lbAML_HQ_Work_HCOP_OWNER_ENGLISH_NAME" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left">
                            <%--身分證換補領查詢結果--%>
                            <cc1:CustLabel ID="CustLabel58" runat="server" CurAlign="right" ShowID="01_08010600_039"></cc1:CustLabel>
                        </td>
                        <td align="left" colspan="5">
                            <asp:Label ID="lbAML_HQ_Work_OWNER_ID_SreachStatus" runat="server" Text=""></asp:Label>
                            <asp:HiddenField ID="hidAML_HQ_Work_OWNER_ID_SreachStatus" runat="server" />
                        </td>
                    </tr>
                      <%--公司負責人長姓名 --%>
                        <tr class="trOdd" runat="server" id ="cmpLname" style="display:none">
                            <td align="right" colspan="1" >
                                <cc1:CustLabel ID="lblHCOP_LNAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_059" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="7">
                                  <cc1:CustLabel ID="HQlblHCOP_OWNER_CHINESE_LNAME" runat="server" CurAlign="Right" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>                            
                        </tr>
                         <tr class="trOdd" runat="server" id ="cmpRname" style="display:none">
                            <td align="right" colspan="1" >
                                <cc1:CustLabel ID="lblHCOP_ROMA" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_060" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="7">
                                <cc1:CustLabel ID="HQlblHCOP_OWNER_ROMA" runat="server" CurAlign="Right" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>                            
                        </tr>
                </table>

                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table1" style="">
                    <tr class="itemTitle">
                        <td align="left" colspan="8">
                            <%--關連人國籍(分公司負責及高階管理人暨實質受益人)--%>
                            <cc1:CustLabel ID="CustLabel61" runat="server" CurAlign="right" ShowID="01_08010600_040"></cc1:CustLabel>
                            <asp:HiddenField ID="hidID_SreachStatus_Msg" runat="server" />
                            <asp:HiddenField ID="hidPersonnelHTML" runat="server" />
                        </td>
                    </tr>
                    <%--table-row--%>
                    <tr class="trOdd" id="Personnel_tr" style="<%=Personnel_HtmlPanel%>">
                        <td align="center" style="width: 15%">
                            <%--姓名--%>
                            <cc1:CustLabel ID="CustLabel65" runat="server" CurAlign="right" ShowID="01_08010600_041"></cc1:CustLabel>
                        </td>
                        <td align="center" style="width: 5%">
                            <%--國籍--%>
                            <cc1:CustLabel ID="CustLabel66" runat="server" CurAlign="right" ShowID="01_08010600_042"></cc1:CustLabel>
                        </td>
                        <td align="center" style="width: 40%">
                            <%--中文長姓名--%>
                            <cc1:CustLabel ID="CustLabel67" runat="server" CurAlign="right" ShowID="01_01080103_059"></cc1:CustLabel>
                        </td>
                        <td align="center" style="width: 40%">
                            <%--羅馬--%>
                            <cc1:CustLabel ID="CustLabel68" runat="server" CurAlign="right" ShowID="01_01080103_060"></cc1:CustLabel>
                        </td>
                    </tr>
                    <%=Personnel_HtmlValue%>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table2">
                    <tr class="trOdd">
                        <td align="left" style="width: 25%">
                            <%--計算風險等級或異常結案--%>
                            <cc1:CustLabel ID="CustLabel69" runat="server" CurAlign="right" ShowID="01_08010600_043"></cc1:CustLabel>
                        </td>
                        <td align="left" colspan="7" style="width: 75%">
                            <%--20190614 (U) by Talas --%>
                            <cc1:CustButton ID="btnCalculatingRiskLevel" CssClass="smallButton" runat="server" Width="120px" DisabledWhenSubmit="False" ShowID="01_08010600_056"   OnClick="btnCalculatingRiskLevel_Click" />
                            <asp:CheckBox ID="cbCalculatingRiskLevel" runat="server" Text="" Visible="false" onclick="CBL_SingleChoice(this);" />
                            <asp:HiddenField ID="hidNewRiskLevel" runat="server" />
                            <%--20191129-RQ-2018-015749-002-因已有異常結案流程，故此處拿掉--%>
                            <%--<asp:CheckBoxList ID="cbCalculatingRiskLevel1" runat="server" RepeatLayout="Flow" RepeatColumns="4">
                                <asp:ListItem Text="不合作" Value="1" onclick="CBL_SingleChoice(this);"></asp:ListItem>
                                <asp:ListItem Text="商店解約" Value="2" onclick="CBL_SingleChoice(this);"></asp:ListItem>
                                <asp:ListItem Text="其他" Value="3" onclick="CBL_SingleChoice(this);"></asp:ListItem>
                            </asp:CheckBoxList>
                            <asp:TextBox ID="txtSR_RiskNote" runat="server" MaxLength="50" Width="300px"></asp:TextBox>--%>

                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="left">
                            <%--綜合說明及審查意見--%>
                            <cc1:CustLabel ID="CustLabel73" runat="server" CurAlign="right" ShowID="01_08010600_044"></cc1:CustLabel>
                            <br />
                            <%--(高風險客戶業務往來必填)--%>
                            <cc1:CustLabel ID="CustLabel74" runat="server" CurAlign="right" ShowID="01_08010600_045"></cc1:CustLabel>
                        </td>
                        <td align="left" colspan="7">
                            <asp:TextBox ID="txtSR_Explanation" runat="server" Height="100px" Width="760px" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table3">
                    <tr class="trOdd" style="<%=Signature_HtmlPanel%>">
                        <td align="left" colspan="8">
                            <%--本人__________________(簽名或蓋章)已就上述客戶進行盡職審查。憑著本表格內所填報的資料，依本人所知道及相信，本人認為客戶及其資金來源根據中國信託內部政策及程序及於有關國家/地區適用的防制洗錢法律均屬合法及可接受。--%>
                            <cc1:CustLabel ID="CustLabel77" runat="server" CurAlign="right" ShowID="01_08010600_046"></cc1:CustLabel>
                        </td>
                    </tr>
                    <tr class="trOdd" style="<%=Signature_HtmlPanel%>">
                        <td align="left">
                            <%--經辦(簽章)：--%>
                            <cc1:CustLabel ID="CustLabel81" runat="server" CurAlign="right" ShowID="01_08010600_047" IsColon="true"></cc1:CustLabel>
                            <br>
                            <br>
                            <br>
                            <br>
                            <br>
                            <%--日期--%>
                            <cc1:CustLabel ID="CustLabel19" runat="server" CurAlign="right" ShowID="01_08010600_048" IsColon="true"></cc1:CustLabel>
                        </td>
                        <td align="left">
                            <%--員編--%>
                            <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                            <br>
                            <br>
                            <br>
                            <br>
                            <br>
                            <%--日期資料--%>
                            <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left">
                            <%--主管(簽章)：--%>
                            <cc1:CustLabel ID="CustLabel22" runat="server" CurAlign="right" ShowID="01_08010600_049" IsColon="true"></cc1:CustLabel>
                            <br>
                            <br>
                            <br>
                            <br>
                            <br>
                            <%--日期--%>
                            <cc1:CustLabel ID="CustLabel24" runat="server" CurAlign="right" ShowID="01_08010600_048" IsColon="true"></cc1:CustLabel>
                        </td>
                        <td align="left">
                            <%--員編--%>
                            <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
                            <br>
                            <br>
                            <br>
                            <br>
                            <br>
                            <%--日期資料--%>
                            <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left">
                            <%--高風險客戶核准主管(簽章)：--%>
                            <cc1:CustLabel ID="CustLabel20" runat="server" CurAlign="right" ShowID="01_08010600_050" IsColon="true"></cc1:CustLabel>
                            <br>
                            <br>
                            <br>
                            <br>
                            <br>
                            <%--日期--%>
                            <cc1:CustLabel ID="CustLabel23" runat="server" CurAlign="right" ShowID="01_08010600_048" IsColon="true"></cc1:CustLabel>
                        </td>
                        <td align="left">
                            <%--員編--%>
                            <asp:Label ID="Label5" runat="server" Text=""></asp:Label>
                            <br>
                            <br>
                            <br>
                            <br>
                            <br>
                            <%--日期資料--%>
                            <asp:Label ID="Label6" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left">
                            <%--洗錢防制二部部長(簽章)：--%>
                            <cc1:CustLabel ID="CustLabel27" runat="server" CurAlign="right" ShowID="01_08010600_051" IsColon="true"></cc1:CustLabel>
                            <br>
                            <br>
                            <br>
                            <br>
                            <br>
                            <%--日期--%>
                            <cc1:CustLabel ID="CustLabel28" runat="server" CurAlign="right" ShowID="01_08010600_048" IsColon="true"></cc1:CustLabel>
                        </td>
                        <td align="left">
                            <%--員編--%>
                            <asp:Label ID="Label7" runat="server" Text=""></asp:Label>
                            <br>
                            <br>
                            <br>
                            <br>
                            <br>
                            <%--日期資料--%>
                            <asp:Label ID="Label8" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                </table>

                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo4" style="">
                        <tr class="itemTitle">
                            <td colspan="4" align="center">
                                <asp:CheckBox ID="chbSR_EDD_Status" runat="server" />
                                <cc1:CustLabel ID="CustLabel31" runat="server" CurAlign="right" ShowID="01_08010600_057" ForeColor="red" Font-Bold="true"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtEDDFinished" runat="server" MaxLength="8" Width="70px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="EDD完成日期"
                                    AutoPostBack="true"></cc1:CustTextBox>&nbsp;&nbsp;&nbsp;&nbsp;
                                <cc1:CustButton ID="btnView" CssClass="smallButton" runat="server" Width="81px"  OnClick="btnView_Click" DisabledWhenSubmit="False" ShowID="01_08010600_070" />
                                <cc1:CustButton ID="btnSubmit" CssClass="smallButton" runat="server" Width="40px"  OnClick="btnSubmit_Click" DisabledWhenSubmit="False" ShowID="01_08010600_052" />
                                <cc1:CustButton ID="btnCancel" CssClass="smallButton" runat="server" Width="69px" OnClick="btnCancel_Click" DisabledWhenSubmit="False" ShowID="01_08010600_053" />
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
