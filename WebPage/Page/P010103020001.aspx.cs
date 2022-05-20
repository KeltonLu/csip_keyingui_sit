//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：特店資料異動 二次鍵檔
//*  創建日期：2009/07/24
//*  修改記錄：2021/03/11_Ares_Stanley-修正粗框問題
//*<author>            <time>            <TaskID>                <desc>
//*lvliangzhao     2010/05/14        20090019           於「特店資料異動-費率」下新增異動銀聯卡費率功能
//*******************************************************************
//20160628 (U) by Tank, add status 10 11
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
using Framework.Common.Logging;
using System.Drawing;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.EntityLayer_new;
using System.Collections.Generic;

/// <summary>
/// 特店資料異動 二次鍵檔
/// </summary>
public partial class P010103020001 : PageBase
{
    #region 變數區
    /// <summary>
    /// 一KEY標識
    /// </summary>
    private string m_KeyFlag = "1";

    /// <summary>
    /// 二KEY標識
    /// </summary>
    private string m_TwoKeyFlag = "2";

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
    /// 記錄異動主機成功數
    /// </summary>
    private int m_iCount;

    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;

    private string strDOCID = string.Empty;
    //20191023 修改：SOC所需資訊  by Peggy
    private structPageInfo sPageInfo;//*記錄網頁訊息

    private string involve = "78360020,78360021,78360023,78360024,78360025,78360026,78360027,78360028,78360029";//自然人
    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
            btnBasicData.BackColor = Color.FromArgb(255, 153, 0);

            //*add by lvliangzhao 20090019 20100326 start
            SetFeeLableValue();
            //*add by lvliangzhao 20090019 20100326 end
            SetControlsText();
            EnabledControls(false);
            LoadDropDownList();
            SetControlsTextColor();//20190822 修改：搬到page_load時執行就好 by Peggy
            SetDropStateCode();//20190923 add by Peggy
        }

        base.strClientMsg += ""; //* 【端末訊息】顯示內容清空
        base.strHostMsg += "";//* 【主機訊息】顯示內容清空
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//20191023 修改：SOC所需資訊  by Peggy

    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
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
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
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
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
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
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
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
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
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
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 查詢事件
    /// </summary>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        string strColumns = "";
        EnabledControls(false);

        if (this.txtShopId.Text.Trim().Substring(2, 1) == "5")
        {
            this.chboxP4.Checked = true;
            this.chboxP4A.Checked = true;
        }
        else
        {
            this.chboxP4A.Checked = true;
        }

        if (GetMainframeData())
        {
            // 若獲取主機訊息成功
            // 基本資料異動
            //20190805-長姓名需求增加6個欄位：isLongName ,Undertaker_L ,Undertaker_Pinyin, isLongName_c ,JunctionPerson_L, JunctionPerson_Pinyin by Peggy
            if (this.pnlBasic.Visible == true)
            {
                if (SelectOneKeyData(m_ShopBasicFlag))
                {
                    strColumns = @"headquarter_corpno,aml_cc,establish,organization,
                               record_name, business_name, MERCHANT_NAME, english_name, undertaker, undertaker_id,Undertaker_L ,Undertaker_Pinyin, 
                               undertaker_EngName,undertaker_tel1, undertaker_tel2, undertaker_tel3, 
                               country_code, passport_no, passport_expdt, resident_no, resident_expdt,
	                           undertaker_add1, undertaker_add2, undertaker_add3, realperson, realperson_id, 
	                           junctionperson, realperson_tel1, realperson_tel2, realperson_tel3,JunctionPerson_L, JunctionPerson_Pinyin, 
	                           junctionperson_tel1, junctionperson_tel2, junctionperson_tel3, 
	                           junctionperson_fax1, junctionperson_fax2, 
	                           realperson_add1, realperson_add2, realperson_add3, 
	                           email, 
	                           REG_ZIP_CODE, junctionperson_recadd1, junctionperson_recadd2, junctionperson_recadd3, 
	                           realadd_zip, junctionperson_realadd1, junctionperson_realadd2, junctionperson_realadd3, 
	                           zip, commadd1, comaddr2, introduce, introduce_flag, invoice_cycle, HOLD_STMT, 
	                           ReqAppro, black, class, LAST_UPD_MAKER, LAST_UPD_CHECKER, LAST_UPD_BRANCH, p4, p4a, REG_ZIP_CODE";
                    string[] aa = strColumns.Split(',');
                    SelectData(this.pnlBasic, m_ShopBasicFlag, strColumns);
                    //base.sbRegScript.Append(BaseHelper.SetFocus("txtRecordNameTwo"));
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtUniNo"));

                    //20190806-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy
                    if (!txtUndertaker_L.Text.Trim().Equals(""))
                    {
                        chkisLongName.Checked = true;
                    }
                    else
                    {
                        chkisLongName.Checked = false;
                    }


                    if (!txtJunctionPerson_L.Text.Trim().Equals(""))
                    {
                        chkisLongName_c.Checked = true;
                    }
                    else
                    {
                        chkisLongName_c.Checked = false;
                    }

                    CheckBox_CheckedChanged(chkisLongName, null);
                    CheckBox_CheckedChanged(chkisLongName_c, null);

                    txtREG_ZIP_CODE.Enabled = false;
                    txtREG_ZIP_CODE.BackColor = Color.LightGray;

                }
                else
                {
                    CommonFunction.SetControlsEnabled(pnlBasic, false);
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
                    return;
                }
            }

            // 費率異動(作業畫面：P4A)
            if (this.pnlFee.Visible == true)
            {
                if (SelectOneKeyData(m_ShopFeeFlag))
                {
                    //*modified by lvliangzhao 20090019 20100326 start 增加查詢欄位字符串
                    strColumns = @"addjcb, cup_status1, cup_rate1, addjcb_us, cup_status2, cup_rate2, addjcb_notus, cup_status3, 
                                   cup_rate3, moddate, cup_status4, cup_rate4, page1, cup_status5, cup_rate5, page2, cup_status6, 
	                               cup_rate6, page3, cup_status7, cup_rate7, page4, rate09, cup_status8, cup_rate8, rate02N, 
	                               rate10, cup_status9, cup_rate9, rate03V, rate11, cup_status10, cup_rate10, rate04V, rate12, 
	                               rate05M, rate13, rate06M, rate14, rate07J, rate15, rate08J, p4, p4a";
                    //*modified by lvliangzhao 20090019 20100326 end
                    SelectData(this.pnlFee, m_ShopFeeFlag, strColumns);
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtAddjcb"));
                }
                else
                {
                    CommonFunction.SetControlsEnabled(pnlFee, false);
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
                    return;
                }
            }

            // 帳號異動(作業畫面：P4/P4A)
            if (this.pnlAccount.Visible == true)
            {
                if (SelectOneKeyData(m_ShopAccountFlag))
                {
                    strColumns = "bank_name, branch_name, account, check_num, account1, account2, p4, p4a";
                    SelectData(this.pnlAccount, m_ShopAccountFlag, strColumns);
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtBankName"));
                }
                else
                {
                    CommonFunction.SetControlsEnabled(pnlAccount, false);
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
                    return;
                }
            }

            // 解約作業(作業畫面：P4/P4A)
            if (this.pnlCancelTask.Visible == true)
            {                
                if (SelectOneKeyData(m_ShopCancelFlag))
                {
                    strColumns = "cancel_code1, cancel_date, cancel_code2, LAST_UPD_MAKER, LAST_UPD_CHECKER, LAST_UPD_BRANCH, p4, p4a";
                    SelectData(this.pnlCancelTask, m_ShopCancelFlag, strColumns);

                    Hashtable htInput = new Hashtable();
                    htInput.Add("ACCT", this.txtShopId.Text.Trim());
                    htInput.Add("FUNCTION_CODE", "I");
                    htInput.Add("ORGN", "822");
                    Hashtable hstPcmmP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCHR, htInput, false, "1", eAgentInfo);

                    //20220316_Ares_Jack_txtCancelCode1文字預設黑色
                    this.txtCancelCode1.ForeColor = Color.Black;
                    //20211112_Ares_Jack_商店已解約
                    if (hstPcmmP4A["STATUS_FLAG"].ToString() == "8")
                    {
                        //20211224_Ares_Jack_ cancel_code1等於1解約還原打勾
                        DataSet ds_cancel_code1_2key = CSIPKeyInGUI.BusinessRules.BRSHOP.Select(txtShopId.Text.Trim(), "2", "4", "cancel_code1");
                        if (ds_cancel_code1_2key.Tables[0].Rows.Count > 0)
                        {
                            if (ds_cancel_code1_2key.Tables[0].Rows[0][0].ToString().Trim() == "1")
                                this.chkCancelRevert.Checked = true;
                            if (ds_cancel_code1_2key.Tables[0].Rows[0][0].ToString().Trim() == "")
                            {
                                DataSet ds_cancel_code1 = CSIPKeyInGUI.BusinessRules.BRSHOP.Select(txtShopId.Text.Trim(), "1", "4", "cancel_code1");
                                if (ds_cancel_code1.Tables[0].Rows.Count > 0)
                                {
                                    if (ds_cancel_code1.Tables[0].Rows[0][0].ToString().Trim() == "1")
                                        this.chkCancelRevert.Checked = true;
                                }
                            }
                        }
                        else
                        {
                            DataSet ds_cancel_code1 = CSIPKeyInGUI.BusinessRules.BRSHOP.Select(txtShopId.Text.Trim(), "1", "4", "cancel_code1");
                            if (ds_cancel_code1.Tables[0].Rows.Count > 0)
                            {
                                if (ds_cancel_code1.Tables[0].Rows[0][0].ToString().Trim() == "1")
                                    this.chkCancelRevert.Checked = true;
                            }
                        }
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
                else
                {
                    CommonFunction.SetControlsEnabled(pnlCancelTask, false);
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
                    return;
                }
            }

            // 機器資料(作業畫面：P4A)
            if (this.pnlMachineData.Visible == true)
            {
                if (SelectOneKeyData(m_ShopMaFlag))
                {
                    strColumns = @"imp1_type, imp1, imp2_type, imp2, imp_money, pos1_type, pos1, pos2_type, pos2, pos_money, 
                                   edc1_type, edc1, edc_type, edc, edc_money, p4, p4a";
                    SelectData(this.pnlMachineData, m_ShopMaFlag, strColumns);
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtImp1Type"));
                }
                else
                {
                    CommonFunction.SetControlsEnabled(pnlMachineData, false);
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
                    return;
                }
            }
        }
        else
        {
            EnabledControls(false);
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 基本資料異動提交事件
    /// </summary>
    protected void btnSubmitClick(object sender, EventArgs e)
    {
        string strScript = "";

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

        EntitySet<EntitySZIP> SZIPSetOne = null;
        EntitySet<EntitySZIP> SZIPSetTwo = null;

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
                    strScript = "if(confirm('" + MessageHelper.GetMessage("01_01030200_003") + "')) {$('#btnHiden').click();}else{document.getElementById('txtJunctionPersonRealadd1').focus();}";
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
                    strScript = "if(confirm('" + MessageHelper.GetMessage("01_01030200_003") + "')) {$('#btnHiden').click();}else{document.getElementById('txtCommadd1').focus();}";
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
                        strScript = "if(confirm('" + MessageHelper.GetMessage("01_01030200_003") + "')) {$('#btnHiden').click();}else{document.getElementById('txtJunctionPersonRealadd1').focus();}";
                    }
                    else
                    {
                        strScript = "if(confirm('" + MessageHelper.GetMessage("01_01030200_003") + "')) {$('#btnHiden').click();}else{document.getElementById('txtCommadd1').focus();}";
                    }
                }
            }

            base.sbRegScript.Append(strScript);
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 基本資料異動提交事件
    /// </summary>
    protected void btnHiden_Click(object sender, EventArgs e)
    {
        #region 為 EntitySHOP 賦值
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
        eShop.KeyIn_Flag = m_TwoKeyFlag;
        eShop.mod_date = DateTime.Now.ToString("yyyyMMdd");
        eShop.shop_type = m_ShopBasicFlag;
        eShop.Change_Item = m_ShopBasicFlag;
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

        //20190806-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy↓
        eShop.Undertaker_L = txtUndertaker_L.Text.Trim(); //負責人中文長姓名
        eShop.Undertaker_Pinyin = txtUndertaker_Pinyin.Text.Trim(); //負責人羅馬拼音
        eShop.JunctionPerson_L = txtJunctionPerson_L.Text.Trim(); //聯絡人中文長姓名
        eShop.JunctionPerson_Pinyin = txtJunctionPerson_Pinyin.Text.Trim(); //聯絡人羅馬拼音
        //20190806-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy↑

        // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis    
        eShop.REG_ZIP_CODE = txtREG_ZIP_CODE.Text.Trim();// 地址郵遞區號
        eShop.LAST_UPD_BRANCH = txtLAST_UPD_BRANCH.Text.Trim();// 資料最後異動分行
        eShop.LAST_UPD_CHECKER = txtLAST_UPD_CHECKER.Text.Trim();// 資料最後異動-CHECKER
        eShop.LAST_UPD_MAKER = txtLAST_UPD_MAKER.Text.Trim();// 資料最後異動-MAKER
        #endregion

        if (CSIPKeyInGUI.BusinessRules_new.BRSHOP.Insert(eShop, this.txtShopId.Text.Trim(), m_TwoKeyFlag, m_ShopBasicFlag))
        {
            //*資料異動成功
            //欄位順序與比對畫面順序相關，要注意
            string strColumns = @"headquarter_corpno,aml_cc,establish,organization,
                               record_name, business_name, MERCHANT_NAME, english_name, undertaker, undertaker_id,Undertaker_L ,Undertaker_Pinyin,  
                               undertaker_EngName,undertaker_tel1, undertaker_tel2, undertaker_tel3,
                               country_code, passport_no, passport_expdt, resident_no, resident_expdt,
	                           undertaker_add1, undertaker_add2, undertaker_add3, realperson, realperson_id, 
	                           junctionperson, realperson_tel1, realperson_tel2, realperson_tel3,JunctionPerson_L, JunctionPerson_Pinyin, 
	                           junctionperson_tel1, junctionperson_tel2, junctionperson_tel3, 
	                           junctionperson_fax1, junctionperson_fax2, 
	                           realperson_add1, realperson_add2, realperson_add3, 
	                           email, 
	                           REG_ZIP_CODE, junctionperson_recadd1, junctionperson_recadd2, junctionperson_recadd3, 
	                           realadd_zip, junctionperson_realadd1, junctionperson_realadd2, junctionperson_realadd3, 
	                           zip, commadd1, comaddr2, introduce, introduce_flag, invoice_cycle, HOLD_STMT, 
	                           ReqAppro, black, class, LAST_UPD_MAKER, LAST_UPD_CHECKER, LAST_UPD_BRANCH, p4, p4a";

            if (CompareKey(pnlBasic, eShop, m_ShopBasicFlag, strColumns))
            {
                InsertShopRpt();
                InsertShop1KeyLog();

                //*更新資料檔trans_num
                if (!BRTRANS_NUM.UpdateTransNum("C02"))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }

                if (UpdateMainFrameData(SubmitType.BasicSubmit))
                {
                    if (m_iCount > 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("01_00000000_005");
                    }

                    ClearAll();
                }
                else
                {
                    CommonFunction.SetEnabled(pnlBasic, false);//*將網頁中的提交按鈕和輸入框disable 
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
                }
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_006");
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 費率異動提交事件
    /// </summary>
    protected void btnFeeSubmit_Click(object sender, EventArgs e)
    {
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

        //*add by lvliangzhao 20090019 20100326 start 銀聯卡欄位賦值
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
        //*add by lvliangzhao 20090019 20100326 end

        eShop.user_id = eAgentInfo.agent_id;
        eShop.KeyIn_Flag = m_TwoKeyFlag;
        eShop.mod_date = DateTime.Now.ToString("yyyyMMdd");
        eShop.shop_type = m_ShopFeeFlag;
        eShop.Change_Item = m_ShopFeeFlag;
        GetP4AndP4AValue(eShop);

        if (CSIPKeyInGUI.BusinessRules_new.BRSHOP.Insert(eShop, this.txtShopId.Text.Trim(), m_TwoKeyFlag, m_ShopFeeFlag))
        {
            //*modified by lvliangzhao 20090019 20100326 start 增加查詢欄位字符串
            string strColumns = @"addjcb, cup_status1, cup_rate1, addjcb_us, cup_status2, cup_rate2, addjcb_notus, cup_status3, 
                                  cup_rate3, moddate, cup_status4, cup_rate4, page1, cup_status5, cup_rate5, page2, cup_status6, 
                                  cup_rate6, page3, cup_status7, cup_rate7, page4, rate09, cup_status8, cup_rate8, rate02N, 
                                  rate10, cup_status9, cup_rate9, rate03V, rate11, cup_status10, cup_rate10, rate04V, rate12, 
                                  rate05M, rate13, rate06M, rate14, rate07J, rate15, rate08J, p4, p4a";
            //*modified by lvliangzhao 20090019 20100326 end

            if (CompareKey(pnlFee, eShop, m_ShopFeeFlag, strColumns))
            {
                //*更新資料檔trans_num
                if (!BRTRANS_NUM.UpdateTransNum("C02"))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }

                if (UpdateMainFrameData(SubmitType.FeeSubmit))
                {
                    if (m_iCount > 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("01_00000000_005");
                    }

                    ClearAll();
                }
                else
                {
                    CommonFunction.SetEnabled(pnlFee, false);//*將網頁中的提交按鈕和輸入框disable 
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
                }
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_006");
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 帳號異動提交事件
    /// </summary>
    protected void btnAccountSubmit_Click(object sender, EventArgs e)
    {
        CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop = GetEntity();
        eShop.shop_id = this.txtShopId.Text.Trim();
        eShop.bank_name = this.txtBankName.Text.Trim();
        eShop.branch_name = this.txtBranchName.Text.Trim();
        eShop.account = this.txtAccount.Text.Trim();
        eShop.check_num = this.txtCheckNum.Text.Trim();
        eShop.account1 = this.txtAccount1.Text.Trim();
        eShop.account2 = this.txtAccount2.Text.Trim();
        eShop.user_id = eAgentInfo.agent_id;
        eShop.KeyIn_Flag = m_TwoKeyFlag;
        eShop.mod_date = DateTime.Now.ToString("yyyyMMdd");
        eShop.shop_type = m_ShopAccountFlag;
        eShop.Change_Item = m_ShopAccountFlag;
        GetP4AndP4AValue(eShop);

        if (CSIPKeyInGUI.BusinessRules_new.BRSHOP.Insert(eShop, this.txtShopId.Text.Trim(), m_TwoKeyFlag, m_ShopAccountFlag))
        {
            string strColumns = "bank_name, branch_name, account, check_num, account1, account2, p4, p4a";

            if (CompareKey(pnlAccount, eShop, m_ShopAccountFlag, strColumns))
            {
                //*更新資料檔trans_num
                if (!BRTRANS_NUM.UpdateTransNum("C02"))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }
                if (UpdateMainFrameData(SubmitType.AccountSubmit))
                {
                    if (m_iCount > 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("01_00000000_005");
                    }

                    ClearAll();
                }
                else
                {
                    CommonFunction.SetEnabled(pnlAccount, false);//*將網頁中的提交按鈕和輸入框disable 
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
                }
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_006");
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
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

        //20211227_Ares_Jack_比較解約還原一二key
        Hashtable htInput = new Hashtable();
        htInput.Add("ACCT", this.txtShopId.Text.Trim());
        htInput.Add("FUNCTION_CODE", "I");
        htInput.Add("ORGN", "822");
        Hashtable hstPcmmP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCHR, htInput, false, "1", eAgentInfo);
        if (hstPcmmP4A["STATUS_FLAG"].ToString() == "8")//已解約
        {
            DataSet ds_cancel_code1 = CSIPKeyInGUI.BusinessRules.BRSHOP.Select(txtShopId.Text.Trim(), "1", "4", "cancel_code1");//一key資料
            if (ds_cancel_code1.Tables[0].Rows.Count > 0)
            {
                if (ds_cancel_code1.Tables[0].Rows[0][0].ToString().Trim() == "1")
                {
                    if (this.chkCancelRevert.Checked == false)
                    {
                        base.sbRegScript.Append("alert('一次鍵檔、二次鍵檔資料不吻合.');");
                        this.chkCancelRevert.BackColor = Color.Red;
                        return;
                    }
                        
                }
                if (ds_cancel_code1.Tables[0].Rows[0][0].ToString().Trim() == "")
                {
                    if (this.chkCancelRevert.Checked == true)
                    {
                        base.sbRegScript.Append("alert('一次鍵檔、二次鍵檔資料不吻合.');");
                        this.chkCancelRevert.BackColor = Color.Red;
                        return;
                    }
                }

                if (this.chkCancelRevert.Checked)
                    this.txtCancelCode1.Text = "1";
                else
                    this.txtCancelCode1.Text = "";
            }
        }
        
        CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop = GetEntity();
        eShop.shop_id = this.txtShopId.Text.Trim();
        eShop.cancel_code1 = this.txtCancelCode1.Text.Trim();
        eShop.cancel_date = this.txtCancelDate.Text.Trim();
        eShop.cancel_code2 = this.txtCancelCode2.Text.Trim().ToUpper();
        eShop.user_id = eAgentInfo.agent_id;
        eShop.KeyIn_Flag = m_TwoKeyFlag;
        eShop.mod_date = DateTime.Now.ToString("yyyyMMdd");
        eShop.shop_type = m_ShopCancelFlag;
        eShop.Change_Item = m_ShopCancelFlag;
        // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis        
        eShop.LAST_UPD_BRANCH = txtLAST_UPD_BRANCH2.Text.Trim();// 資料最後異動分行
        eShop.LAST_UPD_CHECKER = txtLAST_UPD_CHECKER2.Text.Trim();// 資料最後異動-CHECKER
        eShop.LAST_UPD_MAKER = txtLAST_UPD_MAKER2.Text.Trim();// 資料最後異動-MAKER
        GetP4AndP4AValue(eShop);

        if (CSIPKeyInGUI.BusinessRules_new.BRSHOP.Insert(eShop, this.txtShopId.Text.Trim(), m_TwoKeyFlag, m_ShopCancelFlag))
        {
            string strColumns = "cancel_code1, cancel_date, cancel_code2, LAST_UPD_MAKER, LAST_UPD_CHECKER, LAST_UPD_BRANCH, p4, p4a";

            if (CompareKey(pnlCancelTask, eShop, m_ShopCancelFlag, strColumns))
            {
                //*更新資料檔trans_num
                if (!BRTRANS_NUM.UpdateTransNum("C02"))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }
                if (UpdateMainFrameData(SubmitType.CancelTaskSubmit))
                {
                    if (m_iCount > 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("01_00000000_005");
                    }

                    ClearAll();
                }
                else
                {
                    CommonFunction.SetEnabled(pnlCancelTask, false);//*將網頁中的提交按鈕和輸入框disable 
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
                }
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_006");
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 機器資料異動提交事件
    /// </summary>
    protected void btnMaSubmit_Click(object sender, EventArgs e)
    {
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
        eShop.KeyIn_Flag = m_TwoKeyFlag;
        eShop.mod_date = DateTime.Now.ToString("yyyyMMdd");
        eShop.shop_type = m_ShopMaFlag;
        eShop.Change_Item = m_ShopMaFlag;
        GetP4AndP4AValue(eShop);

        if (CSIPKeyInGUI.BusinessRules_new.BRSHOP.Insert(eShop, this.txtShopId.Text.Trim(), m_TwoKeyFlag, m_ShopMaFlag))
        {
            string strColumns = @"imp1_type, imp1, imp2_type, imp2, imp_money, pos1_type, pos1, pos2_type, pos2, pos_money, 
                                  edc1_type, edc1, edc_type, edc, edc_money, p4, p4a";

            if (CompareKey(pnlMachineData, eShop, m_ShopMaFlag, strColumns))
            {
                //*更新資料檔trans_num
                if (!BRTRANS_NUM.UpdateTransNum("C02"))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }

                if (UpdateMainFrameData(SubmitType.MachineSubmit))
                {
                    if (m_iCount > 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("01_00000000_005");
                    }

                    ClearAll();
                }
                else
                {
                    CommonFunction.SetEnabled(pnlMachineData, false);//*將網頁中的提交按鈕和輸入框disable 
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
                }
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_006");
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
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
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

        //*add by lvliangzhao 20090019 20100326 start 初始值為""
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
        //*add by lvliangzhao 20090019 20100326 end

        eShop.user_id = "";
        eShop.KeyIn_Flag = "";
        eShop.mod_date = "";
        eShop.shop_type = "";
        eShop.Change_Item = "";
        eShop.edcnum = "";

        eShop.ReqAppro = "";

        // 新增欄位
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

        //20190806-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy↓
        eShop.Undertaker_L = ""; //負責人中文長姓名
        eShop.Undertaker_Pinyin = ""; //負責人羅馬拼音
        eShop.JunctionPerson_L = ""; //聯絡人中文長姓名
        eShop.JunctionPerson_Pinyin = ""; //聯絡人羅馬拼音
        //20190806-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy↑

        // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis
        eShop.REG_ZIP_CODE = "";// 登記地址郵遞區號
        eShop.LAST_UPD_BRANCH = "";// 資料最後異動分行
        eShop.LAST_UPD_CHECKER = "";// 資料最後異動-CHECKER
        eShop.LAST_UPD_MAKER = "";// 資料最後異動-MAKER

        return eShop;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 設置控件文本
    /// </summary>
    private void SetControlsText()
    {
        chboxP4.Text = BaseHelper.GetShowText("01_01030200_088");
        chboxP4A.Text = BaseHelper.GetShowText("01_01030200_089");
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 查詢二KEY資料信息為各欄位賦值
    /// </summary>
    /// <param name="pnlType">所在的panel</param>
    /// <param name="strShopType">畫面功能類型編號</param>
    /// <param name="strColumns">要查詢欄位字符串</param>
    private void SelectData(CustPanel pnlType, string strShopType, string strColumns)
    {
        //20190919 addy by Peggy
        //DataSet dstInfo = CSIPKeyInGUI.BusinessRules_new.BRSHOP.Select(this.txtShopId.Text.Trim(), m_TwoKeyFlag, strShopType, strColumns);
        DataSet dstInfo = new DataSet();
        if (strShopType.Trim().Equals("6"))
        {
            dstInfo = CSIPKeyInGUI.BusinessRules_new.BRSHOP_CHANGE.Select(this.txtUNI_NO1.Text.Trim(), m_TwoKeyFlag, strColumns, "");
        }
        else
        {
            dstInfo = CSIPKeyInGUI.BusinessRules_new.BRSHOP.Select(this.txtShopId.Text.Trim(), m_TwoKeyFlag, strShopType, strColumns);
        }

        CommonFunction.SetControlsEnabled(pnlType, true);

        if (dstInfo == null)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_027");
            return;
        }

        if (dstInfo.Tables[0].Rows.Count > 0)
        {
            SetTextBoxValue(pnlType, dstInfo.Tables[0]);
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_002");
            return;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 查詢是否有一KEY資料信息,并且比較資料庫里的userid是否和異動userid相同
    /// </summary>
    /// <param name="strShopType">功能畫面編號</param>
    /// <returns>true有，false無</returns>
    private bool SelectOneKeyData(string strShopType)
    {
        //20190923 add by Peggy
        //DataSet dstInfo = CSIPKeyInGUI.BusinessRules_new.BRSHOP.Select(this.txtShopId.Text.Trim(), m_KeyFlag, strShopType, "*");
        DataSet dstInfo = new DataSet();
        if (strShopType.Trim().Equals("6"))
        {
            dstInfo = CSIPKeyInGUI.BusinessRules_new.BRSHOP_CHANGE.Select(this.txtUNI_NO1.Text.Trim(), m_KeyFlag, "*", "");
        }
        else
        {
            dstInfo = CSIPKeyInGUI.BusinessRules_new.BRSHOP.Select(this.txtShopId.Text.Trim(), m_KeyFlag, strShopType, "*");
        }

        if (dstInfo == null)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return false;
        }

        if (dstInfo.Tables[0].Rows.Count > 0)
        {
            string _1Key_user = string.Empty;//20190923 修改：復原這段，檢核一key與二key不能為同一人

            // // 測試機上版時，移除"一KEY與二KEY經辦不能為同一人"驗證*****正式機需加回
            //if (eAgentInfo.agent_id == dstInfo.Tables[0].Rows[0][CSIPKeyInGUI.EntityLayer_new.EntitySHOP.M_user_id].ToString().Trim())
            //{
            //    base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_00000000_008") + "');");
            //    return false;
            //}
            if (strShopType.Trim().Equals("6"))
            {
                _1Key_user = dstInfo.Tables[0].Rows[0]["MOD_USER"].ToString();
                strDOCID = dstInfo.Tables[0].Rows[0]["DOC_ID"].ToString().Trim();

                if (eAgentInfo.agent_id.Trim() == _1Key_user.Trim())
                {
                    base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_00000000_008") + "');");//一KEY與二KEY經辦不能為同一人
                    return false;
                }
            }
            else
            {
                _1Key_user = dstInfo.Tables[0].Rows[0]["user_id"].ToString();
            }

            /*20191009-先讓依統編修改卡這資訊，其他頁籤維持不卡 by Peggy
            if (eAgentInfo.agent_id.Trim() == _1Key_user.Trim())
            {
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_00000000_008") + "');");//一KEY與二KEY經辦不能為同一人
                return false;
            }
            */
            //Logging.SaveLog(ELogLayer.UI, "一次鍵檔員編：" + dstInfo.Tables[0].Rows[0][CSIPKeyInGUI.EntityLayer_new.EntitySHOP.M_user_id].ToString().Trim());
            Logging.Log("一次鍵檔員編：" + _1Key_user.Trim(), LogLayer.UI);
            Logging.Log("二次鍵檔員編：" + eAgentInfo.agent_id, LogLayer.UI);
        }
        else
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_00000000_007") + "');");//一KEY無此筆資料,請一KEY完成後再進行二KEY
            return false;
        }

        return true;
    }

    //*add by lvliangzhao 20090019 20100326 start
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
    //*add by lvliangzhao 20090019 20100326 end

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 執行資料庫操作
    /// </summary>
    /// <param name="strFlag">功能畫面標識</param>
    private void ShopDBAccess(string strFlag)
    {
        InsertShopUpload(strFlag);
        CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop = new CSIPKeyInGUI.EntityLayer_new.EntitySHOP();
        CSIPKeyInGUI.BusinessRules_new.BRSHOP.Delete(eShop, this.txtShopId.Text.Trim(), strFlag);
        InsertShopRpt();
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 比較一、二KEY資料是否相同
    /// </summary>
    /// <param name="pnlType">所在panel</param>
    /// <param name="strShopType">功能畫面編號</param>
    /// <param name="strColumns">欄位的字符串</param>
    /// <returns>true 相同，false 不同</returns>
    private bool CompareKey(CustPanel pnlType, CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop, string strShopType, string strColumns)
    {
        bool blnSame = true;
        // 查詢一Key資料
        DataSet dstInfo = CSIPKeyInGUI.BusinessRules_new.BRSHOP.Select(this.txtShopId.Text.Trim(), m_KeyFlag, strShopType, strColumns);

        if (dstInfo == null)
        {
            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            }
            return false;
        }

        if (dstInfo.Tables[0].Rows.Count > 0)
        {
            int i = 0;//*記錄不相同的數量
            int j = 0;
            string strValue = string.Empty;

            foreach (System.Web.UI.Control control in pnlType.Controls)
            {
                if (control is CustTextBox)
                {
                    CustTextBox txtBox = (CustTextBox)control;
                    strValue = txtBox.Text.Trim();

                    if (strShopType == "5")//*機器資料機型不足3碼左補'0'判斷
                    {
                        ArrayList arrTemp = new ArrayList(new object[] { "txtImp1", "txtImp2", "txtPos1", "txtPos2", "txtEdc1", "txtEdc" });
                        if (arrTemp.Contains(txtBox.ID))
                        {
                            strValue = BRCommon.GetPadLeftString(strValue, 3, '0');
                        }
                    }

                    if (strShopType == "1")
                    {
                        if (txtBox.ID == "txtUndertakerID")
                        {
                            strValue = strValue.ToUpper();
                        }

                        if (txtBox.ID == "txtRealPersonID")
                        {
                            strValue = strValue.ToUpper();
                        }

                        if (txtBox.ID == "txtIntroduceFlag")
                        {
                            strValue = BRCommon.GetPadLeftString(strValue, 5, '0');
                        }

                        // email欄位拆為兩個TextBox(以@符號做為分隔)
                        if (txtBox.ID == "txtEmailOther")
                            continue;

                        if (txtBox.ID == "txtEmailFront")
                        {
                            strValue = hidEmailFall.Value.Trim();
                        }
                    }

                    if (strShopType == "4")
                    {
                        if (txtBox.ID == "txtCancelCode2")
                        {
                            strValue = strValue.ToUpper();
                        }
                    }
                    string columnName = dstInfo.Tables[0].Columns[j].ToString();
                    if (strValue.ToUpper() != dstInfo.Tables[0].Rows[0][j].ToString().Trim().ToUpper())
                    {
                        if (i == 0)
                        {
                            base.sbRegScript.Append(BaseHelper.SetFocus(txtBox.ID));
                        }
                        txtBox.BackColor = Color.Red;
                        blnSame = false;
                        i++;
                    }
                    else
                    {
                        txtBox.BackColor = Color.Empty;
                    }
                    j++;
                }
            }

            if (eShop.P4 != int.Parse(dstInfo.Tables[0].Rows[0][CSIPKeyInGUI.EntityLayer_new.EntitySHOP.M_P4].ToString()))
            {
                this.chboxP4.ForeColor = Color.Red;
                blnSame = false;
            }

            if (eShop.p4A != int.Parse(dstInfo.Tables[0].Rows[0][CSIPKeyInGUI.EntityLayer_new.EntitySHOP.M_p4A].ToString()))
            {
                this.chboxP4A.ForeColor = Color.Red;
                blnSame = false;
            }
        }

        if (!blnSame)
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_009");
        }

        return blnSame;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
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
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
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
        eShopRpt.type = m_TwoKeyFlag;
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
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
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
        eShopKl.cancel_code2 = this.txtCancelCode2.Text.Trim();
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

    /// 作者 楊璐
    /// 創建日期：2012/06/19
    /// 修改日期：2012/06/19 
    /// <summary>
    /// 寫入請款加批日志檔
    /// </summary>
    /// <param name="strShopType">畫面功能編號</param>
    private void InsertShopReqAppro(string strShopType, string strRecord_name, string strBusiness_name, string strMerchantName, string strBefore, string strAfter)
    {
        EntitySet<CSIPKeyInGUI.EntityLayer_new.EntitySHOP> eShop = null;
        try
        {
            eShop = CSIPKeyInGUI.BusinessRules_new.BRSHOP.SelectShopTypeEntitySet(this.txtShopId.Text.Trim(), strShopType);
        }
        catch
        {
            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("01_00000000_020")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_020");
            }
            return;
        }

        for (int i = 0; i < eShop.Count; i++)
        {
            EntitySHOP_ReqAppro eShopReqAppro = new EntitySHOP_ReqAppro();
            eShopReqAppro.shop_id = eShop.GetEntity(i).shop_id;
            eShopReqAppro.Change_Item = eShop.GetEntity(i).Change_Item;
            eShopReqAppro.KeyIn_Flag = eShop.GetEntity(i).KeyIn_Flag;
            eShopReqAppro.mod_date = eShop.GetEntity(i).mod_date;
            eShopReqAppro.user_id = eShop.GetEntity(i).user_id;
            eShopReqAppro.mod_time = DateTime.Now.ToString("HHmmss");
            eShopReqAppro.NewCreate_Flag = "N";
            eShopReqAppro.Record_Name = strRecord_name;
            eShopReqAppro.Business_Name = strBusiness_name;
            eShopReqAppro.Merchant_Name = strMerchantName;
            eShopReqAppro.Before = strBefore;
            eShopReqAppro.After = strAfter;

            try
            {
                BRSHOP_ReqAppro.AddEntity(eShopReqAppro);
            }
            catch
            {
                if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("01_00000000_020")) < 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_00000000_020");
                }

                break;
            }
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 寫入特店資料異動檔
    /// </summary>
    /// <param name="strShopType">畫面功能編號</param>
    private void InsertShopUpload(string strShopType)
    {
        EntitySet<CSIPKeyInGUI.EntityLayer_new.EntitySHOP> eShop = null;
        try
        {
            eShop = CSIPKeyInGUI.BusinessRules_new.BRSHOP.SelectShopTypeEntitySet(this.txtShopId.Text.Trim(), strShopType);
        }
        catch
        {
            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("01_00000000_020")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_020");
            }

            return;
        }

        for (int i = 0; i < eShop.Count; i++)
        {
            EntitySHOP_UPLOAD eShopUpload = new EntitySHOP_UPLOAD();
            eShopUpload.shop_id = eShop.GetEntity(i).shop_id;
            eShopUpload.Change_Item = eShop.GetEntity(i).Change_Item;
            eShopUpload.KeyIn_Flag = eShop.GetEntity(i).KeyIn_Flag;
            eShopUpload.mod_date = eShop.GetEntity(i).mod_date;
            eShopUpload.user_id = eShop.GetEntity(i).user_id;
            eShopUpload.mod_time = DateTime.Now.ToString("HHmmss");
            try
            {
                BRSHOP_UPLOAD.AddEntity(eShopUpload);
            }
            catch
            {
                if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("01_00000000_020")) < 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_00000000_020");
                }

                break;
            }
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
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
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24
    /// <summary>
    /// 清空、禁用異動區域所有欄位，并設置商店代號為焦點
    /// </summary>
    private void ClearAll()
    {
        EnabledControls(false);

        //20190923 add by Peggy
        if (pnlBasicByTaxno.Visible == true)
        {
            this.txtUNI_NO1.Text = "";
            this.txtUNI_NO2.Text = "";
            this.chkCheckSum.Checked = false;
            this.chkisLongName2.Checked = false;
            lblREG_NAME.Text = "";
            lblOWNER_CHINESE_NAME.Text = "";
            this.chkCancelRevert.Checked = false;
            this.chkCancelRevert.BackColor = Color.Transparent;
            base.sbRegScript.Append(BaseHelper.SetFocus("txtUNI_NO1"));
        }
        else
        {
            this.txtShopId.Text = "";
            this.chkCancelRevert.Checked = false;
            this.chkCancelRevert.BackColor = Color.Transparent;
            base.sbRegScript.Append(BaseHelper.SetFocus("txtShopId"));
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
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
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 得到主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool GetMainframeData()
    {
        Hashtable hstInput = new Hashtable();
        hstInput.Add("MERCHANT_NO", this.txtShopId.Text.Trim());
        hstInput.Add("FUNCTION_CODE", "I");

        Hashtable htP4A_JCGR = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGR, hstInput, false, "11", eAgentInfo);
        htP4A_JCGR["MERCHANT_NO"] = hstInput["MERCHANT_NO"];//* for_xml_test  模擬環境測試，正式環境可以不用賦值
        htP4A_JCGR["MESSAGE_TYPE"] = "";
        htP4A_JCGR["MESSAGE_CHI"] = "";

        if (htP4A_JCGR.Contains("HtgMsg"))
        {
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
            etMstType = eMstType.Select;
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

            if (htP4A_JCGR["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            {
                base.strHostMsg += htP4A_JCGR["HtgMsg"].ToString();
                base.strClientMsg += MessageHelper.GetMessage("01_01030200_005");
            }
            else
            {
                base.strClientMsg += htP4A_JCGR["HtgMsg"].ToString();
            }

            return false;
        }

        base.strHostMsg += htP4A_JCGR["HtgSuccess"].ToString();//*主機返回成功訊息              

        Hashtable htInput = new Hashtable();
        htInput.Add("ACCT", this.txtShopId.Text.Trim());
        htInput.Add("FUNCTION_CODE", "I");
        htInput.Add("ORGN", "822");

        Hashtable htP4A_JCHR = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCHR, htInput, false, "11", eAgentInfo);
        htP4A_JCHR["ACCT"] = htInput["ACCT"];//* for_xml_test  模擬環境測試，正式環境可以不用賦值
        htP4A_JCHR["MESSAGE_TYPE"] = "";
        htP4A_JCHR["MESSAGE_CHI"] = "";
        htP4A_JCHR["ORGN"] = "822";

        if (!htP4A_JCHR.Contains("HtgMsg"))
        {
            //20211109_Ares_Jack_需可以解約還原
            // 20210527 EOS_AML(NOVA) 當點選查詢時，若該商店已解約時，不得異動特店資料
            if (htP4A_JCHR["STATUS_FLAG"].ToString() == "8" && htP4A_JCHR["DTE_USER_1"].ToString() != "00000000" && htP4A_JCHR["USER_CODE_2"].ToString() != "")
            {
                if (btnUnchainTask.BackColor != Color.FromArgb(255, 153, 0))//非解約作業
                {
                    base.sbRegScript.Append("alert('商店已解約不可再進行資料異動');");
                    base.strClientMsg += "商店已解約不可再進行資料異動";
                    return false;
                }
            }

            ViewState["HtgInfo_P4A_JCGR"] = htP4A_JCGR;
            ViewState["HtgInfo_P4A_JCHR"] = htP4A_JCHR;

            this.lblRecordNameText.Text = htP4A_JCGR["REG_NAME"].ToString();
            this.lblBusinessNameText.Text = htP4A_JCGR["BUSINESS_NAME"].ToString();
            this.lblMerchantNameText.Text = htP4A_JCHR["MERCHANT_NAME"].ToString();
            this.lblCORP_SEQ.Text = htP4A_JCGR["CORP_NO"].ToString().Trim() + htP4A_JCGR["CORP_SEQ"].ToString().Trim();//20190806-RQ-2019-008595-002-長姓名需求, 記錄序號 by Peggy

            base.strHostMsg += htP4A_JCHR["HtgSuccess"].ToString();//*主機返回成功訊息
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_012");
            return true;
        }
        else
        {
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
            etMstType = eMstType.Select;
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

            if (htP4A_JCHR.Contains("HtgMsg"))
            {
                if (htP4A_JCHR["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htP4A_JCHR["HtgMsg"].ToString();
                    base.strClientMsg += MessageHelper.GetMessage("01_01030200_006");
                }
                else
                {
                    base.strClientMsg += htP4A_JCHR["HtgMsg"].ToString();
                }
            }

            return false;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 設置控件文本顏色為黑色
    /// </summary>
    private void SetControlsTextColor()
    {
        CommonFunction.SetControlsForeColor(this.pnlBasic, Color.Black);
        CommonFunction.SetControlsForeColor(this.pnlFee, Color.Black);
        CommonFunction.SetControlsForeColor(this.pnlAccount, Color.Black);
        CommonFunction.SetControlsForeColor(this.pnlCancelTask, Color.Black);
        CommonFunction.SetControlsForeColor(this.pnlMachineData, Color.Black);
        CommonFunction.SetControlsForeColor(this.pnlBasicByTaxno, Color.Black);
        this.chboxP4.ForeColor = Color.Black;
        this.chboxP4A.ForeColor = Color.Black;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
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

    string strRecord_name = "";
    string strBusiness_name = "";
    string strMerchantName = "";

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 更新主機信息
    /// </summary>
    /// <param name="sType">提交類型</param>
    /// <param name="strMsg">提示信息</param>
    /// <returns>true 成功，false失敗</returns>
    private bool UpdateMainFrameData(SubmitType sType)
    {
        m_iCount = 0;
        bool bWrite = false;
        switch (sType)
        {
            case SubmitType.BasicSubmit:
                #region 
                //使用全局變量記錄請款加批異動前后的Record_name,Business_name,MerchantName
                strRecord_name = this.lblRecordNameText.Text.Trim();
                strBusiness_name = this.lblBusinessNameText.Text.Trim();
                strMerchantName = this.lblMerchantNameText.Text.Trim();

                //*首先異動PCMM
                if (this.chboxP4A.Checked)
                {
                    //20200217-RQ-2019-030155-003 先異動基本資料才異動PCMM
                    if (UpdateBasicP4A_JCGR())
                    {
                        //*20130117 Yucheng 素娟同意取消加批欄位不可空白限制,改為可允許空白,但若填寫僅能Y與N
                        //*若加批欄位為空時則不送加批電文
                        if (this.txtReqAppro.Text.Trim() != "")
                        {
                            if (!UpdateBasicJCGUA32())
                            {
                                return false;
                            }
                        }
                        else
                        {
                            //將UpdateBasicP4A_JCGR中的特店實體刪除移至此處
                            CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop = new CSIPKeyInGUI.EntityLayer_new.EntitySHOP();
                            CSIPKeyInGUI.BusinessRules_new.BRSHOP.Delete(eShop, this.txtShopId.Text.Trim(), m_ShopBasicFlag);
                        }

                        if (!UpdateBasicP4A_JCHR())
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                    /*20200217-mark 
                    if (UpdateBasicP4A_JCHR())
                    {
                        if (!UpdateBasicP4A_JCGR())
                        {
                            return false;
                        }
                        //else
                        //{
                        //    return false;
                        //}
                        //*20130117 Yucheng 素娟同意取消加批欄位不可空白限制,改為可允許空白,但若填寫僅能Y與N
                        //*若加批欄位為空時則不送加批電文
                        if (this.txtReqAppro.Text.Trim() != "")
                        {
                            if (!UpdateBasicJCGUA32())
                            {
                                return false;
                            }
                        }
                        else
                        {
                            //將UpdateBasicP4A_JCGR中的特店實體刪除移至此處
                            CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop = new CSIPKeyInGUI.EntityLayer_new.EntitySHOP();
                            CSIPKeyInGUI.BusinessRules_new.BRSHOP.Delete(eShop, this.txtShopId.Text.Trim(), m_ShopBasicFlag);
                        }
                    }//JCHR
                    else
                    {
                        return false;
                    }*/
                }
                
                if (this.chboxP4.Checked)
                {
                    /*20200217-mark 
                    if (UpdateBasicP4_JCHR())
                    {
                        if (!UpdateBasicP4_JCIJ())
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                    */
                    if (UpdateBasicP4_JCIJ())//20200217-RQ-2019-030155-003 先異動基本資料才異動PCMM
                    {
                        if (!UpdateBasicP4_JCHR())
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                #endregion
                break;
            case SubmitType.FeeSubmit:
                #region
                //*首先異動PCMM
                if (this.chboxP4A.Checked)
                {
                    if (UpdateFeeP4A_JCHR())
                    {
                        //銀聯卡異動主機
                        //*舊制外加費率異動(P4A-6122) 底下所有欄位,皆沒有異動 (都是空白)則不用去 Query 及 Update
                        if (txtStatus1.Text.Trim() != "" || txtStatus2.Text.Trim() != "" || txtStatus3.Text.Trim() != "" || txtStatus4.Text.Trim() != "" || txtStatus5.Text.Trim() != "" || txtStatus6.Text.Trim() != "" || txtStatus7.Text.Trim() != "" || txtStatus8.Text.Trim() != "" || txtStatus9.Text.Trim() != "" || txtStatus10.Text.Trim() != "")
                        {
                            bWrite = true;
                        }
                        if (txtFee1.Text.Trim() != "" || txtFee2.Text.Trim() != "" || txtFee3.Text.Trim() != "" || txtFee4.Text.Trim() != "" || txtFee5.Text.Trim() != "" || txtFee6.Text.Trim() != "" || txtFee7.Text.Trim() != "" || txtFee8.Text.Trim() != "" || txtFee9.Text.Trim() != "" || txtFee10.Text.Trim() != "")
                        {
                            bWrite = true;
                        }

                        if (bWrite)
                        {
                            if (!UpdateFeeJCG1P4A())
                            {
                                return false;
                            }
                        }

                        if (m_iCount > 0)
                        {
                            ShopDBAccess(m_ShopFeeFlag);
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                if (this.chboxP4.Checked)
                {
                    if (QueryP4_JCHR())
                    {
                        if (!QueryP4_JCIJ(m_ShopFeeFlag))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                #endregion
                break;
            case SubmitType.AccountSubmit:
                //*首先異動PCMM
                if (this.chboxP4A.Checked)
                {
                    //20200217-RQ-2019-030155-003 先異動基本資料才異動PCMM
                    if (UpdateAccountsP4A_JCGR())
                    {
                        if (!UpdateAccountsP4A_JCHR())
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                    /*20200217-mark 
                    if (UpdateAccountsP4A_JCHR())
                    {
                        if (!UpdateAccountsP4A_JCGR())
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                    */
                }

                if (this.chboxP4.Checked)
                {
                    /*20200217-mark 
                    if (UpdateAccountsP4_JCHR())
                    {
                        if (!UpdateAccountsP4_JCIJ())
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                    */
                    if (UpdateAccountsP4_JCIJ())//20200217-RQ-2019-030155-003 先異動基本資料才異動PCMM
                    {
                        if (!UpdateAccountsP4_JCHR())
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                break;
            case SubmitType.CancelTaskSubmit:
                if (this.chboxP4A.Checked)
                {
                    if (UpdateCancelTaskP4A_JCHR())
                    {
                        if (m_iCount > 0)
                        {
                            ShopDBAccess(m_ShopCancelFlag);
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                if (this.chboxP4.Checked)
                {
                    if (UpdateCancelTaskP4_JCHR())
                    {
                        if (!QueryP4_JCIJ(m_ShopCancelFlag))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                break;
            case SubmitType.MachineSubmit:

                if (this.chboxP4A.Checked)
                {
                    //*首先異動PCMM
                    if (UpdateMachineP4A_JCHR())
                    {
                        if (!UpdateMachineP4A_JCGR())
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                if (this.chboxP4.Checked)
                {
                    if (QueryP4_JCHR())
                    {
                        if (!QueryP4_JCIJ(m_ShopMaFlag))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                break;
        }

        return true;
    }

    /// <summary>
    /// 比對得到上傳主機聯絡人電話
    /// </summary>
    /// <param name="strHtgTel">主機聯絡人電話</param>
    /// <returns>主機聯絡人電話</returns>
    private string GetTelCompages(string strHtgTel)
    {
        string[] strValue;
        string strHtgTel1 = "";// 主機聯絡人電話1
        string strHtgTel2 = "";// 主機聯絡人電話2
        string strHtgTel3 = "";// 主機聯絡人電話3
        string strRet = "";

        if (strHtgTel.IndexOf('-') > -1)
        {
            strValue = strHtgTel.Split(new char[] { '-' });

            if (strValue.Length == 3)
            {
                strHtgTel1 = strValue[0];
                strHtgTel2 = strValue[1];
                strHtgTel3 = strValue[2];
            }
            else
            {
                strHtgTel1 = strValue[0];
                strHtgTel2 = strValue[1];
            }
        }
        else
        {
            strHtgTel1 = strHtgTel;// 主機聯絡人電話
        }
        // 聯絡人電話一
        if (this.txtJunctionPersonTel1.Text.Trim() != "")
        {
            if (this.txtJunctionPersonTel1.Text.Trim().ToUpper() != "X")
            {
                strHtgTel1 = BRCommon.GetPadRightString(this.txtJunctionPersonTel1.Text.Trim(), 3, ' ');
            }
            else
            {
                strHtgTel1 = "";
            }
        }
        // 聯絡人電話二
        if (this.txtJunctionPersonTel2.Text.Trim() != "")
        {
            if (this.txtJunctionPersonTel2.Text.Trim().ToUpper() != "X")
            {
                strHtgTel2 = this.txtJunctionPersonTel2.Text.Trim();
            }
            else
            {
                strHtgTel2 = "";
            }
        }
        // 聯絡人電話三
        if (this.txtJunctionPersonTel3.Text.Trim() != "")
        {
            if (this.txtJunctionPersonTel3.Text.Trim().ToUpper() != "X")
            {
                strHtgTel3 = this.txtJunctionPersonTel3.Text.Trim();
            }
            else
            {
                strHtgTel3 = "";
            }
        }

        if (strHtgTel3 != "")
        {
            strRet = strHtgTel1 + "-" + strHtgTel2 + "-" + strHtgTel3;
        }
        else if (strHtgTel2 != "")
        {
            strRet = strHtgTel1 + "-" + strHtgTel2;
        }
        else
        {
            strRet = strHtgTel1;
        }

        if (strRet == "")
        {
            return "X";
        }
        else if (strHtgTel == strRet)// 聯絡人電話1.2.3輸入值為"X"
        {
            return "";
        }
        else
        {
            return strRet;
        }
    }

    /// <summary>
    /// 比對得到上傳主機聯絡人傳真
    /// </summary>
    /// <param name="strHtgTel">主機聯絡人傳真</param>
    /// <returns>主機聯絡人傳真</returns>
    private string GetFaxCompages(string strHtgFax)
    {
        string[] strValue;
        string strHtgFax1 = "";//*主機聯絡人傳真1
        string strHtgFax2 = "";//*主機聯絡人傳真2

        if (strHtgFax.IndexOf('-') > -1)
        {
            strValue = strHtgFax.Split(new char[] { '-' });

            strHtgFax1 = strValue[0];
            strHtgFax2 = strValue[1];
        }
        else
        {
            strHtgFax1 = strHtgFax;
        }

        if (this.txtJunctionPersonFax1.Text.Trim() != "")
        {
            if (this.txtJunctionPersonFax1.Text.Trim().ToUpper() != "X")
            {
                strHtgFax1 = BRCommon.GetPadRightString(this.txtJunctionPersonFax1.Text.Trim(), 3, ' ');
            }
            else
            {
                strHtgFax1 = "";
            }
        }
        if (this.txtJunctionPersonFax2.Text.Trim() != "")
        {
            if (this.txtJunctionPersonFax2.Text.Trim().ToUpper() != "X")
            {
                strHtgFax2 = this.txtJunctionPersonFax2.Text.Trim();
            }
            else
            {
                strHtgFax2 = "";
            }
        }

        string strTemp = "";
        if (strHtgFax2 != "")
        {
            strTemp = strHtgFax1 + "-" + strHtgFax2;
        }
        else
        {
            strTemp = strHtgFax1;
        }

        if (strTemp == "")
        {
            return "X";
        }
        else if (strTemp == strHtgFax)
        {
            return "";
        }
        else
        {
            return strTemp;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 查詢主機PCMMP4
    /// </summary>
    /// <returns>true成功,false失敗</returns>
    private bool QueryP4_JCHR()
    {
        Hashtable htInput = new Hashtable();
        htInput.Add("ACCT", this.txtShopId.Text.Trim());
        htInput.Add("FUNCTION_CODE", "I");
        htInput.Add("ORGN", "900");

        Hashtable htP4_JCHR = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCHR, htInput, false, "1", eAgentInfo);
        if (!htP4_JCHR.Contains("HtgMsg"))
        {
            base.strHostMsg += htP4_JCHR["HtgSuccess"].ToString();//*主機返回成功訊息
            base.strClientMsg += MessageHelper.GetMessage("01_01030200_011");
            return true;
        }
        else
        {
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
            etMstType = eMstType.Select;
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

            if (htP4_JCHR["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            {
                base.strHostMsg += htP4_JCHR["HtgMsg"].ToString();
                base.strClientMsg += MessageHelper.GetMessage("01_01030200_008");
            }
            else
            {
                base.strClientMsg += htP4_JCHR["HtgMsg"].ToString();
            }

            return false;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 查詢主機EXMSP4
    /// </summary>
    /// <param name="strFlag">功能畫面標識</param>
    /// <returns>true成功,false失敗</returns>
    private bool QueryP4_JCIJ(string strFlag)
    {
        Hashtable htInput = new Hashtable();
        htInput.Add("MER_NO", this.txtShopId.Text.Trim());
        htInput.Add("FUNCTION_CODE", "2");
        htInput.Add("LINE_CNT", "0000");
        htInput.Add("MESSAGE_TYPE", "");

        Hashtable htP4_JCIJ = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCIJ, htInput, false, "1", eAgentInfo);
        if (!htP4_JCIJ.Contains("HtgMsg"))
        {
            base.strHostMsg += htP4_JCIJ["HtgSuccess"].ToString();//*主機返回成功訊息
            base.strClientMsg += MessageHelper.GetMessage("01_01030200_018");
            if (m_iCount > 0)
            {
                ShopDBAccess(strFlag);
            }

            return true;
        }
        else
        {
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
            etMstType = eMstType.Select;
            //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

            //*查詢主機資料失敗
            if (htP4_JCIJ["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            {
                base.strHostMsg += htP4_JCIJ["HtgMsg"].ToString();
                base.strClientMsg += MessageHelper.GetMessage("01_01030200_017");
            }
            else
            {
                base.strClientMsg += htP4_JCIJ["HtgMsg"].ToString();
            }

            return false;
        }
    }

    #region 基本資料異動
    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 更新基本資料PCMM_P4A主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateBasicP4A_JCHR()
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        Hashtable htP4A_JCHR = new Hashtable();
        CommonFunction.GetViewStateHt(ViewState["HtgInfo_P4A_JCHR"], ref htP4A_JCHR);

        DataTable dtblUpdateData = CommonFunction.GetDataTable();

        //*比對帳單列示名稱
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtMerchantNameTwo.Text.Trim(), "MERCHANT_NAME", BaseHelper.GetShowText("01_01030200_010"));
        //*比對商店英文名稱
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtEnglishName.Text.Trim(), "ID_NAME", BaseHelper.GetShowText("01_01030200_011"));
        //*比對推廣員
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtIntroduces.Text.Trim(), "MERCH_MEMO", BaseHelper.GetShowText("01_01030200_027"));
        //*比對聯絡人電話1
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtJunctionPersonTel1.Text.Trim(), "PHONE_NMBR1", BaseHelper.GetShowText("01_01030200_021"));
        //*比對聯絡人電話2
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtJunctionPersonTel2.Text.Trim(), "PHONE_NMBR2", BaseHelper.GetShowText("01_01030200_021"));
        //*比對聯絡人電話3
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtJunctionPersonTel3.Text.Trim(), "PHONE_NMBR3", BaseHelper.GetShowText("01_01030200_021"));
        //*比對帳單地址-通訊地址一
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtCommadd1.Text.Trim(), "ADDRESS1", BaseHelper.GetShowText("01_01030200_026"));
        //*比對帳單地址-通訊地址二
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtComaddr2.Text.Trim(), "ADDRESS2", BaseHelper.GetShowText("01_01030200_026"));
        //*比對帳單地址-zip
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtZip.Text.Trim(), "ZIP_CODE", BaseHelper.GetShowText("01_01030200_026"));
        //*比對負責人英文名
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtUndertakerEngName.Text.Trim(), "CONTACT", BaseHelper.GetShowText("01_01030200_014"));
        //*比對HOLD STMT
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtHOLDSTMT.Text.Trim(), "HOLD_STMT_FLAG", BaseHelper.GetShowText("01_01030200_030"));
        //*比對推廣代號
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, BRCommon.GetPadLeftString(this.txtIntroduceFlag.Text.Trim(), 5, '0'), "AGENT_BANK", BaseHelper.GetShowText("01_01030200_028"));

        if (dtblUpdateData.Rows.Count > 0)
        {
            htP4A_JCHR["FUNCTION_CODE"] = "M";
            Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCHR, htP4A_JCHR, false, "2", eAgentInfo);
            if (!htResult.Contains("HtgMsg"))
            {
                m_iCount++;
                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                base.strClientMsg += MessageHelper.GetMessage("01_01030200_010");
                if (!CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, this.txtShopId.Text.Trim(), BaseHelper.GetShowText("01_01030200_089").Trim(), (structPageInfo)Session["PageInfo"]))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }

                //更新請款加批日志MERCHANT_NAME.　Add by LuYang 20120620
                for (int i = 0; i < dtblUpdateData.Rows.Count; i++)
                {
                    if (dtblUpdateData.Rows[i][EntityCUSTOMER_LOG.M_field_name].ToString() == BaseHelper.GetShowText("01_01030200_010"))
                    {
                        strMerchantName = dtblUpdateData.Rows[i][EntityCUSTOMER_LOG.M_after].ToString();
                        break;
                    }

                }//end of 更新請款加批日志

                // 20210527 EOS_AML(NOVA) by Ares Dennis
                #region 異動記錄需報送AML  
                Hashtable htP4A_JCHR2 = new Hashtable();// 主機資料
                CommonFunction.GetViewStateHt(ViewState["HtgInfo_P4A_JCHR"], ref htP4A_JCHR2);
                
                bool isChanged = false;
                
                #region 檢核欄位是否異動
                compareForAMLCheckLog(htP4A_JCHR2, htP4A_JCHR, "ID_NAME", ref isChanged);// 商店英文名稱
                compareForAMLCheckLog(htP4A_JCHR2, htP4A_JCHR, "CONTACT", ref isChanged);// 負責人英文名                
                #endregion
                if (isChanged)
                {
                    Hashtable htP4A_JCGR = new Hashtable();// 20211110_Ares_Jack_主機資料
                    htP4A_JCGR.Add("FUNCTION_CODE", "I");
                    htP4A_JCGR.Add("MERCHANT_NO", this.txtShopId.Text);
                    Hashtable htP4A_JCGR2 = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGR, htP4A_JCGR, false, "21", eAgentInfo);

                    EntityAML_CHECKLOG eAMLCheckLog = new EntityAML_CHECKLOG();
                    //eAMLCheckLog.CORP_NO = htP4A_JCHR["DB_ACCT_NMBR"].ToString().Trim();
                    //20211110_Ares_Jack_ HEADQUARTER_CORPNO 為自然人用統編 CORP_NO 帶負責人ID, 其餘帶總公司統編
                    if (involve.Contains(htP4A_JCGR2["HEADQUARTER_CORPNO"].ToString().Trim()))
                        eAMLCheckLog.CORP_NO = htP4A_JCGR2["OWNER_ID"].ToString().Trim();//負責人ID
                    else
                        eAMLCheckLog.CORP_NO = htP4A_JCGR2["CORP_NO"].ToString().Trim();
                    eAMLCheckLog.MER_NO = htP4A_JCHR["ACCT"].ToString().Trim();
                    eAMLCheckLog.TRANS_ID = "CSIPJCHR";//CSIP+電文TRAN_ID
                    eAMLCheckLog.LAST_UPD_BRANCH = txtLAST_UPD_BRANCH.Text.Trim();
                    eAMLCheckLog.LAST_UPD_CHECKER = txtLAST_UPD_CHECKER.Text.Trim();
                    eAMLCheckLog.LAST_UPD_MAKER = txtLAST_UPD_MAKER.Text.Trim();
                    eAMLCheckLog.MOD_USERID = eAgentInfo.agent_id;
                    eAMLCheckLog.MOD_DATE = DateTime.Now.ToString("yyyyMMdd");
                    eAMLCheckLog.MOD_TIME = DateTime.Now.ToString("HHmmss");

                    InsertAMLCheckLog(eAMLCheckLog);
                }                
                #endregion

                return true;
            }
            else
            {
                //*異動主機資料失敗
                if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htResult["HtgMsg"].ToString();
                    base.strClientMsg += HtgType.P4A_JCHR.ToString() + MessageHelper.GetMessage("01_00000000_011");
                }
                else
                {
                    base.strClientMsg += htResult["HtgMsg"].ToString();
                }

                return false;
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01030200_007");
            return true;
        }
    }

    /// 作者 楊璐
    /// 創建日期：2012/06/18
    /// 修改日期：2012/06/18 
    /// <summary>
    /// 更新基本資料JCGUA32主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateBasicJCGUA32()
    {
        etMstType = eMstType.Control;

        Hashtable htInput = new Hashtable();
        htInput.Add("TRAN_ID", "JCPA");
        htInput.Add("MERCHANT_NO", this.txtShopId.Text.Trim());
        htInput.Add("FUNCTION_CODE", "I");
        htInput.Add("FILLER1", "!");
        htInput.Add("FILLER2", "!");

        Hashtable ht_JCGUA32 = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCPA, htInput, false, "1", eAgentInfo);
        ht_JCGUA32["MERCHANT_NO"] = htInput["MERCHANT_NO"];//* for_xml_test  模擬環境測試，正式環境可以不用賦值
        ht_JCGUA32["MESSAGE_TYPE"] = "";
        ht_JCGUA32["MESSAGE"] = "";

        if (!ht_JCGUA32.Contains("HtgMsg"))
        {
            base.strHostMsg += ht_JCGUA32["HtgSuccess"].ToString();//*主機返回成功訊息
            base.strClientMsg += MessageHelper.GetMessage("01_01030200_023");
        }
        else
        {
            if (ht_JCGUA32.Contains("HtgMsg"))
            {
                if (ht_JCGUA32["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += ht_JCGUA32["MESSAGE_TYPE"].ToString();
                    base.strHostMsg += ht_JCGUA32["MESSAGE"].ToString();
                    base.strClientMsg += MessageHelper.GetMessage("01_01030200_024");
                }
                else
                {
                    base.strClientMsg += ht_JCGUA32["HtgMsg"].ToString();
                }
            }

            return false;
        }

        DataTable dtblUpdateData = CommonFunction.GetDataTable();
        //*比對請款加批
        CommonFunction.ContrastDataTwo(ht_JCGUA32, dtblUpdateData, this.txtReqAppro.Text.Trim(), "BATCH_FLAG", BaseHelper.GetShowText("01_01030200_105"));

        if (dtblUpdateData.Rows.Count > 0)
        {
            ht_JCGUA32["FUNCTION_CODE"] = "U";

            ht_JCGUA32["FILLER3"] = "!";
            ht_JCGUA32["FILLER4"] = "!";
            ht_JCGUA32["FILLER5"] = "!";
            ht_JCGUA32["PPMERFG_FLAG"] = "#";
            ht_JCGUA32["PPMERRT_RATE"] = "#";

            Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCPA, ht_JCGUA32, false, "2", eAgentInfo);

            if (!htResult.Contains("HtgMsg"))
            {
                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                base.strHostMsg += "設定成功 ";
                base.strClientMsg += MessageHelper.GetMessage("01_01030200_025");

                if (!CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, this.txtShopId.Text.Trim(), " ", (structPageInfo)Session["PageInfo"]))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }

                string strBefore = dtblUpdateData.Rows[0][EntityCUSTOMER_LOG.M_before].ToString();
                string strAfter = dtblUpdateData.Rows[0][EntityCUSTOMER_LOG.M_after].ToString();
                InsertShopReqAppro(m_ShopBasicFlag, strRecord_name, strBusiness_name, strMerchantName, strBefore, strAfter);

                //將UpdateBasicP4A_JCGR中的特店實體刪除移至此處
                CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop = new CSIPKeyInGUI.EntityLayer_new.EntitySHOP();
                CSIPKeyInGUI.BusinessRules_new.BRSHOP.Delete(eShop, this.txtShopId.Text.Trim(), m_ShopBasicFlag);

                return true;
            }
            else
            {
                //*異動主機資料失敗
                string strMsg = htResult["MESSAGE_TYPE"].ToString() + htResult["MESSAGE"].ToString();
                if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += strMsg;
                    base.strClientMsg += (HtgType.P4A_JCPA.ToString() + MessageHelper.GetMessage("01_00000000_011"));
                }
                else
                {
                    base.strClientMsg += strMsg;
                }

                return false;
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01030200_026");
            MessageHelper.ShowMessage(this.UpdatePanel1, new string[] { "01_01030200_026" });

            string strBefore = this.txtReqAppro.Text;
            string strAfter = this.txtReqAppro.Text;
            InsertShopReqAppro(m_ShopBasicFlag, strRecord_name, strBusiness_name, strMerchantName, strBefore, strAfter);

            //將UpdateBasicP4A_JCGR中的特店實體刪除移至此處
            CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop = new CSIPKeyInGUI.EntityLayer_new.EntitySHOP();
            CSIPKeyInGUI.BusinessRules_new.BRSHOP.Delete(eShop, this.txtShopId.Text.Trim(), m_ShopBasicFlag);

            return true;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 更新基本資料PCMM_P4主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateBasicP4_JCHR()
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        Hashtable htInput = new Hashtable();
        htInput.Add("ACCT", this.txtShopId.Text.Trim());
        htInput.Add("FUNCTION_CODE", "I");
        htInput.Add("ORGN", "900");

        Hashtable htP4_JCHR = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCHR, htInput, false, "1", eAgentInfo);

        // 20210527 EOS_AML(NOVA) by Ares Dennis start
        Hashtable htP_JCHR2 = htP4_JCHR;// 主機資料
        // 20210527 EOS_AML(NOVA) by Ares Dennis end

        htP4_JCHR["ACCT"] = htInput["ACCT"];//* for_xml_test  模擬環境測試，正式環境可以不用賦值
        htP4_JCHR["MESSAGE_TYPE"] = "";
        htP4_JCHR["MESSAGE_CHI"] = "";

        if (!htP4_JCHR.Contains("HtgMsg"))
        {
            base.strHostMsg += htP4_JCHR["HtgSuccess"].ToString();//*主機返回成功訊息
            base.strClientMsg += MessageHelper.GetMessage("01_01030200_011");
        }
        else
        {
            if (htP4_JCHR.Contains("HtgMsg"))
            {
                if (htP4_JCHR["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htP4_JCHR["HtgMsg"].ToString();
                    base.strClientMsg += MessageHelper.GetMessage("01_01030200_008");
                }
                else
                {
                    base.strClientMsg += htP4_JCHR["HtgMsg"].ToString();
                }
            }

            return false;
        }

        DataTable dtblUpdateData = CommonFunction.GetDataTable();
        //*比對帳單列示名稱
        CommonFunction.ContrastDataTwo(htP4_JCHR, dtblUpdateData, this.txtMerchantNameTwo.Text.Trim(), "MERCHANT_NAME", BaseHelper.GetShowText("01_01030200_010"));
        //*比對商店英文名稱
        CommonFunction.ContrastDataTwo(htP4_JCHR, dtblUpdateData, this.txtEnglishName.Text.Trim(), "ID_NAME", BaseHelper.GetShowText("01_01030200_011"));
        //*比對推廣員
        CommonFunction.ContrastDataTwo(htP4_JCHR, dtblUpdateData, this.txtIntroduces.Text.Trim(), "MERCH_MEMO", BaseHelper.GetShowText("01_01030200_027"));
        //*比對聯絡人電話1
        CommonFunction.ContrastDataTwo(htP4_JCHR, dtblUpdateData, this.txtJunctionPersonTel1.Text.Trim(), "PHONE_NMBR1", BaseHelper.GetShowText("01_01030200_021"));
        //*比對聯絡人電話2
        CommonFunction.ContrastDataTwo(htP4_JCHR, dtblUpdateData, this.txtJunctionPersonTel2.Text.Trim(), "PHONE_NMBR2", BaseHelper.GetShowText("01_01030200_021"));
        //*比對聯絡人電話3
        CommonFunction.ContrastDataTwo(htP4_JCHR, dtblUpdateData, this.txtJunctionPersonTel3.Text.Trim(), "PHONE_NMBR3", BaseHelper.GetShowText("01_01030200_021"));
        //*比對帳單地址-通訊地址一
        CommonFunction.ContrastDataTwo(htP4_JCHR, dtblUpdateData, this.txtCommadd1.Text.Trim(), "ADDRESS1", BaseHelper.GetShowText("01_01030200_026"));
        //*比對帳單地址-通訊地址二
        CommonFunction.ContrastDataTwo(htP4_JCHR, dtblUpdateData, this.txtComaddr2.Text.Trim(), "ADDRESS2", BaseHelper.GetShowText("01_01030200_026"));
        //*比對帳單地址-zip
        CommonFunction.ContrastDataTwo(htP4_JCHR, dtblUpdateData, this.txtZip.Text.Trim(), "ZIP_CODE", BaseHelper.GetShowText("01_01030200_026"));
        //*比對負責人英文名
        CommonFunction.ContrastDataTwo(htP4_JCHR, dtblUpdateData, this.txtUndertakerEngName.Text.Trim(), "CONTACT", BaseHelper.GetShowText("01_01030200_014"));
        //*比對HOLD STMT
        CommonFunction.ContrastDataTwo(htP4_JCHR, dtblUpdateData, this.txtHOLDSTMT.Text.Trim(), "HOLD_STMT_FLAG", BaseHelper.GetShowText("01_01030200_030"));
        //*比對推廣代號
        CommonFunction.ContrastDataTwo(htP4_JCHR, dtblUpdateData, BRCommon.GetPadLeftString(this.txtIntroduceFlag.Text.Trim(), 5, '0'), "AGENT_BANK", BaseHelper.GetShowText("01_01030200_028"));

        if (dtblUpdateData.Rows.Count > 0)
        {
            htP4_JCHR["FUNCTION_CODE"] = "M";
            htP4_JCHR["ORGN"] = "900";
            Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCHR, htP4_JCHR, false, "2", eAgentInfo);
            if (!htResult.Contains("HtgMsg"))
            {
                m_iCount++;
                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                base.strClientMsg += MessageHelper.GetMessage("01_01030200_012");
                if (!CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, this.txtShopId.Text.Trim(), BaseHelper.GetShowText("01_01030200_088").Trim(), (structPageInfo)Session["PageInfo"]))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }

                // 20210527 EOS_AML(NOVA) by Ares Dennis
                #region 異動記錄需報送AML                  
                bool isChanged = false;

                #region 檢核欄位是否異動
                compareForAMLCheckLog(htP_JCHR2, htP4_JCHR, "ID_NAME", ref isChanged);// 商店英文名稱
                compareForAMLCheckLog(htP_JCHR2, htP4_JCHR, "CONTACT", ref isChanged);// 負責人英文名                
                #endregion
                if (isChanged)
                {
                    Hashtable htP4A_JCGR = new Hashtable();// 20211110_Ares_Jack_主機資料
                    htP4A_JCGR.Add("FUNCTION_CODE", "I");
                    htP4A_JCGR.Add("MERCHANT_NO", this.txtShopId.Text);
                    Hashtable htP4A_JCGR2 = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGR, htP4A_JCGR, false, "21", eAgentInfo);

                    EntityAML_CHECKLOG eAMLCheckLog = new EntityAML_CHECKLOG();
                    //eAMLCheckLog.CORP_NO = htP4A_JCHR["DB_ACCT_NMBR"].ToString().Trim();
                    //20211110_Ares_Jack_ HEADQUARTER_CORPNO 為自然人用統編 CORP_NO 帶負責人ID, 其餘帶總公司統編
                    if (involve.Contains(htP4A_JCGR2["HEADQUARTER_CORPNO"].ToString().Trim()))
                        eAMLCheckLog.CORP_NO = htP4A_JCGR2["OWNER_ID"].ToString().Trim();//負責人ID
                    else
                        eAMLCheckLog.CORP_NO = htP4A_JCGR2["CORP_NO"].ToString().Trim();
                    eAMLCheckLog.MER_NO = htP4_JCHR["ACCT"].ToString().Trim();
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

                return true;
            }
            else
            {
                //*異動主機資料失敗
                if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htResult["HtgMsg"].ToString();
                    base.strClientMsg += HtgType.P4_JCHR.ToString() + MessageHelper.GetMessage("01_00000000_011");
                }
                else
                {
                    base.strClientMsg += htResult["HtgMsg"].ToString();
                }

                return false;
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01030200_009");
            return true;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 更新基本資料EXMS_P4A主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateBasicP4A_JCGR()
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end


        Hashtable htP4A_JCGR = new Hashtable();
        string mLongName = string.Empty;
        string mContactLName = string.Empty;

        CommonFunction.GetViewStateHt(ViewState["HtgInfo_P4A_JCGR"], ref htP4A_JCGR);

        DataTable dtblUpdateData = CommonFunction.GetDataTable();
        DataTable dtblUpdateDataNew = CommonFunction.GetDataTable();
        DataTable dtblUpdateDataJC68 = CommonFunction.GetDataTable();//記錄長姓名的異動記錄

        if (!string.IsNullOrEmpty(htP4A_JCGR["OWNER_LNAM_FLAG"].ToString().Trim()))//記錄電文負責人狀態
        {
            mLongName = htP4A_JCGR["OWNER_LNAM_FLAG"].ToString().Trim();
        }
        if (!string.IsNullOrEmpty(htP4A_JCGR["CONTACT_LNAM_FLAG"].ToString().Trim()))//記錄電文聯絡人狀態
        {
            mContactLName = htP4A_JCGR["CONTACT_LNAM_FLAG"].ToString().Trim();
        }

        CompareExmsHtgValue(ref htP4A_JCGR, ref dtblUpdateData);


        int count = 0;
        string before = "";
        string after = "";

        foreach (DataRow updateData in dtblUpdateData.Rows)
        {

            if (updateData[0].ToString() == "設立")
            {
                if (this.txtEstablish.Text.Trim() != "")
                {
                    count++;
                    before += updateData[1].ToString();
                    after += updateData[2].ToString();

                    if (count == 2)
                    {
                        DataRow drowRow = dtblUpdateDataNew.NewRow();
                        drowRow[EntityCUSTOMER_LOG.M_field_name] = updateData[0].ToString();
                        drowRow[EntityCUSTOMER_LOG.M_before] = before;
                        drowRow[EntityCUSTOMER_LOG.M_after] = after;
                        dtblUpdateDataNew.Rows.Add(drowRow);
                    }
                }
            }
            else if (updateData[0].ToString() == "E-MAIL")
            {
                before = updateData[1].ToString().ToUpper();
                after = updateData[2].ToString().ToUpper();

                if (before != after)
                {
                    DataRow drowRow = dtblUpdateDataNew.NewRow();
                    drowRow[EntityCUSTOMER_LOG.M_field_name] = updateData[0].ToString();
                    drowRow[EntityCUSTOMER_LOG.M_before] = updateData[1].ToString();
                    drowRow[EntityCUSTOMER_LOG.M_after] = updateData[2].ToString();
                    dtblUpdateDataNew.Rows.Add(drowRow);
                }
            }
            else
            {
                DataRow drowRow = dtblUpdateDataNew.NewRow();
                drowRow[EntityCUSTOMER_LOG.M_field_name] = updateData[0].ToString();
                drowRow[EntityCUSTOMER_LOG.M_before] = updateData[1].ToString();
                drowRow[EntityCUSTOMER_LOG.M_after] = updateData[2].ToString();
                dtblUpdateDataNew.Rows.Add(drowRow);
            }
        }
        dtblUpdateData = dtblUpdateDataNew;


        #region 20190806-RQ-2019-008595-002-長姓名需求，長姓名異動欄位比對 by Peggy
        EntityHTG_JC68 htReturn_JC68 = new EntityHTG_JC68();
        if (!txtUndertaker.Text.Trim().Equals("") || !txtUndertaker_L.Text.Trim().Equals(""))
        {
            htReturn_JC68 = new EntityHTG_JC68();
            htReturn_JC68 = GetJC68(txtUndertakerID.Text.Trim());
            before = htReturn_JC68.LONG_NAME.Trim();//電文
            after = txtUndertaker_L.Text.Trim();//畫面
            if (before != after)
            {
                DataRow drowRow = dtblUpdateDataJC68.NewRow();
                drowRow[EntityCUSTOMER_LOG.M_field_name] = "負責人中文長姓名";
                drowRow[EntityCUSTOMER_LOG.M_before] = htReturn_JC68.LONG_NAME.Trim();
                drowRow[EntityCUSTOMER_LOG.M_after] = txtUndertaker_L.Text.Trim();
                dtblUpdateDataJC68.Rows.Add(drowRow);
            }

            before = htReturn_JC68.PINYIN_NAME.Trim();//電文
            after = LongNameRomaClean(txtUndertaker_Pinyin.Text.Trim());//畫面
            if (before != after)
            {
                DataRow drowRow = dtblUpdateDataJC68.NewRow();
                drowRow[EntityCUSTOMER_LOG.M_field_name] = "負責人羅馬拼音";
                drowRow[EntityCUSTOMER_LOG.M_before] = htReturn_JC68.PINYIN_NAME.Trim();
                drowRow[EntityCUSTOMER_LOG.M_after] = after;
                dtblUpdateDataJC68.Rows.Add(drowRow);
            }
        }

        if (!txtJunctionPerson.Text.Trim().Equals("") || !txtJunctionPerson_L.Text.Trim().Equals(""))
        {
            htReturn_JC68 = new EntityHTG_JC68();
            htReturn_JC68 = GetJC68(lblCORP_SEQ.Text.Trim());
            before = htReturn_JC68.LONG_NAME.Trim();//電文
            after = txtJunctionPerson_L.Text.Trim();//畫面
            if (before != after)
            {
                DataRow drowRow = dtblUpdateDataJC68.NewRow();
                drowRow[EntityCUSTOMER_LOG.M_field_name] = "聯絡人中文長姓名";
                drowRow[EntityCUSTOMER_LOG.M_before] = htReturn_JC68.LONG_NAME.Trim();
                drowRow[EntityCUSTOMER_LOG.M_after] = txtJunctionPerson_L.Text.Trim();
                dtblUpdateDataJC68.Rows.Add(drowRow);
            }

            before = htReturn_JC68.PINYIN_NAME.Trim();//電文
            after = LongNameRomaClean(txtJunctionPerson_Pinyin.Text.Trim());//畫面

            if (before != after)
            {
                DataRow drowRow = dtblUpdateDataJC68.NewRow();
                drowRow[EntityCUSTOMER_LOG.M_field_name] = "聯絡人羅馬拼音";
                drowRow[EntityCUSTOMER_LOG.M_before] = htReturn_JC68.PINYIN_NAME.Trim();
                drowRow[EntityCUSTOMER_LOG.M_after] = after;
                dtblUpdateDataJC68.Rows.Add(drowRow);
            }
        }
        #endregion

        if (dtblUpdateData.Rows.Count > 0 || dtblUpdateDataJC68.Rows.Count > 0)
        {
            //更新主機資料
            htP4A_JCGR["FUNCTION_CODE"] = "C";
            htP4A_JCGR["MESSAGE_TYPE"] = "";
            htP4A_JCGR["LAST_UPD_SOURCE"] = "CSIPJCGR";//20211115_Ares_Jack_ P4A_JCGR LAST_UPD_SOURCE設為CSIPJCGR
            Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGR, htP4A_JCGR, false, "21", eAgentInfo);
            if (!htResult.Contains("HtgMsg"))
            {
                m_iCount++;
                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                base.strClientMsg += MessageHelper.GetMessage("01_01030200_013");

                if (!CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, this.txtShopId.Text.Trim(), BaseHelper.GetShowText("01_01030200_089").Trim(), (structPageInfo)Session["PageInfo"]))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }
                if (m_iCount > 0)
                {
                    InsertShopUpload(m_ShopBasicFlag);

                    //更新請款加批日志record_name ,business_name. Add by LuYang 2012/06/20
                    for (int i = 0; i < dtblUpdateData.Rows.Count; i++)
                    {
                        if (dtblUpdateData.Rows[i][EntityCUSTOMER_LOG.M_field_name].ToString() == BaseHelper.GetShowText("01_01030200_009"))
                        {
                            strBusiness_name = dtblUpdateData.Rows[i][EntityCUSTOMER_LOG.M_after].ToString();
                            break;
                        }
                    }
                    for (int i = 0; i < dtblUpdateData.Rows.Count; i++)
                    {
                        if (dtblUpdateData.Rows[i][EntityCUSTOMER_LOG.M_field_name].ToString() == BaseHelper.GetShowText("01_01030200_008"))
                        {
                            strRecord_name = dtblUpdateData.Rows[i][EntityCUSTOMER_LOG.M_after].ToString();
                            break;
                        }
                    }//end of 更新請款加批日志

                    //shop中記錄刪除被移動至P4A_JCPA中, modified by LuYang 2012/06/20
                    //EntitySHOP eShop = new EntitySHOP();
                    //BRSHOP.Delete(eShop, this.txtShopId.Text.Trim(), m_ShopBasicFlag);
                    InsertShopRpt();
                }

                #region 20190806-RQ-2019-008595-002-長姓名需求
                //如有長姓名，更新JC68資料 by Peggy
                List<EntityHTG_JC68> _JC68s = new List<EntityHTG_JC68>();
                EntityHTG_JC68 _tmpJC68 = new EntityHTG_JC68();

                //當姓名或長姓名有異動時 打JC68電文
                if (!txtUndertaker.Text.Trim().Equals("") || !txtUndertaker_L.Text.Trim().Equals(""))
                {
                    if (chkisLongName.Checked)
                    {
                        _tmpJC68 = new EntityHTG_JC68();
                        _tmpJC68.ID = txtUndertakerID.Text.Trim();
                        _tmpJC68.LONG_NAME = ToWide(txtUndertaker_L.Text.Trim());
                        _tmpJC68.PINYIN_NAME = LongNameRomaClean(txtUndertaker_Pinyin.Text.Trim());

                        _JC68s.Add(_tmpJC68);
                    }
                    else//取消長姓名
                    {
                        if (mLongName.Trim().Equals("Y"))
                        {
                            _tmpJC68 = new EntityHTG_JC68();
                            _tmpJC68.ID = txtUndertakerID.Text.Trim();
                            _tmpJC68.LONG_NAME = "";
                            _tmpJC68.PINYIN_NAME = "";

                            _JC68s.Add(_tmpJC68);
                        }
                    }
                }

                if (!txtJunctionPerson.Text.Trim().Equals("") || !txtJunctionPerson_L.Text.Trim().Equals(""))
                {
                    if (chkisLongName_c.Checked)
                    {
                        _tmpJC68 = new EntityHTG_JC68();
                        _tmpJC68.ID = lblCORP_SEQ.Text.Trim();
                        _tmpJC68.LONG_NAME = ToWide(txtJunctionPerson_L.Text.Trim());
                        _tmpJC68.PINYIN_NAME = LongNameRomaClean(txtJunctionPerson_Pinyin.Text.Trim());

                        _JC68s.Add(_tmpJC68);
                    }
                    else
                    {
                        if (mContactLName.Trim().Equals("Y"))
                        {
                            _tmpJC68 = new EntityHTG_JC68();
                            _tmpJC68.ID = lblCORP_SEQ.Text.Trim();
                            _tmpJC68.LONG_NAME = "";
                            _tmpJC68.PINYIN_NAME = "";

                            _JC68s.Add(_tmpJC68);
                        }
                    }
                }

                using (BRHTG_JC68 obj = new BRHTG_JC68("P010103020001"))
                {
                    EntityResult _EntityResult = new EntityResult();
                    int i = 0;//記錄錯誤的行數
                    foreach (EntityHTG_JC68 item in _JC68s)
                    {
                        _EntityResult = obj.Update(item, this.eAgentInfo, "21");
                        if (_EntityResult.Success == false)//錯誤訊息
                        {
                            base.strHostMsg += "更新長姓名資料:" + _EntityResult.HostMsg;
                            base.strClientMsg += "更新長姓名資料:" + _EntityResult.HostMsg;

                            dtblUpdateDataJC68.Rows[i].Delete();//更新失敗時，從異動比對刪除此筆記錄
                        }

                        i++;
                    }
                }

                dtblUpdateDataJC68.AcceptChanges();

                if (!CommonFunction.InsertCustomerLog(dtblUpdateDataJC68, eAgentInfo, this.txtShopId.Text.Trim(), BaseHelper.GetShowText("01_01030200_089").Trim(), (structPageInfo)Session["PageInfo"]))
                {
                    base.strClientMsg += "更新長姓名異動記錄查詢LOG失敗";
                }

                //20190806-RQ-2019-008595-002-長姓名需求，如有長姓名，更新JC68資料 by Peggy
                #endregion

                // 20210527 EOS_AML(NOVA) by Ares Dennis
                #region 異動記錄需報送AML  
                Hashtable htP4A_JCGR2 = new Hashtable();// 主機資料
                CommonFunction.GetViewStateHt(ViewState["HtgInfo_P4A_JCGR"], ref htP4A_JCGR2);

                bool isChanged = false;

                #region 檢核欄位是否異動
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "HEADQUARTER_CORPNO", ref isChanged);// 總公司統一編號
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "AML_CC", ref isChanged);// AML行業編號
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "ORGAN_TYPE_NEW", ref isChanged);// 法律形式
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "BUILD_DATE", ref isChanged);// 設立
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "BUILD_DATE_DD", ref isChanged);// 設立
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "REG_NAME", ref isChanged);// 登記名稱
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "OWNER_NAME", ref isChanged);// 負責人姓名
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "OWNER_ID", ref isChanged);// 負責人ID
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "OWNER_LNAM_FLAG", ref isChanged);// 負責人長姓名
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "OWNER_PHONE_AREA", ref isChanged);// 負責人電話
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "OWNER_PHONE_NO", ref isChanged);// 負責人電話
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "OWNER_PHONE_EXT", ref isChanged);// 負責人電話                
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "COUNTRY_CODE", ref isChanged);// 國籍
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "PASSPORT_NO", ref isChanged);// 護照號碼
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "PASSPORT_EXPDT", ref isChanged);// 護照效期
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "RESIDENT_NO", ref isChanged);// 統一證號
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "RESIDENT_EXPDT", ref isChanged);// 統一證號效期
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "OWNER_CITY", ref isChanged);// 戶籍地址
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "OWNER_ADDR1", ref isChanged);// 戶籍地址
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "OWNER_ADDR2", ref isChanged);// 戶籍地址
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "EMAIL", ref isChanged);// E-MAIL
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "REG_CITY", ref isChanged);// 登記地址
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "REG_ADDR1", ref isChanged);// 登記地址
                compareForAMLCheckLog(htP4A_JCGR2, htP4A_JCGR, "REG_ADDR2", ref isChanged);// 登記地址               
                #endregion
                if (isChanged)
                {
                    EntityAML_CHECKLOG eAMLCheckLog = new EntityAML_CHECKLOG();
                    //20220113_Ares_Jack_ HEADQUARTER_CORPNO 為自然人用統編 CORP_NO 帶負責人ID, 其餘帶總公司統編
                    if (involve.Contains(htP4A_JCGR["HEADQUARTER_CORPNO"].ToString().Trim()))
                        eAMLCheckLog.CORP_NO = htP4A_JCGR["OWNER_ID"].ToString().Trim();//負責人ID
                    else
                        eAMLCheckLog.CORP_NO = htP4A_JCGR["CORP_NO"].ToString().Trim();
                    eAMLCheckLog.MER_NO = htP4A_JCGR["MERCHANT_NO"].ToString().Trim();
                    eAMLCheckLog.TRANS_ID = "CSIPJCGR";
                    eAMLCheckLog.LAST_UPD_BRANCH = txtLAST_UPD_BRANCH.Text.Trim();
                    eAMLCheckLog.LAST_UPD_CHECKER = txtLAST_UPD_CHECKER.Text.Trim();
                    eAMLCheckLog.LAST_UPD_MAKER = txtLAST_UPD_MAKER.Text.Trim();
                    eAMLCheckLog.MOD_USERID = eAgentInfo.agent_id;
                    eAMLCheckLog.MOD_DATE = DateTime.Now.ToString("yyyyMMdd");
                    eAMLCheckLog.MOD_TIME = DateTime.Now.ToString("HHmmss");

                    InsertAMLCheckLog(eAMLCheckLog);
                }

                #endregion

                return true;

            }//更新主機資料
            else
            {
                //*異動主機資料失敗
                if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htResult["HtgMsg"].ToString();
                    base.strClientMsg += HtgType.P4A_JCGR.ToString() + MessageHelper.GetMessage("01_00000000_011");
                }
                else
                {
                    base.strClientMsg += htResult["HtgMsg"].ToString();
                }

                return false;
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01030200_014");
            if (m_iCount > 0)
            {
                InsertShopUpload(m_ShopBasicFlag);
                //shop中記錄刪除被移動至P4A_JCPA中, modified by LuYang 2012/06/20
                //EntitySHOP eShop = new EntitySHOP();
                //BRSHOP.Delete(eShop, this.txtShopId.Text.Trim(), m_ShopBasicFlag);
                InsertShopRpt();
            }

            return true;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 更新基本資料EXMS_P4主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateBasicP4_JCIJ()
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        Hashtable htInput = new Hashtable();
        htInput.Add("MER_NO", this.txtShopId.Text.Trim());
        htInput.Add("FUNCTION_CODE", "2");
        htInput.Add("LINE_CNT", "0000");
        htInput.Add("MESSAGE_TYPE", "");
        Hashtable htP4_JCIJ = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCIJ, htInput, false, "1", eAgentInfo);

        htP4_JCIJ["MER_NO"] = htInput["MER_NO"];//* for_xml_test
        htP4_JCIJ["MESSAGE_TYPE"] = "";
        htP4_JCIJ["LINE_CNT"] = "0000";

        if (!htP4_JCIJ.Contains("HtgMsg"))
        {
            DataTable dtblUpdateData = CommonFunction.GetDataTable();

            //*比對營業名稱
            CommonFunction.ContrastDataTwo(htP4_JCIJ, dtblUpdateData, this.txtBusinessNameTwo.Text.Trim(), "MER_NEME", BaseHelper.GetShowText("01_01030200_009"));
            //*比對聯絡人
            CommonFunction.ContrastDataTwo(htP4_JCIJ, dtblUpdateData, this.txtJunctionPerson.Text.Trim(), "OWNER_NAME", BaseHelper.GetShowText("01_01030200_020"));
            if (this.txtJunctionPersonTel1.Text.Trim() != "" || this.txtJunctionPersonTel2.Text.Trim() != "" || this.txtJunctionPersonTel3.Text.Trim() != "")
            {
                //*比對聯絡人tel
                CommonFunction.ContrastDataTwo(htP4_JCIJ, dtblUpdateData, GetTelCompages(htP4_JCIJ["CONTACT_TEL"].ToString()), "CONTACT_TEL", BaseHelper.GetShowText("01_01030200_021"));
            }

            if (this.txtJunctionPersonFax1.Text.Trim() != "" || this.txtJunctionPersonFax2.Text.Trim() != "")
            {
                //*比對聯絡人fax
                CommonFunction.ContrastDataTwo(htP4_JCIJ, dtblUpdateData, GetFaxCompages(htP4_JCIJ["CONTACT_FAX"].ToString()), "CONTACT_FAX", BaseHelper.GetShowText("01_01030200_022"));
            }
            //*比對商店營業地址add1
            CommonFunction.ContrastDataTwo(htP4_JCIJ, dtblUpdateData, this.txtJunctionPersonRealadd1.Text.Trim(), "ADDRESS1", BaseHelper.GetShowText("01_01030200_025"));
            //*比對商店營業地址add2
            CommonFunction.ContrastDataTwo(htP4_JCIJ, dtblUpdateData, this.txtJunctionPersonRealadd2.Text.Trim(), "ADDRESS2", BaseHelper.GetShowText("01_01030200_025"));
            //*比對商店營業地址add3
            CommonFunction.ContrastDataTwo(htP4_JCIJ, dtblUpdateData, this.txtJunctionPersonRealadd3.Text.Trim(), "ADDRESS3", BaseHelper.GetShowText("01_01030200_025"));

            if (dtblUpdateData.Rows.Count > 0)
            {
                //JCIJ 所有中文欄位 , 不足部份需填滿"全形"空白
                Fill_JCIJ_FullSpace(ref htP4_JCIJ);

                htP4_JCIJ["FUNCTION_CODE"] = "3";
                htP4_JCIJ["MESSAGE_TYPE"] = "";

                Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCIJ, htP4_JCIJ, false, "2", eAgentInfo);
                if (!htResult.Contains("HtgMsg"))
                {
                    m_iCount++;
                    base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                    base.strClientMsg += MessageHelper.GetMessage("01_01030200_015");
                    if (!CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, this.txtShopId.Text.Trim(), BaseHelper.GetShowText("01_01030200_088").Trim(), (structPageInfo)Session["PageInfo"]))
                    {
                        if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                        {
                            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                        }
                    }

                    if (m_iCount > 0)
                    {
                        InsertShopUpload(m_ShopBasicFlag);
                        CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop = new CSIPKeyInGUI.EntityLayer_new.EntitySHOP();
                        CSIPKeyInGUI.BusinessRules_new.BRSHOP.Delete(eShop, this.txtShopId.Text.Trim(), m_ShopBasicFlag);
                        InsertShopRpt();
                    }
                    return true;
                }
                else
                {
                    //*異動主機資料失敗
                    if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                    {
                        base.strHostMsg += htResult["HtgMsg"].ToString();
                        base.strClientMsg += HtgType.P4_JCIJ.ToString() + MessageHelper.GetMessage("01_00000000_011");
                    }
                    else
                    {
                        base.strClientMsg += htResult["HtgMsg"].ToString();
                    }

                    return false;
                }
            }
            else
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01030200_016");
                if (m_iCount > 0)
                {
                    InsertShopUpload(m_ShopBasicFlag);
                    CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop = new CSIPKeyInGUI.EntityLayer_new.EntitySHOP();
                    CSIPKeyInGUI.BusinessRules_new.BRSHOP.Delete(eShop, this.txtShopId.Text.Trim(), m_ShopBasicFlag);
                    InsertShopRpt();
                }

                return true;
            }
        }
        else
        {
            //*查詢主機資料失敗
            if (htP4_JCIJ["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            {
                base.strHostMsg += htP4_JCIJ["HtgMsg"].ToString();
                base.strClientMsg += MessageHelper.GetMessage("01_01030200_017");
            }
            else
            {
                base.strClientMsg += htP4_JCIJ["HtgMsg"].ToString();
            }

            return false;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 比較EXMS主機資料
    /// </summary>
    /// <param name="htOutput">主機資料Hashtable</param>
    /// <param name="dtblUpdateData">修改信息Datatable</param>
    private void CompareExmsHtgValue(ref Hashtable htOutput, ref DataTable dtblUpdateData)
    {
        //*比對登記名稱
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtRecordNameTwo.Text.Trim(), "REG_NAME", BaseHelper.GetShowText("01_01030200_008"));
        //*比對營業名稱
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtBusinessNameTwo.Text.Trim(), "BUSINESS_NAME", BaseHelper.GetShowText("01_01030200_009"));
        //*比對負責人
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtUndertaker.Text.Trim(), "OWNER_NAME", BaseHelper.GetShowText("01_01030200_012"));
        //*比對負責人ID
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtUndertakerID.Text.Trim().ToUpper(), "OWNER_ID", BaseHelper.GetShowText("01_01030200_013"));
        //*比對負責人tel1
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtUndertakerTel1.Text.Trim(), "OWNER_PHONE_AREA", BaseHelper.GetShowText("01_01030200_015"));
        //*比對負責人tel2
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtUndertakerTel2.Text.Trim(), "OWNER_PHONE_NO", BaseHelper.GetShowText("01_01030200_015"));
        //*比對負責人tel3
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtUndertakerTel3.Text.Trim(), "OWNER_PHONE_EXT", BaseHelper.GetShowText("01_01030200_015"));
        //*比對負責人add1
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtUndertakerAdd1.Text.Trim(), "OWNER_CITY", BaseHelper.GetShowText("01_01030200_016"));
        //*比對負責人add2
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtUndertakerAdd2.Text.Trim(), "OWNER_ADDR1", BaseHelper.GetShowText("01_01030200_016"));
        //*比對負責人add3
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtUndertakerAdd3.Text.Trim(), "OWNER_ADDR2", BaseHelper.GetShowText("01_01030200_016"));
        //*比對實際人
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtRealPerson.Text.Trim(), "MANAGER_NAME", BaseHelper.GetShowText("01_01030200_017"));
        //*比對實際人ID
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtRealPersonID.Text.Trim().ToUpper(), "MANAGER_ID", BaseHelper.GetShowText("01_01030200_018"));
        //*比對實際人tel1
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtRealPersonTel1.Text.Trim(), "MANAGER_PHONE_AREA", BaseHelper.GetShowText("01_01030200_019"));
        //*比對實際人tel2
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtRealPersonTel2.Text.Trim(), "MANAGER_PHONE_NO", BaseHelper.GetShowText("01_01030200_019"));
        //*比對實際人tel3
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtRealPersonTel3.Text.Trim(), "MANAGER_PHONE_EXT", BaseHelper.GetShowText("01_01030200_019"));
        //*比對聯絡人
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtJunctionPerson.Text.Trim(), "CONTACT_NAME", BaseHelper.GetShowText("01_01030200_020"));
        //*比對聯絡人tel1
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtJunctionPersonTel1.Text.Trim(), "CONTACT_PHONE_AREA", BaseHelper.GetShowText("01_01030200_021"));
        //*比對聯絡人tel2
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtJunctionPersonTel2.Text.Trim(), "CONTACT_PHONE_NO", BaseHelper.GetShowText("01_01030200_021"));
        //*比對聯絡人tel3
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtJunctionPersonTel3.Text.Trim(), "CONTACT_PHONE_EXT", BaseHelper.GetShowText("01_01030200_021"));
        //*比對聯絡人fax1
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtJunctionPersonFax1.Text.Trim(), "FAX_AREA", BaseHelper.GetShowText("01_01030200_022"));
        //*比對聯絡人fax2
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtJunctionPersonFax2.Text.Trim(), "FAX_PHONE_NO", BaseHelper.GetShowText("01_01030200_022"));
        //*比對商店登記地址add1
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtJunctionPersonRecadd1.Text.Trim(), "REG_CITY", BaseHelper.GetShowText("01_01030200_024"));
        //*比對商店登記地址add2
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtJunctionPersonRecadd2.Text.Trim(), "REG_ADDR1", BaseHelper.GetShowText("01_01030200_024"));
        //*比對商店登記地址add3
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtJunctionPersonRecadd3.Text.Trim(), "REG_ADDR2", BaseHelper.GetShowText("01_01030200_024"));
        //*比對商店營業地址add1
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtJunctionPersonRealadd1.Text.Trim(), "REAL_CITY", BaseHelper.GetShowText("01_01030200_025"));
        //*比對商店營業地址add2
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtJunctionPersonRealadd2.Text.Trim(), "REAL_ADDR1", BaseHelper.GetShowText("01_01030200_025"));
        //*比對商店營業地址add3
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtJunctionPersonRealadd3.Text.Trim(), "REAL_ADDR2", BaseHelper.GetShowText("01_01030200_025"));
        //*比對黑名單
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtBlack.Text.Trim(), "BLACK_CODE", BaseHelper.GetShowText("01_01030200_031"));
        //*比對推廣員
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtIntroduces.Text.Trim(), "SALE_NAME", BaseHelper.GetShowText("01_01030200_027"));
        //*比對共用類別
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtClass.Text.Trim(), "JOINT_TYPE", BaseHelper.GetShowText("01_01030200_032"));
        //*比對商店營業地址ZIP
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtRealaddZip.Text.Trim(), "REAL_ZIP", BaseHelper.GetShowText("01_01030200_025"));
        //*比對發票週期
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtInvoiceCycle.Text.Trim(), "INVOICE_CYCLKE", BaseHelper.GetShowText("01_01030200_029"));
        #region 電文新增欄位
        // 比對設立
        if (this.txtEstablish.Text.Trim().Length >= 7)
        {
            CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, RetuenCompareValue(this.txtEstablish.Text.Trim().Substring(0, 5)), "BUILD_DATE", BaseHelper.GetShowText("01_01030200_112"));
            CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, RetuenCompareValue(this.txtEstablish.Text.Trim().Substring(5, 2)), "BUILD_DATE_DD", BaseHelper.GetShowText("01_01030200_112"));
        }

        // 比對法律形式
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, RetuenCompareValue(this.txtOrganization.Text.Trim()), "ORGAN_TYPE_NEW", BaseHelper.GetShowText("01_01030200_113"));
        // 比對AML行業編號
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, RetuenCompareValue(this.txtAMLCC.Text.Trim()), "AML_CC", BaseHelper.GetShowText("01_01030200_110"));
        // 比對國籍
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, RetuenCompareValue(this.txtCountryCode.Text.Trim()), "COUNTRY_CODE", BaseHelper.GetShowText("01_01030200_106"));
        // 比對護照號碼
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtPassportNo.Text.Trim(), "PASSPORT_NO", BaseHelper.GetShowText("01_01030200_107"));
        // 比對護照效期
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtPassportExpdt.Text.Trim(), "PASSPORT_EXPDT", BaseHelper.GetShowText("01_01030200_114"));
        // 比對居留證號
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtResidentNo.Text.Trim(), "RESIDENT_NO", BaseHelper.GetShowText("01_01030200_108"));
        // 比對居留效期
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtResidentExpdt.Text.Trim(), "RESIDENT_EXPDT", BaseHelper.GetShowText("01_01030200_115"));
        // 比對E-MAIL
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.hidEmailFall.Value.Trim(), "EMAIL", BaseHelper.GetShowText("01_01030200_109"));
        // 比對總公司統一編號
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtUniNo.Text.Trim(), "HEADQUARTER_CORPNO", BaseHelper.GetShowText("01_01030200_111"));
        #endregion

        //20190806-RQ-2019-008595-002-長姓名需求 by Peggy↓
        //比對負責人中文長姓名FLAG
        if (!txtUndertakerID.Text.Trim().Equals(""))
        {
            CommonFunction.ContrastData(htOutput, dtblUpdateData, chkisLongName.Checked ? "Y" : "N", "OWNER_LNAM_FLAG", BaseHelper.GetShowText("01_01030200_116"));
        }
        // 比對聯絡人中文長姓名FLAG
        CommonFunction.ContrastData(htOutput, dtblUpdateData, chkisLongName_c.Checked ? "Y" : "N", "CONTACT_LNAM_FLAG", BaseHelper.GetShowText("01_01030200_120"));

        //20190806-RQ-2019-008595-002-長姓名需求 by Peggy↑

        //20210527 EOS_AML(NOVA) 增加欄位 by Ares Dennis
        //比對登記郵遞區號
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtREG_ZIP_CODE.Text.Trim(), "REG_ZIP_CODE", BaseHelper.GetShowText("01_01030100_126"));
        //比對資料最後異動分行
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtLAST_UPD_BRANCH.Text.Trim(), "LAST_UPD_BRANCH", BaseHelper.GetShowText("01_01030100_127"));
        //比對資料最後異動MAKER
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtLAST_UPD_MAKER.Text.Trim(), "LAST_UPD_MAKER", BaseHelper.GetShowText("01_01030100_128"));
        //比對資料最後異動CHECKER
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtLAST_UPD_CHECKER.Text.Trim(), "LAST_UPD_CHECKER", BaseHelper.GetShowText("01_01030100_129"));
    }


    /// <summary>
    /// 驗證畫面是否輸入X值
    /// </summary>
    /// <param name="inputValue">畫面輸入值</param>
    /// <param name="htgValue">主機回傳的欄位值</param>
    /// <returns></returns>
    private string RetuenCompareValue(string inputValue)
    {
        string returnValue = inputValue;
        // 畫面輸入值為X則回傳空值
        if (inputValue.ToUpper() == "X")
            returnValue = "";

        return returnValue;
    }
    #endregion

    #region 費率異動(作業畫面：P4A)
    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 更新費率異動資料PCMM_P4A主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateFeeP4A_JCHR()
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end
        
        string[] strPutRate = new string[15];

        //*初始數組值為""
        for (int i = 0; i < strPutRate.Length; i++)
        {
            strPutRate[i] = "";
        }

        Hashtable htP4A_JCHR = new Hashtable();
        CommonFunction.GetViewStateHt(ViewState["HtgInfo_P4A_JCHR"], ref htP4A_JCHR);
        DataTable dtblUpdateData = CommonFunction.GetDataTable();

        //*比對費率異動日
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtModdate.Text.Trim(), "DTE_LST_RTE_ADJ", BaseHelper.GetShowText("01_01030200_038"));

        if (this.txtAddjcb.Text.Trim() == "1" || this.txtAddjcbUs.Text.Trim() == "1")
        {
            htP4A_JCHR["CARD_STATUS7"] = "1";
        }

        if (this.txtAddjcb.Text.Trim() == "1" || this.txtAddjcbNotus.Text.Trim() == "1")
        {
            htP4A_JCHR["CARD_STATUS8"] = "1";
        }

        #region 更新主機費率- 01~費率- 15
        if (this.txtPage1.Text.Trim() != "")
        {
            strPutRate[0] = this.txtPage1.Text.Trim();
            strPutRate[2] = this.txtPage1.Text.Trim();
            strPutRate[4] = this.txtPage1.Text.Trim();
            strPutRate[6] = this.txtPage1.Text.Trim();
            //20160628 (U) by Tank, add status 10
            strPutRate[9] = this.txtPage1.Text.Trim();
        }

        if (this.txtPage2.Text.Trim() != "")
        {
            strPutRate[1] = this.txtPage2.Text.Trim();
            strPutRate[3] = this.txtPage2.Text.Trim();
            strPutRate[5] = this.txtPage2.Text.Trim();
            strPutRate[7] = this.txtPage2.Text.Trim();
            //20160628 (U) by Tank, add status 11
            strPutRate[10] = this.txtPage2.Text.Trim();
        }

        //*全卡(1-8)欄位不為空
        if (this.txtPage3.Text.Trim() != "")
        {
            strPutRate[0] = this.txtPage3.Text.Trim();
            strPutRate[1] = this.txtPage3.Text.Trim();
            strPutRate[2] = this.txtPage3.Text.Trim();
            strPutRate[3] = this.txtPage3.Text.Trim();
            strPutRate[4] = this.txtPage3.Text.Trim();
            strPutRate[5] = this.txtPage3.Text.Trim();
            strPutRate[6] = this.txtPage3.Text.Trim();
            strPutRate[7] = this.txtPage3.Text.Trim();
            //20160628 (U) by Tank, add status 10 11
            strPutRate[9] = this.txtPage3.Text.Trim();
            strPutRate[10] = this.txtPage3.Text.Trim();
        }

        if (this.txtPage4.Text.Trim() != "")
        {
            strPutRate[0] = this.txtPage4.Text.Trim();
        }
        if (this.txtRate03V.Text.Trim() != "")
        {
            strPutRate[2] = this.txtRate03V.Text.Trim();
        }
        if (this.txtRate05M.Text.Trim() != "")
        {
            strPutRate[4] = this.txtRate05M.Text.Trim();
        }
        if (this.txtRate07J.Text.Trim() != "")
        {
            strPutRate[6] = this.txtRate07J.Text.Trim();
        }
        if (this.txtRate02N.Text.Trim() != "")
        {
            strPutRate[1] = this.txtRate02N.Text.Trim();
        }
        if (this.txtRate04V.Text.Trim() != "")
        {
            strPutRate[3] = this.txtRate04V.Text.Trim();
        }
        if (this.txtRate06M.Text.Trim() != "")
        {
            strPutRate[5] = this.txtRate06M.Text.Trim();
        }
        if (this.txtRate08J.Text.Trim() != "")
        {
            strPutRate[7] = this.txtRate08J.Text.Trim();
        }

        //*9~15
        if (this.txtRate09.Text.Trim() != "")
        {
            strPutRate[8] = this.txtRate09.Text.Trim();
        }
        if (this.txtRate10.Text.Trim() != "")
        {
            strPutRate[9] = this.txtRate10.Text.Trim();
        }
        if (this.txtRate11.Text.Trim() != "")
        {
            strPutRate[10] = this.txtRate11.Text.Trim();
        }
        if (this.txtRate12.Text.Trim() != "")
        {
            strPutRate[11] = this.txtRate12.Text.Trim();
        }
        if (this.txtRate13.Text.Trim() != "")
        {
            strPutRate[12] = this.txtRate13.Text.Trim();
        }
        if (this.txtRate14.Text.Trim() != "")
        {
            strPutRate[13] = this.txtRate14.Text.Trim();
        }
        if (this.txtRate15.Text.Trim() != "")
        {
            strPutRate[14] = this.txtRate15.Text.Trim();
        }

        
        if (htP4A_JCHR["CARD_STATUS1"].ToString() == "1" && strPutRate[0] != "")
        {
            CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, strPutRate[0], "CARD_DISC_RATE1", BaseHelper.GetShowText("01_01030200_044"));
        }

        if (htP4A_JCHR["CARD_STATUS2"].ToString() == "1" && strPutRate[1] != "")
        {
            CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, strPutRate[1], "CARD_DISC_RATE2", BaseHelper.GetShowText("01_01030200_045"));
        }

        if (htP4A_JCHR["CARD_STATUS3"].ToString() == "1" && strPutRate[2] != "")
        {
            CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, strPutRate[2], "CARD_DISC_RATE3", BaseHelper.GetShowText("01_01030200_046"));
        }

        if (htP4A_JCHR["CARD_STATUS4"].ToString() == "1" && strPutRate[3] != "")
        {
            CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, strPutRate[3], "CARD_DISC_RATE4", BaseHelper.GetShowText("01_01030200_047"));
        }

        if (htP4A_JCHR["CARD_STATUS5"].ToString() == "1" && strPutRate[4] != "")
        {
            CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, strPutRate[4], "CARD_DISC_RATE5", BaseHelper.GetShowText("01_01030200_048"));
        }

        if (htP4A_JCHR["CARD_STATUS6"].ToString() == "1" && strPutRate[5] != "")
        {
            CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, strPutRate[5], "CARD_DISC_RATE6", BaseHelper.GetShowText("01_01030200_049"));
        }

        if (htP4A_JCHR["CARD_STATUS7"].ToString() == "1" && strPutRate[6] != "")
        {
            CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, strPutRate[6], "CARD_DISC_RATE7", BaseHelper.GetShowText("01_01030200_050"));
        }

        if (htP4A_JCHR["CARD_STATUS8"].ToString() == "1" && strPutRate[7] != "")
        {
            CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, strPutRate[7], "CARD_DISC_RATE8", BaseHelper.GetShowText("01_01030200_051"));
        }

        if (htP4A_JCHR["CARD_STATUS9"].ToString() == "1" && strPutRate[8] != "")
        {
            CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, strPutRate[8], "CARD_DISC_RATE9", BaseHelper.GetShowText("01_01030200_052"));
        }

        if (htP4A_JCHR["CARD_STATUS10"].ToString() == "1" && strPutRate[9] != "")
        {
            CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, strPutRate[9], "CARD_DISC_RATE10", BaseHelper.GetShowText("01_01030200_053"));
        }

        if (htP4A_JCHR["CARD_STATUS11"].ToString() == "1" && strPutRate[10] != "")
        {
            CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, strPutRate[10], "CARD_DISC_RATE11", BaseHelper.GetShowText("01_01030200_054"));
        }

        if (htP4A_JCHR["CARD_STATUS12"].ToString() == "1" && strPutRate[11] != "")
        {
            CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, strPutRate[11], "CARD_DISC_RATE12", BaseHelper.GetShowText("01_01030200_055"));
        }

        if (htP4A_JCHR["CARD_STATUS13"].ToString() == "1" && strPutRate[12] != "")
        {
            CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, strPutRate[12], "CARD_DISC_RATE13", BaseHelper.GetShowText("01_01030200_056"));
        }

        if (htP4A_JCHR["CARD_STATUS14"].ToString() == "1" && strPutRate[13] != "")
        {
            CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, strPutRate[13], "CARD_DISC_RATE14", BaseHelper.GetShowText("01_01030200_057"));
        }

        if (htP4A_JCHR["CARD_STATUS15"].ToString() == "1" && strPutRate[14] != "")
        {
            CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, strPutRate[14], "CARD_DISC_RATE15", BaseHelper.GetShowText("01_01030200_058"));
        }
         
        if (dtblUpdateData.Rows.Count > 0 )
        {
            htP4A_JCHR["FUNCTION_CODE"] = "M";
            Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCHR, htP4A_JCHR, false, "2", eAgentInfo);
            if (!htResult.Contains("HtgMsg"))
            {
                m_iCount++;
                base.strClientMsg += MessageHelper.GetMessage("01_01030200_010");
                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                if (!CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, this.txtShopId.Text.Trim(), BaseHelper.GetShowText("01_01030200_089").Trim(), (structPageInfo)Session["PageInfo"]))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }

                return true;
            }
            else
            {
                //*異動主機資料失敗
                if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htResult["HtgMsg"].ToString();
                    base.strClientMsg += HtgType.P4A_JCHR.ToString() + MessageHelper.GetMessage("01_00000000_011");
                }
                else
                {
                    base.strClientMsg += htResult["HtgMsg"].ToString();
                }

                return false;
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01030200_007");//JCHR P4A環境主機已存在相同資料,故不做異動
            return true;
        }
        #endregion
    }

    //*add by lvliangzhao 20090019 20100326 start
    /// 作者 趙呂梁
    /// 創建日期：2010/03/26
    /// 修改日期：2010/03/26
    /// <summary>
    /// 更新費率銀聯卡主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateFeeJCG1P4A()
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        Hashtable htInput = new Hashtable();
        htInput.Add("MERCH_ACCT", this.txtShopId.Text.Trim());
        htInput.Add("FUNCTION_CODE", "I");
        htInput.Add("MESSAGE_TYPE", "");

        Hashtable htJCG1 = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCG1, htInput, false, "1", eAgentInfo);
        htJCG1["MERCH_ACCT"] = htInput["MERCH_ACCT"];//* for_xml_test  模擬環境測試，正式環境可以不用賦值
        htJCG1["FUNCTION_CODE"] = "C";
        htJCG1["MESSAGE_TYPE"] = "";
        htJCG1["MESSAGE_CHI"] = "";

        if (!htJCG1.Contains("HtgMsg"))
        {
            base.strHostMsg += htJCG1["HtgSuccess"].ToString();//*主機返回成功訊息
            base.strClientMsg += MessageHelper.GetMessage("01_01030200_019");

            DataTable dtblUpdateData = CommonFunction.GetDataTable();

            //*比對銀聯卡status1
            CommonFunction.ContrastDataTwo(htJCG1, dtblUpdateData, this.txtStatus1.Text.Trim(), "CARD_STATUS1", this.lblTag1.Text + BaseHelper.GetShowText("01_01030200_102"));
            //*比對銀聯卡rate1
            CommonFunction.ContrastDataTwo(htJCG1, dtblUpdateData, this.txtFee1.Text.Trim(), "DISC_RATE1", this.lblTag1.Text + BaseHelper.GetShowText("01_01030200_103"));

            //*比對銀聯卡status2
            CommonFunction.ContrastDataTwo(htJCG1, dtblUpdateData, this.txtStatus2.Text.Trim(), "CARD_STATUS2", this.lblTag2.Text + BaseHelper.GetShowText("01_01030200_102"));
            //*比對銀聯卡rate2
            CommonFunction.ContrastDataTwo(htJCG1, dtblUpdateData, this.txtFee2.Text.Trim(), "DISC_RATE2", this.lblTag2.Text + BaseHelper.GetShowText("01_01030200_103"));

            //*比對銀聯卡status3
            CommonFunction.ContrastDataTwo(htJCG1, dtblUpdateData, this.txtStatus3.Text.Trim(), "CARD_STATUS3", this.lblTag3.Text + BaseHelper.GetShowText("01_01030200_102"));
            //*比對銀聯卡rate3
            CommonFunction.ContrastDataTwo(htJCG1, dtblUpdateData, this.txtFee3.Text.Trim(), "DISC_RATE3", this.lblTag3.Text + BaseHelper.GetShowText("01_01030200_103"));

            //*比對銀聯卡status4
            CommonFunction.ContrastDataTwo(htJCG1, dtblUpdateData, this.txtStatus4.Text.Trim(), "CARD_STATUS4", this.lblTag4.Text + BaseHelper.GetShowText("01_01030200_102"));
            //*比對銀聯卡rate4
            CommonFunction.ContrastDataTwo(htJCG1, dtblUpdateData, this.txtFee4.Text.Trim(), "DISC_RATE4", this.lblTag4.Text + BaseHelper.GetShowText("01_01030200_103"));

            //*比對銀聯卡status5
            CommonFunction.ContrastDataTwo(htJCG1, dtblUpdateData, this.txtStatus5.Text.Trim(), "CARD_STATUS5", this.lblTag5.Text + BaseHelper.GetShowText("01_01030200_102"));
            //*比對銀聯卡rate5
            CommonFunction.ContrastDataTwo(htJCG1, dtblUpdateData, this.txtFee5.Text.Trim(), "DISC_RATE5", this.lblTag5.Text + BaseHelper.GetShowText("01_01030200_103"));

            //*比對銀聯卡status6
            CommonFunction.ContrastDataTwo(htJCG1, dtblUpdateData, this.txtStatus6.Text.Trim(), "CARD_STATUS6", this.lblTag6.Text + BaseHelper.GetShowText("01_01030200_102"));
            //*比對銀聯卡rate6
            CommonFunction.ContrastDataTwo(htJCG1, dtblUpdateData, this.txtFee6.Text.Trim(), "DISC_RATE6", this.lblTag6.Text + BaseHelper.GetShowText("01_01030200_103"));

            //*比對銀聯卡status7
            CommonFunction.ContrastDataTwo(htJCG1, dtblUpdateData, this.txtStatus7.Text.Trim(), "CARD_STATUS7", this.lblTag7.Text + BaseHelper.GetShowText("01_01030200_102"));
            //*比對銀聯卡rate7
            CommonFunction.ContrastDataTwo(htJCG1, dtblUpdateData, this.txtFee7.Text.Trim(), "DISC_RATE7", this.lblTag7.Text + BaseHelper.GetShowText("01_01030200_103"));

            //*比對銀聯卡status8
            CommonFunction.ContrastDataTwo(htJCG1, dtblUpdateData, this.txtStatus8.Text.Trim(), "CARD_STATUS8", this.lblTag8.Text + BaseHelper.GetShowText("01_01030200_102"));
            //*比對銀聯卡rate8
            CommonFunction.ContrastDataTwo(htJCG1, dtblUpdateData, this.txtFee8.Text.Trim(), "DISC_RATE8", this.lblTag8.Text + BaseHelper.GetShowText("01_01030200_103"));

            //*比對銀聯卡status9
            CommonFunction.ContrastDataTwo(htJCG1, dtblUpdateData, this.txtStatus9.Text.Trim(), "CARD_STATUS9", this.lblTag9.Text + BaseHelper.GetShowText("01_01030200_102"));
            //*比對銀聯卡rate9
            CommonFunction.ContrastDataTwo(htJCG1, dtblUpdateData, this.txtFee9.Text.Trim(), "DISC_RATE9", this.lblTag9.Text + BaseHelper.GetShowText("01_01030200_103"));

            //*比對銀聯卡status10
            CommonFunction.ContrastDataTwo(htJCG1, dtblUpdateData, this.txtStatus10.Text.Trim(), "CARD_STATUS10", this.lblTag10.Text + BaseHelper.GetShowText("01_01030200_102"));
            //*比對銀聯卡rate10
            CommonFunction.ContrastDataTwo(htJCG1, dtblUpdateData, this.txtFee10.Text.Trim(), "DISC_RATE10", this.lblTag10.Text + BaseHelper.GetShowText("01_01030200_103"));

            if (dtblUpdateData.Rows.Count > 0)
            {
                Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCG1, htJCG1, false, "1", eAgentInfo);
                if (!htResult.Contains("HtgMsg"))
                {
                    m_iCount++;
                    base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                    base.strClientMsg += MessageHelper.GetMessage("01_01030200_022");
                    if (!CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, this.txtShopId.Text.Trim(), BaseHelper.GetShowText("01_01030200_089").Trim(), (structPageInfo)Session["PageInfo"]))
                    {
                        if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                        {
                            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                        }
                    }

                    return true;
                }
                else
                {
                    //*異動主機資料失敗
                    if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                    {
                        base.strHostMsg += htResult["HtgMsg"].ToString();
                        base.strClientMsg += HtgType.P4A_JCG1.ToString() + MessageHelper.GetMessage("01_00000000_011");
                    }
                    else
                    {
                        base.strClientMsg += htResult["HtgMsg"].ToString();
                    }

                    return false;
                }
            }
            else
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01030200_021");
                return true;
            }
        }
        else
        {
            if (htJCG1.Contains("HtgMsg"))
            {
                if (htJCG1["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htJCG1["HtgMsg"].ToString();
                    base.strClientMsg += MessageHelper.GetMessage("01_01030200_020");
                }
                else
                {
                    base.strClientMsg += htJCG1["HtgMsg"].ToString();
                }
            }

            return false;
        }
    }
    //*add by lvliangzhao 20090019 20100326 end
    #endregion

    #region 帳號異動(作業畫面：P4/ P4A)
    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 更新帳號異動資料PCMM_P4A主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateAccountsP4A_JCHR()
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        Hashtable htP4A_JCHR = new Hashtable();
        CommonFunction.GetViewStateHt(ViewState["HtgInfo_P4A_JCHR"], ref htP4A_JCHR);

        DataTable dtblUpdateData = CommonFunction.GetDataTable();
        //*比對帳號（1）
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtAccount1.Text.Trim(), "USER_DATA1", BaseHelper.GetShowText("01_01030200_064"));
        //*比對帳號（2）
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtAccount2.Text.Trim(), "USER_DATA2", BaseHelper.GetShowText("01_01030200_064"));

        if (dtblUpdateData.Rows.Count > 0)
        {
            htP4A_JCHR["FUNCTION_CODE"] = "M";
            Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCHR, htP4A_JCHR, false, "2", eAgentInfo);
            if (!htResult.Contains("HtgMsg"))
            {
                m_iCount++;
                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                base.strClientMsg += MessageHelper.GetMessage("01_01030200_010");
                if (!CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, this.txtShopId.Text.Trim(), BaseHelper.GetShowText("01_01030200_089").Trim(), (structPageInfo)Session["PageInfo"]))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }

                return true;
            }
            else
            {
                //*異動主機資料失敗
                if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htResult["HtgMsg"].ToString();
                    base.strClientMsg += HtgType.P4A_JCHR.ToString() + MessageHelper.GetMessage("01_00000000_011");
                }
                else
                {
                    base.strClientMsg += htResult["HtgMsg"].ToString();
                }

                return false;
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01030200_007");
            return true;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 更新帳號異動資料PCMM_P4主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateAccountsP4_JCHR()
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        Hashtable htInput = new Hashtable();
        htInput.Add("ACCT", this.txtShopId.Text.Trim());
        htInput.Add("FUNCTION_CODE", "I");
        htInput.Add("ORGN", "900");

        Hashtable htP4_JCHR = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCHR, htInput, false, "1", eAgentInfo);
        htP4_JCHR["ACCT"] = htInput["ACCT"];//* for_xml_test  模擬環境測試，正式環境可以不用賦值
        htP4_JCHR["MESSAGE_TYPE"] = "";
        htP4_JCHR["MESSAGE_CHI"] = "";

        if (!htP4_JCHR.Contains("HtgMsg"))
        {

            base.strHostMsg += htP4_JCHR["HtgSuccess"].ToString();//*主機返回成功訊息
            base.strClientMsg += MessageHelper.GetMessage("01_01030200_011");
        }
        else
        {
            if (htP4_JCHR.Contains("HtgMsg"))
            {
                if (htP4_JCHR["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htP4_JCHR["HtgMsg"].ToString();
                    base.strClientMsg += MessageHelper.GetMessage("01_01030200_008");
                }
                else
                {
                    base.strClientMsg += htP4_JCHR["HtgMsg"].ToString();
                }
            }

            return false;
        }

        DataTable dtblUpdateData = CommonFunction.GetDataTable();
        //*比對帳號（1）
        CommonFunction.ContrastDataTwo(htP4_JCHR, dtblUpdateData, this.txtAccount1.Text.Trim(), "USER_DATA1", BaseHelper.GetShowText("01_01030200_064"));
        //*比對帳號（2）
        CommonFunction.ContrastDataTwo(htP4_JCHR, dtblUpdateData, this.txtAccount2.Text.Trim(), "USER_DATA2", BaseHelper.GetShowText("01_01030200_064"));

        if (dtblUpdateData.Rows.Count > 0)
        {
            htP4_JCHR["FUNCTION_CODE"] = "M";
            htP4_JCHR["ORGN"] = "900";
            Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCHR, htP4_JCHR, false, "2", eAgentInfo);
            if (!htResult.Contains("HtgMsg"))
            {
                m_iCount++;
                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                base.strClientMsg += MessageHelper.GetMessage("01_01030200_012");
                if (!CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, this.txtShopId.Text.Trim(), BaseHelper.GetShowText("01_01030200_088").Trim(), (structPageInfo)Session["PageInfo"]))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }

                return true;
            }
            else
            {
                //*異動主機資料失敗
                if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htResult["HtgMsg"].ToString();
                    base.strClientMsg += HtgType.P4_JCHR.ToString() + MessageHelper.GetMessage("01_00000000_011");
                }
                else
                {
                    base.strClientMsg += htResult["HtgMsg"].ToString();
                }

                return false;
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01030200_009");
            return true;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 更新帳號異動資料EXMS_P4A主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateAccountsP4A_JCGR()
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        Hashtable htP4A_JCGR = new Hashtable();
        CommonFunction.GetViewStateHt(ViewState["HtgInfo_P4A_JCGR"], ref htP4A_JCGR);

        DataTable dtblUpdateData = CommonFunction.GetDataTable();

        //*比對銀行名稱
        CommonFunction.ContrastDataTwo(htP4A_JCGR, dtblUpdateData, this.txtBankName.Text.Trim(), "DDA_BANK_NAME", BaseHelper.GetShowText("01_01030200_060"));
        //*比對分行名稱
        CommonFunction.ContrastDataTwo(htP4A_JCGR, dtblUpdateData, this.txtBranchName.Text.Trim(), "DDA_BANK_BRANCH", BaseHelper.GetShowText("01_01030200_061"));
        //*比對檢號
        if (CommonFunction.ContrastDataTwo(htP4A_JCGR, dtblUpdateData, this.txtCheckNum.Text.Trim(), "CHECK_CODE", BaseHelper.GetShowText("01_01030200_063")) == 1)
        {
            CommonFunction.ContrastData(htP4A_JCGR, "N", "MERCH_VCR");
        }
        //*比對戶名
        CommonFunction.ContrastDataTwo(htP4A_JCGR, dtblUpdateData, this.txtAccount.Text.Trim(), "DDA_ACCT_NAME", BaseHelper.GetShowText("01_01030200_062"));

        if (dtblUpdateData.Rows.Count > 0)
        {
            htP4A_JCGR["FUNCTION_CODE"] = "C";
            htP4A_JCGR["MESSAGE_TYPE"] = "";

            Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGR, htP4A_JCGR, false, "21", eAgentInfo);
            if (!htResult.Contains("HtgMsg"))
            {
                m_iCount++;
                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                base.strClientMsg += MessageHelper.GetMessage("01_01030200_013");
                if (!CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, this.txtShopId.Text.Trim(), BaseHelper.GetShowText("01_01030200_089").Trim(), (structPageInfo)Session["PageInfo"]))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }

                if (m_iCount > 0)
                {
                    InsertShopUpload(m_ShopAccountFlag);
                    CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop = new CSIPKeyInGUI.EntityLayer_new.EntitySHOP();
                    CSIPKeyInGUI.BusinessRules_new.BRSHOP.Delete(eShop, this.txtShopId.Text.Trim(), m_ShopAccountFlag);
                    InsertShopRpt();
                }

                return true;
            }
            else
            {
                //*異動主機資料失敗
                if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htResult["HtgMsg"].ToString();
                    base.strClientMsg += HtgType.P4A_JCGR.ToString() + MessageHelper.GetMessage("01_00000000_011");
                }
                else
                {
                    base.strClientMsg += htResult["HtgMsg"].ToString();
                }

                return false;
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01030200_014");
            if (m_iCount > 0)
            {
                InsertShopUpload(m_ShopAccountFlag);
                CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop = new CSIPKeyInGUI.EntityLayer_new.EntitySHOP();
                CSIPKeyInGUI.BusinessRules_new.BRSHOP.Delete(eShop, this.txtShopId.Text.Trim(), m_ShopAccountFlag);
                InsertShopRpt();
            }

            return true;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 更新帳號異動資料EXMS_P4主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateAccountsP4_JCIJ()
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        Hashtable htInput = new Hashtable();
        htInput.Add("MER_NO", this.txtShopId.Text.Trim());
        htInput.Add("FUNCTION_CODE", "2");
        htInput.Add("LINE_CNT", "0000");
        htInput.Add("MESSAGE_TYPE", "");

        Hashtable htP4_JCIJ = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCIJ, htInput, false, "1", eAgentInfo);
        htP4_JCIJ["MER_NO"] = htInput["MER_NO"];//* for_xml_test
        htP4_JCIJ["MESSAGE_TYPE"] = "";
        htP4_JCIJ["LINE_CNT"] = "0000";

        if (!htP4_JCIJ.Contains("HtgMsg"))
        {
            DataTable dtblUpdateData = CommonFunction.GetDataTable();

            //*比對銀行名稱(撥款銀行及分行)
            CommonFunction.ContrastDataTwo(htP4_JCIJ, dtblUpdateData, this.txtBankName.Text.Trim() + this.txtBranchName.Text.Trim(), "BANK_NAME", BaseHelper.GetShowText("01_01030200_060"));
            //*比對戶名
            CommonFunction.ContrastDataTwo(htP4_JCIJ, dtblUpdateData, this.txtAccount.Text.Trim(), "ACCT_NEME", BaseHelper.GetShowText("01_01030200_062"));
            //*比對帳號
            CommonFunction.ContrastDataTwo(htP4_JCIJ, dtblUpdateData, this.txtAccount2.Text.Trim(), "ACCT_NO", BaseHelper.GetShowText("01_01030200_064"));

            if (dtblUpdateData.Rows.Count > 0)
            {
                //JCIJ 所有中文欄位 , 不足部份需填滿"全形"空白
                Fill_JCIJ_FullSpace(ref htP4_JCIJ);

                htP4_JCIJ["FUNCTION_CODE"] = "3";
                htP4_JCIJ["MESSAGE_TYPE"] = "";

                Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCIJ, htP4_JCIJ, false, "2", eAgentInfo);
                if (!htResult.Contains("HtgMsg"))
                {
                    m_iCount++;
                    base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                    base.strClientMsg += MessageHelper.GetMessage("01_01030200_015");
                    if (!CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, this.txtShopId.Text.Trim(), BaseHelper.GetShowText("01_01030200_088").Trim(), (structPageInfo)Session["PageInfo"]))
                    {
                        if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                        {
                            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                        }
                    }
                    if (m_iCount > 0)
                    {
                        InsertShopUpload(m_ShopAccountFlag);
                        CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop = new CSIPKeyInGUI.EntityLayer_new.EntitySHOP();
                        CSIPKeyInGUI.BusinessRules_new.BRSHOP.Delete(eShop, this.txtShopId.Text.Trim(), m_ShopAccountFlag);
                        InsertShopRpt();
                    }

                    return true;
                }
                else
                {
                    //*異動主機資料失敗
                    if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                    {
                        base.strHostMsg += htResult["HtgMsg"].ToString();
                        base.strClientMsg += HtgType.P4_JCIJ.ToString() + MessageHelper.GetMessage("01_00000000_011");
                    }
                    else
                    {
                        base.strClientMsg += htResult["HtgMsg"].ToString();
                    }

                    return false;
                }
            }
            else
            {
                base.strClientMsg += MessageHelper.GetMessage("01_01030200_016");
                if (m_iCount > 0)
                {
                    InsertShopUpload(m_ShopAccountFlag);
                    CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop = new CSIPKeyInGUI.EntityLayer_new.EntitySHOP();
                    CSIPKeyInGUI.BusinessRules_new.BRSHOP.Delete(eShop, this.txtShopId.Text.Trim(), m_ShopAccountFlag);
                    InsertShopRpt();
                }

                return true;
            }
        }
        else
        {
            //*異動主機資料失敗
            if (htP4_JCIJ["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            {
                base.strHostMsg += htP4_JCIJ["HtgMsg"].ToString();
                base.strClientMsg += MessageHelper.GetMessage("01_01030200_017");
            }
            else
            {
                base.strClientMsg += htP4_JCIJ["HtgMsg"].ToString();
            }

            return false;
        }
    }
    #endregion

    #region 解約作業(作業畫面：P4/P4A)
    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 更新解約作業PCMM_P4A主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateCancelTaskP4A_JCHR()
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        Hashtable htP4A_JCHR = new Hashtable();
        CommonFunction.GetViewStateHt(ViewState["HtgInfo_P4A_JCHR"], ref htP4A_JCHR);
        DataTable dtblUpdateData = CommonFunction.GetDataTable();
        string sCancelReasonCode = htP4A_JCHR["USER_CODE_2"].ToString();

        //*20211115_Ares_Jack_比對解約還原
        if (this.chkCancelRevert.Checked)
        {
            CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, "1", "STATUS_FLAG", BaseHelper.GetShowText("01_01030200_069A"));
        }
        //*比對解約代碼
        if (htP4A_JCHR["STATUS_FLAG"].ToString() != "8")
        {
            CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtCancelCode1.Text.Trim(), "STATUS_FLAG", BaseHelper.GetShowText("01_01030200_066"));
        }

        //*比對解約日期
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtCancelDate.Text.Trim(), "DTE_USER_1", BaseHelper.GetShowText("01_01030200_067"));
        //*比對解約原因碼
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtCancelCode2.Text.Trim().ToUpper(), "USER_CODE_2", BaseHelper.GetShowText("01_01030200_069"));

        if (dtblUpdateData.Rows.Count > 0 || chkCancelRevert.Checked)
        {
            htP4A_JCHR["FUNCTION_CODE"] = "M";

            if (this.txtCancelCode2.Text.Trim() != "")
            {
                htP4A_JCHR["USER_CODE_3"] = "A";
            }

            // 20210527 EOS_AML(NOVA) by Ares Dennis
            #region 解約/管制還原作業
            if (chkCancelRevert.Checked && eAgentInfo.roles.ToUpper().Contains("CSIP0101"))
            {
                htP4A_JCHR["USER_CODE_2"] = "";
                htP4A_JCHR["USER_CODE_3"] = "";
                htP4A_JCHR["STATUS_FLAG"] = "1";

                //20220312_Ares_Jack_解約/管制還原作業 增加至15
                for (int i = 1; i < 16; i++)
                {
                    string strCARD_STATUS = "CARD_STATUS" + i;
                    if (htP4A_JCHR[strCARD_STATUS].ToString().Trim() == "8")
                        htP4A_JCHR[strCARD_STATUS] = "1";
                }
            }
            #endregion

            if (chkCancelRevert.Checked)
            {
                // 20211224_Ares_Jack_主機資料
                Hashtable htP4A_JCHR_QUERY = new Hashtable();
                htP4A_JCHR_QUERY.Add("ACCT", this.txtShopId.Text.Trim());
                htP4A_JCHR_QUERY.Add("FUNCTION_CODE", "I");
                htP4A_JCHR_QUERY.Add("ORGN", "822");
                Hashtable htP4A_JCHR_RTN = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCHR, htP4A_JCHR_QUERY, false, "21", eAgentInfo);

                htP4A_JCHR["DTE_USER_1"] = htP4A_JCHR_RTN["DTE_USER_1"];//解約還原 將解約日期保持跟查詢主機時一樣
            }

            Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCHR, htP4A_JCHR, false, "2", eAgentInfo);
            if (!htResult.Contains("HtgMsg"))
            {
                m_iCount++;
                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                base.strClientMsg += MessageHelper.GetMessage("01_01030200_010");
                if (!CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, this.txtShopId.Text.Trim(), BaseHelper.GetShowText("01_01030200_089").Trim(), (structPageInfo)Session["PageInfo"]))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }

                // 20210527 EOS_AML(NOVA) by Ares Dennis
                #region 異動記錄需報送AML  
                Hashtable htP4A_JCHR2 = new Hashtable();// 主機資料
                CommonFunction.GetViewStateHt(ViewState["HtgInfo_P4A_JCHR"], ref htP4A_JCHR2);
                bool isChanged = false;

                #region 檢核欄位是否異動
                compareForAMLCheckLog(htP4A_JCHR2, htP4A_JCHR, "STATUS_FLAG", ref isChanged);// 解約代號
                compareForAMLCheckLog(htP4A_JCHR2, htP4A_JCHR, "DTE_USER_1", ref isChanged);// 解約日期
                compareForAMLCheckLog(htP4A_JCHR2, htP4A_JCHR, "USER_CODE_2", ref isChanged);// 解約原因碼
                #endregion
                if (isChanged)
                {
                    Hashtable htP4A_JCGR = new Hashtable();// 20211110_Ares_Jack_主機資料
                    htP4A_JCGR.Add("FUNCTION_CODE", "I");
                    htP4A_JCGR.Add("MERCHANT_NO", this.txtShopId.Text);
                    Hashtable htP4A_JCGR2 = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGR, htP4A_JCGR, false, "21", eAgentInfo);

                    EntityAML_CHECKLOG eAMLCheckLog = new EntityAML_CHECKLOG();
                    //eAMLCheckLog.CORP_NO = htP4A_JCHR["DB_ACCT_NMBR"].ToString().Trim();
                    //20211110_Ares_Jack_ HEADQUARTER_CORPNO 為自然人用統編 CORP_NO 帶負責人ID, 其餘帶總公司統編
                    if (involve.Contains(htP4A_JCGR2["HEADQUARTER_CORPNO"].ToString().Trim()))
                        eAMLCheckLog.CORP_NO = htP4A_JCGR2["OWNER_ID"].ToString().Trim();//負責人ID
                    else
                        eAMLCheckLog.CORP_NO = htP4A_JCGR2["CORP_NO"].ToString().Trim();
                    eAMLCheckLog.MER_NO = htP4A_JCHR["ACCT"].ToString().Trim();
                    eAMLCheckLog.TRANS_ID = "CSIPJCHR";
                    eAMLCheckLog.LAST_UPD_BRANCH = txtLAST_UPD_BRANCH2.Text.Trim();
                    eAMLCheckLog.LAST_UPD_CHECKER = txtLAST_UPD_CHECKER2.Text.Trim();
                    eAMLCheckLog.LAST_UPD_MAKER = txtLAST_UPD_MAKER2.Text.Trim();
                    eAMLCheckLog.MOD_USERID = eAgentInfo.agent_id;
                    eAMLCheckLog.MOD_DATE = DateTime.Now.ToString("yyyyMMdd");
                    eAMLCheckLog.MOD_TIME = DateTime.Now.ToString("HHmmss");

                    InsertAMLCheckLog(eAMLCheckLog);
                }
                #endregion

                return true;
            }
            else
            {
                //*異動主機資料失敗
                if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htResult["HtgMsg"].ToString();
                    if (htResult["MESSAGE_TYPE"].ToString() == "0053")
                    {
                        base.strHostMsg += " ． 原解約原因碼已是 " + sCancelReasonCode + ",無權修改";
                    }
                    base.strClientMsg += HtgType.P4A_JCHR.ToString() + MessageHelper.GetMessage("01_00000000_011");
                }
                else
                {
                    base.strClientMsg += htResult["HtgMsg"].ToString();
                }

                return false;
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01030200_007");
            return true;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 更新解約作業PCMM_P4主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateCancelTaskP4_JCHR()
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        Hashtable htInput = new Hashtable();
        htInput.Add("ACCT", this.txtShopId.Text.Trim());
        htInput.Add("FUNCTION_CODE", "I");
        htInput.Add("ORGN", "900");

        Hashtable htP4_JCHR = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCHR, htInput, false, "1", eAgentInfo);

        // 20210527 EOS_AML(NOVA) by Ares Dennis start
        Hashtable htP4_JCHR2 = htP4_JCHR;// 主機資料
        // 20210527 EOS_AML(NOVA) by Ares Dennis end

        htP4_JCHR["ACCT"] = htInput["ACCT"];//* for_xml_test  模擬環境測試，正式環境可以不用賦值
        htP4_JCHR["MESSAGE_TYPE"] = "";
        htP4_JCHR["MESSAGE_CHI"] = "";
        string sCancelReasonCode = htP4_JCHR["USER_CODE_2"].ToString();

        if (!htP4_JCHR.Contains("HtgMsg"))
        {

            base.strHostMsg += htP4_JCHR["HtgSuccess"].ToString();//*主機返回成功訊息
            base.strClientMsg += MessageHelper.GetMessage("01_01030200_011");
        }
        else
        {
            if (htP4_JCHR.Contains("HtgMsg"))
            {
                if (htP4_JCHR["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htP4_JCHR["HtgMsg"].ToString();
                    base.strClientMsg += MessageHelper.GetMessage("01_01030200_008");
                }
                else
                {
                    base.strClientMsg += htP4_JCHR["HtgMsg"].ToString();
                }
            }

            return false;
        }

        DataTable dtblUpdateData = CommonFunction.GetDataTable();

        //*20211115_Ares_Jack_比對解約還原
        if (this.chkCancelRevert.Checked)
        {
            CommonFunction.ContrastDataTwo(htP4_JCHR, dtblUpdateData, "1", "STATUS_FLAG", BaseHelper.GetShowText("01_01030200_069A"));
        }
        //*比對解約代碼
        if (htP4_JCHR["STATUS_FLAG"].ToString() != "8")
        {
            CommonFunction.ContrastDataTwo(htP4_JCHR, dtblUpdateData, this.txtCancelCode1.Text.Trim(), "STATUS_FLAG", BaseHelper.GetShowText("01_01030200_066"));
        }

        //*比對解約日期
        CommonFunction.ContrastDataTwo(htP4_JCHR, dtblUpdateData, this.txtCancelDate.Text.Trim(), "DTE_USER_1", BaseHelper.GetShowText("01_01030200_067"));
        //*比對解約原因碼
        CommonFunction.ContrastDataTwo(htP4_JCHR, dtblUpdateData, this.txtCancelCode2.Text.Trim().ToUpper(), "USER_CODE_2", BaseHelper.GetShowText("01_01030200_069"));

        if (dtblUpdateData.Rows.Count > 0 || chkCancelRevert.Checked)
        {
            htP4_JCHR["FUNCTION_CODE"] = "M";
            if (this.txtCancelCode2.Text.Trim() != "")
            {
                htP4_JCHR["USER_CODE_3"] = "A";
            }
            htP4_JCHR["ORGN"] = "900";

            // 20210527 EOS_AML(NOVA) by Ares Dennis
            #region 解約/管制還原作業
            if (chkCancelRevert.Checked)
            {
                htP4_JCHR["USER_CODE_2"] = "";
                htP4_JCHR["USER_CODE_3"] = "";
                htP4_JCHR["STATUS_FLAG"] = "1";

                //20220312_Ares_Jack_解約/管制還原作業 增加至15
                for (int i = 1; i < 16; i++)
                {
                    string strCARD_STATUS = "CARD_STATUS" + i;
                    if (htP4_JCHR[strCARD_STATUS].ToString().Trim() == "8")
                        htP4_JCHR[strCARD_STATUS] = "1";
                }
            }
            #endregion

            Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCHR, htP4_JCHR, false, "2", eAgentInfo);
            if (!htResult.Contains("HtgMsg"))
            {
                m_iCount++;
                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                base.strClientMsg += MessageHelper.GetMessage("01_01030200_012");
                if (!CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, this.txtShopId.Text.Trim(), BaseHelper.GetShowText("01_01030200_088").Trim(), (structPageInfo)Session["PageInfo"]))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }

                // 20210527 EOS_AML(NOVA) by Ares Dennis
                #region 異動記錄需報送AML                  
                bool isChanged = false;

                #region 檢核欄位是否異動
                compareForAMLCheckLog(htP4_JCHR2, htP4_JCHR, "STATUS_FLAG", ref isChanged);// 解約代號
                compareForAMLCheckLog(htP4_JCHR2, htP4_JCHR, "DTE_USER_1", ref isChanged);// 解約日期
                compareForAMLCheckLog(htP4_JCHR2, htP4_JCHR, "USER_CODE_2", ref isChanged);// 解約原因碼
                #endregion
                if (isChanged)
                {
                    Hashtable htP4A_JCGR = new Hashtable();// 20211110_Ares_Jack_主機資料
                    htP4A_JCGR.Add("FUNCTION_CODE", "I");
                    htP4A_JCGR.Add("MERCHANT_NO", this.txtShopId.Text);
                    Hashtable htP4A_JCGR2 = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGR, htP4A_JCGR, false, "21", eAgentInfo);

                    EntityAML_CHECKLOG eAMLCheckLog = new EntityAML_CHECKLOG();
                    //eAMLCheckLog.CORP_NO = htP4_JCHR["DB_ACCT_NMBR"].ToString().Trim();
                    //20211110_Ares_Jack_ HEADQUARTER_CORPNO 為自然人用統編 CORP_NO 帶負責人ID, 其餘帶總公司統編
                    if (involve.Contains(htP4A_JCGR2["HEADQUARTER_CORPNO"].ToString().Trim()))
                        eAMLCheckLog.CORP_NO = htP4A_JCGR2["OWNER_ID"].ToString().Trim();//負責人ID
                    else
                        eAMLCheckLog.CORP_NO = htP4A_JCGR2["CORP_NO"].ToString().Trim();
                    eAMLCheckLog.MER_NO = htP4_JCHR["ACCT"].ToString().Trim();
                    eAMLCheckLog.TRANS_ID = "CSIPJCHR";
                    eAMLCheckLog.LAST_UPD_BRANCH = txtLAST_UPD_BRANCH2.Text.Trim();
                    eAMLCheckLog.LAST_UPD_CHECKER = txtLAST_UPD_CHECKER2.Text.Trim();
                    eAMLCheckLog.LAST_UPD_MAKER = txtLAST_UPD_MAKER2.Text.Trim();
                    eAMLCheckLog.MOD_USERID = eAgentInfo.agent_id;
                    eAMLCheckLog.MOD_DATE = DateTime.Now.ToString("yyyyMMdd");
                    eAMLCheckLog.MOD_TIME = DateTime.Now.ToString("HHmmss");

                    InsertAMLCheckLog(eAMLCheckLog);
                }
                #endregion

                return true;
            }
            else
            {
                //*異動主機資料失敗
                if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htResult["HtgMsg"].ToString();

                    if (htResult["MESSAGE_TYPE"].ToString() == "0053")
                    {
                        base.strHostMsg += " ． 原解約原因碼已是 " + sCancelReasonCode + ",無權修改";
                    }
                    base.strClientMsg += HtgType.P4_JCHR + MessageHelper.GetMessage("01_00000000_011");
                }
                else
                {
                    base.strClientMsg += htResult["HtgMsg"].ToString();
                }

                return false;
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01030200_009");
            return true;
        }
    }
    #endregion

    #region 機器資料(作業畫面：P4A)
    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 更新機器資料PCMM_P4A主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateMachineP4A_JCHR()
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        Hashtable htP4A_JCHR = new Hashtable();
        CommonFunction.GetViewStateHt(ViewState["HtgInfo_P4A_JCHR"], ref htP4A_JCHR);
        DataTable dtblUpdateData = CommonFunction.GetDataTable();

        //*比對IMP1數量
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtImp1.Text.Trim(), "NBR_IMPRINTER1", BaseHelper.GetShowText("01_01030200_071"));
        //*比對pos1數量
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtPos1.Text.Trim(), "NBR_IMPRINTER2", BaseHelper.GetShowText("01_01030200_074"));
        //*比對EDC1數量
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtEdc1.Text.Trim(), "NBR_IMPRINTER3", BaseHelper.GetShowText("01_01030200_077"));
        //*比對imp2數量
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtImp2.Text.Trim(), "NBR_POS_DEV1", BaseHelper.GetShowText("01_01030200_072"));
        //*比對 pos2數量
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtPos2.Text.Trim(), "NBR_POS_DEV2", BaseHelper.GetShowText("01_01030200_075"));
        //*比對 edc編號數量
        CommonFunction.ContrastDataTwo(htP4A_JCHR, dtblUpdateData, this.txtEdc.Text.Trim(), "NBR_POS_DEV3", BaseHelper.GetShowText("01_01030200_078"));

        if (dtblUpdateData.Rows.Count > 0)
        {
            htP4A_JCHR["FUNCTION_CODE"] = "M";
            Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCHR, htP4A_JCHR, false, "2", eAgentInfo);
            if (!htResult.Contains("HtgMsg"))
            {
                m_iCount++;
                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                base.strClientMsg += MessageHelper.GetMessage("01_01030200_010");
                if (!CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, this.txtShopId.Text.Trim(), BaseHelper.GetShowText("01_01030200_089").Trim(), (structPageInfo)Session["PageInfo"]))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }

                return true;
            }
            else
            {
                //*異動主機資料失敗
                if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htResult["HtgMsg"].ToString();
                    base.strClientMsg += HtgType.P4A_JCHR.ToString() + MessageHelper.GetMessage("01_00000000_011");
                }
                else
                {
                    base.strClientMsg += htResult["HtgMsg"].ToString();
                }

                return false;
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01030200_007");
            return true;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 更新機器資料EXMS_P4A主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateMachineP4A_JCGR()
    {
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
        etMstType = eMstType.Control;
        //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

        Hashtable htP4A_JCGR = new Hashtable();
        CommonFunction.GetViewStateHt(ViewState["HtgInfo_P4A_JCGR"], ref htP4A_JCGR);
        DataTable dtblUpdateData = CommonFunction.GetDataTable();

        //*比對IMP1機型
        CommonFunction.ContrastDataTwo(htP4A_JCGR, dtblUpdateData, this.txtImp1Type.Text.Trim(), "IMPRINTER_TYPE1", BaseHelper.GetShowText("01_01030200_090"));
        //*比對IMP1數量
        CommonFunction.ContrastDataTwo(htP4A_JCGR, dtblUpdateData, BRCommon.GetPadLeftString(this.txtImp1.Text.Trim(), 3, '0'), "IMPRINTER_QTY1", BaseHelper.GetShowText("01_01030200_091"));
        //*比對IMP2機型
        CommonFunction.ContrastDataTwo(htP4A_JCGR, dtblUpdateData, this.txtImp2Type.Text.Trim(), "IMPRINTER_TYPE2", BaseHelper.GetShowText("01_01030200_092"));
        //*比對IMP2數量
        CommonFunction.ContrastDataTwo(htP4A_JCGR, dtblUpdateData, BRCommon.GetPadLeftString(this.txtImp2.Text.Trim(), 3, '0'), "IMPRINTER_QTY2", BaseHelper.GetShowText("01_01030200_093"));
        //*比對ＩＭＰ保證金（保證金1）
        CommonFunction.ContrastDataTwo(htP4A_JCGR, dtblUpdateData, this.txtImpMoney.Text.Trim(), "IMPRINTER_DEPO", BaseHelper.GetShowText("01_01030200_073"));

        //*比對POS1機型
        CommonFunction.ContrastDataTwo(htP4A_JCGR, dtblUpdateData, this.txtPos1Type.Text.Trim(), "POS_TYPE1", BaseHelper.GetShowText("01_01030200_094"));
        //*比對POS1數量
        CommonFunction.ContrastDataTwo(htP4A_JCGR, dtblUpdateData, BRCommon.GetPadLeftString(this.txtPos1.Text.Trim(), 3, '0'), "POS_QTY1", BaseHelper.GetShowText("01_01030200_095"));
        //*比對POS2機型
        CommonFunction.ContrastDataTwo(htP4A_JCGR, dtblUpdateData, this.txtPos2Type.Text.Trim(), "POS_TYPE2", BaseHelper.GetShowText("01_01030200_096"));
        //*比對POS2數量
        CommonFunction.ContrastDataTwo(htP4A_JCGR, dtblUpdateData, BRCommon.GetPadLeftString(this.txtPos2.Text.Trim(), 3, '0'), "POS_QTY2", BaseHelper.GetShowText("01_01030200_097"));
        //*比對POS保證金（保證金2）
        CommonFunction.ContrastDataTwo(htP4A_JCGR, dtblUpdateData, this.txtPosMoney.Text.Trim(), "POS_DEPO", BaseHelper.GetShowText("01_01030200_076"));

        //*比對EDC1機型
        CommonFunction.ContrastDataTwo(htP4A_JCGR, dtblUpdateData, this.txtEdc1Type.Text.Trim(), "EDC_TYPE1", BaseHelper.GetShowText("01_01030200_098"));
        //*比對EDC1數量
        CommonFunction.ContrastDataTwo(htP4A_JCGR, dtblUpdateData, BRCommon.GetPadLeftString(this.txtEdc1.Text.Trim(), 3, '0'), "EDC_QTY1", BaseHelper.GetShowText("01_01030200_099"));
        //*比對EDC編號機型
        CommonFunction.ContrastDataTwo(htP4A_JCGR, dtblUpdateData, this.txtEdcType.Text.Trim(), "EDC_TYPE2", BaseHelper.GetShowText("01_01030200_100"));
        //*比對EDC編號數量
        CommonFunction.ContrastDataTwo(htP4A_JCGR, dtblUpdateData, BRCommon.GetPadLeftString(this.txtEdc.Text.Trim(), 3, '0'), "EDC_QTY2", BaseHelper.GetShowText("01_01030200_101"));
        //*比對EDC保證金（保證金3）
        CommonFunction.ContrastDataTwo(htP4A_JCGR, dtblUpdateData, this.txtEdcMoney.Text.Trim(), "EDC_DEPO", BaseHelper.GetShowText("01_01030200_079"));

        if (dtblUpdateData.Rows.Count > 0)
        {
            htP4A_JCGR["FUNCTION_CODE"] = "C";
            htP4A_JCGR["MESSAGE_TYPE"] = "";
            htP4A_JCGR["LAST_UPD_SOURCE"] = "CSIPJCGR";//20211115_Ares_Jack_ P4A_JCGR LAST_UPD_SOURCE設為CSIPJCGR
            Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGR, htP4A_JCGR, false, "21", eAgentInfo);
            if (!htResult.Contains("HtgMsg"))
            {
                m_iCount++;
                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                base.strClientMsg += MessageHelper.GetMessage("01_01030200_013");
                if (!CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, this.txtShopId.Text.Trim(), BaseHelper.GetShowText("01_01030200_089").Trim(), (structPageInfo)Session["PageInfo"]))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }
                if (m_iCount > 0)
                {
                    InsertShopUpload(m_ShopMaFlag);
                    CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop = new CSIPKeyInGUI.EntityLayer_new.EntitySHOP();
                    CSIPKeyInGUI.BusinessRules_new.BRSHOP.Delete(eShop, this.txtShopId.Text.Trim(), m_ShopMaFlag);
                    InsertShopRpt();
                }

                return true;
            }
            else
            {
                //*異動主機資料失敗
                if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strHostMsg += htResult["HtgMsg"].ToString();
                    base.strClientMsg += HtgType.P4A_JCGR.ToString() + MessageHelper.GetMessage("01_00000000_011");
                }
                else
                {
                    base.strClientMsg += htResult["HtgMsg"].ToString();
                }

                return false;
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01030200_014");
            if (m_iCount > 0)
            {
                InsertShopUpload(m_ShopMaFlag);
                CSIPKeyInGUI.EntityLayer_new.EntitySHOP eShop = new CSIPKeyInGUI.EntityLayer_new.EntitySHOP();
                CSIPKeyInGUI.BusinessRules_new.BRSHOP.Delete(eShop, this.txtShopId.Text.Trim(), m_ShopMaFlag);
                InsertShopRpt();
            }

            return true;
        }
    }
    #endregion

    private void Fill_JCIJ_FullSpace(ref Hashtable htP4_JCIJ)
    {
        htP4_JCIJ["ACCT_NEME"] = htP4_JCIJ["ACCT_NEME"].ToString().PadRight(29, '　');
        htP4_JCIJ["ADDRESS1"] = htP4_JCIJ["ADDRESS1"].ToString().PadRight(14, '　');
        htP4_JCIJ["ADDRESS2"] = htP4_JCIJ["ADDRESS2"].ToString().PadRight(29, '　');
        htP4_JCIJ["ADDRESS3"] = htP4_JCIJ["ADDRESS3"].ToString().PadRight(29, '　');
        htP4_JCIJ["BANK_NAME"] = htP4_JCIJ["BANK_NAME"].ToString().PadRight(29, '　');
        htP4_JCIJ["DESCRIPTION"] = htP4_JCIJ["DESCRIPTION"].ToString().PadRight(4, '　');
        htP4_JCIJ["MER_NEME"] = htP4_JCIJ["MER_NEME"].ToString().PadRight(14, '　');
        htP4_JCIJ["OWNER_NAME"] = htP4_JCIJ["OWNER_NAME"].ToString().PadRight(14, '　');
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
                CtrlName = "chkisLongName";//人員名稱
                CtrlName1 = "txtUndertaker";//負責人
                break;
            case "txtJunctionPerson_L"://聯絡人
                CtrlName = "chkisLongName_c";//人員名稱
                CtrlName1 = "txtJunctionPerson";//中文長姓名
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


    private EntityHTG_JC68 GetJC68(string strID)
    {
        EntityHTG_JC68 _result = new EntityHTG_JC68();
        using (BRHTG_JC68 obj = new BRHTG_JC68("P010103020001"))
        {
            EntityHTG_JC68 _data = new EntityHTG_JC68();

            _data.ID = strID;
            _result = obj.getData(_data, eAgentInfo, "11");
        }
        return _result;
    }
    #endregion

    protected void btnSubmitByTaxno_Click(object sender, EventArgs e)
    {
        #region 欄位檢核
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

        // 檢查護照效期
        if (!string.IsNullOrEmpty(txtPrincipalPassportExpdt.Text.Trim()) && txtPrincipalPassportExpdt.Text.Trim().ToUpper() != "X")
        {
            if (!checkDateTime(txtPrincipalPassportExpdt.Text.Trim()))
            {
                base.sbRegScript.Append("alert('護照效期格式錯誤');$('#txtPrincipalPassportExpdt').focus();");
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

        if (!chkCheckSum.Checked)
        {
            base.sbRegScript.Append("alert('內容檢核未勾選');$('#chkCheckSum').focus();");
            return;
        }

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
        eShopChange.PrincipalNameEN = txtPrincipalNameEN.Text.Trim();
        eShopChange.PrincipalName_PINYIN = txtPrincipalName_PINYIN.Text.Trim();
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
        eShopChange.KeyIn_Flag = "2";
        eShopChange.isCHECK = "";
        eShopChange.MOD_DATE = DateTime.Now.ToString("yyyyMMdd");
        eShopChange.MOD_USER = eAgentInfo.agent_id;
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
        eChangeLog.KeyinFLAG = "2";
        eChangeLog.MOD_USER = eAgentInfo.agent_id;
        eChangeLog.MOD_DATE = DateTime.Now.ToString("yyyyMMdd");
        #endregion

        if (CSIPKeyInGUI.BusinessRules_new.BRSHOP_CHANGE.Insert(eShopChange, this.txtUNI_NO1.Text.Trim(), m_TwoKeyFlag))
        {
            BRShopChange_LOG.Insert(eChangeLog, txtDOC_ID.Text.Trim(), m_TwoKeyFlag);
            //*資料異動成功
            //欄位順序與比對畫面順序相關，要注意
            string strColumns = @"[CORP_NO],[CORP_MCC],[CORP_ESTABLISH],[CORP_Organization],[CORP_CountryCode],[CORP_CountryStateCode],[REG_NAME_CH],[REG_NAME_EN]
                                ,[REG_ZIP_CODE],[REG_CITY],[REG_ADDR1],[REG_ADDR2],[CORP_TEL1],[CORP_TEL2],[CORP_TEL3],[PrincipalNameCH],[PrincipalName_L],[PrincipalName_PINYIN],[PrincipalNameEN]
                                ,[PrincipalIDNo],[PrincipalBirth],[PrincipalIssueDate],[PrincipalIssuePlace],[PrincipalReplaceType],[PrincipalCountryCode]
                                ,[PrincipalPassportNo],[PrincipalPassportExpdt],[PrincipalResidentNo],[PrincipalResidentExpdt],[Principal_TEL1],[Principal_TEL2],[Principal_TEL3]
                                ,[HouseholdCITY],[HouseholdADDR1],[HouseholdADDR2],[ARCHIVE_NO],[LAST_UPD_MAKER],[LAST_UPD_CHECKER],[LAST_UPD_BRANCH],[DOC_ID] ";

            if (CompareKeyByTaxNo(pnlBasicByTaxno, eShopChange, strColumns))
            {
                BRSHOP_CHANGE.UpdateShopChangeFlag(txtUNI_NO1.Text.Trim(), "2", "Y");
                BRSHOP_CHANGE.UpdateShopChangeFlag(txtUNI_NO1.Text.Trim(), "1", "Y");

                base.strClientMsg += MessageHelper.GetMessage("01_00000000_005");
                strClientMsg += "二KEY鍵檔同仁：" + eAgentInfo.agent_id;
                ClearAll();
            }
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_006");
        }

        sbRegScript.Append(@"alert('" + base.strClientMsg + "');");
    }

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
        EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
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
                if (SelectOneKeyData(m_ShopBasicByTaxNoFlag))
                {
                    strColumns = @"[CORP_NO],[CORP_MCC],[CORP_ESTABLISH],[CORP_Organization],[CORP_CountryCode],[CORP_CountryStateCode],[REG_NAME_CH],[REG_NAME_EN]
                                ,[REG_ZIP_CODE],[REG_CITY],[REG_ADDR1],[REG_ADDR2],[CORP_TEL1],[CORP_TEL2],[CORP_TEL3],[PrincipalNameCH],[PrincipalName_L],[PrincipalName_PINYIN],[PrincipalNameEN]
                                ,[PrincipalIDNo],[PrincipalBirth],[PrincipalIssueDate],[PrincipalIssuePlace],[PrincipalReplaceType],[PrincipalCountryCode]
                                ,[PrincipalPassportNo],[PrincipalPassportExpdt],[PrincipalResidentNo],[PrincipalResidentExpdt],[Principal_TEL1],[Principal_TEL2],[Principal_TEL3]
                                ,[HouseholdCITY],[HouseholdADDR1],[HouseholdADDR2],[ARCHIVE_NO],[DOC_ID],[LAST_UPD_MAKER],[LAST_UPD_CHECKER],[LAST_UPD_BRANCH] ";

                    string[] aa = strColumns.Split(',');
                    SelectData(this.pnlBasicByTaxno, m_ShopBasicByTaxNoFlag, strColumns);

                    base.sbRegScript.Append(BaseHelper.SetFocus("txtCORP_NO"));

                    if (!txtPrincipalName_L.Text.Trim().Equals(""))
                    {
                        chkisLongName2.Checked = true;
                    }

                    CheckBox_CheckedChanged(chkisLongName2, null);
                    txtDOC_ID.Text = strDOCID.Trim();

                    txtREG_ZIP_CODE2.Enabled = false;
                    txtREG_ZIP_CODE2.BackColor = Color.LightGray;
                }
                else
                {
                    CommonFunction.SetControlsEnabled(pnlBasicByTaxno, false);
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtUNI_NO1"));
                    return;
                }
            }
        }
        else
        {
            EnabledControls(false);
        }

        #endregion
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 比較一、二KEY資料是否相同
    /// </summary>
    /// <param name="pnlType">所在panel</param>
    /// <param name="strColumns">欄位的字符串</param>
    /// <returns>true 相同，false 不同</returns>
    private bool CompareKeyByTaxNo(CustPanel pnlType, CSIPKeyInGUI.EntityLayer_new.EntitySHOP_CHANGE eShopChange, string strColumns)
    {
        bool blnSame = true;
        // 查詢一Key資料
        DataSet dstInfo = CSIPKeyInGUI.BusinessRules_new.BRSHOP_CHANGE.Select(this.txtUNI_NO1.Text.Trim(), m_KeyFlag, strColumns, "");

        if (dstInfo == null)
        {
            if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            }
            return false;
        }

        if (dstInfo.Tables[0].Rows.Count > 0)
        {
            int i = 0;//*記錄不相同的數量
            int j = 0;
            string strValue = string.Empty;

            foreach (System.Web.UI.Control control in pnlType.Controls)
            {
                if (control is CustTextBox)
                {
                    CustTextBox txtBox = (CustTextBox)control;
                    strValue = txtBox.Text.Trim();

                    if (txtBox.ID == "txtDOC_ID")
                    {
                        continue;
                    }

                    string columnName = dstInfo.Tables[0].Columns[j].ToString();
                    if (strValue.ToUpper() != dstInfo.Tables[0].Rows[0][j].ToString().Trim().ToUpper())
                    {
                        if (i == 0)
                        {
                            base.sbRegScript.Append(BaseHelper.SetFocus(txtBox.ID));
                        }
                        txtBox.BackColor = Color.Red;
                        blnSame = false;
                        i++;
                    }
                    else
                    {
                        txtBox.BackColor = Color.Empty;
                    }
                    j++;
                }
            }
        }

        if (!blnSame)
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_009");
        }

        return blnSame;
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

        Hashtable hstPcmmP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC69, htInput, false, "1", eAgentInfo);

        if (!hstPcmmP4A.Contains("HtgMsg"))
        {
            //2021/03/25_Ares_Stanley_移除空白字符避免斷行
            this.lblREG_NAME.Text = hstPcmmP4A["REG_NAME"].ToString().Trim();
            this.lblOWNER_CHINESE_NAME.Text = hstPcmmP4A["OWNER_CHINESE_NAME"].ToString().Trim();

            base.strHostMsg += hstPcmmP4A["HtgSuccess"].ToString();//*主機返回成功訊息
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_012");
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