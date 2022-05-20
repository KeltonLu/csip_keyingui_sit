<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P010501000001.aspx.cs" Inherits="Page_P010501000001" %>

<%--<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>--%>
<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <script type="text/javascript"> 
    
    var strMsg;
    // 页面方法调用完成的回调函数.
    function OnSucceeded(result)
    {
        //* 显示调用结果
       strMsg += result;
       
    }

    function RadioCheck(object)
    {
        strMsg="";
        $("input[type='radio']").attr("checked","");
        $(object).attr("checked","true");
       
        PageMethods.GetString($(object).parent().next()[0].innerText.replace(/>/g,'＞').replace(/</g,'＜').replace(/\""/g,'＂').replace(/\'/g,'＇'),OnSucceeded);
        
        PageMethods.GetString1(OnSucceeded);
        
        document.getElementById("btnDelete").onclick=function(){return  confirm(strMsg);};
 
    }
    </script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />
</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" EnablePageMethods="True" runat="server">
        </asp:ScriptManager>

        <script language="javascript" type="text/javascript">
              window.attachEvent("onscroll",scroll);   
              function   scroll()   
              {   
                $("#divProgress").css("top",290+document.documentElement.scrollTop);
              }    
        </script>

        <asp:UpdateProgress ID="updateProgress1" runat="server">
            <ProgressTemplate>
                <div id="divProgress" align="center" class="progress" style="position: absolute;
                    top: 290px; width: 100%; filter: Alpha(opacity=80); text-align: center;">
                    <div id="divProgress2" align="center" class="progress" style="background-color: White;
                        width: 50%; margin: 0px auto;">
                        <br />
                        <img alt="Please Wait..." src="../Common/images/Waiting.gif" />
                        <br />
                        <cc1:CustLabel ID="lblWaiting" runat="server" CurAlign="center" CurSymbol="£" FractionalDigit="2"
                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                            SetBreak="False" SetOmit="False" ShowID="00_00000000_000" StickHeight="False"></cc1:CustLabel>
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td>
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cc1:CustGridView ID="grvFUNCTION" runat="server" AllowSorting="True" Width="100%"
                                AllowPaging="False" PagerID="gpList" BorderWidth="0px" CellPadding="0" CellSpacing="1"
                                BorderStyle="Solid">
                                <RowStyle CssClass="Grid_Item" Wrap="True" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:TemplateField>
                                        <itemstyle width="5%" cssclass="Grid_Choice" />
                                        <headerstyle width="5%" />
                                        <itemtemplate>
                                        <input class="ChoiceButton"  id="radRole" onclick="RadioCheck(this);" type="radio" runat="server" /> 
                                        <asp:HiddenField ID="FileId" runat="server" Value='<%# Bind("FileId") %>' />
                                    </itemtemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="JOB_ID">
                                        <itemstyle width="10%" horizontalalign="Center" />
                                        <headerstyle width="10%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="DESCRIPTION">
                                        <itemstyle width="35%" horizontalalign="Left" />
                                        <headerstyle width="35%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="FtpFileName">
                                        <itemstyle width="35%" horizontalalign="Left" />
                                        <headerstyle width="35%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="STATUS">
                                        <itemstyle width="15%" horizontalalign="Center" />
                                        <headerstyle width="15%" horizontalalign="Center" />
                                    </asp:BoundField>
                                </Columns>
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header A" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                            </cc1:CustGridView>
                        </td>
                    </tr>
                    <td>
                        <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right"
                            InputBoxStyle="height:15px" OnPageChanged="gpList_PageChanged">
                        </cc1:GridPager>
                    </td>
                    </tr> </tr> </tr> </tr>
                    <tr class="itemTitle" align="center">
                        <td align="center">
                            <a href="#chapter1">
                                <cc1:CustButton ID="btnAdd" runat="server" CssClass="smallButton" OnClick="btnAdd_Click" />
                            </a>&nbsp;&nbsp;
                            <cc1:CustButton ID="btnUpdate" runat="server" CssClass="smallButton" OnClick="btnUpdate_Click" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnDelete" runat="server" CssClass="smallButton" OnClick="btnDelete_Click" />
                        </td>
                    </tr>
                </table>
                <table id="tblJob" runat="server" width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td style="height: 25px" colspan="4">
                            <li><a id="chapter1">
                                <%--<a href="#chapter1" id="Flg" runat="server" ></a> --%>
                                <cc1:CustLabel ID="CustLabel1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06060000_005" StickHeight="False"></cc1:CustLabel></a></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%" colspan="1">
                            <cc1:CustLabel ID="CustLabel3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06060000_007" StickHeight="False"></cc1:CustLabel></td>
                        <td width="35%">
                            <cc1:CustDropDownList ID="ddlJobName" AutoPostBack="true" OnSelectedIndexChanged="ddlJobName_SelectedIndexChanged"
                                runat="server" Width="190px">
                            </cc1:CustDropDownList>*</td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel5" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06060000_041" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="ctxt_Parameter" runat="server" MaxLength="8" Width="148px"></cc1:CustTextBox>
                            <cc1:CustLabel ID="CustLabel9" runat="server" CurAlign="left" ShowID="01_0102140001_008" ForeColor="red"></cc1:CustLabel></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel11" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06060000_017" StickHeight="False"></cc1:CustLabel></td>
                        <td width="35%">
                            <cc1:CustDropDownList ID="ddlUsingType" runat="server">
                            </cc1:CustDropDownList></td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06060000_040" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="ctxt_loopMinutes" runat="server" MaxLength="4" Width="148px"></cc1:CustTextBox>
                            <cc1:CustLabel ID="CustLabel7" runat="server" CurAlign="left" ShowID="01_0102140001_007" ForeColor="red"></cc1:CustLabel></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel4" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06060000_006" StickHeight="False"></cc1:CustLabel></td>
                        <td width="35%">
                            <cc1:CustTextBox ID="txtFileName" MaxLength="100" runat="server"></cc1:CustTextBox></td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="CustLabel8" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06060000_014" StickHeight="False"></cc1:CustLabel></td>
                        <td width="35%">
                            <cc1:CustTextBox ID="txtZipPwd" runat="server" MaxLength="8" TextMode="Password"
                                Width="148px"></cc1:CustTextBox>
                                <cc1:CustLabel ID="CustLabel6" runat="server" CurAlign="left" ShowID="01_0102140001_006" ForeColor="red"></cc1:CustLabel></td>
                    </tr>
                    <tr class="itemTitle">
                        <td style="height: 25px" colspan="4">
                            <li>
                                <cc1:CustLabel ID="CustLabel21" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="06_06060000_018" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td width="15%" align="right">
                            <cc1:CustLabel ID="CustLabel22" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06060000_019" StickHeight="False"></cc1:CustLabel></td>
                        <td width="35%">
                            <%--<cc1:CustDropDownList ID="ddlFtpIp" runat="server">
                                        </cc1:CustDropDownList>--%>
                            <cc1:CustTextBox ID="ctxt_FtpIp" runat="server" MaxLength="15"></cc1:CustTextBox>
                        </td>
                        <td width="15%" align="right">
                            <cc1:CustLabel ID="CustLabel23" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06060000_020" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 25%">
                            <cc1:CustTextBox ID="txtFtpPath" runat="server" MaxLength="100" Width="148px"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td width="15%" align="right" style="height: 22px">
                            <cc1:CustLabel ID="CustLabel24" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06060000_021" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td width="35%" style="height: 22px">
                            <cc1:CustTextBox ID="txtFtpUser" runat="server" MaxLength="50"></cc1:CustTextBox>
                        </td>
                        <td width="15%" align="right" style="height: 22px">
                            <cc1:CustLabel ID="CustLabel25" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="06_06060000_022" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 25%; height: 22px;">
                            <cc1:CustTextBox ID="txtFtpPwd" TextMode="Password" MaxLength="50" runat="server"
                                Width="148px"></cc1:CustTextBox></td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <table width="100%" border="0" cellpadding="0" cellspacing="1">
                                <tr class="itemTitle">
                                    <td align="center" style="height: 25px">
                                        <cc1:CustButton ID="btnSubMit" runat="server" OnClick="btnOK_Click" CssClass="smallButton" />
                                        <cc1:CustButton ID="btnConcel" runat="server" OnClick="btnConcel_Click1" CssClass="smallButton" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btnAdd" />
                <asp:PostBackTrigger ControlID="btnUpdate" />
                <asp:PostBackTrigger ControlID="ddlJobName" />
                <%--<asp:AsyncPostBackTrigger ControlID="ddlJobName" EventName="OnSelectedIndexChanged" />--%>
            </Triggers>
        </asp:UpdatePanel>
    </form>
</body>
</html>
