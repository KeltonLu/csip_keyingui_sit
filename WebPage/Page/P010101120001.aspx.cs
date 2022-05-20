//******************************************************************
//*  作    者：蘇洺葳
//*  功能說明：公用事業一次鍵檔(公用事業一KEY)
//*  創建日期：2018/02/01
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
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
using Newtonsoft.Json;

public partial class P010101120001 : PageBase
{
    #region 變數區

    /// <summary>
    /// Session變數集合
    /// </summary>
    private static EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ClearPage(false, "");
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
            base.strClientMsg = MessageHelper.GetMessage("01_01011200_016");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtPrimaryCardID"));
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01011200_016") + "');");
            return;
        }

        if (receiveNumber == "")
        {
            base.strClientMsg = MessageHelper.GetMessage("01_01011200_017");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01011200_017") + "');");
            return;
        }

        // 資料庫已有檔案要先取
        bool hasData = GetUtilities_Etag();

        if (!hasData)
        {
            // 查詢第一卡人檔
            Hashtable htOutputP4_JCF6 = GetP4JCF6();

            if (htOutputP4_JCF6 != null)
            {
                this.txtName.Text = htOutputP4_JCF6["NAME_1"].ToString().Trim();
                base.sbRegScript.Append(BaseHelper.SetFocus("txtApplyType"));
                ClearPage(true, receiveNumber);
            }
            else
            {
                this.txtName.Text = "";
                base.sbRegScript.Append(BaseHelper.SetFocus("txtPrimaryCardID"));
                ClearPage(false, receiveNumber);
            }
        }

        btnSubmit.Enabled = true;

    }

    /// <summary>
    /// 提交
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        string primaryCardID = this.txtPrimaryCardID.Text.Trim();   // 正卡人ID
        string receiveNumber = this.txtReceiveNumber.Text.Trim();   // 收件編號
        string result = "";

        // 判斷收編是否已用過
        bool isDuplicate = BRUtilities_Etag.DuplicateReceiveNumber(primaryCardID, receiveNumber, "1");

        if (isDuplicate)
        {
            base.strClientMsg = MessageHelper.GetMessage("01_01011200_008");
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01011200_008") + "');");
            return;
        }
        else
        {
            string msg = "";
            // 一key資料儲存
            result = ModifyDataBase();

            // 提交資料
            //result = UploadHostByHtg(out msg);
            base.sbRegScript.Append("alert('" + result + "');");
            base.strClientMsg += "，" + result.Replace("\\n", ";");
            if (msg == "")
            {
                ClearPage(false, "");
                this.ckEtagFlag.Checked = true;
                this.ckTemplateParkFlag.Checked = false;
                this.ckMonthlyParkFlag.Checked = false;
                this.txtPrimaryCardID.Text = "";
                this.txtName.Text = "";
                btnSubmit.Enabled = false;
                base.sbRegScript.Append(BaseHelper.SetFocus("txtPrimaryCardID"));
            }
        }
    }

    #endregion

    #region 方法區

    private bool GetUtilities_Etag()
    {
        bool result = false;
        string primaryCardID = this.txtPrimaryCardID.Text.Trim();   // 正卡人ID
        string receiveNumber = this.txtReceiveNumber.Text.Trim();   // 收件編號

        DataTable dt = BRUtilities_Etag.GetUtilities_Etag(primaryCardID, receiveNumber, "1");

        if (dt.Rows.Count > 0)
        {
            result = true;
            this.ckEtagFlag.Checked = dt.Rows[0]["ETAG_FLAG"].ToString() == "True" ? true : false;
            this.ckTemplateParkFlag.Checked = dt.Rows[0]["TEMPLATE_PARK_FLAG"].ToString() == "True" ? true : false;
            this.ckMonthlyParkFlag.Checked = dt.Rows[0]["MONTHLY_PARK_FLAG"].ToString() == "True" ? true : false;
            this.txtName.Text = dt.Rows[0]["NAME"].ToString();
            this.txtApplyType.Text = dt.Rows[0]["APPLY_TYPE"].ToString();
            this.txtPlateNO1_1.Text = SplitePlateNo(dt.Rows[0]["PLATE_NO"].ToString(), "front");
            this.txtPlateNO1_2.Text = SplitePlateNo(dt.Rows[0]["PLATE_NO"].ToString(), "back");
            this.txtOwnersID.Text = dt.Rows[0]["OWNERS_ID"].ToString();
            this.txtPopluNO.Text = dt.Rows[0]["POPUL_NO"].ToString();
            this.txtPopulEmpNO.Text = dt.Rows[0]["POPUL_EMP_NO"].ToString();
            this.txtIntroducerCardID.Text = dt.Rows[0]["INTRODUCER_CARD_ID"].ToString();
            this.txtMonthlyParkingNO.Text = dt.Rows[0]["MONTHLY_PARKING_NO"].ToString();

            if (dt.Rows.Count > 1)
            {
                this.txtPlateNO2_1.Text = SplitePlateNo(dt.Rows[1]["PLATE_NO"].ToString(), "front");
                this.txtPlateNO2_2.Text = SplitePlateNo(dt.Rows[1]["PLATE_NO"].ToString(), "back");
            }

            if (dt.Rows.Count > 2)
            {
                this.txtPlateNO3_1.Text = SplitePlateNo(dt.Rows[2]["PLATE_NO"].ToString(), "front");
                this.txtPlateNO3_2.Text = SplitePlateNo(dt.Rows[2]["PLATE_NO"].ToString(), "back");
            }

            if (dt.Rows.Count > 3)
            {
                this.txtPlateNO4_1.Text = SplitePlateNo(dt.Rows[3]["PLATE_NO"].ToString(), "front");
                this.txtPlateNO4_2.Text = SplitePlateNo(dt.Rows[3]["PLATE_NO"].ToString(), "back");
            }

            SetEnabledTxt(true);
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

    /// <summary>
    /// 清空畫面
    /// </summary>
    /// <param name="isEnabled"></param>
    private void ClearPage(bool isEnabled, string receiveNumber)
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

        this.txtReceiveNumber.Text = receiveNumber;
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
    }

    /// <summary>
    /// 查詢第一卡人檔
    /// </summary>
    /// <returns></returns>
    private Hashtable GetP4JCF6()
    {
        string upperUserID = this.txtPrimaryCardID.Text.Trim().ToUpper();

        Hashtable htInputP4_JCF6 = new Hashtable(); // 第一卡人檔上行
        Hashtable htOutputP4_JCF6 = null;           // 第一卡人檔下行
        htInputP4_JCF6.Add("ACCT_NBR", upperUserID);
        htInputP4_JCF6.Add("FUNCTION_CODE", "1");
        htInputP4_JCF6.Add("LINE_CNT", "0000");

        try
        {
            htOutputP4_JCF6 = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCF6, htInputP4_JCF6, false, "1", eAgentInfo);

            if (htOutputP4_JCF6.Contains("HtgMsg"))
            {
                // 若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                if (htOutputP4_JCF6["HtgMsgFlag"].ToString() == "0")
                {
                    base.strHostMsg = htOutputP4_JCF6["HtgMsg"].ToString();
                    base.strClientMsg = MessageHelper.GetMessage("01_01011200_002");// 查詢主機失敗
                }
                else
                {
                    base.strClientMsg = htOutputP4_JCF6["HtgMsg"].ToString();
                }

                return null;
            }
            else
            {
                if (htOutputP4_JCF6["NAME_1"].ToString().Trim() == "")
                {
                    base.strClientMsg = MessageHelper.GetMessage("01_01011200_007");// VD卡不可申請扣款
                    return null;
                }

                base.strHostMsg += htOutputP4_JCF6["HtgSuccess"].ToString();
                base.strClientMsg += MessageHelper.GetMessage("01_01011200_001");// 查詢主機成功
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.HTG);
        }

        return htOutputP4_JCF6;
    }

    private string ModifyDataBase()
    {
        string result = "新增失敗";

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
                                                monthlyParkingNO, populNo, populEmpNo, introducerCardID, eAgentInfo.agent_id, keyInDate, "1");

        if (addResult)
        {
            ModifyUtilities_Etag_PlateNo(receiveNumber, "1", plateNO1_1, plateNO1_2, plateNO2_1, plateNO2_2, plateNO3_1, plateNO3_2, plateNO4_1, plateNO4_2, eAgentInfo.agent_id, keyInDate);

            result = "新增成功";
        }

        return result;
    }

    private bool ModifyUtilities_Etag(string receiveNumber, string primaryCardID, string name, bool etagFlag, bool templateParkFlag, bool monthlyParkFlag,
                                        string applyType, string ownersID, string monthlyParkingNO, string populNo, string populEmpNo, string introducerCardID,
                                        string agentID, string keyInDate, string keyInFlag)
    {
        bool result = false;

        // 判斷收編是否已有收編
        bool isExist = BRUtilities_Etag.IsExistReceiveNumber(primaryCardID, receiveNumber, "1");

        if (!isExist)
        {
            // 新增資料庫
            result = BRUtilities_Etag.InsertIntoUtilities_Etag(receiveNumber, primaryCardID, name, etagFlag, templateParkFlag, monthlyParkFlag, applyType, ownersID,
                                                                    monthlyParkingNO, populNo, populEmpNo, introducerCardID, eAgentInfo.agent_id, keyInDate, "1");
        }
        else
        {
            // 修改資料庫
            result = BRUtilities_Etag.UpdateUtilities_Etag(receiveNumber, primaryCardID, name, etagFlag, templateParkFlag, monthlyParkFlag, applyType, ownersID,
                                                                    monthlyParkingNO, populNo, populEmpNo, introducerCardID, eAgentInfo.agent_id, keyInDate, "1");
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

        // 新增資料庫
        bool addResult = ModifyUtilities_Etag(receiveNumber, primaryCardID, name, etagFlag, templateParkFlag, monthlyParkFlag, applyType, ownersID,
                                                                    monthlyParkingNO, populNo, populEmpNo, introducerCardID, eAgentInfo.agent_id, keyInDate, "1");

        // 新增失敗
        if (!addResult)
        {
            msg = MessageHelper.GetMessage("01_01011200_004");
            return msg;
        }

        // 上傳 ETAG儲值 1641
        if (etagFlag)
        {
            // 上傳 所有車牌
            successCount += UploadByPlateNOs("01", monthlyParkingNO, plateNO1_1, plateNO1_2, plateNO2_1, plateNO2_2, plateNO3_1, plateNO3_2, plateNO4_1, plateNO4_2, out msg);
        }

        // 上傳 臨時停車 1642
        if (templateParkFlag)
        {
            successCount += UploadByPlateNOs("02", monthlyParkingNO, plateNO1_1, plateNO1_2, plateNO2_1, plateNO2_2, plateNO3_1, plateNO3_2, plateNO4_1, plateNO4_2, out msg);
        }

        // 上傳 月租停車 1643
        if (monthlyParkFlag)
        {
            successCount += UploadByPlateNOs("03", monthlyParkingNO, plateNO1_1, plateNO1_2, plateNO2_1, plateNO2_2, plateNO3_1, plateNO3_2, plateNO4_1, plateNO4_2, out msg);
        }

        return msg + " 成功：" + successCount + " 筆";
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
        Hashtable htgInput = GetHtgInput("2", type, plateNO1, plateNO2, ownersID, primaryCardID, monthlyParkingNO, receiveNumber, applyType, popluNO, populEmpNO, introducerCardID, eAgentInfo.agent_id);
        Hashtable htgResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCLD, htgInput, false, "1", eAgentInfo);

        if (htgResult["MESSAGE_TYPE"].ToString() == "0002")
        {
            // 檢查是否需修改
            if (htgResult["STATUS"].ToString() != applyType)
            {
                // 組成修改電文
                htgInput = GetHtgInput("3", type, plateNO1, plateNO2, ownersID, primaryCardID, "", receiveNumber, applyType, "", "", "", eAgentInfo.agent_id);
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
            htgInput = GetHtgInput("1", type, plateNO1, plateNO2, ownersID, primaryCardID, "", receiveNumber, applyType, "", "", "", eAgentInfo.agent_id);
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
        bool addResult = BRUtilities_Etag.InsertIntoUtilities_Etag_PlateNo(receiveNumber, "1", plateNO, sendHostFlag, eAgentInfo.agent_id, keyInDate, type);

        if (!addResult)
        {
            msg += " 車號：" + plateNO + MessageHelper.GetMessage("01_01011200_015");
            Logging.Log(msg, LogLayer.DB);
        }

        return result;
    }

    ///// <summary>
    ///// 查詢電文失敗刪除此筆Etag資料
    ///// </summary>
    ///// <param name="receiveNumber"></param>
    ///// <param name="msg"></param>
    //private void DeleteUtilities_Etag(string receiveNumber, string msg)
    //{
    //    bool delResult = BRUtilities_Etag.DeleteUtilities_Etag(receiveNumber, "1");
    //    if (delResult)
    //    {
    //        msg += "；" + MessageHelper.GetMessage("01_01011200_010") + "，收件編號：" + receiveNumber + MessageHelper.GetMessage("01_01011200_011");
    //        Logging.SaveLog(ELogLayer.DB, msg);
    //    }
    //    else
    //    {
    //        msg += "；" + MessageHelper.GetMessage("01_01011200_010") + "，收件編號：" + receiveNumber + MessageHelper.GetMessage("01_01011200_012");
    //        Logging.SaveLog(ELogLayer.DB, msg);
    //    }
    //}

    ///// <summary>
    ///// 查詢電文失敗刪除此筆Etag車牌資料
    ///// </summary>
    ///// <param name="receiveNumber"></param>
    ///// <param name="msg"></param>
    //private void DeleteUtilities_Etag_PlateNo(string receiveNumber, string msg)
    //{
    //    bool delResult = BRUtilities_Etag.DeleteUtilities_Etag_PlateNo(receiveNumber, "1");
    //    if (delResult)
    //    {
    //        msg += "；" + MessageHelper.GetMessage("01_01011200_010") + "，收件編號：" + receiveNumber + MessageHelper.GetMessage("01_01011200_013");
    //        Logging.SaveLog(ELogLayer.DB, msg);
    //    }
    //    else
    //    {
    //        msg += "；" + MessageHelper.GetMessage("01_01011200_010") + "，收件編號：" + receiveNumber + MessageHelper.GetMessage("01_01011200_014");
    //        Logging.SaveLog(ELogLayer.DB, msg);
    //    }
    //}

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
        Hashtable htgResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCLD, htgInput, false, "1", eAgentInfo);

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
        htInput.Add("U_UPDATE_USER", agent_id.PadRight(8, '0'));    // 維護人員，員編or使用者代碼(左靠右補空白)
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

    ///// <summary>
    ///// Call 卡主機 確認是否有正卡
    ///// </summary>
    ///// <returns></returns>
    //private static bool checkCredit(string PrimaryCardholderID)
    //{
    //    string strMsg = string.Empty;

    //    Hashtable htInputP4_JCAA = new Hashtable();
    //    string strAcct_NBR = CommonFunction.GetSubString(PrimaryCardholderID.ToUpper(), 0, 16);
    //    htInputP4_JCAA.Add("USER_ID", eAgentInfo.agent_id);
    //    htInputP4_JCAA.Add("ACCT_NBR", strAcct_NBR);
    //    htInputP4_JCAA["LINE_CNT"] = "0000";
    //    htInputP4_JCAA.Add("FUNCTION_CODE", "1");// 1-透過身分證查詢 2-透過卡號查詢

    //    // 主機傳回信息
    //    Hashtable htResult = new Hashtable();

    //    if (CommonFunction.GetJCAAMainframeData(0, htInputP4_JCAA, ref htResult, eAgentInfo, ref strMsg) && !htResult.Contains("HtgMsg"))
    //    {
    //        // Host Message
    //        base.strHostMsg += htResult["HtgSuccess"] != null ? htResult["HtgSuccess"].ToString() : "P4_JCAA - 查詢成功";// 主機返回成功訊息
    //        return true;
    //    }
    //    else
    //    {
    //        if (!htResult.Contains("HtgMsg"))
    //            base.strHostMsg += htResult["HtgSuccess"] != null ? htResult["HtgSuccess"].ToString() : "";// 主機返回查詢成功訊息
    //        else
    //            base.strHostMsg += htResult["HtgMsg"] != null ? htResult["HtgMsg"].ToString() : "";// 主機返回失敗訊息

    //        // Client Message
    //        base.strClientMsg += strMsg;
    //        return false;
    //    }
    //}

    #endregion
}
