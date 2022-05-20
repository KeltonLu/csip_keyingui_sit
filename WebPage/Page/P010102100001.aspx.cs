//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：卡片資料異動-BLK一KEY 解除管制(Delete)

//*  創建日期：2009/09/22
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
using CSIPKeyInGUI.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Message;
using Framework.Common.Logging;

public partial class P010102100001 : PageBase
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
    /// 創建日期：2009/09/22
    /// 修改日期：2009/09/22 
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
            base.sbRegScript.Append("if(confirm('" + MessageHelper.GetMessage("01_01021000_001") + lblBlkCode.Text + MessageHelper.GetMessage("01_01021000_002") + "')) {$('#btnHiden').click();}");
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
        EntityCHANGE_BLK eChangeBlk = new EntityCHANGE_BLK();
        eChangeBlk.Card_No = this.txtCardNo.Text.Trim();
        eChangeBlk.Change_Type = m_ChangeType;
        eChangeBlk.KeyIn_Flag = m_OneKeyFlag;
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
            if (BRCHANGE_BLK.Insert(eChangeBlk, this.txtCardNo.Text.Trim(), m_OneKeyFlag, m_UploadFlag, m_ChangeType))
            {
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_003");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo"));
            }
            else
            {
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_004");
            }
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
        }
    }
}
