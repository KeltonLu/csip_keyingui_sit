<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010213020001.aspx.cs" Inherits="P010213020001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
<title></title>

<script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

<script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

<script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

<script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

<link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

<script type="text/javascript" language="javascript">

function CheckInput()
{
    var acctID = $('#txtACCT_ID').val().Trim();
    var lprNO = $('#txtLPR_NO').val().Trim();
    // A. 車號須向左靠、中間以"-"分隔，B. 英數字皆為半形、中間去掉空白
    var keyInDate = $('#dpKeyInDate_foo').val().Trim();
    
	if(acctID == '' && lprNO == '' && keyInDate == '')
	{
		alert('請輸入條件!');
		$('#txtACCT_ID').focus();
		return false;
	}
	
	if(acctID != '')
	{
	    // ID檢核
	    if(acctID.length != 10 || !CheckIDN(acctID))
	    {
	        alert('正卡人ID輸入錯誤!');
		    return false;
	    }
	}
	
	// 車號檢核
	if(lprNO != '')
	{
	    // 必須包含符號"-"
	    if(lprNO.indexOf('-') == -1)
	    {
	        alert('車號輸入錯誤!');
		    return false;
	    }
	    else
	    {
	        lprNO = lprNO.toUpperCase();
	        var lprNO1 = lprNO.split('-')[0];
	        var lprNO2 = lprNO.split('-')[1];
            // 車牌規則
            var rule = new RegExp('^[A-Z0-9]+$');
	        
	        // 只允許輸入英文+數字
	        if(!rule.test(lprNO1) || !rule.test(lprNO2))
	        {
	            alert('車號輸入錯誤!');
		        return false;
		    }
	    }
	}
}

function CheckIDN(ID)
{
    var upperID = ID.toUpperCase();
    var strFst = upperID.substr(0, 1);
    var strLast = upperID.substr(1, 9);
    var newID = '';
    // ID規則
    var fstRule = new RegExp('^[A-Z]+$');
    var lastRule = new RegExp('^[0-9]+$');
    // 字首為英文且後9碼為數字
    if (fstRule.test(strFst) && lastRule.test(strLast))
    {
        // 身分證字號驗證邏輯
        var FirstEng = ["A", "B", "C", "D", "E", "F", "G", "H", "J"
                        , "K", "L", "M", "N", "P", "Q", "R", "S", "T"
                        , "U", "V", "X", "Y", "W", "Z", "I", "O"];
        // 判斷第2碼
        var genderCode = ID.substr(1, 1);
        if (genderCode != '1' && genderCode != '2') return false;
        
        for (i = 0; i < FirstEng.length; i++)
        {
            if (strFst == FirstEng[i])
            {
                newID = i + 10 + ID.substr(1, 9);
                break;
            }
        }
        
        var j = 1;
        var checkCode = parseInt(newID.substr(0, 1));
        
        while (newID.length > j)
        {
            checkCode = checkCode + (parseInt(newID.substr(j, 1)) * (10 - j));
            j++;
        }
        
        newID = checkCode.toString();
        var currentCode = newID.substr(newID.length - 1, 1);

        if (ID.substr(9, 1) == '0')
        {
            if (currentCode == '0')
                return true;
            else
                return false;
        }
        else
        {
            if (ID.substr(9, 1) == (10 - parseInt(currentCode)).toString())
                return true;
            else
                return false;
        }
    }
}

</script>

<style type="text/css">

</style>
</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <cust:image runat="server" ID="image1" />
            
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1">
                        <tr class="itemTitle">
                            <td colspan="6">
                                <li>
                                    <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" 
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" Width="200px" ShowID="01_02130200_001" />
                                </li>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 10%" align="right">
                                <cc1:CustLabel ID="lblACCT_ID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" 
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" 
                                    SetOmit="False" StickHeight="False" ShowID="01_02130200_002" />
                            </td>
                            <td style="width: 15%">
                                <cc1:CustTextBox ID="txtACCT_ID" runat="server" MaxLength="10" checktype="num" BoxName="正卡人ID" />
                            </td>
                            <td style="width: 10%" align="right">
                                <cc1:CustLabel ID="lblLPR_NO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" 
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" 
                                    SetOmit="False" StickHeight="False" ShowID="01_02130200_003" />
                            </td>
                            <td style="width: 15%">
                                <cc1:CustTextBox ID="txtLPR_NO" runat="server" MaxLength="9" checktype="num" BoxName="車牌號碼" />
                                <%--<cc1:CustTextBox ID="txtLPR_NO1" runat="server" MaxLength="4" checktype="num" BoxName="車牌號碼1" Width="50px" />
                                &nbsp;-&nbsp;
                                <cc1:CustTextBox ID="txtLPR_NO2" runat="server" MaxLength="4" checktype="num" BoxName="車牌號碼2" Width="50px" />--%>
                            </td>
                            <td style="width: 10%" align="right">
                                <cc1:CustLabel ID="lblKeyInDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" 
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" 
                                    SetOmit="False" StickHeight="False" ShowID="01_02130200_004" />
                            </td>
                            <td style="width: 40%">
                                <cc1:DatePicker ID="dpKeyInDate" runat="server"></cc1:DatePicker>
                                <%--<cc1:CustTextBox ID="txtKeyInDate" runat="server" MaxLength="8" checktype="num" BoxName="鍵檔日期" />--%>
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <cc1:CustButton ID="btnSelect" CssClass="smallButton" runat="server" Width="40px"
                                    DisabledWhenSubmit="False" ShowID="01_02130200_005" OnClick="btnSelect_Click"
                                    OnClientClick="return CheckInput();" />
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1">
                        <tr class="itemTitle">
                            <td colspan="2"></td>
                        </tr>
                    </table>
                    <asp:Panel ID="Panel1" runat="server" ScrollBars="Horizontal" style="overflow:scroll">
                        <table width="100%" border="0" cellpadding="0" cellspacing="1" id="FromTable">
                            <tr>
                                <td>
                                    <cc1:CustGridView ID="gvpbCase" runat="server" AllowSorting="True" PagerID="gpList"
                                        Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid">
                                        <RowStyle CssClass="Grid_Item" Wrap="False" />
                                        <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                        <HeaderStyle CssClass="Grid_Header A" Wrap="False" />
                                        <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                        <PagerSettings Visible="False" />
                                        <EmptyDataRowStyle HorizontalAlign="Center" />
                                        <Columns>
                                            <asp:BoundField DataField="COUNT"></asp:BoundField><%--序號--%>
                                            <asp:BoundField DataField="TYPE"></asp:BoundField><%--代扣種類--%>
                                            <asp:BoundField DataField="ACCT_ID"><itemstyle horizontalalign="Center" /></asp:BoundField><%--正卡人ID--%>
                                            <asp:BoundField DataField="LPR_NO"><itemstyle horizontalalign="Center" /></asp:BoundField><%--車號--%>
                                            <asp:BoundField DataField="CUSTID"><itemstyle horizontalalign="Center" /></asp:BoundField><%--車主ID--%>
                                            <asp:BoundField DataField="STATUS"><itemstyle horizontalalign="Center" /></asp:BoundField><%--狀態碼(1.申請 2.終止)--%>
                                            <asp:BoundField DataField="NO"><itemstyle horizontalalign="Center" /></asp:BoundField><%--月租停車場代碼--%>
                                            <asp:BoundField DataField="RESULT_A_CODE"><itemstyle horizontalalign="Center" /></asp:BoundField><%--申請回覆碼--%>
                                            <asp:BoundField DataField="RESULT_D_CODE"><itemstyle horizontalalign="Center" /></asp:BoundField><%--終止回覆碼--%>
                                            <asp:BoundField DataField="APPLY_NO"><itemstyle horizontalalign="Center" /></asp:BoundField><%--收件編號(16碼)--%>
                                            <asp:BoundField DataField="KEYIN_DATE"><itemstyle horizontalalign="Center" /></asp:BoundField><%--鍵檔日--%>
                                            <asp:BoundField DataField="UPDATE_USER"><itemstyle horizontalalign="Center" /></asp:BoundField><%--維護員--%>
                                            <asp:BoundField DataField="SERVICE_A_DATE"><itemstyle horizontalalign="Center" /></asp:BoundField><%--申請日期--%>
                                            <asp:BoundField DataField="UPDATE_DATE"><itemstyle horizontalalign="Center" /></asp:BoundField><%--維護日期--%>
                                            <asp:BoundField DataField="SERVICE_D_DATE"><itemstyle horizontalalign="Center" /></asp:BoundField><%--終止日期--%>
                                            <asp:BoundField DataField="SEND_DATE"><itemstyle horizontalalign="Center" /></asp:BoundField><%--媒體傳送日--%>
                                            <asp:BoundField DataField="EFF_DATE"><itemstyle horizontalalign="Center" /></asp:BoundField><%--生效日--%>
                                            <asp:BoundField DataField="SPC"><itemstyle horizontalalign="Center" /></asp:BoundField><%--推廣代號 --%>
                                            <asp:BoundField DataField="SPC_ID"><itemstyle horizontalalign="Center" /></asp:BoundField><%--推廣員編--%>
                                            <asp:BoundField DataField="INTRODUCER_CARD_ID"><itemstyle horizontalalign="Center" /></asp:BoundField><%--種子--%>
                                            <asp:BoundField DataField="SOURCE"><itemstyle horizontalalign="Center" /></asp:BoundField><%--來源--%>
                                        </Columns>
                                    </cc1:CustGridView>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Center"
                                        InputBoxStyle="height:15px" OnPageChanged="gpList_PageChanged" PrevPageText="<前一頁"
                                        SubmitButtonText="Go">
                                    </cc1:GridPager>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <table width="100%" border="0" cellpadding="0" cellspacing="1">
                <tr class="itemTitle" align="center">
                    <td colspan="2">
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
