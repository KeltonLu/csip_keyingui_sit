//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：卡片資料異動-BLK二KEY 新增/異動(Change/Add)

//*  創建日期：2009/09/14
//*  修改記錄：

//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CSIPCommonModel.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Message;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using System.Drawing;

public partial class P010102090001 : PageBase
{
    #region 變數區
    /// <summary>
    /// 一KEY標識
    /// </summary>
    private string m_OneKeyFlag = "1";

    /// <summary>
    /// 二KEY標識
    /// </summary>
    private string m_TwoKeyFlag = "2";

    /// <summary>
    /// Keyin標示
    /// </summary>
    private string m_UploadFlag = "Y";

    /// <summary>
    /// 異動類型C標識
    /// </summary>
    private string m_ChangeType = "C";

    /// <summary>
    /// 異動類型A標識
    /// </summary>
    //private string m_ChangeTypeTwo = "A";

    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #endregion

    #region 事件區
    /// <summary>
    /// 頁面加載事件
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CommonFunction.SetControlsEnabled(pnlText, false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        //*獲得Session集合
        eAgentInfo = (EntityAGENT_INFO)Session["Agent"];
        sPageInfo = (structPageInfo)this.Session["PageInfo"]; //*記錄網頁訊息
        CommonFunction.SetControlsForeColor(this.pnlText, Color.Black);
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/09/14
    /// 修改日期：2009/09/14 
    /// <summary>
    /// 查詢事件
    /// </summary>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Account_Nbr = this.txtCardNo.Text;

        //20200109-修改SOC存入條件
        //log.Statement_Text = string.Format("AC_NO:{0}", log.Account_Nbr); //查詢條件內容: 用 | 區隔,要看文件確認
        log.Statement_Text = string.Format("CUSTOMER_ID:{0}|AC_NO:{1}|BRANCH_ID:{2}|ROLE_ID:{3}", log.Customer_Id, log.Account_Nbr, log.Branch_Nbr, log.Role_Id); //查詢條件內容: 用 | 區隔

        BRL_AP_LOG.Add(log);
        //------------------------------------------------------
        string strMsg = "";//*主機返回錯誤信息
        Hashtable htResult = new Hashtable();//*P4_JCAA主機傳回信息

        lblBlkCode.Text = "";
        if (CommonFunction.GetJCAAMainframeData(this.txtCardNo.Text.Trim(), ref htResult, eAgentInfo, ref strMsg))
        {
            ViewState["HtgInfo"] = htResult;//*保存P4_JCAA主機查詢資料
            CommonFunction.SetControlsEnabled(pnlText, true);
            lblBlkCode.Text = htResult["BLOCK_CODE"].ToString();//*原CODE
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_012");
            base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息 

            if (CheckSelectData())
            {
                base.sbRegScript.Append(BaseHelper.SetFocus("txtNewBlkCode"));//*設置新BLK CODE欄位焦點
                //*查詢二KEY資料
                EntitySet<EntityCHANGE_BLK> eChangeBlkSetTwoKey = null;
                try
                {
                    eChangeBlkSetTwoKey = BRCHANGE_BLK.SelectEntitySet(this.txtCardNo.Text.Trim(), m_TwoKeyFlag, m_UploadFlag, m_ChangeType);
                }
                catch
                {

                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                    return;

                }

                if (eChangeBlkSetTwoKey != null && eChangeBlkSetTwoKey.Count > 0)
                {
                    //*將資料檔里的資料賦值給畫面中的欄位【新BLK CODE】、【PURGE DATE】、【MEMO】、【REASON CODE】、【ACTION CODE】、【CWB REGIONS】
                    this.txtNewBlkCode.Text = eChangeBlkSetTwoKey.GetEntity(0).BLOCK_CODE;
                    this.txtPurgeDate.Text = eChangeBlkSetTwoKey.GetEntity(0).PURGE_DATE;
                    this.txtMemo.Text = eChangeBlkSetTwoKey.GetEntity(0).MEMO;
                    this.txtReasonCode.Text = eChangeBlkSetTwoKey.GetEntity(0).REASON_CODE;
                    this.txtActionCode.Text = eChangeBlkSetTwoKey.GetEntity(0).ACTION_CODE;
                    //this.txtCwbRegions.Text = eChangeBlkSetTwoKey.GetEntity(0).CWB_REGIONS;
                }
            }
        }
        else
        {
            CommonFunction.SetControlsEnabled(pnlText, false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
            if (!htResult.Contains("HtgMsg"))
            {
                base.strClientMsg += strMsg;
                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
            }
            else
            {
                //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
                etMstType = eMstType.Select;
                //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

                if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += strMsg;
                    base.strClientMsg += MessageHelper.GetMessage("01_00000000_026");
                }
                else
                {
                    base.strClientMsg += strMsg;
                }
            }
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/09/14
    /// 修改日期：2009/09/14 
    /// <summary>
    /// 提交事件
    /// </summary>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (CheckSelectData())
        {
            //*實體賦值
            EntityCHANGE_BLK eChangeBlk = new EntityCHANGE_BLK();
            eChangeBlk.BLOCK_CODE = this.txtNewBlkCode.Text.Trim().ToUpper();
            eChangeBlk.PURGE_DATE = this.txtPurgeDate.Text.Trim();
            eChangeBlk.MEMO = this.txtMemo.Text.Trim().ToUpper();
            eChangeBlk.REASON_CODE = this.txtReasonCode.Text.Trim().ToUpper();
            eChangeBlk.ACTION_CODE = this.txtActionCode.Text.Trim().ToUpper();
            eChangeBlk.CWB_REGIONS = "";
            eChangeBlk.user_id = eAgentInfo.agent_id;
            eChangeBlk.mod_date = DateTime.Now.ToString("yyyyMMdd");
            eChangeBlk.Card_No = this.txtCardNo.Text.Trim();
            eChangeBlk.Change_Type = m_ChangeType;
            eChangeBlk.KeyIn_Flag = m_TwoKeyFlag;
            eChangeBlk.Upload_Flag = "";

            //*查詢二KEY資料
            EntitySet<EntityCHANGE_BLK> eChangeBlkSetTwoKey = null;
            try
            {
                eChangeBlkSetTwoKey = BRCHANGE_BLK.SelectEntitySet(this.txtCardNo.Text.Trim(), m_TwoKeyFlag, m_UploadFlag, m_ChangeType);
            }
            catch
            {

                if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                }
                return;

            }

            if (eChangeBlkSetTwoKey != null && eChangeBlkSetTwoKey.Count > 0)
            {
                //*要更新欄位的數組
                string[] strField = { EntityCHANGE_BLK.M_BLOCK_CODE, EntityCHANGE_BLK.M_PURGE_DATE, EntityCHANGE_BLK.M_MEMO,
                                                  EntityCHANGE_BLK.M_REASON_CODE, EntityCHANGE_BLK.M_ACTION_CODE, EntityCHANGE_BLK.M_CWB_REGIONS,
                                                  EntityCHANGE_BLK.M_user_id, EntityCHANGE_BLK.M_mod_date};
                if (BRCHANGE_BLK.Update(eChangeBlk, this.txtCardNo.Text.Trim(), m_TwoKeyFlag, m_UploadFlag, m_ChangeType, strField) == false)
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("01_00000000_021")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("01_00000000_021");
                    }
                    return;
                }
            }
            else
            {
                //*插入二KEY資料
                try
                {
                    BRCHANGE_BLK.AddEntity(eChangeBlk);
                }
                catch
                {

                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("01_00000000_020")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("01_00000000_020");
                    }
                    return;

                }
            }

            if (CompareKey())
            {
                Hashtable htReturn = (Hashtable)ViewState["HtgInfo"];//*得到主機傳回信息    
                DataTable dtblUpdateData = CommonFunction.GetDataTable();
                string strDesp = "";
                string strCardNo = this.txtCardNo.Text.Trim();

                //*根據卡號前4碼、前八碼判斷上傳主機類型 
                if (CommonFunction.GetSubString(strCardNo, 0, 4) == "1234" || CommonFunction.GetSubString(strCardNo, 0, 8) == "40000160" || CommonFunction.GetSubString(strCardNo, 0, 8) == "40000162" || CommonFunction.GetSubString(strCardNo, 0, 8) == "40000189")
                {
                    strDesp = "BLK CODE" + "(PCTI-PCMH)";
                    if (!UpdateHtgPCMH(htReturn, dtblUpdateData, strDesp))
                    {
                        return;
                    }
                }
                else  //*OASA_P4
                {
                    strDesp = "BLK CODE" + "(JCAX)";
                    if (!UpdateHtgOASA(htReturn, dtblUpdateData, strDesp))
                    {
                        return;
                    }
                }

                //*寫入資料檔customer_log
                if (!CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, this.txtCardNo.Text.Trim(), "P4", (structPageInfo)Session["PageInfo"]))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }

                //*更新資料檔trans_num
                if (BRTRANS_NUM.UpdateTransNum("B12") == false)
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }

                //*更新Table ReIssue_Card
                eChangeBlk.Upload_Flag = m_UploadFlag;
                string[] strField = { EntityCHANGE_BLK.M_Upload_Flag, EntityCHANGE_BLK.M_mod_date };
                if (BRCHANGE_BLK.Update(eChangeBlk, this.txtCardNo.Text.Trim(), m_UploadFlag, m_ChangeType, strField) == false)
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }
                CommonFunction.SetControlsEnabled(this.pnlText, false);
                this.txtCardNo.Text = "";
                this.lblBlkCode.Text = "";
                base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));

            }

        }
    }

    #endregion

    #region 方法區


    /// 作者 趙呂梁
    /// 創建日期：2009/09/14
    /// 修改日期：2009/09/14 
    /// <summary>
    /// 修改主機PCMH資料
    /// </summary>
    /// <param name="htReturn">主機傳回資料</param>
    /// <param name="dtblUpdateData">更改的主機欄位信息的DataTable</param>
    /// <param name="strDesp">異動BLK CODE欄位名稱</param>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateHtgPCMH(Hashtable htReturn, DataTable dtblUpdateData, string strDesp)
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        string strBlkCode = htReturn["BLOCK_CODE"].ToString();//*主機BLK CODE

        ArrayList arrayName = new ArrayList(new object[] { "ACCT_NBR" });
        Hashtable htOutput = new Hashtable();
        MainFrameInfo.ChangeJCAAtoPCTI(htReturn, htOutput, arrayName, htReturn["TYPE"].ToString());
        htOutput.Add("BLOCK_CODE", strBlkCode);//*主機欄位< BLOCK_CODE>
        //*比對BLK CODE
        int intType = CommonFunction.ContrastDataEdit(htOutput, dtblUpdateData, this.txtNewBlkCode.Text.Trim().ToUpper(), "BLOCK_CODE", strDesp);

        //*若主機<BLK CODE>和畫面中輸入的欄位相同
        if (intType == 0)
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01020900_001");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
            return false;
        }

        if (CommonFunction.GetSubString(this.txtCardNo.Text.Trim(), 0, 8) == "40000189")
        {
            htOutput.Add("MEMO", CommonFunction.GetSubString(this.txtMemo.Text.Trim().ToUpper(), 0, 10));
        }

        htOutput.Add("FUNCTION_ID", "PCMH1");
        //*提交P4_PCTI主機資料
        Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4_PCTI, htOutput, false, "2", eAgentInfo);
        if (!htResult.Contains("HtgMsg"))
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01020900_002");
            base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
            return true;
        }
        else
        {
            if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            {
                base.strHostMsg += htResult["HtgMsg"].ToString();
                base.strClientMsg += MessageHelper.GetMessage("01_01020900_003") + strBlkCode;
            }
            else
            {
                base.strClientMsg += htResult["HtgMsg"].ToString();
            }
            return false;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/09/14
    /// 修改日期：2009/09/14 
    /// <summary>
    /// 修改主機OASA資料
    /// </summary>
    /// <param name="htReturn">主機傳回資料</param>
    /// <param name="dtblUpdateData">更改的主機欄位信息的DataTable</param>
    /// <param name="strDesp">異動BLK CODE欄位名稱</param>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateHtgOASA(Hashtable htReturn, DataTable dtblUpdateData, string strDesp)
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        string strBlkCode = htReturn["BLOCK_CODE"].ToString();//*主機BLK CODE

        if (strBlkCode == this.txtNewBlkCode.Text.Trim().ToUpper())
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01020900_001");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
            return false;
        }

        CommonFunction.UpdateLog(strBlkCode, this.txtNewBlkCode.Text.Trim().ToUpper(), strDesp, dtblUpdateData);

        //*得到卡號類型
        string strCardType = OpinionCardType();

        if (strCardType == "")
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01020900_004") + CommonFunction.GetSubString(this.txtCardNo.Text.Trim(), 0, 4);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
            return false;
        }

        Hashtable htInput = new Hashtable();//*上傳P4_JCAX修改主機資料
        if (strCardType == "V")
        {
            htInput.Add("OASA_REASON_CODE", this.txtReasonCode.Text.Trim().ToUpper());//*Reason Code
            htInput.Add("OASA_ACTION_CODE", this.txtActionCode.Text.Trim().ToUpper());
        }
        if (strCardType == "M")
        {
            htInput.Add("OASA_REASON_CODE", this.txtReasonCode.Text.Trim().ToUpper());//*Reason Code
        }
        if (strCardType == "J")
        {
            htInput.Add("OASA_REASON_CODE", this.txtReasonCode.Text.Trim().ToUpper());//*Reason Code
            htInput.Add("OASA_ACTION_CODE", this.txtActionCode.Text.Trim().ToUpper());
        }
        if (strCardType == "A" || strCardType == "U")
        {
            htInput.Add("OASA_ACTION_CODE", this.txtActionCode.Text.Trim().ToUpper());
        }
        htInput.Add("OASA_BLOCK_CODE", this.txtNewBlkCode.Text.Trim().ToUpper());//*BLK CODE
        htInput.Add("OASA_MEMO", CommonFunction.GetSubString(this.txtMemo.Text.Trim().ToUpper(), 0, 20));//*MEMO
        htInput.Add("OASA_PURGE_DATE", this.txtPurgeDate.Text.Trim());//*PURGE_DATE

        htInput.Add("FUNCTION_CODE", "C");
        htInput.Add("ACCT_NBR", this.txtCardNo.Text.Trim());
        htInput.Add("SOURCE_CODE", "Z");//*交易來源別
        htInput.Add("INHOUSE_INQ_FLAG", "N");//*IN-HOUSE INQUIRY ONLY
        htInput.Add("NCCC_INQ_FLAG", "N");//*NCCC INQUIRY ONLY
        htInput.Add("COUNTERFEIT_FLAG", "N");//*[保留]


        //*提交OASA_P4_Submit主機資料
        Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCAX, htInput, false, "2", eAgentInfo);
        if (!htResult.Contains("HtgMsg"))
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01020900_002");
            base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
            return true;
        }
        else
        {
            htInput["FUNCTION_CODE"] = "A";

            Hashtable htResultA = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCAX, htInput, false, "2", eAgentInfo);
            if (!htResultA.Contains("HtgMsg"))
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01020900_002");
                base.strHostMsg += htResultA["HtgSuccess"].ToString();//*主機返回成功訊息
                return true;
            }
            else
            {
                if (htResultA["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htResultA["HtgMsg"].ToString();
                    base.strClientMsg += MessageHelper.GetMessage("01_01020900_006") + strBlkCode;
                }
                else
                {
                    base.strClientMsg += htResultA["HtgMsg"].ToString();
                }
                return false;
            }
        }
    }


    /// 作者 趙呂梁
    /// 創建日期：2009/09/14
    /// 修改日期：2009/09/14 
    /// <summary>
    /// 判斷卡號類型
    /// </summary>
    /// <returns>卡號類型</returns>
    private string OpinionCardType()
    {
        string strCardType = "";//*卡號類型

        string strCardNo = CommonFunction.GetSubString(this.txtCardNo.Text.Trim(), 0, 4);
        if (strCardNo == "3560" || strCardNo == "3563" || strCardNo == "3565")
        {
            return strCardType = "J";
        }
        if (strCardNo == "4000")
        {
            return strCardType = "U";
        }
        if (strCardNo == "4311" || strCardNo == "4518" || strCardNo == "4563" || strCardNo == "4003" || strCardNo == "4304" || strCardNo == "4617")
        {
            return strCardType = "V";
        }
        if (strCardNo == "5408" || strCardNo == "5433" || strCardNo == "5477" || strCardNo == "5520" || strCardNo == "5466" || strCardNo == "5588")
        {
            return strCardType = "M";
        }
        if (CommonFunction.GetSubString(this.txtCardNo.Text.Trim(), 0, 5) == "03776")
        {
            return strCardType = "A";
        }
        return strCardType;//*卡號前4位不屬于以上情況，返回空（即：輸入卡別有誤）
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/09/14
    /// 修改日期：2009/09/14 
    /// <summary>
    /// 檢核查詢數據
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool CheckSelectData()
    {
        //*查詢一KEY資料
        EntitySet<EntityCHANGE_BLK> eChangeBlkSetOneKey = null;
        try
        {
            eChangeBlkSetOneKey = BRCHANGE_BLK.SelectEntitySet(this.txtCardNo.Text.Trim(), m_OneKeyFlag, m_UploadFlag, m_ChangeType);
        }
        catch
        {

            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            }
            return false;

        }

        //*如果存在一KEY資料
        if (eChangeBlkSetOneKey != null && eChangeBlkSetOneKey.Count > 0)
        {
            //*判斷查詢的結果user_id欄位是否和Session["UserId"]相同
            if (eChangeBlkSetOneKey.GetEntity(0).user_id.Trim() == eAgentInfo.agent_id)
            {
                //base.strClientMsg += MessageHelper.GetMessage("01_00000000_008");
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_00000000_008") + "');");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
                CommonFunction.SetControlsEnabled(pnlText, false);
                return false;
            }
            return true;
        }
        else
        {
            //base.strClientMsg += MessageHelper.GetMessage("01_00000000_007");
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_00000000_007") + "');");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
            CommonFunction.SetControlsEnabled(pnlText, false);
            return false;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/09/14
    /// 修改日期：2009/09/14 
    /// <summary>
    /// 比較一、二KEY資料是否相同
    /// </summary>
    /// <returns>true 相同，false 不同</returns>
    private bool CompareKey()
    {
        bool blnSame = true;

        //得到資料庫指定欄位的一Key資料
        string strColumns = EntityCHANGE_BLK.M_BLOCK_CODE + "," + EntityCHANGE_BLK.M_PURGE_DATE + "," + EntityCHANGE_BLK.M_MEMO + "," +
                                       EntityCHANGE_BLK.M_REASON_CODE + "," + EntityCHANGE_BLK.M_ACTION_CODE + "," + EntityCHANGE_BLK.M_CWB_REGIONS;
        DataSet dstInfo = BRCHANGE_BLK.Select(this.txtCardNo.Text.Trim(), m_OneKeyFlag, m_UploadFlag, m_ChangeType, strColumns);

        if (dstInfo == null)
        {
            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            }
            return false;        }        //*比較資料庫里的一Key資料是否和畫面中的輸入欄位一樣
        if (dstInfo != null && dstInfo.Tables[0].Rows.Count > 0)
        {
            int i = 0;//*記錄不相同的數量
            int j = 0;//*計數器
            foreach (System.Web.UI.Control control in pnlText.Controls)
            {
                if (control is CustTextBox)
                {
                    CustTextBox txtBox = (CustTextBox)control;
                    if (txtBox.Text.Trim().ToUpper() != dstInfo.Tables[0].Rows[0][j].ToString().Trim().ToUpper())
                    {
                        if (i == 0)
                        {
                            base.sbRegScript.Append(BaseHelper.SetFocus(txtBox.ID));
                        }
                        txtBox.ForeColor = Color.Red;
                        blnSame = false;
                        i++;
                    }
                    j++;
                }
            }
        }

        if (blnSame == false)
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_009");
        }
        return blnSame;
    }

    #endregion
}
