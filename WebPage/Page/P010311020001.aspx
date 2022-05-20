<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010311020001.aspx.cs" Inherits="P010311020001" %>

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
        var sendFileDate = document.getElementById("txtSendFileDate").value.Trim();
        var sReplyDate = document.getElementById("txtSReplyDate").value.Trim();
        var eReplyDate = document.getElementById("txtEReplyDate").value.Trim();
        
        if(sendFileDate == "" && sReplyDate == "" && eReplyDate == "")
        {
            alert("至少輸入一種日期資料！");
            document.getElementById("txtSendFileDate").focus();
            return false;
        }
        
        if(sendFileDate != "" && (sReplyDate != "" || eReplyDate != ""))
        {
            alert("至多輸入一種日期資料！");
            document.getElementById("txtSendFileDate").focus();
            return false;
        }
        
        if(sReplyDate != "" || eReplyDate != "")
        {
            if(sReplyDate == "")
            {
                alert("請輸入回覆日期(起)！");
                document.getElementById("txtSReplyDate").focus();
                return false;
            }
            
            if(eReplyDate == "")
            {
                alert("請輸入回覆日期(迄)！");
                document.getElementById("txtEReplyDate").focus();
                return false;
            }
        }
        
        if(sendFileDate != "")
        {
            var dtmSendFileDate = checkDateReport(document.getElementById("txtSendFileDate"));
            if (-2==dtmSendFileDate)
            {
                alert("日期格式不正確，請重新填寫日期！");
                document.getElementById("txtSendFileDate").focus();
                return false;
            }
        }
        
        if(sReplyDate != "")
        {
            var dtmSendFileDate = checkDateReport(document.getElementById("txtSReplyDate"));
            if (-2==dtmSendFileDate)
            {
                alert("日期格式不正確，請重新填寫日期！");
                document.getElementById("txtSReplyDate").focus();
                return false;
            }
        }
        
        if(sReplyDate != "")
        {
            var dtmSendFileDate = checkDateReport(document.getElementById("txtEReplyDate"));
            if (-2==dtmSendFileDate)
            {
                alert("日期格式不正確，請重新填寫日期！");
                document.getElementById("txtEReplyDate").focus();
                return false;
            }
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
                        <td colspan="2">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_03110200_001" StickHeight="False"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblInput_Date" runat="server" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_03110200_002"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 80%">
                            <cc1:CustTextBox ID="txtSendFileDate" runat="server" MaxLength="8" onkeydown="nosubmit();"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblReply_Date" runat="server" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_03110200_021"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 80%">
                            <cc1:CustTextBox ID="txtSReplyDate" runat="server" MaxLength="8" onkeydown="nosubmit();"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtEReplyDate" runat="server" MaxLength="8" onkeydown="nosubmit();"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_03110200_022"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 80%">
                            <cc1:CustDropDownList ID="dropPostRtnMsg" kind="select" runat="server" AutoPostBack="False" />
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 20%">&nbsp;&nbsp;</td>
                        <td style="width: 80%">
                            <cc1:CustCheckBoxList id="radlSearchType" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem>
                                </asp:ListItem>
                                <asp:ListItem>
                                </asp:ListItem>
                            </cc1:CustCheckBoxList>
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td colspan="2" align="center">
                            <cc1:CustButton ID="btnSearch" runat="server" CssClass="smallButton" DisabledWhenSubmit="False" 
                                Text="" Width="50px" ShowID="01_03110200_005" OnClientClick="return checkInput();"
                                OnClick="btnSearch_Click" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnPrint" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                Text="" Width="50px" ShowID="01_03110200_007" OnClientClick="return checkInput();"
                                OnClick="btnPrint_Click" />
                        </td>
                    </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <cc1:CustGridView ID="gvpbR02Record" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid"
                                OnRowDataBound="gvpbR02Record_RowDataBound">
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField>
                                        <itemstyle width="5%" horizontalalign="Center" /><%--序號--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ReceiveNumber">
                                        <itemstyle width="10%" horizontalalign="Center" /><%--收件編號--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="AccNoBank">
                                        <itemstyle width="10%" horizontalalign="Center" /><%--收受行(核印行)--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="PROPERTY_NAME">
                                        <itemstyle width="12%" horizontalalign="Center" /><%--收受行名稱(核印行)--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="AccID">
                                        <itemstyle width="13%" horizontalalign="Center" /><%--委繳戶統編\身分證字號--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="AccNo">
                                        <itemstyle width="6%" horizontalalign="Center" /><%--委繳戶帳號--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CusID">
                                        <itemstyle width="8%" horizontalalign="Center" /><%--持卡人ID--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ApplyType">
                                        <itemstyle width="8%" horizontalalign="Center" /><%--申請類別--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ReturnStatus">
                                        <itemstyle width="8%" horizontalalign="Center" /><%--成功/失敗--%>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="PostRtnMsg">
                                        <itemstyle width="15%" horizontalalign="left"/><%--回覆訊息--%>
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
