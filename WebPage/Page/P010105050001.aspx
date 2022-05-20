<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010105050001.aspx.cs" Inherits="P010105050001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="~/Common/Controls/CustUpdateProgress.ascx" TagName="image" TagPrefix="cust" %>
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
        function checkInputText(id, intType) {
            // 身分證字號不能輸入空
            if (document.getElementById('txtOWNER_ID2').value.Trim() == "") {
                alert('請輸入身分證字號! ');
                document.getElementById('txtOWNER_ID2').focus();
                return false;
            }
            else {
                //檢核身分證字號
                if (!checkID(document.getElementById('txtOWNER_ID2').value.Trim())) {
                    return false;
                }
            }

            // 中文姓名不能輸入空
            if (document.getElementById('txtNameCH').value.Trim() == "") {
                alert('請輸入中文姓名! ');
                document.getElementById('txtNameCH').focus();
                return false;
            }

            // 生日不能輸入空
            if (document.getElementById('txtBIRTH_DATE').value.Trim() == "") {
                alert('請輸入生日! ');
                document.getElementById('txtBIRTH_DATE').focus();
                return false;
            }


            // 發證日期不能輸入空
            if (document.getElementById('txtID_ISSUEDATE').value.Trim() == "") {
                alert('請輸入發證日期! ');
                document.getElementById('txtID_ISSUEDATE').focus();
                return false;
            }

            // 發證地點不能輸入空
            if (document.getElementById('txtID_ISSUEPLACE').value.Trim() == "") {
                alert('請輸入發證地點! ');
                document.getElementById('txtID_ISSUEPLACE').focus();
                return false;
            }

            // 領補換類別
            var id_replaceType = document.getElementById('txtID_REPLACETYPE').value.Trim();
            const tempList = ['1', '2', '3', ''];
            if (tempList.indexOf(id_replaceType) == -1) {
                alert('領補換類別代碼錯誤!');
                document.getElementById('txtID_REPLACETYPE').focus();
                return false;
            }

            // 有無照片
            var id_photoflag = document.getElementById('txtID_PHOTOFLAG').value.Trim();
            const tempList2 = ['0', '1', ''];
            if (tempList2.indexOf(id_photoflag) == -1) {
                alert('有無照片代碼錯誤!');
                document.getElementById('txtID_PHOTOFLAG').focus();
                return false;
            }            

            // 職業編號
            var txtTITLE = document.getElementById('txtTITLE').value.Trim();
            var txtOC = document.getElementById('txtOC').value.Trim();
            if (txtOC.length != 4) {
                alert('職業編號請輸入4碼數字!');
                document.getElementById('txtOC').focus();
                return false;
            }
            else {
                var OC = document.getElementById('hidOC').value.Trim();
                if (OC.indexOf(txtTITLE) == -1) {
                    alert('職業編號不存在!');
                    document.getElementById('txtOC').focus();
                    return false;
                }
            }

            // E-Mail
            var emailF = document.getElementById('txtEMAIL').value.Trim();
            if (emailF.length > 0) {
                if (document.getElementById('radGmail').checked || document.getElementById('radYahoo').checked ||
                    document.getElementById('radHotmail').checked || document.getElementById('radOther').checked) {
                    var emailB = '';
                    if (document.getElementById('radGmail').checked) {
                        emailB = 'gmail.com';
                    }
                    else if (document.getElementById('radYahoo').checked) {
                        emailB = 'yahoo.com.tw';
                    }
                    else if (document.getElementById('radHotmail').checked) {
                        emailB = 'hotmail.com';
                    }
                    else {
                        emailB = document.getElementById('txtEmailOther').value.Trim();
                    }

                    var email = emailF + '@' + emailB;
                    if (email.length > 1) {
                        var emailRule = new RegExp(/^[+a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/i);
                        if (!emailRule.test(email)) {
                            alert('E-Mail錯誤!');
                            document.getElementById('txtEMAIL').focus();
                            return false;
                        }
                    }

                    // 完整E-MAIL填入HiddenField
                    var emailControl = document.getElementById('hidEmailFall');
                    emailControl.value = email;
                }
                else {
                    alert('請選擇E-Mail!');
                    return false;
                }
            }

            return true;
        }


    </script>

    <style type="text/css">
.btnHiden
{display:none; }
        .auto-style2 {
            left: 0px;
            top: 0px;
            position: relative;
            width: 272px;
        }
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
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo1" style="">
                    <tr class="itemTitle1">
                        <td>
                            <li>
                                <cc1:CustLabel ID="Title" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                    SetOmit="False" StickHeight="False" Width="397px" IsColon="False" ShowID="01_01090200_001"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="left" style="width: 100%">
                            <%--自然人身分證字號--%>
                            <div style="margin: 5px">
                                <cc1:CustLabel ID="lblOWNER_ID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_002" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtOWNER_ID" runat="server" MaxLength="10" checktype="ID" Width="200px"
                                    onkeydown="entersubmit('btnSelect');" BoxName="自然人身分證字號" InputType="LetterAndInt"></cc1:CustTextBox>

                                <cc1:CustButton ID="btnSelect" runat="server" CssClass="smallButton" ShowID="01_01040101_027"
                                OnClick="btnSelect_Click"
                                DisabledWhenSubmit="False" BoxName="查詢" />
                            </div> 
                            <%--舊身分證字號--%>
                            <div style="margin: 5px">
                                <cc1:CustLabel ID="lblOWNER_ID_OLD" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01090200_004" StickHeight="False" Style="margin-right:24px"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtOWNER_ID_OLD" runat="server" MaxLength="10" checktype="ID" Width="200px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="舊身分證字號" InputType="LetterAndInt" ></cc1:CustTextBox>
                            </div>     
                        </td>
                    </tr>
                </table>
                <cc1:CustPanel ID="pnlText" runat="server" Width="100%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo4" style="">                                                                        
                        <tr class="trEven">
                            <%--身分證字號--%>
                             <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblOWNER_ID2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_005" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 20%" colspan="2">
                                <cc1:CustTextBox ID="txtOWNER_ID2" runat="server" MaxLength="10" checktype="ID" Width="90px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="身分證字號" InputType="LetterAndInt"></cc1:CustTextBox>
                            </td>
                            <%--中文姓名--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblNameCH" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_006" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td >
                                <cc1:CustTextBox ID="txtNameCH" runat="server" MaxLength="4" Width="200px" onblur="changeFullType(this);"
                                    onkeydown="entersubmit('btnAdd');" BoxName="中文姓名" onpaste="paste();"></cc1:CustTextBox>
                            </td>
                            <%--曾用過的姓名--%>
                            <td align="right" style="width: 11%" colspan="2">
                                <cc1:CustLabel ID="lblNameCH_OLD" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_007" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%" >
                                <cc1:CustTextBox ID="txtNameCH_OLD" runat="server" MaxLength="50" Width="120px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="曾用過的姓名" onpaste="paste();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%-- 英文姓名 --%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblNameEN" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_008" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 25%" colspan="2">
                                <cc1:CustTextBox ID="txtNameEN" runat="server" MaxLength="40" checktype="undefined"
                                    Width="200px" onkeydown="entersubmit('btnAdd');" BoxName="英文姓名"></cc1:CustTextBox>
                            </td>
                            <%-- 出生年月日 --%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblBIRTH_DATE" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01090200_009"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td  >
                                <cc1:CustTextBox ID="txtBIRTH_DATE" runat="server" MaxLength="7" checktype="num"
                                    Width="200px" onkeydown="entersubmit('btnAdd');" BoxName="出生年月日" ToolTip="yyyMMdd"></cc1:CustTextBox>
                            </td>
                            <%-- 性別 --%>
                            <td align="right" style="width: 11%" colspan="2">
                                <cc1:CustLabel ID="lblGENDER" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_010" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 17%">
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtGENDER" runat="server" Width="50px" onfocus="allselect(this);" 
                                        onkeydown="entersubmit('btnAdd');" BoxName="性別" 
                                        Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px;"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropGENDER" kind="select" runat="server" onclick="simOptionClick4IE('txtGENDER');" 
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;"></cc1:CustDropDownList>
                                    <cc1:CustHiddenField ID="hidGENDER" runat="server" />
                                </div>
                            </td>
                        </tr>
                        
                        <tr class="trEven">
                            <%-- 負責人姓名 --%>
                            <td align="right" style="width: 10%">
                                <cc1:CustLabel ID="CustLabel57" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_040" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <%--中文長姓名--%>
                            <td style="width: 10%" colspan="3">
                                <cc1:CustCheckBox ID="chkisLongName_c" runat="server" AutoPostBack="True" BoxName="長姓名" OnCheckedChanged="CheckBox_CheckedChanged" />
                                <cc1:CustLabel ID="lblNameCH_L" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_041" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustTextBox ID="txtNameCH_L" runat="server" MaxLength="50" Width="260px" onblur="changeFullType(this);"
                                    onkeydown="entersubmit('btnAdd');" BoxName="聯絡人長姓名"
                                    onpaste="paste();" AutoPostBack="true"></cc1:CustTextBox>
                            </td>
                            <%--羅馬拼音--%>
                            <td align="right" >
                                <cc1:CustLabel ID="lblNameCH_Pinyin" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_042" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 10%" colspan="3">
                                <cc1:CustTextBox ID="txtNameCH_Pinyin" runat="server" MaxLength="50" Width="260px" onblur="changeFullType(this);"
                                    onkeydown="entersubmit('btnAdd');" BoxName="聯絡人羅馬拼音"
                                    onpaste="paste();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trOdd" >
                            <%-- 國籍1 --%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblCountryCode" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_011" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" >
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtCountryCode" runat="server" Width="50px" onfocus="allselect(this);" 
                                        onkeydown="entersubmit('btnAdd');" BoxName="國籍1" 
                                        Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px;" Enabled="false"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropCountry1" kind="select" runat="server" onclick="simOptionClick4IE('txtCountryCode');" 
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;" Enabled="false"></cc1:CustDropDownList>
                                    <cc1:CustHiddenField ID="hidCountry1" runat="server" />
                                </div>
                            </td>
                            <%-- 國籍2 --%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblCountryCode2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_012" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" colspan="5">
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtCountryCode2" runat="server" Width="50px" onfocus="allselect(this);" MaxLength="3"
                                        onkeydown="entersubmit('btnAdd');" BoxName="國籍2" OnTextChanged="txtCodeType_TextChanged" AutoPostBack="true"
                                        Style="left: 0px; top: 0px; position: relative; width: 32px; height: 11px;"></cc1:CustTextBox>
                                    <cc1:CustDropDownList ID="dropCountry2" kind="select" runat="server" onclick="simOptionClick4IE('txtCountryCode2');" 
                                        Style="left: 0px; top: 0px; clip: rect(0px auto auto 40px); position: absolute; width: 60px;"></cc1:CustDropDownList>
                                    <cc1:CustHiddenField ID="hidCountry2" runat="server" />
                                </div>
                            </td>                            
                        </tr>
                         <tr class="trEven">
                             <%-- 身分證發證日期 --%>
                             <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblID_ISSUEDATE" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01090200_013"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%" >
                                <cc1:CustTextBox ID="txtID_ISSUEDATE" runat="server" MaxLength="7" checktype="num"
                                    Width="200px" onkeydown="entersubmit('btnAdd');" BoxName="身分證發行日期" ToolTip="yyyMMdd"></cc1:CustTextBox>
                            </td>
                             <%-- 發證地點 --%>
                             <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblID_ISSUEPLACE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_014" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtID_ISSUEPLACE" runat="server" MaxLength="18" Width="120px"
                                    onkeydown="entersubmit('btnAdd');" BoxName="發證地點" onpaste="paste();"></cc1:CustTextBox>
                            </td>
                             <%-- 領補換類別 --%>
                             <td align="right">
                                <cc1:CustLabel ID="lblID_REPLACETYPE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_015" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtID_REPLACETYPE" runat="server" MaxLength="1" checktype="undefined"
                                    Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="領補換類別"></cc1:CustTextBox>                                
                            </td>
                             <%-- 有無照片 --%>
                             <td align="right" style="width: 23%">
                                <cc1:CustLabel ID="lblID_PHOTOFLAG" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_016" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 11%">
                                <cc1:CustTextBox ID="txtID_PHOTOFLAG" runat="server" MaxLength="1" checktype="undefined"
                                    Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="有無照片"></cc1:CustTextBox>
                            </td>
                         </tr>
                        <tr class="trOdd">
                            <%-- 戶籍地址 --%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="residenceAddress" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_017" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 88%" colspan="7">
                                <cc1:CustTextBox ID="txtREG_ZIPCODE" runat="server" checktype="fulltype" MaxLength="7"
                                    Width="80px" onkeydown="entersubmit('btnAdd');"
                                    BoxName="戶籍地址郵遞區號"  Enabled="false" BackColor="LightGray"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtREG_CITY" runat="server" checktype="fulltype" MaxLength="6"
                                    Width="100px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="戶籍地址一" onpaste="paste();" AutoPostBack="true" OnTextChanged="TextBox_AddrChanged"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtREG_ADDR1" runat="server" checktype="fulltype" MaxLength="14"
                                    Width="220px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="戶籍地址二" onpaste="paste();"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtREG_ADDR2" runat="server" checktype="fulltype" MaxLength="7"
                                    Width="100px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="戶籍地址三" onpaste="paste();"></cc1:CustTextBox>                               
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%-- 通訊地址 --%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="mailingAddress" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_018" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 88%" colspan="7">
                                <cc1:CustTextBox ID="txtMAILING_CITY" runat="server" checktype="fulltype" MaxLength="6"
                                    Width="100px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="通訊地址一" onpaste="paste();" AutoPostBack="False" ></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtMAILING_ADDR1" runat="server" checktype="fulltype" MaxLength="14"
                                    Width="220px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="通訊地址二" onpaste="paste();"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtMAILING_ADDR2" runat="server" checktype="fulltype" MaxLength="7"
                                    Width="100px" onkeydown="entersubmit('btnAdd');"  onblur="changeFullType(this);"
                                    BoxName="通訊地址三" onpaste="paste();"></cc1:CustTextBox>                               
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%--E-MAIL--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblEMAIL" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_019" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td align="left" style="width:89%" colspan="7">
                                <cc1:CustTextBox ID="txtEMAIL" runat="server"  Width="200px" 
                                    onkeydown="entersubmit('btnAdd');" BoxName="E-MAIL"></cc1:CustTextBox>
                                    @
                                <cc1:CustRadioButton ID="radGmail" runat="server" AutoPostBack="False" GroupName="email" Text="gmail.com" />
                                <cc1:CustRadioButton ID="radYahoo" runat="server" AutoPostBack="False" GroupName="email" Text="yahoo.com.tw" />
                                <cc1:CustRadioButton ID="radHotmail" runat="server" AutoPostBack="False" GroupName="email" Text="hotmail.com" />
                                <cc1:CustRadioButton ID="radOther" runat="server" AutoPostBack="False" GroupName="email" Text="其他：" />
                                <cc1:CustTextBox ID="txtEmailOther" runat="server"  Width="200px" 
                                    onkeydown="entersubmit('btnAdd');" BoxName="E-MAIL"></cc1:CustTextBox>
                                <cc1:CustHiddenField ID="hidEmailFall" runat="server" />
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%-- 連絡電話 --%>
                            <td align="right" style="width: 15%">
                                    <cc1:CustLabel ID="contactTel" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01090200_020"
                                        StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 35%" colspan="3">
                                <cc1:CustTextBox ID="txtCOMP_TEL1" runat="server" Width="60px" MaxLength="3"
                                    onkeydown="entersubmit('btnAdd');" checktype="num" BoxName="連絡電話一"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtCOMP_TEL2" runat="server" Width="100px" MaxLength="8"
                                    onkeydown="entersubmit('btnAdd');" checktype="num" BoxName="連絡電話二"></cc1:CustTextBox>
                                <cc1:CustTextBox ID="txtCOMP_TEL3" runat="server" Width="60px" MaxLength="5"
                                    onkeydown="entersubmit('btnAdd');" checktype="num" BoxName="連絡電話三"></cc1:CustTextBox>
                            </td>
                            <%-- 行動電話 --%>
                            <td align="right" >
                                    <cc1:CustLabel ID="mobilePhone" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01090200_021"
                                        StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 35%" colspan="3">
                                <cc1:CustTextBox ID="txtMobilePhone" runat="server" Width="100px" MaxLength="10"
                                    onkeydown="entersubmit('btnAdd');" checktype="num" BoxName="行動電話"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <%-- 任職公司 --%>
                        <tr class="trOdd">
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblNP_COMPANY_NAME" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_032" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 89%" colspan="7">
                                <cc1:CustTextBox ID="txtNP_COMPANY_NAME" runat="server" MaxLength="120" Width="500px"  onblur="changeFullType(this);"
                                    onkeydown="entersubmit('btnAdd');" BoxName="任職公司" checktype="fulltype" onpaste="paste();"></cc1:CustTextBox>
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%-- 行業別 --%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="industry" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090100_022" StickHeight="False">
                                </cc1:CustLabel>
                            </td>
                            <td align="left" style="width: 89%" colspan="7">
                                <p></p>
                                <div style="position: relative">
                                    <%-- 行業別1 --%>
                                    <cc1:CustLabel ID="labIndustry1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01090100_033" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtIndustry1" runat="server" Width="50px" onfocus="allselect(this);" 
                                        onkeydown="entersubmit('btnAdd');" BoxName="行業別1" MaxLength="2"
                                        Style="left: 0px; top: 0px; position: relative; width: 174px;" AutoPostBack="true" OnTextChanged="txtBasicAMLIndustry1_TextChanged"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="HQlblIndustry1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>                                    
                                    <cc1:CustHiddenField ID="hidIndustry1" runat="server" />
                                </div>
                                <p></p>
                                <div style="position: relative">
                                    <%-- 行業別2 --%>
                                    <cc1:CustLabel ID="labIndustry2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01090100_034" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtIndustry2" runat="server" Width="50px" onfocus="allselect(this);" 
                                        onkeydown="entersubmit('btnAdd');" BoxName="行業別2" MaxLength="2"
                                        Style="left: 0px; top: 0px; position: relative; width: 174px; " AutoPostBack="true" OnTextChanged="txtBasicAMLIndustry2_TextChanged"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="HQlblIndustry2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>                                    
                                    <cc1:CustHiddenField ID="hidIndustry2" runat="server" />
                                </div>
                                <p></p>
                                <div style="position: relative">
                                    <%-- 行業別3 --%>
                                    <cc1:CustLabel ID="labIndustry3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01090100_035" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustTextBox ID="txtIndustry3" runat="server" Width="50px" onfocus="allselect(this);" 
                                        onkeydown="entersubmit('btnAdd');" BoxName="行業別3" MaxLength="2"
                                        Style="left: 0px; top: 0px; position: relative; width: 174px;" AutoPostBack="true" OnTextChanged="txtBasicAMLIndustry3_TextChanged"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="HQlblIndustry3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>                                    
                                    <cc1:CustHiddenField ID="hidIndustry3" runat="server" />
                                </div>
                                <p></p>
                            </td> 
                        </tr>
                        <tr class="trOdd">
                            <%-- 行業別編號1 --%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblCC" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01090100_036" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 89% " colspan="3">                                    
                                    <cc1:CustTextBox ID="txtCC" runat="server" MaxLength="7" checktype="undefined"
                                        Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="行業別編號1" AutoPostBack="true" OnTextChanged="txtBasicAMLCC_TextChanged" ></cc1:CustTextBox>
                                <cc1:CustHiddenField ID="hidAMLCC" runat="server" />
                            </td>
                            <%-- 行業別編號中文名稱 --%>
                            <td align="right" style="width: 11%">
                                <%-- 行業別編號中文名稱1 --%>
                                <cc1:CustLabel ID="CustLabel12" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090100_054" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 89% " colspan="3">
                                <%-- 行業別編號中文姓名1 --%>
                                <cc1:CustLabel ID="HQlblHCOP_CC_Cname1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustHiddenField ID="CustHiddenField1" runat="server" />
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%-- 行業別編號2 --%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblCC2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01090100_037" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3">
                                <cc1:CustTextBox ID="txtCC2" runat="server" MaxLength="7" checktype="undefined" 
                                        Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="行業別編號2" AutoPostBack="true" OnTextChanged="txtBasicAMLCC2_TextChanged"></cc1:CustTextBox>
                            </td>
                            <%-- 行業別編號中文名稱2 --%>
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel14" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090100_055" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3">
                                <%-- 行業別編號中文姓名2 --%>
                                <cc1:CustLabel ID="HQlblHCOP_CC_Cname2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustHiddenField ID="CustHiddenField2" runat="server" />
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%-- 行業別編號3 --%>
                            <td align="right" style="width: 11%">
                               <cc1:CustLabel ID="lblCC3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01090100_038" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3">
                                <cc1:CustTextBox ID="txtCC3" runat="server" MaxLength="7" checktype="undefined" 
                                        Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="行業別編號3" AutoPostBack="true" OnTextChanged="txtBasicAMLCC3_TextChanged"></cc1:CustTextBox>
                            </td>
                            <%-- 行業別編號中文名稱3 --%>
                            <td align="right">
                                <cc1:CustLabel ID="CustLabel16" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" ShowID="01_01090100_056" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="3">
                                <%-- 行業別編號中文姓名3 --%>
                                <cc1:CustLabel ID="HQlblHCOP_CC_Cname3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                <cc1:CustHiddenField ID="CustHiddenField3" runat="server" />
                            </td>
                        </tr>
                        <tr class="trEven">
                            <%-- 職稱 --%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblTITLE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090100_024" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 89%" colspan="7">
                                <div style="position: relative">
                                    <cc1:CustTextBox ID="txtTITLE" runat="server" onfocus="allselect(this);" 
                                        onkeydown="entersubmit('btnAdd');" BoxName="職稱" 
                                        Style="left: 0px; top: 0px; position: relative; width: 100px;" AutoPostBack="true" OnTextChanged="txtBasicAMLTITLE_TextChanged"></cc1:CustTextBox>
                                    <cc1:CustLabel ID="HQlblTITLE" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>                                    
                                    <cc1:CustHiddenField ID="hidTITLE" runat="server" />
                                </div>
                            </td>
                        </tr>
                        <tr class="trOdd">
                            <%-- 職稱編號 --%>
                            <td align="right" style="width: 11%">
                                    <cc1:CustLabel ID="lblOC" runat="server" CurAlign="left" CurSymbol="£"
                                        FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01090100_025"
                                        StickHeight="False"></cc1:CustLabel></td>
                            <td style="width: 89%" colspan="3">
                                <cc1:CustTextBox ID="txtOC" runat="server" Width="100px" MaxLength="4"
                                    onkeydown="entersubmit('btnAdd');" checktype="num" BoxName="職稱編號" AutoPostBack="true" OnTextChanged="txtBasicAMLOC_TextChanged"></cc1:CustTextBox>
                                <cc1:CustHiddenField ID="hidOC" runat="server" />
                            </td>
                            <%-- 職稱編號中文姓名 --%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="CustLabel15" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090100_057" StickHeight="False">
                                </cc1:CustLabel>
                            </td>
                            <td style="width: 89% " colspan="3">
                                <div style="position: relative">
                                    <cc1:CustLabel ID="HQlblHCOP_OC_Cname" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                        IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                        SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>
                                    <cc1:CustHiddenField ID="CustHiddenField4" runat="server" />
                                </div>
                            </td>
                        </tr>
                        <tr class="trEven" style="height:60px">
                            <%-- 收入及資產來源(複選) --%>
                            <td align="right" style="width: 11%">
                                  <cc1:CustLabel ID="income" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_026" StickHeight="False"></cc1:CustLabel>
                             </td >
                             <td  colspan="7">
                                 <cc1:CustCheckBox ID="chkIncome1" runat="server"
                                    AutoPostBack="false" BoxName="薪資" />
                                 <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_043" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                 <cc1:CustCheckBox ID="chkIncome2" runat="server"
                                    AutoPostBack="false" BoxName="經營事業收入" />
                                 <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_044" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                 <cc1:CustCheckBox ID="chkIncome3" runat="server"
                                    AutoPostBack="false" BoxName="退休(職)資金" />
                                 <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_045" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                 <cc1:CustCheckBox ID="chkIncome4" runat="server"
                                    AutoPostBack="false" BoxName="遺產繼承(含贈與)" />
                                 <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_046" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                 <cc1:CustCheckBox ID="chkIncome5" runat="server"
                                    AutoPostBack="false" BoxName="買賣房地產" />
                                 <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_047" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                 <cc1:CustCheckBox ID="chkIncome6" runat="server"
                                    AutoPostBack="false" BoxName="投資理財" />
                                 <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_048" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                 <cc1:CustCheckBox ID="chkIncome7" runat="server"
                                    AutoPostBack="false" BoxName="租金收入" />
                                 <cc1:CustLabel ID="CustLabel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_049" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                 <cc1:CustCheckBox ID="chkIncome8" runat="server"
                                    AutoPostBack="false" BoxName="存款" />
                                 <cc1:CustLabel ID="CustLabel9" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_052" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>

                                 <cc1:CustCheckBox ID="chkIncome9" runat="server"
                                    AutoPostBack="false" BoxName="其他" />
                                 <cc1:CustLabel ID="CustLabel11" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_050" StickHeight="False" Style="margin-right:10px"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--資料最後異動--%>
                        <tr class="trOdd">
                            <%--資料最後異動MAKER--%>
                            <td align="right" style="width: 18%" >
                                <cc1:CustLabel ID="lblLAST_UPD_MAKER" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01090100_028"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td style="width: 16%" >
                                <cc1:CustTextBox ID="txtLAST_UPD_MAKER" runat="server" MaxLength="9" checktype="numandletter"
                                    Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動MAKER"></cc1:CustTextBox>
                            </td>
                            <%--資料最後異動CHECKER--%>
                            <td align="right" colspan="1">
                                <cc1:CustLabel ID="lblLAST_UPD_CHECKER" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090100_029" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td  colspan="2">
                                <cc1:CustTextBox ID="txtLAST_UPD_CHECKER" runat="server" MaxLength="9" checktype="numandletter"
                                    Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動CHECKER"></cc1:CustTextBox>
                            </td>
                            <%--資料最後異動分行--%>
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="lblLAST_UPD_BRANCH" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090100_027" StickHeight="False"></cc1:CustLabel>
                            </td>
                            <td colspan="2">
                                <cc1:CustTextBox ID="txtLAST_UPD_BRANCH" runat="server" MaxLength="4" checktype="numandletter"
                                    Width="100px" onkeydown="entersubmit('btnAdd');" BoxName="資料最後異動分行"></cc1:CustTextBox>                                
                            </td>
                        </tr>
                        <%--已完成SCC表--%>
                        <tr class="trEven">
                            <td align="right" style="width: 11%">
                                <cc1:CustLabel ID="CustLabel10" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090100_053" StickHeight="False"></cc1:CustLabel>
                            </td>
                             <td colspan="7" >
                                <cc1:CustCheckBox ID="chkisSCDD" runat="server"
                                    AutoPostBack="false" BoxName="已完成SCC表" />
                                <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_030" StickHeight="False"></cc1:CustLabel>
                                 <%--AML審查Y為勾選--%>
                                 <cc1:CustLabel ID="CustLabel13" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="false" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" 
                                    SetBreak="False" SetOmit="False" ShowID="01_01090100_058" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                        <%--內容檢核--%>
                        <tr class="trOdd">
                            <td align="right" style="width: 18%" >
                                <cc1:CustLabel ID="lblContentCheck" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="01_01090200_039"
                                    StickHeight="False"></cc1:CustLabel>
                            </td>
                             <td colspan="7" >
                                <cc1:CustCheckBox ID="chkConfirm" runat="server"
                                    AutoPostBack="false" BoxName="以上資料確認無誤" />
                                <cc1:CustLabel ID="lblConfirm" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="01_01090200_051" StickHeight="False"></cc1:CustLabel>
                            </td>
                        </tr>
                    </table>
                    <table width="100%" border="0" cellpadding="0" cellspacing="1" id="tabNo3" style="">
                        <tr class="itemTitle">
                            <td align="center">
                                <cc1:CustButton ID="btnAdd" runat="server" CssClass="smallButton" ShowID="01_01090200_031"
                                    OnClick="btnAdd_Click" DisabledWhenSubmit="False" />
                            </td>
                        </tr>
                    </table>
                </cc1:CustPanel>
            </ContentTemplate>
        </asp:UpdatePanel>
        
        <cc1:CustButton ID="btn007008Hiden"  runat="server" CssClass="btnHiden"
            DisabledWhenSubmit="False"></cc1:CustButton>
        
        <cc1:CustButton ID="btnHiden"  runat="server" CssClass="btnHiden"
            DisabledWhenSubmit="False"></cc1:CustButton>
        <cc1:CustButton ID="btnAddHiden" runat="server" CssClass="btnHiden" DisabledWhenSubmit="False"
             OnClientClick="return checkAdd();"></cc1:CustButton>
            
        
            
        <cc1:CustButton ID="btnAddUpdateHiden" runat="server" CssClass="btnHiden" DisabledWhenSubmit="False"
            ></cc1:CustButton>
    </form>
</body>
</html>
