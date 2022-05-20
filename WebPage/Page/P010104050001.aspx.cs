//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：主機特店基本資料查詢(特店基本資料查核)
//*  創建日期：2010/1/20
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
//20190805 (U) by Peggy, 新增控制項：因長姓名需求，增加是否長姓名，負責人中文長姓名、負責人羅馬拼音、是否聯絡人長姓名、聯絡人長姓名、聯絡人羅馬拼音 6個控制項

using System;
using System.Collections;
using Framework.Common.Message;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.EntityLayer_new;

public partial class P010104050001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    //20191023 修改：SOC所需資訊  by Peggy
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo1"));//*將【統一編號】(1)設為輸入焦點
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//20191023 修改：SOC所需資訊  by Peggy
    }

    /// 作者 趙呂梁


    /// 創建日期：2010/1/20
    /// 修改日期：2010/1/20
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

        Hashtable htInput = new Hashtable();
        htInput.Add("FUNCTION_CODE", "I");//*查詢
        htInput.Add("CORP_NO", this.txtCardNo1.Text.Trim());//*統一編號1
        htInput.Add("CORP_SEQ", this.txtCardNo2.Text.Trim());//*統一編號2

        Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGQ, htInput, false, "11", eAgentInfo);

        if (!htReturn.Contains("HtgMsg"))
        {
            base.strHostMsg += htReturn["HtgSuccess"].ToString();//*主機返回成功訊息
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_031");
            SetValues(htReturn);
        }
        else
        {
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
            etMstType = eMstType.Select;
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

            if ((htReturn["MESSAGE_TYPE"] == null) || (htReturn["MESSAGE_TYPE"].ToString().Trim() != "8888"))
            {
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
            }
            else
            {

                base.strHostMsg += htReturn["HtgMsg"].ToString();
                base.strClientMsg += MessageHelper.GetMessage("01_01040400_001");
            }
            ClearText();
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo1"));
        }
    }

    /// 作者 趙呂梁


    /// 創建日期：2010/1/20
    /// 修改日期：2010/1/20
    /// <summary>
    /// 為網頁賦值


    /// </summary>
    /// <param name="htReturn">主機返回信息的hashtable</param>
    private void SetValues(Hashtable htReturn)
    {
        #region
        this.lblBankText.Text = htReturn["DDA_BANK_NAME"].ToString();//*銀行


        this.lblBookAddressText1.Text = htReturn["REG_CITY"].ToString();//*登記地址1
        this.lblBookAddressText2.Text = htReturn["REG_ADDR1"].ToString();//*登記地址2
        this.lblBookAddressText3.Text = htReturn["REG_ADDR2"].ToString();//*登記地址3
        this.lblBossText.Text = htReturn["OWNER_NAME"].ToString();//*負責人姓名


        this.lblBossIDText.Text = htReturn["OWNER_ID"].ToString();//*負責人ID
        this.lblBossTelText1.Text = htReturn["OWNER_PHONE_AREA"].ToString();//*負責人電話1
        this.lblBossTelText2.Text = htReturn["OWNER_PHONE_NO"].ToString();//*負責人電話2
        this.lblBossTelText3.Text = htReturn["OWNER_PHONE_EXT"].ToString();//*負責人電話3
        this.lblBranchBankText.Text = htReturn["DDA_BANK_BRANCH"].ToString();//*分行
        this.lblBusinessZipText.Text = htReturn["REAL_ZIP"].ToString();//*營業地郵遞區號


        this.lblBusinessAddressText1.Text = htReturn["REAL_CITY"].ToString();//*營業地址2
        this.lblBusinessAddressText2.Text = htReturn["REAL_ADDR1"].ToString();//*營業地址3
        this.lblBusinessAddressText3.Text = htReturn["REAL_ADDR2"].ToString();//*營業地址4
        this.lblBusinessNameText.Text = htReturn["BUSINESS_NAME"].ToString();//*營業名稱
        this.lblCapitalText.Text = htReturn["CAPITAL"].ToString();//*資本
        this.lblCheckManText.Text = htReturn["CREDIT_NO"].ToString();//*徵信員


        this.lblContactManNameText.Text = htReturn["CONTACT_NAME"].ToString();//*聯絡人姓名


        this.lblContactManTelText1.Text = htReturn["CONTACT_PHONE_AREA"].ToString();//*聯絡人電話1
        this.lblContactManTelText2.Text = htReturn["CONTACT_PHONE_NO"].ToString();//*聯絡人電話2
        this.lblContactManTelText3.Text = htReturn["CONTACT_PHONE_EXT"].ToString();//*聯絡人電話3
        this.lblEstablishText.Text = htReturn["BUILD_DATE"].ToString() + htReturn["BUILD_DATE_DD"].ToString();//*設立
        this.lblFaxText1.Text = htReturn["FAX_AREA"].ToString();//*聯絡人傳真1
        this.lblFaxText2.Text = htReturn["FAX_PHONE_NO"].ToString();//*聯絡人傳真2
        this.lblInvoiceCycleText.Text = htReturn["INVOICE_CYCLE"].ToString();//*發票週期

        this.lblNameText.Text = htReturn["DDA_ACCT_NAME"].ToString();//*戶名
        this.lblOperIDText.Text = htReturn["MANAGER_ID"].ToString();//*實際經營者ID
        this.lblOperTelText1.Text = htReturn["MANAGER_PHONE_AREA"].ToString();//*實際經營者電話1
        this.lblOperTelText2.Text = htReturn["MANAGER_PHONE_NO"].ToString();//*實際經營者電話2
        this.lblOperTelText3.Text = htReturn["MANAGER_PHONE_EXT"].ToString();//*實際經營者電話3
        this.lblOpermanText.Text = htReturn["MANAGER_NAME"].ToString();//*實際經營者姓名


        //this.lblOrganizationText.Text = htReturn["ORGAN_TYPE"].ToString();//*組織
        this.lblOrganizationText.Text = htReturn["ORGAN_TYPE_NEW"].ToString();//*法律形式
        
        this.lblPopManText.Text = htReturn["SALE_NAME"].ToString();//*推廣員


        this.lblReceiveNumberText.Text = htReturn["APPL_NO"].ToString();//*收件編號

        this.lblRegAddressText1.Text = htReturn["OWNER_CITY"].ToString();//*戶籍地址1
        this.lblRegAddressText2.Text = htReturn["OWNER_ADDR1"].ToString();//*戶籍地址2
        this.lblRegAddressText3.Text = htReturn["OWNER_ADDR2"].ToString();//*戶籍地址3
        this.lblRegNameText.Text = htReturn["REG_NAME"].ToString();//*登記名稱
        this.lblRiskText.Text = htReturn["RISK_FLAG"].ToString();//*風險

        this.lblBossChangeDateText.Text = htReturn["CHANGE_DATE1"].ToString();//*負責人領換補日


        this.lblBossBirthdayText.Text = htReturn["BIRTHDAY1"].ToString();//*負責人生日


        this.lblBossFlagText.Text = htReturn["CHANGE_FLAG1"].ToString();//*負責人代號


        this.lblBossAtText.Text = htReturn["AT1"].ToString();//*負責人換證點

        this.lblOperChangeDateText.Text = htReturn["CHANGE_DATE2"].ToString();//*實際經營者領換補日


        this.lblOperBirthdayText.Text = htReturn["BIRTHDAY2"].ToString();//*實際經營者生日


        this.lblOperFlagText.Text = htReturn["CHANGE_FLAG2"].ToString();//*實際經營者代號


        this.lblOperAtText.Text = htReturn["AT2"].ToString();//*實際經營者換證點

        this.lblJCICText.Text = htReturn["JCIC_CODE"].ToString();//*JCIC查詢

        this.lblGrantFeeFlagText.Text = htReturn["GRANT_FEE_FLAG"].ToString();//*Y_特店跨行匯費(6116)
        this.lblMposFlagText.Text = htReturn["MPOS_FLAG"].ToString();//*Y_MPOS特店系統服務費免收註記(6086)F001




        this.lblPrevDescText.Text = htReturn["IPMR_PREV_DESC"].ToString();//*帳單內容
        //this.lblRedeemCycleText.Text = htReturn["REDEEM_CYCLE"].ToString();//*紅利週期(M/D)

				this.clbl_MemberServiceText.Text = htReturn["REDEEM_CYCLE"].ToString().Trim().ToUpper();
				/*
        if (htReturn["REDEEM_CYCLE"] != null && (htReturn["REDEEM_CYCLE"].ToString().Trim().ToUpper() == "Y" ||
            htReturn["REDEEM_CYCLE"].ToString().Trim().ToUpper() == "N"))
        {
            this.clbl_MemberServiceText.Text = htReturn["REDEEM_CYCLE"].ToString().Trim().ToUpper();
        }
        else
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040101_010") + "');");
        }
        */


        //總公司型態
        if (htReturn.ContainsKey("SINGLE_MERCHANT"))
        {
            string SCorp_Type = htReturn["SINGLE_MERCHANT"].ToString();

            if (SCorp_Type == "1")
            {
                this.lblSingleMerchantText.Text = "總公司已往來";
            }
            if (SCorp_Type == "2")
            {
                this.lblSingleMerchantText.Text = "總公司未往來";
            }
            if (SCorp_Type == "3")
            {
                this.lblSingleMerchantText.Text = "獨立店";
            }
            if (SCorp_Type == "4")
            {
                this.lblSingleMerchantText.Text = "海外公司";
            }
            if (SCorp_Type == "5")//20200219-RQ-2019-030155-003 新增5.分期平台選項
            {
                this.lblSingleMerchantText.Text = "分期平台";
            }
            if (SCorp_Type == "6")//20211201-新增自然人收單 選項 Edit by Rick
            {
                this.lblSingleMerchantText.Text = "自然人收單";
            }
        }

        //總公司統一編號
        if (htReturn.ContainsKey("HEADQUARTER_CORPNO"))
        {
            lblHEADCorpNoText.Text = htReturn["HEADQUARTER_CORPNO"].ToString();
        }

        //AML行業編號
        if (htReturn.ContainsKey("AML_CC"))
        {
            lblAMLCCText.Text = htReturn["AML_CC"].ToString();
        }

        //負責人國籍
        //if (htReturn.ContainsKey("NATION"))
        //{
        //    lblNationText.Text = htReturn["NATION"].ToString();
        //}
        if (htReturn.ContainsKey("COUNTRY_CODE"))
        {
            lblNationText.Text = htReturn["COUNTRY_CODE"].ToString();
        }

        //負責人護照
        if (htReturn.ContainsKey("PASSPORT_NO"))
        {
            lblpassportText.Text = htReturn["PASSPORT_NO"].ToString();
        }

        //負責人護照效期
        if (htReturn.ContainsKey("PASSPORT_EXPDT"))
        {
            lblPassportTExpDateText.Text = htReturn["PASSPORT_EXPDT"].ToString();
        }


        //負責人居留證
        if (htReturn.ContainsKey("RESIDENT_NO"))
        {
            lblResidentNoText.Text = htReturn["RESIDENT_NO"].ToString();
        }

        //負責人居留證效期
        if (htReturn.ContainsKey("RESIDENT_EXPDT"))
        {
            lblResidentExpDateText.Text = htReturn["RESIDENT_EXPDT"].ToString();
        }


        //Email
        if (htReturn.ContainsKey("EMAIL"))
        {
            lblEmailText.Text = htReturn["EMAIL"].ToString();
        }

        //20190806-RQ-2019-008595-002-長姓名需求 by Peggy↓        
        if (!string.IsNullOrEmpty(htReturn["OWNER_LNAM_FLAG"].ToString()) && htReturn["OWNER_LNAM_FLAG"].ToString().Trim().Equals("Y"))//負責人長姓名flag
        {
            this.chkisLongName.Checked = true;
            
            EntityHTG_JC68 htReturn_JC68 = GetJC68(lblBossIDText.Text.Trim());
            lblboss_1_L.Text = htReturn_JC68.LONG_NAME;//負責人-中文長姓名
            lblboss_1_Pinyin.Text = htReturn_JC68.PINYIN_NAME;//負責人-羅馬拼音
        }
        else
        {
            this.chkisLongName.Checked = false;
        }

        if (!string.IsNullOrEmpty(htReturn["CONTACT_LNAM_FLAG"].ToString()) && htReturn["CONTACT_LNAM_FLAG"].ToString().Trim().Equals("Y"))//聯絡人長姓名flag
        {
            this.chkisLongName_c.Checked = true;

            EntityHTG_JC68 htReturn_JC68 = GetJC68(txtCardNo1.Text.Trim() + txtCardNo2.Text.Trim());
            lblcontact_man_L.Text = htReturn_JC68.LONG_NAME;//聯絡人-中文長姓名
            lblcontact_man_Pinyin.Text = htReturn_JC68.PINYIN_NAME;//聯絡人-羅馬拼音
        }
        else
        {
            this.chkisLongName_c.Checked = false;
        }

        //20190806-RQ-2019-008595-002-長姓名需求 by Peggy↑

        // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis
        if (htReturn.ContainsKey("REG_ZIP_CODE"))
        {
            this.lblREG_ZIP_CODE.Text = htReturn["REG_ZIP_CODE"].ToString();// 登記地址郵遞區號
        }
        if (htReturn.ContainsKey("LAST_UPD_BRANCH"))
        {
            this.lblLAST_UPD_BRANCHText.Text = htReturn["LAST_UPD_BRANCH"].ToString();// 資料最後異動分行
        }
        if (htReturn.ContainsKey("LAST_UPD_CHECKER"))
        {
            this.lblLAST_UPD_CHECKERText.Text = htReturn["LAST_UPD_CHECKER"].ToString();// 資料最後異動-CHECKER
        }            
        if (htReturn.ContainsKey("LAST_UPD_MAKER"))
        {
            this.lblLAST_UPD_MAKERText.Text = htReturn["LAST_UPD_MAKER"].ToString();// 資料最後異動-MAKER
        }            
        #endregion
    }

    /// 作者 趙呂梁
    /// 創建日期：2010/1/20
    /// 修改日期：2010/1/20
    /// <summary>
    /// 清空文本
    /// </summary>
    private void ClearText()
    {
        #region
        this.lblBankText.Text = "";
        this.lblBookAddressText1.Text = "";
        this.lblBookAddressText2.Text = "";
        this.lblBookAddressText3.Text = "";
        this.lblBossText.Text = "";
        this.lblBossIDText.Text = "";
        this.lblBossTelText1.Text = "";
        this.lblBossTelText2.Text = "";
        this.lblBossTelText3.Text = "";
        this.lblBranchBankText.Text = "";
        this.lblBusinessZipText.Text = "";
        this.lblBusinessAddressText1.Text = "";
        this.lblBusinessAddressText2.Text = "";
        this.lblBusinessAddressText3.Text = "";
        this.lblBusinessNameText.Text = "";
        this.lblCapitalText.Text = "";
        this.lblCheckManText.Text = "";
        this.lblContactManNameText.Text = "";
        this.lblContactManTelText1.Text = "";
        this.lblContactManTelText2.Text = "";
        this.lblContactManTelText3.Text = "";
        this.lblEstablishText.Text = "";
        this.lblFaxText1.Text = "";
        this.lblFaxText2.Text = "";
        this.lblInvoiceCycleText.Text = "";

        this.lblNameText.Text = "";
        this.lblOperIDText.Text = "";
        this.lblOperTelText1.Text = "";
        this.lblOperTelText2.Text = "";
        this.lblOperTelText3.Text = "";
        this.lblOpermanText.Text = "";
        this.lblOrganizationText.Text = "";
        this.lblPopManText.Text = "";
        this.lblReceiveNumberText.Text = "";
        this.lblRegAddressText1.Text = "";
        this.lblRegAddressText2.Text = "";
        this.lblRegAddressText3.Text = "";
        this.lblRegNameText.Text = "";
        this.lblRiskText.Text = "";

        this.lblBossChangeDateText.Text = "";
        this.lblBossBirthdayText.Text = "";
        this.lblBossFlagText.Text = "";
        this.lblBossAtText.Text = "";

        this.lblOperChangeDateText.Text = "";
        this.lblOperBirthdayText.Text = "";
        this.lblOperFlagText.Text = "";
        this.lblOperAtText.Text = "";

        this.lblJCICText.Text = "";
        this.lblPrevDescText.Text = "";
        //this.lblRedeemCycleText.Text = "";
        this.clbl_MemberServiceText.Text = "";

        //20190806-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy↓
        chkisLongName.Checked = false;
        lblboss_1_L.Text = "";
        lblboss_1_Pinyin.Text = "";

        chkisLongName_c.Checked = false;
        lblcontact_man_L.Text = "";
        lblcontact_man_Pinyin.Text = "";

        //20190806-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy↑

        //20200225-修正
        
        lblSingleMerchantText.Text = "";
        lblHEADCorpNoText.Text = "";
        lblAMLCCText.Text = "";
        lblNationText.Text = "";
        lblpassportText.Text = "";
        lblPassportTExpDateText.Text = "";
        lblResidentNoText.Text = "";
        lblResidentExpDateText.Text = "";
        lblEmailText.Text = "";

        lblGrantFeeFlagText.Text = "";//*Y_特店跨行匯費(6116)
        lblMposFlagText.Text = "";//*Y_MPOS特店系統服務費免收註記(6086)F001
        #endregion
    }

    //20190806-RQ-2019-008595-002-長姓名需求 by Peggy
    //依據長姓名FLAG至JC68取值
    private EntityHTG_JC68 GetJC68(string strID)
    {
        EntityHTG_JC68 _result = new EntityHTG_JC68();
        using (BRHTG_JC68 obj = new BRHTG_JC68("P010104050001"))
        {
            EntityHTG_JC68 _data = new EntityHTG_JC68();
            
            _data.ID = strID;
            _result = obj.getData(_data, eAgentInfo, "1");
        }
        return _result;
    }
}