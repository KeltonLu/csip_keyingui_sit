<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010211000001.aspx.cs" Inherits="P010211000001"
    EnableEventValidation="false" %>

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
    //* 畫面加載時，設置鼠標焦點
    function loadSetFocus()
    {
        $("#txtKey").focus();
        var disable = '<%=cbtn_disable %>';
        
        if (disable == "Y")
        {
            $("input[type='submit']").attr("disabled",true);
        }
    }        
    
    //* 點選【查詢】按鈕時，畫面輸入欄位的檢核
    function checkInput()
    {
        //查詢時清空備註
        document.getElementById("txt_Memo").value="";
        document.getElementById("txtPID").value="";
        
        //* 【信用卡卡號】欄位沒有輸入
        if (document.getElementById("txtKey").value.Trim() == "")
        {
            alert("信用卡卡號必須要有資料。");
            document.getElementById("txtKey").focus();
            return false;
        }
        
        //* 【信用卡卡號】欄位長度不能超過17
        if (document.getElementById("txtKey").value.Trim().length > 17)
        {
            alert("信用卡卡號欄位輸入不能超過17碼。");
            document.getElementById("txtKey").focus();
            return false;
        }
        
        //* 【信用卡卡號】欄位的英數字檢核
        //var re = /^[A-Za-z0-9]*$/;
        var re = /^[0-9]*$/;
        if (!re.test(document.getElementById("txtKey").value.Trim()))
        {
            alert("信用卡卡號欄位輸入必須全為數字。");
            document.getElementById("txtKey").focus();
            return false;
        }
        
        return true;
    }
    
    function checkPID()
    {
        //查詢時清空備註
        document.getElementById("txt_Memo").value="";
        document.getElementById("txtKey").value="";
        
            
        //* 【PID】欄位沒有輸入
        if (document.getElementById("txtPID").value.Trim() == "")
        {
            alert("PID必須要有資料。");
            document.getElementById("txtPID").focus();
            return false;
        }
        
        //* 【PID】欄位長度不能超過17
        if (document.getElementById("txtPID").value.Trim().length > 16)
        {
            alert("PID欄位輸入不能超過16碼。");
            document.getElementById("txtPID").focus();
            return false;
        }
        
        //* 【PID】欄位的英數字檢核
        var re = /^[A-Za-z0-9]*$/;
        //var re = /^[0-9]*$/;
        if (!re.test(document.getElementById("txtPID").value.Trim()))
        {
            alert("PID欄位輸入必須全為英數字。");
            document.getElementById("txtPID").focus();
            return false;
        }
        
        return true;
    }
                    
      function Import()
      {            
        if (!confirm('是否要匯入資料？'))
        {
            return false;
        }
        return true;
      }
      
      function RadioCheck(id)
      {
        $("input[type='radio']").each(function(){
            if ($(this).attr("id") == id)
            {
                $(this).attr("checked",true);
            }
            else
            {
                $(this).attr("checked",false);
            }
        });
      }
      
    </script>

</head>
<body class="workingArea" onload="loadSetFocus();">
    <form id="form1" runat="server" enctype="multipart/form-data">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <cust:image runat="server" ID="image1" />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <Triggers>
                <asp:PostBackTrigger ControlID="btnImport" />
            </Triggers>
            <ContentTemplate>
                <table width="100%" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="4" style="height: 25px">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_0211000001_015" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblKey" runat="server" CurSymbol="£" IsColon="True" ShowID="01_0211000001_001"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 35%">
                            <cc1:CustTextBox ID="txtKey" runat="server" MaxLength="16" Style="text-align: left"
                                onkeydown="entersubmit('btnSearch');"></cc1:CustTextBox>
                            <cc1:CustButton ID="btnSearch" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClick="btnSearch_Click" OnClientClick="return checkInput();" Text="" Width="50px"
                                ShowID="01_0211000001_013" /></td>
                        <td align="right" style="width: 15%">
                            <%--PID--%>
                            <cc1:CustLabel ID="CustLabel1" runat="server" CurSymbol="£" IsColon="True" ShowID="01_0211000001_004"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 35%">
                            <cc1:CustTextBox ID="txtPID" runat="server" MaxLength="16" Style="text-align: left"
                                onkeydown="entersubmit('btnPID');"></cc1:CustTextBox>
                            <cc1:CustButton ID="btnPID" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClick="btnPID_Click" OnClientClick="return checkPID();" Text="" Width="50px"
                                ShowID="01_0211000001_013" />
                        </td>
                    </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <asp:Panel ID="Pl_gv1" runat="server" ScrollBars="auto">
                                <cc1:CustGridView ID="CustGridView2" runat="server" AllowSorting="True" PagerID="gpList2"
                                    Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid"
                                    OnRowDataBound="cgv2_Bound" AllowPaging="False">
                                    <RowStyle CssClass="Grid_Item" Wrap="False" />
                                    <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                    <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                    <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                    <PagerSettings Visible="False" />
                                    <EmptyDataRowStyle HorizontalAlign="Center" />
                                    <Columns>
                                        <asp:TemplateField ShowHeader="False">
                                            <itemtemplate>
                                            <asp:RadioButton id="rb_select" GroupName="rb_selected" runat="server" 
                                            onclick="RadioCheck(this.id);">
                                            </asp:RadioButton>
                                        
</itemtemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="CARD_NMBR" HeaderText="卡號">
                                            <itemstyle width="15%" horizontalalign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="SVCP_ID" HeaderText="PID">
                                            <itemstyle width="15%" horizontalalign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="外顯註記" DataField="ID_IND" />
                                        <asp:BoundField HeaderText="有效年月" DataField="EXPIRE_DATE" />
                                        <asp:BoundField HeaderText="Bock Code" DataField="BLOCK_CODE" />
                                        <asp:BoundField HeaderText="ReasonCode" DataField="ReasonCode" />
                                        <asp:TemplateField HeaderText="原因碼">
                                            <itemtemplate>
                                            <cc1:CustDropDownList runat="server" ID="cddl_ReasonCode">
                                                <asp:ListItem Value="3">3</asp:ListItem>
                                                <asp:ListItem Value="4">4</asp:ListItem>
                                            </cc1:CustDropDownList>
                                        
</itemtemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="KEY_NMBER" DataField="KEY_NMBER" Visible="False" />
                                    </Columns>
                                </cc1:CustGridView>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel2" runat="server" CurSymbol="£" IsColon="True" ShowID="01_0211000001_010"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 65%">
                            <cc1:CustTextBox ID="txt_Memo" runat="server" Style="text-align: left"
                                Width="300px"></cc1:CustTextBox>
                        </td>
                        <td align="left" style="width: 20%">
                            <cc1:CustButton ID="BtnAdd" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                OnClick="BtnAdd_Click" Text="" Width="50px" ShowID="01_0211000001_014" /></td>
                    </tr>
                    <tr class="trEven">
                        <%--檔案匯入--%>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel3" runat="server" CurSymbol="£" IsColon="True" ShowID="01_0211000001_017"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 65%">
                            <asp:FileUpload ID="fulFilePath" runat="server" Width="80%" UNSELECTABLE="on" /><br />
                        </td>
                        <td align="left" style="width: 20%">
                            <cc1:CustButton ID="btnImport" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                Text="" Width="50px" ShowID="01_0211000001_017" OnClick="btnImport_Click" OnClientClick="return Import();" /></td>
                    </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <asp:Panel ID="pl_cgv1" runat="server" ScrollBars="auto">
                                <cc1:CustGridView ID="CustGridView1" runat="server" AllowSorting="True" Width="100%"
                                    BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid" OnRowDeleting="gv_Deleting"
                                    OnRowDataBound="cgv1_Bound" AllowPaging="False" EmptyDataText="查無資料,查詢結束!">
                                    <RowStyle CssClass="Grid_Item" Wrap="False" />
                                    <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                    <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                    <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                    <EmptyDataRowStyle HorizontalAlign="Left" />
                                    <Columns>
                                        <asp:BoundField DataField="RowNumber">
                                            <itemstyle width="5%" horizontalalign="Center" />
                                        </asp:BoundField>
                                        <asp:ButtonField Text="刪除" CommandName="Delete">
                                            <itemstyle width="10%" />
                                        </asp:ButtonField>
                                        <asp:BoundField DataField="Newid">
                                            <itemstyle width="5%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Trans_Date">
                                            <itemstyle width="15%" horizontalalign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="CardNo">
                                            <itemstyle width="15%" horizontalalign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="PID">
                                            <itemstyle width="15%" horizontalalign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Reason_Code">
                                            <itemstyle width="10%" horizontalalign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Memo">
                                            <itemstyle width="10%" horizontalalign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Process_Flag">
                                            <itemstyle width="5%" horizontalalign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Create_DateTime">
                                            <itemstyle width="10%" horizontalalign="Center" />
                                        </asp:BoundField>
                                    </Columns>
                                    <PagerSettings Visible="False" />
                                </cc1:CustGridView>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td align="center" style="width: 40%">
                            <cc1:CustButton ID="cbtn_Excel" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                Text="" Width="80px" ShowID="01_0211000001_018" OnClick="cbtn_Excel_Click" /></td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
