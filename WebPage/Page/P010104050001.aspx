<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010104050001.aspx.cs" Inherits="P010104050001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%--2021/01/25_Ares_Stanley-修正格式; 2021/03/04_Ares_Stanley-調整版面--%>
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
    function checkInputText(id)
    {              
        //*檢核輸入欄位身分證號碼是否為空
        if(document.getElementById('txtCardNo1').value.Trim() == "" || document.getElementById('txtCardNo2').value.Trim() == "")
        {
            alert('請輸入統一編號! ');
            document.getElementById('txtCardNo1').focus();
            return false;
        } 
        
        //*檢核欄位輸入規則
        if(!checkInputType(id))
        {
              return false;
        }      
        return true;
    }
    
    //*統一編號(1)、統一編號(2)欄位輸入值改變
  function changeStatus()
  {
        if(document.getElementById('txtCardNo1').value.Trim() != document.getElementById('txtCardNo1Hide').value.Trim() || document.getElementById('txtCardNo2').value.Trim() != document.getElementById('txtCardNo2Hide').value.Trim())
        {
            document.getElementById('txtReceiveNumber').value = "";
            document.getElementById('txtEstablish').value = "";
            clearControls('tabNo2');
        }
        document.getElementById('txtCardNo1Hide').value = document.getElementById('txtCardNo1').value;
        document.getElementById('txtCardNo2Hide').value = document.getElementById('txtCardNo2').value;
        document.getElementById('txtLev').value = "N";
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
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                        <tr class="itemTitle">
                            <td colspan="9">
                                <li>
                                    <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01040400_001"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="lblCardNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_002" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 88%" colspan="8">
                                <cc1:CustTextBox ID="txtCardNo1" runat="server" MaxLength="8" checktype="num" Width="80px"
                                    onkeydown="entersubmit('btnSelect');" BoxName="統一編號一"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtCardNo2" runat="server" MaxLength="4" checktype="num" onkeydown="entersubmit('btnSelect');"
                                    Width="40px" BoxName="統一編號二"></cc1:CustTextBox>
                                <cc1:CustButton ID="btnSelect" runat="server" CssClass="smallButton" ShowID="01_01040400_027"
                                    OnClientClick="return checkInputText('pnlText');" DisabledWhenSubmit="False"
                                    onkeydown="setfocus('txtCardNo1');" OnClick="btnSelect_Click" />
                                <cc1:CustTextBox ID="txtCardNo1Hide" runat="server" MaxLength="8" CssClass="btnHiden"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtCardNo2Hide" runat="server" MaxLength="4" CssClass="btnHiden"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="lblReceiveNumber" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_003"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 22%" colspan="2">
                                <cc1:CustLabel ID="lblReceiveNumberText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="100px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="clbl_MemberService" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_066"
                                    StickHeight="False" Style="color: Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="clbl_MemberServiceText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="100px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblCheckMan" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_005" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 33%" colspan="3">
                                <cc1:CustLabel ID="lblCheckManText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="50px"></cc1:CustLabel>
                            </td>
                        </tr>
                        
                        
                        <tr class="trOdd">
                            <td rowspan="5" align="right" style="width: 12%">
                                <cc1:CustLabel ID="lblShopData" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_030" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblSingleMerchant" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_066"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 77%" colspan="8">
                                <cc1:CustLabel ID="lblSingleMerchantText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblHEADCorpNo" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_067"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 77%" colspan="3">
                                <cc1:CustLabel ID="lblHEADCorpNoText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblAMLCC" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_068"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 77%" colspan="3">
                                <cc1:CustLabel ID="lblAMLCCText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                            </td>
                        </tr>
                        
                        <tr class="trOdd">
                            
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblEstablish" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_004" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblEstablishText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="50px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblCapital" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_006" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblCapitalText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="70px"></cc1:CustLabel>
                                &nbsp<cc1:CustLabel ID="lblMessage" runat="server" CurAlign="left" CurSymbol="&#163;"
                                    FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_028"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblOrganization" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_008"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOrganizationText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="30px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblRisk" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_010" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblRiskText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="50px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblRegName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_007" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 77%" colspan="7">
                                <cc1:CustLabel ID="lblRegNameText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBusinessName" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_009"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 77%" colspan="7">
                                <cc1:CustLabel ID="lblBusinessNameText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                            </td>
                        </tr>
                        
                        <%--<tr class="trOdd">
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblSingleMerchant" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_066"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 77%" colspan="8">
                                <cc1:CustLabel ID="lblSingleMerchantText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                            </td>
                        </tr>--%>
                        
                        <%--<tr class="trOdd">
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblHEADCorpNo" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_067"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 77%" colspan="3">
                                <cc1:CustLabel ID="lblHEADCorpNoText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblAMLCC" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_068"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 77%" colspan="3">
                                <cc1:CustLabel ID="lblAMLCCText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                            </td>
                        </tr>--%>
                        
                        
                        
                        
                        <tr class="trEven">
                            <td align="right" style="width: 12%" rowspan="7">
                                <cc1:CustLabel ID="lblBossData" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040400_032" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="lblBossData1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_065" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBoss" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_012" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblBossText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="70px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBossID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_013" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblBossIDText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="100px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBossTel" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_014" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 22%" colspan="3">
                                <cc1:CustLabel ID="lblBossTelText1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="30px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBossTelText2" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="70px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBossTelText3" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="40px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven"><%--20190730 長姓名需求-負責人部份--%>
                            <%--負責人長姓名--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_080" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="3">                                
                                <cc1:CustCheckBox ID="chkisLongName" runat="server" BoxName="長姓名" Enabled="false"/>
                                <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblboss_1_L" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False"></cc1:CustLabel>
                                </td>
                            <td style="width: 22%" colspan="4">
                                <cc1:CustLabel ID="lblboss_1_Pinyin" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                                </td>
                        </tr><%--20190730 長姓名需求-負責人部份--%>
                        <tr class="trEven">
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblNation" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_069"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 77%" colspan="7">
                                <cc1:CustLabel ID="lblNationText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                            </td>
                        </tr>
                        
                        
                         <tr class="trEven">
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblpassport" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_070"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 77%" colspan="3">
                                <cc1:CustLabel ID="lblpassportText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblPassportTExpDate" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_071"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 77%" colspan="3">
                                <cc1:CustLabel ID="lblPassportTExpDateText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                            </td>
                        </tr>
                        
                        <tr class="trEven">
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblResidentNo" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_072"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 77%" colspan="3">
                                <cc1:CustLabel ID="lblResidentNoText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblResidentExpDate" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_073"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 77%" colspan="3">
                                <cc1:CustLabel ID="lblResidentExpDateText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                            </td>
                        </tr>
                        
                        
                        <tr class="trEven">
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBossChangeDate" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_032" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="lblBossChangeDate1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_061"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblBossChangeDateText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="70px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBossFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_033" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblBossFlagText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="30px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBossBirthday" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_034"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblBossBirthdayText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="30px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBossAt" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_040" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblBossAtText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="30px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblRegAddress" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_015" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 88%" colspan="8">
                                <cc1:CustLabel ID="lblRegAddressText1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="100px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblRegAddressText2" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="220px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblRegAddressText3" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="100px"></cc1:CustLabel>
                            </td>
                        </tr>

                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="lblEmail" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_074" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 88%" colspan="8">
                                <cc1:CustLabel ID="lblEmailText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="" Width="100px" style="display:inline" ></cc1:CustLabel>
                            </td>
                        </tr>

                        <tr class="trOdd" style="display:none">
                            <td align="right" style="width: 12%" rowspan="2">
                                <cc1:CustLabel ID="lblOperData" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040400_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="lblOperData1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_065" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperman1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040400_016" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="lblOperman2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_062" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOpermanText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="60px"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperID1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040400_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_063" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOperIDText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="100px"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperTel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" ShowID="01_01040400_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_064" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="3">
                                <cc1:CustLabel ID="lblOperTelText1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="30px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblOperTelText2" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="70px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblOperTelText3" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="40px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd" style="display:none">
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperChangeDate1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_037" StickHeight="False"></cc1:CustLabel>&nbsp;&nbsp;<br />
                                <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_061" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOperChangeDateText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="50px"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperFlag1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_033" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOperFlagText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="20px"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperBirthday1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_034"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOperBirthdayText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="50px"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" align="right">
                                <cc1:CustLabel ID="lblOperAt" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_040" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblOperAtText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="40px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 12%" rowspan="2">
                                <cc1:CustLabel ID="lblContactMan" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_017" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblContactManName" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_041"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustLabel ID="lblContactManNameText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="90px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblContactManTel" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_050"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 22%" colspan="2">
                                <cc1:CustLabel ID="lblContactManTelText1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="30px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblContactManTelText2" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="60px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblContactManTelText3" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="40px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblFax" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_018" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="2">
                                <cc1:CustLabel ID="lblFaxText1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="30px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblFaxText2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="70px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven"><%--20190730 長姓名需求-聯絡人部份--%>
                            <%--聯絡人長姓名--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_080" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="3">                                
                                <cc1:CustCheckBox ID="chkisLongName_c" runat="server" BoxName="長姓名" Enabled="false"/>
                                <cc1:CustLabel ID="CustLabel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_081" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblcontact_man_L" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="CustLabel10" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_082" StickHeight="False"></cc1:CustLabel>
                                </td>
                            <td style="width: 22%" colspan="4">
                                <cc1:CustLabel ID="lblcontact_man_Pinyin" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="260px"></cc1:CustLabel>
                                </td>
                        </tr><%--20190730 長姓名需求-負責人部份--%>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%" rowspan="2">
                                <cc1:CustLabel ID="lblAddData" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_042" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBookAddress" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_019" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 88%" colspan="7">
                                <cc1:CustLabel ID="lblREG_ZIP_CODE" runat="server" CurAlign="left" CurSymbol="￡"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="80px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBookAddressText1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="100px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBookAddressText2" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="220px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBookAddressText3" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" Width="100px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBusinessAddress" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_020"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="8">
                                <cc1:CustLabel ID="lblBusinessZipText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="" Width="50px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBusinessAddressText1" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="" Width="100px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBusinessAddressText2" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="" Width="220px"></cc1:CustLabel>
                                <cc1:CustLabel ID="lblBusinessAddressText3" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="" Width="100px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%--<td align="right" style="width: 12%">
                                <cc1:CustLabel ID="lblJCIC" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_043" StickHeight="False"
                                    ForeColor="Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 88%" colspan="8">
                                <cc1:CustLabel ID="lblJCICText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="" Width="100px"></cc1:CustLabel>
                            </td>--%>
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="lblJCIC" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_043" StickHeight="False"
                                    ForeColor="Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 16%" colspan="2">
                                <cc1:CustLabel ID="lblJCICText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="" Width="100px"></cc1:CustLabel>
                            </td>
                            <%--Y_特店跨行匯費(6116)--%>
                            <td align="right" style="width: 18%" colspan="1">
                                <cc1:CustLabel ID="lblGrantFeeFlag" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_075"
                                    StickHeight="False" ForeColor="Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 16%" colspan="1">
                                <cc1:CustLabel ID="lblGrantFeeFlagText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--Y_MPOS特店系統服務費免收註記(6086)F001--%>
                            <td align="right" style="width: 20%" colspan="3">
                                <cc1:CustLabel ID="lblMposFlag" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_076" StickHeight="False"
                                    ForeColor="Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 18%" colspan="1">
                                <cc1:CustLabel ID="lblMposFlagText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False"></cc1:CustLabel>
                            </td>
                            
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%" rowspan="2">
                                <cc1:CustLabel ID="lblAccounts" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_044" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBank" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_021" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 33%" colspan="3">
                                <cc1:CustLabel ID="lblBankText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="" Width="80px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBranchBank" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_022" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 33%" colspan="3">
                                <cc1:CustLabel ID="lblBranchBankText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="" Width="140px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_023" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 88%" colspan="8">
                                <cc1:CustLabel ID="lblNameText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="" Width="300px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="lblPrev" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_045" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblPrevDesc" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_046" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 33%" colspan="3">
                                <cc1:CustLabel ID="lblPrevDescText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="" Width="100px"></cc1:CustLabel>
                            </td>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblInvoiceCycle" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_025"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="3">
                                <cc1:CustLabel ID="lblInvoiceCycleText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False" ShowID="" Width="20px"></cc1:CustLabel>
                            </td>
                            <%--                        <td align="right" style="width: 11%">
                            <cc1:CustLabel ID="lblRedeemCycle" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040400_047"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 11%">
                            <cc1:CustLabel ID="lblRedeemCycleText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                    StickHeight="False" ShowID="" Width="30px"></cc1:CustLabel>
                        </td>--%>
                        </tr>
                        <tr class="trOdd">
                            <td align="right" style="width: 12%">
                                <cc1:CustLabel ID="lblPopMan" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040400_024" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 88%" colspan="8">
                                <cc1:CustLabel ID="lblPopManText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" ShowID="" Width="50px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--資料最後異動--%>
                        <tr class="trEven">                            
                            <%--資料最後異動MAKER--%>
                            <td align="right" style="width: 18%; height: 33px;">
                                <cc1:CustLabel ID="lblLAST_UPD_MAKER" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01040101_086"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 16%; height: 33px;"colspan="2">
                                <cc1:CustLabel ID="lblLAST_UPD_MAKERText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--資料最後異動CHECKER--%>
                            <td align="right" style="width: 20%; height: 33px;">
                                <cc1:CustLabel ID="lblLAST_UPD_CHECKER" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_087" StickHeight="False"
                                    ></cc1:CustLabel>
                            </td>
                            <td style="width: 18%; height: 33px;"colspan="2" >
                                <cc1:CustLabel ID="lblLAST_UPD_CHECKERText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"
                                    ></cc1:CustLabel>
                            </td>
                            <%--資料最後異動分行--%>
                            <td align="right" style="width: 11%; height: 33px;">
                                <cc1:CustLabel ID="lblLAST_UPD_BRANCH" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01040101_085" StickHeight="False"
                                    ></cc1:CustLabel>
                            </td>
                            <td style="width: 16%; height: 33px;" colspan="2">
                                <cc1:CustLabel ID="lblLAST_UPD_BRANCHText" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"
                                    ></cc1:CustLabel>
                            </td>
                        </tr>
                        <tr>
                            <td nowrap colspan="8" style="height: 1px">
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
