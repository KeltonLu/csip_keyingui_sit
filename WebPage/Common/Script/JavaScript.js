/// <reference path="jquery-1[1].3.2-vsdoc.js" />


/*                
* **************************************************************
* *********************  WINF JavaScript Library ***************
* ************************************************************** 
* Res:
*       1.键盘控制事件
*       2.弹出新窗口
*       3.格式转换
*       4.处理字符串
*       5.其他
*       6.檢核
*       API Ver: 1.0
*       JavaScript Library Developed By WINF Team.
*       Copyright (c) 2000 - 2010 WINF Team.
*/

//////////////////////////////////////////////////////////////////
///键盘控制事件
///1.禁止右键功能
///2.禁用回車
///3.禁止Shift
///4.禁止刷新，回退
///2008-3-4_by_Mark
//////////////////////////////////////////////////////////////////

///禁止右键功能,单击右键将无反应
///document.oncontextmenu=new Function("event.returnValue=false;"); 

document.onkeydown = function() {
    //alert(event.keyCode);
    //禁用回車
    //if (event.keyCode==13 && event.srcElement.type != "textarea")
    //{
    //    return false; 
    //}
    //禁用Shift
    //if (event.keyCode==16)
    //{
    //    return false; 
    //}
    //禁用Shift
    //if(event.shiftKey)
    //{ 
    //   return false;
    //}

    //禁用刷新，回退
    //if ( (event.altKey) || ((event.keyCode == 8) && (event.srcElement.type != "text" && event.srcElement.type != "textarea" && 
    //    event.srcElement.type != "password")) || 
    //    ((event.ctrlKey) && ((event.keyCode == 78) || (event.keyCode == 82)) ) || 
    //    (event.keyCode == 116) ) 
    //    { 
    //    event.keyCode = 0; 
    //    event.returnValue = false; 
    //    } 
}
//////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////
///弹出新窗口

///1.弹出模态窗口

///2.弹出模态窗口无滚动条

///3.弹出窗口回避IE缓存
///2008-3-4_by_Mark
//////////////////////////////////////////////////////////////////

///Open Modal Windows
function OpenWin(url, name, height, width) {
    var left = Get_Center(width, 'x');
    var top = Get_Center(height, 'y');
    if (top > 30) {
        top = top - 30;
    }
    var win = window.open(url, name, "height=" + height + ",width=" + width + ",left=" + left + ",top=" + top + ",scrollbars=yes,toolbar=no,menubar=no,location=no,resizable=no");
    window.onfocus = function() {
        if (win != null && win.closed == false) {
            win.focus();
        }
        else {
            win = null;
        }
    }
}
///Open Modal Windows Without Scrollbars
function OpenWinWithoutScrollbars(url, name, height, width) {
    var left = Get_Center(width, 'x');
    var top = Get_Center(height, 'y');
    if (top > 30) {
        top = top - 30;
    }
    var win = window.open(url, name, "height=" + height + ",width=" + width + ",left=" + left + ",top=" + top + ",scrollbars=no,toolbar=no,menubar=no,location=no,resizable=no");
    window.onfocus = function() {
        if (win != null && win.closed == false) {
            win.focus();
        }
        else {
            win = null;
        }
    }
}
function Get_Center(size, side) {
    self.y_center = (parseInt(screen.height / 2));
    self.x_center = (parseInt(screen.width / 2));
    center = eval('self.' + side + '_center-(' + size + '/2);');
    return (parseInt(center));
}
///Open View Page Windows
function OpenViewWin(url) {
    if (url.indexOf('?') > 0)
        url = url + "&rdm=" + Math.random();
    else
        url = url + "?rdm=" + Math.random();
    var win = window.open(url, "view");
    win.focus();
}
//////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////
///跳转新页面

///1.无参数跳转页面

///2008-3-4_by_Mark
//////////////////////////////////////////////////////////////////
///go to window with params
function gotoWin(url) {
    if (url.indexOf('?') > 0)
        window.location.href = url + "&rdm=" + Math.random();
    else
        window.location.href = url + "?rdm=" + Math.random();
    return false;
}

//////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////
///格式转换
///1.四舍五入
///2.四舍五入补0
///3.加上,去掉逗號
///4.时间格式转换
///2008-3-4_by_Mark
//////////////////////////////////////////////////////////////////
///四舍五入
function GetDecimal(DecimalValue, DecimalBit) {
    var tempDate = parseFloat(DecimalValue);
    var Result = 0;
    var Bit = "1";
    for (i = 0; i < DecimalBit; i++) {
        Bit += "0";
        tempDate = tempDate * 10;
    }
    var intBit = parseInt(Bit);
    tempDate = Math.round(tempDate);
    tempDate = tempDate / intBit;
    return tempDate;
}
///四舍五入补0
function GetDecimalAdv(DecimalValue, DecimalBit) {
    var tempDate = parseFloat(DecimalValue);
    var Result = 0;
    var Bit = "1";
    for (i = 0; i < DecimalBit; i++) {
        Bit += "0";
        tempDate = tempDate * 10;
    }
    var intBit = parseInt(Bit);
    tempDate = Math.round(tempDate);
    tempDate = tempDate / intBit;

    var tempStr = tempDate.toString().split('.');

    if (tempStr.length == 1) {
        tempDate = tempDate.toFixed(DecimalBit);
    }
    else {
        var strRepl = tempStr[1];

        for (i = 0; i < DecimalBit - tempStr[1].toString().length; i++) {
            strRepl = strRepl + '0';
        }

        tempDate = tempStr[0].toString() + "." + strRepl.toString();
    }
    return tempDate;
}
///四舍五入补0
function GetDecimalAdv(DecimalValue, DecimalBit, DefaultValue) {
    var tempDate = parseFloat(DecimalValue);
    if (isNaN(tempDate)) {
        tempDate = DefaultValue;
        return tempDate;
    }
    var Result = 0;
    var Bit = "1";
    for (i = 0; i < DecimalBit; i++) {
        Bit += "0";
        tempDate = tempDate * 10;
    }
    var intBit = parseInt(Bit);
    tempDate = Math.round(tempDate);
    tempDate = tempDate / intBit;

    var tempStr = tempDate.toString().split('.');

    if (tempStr.length == 1) {
        tempDate = tempDate.toFixed(DecimalBit);
    }
    else {
        var strRepl = tempStr[1];

        for (i = 0; i < DecimalBit - tempStr[1].toString().length; i++) {
            strRepl = strRepl + '0';
        }

        tempDate = tempStr[0].toString() + "." + strRepl.toString();
    }
    return tempDate;
}
///加上逗號
function formatNum(num) {
    if (num == "")
        return "";

    num = Number(num);

    if (isNaN(num))
        return 0;

    num = num + "";

    var arrayNum = num.split('.');

    var returnNum = arrayNum[0];

    var re = /(-?\d+)(\d{3})/;
    while (re.test(returnNum)) {
        returnNum = returnNum.replace(re, "$1,$2")
    }
    if (arrayNum.length == 2) {
        returnNum += "." + arrayNum[1];
    }

    return returnNum;
}
///去掉逗號
function DelDouhao(obj) {
    obj = obj.replace(/,/g, '');
    return obj;
}
/// @param hhmm
/// return hh:mm
function GetNumberTime2Str(time) {
    if (time == "") return "";
    if (time.length != 4) return "";
    var tempStr = time.substring(0, 2) + ":"
     + time.substring(2, 4);
    return tempStr;
}
/// @param hh:mm
/// return hhmm
function GetStr2NumberTime(time) {
    if (time == "") return "";
    if (time.length != 5) return "";
    var hm = time.substring(0, 2)
     + time.substring(3, 5);
    return hm;
}

/// @param String(YYYYMMDD)
/// @return str(YYYY/MM/DD)
function GetStrDate2NumberDate(date) {
    if (date == "") return "";
    var str = date.substring(date, 0, 4) + "/"
      + date.substring(date, 4, 6) + "/"
      + date.substring(date, 6, 8);
    return str;
}

/// @param str (YYYY/MM/DD)
/// @return String(YYYYMMDD)
function GetNumberDate2StrDate(date) {
    if (date == "") return "";
    if (date.length != 10) return "";
    var numberDate = date.substring(0, 4)
      + date.substring(5, 7)
      + date.substring(8, 10);
    return numberDate;
}

/// @param String(YYMMDD)
/// @return str(YY/MM/DD)
function GetYymmdd2NumberDate(date) {
    if (date == "") return "";
    var str = date.substring(2, 4) + "/"
      + date.substring(4, 6) + "/"
      + date.substring(6, 8);
    return str;
}

/// @param str (YY/MM/DD)
/// @return String(YYMMDD)
function GetYymmdd2StrDate(date) {
    if (date == "") return "";
    if (date.length != 8) return "";
    var numberDate = date.substring(0, 2)
      + date.substring(3, 5)
      + date.substring(6, 8);
    return numberDate;
}
//////////////////////////////////////////////////////////////////


//////////////////////////////////////////////////////////////////
///检查上传控件的后缀名是否在许可的type中

///inputId:上传控件id
///type:允许的反缀(如"jpg,gif,png")
//////////////////////////////////////////////////////////////////
function checkExt(inputId, type) {
    var obj = document.getElementById(inputId);

    var fileOutHTML = obj ? obj.outerHTML : "";

    if (obj == null)
        return false;
    var tp = type.toUpperCase();

    if (obj.value.indexOf(".") == -1) {

        alert("Only filename with suffix are allowed.");
        obj.FileName = null;
        obj.FileContent = null;
        obj.outerHTML = fileOutHTML;
        obj.value = "";
        return false;
    }

    if (type == "/")
        return true;

    var filepath = obj.value;
    var re = /(\\+)/g;
    var filename = filepath.replace(re, "#");
    var one = filename.split("#");
    var two = one[one.length - 1];
    var three = two.split(".");
    var last = three[three.length - 1].toUpperCase();

    var rs = tp.indexOf(last);
    if (rs >= 0) {
        return true;
    }
    else {
        obj.FileName = null;
        obj.FileContent = null;
        obj.outerHTML = fileOutHTML;
        obj.value = "";
        alert("Allowed file type: " + tp);
        return false;
    }
}

//////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////
///設置端末信息
///2009-7-9_by_yuyang
//////////////////////////////////////////////////////////////////
function ClientMsgShow(strMsg) {

var local = window.parent.location!=window.location?window.parent:window.opener?window.opener.parent:window;

if(strMsg=="")
{
    local.document.all.clientmsg.style.cursor="";
}
else
{
    local.document.all.clientmsg.style.cursor="pointer";
}
local.document.all.clientmsg.innerText=strMsg;
local.document.all.clientmsg.style.display="none";
setTimeout(SetMarquee2,1000);

  
}
function SetMarquee2(strMsg) {
  var local = window.parent.location!=window.location?window.parent:window.opener?window.opener.parent:window;
  local.document.all.clientmsg.style.display="";
}

//////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////
///設置主機信息
///2009-7-30_by_yuyang
//////////////////////////////////////////////////////////////////
function HostMsgShow(strMsg) {
var local = window.parent.location!=window.location?window.parent:window.opener?window.opener.parent:window;
if(strMsg=="")
{
   local.document.all.hostmsg.style.cursor="";
}
else
{
   local.document.all.hostmsg.style.cursor="pointer";
}
local.document.all.hostmsg.innerText=strMsg;
local.document.all.hostmsg.style.display="none";
setTimeout(SetMarquee1,1000);

  
}
function SetMarquee1(strMsg) {
  var local = window.parent.location!=window.location?window.parent:window.opener?window.opener.parent:window;
  local.document.all.hostmsg.style.display="";
}

//////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////
///处理字符串

///1.自动折行
///2.Trim
///2008-3-4_by_Mark
//////////////////////////////////////////////////////////////////
///自动折行
function AutoBreakWord(str, len) {
    var Result = "";
    var m = 1;
    for (i = 0; i < str.toString().length; i++) {
        if (m % len == 0)
            Result = Result + str.substring(i, i + 1) + "<br>";
        else
            Result = Result + str.substring(i, i + 1);
        m++;
    }
    if (Result.substring(Result.toString().length - 4, Result.toString().length) == "<br>") {
        Result = Result.substring(0, Result.toString().length - 4)
    }

    return Result;
}
///trim Contrl
function TrimContrls(str) {
    str.value = str.value.replace(/\ /g, '');
    return str.value;
}
//Trim all space
function TrimAll(str) {
    str = str.replace(/\ /g, '');
    return str;
}
//////////////////////////////////////////////////////////////////

///设置图片大小（保持高宽比）

function SetImgSize(img, maxWidth, maxHeight) {
    var tempImg = new Image();
    tempImg.src = img.src;
    var width = tempImg.width;
    var height = tempImg.height;
    var scale = width / height;
    if (width > 0 && height > 0) {
        if (width > height) {
            img.style.posWidth = width > maxWidth ? maxWidth : width;
            img.style.posHeight = img.style.posWidth / scale;
        }
        else {
            img.style.posHeight = height > maxHeight ? maxHeight : height;
            img.style.posWidth = img.style.posHeight / scale;
        }
    }
    else {
        img.style.posWidth = maxWidth;
        img.style.posHeight = maxHeight;
    }
    var arr = new Array(2);
    arr[0] = width;
    arr[1] = height;
    return arr;
}

//Set CheckBox with <check> by <id> which is the part ID of CheckBoxes 
function CheckAll(check, id) {
    var e = document.forms[0].elements;
    var l = e.length;
    var o;
    for (var i = 0; i < l; i++) {
        o = e[i];
        if (o.type == "checkbox" && o.id.indexOf(id) > -1) {
            if (o.disabled != true) {
                o.checked = check;
            }
        }
    }
}
//validate length of textbox
function ValidateLength(oSrc, args) {
    args.IsValid = (GetByteLength(args.Value) <= oSrc.getAttribute("limit"));
}

//*全選字體為紅的內容

function allselect(obj){
            obj.select();
}

//*Tab鍵設置焦點

function setfocus(id)
{

    if(event.keyCode==9)
    {
        event.returnValue=false;
        document.getElementById(id).focus();

    }
}

//*選擇按鈕上按Tab鍵設置焦點,
function setfocuschoice(idf,ids)
{
    if(event.keyCode==9)
    {
        event.returnValue=false;
       if(document.getElementById(ids).disabled == false)
       {
       
             document.getElementById(ids).focus();
           
       }
       else
       {
             document.getElementById(idf).focus();
           
       }    
    }
}

function setfocuschoicedrop(id)
{

    if(event.keyCode==9)
    {
       event.returnValue=false;
      document.getElementById(id).focus();
       
    }
}

//*按enter提交指定按鈕事件
function entersubmit(id)
{
            if(event.keyCode==13)
            {
                event.returnValue = false;
                event.preventDefault(); //2021/03/30_Ares_Stanley-增加取消默認值設定
                  document.getElementById(id).click();   
            } 
}

//*按enter提交指定按鈕事件、tab鍵設置下一焦點事件
function keystoke(ids,idt)
{
    if(event.keyCode==13)
    {
         event.returnValue=false;
         document.getElementById(ids).click();   
    } 
            
    if(event.keyCode==9)
    {
       event.returnValue=false;
      document.getElementById(idt).focus();
       
    }
}

//*按enter不響應任何事件

function nosubmit()
{
    if(event.keyCode==13)
    {
        event.returnValue = false;
    }
} 
 
//*onkeypress事件      
 function   keypress(id,ischange) {  
         
            //*按enter提交事件
            if(event.keyCode==13)
            {
                  //event.keyCode=9;
                  event.returnValue=false;
                  document.getElementById(id).focus();
                  document.getElementById(id).click();   
            } 
            else
            {  
                //*半型轉全型 
               if(ischange)
              { 
                  if(event.keyCode>=33 && event.keyCode<=126 )
                  {
                       event.keyCode=event.keyCode+65248;
                  }
                  else if (event.keyCode==32)
                  {
                        event.keyCode=12288;
                  }   
              }     
            } 
  } 
  
  //*按下Keypress以後，不響應該事件

  function   keypressstop() {
       
          if(event.keyCode==13)
          {
                  //event.keyCode=9;
                  event.returnValue=false;
                 
          } 
  }
  
  //*onkeypress事件      
//2021/03/29_Ares_Stanley-修改失效語法; 20210415_Ares_Stanley-調整全半形轉換
function changeFullType(obj) {
    var text = obj.value;
    var result = "";

    for (i = 0; i < text.length; i++) {
        var curchar = text.charAt(i);
        var newchar;
        if (curchar.charCodeAt(0) >= 33 && curchar.charCodeAt(0) <= 126) //英數字
        {
            newchar = String.fromCharCode(curchar.charCodeAt(0) + 65248);
            result += newchar;
        } else if (curchar.charCodeAt(0) == 32) //空白字符
        {
            newchar = String.fromCharCode(12288);
            result += newchar;
        } else {
            result += curchar;
        }
    }
    obj.value = result;
}

   //*檢核是否為中文              
function ischinese(str) {        
     var c;
     var result = true;
     var badChar   =   "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 "; //><,[]{}?/+=|\\'\":;~!#$%()`   

     for(i=0;i<str.length;i++)    
      {      
           c=str.charAt(i);//*字符串str中的字符
           if(badChar.indexOf(c)<0)
            {
                 result = false;
                 break; 
             }
      }  
     return result; 
} 

   //*檢核是否為字母             
function isletter(str) { 
    var c;
    var result = true;
    var badChar   =   "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
     for(i=0;i<str.length;i++)    
      {      
           c=str.charAt(i);//*字符串str中的字符
           if(badChar.indexOf(c)<0)
            {
                 result = false;
                 break; 
             }
      }  
     return result; 
}

   //*檢核電話號碼             
function istelphone(str) { 
    var c;
    var result = true;
    var badChar   =   "0123456789- ";
     for(i=0;i<str.length;i++)    
      {      
           c=str.charAt(i);//*字符串str中的字符
           if(badChar.indexOf(c)<0)
            {
                 result = false;
                 break; 
             }
      }  
     return result; 
}

   //*檢核只能輸入數字、空格            
function isNum(str) { 
    var c;
    var result = true;
    var badChar   =   "0123456789 ";
     for(i=0;i<str.length;i++)    
      {      
           c=str.charAt(i);//*字符串str中的字符
           if(badChar.indexOf(c)<0)
            {
                 result = false;
                 break; 
             }
      }  
     return result; 
}

//*檢核浮點型

function isFloatletter(str) {
    var c;
    var result = true;
    var badChar   =   "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789. ";
     for(i=0;i<str.length;i++)    
      {      
           c=str.charAt(i);//*字符串str中的字符
           if(badChar.indexOf(c)<0)
            {
                 result = false;
                 break; 
             }
      }  
     return result;  
} 

//*檢核EMail
function isemail(str) { 
    var c;
    var result = true;
    var badChar   =   "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_.@";;
     for(i=0;i<str.length;i++)    
      {      
           c=str.charAt(i);//*字符串str中的字符
           if(badChar.indexOf(c)<0)
            {
                 result = false;
                 break; 
             }
      }  
     return result; 
}

//*檢核身分證

function checkID(idStr){

   var iError1=0;
   var iError2=0;
   var iError3=0;
  //* 依照字母的編號排列，存入陣列備用。

  var letters = new Array('A', 'B', 'C', 'D',
      'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M',
      'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V',
      'X', 'Y', 'W', 'Z', 'I', 'O');
  //* 儲存各個乘數

  var multiply = new Array(1, 9, 8, 7, 6, 5,
                           4, 3, 2, 1);
  var nums = new Array(2);
  var firstChar;
  var firstNum;
  var lastNum;
  var total = 0;
  //* 撰寫「正規表達式」。第一個字為英文字母，
  //* 第二個字為1或2，後面跟著8個數字，不分大小寫。

  var regExpID=/^[a-z](1|2)\d{8}$/i;
  
 if(idStr.length!=10)
  {
    iError1=1;
    
  }
     
  //* 使用「正規表達式」檢驗格式

  if (idStr.search(regExpID)==-1) 
  {
      iError1=2;    
  } 
  else 
  {
        // 取出第一個字元和最後一個數字。

        firstChar = idStr.charAt(0).toUpperCase();
        lastNum = idStr.charAt(9);
  }
  //* 找出第一個字母對應的數字，並轉換成兩位數數字。

  for (var i=0; i<26; i++) {
        if (firstChar == letters[i]) {
          firstNum = i + 10;
          nums[0] = Math.floor(firstNum / 10);
          nums[1] = firstNum - (nums[0] * 10);
          break;
        }
  }
  //* 執行加總計算
  for(var i=0; i<multiply.length; i++){
    if (i<2) {
      total += nums[i] * multiply[i];
    } else {
      total += parseInt(idStr.charAt(i-1)) *
               multiply[i];
    }
  }
  //* 和最後一個數字比對

  if((10 - (total % 10))==10)
  {
        if (lastNum!=0)
        {
             iError1=3;   
        }
        
  }
  else if ((10 - (total % 10))!= lastNum) 
  {
         
         iError1=3;   
  }
  
  if(iError1==1 || iError1==2 || iError1==3 )
  {
       if(!confirm('身分證號碼格式錯誤，是否繼續進行？'))
       {
                               
               return false;
       }
  }
  return true;
}

//*中文、英文、數字檢核

function checkInputTypeID(id,strCheck)
{

    
    var inputs = document.getElementById(id).getElementsByTagName("input"); 
        
        for(var i=0;i<inputs.length;i++)
        {
            if(inputs[i].type=="text")
            {
                    var inputText = inputs[i].value;
                    var columnName = "";
                    
                    if(inputText.Trim() == "")
                    {
                        continue;
                    }
                    
                    //*得到BoxName屬性欄位名稱

                    if(typeof(inputs[i].BoxName) != "undefined")
                    {
                        columnName = inputs[i].BoxName;
                    }
                    
                   //*textBox有checklength屬性，只檢核textBox文本截取checklength屬性中長度的字符串
                    if(typeof(inputs[i].checklength) != "undefined")
                    {
                         inputText = inputText.substring(0, inputs[i].checklength);
                    }
                    
                    if(typeof(inputs[i].checktype) != "undefined")
                    { 
                        //*checktype屬性為fulltype將文本字符串轉全型

                        if(inputs[i].checktype=="fulltype")
                        {
                            inputs[i].value = FullType(inputText);
                        }
                        
                        //*檢核數字
                        if(inputs[i].checktype=="num")
                        {
                            
                            if(!isNum(inputText))
                            { 
                                alert(columnName + '欄位只能輸入數字');
                                if(inputs[i].disabled==false)
                                {
                                    inputs[i].focus();
                                }
                                return false; 
                            } 
                         }
                        
                        //*檢核數字&&X
                        if(inputs[i].checktype=="numx")
                        {
                            if(!isNum(inputText) && inputText.toUpperCase() != "X")
                            { 
                                alert(columnName + '欄位只能輸入數字');
                                if(inputs[i].disabled==false)
                                {
                                    inputs[i].focus();
                                }
                                return false; 
                            } 
                         } 
                         
                         //*檢核浮點型

                        if(inputs[i].checktype=="floatletter")
                        {
                            
                            if(!isFloatletter(inputText))
                            { 
                                alert(columnName + '欄位只能輸入數字');
                                if(inputs[i].disabled==false)
                                {
                                    inputs[i].focus();
                                }
                                return false; 
                            } 
                         }
                        
                       //*檢核數字和字母

                        if(inputs[i].checktype == "numandletter")
                        {
                            if(!ischinese(inputText))
                            {
                                alert(columnName + '欄位只能輸入英文和數字');
                                if(inputs[i].disabled==false)
                                {
                                    inputs[i].focus();
                                }
                                return false; 
                            }
                        }
                        
                         //*檢核字母
                        if(inputs[i].checktype == "letter")
                        {
                            if(!isletter(inputText))
                            {
                                alert(columnName + '欄位只能輸入英文');
                                if(inputs[i].disabled==false)
                                {
                                    inputs[i].focus();
                                }
                                return false; 
                            }
                        }
                        
                        //*檢核電話號碼
                        if(inputs[i].checktype == "telphone")
                        {
                            if(!istelphone(inputText))
                            {
                                alert(columnName + '請輸入正確的電話號碼格式');
                                if(inputs[i].disabled==false)
                                {
                                    inputs[i].focus();
                                }
                                return false; 
                            }
                        }
                        
                        //*檢核Email
                        if(inputs[i].checktype == "email")
                        {
                            if(!isemail(inputText))
                            {
                                alert(columnName + '請輸入正確 的Email格式');
                                if(inputs[i].disabled==false)
                                {
                                    inputs[i].focus();
                                }
                                return false; 
                            }
                        }

                        //*檢核身分證號碼-2016/07/20 calvin
                        //修改中信及郵局 自扣一,二KEY身份證號 判斷規則
                        //CSIP自扣ID邏輯
                        //1.10碼ID  EX: A123456***
                        //2.11碼ID  EX: 9+A123456***
                        //3.12碼ID  EX: 9+A123456***A (A-Z英文字母都有可能)
                        if(strCheck!="1" && inputs[i].checktype == "ID")
                        {
                            //*身分證號碼長度不為10  (11碼 or 12碼)
                            if ((inputText.trim().length == 11) || (inputText.trim().length == 12)) {
                                //判斷第1碼 為9
                                if (inputText.toString().substring(0, 1) != "9") {
                                    alert(columnName + '請輸入正確 的身分證號碼格式');
                                    if (inputs[i].disabled == false) {
                                        inputs[i].focus();
                                    }
                                    return false;
                                }

                                //判斷最後1碼 為英文字母                               
                                if (inputText.trim().length == 12) {
                                    if (!isletter(inputText.toString().substring(11, 12))) {
                                        alert(columnName + '請輸入正確 的身分證號碼格式');

                                        if (inputs[i].disabled == false) {
                                            inputs[i].focus();
                                        }
                                        return false;
                                    }
                                }
                                inputText = inputText.toString().substring(1, 11);
                            }   

                            if(!checkID(inputText.trim().toLowerCase()))
                            {                           
                                if(inputs[i].disabled==false)
                                {
                                    inputs[i].focus();
                                }
                                return false; 
                            }
                        }
                 }   
             }
        }
        return true;
}

//*中文、英文、數字檢核

function checkInputType(id)
{
    var inputs = document.getElementById(id).getElementsByTagName("input"); 
        
        for(var i=0;i<inputs.length;i++)
        {
            if(inputs[i].type=="text")
            {
                    var inputText = inputs[i].value;
                    var columnName = "";
                    
                    if(inputText.Trim() == "")
                    {
                        continue;
                    }
                    
                    //*得到BoxName屬性欄位名稱

                    if(typeof(inputs[i].BoxName) != "undefined")
                    {
                        columnName = inputs[i].BoxName;
                    }
                    
                   //*textBox有checklength屬性，只檢核textBox文本截取checklength屬性中長度的字符串
                    if(typeof(inputs[i].checklength) != "undefined")
                    {
                         inputText = inputText.substring(0, inputs[i].checklength);
                    }
                    
                    if(typeof(inputs[i].checktype) != "undefined")
                    { 
                        //*checktype屬性為fulltype將文本字符串轉全型

                        if(inputs[i].checktype=="fulltype")
                        {
                            inputs[i].value = FullType(inputText);
                        }
                        
                        //*檢核數字
                        if(inputs[i].checktype=="num")
                        {
                            
                            if(!isNum(inputText))
                            { 
                                alert(columnName + '欄位只能輸入數字');
                                if(inputs[i].disabled==false)
                                {
                                    inputs[i].focus();
                                }
                                return false; 
                            } 
                         }
                        
                        //*檢核數字&&X
                        if(inputs[i].checktype=="numx")
                        {
                            if(!isNum(inputText) && inputText.toUpperCase() != "X")
                            { 
                                alert(columnName + '欄位只能輸入數字');
                                if(inputs[i].disabled==false)
                                {
                                    inputs[i].focus();
                                }
                                return false; 
                            } 
                         } 
                         
                         //*檢核浮點型

                        if(inputs[i].checktype=="floatletter")
                        {
                            
                            if(!isFloatletter(inputText))
                            { 
                                alert(columnName + '欄位只能輸入數字');
                                if(inputs[i].disabled==false)
                                {
                                    inputs[i].focus();
                                }
                                return false; 
                            } 
                         }
                        
                       //*檢核數字和字母

                        if(inputs[i].checktype == "numandletter")
                        {
                            if(!ischinese(inputText))
                            {
                                alert(columnName + '欄位只能輸入英文和數字');
                                if(inputs[i].disabled==false)
                                {
                                    inputs[i].focus();
                                }
                                return false; 
                            }
                        }
                        
                         //*檢核字母
                        if(inputs[i].checktype == "letter")
                        {
                            if(!isletter(inputText))
                            {
                                alert(columnName + '欄位只能輸入英文');
                                if(inputs[i].disabled==false)
                                {
                                    inputs[i].focus();
                                }
                                return false; 
                            }
                        }
                        
                        //*檢核電話號碼
                        if(inputs[i].checktype == "telphone")
                        {
                            if(!istelphone(inputText))
                            {
                                alert(columnName + '請輸入正確的電話號碼格式');
                                if(inputs[i].disabled==false)
                                {
                                    inputs[i].focus();
                                }
                                return false; 
                            }
                        }
                        
                        //*檢核Email
                        if(inputs[i].checktype == "email")
                        {
                            if(!isemail(inputText))
                            {
                                alert(columnName + '請輸入正確 的Email格式');
                                if(inputs[i].disabled==false)
                                {
                                    inputs[i].focus();
                                }
                                return false; 
                            }
                        }
                        
                        //*檢核身分證號碼
                        //修改自扣一二key身份證號 判斷規則
                        //CSIP自扣ID邏輯
                        //1.10碼ID  EX: A123456***
                        //2.11碼ID  EX: 9+A123456***
                        //3.12碼ID  EX: 9+A123456***A (A-Z英文字母都有可能)
                        if(inputs[i].checktype == "ID")
                        {
                            //*身分證號碼長度不為10  (11碼 or 12碼)
                            if((inputText.trim().length==11) || (inputText.trim().length==12))
                            {
                               //判斷第1碼 為9
                               if(inputText.toString().substring(0,1) != "9")
                               {
                                  alert(columnName + '請輸入正確 的身分證號碼格式');  
                                  if(inputs[i].disabled==false)
                                  {
                                     inputs[i].focus();
                                  }
                                  return false;                                                                
                               }
                               
                               //判斷最後1碼 為英文字母                               
                               if(inputText.trim().length==12)                           
                               {                                                           
                                  if(!isletter(inputText.toString().substring(11,12)))
                                  {
                                     alert(columnName + '請輸入正確 的身分證號碼格式');                                     
                                      
                                     if(inputs[i].disabled==false)
                                     {
                                       inputs[i].focus();
                                     }
                                     return false; 
                                  }
                               }                     
                               inputText = inputText.toString().substring(1,11);
                            }   
                                                       
                            if(!checkID(inputText.trim().toLowerCase()))
                            {                           
                                if(inputs[i].disabled==false)
                                {
                                    inputs[i].focus();
                                }
                                return false; 
                            }                                                     
                        }
                 }   
             }
        }
        return true;
}

//*中文、英文、數字檢核(特點資料異動)
function checkNewInputType(id)
{
    var inputs = document.getElementById(id).getElementsByTagName("input"); 
        
        for(var i=0;i<inputs.length;i++)
        {
            if(inputs[i].type=="text" && inputs[i].disabled==false)
            {
                    var inputText = inputs[i].value;
                    if(inputText.Trim() == "")
                    {
                        continue;
                    }
                    
                    //*得到BoxName屬性欄位名稱

                    var columnName = "";
                    if(typeof(inputs[i].BoxName) != "undefined")
                    {
                        columnName = inputs[i].BoxName;
                    }
                    
                    //*textBox有checklength屬性，只檢核textBox文本截取checklength屬性中長度的字符串   
                    if(typeof(inputs[i].checklength) != "undefined")
                    {
                        
                         inputText = inputText.substring(0, inputs[i].checklength);
                    }
                    
                    if(typeof(inputs[i].checktype) != "undefined")
                    { 
                        //*checktype屬性為fulltype將文本字符串轉全型

                        if(inputs[i].checktype=="fulltype")
                        {
                            inputs[i].value = FullType(inputText);
                        }
                        
                        //*檢核數字
                        if(inputs[i].checktype=="num")
                        {
                            if(!isNum(inputText) && inputText.toUpperCase() != "X")
                            { 
                                alert(columnName + '欄位只能輸入數字');
                                inputs[i].focus();
                                return false; 
                            } 
                         }
                         
                         //*檢核浮點型

                        if(inputs[i].checktype=="floatletter")
                        {
                            
                            if(!isFloatletter(inputText))
                            { 
                                alert(columnName + '欄位只能輸入數字');
                                inputs[i].focus();
                                return false; 
                            } 
                         }
                        
                       //*檢核數字和字母

                        if(inputs[i].checktype == "numandletter")
                        {
                            if(!ischinese(inputText))
                            {
                                alert(columnName + '欄位只能輸入英文和數字');
                                inputs[i].focus();
                                return false; 
                            }
                        }
                        
                         //*檢核字母
                        if(inputs[i].checktype == "letter")
                        {
                            if(!isletter(inputText))
                            {
                                alert(columnName + '欄位只能輸入英文');
                                inputs[i].focus();
                                return false; 
                            }
                        }
                        
                        //*檢核電話號碼
                        if(inputs[i].checktype == "telphone")
                        {
                            if(!istelphone(inputText))
                            {
                                alert(columnName + '欄位請輸入正確的電話號碼格式');
                                inputs[i].focus();
                                return false; 
                            }
                        }
                        
                        //*檢核Email
                        if(inputs[i].checktype == "email")
                        {
                            if(!isemail(inputText))
                            {
                                alert(columnName + '欄位請輸入正確 的Email格式');
                                inputs[i].focus();
                                return false; 
                            }
                        }
                        
                        //*檢核身分證號碼

                        if(inputs[i].checktype == "ID")
                        {
                            //*身分證號碼長度不為10
                            if(!checkID(inputText.trim().toLowerCase()))
                            {                           
                                inputs[i].focus();
                                return false; 
                            }
                        }
                  }
             }
        }
        return true;
}

//*清空、設置控件不可用
function setControlsDisabled(id)
{
    var inputs = document.getElementById(id).getElementsByTagName("input");
    var selects = document.getElementById(id).getElementsByTagName("select");
     for(var i=0;i<inputs.length;i++)
     {
            if(inputs[i].type=="text")
            {
                inputs[i].value = "";
                inputs[i].disabled = true;
            }
            if(inputs[i].type=="checkbox" || inputs[i].type=="radio")
            {
                inputs[i].checked = false;
                inputs[i].disabled = true;
            }
            if(inputs[i].type=="submit")
            {
                inputs[i].disabled = true;
            }
     }
     for(var i=0;i<selects.length;i++)
     { 
            if(selects[i].kind=="select")
            {
                selects[i].disabled = true;
            } 
      }
}

//*清空TextBox文本
function clearControls(id)
{
    var inputs = document.getElementById(id).getElementsByTagName("input");
     for(var i=0;i<inputs.length;i++)
     {
            if(inputs[i].type=="text")
            {
                inputs[i].value = "";
            }
     }
}

//*將字符串轉為全型
function FullType(str)
{
    var tmp = "";   
    for(var i = 0; i < str.length; i++)   
    {   
          if(str.charCodeAt(i) >= 33 && str.charCodeAt(i) <= 126 )
          {
               tmp += String.fromCharCode(str.charCodeAt(i) + 65248);    
          }
          else if (str.charCodeAt(i) == 32)
          {
               tmp += String.fromCharCode(12288);   

          }
          else
          {
                tmp += str.charAt(i);
          }    
    }
    return tmp;   
}

//*粘貼后轉全型
  function paste()
  {
        var str1 = window.clipboardData.getData('text');
         var tmp   =   "";   

            for(var   i=0;i<str1.length;i++)   
            {   
                    
                          if(str1.charCodeAt(i)>=33 && str1.charCodeAt(i)<=126 )
                          {
                               tmp+=String.fromCharCode(str1.charCodeAt(i)+65248);    
                          }
                          else if (str1.charCodeAt(i)==32)
                          {
                               tmp+=String.fromCharCode(12288);   

                          }
                          else
                          {
                                tmp+=str1.charAt(i);
                          }
                    
                    
            }   
            window.clipboardData.setData("Text",tmp);
  }
  
  //*下拉列表選項顯示在TexyBox
  function simOptionClick4IE(id)//*傳入id為TexyBox ID
{     
     var evt=window.event   ;      
     var selectObj=evt?evt.srcElement:null;     
 
      // IE Only     
      if (evt && selectObj &&   evt.offsetY && evt.button!=2     
 
         && (evt.offsetY > selectObj.offsetHeight || evt.offsetY<0 ) ) 
         {                      
            //2021/04/01_Ares_Stanley-下拉選單資料取得失敗
              //* 记录原先的选中项     
          var option;
          if (selectObj.index != undefined) {
              option = selectObj.parentNode[selectObj.index];
          }
          else
          {
              option = selectObj.options[selectObj.selectedIndex];
          }
              document.getElementById(id).value=option.innerText;  
              document.getElementById(id).focus(); 
 
      }     
}

//*去除字符串左右空格

String.prototype.Trim = function()
{
    return this.replace(/(^\s*)|(\s*$)/g, "");
}

 function checkDateSn(strSrc)
 {
      

          
                try
                {
                    //* 非法字符檢查
                    var patten=new RegExp(/^[0-9]*$/); 
                    if (!patten.test(strSrc))
                        return -2;
                    
                    //* 加19110000，轉換成西元日期格式
                    var intDateSend = parseFloat(strSrc) + 19110000;
                    year = intDateSend.toString().substring(0,4);
                    month = intDateSend.toString().substring(4,6);
                    day = intDateSend.toString().substring(6,8);
                    if (year < 1000 || year > 9999){
                        return -2;
                    }
                    if (month < 1 || month > 12){
                        return -2;
                    }
                    if (day < 1 || day > 31){
                        return -2;
                    }
                    if ((month==4 || month==6 || month==9 || month==11) && day==31){
                        return -2;
                    }
                    if (month==2){
                        var isleap=(year % 4==0 && (year % 100 !=0 || year % 400==0));
                        
                        if (day>29){
                            return -2;
                        }
                        if ((day==29) && (!isleap)){
                            return -2;
                        }
                    }
                    
                    //* 生成日期返回
                    var strDateSend = intDateSend.toString().substring(0,4) + "/" + intDateSend.toString().substring(4,6) 
                            + "/" + intDateSend.toString().substring(6,8);
                    var datDateSend = new Date(strDateSend);
                    return datDateSend;
                }
                catch(e)
                {
                    return -2;
                }
           
}

 //* 檢查輸入的日期格式是否正確

    function checkDateReport( obj )
    {
        if (null==obj)
            return 0;
        
        //* 檢查是否輸入
        if (obj.value.Trim() == "")
        {
            return -1;
        }
        else
        {
            //* 檢查輸入的長度是否正確

            if (obj.value.Trim().length != 8)
            {
                return -2;
            }else
            {
                try
                {
                    //* 非法字符檢查
                    var patten=new RegExp(/^[0-9]*$/); 
                    if (!patten.test(obj.value.Trim()))
                        return -2;
                    
                    //* 加19110000，轉換成西元日期格式
                    var intDateSend = parseFloat(obj.value.Trim()) + 19110000;
                    year = intDateSend.toString().substring(0,4);
                    month = intDateSend.toString().substring(4,6);
                    day = intDateSend.toString().substring(6,8);
                    if (year < 1000 || year > 9999){
                        return -2;
                    }
                    if (month < 1 || month > 12){
                        return -2;
                    }
                    if (day < 1 || day > 31){
                        return -2;
                    }
                    if ((month==4 || month==6 || month==9 || month==11) && day==31){
                        return -2;
                    }
                    if (month==2){
                        var isleap=(year % 4==0 && (year % 100 !=0 || year % 400==0));
                        
                        if (day>29){
                            return -2;
                        }
                        if ((day==29) && (!isleap)){
                            return -2;
                        }
                    }
                    
                    //* 生成日期返回
                    var strDateSend = intDateSend.toString().substring(0,4) + "/" + intDateSend.toString().substring(4,6) 
                            + "/" + intDateSend.toString().substring(6,8);
                    var datDateSend = new Date(strDateSend);
                    return datDateSend;
                }catch(e)
                {
                    return -2;
                }
            }
        }
    }