//******************************************************************
//*  作    者：陳香琦
//*  功能說明：主機特店基本資料查詢(特店基本資料查核)
//*                      單純將主機資料帶出顯示    
//*  創建日期：2019/09/20
//*******************************************************************

using System;
using System.Collections;
using Framework.Common.Message;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.EntityLayer_new;

public partial class P010103030001 : PageBase
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
            base.sbRegScript.Append(BaseHelper.SetFocus("txtUNI_NO1"));//*將【統一編號】(1)設為輸入焦點
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//20191023 修改：SOC所需資訊  by Peggy
    }
    
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
        log.Customer_Id = this.txtUNI_NO1.Text.Trim() + txtUNI_NO2.Text.Trim();//查詢條件        
        log.Statement_Text = string.Format("CUSTOMER_ID:{0}|AC_NO:{1}|BRANCH_ID:{2}|ROLE_ID:{3}", log.Customer_Id, log.Account_Nbr, log.Branch_Nbr, log.Role_Id); //查詢條件內容: 用 | 區隔
        BRL_AP_LOG.Add(log);
        #endregion

        Hashtable htInput = new Hashtable();
        htInput.Add("FUNCTION_CODE", "I");//*查詢
        htInput.Add("CORP_NO", this.txtUNI_NO1.Text.Trim());//*統一編號1
        htInput.Add("CORP_SEQ", "0000");//*統一編號2

        Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC69, htInput, false, "11", eAgentInfo);

        if (!htReturn.Contains("HtgMsg"))
        {
            base.strHostMsg += htReturn["HtgSuccess"].ToString();//*主機返回成功訊息
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_031");
            SetValues(htReturn);
        }
        else
        {
            //鍵檔GUI訊息呈現方式
            etMstType = eMstType.Select;

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
            base.sbRegScript.Append(BaseHelper.SetFocus("txtUNI_NO1"));
        }
    }
    
    /// <summary>
    /// 為網頁賦值
    /// </summary>
    /// <param name="htReturn">主機返回信息的hashtable</param>
    private void SetValues(Hashtable htReturn)
    {
        #region
        //總公司統一編號
        if (htReturn.ContainsKey("HEAD_CORP_NO"))
        {
            lblHEAD_CORP_NO.Text = htReturn["HEAD_CORP_NO"].ToString();
        }
        //行業別
        if (htReturn.ContainsKey("CC"))
        {
            lblCORP_MCC.Text = htReturn["CC"].ToString();
        }
        //設立日期
        if (htReturn.ContainsKey("BUILD_DATE"))
        {
            lblCORP_ESTABLISH.Text = htReturn["BUILD_DATE"].ToString();
        }
        //法律形式
        if (htReturn.ContainsKey("BUSINESS_ORGAN_TYPE"))
        {
            lblCORP_Organization.Text = htReturn["BUSINESS_ORGAN_TYPE"].ToString();
        }
        //註冊國籍
        if (htReturn.ContainsKey("REGISTER_NATION"))
        {
            lblCORP_CountryCode.Text = htReturn["REGISTER_NATION"].ToString();
        }
        //州別
        if (htReturn.ContainsKey("REGISTER_US_STATE"))
        {
            lblCORP_CountryStateCode.Text = htReturn["REGISTER_US_STATE"].ToString();
        }
        //總公司登記名稱
        if (htReturn.ContainsKey("REG_NAME"))
        {
            lblREG_NAME_CH.Text = htReturn["REG_NAME"].ToString();
        }
        //總公司英文名稱
        if (htReturn.ContainsKey("CORP_REG_ENG_NAME"))
        {
            lblREG_NAME_EN.Text = htReturn["CORP_REG_ENG_NAME"].ToString();
        }
        //登記地址
        if (htReturn.ContainsKey("REG_CITY"))
        {
            lblREG_ADDR.Text = htReturn["REG_CITY"].ToString() + htReturn["REG_ADDR1"].ToString() + htReturn["REG_ADDR2"].ToString();
        }
        //登記電話
        if (htReturn.ContainsKey("TEL"))
        {
            lblCORP_TEL.Text = htReturn["TEL"].ToString();
        }
        //負責人
        if (htReturn.ContainsKey("OWNER_CHINESE_NAME"))
        {
            lblPrincipalNameCH.Text = htReturn["OWNER_CHINESE_NAME"].ToString();
        }
        //負責人ID
        if (htReturn.ContainsKey("OWNER_ID"))
        {
            lblPrincipalIDNo.Text = htReturn["OWNER_ID"].ToString();
        }
        //長姓名
        if (htReturn.ContainsKey("LONG_NAME_FLAG"))
        {
            chkisLongName.Checked = htReturn["LONG_NAME_FLAG"].ToString().Trim().Equals("Y") ? true : false;
        }
        
        if (!string.IsNullOrEmpty(htReturn["LONG_NAME_FLAG"].ToString()) && htReturn["LONG_NAME_FLAG"].ToString().Trim().Equals("Y") 
            && !lblPrincipalIDNo.Text.Trim().Equals(""))
        {
            this.chkisLongName.Checked = true;

            EntityHTG_JC68 htReturn_JC68 = GetJC68(htReturn["OWNER_ID"].ToString().Trim());
            lblPrincipalName_L.Text = htReturn_JC68.LONG_NAME;//負責人-中文長姓名
            lblPrincipalName_PINYIN.Text = htReturn_JC68.PINYIN_NAME;//負責人-羅馬拼音
        }
        else
        {
            this.chkisLongName.Checked = false;
        }
        //負責人英文名
        if (htReturn.ContainsKey("OWNER_ENGLISH_NAME"))
        {
            lblPrincipalNameEN.Text = htReturn["OWNER_ENGLISH_NAME"].ToString();
        }
        //負責人生日
        if (htReturn.ContainsKey("OWNER_BIRTH_DATE"))
        {
            lblPrincipalBirth.Text = htReturn["OWNER_BIRTH_DATE"].ToString();
        }
        //身分證發證日期
        if (htReturn.ContainsKey("OWNER_ID_ISSUE_DATE"))
        {
            lblPrincipalIssueDate.Text = htReturn["OWNER_ID_ISSUE_DATE"].ToString();
        }
        //身分證發證地點
        if (htReturn.ContainsKey("OWNER_ID_ISSUE_PLACE"))
        {
            lblPrincipalIssuePlace.Text = htReturn["OWNER_ID_ISSUE_PLACE"].ToString();
        }
        //身分證換補領別
        if (htReturn.ContainsKey("OWNER_ID_REPLACE_TYPE"))
        {
            lblReplaceType.Text = htReturn["OWNER_ID_REPLACE_TYPE"].ToString().Trim();
        }
        //負責人國籍
        if (htReturn.ContainsKey("OWNER_NATION"))
        {
            lblPrincipalCountryCode.Text = htReturn["OWNER_NATION"].ToString();
        }
        //護照號碼
        if (htReturn.ContainsKey("PASSPORT"))
        {
            lblPrincipalPassportNo.Text = htReturn["PASSPORT"].ToString();
        }
        //護照效期
        if (htReturn.ContainsKey("PASSPORT_EXP_DATE"))
        {
            lblPrincipalPassportExpdt.Text = htReturn["PASSPORT_EXP_DATE"].ToString();
        }
        //居留證號
        if (htReturn.ContainsKey("RESIDENT_NO"))
        {
            lblPrincipalResidentNo.Text = htReturn["RESIDENT_NO"].ToString();
        }
        //居留證效期
        if (htReturn.ContainsKey("RESIDENT_EXP_DATE"))
        {
            lblPrincipalResidentExpdt.Text = htReturn["RESIDENT_EXP_DATE"].ToString();
        }
        //負責人聯絡電話
        if (htReturn.ContainsKey("OWNER_TEL"))
        {
            lblOWNER_TEL.Text = htReturn["OWNER_TEL"].ToString();
        }
        //負責人戶籍地址
        if (htReturn.ContainsKey("OWNER_CITY"))
        {
            lblOWNER_ADDR.Text = htReturn["OWNER_CITY"].ToString() + htReturn["OWNER_ADDR1"].ToString() + htReturn["OWNER_ADDR2"].ToString();
        }
        /*20191101 修改：決議不能異動帳號相關資訊 by Peggy
        //銀行名稱
        if (htReturn.ContainsKey("DDA_BANK_NAME"))
        {
            lblBANK_NAME.Text = htReturn["DDA_BANK_NAME"].ToString();
        }
        //分行名稱
        if (htReturn.ContainsKey("DDA_BANK_BRANCH"))
        {
            lblBANK_BRANCH.Text = htReturn["DDA_BANK_BRANCH"].ToString();
        }
        //戶名
        if (htReturn.ContainsKey("DDA_ACCT_NAME"))
        {
            lblBANK_ACCT_NAME.Text = htReturn["DDA_ACCT_NAME"].ToString();
        }
        //檢碼
        if (htReturn.ContainsKey("CHECK_CODE"))
        {
            lblCHECK_CODE.Text = htReturn["CHECK_CODE"].ToString();
        }
        //帳號
        if (htReturn.ContainsKey("DDA_BANK_ACCT"))
        {
            if (htReturn["DDA_BANK_ACCT"].ToString().Trim().Length > 4)
            {
                lblBANK_ACCT.Text = htReturn["DDA_BANK_ACCT"].ToString().Trim().Substring(0, 3) + "  -  " + htReturn["DDA_BANK_ACCT"].ToString().Trim().Substring(4);
            }
            else
            {
                lblBANK_ACCT.Text = htReturn["DDA_BANK_ACCT"].ToString();
            }            
        }
        */
        //歸檔編號
        if (htReturn.ContainsKey("ARCHIVE_NO"))
        {
            lblARCHIVE_NO.Text = htReturn["ARCHIVE_NO"].ToString();
        }

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
    
    /// <summary>
    /// 清空文本
    /// </summary>
    private void ClearText()
    {
        #region
        //總公司統一編號
        lblHEAD_CORP_NO.Text = "";
        //行業別
        lblCORP_MCC.Text = "";
        //設立日期
        lblCORP_ESTABLISH.Text = "";
        //法律形式
        lblCORP_Organization.Text = "";
        //註冊國籍
        lblCORP_CountryCode.Text = "";
        //州別
        lblCORP_CountryStateCode.Text = "";
        //總公司登記名稱
        lblREG_NAME_CH.Text = "";
        //總公司英文名稱
        lblREG_NAME_EN.Text = "";
        //登記地址
        lblREG_ADDR.Text = "";
        //登記電話
        lblCORP_TEL.Text = "";
        //負責人
        lblPrincipalNameCH.Text = "";
        //負責人ID
        lblPrincipalIDNo.Text = "";
        //長姓名
        this.chkisLongName.Checked = false;
        lblPrincipalName_L.Text = "";
        lblPrincipalName_PINYIN.Text = "";
        //負責人英文名
        lblPrincipalNameEN.Text = "";
        //負責人生日
        lblPrincipalBirth.Text = "";
        //身分證發證日期
        lblPrincipalIssueDate.Text = "";
        //身分證發證地點
        lblPrincipalIssuePlace.Text = "";
        //身分證換補領別
        lblReplaceType.Text = "";
        //負責人國籍
        lblPrincipalCountryCode.Text = "";
        //護照號碼
        lblPrincipalPassportNo.Text = "";
        //護照效期
        lblPrincipalPassportExpdt.Text = "";
        //居留證號
        lblPrincipalResidentNo.Text = "";
        //居留證效期
        lblPrincipalResidentExpdt.Text = "";
        //負責人聯絡電話
        lblOWNER_TEL.Text = "";
        //負責人戶籍地址
        lblOWNER_ADDR.Text = "";
        /*
        //銀行名稱
        lblBANK_NAME.Text = "";
        //分行名稱
        lblBANK_BRANCH.Text = "";
        //戶名
        lblBANK_ACCT_NAME.Text = "";
        //檢碼
        lblCHECK_CODE.Text = "";
        //帳號
        lblBANK_ACCT.Text = "";
        */
        //歸檔編號
        lblARCHIVE_NO.Text = ""; 

        #endregion
    }

    //依據長姓名FLAG至JC68取值
    private EntityHTG_JC68 GetJC68(string strID)
    {
        EntityHTG_JC68 _result = new EntityHTG_JC68();
        using (BRHTG_JC68 obj = new BRHTG_JC68("P010103030001"))
        {
            EntityHTG_JC68 _data = new EntityHTG_JC68();
            
            _data.ID = strID;
            _result = obj.getData(_data, eAgentInfo, "1");
        }
        return _result;
    }
}