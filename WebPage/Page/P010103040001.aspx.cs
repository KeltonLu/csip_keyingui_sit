//******************************************************************
//*  作    者：陳香琦
//*  功能說明：主機特店基本資料查詢(特店基本資料查核)
//*  創建日期：2019/09/20
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************

using System;
using System.Collections;
using Framework.Common.Message;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.EntityLayer_new;
using System.Data;
using Framework.WebControls;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using CSIPKeyInGUI.EntityLayer;
using CSIPKeyInGUI.BusinessRules_new;
using Framework.Common.Logging;
using System.Text;
using Framework.Data.OM.Collections;
using CSIPKeyInGUI.BusinessRules;

public partial class P010103040001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    //20191023 修改：SOC所需資訊  by Peggy
    private structPageInfo sPageInfo;//*記錄網頁訊息

    /// <summary>
    /// 記錄異動主機成功數
    /// </summary>
    private int m_iCount;
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadGrid();
            base.sbRegScript.Append(BaseHelper.SetFocus("txtUNI_NO1"));//*將【統一編號】(1)設為輸入焦點

            btnSend.Enabled = false;
            btnReject.Enabled = false;
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//20191023 修改：SOC所需資訊  by Peggy

    }

    /// <summary>
    /// 撈取待覆核資料
    /// </summary>
    private void LoadGrid()
    {
        DataSet dstInfo = BRSHOP_CHANGE.GetDataFor304(this.txtUNI_NO1.Text.Trim());
        if (dstInfo != null)
        {
            gdvDataChange.DataSource = dstInfo;
            gdvDataChange.DataBind();
        }
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

        //放行同仁與一KEY/二KEY經辦不能為同一人
        bool isAgentIdDup = BRSHOP_CHANGE.CheckKeyinFlagNotDup(this.txtUNI_NO1.Text.Trim(), eAgentInfo.agent_id.Trim());
        if (isAgentIdDup)
        {
            base.sbRegScript.Append("alert('" + MessageHelper.GetMessage("01_01030400_005") + "');");
            return;
        }

        string strColumns = @"[UNI_NO1],[UNI_NO2],[CORP_NO],[CORP_MCC],[CORP_ESTABLISH],[CORP_Organization],[CORP_CountryCode],[CORP_CountryStateCode],[REG_NAME_CH],[REG_NAME_EN]
                                ,[REG_ZIP_CODE],[REG_CITY],[REG_ADDR1],[REG_ADDR2],[CORP_TEL1],[CORP_TEL2],[CORP_TEL3],[PrincipalNameCH],[PrincipalName_L],[PrincipalName_PINYIN],[PrincipalNameEN]
                                ,[PrincipalIDNo],[PrincipalBirth],[PrincipalIssueDate],[PrincipalIssuePlace],[PrincipalReplaceType],[PrincipalCountryCode]
                                ,[PrincipalPassportNo],[PrincipalPassportExpdt],[PrincipalResidentNo],[PrincipalResidentExpdt],[Principal_TEL1],[Principal_TEL2],[Principal_TEL3]
                                ,[HouseholdCITY],[HouseholdADDR1],[HouseholdADDR2],[ARCHIVE_NO],[DOC_ID],[LAST_UPD_MAKER],[LAST_UPD_CHECKER],[LAST_UPD_BRANCH] ";
        string[] aa = strColumns.Split(',');
        SelectData(this.pnlText, strColumns);
        base.sbRegScript.Append(BaseHelper.SetFocus("txtUNI_NO1"));

        if (!txtPrincipalName_L.Text.Trim().Equals(""))
        {
            chkisLongName2.Checked = true;
        }

        btnSend.Enabled = true;//20191022 待主機電文上線後再open
        btnReject.Enabled = true;
    }

    /// <summary>
    /// 查詢一KEY資料信息
    /// </summary>
    /// <param name="pnlType">所在panel</param>
    /// <param name="strShopType">功能畫面編號</param>
    /// <param name="strColumns">查詢的列名</param>
    private void SelectData(CustPanel pnlType, string strColumns)
    {
        DataSet dstInfo = BRSHOP_CHANGE.Select(this.txtUNI_NO1.Text.Trim(), "2", strColumns, "Y");

        if (dstInfo == null)
        {
            base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            return;
        }

        if (dstInfo.Tables[0].Rows.Count > 0)
        {
            // 查詢的欄位必須和網頁中的欄位相匹配
            // 如若網頁中要顯示10各欄位，則查詢的前10各欄位必須和網頁中的欄位對應
            CommonFunction.SetControlsForeColor(pnlType, System.Drawing.Color.Black);
            SetTextBoxValue(pnlType, dstInfo.Tables[0]);
        }
        else
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_001");
        }
    }

    /// <summary>
    /// 設置TextBox的值
    /// </summary>
    /// <param name="pnlType">所在的PANEL</param>
    /// <param name="dtInfo">表信息</param>
    private void SetTextBoxValue(CustPanel pnlType, DataTable dtInfo)
    {
        int intIndex = 0;// index for textbox
        int intCount = dtInfo.Columns.Count;// count for items from datatable

        foreach (System.Web.UI.Control control in pnlType.Controls)
        {
            if (control is CustTextBox)
            {
                CustTextBox txtBox = (CustTextBox)control;

                txtBox.Text = dtInfo.Rows[0][intIndex].ToString().Trim();
                intIndex++;

                if (intIndex >= intCount)
                    break;
            }
        }
    }


    protected void gdvDataChange_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
        if (e.CommandName == "SELECT")
        {
            int index = Convert.ToInt32(e.CommandArgument);

            GridViewRow row = gdvDataChange.Rows[index];

            txtUNI_NO1.Text = row.Cells[1].Text;
            btnSelect_Click(null, null);
        }
    }

    /// <summary>
    /// 退件事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReject_Click(object sender, EventArgs e)
    {
        try
        {
            BRSHOP_CHANGE.UpdateShopChangeFlag(txtUNI_NO1.Text.Trim(), "1", "");
            BRSHOP_CHANGE.UpdateShopChangeFlag(txtUNI_NO1.Text.Trim(), "2", "");

            ClearAll();
            LoadGrid();

            base.strClientMsg += MessageHelper.GetMessage("01_01030400_001");
        }
        catch (Exception ex)
        {
            base.strClientMsg += MessageHelper.GetMessage("01_00000000_021");
            Logging.Log(ex, LogLayer.BusinessRule);
        }
    }

    /// <summary>
    /// 放行事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSend_Click(object sender, EventArgs e)
    {
        #region 為EntityShopChange_LOG賦值
        CSIPKeyInGUI.EntityLayer_new.EntityShopChange_LOG eChangeLog = new CSIPKeyInGUI.EntityLayer_new.EntityShopChange_LOG();
        eChangeLog.DOC_ID = txtDOC_ID.Text.Trim();
        eChangeLog.CORP_NO = txtUNI_NO1.Text.Trim();
        eChangeLog.CORP_SEQ = "0000";
        eChangeLog.KeyinFLAG = "3";
        eChangeLog.MOD_USER = eAgentInfo.agent_id;
        eChangeLog.MOD_DATE = DateTime.Now.ToString("yyyyMMdd");
        #endregion
        BRShopChange_LOG.Insert(eChangeLog, txtDOC_ID.Text.Trim(), "3");
        string _ErrorMsg = string.Empty;
        if (UpdateMainFrameData(ref _ErrorMsg))
        {
            if (m_iCount > 0)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_00000000_046");//20210111-RQ-2020-021027-000 修改顯示資訊
            }

            ClearAll();
            LoadGrid();
        }
        else
        {
            base.strClientMsg += _ErrorMsg;
            CommonFunction.SetEnabled(pnlText, false);//*將網頁中的提交按鈕和輸入框disable 
            btnSend.Enabled = true;//20191022 待主機電文上線後再open
            btnReject.Enabled = true;
            base.sbRegScript.Append(BaseHelper.SetFocus("txtUNI_NO1"));
        }
        sbRegScript.Append(@"alert('" + base.strClientMsg + "');");
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/07/24
    /// 修改日期：2009/07/24 
    /// <summary>
    /// 更新主機信息
    /// </summary>
    /// <param name="strMsg">提示信息</param>
    /// <returns>true 成功，false失敗</returns>
    private bool UpdateMainFrameData(ref string _ErrorMsg)
    {
        m_iCount = 0;

        if (UpdateBasicP4A_JC69(ref _ErrorMsg))
        {
            //將UpdateBasicP4A_JC69中的特店實體刪除移至此處
            CSIPKeyInGUI.EntityLayer_new.EntitySHOP_CHANGE eShopChange = new CSIPKeyInGUI.EntityLayer_new.EntitySHOP_CHANGE();
            CSIPKeyInGUI.BusinessRules_new.BRSHOP_CHANGE.Delete(eShopChange, this.txtUNI_NO1.Text.Trim(), "1");
            CSIPKeyInGUI.BusinessRules_new.BRSHOP_CHANGE.Delete(eShopChange, this.txtUNI_NO1.Text.Trim(), "2");
        }
        else
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 清空、禁用異動區域所有欄位，并設置商店代號為焦點
    /// </summary>
    private void ClearAll()
    {
        CommonFunction.SetControlsEnabled(this.pnlText, false);//基本資料by統編部份disable//20190912 add by Peggy

        this.txtUNI_NO1.Text = "";
        this.txtUNI_NO2.Text = "";
        this.chkisLongName2.Checked = false;

        base.sbRegScript.Append(BaseHelper.SetFocus("txtUNI_NO1"));
    }

    /// <summary>
    /// 更新基本資料EXMS_P4A主機資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool UpdateBasicP4A_JC69(ref string _ErrorMsg)
    {
        //鍵檔GUI訊息呈現方式
        etMstType = eMstType.Control;

        Hashtable htInput = new Hashtable();
        htInput.Add("FUNCTION_CODE", "I");//*查詢
        htInput.Add("CORP_NO", this.txtUNI_NO1.Text.Trim());//*統一編號1
        htInput.Add("CORP_SEQ", "0000");//*統一編號2

        Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC69, htInput, false, "11", eAgentInfo);

        if (!htReturn.Contains("HtgMsg"))
        {
            ViewState["HtgInfo_P4A_JC69"] = htReturn;
            //20210111-RQ-2020-021027-000-判斷原始電文之總公司統編不得為空白
            if ((string.IsNullOrEmpty(htReturn["HEAD_CORP_NO"].ToString()) || htReturn["HEAD_CORP_NO"].ToString().Trim().Length < 8) && txtCORP_NO.Text.Trim().Equals(""))
            {
                _ErrorMsg = "【原始資料】總公司統編空白，請確認";
                return false;
            }
        }
        else
        {
            base.strClientMsg += htReturn["HtgMsg"].ToString();
            return false;
        }

        Hashtable htP4A_JC69 = new Hashtable();
        string mLongName = string.Empty;

        CommonFunction.GetViewStateHt(ViewState["HtgInfo_P4A_JC69"], ref htP4A_JC69);

        // 20210527 EOS_AML(NOVA) by Ares Dennis start
        Hashtable htP4A_JC692 = new Hashtable();
        CommonFunction.GetViewStateHt(ViewState["HtgInfo_P4A_JC69"], ref htP4A_JC692);// 主機資料
        // 20210527 EOS_AML(NOVA) by Ares Dennis end

        DataTable dtblUpdateData = CommonFunction.GetDataTable();
        DataTable dtblUpdateDataJC68 = CommonFunction.GetDataTable();//記錄長姓名的異動記錄

        if (!string.IsNullOrEmpty(htP4A_JC69["LONG_NAME_FLAG"].ToString().Trim()))//記錄電文負責人狀態
        {
            mLongName = htP4A_JC69["LONG_NAME_FLAG"].ToString().Trim();
        }
        //20191015 Talas 修正電文欄位過長，寫入異動前(varchar(100))會造成截斷 後續如有補空白欄位，請並於下方修改

        htP4A_JC69["REG_NAME"] = htP4A_JC69["REG_NAME"].ToString().Trim();  //公司登記名稱

        CompareExmsHtgValue(ref htP4A_JC69, ref dtblUpdateData);
        //20211222_Ares_Jack_ 比對生日, 比對發證日期
        //檢核生日、發證日期，有值才帶入畫面資料做比對 by Ares Stanley 20220312
        bool isChangedBIRTH_DATE = false;
        bool isChangedID_ISSUE_DATE = false;
        Hashtable htInputDate = new Hashtable();
        if (!string.IsNullOrEmpty(this.txtPrincipalBirth.Text))
        {
            htInputDate.Add("OWNER_BIRTH_DATE", ConvertToDC(this.txtPrincipalBirth.Text));
            compareForAMLCheckLog(htReturn, htInputDate, "OWNER_BIRTH_DATE", ref isChangedBIRTH_DATE);// 生日
        }

        if (!string.IsNullOrEmpty(this.txtPrincipalIssueDate.Text))
        {
            htInputDate.Add("OWNER_ID_ISSUE_DATE", ConvertToDC(this.txtPrincipalIssueDate.Text));
            compareForAMLCheckLog(htReturn, htInputDate, "OWNER_ID_ISSUE_DATE", ref isChangedID_ISSUE_DATE);// 身分證發證日期
        }

        //int count = 0;
        string before = "";
        string after = "";

        #region 20190806-RQ-2019-008595-002-長姓名需求，長姓名異動欄位比對 by Peggy
        EntityHTG_JC68 htReturn_JC68 = new EntityHTG_JC68();
        if (!txtPrincipalNameCH.Text.Trim().Equals("") || !txtPrincipalName_L.Text.Trim().Equals(""))
        {
            htReturn_JC68 = new EntityHTG_JC68();
            htReturn_JC68 = GetJC68(txtPrincipalIDNo.Text.Trim());
            before = htReturn_JC68.LONG_NAME.Trim();//電文
            after = txtPrincipalName_L.Text.Trim();//畫面
            if (before != after)
            {
                DataRow drowRow = dtblUpdateDataJC68.NewRow();
                drowRow[EntityCUSTOMER_LOG.M_field_name] = "負責人中文長姓名";
                drowRow[EntityCUSTOMER_LOG.M_before] = htReturn_JC68.LONG_NAME.Trim();
                drowRow[EntityCUSTOMER_LOG.M_after] = txtPrincipalName_L.Text.Trim();
                dtblUpdateDataJC68.Rows.Add(drowRow);
            }

            before = htReturn_JC68.PINYIN_NAME.Trim();//電文
            after = LongNameRomaClean(txtPrincipalName_PINYIN.Text.Trim());//畫面
            if (before != after)
            {
                DataRow drowRow = dtblUpdateDataJC68.NewRow();
                drowRow[EntityCUSTOMER_LOG.M_field_name] = "負責人羅馬拼音";
                drowRow[EntityCUSTOMER_LOG.M_before] = htReturn_JC68.PINYIN_NAME.Trim();
                drowRow[EntityCUSTOMER_LOG.M_after] = after;
                dtblUpdateDataJC68.Rows.Add(drowRow);
            }
        }
        #endregion

        if (dtblUpdateData.Rows.Count > 0 || dtblUpdateDataJC68.Rows.Count > 0 || isChangedBIRTH_DATE == true || isChangedID_ISSUE_DATE == true)
        {
            //更新主機資料
            htP4A_JC69["FUNCTION_CODE"] = "C";
            htP4A_JC69["MESSAGE_TYPE"] = "";
            htP4A_JC69["MESSAGE_CHI"] = "";
            //新增檢核，生日或發證日要有異動才帶畫面資料，否則帶入原主機資料 by Ares Stanley 20220312
            if (isChangedBIRTH_DATE == false)
            {
                htP4A_JC69["OWNER_BIRTH_DATE"] = ConvertToDC(htP4A_JC692["OWNER_BIRTH_DATE"].ToString());//主機原西元生日
            }
            else
            {
                htP4A_JC69["OWNER_BIRTH_DATE"] = ConvertToDC(this.txtPrincipalBirth.Text.Trim());//西元生日
            }

            if (isChangedID_ISSUE_DATE == false)
            {
                htP4A_JC69["OWNER_ID_ISSUE_DATE"] = ConvertToDC(htP4A_JC692["OWNER_ID_ISSUE_DATE"].ToString());//主機原西元發證日期
            }
            else
            {
                htP4A_JC69["OWNER_ID_ISSUE_DATE"] = ConvertToDC(this.txtPrincipalIssueDate.Text.Trim());//西元發證日期
            }
            

            Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC69, htP4A_JC69, false, "21", eAgentInfo);
            //20210824 EOS_AML(NOVA) by Ares Jack 若查無主機資料(新的一筆資料)也需比對異動欄位並紀錄到DB
            if (!htResult.Contains("HtgMsg") || (htResult["MESSAGE_TYPE"] != null && htResult["MESSAGE_TYPE"].ToString().Trim() == "0124"))
            {
                m_iCount++;
                base.strHostMsg += htResult["HtgSuccess"].ToString();//*主機返回成功訊息
                base.strClientMsg += MessageHelper.GetMessage("01_01030400_002");

                //20211222_Ares_Jack_ 生日, 發證日期 customerLog轉民國年
                if (isChangedBIRTH_DATE == true)
                {
                    CommonFunction.UpdateLog(ConvertToROCYear(htReturn["OWNER_BIRTH_DATE"].ToString()), this.txtPrincipalBirth.Text.Trim(), "生日", dtblUpdateData);
                }
                if (isChangedID_ISSUE_DATE == true)
                {
                    CommonFunction.UpdateLog(ConvertToROCYear(htReturn["OWNER_ID_ISSUE_DATE"].ToString()), this.txtPrincipalIssueDate.Text.Trim(), "發證日期", dtblUpdateData);
                }

                if (!CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, this.txtUNI_NO1.Text.Trim(), BaseHelper.GetShowText("01_01030200_089").Trim(), (structPageInfo)Session["PageInfo"]))
                {
                    if (base.strClientMsg.IndexOf(MessageHelper.GetMessage("00_00000000_000")) < 0)
                    {
                        base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
                    }
                }

                #region 20190806-RQ-2019-008595-002-長姓名需求
                //如有長姓名，更新JC68資料 by Peggy
                List<EntityHTG_JC68> _JC68s = new List<EntityHTG_JC68>();
                EntityHTG_JC68 _tmpJC68 = new EntityHTG_JC68();

                //當姓名或長姓名有異動時 打JC68電文
                if (!txtPrincipalNameCH.Text.Trim().Equals("") || !txtPrincipalName_L.Text.Trim().Equals(""))
                {
                    if (chkisLongName2.Checked)
                    {
                        _tmpJC68 = new EntityHTG_JC68();
                        _tmpJC68.ID = txtPrincipalIDNo.Text.Trim();
                        _tmpJC68.LONG_NAME = ToWide(txtPrincipalName_L.Text.Trim());
                        _tmpJC68.PINYIN_NAME = LongNameRomaClean(txtPrincipalName_PINYIN.Text.Trim());

                        _JC68s.Add(_tmpJC68);
                    }
                    else//取消長姓名
                    {
                        if (mLongName.Trim().Equals("Y"))
                        {
                            _tmpJC68 = new EntityHTG_JC68();
                            _tmpJC68.ID = txtPrincipalIDNo.Text.Trim();
                            _tmpJC68.LONG_NAME = "";
                            _tmpJC68.PINYIN_NAME = "";

                            _JC68s.Add(_tmpJC68);
                        }
                    }
                }

                using (BRHTG_JC68 obj = new BRHTG_JC68("P010103040001"))
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

                if (!CommonFunction.InsertCustomerLog(dtblUpdateDataJC68, eAgentInfo, this.txtUNI_NO1.Text.Trim(), BaseHelper.GetShowText("01_01030200_089").Trim(), (structPageInfo)Session["PageInfo"]))
                {
                    base.strClientMsg += "更新長姓名異動記錄查詢LOG失敗";
                }

                #endregion

                // 20210527 EOS_AML(NOVA) by Ares Dennis
                #region 異動記錄需報送AML                
                bool isChanged = false;

                #region 檢核欄位是否異動
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "HEAD_CORP_NO", ref isChanged);// 總公司統一編號
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "CC", ref isChanged);// AML行業編號
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "BUSINESS_ORGAN_TYPE", ref isChanged);// 法律形式
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "BUILD_DATE", ref isChanged);// 設立 
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "REGISTER_NATION", ref isChanged);// 註冊國籍
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "REGISTER_US_STATE", ref isChanged);// 註冊國籍為美國者，請勾選州別
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "REG_NAME", ref isChanged);// 中文登記名稱
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "CORP_REG_ENG_NAME", ref isChanged);// 英文登記名稱
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "REG_CITY", ref isChanged);// 商店登記地址
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "REG_ADDR1", ref isChanged);// 商店登記地址
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "REG_ADDR2", ref isChanged);// 商店登記地址
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "TEL", ref isChanged);// 登記電話
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "OWNER_CHINESE_NAME", ref isChanged);// 負責人長姓名
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "OWNER_ENGLISH_NAME", ref isChanged);// 負責人英文名
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "OWNER_ID", ref isChanged);// 負責人ID
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "OWNER_BIRTH_DATE", ref isChanged);// 生日                    
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "OWNER_NATION", ref isChanged);// 國籍
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "PASSPORT", ref isChanged);// 護照號碼
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "PASSPORT_EXP_DATE", ref isChanged);// 護照效期
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "RESIDENT_NO", ref isChanged);// 統一證號
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "RESIDENT_EXP_DATE", ref isChanged);// 統一證號效期
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "OWNER_TEL", ref isChanged);// 負責人電話
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "OWNER_CITY", ref isChanged);// 負責人戶籍地址
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "OWNER_ADDR1", ref isChanged);// 負責人戶籍地址
                compareForAMLCheckLog(htP4A_JC692, htP4A_JC69, "OWNER_ADDR2", ref isChanged);// 負責人戶籍地址                                        
                #endregion
                if (isChanged)
                {
                    EntityAML_CHECKLOG eAMLCheckLog = new EntityAML_CHECKLOG();
                    eAMLCheckLog.CORP_NO = htP4A_JC69["CORP_NO"].ToString().Trim();
                    eAMLCheckLog.TRANS_ID = "CSIPJC69";
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
                    base.strClientMsg += HtgType.P4A_JC69.ToString() + MessageHelper.GetMessage("01_00000000_011");
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
            base.strClientMsg += MessageHelper.GetMessage("01_01030400_006"); //20210517_Ares_Stanley-調整主機已存在相同資料的訊息代碼;20210521_Ares_Stanley-調整主機已存在相同資料的訊息代碼
            return true;
        }
    }

    /// <summary>
    /// 比較EXMS主機資料
    /// </summary>
    /// <param name="htOutput">主機資料Hashtable</param>
    /// <param name="dtblUpdateData">修改信息Datatable</param>
    private void CompareExmsHtgValue(ref Hashtable htOutput, ref DataTable dtblUpdateData)
    {
        //*比對總公司統一編號
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtCORP_NO.Text.Trim(), "HEAD_CORP_NO", BaseHelper.GetShowText("01_01030100_097"));
        //*比對AML行業編號
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtCORP_MCC.Text.Trim(), "CC", BaseHelper.GetShowText("01_01030100_096"));
        //*比對設立日期
        if (this.txtCORP_ESTABLISH.Text.Trim().Length >= 7)
        {
            CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, ConvertToDC(this.txtCORP_ESTABLISH.Text.Trim()), "BUILD_DATE", BaseHelper.GetShowText("01_01030100_098"));
        }
        // 比對法律形式
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtCORP_Organization.Text.Trim(), "BUSINESS_ORGAN_TYPE", BaseHelper.GetShowText("01_01030100_099"));
        //*比對註冊國籍
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtCORP_CountryCode.Text.Trim(), "REGISTER_NATION", BaseHelper.GetShowText("01_01050100_013"));
        //20191018 如果國籍非US，自動清空州別內容
        if (!txtCORP_CountryCode.Text.Trim().Equals("") && !txtCORP_CountryCode.Text.Trim().Equals("US"))
        {
            txtCORP_CountryStateCode.Text = "X";
        }
        //*比對註冊國為美國者，請勾選州別
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtCORP_CountryStateCode.Text.Trim(), "REGISTER_US_STATE", BaseHelper.GetShowText("01_01050100_014"));
        //*比對中文登記名稱
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtREG_NAME_CH.Text.Trim(), "REG_NAME", BaseHelper.GetShowText("01_01030100_008"));
        //*比對總公司英文名稱
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtREG_NAME_EN.Text.Trim(), "CORP_REG_ENG_NAME", BaseHelper.GetShowText("01_01030100_011"));
        //*比對登記地址城市
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtREG_CITY.Text.Trim(), "REG_CITY", BaseHelper.GetShowText("01_01030100_024"));
        //*比對登記地址第一段
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtREG_ADDR1.Text.Trim(), "REG_ADDR1", BaseHelper.GetShowText("01_01030100_024"));
        //*比對登記地址第二段
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtREG_ADDR2.Text.Trim(), "REG_ADDR2", BaseHelper.GetShowText("01_01030100_024"));
        //*比對登記電話
        string _TEL = string.Empty;
        if (!txtCORP_TEL1.Text.Trim().Equals("") || !txtCORP_TEL2.Text.Trim().Equals("") || !txtCORP_TEL3.Text.Trim().Equals(""))
        {
            //20191031 配合主機修改，依3-8-5補空白送主機
            //_TEL = txtCORP_TEL1.Text.Trim() + "-" + txtCORP_TEL2.Text.Trim() + "-" + RetuenCompareValue(txtCORP_TEL3.Text.Trim());
            _TEL = txtCORP_TEL1.Text.Trim().PadRight(3,' ') + "-" + txtCORP_TEL2.Text.Trim().PadRight(8,' ') + "-" + RetuenCompareValue(txtCORP_TEL3.Text.Trim()).Trim().PadRight(5, ' ');
        }
        
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, _TEL.Trim().ToUpper(), "TEL", BaseHelper.GetShowText("01_01030100_123"));
        //*比對負責人姓名
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtPrincipalNameCH.Text.Trim(), "OWNER_CHINESE_NAME", BaseHelper.GetShowText("01_01030100_012"));
        //*比對負責人英文名
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtPrincipalNameEN.Text.Trim(), "OWNER_ENGLISH_NAME", BaseHelper.GetShowText("01_01030100_014"));
        //*比對負責人ID
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtPrincipalIDNo.Text.Trim(), "OWNER_ID", BaseHelper.GetShowText("01_01030100_013"));
        ////*比對負責人生日
        //if (this.txtPrincipalBirth.Text.Trim().Length >= 7)
        //{
        //    CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, ConvertToDC(this.txtPrincipalBirth.Text.Trim()), "OWNER_BIRTH_DATE", BaseHelper.GetShowText("01_01040101_034"));
        //}

        ////*比對身分證發證日期
        //if (this.txtPrincipalIssueDate.Text.Trim().Length >= 7)
        //{
        //    CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, ConvertToDC(this.txtPrincipalIssueDate.Text.Trim()), "OWNER_ID_ISSUE_DATE", BaseHelper.GetShowText("01_01050100_026"));
        //}

        //*比對發證地點
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtPrincipalIssuePlace.Text.Trim(), "OWNER_ID_ISSUE_PLACE", BaseHelper.GetShowText("01_01050100_027"));
        //*比對領補換類別
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtReplaceType.Text.Trim(), "OWNER_ID_REPLACE_TYPE", BaseHelper.GetShowText("01_01050100_028"));
        //*比對負責人國籍
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtPrincipalCountryCode.Text.Trim(), "OWNER_NATION", BaseHelper.GetShowText("01_01040101_071"));
        // 比對護照號碼
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtPrincipalPassportNo.Text.Trim(), "PASSPORT", BaseHelper.GetShowText("01_01030100_093"));
        // 比對護照效期
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtPrincipalPassportExpdt.Text.Trim(), "PASSPORT_EXP_DATE", BaseHelper.GetShowText("01_01030100_100"));
        // 比對居留證號
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtPrincipalResidentNo.Text.Trim(), "RESIDENT_NO", BaseHelper.GetShowText("01_01030100_094"));
        // 比對居留效期
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtPrincipalResidentExpdt.Text.Trim(), "RESIDENT_EXP_DATE", BaseHelper.GetShowText("01_01030100_101"));
        //*比對負責人電話
        string _PrincipalTEL = string.Empty;

        if (!txtPrincipal_TEL1.Text.Trim().Equals("") || !txtPrincipal_TEL2.Text.Trim().Equals("") || !txtPrincipal_TEL3.Text.Trim().Equals(""))
        {
            //20191031 配合主機修改，依3-8-5補空白送主機
            //_PrincipalTEL = txtPrincipal_TEL1.Text.Trim() + "-" + txtPrincipal_TEL2.Text.Trim() + "-" + RetuenCompareValue(txtPrincipal_TEL3.Text.Trim());
            _PrincipalTEL = txtPrincipal_TEL1.Text.Trim().PadRight(3, ' ') + "-" + txtPrincipal_TEL2.Text.Trim().PadRight(8, ' ') + "-" + RetuenCompareValue(txtPrincipal_TEL3.Text.Trim()).Trim().PadRight(5, ' ');
        }
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, _PrincipalTEL.Trim(), "OWNER_TEL", BaseHelper.GetShowText("01_01030100_015"));

        //*比對負責人戶籍地址城市
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtHouseholdCITY.Text.Trim(), "OWNER_CITY", BaseHelper.GetShowText("01_01030100_016"));
        //*比對負責人戶籍地址第一段
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtHouseholdADDR1.Text.Trim(), "OWNER_ADDR1", BaseHelper.GetShowText("01_01030100_016"));
        //*比對負責人戶籍地址第二段
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtHouseholdADDR2.Text.Trim(), "OWNER_ADDR2", BaseHelper.GetShowText("01_01030100_016"));
        if (!this.txtPrincipalIDNo.Text.Trim().Equals(""))
        {
            //比對負責人中文長姓名FLAG
            CommonFunction.ContrastData(htOutput, dtblUpdateData, chkisLongName2.Checked ? "Y" : "N", "LONG_NAME_FLAG", BaseHelper.GetShowText("01_01030200_116"));
        }
        /*20191101 修改：決議不能異動帳號相關資訊 by Peggy
        //*比對銀行名稱
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtBANK_NAME.Text.Trim(), "DDA_BANK_NAME", BaseHelper.GetShowText("01_01030100_110"));
        //*比對分行名稱
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtBANK_BRANCH.Text.Trim(), "DDA_BANK_BRANCH", BaseHelper.GetShowText("01_01030100_111"));
        //*比對戶名
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtBANK_ACCT_NAME.Text.Trim(), "DDA_ACCT_NAME", BaseHelper.GetShowText("01_01030100_112"));
        //*比對檢碼
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtCHECK_CODE.Text.Trim(), "CHECK_CODE", BaseHelper.GetShowText("01_01030100_116"));
        //*比對帳號, 給主機帳號資訊時，銀行別與帳號中間要空一格
        string _bankACC = string.Empty;
        if (!this.txtBANK_ACCT1.Text.Trim().Equals("") || !this.txtBANK_ACCT2.Text.Trim().Equals(""))
        {
            _bankACC = this.txtBANK_ACCT1.Text.Trim() + " " + this.txtBANK_ACCT2.Text.Trim();
        }
        else
            _bankACC = "";
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, _bankACC.Trim(), "DDA_BANK_ACCT", BaseHelper.GetShowText("01_01030100_114"));
        */
        //*比對歸檔編號
        CommonFunction.ContrastDataTwo(htOutput, dtblUpdateData, this.txtARCHIVE_NO.Text.Trim(), "ARCHIVE_NO", BaseHelper.GetShowText("01_01030100_124"));

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
    private EntityHTG_JC68 GetJC68(string strID)
    {
        EntityHTG_JC68 _result = new EntityHTG_JC68();
        using (BRHTG_JC68 obj = new BRHTG_JC68("P010103040001"))
        {
            EntityHTG_JC68 _data = new EntityHTG_JC68();

            _data.ID = strID;
            _result = obj.getData(_data, eAgentInfo, "11");
        }
        return _result;
    }
    /// <summary>
    /// 使用者輸入的地址解析後系統自動帶入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void TextBox_AddrChanged(object sender, EventArgs e)
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
            this.txtREG_ZIP_CODE.Text = SZIPSet.GetEntity(0).zip_code;
        }
        else
        {
            this.txtREG_ZIP_CODE.Text = "";
            if (this.txtREG_CITY.Text.Trim() != "")//20220114_Ares_Jack_不等於空值才跳錯誤檢核
            {
                base.strClientMsg += "地址查無郵遞區號，請輸入正確地址或請聯繫MFA更新";
                base.sbRegScript.Append("alert('地址查無郵遞區號，請輸入正確地址或請聯繫MFA更新');");
            }
        }
    }
}