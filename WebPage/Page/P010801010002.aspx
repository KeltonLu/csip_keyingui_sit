<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010801010002.aspx.cs" Inherits="Page_P010801010002" %>

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
        function checkInputData() {
            if (document.getElementById('txtNoteLog_NL_Value').value == "") {
                alert('聯絡內容不可空白');
                return false;
            }
            else if ((document.getElementById('txtNoteLog_NL_Value').value.length > 500)) {
                alert('內容長度過長');
                return false;
            }
            else {
                return true;
            }
        }
        function windowClose() {
            this.close();
        }

    </script>

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
        <%--<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">--%>
        <contenttemplate>
            
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="CaseInfo" style="">
                    <tr class="trOdd">
                        <td align="right" style="width: 10%">
                            <%--案件編號--%>
                            <cc1:CustLabel ID="clblbAML_HQ_Work_CASE_NO" runat="server" CurAlign="right" ShowID="01_08011000_002" ></cc1:CustLabel>
                        </td>
                        
                        <td style="width: 20%">    
                            <asp:Label ID="lbAML_HQ_Work_CASE_NO" runat="server" Text=""></asp:Label>
                            <asp:HiddenField ID="hidAML_HQ_Work_CASE_NO" runat="server" />
                        </td>                        
                        <td align="right" style="width: 10%">                            
                            <%--客戶ID(統編)--%>
                            <cc1:CustLabel ID="clbAML_HQ_Work_HCOP_HEADQUATERS_CORP_NO" runat="server" CurAlign="right" ShowID="01_08011000_003" ></cc1:CustLabel>
                        </td>
                        <td style="width: 20%">                                
                            <asp:Label ID="lbAML_HQ_Work_HCOP_HEADQUATERS_CORP_NO" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="right" style="width: 10%">                            
                            <%--登記名稱--%>
                            <cc1:CustLabel ID="clbAML_HQ_Work_HCOP_REG_NAME" runat="server" CurAlign="right" ShowID="01_08011000_004" ></cc1:CustLabel>
                        </td>
                        <td style="width: 20%">                        
                            <asp:Label ID="lbAML_HQ_Work_HCOP_REG_NAME" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <%-- 風險等級 --%>
                        <td align="right" style="width: 10%">
                            <cc1:CustLabel ID="riskLevel" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_08011000_010" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 20%">
                            <asp:Label ID="lbCalculatingRiskLevel" runat="server" Text=""></asp:Label>                                     
                        </td>
                         <td style="width: 10%"></td>
                         <td style="width: 10%"></td>
                         <td style="width: 10%"></td>
                         <td style="width: 10%"></td>
                    </tr>
                </table>
                
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                    <tr class="itemTitle">
                        <td colspan="2">
                              
                                <%--異常結案 類別--%>
                                <cc1:CustLabel ID="clbTitle" runat="server" CurAlign="right" ShowID="01_08011000_009" ></cc1:CustLabel>
                                <asp:RadioButton ID="rblAbnormal_I" runat="server" GroupName="Abnormal" Text="不合作結案"></asp:RadioButton>
                                <asp:RadioButton ID="rblAbnormal_C" runat="server" GroupName="Abnormal" Text="商店解約結案"></asp:RadioButton>
                                <asp:RadioButton ID="rblAbnormal_O" runat="server" GroupName="Abnormal" Text="其他"></asp:RadioButton>
                            
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="left" style="width: 20%">
                            <%--聯絡內容&#010;(必填)--%>
                                <cc1:CustLabel ID="clbNoteLog_NL_Value" runat="server" CurAlign="right" ShowID="01_08011000_006" ></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 80%">
                             <%-- 20220620 調整聯絡內容可以換行 By Kelton --%>
                            <%--<asp:TextBox ID="txtNoteLog_NL_Value" runat="server" MaxLength="500" Height="180px" Width="660px"></asp:TextBox>--%>
                            <asp:TextBox ID="txtNoteLog_NL_Value" runat="server" MaxLength="500" Height="180px" Width="660px" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo4" style="">
                        <tr class="itemTitle">
                            <td align="center">
                                <%--返回--%>
                                <cc1:CustButton ID="btnCancel" CssClass="smallButton" runat="server" Width="40px" OnClick="btnCancel_Click" DisabledWhenSubmit="False" ShowID="01_01080101_073" />
                                <%--申請送審--%>
                                <cc1:CustButton ID="btnSubmit" CssClass="smallButton" runat="server" Width="70px" OnClientClick="return checkInputData();" OnClick="btnSubmit_Click" DisabledWhenSubmit="False" ShowID="01_01080101_074" />    
                                
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
            </contenttemplate>
        <%--</asp:UpdatePanel>--%>
    </form>
</body>
</html>