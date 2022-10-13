<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010504000001.aspx.cs" Inherits="P010504000001" %>

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
    .hiddenCol
    {
        display:none;
    }
</style>

<script type="text/javascript" language="javascript">

// 驗證輸入資訊
function CheckInputValue()
{
    var txtRtnType = $('#txtRtnType').val().Trim();
    var txtPostRtnCode = $('#txtPostRtnCode').val().Trim();
    var txtPostRtnMsg = $('#txtPostRtnMsg').val().Trim();
    
    // 控制項代碼
    if(txtRtnType === '' || txtPostRtnCode === '' || txtPostRtnMsg === '')
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
                <cc1:custtextbox id="txtPostOfficeRtnInfoSeq" runat="server" Visible="False"></cc1:custtextbox>
                <table cellpadding="0" cellspacing="1" width="100%">
                    <tr class="itemTitle">
                        <td colspan="6">
                            <li>
                                <cc1:custlabel id="lblTitle" runat="server" ShowID="01_05040000_000"></cc1:custlabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="text-align: right; width: 10%">
                            <cc1:custlabel id="labelRtnType" runat="server" ShowID="01_05040000_006"></cc1:custlabel>：
                        </td>
                        <td style="text-align: left; width: 10%">
                            <cc1:custtextbox id="txtRtnType" runat="server" MaxLength="2"></cc1:custtextbox>
                        </td>
                        <td style="text-align: right; width: 10%">
                            <cc1:custlabel id="labelPostRtnCode" runat="server" ShowID="01_05040000_007"></cc1:custlabel>：
                        </td>
                        <td style="text-align: left; width: 10%">
                            <cc1:custtextbox id="txtPostRtnCode" runat="server" MaxLength="2"></cc1:custtextbox>
                        </td>
                        <td style="text-align: right; width: 10%">
                            <cc1:custlabel id="labelPostRtnMsg" runat="server" ShowID="01_05040000_008"></cc1:custlabel>：
                        </td>
                        <td style="text-align: left; width: 50%">
                            <cc1:custtextbox id="txtPostRtnMsg" runat="server" Width="400px"></cc1:custtextbox>
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td colspan="6" align="center">
                            <cc1:CustButton id="btnADD" runat="server" Cssclass="smallButton" OnClick="btnADD_Click"  
                                OnClientClick="return CheckInputValue();" DisabledWhenSubmit="False" ShowID="01_05040000_001" />
                            &nbsp;|&nbsp;
                            <cc1:CustButton id="btnOK" runat="server" Cssclass="smallButton" OnClick="btnOK_Click" 
                                OnClientClick="return CheckInputValue();" DisabledWhenSubmit="False" ShowID="01_05040000_004" />
                            &nbsp;|&nbsp;
                            <cc1:CustButton id="btnCancel" runat="server" Cssclass="smallButton" OnClick="btnCancel_Click" 
                                            DisabledWhenSubmit="False" ShowID="01_05040000_005" />
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td colspan="6">
                            <li>
                                <cc1:custlabel runat="server" ShowID="01_05040000_006"></cc1:custlabel>
                            </li>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <cc1:custgridview id="grvFUNCTION" runat="server" allowsorting="True" pagerid="gpList" PageSize="100"
                                width="100%" borderwidth="0px" cellpadding="0" cellspacing="1" borderstyle="Solid"
                                onrowdatabound="grvFUNCTION_RowDataBound" OnRowCommand="grvFUNCTION_RowSelecting">
                                <Columns>
                                    <asp:BoundField DataField="PostOfficeRtnInfoSeq" ItemStyle-cssclass="hiddenCol" HeaderStyle-CssClass="hiddenCol">
                                        <itemstyle width="10%" horizontalalign="Center"/>
                                    </asp:BoundField>
                                    <%--回覆類別--%>
                                    <asp:BoundField DataField="RtnType">
                                        <itemstyle width="10%" horizontalalign="Center"/>
                                    </asp:BoundField>
                                    <%--回覆代碼--%>
                                    <asp:BoundField DataField="PostRtnCode">
                                        <itemstyle width="10%" horizontalalign="Center"/>
                                    </asp:BoundField>
                                    <%--回覆名稱--%>
                                    <asp:BoundField DataField="PostRtnMsg">
                                        <itemstyle width="60%" horizontalalign="LEFT"/>
                                    </asp:BoundField>
                                    <asp:TemplateField>
                                        <itemstyle width="10%" horizontalalign="Center" />
                                        <itemtemplate>
                                            <cc1:CustLinkButton ID="btnModify" runat="server" Style="width: 50px;" CausesValidation="False" 
                                                CommandName="Select"/>
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
                            <cc1:gridpager id="gpList" runat="server" PageSize="100" custominfotextalign="Right" prevpagetext="<前一頁" alwaysshow="True" onpagechanged="gpList_PageChanged">
                            </cc1:gridpager>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
