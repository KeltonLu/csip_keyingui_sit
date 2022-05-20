//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：系統維護-刪除作業

//*  創建日期：2009/10/8
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
using System.Drawing;
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using CSIPCommonModel.EntityLayer;
public partial class P010206000001 : PageBase
{
    #region 變數區

    /// <summary>
    /// 一KEY標識
    /// </summary>
    private string m_KeyInFlagOne = "1";

    /// <summary>
    /// 二KEY標識
    /// </summary>
    private string m_KeyInFlagTwo = "2";

    /// <summary>
    ///Keyin標示
    /// </summary>
    private string m_UploadFlag = "Y";

    /// <summary>
    /// 錯誤類型
    /// </summary>
    private string m_ErrorType = "ERROR:0";
    private EntityAGENT_INFO eAgentInfo;//*記錄登陸Session訊息   
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ClearText();
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            this.btnDelete.Enabled = false;
        }
        base.strClientMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"]; //*記錄網頁訊息
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/08
    /// 修改日期：2009/10/08 
    /// <summary>
    /// 查詢事件
    /// </summary>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtUserId.Text;

        //20200109-修改SOC存入條件
        //log.Statement_Text = string.Format("UserId:{0}", log.Customer_Id); //查詢條件內容: 用 | 區隔,要看文件確認
        log.Statement_Text = string.Format("CUSTOMER_ID:{0}|AC_NO:{1}|BRANCH_ID:{2}|ROLE_ID:{3}", log.Customer_Id, log.Account_Nbr, log.Branch_Nbr, log.Role_Id); //查詢條件內容: 用 | 區隔

        BRL_AP_LOG.Add(log);
        //------------------------------------------------------

        ClearText();
        EntitySet<EntityOTHER_BANK_TEMP> eOtherBankTempSet = null;
        try
        {
            //*查詢符合 收件編號和身分證號碼的資料
            eOtherBankTempSet = BROTHER_BANK_TEMP.SelectEntitySet(this.txtReceiveNumber.Text.Trim(), this.txtUserId.Text.Trim().ToUpper(), m_UploadFlag, m_KeyInFlagTwo);
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (eOtherBankTempSet != null && eOtherBankTempSet.Count > 0)
        {
            EntityOTHER_BANK_TEMP eOtherBankTemp = eOtherBankTempSet.GetEntity(0);
            this.lblBankCodeText.Text = eOtherBankTemp.Other_Bank_Code_S;
            this.lblBankAccNoText.Text = eOtherBankTemp.Other_Bank_Acc_No;
            this.lblBankPayWayText.Text = eOtherBankTemp.Other_Bank_Pay_Way;
            this.lblBankCusIDText.Text = eOtherBankTemp.Other_Bank_Cus_ID;
            this.lblBcycleCodeText.Text = eOtherBankTemp.bcycle_code;
            this.lblMobilePhoneText.Text = eOtherBankTemp.Mobile_Phone;
            this.lblEMailText.Text = eOtherBankTemp.E_Mail;
            this.lblEBillText.Text = eOtherBankTemp.E_Bill;
            this.lblBuildDateText.Text = eOtherBankTemp.Build_Date;
            this.lblApplyTypeText.Text = eOtherBankTemp.Apply_Type;
            this.lblDealNoText.Text = eOtherBankTemp.Deal_No;

            //*得到ACH回覆碼中文訊息和生效碼
            GetACHReturnCode(eOtherBankTemp.ACH_Return_Code, eOtherBankTemp.Batch_no, eOtherBankTemp.Pcmc_Upload_flag);
            //*得到7碼銀行代碼和銀行名稱
            GetBankCodeAndBankName();
            this.btnDelete.Enabled = true;
        }
        else
        {
            ClearText();
            this.btnDelete.Enabled = false;
            base.strClientMsg += MessageHelper.GetMessage("01_04040000_001");
            return;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/08
    /// 修改日期：2009/10/08 
    /// <summary>
    /// 刪除事件
    /// </summary>
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        DataSet dstResult = BROTHER_BANK_TEMP.Select(this.txtReceiveNumber.Text.Trim(), this.txtUserId.Text.Trim().ToUpper(), m_UploadFlag, m_KeyInFlagTwo);

        if (dstResult != null && dstResult.Tables[0].Rows.Count > 0)
        {
            //*得到欄位P02_flag
            string strP02Flag = dstResult.Tables[0].Rows[0][EntityBATCH.M_P02_flag.Trim()].ToString().Trim();

            if (strP02Flag == "N")
            {
                base.sbRegScript.Append("if(confirm('" + MessageHelper.GetMessage("01_04040000_002") + "')) {$('#btnHiden').click();}else{document.getElementById('txtUserId').focus();}");
            }
            if (strP02Flag == "Y")
            {
                //*Pcmc_Upload_flag欄位
                string strPcmcUploadFlag = dstResult.Tables[0].Rows[0][EntityOTHER_BANK_TEMP.M_Pcmc_Upload_flag.Trim()].ToString().Trim();
                //*Pcmc_Return_Code欄位
                string strPcmcReturnCode = dstResult.Tables[0].Rows[0][EntityOTHER_BANK_TEMP.M_Pcmc_Return_Code.Trim()].ToString().Trim();
                //*ACH_Return_Code欄位
                string strAchReturnCode = dstResult.Tables[0].Rows[0][EntityOTHER_BANK_TEMP.M_ACH_Return_Code.Trim()].ToString().Trim();
                if (strPcmcUploadFlag != "1" && strAchReturnCode == "0" && strPcmcReturnCode != "ERROR:0")
                {
                    base.sbRegScript.Append("if(confirm('" + MessageHelper.GetMessage("01_04040000_002") + "')) {$('#btnHiden1').click();}else{document.getElementById('txtUserId').focus();}");
                }

                if (strPcmcUploadFlag == "1")
                {
                    //base.strClientMsg += MessageHelper.GetMessage("01_04040000_004");
                    this.btnDelete.Enabled = false;
                }

                if (strPcmcReturnCode == "ERROR:0")
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_04040000_005");
                    this.btnDelete.Enabled = false;
                    return;
                }
            }
        }
        else
        {
            ClearText();
            base.strClientMsg += MessageHelper.GetMessage("01_04040000_001");
            this.btnDelete.Enabled = false;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/12
    /// 修改日期：2009/10/12 
    /// <summary>
    /// 隱藏Button刪除事件
    /// </summary>
    protected void btnHiden_Click(object sender, EventArgs e)
    {
        if (BROTHER_BANK_TEMP.Delete(this.txtReceiveNumber.Text.Trim(), this.txtUserId.Text.Trim().ToUpper(), m_UploadFlag, m_KeyInFlagOne, m_KeyInFlagTwo))
        {
            ClearAll();
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/12
    /// 修改日期：2009/10/12 
    /// <summary>
    /// 隱藏Button刪除事件
    /// </summary>
    protected void btnHiden1_Click(object sender, EventArgs e)
    {
        EntityOTHER_BANK_TEMP eOtherBankTemp = new EntityOTHER_BANK_TEMP();
        eOtherBankTemp.Pcmc_Return_Code = m_ErrorType;
        eOtherBankTemp.Pcmc_Upload_flag = "1";
        string[] strFields = { EntityOTHER_BANK_TEMP.M_Pcmc_Return_Code, EntityOTHER_BANK_TEMP.M_Pcmc_Upload_flag };

        BROTHER_BANK_TEMP.Update(eOtherBankTemp, this.txtReceiveNumber.Text.Trim(), this.txtUserId.Text.Trim().ToUpper(), m_UploadFlag, m_KeyInFlagTwo, strFields);
        ClearAll();
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/09
    /// 修改日期：2009/10/09 
    /// <summary>
    /// 清空網頁中的所有欄位，【收件編號】按鈕為網頁焦點，【刪除】按鈕disable
    /// </summary>
    private void ClearAll()
    {
        ClearText();
        base.strClientMsg += MessageHelper.GetMessage("01_04040000_003");
        this.txtReceiveNumber.Text = "";
        this.txtUserId.Text = "";
        base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
        this.btnDelete.Enabled = false;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/12
    /// 修改日期：2009/10/12 
    /// <summary>
    /// 清空文本
    /// </summary>
    private void ClearText()
    {
        this.lblBankCodeText.Text = "";
        this.lblBankAccNoText.Text = "";
        this.lblBankPayWayText.Text = "";
        this.lblBankCusIDText.Text = "";
        this.lblBcycleCodeText.Text = "";
        this.lblMobilePhoneText.Text = "";
        this.lblEMailText.Text = "";
        this.lblEBillText.Text = "";
        this.lblBuildDateText.Text = "";
        this.lblApplyTypeText.Text = "";
        this.lblDealNoText.Text = "";
        this.lblACHReturnCodeText.Text = "";
        this.lblRtnDateText.Text = "";
        this.lblRemarkText.Text = "";
        this.lblCodeText.Text = "";
        this.lblBankCodeLText.Text = "";
        this.lblBankNameText.Text = "";
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/09
    /// 修改日期：2009/10/09 
    /// <summary>
    /// 得到ACH回覆碼中文訊息和生效碼
    /// </summary>
    /// <param name="strACHReturnCode">ACH回覆碼</param>
    /// <param name="strBatchNo">批次編號</param>
    /// <param name="strPcmcUploadFlag">自扣申請書回貼主機動作標志</param>
    private void GetACHReturnCode(string strACHReturnCode, string strBatchNo, string strPcmcUploadFlag)
    {
        EntitySet<EntityACH_RTN_INFO> eAchRtnInfoSet = null;
        try
        {
            //*查詢ACH_RTN_INFO表得到ACH回覆碼中文訊息
            eAchRtnInfoSet = BRACH_RTN_INFO.SelectEntitySet(strACHReturnCode);
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (eAchRtnInfoSet != null && eAchRtnInfoSet.Count > 0)
        {
            this.lblACHReturnCodeText.Text = eAchRtnInfoSet.GetEntity(0).Ach_Rtn_Msg;
        }

        EntitySet<EntityBATCH> eBatch = null;

        try
        {
            eBatch = BRBATCH.SelectEntitySet(strBatchNo);
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }
        if (eBatch != null && eBatch.Count > 0)
        {
            //*處理註記
            if (eBatch.GetEntity(0).P02_flag == "Y")
            {
                this.lblRemarkText.Text = "*";
            }
            //*回覆日期
            this.lblRtnDateText.Text = eBatch.GetEntity(0).R02DateReceive;

            //*生效碼
            this.lblCodeText.Text = "N";
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/09
    /// 修改日期：2009/10/09 
    /// <summary>
    /// 得到7碼銀行代碼和銀行名稱
    /// </summary>
    private void GetBankCodeAndBankName()
    {
        //*查詢BankCodeCont

        DataSet dstInfo;
        dstInfo = BaseHelper.GetCommonPropertySet("01", "16", this.lblBankCodeText.Text.Trim());

        if (dstInfo == null)
        {
            //*查詢資料庫時發生錯誤
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;

        }
        else
        {
            if (dstInfo.Tables[0].Rows.Count > 0)
            {
                this.lblBankCodeLText.Text = dstInfo.Tables[0].Rows[0][0].ToString().Trim();
            }
        }

        dstInfo = BaseHelper.GetCommonPropertySet("01", "17", this.lblBankCodeText.Text.Trim());

        if (dstInfo == null)
        {
            //*查詢資料庫時發生錯誤
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;

        }
        else
        {
            if (dstInfo.Tables[0].Rows.Count > 0)
            {
                this.lblBankNameText.Text = dstInfo.Tables[0].Rows[0][0].ToString().Trim();
            }
        }

    }
}
