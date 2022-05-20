//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：特店資料更改- PCAM(PCAM修改)
//*  創建日期：2009/11/26
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
//20160525 (U) by Tank, 新增9~11費率上傳
using System;
using System.Collections;
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Message;
using Framework.Common.Logging;
using Framework.WebControls;
using Framework.Data.OM.Collections;

public partial class P010104070001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    private string involve = "78360020,78360021,78360023,78360024,78360025,78360026,78360027,78360028,78360029";//自然人
    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CommonFunction.SetControlsEnabled(pnlText, false);
            this.btnPcmm.Enabled = false;
            this.txtShopId.Enabled = true;
            this.btnPcim.Enabled = true;
            base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
            SetFeeDisable();
        }
        base.strHostMsg += "";
        base.strClientMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合        
    }

    /// 作者 趙呂梁


    /// 創建日期：2009/11/26
    /// 修改日期：2009/11/26
    /// <summary>
    /// 查詢事件
    /// </summary>
    protected void btnPcim_Click(object sender, EventArgs e)
    {

        string strShopId = this.txtShopId.Text.Trim();
        CommonFunction.SetControlsEnabled(pnlText, false);//*	清空網頁中的所有輸入欄位

        this.btnPcmm.Enabled = false;
        this.txtShopId.Enabled = true;
        this.btnPcim.Enabled = true;
        this.txtShopId.Text = strShopId;
        this.txtShopIdHide.Text = strShopId;
        base.strClientMsg = MessageHelper.GetMessage("01_01040302_001");

        Hashtable htInput = new Hashtable();
        htInput.Add("ORGN", "822");//*提交< ORGANIZATION>
        htInput.Add("ACCT", this.txtShopId.Text.Trim());//*提交<商店代號>
        htInput.Add("FUNCTION_CODE", "I");

        Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCHR, htInput, false, "1", eAgentInfo);
        htReturn["ORGN"] = htInput["ORGN"];//* for_xml_test  模擬環境測試，正式環境可以不用賦值


        htReturn["ACCT"] = htInput["ACCT"];//* for_xml_test  模擬環境測試，正式環境可以不用賦值


        if (!htReturn.Contains("HtgMsg"))
        {
            CommonFunction.SetControlsEnabled(pnlText, true);
            this.txtShopId.Text = strShopId;
            this.txtShopIdHide.Text = strShopId;
            SetValues(htReturn);
            ViewState["HtgInfo"] = htReturn;
            //SetFeeDisable();
            base.strClientMsg = MessageHelper.GetMessage("01_01040302_003");
            base.strHostMsg += htReturn["HtgSuccess"].ToString();//*主機返回成功訊息  
            this.btnPcmm.Enabled = true;
        }
        else
        {
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
            etMstType = eMstType.Select;
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

            //*查詢主機資料失敗
            if (htReturn["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            {
                base.strHostMsg += htReturn["HtgMsg"].ToString();
                base.strClientMsg = MessageHelper.GetMessage("01_01040302_002");
            }
            else
            {
                base.strClientMsg = htReturn["HtgMsg"].ToString();
            }
        }
        base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
    }

    /// 作者 趙呂梁


    /// 創建日期：2009/11/26
    /// 修改日期：2009/11/26
    /// 修改紀錄：20210416_Ares_Stanley-調整異動成功後才需要將更新按鈕禁用
    /// <summary>
    /// 更新事件
    /// </summary>
    protected void btnPcmm_Click(object sender, EventArgs e)
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

        if (UploadHtg())//*若異動全部主機資料成功
        {
            base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
            CommonFunction.SetControlsEnabled(pnlText, false);//*	清空網頁中的所有輸入欄位        
            this.txtShopId.Enabled = true;
            this.btnPcim.Enabled = true;
            this.btnPcmm.Enabled = false; //確定異動全部主機資料成功後再將按鈕Disabled
        }
        else
        {
            this.btnPcmm.Enabled = true;
        }
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/11/26
    /// 修改日期：2009/11/26
    /// <summary>
    /// 城市資料檢核事件
    /// </summary>
    protected void btnCheck_Click(object sender, EventArgs e)
    {
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
                base.sbRegScript.Append("if(confirm('" + MessageHelper.GetMessage("01_01040302_010") + "')) {document.getElementById('txtAddress1').focus();}");
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
    /// 作者 趙呂梁

    /// 創建日期：2009/11/26
    /// 修改日期：2009/11/26
    /// <summary>
    /// 設置【費率01】~【費率08】不可用
    /// </summary>
    private void SetFeeDisable()
    {
        this.txtDisRate1.Enabled = false;
        this.txtDisRate2.Enabled = false;
        this.txtDisRate3.Enabled = false;
        this.txtDisRate4.Enabled = false;
        this.txtDisRate5.Enabled = false;
        this.txtDisRate6.Enabled = false;
        this.txtDisRate7.Enabled = false;
        this.txtDisRate8.Enabled = false;
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/11/26
    /// 修改日期：2009/11/26
    /// <summary>
    /// 為頁面欄位賦值

    /// </summary>
    /// <param name="htReturn">EntitySHOP_PCAM</param>
    private void SetValues(Hashtable htReturn)
    {
        #region
        try
        {
            this.txtMerchantName.Text = htReturn["MERCHANT_NAME"].ToString();//*登記名稱

            //*統一編號
            if (!string.IsNullOrEmpty(htReturn["DB_ACCT_NMBR"].ToString()))
            {
                if (htReturn["DB_ACCT_NMBR"].ToString().Length >= 9)
                {
                    this.txtCardNo1.Text = htReturn["DB_ACCT_NMBR"].ToString().Substring(1, 8);
                    this.txtCardNo2.Text = htReturn["DB_ACCT_NMBR"].ToString().Remove(0, 9);
                }
                else
                {
                    this.txtCardNo1.Text = htReturn["DB_ACCT_NMBR"].ToString();
                    this.txtCardNo2.Text = "";
                }
            }
            else
            {
                this.txtCardNo1.Text = "";
                this.txtCardNo2.Text = "";
            }

            this.txtEngName.Text = htReturn["ID_NAME"].ToString();//*英文名稱
            this.txtDbaCity.Text = htReturn["ID_CITY"].ToString();//*城市
            this.txtZip.Text = htReturn["ZIP_CODE"].ToString();//*區域編號


            this.txtAddress1.Text = htReturn["ADDRESS1"].ToString();//*【帳單地址】(1)
            this.txtAddress2.Text = htReturn["ADDRESS2"].ToString();//*【帳單地址】(2)
            //this.txtAddress3.Text = htReturn["ADDRESS3"].ToString();//*【帳單地址】(3)

            //*連絡電話
            this.txtPhone1.Text = htReturn["PHONE_NMBR1"].ToString();
            this.txtPhone2.Text = htReturn["PHONE_NMBR2"].ToString();
            this.txtPhone3.Text = htReturn["PHONE_NMBR3"].ToString();

            this.txtOwner.Text = htReturn["CONTACT"].ToString();//*負責人英文名
            this.txtImp1.Text = htReturn["NBR_IMPRINTER1"].ToString();//*IMP1
            this.txtPos1.Text = htReturn["NBR_IMPRINTER2"].ToString();//*POS1
            this.txtEdc1.Text = htReturn["NBR_IMPRINTER3"].ToString();//*EDC1
            this.txtImp2.Text = htReturn["NBR_POS_DEV1"].ToString();//*IMP2
            this.txtPos2.Text = htReturn["NBR_POS_DEV2"].ToString();//*POS2
            this.txtEdc2.Text = htReturn["NBR_POS_DEV3"].ToString();//*EDC2
            this.txtSalesName.Text = htReturn["MERCH_MEMO"].ToString();//*推廣姓名
            this.txtAgentBank.Text = htReturn["AGENT_BANK"].ToString();//*推廣代碼
            this.txtType.Text = htReturn["MERCH_TYPE"].ToString();//*Type
            this.txtStore.Text = htReturn["CHAIN_STORE"].ToString();//*STORE
            this.txtMerch.Text = htReturn["CHAIN_MER_NBR"].ToString();//*MERCH
            this.txtLevel.Text = htReturn["CHAIN_MER_LEVEL"].ToString();//*LEVEL
            this.txtSource.Text = htReturn["OFFICER_ID"].ToString();//*SOURE
            this.txtMcc.Text = htReturn["MCC"].ToString();//*MCC
            this.txtAve.Text = htReturn["PROJ_AVG_TKT"].ToString();//*AVE
            this.txtMonth.Text = htReturn["PROJ_MTH_VOLUME"].ToString();//*Month
            this.txtBranch.Text = htReturn["BRANCH"].ToString();//*BRANCH
            this.txtInt.Text = htReturn["VISA_INTCHG_FLAG"].ToString();//*INT
            this.txtMail.Text = htReturn["VISA_MAIL_PHONE_IND"].ToString();//*MAIL
            this.txtPosCa.Text = htReturn["POS_CAP"].ToString();//*POS_CA
            this.txtPosMo.Text = htReturn["POS_MODE"].ToString();//*POS_MO
            this.txtCH.Text = htReturn["CH_ID"].ToString();//*C/H
            this.txtMC.Text = htReturn["MC_INTCHG_FLAG"].ToString();//*M/C
            this.txtFileNo.Text = htReturn["ROUTE_TRANSIT"].ToString();//*歸檔編號
            this.txtHOLDSTMT.Text = htReturn["HOLD_STMT_FLAG"].ToString();//*Hold STMT

            //*帳號
            this.txtDdaNo1.Text = htReturn["USER_DATA1"].ToString();
            this.txtDdaNo2.Text = htReturn["USER_DATA2"].ToString();

            //*Status-01-08
            this.txtStatus1.Text = htReturn["CARD_STATUS1"].ToString();
            this.txtStatus2.Text = htReturn["CARD_STATUS2"].ToString();
            this.txtStatus3.Text = htReturn["CARD_STATUS3"].ToString();
            this.txtStatus4.Text = htReturn["CARD_STATUS4"].ToString();
            this.txtStatus5.Text = htReturn["CARD_STATUS5"].ToString();
            this.txtStatus6.Text = htReturn["CARD_STATUS6"].ToString();
            this.txtStatus7.Text = htReturn["CARD_STATUS7"].ToString();
            this.txtStatus8.Text = htReturn["CARD_STATUS8"].ToString();

            //20160525 (U) by Tank
            this.txtStatus9.Text = htReturn["CARD_STATUS9"].ToString();
            this.txtStatus10.Text = htReturn["CARD_STATUS10"].ToString();
            this.txtStatus11.Text = htReturn["CARD_STATUS11"].ToString();

            //*費率01-08
            GetFeeValue(this.txtStatus1.Text, htReturn["CARD_DISC_RATE1"].ToString(), txtDisRate1);
            GetFeeValue(this.txtStatus2.Text, htReturn["CARD_DISC_RATE2"].ToString(), txtDisRate2);
            GetFeeValue(this.txtStatus3.Text, htReturn["CARD_DISC_RATE3"].ToString(), txtDisRate3);
            GetFeeValue(this.txtStatus4.Text, htReturn["CARD_DISC_RATE4"].ToString(), txtDisRate4);
            GetFeeValue(this.txtStatus5.Text, htReturn["CARD_DISC_RATE5"].ToString(), txtDisRate5);
            GetFeeValue(this.txtStatus6.Text, htReturn["CARD_DISC_RATE6"].ToString(), txtDisRate6);
            GetFeeValue(this.txtStatus7.Text, htReturn["CARD_DISC_RATE7"].ToString(), txtDisRate7);
            GetFeeValue(this.txtStatus8.Text, htReturn["CARD_DISC_RATE8"].ToString(), txtDisRate8);

            //20160525 (U) by Tank
            GetFeeValue(this.txtStatus9.Text, htReturn["CARD_DISC_RATE9"].ToString(), txtDisRate9);
            GetFeeValue(this.txtStatus10.Text, htReturn["CARD_DISC_RATE10"].ToString(), txtDisRate10);
            GetFeeValue(this.txtStatus11.Text, htReturn["CARD_DISC_RATE11"].ToString(), txtDisRate11);

            // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis            
            if (htReturn.ContainsKey("LAST_UPD_BRANCH"))
            {
                this.txtLAST_UPD_BRANCH.Text = htReturn["LAST_UPD_BRANCH"].ToString();// 資料最後異動分行
            }
            if (htReturn.ContainsKey("LAST_UPD_CHECKER"))
            {
                this.txtLAST_UPD_CHECKER.Text = htReturn["LAST_UPD_CHECKER"].ToString();// 資料最後異動-CHECKER
            }
            if (htReturn.ContainsKey("LAST_UPD_MAKER"))
            {
                this.txtLAST_UPD_MAKER.Text = htReturn["LAST_UPD_MAKER"].ToString();// 資料最後異動-MAKER
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
        }
        #endregion
    }

    /// 作者 趙呂梁


    /// 創建日期：2009/11/27
    /// 修改日期：2009/11/27
    /// <summary>
    /// 得到資料庫中{費率01}~{費率08}顯示在網頁中的值


    /// </summary>
    /// <param name="strStatusValue">主機Status值</param>
    /// <param name="strDataValue">主機費率值</param>
    /// <param name="txtBox">費率對應的TextBox</param>
    private void GetFeeValue(string strStatusValue, string strFeeValue, CustTextBox txtBox)
    {
        if (strStatusValue != "0")
        {
            if (!string.IsNullOrEmpty(strFeeValue) && strFeeValue != "0")
            {
                //float intFeeValue = float.Parse(strFeeValue) / 1000;
                //txtBox.Text = intFeeValue.ToString();  
                txtBox.Text = strFeeValue;
            }
            else
            {
                txtBox.Text = "";
            }
            txtBox.Enabled = true;
        }
        else
        {
            txtBox.Enabled = false;
        }
    }

    /// 作者 趙呂梁


    /// 創建日期：2009/11/27
    /// 修改日期：2009/11/27
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


    /// 創建日期：2009/11/27
    /// 修改日期：2009/11/27
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


    /// 創建日期：2009/11/27
    /// 修改日期：2009/11/27
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


    /// 創建日期：2009/11/27
    /// 修改日期：2009/11/27
    /// <summary>
    /// 得到異動主機信息
    /// </summary>
    /// <param name="htInput">主機返回信息的Hashtable</param>
    /// <param name="blnType">上傳Status,費率選擇類型</param>
    /// <returns>Hashtable</returns>
    private Hashtable GetUploadInfo(Hashtable htInput, bool blnType)
    {
        try
        {
            if (blnType)
            {
                htInput["ORGN"] = "822";//*<ORGANIZATION>
            }
            else
            {
                htInput["ORGN"] = "900";//*<ORGANIZATION>
            }
            htInput["ACCT"] = this.txtShopId.Text.Trim().ToUpper();//*<商店代號>
            htInput["FUNCTION_CODE"] = "M";
            htInput["MESSAGE_TYPE"] = "";
            htInput["MESSAGE_CHI"] = "";
            htInput["ID_NAME"] = this.txtEngName.Text.Trim();//*<英文名稱>
            htInput["ID_CITY"] = this.txtDbaCity.Text.Trim().ToUpper();//*<City>
            htInput["CONTACT"] = this.txtOwner.Text.Trim();//*<負責人英文名>
            htInput["MERCH_MEMO"] = this.txtSalesName.Text.Trim().ToUpper();//*<推廣姓名>
            htInput["PHONE_NMBR1"] = this.txtPhone1.Text.PadRight(3, ' ');//*<連絡電話>(1)（不足3碼右補空格）
            htInput["PHONE_NMBR2"] = this.txtPhone2.Text.Trim();//*<連絡電話>(2)
            htInput["PHONE_NMBR3"] = this.txtPhone3.Text.Trim();//*<連絡電話>(3)
            htInput["OFFICER_ID"] = this.txtSource.Text.Trim().ToUpper();//*<SOURCE>
            htInput["DB_ACCT_NMBR"] = "0" + this.txtCardNo1.Text.Trim().ToUpper() + this.txtCardNo2.Text.Trim().ToUpper();//*<統一編號>(1)+ <統一編號>(2)
            htInput["MERCH_TYPE"] = this.txtType.Text.Trim().ToUpper();//*<Type>


            htInput["CHAIN_STORE"] = GetValue(this.txtStore.Text.Trim().ToUpper(), "00000000", 8, '0');//*<STORE>

            htInput["MERCHANT_NAME"] = this.txtMerchantName.Text.Trim();//*<登記名稱>
            htInput["HOLD_STMT_FLAG"] = GetValue(this.txtHOLDSTMT.Text.Trim().ToUpper(), "0"); //*<Hold STMT>
            htInput["ADDRESS1"] = this.txtAddress1.Text.Trim().ToUpper();//*<帳單地址>(1)
            htInput["ADDRESS2"] = this.txtAddress2.Text.Trim().ToUpper();//*<帳單地址>(2)
            htInput["ADDRESS3"] = "";//*<帳單地址>(3)
            htInput["ZIP_CODE"] = this.txtZip.Text.Trim().ToUpper();//*<區域編號>    
            htInput["MCC"] = GetValue(this.txtMcc.Text.Trim().ToUpper(), "0000", 4, '0');//*<MCC>

            htInput["CHAIN_MER_NBR"] = GetValue(this.txtMerch.Text.Trim().ToUpper(), "000000000");//*<MERCH>
            htInput["CHAIN_MER_LEVEL"] = GetValue(this.txtLevel.Text.Trim().ToUpper(), "0");//*<LEVEL>
            htInput["NBR_IMPRINTER1"] = GetValue(this.txtImp1.Text.Trim().ToUpper(), "000");//*<IMP1>
            htInput["NBR_IMPRINTER2"] = GetValue(this.txtPos1.Text.Trim().ToUpper(), "000");//*<POS1>
            htInput["NBR_IMPRINTER3"] = GetValue(this.txtEdc1.Text.Trim().ToUpper(), "000");//*<EDC1>        
            htInput["NBR_POS_DEV1"] = GetValue(this.txtImp2.Text.Trim().ToUpper(), "000");//*<IMP2>
            htInput["NBR_POS_DEV2"] = GetValue(this.txtPos2.Text.Trim().ToUpper(), "000");//*<POS2>
            htInput["NBR_POS_DEV3"] = GetValue(this.txtEdc2.Text.Trim().ToUpper(), "000");//*<EDC2>
            htInput["PROJ_AVG_TKT"] = GetValue(this.txtAve.Text.Trim().ToUpper(), "000");//*<AVE>
            htInput["PROJ_MTH_VOLUME"] = GetValue(this.txtMonth.Text.Trim().ToUpper(), "000000000");//*<Month>
            htInput["AGENT_BANK"] = GetValue(this.txtAgentBank.Text.Trim().ToUpper(), "00000", 5, '0');//*<推廣代碼>

            htInput["BRANCH"] = this.txtBranch.Text.Trim().ToUpper();//*<BRANCH>
            htInput["ROUTE_TRANSIT"] = this.txtFileNo.Text.Trim().ToUpper();//*<歸檔編號>

            //20200811-RQ-2020-021027-001 bug修正，邏輯應與PCAM鍵檔時相同 by Peggy
            //if (this.txtMerch.Text.Trim() == "" && this.txtLevel.Text.Trim() == "")
            if ((!(this.txtMerch.Text.Trim() == "" || this.txtMerch.Text.Trim() == "0")) && (!(this.txtLevel.Text.Trim() == "" || this.txtLevel.Text.Trim() == "0")))
            {
                htInput["CHAIN_STMT_IND"] = "1";//*<STATEMENT>
                htInput["CHAIN_REPRT_IND"] = "1";//*<REPORTING>
                htInput["CHAIN_SETT_IND"] = "0";//*<SETTLEMENT>
                htInput["CHAIN_DISC_IND"] = "1";//*< DISCOUNT>
                htInput["CHAIN_FEES_IND"] = "1";//*< FEES>
                htInput["CHAIN_DD_IND"] = "0";//*<DISC/DEE DD>
            }
            else
            {
                htInput["CHAIN_STMT_IND"] = "0";//*<STATEMENT>
                htInput["CHAIN_REPRT_IND"] = "0";//*<REPORTING>
                htInput["CHAIN_SETT_IND"] = "0";//*<SETTLEMENT>
                htInput["CHAIN_DISC_IND"] = "0";//*< DISCOUNT>
                htInput["CHAIN_FEES_IND"] = "0";//*< FEES>
                htInput["CHAIN_DD_IND"] = "0";//*<DISC/DEE DD>
            }


            htInput["USER_DATA1"] = this.txtDdaNo1.Text.Trim().ToUpper();//*<帳號1>
            htInput["USER_DATA2"] = this.txtDdaNo2.Text.Trim().ToUpper();//*<帳號2>

            htInput["VISA_INTCHG_FLAG"] = this.txtInt.Text.Trim().ToUpper();//*<INT>
            htInput["VISA_MAIL_PHONE_IND"] = this.txtMail.Text.Trim().ToUpper();//*<MAIL>
            htInput["POS_CAP"] = this.txtPosCa.Text.Trim().ToUpper();//*<POS CA>
            htInput["POS_MODE"] = this.txtPosMo.Text.Trim().ToUpper();//*<POS MO>
            htInput["CH_ID"] = this.txtCH.Text.Trim().ToUpper();//*<C/H>
            htInput["MC_INTCHG_FLAG"] = this.txtMC.Text.Trim().ToUpper();//*<M/C>

            
            //若為分期店 (blnType=false)則 CARD_STATUS 及 RATE 不 Update
            if (blnType)
            {
                htInput["CARD_STATUS1"] = GetValue(this.txtStatus1.Text.Trim().ToUpper(), "0");//*<Status-01>
                htInput["CARD_STATUS2"] = GetValue(this.txtStatus2.Text.Trim().ToUpper(), "0");//*<Status-02>
                htInput["CARD_STATUS3"] = GetValue(this.txtStatus3.Text.Trim().ToUpper(), "0");//*<Status-03>
                htInput["CARD_STATUS4"] = GetValue(this.txtStatus4.Text.Trim().ToUpper(), "0");//*<Status-04>
                htInput["CARD_STATUS5"] = GetValue(this.txtStatus5.Text.Trim().ToUpper(), "0");//*<Status-05>
                htInput["CARD_STATUS6"] = GetValue(this.txtStatus6.Text.Trim().ToUpper(), "0");//*<Status-06>
                htInput["CARD_STATUS7"] = GetValue(this.txtStatus7.Text.Trim().ToUpper(), "0");//*<Status-07>
                htInput["CARD_STATUS8"] = GetValue(this.txtStatus8.Text.Trim().ToUpper(), "0");//*<Status-08>

                //20160525 (U) by Tank
                htInput["CARD_STATUS9"] = GetValue(this.txtStatus9.Text.Trim().ToUpper(), "0");//*<Status-09>
                htInput["CARD_STATUS10"] = GetValue(this.txtStatus10.Text.Trim().ToUpper(), "0");//*<Status-10>
                htInput["CARD_STATUS11"] = GetValue(this.txtStatus11.Text.Trim().ToUpper(), "0");//*<Status-11>

                //*<費率>
                htInput["CARD_DISC_RATE1"] = this.txtDisRate1.Text.Trim();
                htInput["CARD_DISC_RATE2"] = this.txtDisRate2.Text.Trim();
                htInput["CARD_DISC_RATE3"] = this.txtDisRate3.Text.Trim();
                htInput["CARD_DISC_RATE4"] = this.txtDisRate4.Text.Trim();
                htInput["CARD_DISC_RATE5"] = this.txtDisRate5.Text.Trim();
                htInput["CARD_DISC_RATE6"] = this.txtDisRate6.Text.Trim();
                htInput["CARD_DISC_RATE7"] = this.txtDisRate7.Text.Trim();
                htInput["CARD_DISC_RATE8"] = this.txtDisRate8.Text.Trim();
                
                //20160525 (U) by Tank
                htInput["CARD_DISC_RATE9"] = this.txtDisRate9.Text.Trim();
                htInput["CARD_DISC_RATE10"] = this.txtDisRate10.Text.Trim();
                htInput["CARD_DISC_RATE11"] = this.txtDisRate11.Text.Trim();
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


    /// 創建日期：2009/11/27
    /// 修改日期：2009/11/27
    /// <summary>
    /// 異動主機資料
    /// </summary>
    private bool UploadHtg()
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        try
        {
            Hashtable htInput = new Hashtable();
            CommonFunction.GetViewStateHt((Hashtable)ViewState["HtgInfo"], ref htInput);
            //20210527 EOS_AML(NOVA) by Ares Dennis
            Hashtable htMainFrame = new Hashtable();//主機資料 於異動紀錄比對用
            CommonFunction.GetViewStateHt((Hashtable)ViewState["HtgInfo"], ref htMainFrame);
            //*若【商店代號】第3碼不為5
            if (this.txtShopId.Text.Trim().Length < 3 || this.txtShopId.Text.Trim().Substring(2, 1) != "5")
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01040302_004");
                Hashtable htInputOne = GetUploadInfo(htInput, true);//得到PCMM P4A Submit異動主機資料

                if (htInputOne == null)
                {
                    return false;
                }

                Hashtable htResultOne = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCHR, htInputOne, false, "2", eAgentInfo);
                //有查出主機資料(已有資料) 或 P4A_JCGQ:0124 無此筆查詢資料請重新輸入(新的一筆資料)，比對欄位是否異動
                if (!htResultOne.Contains("HtgMsg") || (htResultOne["MESSAGE_TYPE"] != null && htResultOne["MESSAGE_TYPE"].ToString().Trim() == "0124"))//*若異動主機資料成功
                {
                    // 20210527 EOS_AML(NOVA) by Ares Dennis
                    #region 異動記錄需報送AML                                    
                    bool isChanged = false;                    
                    #region 檢核欄位是否異動
                    compareForAMLCheckLog(htMainFrame, htInputOne, "ID_NAME", ref isChanged);// 英文名稱
                    compareForAMLCheckLog(htMainFrame, htInputOne, "CONTACT", ref isChanged);// 負責人英文名
                    compareForAMLCheckLog(htMainFrame, htInputOne, "PHONE_NMBR1", ref isChanged);// 連絡電話
                    compareForAMLCheckLog(htMainFrame, htInputOne, "PHONE_NMBR2", ref isChanged);// 連絡電話
                    compareForAMLCheckLog(htMainFrame, htInputOne, "PHONE_NMBR3", ref isChanged);// 連絡電話                    
                    #endregion
                    if (isChanged)
                    {
                        #region 查詢JCGQ
                        Hashtable htP4A_JCGQ = new Hashtable();
                        htP4A_JCGQ.Add("FUNCTION_CODE", "I");
                        htP4A_JCGQ.Add("CORP_NO", this.txtCardNo1.Text.Trim());// 統一編號1
                        htP4A_JCGQ.Add("CORP_SEQ", this.txtCardNo2.Text.Trim());// 序號
                        Hashtable htP4A_JCGQ2 = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGQ, htP4A_JCGQ, false, "21", eAgentInfo);
                        #endregion

                        EntityAML_CHECKLOG eAMLCheckLog = new EntityAML_CHECKLOG();
                        //eAMLCheckLog.CORP_NO = htInputOne["DB_ACCT_NMBR"].ToString().Trim();
                        //20211202_Ares_Jack_統編為自然人用, 將CORP_NO 改帶負責人ID, 不符合則把 CORP_NO欄位帶成 HEADQUARTER_CORPNO(總公司統編)
                        if (involve.Contains(htP4A_JCGQ2["HEADQUARTER_CORPNO"].ToString().Trim()))
                            eAMLCheckLog.CORP_NO = htP4A_JCGQ2["OWNER_ID"].ToString().Trim();//負責人ID
                        else
                            eAMLCheckLog.CORP_NO = htP4A_JCGQ2["CORP_NO"].ToString().Trim();
                        eAMLCheckLog.MER_NO = htInputOne["ACCT"].ToString().Trim();
                        eAMLCheckLog.TRANS_ID = "CSIPJCHR";
                        eAMLCheckLog.LAST_UPD_BRANCH = txtLAST_UPD_BRANCH.Text.Trim();
                        eAMLCheckLog.LAST_UPD_CHECKER = txtLAST_UPD_CHECKER.Text.Trim();
                        eAMLCheckLog.LAST_UPD_MAKER = txtLAST_UPD_MAKER.Text.Trim();
                        eAMLCheckLog.MOD_USERID = eAgentInfo.agent_id;
                        eAMLCheckLog.MOD_DATE = DateTime.Now.ToString("yyyyMMdd");
                        eAMLCheckLog.MOD_TIME = DateTime.Now.ToString("HHmmss");

                        InsertAMLCheckLog(eAMLCheckLog);
                    }                    
                    #endregion

                    //if (!UpdateSend3270())//*更新資料庫

                    //{
                    //    base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    //}
                    base.strHostMsg += htResultOne["HtgSuccess"].ToString();//*主機返回成功訊息
                    base.strClientMsg += MessageHelper.GetMessage("01_01040302_006");
                    return true;
                }
                else
                {
                    //*異動主機資料失敗
                    if (htResultOne["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                    {
                        base.strHostMsg += htResultOne["HtgMsg"].ToString();
                        base.strClientMsg += MessageHelper.GetMessage("01_00000000_027");
                    }
                    else
                    {
                        base.strClientMsg += htResultOne["HtgMsg"].ToString();
                    }
                    return false;
                }
            }
            else//*若【商店代號】第3碼為5
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01040302_007");
                Hashtable htInputTwo = GetUploadInfo(htInput, true);//*得到PCMM P4A Submit異動主機資料
                Hashtable htResultTwo = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCHR, htInputTwo, false, "2", eAgentInfo);
                //20210824 EOS_AML(NOVA) by Ares Jack 若查無主機資料(新的一筆資料)也需比對異動欄位並紀錄到DB
                if (!htResultTwo.Contains("HtgMsg") || (htResultTwo["MESSAGE_TYPE"] != null && htResultTwo["MESSAGE_TYPE"].ToString().Trim() == "0124"))//*若異動主機資料成功，更新資料庫,進行分期店 P4 PCMM 作業
                {
                    // 20210527 EOS_AML(NOVA) by Ares Dennis
                    #region 異動記錄需報送AML                                    
                    bool isChanged = false;
                    #region 檢核欄位是否異動
                    compareForAMLCheckLog(htMainFrame, htInputTwo, "ID_NAME", ref isChanged);// 英文名稱
                    compareForAMLCheckLog(htMainFrame, htInputTwo, "CONTACT", ref isChanged);// 負責人英文名
                    compareForAMLCheckLog(htMainFrame, htInputTwo, "PHONE_NMBR1", ref isChanged);// 連絡電話
                    compareForAMLCheckLog(htMainFrame, htInputTwo, "PHONE_NMBR2", ref isChanged);// 連絡電話
                    compareForAMLCheckLog(htMainFrame, htInputTwo, "PHONE_NMBR3", ref isChanged);// 連絡電話                    
                    #endregion
                    if (isChanged)
                    {
                        #region 查詢JCGQ
                        Hashtable htP4A_JCGQ = new Hashtable();
                        htP4A_JCGQ.Add("FUNCTION_CODE", "I");
                        htP4A_JCGQ.Add("CORP_NO", this.txtCardNo1.Text.Trim());// 統一編號1
                        htP4A_JCGQ.Add("CORP_SEQ", this.txtCardNo2.Text.Trim());// 序號
                        Hashtable htP4A_JCGQ2 = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGQ, htP4A_JCGQ, false, "21", eAgentInfo);
                        #endregion

                        EntityAML_CHECKLOG eAMLCheckLog = new EntityAML_CHECKLOG();
                        //eAMLCheckLog.CORP_NO = htInputOne["DB_ACCT_NMBR"].ToString().Trim();
                        //20211202_Ares_Jack_統編為自然人用, 將CORP_NO 改帶負責人ID, 不符合則把 CORP_NO欄位帶成 HEADQUARTER_CORPNO(總公司統編)
                        if (involve.Contains(htP4A_JCGQ2["HEADQUARTER_CORPNO"].ToString().Trim()))
                            eAMLCheckLog.CORP_NO = htP4A_JCGQ2["OWNER_ID"].ToString().Trim();//負責人ID
                        else
                            eAMLCheckLog.CORP_NO = htP4A_JCGQ2["CORP_NO"].ToString().Trim() + htP4A_JCGQ2["CORP_SEQ"].ToString().Trim();//CORP_NO + CORP_SEQ

                        eAMLCheckLog.MER_NO = htInputTwo["ACCT"].ToString().Trim();
                        eAMLCheckLog.TRANS_ID = "CSIPJCHR";
                        eAMLCheckLog.LAST_UPD_BRANCH = txtLAST_UPD_BRANCH.Text.Trim();
                        eAMLCheckLog.LAST_UPD_CHECKER = txtLAST_UPD_CHECKER.Text.Trim();
                        eAMLCheckLog.LAST_UPD_MAKER = txtLAST_UPD_MAKER.Text.Trim();
                        eAMLCheckLog.MOD_USERID = eAgentInfo.agent_id;
                        eAMLCheckLog.MOD_DATE = DateTime.Now.ToString("yyyyMMdd");
                        eAMLCheckLog.MOD_TIME = DateTime.Now.ToString("HHmmss");

                        InsertAMLCheckLog(eAMLCheckLog);
                    }
                    #endregion

                    base.strHostMsg += htResultTwo["HtgSuccess"].ToString();//*主機返回成功訊息                
                }
                else
                {
                    //*異動主機資料失敗
                    if (htResultTwo["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                    {
                        base.strHostMsg += htResultTwo["HtgMsg"].ToString();
                        base.strClientMsg += MessageHelper.GetMessage("01_00000000_027");
                    }
                    else
                    {
                        base.strClientMsg += htResultTwo["HtgMsg"].ToString();
                    }
                    return false;
                }

                //*進行分期店 P4 PCMM 作業
                base.strClientMsg += MessageHelper.GetMessage("01_01040302_008");

                Hashtable htInputP4 = new Hashtable();
                htInputP4.Add("ORGN", "900");//*提交< ORGANIZATION>
                htInputP4.Add("ACCT", this.txtShopId.Text.Trim());//*提交<商店代號>
                htInputP4.Add("FUNCTION_CODE", "I");

                Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCHR, htInputP4, false, "1", eAgentInfo);
                htReturn["ORGN"] = htInputP4["ORGN"];//* for_xml_test  模擬環境測試，正式環境可以不用賦值

                htReturn["ACCT"] = htInputP4["ACCT"];//* for_xml_test  模擬環境測試，正式環境可以不用賦值


                if (!htReturn.Contains("HtgMsg"))
                {
                    Hashtable htInputP4_JCHR = GetUploadInfo(htReturn, false);//得到PCAM P4 Submit異動主機資料
                    Hashtable htResultP4_JCHR = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCHR, htInputP4_JCHR, false, "2", eAgentInfo);
                    //20210824 EOS_AML(NOVA) by Ares Jack 若查無主機資料(新的一筆資料)也需比對異動欄位並紀錄到DB
                    if (!htResultP4_JCHR.Contains("HtgMsg") || (htResultP4_JCHR["MESSAGE_TYPE"] != null && htResultP4_JCHR["MESSAGE_TYPE"].ToString().Trim() == "0124"))//*若異動主機資料成功
                    {
                        // 20210527 EOS_AML(NOVA) by Ares Dennis
                        #region 異動記錄需報送AML                                    
                        bool isChanged = false;
                        #region 檢核欄位是否異動
                        compareForAMLCheckLog(htReturn, htInputP4_JCHR, "ID_NAME", ref isChanged);// 英文名稱
                        compareForAMLCheckLog(htReturn, htInputP4_JCHR, "CONTACT", ref isChanged);// 負責人英文名
                        compareForAMLCheckLog(htReturn, htInputP4_JCHR, "PHONE_NMBR1", ref isChanged);// 連絡電話
                        compareForAMLCheckLog(htReturn, htInputP4_JCHR, "PHONE_NMBR2", ref isChanged);// 連絡電話
                        compareForAMLCheckLog(htReturn, htInputP4_JCHR, "PHONE_NMBR3", ref isChanged);// 連絡電話                    
                        #endregion
                        if (isChanged)
                        {
                            #region 查詢JCGQ
                            Hashtable htP4A_JCGQ = new Hashtable();
                            htP4A_JCGQ.Add("FUNCTION_CODE", "I");
                            htP4A_JCGQ.Add("CORP_NO", this.txtCardNo1.Text.Trim());// 統一編號1
                            htP4A_JCGQ.Add("CORP_SEQ", this.txtCardNo2.Text.Trim());// 序號
                            Hashtable htP4A_JCGQ2 = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGQ, htP4A_JCGQ, false, "21", eAgentInfo);
                            #endregion

                            EntityAML_CHECKLOG eAMLCheckLog = new EntityAML_CHECKLOG();
                            //eAMLCheckLog.CORP_NO = htInputOne["DB_ACCT_NMBR"].ToString().Trim();
                            //20211202_Ares_Jack_統編為自然人用, 將CORP_NO 改帶負責人ID, 不符合則把 CORP_NO欄位帶成 HEADQUARTER_CORPNO(總公司統編)
                            if (involve.Contains(htP4A_JCGQ2["HEADQUARTER_CORPNO"].ToString().Trim()))
                                eAMLCheckLog.CORP_NO = htP4A_JCGQ2["OWNER_ID"].ToString().Trim();//負責人ID
                            else
                                eAMLCheckLog.CORP_NO = htP4A_JCGQ2["CORP_NO"].ToString().Trim() + htP4A_JCGQ2["CORP_SEQ"].ToString().Trim();//CORP_NO + CORP_SEQ

                            eAMLCheckLog.MER_NO = htInputP4_JCHR["ACCT"].ToString().Trim();
                            eAMLCheckLog.TRANS_ID = "CSIPJCHR";
                            eAMLCheckLog.LAST_UPD_BRANCH = txtLAST_UPD_BRANCH.Text.Trim();
                            eAMLCheckLog.LAST_UPD_CHECKER = txtLAST_UPD_CHECKER.Text.Trim();
                            eAMLCheckLog.LAST_UPD_MAKER = txtLAST_UPD_MAKER.Text.Trim();
                            eAMLCheckLog.MOD_USERID = eAgentInfo.agent_id;
                            eAMLCheckLog.MOD_DATE = DateTime.Now.ToString("yyyyMMdd");
                            eAMLCheckLog.MOD_TIME = DateTime.Now.ToString("HHmmss");

                            InsertAMLCheckLog(eAMLCheckLog);
                        }
                        #endregion

                        //if (!UpdateSend3270())//*更新資料庫

                        //{
                        //    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                        //    {
                        //        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                        //    }
                        //}
                        base.strHostMsg += htResultP4_JCHR["HtgSuccess"].ToString();//*主機返回成功訊息
                        base.strClientMsg += MessageHelper.GetMessage("01_01040302_009");
                        return true;
                    }
                    else
                    {
                        //*異動主機資料失敗
                        if (htResultP4_JCHR["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                        {
                            base.strHostMsg += htResultP4_JCHR["HtgMsg"].ToString();
                            base.strClientMsg += MessageHelper.GetMessage("01_00000000_027");
                        }
                        else
                        {
                            base.strClientMsg += htResultP4_JCHR["HtgMsg"].ToString();
                        }
                        return false;
                    }
                }
                else
                {
                    //*查詢主機資料失敗
                    if (htReturn["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                    {
                        base.strHostMsg += htReturn["HtgMsg"].ToString();
                        base.strClientMsg += MessageHelper.GetMessage("01_01040302_011");
                    }
                    else
                    {
                        base.strClientMsg += htReturn["HtgMsg"].ToString();
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


    /// 創建日期：2009/11/27
    /// 修改日期：2009/11/27
    /// <summary>
    /// 更新資料庫Send3270欄位
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateSend3270()
    {
        EntitySHOP_PCAM eShopPcam = new EntitySHOP_PCAM();
        eShopPcam.send3270 = "Y";
        string[] strFields = { EntitySHOP_PCAM.M_send3270 };
        return BRSHOP_PCAM.Update(eShopPcam, this.txtShopId.Text.Trim(), strFields);
    }
    #endregion
}
