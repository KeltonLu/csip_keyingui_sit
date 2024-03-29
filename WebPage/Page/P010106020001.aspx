﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010106020001.aspx.cs" Inherits="Page_P010106020001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%--修改紀錄:2021/02/01,03_Ares_Stanley-內頁建檔-資料未填滿時按ENTER觸發TAB鍵(跳至下一個項目); 2021/03/08_Ares_Stanley-調整按Enter觸發TAB--%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript" language="javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript">             
        
       function checkInputText()        
       {
            var errMsg = "";
       
         //*檢核輸入欄位【編批日期】是否為空 
            if(document.getElementById('txtShop_ID').value.Trim() == "")
            {
                errMsg += "請輸入商店代號!!\r\n";
                $("#txtShop_ID").focus();
            }
            
             if(document.getElementById('txtBatch_NO').value.Trim() == "")
            {
                errMsg += "請輸入批號!!\r\n";
                $("#txtBatch_NO").focus();
            }
            
             if(document.getElementById('txtReceive_Batch').value.Trim() == "")
            {
                errMsg += "請輸入收件批次!!\r\n";
                $("#txtReceive_Batch").focus();
            }
             
            if(document.getElementById('txtBatch_Date').value.Trim() == "")
            {
                errMsg += "請輸入編批日期!!\r\n";
                $("#txtBatch_Date").focus();
            }
                                    
            if (errMsg != "")
            {
                alert(errMsg);
                return false;
            }
       }
              
      function CardNo(no)
      {
        switch (no)
        {
            // 20220505 調整輸入信用卡號欄位，由4欄合併為1欄 By Kelton 
            //case 1:                
            //    //if ($("#txtCard_No1").val().length == 4){
            //    if(document.getElementById('txtCard_No1').value.length == 4){
            //        //$("#txtCard_No2").focus();
            //        document.getElementById('txtCard_No2').focus();
            //    }
            //    break;
            //case 2:
            //    //if ($("#txtCard_No2").val().length == 4){
            //    if(document.getElementById('txtCard_No2').value.length == 4){
            //        //$("#txtCard_No3").focus();
            //        document.getElementById('txtCard_No3').focus();
            //    }
            //    break;
            //case 3:
            //    //if ($("#txtCard_No3").val().length == 4){
            //    if(document.getElementById('txtCard_No3').value.length == 4){
            //        //$("#txtCard_No4").focus();
            //        document.getElementById('txtCard_No4').focus();
            //    }
            //    break;
            //case 4:
            case 1:
                //if ($("#txtCard_No4").val().length == 4){
                // 20220505 調整輸入信用卡號欄位，由4欄合併為1欄 By Kelton
                //if(document.getElementById('txtCard_No4').value.length == 4){
                if (document.getElementById('txtCard_No1').value.length == 16) {
                
                    //20160526 (U) by Tank, 新增卡號檢核
                    // 20220505 調整輸入信用卡號欄位，由4欄合併為1欄 By Kelton
                    //var card_no = $("#txtCard_No1").val()+$("#txtCard_No2").val()+$("#txtCard_No3").val()+$("#txtCard_No4").val();
                    var card_no = $("#txtCard_No1").val();
                    var card_no16 = card_no.substr(15,1);                    
                    var sumNum=0,card_no_temp,Num;
                    for (i=0;i<15;i++)
                    {
                        card_no_temp = card_no.substr(i,1);
                        if (i%2 == 0)
                        {
                            Num = card_no_temp * 2;
                        }
                        else
                        {
                            Num = card_no_temp * 1;
                        }
                        if (Num>9)
                        {
                            Num = Num-9;
                        }
                        sumNum = sumNum + Num;
                    }                    
                    //alert("sumNum="+sumNum);
                    
                   //var checkDigit = 10-(sumNum%10);
                    var checkDigit = sumNum%10;
                    
                    if (checkDigit > 0)
                    {
                        checkDigit = 10-(checkDigit);
                    }
                    
                    //alert("checkDigit="+checkDigit);
                    
                    if (checkDigit == card_no16){
                        $("#txt_dpTran_Date").focus();
                    }
                    else 
                    {
                        alert("卡號檢核錯誤，請確認卡號是否正確！");
//                        $("#txtCard_No1").val("");
//                        $("#txtCard_No1").focus();
//                        $("#txtCard_No2").val("");
//                        $("#txtCard_No3").val("");
//                        $("#txtCard_No4").val("");
                    }                    
                }
                break;
            case 5:
                if ($("#txtAuth_Code").val().length == 6){
                    $("#txtP1").focus();
                }
                break;
            case 6:
                if ($("#txtAMT").val().substring(0,1) == "-"){
                    alert("金額不可輸入負號!");
                    $("#txtAMT").val("");
                }
                break;
            //20160526 (U) by Tank, add txtP1 event
            case 7:
                if ($("#txtP1").val().length == 2)
                {
                    if ($("#txtP1").val() == "40")
                    {
                        $('#ddlP1 option:eq(0)').attr('selected', true);
                    }
                    else if ($("#txtP1").val() == "41")
                    {
                        $('#ddlP1 option:eq(1)').attr('selected', true);
                    }
                    else
                    {
                        alert("請Key40或41");
                        $("#txtP1").val("");
                        $("#txtP1").focus();
                        break;
                    }
                    $("#txtAMT").focus();
                }
                break;
            case 8:
                //if ($("#txt_dpTran_Date").val().length == 8)
                if (document.getElementById('txt_dpTran_Date').value.length == 8)
                {
                    //$("#txtAuth_Code").focus();
                    document.getElementById('txtAuth_Code').focus();
                }
                break;
        }
      }
     
     function AuthCode()
     {
         if ($("#ddlP1").val() == "40"){
            if ($("#txtAuth_Code").val() == "00000Y"){
                $("#txtAuth_Code").val("");
            }
            $("#txtAuth_Code").attr("disabled",false);
         }
         else{
            if ($("#txtAuth_Code").val().length == 0){
                $("#txtAuth_Code").val("00000Y");
                $("#txtAuth_Code").attr("disabled",true);
            }
            else{
                if ($("#txtAuth_Code").val().length < 6){
                    $("#txtAuth_Code").focus();
                    alert("授權號碼規則不符");
                    return;
                }
            }
         }         
        }
        function CardNo_Press(no) {
            switch (no) {
                // 20220505 調整輸入信用卡號欄位，由4欄合併為1欄 By Kelton 
                //case 1:
                //    if (document.getElementById('txtCard_No1').value.length < 4) {
                //        if (event.keyCode == 13) {
                //            document.getElementById('txtCard_No2').focus();
                //        }
                //    }
                //    break;
                //case 2:
                //    if (document.getElementById('txtCard_No2').value.length < 4) {
                //        if (event.keyCode == 13) {
                //            document.getElementById('txtCard_No3').focus();
                //        }
                //    }
                //    break;
                //case 3:
                //    if (document.getElementById('txtCard_No3').value.length < 4) {
                //        if (event.keyCode == 13) {
                //            document.getElementById('txtCard_No4').focus();
                //        }
                //    }
                //    break;
                //case 4:
                case 1:
                    // 20220505 調整輸入信用卡號欄位，由4欄合併為1欄 By Kelton
                    //if (document.getElementById('txtCard_No4').value.length < 4) {
                    if (document.getElementById('txtCard_No1').value.length < 16) {
                        if (event.keyCode == 13) {
                            document.getElementById('txt_dpTran_Date').focus();
                            event.preventDefault();
                        }
                    }
                case 8:
                    if (document.getElementById("txt_dpTran_Date").value.length < 8) {
                        $("#txt_dpTran_Date").keydown(function (e) {
                            if (e.keyCode == 13) {
                                $("#txtAuth_Code").focus();
                                event.preventDefault();
                            }
                        });
                    }
                    break;
                case 5:
                    if (document.getElementById('txtAuth_Code').value.length <= 6) {
                        if (event.keyCode == 13) {
                            $("#txtP1").focus();
                            event.preventDefault();
                        }
                    }
                    break;
                case 7:
                    if (document.getElementById('txtP1').value.length <= 2) {
                        if (event.keyCode == 13) {
                            $("#txtAMT").focus();
                        }
                    }
                    break;
                case 6:
                    if (event.keyCode == 13) {
                        $("#btnConfirm").focus();
                        event.preventDefault();
                    }
                    break;
                case 9:
                    if (event.keyCode == 13) {
                        $("#txtAMT_1").focus();
                        event.preventDefault();
                    }
                    break;
                case 10:
                    if (event.keyCode == 13) {
                        $("#ddlReject_Reason").focus();
                        event.preventDefault();
                    }
                    break;
                case 11:
                    if (event.keyCode == 13) {
                        $("#txtCard_No_1").focus();
                        event.preventDefault();
                    }
                    break;
                case 12:
                    if (event.keyCode == 13) {
                        $("#btnReject").focus();
                        event.preventDefault();
                    }
                    break;
            }
        }
    </script>

</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <cust:image runat="server" ID="image1" />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <Triggers>
                <asp:PostBackTrigger ControlID="CustButton3" />
                <asp:PostBackTrigger ControlID="btnDubleConfirm" />
                <asp:PostBackTrigger ControlID="BtnEditSave" />
            </Triggers>
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                    <tr class="itemTitle">
                        <td colspan="8">
                            <li>
                                <cc1:CustLabel ID="CustLabel46" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="200px" IsColon="False" ShowID="01_01060200_001"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="width: 15%; height: 25px;" align="right">
                            <cc1:CustLabel ID="lblBatch_Date" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01060200_008" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 20%; height: 25px;">
                            <cc1:CustTextBox ID="txtBatch_Date" runat="server" TabIndex="1" MaxLength="8" InputType="int"></cc1:CustTextBox>
                            <%--<cc1:DatePicker ID="dtpBatch_Date" Text="" MaxLength="10" runat="server" Enable="True">
                            </cc1:DatePicker>--%>
                        </td>
                        <!-- 收件批次  -->
                        <td style="width: 15%">
                            <cc1:CustLabel ID="lblReceive_Batch" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01060200_011"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 20%">
                            <cc1:CustTextBox ID="txtReceive_Batch" MaxLength="1" runat="server" TabIndex="5"></cc1:CustTextBox>
                        </td>
                        <td style="width: 30%" colspan="2" rowspan="3">
                            <cc1:CustButton ID="CustButton2" runat="server" DisabledWhenSubmit="false" CssClass="smallButton"
                                ShowID="01_01060200_014" OnClick="CustButton2_Click" OnClientClick="return checkInputText();"
                                TabIndex="20" Font-Bold="True" Font-Size="X-Large" />
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td style="width: 15%" align="right">
                            <cc1:CustLabel ID="lblBatch_NO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01060200_010" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 20%">
                            <cc1:CustTextBox ID="txtBatch_NO" MaxLength="3" runat="server" TabIndex="10"></cc1:CustTextBox>
                        </td>
                        <!-- 商店代號 -->
                        <td style="width: 15%; height: 25px;">
                            <cc1:CustLabel ID="lblShop_ID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="01_01060200_009" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 20%; height: 25px;">
                            <cc1:CustTextBox ID="txtShop_ID" MaxLength="10" runat="server" TabIndex="15"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="width: 15%" align="right">
                            <cc1:CustLabel ID="lblReceive_Total_Count" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01060200_012"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 20%">
                            <cc1:CustTextBox ID="txtReceive_Total_Count" runat="server"></cc1:CustTextBox>
                        </td>
                        <td style="width: 15%">
                            <cc1:CustLabel ID="lblReceive_Total_AMT" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01060200_013"
                                StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 20%">
                            <cc1:CustTextBox ID="txtReceive_Total_AMT" runat="server"></cc1:CustTextBox>
                        </td>
                    </tr>
                </table>
                <cc1:CustPanel ID="pnl1" Width="100%" runat="server" Visible="false">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table1" style="">
                        <tr class="itemTitle">
                            <td colspan="8" style="height: 26px">
                                <li>
                                    <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" Width="200px" IsColon="False" ShowID="01_01060200_002"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblShop_ID2" runat="server" CurSymbol="£" IsColon="True" ShowID="01_01060200_009"
                                    Font-Bold="True" ForeColor="Red"></cc1:CustLabel></td>
                            <td style="width: 85%" colspan="7">
                                <cc1:CustTextBox ID="txtlblShop_ID2" runat="server" TabIndex="21"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtSN" runat="server" Style="display: none"></cc1:CustTextBox></td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 10%; height: 25px;" align="right">
                                <cc1:CustLabel ID="lblCard_No" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_015" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 85%; height: 25px;">
                                <%-- 20220505 調整輸入信用卡號欄位，由4欄合併為1欄 By Kelton --%>
<%--                                <cc1:CustTextBox ID="txtCard_No1" runat="server" MaxLength="4" InputType="Int" TabIndex="25"></cc1:CustTextBox>
                                <span>─</span>
                                <cc1:CustTextBox ID="txtCard_No2" runat="server" MaxLength="4" InputType="Int" TabIndex="30"></cc1:CustTextBox>
                                <span>─</span>
                                <cc1:CustTextBox ID="txtCard_No3" runat="server" MaxLength="4" InputType="Int" TabIndex="35"></cc1:CustTextBox>
                                <span>─</span>
                                <cc1:CustTextBox ID="txtCard_No4" runat="server" MaxLength="4" InputType="Int" TabIndex="40"></cc1:CustTextBox>--%>
                                <cc1:CustTextBox ID="txtCard_No1" runat="server" MaxLength="16" InputType="Int" TabIndex="25"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblTran_Date" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_016" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 85%" colspan="7">
                                <%--<cc1:DatePicker ID="dpTran_Date" Text="" MaxLength="10" runat="server">
                                </cc1:DatePicker>--%>
                                <cc1:CustTextBox ID="txt_dpTran_Date" runat="server" TabIndex="45" MaxLength="8" InputType="int"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblAuth_Code" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_017" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 85%" colspan="7">
                                <cc1:CustTextBox ID="txtAuth_Code" runat="server" MaxLength="6" TabIndex="50"></cc1:CustTextBox></td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%" align="right">                                
                                <cc1:CustLabel ID="lblReceipt_Type2" runat="server" CurSymbol="£" IsColon="True" ShowID="01_01060200_019" Font-Bold="True" ForeColor="Red"></cc1:CustLabel>
                            </td>
                            <td style="width: 85%" colspan="7">
                                <cc1:CustTextBox ID="txtP1" runat="server" MaxLength="2" TabIndex="55"></cc1:CustTextBox>
                                <cc1:CustDropDownList ID="ddlP1" Enabled="false" runat="server" onchange="AuthCode()"></cc1:CustDropDownList>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%" align="right">
                                <cc1:CustLabel ID="lblAMT" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_018" StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 85%" colspan="7">
                                <cc1:CustTextBox ID="txtAMT" runat="server" MaxLength="7" InputType="int" TabIndex="60"></cc1:CustTextBox>
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <cc1:CustButton ID="btnConfirm" runat="server" DisabledWhenSubmit="false" CssClass="smallButton2"
                                    ShowID="01_01060200_020" OnClick="btnConfirm_Click" TabIndex="65" Font-Bold="True"
                                    Font-Size="X-Large" BorderStyle="Double" />
                                <cc1:CustButton ID="CustButton3" runat="server" DisabledWhenSubmit="false" CssClass="smallButton2"
                                    ShowID="01_01060200_020" OnClick="CustButton3_Click" Style="display: none" />
                                <cc1:CustButton ID="btnDubleConfirm" runat="server" DisabledWhenSubmit="false" CssClass="smallButton2"
                                    ShowID="01_01060200_020" Style="display: none" OnClick="btnDubleConfirm_Click" />
                                <cc1:CustButton ID="BtnEditSave" runat="server" DisabledWhenSubmit="false" CssClass="smallButton2"
                                    Style="display: none" OnClick="BtnEditSave_Click" />
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table2" style="">
                        <tr class="itemTitle">
                            <td colspan="9">
                                <li>
                                    <cc1:CustLabel ID="CustLabel13" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" Width="200px" IsColon="False" ShowID="01_01060200_003"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 100%" align="left">
                                <cc1:CustLabel ID="CustLabel15" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_019" StickHeight="False"></cc1:CustLabel>
                                &nbsp;&nbsp;
                                <cc1:CustDropDownList ID="ddlrt1" runat="server">
                                </cc1:CustDropDownList>
                                &nbsp;&nbsp;
                                <cc1:CustLabel ID="CustLabel14" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_015" StickHeight="False"></cc1:CustLabel>
                                &nbsp;&nbsp;
                                <cc1:CustTextBox ID="txtCard_No_1" runat="server" MaxLength="16" InputType="Int"></cc1:CustTextBox>
                                &nbsp;&nbsp;
                                <cc1:CustLabel ID="CustLabel17" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_026" StickHeight="False"></cc1:CustLabel>
                                &nbsp;&nbsp;
                                <cc1:CustTextBox ID="txtAMT_1" runat="server" Width="70px" MaxLength="7" InputType="Int"></cc1:CustTextBox>
                                &nbsp;&nbsp;
                                <cc1:CustLabel ID="CustLabel16" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_027" StickHeight="False"></cc1:CustLabel>
                                &nbsp;&nbsp;
                                <cc1:CustDropDownList ID="ddlReject_Reason" runat="server">
                                </cc1:CustDropDownList>
                                &nbsp;&nbsp;
                                <cc1:CustLabel ID="lblSNText" runat="server" Style="display: none"></cc1:CustLabel>
                                <cc1:CustButton ID="btnReject" runat="server" DisabledWhenSubmit="false" CssClass="smallButton2"
                                    ShowID="01_01060200_020" OnClick="btnReject_Click" Font-Bold="True" Font-Size="X-Large" />
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table3" style="">
                        <tr class="itemTitle">
                            <td colspan="9">
                                <li>
                                    <cc1:CustLabel ID="CustLabel18" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" Width="200px" IsColon="False" ShowID="01_01060200_004"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 100%" align="left">
                                <asp:Panel ID="Pl_gv1" runat="server" ScrollBars="auto">
                                    <cc1:CustGridView ID="grvDetail0" runat="server" AllowSorting="True" Width="99%"
                                        BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid" OnRowDataBound="grvDetail0_RowDataBound"
                                        OnRowDeleting="grvDetail0_RowDeleting" OnRowEditing="grvDetail0_RowEditing" AllowPaging="False"
                                        PagerID="GridPager1">
                                        <Columns>
                                            <asp:TemplateField>
                                                <itemtemplate>
 <cc1:CustLinkButton id="lbtnModify" runat="server" CommandName="Edit" CausesValidation="False"></cc1:CustLinkButton>&nbsp; 
 <cc1:CustLinkButton id="lbtnDelete" runat="server" CommandName="Delete" CausesValidation="False"  OnClientClick="javascript:return confirm('確定刪除?');"></cc1:CustLinkButton>&nbsp; 
</itemtemplate>
                                                <headerstyle width="10%" />
                                                <itemstyle horizontalalign="Center" width="10%" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="SN">
                                                <headerstyle width="5%" />
                                                <itemstyle width="5%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Shop_ID">
                                                <controlstyle font-bold="False" />
                                                <headerstyle width="15%" font-bold="True" forecolor="Red" />
                                                <itemstyle width="15%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Card_No">
                                                <headerstyle width="20%" />
                                                <itemstyle width="20%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="fTran_Date">
                                                <headerstyle width="15%" />
                                                <itemstyle width="15%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Auth_Code">
                                                <headerstyle width="15%" />
                                                <itemstyle width="15%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="AMT">
                                                <headerstyle width="15%" />
                                                <itemstyle width="15%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="NewReceipt_Type">
                                                <headerstyle width="15%" font-bold="True" forecolor="Red" />
                                                <itemstyle width="15%" />
                                            </asp:BoundField>
                                        </Columns>
                                        <RowStyle CssClass="Grid_Item" Wrap="False" />
                                        <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                        <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                        <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                        <PagerSettings Visible="False" />
                                        <EmptyDataRowStyle HorizontalAlign="Center" />
                                    </cc1:CustGridView>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table4" style="">
                        <tr class="itemTitle">
                            <td colspan="9">
                                <li>
                                    <cc1:CustLabel ID="CustLabel19" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" Width="200px" IsColon="False" ShowID="01_01060200_005"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 100%" align="left">
                                <asp:Panel ID="Pl_gv2" runat="server" ScrollBars="Auto">
                                    <cc1:CustGridView ID="grvDetail1" runat="server" AllowSorting="True" Width="99%"
                                        BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid" OnRowDataBound="grvDetail1_RowDataBound"
                                        OnRowDeleting="grvDetail1_RowDeleting" OnRowEditing="grvDetail1_RowEditing" AllowPaging="False"
                                        PagerID="GridPager1">
                                        <Columns>
                                            <asp:TemplateField>
                                                <itemtemplate>
 <cc1:CustLinkButton id="lbtnModify1" runat="server" CommandName="Edit" CausesValidation="False"></cc1:CustLinkButton>&nbsp; 
 <cc1:CustLinkButton id="lbtnDelete1" runat="server" CommandName="Delete" CausesValidation="False" OnClientClick="javascript:return confirm('確定刪除?');"></cc1:CustLinkButton>&nbsp; 
</itemtemplate>
                                                <headerstyle width="10%" />
                                                <itemstyle horizontalalign="Center" width="10%" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="SN">
                                                <headerstyle width="5%" />
                                                <itemstyle width="5%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Card_No">
                                                <headerstyle width="20%" />
                                                <itemstyle width="20%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="AMT">
                                                <headerstyle width="20%" font-bold="True" forecolor="Red" />
                                                <itemstyle width="20%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="NewReceipt_Type">
                                                <headerstyle width="15%" />
                                                <itemstyle width="15%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Reject_Reason">
                                                <headerstyle width="30%" font-bold="True" forecolor="Red" />
                                                <itemstyle width="30%" />
                                            </asp:BoundField>
                                        </Columns>
                                        <RowStyle CssClass="Grid_Item" Wrap="False" />
                                        <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                        <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                        <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                        <PagerSettings Visible="False" />
                                        <EmptyDataRowStyle HorizontalAlign="Center" />
                                    </cc1:CustGridView>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table6" style="">
                        <tr class="itemTitle">
                            <td colspan="9">
                                <li>
                                    <cc1:CustLabel ID="CustLabel21" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" Width="200px" IsColon="False" ShowID="01_01060200_006"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_008" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustTextBox ID="txtBatchDateP1" runat="server"></cc1:CustTextBox>
                            </td>
                            <td style="width: 15%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel20" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_009" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustTextBox ID="txtShop_IDP1" runat="server"></cc1:CustTextBox>
                            </td>
                            <td style="width: 30%; height: 25px;" align="left" colspan="2" rowspan="3">
                                <cc1:CustButton ID="btnFirst" runat="server" DisabledWhenSubmit="false" CssClass="smallButton2"
                                    ShowID="01_01060200_046" OnClick="btnFirst_Click" Font-Bold="True" Font-Size="X-Large" />
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 15%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel22" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_010" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustTextBox ID="txtBatch_NOP1" runat="server"></cc1:CustTextBox>
                            </td>
                            <td style="width: 15%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel23" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_011" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustTextBox ID="txtReceive_BatchP1" runat="server"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 15%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel24" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_012" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustTextBox ID="txtReceive_Total_CountP1" runat="server"></cc1:CustTextBox>
                            </td>
                            <td style="width: 15%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel25" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_013" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustTextBox ID="txtReceive_Total_AMTP1" runat="server"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 100%; height: 25px;" align="left" colspan="6">
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel26" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_028" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 15%; height: 25px;" align="left">
                                <cc1:CustTextBox ID="txtKeyin_Success_Count_All" runat="server"></cc1:CustTextBox>
                            </td>
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel27" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_029" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 45%; height: 25px;" align="left" colspan="3">
                                <cc1:CustTextBox ID="txtKeyin_Success_AMT_All" runat="server"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel28" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_030" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 15%; height: 25px;" align="left">
                                <cc1:CustTextBox ID="txtKeyin_Success_Count_40" runat="server"></cc1:CustTextBox>
                            </td>
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel29" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_031" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 45%; height: 25px;" align="left" colspan="3">
                                <cc1:CustTextBox ID="txtKeyin_Success_AMT_40" runat="server"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel30" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_032" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 15%; height: 25px;" align="left">
                                <cc1:CustTextBox ID="txtKeyin_Success_Count_41" runat="server"></cc1:CustTextBox>
                            </td>
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel31" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_033" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 45%; height: 25px;" align="left" colspan="3">
                                <cc1:CustTextBox ID="txtKeyin_Success_AMT_41" runat="server"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 100%; height: 25px;" align="left" colspan="6">
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel32" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_034" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 15%; height: 25px;" align="left">
                                <cc1:CustTextBox ID="txtKeyin_Reject_Count_All" runat="server"></cc1:CustTextBox>
                            </td>
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel33" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_035" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 45%; height: 25px;" align="left" colspan="3">
                                <cc1:CustTextBox ID="txtKeyin_Reject_AMT_All" runat="server"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel34" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_036" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 15%; height: 25px;" align="left">
                                <cc1:CustTextBox ID="txtKeyin_Reject_Count_40" runat="server"></cc1:CustTextBox>
                            </td>
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel35" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_037" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 45%; height: 25px;" align="left" colspan="3">
                                <cc1:CustTextBox ID="txtKeyin_Reject_AMT_40" runat="server"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel36" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_038" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 15%; height: 25px;" align="left">
                                <cc1:CustTextBox ID="txtKeyin_Reject_Count_41" runat="server"></cc1:CustTextBox>
                            </td>
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel37" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_039" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 45%; height: 25px;" align="left" colspan="3">
                                <cc1:CustTextBox ID="txtKeyin_Reject_AMT_41" runat="server"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel38" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_040" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 15%; height: 25px;" align="left">
                                <cc1:CustTextBox ID="txtFirst_Balance_Count" runat="server"></cc1:CustTextBox>
                            </td>
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel39" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_041" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 45%; height: 25px;" align="left" colspan="3">
                                <cc1:CustTextBox ID="txtFirst_Balance_AMT" runat="server"></cc1:CustTextBox>
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="Table7" style="">
                        <tr class="itemTitle">
                            <td colspan="9">
                                <li>
                                    <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                        SetOmit="False" StickHeight="False" Width="200px" IsColon="False" ShowID="01_01060200_007"></cc1:CustLabel></li>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel40" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_042" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustTextBox ID="txtAdjust_Count" runat="server"></cc1:CustTextBox>
                            </td>
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel41" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_043" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustTextBox ID="txtAdjust_AMT" runat="server"></cc1:CustTextBox>
                            </td>
                            <td style="width: 30%; height: 25px;" align="left" valign="middle" colspan="2" rowspan="2">
                                <cc1:CustButton ID="btnSecond" runat="server" DisabledWhenSubmit="false" CssClass="smallButton2"
                                    ShowID="01_01060200_047" OnClick="btnSecond_Click" Enabled="false" Font-Bold="True"
                                    Font-Size="X-Large" />
                            </td>
                        </tr>
                        <tr class="trEven">
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel42" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_044" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustTextBox ID="txtSecond_Balance_Count" runat="server"></cc1:CustTextBox>
                            </td>
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustLabel ID="CustLabel43" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01060200_045" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 20%; height: 25px;" align="left">
                                <cc1:CustTextBox ID="txtSecond_Balance_AMT" runat="server"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <td style="width: 30%; height: 25px;" align="center" colspan="6">
                                <cc1:CustButton ID="btnBalanceOK" runat="server" DisabledWhenSubmit="false" CssClass="smallButton2"
                                    ShowID="01_01060200_048" OnClick="btnBalanceOK_Click" Font-Bold="True" Font-Size="X-Large" />
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
