//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：卡片資料異動-BLK二KEY 解除管制(Delete)

//*  創建日期：2009/09/23
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
using CSIPKeyInGUI.BusinessRules;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Message;
using CSIPKeyInGUI.EntityLayer;
using Framework.Common.Logging;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using System.Drawing;

public partial class P010102110001 : PageBase
{
    #region 變數區
    /// <summary>
    /// 二KEY標識
    /// </summary>
    private string m_TwoKeyFlag = "2";

    /// <summary>
    /// Keyin標示
    /// </summary>
    private string m_UploadFlag = "Y";

    /// <summary>
    /// 異動類型D標識
    /// </summary>
    private string m_ChangeType = "D";

    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        //*獲得Session集合
        eAgentInfo = (EntityAGENT_INFO)Session["Agent"];
        sPageInfo = (structPageInfo)this.Session["PageInfo"]; //*記錄網頁訊息
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/09/23
    /// 修改日期：2009/09/23 
    /// <summary>
    /// 提交事件
    /// </summary>
    protected void btnSubmit_Click(object sender, EventArgs e)
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
            lblBlkCode.Text = htResult["BLOCK_CODE"].ToString();//*原CODE
            base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
            ViewState["HtgInfo"] = htResult;//*保存P4_JCAA主機查詢資料
            base.sbRegScript.Append("if(confirm('" + MessageHelper.GetMessage("01_01021100_003") + lblBlkCode.Text + MessageHelper.GetMessage("01_01021100_004") + "')) {$('#btnHiden').click();}");
        }
        else
        {
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
    /// 創建日期：2009/09/22
    /// 修改日期：2009/09/22 
    /// <summary>
    /// 隱藏Button事件
    /// </summary>
    protected void btnHiden_Click(object sender, EventArgs e)
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        //*查詢資料庫一Key資料
        EntitySet<EntityCHANGE_BLK> eChangeBlkSetOneKey = null;
        try
        {
            eChangeBlkSetOneKey = BRCHANGE_BLK.SelectEntitySet(this.txtCardNo.Text.Trim(), "1", "Y", "D");
        }
        catch
        {

            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            }
            return;

        }

        if (eChangeBlkSetOneKey != null && eChangeBlkSetOneKey.Count <= 0)
        {
            //查詢不到一Key資料
            //base.strClientMsg += MessageHelper.GetMessage("01_00000000_007");
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_00000000_007") + "');");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
            return;

        }

        //查到了一Key資料，檢核一、二Key是否為同一人
        //*判斷查詢的結果user_id欄位是否和Session["UserId"]相同
        if (eChangeBlkSetOneKey.GetEntity(0).user_id.Trim() == eAgentInfo.agent_id)
        {
            //base.strClientMsg += MessageHelper.GetMessage("01_00000000_008");
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_00000000_008") + "');");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
            return;
        }

        //*插入Table Change_BLK 中的資料
        EntityCHANGE_BLK eChangeBlk = new EntityCHANGE_BLK();
        eChangeBlk.Card_No = this.txtCardNo.Text.Trim();
        eChangeBlk.Change_Type = m_ChangeType;
        eChangeBlk.KeyIn_Flag = m_TwoKeyFlag;
        eChangeBlk.user_id = eAgentInfo.agent_id;
        eChangeBlk.mod_date = DateTime.Now.ToString("yyyyMMdd");
        eChangeBlk.ACTION_CODE = "";
        eChangeBlk.BLOCK_CODE = "";
        eChangeBlk.CWB_REGIONS = "";
        eChangeBlk.MEMO = "";
        eChangeBlk.PURGE_DATE = "";
        eChangeBlk.REASON_CODE = "";
        eChangeBlk.Upload_Flag = "";

        try
        {
            if (BRCHANGE_BLK.Insert(eChangeBlk, this.txtCardNo.Text.Trim(), m_TwoKeyFlag, m_UploadFlag, m_ChangeType))
            {
                //*添加上傳主機信息
                Hashtable htInput = new Hashtable();

                //*上傳主機傳回信息
                Hashtable htResult = new Hashtable();
                string strCardNo = this.txtCardNo.Text.Trim();
                //*根據卡號前4碼、前八碼判斷上傳主機類型
                if (CommonFunction.GetSubString(strCardNo, 0, 4) == "1234" || CommonFunction.GetSubString(strCardNo, 0, 8) == "40000160" || CommonFunction.GetSubString(strCardNo, 0, 8) == "40000162" || CommonFunction.GetSubString(strCardNo, 0, 8) == "40000189")
                {
                    Hashtable htReturn = (Hashtable)ViewState["HtgInfo"];
                    ArrayList arrayName = new ArrayList(new object[] { "ACCT_NBR" });//*P4_JCDH欄位名稱
                    Hashtable htOutput = new Hashtable();
                    MainFrameInfo.ChangeJCAAtoPCTI(htReturn, htOutput, arrayName, htReturn["TYPE"].ToString());

                    //*上傳主機<BLK CODE>為""
                    htOutput.Add("BLOCK_CODE", "");
                    htOutput.Add("FUNCTION_ID", "PCMH1");
                    //*提交P4_PCTI主機資料
                    htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4_PCTI, htOutput, false, "2", eAgentInfo);
                }
                else
                {
                    htInput.Add("ACCT_NBR", this.txtCardNo.Text.Trim());
                    //*上傳主機<BLK CODE>為""
                    htInput.Add("OASA_BLOCK_CODE", "");
                    //*上傳主機<作業類型>為"D"
                    htInput.Add("FUNCTION_CODE", m_ChangeType);
                    htInput.Add("SOURCE_CODE", "Z");//*交易來源別
                    htInput.Add("INHOUSE_INQ_FLAG", "N");//*IN-HOUSE INQUIRY ONLY
                    htInput.Add("NCCC_INQ_FLAG", "N");//*NCCC INQUIRY ONLY
                    htInput.Add("COUNTERFEIT_FLAG", "N");//*[保留]
                    //*提交OASA_P4_Submit主機資料
                    htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCAX, htInput, false, "2", eAgentInfo);

                }

                //*上傳主機資料成功
                if (!htResult.Contains("HtgMsg"))
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_01021100_001");
                    base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                    InsertCustomerLog();//*寫入BLK CODE資料更新檔customer_log
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
                }
                else
                {
                    if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                    {
                        base.strHostMsg += htResult["HtgMsg"].ToString();
                        base.strClientMsg += MessageHelper.GetMessage("01_01021100_002");
                    }
                    else
                    {
                        base.strClientMsg += htResult["HtgMsg"].ToString();
                    }
                }
            }
            else
            {
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_019");
            }
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
        }
        base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/09/23
    /// 修改日期：2009/09/23 
    /// <summary>
    /// 資料檔Customer_Log中寫入更改的BLK CODE欄位信息
    /// </summary>
    private void InsertCustomerLog()
    {
        structPageInfo sPageInfo = (structPageInfo)Session["PageInfo"];//*獲取Session中頁面的功能ID號
        EntityCUSTOMER_LOG eCustomerLog = new EntityCUSTOMER_LOG();
        eCustomerLog.query_key = this.txtCardNo.Text.Trim();
        eCustomerLog.trans_id = sPageInfo.strPageCode.Trim();
        eCustomerLog.field_name = "BLK CODE(D)";
        eCustomerLog.before = this.lblBlkCode.Text;
        eCustomerLog.after = " ";
        eCustomerLog.user_id = eAgentInfo.agent_id;
        eCustomerLog.mod_date = DateTime.Now.ToString("yyyyMMdd");
        eCustomerLog.mod_time = DateTime.Now.ToString("HHmmss");
        eCustomerLog.log_flag = "P4";
        try
        {
            BRCustomer_Log.AddEntity(eCustomerLog);
        }
        catch
        {

            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            }
        }
    }
}
