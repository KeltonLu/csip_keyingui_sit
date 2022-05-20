<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010201000001.aspx.cs" Inherits="P010201000001" %>

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
    //* 畫面加載時，設置鼠標焦點
    function loadSetFocus()
    {
        object = document.getElementById("txtKey");
        if (object!=null)
            object.focus();
    }        
    
    //* 點選【查詢】按鈕時，畫面輸入欄位的檢核
    function checkInput()
    {
        //* 【關鍵字】欄位沒有輸入
        if (document.getElementById("txtKey").value.Trim() == "")
        {
            alert("關鍵字必須要有資料。");
            document.getElementById("txtKey").focus();
            return false;
        }
        
        //* 【關鍵字】欄位長度不能超過17
        if (document.getElementById("txtKey").value.Trim().length > 17)
        {
            alert("關鍵字欄位輸入不能超過17碼。");
            document.getElementById("txtKey").focus();
            return false;
        }
        
        //* 【關鍵字】欄位的英數字檢核
        var re = /^[A-Za-z0-9]*$/;
        if (!re.test(document.getElementById("txtKey").value.Trim()))
        {
            alert("關鍵字欄位輸入必須全為英數字。");
            document.getElementById("txtKey").focus();
            return false;
        }
        
        return true;
    }
    </script>

</head>
<body class="workingArea" onload="loadSetFocus();">
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
                                    SetBreak="False" SetOmit="False" ShowID="01_02010000_001" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblKey" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_02010000_002" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 85%">
                            <cc1:CustTextBox ID="txtKey" runat="server" MaxLength="20" Style="ime-mode: disabled;text-align: left" onkeydown="entersubmit('btnSearch');"></cc1:CustTextBox>
                            <cc1:CustButton ID="btnSearch" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClick="btnSearch_Click" OnClientClick="return checkInput();" Text="" Width="50px"
                                ShowID="01_02010000_003" /></td>
                    </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <cc1:CustGridView ID="gvpbSearchRecord" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid">
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header A" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="Log_Flag">
                                        <itemstyle width="8%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="FUNCTION_NAME">
                                        <itemstyle width="16%" horizontalalign="left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="field_name" >
                                        <itemstyle width="12%" horizontalalign="left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="before" >
                                        <itemstyle width="17%" horizontalalign="left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="after">
                                        <itemstyle width="17%" horizontalalign="left"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="user_name">
                                        <itemstyle width="10%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="mod_date">
                                        <itemstyle width="10%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="mod_time">
                                        <itemstyle width="10%" horizontalalign="Center" />
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
