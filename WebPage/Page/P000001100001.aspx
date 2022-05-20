<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P000001100001.aspx.cs" Inherits="Page_P000001100001" %>

<%@ Register Src="../Common/Controls/CustUpdateProgress.ascx" TagName="CustUpdateProgress"
    TagPrefix="uc1" %>
<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html>
<%--2021/04/12_Ares_Stanley-調整失效語法--%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script type="text/javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <script type="text/javascript">
        function RadioCheck(object) {
            $("input[type='radio']").attr("checked", "");
            $(object).attr("checked", "true");
            $("#btnUpdateCode").trigger("click");
        }
        function DeleteConfirmation() {
            return confirm("確定刪除?");
        }


    </script>
    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />
</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" EnablePageMethods="True" runat="server"></asp:ScriptManager>
        <uc1:CustUpdateProgress ID="CustUpdateProgress1" runat="server" />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="2">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" Text="AML建檔參數維護"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblCodeType" Text="類型" runat="server"></cc1:CustLabel>：
                        </td>
                        <td align="left" style="width: 85%">
                            <asp:DropDownList ID="ddlCodeType" runat="server"></asp:DropDownList>
                            <asp:HiddenField ID="hidID" runat="server" />
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblCodeID" Text="代號" runat="server"></cc1:CustLabel>：
                        </td>
                        <td align="left" style="width: 85%">
                            <cc1:CustTextBox ID="txtCodeID" runat="server" MaxLength="12" InputType="String"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblCodeName" Text="中文名稱" runat="server"></cc1:CustLabel>：
                        </td>
                        <td align="left" style="width: 85%">
                            <cc1:CustTextBox ID="txtCodeName" runat="server" MaxLength="30"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblCodeENName" Text="英文名稱" runat="server"></cc1:CustLabel>：
                        </td>
                        <td align="left" style="width: 85%">
                            <cc1:CustTextBox ID="txtCodeEnName" runat="server"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblOrderBy" Text="順序" runat="server"></cc1:CustLabel>：
                        </td>
                        <td align="left" style="width: 85%">
                            <cc1:CustTextBox ID="txtOrderBy" runat="server" InputType="Int"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblDescription" Text="描述" runat="server"></cc1:CustLabel>：
                        </td>
                        <td align="left" style="width: 85%">
                            <cc1:CustTextBox ID="txtDescription" runat="server" ></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblIsValid" Text="是否啟用" runat="server"></cc1:CustLabel>：
                        </td>
                        <td align="left" style="width: 85%">
                            <asp:CheckBox ID="chkIsValid" runat="server"></asp:CheckBox>

                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td colspan="2" align="center">
                            <cc1:CustButton ID="btnQuery" runat="server" CssClass="smallButton" Style="width: 70px;"
                                OnClick="btnQuery_Click" Text="查詢" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnAdd" runat="server" CssClass="smallButton" Style="width: 50px;"
                                OnClick="btnAdd_Click" Text="新增" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnUpdate" runat="server" CssClass="smallButton" Style="width: 50px;"
                                OnClick="btnUpdate_Click" Text="更新" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnDelete" runat="server" CssClass="smallButton" Style="width: 50px;"
                                  Text="刪除" OnClientClick="return DeleteConfirmation();"  CausesValidation="false" OnClick="btnDelete_Click"/>&nbsp;
                            <cc1:CustButton ID="btnCancel" runat="server" CssClass="smallButton" Style="width: 50px;"
                                OnClick="btnCancel_Click" Text="取消" />&nbsp;&nbsp;
                            <asp:Button ID="btnUpdateCode" CssClass="smallButton" CausesValidation="false" runat="server" 
                                Text="Button" OnClick="btnUpdateCode_Click" Style="display: none;" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <cc1:CustGridView ID="gvpbCodeInfo" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid"
                                OnRowDataBound="gvpbCodeInfo_RowDataBound">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemStyle Width="5%" CssClass="Grid_Choice" />
                                        <HeaderStyle Width="5%" />
                                        <ItemTemplate>
                                            <input class="ChoiceButton" id="radCode" onclick="RadioCheck(this);" type="radio" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    
                                    <asp:BoundField DataField="ID" HeaderText="ID" ShowHeader="False" >
                                        <ItemStyle Width="0%" />
                                        <HeaderStyle Width="0%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CODE_ID" HeaderText="代號">
                                        <ItemStyle Width="20%" />
                                        <HeaderStyle Width="20%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CODE_NAME" HeaderText="中文名稱">
                                        <ItemStyle Width="25%" />
                                        <HeaderStyle Width="25%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CODE_EN_NAME" HeaderText="英文名稱">
                                        <ItemStyle Width="25%" />
                                        <HeaderStyle Width="25%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ORDERBY" HeaderText="順序">
                                        <ItemStyle Width="10%" />
                                        <HeaderStyle Width="10%" />
                                    </asp:BoundField>
                                     <asp:BoundField DataField="DESCRIPTION" HeaderText="描述">
                                        <ItemStyle Width="10%" />
                                        <HeaderStyle Width="10%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="IsValid" HeaderText="是否啟用" />
                                </Columns>
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                            </cc1:CustGridView>
                            <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right"
                                InputBoxStyle="height:22px" OnPageChanged="gpList_PageChanged">
                            </cc1:GridPager>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
