<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010801040001.aspx.cs" Inherits="Page_P010801040001" %>

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
                        <td colspan="2">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01080104_001"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                </table>
                <br />
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                    <tr class="trOdd">
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblCardNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080104_002" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 12%">
                            <cc1:CustLabel ID="hlblCaseNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="CustLabel28" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080104_003" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 12%">
                            <cc1:CustLabel ID="hlblHCOP_HEADQUATERS_CORP_NO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="CustLabel29" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080104_005" StickHeight="False"></cc1:CustLabel>
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
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo4" style="">
                        <tr class="trEven">
                            <td align="center" style="width: 5%">
                                <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080104_006" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080104_007" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 5%">
                                <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080104_008" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080104_009" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 8%">
                                <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080104_010" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 5%">
                                <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080104_011" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 8%">
                                <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080104_012" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 6%">
                                <cc1:CustLabel ID="CustLabel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080104_013" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080104_014" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 5%">
                                <cc1:CustLabel ID="CustLabel10" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080104_015" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 5%">
                                <cc1:CustLabel ID="CustLabel11" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080104_016" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel12" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080104_017" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 7%">
                                <cc1:CustLabel ID="CustLabel13" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080104_018" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 5%">
                                <cc1:CustLabel ID="CustLabel14" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080104_019" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 5%">
                                <cc1:CustLabel ID="CustLabel15" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01080104_020" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="center" style="width: 5%">
                                <cc1:CustLabel ID="BrlblBRCH_CHINESE_NAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 10%">
                                <cc1:CustLabel ID="BrlblBRCH_ENGLISH_NAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 5%">
                                <%--<cc1:CustLabel ID="BrlblBRCH_BIRTH_DATE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>--%>
                                <cc1:CustTextBox ID="txtBRCH_BIRTH_DATE" runat="server" MaxLength="8" checktype="num"
                                    Width="60px" BoxName="負責人生日"></cc1:CustTextBox>
                            </td>
                             <td align="center" style="width: 5%">
                                <cc1:CustLabel ID="BrlblBRCH_ID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 5%">
                                <cc1:CustTextBox ID="txtBRCH_OWNER_ID_ISSUE_DATE" runat="server" MaxLength="8" checktype="num"
                                    Width="60px" BoxName="發證日期"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 5%">
                                <cc1:CustTextBox ID="txtBRCH_OWNER_ID_ISSUE_PLACE" runat="server" MaxLength="10" checktype="num"
                                    Width="40px" BoxName="發證地點"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 5%"> 
                                <cc1:CustDropDownList ID="dropHCOP_OWNER_ID_REPLACE_TYPE" kind="select" runat="server" 
                                    Style=" width: 40px;">
                                </cc1:CustDropDownList> 
                            </td>
                            <td align="center" style="width: 5%">
                                <cc1:CustRadioButton ID="radHasPhoto" runat="server" AutoPostBack="False" GroupName="Photo" Text="有" Checked="true" /><br />
                                <cc1:CustRadioButton ID="radNoPhoto" runat="server" AutoPostBack="False" GroupName="Photo" Text="無" />
                            </td>
                            <td align="center" style="width: 4%">
                                <cc1:CustTextBox ID="txtBRCH_NATION" runat="server" MaxLength="2" checktype="num"
                               Width="20px" BoxName="國籍" AutoPostBack="true" OnTextChanged="txtBRCH_NATION_TextChanged"></cc1:CustTextBox>
                            </td>
                            <td align="center" style="width: 5%">
                                <cc1:CustLabel ID="BrlblBRCH_PASSPORT" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 5%">
                                <cc1:CustTextBox ID="txtBRCH_PASSPORT_EXP_DATE" runat="server" MaxLength="8" checktype="num"
                                  Width="70px" BoxName="護照效期"></cc1:CustTextBox>
                            </td>
                             <td align="center" style="width: 5%">
                                <cc1:CustLabel ID="BrlblBRCH_RESIDENT_NO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="center" style="width: 5%">
                                <cc1:CustTextBox ID="txtBRCH_RESIDENT_EXP_DATE" runat="server" MaxLength="8" checktype="num"
                                   Width="70px" BoxName="居留證效期"></cc1:CustTextBox>
                            </td>
                              <td align="center" style="width: 5%">
                                <cc1:CustLabel ID="BrlblBRCH_BRCH_NO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                              <td align="center" style="width: 5%"> 
                                 <cc1:CustCheckBox ID="chkSreachStatusY" runat="server"   AutoPostBack="true"    BoxName="正常" Text="正常" OnCheckedChanged="chkSreachStatusY_CheckedChanged" />
                                 <br /><cc1:CustCheckBox ID="chkSreachStatusN" runat="server"   AutoPostBack="true"    BoxName="不適用" Text="不適用" OnCheckedChanged="chkSreachStatusN_CheckedChanged" />

                            </td>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo3" style="">
                        <tr class="itemTitle">
                            <td align="center">
                                <cc1:CustButton ID="btnAdd" runat="server" CssClass="smallButton" ShowID="01_01080105_002"
                                    OnClick="btnAdd_Click" OnClientClick="return checkInputText('pnlText', 1);" DisabledWhenSubmit="False"
                                    onkeydown="movefocus();" />
                                &nbsp;     &nbsp;     &nbsp;     &nbsp;
                                <cc1:CustButton ID="btnCancel" runat="server" CssClass="smallButton" ShowID="01_01080105_003"
                                    DisabledWhenSubmit="False"
                                    onkeydown="movefocus();" OnClick="btnCancel_Click" />
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
