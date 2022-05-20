// *****************************************************************
//   作    者：趙呂梁
//   功能說明：特店基本資料一次鍵檔(6001新增一KEY)
//   創建日期：2009/10/29
//   修改記錄：
// <author>            <time>            <TaskID>                <desc>
// ******************************************************************
//20180119 (U) by Grezz, 新增控制項：1. Y_MPOS特店系統服務費免收註記(6086)F001, 2. Y_特店跨行匯費(6116) 
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

public partial class P010104010001 : PageBase
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
            CommonFunction.SetControlsEnabled(pnlText, false);// 清空網頁中所有的輸入欄位
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo1"));// 將【統一編號】(1)設為輸入焦點
            this.txtNewCardNo1.Visible = false;
            this.txtNewCardNo2.Visible = false;
            this.cddl_MemberService.Items.Add(new ListItem("", ""));
            this.cddl_MemberService.Items.Add(new ListItem("Y", "Y"));
            this.cddl_MemberService.SelectedValue = "";
            LoadDropDownList();

            this.btnAdd.Enabled = false;
            this.radSingleMerchant1.Enabled = false;
            this.radSingleMerchant2.Enabled = false;

            //20200213-RQ-2019-030155-003 拿掉獨立店/海外公司，新增分期平台選項
            //this.radSingleMerchant3.Enabled = false;
            //this.radSingleMerchant4.Enabled = false;
            this.radSingleMerchant5.Enabled = false;
            //20210906 jack 自然人收單，新增分期平台選項
            this.radSingleMerchant6.Enabled = false;
        }

        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//20191023 修改：SOC所需資訊  by Peggy
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/29
    /// 修改日期：2009/10/29
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

        btnAdd.Enabled = false;

        this.radSingleMerchant1.Enabled = false;
        this.radSingleMerchant2.Enabled = false;

        //20200213-RQ-2019-030155-003 拿掉獨立店/海外公司，新增分期平台選項
        //this.radSingleMerchant3.Enabled = false;
        //this.radSingleMerchant4.Enabled = false;
        this.radSingleMerchant5.Enabled = false;
        //20210906 jack 自然人收單，新增分期平台選項
        this.radSingleMerchant6.Enabled = false;

        ClearPage(false);
        this.txtNewCardNo1.Visible = false;
        this.txtNewCardNo2.Visible = false;
        // 查詢資料庫
        EntitySet<CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC> eShopBasicSet = null;
        try
        {
            eShopBasicSet = CSIPKeyInGUI.BusinessRules_new.BRSHOP_BASIC.SelectEntitySet(this.txtCardNo1.Text.Trim(), this.txtCardNo2.Text.Trim(), "1", "N");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            ClearPage(false);
            return;
        }

        if (eShopBasicSet != null && eShopBasicSet.Count > 0)
        {
            ClearPage(true);
            btnAdd.Enabled = false;

            SetValues(eShopBasicSet.GetEntity(0));// 為網頁中的欄位賦值       
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            // 2015/1/22 刪除右下角紅利週期欄位 by Eric
            //this.txtRedeemCycle.Text = "M"; 
            //this.txtRedeemCycle.Enabled = false;

            this.radSingleMerchant1.Enabled = true;
            this.radSingleMerchant2.Enabled = true;

            //20200213-RQ-2019-030155-003 拿掉獨立店/海外公司，新增分期平台選項
            //this.radSingleMerchant3.Enabled = true;
            //this.radSingleMerchant4.Enabled = true;
            this.radSingleMerchant5.Enabled = true;
            //20210906 jack 自然人收單，新增分期平台選項
            this.radSingleMerchant6.Enabled = true;

            // 20220408 修改說明:代入資料後若郵遞區號為空值時，執行查詢郵遞區號動作 by Kelton 
            if (string.IsNullOrEmpty(this.txtREG_ZIP_CODE.Text.Trim()))
                this.SetZipCode();
        }
        else
        {
            Hashtable htInput = new Hashtable();
            htInput.Add("FUNCTION_CODE", "I");// 查詢
            htInput.Add("CORP_NO", this.txtCardNo1.Text.Trim());// 統一編號1
            htInput.Add("CORP_SEQ", this.txtCardNo2.Text.Trim());// 統一編號2
            htInput.Add("FORCE_FLAG", "");

            Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGQ, htInput, false, "11", eAgentInfo);

            if (!htReturn.Contains("HtgMsg"))
            {
                // 20220427 修改說明:代入資料後若郵遞區號為空值時，執行查詢郵遞區號動作 by Kelton 
                if (string.IsNullOrEmpty(htReturn["REG_ZIP_CODE"].ToString().Trim()))
                {
                    this.txtBookAddr1.Text = htReturn["REG_CITY"].ToString();
                    this.SetZipCode();
                    htReturn["REG_ZIP_CODE"] = this.txtREG_ZIP_CODE.Text.Trim();
                }

                ViewState["HtgInfo"] = htReturn;
                base.sbRegScript.Append("if(confirm('" + string.Format(MessageHelper.GetMessage("01_01040101_003"), this.txtCardNo1.Text.Trim() + "-" + this.txtCardNo2.Text.Trim(), htReturn["APPL_NO"].ToString().Trim()) + "')) {$('#btnHiden').click();}else{document.getElementById('txtCardNo1').value='" + "" + "';document.getElementById('txtCardNo2').value='" + "" + "';document.getElementById('txtCardNo1').focus()}");
            }
            else
            {
                if (htReturn["MESSAGE_TYPE"] != null)
                {
                    ClearPage(true);
                    btnAdd.Enabled = false;
                    this.txtCheckMan.Text = "0000";
                    int intTime = int.Parse(DateTime.Now.ToString("yyyyMMdd")) - 19110000;
                    this.txtReceiveNumber.Text = intTime.ToString().PadLeft(7, '0');
                    // 2015/1/22 刪除右下角紅利週期欄位 by Eric
                    //this.txtRedeemCycle.Text = "M";
                    //this.txtRedeemCycle.Enabled = false;
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                    base.strHostMsg += MessageHelper.GetMessage("01_01040101_006"); //新特店，請繼續新增作業.                                        

                    this.radSingleMerchant1.Enabled = true;
                    this.radSingleMerchant2.Enabled = true;

                    //20200213-RQ-2019-030155-003 拿掉獨立店/海外公司，新增分期平台選項
                    //this.radSingleMerchant3.Enabled = true;
                    //this.radSingleMerchant4.Enabled = true;
                    this.radSingleMerchant5.Enabled = true;
                    //20210906 jack 自然人收單，新增分期平台選項
                    this.radSingleMerchant6.Enabled = true;

                    //20190822 修改：狀態為新增時 by Peggy
                    chkisLongName.Checked = false;
                    CheckBox_CheckedChanged(chkisLongName, null);
                    chkisLongName_c.Checked = false;
                    CheckBox_CheckedChanged(chkisLongName_c, null);

                }
                else
                {
                    //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 start
                    etMstType = eMstType.Select;
                    //add by linhuanhuang 修改鍵檔GUI訊息呈現方式 20100709 end

                    base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo1"));
                    // 異動主機資料失敗
                    if (htReturn["HtgMsgFlag"].ToString() == "0")// 若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                    {
                        base.strHostMsg += htReturn["HtgMsg"].ToString();
                        base.strClientMsg += MessageHelper.GetMessage("01_00000000_026");
                    }
                    else
                    {
                        base.strClientMsg += htReturn["HtgMsg"].ToString();
                    }

                    ClearPage(false);
                }
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

    //查詢商店資料
    protected void btnSelect_ShopInfo(object sender, EventArgs e)
    {
        string corpType = "";

        if (this.radSingleMerchant1.Checked)
        {
            corpType = "1";
        }
        else if (this.radSingleMerchant2.Checked)
        {
            corpType = "2";
        }
        //20200213-RQ-2019-030155-003 拿掉獨立店/海外公司，新增分期平台選項
        //else if (this.radSingleMerchant3.Checked)
        //{
        //    corpType = "3";
        //}
        //else if (this.radSingleMerchant4.Checked)
        //{
        //    corpType = "4";
        //}
        else if (this.radSingleMerchant5.Checked)
        {
            corpType = "5";
        }
        //20210906 jack 自然人收單，新增分期平台選項
        else if (this.radSingleMerchant6.Checked)
        {
            corpType = "6";
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

        this.btnAdd.Enabled = false;

        if (corpType == "")
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040101_013") + "');$('#radSingleMerchant1').focus();");
            return;
        }

        Hashtable htInput = new Hashtable();
        htInput.Add("FUNCTION_CODE", "I");                          // 查詢
        htInput.Add("BRCH_CORP_NO", this.txtCardNo1.Text.Trim());   // 分公司統編
        htInput.Add("BRCH_SEQ", "0000");                            // 分公司統編序號
        if(radSingleMerchant6.Checked)//20211119_Ares_Jack_自然人查詢特店資料
        {
            if (this.txtBossID.Text != "")
            {
                // 負責人ID, 統編放前八碼, 序號放後兩碼加兩個空格
                htInput.Add("HCOP_CORP_NO", this.txtBossID.Text.Substring(0, 8).Trim());
                htInput.Add("HCOP_CORP_SEQ", this.txtBossID.Text.Substring(8, 2).Trim() + "  ");
            }
        }
        else
        {
            htInput.Add("HCOP_CORP_NO", this.txtUniNo.Text.Trim());     // 總公司統編
            htInput.Add("HCOP_CORP_SEQ", "0000");                       // 總公司統編序號
        }
        htInput.Add("CORP_TYPE", corpType);                         // 總公司類型

        Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC67, htInput, false, "11", eAgentInfo);

        string hostMsg = "";
        string clientMsg = "";
        string messageType = "";

        if (htReturn.Contains("MESSAGE_TYPE"))
        {
            messageType = htReturn["MESSAGE_TYPE"].ToString();
            clientMsg = GetClientMsg(messageType);
            hostMsg = htReturn["MESSAGE_CHI"].ToString();

            ViewState["HtgInfoAML"] = htReturn;
        }
        else
        {
            clientMsg = GetClientMsg("");
        }

        ////代表主機查到資料
        //if (messageType == "0001" || messageType == "0006" || messageType == "0007" || messageType == "0008")
        //{
        //    this.btnAdd.Enabled = true;
        //}

        base.strHostMsg += hostMsg;
        base.strClientMsg += MessageHelper.GetMessage(clientMsg);
        //base.sbRegScript.Append("alert('" + strClientMsg + "');$('#txtUniNo').focus();");


        //if (messageType != "0001" && messageType != "0006" && messageType != "0007" && messageType != "0008")

        //20190614 Dirk修改，自然人統編因不需要報送AML，故排除自然人的統編不去查詢AML
        string exclude = "03077208,78360022";

        if (messageType != "0001" && messageType != "0006" && messageType != "0007" && messageType != "0008" && !exclude.Contains(this.txtCardNo1.Text.Trim()))
        {
            base.sbRegScript.Append("alert('" + strClientMsg + "');$('#txtUniNo').focus();");
        }
        else//代表主機查到資料
        {
            /*
                 * QUALIFY_FLAG 的判斷，僅針對分期平台時才做檢核
                 * 各種公司選項皆判斷不合作註記欄位
                 *勾選分期平台 + QUALIFY_FLAG = 'N' + 不合作註記 = 'N' 時，才能往下繼續鍵檔 
                 */
            DataTable dt_HCOP = new DataTable();
            //讀取主機現行總公司狀態 //20211123_Ares_Jack_判斷是否為自然人
            if (radSingleMerchant6.Checked)
            {
                dt_HCOP = BRAML_HCOP_STATUS.GetHCOPQualify_Flag(txtBossID.Text.Substring(0, 8).Trim(), txtBossID.Text.Substring(8, 2).Trim());// 6001鍵檔時判斷(自然人)
            }
            else
            {
                dt_HCOP = BRAML_HCOP_STATUS.GetHCOPQualify_Flag(txtUniNo.Text.Trim());
            }
            string _Qualify_Flag = string.Empty;
            string _NonCooperation = string.Empty;
            bool _NewCorp = false;//是否為總公司未往來的案件

            if (dt_HCOP != null && dt_HCOP.Rows.Count > 0)
            {
                _Qualify_Flag = dt_HCOP.Rows[0]["QUALIFY_FLAG"].ToString().Trim();
                _NonCooperation = dt_HCOP.Rows[0]["NonCooperation"].ToString().Trim();

                _NewCorp = false;
            }
            else
            {
                _NewCorp = true;
            }

            //非不合作商店
            //20200812-RQ-2020-021027-001-新增一例外處理狀況，如勾選
            //if (!_NonCooperation.Trim().Equals("Y") || _NewCorp)
            if (!_NonCooperation.Trim().Equals("Y") || _NewCorp || chkUnsightCooperation.Checked)
            {
                //if (messageType == "0001" || messageType == "0006")
                if (messageType == "0001" || messageType == "0006" || exclude.Contains(this.txtCardNo1.Text.Trim()))
                {
                    //20200215-RQ-2019-030155-003
                    #region 原始程式
                    /*原始程式
                     * if (exclude.Contains(this.txtCardNo1.Text.Trim()))
                        {
                            //查詢成功，因不報送AML，請繼續鍵檔
                            base.sbRegScript.Append("alert('" + strClientMsg + ",因不報送AML" + MessageHelper.GetMessage("01_01040101_024") + "');$('#txtAMLCC').focus();");
                        }
                        else
                        {
                            //查詢成功，請繼續鍵檔
                            base.sbRegScript.Append("alert('" + strClientMsg + MessageHelper.GetMessage("01_01040101_024") + "');$('#txtAMLCC').focus();");
                        }

                        this.btnAdd.Enabled = true;
                     */
                    #endregion

                    if (exclude.Contains(this.txtCardNo1.Text.Trim()))//鍵檔統編存在自然人統編清單中
                    {
                        //查詢成功，因不報送AML，請繼續鍵檔
                        base.sbRegScript.Append("alert('" + strClientMsg + ",因不報送AML" + MessageHelper.GetMessage("01_01040101_024") + "');$('#txtAMLCC').focus();");
                        this.btnAdd.Enabled = true;
                    }
                    else
                    {
                        if (corpType.Trim().Equals("5") && (!_Qualify_Flag.Trim().Equals("N") && !_NewCorp))
                        {
                            base.sbRegScript.Append("alert('" + strClientMsg + ",此統編需報送AML無法點選分期平台" + MessageHelper.GetMessage("01_01040101_025") + "');$('#txtAMLCC').focus();");
                            this.btnAdd.Enabled = false;
                        }
                        else
                        {
                            //查詢成功，請繼續鍵檔
                            base.sbRegScript.Append("alert('" + strClientMsg + MessageHelper.GetMessage("01_01040101_024") + "');$('#txtAMLCC').focus();");
                            this.btnAdd.Enabled = true;
                        }
                    }
                }

                if (messageType == "0007" || messageType == "0008")
                {
                    base.sbRegScript.Append("if(confirm('" + strClientMsg + MessageHelper.GetMessage("01_01040101_025") + "')){$('#btn007008Hiden').click();}else{$('#txtUniNo').focus();}");
                }
            }
            else //不合作商店
            {
                this.btnAdd.Enabled = false;

                base.sbRegScript.Append("alert('" + strClientMsg + "\\n 總公司存在不合作名單" + MessageHelper.GetMessage("01_01040101_025") + "');$('#txtAMLCC').focus();");
                base.strClientMsg += "\\n 總公司存在不合作名單" + MessageHelper.GetMessage("01_01040101_025");
            }
        }
    }


    /// 作者 趙呂梁
    /// 創建日期：2009/10/29
    /// 修改日期：2009/10/29
    /// <summary>
    /// 新增事件
    /// </summary>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        DateTime realDate;
        string date = "";
        #region 檢核
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

        //20211229_Ares_Jack_自然人收單時檢核負責人ID, 國籍
        if (this.radSingleMerchant6.Checked)
        {
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
                sbRegScript.Append("alert('" + strAlertMsg + "'); ");
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

        //如果負責人國籍非ＴＷ
        if (!string.IsNullOrEmpty(txtCountryCode.Text.Trim()) && txtCountryCode.Text.Trim().ToUpper() != "TW")
        {
            if (string.IsNullOrEmpty(txtPassportNo.Text) && string.IsNullOrEmpty(txtResidentNo.Text))
            {
                base.sbRegScript.Append("alert('負責人國籍非ＴＷ，護照號碼或統一證號擇一填寫');");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
                return;
            }
                /*20191001 10月需求-效期無需檢核
                else
                {
                    if (!string.IsNullOrEmpty(txtPassportNo.Text))
                    {
                        if (string.IsNullOrEmpty(txtPassportExpdt.Text))
                        {
                            base.sbRegScript.Append("alert('負責人國籍非ＴＷ，護照效期不可空白');$('#txtPassportExpdt').focus();");
                            return;
                        }
                    }

                    if (!string.IsNullOrEmpty(txtResidentNo.Text))
                    {
                        if (string.IsNullOrEmpty(txtResidentExpdt.Text))
                        {
                            base.sbRegScript.Append("alert('負責人國籍非ＴＷ，居留效期不可空白');$('#txtResidentExpdt').focus();");
                            return;
                        }
                    }
                }
                */
            }

        /*20191001 10月需求-效期無需檢核
        //護照號碼不為空
        if (!string.IsNullOrEmpty(txtPassportNo.Text))
        {
            //護照效期空白
            if (string.IsNullOrEmpty(txtPassportExpdt.Text))
            {
                base.sbRegScript.Append("alert('護照效期不可空白');$('#txtPassportExpdt').focus();");
                return;
            }           
        }
        */

        //居留證號碼不為空
        if (!string.IsNullOrEmpty(txtResidentNo.Text))
        {
            if (txtResidentNo.Text.Length != 10)
            {
                base.sbRegScript.Append("alert('統一證號須為10碼');$('#txtResidentNo').focus();");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
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
            /*20191001 10月需求-效期無需檢核
            //居留效期空白
            if (string.IsNullOrEmpty(txtResidentExpdt.Text))
            {
                base.sbRegScript.Append("alert('居留效期不可空白');$('#txtResidentExpdt').focus();");
                return;
            }
            */
        }

        //20220106_Ares_Jack_國籍不得選無
        if (this.txtCountryCode.Text == "無")
        {
            base.sbRegScript.Append("alert('負責人國籍不得選無');$('#txtCountryCode').focus();");
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
            if (txtboss_1_L.Text.Trim().Equals(""))
            {
                base.sbRegScript.Append("alert('負責人長姓名FLAG勾選時，請輸入中文長姓名');$('#txtboss_1_L').focus();");
                return;
            }
            
            if (txtboss_1_Pinyin.Text.Trim().Equals(""))
            {
                base.sbRegScript.Append("alert('負責人長姓名FLAG勾選時，請輸入羅馬拼音');$('#txtboss_1_Pinyin').focus();");
                return;
            }

            if ((ToWide(txtboss_1_L.Text.Trim()).Length + LongNameRomaClean(txtboss_1_Pinyin.Text).Trim().Length) < 5)
            {
                base.sbRegScript.Append("alert('負責人長姓名FLAG勾選時，負責人姓名(中文+羅馬拼音)需超過4個字以上');$('#txtboss_1_L').focus();");
                return;
            }
        }

        if (chkisLongName_c.Checked)
        {
            if (txtcontact_man_L.Text.Trim().Equals(""))
            {
                base.sbRegScript.Append("alert('聯絡人長姓名FLAG勾選時，請輸入中文長姓名');$('#txtcontact_man_L').focus();");
                return;
            }

            if (txtcontact_man_Pinyin.Text.Trim().Equals(""))
            {
                base.sbRegScript.Append("alert('聯絡人長姓名FLAG勾選時，請輸入羅馬拼音');$('#txtcontact_man_Pinyin').focus();");
                return;
            }

            if ((ToWide(txtcontact_man_L.Text.Trim()).Length + LongNameRomaClean(txtcontact_man_Pinyin.Text).Trim().Length) < 5)
            {
                base.sbRegScript.Append("alert('聯絡人長姓名FLAG勾選時，負責人姓名(中文+羅馬拼音)需超過4個字以上');$('#txtcontact_man_L').focus();");
                return;
            }
        }

        if (!txtboss_1_Pinyin.Text.Trim().Equals(""))
        {
            if (!ValidRoma(txtboss_1_Pinyin.Text.Trim()))
            {
                base.sbRegScript.Append("alert('負責人羅馬拼音輸入有誤');$('#txtboss_1_Pinyin').focus();");
                return;
            }
        }
        if (!txtcontact_man_Pinyin.Text.Trim().Equals(""))
        {
            if (!ValidRoma(txtcontact_man_Pinyin.Text.Trim()))
            {
                base.sbRegScript.Append("alert('聯絡人羅馬拼音輸入有誤');$('#txtcontact_man_Pinyin').focus();");
                return;
            }
        }
        //20190806-RQ-2019-008595-002-長姓名需求 by Peggy↑
        #endregion

        #region 2015/1/22 by Eric
        if (this.cddl_MemberService.SelectedValue.ToUpper() == "Y" && this.txtJCIC.Text.Trim().ToUpper() != "A")
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040101_008") + "');");

            return;
        }
        #endregion

        //
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

        Hashtable htAML = (Hashtable)ViewState["HtgInfoAML"];
        string strMsg = null;

       

        //if (messageType != "0001" && messageType != "0006" && messageType != "0007" && messageType != "0008" && !exclude.Contains(this.txtCardNo1.Text.Trim()))

        //0006代表新增
        //if (htAML.ContainsKey("MESSAGE_TYPE") && htAML["MESSAGE_TYPE"].ToString() != "0006")

        string exclude = "03077208,78360022";

        if (htAML.ContainsKey("MESSAGE_TYPE") && htAML["MESSAGE_TYPE"].ToString() != "0006" && !exclude.Contains(this.txtCardNo1.Text.Trim()))
        {

            if (htAML["CHINESE_NAME"].ToString().Trim().ToUpper() != txtBoss.Text.ToUpper())//負責人姓名
            {
                strMsg += BaseHelper.GetShowText("01_01040101_012") + "資料不一致\\n";
            }

            if (htAML["ID"].ToString().ToUpper() != txtBossID.Text.ToUpper())//負責人ID
            {
                strMsg += BaseHelper.GetShowText("01_01040101_013") + "資料不一致\\n";
            }

            if (htAML["NATION"].ToString().ToUpper() != txtCountryCode.Text.ToUpper())
            {
                strMsg += BaseHelper.GetShowText("01_01040101_071") + "資料不一致\\n";
            }

            if (htAML["PASSPORT"].ToString().ToUpper() != txtPassportNo.Text.ToUpper())
            {
                strMsg += BaseHelper.GetShowText("01_01040101_072") + "資料不一致\\n";
            }

            if (htAML["PASSPORT_EXP_DATE"].ToString().ToUpper() != txtPassportExpdt.Text.ToUpper())
            {
                strMsg += BaseHelper.GetShowText("01_01040101_077") + "資料不一致\\n";
            }

            if (htAML["RESIDENT_NO"].ToString().ToUpper() != txtResidentNo.Text.ToUpper())
            {
                strMsg += BaseHelper.GetShowText("01_01040101_073") + "資料不一致\\n";
            }

            if (htAML["RESIDENT_EXP_DATE"].ToString().ToUpper() != txtResidentExpdt.Text.ToUpper())
            {
                strMsg += BaseHelper.GetShowText("01_01040101_078") + "資料不一致\\n";
            }

            string regAddr = txtRegAddr1.Text + txtRegAddr2.Text + txtRegAddr3.Text;
            string htAddr = htAML["PERM_CITY"].ToString().Trim().ToUpper() + htAML["PERM_ADDR1"].ToString().Trim().ToUpper() + htAML["PERM_ADDR2"].ToString().Trim().ToUpper();
            if (htAddr != regAddr)
            {
                strMsg += BaseHelper.GetShowText("01_01040101_015") + "資料不一致\\n";
            }

            string BossChangeDate = txtBossChangeDate.Text;

            if (BossChangeDate.Length == 7)
            {
                BossChangeDate = ConvertToDC(BossChangeDate);
            }

            if (htAML["OWNER_ID_ISSUE_DATE"].ToString().ToUpper() != BossChangeDate)
            {
                strMsg += BaseHelper.GetShowText("01_01040101_061").Replace("<br/>", "") + "資料不一致\\n";
            }

            if (htAML["OWNER_ID_REPLACE_TYPE"].ToString().ToUpper() != txtBossFlag.Text.ToUpper())
            {
                strMsg += BaseHelper.GetShowText("01_01040101_033") + "資料不一致\\n";
            }

            string BossBirthday = txtBossBirthday.Text;

            if (BossBirthday.Length == 7)
            {
                BossBirthday = ConvertToDC(BossBirthday);
            }

            if (htAML["BIRTH_DATE"].ToString().ToUpper() != BossBirthday)
            {
                strMsg += BaseHelper.GetShowText("01_01040101_034") + "資料不一致\\n";
            }

            if (htAML["OWNER_ID_ISSUE_PLACE"].ToString().Trim().ToUpper() != txtBossAt.Text.ToUpper())
            {
                strMsg += BaseHelper.GetShowText("01_01040101_040") + "資料不一致\\n";
            }

            //20190806-RQ-2019-008595-002-長姓名需求，當JC67的長姓名FLAG勾選時，比對長姓名資料是否相符 by Peggy
            if (!string.IsNullOrEmpty(htAML["OWNER_LNAM_FLAG"].ToString()) && htAML["OWNER_LNAM_FLAG"].ToString().Trim().Equals("Y"))
            {
                EntityHTG_JC68 htReturn_JC68 = GetJC68(htAML["ID"].ToString().Trim());
                if (htReturn_JC68.LONG_NAME.Trim() != txtboss_1_L.Text.Trim())
                {
                    strMsg += "負責人中文長姓名資料不一致\\n";
                }
                if (htReturn_JC68.PINYIN_NAME.Trim() != LongNameRomaClean(txtboss_1_L.Text.Trim()))
                {
                    strMsg += "負責人羅馬拼音資料不一致\\n";
                }
            }
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

            if (!string.IsNullOrEmpty(strMsg))
            {
                strMsg += "是否確定要修改?";

                base.sbRegScript.Append("if(confirm('" + strMsg + "')) {$('#btnAddHidenDataConfirm').click();}else{ };");
            }
            else
            {
                btnAddHiden_Click(sender, e);
            }

            //btnAddHiden_Click(sender, e);
        }
        else
        {
            this.lblZipText.Text = "";
            this.lblBusinessZipText.Text = "";

            if (!string.IsNullOrEmpty(strMsg))
            {
                strMsg += ",是否確定要修改?";

                base.sbRegScript.Append("if(confirm('" + strMsg + "')) {$('#btnAddHiden').click();}else{ }");
            }
            else
            {
                base.sbRegScript.Append("$('#btnAddHiden').click();");
            }            
        }
    }


    /// 作者 黃建榮
    /// 創建日期：2019/2/27
    /// 創建日期：2019/2/27
    /// <summary>
    /// 新增隱藏事件
    /// </summary>
    protected void btn007008Hiden_Click(object sender, EventArgs e)
    {
        this.btnAdd.Enabled = true;
        base.sbRegScript.Append(BaseHelper.SetFocus("txtAMLCC"));
    }


    /// 作者 趙呂梁
    /// 創建日期：2010/1/20
    /// 修改日期：2010/1/20
    /// <summary>
    /// 新增隱藏事件
    /// </summary>
    protected void btnAddHiden_Click(object sender, EventArgs e)
    {
        EntitySet<CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC> eShopBasicSet = null;
        string strCardNo1 = "";
        string strCardNo2 = "";
        string strFocus = "";// 焦點

        if (this.txtNewCardNo1.Visible && this.txtNewCardNo2.Visible)
        {
            strCardNo1 = this.txtNewCardNo1.Text.Trim();
            strCardNo2 = this.txtNewCardNo2.Text.Trim();
            strFocus = "txtNewCardNo1";
        }
        else
        {
            strCardNo1 = this.txtCardNo1.Text.Trim();
            strCardNo2 = this.txtCardNo2.Text.Trim();
            strFocus = "txtCardNo1";
        }

        try
        {
            eShopBasicSet = CSIPKeyInGUI.BusinessRules_new.BRSHOP_BASIC.SelectData(strCardNo1, strCardNo2, "1");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            ClearPage(false);
            return;
        }

        bool blnSucceed = false;// 是否新增或修改成功
        if (eShopBasicSet != null && eShopBasicSet.Count > 0)
        {
            if (eShopBasicSet.GetEntity(0).sendhost_flag.Trim() != "Y")
            {
                //提示是否要覆蓋一Key資料
                if (this.txtNewCardNo1.Visible && this.txtNewCardNo2.Visible)
                {
                    base.sbRegScript.Append("if(confirm('" + string.Format(MessageHelper.GetMessage("01_01040101_007"), this.txtNewCardNo1.Text.Trim() + "-" + this.txtNewCardNo2.Text.Trim()) + "')) {$('#btnAddUpdateHiden').click();}else{document.getElementById('txtNewCardNo1').focus()}");
                    return;
                }
                else
                {
                    if (UpdateData(eShopBasicSet.GetEntity(0).keyin_day.Trim()))// 更新
                    {
                        blnSucceed = true;
                    }
                }
            }
            else
            {
                // 已上傳主機
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040101_005") + "');");
                base.sbRegScript.Append(BaseHelper.SetFocus(strFocus));
                return;
            }
        }
        else
        {
            if (AddNewData())// 新增
            {
                blnSucceed = true;
            }
        }

        if (blnSucceed)// 新增或修改成功
        {
            this.txtCardNo1.Enabled = true;
            this.txtCardNo2.Enabled = true;
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo1"));// 將【統一編號】(1)設為輸入焦點
            base.strClientMsg += MessageHelper.GetMessage("01_01040101_001");
            ClearPage(false);
            this.txtCardNo1.Text = "";//統一編號 清空
            this.txtCardNo2.Text = "";//新統一編號 清空
            this.txtNewCardNo1.Visible = false;
            this.txtNewCardNo2.Visible = false;
            this.cddl_MemberService.SelectedValue = "";
        }
    }

    /// 創建日期：2010/5/26
    /// 修改日期：2010/5/26
    /// <summary>
    /// 新增隱藏事件
    /// </summary>
    protected void btnAddUpdateHiden_Click(object sender, EventArgs e)
    {
        EntitySet<CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC> eShopBasicSet = null;
        string strCardNo1 = "";
        string strCardNo2 = "";
        string strFocus = "";// 焦點

        strCardNo1 = this.txtNewCardNo1.Text.Trim();
        strCardNo2 = this.txtNewCardNo2.Text.Trim();
        strFocus = "txtNewCardNo1";

        try
        {
            eShopBasicSet = CSIPKeyInGUI.BusinessRules_new.BRSHOP_BASIC.SelectData(strCardNo1, strCardNo2, "1");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            ClearPage(false);
            return;
        }

        bool blnSucceed = false;// 是否新增或修改成功

        if (eShopBasicSet != null && eShopBasicSet.Count > 0)
        {
            if (eShopBasicSet.GetEntity(0).sendhost_flag.Trim() != "Y")
            {
                if (UpdateData(eShopBasicSet.GetEntity(0).keyin_day.Trim()))// 更新
                {
                    blnSucceed = true;
                }
            }
            else
            {
                // 已上傳主機
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040101_005") + "');");
                base.sbRegScript.Append(BaseHelper.SetFocus(strFocus));
                return;
            }
        }

        if (blnSucceed)// 新增或修改成功
        {
            this.txtCardNo1.Enabled = true;
            this.txtCardNo2.Enabled = true;
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo1"));// 將【統一編號】(1)設為輸入焦點
            base.strClientMsg += MessageHelper.GetMessage("01_01040101_001");
            ClearPage(false);
            this.txtNewCardNo1.Visible = false;
            this.txtNewCardNo2.Visible = false;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2010/1/20
    /// 修改日期：2010/1/20
    /// <summary>
    /// 查詢隱藏事件
    /// </summary>
    protected void btnHiden_Click(object sender, EventArgs e)
    {
        this.radSingleMerchant1.Enabled = true;
        this.radSingleMerchant2.Enabled = true;

        //20200213-RQ-2019-030155-003 拿掉獨立店/海外公司，新增分期平台選項
        //this.radSingleMerchant3.Enabled = true;
        //this.radSingleMerchant4.Enabled = true;
        this.radSingleMerchant5.Enabled = true;
        //20210906 jack 自然人收單，新增分期平台選項
        this.radSingleMerchant6.Enabled = true;

        ClearPage(true);
        Hashtable htReturn = (Hashtable)ViewState["HtgInfo"];
        base.strHostMsg += htReturn["HtgSuccess"].ToString();// 主機返回成功訊息

        #region
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

        /*20191024 修改：因操作模式關係，有可能叫出舊資料是有實際經營者的資訊，故以空白帶給主機，以絕後患!!  by Peggy
        this.txtOperID.Text = htReturn["MANAGER_ID"].ToString();          // 實際經營者ID
        this.txtOperTel1.Text = htReturn["MANAGER_PHONE_AREA"].ToString();// 實際經營者電話1
        this.txtOperTel2.Text = htReturn["MANAGER_PHONE_NO"].ToString();  // 實際經營者電話2
        this.txtOperTel3.Text = htReturn["MANAGER_PHONE_EXT"].ToString(); // 實際經營者電話3
        this.txtOperman.Text = htReturn["MANAGER_NAME"].ToString();       // 實際經營者姓名
        this.txtOperChangeDate.Text = htReturn["CHANGE_DATE2"].ToString();// 實際經營者領換補日
        this.txtOperBirthday.Text = htReturn["BIRTHDAY2"].ToString();     // 實際經營者生日
        this.txtOperFlag.Text = htReturn["CHANGE_FLAG2"].ToString();      // 實際經營者代號
        this.txtOperAt.Text = htReturn["AT2"].ToString();                 // 實際經營者換證點
        */
        this.txtOperID.Text = "";                     // 實際經營者ID
        this.txtOperTel1.Text = "";                 // 實際經營者電話1
        this.txtOperTel2.Text = "";                 // 實際經營者電話2
        this.txtOperTel3.Text = "";                 // 實際經營者電話3
        this.txtOperman.Text = "";                // 實際經營者姓名
        this.txtOperChangeDate.Text = "";// 實際經營者領換補日
        this.txtOperBirthday.Text = "";        // 實際經營者生日
        this.txtOperFlag.Text = "";                // 實際經營者代號
        this.txtOperAt.Text = "";                   // 實際經營者換證點


        //this.txtOrganization.Text = htReturn["ORGAN_TYPE"].ToString();    // 組織
        this.txtPopMan.Text = htReturn["SALE_NAME"].ToString();           // 推廣員
        //this.txtReceiveNumber.Text = htReturn["APPL_NO"].ToString();// 收件編號
        int intTime = int.Parse(DateTime.Now.ToString("yyyyMMdd")) - 19110000;
        this.txtReceiveNumber.Text = intTime.ToString().PadLeft(7, '0');

        this.txtRegAddr1.Text = htReturn["OWNER_CITY"].ToString(); // 戶籍地址1
        this.txtRegAddr2.Text = htReturn["OWNER_ADDR1"].ToString();// 戶籍地址2
        this.txtRegAddr3.Text = htReturn["OWNER_ADDR2"].ToString();// 戶籍地址3
        this.txtRegName.Text = htReturn["REG_NAME"].ToString();    // 登記名稱
        this.txtRisk.Text = htReturn["RISK_FLAG"].ToString();      // 風險

        this.txtBossChangeDate.Text = htReturn["CHANGE_DATE1"].ToString();// 負責人領換補日
        this.txtBossBirthday.Text = htReturn["BIRTHDAY1"].ToString();     // 負責人生日
        this.txtBossFlag.Text = htReturn["CHANGE_FLAG1"].ToString();      // 負責人代號
        this.txtBossAt.Text = htReturn["AT1"].ToString();                 // 負責人換證點
        
        this.txtJCIC.Text = htReturn["JCIC_CODE"].ToString();             // JCIC查詢
        this.txtPrevDesc.Text = htReturn["IPMR_PREV_DESC"].ToString();    // 帳單內容
        // 2015/1/22 刪除右下角紅利週期欄位 by Eric
        //this.txtRedeemCycle.Text = htReturn["REDEEM_CYCLE"].ToString();// 紅利週期(M/D)
        if (htReturn["REDEEM_CYCLE"] != null && (htReturn["REDEEM_CYCLE"].ToString().Trim() == "" ||
            htReturn["REDEEM_CYCLE"].ToString().Trim().ToUpper() == "Y"))
            this.cddl_MemberService.SelectedValue = htReturn["REDEEM_CYCLE"].ToString().Trim();
        else
            this.cddl_MemberService.SelectedValue = "";
        this.lblZipText.Text = htReturn["REAL_ZIP"].ToString();// 營業地郵遞區號
        #endregion

        #region 電文新增欄位
        this.txtEstablish.Text = htReturn["BUILD_DATE"].ToString() + htReturn["BUILD_DATE_DD"].ToString(); // 設立
        this.txtOrganization.Text = htReturn["ORGAN_TYPE_NEW"].ToString();                              // 組織

        // 商店別標識(1.總公司已往來 2.總公司未往來 3.獨立店 4.海外公司)
        if (htReturn["SINGLE_MERCHANT"].ToString() == "1")
            this.radSingleMerchant1.Checked = true;// 總公司已往來
        else if (htReturn["SINGLE_MERCHANT"].ToString() == "2")
            this.radSingleMerchant2.Checked = true;// 總公司未往來
        //20200213-RQ-2019-030155-003 拿掉獨立店/海外公司，新增分期平台選項
        //else if (htReturn["SINGLE_MERCHANT"].ToString() == "3")
        //    this.radSingleMerchant3.Checked = true;// 獨立店
        //else if (htReturn["SINGLE_MERCHANT"].ToString() == "4")
        //    this.radSingleMerchant4.Checked = true;// 海外公司
        //else if (htReturn["SINGLE_MERCHANT"].ToString() == "4")
        //    this.radSingleMerchant4.Checked = true;// 分期平台
        else if (htReturn["SINGLE_MERCHANT"].ToString() == "5")
            this.radSingleMerchant5.Checked = true;// 分期平台
        //20210906 jack 自然人收單，新增分期平台選項
        else if (htReturn["SINGLE_MERCHANT"].ToString() == "6")
            this.radSingleMerchant6.Checked = true;// 自然人收單

        this.txtUniNo.Text = htReturn["HEADQUARTER_CORPNO"].ToString();// 統一編號
        this.txtAMLCC.Text = htReturn["AML_CC"].ToString();            // AML行業編號
        this.txtCountryCode.Text = htReturn["COUNTRY_CODE"].ToString();// 國籍
        this.txtPassportNo.Text = htReturn["PASSPORT_NO"].ToString();  // 護照號碼
        this.txtPassportExpdt.Text = htReturn["PASSPORT_EXPDT"].ToString();// 護照效期
        this.txtResidentNo.Text = htReturn["RESIDENT_NO"].ToString();  // 居留證號
        this.txtResidentExpdt.Text = htReturn["RESIDENT_EXPDT"].ToString();// 居留效期
        SetEmailValue(htReturn["EMAIL"].ToString().Trim());            // 電子郵件信箱
        #endregion

        //20190822 修改 by Peggy
        if (!string.IsNullOrEmpty(htReturn["OWNER_LNAM_FLAG"].ToString()) && htReturn["OWNER_LNAM_FLAG"].ToString().Trim().Equals("Y"))
        {
            chkisLongName.Checked = true;
            EntityHTG_JC68 htReturn_JC68 = GetJC68(htReturn["OWNER_ID"].ToString().Trim());
            txtboss_1_L.Text = htReturn_JC68.LONG_NAME;
            txtboss_1_Pinyin.Text = htReturn_JC68.PINYIN_NAME;
        }
        if (!string.IsNullOrEmpty(htReturn["CONTACT_LNAM_FLAG"].ToString()) && htReturn["CONTACT_LNAM_FLAG"].ToString().Trim().Equals("Y"))
        {
            chkisLongName_c.Checked = true;
            EntityHTG_JC68 htReturn_JC68 = GetJC68(txtCardNo1.Text.Trim() + txtCardNo2.Text.Trim());
            txtcontact_man_L.Text = htReturn_JC68.LONG_NAME;
            txtcontact_man_Pinyin.Text = htReturn_JC68.PINYIN_NAME;
        }
        CheckBox_CheckedChanged(chkisLongName, null);
        CheckBox_CheckedChanged(chkisLongName_c, null);

        // 2015/1/22 刪除右下角紅利週期欄位 by Eric
        //this.txtRedeemCycle.Text = "M";
        //this.txtRedeemCycle.Enabled = false;
        this.txtCardNo1.Enabled = false;
        this.txtCardNo2.Enabled = false;
        this.txtNewCardNo1.Visible = true;
        this.txtNewCardNo2.Visible = true;

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

        btnAdd.Enabled = false;

        base.sbRegScript.Append(BaseHelper.SetFocus("txtNewCardNo1"));

        //202109023_Ares_jack 統編符合involve範圍時，將預設勾選自然人收單選項
        string involve = "78360020,78360021,78360023,78360024,78360025,78360026,78360027,78360028,78360029";
        if (involve.Contains(this.txtCardNo1.Text.Trim()))
        {
            //202109023_Ares_jack 當確認為自然人收單時 與公司往來的Radio Button不讓USER選
            radSingleMerchant6.Checked = true;
            radSingleMerchant6.Enabled = false;
            radSingleMerchant1.Enabled = false;
            radSingleMerchant2.Enabled = false;
            radSingleMerchant5.Enabled = false;
        }
        else
        {
            //20211207_Ares_jack_統編若非自然人請將  自然人收單選項 Disabled
            radSingleMerchant6.Enabled = false;
            radSingleMerchant6.Checked = false;
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
            lblOpermanText.Text = BRCommon.ChangeToSBC(txtBoss.Text);
            lblOperIDText.Text = txtBossID.Text;
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
    /// 同登記地址料擇事件
    /// </summary>
    protected void chkAddress_CheckedChanged(object sender, EventArgs e)
    {
        if (chkAddress.Checked)
        {
            lblBusinessAddrText1.Text = BRCommon.ChangeToSBC(txtBookAddr1.Text.Trim());
            lblBusinessAddrText2.Text = BRCommon.ChangeToSBC(txtBookAddr2.Text);
            lblBusinessAddrText3.Text = BRCommon.ChangeToSBC(txtBookAddr3.Text);

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
    #endregion

    #region 方法區

    private string GetClientMsg(string messageType)
    {
        string result = "";

        switch (messageType)
        {
            case "0001":
                result = "01_01040101_010";//查詢成功
                break;
            case "0003":
                result = "01_01040101_012";//總公司已存在
                break;
            case "0002":
                result = "01_01040101_014";//總公司不存在
                break;
            case "0004":
                result = "01_01040101_015";//分公司統一編號錯誤
                break;
            case "0005":
                result = "01_01040101_016";//分公司統編序號錯誤
                break;
            case "0006":
                result = "01_01040101_017";//分公司不存在
                break;
            case "0007":
                result = "01_01040101_018";//總公司統編與分公司檔中的總公司統編不同
                break;
            case "0008":
                result = "01_01040101_019";//總公司統編序號與分公司檔中的總公司統編序號不同
                break;
            case "8880":
                result = "01_01040101_020";// 上傳資料錯誤
                break;
            case "8881":
                result = "01_01040101_021";//功能代碼錯誤
                break;
            case "8888":
                result = "01_01040101_022";//檔案未開啟
                break;
            case "9999":
                result = "01_01040101_023";//系統錯誤
                break;
            default:
                result = "01_01040101_023";//系統錯誤
                break;
        }

        return result;
    }

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

        // 載入組織
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
        CommonFunction.SetControlsEnabled(pnlText, blnEnabled);// 清空網頁中所有的輸入欄位
        chkAddress.Checked = false;
        chkBusinessName.Checked = false;
        chkOper.Checked = false;
        lblBusinessNameText.Text = "";
        ClearOwnerLable();// 清空負責人相關資料
        ClearAddLable();  // 清空地址資料
        txtCardNo1.Enabled = true;
        txtCardNo2.Enabled = true;
        this.txtJCIC.Text = "";
        this.txtGrantFeeFlag.Text = "";
        this.txtMposFlag.Text = "";
        this.txtREG_ZIP_CODE.Enabled = false;

        #region 表單新增欄位
        this.txtUniNo.Text = "";        // 總公司已往來統一編號
        this.txtAMLCC.Text = "";        // AML行業編號
        this.txtCountryCode.Text = "";  // 國籍
        this.txtPassportNo.Text = "";   // 護照號碼
        this.txtPassportExpdt.Text = "";// 護照效期
        this.txtResidentNo.Text = "";   // 居留證號
        this.txtResidentExpdt.Text = "";// 居留效期
        // 電子郵件信箱
        this.txtEmailFront.Text = "";
        this.txtEmailOther.Text = "";
        this.hidEmailFall.Value = "";
        #endregion

        //20190806-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy↓
        chkisLongName.Checked = false;
        CheckBox_CheckedChanged(chkisLongName, null);
        chkisLongName_c.Checked = false;
        CheckBox_CheckedChanged(chkisLongName_c, null);
        //20190806-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy↑
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
        lblZipText.Text = "";
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
    /// 清空除統一編號外的所有欄位值
    /// </summary>
    private void ClearShowValue()
    {
        string strUnion1 = txtCardNo1.Text.Trim();
        string strUnion2 = txtCardNo2.Text.Trim();
        ClearPage(true);
        txtCardNo1.Text = strUnion1;
        txtCardNo2.Text = strUnion2;
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
    /// 創建日期：2009/10/29
    /// 修改日期：2009/10/29
    /// <summary>
    /// 向資料庫中新增資料
    /// </summary>
    /// <returns>true成功,false失敗</returns>
    private bool AddNewData()
    {
        CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC eShopBasic = GetEntity();

        try
        {
            return CSIPKeyInGUI.BusinessRules_new.BRSHOP_BASIC.AddEntity(eShopBasic);
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return false;
        }
    }

    /// <summary>
    /// 更新資料庫
    /// </summary>
    /// <returns></returns>
    private bool UpdateData(string strKeyInDay)
    {
        CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC eShopBasic = GetEntity();

        try
        {
            return CSIPKeyInGUI.BusinessRules_new.BRSHOP_BASIC.Update(eShopBasic, eShopBasic.uni_no1.Trim(), eShopBasic.uni_no2.Trim(), strKeyInDay, "1", "N");
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
    private CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC GetEntity()
    {
        CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC eShopBasic = new CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC();

        if (this.txtNewCardNo1.Visible && this.txtNewCardNo2.Visible)
        {
            eShopBasic.uni_no1 = this.txtNewCardNo1.Text.Trim();
            eShopBasic.uni_no2 = this.txtNewCardNo2.Text.Trim();
        }
        else
        {
            eShopBasic.uni_no1 = this.txtCardNo1.Text.Trim();
            eShopBasic.uni_no2 = this.txtCardNo2.Text.Trim();
        }

        eShopBasic.bank = this.txtBank.Text.Trim();        
        eShopBasic.book_addr1 = this.txtBookAddr1.Text.Trim();
        eShopBasic.book_addr2 = this.txtBookAddr2.Text;
        eShopBasic.book_addr3 = this.txtBookAddr3.Text;
        eShopBasic.boss_1 = this.txtBoss.Text;
        eShopBasic.boss_id = this.txtBossID.Text.Trim().ToUpper();
        eShopBasic.boss_tel1 = this.txtBossTel1.Text.Trim();
        eShopBasic.boss_tel2 = this.txtBossTel2.Text.Trim();
        eShopBasic.boss_te3 = this.txtBossTel3.Text.Trim();
        eShopBasic.boss_change_date = this.txtBossChangeDate.Text.Trim();
        eShopBasic.boss_change_flag = this.txtBossFlag.Text.Trim();
        eShopBasic.boss_birthday = this.txtBossBirthday.Text.Trim();
        eShopBasic.boss_at = this.txtBossAt.Text.Trim();
        eShopBasic.branch_bank = this.txtBranchBank.Text;

        // 營業地址
        if (!chkAddress.Checked)
        {
            eShopBasic.zip = this.lblZipText.Text.Trim();
            eShopBasic.business_addr1 = this.txtBusinessAddr4.Text.Trim();
            eShopBasic.business_addr2 = this.txtBusinessAddr5.Text;
            eShopBasic.business_addr3 = this.txtBusinessAddr6.Text;
            eShopBasic.chk_address = "";
        }
        else
        {
            // 20211230 同登記地址勾選 代入登記地址 by Ares Dennis
            eShopBasic.zip = this.txtREG_ZIP_CODE.Text.Trim();
            eShopBasic.business_addr1 = this.txtBookAddr1.Text.Trim();
            eShopBasic.business_addr2 = this.txtBookAddr2.Text.Trim();
            eShopBasic.business_addr3 = this.txtBookAddr3.Text.Trim();
            eShopBasic.chk_address = "1";
        }

        // 營業名稱
        if (!chkBusinessName.Checked)
        {
            eShopBasic.business_name = this.txtBusinessName.Text;
            eShopBasic.chk_business_name = "";
        }
        else
        {
            // 20211230 同登記名稱勾選 代入登記名稱 by Ares Dennis
            eShopBasic.business_name = this.txtRegName.Text.Trim();
            eShopBasic.chk_business_name = "1";
        }

        eShopBasic.capital = this.txtCapital.Text.Trim();
        eShopBasic.checkman = this.txtCheckMan.Text.Trim();
        eShopBasic.contact_man = this.txtContactMan.Text;
        eShopBasic.contact_tel1 = this.txtContactManTel1.Text.Trim();
        eShopBasic.contact_tel2 = this.txtContactManTel2.Text.Trim();
        eShopBasic.contact_tel3 = this.txtContactManTel3.Text.Trim();
        eShopBasic.establish = this.txtEstablish.Text.Trim();
        eShopBasic.fax1 = this.txtFax1.Text.Trim();
        eShopBasic.fax2 = this.txtFax2.Text.Trim();
        eShopBasic.keyin_userID = eAgentInfo.agent_id;
        eShopBasic.invoice_cycle = this.txtInvoiceCycle.Text.Trim();
        eShopBasic.keyin_day = DateTime.Now.ToString("yyyyMMdd");
        eShopBasic.name = this.txtName.Text;

        /*20191024 修改：因操作模式關係，有可能叫出舊資料是有實際經營者的資訊，故以空白帶給主機，以絕後患!!  by Peggy
        // 實際經營者相關資料
        if (!chkOper.Checked)
        {
            eShopBasic.oper_id = "";//this.txtOperID.Text.Trim().ToUpper();
            eShopBasic.oper_tel1 = this.txtOperTel1.Text.Trim();
            eShopBasic.oper_tel2 = this.txtOperTel2.Text.Trim();
            eShopBasic.oper_tel3 = this.txtOperTel3.Text.Trim();
            eShopBasic.operman = this.txtOperman.Text;
            eShopBasic.oper_change_date = this.txtOperChangeDate.Text.Trim();
            eShopBasic.oper_change_flag = this.txtOperFlag.Text.Trim();
            eShopBasic.oper_birthday = this.txtOperBirthday.Text.Trim();
            eShopBasic.oper_at = this.txtOperAt.Text.Trim();
            eShopBasic.chk_oper = "";
        }
        else
        {
            eShopBasic.oper_id = "";// this.lblOperIDText.Text.Trim().ToUpper();
            eShopBasic.oper_tel1 = this.lblOperTelText1.Text.Trim();
            eShopBasic.oper_tel2 = this.lblOperTelText2.Text.Trim();
            eShopBasic.oper_tel3 = this.lblOperTelText3.Text.Trim();
            eShopBasic.operman = BRCommon.ChangeToSBC(this.lblOpermanText.Text);
            eShopBasic.oper_change_date = this.lblOperChangeDateText.Text.Trim();
            eShopBasic.oper_change_flag = this.lblOperFlagText.Text.Trim();
            eShopBasic.oper_birthday = this.lblOperBirthdayText.Text.Trim();
            eShopBasic.oper_at = BRCommon.ChangeToSBC(this.lblOperAtText.Text.Trim());
            eShopBasic.chk_oper = "1";
        }
        */
        eShopBasic.oper_id = "";
        eShopBasic.oper_tel1 = "";
        eShopBasic.oper_tel2 = "";
        eShopBasic.oper_tel3 = "";
        eShopBasic.operman = "";
        eShopBasic.oper_change_date = "";
        eShopBasic.oper_change_flag = "";
        eShopBasic.oper_birthday = "";
        eShopBasic.oper_at = "";
        eShopBasic.chk_oper = "";

        eShopBasic.organization = this.txtOrganization.Text.Trim();
        eShopBasic.pop_man = this.txtPopMan.Text;
        eShopBasic.recv_no = this.txtReceiveNumber.Text.Trim();
        eShopBasic.reg_addr1 = this.txtRegAddr1.Text.Trim();
        eShopBasic.reg_addr2 = this.txtRegAddr2.Text;
        eShopBasic.reg_addr3 = this.txtRegAddr3.Text;
        eShopBasic.reg_name = this.txtRegName.Text.Trim();
        eShopBasic.risk = this.txtRisk.Text.Trim();
        eShopBasic.sendhost_flag = "N";
        eShopBasic.keyin_flag = "1";
        eShopBasic.prev_desc = this.txtPrevDesc.Text;
        // 2015/1/22 刪除右下角紅利週期欄位 by Eric
        //eShopBasic.redeem_cycle = this.txtRedeemCycle.Text.Trim().ToUpper();
        eShopBasic.member_service = this.cddl_MemberService.SelectedValue;
        eShopBasic.jcic_code = this.txtJCIC.Text.Trim().ToUpper();
        eShopBasic.jcic_code = this.txtJCIC.Text.Trim().ToUpper();
        eShopBasic.mpos_flag = this.txtMposFlag.Text.Trim().ToUpper();         // Y_MPOS特店系統服務費免收註記(6086)F001
        eShopBasic.grant_fee_flag = this.txtGrantFeeFlag.Text.Trim().ToUpper();// Y_特店跨行匯費(6116)

        #region 表單新增欄位
        // 商店別標識(1.總公司已往來 2.總公司未往來 3.獨立店 4.海外公司)
        if (this.radSingleMerchant1.Checked)
            eShopBasic.single_merchant = "1";// 總公司已往來
        else if (this.radSingleMerchant2.Checked)
            eShopBasic.single_merchant = "2";// 總公司未往來

        //20200213-RQ-2019-030155-003 拿掉獨立店/海外公司，新增分期平台選項
        //else if (this.radSingleMerchant3.Checked)
        //    eShopBasic.single_merchant = "3";// 獨立店
        //else if (this.radSingleMerchant4.Checked)
        //    eShopBasic.single_merchant = "4";// 海外公司
        //else if (this.radSingleMerchant4.Checked)
        //    eShopBasic.single_merchant = "4";// 分期平台
        else if (this.radSingleMerchant5.Checked)
            eShopBasic.single_merchant = "5";// 分期平台
        //20210906 jack 自然人收單，新增分期平台選項
        else if (this.radSingleMerchant6.Checked)
            eShopBasic.single_merchant = "6";// 自然人收單

        eShopBasic.headquarter_corpno = this.txtUniNo.Text.Trim();      // 統一編號
        eShopBasic.aml_cc = this.txtAMLCC.Text.Trim();                  // AML行業編號
        eShopBasic.country_code = this.txtCountryCode.Text.Trim();      // 國籍
        eShopBasic.passport_no = this.txtPassportNo.Text.Trim();        // 護照號碼
        eShopBasic.passport_expdt = this.txtPassportExpdt.Text.Trim();  // 護照效期
        eShopBasic.resident_no = this.txtResidentNo.Text.Trim();        // 居留證號
        eShopBasic.resident_expdt = this.txtResidentExpdt.Text.Trim();  // 居留效期
        eShopBasic.email = this.hidEmailFall.Value.Trim();              // 電子郵件信箱
        #endregion


        //20190806-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy↓

        eShopBasic.boss_1_L = ToWide(txtboss_1_L.Text.Trim());
        eShopBasic.boss_1_Pinyin = txtboss_1_Pinyin.Text.Trim();
        
        eShopBasic.contact_man_L = ToWide(txtcontact_man_L.Text.Trim());
        eShopBasic.contact_man_Pinyin = txtcontact_man_Pinyin.Text.Trim();

        //20190806-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy↑

        // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis
        eShopBasic.REG_ZIP_CODE = this.txtREG_ZIP_CODE.Text.Trim();// 登記地址郵遞區號
        eShopBasic.LAST_UPD_BRANCH = this.txtLAST_UPD_BRANCH.Text.Trim();// 資料最後異動分行
        eShopBasic.LAST_UPD_CHECKER = this.txtLAST_UPD_CHECKER.Text.Trim();// 資料最後異動-CHECKER
        eShopBasic.LAST_UPD_MAKER = this.txtLAST_UPD_MAKER.Text.Trim();// 資料最後異動-MAKER

        return eShopBasic;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/29
    /// 修改日期：2009/10/29
    /// <summary>
    /// 為網頁中的欄位賦值
    /// </summary>
    /// <param name="eShopBasic">EntitySHOP_BASIC</param>
    private void SetValues(CSIPKeyInGUI.EntityLayer_new.EntitySHOP_BASIC eShopBasic)
    {
        if (eShopBasic.member_service != null && (eShopBasic.member_service.Trim() == "" ||
            eShopBasic.member_service.Trim().ToUpper() == "Y"))
        {
            this.cddl_MemberService.SelectedValue = eShopBasic.member_service;
        }

        this.txtBank.Text = eShopBasic.bank.Trim();        
        this.txtBookAddr1.Text = eShopBasic.book_addr1;
        this.txtBookAddr2.Text = eShopBasic.book_addr2;
        this.txtBookAddr3.Text = eShopBasic.book_addr3;
        this.txtBoss.Text = eShopBasic.boss_1;
        this.txtBossID.Text = eShopBasic.boss_id;
        this.txtBossTel1.Text = eShopBasic.boss_tel1;
        this.txtBossTel2.Text = eShopBasic.boss_tel2;
        this.txtBossTel3.Text = eShopBasic.boss_te3;
        this.txtBranchBank.Text = eShopBasic.branch_bank;

        // 營業地址
        if (eShopBasic.chk_address == "1")
        {
            this.chkAddress.Checked = true;
            this.lblBusinessAddrText1.Text = eShopBasic.business_addr1;
            this.lblBusinessAddrText2.Text = eShopBasic.business_addr2;
            this.lblBusinessAddrText3.Text = eShopBasic.business_addr3;
            this.lblBusinessZipText.Text = eShopBasic.zip;// 營業地郵遞區號 
        }
        else
        {
            this.txtBusinessAddr4.Text = eShopBasic.business_addr1;
            this.txtBusinessAddr5.Text = eShopBasic.business_addr2;
            this.txtBusinessAddr6.Text = eShopBasic.business_addr3;
            this.lblZipText.Text = eShopBasic.zip;// 營業地郵遞區號
        }

        // 營業名稱
        if (eShopBasic.chk_business_name == "1")
        {
            this.lblBusinessNameText.Text = eShopBasic.business_name;
            this.chkBusinessName.Checked = true;
        }
        else
        {
            this.txtBusinessName.Text = eShopBasic.business_name;
        }
        this.txtCapital.Text = eShopBasic.capital;
        this.txtCheckMan.Text = eShopBasic.checkman;

        this.txtContactMan.Text = eShopBasic.contact_man;
        this.txtContactManTel1.Text = eShopBasic.contact_tel1;
        this.txtContactManTel2.Text = eShopBasic.contact_tel2;
        this.txtContactManTel3.Text = eShopBasic.contact_tel3;
        this.txtEstablish.Text = eShopBasic.establish;
        this.txtFax1.Text = eShopBasic.fax1;
        this.txtFax2.Text = eShopBasic.fax2;
        this.txtInvoiceCycle.Text = eShopBasic.invoice_cycle;

        this.txtName.Text = eShopBasic.name;

        this.txtOrganization.Text = eShopBasic.organization;
        //20190803 將法律形式的dropdownlist set在值的地方
        this.dropOrganization.SelectByValue(eShopBasic.organization);
        this.txtPopMan.Text = eShopBasic.pop_man;
        this.txtReceiveNumber.Text = eShopBasic.recv_no;
        this.txtRegAddr1.Text = eShopBasic.reg_addr1;
        this.txtRegAddr2.Text = eShopBasic.reg_addr2;
        this.txtRegAddr3.Text = eShopBasic.reg_addr3;
        this.txtRegName.Text = eShopBasic.reg_name;
        this.txtRisk.Text = eShopBasic.risk;

        this.txtBossChangeDate.Text = eShopBasic.boss_change_date;//負責人領換補日
        this.txtBossBirthday.Text = eShopBasic.boss_birthday;     // 負責人生日
        this.txtBossFlag.Text = eShopBasic.boss_change_flag;      // 負責人代號
        this.txtBossAt.Text = eShopBasic.boss_at;                 // 負責人換證點
        /*20191024 修改：因操作模式關係，有可能叫出舊資料是有實際經營者的資訊，故以空白帶給主機，以絕後患!!  by Peggy
        if (eShopBasic.chk_oper == "1")
        {
            this.chkOper.Checked = true;
            this.lblOpermanText.Text = eShopBasic.operman;                // 實際經營者姓名
            this.lblOperIDText.Text = "";// eShopBasic.oper_id;                 // 實際經營者ID
            this.lblOperTelText1.Text = eShopBasic.oper_tel1;             // 實際經營者電話
            this.lblOperTelText2.Text = eShopBasic.oper_tel2;
            this.lblOperTelText3.Text = eShopBasic.oper_tel3;
            this.lblOperChangeDateText.Text = eShopBasic.oper_change_date;// 實際經營者領換補日
            this.lblOperFlagText.Text = eShopBasic.oper_change_flag;      // 實際經營者代號
            this.lblOperBirthdayText.Text = eShopBasic.oper_birthday;     // 實際經營者生日
            this.lblOperAtText.Text = eShopBasic.oper_at;                 //實際經營者換證點
        }
        else
        {
            this.txtOperman.Text = eShopBasic.operman;                // 實際經營者姓名
            this.txtOperID.Text = "";// eShopBasic.oper_id;                 // 實際經營者ID
            this.txtOperTel1.Text = eShopBasic.oper_tel1;             // 實際經營者電話
            this.txtOperTel2.Text = eShopBasic.oper_tel2;
            this.txtOperTel3.Text = eShopBasic.oper_tel3;
            this.txtOperChangeDate.Text = eShopBasic.oper_change_date;// 實際經營者領換補日
            this.txtOperFlag.Text = eShopBasic.oper_change_flag;      // 實際經營者代號
            this.txtOperBirthday.Text = eShopBasic.oper_birthday;     // 實際經營者生日
            this.txtOperAt.Text = eShopBasic.oper_at;                 // 實際經營者換證點
        }
        */
        this.txtOperman.Text = "";                // 實際經營者姓名
        this.txtOperID.Text = "";// eShopBasic.oper_id;                 // 實際經營者ID
        this.txtOperTel1.Text = "";             // 實際經營者電話
        this.txtOperTel2.Text = "";
        this.txtOperTel3.Text = "";
        this.txtOperChangeDate.Text = "";// 實際經營者領換補日
        this.txtOperFlag.Text = "";      // 實際經營者代號
        this.txtOperBirthday.Text = "";     // 實際經營者生日
        this.txtOperAt.Text = "";                 // 實際經營者換證點

        this.txtJCIC.Text = eShopBasic.jcic_code;             // JCIC查詢
        this.txtMposFlag.Text = eShopBasic.mpos_flag;         // Y_MPOS特店系統服務費免收註記
        this.txtGrantFeeFlag.Text = eShopBasic.grant_fee_flag;// Y_特店跨行匯費
        this.txtPrevDesc.Text = eShopBasic.prev_desc;         // 帳單內容

        #region 表單新增欄位
        // 商店別標識(1.總公司已往來 2.總公司未往來 3.獨立店 4.海外公司 5.分期平台)
        if (eShopBasic.single_merchant == "1")
            this.radSingleMerchant1.Checked = true;// 總公司已往來
        else if (eShopBasic.single_merchant == "2")
            this.radSingleMerchant2.Checked = true;// 總公司未往來
        //20200213-RQ-2019-030155-003 拿掉獨立店/海外公司，新增分期平台選項
        //else if (eShopBasic.single_merchant == "3")
        //    this.radSingleMerchant3.Checked = true;// 獨立店
        //else if (eShopBasic.single_merchant == "4")
        //    this.radSingleMerchant4.Checked = true;// 海外公司
        //else if (eShopBasic.single_merchant == "4")
        //    this.radSingleMerchant4.Checked = true;// 分期平台
        else if (eShopBasic.single_merchant == "5")
            this.radSingleMerchant5.Checked = true;// 分期平台
        //20210906 jack 自然人收單，新增分期平台選項
        else if (eShopBasic.single_merchant == "6")
            this.radSingleMerchant6.Checked = true;//自然人收單

        this.txtUniNo.Text = eShopBasic.headquarter_corpno;         // 統一編號
        this.txtAMLCC.Text = eShopBasic.aml_cc;                     // AML行業編號
        this.txtCountryCode.Text = eShopBasic.country_code;         // 國籍
        this.dropCountryCode.SelectByValue(eShopBasic.country_code);//20190731 修改：使下單選拉也focus在資料值的index by Peggy
        this.txtPassportNo.Text = eShopBasic.passport_no;           // 護照號碼
        this.txtPassportExpdt.Text = eShopBasic.passport_expdt;     // 護照效期
        this.txtResidentNo.Text = eShopBasic.resident_no;           // 居留證號
        this.txtResidentExpdt.Text = eShopBasic.resident_expdt;     // 居留效期
        SetEmailValue(eShopBasic.email);                            // 電子郵件信箱
        #endregion

        // 2015/1/22 刪除右下角紅利週期欄位 by Eric
        //this.txtRedeemCycle.Text = eShopBasic.redeem_cycle;// 紅利週期(M/D)

        //20190806-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy↓
        this.txtboss_1_L.Text = eShopBasic.boss_1_L;//負責人-中文長姓名
        this.txtboss_1_Pinyin.Text = eShopBasic.boss_1_Pinyin;//負人責-羅馬拼音

        if (!txtboss_1_L.Text.Trim().Equals(""))//負責人-是否長姓名
        {
            this.chkisLongName.Checked = true;
        }
        
        this.txtcontact_man_L.Text = eShopBasic.contact_man_L;//聯絡人-中文長姓名
        this.txtcontact_man_Pinyin.Text = eShopBasic.contact_man_Pinyin;//聯絡人-羅馬拼音
        if (!txtcontact_man_L.Text.Trim().Equals(""))//聯絡人-是否長姓名
        {
            this.chkisLongName_c.Checked = true;
        }

        //20190822 修改 by Peggy
        CheckBox_CheckedChanged(chkisLongName, null);
        CheckBox_CheckedChanged(chkisLongName_c, null);
        //20190806-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy↑

        // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis
        this.txtREG_ZIP_CODE.Text = eShopBasic.REG_ZIP_CODE;// 登記地址郵遞區號
        this.txtLAST_UPD_BRANCH.Text = eShopBasic.LAST_UPD_BRANCH;// 資料最後異動分行
        this.txtLAST_UPD_CHECKER.Text = eShopBasic.LAST_UPD_CHECKER;// 資料最後異動-CHECKER
        this.txtLAST_UPD_MAKER.Text = eShopBasic.LAST_UPD_MAKER;// 資料最後異動-MAKER
    }
    #endregion
    protected void radSingleMerchant1_CheckedChanged(object sender, EventArgs e)
    {
        btnSelectShopInfo.Enabled = true;
        btnAdd.Enabled = false;
    }

    protected void radSingleMerchant2_CheckedChanged(object sender, EventArgs e)
    {
        btnSelectShopInfo.Enabled = true;
        btnAdd.Enabled = false;
    }
    /*
    protected void radSingleMerchant3_CheckedChanged(object sender, EventArgs e)
    {

        btnSelectShopInfo.Enabled = true;
        btnAdd.Enabled = false;
    }

    protected void radSingleMerchant4_CheckedChanged(object sender, EventArgs e)
    {
        //btnSelectShopInfo.Enabled = false;
        //btnAdd.Enabled = true;
        btnSelectShopInfo.Enabled = true;
        btnAdd.Enabled = false;
    }
    */

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
                CtrlName1 = "txtBoss";//人員名稱
                CtrlName2 = "txtboss_1_L";//中文長姓名
                CtrlName3 = "txtboss_1_Pinyin";//馬拼音
                break;
            case "chkisLongName_c"://聯絡人
                CtrlName1 = "txtContactMan";//人員名稱
                CtrlName2 = "txtcontact_man_L";//中文長姓名
                CtrlName3 = "txtcontact_man_Pinyin";//羅馬拼音
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
        string lid = string.Empty;
        CustTextBox txt = (CustTextBox)sender;

        string CtrlName = string.Empty;//長姓名flag
        string CtrlName1 = string.Empty;//原人員姓名

        switch (txt.ID.Trim())
        {
            case "txtboss_1_L"://負責人中文長姓名
                CtrlName = "chkisLongName";//人員名稱
                CtrlName1 = "txtBoss";//負責人
                break;
            case "txtcontact_man_L"://聯絡人
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

    //取長姓名的內容值
    private EntityHTG_JC68 GetJC68(string strID)
    {
        EntityHTG_JC68 _result = new EntityHTG_JC68();
        using (BRHTG_JC68 obj = new BRHTG_JC68("P010104010001"))
        {
            EntityHTG_JC68 _data = new EntityHTG_JC68();

            _data.ID = strID;
            _result = obj.getData(_data, eAgentInfo, "11");
        }
        return _result;
    }
    #endregion

    protected void radSingleMerchant5_CheckedChanged(object sender, EventArgs e)
    {
        btnSelectShopInfo.Enabled = true;
        btnAdd.Enabled = false;
    }

    //20210906 jack 自然人收單，新增分期平台選項
    protected void radSingleMerchant6_CheckedChanged(object sender, EventArgs e)
    {
        btnSelectShopInfo.Enabled = true;
        btnAdd.Enabled = false;
    }

    /// <summary>
    /// 使用者輸入的地址解析後系統自動帶入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void TextBox_AddrChanged(object sender, EventArgs e)
    {
        // 20220408 修改說明:將執行查詢郵遞區號動作調整為獨立方法，供多個地方使用 by Kelton 
        this.SetZipCode();
        base.sbRegScript.Append(BaseHelper.SetFocus("txtBookAddr2"));// 將地址2設為輸入焦點
    }

    private void SetZipCode()
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
    }
}
 