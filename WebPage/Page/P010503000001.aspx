<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010503000001.aspx.cs" Inherits="P010503000001" %>

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
    var txtAchRtnCode = $('#txtAchRtnCode').val().Trim();
    var txtAchRtnMsg = $('#txtAchRtnMsg').val().Trim();
    var txtEddaRtnCode = $('#txtEddaRtnCode').val().Trim();
    var txtEddaRtnMsg = $('#txtEddaRtnMsg').val().Trim();
    
    // 控制項代碼
    if(txtAchRtnCode === '' || txtAchRtnMsg === '' || txtEddaRtnCode === '' || txtEddaRtnMsg === '')
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
                <cc1:custtextbox id="txtAchRtnInfoSeq" runat="server" Visible="False"></cc1:custtextbox>
                <table cellpadding="0" cellspacing="1" width="100%">
                    <tr class="itemTitle">
                        <td colspan="4">
                            <li>
                                <cc1:custlabel id="lblTitle" runat="server" ShowID="01_05030000_000"></cc1:custlabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="text-align: right; width: 10%">
                            <cc1:custlabel id="labelAchRtnCode" runat="server" ShowID="01_05030000_007"></cc1:custlabel>：
                        </td>
                        <td style="text-align: left; width: 40%">
                            <cc1:custtextbox id="txtAchRtnCode" runat="server" Width="400px" MaxLength="2"></cc1:custtextbox>
                        </td>
                        <td style="text-align: right; width: 10%">
                            <cc1:custlabel id="labelAchRtnMsg" runat="server" ShowID="01_05030000_008"></cc1:custlabel>：
                        </td>
                        <td style="text-align: left; width: 40%">
                            <cc1:custtextbox id="txtAchRtnMsg" runat="server" Width="400px"></cc1:custtextbox>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td style="text-align: right; width: 10%">
                            <cc1:custlabel id="labelEddaRtnCode" runat="server" ShowID="01_05030000_009"></cc1:custlabel>：
                        </td>
                        <td style="text-align: left; width: 40%">
                            <cc1:custtextbox id="txtEddaRtnCode" runat="server" Width="400px" MaxLength="2"></cc1:custtextbox>
                        </td>
                        <td style="text-align: right; width: 10%">
                            <cc1:custlabel id="labelEddaRtnMsg" runat="server" ShowID="01_05030000_010"></cc1:custlabel>：
                        </td>
                        <td style="text-align: left; width: 40%">
                            <cc1:custtextbox id="txtEddaRtnMsg" runat="server" Width="400px"></cc1:custtextbox>
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td colspan="4" align="center">
                            <cc1:CustButton id="btnADD" runat="server" Cssclass="smallButton" OnClick="btnADD_Click"  
                                OnClientClick="return CheckInputValue();" DisabledWhenSubmit="False" ShowID="01_05030000_001" />
                            &nbsp;|&nbsp;
                            <cc1:CustButton id="btnOK" runat="server" Cssclass="smallButton" OnClick="btnOK_Click" 
                                OnClientClick="return CheckInputValue();" DisabledWhenSubmit="False" ShowID="01_05030000_004" />
                            &nbsp;|&nbsp;
                            <cc1:CustButton id="btnCancel" runat="server" Cssclass="smallButton" OnClick="btnCancel_Click" 
                                            DisabledWhenSubmit="False" ShowID="01_05030000_005" />
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td colspan="4">
                            <li>
                                <cc1:custlabel runat="server" ShowID="01_05030000_006"></cc1:custlabel>
                            </li>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <cc1:custgridview id="grvFUNCTION" runat="server" allowsorting="True" pagerid="gpList" PageSize="100"
                                width="100%" borderwidth="0px" cellpadding="0" cellspacing="1" borderstyle="Solid"
                                onrowdatabound="grvFUNCTION_RowDataBound" OnRowCommand="grvFUNCTION_RowSelecting">
                                <Columns>
                                    <asp:BoundField DataField="AchRtnInfoSeq" ItemStyle-cssclass="hiddenCol" HeaderStyle-CssClass="hiddenCol">
                                        <itemstyle width="10%" horizontalalign="Center"/>
                                    </asp:BoundField>
                                    <%--ACH回覆代碼--%>
                                    <asp:BoundField DataField="Ach_Rtn_Code">
                                        <itemstyle width="10%" horizontalalign="Center"/>
                                    </asp:BoundField>
                                    <%--ACH回覆訊息--%>
                                    <asp:BoundField DataField="Ach_Rtn_Msg">
                                        <itemstyle width="35%" horizontalalign="LEFT"/>
                                    </asp:BoundField>
                                    <%--EDDA回覆代碼--%>
                                    <asp:BoundField DataField="EDDA_Rtn_Code">
                                        <itemstyle width="10%" horizontalalign="Center"/>
                                    </asp:BoundField>
                                    <%--EDDA回覆訊息--%>
                                    <asp:BoundField DataField="EDDA_Rtn_Msg">
                                        <itemstyle width="35%" horizontalalign="LEFT"/>
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
