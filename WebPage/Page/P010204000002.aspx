<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010204000002.aspx.cs" Inherits="P010204000002" %>

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
    //* 畫面加載時，設置光標
    function loadSetFocus()
    {
        object = document.getElementById("txtSubject");
        if (object!=null)
            object.focus();
    }
    
    //* 點選【添加】或【修改】按鈕時，畫面輸入正確性檢查
    function checkInput()
    {
        //* 作業項目沒有輸入時
        if (document.getElementById("txtSubject").value.Trim() == "")
        {
            alert("作業項目必須輸入");
            document.getElementById('txtSubject').focus();
            return false;
        }
        //* 作業項目長度超過時
        if (document.getElementById("txtSubject").value.length > 20)
        {
            alert("作業項目欄位輸入不能超過20碼");
            document.getElementById('txtSubject').focus();
            return false;
        }
        
        //* 訊息內容必須輸入
        if (document.getElementById("txtContent").value.Trim() == "")
        {
            alert("訊息內容必須輸入");
            document.getElementById('txtContent').focus();
            return false;
        }
        
        //* 訊息內容長度不能超過255
        if (document.getElementById("txtContent").value.length > 255)
        {
            alert("訊息內容欄位輸入不能超過255碼");
            document.getElementById('txtContent').focus();
            return false;
        }
        
        if (document.getElementById("hidCode").value == "")
            return confirm("是否要添加公佈欄?"); //* 修改時
            //return true;//* 添加時
        else
            return confirm("是否要修改公佈欄?"); //* 修改時
    }
    
    
     //* 點選【刪除】按鈕時，彈出確認窗體
    function checkInputD()
    {

        return confirm("是否要刪除公佈欄?"); //* 刪除時
    }
    </script>
</head>
<body class="workingArea" onload="javascript:loadSetFocus();">
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
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 25%">
                            <cc1:CustLabel ID="lblStatus" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_04020000_015" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 75%">
                            <cc1:CustRadioButtonList ID="radlStatus" runat="server" RepeatDirection="Horizontal"
                                Width="97px">
                                <asp:ListItem Selected="True" Value="Y">
                                </asp:ListItem>
                                <asp:ListItem Value="N">
                                </asp:ListItem>
                            </cc1:CustRadioButtonList></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 25%">
                            <cc1:CustLabel ID="lblSubject" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_04020000_016" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 75%">
                            <cc1:CustTextBox ID="txtSubject" runat="server" IsValEmpty="false" MaxLength="20"
                                ValidationGroup="save" Width="342px" onkeydown="entersubmit('btnAddUpdate');"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 25%">
                            <cc1:CustLabel ID="lblImportant_status" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04020000_017"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 75%">
                            <cc1:CustLabel ID="lblPrompt" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_04020000_018" StickHeight="False"></cc1:CustLabel>&nbsp;
                            <cc1:CustRadioButton ID="rabImportant_status_yes" runat="server" AutoPostBack="True"
                                OnCheckedChanged="rabImportant_status_yes_CheckedChanged" />&nbsp;&nbsp;
                            <cc1:CustRadioButton ID="rabImportant_status_no" runat="server" AutoPostBack="True"
                                OnCheckedChanged="rabImportant_status_no_CheckedChanged" />
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 25%" valign="top" >
                            <div style="line-height:3pt">&nbsp;<br></div>
                            <cc1:CustLabel ID="lblContent" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_04020000_019" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 75%">
                            <cc1:CustTextBox ID="txtContent" runat="server" ReadOnly="false" TextMode="MultiLine" Height="112px" Width="345px" MaxLength="255" InputType="Memo" onkeydown="nosubmit();"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td colspan="2" align="center">
                            <cc1:CustButton ID="btnAddUpdate" Text="" runat="server" CssClass="smallButton" Width="50px" 
                                DisabledWhenSubmit="False" OnClick="btnAddUpdate_Click" OnClientClick="return checkInput();" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnDelete" Text="" runat="server" CssClass="smallButton" Width="50px" DisabledWhenSubmit="False" ShowID="01_04020000_022"
                                OnClick="btnDelete_Click" OnClientClick="return checkInputD();"/>&nbsp;&nbsp;
                            <cc1:CustButton ID="btnReturn" Text="" runat="server" CssClass="smallButton" Width="50px"
                                UseSubmitBehavior="False" DisabledWhenSubmit="False" ShowID="01_04020000_023"
                                OnClick="btnReturn_Click" /></td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <cc1:CustHiddenField ID="hidCode" Value="" runat="server" />
    </form>
</body>
</html>
