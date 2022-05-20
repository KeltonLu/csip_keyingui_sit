<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010801150001.aspx.cs" Inherits="Page_P010801150001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <%-- 2020/11/23_Ares_Stanley-修正格式; 2021/01/21_Ares_Stanley-調整版面; 2021/03/10_Ares_Stanley-調整查詢條件textbox長度; 2021/03/26_Ares_Stanley-調整版面 --%>
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
        }

        function mustKeyIn() {
            var isSuccess = true;

        }
        $(function () {
            $("#HQlblHCOP_SIXM_TOT_AMT").click(function () {
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

        /*20191107-RQ-2018-015749-002*/
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
                <%--自然人收單AML資料定期審查案件明細(經辦)--%>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                    <tr class="itemTitle">
                        <td>
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01090400_001"></cc1:CustLabel>
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
                                        <cc1:CustLabel ID="HQlblCASE_NO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <%-- 客戶ID --%>
                                    <td align="right" width="4%">
                                        <cc1:CustLabel ID="customerId" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_003" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td colspan="4">
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
                                    <td style="width: 89%" colspan="7">
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
                <%-- Button --%>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo9" style="">
                    <tr>
                        <td>
                            <%-- 定審維護 --%>
                            <cc1:CustButton ID="BtnHQ_Modify" runat="server" CssClass="smallButton" ShowID="01_01090400_010"
                                     OnClientClick="return checkInputText('pnlText', 0);" OnClick="BtnHQ_Modify_Click"
                                    DisabledWhenSubmit="False" BoxName="定審維護" Style="margin-right:50px" />
                            <%-- SCDD --%>
                            <cc1:CustButton ID="Btn_SCCD_Modify" runat="server" CssClass="smallButton" ShowID="01_01090400_011"
                                     OnClientClick="return checkInputText('pnlText', 0);" OnClick="Btn_SCCD_Modify_Click"
                                    DisabledWhenSubmit="False" BoxName="SCDD" Style="margin-right:50px"/>
                            <%-- 異常結案 --%>
                            <cc1:CustButton ID="btnAbnormal" runat="server" CssClass="smallButton" ShowID="01_01090400_012"
                                     OnClientClick="return checkInputText('pnlText', 0);" OnClick="btnAbnormal_Click"
                                    DisabledWhenSubmit="False" BoxName="異常結案" Style="margin-right:50px"/>
                            <%-- 更新特店狀態 --%>
                            <cc1:CustButton ID="btnUpdateHQWork" runat="server" CssClass="smallButton" ShowID="01_01090400_013"
                                     OnClientClick="return checkInputText('pnlText', 0);" OnClick="btnUpdateHQWork_Click"
                                    DisabledWhenSubmit="False" BoxName="更新特電狀態" />
                        </td>
                    </tr>
                </table>
                
                <%-- 客戶資料 --%>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo2" style="table-layout:fixed">
                    <tr class="itemTitle">
                        <td colspan="8">
                            <li>
                                <cc1:CustLabel ID="title2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01090400_014"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>

                    <tr class="trEven">
                        <%--商店狀態--%>
                            <td align="right"   style="width: 11%">
                            <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080103_003" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 22%" >
                            <cc1:CustLabel ID="HQlblHCOP_STATUS" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>   
                        <%-- 身分證字號 --%>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="IDNum" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_015" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 22%" colspan="2">
                            <cc1:CustLabel ID="HQlblHCOP_OWNER_ID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <%-- 中文姓名 --%>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="chineseName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_016" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 22%" colspan="2" >
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
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_017" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 22%" colspan="2">
                            <cc1:CustLabel ID="HQlblHCOP_OWNER_ENGLISH_NAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <%-- 出生年月日 --%>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="Birthday" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01090400_018"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 22%" colspan="2">
                            <cc1:CustLabel ID="HQlblHCOP_OWNER_BIRTH_DATE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <%-- 性別 --%>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="sex" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_019" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 23%" >
                            <cc1:CustLabel ID="HCOP_GENDER" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            <cc1:CustHiddenField ID="HQlblHCOP_GENDER" runat="server" />
                        </td>
                    </tr>
                    <tr class="trEven" >
                        <%-- 國籍1 --%>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="country1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_020" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%" >                                        
                            <cc1:CustLabel ID="HQlblHCOP_OWNER_NATION" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <%-- 國籍2 --%>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="country2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_021" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 67%" colspan="5" >                                        
                            <cc1:CustLabel ID="HQlblHCOP_COUNTRY_CODE_2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>                                    
                    </tr>
                    <tr class="trOdd">
                            <%-- 身分證發證日期 --%>
                            <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="IdCardIssuanceDate" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01090400_022"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%" >
                            <cc1:CustLabel ID="HQlblHCOP_OWNER_ID_ISSUE_DATE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                            <%-- 發證地點 --%>
                            <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="issuanceLocation" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_023" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustLabel ID="HQlblHCOP_OWNER_ID_ISSUE_PLACE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                            <%-- 領補換類別 --%>
                            <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="category" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_024" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustLabel ID="HQlblHCOP_OWNER_ID_REPLACE_TYPE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                            <%-- 有無照片 --%>
                            <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="photos" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_025" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 23%">
                            <cc1:CustLabel ID="HQlblHCOP_ID_PHOTO_FLAG" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                        </tr>
                    <tr class="trEven">
                    <%-- 戶籍地址 --%>
                    <td align="right" style="width: 11%">
                        <cc1:CustLabel ID="residenceAddress" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                            SetBreak="False" SetOmit="False" ShowID="01_01090400_026" StickHeight="False"></cc1:CustLabel>
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
                    </td>
                </tr>
                    <tr class="trOdd">
                        <%-- 通訊地址 --%>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_070" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 89%" colspan="7">                                        
                            <cc1:CustLabel ID="HQlblHCOP_MAILING_CITY" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            <cc1:CustLabel ID="HQlblHCOP_MAILING_ADDR1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            <cc1:CustLabel ID="HQlblHCOP_MAILING_ADDR2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <%--E-MAIL--%>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblEmail" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_027" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width:89%" colspan="7">
                            <cc1:CustLabel ID="HQlblHCOP_EMAIL" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>                                        
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <%-- 連絡電話 --%>
                        <td align="right"  style="width: 11%">
                                <cc1:CustLabel ID="contactTel" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01090400_028"
                                    StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 39%" colspan="3">
                            <cc1:CustLabel ID="HQlblHCOP_COMP_TEL" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>                                        
                        </td>
                        <%-- 行動電話 --%>
                        <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="mobilePhone" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01090400_029"
                                    StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 39%" colspan="3">
                            <cc1:CustLabel ID="HQlblHCOP_MOBILE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                    </tr>
                                
                    <tr class="trEven">
                        <%-- 任職公司 --%>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="employedCompany" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_030" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 89%" colspan="7">
                            <cc1:CustLabel ID="HQlblHCOP_NP_COMPANY_NAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <%-- 行業別編號 --%>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="industryNumber" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_031" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 89% " colspan="7">
                            <cc1:CustLabel ID="labIndustryNum1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090400_032" StickHeight="False"></cc1:CustLabel>
                            <cc1:CustLabel ID="HQlblHCOP_CC" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                        
                            <cc1:CustLabel ID="labIndustryNum2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090400_033" StickHeight="False"></cc1:CustLabel>
                            <cc1:CustLabel ID="HQlblHCOP_CC_2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>

                            <cc1:CustLabel ID="labIndustryNum3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090400_034" StickHeight="False"></cc1:CustLabel>
                            <cc1:CustLabel ID="HQlblHCOP_CC_3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <%-- 職稱編號 --%>
                        <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="jobNum" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01090400_035"
                                    StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 89%" colspan="7">
                            <cc1:CustLabel ID="HQlblHCOP_OC" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel> 
                        </td>
                    </tr>
                    <tr class="trOdd" style="height:60px">
                        <%-- 收入及資產來源(複選) --%>
                        <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="income" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_036" StickHeight="False"></cc1:CustLabel>
                            </td >
                            <td  style="width: 89%" colspan="7">
                                <cc1:CustCheckBox ID="chkIncome1" runat="server"
                                AutoPostBack="false" BoxName="薪資" Enabled="false"/>
                                <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_037" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                <cc1:CustCheckBox ID="chkIncome2" runat="server"
                                AutoPostBack="false" BoxName="經營事業收入" Enabled="false"/>
                                <cc1:CustLabel ID="CustLabel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_038" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                <cc1:CustCheckBox ID="chkIncome3" runat="server"
                                AutoPostBack="false" BoxName="退休(職)資金" Enabled="false"/>
                                <cc1:CustLabel ID="CustLabel9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_039" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                <cc1:CustCheckBox ID="chkIncome4" runat="server"
                                AutoPostBack="false" BoxName="遺產繼承(含贈與)" Enabled="false"/>
                                <cc1:CustLabel ID="CustLabel10" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_040" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                <cc1:CustCheckBox ID="chkIncome5" runat="server"
                                AutoPostBack="false" BoxName="買賣房地產" Enabled="false"/>
                                <cc1:CustLabel ID="CustLabel11" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_041" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                <cc1:CustCheckBox ID="chkIncome6" runat="server"
                                AutoPostBack="false" BoxName="投資理財" Enabled="false"/>
                                <cc1:CustLabel ID="CustLabel12" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_042" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                <cc1:CustCheckBox ID="chkIncome7" runat="server"
                                AutoPostBack="false" BoxName="租金收入" Enabled="false"/>
                                <cc1:CustLabel ID="CustLabel13" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_043" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                <cc1:CustCheckBox ID="chkIncome8" runat="server"
                                AutoPostBack="false" BoxName="存款" Enabled="false"/>
                                <cc1:CustLabel ID="CustLabel14" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_074" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                <cc1:CustCheckBox ID="chkIncome9" runat="server"
                                AutoPostBack="false" BoxName="其他" Enabled="false"/>
                                <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01090400_044" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>
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
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01090400_045"></cc1:CustLabel>
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

                                <asp:GridView ID="gdvBRCHPerAMT" runat="server" AutoGenerateColumns="False" EnableModelValidation="True" BorderStyle="Solid" Width="100%" OnPreRender="gdvBRCHPerAMT_PreRender" >
                                    <AlternatingRowStyle CssClass="Grid_AlternatingItem" />
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
                                SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01090400_057"></cc1:CustLabel>
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
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_060" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                   
                                    <%-- 結果 --%>
                                    <td align="center" width="50%" bgcolor="#FF9900">
                                        <cc1:CustLabel ID="result" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090400_061" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                </tr>
                                <tr class="trOdd">
                                    <%-- 名單掃描案件編號(內容) --%>
                                    <td colspan="">
                                        <cc1:CustLabel ID="SCDDlblNameCheck_No" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <%-- 結果(內容) --%>
                                    <td colspan="">
                                        <cc1:CustLabel ID="SCDDlblNameCheck_Item" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                        <br>
                                        <cc1:CustLabel ID="SCDDlblNameCheck_Note" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
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
                                    <%-- 業發經辦(MFA) --%>
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
                                    <%-- 區域中心 --%>
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
                <cc1:CustButton ID="btnContactRecordEditor" runat="server" CssClass="smallButton" ShowID="01_01090400_065"
                         OnClientClick="return checkInputText('pnlText', 0);" OnClick="btnNOTEEdit_Click"
                        DisabledWhenSubmit="False" BoxName="聯絡記錄編輯" Style="margin-right:50px" />
                <%-- 案件歷程聯絡註記 --%>
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
                <%-- Button 返回 - 送審申請 --%>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo10" style="">
                    <tr class="trOdd"> 
                        <td align="center" width="16%">
                            <cc1:CustButton ID="btnBack" runat="server" CssClass="smallButton" ShowID="01_01090400_067"
                                 OnClientClick="return checkInputText('pnlText', 0);" OnClick="btnBack_Click"
                                DisabledWhenSubmit="False" BoxName="返回" />
                            &nbsp;     &nbsp;     &nbsp;     &nbsp;
                            <cc1:CustButton ID="btnApply" runat="server" CssClass="smallButton" ShowID="01_01090400_068"
                                 OnClientClick="return checkInputText('pnlText', 0);" OnClick="btnApply_Click"
                                 DisabledWhenSubmit="False" BoxName="送審申請"/>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
