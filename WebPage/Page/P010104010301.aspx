<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010104010301.aspx.cs" Inherits="P010104010301" %>

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
              
               
                if(intType == 0)
                {
                     if(document.getElementById('txtShopNo').value.Trim() == "")
                    {
                        alert('請輸入特店編號資料!');
                        document.getElementById('txtShopNo').focus();
                        return false;
                    }
             
                    //*檢核欄位輸入規則
                    if(!checkInputType(id))
                    {
                        return false;
                    }
                                 
                    //*檢核CARD-TYPE欄位
                    var input = document.getElementById('txtCardType').value.toUpperCase();
                   
                    if(input != "J" && input != "M" && input != "N" && input != "V")
                    {
                        alert('CARD-TYPE只能輸入J,M,N,V');
                        return false;
                    }
                }
                            
                if(intType == 1)//*新增
                {       
                    //*顯示確認提示框                    var table = document.getElementById('grvShow');                                        if(table != null && table.rows.length > 1)                    {                                                 if(!confirm('確定是否要新增資料？'))
                             {
                                       return false;
                              }                    }                    else                    {                                                 alert('請鍵入資料後再新增. ');
                             return false;                                          }

                }
                
                 if(intType == 2)//*刪除
                {
                    if(!isChecked())
                    {
                        alert('請選擇要刪除的資料');
                        return false;
                    }
                    
                    //*顯示確認提示框
                    if(!confirm('確定是否要刪除？'))
                    {
                        return false;
                    }
                }
                
                if(intType == 3)//*查詢
                {
                    if(document.getElementById('txtShopNo').value.Trim() == "")
                    {
                        alert('請輸入特店編號資料!');
                        document.getElementById('txtShopNo').focus();
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
                clearControls('tabNo1');
                document.getElementById('txtShopNo').value = value;
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
                        document.getElementById('txtShopNo').value = obj.rows(i+1).cells(1).innerText;
                        document.getElementById('txtShopNo').focus();
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
                for(var i = 0; i <= count; i++)
                {
                    if(i != 0)
                    {
                        if(i > 1)
                        {
                            table.deleteRow(i-1);                      
                        }
                        else
                        {
                            table.deleteRow(i);
                        }
                        count--;
                    } 
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
                            <td colspan="6">
                                <li>
                                    <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01040103_001"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 17%">
                                <cc1:CustLabel ID="lblShopNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040103_002" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 16%">
                                <cc1:CustTextBox ID="txtShopNo" runat="server" MaxLength="9" checktype="numandletter"
                                    Width="90px" onkeyup="changeStatus();" onkeydown="return nosubmit();" BoxName="特店編號"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtShopNoHide" runat="server" MaxLength="10" CssClass="btnHiden"></cc1:CustTextBox>
                            </td>
                            <td align="right" style="width: 16%">
                                <cc1:CustLabel ID="lblCardType" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040103_003" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 18%">
                                <cc1:CustTextBox ID="txtCardType" runat="server" MaxLength="1" Width="50px" checktype="letter"
                                    onkeypress="entersubmit('btnSelectHiden');" BoxName="CARD-TYPE"></cc1:CustTextBox>
                                <cc1:CustLabel ID="lblMessage" runat="server" ShowID="01_01040103_004"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 16%">
                                <cc1:CustLabel ID="lblIdentifyNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040103_005" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%">
                                <cc1:CustTextBox ID="txtIdentifyNo" runat="server" MaxLength="5" Width="70px" checktype="numandletter"
                                    onkeydown="return nosubmit();" BoxName="認同代號"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 17%">
                                <cc1:CustLabel ID="lblFavourableFee" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040103_006"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 16%">
                                <cc1:CustTextBox ID="txtFavourableFee" runat="server" MaxLength="5" checktype="floatletter"
                                    Width="70px" onkeydown="return nosubmit();" BoxName="優惠費率"></cc1:CustTextBox>
                            </td>
                            <td align="right" style="width: 16%">
                                <cc1:CustLabel ID="lblBatchDepict" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040103_007" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 16%" colspan="3">
                                <cc1:CustTextBox ID="txtBatchDepict" runat="server" MaxLength="30" Width="280px"
                                    onkeypress="entersubmit('btnAddHiden');" BoxName="BATCH-描述"></cc1:CustTextBox>
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo2" style="">
                    <tr class="itemTitle">
                        <td colspan="6">
                            <cc1:CustLabel ID="lblTitle2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01040103_008"></cc1:CustLabel>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <cc1:CustGridView ID="grvShow" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid">
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:TemplateField>
                                        <itemstyle width="7%" cssclass="Grid_Choice" />
                                        <itemtemplate>
                                            <input class="ChoiceButton"  id="radSelect" type="radio" runat="server" onclick="selected(this,'grvShow');"/> 
                                        
</itemtemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="shop_no">
                                        <itemstyle width="12%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="card_type">
                                        <itemstyle width="12%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="identify_no">
                                        <itemstyle width="12%" horizontalalign="Center" />
                                        <headerstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="favour_fee">
                                        <itemstyle width="12%" horizontalalign="Center" />
                                        <headerstyle horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="batch_depict">
                                        <itemstyle width="45%" />
                                    </asp:BoundField>
                                </Columns>
                            </cc1:CustGridView>
                            <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right"
                                OnPageChanged="gpList_PageChanged">
                            </cc1:GridPager>
                        </td>
                    </tr>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo3" style="">
                    <tr class="itemTitle">
                        <td align="center">
                            <cc1:CustButton ID="btnAdd" runat="server" CssClass="smallButton" ShowID="01_01040103_011"
                                OnClick="btnAdd_Click" OnClientClick="return checkInputText('pnlText', 1);" DisabledWhenSubmit="False" />&nbsp;&nbsp
                            <cc1:CustButton ID="btnDelete" runat="server" CssClass="smallButton" ShowID="01_01040103_012"
                                OnClick="btnDelete_Click" OnClientClick="return checkInputText('pnlText', 2);"
                                DisabledWhenSubmit="False" onkeydown="setfocus('txtShopNo');" />
                            <cc1:CustButton ID="btnSelectHiden" runat="server" CssClass="btnHiden" DisabledWhenSubmit="False"
                                OnClick="btnSelectHiden_Click" OnClientClick="return checkInputText('pnlText', 3);" />
                            <cc1:CustButton ID="btnAddHiden" runat="server" CssClass="btnHiden" OnClientClick="return checkInputText('pnlText', 0);"
                                OnClick="btnAddHiden_Click" DisabledWhenSubmit="False" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
