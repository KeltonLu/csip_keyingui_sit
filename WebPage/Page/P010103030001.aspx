<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010103030001.aspx.cs" Inherits="P010103030001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript">
    
    //*客戶端檢核
    function checkInputText(id)
    {
        if (document.getElementById('txtUNI_NO1').value.Trim() == "")
        {
            alert('請輸入統一編號! ');
            document.getElementById('txtUNI_NO1').focus();
            return false;
        } 
        
        return true;
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
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                        <tr class="itemTitle">
                            <%--基本資料異動--%>
                            <td colspan="5" align="left">
                                <li>
                                    <cc1:CustLabel ID="CustLabel16" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" Width="232px" IsColon="False" ShowID="01_01030100_117"></cc1:CustLabel>
                                </li>
                            </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="lblCardNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_002" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 88%" colspan="4">
                                <cc1:CustTextBox ID="txtUNI_NO1" runat="server" MaxLength="8" checktype="num" Width="80px"
                                    onkeydown="entersubmit('btnSelect');" BoxName="統一編號一"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtUNI_NO2" runat="server" MaxLength="4" checktype="num" onkeydown="entersubmit('btnSelect');"
                                    Width="40px" BoxName="統一編號二"></cc1:CustTextBox>
                                <cc1:CustButton ID="btnSelect" runat="server" CssClass="smallButton" ShowID="01_01040400_027"
                                    OnClientClick="return checkInputText('pnlText');" DisabledWhenSubmit="False"
                                    OnClick="btnSelect_Click"/>
                            </td>
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
                                    <cc1:CustLabel ID="lblHEAD_CORP_NO" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <%--AML行業編號--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel43" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_096"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustLabel ID="lblCORP_MCC" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <%--設立日期--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel19" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_098"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustLabel ID="lblCORP_ESTABLISH" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <%--法律形式--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel20" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_099"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustLabel ID="lblCORP_Organization" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
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
                                        <cc1:CustLabel ID="lblCORP_CountryCode" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <%--註冊國為美國者，請勾選州別--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblCountryStateCode" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050100_014"
                                        StickHeight="False" Style="color: Red"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustLabel ID="lblCORP_CountryStateCode" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <%--登記名稱--%>
                            <tr class="trOdd">
                                <%--總公司登記名稱--%>
                                <td style="width: 15%;" align="right">
                                    <cc1:CustLabel ID="CustLabel21" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_121"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%;" colspan="3">
                                    <cc1:CustLabel ID="lblREG_NAME_CH" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <%--總公司英文名稱--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel24" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_122"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%" colspan="3">
                                    <cc1:CustLabel ID="lblREG_NAME_EN" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ></cc1:CustLabel>
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
                                    <cc1:CustLabel ID="lblREG_ZIP_CODE" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ></cc1:CustLabel>
                                    <cc1:CustLabel ID="lblREG_ADDR" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ></cc1:CustLabel>
                                </td>
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
                                    <cc1:CustLabel ID="lblCORP_TEL" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ></cc1:CustLabel>
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
                                    <cc1:CustLabel ID="lblPrincipalNameCH" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" ></cc1:CustLabel>
                            </tr>
                            <tr class="trEven">
                                <%--中文長姓名--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel27" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01030100_102" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustCheckBox ID="chkisLongName" runat="server" BoxName="長姓名" Enabled="false" />
                                    <cc1:CustLabel ID="CustLabel28" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustLabel ID="lblPrincipalName_L" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <%--羅馬拼音--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel29" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustLabel ID="lblPrincipalName_PINYIN" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
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
                                    <cc1:CustLabel ID="lblPrincipalNameEN" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" ></cc1:CustLabel>
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
                                    <cc1:CustLabel ID="lblPrincipalIDNo" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" ></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <%--負責人生日--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="lblBossBirthday" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_034"
                                    StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustLabel ID="lblPrincipalBirth" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" ></cc1:CustLabel>
                                </td>
                                <%--身分證發證日期--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel46" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050100_026"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustLabel ID="lblPrincipalIssueDate" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <%--身分證發證地點--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel38" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050100_027" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustLabel ID="lblPrincipalIssuePlace" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" ></cc1:CustLabel>
                                </td>
                                <%--身分證換補領別--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel39" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050100_028"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustLabel ID="lblReplaceType" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" ToolTip="1.初 2.補 3.換" ></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <%--負責人國籍--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel22" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_071" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%" colspan="3">
                                    <cc1:CustLabel ID="lblPrincipalCountryCode" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" ></cc1:CustLabel>
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
                                    <cc1:CustLabel ID="lblPrincipalPassportNo" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" ></cc1:CustLabel>
                                </td>
                                <%--護照效期--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel34" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030100_100" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustLabel ID="lblPrincipalPassportExpdt" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" ></cc1:CustLabel>
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
                                    <cc1:CustLabel ID="lblPrincipalResidentNo" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" ></cc1:CustLabel>
                                </td>
                                <%--居留效期--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel36" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030100_101" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustLabel ID="lblPrincipalResidentExpdt" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" ></cc1:CustLabel>
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
                                    <cc1:CustLabel ID="lblOWNER_TEL" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" ></cc1:CustLabel>
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
                                <td style="width: 35%" colspan="3" >
                                    <cc1:CustLabel ID="lblOWNER_ADDR" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" ></cc1:CustLabel>
                                </td>
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
                                    <cc1:CustLabel ID="lblBANK_NAME" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" ></cc1:CustLabel>
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
                                    <cc1:CustLabel ID="lblBANK_BRANCH" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
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
                                    <cc1:CustLabel ID="lblBANK_ACCT_NAME" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" ></cc1:CustLabel>
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
                                        <cc1:CustLabel ID="lblCHECK_CODE" runat="server" CurAlign="left" CurSymbol="£"
                                            FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                            SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
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
                                    <cc1:CustLabel ID="lblBANK_ACCT" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
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
                                    <cc1:CustLabel ID="lblARCHIVE_NO" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" ></cc1:CustLabel>
                                </td>
                            </tr>
                            <%--資料最後異動--%>
                            <tr class="trOdd">                            
                                <%--資料最後異動MAKER--%>
                                <td align="right" style="width: 18%; height: 33px;"colspan="2">
                                    <cc1:CustLabel ID="lblLAST_UPD_MAKER" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_086"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 16%; height: 33px;">
                                    <cc1:CustLabel ID="lblLAST_UPD_MAKERText" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <%--資料最後異動CHECKER--%>
                                <td align="right" style="width: 20%; height: 33px;">
                                    <cc1:CustLabel ID="lblLAST_UPD_CHECKER" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_087" StickHeight="False"
                                        ></cc1:CustLabel>
                                </td>
                                <td style="width: 18%; height: 33px;">
                                    <cc1:CustLabel ID="lblLAST_UPD_CHECKERText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"
                                        ></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <%--資料最後異動分行--%>
                                <td align="right" style="width: 11%; height: 33px;" colspan="2">
                                    <cc1:CustLabel ID="lblLAST_UPD_BRANCH" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_085" StickHeight="False"
                                        ></cc1:CustLabel>
                                </td>
                                <td style="width: 16%; height: 33px;"colspan="3">
                                    <cc1:CustLabel ID="lblLAST_UPD_BRANCHText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"
                                        ></cc1:CustLabel>
                                </td>
                            </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>