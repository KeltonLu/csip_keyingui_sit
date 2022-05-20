<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010103040001.aspx.cs" Inherits="P010103040001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <%-- 2020/11/23_Ares_Stanley-修正格式 --%>
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />


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
        <asp:GridView ID="gdvDataChange" runat="server" AutoGenerateColumns="False" EnableModelValidation="True" CellPadding="4" ForeColor="#333333" GridLines="None" OnRowCommand="gdvDataChange_RowCommand" >
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:ButtonField ButtonType="Button" Text="查詢" CommandName="SELECT" />
                <asp:BoundField DataField="UNI_NO1" HeaderText="統一編號"/>
                <asp:BoundField DataField="USER1" HeaderText="鍵一同仁"/>
                <asp:BoundField DataField="USER2" HeaderText="鍵二同仁"/>
            </Columns>
            <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#CEF9F9" />
            <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />

        </asp:GridView>
        <%--<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">--%>
            <ContentTemplate>
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                        <tr class="itemTitle1">
                            <%--基本資料異動--%>
                            <td colspan="5" align="left">
                                <li>
                                    <cc1:CustLabel ID="CustLabel16" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" Width="232px" IsColon="False" ShowID="01_01030100_118"></cc1:CustLabel>
                                </li>
                            </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="lblCardNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_002" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 88%" colspan="4">
                                <cc1:CustTextBox ID="txtUNI_NO1" runat="server" Width="80px" BoxName="統一編號一" Enabled="false"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtUNI_NO2" runat="server" Width="40px" BoxName="統一編號二" Enabled="false"></cc1:CustTextBox>
                            </td>
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
                                    <cc1:CustTextBox ID="txtCORP_NO" runat="server" Width="140px" BoxName="總公司統一編號"  Enabled="false"></cc1:CustTextBox>
                                </td>
                                <%--AML行業編號--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel43" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_096"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtCORP_MCC" runat="server" Width="80px" BoxName="AML行業編號" Enabled="false"></cc1:CustTextBox>
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
                                    <cc1:CustTextBox ID="txtCORP_ESTABLISH" runat="server" Width="60px" BoxName="設立" Enabled="false"></cc1:CustTextBox>
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
                                        <cc1:CustTextBox ID="txtCORP_Organization" runat="server" Width="30px" BoxName="法律形式" Enabled="false"></cc1:CustTextBox>
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
                                        <cc1:CustTextBox ID="txtCORP_CountryCode" runat="server" Width="50px" BoxName="國籍" Enabled="false"></cc1:CustTextBox>
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
                                    <cc1:CustTextBox ID="txtCORP_CountryStateCode" runat="server" Width="50px" BoxName="註冊國為美國者，請勾選州別" Enabled="false"></cc1:CustTextBox>
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
                                    <cc1:CustTextBox ID="txtREG_NAME_CH" runat="server" Width="240px" BoxName="中文登記名稱" Enabled="false"></cc1:CustTextBox>
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
                                    <cc1:CustTextBox ID="txtREG_NAME_EN" runat="server" Width="240px" BoxName="英文登記名稱" Enabled="false"></cc1:CustTextBox>
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
                                    <cc1:CustTextBox ID="txtREG_ZIP_CODE" runat="server" Width="130px" BoxName="登記地址郵遞區號" Enabled="false" ></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtREG_CITY" runat="server" Width="130px" BoxName="商店登記地址一" Enabled="false" AutoPostBack="true" OnTextChanged="TextBox_AddrChanged"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtREG_ADDR1" runat="server" Width="280px" BoxName="商店登記地址二" Enabled="false"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtREG_ADDR2" runat="server" Width="140px" BoxName="商店登記地址三"  Enabled="false"></cc1:CustTextBox></td>
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
                                    <cc1:CustTextBox ID="txtCORP_TEL1" runat="server" Width="36px" BoxName="登記電話一" Enabled="false"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtCORP_TEL2" runat="server" Width="110px" BoxName="登記電話二" Enabled="false"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtCORP_TEL3" runat="server" Width="50px" BoxName="登記電話三" Enabled="false"></cc1:CustTextBox>
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
                                    <cc1:CustTextBox ID="txtPrincipalNameCH" runat="server" Width="130px" BoxName="負責人" Enabled="false" ></cc1:CustTextBox></td>
                            </tr>
                            <tr class="trEven">
                                <%--中文長姓名--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel27" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01030100_102" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                     <cc1:CustCheckBox ID="chkisLongName2" runat="server" BoxName="長姓名" Enabled="false"/>
                                    <cc1:CustLabel ID="CustLabel28" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtPrincipalName_L" runat="server" Width="260px" Enabled="false"></cc1:CustTextBox>
                                </td>
                                <%--羅馬拼音--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel29" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtPrincipalName_PINYIN" runat="server" Width="260px" BoxName="負責人羅馬拼音" Enabled="false"></cc1:CustTextBox>
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
                                    <cc1:CustTextBox ID="txtPrincipalNameEN" runat="server" Width="240px" BoxName="負責人英文名" Enabled="false"></cc1:CustTextBox>
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
                                    <cc1:CustTextBox ID="txtPrincipalIDNo" runat="server" Width="140px" BoxName="負責人ID" Enabled="false"></cc1:CustTextBox>
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
                                    <cc1:CustTextBox ID="txtPrincipalBirth" runat="server" Width="50px" BoxName="生日" Enabled="false"></cc1:CustTextBox>
                                </td>
                                <%--發證日期--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel46" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050100_026"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtPrincipalIssueDate" runat="server" Width="50px" BoxName="發證日期" Enabled="false"></cc1:CustTextBox>
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
                                    <cc1:CustTextBox ID="txtPrincipalIssuePlace" runat="server" Width="40px" BoxName="換證點" Enabled="false" ></cc1:CustTextBox>
                                </td>
                                <%--領補換類別--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel39" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050100_028"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtReplaceType" runat="server" Width="40px" BoxName="換證點" Enabled="false" ToolTip="1.初 2.補 3.換" ></cc1:CustTextBox>
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
                                        <cc1:CustTextBox ID="txtPrincipalCountryCode" runat="server" Width="50px" BoxName="國籍" Enabled="false"></cc1:CustTextBox>
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
                                    <cc1:CustTextBox ID="txtPrincipalPassportNo" runat="server" Width="200px" BoxName="護照號碼" Enabled="false"></cc1:CustTextBox>
                                </td>
                                <%--護照效期--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel34" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030100_100" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtPrincipalPassportExpdt" runat="server" Width="200px" BoxName="護照效期" Enabled="false"></cc1:CustTextBox>
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
                                    <cc1:CustTextBox ID="txtPrincipalResidentNo" runat="server" Width="200px" BoxName="居留證號" Enabled="false"></cc1:CustTextBox>
                                </td>
                                <%--居留效期--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel36" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01030100_101" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%">
                                    <cc1:CustTextBox ID="txtPrincipalResidentExpdt" runat="server" Width="200px" BoxName="居留效期" Enabled="false"></cc1:CustTextBox>
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
                                    <cc1:CustTextBox ID="txtPrincipal_TEL1" runat="server" Width="40px" BoxName="負責人電話一" Enabled="false"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtPrincipal_TEL2" runat="server" Width="100px" BoxName="負責人電話二" Enabled="false"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtPrincipal_TEL3" runat="server" Width="40px" BoxName="負責人電話三" Enabled="false"></cc1:CustTextBox>
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
                                    <cc1:CustTextBox ID="txtHouseholdCITY" runat="server" Width="130px" BoxName="負責人戶籍地址一" Enabled="false"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtHouseholdADDR1" runat="server" Width="280px" BoxName="負責人戶籍地址二"  Enabled="false"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtHouseholdADDR2" runat="server" Width="140px" BoxName="負責人戶籍地址三" Enabled="false" ></cc1:CustTextBox></td>
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
                                    <cc1:CustTextBox ID="txtBANK_NAME" runat="server" Width="140px" BoxName="銀行名稱" Enabled="false"></cc1:CustTextBox>
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
                                    <cc1:CustTextBox ID="txtBANK_BRANCH" runat="server" Width="140px" BoxName="分行名稱" Enabled="false"></cc1:CustTextBox>
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
                                    <cc1:CustTextBox ID="txtBANK_ACCT_NAME" runat="server" Width="140px" BoxName="戶名" Enabled="false"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd" runat="server" visible="false">
                                <%--檢碼--%>
                                <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="CustLabel14" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01030100_116"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 35%" colspan="3">
                                    <cc1:CustTextBox ID="txtCHECK_CODE" runat="server" Width="140px" BoxName="檢碼" Enabled="false"></cc1:CustTextBox>
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
                                    <cc1:CustTextBox ID="txtBANK_ACCT1" runat="server" Width="96px" MaxLength="3" BoxName="帳號一" Enabled="false"></cc1:CustTextBox>&nbsp;
                                    <cc1:CustTextBox ID="txtBANK_ACCT2" runat="server" MaxLength="14" checktype="num" BoxName="帳號二" Enabled="false"></cc1:CustTextBox>
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
                                    <cc1:CustTextBox ID="txtARCHIVE_NO" runat="server" Width="140px" BoxName="歸檔編號" Enabled="false"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtDOC_ID" runat="server" Width="140px" MaxLength="14" BoxName="案件編號" Visible="false"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <%--資料最後異動--%>
                            <tr class="trOdd">                                  
                                
                                <%--資料最後異動MAKER--%>
                                <td align="right" style="width: 18%; height: 33px;"colspan="2">
                                    <cc1:CustLabel ID="lbLAST_UPD_MAKER" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_086"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 16%; height: 33px;">
                                    <cc1:CustTextBox ID="txtLAST_UPD_MAKER" runat="server" MaxLength="12" checktype="numandletter" Enabled="false"
                                        Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動MAKER" onfocus="allselect(this);"></cc1:CustTextBox>
                                </td>
                                <%--資料最後異動CHECKER--%>
                                <td align="right" style="width: 20%; height: 33px;" >
                                    <cc1:CustLabel ID="lbLAST_UPD_CHECKER" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_087" StickHeight="False"
                                        ></cc1:CustLabel>
                                </td>
                                <td style="width: 18%; height: 33px;" >
                                    <cc1:CustTextBox ID="txtLAST_UPD_CHECKER" runat="server" MaxLength="12" checktype="numandletter" Enabled="false"
                                        Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動CHECKER" onfocus="allselect(this);"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven">
                                
                                <%--資料最後異動分行--%>
                                <td align="right" style="width: 11%; height: 33px;" colspan="2">
                                    <cc1:CustLabel ID="lbLAST_UPD_BRANCH" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01040101_085" StickHeight="False"
                                        ></cc1:CustLabel>
                                </td>
                                <td style="width: 16%; height: 33px;"colspan="3">
                                    <cc1:CustTextBox ID="txtLAST_UPD_BRANCH" runat="server" MaxLength="12" Width="100px"
                                        onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動分行" checktype="numandletter" 
                                        onfocus="allselect(this);" Enabled="false"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="itemTitle1">
                                <%--提交--%>
                                <td colspan="5" align="center">
                                    &nbsp;<cc1:CustButton ID="btnSend" Text="" runat="server" Width="57px" CssClass="smallButton"
                                        onkeydown="setfocus('btnBasicData');" DisabledWhenSubmit="False" ShowID="01_01030100_119" OnClick="btnSend_Click" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    <cc1:CustButton ID="btnReject" Text="" runat="server" Width="57px" CssClass="smallButton"
                                        onkeydown="setfocus('btnBasicData');" DisabledWhenSubmit="False" ShowID="01_01030100_120" OnClick="btnReject_Click" />
                                </td>
                            </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        <%--</asp:UpdatePanel>--%>
    </form>
</body>
</html>