<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010406000001.aspx.cs" Inherits="P010406000001" %>

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
        //*匯入檢核
        function sumbit(type)
        {                    
                if(document.getElementById('fulFilePath').value == "")
                {
                    window.parent.postMessage({ func: 'ClientMsgShow', data: '請選擇打開文件的路徑！' }, '*');
                    // ClientMsgShow('請選擇打開文件的路徑！');
                    return false;
                } 
                
                if(type == 1)
                {
                    //*顯示確認提示框
                    if(!confirm('確定是否要匯入資料？'))
                    {
                        return false;
                    }
                }              
                return true;
        }         
    </script>
    
</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <cust:image runat="server" ID="image1"/>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <Triggers>
                <asp:PostBackTrigger ControlID="btnOpen" />
                <asp:PostBackTrigger ControlID="btnInsert" />
            </Triggers>
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                    <tr class="itemTitle">
                        <td colspan="2">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_04060000_001"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblPath" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_04060000_002" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 85%">
                            <asp:FileUpload ID="fulFilePath" runat="server" UNSELECTABLE="on" Width="80%" /><br />
                        </td>
                    </tr>
                    <tr height="30">
                        <td colspan="2" align="center">
                            <cc1:CustButton ID="btnOpen" runat="server" DisabledWhenSubmit="false" CssClass="smallButton"
                                ShowID="01_04060000_004" OnClick="btnOpen_Click" OnClientClick="return sumbit(0);"/>&nbsp;&nbsp
                            <cc1:CustButton ID="btnInsert" runat="server" DisabledWhenSubmit="false" CssClass="smallButton"
                                ShowID="01_04060000_003" OnClick="btnInsert_Click" OnClientClick="return sumbit(1);"
                                onkeydown="setfocus('btnOpen');" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <cc1:CustGridView ID="grvUpload" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid">
                                <Columns>
                                    <asp:BoundField DataField="BankCodeS">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BankCodeL">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BankName">
                                        <itemstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                </Columns>
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header A" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                            </cc1:CustGridView>
                            <cc1:GridPager ID="gpList" runat="server" CustomInfoTextAlign="Right" AlwaysShow="True"
                                OnPageChanged="gpList_PageChanged">
                            </cc1:GridPager>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
