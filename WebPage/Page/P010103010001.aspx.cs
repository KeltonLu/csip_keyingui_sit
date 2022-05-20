//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：特店資料異動 一次鍵檔
//*  創建日期：2009/07/11
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*lvliangzhao     2010/05/14        20090019           於「特店資料異動-費率」下新增異動銀聯卡費率功能
//*******************************************************************
using System;
using System.Data;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using CSIPKeyInGUI.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.BusinessRules_new;
using Framework.Common.Message;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using System.Drawing;
using CSIPCommonModel.EntityLayer;
using System.Collections.Generic;

/// <summary>
/// 特店資料異動 一次鍵檔
/// </summary>
public partial class P010103010001 : PageBase
{
    #region 變數區
    /// <summary>
    /// 一KEY標識
    /// </summary>
    private string m_KeyFlag = "1";

    /// <summary>
    /// 基本資料異動功能畫面編號
    /// </summary>
    private string m_ShopBasicFlag = "1";

    /// <summary>
    /// 費率異動功能畫面編號
    /// </summary>
    private string m_ShopFeeFlag = "2";

    /// <summary>
    /// 帳號異動功能畫面編號
    /// </summary>
    private string m_ShopAccountFlag = "3";

    /// <summary>
    /// 解約作業功能畫面編號
    /// </summary>
    private string m_ShopCancelFlag = "4";

    /// <summary>
    /// 機器資料功能畫面編號
    /// </summary>
    private string m_ShopMaFlag = "5";

    /// <summary>
    /// 基本資料異動by統編功能畫面編號
    /// </summary>
    private string m_ShopBasicByTaxNoFlag = "6";

    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;

    private bool isNew = false;
    //20191023 修改：SOC所需資訊  by Peggy
    private structPageInfo sPageInfo;//*記錄網頁訊息

    //202012RC-20201112
    Hashtable hstPcmmP4A = new Hashtable();
    #endregion

    #region 事件區

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
            btnBasicData.BackColor = Color.FromArgb(255, 153, 0);

            SetControlsText();//*設置Lable文本
            //*add by lvliangzhao 20090019 20100325 start
            SetFeeLableValue();
            //*add by lvliangzhao 20090019 20100325 end
            EnabledControls(false);
            LoadDropDownList();
            SetDropStateCode();//20190923 add by Peggy
        }

        base.strClientMsg += ""; //* 【端末訊息】顯示內容清空
        base.strHostMsg += "";//* 【主機訊息】顯示內容清空
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//20191023 修改：SOC所需資訊  by Peggy
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 基礎資料異動Button事件
    /// </summary>
    protected void btnBasicData_Click(object sender, EventArgs e)
    {
        this.pnlBasic.Visible = true;
        this.pnlFee.Visible = false;
        this.pnlAccount.Visible = false;
        this.pnlCancelTask.Visible = false;
        this.pnlMachineData.Visible = false;
        this.chboxP4.Visible = true;

        this.tabMostly.Visible = true;//20190919 add by Peggy
        this.tabBasicData.Visible = false;//20190919 add by Peggy
        this.pnlBasicByTaxno.Visible = false;//20190919 add by Peggy
        ClearAll();

        //變更button的背景色
        btnBasicData.BackColor = Color.FromArgb(255, 153, 0);
        btnFee.BackColor = Color.Empty;
        btnAccounts.BackColor = Color.Empty;
        btnUnchainTask.BackColor = Color.Empty;
        btnMachineData.BackColor = Color.Empty;
        btnBasicDataByTaxno.BackColor = Color.Empty;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 費率異動Button事件
    /// </summary>
    protected void btnFee_Click(object sender, EventArgs e)
    {
        this.pnlBasic.Visible = false;
        this.pnlFee.Visible = true;
        this.pnlAccount.Visible = false;
        this.pnlCancelTask.Visible = false;
        this.pnlMachineData.Visible = false;
        this.chboxP4.Visible = false;

        this.tabMostly.Visible = true;//20190919 add by Peggy
        this.tabBasicData.Visible = false;//20190919 add by Peggy
        this.pnlBasicByTaxno.Visible = false;//20190919 add by Peggy
        ClearAll();

        //變更button的背景色
        btnBasicData.BackColor = Color.Empty;
        btnFee.BackColor = Color.FromArgb(255, 153, 0);
        btnAccounts.BackColor = Color.Empty;
        btnUnchainTask.BackColor = Color.Empty;
        btnMachineData.BackColor = Color.Empty;
        btnBasicDataByTaxno.BackColor = Color.Empty;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 帳號異動Button事件
    /// </summary>
    protected void btnAccounts_Click(object sender, EventArgs e)
    {
        this.pnlBasic.Visible = false;
        this.pnlFee.Visible = false;
        this.pnlAccount.Visible = true;
        this.pnlCancelTask.Visible = false;
        this.pnlMachineData.Visible = false;
        this.chboxP4.Visible = true;

        this.tabMostly.Visible = true;//20190919 add by Peggy
        this.tabBasicData.Visible = false;//20190919 add by Peggy
        this.pnlBasicByTaxno.Visible = false;//20190919 add by Peggy
        ClearAll();

        //變更button的背景色
        btnBasicData.BackColor = Color.Empty;
        btnFee.BackColor = Color.Empty;
        btnAccounts.BackColor = Color.FromArgb(255, 153, 0);
        btnUnchainTask.BackColor = Color.Empty;
        btnMachineData.BackColor = Color.Empty;
        btnBasicDataByTaxno.BackColor = Color.Empty;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 機器資料Button事件
    /// </summary>
    protected void btnMachineData_Click(object sender, EventArgs e)
    {
        this.pnlBasic.Visible = false;
        this.pnlFee.Visible = false;
        this.pnlAccount.Visible = false;
        this.pnlCancelTask.Visible = false;
        this.pnlMachineData.Visible = true;
        this.chboxP4.Visible = false;

        this.tabMostly.Visible = true;//20190919 add by Peggy
        this.tabBasicData.Visible = false;//20190919 add by Peggy
        this.pnlBasicByTaxno.Visible = false;//20190919 add by Peggy
        ClearAll();

        //變更button的背景色
        btnBasicData.BackColor = Color.Empty;
        btnFee.BackColor = Color.Empty;
        btnAccounts.BackColor = Color.Empty;
        btnUnchainTask.BackColor = Color.Empty;
        btnMachineData.BackColor = Color.FromArgb(255, 153, 0);
        btnBasicDataByTaxno.BackColor = Color.Empty;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 解除作業Button事件
    /// </summary>
    protected void btnUnchainTask_Click(object sender, EventArgs e)
    {
        this.pnlBasic.Visible = false;
        this.pnlFee.Visible = false;
        this.pnlAccount.Visible = false;
        this.pnlCancelTask.Visible = true;
        this.pnlMachineData.Visible = false;
        this.chboxP4.Visible = true;

        this.tabMostly.Visible = true;//20190919 add by Peggy
        this.tabBasicData.Visible = false;//20190919 add by Peggy
        this.pnlBasicByTaxno.Visible = false;//20190919 add by Peggy
        ClearAll();

        //變更button的背景色
        btnBasicData.BackColor = Color.Empty;
        btnFee.BackColor = Color.Empty;
        btnAccounts.BackColor = Color.Empty;
        btnUnchainTask.BackColor = Color.FromArgb(255, 153, 0);
        btnMachineData.BackColor = Color.Empty;
        btnBasicDataByTaxno.BackColor = Color.Empty;
    }

    //基本資料異動(By 統編)
    protected void btnBasicDataByTaxno_Click(object sender, EventArgs e)
    {
        this.pnlBasic.Visible = false;
        this.pnlFee.Visible = false;
        this.pnlAccount.Visible = false;
        this.pnlCancelTask.Visible = false;
        this.pnlMachineData.Visible = false;
        this.chboxP4.Visible = false;

        this.tabMostly.Visible = false;//20190919 add by Peggy
        this.tabBasicData.Visible = true;//20190919 add by Peggy
        this.pnlBasicByTaxno.Visible = true;//20190919 add by Peggy
        ClearAll();

        //變更button的背景色
        btnBasicData.BackColor = Color.Empty;
        btnFee.BackColor = Color.Empty;
        btnAccounts.BackColor = Color.Empty;
        btnUnchainTask.BackColor = Color.Empty;
        btnMachineData.BackColor = Color.Empty;
        btnBasicDataByTaxno.BackColor = Color.FromArgb(255, 153, 0);
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 查詢事件
    /// </summary>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        string strColumns = "";//*sql語句中查詢的欄位名

        EnabledControls(false);

        //*設置checkbox選擇狀態
        if (this.txtShopId.Text.Trim().Substring(2, 1) == "5")
        {
            this.chboxP4.Checked = true;
            this.chboxP4A.Checked = true;
        }
        else
        {
            this.chboxP4A.Checked = true;
        }

        #region 給查詢區域欄位賦值

        if (GetMainframeData())
        {
            // 若獲取主機訊息成功
            // 基本資料異動
            //20190806-RQ-2019-008595-002-長姓名需求，新4個欄位 by Peggy
            if (this.pnlBasic.Visible == true)
            {
                strColumns = @"headquarter_corpno,aml_cc,establish,organization,
                               record_name, business_name, MERCHANT_NAME, english_name, undertaker, undertaker_id,Undertaker_L,Undertaker_Pinyin,
                               undertaker_EngName,undertaker_tel1, undertaker_tel2, undertaker_tel3,
                               country_code, passport_no, passport_expdt, resident_no, resident_expdt,
	                           undertaker_add1, undertaker_add2, undertaker_add3, realperson, realperson_id,
	                           junctionperson, realperson_tel1, realperson_tel2, realperson_tel3,JunctionPerson_L,JunctionPerson_Pinyin,
	                           junctionperson_tel1, junctionperson_tel2, junctionperson_tel3,
	                           junctionperson_fax1, junctionperson_fax2,realperson_add1, realperson_add2, realperson_add3,email,
	                           REG_ZIP_CODE, junctionperson_recadd1, junctionperson_recadd2, junctionperson_recadd3,
	                           realadd_zip, junctionperson_realadd1, junctionperson_realadd2, junctionperson_realadd3,
	                           zip, commadd1, comaddr2, introduce, introduce_flag, invoice_cycle, HOLD_STMT,
	                           ReqAppro, black, class, LAST_UPD_MAKER, LAST_UPD_CHECKER, LAST_UPD_BRANCH, p4, p4a";
                string[] aa = strColumns.Split(',');
                SelectData(this.pnlBasic, m_ShopBasicFlag, strColumns);
                //base.sbRegScript.Append(BaseHelper.SetFocus("txtRecordNameTwo"));
                base.sbRegScript.Append(BaseHelper.SetFocus("txtUniNo"));

                //20190806-RQ-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy
                if (!txtUndertaker_L.Text.Trim().Equals(""))
                {
                    chkisLongName.Checked = true;
                }
                CheckBox_CheckedChanged(chkisLongName, null);
                
                if (!txtJunctionPerson_L.Text.Trim().Equals(""))
                {
                    chkisLongName_c.Checked = true;
                }
                CheckBox_CheckedChanged(chkisLongName_c, null);

                txtREG_ZIP_CODE.Enabled = false;
                txtREG_ZIP_CODE.BackColor = Color.LightGray;
            }

            // 費率異動(作業畫面：P4A)
            if (this.pnlFee.Visible == true)
            {
                //*modified by lvliangzhao 20090019 20100325 start 增加查詢欄位字符串
                strColumns = @"addjcb, cup_status1, cup_rate1, addjcb_us, cup_status2, cup_rate2, addjcb_notus, cup_status3, 
                               cup_rate3, moddate, cup_status4, cup_rate4, page1, cup_status5, cup_rate5, page2, cup_status6, 
	                           cup_rate6, page3, cup_status7, cup_rate7, page4, rate09, cup_status8, cup_rate8, rate02N, 
	                           rate10, cup_status9, cup_rate9, rate03V, rate11, cup_status10, cup_rate10, rate04V, rate12, 
	                           rate05M, rate13, rate06M, rate14, rate07J, rate15, rate08J, p4, p4a";
                //*modified by lvliangzhao 20090019 20100325 end

                SelectData(this.pnlFee, m_ShopFeeFlag, strColumns);
                base.sbRegScript.Append(BaseHelper.SetFocus("txtAddjcb"));
            }

            // 帳號異動(作業畫面：P4/P4A)
            if (this.pnlAccount.Visible == true)
            {
                strColumns = "bank_name, branch_name, account, check_num, account1, account2, p4, p4a";
                SelectData(this.pnlAccount, m_ShopAccountFlag, strColumns);
                ViewState["AccountData"] = GetBeforeUpdateValues(this.pnlAccount);
                base.sbRegScript.Append(BaseHelper.SetFocus("txtBankName"));
            }

            // 解約作業(作業畫面：P4/P4A)
            if (this.pnlCancelTask.Visible == true)
            {
                strColumns = "cancel_code1, cancel_date, cancel_code2, LAST_UPD_MAKER, LAST_UPD_CHECKER, LAST_UPD_BRANCH, p4, p4a";
                SelectData(this.pnlCancelTask, m_ShopCancelFlag, strColumns);

                Hashtable htInput = new Hashtable();
                htInput.Add("ACCT", this.txtShopId.Text.Trim());
                htInput.Add("FUNCTION_CODE", "I");
                htInput.Add("ORGN", "822");
                Hashtable hstPcmmP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCHR, htInput, false, "1", eAgentInfo);

                //20211224_Ares_Jack_ cancel_code1等於1解約還原打勾
                DataSet ds_cancel_code1 = CSIPKeyInGUI.BusinessRules.BRSHOP.Select(txtShopId.Text.Trim(), "1", "4", "cancel_code1");
                if (ds_cancel_code1.Tables[0].Rows.Count > 0)
                {
                    if (ds_cancel_code1.Tables[0].Rows[0][0].ToString().Trim() == "1")
                        this.chkCancelRevert.Checked = true;
                }

                //20220316_Ares_Jack_txtCancelCode1文字預設黑色
                this.txtCancelCode1.ForeColor = Color.Black;
                //20211112_Ares_Jack_商店已解約
                if (hstPcmmP4A["STATUS_FLAG"].ToString() == "8")
                {
                    this.txtCancelCode1.ForeColor = Color.FromArgb(0, Color.Transparent);
                    this.txtCancelCode1.Enabled = false;//解約代號
                    this.txtCancelDate.Enabled = false;//解約日期
                    this.txtCancelCode2.Enabled = false;//解約原因碼
                }

                // 20210527 EOS_AML(NOVA) by Ares Dennis
                if (eAgentInfo.roles.ToUpper().Contains("CSIP0101") && hstPcmmP4A["STATUS_FLAG"].ToString() == "8" && hstPcmmP4A["DTE_USER_1"].ToString() != "00000000" && hstPcmmP4A["USER_CODE_2"].ToString() != "")// 新增解約/管制還原作業，權限：CSIP0101，僅正職人員可操作 
                {
                    chkCancelRevert.Enabled = true;
                }
                else
                {
                    chkCancelRevert.Enabled = false;                    
                }

                base.sbRegScript.Append(BaseHelper.SetFocus("txtCancelCode1"));
            }

            // 機器資料(作業畫面：P4A)
            if (this.pnlMachineData.Visible == true)
            {
                strColumns = @"imp1_type, imp1, imp2_type, imp2, imp_money, pos1_type, pos1, pos2_type, pos2, pos_money, 
                               edc1_type, edc1, edc_type, edc, edc_money, p4, p4a";
                SelectData(this.pnlMachineData, m_ShopMaFlag, strColumns);
                base.sbRegScript.Append(BaseHelper.SetFocus("txtImp1Type"));
            }
        }
        else
        {
            EnabledControls(false);
        }

        #endregion
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 檢核商店營業地址所填入是否存在事件
    /// </summary>
    protected void btnSubmitClick(object sender, EventArgs e)
    {
        string strScript = "";


        EntitySet<EntitySZIP> SZIPSetOne = null;
        EntitySet<EntitySZIP> SZIPSetTwo = null;

        DateTime realDate;
        string date = "";

        // 檢查設立日期
        string establishDate = this.txtEstablish.Text.Trim();
        if (!string.IsNullOrEmpty(establishDate))
        {
            int year = int.Parse(establishDate.Substring(0, 3)) + 1911;
            date = year + "/" + establishDate.Substring(3, 2) + "/" + establishDate.Substring(5, 2);
            if (!DateTime.TryParse(date, out realDate))
            {
                base.sbRegScript.Append("alert('設立日期格式錯誤');$('#txtEstablish').focus();");
                return;
            }
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

        //20220106_Ares_Jack_國籍不得選無
        if (this.txtCountryCode.Text == "無")
        {
            base.sbRegScript.Append("alert('國籍不得選無');$('#txtCountryCode').focus();");
            return;
        }

        // 檢查護照效期
        string passportExpdt = this.txtPassportExpdt.Text.Trim();
        if (!string.IsNullOrEmpty(passportExpdt) && passportExpdt.ToUpper() != "X")
        {
            date = passportExpdt.Substring(0, 4) + "/" + passportExpdt.Substring(4, 2) + "/" + passportExpdt.Substring(6, 2);
            if (!DateTime.TryParse(date, out realDate))
            {
                base.sbRegScript.Append("alert('護照效期格式錯誤');$('#txtPassportExpdt').focus();");
                return;
            }
        }

        //20201021-202012RC 統一證號碼(新+舊)邏輯檢核
        if (!string.IsNullOrEmpty(txtResidentNo.Text) && !txtResidentNo.Text.Trim().ToUpper().Equals("X"))
        {
            if (txtResidentNo.Text.Length != 10)
            {
                base.sbRegScript.Append("alert('統一證號須為10碼');$('#txtResidentNo').focus();");
                return;
            }
            else
            {
                if (!CheckResidentID(txtResidentNo.Text.Trim()))//20200515統一證號(新+舊)檢核
                {
                    base.sbRegScript.Append("alert('統一證號輸入錯誤，請重新輸入');$('#txtResidentNo').focus();");
                    return;
                }
            }
        }

        // 檢查居留效期
        string residentExpdt = this.txtResidentExpdt.Text.Trim();
        if (!string.IsNullOrEmpty(residentExpdt) && residentExpdt.ToUpper() != "X")
        {
            date = residentExpdt.Substring(0, 4) + "/" + residentExpdt.Substring(4, 2) + "/" + residentExpdt.Substring(6, 2);
            if (!DateTime.TryParse(date, out realDate))
            {
                base.sbRegScript.Append("alert('統一證號效期格式錯誤');$('#txtResidentExpdt').focus();");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
                return;
            }
        }

        //20190806-RQ-2019-008595-002-長姓名需求 by Peggy↓
        if ((!txtUndertaker_L.Text.Trim().Equals("") || !txtUndertaker.Text.Trim().Equals("")) && txtUndertakerID.Text.Trim().Equals(""))
        {
            base.sbRegScript.Append("alert('修改姓名時，請輸入負責人身份ID');$('#txtUndertakerID').focus();");
            return;
        }

        if (chkisLongName.Checked)
        {
            if (txtUndertakerID.Text.Trim().Equals(""))//異動長姓名時，身份ID為KEY值不得為空白
            {
                base.sbRegScript.Append("alert('負責人長姓名FLAG勾選時，請輸入負責人身份ID');$('#txtUndertakerID').focus();");
                return;
            }

            if (txtUndertaker_L.Text.Trim().Equals(""))
            {
                base.sbRegScript.Append("alert('負責人長姓名FLAG勾選時，請輸入負責人中文長姓名');$('#txtUndertaker_L').focus();");
                return;
            }

            if (txtUndertaker_Pinyin.Text.Trim().Equals(""))
            {
                base.sbRegScript.Append("alert('負責人長姓名FLAG勾選時，請輸入羅馬拼音');$('#txtUndertaker_Pinyin').focus();");
                return;
            }

            if ((ToWide(txtUndertaker_L.Text.Trim()).Length + LongNameRomaClean(txtUndertaker_Pinyin.Text).Trim().Length) < 5)
            {
                base.sbRegScript.Append("alert('負責人長姓名FLAG勾選時，負責人姓名(中文+羅馬拼音)需超過4個字以上');$('#txtUndertaker_L').focus();");
                return;
            }
        }

        if (chkisLongName_c.Checked)
        {
            if (txtJunctionPerson_L.Text.Trim().Equals(""))
            {
                base.sbRegScript.Append("alert('聯絡人長姓名FLAG勾選時，請輸入中文長姓名');$('#txtJunctionPerson_L').focus();");
                return;
            }
            if (txtJunctionPerson_Pinyin.Text.Trim().Equals(""))
            {
                base.sbRegScript.Append("alert('聯絡人長姓名FLAG勾選時，請輸入羅馬拼音');$('#txtJunctionPerson_Pinyin').focus();");
                return;
            }
            if ((ToWide(txtJunctionPerson_L.Text.Trim()).Length + LongNameRomaClean(txtJunctionPerson_Pinyin.Text).Trim().Length) < 5)
            {
                base.sbRegScript.Append("alert('聯絡人長姓名FLAG勾選時，聯絡人姓名(中文+羅馬拼音)需超過4個字以上');$('#txtJunctionPerson_L').focus();");
                return;
            }
        }
        if (!txtUndertaker_Pinyin.Text.Trim().Equals(""))
        {
            if (!ValidRoma(txtUndertaker_Pinyin.Text.Trim()))
            {
                base.sbRegScript.Append("alert('負責人羅馬拼音輸入有誤');$('#txtUndertaker_Pinyin').focus();");
                return;
            }
        }
        if (!txtJunctionPerson_Pinyin.Text.Trim().Equals(""))
        {
            if (!ValidRoma(txtJunctionPerson_Pinyin.Text.Trim()))
            {
                base.sbRegScript.Append("alert('聯絡人羅馬拼音輸入有誤');$('#txtJunctionPerson_Pinyin').focus();");
                return;
            }
        }
        //20190806-RQ-2019-008595-002-長姓名需求 by Peggy↑

        //*檢核商店營業地址所填入【商店營業地址(2)】是否存在（若填入為空，則不檢核）
        if (txtJunctionPersonRealadd1.Text != "")
        {
            try
            {
                SZIPSetOne = BRSZIP.SelectEntitySet(txtJunctionPersonRealadd1.Text.Trim());
            }
            catch
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                return;
            }
        }

        //*檢核帳單地址所填入【帳單地址(2)】是否存在（若填入為空，則不檢核）

        if (txtCommadd1.Text != "")
        {
            try
            {
                SZIPSetTwo = BRSZIP.SelectEntitySet(txtCommadd1.Text.Trim());
            }
            catch
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                return;
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
        string address = this.txtJunctionPersonRecadd1.Text.Trim();
        if (!string.IsNullOrEmpty(address) && !checkREG_ZIP_CODE(address))
        {
            base.sbRegScript.Append("alert('地址查無郵遞區號，請輸入正確地址或請聯繫MFA更新');");
            return;
        }

        if (SZIPSetOne == null && SZIPSetTwo == null)
        {
            btnHiden_Click(sender, e);
        }
        else
        {
            if (SZIPSetOne != null && SZIPSetTwo == null)
            {
                if (SZIPSetOne.Count > 0)
                {
                    txtRealaddZip.Text = SZIPSetOne.GetEntity(0).zip_code;
                    btnHiden_Click(sender, e);
                }
                else
                {
                    strScript = "if(confirm('" + MessageHelper.GetMessage("01_01030100_003") + "')) {$('#btnHiden').click();}else{document.getElementById('txtJunctionPersonRealadd1').focus();}";
                }
            }

            if (SZIPSetOne == null && SZIPSetTwo != null)
            {
                if (SZIPSetTwo.Count > 0)
                {
                    txtZip.Text = SZIPSetTwo.GetEntity(0).zip_code;
                    btnHiden_Click(sender, e);
                }
                else
                {
                    strScript = "if(confirm('" + MessageHelper.GetMessage("01_01030100_003") + "')) {$('#btnHiden').click();}else{document.getElementById('txtCommadd1').focus();}";
                }
            }

            if (SZIPSetOne != null && SZIPSetTwo != null)
            {
                if (SZIPSetOne.Count > 0 && SZIPSetTwo.Count > 0)
                {
                    txtRealaddZip.Text = SZIPSetOne.GetEntity(0).zip_code;
                    txtZip.Text = SZIPSetTwo.GetEntity(0).zip_code;
                    btnHiden_Click(sender, e);
                }
                else
                {
                    if (SZIPSetOne.Count == 0)
                    {
                        strScript = "if(confirm('" + MessageHelper.GetMessage("01_01030100_003") + "')) {$('#btnHiden').click();}else{document.getElementById('txtJunctionPersonRealadd1').focus();}";
                    }
                    else
                    {
                        strScript = "if(confirm('" + MessageHelper.GetMessage("01_01030100_003") + "')) {$('#btnHiden').click();}else{document.getElementById('txtCommadd1').focus();}";
                    }
                }
            }

            base.sbRegScript.Append(strScript);
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 基本資料異動提交事件
    /// </summary>
    protected void btnHiden_Click(object sender, EventArgs e)
    {
        #region 為EntitySHOP賦值
        CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop = GetEntity();
        eShop.shop_id = this.txtShopId.Text.Trim();
        eShop.record_name = this.txtRecordNameTwo.Text.Trim();
        eShop.business_name = this.txtBusinessNameTwo.Text.Trim();
        eShop.MERCHANT_NAME = this.txtMerchantNameTwo.Text.Trim();
        eShop.english_name = this.txtEnglishName.Text.Trim();
        eShop.undertaker = this.txtUndertaker.Text.Trim();
        eShop.undertaker_id = this.txtUndertakerID.Text.Trim().ToUpper();
        eShop.undertaker_EngName = this.txtUndertakerEngName.Text.Trim();
        eShop.undertaker_tel1 = this.txtUndertakerTel1.Text.Trim();
        eShop.undertaker_tel2 = this.txtUndertakerTel2.Text.Trim();
        eShop.undertaker_tel3 = this.txtUndertakerTel3.Text.Trim();
        eShop.undertaker_add1 = this.txtUndertakerAdd1.Text.Trim();
        eShop.undertaker_add2 = this.txtUndertakerAdd2.Text.Trim();
        eShop.undertaker_add3 = this.txtUndertakerAdd3.Text.Trim();
        eShop.realperson = this.txtRealPerson.Text.Trim();
        eShop.realperson_id = this.txtRealPersonID.Text.Trim().ToUpper();
        eShop.realperson_tel1 = this.txtRealPersonTel1.Text.Trim();
        eShop.realperson_tel2 = this.txtRealPersonTel2.Text.Trim();
        eShop.realperson_tel3 = this.txtRealPersonTel3.Text.Trim();
        eShop.junctionperson = this.txtJunctionPerson.Text.Trim();
        eShop.junctionperson_tel1 = this.txtJunctionPersonTel1.Text.Trim();
        eShop.junctionperson_tel2 = this.txtJunctionPersonTel2.Text.Trim();
        eShop.junctionperson_tel3 = this.txtJunctionPersonTel3.Text.Trim();
        eShop.junctionperson_fax1 = this.txtJunctionPersonFax1.Text.Trim();
        eShop.junctionperson_fax2 = this.txtJunctionPersonFax2.Text.Trim();
        eShop.realperson_add1 = this.txtRealPersonAdd1.Text.Trim();
        eShop.realperson_add2 = this.txtRealPersonAdd2.Text.Trim();
        eShop.realperson_add3 = this.txtRealPersonAdd3.Text.Trim();
        eShop.junctionperson_recadd1 = this.txtJunctionPersonRecadd1.Text.Trim();
        eShop.junctionperson_recadd2 = this.txtJunctionPersonRecadd2.Text.Trim();
        eShop.junctionperson_recadd3 = this.txtJunctionPersonRecadd3.Text.Trim();
        eShop.realadd_zip = this.txtRealaddZip.Text.Trim();
        eShop.junctionperson_realadd1 = this.txtJunctionPersonRealadd1.Text.Trim();
        eShop.junctionperson_realadd2 = this.txtJunctionPersonRealadd2.Text.Trim();
        eShop.junctionperson_realadd3 = this.txtJunctionPersonRealadd3.Text.Trim();
        eShop.zip = this.txtZip.Text.Trim();
        eShop.commadd1 = this.txtCommadd1.Text.Trim();
        eShop.comaddr2 = this.txtComaddr2.Text.Trim();
        eShop.introduce = this.txtIntroduces.Text.Trim();
        eShop.introduce_flag = BRCommon.GetPadLeftString(this.txtIntroduceFlag.Text.Trim(), 5, '0');
        eShop.invoice_cycle = this.txtInvoiceCycle.Text.Trim();
        eShop.HOLD_STMT = this.txtHOLDSTMT.Text.Trim();
        eShop.black = this.txtBlack.Text.Trim();
        eShop.@class = this.txtClass.Text.Trim();
        eShop.user_id = eAgentInfo.agent_id;
        eShop.KeyIn_Flag = m_KeyFlag;
        eShop.mod_date = DateTime.Now.ToString("yyyyMMdd");
        eShop.shop_type = m_ShopBasicFlag;
        eShop.Change_Item = m_ShopBasicFlag;
        //20130125 Yucheng 一KEY保存時Change_Item=0 二KEY成功才變1
        //eShop.Change_Item = "0";
        eShop.ReqAppro = this.txtReqAppro.Text.Trim();
        GetP4AndP4AValue(eShop);
        eShop.establish = this.txtEstablish.Text.Trim();            // 設立
        eShop.organization = this.txtOrganization.Text.Trim();      // 法律形式
        eShop.aml_cc = this.txtAMLCC.Text.Trim();                   // AML行業編號
        eShop.country_code = this.txtCountryCode.Text.Trim();       // 國籍
        eShop.passport_no = this.txtPassportNo.Text.Trim();         // 護照號碼
        eShop.passport_expdt = this.txtPassportExpdt.Text.Trim();   // 護照效期
        eShop.resident_no = this.txtResidentNo.Text.Trim();         // 居留證號
        eShop.resident_expdt = this.txtResidentExpdt.Text.Trim();   // 居留效期
        eShop.email = this.hidEmailFall.Value.Trim();               // 電子郵件信箱
        eShop.headquarter_corpno = this.txtUniNo.Text.Trim();       // 總公司統一編號

        //20190806-RQ-2019-008595-002-長姓名需求，新4個欄位 by Peggy↓
        eShop.Undertaker_L = ToWide(txtUndertaker_L.Text.Trim()); //負責人中文長姓名
        eShop.Undertaker_Pinyin = txtUndertaker_Pinyin.Text.Trim(); //負責人羅馬拼音,當值為X，清空

        eShop.JunctionPerson_L = ToWide(txtJunctionPerson_L.Text.Trim()); //聯絡人中文長姓名
        eShop.JunctionPerson_Pinyin = txtJunctionPerson_Pinyin.Text.Trim(); //聯絡人羅馬拼音
        //20190806-RQ-2019-008595-002-長姓名需求，新4個欄位 by Peggy↑
        
        // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis    
        eShop.REG_ZIP_CODE = txtREG_ZIP_CODE.Text.Trim();// 地址郵遞區號
        eShop.LAST_UPD_BRANCH = txtLAST_UPD_BRANCH.Text.Trim();// 資料最後異動分行
        eShop.LAST_UPD_CHECKER = txtLAST_UPD_CHECKER.Text.Trim();// 資料最後異動-CHECKER
        eShop.LAST_UPD_MAKER = txtLAST_UPD_MAKER.Text.Trim();// 資料最後異動-MAKER
        #endregion

        if (CSIPKeyInGUI.BusinessRules_new.BRSHOP.Insert(eShop, this.txtShopId.Text.Trim(), m_KeyFlag, m_ShopBasicFlag))//if (BRM_SHOP.Add(eShop))//
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_003");
            MessageHelper.ShowMessage(this.UpdatePanel1, new string[] { "01_00000000_003" });

            InsertShopRpt();
            InsertShop1KeyLog();
            //*更新資料檔trans_num
            if (!BRTRANS_NUM.UpdateTransNum("C01"))
            {
                if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    MessageHelper.ShowMessage(this.UpdatePanel1, new string[] { "00_00000000_000" });
                }
            }

            ClearAll();
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_004");
            MessageHelper.ShowMessage(this.UpdatePanel1, new string[] { "01_00000000_004" });
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 費率異動提交事件
    /// </summary>
    protected void btnFeeSubmit_Click(object sender, EventArgs e)
    {
        #region 為EntitySHOP賦值
        CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop = GetEntity();
        eShop.shop_id = this.txtShopId.Text.Trim();
        eShop.addjcb = this.txtAddjcb.Text.Trim();
        eShop.addjcb_us = this.txtAddjcbUs.Text.Trim();
        eShop.addjcb_notus = this.txtAddjcbNotus.Text.Trim();
        eShop.moddate = this.txtModdate.Text.Trim();
        eShop.page1 = this.txtPage1.Text.Trim();
        eShop.page2 = this.txtPage2.Text.Trim();
        eShop.page3 = this.txtPage3.Text.Trim();
        eShop.page4 = this.txtPage4.Text.Trim();
        eShop.rate02N = this.txtRate02N.Text.Trim();
        eShop.rate03V = this.txtRate03V.Text.Trim();
        eShop.rate04V = this.txtRate04V.Text.Trim();
        eShop.rate05M = this.txtRate05M.Text.Trim();
        eShop.rate06M = this.txtRate06M.Text.Trim();
        eShop.rate07J = this.txtRate07J.Text.Trim();
        eShop.rate08J = this.txtRate08J.Text.Trim();
        eShop.rate09 = this.txtRate09.Text.Trim();
        eShop.rate10 = this.txtRate10.Text.Trim();
        eShop.rate11 = this.txtRate11.Text.Trim();
        eShop.rate12 = this.txtRate12.Text.Trim();
        eShop.rate13 = this.txtRate13.Text.Trim();
        eShop.rate14 = this.txtRate14.Text.Trim();
        eShop.rate15 = this.txtRate15.Text.Trim();

        //*add by lvliangzhao 20090019 20100325 start 增加銀聯卡欄位賦值
        eShop.cup_status1 = this.txtStatus1.Text.Trim();
        eShop.cup_status2 = this.txtStatus2.Text.Trim();
        eShop.cup_status3 = this.txtStatus3.Text.Trim();
        eShop.cup_status4 = this.txtStatus4.Text.Trim();
        eShop.cup_status5 = this.txtStatus5.Text.Trim();
        eShop.cup_status6 = this.txtStatus6.Text.Trim();
        eShop.cup_status7 = this.txtStatus7.Text.Trim();
        eShop.cup_status8 = this.txtStatus8.Text.Trim();
        eShop.cup_status9 = this.txtStatus9.Text.Trim();
        eShop.cup_status10 = this.txtStatus10.Text.Trim();
        eShop.cup_rate1 = this.txtFee1.Text.Trim();
        eShop.cup_rate2 = this.txtFee2.Text.Trim();
        eShop.cup_rate3 = this.txtFee3.Text.Trim();
        eShop.cup_rate4 = this.txtFee4.Text.Trim();
        eShop.cup_rate5 = this.txtFee5.Text.Trim();
        eShop.cup_rate6 = this.txtFee6.Text.Trim();
        eShop.cup_rate7 = this.txtFee7.Text.Trim();
        eShop.cup_rate8 = this.txtFee8.Text.Trim();
        eShop.cup_rate9 = this.txtFee9.Text.Trim();
        eShop.cup_rate10 = this.txtFee10.Text.Trim();
        //*add by lvliangzhao 20090019 20100325 end

        eShop.user_id = eAgentInfo.agent_id;
        eShop.KeyIn_Flag = m_KeyFlag;
        eShop.mod_date = DateTime.Now.ToString("yyyyMMdd");
        eShop.shop_type = m_ShopFeeFlag;
        eShop.Change_Item = m_ShopFeeFlag;
        GetP4AndP4AValue(eShop);
        #endregion

        if (CSIPKeyInGUI.BusinessRules_new.BRSHOP.Insert(eShop, this.txtShopId.Text.Trim(), m_KeyFlag, m_ShopFeeFlag))
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_003");
            InsertShopRpt();
            InsertShop1KeyLog();
            //*更新資料檔trans_num
            if (!BRTRANS_NUM.UpdateTransNum("C01"))
            {
                if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                }
            }

            ClearAll();
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_004");
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 帳號異動提交事件
    /// </summary>
    protected void btnAccountSubmit_Click(object sender, EventArgs e)
    {
        #region 為EntitySHOP賦值
        CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop = GetEntity();
        eShop.shop_id = this.txtShopId.Text.Trim();
        eShop.bank_name = this.txtBankName.Text.Trim();
        eShop.branch_name = this.txtBranchName.Text.Trim();
        eShop.account = this.txtAccount.Text.Trim();
        eShop.check_num = this.txtCheckNum.Text.Trim();
        eShop.account1 = this.txtAccount1.Text.Trim();
        eShop.account2 = this.txtAccount2.Text.Trim();
        eShop.user_id = eAgentInfo.agent_id;
        eShop.KeyIn_Flag = m_KeyFlag;
        eShop.mod_date = DateTime.Now.ToString("yyyyMMdd");
        eShop.shop_type = m_ShopAccountFlag;
        eShop.Change_Item = m_ShopAccountFlag;
        GetP4AndP4AValue(eShop);
        #endregion

        if (CSIPKeyInGUI.BusinessRules_new.BRSHOP.Insert(eShop, this.txtShopId.Text.Trim(), m_KeyFlag, m_ShopAccountFlag))
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_003");
            InsertShopRpt();

            int intIndex = 0;
            ArrayList arrList = (ArrayList)ViewState["AccountData"];
            foreach (System.Web.UI.Control control in this.pnlAccount.Controls)
            {
                if (control is CustTextBox)
                {
                    CustTextBox txtBox = (CustTextBox)control;
                    if (txtBox.Text == arrList[intIndex].ToString().Trim())
                    {
                        txtBox.Text = "";
                    }
                    intIndex++;
                }
            }

            InsertShop1KeyLog();

            //*更新資料檔trans_num
            if (!BRTRANS_NUM.UpdateTransNum("C01"))
            {
                if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                }
            }

            ClearAll();
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_004");
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 解約作業異動提交事件
    /// </summary>
    protected void btnCancelTaskSubmit_Click(object sender, EventArgs e)
    {
        // 20210527 EOS_AML(NOVA) 檢查最後異動分行 by Ares Dennis
        string LAST_UPD_BRANCH = this.txtLAST_UPD_BRANCH2.Text.Trim();
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
        string LAST_UPD_MAKER = this.txtLAST_UPD_MAKER2.Text.Trim();
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
        string LAST_UPD_CHECKER = this.txtLAST_UPD_CHECKER2.Text.Trim();
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

        #region 為EntitySHOP賦值
        CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop = GetEntity();
        eShop.shop_id = this.txtShopId.Text.Trim();
        eShop.cancel_code1 = this.txtCancelCode1.Text.Trim();
        eShop.cancel_date = this.txtCancelDate.Text.Trim();
        eShop.cancel_code2 = this.txtCancelCode2.Text.Trim().ToUpper();
        eShop.user_id = eAgentInfo.agent_id;
        eShop.KeyIn_Flag = m_KeyFlag;
        eShop.mod_date = DateTime.Now.ToString("yyyyMMdd");
        eShop.shop_type = m_ShopCancelFlag;
        eShop.Change_Item = m_ShopCancelFlag;
        // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis        
        eShop.LAST_UPD_BRANCH = txtLAST_UPD_BRANCH2.Text.Trim();// 資料最後異動分行
        eShop.LAST_UPD_CHECKER = txtLAST_UPD_CHECKER2.Text.Trim();// 資料最後異動-CHECKER
        eShop.LAST_UPD_MAKER = txtLAST_UPD_MAKER2.Text.Trim();// 資料最後異動-MAKER

        Hashtable htInput = new Hashtable();
        htInput.Add("ACCT", this.txtShopId.Text.Trim());
        htInput.Add("FUNCTION_CODE", "I");
        htInput.Add("ORGN", "822");
        Hashtable hstPcmmP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCHR, htInput, false, "1", eAgentInfo);
        //20211227_Ares_Jack_勾選解約還原, 寫1進db cancel_code1
        if (hstPcmmP4A["STATUS_FLAG"].ToString() == "8")//已解約
        {
            //20220121_Ares_Jack_已解約商店 未勾選解約還原不得提交
            if (this.chkCancelRevert.Checked == false)
            {
                base.sbRegScript.Append("alert('請勾選解約還原!');");
                return;
            }
            if (this.chkCancelRevert.Checked == true)
                eShop.cancel_code1 = "1";
            else
                eShop.cancel_code1 = "";
        }

        GetP4AndP4AValue(eShop);
        #endregion

        if (CSIPKeyInGUI.BusinessRules_new.BRSHOP.Insert(eShop, this.txtShopId.Text.Trim(), m_KeyFlag, m_ShopCancelFlag))
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_003");
            InsertShopRpt();
            InsertShop1KeyLog();
            //*更新資料檔trans_num
            if (!BRTRANS_NUM.UpdateTransNum("C01"))
            {
                if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                }
            }
            ClearAll();
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_004");
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 機器資料異動提交事件
    /// </summary>
    protected void btnMaSubmit_Click(object sender, EventArgs e)
    {
        #region 為EntitySHOP賦值
        CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop = GetEntity();
        eShop.shop_id = this.txtShopId.Text.Trim();
        eShop.imp1 = BRCommon.GetPadLeftString(this.txtImp1.Text.Trim(), 3, '0');
        eShop.imp1_type = this.txtImp1Type.Text.Trim();
        eShop.imp2 = BRCommon.GetPadLeftString(this.txtImp2.Text.Trim(), 3, '0');
        eShop.imp2_type = this.txtImp2Type.Text.Trim();
        eShop.imp_money = this.txtImpMoney.Text.Trim();
        eShop.pos1 = BRCommon.GetPadLeftString(this.txtPos1.Text.Trim(), 3, '0');
        eShop.pos1_type = this.txtPos1Type.Text.Trim();
        eShop.pos2 = BRCommon.GetPadLeftString(this.txtPos2.Text.Trim(), 3, '0');
        eShop.pos2_type = this.txtPos2Type.Text.Trim();
        eShop.pos_money = this.txtPosMoney.Text.Trim();
        eShop.edc1 = BRCommon.GetPadLeftString(this.txtEdc1.Text.Trim(), 3, '0');
        eShop.edc1_type = this.txtEdc1Type.Text.Trim();
        eShop.edc_type = this.txtEdcType.Text.Trim();
        eShop.edc = BRCommon.GetPadLeftString(this.txtEdc.Text.Trim(), 3, '0');
        eShop.edc_money = this.txtEdcMoney.Text.Trim();
        eShop.user_id = eAgentInfo.agent_id;
        eShop.KeyIn_Flag = m_KeyFlag;
        eShop.mod_date = DateTime.Now.ToString("yyyyMMdd");
        eShop.shop_type = m_ShopMaFlag;
        eShop.Change_Item = m_ShopMaFlag;
        GetP4AndP4AValue(eShop);
        #endregion

        if (CSIPKeyInGUI.BusinessRules_new.BRSHOP.Insert(eShop, this.txtShopId.Text.Trim(), m_KeyFlag, m_ShopMaFlag))
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_003");
            InsertShopRpt();
            InsertShop1KeyLog();
            //*更新資料檔trans_num
            if (!BRTRANS_NUM.UpdateTransNum("C01"))
            {
                if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                }
            }

            ClearAll();
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_004");
        }
    }

    /// <summary>
    /// 使用者輸入的地址解析後系統自動帶入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void TextBox_AddrChanged(object sender, EventArgs e)
    {
        string strZipData = this.txtJunctionPersonRecadd1.Text.Trim();

        EntitySet<EntitySZIP> SZIPSet = null;
        try
        {
            SZIPSet = BRSZIP.SelectEntitySet(strZipData);
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (SZIPSet != null && SZIPSet.Count > 0)
        {
            this.txtREG_ZIP_CODE.Text = SZIPSet.GetEntity(0).zip_code;
        }
        else
        {
            this.txtREG_ZIP_CODE.Text = "";
            if (this.txtJunctionPersonRecadd1.Text.Trim() != "")//20220114_Ares_Jack_不等於空值才跳錯誤檢核
            {
                base.strClientMsg += "地址查無郵遞區號，請輸入正確地址或請聯繫MFA更新";
                base.sbRegScript.Append("alert('地址查無郵遞區號，請輸入正確地址或請聯繫MFA更新');");
            }
        }
        base.sbRegScript.Append(BaseHelper.SetFocus("txtJunctionPersonRecadd2"));// 將地址2設為輸入焦點
    }

    /// <summary>
    /// 使用者輸入的地址解析後系統自動帶入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void TextBox_AddrChanged2(object sender, EventArgs e)
    {
        string strZipData = this.txtREG_CITY.Text.Trim();

        EntitySet<EntitySZIP> SZIPSet = null;
        try
        {
            SZIPSet = BRSZIP.SelectEntitySet(strZipData);
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (SZIPSet != null && SZIPSet.Count > 0)
        {
            this.txtREG_ZIP_CODE2.Text = SZIPSet.GetEntity(0).zip_code;
        }
        else
        {
            this.txtREG_ZIP_CODE2.Text = "";
            if (this.txtREG_CITY.Text.Trim() != "")//20220114_Ares_Jack_不等於空值才跳錯誤檢核
            {
                base.strClientMsg += "地址查無郵遞區號，請輸入正確地址或請聯繫MFA更新";
                base.sbRegScript.Append("alert('地址查無郵遞區號，請輸入正確地址或請聯繫MFA更新');");
            }
        }
        base.sbRegScript.Append(BaseHelper.SetFocus("txtREG_ADDR1"));// 將地址2設為輸入焦點
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
        //20190731 修改：將result Table排序 by Peggy
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

                //20190919 add by Peggy
                //註冊國籍
                ddlCORP_CountryCode.Items.Add(listItem);
                //國籍
                ddlPrincipalCountryCode.Items.Add(listItem);
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

        // 載入法律形式
        result = BRPostOffice_CodeType.GetCodeType("2");
        if (result != null && result.Rows.Count > 0)
        {
            for (int i = 0; i < result.Rows.Count; i++)
            {
                listItem = new ListItem();
                listItem.Value = result.Rows[i][0].ToString();
                listItem.Text = result.Rows[i][0].ToString();
                //listItem.Text = result.Rows[i][1].ToString();
                this.dropOrganization.Items.Add(listItem);
                organization += result.Rows[i][0].ToString() + ":";

                //20190919 addy by Peggy
                //法律形式
                ddlCORP_Organization.Items.Add(listItem);
            }
            this.hidOrganization.Value = organization;
        }
    }

    // 設定 州別 //20190919 add by Peggy
    private void SetDropStateCode()
    {
        ListItem listItem = new ListItem();
        string listString = string.Empty;

        string[] arr = { "德克薩斯州;TX", "新墨西哥州;NM", "亞利桑那州;AZ", "加利福尼亞州;CA", "達拉瓦州;DE", "內華達州;NV", "非前述州別;NA" };
        string[] arrs = null;

        for (int i = 0; i < arr.Length; i++)
        {
            arrs = arr[i].Split(';');

            listItem = new ListItem();

            listItem.Value = arrs[0].ToString();
            listItem.Text = arrs[1].ToString();

            listString += arrs[1].ToString() + ":";

            this.ddlCORP_CountryStateCode.Items.Add(listItem);
        }

        //hiddenField.Value = listString;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 初始化EntitySHOP
    /// </summary>
    /// <returns>EntitySHOP</returns>
    private CSIPKeyInGUI.EntityLayer_new.EntitySHOP GetEntity()
    {
        CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop = new CSIPKeyInGUI.EntityLayer_new.EntitySHOP();
        eShop.shop_id = "";
        eShop.record_name = "";
        eShop.business_name = "";
        eShop.MERCHANT_NAME = "";
        eShop.english_name = "";
        eShop.undertaker = "";
        eShop.undertaker_id = "";
        eShop.undertaker_EngName = "";
        eShop.undertaker_tel1 = "";
        eShop.undertaker_tel2 = "";
        eShop.undertaker_tel3 = "";
        eShop.undertaker_add1 = "";
        eShop.undertaker_add2 = "";
        eShop.undertaker_add3 = "";
        eShop.realperson = "";
        eShop.realperson_id = "";
        eShop.realperson_tel1 = "";
        eShop.realperson_tel2 = "";
        eShop.realperson_tel3 = "";
        eShop.junctionperson = "";
        eShop.junctionperson_tel1 = "";
        eShop.junctionperson_tel2 = "";
        eShop.junctionperson_tel3 = "";
        eShop.junctionperson_fax1 = "";
        eShop.junctionperson_fax2 = "";
        eShop.realperson_add1 = "";
        eShop.realperson_add2 = "";
        eShop.realperson_add3 = "";
        eShop.junctionperson_recadd1 = "";
        eShop.junctionperson_recadd2 = "";
        eShop.junctionperson_recadd3 = "";
        eShop.realadd_zip = "";
        eShop.junctionperson_realadd1 = "";
        eShop.junctionperson_realadd2 = "";
        eShop.junctionperson_realadd3 = "";
        eShop.zip = "";
        eShop.commadd1 = "";
        eShop.comaddr2 = "";
        eShop.introduce = "";
        eShop.introduce_flag = "";
        eShop.invoice_cycle = "";
        eShop.HOLD_STMT = "";
        eShop.black = "";
        eShop.@class = "";
        eShop.addjcb = "";
        eShop.addjcb_us = "";
        eShop.addjcb_notus = "";
        eShop.moddate = "";
        eShop.page1 = "";
        eShop.page2 = "";
        eShop.page3 = "";
        eShop.page4 = "";
        eShop.rate02N = "";
        eShop.rate03V = "";
        eShop.rate04V = "";
        eShop.rate05M = "";
        eShop.rate06M = "";
        eShop.rate07J = "";
        eShop.rate08J = "";
        eShop.rate09 = "";
        eShop.rate10 = "";
        eShop.rate11 = "";
        eShop.rate12 = "";
        eShop.rate13 = "";
        eShop.rate14 = "";
        eShop.rate15 = "";
        eShop.bank_name = "";
        eShop.branch_name = "";
        eShop.account = "";
        eShop.check_num = "";
        eShop.account1 = "";
        eShop.account2 = "";
        eShop.cancel_code1 = "";
        eShop.cancel_date = "";
        eShop.cancel_code2 = "";
        eShop.imp1 = "";
        eShop.imp1_type = "";
        eShop.imp2 = "";
        eShop.imp2_type = "";
        eShop.imp_money = "";
        eShop.pos1 = "";
        eShop.pos1_type = "";
        eShop.pos2 = "";
        eShop.pos2_type = "";
        eShop.pos_money = "";
        eShop.edc1 = "";
        eShop.edc1_type = "";
        eShop.edc_type = "";
        eShop.edc = "";
        eShop.edc_money = "";
        //*add by lvliangzhao 20090019 20100325 start 初始值為""
        eShop.cup_status1 = "";
        eShop.cup_status2 = "";
        eShop.cup_status3 = "";
        eShop.cup_status4 = "";
        eShop.cup_status5 = "";
        eShop.cup_status6 = "";
        eShop.cup_status7 = "";
        eShop.cup_status8 = "";
        eShop.cup_status9 = "";
        eShop.cup_status10 = "";
        eShop.cup_rate1 = "";
        eShop.cup_rate2 = "";
        eShop.cup_rate3 = "";
        eShop.cup_rate4 = "";
        eShop.cup_rate5 = "";
        eShop.cup_rate6 = "";
        eShop.cup_rate7 = "";
        eShop.cup_rate8 = "";
        eShop.cup_rate9 = "";
        eShop.cup_rate10 = "";
        //*add by lvliangzhao 20090019 20100325 end
        eShop.user_id = "";
        eShop.KeyIn_Flag = "";
        eShop.mod_date = "";
        eShop.shop_type = "";
        eShop.Change_Item = "";
        eShop.edcnum = "";
        eShop.ReqAppro = "";

        eShop.establish = "";
        eShop.organization = "";
        eShop.aml_cc = "";
        eShop.country_code = "";
        eShop.passport_no = "";
        eShop.resident_no = "";
        eShop.passport_expdt = "";
        eShop.resident_expdt = "";
        eShop.email = "";
        eShop.single_merchant = "";
        eShop.headquarter_corpno = "";

        //20190806-RQ-2019-008595-002-長姓名需求，新4個欄位 by Peggy↓
        eShop.Undertaker_L = ""; //負責人中文長姓名
        eShop.Undertaker_Pinyin = ""; //負責人羅馬拼音
        eShop.JunctionPerson_L = ""; //聯絡人中文長姓名
        eShop.JunctionPerson_Pinyin = ""; //聯絡人羅馬拼音
        //20190806-RQ-2019-008595-002-長姓名需求，新4個欄位 by Peggy↑

        // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis
        eShop.REG_ZIP_CODE = "";// 登記地址郵遞區號
        eShop.LAST_UPD_BRANCH = "";// 資料最後異動分行
        eShop.LAST_UPD_CHECKER = "";// 資料最後異動-CHECKER
        eShop.LAST_UPD_MAKER = "";// 資料最後異動-MAKER

        return eShop;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 設置控件文本
    /// </summary>
    private void SetControlsText()
    {
        #region 給文本賦值
        chboxP4.Text = BaseHelper.GetShowText("01_01030100_088");
        chboxP4A.Text = BaseHelper.GetShowText("01_01030100_089");
        
        #endregion
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 查詢一KEY資料信息
    /// </summary>
    /// <param name="pnlType">所在panel</param>
    /// <param name="strShopType">功能畫面編號</param>
    /// <param name="strColumns">查詢的列名</param>
    private void SelectData(CustPanel pnlType, string strShopType, string strColumns)
    {
        //20190919 addy by Peggy
        //DataSet dstInfo = CSIPKeyInGUI.BusinessRules_new.BRSHOP.Select(this.txtShopId.Text.Trim(), m_KeyFlag, strShopType, strColumns);
        DataSet dstInfo = new DataSet();
        if (strShopType.Trim().Equals("6"))
        {
            dstInfo = CSIPKeyInGUI.BusinessRules_new.BRSHOP_CHANGE.Select(this.txtUNI_NO1.Text.Trim(), m_KeyFlag, strColumns, "");
        }
        else
        {
            dstInfo = CSIPKeyInGUI.BusinessRules_new.BRSHOP.Select(this.txtShopId.Text.Trim(), m_KeyFlag, strShopType, strColumns);
        }
        
        CommonFunction.SetControlsEnabled(pnlType, true);

        if (dstInfo == null)
        {
            isNew = true;
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (dstInfo.Tables[0].Rows.Count > 0)
        {
            // 查詢的欄位必須和網頁中的欄位相匹配
            // 如若網頁中要顯示10各欄位，則查詢的前10各欄位必須和網頁中的欄位對應
            SetTextBoxValue(pnlType, dstInfo.Tables[0]);
        }
        else
        {
            isNew = true;
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_001");
        }
    }

    //*add by lvliangzhao 20090019 20100325 start
    /// 作者 趙呂梁
    /// 創建日期：2010/03/25
    /// 修改日期：2010/03/25 
    /// <summary>
    /// 設置費率銀聯卡文字
    /// </summary>
    private void SetFeeLableValue()
    {
        DataTable dtblResult = new DataTable();
        if (BaseHelper.GetCommonProperty("01", "21", ref dtblResult))
        {
            if (dtblResult.Rows.Count > 9)
            {
                lblTag1.Text = dtblResult.Rows[0][1].ToString();
                lblTag2.Text = dtblResult.Rows[1][1].ToString();
                lblTag3.Text = dtblResult.Rows[2][1].ToString();
                lblTag4.Text = dtblResult.Rows[3][1].ToString();
                lblTag5.Text = dtblResult.Rows[4][1].ToString();
                lblTag6.Text = dtblResult.Rows[5][1].ToString();
                lblTag7.Text = dtblResult.Rows[6][1].ToString();
                lblTag8.Text = dtblResult.Rows[7][1].ToString();
                lblTag9.Text = dtblResult.Rows[8][1].ToString();
                lblTag10.Text = dtblResult.Rows[9][1].ToString();
            }
        }
    }
    //*add by lvliangzhao 20090019 20100325 end

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 設置TextBox的值
    /// </summary>
    /// <param name="pnlType">所在的PANEL</param>
    /// <param name="dtInfo">表信息</param>
    private void SetTextBoxValue(CustPanel pnlType, DataTable dtInfo)
    {
        int intIndex = 0;// index for textbox
        int intCount = dtInfo.Columns.Count;// count for items from datatable
        string strP4 = "";
        string strP4A = "";

        foreach (System.Web.UI.Control control in pnlType.Controls)
        {
            if (control is CustTextBox)
            {
                CustTextBox txtBox = (CustTextBox)control;
                // email欄位拆為兩個TextBox(以@符號做為分隔)
                if (txtBox.ID == "txtEmailFront")
                    continue;

                txtBox.Text = dtInfo.Rows[0][intIndex].ToString().Trim();
                intIndex++;

                if (intIndex >= intCount)
                    break;
            }
        }

        try
        {
            if (!pnlType.ID.Equals("pnlBasicByTaxno"))
            {
                SetEmailValue(dtInfo.Rows[0]["email"].ToString().Trim());

                //strP4 = dtInfo.Rows[0][intIndex++].ToString().Trim();
                //strP4A = dtInfo.Rows[0][intIndex++].ToString().Trim();
                strP4 = dtInfo.Rows[0]["p4"].ToString().Trim();
                strP4A = dtInfo.Rows[0]["p4A"].ToString().Trim();

                this.chboxP4.Checked = false;
                this.chboxP4A.Checked = false;

                if (strP4 == "1")
                {
                    this.chboxP4.Checked = true;
                }
                else
                {
                    this.chboxP4.Checked = false;
                }

                if (strP4A == "1")
                {
                    this.chboxP4A.Checked = true;
                }
                else
                {
                    this.chboxP4A.Checked = false;
                }

            }
        }
        catch
        {

        }
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

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 資料檔shop_rpt中寫入一筆記錄
    /// </summary>
    private void InsertShopRpt()
    {
        EntitySHOP_RPT eShopRpt = new EntitySHOP_RPT();
        eShopRpt.shop_id = this.txtShopId.Text.Trim();
        eShopRpt.shop_name = this.txtRecordNameTwo.Text.Trim();
        eShopRpt.user_id = eAgentInfo.agent_id;
        eShopRpt.write_date = DateTime.Now.ToString("yyyyMMdd");
        eShopRpt.type = m_KeyFlag;
        try
        {
            BRSHOP_RPT.AddEntity(eShopRpt);
        }
        catch
        {
            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("01_00000000_020")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_020");
            }
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 資料檔shop_1keylog中寫入一筆記錄
    /// </summary>
    private void InsertShop1KeyLog()
    {
        EntitySHOP_1KEYLOG eShopKl = new EntitySHOP_1KEYLOG();
        eShopKl.shop_id = this.txtShopId.Text.Trim();
        eShopKl.user_id = eAgentInfo.agent_id;
        eShopKl.write_date = DateTime.Now.ToString("yyyyMMdd");
        eShopKl.bank_name = this.txtBankName.Text.Trim();
        eShopKl.branch_name = this.txtBranchName.Text.Trim();
        eShopKl.account = this.txtAccount.Text.Trim();
        eShopKl.check_num = this.txtCheckNum.Text.Trim();
        eShopKl.account1 = this.txtAccount1.Text.Trim();
        eShopKl.account2 = this.txtAccount2.Text.Trim();
        eShopKl.cancel_code1 = this.txtCancelCode1.Text.Trim();
        eShopKl.cancel_code2 = this.txtCancelCode2.Text.Trim().ToUpper();
        eShopKl.cancel_date = this.txtCancelDate.Text.Trim();
        try
        {
            BRSHOP_1KEYLOG.AddEntity(eShopKl);
        }
        catch
        {
            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("01_00000000_020")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_020");
            }
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 設置輸入欄位是否可用
    /// </summary>
    /// <param name="blnEnabled">true可用，false不可用</param>
    private void EnabledControls(bool blnEnabled)
    {
        CommonFunction.SetControlsEnabled(this.pnlAccount, blnEnabled);//*帳號部分disable
        CommonFunction.SetControlsEnabled(this.pnlBasic, blnEnabled);//*基本資料部分disable
        CommonFunction.SetControlsEnabled(this.pnlCancelTask, blnEnabled);//*解約作業部分disable
        CommonFunction.SetControlsEnabled(this.pnlFee, blnEnabled);//*費率部分disable
        CommonFunction.SetControlsEnabled(this.pnlMachineData, blnEnabled);//*機器資料部分disable
        CommonFunction.SetControlsEnabled(this.pnlBasicByTaxno, blnEnabled);//基本資料by統編部份disable//20190912 add by Peggy
        ClearCommonPartText();
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11
    /// <summary>
    /// 清空、禁用異動區域所有欄位，并設置商店代號為焦點
    /// </summary>
    private void ClearAll()
    {
        EnabledControls(false);
        
        //20190924 add by Peggy
        if (pnlBasicByTaxno.Visible == true)
        {
            chkisLongName2.Checked = false;
            txtUNI_NO1.Text = "";
            txtUNI_NO2.Text = "";
            lblREG_NAME.Text = "";
            lblOWNER_CHINESE_NAME.Text = "";
            this.chkCancelRevert.Checked = false;

            base.sbRegScript.Append(BaseHelper.SetFocus("txtUNI_NO1"));
        }
        else
        {
            this.txtShopId.Text = "";
            this.chkCancelRevert.Checked = false;

            base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
        }            
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 清空公共部分文本信息
    /// </summary>
    private void ClearCommonPartText()
    {
        this.lblRecordNameText.Text = "";
        this.lblMerchantNameText.Text = "";
        this.lblBusinessNameText.Text = "";
        this.chboxP4.Checked = false;
        this.chboxP4A.Checked = false;

        //20190806-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy
        this.chkisLongName.Checked = false;
        this.chkisLongName_c.Checked = false;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 得到主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool GetMainframeData()
    {
        Hashtable hstInput = new Hashtable();
        hstInput.Add("MERCHANT_NO", this.txtShopId.Text.Trim());
        hstInput.Add("FUNCTION_CODE", "I");

        Hashtable hstExmsP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGR, hstInput, false, "11", eAgentInfo);
        hstExmsP4A["MERCHANT_NO"] = hstInput["MERCHANT_NO"];//* for_xml_test  模擬環境測試，正式環境可以不用賦值

        if (hstExmsP4A.Contains("HtgMsg"))
        {
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
            etMstType = eMstType.Select;
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

            if (hstExmsP4A["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            {
                base.strHostMsg += hstExmsP4A["HtgMsg"].ToString();
                base.strClientMsg += MessageHelper.GetMessage("01_01030100_005");
            }
            else
            {
                base.strClientMsg += hstExmsP4A["HtgMsg"].ToString();
            }

            return false;
        }

        base.strHostMsg += hstExmsP4A["HtgSuccess"].ToString();//*主機返回成功訊息

        Hashtable htInput = new Hashtable();
        htInput.Add("ACCT", this.txtShopId.Text.Trim());
        htInput.Add("FUNCTION_CODE", "I");
        htInput.Add("ORGN", "822");

        Hashtable hstPcmmP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCHR, htInput, false, "1", eAgentInfo);
        hstPcmmP4A["ACCT"] = htInput["ACCT"];//* for_xml_test  模擬環境測試，正式環境可以不用賦值

        if (!hstPcmmP4A.Contains("HtgMsg"))
        {
            //20211109_Ares_Jack_需可以解約還原
            // 20210527 EOS_AML(NOVA) 當點選查詢時，若該商店已解約時，不得異動特店資料
            if (hstPcmmP4A["STATUS_FLAG"].ToString() == "8" && hstPcmmP4A["DTE_USER_1"].ToString() != "00000000" && hstPcmmP4A["USER_CODE_2"].ToString() != "")
            {
                if (btnUnchainTask.BackColor != Color.FromArgb(255, 153, 0))//非解約作業
                {
                    base.sbRegScript.Append("alert('商店已解約不可再進行資料異動');");
                    base.strClientMsg += "商店已解約不可再進行資料異動";
                    return false;
                }
            }

            this.lblRecordNameText.Text = hstExmsP4A["REG_NAME"].ToString();
            this.lblBusinessNameText.Text = hstExmsP4A["BUSINESS_NAME"].ToString();
            this.lblMerchantNameText.Text = hstPcmmP4A["MERCHANT_NAME"].ToString();

            base.strHostMsg += hstPcmmP4A["HtgSuccess"].ToString();//*主機返回成功訊息
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_012");
            return true;
        }
        else
        {
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
            etMstType = eMstType.Select;
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

            if (hstPcmmP4A.Contains("HtgMsg"))
            {
                if (hstPcmmP4A["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += hstPcmmP4A["HtgMsg"].ToString();
                    base.strClientMsg += MessageHelper.GetMessage("01_01030100_006");
                }
                else
                {
                    base.strClientMsg += hstPcmmP4A["HtgMsg"].ToString();
                }
            }

            return false;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 得到panel中修改之前的值的集合
    /// </summary>
    /// <param name="panel">所在的panel</param>
    /// <returns>修改之前的值的集合</returns>
    private ArrayList GetBeforeUpdateValues(CustPanel panel)
    {
        ArrayList arrOldList = new ArrayList();
        foreach (System.Web.UI.Control control in pnlAccount.Controls)
        {
            if (control is CustTextBox)
            {
                CustTextBox txtBox = (CustTextBox)control;
                arrOldList.Add(txtBox.Text);
            }
        }

        return arrOldList;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 得到P4AndP4A的值
    /// </summary>
    /// <param name="eShop">SHOP表實體</param>
    private void GetP4AndP4AValue(CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop)
    {
        if (chboxP4.Checked == true)
        {
            eShop.P4 = 1;
        }
        else
        {
            eShop.P4 = 0;
        }

        if (chboxP4A.Checked == true)
        {
            eShop.p4A = 1;
        }
        else
        {
            eShop.p4A = 0;
        }
    }

    #endregion

    #region 20190806-RQ-2019-008595-002-長姓名需求 by Peggy
    /// <summary>
    /// 長姓名flag勾選時，checkbox控制事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void CheckBox_CheckedChanged(object sender, EventArgs e)
    {
        string lid = string.Empty;
        CustCheckBox chk = (CustCheckBox)sender;
        string CtrlName1 = string.Empty;//(原)人員名稱
        string CtrlName2 = string.Empty;//中文長姓名
        string CtrlName3 = string.Empty;//羅馬拼音

        switch (chk.ID.Trim())
        {
            case "chkisLongName"://負責人
                CtrlName1 = "txtUndertaker";//人員名稱
                CtrlName2 = "txtUndertaker_L";//中文長姓名
                CtrlName3 = "txtUndertaker_Pinyin";//馬拼音
                break;
            case "chkisLongName_c"://聯絡人
                CtrlName1 = "txtJunctionPerson";//人員名稱
                CtrlName2 = "txtJunctionPerson_L";//中文長姓名
                CtrlName3 = "txtJunctionPerson_Pinyin";//羅馬拼音
                break;
            case "chkisLongName2"://20190923 add by Peggy
                CtrlName1 = "txtPrincipalNameCH";//人員名稱
                CtrlName2 = "txtPrincipalName_L";//中文長姓名
                CtrlName3 = "txtPrincipalName_PINYIN";//羅馬拼音
                break;
        }

        CustTextBox contNAME = this.FindControl(CtrlName1) as CustTextBox;
        CustTextBox contNameL = this.FindControl(CtrlName2) as CustTextBox;
        CustTextBox contNamePinyin = this.FindControl(CtrlName3) as CustTextBox;
        
        contNAME.Enabled = !chk.Checked;
        //2021/03/11_Ares_Stanley-修正粗框問題
        contNAME.BackColor = chk.Checked ? Color.LightGray : Color.Empty;
        contNameL.Enabled = chk.Checked;
        contNameL.BackColor = chk.Checked ? Color.Empty : Color.LightGray;
        contNamePinyin.Enabled = chk.Checked;
        contNamePinyin.BackColor = chk.Checked ? Color.Empty : Color.LightGray;

        if (!chk.Checked)
        {
            contNAME.Focus();
            contNameL.Text = "";
            contNamePinyin.Text = "";
        }
        else
            contNameL.Focus();
    }


    /// <summary>
    /// 當長姓名flag勾選時，高管人名由中文長姓名取前4碼填入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void TextBox_TextChanged(object sender, EventArgs e)
    {
        string lid = string.Empty;
        CustTextBox txt = (CustTextBox)sender;

        string CtrlName = string.Empty;//長姓名flag
        string CtrlName1 = string.Empty;//原人員姓名

        switch (txt.ID.Trim())
        {
            case "txtUndertaker_L"://負責人中文長姓名
                CtrlName = "chkisLongName";//長姓名flag
                CtrlName1 = "txtUndertaker";//原人員姓名
                break;
            case "txtJunctionPerson_L"://聯絡人
                CtrlName = "chkisLongName_c";//長姓名flag
                CtrlName1 = "txtJunctionPerson";//原人員姓名
                break;            
            case "txtPrincipalName_L"://20190923 add by Peggy
                CtrlName = "chkisLongName2";//長姓名flag
                CtrlName1 = "txtPrincipalNameCH";//原人員姓名
                break;
        }

        CustCheckBox chk = this.FindControl(CtrlName) as CustCheckBox;
        CustTextBox contNAME = this.FindControl(CtrlName1) as CustTextBox;

        if (chk.Checked)
        {
            if (txt.Text.Trim().Length > 4)
            {
                contNAME.Text = txt.Text.Trim().Substring(0, 4);
            }
            else
            {
                contNAME.Text = txt.Text.Trim();
            }            
        }
    }
    #endregion

    
    /// <summary>
    /// 資料異動by統編 查詢Button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnInQuery_Click(object sender, EventArgs e)
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
        log.Customer_Id = this.txtUNI_NO1.Text.Trim() + txtUNI_NO2.Text.Trim();//查詢條件        
        log.Statement_Text = string.Format("CUSTOMER_ID:{0}|AC_NO:{1}|BRANCH_ID:{2}|ROLE_ID:{3}", log.Customer_Id, log.Account_Nbr, log.Branch_Nbr, log.Role_Id); //查詢條件內容: 用 | 區隔
        BRL_AP_LOG.Add(log);
        #endregion

        string strColumns = "";//*sql語句中查詢的欄位名
        
        EnabledControls(false);
        
        #region 給查詢區域欄位賦值

        if (GetMainframeData4JC69())
        {
            // 若獲取主機訊息成功
            // 基本資料異動
            if (this.pnlBasicByTaxno.Visible == true)
            {
                strColumns = @"[CORP_NO],[CORP_MCC],[CORP_ESTABLISH],[CORP_Organization],[CORP_CountryCode],[CORP_CountryStateCode],[REG_NAME_CH],[REG_NAME_EN]
                                ,[REG_ZIP_CODE],[REG_CITY],[REG_ADDR1],[REG_ADDR2],[CORP_TEL1],[CORP_TEL2],[CORP_TEL3],[PrincipalNameCH],[PrincipalName_L],[PrincipalName_PINYIN],[PrincipalNameEN]
                                ,[PrincipalIDNo],[PrincipalBirth],[PrincipalIssueDate],[PrincipalIssuePlace],[PrincipalReplaceType],[PrincipalCountryCode]
                                ,[PrincipalPassportNo],[PrincipalPassportExpdt],[PrincipalResidentNo],[PrincipalResidentExpdt],[Principal_TEL1],[Principal_TEL2],[Principal_TEL3]
                                ,[HouseholdCITY],[HouseholdADDR1],[HouseholdADDR2],[ARCHIVE_NO],[DOC_ID],[LAST_UPD_MAKER],[LAST_UPD_CHECKER],[LAST_UPD_BRANCH] ";
                string[] aa = strColumns.Split(',');
                SelectData(this.pnlBasicByTaxno, m_ShopBasicByTaxNoFlag, strColumns);

                //20210111-RQ-2020-021027-000-判斷原始電文之總公司統編若為空白，ALERT訊息警示
                if (string.IsNullOrEmpty(hstPcmmP4A["HEAD_CORP_NO"].ToString()) || hstPcmmP4A["HEAD_CORP_NO"].ToString().Trim().Length < 8)
                {
                    base.sbRegScript.Append("alert('【原始資料】總公司統編空白，請確認');$('#txtCORP_NO').focus();");
                    base.strClientMsg += "【原始資料】總公司統編空白，請確認";//抓取主機資料完畢
                }

                base.sbRegScript.Append(BaseHelper.SetFocus("txtCORP_NO"));

                if (!txtPrincipalName_L.Text.Trim().Equals(""))
                {
                    chkisLongName2.Checked = true;
                }
                CheckBox_CheckedChanged(chkisLongName2, null);
                if (isNew)
                {
                    txtDOC_ID.Text = DateTime.Now.ToString("yyyyMMddHHmmss");
                }

                txtREG_ZIP_CODE2.Enabled = false;
                txtREG_ZIP_CODE2.BackColor = Color.LightGray;
            }
        }
        else
        {
            EnabledControls(false);
        }

        #endregion
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/11
    /// 修改日期：2009/07/11 
    /// <summary>
    /// 得到主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool GetMainframeData4JC69()
    {
        Hashtable htInput = new Hashtable();
        htInput.Add("CORP_NO", this.txtUNI_NO1.Text.Trim());
        htInput.Add("CORP_SEQ", "0000");
        htInput.Add("FUNCTION_CODE", "I");

        //202012RC-20201112
        //Hashtable hstPcmmP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC69, htInput, false, "11", eAgentInfo);
        hstPcmmP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC69, htInput, false, "11", eAgentInfo);
        if (!hstPcmmP4A.Contains("HtgMsg"))
        {
            //2021/03/25_Ares_Stanley_移除空白字符避免斷行
            this.lblREG_NAME.Text = hstPcmmP4A["REG_NAME"].ToString().Trim();
            this.lblOWNER_CHINESE_NAME.Text = hstPcmmP4A["OWNER_CHINESE_NAME"].ToString().Trim();

            base.strHostMsg += hstPcmmP4A["HtgSuccess"].ToString();//*主機返回成功訊息
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_012");//抓取主機資料完畢
            
            return true;
        }
        else
        {
            //鍵檔GUI訊息呈現方式
            etMstType = eMstType.Select;

            if (hstPcmmP4A.Contains("HtgMsg"))
            {
                if (hstPcmmP4A["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += hstPcmmP4A["HtgMsg"].ToString();
                    //base.strClientMsg += MessageHelper.GetMessage("01_01030100_006");
                }
                else
                {
                    base.strClientMsg += hstPcmmP4A["HtgMsg"].ToString();
                }
            }
            return false;
        }
    }

    protected void btnSubmitByTaxno_Click(object sender, EventArgs e)
    {
        #region 欄位檢核
        //20210111-RQ-2020-021027-000-判斷原始電文之總公司統編不得為空白
        if (GetMainframeData4JC69())
        {
            if ((string.IsNullOrEmpty(hstPcmmP4A["HEAD_CORP_NO"].ToString()) || hstPcmmP4A["HEAD_CORP_NO"].ToString().Trim().Length < 8) && txtCORP_NO.Text.Trim().Equals(""))
            {
                base.sbRegScript.Append("alert('【原始資料】總公司統編空白，請輸入總公司統編');$('#txtCORP_NO').focus();");
                return;
            }
        }

        // 檢查設立日期
        string establishDate = this.txtEstablish.Text.Trim();
        if (!string.IsNullOrEmpty(txtCORP_ESTABLISH.Text.Trim()) && txtCORP_ESTABLISH.Text.Trim().Length == 7)
        {
            if (!checkDateTime(txtCORP_ESTABLISH.Text.Trim()))
            {
                base.sbRegScript.Append("alert('設立日期格式錯誤');$('#txtCORP_ESTABLISH').focus();");
                return;
            }
        }

        //20201021-202012RC 統一證號碼(新+舊)邏輯檢核
        if (!string.IsNullOrEmpty(txtPrincipalResidentNo.Text) && !txtPrincipalResidentNo.Text.Trim().ToUpper().Equals("X"))//20201214
        {
            if (txtPrincipalResidentNo.Text.Length != 10)
            {
                base.sbRegScript.Append("alert('統一證號須為10碼');$('#txtPrincipalResidentNo').focus();");
                return;
            }
            else
            {
                if (!CheckResidentID(txtPrincipalResidentNo.Text.Trim()))//20200515統一證號(新+舊)檢核
                {
                    base.sbRegScript.Append("alert('統一證號輸入錯誤，請重新輸入');$('#txtPrincipalResidentNo').focus();");
                    return;
                }
            }
        }

        //20220106_Ares_Jack_國籍不得選無
        if (this.txtCORP_CountryCode.Text == "無")
        {
            base.sbRegScript.Append("alert('註冊國籍不得選無');$('#txtCORP_CountryCode').focus();");
            return;
        }
        if (this.txtPrincipalCountryCode.Text == "無")
        {
            base.sbRegScript.Append("alert('負責人國籍不得選無');$('#txtPrincipalCountryCode').focus();");
            return;
        }

        // 檢查護照效期
        if (!string.IsNullOrEmpty(txtPrincipalPassportExpdt.Text.Trim()) && txtPrincipalPassportExpdt.Text.Trim().ToUpper() != "X")
        {
            if (!checkDateTime(txtPrincipalPassportExpdt.Text.Trim()))
            {
                base.sbRegScript.Append("alert('護照效期格式錯誤');$('#txtPrincipalPassportExpdt').focus();");
                return;
            }
        }

        // 檢查居留效期
        if (!string.IsNullOrEmpty(txtPrincipalResidentExpdt.Text.Trim()) && txtPrincipalResidentExpdt.Text.Trim().ToUpper() != "X")
        {
            if (!checkDateTime(txtPrincipalResidentExpdt.Text.Trim()))
            {
                base.sbRegScript.Append("alert('統一證號效期格式錯誤');$('#txtPrincipalResidentExpdt').focus();");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
                return;
            }
        }
        if ((!txtPrincipalNameCH.Text.Trim().Equals("") || !txtPrincipalName_L.Text.Trim().Equals("")) && txtPrincipalIDNo.Text.Trim().Equals(""))
        {
            base.sbRegScript.Append("alert('修改姓名時，請輸入負責人身份ID');$('#txtPrincipalIDNo').focus();");
            return;
        }

        #region 長姓名檢核
        if (chkisLongName2.Checked)
        {
            if (txtPrincipalIDNo.Text.Trim().Equals(""))//異動長姓名時，身份ID為KEY值不得為空白
            {
                base.sbRegScript.Append("alert('負責人長姓名FLAG勾選時，請輸入負責人身份ID');$('#txtPrincipalIDNo').focus();");
                return;
            }

            if (txtPrincipalName_L.Text.Trim().Equals(""))
            {
                base.sbRegScript.Append("alert('負責人長姓名FLAG勾選時，請輸入負責人中文長姓名');$('#txtPrincipalName_L').focus();");
                return;
            }

            if (txtPrincipalName_PINYIN.Text.Trim().Equals(""))
            {
                base.sbRegScript.Append("alert('負責人長姓名FLAG勾選時，請輸入羅馬拼音');$('#txtPrincipalName_PINYIN').focus();");
                return;
            }

            if ((ToWide(txtPrincipalName_L.Text.Trim()).Length + LongNameRomaClean(txtPrincipalName_PINYIN.Text).Trim().Length) < 5)
            {
                base.sbRegScript.Append("alert('負責人長姓名FLAG勾選時，負責人姓名(中文+羅馬拼音)需超過4個字以上');$('#txtPrincipalName_L').focus();");
                return;
            }
        }
        #endregion

        #region 地址檢核
        string strScript = "";
        EntitySet<EntitySZIP> SZIPSetOne = null;
        EntitySet<EntitySZIP> SZIPSetTwo = null;

        //*檢核商店登記地址所填入【商店登記地址(2)】是否存在（若填入為空，則不檢核）
        if (txtREG_CITY.Text.Trim() != "")
        {
            try
            {
                SZIPSetOne = BRSZIP.SelectEntitySet(txtREG_CITY.Text.Trim());

                if (SZIPSetOne.Count != 0)
                {
                    strScript = "if(confirm('" + MessageHelper.GetMessage("01_01030200_003") + "')) {$('#btnSubmitByTaxno_Click').click();}else{document.getElementById('txtREG_CITY').focus();}";
                }
            }
            catch
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                return;
            }
        }

        //*檢核戶籍地址所填入【戶籍地址(2)】是否存在（若填入為空，則不檢核）
        if (txtHouseholdCITY.Text.Trim() != "")
        {
            try
            {
                SZIPSetTwo = BRSZIP.SelectEntitySet(txtHouseholdCITY.Text.Trim());

                if (SZIPSetTwo.Count != 0)
                {
                    strScript = "if(confirm('" + MessageHelper.GetMessage("01_01030200_003") + "')) {$('#btnSubmitByTaxno_Click').click();}else{document.getElementById('txtHouseholdCITY').focus();}";
                }
            }
            catch
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                return;
            }
        }
        #endregion

        if (CSIPKeyInGUI.BusinessRules_new.BRSHOP_CHANGE.checkUniNoInFlow(this.txtUNI_NO1.Text.Trim(), "*") && !isNew)
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01030100_007");//此統一編號已存在相同資料，請勿重覆鍵檔
            return;
        }

        // 20210527 EOS_AML(NOVA) 檢查最後異動分行 by Ares Dennis
        string LAST_UPD_BRANCH = this.txtLAST_UPD_BRANCH3.Text.Trim();
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
        string LAST_UPD_MAKER = this.txtLAST_UPD_MAKER3.Text.Trim();
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
        string LAST_UPD_CHECKER = this.txtLAST_UPD_CHECKER3.Text.Trim();
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
        string address = this.txtREG_CITY.Text.Trim();
        if (!string.IsNullOrEmpty(address) && !checkREG_ZIP_CODE(address))
        {
            base.sbRegScript.Append("alert('地址查無郵遞區號，請輸入正確地址或請聯繫MFA更新');");
            return;
        }

        #endregion

        #region 為EntitySHOP_CHANGE賦值
        CSIPKeyInGUI.EntityLayer_new.EntitySHOP_CHANGE eShopChange = new CSIPKeyInGUI.EntityLayer_new.EntitySHOP_CHANGE();
        eShopChange.UNI_NO1 = txtUNI_NO1.Text.Trim();
        eShopChange.UNI_NO2 = txtUNI_NO2.Text.Trim();
        eShopChange.CORP_NO = txtCORP_NO.Text.Trim();
        eShopChange.CORP_MCC = txtCORP_MCC.Text.Trim();
        eShopChange.CORP_Organization = txtCORP_Organization.Text.Trim();
        eShopChange.CORP_CountryCode = txtCORP_CountryCode.Text.Trim();
        eShopChange.CORP_CountryStateCode = txtCORP_CountryStateCode.Text.Trim();
        eShopChange.CORP_ESTABLISH = txtCORP_ESTABLISH.Text.Trim();
        eShopChange.REG_NAME_CH = txtREG_NAME_CH.Text.Trim();
        eShopChange.REG_NAME_EN = txtREG_NAME_EN.Text.Trim();
        eShopChange.REG_CITY = txtREG_CITY.Text.Trim();
        eShopChange.REG_ADDR1 = txtREG_ADDR1.Text.Trim();
        eShopChange.REG_ADDR2 = txtREG_ADDR2.Text.Trim();
        eShopChange.CORP_TEL1 = txtCORP_TEL1.Text.Trim();
        eShopChange.CORP_TEL2 = txtCORP_TEL2.Text.Trim();
        eShopChange.CORP_TEL3 = txtCORP_TEL3.Text.Trim();
        eShopChange.PrincipalNameCH = txtPrincipalNameCH.Text.Trim();
        eShopChange.PrincipalName_L = txtPrincipalName_L.Text.Trim();
        eShopChange.PrincipalName_PINYIN = txtPrincipalName_PINYIN.Text.Trim();
        eShopChange.PrincipalNameEN = txtPrincipalNameEN.Text.Trim();
        eShopChange.PrincipalIDNo = txtPrincipalIDNo.Text.Trim();
        eShopChange.PrincipalBirth = txtPrincipalBirth.Text.Trim();
        eShopChange.PrincipalIssueDate = txtPrincipalIssueDate.Text.Trim();
        eShopChange.PrincipalIssuePlace = txtPrincipalIssuePlace.Text.Trim();
        eShopChange.PrincipalReplaceType = txtReplaceType.Text.Trim();
        eShopChange.PrincipalCountryCode = txtPrincipalCountryCode.Text.Trim();
        eShopChange.PrincipalPassportNo = txtPrincipalPassportNo.Text.Trim();
        eShopChange.PrincipalPassportExpdt = txtPrincipalPassportExpdt.Text.Trim().ToUpper();
        eShopChange.PrincipalResidentNo = txtPrincipalResidentNo.Text.Trim();
        eShopChange.PrincipalResidentExpdt = txtPrincipalResidentExpdt.Text.Trim().ToUpper();
        eShopChange.Principal_TEL1 = txtPrincipal_TEL1.Text.Trim();
        eShopChange.Principal_TEL2 = txtPrincipal_TEL2.Text.Trim();
        eShopChange.Principal_TEL3 = txtPrincipal_TEL3.Text.Trim();
        eShopChange.HouseholdCITY = txtHouseholdCITY.Text.Trim();
        eShopChange.HouseholdADDR1 = txtHouseholdADDR1.Text.Trim();
        eShopChange.HouseholdADDR2 = txtHouseholdADDR2.Text.Trim();
        //eShopChange.BANK_NAME = txtBANK_NAME.Text.Trim();
        //eShopChange.BANK_BRANCH = txtBANK_BRANCH.Text.Trim();
        //eShopChange.BANK_ACCT1 = txtBANK_ACCT1.Text.Trim();
        //eShopChange.BANK_ACCT2 = txtBANK_ACCT2.Text.Trim();
        //eShopChange.BANK_ACCT_NAME = txtBANK_ACCT_NAME.Text.Trim();
        eShopChange.KeyIn_Flag = "1";
        eShopChange.MOD_DATE = DateTime.Now.ToString("yyyyMMdd");
        eShopChange.MOD_USER = eAgentInfo.agent_id;
        eShopChange.isCHECK = "";
        eShopChange.ARCHIVE_NO = txtARCHIVE_NO.Text.Trim();
        //eShopChange.CHECK_CODE = txtCHECK_CODE.Text.Trim();
        eShopChange.DOC_ID = txtDOC_ID.Text.Trim();
        // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis        
        eShopChange.REG_ZIP_CODE = txtREG_ZIP_CODE2.Text.Trim();// 地址郵遞區號
        eShopChange.LAST_UPD_BRANCH = txtLAST_UPD_BRANCH3.Text.Trim();// 資料最後異動分行
        eShopChange.LAST_UPD_CHECKER = txtLAST_UPD_CHECKER3.Text.Trim();// 資料最後異動-CHECKER
        eShopChange.LAST_UPD_MAKER = txtLAST_UPD_MAKER3.Text.Trim();// 資料最後異動-MAKER
        #endregion

        #region 為EntityShopChange_LOG賦值
        CSIPKeyInGUI.EntityLayer_new.EntityShopChange_LOG eChangeLog = new CSIPKeyInGUI.EntityLayer_new.EntityShopChange_LOG();
        eChangeLog.DOC_ID = txtDOC_ID.Text.Trim();
        eChangeLog.CORP_NO = txtUNI_NO1.Text.Trim();
        eChangeLog.CORP_SEQ = "0000";
        eChangeLog.KeyinFLAG = "1";
        eChangeLog.MOD_USER = eAgentInfo.agent_id;
        eChangeLog.MOD_DATE = DateTime.Now.ToString("yyyyMMdd");
        #endregion

        if (CSIPKeyInGUI.BusinessRules_new.BRSHOP_CHANGE.Insert(eShopChange, this.txtUNI_NO1.Text.Trim(), m_KeyFlag))
        {
            BRShopChange_LOG.Insert(eChangeLog, txtDOC_ID.Text.Trim(), m_KeyFlag);

            base.strClientMsg += MessageHelper.GetMessage("01_00000000_003");
            strClientMsg += "一KEY鍵檔同仁：" + eAgentInfo.agent_id;


            ClearAll();
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_004");
        }
        sbRegScript.Append(@"alert('" + base.strClientMsg + "');");
    }

    protected void txtCodeType_TextChanged(object sender, EventArgs e)
    {
        CustTextBox txt = (CustTextBox)sender;
        txt.Text = txt.Text.ToUpper();
        if (txt.Text.Trim().Equals(""))//欄位為空時不進行檢核
        {
            return;
        }

        string _codeType = string.Empty;
        string Col_codeName = string.Empty;
        bool isReturn = false;

        switch (txt.ID.Trim())
        {
            case "txtCORP_CountryCode"://總公司註冊國籍
            case "txtPrincipalCountryCode"://負責人國籍       
                _codeType = "1";
                break;
            case "txtCORP_Organization"://法律形式
                _codeType = "2";
                break;
            case "txtCORP_MCC":
                _codeType = "3"; //AML行業別
                break;
            default:
                _codeType = "";
                break;
        }

        checkCodeType(_codeType, txt.ID, isReturn, Col_codeName);

    }
}