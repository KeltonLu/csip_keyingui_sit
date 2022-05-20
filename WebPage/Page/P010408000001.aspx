<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010408000001.aspx.cs" Inherits="P010408000001" %>

<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script language="Javascript" type="text/javascript">
        function loadSetFocus()
        {
            object = document.getElementById("txtACODE");
            if (object!=null)
                object.focus();
        }
    </script>

</head>
<body class="workingArea" onload="loadSetFocus();">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <cust:image runat="server" ID="image1" />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="2">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_04080000_001" StickHeight="False"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblType" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_04080000_002" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td>
                            <cc1:CustRadioButton ID="rbType_Redeem" runat="server" Checked="true" GroupName="rbType"
                                AutoPostBack="True" OnCheckedChanged="rbType_Redeem_CheckedChanged" />
                            <cc1:CustRadioButton ID="rbType_Award" runat="server" GroupName="rbType" AutoPostBack="True"
                                OnCheckedChanged="rbType_Award_CheckedChanged" />
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right">
                            <cc1:CustLabel ID="lblACODE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_04080000_003" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td>
                            <cc1:CustTextBox ID="txtACODE" runat="server" MaxLength="3" Width="50px"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td align="center" colspan="2">
                            <cc1:CustButton ID="btnSearch" runat="server" ShowID="01_04080000_005" Width="50px"
                                DisabledWhenSubmit="False" CssClass="smallButton" OnClick="btnSearch_Click"></cc1:CustButton>
                        </td>
                    </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="1">
                    <tr>
                        <td>
                            <cc1:CustGridView ID="gvList" Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
                                BorderStyle="Solid" runat="server" PagerID="gpList" AllowSorting="True" OnRowDataBound="gvList_RowDataBound"
                                OnRowEditing="gvList_RowEditing">
                                <PagerSettings Visible="False" />
                                <RowStyle CssClass="Grid_Item" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" />
                                <Columns>
                                    <asp:BoundField DataField="CODE">
                                        <itemstyle horizontalalign="Center" />
                                        <headerstyle width="30%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="MEMO">
                                        <itemstyle horizontalalign="Center" />
                                        <headerstyle width="45%" />
                                    </asp:BoundField>
                                    <asp:CommandField InsertVisible="False" ShowCancelButton="False" ShowEditButton="True"
                                        EditText="設定">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:CommandField>
                                </Columns>
                            </cc1:CustGridView>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" OnPageChanged="gpList_PageChanged">
                            </cc1:GridPager>
                        </td>
                    </tr>
                </table>
                <cc1:CustPanel ID="CPSet" Width="100%" runat="server">
                    <table width="100%" cellpadding="0" cellspacing="1">
                        <tr>
                            <td>
                                <cc1:CustGridView ID="gvSet" Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
                                    BorderStyle="Solid" runat="server" AllowSorting="True" PagerID="gpSet" OnRowDataBound="gvSet_RowDataBound">
                                    <PagerSettings Visible="False" />
                                    <RowStyle CssClass="Grid_Item" />
                                    <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                    <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                    <AlternatingRowStyle CssClass="Grid_AlternatingItem" />
                                    <Columns>
                                        <asp:TemplateField>
                                            <itemtemplate>
<cc1:CustCheckBox id="ccBox" runat="server" __designer:wfdid="w2"></cc1:CustCheckBox>
</itemtemplate>
                                            <headerstyle width="20%" />
                                            <itemstyle horizontalalign="Center" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="CODE">
                                            <headerstyle width="40%" />
                                            <itemstyle horizontalalign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="MEMO" />
                                    </Columns>
                                </cc1:CustGridView>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <cc1:GridPager ID="gpSet" runat="server" AlwaysShow="True" OnPageChanged="gpSet_PageChanged">
                                </cc1:GridPager>
                            </td>
                        </tr>
                        <tr class="itemTitle">
                            <td align="center">
                                <cc1:CustButton ID="btnSubmit" runat="server" ShowID="01_01050201_003" Width="50px"
                                    DisabledWhenSubmit="False" CssClass="smallButton" OnClick="btnSubmit_Click"></cc1:CustButton>
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
