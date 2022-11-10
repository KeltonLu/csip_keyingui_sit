<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010505000001.aspx.cs" Inherits="P010505000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="../Common/Controls/CustUpdateProgress.ascx" TagName="CustUpdateProgress" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<title></title>

<script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

<script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

<script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

<script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

<script type="text/javascript" src="../Common/Script/JQuery/json3-3.3.2.min.js"></script>

<link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />
    
<style type="text/css">
    .hiddenCol {
        display:none;
    }
</style>

<script type="text/javascript" language="javascript">

// 驗證輸入資訊
function CheckInputValue()
{
    var txtEddaRtnCode = $('#txtEddaRtnCode').val().Trim();
    var txtEddaRtnMsg = $('#txtEddaRtnMsg').val().Trim();
    
    // 控制項代碼
    if(txtEddaRtnCode === '' || txtEddaRtnMsg === '')
	{
		alert('代碼或訊息不能是空白!');
		return false;
	}
}

</script>
</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" EnablePageMethods="True" runat="server">
        </asp:ScriptManager>
        <uc1:custupdateprogress id="CustUpdateProgress1" runat="server" />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <cc1:custtextbox id="txtEddaRtnInfoSeq" runat="server" Visible="False"></cc1:custtextbox>
                <table cellpadding="0" cellspacing="1" width="100%">
                    <tr class="itemTitle">
                        <td colspan="4">
                            <li>
                                <cc1:custlabel runat="server" ShowID="01_05050000_000"></cc1:custlabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="text-align: right; width: 15%">
                            <cc1:custlabel runat="server" ShowID="01_05050000_008"></cc1:custlabel>：
                        </td>
                        <td style="text-align: left; width: 15%">
                            <cc1:custtextbox id="txtEddaRtnCode" runat="server" MaxLength="2"></cc1:custtextbox>
                        </td>
                        <td style="text-align: right; width: 15%">
                            <cc1:custlabel runat="server" ShowID="01_05050000_009"></cc1:custlabel>：
                        </td>
                        <td style="text-align: left; width: 55%">
                            <cc1:custtextbox id="txtEddaRtnMsg" runat="server" Width="400px" MaxLength="50"></cc1:custtextbox>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td style="text-align: right; width: 15%">
                            <cc1:custlabel runat="server" ShowID="01_05050000_010"></cc1:custlabel>：
                        </td>
                        <td style="text-align: left; width: 15%">
                            <cc1:CustDropDownList runat="server" ID="NeedSendHostList" AutoPostBack="False" Width="100px" kind="select">
                                <asp:ListItem Value="Y">是</asp:ListItem>
                                <asp:ListItem Value="N">否</asp:ListItem>
                            </cc1:CustDropDownList>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:custlabel runat="server" ShowID="01_05050000_011"></cc1:custlabel>：
                        </td>
                        <td style="width: 55%">
                            <cc1:custtextbox runat="server" id="txtSendHostMsg" Width="400px" MaxLength="9"></cc1:custtextbox>
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td colspan="4" align="center">
                            <cc1:CustButton id="btnADD" runat="server" Cssclass="smallButton" OnClick="BtnAddClick"  
                                OnClientClick="return CheckInputValue();" DisabledWhenSubmit="False" ShowID="01_05050000_001" />
                            &nbsp;|&nbsp;
                            <cc1:CustButton id="btnOK" runat="server" Cssclass="smallButton" OnClick="BtnOkClick" 
                                OnClientClick="return CheckInputValue();" DisabledWhenSubmit="False" ShowID="01_05050000_004" />
                            &nbsp;|&nbsp;
                            <cc1:CustButton runat="server" Cssclass="smallButton" OnClick="BtnCancelClick" 
                                            DisabledWhenSubmit="False" ShowID="01_05050000_005" />
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td colspan="4">
                            <li>
                                <cc1:custlabel runat="server" ShowID="01_05050000_006"></cc1:custlabel>
                            </li>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <cc1:custgridview id="gridView" runat="server" allowsorting="True" pagerid="gpList" PageSize="100"
                                width="100%" borderwidth="0px" cellpadding="0" cellspacing="1" borderstyle="Solid"
                                onrowdatabound="GridViewRowDataBound" OnRowCommand="GridViewRowSelect"
                                              OnRowDeleting="GridViewRowDelete">
                                <Columns>
                                    <asp:BoundField DataField="EddaRtnInfoSeq" ItemStyle-cssclass="hiddenCol" HeaderStyle-CssClass="hiddenCol">
                                        <itemstyle width="0%" horizontalalign="Center"/>
                                    </asp:BoundField>
                                    <%--回覆代碼--%>
                                    <asp:BoundField DataField="EddaRtnCode">
                                        <itemstyle width="15%" horizontalalign="Center"/>
                                    </asp:BoundField>
                                    <%--回覆訊息--%>
                                    <asp:BoundField DataField="EddaRtnMsg">
                                        <itemstyle width="30%" horizontalalign="LEFT"/>
                                    </asp:BoundField>
                                    <%--核印失敗是否送主機--%>
                                    <asp:BoundField DataField="NeedSendHost">
                                        <itemstyle width="15%" horizontalalign="Center"/>
                                    </asp:BoundField>
                                    <%--給主機的簡訊--%>
                                    <asp:BoundField DataField="SendHostMsg">
                                        <itemstyle width="25%" horizontalalign="LEFT"/>
                                    </asp:BoundField>
                                    <%--功能--%>
                                    <asp:TemplateField>
                                        <itemstyle width="15%" horizontalalign="Center" />
                                        <itemtemplate>
                                            <cc1:CustLinkButton ID="btnModify" runat="server" Style="width: 50px;" CausesValidation="False" 
                                                CommandName="Modify"/>
                                            &nbsp;&nbsp;
                                            <cc1:CustLinkButton ID="btnDelete" runat="server" Style="width: 50px;" CausesValidation="False"
                                                                CommandName="Delete"/>
                                        </itemtemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                            </cc1:custgridview>
                            <cc1:gridpager id="gpList" runat="server" PageSize="100" custominfotextalign="Right" prevpagetext="<前一頁" alwaysshow="True" onpagechanged="GridViewPageChanged">
                            </cc1:gridpager>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
