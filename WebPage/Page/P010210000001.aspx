<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010210000001.aspx.cs" Inherits="P010210000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust"%>
<%--20210707_Ares_Stanley-畫面調整--%>
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
    
    function checkInput()
    {
        var strDateStart=document.getElementById("dpDateStart_foo").value.Trim();
        if (strDateStart == "")
        {
            alert("請選擇键檔起日.");
            return false;
        }

        var strDateEnd=document.getElementById("dpDateEnd_foo").value.Trim();
        if (strDateEnd == "")
        {
            alert("請選擇键檔迄日.");
            return false;
        }
        
        if(strDateStart>strDateEnd)
        {
            alert("键檔起日不可晚于键檔迄日");
            return false;
        }

        return true;
    }
    </script>

</head>
<body class="workingArea" >
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <cust:image runat="server" ID="image1"/>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="4">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_02100000_001" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_02100000_021" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 35%">
                            <cc1:CustTextBox ID="txtCust_id" runat="server" MaxLength="20" Style="ime-mode: disabled;text-align: left"></cc1:CustTextBox>
                        </td>
                        <td></td><td></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblKey" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_02100000_002" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 35%">
                            <cc1:DatePicker ID="dpDateStart" runat="server"></cc1:DatePicker>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_02100000_003" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 35%">
                            <cc1:DatePicker ID="dpDateEnd" runat="server"></cc1:DatePicker>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_02100000_004" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 85%" colspan="3">
                            <cc1:CustRadioButton ID="radIsCTCB" runat="server" AutoPostBack="False" GroupName="type" Text="本行自扣" Checked="true" />&nbsp;&nbsp;
                            <cc1:CustRadioButton ID="radIsPostOffice" runat="server" AutoPostBack="False" GroupName="type" Text="郵局自扣" />&nbsp;&nbsp;
                            <cc1:CustRadioButton ID="radIsNotCTCB" runat="server" AutoPostBack="False" GroupName="type" Text="他行自扣"/>&nbsp;&nbsp;
                            <cc1:CustRadioButton ID="radAll" runat="server" AutoPostBack="False" GroupName="type" Text="本行+郵局+他行自扣"/>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_02100000_020" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 85%" colspan="3">
                            <cc1:CustRadioButton ID="radIsNotUpdateByTXT" runat="server" AutoPostBack="False" GroupName="SettingWay" Text="上送主機" Checked="true"/>&nbsp;&nbsp;
                            <cc1:CustRadioButton ID="radIsUpdateByTXT" runat="server" AutoPostBack="False" GroupName="SettingWay" Text="主機Temp檔"/>
                            <cc1:CustRadioButton ID="radIsUpdateAll" runat="server" AutoPostBack="False" GroupName="SettingWay" Text="上送主機+主機Temp檔"/>
                            
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="center" colspan="4" >
                                <cc1:CustButton ID="CustButton1" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClick="btnSearch_Click" OnClientClick="return checkInput();" Text="" Width="50px"
                                ShowID="01_02100000_005" />&nbsp;&nbsp;
                                <cc1:CustButton ID="CustButton2" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClick="btnExc_Click" OnClientClick="return checkInput();" Text="" Width="50px"
                                ShowID="01_02100000_006" />
                        </td>
                    </tr>
                <%--</table>--%>
                <%--<table width="100%" cellpadding="0" cellspacing="0">--%>
                    <tr>
                        <td colspan="4">
                            <cc1:CustGridView ID="gvpbSearchRecord" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid">
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header A" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="receive_number">
                                        <itemstyle width="8%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CardPersonID">
                                        <itemstyle width="6%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="cus_name">
                                        <itemstyle width="8%" horizontalalign="left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="acc_id" >
                                        <itemstyle width="6%" horizontalalign="left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="accnoBank" >
                                        <itemstyle width="2%" horizontalalign="left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="acc_no">
                                        <itemstyle width="10%" horizontalalign="Left"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="pay_way">
                                        <itemstyle width="1%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="bcycle_code">
                                        <itemstyle width="1%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="isUpdateByTXT">
                                        <itemstyle width="13%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="IsCTCB">
                                        <itemstyle width="1%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="mod_date">
                                        <itemstyle width="5%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Status">
                                        <itemstyle width="8%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="DocDate">
                                        <itemstyle width="5%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Ach_Batch_date">
                                        <itemstyle width="8%" horizontalalign="Left" />
                                    </asp:BoundField>
                                </Columns>
                            </cc1:CustGridView>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right"
                                InputBoxStyle="height:15px" OnPageChanged="gpList_PageChanged" PrevPageText="<前一頁"
                                SubmitButtonText="Go">
                            </cc1:GridPager>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
