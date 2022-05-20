<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010106060001.aspx.cs" Inherits="P010106060001" %>

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

    function checkInput()
    {
        var strDateStart=document.getElementById("dpBatchDate_foo").value.Trim();
        var strBatchNO = document.getElementById("txtBatchNO").value.Trim();
        var strShopID = document.getElementById("txtShopID").value.Trim();
        if (strDateStart == "")
        {
            alert("請選擇編批日期.");
            return false;
        }
        if (strBatchNO == "")
        {
            alert("請輸入批號.");
            return false;
        }
        if (strShopID == "")
        {
            alert("請輸入商店代號.");
            return false;
        }
        return true;
    }
    
    function checkAddInput()
    {
        var strSN = document.getElementById("dropSN").value.Trim();
        var strErrColumn = document.getElementById("dropErrColumn").value.Trim();
        var strCorrectValue = document.getElementById("txtCorrectValue").value.Trim();
        var strReflectSource = document.getElementById("dropReflectSource").value.Trim();
        if (strSN == "")
        {
            alert("請選擇序號.");
            return false;
        }
        if (strErrColumn == "")
        {
            alert("請選擇錯誤欄位.");
            return false;
        }
        if (strCorrectValue == "")
        {
            alert("請輸入正確值.");
            return false;
        }
        if (strReflectSource == "")
        {
            alert("請選擇錯誤來源.");
            return false;
        }
        return true;
    }
    </script>  
    
    <style type="text/css">
    .hidden
    {
        display:none;
    }
    </style>
</head>
<body class="workingArea" >
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <cust:image runat="server" ID="image1"/>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="4">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060600_001" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01060600_002" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 85%">
                            <cc1:DatePicker ID="dpBatchDate" runat="server"></cc1:DatePicker>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01060600_003" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 85%">
                            <cc1:CustTextBox ID="txtBatchNO" runat="server" MaxLength="4" InputType="Int" Style="ime-mode: disabled;text-align: left"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01060600_004" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 85%">
                            <cc1:CustTextBox ID="txtShopID" runat="server" MaxLength="10" InputType="Int" Style="ime-mode: disabled;text-align: left"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td align="center" colspan="4" >
                                <cc1:CustButton ID="btnSearch" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClick="btnSearch_Click" OnClientClick="return checkInput();" Text="" Width="150px"
                                ShowID="01_01060600_005" />
                        </td>
                    </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <asp:Panel ID="Pl_gv1" runat="server" ScrollBars="auto">
                            <cc1:CustGridView ID="gvpbSearchRecord" runat="server" AllowSorting="True" PagerID="gpList" AllowPaging="False"
                                Width="99%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid" OnRowDataBound="gvpbSearchRecord_RowDataBound" >
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header A" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:TemplateField>
                                        <itemtemplate>
                                            <asp:Label ID="lblSeqNo" runat="server" 
                                                       Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
                                            <asp:HiddenField ID="Receive_Batch" runat="server" 
                                                 Value='<%# Eval("Receive_Batch") %>' />
                                            <asp:HiddenField ID="DetailSN" runat="server" 
                                                 Value='<%# Eval("SN") %>' />
                                        </itemtemplate>
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Shop_ID">
                                        <itemstyle width="10%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Card_No">
                                        <itemstyle width="10%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Tran_Date" DataFormatString="{0:yyyy/MM/dd}">
                                        <itemstyle width="7%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Product_Type">
                                        <itemstyle width="8%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Installment_Periods">
                                        <itemstyle width="8%" horizontalalign="Center"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Auth_Code">
                                        <itemstyle width="8%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="AMT" DataFormatString="{0:N0}">
                                        <itemstyle width="10%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Receipt_Type">
                                        <itemstyle width="8%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="1Key_user" HtmlEncode="False">
                                        <itemstyle width="13%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="2Key_user" HtmlEncode="False">
                                        <itemstyle width="13%" horizontalalign="Center" />
                                    </asp:BoundField>
                                </Columns>
                            </cc1:CustGridView>
                            </asp:Panel>
                        </td>
                    </tr>
<%--                    <tr>
                        <td>
                            <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right"
                                InputBoxStyle="height:15px" OnPageChanged="gpList_PageChanged" PrevPageText="<前一頁"
                                SubmitButtonText="Go">
                            </cc1:GridPager>
                        </td>
                    </tr>--%>
                </table>
                <cc1:CustPanel ID="pnlErrKeyin" runat="server" Height="348px" Width="100%" Visible="False">
                    <table width="100%" cellpadding="0" cellspacing="1">
                        <tr class="itemTitle1">
                            <td colspan="4">
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060600_006" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 85%">
                                <cc1:CustDropDownList ID="dropSN" runat="server">
                                </cc1:CustDropDownList>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060600_017" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 85%">
                                <cc1:CustDropDownList ID="dropErrColumn" runat="server">
                            </cc1:CustDropDownList>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060600_018" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 85%">
                                <cc1:CustTextBox ID="txtCorrectValue" runat="server" MaxLength="20" Width="280px"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060600_019" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 85%">
                                <cc1:CustDropDownList ID="dropReflectSource" runat="server">
                            </cc1:CustDropDownList>
                            </td>
                        </tr>
                        <tr class="itemTitle1">
                            <td align="center" colspan="4" >
                                    <cc1:CustButton ID="btnAdd" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                    OnClick="btnAdd_Click" OnClientClick="return checkAddInput();" Text="" Width="150px"
                                    ShowID="01_01060600_026" />
                            </td>
                        </tr>
                    </table>
                    <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                        <asp:Panel ID="Pl_gv2" runat="server" ScrollBars="auto">
                            <cc1:CustGridView ID="gvSearchASErr" runat="server" AllowSorting="True" AllowPaging="False"
                                Width="99%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid" 
                                OnRowDataBound="gvSearchASErr_RowDataBound" 
                                OnRowDeleting="gvSearchASErr_RowDeleting">
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header A" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:TemplateField>
                                        <itemtemplate>
                                            <cc1:CustLinkButton id="lbtnDelete" runat="server" CommandName="Delete" CausesValidation="False"></cc1:CustLinkButton>
                                            <asp:HiddenField ID="Err_Receive_Batch" runat="server" 
                                                 Value='<%# Eval("Receive_Batch") %>' />
                                        </itemtemplate>
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Shop_ID">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SN">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Error_Column">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Error_Value">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Correct_Value">
                                        <itemstyle width="5%" horizontalalign="Center"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Create_User">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Create_DateTime" DataFormatString="{0:yyyy/MM/dd HH:mm:ss}">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                    </asp:BoundField>
                                </Columns>
                            </cc1:CustGridView>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
