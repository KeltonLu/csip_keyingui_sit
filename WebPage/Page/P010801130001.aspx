<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010801130001.aspx.cs" Inherits="P010801130001" enableSessionState="true" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <%-- 2020/11/23_Ares_Stanley-修正格式 --%>
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
//        var ControlDDL = document.getElementById(ddlId.id);
//        var TextBoxName = ddlId.id.replace('ddlUSER_STATUS', 'txtASSIGN_RATE');
//        var ControlTextBox = document.getElementById(TextBoxName);
//            
//         if(ControlDDL.value == "0") 
//         {
//             ControlTextBox.value = "0";  
//             ControlTextBox.disabled = true;      
//         }
//         else
//         {
//             ControlTextBox.disabled = false; 
//         }
      }
      
      function GridView_SelectAll()
      {
            var gv =  document.getElementById("GridView1");
            var GridViewObjList = gv.getElementsByTagName("input");
            
            for(var i = 0;i < GridViewObjList.length; i++)
            {
                if(GridViewObjList[i].type == 'checkbox')
                {
                    if(GridViewObjList[i].name.indexOf("chbSelect") > -1)
                    {
                        GridViewObjList[i].checked = true;
                    }
                }
            }
      }
      
      function GridView_ClearAll()
      {
            var gv =  document.getElementById("GridView1");
            var GridViewObjList = gv.getElementsByTagName("input");
            
            for(var i = 0;i < GridViewObjList.length; i++)
            {
                if(GridViewObjList[i].type == 'checkbox')
                {
                    if(GridViewObjList[i].name.indexOf("chbSelect") > -1)
                    {
                        GridViewObjList[i].checked = false;
                    }
                }
            }
      }
      
           
      function Check_GridView()
      {
            var gv =  document.getElementById("GridView1");
            var GridViewObjList = gv.getElementsByTagName("input");
            var iCount = 0;
            
            for(var i = 0;i < GridViewObjList.length; i++)
            {
                if(GridViewObjList[i].type == 'checkbox')
                {                
                    if(GridViewObjList[i].name.indexOf("chbSelect") > -1)
                    {
                        if(GridViewObjList[i].checked == true)
                        {
                            iCount = iCount + 1;
                        }
                    }
                }
            }
            
            if(iCount == 0)
            {
                alert('沒有勾選資料');
                return false;
            }
            else
            {
                var ControlDDL = document.getElementById('ddlNew_CaseOwner_User');
                if(ControlDDL.value == "請選擇") 
                {
                    alert('沒有選擇轉派對象');
                    return false;
                }
                else
                return true;
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
                        <td colspan="3">
                            <li>
                                <cc1:CustLabel ID="clbTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="01_01080113_013" ></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="width:40%">
                            <cc1:CustLabel ID="clbCASE_NO" runat="server" ShowID="01_01080113_001" ></cc1:CustLabel>
                            <asp:TextBox ID="txtCASE_NO" runat="server"></asp:TextBox>
                            &nbsp;&nbsp;&nbsp;
                            <cc1:CustLabel ID="clbHCOP_HEADQUATERS_CORP_NO" runat="server" ShowID="01_01080113_002" ></cc1:CustLabel>
                            <asp:TextBox ID="txtHCOP_HEADQUATERS_CORP_NO" runat="server"></asp:TextBox>
                        </td> 
                        <td style="width:30%">
                            <cc1:CustLabel ID="clbCaseOwner_User" runat="server" ShowID="01_01080113_003" style="display:inline;" ></cc1:CustLabel>   
                                <div style="position: absolute; display:inline; margin-top:2px;">
                                    <asp:TextBox ID="txtCaseOwner_User" runat="server" onfocus="allselect(this);"
                                        Style="position: absolute; top: -3px; width: 150px; height: 11px;"></asp:TextBox>
                                    <asp:DropDownList ID="ddlCaseOwner_User" runat="server" onclick="simOptionClick4IE('txtCaseOwner_User');"
                                        Style="position: absolute; top: -3px;clip: rect(0px auto auto 140px); width: 160px;"></asp:DropDownList>                             
                                </div>
                                
<%--                                <div style="position: absolute">
                                    <cc1:CustDropDownList ID="ddlCaseOwner_User" kind="select" runat="server" Style="left: 0px;
                                        top: 0px; clip: rect(0px auto auto 130px); position: absolute; width: 150px;"
                                        onclick="simOptionClick4IE('txtCaseOwner_User');">
                                    </cc1:CustDropDownList>
                                    <cc1:CustTextBox ID="txtCaseOwner_User" runat="server" MaxLength="30" checklength="3"
                                        oonfocus="allselect(this);" Style="left: 0px; top: 0px; position: absolute;
                                        width: 140px; height: 14px;" ></cc1:CustTextBox>
                                </div>--%>
                                
                                
<%--                                    <div style="position: absolute">
                                        <cc1:CustTextBox ID="txtProjYearBegin" runat="server" MaxLength="4" Width="50px" onfocus="allselect(this);"
                                             BoxName="開始年"
                                          Style="left: 0px; top: 0px; position: absolute; width: 50px; height: 14px"></cc1:CustTextBox>
                                        <cc1:CustDropDownList ID="dropProjYearBegin" kind="select" runat="server" onclick="simOptionClick4IE('txtProjYearBegin');"
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;">
                                        </cc1:CustDropDownList>
                                    </div>--%>
                                    
                        </td>                        
                        <td style="width:5%" align="left">
                            <cc1:CustButton ID="btnSearch" CssClass="smallButton" runat="server" Width="40px" DisabledWhenSubmit="False" ShowID="01_01080113_005" OnClick="btnSearch_Click"/>
                        </td>
                    </tr>
                </table>  
                <br />                
                <cc1:CustButton ID="btnSelectAll" CssClass="smallButton" runat="server" Width="60px" OnClientClick="return GridView_SelectAll();" DisabledWhenSubmit="False" ShowID="01_01080113_007" />   
                <cc1:CustButton ID="btnClearAll" CssClass="smallButton" runat="server" Width="60px" OnClientClick="return GridView_ClearAll();" DisabledWhenSubmit="False" ShowID="01_01080113_008" />   
                <asp:GridView ID="GridView1" runat="server" OnRowDataBound="GridView1_RowDataBound" Width="100%" AutoGenerateColumns="false">                    
                    <Columns>
                        <asp:TemplateField HeaderText="勾選" HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Center" >
                            <ItemTemplate>                                
                                <asp:CheckBox ID="chbSelect" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Show_NAME" DataField="tempRowNum" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center"/>
                        <asp:BoundField HeaderText="Show_NAME" DataField="CASE_NO" HeaderStyle-Width="20%" ItemStyle-HorizontalAlign="Center"/>
                        <asp:BoundField HeaderText="Show_NAME" DataField="HCOP_HEADQUATERS_CORP_NO" HeaderStyle-Width="20%" ItemStyle-HorizontalAlign="Center"/>
                        <asp:BoundField HeaderText="Show_NAME" DataField="HCOP_REG_NAME" HeaderStyle-Width="20%" ItemStyle-HorizontalAlign="Left"/>
                        <asp:BoundField HeaderText="Show_NAME" DataField="Show_NAME" HeaderStyle-Width="25%" ItemStyle-HorizontalAlign="Center"/>
                    </Columns>
                </asp:GridView>  
                <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right"
                    OnPageChanged="gpList_PageChanged">
                </cc1:GridPager>      
                <asp:HiddenField ID="hidRowCount" runat="server" />  
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo4" style="">
                        <tr class="itemTitle">
                            <td align="center">
                                <cc1:CustLabel ID="clbNew_CaseOwner_User" runat="server" ShowID="01_01080113_004" ></cc1:CustLabel>
                                <asp:DropDownList ID="ddlNew_CaseOwner_User" runat="server"></asp:DropDownList>
                                <cc1:CustButton ID="btnSubmit" CssClass="smallButton" runat="server" Width="40px" OnClientClick="return Check_GridView();" OnClick="btnSubmit_Click" DisabledWhenSubmit="False" ShowID="01_01080113_011" />    
                                <%--<cc1:CustButton ID="btnCancel" CssClass="smallButton" runat="server" Width="40px" OnClick="btnCancel_Click" DisabledWhenSubmit="False" ShowID="01_08011000_008" />  --%>                           
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
