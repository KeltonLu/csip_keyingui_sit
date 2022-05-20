//******************************************************************
//*  作    者：占偉林(James)
//*  功能說明：系統角色維護
//*  創建日期：2009/07/10
//*  修改紀錄：
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*******************************************************************
//20160907 (U) by Tank, 新增btn權限，控制元件顯示/隱藏

using System;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using System.Reflection;
using CSIPCommonModel.BusinessRules;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BaseItem;
using Framework.Common.Utility;
using Framework.Common.Message;
using Framework.WebControls;
using Framework.Common.JavaScript;
using Framework.Data.OM.OMAttribute;
using CSIPKeyInGUI.BusinessRules_new;
using System.Text.RegularExpressions;
using Framework.Common;
using Framework.Common.Logging;
using System.Collections;
using Framework.Data.OM.Collections;
using CSIPKeyInGUI.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using System.Data.SqlClient;
using System.Linq;

/// <summary>
/// 要做權限判斷的頁面基礎類別
/// </summary>
public class PageBase : System.Web.UI.Page
{
    protected long ProgramBeginRunTime;
    protected long programRunTime;

    protected StringBuilder sbRegScript;
    protected string strClientMsg = "";
    protected string strHostMsg = "";
    protected string strAlertMsg = "";
    protected string strIsShow = "";

    /// <summary>
    /// 追加字典設定，與畫面檢核分開
    /// </summary>
    protected Dictionary<string, string> BaseDc = new Dictionary<string, string>();


    //edit by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
    protected eMstType etMstType = eMstType.NoAlert;

    protected enum eMstType
    {
        //* 無需做pop動作
        NoAlert = 1,
        //* 查詢操作
        Select = 2,
        //* 新增、異動、刪除操作
        Control = 3,

    }
    //edit by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

    /*增加記錄網頁訊息的struct Add by 陳靜嫻2009-09-21 Start */
    public struct structPageInfo
    {
        public string strPageCode;//*網頁FunctionID
        public string strPageName;//*網頁名稱
    }
    /*增加記錄網頁訊息的struct Add by 陳靜嫻2009-09-21 End */

    public PageBase()
    {
        sbRegScript = new StringBuilder();
        this.ProgramBeginRunTime = System.Environment.TickCount; //程序开始运行时间
    }

    /// <summary>
    /// 填充页面上显示程序运行时间的文本控件
    /// </summary>
    /// <param name="literal">显示程序运行时间的文本控件</param>
    private void ProgramRunTime()
    {
        long ProgramEndRunTime = System.Environment.TickCount;
        programRunTime = ProgramEndRunTime - this.ProgramBeginRunTime;
        // jsBuilder.RegScript(this.Page, "var local = window.parent.location!=window.location?window.parent:window.opener?window.opener.parent.location!=window.opener.location?window.opener.parent:window.opener.opener.parent:window;local.document.all.runtime.innerText='" + programRunTime.ToString() + " 毫秒';");
        // jsBuilder.RegScript(this.Page, "window.parent.postMessage({ func: 'ProgramRunTime', data: '" + programRunTime.ToString() + " 毫秒' }, '*');");
        sbRegScript.Append("window.parent.postMessage({ func: 'ProgramRunTime', data: '" + programRunTime.ToString() + " 毫秒' }, '*');");
    }

    protected override void Render(System.Web.UI.HtmlTextWriter writer)
    {
        //* 如果Session中含有主機的SessionID,則發送電文關閉之,并清空網站中存的主機SessionID
        MainFrameInfo.ClearHtgSession();

        this.ProgramRunTime();
        if (this.strAlertMsg != "")
        {
            sbRegScript.Append("alert('" + strAlertMsg + "');");
        }

        if (strIsShow != "")
        {
            sbRegScript.Append(strIsShow);
        }
        else
        {
            //edit by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
            // sbRegScript.Append(@"ClientMsgShow('" + strClientMsg + "');" + "HostMsgShow(\"" + strHostMsg + "\");");
            sbRegScript.Append(@"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + strClientMsg + "' }, '*');" + "window.parent.postMessage({ func: 'HostMsgShow', data: '" + strHostMsg + "' }, '*');");
            if (etMstType != eMstType.NoAlert && "" != strHostMsg.Trim())
            {
                sbRegScript.Append(@"alert('" + strHostMsg + "');");
            }
            //edit by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end
        }
        jsBuilder.RegScript(this.Page, sbRegScript.ToString());
        base.Render(writer);
    }

    /// <summary>
    /// 頁面的Function_ID
    /// </summary>
    private String _Function_ID;

    protected string M_Function_ID
    {
        get { return this._Function_ID; }
        set
        {
            this._Function_ID = value;
        }
    }

    /// <summary>
    /// 頁面的Function_Name
    /// </summary>
    private String _Function_Name;


    public string M_Function_Name
    {
        get { return this._Function_Name; }
        set
        {
            //this._Function_Name = value; 
        }
    }

    /// <summary>
    /// 黨頁面加載時
    /// </summary>
    /// <param name="e">事件參數</param>
    protected override void OnLoad(EventArgs e)
    {
        //檢核操作瀏覽器功能
        CensorPage();

        //* 取頁面的功能ID號(Function_ID)
        /*增加記錄網頁訊息的struct Add by 陳靜嫻2009-09-21 Start */
        structPageInfo sPageInfo = new structPageInfo();
        /*增加記錄網頁訊息的struct Add by 陳靜嫻2009-09-21 End */

        string strMsg = "";
        string strUrlError = UtilHelper.GetAppSettings("Error").ToString();

        //* 下載文檔默認名稱 
        if (CSIPKeyInGUI.BusinessRules.BRExcel_File.DownloadFileName == "")
            CSIPKeyInGUI.BusinessRules.BRExcel_File.DownloadFileName = UtilHelper.GetAppSettings("CS27DownloadFileName").ToString();

        //* 判斷Session是否存在
        if (Session == null || HttpContext.Current.Session == null || this.Session["Agent"] == null)
        {

            #region 判斷Session是否存在及重新取Session值
            //* Session不存在時，判斷TicketID是否存在
            if (string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "TicketID")))
            {
                //* TicketID不存在，顯示重新登入訊息，轉向重新登入畫面
                //jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage("00_00000000_035") + "');var local = window.parent?window.parent:window;local.location.href='" + strUrlLogon + "';");
                //return;
                strMsg = "00_00000000_035";
                //*TicketID不存在，顯示重新登入訊息，轉向重新登入畫面 //20210412_Ares_Stanley-調整轉向方法
                Response.Redirect(strUrlError + "?MsgID=" + RedirectHelper.GetEncryptParam(strMsg), false);
            }
            else
            {
                //* TicketID存在時，
                //* 取TicketID
                string strTicketID = RedirectHelper.GetDecryptString(this.Page, "TicketID");
                // 以TicketID到DB中取Session資料。

                if (!getSessionFromDB(strTicketID, ref strMsg))
                {
                    //jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsg) + "');var local = window.parent?window.parent:window;local.location.href='" + strUrlLogon + "';");
                    //return;
                    //20210412_Ares_Stanley-調整轉向方法
                    Response.Redirect(strUrlError + "?MsgID=" + RedirectHelper.GetEncryptParam(strMsg), false);
                }
            }
            #endregion 判斷Session是否存在及重新取Session值
        }
        else
        {

            #region 判斷用戶是否有使用該頁面的權限
            //* 取頁面的功能ID號(Function_ID)
            this._Function_ID = "88888888";
            string strPath = this.Server.MapPath(this.Request.Url.AbsolutePath).ToUpper();
            if (strPath.IndexOf("DEFAULT") == -1)
            {
                PageAction pgaNow = PopedomManager.MainPopedomManager.PageSettings[strPath];
                this._Function_ID = pgaNow.FunctionID;   //* 頁面的功能ID號
                /*Session中增加記錄網頁訊息的struct Add by 陳靜嫻2009-09-21 Start */
                sPageInfo.strPageCode = pgaNow.FunctionID;
                this.Session["PageInfo"] = sPageInfo;
                Framework.Common.Logging.Logging.UpdateLogAgentFunctionId(sPageInfo.strPageCode);
                /*Session中增加記錄網頁訊息的struct Add by 陳靜嫻2009-09-21 End */

                strMsg = "00_00000000_021";
                string strUrlLogon = UtilHelper.GetAppSettings("LOGOUT").ToString();

                if (this.Session["Agent"] == null || ((EntityAGENT_INFO)this.Session["Agent"]).dtfunction == null ||
               ((DataTable)((EntityAGENT_INFO)this.Session["Agent"]).dtfunction).Rows.Count <= 0)
                {
                    jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsg) + "');var local = window.parent?window.parent:window;local.location.href='" + strUrlLogon + "';");
                    return;
                }
                else
                {
                    bool blCanUseAction = false;
                    //* 檢查用戶的權限列表中是否存在當前頁面的Funcion_ID;
                    for (int intLoop = 0; intLoop < ((DataTable)((EntityAGENT_INFO)this.Session["Agent"]).dtfunction).Rows.Count; intLoop++)
                    {
                        if (((DataTable)((EntityAGENT_INFO)this.Session["Agent"]).dtfunction).Rows[intLoop]["Function_ID"].ToString() == this._Function_ID)
                        {
                            this._Function_Name = ((DataTable)((EntityAGENT_INFO)this.Session["Agent"]).dtfunction).Rows[intLoop]["Function_Name"].ToString();
                            blCanUseAction = true;
                            if (((DataTable)((EntityAGENT_INFO)this.Session["Agent"]).dtfunction).Rows[intLoop]["Action_List"].ToString() != "")
                            {
                                DataTable dtblAction = Framework.Common.Serialization.XMLSerialization<DataTable>.XmlStringToObject(((DataTable)((EntityAGENT_INFO)this.Session["Agent"]).dtfunction).Rows[intLoop]["Action_List"].ToString());



                                for (int i = 0; i < dtblAction.Rows.Count; i++)
                                {
                                    switch (dtblAction.Rows[i][EntityR_ROLE_ACTION.M_ACTION_ID].ToString().Substring(0, 3).ToLower())
                                    {
                                        case "chk": //複選框
                                            CheckBox chkAction = (CheckBox)this.Page.FindControl(dtblAction.Rows[i][EntityR_ROLE_ACTION.M_ACTION_ID].ToString());
                                            SetControl(chkAction, dtblAction.Rows[i]["HadRight"].ToString());
                                            break;
                                        //20160907 (U) by Tank, add btn項目
                                        case "btn":
                                            Button btnAction = (Button)this.Page.FindControl(dtblAction.Rows[i][EntityR_ROLE_ACTION.M_ACTION_ID].ToString());
                                            SetControl_Visible(btnAction, dtblAction.Rows[i]["HadRight"].ToString());
                                            break;
                                    }
                                }
                            }

                            break;
                        }
                    }

                    //* 沒有權限使用該功能ID
                    if (!blCanUseAction)
                    {
                        //jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsg) + "');var local = window.parent?window.parent:window;local.location.href='" + strUrlLogon + "';");
                        strMsg = "00_00000000_025";
                        //20210412_Ares_Stanley-調整轉向方法
                        Response.Redirect(strUrlError + "?MsgID=" + RedirectHelper.GetEncryptParam(strMsg), false);
                        return;
                    }
                }
            }
            #endregion
        }
        base.OnLoad(e);
    }

    /// <summary>
    /// 以TicketID到DB中取Session資料。
    /// </summary>
    /// <param name="strTicketID"></param>
    private bool getSessionFromDB(String strTicketID, ref string strMsg)
    {
        EntityAGENT_INFO eAgentInfo = new EntityAGENT_INFO();

        EntitySESSION_INFO eSessionInfo = new EntitySESSION_INFO();

        eSessionInfo.TICKET_ID = strTicketID;

        //* 取Session訊息
        if (!BRSESSION_INFO.Search(eSessionInfo, ref eAgentInfo, ref strMsg))
        {
            return false;
        }

        //* 重新回覆當前Session的訊息
        this.Session["Agent"] = eAgentInfo;
        Logging.NewLogAgent(eAgentInfo.agent_id);

        //* 刪除DB中的TicketID對應的Session訊息
        if (!BRSESSION_INFO.Delete(eSessionInfo, ref strMsg))
        {
            return false;
        }
        return true;
    }


    private void SetControl(System.Web.UI.WebControls.WebControl btnAction, string strHadRight)
    {
        if (btnAction != null)
        {
            if (strHadRight == "Y")
            {
                if (btnAction.Enabled)
                {
                    btnAction.Enabled = true;
                }
            }
            else
            {
                if (btnAction.Enabled)
                {
                    btnAction.Enabled = false;
                }
            }
        }
    }

    //20160907 (U) by Tank, 透過R_ROLE_ACTION，控制元件顯示/隱藏
    private void SetControl_Visible(System.Web.UI.WebControls.WebControl btnAction, string strHadRight)
    {
        if (btnAction != null)
        {
            if (strHadRight == "Y")
            {
                if (btnAction.Visible)
                {
                    btnAction.Visible = true;
                }
            }
            else
            {
                if (btnAction.Visible)
                {
                    btnAction.Visible = false;
                }
            }
        }
    }

    #region 映射取值後檢核
    /// <summary>
    /// 傳入物件，依欄位自帶檢核項目執行
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="RtnMsg"></param>
    public void ValidVal_Nochange<T>(T obj, ref List<string> RtnMsg, Page ctColl, string LineID)
    {
        Type v = obj.GetType();  //取的型別實體
        PropertyInfo[] props = v.GetProperties(); //取出所有公開屬性(可以被外部存取得 
        foreach (PropertyInfo prop in props)
        {
            object[] attrs = prop.GetCustomAttributes(true); //取得自訂屬性，第一個物件          

            //  CustTextBox tmpObj = null;
            //先找出有沒有控制項，且為CustTextBox
            //for (int xi = 0; xi < attrs.Length; xi++)
            //{
            ////    //找出設定欄位屬性(非驗證)
            ////    if (attrs[xi] is AttributeRfPage)
            ////    {
            ////        AttributeRfPage authAttr = attrs[xi] as AttributeRfPage;
            ////        string ctrlID = authAttr.ControlID;
            ////        object controllS = this.FindControl(ctrlID + LineID);
            ////        if (controllS != null && controllS is CustTextBox)
            ////        {
            ////            //有找到，先把底色還原
            ////            tmpObj = controllS as CustTextBox;
            ////            tmpObj.BackColor = System.Drawing.Color.White;

            ////        }
            ////    }
            //}
            for (int i = 0; i < attrs.Length; i++)
            {
                //確認型別為驗證用
                if (attrs[i] is AttributeValidPage)
                {
                    string exVal = prop.GetValue(obj, null) as string;
                    bool result = ValidByObject(attrs[i] as AttributeValidPage, exVal, ref RtnMsg);
                    ////有錯誤，且為textbox
                    //if (!result && tmpObj != null)
                    //{
                    //    tmpObj.BackColor = System.Drawing.Color.Red;
                    //}
                    break;
                }
            }

        }

    }

    /// <summary>
    /// 傳入物件，依欄位自帶檢核項目執行
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="RtnMsg"></param>
    public void ValidVal<T>(T obj, ref List<string> RtnMsg, Page ctColl, string LineID)
    {
        Type v = obj.GetType();  //取的型別實體
        PropertyInfo[] props = v.GetProperties(); //取出所有公開屬性(可以被外部存取得 
        foreach (PropertyInfo prop in props)
        {
            object[] attrs = prop.GetCustomAttributes(true); //取得自訂屬性，第一個物件          

            CustTextBox tmpObj = null;
            //先找出有沒有控制項，且為CustTextBox
            for (int xi = 0; xi < attrs.Length; xi++)
            {
                //找出設定欄位屬性(非驗證)
                if (attrs[xi] is AttributeRfPage)
                {
                    AttributeRfPage authAttr = attrs[xi] as AttributeRfPage;
                    string ctrlID = authAttr.ControlID;
                    object controllS = this.FindControl(ctrlID + LineID);
                    if (controllS != null && controllS is CustTextBox)
                    {
                        //有找到，先把底色還原
                        tmpObj = controllS as CustTextBox;
                        //Talas 20190919 調整還原底色為default,非白色
                        //tmpObj.BackColor = System.Drawing.Color.White;
                        tmpObj.BackColor = default(System.Drawing.Color);
                    }
                }
            }
            for (int i = 0; i < attrs.Length; i++)
            {
                //確認型別為驗證用
                if (attrs[i] is AttributeValidPage)
                {
                    string exVal = prop.GetValue(obj, null) as string;
                    bool result = ValidByObject(attrs[i] as AttributeValidPage, exVal.Trim(), ref RtnMsg);
                    //有錯誤，且為textbox
                    if (!result && tmpObj != null)
                    {
                        tmpObj.BackColor = System.Drawing.Color.Red;
                    }
                    break;
                }
            }

        }

    }
    /// 作者 Ares Jack
    /// 創建日期：2021/11/23
    /// 修改日期：2021/11/23
    /// <summary>
    /// 傳入物件，依欄位自帶檢核項目執行(for自然人)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="RtnMsg"></param>
    public void ValidVal_NATURAL_PERSON<T>(T obj, ref List<string> RtnMsg, Page ctColl, string LineID)
    { 
        Type v = obj.GetType();  //取的型別實體
        PropertyInfo[] props = v.GetProperties(); //取出所有公開屬性(可以被外部存取得 

        foreach (PropertyInfo prop in props)
        {
            //20211123_Ares_Jack_自然人驗證下列項目
            if (prop.Name.ToString() == "CASE_NO" || prop.Name.ToString() == "CORP_NO" || prop.Name.ToString() == "NameCheck_No" || prop.Name.ToString() == "NameCheck_Item" || prop.Name.ToString() == "NameCheck_Note" || prop.Name.ToString() == "NameCheck_RiskRanking")
            {
                object[] attrs = prop.GetCustomAttributes(true); //取得自訂屬性，第一個物件          

                CustTextBox tmpObj = null;
                //先找出有沒有控制項，且為CustTextBox
                for (int xi = 0; xi < attrs.Length; xi++)
                {
                    //找出設定欄位屬性(非驗證)
                    if (attrs[xi] is AttributeRfPage)
                    {
                        AttributeRfPage authAttr = attrs[xi] as AttributeRfPage;
                        string ctrlID = authAttr.ControlID;
                        object controllS = this.FindControl(ctrlID + LineID);
                        if (controllS != null && controllS is CustTextBox)
                        {
                            //有找到，先把底色還原
                            tmpObj = controllS as CustTextBox;
                            //Talas 20190919 調整還原底色為default,非白色
                            //tmpObj.BackColor = System.Drawing.Color.White;
                            tmpObj.BackColor = default(System.Drawing.Color);
                        }
                    }
                }
                for (int i = 0; i < attrs.Length; i++)
                {
                    //確認型別為驗證用
                    if (attrs[i] is AttributeValidPage)
                    {
                        string exVal = prop.GetValue(obj, null) as string;
                        bool result = ValidByObject(attrs[i] as AttributeValidPage, exVal.Trim(), ref RtnMsg);
                        //有錯誤，且為textbox
                        if (!result && tmpObj != null)
                        {
                            tmpObj.BackColor = System.Drawing.Color.Red;
                        }
                        break;
                    }
                }
            }
        }
    }
    /// <summary>
    /// 透過輸入值取得物件
    /// </summary>
    /// <param name="ValidF"></param>
    /// <param name="strVal"></param>
    /// <param name="Msg"></param>
    private bool ValidByObject(AttributeValidPage ValidF, string strVal, ref List<string> Msg)
    {
        bool result = true;
        //依序檢核 空值
        if (ValidF.CanBeEmpty)
        {
            if (string.IsNullOrEmpty(strVal))
            {
                // Msg ValidF.sShowMsgEmpty + "不可空白";
                Msg.Add(ValidF.sShowMsgEmpty + "不可空白\\n");
                result = false;
            }
        }
        //長度檢核
        if (ValidF.isCheckLen)
        {
            if (!string.IsNullOrEmpty(strVal))
            {
                if (strVal.Length > ValidF.cLength)
                {
                    Msg.Add(ValidF.sShowMsgLen + "長度過長\\n");
                    result = false;
                }
            }

        }
        //字典檢核
        if (ValidF.isCheckDC)
        {    //初始化字典
            initBaseDc();
            //有值才去對，沒有就忽略
            if (!string.IsNullOrEmpty(strVal))
            {
                //額外處理高風險或高制裁
                switch (ValidF.sDC_Head)
                {
                    case "CT_NA_":

                        if (!BaseDc.ContainsKey("CT_12_" + strVal) && !BaseDc.ContainsKey("CT_13_" + strVal))
                        {
                            Msg.Add(ValidF.sShowMsgDC + "輸入值無效\\n");
                            result = false;
                        }
                        break;
                    ///數字限定
                    case "isNumeric":
                        if (!isNumeric(strVal))
                        {
                            Msg.Add(ValidF.sShowMsgDC + "必須為數字\\n");
                            result = false;
                        }
                        break;
                    ///日期格式限定 民國年
                    case "isROCDateTime":
                        DateTime ROCdt = DateTime.Now;
                        if (strVal.Length != 7)
                        {
                            Msg.Add(ValidF.sShowMsgDC + "必須為民國年格式\\n");
                            result = false;
                            return result;
                        }
                        strVal = strVal.PadLeft(8, '0');
                        strVal = strVal.Substring(0, 4) + "/" + strVal.Substring(4, 2) + "/" + strVal.Substring(6, 2);
                        if (DateTime.TryParse(strVal, out ROCdt))
                        {
                            Msg.Add(ValidF.sShowMsgDC + "必須為民國年格式\\n");
                            result = false;
                        }

                        break;
                    ///日期格式限定
                    case "isDateTime":
                        if (strVal.Length != 8)
                        {
                            Msg.Add(ValidF.sShowMsgDC + "必須為西元年格式\\n");
                            result = false;
                            return result;
                        }
                        DateTime DCdt = DateTime.Now;
                        strVal = strVal.Substring(0, 4) + "/" + strVal.Substring(4, 2) + "/" + strVal.Substring(6, 2);
                        if (!DateTime.TryParse(strVal, out DCdt))
                        {
                            Msg.Add(ValidF.sShowMsgDC + "必須為西元年日期格式\\n");
                            result = false;
                        };
                        break;
                    default:
                        if (!BaseDc.ContainsKey(ValidF.sDC_Head + strVal))
                        {
                            Msg.Add(ValidF.sShowMsgDC + "輸入值無效\\n");
                            result = false;
                        }
                        break;
                }
            }
        }
        return result;
    }

    #endregion

    #region 映射取值，映射設定值


    /// <summary>
    /// 以映射方式透過物件中對應Property設定控制項的值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    public void SetVal<T>(T obj, bool isReset)
    {
        Type v = obj.GetType();  //取的型別實體
        PropertyInfo[] props = v.GetProperties(); //取出所有公開屬性(可以被外部存取得 
        string LineID = "";  //若有分行，則用來帶出LINEID
        if (typeof(IMutiLineClass).IsAssignableFrom(typeof(T)))
        {
            //讀取MutiLine設定值
            IMutiLineClass ut = obj as IMutiLineClass;
            if (ut.isMutilime)
            {
                LineID = ut.InterFaceLineID;
            }
        }
        foreach (PropertyInfo prop in props)
        {
            object[] attrs = prop.GetCustomAttributes(true); //取得自訂屬性，第一個物件
            AttributeRfPage authAttr;
            for (int xi = 0; xi < attrs.Length; xi++)
            {
                if (attrs[xi] is AttributeRfPage)
                {
                    authAttr = attrs[xi] as AttributeRfPage;
                    string propName = prop.Name;
                    string authID = authAttr.ControlID;
                    string fType = authAttr.FieldType;
                    string exVal = prop.GetValue(obj, null) as string;
                    bool isMutl = authAttr.IsMutip;
                    if (isMutl)  //多重物件處理，目前限Chechbox.Radio
                    {
                        SetMapValMuti(authID, fType, exVal, LineID, isReset);
                    }
                    else
                    {
                        SetMapVal(authID, fType, exVal, LineID, isReset);
                    }
                }
            }
        }
    }
    /// <summary>
    /// 以映射方式透過設定取得控制項的值回填物件中對應Property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    public void GetVal<T>(ref T obj)
    {
        Type v = obj.GetType();  //取的型別實體
        PropertyInfo[] props = v.GetProperties(); //取出所有公開屬性(可以被外部存取得 
        string LineID = ""; //若有分行，則用來帶出LINEID
        if (typeof(IMutiLineClass).IsAssignableFrom(typeof(T)))
        {
            //讀取MutiLine設定值
            IMutiLineClass ut = obj as IMutiLineClass;
            if (ut.isMutilime)
            {
                LineID = ut.InterFaceLineID.ToString();
            }
        }
        foreach (PropertyInfo prop in props)
        {
            object[] attrs = prop.GetCustomAttributes(true); //取得自訂屬性，第一個物件
            AttributeRfPage authAttr;
            for (int xi = 0; xi < attrs.Length; xi++)
            {
                if (attrs[xi] is AttributeRfPage)
                {
                    authAttr = attrs[xi] as AttributeRfPage;
                    string propVla = "";
                    string propName = prop.Name;
                    string authID = authAttr.ControlID;
                    string fType = authAttr.FieldType;

                    bool isMutl = authAttr.IsMutip;
                    if (isMutl)  //多重物件處理，目前限Chechbox.Radio
                    {
                        propVla = GetMapValMuti(authID, fType, LineID);
                    }
                    else
                    {
                        propVla = GetMapVal(authID, fType, LineID);
                    }
                    if (propVla != null)  //有取到值
                    {
                        prop.SetValue(obj, propVla, null);
                    }

                }
            }

        }
    }


    /// <summary>
    /// 將值映射給控制項 只處理多重 CheckBox,RadioButton
    /// </summary>
    /// <param name="authID"></param>
    /// <param name="fType"></param>
    /// <param name="exVal"></param>
    /// <param name="LineID"></param>
    /// <param name="isReset"></param>
    private void SetMapValMuti(string authID, string fType, string exVal, string LineID, bool isReset)
    {
        switch (fType)
        {
            case "RadioButton":
                string[] controlColl = authID.Split(',');
                foreach (string item in controlColl)
                {
                    object controll = this.FindControl(item + LineID);
                    if (controll == null) { return; }
                    RadioButton rbl = controll as RadioButton;
                    if (rbl.Text == exVal)
                    {
                        rbl.Checked = true;
                    }
                    //清除，則給予預設不選
                    if (isReset) { rbl.Checked = false; }
                }
                break;
            case "CheckBox":
                string[] controlChkColl = authID.Split(',');  //控制項數
                string[] keyVal = exVal.Split(',');           //欄位值
                //if (controlChkColl.Length != keyVal.Length) //欄位與值數量不匹配時，不處理
                //{
                //    return;
                //}
                int i = 0;
                foreach (string item in controlChkColl)
                {
                    object controll = this.FindControl(item + LineID);
                    if (controll == null) { return; }
                    CheckBox ckll = controll as CheckBox;
                    ckll.Checked = keyVal[i] == "1" ? true : false;
                    i++;
                    //清除，則給予預設不選
                    if (isReset) { ckll.Checked = false; }
                }
                break;
            case "CustCheckBox":
                string[] CustcontrolChkColl = authID.Split(',');  //控制項數
                string[] CustkeyVal = exVal.Split(',');           //欄位值
                //if (CustcontrolChkColl.Length != CustkeyVal.Length) //欄位與值數量不匹配時，不處理
                //{
                //    return;
                //}
                int Custi = 0;
                foreach (string item in CustcontrolChkColl)
                {
                    object controll = this.FindControl(item + LineID);
                    if (controll == null) { return; }
                    CustCheckBox ckll = controll as CustCheckBox;
                    ckll.Checked = CustkeyVal[Custi] == "1" ? true : false;
                    Custi++;
                    //清除，則給予預設不選
                    if (isReset) { ckll.Checked = false; }
                }
                break;
        }
    }

    /// <summary>
    /// 將值映射給控制項
    /// </summary>
    /// <param name="auth"></param>
    /// <param name="fType"></param>
    /// <param name="exVal"></param>
    /// <param name="LineID"></param>
    /// <param name="isRese"></param>
    private void SetMapVal(string auth, string fType, string exVal, string LineID, bool isReset)
    {

        object controllS = this.FindControl(auth + LineID);
        if (controllS == null)
        { return; }
        object controll = controllS;
        //清除，則給予預設不填
        if (isReset) { exVal = ""; }
        switch (fType)
        {
            case "CustHiddenField":
                CustHiddenField hdnText = controll as CustHiddenField; //轉型
                hdnText.Value = exVal;
                break;
            case "Label":

                Label lblText = controll as Label; //轉型
                lblText.Text = exVal;
                break;
            case "CustLabel":

                CustLabel CuslblText = controll as CustLabel; //轉型
                CuslblText.Text = exVal;
                break;
            case "TextBox":
                TextBox txtText = controll as TextBox; //轉型
                txtText.Text = exVal;
                break;
            case "CustTextBox":
                CustTextBox CusttxtText = controll as CustTextBox; //轉型
                CusttxtText.Text = exVal;
                //Talas 20190919 調整還原底色為default,非白色
                //CusttxtText.BackColor = System.Drawing.Color.White; //設新值，順便變回原色
                CusttxtText.BackColor = default(System.Drawing.Color);
                break;

            case "CustCheckBox":
                CustCheckBox ckll = controll as CustCheckBox;
                ckll.Checked = exVal == "1" ? true : false;
                break;
            case "DropDownList":
                DropDownList comB = controll as DropDownList;//轉型
                comB.SelectedValue = exVal;
                break;
            case "CustDropDownList":
                CustDropDownList CustcomB = controll as CustDropDownList;//轉型
                CustcomB.SelectedValue = exVal;
                break;
        }

    }
    private string GetMapValMuti(string authID, string fType, string LineID)
    {
        string retVal = "";
        switch (fType)
        {
            case "RadioButton":
                string[] controlColl = authID.Split(',');
                foreach (string item in controlColl)
                {
                    object controll = this.FindControl(item + LineID);
                    if (controll == null) { return null; } //找不到對應物件，應以NULL回傳，讓取值可以判斷不更新原始值

                    RadioButton rbl = controll as RadioButton;
                    if (rbl.Checked == true)
                    {
                        retVal = rbl.Text;
                    }
                }
                break;
            case "CheckBox":
                string[] controlChkColl = authID.Split(',');  //控制項數

                int i = 0;
                foreach (string item in controlChkColl)
                {
                    object controll = this.FindControl(item + LineID);
                    if (controll == null) { return null; } //找不到對應物件，應以NULL回傳，讓取值可以判斷不更新原始值
                    CheckBox ckll = controll as CheckBox;
                    if (ckll.Checked == true)
                    {
                        retVal += "1,";
                    }
                    else
                    {
                        retVal += "0,";
                    }
                    i++;
                }
                //移除最後的，
                retVal = retVal.Remove(retVal.Length - 1, 1);

                break;
        }
        return retVal;
    }
    /// <summary>
    /// 將值映射給控制項
    /// </summary>
    /// <param name="auth"></param>
    /// <param name="fType"></param>
    /// <param name="exVal"></param>
    private string GetMapVal(string auth, string fType, string LineID)
    {
        string retVal = "";
        object controllS = this.FindControl(auth + LineID);
        if (controllS == null)
        { return null; } //找不到對應物件，應以NULL回傳，讓取值可以判斷不更新原始值
        object controll = controllS;
        switch (fType)
        {
            case "CustHiddenField":
                CustHiddenField hdnText = controll as CustHiddenField; //轉型
                retVal = hdnText.Value;
                break;
            case "Label":
                Label lblText = controll as Label; //轉型
                retVal = lblText.Text;
                break;
            case "CustLabel":

                CustLabel CuslblText = controll as CustLabel; //轉型
                retVal = CuslblText.Text;
                break;
            case "TextBox":
                TextBox txtText = controll as TextBox; //轉型
                retVal = txtText.Text;
                break;
            case "ComboBox":
                DropDownList comB = controll as DropDownList;//轉型
                retVal = comB.SelectedItem.ToString();
                break;
            case "CustCheckBox":
                CustCheckBox ckll = controll as CustCheckBox;
                retVal = ckll.Checked == true ? "1" : "0";
                break;
            case "CustTextBox":
                CustTextBox CusttxtText = controll as CustTextBox; //轉型
                retVal = CusttxtText.Text;
                break;
            case "CustDropDownList":
                CustDropDownList CustcomB = controll as CustDropDownList;//轉型
                retVal = CustcomB.SelectedItem.ToString();
                break;
        }
        return retVal;
    }
    /// <summary>
    /// 民國年轉換
    /// </summary>
    /// <param name="sDate"></param>
    /// <returns></returns>
    public string ConvertToROCYear(string sDate)
    {
        string result = sDate;
        if (sDate.Length == 8)
        {
            try
            {
                /*
                //Tals 20190925 修正 只調整輸入年，不做轉型
                int nYear = int.Parse(sDate.Substring(0, 4));
                nYear -= 1911; //減去民國與西元差1911
                result = nYear + sDate.Substring(4, 2) + sDate.Substring(6, 2);
                */

                //string sD = sDate.Substring(0, 4) + "-" + sDate.Substring(4, 2) + "-" + sDate.Substring(6, 2);
                //DateTime Ds = Convert.ToDateTime(sD);
                //Ds = Ds.AddYears(-1911);
                //result = Ds.ToString("yyyMMdd");

                if (!sDate.Trim().Equals("00000000"))
                {
                    DateTime dt1 = new DateTime(Convert.ToInt32(sDate.Substring(0, 4)), Convert.ToInt32(sDate.Substring(4, 2)), Convert.ToInt32(sDate.Substring(6, 2)));

                    CultureInfo culture = new CultureInfo("ZH-TW");
                    culture.DateTimeFormat.Calendar = new TaiwanCalendar();

                    TaiwanCalendar tc = new TaiwanCalendar();
                    result = string.Format("{0}{1}{2}", tc.GetYear(dt1).ToString().PadLeft(3, '0'), dt1.Month.ToString().PadLeft(2, '0'), dt1.Day.ToString().PadLeft(2, '0'));
                }
            }
            catch (Exception ex)
            {
                ///不寫LOG
                string res = ex.Message;
            }
        }
        return result;
    }
    /// <summary>
    /// 西元年轉換
    /// </summary>
    /// <param name="sDate"></param>
    /// <returns></returns>
    public string ConvertToDC(string sDate)
    {
        string result = sDate;
        if (sDate.Length == 7)
        {
            try
            {
                /*
                //Tals 20190925 修正民國年轉換閏年問題 應先將民國年+1911後再回組字串
                //補滿8碼
                sDate = sDate.PadLeft(8, '0');
                //轉換前四碼，加上1911
                int nYear = int.Parse(sDate.Substring(0, 4));
                nYear += 1911; //補上民國與西元差1911
                //組回八碼回傳
                result = nYear.ToString() + sDate.Substring(4, 2) + sDate.Substring(6, 2);
                */
                
                CultureInfo culture = new CultureInfo("ZH-TW");
                culture.DateTimeFormat.Calendar = new TaiwanCalendar();
                result = DateTime.ParseExact(sDate.PadLeft(8, '0'), "yyyyMMdd", culture).ToString("yyyyMMdd");

            }
            catch (Exception ex)
            {
                ///不寫LOG
                string res = ex.Message;
            }
        }
        return result;
    }

    //20191029 
    ///檢查輸入日期是否合規
    ///</summary>
    ///<param name="input">任一字元串</param>
    ///<returns>全形字元串</returns>
    public bool checkDateTime(string _dateTime)
    {
        string result = string.Empty;
        try
        {
            if (_dateTime.Length == 7)
            {
                CultureInfo culture = new CultureInfo("ZH-TW");
                culture.DateTimeFormat.Calendar = new TaiwanCalendar();
                result = DateTime.ParseExact(_dateTime.PadLeft(8, '0'), "yyyyMMdd", culture).ToString("yyyyMMdd");
            }

            if (_dateTime.Length == 8)
            {
                if (!_dateTime.Trim().Equals("00000000"))
                {
                    DateTime dt1 = new DateTime(Convert.ToInt32(_dateTime.Substring(0, 4)), Convert.ToInt32(_dateTime.Substring(4, 2)), Convert.ToInt32(_dateTime.Substring(6, 2)));

                    CultureInfo culture = new CultureInfo("ZH-TW");
                    culture.DateTimeFormat.Calendar = new TaiwanCalendar();

                    TaiwanCalendar tc = new TaiwanCalendar();
                    result = string.Format("{0}{1}{2}", tc.GetYear(dt1).ToString().PadLeft(3, '0'), dt1.Month.ToString().PadLeft(2, '0'), dt1.Day.ToString().PadLeft(2, '0'));
                }
            }
            return true;
        }
        catch (Exception e)
        {
            string a = e.Message;
            return false;
        }
    }
    //20191029 
    ///檢查輸入日期是否合規
    ///</summary>
    ///<param name="_dateTime">輸入字串</param>
    ///<param name="_Type">C:民國年 V:西元年</param>
    ///<returns>全形字元串</returns>
    public bool checkDateTime(string _dateTime, string _Type)
    {
        string result = string.Empty;
        try
        {
            //民國年的字串檢核
            if (_Type.Trim().Equals("C"))
            {
                CultureInfo culture = new CultureInfo("ZH-TW");
                culture.DateTimeFormat.Calendar = new TaiwanCalendar();
                result = DateTime.ParseExact(_dateTime.PadLeft(8, '0'), "yyyyMMdd", culture).ToString("yyyyMMdd");
            }

            //西元年的字串檢核
            if (_Type.Trim().Equals("V"))
            {
                if (!_dateTime.Trim().Equals("00000000"))
                {
                    DateTime dt1 = new DateTime(Convert.ToInt32(_dateTime.Substring(0, 4)), Convert.ToInt32(_dateTime.Substring(4, 2)), Convert.ToInt32(_dateTime.Substring(6, 2)));

                    CultureInfo culture = new CultureInfo("ZH-TW");
                    culture.DateTimeFormat.Calendar = new TaiwanCalendar();

                    TaiwanCalendar tc = new TaiwanCalendar();
                    result = string.Format("{0}{1}{2}", tc.GetYear(dt1).ToString().PadLeft(3, '0'), dt1.Month.ToString().PadLeft(2, '0'), dt1.Day.ToString().PadLeft(2, '0'));
                }
            }
            return true;
        }
        catch (Exception e)
        {
            string a = e.Message;
            return false;
        }
    }

    ///字串轉全形
    ///</summary>
    ///<param name="input">任一字元串</param>
    ///<returns>全形字元串</returns>
    public string ToWide(string input)
    {
        //半形轉全形：
        char[] c = input.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            //全形空格為12288，半形空格為32
            if (c[i] == 32)
            {
                c[i] = (char)12288;
                continue;
            }
            //其他字元半形(33-126)與全形(65281-65374)的對應關係是：均相差65248
            if (c[i] < 127)
                c[i] = (char)(c[i] + 65248);
        }
        return new string(c);
    }

    ///<summary>
    ///字串轉半形
    ///</summary>
    ///<paramname="input">任一字元串</param>
    ///<returns>半形字元串</returns>
    public string ToNarrow(string input)
    {
        char[] c = input.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] == 12288)
            {
                c[i] = (char)32;
                continue;
            }
            if (c[i] > 65280 && c[i] < 65375)
                c[i] = (char)(c[i] - 65248);
        }
        return new string(c);
    }

    /// <summary>
    /// 檢驗是否為指定的羅馬拼音字元或符號
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public bool ValidRoma(string input)
    {
        bool result = true;
        char[] valArr = "　ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＺＹａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ．：’‧˙".ToCharArray();
        char[] chars = input.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (Array.IndexOf(valArr, chars[i]) == -1)
            {
                result = false;
                break;
            }
        }

        return result;
    }
    #endregion
    #region 文檔上傳
    /// <summary>
    /// 文件上傳功能
    /// </summary>
    /// <param name="hpfUploadFile">客戶端要上傳的文檔</param>
    /// <param name="strMsgID">返回出錯MessageID</param>
    /// <returns>返回值：""----上傳不成功；其他：上傳文檔服務器路徑及文檔名稱</returns>
    protected string FileUpload(HttpPostedFile hpfUploadFile, ref string strMsgID)
    {
        string strServerPath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("UpLoadFilePath");


        // Determine whether the directory exists.	
        if (!Directory.Exists(strServerPath))
        {
            // Create the directory it does not exist.
            Directory.CreateDirectory(strServerPath);
        }

        strServerPath = strServerPath + "\\" + hpfUploadFile.FileName.Substring(hpfUploadFile.FileName.LastIndexOf("\\") + 1);
        // 檢查文檔是否已經存在
        if (File.Exists(strServerPath))
        {
            File.Delete(strServerPath);
        }

        try
        {
            hpfUploadFile.SaveAs(strServerPath);
            return strServerPath; // 上傳成功      
        }
        catch
        {
            strMsgID = "00_00000000_037";
        }
        return "";
    }
    /// <summary>
    /// 初始化字典
    /// </summary>
    private void initBaseDc()
    {
        if (BaseDc.Count > 0)
        {
            return;
        }
        //限定只能是Y/N  LM開頭 
        BaseDc.Add("LM_YN_Y", "");
        BaseDc.Add("LM_YN_N", "");
        //限定只能是1-0
        BaseDc.Add("LM_10_1", "");
        BaseDc.Add("LM_10_0", "");
        //限定只能是1-2
        BaseDc.Add("LM_12_1", "");
        BaseDc.Add("LM_12_2", "");

        //限定只能是1-4
        BaseDc.Add("LM_14_1", "");
        BaseDc.Add("LM_14_2", "");
        BaseDc.Add("LM_14_3", "");
        BaseDc.Add("LM_14_4", "");


        //值轉換，以TS開頭
        // 通用項目
        BaseDc.Add("TS_YN_1", "是");
        BaseDc.Add("TS_YN_0", "否");
        BaseDc.Add("TS_YN_Y", "是");
        BaseDc.Add("TS_YN_N", "否");
        BaseDc.Add("TS_HS_1", "有");
        BaseDc.Add("TS_HS_0", "無");
        BaseDc.Add("TS_HS_Y", "有");
        BaseDc.Add("TS_HS_N", "無");

        BaseDc.Add("TS_YN1_是", "1");
        BaseDc.Add("TS_YN1_否", "0");
        BaseDc.Add("TS_YNY_是", "Y");
        BaseDc.Add("TS_YNY_否", "N");
        BaseDc.Add("TS_HS1_有", "1");
        BaseDc.Add("TS_HS1_無", "0");
        BaseDc.Add("TS_HSY_有", "Y");
        BaseDc.Add("TS_HSY_無", "N");
        //開關 OC_ 
        BaseDc.Add("TS_OC_O", "OPEN");
        BaseDc.Add("TS_OC_C", "Close");
        BaseDc.Add("TS_OC_OPEN", "O");
        BaseDc.Add("TS_OC_Close", "C");


        //由CodeType來，CT_codetyepe_Code
        setFromCodeType("1"); //國家
        setFromCodeType("2");  //法律形式
        setFromCodeType("3");  //行業編號
        setFromCodeType("4");  // 領補換類別
        setFromCodeType("5");  //名單掃描結果項目
        setFromCodeType("6");  //組織運作
        setFromCodeType("7");  //存在證明
        setFromCodeType("8");  //美國州別

        //追加NA，        
        //BaseDc.Add("CT_8_NA", "");//20201021 mark BY Peggy

        setFromCodeType("10");  //身分類型
        setFromCodeType("11");  //高風險行業別
        setFromCodeType("12"); //高風險國家 
        setFromCodeType("13");// 高制裁國家
        //若同時須檢核高風險或高制裁，給予特殊KEY  CT_NA_

        //身分證件類型 1身分證 3護照 4居留證 7其他
        BaseDc.Add("CT_99_1", "身分證");
        BaseDc.Add("CT_99_3", "護照");
        // 20200410-RQ-2019-030155-005-居留證號更名為統一證號
        //BaseDc.Add("CT_99_4", "居留證");
        BaseDc.Add("CT_99_4", "統一證號");
        BaseDc.Add("CT_99_7", "其他");
    }
    /// <summary>
    /// 將指定CODETYPE鍵入字典中
    /// </summary>
    /// <param name="codeType"></param>
    private void setFromCodeType(string codeType)
    {
        DataTable result = BRPostOffice_CodeType.GetCodeType(codeType);
        if (result != null && result.Rows.Count > 0)
        {
            for (int i = 0; i < result.Rows.Count; i++)
            {
                string sKey = "CT_" + codeType + "_" + result.Rows[i][0].ToString();
                if (!BaseDc.ContainsKey(sKey))
                {
                    BaseDc.Add("CT_" + codeType + "_" + result.Rows[i][0].ToString(), result.Rows[i][1].ToString());
                }
            }
        }
    }
    /// <summary>
    /// 檢查傳入文字是否為數字
    /// </summary>
    /// <param name="inText"></param>
    /// <returns></returns>
    protected bool isNumeric(string inText)
    {
        bool result = false;

        Regex NumberPattern = new Regex("[^0-9.-]");
        result = !NumberPattern.IsMatch(inText);

        return result;

    }

    //判斷輸入的羅馬姓名若為x 則清空欄位值
    public string LongNameRomaClean(string _Roma)
    {
        _Roma = ToWide(_Roma.Trim());
        if (_Roma.Trim().Equals("Ｘ") || _Roma.Trim().Equals("ｘ"))
            return "";
        else
            return _Roma.Trim();
    }
    #endregion

    ///修改日期：2019/09/09
    ///修改人：陳香琦
    /// <summary>
    /// 檢核PostOffice_CodeType類別是否啟用
    /// </summary>
    /// <param name="_Type">類別</param>
    /// <param name="text">檢核代碼</param>
    public string checkCodeType(string _Type, string _control, bool needReturn, string _codeName)
    {
        CustTextBox CName = FindControl(_control.Trim()) as CustTextBox; //觸發的元件
        CustLabel CodeName = FindControl(_codeName.Trim()) as CustLabel; //接收回傳codename的顯示欄位，若無即帶空白

        DataTable result = BRPostOffice_CodeType.GetCodeTypeByCodeID(_Type, CName.Text.Trim());

        string _RetuenValue = string.Empty;
        if ((result.Rows.Count > 0 && CheckIsValid(result)) 
            || CName.Text.Trim().Equals(""))
        {
            if (needReturn)//是否需要回傳中文
                CodeName.Text = result.Rows[0]["CODE_NAME"].ToString();

            _RetuenValue = "";
            CName.BackColor = System.Drawing.Color.White;
        }
        else
        {
            CName.BackColor = System.Drawing.Color.Red;
            _RetuenValue = "error";
        }

        return _RetuenValue;
    }

    private bool CheckIsValid(DataTable dt)
    {
        bool result = true;
        bool? isValid = dt.Rows[0]["IsValid"] as bool?;
        if (isValid == null)
        {
            result = false;
        }
        else
        {
            if (isValid == false)
                result = false;
        }
        return result;
    }

    #region 20201211RC 舊/新式居留證號檢核
    //20200513-RQ-2019-030155-000 居留證號檢核
    /// <summary>
    /// 檢核中華民國外僑及大陸人士在台居留證(舊式+新式)
    /// </summary>
    /// <param name="idNo">身分證</param>
    /// <returns></returns>
    public bool CheckResidentID(string idNo)
    {
        if (idNo == null)
        {
            return false;
        }
        idNo = idNo.ToUpper();
        Regex regex = new Regex(@"^([A-Z])(A|B|C|D|8|9)(\d{8})$");
        Match match = regex.Match(idNo);
        if (!match.Success)
        {
            return false;
        }

        if ("ABCD".IndexOf(match.Groups[2].Value) >= 0)
        {
            //舊式
            return CheckOldResidentID(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value);
        }
        else
        {
            //新式(2021/01/02)正式生效
            return CheckNewResidentID(match.Groups[1].Value, match.Groups[2].Value + match.Groups[3].Value);
        }
    }

    /// <summary>
    /// 舊式檢核
    /// </summary>
    /// <param name="firstLetter">第1碼英文字母(區域碼)</param>
    /// <param name="secondLetter">第2碼英文字母(性別碼)</param>
    /// <param name="num">第3~9流水號 + 第10碼檢查碼</param>
    /// <returns></returns>
    private bool CheckOldResidentID(string firstLetter, string secondLetter, string num)
    {
        ///建立字母對應表(A~Z)
        ///A=10 B=11 C=12 D=13 E=14 F=15 G=16 H=17 J=18 K=19 L=20 M=21 N=22
        ///P=23 Q=24 R=25 S=26 T=27 U=28 V=29 X=30 Y=31 W=32  Z=33 I=34 O=35         
        string alphabet = "ABCDEFGHJKLMNPQRSTUVXYWZIO";
        
        string transferIdNo = Convert.ToString(alphabet.IndexOf(firstLetter) + 10).Trim()+ Convert.ToString((alphabet.IndexOf(secondLetter) + 10) % 10).Trim() + num;
        int[] idNoArray = new int[transferIdNo.Length];
        
        int sum = 0;
        int[] weight = new int[] { 1,9, 8, 7, 6, 5, 4, 3, 2, 1, 1 };
        for (int i = 0; i < weight.Length; i++)
        {
            sum += weight[i] * idNoArray[i];
        }
        return (sum % 10 == 0);
        
    }

    /// <summary>
    /// 新式檢核
    /// </summary>
    /// <param name="firstLetter">第1碼英文字母(區域碼)</param>
    /// <param name="num">第2碼(性別碼) + 第3~9流水號 + 第10碼檢查碼</param>
    /// <returns></returns>
    private bool CheckNewResidentID(string firstLetter, string num)
    {        
        ///建立字母對應表(A~Z)
        ///A=10 B=11 C=12 D=13 E=14 F=15 G=16 H=17 J=18 K=19 L=20 M=21 N=22
        ///P=23 Q=24 R=25 S=26 T=27 U=28 V=29 X=30 Y=31 W=32  Z=33 I=34 O=35 
        string alphabet = "ABCDEFGHJKLMNPQRSTUVXYWZIO";
        string transferIdNo = Convert.ToString(alphabet.IndexOf(firstLetter) + 10).Trim() + num;
        int[] idNoArray = new int[transferIdNo.Length];
        
        for (int y = 0; y < transferIdNo.Length; y++)
        {
            idNoArray[y] = Convert.ToInt32(transferIdNo.Trim().Substring(y, 1));
        }

        int sum = 0;
        int[] weight = new int[] { 1, 9, 8, 7, 6, 5, 4, 3, 2, 1, 1 };
        for (int i = 0; i < weight.Length; i++)
        {
            sum += (weight[i] * idNoArray[i]) % 10;
        }

        return (sum % 10 == 0);
    }
    /// <summary>
    /// 檢核高階管理人虛擬ID
    /// </summary>
    /// <param name="idNo">身分證</param>
    /// <returns></returns>
    public bool CheckResidentID_SeniorManager(string idNo, string idType)
    {
        if (idNo == null)
        {
            return false;
        }
        if(idType == null)
        {
            return false;
        }
        //先檢核是否為虛擬ID
        idNo = idNo.ToUpper();
        Regex regexVirtualID = new Regex(@"^CA(?:(?:(?:(?:(?:[13579][26]|[2468][048])00)|(?:[0-9]{2}(?:(?:[13579][26])|(?:[2468][048]|0[48]))))(?:(?:(?:09|04|06|11)(?:0[1-9]|1[0-9]|2[0-9]|30))|(?:(?:01|03|05|07|08|10|12)(?:0[1-9]|1[0-9]|2[0-9]|3[01]))|(?:02(?:0[1-9]|1[0-9]|2[0-9]))))|(?:[0-9]{4}(?:(?:(?:09|04|06|11)(?:0[1-9]|1[0-9]|2[0-9]|30))|(?:(?:01|03|05|07|08|10|12)(?:0[1-9]|1[0-9]|2[0-9]|3[01]))|(?:02(?:[01][0-9]|2[0-8])))))[A-Z]{2}[A-Za-z0-9]{2}$");
        Match matchVirtualID = regexVirtualID.Match(idNo);
        if (!matchVirtualID.Success)
        {
            return false;
        }
        return true;
    }
    #endregion

    /// <summary>
    /// 檢核操作瀏覽器
    /// (避免分頁多開導致Session異常)
    /// </summary>
    protected void CensorPage()
    {
        string strUrlErrorIframe = UtilHelper.GetAppSettings("ERROR_IFRAME").ToString();

        try
        {
            if (!IsPostBack)
            {
                String usrBrowser = Request.Browser.Browser;
                String GUID = Guid.NewGuid().ToString();

                HttpContext.Current.Session["usrBrowser"] = usrBrowser;
                this.ViewState["usrBrowser"] = usrBrowser;

                HttpContext.Current.Session["usrGUID"] = GUID;
                this.ViewState["usrGUID"] = GUID;
            }
            else
            {
                //檢核操作瀏覽器是否相符
                if (this.ViewState["usrBrowser"].Equals(Session["usrBrowser"]))
                {
                    //檢核KEY是否相符
                    if (!Session["usrGUID"].Equals(this.ViewState["usrGUID"]))
                    {
                        //20210412_Ares_Stanley-調整轉向方法
                        Response.Redirect(strUrlErrorIframe, false);
                        //jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsg) + "');var local = window.parent.location!=window.location?window.parent:window.opener?window.opener.parent:window;local.location.href='" + strUrlError2 + "';");
                        return;
                    }
                }
            }
        }
        catch (ThreadAbortException taex)
        {
            Logging.Log(taex, LogLayer.UI);
            //20210412_Ares_Stanley-調整轉向方法
            Response.Redirect(strUrlErrorIframe, false);
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //20210412_Ares_Stanley-調整轉向方法
            Response.Redirect(strUrlErrorIframe, false);
        }
    }

    /// <summary>
    /// 檢查最後異動分行
    /// </summary>
    /// <param name="lastUPDBranch"></param>
    /// <returns></returns>
    public bool checkLAS_UPD_BRANCH(string lastUPDBranch)
    {
        bool isMatch = false;
        //string[] branchList = { "0049", "9999", "MFA1", "MFA2", "MFA3", "MFA4", "MFA5", "MFA6", "MFA7", "MFA8", "MFA9" };
        //20210917 by Ares Jack 改為讀取資料庫檢查最後異動分行
        DataTable branchList = new DataTable();
        branchList = BRPostOffice_CodeType.GetCodeType("22");
        DataView dv = branchList.DefaultView;
        dv.Sort = "CODE_ID";
        branchList = dv.ToTable();
        
        foreach (DataRow dr in branchList.Rows)
        {
            if (lastUPDBranch.ToUpper().Trim() == dr[0].ToString().Trim())
            {
                isMatch = true;
                break;
            }
        }
        return isMatch;
    }
    /// 作者 Ares Jack
    /// 創建日期：2021/11/22
    /// 修改日期：2021/11/22
    /// <summary>
    /// 檢查MAKER
    /// </summary>
    /// <param name="lastUPDMAKER"></param>
    /// <param name="lastUPDBranch"></param>
    /// <returns></returns>
    public bool checkLAS_UPD_MAKER(string lastUPDMAKER)
    {
        bool isMatch = false;
        isMatch = BRPostOffice_CodeType.GetMFA_ID(lastUPDMAKER);

        return isMatch;
    }
    /// 作者 Ares Jack
    /// 創建日期：2021/11/22
    /// 修改日期：2021/11/23
    /// <summary>
    /// 檢查CHECKER
    /// </summary>
    /// <param name="lastUPDCHECKER"></param>
    /// <returns></returns>
    public bool checkLAS_UPD_CHECKER(string lastUPDCHECKER)
    {
        bool isMatch = false;
        isMatch = BRPostOffice_CodeType.GetMFA_ID(lastUPDCHECKER);

        return isMatch;
    }

    /// <summary>
    /// 檢查地址郵遞區號
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    public bool checkREG_ZIP_CODE(string address)
    {
        EntitySet<EntitySZIP> SZIPSet = BRSZIP.SelectEntitySet(address);

        if (SZIPSet != null && SZIPSet.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 資料檔AML_CHECKLOG中寫入一筆記錄
    /// </summary>
    public void InsertAMLCheckLog(EntityAML_CHECKLOG eAMLCheckLog)
    {        
        try
        {
            BRAML_CHECKLOG.AddEntity(eAMLCheckLog);
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.Entity);
        }
    }

    /// <summary>
    /// 檢核相關欄位是否有異動
    /// </summary>
    /// <param name="htReturn">主機資料</param>
    /// <param name="htInput">上送主機資料</param>
    /// <param name="fieldName">欄位名稱</param>
    /// <param name="isChanged">是否有異動</param>
    public void compareForAMLCheckLog(Hashtable htReturn, Hashtable htInput, string fieldName, ref bool isChanged)
    {
        if (!isChanged && htReturn[fieldName] != null && htInput[fieldName] != null)
        {
            if(htReturn[fieldName].ToString().Trim() != htInput[fieldName].ToString().Trim())
            {
                isChanged = true;
            }
        }
    }
    /// <summary>
    /// 檢核相關欄位是否有異動
    /// </summary>
    /// <param name="htReturn">主機資料</param>
    /// <param name="htInput">上送主機資料</param>
    /// <param name="fieldName">欄位名稱</param>
    /// <param name="isChanged">是否有異動</param>
    public void compareForAMLCheckLog(Hashtable htReturn, Hashtable htInput, string fieldName, ref bool isChanged, ref List<string> changedFileds)
    {
        //if (!isChanged && htReturn[fieldName] != null && htInput[fieldName] != null)
        //20211209_Ares_Jack_每一筆欄位異動都要寫進customer_log
        if (htReturn[fieldName] != null && htInput[fieldName] != null)
        {
            if (htReturn[fieldName].ToString().Trim() != htInput[fieldName].ToString().Trim())
            {
                changedFileds.Add(fieldName);
                isChanged = true;
            }
        }
    }
    /// <summary>
    /// 檢核虛擬ID
    /// </summary>
    /// <param name="idNo">身分證</param>
    /// <returns></returns>
    public bool checkVirutalID(string idNo)
    {
        if (idNo == null)
        {
            return false;
        }
        idNo = idNo.ToUpper();
        Regex regex = new Regex(@"^CA(?:(?:(?:(?:(?:[13579][26]|[2468][048])00)|(?:[0-9]{2}(?:(?:[13579][26])|(?:[2468][048]|0[48]))))(?:(?:(?:09|04|06|11)(?:0[1-9]|1[0-9]|2[0-9]|30))|(?:(?:01|03|05|07|08|10|12)(?:0[1-9]|1[0-9]|2[0-9]|3[01]))|(?:02(?:0[1-9]|1[0-9]|2[0-9]))))|(?:[0-9]{4}(?:(?:(?:09|04|06|11)(?:0[1-9]|1[0-9]|2[0-9]|30))|(?:(?:01|03|05|07|08|10|12)(?:0[1-9]|1[0-9]|2[0-9]|3[01]))|(?:02(?:[01][0-9]|2[0-8])))))[A-Z]{2}[A-Za-z0-9]{2}$");
        Match match = regex.Match(idNo);
        if (!match.Success)
        {
            return false;
        }
        return true;
    }

    /// 作者 Ares Jack
    /// 創建日期：2021/12/27
    /// 修改日期：2021/12/27
    /// <summary>
    /// 檢查身分證字號
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool CheckNaturePersonID(string id)
    {
        var regex = new Regex("^[A-Z]{1}[0-9]{9}$");
        if (!regex.IsMatch(id))
        {
            return false;
        }

        int[] seed = new int[10];        
        string[] charMapping = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "X", "Y", "W", "Z", "I", "O" };
        string target = id.Substring(0, 1); //取第一個英文數字
        for (int index = 0; index < charMapping.Length; index++)
        {
            if (charMapping[index] == target)
            {
                index += 10;
                seed[0] = index / 10;
                seed[1] = (index % 10) * 9;
                break;
            }
        }
        for (int index = 2; index < 10; index++) 
        {                  
            seed[index] = Convert.ToInt32(id.Substring(index - 1, 1)) * (10 - index);
        }

        if ((10 - (seed.Sum() % 10)) % 10 != Convert.ToInt32(id.Substring(9, 1)))
        {
            return false;
        }

        return true;
    }

    #region 自然人收單
    /// 作者 Ares Jack
    /// 創建日期：2021/11/19
    /// 修改日期：2022/01/21
    /// <summary>
    /// 使用者輸入行業別自動帶出中文名稱
    /// </summary>
    /// <param name="txtIndustry"></param>
    public void setIndustryChName(string controlID, string CH_controlID, string txtIndustry, string no)
    {
        try
        {
            CustLabel HQlblIndustry = this.FindControl(CH_controlID) as CustLabel;
            CustTextBox txtIndustry_Control = FindControl(controlID) as CustTextBox;

            if (!string.IsNullOrEmpty(txtIndustry.Trim()))
            {
                string setIndustryChName_result = checkCodeType("18", controlID, true, CH_controlID);
                if (setIndustryChName_result.Trim() != "")
                {
                    HQlblIndustry.Text = "";
                }
            }
            else
            {
                txtIndustry_Control.BackColor = System.Drawing.Color.White;
                HQlblIndustry.Text = "";
            }
            
        }
        catch (Exception ex)
        {
            Logging.Log("行業別" + no + "自動帶出中文名稱失敗 :" + ex);
        }
    }
    /// 作者 Ares Jack
    /// 創建日期：2021/11/22
    /// 修改日期：2022/01/21
    /// <summary>
    /// 使用者輸入職稱自動帶出中文名稱
    /// </summary>
    /// <param name="txtTITLE"></param>
    public void setTITLEName(string controlID, string CH_controlID, string txtTITLE)
    {
        try
        {
            CustLabel HQlblTITLE = this.FindControl(CH_controlID) as CustLabel;
            CustTextBox txtTITLE_Control = FindControl(controlID) as CustTextBox;

            if (!string.IsNullOrEmpty(txtTITLE.Trim()))
            {
                string setTITLEName_result = checkCodeType("19", controlID, true, CH_controlID);
                if (setTITLEName_result.Trim() != "")
                {
                    HQlblTITLE.Text = "";
                }
            }
            else
            {
                txtTITLE_Control.BackColor = System.Drawing.Color.White;
                HQlblTITLE.Text = "";
            }
        }
        catch (Exception ex)
        {
            Logging.Log("職稱自動帶出中文名稱失敗 :" + ex);
        }
    }
    /// 作者 Ares Jack
    /// 創建日期：2021/11/19
    /// 修改日期：2022/01/21
    /// <summary>
    /// 使用者輸入行業別編號自動帶出中文名稱
    /// </summary>
    /// <param name="txtCC"></param>
    public void setCC_ChName(string controlID, string CH_controlID, string txtCC, string no)
    {
        try
        {
            CustLabel HQlblHCOP_CC_Cname = this.FindControl(CH_controlID) as CustLabel;
            CustTextBox txtCC_Control = FindControl(controlID) as CustTextBox;

            if (!string.IsNullOrEmpty(txtCC.Trim()))
            {
                string setCC_ChName_result = checkCodeType("3", controlID, true, CH_controlID);
                if (setCC_ChName_result.Trim() != "")
                {
                    HQlblHCOP_CC_Cname.Text = "";
                }
            }
            else
            {
                txtCC_Control.BackColor = System.Drawing.Color.White;
                HQlblHCOP_CC_Cname.Text = "";
            }
        }
        catch (Exception ex)
        {
            Logging.Log("行業別編號"+ no + "自動帶出中文名稱失敗 :" + ex);
        }
    }
    /// 作者 Ares Jack
    /// 創建日期：2021/11/19
    /// 修改日期：2022/01/21
    /// <summary>
    /// 使用者輸入職稱編號自動帶出中文名稱
    /// </summary>
    /// <param name="txtOC"></param>
    public void setOC_ChName(string controlID, string CH_controlID, string txtOC)
    {
        try
        {
            CustLabel HQlblHCOP_OC_Cname = this.FindControl(CH_controlID) as CustLabel;
            CustTextBox txtOC_Control = FindControl(controlID) as CustTextBox;

            if (!string.IsNullOrEmpty(txtOC.Trim()))
            {
                string setOC_ChName_result = checkCodeType("16", controlID, true, CH_controlID);
                if (setOC_ChName_result.Trim() != "")
                {
                    HQlblHCOP_OC_Cname.Text = "";
                }
            }    
            else
            {
                txtOC_Control.BackColor = System.Drawing.Color.White;
                HQlblHCOP_OC_Cname.Text = "";
            }
        }
        catch (Exception ex)
        {
            Logging.Log("職稱編號自動帶出中文名稱失敗 :" + ex);
        }
    }
    #endregion

}