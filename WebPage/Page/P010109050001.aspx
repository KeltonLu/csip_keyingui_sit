<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010109050001.aspx.cs" Inherits="Page_P010109050001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">

    protected void grvCardData_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
    
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript"> 
        // 面頁邏輯檢核
        function checkInputText(id, intType) {
            mustKeyIn(intType);

            var emailF = document.getElementById('txtHCOP_EMAIL').value.Trim();
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
                            document.getElementById('txtHCOP_EMAIL').focus();
                            return false;
                        }
                    }
                }
                else {
                    alert('請選擇E-Mail!');
                    return false;
                }
            }

            return true;
        }

        function mustKeyIn() {
            var isSuccess = true;

        }
        $(function () {
            $("#HCOP_SIXM_TOT_AMT").click(function () {
                //$("#myModal").slideToggle("normal");
                $("#myModal").dialog({
                    autoOpen: false, width: 1500, draggable: true
                });
                $("#myModal").dialog('open');
            });
            $("#mySpan").click(function () {
                //$("#myModal").slideToggle("normal");
                $("#myModal").dialog('close');
            });
        });
        $(function () {
            $("#grvCardData_ctl02_lblBRCH_SIXM_TOT_AMT").click(function () {
                //$("#myBrchModal").slideToggle("normal");
                $("#myBrchModal").dialog({
                    autoOpen: false, width: 1500, draggable: true
                });
                $("#myBrchModal").dialog('open');
            });
            $("#myBrchSpan").click(function () {
                //$("#myBrchModal").slideToggle("normal");
                $("#myBrchModal").dialog('close');
            });
        });
    </script> 
   <style type="text/css">
        .btnHiden {
            display: none;
        }

        .auto-style2 {
            width: 1412px;
        }
        
        /* The Modal (background) */
        .modal {
            display: none; /* Hidden by default */
            position: fixed; /* Stay in place */
            z-index: 1; /* Sit on top */
            right: 20px;
            top: 300px;
            width: 100%; /* Full width */
            height: 80%; /* Full height */
            overflow: auto; /* Enable scroll if needed */
          /*left: 200px;*/
        }

        /* Modal Content/Box */
        .modal-content {
            background-color: #fefefe;
             /*.background-color: rgb(230, 230, 230);*/
            margin: 0 auto; /* 15% from the top and centered */
            padding: 20px;
            border: 1px solid #888;
            width: 70%;/* Could be more or less, depending on screen size */
        }
        .ui-dialog-titlebar-close{
            visibility:hidden;
        }
        .modal-header {
            padding: 2px 16px;
            background-color: #006666;
            color:white;
            margin: 0 auto;
          /*style="background-color:#FFAFFE;width:300px;height:20px;margin:0 auto;"*/
        }
        /* The Close Button */
        .close {
            color: #aaa;
            float: right;
            float: right;
            font-size: 28px;
            font-weight: bold;
        }

        .close:hover,
        .close:focus {
            cursor:pointer;
            color:black;
            text-decoration: none;
        }
        .pointer {cursor: pointer;}
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
                <%--自然人收單AML資料定期審查--%>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                    <tr class="itemTitle">
                        <td>
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="300px" IsColon="False" ShowID="01_01090500_001"></cc1:CustLabel>                                                                
                            </li>
                        </td>
                    </tr>
                    
                    <tr>
                        <td style="width: 100%" colspan="1">
                            <table border="0" width="100%" cellspacing="1">
                                <tr class="trOdd">
                                    <%-- 案件編號 --%>
                                    <td align="right" width="4%">
                                        <cc1:CustLabel ID="caseNumber" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_002" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td colspan="">
                                        <cc1:CustLabel ID="hlblCaseNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <%-- 客戶ID --%>
                                    <td align="right" width="4%">
                                        <cc1:CustLabel ID="customerId" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_003" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td colspan="3">
                                        <cc1:CustLabel ID="HQlblHCOP_HEADQUATERS_CORP_NO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>                                    
                                </tr>
                                <%--新增案件編號--%>
                                <%--//20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis--%>
                                <%--<tr class="trOdd">
                                    <td align="right" style="width: 11%">
                                        <cc1:CustLabel ID="CustLabel75" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01080101_098" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style="width: 89%" colspan="5">
                                        <cc1:CustLabel ID="hlblRelateCaseNo" runat="server" CurSymbol="£"></cc1:CustLabel>
                                    </td>
                                </tr>--%>
                                <%--//20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis--%>
                                <tr class="trOdd">
                                    <%-- 原本的風險等級 --%>
                                    <td align="right" width="4%">
                                        <cc1:CustLabel ID="originalRiskLevel" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_004" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="12%">
                                        <cc1:CustLabel ID="hlblOriginalRiskRanking" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <%-- 原本的下次審查日期 --%>
                                    <td align="right" width="6%">
                                        <cc1:CustLabel ID="originalNextReviewDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_005" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="4%">
                                        <cc1:CustLabel ID="hlblOriginalNextReviewDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                   <%-- 審查到期日 --%>
                                    <td align="right" width="4%">
                                        <cc1:CustLabel ID="reviewExpiryDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_006" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="4%">
                                        <cc1:CustLabel ID="hlblCaseExpiryDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                </tr>
                                <tr class="trOdd">
                                    <%-- 最後試算的風險等級 --%>
                                    <td align="right" width="8%">
                                        <cc1:CustLabel ID="finalCalculationRiskLevel" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_007" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="8%">
                                        <cc1:CustLabel ID="hlblNewRiskRanking" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <%-- 最後試算後的下次審查日期 --%>
                                    <td align="right" width="6%">
                                        <cc1:CustLabel ID="finalCalculationNextReviewDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_008" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="8%">
                                        <cc1:CustLabel ID="hlblNewNextReviewDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <%-- 案件種類 --%>
                                    <td align="right" width="6%">
                                        <cc1:CustLabel ID="caseType" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_009" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td width="8%">
                                        <cc1:CustLabel ID="hlblCaseType" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>

                                </tr>
                            </table>
                        </td>
                    </tr> 
                </table>
                <%-- 客戶資料 --%>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo2" style="">
                    <tr class="itemTitle">
                        <td>
                            <li>
                                <cc1:CustLabel ID="title2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01090500_014"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    
                    <tr>
                        <td style="width: 100%" colspan="1">
                            <table border="0" width="100%" cellspacing="1">
                                <tr class="trEven">
                                    <%--商店狀態--%>
                                     <td align="right" class="auto-style1">
                                        <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01080103_003" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td class="auto-style2">
                                        <cc1:CustLabel ID="HQlblHCOP_STATUS" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>               
                                    <%-- 身分證字號 --%>
                                    <td align="right" width="4%">
                                        <cc1:CustLabel ID="IDNum" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_015" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td colspan="2">
                                        <cc1:CustLabel ID="HQlblHCOP_OWNER_ID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <%-- 中文姓名 --%>
                                    <td align="right" width="4%">
                                        <cc1:CustLabel ID="chineseName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_016" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td colspan="2">
                                        <cc1:CustLabel ID="HQlblHCOP_OWNER_CHINESE_NAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                </tr>
                                <tr class="trOdd">
                                    <%-- 英文姓名 --%>
                                    <td align="right" style="width: 11%">
                                        <cc1:CustLabel ID="eName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_017" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style="width: 25%" colspan="2">
                                        <cc1:CustLabel ID="HQlblHCOP_OWNER_ENGLISH_NAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <%-- 出生年月日 --%>
                                    <td align="right" style="width: 11%">
                                        <cc1:CustLabel ID="Birthday" runat="server" CurAlign="left" CurSymbol="£"
                                            FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                            NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01090500_018"
                                            StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style="width: 17%" >
                                        <cc1:CustTextBox ID="txtHCOP_OWNER_BIRTH_DATE" runat="server" MaxLength="8" checktype="num"
                                            Width="200px" onkeydown="entersubmit('btnAdd');" BoxName="出生年月日"></cc1:CustTextBox>
                                    </td>
                                    <%-- 性別 --%>
                                    <td align="right" style="width: 11%">
                                        <cc1:CustLabel ID="sex" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_019" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style="width: 11%" colspan="2">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtGENDER" runat="server" MaxLength="3" Width="50px" onfocus="allselect(this);" 
                                            onkeydown="entersubmit('btnAdd');" BoxName="性別"
                                            Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px;" Columns="7"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropGENDER" kind="select" runat="server" onclick="simOptionClick4IE('txtGENDER');" 
                                            Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;"></cc1:CustDropDownList>
                                        </div>
                                        
                                        <cc1:CustHiddenField ID="HQlblHCOP_GENDER" runat="server" />
                                    </td>
                                </tr>
                                <tr class="trEven" >
                                    <%-- 國籍1 --%>
                                    <td align="right" style="width: 11%">
                                        <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_020" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style="width: 11%" >
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtHCOP_OWNER_NATION" runat="server" MaxLength="2" Width="50px" onfocus="allselect(this);" 
                                                onkeydown="entersubmit('btnAdd');" BoxName="國籍1" Enabled="false"
                                                Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropCountry1" kind="select" runat="server" onclick="simOptionClick4IE('txtCountryCode');" 
                                                Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;" Enabled="false"></cc1:CustDropDownList>
                                            <cc1:CustHiddenField ID="hidCountry1" runat="server" />
                                        </div>
                                    </td>
                                    <%-- 國籍2 --%>
                                    <td align="right" style="width: 11%">
                                        <cc1:CustLabel ID="country2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_021" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style="width: 11%" colspan="5">
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtHCOP_COUNTRY_CODE_2" runat="server" MaxLength="3" Width="50px" onfocus="allselect(this);" 
                                                onkeydown="entersubmit('btnAdd');" BoxName="國籍2" OnTextChanged="txtHCOP_COUNTRY_CODE_2_TextChanged" AutoPostBack="true"
                                                Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropCountry2" kind="select" runat="server" onclick="simOptionClick4IE('txtHCOP_COUNTRY_CODE_2');" 
                                                Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;"></cc1:CustDropDownList>
                                            <cc1:CustHiddenField ID="hidCountry2" runat="server" />
                                        </div>
                                    </td>                                    
                                </tr>
                                <tr class="trOdd">
                                     <%-- 身分證發證日期 --%>
                                     <td align="right" style="width: 11%">                                        
                                         <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£"
                                            FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                            NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01090500_022"
                                            StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style="width: 11%" >
                                        <cc1:CustTextBox ID="txtHCOP_OWNER_ID_ISSUE_DATE" runat="server" MaxLength="8" checktype="num"
                                            Width="200px" onkeydown="entersubmit('btnAdd');" BoxName="身分證發行日期"></cc1:CustTextBox>
                                    </td>
                                     <%-- 發證地點 --%>
                                     <td align="right" style="width: 11%">                                        
                                         <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_023" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style="width: 11%">
                                        <cc1:CustTextBox ID="txtHCOP_OWNER_ID_ISSUE_PLACE" runat="server" MaxLength="18" Width="120px"  onblur="changeFullType(this);"
                                            onkeydown="entersubmit('btnAdd');" BoxName="發證地點" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                                    </td>
                                     <%-- 領補換類別 --%>
                                     <td align="right" style="width: 11%">
                                        <cc1:CustLabel ID="category" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_024" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style="width: 11%" >
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtHQlblHCOP_OWNER_ID_REPLACE_TYPE" runat="server" MaxLength="3" Width="50px" onfocus="allselect(this);" 
                                                onkeydown="entersubmit('btnAdd');" BoxName="領補換類別" 
                                                Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropHCOP_OWNER_ID_REPLACE_TYPE" kind="select" runat="server" onclick="simOptionClick4IE('txtHQlblHCOP_OWNER_ID_REPLACE_TYPE');" 
                                                Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;"></cc1:CustDropDownList>                                            
                                        </div>
                                    </td>                                    
                                     <%-- 有無照片 --%>
                                     <td align="right" style="width: 11%">
                                        <cc1:CustLabel ID="photos" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_025" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style="width: 11%" >
                                        <div style="position: relative">
                                            <cc1:CustTextBox ID="txtHQlblHCOP_ID_PHOTO_FLAG" runat="server" MaxLength="3" Width="50px" onfocus="allselect(this);" 
                                                onkeydown="entersubmit('btnAdd');" BoxName="有無照片" 
                                                Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px;"></cc1:CustTextBox>
                                            <cc1:CustDropDownList ID="dropHCOP_ID_PHOTO_FLAG" kind="select" runat="server" onclick="simOptionClick4IE('txtHQlblHCOP_ID_PHOTO_FLAG');" 
                                                Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;"></cc1:CustDropDownList>                                            
                                        </div>
                                    </td>                                     
                                 </tr>
                                 <tr class="trEven">
                                    <%-- 戶籍地址 --%>
                                    <td align="right" style="width: 11%">
                                        <cc1:CustLabel ID="residenceAddress" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_026" StickHeight="False"></cc1:CustLabel>
                                    </td>                                    
                                     <td style="width: 88%" colspan="7">
                                        <cc1:CustLabel ID="HQlblHCOP_REG_ZIP_CODE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                        <cc1:CustLabel ID="HQlblHCOP_REG_CITY" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                        <cc1:CustLabel ID="HQlblHCOP_REG_ADDR1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                        <cc1:CustLabel ID="HQlblHCOP_REG_ADDR2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                        <cc1:CustHiddenField ID="hidHCOP_REG_ZIP_CODE" runat="server" />
                                        <cc1:CustHiddenField ID="hidHCOP_REG_CITY" runat="server" />
                                        <cc1:CustHiddenField ID="hidHCOP_REG_ADDR1" runat="server" />
                                        <cc1:CustHiddenField ID="hidHCOP_REG_ADDR2" runat="server" />
                                    </td>
                                </tr>
                                <tr class="trOdd">
                                    <%-- 通訊地址 --%>
                                    <td align="right" style="width: 11%">
                                        <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_070" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style="width: 88%" colspan="7">                                         
                                        <cc1:CustTextBox ID="txtHCOP_MAILING_CITY" runat="server" checktype="fulltype" MaxLength="6"
                                            Width="100px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                            BoxName="通訊地址一" onpaste="paste();" AutoPostBack="False" ></cc1:CustTextBox>
                                        <cc1:CustTextBox ID="txtHCOP_MAILING_ADDR1" runat="server" checktype="fulltype" MaxLength="14"
                                            Width="220px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                            BoxName="通訊地址二" onpaste="paste();"></cc1:CustTextBox>
                                        <cc1:CustTextBox ID="txtHCOP_MAILING_ADDR2" runat="server" checktype="fulltype" MaxLength="7"
                                            Width="100px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                            BoxName="通訊地址三" onpaste="paste();"></cc1:CustTextBox>
                                    </td>
                                </tr>
                                <tr class="trOdd">
                                    <%--E-MAIL--%>
                                    <td align="right" style="width: 11%">
                                        <cc1:CustLabel ID="lblEmail" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_027" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td align="left" style="width:89%" colspan="7">
                                        <cc1:CustTextBox ID="txtHCOP_EMAIL" runat="server" Width="200px" 
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
                                <tr class="trEven">
                                    <%-- 連絡電話 --%>
                                    <td align="right" style="width: 15%">
                                            <cc1:CustLabel ID="contactTel" runat="server" CurAlign="left" CurSymbol="£"
                                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01090500_028"
                                                StickHeight="False"></cc1:CustLabel></td>
                                    <td style="width: 35%" colspan="3">
                                        <cc1:CustTextBox ID="txtContactTel1" runat="server" Width="60px" MaxLength="3"
                                            onkeydown="entersubmit('btnBasicSubmit');" checktype="num" BoxName="連絡電話一"></cc1:CustTextBox>
                                        <cc1:CustTextBox ID="txtContactTel2" runat="server" Width="100px" MaxLength="8"
                                            onkeydown="entersubmit('btnBasicSubmit');" checktype="num" BoxName="連絡電話二"></cc1:CustTextBox>
                                        <cc1:CustTextBox ID="txtContactTel3" runat="server" Width="60px" MaxLength="5"
                                            onkeydown="entersubmit('btnBasicSubmit');" checktype="num" BoxName="連絡電話三"></cc1:CustTextBox>
                                        <cc1:CustHiddenField ID="hidHCOP_CONTACT_TEL" runat="server" />
                                    </td>
                                    <%-- 行動電話 --%>
                                    <td align="right" style="width: 15%">
                                            <cc1:CustLabel ID="mobilePhone" runat="server" CurAlign="left" CurSymbol="£"
                                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01090500_029"
                                                StickHeight="False"></cc1:CustLabel></td>
                                    <td style="width: 35%" colspan="3">
                                        <cc1:CustTextBox ID="txtHCOP_MOBILE" runat="server" Width="100px" MaxLength="10"
                                            onkeydown="entersubmit('btnBasicSubmit');" checktype="num" BoxName="行動電話"></cc1:CustTextBox>
                                    </td>
                                </tr>
                                
                                <tr class="trOdd">
                                    <%-- 任職公司 --%>
                                    <td align="right" style="width: 11%">
                                        <cc1:CustLabel ID="employedCompany" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_030" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style="width: 89%" colspan="7">
                                        <cc1:CustTextBox ID="HQlblHCOP_NP_COMPANY_NAME" runat="server" MaxLength="120" Width="500px"  onblur="changeFullType(this);"
                                            onkeydown="entersubmit('btnAdd');" TabIndex="21" BoxName="任職公司" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                                        <cc1:CustHiddenField ID="hidHCOP_NP_COMPANY_NAME" runat="server" />
                                    </td>
                                </tr>
                                <tr class="trEven">
                                    <%-- 行業別編號 --%>
                                    <td align="right" style="width: 11%">
                                        <cc1:CustLabel ID="industryNumber" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_031" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style="width: 89% " colspan="7">
                                        <cc1:CustLabel ID="labIndustryNum1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                                SetBreak="False" SetOmit="False" ShowID="01_01090500_032" StickHeight="False"></cc1:CustLabel>
                                        <cc1:CustTextBox ID="txtAMLCC" runat="server" MaxLength="7" checktype="undefined" AutoPostBack="true" OnTextChanged="txtBasicAMLCC_TextChanged" 
                                            Width="100px" onkeydown="entersubmit('btnAdd');" TabIndex="8" BoxName="行業別編號1" ></cc1:CustTextBox>
                                        <%-- 行業別編號 1 中文名稱 --%>
                                        <cc1:CustLabel ID="HQlblHCOP_CC_CNAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False" Style="margin-right:50px"></cc1:CustLabel>

                                        <cc1:CustLabel ID="labIndustryNum2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                                SetBreak="False" SetOmit="False" ShowID="01_01090500_033" StickHeight="False"></cc1:CustLabel>
                                        <cc1:CustTextBox ID="txtHCOP_CC_2" runat="server" MaxLength="7" checktype="undefined" AutoPostBack="true" OnTextChanged="txtBasicAMLCC2_TextChanged"
                                            Width="100px" onkeydown="entersubmit('btnAdd');" TabIndex="8" BoxName="行業別編號2" ></cc1:CustTextBox>
                                        <%-- 行業別編號 2 中文名稱 --%>
                                        <cc1:CustLabel ID="HQlblHCOP_CC2_CNAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False" Style="margin-right:50px"></cc1:CustLabel>

                                        <cc1:CustLabel ID="labIndustryNum3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                                SetBreak="False" SetOmit="False" ShowID="01_01090500_034" StickHeight="False"></cc1:CustLabel>
                                        <cc1:CustTextBox ID="txtHCOP_CC_3" runat="server" MaxLength="7" checktype="undefined" AutoPostBack="true" OnTextChanged="txtBasicAMLCC3_TextChanged"
                                            Width="100px" onkeydown="entersubmit('btnAdd');" TabIndex="8" BoxName="行業別編號3"></cc1:CustTextBox>
                                        <%-- 行業別編號 3 中文名稱 --%>
                                        <cc1:CustLabel ID="HQlblHCOP_CC3_CNAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                        <cc1:CustHiddenField ID="hidCC" runat="server" />
                                        <cc1:CustHiddenField ID="hidCC2" runat="server" />
                                        <cc1:CustHiddenField ID="hidCC3" runat="server" />
                                        <cc1:CustHiddenField ID="hidAllCC" runat="server" />
                                    </td>
                                </tr>
                                <tr class="trOdd">
                                    <%-- 職稱編號 --%>
                                    <td align="right" style="width: 11%">
                                            <cc1:CustLabel ID="jobNum" runat="server" CurAlign="left" CurSymbol="£"
                                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01090500_035"
                                                StickHeight="False"></cc1:CustLabel></td>
                                    <td style="width: 89%" colspan="7">
                                        <cc1:CustTextBox ID="txtHCOP_OC" runat="server" Width="100px" MaxLength="4"  AutoPostBack="true" OnTextChanged="txtBasicAMLOC_TextChanged"
                                            onkeydown="entersubmit('btnBasicSubmit');" checktype="num" BoxName="職稱編號"></cc1:CustTextBox>
                                        <%-- 職稱編號 中文名稱 --%>
                                        <cc1:CustLabel ID="HQlblHCOP_OC_CNAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                        <cc1:CustHiddenField ID="hidOC" runat="server" />
                                    </td>
                                </tr>
                                <tr class="trEven" style="height:60px">
                                    <%-- 收入及資產來源(複選) --%>
                                    <td align="right" style="width: 11%">
                                          <cc1:CustLabel ID="income" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_036" StickHeight="False"></cc1:CustLabel>
                                     </td >
                                     <td  colspan="7">
                                         <cc1:CustCheckBox ID="chkIncome1" runat="server"
                                            AutoPostBack="false" BoxName="薪資" />
                                         <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_037" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                         <cc1:CustCheckBox ID="chkIncome2" runat="server"
                                            AutoPostBack="false" BoxName="經營事業收入" />
                                         <cc1:CustLabel ID="CustLabel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_038" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                         <cc1:CustCheckBox ID="chkIncome3" runat="server"
                                            AutoPostBack="false" BoxName="退休(職)資金" />
                                         <cc1:CustLabel ID="CustLabel9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_039" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                         <cc1:CustCheckBox ID="chkIncome4" runat="server"
                                            AutoPostBack="false" BoxName="遺產繼承(含贈與)" />
                                         <cc1:CustLabel ID="CustLabel10" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_040" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                         <cc1:CustCheckBox ID="chkIncome5" runat="server"
                                            AutoPostBack="false" BoxName="買賣房地產" />
                                         <cc1:CustLabel ID="CustLabel11" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_041" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                         <cc1:CustCheckBox ID="chkIncome6" runat="server"
                                            AutoPostBack="false" BoxName="投資理財" />
                                         <cc1:CustLabel ID="CustLabel12" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_042" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                         <cc1:CustCheckBox ID="chkIncome7" runat="server"
                                            AutoPostBack="false" BoxName="租金收入" />
                                         <cc1:CustLabel ID="CustLabel13" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_043" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                         <cc1:CustCheckBox ID="chkIncome8" runat="server"
                                            AutoPostBack="false" BoxName="存款" />
                                         <cc1:CustLabel ID="CustLabel14" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_070" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                         <cc1:CustCheckBox ID="chkIncome9" runat="server"
                                            AutoPostBack="false" BoxName="其他" />
                                         <cc1:CustLabel ID="CustLabel15" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_044" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                         <cc1:CustHiddenField ID="hidHCOP_INCOME_SOURCE" runat="server" />
                                         
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <%-- 客戶風險分析 --%>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo3" style="">
                    <tr class="itemTitle">
                        <td>
                            <li>
                                <cc1:CustLabel ID="title3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01090500_045"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    
                    <tr>
                        <td style="width: 100%" colspan="1">
                            <table border="0" width="100%" cellspacing="1">
                                <tr class="trOdd">
                                    <%-- 客戶 國籍、戶籍地 或 通訊地 或關聯人任一人 國籍 位於位於高風險國家/地區 --%>
                                    <td align="left" width="80%">
                                        <cc1:CustLabel ID="customerRisk1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_046" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td align="center" colspan="">
                                        <cc1:CustLabel ID="lblIsRisk" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                </tr>
                                <tr class="trEven">
                                    <%-- 客戶 國籍、戶籍地 或 通訊地 位於一般或高度制裁國家/地區 --%>
                                    <td align="left" width="80%">
                                        <cc1:CustLabel ID="customerRisk2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_047" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td align="center" colspan="">
                                        <cc1:CustLabel ID="lblIsSanction" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                </tr>
                                <tr class="trOdd">
                                    <%-- 國外PEP --%>
                                    <td align="left" width="4%">
                                        <cc1:CustLabel ID="customerRisk3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_048" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td align="center" colspan="">
                                        <cc1:CustLabel ID="CDlblForeignPEPStakeholder" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                </tr>
                                <tr class="trEven">
                                    <%-- 負面新聞(NN) --%>
                                    <td align="left" width="4%">
                                        <cc1:CustLabel ID="customerRisk4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_049" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td align="center" colspan="">
                                        <cc1:CustLabel ID="CDlblNNListHitFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                </tr>
                                <tr class="trOdd">
                                    <%-- 客戶帳戶為警示或衍生帳戶 --%>
                                    <td align="left" width="4%">
                                        <cc1:CustLabel ID="customerRisk5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_050" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td align="center" colspan="">
                                        <cc1:CustLabel ID="CDlblWarningFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                </tr>
                                <tr class="trEven">
                                    <%-- 客戶信用卡有不良註記 --%>
                                    <td align="left" width="4%">
                                        <cc1:CustLabel ID="customerRisk6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_051" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td align="center" colspan="">
                                        <cc1:CustLabel ID="CDlblCreditCardBlockCode" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                </tr>
                                <tr class="trOdd">
                                    <%-- 集團關注名單 --%>
                                    <td align="left" width="4%">
                                        <cc1:CustLabel ID="customerRisk7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_052" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td align="center" colspan="">
                                        <cc1:CustLabel ID="CDlblGroupInformationSharingNameListflag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                </tr>
                                <tr class="trEven">
                                    <%-- 不合作/拒絕提供資訊 --%>
                                    <td align="left" width="4%">
                                        <cc1:CustLabel ID="customerRisk8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_053" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td align="center" colspan="">
                                        <cc1:CustLabel ID="CDlblIncorporated" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <%--商店資訊--%>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo11" style="table-layout:fixed">
                    <tr class="itemTitle">
                        <td>
                            <li>
                                <cc1:CustLabel ID="CustLabel56" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01090400_054"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cc1:CustGridView ID="grvCardData" runat="server" BorderStyle="Solid" EnableModelValidation="True" PagerID="GridPager1" Width="100%" OnRowDataBound="grvCardData_RowDataBound" OnRowCommand="grvCardData_RowCommand" Class="longTableGridView" >
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" />
                                <Columns>
                                        <asp:BoundField DataField="BRCH_BRCH_NO" HeaderText="商店代號" HeaderStyle-CssClass="whiteSpaceNormal" ItemStyle-CssClass="whiteSpaceNormal" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="REG_CHI_NAME" HeaderText="商店名稱" HeaderStyle-CssClass="whiteSpaceNormal" ItemStyle-CssClass="whiteSpaceNormal" ItemStyle-HorizontalAlign="Center" />                                                                                                                                                                                                        
                                        <asp:TemplateField HeaderText="年度請款金額">
                                            <ItemTemplate>
                                                <asp:Label ID="lblBRCH_SIXM_TOT_AMT" runat="server" Text='<%# Bind("BRCH_SIXM_TOT_AMT") %>' Font-Bold="true" CssClass="pointer"></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="center" CssClass="whiteSpaceNormal" />
                                            <HeaderStyle CssClass="whiteSpaceNormal" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="編輯" Visible="false">
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" ID="lnkView" CommandArgument='<%#Eval("ArgNo") %>'
                                                    CommandName="Show">編輯</asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                <HeaderStyle CssClass="Grid_Header" />
                                <PagerSettings Visible="False" />
                                <RowStyle CssClass="Grid_Item" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                            </cc1:CustGridView>
                            <!-- The Modal -->
                            <div id="myBrchModal" class="modal" style="display:none">                                    
                                <!-- Modal content -->
                                <div class="modal-content">
                                    <span  id="myBrchSpan" class="close">&times;</span>
                                    <div class="modal-header" >
                                    <p style="text-align:center">分公司-月請款金額</p>
                                </div>

                                <asp:GridView ID="gdvBRCHPerAMT" runat="server" AutoGenerateColumns="False" EnableModelValidation="True" BorderStyle="Solid" Width="100%" OnPreRender="gdvBRCHPerAMT_PreRender">
                                    <AlternatingRowStyle CssClass="Grid_AlternatingItem"/>
                                    <Columns>
                                        <asp:BoundField HeaderText="統一編號" DataField="BRCH_BRCH_NO">
                                        <ItemStyle Width="90px" HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="1月<br />請款金額" DataField="BRCH_MON_AMT1" DataFormatString="{0:N0}" HtmlEncode="False"  >
                                        <ItemStyle Width="90px" HorizontalAlign="Right" />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="2月<br />請款金額" DataField="BRCH_MON_AMT2" DataFormatString="{0:N}" HtmlEncode="False" >
                                        <ItemStyle Width="90px" HorizontalAlign="Right"  />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="3月<br />請款金額" DataField="BRCH_MON_AMT3" DataFormatString="{0:N}" HtmlEncode="False" >
                                        <ItemStyle Width="90px" HorizontalAlign="Right"  />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="4月<br />請款金額" DataField="BRCH_MON_AMT4" DataFormatString="{0:N}" HtmlEncode="False" >
                                        <ItemStyle Width="90px" HorizontalAlign="Right" />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="5月<br />請款金額" DataField="BRCH_MON_AMT5" DataFormatString="{0:N}" HtmlEncode="False" >
                                        <ItemStyle Width="90px" HorizontalAlign="Right" />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="6月<br />請款金額" DataField="BRCH_MON_AMT6" DataFormatString="{0:N}" HtmlEncode="False" >
                                        <ItemStyle Width="90px" HorizontalAlign="Right" />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="7月<br />請款金額" DataField="BRCH_MON_AMT7" DataFormatString="{0:N}" HtmlEncode="False" >
                                        <ItemStyle Width="90px" HorizontalAlign="Right" />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="8月<br />請款金額" DataField="BRCH_MON_AMT8" DataFormatString="{0:N}" HtmlEncode="False" >
                                        <ItemStyle Width="90px" HorizontalAlign="Right" />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="9月<br />請款金額" DataField="BRCH_MON_AMT9" DataFormatString="{0:N}" HtmlEncode="False" >
                                        <ItemStyle Width="90px" HorizontalAlign="Right" />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="10月<br />請款金額" DataField="BRCH_MON_AMT10" DataFormatString="{0:N}" HtmlEncode="False" >
                                        <ItemStyle Width="90px" HorizontalAlign="Right" />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="11月<br />請款金額" DataField="BRCH_MON_AMT11" DataFormatString="{0:N}" HtmlEncode="False" >
                                        <ItemStyle Width="90px" HorizontalAlign="Right" />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="12月<br />請款金額" DataField="BRCH_MON_AMT12" DataFormatString="{0:#,#}" HtmlEncode="False" >
                                        <ItemStyle Width="90px" HorizontalAlign="Right" />
                                        </asp:BoundField>
                                    </Columns>
                                        <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                        <PagerSettings Visible="False" />
                                        <RowStyle CssClass="Grid_Item" />
                                        <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                    </asp:GridView>
                                    

                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
                <p></p>
                <%-- SCDD --%>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo5" style="">
                    <tr>
                        <td style="width: 100%" colspan="1">
                            <table border="0" width="100%" cellspacing="1">
                                <tr class="trOdd">
                                    <%-- SCDD編輯日期 --%>
                                    <td align="right" width="16%" bgcolor="#FF9900">
                                        <cc1:CustLabel ID="SCDDEditDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_055" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td colspan="">
                                        <cc1:CustLabel ID="lblSR_DateTime" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <%-- SCDD風險等級 --%>
                                    <td align="right" width="16%" bgcolor="#FF9900">
                                        <cc1:CustLabel ID="riskLevel" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_056" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td colspan="">
                                        <cc1:CustLabel ID="lblSR_RiskLevel" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <p></p>
                <%-- 名單掃描結果 --%>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo6" style="">
                    <tr class="itemTitle">
                        <td align="center">                        
                            <cc1:CustLabel ID="title5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" Width="200px" IsColon="False" ShowID="01_01090500_057"></cc1:CustLabel>
                            <%-- 執行名單掃描 --%>
                            <cc1:CustButton ID="btnListScan" runat="server" CssClass="smallButton" ShowID="01_01090500_058"
                                OnClick="btnESB_NameCheck_Click"
                                DisabledWhenSubmit="False" BoxName="執行名單掃描" Style="margin-right:50px" />
                                <%-- 名單掃描紀錄查詢 --%>
                            <cc1:CustButton ID="btnListScanRecordQuery" runat="server" CssClass="smallButton" ShowID="01_01090500_059"
                                OnClick="btnESB_NameCheck_Detail_Click" 
                                DisabledWhenSubmit="False" BoxName="名單掃描紀錄查詢" Style="margin-right:50px" />
                            <div style="display:none">
                                <cc1:CustButton ID="btnESB_NameCheckAdd" runat="server" CssClass="smallButton" ShowID="01_01080101_087"
                                    DisabledWhenSubmit="False" Enabled="true" OnClick="btnESB_NameCheckAdd_Click" />
                            </div>
                        </td>
                    </tr>
                    
                    <tr>
                        <td style="width: 100%" colspan="1">
                            <table border="0" width="100%" cellspacing="1">
                                <tr class="trOdd">
                                    <%-- 名單掃描案件編號 --%>
                                    <td align="center" width="50%" bgcolor="#FF9900">
                                        <cc1:CustLabel ID="listScanCaseNumber" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_060" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                   
                                    <%-- 結果 --%>
                                    <td align="center" width="50%" bgcolor="#FF9900">
                                        <cc1:CustLabel ID="result" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090500_061" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                </tr>
                                <tr class="trOdd">
                                    <%-- 名單掃描案件編號 --%>
                                    <td align="right" style="width: 40%">                                        
                                            <cc1:CustTextBox ID="txtNameCheck_No" runat="server" MaxLength="70"
                                            Width="420px" BoxName="名單掃描案件編號"></cc1:CustTextBox>
                                    </td>
                                    <%-- 結果 --%>
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
                        </td>
                    </tr>
                </table>


                <%-- MFA資訊 --%>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo7" style="">
                    <tr class="itemTitle">
                        <td align="center">
                            <cc1:CustLabel ID="title6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01090400_062"></cc1:CustLabel>
                        </td>
                    </tr>
                    
                    <tr>
                        <td style="width: 100%" colspan="1">
                            <table border="0" width="100%" cellspacing="1">
                                <tr class="trOdd">
                                   
                                    <td align="right" width="16%" bgcolor="#FF9900">
                                        <cc1:CustLabel ID="MAF" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_063" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style="width: 34%">
                                        <cc1:CustLabel ID="HQlblMFAF_NAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                   
                                    <td align="right" width="16%" bgcolor="#FF9900">
                                        <cc1:CustLabel ID="areaCenter" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_064" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style="width: 34%">
                                        <cc1:CustLabel ID="HQlblMFAF_AREA" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>

                 <%-- 聯絡記錄編輯 --%>
                <cc1:CustButton ID="btnContactRecordEditor" runat="server" CssClass="smallButton" ShowID="01_01090500_065"
                          OnClick="btnNOTEEdit_Click"
                        DisabledWhenSubmit="False" BoxName="聯絡記錄編輯" Style="margin-right:50px" />
                
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo8" style="">
                    <tr class="itemTitle">
                        <td align="center">
                            <cc1:CustLabel ID="title7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01090400_066"></cc1:CustLabel>
                        </td>
                    </tr>
                    
                    <tr>
                        <td style="width: 100%" colspan="1">
                            <table border="0" width="100%" cellspacing="1">
                                <tr class="trEven">
                                    <td style="width: 99%">
                                        <cc1:CustGridView ID="grdNoteLog" runat="server" BorderStyle="Solid" PagerID="GridPager1" Width="100%"  OnRowDataBound="grdNoteLog_RowDataBound">
                                            <AlternatingRowStyle CssClass="Grid_AlternatingItem" />
                                            <Columns>
                                                <asp:BoundField DataField="" HeaderText="日期" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" >
                                                  <HeaderStyle HorizontalAlign="Center" />
                                                  <ItemStyle HorizontalAlign="Center" />
                                                  </asp:BoundField>
                                                <asp:BoundField DataField="" HeaderText="時間" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" >
                                                  <HeaderStyle HorizontalAlign="Center" />
                                                  <ItemStyle HorizontalAlign="Center" />
                                                  </asp:BoundField>
                                                <asp:BoundField DataField="NL_User" HeaderText="維護經辦" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" >
                                                  <HeaderStyle HorizontalAlign="Center" />
                                                  <ItemStyle HorizontalAlign="Center" />
                                                  </asp:BoundField>
                                                <asp:BoundField DataField="NL_Type" HeaderText="作業類別" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" >
                                                  <HeaderStyle HorizontalAlign="Center" />
                                                  <ItemStyle HorizontalAlign="Center" />
                                                  </asp:BoundField>
                                                <asp:BoundField DataField="NL_Value" HeaderText="內容" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" >
                                                  <HeaderStyle HorizontalAlign="Center" />
                                                  <ItemStyle HorizontalAlign="Center" />
                                                  </asp:BoundField>
                                            </Columns>
                                            <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                            <PagerSettings Visible="False" />
                                            <RowStyle CssClass="Grid_Item" />
                                            <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                        </cc1:CustGridView>
                                        <cc1:GridPager ID="gpNoteLogList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right"
                                            OnPageChanged="gpNoteLogList_PageChanged">
                                        </cc1:GridPager>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <%-- Button 取消 - 確定 --%>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo10" style="">
                    <tr class="trOdd"> 
                        <td align="center" width="16%">
                            <%--確定--%>
                            <cc1:CustButton ID="btnApply" runat="server" CssClass="smallButton" ShowID="01_01090500_072"
                                 OnClientClick="return checkInputText('pnlText', 0);" OnClick="btnAdd_Click"
                                 DisabledWhenSubmit="False" BoxName="確定"/>&nbsp;     &nbsp;     &nbsp;     &nbsp;
                            <%--取消--%>
                            <cc1:CustButton ID="btnBack" runat="server" CssClass="smallButton" ShowID="01_01090500_073"
                                  OnClick="btnCancel_Click"
                                DisabledWhenSubmit="False" BoxName="取消" />
                        </td>
                    </tr>
                    <cc1:CustHiddenField ID="hidHCOP_LAST_UPD_MALER" runat="server" />
                    <cc1:CustHiddenField ID="hidHCOP_LAST_UPD_CHECKER" runat="server" />
                    <cc1:CustHiddenField ID="hidHCOP_LAST_UPD_BRANCH" runat="server" />
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
