<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010801120001.aspx.cs" Inherits="P010801120001" enableSessionState="true" %>

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
      function ChangeTextBox(ddlId)
      {
        var SearchDDL = document.getElementById('ddlSearch_USER_STATUS');
        var ControlDDL = document.getElementById(ddlId.id);
        var TextBoxName = ddlId.id.replace('ddlUSER_STATUS', 'txtASSIGN_RATE');
        var ControlTextBox = document.getElementById(TextBoxName);
        
            
         if(ControlDDL.value == "0") 
         {
             ControlTextBox.value = "0";  
             ControlTextBox.disabled = true;      
         }
         else
         {
            if(SearchDDL == "1")
                ControlTextBox.disabled = false; 
         }
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
        
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>        
        
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="CaseInfo" style="">
                    <tr class="itemTitle">
                        <td>
                            <li>
                                <cc1:CustLabel ID="clbTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="01_01080112_014" ></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="center">
                            <cc1:CustLabel ID="clbSearch_USER_STATUS" runat="server" ShowID="01_01080112_009" ></cc1:CustLabel>
                            <asp:DropDownList ID="ddlSearch_USER_STATUS" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSearch_USER_STATUS_SelectedIndexChanged">
                                <asp:ListItem Text="正常" Value="1"></asp:ListItem>
                                <asp:ListItem Text="失效" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                            <cc1:CustButton ID="btnSearch" CssClass="smallButton" runat="server" Width="40px" DisabledWhenSubmit="False" ShowID="01_01080112_008" OnClick="btnSearch_Click" Visible="false" />   
                        </td> 
                    </tr>   
                </table>           
                <asp:GridView ID="GridView1" runat="server" OnRowDataBound="GridView1_RowDataBound" Width="100%">                    
                    <Columns>
                        <asp:TemplateField HeaderText="NO"> 
                            <ItemTemplate> 
                                <%#Container.DataItemIndex+1 %> 
                            </ItemTemplate> 
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Show_NAME" DataField="Show_NAME" HeaderStyle-Width="30%" />
                        <asp:TemplateField HeaderText="ASSIGN_RATE" HeaderStyle-Width="30%">
                            <ItemTemplate>
                                <asp:TextBox ID="txtASSIGN_RATE" runat="server" MaxLength="3"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Sequence" HeaderStyle-Width="30%">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlUSER_STATUS" runat="server" onchange="ChangeTextBox(this);" >
                                    <asp:ListItem Text="正常" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="失效" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>        
                <asp:HiddenField ID="hidRowCount" runat="server" />  
                <asp:HiddenField ID="hidUSER_STATUS" runat="server" />  
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo4" style="">
                        <tr class="itemTitle">
                            <td colspan="4" align="center">
                                <cc1:CustButton ID="btnSubmit" CssClass="smallButton" runat="server" Width="40px" OnClientClick="return checkInputData();" OnClick="btnSubmit_Click" DisabledWhenSubmit="False" ShowID="01_01080112_011" />    
                                <%--<cc1:CustButton ID="btnCancel" CssClass="smallButton" runat="server" Width="40px" OnClick="btnCancel_Click" DisabledWhenSubmit="False" ShowID="01_08011000_008" /> --%>                            
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
