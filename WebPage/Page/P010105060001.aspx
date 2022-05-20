<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010105060001.aspx.cs" Inherits="P010105060001" %>

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
        function checkInputText() {
            var idNum = document.getElementById('txtIdNum').value;
            if (idNum == "") {
                alert("請輸入身分證字號");
                document.getElementById('txtIdNum').focus();
                return false;
            }
            return true;
        }
    </script>

    <style type="text/css">
.btnHiden
{display:none; }
</style>
</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <cust:image runat="server" ID="image1" />
        &nbsp;
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                    <tr class="itemTitle">
                        <td>
                            <li>
                                <cc1:CustLabel ID="Title" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01090300_001"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="left" style="width: 100%">
                            <%-- 查詢條件 --%>
                            <div style="margin: 5px">
                                <cc1:CustLabel ID="CustLabel10" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090300_002" StickHeight="False"></cc1:CustLabel>
                            </div>
                            <%--身分證字號--%>
                            <div style="margin: 5px">
                                <cc1:CustLabel ID="idNum" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090300_003" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtIdNum" runat="server" MaxLength="10" checktype="ID" Width="120px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證字號" InputType="LetterAndInt" Style="margin-right: 40px"></cc1:CustTextBox>

                                <%--曾用過的姓名--%>
                                <cc1:CustLabel ID="usedName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090300_004" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtUsedName" runat="server" MaxLength="50" Width="120px" 
                                    onkeydown="entersubmit('btnAdd');" TabIndex="21" BoxName="曾用過的姓名" onpaste="paste();"></cc1:CustTextBox>

                                <cc1:CustButton ID="btnIdNum" runat="server" CssClass="smallButton" ShowID="01_01090300_005"                                    
                                    OnClick="btnSelect_Click" DisabledWhenSubmit="False" BoxName="查詢" />
                            </div> 
                        </td>
                    </tr>
                </table>
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo4" style="">                        
                        <tr class="trEven">
                            <%-- 查詢結果 --%>                    
                            <td align="left" style="width: 100%">
                                <cc1:CustLabel ID="CustLabel11" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090300_006" StickHeight="False" Style="margin-left:5px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%-- 查詢結果 --%>   
                        <tr>
                        <td>
                            <cc1:CustGridView ID="gvpbCodeInfo" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid"
                                >
                                <Columns>
                                    <asp:BoundField DataField="ID" HeaderText="身分證字號">
                                        <ItemStyle Width="40%" />
                                        <HeaderStyle Width="40%" />
                                    </asp:BoundField>                                    
                                    <asp:BoundField DataField="NAME" HeaderText="別名">
                                        <ItemStyle Width="40%" />
                                        <HeaderStyle Width="40%" />
                                    </asp:BoundField>                           
                                    <asp:TemplateField HeaderText="刪除">
                                        <itemstyle horizontalalign="Center" width="20%" />
                                        <headerstyle width="20%" />
                                        <itemtemplate>                                                                                    
                                            <cc1:CustCheckBox ID="delFlag" runat="server" Checked="false" />
                                        </itemtemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                            </cc1:CustGridView>
                            <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right"
                                InputBoxStyle="height:22px" >
                            </cc1:GridPager>
                        </td>
                    </tr>
                        <tr class="trOdd">
                            <td>
                                <%-- 新增別名 --%>
                                <div style="margin: 5px">
                                    <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01090300_007" StickHeight="False"></cc1:CustLabel>
                                </div>
                                <%--身分證字號--%>
                                <div style="margin: 5px"> 
                                    <cc1:CustLabel ID="newIdNum" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01090300_008" StickHeight="False"></cc1:CustLabel>
                            
                                    <cc1:CustTextBox ID="txtNewIdNum" runat="server" MaxLength="10" checktype="ID" Width="120px"
                                        onkeydown="entersubmit('btnAdd');" BoxName="身分證字號" InputType="LetterAndInt" ReadOnly="true" BackColor="LightGray"></cc1:CustTextBox>
                                </div>
                                <%--新別名--%>
                                <div style="margin: 5px"> 
                                    <cc1:CustLabel ID="chineseName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01090300_009" StickHeight="False" Style="margin-left:24px"></cc1:CustLabel>
                            
                                    <cc1:CustTextBox ID="txtChineseName" runat="server" MaxLength="20" Width="120px"
                                        onkeydown="entersubmit('btnAdd');" TabIndex="21" BoxName="新別名" onpaste="paste();"></cc1:CustTextBox>
                               </div> 
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo3" style="">
                        <tr class="itemTitle">
                            <td align="center">
                                <cc1:CustButton ID="btnAdd" runat="server" CssClass="smallButton" ShowID="01_01090300_010"
                                    OnClick="btnAdd_Click" DisabledWhenSubmit="False"
                                    onkeydown="movefocus();" />
                                <cc1:CustButton ID="btnCancel" runat="server" CssClass="smallButton" ShowID="01_01090300_011" OnClick="btnCancel_Click" DisabledWhenSubmit="false" />
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>                
    </form>
</body>
</html>
