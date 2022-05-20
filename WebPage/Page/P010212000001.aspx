<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010212000001.aspx.cs" Inherits="P010212000001" %>

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

    <script type="text/javascript" language="javascript">
    
    //* 輸入欄位正確性檢查
   function checkInput()
    {
        var msg = "";
        
        if ($("#CustTextBox1").val() == "" && $("#CustTextBox2").val() == "" && $("#dpDateStart_foo").val() == ""
            && $("#dpDateEnd_foo").val() == "")
        {
            msg += "卡號,PID,餘額轉置日期,請擇一輸入\r\n";
        }
        
    
        var strDateStart=document.getElementById("dpDateStart_foo").value.Trim();
        var strDateEnd=document.getElementById("dpDateEnd_foo").value.Trim();
        
        if(strDateStart>strDateEnd)
        {
            msg += "餘額轉置起日不可晚于餘額轉置迄日\r\n";
        }
        
        if (msg == "")
            return true;
        else
        {
            alert(msg);
            return false;
        }
    }
    </script>

</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <cust:image runat="server" ID="image1" />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="4">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_0212000001_001" StickHeight="False"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right">
                            <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_0212000001_014" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td>
                            <cc1:CustTextBox ID="CustTextBox1" runat="server" MaxLength="16" onkeydown="nosubmit();"></cc1:CustTextBox>
                        </td>
                        <td align="right">
                            <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_0212000001_008" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td>
                            <cc1:CustTextBox ID="CustTextBox2" runat="server" MaxLength="16" onkeydown="nosubmit();"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right">
                            <cc1:CustLabel ID="lblInput_Date" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_0212000001_002" StickHeight="False"></cc1:CustLabel></td>
                        <td colspan="3">
                            <cc1:DatePicker ID="dpDateStart" runat="server">
                            </cc1:DatePicker>
                            --
                            <cc1:DatePicker ID="dpDateEnd" runat="server">
                            </cc1:DatePicker>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right">
                            <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_0212000001_021" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td colspan="3">
                            <cc1:CustDropDownList runat="server" ID="cddl_Upload">
                                <asp:ListItem Value="0" Selected="True">請選擇</asp:ListItem>
                                <asp:ListItem Value="Y">己上傳</asp:ListItem>
                                <asp:ListItem Value="N">未上傳</asp:ListItem>
                            </cc1:CustDropDownList>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right">
                            <cc1:CustLabel ID="lbReceiveNumber" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_0212000001_011"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td colspan="3">
                            <cc1:CustDropDownList runat="server" ID="CDDList1">
                                <asp:ListItem Value="0" Selected="True">請選擇</asp:ListItem>
                                <asp:ListItem Value="D">刪除</asp:ListItem>
                                <asp:ListItem Value="N">未處理</asp:ListItem>
                                <asp:ListItem Value="Y">成功</asp:ListItem>
                                <asp:ListItem Value="E">失敗</asp:ListItem>
                            </cc1:CustDropDownList>
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td colspan="4" align="center">
                            <cc1:CustButton ID="btnSearch" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                Text="" Width="50px" ShowID="01_0212000001_015" OnClick="btnSearch_Click" OnClientClick="return checkInput();" />&nbsp;
                            <cc1:CustButton ID="btnClear" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                Text="" Width="50px" ShowID="01_0212000001_016" OnClick="btnClear_Click" />
                            <cc1:CustButton ID="cbtn_Excel" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                Text="" Width="80px" ShowID="01_0211000001_018" OnClick="cbtn_Excel_Click" />    
                        </td>
                    </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr style="background-color: Green">
                        <td align="right">
                            <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_0212000001_022" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td>
                            <cc1:CustTextBox ID="ctxt_total" ReadOnly="true" runat="server" onkeydown="nosubmit();"></cc1:CustTextBox>
                        </td>
                        <td align="right">
                            <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_0212000001_023" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td>
                            <cc1:CustTextBox ID="ctxt_success" ReadOnly="true" runat="server" onkeydown="nosubmit();"></cc1:CustTextBox>
                        </td>
                        <td align="right">
                            <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_0212000001_024" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td>
                            <cc1:CustTextBox ID="ctxt_faile" ReadOnly="true" runat="server" onkeydown="nosubmit();"></cc1:CustTextBox>
                        </td>
                    </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <cc1:CustGridView ID="gvList" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid"
                                OnRowDataBound="gvList_RowDataBound">
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="RowNumber">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Trans_Date">
                                        <itemstyle width="20%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CardNo">
                                        <itemstyle width="10%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="PID">
                                        <itemstyle width="10%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Reason_Code">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Memo">
                                        <itemstyle width="15%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Upload_Flag">
                                        <itemstyle width="10%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Process_Flag">
                                        <itemstyle width="10%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Process_Note">
                                        <itemstyle width="10%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Create_DateTime">
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
