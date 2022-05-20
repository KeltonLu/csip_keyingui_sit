<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010105030101.aspx.cs" Inherits="P010105030101" %>

<%@ Register Src="~/Common/Controls/CustATypeList.ascx" TagName="CustATypeList" TagPrefix="uc1" %>
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
            var value;
            var value1;
            
            //*檢核輸入欄位【收件編號】是否為空             if(document.getElementById('txtReceiveNumber').value.Trim() == "")
            {
                document.getElementById('txtReceiveNumber').focus();
                alert('請輸入收件編號');

                return false;
            }                        //*檢核輸入欄位【收件編號】前兩碼需為 RK            if (document.getElementById("txtReceiveNumber").value.Trim().substring(0,2).toUpperCase() != "AK")
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
            
            //*新增按鈕檢核
            if(1 == intType)
            {
                if(!checkInputType(id))
                {
                    return false;
                }
                
                if (!checkUserControl('atl_AT1'))
                {
                    return false;
                }
                
                var valueAdd = document.getElementById('txtTCCODE_Add').value.Trim();
                if (valueAdd != "40" && valueAdd != "41")
                {
                    if (!confirm("加項值不為40/41，請確認是否輸入"))
                    {
                        document.getElementById('txtTCCODE_Add').focus();
               
                        return false;
                    }
                }
                
                var valueAdd = document.getElementById('txtTCCODE_Minus').value.Trim();
                if (valueAdd != "40" && valueAdd != "41")
                {
                    if (!confirm("減項值不為40/41，請確認是否輸入"))
                    {
                        document.getElementById('txtTCCODE_Minus').focus();
               
                        return false;
                    }
                }
                
                var valueArea = document.getElementById('txtConsumptionArea').value.Trim().toUpperCase();
                if (!("I" == valueArea || "N" == valueArea || "X" == valueArea))
                {
                    alert('設定消費地區只能輸入I/N/X');
                    document.getElementById('txtConsumptionArea').focus();
                   
                    return false;
                }
                value = "";
                if ("I" == valueArea || "X" == valueArea)
                {
                    for (i=1; i<11; i++)
                    {
                        value = value + document.getElementById('txtConsumptionAreaCode' + i).value.Trim();
                    }
                    
                    if ("" == value)
                    {
                        alert('請至少填一組地區代碼！');
                        document.getElementById('txtConsumptionAreaCode1').focus();
                       
                        return false;
                    }
                }
                
                //  正卡
                var valueMCSP = document.getElementById('txtMainCardSetPoint').value.Trim();
                if (!("" == valueMCSP || "0" == valueMCSP || "1" == valueMCSP || "2" == valueMCSP))
                {
                    alert('給點方式只能輸入0/1/2或空');
                    document.getElementById('txtMainCardSetPoint').focus();
                   
                    return false;
                }
                
                value = "";
                value1 = "";
                if ("" != valueMCSP)
                {
                    for (i=1; i<5; i++)
                    {
                        value = value + document.getElementById('MainCardAMT' + i).value.Trim();
                        value1 = value1 + document.getElementById('MainCardRATE' + i).value.Trim();
                    }
                    
                    if ("" == value)
                    {
                        alert('請至少填一組給點規則(ATM)！');
                        document.getElementById('MainCardAMT1').focus();
                       
                        return false;
                    }
                    
                    if ("" == value1)
                    {
                        alert('請至少填一組給點規則(RATE)！');
                        document.getElementById('MainCardRATE1').focus();
                       
                        return false;
                    }
                }
                
                // 附卡
                var valueSCSP = document.getElementById('txtSUPPCardSetPoint').value.Trim();
                if (!("" == valueSCSP || "0" == valueSCSP || "1" == valueSCSP || "2" == valueSCSP))
                {
                    alert('給點方式只能輸入0/1/2或空');
                    document.getElementById('txtSUPPCardSetPoint').focus();
                   
                    return false;
                }
                
                value = "";
                value1 = "";
                if ("" != valueSCSP)
                {
                    for (i=1; i<5; i++)
                    {
                        value = value + document.getElementById('SUPPCardAMT' + i).value.Trim();
                        value1 = value1 + document.getElementById('SUPPCardRATE' + i).value.Trim();
                    }
                    
                    if ("" == value)
                    {
                        alert('請至少填一組給點規則(ATM)！');
                        document.getElementById('SUPPCardAMT1').focus();
                       
                        return false;
                    }
                    
                    if ("" == value1)
                    {
                        alert('請至少填一組給點規則(RATE)！');
                        document.getElementById('SUPPCardRATE1').focus();
                       
                        return false;
                    }
                }
                
                // 短期活動
                var valueUESP = document.getElementById('txtUSEREXITSetPoint').value.Trim();
                if (!("" == valueUESP || "0" == valueUESP || "1" == valueUESP || "2" == valueUESP))
                {
                    alert('給點方式只能輸入0/1/2或空');
                    document.getElementById('txtUSEREXITSetPoint').focus();
                   
                    return false;
                }
                
                value = "";
                value1 = "";
                if ("" != valueUESP)
                {
                    for (i=1; i<5; i++)
                    {
                        value = value + document.getElementById('USEREXITAMT' + i).value.Trim();
                        value1 = value1 + document.getElementById('USEREXITRATE' + i).value.Trim();
                    }
                    
                    if ("" == value)
                    {
                        alert('請至少填一組給點規則(ATM)！');
                        document.getElementById('USEREXITAMT1').focus();
                       
                        return false;
                    }
                    
                    if ("" == value1)
                    {
                        alert('請至少填一組給點規則(RATE)！');
                        document.getElementById('USEREXITRATE1').focus();
                       
                        return false;
                    }
                }
                
                var valueUEFrom = document.getElementById('txtUSEREXITFrom').value.Trim();
                if (-2 == checkDateSn((parseFloat(valueUEFrom.substring(4,8))-1911).toString()+valueUEFrom.substring(0,4)))
                {
                        alert('活動時間起日錯誤！');
                        document.getElementById('txtUSEREXITFrom').focus();
                       
                        return false;
                }
                
                var valueUETo = document.getElementById('txtUSEREXITTo').value.Trim();
                if (-2 == checkDateSn((parseFloat(valueUETo.substring(4,8))-1911).toString()+valueUETo.substring(0,4)))
                {
                        alert('活動時間迄日錯誤！');
                        document.getElementById('txtUSEREXITTo').focus();
                       
                        return false;
                }
                
                
                // 生日活動
                var valueBSP = document.getElementById('txtBIRTHSetPoint').value.Trim();
                if (!("" == valueBSP || "0" == valueBSP || "1" == valueBSP || "2" == valueBSP))
                {
                    alert('給點方式只能輸入0/1/2或空');
                    document.getElementById('txtBIRTHSetPoint').focus();
                   
                    return false;
                }
                
                value = "";
                value1 = "";
                if ("" != valueBSP)
                {
                    for (i=1; i<5; i++)
                    {
                        value = value + document.getElementById('BIRTHAMT' + i).value.Trim();
                        value1 = value1 + document.getElementById('BIRTHRATE' + i).value.Trim();
                    }
                    
                    if ("" == value)
                    {
                        alert('請至少填一組給點規則(ATM)！');
                        document.getElementById('BIRTHAMT1').focus();
                       
                        return false;
                    }
                    
                    if ("" == value1)
                    {
                        alert('請至少填一組給點規則(RATE)！');
                        document.getElementById('BIRTHRATE1').focus();
                       
                        return false;
                    }
                }
                
                var valueBFrom = document.getElementById('txtBIRTHFrom').value.Trim();
                if (-2 == checkDateSn((parseFloat(valueBFrom.substring(4,8))-1911).toString()+valueBFrom.substring(0,4)))
                {
                        alert('活動時間起日錯誤！');
                        document.getElementById('txtBIRTHFrom').focus();
                       
                        return false;
                }
                
                var valueBTo = document.getElementById('txtBIRTHTo').value.Trim();
                if (-2 == checkDateSn((parseFloat(valueBTo.substring(4,8))-1911).toString()+valueBTo.substring(0,4)))
                {
                        alert('活動時間迄日錯誤！');
                        document.getElementById('txtBIRTHTo').focus();
                       
                        return false;
                }
                
                
            }
            return true;
            
        }
        
        function setDisable()
        {
            var value = document.getElementById('txtConsumptionArea').value.Trim().toUpperCase();
            
            for (i=1; i<11; i++)
            {
                if ("X" == value || "I" == value)
                {
                
                    document.getElementById('txtConsumptionAreaCode' + i).disabled = false;
                }
                else
                {
                    document.getElementById('txtConsumptionAreaCode' + i).disabled = true;
                    document.getElementById('txtConsumptionAreaCode' + i).value = "";
                }
            }
        }
        
        function fillstring(obj,ilength)
        {
            var szero = "";
            for (i=0; i<ilength; i++)
            {
                szero += "0";
            }

            szero = szero + obj.value.Trim();
            
            obj.value = szero.substring(szero.length - ilength,szero.length);
        }
        
        function checkUserControl(idTitle)
        {
            if ("" == document.getElementById('txtORG').value.Trim())
            {
                document.getElementById('txtORG').focus();
                alert('此欄位為必填項！');
                return false;
            }
            
            if ("" == document.getElementById('txtPROGRAMNO').value.Trim())
            {
                document.getElementById('txtPROGRAMNO').focus();
                alert('此欄位為必填項！');
                return false;
            }
            
            if ("" == document.getElementById('txtPARTNERNO').value.Trim())
            {
                document.getElementById('txtPARTNERNO').focus();
                alert('此欄位為必填項！');
                return false;
            }
            
            var objinput = document.getElementsByTagName("INPUT");
            var len = objinput.length;
            var isNull = true;

            for (i=0; i<len; i++)
            {
                if ("text" == objinput[i].type && !objinput[i].disabled)
                {
                    if (idTitle == objinput[i].id.substring(0,idTitle.length) && "_AType" == objinput[i].id.substring(objinput[i].id.length-7,objinput[i].id.length-1))
                    {
                        if (objinput[i].value.Trim() != "")
                        {       
                            isNull = false;
                            break;
                        }
                    }
                }
            }

            if (isNull)
            {
                document.getElementById(idTitle + '_tb_AType0').focus();
                alert('請至少填一項！');
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
                <ContentTemplate>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                        <tr class="itemTitle">
                            <td colspan="2">
                                <li>
                                    <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" Width="200px" IsColon="False" ShowID="01_01050301_001"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblReceiveNumber" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01010500_002"
                                    StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 85%">
                                <cc1:CustTextBox ID="txtReceiveNumber" runat="server" MaxLength="12" checktype="numandletter"></cc1:CustTextBox>
                                <cc1:CustButton ID="btnSelect" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                    Width="40px" OnClientClick="return checkInputText('tabNo1',0);" onkeydown="setfocuschoice('txtReceiveNumber','txtORG');"
                                    OnClick="btnSelect_Click" ShowID="01_01050301_037" /></td>
                        </tr>
                    </table>
                    <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                        <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table1">
                            <tr class="itemTitle">
                                <td colspan="2">
                                    <cc1:CustLabel ID="lbTitle1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_002" StickHeight="False"
                                        Font-Bold="True"></cc1:CustLabel>
                                </td>
                                <td style="width: 40%" align="center">
                                    <cc1:CustLabel ID="lbUSER" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_012" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustLabel ID="lbUSERValue" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td style="width: 35%" align="right">
                                    <cc1:CustLabel ID="lbORG" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_003" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td colspan="2">
                                    <cc1:CustTextBox ID="txtORG" runat="server" MaxLength="3" checktype="num"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td align="right">
                                    <cc1:CustLabel ID="lbPROGRAMNO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_004" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td colspan="2">
                                    <cc1:CustTextBox ID="txtPROGRAMNO" runat="server" MaxLength="5" checktype="num"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td align="right">
                                    <cc1:CustLabel ID="lbPARTNERNO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_005" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td colspan="2">
                                    <cc1:CustTextBox ID="txtPARTNERNO" runat="server" MaxLength="9" checktype="num"></cc1:CustTextBox>
                                </td>
                            </tr>
                        </table>
                        <uc1:CustATypeList ID="atl_AT1" runat="server" FirstRowClass="trOdd" IsAType1="true"
                            ATypeShowID="01_01050301_006" RTBCount="16" IsRedeem="false"></uc1:CustATypeList>
                        <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table2">
                            <tr class="itemTitle">
                                <td colspan="2">
                                    <cc1:CustLabel ID="lbTCCODE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_007"
                                        Font-Bold="True"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lbTCCODE_Add" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_008" StickHeight="False"></cc1:CustLabel></td>
                                <td style="width: 85%">
                                    <cc1:CustTextBox ID="txtTCCODE_Add" runat="server" MaxLength="2" checktype="num"
                                        Width="30px"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td style="width: 15%" align="right">
                                    <cc1:CustLabel ID="lbTCCODE_Minus" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_009" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td style="width: 85%">
                                    <cc1:CustTextBox ID="txtTCCODE_Minus" runat="server" MaxLength="2" checktype="num"
                                        Width="30px"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td colspan="2">
                                    <cc1:CustLabel ID="lbMMCCODE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_010" StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td colspan="2" align="center">
                                    <cc1:CustTextBox ID="txtMMCCODE_1_B" runat="server" MaxLength="4" checktype="num"
                                        Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbMMCCODE_1_B" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_011" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtMMCCODE_1_E" runat="server" MaxLength="4" checktype="num"
                                        Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbMMCCODE_1_E" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_012" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtMMCCODE_2_B" runat="server" MaxLength="4" checktype="num"
                                        Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbMMCCODE_2_B" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_011" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtMMCCODE_2_E" runat="server" MaxLength="4" checktype="num"
                                        Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbMMCCODE_2_E" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_012" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtMMCCODE_3_B" runat="server" MaxLength="4" checktype="num"
                                        Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbMMCCODE_3_B" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_011" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtMMCCODE_3_E" runat="server" MaxLength="4" checktype="num"
                                        Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbMMCCODE_3_E" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_012" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtMMCCODE_4_B" runat="server" MaxLength="4" checktype="num"
                                        Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbMMCCODE_4_B" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_011" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtMMCCODE_4_E" runat="server" MaxLength="4" checktype="num"
                                        Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbMMCCODE_4_E" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_012" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtMMCCODE_5_B" runat="server" MaxLength="4" checktype="num"
                                        Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbMMCCODE_5_B" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_011" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtMMCCODE_5_E" runat="server" MaxLength="4" checktype="num"
                                        Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td colspan="2" align="center">
                                    <cc1:CustTextBox ID="txtMMCCODE_6_B" runat="server" MaxLength="4" checktype="num"
                                        Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbMMCCODE_6_B" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_011" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtMMCCODE_6_E" runat="server" MaxLength="4" checktype="num"
                                        Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbMMCCODE_6_E" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_012" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtMMCCODE_7_B" runat="server" MaxLength="4" checktype="num"
                                        Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbMMCCODE_7_B" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_011" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtMMCCODE_7_E" runat="server" MaxLength="4" checktype="num"
                                        Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbMMCCODE_7_E" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_012" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtMMCCODE_8_B" runat="server" MaxLength="4" checktype="num"
                                        Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbMMCCODE_8_B" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_011" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtMMCCODE_8_E" runat="server" MaxLength="4" checktype="num"
                                        Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbMMCCODE_8_E" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_012" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtMMCCODE_9_B" runat="server" MaxLength="4" checktype="num"
                                        Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbMMCCODE_9_B" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_011" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtMMCCODE_9_E" runat="server" MaxLength="4" checktype="num"
                                        Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbMMCCODE_9_E" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_012" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtMMCCODE_10_B" runat="server" MaxLength="4" checktype="num"
                                        Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbMMCCODE_10_B" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_011" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtMMCCODE_10_E" runat="server" MaxLength="4" checktype="num"
                                        Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td colspan="2">
                                    <cc1:CustLabel ID="lbsetArea" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_013" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustLabel ID="lbsetAreaOnlyOne" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_014"
                                        StickHeight="False" ForeColor="Tomato"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td colspan="2" align="center">
                                    <cc1:CustTextBox ID="txtConsumptionArea" runat="server" MaxLength="1" checktype="letter"
                                        Width="20px" onkeyup="setDisable();"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbConsumptionArea" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_017"
                                        StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtConsumptionAreaCode1" runat="server" MaxLength="2" checktype="num"
                                        Width="30px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbConsumptionAreaCode1" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_012"
                                        StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtConsumptionAreaCode2" runat="server" MaxLength="2" checktype="num"
                                        Width="30px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbConsumptionAreaCode2" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_012"
                                        StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtConsumptionAreaCode3" runat="server" MaxLength="2" checktype="num"
                                        Width="30px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbConsumptionAreaCode3" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_012"
                                        StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtConsumptionAreaCode4" runat="server" MaxLength="2" checktype="num"
                                        Width="30px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbConsumptionAreaCode4" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_012"
                                        StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtConsumptionAreaCode5" runat="server" MaxLength="2" checktype="num"
                                        Width="30px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbConsumptionAreaCode5" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_012"
                                        StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtConsumptionAreaCode6" runat="server" MaxLength="2" checktype="num"
                                        Width="30px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbConsumptionAreaCode6" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_012"
                                        StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtConsumptionAreaCode7" runat="server" MaxLength="2" checktype="num"
                                        Width="30px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbConsumptionAreaCode7" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_012"
                                        StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtConsumptionAreaCode8" runat="server" MaxLength="2" checktype="num"
                                        Width="30px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbConsumptionAreaCode8" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_012"
                                        StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtConsumptionAreaCode9" runat="server" MaxLength="2" checktype="num"
                                        Width="30px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbConsumptionAreaCode9" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_012"
                                        StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtConsumptionAreaCode10" runat="server" MaxLength="2" checktype="num"
                                        Width="30px"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td colspan="2">
                                    <cc1:CustLabel ID="lbsetArea_Memo1" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_015"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td colspan="2">
                                    <cc1:CustLabel ID="lbsetArea_Memo2" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_016"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                        </table>
                        <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table3">
                            <tr class="itemTitle">
                                <td style="width: 50%">
                                    <cc1:CustLabel ID="lbLongtime" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_018"
                                        Font-Bold="True"></cc1:CustLabel>
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td align="center">
                                    <cc1:CustLabel ID="lbMainCard" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_019" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustLabel ID="lbMainCardOnlyOne" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_014"
                                        StickHeight="False" ForeColor="Tomato"></cc1:CustLabel>
                                </td>
                                <td align="center">
                                    <cc1:CustLabel ID="lbSUPPCard" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050301_020" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustLabel ID="lbSUPPCardOnlyOne" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_014"
                                        StickHeight="False" ForeColor="Tomato"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td align="center">
                                    <cc1:CustTextBox ID="txtMainCardSetPoint" runat="server" MaxLength="1" checktype="num"
                                        Width="20px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbMainCardSetPointMemo1" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_021"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td align="center">
                                    <cc1:CustTextBox ID="txtSUPPCardSetPoint" runat="server" MaxLength="1" checktype="num"
                                        Width="20px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbSUPPCardSetPointMemo1" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_021"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td align="center">
                                    <cc1:CustLabel ID="lbMainCardSetPointMemo2" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_022"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td align="center">
                                    <cc1:CustLabel ID="lbSUPPCardSetPointMemo2" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_022"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td align="center">
                                    <cc1:CustLabel ID="lbMainCardSetPointMemo3" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_023"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td align="center">
                                    <cc1:CustLabel ID="lbSUPPCardSetPointMemo3" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_023"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td>
                                    <cc1:CustLabel ID="lbMainCardSetPointRule" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_024"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td>
                                    <cc1:CustLabel ID="lbSUPPCardSetPointRule" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_024"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr style="background-color: #006666">
                                <td align="center" valign="middle">
                                    <table width="80%" border="0" cellpadding="0" cellspacing="1" id="Table4">
                                        <tr class="trEven">
                                            <td align="center" style="width: 70%">
                                                <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                                    SetBreak="False" SetOmit="False" ShowID="01_01050301_025" StickHeight="False"></cc1:CustLabel>
                                            </td>
                                            <td align="center">
                                                <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                                    SetBreak="False" SetOmit="False" ShowID="01_01050301_026" StickHeight="False"></cc1:CustLabel>
                                            </td>
                                        </tr>
                                        <tr class="trOdd">
                                            <td align="center">
                                                <cc1:CustTextBox ID="MainCardAMT1" runat="server" checktype="num" MaxLength="9" Width="90px" onblur="fillstring(this,9);"></cc1:CustTextBox></td>
                                            <td align="center">
                                                <cc1:CustTextBox ID="MainCardRATE1" runat="server" checktype="num" MaxLength="4"
                                                    Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox></td>
                                        </tr>
                                        <tr class="trEven">
                                            <td align="center">
                                                <cc1:CustTextBox ID="MainCardAMT2" runat="server" checktype="num" MaxLength="9" Width="90px" onblur="fillstring(this,9);"></cc1:CustTextBox></td>
                                            <td align="center">
                                                <cc1:CustTextBox ID="MainCardRATE2" runat="server" checktype="num" MaxLength="4"
                                                    Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox></td>
                                        </tr>
                                        <tr class="trOdd">
                                            <td align="center">
                                                <cc1:CustTextBox ID="MainCardAMT3" runat="server" checktype="num" MaxLength="9" Width="90px" onblur="fillstring(this,9);"></cc1:CustTextBox></td>
                                            <td align="center">
                                                <cc1:CustTextBox ID="MainCardRATE3" runat="server" checktype="num" MaxLength="4"
                                                    Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox></td>
                                        </tr>
                                        <tr class="trEven">
                                            <td align="center">
                                                <cc1:CustTextBox ID="MainCardAMT4" runat="server" checktype="num" MaxLength="9" Width="90px" onblur="fillstring(this,9);"></cc1:CustTextBox></td>
                                            <td align="center">
                                                <cc1:CustTextBox ID="MainCardRATE4" runat="server" checktype="num" MaxLength="4"
                                                    Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox></td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" valign="middle">
                                    <table width="80%" border="0" cellpadding="0" cellspacing="1" id="Table5">
                                        <tr class="trEven">
                                            <td align="center" style="width: 70%">
                                                <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                                    SetBreak="False" SetOmit="False" ShowID="01_01050301_025" StickHeight="False"></cc1:CustLabel>
                                            </td>
                                            <td align="center">
                                                <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                                    SetBreak="False" SetOmit="False" ShowID="01_01050301_026" StickHeight="False"></cc1:CustLabel>
                                            </td>
                                        </tr>
                                        <tr class="trOdd">
                                            <td align="center">
                                                <cc1:CustTextBox ID="SUPPCardAMT1" runat="server" checktype="num" MaxLength="9" Width="90px" onblur="fillstring(this,9);"></cc1:CustTextBox></td>
                                            <td align="center">
                                                <cc1:CustTextBox ID="SUPPCardRATE1" runat="server" checktype="num" MaxLength="4" onblur="fillstring(this,4);"
                                                    Width="40px"></cc1:CustTextBox></td>
                                        </tr>
                                        <tr class="trEven">
                                            <td align="center">
                                                <cc1:CustTextBox ID="SUPPCardAMT2" runat="server" checktype="num" MaxLength="9" Width="90px" onblur="fillstring(this,9);"></cc1:CustTextBox></td>
                                            <td align="center">
                                                <cc1:CustTextBox ID="SUPPCardRATE2" runat="server" checktype="num" MaxLength="4" onblur="fillstring(this,4);"
                                                    Width="40px"></cc1:CustTextBox></td>
                                        </tr>
                                        <tr class="trOdd">
                                            <td align="center">
                                                <cc1:CustTextBox ID="SUPPCardAMT3" runat="server" checktype="num" MaxLength="9" Width="90px" onblur="fillstring(this,9);"></cc1:CustTextBox></td>
                                            <td align="center">
                                                <cc1:CustTextBox ID="SUPPCardRATE3" runat="server" checktype="num" MaxLength="4" onblur="fillstring(this,4);"
                                                    Width="40px"></cc1:CustTextBox></td>
                                        </tr>
                                        <tr class="trEven">
                                            <td align="center">
                                                <cc1:CustTextBox ID="SUPPCardAMT4" runat="server" checktype="num" MaxLength="9" Width="90px" onblur="fillstring(this,9);"></cc1:CustTextBox></td>
                                            <td align="center">
                                                <cc1:CustTextBox ID="SUPPCardRATE4" runat="server" checktype="num" MaxLength="4" onblur="fillstring(this,4);"
                                                    Width="40px"></cc1:CustTextBox></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table6">
                            <tr class="itemTitle">
                                <td>
                                    <cc1:CustLabel ID="lbPromotionsTitle" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_027"
                                        Font-Bold="True"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td>
                                    <cc1:CustLabel ID="lbPromotionsMemo" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_028"
                                        Font-Bold="False" ForeColor="Tomato"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td>
                                    <cc1:CustLabel ID="lbUSEREXIT" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_029"
                                        Font-Bold="False" ForeColor="Fuchsia"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td>
                                    <cc1:CustLabel ID="lbUSEREXITFrom" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_031"
                                        Font-Bold="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtUSEREXITFrom" runat="server" MaxLength="8" checktype="num"
                                        Width="80px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbUSEREXITTo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_032"
                                        Font-Bold="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtUSEREXITTo" runat="server" MaxLength="8" checktype="num"
                                        Width="80px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbUSEREXITFORMAT" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_033"
                                        Font-Bold="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td>
                                    <cc1:CustLabel ID="lbUSEREXITPOINT" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_034"
                                        Font-Bold="False"></cc1:CustLabel>
                                    <cc1:CustLabel ID="lbUSEREXITPOINTONLYONE" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_014"
                                        StickHeight="False" ForeColor="Tomato"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td>
                                    <cc1:CustLabel ID="lbSPACE1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_036"
                                        Font-Bold="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtUSEREXITSetPoint" runat="server" MaxLength="1" checktype="num"
                                        Width="20px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbUSEREXITSetPoint0" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_021"
                                        Font-Bold="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td>
                                    <cc1:CustLabel ID="lbSPACE2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_036"
                                        Font-Bold="False"></cc1:CustLabel>
                                    <cc1:CustLabel ID="lbUSEREXITSetPoint1" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_022"
                                        Font-Bold="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td>
                                    <cc1:CustLabel ID="lbSPACE3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_036"
                                        Font-Bold="False"></cc1:CustLabel>
                                    <cc1:CustLabel ID="lbUSEREXITSetPoint2" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_023"
                                        Font-Bold="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td>
                                    <cc1:CustLabel ID="lbUSEREXITPointRule" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_035"
                                        Font-Bold="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr style="background-color: #006666">
                                <td>
                                    <table width="100%" border="0" cellpadding="0" cellspacing="0" id="Table10">
                                        <tr>
                                            <td style="width: 50%" align="center">
                                                <table width="80%" border="0" cellpadding="0" cellspacing="1" id="Table7">
                                                    <tr class="trEven">
                                                        <td align="center" style="width: 70%">
                                                            <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                                                SetBreak="False" SetOmit="False" ShowID="01_01050301_025" StickHeight="False"></cc1:CustLabel>
                                                        </td>
                                                        <td align="center">
                                                            <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                                                SetBreak="False" SetOmit="False" ShowID="01_01050301_026" StickHeight="False"></cc1:CustLabel>
                                                        </td>
                                                    </tr>
                                                    <tr class="trOdd">
                                                        <td align="center">
                                                            <cc1:CustTextBox ID="USEREXITAMT1" runat="server" checktype="num" MaxLength="9" Width="90px" onblur="fillstring(this,9);"></cc1:CustTextBox></td>
                                                        <td align="center">
                                                            <cc1:CustTextBox ID="USEREXITRATE1" runat="server" checktype="num" MaxLength="4" onblur="fillstring(this,4);"
                                                                Width="40px"></cc1:CustTextBox></td>
                                                    </tr>
                                                    <tr class="trEven">
                                                        <td align="center">
                                                            <cc1:CustTextBox ID="USEREXITAMT2" runat="server" checktype="num" MaxLength="9" Width="90px" onblur="fillstring(this,9);"></cc1:CustTextBox></td>
                                                        <td align="center">
                                                            <cc1:CustTextBox ID="USEREXITRATE2" runat="server" checktype="num" MaxLength="4" onblur="fillstring(this,4);"
                                                                Width="40px"></cc1:CustTextBox></td>
                                                    </tr>
                                                    <tr class="trOdd">
                                                        <td align="center">
                                                            <cc1:CustTextBox ID="USEREXITAMT3" runat="server" checktype="num" MaxLength="9" Width="90px" onblur="fillstring(this,9);"></cc1:CustTextBox></td>
                                                        <td align="center">
                                                            <cc1:CustTextBox ID="USEREXITRATE3" runat="server" checktype="num" MaxLength="4" onblur="fillstring(this,4);"
                                                                Width="40px"></cc1:CustTextBox></td>
                                                    </tr>
                                                    <tr class="trEven">
                                                        <td align="center">
                                                            <cc1:CustTextBox ID="USEREXITAMT4" runat="server" checktype="num" MaxLength="9" Width="90px" onblur="fillstring(this,9);"></cc1:CustTextBox></td>
                                                        <td align="center">
                                                            <cc1:CustTextBox ID="USEREXITRATE4" runat="server" checktype="num" MaxLength="4" onblur="fillstring(this,4);"
                                                                Width="40px"></cc1:CustTextBox></td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td>
                                    <cc1:CustLabel ID="lbBIRTH" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_030"
                                        Font-Bold="False" ForeColor="Fuchsia"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td>
                                    <cc1:CustLabel ID="lbBIRTHFrom" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_031"
                                        Font-Bold="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtBIRTHFrom" runat="server" MaxLength="8" checktype="num" Width="80px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbBIRTHTo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_032"
                                        Font-Bold="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtBIRTHTo" runat="server" MaxLength="8" checktype="num" Width="80px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbBIRTHFORMAT" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_033"
                                        Font-Bold="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td>
                                    <cc1:CustLabel ID="lbBIRTHPOINT" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_034"
                                        Font-Bold="False"></cc1:CustLabel>
                                    <cc1:CustLabel ID="lbBIRTHPOINTONLYONE" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050301_014"
                                        StickHeight="False" ForeColor="Tomato"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td>
                                    <cc1:CustLabel ID="lbSPACE5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_036"
                                        Font-Bold="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtBIRTHSetPoint" runat="server" MaxLength="1" checktype="num"
                                        Width="20px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbBIRTHSetPoint0" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_021"
                                        Font-Bold="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td>
                                    <cc1:CustLabel ID="lbSPACE6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_036"
                                        Font-Bold="False"></cc1:CustLabel>
                                    <cc1:CustLabel ID="lbBIRTHSetPoint1" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_022"
                                        Font-Bold="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td>
                                    <cc1:CustLabel ID="lbSPACE7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_036"
                                        Font-Bold="False"></cc1:CustLabel>
                                    <cc1:CustLabel ID="lbBIRTHSetPoint2" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_023"
                                        Font-Bold="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td>
                                    <cc1:CustLabel ID="lbBIRTHPointRule" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" IsColon="False" ShowID="01_01050301_035"
                                        Font-Bold="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr style="background-color: #006666">
                                <td>
                                    <table width="100%" border="0" cellpadding="0" cellspacing="0" id="Table9">
                                        <tr>
                                            <td style="width: 50%" align="center">
                                                <table width="80%" border="0" cellpadding="0" cellspacing="1" id="Table8">
                                                    <tr class="trEven">
                                                        <td align="center" style="width: 70%">
                                                            <cc1:CustLabel ID="CustLabel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                                                SetBreak="False" SetOmit="False" ShowID="01_01050301_025" StickHeight="False"></cc1:CustLabel>
                                                        </td>
                                                        <td align="center">
                                                            <cc1:CustLabel ID="CustLabel9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                                                SetBreak="False" SetOmit="False" ShowID="01_01050301_026" StickHeight="False"></cc1:CustLabel>
                                                        </td>
                                                    </tr>
                                                    <tr class="trOdd">
                                                        <td align="center">
                                                            <cc1:CustTextBox ID="BIRTHAMT1" runat="server" checktype="num" MaxLength="9" Width="90px" onblur="fillstring(this,9);"></cc1:CustTextBox></td>
                                                        <td align="center">
                                                            <cc1:CustTextBox ID="BIRTHRATE1" runat="server" checktype="num" MaxLength="4" Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox></td>
                                                    </tr>
                                                    <tr class="trEven">
                                                        <td align="center">
                                                            <cc1:CustTextBox ID="BIRTHAMT2" runat="server" checktype="num" MaxLength="9" Width="90px" onblur="fillstring(this,9);"></cc1:CustTextBox></td>
                                                        <td align="center">
                                                            <cc1:CustTextBox ID="BIRTHRATE2" runat="server" checktype="num" MaxLength="4" Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox></td>
                                                    </tr>
                                                    <tr class="trOdd">
                                                        <td align="center">
                                                            <cc1:CustTextBox ID="BIRTHAMT3" runat="server" checktype="num" MaxLength="9" Width="90px" onblur="fillstring(this,9);"></cc1:CustTextBox></td>
                                                        <td align="center">
                                                            <cc1:CustTextBox ID="BIRTHRATE3" runat="server" checktype="num" MaxLength="4" Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox></td>
                                                    </tr>
                                                    <tr class="trEven">
                                                        <td align="center">
                                                            <cc1:CustTextBox ID="BIRTHAMT4" runat="server" checktype="num" MaxLength="9" Width="90px" onblur="fillstring(this,9);"></cc1:CustTextBox></td>
                                                        <td align="center">
                                                            <cc1:CustTextBox ID="BIRTHRATE4" runat="server" checktype="num" MaxLength="4" Width="40px" onblur="fillstring(this,4);"></cc1:CustTextBox></td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr class="itemTitle">
                                <td align="center">
                                    <cc1:CustButton ID="btnAdd" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                        Width="40px" OnClick="btnAdd_Click" ShowID="01_01050301_038" OnClientClick="return checkInputText('pnlText', 1);" /></td>
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
