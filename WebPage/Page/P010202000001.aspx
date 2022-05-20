<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010202000001.aspx.cs" Inherits="P010202000001" %>

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
        //* 首錄日填寫不完整
        if (document.getElementById("txtInputDateStart").value.Trim() == "" || 
            document.getElementById("txtInputDateEnd").value.Trim() == "" )
        {
            alert("請將keyin日起迄日期資料填寫完整！");
            if (document.getElementById("txtInputDateStart").value.Trim() == "")
            {
                document.getElementById("txtInputDateStart").focus();
                return false;
            }
            if (document.getElementById("txtInputDateEnd").value.Trim() == "")
            {
                document.getElementById("txtInputDateEnd").focus();
                return false;
            }
        }
        
        //* 查詢條件都未填寫
        if (document.getElementById("txtInputDateStart").value.Trim() == "" &&
            document.getElementById("txtInputDateEnd").value.Trim() == "")
        {
            alert("請填寫查詢或列印條件！");
            document.getElementById("txtInputDateStart").focus();
            return false;
        }
            
        //* 日期格式檢查
        var dtmInputDateStart = checkDateSn((parseFloat(document.getElementById("txtInputDateStart").value.Trim()) - 19110000).toString());
        
        if (-2==dtmInputDateStart)
        {
            alert("日期格式不正確，請重新填寫日期！");
            document.getElementById("txtInputDateStart").focus();
            return false;
        }
        
        //* 日期格式檢查
        var dtmInputDateEnd = checkDateSn((parseFloat(document.getElementById("txtInputDateEnd").value.Trim()) - 19110000).toString());
        if (-2==dtmInputDateEnd)
        {
            alert("日期格式不正確，請重新填寫日期！");
            document.getElementById("txtInputDateEnd").focus();
            return false;
        }
        
        //* 日期格式檢查
        if (dtmInputDateStart > dtmInputDateEnd)
        {
            alert("keyin日迄日必須不小於起日！");
            document.getElementById("txtInputDateStart").focus();
            return false;
        }
        
        if (document.getElementById("txtReceiveNumber").value.Trim() != "")
        {
            //*檢核輸入欄位【收件編號】前兩碼需為            if (document.getElementById("txtReceiveNumber").value.Trim().substring(0,2).toUpperCase() != "RU")
            {
                alert("收件編號格式不對！");
                document.getElementById('txtReceiveNumber').focus();

                return false;
            }
            
            if (checkDateSn(document.getElementById("txtReceiveNumber").value.Trim().substring(2,9))==-2)
            {
                alert("收件編號格式不對！");
                document.getElementById('txtReceiveNumber').focus();

                return false;
            }
            
            //*檢核收件編號是否輸入正確
            if(document.getElementById('txtReceiveNumber').value.Trim().length!=12)
            {
                alert('收件編號格式不對！');
                document.getElementById('txtReceiveNumber').focus();
               
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
        <cust:image runat="server" ID="image1" />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="2">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_02020000_001" StickHeight="False"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblInput_Date" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_02020000_002"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 80%">
                            <cc1:CustTextBox ID="txtInputDateStart" runat="server" MaxLength="8" onkeydown="nosubmit();"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtInputDateEnd" runat="server" MaxLength="8" onkeydown="nosubmit();"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right">
                            <cc1:CustLabel ID="lbReceiveNumber" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_02020000_004"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td>
                            <cc1:CustTextBox ID="txtReceiveNumber" runat="server" MaxLength="12" onkeydown="nosubmit();"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td colspan="2" align="center">
                            <cc1:CustButton ID="btnSearch" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClientClick="return checkInput();" Text="" Width="50px" ShowID="01_02020000_003"
                                OnClick="btnSearch_Click" />
                        </td>
                    </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <cc1:CustGridView ID="gvList" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid" OnRowDataBound="gvList_RowDataBound">
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="RECEIVE_NUMBER">
                                        <itemstyle width="40%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="UPDATE_DATE">
                                        <itemstyle width="30%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SEND3270">
                                        <itemstyle width="30%" horizontalalign="Center" />
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
