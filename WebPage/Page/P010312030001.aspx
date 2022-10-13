<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010312030001.aspx.cs" Inherits="P010312030001" %>

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
    
    // 輸入欄位正確性檢查
    function checkInput()
    {
        var txtBatchDateStart = document.getElementById("txtBatchDateStart").value.Trim();
        if (txtBatchDateStart == "")
        {
            alert("請輸入批次起日！");
            document.getElementById("txtBatchDateStart").focus();
            return false;
        }
        if (txtBatchDateStart != "")
        {
            var dtmDownloadFileDate = checkDateReport(document.getElementById("txtBatchDateStart"));
            if (-2==dtmDownloadFileDate)
            {
                alert("日期格式不正確，請重新填寫日期！");
                document.getElementById("txtBatchDateStart").focus();
                return false;
            }
        }

        var txtBatchDateEnd = document.getElementById("txtBatchDateEnd").value.Trim();
        if (txtBatchDateEnd == "") {
            alert("請輸入批次迄日！");
            document.getElementById("txtBatchDateEnd").focus();
            return false;
        }
        if (txtBatchDateEnd != "") {
            var dtmDownloadFileDate = checkDateReport(document.getElementById("txtBatchDateEnd"));
            if (-2 == dtmDownloadFileDate) {
                alert("日期格式不正確，請重新填寫日期！");
                document.getElementById("txtBatchDateEnd").focus();
                return false;
            }
        }

        //* 檢核 起日 > 迄日”
        if (txtBatchDateStart > txtBatchDateEnd) {
            alert("批次迄日必須不小於起日！");
            document.getElementById("txtBatchDateStart").focus();
            return false;
        }

        return true;
    }
    
    </script>
    
    <style type="text/css" >
    .btnHiden
    {display:none; }
    </style>
</head>
<body class="workingArea">
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
                                    SetBreak="False" SetOmit="False" ShowID="01_03120300_001" StickHeight="False"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblCus_ID" runat="server" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_03120300_002"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td  colspan="3">
                            <cc1:CustTextBox ID="txtCus_ID" runat="server" MaxLength="20" onkeydown="nosubmit();"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" >
                            <cc1:CustLabel ID="lblBatchDateStart" runat="server" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_03120300_003"
                                StickHeight="False">
                            </cc1:CustLabel>
                        </td>
                        <td >
                            <cc1:CustTextBox ID="txtBatchDateStart" runat="server" MaxLength="8" onkeydown="nosubmit();"></cc1:CustTextBox>
                        </td>
                        <td align="right" >
                            <cc1:CustLabel ID="lblBatchDateEnd" runat="server" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_03120300_004"
                                StickHeight="False">
                            </cc1:CustLabel>
                        </td>
                        <td >
                            <cc1:CustTextBox ID="txtBatchDateEnd" runat="server" MaxLength="8" onkeydown="nosubmit();"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" >
                            <cc1:CustLabel ID="lblComparisonStatus" runat="server" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_03120300_005"
                                StickHeight="False">
                            </cc1:CustLabel>
                        </td>
                        <td colspan="3">
                            <cc1:CustRadioButton ID="radComparisonStatus_0" runat="server" Text="待比對" GroupName="ComparisonStatus" Checked="True" />
                            <cc1:CustRadioButton ID="radComparisonStatus_1" runat="server" Text="正常" GroupName="ComparisonStatus"/>
                            <cc1:CustRadioButton ID="radComparisonStatus_2" runat="server" Text="缺少網銀資料" GroupName="ComparisonStatus"/>
                            <cc1:CustRadioButton ID="radComparisonStatus_3" runat="server" Text="網銀異常資料" GroupName="ComparisonStatus"/>
                            <cc1:CustRadioButton ID="radComparisonStatus_4" runat="server" Text="全部" GroupName="ComparisonStatus"/>
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td colspan="4" align="center">
                            <cc1:CustButton ID="btnSearch" runat="server" CssClass="smallButton" DisabledWhenSubmit="False" 
                                Text="" Width="50px" ShowID="01_03120300_006" OnClientClick="return checkInput();"
                                OnClick="btnSearch_Click" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnPrint" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                Text="" Width="50px" ShowID="01_03120300_007" OnClientClick="return checkInput();"
                                OnClick="btnPrint_Click" />
                        </td>
                    </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <cc1:CustGridView ID="gvpbR02Record" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid" >
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="BatchDate">
                                        <itemstyle horizontalalign="Center" /><%--批次日期--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ComparisonStatus">
                                        <itemstyle horizontalalign="Center" /><%--資料比對狀態--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="UploadTime">
                                        <itemstyle horizontalalign="Center" /><%--上傳主機時間--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Deal_S_No">
                                        <itemstyle horizontalalign="Center" /><%--交易序號(ACH)--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Deal_No">
                                        <itemstyle horizontalalign="Center" /><%--交易代號(EDDA)--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="AuthCode">
                                        <itemstyle horizontalalign="Center" /><%--授權編號--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Cus_ID">
                                        <itemstyle horizontalalign="Center" /><%--用戶號碼--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Apply_Type">
                                        <itemstyle horizontalalign="Center" /><%--新增或取消--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ApplyDate">
                                        <itemstyle horizontalalign="Center"/><%--申請之交易日期--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="AccNoBank">
                                        <itemstyle horizontalalign="Center"/><%--他行行庫代碼--%>
                                    </asp:BoundField>
                                     <asp:BoundField DataField="AccNo">
                                        <itemstyle horizontalalign="Center"/><%--他行銀行帳號--%>
                                    </asp:BoundField>
                                     <asp:BoundField DataField="PayWay">
                                        <itemstyle horizontalalign="Center"/><%--繳款方式--%>
                                    </asp:BoundField>
                                     <asp:BoundField DataField="Reply_Info">
                                        <itemstyle horizontalalign="Center"/><%--回復訊息--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="AccID">
                                        <itemstyle horizontalalign="Center"/><%--自扣者ID--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SalesChannel">
                                        <itemstyle horizontalalign="Center"/><%--推廣通路代碼--%>
                                    </asp:BoundField>
                                     <asp:BoundField DataField="SalesUnit">
                                        <itemstyle horizontalalign="Center"/><%--推廣單位代碼--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SalesEmpoNo">
                                        <itemstyle horizontalalign="Center"/><%--推廣員員編--%>
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
