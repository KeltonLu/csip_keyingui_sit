//******************************************************************
//*  作    者：趙呂梁

//*  功能說明：特店基本資料二次鍵檔

//*  創建日期：2009/11/02
//*  修改記錄： 


//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Data;
using System.Drawing;
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

public partial class P010104020101 : PageBase
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
            ViewState["Send"] = false;
            CommonFunction.SetControlsEnabled(pnlText, false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo1"));
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        CommonFunction.SetControlsForeColor(pnlText, Color.Black);
        SetLableForeColor();
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/02
    /// 修改日期：2009/11/02
    /// <summary>
    /// 查詢事件
    /// </summary>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        //*查詢是否上傳主機
        EntitySet<EntitySHOP_BASIC> eShopBasicUploadHtg = null;
        try
        {
            eShopBasicUploadHtg = BRSHOP_BASIC.SelectEntitySet(this.txtCardNo1.Text.Trim(), this.txtCardNo2.Text.Trim(), "Y");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (eShopBasicUploadHtg.Count > 0)//*此筆資料已上傳主機
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040201_002") + "');");
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo1"));
            return;
        }
        
        EntitySet<EntitySHOP_BASIC> eShopBasicSet = null;
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
            EntitySHOP_BASIC eShopBasicOneKey = null;
            bool blnTemp = false;//*是否有一KEY資料
            for(int i = 0; i<eShopBasicSet.Count; i++)
            {
                 if(eShopBasicSet.GetEntity(i).keyin_flag.Trim() == "1")
                {
                    blnTemp = true;
                     strOneKeyUserID = eShopBasicSet.GetEntity(i).keyin_userID.Trim();
                     eShopBasicOneKey = eShopBasicSet.GetEntity(i);
                     ViewState["EntityShopBasic"] = eShopBasicSet.GetEntity(i);
                     break;
                }
            }
        
            if(blnTemp)
            {
                if (eAgentInfo.agent_id.Trim() == strOneKeyUserID)
                {
                    //base.strClientMsg += MessageHelper.GetMessage("01_01040201_012");
                    base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040201_012") + "');");
                    return;
                }

                ClearPage(true);
                blnTemp =false;
                for(int i = 0; i<eShopBasicSet.Count; i++)
                {
                    if (eShopBasicSet.GetEntity(i).keyin_flag.Trim() == "2")
                    {
                        blnTemp = true;
                        SetValues(eShopBasicSet.GetEntity(i));//*為網頁中的欄位賦值
                        break;
                    }
                }

                if (blnTemp == false)
                {                  
                    this.txtCheckMan.Text = "0000";
                    int intTime = int.Parse(DateTime.Now.ToString("yyyyMMdd")) - 19110000;
                    this.txtReceiveNumber.Text = intTime.ToString().PadLeft(7, '0');   
                }
                SetOneKeyValues(eShopBasicOneKey);
                this.txtRedeemCycle.Text = "M";
                this.txtRedeemCycle.Enabled = false;

                base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            }
            else
            {
                //base.strClientMsg += MessageHelper.GetMessage("01_01040201_011");
                base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040201_011") + "');");
                ClearPage(false);
                base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo1"));
            }
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
    /// 新增隱藏事件
    /// </summary>
    protected void btnAddHiden_Click(object sender, EventArgs e)
    {
        //*查詢是否上傳主機
        EntitySet<EntitySHOP_BASIC> eShopBasicUploadHtg = null;
        try
        {
            eShopBasicUploadHtg = BRSHOP_BASIC.SelectEntitySet(this.txtCardNo1.Text.Trim(), this.txtCardNo2.Text.Trim(), "Y");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (eShopBasicUploadHtg.Count > 0)//*此筆資料已上傳主機
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01040201_002") + "');");
            return;
        }
        
        EntitySet<EntitySHOP_BASIC> eShopBasicSet = null;
        try
        {
            eShopBasicSet = BRSHOP_BASIC.SelectEntitySet(this.txtCardNo1.Text.Trim(), this.txtCardNo2.Text.Trim(), "N");
        }
        catch
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        bool blnTwoKey = false;//*是否有二KEY資料

        for (int i = 0; i < eShopBasicSet.Count; i++)
        {
            if (eShopBasicSet.GetEntity(i).keyin_flag.Trim() == "2")
            {
                blnTwoKey = true;
                break;
            }
        }

        if (blnTwoKey)//*原 DB 已存在2key資料
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
            UploadHtgInfo();
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_01040201_003");
        }
    }

    ///// 作者 趙呂梁
    /// 創建日期：2009/11/03
    /// 修改日期：2009/11/03
    /// <summary>
    /// 錯誤訊息包含“強制執行”執行事件
    /// </summary>
    protected void btnHiden_Click(object sender, EventArgs e)
    {
        Hashtable htInput = GetUploadHtgInfo("Y");
        htInput["MESSAGE_TYPE"] = "5555";
        UploadHtgInfo(htInput);     
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

    /// <summary>
    /// 設置lable文字顏色
    /// </summary>
    private void SetLableForeColor()
    {
        lblBusinessNameText.ForeColor = Color.Black;
        lblOpermanText.ForeColor = Color.Black;
        lblOperIDText.ForeColor = Color.Black;
        lblOperTelText1.ForeColor = Color.Black;
        lblOperTelText2.ForeColor = Color.Black;
        lblOperTelText3.ForeColor = Color.Black;
        lblOperChangeDateText.ForeColor = Color.Black;
        lblOperFlagText.ForeColor = Color.Black;
        lblOperBirthdayText.ForeColor = Color.Black;
        lblOperAtText.ForeColor = Color.Black;
        lblBusinessAddrText1.ForeColor = Color.Black;
        lblBusinessAddrText2.ForeColor = Color.Black;
        lblBusinessAddrText3.ForeColor = Color.Black;
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
    /// 創建日期：2009/11/03
    /// 修改日期：2009/11/04
    /// <summary>
    /// 為網頁中的欄位賦值
    /// </summary>
    /// <param name="eShopBasic">EntitySHOP_BASIC</param>
    private void SetValues(EntitySHOP_BASIC eShopBasic)
    {
        #region
        this.txtCardNo1.Text = eShopBasic.uni_no1;
        this.txtCardNo2.Text = eShopBasic.uni_no2;
        this.txtBank.Text = eShopBasic.bank;
        this.txtBookAddr1.Text = eShopBasic.book_addr1;
        this.txtBookAddr2.Text = eShopBasic.book_addr2;
        this.txtBookAddr3.Text = eShopBasic.book_addr3;
        this.txtBoss.Text = eShopBasic.boss_1;
        this.txtBossID.Text = eShopBasic.boss_id;
        this.txtBranchBank.Text = eShopBasic.branch_bank;
        this.txtBusinessAddr4.Text = eShopBasic.business_addr1;
        this.txtBusinessAddr5.Text = eShopBasic.business_addr2;
        this.txtBusinessAddr6.Text = eShopBasic.business_addr3;
        this.txtBusinessName.Text = eShopBasic.business_name;
        this.txtContactMan.Text = eShopBasic.contact_man;
        this.txtContactManTel1.Text = eShopBasic.contact_tel1;
        this.txtContactManTel2.Text = eShopBasic.contact_tel2;
        this.txtContactManTel3.Text = eShopBasic.contact_tel3;
        this.txtInvoiceCycle.Text = eShopBasic.invoice_cycle;
        this.txtName.Text = eShopBasic.name;
        this.txtOperID.Text = eShopBasic.oper_id;
        this.txtOperTel1.Text = eShopBasic.oper_tel1;
        this.txtOperTel2.Text = eShopBasic.oper_tel2;
        this.txtOperTel3.Text = eShopBasic.oper_tel3;
        this.txtOperman.Text = eShopBasic.operman;
        this.txtReceiveNumber.Text = eShopBasic.recv_no;
        this.txtRegAddr1.Text = eShopBasic.reg_addr1;
        this.txtRegAddr2.Text = eShopBasic.reg_addr2;
        this.txtRegAddr3.Text = eShopBasic.reg_addr3;
        this.txtRegName.Text = eShopBasic.reg_name;
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

    /// 作者 趙呂梁
    /// 創建日期：2009/11/03
    /// 修改日期：2009/11/04
    /// <summary>
    /// 為網頁中的欄位賦值一KEY值并Disabled
    /// </summary>
    /// <param name="eShopBasic">EntitySHOP_BASIC</param>
    private void SetOneKeyValues(EntitySHOP_BASIC eShopBasic)
    {
        this.txtEstablish.Text = eShopBasic.establish;//*設立
        this.txtCapital.Text = eShopBasic.capital;//*資本
        this.txtCheckMan.Text = eShopBasic.checkman;//*徽信員
        this.txtOrganization.Text = eShopBasic.organization;//*組織
        this.txtRisk.Text = eShopBasic.risk;//*風險
        this.txtBossChangeDate.Text = eShopBasic.boss_change_date;//*負責人領換補日
        this.txtBossBirthday.Text = eShopBasic.boss_birthday;//*負責人生日
        this.txtBossFlag.Text = eShopBasic.boss_change_flag;//*負責人代號
        this.txtBossAt.Text = eShopBasic.boss_at;//*負責人換證點
        this.txtBossTel1.Text = eShopBasic.boss_tel1;//*負責人電話1
        this.txtBossTel2.Text = eShopBasic.boss_tel2;//*負責人電話2
        this.txtBossTel3.Text = eShopBasic.boss_te3;//*負責人電話3
        this.txtFax1.Text = eShopBasic.fax1;//*聯絡人傳真1
        this.txtFax2.Text = eShopBasic.fax2;//*聯絡人傳真2
        this.txtPopMan.Text = eShopBasic.pop_man;//*推廣員

        this.txtEstablish.Enabled = false;//*設立
        this.txtCapital.Enabled = false;//*資本
        this.txtCheckMan.Enabled = false;//*徽信員
        this.txtOrganization.Enabled = false;//*組織
        this.txtRisk.Enabled = false;//*風險
        this.txtBossChangeDate.Enabled = false;//*負責人領換補日
        this.txtBossBirthday.Enabled = false;//*負責人生日
        this.txtBossFlag.Enabled = false;//*負責人代號
        this.txtBossAt.Enabled = false;//*負責人換證點
        this.txtBossTel1.Enabled = false;//*負責人電話1
        this.txtBossTel2.Enabled = false;//*負責人電話2
        this.txtBossTel3.Enabled = false;//*負責人電話3
        this.txtFax1.Enabled = false;//*聯絡人傳真1
        this.txtFax2.Enabled = false;//*聯絡人傳真2
        this.txtPopMan.Enabled = false;//*推廣員
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
        int intCount = 0;//*記錄不相同的數量

        if (ViewState["EntityShopBasic"] != null)
        {                
            EntitySHOP_BASIC eShopBasic = (EntitySHOP_BASIC)ViewState["EntityShopBasic"];

            CompareValue(txtReceiveNumber, eShopBasic.recv_no, ref blnSame, ref intCount);//*收件編號
            CompareValue(txtCheckMan, eShopBasic.checkman, ref blnSame, ref intCount);//*徵信員
            CompareValue(txtEstablish, eShopBasic.establish, ref blnSame, ref intCount);//*設立
            CompareValue(txtCapital, eShopBasic.capital, ref blnSame, ref intCount);//*資本
            CompareValue(txtOrganization, eShopBasic.organization, ref blnSame, ref intCount);//*組織
            CompareValue(txtRisk, eShopBasic.risk, ref blnSame, ref intCount);//*風險
            CompareValue(txtRegName, eShopBasic.reg_name, ref blnSame, ref intCount);//*登記名稱

            //*營業名稱
            if (!chkBusinessName.Checked)
            {
                CompareValue(txtBusinessName, eShopBasic.business_name, ref blnSame, ref intCount);
            }
            else
            {
                CompareValue(lblBusinessNameText, eShopBasic.business_name, ref blnSame);
            }

            CompareValue(txtBoss, eShopBasic.boss_1, ref blnSame, ref intCount);//*負責人姓名
            CompareValueToUpper(txtBossID, eShopBasic.boss_id, ref blnSame, ref intCount);//*負責人ID
            CompareValue(txtBossTel1, eShopBasic.boss_tel1, ref blnSame, ref intCount);//*負責人電話(1)
            CompareValue(txtBossTel2, eShopBasic.boss_tel2, ref blnSame, ref intCount);//*負責人電話(2)
            CompareValue(txtBossTel3, eShopBasic.boss_te3, ref blnSame, ref intCount);//*負責人電話(3)
            CompareValue(txtBossChangeDate, eShopBasic.boss_change_date, ref blnSame, ref intCount);//*負責人領換補日
            CompareValue(txtBossFlag, eShopBasic.boss_change_flag, ref blnSame, ref intCount);//*負責人代號
            CompareValue(txtBossBirthday, eShopBasic.boss_birthday, ref blnSame, ref intCount);//*負責人生日
            CompareValue(txtBossAt, eShopBasic.boss_at, ref blnSame, ref intCount);//*負責人換證點
            CompareValue(txtRegAddr1, eShopBasic.reg_addr1, ref blnSame, ref intCount);//*戶籍地址1
            CompareValue(txtRegAddr2, eShopBasic.reg_addr2, ref blnSame, ref intCount);//*戶籍地址2
            CompareValue(txtRegAddr3, eShopBasic.reg_addr3, ref blnSame, ref intCount);//*戶籍地址3

            //*實際經營者相關資料
            if (!chkOper.Checked)
            {
                CompareValue(txtOperman, eShopBasic.operman, ref blnSame, ref intCount);//*實際經營者
                CompareValueToUpper(txtOperID, eShopBasic.oper_id, ref blnSame, ref intCount);//*實際經營者ID
                CompareValue(txtOperTel1, eShopBasic.oper_tel1, ref blnSame, ref intCount);//*實際經營者電話1
                CompareValue(txtOperTel2, eShopBasic.oper_tel2, ref blnSame, ref intCount);//*實際經營者電話2
                CompareValue(txtOperTel3, eShopBasic.oper_tel3, ref blnSame, ref intCount);//*實際經營者電話3
                CompareValue(txtOperChangeDate, eShopBasic.oper_change_date, ref blnSame, ref intCount);//*實際經營者領換補日
                CompareValue(txtOperFlag, eShopBasic.oper_change_flag, ref blnSame, ref intCount);//*實際經營者代號
                CompareValue(txtOperBirthday, eShopBasic.oper_birthday, ref blnSame, ref intCount);//*實際經營者生日
                CompareValue(txtOperAt, eShopBasic.oper_at, ref blnSame, ref intCount); //*實際經營者換證點            
            }
            else
            {
                CompareValue(lblOpermanText, eShopBasic.operman, ref blnSame);//*實際經營者
                CompareValueToUpper(lblOperIDText, eShopBasic.oper_id, ref blnSame);//*實際經營者ID
                CompareValue(lblOperTelText1, eShopBasic.oper_tel1, ref blnSame);//*實際經營者電話1
                CompareValue(lblOperTelText2, eShopBasic.oper_tel2, ref blnSame);//*實際經營者電話2
                CompareValue(lblOperTelText3, eShopBasic.oper_tel3, ref blnSame);//*實際經營者電話3
                CompareValue(lblOperChangeDateText, eShopBasic.oper_change_date, ref blnSame);//*實際經營者領換補日
                CompareValue(lblOperFlagText, eShopBasic.oper_change_flag, ref blnSame);//*實際經營者代號
                CompareValue(lblOperBirthdayText, eShopBasic.oper_birthday, ref blnSame);//*實際經營者生日
                CompareValue(lblOperAtText, eShopBasic.oper_at, ref blnSame);//*實際經營者換證點 
            }

            CompareValue(txtContactMan, eShopBasic.contact_man, ref blnSame, ref intCount);//*聯絡人姓名
            CompareValue(txtContactManTel1, eShopBasic.contact_tel1, ref blnSame, ref intCount);//*聯絡人電話1
            CompareValue(txtContactManTel2, eShopBasic.contact_tel2, ref blnSame, ref intCount);//*聯絡人電話2
            CompareValue(txtContactManTel3, eShopBasic.contact_tel3, ref blnSame, ref intCount);//*聯絡人電話3
            CompareValue(txtFax1, eShopBasic.fax1, ref blnSame, ref intCount);//*聯絡人傳真1
            CompareValue(txtFax2, eShopBasic.fax2, ref blnSame, ref intCount);//*聯絡人傳真2
            CompareValue(txtBookAddr1, eShopBasic.book_addr1, ref blnSame, ref intCount);//*登記地址1
            CompareValue(txtBookAddr2, eShopBasic.book_addr2, ref blnSame, ref intCount);//*登記地址2
            CompareValue(txtBookAddr3, eShopBasic.book_addr3, ref blnSame, ref intCount);//*登記地址3

            //*營業地址
            if (!chkAddress.Checked)
            {
                CompareValue(lblZipText, eShopBasic.zip, ref blnSame);//*營業地郵遞區號
                CompareValue(txtBusinessAddr4, eShopBasic.business_addr1, ref blnSame, ref intCount);//*營業地址1
                CompareValue(txtBusinessAddr5, eShopBasic.business_addr2, ref blnSame, ref intCount);//*營業地址2
                CompareValue(txtBusinessAddr6, eShopBasic.business_addr3, ref blnSame, ref intCount);//*營業地址3
            }
            else
            {
                CompareValue(lblBusinessZipText, eShopBasic.zip, ref blnSame);//*營業地郵遞區號
                CompareValue(lblBusinessAddrText1, eShopBasic.business_addr1, ref blnSame);//*營業地址1
                CompareValue(lblBusinessAddrText2, eShopBasic.business_addr2, ref blnSame);//*營業地址2
                CompareValue(lblBusinessAddrText3, eShopBasic.business_addr3, ref blnSame);//*營業地址3
            }

            CompareValueToUpper(txtJCIC, eShopBasic.jcic_code, ref blnSame, ref intCount);//*JCIC查詢
            CompareValue(txtBank, eShopBasic.bank, ref blnSame, ref intCount);//*銀行
            CompareValue(txtBranchBank, eShopBasic.branch_bank, ref blnSame, ref intCount);//*分行
            CompareValue(txtName, eShopBasic.name, ref blnSame, ref intCount);//*戶名
            CompareValue(txtPrevDesc, eShopBasic.prev_desc, ref blnSame, ref intCount);//*帳單內容
            CompareValue(txtInvoiceCycle, eShopBasic.invoice_cycle, ref blnSame, ref intCount);//*發票週期
            CompareValueToUpper(txtRedeemCycle, eShopBasic.redeem_cycle, ref blnSame, ref intCount);//*紅利週期(M/D)
            CompareValue(txtPopMan, eShopBasic.pop_man, ref blnSame, ref intCount);//*推廣員

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
        if (txtBox.Text.Trim() != NullToString(strValue).Trim())
        {
            intCount++;
            if (intCount == 1)
            {
                if (txtBox.Enabled == true)
                {
                    base.sbRegScript.Append(BaseHelper.SetFocus(txtBox.ID));
                }
            }
            txtBox.ForeColor = Color.Red;
            blnSame = false;
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
            txtBox.ForeColor = Color.Red;
            blnSame = false;
        }
    }

    /// <summary>
    /// 比較輸入欄位值與資料庫欄位值
    /// </summary>
    /// <param name="txtBox">Lable</param>
    /// <param name="strValue">資料庫欄位值</param>
    /// <param name="blnSame">是否相同</param>
    private void CompareValue(CustLabel lblText, string strValue, ref bool blnSame)
    {
        if (lblText.Text.Trim() != NullToString(strValue).Trim())
        {
            lblText.ForeColor = Color.Red;
            blnSame = false;
        }
    }

    /// <summary>
    /// 比較輸入欄位值與資料庫欄位值(轉大寫比較)
    /// </summary>
    /// <param name="txtBox">Lable</param>
    /// <param name="strValue">資料庫欄位值</param>
    /// <param name="blnSame">是否相同</param>
    private void CompareValueToUpper(CustLabel lblText, string strValue, ref bool blnSame)
    {
        if (lblText.Text.Trim().ToUpper() != NullToString(strValue.ToUpper()).Trim())
        {
            lblText.ForeColor = Color.Red;
            blnSame = false;
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
    private void UploadHtgInfo()
    {
        if (!UploadHtgInfo(GetUploadHtgInfo("")))//*上傳主機失敗
        {
            //base.strIsShow = "var local = window.parent.location!=window.location?window.parent:window.opener?window.opener.parent:window;local.document.all.clientmsg.style.cursor='hand';local.document.all.clientmsg.innerText='"+ strClientMsg +"';local.document.all.clientmsg.style.display=''; local.document.all.hostmsg.style.cursor='hand';local.document.all.hostmsg.innerText='"+ strHostMsg +"';local.document.all.hostmsg.style.display='';if(confirm('" + strHostMsg + "\\n" + MessageHelper.GetMessage("01_01040201_006") + "')) {$('#btnHiden').click();}";
            string hostUrlString = @"window.parent.postMessage({ func: 'HostMsgShow', data: '" + strHostMsg + "' }, '*');";
            base.sbRegScript.Append(hostUrlString);
            string clientUrlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + strClientMsg + "' }, '*');";
            base.sbRegScript.Append(clientUrlString);
            base.strIsShow = "if(confirm('" + strHostMsg + "\\n" + MessageHelper.GetMessage("01_01040201_006") + "')) {$('#btnHiden').click();}";
        }
    }

    /// <summary>
    /// 上傳主機
    /// </summary>
    /// <param name="htInput">上傳主機信息的HashTable</param>
    /// <returns>true成功，false失敗</returns>
    private bool UploadHtgInfo(Hashtable htInput)
    {
        Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JCGQ, htInput, false, "21", eAgentInfo);

        if (!htResult.Contains("HtgMsg"))
        {
            UpdateData("Y");//*更新二KEY上傳主機標識
            UpdateOneKeyData();//*更新一KEY上傳主機標識
            base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息;
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_039");
            ClearPage(false);
            this.txtCardNo1.Text = "";
            this.txtCardNo2.Text = "";       
            return true;
        }
        else
        {
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
        Hashtable htInput = new Hashtable();
        htInput.Add("FUNCTION_CODE", "A");//*新增
        htInput.Add("CORP_NO", this.txtCardNo1.Text.Trim());//*統一編號1
        htInput.Add("CORP_SEQ", this.txtCardNo2.Text.Trim());//*統一編號2
        htInput.Add("APPL_NO", this.txtReceiveNumber.Text.Trim());//*收件編號
        htInput.Add("BUILD_DATE", this.txtEstablish.Text.Trim());//*設立
        htInput.Add("CREDIT_NO", this.txtCheckMan.Text.Trim());//*徵信員
        htInput.Add("CAPITAL", this.txtCapital.Text.Trim());//*資本
        htInput.Add("REG_NAME", this.txtRegName.Text.Trim());//*登記名稱
        htInput.Add("ORGAN_TYPE", this.txtOrganization.Text.Trim());//*組織

        //*營業名稱
        if (!chkBusinessName.Checked)
        {
            htInput.Add("BUSINESS_NAME", this.txtBusinessName.Text.Trim());
        }
        else
        {
            htInput.Add("BUSINESS_NAME", this.lblBusinessNameText.Text.Trim());
        }
        htInput.Add("RISK_FLAG", this.txtRisk.Text.Trim());//*風險
        htInput.Add("OWNER_NAME", this.txtBoss.Text.Trim());//*負責人
        htInput.Add("OWNER_ID", this.txtBossID.Text.Trim().ToUpper());//*負責人ID
        htInput.Add("OWNER_PHONE_AREA", this.txtBossTel1.Text.Trim());//*負責人電話1
        htInput.Add("OWNER_PHONE_NO", this.txtBossTel2.Text.Trim());//*負責人電話2
        htInput.Add("OWNER_PHONE_EXT", this.txtBossTel3.Text.Trim());//*負責人電話3
        htInput.Add("OWNER_CITY", this.txtRegAddr1.Text.Trim());//*負責人戶籍地址1
        htInput.Add("OWNER_ADDR1", this.txtRegAddr2.Text.Trim());//*負責人戶籍地址2
        htInput.Add("OWNER_ADDR2", this.txtRegAddr3.Text.Trim());//*負責人戶籍地址3
        htInput.Add("CHANGE_DATE1", this.txtBossChangeDate.Text.Trim());//*負責人領換補日
        htInput.Add("BIRTHDAY1", this.txtBossBirthday.Text.Trim());//*負責人生日
        htInput.Add("CHANGE_FLAG1", this.txtBossFlag.Text.Trim());//*負責人代號
        htInput.Add("AT1", this.txtBossAt.Text.Trim());//*負責人換證點

        //*實際經營者相關資料
        if (!chkOper.Checked)
        {
            htInput.Add("MANAGER_NAME", this.txtOperman.Text.Trim());//*實際經營者
            htInput.Add("MANAGER_ID", this.txtOperID.Text.Trim().ToUpper());//*實際經營者ID
            htInput.Add("MANAGER_PHONE_AREA", this.txtOperTel1.Text.Trim());//*實際經營者電話1
            htInput.Add("MANAGER_PHONE_NO", this.txtOperTel2.Text.Trim());//*實際經營者電話2
            htInput.Add("MANAGER_PHONE_EXT", this.txtOperTel3.Text.Trim());//*實際經營者電話3
            htInput.Add("CHANGE_DATE2", this.txtOperChangeDate.Text.Trim());//*實際經營者領換補日
            htInput.Add("BIRTHDAY2", this.txtOperBirthday.Text.Trim());//*實際經營者生日
            htInput.Add("CHANGE_FLAG2", this.txtOperFlag.Text.Trim());//*實際經營者代號
            htInput.Add("AT2", this.txtOperAt.Text.Trim());//*實際經營者換證點
        }
        else
        {
            htInput.Add("MANAGER_ID", this.lblOperIDText.Text.Trim().ToUpper());
            htInput.Add("MANAGER_PHONE_AREA", this.lblOperTelText1.Text.Trim());
            htInput.Add("MANAGER_PHONE_NO", this.lblOperTelText2.Text.Trim());
            htInput.Add("MANAGER_PHONE_EXT", this.lblOperTelText3.Text.Trim());
            htInput.Add("MANAGER_NAME", this.lblOpermanText.Text.Trim());
            htInput.Add("CHANGE_DATE2", this.lblOperChangeDateText.Text.Trim());
            htInput.Add("CHANGE_FLAG2", this.lblOperFlagText.Text.Trim());
            htInput.Add("BIRTHDAY2", this.lblOperBirthdayText.Text.Trim());
            htInput.Add("AT2", this.lblOperAtText.Text.Trim());
        }
        htInput.Add("CONTACT_NAME", this.txtContactMan.Text.Trim());//*聯絡人
        htInput.Add("CONTACT_PHONE_AREA", this.txtContactManTel1.Text.Trim());//*聯絡人電話1
        htInput.Add("CONTACT_PHONE_NO", this.txtContactManTel2.Text.Trim());//*聯絡人電話2
        htInput.Add("CONTACT_PHONE_EXT", this.txtContactManTel3.Text.Trim());//*聯絡人電話3
        htInput.Add("FAX_AREA", this.txtFax1.Text.Trim());//*聯絡人傳真1
        htInput.Add("FAX_PHONE_NO", this.txtFax2.Text.Trim());//*聯絡人傳真2
        htInput.Add("REG_CITY", this.txtBookAddr1.Text.Trim());//*登記地址1
        htInput.Add("REG_ADDR1", this.txtBookAddr2.Text.Trim());//*登記地址2
        htInput.Add("REG_ADDR2", this.txtBookAddr3.Text.Trim());//*登記地址3

        //*營業地址
        if (!chkAddress.Checked)
        {
            htInput.Add("REAL_ZIP", this.lblZipText.Text.Trim());//*營業地郵遞區號
            htInput.Add("REAL_CITY", this.txtBusinessAddr4.Text.Trim());//*營業地址1
            htInput.Add("REAL_ADDR1", this.txtBusinessAddr5.Text.Trim());//*營業地址2
            htInput.Add("REAL_ADDR2", this.txtBusinessAddr6.Text.Trim());//*營業地址3
        }
        else
        {
            htInput.Add("REAL_ZIP", this.lblBusinessZipText.Text.Trim());
            htInput.Add("REAL_CITY", this.lblBusinessAddrText1.Text.Trim());
            htInput.Add("REAL_ADDR1", this.lblBusinessAddrText2.Text.Trim());
            htInput.Add("REAL_ADDR2", this.lblBusinessAddrText3.Text.Trim());
        }

        htInput.Add("DDA_BANK_NAME", this.txtBank.Text.Trim());//*銀行
        htInput.Add("DDA_BANK_BRANCH", this.txtBranchBank.Text.Trim());//*分行別
        htInput.Add("DDA_ACCT_NAME", this.txtName.Text.Trim());//*戶名
        htInput.Add("SALE_NAME", this.txtPopMan.Text.Trim());//*推廣員
        htInput.Add("INVOICE_CYCLE", this.txtInvoiceCycle.Text.Trim());//*發票週期
        htInput.Add("FORCE_FLAG", strSend3270);//*強制

        htInput.Add("JCIC_CODE", this.txtJCIC.Text.Trim().ToUpper());//*JCIC查詢
        htInput.Add("IPMR_PREV_DESC", this.txtPrevDesc.Text.Trim());//*帳單內容
        htInput.Add("REDEEM_CYCLE", this.txtRedeemCycle.Text.Trim().ToUpper());//*紅利週期(M/D)
     
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
        EntitySHOP_BASIC eShopBasic = GetEntity(strSendHostFlag);
        try
        {
            return BRSHOP_BASIC.Update(eShopBasic, this.txtCardNo1.Text.Trim(), this.txtCardNo2.Text.Trim(), "","2","N");
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
        //EntitySHOP_BASIC eShopBasicOneKey = (EntitySHOP_BASIC)ViewState["EntityShopBasic"];//*一KEY資料
        EntitySHOP_BASIC eShopBasic = new EntitySHOP_BASIC();
        eShopBasic.sendhost_flag = "Y";
        string[] strFields ={ EntitySHOP_BASIC.M_sendhost_flag};
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
    /// 向資料庫中新增資料
    /// </summary>
    /// <returns>true成功,false失敗</returns>
    private bool AddNewData(string strSendHostFlag)
    {
        EntitySHOP_BASIC eShopBasic = GetEntity(strSendHostFlag);
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
    /// 創建日期：2009/10/29
    /// 修改日期：2009/10/29
    /// <summary>
    /// 得到插入或更新資料的Entity
    /// </summary>
    /// <returns>Entity</returns>
    private EntitySHOP_BASIC GetEntity(string strSendHostFlag)
    {
        #region
        EntitySHOP_BASIC eShopBasic = new EntitySHOP_BASIC();   
        eShopBasic.uni_no1 = this.txtCardNo1.Text.Trim();
        eShopBasic.uni_no2 = this.txtCardNo2.Text.Trim();
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
            eShopBasic.business_addr1 = this.lblBusinessAddrText1.Text.Trim();
            eShopBasic.business_addr2 = this.lblBusinessAddrText2.Text.Trim();
            eShopBasic.business_addr3 = this.lblBusinessAddrText3.Text.Trim();
        }

        //*營業名稱
        if (!chkBusinessName.Checked)
        {
            eShopBasic.business_name = this.txtBusinessName.Text.Trim();
        }
        else
        {
            eShopBasic.business_name = this.lblBusinessNameText.Text.Trim();
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
            eShopBasic.operman = this.lblOpermanText.Text.Trim();
            eShopBasic.oper_change_date = this.lblOperChangeDateText.Text.Trim();
            eShopBasic.oper_change_flag = this.lblOperFlagText.Text.Trim();
            eShopBasic.oper_birthday = this.lblOperBirthdayText.Text.Trim();
            eShopBasic.oper_at = this.lblOperAtText.Text.Trim();
        }


        eShopBasic.organization = this.txtOrganization.Text.Trim();
        eShopBasic.pop_man = this.txtPopMan.Text.Trim();
        eShopBasic.recv_no = this.txtReceiveNumber.Text.Trim();
        eShopBasic.reg_addr1 = this.txtRegAddr1.Text.Trim();
        eShopBasic.reg_addr2 = this.txtRegAddr2.Text.Trim();
        eShopBasic.reg_addr3 = this.txtRegAddr3.Text.Trim();
        eShopBasic.reg_name = this.txtRegName.Text.Trim();
        eShopBasic.risk = this.txtRisk.Text.Trim();
        eShopBasic.sendhost_flag = strSendHostFlag;
        eShopBasic.keyin_flag = "2";
        eShopBasic.prev_desc = this.txtPrevDesc.Text.Trim();
        eShopBasic.redeem_cycle = this.txtRedeemCycle.Text.Trim().ToUpper();
        eShopBasic.jcic_code = this.txtJCIC.Text.Trim().ToUpper();
        return eShopBasic;
        #endregion
    }
    #endregion
}
