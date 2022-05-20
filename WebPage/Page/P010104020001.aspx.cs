// *****************************************************************
//   作    者：趙呂梁
//   功能說明：特店基本資料二次鍵檔(6001新增二KEY)
//   創建日期：2009/11/02
//   修改記錄： 
// 2010/12/21   zhen chen      RQ-2010-005537-000      
// 除：收編,負責人ID,實際經營ID,JCIC,帳單內容,週期
// 等欄位 ,剩下的欄位 二KEY 鍵檔畫面 , default 直接帶入 一KEY 資料
// <author>            <time>            <TaskID>                <desc>
// ******************************************************************
//20180119 (U) by Grezz, 新增控制項：1. Y_MPOS特店系統服務費免收註記(6086)F001, 2. Y_特店跨行匯費(6116) 
//20190805 (U) by Peggy, 新增控制項：因長姓名需求，增加是否長姓名，負責人中文長姓名、負責人羅馬拼音、是否聯絡人長姓名、聯絡人長姓名、聯絡人羅馬拼音 6個控制項
//20191023 (U) by Peggy, 新增SOC所需資訊
//20200215 (U) by Peggy, RQ-2019-030155-003：總公司類型拿掉獨立店/海外公司選項，新增分期平台選項

using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Web.UI.WebControls;
using Framework.Common.Message;
using CSIPCommonModel.EntityLayer;
using CSIPKeyInGUI.BusinessRules_new;
using Framework.Data.OM.Collections;
using Framework.Common.Logging;
using Framework.WebControls;
using CSIPCommonModel.EntityLayer_new;
using System.Collections.Generic;
using CSIPKeyInGUI.EntityLayer;

public partial class P010104020001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    //20191023 修改：SOC所需資訊  by Peggy
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ViewState["Send"] = false;
            CommonFunction.SetControlsEnabled(pnlText, false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo1"));
            this.cddl_MemberService.Items.Add(new ListItem("", ""));
            this.cddl_MemberService.Items.Add(new ListItem("Y", "Y"));
            this.cddl_MemberService.SelectedValue = "";
            LoadDropDownList();
        }

        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; // Session變數集合
        string msg = " 二 key : " + eAgentInfo.agent_id.Trim();
        // Logging.SaveLog(ELogLayer.UI, msg, ELogType.Debug);
        Logging.Log(msg, LogState.Debug,LogLayer.UI);
        CommonFunction.SetControlsForeColor(pnlText, Color.Black);

        sPageInfo = (structPageInfo)this.Session["PageInfo"];//20191023 修改：SOC所需資訊  by Peggy
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/02
    /// 修改日期：2009/11/02
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
        EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtCardNo1.Text.Trim() + txtCardNo2.Text.Trim();//查詢條件        
        log.Statement_Text = string.Format("CUSTOMER_ID:{0}|AC_NO:{1}|BRANCH_ID:{2}|ROLE_ID:{3}", log.Customer_Id, log.Account_Nbr, log.Branch_Nbr, log.Role_Id); //查詢條件內容: 用 | 區隔
        BRL_AP_LOG.Add(log);
        #endregion

        // 查詢是否上傳主機
        EntitySet<CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC> eShopBasicUploadHtg = null;
        try
        {
            eShopBasicUploadHtg = BRSHOP_BASIC.SelectEntitySet(this.txtCardNo1.Text.Trim(), this.txtCardNo2.Text.Trim(), "Y");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (eShopBasicUploadHtg.Count > 0)// 此筆資料已上傳主機
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040201_002") + "');");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo1"));
            return;
        }

        EntitySet<CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC> eShopBasicSet = null;
        try
        {
            eShopBasicSet = BRSHOP_BASIC.SelectEntitySet(this.txtCardNo1.Text.Trim(), this.txtCardNo2.Text.Trim(), "N");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (eShopBasicSet != null)
        {
            string strOneKeyUserID = "";
            CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC eShopBasicOneKey = null;
            bool blnOneKeyin = false;// 是否有一KEY資料
            bool blnTwoKeyin = false;// 是否有二KEY資料

            for (int i = 0; i < eShopBasicSet.Count; i++)
            {
                if (eShopBasicSet.GetEntity(i).keyin_flag.Trim() == "1")
                {
                    blnOneKeyin = true;
                    strOneKeyUserID = eShopBasicSet.GetEntity(i).keyin_userID.Trim();
                    eShopBasicOneKey = eShopBasicSet.GetEntity(i);
                    ViewState["EntityShopBasic"] = eShopBasicSet.GetEntity(i);
                    break;
                }
            }

            if (blnOneKeyin)
            {
                string agent_id = eAgentInfo.agent_id.Trim();
                string msg = "一 key : " + strOneKeyUserID + " 二 key : " + agent_id;// eAgentInfo.agent_id.Trim();
                //Logging.SaveLog(ELogLayer.Util, strSql, ELogType.Debug);
                //Logging.SaveLog(ELogLayer.UI, msg, ELogType.Debug);
                Logging.Log(msg, LogState.Debug, LogLayer.UI);

                // 測試機上版時，移除"一KEY與二KEY經辦不能為同一人"驗證*****正式機需加回
                //if (eAgentInfo.agent_id.Trim() == strOneKeyUserID)
                //{
                //    base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040201_012") + "');");
                //    return;
                //}

                ClearPage(true);

                SetOneKeyValues(eShopBasicOneKey);

                for (int i = 0; i < eShopBasicSet.Count; i++)
                {
                    if (eShopBasicSet.GetEntity(i).keyin_flag.Trim() == "2")
                    {
                        blnTwoKeyin = true;
                        SetTwoKeyValues(eShopBasicSet.GetEntity(i));// 為網頁中的欄位賦值
                        break;
                    }
                }

                if (blnTwoKeyin == false)
                {
                    this.lblCheckManText.Text = "0000";
                    int intTime = int.Parse(DateTime.Now.ToString("yyyyMMdd")) - 19110000;
                    this.txtReceiveNumber.Text = intTime.ToString().PadLeft(7, '0');
                }

                DisabledColumn();

                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            }
            else
            {
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040201_011") + "');");
                ClearPage(false);
                base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo1"));
            }
        }
        //20210906_Ares_jack 統編符合involve範圍時，將預設勾選自然人收單選項
        string involve = "78360020,78360021,78360023,78360024,78360025,78360026,78360027,78360028,78360029";
        if (involve.Contains(this.txtCardNo1.Text.Trim()))
        {
            //202109023_Ares_jack 當確認為自然人收單時 與公司往來的Radio Button不讓USER選
            radSingleMerchant6.Checked = true;
            radSingleMerchant6.Enabled = false;
            radSingleMerchant1.Enabled = false;
            radSingleMerchant1.Checked = false;
            radSingleMerchant2.Enabled = false;
            radSingleMerchant2.Checked = false;
            radSingleMerchant5.Enabled = false;
            radSingleMerchant5.Checked = false;
        }
        else
        {
            //20211206_Ares_jack_統編若非自然人請將  自然人收單選項 Disabled
            radSingleMerchant6.Enabled = false;
            radSingleMerchant6.Checked = false;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/02
    /// 修改日期：2009/11/02
    /// <summary>
    /// 新增事件
    /// </summary>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        #region 2015/1/22 by Eric
        if (this.cddl_MemberService.SelectedValue.ToUpper() == "Y" && this.txtJCIC.Text.Trim().ToUpper() != "A")
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040101_008") + "');");

            return;
        }
        #endregion

        //20200515 統一證號碼(新+舊)邏輯檢核
        if (!string.IsNullOrEmpty(txtResidentNo.Text))
        {
            if (txtResidentNo.Text.Length != 10)
            {
                base.sbRegScript.Append("alert('統一證號須為10碼');$('#txtResidentNo').focus();");
                return;
            }
            else
            {
                if (!CheckResidentID(txtResidentNo.Text.Trim()))//20201021-202012RC 統一證號碼(新+舊)邏輯檢核
                {
                    base.sbRegScript.Append("alert('統一證號輸入錯誤，請重新輸入');$('#txtResidentNo').focus();");
                    return;
                }
            }
        }

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

        // 20210527 EOS_AML(NOVA) 檢查郵遞區號 by Ares Dennis
        string address = this.lblBookAddr1Text.Text.Trim();
        if (!string.IsNullOrEmpty(address) && !checkREG_ZIP_CODE(address))
        {
            base.sbRegScript.Append("alert('地址查無郵遞區號，請輸入正確地址或請聯繫MFA更新');");
            return;
        }
		
        //20210902_Ares_Stanley-檢查Email總長度為50; 20210913_Ares_Stanley-改共用function
        string mailEnd = string.Empty;
        if (this.radGmail.Checked)
        {
            mailEnd = this.radGmail.Text;
        }
        if (this.radYahoo.Checked)
        {
            mailEnd = this.radYahoo.Text;
        }
        if (this.radHotmail.Checked)
        {
            mailEnd = this.radHotmail.Text;
        }
        if (this.radOther.Checked)
        {
            mailEnd = this.txtEmailOther.Text;
        }
        if (!CommonFunction.CheckMailLength(this.txtEmailFront.Text, mailEnd))
        {
            base.sbRegScript.Append("alert('Email總長度不可大於50碼');$('#txtEmailFront').focus();");
            return;
        }

        DisabledColumn();

        EntitySet<CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC> eShopBasicUploadHtg = null;
        try
        {
            eShopBasicUploadHtg = BRSHOP_BASIC.SelectEntitySet(this.txtCardNo1.Text.Trim(), this.txtCardNo2.Text.Trim(), "Y");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (eShopBasicUploadHtg.Count > 0)// 此筆資料已上傳主機
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040201_002") + "');");
            return;
        }

        EntitySet<CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC> eShopBasicSet = null;
        try
        {
            eShopBasicSet = BRSHOP_BASIC.SelectEntitySet(this.txtCardNo1.Text.Trim(), this.txtCardNo2.Text.Trim(), "N");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        bool blnTwoKey = false;// 是否有二KEY資料

        for (int i = 0; i < eShopBasicSet.Count; i++)
        {
            if (eShopBasicSet.GetEntity(i).keyin_flag.Trim() == "2")
            {
                blnTwoKey = true;
                break;
            }
        }

        if (blnTwoKey)// 原 DB 已存在2key資料
        {
            UpdateData("N");
        }
        else
        {
            AddNewData("N");
        }

        if (Compare())
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01040201_004");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo1"));

            //因為UploadHtgInfo之後會把控制項的值清空，所以要先把值記錄下來
            string taxID = this.txtUniNo.Text.Trim();           // 總公司統一編號
            string branch_TaxID = this.txtCardNo1.Text.Trim();  // 分公司統一編號
            string branch_No = this.txtCardNo2.Text.Trim();     // 分公司序號
            string recv_no = this.txtReceiveNumber.Text.Trim(); // 收件編號            

            bool isUpload = UploadHtgInfo();
            if (isUpload)
            {
                insertAML_AddHeadOfficeReport(taxID, branch_TaxID, branch_No, recv_no);
                // 20210527 EOS_AML(NOVA) 6001二KEY建檔成功，寫一筆異動紀錄 by Ares Dennis
                #region 異動記錄需報送AML
                EntityAML_CHECKLOG eAMLCheckLog = new EntityAML_CHECKLOG();
                Hashtable htInput = GetUploadHtgInfo("");//得到上傳主機信息
                eAMLCheckLog.CORP_NO = htInput["HEADQUARTER_CORPNO"].ToString().Trim();
                eAMLCheckLog.TRANS_ID = "CSIPJCGQ";
                eAMLCheckLog.LAST_UPD_BRANCH = htInput["LAST_UPD_BRANCH"].ToString().Trim();
                eAMLCheckLog.LAST_UPD_CHECKER = htInput["LAST_UPD_CHECKER"].ToString().Trim();
                eAMLCheckLog.LAST_UPD_MAKER = htInput["LAST_UPD_MAKER"].ToString().Trim();
                eAMLCheckLog.MOD_USERID = eAgentInfo.agent_id;
                eAMLCheckLog.MOD_DATE = DateTime.Now.ToString("yyyyMMdd");
                eAMLCheckLog.MOD_TIME = DateTime.Now.ToString("HHmmss");

                InsertAMLCheckLog(eAMLCheckLog);
                #endregion
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01040201_003");
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/03
    /// 修改日期：2009/11/03
    /// <summary>
    /// 錯誤訊息包含“強制執行”執行事件
    /// </summary>
    protected void btnHiden_Click(object sender, EventArgs e)
    {
        Hashtable htInput = GetUploadHtgInfo("Y");
        htInput["MESSAGE_TYPE"] = "5555";
        //UploadHtgInfo(htInput);
        if(UploadHtgInfo(htInput))
        {
            //20211209_Ares_Jack_強制執行 異動紀錄

            //因為UploadHtgInfo之後會把控制項的值清空，所以要先把值記錄下來
            string taxID = this.txtUniNo.Text.Trim();           // 總公司統一編號
            string branch_TaxID = this.txtCardNo1.Text.Trim();  // 分公司統一編號
            string branch_No = this.txtCardNo2.Text.Trim();     // 分公司序號
            string recv_no = this.txtReceiveNumber.Text.Trim(); // 收件編號   

            insertAML_AddHeadOfficeReport(taxID, branch_TaxID, branch_No, recv_no);
            // 20210527 EOS_AML(NOVA) 6001二KEY建檔成功，寫一筆異動紀錄 by Ares Dennis
            #region 異動記錄需報送AML
            EntityAML_CHECKLOG eAMLCheckLog = new EntityAML_CHECKLOG();

            eAMLCheckLog.CORP_NO = htInput["HEADQUARTER_CORPNO"].ToString().Trim();
            eAMLCheckLog.TRANS_ID = "CSIPJCGQ";
            eAMLCheckLog.LAST_UPD_BRANCH = htInput["LAST_UPD_BRANCH"].ToString().Trim();
            eAMLCheckLog.LAST_UPD_CHECKER = htInput["LAST_UPD_CHECKER"].ToString().Trim();
            eAMLCheckLog.LAST_UPD_MAKER = htInput["LAST_UPD_MAKER"].ToString().Trim();
            eAMLCheckLog.MOD_USERID = eAgentInfo.agent_id;
            eAMLCheckLog.MOD_DATE = DateTime.Now.ToString("yyyyMMdd");
            eAMLCheckLog.MOD_TIME = DateTime.Now.ToString("HHmmss");

            InsertAMLCheckLog(eAMLCheckLog);
            #endregion
        }
    }

    /// <summary>
    /// 選擇獨立店事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void radSingleMerchant3_CheckedChanged(object sender, EventArgs e)
    {
        //20200215-RQ-2019-030155-003：拿掉獨立店/海外公司選項
        // 將查詢區的統一編號自動帶入下方統一編號
        //if (this.radSingleMerchant3.Checked)
        //    this.txtUniNo.Text = this.txtCardNo1.Text;
    }

    /// 作者 趙呂梁
    /// 創建日期：2010/1/20
    /// 修改日期：2010/1/20
    /// <summary>
    /// 新增隱藏事件
    /// </summary>
    protected void btnAddHiden_Click(object sender, EventArgs e)
    {
        base.strClientMsg += MessageHelper.GetMessage("01_01040201_004");
        base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo1"));
        UploadHtgInfo();
    }

    #endregion

    #region 方法區

    /// <summary>
    /// 載入下拉選單內容
    /// </summary>
    private void LoadDropDownList()
    {
        DataTable result = new DataTable();
        ListItem listItem = null;
        string countryCode = string.Empty;
        string amlcc = string.Empty;
        string organization = string.Empty;

        // 載入國籍
        result = BRPostOffice_CodeType.GetCodeType("1");

        //20190805 修改：將result Table排序 by Peggy
        DataView dv = result.DefaultView;
        dv.Sort = "CODE_ID asc";
        result = dv.ToTable();

        if (result != null && result.Rows.Count > 0)
        {
            for (int i = 0; i < result.Rows.Count; i++)
            {
                listItem = new ListItem();
                listItem.Value = result.Rows[i][0].ToString();
                listItem.Text = result.Rows[i][0].ToString();
                //listItem.Text = result.Rows[i][1].ToString();
                this.dropCountryCode.Items.Add(listItem);
                countryCode += result.Rows[i][0].ToString() + ":";
            }
            this.hidCountryCode.Value = countryCode;
        }

        // 載入AML行業編號
        result = BRPostOffice_CodeType.GetCodeType("3");
        if (result != null && result.Rows.Count > 0)
        {
            for (int i = 0; i < result.Rows.Count; i++)
            {
                amlcc += result.Rows[i][0].ToString() + ":";
            }
            this.hidAMLCC.Value = amlcc;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2010/1/20
    /// 修改日期：2010/1/20
    /// <summary>
    /// 清空頁面內容
    /// </summary>
    private void ClearPage(bool blnEnabled)
    {
        this.txtReceiveNumber.Text = "";
        this.txtBossID.Text = "";
        this.txtOperID.Text = "";
        this.txtJCIC.Text = "";
        this.txtGrantFeeFlag.Text = "";
        this.txtMposFlag.Text = "";
        this.txtPrevDesc.Text = "";
        this.txtInvoiceCycle.Text = "";
        this.txtRedeemCycle.Text = "";
        this.cddl_MemberService.SelectedValue = "";

        this.btnAdd.Enabled = blnEnabled;
        this.txtReceiveNumber.Enabled = blnEnabled;
        this.txtBossID.Enabled = blnEnabled;
        this.txtOperID.Enabled = blnEnabled;
        this.txtJCIC.Enabled = blnEnabled;
        this.txtGrantFeeFlag.Enabled = blnEnabled;
        this.txtMposFlag.Enabled = blnEnabled;
        this.txtPrevDesc.Enabled = blnEnabled;
        this.txtInvoiceCycle.Enabled = blnEnabled;
        this.txtRedeemCycle.Enabled = blnEnabled;
        this.cddl_MemberService.Enabled = blnEnabled;

        chkAddress.Checked = false;
        chkBusinessName.Checked = false;
        chkOper.Checked = false;

        this.lblCheckManText.Text = "";
        this.lblEstablishText.Text = "";
        this.lblCapitalText.Text = "";
        this.lblOrganizationText.Text = "";
        this.lblRiskText.Text = "";
        this.lblRegNameText.Text = "";
        this.lblBusinessNameText.Text = "";
        this.lblBusinessNameText_Other.Text = "";
        this.lblBossText.Text = "";
        this.lblBossTel1Text.Text = "";
        this.lblBossTel2Text.Text = "";
        this.lblBossTel3Text.Text = "";
        this.lblBossChangeDateText.Text = "";
        this.lblBossFlagText.Text = "";
        this.lblBossBirthdayText.Text = "";
        this.lblBossAtText.Text = "";
        this.lblBossRegAddr1Text.Text = "";
        this.lblBossRegAddr2Text.Text = "";
        this.lblBossRegAddr3Text.Text = "";
        this.lblOpermanText.Text = "";
        this.lblOperIDText.Text = "";
        this.lblOperTelText1.Text = "";
        this.lblOperTelText2.Text = "";
        this.lblOperTelText3.Text = "";
        this.lblOperChangeDateText.Text = "";
        this.lblOperFlagText.Text = "";
        this.lblOperBirthdayText.Text = "";
        this.lblOperAtText.Text = "";
        this.lblOpermanText_Other.Text = "";
        this.lblOperTelText1_Other.Text = "";
        this.lblOperTelText2_Other.Text = "";
        this.lblOperTelText3_Other.Text = "";
        this.lblOperChangeDateText_Other.Text = "";
        this.lblOperFlagText_Other.Text = "";
        this.lblOperBirthdayText_Other.Text = "";
        this.lblOperAtText_Other.Text = "";
        this.lblContactManText.Text = "";
        this.lblContactManTel1Text.Text = "";
        this.lblContactManTel2Text.Text = "";
        this.lblContactManTel3Text.Text = "";
        this.lblFax1Text.Text = "";
        this.lblFax2Text.Text = "";
        this.lblREG_ZIP_CODE.Text = "";
        this.lblBookAddr1Text.Text = "";
        this.lblBookAddr2Text.Text = "";
        this.lblBookAddr3Text.Text = "";
        this.lblBusinessZipText.Text = "";
        this.lblBusinessAddrText1.Text = "";
        this.lblBusinessAddrText2.Text = "";
        this.lblBusinessAddrText3.Text = "";
        this.lblZipText.Text = "";
        this.lblBusinessAddrText1_Other.Text = "";
        this.lblBusinessAddrText2_Other.Text = "";
        this.lblBusinessAddrText3_Other.Text = "";
        this.lblBankText.Text = "";
        this.lblBranchBankText.Text = "";
        this.lblNameText.Text = "";
        this.lblPopManText.Text = "";
        this.txtJCIC.Text = "";
        this.txtGrantFeeFlag.Text = "";
        this.txtMposFlag.Text = "";

        #region 表單新增欄位
        this.radSingleMerchant1.Enabled = blnEnabled;
        this.radSingleMerchant2.Enabled = blnEnabled;
        //20200215-RQ-2019-030155-003：拿掉獨立店/海外公司選項，新增分期平台選項
        //this.radSingleMerchant3.Enabled = blnEnabled;
        //this.radSingleMerchant4.Enabled = blnEnabled;
        this.radSingleMerchant5.Enabled = blnEnabled;
        //20210906 jack 自然人收單，新增分期平台選項
        this.radSingleMerchant6.Enabled = blnEnabled;

        this.txtUniNo.Enabled = blnEnabled;
        this.txtAMLCC.Enabled = blnEnabled;
        this.dropCountryCode.Enabled = blnEnabled;
        this.txtCountryCode.Enabled = blnEnabled;
        this.txtPassportNo.Enabled = blnEnabled;
        this.txtPassportExpdt.Enabled = blnEnabled;
        this.txtResidentNo.Enabled = blnEnabled;
        this.txtResidentExpdt.Enabled = blnEnabled;
        this.txtEmailFront.Enabled = blnEnabled;
        this.txtEmailOther.Enabled = blnEnabled;
        this.txtUniNo.Text = "";
        this.txtAMLCC.Text = "";
        this.txtCountryCode.Text = "";
        this.txtPassportNo.Text = "";
        this.txtPassportExpdt.Text = "";
        this.txtResidentNo.Text = "";
        this.txtResidentExpdt.Text = "";
        this.txtEmailFront.Text = "";
        this.txtEmailOther.Text = "";
        this.hidEmailFall.Value = "";
        #endregion

        //20190805 長姓名需求，新6個欄位 by Peggy↓
        chkisLongName.Checked = false;
        lblboss_1_L.Text = "";
        lblboss_1_Pinyin.Text = "";

        chkisLongName_c.Checked = false;
        lblcontact_man_L.Text = "";
        lblcontact_man_Pinyin.Text = "";

        //20190805 長姓名需求，新6個欄位 by Peggy↑

        // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis
        this.txtLAST_UPD_BRANCH.Enabled = blnEnabled;
        this.txtLAST_UPD_MAKER.Enabled = blnEnabled;
        this.txtLAST_UPD_CHECKER.Enabled = blnEnabled;
        this.txtLAST_UPD_BRANCH.Text = "";
        this.txtLAST_UPD_MAKER.Text = "";
        this.txtLAST_UPD_CHECKER.Text = "";
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/03
    /// 修改日期：2009/11/04
    /// <summary>
    /// 為網頁中的欄位賦值
    /// </summary>
    /// <param name="eShopBasic">EntitySHOP_BASIC</param>
    private void SetTwoKeyValues(CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC eShopBasic)
    {
        #region
        this.txtReceiveNumber.Text = eShopBasic.recv_no;// 收件編號
        this.txtBossID.Text = eShopBasic.boss_id;// 負責人ID
        if (eShopBasic.chk_oper != "1")
        {
            this.txtOperID.Text = "";// eShopBasic.oper_id;// 實際經營者ID
        }

        this.txtJCIC.Text = eShopBasic.jcic_code;// JCIC查詢
        this.txtMposFlag.Text = eShopBasic.mpos_flag;// Y_MPOS特店系統服務費免收註記
        this.txtGrantFeeFlag.Text = eShopBasic.grant_fee_flag;// Y_特店跨行匯費
        this.txtPrevDesc.Text = eShopBasic.prev_desc;// 帳單內容
        this.txtInvoiceCycle.Text = eShopBasic.invoice_cycle;// 發票週期
        //this.txtRedeemCycle.Text = eShopBasic.redeem_cycle;// 紅利週期(M/D)
        //this.cddl_MemberService.SelectedValue = eShopBasic.member_service;

        // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis        
        this.txtLAST_UPD_BRANCH.Text = eShopBasic.LAST_UPD_BRANCH;// 資料最後異動分行
        this.txtLAST_UPD_CHECKER.Text = eShopBasic.LAST_UPD_CHECKER;// 資料最後異動-CHECKER
        this.txtLAST_UPD_MAKER.Text = eShopBasic.LAST_UPD_MAKER;// 資料最後異動-MAKER
        #endregion
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/03
    /// 修改日期：2009/11/04
    /// <summary>
    /// 為網頁中的欄位賦值一KEY值并Disabled
    /// </summary>
    /// <param name="eShopBasic">EntitySHOP_BASIC</param>
    private void SetOneKeyValues(CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC eShopBasic)
    {
        this.txtReceiveNumber.Text = "";                         // 收件編號
        this.lblCheckManText.Text = eShopBasic.checkman;         // 徽信員
        this.lblEstablishText.Text = eShopBasic.establish;       // 設立
        this.lblCapitalText.Text = eShopBasic.capital;           // 資本
        this.lblOrganizationText.Text = eShopBasic.organization; // 組織
        this.lblRiskText.Text = eShopBasic.risk;                 //風險
        this.lblRegNameText.Text = eShopBasic.reg_name;          // 登記名稱
        if (eShopBasic.chk_business_name == "1")
        {
            this.chkBusinessName.Checked = true;
            this.lblBusinessNameText.Text = eShopBasic.business_name;// 營業名稱
        }
        else
        {
            this.chkBusinessName.Checked = false;
            this.lblBusinessNameText_Other.Text = eShopBasic.business_name;// 營業名稱
        }

        this.lblBossText.Text = eShopBasic.boss_1;                    // 負責人姓名
        this.lblBossTel1Text.Text = eShopBasic.boss_tel1;             // 負責人電話1
        this.lblBossTel2Text.Text = eShopBasic.boss_tel2;             // 負責人電話2
        this.lblBossTel3Text.Text = eShopBasic.boss_te3;              // 負責人電話3
        this.lblBossChangeDateText.Text = eShopBasic.boss_change_date;// 負責人領換補日
        this.lblBossFlagText.Text = eShopBasic.boss_change_flag;      // 負責人代號
        this.lblBossBirthdayText.Text = eShopBasic.boss_birthday;     // 負責人生日
        this.lblBossAtText.Text = eShopBasic.boss_at;                 // 負責人換證點
        this.lblBossRegAddr1Text.Text = eShopBasic.reg_addr1;         // 戶籍地址
        this.lblBossRegAddr2Text.Text = eShopBasic.reg_addr2;         // 戶籍地址
        this.lblBossRegAddr3Text.Text = eShopBasic.reg_addr3;         // 戶籍地址

        if (eShopBasic.chk_oper == "1")
        {
            this.chkOper.Checked = true;
            this.lblOpermanText.Text = eShopBasic.operman;                // 實際經營者姓名
            this.lblOperIDText.Text = "";// eShopBasic.oper_id;                 // 實際經營者ID
            this.lblOperTelText1.Text = eShopBasic.oper_tel1;             // 經營者電話
            this.lblOperTelText2.Text = eShopBasic.oper_tel2;             // 經營者電話
            this.lblOperTelText3.Text = eShopBasic.oper_tel3;             // 經營者電話
            this.lblOperChangeDateText.Text = eShopBasic.oper_change_date;// 經營者領換補日
            this.lblOperFlagText.Text = eShopBasic.oper_change_flag;      // 經營者代號
            this.lblOperBirthdayText.Text = eShopBasic.oper_birthday;     // 經營者生日
            this.lblOperAtText.Text = eShopBasic.oper_at;                 // 經營者換證點
        }
        else
        {
            this.chkOper.Checked = false;
            this.lblOpermanText_Other.Text = eShopBasic.operman;                // 實際經營者姓名
            this.lblOperTelText1_Other.Text = eShopBasic.oper_tel1;             // 經營者電話
            this.lblOperTelText2_Other.Text = eShopBasic.oper_tel2;             // 經營者電話
            this.lblOperTelText3_Other.Text = eShopBasic.oper_tel3;             // 經營者電話
            this.lblOperChangeDateText_Other.Text = eShopBasic.oper_change_date;// 經營者領換補日
            this.lblOperFlagText_Other.Text = eShopBasic.oper_change_flag;      // 經營者代號
            this.lblOperBirthdayText_Other.Text = eShopBasic.oper_birthday;     // 經營者生日
            this.lblOperAtText_Other.Text = eShopBasic.oper_at;                 // 經營者換證點
            this.txtOperID.ReadOnly = false;
            //2021/03/11_Ares_Stanley-修正粗框問題
            this.txtOperID.BackColor = Color.Empty;
        }

        this.lblContactManText.Text = eShopBasic.contact_man;     // 聯絡人姓名
        this.lblContactManTel1Text.Text = eShopBasic.contact_tel1;// 聯絡人電話
        this.lblContactManTel2Text.Text = eShopBasic.contact_tel2;// 聯絡人電話
        this.lblContactManTel3Text.Text = eShopBasic.contact_tel3;// 聯絡人電話
        this.lblFax1Text.Text = eShopBasic.fax1;                  // 聯絡人傳真1
        this.lblFax2Text.Text = eShopBasic.fax2;                  // 聯絡人傳真2
        this.lblBookAddr1Text.Text = eShopBasic.book_addr1;       // 登記地址
        this.lblBookAddr2Text.Text = eShopBasic.book_addr2;       // 登記地址
        this.lblBookAddr3Text.Text = eShopBasic.book_addr3;       // 登記地址

        if (eShopBasic.chk_address == "1")
        {
            this.chkAddress.Checked = true;
            this.lblBusinessZipText.Text = eShopBasic.zip;
            this.lblBusinessAddrText1.Text = eShopBasic.business_addr1;
            this.lblBusinessAddrText2.Text = eShopBasic.business_addr2;
            this.lblBusinessAddrText3.Text = eShopBasic.business_addr3;
        }
        else
        {
            this.chkAddress.Checked = false;
            this.lblZipText.Text = eShopBasic.zip;
            this.lblBusinessAddrText1_Other.Text = eShopBasic.business_addr1;
            this.lblBusinessAddrText2_Other.Text = eShopBasic.business_addr2;
            this.lblBusinessAddrText3_Other.Text = eShopBasic.business_addr3;
        }

        this.lblBankText.Text = eShopBasic.bank;              // 銀行
        this.lblBranchBankText.Text = eShopBasic.branch_bank; // 分行
        this.lblNameText.Text = eShopBasic.name;              // 戶名
        this.lblPopManText.Text = eShopBasic.pop_man;         // 推廣員
        this.txtMposFlag.Text = eShopBasic.mpos_flag;         // Y_MPOS特店系統服務費免收註記
        this.txtGrantFeeFlag.Text = eShopBasic.grant_fee_flag;// Y_特店跨行匯費

        #region 表單新增欄位
        // 表單新增欄位二Key需重新輸入
        // 商店別標識(1.總公司已往來 2.總公司未往來 3.獨立店)
        //if (eShopBasic.single_merchant == "1")
        //    this.radSingleMerchant1.Checked = true;// 總公司已往來
        //else if (eShopBasic.single_merchant == "2")
        //    this.radSingleMerchant2.Checked = true;// 總公司未往來
        //else
        //    this.radSingleMerchant3.Checked = true;// 獨立店

        //this.txtUniNo.Text = eShopBasic.headquarter_corpno;// 總公司已往來統一編號
        //this.txtAMLCC.Text = eShopBasic.aml_cc;            // AML行業編號
        this.txtCountryCode.Text = eShopBasic.country_code;     // 國籍
        this.txtPassportNo.Text = eShopBasic.passport_no;       // 護照號碼
        this.txtPassportExpdt.Text = eShopBasic.passport_expdt; // 護照效期
        this.txtResidentNo.Text = eShopBasic.resident_no;       // 居留證號
        this.txtResidentExpdt.Text = eShopBasic.resident_expdt; // 護照效期
        //SetEmailValue(eShopBasic.email);                   // 電子郵件信箱
        #endregion

        //20190806-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy↓
        this.lblboss_1_L.Text = eShopBasic.boss_1_L;//負責人-中文長姓名
        this.lblboss_1_Pinyin.Text = eShopBasic.boss_1_Pinyin;//負人責-羅馬拼音
        if (!lblboss_1_L.Text.Trim().Equals(""))//負責人-是否長姓名
        {
            this.chkisLongName.Checked = true;
        }

        this.lblcontact_man_L.Text = eShopBasic.contact_man_L;//聯絡人-中文長姓名
        this.lblcontact_man_Pinyin.Text = eShopBasic.contact_man_Pinyin;//聯絡人-羅馬拼音
        if (!lblcontact_man_L.Text.Trim().Equals(""))//聯絡人-是否長姓名
        {
            this.chkisLongName_c.Checked = true;
        }
        //20190806-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy↑

        // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis
        this.lblREG_ZIP_CODE.Text = eShopBasic.REG_ZIP_CODE;// 登記地址郵遞區號
        this.txtLAST_UPD_BRANCH.Text = eShopBasic.LAST_UPD_BRANCH;// 資料最後異動分行
        this.txtLAST_UPD_CHECKER.Text = eShopBasic.LAST_UPD_CHECKER;// 資料最後異動-CHECKER
        this.txtLAST_UPD_MAKER.Text = eShopBasic.LAST_UPD_MAKER;// 資料最後異動-MAKER
    }

    /// <summary>
    /// 填入Email到畫面
    /// </summary>
    /// <param name="email"></param>
    private void SetEmailValue(string email)
    {
        string[] emailArr = email.Split('@');
        if (emailArr.Length > 1)
        {
            this.txtEmailFront.Text = emailArr[0];
            switch (emailArr[1].ToLower())
            {
                case "gmail.com":
                    this.radGmail.Checked = true;
                    this.txtEmailOther.Text = "";
                    break;
                case "yahoo.com.tw":
                    this.radYahoo.Checked = true;
                    this.txtEmailOther.Text = "";
                    break;
                case "hotmail.com":
                    this.radHotmail.Checked = true;
                    this.txtEmailOther.Text = "";
                    break;
                default:
                    this.radOther.Checked = true;
                    this.txtEmailOther.Text = emailArr[1];
                    break;
            }
        }
    }

    private void DisabledColumn()
    {
        this.txtRedeemCycle.Text = "M";
        this.txtRedeemCycle.ReadOnly = true;
        this.txtRedeemCycle.BackColor = Color.LightGray;

        if (this.chkOper.Checked == true)
        {
            this.txtOperID.ReadOnly = true;
            this.txtOperID.BackColor = Color.LightGray;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/03
    /// 修改日期：2009/11/03
    /// <summary>
    /// 比較一次鍵檔和二次鍵檔的資料
    /// </summary>
    /// <returns></returns>
    private bool Compare()
    {
        bool blnSame = true;
        int intCount = 0;// 記錄不相同的數量

        if (ViewState["EntityShopBasic"] != null)
        {
            CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC eShopBasic = (CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC)ViewState["EntityShopBasic"];

            CompareValue(txtReceiveNumber, eShopBasic.recv_no, ref blnSame, ref intCount);// 收件編號

            CompareValueToUpper(txtBossID, eShopBasic.boss_id, ref blnSame, ref intCount);// 負責人ID

            if (eShopBasic.chk_oper != "1")
            {
                //CompareValueToUpper(txtOperID, eShopBasic.oper_id, ref blnSame, ref intCount);// 實際經營者ID
            }

            CompareValueToUpper(txtJCIC, eShopBasic.jcic_code, ref blnSame, ref intCount);// JCIC查詢

            CompareValueToUpper(txtMposFlag, eShopBasic.mpos_flag, ref blnSame, ref intCount);// Y_MPOS特店系統服務費免收註記

            CompareValueToUpper(txtGrantFeeFlag, eShopBasic.grant_fee_flag, ref blnSame, ref intCount);// Y_特店跨行匯費

            CompareValue(txtPrevDesc, eShopBasic.prev_desc, ref blnSame, ref intCount);// 帳單內容

            CompareValue(txtInvoiceCycle, eShopBasic.invoice_cycle, ref blnSame, ref intCount);// 發票週期

            //CompareValueToUpper(txtRedeemCycle, eShopBasic.redeem_cycle, ref blnSame, ref intCount);// 紅利週期(M/D)

            // 總公司已往來
            string singleMerchant = "1";

            // 總公司未往來
            if (this.radSingleMerchant2.Checked)
            {
                singleMerchant = "2";
            }
            //20200215-RQ-2019-030155-003：拿掉獨立店/海外公司選項，新增分期平台選項
            // 獨立店
            //else if (this.radSingleMerchant3.Checked)
            //{
            //    singleMerchant = "3";
            //}
            //// 海外公司
            //else if (this.radSingleMerchant4.Checked)
            //{
            //    singleMerchant = "4";
            //}
            //分期平台
            else if (this.radSingleMerchant5.Checked)
            {
                singleMerchant = "5";
            }
            //20210906 jack 自然人收單，新增分期平台選項
            else if (this.radSingleMerchant6.Checked)
            {
                singleMerchant = "6";
            }


            CompareValueToUpper(singleMerchant, eShopBasic.single_merchant, ref blnSame, ref intCount); // 總公司已往來、總公司未往來、獨立店

            CompareValueToUpper(txtUniNo, eShopBasic.headquarter_corpno, ref blnSame, ref intCount); // 總公司統一編號

            CompareValueToUpper(txtAMLCC, eShopBasic.aml_cc, ref blnSame, ref intCount); // AML行業編號

            CompareValueToUpper(txtCountryCode, eShopBasic.country_code, ref blnSame, ref intCount); // 國籍

            CompareValueToUpper(txtPassportNo, eShopBasic.passport_no, ref blnSame, ref intCount); // 護照號碼

            CompareValueToUpper(txtPassportExpdt, eShopBasic.passport_expdt, ref blnSame, ref intCount); // 護照效期

            CompareValueToUpper(txtResidentNo, eShopBasic.resident_no, ref blnSame, ref intCount); // 居留證號

            CompareValueToUpper(txtResidentExpdt, eShopBasic.resident_expdt, ref blnSame, ref intCount); // 居留效期

            CompareValueToUpper(hidEmailFall.Value, txtEmailFront, eShopBasic.email, ref blnSame, ref intCount); // E-Mail

            // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis
            CompareValueToUpper(txtLAST_UPD_BRANCH, eShopBasic.LAST_UPD_BRANCH, ref blnSame, ref intCount); // 資料最後異動分行

            CompareValueToUpper(txtLAST_UPD_MAKER, eShopBasic.LAST_UPD_MAKER, ref blnSame, ref intCount); // 資料最後異動-MAKER

            CompareValueToUpper(txtLAST_UPD_CHECKER, eShopBasic.LAST_UPD_CHECKER, ref blnSame, ref intCount); // 資料最後異動-CHECKER

            if (eShopBasic.member_service == null || this.cddl_MemberService.SelectedValue.Trim().ToUpper() !=
                eShopBasic.member_service.Trim().ToUpper())
            {
                intCount++;
                if (intCount == 1)
                {
                    if (this.cddl_MemberService.Enabled == true)
                    {
                        base.sbRegScript.Append(BaseHelper.SetFocus(this.cddl_MemberService.ID));
                    }
                }
                this.cddl_MemberService.BackColor = Color.Red;
                blnSame = false;
            }
            else if (this.cddl_MemberService.Enabled == false)
            {
                //2021/03/11_Ares_Stanley-修正粗框問題
                this.cddl_MemberService.BackColor = Color.Empty;
            }

            return blnSame;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 比較輸入欄位值與資料庫欄位值
    /// </summary>
    /// <param name="txtBox">TextBox</param>
    /// <param name="strValue">資料庫欄位值</param>
    /// <param name="blnSame">是否相同</param>
    /// <param name="intCount">比較數量</param>
    private void CompareValue(CustTextBox txtBox, string strValue, ref bool blnSame, ref int intCount)
    {
        // Add by Carolyn 2010.06.09
        string txtBoxValue = "";
        if (txtBox.ID == "txtBusinessName" || txtBox.ID == "txtBoss" || txtBox.ID == "txtRegAddr2" ||
            txtBox.ID == "txtRegAddr3" || txtBox.ID == "txtOperman" || txtBox.ID == "txtContactMan" || txtBox.ID == "txtBookAddr2" ||
            txtBox.ID == "txtBookAddr3" || txtBox.ID == "txtBusinessAddr5" || txtBox.ID == "txtBusinessAddr6" || txtBox.ID == "txtName" ||
            txtBox.ID == "txtBranchBank" || txtBox.ID == "txtPrevDesc" || txtBox.ID == "txtPopMan")
        {
            txtBoxValue = txtBox.Text;
        }
        else
        {
            txtBoxValue = txtBox.Text.Trim();
        }

        if (txtBoxValue != NullToString(strValue))
        {
            intCount++;
            if (intCount == 1)
            {
                if (txtBox.Enabled == true)
                {
                    base.sbRegScript.Append(BaseHelper.SetFocus(txtBox.ID));
                }
            }

            txtBox.BackColor = Color.Red;
            blnSame = false;
        }
        else if (txtBox.ReadOnly == false)
        {
            //2021/03/11_Ares_Stanley-修正粗框問題
            txtBox.BackColor = Color.Empty;
        }
    }

    /// <summary>
    /// 比較輸入欄位值與資料庫欄位值(轉大寫比較)
    /// </summary>
    /// <param name="txtBox">TextBox</param>
    /// <param name="strValue">資料庫欄位值</param>
    /// <param name="blnSame">是否相同</param>
    /// <param name="intCount">比較數量</param>
    private void CompareValueToUpper(CustTextBox txtBox, string strValue, ref bool blnSame, ref int intCount)
    {
        if (txtBox.Text.Trim().ToUpper() != NullToString(strValue.ToUpper()).Trim())
        {
            intCount++;
            if (intCount == 1)
            {
                if (txtBox.Enabled == true)
                {
                    base.sbRegScript.Append(BaseHelper.SetFocus(txtBox.ID));
                }
            }

            txtBox.BackColor = Color.Red;
            blnSame = false;
        }
        else if (txtBox.ReadOnly == false)
        {
            //2021/03/11_Ares_Stanley-修正粗框問題
            txtBox.BackColor = Color.Empty;
        }
    }

    private void CompareValueToUpper(string inputValue, CustTextBox txtBox, string strValue, ref bool blnSame, ref int intCount)
    {
        if (inputValue.Trim().ToUpper() != NullToString(strValue.ToUpper()).Trim())
        {
            intCount++;
            if (intCount == 1)
            {
                if (txtBox.Enabled == true)
                {
                    base.sbRegScript.Append(BaseHelper.SetFocus(txtBox.ID));
                }
            }

            txtBox.BackColor = Color.Red;
            blnSame = false;
        }
        else if (txtBox.ReadOnly == false)
        {
            //2021/03/11_Ares_Stanley-修正粗框問題
            txtBox.BackColor = Color.Empty;
        }
    }

    private void CompareValueToUpper(string inputValue, string strValue, ref bool blnSame, ref int intCount)
    {
        if (inputValue.Trim().ToUpper() != NullToString(strValue.ToUpper()).Trim())
        {
            intCount++;

            radSingleMerchant1.BackColor = Color.Red;
            radSingleMerchant2.BackColor = Color.Red;
            //20200215-RQ-2019-030155-003：拿掉獨立店/海外公司選項，新增分期平台選項
            //radSingleMerchant3.BackColor = Color.Red;
            //radSingleMerchant4.BackColor = Color.Red;
            radSingleMerchant5.BackColor = Color.Red;
            //20210906 jack 自然人收單，新增分期平台選項
            radSingleMerchant6.BackColor = Color.Red;
            blnSame = false;
        }
        else
        {
            radSingleMerchant1.BackColor = (Color)ColorTranslator.FromHtml("#cef9f9");
            radSingleMerchant2.BackColor = (Color)ColorTranslator.FromHtml("#cef9f9");
            //20200215-RQ-2019-030155-003：拿掉獨立店/海外公司選項，新增分期平台選項
            //radSingleMerchant3.BackColor = (Color)ColorTranslator.FromHtml("#cef9f9");
            //radSingleMerchant4.BackColor = (Color)ColorTranslator.FromHtml("#cef9f9");
            radSingleMerchant5.BackColor = (Color)ColorTranslator.FromHtml("#cef9f9");
            //20210906 jack 自然人收單，新增分期平台選項
            radSingleMerchant6.BackColor = (Color)ColorTranslator.FromHtml("#cef9f9");
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/04
    /// 修改日期：2009/11/04
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
    /// 創建日期：2009/11/04
    /// 修改日期：2009/11/04; 20210914_Ares_Stanley-調整端末訊息跨域錯誤
    /// <summary>
    ///  上傳主機資料
    /// </summary>
    private bool UploadHtgInfo()
    {
        bool result = true;

        if (!UploadHtgInfo(GetUploadHtgInfo("")))// 上傳主機失敗
        {
            result = false;
            //base.strIsShow = "var local = window.parent.location!=window.location?window.parent:window.opener?window.opener.parent:window;local.document.all.clientmsg.style.cursor='hand';local.document.all.clientmsg.innerText='" + strClientMsg + "';local.document.all.clientmsg.style.display=''; local.document.all.hostmsg.style.cursor='hand';local.document.all.hostmsg.innerText='" + strHostMsg + "';local.document.all.hostmsg.style.display='';if(confirm('" + strHostMsg + "\\n" + MessageHelper.GetMessage("01_01040201_006") + "')) {$('#btnHiden').click();}";
            string hostUrlString = @"window.parent.postMessage({ func: 'HostMsgShow', data: '" + strHostMsg + "' }, '*');";
            base.sbRegScript.Append(hostUrlString);
            string clientUrlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + strClientMsg + "' }, '*');";
            base.sbRegScript.Append(clientUrlString);
            base.strIsShow = "if(confirm('" + strHostMsg + "\\n" + MessageHelper.GetMessage("01_01040201_006") + "')) {$('#btnHiden').click();}";
        }

        return result;
    }

    /// <summary>
    /// 上傳主機
    /// </summary>
    /// <param name="htInput">上傳主機信息的HashTable</param>
    /// <returns>true成功，false失敗</returns>
    private bool UploadHtgInfo(Hashtable htInput)
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGQ, htInput, false, "21", eAgentInfo);

        if (!htResult.Contains("HtgMsg"))
        {
            UpdateData("Y");// 更新二KEY上傳主機標識
            UpdateOneKeyData();// 更新一KEY上傳主機標識
            base.strHostMsg += htResult["HtgSuccess"].ToString();// 主機返回成功訊息;
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_039");

            #region 20190806-RQ-2019-008595-002-長姓名需求 by Peggy
            List<EntityHTG_JC68> _JC68s = new List<EntityHTG_JC68>();
            EntityHTG_JC68 _tmpJC68 = new EntityHTG_JC68();

            #region 負責人
            string _mLongName = string.Empty;
            EntityHTG_JC68 htReturn_JC68 = GetJC68(txtBossID.Text.Trim());//負責人id
            if (htReturn_JC68.MESSAGE_TYPE.Trim().Equals("0000") || htReturn_JC68.MESSAGE_TYPE.Trim().Equals("0001"))
                _mLongName = "Y";
            else
                _mLongName = "N";

            if (chkisLongName.Checked)//當新增資料為長姓名時
            {
                _tmpJC68 = new EntityHTG_JC68();
                _tmpJC68.ID = txtBossID.Text.Trim();
                _tmpJC68.LONG_NAME = ToWide(lblboss_1_L.Text.Trim());
                _tmpJC68.PINYIN_NAME = LongNameRomaClean(lblboss_1_Pinyin.Text.Trim());

                _JC68s.Add(_tmpJC68);
            }
            else
            {
                if (_mLongName.Trim().Equals("Y"))//當複製來源資料為長姓名，但現在卻為短姓名時
                {
                    _tmpJC68 = new EntityHTG_JC68();
                    _tmpJC68.ID = txtBossID.Text.Trim();
                    _tmpJC68.LONG_NAME = "";
                    _tmpJC68.PINYIN_NAME = "";

                    _JC68s.Add(_tmpJC68);
                }
            }
            #endregion

            #region 聯絡人
            htReturn_JC68 = new EntityHTG_JC68(); string _mContactLName= string.Empty;
            htReturn_JC68 = GetJC68(txtCardNo1.Text.Trim() + txtCardNo2.Text.Trim());
            if (htReturn_JC68.MESSAGE_TYPE.Trim().Equals("0000") || htReturn_JC68.MESSAGE_TYPE.Trim().Equals("0001"))
                _mContactLName = "Y";
            else
                _mContactLName = "N";

            if (chkisLongName_c.Checked)//當新增資料為長姓名時
            {
                _tmpJC68 = new EntityHTG_JC68();
                _tmpJC68.ID = txtCardNo1.Text.Trim() + txtCardNo2.Text.Trim();
                _tmpJC68.LONG_NAME = ToWide(lblcontact_man_L.Text.Trim());
                _tmpJC68.PINYIN_NAME = LongNameRomaClean(lblcontact_man_Pinyin.Text.Trim());

                _JC68s.Add(_tmpJC68);
            }
            else
            {
                if (_mContactLName.Trim().Equals("Y"))//當複製來源資料為長姓名，但現在卻為短姓名時
                {
                    _tmpJC68 = new EntityHTG_JC68();
                    _tmpJC68.ID = txtCardNo1.Text.Trim() + txtCardNo2.Text.Trim();
                    _tmpJC68.LONG_NAME = "";
                    _tmpJC68.PINYIN_NAME = "";

                    _JC68s.Add(_tmpJC68);
                }
            }

            #endregion

            using (BRHTG_JC68 obj = new BRHTG_JC68("P010104020001"))
            {
                EntityResult _EntityResult = new EntityResult();
                foreach (EntityHTG_JC68 item in _JC68s)
                {
                    _EntityResult = obj.Update(item, this.eAgentInfo, "21");
                    if (_EntityResult.Success == false)//錯誤訊息
                    {
                        base.strHostMsg += "更新長姓名資料:" + _EntityResult.HostMsg;
                        base.strClientMsg += "更新長姓名資料:" + _EntityResult.HostMsg;
                    }
                }
            }
            #endregion

            ClearPage(false);
            this.txtCardNo1.Text = "";
            this.txtCardNo2.Text = "";
            
            return true;
        }
        else
        {
            if (htResult["HtgMsgFlag"].ToString() == "0")// 若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            {
                base.strHostMsg += htResult["HtgMsg"].ToString();
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_027");
            }
            else
            {
                base.strClientMsg += htResult["HtgMsg"].ToString();
            }
            if (htResult["MESSAGE_TYPE"] != null && htResult["MESSAGE_TYPE"].ToString() == "5555")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
        
    /// 作者 趙呂梁
    /// 創建日期：2009/11/04
    /// 修改日期：2009/11/04
    /// <summary>
    /// 得到上傳主機信息
    /// </summary>
    /// <returns>Hashtable</returns>
    /// <param name="strSend3270">強制執行</param>
    private Hashtable GetUploadHtgInfo(string strSend3270)
    {
        CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC eShop1KeyBasic = (CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC)ViewState["EntityShopBasic"];
        Hashtable htInput = new Hashtable();

        htInput.Add("FUNCTION_CODE", "A");// 新增
        htInput.Add("CORP_NO", this.txtCardNo1.Text.Trim());// 統一編號1
        htInput.Add("CORP_SEQ", this.txtCardNo2.Text.Trim());// 統一編號2
        htInput.Add("APPL_NO", this.txtReceiveNumber.Text.Trim());// 收件編號
        //htInput.Add("BUILD_DATE", eShop1KeyBasic.establish);// 設立
        htInput.Add("BUILD_DATE", eShop1KeyBasic.establish.Substring(0, 5));// 設立(yyymm)
        htInput.Add("CREDIT_NO", eShop1KeyBasic.checkman);// 徵信員
        htInput.Add("CAPITAL", eShop1KeyBasic.capital);// 資本
        htInput.Add("REG_NAME", eShop1KeyBasic.reg_name);// 登記名稱
        //htInput.Add("ORGAN_TYPE", eShop1KeyBasic.organization);// 組織
        htInput.Add("BUSINESS_NAME", eShop1KeyBasic.business_name);// 營業名稱

        htInput.Add("RISK_FLAG", eShop1KeyBasic.risk);// 風險
        htInput.Add("OWNER_NAME", eShop1KeyBasic.boss_1);// 負責人
        htInput.Add("OWNER_ID", this.txtBossID.Text.Trim().ToUpper());// 負責人ID
        htInput.Add("OWNER_PHONE_AREA", eShop1KeyBasic.boss_tel1);// 負責人電話1
        htInput.Add("OWNER_PHONE_NO", eShop1KeyBasic.boss_tel2);// 負責人電話2
        htInput.Add("OWNER_PHONE_EXT", eShop1KeyBasic.boss_te3);// 負責人電話3
        htInput.Add("OWNER_CITY", eShop1KeyBasic.reg_addr1);// 負責人戶籍地址1
        htInput.Add("OWNER_ADDR1", eShop1KeyBasic.reg_addr2);// 負責人戶籍地址2
        htInput.Add("OWNER_ADDR2", eShop1KeyBasic.reg_addr3);// 負責人戶籍地址3
        htInput.Add("CHANGE_DATE1", eShop1KeyBasic.boss_change_date);// 負責人領換補日
        htInput.Add("BIRTHDAY1", eShop1KeyBasic.boss_birthday);// 負責人生日
        htInput.Add("CHANGE_FLAG1", eShop1KeyBasic.boss_change_flag);// 負責人代號
        htInput.Add("AT1", eShop1KeyBasic.boss_at);// 負責人換證點

        /*20191024 修改：因操作模式關係，有可能叫出舊資料是有實際經營者的資訊，故以空白帶給主機，以絕後患!!  by Peggy
        if (!chkOper.Checked)
        {
            htInput.Add("MANAGER_ID", this.txtOperID.Text.Trim().ToUpper());// 實際經營者ID
        }
        else
        {
            htInput.Add("MANAGER_ID", eShop1KeyBasic.oper_id);
        }

        htInput.Add("MANAGER_NAME", eShop1KeyBasic.operman);// 實際經營者

        htInput.Add("MANAGER_PHONE_AREA", eShop1KeyBasic.oper_tel1);// 實際經營者電話1
        htInput.Add("MANAGER_PHONE_NO", eShop1KeyBasic.oper_tel2);// 實際經營者電話2
        htInput.Add("MANAGER_PHONE_EXT", eShop1KeyBasic.oper_tel3);// 實際經營者電話3
        htInput.Add("CHANGE_DATE2", eShop1KeyBasic.oper_change_date);// 實際經營者領換補日
        htInput.Add("BIRTHDAY2", eShop1KeyBasic.oper_birthday);// 實際經營者生日
        htInput.Add("CHANGE_FLAG2", eShop1KeyBasic.oper_change_flag);// 實際經營者代號
        htInput.Add("AT2", eShop1KeyBasic.oper_at);// 實際經營者換證點
        */
        htInput.Add("MANAGER_ID", "");// 實際經營者ID
        htInput.Add("MANAGER_NAME", "");// 實際經營者
        htInput.Add("MANAGER_PHONE_AREA", "");// 實際經營者電話1
        htInput.Add("MANAGER_PHONE_NO", "");// 實際經營者電話2
        htInput.Add("MANAGER_PHONE_EXT", "");// 實際經營者電話3
        htInput.Add("CHANGE_DATE2", "");// 實際經營者領換補日
        htInput.Add("BIRTHDAY2", "");// 實際經營者生日
        htInput.Add("CHANGE_FLAG2", "");// 實際經營者代號
        htInput.Add("AT2", "");// 實際經營者換證點

        htInput.Add("CONTACT_NAME", eShop1KeyBasic.contact_man);// 聯絡人
        htInput.Add("CONTACT_PHONE_AREA", eShop1KeyBasic.contact_tel1);// 聯絡人電話1
        htInput.Add("CONTACT_PHONE_NO", eShop1KeyBasic.contact_tel2);// 聯絡人電話2
        htInput.Add("CONTACT_PHONE_EXT", eShop1KeyBasic.contact_tel3);// 聯絡人電話3
        htInput.Add("FAX_AREA", eShop1KeyBasic.fax1);// 聯絡人傳真1
        htInput.Add("FAX_PHONE_NO", eShop1KeyBasic.fax2);// 聯絡人傳真2
        htInput.Add("REG_CITY", eShop1KeyBasic.book_addr1);// 登記地址1
        htInput.Add("REG_ADDR1", eShop1KeyBasic.book_addr2);// 登記地址2
        htInput.Add("REG_ADDR2", eShop1KeyBasic.book_addr3);// 登記地址3

        htInput.Add("REAL_ZIP", eShop1KeyBasic.zip);// 營業地郵遞區號
        htInput.Add("REAL_CITY", eShop1KeyBasic.business_addr1);// 營業地址1
        htInput.Add("REAL_ADDR1", eShop1KeyBasic.business_addr2);// 營業地址2
        htInput.Add("REAL_ADDR2", eShop1KeyBasic.business_addr3);// 營業地址3

        htInput.Add("DDA_BANK_NAME", eShop1KeyBasic.bank);// 銀行
        htInput.Add("DDA_BANK_BRANCH", eShop1KeyBasic.branch_bank);// 分行別
        htInput.Add("DDA_ACCT_NAME", eShop1KeyBasic.name);// 戶名
        htInput.Add("SALE_NAME", eShop1KeyBasic.pop_man);// 推廣員
        htInput.Add("INVOICE_CYCLE", this.txtInvoiceCycle.Text.Trim());// 發票週期
        htInput.Add("FORCE_FLAG", strSend3270);// 強制

        htInput.Add("JCIC_CODE", this.txtJCIC.Text.Trim().ToUpper());// JCIC查詢
        htInput.Add("MPOS_FLAG", this.txtMposFlag.Text.Trim().ToUpper());// Y_MPOS特店系統服務費免收註記
        htInput.Add("GRANT_FEE_FLAG", this.txtGrantFeeFlag.Text.Trim().ToUpper());// Y_特店跨行匯費
        htInput.Add("IPMR_PREV_DESC", this.txtPrevDesc.Text);// 帳單內容
        //htInput.Add("REDEEM_CYCLE", this.txtRedeemCycle.Text.Trim().ToUpper());// 紅利週期(M/D)
        htInput.Add("REDEEM_CYCLE", this.cddl_MemberService.SelectedValue.Trim().ToUpper());

        #region 電文新增欄位
        htInput.Add("BUILD_DATE_DD", eShop1KeyBasic.establish.Substring(5, 2));// 設立(dd)
        htInput.Add("ORGAN_TYPE_NEW", eShop1KeyBasic.organization);         // 組織

        // 商店別標識(1.總公司已往來 2.總公司未往來 3.獨立店 4.海外公司)
        if (this.radSingleMerchant1.Checked)
            htInput.Add("SINGLE_MERCHANT", "1");// 總公司已往來
        else if (this.radSingleMerchant2.Checked)
            htInput.Add("SINGLE_MERCHANT", "2");// 總公司未往來
        //20200215-RQ-2019-030155-003：拿掉獨立店/海外公司選項，新增分期平台選項
        //else if (this.radSingleMerchant3.Checked)
        //    htInput.Add("SINGLE_MERCHANT", "3");// 獨立店
        //else if (this.radSingleMerchant4.Checked)
        //    htInput.Add("SINGLE_MERCHANT", "4");// 海外公司
        else if (this.radSingleMerchant5.Checked)
            htInput.Add("SINGLE_MERCHANT", "5");//分期平台
        //20210906 jack 自然人收單，新增分期平台選項
        else if (this.radSingleMerchant6.Checked)
            htInput.Add("SINGLE_MERCHANT", "6");//自然人收單

        htInput.Add("HEADQUARTER_CORPNO", eShop1KeyBasic.headquarter_corpno);// 統一編號
        htInput.Add("AML_CC", this.txtAMLCC.Text.Trim());                   // AML行業編號
        htInput.Add("COUNTRY_CODE", this.txtCountryCode.Text.Trim());       // 國籍
        htInput.Add("PASSPORT_NO", this.txtPassportNo.Text.Trim());         // 護照號碼
        htInput.Add("PASSPORT_EXPDT", this.txtPassportExpdt.Text.Trim());   // 護照效期
        htInput.Add("RESIDENT_NO", this.txtResidentNo.Text.Trim());         // 居留證號
        htInput.Add("RESIDENT_EXPDT", this.txtResidentExpdt.Text.Trim());   // 居留效期
        htInput.Add("EMAIL", this.hidEmailFall.Value.Trim());               // 電子郵件信箱
        #endregion

        //20190806-RQ-2019-008595-002-長姓名需求，電文新增欄位 by Peggy↓
        htInput.Add("OWNER_LNAM_FLAG", !eShop1KeyBasic.boss_1_L.Trim().Equals("") ? "Y" : "N");// 負責人長姓名FLAG
        htInput.Add("CONTACT_LNAM_FLAG", !eShop1KeyBasic.contact_man_L.Trim().Equals("") ? "Y" : "N");// 聯絡人長姓名FLAG
        //20190806-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy↑

        // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis
        htInput.Add("REG_ZIP_CODE", eShop1KeyBasic.REG_ZIP_CODE);// 登記地址郵遞區號
        htInput.Add("LAST_UPD_BRANCH", eShop1KeyBasic.LAST_UPD_BRANCH);// 資料最後異動分行
        htInput.Add("LAST_UPD_CHECKER", eShop1KeyBasic.LAST_UPD_CHECKER);// 資料最後異動-CHECKER
        htInput.Add("LAST_UPD_MAKER", eShop1KeyBasic.LAST_UPD_MAKER);// 資料最後異動-MAKER

        return htInput;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/04
    /// 修改日期：2009/11/04
    /// <summary>
    /// 更新資料庫
    /// </summary>
    private bool UpdateData(string strSendHostFlag)
    {
        CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC eShopBasic = GetEntity(strSendHostFlag);

        try
        {
            return BRSHOP_BASIC.Update(eShopBasic, this.txtCardNo1.Text.Trim(), this.txtCardNo2.Text.Trim(), "", "2", "N");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return false;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/29
    /// 修改日期：2009/10/29
    /// <summary>
    /// 向資料庫中新增資料
    /// </summary>
    /// <returns>true成功,false失敗</returns>
    private bool AddNewData(string strSendHostFlag)
    {
        CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC eShopBasic = GetEntity(strSendHostFlag);

        try
        {
            return BRSHOP_BASIC.AddEntity(eShopBasic);
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return false;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/04
    /// 修改日期：2009/11/04
    /// <summary>
    /// 更新一KEY資料庫上傳主機標識
    /// </summary>
    private bool UpdateOneKeyData()
    {
        //EntitySHOP_BASIC eShopBasicOneKey = (EntitySHOP_BASIC)ViewState["EntityShopBasic"];// 一KEY資料
        CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC eShopBasic = new CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC();
        eShopBasic.sendhost_flag = "Y";
        string[] strFields = { CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC.M_sendhost_flag };

        try
        {
            return BRSHOP_BASIC.Update(eShopBasic, this.txtCardNo1.Text.Trim(), this.txtCardNo2.Text.Trim(), "1", "N", "", strFields);
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return false;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/29
    /// 修改日期：2009/10/29
    /// <summary>
    /// 得到插入或更新資料的Entity
    /// </summary>
    /// <returns>Entity</returns>
    private CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC GetEntity(string strSendHostFlag)
    {
        #region
        CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC eShopBasic = new CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC();
        CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC eShop1KeyBasic = (CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC)ViewState["EntityShopBasic"];

        eShopBasic.uni_no1 = this.txtCardNo1.Text.Trim();
        eShopBasic.uni_no2 = this.txtCardNo2.Text.Trim();
        eShopBasic.recv_no = this.txtReceiveNumber.Text.Trim();

        eShopBasic.checkman = eShop1KeyBasic.checkman;                // 徵信員
        eShopBasic.establish = eShop1KeyBasic.establish;              // 設立
        eShopBasic.capital = eShop1KeyBasic.capital;                  // 資本
        eShopBasic.organization = eShop1KeyBasic.organization;        // 組織
        eShopBasic.risk = eShop1KeyBasic.risk;                        // 風險
        eShopBasic.reg_name = eShop1KeyBasic.reg_name;                // 登記名稱
        eShopBasic.chk_business_name = eShop1KeyBasic.chk_business_name;
        eShopBasic.business_name = eShop1KeyBasic.business_name;      // 營業名稱
        eShopBasic.boss_1 = eShop1KeyBasic.boss_1;                    // 負責人姓名
        eShopBasic.boss_tel1 = eShop1KeyBasic.boss_tel1;              // 負責人電話1
        eShopBasic.boss_tel2 = eShop1KeyBasic.boss_tel2;              // 負責人電話2
        eShopBasic.boss_te3 = eShop1KeyBasic.boss_te3;                // 負責人電話3
        eShopBasic.boss_change_date = eShop1KeyBasic.boss_change_date;// 負責人領換補日
        eShopBasic.boss_change_flag = eShop1KeyBasic.boss_change_flag;// 負責人代號
        eShopBasic.boss_birthday = eShop1KeyBasic.boss_birthday;      // 負責人生日
        eShopBasic.boss_at = eShop1KeyBasic.boss_at;                  // 負責人換證點
        eShopBasic.reg_addr1 = eShop1KeyBasic.reg_addr1;              // 戶籍地址
        eShopBasic.reg_addr2 = eShop1KeyBasic.reg_addr2;              // 戶籍地址
        eShopBasic.reg_addr3 = eShop1KeyBasic.reg_addr3;              // 戶籍地址
        eShopBasic.chk_oper = eShop1KeyBasic.chk_oper;
        eShopBasic.operman = eShop1KeyBasic.operman;                  // 實際經營者姓名

        eShopBasic.oper_tel1 = eShop1KeyBasic.oper_tel1;              // 經營者電話
        eShopBasic.oper_tel2 = eShop1KeyBasic.oper_tel2;              // 經營者電話
        eShopBasic.oper_tel3 = eShop1KeyBasic.oper_tel3;              // 經營者電話
        eShopBasic.oper_change_date = eShop1KeyBasic.oper_change_date;// 經營者領換補日
        eShopBasic.oper_change_flag = eShop1KeyBasic.oper_change_flag;// 經營者代號
        eShopBasic.oper_birthday = eShop1KeyBasic.oper_birthday;      // 經營者生日
        eShopBasic.oper_at = eShop1KeyBasic.oper_at;                  // 經營者換證點
        eShopBasic.contact_man = eShop1KeyBasic.contact_man;          // 聯絡人姓名
        eShopBasic.contact_tel1 = eShop1KeyBasic.contact_tel1;        // 聯絡人電話
        eShopBasic.contact_tel2 = eShop1KeyBasic.contact_tel2;        // 聯絡人電話
        eShopBasic.contact_tel3 = eShop1KeyBasic.contact_tel3;        // 聯絡人電話
        eShopBasic.fax1 = eShop1KeyBasic.fax1;                        // 聯絡人傳真1
        eShopBasic.fax2 = eShop1KeyBasic.fax2;                        // 聯絡人傳真2
        eShopBasic.book_addr1 = eShop1KeyBasic.book_addr1;            // 登記地址
        eShopBasic.book_addr2 = eShop1KeyBasic.book_addr2;            // 登記地址
        eShopBasic.book_addr3 = eShop1KeyBasic.book_addr3;            // 登記地址
        eShopBasic.chk_address = eShop1KeyBasic.chk_address;
        eShopBasic.zip = eShop1KeyBasic.zip;
        eShopBasic.business_addr1 = eShop1KeyBasic.business_addr1;
        eShopBasic.business_addr2 = eShop1KeyBasic.business_addr2;
        eShopBasic.business_addr3 = eShop1KeyBasic.business_addr3;
        eShopBasic.bank = eShop1KeyBasic.bank;              // 銀行
        eShopBasic.branch_bank = eShop1KeyBasic.branch_bank;// 分行
        eShopBasic.name = eShop1KeyBasic.name;              // 戶名
        eShopBasic.pop_man = eShop1KeyBasic.pop_man;        // 推廣員

        eShopBasic.boss_id = this.txtBossID.Text.Trim().ToUpper();
        eShopBasic.keyin_userID = eAgentInfo.agent_id;
        eShopBasic.invoice_cycle = this.txtInvoiceCycle.Text.Trim();
        eShopBasic.keyin_day = DateTime.Now.ToString("yyyyMMdd");

        // 實際經營者相關資料
        if (!chkOper.Checked)
        {
            eShopBasic.oper_id = "";// this.txtOperID.Text.Trim().ToUpper();
        }
        else
        {
            eShopBasic.oper_id = "";// eShop1KeyBasic.oper_id;// 實際經營者ID
        }

        eShopBasic.sendhost_flag = strSendHostFlag;
        eShopBasic.keyin_flag = "2";
        eShopBasic.prev_desc = this.txtPrevDesc.Text;
        //eShopBasic.redeem_cycle = this.txtRedeemCycle.Text.Trim().ToUpper();
        eShopBasic.member_service = this.cddl_MemberService.SelectedValue.Trim().ToUpper();
        eShopBasic.jcic_code = this.txtJCIC.Text.Trim().ToUpper();
        eShopBasic.mpos_flag = this.txtMposFlag.Text.Trim().ToUpper();         // Y_MPOS特店系統服務費免收註記(6086)F001
        eShopBasic.grant_fee_flag = this.txtGrantFeeFlag.Text.Trim().ToUpper();// Y_特店跨行匯費(6116)
        #endregion

        #region 表單新增欄位
        eShopBasic.single_merchant = eShop1KeyBasic.single_merchant;      // 商店別標識(1.總公司已往來 2.總公司未往來 3.獨立店)
        eShopBasic.headquarter_corpno = eShop1KeyBasic.headquarter_corpno;// 統一編號
        eShopBasic.aml_cc = eShop1KeyBasic.aml_cc;                        // AML行業編號
        eShopBasic.country_code = eShop1KeyBasic.country_code;            // 國籍
        eShopBasic.passport_no = eShop1KeyBasic.passport_no;              // 護照號碼
        eShopBasic.passport_expdt = eShop1KeyBasic.passport_expdt;        // 護照效期
        eShopBasic.resident_no = eShop1KeyBasic.resident_no;              // 居留證號
        eShopBasic.resident_expdt = eShop1KeyBasic.resident_expdt;        // 居留效期
        eShopBasic.email = eShop1KeyBasic.email;                          // 電子郵件信箱
        #endregion

        //20190806-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy
        eShopBasic.boss_1_L = eShop1KeyBasic.boss_1_L;
        eShopBasic.boss_1_Pinyin = eShop1KeyBasic.boss_1_Pinyin;
        eShopBasic.contact_man_L = eShop1KeyBasic.contact_man_L;
        eShopBasic.contact_man_Pinyin = eShop1KeyBasic.contact_man_Pinyin;

        // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis
        eShopBasic.REG_ZIP_CODE = eShop1KeyBasic.REG_ZIP_CODE;// 登記地址郵遞區號
        eShopBasic.LAST_UPD_BRANCH = this.txtLAST_UPD_BRANCH.Text.Trim();// 資料最後異動分行
        eShopBasic.LAST_UPD_CHECKER = this.txtLAST_UPD_CHECKER.Text.Trim();// 資料最後異動-CHECKER
        eShopBasic.LAST_UPD_MAKER = this.txtLAST_UPD_MAKER.Text.Trim();// 資料最後異動-MAKER

        return eShopBasic;
    }

    private void insertAML_AddHeadOfficeReport(string taxID, string branch_TaxID, string branch_No, string recv_no)
    {
        //string taxID = this.txtUniNo.Text.Trim();           // 總公司統一編號
        //string branch_TaxID = this.txtCardNo1.Text.Trim();  // 分公司統一編號
        //string branch_No = this.txtCardNo2.Text.Trim();     // 分公司序號
        //string recv_no = this.txtReceiveNumber.Text.Trim(); // 收件編號
        string create_day = DateTime.Now.ToString("yyyyMMdd");
        int masterID = 0;

        // 總公司未往來
        if (this.radSingleMerchant2.Checked)
        {
            masterID = BRSHOP_BASIC.insertAML_AddHeadOfficeReport(taxID, branch_TaxID, branch_No, recv_no, create_day);
        }
    }

    private EntityHTG_JC68 GetJC68(string strID)
    {
        EntityHTG_JC68 _result = new EntityHTG_JC68();
        using (BRHTG_JC68 obj = new BRHTG_JC68("P010104060001"))
        {
            EntityHTG_JC68 _data = new EntityHTG_JC68();

            _data.ID = strID;
            _result = obj.getData(_data, eAgentInfo, "11");
        }
        return _result;
    }

    #endregion
}