//******************************************************************
//*  作    者：趙呂梁

//*  功能說明：卡片資料異動-BLK一KEY 新增/異動(Change/Add)

//*  創建日期：2009/09/11
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

public partial class P010102080001 : PageBase
{
    #region 變數區
    /// <summary>
    /// 一KEY標識
    /// </summary>
    private string m_OneKeyFlag = "1";

    /// <summary>
    /// Keyin標示
    /// </summary>
    private string m_UploadFlag = "Y";

    /// <summary>
    /// 異動類型
    /// </summary>
    private string m_ChangeType = "C";

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
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/09/11
    /// 修改日期：2009/09/11 
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
            CommonFunction.SetControlsEnabled(pnlText, true);
            lblBlkCode.Text = htResult["BLOCK_CODE"].ToString();//*原CODE

            base.strClientMsg += MessageHelper.GetMessage("01_00000000_012");
            base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息 
            base.sbRegScript.Append(BaseHelper.SetFocus("txtNewBlkCode"));//*設置新BLK CODE欄位焦點
            //*查詢一KEY資料
            EntitySet<EntityCHANGE_BLK> eChangeBlkSet = null;
            try
            {
                eChangeBlkSet = BRCHANGE_BLK.SelectEntitySet(this.txtCardNo.Text.Trim(), m_OneKeyFlag, m_UploadFlag, m_ChangeType);
            }
            catch
            {

                if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                }
                return;

            }

            //*如果存在一KEY資料
            if (eChangeBlkSet != null && eChangeBlkSet.Count > 0)
            {
                //*將資料檔里的資料賦值給畫面中的欄位【新BLK CODE】、【PURGE DATE】、【MEMO】、【REASON CODE】、【ACTION CODE】、【CWB REGIONS】

                this.txtNewBlkCode.Text = eChangeBlkSet.GetEntity(0).BLOCK_CODE;
                this.txtPurgeDate.Text = eChangeBlkSet.GetEntity(0).PURGE_DATE;
                this.txtMemo.Text = eChangeBlkSet.GetEntity(0).MEMO;
                this.txtReasonCode.Text = eChangeBlkSet.GetEntity(0).REASON_CODE;
                this.txtActionCode.Text = eChangeBlkSet.GetEntity(0).ACTION_CODE;
                //this.txtCwbRegions.Text = eChangeBlkSet.GetEntity(0).CWB_REGIONS;
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
    /// 創建日期：2009/09/11
    /// 修改日期：2009/09/11 
    /// <summary>
    /// 提交事件
    /// </summary>
    protected void btnSubmit_Click(object sender, EventArgs e)
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
        eChangeBlk.KeyIn_Flag = m_OneKeyFlag;
        eChangeBlk.Upload_Flag = "";

        //*查詢一KEY資料
        EntitySet<EntityCHANGE_BLK> eChangeBlkSet = null;
        try
        {
            eChangeBlkSet = BRCHANGE_BLK.SelectEntitySet(this.txtCardNo.Text.Trim(), m_OneKeyFlag, m_UploadFlag, m_ChangeType);
        }
        catch
        {

            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            }
            return;

        }

        //*如果存在一KEY資料
        if (eChangeBlkSet != null && eChangeBlkSet.Count > 0)
        {
            //*要更新欄位的數組
            string[] strField = { EntityCHANGE_BLK.M_BLOCK_CODE, EntityCHANGE_BLK.M_PURGE_DATE, EntityCHANGE_BLK.M_MEMO,
                                                  EntityCHANGE_BLK.M_REASON_CODE, EntityCHANGE_BLK.M_ACTION_CODE, EntityCHANGE_BLK.M_CWB_REGIONS,
                                                  EntityCHANGE_BLK.M_user_id, EntityCHANGE_BLK.M_mod_date};
            if (BRCHANGE_BLK.Update(eChangeBlk, this.txtCardNo.Text.Trim(), m_OneKeyFlag, m_UploadFlag, m_ChangeType, strField) == false)
            {
                if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("01_00000000_004")) < 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_00000000_004");
                }
                return;
            }
        }
        else
        {
            //*插入一KEY資料
            try
            {
                BRCHANGE_BLK.AddEntity(eChangeBlk);
            }
            catch
            {

                if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("01_00000000_004")) < 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_00000000_004");
                }
                return;

            }
        }
        base.strClientMsg += MessageHelper.GetMessage("01_00000000_003");
        CommonFunction.SetControlsEnabled(pnlText, false);
        this.lblBlkCode.Text = "";
        this.txtCardNo.Text = "";
        base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
    }
    #endregion
}
