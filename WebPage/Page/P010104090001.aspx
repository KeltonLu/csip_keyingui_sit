<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010104090001.aspx.cs" Inherits="P010104090001" %>

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
        
        //*客戶端檢核
        function checkInputText(id,intType)
        { 
              
               
                  if(document.getElementById('txtShopNo').value.Trim() == "")
                  {
                        alert('請輸入特店編號!');
                        document.getElementById('txtShopNo').focus();
                        return false;
                 }
       
                 if(intType == 2 )//*刪除
                { 
                     //*檢核欄位輸入規則      
                    if(!checkInputType(id))
                    {
                        return false;
                    }                    
                     if(!confirm('確定是否要刪除資料？'))
                    {
                         return false;
                    }


                }
                

                

                return true;
        }
        
      //*是否選中GridView中的資料 
      function isChecked()
      {
           var issel = false;
            var obj = document.getElementById('grvShow');
            if(obj != null)
            {
                var inputs = document.getElementById('grvShow').getElementsByTagName("input");
            
                for(var i=0; i<inputs.length; i++)
                {
                    if(inputs[i].type == "radio")
                    {
                        if(inputs[i].checked == true)
                        {
                            issel = true;
                            break;
                        }
                    }
                }
            }
            return issel;
      }  
        
         //*商店代號欄位輸入值改變
      function changeStatus()
      {
            if(document.getElementById('txtShopNo').value.Trim() != document.getElementById('txtShopNoHide').value.Trim())
            {
                var value = document.getElementById('txtShopNo').value.Trim();
                document.getElementById('btnSubmit').disabled = true;
                document.getElementById('txtShopNo').value = value;
                deleteRow();
                if(document.getElementById('gpList')!=null)
               {
                    //document.getElementById('gpList').disabled = true;
                    document.getElementById('gpList').style.display="none";
                }
            }
            
            document.getElementById('txtShopNoHide').value = document.getElementById('txtShopNo').value;
      }
      
      //*radio選擇事件
      function selected(object,id)
      {
            var inputs = document.getElementById(id).getElementsByTagName("input");
            var obj = document.getElementById(id);
             
            $("input[type='radio']").attr("checked","");
            $(object).attr("checked", "true");
            
            for(var i=0; i<inputs.length; i++)
            {
                if(inputs[i].type == "radio")
                {             
                    if(inputs[i].checked == true)
                    {

                        document.getElementById('btnSubmit').disabled = false;
                        
                        break;
                    }
                }
            } 
      }
      
      //*清空GridView
      function deleteRow()
      {
            var table = document.getElementById('grvShow');
            
            if(table != null && table.rows.length > 1)
            {             
                var count = table.rows.length;
     
                for(var i = count-1; i >0; i--)
                {

                      table.deleteRow(i);

                } 
            }          
      } 
      
            //*使單項選擇項全部不能選

      function DisableList()
      {
      
           var issel = false;
            var obj = document.getElementById('grvShow');
            if(obj != null)
            {
                var inputs = document.getElementById('grvShow').getElementsByTagName("input");
            
                for(var i=0; i<inputs.length; i++)
                {
                    if(inputs[i].type == "radio")
                    {
                       inputs[i].disabled = true;
                    }
                }
            }
           if(document.getElementById('gpList')!=null)
          {
              document.getElementById('gpList').disabled = true;
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
        <cust:image runat="server" ID="image1"/>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
               <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                        <tr class="itemTitle">
                            <td colspan="4">
                                <li>
                                    <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01040502_001"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="lblShopNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040502_002" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 85%" colspan="3">
                                <cc1:CustTextBox ID="txtShopNo" runat="server" MaxLength="9" checktype="numandletter"
                                    Width="90px" onkeydown="entersubmit('btnSelect');" onkeypress="keypress('btnSelect',false);"  onkeyup="changeStatus();"  BoxName="特店編號"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtShopNoHide" runat="server" MaxLength="10" CssClass="btnHiden"></cc1:CustTextBox>
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <cc1:CustButton ID="btnSelect" CssClass="smallButton" runat="server" Width="40px"
                                    OnClick="btnSelect_Click" OnClientClick="return checkInputText('tabNo1',0);"  onkeydown="setfocuschoice('txtShopNo','btnSubmit');"
                                    DisabledWhenSubmit="False"  ShowID="01_01040501_013"/>
                            </td>

                       </tr>
                      
                        </table>
                        <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo2" style="">
                      <tr class="itemTitle">
                        <td align="center" colspan="4">
                          
                        </td>
                    </tr>
                    </table>
                    
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo3" style="">
                    <tr>
                        <td >
                 <cc1:CustGridView ID="grvShow" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid" >
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:TemplateField>
                                        <itemtemplate>
                                            <input class="ChoiceButton"  id="radSelect" type="radio" runat="server" onclick="selected(this,'grvShow');"/> 
                                        
</itemtemplate>
                                       <itemstyle width="7%" cssclass="Grid_Choice" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="CARD_TYPE">
                                        <itemstyle width="14%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="AGENT_BANK_NMBR">
                                        <itemstyle width="14%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="DISCOUNT_RATE">
                                        <itemstyle width="14%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="AGENT_DESC">
                                        <itemstyle width="51%" />
                                    </asp:BoundField>
                                </Columns>
                            </cc1:CustGridView>
                         <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right"
                                OnPageChanged="gpList_PageChanged">
                            </cc1:GridPager>

                        </td>
                    </tr>
                </table>
                
                      <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table1" style="">
                      <tr class="itemTitle">
                        <td align="center" colspan="4">
                            <cc1:CustButton ID="btnSubmit" runat="server" CssClass="smallButton" ShowID="01_01040502_014"
                                OnClick="btnSubmit_Click" OnClientClick="return checkInputText('pnlText', 2);"
                                DisabledWhenSubmit="False" onkeydown="setfocus('txtShopNo');" />
                        </td>
                    </tr>
                    </table>
                    
                    
                    
                </cc1:CustPanel>

            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
