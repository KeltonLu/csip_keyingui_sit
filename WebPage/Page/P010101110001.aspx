<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010101110001.aspx.cs" Inherits="P010101110001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <%-- 2020/11/19_Ares_Stanley-修正格式; 2020/12/11_Ares_Stanley-修正格式 --%>
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript" language="javascript">        
        function checkInputText(id,intType)
        {                        
            //*檢核輸入欄位【收件編號】是否為空 
            if(document.getElementById('txtReceiveNumber').value.Trim() == "")
            {

                document.getElementById('txtReceiveNumber').focus();
                alert('請輸入收件編號');
                setControlsDisabled('tabNo2');
                document.getElementById('lblUserNameText').innerText = "";
                
                return false;
            }
            
            if (checkDateSn(document.getElementById("txtReceiveNumber").value.Trim().substring(0,8))==-2)
            {
                alert("收件編號格式不對！");
                document.getElementById('txtReceiveNumber').focus();
                setControlsDisabled('tabNo2');
                document.getElementById('lblUserNameText').innerText = "";
                return false;
            }
            
            //*檢核輸入欄位【身分證號碼】是否為空
             if(document.getElementById('txtUserId').value.Trim()== "")
            {
                alert('請輸入身分證號碼');
                document.getElementById('txtUserId').focus();
                setControlsDisabled('tabNo2');
                document.getElementById('lblUserNameText').innerText = "";
                
                return false;
            }
            
            //*檢核收件編號是否輸入正確
            if(document.getElementById('txtReceiveNumber').value.Trim().length!=13)
            {
                alert('收件編號格式不對！');
                document.getElementById('txtReceiveNumber').focus();
                setControlsDisabled('tabNo2');
                document.getElementById('lblUserNameText').innerText = "";
               
                return false;
               
            }
            if(!checkInputType(id))
            {
                return false;
            } 
            
            //*提交按鈕
            if(intType == 1)
            {  
                if(!confirm('請確認是否要提交資料'))
                {
                    return false;
                }
                return true;
            }
            else
            {
                //20160606 (U) by Tank
                var ID = $('#txtUserId').val();
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    async: false,
                    url: "P010101110001.aspx/funGetDataCnt",
                    data: "{'KeyValue':'" + ID + "'}",
                    dataType: "json",
                    success: function (data) 
                    {
                        if (data > 0)
                        {
                            if(!confirm('此ID有再途案件申請中，是否繼續KeyIn？'))
                            {
                                return false;
                            }
                            else 
                            {
                                $('#btnSelect2').click();
                                return true;
                            }
                        }
                        else 
                        {
                            $('#btnSelect2').click();
                            return true;
                        }
                    },
                    error: function (result) 
                    {
                        alert("funGetDataCnt Error");
                        return false;
                    }                  
                });
                return false;
            }
        }
        
    </script>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />
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
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" style="" id="tabNo1">
                        <tr class="itemTitle">
                            <td colspan="4">
                                <li>
                                    <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" Width="200px" IsColon="False" ShowID="01_01110001_001"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblReceiveNumber" runat="server" CurSymbol="£" IsColon="True"
                                    ShowID="01_01110001_002"></cc1:CustLabel></td>
                            <td style="width: 85%" colspan="3">
                                <cc1:CustTextBox ID="txtReceiveNumber" runat="server" MaxLength="13" checktype="num"
                                    BoxName="收件編號"></cc1:CustTextBox><cc1:CustTextBox ID="txtReceiveNumberH" runat="server"
                                        MaxLength="13" checktype="num" CssClass="btnHiden" Text=""></cc1:CustTextBox></td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblUserId" runat="server" CurSymbol="£" IsColon="True" ShowID="01_01110001_003"></cc1:CustLabel></td>
                            <td style="width: 35%">
                                <cc1:CustTextBox ID="txtUserId" runat="server" MaxLength="20" checktype="ID" BoxName="身分證號碼"></cc1:CustTextBox><cc1:CustTextBox
                                    ID="txtUserIdH" runat="server" MaxLength="20" checktype="numandletter" CssClass="btnHiden"
                                    Text=""></cc1:CustTextBox>&nbsp;&nbsp;&nbsp;&nbsp;
                                <cc1:CustButton ID="btnSelect" CssClass="smallButton" runat="server" Width="40px"
                                    OnClientClick="return checkInputText('tabNo1',0);"
                                    DisabledWhenSubmit="False" ShowID="01_01110001_004" />
                                <%--20160606 (U) by Tank--%>
                                <asp:Button runat="server" ID="btnSelect2" OnClick="btnSelect_Click" style="display:none"/>        
                            </td>
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblUserName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01110001_005" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 35%">
                                <cc1:CustLabel ID="lblUserNameText" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="" StickHeight="False"></cc1:CustLabel></td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table2">
                        <tr>
                            <td align="right" colspan="4" style="height: 1px">
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo2" style="">
                        <tr class="itemTitle">
                            <td colspan="6">
                                <li>
                                    <cc1:CustLabel ID="lblAuto" runat="server" CurSymbol="£" Width="200px" ShowID="01_01110001_006"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="height: 25px; width: 1%">
                                <cc1:CustLabel ID="lblAccNoBank" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01110001_007" StickHeight="False"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 1%">
                                <cc1:CustLabel ID="lblAccNoBankText" runat="server" CurSymbol="£"></cc1:CustLabel></td>
                            <cc1:CustDropDownList ID="dropAccNo" kind="select" runat="server" Style="left: 0px;
                                top: 0px; clip: rect(0px auto auto 130px); position: absolute; width: 150px;"
                                Visible="false">
                            </cc1:CustDropDownList>
                            <td style="height: 25px; width: 1%">
                                <cc1:CustLabel ID="lbPayWay" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01110001_008" StickHeight="False"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 1%">
                                <cc1:CustLabel ID="lbPayWayText" runat="server" CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 1%">
                                <cc1:CustLabel ID="lblIsAuto" runat="server" CurSymbol="£" IsColon="True" ShowID="01_01110001_009"
                                    ForeColor="red"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 2%">
                                <cc1:CustRadioButton ID="radAuto" runat="Server" CurAlign="left" Checked="true" GroupName="IsAuto"
                                    OnCheckedChanged="radEnd_CheckedChanged" AutoPostBack="True" />
                                <cc1:CustRadioButton ID="radEnd" runat="Server" CurAlign="left" GroupName="IsAuto"
                                    OnCheckedChanged="radEnd_CheckedChanged" AutoPostBack="True" />
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="height: 25px; width: 1%">
                                <cc1:CustLabel ID="lblDDID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01110001_016" StickHeight="False"></cc1:CustLabel></td>
                            <td style="height: 25px; width: 1%">
                                <cc1:CustLabel ID="lblDDIDText" runat="server" CurSymbol="£"></cc1:CustLabel></td>
                            <td style="height: 25px;" colspan="4">
                            </td>
                        </tr>
                        <tr class="itemTitle">
                            <td colspan="6" style="height: 25px">
                                <li>
                                    <cc1:CustLabel ID="lblIntroduce" runat="server" CurSymbol="£" Width="200px" ShowID="01_01110001_012"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="height: 25px">
                                <cc1:CustLabel ID="lblPopul" runat="server" CurSymbol="£" IsColon="True" ShowID="01_01110001_013"></cc1:CustLabel></td>
                            <td style="height: 25px">
                                <cc1:CustTextBox ID="txtPopul" runat="server" checktype="num" MaxLength="5"></cc1:CustTextBox></td>
                            <td style="height: 25px">
                                <cc1:CustLabel ID="lblPopulNumber" runat="server" CurSymbol="£" IsColon="True" ShowID="01_01110001_014"></cc1:CustLabel></td>
                            <td style="height: 25px" colspan="3">
                                <cc1:CustTextBox ID="txtPopulNumber" runat="server" checktype="num" MaxLength="8"></cc1:CustTextBox></td>
                        </tr>
                        <tr class="trOdd">
                            <td align="left">
                                <cc1:CustLabel ID="lbCaseClass" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01010900_017" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="5">
                                <div style="position: relative">
                                    <cc1:CustDropDownList ID="dropCaseClass" kind="select" runat="server" Style="left: 0px;
                                        top: 0px; clip: rect(0px auto auto 130px); position: absolute; width: 150px;"
                                        onclick="simOptionClick4IE('txtCaseClass');" Enabled="false">
                                    </cc1:CustDropDownList>
                                    <cc1:CustTextBox ID="txtCaseClass" runat="server" MaxLength="50" checktype="" onkeydown="entersubmit('btnSubmit');"
                                        Style="left: 0px; top: 0px; position: relative; width: 125px; height: 11px;"
                                        AutoPostBack="False" BoxName="案件類別" onfocus="allselect(this);" Enabled="false"></cc1:CustTextBox>
                                    <cc1:CustTextBox ID="txtbCaseClassH" runat="server" MaxLength="30" CssClass="btnHiden"
                                        Text=""></cc1:CustTextBox>
                                </div>
                            </td>
                        </tr>
                        <tr class="itemTitle">
                            <td colspan="6" align="center">
                                <cc1:CustButton ID="btnSubmit" CssClass="smallButton" Width="40px" runat="server"
                                    OnClientClick="return checkInputText('tabNo2',1);" OnClick="btnSubmit_Click"
                                    DisabledWhenSubmit="False" ShowID="01_01110001_015" Enabled="False" /></td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
            <br />
        </div>
    </form>
</body>
</html>
