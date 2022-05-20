<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010407000001.aspx.cs" Inherits="P010407000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
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
        //* 畫面加載時，光標位置設定
        function loadSetFocus()
        {
            object = document.getElementById("txtCode");
            if (object!=null)
                object.focus();
        }
    
        function checkButton()
        {
            object = document.getElementById("txtCode");
            
            if (object.value.trim() == "")
            {
                alert("CODE不能為空！");
                return false;
            }
            return true;
        }
        
        function setrbType(bState)
        {
            var rbTypeR
            var rbTypeA
            
            rbTypeR = document.getElementById("rbType_Redeem");
            rbTypeA = document.getElementById("rbType_Award");
            
            
            
            rbTypeR.disabled = true;
            rbTypeA.disabled = true;
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
                                    SetBreak="False" SetOmit="False" ShowID="01_04070000_001" StickHeight="False"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblFunction" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_04070000_005" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td>
                            <cc1:CustRadioButton ID="rbFunction_P" runat="server" Checked="true" GroupName="rbFunction"
                                AutoPostBack="True" OnCheckedChanged="rbFunction_P_CheckedChanged" />
                            <cc1:CustRadioButton ID="rbFunction_C" runat="server" GroupName="rbFunction" AutoPostBack="True"
                                OnCheckedChanged="rbFunction_C_CheckedChanged" />
                            <cc1:CustRadioButton ID="rbFunction_A" runat="server" GroupName="rbFunction" AutoPostBack="True"
                                OnCheckedChanged="rbFunction_A_CheckedChanged" />
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right">
                            <cc1:CustLabel ID="lblType" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_04070000_006" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td>
                            <cc1:CustRadioButton ID="rbType_Redeem" runat="server" Checked="true" GroupName="rbType"
                                AutoPostBack="True" OnCheckedChanged="rbType_Redeem_CheckedChanged" />
                            <cc1:CustRadioButton ID="rbType_Award" runat="server" GroupName="rbType" AutoPostBack="True"
                                OnCheckedChanged="rbType_Award_CheckedChanged" />
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right">
                            <cc1:CustLabel ID="lblCode" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_04070000_009" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td>
                            <cc1:CustTextBox ID="txtCode" runat="server" MaxLength="5" InputType="Int" Width="60px"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right">
                            <cc1:CustLabel ID="lblMEMO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_04070000_010" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td>
                            <cc1:CustTextBox ID="txtMEMO" runat="server" MaxLength="50" TextMode="MultiLine"
                                Width="500px"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td align="center" colspan="2">
                            <cc1:CustButton ID="btnAdd" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClientClick="return checkButton();" Width="50px" ShowID="01_04070000_011" OnClick="btnAdd_Click" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnSearch" runat="server" ShowID="01_04070000_012" Width="50px"
                                DisabledWhenSubmit="False" CssClass="smallButton" OnClick="btnSearch_Click"></cc1:CustButton>
                        </td>
                    </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <cc1:CustGridView ID="gvList" Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
                                BorderStyle="Solid" runat="server" PagerID="gpList" AllowSorting="True" OnRowDataBound="gvList_RowDataBound" OnRowDeleting="gvList_RowDeleting">
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
                                    <asp:CommandField InsertVisible="False" ShowCancelButton="False" ShowDeleteButton="True"
                                        DeleteText="刪除 ">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:CommandField>
                                </Columns>
                            </cc1:CustGridView>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cc1:GridPager ID="gpList" runat="server" OnPageChanged="gpList_PageChanged" AlwaysShow="True">
                            </cc1:GridPager>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
