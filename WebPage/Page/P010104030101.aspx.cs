//******************************************************************
//*  作    者：趙呂梁

//*  功能說明：特店基本資料修改

//*  創建日期：2009/11/23 
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

public partial class P010104030101 : PageBase
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
            CommonFunction.SetControlsEnabled(pnlText, false);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtCardNo1"));
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/11/23
    /// 修改日期：2009/11/23
    /// <summary>
    /// 更新事件
    /// </summary>
    protected void btnUpdate_Click(object sender, EventArgs e)
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
        Hashtable htReturn = GetHtgInfo();

        if (!htReturn.Contains("HtgMsg"))
        {
            ClearPage(true);
            SetValues(htReturn);
            base.sbRegScript.Append(BaseHelper.SetFocus("txtReceiveNumber"));
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_031");
            base.strHostMsg += htReturn["HtgSuccess"].ToString();//*主機返回成功訊息
            ViewState["HtgInfo"] = htReturn;
        }
        else
        {
            if ((htReturn["MESSAGE_TYPE"]==null)||(htReturn["MESSAGE_TYPE"].ToString().Trim() != "8888"))
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
        #region 欄位賦值
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
        if (ViewState["HtgInfo"] != null)
        {
            Hashtable htReturn = new Hashtable(); 

            CommonFunction.GetViewStateHt(ViewState["HtgInfo"], ref htReturn);

            htReturn["MESSAGE_TYPE"] = "";
            htReturn["MESSAGE_CHI"] = "";

            #region 比對主機資料
            //*收件編號
            CommonFunction.ContrastData(htReturn, this.txtReceiveNumber.Text.Trim(), "APPL_NO");
            //*設立
            CommonFunction.ContrastData(htReturn, this.txtEstablish.Text.Trim(), "BUILD_DATE");
            //*徵信員
            CommonFunction.ContrastData(htReturn, this.txtCheckMan.Text.Trim(), "CREDIT_NO");
            //*資本
            CommonFunction.ContrastData(htReturn, this.txtCapital.Text.Trim(), "CAPITAL");
            //*登記名稱
            CommonFunction.ContrastData(htReturn, this.txtRegName.Text.Trim(), "REG_NAME");
            //*組織
            CommonFunction.ContrastData(htReturn, this.txtOrganization.Text.Trim(), "ORGAN_TYPE");

            //*營業名稱
            if (!chkBusinessName.Checked)
            {
                CommonFunction.ContrastData(htReturn, this.txtBusinessName.Text.Trim(), "BUSINESS_NAME");
            }
            else
            {
                CommonFunction.ContrastData(htReturn, this.lblBusinessNameText.Text.Trim(), "BUSINESS_NAME");
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
            CommonFunction.ContrastData(htReturn, this.txtBossTel3.Text.Trim(), "OWNER_PHONE_EXT");
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
                CommonFunction.ContrastData(htReturn, this.txtOperTel3.Text.Trim(), "MANAGER_PHONE_EXT");
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
            CommonFunction.ContrastData(htReturn, this.txtContactManTel3.Text.Trim(), "CONTACT_PHONE_EXT");
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
                //*營業地郵遞區號
                CommonFunction.ContrastData(htReturn, this.lblBusinessZipText.Text.Trim(), "REAL_ZIP");
                //*營業地址1
                CommonFunction.ContrastData(htReturn, this.lblBusinessAddrText1.Text.Trim(), "REAL_CITY");
                //*營業地址2
                CommonFunction.ContrastData(htReturn, this.lblBusinessAddrText2.Text.Trim(), "REAL_ADDR1");
                //*營業地址3
                CommonFunction.ContrastData(htReturn, this.lblBusinessAddrText3.Text.Trim(), "REAL_ADDR2");
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
            #endregion

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

        //*異動主機資料成功
        if (!htResult.Contains("HtgMsg"))
        {
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
            //if (htResult["MESSAGE_TYPE"].ToString() == "5555")
            //{
                return false;
            //}
            //else
            //{
            //    return true;
            //}
        }
    }
    #endregion

}
