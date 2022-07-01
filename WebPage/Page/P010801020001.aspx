<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010801020001.aspx.cs" Inherits="Page_P010801020001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%-- 修改紀錄:2021/01/25_Ares_Stanley-調整版面 --%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript"> </script>
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
        &nbsp;
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                    <tr class="itemTitle">
                        <td colspan="6">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01080102_001"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblCardNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080102_002" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 12%">
                            <cc1:CustLabel ID="hlblCaseNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080102_003" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 12%">
                            <cc1:CustLabel ID="hlblHCOP_HEADQUATERS_CORP_NO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="right" style="width: 11%"><%--審查到期日--%>
                            <cc1:CustLabel ID="CustLabel12" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080102_006" StickHeight="False"></cc1:CustLabel>
                        </td>

                        <td style="width: 12%">
                            <cc1:CustLabel ID="hlblCaseExpiryDate" runat="server" CurSymbol="£"></cc1:CustLabel>                            
                        </td>
                    </tr>
                    <%--新增案件編號--%>
                    <%--//20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis--%>
                    <%--<tr class="trOdd">
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="CustLabel79" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080102_082" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 89%" colspan="7">
                            <cc1:CustLabel ID="hlblRelateCaseNo" runat="server" CurSymbol="£"></cc1:CustLabel>
                        </td>
                    </tr>--%>
                    <%--//20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis--%>
                    <tr class="trOdd">
                        <td align="right" style="width: 11%"><%--原本的風險等級--%>
                            <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080102_004" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 12%">
                            <cc1:CustLabel ID="hlblOriginalRiskRanking" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="right" style="width: 11%"><%--原本的下次審查日期--%>
                            <cc1:CustLabel ID="CustLabel71" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080102_080" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 12%">
                            <cc1:CustLabel ID="hlblOriginalNextReviewDate" runat="server" CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td align="right" style="width: 11%"><%--案件種類--%>
                            <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080102_005" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 12%">
                            <cc1:CustLabel ID="hlblCaseType" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>                         
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 11%"><%--最新試算後的風險等級--%>
                            <cc1:CustLabel ID="CustLabel73" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080102_081" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 12%">
                            <cc1:CustLabel ID="hlblNewRiskRanking" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="right" style="width: 11%"><%--最新試算後的下次審查日期--%>
                            <cc1:CustLabel ID="CustLabel14" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080102_007" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td colspan="3">
                            <cc1:CustLabel ID="hlblNewNextReviewDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                    </tr>
                </table>
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                     <table width="30%" border="0" cellpadding="0" cellspacing="1" id="tabNo7">
                        <tr>
                           
                            <td>
                                <cc1:CustButton ID="Btn_SCCD_View" runat="server" CssClass="smallButton" ShowID="01_01080102_071"
                                    DisabledWhenSubmit="False" Enabled="true" OnClick="Btn_SCCD_View_Click"   onkeydown="movefocus();"  />
                           &nbsp;  &nbsp;
                            <%--    <cc1:CustButton ID="Btn_EDD_View" runat="server" CssClass="smallButton" ShowID="01_01080102_072"
                                    DisabledWhenSubmit="False" Enabled="true" OnClick="Btn_EDD_View_Click" />--%>
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo2" style="table-layout:fixed">
                        <tr class="itemTitle">
                            <td colspan="8">
                                <li>
                                    <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01080102_009"></cc1:CustLabel>
                                </li>
                            </td>
                        </tr>
                        <%--公司基本資料 L1 --%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_010" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_STATUS" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_011" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_REG_NAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_012" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_CORP_REG_ENG_NAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="20" NumOmit="0"
                                    SetBreak="True" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel11" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_013" StickHeight="False"></cc1:CustLabel>
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
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_014" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_CORP_TYPE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel10" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_015" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_REGISTER_NATION" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel15" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_016" StickHeight="False"></cc1:CustLabel>
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
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_017" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_CC" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel16" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_018" StickHeight="False"></cc1:CustLabel>
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
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_019" StickHeight="False"></cc1:CustLabel>
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
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_020" StickHeight="False"></cc1:CustLabel>
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
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_021" StickHeight="False"></cc1:CustLabel>
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
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_022" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_BUSINESS_ORGAN_TYPE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel21" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_023" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_COMPLEX_STR_CODE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel23" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_024" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_ALLOW_ISSUE_STOCK_FLAG" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel25" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_025" StickHeight="False"></cc1:CustLabel>
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
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_026" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_OVERSEAS_FOREIGN" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel24" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_027" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_OVERSEAS_FOREIGN_COUNTRY" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel27" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_028" StickHeight="False"></cc1:CustLabel>
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
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_029" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblWarningFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel28" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_030" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblGroupInformationSharingNameListflag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel30" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_031" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblIncorporated" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel33" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_032" StickHeight="False"></cc1:CustLabel>
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
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_033" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblBlackListHitFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel31" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_034" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblPEPListHitFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel34" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_035" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblNNListHitFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel36" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_036" StickHeight="False"></cc1:CustLabel>
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
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_037" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblDomesticPEP" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel35" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_038" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblForeignPEPStakeholder" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel38" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_039" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblInternationalOrgPEPStakeholder" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel40" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_040" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblDomesticPEPStakeholder" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--<tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel76" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080101_089" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="CDlblDormant_Flag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel77" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
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
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01080102_041"></cc1:CustLabel>
                                </li>
                            </td>
                        </tr>
                        <%--公司基本資料 L12 --%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel37" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_042" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 13%">
                                <cc1:CustLabel ID="HQlblHCOP_CONTACT_NAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel41" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_043" StickHeight="False"></cc1:CustLabel>
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
                                <cc1:CustLabel ID="CustLabel75" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_062" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3" style="width: 39%">
                                <cc1:CustLabel ID="HQlblHCOP_MOBILE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </tr>
                          <%-- 聯絡人-長姓名 --%>
                        <tr class="trOdd"  runat="server" id ="cmpLname1" style="display:none">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel65" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
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
                        <tr class="trOdd"  runat="server" id ="cmpRname1" style="display:none">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="CustLabel68" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
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
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo3" style="">
                        <tr class="itemTitle">
                            <td colspan="14">
                                <li>
                                    <cc1:CustLabel ID="CustLabel39" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01080102_058"></cc1:CustLabel>
                                </li>
                            </td>
                        </tr>
                        <%--公司負責人資料資料 HEADERLINE --%>
                        <tr class="trOdd">
                            <td align="center" style="width: 5%">
                                <cc1:CustLabel ID="CustLabel42" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_044" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 8%">
                                <cc1:CustLabel ID="CustLabel43" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_045" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 8%">
                                <cc1:CustLabel ID="CustLabel44" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_046" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel45" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_047" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 8%">
                                <cc1:CustLabel ID="CustLabel46" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_048" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel47" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_049" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel48" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_050" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel49" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_051" StickHeight="False"></cc1:CustLabel>
                            </td>
                           <%-- <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel50" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_052" StickHeight="False"></cc1:CustLabel>
                            </td>--%>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel51" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_053" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel52" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_054" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel53" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_055" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel54" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_056" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel55" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_057" StickHeight="False"></cc1:CustLabel>
                            </td>
                             <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel50" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_077" StickHeight="False"></cc1:CustLabel>
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
                            <%--<td align="left" style="width: 7%">
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
                                <td align="left" style="width: 7%">
                                <cc1:CustLabel ID="HQlblHCOP_SIXM_TOT_AMT" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                          <%--公司負責人長姓名 --%>
                        <tr class="trOdd">
                            <td align="right" colspan="2"  runat="server" id ="cmpLname" style="display:none" >
                                <cc1:CustLabel ID="lblHCOP_LNAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_059" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="14">
                                  <cc1:CustLabel ID="HQlblHCOP_OWNER_CHINESE_LNAME" runat="server" CurAlign="Right" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>                            
                        </tr>
                         <tr class="trOdd">
                            <td align="right" colspan="2"  runat="server" id ="cmpRname" style="display:none">
                                <cc1:CustLabel ID="lblHCOP_ROMA" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080103_060" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" colspan="14">
                                <cc1:CustLabel ID="HQlblHCOP_OWNER_ROMA" runat="server" CurAlign="Right" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>                            
                        </tr>
                    </table>
                    <%--分公司負責人資料資料  --%>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo3" style="">
                        <tr class="itemTitle">
                            <td>
                                <li>
                                    <cc1:CustLabel ID="CustLabel56" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01080102_059"></cc1:CustLabel>
                                </li>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <cc1:CustGridView ID="grvCardData" runat="server" BorderStyle="Solid" EnableModelValidation="True" PagerID="GridPager1" Width="100%" OnRowDataBound="grvCardData_RowDataBound"  >
                                    <AlternatingRowStyle CssClass="Grid_AlternatingItem" />
                                    <Columns>
                                        <asp:BoundField DataField="BRCH_BRCH_NO" HeaderText="分公司/分店統一編號" /> 
                                        <asp:BoundField DataField="REG_CHI_NAME" HeaderText="分公司/分店登記名稱" />
                                        <asp:BoundField DataField="BRCH_CHINESE_NAME" HeaderText="姓名(中)">
                                            <ItemStyle Width="60px" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="BRCH_ENGLISH_NAME" HeaderText="姓名(英)">
                                            <ItemStyle Width="80px" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="BRCH_BIRTH_DATE" HeaderText="出生日期" />
                                        <asp:BoundField DataField="BRCH_ID" HeaderText="身分證號碼" />
                                        <asp:BoundField DataField="BRCH_OWNER_ID_ISSUE_DATE" HeaderText="身分證發證日期" />
                                        <asp:BoundField DataField="BRCH_OWNER_ID_ISSUE_PLACE" HeaderText="發證地點" />
                                        <asp:BoundField DataField="BRCH_OWNER_ID_REPLACE_TYPE" HeaderText="領補換類別" />
                                        <asp:BoundField DataField="BRCH_ID_PHOTO_FLAG" HeaderText="有無照片" />
                                        <%--<asp:BoundField DataField="" HeaderText="身分證件類型" />--%>
                                        <asp:BoundField DataField="BRCH_NATION" HeaderText="國籍" />
                                        <asp:BoundField DataField="BRCH_PASSPORT" HeaderText="護照號碼" />
                                        <asp:BoundField DataField="BRCH_PASSPORT_EXP_DATE" HeaderText="護照效期" />
                                        <%--20200410-RQ-2019-030155-005-居留證號更名為統一證號--%>
                                        <asp:BoundField DataField="BRCH_RESIDENT_NO" HeaderText="統一證號" />
                                        <asp:BoundField DataField="BRCH_RESIDENT_EXP_DATE" HeaderText="統一證號效期" />
                                        
                                        <asp:BoundField DataField="BRCH_SIXM_TOT_AMT" HeaderText="年度請款金額" /> 
                                    </Columns>
                                    <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                    <PagerSettings Visible="False" />
                                    <RowStyle CssClass="Grid_Item" />
                                    <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                </cc1:CustGridView>

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
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01080102_060"></cc1:CustLabel>
                                </li>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <cc1:CustGridView ID="grvManagerData" runat="server" BorderStyle="Solid" EnableModelValidation="True" PagerID="GridPager1" Width="100%" OnRowDataBound="grvManagerData_RowDataBound"  >
                                    <AlternatingRowStyle CssClass="Grid_AlternatingItem" />
                                    <Columns>
                                        <asp:BoundField DataField="HCOP_BENE_NAME" HeaderText="姓名">
                                            <ItemStyle Width="120px" />
                                        </asp:BoundField>
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
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_061" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%">
                                <cc1:CustLabel ID="lblSR_DateTime" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 16%" bgcolor="#FF9900">
                                <cc1:CustLabel ID="CustLabel58" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_062" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%">
                                <cc1:CustLabel ID="lblSR_RiskLevel" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 16%" bgcolor="#FF9900">
                                <cc1:CustLabel ID="CustLabel62" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_063" StickHeight="False"></cc1:CustLabel>
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
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01080102_064"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 20%" bgcolor="#FF9900">
                                <cc1:CustLabel ID="CustLabel66" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_065" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 80%" bgcolor="#FF9900">
                                <cc1:CustLabel ID="CustLabel67" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080102_066" StickHeight="False"></cc1:CustLabel>
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
                         <%--案件歷程註記 --%>
                      <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo12">
                        <tr class="itemTitle">
                            <td align="left" colspan="2">
                                <cc1:CustLabel ID="CustLabel60" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False"></cc1:CustLabel>
                                <%--SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01080102_078"></cc1:CustLabel>--%>
                            </td>
                        </tr>
                          <tr class="trOdd">
                               <td style="width: 18%">
                                <cc1:CustLabel ID="CustLabel61" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="01_01080102_079"></cc1:CustLabel>
                            </td>
                                 <td style="width:82%">
                              <%--    <cc1:CustTextBox ID="txtNotelogValue" runat="server" MaxLength="1000"  Width="100%" 
                               BoxName="聯絡內容" Height="143px" InputType="String" Rows="5"  ></cc1:CustTextBox>--%>
                                     <asp:TextBox TextMode="MultiLine" ID="txtNotelogValue" runat="server" MaxLength="1000"  Width="100%"
                                         Height="143px" ></asp:TextBox>
                           </td>
                          </tr>

                      </table>
                      <%--案件 退回、放行、呈上階主管 --%>
                   <table width="100%" border="0" cellpadding="0" cellspacing="0" id="tabNo11" style="">
                        <tr class="trOdd">
                            <td align="center" style="width:10%;">
                                <cc1:CustButton ID="btnReject" runat="server" CssClass="smallButton" ShowID="01_01080102_074"
                                   onkeydown="movefocus();"    DisabledWhenSubmit="False" OnClick="btnReject_Click" />
                             </td>
                            <td style="width:20%;">
                                <cc1:CustButton ID="btnFinish" runat="server" CssClass="smallButton" ShowID="01_01080102_075"
                                    DisabledWhenSubmit="False"
                                    onkeydown="movefocus();" OnClick="btnFinish_Click" />
                            &nbsp;  &nbsp;  &nbsp;
                                <cc1:CustButton ID="btnApply" runat="server" CssClass="smallButton" ShowID="01_01080102_076"
                                    DisabledWhenSubmit="False"
                                    onkeydown="movefocus();" OnClick="btnApply_Click" />
                            </td>
                            <td style="width:70%;"></td>
                        </tr>
                    </table>
                    <%--案件歷程聯絡註記 --%>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo10">
                        <tr class="itemTitle">
                            <td align="center">
                                <cc1:CustLabel ID="CustLabel64" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01080102_068"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <cc1:CustGridView ID="grdNoteLog" runat="server" BorderStyle="Solid" EnableModelValidation="True" PagerID="GridPager1" Width="100%" OnRowDataBound="grdNoteLog_RowDataBound" >
                                    <AlternatingRowStyle CssClass="Grid_AlternatingItem" />
                                    <Columns>
                                        <asp:BoundField DataField="" HeaderText="日期" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"  />
                                        <asp:BoundField DataField="" HeaderText="時間" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="NL_User" HeaderText="維護經辦" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="NL_Type" HeaderText="作業類別" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                        <%-- 20220620 調整歷程註記可以顯示換行 By Kelton --%>
                                        <%--<asp:BoundField DataField="NL_Value" HeaderText="內容" HeaderStyle-HorizontalAlign="Center">
                                            <ItemStyle Width="320px" />
                                        </asp:BoundField>--%>
                                        <asp:BoundField DataField="NL_Value" HeaderText="內容" HeaderStyle-HorizontalAlign="Center" HtmlEncode="false">
                                            <ItemStyle Width="320px" />
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
                        <tr  >
                            <td align="center">
                                <cc1:CustButton ID="btnBack" runat="server" CssClass="smallButton" ShowID="01_01080101_073"
                                   onkeydown="movefocus();"    DisabledWhenSubmit="False" OnClick="btnBack_Click" /> 
                            </td>

                        </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
