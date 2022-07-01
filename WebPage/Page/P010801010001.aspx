<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010801010001.aspx.cs" Inherits="Page_P010801010001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%--修改紀錄:2021/01/25,27_Ares_Stanley-調整版面; 2021/03/11_Ares_Stanley-調整總公司及分公司年度請款金額顯示視窗--%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <%--20191106-RQ-2018-015749-002-增加可查詢每個月的請款金額--%>    
    
    <script type="text/javascript" language="javascript">
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
                autoOpen: false, width:1500, draggable: true
            });
            $("#myBrchModal").dialog('open');
        });
        $("#myBrchSpan").click(function () {
            //$("#myBrchModal").slideToggle("normal");
            $("#myBrchModal").dialog('close');
        });
    });
    </script>

    <script type="text/javascript" language="javascript"></script>
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
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                    <tr class="itemTitle">
                        <td colspan="6">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01080101_001"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblCardNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080101_002" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 12%">
                            <cc1:CustLabel ID="hlblCaseNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="right" style="width: 11%"><%--客戶ID(統編)： --%>
                            <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080101_003" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 12%">
                            <cc1:CustLabel ID="hlblHCOP_HEADQUATERS_CORP_NO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="right" style="width: 11%"><%--審查到期日--%>
                            <cc1:CustLabel ID="CustLabel12" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080101_006" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 12%">
                            <cc1:CustLabel ID="hlblCaseExpiryDate" runat="server" CurSymbol="£"></cc1:CustLabel>
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
                    <tr class="trOdd"><%--原本的風險等級：--%>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080101_004" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 12%">
                            <cc1:CustLabel ID="hlblOriginalRiskRanking" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            
                        </td>
                        <td align="right" style="width: 11%"><%--原本的下次審查日期 --%>
                            <cc1:CustLabel ID="CustLabel68" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080101_084" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 12%">
                            <cc1:CustLabel ID="hlblOriginalNextReviewDate" runat="server" CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td align="right" style="width: 11%"><%--案件種類--%>
                            <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080101_005" StickHeight="False"></cc1:CustLabel>                            
                        </td>
                        <td style="width: 12%">                            
                            <cc1:CustLabel ID="hlblCaseType" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 11%"><%--最新試算後的風險等級：--%>
                            <cc1:CustLabel ID="CustLabel73" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080101_085" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 12%">
                            <cc1:CustLabel ID="hlblNewRiskRanking" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="right" style="width: 11%"><%--最新試算後的下次審查日期--%>
                            <cc1:CustLabel ID="CustLabel14" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080101_007" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td colspan="3">
                            <cc1:CustLabel ID="hlblNewNextReviewDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                    </tr>
                </table>
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="60%" border="0" cellpadding="0" cellspacing="1" id="tabNo7">
                        <tr>
                            <td>
                                <%--審查維護--%>
                                <cc1:CustButton ID="BtnHQ_Modify" runat="server" CssClass="smallButton" ShowID="01_01080101_069"
                                    DisabledWhenSubmit="False" Enabled="true" OnClick="BtnHQ_Modify_Click" />
                                &nbsp;   &nbsp;
                            <%--    <cc1:CustButton ID="BtnHQ_SCCD_Modify" runat="server" CssClass="smallButton" ShowID="01_01080101_070"
                                    DisabledWhenSubmit="False" Enabled="true" OnClick="BtnHQ_SCCD_Modify_Click" />--%>
                                &nbsp;   &nbsp;
                                <%--分公司資訊維護--%>
                                <cc1:CustButton ID="btnBRCH_Modify" runat="server" CssClass="smallButton" ShowID="01_01080101_079"
                                    DisabledWhenSubmit="False" Enabled="true" OnClick="btnBRCH_Modify_Click"/>
                                &nbsp;   &nbsp;
                                &nbsp;   &nbsp;
                                <%--SCCD--%>
                                <cc1:CustButton ID="Btn_SCCD_Modify" runat="server" CssClass="smallButton" ShowID="01_01080101_071"
                                    DisabledWhenSubmit="False" Enabled="true" OnClick="Btn_SCCD_Modify_Click" />
                                &nbsp;   &nbsp;
                            <%--    <cc1:CustButton ID="Btn_EDD_Modify" runat="server" CssClass="smallButton" ShowID="01_01080101_072"
                                    DisabledWhenSubmit="False" Enabled="true" OnClick="Btn_EDD_Modify_Click" />--%>
                                &nbsp;   &nbsp;
                                <%--異常結案--%>
                                <cc1:CustButton ID="btnAbnormal" runat="server" CssClass="smallButton" ShowID="01_01080101_078"
                                    DisabledWhenSubmit="False" Enabled="true" OnClick="btnAbnormal_Click"/>
                                &nbsp;   &nbsp;
                                &nbsp;   &nbsp;
                                <%--更新特店狀態--%>
                                <cc1:CustButton ID="btnUpdateHQWork" runat="server" CssClass="smallButton" ShowID="01_01080101_080"
                                    DisabledWhenSubmit="False" Enabled="true" OnClick="btnUpdateHQWork_Click"/>
                                &nbsp;   &nbsp;
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo2" style="table-layout:fixed">
                        <tr class="itemTitle">
                            <td colspan="8">
                                <li>
                                    <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01080101_009"></cc1:CustLabel>
                                </li>
                            </td>
                        </tr>
                        <%--公司基本資料 L1 --%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_010" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_STATUS" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_011" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_REG_NAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_012" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_CORP_REG_ENG_NAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="20" NumOmit="0"
                                    SetBreak="True" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel11" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_013" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_BUILD_DATE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--公司基本資料 L2 --%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_014" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_CORP_TYPE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel10" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_015" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_REGISTER_NATION" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel15" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_016" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3" width="38%">
                                <cc1:CustLabel ID="HQlblHCOP_REGISTER_US_STATE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>

                        </tr>
                        <%--公司基本資料 L3 --%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_017" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_CC" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel16" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_018" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="5" style="width: 63%">
                                <cc1:CustLabel ID="HQlblHCOP_CC_Cname" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>

                        </tr>
                        <%--公司基本資料 L4 --%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel13" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_019" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="7" style="width: 88%">
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
                        <%--公司基本資料 L5 --%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel18" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_020" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="7" style="width: 88%">
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
                        <%--公司基本資料 L6 --%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel20" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_021" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="7" style="width: 88%">
                                <cc1:CustLabel ID="HQlblHCOP_EMAIL" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--公司基本資料 L7 --%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel17" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_022" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_BUSINESS_ORGAN_TYPE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel21" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_023" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_COMPLEX_STR_CODE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel23" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_024" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_ALLOW_ISSUE_STOCK_FLAG" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel25" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_025" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_ISSUE_STOCK_FLAG" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--公司基本資料 L8 --%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel19" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_026" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_OVERSEAS_FOREIGN" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel24" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_027" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_OVERSEAS_FOREIGN_COUNTRY" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel27" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_028" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 38%" colspan="3">
                                <cc1:CustLabel ID="HQlblHCOP_PRIMARY_BUSI_COUNTRY" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--公司基本資料 L9 --%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel22" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_029" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblWarningFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel28" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_030" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblGroupInformationSharingNameListflag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel30" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_031" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblIncorporated" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel33" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_032" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblIncorporatedDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--公司基本資料 L10 --%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel26" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_033" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblBlackListHitFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel31" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_034" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblPEPListHitFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel34" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_035" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblNNListHitFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel36" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_036" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblInternationalOrgPEP" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--公司基本資料 L11 --%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel29" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_037" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblDomesticPEP" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel35" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_038" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblForeignPEPStakeholder" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel38" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_039" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblInternationalOrgPEPStakeholder" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel40" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_040" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblDomesticPEPStakeholder" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--<tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel71" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_089" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblDormant_Flag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel76" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_090" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblDormant_Date" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel78" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_091" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblIncorporated_Source_System" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel80" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_092" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblAML_Last_Review_Date" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel82" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_093" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblRisk_Factor_PEP" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel84" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_094" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblRisk_Factor_RP_PEP" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel86" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_095" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblInternal_List_Flag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel88" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_096" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblHigh_Risk_Flag_Because_Rpty" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel90" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_097" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblHigh_Risk_Flag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%"></td>
                            <td style="width: 13%"></td>
                            <td style="width: 13%"></td>
                            <td style="width: 13%"></td>
                            <td style="width: 13%"></td>
                            <td style="width: 13%"></td>
                        </tr>--%>
                        <tr class="itemTitle">
                            <td colspan="8">
                                <li>
                                    <cc1:CustLabel ID="CustLabel32" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01080101_041"></cc1:CustLabel>
                                </li>
                            </td>
                        </tr>
                        <%--公司基本資料 L12 --%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel37" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_042" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_CONTACT_NAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel41" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_043" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--<td colspan="5" style="width: 63%">--%>
                            <td style="width: 12%">
                                <%--<cc1:CustLabel ID="HQlblHCOP_CONTACT_TEL" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>--%>
                                <cc1:CustLabel ID="HQlblHCOP_COMP_TEL" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%"><%--聯絡電話--%><%--RQ-2019-030155-002--%>
                                <cc1:CustLabel ID="CustLabel61" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_062" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3" style="width: 39%">
                                <cc1:CustLabel ID="HQlblHCOP_MOBILE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%-- 聯絡人-長姓名 --%>
                        <tr class="trOdd" runat="server" id ="cmpLname1" style="display:none">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel60" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_059" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="7">
                                <cc1:CustLabel ID="HQlblHCOP_CONTACT_LNAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>                            
                        </tr>
                        <%-- 聯絡人-羅馬拼音 --%>
                        <tr class="trOdd" runat="server" id ="cmpRname1" style="display:none">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel65" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_060" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="7">
                                <cc1:CustLabel ID="HQlblHCOP_CONTACT_ROMA" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>                            
                        </tr>
                    </table>
                    <%--公司負責人資料資料  --%>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo6" style="">
                        <tr class="itemTitle">
                            <td colspan="14">
                                <li>
                                    <cc1:CustLabel ID="CustLabel39" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01080101_058"></cc1:CustLabel>
                                </li>
                            </td>
                        </tr>
                        <%--公司負責人資料資料 HEADERLINE --%>
                        <tr class="trOdd">
                            <td align="center" style="width: 5%">
                                <cc1:CustLabel ID="CustLabel42" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_044" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 8%">
                                <cc1:CustLabel ID="CustLabel43" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_045" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 8%">
                                <cc1:CustLabel ID="CustLabel44" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_046" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel45" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_047" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 8%">
                                <cc1:CustLabel ID="CustLabel46" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_048" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel47" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_049" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel48" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_050" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel49" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_051" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--      <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel50" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_052" StickHeight="False"></cc1:CustLabel>
                            </td>--%>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel51" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_053" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel52" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_054" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel53" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_055" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel54" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_056" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel55" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_057" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel50" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_075" StickHeight="False"></cc1:CustLabel>
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
                                <cc1:CustLabel ID="HQlblHCOP_OWNER_BIRTH_DATE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 7%">
                                <cc1:CustLabel ID="HQlblHCOP_OWNER_ID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 8%">
                                <cc1:CustLabel ID="HQlblHCOP_OWNER_ID_ISSUE_DATE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 7%">
                                <cc1:CustLabel ID="HQlblHCOP_OWNER_ID_ISSUE_PLACE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 7%">
                                <cc1:CustLabel ID="HQlblHCOP_OWNER_ID_REPLACE_TYPE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 7%">
                                <cc1:CustLabel ID="HQlblHCOP_ID_PHOTO_FLAG" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%-- <td align="left" style="width: 7%">
                                <cc1:CustLabel ID="HQlblOWNER_ID_Type" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>--%>
                            <td align="left" style="width: 7%">
                                <cc1:CustLabel ID="HQlblHCOP_OWNER_NATION" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 7%">
                                <cc1:CustLabel ID="HQlblHCOP_PASSPORT" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 7%">
                                <cc1:CustLabel ID="HQlblHCOP_PASSPORT_EXP_DATE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 7%">
                                <cc1:CustLabel ID="HQlblHCOP_RESIDENT_NO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 7%">
                                <cc1:CustLabel ID="HQlblHCOP_RESIDENT_EXP_DATE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%"><%--年度請款金額--%>
                                <cc1:CustLabel ID="HQlblHCOP_SIXM_TOT_AMT" runat="server" CurAlign="Right" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Font-Bold="true" CssClass="pointer" ></cc1:CustLabel>
                                <%--20191106-RQ-2018-015749-002  請款金額--%>
                                <!-- The Modal -->
                                <div id="myModal" class="modal" style="display:none;">
                                  <!-- Modal content -->
                                  <div class="modal-content">
                                      <span  id="mySpan" class="close">&times;</span>
                                      <div class="modal-header" >
                                      <p style="text-align:center;">總公司-月請款金額</p>
                                    </div>                                    
                                    <asp:GridView ID="gdvHCOPPerAMT" runat="server" AutoGenerateColumns="False" EnableModelValidation="True" BorderStyle="Solid" OnPreRender="gdvHCOPPerAMT_PreRender" >
                                        <AlternatingRowStyle CssClass="Grid_AlternatingItem" />
                                        <Columns>
                                            <asp:BoundField HeaderText="統一編號" DataField="HCOP_HEADQUATERS_CORP_NO">
                                            <ItemStyle Width="90px" HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="1月<br />請款金額" DataField="HCOP_MON_AMT1" DataFormatString="{0:N0}" HtmlEncode="False"  >
                                            <ItemStyle Width="90px" HorizontalAlign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="2月<br />請款金額" DataField="HCOP_MON_AMT2" DataFormatString="{0:N}" HtmlEncode="False" >
                                            <ItemStyle Width="90px" HorizontalAlign="Right"  />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="3月<br />請款金額" DataField="HCOP_MON_AMT3" DataFormatString="{0:N}" HtmlEncode="False" >
                                            <ItemStyle Width="90px" HorizontalAlign="Right"  />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="4月<br />請款金額" DataField="HCOP_MON_AMT4" DataFormatString="{0:N}" HtmlEncode="False" >
                                            <ItemStyle Width="90px" HorizontalAlign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="5月<br />請款金額" DataField="HCOP_MON_AMT5" DataFormatString="{0:N}" HtmlEncode="False" >
                                            <ItemStyle Width="90px" HorizontalAlign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="6月<br />請款金額" DataField="HCOP_MON_AMT6" DataFormatString="{0:N}" HtmlEncode="False" >
                                            <ItemStyle Width="90px" HorizontalAlign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="7月<br />請款金額" DataField="HCOP_MON_AMT7" DataFormatString="{0:N}" HtmlEncode="False" >
                                            <ItemStyle Width="90px" HorizontalAlign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="8月<br />請款金額" DataField="HCOP_MON_AMT8" DataFormatString="{0:N}" HtmlEncode="False" >
                                            <ItemStyle Width="90px" HorizontalAlign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="9月<br />請款金額" DataField="HCOP_MON_AMT9" DataFormatString="{0:N}" HtmlEncode="False" >
                                            <ItemStyle Width="90px" HorizontalAlign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="10月<br />請款金額" DataField="HCOP_MON_AMT10" DataFormatString="{0:N}" HtmlEncode="False" >
                                            <ItemStyle Width="90px" HorizontalAlign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="11月<br />請款金額" DataField="HCOP_MON_AMT11" DataFormatString="{0:N}" HtmlEncode="False" >
                                            <ItemStyle Width="90px" HorizontalAlign="Right" />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="12月<br />請款金額" DataField="HCOP_MON_AMT12" DataFormatString="{0:#,#}" HtmlEncode="False" >
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
                        
                        <%--公司負責人長姓名 --%>
                        <tr class="trOdd" runat="server" id ="cmpLname" style="display:none">
                            <td align="right" colspan="2" >
                                <cc1:CustLabel ID="lblHCOP_LNAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_059" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="12">
                                  <cc1:CustLabel ID="HQlblHCOP_OWNER_CHINESE_LNAME" runat="server" CurAlign="Right" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>                            
                        </tr>
                         <tr class="trOdd" runat="server" id ="cmpRname" style="display:none">
                            <td align="right" colspan="2" >
                                <cc1:CustLabel ID="lblHCOP_ROMA" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_060" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="12">
                                <cc1:CustLabel ID="HQlblHCOP_OWNER_ROMA" runat="server" CurAlign="Right" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>                            
                        </tr>
                    </table>
                    <%--分公司負責人資料資料  --%>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo8" style="table-layout:fixed">
                        <tr class="itemTitle">
                            <td>
                                <li>
                                    <cc1:CustLabel ID="CustLabel56" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01080101_059"></cc1:CustLabel>
                                </li>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <cc1:CustGridView ID="grvCardData" runat="server" BorderStyle="Solid" EnableModelValidation="True" PagerID="GridPager1" Width="100%" OnRowDataBound="grvCardData_RowDataBound" OnRowCommand="grvCardData_RowCommand" Class="longTableGridView">
                                    <AlternatingRowStyle CssClass="Grid_AlternatingItem" />
                                    <Columns>
                                        <asp:BoundField DataField="BRCH_BRCH_NO" HeaderText="分公司/分店統一編號" HeaderStyle-CssClass="whiteSpaceNormal" ItemStyle-CssClass="whiteSpaceNormal" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="REG_CHI_NAME" HeaderText="分公司/分店登記名稱" HeaderStyle-CssClass="whiteSpaceNormal" ItemStyle-CssClass="whiteSpaceNormal" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="BRCH_CHINESE_NAME" HeaderText="姓名(中)">
                                            <ItemStyle CssClass="whiteSpaceNormal" HorizontalAlign="Center" Width="90px" />
                                            <HeaderStyle CssClass="whiteSpaceNormal" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="BRCH_ENGLISH_NAME" HeaderText="姓名(英)" HeaderStyle-CssClass="whiteSpaceNormal" ItemStyle-CssClass="whiteSpaceNormal" ItemStyle-HorizontalAlign="Center">
                                            <ItemStyle Width="80px" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="BRCH_BIRTH_DATE" HeaderText="出生日期" HeaderStyle-CssClass="whiteSpaceNormal" ItemStyle-CssClass="whiteSpaceNormal" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="BRCH_ID" HeaderText="身分證號碼" HeaderStyle-CssClass="whiteSpaceNormal" HeaderStyle-Width="7%" ItemStyle-CssClass="whiteSpaceNormal" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="BRCH_OWNER_ID_ISSUE_DATE" HeaderText="身分證發證日期" HeaderStyle-CssClass="whiteSpaceNormal" HeaderStyle-Width="85px" ItemStyle-CssClass="whiteSpaceNormal" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="BRCH_OWNER_ID_ISSUE_PLACE" HeaderText="發證地點" HeaderStyle-CssClass="whiteSpaceNormal" ItemStyle-CssClass="whiteSpaceNormal" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="BRCH_OWNER_ID_REPLACE_TYPE" HeaderText="領補換類別" HeaderStyle-CssClass="whiteSpaceNormal" ItemStyle-CssClass="whiteSpaceNormal" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="BRCH_ID_PHOTO_FLAG" HeaderText="有無照片" HeaderStyle-CssClass="whiteSpaceNormal" ItemStyle-CssClass="whiteSpaceNormal" ItemStyle-HorizontalAlign="Center" />
                                        <%--<asp:BoundField DataField="" HeaderText="身分證件類型" />--%>
                                        <asp:BoundField DataField="BRCH_NATION" HeaderText="國籍" HeaderStyle-CssClass="whiteSpaceNormal" ItemStyle-CssClass="whiteSpaceNormal" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="BRCH_PASSPORT" HeaderText="護照號碼" HeaderStyle-CssClass="whiteSpaceNormal" ItemStyle-CssClass="whiteSpaceNormal" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="BRCH_PASSPORT_EXP_DATE" HeaderText="護照效期" HeaderStyle-CssClass="whiteSpaceNormal" ItemStyle-CssClass="whiteSpaceNormal" ItemStyle-HorizontalAlign="Center" />
                                        <%--20200410-RQ-2019-030155-005-居留證號更名為統一證號--%>
                                        <asp:BoundField DataField="BRCH_RESIDENT_NO" HeaderText="統一證號" HeaderStyle-CssClass="whiteSpaceNormal" HeaderStyle-Width="7%" ItemStyle-CssClass="whiteSpaceNormal" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="BRCH_RESIDENT_EXP_DATE" HeaderText="統一證號效期" HeaderStyle-CssClass="whiteSpaceNormal" ItemStyle-CssClass="whiteSpaceNormal" ItemStyle-HorizontalAlign="Center" />
                                        
                                        <%--<asp:BoundField DataField="BRCH_SIXM_TOT_AMT" HeaderText="年度請款金額" ItemStyle-HorizontalAlign="Right" />--%>
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

                                    <asp:GridView ID="gdvBRCHPerAMT" runat="server" AutoGenerateColumns="False" EnableModelValidation="True" BorderStyle="Solid" Width="100%" >
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
                    <%--高階管理人暨實質受益人資料明細表  --%>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo3" style="">
                        <tr class="itemTitle">
                            <td>
                                <li>
                                    <cc1:CustLabel ID="CustLabel57" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01080101_060"></cc1:CustLabel>
                                </li>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <cc1:CustGridView ID="grvManagerData" runat="server" BorderStyle="Solid" EnableModelValidation="True" PagerID="GridPager1" Width="100%" OnRowDataBound="grvManagerData_RowDataBound">
                                    <AlternatingRowStyle CssClass="Grid_AlternatingItem" />
                                    <Columns>                                        
                                        <%--<asp:TemplateField HeaderText="姓名" ShowHeader="False">
                                           <ItemTemplate>
                                             <asp:Label ID="ProductIDLabel" runat="server" Text='<%# Eval("HCOP_BENE_NAME") %>' />                                               
                                           </ItemTemplate>
                                       </asp:TemplateField>--%>
                                        <asp:BoundField DataField="HCOP_BENE_NAME" HeaderText="姓名"  ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="HCOP_BENE_BIRTH_DATE" HeaderText="出生日期"  ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="HCOP_BENE_ID" HeaderText="身分證號碼"  ItemStyle-HorizontalAlign="Center"/>
                                        <asp:BoundField DataField="HCOP_BENE_NATION" HeaderText="國籍"  ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="HCOP_BENE_PASSPORT" HeaderText="護照號碼"  ItemStyle-HorizontalAlign="Center"/>
                                        <%--<asp:BoundField DataField="HCOP_BENE_PASSPORT_EXP" HeaderText="護照效期"  ItemStyle-HorizontalAlign="Center"/>--%>
                                        <asp:BoundField DataField="HCOP_BENE_RESIDENT_NO" HeaderText="統一證號"  ItemStyle-HorizontalAlign="Center"/><%--20200410-RQ-2019-030155-005-居留證號更名為統一證號--%>
                                        <%--<asp:BoundField DataField="HCOP_BENE_RESIDENT_EXP" HeaderText="居留證效期"  ItemStyle-HorizontalAlign="Center"/>--%>
                                        <asp:BoundField DataField="HCOP_BENE_OTH_CERT" HeaderText="其他證號"  ItemStyle-HorizontalAlign="Center"/>
                                        <asp:BoundField DataField="" HeaderText="身分類型"  ItemStyle-HorizontalAlign="Left"/>
                                    </Columns>
                                    <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                    <PagerSettings Visible="False" />
                                    <RowStyle CssClass="Grid_Item" />
                                    <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                </cc1:CustGridView>
                            </td>
                        </tr>
                    </table>
                    <br>
                    <%--SCDD情報 --%>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo4">
                        <tr class="trOdd">
                            <td align="right" style="width: 16%" bgcolor="#FF9900">
                                <cc1:CustLabel ID="CustLabel59" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_061" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%">
                                <cc1:CustLabel ID="lblSR_DateTime" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 16%" bgcolor="#FF9900">
                                <cc1:CustLabel ID="CustLabel58" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_062" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%">
                                <cc1:CustLabel ID="lblSR_RiskLevel" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 16%" bgcolor="#FF9900">
                                <cc1:CustLabel ID="CustLabel62" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_063" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 18%">
                                <cc1:CustLabel ID="CustLabel63" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <%--SCDD情報 --%>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo5">
                        <tr class="itemTitle">
                            <td colspan="2" align="center">
                                <cc1:CustLabel ID="CustLabel70" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01080101_064"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 20%" bgcolor="#FF9900">
                                <cc1:CustLabel ID="CustLabel66" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_065" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 80%" bgcolor="#FF9900">
                                <cc1:CustLabel ID="CustLabel67" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_066" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 20%">
                                <cc1:CustLabel ID="SCDDlblNameCheck_No" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 80%">
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
                    <%--MFA資訊 --%>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1">
                        <tr class="itemTitle">
                            <td colspan="4">
                                <li>
                                    <cc1:CustLabel ID="CustLabel74" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01080101_083"></cc1:CustLabel>
                                </li>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 16%" bgcolor="#FF9900"><%--業發經辦(MFA)--%>
                                <cc1:CustLabel ID="CustLabel69" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_081" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 34%">
                                <cc1:CustLabel ID="HQlblMFAF_NAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 16%" bgcolor="#FF9900"><%--區域中心--%>
                                <cc1:CustLabel ID="CustLabel72" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_082" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 34%">
                                <cc1:CustLabel ID="HQlblMFAF_AREA" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                    </table>
                    <cc1:CustButton ID="btnNOTEEdit" runat="server" CssClass="smallButton" ShowID="01_01080101_067"
                        DisabledWhenSubmit="False" Enabled="true" OnClick="btnNOTEEdit_Click" />
                    <%--案件歷程聯絡註記 --%>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo10">
                        <tr class="itemTitle">
                            <td align="center">
                                <cc1:CustLabel ID="CustLabel64" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01080101_068"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <cc1:CustGridView ID="grdNoteLog" runat="server" BorderStyle="Solid" EnableModelValidation="True" PagerID="GridPager1" Width="100%" OnRowDataBound="grdNoteLog_RowDataBound">
                                    <AlternatingRowStyle CssClass="Grid_AlternatingItem" />
                                    <Columns>
                                        <asp:BoundField DataField="" HeaderText="日期" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="" HeaderText="時間" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="NL_User" HeaderText="維護經辦" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="NL_Type" HeaderText="作業類別" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                        <%-- 20220620 調整歷程註記可以顯示換行 By Kelton --%>
<%--                                        <asp:BoundField DataField="NL_Value" HeaderText="內容" HeaderStyle-HorizontalAlign="Center">
                                            <ItemStyle Width="500px" />
                                        </asp:BoundField>--%>
                                        <asp:BoundField DataField="NL_Value" HeaderText="內容" HeaderStyle-HorizontalAlign="Center" HtmlEncode="false">
                                            <ItemStyle Width="500px" />
                                        </asp:BoundField>
                                    </Columns>
                                    <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                    <PagerSettings Visible="False" />
                                    <RowStyle CssClass="Grid_Item" />
                                    <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                </cc1:CustGridView>
                                <cc1:GridPager ID="gpNoteLogList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Center"
                                    OnPageChanged="gpNoteLogList_PageChanged">
                                </cc1:GridPager>

                            </td>
                        </tr>
                    </table>
                    <br />
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo9" style="">
                        <tr class="trOdd">
                            <td align="center">
                                <cc1:CustButton ID="btnBack" runat="server" CssClass="smallButton" ShowID="01_01080101_073"
                                    onkeydown="movefocus();" DisabledWhenSubmit="False" OnClick="btnBack_Click" />
                                &nbsp;     &nbsp;     &nbsp;     &nbsp;
                                <cc1:CustButton ID="btnApply" runat="server" CssClass="smallButton" ShowID="01_01080101_074"
                                    DisabledWhenSubmit="False"
                                    onkeydown="movefocus();" OnClick="btnApply_Click" />
                            </td>

                        </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>

    </form>
</body>
</html>
