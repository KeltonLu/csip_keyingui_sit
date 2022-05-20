<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010104050101.aspx.cs" Inherits="P010104050101" %>

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

                            
                if(intType == 1 )//*新增
                { 
                     //*檢核欄位輸入規則      
                    if(!checkInputType(id))
                    {
                        return false;
                    }                    //*檢核CARD-TYPE欄位
                    var input = document.getElementById('txtCardType').value.toUpperCase();
                   
                    if(input != "J" && input != "M" && input != "N" && input != "V")
                    {
                        alert('CARD-TYPE只能輸入J,M,N,V');
                        return false;
                    }                                         if(!confirm('確定是否要新增資料？'))
                    {
                         return false;
                    }

                }
                
                
                    if(intType == 2 )//*修改
                { 
                     //*檢核欄位輸入規則      
                    if(!checkInputType(id))
                    {
                        return false;
                    }                    //*檢核CARD-TYPE欄位
                    var input = document.getElementById('txtCardType').value.toUpperCase();
                   
                    if(input != "J" && input != "M" && input != "N" && input != "V")
                    {
                        alert('CARD-TYPE只能輸入J,M,N,V');
                        return false;
                    }                                         if(!confirm('確定是否要修改資料？'))
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
                document.getElementById('txtHiden').value="1";
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

                        document.getElementById('txtCardType').value = obj.rows(i+1).cells(1).innerText;
                        document.getElementById('txtIdentifyNo').value = obj.rows(i+1).cells(2).innerText;
                        document.getElementById('txtFavourableFee').value = obj.rows(i+1).cells(3).innerText;
                        document.getElementById('txtBatchDepict').value = obj.rows(i+1).cells(4).innerText;
                        document.getElementById('txtBatchDepict').value = obj.rows(i+1).cells(4).innerText;
                        document.getElementById('btnSubmit').disabled = false;
                        
                        break;
                    }
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
                                        SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01040501_001"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="lblShopNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040501_002" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 85%" colspan="3">
                                <cc1:CustTextBox ID="txtShopNo" runat="server" MaxLength="9" checktype="numandletter"
                                    Width="90px"  onkeydown="entersubmit('btnSelect');" onkeypress="keypress('btnSelect',false);"  onkeyup="changeStatus();"  BoxName="特店編號"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtShopNoHide" runat="server" MaxLength="10" CssClass="btnHiden"></cc1:CustTextBox>
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <cc1:CustButton ID="btnSelect" CssClass="smallButton" runat="server" Width="40px"
                                    OnClick="btnSelect_Click" OnClientClick="return checkInputText('tabNo1',0);"
                                    DisabledWhenSubmit="False"  ShowID="01_01040501_013"/>
                                <cc1:CustTextBox ID="txtHiden" runat="server" MaxLength="100" CssClass="btnHiden"></cc1:CustTextBox>
                            </td>

                       </tr>
                      <tr class="trEven"> 
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="lblCardType" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040501_003" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtCardType" runat="server" MaxLength="1" Width="50px" checktype="letter"
                                    onkeydown="return nosubmit();"  BoxName="CARD-TYPE"></cc1:CustTextBox>
                                <cc1:CustLabel ID="lblMessage" runat="server" ShowID="01_01040501_004"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="lblIdentifyNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040501_005" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtIdentifyNo" runat="server" MaxLength="5" Width="70px" checktype="numandletter"
                                    onkeydown="return nosubmit();" BoxName="認同代號"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="lblFavourableFee" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040501_006"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtFavourableFee" runat="server" MaxLength="5" checktype="num"
                                    Width="70px" onkeydown="return nosubmit();" BoxName="優惠費率"></cc1:CustTextBox>
                            </td>
                            <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="lblBatchDepict" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040501_007" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 35%" >
                                <cc1:CustTextBox ID="txtBatchDepict" runat="server" MaxLength="30" Width="280px"
                                    onkeydown="return nosubmit();"  BoxName="BATCH-描述"></cc1:CustTextBox>
                            </td>
                        </tr>
                        </table>
                        <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo2" style="">
                      <tr class="itemTitle">
                        <td align="center" colspan="4">
                            <cc1:CustButton ID="btnAdd" runat="server" CssClass="smallButton" ShowID="01_01040501_011"
                                OnClick="btnAdd_Click" OnClientClick="return checkInputText('pnlText', 1);" DisabledWhenSubmit="False"  onkeydown="setfocuschoice('txtShopNo','btnSubmit');"/>&nbsp;&nbsp
                            <cc1:CustButton ID="btnSubmit" runat="server" CssClass="smallButton" ShowID="01_01040501_014"
                                OnClick="btnSubmit_Click" OnClientClick="return checkInputText('pnlText', 2);"
                                DisabledWhenSubmit="False" onkeydown="setfocus('txtShopNo');" />
                        </td>
                    </tr>
                    </table>
                    
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo3" style="">
                    <tr>
                        <td >
                 <cc1:CustGridView ID="grvShow" runat="server" AllowSorting="True" 
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid" AllowPaging="False" >
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
                                       <itemstyle width="5%" cssclass="Grid_Choice" /> 
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
                                        <itemstyle width="53%" />
                                    </asp:BoundField>
                                </Columns>
                            </cc1:CustGridView>


                        </td>
                    </tr>
                </table>
                    
                    
                    
                </cc1:CustPanel>

            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
