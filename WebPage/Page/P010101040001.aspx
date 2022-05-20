<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010101040001.aspx.cs" Inherits="P010101040001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%--20210329_Ares_Stanley-調整半形轉全形失效; 20210408_Ares_Stanley-調整半形轉全形失效; 20210415_Ares_Stanley-調整全半形轉換--%>
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
            alert('★異動前請確認基本資料是否為本人！');
            //*檢核輸入欄位身份證號碼是否為空

             if(document.getElementById('txtUserId').value.Trim() == "")
            {
                alert('請輸入身份證號碼後,點選查詢按鈕');
                setControlsDisabled('pnlText');
                document.getElementById('txtUserId').focus();
                return false;
            }
            
             if(!checkInputType(id))
            {
                return false;
            } 
                     
            if(intType == 1)
            {
               //*檢核查詢部分欄位輸入規則
               if(!checkInputType('tabNo1'))
               {
                    return false;
               }

               //*顯示確認提示框

                if(!confirm('確定是否要異動資料？'))
                {
                    return false;
                }            
            }
            return true;
       }
      
      //*身份證號碼欄位輸入值改變

      function changeStatus(id)
      {
            if(document.getElementById('txtUserId').value.Trim() != document.getElementById('txtUserIdHide').value.Trim())
            {
                setControlsDisabled('pnlText');
            }
            document.getElementById('txtUserIdHide').value = document.getElementById('txtUserId').value;
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
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                    <tr class="itemTitle">
                        <td colspan="4">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010400_001" StickHeight="False"
                                    Width="240px"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="width: 15%" align="right">
                            <cc1:CustLabel ID="lblUserId" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01010400_002" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtUserId" runat="server" MaxLength="12" checktype="ID" onkeydown="entersubmit('btnSelect');"
                                onkeyup="changeStatus('txtUserId');" Width="156px" BoxName="身分證號碼"></cc1:CustTextBox>
                            <cc1:CustTextBox ID="txtUserIdHide" runat="server" MaxLength="10" CssClass="btnHiden"></cc1:CustTextBox>
                            &nbsp;&nbsp;&nbsp;&nbsp;
                            <cc1:CustButton ID="btnSelect" CssClass="smallButton" runat="server" Width="40px"
                                OnClick="btnSelect_Click" OnClientClick="return checkInputText('tabNo1',0);"
                                DisabledWhenSubmit="False" ShowID="01_01010400_003" onkeydown="setfocuschoice('txtUserId','txtName');" />
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblTask" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01010400_019" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 37%">
                            <cc1:CustCheckBox ID="chkP4" runat="server" Text="P4" />
                            &nbsp;&nbsp;<cc1:CustCheckBox ID="chkP4D" runat="server" Text="P4D" /></td>
                    </tr>
                    <tr>
                        <td nowrap colspan="4" style="height: 1px">
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td colspan="4">
                        </td>
                    </tr>
                </table>
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo2" style="">
                        <tr class="trOdd">
                            <td style="width: 15%;" align="right">
                                <cc1:CustLabel ID="lblName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010400_004" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 85%;">
                                <cc1:CustTextBox ID="txtName" runat="server" onkeydown="entersubmit('btnSubmit');"
                                     onblur="changeFullType(this);" onpaste="paste();" Width="300px" MaxLength="5"
                                    checktype="fulltype" BoxName="姓名"></cc1:CustTextBox>
                            </td>
                        </tr>
                         <tr class="trEven">
                            <td style="width: 15%;" align="right">
                                <cc1:CustLabel ID="lblLName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010400_021" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 85%;">
                                <cc1:CustTextBox ID="txtLName" runat="server" onkeydown="entersubmit('btnSubmit');"
                                     onblur="changeFullType(this);" onpaste="paste();" Width="700px" MaxLength="50"
                                    checktype="fulltype" BoxName="中文姓名"></cc1:CustTextBox>(至多50個全型字)
                            </td>
                        </tr>                        
                        <tr class="trOdd">
                            <td style="width: 15%;" align="right">
                                <cc1:CustLabel ID="lblEName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010400_005" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 85%;">
                                <cc1:CustTextBox ID="txtEName" runat="server" checktype="" onkeydown="entersubmit('btnSubmit');"
                                    Width="300px" MaxLength="26" BoxName="英文姓名"></cc1:CustTextBox>(至多26個字)
                                <cc1:CustButton ID="btnTranslation" CssClass="smallButton" runat="server" Width="80px"
                                 ShowID="01_01010400_020" OnClick="btnTranslation_Click"  /></td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%;" align="right">
                                <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010400_022" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 85%;">
                                <cc1:CustTextBox ID="txtRoma" runat="server" onkeydown="entersubmit('btnSubmit');"
                                     onblur="changeFullType(this);" onpaste="paste();" Width="700px" MaxLength="50"
                                    checktype="fulltype" BoxName="長姓名羅馬拼音"></cc1:CustTextBox>(至多50個全型字)
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%;" align="right">
                                <cc1:CustLabel ID="lblPostalDistrict" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01010400_006"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 85%;">
                                <cc1:CustTextBox ID="txtPostalDistrict" runat="server" checktype="num" onkeydown="entersubmit('btnSubmit');"
                                    Width="300px" MaxLength="3" BoxName="管理郵區"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%; height: 25px;" align="right">
                                <cc1:CustLabel ID="lblRemarkOne" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010400_007" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 85%; height: 25px;">
                                <cc1:CustTextBox ID="txtRemarkOne" runat="server" onkeydown="entersubmit('btnSubmit');"
                                    Width="400px" MaxValueMaxLength="52" BoxName="註記一"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%;" align="right">
                                <cc1:CustLabel ID="lblRemarkTwo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010400_008" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 85%;">
                                <cc1:CustTextBox ID="txtRemarkTwo" runat="server" onkeydown="entersubmit('btnSubmit');"
                                    Width="400px" MaxLength="52" BoxName="註記二"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%; height: 25px;" align="right">
                                <cc1:CustLabel ID="lblBirthday" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010400_009" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 85%; height: 25px;">
                                <cc1:CustTextBox ID="txtBirthday" runat="server" checktype="num" onkeydown="entersubmit('btnSubmit');"
                                    Width="156px" MaxLength="8" BoxName="生日"></cc1:CustTextBox>
                                <cc1:CustLabel ID="lblMessage" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010400_010" StickHeight="False"
                                    Width="150px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr>
                            <td nowrap colspan="2" style="height: 1px">
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo3" style="">
                        <tr>
                            <td nowrap colspan="4" style="height: 1px">
                                <cc1:CustGridView ID="grvCardData" runat="server" Width="100%" BorderWidth="0px"
                                    CellPadding="0" CellSpacing="1" BorderStyle="Solid" AllowSorting="True" PagerID="GridPager1">
                                    <PagerSettings Visible="False" />
                                    <RowStyle CssClass="Grid_Item" />
                                    <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                    <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                    <AlternatingRowStyle CssClass="Grid_AlternatingItem" />
                                    <Columns>
                                        <asp:BoundField DataField="CName">
                                            <itemstyle width="14%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="EName">
                                            <itemstyle width="20%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="CardNum">
                                            <itemstyle width="20%" horizontalalign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="ID">
                                            <itemstyle width="14%" horizontalalign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Type">
                                            <itemstyle width="10%" horizontalalign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="P4/P4D">
                                            <itemstyle width="11%" horizontalalign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Status">
                                            <itemstyle width="11%" horizontalalign="Center" />
                                        </asp:BoundField>
                                    </Columns>
                                </cc1:CustGridView>
                                <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right"
                                    OnPageChanged="gpList_PageChanged">
                                </cc1:GridPager>
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo4" style="">
                        <tr class="itemTitle">
                            <td colspan="4" align="center">
                                <cc1:CustButton ID="btnSubmit" CssClass="smallButton" runat="server" Width="40px"
                                    OnClick="btnSubmit_Click" OnClientClick="return checkInputText('tabNo2',1);"
                                    onkeydown="setfocus('txtUserId');" DisabledWhenSubmit="False" ShowID="01_01010400_011" />
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
