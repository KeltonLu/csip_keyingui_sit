//******************************************************************
//*  作    者：蘇洺葳
//*  功能說明：ETag二次鍵檔(ETag二KEY)
//*  創建日期：2018/02/
//* 修改紀錄：
//* <author>            <time>            <TaskID>                <desc>
//* Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//* Ares Stanley      2021/04/15                                  新增HTG錯誤時顯示端末訊息
//* Ares Stanley      2021/04/27                                  調整HTG錯誤判斷
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Drawing;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using Framework.Common.Message;
using Framework.Common.Logging;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using Framework.Common.Utility;
using Newtonsoft.Json;

public partial class P010101130001 : PageBase
{
    #region 變數區

    /// <summary>
    /// Session變數集合
    /// </summary>
    private static EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    /// <summary>
    /// 切換測試電文
    /// </summary>
    private bool isTest;
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        /// 切換測試電文
        string isTEST = UtilHelper.GetAppSettings("isTest");
        if (isTEST == "Y") { isTest = true; } else { isTest = false; }
        if (!IsPostBack)
        {
            ClearPage(false, "", "");
        }

        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"]; //*記錄網頁訊息
    }

    #region 事件區

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtPrimaryCardID.Text;

        //20200109-修改SOC存入條件
        //log.Statement_Text = string.Format("UserId:{0}", log.Customer_Id); //查詢條件內容: 用 | 區隔,要看文件確認
        log.Statement_Text = string.Format("CUSTOMER_ID:{0}|AC_NO:{1}|BRANCH_ID:{2}|ROLE_ID:{3}", log.Customer_Id, log.Account_Nbr, log.Branch_Nbr, log.Role_Id); //查詢條件內容: 用 | 區隔

        BRL_AP_LOG.Add(log);
        //------------------------------------------------------
        string primaryCardID = this.txtPrimaryCardID.Text.Trim();   // 正卡人ID
        string receiveNumber = this.txtReceiveNumber.Text.Trim();   // 收件編號

        if (primaryCardID == "")
        {
            base.strClientMsg = MessageHelper.GetMessage("01_01011300_013");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtPrimaryCardID"));
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01011300_013") + "');");
            return;
        }

        if (receiveNumber == "")
        {
            base.strClientMsg = MessageHelper.GetMessage("01_01011300_014");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01011300_014") + "');");
            return;
        }

        DataTable dt = BRUtilities_Etag.GetUtilities_Etag(primaryCardID, receiveNumber, "1");

        if (dt.Rows.Count == 0)
        {
            base.strClientMsg = MessageHelper.GetMessage("01_01011300_007");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtPrimaryCardID"));
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01011300_007") + "');");
            ClearPage(false, "", "");
            return;
        }
        else
        {
            base.sbRegScript.Append(BaseHelper.SetFocus("txtApplyType"));
            ClearPage(true, receiveNumber, primaryCardID);
            this.txtName.Text = dt.Rows[0]["NAME"].ToString();
            btnSubmit.Enabled = true;

            DataTable dt2 = BRUtilities_Etag.GetUtilities_Etag(primaryCardID, receiveNumber, "2");

            if (dt2.Rows.Count > 0)
            {
                this.ckEtagFlag.Checked = dt2.Rows[0]["ETAG_FLAG"].ToString() == "True" ? true : false;
                this.ckTemplateParkFlag.Checked = dt2.Rows[0]["TEMPLATE_PARK_FLAG"].ToString() == "True" ? true : false;
                this.ckMonthlyParkFlag.Checked = dt2.Rows[0]["MONTHLY_PARK_FLAG"].ToString() == "True" ? true : false;
                this.txtName.Text = dt2.Rows[0]["NAME"].ToString();
                this.txtApplyType.Text = dt2.Rows[0]["APPLY_TYPE"].ToString();
                this.txtPlateNO1_1.Text = SplitePlateNo(dt2.Rows[0]["PLATE_NO"].ToString(), "front");
                this.txtPlateNO1_2.Text = SplitePlateNo(dt2.Rows[0]["PLATE_NO"].ToString(), "back");
                this.txtOwnersID.Text = dt2.Rows[0]["OWNERS_ID"].ToString();
                this.txtPopluNO.Text = dt2.Rows[0]["POPUL_NO"].ToString();
                this.txtPopulEmpNO.Text = dt2.Rows[0]["POPUL_EMP_NO"].ToString();
                this.txtIntroducerCardID.Text = dt2.Rows[0]["INTRODUCER_CARD_ID"].ToString();
                this.txtMonthlyParkingNO.Text = dt2.Rows[0]["MONTHLY_PARKING_NO"].ToString();

                if (dt2.Rows.Count > 1)
                {
                    this.txtPlateNO2_1.Text = SplitePlateNo(dt2.Rows[1]["PLATE_NO"].ToString(), "front");
                    this.txtPlateNO2_2.Text = SplitePlateNo(dt2.Rows[1]["PLATE_NO"].ToString(), "back");
                }

                if (dt2.Rows.Count > 2)
                {
                    this.txtPlateNO3_1.Text = SplitePlateNo(dt2.Rows[2]["PLATE_NO"].ToString(), "front");
                    this.txtPlateNO3_2.Text = SplitePlateNo(dt2.Rows[2]["PLATE_NO"].ToString(), "back");
                }

                if (dt2.Rows.Count > 3)
                {
                    this.txtPlateNO4_1.Text = SplitePlateNo(dt2.Rows[3]["PLATE_NO"].ToString(), "front");
                    this.txtPlateNO4_2.Text = SplitePlateNo(dt2.Rows[3]["PLATE_NO"].ToString(), "back");
                }

                SetEnabledTxt(true);
            }
        }


        //// 查詢第一卡人檔
        //Hashtable htOutputP4_JCF6 = GetP4JCF6();

        //if (htOutputP4_JCF6 != null)
        //{
        //    this.txtName.Text = htOutputP4_JCF6["NAME_1"].ToString().Trim();
        //    base.sbRegScript.Append("document.getElementById('txtReceiveNumber').focus();");
        //    ClearPage(true);
        //}
        //else
        //{
        //    this.txtName.Text = "";
        //    base.sbRegScript.Append("document.getElementById('txtPrimaryCardID').focus();");
        //    ClearPage(false);
        //}
    }

    /// <summary>
    /// 提交
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        string primaryCardID = this.txtPrimaryCardID.Text.Trim();       // 正卡人ID
        string receiveNumber = this.txtReceiveNumber.Text.Trim();       // 收件編號
        DataTable dt2 = new DataTable();

        DataTable dt1 = BRUtilities_Etag.GetUtilities_Etag(primaryCardID, receiveNumber, "1");
        //寫入2KEY資料
        bool isModify = ModifyDataBase();

        if (isModify)
        {
            dt2 = BRUtilities_Etag.GetUtilities_Etag(primaryCardID, receiveNumber, "2");
        }

        string setFocus = "";
        string msg = CompareData(dt1, dt2, out setFocus);

        if (msg != "")
        {
            msg = msg + "與 一Key 資料不符";
            base.strClientMsg += MessageHelper.GetMessage("01_01011300_010");
            base.sbRegScript.Append(BaseHelper.SetFocus(setFocus));
            base.sbRegScript.Append("alert('" + msg + "');");
            return;
        }
        //若相符，開始打電文

        //UploadHostByHtg(out msg);
        //base.sbRegScript.Append("alert('" + msg + "');");

        string allmsg = UploadHostByHtg(out msg);
        base.sbRegScript.Append("alert('" + allmsg + "');");
        //20190614 (U) by Nash, 按儲存送出後，儲存欄位不可反白
        //btnSubmit.Enabled = false;
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        //20190614(U) by Nash, 按清除鍵後畫面所有欄位須全部清除
        ClearPage(false, "", "");
        btnSubmit.Enabled = false;
    }

    #endregion

    #region 方法區
    /// <summary>
    /// 透過電文傳送Etag資料
    /// </summary>
    /// <returns></returns>
    private string UploadHostByHtg(out string msg)
    {
        bool etagFlag = this.ckEtagFlag.Checked;                        // ETAG儲值
        bool templateParkFlag = this.ckTemplateParkFlag.Checked;        // 臨時停車
        bool monthlyParkFlag = this.ckMonthlyParkFlag.Checked;          // 月租停車

        string receiveNumber = this.txtReceiveNumber.Text.Trim();       // 收件編號
        string primaryCardID = this.txtPrimaryCardID.Text.Trim();       // 正卡人ID
        string name = this.txtName.Text.Trim();                         // 姓名
        string applyType = this.txtApplyType.Text.Trim();               // 申請別(1.申請 2.終止)
        string ownersID = this.txtOwnersID.Text.Trim();                 // 車主ID或統編
        string monthlyParkingNO = this.txtMonthlyParkingNO.Text.Trim(); // 月租停車廠代碼
        string populNo = this.txtPopluNO.Text.Trim();                   // 推廣代號
        string populEmpNo = this.txtPopulEmpNO.Text.Trim();             // 推廣員編
        string introducerCardID = this.txtIntroducerCardID.Text.Trim(); // 種子
        string plateNO1_1 = this.txtPlateNO1_1.Text.Trim();             // 車號1-前
        string plateNO1_2 = this.txtPlateNO1_2.Text.Trim();             // 車號1-後
        string plateNO2_1 = this.txtPlateNO2_1.Text.Trim();             // 車號2-前
        string plateNO2_2 = this.txtPlateNO2_2.Text.Trim();             // 車號2-後
        string plateNO3_1 = this.txtPlateNO3_1.Text.Trim();             // 車號3-前
        string plateNO3_2 = this.txtPlateNO3_2.Text.Trim();             // 車號3-後
        string plateNO4_1 = this.txtPlateNO4_1.Text.Trim();             // 車號4-前
        string plateNO4_2 = this.txtPlateNO4_2.Text.Trim();             // 車號4-後
        string keyInDate = DateTime.Now.ToString("yyyyMMdd");

        int successCount = 0;
        msg = "";
        string allmsg = "";

        // 上傳 ETAG儲值 1641
        if (etagFlag)
        {
            // 上傳 所有車牌
            successCount += UploadByPlateNOs("01", monthlyParkingNO, plateNO1_1, plateNO1_2, plateNO2_1, plateNO2_2, plateNO3_1, plateNO3_2, plateNO4_1, plateNO4_2, out msg);

            if (msg != "")
            {
                allmsg += "ETAG儲值\\n" + msg;
            }

        }

        // 上傳 臨時停車 1642
        if (templateParkFlag)
        {
            successCount += UploadByPlateNOs("02", monthlyParkingNO, plateNO1_1, plateNO1_2, plateNO2_1, plateNO2_2, plateNO3_1, plateNO3_2, plateNO4_1, plateNO4_2, out msg);

            if (msg != "")
            {
                allmsg += "臨時停車\\n" + msg;
            }
        }

        // 上傳 月租停車 1643
        if (monthlyParkFlag)
        {
            successCount += UploadByPlateNOs("03", monthlyParkingNO, plateNO1_1, plateNO1_2, plateNO2_1, plateNO2_2, plateNO3_1, plateNO3_2, plateNO4_1, plateNO4_2, out msg);

            if (msg != "")
            {
                allmsg += "月租停車\\n" + msg;
            }
        }

        //return msg + " 成功：" + successCount + " 筆";
        return allmsg + " 成功：" + successCount + " 筆";
    }
    /// <summary>
    /// 逐筆傳送車牌號碼
    /// </summary>
    /// <param name="type">代扣類別 01: 儲值，02: 臨停，03: 月租</param>
    /// <param name="monthlyParkingNO"></param>
    /// <param name="plateNO1_1"></param>
    /// <param name="plateNO1_2"></param>
    /// <param name="plateNO2_1"></param>
    /// <param name="plateNO2_2"></param>
    /// <param name="plateNO3_1"></param>
    /// <param name="plateNO3_2"></param>
    /// <param name="plateNO4_1"></param>
    /// <param name="plateNO4_2"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    private int UploadByPlateNOs(string type, string monthlyParkingNO, string plateNO1_1, string plateNO1_2, string plateNO2_1, string plateNO2_2,
                                    string plateNO3_1, string plateNO3_2, string plateNO4_1, string plateNO4_2, out string msg)
    {
        msg = "";
        int successCount = 0;
        string errorMsg = "";

        successCount += UploadByPlateNO(type, monthlyParkingNO, plateNO1_1, plateNO1_2, out errorMsg);
        msg += errorMsg;

        successCount += UploadByPlateNO(type, monthlyParkingNO, plateNO2_1, plateNO2_2, out errorMsg);
        msg += errorMsg;

        successCount += UploadByPlateNO(type, monthlyParkingNO, plateNO3_1, plateNO3_2, out errorMsg);
        msg += errorMsg;

        successCount += UploadByPlateNO(type, monthlyParkingNO, plateNO4_1, plateNO4_2, out errorMsg);
        msg += errorMsg;

        return successCount;
    }

    /// <summary>
    /// 傳送單筆車號
    /// </summary>
    /// <param name="type">代扣類別 01: 儲值，02: 臨停，03: 月租</param>
    /// <param name="monthlyParkingNO"></param>
    /// <param name="plateNO1"></param>
    /// <param name="plateNO2"></param>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    private int UploadByPlateNO(string type, string monthlyParkingNO, string plateNO1, string plateNO2, out string errorMsg)
    {
        errorMsg = "";
        string msg = "";
        int result = 0;

        if (plateNO1 != "")
        {
            // 新增 及 修改 電文
            result = UploadByType(type, monthlyParkingNO, plateNO1, plateNO2, out msg);

            if (result == 0)
            {
                // 組錯誤資訊
                errorMsg = CombinMsg(plateNO1, plateNO2, msg);
            }
        }

        return result;
    }
    /// <summary>
    /// 傳送電文異動主機資料
    /// </summary>
    /// <param name="type">代扣類別 01: 儲值，02: 臨停，03: 月租</param>
    /// <param name="monthlyParkingNO"></param>
    /// <param name="plateNO1"></param>
    /// <param name="plateNO2"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    private int UploadByType(string type, string monthlyParkingNO, string plateNO1, string plateNO2, out string msg)
    {
        msg = "";
        int result = 0;
        string sendHostFlag = "N";
        string primaryCardID = this.txtPrimaryCardID.Text.Trim();       // 正卡人ID
        string name = this.txtName.Text.Trim();                         // 姓名
        string receiveNumber = this.txtReceiveNumber.Text.Trim();       // 收件編號
        string applyType = this.txtApplyType.Text.Trim();               // 申請別(1.申請 2.終止)
        string ownersID = this.txtOwnersID.Text.Trim();                 // 車主ID或統編
        string popluNO = this.txtPopluNO.Text.Trim();                   // 推廣代號
        string populEmpNO = this.txtPopulEmpNO.Text.Trim();             // 推廣員編
        string introducerCardID = this.txtIntroducerCardID.Text.Trim(); // 種子
        string plateNO = plateNO1 + "-" + plateNO2;
        string keyInDate = DateTime.Now.ToString("yyyyMMdd");
        string typeName = (type == "01") ? "ETAG儲值" : ((type == "02") ? "臨時停車" : "月租停車");

        // 查詢電文
        Hashtable htgInput = GetHtgInput("2", type, plateNO1, plateNO2, ownersID, primaryCardID, monthlyParkingNO, receiveNumber, applyType, popluNO, populEmpNO, introducerCardID, eAgentInfo.agent_id_racf);
        Hashtable htgResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCLD, htgInput, false, "1", eAgentInfo);

        //20211213-依需求單號：RQ-2021-034274-000  ，調整顯示訊息 BY Peggy
        //if (htgResult.Contains("HtgMsg") && htgResult["HtgMsg"].ToString().Trim() != "P4_JCLD:8888 查無符合資料")
        if (htgResult.Contains("HtgMsg") && htgResult["HtgMsg"].ToString().Trim() != "P4_JCLD:8888 無符合條件資料，請重新輸入！")
        {
            // 若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            if (htgResult["HtgMsgFlag"].ToString() == "0")
            {
                base.strHostMsg = htgResult["HtgMsg"].ToString();
                msg = htgResult["HtgMsg"].ToString();
                base.strClientMsg = MessageHelper.GetMessage("01_01011200_002");// 查詢主機失敗
            }
            else
            {
                base.strClientMsg = htgResult["HtgMsg"].ToString();
            }
            sendHostFlag = "N";
            return result;
        }

        if (htgResult["MESSAGE_TYPE"].ToString() == "0002")
        {
            // 檢查是否需修改
            //if (htgResult["STATUS"].ToString() != applyType)
            if (htgResult["D_STATUS"].ToString() != applyType)
            {
                // 組成修改電文
                //htgInput = GetHtgInput("3", type, plateNO1, plateNO2, ownersID, primaryCardID, "", receiveNumber, applyType, "", "", "", eAgentInfo.agent_id);

                htgInput = GetHtgInput("3", type, plateNO1, plateNO2, ownersID, primaryCardID, txtMonthlyParkingNO.Text.Trim(), receiveNumber, applyType, popluNO, populEmpNO, introducerCardID, eAgentInfo.agent_id_racf);
                // 比對資料後記錄Customer_Log
                ContrastData(htgResult, type, plateNO1, plateNO2);
                // 傳送電文
                sendHostFlag = UploadHtg(htgInput, typeName, plateNO, out msg);
            }
            else
            {
                msg = MessageHelper.GetMessage("01_01011200_009");
                sendHostFlag = "N";
            }
        }
        else if (htgResult["MESSAGE_TYPE"].ToString() == "8888")
        {
            // 組成新增電文
            //htgInput = GetHtgInput("1", type, plateNO1, plateNO2, ownersID, primaryCardID, "", receiveNumber, applyType, "", "", "", eAgentInfo.agent_id);

            htgInput = GetHtgInput("1", type, plateNO1, plateNO2, ownersID, primaryCardID, txtMonthlyParkingNO.Text.Trim(), receiveNumber, applyType, popluNO, populEmpNO, introducerCardID, eAgentInfo.agent_id_racf);
            // 記錄Customer_Log
            ContrastData(typeName, plateNO1, plateNO2);
            // 傳送電文
            sendHostFlag = UploadHtg(htgInput, typeName, plateNO, out msg);
        }
        else
        {
            msg = htgResult["HtgMsg"].ToString();

            // 若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            if (htgResult["HtgMsgFlag"].ToString() == "0")
            {
                base.strHostMsg += " 車號：" + plateNO + "，" + typeName + "，" + msg + "；";
                base.strClientMsg += MessageHelper.GetMessage("01_01011200_002");
            }
            else
            {
                base.strClientMsg += " 車號：" + plateNO + "，" + typeName + "，" + msg + "；";
            }

            sendHostFlag = "N";
        }

        if (sendHostFlag == "Y") result = 1;

        Logging.Log(" 車號：" + plateNO + "，" + typeName + "，" + msg, LogLayer.HTG);

        // 新增二KEY車牌資料
        //bool addResult = BRUtilities_Etag.InsertIntoUtilities_Etag_PlateNo(receiveNumber, "1", plateNO, sendHostFlag, eAgentInfo.agent_id, keyInDate, type);
        //上述程式碼改成update
        bool addResult = BRUtilities_Etag.UpdateUtilities_Etag_PlateNo(receiveNumber, plateNO, sendHostFlag, "2");


        if (!addResult)
        {
            msg += " 車號：" + plateNO + MessageHelper.GetMessage("01_01011200_015");
            Logging.Log(msg, LogLayer.DB);
        }

        return result;
    }
    /// <summary>
    /// 傳送電文
    /// </summary>
    /// <param name="htgInput"></param>
    /// <param name="type">代扣類別 01: 儲值，02: 臨停，03: 月租</param>
    /// <param name="plateNO"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    private string UploadHtg(Hashtable htgInput, string typeName, string plateNO, out string msg)
    {
        Hashtable htgResult = null;
        if (!isTest)
        {
            htgResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCLD, htgInput, false, "1", eAgentInfo);
        }
        else
        {
            htgResult = new Hashtable();
            htgResult.Add("HtgSuccess", "P4_JCF6:0001 卡人資料更新成功．");
        }
        if (htgResult.Contains("HtgMsg"))
        {
            msg = htgResult["HtgMsg"].ToString();

            // 若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            if (htgResult["HtgMsgFlag"].ToString() == "0")
            {
                base.strHostMsg += " 車號：" + plateNO + "，" + typeName + "，" + msg + "；";
                base.strClientMsg += "車號：" + plateNO + "，" + typeName + "，" + MessageHelper.GetMessage("01_01011200_006") + "；";// 異動主機失敗
            }
            else
            {
                base.strClientMsg += msg;
            }

            return "N";
        }
        else
        {
            msg = htgResult["HtgSuccess"].ToString();// 異動主機成功
            base.strHostMsg += " 車號：" + plateNO + "，" + typeName + "，" + msg + "；";
            base.strClientMsg += "車號：" + plateNO + "，" + typeName + "，" + MessageHelper.GetMessage("01_01011200_005") + "；";
            return "Y";
        }
    }
    /// <summary>
    /// 比對資料後記錄Customer_Log
    /// </summary>
    /// <param name="htgP4_JCLD"></param>
    /// <param name="type"></param>
    /// <param name="plateNO1"></param>
    /// <param name="plateNO2"></param>
    private void ContrastData(Hashtable htgP4_JCLD, string type, string plateNO1, string plateNO2)
    {
        DataTable updateData = CommonFunction.GetDataTable();

        // 比對代扣類別 01: 儲值，02: 臨停，03: 月租
        CommonFunction.ContrastData(htgP4_JCLD, updateData, type, "TYPE", BaseHelper.GetShowText("01_01011200_005"));
        // 比對申請別
        CommonFunction.ContrastData(htgP4_JCLD, updateData, this.txtApplyType.Text.Trim(), "STATUS", BaseHelper.GetShowText("01_01011200_005"));
        // 比對車主ID或統編
        CommonFunction.ContrastData(htgP4_JCLD, updateData, this.txtOwnersID.Text.Trim(), "CUSTID", BaseHelper.GetShowText("01_01011200_013"));
        // 比對停車場代碼
        CommonFunction.ContrastData(htgP4_JCLD, updateData, this.txtMonthlyParkingNO.Text.Trim(), "VENDOR_CODE", BaseHelper.GetShowText("01_01011200_014"));
        // 比對推廣代號
        CommonFunction.ContrastData(htgP4_JCLD, updateData, this.txtPopluNO.Text.Trim(), "SPC", BaseHelper.GetShowText("01_01011200_015"));
        // 比對推廣員編
        CommonFunction.ContrastData(htgP4_JCLD, updateData, this.txtPopulEmpNO.Text.Trim(), "SPC_ID", BaseHelper.GetShowText("01_01011200_016"));
        // 比對種子
        CommonFunction.ContrastData(htgP4_JCLD, updateData, this.txtIntroducerCardID.Text.Trim(), "INTRODUCER_CARD_ID", BaseHelper.GetShowText("01_01011200_017"));
        // 比對車號-前
        CommonFunction.ContrastData(htgP4_JCLD, updateData, plateNO1, "LPR_NO_A", BaseHelper.GetShowText("01_01011200_003"));
        // 比對車號-後
        CommonFunction.ContrastData(htgP4_JCLD, updateData, plateNO2, "LPR_NO_B", BaseHelper.GetShowText("01_01011200_004"));

        if (updateData.Rows.Count > 0)
        {
            if (!CommonFunction.InsertCustomerLog(updateData, eAgentInfo, this.txtPrimaryCardID.Text.Trim(), "P4", (structPageInfo)Session["PageInfo"]))
            {
                if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    Logging.Log("記錄CustomerLog失敗", LogLayer.DB);
                }
            }
        }
    }

    /// <summary>
    /// 記錄Customer_Log
    /// </summary>
    /// <param name="typeName">代扣類別 01: 儲值，02: 臨停，03: 月租</param>
    /// <param name="plateNO1"></param>
    /// <param name="plateNO2"></param>
    private void ContrastData(string typeName, string plateNO1, string plateNO2)
    {
        DataTable updateData = CommonFunction.GetDataTable();
        DataRow drowRow = updateData.NewRow();

        drowRow = updateData.NewRow();
        drowRow[EntityCUSTOMER_LOG.M_field_name] = "代扣類別";
        drowRow[EntityCUSTOMER_LOG.M_before] = "";
        drowRow[EntityCUSTOMER_LOG.M_after] = typeName;
        updateData.Rows.Add(drowRow);

        drowRow = updateData.NewRow();
        drowRow[EntityCUSTOMER_LOG.M_field_name] = "姓名";
        drowRow[EntityCUSTOMER_LOG.M_before] = "";
        drowRow[EntityCUSTOMER_LOG.M_after] = this.txtName.Text.Trim();
        updateData.Rows.Add(drowRow);

        drowRow = updateData.NewRow();
        drowRow[EntityCUSTOMER_LOG.M_field_name] = "申請別(1.申請 2.終止)";
        drowRow[EntityCUSTOMER_LOG.M_before] = "";
        drowRow[EntityCUSTOMER_LOG.M_after] = this.txtApplyType.Text.Trim();
        updateData.Rows.Add(drowRow);

        drowRow = updateData.NewRow();
        drowRow[EntityCUSTOMER_LOG.M_field_name] = "車主ID或統編";
        drowRow[EntityCUSTOMER_LOG.M_before] = "";
        drowRow[EntityCUSTOMER_LOG.M_after] = this.txtOwnersID.Text.Trim();
        updateData.Rows.Add(drowRow);

        drowRow = updateData.NewRow();
        drowRow[EntityCUSTOMER_LOG.M_field_name] = "車號-前";
        drowRow[EntityCUSTOMER_LOG.M_before] = "";
        drowRow[EntityCUSTOMER_LOG.M_after] = plateNO1;
        updateData.Rows.Add(drowRow);

        drowRow = updateData.NewRow();
        drowRow[EntityCUSTOMER_LOG.M_field_name] = "車號-後";
        drowRow[EntityCUSTOMER_LOG.M_before] = "";
        drowRow[EntityCUSTOMER_LOG.M_after] = plateNO2;
        updateData.Rows.Add(drowRow);

        string monthlyParkingNO = this.txtMonthlyParkingNO.Text.Trim();
        if (!string.IsNullOrEmpty(monthlyParkingNO))
        {
            drowRow = updateData.NewRow();
            drowRow[EntityCUSTOMER_LOG.M_field_name] = "停車場代碼";
            drowRow[EntityCUSTOMER_LOG.M_before] = "";
            drowRow[EntityCUSTOMER_LOG.M_after] = monthlyParkingNO;
            updateData.Rows.Add(drowRow);
        }

        string popluNO = this.txtPopluNO.Text.Trim();
        if (!string.IsNullOrEmpty(this.txtPopluNO.Text.Trim()))
        {
            drowRow = updateData.NewRow();
            drowRow[EntityCUSTOMER_LOG.M_field_name] = "推廣代號";
            drowRow[EntityCUSTOMER_LOG.M_before] = "";
            drowRow[EntityCUSTOMER_LOG.M_after] = popluNO;
            updateData.Rows.Add(drowRow);
        }

        string populEmpNO = this.txtPopulEmpNO.Text.Trim();
        if (!string.IsNullOrEmpty(populEmpNO))
        {
            drowRow = updateData.NewRow();
            drowRow[EntityCUSTOMER_LOG.M_field_name] = "推廣員編";
            drowRow[EntityCUSTOMER_LOG.M_before] = "";
            drowRow[EntityCUSTOMER_LOG.M_after] = populEmpNO;
            updateData.Rows.Add(drowRow);
        }

        string introducerCardID = this.txtIntroducerCardID.Text.Trim();
        if (!string.IsNullOrEmpty(introducerCardID))
        {
            drowRow = updateData.NewRow();
            drowRow[EntityCUSTOMER_LOG.M_field_name] = "種子";
            drowRow[EntityCUSTOMER_LOG.M_before] = "";
            drowRow[EntityCUSTOMER_LOG.M_after] = introducerCardID;
            updateData.Rows.Add(drowRow);
        }

        if (updateData.Rows.Count > 0)
        {
            if (!CommonFunction.InsertCustomerLog(updateData, eAgentInfo, this.txtPrimaryCardID.Text.Trim(), "P4", (structPageInfo)Session["PageInfo"]))
            {
                if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    Logging.Log("記錄CustomerLog失敗", LogLayer.DB);
                }
            }
        }

        if (updateData.Rows.Count > 0)
        {
            if (!CommonFunction.InsertCustomerLog(updateData, eAgentInfo, this.txtPrimaryCardID.Text.Trim(), "P4", (structPageInfo)Session["PageInfo"]))
            {
                if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    Logging.Log("記錄CustomerLog失敗", LogLayer.DB);
                }
            }
        }
    }

    /// <summary>
    /// 組成電文上行
    /// </summary>
    /// <param name="functionCode"></param>
    /// <param name="type"></param>
    /// <param name="plateNo1"></param>
    /// <param name="plateNo2"></param>
    /// <param name="ownersID"></param>
    /// <param name="primaryCardID"></param>
    /// <param name="vendorCode"></param>
    /// <param name="receiveNumber"></param>
    /// <param name="applyType"></param>
    /// <param name="popluNO"></param>
    /// <param name="populEmpNO"></param>
    /// <param name="introducerCardID"></param>
    /// <param name="agent_id"></param>
    /// <returns></returns>
    private Hashtable GetHtgInput(string functionCode, string type, string plateNo1, string plateNo2, string ownersID, string primaryCardID, string vendorCode,
                                    string receiveNumber, string applyType, string popluNO, string populEmpNO, string introducerCardID, string agent_id)
    {
        Hashtable htInput = new Hashtable();

        // 上行
        htInput.Add("TRAN_ID", "JCLD");                             // 電文交易名稱
        htInput.Add("PROGRAM_ID", "JCGU851");                       // 電文程式名稱
        htInput.Add("USER_ID", "CSIP");                             // 電文使用者ID
        htInput.Add("MESSAGE_TYPE", "");                            // 回覆碼
        htInput.Add("FUNCTION_CODE", functionCode);                 // 功能碼 1: 新增(帶入Pkey & 鍵檔欄位)，2: 查詢(帶入Pkey)，3: 修改(帶入Pkey & 有異動的欄位)，4: 刪除(帶入Pkey)
        htInput.Add("TYPE", type);                                  // 代扣類別 01: 儲值，02: 臨停，03: 月租
        htInput.Add("LPR_NO_A", plateNo1);                          // 車號"-"左邊4位
        htInput.Add("LPR_NO_B", plateNo2);                          // 車號"-"右邊4位
        htInput.Add("CUSTID", ownersID);                            // 車主ID
        htInput.Add("ACCT_ID", primaryCardID);                      // 正卡人ID
        htInput.Add("VENDOR_CODE", vendorCode);                     // 停車場代碼 代扣類別= 03(月租)時需要提供，否則放空白
        htInput.Add("APPLY_NO", receiveNumber);                     // 收件編號/信用卡申請書編號 1.若為13碼則為[收件編號]，2.若為16碼則為[信用卡申請書編號]
        htInput.Add("STATUS", applyType);                           // 狀態
        htInput.Add("SPC", popluNO);                                // 推廣通路代號
        htInput.Add("SPC_ID", populEmpNO);                          // 推廣員編
        htInput.Add("INTRODUCER_CARD_ID", introducerCardID);        // 種子
        htInput.Add("U_UPDATE_USER", agent_id);    // 維護人員，員編or使用者代碼(左靠右補空白)
        htInput.Add("FILLER_00", "");                               // 保留空白

        return htInput;
    }

    /// <summary>
    /// 組錯誤資訊
    /// </summary>
    /// <param name="plateNO1"></param>
    /// <param name="plateNO2"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    private string CombinMsg(string plateNO1, string plateNO2, string msg)
    {
        return "車號：" + plateNO1 + "-" + plateNO2 + "\\n" + msg + "\\n";
    }

    /// <summary>
    /// 比對資料庫目前紀錄資料，1KEY與二KEY是否相符
    /// </summary>
    /// <param name="dt1"></param>
    /// <param name="dt2"></param>
    /// <param name="setFocus"></param>
    /// <returns></returns>
    private string CompareData(DataTable dt1, DataTable dt2, out string setFocus)
    {
        string result = "";
        setFocus = "";

        if (dt1.Rows[0]["ETAG_FLAG"].ToString() != dt2.Rows[0]["ETAG_FLAG"].ToString())
        {
            result = result + "「ETAG儲值」、";

            if (setFocus == "")
            {
                setFocus = "ckEtagFlag";
            }
        }

        if (dt1.Rows[0]["TEMPLATE_PARK_FLAG"].ToString() != dt2.Rows[0]["TEMPLATE_PARK_FLAG"].ToString())
        {
            result = result + "「臨時停車」、";

            if (setFocus == "")
            {
                setFocus = "ckTemplateParkFlag";
            }
        }

        if (dt1.Rows[0]["MONTHLY_PARK_FLAG"].ToString() != dt2.Rows[0]["MONTHLY_PARK_FLAG"].ToString())
        {
            result = result + "「月租停車」、";

            if (setFocus == "")
            {
                setFocus = "ckMonthlyParkFlag";
            }
        }

        if (dt1.Rows[0]["APPLY_TYPE"].ToString() != dt2.Rows[0]["APPLY_TYPE"].ToString())
        {
            result = result + "「申請別」、";

            if (setFocus == "")
            {
                setFocus = "txtApplyType";
            }
        }

        if (dt1.Rows[0]["PLATE_NO"].ToString() != dt2.Rows[0]["PLATE_NO"].ToString())
        {
            result = result + "「車牌號碼1」、";

            if (setFocus == "")
            {
                setFocus = "txtPlateNO1_1";
            }
        }

        if (dt1.Rows[0]["OWNERS_ID"].ToString() != dt2.Rows[0]["OWNERS_ID"].ToString())
        {
            result = result + "「車主ID或統編」、";

            if (setFocus == "")
            {
                setFocus = "txtOwnersID";
            }
        }

        if (dt1.Rows[0]["POPUL_NO"].ToString() != dt2.Rows[0]["POPUL_NO"].ToString())
        {
            result = result + "「推廣代號」、";

            if (setFocus == "")
            {
                setFocus = "txtPopluNO";
            }
        }

        if (dt1.Rows[0]["POPUL_EMP_NO"].ToString() != dt2.Rows[0]["POPUL_EMP_NO"].ToString())
        {
            result = result + "「推廣員編」、";

            if (setFocus == "")
            {
                setFocus = "txtPopulEmpNO";
            }
        }

        if (dt1.Rows[0]["INTRODUCER_CARD_ID"].ToString() != dt2.Rows[0]["INTRODUCER_CARD_ID"].ToString())
        {
            result = result + "「種子」、";

            if (setFocus == "")
            {
                setFocus = "txtIntroducerCardID";
            }
        }

        if (dt1.Rows[0]["MONTHLY_PARKING_NO"].ToString() != dt2.Rows[0]["MONTHLY_PARKING_NO"].ToString())
        {
            result = result + "「停車場代碼」、";

            if (setFocus == "")
            {
                setFocus = "txtMonthlyParkingNO";
            }
        }

        if (dt1.Rows.Count > 1 || dt2.Rows.Count > 1)
        {
            if (dt1.Rows.Count > 1 && dt2.Rows.Count > 1)
            {
                if (dt1.Rows[1]["PLATE_NO"].ToString() != dt2.Rows[1]["PLATE_NO"].ToString())
                {
                    result = result + "「車牌號碼2」、";

                    if (setFocus == "")
                    {
                        setFocus = "txtPlateNO2_1";
                    }
                }
            }
            else
            {
                if (dt1.Rows.Count == 1)
                {
                    result = result + "「車牌號碼2」、";

                    if (setFocus == "")
                    {
                        setFocus = "txtPlateNO2_1";
                    }
                }
                else
                {
                    result = result + "「車牌號碼2」、";

                    if (setFocus == "")
                    {
                        setFocus = "txtPlateNO2_1";
                    }
                }
            }
        }

        if (dt1.Rows.Count > 2 || dt2.Rows.Count > 2)
        {
            if (dt1.Rows.Count > 2 && dt2.Rows.Count > 2)
            {
                if (dt1.Rows[2]["PLATE_NO"].ToString() != dt2.Rows[2]["PLATE_NO"].ToString())
                {
                    result = result + "「車牌號碼3」、";

                    if (setFocus == "")
                    {
                        setFocus = "txtPlateNO3_1";
                    }
                }
            }
            else
            {
                if (dt1.Rows.Count == 2)
                {
                    result = result + "「車牌號碼3」、";

                    if (setFocus == "")
                    {
                        setFocus = "txtPlateNO3_1";
                    }
                }
                else
                {
                    result = result + "「車牌號碼3」、";

                    if (setFocus == "")
                    {
                        setFocus = "txtPlateNO3_1";
                    }
                }
            }
        }

        if (dt1.Rows.Count > 3 || dt2.Rows.Count > 3)
        {
            if (dt1.Rows.Count > 3 && dt2.Rows.Count > 3)
            {
                if (dt1.Rows[3]["PLATE_NO"].ToString() != dt2.Rows[3]["PLATE_NO"].ToString())
                {
                    result = result + "「車牌號碼4」、";

                    if (setFocus == "")
                    {
                        setFocus = "txtPlateNO4_1";
                    }
                }
            }
            else
            {
                if (dt1.Rows.Count == 3)
                {
                    result = result + "「車牌號碼4」、";

                    if (setFocus == "")
                    {
                        setFocus = "txtPlateNO4_1";
                    }
                }
                else
                {
                    result = result + "「車牌號碼4」、";

                    if (setFocus == "")
                    {
                        setFocus = "txtPlateNO4_1";
                    }
                }
            }
        }

        //表示有值，得移除最後一碼
        if (result.Length > 1)
        {
            result = result.Remove(result.Length - 1, 1);
        }
        return result;
    }

    private string SplitePlateNo(string plateNo, string type)
    {
        string result = "";

        if (plateNo != "")
        {
            if (type == "front")
            {
                result = plateNo.Split('-')[0];
            }
            else
            {
                result = plateNo.Split('-')[1];
            }
        }

        return result;
    }

    private void SetEnabledTxt(bool isEnabled)
    {
        this.txtApplyType.Enabled = isEnabled;
        this.txtPlateNO1_1.Enabled = isEnabled;
        this.txtPlateNO1_2.Enabled = isEnabled;
        this.txtOwnersID.Enabled = isEnabled;
        this.txtMonthlyParkingNO.Enabled = isEnabled;
        this.txtPopluNO.Enabled = isEnabled;
        this.txtPopulEmpNO.Enabled = isEnabled;
        this.txtIntroducerCardID.Enabled = isEnabled;
        this.txtPlateNO2_1.Enabled = isEnabled;
        this.txtPlateNO2_2.Enabled = isEnabled;
        this.txtPlateNO3_1.Enabled = isEnabled;
        this.txtPlateNO3_2.Enabled = isEnabled;
        this.txtPlateNO4_1.Enabled = isEnabled;
        this.txtPlateNO4_2.Enabled = isEnabled;
    }

    private bool ModifyDataBase()
    {
        bool result = false;

        bool etagFlag = this.ckEtagFlag.Checked;                        // ETAG儲值
        bool templateParkFlag = this.ckTemplateParkFlag.Checked;        // 臨時停車
        bool monthlyParkFlag = this.ckMonthlyParkFlag.Checked;          // 月租停車

        string receiveNumber = this.txtReceiveNumber.Text.Trim();       // 收件編號
        string primaryCardID = this.txtPrimaryCardID.Text.Trim();       // 正卡人ID
        string name = this.txtName.Text.Trim();                         // 姓名
        string applyType = this.txtApplyType.Text.Trim();               // 申請別(1.申請 2.終止)
        string ownersID = this.txtOwnersID.Text.Trim();                 // 車主ID或統編
        string monthlyParkingNO = this.txtMonthlyParkingNO.Text.Trim(); // 月租停車廠代碼
        string populNo = this.txtPopluNO.Text.Trim();                   // 推廣代號
        string populEmpNo = this.txtPopulEmpNO.Text.Trim();             // 推廣員編
        string introducerCardID = this.txtIntroducerCardID.Text.Trim(); // 種子
        string plateNO1_1 = this.txtPlateNO1_1.Text.Trim();             // 車號1-前
        string plateNO1_2 = this.txtPlateNO1_2.Text.Trim();             // 車號1-後
        string plateNO2_1 = this.txtPlateNO2_1.Text.Trim();             // 車號2-前
        string plateNO2_2 = this.txtPlateNO2_2.Text.Trim();             // 車號2-後
        string plateNO3_1 = this.txtPlateNO3_1.Text.Trim();             // 車號3-前
        string plateNO3_2 = this.txtPlateNO3_2.Text.Trim();             // 車號3-後
        string plateNO4_1 = this.txtPlateNO4_1.Text.Trim();             // 車號4-前
        string plateNO4_2 = this.txtPlateNO4_2.Text.Trim();             // 車號4-後
        string keyInDate = DateTime.Now.ToString("yyyyMMdd");

        // 新增資料庫
        bool addResult = ModifyUtilities_Etag(receiveNumber, primaryCardID, name, etagFlag, templateParkFlag, monthlyParkFlag, applyType, ownersID,
                                                monthlyParkingNO, populNo, populEmpNo, introducerCardID, eAgentInfo.agent_id, keyInDate, "2");

        if (addResult)
        {
            ModifyUtilities_Etag_PlateNo(receiveNumber, "2", plateNO1_1, plateNO1_2, plateNO2_1, plateNO2_2, plateNO3_1, plateNO3_2, plateNO4_1, plateNO4_2, eAgentInfo.agent_id, keyInDate);

            result = true;
        }

        return result;
    }

    private bool ModifyUtilities_Etag(string receiveNumber, string primaryCardID, string name, bool etagFlag, bool templateParkFlag, bool monthlyParkFlag,
                                        string applyType, string ownersID, string monthlyParkingNO, string populNo, string populEmpNo, string introducerCardID,
                                        string agentID, string keyInDate, string keyInFlag)
    {
        bool result = false;

        // 判斷收編是否已有收編
        bool isExist = BRUtilities_Etag.IsExistReceiveNumber(primaryCardID, receiveNumber, "2");

        if (!isExist)
        {
            // 新增資料庫
            result = BRUtilities_Etag.InsertIntoUtilities_Etag(receiveNumber, primaryCardID, name, etagFlag, templateParkFlag, monthlyParkFlag, applyType, ownersID,
                                                                    monthlyParkingNO, populNo, populEmpNo, introducerCardID, eAgentInfo.agent_id, keyInDate, "2");
        }
        else
        {
            // 修改資料庫
            result = BRUtilities_Etag.UpdateUtilities_Etag(receiveNumber, primaryCardID, name, etagFlag, templateParkFlag, monthlyParkFlag, applyType, ownersID,
                                                                    monthlyParkingNO, populNo, populEmpNo, introducerCardID, eAgentInfo.agent_id, keyInDate, "2");
        }

        return result;
    }

    private void ModifyUtilities_Etag_PlateNo(string receiveNumber, string keyInFlag, string plateNO1_1, string plateNO1_2, string plateNO2_1, string plateNO2_2,
                                                string plateNO3_1, string plateNO3_2, string plateNO4_1, string plateNO4_2, string agentID, string keyInDate)
    {
        string plateNo1 = plateNO1_1 + "-" + plateNO1_2;
        string plateNo2 = plateNO2_1 + "-" + plateNO2_2;
        string plateNo3 = plateNO3_1 + "-" + plateNO3_2;
        string plateNo4 = plateNO4_1 + "-" + plateNO4_2;

        BRUtilities_Etag.DeleteUtilities_Etag_PlateNo(receiveNumber, keyInFlag);

        InsertUtilities_Etag_PlateNo(receiveNumber, keyInFlag, plateNo1, agentID, keyInDate, "1");
        InsertUtilities_Etag_PlateNo(receiveNumber, keyInFlag, plateNo2, agentID, keyInDate, "2");
        InsertUtilities_Etag_PlateNo(receiveNumber, keyInFlag, plateNo3, agentID, keyInDate, "3");
        InsertUtilities_Etag_PlateNo(receiveNumber, keyInFlag, plateNo4, agentID, keyInDate, "4");
    }

    private void InsertUtilities_Etag_PlateNo(string receiveNumber, string keyInFlag, string plateNo, string agentID, string keyInDate, string rowNumber)
    {
        if (plateNo != "-")
        {
            BRUtilities_Etag.InsertIntoUtilities_Etag_PlateNo(receiveNumber, keyInFlag, plateNo, "N", agentID, keyInDate, "", rowNumber);
        }
    }

    /// <summary>
    /// 清空畫面
    /// </summary>
    /// 20190614 (U) by Nash, 按清除鍵後畫面所有欄位須全部清除
    /// <param name="isEnabled"></param>
    private void ClearPage(bool isEnabled, string receiveNumber, string primaryCardID)
    {
        this.txtApplyType.Enabled = isEnabled;
        this.txtPlateNO1_1.Enabled = isEnabled;
        this.txtPlateNO1_2.Enabled = isEnabled;
        this.txtOwnersID.Enabled = isEnabled;
        this.txtMonthlyParkingNO.Enabled = isEnabled;
        this.txtPopluNO.Enabled = isEnabled;
        this.txtPopulEmpNO.Enabled = isEnabled;
        this.txtIntroducerCardID.Enabled = isEnabled;
        this.txtPlateNO2_1.Enabled = isEnabled;
        this.txtPlateNO2_2.Enabled = isEnabled;
        this.txtPlateNO3_1.Enabled = isEnabled;
        this.txtPlateNO3_2.Enabled = isEnabled;
        this.txtPlateNO4_1.Enabled = isEnabled;
        this.txtPlateNO4_2.Enabled = isEnabled;

        //20190614(U) by Nash, 按清除鍵後畫面所有欄位須全部清除
        this.txtPrimaryCardID.Text = primaryCardID;    // 正卡人ID
        this.txtName.Text = "";                  // 姓名
        this.txtReceiveNumber.Text = receiveNumber;    // 收件編號
        this.txtApplyType.Text = "";        // 申請別(1.申請 2.終止)
        this.txtPlateNO1_1.Text = "";       // 車號1-前
        this.txtPlateNO1_2.Text = "";       // 車號1-後
        this.txtOwnersID.Text = "";         // 車主ID或統編
        this.txtMonthlyParkingNO.Text = ""; // 停車場代碼
        this.txtPopluNO.Text = "";          // 推廣代號
        this.txtPopulEmpNO.Text = "";       // 推廣員編
        this.txtIntroducerCardID.Text = ""; // 種子
        this.txtPlateNO2_1.Text = "";       // 車號2-前
        this.txtPlateNO2_2.Text = "";       // 車號2-後
        this.txtPlateNO3_1.Text = "";       // 車號3-前
        this.txtPlateNO3_2.Text = "";       // 車號3-後
        this.txtPlateNO4_1.Text = "";       // 車號4-前
        this.txtPlateNO4_2.Text = "";       // 車號4-後

        if (primaryCardID == "" && receiveNumber == "")
        {
            this.ckEtagFlag.Checked = true;
            this.ckTemplateParkFlag.Checked = false;
            this.ckMonthlyParkFlag.Checked = false;
        }
    }






    #endregion

}