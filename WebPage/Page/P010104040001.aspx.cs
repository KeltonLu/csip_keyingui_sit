//******************************************************************
//*  作    者：趙呂梁

//*  功能說明：特店資料新增- PCAM二次鍵檔(PCAM新增二KEY)

//*  創建日期：2009/11/09
//*  修改記錄：
//*2010/12/21   zhen chen      RQ-2010-005537-000      
//*聯絡電話,機型,推廣姓名,推廣代碼,,SOURE,AVE,MONTH,
//*BRANCH,INT,MAIL,POS CA,POS MO,C/H,M/C
//*等欄位 , 二KEY 鍵檔畫面 , default 直接帶入 一KEY 資料
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
//20160627 (U) by Tank, 商店代號第3碼為5，status10預設=1

using System;
using System.Collections;
using System.Drawing;
using Framework.Common.Message;
using Framework.Common.Logging;
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using CSIPCommonModel.EntityLayer;

public partial class P010104040001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    //20191023 修改：SOC所需資訊  by Peggy
    private structPageInfo sPageInfo;//*記錄網頁訊息
    private string involve = "78360020,78360021,78360023,78360024,78360025,78360026,78360027,78360028,78360029";//自然人
    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ClearAll();
            base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
            //SetFeeDisable();
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        CommonFunction.SetControlsForeColor(pnlText, Color.Black);
        lblBusinessNameText.ForeColor = Color.Black;
        lblRegNameText.ForeColor = Color.Black;
        lblRegName.ForeColor = Color.Black;
        lblBusinessName.ForeColor = Color.Black;
        lblMerchantNameRight.ForeColor = Color.Black;
        this.txtCardNo1.ForeColor = Color.Black;
        this.txtCardNo2.ForeColor = Color.Black;

        sPageInfo = (structPageInfo)this.Session["PageInfo"];//20191023 修改：SOC所需資訊  by Peggy
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/09
    /// 修改日期：2009/11/09
    /// <summary>
    /// 查詢事件
    /// </summary>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        //20191023 修改：SOC所需資訊  by Peggy
        #region AuditLog to SOC
        /*
             Statement_Text：請提供以下屬性資料，各屬性間用'|'分隔，若無值仍需帶attribute name
                                        Ex.  CUSTOMER_ID= |AC_NO=123456789012|BRANCH_ID=0901|ROLE_ID=CBABG01
             (必輸)CUSTOMER_ID：客戶ID/統編
             AC_NO：交易帳號/卡號
             BRANCH_ID：帳務分行別
             ROLE_ID：登入系統帳號之角色
        */
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtCardNo1.Text.Trim() + txtCardNo2.Text.Trim();//查詢條件        
        log.Statement_Text = string.Format("CUSTOMER_ID:{0}|AC_NO:{1}|BRANCH_ID:{2}|ROLE_ID:{3}", log.Customer_Id, log.Account_Nbr, log.Branch_Nbr, log.Role_Id); //查詢條件內容: 用 | 區隔
        BRL_AP_LOG.Add(log);
        #endregion

        ClearPage(true);
        if (GetMainframeData())
        {

            //* 查詢一Key資料
            EntitySet<EntitySHOP_PCAM> eShopPcamSet1Key = null;
            try
            {
                eShopPcamSet1Key = BRSHOP_PCAM.SelectData(this.txtShopId.Text.Trim(), "1");
            }
            catch
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
                ClearPage(false);
                return;
            }

            if (eShopPcamSet1Key == null || eShopPcamSet1Key.Count < 1)
            {
                //*沒有一Key資料
                ClearPage(false);
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040202_013") + "');");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
                return;
            }

            if (eShopPcamSet1Key.GetEntity(0).send3270.Trim() == "Y")
            {
                ClearPage(false);
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040202_001") + "');");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
                return;
            }

            //*比較一Key和二Key的User是否為同一人
            if (eAgentInfo.agent_id.ToString().Trim() == eShopPcamSet1Key.GetEntity(0).keyin_userID.ToString().Trim())
            {
                //* 一、二Key不能為同一人
                ClearPage(false);
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040202_014") + "');");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
                return;
            }

            //*查詢二Key資料
            EntitySet<EntitySHOP_PCAM> eShopPcamSet2Key = null;
            try
            {
                //*根據商店代號查詢資料庫
                eShopPcamSet2Key = BRSHOP_PCAM.SelectData(this.txtShopId.Text.Trim(), "2");
            }
            catch
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                return;
            }

            CheckShopId();

            if (eShopPcamSet2Key != null && eShopPcamSet2Key.Count > 0)
            {
                if (eShopPcamSet2Key.GetEntity(0).send3270.Trim() != "Y")
                {
                    SetValues(eShopPcamSet2Key.GetEntity(0));
                    ViewState["EntityShopPcam"] = eShopPcamSet2Key.GetEntity(0);
                }
                else
                {
                    ClearPage(false);
                    base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040202_001") + "');");
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
                    return;
                }
            }
            else
            {
                //* add by zhenchen
                SetValuesOneKey(eShopPcamSet1Key.GetEntity(0));
                //* add end
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_040");
                this.txtBranch.Text = "3";
            }
            SetDisabledValue(eShopPcamSet1Key.GetEntity(0));
            DisabledColumn();
        }

        base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/09
    /// 修改日期：2009/11/09
    /// <summary>
    /// 新增事件
    /// </summary>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        // 20210527 EOS_AML(NOVA) 檢查最後異動分行 by Ares Dennis
        string LAST_UPD_BRANCH = this.txtLAST_UPD_BRANCH.Text.Trim();
        if (!string.IsNullOrEmpty(LAST_UPD_BRANCH))
        {
            //20211203_Ares_Jack_異動分行為9999不檢驗BRANCH, MAKER, CHECKER
            if (!(LAST_UPD_BRANCH == "9999"))
            {
                if (!checkLAS_UPD_BRANCH(LAST_UPD_BRANCH))
                {
                    base.sbRegScript.Append("alert('異動分行不存在，請重新填寫');");
                    return;
                }
            }
        }
        //20211122_EOS_AML(NOVA)_Ares_Jack_檢查MAKER
        string LAST_UPD_MAKER = this.txtLAST_UPD_MAKER.Text.Trim();
        if (!string.IsNullOrEmpty(LAST_UPD_MAKER))
        {
            //20211203_Ares_Jack_異動分行為9999不檢驗BRANCH, MAKER, CHECKER
            if (!(LAST_UPD_BRANCH == "9999"))
            {
                if (!checkLAS_UPD_MAKER(LAST_UPD_MAKER))
                {
                    base.sbRegScript.Append("alert('MAKER不存在，請重新填寫');");
                    return;
                }
            }
        }
        //20211122_EOS_AML(NOVA)_Ares_Jack_檢查CHECKER
        string LAST_UPD_CHECKER = this.txtLAST_UPD_CHECKER.Text.Trim();
        if (!string.IsNullOrEmpty(LAST_UPD_CHECKER))
        {
            //20211203_Ares_Jack_異動分行為9999不檢驗BRANCH, MAKER, CHECKER
            if (!(LAST_UPD_BRANCH == "9999"))
            {
                if (!checkLAS_UPD_CHECKER(LAST_UPD_CHECKER))
                {
                    base.sbRegScript.Append("alert('CHECKER不存在，請重新填寫');");
                    return;
                }
            }
        }

        DisabledColumn();
        //* 查詢一Key資料
        EntitySet<EntitySHOP_PCAM> eShopPcamSet1Key = null;
        try
        {

            eShopPcamSet1Key = BRSHOP_PCAM.SelectData(this.txtShopId.Text.Trim(), "1");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (eShopPcamSet1Key == null || eShopPcamSet1Key.Count < 1)
        {
            //*沒有一Key資料
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040202_013") + "');");
            return;
        }

        if (eShopPcamSet1Key.GetEntity(0).send3270.Trim() == "Y")
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040202_001") + "');");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
            return;
        }

        EntitySHOP_PCAM eShopPcam = eShopPcamSet1Key.GetEntity(0);
        ViewState["EntityShopPcam"] = eShopPcam;

        //*查詢二Key資料
        EntitySet<EntitySHOP_PCAM> eShopPcamSet2Key = null;
        try
        {
            //*根據商店代號查詢資料庫
            eShopPcamSet2Key = BRSHOP_PCAM.SelectData(this.txtShopId.Text.Trim(), "2");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (eShopPcamSet2Key != null && eShopPcamSet2Key.Count > 0)
        {
            if (eShopPcamSet2Key.GetEntity(0).send3270.Trim() != "Y")
            {
                //*查詢出有二Key資料，則更新資料
                if (UpdateData())
                {

                    //CommonFunction.SetControlsEnabled(pnlText, true);
                }
            }
            else
            {
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040202_001") + "');");
                base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
                return;
            }
        }
        else
        {
            //查詢出無二Key資料，則新增資料
            if (InsertNewData())
            {

                //CommonFunction.SetControlsEnabled(pnlText, true);
            }

        }



        if (Compare())
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01040202_004");            

            if (UploadHtg())//*異動主機資料成功
            {
                // 20210527 EOS_AML(NOVA) PCAM二KEY建檔成功，寫一筆異動紀錄 by Ares Dennis
                #region 異動記錄需報送AML  
                EntityAML_CHECKLOG eAMLCheckLog = new EntityAML_CHECKLOG();
                Hashtable htInput = GetUploadInfo(true);//得到PCAM P4A Submit異動主機資料

                #region 查詢JCGQ
                Hashtable htP4A_JCGQ = new Hashtable();
                htP4A_JCGQ.Add("FUNCTION_CODE", "I");
                htP4A_JCGQ.Add("CORP_NO", this.txtCardNo1.Text.Trim());// 統一編號1
                htP4A_JCGQ.Add("CORP_SEQ", this.txtCardNo2.Text.Trim());// 序號
                Hashtable htP4A_JCGQ2 = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGQ, htP4A_JCGQ, false, "21", eAgentInfo);
                #endregion
                //eAMLCheckLog.CORP_NO = htInput["DB_ACCT_NMBR"].ToString().Trim();
                //20211202_Ares_Jack_統編為自然人用, 將CORP_NO 改帶負責人ID, 不符合則把 CORP_NO欄位帶成 HEADQUARTER_CORPNO(總公司統編)
                if (involve.Contains(htP4A_JCGQ2["HEADQUARTER_CORPNO"].ToString().Trim()))
                    eAMLCheckLog.CORP_NO = htP4A_JCGQ2["OWNER_ID"].ToString().Trim();//負責人ID
                else
                    eAMLCheckLog.CORP_NO = htP4A_JCGQ2["CORP_NO"].ToString().Trim();

                eAMLCheckLog.MER_NO = htInput["ACCT"].ToString().Trim();
                eAMLCheckLog.TRANS_ID = "CSIPJCHQ";
                eAMLCheckLog.LAST_UPD_BRANCH = txtLAST_UPD_BRANCH.Text.Trim();
                eAMLCheckLog.LAST_UPD_CHECKER = txtLAST_UPD_CHECKER.Text.Trim();
                eAMLCheckLog.LAST_UPD_MAKER = txtLAST_UPD_MAKER.Text.Trim();
                eAMLCheckLog.MOD_USERID = eAgentInfo.agent_id;
                eAMLCheckLog.MOD_DATE = DateTime.Now.ToString("yyyyMMdd");
                eAMLCheckLog.MOD_TIME = DateTime.Now.ToString("HHmmss");

                InsertAMLCheckLog(eAMLCheckLog);
                #endregion

                if (!BRSHOP_PCAM.Update2KeyInfo(this.txtShopId.Text.Trim()))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }
                else
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_01040202_015");
                    ClearAll();
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
                }

            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01040202_003");
        }

    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/09
    /// 修改日期：2009/11/09
    /// <summary>
    /// 強制事件
    /// </summary>
    protected void btnForce_Click(object sender, EventArgs e)
    {
        base.strClientMsg += MessageHelper.GetMessage("01_01040202_010");
        if (UploadHtg())//*異動主機資料成功
        {
            UpdateData();//*更新資料庫欄位
            ClearAll();
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/09
    /// 修改日期：2009/11/09
    /// <summary>
    /// 城市資料檢核
    /// </summary>
    protected void btnCheck_Click(object sender, EventArgs e)
    {
        DisabledColumn();
        EntitySet<EntitySZIP> eSzipSet = null;
        try
        {
            eSzipSet = BRSZIP.SelectEntitySet(this.txtAddress1.Text.Trim());
            if (eSzipSet.Count > 0)
            {
                //*網頁中的區域號碼顯示為查詢結果
                txtZip.Text = eSzipSet.GetEntity(0).zip_code.Trim();
            }
            else
            {
                txtZip.Text = "";
                base.sbRegScript.Append("if(confirm('" + MessageHelper.GetMessage("01_01040202_012") + "')) {document.getElementById('txtAddress1').focus();}");
            }
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }
    }
    #endregion

    #region 方法區
    ///// 作者 趙呂梁
    ///// 創建日期：2009/11/10
    ///// 修改日期：2009/11/10
    ///// <summary>
    ///// 清空頁面
    ///// </summary>
    //private void ClearControl()
    //{
    //    base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
    //    string strShopId = this.txtShopId.Text.Trim();
    //    CommonFunction.SetControlsEnabled(pnlText, false);//*	清空網頁中的所有輸入欄位
    //    this.txtShopId.Enabled = true;
    //    this.txtShopId.Text = strShopId;
    //    this.btnAdd.Enabled = false;
    //    this.btnForce.Enabled = false;
    //}

    ///// 作者 趙呂梁
    ///// 創建日期：2009/11/10
    ///// 修改日期：2009/11/10
    ///// <summary>
    ///// 清空欄位設置費率欄位狀態
    ///// </summary>
    //private void SetControlStatus()
    //{
    //    this.txtMerchantName.Text = "";//*登記名稱
    //    this.txtCardNo1.Text = "";//*統一編號(1)
    //    this.txtCardNo2.Text = "";//*統一編號(2)
    //    this.txtEngName.Text = "";//*英文名稱
    //    this.txtAddress1.Text = ""; //*【帳單地址】(1)
    //    this.txtAddress2.Text = "";//*【帳單地址】(2)
    //    //this.txtAddress3.Text = "";//*【帳單地址】(3)
    //    this.txtMcc.Text = "";//*MCC
    //    this.txtInt.Text = "";//*INT
    //    this.txtMail.Text = "";//*MAIL
    //    this.txtPosCa.Text = "";//*POS_CA
    //    this.txtPosMo.Text = "";//*POS_MO
    //    this.txtCH.Text = "";//*C/H
    //    this.txtMC.Text = "";//*M/C
    //    this.txtDdaNo1.Text = "";//* 帳號(1)
    //    this.txtDdaNo2.Text = "";//* 帳號(2)

    //    //*費率01】~【費率08】

    //    this.txtDisRate1.Text = "";
    //    this.txtDisRate2.Text = "";
    //    this.txtDisRate3.Text = "";
    //    this.txtDisRate4.Text = "";
    //    this.txtDisRate5.Text = "";
    //    this.txtDisRate6.Text = "";
    //    this.txtDisRate7.Text = "";
    //    this.txtDisRate8.Text = "";

    //    if (this.txtStatus1.Text.Trim() == "")
    //    {
    //        this.txtDisRate1.Enabled = false;
    //    }
    //    if (this.txtStatus2.Text.Trim() == "")
    //    {
    //        this.txtDisRate2.Enabled = false;
    //    }
    //    if (this.txtStatus3.Text.Trim() == "")
    //    {
    //        this.txtDisRate3.Enabled = false;
    //    }
    //    if (this.txtStatus4.Text.Trim() == "")
    //    {
    //        this.txtDisRate4.Enabled = false;
    //    }
    //    if (this.txtStatus5.Text.Trim() == "")
    //    {
    //        this.txtDisRate5.Enabled = false;
    //    }
    //    if (this.txtStatus6.Text.Trim() == "")
    //    {
    //        this.txtDisRate6.Enabled = false;
    //    }
    //    if (this.txtStatus7.Text.Trim() == "")
    //    {
    //        this.txtDisRate7.Enabled = false;
    //    }
    //    if (this.txtStatus8.Text.Trim() == "")
    //    {
    //        this.txtDisRate8.Enabled = false;
    //    }
    //}

    ///// 作者 趙呂梁
    ///// 創建日期：2009/11/10
    ///// 修改日期：2009/11/10
    ///// <summary>
    ///// 設置【費率01】~【費率08】不可用
    ///// </summary>
    //private void SetFeeDisable()
    //{
    //    this.txtDisRate1.Enabled = false;
    //    this.txtDisRate2.Enabled = false;
    //    this.txtDisRate3.Enabled = false;
    //    this.txtDisRate4.Enabled = false;
    //    this.txtDisRate5.Enabled = false;
    //    this.txtDisRate6.Enabled = false;
    //    this.txtDisRate7.Enabled = false;
    //    this.txtDisRate8.Enabled = false;
    //}

    /// 作者 陳甄
    /// 創建日期：2010/12/21
    /// 修改日期：2009/12/21
    /// <param name="eShopPcam"></param>
    private void SetValuesOneKey(EntitySHOP_PCAM eShopPcam)
    {
        #region
        try
        {
            //Add by CTCB-Carolyn 2010.05.31
            //當MER_NAME_TYPE<>3時 , txtMerchantName欄位不顯示
            if (eShopPcam.MER_NAME_TYPE == "3")
            {
                this.txtMerchantName.Text = eShopPcam.merchant_name;//*帳列名稱
            }
            

            //*連絡電話
            if (!string.IsNullOrEmpty(eShopPcam.phone))
            {
                if (eShopPcam.phone.IndexOf('-') >= 0)
                {
                    string[] strPhones = eShopPcam.phone.Split(new char[] { '-' });
                    this.txtPhone1.Text = strPhones[0];
                    this.txtPhone2.Text = strPhones[1];
                    this.txtPhone3.Text = strPhones[2];
                }
                else
                {
                    if (eShopPcam.phone.Length >= 3)
                    {
                        this.txtPhone1.Text = eShopPcam.phone.Substring(0, 3);
                        this.txtPhone2.Text = eShopPcam.phone.Substring(3, eShopPcam.phone.Length - 3);
                    }
                    else
                    {
                        this.txtPhone1.Text = eShopPcam.phone;
                        this.txtPhone2.Text = "";
                        this.txtPhone3.Text = "";
                    }
                }
            }
            else
            {
                this.txtPhone1.Text = "";
                this.txtPhone2.Text = "";
                this.txtPhone3.Text = "";
            }

            this.txtSalesName.Text = eShopPcam.sales_name;//*推廣姓名
            this.txtAgentBank.Text = eShopPcam.agent_bank;//*推廣代碼
            //*機型
            this.txtImp1.Text = eShopPcam.imp1;//*IMP1
            this.txtImp2.Text = eShopPcam.imp2;//*IMP2
            this.txtPos1.Text = eShopPcam.pos1;//*POS1
            this.txtPos2.Text = eShopPcam.pos2;//*POS2
            this.txtEdc1.Text = eShopPcam.edc1;//*EDC1
            this.txtEdc2.Text = eShopPcam.edc2;//*EDC2

            this.txtSource.Text = eShopPcam.Source;//*SOURE
            
            this.txtAve.Text = eShopPcam.avg_tkt;//*AVE
            this.txtMonth.Text = eShopPcam.monthly_volume;//*Month
            this.txtBranch.Text = eShopPcam.branch;//*BRANCH
            this.txtInt.Text = eShopPcam.INT;//*INT
            this.txtMail.Text = eShopPcam.MAIL;//*MAIL
            this.txtPosCa.Text = eShopPcam.POS_CA;//*POS_CA
            this.txtPosMo.Text = eShopPcam.POS_MO;//*POS_MO
            this.txtCH.Text = eShopPcam.CH;//*C/H
            this.txtMC.Text = eShopPcam.MC;//*M/C

            // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis            
            this.txtLAST_UPD_BRANCH.Text = eShopPcam.LAST_UPD_BRANCH;// 資料最後異動分行
            this.txtLAST_UPD_CHECKER.Text = eShopPcam.LAST_UPD_CHECKER;// 資料最後異動-CHECKER
            this.txtLAST_UPD_MAKER.Text = eShopPcam.LAST_UPD_MAKER;// 資料最後異動-MAKER
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
        }
        #endregion

    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/10
    /// 修改日期：2009/11/10
    /// <summary>
    /// 為頁面欄位賦值
    /// </summary>
    /// <param name="eShopPcam">EntitySHOP_PCAM</param>
    private void SetValues(EntitySHOP_PCAM eShopPcam)
    {
        #region
        //Add by CTCB-Carolyn 2010.05.31
        //當MER_NAME_TYPE<>3時 , txtMerchantName欄位不顯示
        if (eShopPcam.MER_NAME_TYPE == "3")
        {
            this.txtMerchantName.Text = eShopPcam.merchant_name;//*帳列名稱
        }

        ////*統一編號
        //if (!string.IsNullOrEmpty(eShopPcam.corp))
        //{
        //    if (eShopPcam.corp.Length >= 8)
        //    {
        //        this.txtCardNo1.Text = eShopPcam.corp.Substring(0, 8);
        //        this.txtCardNo2.Text = eShopPcam.corp.Remove(0, 8);
        //    }
        //    else
        //    {
        //        this.txtCardNo1.Text = eShopPcam.corp;
        //        this.txtCardNo2.Text = "";
        //    }
        //}
        //else
        //{
        //    this.txtCardNo1.Text = "";
        //    this.txtCardNo2.Text = "";
        //}
        this.txtEngName.Text = eShopPcam.merchdba_name;//*英文名稱
        this.txtDbaCity.Text = eShopPcam.dba_city;//*城市
        this.txtZip.Text = eShopPcam.zip;//*區域編號

        this.txtAddress1.Text = eShopPcam.address1;//*【帳單地址】(1)
        this.txtAddress2.Text = eShopPcam.address2;//*【帳單地址】(2)
        //this.txtAddress3.Text = eShopPcam.address3;//*【帳單地址】(3)

        //*連絡電話
        if (!string.IsNullOrEmpty(eShopPcam.phone))
        {
            if (eShopPcam.phone.IndexOf('-') >= 0)
            {
                string[] strPhones = eShopPcam.phone.Split(new char[] { '-' });
                this.txtPhone1.Text = strPhones[0];
                this.txtPhone2.Text = strPhones[1];
                this.txtPhone3.Text = strPhones[2];
            }
            else
            {
                if (eShopPcam.phone.Length >= 3)
                {
                    this.txtPhone1.Text = eShopPcam.phone.Substring(0, 3);
                    this.txtPhone2.Text = eShopPcam.phone.Substring(3, eShopPcam.phone.Length - 3);
                }
                else
                {
                    this.txtPhone1.Text = eShopPcam.phone;
                    this.txtPhone2.Text = "";
                    this.txtPhone3.Text = "";
                }
            }
        }
        else
        {
            this.txtPhone1.Text = "";
            this.txtPhone2.Text = "";
            this.txtPhone3.Text = "";
        }

        this.txtOwner.Text = eShopPcam.owner;//*負責人英文名
        this.txtImp1.Text = eShopPcam.imp1;//*IMP1
        this.txtPos1.Text = eShopPcam.pos1;//*POS1
        this.txtEdc1.Text = eShopPcam.edc1;//*EDC1
        this.txtImp2.Text = eShopPcam.imp2;//*IMP2
        this.txtPos2.Text = eShopPcam.pos2;//*POS2
        this.txtEdc2.Text = eShopPcam.edc2;//*EDC2
        this.txtSalesName.Text = eShopPcam.sales_name;//*推廣姓名
        this.txtAgentBank.Text = eShopPcam.agent_bank;//*推廣代碼
        this.txtType.Text = eShopPcam.merchant_type;//*Type
        this.txtStore.Text = eShopPcam.chain_store;//*STORE
        this.txtMerch.Text = eShopPcam.chain_merch;//*MERCH
        this.txtLevel.Text = eShopPcam.chain_level;//*LEVEL
        this.txtSource.Text = eShopPcam.Source;//*SOURE
        this.txtMcc.Text = eShopPcam.mcc_VISA;//*MCC
        this.txtAve.Text = eShopPcam.avg_tkt;//*AVE
        this.txtMonth.Text = eShopPcam.monthly_volume;//*Month
        this.txtBranch.Text = eShopPcam.branch;//*BRANCH
        this.txtInt.Text = eShopPcam.INT;//*INT
        this.txtMail.Text = eShopPcam.MAIL;//*MAIL
        this.txtPosCa.Text = eShopPcam.POS_CA;//*POS_CA
        this.txtPosMo.Text = eShopPcam.POS_MO;//*POS_MO
        this.txtCH.Text = eShopPcam.CH;//*C/H
        this.txtMC.Text = eShopPcam.MC;//*M/C
        this.txtFileNo.Text = eShopPcam.FileNo;//*歸檔編號
        this.txtHOLDSTMT.Text = eShopPcam.hold_stmt;//*Hold STMT

        SelectRadioButton(eShopPcam.MER_NAME_TYPE.Trim());//*帳列類型選擇

        //*帳號
        if (!string.IsNullOrEmpty(eShopPcam.DDA_NO))
        {
            if (eShopPcam.DDA_NO.Length >= 3)
            {
                this.txtDdaNo1.Text = eShopPcam.DDA_NO.Substring(0, 3);
                this.txtDdaNo2.Text = eShopPcam.DDA_NO.Substring(3, eShopPcam.DDA_NO.Length - 3).Trim();
            }
            else
            {
                this.txtDdaNo1.Text = eShopPcam.DDA_NO;
                this.txtDdaNo2.Text = "";
            }
        }
        else
        {
            this.txtDdaNo1.Text = "";
            this.txtDdaNo2.Text = "";
        }

        //*Status-01-15
        this.txtStatus1.Text = eShopPcam.status1;
        this.txtStatus2.Text = eShopPcam.status2;
        this.txtStatus3.Text = eShopPcam.status3;
        this.txtStatus4.Text = eShopPcam.status4;
        this.txtStatus5.Text = eShopPcam.status5;
        this.txtStatus6.Text = eShopPcam.status6;
        this.txtStatus7.Text = eShopPcam.status7;
        this.txtStatus8.Text = eShopPcam.status8;
        this.txtStatus9.Text = eShopPcam.status9;
        this.txtStatus10.Text = eShopPcam.status10;
        this.txtStatus11.Text = eShopPcam.status11;
        this.txtStatus12.Text = eShopPcam.status12;
        this.txtStatus13.Text = eShopPcam.status13;
        this.txtStatus14.Text = eShopPcam.status14;
        this.txtStatus15.Text = eShopPcam.status15;

        //*費率01-15
        this.txtDisRate1.Text = eShopPcam.dis_rate1;
        this.txtDisRate2.Text = eShopPcam.dis_rate2;
        this.txtDisRate3.Text = eShopPcam.dis_rate3;
        this.txtDisRate4.Text = eShopPcam.dis_rate4;
        this.txtDisRate5.Text = eShopPcam.dis_rate5;
        this.txtDisRate6.Text = eShopPcam.dis_rate6;
        this.txtDisRate7.Text = eShopPcam.dis_rate7;
        this.txtDisRate8.Text = eShopPcam.dis_rate8;
        this.txtDisRate9.Text = eShopPcam.dis_rate9;
        this.txtDisRate10.Text = eShopPcam.dis_rate10;
        this.txtDisRate11.Text = eShopPcam.dis_rate11;
        this.txtDisRate12.Text = eShopPcam.dis_rate12;
        this.txtDisRate13.Text = eShopPcam.dis_rate13;
        this.txtDisRate14.Text = eShopPcam.dis_rate14;
        this.txtDisRate15.Text = eShopPcam.dis_rate15;

        // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis            
        this.txtLAST_UPD_BRANCH.Text = eShopPcam.LAST_UPD_BRANCH;// 資料最後異動分行
        this.txtLAST_UPD_CHECKER.Text = eShopPcam.LAST_UPD_CHECKER;// 資料最後異動-CHECKER
        this.txtLAST_UPD_MAKER.Text = eShopPcam.LAST_UPD_MAKER;// 資料最後異動-MAKER
        #endregion
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/11
    /// 修改日期：2009/11/11
    /// <summary>
    /// 得到資料庫中{費率01}~{費率08}顯示在網頁中的值
    /// </summary>
    /// <param name="strDataValue">資料庫中費率</param>
    /// <param name="txtBox">費率對應的TextBox</param>
    private void GetFeeValue(string strDataValue, CustTextBox txtBox)
    {
        if (!string.IsNullOrEmpty(strDataValue) && strDataValue != "0")
        {
            float intFeeValue = float.Parse(strDataValue) / 1000;
            txtBox.Text = intFeeValue.ToString();
            txtBox.Enabled = true;
        }
        else
        {
            txtBox.Text = "";
        }
    }

    /// <summary>
    /// 得到資料庫中{費率01}~{費率08}顯示在網頁中的值
    /// </summary>
    /// <param name="strDataValue">資料庫中費率</param>
    /// <returns></returns>
    private string GetFeeValueText(string strDataValue)
    {
        string strRet = "";

        if (!string.IsNullOrEmpty(strDataValue) && strDataValue != "0")
        {
            float intFeeValue = float.Parse(strDataValue) / 1000;
            strRet = intFeeValue.ToString();
        }
        else
        {
            strRet = "";
        }

        return strRet;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/11
    /// 修改日期：2009/11/11
    /// <summary>
    /// 得到異動主機費率欄位值
    /// </summary>
    /// <param name="strValue">頁面費率01-08欄位值</param>
    /// <returns>轉換后的費率值</returns>
    private string GetFeeValue(string strValue)
    {
        if (!string.IsNullOrEmpty(strValue))
        {
            float intTemp = float.Parse(strValue) * 1000;
            return intTemp.ToString().PadLeft(5, '0');
        }
        else
        {
            return "00000";
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/11
    /// 修改日期：2009/11/11
    /// <summary>
    /// 比較一次鍵檔和二次鍵檔的資料
    /// </summary>
    /// <returns>true相同，false不同</returns>
    private bool Compare()
    {
        bool blnSame = true;
        int intCount = 0;//*記錄不相同的數量
        string strTel1 = "";
        string strTel2 = "";
        string strTel3 = "";

        if (ViewState["EntityShopPcam"] != null)
        {
            EntitySHOP_PCAM eShopPcam = (EntitySHOP_PCAM)ViewState["EntityShopPcam"];

            if (this.radMerchantName.Checked == true)
            {
                //*比較【帳列名稱】
                GetCompareResult(txtMerchantName, eShopPcam.merchant_name, ref intCount, ref blnSame);
            }

            if (this.radRegName.Checked == true)
            {
                //*比較【帳列名稱】
                GetCompareResult(lblRegNameText, eShopPcam.merchant_name, ref intCount, ref blnSame);
            }

            if (this.radBusinessName.Checked == true)
            {
                //*比較【帳列名稱】
                GetCompareResult(lblBusinessNameText, eShopPcam.merchant_name, ref intCount, ref blnSame);
            }

            string strType = GetMerNameType();
            if (strType != eShopPcam.MER_NAME_TYPE.Trim())
            {
                intCount++;

                if (strType == "1")
                {
                    this.lblRegName.ForeColor = Color.Red;
                }

                if (strType == "2")
                {
                    this.lblBusinessName.ForeColor = Color.Red;
                }

                if (strType == "3")
                {
                    this.lblMerchantNameRight.ForeColor = Color.Red;
                }
                blnSame = false;
            }

            string strCardNo1 = "";//*統編序號(1)
            string strCardNo2 = "";//*統編序號(2)
            //*統編序號
            if (!string.IsNullOrEmpty(eShopPcam.corp))
            {
                if (eShopPcam.corp.Length >= 8)
                {
                    strCardNo1 = eShopPcam.corp.Substring(0, 8);
                    strCardNo2 = eShopPcam.corp.Remove(0, 8);
                }
                else
                {
                    strCardNo1 = eShopPcam.corp;
                    strCardNo2 = "";
                }
            }

            //*比較統編序號(1)
            GetCompareResult(txtCardNo1, strCardNo1, ref intCount, ref blnSame);

            //*比較統編序號(2)
            GetCompareResult(txtCardNo2, strCardNo2, ref intCount, ref blnSame);

            //*比較郵區編號
            GetCompareResult(txtZip, eShopPcam.zip, ref intCount, ref blnSame);

            //*比較【帳單地址1】

            GetCompareResult(txtAddress1, eShopPcam.address1, ref intCount, ref blnSame);

            //*比較【帳單地址2】

            GetCompareResult(txtAddress2, eShopPcam.address2, ref intCount, ref blnSame);

            //*比較【帳單地址3】

            //GetCompareResult(txtAddress3, eShopPcam.address3, ref intCount, ref blnSame);

            //*比較聯絡電話
            //*連絡電話
            if (!string.IsNullOrEmpty(eShopPcam.phone))
            {
                if (eShopPcam.phone.IndexOf('-') >= 0)
                {
                    string[] strPhones = eShopPcam.phone.Split(new char[] { '-' });

                    strTel1 = strPhones[0];
                    strTel2 = strPhones[1];
                    strTel3 = strPhones[2];
                }
                else
                {
                    if (eShopPcam.phone.Length >= 3)
                    {
                        strTel1 = eShopPcam.phone.Substring(0, 3);
                        strTel2 = eShopPcam.phone.Substring(3, eShopPcam.phone.Length - 3);
                    }
                    else
                    {
                        strTel1 = eShopPcam.phone;
                        strTel2 = "";
                        strTel3 = "";
                    }
                }
            }
            else
            {
                strTel1 = "";
                strTel2 = "";
                strTel3 = "";
            }

            GetCompareResult(txtPhone1, strTel1, ref intCount, ref blnSame);
            GetCompareResult(txtPhone2, strTel2, ref intCount, ref blnSame);
            GetCompareResult(txtPhone3, strTel3, ref intCount, ref blnSame);

            //*比較IMP1
            GetCompareResult(txtImp1, eShopPcam.imp1, ref intCount, ref blnSame);

            //*比較POS1
            GetCompareResult(txtPos1, eShopPcam.pos1, ref intCount, ref blnSame);

            //*比較EDC1
            GetCompareResult(txtEdc1, eShopPcam.edc1, ref intCount, ref blnSame);


            //*比較IMP2
            GetCompareResult(txtImp2, eShopPcam.imp2, ref intCount, ref blnSame);

            //*比較POS2
            GetCompareResult(txtPos2, eShopPcam.pos2, ref intCount, ref blnSame);

            //*比較EDC2
            GetCompareResult(txtEdc2, eShopPcam.edc2, ref intCount, ref blnSame);

            //*比較TYPE
            GetCompareResult(txtType, eShopPcam.merchant_type, ref intCount, ref blnSame);

            //*比較STORE
            GetCompareResult(txtStore, eShopPcam.chain_store, ref intCount, ref blnSame);

            //*比較MERCH
            GetCompareResult(txtMerch, eShopPcam.chain_merch, ref intCount, ref blnSame);

            //*比較 LEVEL
            GetCompareResult(txtLevel, eShopPcam.chain_level, ref intCount, ref blnSame);


            //*比較 SOURCE
            GetCompareResult(txtSource, eShopPcam.Source, ref intCount, ref blnSame);


            //*比較【MCC】

            GetCompareResult(txtMcc, eShopPcam.mcc_VISA, ref intCount, ref blnSame);

            //*比較 AVE
            GetCompareResult(txtAve, eShopPcam.avg_tkt, ref intCount, ref blnSame);

            //*比較 MONTH
            GetCompareResult(txtMonth, eShopPcam.monthly_volume, ref intCount, ref blnSame);

            //*比較 BRANCH
            GetCompareResult(txtBranch, eShopPcam.branch, ref intCount, ref blnSame);

            //*比較【INT】

            GetCompareResult(txtInt, eShopPcam.INT, ref intCount, ref blnSame);

            //*比較【MAIL】

            GetCompareResult(txtMail, eShopPcam.MAIL, ref intCount, ref blnSame);

            //*比較【POS CA】

            GetCompareResult(txtPosCa, eShopPcam.POS_CA, ref intCount, ref blnSame);

            //*比較【POS MO】

            GetCompareResult(txtPosMo, eShopPcam.POS_MO, ref intCount, ref blnSame);

            //*比較【C/H】

            GetCompareResult(txtCH, eShopPcam.CH, ref intCount, ref blnSame);

            //*比較【M/C】

            GetCompareResult(txtMC, eShopPcam.MC, ref intCount, ref blnSame);

            //*比較HOLD STMT

            GetCompareResult(txtHOLDSTMT, eShopPcam.hold_stmt, ref intCount, ref blnSame);

            //*比較帳號
            string sDDA_NO = eShopPcam.DDA_NO;
            GetCompareResult(txtDdaNo1, sDDA_NO.Substring(0, 3), ref intCount, ref blnSame);
            GetCompareResult(txtDdaNo2, sDDA_NO.Substring(3, eShopPcam.DDA_NO.Length - 3).Trim(), ref intCount, ref blnSame);

            //*比較status01-15
            GetCompareResult(txtStatus1, eShopPcam.status1, ref intCount, ref blnSame);
            GetCompareResult(txtStatus2, eShopPcam.status2, ref intCount, ref blnSame);
            GetCompareResult(txtStatus3, eShopPcam.status3, ref intCount, ref blnSame);
            GetCompareResult(txtStatus4, eShopPcam.status4, ref intCount, ref blnSame);
            GetCompareResult(txtStatus5, eShopPcam.status5, ref intCount, ref blnSame);
            GetCompareResult(txtStatus6, eShopPcam.status6, ref intCount, ref blnSame);
            GetCompareResult(txtStatus7, eShopPcam.status7, ref intCount, ref blnSame);
            GetCompareResult(txtStatus8, eShopPcam.status8, ref intCount, ref blnSame);
            GetCompareResult(txtStatus9, eShopPcam.status9, ref intCount, ref blnSame);
            GetCompareResult(txtStatus10, eShopPcam.status10, ref intCount, ref blnSame);
            GetCompareResult(txtStatus11, eShopPcam.status11, ref intCount, ref blnSame);
            GetCompareResult(txtStatus12, eShopPcam.status12, ref intCount, ref blnSame);
            GetCompareResult(txtStatus13, eShopPcam.status13, ref intCount, ref blnSame);
            GetCompareResult(txtStatus14, eShopPcam.status14, ref intCount, ref blnSame);
            GetCompareResult(txtStatus15, eShopPcam.status15, ref intCount, ref blnSame);

            //比較費率01-15
            GetCompareResultFee(txtDisRate1, eShopPcam.dis_rate1, ref intCount, ref blnSame);
            GetCompareResultFee(txtDisRate2, eShopPcam.dis_rate2, ref intCount, ref blnSame);
            GetCompareResultFee(txtDisRate3, eShopPcam.dis_rate3, ref intCount, ref blnSame);
            GetCompareResultFee(txtDisRate4, eShopPcam.dis_rate4, ref intCount, ref blnSame);
            GetCompareResultFee(txtDisRate5, eShopPcam.dis_rate5, ref intCount, ref blnSame);
            GetCompareResultFee(txtDisRate6, eShopPcam.dis_rate6, ref intCount, ref blnSame);
            GetCompareResultFee(txtDisRate7, eShopPcam.dis_rate7, ref intCount, ref blnSame);
            GetCompareResultFee(txtDisRate8, eShopPcam.dis_rate8, ref intCount, ref blnSame);
            GetCompareResultFee(txtDisRate9, eShopPcam.dis_rate9, ref intCount, ref blnSame);
            GetCompareResultFee(txtDisRate10, eShopPcam.dis_rate10, ref intCount, ref blnSame);
            GetCompareResultFee(txtDisRate11, eShopPcam.dis_rate11, ref intCount, ref blnSame);
            GetCompareResultFee(txtDisRate12, eShopPcam.dis_rate12, ref intCount, ref blnSame);
            GetCompareResultFee(txtDisRate13, eShopPcam.dis_rate13, ref intCount, ref blnSame);
            GetCompareResultFee(txtDisRate14, eShopPcam.dis_rate14, ref intCount, ref blnSame);
            GetCompareResultFee(txtDisRate15, eShopPcam.dis_rate15, ref intCount, ref blnSame);

            // 20210527 EOS_AML(NOVA) 比較最後異動 by Ares Dennis
            GetCompareResult(txtLAST_UPD_BRANCH, eShopPcam.LAST_UPD_BRANCH, ref intCount, ref blnSame);
            GetCompareResult(txtLAST_UPD_CHECKER, eShopPcam.LAST_UPD_CHECKER, ref intCount, ref blnSame);
            GetCompareResult(txtLAST_UPD_MAKER, eShopPcam.LAST_UPD_MAKER, ref intCount, ref blnSame);

            return blnSame;
        }
        else
        {
            return false;
        }
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/11/11
    /// 修改日期：2009/11/11
    /// <summary>
    /// 得到欄位值與一次鍵檔的比較結果
    /// </summary>
    /// <param name="txtBox">頁面欄位的TextBox</param>
    /// <param name="strValue">一次鍵檔的欄位值</param>
    /// <param name="intCount">不同的欄位數量</param>
    /// <param name="blnSame">是否相同</param>
    /// <returns>true相同，false不相同</returns>
    private bool GetCompareResult(CustTextBox txtBox, string strValue, ref int intCount, ref bool blnSame)
    {
        if (txtBox.Text.ToUpper().Trim() != NullToString(strValue).ToUpper().Trim())
        {
            intCount++;
            if (intCount == 1)
            {
                if (txtBox.Enabled == true)
                {
                    base.sbRegScript.Append(BaseHelper.SetFocus(txtBox.ID));
                }
            }
            //txtBox.ForeColor = Color.Red;
            txtBox.BackColor = Color.Red;
            blnSame = false;
        }
        else
        {
            txtBox.BackColor = Color.White;
        }
        return blnSame;
    }

    /// 創建日期：2009/11/11
    /// 修改日期：2009/11/11
    /// <summary>
    /// 得到欄位值與一次鍵檔的比較結果
    /// </summary>
    /// <param name="lblText">頁面Lable</param>
    /// <param name="strValue">一次鍵檔的欄位值</param>
    /// <param name="intCount">不同的欄位數量</param>
    /// <param name="blnSame">是否相同</param>
    /// <returns>true相同，false不相同</returns>
    private bool GetCompareResult(CustLabel lblText, string strValue, ref int intCount, ref bool blnSame)
    {
        string sInputValue = "";

        if (lblText.ID == "lblRegNameText" || lblText.ID == "lblBusinessNameText")
        {

            sInputValue = lblText.Text.Trim();
            if (sInputValue.Length > 14)
            {
                sInputValue = sInputValue.Substring(0, 14);
            };
        }
        else
        {
            sInputValue = lblText.Text.ToUpper().Trim();
        }

        if (sInputValue != NullToString(strValue).Trim())
        {
            intCount++;
            lblText.ForeColor = Color.Red;
            blnSame = false;
        }
        return blnSame;
    }


    /// 創建日期：2009/11/11
    /// 修改日期：2009/11/11
    /// <summary>
    /// 得到欄位值與一次鍵檔的比較結果
    /// </summary>
    /// <param name="txtBox">頁面欄位的TextBox</param>
    /// <param name="strValue">一次鍵檔的欄位值</param>
    /// <param name="intCount">不同的欄位數量</param>
    /// <param name="blnSame">是否相同</param>
    /// <returns>true相同，false不相同</returns>
    private bool GetCompareResultFee(CustTextBox txtBox, string strValue, ref int intCount, ref bool blnSame)
    {
        if (txtBox.Text.ToUpper().Trim() != NullToString(strValue).Trim())
        {
            intCount++;
            if (intCount == 1)
            {
                if (txtBox.Enabled == true)
                {
                    base.sbRegScript.Append(BaseHelper.SetFocus(txtBox.ID));
                }
            }
            //txtBox.ForeColor = Color.Red;
            txtBox.BackColor = Color.Red;
            blnSame = false;
        }
        else
        {
            txtBox.BackColor = Color.White;
        }
        return blnSame;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/11
    /// 修改日期：2009/11/11
    /// <summary>
    /// 將欄位值為NULL的轉換為空的字符串
    /// </summary>
    /// <param name="strValue">欄位值</param>
    /// <returns>為空的字符串</returns>
    private string NullToString(string strValue)
    {
        if (strValue == null)
        {
            return strValue = "";
        }
        return strValue;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/11
    /// 修改日期：2009/11/11
    /// <summary>
    /// 新增異動主機資料
    /// </summary>
    private bool UploadHtg()
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        try
        {
            if (this.txtShopId.Text.Trim().Length < 3 || this.txtShopId.Text.Trim().Substring(2, 1) != "5")
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01040202_005");
                Hashtable htInputOne = GetUploadInfo(true);//得到PCAM P4A Submit異動主機資料

                if (htInputOne == null)
                {
                    return false;
                }

                Hashtable htResultOne = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCHQ, htInputOne, false, "2", eAgentInfo);

                if (!htResultOne.Contains("HtgMsg"))
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_01040202_006");
                    base.strHostMsg += htResultOne["HtgSuccess"].ToString();//*主機返回成功訊息           
                    return true;
                }
                else
                {
                    //*異動主機資料失敗
                    if (htResultOne["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                    {
                        base.strHostMsg += htResultOne["HtgMsg"].ToString();
                        base.strClientMsg += MessageHelper.GetMessage("01_01040202_018");
                    }
                    else
                    {
                        base.strClientMsg += htResultOne["HtgMsg"].ToString();
                    }
                    return false;
                }
            }
            else
            {
                Hashtable htInputTwo = GetUploadInfo(true);//得到PCAM P4A Submit異動主機資料
                Hashtable htResultTwo = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCHQ, htInputTwo, false, "2", eAgentInfo);

                //htResultTwo["MESSAGE_TYPE"]=="0003" : 特店資料已存在
                //當 P4A特店資料已存在時 , 亦要發 P4交易 (以防同一特店 , 前一次交易 P4A 成功,P4 失敗時,再也無法由 GUI發動P4 request問題) 
                if ((!htResultTwo.Contains("HtgMsg")) || htResultTwo["MESSAGE_TYPE"].ToString() == "0003")
                {
                    if (htResultTwo["MESSAGE_TYPE"].ToString() == "0003")
                    {
                        base.strHostMsg += htResultTwo["HtgMsg"].ToString() + "。";//*主機返回成功訊息
                    }
                    else
                    {
                        base.strHostMsg += htResultTwo["HtgSuccess"].ToString() + "。";//*主機返回成功訊息
                    }
                    Hashtable htInputThree = GetUploadInfo(false);//得到PCAM P4 Submit異動主機資料
                    Hashtable htResultThree = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCHQ, htInputThree, false, "2", eAgentInfo);

                    if (!htResultThree.Contains("HtgMsg"))
                    {

                        base.strHostMsg += htResultThree["HtgSuccess"].ToString();//*主機返回成功訊息
                        base.strClientMsg += MessageHelper.GetMessage("01_01040202_007");
                        return true;
                    }
                    else
                    {
                        //*異動主機資料失敗
                        if (htResultThree["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                        {
                            base.strHostMsg += htResultThree["HtgMsg"].ToString();
                            base.strClientMsg += MessageHelper.GetMessage("01_01040202_017");
                        }
                        else
                        {
                            base.strClientMsg += htResultThree["HtgMsg"].ToString();
                        }
                        return false;
                    }
                }
                else
                {
                    //*異動主機資料失敗
                    if (htResultTwo["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                    {
                        base.strHostMsg += htResultTwo["HtgMsg"].ToString();
                        base.strClientMsg += MessageHelper.GetMessage("01_01040202_018");
                    }
                    else
                    {
                        base.strClientMsg += htResultTwo["HtgMsg"].ToString();
                    }
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.HTG);
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return false;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/11
    /// 修改日期：2009/11/11
    /// <summary>
    /// 得到新增異動主機信息
    /// </summary>
    /// <param name="blnType">上傳Status,費率選擇類型</param>
    /// <returns>Hashtable</returns>
    private Hashtable GetUploadInfo(bool blnType)
    {
        try
        {

            //Add by CTCB-Carolyn 2010.05.28
            //PROJ_MTH_VOLUME 鍵檔單位(萬),EX:鍵檔50,帶入主機000500000
            string strProjMthVolumn = this.txtMonth.Text.Trim();
            if (!string.IsNullOrEmpty(strProjMthVolumn))
            {
                strProjMthVolumn = (double.Parse(strProjMthVolumn) * 10000).ToString();
            }
            //

            Hashtable htInput = new Hashtable();
            if (blnType)
            {
                htInput.Add("ORGN", "822");//*<ORGANIZATION>
            }
            else
            {
                htInput.Add("ORGN", "900");//*<ORGANIZATION>
            }
            htInput.Add("ACCT", this.txtShopId.Text.Trim().ToUpper());//*<商店代號>
            htInput.Add("FUNCTION_CODE", "A");
            htInput.Add("ID_NAME", this.txtEngName.Text.Trim().ToUpper());//*<英文名稱>

            htInput.Add("ID_CITY", this.txtDbaCity.Text.Trim().ToUpper());//*<City>
            htInput.Add("CONTACT", this.txtOwner.Text.Trim().ToUpper());//*<負責人英文名>

            htInput.Add("MERCH_MEMO", this.txtSalesName.Text.Trim().ToUpper());//*<推廣姓名>
            htInput.Add("PHONE_NMBR1", this.txtPhone1.Text.PadRight(3, ' '));//*連絡電話1
            htInput.Add("PHONE_NMBR2", this.txtPhone2.Text.Trim());//*連絡電話2
            htInput.Add("PHONE_NMBR3", this.txtPhone3.Text.Trim());//*連絡電話3

            htInput.Add("OFFICER_ID", this.txtSource.Text.Trim().ToUpper());//*<SOURCE>
            htInput.Add("DB_ACCT_NMBR", this.txtCardNo1.Text.Trim().ToUpper() + this.txtCardNo2.Text.Trim().ToUpper());//*<統一編號>(1)+ <統一編號>(2)
            htInput.Add("MERCH_TYPE", this.txtType.Text.Trim().ToUpper());//*<Type>
            htInput.Add("CHAIN_STORE", GetValue(this.txtStore.Text.Trim().ToUpper(), "00000000", 8, '0'));//*<STORE>

            htInput.Add("MERCHANT_NAME", GetMerName());//*<登記名稱>

            htInput.Add("HOLD_STMT_FLAG", GetValue(this.txtHOLDSTMT.Text.Trim().ToUpper(), "0")); //*<Hold STMT>
            htInput.Add("ADDRESS1", this.txtAddress1.Text.Trim().ToUpper());//*<帳單地址>(1)
            htInput.Add("ADDRESS2", this.txtAddress2.Text.Trim().ToUpper());//*<帳單地址>(2)
            htInput.Add("ADDRESS3", "");//*<帳單地址>(3)
            htInput.Add("ZIP_CODE", this.txtZip.Text.Trim().ToUpper());//*<區域編號>

            htInput.Add("MCC", GetValue(this.txtMcc.Text.Trim().ToUpper(), "0000", 4, '0'));//*<MCC>
            htInput.Add("CHAIN_MER_NBR", GetValue(this.txtMerch.Text.Trim().ToUpper(), "000000000"));//*<MERCH>
            htInput.Add("CHAIN_MER_LEVEL", GetValue(this.txtLevel.Text.Trim().ToUpper(), "0"));//*<LEVEL>
            htInput.Add("NBR_IMPRINTER1", GetValue(this.txtImp1.Text.Trim().ToUpper(), "000"));//*<IMP1>
            htInput.Add("NBR_IMPRINTER2", GetValue(this.txtPos1.Text.Trim().ToUpper(), "000"));//*<POS1>
            htInput.Add("NBR_IMPRINTER3", GetValue(this.txtEdc1.Text.Trim().ToUpper(), "000"));//*<EDC1>        
            htInput.Add("NBR_POS_DEV1", GetValue(this.txtImp2.Text.Trim().ToUpper(), "000"));//*<IMP2>
            htInput.Add("NBR_POS_DEV2", GetValue(this.txtPos2.Text.Trim().ToUpper(), "000"));//*<POS2>
            htInput.Add("NBR_POS_DEV3", GetValue(this.txtEdc2.Text.Trim().ToUpper(), "000"));//*<EDC2>
            htInput.Add("PROJ_AVG_TKT", GetValue(this.txtAve.Text.Trim().ToUpper(), "000"));//*<AVE>
            //Mark By CTCB-Carolyn 2010.05.28
            //htInput.Add("PROJ_MTH_VOLUME", GetValue(this.txtMonth.Text.Trim().ToUpper(), "000000000"));//*<Month>
            //PROJ_MTH_VOLUME 鍵檔單位(萬),EX:鍵檔50,帶入主機000500000
            htInput.Add("PROJ_MTH_VOLUME", strProjMthVolumn);//*<Month>
            //
            htInput.Add("AGENT_BANK", GetValue(this.txtAgentBank.Text.Trim().ToUpper(), "00000", 5, '0'));//*<推廣代碼>
            htInput.Add("BRANCH", this.txtBranch.Text.Trim().ToUpper());//*<BRANCH>
            htInput.Add("ROUTE_TRANSIT", this.txtFileNo.Text.Trim().ToUpper());//*<歸檔編號>

            if ((!(this.txtMerch.Text.Trim() == "" || this.txtMerch.Text.Trim() == "0")) && (!(this.txtLevel.Text.Trim() == "" || this.txtLevel.Text.Trim() == "0")))
            {
                htInput.Add("CHAIN_STMT_IND", "1");//*<STATEMENT>
                htInput.Add("CHAIN_REPRT_IND", "1");//*<REPORTING>
                htInput.Add("CHAIN_SETT_IND", "0");//*<SETTLEMENT>
                htInput.Add("CHAIN_DISC_IND", "1");//*< DISCOUNT>
                htInput.Add("CHAIN_FEES_IND", "1");//*< FEES>
                htInput.Add("CHAIN_DD_IND", "0");//*<DISC/DEE DD>
            }
            else
            {
                htInput.Add("CHAIN_STMT_IND", "0");//*<STATEMENT>
                htInput.Add("CHAIN_REPRT_IND", "0");//*<REPORTING>
                htInput.Add("CHAIN_SETT_IND", "0");//*<SETTLEMENT>
                htInput.Add("CHAIN_DISC_IND", "0");//*< DISCOUNT>
                htInput.Add("CHAIN_FEES_IND", "0");//*< FEES>
                htInput.Add("CHAIN_DD_IND", "0");//*<DISC/DEE DD>
            }
            htInput.Add("USER_DATA1", this.txtDdaNo1.Text.Trim().ToUpper());//*<帳號1>
            htInput.Add("USER_DATA2", this.txtDdaNo2.Text.Trim().ToUpper());//*<帳號2>
            htInput.Add("VISA_INTCHG_FLAG", this.txtInt.Text.Trim().ToUpper());//*<INT>
            htInput.Add("VISA_MAIL_PHONE_IND", this.txtMail.Text.Trim().ToUpper());//*<MAIL>
            htInput.Add("POS_CAP", this.txtPosCa.Text.Trim().ToUpper());//*<POS CA>
            htInput.Add("POS_MODE", this.txtPosMo.Text.Trim().ToUpper());//*<POS MO>
            htInput.Add("CH_ID", this.txtCH.Text.Trim().ToUpper());//*<C/H>
            htInput.Add("MC_INTCHG_FLAG", this.txtMC.Text.Trim().ToUpper());//*<M/C>

            if (blnType)
            {
                htInput.Add("CARD_STATUS1", GetValue(this.txtStatus1.Text.Trim().ToUpper(), "0"));//*<Status-01>
                htInput.Add("CARD_STATUS2", GetValue(this.txtStatus2.Text.Trim().ToUpper(), "0"));//*<Status-02>
                htInput.Add("CARD_STATUS3", GetValue(this.txtStatus3.Text.Trim().ToUpper(), "0"));//*<Status-03>
                htInput.Add("CARD_STATUS4", GetValue(this.txtStatus4.Text.Trim().ToUpper(), "0"));//*<Status-04>
                htInput.Add("CARD_STATUS5", GetValue(this.txtStatus5.Text.Trim().ToUpper(), "0"));//*<Status-05>
                htInput.Add("CARD_STATUS6", GetValue(this.txtStatus6.Text.Trim().ToUpper(), "0"));//*<Status-06>
                htInput.Add("CARD_STATUS7", GetValue(this.txtStatus7.Text.Trim().ToUpper(), "0"));//*<Status-07>
                htInput.Add("CARD_STATUS8", GetValue(this.txtStatus8.Text.Trim().ToUpper(), "0"));//*<Status-08>
                htInput.Add("CARD_STATUS9", GetValue(this.txtStatus9.Text.Trim().ToUpper(), "0"));//*<Status-09>
                htInput.Add("CARD_STATUS10", GetValue(this.txtStatus10.Text.Trim().ToUpper(), "0"));//*<Status-10>
                htInput.Add("CARD_STATUS11", GetValue(this.txtStatus11.Text.Trim().ToUpper(), "0"));//*<Status-11>
                htInput.Add("CARD_STATUS12", GetValue(this.txtStatus12.Text.Trim().ToUpper(), "0"));//*<Status-12>
                htInput.Add("CARD_STATUS13", GetValue(this.txtStatus13.Text.Trim().ToUpper(), "0"));//*<Status-13>
                htInput.Add("CARD_STATUS14", GetValue(this.txtStatus14.Text.Trim().ToUpper(), "0"));//*<Status-14>
                htInput.Add("CARD_STATUS15", GetValue(this.txtStatus15.Text.Trim().ToUpper(), "0"));//*<Status-15>

                //*<費率01>
                htInput.Add("CARD_DISC_RATE1", this.txtDisRate1.Text.Trim().PadLeft(5, '0'));

                //*<費率02>
                htInput.Add("CARD_DISC_RATE2", this.txtDisRate2.Text.Trim().PadLeft(5, '0'));

                //*<費率03>
                htInput.Add("CARD_DISC_RATE3", this.txtDisRate3.Text.Trim().PadLeft(5, '0'));

                //*<費率04>
                htInput.Add("CARD_DISC_RATE4", this.txtDisRate4.Text.Trim().PadLeft(5, '0'));

                //*<費率05>
                htInput.Add("CARD_DISC_RATE5", this.txtDisRate5.Text.Trim().PadLeft(5, '0'));

                //*<費率06>
                htInput.Add("CARD_DISC_RATE6", this.txtDisRate6.Text.Trim().PadLeft(5, '0'));

                //*<費率07>
                htInput.Add("CARD_DISC_RATE7", this.txtDisRate7.Text.Trim().PadLeft(5, '0'));

                //*<費率08>
                htInput.Add("CARD_DISC_RATE8", this.txtDisRate8.Text.Trim().PadLeft(5, '0'));

                //*<費率09>
                htInput.Add("CARD_DISC_RATE9", this.txtDisRate9.Text.Trim().PadLeft(5, '0'));

                //*<費率10>
                htInput.Add("CARD_DISC_RATE10", this.txtDisRate10.Text.Trim().PadLeft(5, '0'));

                //*<費率11>
                htInput.Add("CARD_DISC_RATE11", this.txtDisRate11.Text.Trim().PadLeft(5, '0'));

                //*<費率12>
                htInput.Add("CARD_DISC_RATE12", this.txtDisRate12.Text.Trim().PadLeft(5, '0'));

                //*<費率13>
                htInput.Add("CARD_DISC_RATE13", this.txtDisRate13.Text.Trim().PadLeft(5, '0'));

                //*<費率14>
                htInput.Add("CARD_DISC_RATE14", this.txtDisRate14.Text.Trim().PadLeft(5, '0'));

                //*<費率15>
                htInput.Add("CARD_DISC_RATE15", this.txtDisRate15.Text.Trim().PadLeft(5, '0'));
            }
            else
            {
                htInput.Add("CARD_STATUS1", "1");//*<Status-01>
                htInput.Add("CARD_STATUS2", "1");//*<Status-02>
                htInput.Add("CARD_STATUS3", "1");//*<Status-03>
                htInput.Add("CARD_STATUS4", "1");//*<Status-04>
                htInput.Add("CARD_STATUS5", "1");//*<Status-05>
                htInput.Add("CARD_STATUS6", "1");//*<Status-06>
                htInput.Add("CARD_STATUS7", "1");//*<Status-07>
                htInput.Add("CARD_STATUS8", "1");//*<Status-08>
                htInput.Add("CARD_STATUS9", "1");//*<Status-09>
                htInput.Add("CARD_STATUS10", "0");//*<Status-10>
                htInput.Add("CARD_STATUS11", "0");//*<Status-11>
                htInput.Add("CARD_STATUS12", "0");//*<Status-12>
                htInput.Add("CARD_STATUS13", "0");//*<Status-13>
                htInput.Add("CARD_STATUS14", "0");//*<Status-14>
                htInput.Add("CARD_STATUS15", "0");//*<Status-15>

                //*<費率01>
                htInput.Add("CARD_DISC_RATE1", "00000");

                //*<費率02>
                htInput.Add("CARD_DISC_RATE2", "00000");

                //*<費率03>
                htInput.Add("CARD_DISC_RATE3", "00000");

                //*<費率04>
                htInput.Add("CARD_DISC_RATE4", "00000");

                //*<費率05>
                htInput.Add("CARD_DISC_RATE5", "00000");

                //*<費率06>
                htInput.Add("CARD_DISC_RATE6", "00000");

                //*<費率07>
                htInput.Add("CARD_DISC_RATE7", "00000");

                //*<費率08>
                htInput.Add("CARD_DISC_RATE8", "00000");

                //*<費率09>
                htInput.Add("CARD_DISC_RATE9", "00000");

                //*<費率10>
                htInput.Add("CARD_DISC_RATE10", "00000");

                //*<費率11>
                htInput.Add("CARD_DISC_RATE11", "00000");

                //*<費率12>
                htInput.Add("CARD_DISC_RATE12", "00000");

                //*<費率13>
                htInput.Add("CARD_DISC_RATE13", "00000");

                //*<費率14>
                htInput.Add("CARD_DISC_RATE14", "00000");

                //*<費率15>
                htInput.Add("CARD_DISC_RATE15", "00000");

            }            

            return htInput;
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return null;
        }
    }



    /// 作者 趙呂梁

    /// 創建日期：2009/11/10
    /// 修改日期：2009/11/10
    /// <summary>
    /// 若字符串為空，返回指定字符串
    /// </summary>
    /// <param name="strValue">要判斷的字符串</param>
    /// <param name="strReturnValue">指定字符串</param>
    /// <returns>字符串</returns>
    private string GetValue(string strValue, string strReturnValue)
    {
        if (strValue.Trim() == "")
        {
            return strReturnValue;
        }
        return strValue;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/10
    /// 修改日期：2009/11/10
    /// <summary>
    /// 得到網頁中欄位的值(若輸入為空，返回指定的字符串；若不為空，不足指定碼數時左補要填充的字符,填滿指定碼數)
    /// </summary>
    /// <param name="strValue">網頁中欄位的值</param>
    /// <param name="strReturnValue">為空時返回的字符串</param>
    /// <param name="intPadLength">填充碼數</param>
    /// <param name="chrPad">要填充的字符</param>
    /// <returns>新字符串</returns>
    private string GetValue(string strValue, string strReturnValue, int intPadLength, char chrPad)
    {
        if (strValue.Trim() != "")
        {
            //*不足4碼時左補"0",填滿4碼

            if (strValue.Trim().Length < intPadLength)
            {
                return strValue.PadLeft(intPadLength, chrPad);
            }
            else
            {
                return strValue;
            }
        }
        else
        {
            return strReturnValue;
        }
    }



    /// 作者 趙呂梁

    /// 創建日期：2009/11/05
    /// 修改日期：2009/11/05
    /// <summary>
    /// 向資料庫中新增資料

    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool InsertNewData()
    {
        #region
        EntitySHOP_PCAM eShopPcam = new EntitySHOP_PCAM();
        eShopPcam.organization = "";//*組織
        eShopPcam.merchant_name = GetMerName();//*帳列名稱
        eShopPcam.corp = this.txtCardNo1.Text.Trim().ToUpper() + this.txtCardNo2.Text.Trim().ToUpper();//*【統一編號】(1)+【統一編號】(2)
        eShopPcam.merchdba_name = this.txtEngName.Text.Trim().ToUpper();//*英文名稱
        eShopPcam.merchant_nbr = this.txtShopId.Text.Trim().ToUpper();//*商店代號
        eShopPcam.dba_city = this.txtDbaCity.Text.Trim().ToUpper();//*城市
        eShopPcam.zip = this.txtZip.Text.Trim().ToUpper();//*區域編號

        eShopPcam.address1 = this.txtAddress1.Text.Trim().ToUpper();//*【帳單地址】(1)
        eShopPcam.address2 = this.txtAddress2.Text.Trim().ToUpper();//*【帳單地址】(2)
        eShopPcam.address3 = "";//*【帳單地址】(3)
        eShopPcam.address4 = "";
        eShopPcam.phone = this.txtPhone1.Text.Trim() + "-" + this.txtPhone2.Text.Trim() + "-" + this.txtPhone3.Text.Trim();//*連絡電話
        eShopPcam.owner = this.txtOwner.Text.Trim().ToUpper();//*負責人英文名
        eShopPcam.imp1 = this.txtImp1.Text.Trim().ToUpper();//*IMP1
        eShopPcam.pos1 = this.txtPos1.Text.Trim().ToUpper();//*POS1
        eShopPcam.edc1 = this.txtEdc1.Text.Trim().ToUpper();//*EDC1
        eShopPcam.imp2 = this.txtImp2.Text.Trim().ToUpper();//*IMP2
        eShopPcam.pos2 = this.txtPos2.Text.Trim().ToUpper();//*POS2
        eShopPcam.edc2 = this.txtEdc2.Text.Trim().ToUpper();//*EDC2
        eShopPcam.sales_name = this.txtSalesName.Text.Trim().ToUpper();//*推廣姓名
        eShopPcam.agent_bank = this.txtAgentBank.Text.Trim().ToUpper();//*推廣代碼
        eShopPcam.merchant_type = this.txtType.Text.Trim().ToUpper();//*Type
        eShopPcam.chain_store = this.txtStore.Text.Trim().ToUpper();//*STORE
        eShopPcam.chain_merch = this.txtMerch.Text.Trim().ToUpper();//*MERCH
        eShopPcam.chain_level = this.txtLevel.Text.Trim().ToUpper();//*LEVEL
        eShopPcam.Source = this.txtSource.Text.Trim().ToUpper();//*SOURE
        eShopPcam.mcc_VISA = this.txtMcc.Text.Trim().ToUpper();//*MCC
        //eShopPcam.mcc_MC = this.txtMcc.Text.Trim().ToUpper();
        eShopPcam.mcc_MC = "";
        //eShopPcam.mcc_PL = this.txtMcc.Text.Trim().ToUpper();
        eShopPcam.mcc_PL = "";
        eShopPcam.avg_tkt = this.txtAve.Text.Trim().ToUpper();//*AVE
        eShopPcam.monthly_volume = this.txtMonth.Text.Trim().ToUpper();//*Month
        eShopPcam.branch = this.txtBranch.Text.Trim().ToUpper();//*BRANCH
        eShopPcam.INT = this.txtInt.Text.Trim().ToUpper();//*INT
        eShopPcam.MAIL = this.txtMail.Text.Trim().ToUpper();//*MAIL
        eShopPcam.POS_CA = this.txtPosCa.Text.Trim().ToUpper();//*Pos CA
        eShopPcam.POS_MO = this.txtPosMo.Text.Trim().ToUpper();//*Pos Mo
        eShopPcam.CH = this.txtCH.Text.Trim().ToUpper();//*C/H
        eShopPcam.MC = this.txtMC.Text.Trim().ToUpper();//*M/C
        eShopPcam.FileNo = this.txtFileNo.Text.Trim().ToUpper();//*歸檔編號
        eShopPcam.hold_stmt = this.txtHOLDSTMT.Text.Trim().ToUpper();//*Hold STMT
        eShopPcam.DDA_NO = this.txtDdaNo1.Text.Trim().ToUpper() + " " + this.txtDdaNo2.Text.Trim().ToUpper();//*【帳號】(1)+’ ’+【帳號】(2)
        //*Status-01-15
        eShopPcam.status1 = this.txtStatus1.Text.Trim().ToUpper();
        eShopPcam.status2 = this.txtStatus2.Text.Trim().ToUpper();
        eShopPcam.status3 = this.txtStatus3.Text.Trim().ToUpper();
        eShopPcam.status4 = this.txtStatus4.Text.Trim().ToUpper();
        eShopPcam.status5 = this.txtStatus5.Text.Trim().ToUpper();
        eShopPcam.status6 = this.txtStatus6.Text.Trim().ToUpper();
        eShopPcam.status7 = this.txtStatus7.Text.Trim().ToUpper();
        eShopPcam.status8 = this.txtStatus8.Text.Trim().ToUpper();
        eShopPcam.status9 = this.txtStatus9.Text.Trim().ToUpper();
        eShopPcam.status10 = this.txtStatus10.Text.Trim().ToUpper();
        eShopPcam.status11 = this.txtStatus11.Text.Trim().ToUpper();
        eShopPcam.status12 = this.txtStatus12.Text.Trim().ToUpper();
        eShopPcam.status13 = this.txtStatus13.Text.Trim().ToUpper();
        eShopPcam.status14 = this.txtStatus14.Text.Trim().ToUpper();
        eShopPcam.status15 = this.txtStatus15.Text.Trim().ToUpper();

        //*費率01-15
        eShopPcam.dis_rate1 = this.txtDisRate1.Text.Trim();
        eShopPcam.dis_rate2 = this.txtDisRate2.Text.Trim();
        eShopPcam.dis_rate3 = this.txtDisRate3.Text.Trim();
        eShopPcam.dis_rate4 = this.txtDisRate4.Text.Trim();
        eShopPcam.dis_rate5 = this.txtDisRate5.Text.Trim();
        eShopPcam.dis_rate6 = this.txtDisRate6.Text.Trim();
        eShopPcam.dis_rate7 = this.txtDisRate7.Text.Trim();
        eShopPcam.dis_rate8 = this.txtDisRate8.Text.Trim();
        eShopPcam.dis_rate9 = this.txtDisRate9.Text.Trim();
        eShopPcam.dis_rate10 = this.txtDisRate10.Text.Trim();
        eShopPcam.dis_rate11 = this.txtDisRate11.Text.Trim();
        eShopPcam.dis_rate12 = this.txtDisRate12.Text.Trim();
        eShopPcam.dis_rate13 = this.txtDisRate13.Text.Trim();
        eShopPcam.dis_rate14 = this.txtDisRate14.Text.Trim();
        eShopPcam.dis_rate15 = this.txtDisRate15.Text.Trim();
        eShopPcam.keyin_day = DateTime.Now.ToString("yyyyMMdd");
        // eShopPcam.first_user = eAgentInfo.agent_id;
        eShopPcam.first_user = "";

        eShopPcam.second_user = eAgentInfo.agent_name;
        eShopPcam.send3270 = "N";

        //新增keyin_flag 和 keyin_userID 欄位
        eShopPcam.keyin_flag = "2";
        eShopPcam.keyin_userID = eAgentInfo.agent_id;
        eShopPcam.MER_NAME_TYPE = GetMerNameType();//*帳列類型

        // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis        
        eShopPcam.LAST_UPD_BRANCH = this.txtLAST_UPD_BRANCH.Text.Trim();// 資料最後異動分行
        eShopPcam.LAST_UPD_CHECKER = this.txtLAST_UPD_CHECKER.Text.Trim();// 資料最後異動-CHECKER
        eShopPcam.LAST_UPD_MAKER = this.txtLAST_UPD_MAKER.Text.Trim();// 資料最後異動-MAKER

        try
        {
            return BRSHOP_PCAM.AddEntity(eShopPcam);
        }
        catch
        {
            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("01_00000000_020")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_020");
            }
            return false;
        }
        #endregion
    }


    /// 作者 趙呂梁
    /// 創建日期：2009/11/05
    /// 修改日期：2009/11/05
    /// <summary>
    /// 更新資料庫資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateData()
    {
        #region
        EntitySHOP_PCAM eShopPcam = new EntitySHOP_PCAM();
        eShopPcam.organization = "";//*組織
        eShopPcam.merchant_name = GetMerName();//*帳列名稱
        eShopPcam.corp = this.txtCardNo1.Text.Trim().ToUpper() + this.txtCardNo2.Text.Trim().ToUpper();//*【統一編號】(1)+【統一編號】(2)
        eShopPcam.merchdba_name = this.txtEngName.Text.Trim().ToUpper();//*英文名稱
        eShopPcam.merchant_nbr = this.txtShopId.Text.Trim().ToUpper();//*商店代號
        eShopPcam.dba_city = this.txtDbaCity.Text.Trim().ToUpper();//*城市
        eShopPcam.zip = this.txtZip.Text.Trim().ToUpper();//*區域編號

        eShopPcam.address1 = this.txtAddress1.Text.Trim().ToUpper();//*【帳單地址】(1)
        eShopPcam.address2 = this.txtAddress2.Text.Trim().ToUpper();//*【帳單地址】(2)
        eShopPcam.address3 = "";//*【帳單地址】(3)
        eShopPcam.address4 = "";
        eShopPcam.phone = this.txtPhone1.Text.Trim() + "-" + this.txtPhone2.Text.Trim() + "-" + this.txtPhone3.Text.Trim();//*連絡電話
        eShopPcam.owner = this.txtOwner.Text.Trim().ToUpper();//*負責人英文名
        eShopPcam.imp1 = this.txtImp1.Text.Trim().ToUpper();//*IMP1
        eShopPcam.pos1 = this.txtPos1.Text.Trim().ToUpper();//*POS1
        eShopPcam.edc1 = this.txtEdc1.Text.Trim().ToUpper();//*EDC1
        eShopPcam.imp2 = this.txtImp2.Text.Trim().ToUpper();//*IMP2
        eShopPcam.pos2 = this.txtPos2.Text.Trim().ToUpper();//*POS2
        eShopPcam.edc2 = this.txtEdc2.Text.Trim().ToUpper();//*EDC2
        eShopPcam.sales_name = this.txtSalesName.Text.Trim().ToUpper();//*推廣姓名
        eShopPcam.agent_bank = this.txtAgentBank.Text.Trim().ToUpper();//*推廣代碼
        eShopPcam.merchant_type = this.txtType.Text.Trim().ToUpper();//*Type
        eShopPcam.chain_store = this.txtStore.Text.Trim().ToUpper();//*STORE
        eShopPcam.chain_merch = this.txtMerch.Text.Trim().ToUpper();//*MERCH
        eShopPcam.chain_level = this.txtLevel.Text.Trim().ToUpper();//*LEVEL
        eShopPcam.Source = this.txtSource.Text.Trim().ToUpper();//*SOURE
        eShopPcam.mcc_VISA = this.txtMcc.Text.Trim().ToUpper();//*MCC
        //eShopPcam.mcc_MC = this.txtMcc.Text.Trim().ToUpper();
        eShopPcam.mcc_MC = "";
        //eShopPcam.mcc_PL = this.txtMcc.Text.Trim().ToUpper();
        eShopPcam.mcc_PL = "";
        eShopPcam.avg_tkt = this.txtAve.Text.Trim().ToUpper();//*AVE
        eShopPcam.monthly_volume = this.txtMonth.Text.Trim().ToUpper();//*Month
        eShopPcam.branch = this.txtBranch.Text.Trim().ToUpper();//*BRANCH
        eShopPcam.INT = this.txtInt.Text.Trim().ToUpper();//*INT
        eShopPcam.MAIL = this.txtMail.Text.Trim().ToUpper();//*MAIL
        eShopPcam.POS_CA = this.txtPosCa.Text.Trim().ToUpper();//*Pos CA
        eShopPcam.POS_MO = this.txtPosMo.Text.Trim().ToUpper();//*Pos Mo
        eShopPcam.CH = this.txtCH.Text.Trim().ToUpper();//*C/H
        eShopPcam.MC = this.txtMC.Text.Trim().ToUpper();//*M/C
        eShopPcam.FileNo = this.txtFileNo.Text.Trim().ToUpper();//*歸檔編號
        eShopPcam.hold_stmt = this.txtHOLDSTMT.Text.Trim().ToUpper();//*Hold STMT
        eShopPcam.DDA_NO = this.txtDdaNo1.Text.Trim().ToUpper() + " " + this.txtDdaNo2.Text.Trim().ToUpper();//*【帳號】(1)+’ ’+【帳號】(2)
        //*Status-01-15
        eShopPcam.status1 = this.txtStatus1.Text.Trim().ToUpper();
        eShopPcam.status2 = this.txtStatus2.Text.Trim().ToUpper();
        eShopPcam.status3 = this.txtStatus3.Text.Trim().ToUpper();
        eShopPcam.status4 = this.txtStatus4.Text.Trim().ToUpper();
        eShopPcam.status5 = this.txtStatus5.Text.Trim().ToUpper();
        eShopPcam.status6 = this.txtStatus6.Text.Trim().ToUpper();
        eShopPcam.status7 = this.txtStatus7.Text.Trim().ToUpper();
        eShopPcam.status8 = this.txtStatus8.Text.Trim().ToUpper();
        eShopPcam.status9 = this.txtStatus9.Text.Trim().ToUpper();
        eShopPcam.status10 = this.txtStatus10.Text.Trim().ToUpper();
        eShopPcam.status11 = this.txtStatus11.Text.Trim().ToUpper();
        eShopPcam.status12 = this.txtStatus12.Text.Trim().ToUpper();
        eShopPcam.status13 = this.txtStatus13.Text.Trim().ToUpper();
        eShopPcam.status14 = this.txtStatus14.Text.Trim().ToUpper();
        eShopPcam.status15 = this.txtStatus15.Text.Trim().ToUpper();
        //*費率01-15
        eShopPcam.dis_rate1 = this.txtDisRate1.Text.Trim();
        eShopPcam.dis_rate2 = this.txtDisRate2.Text.Trim();
        eShopPcam.dis_rate3 = this.txtDisRate3.Text.Trim();
        eShopPcam.dis_rate4 = this.txtDisRate4.Text.Trim();
        eShopPcam.dis_rate5 = this.txtDisRate5.Text.Trim();
        eShopPcam.dis_rate6 = this.txtDisRate6.Text.Trim();
        eShopPcam.dis_rate7 = this.txtDisRate7.Text.Trim();
        eShopPcam.dis_rate8 = this.txtDisRate8.Text.Trim();
        eShopPcam.dis_rate9 = this.txtDisRate9.Text.Trim();
        eShopPcam.dis_rate10 = this.txtDisRate10.Text.Trim();
        eShopPcam.dis_rate11 = this.txtDisRate11.Text.Trim();
        eShopPcam.dis_rate12 = this.txtDisRate12.Text.Trim();
        eShopPcam.dis_rate13 = this.txtDisRate13.Text.Trim();
        eShopPcam.dis_rate14 = this.txtDisRate14.Text.Trim();
        eShopPcam.dis_rate15 = this.txtDisRate15.Text.Trim();
        eShopPcam.keyin_day = DateTime.Now.ToString("yyyyMMdd");
        // eShopPcam.first_user = eAgentInfo.agent_id;
        eShopPcam.first_user = "";

        eShopPcam.second_user = eAgentInfo.agent_name;
        eShopPcam.send3270 = "N";

        //新增keyin_flag 和 keyin_userID 欄位
        eShopPcam.keyin_flag = "2";
        eShopPcam.keyin_userID = eAgentInfo.agent_id;
        eShopPcam.MER_NAME_TYPE = GetMerNameType();//*帳列類型

        // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis        
        eShopPcam.LAST_UPD_BRANCH = this.txtLAST_UPD_BRANCH.Text.Trim();// 資料最後異動分行
        eShopPcam.LAST_UPD_CHECKER = this.txtLAST_UPD_CHECKER.Text.Trim();// 資料最後異動-CHECKER
        eShopPcam.LAST_UPD_MAKER = this.txtLAST_UPD_MAKER.Text.Trim();// 資料最後異動-MAKER

        try
        {
            return BRSHOP_PCAM.Update(eShopPcam, this.txtShopId.Text.Trim().ToUpper(), "2");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_021");
            return false;
        }

        #endregion
    }

    /// <summary>
    /// 得到主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool GetMainframeData()
    {

        Hashtable htInput = new Hashtable();
        htInput.Add("FUNCTION_CODE", "I");//*查詢
        htInput.Add("CORP_NO", this.txtCardNo1.Text.Trim());//*統編序號CORP1
        htInput.Add("CORP_SEQ", this.txtCardNo2.Text.Trim());//*統編序號CORP2
        htInput.Add("FORCE_FLAG", "");

        Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGQ, htInput, false, "11", eAgentInfo);

        if (!htReturn.Contains("HtgMsg") )
        {
            lblRegNameText.Text = htReturn["REG_NAME"].ToString();
            lblBusinessNameText.Text = htReturn["BUSINESS_NAME"].ToString();
            return true;
        }
        else
        {
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
            etMstType = eMstType.Select;
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

            //*異動主機資料失敗
            if (htReturn["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            {
                base.strHostMsg += htReturn["HtgMsg"].ToString();
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_026");
            }
            else
            {
                base.strClientMsg += htReturn["HtgMsg"].ToString();
            }
            return false;
        }
    }

    /// <summary>
    /// 得到賬列名稱類型
    /// </summary>
    /// <returns>類型</returns>
    private string GetMerNameType()
    {
        if (this.radRegName.Checked)
        {
            return "1";
        }

        if (this.radBusinessName.Checked)
        {
            return "2";
        }

        if (this.radMerchantName.Checked)
        {
            return "3";
        }
        return "";
    }

    /// <summary>
    /// 得到賬列名稱
    /// </summary>
    /// <returns>賬列名稱</returns>
    private string GetMerName()
    {
        string sRtnMerName = "";

        if (this.radRegName.Checked)
        {
            sRtnMerName = lblRegNameText.Text.Trim();
            if (sRtnMerName.Length > 14)
            {
                sRtnMerName = sRtnMerName.Substring(0, 14);
            }
        }

        if (this.radBusinessName.Checked)
        {
            sRtnMerName = lblBusinessNameText.Text.Trim();
            if (sRtnMerName.Length > 14)
            {
                sRtnMerName = sRtnMerName.Substring(0, 14);
            }
        }

        if (this.radMerchantName.Checked)
        {
            sRtnMerName = txtMerchantName.Text.Trim();
            if (sRtnMerName.Length > 14)
            {
                sRtnMerName = sRtnMerName.Substring(0, 14);
            }
        }

        return sRtnMerName;
    }

    /// <summary>
    /// 根據賬列名稱類型選擇對應RadioButton
    /// </summary>
    /// <param name="strValue">類型</param>
    private void SelectRadioButton(string strValue)
    {
        if (strValue == "1")
        {
            this.radRegName.Checked = true;
        }

        if (strValue == "2")
        {
            this.radBusinessName.Checked = true;
        }

        if (strValue == "3")
        {
            this.radMerchantName.Checked = true;
        }
    }

    /// <summary>
    /// 清空提交部分畫面
    /// </summary>
    private void ClearPage(bool blnEnabled)
    {
        CommonFunction.SetControlsEnabled(pnlText, blnEnabled);
        this.radRegName.Checked = false;
        this.radBusinessName.Checked = false;
        this.radMerchantName.Checked = false;
        this.radRegName.Enabled = blnEnabled;
        this.radBusinessName.Enabled = blnEnabled;
        this.radMerchantName.Enabled = blnEnabled;
        lblRegNameText.Text = "";
        lblBusinessNameText.Text = "";
    }

    /// <summary>
    /// 清空網頁所有內容
    /// </summary>
    private void ClearAll()
    {
        ClearPage(false);
        this.txtShopId.Text = "";
        this.txtShopIdHide.Text = "";
        this.txtCardNo1.Text = "";
        this.txtCardNo1Hide.Text = "";
        this.txtCardNo2.Text = "";
        this.txtCardNo2Hide.Text = "";
    }

    //*檢核商店代號
    private void CheckShopId()
    {
        string sShopID = this.txtShopId.Text.Trim();

        if (CommonFunction.GetSubString(sShopID, 0, 5) == "88320")
        {
            //*Status-01-15
            this.txtStatus1.Text = "0";
            this.txtStatus2.Text = "0";
            this.txtStatus3.Text = "0";
            this.txtStatus4.Text = "0";
            this.txtStatus5.Text = "0";
            this.txtStatus6.Text = "0";
            this.txtStatus7.Text = "0";
            this.txtStatus8.Text = "0";
            this.txtStatus9.Text = "0";
            this.txtStatus10.Text = "0";
            this.txtStatus11.Text = "0";
            this.txtStatus12.Text = "0";
            this.txtStatus13.Text = "0";
            this.txtStatus14.Text = "0";
            this.txtStatus15.Text = "0";

            this.txtDisRate1.Text = "0";
            this.txtDisRate2.Text = "0";
            this.txtDisRate3.Text = "0";
            this.txtDisRate4.Text = "0";
            this.txtDisRate5.Text = "0";
            this.txtDisRate6.Text = "0";
            this.txtDisRate7.Text = "0";
            this.txtDisRate8.Text = "0";
            this.txtDisRate9.Text = "0";
            this.txtDisRate10.Text = "0";
            this.txtDisRate11.Text = "0";
            this.txtDisRate12.Text = "0";
            this.txtDisRate13.Text = "0";
            this.txtDisRate14.Text = "0";
            this.txtDisRate15.Text = "0";

            this.txtStatus1.Enabled = false;
            this.txtStatus2.Enabled = false;
            this.txtStatus3.Enabled = false;
            this.txtStatus4.Enabled = false;
            this.txtStatus5.Enabled = false;
            this.txtStatus6.Enabled = false;
            this.txtStatus7.Enabled = false;
            this.txtStatus8.Enabled = false;
            this.txtStatus9.Enabled = false;
            this.txtStatus10.Enabled = false;
            this.txtStatus11.Enabled = false;
            this.txtStatus12.Enabled = false;
            this.txtStatus13.Enabled = false;
            this.txtStatus14.Enabled = false;
            this.txtStatus15.Enabled = false;

            //*費率01-15
            this.txtDisRate1.Enabled = false;
            this.txtDisRate2.Enabled = false;
            this.txtDisRate3.Enabled = false;
            this.txtDisRate4.Enabled = false;
            this.txtDisRate5.Enabled = false;
            this.txtDisRate6.Enabled = false;
            this.txtDisRate7.Enabled = false;
            this.txtDisRate8.Enabled = false;
            this.txtDisRate9.Enabled = false;
            this.txtDisRate10.Enabled = false;
            this.txtDisRate11.Enabled = false;
            this.txtDisRate12.Enabled = false;
            this.txtDisRate13.Enabled = false;
            this.txtDisRate14.Enabled = false;
            this.txtDisRate15.Enabled = false;
        }

        string strNum = CommonFunction.GetSubString(sShopID, 2, 3);
        if (strNum == "251" || strNum == "252" || strNum == "253" || strNum == "254")
        {
            //*Status-01-15
            this.txtStatus1.Text = "0";
            this.txtStatus2.Text = "0";
            this.txtStatus3.Text = "0";
            this.txtStatus4.Text = "0";
            this.txtStatus5.Text = "0";
            this.txtStatus6.Text = "0";
            this.txtStatus7.Text = "0";
            this.txtStatus8.Text = "0";
            this.txtStatus9.Text = "0";
            //20200804-RQ-2020-021027-001 因主機會檢查 卡類05/10及卡類06/11的STATUS要一致, 故請CSIP配合修改, 增加鎖住卡類10及11也不能修改
            //this.txtStatus10.Text = "1";
            //this.txtStatus11.Text = "1";
            this.txtStatus10.Text = "0";
            this.txtStatus11.Text = "0";
            this.txtStatus12.Text = "0";
            this.txtStatus13.Text = "1";
            this.txtStatus14.Text = "0";
            this.txtStatus15.Text = "0";

            this.txtDisRate1.Text = "0";
            this.txtDisRate2.Text = "0";
            this.txtDisRate3.Text = "0";
            this.txtDisRate4.Text = "0";
            this.txtDisRate5.Text = "0";
            this.txtDisRate6.Text = "0";
            this.txtDisRate7.Text = "0";
            this.txtDisRate8.Text = "0";
            this.txtDisRate9.Text = "0";
            this.txtDisRate10.Text = "0";
            this.txtDisRate11.Text = "0";
            this.txtDisRate12.Text = "0";
            this.txtDisRate13.Text = "0";
            this.txtDisRate14.Text = "0";
            this.txtDisRate15.Text = "0";

            this.txtStatus1.Enabled = false;
            this.txtStatus2.Enabled = false;
            this.txtStatus3.Enabled = false;
            this.txtStatus4.Enabled = false;
            this.txtStatus5.Enabled = false;
            this.txtStatus6.Enabled = false;
            this.txtStatus7.Enabled = false;
            this.txtStatus8.Enabled = false;
            this.txtStatus9.Enabled = false;
            //20200804-RQ-2020-021027-001 因主機會檢查 卡類05/10及卡類06/11的STATUS要一致, 故請CSIP配合修改, 增加鎖住卡類10及11也不能修改
            //this.txtStatus10.Enabled = true;
            //this.txtStatus11.Enabled = true;
            this.txtStatus10.Enabled = false;
            this.txtStatus11.Enabled = false;
            this.txtStatus12.Enabled = false;
            this.txtStatus13.Enabled = true;
            this.txtStatus14.Enabled = false;
            this.txtStatus15.Enabled = false;

            //*費率01-15
            this.txtDisRate1.Enabled = false;
            this.txtDisRate2.Enabled = false;
            this.txtDisRate3.Enabled = false;
            this.txtDisRate4.Enabled = false;
            this.txtDisRate5.Enabled = false;
            this.txtDisRate6.Enabled = false;
            this.txtDisRate7.Enabled = false;
            this.txtDisRate8.Enabled = false;
            this.txtDisRate9.Enabled = false;
            //20200804-RQ-2020-021027-001 因主機會檢查 卡類05/10及卡類06/11的STATUS要一致, 故請CSIP配合修改, 增加鎖住卡類10及11也不能修改
            //this.txtDisRate10.Enabled = true;
            //this.txtDisRate11.Enabled = true;
            this.txtDisRate10.Enabled = false;
            this.txtDisRate11.Enabled = false;
            this.txtDisRate12.Enabled = false;
            this.txtDisRate13.Enabled = true;
            this.txtDisRate14.Enabled = false;
            this.txtDisRate15.Enabled = false;
        }

        if (CommonFunction.GetSubString(sShopID, 2, 1) == "5")
        {
            this.txtStatus1.Text = "1";
            this.txtStatus2.Text = "0";
            this.txtStatus3.Text = "1";
            this.txtStatus4.Text = "0";
            this.txtStatus5.Text = "1";
            this.txtStatus6.Text = "0";
            this.txtStatus7.Text = "1";
            this.txtStatus8.Text = "0";
            this.txtStatus9.Text = "0";
            //20160627 (U) by Tank, 商店代號第3碼為5，status10預設=1
            this.txtStatus10.Text = "1";
            this.txtStatus11.Text = "0";
            this.txtStatus12.Text = "0";
            this.txtStatus13.Text = "0";
            this.txtStatus14.Text = "0";
            this.txtStatus15.Text = "0";

            this.txtDisRate1.Text = "0";
            this.txtDisRate2.Text = "0";
            this.txtDisRate3.Text = "0";
            this.txtDisRate4.Text = "0";
            this.txtDisRate5.Text = "0";
            this.txtDisRate6.Text = "0";
            this.txtDisRate7.Text = "0";
            this.txtDisRate8.Text = "0";
            this.txtDisRate9.Text = "0";
            this.txtDisRate10.Text = "0";
            this.txtDisRate11.Text = "0";
            this.txtDisRate12.Text = "0";
            this.txtDisRate13.Text = "0";
            this.txtDisRate14.Text = "0";
            this.txtDisRate15.Text = "0";

            this.txtStatus1.Enabled = false;
            this.txtStatus2.Enabled = false;
            this.txtStatus3.Enabled = false;
            this.txtStatus4.Enabled = false;
            this.txtStatus5.Enabled = false;
            this.txtStatus6.Enabled = false;
            this.txtStatus7.Enabled = false;
            this.txtStatus8.Enabled = false;
            this.txtStatus9.Enabled = false;
            this.txtStatus10.Enabled = false;
            this.txtStatus11.Enabled = false;
            this.txtStatus12.Enabled = false;
            this.txtStatus13.Enabled = false;
            this.txtStatus14.Enabled = false;
            this.txtStatus15.Enabled = false;

            this.txtDisRate1.Enabled = false;
            this.txtDisRate2.Enabled = false;
            this.txtDisRate3.Enabled = false;
            this.txtDisRate4.Enabled = false;
            this.txtDisRate5.Enabled = false;
            this.txtDisRate6.Enabled = false;
            this.txtDisRate7.Enabled = false;
            this.txtDisRate8.Enabled = false;
            this.txtDisRate9.Enabled = false;
            this.txtDisRate10.Enabled = false;
            this.txtDisRate11.Enabled = false;
            this.txtDisRate12.Enabled = false;
            this.txtDisRate13.Enabled = false;
            this.txtDisRate14.Enabled = false;
            this.txtDisRate15.Enabled = false;
        }
    }

    /// <summary>
    /// 設置一Key值
    /// </summary>
    /// <param name="eShopPcam">Entity</param>
    private void SetDisabledValue(EntitySHOP_PCAM eShopPcam)
    {
        this.txtEngName.Text = eShopPcam.merchdba_name;//*英文名稱
        this.txtDbaCity.Text = eShopPcam.dba_city;//*英文CITY
        this.txtOwner.Text = eShopPcam.owner;//*負責人英文名
        this.txtSalesName.Text = eShopPcam.sales_name;//*推廣姓名
        this.txtAgentBank.Text = eShopPcam.agent_bank;//*推廣代碼
        this.txtFileNo.Text = eShopPcam.FileNo;//*歸檔編號

        /*Remark by CTCB-Carolyn 2010.05.28 二KEY帳號為開放鍵檔欄位(非帶入一KEY資料)
        //*帳號
        if (!string.IsNullOrEmpty(eShopPcam.DDA_NO))
        {
            if (eShopPcam.DDA_NO.Length >= 3)
            {
                this.txtDdaNo1.Text = eShopPcam.DDA_NO.Substring(0, 3);
                this.txtDdaNo2.Text = eShopPcam.DDA_NO.Substring(3, eShopPcam.DDA_NO.Length - 3).Trim();
            }
            else
            {
                this.txtDdaNo1.Text = eShopPcam.DDA_NO;
                this.txtDdaNo2.Text = "";
            }
        }
        else
        {
            this.txtDdaNo1.Text = "";
            this.txtDdaNo2.Text = "";
        }
        */

    }

    /// <summary>
    /// 禁用指定欄位
    /// </summary>
    private void DisabledColumn()
    {
        this.txtEngName.BackColor = Color.LightGray;
        this.txtDbaCity.BackColor = Color.LightGray;
        this.txtOwner.BackColor = Color.LightGray;
        this.txtSalesName.BackColor = Color.LightGray;
        this.txtAgentBank.BackColor = Color.LightGray;
        this.txtFileNo.BackColor = Color.LightGray;

        this.txtEngName.ReadOnly = true;//*英文名稱
        this.txtDbaCity.ReadOnly = true;//*英文CITY
        this.txtOwner.ReadOnly = true;//*負責人英文名
        this.txtSalesName.ReadOnly = true;//*推廣姓名
        this.txtAgentBank.ReadOnly = true;//*推廣代碼
        this.txtFileNo.ReadOnly = true;//*歸檔編號

        /*
        this.txtEngName.Enabled = false;//*英文名稱
        this.txtDbaCity.Enabled = false;//*英文CITY
        this.txtOwner.Enabled = false;//*負責人英文名
        this.txtSalesName.Enabled = false;//*推廣姓名
        this.txtAgentBank.Enabled = false;//*推廣代碼
        this.txtFileNo.Enabled = false;//*歸檔編號          
        this.txtDdaNo1.Enabled = false;
        this.txtDdaNo2.Enabled = false;
        */
    }
    #endregion

}