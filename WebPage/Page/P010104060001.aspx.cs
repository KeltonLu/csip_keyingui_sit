//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：特店基本資料修改(6001修改)
//*  創建日期：2009/11/23 
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
//20190805 (U) by Peggy, 新增控制項：因長姓名需求，增加是否長姓名，負責人中文長姓名、負責人羅馬拼音、是否聯絡人長姓名、聯絡人長姓名、聯絡人羅馬拼音 6個控制項

using System;
using System.Data;
using System.Collections;
using System.Web.UI.WebControls;
using Framework.Common.Message;
using CSIPCommonModel.EntityLayer;
using CSIPKeyInGUI.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.BusinessRules_new;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using System.Drawing;
using CSIPCommonModel.EntityLayer_new;
using System.Collections.Generic;
using System.Text;

public partial class P010104060001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    //20190806-RQ-2019-008595-002-長姓名需求，如有長姓名，更新JC68資料 by Peggy
    List<EntityHTG_JC68> _JC68s = new List<EntityHTG_JC68>();
    EntityHTG_JC68 _tmpJC68 = new EntityHTG_JC68();
    //20191023 修改：SOC所需資訊  by Peggy
    private structPageInfo sPageInfo;//*記錄網頁訊息

    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CommonFunction.SetControlsEnabled(pnlText, false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo1"));
            this.cddl_MemberService.Items.Add(new ListItem("", ""));
            this.cddl_MemberService.Items.Add(new ListItem("Y", "Y"));
            this.cddl_MemberService.SelectedValue = "";
            LoadDropDownList();
        }

        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//20191023 修改：SOC所需資訊  by Peggy
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/23
    /// 修改日期：2009/11/23
    /// <summary>
    /// 更新事件
    /// </summary>
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        #region 2015/1/22 by Eric

        if (this.cddl_MemberService.SelectedValue.ToUpper() == "Y" && this.txtJCIC.Text.Trim().ToUpper() != "A")
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040101_008") + "');");

            return;
        }

        #endregion

        // 檢查設立日期
        string establishDate = this.txtEstablish.Text.Trim();
        int year = int.Parse(establishDate.Substring(0, 3)) + 1911;
        string date = year + "/" + establishDate.Substring(3, 2) + "/" + establishDate.Substring(5, 2);
        DateTime realDate;
        if (!DateTime.TryParse(date, out realDate))
        {
            base.sbRegScript.Append("alert('設立日期格式錯誤');$('#txtEstablish').focus();");
            return;
        }

        //20220112_Ares_Jack_自然人檢核
        if (this.radSingleMerchant6.Checked)
        {
            #region 自然人收單檢核
            List<string> errMsg = new List<string>();
            if (this.txtCountryCode.Text == "")
            {
                errMsg.Add("請輸入負責人國籍\\n");
            }
            else if (this.txtCountryCode.Text.ToUpper() != "TW")
            {
                errMsg.Add("負責人國籍請輸入TW\\n");
            }

            if (this.txtBossID.Text == "")
            {
                errMsg.Add("請輸入負責人ID\\n");
            }
            else if (!CheckNaturePersonID(txtBossID.Text))
            {
                errMsg.Add("負責人ID輸入錯誤\\n");
            }

            if (errMsg.Count > 0)
            {
                string strAlertMsg = "";
                StringBuilder sb = new StringBuilder();
                int linC = 0;
                foreach (string oitem in errMsg)
                {
                    sb.Append(oitem);
                    linC++;
                }
                strAlertMsg = sb.ToString();
                sbRegScript.Append("alert('" + strAlertMsg + "');");
                if (this.txtBossID.Text == "")
                {
                    base.sbRegScript.Append("$('#txtBossID').focus();");
                }
                else if (!CheckNaturePersonID(txtBossID.Text))
                {
                    base.sbRegScript.Append("$('#txtBossID').focus();");
                }
                if (this.txtCountryCode.Text == "")
                {
                    base.sbRegScript.Append("$('#txtCountryCode').focus();");
                }
                else if (this.txtCountryCode.Text.ToUpper() != "TW")
                {
                    base.sbRegScript.Append("$('#txtCountryCode').focus();");
                }
                return;
            }
            #endregion
        }

        //20220214_Ares_Jack_檢核生日
        if (string.IsNullOrEmpty(txtBossBirthday.Text))
        {
            base.sbRegScript.Append("alert('請輸入生日! ');$('#txtBossBirthday').focus();");
            return;
        }
        else
        {
            if (txtBossBirthday.Text.Length != 7)
            {
                base.sbRegScript.Append("alert('生日請輸入七碼!');$('#txtBossBirthday').focus();");
                return;
            }
            if (!checkDateTime(txtBossBirthday.Text))
            {
                base.sbRegScript.Append("alert('生日格式錯誤');$('#txtBossBirthday').focus();");
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

        //20220106_Ares_Jack_國籍不得選無
        if (this.txtCountryCode.Text == "無")
        {
            base.sbRegScript.Append("alert('負責人國籍不得選無');$('#txtCountryCode').focus();");
            return;
        }

        // 20210527 EOS_AML(NOVA) 檢查郵遞區號 by Ares Dennis
        string address = this.txtBookAddr1.Text.Trim();
        if (!string.IsNullOrEmpty(address) && !checkREG_ZIP_CODE(address))
        {
            base.sbRegScript.Append("alert('地址查無郵遞區號，請輸入正確地址或請聯繫MFA更新');");
            return;
        }

        //20190806-RQ-2019-008595-002-長姓名需求 by Peggy↓
        if (chkisLongName.Checked)
        {
            if (txtBoss_L.Text.Trim().Equals(""))
            {
                base.sbRegScript.Append("alert('負責人長姓名FLAG勾選時，請輸入中文長姓名');$('#txtBoss_L').focus();");
                return;
            }

            if (txtBoss_Pinyin.Text.Trim().Equals(""))
            {
                base.sbRegScript.Append("alert('負責人長姓名FLAG勾選時，請輸入羅馬拼音');$('#txtBoss_Pinyin').focus();");
                return;
            }

            if ((ToWide(txtBoss_L.Text.Trim()).Length + LongNameRomaClean(txtBoss_Pinyin.Text).Trim().Length) < 5)
            {
                base.sbRegScript.Append("alert('負責人長姓名FLAG勾選時，負責人姓名(中文+羅馬拼音)需超過4個字以上');$('#txtBoss_L').focus();");
                return;
            }
        }

        if (chkisLongName_c.Checked)
        {
            if (txtContactMan_L.Text.Trim().Equals(""))
            {
                base.sbRegScript.Append("alert('聯絡人長姓名FLAG勾選時，請輸入中文長姓名');$('#txtContactMan_L').focus();");
                return;
            }

            if (txtContactMan_Pinyin.Text.Trim().Equals(""))
            {
                base.sbRegScript.Append("alert('聯絡人長姓名FLAG勾選時，請輸入羅馬拼音');$('#txtContactMan_Pinyin').focus();");
                return;
            }

            if ((ToWide(txtContactMan_L.Text.Trim()).Length + LongNameRomaClean(txtContactMan_Pinyin.Text).Trim().Length) < 5)
            {
                base.sbRegScript.Append("alert('聯絡人長姓名FLAG勾選時，負責人姓名(中文+羅馬拼音)需超過4個字以上');$('#txtContactMan_L').focus();");
                return;
            }

        }
        if (!txtBoss_Pinyin.Text.Trim().Equals(""))
        {
            if (!ValidRoma(txtBoss_Pinyin.Text.Trim()))
            {
                base.sbRegScript.Append("alert('負責人羅馬拼音輸入有誤');$('#txtBoss_Pinyin').focus();");
                return;
            }
        }
        if (!txtContactMan_Pinyin.Text.Trim().Equals(""))
        {
            if (!ValidRoma(txtContactMan_Pinyin.Text.Trim()))
            {
                base.sbRegScript.Append("alert('聯絡人羅馬拼音輸入有誤');$('#txtContactMan_Pinyin').focus();");
                return;
            }
        }
        //20190806-RQ-2019-008595-002-長姓名需求 by Peggy↑
        if (!string.IsNullOrEmpty(txtPassportExpdt.Text) && !checkDateTime(txtPassportExpdt.Text.Trim()))
        {
            base.sbRegScript.Append("alert('護照效期有誤，請輸入正確日期');$('#txtPassportExpdt').focus();");
            return;
        }
        if (!string.IsNullOrEmpty(txtResidentExpdt.Text) && !checkDateTime(txtResidentExpdt.Text.Trim()))
        {
            base.sbRegScript.Append("alert('統一證號效期有誤，請輸入正確日期');$('#txtResidentExpdt').focus();");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
            return;
        }

        //如果負責人國籍非ＴＷ
        if (!string.IsNullOrEmpty(txtCountryCode.Text.Trim()) && txtCountryCode.Text.Trim().ToUpper() != "TW")
        {
            if (string.IsNullOrEmpty(txtPassportNo.Text) && string.IsNullOrEmpty(txtResidentNo.Text))
            {
                base.sbRegScript.Append("alert('負責人國籍非ＴＷ，護照號碼或統一證號擇一填寫');$('#txtPassportNo').focus();");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
                return;
            }
            else
            {
                if (!string.IsNullOrEmpty(txtPassportExpdt.Text))
                {
                    if (string.IsNullOrEmpty(txtPassportNo.Text))
                    {
                        base.sbRegScript.Append("alert('護照效期有值時，護照號碼不可空白');$('#txtPassportNo').focus();");
                        return;
                    }
                    if (txtPassportExpdt.Text.Trim().Length != 8)
                    {
                        base.sbRegScript.Append("alert('護照效期有誤，請輸入8位日期格式');$('#txtPassportExpdt').focus();");
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(txtResidentExpdt.Text))
                {
                    if (string.IsNullOrEmpty(txtResidentNo.Text))
                    {
                        base.sbRegScript.Append("alert(統一證號效期有值時，統一證號證號不可空白');$('#txtResidentNo').focus();");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
                        return;
                    }
                    if (txtResidentExpdt.Text.Trim().Length != 8)
                    {
                        base.sbRegScript.Append("alert('統一證號效期有誤，請輸入8位日期格式');$('#txtResidentExpdt').focus();");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
                        return;
                    }
                }
            }
        }

        //20201021-202012RC 統一證號碼(新+舊)邏輯檢核
        if (!string.IsNullOrEmpty(txtResidentNo.Text))
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

        string strZipData = "";
        if (!chkAddress.Checked)
        {
            strZipData = this.txtBusinessAddr4.Text.Trim();
        }
        else
        {
            strZipData = this.lblBusinessAddrText1.Text.Trim();
        }

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
            if (!chkAddress.Checked)
            {
                this.lblZipText.Text = SZIPSet.GetEntity(0).zip_code;
            }
            else
            {
                this.lblBusinessZipText.Text = SZIPSet.GetEntity(0).zip_code;
            }

            btnUpdateHiden_Click(sender, e);
        }
        else
        {
            this.lblZipText.Text = "";
            this.lblBusinessZipText.Text = "";
            base.sbRegScript.Append("$('#btnUpdateHiden').click();");
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2010/1/20
    /// 修改日期：2010/1/20
    /// <summary>
    /// 新增隱藏事件
    /// </summary>
    protected void btnUpdateHiden_Click(object sender, EventArgs e)
    {
        UploadHtgInfo();
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/23
    /// 修改日期：2009/11/23
    /// <summary>
    /// 強制執行更新事件
    /// </summary>
    protected void btnForce_Click(object sender, EventArgs e)
    {
        Hashtable htInput = GetUploadHtgInfo("Y");
        htInput["MESSAGE_TYPE"] = "5555";
        UploadHtgInfo(htInput);
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/23
    /// 修改日期：2009/11/23
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

        Hashtable htReturn = GetHtgInfo();

        if (!htReturn.Contains("HtgMsg"))
        {
            ClearPage(true);
            SetValues(htReturn);

            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_031");
            base.strHostMsg += htReturn["HtgSuccess"].ToString();//*主機返回成功訊息
            ViewState["HtgInfo"] = htReturn;//20190812 移至下方放VIEWSTATE
            txtUniNo.Enabled = false;//20200219-RQ-2019-030155-003
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
                base.strClientMsg += MessageHelper.GetMessage("01_01040301_004");
            }

            ClearPage(false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo1"));
        }

        //202109023_Ares_jack 統編符合involve範圍時，將預設勾選自然人收單選項
        string involve = "78360020,78360021,78360023,78360024,78360025,78360026,78360027,78360028,78360029";
        if (involve.Contains(this.txtCardNo1.Text.Trim()))
        {
            radSingleMerchant6.Checked = true;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2010/1/20
    /// 修改日期：2010/1/20
    /// <summary>
    /// 同登記名稱選擇事件
    /// </summary>
    protected void chkBusinessName_CheckedChanged(object sender, EventArgs e)
    {
        if (chkBusinessName.Checked)
        {
            lblBusinessNameText.Text = BRCommon.ChangeToSBC(txtRegName.Text.Trim());
            txtBusinessName.Enabled = false;
            base.sbRegScript.Append(BaseHelper.SetFocus("txtBoss"));
        }
        else
        {
            lblBusinessNameText.Text = "";
            txtBusinessName.Enabled = true;
            base.sbRegScript.Append(BaseHelper.SetFocus("txtBusinessName"));
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2010/1/20
    /// 修改日期：2010/1/20
    /// <summary>
    /// 同負責人相關資料擇事件
    /// </summary>
    protected void chkOper_CheckedChanged(object sender, EventArgs e)
    {
        if (chkOper.Checked)
        {
            lblOpermanText.Text = BRCommon.ChangeToSBC(txtBoss.Text.Trim());
            lblOperIDText.Text = txtBossID.Text.Trim();
            lblOperTelText1.Text = txtBossTel1.Text.Trim();
            lblOperTelText2.Text = txtBossTel2.Text.Trim();
            lblOperTelText3.Text = txtBossTel3.Text.Trim();
            lblOperChangeDateText.Text = txtBossChangeDate.Text.Trim();
            lblOperFlagText.Text = txtBossFlag.Text.Trim();
            lblOperBirthdayText.Text = txtBossBirthday.Text.Trim();
            lblOperAtText.Text = BRCommon.ChangeToSBC(txtBossAt.Text.Trim());
            EnabledPartOne(false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtContactMan"));
        }
        else
        {
            ClearOwnerLable();
            EnabledPartOne(true);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtOperman"));
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2010/1/20
    /// 修改日期：2010/1/20
    /// <summary>
    /// 點選查詢郵區按鈕，帶出郵區編號
    /// </summary>

    protected void btnSearchZip_Click(object sender, EventArgs e)
    {
        EntitySet<EntitySZIP> SZIPSet = null;
        try
        {
            SZIPSet = BRSZIP.SelectEntitySet(txtBusinessAddr4.Text.Trim());
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (SZIPSet != null && SZIPSet.Count > 0)
        {

            this.lblZipText.Text = SZIPSet.GetEntity(0).zip_code;
        }
        else
        {
            this.lblZipText.Text = "";
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2010/1/20
    /// 修改日期：2010/1/20
    /// <summary>
    /// 同登記地址料擇事件
    /// </summary>
    protected void chkAddress_CheckedChanged(object sender, EventArgs e)
    {
        if (chkAddress.Checked)
        {
            lblBusinessAddrText1.Text = BRCommon.ChangeToSBC(txtBookAddr1.Text.Trim());
            lblBusinessAddrText2.Text = BRCommon.ChangeToSBC(txtBookAddr2.Text.Trim());
            lblBusinessAddrText3.Text = BRCommon.ChangeToSBC(txtBookAddr3.Text.Trim());

            //查詢管理郵區

            EntitySet<EntitySZIP> SZIPSet = null;
            try
            {
                SZIPSet = BRSZIP.SelectEntitySet(lblBusinessAddrText1.Text);
            }
            catch
            {
                base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                return;
            }

            if (SZIPSet != null && SZIPSet.Count > 0)
            {

                this.lblBusinessZipText.Text = SZIPSet.GetEntity(0).zip_code;

            }
            else
            {
                this.lblBusinessZipText.Text = "";

            }

            EnabledPartTwo(false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtJCIC"));
        }
        else
        {
            ClearAddLable();
            EnabledPartTwo(true);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtBusinessAddr4"));
        }
    }

    //20190806-RQ-2019-008595-002-長姓名需求 by Peggy
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
                CtrlName1 = "txtBoss";//人員名稱
                CtrlName2 = "txtBoss_L";//中文長姓名
                CtrlName3 = "txtBoss_Pinyin";//馬拼音
                break;
            case "chkisLongName_c"://聯絡人
                CtrlName1 = "txtContactMan";//人員名稱
                CtrlName2 = "txtContactMan_L";//中文長姓名
                CtrlName3 = "txtContactMan_Pinyin";//羅馬拼音
                break;
        }

        CustTextBox contNAME = this.FindControl(CtrlName1) as CustTextBox;
        CustTextBox contNameL = this.FindControl(CtrlName2) as CustTextBox;
        CustTextBox contNamePinyin = this.FindControl(CtrlName3) as CustTextBox;
        //2021/03/11_Ares_Stanley-修正粗框問題
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
        CustTextBox txt = (CustTextBox)sender;

        string CtrlName = string.Empty;//長姓名flag
        string CtrlName1 = string.Empty;//原人員姓名

        switch (txt.ID.Trim())
        {
            case "txtBoss_L"://負責人01_01030200_117
                CtrlName = "chkisLongName";//人員名稱
                CtrlName1 = "txtBoss";//中文長姓名
                break;
            case "txtContactMan_L"://聯絡人
                CtrlName = "chkisLongName_c";//人員名稱
                CtrlName1 = "txtContactMan";//中文長姓名
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

    /// <summary>
    /// 使用者輸入的地址解析後系統自動帶入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void TextBox_AddrChanged(object sender, EventArgs e)
    {
        string strZipData = this.txtBookAddr1.Text.Trim();

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
            if (this.txtBookAddr1.Text.Trim() != "")//20220114_Ares_Jack_不等於空值才跳錯誤檢核
            {
                base.strClientMsg += "地址查無郵遞區號，請輸入正確地址或請聯繫MFA更新";
                base.sbRegScript.Append("alert('地址查無郵遞區號，請輸入正確地址或請聯繫MFA更新');");
            }
        }
        base.sbRegScript.Append(BaseHelper.SetFocus("txtBookAddr2"));// 將地址2設為輸入焦點
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
        string organization = string.Empty;
        string countryCode = string.Empty;
        string amlcc = string.Empty;

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
            }
            this.hidOrganization.Value = organization;
        }

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
    /// 清空負責人相關資料部分lable
    /// </summary>
    private void ClearOwnerLable()
    {
        lblOpermanText.Text = "";
        lblOperIDText.Text = "";
        lblOperTelText1.Text = "";
        lblOperTelText2.Text = "";
        lblOperTelText3.Text = "";
        lblOperChangeDateText.Text = "";
        lblOperFlagText.Text = "";
        lblOperBirthdayText.Text = "";
        lblOperAtText.Text = "";
    }

    /// 作者 趙呂梁
    /// 創建日期：2010/1/20
    /// 修改日期：2010/1/20
    /// <summary>
    /// 清空地址資料部分lable
    /// </summary>
    private void ClearAddLable()
    {
        lblBusinessAddrText1.Text = "";
        lblBusinessAddrText2.Text = "";
        lblBusinessAddrText3.Text = "";

        lblBusinessZipText.Text = "";
    }

    /// 作者 趙呂梁
    /// 創建日期：2010/1/20
    /// 修改日期：2010/1/20
    /// <summary>
    /// 清空頁面內容
    /// </summary>
    private void ClearPage(bool blnEnabled)
    {
        CommonFunction.SetControlsEnabled(pnlText, blnEnabled);//*清空網頁中所有的輸入欄位
        chkAddress.Checked = false;
        chkBusinessName.Checked = false;
        chkOper.Checked = false;
        lblBusinessNameText.Text = "";
        ClearOwnerLable();
        ClearAddLable();
        this.txtJCIC.Text = "";
        this.txtGrantFeeFlag.Text = "";
        this.txtMposFlag.Text = "";
        this.txtREG_ZIP_CODE.Enabled = false;

        #region 表單新增欄位
        this.txtUniNo.Text = "";            // 總公司已往來統一編號
        this.txtAMLCC.Text = "";            // AML行業編號
        this.txtCountryCode.Text = "";      // 國籍
        this.txtPassportNo.Text = "";       // 護照號碼
        this.txtPassportExpdt.Text = "";    // 護照效期
        this.txtResidentNo.Text = "";       // 居留證號
        this.txtResidentExpdt.Text = "";    // 居留效期
        // 電子郵件信箱
        this.txtEmailFront.Text = "";
        this.txtEmailOther.Text = "";
        this.hidEmailFall.Value = "";
        #endregion

        //20190805 長姓名需求，新6個欄位 by Peggy↓
        chkisLongName.Checked = false;
        txtBoss_L.Text = "";
        txtBoss_Pinyin.Text = "";
        chkisLongName_c.Checked = false;
        txtContactMan_L.Text = "";
        txtContactMan_Pinyin.Text = "";
        //20190805 長姓名需求，新6個欄位 by Peggy↑

        //20200306-RQ-2019-030155-003 清空時，也要將radioButton值清空
        this.radSingleMerchant1.Checked = false;
        this.radSingleMerchant2.Checked = false;
        this.radSingleMerchant3.Checked = false;
        this.radSingleMerchant4.Checked = false;
        this.radSingleMerchant5.Checked = false;
        //20210906 jack 自然人收單，新增分期平台選項
        this.radSingleMerchant6.Checked = false;

    }

    /// 作者 趙呂梁
    /// 創建日期：2010/1/20
    /// 修改日期：2010/1/20
    /// <summary>
    /// 設置負責人相關資料部分欄位是否可用
    /// </summary>
    /// <param name="isEnabled">true可用,false不可用</param>
    private void EnabledPartOne(bool isEnabled)
    {
        txtOperID.Enabled = isEnabled;
        txtOperman.Enabled = isEnabled;
        txtOperTel1.Enabled = isEnabled;
        txtOperTel2.Enabled = isEnabled;
        txtOperTel3.Enabled = isEnabled;
        txtOperChangeDate.Enabled = isEnabled;
        txtOperBirthday.Enabled = isEnabled;
        txtOperFlag.Enabled = isEnabled;
        txtOperAt.Enabled = isEnabled;
    }

    /// 作者 趙呂梁
    /// 創建日期：2010/1/20
    /// 修改日期：2010/1/20
    /// <summary>
    /// 設置地址資料部分欄位是否可用
    /// </summary>
    /// <param name="isEnabled">true可用,false不可用</param>
    private void EnabledPartTwo(bool isEnabled)
    {
        txtBusinessAddr4.Enabled = isEnabled;
        txtBusinessAddr5.Enabled = isEnabled;
        txtBusinessAddr6.Enabled = isEnabled;
    }

    /// 作者 趙呂梁

    /// 創建日期：2009/11/24
    /// 修改日期：2009/11/24
    /// <summary>
    /// 依據EXMS 6001 P4A查詢主機資料
    /// </summary>
    /// <returns>主機資料的HashTable</returns>
    private Hashtable GetHtgInfo()
    {
        Hashtable htInput = new Hashtable();
        htInput.Add("CORP_NO", this.txtCardNo1.Text.Trim());//*統一編號1
        htInput.Add("CORP_SEQ", this.txtCardNo2.Text.Trim());//*統一編號2
        htInput.Add("FUNCTION_CODE", "I");//*功能別 查詢
        Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGQ, htInput, false, "11", eAgentInfo);
        htReturn["CORP_NO"] = htInput["CORP_NO"];//* for_xml_test 
        htReturn["CORP_SEQ"] = htInput["CORP_SEQ"];
        htReturn["FUNCTION_CODE"] = "I";
        return htReturn;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/24
    /// 修改日期：2009/11/24
    /// <summary>
    /// 為網頁中的欄位賦值
    /// </summary>
    /// <param name="htReturn">返回的主機信息</param>
    private void SetValues(Hashtable htReturn)
    {
        this.txtReceiveNumber.Text = htReturn["APPL_NO"].ToString();     // 收件編號
        this.txtBank.Text = htReturn["DDA_BANK_NAME"].ToString();        // 銀行
        this.txtBookAddr1.Text = htReturn["REG_CITY"].ToString();        // 登記地址1
        this.txtBookAddr2.Text = htReturn["REG_ADDR1"].ToString();       // 登記地址2
        this.txtBookAddr3.Text = htReturn["REG_ADDR2"].ToString();       // 登記地址3
        this.txtBoss.Text = htReturn["OWNER_NAME"].ToString();           // 負責人姓名
        this.txtBossID.Text = htReturn["OWNER_ID"].ToString();           // 負責人ID
        this.txtBossTel1.Text = htReturn["OWNER_PHONE_AREA"].ToString(); // 負責人電話1
        this.txtBossTel2.Text = htReturn["OWNER_PHONE_NO"].ToString();   // 負責人電話2
        this.txtBossTel3.Text = htReturn["OWNER_PHONE_EXT"].ToString();  // 負責人電話3
        this.txtBranchBank.Text = htReturn["DDA_BANK_BRANCH"].ToString();// 分行
        this.txtBusinessAddr4.Text = htReturn["REAL_CITY"].ToString();   // 營業地址2
        this.txtBusinessAddr5.Text = htReturn["REAL_ADDR1"].ToString();  // 營業地址3
        this.txtBusinessAddr6.Text = htReturn["REAL_ADDR2"].ToString();  // 營業地址4
        this.txtBusinessName.Text = htReturn["BUSINESS_NAME"].ToString();// 營業名稱
        this.txtCapital.Text = htReturn["CAPITAL"].ToString();           // 資本
        this.txtCheckMan.Text = htReturn["CREDIT_NO"].ToString();        // 徵信員

        this.txtContactMan.Text = htReturn["CONTACT_NAME"].ToString();          // 聯絡人姓名
        this.txtContactManTel1.Text = htReturn["CONTACT_PHONE_AREA"].ToString();// 聯絡人電話1
        this.txtContactManTel2.Text = htReturn["CONTACT_PHONE_NO"].ToString();  // 聯絡人電話2
        this.txtContactManTel3.Text = htReturn["CONTACT_PHONE_EXT"].ToString(); // 聯絡人電話3
        //this.txtEstablish.Text = htReturn["BUILD_DATE"].ToString();             // 設立
        this.txtFax1.Text = htReturn["FAX_AREA"].ToString();                    // 聯絡人傳真1
        this.txtFax2.Text = htReturn["FAX_PHONE_NO"].ToString();                // 聯絡人傳真2
        this.txtInvoiceCycle.Text = htReturn["INVOICE_CYCLE"].ToString();       // 發票週期

        this.txtName.Text = htReturn["DDA_ACCT_NAME"].ToString();         // 戶名
        this.txtOperID.Text = htReturn["MANAGER_ID"].ToString();          // 實際經營者ID
        this.txtOperTel1.Text = htReturn["MANAGER_PHONE_AREA"].ToString();// 實際經營者電話1
        this.txtOperTel2.Text = htReturn["MANAGER_PHONE_NO"].ToString();  // 實際經營者電話2
        this.txtOperTel3.Text = htReturn["MANAGER_PHONE_EXT"].ToString(); // 實際經營者電話3
        this.txtOperman.Text = htReturn["MANAGER_NAME"].ToString();       // 實際經營者姓名
        //this.txtOrganization.Text = htReturn["ORGAN_TYPE"].ToString();    // 組織
        this.txtPopMan.Text = htReturn["SALE_NAME"].ToString();           // 推廣員

        this.txtRegAddr1.Text = htReturn["OWNER_CITY"].ToString(); // 戶籍地址1
        this.txtRegAddr2.Text = htReturn["OWNER_ADDR1"].ToString();// 戶籍地址2
        this.txtRegAddr3.Text = htReturn["OWNER_ADDR2"].ToString();// 戶籍地址3
        this.txtRegName.Text = htReturn["REG_NAME"].ToString();    // 登記名稱
        this.txtRisk.Text = htReturn["RISK_FLAG"].ToString();      // 風險

        this.txtBossChangeDate.Text = htReturn["CHANGE_DATE1"].ToString();// 負責人領換補日
        this.txtBossBirthday.Text = htReturn["BIRTHDAY1"].ToString();     // 負責人生日
        this.txtBossFlag.Text = htReturn["CHANGE_FLAG1"].ToString();      // 負責人代號
        this.txtBossAt.Text = htReturn["AT1"].ToString();                 // 負責人換證點

        this.txtOperChangeDate.Text = htReturn["CHANGE_DATE2"].ToString();// 實際經營者領換補日
        this.txtOperBirthday.Text = htReturn["BIRTHDAY2"].ToString();     // 實際經營者生日
        this.txtOperFlag.Text = htReturn["CHANGE_FLAG2"].ToString();      // 實際經營者代號
        this.txtOperAt.Text = htReturn["AT2"].ToString();                 // 實際經營者換證點

        this.txtJCIC.Text = htReturn["JCIC_CODE"].ToString();             // JCIC查詢
        this.txtPrevDesc.Text = htReturn["IPMR_PREV_DESC"].ToString();    // 帳單內容
        this.txtRedeemCycle.Text = htReturn["REDEEM_CYCLE"].ToString();   // 紅利週期(M/D)
        this.lblZipText.Text = htReturn["REAL_ZIP"].ToString();           // 營業地郵遞區號
        this.txtMposFlag.Text = htReturn["MPOS_FLAG"].ToString();         // Y_MPOS特店系統服務費免收註記
        this.txtGrantFeeFlag.Text = htReturn["GRANT_FEE_FLAG"].ToString();// Y_特店跨行匯費

        #region 表單新增欄位
        this.txtEstablish.Text = htReturn["BUILD_DATE"].ToString() + htReturn["BUILD_DATE_DD"].ToString();// 設立日
        this.txtOrganization.Text = htReturn["ORGAN_TYPE_NEW"].ToString();                             // 組織

        // 商店別標識(1.總公司已往來 2.總公司未往來 3.獨立店)
        if (htReturn["SINGLE_MERCHANT"].ToString() == "1")
            this.radSingleMerchant1.Checked = true;// 總公司已往來
        else if (htReturn["SINGLE_MERCHANT"].ToString() == "2")
            this.radSingleMerchant2.Checked = true;// 總公司未往來
        //20200219-RQ-2019-030155-003-ENABLED獨立店/海外公司選項，新增分期平台選項
        else if (htReturn["SINGLE_MERCHANT"].ToString() == "3")
            this.radSingleMerchant3.Checked = true;// 獨立店
        else if (htReturn["SINGLE_MERCHANT"].ToString() == "4")
            this.radSingleMerchant4.Checked = true;// 海外公司
        else if (htReturn["SINGLE_MERCHANT"].ToString() == "5")
            this.radSingleMerchant5.Checked = true;// 分期平台
        //20210906 jack 自然人收單，新增分期平台選項
        else if (htReturn["SINGLE_MERCHANT"].ToString() == "6")
            this.radSingleMerchant6.Checked = true;// 自然人收單

        this.txtUniNo.Text = htReturn["HEADQUARTER_CORPNO"].ToString();     // 統一編號
        this.txtAMLCC.Text = htReturn["AML_CC"].ToString();                 // AML行業編號
        this.txtCountryCode.Text = htReturn["COUNTRY_CODE"].ToString();     // 國籍
        this.txtPassportNo.Text = htReturn["PASSPORT_NO"].ToString();       // 護照號碼
        this.txtPassportExpdt.Text = htReturn["PASSPORT_EXPDT"].ToString(); // 護照效期
        this.txtResidentNo.Text = htReturn["RESIDENT_NO"].ToString();       // 居留證號
        this.txtResidentExpdt.Text = htReturn["RESIDENT_EXPDT"].ToString(); // 護照效期
        SetEmailValue(htReturn["EMAIL"].ToString().Trim());                 // 電子郵件信箱
        #endregion

        //20190806-RQ-2019-008595-002-長姓名需求 by Peggy↓        
        if (htReturn.ContainsKey("OWNER_LNAM_FLAG") && htReturn["OWNER_LNAM_FLAG"].ToString().Trim().Equals("Y"))//負責人長姓名flag
        {
            this.chkisLongName.Checked = true;

            EntityHTG_JC68 htReturn_JC68 = GetJC68(txtBossID.Text.Trim());//負責人id
            txtBoss_L.Text = htReturn_JC68.LONG_NAME;//負責人-中文長姓名
            txtBoss_Pinyin.Text = htReturn_JC68.PINYIN_NAME;//負責人-羅馬拼音
        }
        else
        {
            this.chkisLongName.Checked = false;
        }

        if (htReturn.ContainsKey("CONTACT_LNAM_FLAG") && htReturn["CONTACT_LNAM_FLAG"].ToString().Trim().Equals("Y"))//聯絡人長姓名flag
        {
            this.chkisLongName_c.Checked = true;

            EntityHTG_JC68 htReturn_JC68 = GetJC68(txtCardNo1.Text.Trim() + txtCardNo2.Text.Trim());//聯絡人key值
            txtContactMan_L.Text = htReturn_JC68.LONG_NAME;//負責人-中文長姓名
            txtContactMan_Pinyin.Text = htReturn_JC68.PINYIN_NAME;//負責人-羅馬拼音
        }
        else
        {
            this.chkisLongName_c.Checked = false;
        }

        CheckBox_CheckedChanged(chkisLongName, null);
        CheckBox_CheckedChanged(chkisLongName_c, null);
        //20190806-RQ-2019-008595-002-長姓名需求 by Peggy↑

        // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis
        if (htReturn.ContainsKey("REG_ZIP_CODE"))
        {
            this.txtREG_ZIP_CODE.Text = htReturn["REG_ZIP_CODE"].ToString();// 登記地址郵遞區號
        }
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
    /// 創建日期：2009/11/04
    /// 修改日期：2009/11/04
    /// <summary>
    /// 得到上傳主機信息
    /// </summary>
    /// <returns>Hashtable</returns>
    /// <param name="strSend3270">強制執行</param>
    private Hashtable GetUploadHtgInfo(string strSendFlag)
    {
        string strMgr_ext = this.txtOperTel3.Text.Trim().ToUpper();
        string strContact_ext = this.txtContactManTel3.Text.Trim().ToUpper();
        string strOwner_ext = this.txtBossTel3.Text.Trim().ToUpper();
        string mLongName = "N";
        string mContactLName = "N";

        if (ViewState["HtgInfo"] != null)
        {
            Hashtable htReturn = new Hashtable();

            CommonFunction.GetViewStateHt(ViewState["HtgInfo"], ref htReturn);

            htReturn["MESSAGE_TYPE"] = "";
            htReturn["MESSAGE_CHI"] = "";
            //Add By CTCB-Carolyn 2010.05.28
            // 判斷若分機為'X',則用空白取代
            if (strMgr_ext == "X")
            {
                strMgr_ext = "";
            }
            if (strContact_ext == "X")
            {
                strContact_ext = "";
            }
            if (strOwner_ext == "X")
            {
                strOwner_ext = "";
            }

            #region 比對主機資料
            //*收件編號
            CommonFunction.ContrastData(htReturn, this.txtReceiveNumber.Text.Trim(), "APPL_NO");
            //*設立
            CommonFunction.ContrastData(htReturn, this.txtEstablish.Text.Trim().Substring(0, 5), "BUILD_DATE");
            //*徵信員
            CommonFunction.ContrastData(htReturn, this.txtCheckMan.Text.Trim(), "CREDIT_NO");
            //*資本
            CommonFunction.ContrastData(htReturn, this.txtCapital.Text.Trim(), "CAPITAL");
            //*登記名稱
            CommonFunction.ContrastData(htReturn, this.txtRegName.Text.Trim(), "REG_NAME");
            //*組織
            //CommonFunction.ContrastData(htReturn, this.txtOrganization.Text.Trim(), "ORGAN_TYPE");

            //*營業名稱
            if (!chkBusinessName.Checked)
            {
                CommonFunction.ContrastData(htReturn, this.txtBusinessName.Text.Trim(), "BUSINESS_NAME");
            }
            else
            {
                // 20211230 同登記名稱勾選 代入登記名稱 by Ares Dennis
                CommonFunction.ContrastData(htReturn, this.txtRegName.Text.Trim(), "BUSINESS_NAME");
            }

            //*風險
            CommonFunction.ContrastData(htReturn, this.txtRisk.Text.Trim(), "RISK_FLAG");
            //*負責人
            CommonFunction.ContrastData(htReturn, this.txtBoss.Text.Trim(), "OWNER_NAME");
            //*負責人ID
            CommonFunction.ContrastData(htReturn, this.txtBossID.Text.Trim().ToUpper(), "OWNER_ID");
            //*負責人電話1
            CommonFunction.ContrastData(htReturn, this.txtBossTel1.Text.Trim(), "OWNER_PHONE_AREA");
            //*負責人電話2
            CommonFunction.ContrastData(htReturn, this.txtBossTel2.Text.Trim(), "OWNER_PHONE_NO");
            //*負責人電話3
            CommonFunction.ContrastData(htReturn, strOwner_ext, "OWNER_PHONE_EXT");
            //*負責人戶籍地址1
            CommonFunction.ContrastData(htReturn, this.txtRegAddr1.Text.Trim(), "OWNER_CITY");
            //*負責人戶籍地址2
            CommonFunction.ContrastData(htReturn, this.txtRegAddr2.Text.Trim(), "OWNER_ADDR1");
            //*負責人戶籍地址3
            CommonFunction.ContrastData(htReturn, this.txtRegAddr3.Text.Trim(), "OWNER_ADDR2");
            //*負責人領換補日
            CommonFunction.ContrastData(htReturn, this.txtBossChangeDate.Text.Trim(), "CHANGE_DATE1");
            //*負責人生日
            CommonFunction.ContrastData(htReturn, this.txtBossBirthday.Text.Trim(), "BIRTHDAY1");
            //*負責人代號
            CommonFunction.ContrastData(htReturn, this.txtBossFlag.Text.Trim(), "CHANGE_FLAG1");
            //*負責人換證點
            CommonFunction.ContrastData(htReturn, this.txtBossAt.Text.Trim(), "AT1");

            //*實際經營者相關資料
            if (!chkOper.Checked)
            {
                //*實際經營者
                CommonFunction.ContrastData(htReturn, this.txtOperman.Text.Trim(), "MANAGER_NAME");
                //*實際經營者ID
                CommonFunction.ContrastData(htReturn, this.txtOperID.Text.Trim().ToUpper(), "MANAGER_ID");
                //*實際經營者電話1
                CommonFunction.ContrastData(htReturn, this.txtOperTel1.Text.Trim(), "MANAGER_PHONE_AREA");
                //*實際經營者電話2
                CommonFunction.ContrastData(htReturn, this.txtOperTel2.Text.Trim(), "MANAGER_PHONE_NO");
                //*實際經營者電話3
                CommonFunction.ContrastData(htReturn, strMgr_ext, "MANAGER_PHONE_EXT");
                //*實際經營者領換補日
                CommonFunction.ContrastData(htReturn, this.txtOperChangeDate.Text.Trim(), "CHANGE_DATE2");
                //*實際經營者生日
                CommonFunction.ContrastData(htReturn, this.txtOperBirthday.Text.Trim(), "BIRTHDAY2");
                //*實際經營者代號
                CommonFunction.ContrastData(htReturn, this.txtOperFlag.Text.Trim(), "CHANGE_FLAG2");
                //*實際經營者換證點
                CommonFunction.ContrastData(htReturn, this.txtOperAt.Text.Trim(), "AT2");
            }
            else
            {
                //*實際經營者
                CommonFunction.ContrastData(htReturn, this.lblOpermanText.Text.Trim(), "MANAGER_NAME");
                //*實際經營者ID
                CommonFunction.ContrastData(htReturn, this.lblOperIDText.Text.Trim().ToUpper(), "MANAGER_ID");
                //*實際經營者電話1
                CommonFunction.ContrastData(htReturn, this.lblOperTelText1.Text.Trim(), "MANAGER_PHONE_AREA");
                //*實際經營者電話2
                CommonFunction.ContrastData(htReturn, this.lblOperTelText2.Text.Trim(), "MANAGER_PHONE_NO");
                //*實際經營者電話3
                CommonFunction.ContrastData(htReturn, this.lblOperTelText3.Text.Trim(), "MANAGER_PHONE_EXT");
                //*實際經營者領換補日
                CommonFunction.ContrastData(htReturn, this.lblOperChangeDateText.Text.Trim(), "CHANGE_DATE2");
                //*實際經營者生日
                CommonFunction.ContrastData(htReturn, this.lblOperBirthdayText.Text.Trim(), "BIRTHDAY2");
                //*實際經營者代號
                CommonFunction.ContrastData(htReturn, this.lblOperFlagText.Text.Trim(), "CHANGE_FLAG2");
                //*實際經營者換證點
                CommonFunction.ContrastData(htReturn, this.lblOperAtText.Text.Trim(), "AT2");
            }

            //*聯絡人
            CommonFunction.ContrastData(htReturn, this.txtContactMan.Text.Trim(), "CONTACT_NAME");
            //*聯絡人電話1
            CommonFunction.ContrastData(htReturn, this.txtContactManTel1.Text.Trim(), "CONTACT_PHONE_AREA");
            //*聯絡人電話2
            CommonFunction.ContrastData(htReturn, this.txtContactManTel2.Text.Trim(), "CONTACT_PHONE_NO");
            //*聯絡人電話3
            CommonFunction.ContrastData(htReturn, strContact_ext, "CONTACT_PHONE_EXT");
            //*聯絡人傳真1
            CommonFunction.ContrastData(htReturn, this.txtFax1.Text.Trim(), "FAX_AREA");
            //*聯絡人傳真2
            CommonFunction.ContrastData(htReturn, this.txtFax2.Text.Trim(), "FAX_PHONE_NO");
            //*登記地址1
            CommonFunction.ContrastData(htReturn, this.txtBookAddr1.Text.Trim(), "REG_CITY");
            //*登記地址2
            CommonFunction.ContrastData(htReturn, this.txtBookAddr2.Text.Trim(), "REG_ADDR1");
            //*登記地址3
            CommonFunction.ContrastData(htReturn, this.txtBookAddr3.Text.Trim(), "REG_ADDR2");

            //*營業地址
            if (!chkAddress.Checked)
            {
                //*營業地郵遞區號
                CommonFunction.ContrastData(htReturn, this.lblZipText.Text.Trim(), "REAL_ZIP");
                //*營業地址1
                CommonFunction.ContrastData(htReturn, this.txtBusinessAddr4.Text.Trim(), "REAL_CITY");
                //*營業地址2
                CommonFunction.ContrastData(htReturn, this.txtBusinessAddr5.Text.Trim(), "REAL_ADDR1");
                //*營業地址3
                CommonFunction.ContrastData(htReturn, this.txtBusinessAddr6.Text.Trim(), "REAL_ADDR2");
            }
            else
            {
                // 20211230 同登記地址勾選 代入登記地址 by Ares Dennis
                //*營業地郵遞區號
                CommonFunction.ContrastData(htReturn, this.txtREG_ZIP_CODE.Text.Trim(), "REAL_ZIP");
                //*營業地址1
                CommonFunction.ContrastData(htReturn, this.txtBookAddr1.Text.Trim(), "REAL_CITY");
                //*營業地址2
                CommonFunction.ContrastData(htReturn, this.txtBookAddr2.Text.Trim(), "REAL_ADDR1");
                //*營業地址3
                CommonFunction.ContrastData(htReturn, this.txtBookAddr3.Text.Trim(), "REAL_ADDR2");
            }

            //*銀行
            CommonFunction.ContrastData(htReturn, this.txtBank.Text.Trim(), "DDA_BANK_NAME");
            //*分行別
            CommonFunction.ContrastData(htReturn, this.txtBranchBank.Text.Trim(), "DDA_BANK_BRANCH");
            //*戶名
            CommonFunction.ContrastData(htReturn, this.txtName.Text.Trim(), "DDA_ACCT_NAME");
            //*推廣員
            CommonFunction.ContrastData(htReturn, this.txtPopMan.Text.Trim(), "SALE_NAME");
            //*發票週期
            CommonFunction.ContrastData(htReturn, this.txtInvoiceCycle.Text.Trim(), "INVOICE_CYCLE");
            //*JCIC查詢
            CommonFunction.ContrastData(htReturn, this.txtJCIC.Text.Trim().ToUpper(), "JCIC_CODE");
            //*帳單內容
            CommonFunction.ContrastData(htReturn, this.txtPrevDesc.Text.Trim(), "IPMR_PREV_DESC");
            //*紅利週期(M/D)
            CommonFunction.ContrastData(htReturn, this.txtRedeemCycle.Text.Trim().ToUpper(), "REDEEM_CYCLE");
            // Y_MPOS特店系統服務費免收註記(6086)F001
            CommonFunction.ContrastData(htReturn, this.txtMposFlag.Text.Trim().ToUpper(), "MPOS_FLAG");
            // Y_特店跨行匯費(6116)
            CommonFunction.ContrastData(htReturn, this.txtGrantFeeFlag.Text.Trim().ToUpper(), "GRANT_FEE_FLAG");
            #endregion

            #region 電文新增欄位
            // 設立
            CommonFunction.ContrastData(htReturn, this.txtEstablish.Text.Trim().Substring(5, 2), "BUILD_DATE_DD");
            // 組織
            CommonFunction.ContrastData(htReturn, this.txtOrganization.Text.Trim(), "ORGAN_TYPE_NEW");

            string singleMerchant = string.Empty;
            string uniNo = string.Empty;

            //20200306-RQ-2019-030155-003：不能修改總公司統編及公司類型
            /*
            // 商店別標識(1.總公司已往來 2.總公司未往來 3.獨立店)
            if (this.radSingleMerchant1.Checked)
                singleMerchant = "1";// 總公司已往來
            else if (this.radSingleMerchant2.Checked)
                singleMerchant = "2";// 總公司未往來
            //20200219-RQ-2019-030155-003ENABLED獨立店/海外公司選項，新增分期平台選項
            else if (this.radSingleMerchant3.Checked)
                singleMerchant = "3";// 獨立店
            else if (this.radSingleMerchant4.Checked)
                singleMerchant = "4";// 海外公司
            else if (this.radSingleMerchant5.Checked)
                singleMerchant = "5";// 分期平台

            CommonFunction.ContrastData(htReturn, singleMerchant, "SINGLE_MERCHANT");
            // 統一編號
            CommonFunction.ContrastData(htReturn, this.txtUniNo.Text.Trim(), "HEADQUARTER_CORPNO");
            */
            // AML行業編號
            CommonFunction.ContrastData(htReturn, this.txtAMLCC.Text.Trim(), "AML_CC");
            // 國籍
            CommonFunction.ContrastData(htReturn, this.txtCountryCode.Text.ToUpper().Trim(), "COUNTRY_CODE");
            // 護照號碼
            CommonFunction.ContrastData(htReturn, this.txtPassportNo.Text.Trim(), "PASSPORT_NO");
            // 護照效期
            CommonFunction.ContrastData(htReturn, this.txtPassportExpdt.Text.Trim(), "PASSPORT_EXPDT");
            // 居留證號
            CommonFunction.ContrastData(htReturn, this.txtResidentNo.Text.Trim(), "RESIDENT_NO");
            // 居留效期
            CommonFunction.ContrastData(htReturn, this.txtResidentExpdt.Text.Trim(), "RESIDENT_EXPDT");
            // 電子郵件信箱
            CommonFunction.ContrastData(htReturn, this.hidEmailFall.Value.Trim(), "EMAIL");
            #endregion

            //20190806-RQ-2019-008595-002-長姓名需求 by Peggy↓
            //*負責人-是否長姓名flag
            if (!string.IsNullOrEmpty(htReturn["OWNER_LNAM_FLAG"].ToString().Trim()))
            {
                mLongName = htReturn["OWNER_LNAM_FLAG"].ToString().Trim();
            }
            else
            {
                htReturn["OWNER_LNAM_FLAG"] = "N";
            }

            CommonFunction.ContrastData(htReturn, this.chkisLongName.Checked ? "Y" : "N", "OWNER_LNAM_FLAG");

            //*聯絡人-是否長姓名flag
            if (!string.IsNullOrEmpty(htReturn["CONTACT_LNAM_FLAG"].ToString().Trim()))
            {
                mContactLName = htReturn["CONTACT_LNAM_FLAG"].ToString().Trim();
            }
            else
            {
                htReturn["CONTACT_LNAM_FLAG"] = "N";
            }
            CommonFunction.ContrastData(htReturn, this.chkisLongName_c.Checked ? "Y" : "N", "CONTACT_LNAM_FLAG");


            //比對主機資料
            Hashtable htQJC68 = new Hashtable();

            if (chkisLongName.Checked || mLongName.Trim().Equals("Y"))
            {
                htQJC68 = new Hashtable();
                htQJC68 = QueryJC68(txtBossID.Text.Trim());

                //if (!htQJC68.Contains("HtgMsg"))//20190907 make by Peggy
                //{
                //*負責人-中文長姓名
                CommonFunction.ContrastData(htQJC68, this.txtBoss_L.Text.Trim(), "LONG_NAME");
                //*負責人-羅馬拼音
                CommonFunction.ContrastData(htQJC68, LongNameRomaClean(this.txtBoss_Pinyin.Text.Trim()), "PINYIN_NAME");

                //20190806-RQ-2019-008595-002-長姓名需求，如有長姓名，更新JC68資料 by Peggy
                _tmpJC68 = new EntityHTG_JC68();
                if (chkisLongName.Checked)
                {
                    _tmpJC68.ID = txtBossID.Text.Trim();
                    _tmpJC68.LONG_NAME = ToWide(txtBoss_L.Text.Trim());
                    _tmpJC68.PINYIN_NAME = LongNameRomaClean(this.txtBoss_Pinyin.Text.Trim());

                    _JC68s.Add(_tmpJC68);
                }
                else
                {
                    _tmpJC68.ID = txtBossID.Text.Trim();
                    _tmpJC68.LONG_NAME = "";
                    _tmpJC68.PINYIN_NAME = "";

                    _JC68s.Add(_tmpJC68);
                }
                //}
            }

            if (chkisLongName_c.Checked || mContactLName.Trim().Equals("Y"))
            {
                htQJC68 = new Hashtable();
                htQJC68 = QueryJC68(txtCardNo1.Text.Trim() + txtCardNo2.Text.Trim());
                //if (!htQJC68.Contains("HtgMsg"))
                //{
                //*聯絡人-中文長姓名
                CommonFunction.ContrastData(htQJC68, this.txtContactMan_L.Text.Trim(), "LONG_NAME");
                //*聯絡人-羅馬拼音
                CommonFunction.ContrastData(htQJC68, LongNameRomaClean(this.txtContactMan_Pinyin.Text.Trim()), "PINYIN_NAME");

                _tmpJC68 = new EntityHTG_JC68();
                if (chkisLongName_c.Checked)
                {
                    _tmpJC68.ID = txtCardNo1.Text.Trim() + txtCardNo2.Text.Trim();
                    _tmpJC68.LONG_NAME = ToWide(txtContactMan_L.Text.Trim());
                    _tmpJC68.PINYIN_NAME = LongNameRomaClean(this.txtContactMan_Pinyin.Text.Trim());

                    _JC68s.Add(_tmpJC68);
                }
                else
                {
                    _tmpJC68.ID = txtCardNo1.Text.Trim() + txtCardNo2.Text.Trim();
                    _tmpJC68.LONG_NAME = "";
                    _tmpJC68.PINYIN_NAME = "";

                    _JC68s.Add(_tmpJC68);
                }
                //}
            }
            //20190806-RQ-2019-008595-002-長姓名需求 by Peggy↑

            // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis
            // 登記地址郵遞區號
            CommonFunction.ContrastData(htReturn, this.txtREG_ZIP_CODE.Text.Trim(), "REG_ZIP_CODE");
            // 資料最後異動分行
            CommonFunction.ContrastData(htReturn, this.txtLAST_UPD_BRANCH.Text.Trim(), "LAST_UPD_BRANCH");
            // 資料最後異動-CHECKER
            CommonFunction.ContrastData(htReturn, this.txtLAST_UPD_CHECKER.Text.Trim(), "LAST_UPD_CHECKER");
            // 資料最後異動-MAKER
            CommonFunction.ContrastData(htReturn, this.txtLAST_UPD_MAKER.Text.Trim(), "LAST_UPD_MAKER");

            //*強制執行
            if (!htReturn.Contains("FORCE_FLAG"))
            {
                htReturn.Add("FORCE_FLAG", strSendFlag);
            }
            else
            {
                htReturn["FORCE_FLAG"] = strSendFlag;
            }

            htReturn["FUNCTION_CODE"] = "C";//*功能別 更新
            return htReturn;
        }
        return null;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/24
    /// 修改日期：2009/11/24
    /// <summary>
    /// 異動主機資料
    /// </summary>
    private void UploadHtgInfo()
    {
        if (!UploadHtgInfo(GetUploadHtgInfo("")))
        {
            //base.sbRegScript.Append("if(confirm('" + MessageHelper.GetMessage("01_01040301_003") + "')) {$('#btnForce').click();}");
            //base.sbRegScript.Append("if(confirm('" + base.strHostMsg + ''\n'' + MessageHelper.GetMessage("01_01040301_003") + "')) {$('#btnForce').click();}");
            base.sbRegScript.Append("if(confirm('" + base.strHostMsg + "\\n\\n" + MessageHelper.GetMessage("01_01040301_003") + "')) {$('#btnForce').click();}");
        }
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

        // 20210527 EOS_AML(NOVA) by Ares Dennis start
        Hashtable htReturn = GetHtgInfo();
        // 20210527 EOS_AML(NOVA) by Ares Dennis end

        Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGQ, htInput, false, "21", eAgentInfo);

        //*異動主機資料成功
        if (!htResult.Contains("HtgMsg"))
        {
            //20190806-RQ-2019-008595-002-長姓名需求，如有長姓名，更新JC68資料 by Peggy
            using (BRHTG_JC68 obj = new BRHTG_JC68("P010104060001"))
            {
                EntityResult _EntityResult = new EntityResult();
                foreach (EntityHTG_JC68 item in _JC68s)//錯誤訊息
                {
                    _EntityResult = obj.Update(item, this.eAgentInfo, "21");
                    if (_EntityResult.Success == false)
                    {
                        base.strHostMsg += "更新長姓名資料:" + _EntityResult.HostMsg;
                        base.strClientMsg += "更新長姓名資料:" + _EntityResult.HostMsg;
                    }
                }
            }
            //20190806-RQ-2019-008595-002-長姓名需求，如有長姓名，更新JC68資料 by Peggy

            // 20210527 EOS_AML(NOVA) by Ares Dennis
            #region 異動記錄需報送AML                
            if (!htReturn.Contains("HtgMsg"))
            {
                bool isChanged = false;                

                #region 檢核欄位是否異動
                compareForAMLCheckLog(htReturn, htInput, "HEADQUARTER_CORPNO", ref isChanged);// 總公司統一編號
                compareForAMLCheckLog(htReturn, htInput, "AML_CC", ref isChanged);// AML行業編號
                compareForAMLCheckLog(htReturn, htInput, "ORGAN_TYPE_NEW", ref isChanged);// 法律形式
                compareForAMLCheckLog(htReturn, htInput, "BUILD_DATE", ref isChanged);// 設立
                compareForAMLCheckLog(htReturn, htInput, "BUILD_DATE_DD", ref isChanged);// 設立
                compareForAMLCheckLog(htReturn, htInput, "REG_NAME", ref isChanged);// 登記名稱
                compareForAMLCheckLog(htReturn, htInput, "OWNER_NAME", ref isChanged);// 負責人姓名
                compareForAMLCheckLog(htReturn, htInput, "OWNER_ID", ref isChanged);// 負責人ID
                compareForAMLCheckLog(htReturn, htInput, "OWNER_PHONE_AREA", ref isChanged);// 負責人電話
                compareForAMLCheckLog(htReturn, htInput, "OWNER_PHONE_NO", ref isChanged);// 負責人電話
                compareForAMLCheckLog(htReturn, htInput, "OWNER_PHONE_EXT", ref isChanged);// 負責人電話
                compareForAMLCheckLog(htReturn, htInput, "OWNER_LNAM_FLAG", ref isChanged);// 長姓名
                compareForAMLCheckLog(htReturn, htInput, "COUNTRY_CODE", ref isChanged);// 國籍
                compareForAMLCheckLog(htReturn, htInput, "PASSPORT_NO", ref isChanged);// 護照號碼
                compareForAMLCheckLog(htReturn, htInput, "PASSPORT_EXPDT", ref isChanged);// 護照效期
                compareForAMLCheckLog(htReturn, htInput, "RESIDENT_NO", ref isChanged);// 統一證號
                compareForAMLCheckLog(htReturn, htInput, "RESIDENT_EXPDT", ref isChanged);// 統一證號效期
                compareForAMLCheckLog(htReturn, htInput, "OWNER_CITY", ref isChanged);// 戶籍地址
                compareForAMLCheckLog(htReturn, htInput, "OWNER_ADDR1", ref isChanged);// 戶籍地址
                compareForAMLCheckLog(htReturn, htInput, "OWNER_ADDR2", ref isChanged);// 戶籍地址
                compareForAMLCheckLog(htReturn, htInput, "EMAIL", ref isChanged);// E-MAIL
                compareForAMLCheckLog(htReturn, htInput, "REG_CITY", ref isChanged);// 登記地址
                compareForAMLCheckLog(htReturn, htInput, "REG_ADDR1", ref isChanged);// 登記地址
                compareForAMLCheckLog(htReturn, htInput, "REG_ADDR2", ref isChanged);// 登記地址
                #endregion
                if (isChanged)
                {
                    EntityAML_CHECKLOG eAMLCheckLog = new EntityAML_CHECKLOG();
                    eAMLCheckLog.CORP_NO = htInput["CORP_NO"].ToString().Trim();
                    eAMLCheckLog.TRANS_ID = "CSIPJCGQ";
                    eAMLCheckLog.LAST_UPD_BRANCH = txtLAST_UPD_BRANCH.Text.Trim();
                    eAMLCheckLog.LAST_UPD_CHECKER = txtLAST_UPD_CHECKER.Text.Trim();
                    eAMLCheckLog.LAST_UPD_MAKER = txtLAST_UPD_MAKER.Text.Trim();
                    eAMLCheckLog.MOD_USERID = eAgentInfo.agent_id;
                    eAMLCheckLog.MOD_DATE = DateTime.Now.ToString("yyyyMMdd");
                    eAMLCheckLog.MOD_TIME = DateTime.Now.ToString("HHmmss");

                    InsertAMLCheckLog(eAMLCheckLog);
                }
            }
            #endregion

            ClearPage(false);
            base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_039");
            this.txtCardNo1.Text = "";
            this.txtCardNo2.Text = "";
            return true;
        }
        else
        {
            //*異動主機資料失敗
            if (htResult["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
            {
                base.strHostMsg += htResult["HtgMsg"].ToString();
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_027");
            }
            else
            {
                base.strClientMsg += htResult["HtgMsg"].ToString();
            }

            if (htResult["MESSAGE_TYPE"].ToString() == "5555")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    //取長姓名的內容值
    private Hashtable QueryJC68(string strID)
    {
        Hashtable htInput = new Hashtable();
        htInput.Add("TRAN_ID", "JC68");//*查詢
        htInput.Add("PROGRAM_ID", "JCGUA68");//*查詢
        htInput.Add("FUNCTION_CODE", "I");//*統一編號1
        htInput.Add("ID", strID);//身份ID

        Hashtable htP4A_JC68 = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC68, htInput, false, "11", eAgentInfo);

        return htP4A_JC68;
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