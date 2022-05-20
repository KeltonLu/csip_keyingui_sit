<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010801040002.aspx.cs" Inherits="Page_P010801040002" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%--2021/01/27_Ares_Stanley-調整版面; 2021/03/10_Ares_Stanley-出生日期欄寬; 20210412_Ares_Stanley版面調整--%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript"> </script>
    <style type="text/css">
        .btnHiden {
            display: none;
        }
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
                        <td colspan="2">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01080104_001"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                </table>
                <br />
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                    <tr class="trOdd">
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblCardNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080104_002" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 12%">
                            <cc1:CustLabel ID="hlblCaseNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="CustLabel28" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080104_003" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 12%">
                            <cc1:CustLabel ID="hlblHCOP_HEADQUATERS_CORP_NO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="CustLabel29" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01080104_005" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 12%">
                            <cc1:CustLabel ID="hlblHCOP_REG_NAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                        </td>
                    </tr>
                </table>
                  <br />
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <cc1:CustGridView ID="grvBRCHData" runat="server" BorderStyle="Solid" EnableModelValidation="True" PagerID="GridPager1" Width="100%" DataKeyNames="ID" 
                        OnRowDataBound="grvBRCHData_RowDataBound" OnRowCommand="grvBRCHData_RowCommand" Class="longTableGridView" >
                                    <AlternatingRowStyle CssClass="Grid_AlternatingItem" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="編輯">
                                            <EditItemTemplate>
                                                <asp:LinkButton ID="gvdUpdate" runat="server" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="Update2">更新</asp:LinkButton>
                                                                                                &nbsp;<asp:LinkButton ID="gvdCancel" runat="server" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="Cancel2">取消</asp:LinkButton>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="gvdEdit" runat="server" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CommandName="Edit2">編輯</asp:LinkButton>
                                            </ItemTemplate>
                                            <ItemStyle  HorizontalAlign="Center"  Width="60px" CssClass="whiteSpaceNormal" />
                                            <HeaderStyle CssClass="whiteSpaceNormal" Width="6%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="姓名(中)">
                                            <ItemTemplate>
                                                <cc1:CustLabel ID="lblBRCH_CHINESE_NAME" runat="server" CurAlign="left" Text='<%# Bind("BRCH_CHINESE_NAME") %>'></cc1:CustLabel>
                                            </ItemTemplate>
                                            <ItemStyle  HorizontalAlign="Center"  Width="100px" CssClass="whiteSpaceNormal" />
                                            <HeaderStyle CssClass="whiteSpaceNormal" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="姓名(英)">
                                            <ItemTemplate>
                                                <cc1:CustLabel ID="lblBRCH_ENGLISH_NAME" runat="server" CurAlign="left" Text='<%# Bind("BRCH_ENGLISH_NAME") %>'></cc1:CustLabel>
                                            </ItemTemplate>
                                            <ItemStyle  HorizontalAlign="Center"  Width="120px" CssClass="whiteSpaceNormal" />
                                            <HeaderStyle CssClass="whiteSpaceNormal" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="出生日期">
                                            <EditItemTemplate>
                                                <cc1:CustTextBox ID="txtBRCH_BIRTH_DATE" runat="server" BoxName="負責人生日" checktype="num" MaxLength="8" Text='<%# Bind("BRCH_BIRTH_DATE") %>' Width="65px"></cc1:CustTextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <cc1:CustLabel ID="lblBRCH_BIRTH_DATE" runat="server" CurAlign="left" Text='<%# Bind("BRCH_BIRTH_DATE") %>' Width="70px"></cc1:CustLabel>
                                            </ItemTemplate>
                                            <ItemStyle  HorizontalAlign="Center"  CssClass="whiteSpaceNormal" />
                                            <HeaderStyle CssClass="whiteSpaceNormal" Width="75px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="身分證號碼">
                                            <ItemTemplate>
                                                <cc1:CustLabel ID="lblBRCH_ID" runat="server" CurAlign="left" Text='<%# Bind("BRCH_ID") %>' ></cc1:CustLabel>
                                            </ItemTemplate>
                                            <ItemStyle  HorizontalAlign="Center"  CssClass="whiteSpaceNormal" />
                                            <HeaderStyle CssClass="whiteSpaceNormal" Width="9%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="身分證<br>發證日期">
                                            <ItemTemplate>
                                                <cc1:CustLabel ID="lblBRCH_OWNER_ID_ISSUE_DATE" runat="server" CurAlign="left" Text='<%# Bind("BRCH_OWNER_ID_ISSUE_DATE") %>' Width="60px"></cc1:CustLabel>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <cc1:CustTextBox ID="txtBRCH_OWNER_ID_ISSUE_DATE" runat="server" MaxLength="8" checktype="num" Width="65px" BoxName="發證日期"  Text='<%# Bind("BRCH_OWNER_ID_ISSUE_DATE") %>'></cc1:CustTextBox>
                                                </EditItemTemplate>
                                            <ItemStyle  HorizontalAlign="Center"  Width="80px" CssClass="whiteSpaceNormal"/>
                                            <HeaderStyle CssClass="whiteSpaceNormal" Width="7%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="發證地點">
                                            <ItemTemplate>
                                                <cc1:CustLabel ID="lblBRCH_OWNER_ID_ISSUE_PLACE" runat="server" CurAlign="left" Text='<%# Bind("BRCH_OWNER_ID_ISSUE_PLACE") %>' Width="40px"></cc1:CustLabel>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <cc1:CustTextBox ID="txtBRCH_OWNER_ID_ISSUE_PLACE" runat="server" MaxLength="10" checktype="num"  Width="40px" BoxName="發證地點" Text='<%# Bind("BRCH_OWNER_ID_ISSUE_PLACE") %>'></cc1:CustTextBox>
                                            </EditItemTemplate>
                                            <ItemStyle  HorizontalAlign="Center"  CssClass="whiteSpaceNormal" />
                                            <HeaderStyle CssClass="whiteSpaceNormal" Width="55px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="領補換<br>類別">
                                            <ItemTemplate>
                                                <cc1:CustLabel ID="lblBRCH_OWNER_ID_REPLACE_TYPE" runat="server" CurAlign="left" Text='<%# Bind("BRCH_OWNER_ID_REPLACE_TYPE") %>' Width="40px"></cc1:CustLabel>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <cc1:CustDropDownList ID="ddlBRCH_OWNER_ID_REPLACE_TYPE" kind="select" runat="server" Style=" width: 50px;">
                                                    <asp:ListItem Text="請選擇" Value=""></asp:ListItem>
                                                    <asp:ListItem Text="初" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="補" Value="2"></asp:ListItem>
                                                    <asp:ListItem Text="換" Value="3"></asp:ListItem>
                                                </cc1:CustDropDownList>
                                                <cc1:CustLabel ID="lblBRCH_OWNER_ID_REPLACE_TYPEE" runat="server" CurAlign="left" Text='<%# Bind("BRCH_OWNER_ID_REPLACE_TYPE") %>' Width="40px" Visible="false"></cc1:CustLabel>
                                            </EditItemTemplate>
                                            <ItemStyle  HorizontalAlign="Center"  Width="50px" CssClass="whiteSpaceNormal" />
                                            <HeaderStyle CssClass="whiteSpaceNormal" Width="60px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="有無照片">
                                            <ItemTemplate>
                                                <cc1:CustLabel ID="lblBRCH_ID_PHOTO_FLAG" runat="server" CurAlign="left" Text='<%# Bind("BRCH_ID_PHOTO_FLAG") %>' Width="40px"></cc1:CustLabel>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <cc1:CustRadioButton ID="radHasPhoto" runat="server" AutoPostBack="False" GroupName="Photo" Text="有" /><br />
                                                <cc1:CustRadioButton ID="radNoPhoto" runat="server" AutoPostBack="False" GroupName="Photo" Text="無" />
                                                <cc1:CustLabel ID="lblHasPHOTO" runat="server" CurAlign="left" Text='<%# Bind("BRCH_ID_PHOTO_FLAG") %>' Width="40px" Visible="false"></cc1:CustLabel>
                                            </EditItemTemplate>
                                            <ItemStyle  HorizontalAlign="Center"  CssClass="whiteSpaceNormal" />
                                            <HeaderStyle CssClass="whiteSpaceNormal" Width="55px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="國籍<br>(必填)">
                                            <ItemTemplate>
                                                <cc1:CustLabel ID="lblBRCH_NATION" runat="server" CurAlign="left" Text='<%# Bind("BRCH_NATION") %>' Width="20px"></cc1:CustLabel>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <cc1:CustTextBox ID="txtBRCH_NATION" runat="server" MaxLength="2" checktype="num" Width="20px" BoxName="國籍" AutoPostBack="true" Text='<%# Bind("BRCH_NATION") %>' OnTextChanged="txtBRCH_NATION_TextChanged"></cc1:CustTextBox>
                                            </EditItemTemplate>
                                            <ItemStyle  HorizontalAlign="Center"  Width="50px" CssClass="whiteSpaceNormal" />
                                            <HeaderStyle CssClass="whiteSpaceNormal" Width="45px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="護照號碼">
                                            <ItemTemplate>
                                                <cc1:CustLabel ID="lblBRCH_PASSPORT" runat="server" CurAlign="left" Text='<%# Bind("BRCH_PASSPORT") %>'></cc1:CustLabel>
                                            </ItemTemplate>
                                            <ItemStyle  HorizontalAlign="Center"  CssClass="whiteSpaceNormal" />
                                            <HeaderStyle CssClass="whiteSpaceNormal" Width="7%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="護照效期">
                                            <ItemTemplate>
                                                <cc1:CustLabel ID="lblBRCH_PASSPORT_EXP_DATE" runat="server" CurAlign="left" Text='<%# Bind("BRCH_PASSPORT_EXP_DATE") %>' Width="70px"></cc1:CustLabel>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <cc1:CustTextBox ID="txtBRCH_PASSPORT_EXP_DATE" runat="server" MaxLength="8" checktype="num" Width="70px" BoxName="護照效期" Text='<%# Bind("BRCH_PASSPORT_EXP_DATE") %>'></cc1:CustTextBox>
                                            </EditItemTemplate>
                                            <ItemStyle  HorizontalAlign="Center"  CssClass="whiteSpaceNormal" />
                                            <HeaderStyle CssClass="whiteSpaceNormal" Width="80px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="統一證號"><%--20200410-RQ-2019-030155-005-居留證號更名為統一證號--%>
                                            <ItemTemplate>
                                                <cc1:CustLabel ID="lblBRCH_RESIDENT_NO" runat="server" CurAlign="left" Text='<%# Bind("BRCH_RESIDENT_NO") %>'></cc1:CustLabel>
                                            </ItemTemplate>
                                            <ItemStyle  HorizontalAlign="Center"  CssClass="whiteSpaceNormal" />
                                            <HeaderStyle CssClass="whiteSpaceNormal" Width="8%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="統一證號效期"><%--20200410-RQ-2019-030155-005-居留證號更名為統一證號--%>
                                            <ItemTemplate>
                                                <cc1:CustLabel ID="lblBRCH_RESIDENT_EXP_DATE" runat="server" CurAlign="left" Text='<%# Bind("BRCH_RESIDENT_EXP_DATE") %>' Width="70px"></cc1:CustLabel>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <cc1:CustTextBox ID="txtBRCH_RESIDENT_EXP_DATE" runat="server" MaxLength="8" checktype="num" Width="70px" BoxName="居留證效期" Text='<%# Bind("BRCH_RESIDENT_EXP_DATE") %>'></cc1:CustTextBox>
                                            </EditItemTemplate>
                                            <ItemStyle  HorizontalAlign="Center"  CssClass="whiteSpaceNormal" />
                                            <HeaderStyle CssClass="whiteSpaceNormal" Width="80px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="分公司/<br>分店統一編號">
                                            <ItemTemplate>
                                                <cc1:CustLabel ID="lblBRCH_BRCH_NO" runat="server" CurAlign="left" Text='<%# Bind("BRCH_BRCH_NO") %>'></cc1:CustLabel>
                                            </ItemTemplate>
                                            <ItemStyle  HorizontalAlign="Center"  CssClass="whiteSpaceNormal" />
                                            <HeaderStyle CssClass="whiteSpaceNormal" Width="8%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="ID換補領<br>查詢結果">
                                            <ItemTemplate>
                                                <cc1:CustLabel ID="lblBRCH_ID_SreachStatus" runat="server" CurAlign="left" Text='<%# Bind("BRCH_ID_SreachStatus") %>'></cc1:CustLabel>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <cc1:CustCheckBox ID="chkSreachStatusY" runat="server"   AutoPostBack="true"    BoxName="正常" Text="正常" OnCheckedChanged="chkSreachStatusY_CheckedChanged"/>
                                   <br /><cc1:CustCheckBox ID="chkSreachStatusN" runat="server"   AutoPostBack="true"    BoxName="不適用" Text="不適用" OnCheckedChanged="chkSreachStatusN_CheckedChanged"/>
                                                <cc1:CustLabel ID="lblBRCH_ID_SreachStatusE" runat="server" CurAlign="left" Text='<%# Bind("BRCH_ID_SreachStatus") %>' Width="70px" Visible="false"></cc1:CustLabel>
                                            </EditItemTemplate>
                                            <ItemStyle  HorizontalAlign="Left"  CssClass="whiteSpaceNormal" />
                                            <HeaderStyle CssClass="whiteSpaceNormal" Width="60px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="ID" Visible="false">
                                            <ItemTemplate>
                                                <cc1:CustLabel ID="lblID" runat="server" CurAlign="left" Text='<%# Bind("ID") %>' Width="60px"></cc1:CustLabel>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                    <PagerSettings Visible="False" />
                                    <RowStyle CssClass="Grid_Item" />
                                    <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                </cc1:CustGridView>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo3" style="">
                        <tr class="itemTitle">
                            <td align="center">
                                <cc1:CustButton ID="btnAdd" runat="server" CssClass="smallButton" ShowID="01_01080105_002"
                                    OnClick="btnAdd_Click" OnClientClick="return checkInputText('pnlText', 1);" DisabledWhenSubmit="False"
                                    onkeydown="movefocus();" />
                                &nbsp;     &nbsp;     &nbsp;     &nbsp;
                                <cc1:CustButton ID="btnCancel" runat="server" CssClass="smallButton" ShowID="01_01080105_003"
                                    DisabledWhenSubmit="False"
                                    onkeydown="movefocus();" OnClick="btnCancel_Click" />
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
