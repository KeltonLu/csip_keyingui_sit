<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010107030001.aspx.cs" Inherits="P010107030001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust"%>

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
    
        //20160601 (U) by Tank, add 最後重覆日 check
        function checkInput()
        {
            if($("#dpBatchDate_foo").val() == "")
            {
                alert('請選擇最後重覆日');
                $("#dpBatchDate_foo").focus();
                return false
            }
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
                        <td colspan="2">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01070300_001" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <%--20160601 (U) by Tank, add 最後重覆日--%>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01070300_004" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 85%">
                            <cc1:DatePicker ID="dpBatchDate" runat="server"></cc1:DatePicker>
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td align="center" colspan="2" >
                                <cc1:CustButton ID="btnSearch" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClick="btnSearch_Click" OnClientClick="return checkInput();" Text="" Width="150px"
                                ShowID="01_01070300_002" />&nbsp;&nbsp;
                                <%--20160907 (U) by Tank, add 查詢(新) 按鈕--%>
                                <cc1:CustButton ID="btnSearch_New" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClick="btnSearch_New_Click" OnClientClick="return checkInput();" Text="" Width="150px"
                                ShowID="01_01070300_020" />&nbsp;&nbsp;
                                <cc1:CustButton ID="btnExportEXCLE" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClick="btnExportEXCLE_Click" Text="" Width="150px"
                                ShowID="01_01070300_003" />
                                <cc1:CustLabel ID="lblstrFlag" runat="server" Visible="false"></cc1:CustLabel>
                        </td>
                    </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <cc1:CustGridView ID="gvpbSearchRecord" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid" >
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header A" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="MaxRepeatDay" DataFormatString="{0:yyyy/MM/dd}">
                                        <itemstyle width="5%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Batch_Date" DataFormatString="{0:yyyy/MM/dd}">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Batch_NO">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Receive_Batch">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Shop_ID">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Sign_Type">
                                        <itemstyle width="5%" horizontalalign="Center"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Process_Flag">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SN">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Card_No">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Tran_Date" DataFormatString="{0:yyyy/MM/dd}" HtmlEncode="False">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Installment_Periods" HtmlEncode="False">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Auth_Code" HtmlEncode="False">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="AMT" DataFormatString="{0:N0}" HtmlEncode="False">
                                        <itemstyle width="5%" horizontalalign="right" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Receipt_Type">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="1Key_user" HtmlEncode="False">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="2Key_user" HtmlEncode="False">
                                        <itemstyle width="5%" horizontalalign="Center" />
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
