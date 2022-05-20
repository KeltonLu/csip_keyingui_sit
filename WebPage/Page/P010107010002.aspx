<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010107010002.aspx.cs" Inherits="P010107010002" %>

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

</head>
<body class="workingArea" >
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <cust:image runat="server" ID="image1"/>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <cc1:CustGridView ID="gvpbSearchRecord" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid" OnRowDataBound="gvpbSearchRecord_RowDataBound" >
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header A" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="SN">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Card_No">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Tran_Date" DataFormatString="{0:yyyy/MM/dd}" HtmlEncode = false>
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Product_Type">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Installment_Periods">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="AMT" DataFormatString="{0:N0}">
                                        <itemstyle width="5%" horizontalalign="right"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Auth_Code">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Receipt_Type">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Case_Status">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="KeyIn_Flag">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Keyin_User">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Modify_DateTime" DataFormatString="{0:yyyy/MM/dd HH:mm:ss}" HtmlEncode = false>
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
