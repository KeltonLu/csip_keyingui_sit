//******************************************************************
//*  作    者：趙呂梁

//*  功能說明：特店基本資料一次鍵檔

//*  創建日期：2009/10/29
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
using Framework.Common.Message;
using CSIPCommonModel.EntityLayer;
using CSIPKeyInGUI.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Common.Logging;
using Framework.WebControls;
using System.Drawing;


public partial class P010104010101 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CommonFunction.SetControlsEnabled(pnlText, false);//*清空網頁中所有的輸入欄位
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo1"));//*將【統一編號】(1)設為輸入焦點
            this.txtNewCardNo1.Visible = false;
            this.txtNewCardNo2.Visible = false;       
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/29
    /// 修改日期：2009/10/29
    /// <summary>
    /// 新增事件
    /// </summary>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
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
            btnAddHiden_Click(sender, e);
        }
        else
        {
            this.lblZipText.Text = "";
            this.lblBusinessZipText.Text = "";
            base.sbRegScript.Append("$('#btnAddHiden').click();");
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
    /// 新增隱藏事件
    /// </summary>
    protected void btnAddHiden_Click(object sender, EventArgs e)
    {
        EntitySet<EntitySHOP_BASIC> eShopBasicSet = null;
        string strCardNo1 = "";
        string strCardNo2 = "";
        string strFocus = "";//*焦點

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
            eShopBasicSet = BRSHOP_BASIC.SelectData(strCardNo1, strCardNo2, "1");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            ClearPage(false);
            return;
        }

        bool blnSucceed = false;//*是否新增或修改成功
        if (eShopBasicSet != null && eShopBasicSet.Count > 0)
        {
            if (eShopBasicSet.GetEntity(0).sendhost_flag.Trim() != "Y")
            {
                if (UpdateData(eShopBasicSet.GetEntity(0).keyin_day.Trim()))//*更新
                {
                    blnSucceed = true;
                }
            }
            else
            {
                //*已上傳主機
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040101_005") + "');");
                base.sbRegScript.Append(BaseHelper.SetFocus(strFocus));
                return;
            }
        }
        else
        {
            if (AddNewData())//*新增
            {
                blnSucceed = true;
            }
        }
        
        if (blnSucceed)//*新增或修改成功
        {
            this.txtCardNo1.Enabled = true;
            this.txtCardNo2.Enabled = true;
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo1"));//*將【統一編號】(1)設為輸入焦點
            base.strClientMsg += MessageHelper.GetMessage("01_01040101_001");
            ClearPage(false);
            this.txtNewCardNo1.Visible = false;
            this.txtNewCardNo2.Visible = false;
        } 
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/29
    /// 修改日期：2009/10/29
    /// <summary>
    /// 查詢事件
    /// </summary>
    protected void btnSelect_Click(object sender, EventArgs e)
    {     
        ClearPage(true);
        this.txtNewCardNo1.Visible = false;
        this.txtNewCardNo2.Visible = false;
        //*查詢資料庫
        EntitySet<EntitySHOP_BASIC> eShopBasicSet = null;
        try
        {
            eShopBasicSet = BRSHOP_BASIC.SelectEntitySet(this.txtCardNo1.Text.Trim(), this.txtCardNo2.Text.Trim(), "1", "N");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            ClearPage(false);
            return;
        }

        if (eShopBasicSet != null && eShopBasicSet.Count > 0)
        {
            SetValues(eShopBasicSet.GetEntity(0));//*為網頁中的欄位賦值       
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            this.txtRedeemCycle.Text = "M";
            this.txtRedeemCycle.Enabled = false;
        }
        else
        {
            Hashtable htInput = new Hashtable();
            htInput.Add("FUNCTION_CODE", "I");//*查詢
            htInput.Add("CORP_NO", this.txtCardNo1.Text.Trim()) ;//*統一編號1
            htInput.Add("CORP_SEQ", this.txtCardNo2.Text.Trim());//*統一編號2
            htInput.Add("FORCE_FLAG", "");

            Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGQ, htInput, false, "11", eAgentInfo);

            if (!htReturn.Contains("HtgMsg"))
            {
                ViewState["HtgInfo"] = htReturn;             
                base.sbRegScript.Append("if(confirm('" + string.Format(MessageHelper.GetMessage("01_01040101_003"), this.txtCardNo1.Text.Trim() + "-" + this.txtCardNo2.Text.Trim(), htReturn["APPL_NO"].ToString().Trim()) + "')) {$('#btnHiden').click();}else{document.getElementById('txtCardNo1').value='" + "" + "';document.getElementById('txtCardNo2').value='" + "" + "';document.getElementById('txtCardNo1').focus()}");
            }
            else
            {
                if ((htReturn["MESSAGE_TYPE"]==null) || (htReturn["MESSAGE_TYPE"].ToString().Trim() != "8888"))
                {
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo1"));
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
                    ClearPage(false);
                }
                else
                {
                    this.txtCheckMan.Text = "0000";
                    int intTime = int.Parse(DateTime.Now.ToString("yyyyMMdd")) - 19110000;
                    this.txtReceiveNumber.Text = intTime.ToString().PadLeft(7, '0');
                    this.txtRedeemCycle.Text = "M";
                    this.txtRedeemCycle.Enabled = false;
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
                }             
            }
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
        Hashtable htReturn = (Hashtable)ViewState["HtgInfo"];
        base.strHostMsg += htReturn["HtgSuccess"].ToString();//*主機返回成功訊息

        #region
        this.txtBank.Text = htReturn["DDA_BANK_NAME"].ToString();//*銀行
        this.txtBookAddr1.Text = htReturn["REG_CITY"].ToString();//*登記地址1
        this.txtBookAddr2.Text = htReturn["REG_ADDR1"].ToString();//*登記地址2
        this.txtBookAddr3.Text = htReturn["REG_ADDR2"].ToString();//*登記地址3
        this.txtBoss.Text = htReturn["OWNER_NAME"].ToString();//*負責人姓名
        this.txtBossID.Text = htReturn["OWNER_ID"].ToString();//*負責人ID
        this.txtBossTel1.Text = htReturn["OWNER_PHONE_AREA"].ToString();//*負責人電話1
        this.txtBossTel2.Text = htReturn["OWNER_PHONE_NO"].ToString();//*負責人電話2
        this.txtBossTel3.Text = htReturn["OWNER_PHONE_EXT"].ToString();//*負責人電話3
        this.txtBranchBank.Text = htReturn["DDA_BANK_BRANCH"].ToString();//*分行
        this.txtBusinessAddr4.Text = htReturn["REAL_CITY"].ToString();//*營業地址2
        this.txtBusinessAddr5.Text = htReturn["REAL_ADDR1"].ToString();//*營業地址3
        this.txtBusinessAddr6.Text = htReturn["REAL_ADDR2"].ToString();//*營業地址4
        this.txtBusinessName.Text = htReturn["BUSINESS_NAME"].ToString();//*營業名稱
        this.txtCapital.Text = htReturn["CAPITAL"].ToString();//*資本
        this.txtCheckMan.Text = htReturn["CREDIT_NO"].ToString();//*徵信員

        this.txtContactMan.Text = htReturn["CONTACT_NAME"].ToString();//*聯絡人姓名
        this.txtContactManTel1.Text = htReturn["CONTACT_PHONE_AREA"].ToString();//*聯絡人電話1
        this.txtContactManTel2.Text = htReturn["CONTACT_PHONE_NO"].ToString();//*聯絡人電話2
        this.txtContactManTel3.Text = htReturn["CONTACT_PHONE_EXT"].ToString();//*聯絡人電話3
        this.txtEstablish.Text = htReturn["BUILD_DATE"].ToString();//*設立
        this.txtFax1.Text = htReturn["FAX_AREA"].ToString();//*聯絡人傳真1
        this.txtFax2.Text = htReturn["FAX_PHONE_NO"].ToString();//*聯絡人傳真2
        this.txtInvoiceCycle.Text = htReturn["INVOICE_CYCLE"].ToString();//*發票週期

        this.txtName.Text = htReturn["DDA_ACCT_NAME"].ToString();//*戶名
        this.txtOperID.Text = htReturn["MANAGER_ID"].ToString();//*實際經營者ID
        this.txtOperTel1.Text = htReturn["MANAGER_PHONE_AREA"].ToString();//*實際經營者電話1
        this.txtOperTel2.Text = htReturn["MANAGER_PHONE_NO"].ToString();//*實際經營者電話2
        this.txtOperTel3.Text = htReturn["MANAGER_PHONE_EXT"].ToString();//*實際經營者電話3
        this.txtOperman.Text = htReturn["MANAGER_NAME"].ToString();//*實際經營者姓名
        this.txtOrganization.Text = htReturn["ORGAN_TYPE"].ToString();//*組織
        this.txtPopMan.Text = htReturn["SALE_NAME"].ToString();//*推廣員
        this.txtReceiveNumber.Text = htReturn["APPL_NO"].ToString();//*收件編號

        this.txtRegAddr1.Text = htReturn["OWNER_CITY"].ToString();//*戶籍地址1
        this.txtRegAddr2.Text = htReturn["OWNER_ADDR1"].ToString();//*戶籍地址2
        this.txtRegAddr3.Text = htReturn["OWNER_ADDR2"].ToString();//*戶籍地址3
        this.txtRegName.Text = htReturn["REG_NAME"].ToString();//*登記名稱
        this.txtRisk.Text = htReturn["RISK_FLAG"].ToString();//*風險

        this.txtBossChangeDate.Text = htReturn["CHANGE_DATE1"].ToString();//*負責人領換補日
        this.txtBossBirthday.Text = htReturn["BIRTHDAY1"].ToString();//*負責人生日
        this.txtBossFlag.Text = htReturn["CHANGE_FLAG1"].ToString();//*負責人代號
        this.txtBossAt.Text = htReturn["AT1"].ToString();//*負責人換證點

        this.txtOperChangeDate.Text = htReturn["CHANGE_DATE2"].ToString();//*實際經營者領換補日
        this.txtOperBirthday.Text = htReturn["BIRTHDAY2"].ToString();//*實際經營者生日
        this.txtOperFlag.Text = htReturn["CHANGE_FLAG2"].ToString();//*實際經營者代號
        this.txtOperAt.Text = htReturn["AT2"].ToString();//*實際經營者換證點

        this.txtJCIC.Text = htReturn["JCIC_CODE"].ToString();//*JCIC查詢
        this.txtPrevDesc.Text = htReturn["IPMR_PREV_DESC"].ToString();//*帳單內容
        this.txtRedeemCycle.Text = htReturn["REDEEM_CYCLE"].ToString();//*紅利週期(M/D)
        this.lblZipText.Text = htReturn["REAL_ZIP"].ToString();//*營業地郵遞區號
        #endregion

        this.txtRedeemCycle.Text = "M";
        this.txtRedeemCycle.Enabled = false;
        this.txtCardNo1.Enabled = false;
        this.txtCardNo2.Enabled = false;
        this.txtNewCardNo1.Visible = true;
        this.txtNewCardNo2.Visible = true;
        base.sbRegScript.Append(BaseHelper.SetFocus("txtNewCardNo1"));
    }

    /// 作者 趙呂梁
    /// 創建日期：2010/1/20
    /// 修改日期：2010/1/20
    /// <summary>
    /// 同登記名稱選擇事件
    /// </summary>
    protected void chkBusinessName_CheckedChanged(object sender, EventArgs e)
    {
        if(chkBusinessName.Checked)
        {
            lblBusinessNameText.Text = BRCommon.ChangeToSBC(txtRegName.Text.Trim());
            txtBusinessName.Enabled = false;
        }
        else
        {
            lblBusinessNameText.Text = "";
            txtBusinessName.Enabled = true;
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
        if(chkOper.Checked)
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
        }
        else
        {
            ClearOwnerLable();
            EnabledPartOne(true);
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
        if(chkAddress.Checked)
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
        }
        else
        {
            ClearAddLable();
            EnabledPartTwo(true);
        }
    }
    #endregion

    #region 方法區

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
        txtCardNo1.Enabled = true;
        txtCardNo2.Enabled = true;
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

    /// 作者 趙呂梁
    /// 創建日期：2009/10/29
    /// 修改日期：2009/10/29
    /// <summary>
    /// 得到插入或更新資料的Entity
    /// </summary>
    /// <returns>Entity</returns>
    private EntitySHOP_BASIC GetEntity()
    {
        #region
        EntitySHOP_BASIC eShopBasic = new EntitySHOP_BASIC();

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
        eShopBasic.book_addr2 = this.txtBookAddr2.Text.Trim();
        eShopBasic.book_addr3 = this.txtBookAddr3.Text.Trim();
        eShopBasic.boss_1 = this.txtBoss.Text.Trim();
        eShopBasic.boss_id = this.txtBossID.Text.Trim().ToUpper();
        eShopBasic.boss_tel1 = this.txtBossTel1.Text.Trim();
        eShopBasic.boss_tel2 = this.txtBossTel2.Text.Trim();
        eShopBasic.boss_te3 = this.txtBossTel3.Text.Trim();
        eShopBasic.boss_change_date = this.txtBossChangeDate.Text.Trim();
        eShopBasic.boss_change_flag = this.txtBossFlag.Text.Trim();
        eShopBasic.boss_birthday = this.txtBossBirthday.Text.Trim();
        eShopBasic.boss_at = this.txtBossAt.Text.Trim();
        eShopBasic.branch_bank = this.txtBranchBank.Text.Trim();

        //*營業地址
        if (!chkAddress.Checked)
        {
            eShopBasic.zip = this.lblZipText.Text.Trim();
            eShopBasic.business_addr1 = this.txtBusinessAddr4.Text.Trim();
            eShopBasic.business_addr2 = this.txtBusinessAddr5.Text.Trim();
            eShopBasic.business_addr3 = this.txtBusinessAddr6.Text.Trim();
        }
        else
        {
            eShopBasic.zip = this.lblBusinessZipText.Text.Trim();
            eShopBasic.business_addr1 = BRCommon.ChangeToSBC(this.lblBusinessAddrText1.Text.Trim());
            eShopBasic.business_addr2 = BRCommon.ChangeToSBC(this.lblBusinessAddrText2.Text.Trim());
            eShopBasic.business_addr3 = BRCommon.ChangeToSBC(this.lblBusinessAddrText3.Text.Trim());
        }

        //*營業名稱
        if (!chkBusinessName.Checked)
        {
            eShopBasic.business_name = this.txtBusinessName.Text.Trim();
        }
        else
        {
            eShopBasic.business_name = BRCommon.ChangeToSBC(this.lblBusinessNameText.Text.Trim());
        }

        eShopBasic.capital = this.txtCapital.Text.Trim();
        eShopBasic.checkman = this.txtCheckMan.Text.Trim();
        eShopBasic.contact_man = this.txtContactMan.Text.Trim();
        eShopBasic.contact_tel1 = this.txtContactManTel1.Text.Trim();
        eShopBasic.contact_tel2 = this.txtContactManTel2.Text.Trim();
        eShopBasic.contact_tel3 = this.txtContactManTel3.Text.Trim();
        eShopBasic.establish = this.txtEstablish.Text.Trim();
        eShopBasic.fax1 = this.txtFax1.Text.Trim();
        eShopBasic.fax2 = this.txtFax2.Text.Trim();
        eShopBasic.keyin_userID = eAgentInfo.agent_id;
        eShopBasic.invoice_cycle = this.txtInvoiceCycle.Text.Trim();
        eShopBasic.keyin_day = DateTime.Now.ToString("yyyyMMdd");
        eShopBasic.name = this.txtName.Text.Trim();

        //*實際經營者相關資料
        if (!chkOper.Checked)
        {
            eShopBasic.oper_id = this.txtOperID.Text.Trim().ToUpper();
            eShopBasic.oper_tel1 = this.txtOperTel1.Text.Trim();
            eShopBasic.oper_tel2 = this.txtOperTel2.Text.Trim();
            eShopBasic.oper_tel3 = this.txtOperTel3.Text.Trim();
            eShopBasic.operman = this.txtOperman.Text.Trim();
            eShopBasic.oper_change_date = this.txtOperChangeDate.Text.Trim();
            eShopBasic.oper_change_flag = this.txtOperFlag.Text.Trim();
            eShopBasic.oper_birthday = this.txtOperBirthday.Text.Trim();
            eShopBasic.oper_at = this.txtOperAt.Text.Trim();
        }
        else
        {
            eShopBasic.oper_id = this.lblOperIDText.Text.Trim().ToUpper();
            eShopBasic.oper_tel1 = this.lblOperTelText1.Text.Trim();
            eShopBasic.oper_tel2 = this.lblOperTelText2.Text.Trim();
            eShopBasic.oper_tel3 = this.lblOperTelText3.Text.Trim();
            eShopBasic.operman = BRCommon.ChangeToSBC(this.lblOpermanText.Text.Trim());
            eShopBasic.oper_change_date = this.lblOperChangeDateText.Text.Trim();
            eShopBasic.oper_change_flag = this.lblOperFlagText.Text.Trim();
            eShopBasic.oper_birthday = this.lblOperBirthdayText.Text.Trim();
            eShopBasic.oper_at = BRCommon.ChangeToSBC(this.lblOperAtText.Text.Trim());
        }


        eShopBasic.organization = this.txtOrganization.Text.Trim();
        eShopBasic.pop_man = this.txtPopMan.Text.Trim();
        eShopBasic.recv_no = this.txtReceiveNumber.Text.Trim();
        eShopBasic.reg_addr1 = this.txtRegAddr1.Text.Trim();
        eShopBasic.reg_addr2 = this.txtRegAddr2.Text.Trim();
        eShopBasic.reg_addr3 = this.txtRegAddr3.Text.Trim();
        eShopBasic.reg_name = this.txtRegName.Text.Trim();
        eShopBasic.risk = this.txtRisk.Text.Trim();
        eShopBasic.sendhost_flag = "N";
        eShopBasic.keyin_flag = "1";
        eShopBasic.prev_desc = this.txtPrevDesc.Text.Trim();
        eShopBasic.redeem_cycle = this.txtRedeemCycle.Text.Trim().ToUpper();
        eShopBasic.jcic_code = this.txtJCIC.Text.Trim().ToUpper();
        return eShopBasic; 
        #endregion
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
        EntitySHOP_BASIC eShopBasic = GetEntity();
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

    /// <summary>
    /// 更新資料庫
    /// </summary>
    /// <returns></returns>
    private bool UpdateData(string strKeyInDay)
    {
        EntitySHOP_BASIC eShopBasic = GetEntity();

        try
        {
            return BRSHOP_BASIC.Update(eShopBasic, this.txtCardNo1.Text.Trim(), this.txtCardNo2.Text.Trim(), strKeyInDay, "1", "N");
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
    /// 為網頁中的欄位賦值
    /// </summary>
    /// <param name="eShopBasic">EntitySHOP_BASIC</param>
    private void SetValues(EntitySHOP_BASIC eShopBasic)
    {
        #region
        this.txtBank.Text = eShopBasic.bank;
        this.txtBookAddr1.Text = eShopBasic.book_addr1;
        this.txtBookAddr2.Text = eShopBasic.book_addr2;
        this.txtBookAddr3.Text = eShopBasic.book_addr3;
        this.txtBoss.Text = eShopBasic.boss_1;
        this.txtBossID.Text = eShopBasic.boss_id;
        this.txtBossTel1.Text = eShopBasic.boss_tel1;
        this.txtBossTel2.Text = eShopBasic.boss_tel2;
        this.txtBossTel3.Text = eShopBasic.boss_te3;
        this.txtBranchBank.Text = eShopBasic.branch_bank;
        this.txtBusinessAddr4.Text = eShopBasic.business_addr1;
        this.txtBusinessAddr5.Text = eShopBasic.business_addr2;
        this.txtBusinessAddr6.Text = eShopBasic.business_addr3;
        this.txtBusinessName.Text = eShopBasic.business_name;
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
        this.txtOperID.Text = eShopBasic.oper_id;
        this.txtOperTel1.Text = eShopBasic.oper_tel1;
        this.txtOperTel2.Text = eShopBasic.oper_tel2;
        this.txtOperTel3.Text = eShopBasic.oper_tel3;
        this.txtOperman.Text = eShopBasic.operman;
        this.txtOrganization.Text = eShopBasic.organization;
        this.txtPopMan.Text = eShopBasic.pop_man;
        this.txtReceiveNumber.Text = eShopBasic.recv_no;
        this.txtRegAddr1.Text = eShopBasic.reg_addr1;
        this.txtRegAddr2.Text = eShopBasic.reg_addr2;
        this.txtRegAddr3.Text = eShopBasic.reg_addr3;
        this.txtRegName.Text = eShopBasic.reg_name;
        this.txtRisk.Text = eShopBasic.risk;

        this.txtBossChangeDate.Text = eShopBasic.boss_change_date;//*負責人領換補日
        this.txtBossBirthday.Text = eShopBasic.boss_birthday;//*負責人生日
        this.txtBossFlag.Text = eShopBasic.boss_change_flag;//*負責人代號
        this.txtBossAt.Text = eShopBasic.boss_at;//*負責人換證點

        this.txtOperChangeDate.Text = eShopBasic.oper_change_date;//*實際經營者領換補日
        this.txtOperBirthday.Text = eShopBasic.oper_birthday;//*實際經營者生日
        this.txtOperFlag.Text = eShopBasic.oper_change_flag;//*實際經營者代號
        this.txtOperAt.Text = eShopBasic.oper_at;//*實際經營者換證點

        this.txtJCIC.Text = eShopBasic.jcic_code;//*JCIC查詢
        this.txtPrevDesc.Text = eShopBasic.prev_desc;//*帳單內容
        this.txtRedeemCycle.Text = eShopBasic.redeem_cycle;//*紅利週期(M/D)
        this.lblZipText.Text = eShopBasic.zip;//*營業地郵遞區號       
        #endregion
    }
    #endregion
    
}
