<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010107010001.aspx.cs" Inherits="P010107010001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <%-- 2020/11/23_Ares_Stanley-修正格式; 2021/01/21_Ares_Stanley-修正格式 --%>
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript">




        function checkInput() {
            var strDateStart = document.getElementById("dpBatchDate_foo").value.Trim();
            var strReceiveBatch = document.getElementById("txtReceiveBatch").value.Trim();
            if (strDateStart == "") {
                alert("請選擇編批日期.");
                return false;
            }
            if (strReceiveBatch == "") {
                alert("請輸入收件批次.");
                return false;
            }
            return true;
        }
        //增加提示

        function CfmMsg() {
            if (checkInput()) {
                var msg = $('#hidMessage').val();
                return confirm(msg);
            }
            else {
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
                                    SetBreak="False" SetOmit="False" ShowID="01_01070100_037" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01070100_001" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 85%">
                            <cc1:DatePicker ID="dpBatchDate" runat="server"></cc1:DatePicker>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01070100_002" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 85%">
                            <cc1:CustTextBox ID="txtReceiveBatch" runat="server" MaxLength="1" InputType="Int" Style="ime-mode: disabled; text-align: left"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td align="center" colspan="4">
                            <cc1:CustButton ID="btnSearch" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClick="btnSearch_Click" OnClientClick="return checkInput();" Text="" Width="50px"
                                ShowID="01_01070100_003" />&nbsp;&nbsp;
                                <cc1:CustButton ID="btnExportASDetail" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                    OnClick="btnExportASDetail_Click" OnClientClick="return checkInput();" Text="" Width="150px"
                                    ShowID="01_01070100_004" />
                            <cc1:CustButton ID="btnExportASgene" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClick="btnExportASgene_Click" OnClientClick="return checkInput();" Text="" Width="150px"
                                ShowID="01_01070100_005" />
                            <cc1:CustButton ID="btnExportASinst" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClick="btnExportASinst_Click" OnClientClick="return checkInput();" Text="" Width="150px"
                                ShowID="01_01070100_006" />
                            &nbsp;&nbsp;&nbsp;&nbsp;
                            <%--Talas 20191113 連回收單系統按鈕--%>
                            <cc1:CustButton ID="btnToACQ" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClick="btnToACQ_Click" OnClientClick="return CfmMsg();" Text="" Width="150px"
                                ShowID="01_01070100_038" />
                        </td>
                    </tr>
<%--                </table>
                <table width="100%" cellpadding="0" cellspacing="0">--%>
                    <tr>
                        <td align="center" colspan="4">
                            <cc1:CustGridView ID="gvpbSearchRecord" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid" OnRowDataBound="gvpbSearchRecord_RowDataBound" CssClass="longTableGridView">
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header A" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="Sign_Type">
                                        <ItemStyle Width="5%" HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Batch_NO">
                                        <ItemStyle Width="5%" HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:HyperLinkField DataTextField="Shop_ID">
                                        <ItemStyle  HorizontalAlign="Center" />
                                    </asp:HyperLinkField>
                                    <asp:BoundField DataField="Receive_Total_Count" DataFormatString="{0:N0}">
                                        <ItemStyle Width="5%" HorizontalAlign="right" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Receive_Total_AMT" DataFormatString="{0:N0}">
                                        <ItemStyle Width="5%" HorizontalAlign="right" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Keyin_Success_Count_40" DataFormatString="{0:N0}">
                                        <ItemStyle Width="5%" HorizontalAlign="right" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Keyin_Success_AMT_40" DataFormatString="{0:N0}">
                                        <ItemStyle Width="5%" HorizontalAlign="right" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Keyin_Success_Count_41" DataFormatString="{0:N0}">
                                        <ItemStyle Width="5%" HorizontalAlign="right" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Keyin_Success_AMT_41" DataFormatString="{0:N0}">
                                        <ItemStyle Width="5%" HorizontalAlign="right" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Keyin_Reject_Count_40" DataFormatString="{0:N0}" HtmlEncode="False">
                                        <ItemStyle Width="5%" HorizontalAlign="right" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Keyin_Reject_AMT_40" DataFormatString="{0:N0}" HtmlEncode="False">
                                        <ItemStyle Width="5%" HorizontalAlign="right" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Keyin_Reject_Count_41" DataFormatString="{0:N0}" HtmlEncode="False">
                                        <ItemStyle Width="5%" HorizontalAlign="right" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Keyin_Reject_AMT_41" DataFormatString="{0:N0}" HtmlEncode="False">
                                        <ItemStyle Width="5%" HorizontalAlign="right" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Adjust_Count" DataFormatString="{0:N0}">
                                        <ItemStyle Width="5%" HorizontalAlign="right" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Adjust_AMT" DataFormatString="{0:N0}" HtmlEncode="False">
                                        <ItemStyle Width="5%" HorizontalAlign="right" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Process_Flag" HtmlEncode="False">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Balance_Flag" HtmlEncode="False">
                                        <ItemStyle Width="5%" HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="KeyIn_MatchFlag" HtmlEncode="False">
                                        <ItemStyle Width="5%" HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Modify_DateTime" DataFormatString="{0:yyyy/MM/dd}">
                                        <ItemStyle HorizontalAlign="Center" />
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
                    <asp:HiddenField id="hidMessage" runat="server" Value="是否產生一般簽單高收檔及分期簽單高收檔，並上傳收單系統?"></asp:HiddenField> 
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
