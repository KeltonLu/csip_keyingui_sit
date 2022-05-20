<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010801170101.aspx.cs" Inherits="P010801170101" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">

    protected void grvCardData_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
</script>

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
    </script> 
   
    <style type="text/css">
        .auto-style1 {
            height: 116px;
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
                <%-- 客戶資料 --%>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                    <tr class="itemTitle">
                        <td>
                            <li>
                                <cc1:CustLabel ID="title1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01090400_014"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    
                    <tr>
                        <td style="width: 100%" colspan="1">
                            <table border="0" width="100%" cellspacing="1">
                                <tr class="trEven">
                                    <asp:HiddenField ID="hidAML_HQ_Work_CASE_NO" runat="server" />
                                    <%-- 戶名 --%>
                                    <td align="right">
                                        <cc1:CustLabel ID="householdName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090600_001" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td>
                                        <cc1:CustLabel ID="HQlblHCOP_OWNER_CHINESE_NAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <%-- 身分證字號 --%>
                                    <td align="right">
                                        <cc1:CustLabel ID="IDNum" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090600_002" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style="width: 10%" colspan="3">
                                        <cc1:CustLabel ID="HQlblHCOP_OWNER_ID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    
                                </tr>
                                <tr class="trOdd">
                                    <%-- 戶籍地址 --%>
                                    <td align="right" style="width: 11%">
                                        <cc1:CustLabel ID="residenceAddress" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090600_004" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style="width: 88%" colspan="5">
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
                                        <cc1:CustLabel ID="HIDE_REG_ADDR" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                </tr>
                                <tr class="trEven">
                                    <%-- 通訊地址 --%>
                                    <td align="right" style="width: 11%">
                                        <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090600_005" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style="width: 88%" colspan="5">
                                        <cc1:CustLabel ID="HQlblHCOP_MAILING_CITY" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                        <cc1:CustLabel ID="HQlblHCOP_MAILING_ADDR1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                        <cc1:CustLabel ID="HQlblHCOP_MAILING_ADDR2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                        <cc1:CustLabel ID="HIDE_MAILING_ADDR" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                </tr>
                                <tr class="trOdd" style="">
                                    <%--商店狀態--%>
                                    <td align="right">
                                        <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="right" IsColon="True"  ShowID="01_08010600_009"></cc1:CustLabel>
                                    </td>
                                    <td align="left">
                                        <asp:Label ID="lbAML_HQ_Work_HCOP_STATUS" runat="server" Text=""></asp:Label>
                                    </td>
                                    <%-- 國籍1 --%>
                                    <td align="right" style="">
                                        <cc1:CustLabel ID="country1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090600_006" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style=""  >
                                        <div style="position: relative">
                                            <cc1:CustLabel ID="HQlblHCOP_OWNER_NATION" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                        </div>
                                    </td>
                                    <%-- 國籍2 --%>
                                    <td align="right" style="">
                                        <cc1:CustLabel ID="country2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090600_007" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style="width: 10%" colspan="3" >
                                        <div style="position: relative">
                                            <cc1:CustLabel ID="HQlblHCOP_COUNTRY_CODE_2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                        </div>
                                    </td>

                                </tr>
                                <tr class="trEven">
                                    <%-- 任職公司 --%>
                                    <td align="right" style="width: 11%">
                                        <cc1:CustLabel ID="employedCompany" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090600_008" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style="" colspan="">
                                        <cc1:CustLabel ID="HQlblHCOP_NP_COMPANY_NAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False" Width="160px"></cc1:CustLabel>
                                    </td>
                                     <%-- 行業編號 --%>
                                    <td align="right" style="width: 11%">
                                        <cc1:CustLabel ID="industryNumber" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090600_009" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style=" ">
                                        <cc1:CustLabel ID="HQlblHCOP_CC" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>

                                        <cc1:CustLabel ID="HQlblHCOP_CC_2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>

                                        <cc1:CustLabel ID="HQlblHCOP_CC_3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <%-- 職稱編號 --%>
                                    <td align="right">
                                            <cc1:CustLabel ID="jobNum" runat="server" CurAlign="left" CurSymbol="£"
                                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01090600_010"
                                                StickHeight="False"></cc1:CustLabel></td>
                                    <td style="width: 10%">
                                        <cc1:CustLabel ID="HQlblHCOP_OC" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                </tr>
                                <tr class="trOdd">
                                    <%-- 客戶收入及資產主要收入來源 --%>
                                    <td align="right" style="width: 11%">
                                            <cc1:CustLabel ID="income" runat="server" CurAlign="left" CurSymbol="£"
                                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01090600_011"
                                                StickHeight="False"></cc1:CustLabel></td>
                                    <td colspan="5" style="">
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
                                <tr class="trEven">
                                    <%-- 業務往來目的 --%>
                                    <td align="right" style="">
                                            <cc1:CustLabel ID="businessContactsPurpose" runat="server" CurAlign="left" CurSymbol="£"
                                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01090600_012"
                                                StickHeight="False"></cc1:CustLabel></td>
                                    <td colspan="1" style="">
                                        <cc1:CustLabel ID="CustLabel17" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" ShowID="01_01090600_033"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <%-- 身分證換補領查詢結果 --%>
                                    <td align="right" >
                                            <cc1:CustLabel ID="IDCardReplacementSearchResult" runat="server" CurAlign="left" CurSymbol="£"
                                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01090600_013"
                                                StickHeight="False"></cc1:CustLabel></td>
                                    <td style="width: 10%" colspan="3">
                                        <cc1:CustLabel ID="lbAML_HQ_Work_OWNER_ID_SreachStatus" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" ShowID="01_01090600_034"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                        <asp:HiddenField ID="hidAML_HQ_Work_OWNER_ID_SreachStatus" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <%-- 客戶風險分析 --%>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo2" style="">
                    <tr class="itemTitle">
                        <td>
                            <li>
                                <cc1:CustLabel ID="title2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01090600_014"></cc1:CustLabel>
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
                                            SetBreak="False" SetOmit="False" ShowID="01_01090600_015" StickHeight="False"></cc1:CustLabel>
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
                                            SetBreak="False" SetOmit="False" ShowID="01_01090600_016" StickHeight="False"></cc1:CustLabel>
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
                                            SetBreak="False" SetOmit="False" ShowID="01_01090600_017" StickHeight="False"></cc1:CustLabel>
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
                                            SetBreak="False" SetOmit="False" ShowID="01_01090600_018" StickHeight="False"></cc1:CustLabel>
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
                                            SetBreak="False" SetOmit="False" ShowID="01_01090600_019" StickHeight="False"></cc1:CustLabel>
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
                                            SetBreak="False" SetOmit="False" ShowID="01_01090600_020" StickHeight="False"></cc1:CustLabel>
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
                                            SetBreak="False" SetOmit="False" ShowID="01_01090600_021" StickHeight="False"></cc1:CustLabel>
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
                                            SetBreak="False" SetOmit="False" ShowID="01_01090600_022" StickHeight="False"></cc1:CustLabel>
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
               <br />
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo3" style="">
                    <tr>
                        <td style="width: 100%" colspan="1">
                            <table border="0" width="100%" cellspacing="1">
                                <tr class="trOdd">
                                    <%-- 名單掃描案件編號 --%>
                                    <td align="right" width="22%">
                                        <cc1:CustLabel ID="listScanCaseNumber" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090600_023" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td align="left" width="22%">
                                        <cc1:CustLabel ID="SCDDlblNameCheck_No" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <%-- AML名單掃描結果 --%>
                                    <td align="right" width="22%">
                                        <cc1:CustLabel ID="listScanResults" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090600_024" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td align="left" width="22%">
                                        <cc1:CustLabel ID="lbHQ_SCDD_NameCheck_Item" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                        <asp:Label ID="lbHQ_SCDD_NameCheck_Note" runat="server" Text=""></asp:Label>
                                        <asp:HiddenField ID="hidHQ_SCDD_NameCheck_Item" runat="server" />
                                </tr>
                            </table>
                            <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table2">
                                <tr class="trEven">
                                    <%--計算風險等級或異常結案--%>
                                    <td align="right" style="width: 11%">
                                        <cc1:CustLabel ID="riskLevel" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                            IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" ShowID="01_01090600_025" StickHeight="False"></cc1:CustLabel>
                                    </td>
                                    <td style="width: 88%">
                                        <asp:Label ID="lbSR_RiskItem" runat="server" Text=""></asp:Label>
                                        <asp:Label ID="lbSR_RiskNote" runat="server" Text=""></asp:Label>                                                       
                                        <asp:HiddenField ID="hidNewRiskLevel" runat="server" />                                       
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
                                    <td align="left">
                                        <div><asp:Label ID="lbSR_EDD_Date" runat="server" Text=""></asp:Label></div>
                                        <asp:Label ID="lbSR_Explanation" runat="server" Text=""></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>               
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table3">
                    <tr class="trOdd">
                        <td align="left" colspan="8">
                            <%--本人__________________(簽名或蓋章)已就上述客戶進行盡職審查。憑著本表格內所填報的資料，依本人所知道及相信，本人認為客戶及其資金來源根據中國信託內部政策及程序及於有關國家/地區適用的防制洗錢法律均屬合法及可接受。--%>
                            <%--<cc1:CustLabel ID="CustLabel77" runat="server" CurAlign="right" ShowID="01_08010600_046" ></cc1:CustLabel>--%>
                            <asp:Label ID="Label9" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="left" style="width: 10%">
                            <%--經辦(簽章)：--%>
                                <cc1:CustLabel ID="CustLabel81" runat="server" CurAlign="right" ShowID="01_08010600_047" IsColon="true"></cc1:CustLabel>
                                <br><br><br><br><br>
                            <%--日期--%>
                                <cc1:CustLabel ID="CustLabel19" runat="server" CurAlign="right" ShowID="01_08010600_048" IsColon="true"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 15%">
                            <%--員編--%>
                                <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                                <br><br><br><br><br>
                            <%--日期資料--%>
                                <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left" style="width: 10%">
                            <%--主管(簽章)：--%>
                                <cc1:CustLabel ID="CustLabel22" runat="server" CurAlign="right" ShowID="01_08010600_049" IsColon="true"></cc1:CustLabel>
                                <br><br><br><br><br>
                            <%--日期--%>
                                <cc1:CustLabel ID="CustLabel24" runat="server" CurAlign="right" ShowID="01_08010600_048" IsColon="true"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 15%">
                            <%--員編--%>
                                <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
                                <br><br><br><br><br>
                            <%--日期資料--%>
                                <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left" style="width: 10%">
                            <%--高風險客戶核准主管(簽章)：--%>
                                <cc1:CustLabel ID="CustLabel20" runat="server" CurAlign="right" ShowID="01_08010600_050" IsColon="true"></cc1:CustLabel>
                                <br><br><br><br><br>
                            <%--日期--%>
                                <cc1:CustLabel ID="CustLabel23" runat="server" CurAlign="right" ShowID="01_08010600_048" IsColon="true"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 15%">
                            <%--員編--%>
                                <asp:Label ID="Label5" runat="server" Text=""></asp:Label>
                                <br><br><br><br><br>
                            <%--日期資料--%>
                                <asp:Label ID="Label6" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left" style="width: 10%">
                            <%--洗錢防制二部部長(簽章)：--%>
                                <cc1:CustLabel ID="CustLabel27" runat="server" CurAlign="right" ShowID="01_08010600_051" IsColon="true"></cc1:CustLabel>
                                <br><br><br><br><br>
                            <%--日期--%>
                                <cc1:CustLabel ID="CustLabel28" runat="server" CurAlign="right" ShowID="01_08010600_048" IsColon="true"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 15%">
                            <%--員編--%>
                                <asp:Label ID="Label7" runat="server" Text=""></asp:Label>
                                <br><br><br><br><br>
                            <%--日期資料--%>
                                <asp:Label ID="Label8" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                </table>
            
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo4" style="">
                        <tr class="itemTitle">
                            <td align="center"> 
                               <cc1:CustButton ID="btnPrint" CssClass="smallButton" runat="server" Width="69px" OnClick="btnPrint_Click" DisabledWhenSubmit="False" ShowID="01_08010600_075" />                             
                                &nbsp;  &nbsp;  &nbsp;  &nbsp;
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
