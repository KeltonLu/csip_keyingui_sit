<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010403000001.aspx.cs" Inherits="P010403000001" %>

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
    //* 畫面加載時，光標位置設定
    function loadSetFocus()
    {
        object = document.getElementById("txtDateInput");
        if (object!=null)
            object.focus();
    }
    
    //* 檢查輸入的日期格式是否正確
    function checkDate( obj )
    {
        if (null==obj)
            return 0;
        
        //* 檢查是否輸入
        if (obj.value.Trim() == "")
        {
            return -1;
        }
        else
        {
            //* 檢查輸入的長度是否正確
            if (obj.value.Trim().length != 8)
            {
                return -2;
            }else
            {
                try
                {
                    //* 非法字符檢查
                    var patten=new RegExp(/^[0-9]*$/); 
                    if (!patten.test(obj.value.Trim()))
                        return -2;
                    
                    //* 加19110000，轉換成西元日期格式
                    var intDateSend = parseFloat(obj.value.Trim()) + 19110000;
                    year = intDateSend.toString().substring(0,4);
                    month = intDateSend.toString().substring(4,6);
                    day = intDateSend.toString().substring(6,8);
                    if (year < 1000 || year > 9999){
                        return -2;
                    }
                    if (month < 1 || month > 12){
                        return -2;
                    }
                    if (day < 1 || day > 31){
                        return -2;
                    }
                    if ((month==4 || month==6 || month==9 || month==11) && day==31){
                        return -2;
                    }
                    if (month==2){
                        var isleap=(year % 4==0 && (year % 100 !=0 || year % 400==0));
                        
                        if (day>29){
                            return -2;
                        }
                        if ((day==29) && (!isleap)){
                            return -2;
                        }
                    }
                    
                    //* 生成日期返回
                    var strDateSend = intDateSend.toString().substring(0,4) + "/" + intDateSend.toString().substring(4,6) 
                            + "/" + intDateSend.toString().substring(6,8);
                    var datDateSend = new Date(strDateSend);
                    return datDateSend;
                }catch(e)
                {
                    return -2;
                }
            }
        }
    }
    
    //* 輸入欄位正確性檢查
    function checkSearch()
    {
        //* 送檔日格式檢查
        var dtmDateSend = checkDate(document.getElementById("txtDateSend"));
        if (dtmDateSend==-2)
        {
            alert("送檔日日期格式不正確，請重新填寫日期！");
            document.getElementById("txtDateSend").focus();
            return false;
        }
        
        //* 首錄日格式檢查
        var dtmDateInput = checkDate(document.getElementById("txtDateInput"));
        if (dtmDateInput==-2)
        {
            alert("首錄日日期格式不正確，請重新填寫日期！");
            document.getElementById("txtDateInput").focus();
            return false;
        }
        
        //* 收檔日格式檢查
        var dtmDateReceive = checkDate(document.getElementById("txtDateReceive"));
        if (dtmDateReceive==-2)
        {
            alert("收檔日日期格式不正確，請重新填寫日期！");
            document.getElementById("txtDateReceive").focus();
            return false;
        }
        
        return true;
    }
    
    //* 輸入欄位正確性檢查
    function checkInput()
    {
    
        
         
        //* 送檔日格式檢查
        var dtmDateSend = checkDate(document.getElementById("txtDateSend"));
        if (dtmDateSend==-1)
        {
            alert("送檔日必須輸入！");
            document.getElementById("txtDateSend").focus();
            return false;
        }else if (dtmDateSend == -2)
        {
            alert("送檔日日期格式不正確，請重新填寫日期！");
            document.getElementById("txtDateSend").focus();
            return false;
        }
        
        //* 首錄日格式檢查
        var dtmDateInput = checkDate(document.getElementById("txtDateInput"));
        if (dtmDateInput==-1)
        {
            alert("首錄日必須輸入！");
            document.getElementById("txtDateInput").focus();
            return false;
        }else if (dtmDateInput == -2)
        {
            alert("首錄日日期格式不正確，請重新填寫日期！");
            document.getElementById("txtDateInput").focus();
            return false;
        }
        
        //* 收檔日格式檢查
        var dtmDateReceive = checkDate(document.getElementById("txtDateReceive"));
        if (dtmDateReceive==-1)
        {
            alert("收檔日必須輸入！");
            document.getElementById("txtDateReceive").focus();
            return false;
        }else if (dtmDateReceive == -2)
        {
            alert("收檔日日期格式不正確，請重新填寫日期！");
            document.getElementById("txtDateReceive").focus();
            return false;
        }
        
        //* 當日
        var today = new Date();
        //* 確認收檔日>首錄日>送檔日>今日日期
        if (dtmDateReceive <= dtmDateInput || 
            dtmDateInput <= dtmDateSend || 
            dtmDateSend <= today )
        {
            
            alert("請確認收檔日>首錄日>送檔日>今日日期！");
            document.getElementById("txtDateSend").focus();
            return false;
        }
        
        //return true;
        
         if(!confirm('是否要添加屬性設定資料？'))
         {
                    return false;
         }
    }
    
    //* 點選【Reset】鍵時，檢查【首錄日】是否輸入及輸入的日期格式是否正確
    function checkInputReset()
    {
        return confirm("是否要Reset屬性設定資料");
        
        //* 首錄日格式檢查
        var dtmDateInput = checkDate(document.getElementById("txtDateInput"));
        if (dtmDateInput==-1)
        {
            alert("首錄日必須輸入！");
            document.getElementById("txtDateInput").focus();
            return false;
        }else if (dtmDateInput == -2)
        {
            alert("首錄日日期格式不正確，請重新填寫日期！");
            document.getElementById("txtDateInput").focus();
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
                                    SetBreak="False" SetOmit="False" ShowID="01_04030000_001" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblDateSend" runat="server" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04030000_002"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 85%">
                            <cc1:CustTextBox ID="txtDateSend" runat="server" ValidationGroup="save" MaxLength="8"
                                IsValEmpty="false" onkeydown="nosubmit();"></cc1:CustTextBox>
                            (yyyymmdd)
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblDateInput" runat="server" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04030000_003"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 85%">
                            <cc1:CustTextBox ID="txtDateInput" runat="server" ValidationGroup="save" MaxLength="8"
                                IsValEmpty="false" onkeydown="nosubmit();"></cc1:CustTextBox>
                            (yyyymmdd)
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblDateReceive" runat="server" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_04030000_004"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 85%">
                            <cc1:CustTextBox ID="txtDateReceive" runat="server" ValidationGroup="save" MaxLength="8"
                                IsValEmpty="false" onkeydown="nosubmit();"></cc1:CustTextBox>
                            (yyyymmdd)
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td colspan="3" align="center">
                            <cc1:CustButton ID="btnAdd" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClick="btnAdd_Click" OnClientClick="return checkInput();" Text="" Width="50px"
                                ShowID="01_04030000_005" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnSearch" OnClick="btnSearch_Click" runat="server" ShowID="01_04030000_006"
                                Width="50px" Text="" OnClientClick="return checkSearch();" DisabledWhenSubmit="False"
                                CssClass="smallButton"></cc1:CustButton>&nbsp;&nbsp;
                            <cc1:CustButton ID="btnReset" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClick="btnReset_Click" OnClientClick="return checkInputReset();" Text="" Width="50px"
                                ShowID="01_04030000_007" /></td>
                    </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td colspan="2">
                            <cc1:CustGridView ID="gvpbACH" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid"
                                OnRowDataBound="gvpbACH_RowDataBound">
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header A" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="dateSend">
                                        <itemstyle width="15%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="dateInput">
                                        <itemstyle width="15%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="dateReceive">
                                        <itemstyle width="15%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="P02_flag">
                                        <itemstyle width="15%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CountUpdate">
                                        <itemstyle width="20%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CountTotal">
                                        <itemstyle horizontalalign="right" width="20%" />
                                    </asp:BoundField>
                                </Columns>
                            </cc1:CustGridView>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
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
