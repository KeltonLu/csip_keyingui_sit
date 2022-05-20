<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010304000001.aspx.cs" Inherits="P010304000001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust"%>
<%-- 2021/01/20_Ares_Stanley-新增查詢按鈕 --%>
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
        //* 區間起
        if (null!=document.getElementById("dtpSearchStart_foo"))
        {
            //* 區間起得到焦點時
            document.getElementById("dtpSearchStart_foo").onfocus = function(){
                document.getElementById("dtpSearchStart_foo").select();
            }
            document.getElementById("dtpSearchStart_foo").focus();
            document.getElementById("dtpSearchStart_foo").select();
            
            //* 區間起失去焦點時，
            document.getElementById("dtpSearchStart_foo").onblur = function(){
                //* 檢查日期格式是否正確
                if(!$('#dtpSearchStart_foo').attr('readonly')){
                    if(!checkDateTime(document.all.dtpSearchStart_foo)){
                        document.all.dtpSearchStart_foo.value=textValue;
                    }
                }
                
                //* 將‘區間迄’的值設置和‘區間起’相同。
                if (document.getElementById("dtpSearchEnd_foo") == document.activeElement)
                {
                    document.getElementById("dtpSearchEnd_foo").value = document.getElementById("dtpSearchStart_foo").value;    
                }
            }
        }
        
        //* 區間迄
        if (null!=document.getElementById("dtpSearchEnd_foo"))
        {
            document.getElementById("dtpSearchEnd_foo").onfocus = function(){
                document.getElementById("dtpSearchEnd_foo").select();
            }
        }
    }
    
    //* 輸入欄位正確性檢查
    function checkInput()
    {
        //* 區間起
        if (document.getElementById("dtpSearchStart_foo").value.Trim() == "")
        {
            alert("區間必需輸入資料!");
            document.getElementById("dtpSearchStart_foo").focus();
            return false;
        }
        
        //* 區間迄
        if (document.getElementById("dtpSearchEnd_foo").value.Trim() == "")
        {
            alert("區間必需輸入資料!");
            document.getElementById("dtpSearchEnd_foo").focus();
            return false;
        }
        
        var dtmSearchStart = new Date(document.getElementById("dtpSearchStart_foo").value);
        var dtmSearchEnd = new Date(document.getElementById("dtpSearchEnd_foo").value);
        if (dtmSearchStart > dtmSearchEnd)
        {
            alert("日期輸入錯誤!");
            document.getElementById("dtpSearchStart_foo").focus();
            return false;
        }
        
        return true;
    }
    
    //* 【鍵檔】RadioButton 點選時，客戶處理function
    function radClick( obj )
    {
        obj.checked = true;
        if (obj == document.getElementById("radKey1"))
            document.getElementById("radKey2").checked = false;
        else
            document.getElementById("radKey1").checked = false;
    }
    
    </script>

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
                                    SetBreak="False" SetOmit="False" ShowID="01_03040000_001" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblSearchSpan" runat="server" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_03040000_002"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 80%">
                            <cc1:DatePicker ID="dtpSearchStart" Text="" MaxLength="10" runat="server">
                            </cc1:DatePicker>
                            <cc1:DatePicker ID="dtpSearchEnd" Text="" MaxLength="10" runat="server">
                            </cc1:DatePicker>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td colspan="2">
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            &nbsp;&nbsp;&nbsp;&nbsp;
                            <cc1:CustRadioButton ID="radKey1" onclick="radClick(this)" runat="server"></cc1:CustRadioButton>
                            <cc1:CustRadioButton ID="radKey2" onclick="radClick(this)" runat="server"></cc1:CustRadioButton>
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td colspan="2" align="center">
                            <cc1:CustButton ID="btnSearch" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClientClick="return checkInput();" Text="" Width="50px" ShowID="01_03040000_006"
                                OnClick="btnSearch_Click" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnPrint" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClientClick="return checkInput();" Text="" Width="50px" ShowID="01_03040000_005"
                                OnClick="btnPrint_Click" />&nbsp;&nbsp;
                        </td>
                    </tr>
                </table>
                <table width="100%" style="table-layout:fixed">
                    <tr>
                        <td>
                            <cc1:CustGridView ID="gvbpShop" runat="server" AllowSorting="true" PagerID="gpList" Width="100%" CellSpacing="1" BorderWidth="0px" CellPadding="0" BorderStyle="Solid" Visible="false">
                            <RowStyle CssClass="Grid_Item" Wrap="false" />
                            <SelectedRowStyle CssClass="Grid_SelectedItem" />
                            <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                            <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                            <PagerSettings Visible="False" />
                            <EmptyDataRowStyle HorizontalAlign="Center" />
                            <Columns>
                            </Columns>
                            </cc1:CustGridView>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right"
                                InputBoxStyle="height:15px" OnPageChanged="gpList_PageChanged" PrevPageText="<前一頁"
                                SubmitButtonText="Go" Visible="false">
                            </cc1:GridPager>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
