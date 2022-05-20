<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010402010001.aspx.cs" Inherits="P010402010001" %>

<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script language="javascript" type="text/javascript">
        function checkInputText(id,intType)
        {
            //*檢核輸入欄位【收件編號】是否為空 
            if(document.getElementById('txtReceiveNumber').value.Trim() == "")
            {
                document.getElementById('txtReceiveNumber').focus();
                alert('請輸入收件編號');

                return false;
            }
            
            //*檢核輸入欄位【收件編號】前兩碼需為 RK
            if (document.getElementById("txtReceiveNumber").value.Trim().substring(0,2).toUpperCase() != "RU")
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
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <cust:image runat="server" ID="image1" />
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <Triggers>
                <asp:PostBackTrigger ControlID="btnShowData" />
            </Triggers>
                <ContentTemplate>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                        <tr class="itemTitle">
                            <td colspan="2">
                                <li>
                                    <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" Width="200px" IsColon="False" ShowID="01_01050201_001"></cc1:CustLabel>
                                </li>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblReceiveNumber" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01010500_002"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 85%">
                                <cc1:CustTextBox ID="txtReceiveNumber" runat="server" MaxLength="12" checktype="numandletter"></cc1:CustTextBox>
                                <cc1:CustButton ID="btnSelect" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                    Width="40px" OnClick="btnSelect_Click" OnClientClick="return checkInputText('tabNo1',0);"
                                    onkeydown="setfocuschoice('txtReceiveNumber','cFUpload');" ShowID="01_01050201_002" />
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table1" style="">
                        <tr class="itemTitle">
                            <td align="right">
                                <cc1:CustLabel ID="lbUploadPath" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01050201_004" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td>
                                <cc1:CustFileUpload ID="cFUpload" runat="server"  unselectable="on"/></td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo8" style="">
                        <tr class="itemTitle">
                            <td align="center">
                                <cc1:CustButton ID="btnShowData" runat="server" CssClass="smallButton" ShowID="01_01050201_002"
                                    DisabledWhenSubmit="False" TabIndex="59" Width="40px" OnClick="btnShowData_Click"/>
                            </td>
                        </tr>
                    </table>
                    <cc1:CustPanel ID="pnList" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table2" style="">
                        <tr>
                            <td>
                                <cc1:CustGridView ID="gvList"  Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
                                    BorderStyle="Solid" AllowPaging="false" runat="server" OnRowDataBound="gvList_RowDataBound">
                                    <PagerSettings Visible="False" />
                                    <RowStyle CssClass="Grid_Item" />
                                    <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                    <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                    <AlternatingRowStyle CssClass="Grid_AlternatingItem" />
                                    <Columns>
                                        <asp:BoundField DataField="DATA_TYPE">
                                            <headerstyle width="50%" />
                                            <itemstyle horizontalalign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="UPLOAD_DATA">
                                            <itemstyle horizontalalign="Center" />
                                        </asp:BoundField>
                                    </Columns>
                                </cc1:CustGridView>
                                
                            </td>
                        </tr>
                        <tr class="itemTitle">
                            <td align="center">
                                <cc1:CustButton ID="btnSubmit" runat="server" CssClass="smallButton" ShowID="01_01050201_003"
                                    DisabledWhenSubmit="False" TabIndex="59" Width="40px" OnClick="btnSubmit_Click" />
                            </td>
                        </tr>
                    </table>
                    </cc1:CustPanel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <br />
        </div>
    </form>
</body>
</html>
