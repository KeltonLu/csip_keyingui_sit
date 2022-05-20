<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010105010101.aspx.cs" Inherits="P010105010101" %>

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
            //*檢核輸入欄位【收件編號】是否為空             if(document.getElementById('txtReceiveNumber').value.Trim() == "")
            {
                document.getElementById('txtReceiveNumber').focus();
                alert('請輸入收件編號');

                return false;
            }                        //*檢核輸入欄位【收件編號】前兩碼需為 RK            if (document.getElementById("txtReceiveNumber").value.Trim().substring(0,2).toUpperCase() != "RK")
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
                
                if (parseInt(document.getElementById('txtMERCHANT_INT').value.Trim(),10) > 100)
                {
                    alert('本行分攤比例整數部份不能大於100！');
                    document.getElementById('txtMERCHANT_INT').focus();
               
                    return false;
                }
                
                // 長期活動
                if (parseInt(document.getElementById('atl_AT1_tbTBBLimit_INT').value.Trim(),10) > 100)
                {
                    alert('折抵上限整數部份不能大於100！');
                    document.getElementById('atl_AT1_tbTBBLimit_INT').focus();
               
                    return false;
                }
                
                if (parseInt(document.getElementById('atl_AT2_tbTBBLimit_INT').value.Trim(),10) > 100)
                {
                    alert('折抵上限整數部份不能大於100！');
                    document.getElementById('atl_AT2_tbTBBLimit_INT').focus();
               
                    return false;
                }
                
                if (parseInt(document.getElementById('atl_AT3_tbTBBLimit_INT').value.Trim(),10) > 100)
                {
                    alert('折抵上限整數部份不能大於100！');
                    document.getElementById('atl_AT3_tbTBBLimit_INT').focus();
               
                    return false;
                }
                
                //短期促銷
                if (parseInt(document.getElementById('atl_USEREXITAT1_tbTBBLimit_INT').value.Trim(),10) > 100)
                {
                    alert('折抵上限整數部份不能大於100！');
                    document.getElementById('atl_USEREXITAT1_tbTBBLimit_INT').focus();
               
                    return false;
                }
                
                if (parseInt(document.getElementById('atl_USEREXITAT2_tbTBBLimit_INT').value.Trim(),10) > 100)
                {
                    alert('折抵上限整數部份不能大於100！');
                    document.getElementById('atl_USEREXITAT2_tbTBBLimit_INT').focus();
               
                    return false;
                }
                
                if (parseInt(document.getElementById('atl_USEREXITAT3_tbTBBLimit_INT').value.Trim(),10) > 100)
                {
                    alert('折抵上限整數部份不能大於100！');
                    document.getElementById('atl_USEREXITAT3_tbTBBLimit_INT').focus();
               
                    return false;
                }
                
                var valueS = document.getElementById('txtUSEREXIT_Type').value.Trim();
                if (!("00" == valueS || "01" == valueS || "02" == valueS || "03" == valueS))
                {
                    alert('類型只能輸入00/01/02/03');
                    document.getElementById('txtUSEREXIT_Type').focus();
               
                    return false;
                }
                
                var valueUES = document.getElementById('txtUSEREXIT_DateS').value.Trim();
                if (-2 == checkDateSn((parseFloat(valueUES.substring(4,8))-1911).toString()+valueUES.substring(0,4)))
                {
                    if ("00" != document.getElementById('txtUSEREXIT_Type').value.Trim())
                    {
                        alert('活動起日錯誤！');
                        document.getElementById('txtUSEREXIT_DateS').focus();
                       
                        return false;
                    }
                }
                
                var valueUEE = document.getElementById('txtUSEREXIT_DateE').value.Trim();
                if (-2 == checkDateSn((parseFloat(valueUEE.substring(4,8))-1911).toString()+valueUEE.substring(0,4)))
                {
                    if ("00" != document.getElementById('txtUSEREXIT_Type').value.Trim())
                    {
                        alert('活動迄日錯誤！');
                        document.getElementById('txtUSEREXIT_DateE').focus();
                       
                        return false;
                    }
                }
                
                var valueOAOS = document.getElementById('txtUSEREXIT_OAOS').value.Trim().substring(0,1);
                if (!("M" == valueOAOS || "W" == valueOAOS || "D" == valueOAOS || "" == valueOAOS))
                {
                    if ("00" != document.getElementById('txtUSEREXIT_Type').value.Trim())
                    {
                        alert('OAOS第一位只能為M/W/D');
                        document.getElementById('txtUSEREXIT_OAOS').focus();
                   
                        return false;
                    }
                }
                
                
                //生日活動
                if (parseInt(document.getElementById('atl_BIRTH1_tbTBBLimit_INT').value.Trim(),10) > 100)
                {
                    alert('折抵上限整數部份不能大於100！');
                    document.getElementById('atl_BIRTH1_tbTBBLimit_INT').focus();
               
                    return false;
                }
                
                if (parseInt(document.getElementById('atl_BIRTH2_tbTBBLimit_INT').value.Trim(),10) > 100)
                {
                    alert('折抵上限整數部份不能大於100！');
                    document.getElementById('atl_BIRTH2_tbTBBLimit_INT').focus();
               
                    return false;
                }
                
                if (parseInt(document.getElementById('atl_BIRTH3_tbTBBLimit_INT').value.Trim(),10) > 100)
                {
                    alert('折抵上限整數部份不能大於100！');
                    document.getElementById('atl_BIRTH3_tbTBBLimit_INT').focus();
               
                    return false;
                }
                
                var valueB = document.getElementById('txtBIRTH_Type').value.Trim();
                if (!("00" == valueB || "01" == valueB || "02" == valueB || "03" == valueB))
                {
                    alert('類型只能輸入00/01/02/03');
                    document.getElementById('txtBIRTH_Type').focus();
               
                    return false;
                }
                
                var valueBS = document.getElementById('txtBIRTH_DateS').value.Trim();
                if (-2 == checkDateSn((parseFloat(valueBS.substring(4,8))-1911).toString()+valueBS.substring(0,4)))
                {
                    if ("00" != document.getElementById('txtBIRTH_Type').value.Trim())
                    {
                        alert('活動起日錯誤！');
                        document.getElementById('txtBIRTH_DateS').focus();
                       
                        return false;
                    }
                }
                
                var valueBE = document.getElementById('txtBIRTH_DateE').value.Trim();
                if (-2 == checkDateSn((parseFloat(valueBE.substring(4,8))-1911).toString()+valueBE.substring(0,4)))
                {
                    if ("00" != document.getElementById('txtBIRTH_Type').value.Trim())
                    {
                        alert('活動迄日錯誤！');
                        document.getElementById('txtBIRTH_DateE').focus();
                       
                        return false;
                    }
                }
            
                if (!checkNull())
                {
                    alert('此欄位為必填項！');
                    
                    return false;
                }
                
                //如果沒有填寫CardType則不允許提交
                if(checkUserControlNull('atl_AT1') && checkUserControlNull('atl_USEREXITAT1') && checkUserControlNull('atl_BIRTH1'))
                {
                    alert('必須選擇卡種明細！');
                    document.getElementById('atl_AT1_cddlATList').focus();
                    
                    return false;
                }
                
                // 長期活動
                if(!checkUserControl('atl_AT1'))
                {
                    return false;
                }
                
                if(!checkUserControl('atl_AT2'))
                {
                    return false;
                }
                
                if(!checkUserControl('atl_AT3'))
                {
                    return false;
                }
                
                // 短期促銷
                if(!checkUserControl('atl_USEREXITAT1'))
                {
                    return false;
                }
                
                if(!checkUserControl('atl_USEREXITAT2'))
                {
                    return false;
                }
                
                if(!checkUserControl('atl_USEREXITAT3'))
                {
                    return false;
                }
                
                // 生日活動
                if(!checkUserControl('atl_BIRTH1'))
                {
                    return false;
                }
                
                if(!checkUserControl('atl_BIRTH2'))
                {
                    return false;
                }
                
                if(!checkUserControl('atl_BIRTH3'))
                {
                    return false;
                }
            }
            
            //*查主機按鈕檢核
            if(2 == intType)
            {
                if ("" == document.getElementById('txtORG').value.Trim())
                {
                    document.getElementById('txtORG').focus();
                    alert('此欄位為必填項');
                    return false;
                }
                
                var iNull = true;
                for (i=1; i<11; i++)
                {
                    if ("" != document.getElementById('txtMERCHANTNO' + i.toString()).value.Trim())
                    {
                        iNull = false;
                        break;
                    }
                }
                if (iNull)
                {
                    document.getElementById('txtMERCHANTNO1').focus();
                    alert('請最少填一項MERCHANT NO');
                    return false;
                }
            }
            
            return true;
        }
    
        function setDisable(strType)
        {
            var value;
            
            if (!("U" == strType.toUpperCase() || "B" == strType.toUpperCase()))
            {
                return;
            }
                    
            if ("U" == strType.toUpperCase())
            {
                value = document.getElementById('txtUSEREXIT_Type').value.Trim();
            }
            
            if ("B" == strType.toUpperCase())
            {
                value = document.getElementById('txtBIRTH_Type').value.Trim();
            }
            
            var objinput = document.getElementsByTagName("INPUT");
            var len = objinput.length;
            
            for (i=0; i<len; i++)
            {
                if ("text" == objinput[i].type)
                {
                    if ("U" == strType.toUpperCase())
                    {
                        if ("atl_USEREXITAT" == objinput[i].id.substring(0,14))
                        {
                            if ("00" == value)
                            {
                                objinput[i].disabled = true;
                                objinput[i].value = "";
                            }
                            else
                            {
                                objinput[i].disabled = false;
                            }
                        }
                    }
                    
                    if ("B" == strType.toUpperCase())
                    {
                        if ("atl_BIRTH" == objinput[i].id.substring(0,9))
                        {
                            if ("00" == value)
                            {
                                objinput[i].disabled = true;
                                objinput[i].value = "";
                            }
                            else
                            {
                                objinput[i].disabled = false;
                            }
                        }
                    }
                    
                }
            }
            
            if ("U" == strType.toUpperCase())
            {
                if ("00" == value)
                {
                    document.getElementById('txtUSEREXIT_DateS').disabled = true;
                    document.getElementById('txtUSEREXIT_DateE').disabled = true;
                    document.getElementById('txtUSEREXIT_OAOS').disabled = true;
                    document.getElementById('atl_USEREXITAT1_cddlATList').disabled = true;
                    document.getElementById('atl_USEREXITAT2_cddlATList').disabled = true;
                    document.getElementById('atl_USEREXITAT3_cddlATList').disabled = true;
                    
                    document.getElementById('txtUSEREXIT_DateS').value = "";
                    document.getElementById('txtUSEREXIT_DateE').value = "";
                    document.getElementById('txtUSEREXIT_OAOS').value = "";
                    document.getElementById('atl_USEREXITAT1_cddlATList').options[0].selected = true;
                    document.getElementById('atl_USEREXITAT2_cddlATList').options[0].selected = true;
                    document.getElementById('atl_USEREXITAT3_cddlATList').options[0].selected = true;
                }
                else
                {
                    document.getElementById('txtUSEREXIT_DateS').disabled = false;
                    document.getElementById('txtUSEREXIT_DateE').disabled = false;
                    document.getElementById('txtUSEREXIT_OAOS').disabled = false;
                    document.getElementById('atl_USEREXITAT1_cddlATList').disabled = false;
                    document.getElementById('atl_USEREXITAT2_cddlATList').disabled = false;
                    document.getElementById('atl_USEREXITAT3_cddlATList').disabled = false;
                    document.getElementById('atl_USEREXITAT1_tbTBBLimit_DEC').value = "00";
                    document.getElementById('atl_USEREXITAT2_tbTBBLimit_DEC').value = "00";
                    document.getElementById('atl_USEREXITAT3_tbTBBLimit_DEC').value = "00";
                    //document.getElementById('txtUSEREXIT_OAOS').value = "000000";
                }
            }
            
            if ("B" == strType.toUpperCase())
            {
                if ("00" == value)
                {
                    document.getElementById('txtBIRTH_DateS').disabled = true;
                    document.getElementById('txtBIRTH_DateE').disabled = true;
                    document.getElementById('atl_BIRTH1_cddlATList').disabled = true;
                    document.getElementById('atl_BIRTH2_cddlATList').disabled = true;
                    document.getElementById('atl_BIRTH3_cddlATList').disabled = true;
                    
                    document.getElementById('txtBIRTH_DateS').value = "";
                    document.getElementById('txtBIRTH_DateE').value = "";
                    document.getElementById('atl_BIRTH1_cddlATList').options[0].selected = true;
                    document.getElementById('atl_BIRTH2_cddlATList').options[0].selected = true;
                    document.getElementById('atl_BIRTH3_cddlATList').options[0].selected = true;
                }
                else
                {
                    document.getElementById('txtBIRTH_DateS').disabled = false;
                    document.getElementById('txtBIRTH_DateE').disabled = false;
                    document.getElementById('atl_BIRTH1_cddlATList').disabled = false;
                    document.getElementById('atl_BIRTH2_cddlATList').disabled = false;
                    document.getElementById('atl_BIRTH3_cddlATList').disabled = false;
                    document.getElementById('atl_BIRTH1_tbTBBLimit_DEC').value = "00";
                    document.getElementById('atl_BIRTH2_tbTBBLimit_DEC').value = "00";
                    document.getElementById('atl_BIRTH3_tbTBBLimit_DEC').value = "00";                      
                }
            }
        }
        
        function checkNull()
        {
            var objinput = document.getElementsByTagName("INPUT");
            var len = objinput.length;
            
            for (i=0; i<len; i++)
            {
                if ("text" == objinput[i].type && !objinput[i].disabled)
                {
                    if (!("atl_USEREXITAT" == objinput[i].id.substring(0,14) || "atl_BIRTH" == objinput[i].id.substring(0,9) || "atl_AT" == objinput[i].id.substring(0,6) || "txtUSEREXIT_OAOS" == objinput[i].id || "txtMERCHANTNO" == objinput[i].id.substring(0,13)))
                    {
                        if ("" == objinput[i].value.Trim())
                        {
                            objinput[i].focus();
                           
                            return false;
                        }
                    }
                }
            }
            
            return true;
        }
        
        function checkUserControl(idTitle)
        {
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
            
            if (!isNull)
            {
                if ("" == document.getElementById(idTitle + '_tbTBBLimit_INT').value.Trim())
                {
                    document.getElementById(idTitle + '_tbTBBLimit_INT').focus();
                    alert('已設定卡種類別，此欄位為必填項！');
                    return false;
                }
                
                if ("" == document.getElementById(idTitle + '_tbTBBLimit_DEC').value.Trim())
                {
                    document.getElementById(idTitle + '_tbTBBLimit_DEC').focus();
                    alert('已設定卡種類別，此欄位為必填項！');
                    return false;
                }
                
                if ("" == document.getElementById(idTitle + '_tbTBBPoints').value.Trim())
                {
                    document.getElementById(idTitle + '_tbTBBPoints').focus();
                    alert('已設定卡種類別，此欄位為必填項！');
                    return false;
                }
                
                if ("" == document.getElementById(idTitle + '_tbTBBAmount').value.Trim())
                {
                    document.getElementById(idTitle + '_tbTBBAmount').focus();
                    alert('已設定卡種類別，此欄位為必填項！');
                    return false;
                }
            }
            return true;
        }
        
        function checkUserControlNull(idTitle)
        {
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
            
            return isNull;
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
                                        SetOmit="False" StickHeight="False" Width="200px" IsColon="False" ShowID="01_01050101_001"></cc1:CustLabel></li>
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
                                    Width="40px" OnClick="btnSelect_Click" OnClientClick="return checkInputText('tabNo1',0);"
                                    onkeydown="setfocuschoice('txtReceiveNumber','txtORG');" ShowID="01_01050101_040" /></td>
                        </tr>
                    </table>
                    <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                        <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table1" style="">
                            <tr class="itemTitle">
                                <td colspan="3">
                                    <cc1:CustLabel ID="lbTitle1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_002" StickHeight="False"
                                        Font-Bold="True"></cc1:CustLabel></td>
                                <td colspan="3" align="center">
                                    <cc1:CustLabel ID="lbUSER" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_012" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustLabel ID="lbUSERValue" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False" ShowID=""></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td colspan="1" align="right">
                                    <cc1:CustLabel ID="lbORG" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_003" StickHeight="False"
                                        ForeColor="DodgerBlue"></cc1:CustLabel>
                                </td>
                                <td colspan="5">
                                    <cc1:CustTextBox ID="txtORG" runat="server" MaxLength="3" checktype="num"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td align="right">
                                    <cc1:CustLabel ID="lbMERCHANTNO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_004" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td>
                                    <cc1:CustTextBox ID="txtMERCHANTNO1" runat="server" MaxLength="9" checktype="num"
                                        Width="100px"></cc1:CustTextBox>
                                </td>
                                <td>
                                    <cc1:CustTextBox ID="txtMERCHANTNO2" runat="server" MaxLength="9" checktype="num"
                                        Width="100px"></cc1:CustTextBox>
                                </td>
                                <td>
                                    <cc1:CustTextBox ID="txtMERCHANTNO3" runat="server" MaxLength="9" checktype="num"
                                        Width="100px"></cc1:CustTextBox>
                                </td>
                                <td>
                                    <cc1:CustTextBox ID="txtMERCHANTNO4" runat="server" MaxLength="9" checktype="num"
                                        Width="100px"></cc1:CustTextBox>
                                </td>
                                <td>
                                    <cc1:CustTextBox ID="txtMERCHANTNO5" runat="server" MaxLength="9" checktype="num"
                                        Width="100px"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td>
                                </td>
                                <td>
                                    <cc1:CustTextBox ID="txtMERCHANTNO6" runat="server" MaxLength="9" checktype="num"
                                        Width="100px"></cc1:CustTextBox>
                                </td>
                                <td>
                                    <cc1:CustTextBox ID="txtMERCHANTNO7" runat="server" MaxLength="9" checktype="num"
                                        Width="100px"></cc1:CustTextBox>
                                </td>
                                <td>
                                    <cc1:CustTextBox ID="txtMERCHANTNO8" runat="server" MaxLength="9" checktype="num"
                                        Width="100px"></cc1:CustTextBox>
                                </td>
                                <td>
                                    <cc1:CustTextBox ID="txtMERCHANTNO9" runat="server" MaxLength="9" checktype="num"
                                        Width="100px"></cc1:CustTextBox>
                                </td>
                                <td>
                                    <cc1:CustTextBox ID="txtMERCHANTNO10" runat="server" MaxLength="9" checktype="num"
                                        Width="100px"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td colspan="6">
                                    <cc1:CustLabel ID="lbMERCHANTNOExcel" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050101_005"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="itemTitle">
                                <td colspan="6" align="center">
                                    <cc1:CustButton ID="btnSelectMF" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                        Width="60px" OnClick="btnSelectMF_Click" OnClientClick="return checkInputText('pnlText',2);"
                                        ShowID="01_01050101_042" />
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td align="right">
                                    <cc1:CustLabel ID="lbPRODCODE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_006" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td>
                                    <cc1:CustTextBox ID="txtPRODCODE" runat="server" MaxLength="2" checktype="num" Width="60px"></cc1:CustTextBox>
                                </td>
                                <td colspan="4">
                                    <cc1:CustLabel ID="lbPRODCODE_MEMO" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050101_013"
                                        StickHeight="False" ForeColor="Tomato"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td align="right">
                                    <cc1:CustLabel ID="lbPROGRAM" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_014" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td colspan="5">
                                    <cc1:CustTextBox ID="txtPROGRAM" runat="server" MaxLength="5" checktype="num" Width="80px"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td>
                                </td>
                                <td colspan="2">
                                    <cc1:CustLabel ID="lbPROGRAM_MEMO1" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050101_007"
                                        StickHeight="False" ForeColor="DodgerBlue"></cc1:CustLabel>
                                </td>
                                <td colspan="3">
                                    <cc1:CustDropDownList ID="ddlPROGRAM" runat="server" OnSelectedIndexChanged="ddlPROGRAM_SelectedIndexChanged"
                                        AutoPostBack="True">
                                    </cc1:CustDropDownList>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td>
                                </td>
                                <td colspan="5">
                                    <cc1:CustLabel ID="lbPROGRAM_MEMO2" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050101_008"
                                        StickHeight="False" ForeColor="Tomato"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td align="right">
                                    <cc1:CustLabel ID="lbMERCHANT" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_009" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td colspan="5">
                                    <cc1:CustTextBox ID="txtMERCHANT_INT" runat="server" MaxLength="3" checktype="num"
                                        Width="40px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbDECPoint" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_010" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtMERCHANT_DEC" runat="server" MaxLength="2" checktype="num"
                                        Width="40px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbPercentSign" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_011" StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                        </table>
                        <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table2" style="">
                            <tr class="itemTitle">
                                <td>
                                    <cc1:CustLabel ID="lbTitle2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_015" StickHeight="False"
                                        Font-Bold="True"></cc1:CustLabel>
                                    <cc1:CustLabel ID="lbTitle2MEMO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_016" StickHeight="False"
                                        ForeColor="Tomato"></cc1:CustLabel>
                                </td>
                            </tr>
                        </table>
                        <uc1:CustATypeList ID="atl_AT1" runat="server" FirstRowClass="trOdd" IsAType1="true"
                            ATypeShowID="01_01050101_023" RTBCount="16"></uc1:CustATypeList>
                        <uc1:CustATypeList ID="atl_AT2" runat="server" FirstRowClass="trOdd" IsAType1="false"
                            ATypeShowID="01_01050101_024" RTBCount="16"></uc1:CustATypeList>
                        <uc1:CustATypeList ID="atl_AT3" runat="server" FirstRowClass="trEven" IsAType1="false"
                            ATypeShowID="01_01050101_025" RTBCount="16"></uc1:CustATypeList>
                        <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table3" style="">
                            <tr class="itemTitle">
                                <td colspan="3">
                                    <cc1:CustLabel ID="lbTitle3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_026" StickHeight="False"
                                        Font-Bold="True"></cc1:CustLabel>
                                    <cc1:CustLabel ID="lbTitle3MEMO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_027" StickHeight="False"
                                        ForeColor="Tomato"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td colspan="3">
                                    <cc1:CustLabel ID="lbUSEREXIT" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_028" StickHeight="False"
                                        ForeColor="Fuchsia"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td colspan="2">
                                    <cc1:CustLabel ID="lbUSEREXIT_Type" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050101_029"
                                        StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustLabel ID="lbUSEREXIT_TypeMEMO" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050101_030"
                                        StickHeight="False" ForeColor="Tomato"></cc1:CustLabel>
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td align="right" style="width: 15%">
                                    <cc1:CustTextBox ID="txtUSEREXIT_Type" runat="server" MaxLength="2" checktype="num"
                                        Width="35px" onkeyup="setDisable('U');"></cc1:CustTextBox>
                                </td>
                                <td colspan="2">
                                    <cc1:CustLabel ID="lbUSEREXIT_Type00" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050101_031"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td>
                                </td>
                                <td colspan="2">
                                    <cc1:CustLabel ID="lbUSEREXIT_Type01" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050101_032"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td>
                                </td>
                                <td colspan="2">
                                    <cc1:CustLabel ID="lbUSEREXIT_Type02" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050101_033"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td>
                                </td>
                                <td colspan="2">
                                    <cc1:CustLabel ID="lbUSEREXIT_Type03" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050101_034"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td align="right">
                                    <cc1:CustLabel ID="lbUSEREXIT_Date" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050101_035"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td colspan="2">
                                    <cc1:CustTextBox ID="txtUSEREXIT_DateS" runat="server" MaxLength="8" checktype="num"
                                        Width="80px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbUSEREXIT_Date1" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050101_036"
                                        StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtUSEREXIT_DateE" runat="server" MaxLength="8" checktype="num"
                                        Width="80px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbUSEREXIT_Date2" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050101_037"
                                        StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                        </table>
                        <uc1:CustATypeList ID="atl_USEREXITAT1" runat="server" FirstRowClass="trEven" IsAType1="true"
                            ATypeShowID="01_01050101_023" RTBCount="16"></uc1:CustATypeList>
                        <uc1:CustATypeList ID="atl_USEREXITAT2" runat="server" FirstRowClass="trEven" IsAType1="false"
                            ATypeShowID="01_01050101_024" RTBCount="16"></uc1:CustATypeList>
                        <uc1:CustATypeList ID="atl_USEREXITAT3" runat="server" FirstRowClass="trOdd" IsAType1="false"
                            ATypeShowID="01_01050101_025" RTBCount="16"></uc1:CustATypeList>
                        <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table4" style="">
                            <tr class="trEven">
                                <td colspan="3">
                                    <cc1:CustLabel ID="lbUSEREXIT_OAOS" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050101_039"
                                        StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtUSEREXIT_OAOS" runat="server" checktype="numandletter" MaxLength="6"
                                        Width="60px"></cc1:CustTextBox>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td colspan="3">
                                    <cc1:CustLabel ID="lbBIRTH" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_038" StickHeight="False"
                                        ForeColor="Fuchsia"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td colspan="3">
                                    <cc1:CustLabel ID="lbBIRTH_Type" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_029" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustLabel ID="lbBIRTH_TypeMEMO" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01050101_030"
                                        StickHeight="False" ForeColor="Tomato"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td align="right" style="width: 15%">
                                    <cc1:CustTextBox ID="txtBIRTH_Type" runat="server" MaxLength="2" checktype="num"
                                        Width="35px" onkeyup="setDisable('B');"></cc1:CustTextBox>
                                </td>
                                <td colspan="2">
                                    <cc1:CustLabel ID="lbBIRTH_Type00" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_031" StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td>
                                </td>
                                <td colspan="2">
                                    <cc1:CustLabel ID="lbBIRTH_Type01" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_032" StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td>
                                </td>
                                <td colspan="2">
                                    <cc1:CustLabel ID="lbBIRTH_Type02" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_033" StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trEven">
                                <td>
                                </td>
                                <td colspan="2">
                                    <cc1:CustLabel ID="lbBIRTH_Type03" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_034" StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                            <tr class="trOdd">
                                <td align="right">
                                    <cc1:CustLabel ID="lbBIRTH_Date" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_035" StickHeight="False"></cc1:CustLabel>
                                </td>
                                <td colspan="2">
                                    <cc1:CustTextBox ID="txtBIRTH_DateS" runat="server" MaxLength="8" checktype="num"
                                        Width="80px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbBIRTH_Date1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_036" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtBIRTH_DateE" runat="server" MaxLength="8" checktype="num"
                                        Width="80px"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="lbBIRTH_Date2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01050101_037" StickHeight="False"></cc1:CustLabel>
                                </td>
                            </tr>
                        </table>
                        <uc1:CustATypeList ID="atl_BIRTH1" runat="server" FirstRowClass="trEven" IsAType1="true"
                            ATypeShowID="01_01050101_023" RTBCount="16"></uc1:CustATypeList>
                        <uc1:CustATypeList ID="atl_BIRTH2" runat="server" FirstRowClass="trEven" IsAType1="false"
                            ATypeShowID="01_01050101_024" RTBCount="16"></uc1:CustATypeList>
                        <uc1:CustATypeList ID="atl_BIRTH3" runat="server" FirstRowClass="trOdd" IsAType1="false"
                            ATypeShowID="01_01050101_025" RTBCount="16"></uc1:CustATypeList>
                        <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table5" style="">
                            <tr class="itemTitle">
                                <td align="center">
                                    <cc1:CustButton ID="btnAdd" runat="server" CssClass="smallButton" DisabledWhenSubmit="False"
                                        Width="40px" OnClick="btnAdd_Click" ShowID="01_01050101_041" OnClientClick="return checkInputText('pnlText', 1);" /></td>
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
