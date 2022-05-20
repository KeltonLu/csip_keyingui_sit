<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010801030002.aspx.cs" Inherits="Page_P010801030002" %>

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

</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="0">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <cc1:CustGridView ID="grvNameCheckD" runat="server" BorderStyle="Solid" Width="100%" OnPageIndexChanging="grvNameCheckD_PageIndexChanging" OnRowDataBound="grvNameCheckD_RowDataBound" PageSize="10" PagerVisible="True" PagerStyle-BackColor="Silver">
                    <AlternatingRowStyle CssClass="Grid_AlternatingItem" />
                    <Columns>
                        <asp:BoundField DataField="AMLReferenceNumber" HeaderText="名單掃描案件編號">
                            <ItemStyle Width="13%" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TRNNUM" HeaderText="交易序號">
                            <ItemStyle Width="13%" />
                        </asp:BoundField>
                        <asp:BoundField DataField="NonEnglishName" HeaderText="姓名(中)">
                            <ItemStyle Width="20%" />
                        </asp:BoundField>
                        <asp:BoundField DataField="EnglishName" HeaderText="姓名(英)">
                            <ItemStyle Width="20%" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ID" HeaderText="ID證號">
                            <ItemStyle Width="10%" HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DOB" HeaderText="出生年月日">
                            <ItemStyle Width="10%" HorizontalAlign="Center"/>
                        </asp:BoundField>
                        <asp:BoundField DataField="Nationality" HeaderText="國籍">
                            <ItemStyle Width="7%" HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="MatchedResult" HeaderText="是否HIT中">
                        <ItemStyle Width="7%" HorizontalAlign="Center" />
                        </asp:BoundField>
                    </Columns>
                    <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                    <PagerSettings Mode="NumericFirstLast" />
                    <PagerStyle HorizontalAlign="Center" />
                    <RowStyle CssClass="Grid_Item" />
                    <SelectedRowStyle CssClass="Grid_SelectedItem" />
                </cc1:CustGridView>
                <!--20210603_Ares_Rick- 視窗跳轉調整 避免雙開 並新增 返回按鈕-->
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo4" style="">
                        <tr class="itemTitle">
                            <td align="center">
                                <%--返回--%>
                                <cc1:CustButton ID="btnCancel" CssClass="smallButton" runat="server" Width="40px" OnClick="btnCancel_Click" DisabledWhenSubmit="False" ShowID="01_01080101_073" />
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>

            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
