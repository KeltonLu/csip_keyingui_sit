<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010502000001.aspx.cs" Inherits="P010502000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="../Common/Controls/CustUpdateProgress.ascx" TagName="CustUpdateProgress" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%-- 2020/11/19_Ares_Luke-修正添加初始化效果; 2021/05/07_Ares_Stanley-修正修改資料文字欄位會變成編輯欄位--%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<title></title>

<script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

<script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

<script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

<script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

<script type="text/javascript" src="../Common/Script/JQuery/json3-3.3.2.min.js"></script>

<link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

<script type="text/javascript" language="javascript">

$(document).ready(function() {
    
    
});

// 驗證輸入資訊
function CheckInputValue()
{
    var elementCode = $('#txtElementCode').val().Trim();
    var elementName = $('#txtElementName').val().Trim();
    var elementID = $('#txtElementID').val().Trim();
    var sequence = $('#txtSequence').val().Trim();
    var checkFlag = $('#txtCheckFlag').val().Trim();
    var valueLength = $('#txtValueLength').val().Trim();
    
    // 控制項代碼
    if(elementCode == '' || elementCode.length != 4 || elementCode == '0000')
	{
		alert('控制項代碼輸入錯誤!');
		$('#txtElementCode').focus();
		return false;
	}
    
    // 控制項欄位名稱
    if(elementName == '')
	{
		alert('控制項欄位名稱未輸入!');
		$('#txtElementName').focus();
		return false;
    }
    
    // 控制項ID
    if(elementID == '')
	{
		alert('控制項ID未輸入!');
		$('#txtElementID').focus();
		return false;
	}
	else
	{
	    var rule = new RegExp('^[A-Za-z0-9]+$');
        
        if (!rule.test(elementID))
        {
            alert('控制項ID只允許英文、數字!');
		    $('#txtElementID').focus();
		    return false;
		}
	}
    
    // 順序
    if(sequence == '')
	{
		alert('順序未輸入!');
		$('#txtSequence').focus();
		return false;
	}
    
    // 檢核型態
    if(checkFlag == '')
	{
		alert('檢核型態未輸入!');
		$('#txtCheckFlag').focus();
		return false;
	}
	else
	{
	    if(!(checkFlag == '0' || checkFlag == '1' || checkFlag == '2'))
	    {
		    alert('檢核型態只能輸入0/1/2');
		    $('#txtCheckFlag').focus();
		    return false;
	    }
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
                <table cellpadding="0" cellspacing="1" width="100%">
                    <tr class="itemTitle">
                        <td colspan="2">
                            <li>
                                <cc1:custlabel id="lblTitle" runat="server" ShowID="01_05020000_001"></cc1:custlabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="text-align: right; width: 15%">
                            <cc1:custlabel id="lblElementCode" runat="server" ShowID="01_05020000_002"></cc1:custlabel>：
                        </td>
                        <td style="text-align: left; width: 85%">
                            <cc1:custtextbox id="txtElementCode" runat="server" Width="300px" Enabled="false"></cc1:custtextbox>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td style="text-align: right; width: 15%">
                            <cc1:custlabel id="lblElementName" runat="server" ShowID="01_05020000_003"></cc1:custlabel>：
                        </td>
                        <td style="text-align: left; width: 85%">
                            <cc1:custtextbox id="txtElementName" runat="server" maxlength="50" Width="300px"></cc1:custtextbox>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="text-align: right; width: 15%">
                            <cc1:custlabel id="lblElementID" runat="server" ShowID="01_05020000_004"></cc1:custlabel>：
                        </td>
                        <td style="text-align: left; width: 85%">
                            <cc1:custtextbox id="txtElementID" runat="server" maxlength="50" Width="300px"></cc1:custtextbox>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td style="text-align: right; width: 15%">
                            <cc1:custlabel id="lblSequence" runat="server" ShowID="01_05020000_005"></cc1:custlabel>：
                        </td>
                        <td style="text-align: left; width: 85%">
                            <cc1:custtextbox id="txtSequence" runat="server" inputtype="Int" maxlength="3" Width="300px"></cc1:custtextbox>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="text-align: right; width: 15%">
                            <cc1:custlabel id="lblIsRequired" runat="server" ShowID="01_05020000_006"></cc1:custlabel>：
                        </td>
                        <td style="text-align: left; width: 85%">
                            <asp:CheckBox ID="chkIsRequired" runat="server" />
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td style="text-align: right; width: 15%">
                            <cc1:custlabel id="lblCheckFlag" runat="server" ShowID="01_05020000_007"></cc1:custlabel>：
                        </td>
                        <td style="text-align: left; width: 85%">
                            <cc1:custtextbox id="txtCheckFlag" runat="server" inputtype="Int" maxlength="1" Width="300px"></cc1:custtextbox>
                            (0.不檢核 1.數字 2.英數)
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="text-align: right; width: 15%">
                            <cc1:custlabel id="lblValueLength" runat="server" ShowID="01_05020000_008"></cc1:custlabel>：
                        </td>
                        <td style="text-align: left; width: 85%">
                            <cc1:custtextbox id="txtValueLength" runat="server" inputtype="Int" maxlength="3" Width="300px"></cc1:custtextbox>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td style="text-align: right; width: 15%">
                            <cc1:custlabel id="lblDefaultValue" runat="server" ShowID="01_05020000_009"></cc1:custlabel>：
                        </td>
                        <td style="text-align: left; width: 85%">
                            <cc1:custtextbox id="txtDefaultValue" runat="server" maxlength="50" Width="300px"></cc1:custtextbox>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="text-align: right; width: 15%">
                            <cc1:custlabel id="lblPropertyCode" runat="server" ShowID="01_05020000_010"></cc1:custlabel>：
                        </td>
                        <td style="text-align: left; width: 85%">
                            <cc1:CustCheckBoxList ID="chkBusinessType" runat="server" RepeatDirection="Horizontal" ></cc1:CustCheckBoxList>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td style="text-align: right; width: 15%">
                            <cc1:custlabel id="lblRemark" runat="server" ShowID="01_05020000_011"></cc1:custlabel>：
                        </td>
                        <td style="text-align: left; width: 85%">
                            <cc1:custtextbox id="txtRemark" runat="server" maxlength="50" Width="300px"></cc1:custtextbox>
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td colspan="2" align="center">
                            <cc1:CustButton id="btnADD" runat="server" Cssclass="smallButton" OnClick="btnADD_Click"  
                                OnClientClick="return CheckInputValue();" DisabledWhenSubmit="False" ShowID="01_05020000_013" />&nbsp;&nbsp;
                            <cc1:CustButton id="btnOK" runat="server" Cssclass="smallButton" OnClick="btnOK_Click" 
                                OnClientClick="return CheckInputValue();" DisabledWhenSubmit="False" ShowID="01_05020000_014" />
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td colspan="2">
                            <li>
                                <cc1:custlabel id="lblWriteoff" runat="server" ShowID="01_05020000_015"></cc1:custlabel>
                            </li>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <cc1:custgridview id="grvFUNCTION" runat="server" allowsorting="True" pagerid="gpList"
                                width="100%" borderwidth="0px" cellpadding="0" cellspacing="1" borderstyle="Solid"
                                onrowdatabound="grvFUNCTION_RowDataBound" OnRowCommand="grvFUNCTION_RowSelecting">
                                <Columns>
                                    <asp:BoundField DataField="ELEMENT_CODE"><itemstyle width="10%" horizontalalign="Center" /></asp:BoundField><%--控制項代碼--%>
                                    <asp:BoundField DataField="ELEMENT_NAME"><itemstyle width="15%" /></asp:BoundField><%--控制項欄位名稱--%>
                                    <asp:BoundField DataField="ELEMENT_ID"><itemstyle width="10%" /></asp:BoundField><%--控制項ID--%>
                                    <asp:BoundField DataField="SEQUENCE"><itemstyle width="5%" horizontalalign="Center" /></asp:BoundField><%--順序--%>
                                    <asp:BoundField DataField="IS_REQUIRED"><itemstyle width="8%" horizontalalign="Center" /></asp:BoundField><%--是否必填--%>
                                    <asp:BoundField DataField="CHECK_FLAG"><itemstyle width="8%" horizontalalign="Center" /></asp:BoundField><%--檢核型態--%>
                                    <asp:BoundField DataField="VALUE_LENGTH"><itemstyle width="5%" horizontalalign="Center" /></asp:BoundField><%--長度--%>
                                    <asp:BoundField DataField="DEFAULT_VALUE"><itemstyle width="10%" /></asp:BoundField><%--預設值--%>
                                    <%--<asp:BoundField DataField="PROPERTY_CODE"><itemstyle width="10%" /></asp:BoundField>--%><%--使用類別編號--%>
                                    <asp:BoundField DataField="PROPERTY_CODE_NAME"><itemstyle width="10%" /></asp:BoundField><%--使用類別編號中文名稱--%>
                                    <asp:BoundField DataField="REMARK"><itemstyle width="10%" horizontalalign="Center" /></asp:BoundField><%--備註--%>
                                    <asp:TemplateField>
                                        <itemstyle width="15%" horizontalalign="Center" />
                                        <itemtemplate>
                                            <cc1:CustLinkButton ID="lbtnModify" runat="server" Style="width: 50px;" CausesValidation="False" 
                                                CommandName="Select" CommandArgument='<%# Server.HtmlEncode("ELEMENT_CODE") %>'/>
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
                            <cc1:gridpager id="gpList" runat="server" custominfotextalign="Right" prevpagetext="<前一頁" alwaysshow="True" onpagechanged="gpList_PageChanged">
                            </cc1:gridpager>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
