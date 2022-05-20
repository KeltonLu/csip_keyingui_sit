// *****************************************************************
//   作    者：王予濠
//   功能說明：特店AML資料 二次鍵檔
//   創建日期：2019/01/10
//*  修改紀錄：2021/03/11_Ares_Stanley-修正粗框問題
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
// ******************************************************************

using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Framework.Common.Message;
using CSIPCommonModel.EntityLayer;
using CSIPNewInvoice.EntityLayer_new;
using CSIPKeyInGUI.EntityLayer;
using CSIPKeyInGUI.EntityLayer_new;
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.BusinessRules_new;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Common.Logging;
using Framework.WebControls;
using System.Drawing;
using System.Reflection;
using Framework.Data.OM.OMAttribute;
using System.Text;
using System.IO;
using Framework.Data.OM.Transaction;
using CSIPCommonModel.EntityLayer_new;
using Framework.Common.Utility;

public partial class Page_P010105020001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    //20191023 修改：SOC所需資訊  by Peggy
    private structPageInfo sPageInfo;//*記錄網頁訊息
    /// <summary>
    /// 切換測試電文
    /// </summary>
    private bool isTest;
    private Dictionary<string, string> compareDC = new Dictionary<string, string>();
    /// <summary>
    /// 通用字典，各項次機能以前兩碼區分
    /// </summary>
    Dictionary<string, string> DCCommonColl;
    /// <summary>
    /// 修改日期：20191007 modify by Peggy
    /// 因重新排序高管資料，故改為使用DB的ID做為KEY值，所以如果在2KEY為新增狀態時(2KEY沒資料帶1KEY資料)，需將ID清空
    /// </summary>
    private bool isExistK2 = true;
    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {
        string isTEST = UtilHelper.GetAppSettings("isTest");
        if (isTEST == "Y") { isTest = true; } else { isTest = false; }
        if (!IsPostBack)
        {
            CommonFunction.SetControlsEnabled(pnlText, false);// 清空網頁中所有的輸入欄位
            base.sbRegScript.Append(BaseHelper.SetFocus("txtTaxID"));// 將 總公司/總店統編 設為輸入焦點
            init();//20190806-RQ-2019-008595-002-長姓名需求 by Peggy

            LoadDropDownList();
        }

        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//20191023 修改：SOC所需資訊  by Peggy
    }
    /// <summary>
    /// 行業別變更時
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtBasicAMLCC_TextChanged(object sender, EventArgs e)
    {
        setCCName(txtBasicAMLCC.Text);
    }
    //查詢機能
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        //查詢前先清除畫面上會用到的控制項，避免資料殘留
        clearDataObjVal();
        init();//20190806-RQ-2019-008595-002-長姓名需求 by Peggy

        CommonFunction.SetControlsEnabled(pnlText, true);

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
        log.Customer_Id = this.txtTaxID.Text.Trim() + txtTaxNo.Text.Trim();//查詢條件        
        log.Statement_Text = string.Format("CUSTOMER_ID:{0}|AC_NO:{1}|BRANCH_ID:{2}|ROLE_ID:{3}", log.Customer_Id, log.Account_Nbr, log.Branch_Nbr, log.Role_Id); //查詢條件內容: 用 | 區隔
        BRL_AP_LOG.Add(log);
        #endregion

        //先查本機資料，有資料直接帶出
        //新增查詢條件, keyin_day須為當天日期(yyyyMMdd) by Ares Stanley 20211217
        string today = DateTime.Now.ToString("yyyyMMdd");
        EntityAML_HeadOffice DataObj = BRAML_HeadOffice.Query(txtTaxID.Text, "", "2", today);
        
        //刪除 keyin_day 非當日資料 by Ares Stanley 20211217
        BRAML_HeadOffice.DeleteNotTodayKData(txtTaxID.Text, "2", today);

        //若無資料，提示1KEY無資料
        if (string.IsNullOrEmpty(DataObj.ID))
        {
            //在查1KEY
            //新增查詢條件, keyin_day須為當天日期(yyyyMMdd) by Ares Stanley 20211217
            DataObj = BRAML_HeadOffice.Query(txtTaxID.Text, "", "1", today);

            //刪除 keyin_day 非當日資料 by Ares Stanley 20211217
            BRAML_HeadOffice.DeleteNotTodayKData(txtTaxID.Text, "1", today);
            
            isExistK2 = false;
        }


        //若無資料，提示1KEY無資料
        //        if (string.IsNullOrEmpty(DataObj.ID))
        if (string.IsNullOrEmpty(DataObj.ID))
        {
            //無資料
            //提示HTG電文回應訊息
            //strAlertMsg = MessageHelper.GetMessages("01_01050101_003");
            //sbRegScript.Append("alert('" + strAlertMsg + "');window.location.href = 'P010105010001.aspx';");

            string sAlertMsg = MessageHelper.GetMessages("01_01050101_003");
            sbRegScript.Append("alert('" + sAlertMsg + "');window.location.href = 'P010105010001.aspx';");
            isExistK2 = false;
            return;
        }
        ShowPageData(DataObj);
        setLabNation();
    }
    /// <summary>
    /// 存檔機能
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        //新增查詢條件, keyin_day須為當天日期(yyyyMMdd) by Ares Stanley 20211217
        string today = DateTime.Now.ToString("yyyyMMdd");

        //20210906_Ares_Stanley - 選取其他選項時, 文字欄位不可為空
        if (this.radBasicOther.Checked && this.txtBasicOfficeEmailOther.Text.Replace(" ", "").Replace("　", "").Length <= 0)
        {
            base.sbRegScript.Append("alert('請輸入完整E-Mail!');$('#txtBasicOfficeEmailOther').focus();");
            return;
        }

        //20210902_Ares_Stanley-檢查Email總長度為50; 20210913_Ares_Stanley-改共用function
        string mailEnd = string.Empty;
        if (this.radBasicGmail.Checked)
        {
            mailEnd = this.radBasicGmail.Text;
        }
        if (this.radBasicYahoo.Checked)
        {
            mailEnd = this.radBasicYahoo.Text;
        }
        if (this.radBasicHotmail.Checked)
        {
            mailEnd = this.radBasicHotmail.Text;
        }
        if (this.radBasicOther.Checked)
        {
            mailEnd = this.txtBasicOfficeEmailOther.Text;
        }
        if (!CommonFunction.CheckMailLength(this.txtBasicOfficeEmail.Text, mailEnd))
        {
            base.sbRegScript.Append("alert('Email總長度不可大於50碼');$('#txtBasicOfficeEmail').focus();");
            return;
        }

        //20190806-RQ-2019-008595-002-長姓名需求 by Peggy↓
        if (chkisLongName.Checked)
        {
            if (txtPrincipalNameCH_L.Text.Trim().Equals(""))
            {
                base.sbRegScript.Append("alert('負責人長姓名FLAG勾選時，請輸入中文長姓名');$('#txtPrincipalNameCH_L').focus();");
                return;
            }

            if (txtPrincipalNameCH_Pinyin.Text.Trim().Equals(""))
            {
                base.sbRegScript.Append("alert('負責人長姓名FLAG勾選時，請輸入羅馬拼音');$('#txtPrincipalNameCH_Pinyin').focus();");
                return;
            }

            if ((ToWide(txtPrincipalNameCH_L.Text.Trim()).Length + LongNameRomaClean(txtPrincipalNameCH_Pinyin.Text).Trim().Length) < 5)
            {
                base.sbRegScript.Append("alert('負責人長姓名FLAG勾選時，負責人姓名(中文+羅馬拼音)需超過4個字以上');$('#txtPrincipalNameCH_L').focus();");
                return;
            }
        }

        if (chkisLongName_c.Checked)
        {
            if (txtBasicContactMan_L.Text.Trim().Equals(""))
            {
                base.sbRegScript.Append("alert('聯絡人長姓名FLAG勾選時，請輸入中文長姓名');$('#txtBasicContactMan_L').focus();");
                return;
            }

            if (txtBasicContactMan_Pinyin.Text.Trim().Equals(""))
            {
                base.sbRegScript.Append("alert('聯絡人長姓名FLAG勾選時，請輸入羅馬拼音');$('#txtBasicContactMan_Pinyin').focus();");
                return;
            }

            if ((ToWide(txtBasicContactMan_L.Text.Trim()).Length + LongNameRomaClean(txtBasicContactMan_Pinyin.Text).Trim().Length) < 5)
            {
                base.sbRegScript.Append("alert('聯絡人長姓名FLAG勾選時，負責人姓名(中文+羅馬拼音)需超過4個字以上');$('#txtBasicContactMan_L').focus();");
                return;
            }
        }

        if (!txtPrincipalNameCH_Pinyin.Text.Trim().Equals(""))
        {
            if (!ValidRoma(txtPrincipalNameCH_Pinyin.Text.Trim()))
            {
                base.sbRegScript.Append("alert('負責人羅馬拼音輸入有誤');$('#txtPrincipalNameCH_Pinyin').focus();");
                return;
            }
        }
        if (!txtBasicContactMan_Pinyin.Text.Trim().Equals(""))
        {
            if (!ValidRoma(txtBasicContactMan_Pinyin.Text.Trim()))
            {
                base.sbRegScript.Append("alert('聯絡人羅馬拼音輸入有誤');$('#txtBasicContactMan_Pinyin').focus();");
                return;
            }
        }
        //20190806-RQ-2019-008595-002-長姓名需求 by Peggy↑

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
        string address = this.txtBasicBookAddr1.Text.Trim();
        if (!string.IsNullOrEmpty(address) && !checkREG_ZIP_CODE(address))
        {
            base.sbRegScript.Append("alert('地址查無郵遞區號，請輸入正確地址或請聯繫MFA更新');");
            return;
        }

        string strAlertMsg = "";

        ///未勾選不可以存檔
        if (CustChK01.Checked == false || CustChK02.Checked == false || CustChK03.Checked == false)
        {
            strAlertMsg = @"『" + MessageHelper.GetMessage("01_01050101_009") + "』";
            sbRegScript.Append("alert('" + strAlertMsg + "');");
            return;
        }

        //將畫面欄位轉換為物件
        //新增查詢條件, keyin_day須為當天日期(yyyyMMdd) by Ares Stanley 20211217
        EntityAML_HeadOffice DataObj = BRAML_HeadOffice.Query(txtTaxID.Text, txtTaxNo.Text, "2", today);

        this.GetVal<EntityAML_HeadOffice>(ref DataObj);

        //20201125-存檔時將地址直接轉全型存檔送出
        //登記地址
        DataObj.BasicBookAddr1 = ToWide(txtBasicBookAddr1.Text);
        DataObj.BasicBookAddr2 = ToWide(txtBasicBookAddr2.Text);
        DataObj.BasicBookAddr3 = ToWide(txtBasicBookAddr3.Text);
        //通訊地址
        DataObj.BasicContactAddr1 = ToWide(txtBasicContactAddr1.Text);
        DataObj.BasicContactAddr2 = ToWide(txtBasicContactAddr2.Text);
        DataObj.BasicContactAddr3 = ToWide(txtBasicContactAddr3.Text);
        //戶籍地址
        DataObj.PrincipalBookAddr1 = ToWide(txtPrincipalBookAddr1.Text);
        DataObj.PrincipalBookAddr2 = ToWide(txtPrincipalBookAddr2.Text);
        DataObj.PrincipalBookAddr3 = ToWide(txtPrincipalBookAddr3.Text);

        //特殊欄位處理，如EMAIL
        CollectEmail(ref DataObj);
        DataObj.keyin_day = DateTime.Now.ToString("yyyyMMdd");
        DataObj.keyin_Flag = "2";
        DataObj.keyin_userID = eAgentInfo.agent_id;


        List<string> errMsg = new List<string>();
        string sLineID = "";
        int addItems = 0;//20191029 記錄新增筆數 add by Peggy
        string _ID = "";
        //讀取子項目
        List<EntityAML_SeniorManager> managerColl = DataObj.AML_SeniorManagerColl;
        
        for (int i = 0; i < 12; i++)
        {
            sLineID = (i + 1).ToString();
            _ID = "";
            EntityAML_SeniorManager dObj = null;
            CustLabel IdentityID = this.FindControl("lblID_" + sLineID.Trim()) as CustLabel;//20190919-高管畫面重新排序 by Peggy
            CustCheckBox DelID = this.FindControl("chkSeniorManagerDelete_" + sLineID.Trim()) as CustCheckBox;//20191029-高管畫面重新排序 by Peggy
            CustTextBox NameL = this.FindControl("txtSeniorManagerName_" + sLineID.Trim()) as CustTextBox;//20191029-高管畫面重新排序 by Peggy

            if (!NameL.Text.Trim().Equals(""))
            {
                foreach (EntityAML_SeniorManager mob in managerColl)
                {
                    //20190919-高管畫面重新排序 by Peggy
                    //if (mob.LineID == sLineID)
                    //{
                    //    dObj = mob;
                    //    continue;
                    //}

                    //20191030 test mark by Peggy 
                    //if (!IdentityID.Text.Trim().Equals("") && mob.ID.Trim().Equals(IdentityID.Text.Trim()))
                    //{
                    //    dObj = mob;
                    //    dObj.LineID = sLineID;
                    //    //dObj.Expdt = "";//201910月RC-姵晴要求，到期日要清空

                    //    addItems++;
                    //    break;
                    //}

                    if (mob.LineID.Trim() == (addItems+1).ToString().Trim())
                    {
                        dObj = mob;
                        _ID = mob.ID;
                        if (IdentityID.Text.Trim().Equals(""))//因資料是抓畫面行數的資料，如畫面不重整，DB的LINEID與畫面行會不同，故先把LINEID的值先以畫面為主，抓完資料才變更回來
                        {
                            dObj.LineID = sLineID;
                        }
                        addItems++;
                        break;
                    }
                }

                //找不到舊資料
                if (dObj == null)
                {
                    dObj = new EntityAML_SeniorManager();
                    dObj.LineID = (i + 1).ToString();

                    addItems++;//20191029 add by Peggy
                    //填入欄位以外 
                    managerColl.Add(dObj);
                }

                dObj.BasicTaxID = DataObj.BasicTaxID;  //以主畫面輸入統編為主
                dObj.keyin_day = DateTime.Now.ToString("yyyyMMdd");
                dObj.keyin_Flag = "2";
                dObj.keyin_userID = eAgentInfo.agent_id;

                this.GetVal<EntityAML_SeniorManager>(ref dObj);
                if (dObj.ID.Trim().Equals("") && !_ID.Trim().Equals(""))
                {
                    dObj.ID = _ID.Trim();
                }

                if (dObj.isDEL == "1")
                {
                    continue;
                }

                //追加驗證
                //ValidVal<EntityAML_SeniorManager>(dObj, ref errMsg, (Page)this, dObj.LineID);

                //追加國籍與姓名轉換處理
                checkNation(dObj, ref errMsg, dObj.LineID);

                if (!LoadInentity(dObj.LineID, dObj))
                {
                    errMsg.Add("高階經理人第 " + dObj.LineID + "行身分類型輸入值有誤\\n");
                }

                //20201102-RQ-2020-021027-003 外國人新式統一證號
                if (!LoadCheckFK(dObj.LineID, dObj))
                {
                    errMsg.Add("高階經理人第 " + dObj.LineID + "行，統一證號輸入錯誤，請重新輸入\\n");
                }

                //20220105 檢核 國籍TW 身分證為Z999999999, 身分證指定為1 by Ares Dennis
                if (!checkIdNoType(dObj.LineID))
                {
                    errMsg.Add("高階經理人第 " + dObj.LineID + "行身分證類型輸入值有誤\\n");
                }

                if (!sLineID.Trim().Equals(addItems.ToString().Trim()))//FOR重新排序
                {
                    dObj.LineID = addItems.ToString().Trim();
                }
            }
        }//高管12次結束

        DataObj.AML_SeniorManagerColl = managerColl;

    
        //以國籍設定公司型態 
        DataObj.BasicCORP_TYPE = "4";
        if (DataObj.BasicCountryCode == "TW")
        {
            DataObj.BasicCORP_TYPE = "1";
        }
         
        //追加驗證
        ValidVal<EntityAML_HeadOffice>(DataObj, ref errMsg, (Page)this, "");
        //特殊關聯欄位處理
        ValidLinkedFields(DataObj, ref errMsg);

        if (errMsg.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            int linC = 0;
            foreach (string oitem in errMsg)
            {
                sb.Append(oitem);
                linC++;
                if (linC > 10)
                {
                    sb.Append("---顯示前十筆，以下省略---");
                    break;
                }
            }
            strAlertMsg = @"『" + MessageHelper.GetMessage("01_01050101_008") + "』" + @"\n" + sb.ToString();
            sbRegScript.Append("alert('" + strAlertMsg + "');");
            return;
        }

        //是否為新建還是修改，以電文行業別判斷 ，若不存在則以資料庫讀出為主
        if (Session["2KisNew" + txtTaxID.Text] != null)
        {
            DataObj.isNew = Session["2KisNew" + txtTaxID.Text].ToString();
        }
        //以上畫面資料
        //將2KEY寫入資料庫
        bool result = BRAML_HeadOffice.Insert(DataObj);

        if (!result)
        {

            strAlertMsg = @"『" + MessageHelper.GetMessage("01_01050101_004") + "』";
            sbRegScript.Append("alert('" + strAlertMsg + "');");
            return;
        }

        //新增查詢條件, keyin_day須為當天日期(yyyyMMdd) by Ares Stanley 20211217
        DataSet ds = BRAML_HeadOffice.QueryTB(txtBasicTaxID.Text, today);

        //比對12K差異用
        List<string> errMsgT = new List<string>();
        //比對所有欄位，是否有差異
        bool retCompare = compareValue(ds, ref errMsgT);
        //有差異則提示後離開
        if (!retCompare)
        {
            StringBuilder sb = new StringBuilder();
            int linC = 0;
            foreach (string oitem in errMsgT)
            {
                sb.Append(oitem);
                linC++;
                if (linC > 10)
                {
                    sb.Append("---顯示前十筆，以下省略---");
                    break;
                }
            }
            strAlertMsg = @"『" + MessageHelper.GetMessage("01_01050101_006") + "』" + "\\n" + sb.ToString();
            sbRegScript.Append("alert('" + strAlertMsg + "');");
            return;
        }

        //無差異，往下
        string isNew = ds.Tables[1].Rows[0]["isNew"].ToString();


        UpdateJC66(txtTaxID.Text.Trim(), isNew, DataObj);

        return;



        ////以畫面上物件產生JC66用HASHTABLE
        //Hashtable JC66OBj = buildJC66Hash(DataObj);
        //JC66OBj.Add("CORP_SEQ", "0000");
        //if (isNew == "0") //0表示為修改，給C，反之給A
        //{
        //    JC66OBj.Add("FUNCTION_CODE", "C");
        //}
        //if (isNew == "1")
        //{
        //    JC66OBj.Add("FUNCTION_CODE", "A");
        //}


        //Hashtable hstExmsP4A;
        //if (!isTest)
        //{
        //    //上送主機資料
        //    hstExmsP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC66, JC66OBj, false, "11", eAgentInfo);
        //}
        //else
        //{
        //    ////測試用模擬資料
        //    hstExmsP4A = BuildTestHashTB();
        //}

        ////檢核電文回傳欄位是否成功
        //if (hstExmsP4A["MESSAGE_TYPE"].ToString() == "0000")
        //{  ///使用交易
        //    using (OMTransactionScope ts = new OMTransactionScope())
        //    {
        //        ///電文更新成功 {
        //        //更新欄位  hidisNew.Value == "1" 才需要     AML_ADDHEADDOfficereport  OfficeAdd_day -KEY==> YYYYMMDD 統編      
        //        if (isNew == "1")
        //        {
        //            EntityAML_AddHeadOfficeReport pObj = new EntityAML_AddHeadOfficeReport();
        //            pObj.TaxID = DataObj.BasicTaxID;
        //            pObj.OfficeAdd_day = DateTime.Now.ToString("yyyyMMdd");
        //            BRAML_AddHeadOfficeReport.Update(pObj);
        //        }
        //        //清除資料庫  }
        //        bool dRet = BRAML_HeadOffice.DeleteKData(DataObj.BasicTaxID);
        //        if (dRet)
        //        {
        //            ts.Complete();
        //            strAlertMsg = @"『" + MessageHelper.GetMessage("01_01050101_007") + "』";
        //            sbRegScript.Append("alert('" + strAlertMsg + "');");
        //            CommonFunction.SetControlsEnabled(pnlText, false);// 清空網頁中所有的輸入欄位
        //        }
        //        else
        //        {

        //            strAlertMsg = @"『" + MessageHelper.GetMessage("01_01050101_004") + "』";
        //            sbRegScript.Append("alert('" + strAlertMsg + "');");
        //            return;
        //        }


        //    }
        //}
        //else
        //{
        //    if (hstExmsP4A.ContainsKey("HtgMsg"))
        //    {
        //        strAlertMsg = hstExmsP4A["HtgMsg"].ToString();
        //    }
        //    else
        //    {
        //        strAlertMsg = @"『" + MessageHelper.GetMessage("01_01050101_005") + "』";

        //    }
        //    sbRegScript.Append("alert('" + strAlertMsg + "');");
        //    return;
        //}

    }
    protected void txtHCOP_BENE_NATION_TextChanged(object sender, EventArgs e)
    {
        //找出LINEID 以控制像名稱檢核
        CustTextBox mConteol = (CustTextBox)sender;
        string sname = mConteol.ID;
        string[] namePhase = sname.Split('_');
        string LineID = "";
        if (namePhase.Length == 2)
        {
            LineID = namePhase[1];
        }
        else { return; }


        string strAlertMsg = "";
        mConteol.Text = mConteol.Text.ToUpper();

        strIsShow = "setfocus('" + "txtSeniorManagerIDNo_" + LineID + "');";

        CustLabel myControl = FindControl("lblEnNotice_" + LineID) as CustLabel;
        if (string.IsNullOrEmpty(mConteol.Text)) //空白不檢核
        {
            //開放該行
            myControl.Visible = false;
            return;
        }
        //   mConteol.BackColor = Color.Empty;
        //檢核國籍       
        //不正確的國籍
        if (GetDcValue("CT_1_" + mConteol.Text) == "")
        {

            //      mConteol.BackColor = Color.Red;
            strAlertMsg = MessageHelper.GetMessages("01_01080103_009") + "第" + LineID + "行\\n";
            sbRegScript.Append("alert('" + strAlertMsg + "');");
            return;
        }
        myControl.Visible = true;
        if (mConteol.Text != "TW")
        {
            myControl.ShowID = "01_01080103_057";
        }
        else
        {
            myControl.ShowID = "01_01080103_058";
        }
        setLabNation();
    }    

    /// <summary>
    /// 使用者輸入的地址解析後系統自動帶入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void TextBox_AddrChanged(object sender, EventArgs e)
    {
        string strZipData = this.txtBasicBookAddr1.Text.Trim();

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
            if (this.txtBasicBookAddr1.Text.Trim() != "")//20220114_Ares_Jack_不等於空值才跳錯誤檢核
            {
                base.strClientMsg += "地址查無郵遞區號，請輸入正確地址或請聯繫MFA更新";
                base.sbRegScript.Append("alert('地址查無郵遞區號，請輸入正確地址或請聯繫MFA更新');");
            }
        }
        base.sbRegScript.Append(BaseHelper.SetFocus("txtBasicBookAddr2"));// 將地址2設為輸入焦點
    }
    #endregion

    #region 方法區
    private void setLabNation()
    {
        for (int i = 1; i < 13; i++)
        {
            CustLabel myControl = FindControl("lblEnNotice_" + i.ToString()) as CustLabel;
            CustTextBox myControlNat = FindControl("txtSeniorManagerCountryCode_" + i.ToString()) as CustTextBox;
            if (string.IsNullOrEmpty(myControlNat.Text))
            {
                myControl.Visible = false;
            }
            else
            {
                if (myControlNat.Text.ToUpper() != "TW")
                {
                    myControl.ShowID = "01_01080103_057";
                }
                else
                {
                    myControl.ShowID = "01_01080103_058";
                }
            }
        }
    }
    /// <summary>
    /// 將資料顯示於畫面
    /// </summary>
    /// <param name="DataObj"></param>
    /// <param name="isHTG"></param>
    private void ShowPageData(EntityAML_HeadOffice DataObj)
    {
        if (DataObj.PrincipalBirth == "00000000")
        {
            DataObj.PrincipalBirth = "";
        }

        this.SetVal(DataObj, false); //將本機資料寫入畫面
                                     
        txtBasicTaxID.Enabled = false;//鎖住統編，不允許修改

        //20190806-RQ-2019-008595-002-長姓名需求 by Peggy
        //處理特殊欄位
        //負責人
        //當中文長姓名不為空時，中文長姓名flag要打勾
        if (!txtPrincipalNameCH_L.Text.Trim().Equals(""))
            chkisLongName.Checked = true;

        CheckBox_CheckedChanged(chkisLongName, null);

        //聯絡人
        //當中文長姓名不為空時，中文長姓名flag要打勾
        if (!txtBasicContactMan_L.Text.Trim().Equals(""))
            chkisLongName_c.Checked = true;

        CheckBox_CheckedChanged(chkisLongName_c, null);
        //20190806-RQ-2019-008595-002-長姓名需求 by Peggy

        //20190731 修改：使下單選拉也focus在資料值的index by Peggy ↓
        //總公司基本資料-註冊國籍
        this.dropBasicCountryCode.SelectByValue(DataObj.BasicCountryCode);
        //負責人國籍
        this.dropPrincipalCountryCode.SelectByValue(DataObj.PrincipalCountryCode);
        //SCDD表-註冊國籍
        this.dropSCCDCountryCode.SelectByValue(DataObj.SCCDCountryCode);
        //僑外資 / 外商母公司國別
        this.dropSCCDForeignCountryStateCode.SelectByValue(DataObj.SCCDForeignCountryStateCode);
        //主要之營業處所國別
        this.dropSCCDOtherCountryCode.SelectByValue(DataObj.SCCDOtherCountryCode);

        //業處所是否在高風險或制裁國家
        this.dropSCCDIsSanctionCountryCode1.SelectByValue(DataObj.SCCDIsSanctionCountryCode1);
        this.dropSCCDIsSanctionCountryCode2.SelectByValue(DataObj.SCCDIsSanctionCountryCode2);
        this.dropSCCDIsSanctionCountryCode3.SelectByValue(DataObj.SCCDIsSanctionCountryCode3);
        this.dropSCCDIsSanctionCountryCode4.SelectByValue(DataObj.SCCDIsSanctionCountryCode4);
        this.dropSCCDIsSanctionCountryCode5.SelectByValue(DataObj.SCCDIsSanctionCountryCode5);

        //20190731 修改：使下單選拉也focus在資料值的index by Peggy ↑

        List<EntityAML_SeniorManager> ManaObjColl = DataObj.AML_SeniorManagerColl;

        int resetLineID = 1;//20190919-高管畫面重新排序 by Peggy
        foreach (EntityAML_SeniorManager oitem in ManaObjColl)
        {
            oitem.LineID = resetLineID.ToString().Trim();//20190919-高管畫面重新排序 by Peggy
            if (!isExistK2)//因現已使用DB的ID做為KEY值，所以如果在2KEY沒資料帶1KEY資料時，需將ID清空
            {
                oitem.ID = "";
            }

            this.SetVal<EntityAML_SeniorManager>(oitem, false);
            //BEEN_JOB_TYPE轉換 
            string lid = oitem.LineID;

            setInentity(lid, oitem);            

            //高管人員長姓名控制
            GVLongNameDisplay(lid);//20190806-RQ-2019-008595-002-長姓名需求 by Peggy
            resetLineID++;//20190919-高管畫面重新排序 by Peggy
        }

        //處理特殊欄位 .如EMAIL等非常規欄位填值
        SetEmailVal(DataObj);

        // 20210527 EOS_AML(NOVA) by Ares Dennis start
        txtREG_ZIP_CODE.Enabled = false;
        txtREG_ZIP_CODE.BackColor = Color.LightGray;
        // 20210527 EOS_AML(NOVA) by Ares Dennis end
    }

    /// <summary>
    /// BEEN_JOB_TYPE轉換 
    /// </summary>
    /// <param name="lid"></param>
    private void setInentity(string lid, EntityAML_SeniorManager oitem)
    {
        string CtrlName = "txtSeniorManagerIdentity_" + lid;
        CustTextBox controllS = this.FindControl(CtrlName) as CustTextBox;
        //找不到不處理;
        if (controllS == null) { return; }
        controllS.Text = "";
        controllS.BackColor = Color.Empty;
        if (oitem.Identity1 == "Y")
        {
            controllS.Text += "1/";
        }
        if (oitem.Identity2 == "Y")
        {
            controllS.Text += "2/";
        }
        if (oitem.Identity3 == "Y")
        {
            controllS.Text += "3/";
        }
        if (oitem.Identity4 == "Y")
        {
            controllS.Text += "4/";
        }
        if (oitem.Identity5 == "Y")
        {
            controllS.Text += "5/";
        }
        if (oitem.Identity6 == "Y")
        {
            controllS.Text += "6/";
        }
        //若有值，移除最後一個
        if (controllS.Text.Length > 1)
        {
            controllS.Text = controllS.Text.Remove(controllS.Text.Length - 1, 1);
        }

        string CtrlNamePinyin = "txtName_Pinyin_" + lid;
        CustTextBox ctbNamePinyin = this.FindControl(CtrlNamePinyin) as CustTextBox;
        if (ctbNamePinyin.Text.Trim().Equals(""))
            ctbNamePinyin.Text = "Ｘ";

    }
    /// <summary>
    /// BEEN_JOB_TYPE轉換  讀取
    /// </summary>
    /// <param name="lid"></param>
    private bool LoadInentity(string lid, EntityAML_SeniorManager oitem)
    {
        bool result = false;
        string CtrlName = "txtSeniorManagerIdentity_" + lid;
        CustTextBox controllS = this.FindControl(CtrlName) as CustTextBox;
        Dictionary<string, string> vKey = new Dictionary<string, string>();
        vKey.Add("1", "");
        vKey.Add("2", "");
        vKey.Add("3", "");
        vKey.Add("4", "");
        vKey.Add("5", "");
        vKey.Add("6", "");

        //找不到不處理;
        if (controllS == null) { return true; }
        //無輸入值不處理
        //if (string.IsNullOrEmpty(controllS.Text)) { return true; }

        CustTextBox CName = FindControl("txtSeniorManagerName_" + lid) as CustTextBox; //姓名
        CustTextBox CNation = FindControl("txtSeniorManagerCountryCode_" + lid) as CustTextBox; //國籍;

        //姓名和國籍都空值，表示此列要全部空值，所以身分類型要空值
        if (string.IsNullOrEmpty(CName.Text) && string.IsNullOrEmpty(CNation.Text) && string.IsNullOrEmpty(controllS.Text))
        {
            return true;
        }

        if (string.IsNullOrEmpty(controllS.Text) && !(string.IsNullOrEmpty(CName.Text) && string.IsNullOrEmpty(CNation.Text)))
        {
            oitem.Identity1 = "N";
            oitem.Identity2 = "N";
            oitem.Identity3 = "N";
            oitem.Identity4 = "N";
            oitem.Identity5 = "N";
            oitem.Identity6 = "N";
            return true;
        }


        if (string.IsNullOrEmpty(controllS.Text)) 
        {
            oitem.Identity1 = "N";
            oitem.Identity2 = "N";
            oitem.Identity3 = "N";
            oitem.Identity4 = "N";
            oitem.Identity5 = "N";
            oitem.Identity6 = "N";
            return true; 
        }

        oitem.Identity1 = "N";
        oitem.Identity2 = "N";
        oitem.Identity3 = "N";
        oitem.Identity4 = "N";
        oitem.Identity5 = "N";
        oitem.Identity6 = "N";
        string[] inputValid = controllS.Text.Split('/');
        controllS.BackColor = Color.Empty;

        foreach (string ootem in inputValid)
        {
            if (!vKey.ContainsKey(ootem))
            {
                controllS.BackColor = Color.Red;
                return result;
            }
        }

        if (controllS.Text.IndexOf("1") > -1)
        {
            oitem.Identity1 = "Y";
        }
        if (controllS.Text.IndexOf("2") > -1)
        {
            oitem.Identity2 = "Y";
        }
        if (controllS.Text.IndexOf("3") > -1)
        {
            oitem.Identity3 = "Y";
        }
        if (controllS.Text.IndexOf("4") > -1)
        {
            oitem.Identity4 = "Y";
        }
        if (controllS.Text.IndexOf("5") > -1)
        {
            oitem.Identity5 = "Y";
        }
        if (controllS.Text.IndexOf("6") > -1)
        {
            oitem.Identity6 = "Y";
        }
        result = true;
        return result;
    }

    /// <summary>
    /// 高階管理人之統一證號、虛擬ID驗證
    /// </summary>
    /// <param name="lid"></param>
    private bool LoadCheckFK(string lid, EntityAML_SeniorManager oitem)
    {
        bool result = true;
        //姓名
        CustTextBox txtName = FindControl("txtSeniorManagerName_" + lid) as CustTextBox;
        //身分證件號碼
        CustTextBox txtSMID = FindControl("txtSeniorManagerIDNo_" + lid) as CustTextBox;
        //身分證件類型
        CustTextBox txtIDType = FindControl("txtSeniorManagerIDNoType_" + lid) as CustTextBox;

        //沒有姓名就不處理
        if (txtName == null) { return true; }

        //20211029_Ares_Jack_虛擬ID延後上線, 先還原Z999999999
        ////身分證類型不是其他(7)開頭不能是CA
        //if (txtIDType.Text.Trim() != "7" && txtSMID.Text.Trim().Substring(0, 2) == "CA") { return false; }

        if (!string.IsNullOrEmpty(txtName.Text) && !string.IsNullOrEmpty(txtIDType.Text) && !string.IsNullOrEmpty(txtSMID.Text) && txtIDType.Text.Trim().Equals("4"))
        {
            if (!CheckResidentID(txtSMID.Text.Trim()))
                result = false;
        }

        //20211029_Ares_Jack_虛擬ID延後上線, 先還原Z999999999
        //if (!string.IsNullOrEmpty(txtName.Text) && !string.IsNullOrEmpty(txtIDType.Text) && !string.IsNullOrEmpty(txtSMID.Text) && txtIDType.Text.Trim().Equals("7"))
        //{
        //    //20210629_Ares_Stanley-新增其他身分證虛擬ID驗證
        //    if (!CheckResidentID_SeniorManager(txtSMID.Text.Trim(), txtIDType.Text.Trim()))
        //        result = false;
        //}

        return result;
    }

    /// <summary>
    /// 檢核 國籍TW 身分證為Z999999999, 身分證指定為1
    /// </summary>
    /// <param name="lid">高管畫面LINEID</param>
    /// <returns></returns>
    private bool checkIdNoType(string lid)
    {
        //國籍
        CustTextBox txtCountryCode = FindControl("txtSeniorManagerCountryCode_" + lid) as CustTextBox;
        //身分證件號碼
        CustTextBox txtSMID = FindControl("txtSeniorManagerIDNo_" + lid) as CustTextBox;
        //身分證件類型
        CustTextBox txtIDType = FindControl("txtSeniorManagerIDNoType_" + lid) as CustTextBox;

        if (txtCountryCode.Text.Trim().ToUpper() == "TW" && txtSMID.Text.Trim() == "Z999999999")
        {
            if (txtIDType.Text != "1")
            {
                return false;
            }
        }

        return true;
    }

    private EntityAML_HeadOffice GetHTGMsg(string TaxID, string TaxNo)
    {
        EntityAML_HeadOffice rtn = new EntityAML_HeadOffice();

        //建立HASHTABLE
        Hashtable JC66OBj = new Hashtable();
        JC66OBj.Add("FUNCTION_CODE", "I");
        JC66OBj.Add("CORP_NO", txtTaxID.Text);
        JC66OBj.Add("CORP_SEQ", "0000");


        ////上送主機資料
        //   Hashtable hstExmsP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC66, JC66OBj, false, "11", eAgentInfo);
        ////測試用模擬資料
        ////Hashtable hstExmsP4A = GetTestData();

        Hashtable hstExmsP4A;
        if (!isTest)
        {
            //上送主機資料
            hstExmsP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC66, JC66OBj, false, "11", eAgentInfo);
        }
        else
        {
            ////測試用模擬資料
            hstExmsP4A = GetTestData();
        }
        

        //取回後MAP至物件 
        rtn = HTconvertToObj(hstExmsP4A);
        //檢核回傳欄位MESSAGE_TYPE 0000表示以存在總公司，故為修改  0006表示不存在，為新增
        string rMsg = hstExmsP4A["MESSAGE_TYPE"].ToString();
        switch (rMsg)
        {
            case "0000":
                Session["2KisNew" + TaxID] = "0";
                
                break;
            case "0006":
                Session["2KisNew" + TaxID] = "1";
                break;

        }
                

        if (!string.IsNullOrEmpty(hstExmsP4A["OWNER_LNAM_FLAG"].ToString()) && hstExmsP4A["OWNER_LNAM_FLAG"].ToString().Trim().Equals("Y"))//負責人長姓名FLAG
        {
            chkisLongName.Checked = true;
            
            EntityHTG_JC68 htReturn_JC68 = GetJC68(txtPrincipalIDNo.Text.Trim());//負責人身分證ID
            txtPrincipalNameCH_L.Text = htReturn_JC68.LONG_NAME;//負責人-中文長姓名
            txtPrincipalNameCH_Pinyin.Text = htReturn_JC68.PINYIN_NAME;//負責人-羅馬拼音

        }
        if (hstExmsP4A["CONTACT_LNAM_FLAG"] != null && hstExmsP4A["CONTACT_LNAM_FLAG"].ToString().Trim().Equals("Y"))//負責人長姓名FLAG
        {
            chkisLongName_c.Checked = true;
            
            EntityHTG_JC68 htReturn_JC68 = GetJC68(txtTaxID.Text.Trim() + txtTaxNo.Text.Trim());//總公司統編+序號
            txtBasicContactMan_L.Text = htReturn_JC68.LONG_NAME;//負責人-中文長姓名
            txtBasicContactMan_Pinyin.Text = htReturn_JC68.PINYIN_NAME;//負責人-羅馬拼音
        }
        return rtn;
    }
    
    private EntityHTG_JC68 GetJC68(string strID)
    {
        EntityHTG_JC68 _result = new EntityHTG_JC68();
        using (BRHTG_JC68 obj = new BRHTG_JC68("P010105020001"))
        {
            EntityHTG_JC68 _data = new EntityHTG_JC68();

            _data.ID = strID;
            _result = obj.getData(_data, eAgentInfo, "11");
        }
        return _result;
    }

    /// <summary>
    /// 模擬測試用JC66電文回傳
    /// </summary>
    /// <returns></returns>
    private Hashtable GetTestData()
    {
        //讀取D:\JC66_Download_TEST.txt做成HSAHTABLE
        string[] lincoll = File.ReadAllLines(@"D:\JC66_Download_TEST.txt", Encoding.Default);
        Hashtable JC66OBj = new Hashtable();
        foreach (string oitem in lincoll)
        {
            string[] tmpColl = oitem.Split(new string[] { "<!>" }, StringSplitOptions.None);
            if (tmpColl.Length == 2)
            {
                JC66OBj.Add(tmpColl[0], tmpColl[1]);
            }
        }
        return JC66OBj;
    }
    /// <summary>
    /// 轉換HASHTABLE to EntityAML_HeadOffice物件
    /// </summary>
    /// <param name="inObj"></param>
    /// <returns></returns>
    private EntityAML_HeadOffice HTconvertToObj(Hashtable inObj)
    {
        EntityAML_HeadOffice rtnObj = new EntityAML_HeadOffice();

        //先將主體值找出來，方法是用映射，取出對應欄位
        Type v = rtnObj.GetType();  //取的型別實體
        PropertyInfo[] props = v.GetProperties(); //取出所有公開屬性(可以被外部存取得 
                                                  //處理總公司資料
        foreach (PropertyInfo prop in props)
        {
            object[] attrs = prop.GetCustomAttributes(true); //取得自訂屬性，第一個物件
            AttributeRfPage authAttr;
            for (int xi = 0; xi < attrs.Length; xi++)
            {
                if (attrs[xi] is AttributeRfPage)
                {
                    authAttr = attrs[xi] as AttributeRfPage;
                    //暫時用不道
                    //string propName = prop.Name;
                    //string authID = authAttr.ControlID; 
                    string Jc66FieldKey = authAttr.JC66NAME;
                    //JC66有值，建立HASTABLE欄位，
                    if (!string.IsNullOrEmpty(Jc66FieldKey))
                    {
                        if (inObj.ContainsKey(Jc66FieldKey))
                        {
                            prop.SetValue(rtnObj, inObj[Jc66FieldKey], null);
                        }
                    }
                }
            }
        }
        //轉換子公司資料
        for (int i = 1; i < 13; i++)
        {
            EntityAML_SeniorManager sMan = new EntityAML_SeniorManager();
            sMan.LineID = i.ToString();
            Type Stype = sMan.GetType();  //取的型別實體
            PropertyInfo[] Sprops = Stype.GetProperties(); //取出所有公開屬性(可以被外部存取得 
                                                           //處理總公司資料
            foreach (PropertyInfo prop in Sprops)
            {
                object[] attrs = prop.GetCustomAttributes(true); //取得自訂屬性，第一個物件
                AttributeRfPage authAttr;
                for (int xi = 0; xi < attrs.Length; xi++)
                {
                    if (attrs[xi] is AttributeRfPage)
                    {
                        authAttr = attrs[xi] as AttributeRfPage;
                        //暫時用不道
                        string propName = prop.Name;
                        if (propName == "Name")
                        {
                            propName = prop.Name;
                        }
                        //string authID = authAttr.ControlID; 
                        if (string.IsNullOrEmpty(authAttr.JC66NAME)) { continue; }
                        string Jc66FieldKey = authAttr.JC66NAME + i.ToString();
                        //JC66有值，建立HASTABLE欄位，
                        if (!string.IsNullOrEmpty(Jc66FieldKey))
                        {
                            if (inObj.ContainsKey(Jc66FieldKey))
                            {
                                prop.SetValue(sMan, inObj[Jc66FieldKey], null);

                            }
                        }
                        continue;
                    }
                }
            }
            //會有12筆
            rtnObj.AML_SeniorManagerColl.Add(sMan);

        }

        return rtnObj;
    }
    //產生測試用HSAHTAB:E 模擬回傳訊息
    private Hashtable BuildTestHashTB()
    {
        Hashtable rtn = new Hashtable();
        rtn.Add("MESSAGE_TYPE", "0000");
        return rtn;
    }

    /// <summary>
    /// 比對 1K 2K 資料內容
    /// </summary>
    /// <param name="dataObj1K"></param>
    /// <param name="dataObj"></param>
    /// <returns></returns>
    private bool compareValue(DataSet DS, ref List<string> errMsg)
    {
        if (compareDC.Count == 0)
        {
            initcompareDC();
        }
        ///TABLE數不正確不比對
        if (DS.Tables.Count != 4)
        {
            return false;
        }
        DataTable K1 = DS.Tables[0];     //1Key總公司
        DataTable K2 = DS.Tables[1];     //2Key總公司
        DataTable KM1 = DS.Tables[2];    //1Key高階經理人
        DataTable KM2 = DS.Tables[3];   //2Key高階經理人


        //從資料庫讀取1KEY 2KEY 以DATATABEL帶回，方便比對 
        bool result = true;
        string tmp = "名稱: {2} 差異值: 1KEY:{0}, 2KEY:{1} \\n";
        //因為1K多一個isNew欄位，所以比到-1即可
        for (int i = 0; i < K1.Columns.Count - 1; i++)
        {
            string f1 = K1.Rows[0][i].ToString();
            string f2 = K2.Rows[0][i].ToString();
            string sKey = K1.Columns[i].ColumnName;
            CustTextBox tt = this.FindControl("txt" + sKey) as CustTextBox;
            if (tt != null)
            {
                //驗證前把控制項改為白底
                tt.BackColor = System.Drawing.Color.Empty;
            }
            if (f1 != f2)
            {
                result = false;

                string FieldName = compareDC[sKey];

                errMsg.Add(string.Format(tmp, f1, f2, FieldName));
                //追加嘗試找到控制項，給予紅底
                if (tt != null)
                {
                    tt.BackColor = System.Drawing.Color.Red;
                }

            }
        }
        //行數不同就不用比了
        if (KM1.Rows.Count != KM2.Rows.Count)
        {
            //20191030 修改：比對顯示訊息有誤 by Peggy
            //errMsg.Add("高階經理人筆數不符:1KEY :" + KM1.Rows.Count + ":2KEY :" + KM1.Rows.Count);
            errMsg.Add("高階經理人筆數不符:1KEY :" + KM1.Rows.Count + ":2KEY :" + KM2.Rows.Count);
            result = false;
            return result;
        }

        //比高階經理人
        for (int xLine = 1; xLine < 13; xLine++)
        {
            //行數不足不比對
            if (KM1.Rows.Count < xLine)
            {
                continue;
            }
            //行數不足不比對
            if (KM2.Rows.Count < xLine)
            {
                continue;
            }

            int iIdentityCheckColor = 0;
            for (int i = 0; i < KM1.Columns.Count; i++)
            {
                string f1 = KM1.Rows[xLine - 1][i].ToString();
                string f2 = KM2.Rows[xLine - 1][i].ToString();
                string sKey = KM1.Columns[i].ColumnName;
                CustTextBox tt = this.FindControl("txtSeniorManager" + sKey + "_" + xLine.ToString()) as CustTextBox;

                //如果是身分類型
                if (sKey.StartsWith("Identity"))
                {
                     tt = this.FindControl("txtSeniorManagerIdentity" + "_" + xLine.ToString()) as CustTextBox;
                     //sKey = "Identity";
                }


                if (tt != null)
                {
                    //驗證前把控制項改為白底
                    tt.BackColor = System.Drawing.Color.Empty;

                    //因為Identity總共6種類型，但是除非每一種類型都跟1key不同，不然會被重製成白色，所以才加這個判斷式
                    //if (sKey == "Identity" && iIdentityCheckColor>0)
                    if (sKey.StartsWith("Identity") && iIdentityCheckColor > 0)
                    {
                        tt.BackColor = System.Drawing.Color.Red;
                    }

                }
                if (f1 != f2)
                {
                    iIdentityCheckColor++;
                    result = false;
                    string FieldName = compareDC[sKey];
                    errMsg.Add(string.Format(tmp, f1, f2, FieldName));
                    //追加嘗試找到控制項，給予紅底
                    if (tt != null)
                    {
                        tt.BackColor = System.Drawing.Color.Red;
                    }
                }
            }
        }
        return result;

    }
    /// <summary>
    /// 建立欄位比對用字典
    /// </summary>
    private void initcompareDC()
    {
        compareDC.Add("BasicTaxID", "統一編號");
        compareDC.Add("BasicRegistyNameCH", "登記名稱(中文)");
        compareDC.Add("BasicRegistyNameEN", "登記名稱(英文)");
        compareDC.Add("BasicAMLCC", "行業編號");
        compareDC.Add("BasicEstablish", "設立日期");
        compareDC.Add("BasicCountryCode", "註冊國籍");
        compareDC.Add("BasicCountryStateCode", "勾選州別");
        compareDC.Add("BasicBookAddr1", "登記地址");
        compareDC.Add("BasicBookAddr2", "登記地址1");
        compareDC.Add("BasicBookAddr3", "登記地址2");
        compareDC.Add("BasicOfficePhone1", "公司電話");
        compareDC.Add("BasicOfficePhone2", "公司電話1");
        compareDC.Add("BasicOfficePhone3", "公司電話2");
        compareDC.Add("BasicContactMan", "聯絡人");
        compareDC.Add("BasicContactAddr1", "通訊地址");
        compareDC.Add("BasicContactAddr2", "通訊地址1");
        compareDC.Add("BasicContactAddr3", "通訊地址2");
        compareDC.Add("BasicEmail", "公司email");
        compareDC.Add("PrincipalNameCH", "中文姓名");
        compareDC.Add("PrincipalNameEn", "英文姓名");
        compareDC.Add("PrincipalBirth", "生日");
        compareDC.Add("PrincipalCountryCode", "負責人國籍");
        compareDC.Add("PrincipalIDNo", "身分證字號");
        compareDC.Add("PrincipalIssueDate", "發證日期");
        compareDC.Add("PrincipalIssuePlace", "發證地點");
        compareDC.Add("PrincipalRedemption", "領補換類別");
        compareDC.Add("PrincipalHasPic", "有無照片");
        compareDC.Add("PrincipalBookAddr1", "戶籍地址");
        compareDC.Add("PrincipalBookAddr2", "戶籍地址2");
        compareDC.Add("PrincipalBookAddr3", "戶籍地址3");
        compareDC.Add("PrincipalPassportNo", "護照號碼");
        compareDC.Add("PrincipalPassportExpdt", "護照效期");
        compareDC.Add("PrincipalContactAddr1", "通訊地址");
        compareDC.Add("PrincipalContactAddr2", "通訊地址");
        compareDC.Add("PrincipalContactAddr3", "通訊地址");
        compareDC.Add("PrincipalResidentNo", "統一證號");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
        compareDC.Add("PrincipalResidentExpdt", "統一證號效期");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
        compareDC.Add("SCCDOrganization", "法律形式");
        compareDC.Add("SCCDCountryCode", "註冊國籍");
        compareDC.Add("SCCDCountryStateCode", "勾選州別");
        compareDC.Add("SCCDForeign", "僑外資/外商");
        compareDC.Add("SCCDForeignCountryStateCode", "僑外資/外商母公司國別");
        compareDC.Add("SCCDOtherOfficeAddr1", "台灣以外主要之營業處所地址");
        compareDC.Add("SCCDOtherOfficeAddr2", "台灣以外主要之營業處所地址2");
        compareDC.Add("SCCDOtherOfficeAddr3", "台灣以外主要之營業處所地址3");
        compareDC.Add("SCCDOtherCountryCode", "主要之營業處所國別");
        compareDC.Add("SCCDIsSanction", "營業處所是否在高風險或制裁國家");
        compareDC.Add("SCCDIsSanctionCountryCode1", "高風險或制裁國家1");
        compareDC.Add("SCCDIsSanctionCountryCode2", "高風險或制裁國家2");
        compareDC.Add("SCCDIsSanctionCountryCode3", "高風險或制裁國家3");
        compareDC.Add("SCCDIsSanctionCountryCode4", "高風險或制裁國家4");
        compareDC.Add("SCCDIsSanctionCountryCode5", "高風險或制裁國家5");
        compareDC.Add("SCCDEquity", "複雜股權結構");
        compareDC.Add("SCCDCanBearerStock", "是否可發行無記名股票");
        compareDC.Add("SCCDAlreadyBearerStock", "是否已發行無記名股票");
        compareDC.Add("isSCDD", "是否已完成SCDD表");
        compareDC.Add("BasicCORP_TYPE", "公司類型");

        //20190806-RQ-2019-008595-002-長姓名需求 by Peggy
        compareDC.Add("isLongName", "負責人長姓名FLAG");
        compareDC.Add("PrincipalNameCH_L", "負責人-中文長姓名");
        compareDC.Add("PrincipalNameCH_Pinyin", "負責人-羅馬拼音");
        compareDC.Add("isLongName_c", "聯絡人長姓名FLAG");
        compareDC.Add("BasicContactMan_L", "聯絡人-中文長姓名");
        compareDC.Add("BasicContactMan_Pinyin", "聯絡人-羅馬拼音");
        
        //高階經理人
        compareDC.Add("Name", "姓名");
        compareDC.Add("Birth", "出生日期(西元)");
        compareDC.Add("CountryCode", "國籍");
        compareDC.Add("IDNo", "身分證件號碼");
        compareDC.Add("IDNoType", "身分證件類型");
        compareDC.Add("Identity", "身分類型");
        compareDC.Add("Expdt", "護照效期/統一證號期限(西元)");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
        compareDC.Add("[LineID]", "行號");
        compareDC.Add("LineID", "行號");
        compareDC.Add("isDEL", "是否刪除");

        compareDC.Add("Identity1", "身分類型1");
        compareDC.Add("Identity2", "身分類型2");
        compareDC.Add("Identity3", "身分類型3");
        compareDC.Add("Identity4", "身分類型4");
        compareDC.Add("Identity5", "身分類型5");
        compareDC.Add("Identity6", "身分類型6");

        //20190806-RQ-2019-008595-002-長姓名需求 by Peggy
        compareDC.Add("isLongName_gv", "高管人員長姓名FLAG");
        compareDC.Add("Name_L", "高管人員-中文長姓名");
        compareDC.Add("Name_Pinyin", "高管人員-羅馬拼音");

        // 20210527 EOS_AML(NOVA) by Ares Dennis start
        compareDC.Add("REG_ZIP_CODE", "地址郵遞區號");
        compareDC.Add("LAST_UPD_MAKER", "資料最後異動¬MAKER");
        compareDC.Add("LAST_UPD_CHECKER", "資料最後異動¬CHECKER");
        compareDC.Add("LAST_UPDATE_BRANCH", "資料最後異動分行");
        // 20210527 EOS_AML(NOVA) by Ares Dennis end

        compareDC.Add("LAST_UPDATE_DATE", "資料最後異動日期");
        compareDC.Add("LAST_UPDATE_SOURCE", "AML資料最後異動來源");
    }

    /// <summary>
    /// 依照傳入字串陣列比對1K 2K
    /// </summary>
    /// <param name="arrHead"></param>
    /// <param name="arrManger"></param>
    /// <param name="hs1K"></param>
    /// <param name="hs2K"></param>
    /// <returns></returns>
    private bool compareInside(string[] arrHead, Hashtable hs1K, Hashtable hs2K)
    {
        //處理HEAD
        foreach (string sHead in arrHead)
        {
            if (!hs1K[sHead].Equals(hs2K[sHead]))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    ///  以畫面上物件產生JC66用HASHTABLE
    /// </summary>
    private Hashtable buildJC66Hash(EntityAML_HeadOffice DataObj)
    {
        //建立HASHTABLE
        Hashtable hs = new Hashtable();
        //先取出所有屬性，其中有JC66對應名稱的 (總公司資料直接用映射)
        Type v = DataObj.GetType();  //取的型別實體
        PropertyInfo[] props = v.GetProperties(); //取出所有公開屬性(可以被外部存取得 
        //處理總公司資料
        foreach (PropertyInfo prop in props)
        {
            object[] attrs = prop.GetCustomAttributes(true); //取得自訂屬性，第一個物件
            AttributeRfPage authAttr;
            for (int xi = 0; xi < attrs.Length; xi++)
            {
                if (attrs[xi] is AttributeRfPage)
                {
                    authAttr = attrs[xi] as AttributeRfPage;

                    string Jc66FieldKey = authAttr.JC66NAME;
                    int Jc66Len = authAttr.JC66Len;
                    char Jc66Char = authAttr.JC6LenChar;
                    bool isPadLeft = authAttr.JC6PadLeft;
                    //JC66有值，建立HASTABLE欄位，
                    if (!string.IsNullOrEmpty(Jc66FieldKey))
                    {
                        string exVal = prop.GetValue(DataObj, null) as string;
                        if (Jc66Len > 0)
                        {
                            if (isPadLeft)
                            {
                                exVal = exVal.PadLeft(Jc66Len, Jc66Char);
                            }
                            else
                            {
                                exVal = exVal.PadRight(Jc66Len, Jc66Char);
                            }
                        }
                        hs.Add(Jc66FieldKey, exVal);
                    }
                }
            }
        }
        //總公司額外資料處理
        //合併為COMP_TEL  給予- 區分區碼 # 區分 分機
        //string strCOMP_TEL = DataObj.BasicOfficePhone1.PadRight(3) + "-" + DataObj.BasicOfficePhone2.PadRight(8) + "-" + DataObj.BasicOfficePhone3.PadRight(5);
        string strCOMP_TEL = DataObj.BasicOfficePhone1 + "-" + DataObj.BasicOfficePhone2 + "-" + DataObj.BasicOfficePhone3;
        hs["COMP_TEL"] = strCOMP_TEL;

        //20190806-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy
        hs.Add("OWNER_LNAM_FLAG", !DataObj.PrincipalNameCH_L.Trim().Equals("") ? "Y" : "N");
        hs.Add("CONTACT_LNAM_FLAG", !DataObj.BasicContactMan_L.Trim().Equals("") ? "Y" : "N");

        //逐項處理高階經理人
        //因為寫入不一定是12行，故以12處理

        foreach (EntityAML_SeniorManager seMobj in DataObj.AML_SeniorManagerColl)
        {
            //如果是刪除打勾，這一列資料就不送給主機
            if (seMobj.isDEL == "1")
            {
                continue;
            }

            //因欄位不多，不用映射 ，但須取得LINEID
            string sLineID = seMobj.LineID;
            //姓名
            hs.Add("BENE_NAME_" + sLineID, seMobj.Name);
            //生日
            hs.Add("BENE_BIRTH_DATE_" + sLineID, seMobj.Birth);
            //國籍
            hs.Add("BENE_NATION_" + sLineID, seMobj.CountryCode);

            //IDNoType
            // LoadInentity(sLineID, seMobj);
            //hs.Add("BENE_JOB_TYPE" + sLineID, seMobj.Identity1);


            hs.Add("BENE_JOB_TYPE_1_" + sLineID, seMobj.Identity1);
            hs.Add("BENE_JOB_TYPE_2_" + sLineID, seMobj.Identity2);
            hs.Add("BENE_JOB_TYPE_3_" + sLineID, seMobj.Identity3);
            hs.Add("BENE_JOB_TYPE_4_" + sLineID, seMobj.Identity4);
            hs.Add("BENE_JOB_TYPE_5_" + sLineID, seMobj.Identity5);
            hs.Add("BENE_JOB_TYPE_6_" + sLineID, seMobj.Identity6);



            //身分證別處理 1身分證 3護照 4居留證 7其他
            if (seMobj.IDNoType != "")
            {
                switch (seMobj.IDNoType)
                {
                    case "1":
                        hs.Add("BENE_ID_" + sLineID, seMobj.IDNo);
                        break;
                    case "3":
                        hs.Add("BENE_PASSPORT_" + sLineID, seMobj.IDNo);
                        hs.Add("BENE_PASSPORT_EXP_" + sLineID, seMobj.Expdt);
                        break;
                    case "4":
                        hs.Add("BENE_RESIDENT_NO_" + sLineID, seMobj.IDNo);
                        hs.Add("BENE_RESIDENT_EXP_" + sLineID, seMobj.Expdt);
                        break;
                    case "7":
                        hs.Add("BENE_OTH_CERT_" + sLineID, seMobj.IDNo);
                        hs.Add("BENE_OTH_CERT_EXP_" + sLineID, seMobj.Expdt);
                        break;
                }
            }




            if (hs.ContainsKey("BENE_NAME_" + sLineID) && !string.IsNullOrEmpty(hs["BENE_NAME_" + sLineID].ToString()) && hs.ContainsKey("BENE_NATION_" + sLineID) && !string.IsNullOrEmpty(hs["BENE_NATION_" + sLineID].ToString()))
            {
                //先依國籍判斷全半形
                if (hs["BENE_NATION_" + sLineID].ToString() == "TW")
                {
                    //全型
                    hs["BENE_NAME_" + sLineID] = ToWide(hs["BENE_NAME_" + sLineID].ToString());
                    hs["BENE_NAME_" + sLineID] = hs["BENE_NAME_" + sLineID].ToString().PadRight(19, '　');
                }
                else
                {
                    //半形
                    hs["BENE_NAME_" + sLineID] = ToNarrow(hs["BENE_NAME_" + sLineID].ToString());
                    hs["BENE_NAME_" + sLineID] = hs["BENE_NAME_" + sLineID].ToString().PadRight(40, ' ');
                }

            }

            //20190806-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy
            hs.Add("BENE_LNAM_FLAG" + sLineID, !seMobj.Name_L.Trim().Equals("") ? "Y" : "N");

        }
        //加上美國州別 若為NA，改成空白 ?
        if (hs.ContainsKey("REGISTER_US_STATE"))
        {
            if (hs["REGISTER_US_STATE"].ToString() == "NA")
            {
                hs["REGISTER_US_STATE"] = "";
            }
        }

        //設立日期 改成西元年
        if (hs.ContainsKey("BUILD_DATE"))
        {
            if (hs["BUILD_DATE"].ToString().Length == 7)
            {
                hs["BUILD_DATE"] = ConvertToDC(hs["BUILD_DATE"].ToString());
            }

        }


        //身分證發證日期 改成西元年
        if (hs.ContainsKey("OWNER_ID_ISSUE_DATE"))
        {
            if (hs["OWNER_ID_ISSUE_DATE"].ToString().Length == 7)
            {
                hs["OWNER_ID_ISSUE_DATE"] = ConvertToDC(hs["OWNER_ID_ISSUE_DATE"].ToString());
            }

        }
        //生日 改成西元年
        if (hs.ContainsKey("OWNER_BIRTH_DATE"))
        {
            if (hs["OWNER_BIRTH_DATE"].ToString().Length == 7)
            {
                hs["OWNER_BIRTH_DATE"] = ConvertToDC(hs["OWNER_BIRTH_DATE"].ToString());
            } 
        } 

        //當地址第三段打Ｘ，送出主機的資料要把第三段清空
        if (hs.ContainsKey("REG_ADDR2") && hs["REG_ADDR2"].ToString().ToUpper() == "Ｘ　　　　　　　　　　　　　")
        {
            hs["REG_ADDR2"] = "";
            hs["REG_ADDR2"] = hs["REG_ADDR2"].ToString().PadRight(14, '　');
        }

        if (hs.ContainsKey("MAILING_ADDR2") && hs["MAILING_ADDR2"].ToString().ToUpper() == "Ｘ　　　　　　　　　　　　　")
        {
            hs["MAILING_ADDR2"] = "";
            hs["MAILING_ADDR2"] = hs["MAILING_ADDR2"].ToString().PadRight(14, '　');
        }

        if (hs.ContainsKey("OWNER_ADDR2") && hs["OWNER_ADDR2"].ToString().ToUpper() == "Ｘ　　　　　　　　　　　　　")
        {
            hs["OWNER_ADDR2"] = "";
            hs["OWNER_ADDR2"] = hs["OWNER_ADDR2"].ToString().PadRight(14, '　');
        }


        return hs;
    }
    #region 20190614 Talas 修正驗證條件  暫時使用 
    /// <summary>
    /// 特殊關聯欄位處理
    /// </summary>
    /// <param name="dataObj"></param>
    /// <param name="errMsg"></param>
    private void ValidLinkedFields(EntityAML_HeadOffice dataObj, ref List<string> errMsg)
    {

        txtPrincipalIssuePlace.BackColor = System.Drawing.Color.Empty; //發證日期
        txtPrincipalIssueDate.BackColor = System.Drawing.Color.Empty;  //發證地點
        txtPrincipalRedemption.BackColor = System.Drawing.Color.Empty; //領補換別
        //txtPrincipalHasPic.BackColor = System.Drawing.Color.Empty; //有無照片
        txtPrincipalBirth.BackColor = System.Drawing.Color.Empty; //生日
        txtPrincipalPassportExpdt.BackColor = System.Drawing.Color.Empty; //護照效期
        txtPrincipalResidentExpdt.BackColor = System.Drawing.Color.Empty; //居留效期
        txtPrincipalIDNo.BackColor = System.Drawing.Color.Empty; //身分證字號
        txtBasicRegistyNameEN.BackColor = System.Drawing.Color.Empty; //英文名稱
        txtBasicCountryCode.BackColor = System.Drawing.Color.Empty; //國別
        txtBasicEstablish.BackColor = System.Drawing.Color.Empty; //設立日期


        DateTime realDate;
        string date = "";

        //行業編號不存在
        if (!string.IsNullOrEmpty(hidCC.Value))
        {
            errMsg.Add("行業編號不存在\\n");
            txtBasicAMLCC.BackColor = System.Drawing.Color.Red;
        }

        // 檢查設立日期
        string establishDate = this.txtBasicEstablish.Text.Trim();
        if (!string.IsNullOrEmpty(establishDate))
        {
            if (establishDate.Length != 7)
            {
                errMsg.Add("設立日期請輸入7碼數字\\n");
                txtBasicEstablish.BackColor = System.Drawing.Color.Red;
            }
            else
            {

                int year = int.Parse(establishDate.Substring(0, 3)) + 1911;
                date = year + "/" + establishDate.Substring(3, 2) + "/" + establishDate.Substring(5, 2);
                if (!DateTime.TryParse(date, out realDate))
                {
                    errMsg.Add("設立日期格式錯誤\\n");
                    txtBasicEstablish.BackColor = System.Drawing.Color.Red;
                }
            }
        }

        //國籍非TW，則需填英文名
        if (txtBasicCountryCode.Text.ToUpper() != "TW")
        {
            if (string.IsNullOrEmpty(txtBasicRegistyNameEN.Text))
            {
                errMsg.Add("國別非TW，需填公司英文名稱\\n");
                txtBasicRegistyNameEN.BackColor = System.Drawing.Color.Red;
            }
        }
 
        //有身分證字號，須追加檢核
        if (!string.IsNullOrEmpty(txtPrincipalIDNo.Text))
        {
            //生日空白
            if (string.IsNullOrEmpty(txtPrincipalBirth.Text))
            {
                errMsg.Add("生日不可空白\\n");
                txtPrincipalBirth.BackColor = System.Drawing.Color.Red;
            }
            else
            {
                //檢核是否為數字
                if (!isNumeric(txtPrincipalBirth.Text))
                {
                    errMsg.Add("生日需為數字\\n");
                    txtPrincipalBirth.BackColor = System.Drawing.Color.Red;
                }
            }
        }
     
        //如果負責人國籍非ＴＷ
        if (!string.IsNullOrEmpty(txtPrincipalCountryCode.Text.Trim()) && txtPrincipalCountryCode.Text.Trim().ToUpper() != "TW")
        {
            if (string.IsNullOrEmpty(txtPrincipalPassportNo.Text) && string.IsNullOrEmpty(txtPrincipalResidentNo.Text))
            {
                errMsg.Add("負責人國籍非ＴＷ，護照號碼或統一證號擇一填寫\\n");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
                txtPrincipalPassportNo.BackColor = System.Drawing.Color.Red;
                txtPrincipalResidentNo.BackColor = System.Drawing.Color.Red;
            }

            //居留證號碼不為空，居留證效期需為數字
            if (!string.IsNullOrEmpty(txtPrincipalResidentNo.Text))
            {
                if (txtPrincipalResidentNo.Text.Length != 10)
                {
                    errMsg.Add("統一證號須為10碼\\n");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
                    txtPrincipalResidentNo.BackColor = System.Drawing.Color.Red;
                }

                //20201021-202012RC 統一證號碼(新+舊)邏輯檢核
                if (!CheckResidentID(txtPrincipalResidentNo.Text.Trim()))//20201021-202012RC 統一證號碼(新+舊)邏輯檢核
                {
                    errMsg.Add("統一證號輸入錯誤，請重新輸入\\n");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
                    txtPrincipalResidentNo.BackColor = System.Drawing.Color.Red;
                }
            }
        }
    }

    #endregion 20190614 Talas 修正驗證條件  暫時使用 
    #region 20190614 Talas 修正驗證條件  原方法改名保留  ValidLinkedFields => ValidLinkedFields_BAK
    /// <summary>
    /// 特殊關聯欄位處理
    /// </summary>
    /// <param name="dataObj"></param>
    /// <param name="errMsg"></param>
    private void ValidLinkedFields_BAK(EntityAML_HeadOffice dataObj, ref List<string> errMsg)
    {

        txtPrincipalIssuePlace.BackColor = System.Drawing.Color.Empty; //發證日期
        txtPrincipalIssueDate.BackColor = System.Drawing.Color.Empty;  //發證地點
        txtPrincipalRedemption.BackColor = System.Drawing.Color.Empty; //領補換別
        //txtPrincipalHasPic.BackColor = System.Drawing.Color.Empty; //有無照片
        txtPrincipalBirth.BackColor = System.Drawing.Color.Empty; //生日
        txtPrincipalPassportExpdt.BackColor = System.Drawing.Color.Empty; //護照效期
        txtPrincipalResidentExpdt.BackColor = System.Drawing.Color.Empty; //居留效期
        txtPrincipalIDNo.BackColor = System.Drawing.Color.Empty; //身分證字號
        txtBasicRegistyNameEN.BackColor = System.Drawing.Color.Empty; //英文名稱
        txtBasicCountryCode.BackColor = System.Drawing.Color.Empty; //國別
        txtBasicEstablish.BackColor = System.Drawing.Color.Empty; //設立日期


        DateTime realDate;
        string date = "";

        //行業編號不存在
        if (!string.IsNullOrEmpty(hidCC.Value))
        {
            errMsg.Add("行業編號不存在\\n");
            txtBasicAMLCC.BackColor = System.Drawing.Color.Red;
        }

        // 檢查設立日期
        string establishDate = this.txtBasicEstablish.Text.Trim();
        if (!string.IsNullOrEmpty(establishDate))
        {
            if (establishDate.Length != 7)
            {
                errMsg.Add("設立日期請輸入7碼數字\\n");
                txtBasicEstablish.BackColor = System.Drawing.Color.Red;
            }
            else
            {

                int year = int.Parse(establishDate.Substring(0, 3)) + 1911;
                date = year + "/" + establishDate.Substring(3, 2) + "/" + establishDate.Substring(5, 2);
                if (!DateTime.TryParse(date, out realDate))
                {
                    errMsg.Add("設立日期格式錯誤\\n");
                    txtBasicEstablish.BackColor = System.Drawing.Color.Red;
                }
            }

        }



        //國籍非TW，則需填英文名
        if (txtBasicCountryCode.Text.ToUpper() != "TW")
        {
            if (string.IsNullOrEmpty(txtBasicRegistyNameEN.Text))
            {
                errMsg.Add("國別非TW，需填公司英文名稱\\n");
                txtBasicRegistyNameEN.BackColor = System.Drawing.Color.Red;
            }
        }

        //if (txtBasicCountryCode.Text != txtSCCDCountryCode.Text)
        //{
        //    errMsg.Add("總公司註冊國籍和SCDD表註冊國籍不同\\n");
        //    txtSCCDCountryCode.BackColor = System.Drawing.Color.Red;
        //}

        //if (txtBasicCountryStateCode.Text != txtSCCDCountryStateCode.Text)
        //{
        //    errMsg.Add("總公司州別和SCDD表州別不同\\n");
        //    txtSCCDCountryStateCode.BackColor = System.Drawing.Color.Red;
        //}

        //有身分證字號，須追加檢核
        if (!string.IsNullOrEmpty(txtPrincipalIDNo.Text))
        {
            //生日空白
            if (string.IsNullOrEmpty(txtPrincipalBirth.Text))
            {
                errMsg.Add("生日不可空白\\n");
                txtPrincipalBirth.BackColor = System.Drawing.Color.Red;
            }
            else
            {
                //檢核是否為數字
                if (!isNumeric(txtPrincipalBirth.Text))
                {
                    errMsg.Add("生日需為數字\\n");
                    txtPrincipalBirth.BackColor = System.Drawing.Color.Red;
                }
               
            }

            string eng = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";

            //第一碼英文，後9碼數字，一定是台灣人，所以要判斷發證日期、發證地點領補換別、有無照片
            if (eng.Contains(txtPrincipalIDNo.Text.Substring(0, 1)) && isNumeric(txtPrincipalIDNo.Text.Substring(1)))
            {
                //先檢核發證日期空白
                if (string.IsNullOrEmpty(txtPrincipalIssueDate.Text))
                {
                    errMsg.Add("身分證發證日期不可空白\\n");
                    txtPrincipalIssueDate.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    //檢核是否為數字
                    if (!isNumeric(txtPrincipalIssueDate.Text))
                    {
                        errMsg.Add("身分證發證日期需為數字\\n");
                        txtPrincipalIssueDate.BackColor = System.Drawing.Color.Red;
                    }

                }
                //發證地點，不可空白
                if (string.IsNullOrEmpty(txtPrincipalIssuePlace.Text))
                {
                    errMsg.Add("身分證發證地點不可空白\\n");
                    txtPrincipalIssuePlace.BackColor = System.Drawing.Color.Red; //發證地點
                }
                //領補換別，不可空白
                if (string.IsNullOrEmpty(txtPrincipalRedemption.Text))
                {
                    errMsg.Add("身分證領補換別不可空白\\n");
                    txtPrincipalRedemption.BackColor = System.Drawing.Color.Red; //領補換別
                }
                //有無照片，不可空白
                if (string.IsNullOrEmpty(txtPrincipalHasPic.Text))
                {
                    errMsg.Add("身分證有無照片不可空白\\n");
                    txtPrincipalHasPic.BackColor = System.Drawing.Color.Red; //有無照片
                }

            }



        }

        ////先檢核發證日期是否空白
        //if (!string.IsNullOrEmpty(txtPrincipalIssueDate.Text))
        //{
        //    //檢核是否為數字
        //    if (!isNumeric(txtPrincipalIssueDate.Text))
        //    {
        //        errMsg.Add("身分證發證日期需為數字\\n");
        //        txtPrincipalIssueDate.BackColor = System.Drawing.Color.Red;
        //    }
        //}

        //如果負責人國籍非ＴＷ
        if (!string.IsNullOrEmpty(txtPrincipalCountryCode.Text.Trim()) && txtPrincipalCountryCode.Text.Trim().ToUpper() != "TW")
        {
            if (string.IsNullOrEmpty(txtPrincipalPassportNo.Text) && string.IsNullOrEmpty(txtPrincipalResidentNo.Text))
            {
                errMsg.Add("負責人國籍非ＴＷ，護照號碼或統一證號擇一填寫\\n");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
                txtPrincipalPassportNo.BackColor = System.Drawing.Color.Red;
                txtPrincipalResidentNo.BackColor = System.Drawing.Color.Red;
            }
            //else
            //{
            //    if (!string.IsNullOrEmpty(txtPrincipalPassportNo.Text))
            //    {
            //        if (string.IsNullOrEmpty(txtPrincipalPassportExpdt.Text))
            //        {
            //            errMsg.Add("負責人國籍非ＴＷ，護照效期不可空白\\n");
            //            txtPrincipalPassportExpdt.BackColor = System.Drawing.Color.Red;
            //        }
            //    }

            //    if (!string.IsNullOrEmpty(txtPrincipalPassportExpdt.Text))
            //    {
            //        if (string.IsNullOrEmpty(txtPrincipalPassportNo.Text))
            //        {
            //            errMsg.Add("負責人國籍非ＴＷ，護照號碼不可空白\\n");
            //            txtPrincipalPassportNo.BackColor = System.Drawing.Color.Red;
            //        }
            //    }

            //    if (!string.IsNullOrEmpty(txtPrincipalResidentNo.Text))
            //    {
            //        if (string.IsNullOrEmpty(txtPrincipalResidentExpdt.Text))
            //        {
            //            errMsg.Add("負責人國籍非ＴＷ，居留效期不可空白\\n");
            //            txtPrincipalResidentExpdt.BackColor = System.Drawing.Color.Red;
            //        }
            //    }


            //    if (!string.IsNullOrEmpty(txtPrincipalResidentExpdt.Text))
            //    {
            //        if (string.IsNullOrEmpty(txtPrincipalResidentNo.Text))
            //        {
            //            errMsg.Add("負責人國籍非ＴＷ，居留證號不可空白\\n");
            //            txtPrincipalResidentNo.BackColor = System.Drawing.Color.Red;
            //        }
            //    }

            //}


        }


        DateTime realDate2;

        //護照號碼不為空，護照效期需為數字
        if (!string.IsNullOrEmpty(txtPrincipalPassportNo.Text))
        {
            //護照效期空白
            if (string.IsNullOrEmpty(txtPrincipalPassportExpdt.Text))
            {
                errMsg.Add("護照效期不可空白\\n");
                txtPrincipalPassportExpdt.BackColor = System.Drawing.Color.Red;
            }
            else
            {

                string date2 = this.txtPrincipalPassportExpdt.Text.Trim().Substring(0, 4) + "/" + this.txtPrincipalPassportExpdt.Text.Trim().Substring(4, 2) + "/" + this.txtPrincipalPassportExpdt.Text.Trim().Substring(6, 2);

                if (!DateTime.TryParse(date2, out realDate2))
                {
                    errMsg.Add("護照效期格式錯誤\\n");
                    txtPrincipalPassportExpdt.BackColor = System.Drawing.Color.Red;
                }

                ////檢核是否為數字
                //if (!isNumeric(txtPrincipalPassportExpdt.Text))
                //{
                //    errMsg.Add("護照效期需為數字\\n");
                //    txtPrincipalPassportExpdt.BackColor = System.Drawing.Color.Red;
                //}

            }
        }

        //護照效期不為空，護照就不能空
        if (!string.IsNullOrEmpty(txtPrincipalPassportExpdt.Text))
        {
            //護照號碼空白
            if (string.IsNullOrEmpty(txtPrincipalPassportNo.Text))
            {
                errMsg.Add("護照號碼不可空白\\n");
                txtPrincipalPassportNo.BackColor = System.Drawing.Color.Red;
            }
        }


        //居留證號碼不為空，居留證效期需為數字
        if (!string.IsNullOrEmpty(txtPrincipalResidentNo.Text))
        {
            if (txtPrincipalResidentNo.Text.Length != 10)
            {
                errMsg.Add("統一證號須為10碼\\n");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
                txtPrincipalResidentNo.BackColor = System.Drawing.Color.Red;
            }

            /*
            //居留證效期空白
            if (string.IsNullOrEmpty(txtPrincipalResidentExpdt.Text))
            {
                errMsg.Add("統一證號效期不可空白\\n");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
                txtPrincipalResidentExpdt.BackColor = System.Drawing.Color.Red;
            }
            else
            {
                string date2 = this.txtPrincipalResidentExpdt.Text.Trim().Substring(0, 4) + "/" + this.txtPrincipalResidentExpdt.Text.Trim().Substring(4, 2) + "/" + this.txtPrincipalResidentExpdt.Text.Trim().Substring(6, 2);

                if (!DateTime.TryParse(date2, out realDate2))
                {
                    errMsg.Add("統一證號效期格式錯誤\\n");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
                    txtPrincipalResidentExpdt.BackColor = System.Drawing.Color.Red;
                }

                ////檢核是否為數字
                //if (!isNumeric(txtPrincipalResidentExpdt.Text))
                //{
                //    errMsg.Add("居留證效期需為數字\\n");
                //    txtPrincipalResidentExpdt.BackColor = System.Drawing.Color.Red;
                //}
            }
            */
        }


        //居留證效期不為空，居留證就不能空
        if (!string.IsNullOrEmpty(txtPrincipalResidentExpdt.Text))
        {
            string date2 = this.txtPrincipalResidentExpdt.Text.Trim().Substring(0, 4) + "/" + this.txtPrincipalResidentExpdt.Text.Trim().Substring(4, 2) + "/" + this.txtPrincipalResidentExpdt.Text.Trim().Substring(6, 2);

            if (!DateTime.TryParse(date2, out realDate2))
            {
                errMsg.Add("統一證號效期格式錯誤\\n");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
                txtPrincipalResidentExpdt.BackColor = System.Drawing.Color.Red;
            }

            //居留證號空白
            if (string.IsNullOrEmpty(txtPrincipalResidentNo.Text))
            {
                errMsg.Add("統一證號不可空白\\n");//20200410-RQ-2019-030155-005-居留證號更名為統一證號
                txtPrincipalResidentNo.BackColor = System.Drawing.Color.Red;
            }
        }
    }
    #endregion 20190614 Talas 修正驗證條件  原方法改名保留  ValidLinkedFields => ValidLinkedFields_BAK

    /// <summary>
    /// 特殊欄位處理 EMAIL設定給畫面
    /// </summary>
    /// <param name="DataObj"></param>
    private void SetEmailVal(EntityAML_HeadOffice DataObj)
    {
        /*20210511-RQ-2021-004136-003 修正MAIL顯示錯誤問題
        if (DataObj.BasicEmail.IndexOf(radBasicGmail.Text) > -1)
        {
            txtBasicOfficeEmail.Text = DataObj.BasicEmail.Replace("@" + radBasicGmail.Text, "");
            radBasicGmail.Checked = true;
        }
        if (DataObj.BasicEmail.IndexOf(radBasicYahoo.Text) > -1)
        {
            txtBasicOfficeEmail.Text = DataObj.BasicEmail.Replace("@" + radBasicYahoo.Text, "");
            radBasicYahoo.Checked = true;
        }
        if (DataObj.BasicEmail.IndexOf(radBasicHotmail.Text) > -1)
        {
            txtBasicOfficeEmail.Text = DataObj.BasicEmail.Replace("@" + radBasicHotmail.Text, "");
            radBasicHotmail.Checked = true;
        }
        if (DataObj.BasicEmail.IndexOf(radBasicGmail.Text) == -1 && DataObj.BasicEmail.IndexOf(radBasicYahoo.Text) == -1 && DataObj.BasicEmail.IndexOf(radBasicHotmail.Text) == -1 && !string.IsNullOrEmpty(DataObj.BasicEmail))
        {
            //txtBasicOfficeEmailOther.Text = DataObj.BasicEmail;
            //radBasicOther.Checked = true;
            //txtBasicOfficeEmail.Text = "";

            //20190614 Talas 調整 
            if (!string.IsNullOrEmpty(DataObj.BasicEmail) && DataObj.BasicEmail.IndexOf("@") > -1)
            {
                txtBasicOfficeEmail.Text = DataObj.BasicEmail.Split('@')[0];
                txtBasicOfficeEmailOther.Text = DataObj.BasicEmail.Split('@')[1];
            }

            this.radBasicOther.Checked = true;
        }
        */

        //20210517_Ares_Stanley-修正特殊Email資料造成的異常; 20210520_Ares_Stanley-合併中信新版
        #region Email
        if (!string.IsNullOrEmpty(DataObj.BasicEmail))
        {
            txtBasicOfficeEmail.Text = DataObj.BasicEmail.Split('@')[0].Trim();
            string _mailDomain = DataObj.BasicEmail.Split('@')[1].ToLower();
            switch (_mailDomain.Trim())
            {
                case "gmail.com":
                    radBasicGmail.Checked = true;
                    break;
                case "yahoo.com.tw":
                    radBasicYahoo.Checked = true;
                    break;
                case "hotmail.com":
                    radBasicHotmail.Checked = true;
                    break;
                default:
                    this.radBasicOther.Checked = true;
                    txtBasicOfficeEmailOther.Text = _mailDomain.Trim();
                    break;
            }
        }
        #endregion Email

        //處理行業別
        if (DataObj.BasicAMLCC != "")
        {
            setCCName(DataObj.BasicAMLCC);
        }
       
        //電話號碼處理
        if (txtBasicOfficePhone2.Text.IndexOf("-") > -1)
        {
            string[] phColl = txtBasicOfficePhone2.Text.Split('-');
            if (phColl.Length == 2)
            {
                txtBasicOfficePhone1.Text = phColl[0];
                txtBasicOfficePhone2.Text = phColl[1];
            }
            if (txtBasicOfficePhone2.Text.IndexOf("#") > -1)
            {
                string[] phCollExt = txtBasicOfficePhone2.Text.Split('#');
                if (phCollExt.Length == 2)
                {
                    txtBasicOfficePhone2.Text = phCollExt[0];
                    txtBasicOfficePhone3.Text = phCollExt[1];
                }
            }
        }

        ////有無照片處理
        //if (!string.IsNullOrEmpty(DataObj.PrincipalHasPic))
        //{
        //    txtPrincipalHasPic.Text = DataObj.PrincipalHasPic == "Y" ? "1" : "2";
        //}
        //處理SCDD旗標
        if (DataObj.isSCDD == "A")
        {
            chkSCCD.Checked = true;
        }
        else
        {
            chkSCCD.Checked = false;
        }
        //如果主機登記地址第一段不是空白，那第三段就填Ｘ
        if (!string.IsNullOrEmpty(DataObj.BasicBookAddr1) && string.IsNullOrEmpty(DataObj.BasicBookAddr3))
        {
            txtBasicBookAddr3.Text = "Ｘ";
        }

        //如果主機通訊地址第一段不是空白，那第三段就填Ｘ
        if (!string.IsNullOrEmpty(DataObj.BasicContactAddr1) && string.IsNullOrEmpty(DataObj.BasicContactAddr3))
        {
            txtBasicContactAddr3.Text = "Ｘ";
        }

        //如果主機負責人戶籍地址第一段不是空白，那第三段就填Ｘ
        if (!string.IsNullOrEmpty(DataObj.PrincipalBookAddr1) && string.IsNullOrEmpty(DataObj.PrincipalBookAddr3))
        {
            txtPrincipalBookAddr3.Text = "Ｘ";
        }
        //注意，若txtBasicEstablish長度為8碼，須轉民國年
        if (txtBasicEstablish.Text.Length == 8)
        {
            txtBasicEstablish.Text = ConvertToROCYear(txtBasicEstablish.Text);
        }
        //注意，若txtBasicEstablish長度為8碼，須轉民國年
        if (txtPrincipalIssueDate.Text.Length == 8)
        {
            txtPrincipalIssueDate.Text = ConvertToROCYear(txtPrincipalIssueDate.Text);
        }
        //注意，若txtBasicEstablish長度為8碼，須轉民國年
        if (txtPrincipalBirth.Text.Length == 8)
        {
            txtPrincipalBirth.Text = ConvertToROCYear(txtPrincipalBirth.Text);
        }


    }
    /// <summary>
    /// 依傳入行業別顯示於畫面
    /// </summary>
    /// <param name="text"></param>
    private void setCCName(string text)
    {
        DataTable result = BRPostOffice_CodeType.GetCodeTypeByCodeID("3", text);
        if (result.Rows.Count > 0 && CheckCodeType(result))
        {
            HQlblHCOP_CC_Cname.Text = result.Rows[0]["CODE_NAME"].ToString();
            hidCC.Value = "";
            txtBasicAMLCC.BackColor = System.Drawing.Color.Empty;
        }
        else
        {
            txtBasicAMLCC.BackColor = Color.Red;
            HQlblHCOP_CC_Cname.Text = "";
            hidCC.Value = "error";
        }
    }
    /// <summary>
    /// 特殊處理，畫面上的EMAIL取值
    /// </summary>
    /// <param name="DataObj"></param>
    private void CollectEmail(ref EntityAML_HeadOffice DataObj)
    {
        if (radBasicGmail.Checked == true && !string.IsNullOrEmpty(txtBasicOfficeEmail.Text.Trim()))
        {
            DataObj.BasicEmail = txtBasicOfficeEmail.Text + "@" + radBasicGmail.Text;
        }
        if (radBasicYahoo.Checked == true && !string.IsNullOrEmpty(txtBasicOfficeEmail.Text.Trim()))
        {
            DataObj.BasicEmail = txtBasicOfficeEmail.Text + "@" + radBasicYahoo.Text;
        }
        if (radBasicHotmail.Checked == true && !string.IsNullOrEmpty(txtBasicOfficeEmail.Text.Trim()))
        {
            DataObj.BasicEmail = txtBasicOfficeEmail.Text + "@" + radBasicHotmail.Text;
        }
        if (radBasicOther.Checked == true && !string.IsNullOrEmpty(txtBasicOfficeEmailOther.Text.Trim()))
        {
            
          //  DataObj.BasicEmail = txtBasicOfficeEmailOther.Text;
             DataObj.BasicEmail = txtBasicOfficeEmail.Text + "@" + txtBasicOfficeEmailOther.Text;
        }

        ////有無照片處理
        //if (!string.IsNullOrEmpty(txtPrincipalHasPic.Text))
        //{
        //    DataObj.PrincipalHasPic = txtPrincipalHasPic.Text == "1" ? "Y" : "N";
        //}

        ////處理SCDD旗標
        //if (chkSCCD.Checked == true)
        //{
        //    DataObj.isSCDD = "Y";
        //}
        //else
        //{
        //    DataObj.isSCDD = "N";
        //}

        //處理SCDD旗標
        if (chkSCCD.Checked == true)
        {
            DataObj.isSCDD = "A";
        }
        else
        {
            //20210324 -RQ-2021-004136-003應姵晴要求(20210323MAIL所述，當是否已完成SCDD未勾選時，保持空白不帶N，請於下個變更異動
            //DataObj.isSCDD = "N";
            DataObj.isSCDD = "";
        }


    }
    /// <summary>
    /// 載入下拉選單內容
    /// </summary>
    private void LoadDropDownList()
    {
        DataTable result = new DataTable();
        //ListItem listItem = null;
        string aMLCC = string.Empty;
        string countryCode = string.Empty;
        string stateCode = string.Empty;
        string organization = string.Empty;
        string yNEmpty = string.Empty;
        string number = string.Empty;

        // 設定 行業編號
        SetDropAMLCCList(this.hidAMLCC, "3");

        // 設定 國籍
        SetDropCountryCodeList(this.hidCountryCode, "1");

        // 設定 州別
        SetDropStateCode(this.hidStateCode);

        // 設定 法律形式
        SetDropOrganization(this.hidOrganization, "2");

        // 設定 是、否、空白
        SetDropYNEmpty(this.hidYNEmpty);

        // 設定 數值
        SetDropNumber(this.hidNumber);
        ///高風險國家
        SetDropCountryHiRiskCodeList(null, "12");
    }

    // 設定 行業編號
    private void SetDropAMLCCList(CustHiddenField hiddenField, string type)
    {
        DataTable result = BRPostOffice_CodeType.GetCodeType(type);
        string listString = string.Empty;

        if (result != null && result.Rows.Count > 0)
        {
            for (int i = 0; i < result.Rows.Count; i++)
            {
                listString += result.Rows[i][0].ToString() + ":";
            }

            hiddenField.Value = listString;
        }
    }

    // 設定 國籍
    private void SetDropCountryCodeList(CustHiddenField hiddenField, string type)
    {
        DataTable result = BRPostOffice_CodeType.GetCodeType(type);

//20190731 修改：將result Table排序 by Peggy
        DataView dv = result.DefaultView;
        dv.Sort = "CODE_ID asc";
        result = dv.ToTable();

        ListItem listItem = new ListItem();
        string listString = string.Empty;

        if (result != null && result.Rows.Count > 0)
        {
            for (int i = 0; i < result.Rows.Count; i++)
            {
                listItem = new ListItem();

                listItem.Value = result.Rows[i][0].ToString();
                listItem.Text = result.Rows[i][0].ToString();

                listString += result.Rows[i][0].ToString() + ":";

                this.dropBasicCountryCode.Items.Add(listItem);
                this.dropPrincipalCountryCode.Items.Add(listItem);
                this.dropSCCDCountryCode.Items.Add(listItem);
                this.dropSCCDForeignCountryStateCode.Items.Add(listItem);
                this.dropSCCDOtherCountryCode.Items.Add(listItem);

            }

            hiddenField.Value = listString;
        }
    }
    // 設定 高風險國籍
    private void SetDropCountryHiRiskCodeList(CustHiddenField hiddenField, string type)
    {
        DataTable result = BRPostOffice_CodeType.GetCodeType(type);

//20190731 修改：將result Table排序 by Peggy
        DataView dv = result.DefaultView;
        dv.Sort = "CODE_ID asc";
        result = dv.ToTable();

        ListItem listItem = new ListItem();
        string listString = string.Empty;

        if (result != null && result.Rows.Count > 0)
        {
            for (int i = 0; i < result.Rows.Count; i++)
            {
                listItem = new ListItem();

                listItem.Value = result.Rows[i][0].ToString();
                listItem.Text = result.Rows[i][0].ToString();


                this.dropSCCDIsSanctionCountryCode1.Items.Add(listItem);
                this.dropSCCDIsSanctionCountryCode2.Items.Add(listItem);
                this.dropSCCDIsSanctionCountryCode3.Items.Add(listItem);
                this.dropSCCDIsSanctionCountryCode4.Items.Add(listItem);
                this.dropSCCDIsSanctionCountryCode5.Items.Add(listItem);
            }


        }
    }

    // 設定 州別
    private void SetDropStateCode(CustHiddenField hiddenField)
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

            this.dropBasicCountryStateCode.Items.Add(listItem);
            this.dropSCCDCountryStateCode.Items.Add(listItem);
        }

        hiddenField.Value = listString;
    }

    // 設定 法律形式
    private void SetDropOrganization(CustHiddenField hiddenField, string type)
    {
        DataTable result = BRPostOffice_CodeType.GetCodeType(type);
        ListItem listItem = new ListItem();
        string listString = string.Empty;

        if (result != null && result.Rows.Count > 0)
        {
            for (int i = 0; i < result.Rows.Count; i++)
            {
                listItem = new ListItem();

                listItem.Value = result.Rows[i][0].ToString();
                listItem.Text = result.Rows[i][0].ToString();

                listString += result.Rows[i][0].ToString() + ":";

                this.dropSCCDOrganization.Items.Add(listItem);
            }

            hiddenField.Value = listString;
        }
    }

    // 設定 是、否、空白
    private void SetDropYNEmpty(CustHiddenField hiddenField)
    {
        ListItem listItem = new ListItem();
        string listString = string.Empty;

        string[] arr = { ";", "是;Y", "否;N" };
        string[] arrs = null;

        for (int i = 0; i < arr.Length; i++)
        {
            arrs = arr[i].Split(';');

            listItem = new ListItem();

            listItem.Value = arrs[0].ToString();
            listItem.Text = arrs[1].ToString();

            listString += arrs[1].ToString() + ":";

            this.dropSCCDForeign.Items.Add(listItem);
            this.dropSCCDIsSanction.Items.Add(listItem);
            this.dropSCCDCanBearerStock.Items.Add(listItem);
            this.dropSCCDAlreadyBearerStock.Items.Add(listItem);
            //複雜股權結構
            this.dropSCCDEquity.Items.Add(listItem);
        }

        hiddenField.Value = listString;
    }

    // 設定 數值
    private void SetDropNumber(CustHiddenField hiddenField)
    {
        ListItem listItem = new ListItem();
        string listString = string.Empty;

        string[] arr = { "是;1", "否;2" };
        string[] arrs = null;

        for (int i = 0; i < arr.Length; i++)
        {
            arrs = arr[i].Split(';');

            listItem = new ListItem();

            listItem.Value = arrs[0].ToString();
            listItem.Text = arrs[1].ToString();

            listString += arrs[1].ToString() + ":";

            // this.dropSCCDEquity.Items.Add(listItem);
        }

        hiddenField.Value = listString;
    }

    /// <summary>
    /// 透過SetVal方法，指定清除屬性對應之控制項VALUE
    /// </summary>
    void clearDataObjVal()
    {
        //本業會用到的物件，宣告後呼叫SETVAL即可，注意:若為多行對應，仍需以迴圈逐筆呼叫
        EntityAML_HeadOffice DataObj = new EntityAML_HeadOffice();
        this.SetVal(DataObj, true);
        //讀取子項目

        for (int i = 0; i < 12; i++)
        {
            EntityAML_SeniorManager dObj = new EntityAML_SeniorManager();
            dObj.LineID = (i + 1).ToString();
            this.SetVal(dObj, true);

        }
        // 20210527 EOS_AML(NOVA) by Ares Jack 郵遞區號反灰不能修改
        txtREG_ZIP_CODE.Enabled = false;
        txtREG_ZIP_CODE.BackColor = Color.LightGray;
    }
    /// <summary>
    /// 處理高階經理人國籍與姓名
    /// </summary>
    /// <param name="dObj"></param>
    /// <param name="errMsg"></param>
    /// <param name="lineID"></param>
    private void checkNation(EntityAML_SeniorManager dObj, ref List<string> errMsg, string lineID)
    {
        CustTextBox CName = FindControl("txtSeniorManagerName_" + lineID) as CustTextBox; //姓名
        CustTextBox CBirth = FindControl("txtSeniorManagerBirth_" + lineID) as CustTextBox; //生日
        CustTextBox CID = FindControl("txtSeniorManagerIDNo_" + lineID) as CustTextBox; //身分證字號
        CustTextBox CNation = FindControl("txtSeniorManagerCountryCode_" + lineID) as CustTextBox; //國籍;
        CustTextBox CIDType = FindControl("txtSeniorManagerIDNoType_" + lineID) as CustTextBox; //身分證件類型;
        CustTextBox CExpDt = FindControl("txtSeniorManagerExpdt_" + lineID) as CustTextBox; //期限(西元);
        CustTextBox CIdentity = FindControl("txtSeniorManagerIdentity_" + lineID) as CustTextBox; //身分類型;
        CustLabel lblEnNotice = FindControl("lblEnNotice_" + lineID) as CustLabel; //姓名提示文字

        //20190806-RQ-2019-008595-002-長姓名需求:檢核若長姓名flag為勾選時，中文長姓名為必輸 by Peggy
        CustCheckBox cLongNameFlag = FindControl("chkisLongName_gdv_" + lineID) as CustCheckBox;//長姓名flag
        CustTextBox CNameL = FindControl("txtName_L_" + lineID) as CustTextBox; //中文長姓名
        CustTextBox CNamePinyin = FindControl("txtName_Pinyin_" + lineID) as CustTextBox; //羅馬拼音

        //直接不顯示，未來如果要檢核，也加在這
        lblEnNotice.Visible = false;

        //復歸底色
        CName.BackColor = Color.Empty;
        CID.BackColor = Color.Empty;
        CNation.BackColor = Color.Empty;
        CIDType.BackColor = Color.Empty;
        CExpDt.BackColor = Color.Empty;
        CIdentity.BackColor = Color.Empty;

        //任一欄有值，身分類型不能為空白
        if (!(string.IsNullOrEmpty(CName.Text) && string.IsNullOrEmpty(CBirth.Text) && string.IsNullOrEmpty(CNation.Text) && string.IsNullOrEmpty(CID.Text) && string.IsNullOrEmpty(CIDType.Text) && string.IsNullOrEmpty(CExpDt.Text)) && string.IsNullOrEmpty(CIdentity.Text))
        {
            errMsg.Add("身分類型不可為空白\\n");
            CIdentity.BackColor = Color.Red;
        }

        //先檢核 有國籍，姓名不能空
        if (string.IsNullOrEmpty(CName.Text) && !string.IsNullOrEmpty(CNation.Text))
        {
            errMsg.Add("輸入國籍，姓名不可為空白\\n");
            CName.BackColor = Color.Red;
        }
        //先檢核 有姓名，國籍不能空
        if (!string.IsNullOrEmpty(CName.Text) && string.IsNullOrEmpty(CNation.Text))
        {
            errMsg.Add("輸入姓名，國籍不可為空白\\n");
            CNation.BackColor = Color.Red;
        }
        if (!string.IsNullOrEmpty(CName.Text) && !string.IsNullOrEmpty(CNation.Text))
        {
            //先依國籍判斷全半形
            if (CNation.Text.ToUpper() == "TW")
            {
                //追加檢查身分證格式 
                //20190909 修改：10月RC要求-高管身分證號檢核需排除id為Z999999999這帳號  by Peggy
                //20210629_Ares_Stanley-關聯人無ID時不能報Z999999999，改用虛擬ID規範【CA+YYYYMMDD+國籍(兩碼英文)+序號(兩碼01-99,AA-ZZ)
                //20211029_Ares_Jack_虛擬ID延後上線, 先還原Z999999999
                //if (!checkVirutalID(CID.Text.Trim()) && !isIdentificationId(CID.Text))
                if (!CID.Text.Trim().Equals("Z999999999") && !isIdentificationId(CID.Text))
                {
                    errMsg.Add("身分證號碼無效\\n");
                    CID.BackColor = Color.Red;
                }
                //全型
                CName.Text = ToWide(CName.Text);
                if (CName.Text.Length > 19)
                {
                    errMsg.Add("姓名欄位太長\\n");
                    CName.BackColor = Color.Red;
                }
                else
                {
                    CName.Text = CName.Text.PadRight(19, '　');
                    dObj.Name = CName.Text;//20200713
                }

                if (!string.IsNullOrEmpty(CIDType.Text) && !(CIDType.Text == "1" || CIDType.Text == "7"))
                {
                    errMsg.Add("國籍為TW，身分證件類型須為1或7\\n");
                    CIDType.BackColor = Color.Red;
                }
            }
            else
            {
                //半形
                CName.Text = ToNarrow(CName.Text);
                if (CName.Text.Length > 40)
                {
                    errMsg.Add("姓名欄位太長\\n");
                    CName.BackColor = Color.Red;
                }
                else
                {
                    CName.Text = CName.Text.PadRight(40, ' ');
                    dObj.Name = CName.Text;//20200713
                }
            }
            //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
            ////20211022_Ares_Jack_檢核出生日期跟虛擬ID相符
            //if (CIDType.Text == "7" && CID.Text.Trim().Substring(0, 2) == "CA" && CID.Text.Trim().Length == 14)
            //{
            //    if (CID.Text.Trim().Substring(2, 8) != CBirth.Text.Trim())
            //    {
            //        errMsg.Add("生日與虛擬ID不符\\n");
            //        CID.BackColor = Color.Red;
            //        CBirth.BackColor = Color.Red;
            //    }
            //}
            //20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis
        }

        //同時有值，證件為必填
        if (!string.IsNullOrEmpty(CName.Text) && !string.IsNullOrEmpty(CNation.Text) && string.IsNullOrEmpty(CID.Text))
        {
            errMsg.Add("身分證件號碼不可為空白\\n");
            CID.BackColor = Color.Red;
        }


        ////同時有值，身分類型為必填
        //if (!string.IsNullOrEmpty(CName.Text) && !string.IsNullOrEmpty(CNation.Text) && string.IsNullOrEmpty(CIdentity.Text))
        //{
        //    errMsg.Add("輸入姓名和國籍時，身分證件號碼不可為空白\\n");
        //    CID.BackColor = Color.Red;
        //}

        //檢核證件有填，身分證件類型必填
        if (!string.IsNullOrEmpty(CID.Text) && string.IsNullOrEmpty(CIDType.Text))
        {
            errMsg.Add("身分證件類型不可為空白\\n");
            CIDType.BackColor = Color.Red;
        }

        //檢核身分別有填，身分證件號碼必填
        if (!string.IsNullOrEmpty(CIDType.Text) && string.IsNullOrEmpty(CID.Text))
        {
            errMsg.Add("身分證件號碼不可為空白\\n");
            CIDType.BackColor = Color.Red;
        }

        //檢核證件有填，身分證件類型有填
        if (!string.IsNullOrEmpty(CIDType.Text) && !string.IsNullOrEmpty(CID.Text))
        {
            //身分證件類型3 or 4，護照效期/居留期限(西元)必填
            /*20191007 -10月RC modify by Peggy
            if ((CIDType.Text == "3" || CIDType.Text == "4") && string.IsNullOrEmpty(CExpDt.Text))
            {
                errMsg.Add("護照效期/居留期限(西元)不可為空白\\n");
                CExpDt.BackColor = Color.Red;
            }
            */
            //如果是身分證件類型1 or 4，則身分證件號碼一定要是10碼
            if ((CIDType.Text == "1" || CIDType.Text == "4") && CID.Text.Length != 10)
            {
                errMsg.Add("身分證件類型1或4時，身分證件號碼須為10碼\\n");
                CID.BackColor = Color.Red;
            }
        }

        //20190806-RQ-2019-008595-002-長姓名需求 by Peggy
        //若長姓名為勾選狀態，則中文長姓名必需有值
        if (cLongNameFlag.Checked)
        {
            if (CNameL.Text.Trim().Equals(""))
            {
                errMsg.Add("當長姓名打勾時，中文長姓名為必要輸入\\n");
                CNameL.Focus();
                CNameL.BackColor = Color.Red;
            }

            if (CNamePinyin.Text.Trim().Equals(""))
            {
                errMsg.Add("當長姓名打勾時，羅馬拼音為必要輸入\\n");
                CNamePinyin.Focus();
                CNamePinyin.BackColor = Color.Red;
            }

            if ((ToWide(CNameL.Text.Trim()).Length + LongNameRomaClean(CNamePinyin.Text).Trim().Length) < 5)
            {
                errMsg.Add("當長姓名打勾時，中文長姓名+羅馬拼音 需超過4個字以上\\n");
                CNameL.Focus();
                CNameL.BackColor = Color.Red;
                return;
            }
        }
    }

    private void UpdateJC66(string TaxID, string sIsNew, EntityAML_HeadOffice DataObj)
    {
        //建立HASHTABLE
        Hashtable JC66QueryOBj = new Hashtable();
        JC66QueryOBj.Add("FUNCTION_CODE", "I");
        JC66QueryOBj.Add("CORP_NO", txtTaxID.Text);
        JC66QueryOBj.Add("CORP_SEQ", "0000");


        Hashtable hstQueryP4A;
        Hashtable QueryJC68_Principal = new Hashtable();//主機資料-負責人JC68
        Hashtable QueryJC68_CONTACT = new Hashtable();//主機資料-聯絡人JC68
        string mLongName = "N";//20190912 modify by Peggy
        string mContactLName = "N";//20190912 modify by Peggy

        if (!isTest)
        {
            //上送主機資料
            hstQueryP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC66, JC66QueryOBj, false, "11", eAgentInfo);
            QueryJC68_Principal = QueryJC68(this.txtPrincipalIDNo.Text.Trim());
            QueryJC68_CONTACT = QueryJC68(this.txtTaxID.Text.Trim()+"0000");
        }
        else
        {
            //測試用模擬資料
            hstQueryP4A = GetTestData();
        }

        // 20210527 EOS_AML(NOVA) by Ares Dennis start
        Hashtable htgJC66 = hstQueryP4A;// 主機資料
        // 20210527 EOS_AML(NOVA) by Ares Dennis end

        //MESSAGE_TYPE=0006代表新增以前主機查不到資料，所以還是要讓他繼續往下做
        if ((hstQueryP4A["MESSAGE_TYPE"] != null && hstQueryP4A["MESSAGE_TYPE"].ToString() == "0006") || (!hstQueryP4A.Contains("HtgMsg")))
        {
            
            //base.strHostMsg += hstQueryP4A["HtgSuccess"].ToString();//*主機返回成功訊息
            base.strClientMsg += MessageHelper.GetMessage("01_01050101_010");

            DataTable dtblUpdateData = CommonFunction.GetDataTable();
            DataTable dtblUpdateData_JC68 = CommonFunction.GetDataTable();
            int _LNameCompare = 0;//接收中文長姓名異動比對結果
            int _PinyinCompare = 0;//接收羅馬拼音異動比對結果
            int _LnameFlag = 0;//接收flag異動比對結果

            //20190806-RQ-2019-008595-002-長姓名需求，新增4個欄位 by Peggy
            if (!string.IsNullOrEmpty(hstQueryP4A["OWNER_LNAM_FLAG"].ToString().Trim()))
            {
                mLongName = hstQueryP4A["OWNER_LNAM_FLAG"].ToString().Trim();
            }
            if (!string.IsNullOrEmpty(hstQueryP4A["CONTACT_LNAM_FLAG"].ToString().Trim()))
            {
                mContactLName = hstQueryP4A["CONTACT_LNAM_FLAG"].ToString().Trim();
            }

            List<EntityHTG_JC68> _JC68s = new List<EntityHTG_JC68>();
            EntityHTG_JC68 _tmpJC68 = new EntityHTG_JC68();
            
            //*比對統一編號
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtBasicTaxID.Text.Trim(), "CORP_NO", BaseHelper.GetShowText("01_01050100_007"));
            //*比對登記名稱(中文)
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtBasicRegistyNameCH.Text.Trim(), "REG_NAME", BaseHelper.GetShowText("01_01050100_008"));
            //*比對登記名稱(英文)
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtBasicRegistyNameEN.Text.Trim(), "CORP_REG_ENG_NAME", BaseHelper.GetShowText("01_01050100_009"));
            //*比對行業編號
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtBasicAMLCC.Text.Trim(), "CC", BaseHelper.GetShowText("01_01050100_010"));

            if (hstQueryP4A.ContainsKey("BUILD_DATE"))
            {
                hstQueryP4A["BUILD_DATE"] = ConvertToROCYear(hstQueryP4A["BUILD_DATE"].ToString());
            }

            //*比對設立日期
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtBasicEstablish.Text.Trim(), "BUILD_DATE", BaseHelper.GetShowText("01_01050100_012"));
            //*比對註冊國籍
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtBasicCountryCode.Text.Trim(), "REGISTER_NATION", BaseHelper.GetShowText("01_01050100_013"));
            //*比對註冊國為美國者，請勾選州別
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtBasicCountryStateCode.Text.Trim(), "REGISTER_US_STATE", BaseHelper.GetShowText("01_01050100_014"));
            //*比對郵遞區號
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtREG_ZIP_CODE.Text.Trim(), "REG_ZIP_CODE", BaseHelper.GetShowText("01_01050100_070"));
            //*比對登記地址1
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtBasicBookAddr1.Text.Trim(), "REG_CITY", BaseHelper.GetShowText("01_01050100_015"));
            //*比對登記地址2
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtBasicBookAddr2.Text.Trim(), "REG_ADDR1", BaseHelper.GetShowText("01_01050100_015"));
            //*比對登記地址3
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtBasicBookAddr3.Text.Trim(), "REG_ADDR2", BaseHelper.GetShowText("01_01050100_015"));

            //string sOfficePhone = this.txtBasicOfficePhone1.Text.PadRight(3) + "-" + this.txtBasicOfficePhone2.Text.PadRight(8) + "-" + this.txtBasicOfficePhone3.Text.Trim();
            string sOfficePhone = this.txtBasicOfficePhone1.Text + "-" + this.txtBasicOfficePhone2.Text + "-" + this.txtBasicOfficePhone3.Text.Trim();

            //*比對公司電話
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, sOfficePhone, "COMP_TEL", BaseHelper.GetShowText("01_01050100_016"));
            //*比對聯絡人
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtBasicContactMan.Text.Trim(), "CONTACT_NAME", BaseHelper.GetShowText("01_01050100_017"));

            //*比對通訊地址1
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtBasicContactAddr1.Text, "MAILING_CITY", BaseHelper.GetShowText("01_01050100_018"));
            //*比對通訊地址2
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtBasicContactAddr2.Text, "MAILING_ADDR1", BaseHelper.GetShowText("01_01050100_018"));
            //*比對通訊地址3
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtBasicContactAddr3.Text, "MAILING_ADDR2", BaseHelper.GetShowText("01_01050100_018"));

            string sEmail=string.Empty;
            if (radBasicGmail.Checked == true && !string.IsNullOrEmpty(txtBasicOfficeEmail.Text.Trim()))
            {
                sEmail = txtBasicOfficeEmail.Text.Trim() + "@" + radBasicGmail.Text;
            }
            if (radBasicYahoo.Checked == true && !string.IsNullOrEmpty(txtBasicOfficeEmail.Text.Trim()))
            {
                sEmail = txtBasicOfficeEmail.Text.Trim() + "@" + radBasicYahoo.Text;
            }
            if (radBasicHotmail.Checked == true && !string.IsNullOrEmpty(txtBasicOfficeEmail.Text.Trim()))
            {
                sEmail = txtBasicOfficeEmail.Text.Trim() + "@" + radBasicHotmail.Text;
            }
            if (radBasicOther.Checked == true && !string.IsNullOrEmpty(txtBasicOfficeEmailOther.Text.Trim()))
            {
                //sEmail = txtBasicOfficeEmailOther.Text.Trim();
                sEmail = txtBasicOfficeEmail.Text + "@" + txtBasicOfficeEmailOther.Text;
            }

            //*比對公司Email
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, sEmail, "EMAIL", BaseHelper.GetShowText("01_01050100_019"));
            //*比對負責人中文姓名
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtPrincipalNameCH.Text.Trim(), "OWNER_CHINESE_NAME", BaseHelper.GetShowText("01_01050100_021"));
            //*比對負責人英文姓名
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtPrincipalNameEn.Text.Trim(), "OWNER_ENGLISH_NAME", BaseHelper.GetShowText("01_01050100_022"));

            if (hstQueryP4A.ContainsKey("OWNER_BIRTH_DATE"))
            {
                hstQueryP4A["OWNER_BIRTH_DATE"] = ConvertToROCYear(hstQueryP4A["OWNER_BIRTH_DATE"].ToString());
            }

            //*比對負責人生日
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtPrincipalBirth.Text.Trim(), "OWNER_BIRTH_DATE", BaseHelper.GetShowText("01_01050100_023"));
            //*比對負責人國籍
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtPrincipalCountryCode.Text.Trim(), "OWNER_NATION", BaseHelper.GetShowText("01_01050100_024"));
            //*比對負責人身分證字號
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtPrincipalIDNo.Text.Trim(), "OWNER_ID", BaseHelper.GetShowText("01_01050100_025"));

            if (hstQueryP4A.ContainsKey("OWNER_ID_ISSUE_DATE"))
            {
                hstQueryP4A["OWNER_ID_ISSUE_DATE"] = ConvertToROCYear(hstQueryP4A["OWNER_ID_ISSUE_DATE"].ToString());
            }
            
            //*比對負責人發證日期
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtPrincipalIssueDate.Text.Trim(), "OWNER_ID_ISSUE_DATE", BaseHelper.GetShowText("01_01050100_026"));
            //*比對負責人發證地點
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtPrincipalIssuePlace.Text.Trim(), "OWNER_ID_ISSUE_PLACE", BaseHelper.GetShowText("01_01050100_027"));
            //*比對負責人領補換類別
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtPrincipalRedemption.Text.Trim(), "OWNER_ID_REPLACE_TYPE", BaseHelper.GetShowText("01_01050100_028"));
            //*比對負責人有無照片
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtPrincipalHasPic.Text.Trim(), "ID_PHOTO_FLAG", BaseHelper.GetShowText("01_01050100_029"));
            //*比對負責人戶籍地址1
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtPrincipalBookAddr1.Text.Trim(), "OWNER_CITY", BaseHelper.GetShowText("01_01050100_030"));
            //*比對負責人戶籍地址2
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtPrincipalBookAddr2.Text.Trim(), "OWNER_ADDR1", BaseHelper.GetShowText("01_01050100_030"));
            //*比對負責人戶籍地址3
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtPrincipalBookAddr3.Text.Trim(), "OWNER_ADDR2", BaseHelper.GetShowText("01_01050100_030"));
            //*比對護照號碼
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtPrincipalPassportNo.Text.Trim(), "PASSPORT", BaseHelper.GetShowText("01_01050100_031"));
            //*比對護照效期
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtPrincipalPassportExpdt.Text.Trim(), "PASSPORT_EXP_DATE", BaseHelper.GetShowText("01_01050100_032"));
            //*比對居留證號
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtPrincipalResidentNo.Text.Trim(), "RESIDENT_NO", BaseHelper.GetShowText("01_01050100_034"));
            //*比對居留效期
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtPrincipalResidentExpdt.Text.Trim(), "RESIDENT_EXP_DATE", BaseHelper.GetShowText("01_01050100_035"));
            
            //*比對SCDD表法律形式
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtSCCDOrganization.Text.Trim(), "BUSINESS_ORGAN_TYPE", BaseHelper.GetShowText("01_01050100_037"));
            //*比對SCDD表註冊國籍
            //CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtSCCDCountryCode.Text.Trim(), "REGISTER_NATION", this.txtTaxID.Text + BaseHelper.GetShowText("01_01050100_038"));
            //*比對SCDD表僑外資/外商
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtSCCDForeign.Text.Trim(), "OVERSEAS_FOREIGN", BaseHelper.GetShowText("01_01050100_040"));
            //*比對SCDD表僑外資/外商母公司國別
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtSCCDForeignCountryStateCode.Text.Trim(), "OVERSEAS_FOREIGN_COUNTRY", BaseHelper.GetShowText("01_01050100_041"));
            //*比對SCDD表主要之營業處所國別
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtSCCDOtherCountryCode.Text.Trim(), "PRIMARY_BUSI_COUNTRY", BaseHelper.GetShowText("01_01050100_043"));
            //*比對SCDD表營業處所是否在高風險或制裁國家
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtSCCDIsSanction.Text.Trim(), "BUSI_RISK_NATION_FLAG", BaseHelper.GetShowText("01_01050100_044"));
            //*比對SCDD表高風險或制裁國別1
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtSCCDIsSanctionCountryCode1.Text.Trim(), "BUSI_RISK_NATION_1", BaseHelper.GetShowText("01_01050100_045"));
            //*比對SCDD表高風險或制裁國別2
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtSCCDIsSanctionCountryCode2.Text.Trim(), "BUSI_RISK_NATION_2", BaseHelper.GetShowText("01_01050100_045"));
            //*比對SCDD表高風險或制裁國別3
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtSCCDIsSanctionCountryCode3.Text.Trim(), "BUSI_RISK_NATION_3", BaseHelper.GetShowText("01_01050100_045"));
            //*比對SCDD表高風險或制裁國別4
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtSCCDIsSanctionCountryCode4.Text.Trim(), "BUSI_RISK_NATION_4", BaseHelper.GetShowText("01_01050100_045"));
            //*比對SCDD表高風險或制裁國別5
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtSCCDIsSanctionCountryCode5.Text.Trim(), "BUSI_RISK_NATION_5", BaseHelper.GetShowText("01_01050100_045"));
            //*比對SCDD表複雜股權結構
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtSCCDEquity.Text.Trim(), "COMPLEX_STR_CODE", BaseHelper.GetShowText("01_01050100_046"));
            //*比對SCDD表是否可發行無記名股票
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtSCCDCanBearerStock.Text.Trim(), "ALLOW_ISSUE_STOCK_FLAG", BaseHelper.GetShowText("01_01050100_047"));
            //*比對SCDD表是否已發行無記名股票
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtSCCDAlreadyBearerStock.Text.Trim(), "ISSUE_STOCK_FLAG", BaseHelper.GetShowText("01_01050100_048"));
            //*比對SCDD表是否已完成SCDD表
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, DataObj.isSCDD, "EXAMINE_FLAG", BaseHelper.GetShowText("01_01050100_049"));
            //*比對資料最後異動MAKER
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtLAST_UPD_MAKER.Text.Trim(), "LAST_UPD_MAKER", BaseHelper.GetShowText("01_01050100_071"));
            //*比對資料最後異動CHECKER
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtLAST_UPD_CHECKER.Text.Trim(), "LAST_UPD_CHECKER", BaseHelper.GetShowText("01_01050100_072"));
            //*比對資料最後異動分行
            CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, this.txtLAST_UPD_BRANCH.Text.Trim(), "LAST_UPDATE_BRANCH", BaseHelper.GetShowText("01_01050100_073"));

            //20190806-RQ-2019-008595-002-長姓名需求 by Peggy
            //比對負責人中文長姓名FLAG
            //20190912 modify by Peggy
            _LnameFlag = 0;
            if (hstQueryP4A.Contains("OWNER_LNAM_FLAG") && string.IsNullOrEmpty(hstQueryP4A["OWNER_LNAM_FLAG"].ToString().Trim()))
            {
                hstQueryP4A["OWNER_LNAM_FLAG"] = "N";
            }
            _LnameFlag = CommonFunction.ContrastData(hstQueryP4A, dtblUpdateData, chkisLongName.Checked ? "Y" : "N", "OWNER_LNAM_FLAG", BaseHelper.GetShowText("01_01030200_116"));

            if (chkisLongName.Checked || _LnameFlag != 0)//假設現在為長姓名，或為現在FLAG狀態與主機不同時，才做異動比對與異動長姓名
            {
                //負責人長姓名
                _LNameCompare = CommonFunction.ContrastDataTwoNew(QueryJC68_Principal, dtblUpdateData_JC68, this.txtPrincipalNameCH_L.Text.Trim(), "LONG_NAME", BaseHelper.GetShowText("01_01030200_117"));
                //負責人羅馬拼音
                _PinyinCompare = CommonFunction.ContrastDataTwoNew(QueryJC68_Principal, dtblUpdateData_JC68, LongNameRomaClean(txtPrincipalNameCH_Pinyin.Text.Trim()), "PINYIN_NAME", BaseHelper.GetShowText("01_01030200_118"));
            }
            
            if (_LNameCompare != 0 || _PinyinCompare != 0)//有差異才需打JC68
            {
                if (chkisLongName.Checked)//負責人
                {
                    _tmpJC68 = new EntityHTG_JC68();
                    _tmpJC68.ID = txtPrincipalIDNo.Text.Trim();
                    _tmpJC68.LONG_NAME = ToWide(txtPrincipalNameCH_L.Text.Trim());
                    _tmpJC68.PINYIN_NAME = LongNameRomaClean(txtPrincipalNameCH_Pinyin.Text.Trim());

                    _JC68s.Add(_tmpJC68);

                }
                else
                {
                    if (mLongName.Trim().Equals("Y"))//長變短
                    {
                        _tmpJC68 = new EntityHTG_JC68();
                        _tmpJC68.ID = txtPrincipalIDNo.Text.Trim();
                        _tmpJC68.LONG_NAME = "";
                        _tmpJC68.PINYIN_NAME = "";

                        _JC68s.Add(_tmpJC68);
                    }
                }
            }

            // 比對聯絡人中文長姓名FLAG
            //20190912 modify by Peggy
            _LnameFlag = 0;
            if (hstQueryP4A.Contains("CONTACT_LNAM_FLAG") && string.IsNullOrEmpty(hstQueryP4A["CONTACT_LNAM_FLAG"].ToString().Trim()))
            {
                hstQueryP4A["CONTACT_LNAM_FLAG"] = "N";
            }
            _LnameFlag = CommonFunction.ContrastData(hstQueryP4A, dtblUpdateData, chkisLongName_c.Checked ? "Y" : "N", "CONTACT_LNAM_FLAG", BaseHelper.GetShowText("01_01030200_120"));

            _LNameCompare = 0;//接收異動比對結果
            _PinyinCompare = 0;
            if (chkisLongName_c.Checked || _LnameFlag != 0)//假設現在為長姓名，或為現在FLAG狀態與主機不同時，才做異動比對與異動長姓名
            {
                //聯絡人長姓名
                _LNameCompare = CommonFunction.ContrastDataTwoNew(QueryJC68_CONTACT, dtblUpdateData_JC68, this.txtBasicContactMan_L.Text.Trim(), "LONG_NAME", BaseHelper.GetShowText("01_01030200_120"));
                //聯絡人羅馬拼音
                _PinyinCompare = CommonFunction.ContrastDataTwoNew(QueryJC68_CONTACT, dtblUpdateData_JC68, LongNameRomaClean(txtBasicContactMan_Pinyin.Text.Trim()), "PINYIN_NAME", BaseHelper.GetShowText("01_01030200_121"));
            }
            
            if (_LNameCompare != 0 || _PinyinCompare != 0)//有差異才需打JC68
            {
                if (chkisLongName_c.Checked)//聯絡人
                {
                    _tmpJC68 = new EntityHTG_JC68();
                    _tmpJC68.ID = txtTaxID.Text.Trim() + "0000";
                    _tmpJC68.LONG_NAME = ToWide(txtBasicContactMan_L.Text.Trim());
                    _tmpJC68.PINYIN_NAME = LongNameRomaClean(txtBasicContactMan_Pinyin.Text.Trim());

                    _JC68s.Add(_tmpJC68);
                }
                else
                {
                    if (mContactLName.Trim().Equals("Y"))
                    {
                        _tmpJC68 = new EntityHTG_JC68();
                        _tmpJC68.ID = txtTaxID.Text.Trim() + "0000";
                        _tmpJC68.LONG_NAME = "";
                        _tmpJC68.PINYIN_NAME = "";

                        _JC68s.Add(_tmpJC68);
                    }
                }
            }

            string sLineID = "";
            //讀取子項目
            List<EntityAML_SeniorManager> managerColl = DataObj.AML_SeniorManagerColl;
            
            for (int i = 1; i < 13; i++)
            {
                sLineID = i.ToString();

                CustTextBox CName = FindControl("txtSeniorManagerName_" + sLineID) as CustTextBox; //姓名
                CustTextBox CBirth = FindControl("txtSeniorManagerBirth_" + sLineID) as CustTextBox; //生日
                CustTextBox CNation = FindControl("txtSeniorManagerCountryCode_" + sLineID) as CustTextBox; //國籍;
                CustTextBox CID = FindControl("txtSeniorManagerIDNo_" + sLineID) as CustTextBox; //身分證件號碼
                CustTextBox CIDType = FindControl("txtSeniorManagerIDNoType_" + sLineID) as CustTextBox; //身分證件類型;
                CustTextBox CIdentity = FindControl("txtSeniorManagerIdentity_" + sLineID) as CustTextBox; //身分類型;
                CustTextBox CExpDt = FindControl("txtSeniorManagerExpdt_" + sLineID) as CustTextBox; //護照效期/居留期限(西元);
                //20190806-RQ-2019-008595-002-長姓名需求 by Peggy
                CustCheckBox CLNameFlag = FindControl("chkisLongName_gdv_" + sLineID) as CustCheckBox; //長姓名FLAG;
                CustTextBox CName_L = FindControl("txtName_L_" + sLineID) as CustTextBox; //中文長姓名;
                CustTextBox CName_Pinyin = FindControl("txtName_Pinyin_" + sLineID) as CustTextBox; //羅馬拼音;

                //*比對高管姓名
                CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, CName.Text.Trim(), "BENE_NAME" + sLineID, BaseHelper.GetShowText("01_01050100_067") + sLineID + BaseHelper.GetShowText("01_01050100_053"));
                //*比對高管出生日期
                CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, CBirth.Text.Trim(), "BENE_BIRTH_DATE" + sLineID, BaseHelper.GetShowText("01_01050100_067") + sLineID + BaseHelper.GetShowText("01_01050100_054"));
                //*比對高管國籍
                CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, CNation.Text.Trim(), "BENE_NATION" + sLineID, BaseHelper.GetShowText("01_01050100_067") + sLineID + BaseHelper.GetShowText("01_01050100_055"));
                
                string sIDType = string.Empty;
                string sIDNo = string.Empty;
                string sIDNoEXP = string.Empty;

                if (hstQueryP4A.ContainsKey("BENE_ID" + sLineID) && !string.IsNullOrEmpty(hstQueryP4A["BENE_ID" + sLineID].ToString()))
                {
                    sIDType = "1";
                    sIDNo = hstQueryP4A["BENE_ID" + sLineID].ToString();
                }

                if (hstQueryP4A.ContainsKey("BENE_PASSPORT" + sLineID) && !string.IsNullOrEmpty(hstQueryP4A["BENE_PASSPORT" + sLineID].ToString()))
                {
                    sIDType = "3";
                    sIDNo = hstQueryP4A["BENE_PASSPORT" + sLineID].ToString();
                    sIDNoEXP = hstQueryP4A["BENE_PASSPORT_EXP" + sLineID].ToString();
                }

                if (hstQueryP4A.ContainsKey("BENE_RESIDENT_NO" + sLineID) && !string.IsNullOrEmpty(hstQueryP4A["BENE_RESIDENT_NO" + sLineID].ToString()))
                {
                    sIDType = "4";
                    sIDNo = hstQueryP4A["BENE_RESIDENT_NO" + sLineID].ToString();
                    sIDNoEXP = hstQueryP4A["BENE_RESIDENT_EXP" + sLineID].ToString();
                }

                if (hstQueryP4A.ContainsKey("BENE_OTH_CERT" + sLineID) && !string.IsNullOrEmpty(hstQueryP4A["BENE_OTH_CERT" + sLineID].ToString()))
                {
                    sIDType = "7";
                    sIDNo = hstQueryP4A["BENE_OTH_CERT" + sLineID].ToString();
                }

                //*比對高管身分證件類型
                if (CIDType.Text.Trim() != sIDType)
                {

                    DataRow drowRow = dtblUpdateData.NewRow();

                    drowRow[EntityCUSTOMER_LOG.M_field_name] = BaseHelper.GetShowText("01_01050100_067") + sLineID + BaseHelper.GetShowText("01_01050100_068");
                    drowRow[EntityCUSTOMER_LOG.M_before] = sIDType;
                    drowRow[EntityCUSTOMER_LOG.M_after] = CIDType.Text.Trim();
                    dtblUpdateData.Rows.Add(drowRow);
                }

                //*比對高管身分證件號碼 
                if (CID.Text.Trim() != sIDNo)
                {

                    DataRow drowRow = dtblUpdateData.NewRow();

                    drowRow[EntityCUSTOMER_LOG.M_field_name] = BaseHelper.GetShowText("01_01050100_067") + sLineID + BaseHelper.GetShowText("01_01050100_056");
                    drowRow[EntityCUSTOMER_LOG.M_before] = sIDNo;
                    drowRow[EntityCUSTOMER_LOG.M_after] = CID.Text.Trim();
                    dtblUpdateData.Rows.Add(drowRow);
                }

                //*比對高管護照效期/居留期限(西元) 
                if (CExpDt.Text.Trim() != sIDNoEXP)
                {

                    DataRow drowRow = dtblUpdateData.NewRow();

                    drowRow[EntityCUSTOMER_LOG.M_field_name] = BaseHelper.GetShowText("01_01050100_067") + sLineID + BaseHelper.GetShowText("01_01050100_059");
                    drowRow[EntityCUSTOMER_LOG.M_before] = sIDNoEXP;
                    drowRow[EntityCUSTOMER_LOG.M_after] = CExpDt.Text.Trim();
                    dtblUpdateData.Rows.Add(drowRow);
                }

                string sJOBTYPE = string.Empty;
      
                //高管身分類型1
                if (hstQueryP4A.ContainsKey("BENE_JOB_TYPE" + sLineID) && hstQueryP4A["BENE_JOB_TYPE" + sLineID].ToString().ToUpper() == "Y")
                {
                    sJOBTYPE += "1/";
                }

                //高管身分類型2
                if (hstQueryP4A.ContainsKey("BENE_JOB_TYPE_2" + sLineID) && hstQueryP4A["BENE_JOB_TYPE_2" + sLineID].ToString().ToUpper() == "Y")
                {
                    sJOBTYPE += "2/";
                }

                //高管身分類型3
                if (hstQueryP4A.ContainsKey("BENE_JOB_TYPE_3" + sLineID) && hstQueryP4A["BENE_JOB_TYPE_3" + sLineID].ToString().ToUpper() == "Y")
                {
                    sJOBTYPE += "3/";
                }

                //高管身分類型4
                if (hstQueryP4A.ContainsKey("BENE_JOB_TYPE_4" + sLineID) && hstQueryP4A["BENE_JOB_TYPE_4" + sLineID].ToString().ToUpper() == "Y")
                {
                    sJOBTYPE += "4/";
                }

                //高管身分類型5
                if (hstQueryP4A.ContainsKey("BENE_JOB_TYPE_5" + sLineID) && hstQueryP4A["BENE_JOB_TYPE_5" + sLineID].ToString().ToUpper() == "Y")
                {
                    sJOBTYPE += "5/";
                }

                //高管身分類型6
                if (hstQueryP4A.ContainsKey("BENE_JOB_TYPE_6" + sLineID) && hstQueryP4A["BENE_JOB_TYPE_6" + sLineID].ToString().ToUpper() == "Y")
                {
                    sJOBTYPE += "6/";
                }

                if (!string.IsNullOrEmpty(sJOBTYPE))
                {
                    sJOBTYPE = sJOBTYPE.Remove(sJOBTYPE.Length - 1, 1);
                }

                //*比對高管身分類型
                if (CIdentity.Text.Trim() != sJOBTYPE)
                {
                    DataRow drowRow = dtblUpdateData.NewRow();

                    drowRow[EntityCUSTOMER_LOG.M_field_name] = BaseHelper.GetShowText("01_01050100_067") + sLineID + BaseHelper.GetShowText("01_01050100_069");
                    drowRow[EntityCUSTOMER_LOG.M_before] = sJOBTYPE;
                    drowRow[EntityCUSTOMER_LOG.M_after] = CIdentity.Text.Trim();
                    dtblUpdateData.Rows.Add(drowRow);
                }

                //比對高管長姓名FLAG
                //找不到舊資料
                if (CName.Text.Trim() != "")
                {
                    string L_Before = string.Empty;
                    string L_After = string.Empty;

                    Hashtable htAML_JC68 = QueryJC68(CID.Text.Trim());

                    //高管異動比對
                    string strOriBENEFlag = string.IsNullOrEmpty(hstQueryP4A["BENE_LNAM_FLAG" + sLineID].ToString().Trim()) ? "N" : hstQueryP4A["BENE_LNAM_FLAG" + sLineID].ToString().Trim();
                    if (hstQueryP4A.Contains("BENE_LNAM_FLAG" + sLineID) && string.IsNullOrEmpty(hstQueryP4A["BENE_LNAM_FLAG" + sLineID].ToString().Trim()))
                    {
                        hstQueryP4A["BENE_LNAM_FLAG" + sLineID] = "N";
                    }

                    //*比對高管長姓名flag                    
                    _LnameFlag = 0;
                    _LnameFlag = CommonFunction.ContrastDataTwoNew(hstQueryP4A, dtblUpdateData, CLNameFlag.Checked ? "Y" : "N", "BENE_LNAM_FLAG" + sLineID, BaseHelper.GetShowText("01_01050100_067") + sLineID + "長姓名FLAG");

                    _LNameCompare = 0;//接收異動比對結果
                    _PinyinCompare = 0;

                    if (CLNameFlag.Checked || _LnameFlag != 0)//當長姓名FLAG打勾時，亦或是勾選狀態有異動時才做異動比對與更新長姓名結果
                    {
                        //高管長姓名
                        _LNameCompare = CommonFunction.ContrastDataTwoNew(htAML_JC68, dtblUpdateData_JC68, CName_L.Text.Trim(), "LONG_NAME", BaseHelper.GetShowText("01_01050100_067") + sLineID + "中文長姓名");
                        //高管羅馬拼音
                        _PinyinCompare = CommonFunction.ContrastDataTwoNew(htAML_JC68, dtblUpdateData_JC68, LongNameRomaClean(CName_Pinyin.Text.Trim()), "PINYIN_NAME", BaseHelper.GetShowText("01_01050100_067") + sLineID + "羅馬拼音");
                    }

                    _tmpJC68 = new EntityHTG_JC68();
                    if (_LNameCompare != 0 || _PinyinCompare != 0)//有差異才需打JC68
                    {
                        if (CLNameFlag.Checked)
                        {
                            //高管資料 *12
                            _tmpJC68.ID = CID.Text.Trim();
                            _tmpJC68.LONG_NAME = ToWide(CName_L.Text.Trim());
                            _tmpJC68.PINYIN_NAME = LongNameRomaClean(CName_Pinyin.Text.Trim());

                            _JC68s.Add(_tmpJC68);
                        }
                        else
                        {
                            _tmpJC68.ID = CID.Text.Trim();
                            _tmpJC68.LONG_NAME = "";
                            _tmpJC68.PINYIN_NAME = "";

                            _JC68s.Add(_tmpJC68);
                        }
                    }
                }
            }//高管12次結束

            //20190614 Talas 是否為新增件，應以本次I電文查詢結果判斷 判斷依據為 hstQueryP4A["MESSAGE_TYPE"].ToString()
            if (hstQueryP4A["MESSAGE_TYPE"].ToString() == "0006")
            {
                //表示不存在，狀態碼應為 A新增
                sIsNew = "1";

            }
            if (hstQueryP4A["MESSAGE_TYPE"].ToString() == "0000")
            {
                //表示已存在，狀態碼應為 C修改
                sIsNew = "0";
            }


            if (dtblUpdateData.Rows.Count > 0 || dtblUpdateData_JC68.Rows.Count > 0)
            {
                //以畫面上物件產生JC66用HASHTABLE
                Hashtable JC66OBj = buildJC66Hash(DataObj);
                JC66OBj.Add("CORP_SEQ", "0000");

                JC66OBj.Add("LAST_UPDATE_DATE", DateTime.Now.ToString("yyyyMMdd"));//資料最後異動日期


                if (sIsNew == "0") //0表示為修改，給C，反之給A
                {
                    JC66OBj.Add("FUNCTION_CODE", "C");
                }
                if (sIsNew == "1")
                {
                    JC66OBj.Add("FUNCTION_CODE", "A");
                }


                Hashtable hstExmsP4A;
                if (!isTest)
                {
                    //上送主機資料
                    hstExmsP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC66, JC66OBj, false, "11", eAgentInfo);
                }
                else
                {
                    //測試用模擬資料
                    hstExmsP4A = BuildTestHashTB();
                }

                if (!hstExmsP4A.Contains("HtgMsg"))
                {
                    base.strHostMsg += hstExmsP4A["HtgSuccess"].ToString();//*主機返回成功訊息
                    base.strClientMsg += MessageHelper.GetMessage("01_01050101_007");

                    // 20210527 EOS_AML(NOVA) by Ares Dennis
                    #region 異動記錄需報送AML                
                    bool isChanged = false;                    

                    #region 檢核欄位是否異動
                    compareForAMLCheckLog(htgJC66, JC66OBj, "CORP_NO", ref isChanged);// 統一編號
                    compareForAMLCheckLog(htgJC66, JC66OBj, "REG_NAME", ref isChanged);// 登記名稱(中文)
                    compareForAMLCheckLog(htgJC66, JC66OBj, "CORP_REG_ENG_NAME", ref isChanged);// 登記名稱(英文)
                    compareForAMLCheckLog(htgJC66, JC66OBj, "CC", ref isChanged);// 行業編號
                    compareForAMLCheckLog(htgJC66, JC66OBj, "BUILD_DATE", ref isChanged);// 設立日期
                    compareForAMLCheckLog(htgJC66, JC66OBj, "REGISTER_NATION", ref isChanged);// 註冊國籍
                    compareForAMLCheckLog(htgJC66, JC66OBj, "REGISTER_US_STATE", ref isChanged);// 註冊國籍為美國者，請勾選州別
                    compareForAMLCheckLog(htgJC66, JC66OBj, "REG_CITY", ref isChanged);// 登記地址
                    compareForAMLCheckLog(htgJC66, JC66OBj, "REG_ADDR1", ref isChanged);// 登記地址
                    compareForAMLCheckLog(htgJC66, JC66OBj, "REG_ADDR2", ref isChanged);// 登記地址
                    compareForAMLCheckLog(htgJC66, JC66OBj, "COMP_TEL", ref isChanged);// 公司電話
                    compareForAMLCheckLog(htgJC66, JC66OBj, "MAILING_CITY", ref isChanged);// 通訊地址
                    compareForAMLCheckLog(htgJC66, JC66OBj, "MAILING_ADDR1", ref isChanged);// 通訊地址
                    compareForAMLCheckLog(htgJC66, JC66OBj, "MAILING_ADDR2", ref isChanged);// 通訊地址
                    compareForAMLCheckLog(htgJC66, JC66OBj, "EMAIL", ref isChanged);// 公司email
                    compareForAMLCheckLog(htgJC66, JC66OBj, "OWNER_CHINESE_NAME", ref isChanged);// 中文姓名
                    compareForAMLCheckLog(htgJC66, JC66OBj, "OWNER_ENGLISH_NAME", ref isChanged);// 英文姓名
                    compareForAMLCheckLog(htgJC66, JC66OBj, "OWNER_BIRTH_DATE", ref isChanged);// 生日
                    compareForAMLCheckLog(htgJC66, JC66OBj, "OWNER_NATION", ref isChanged);// 負責人國籍
                    compareForAMLCheckLog(htgJC66, JC66OBj, "LONG_NAME", ref isChanged);// 中文長姓名
                    compareForAMLCheckLog(htgJC66, JC66OBj, "PINYIN_NAME", ref isChanged);// 羅馬拼音
                    compareForAMLCheckLog(htgJC66, JC66OBj, "OWNER_ID", ref isChanged);// 身分證字號
                    compareForAMLCheckLog(htgJC66, JC66OBj, "OWNER_CITY", ref isChanged);// 戶籍地址
                    compareForAMLCheckLog(htgJC66, JC66OBj, "OWNER_ADDR1", ref isChanged);// 戶籍地址
                    compareForAMLCheckLog(htgJC66, JC66OBj, "OWNER_ADDR2", ref isChanged);// 戶籍地址
                    compareForAMLCheckLog(htgJC66, JC66OBj, "PASSPORT", ref isChanged);// 護照號碼
                    compareForAMLCheckLog(htgJC66, JC66OBj, "PASSPORT_EXP_DATE", ref isChanged);// 護照效期
                    compareForAMLCheckLog(htgJC66, JC66OBj, "RESIDENT_NO", ref isChanged);// 統一證號
                    compareForAMLCheckLog(htgJC66, JC66OBj, "RESIDENT_EXP_DATE", ref isChanged);// 統一證號效期
                    compareForAMLCheckLog(htgJC66, JC66OBj, "BUSINESS_ORGAN_TYPE", ref isChanged);// 法律形式
                    compareForAMLCheckLog(htgJC66, JC66OBj, "OVERSEAS_FOREIGN", ref isChanged);// 僑外資/外商
                    compareForAMLCheckLog(htgJC66, JC66OBj, "OVERSEAS_FOREIGN_COUNTRY", ref isChanged);// 僑外資/外商母公司國別
                    compareForAMLCheckLog(htgJC66, JC66OBj, "PRIMARY_BUSI_COUNTRY", ref isChanged);// 主要之營業處所國別
                    compareForAMLCheckLog(htgJC66, JC66OBj, "BUSI_RISK_NATION_FLAG", ref isChanged);// 營業處所是否在高風險或制裁國家
                    compareForAMLCheckLog(htgJC66, JC66OBj, "BUSI_RISK_NATION_1", ref isChanged);// 國家
                    compareForAMLCheckLog(htgJC66, JC66OBj, "BUSI_RISK_NATION_2", ref isChanged);// 國家
                    compareForAMLCheckLog(htgJC66, JC66OBj, "BUSI_RISK_NATION_3", ref isChanged);// 國家
                    compareForAMLCheckLog(htgJC66, JC66OBj, "BUSI_RISK_NATION_4", ref isChanged);// 國家
                    compareForAMLCheckLog(htgJC66, JC66OBj, "BUSI_RISK_NATION_5", ref isChanged);// 國家
                    compareForAMLCheckLog(htgJC66, JC66OBj, "COMPLEX_STR_CODE", ref isChanged);// 複雜股權結構
                    compareForAMLCheckLog(htgJC66, JC66OBj, "ALLOW_ISSUE_STOCK_FLAG", ref isChanged);// 是否可發行無記名股票
                    compareForAMLCheckLog(htgJC66, JC66OBj, "ISSUE_STOCK_FLAG", ref isChanged);// 是否已發行無記名股票
                    compareForAMLCheckLog(htgJC66, JC66OBj, "EXAMINE_FLAG", ref isChanged);// 是否已完成SCDD表
                    for(int i = 1; i < 13; i++)// 高管12次
                    {
                        compareForAMLCheckLog(htgJC66, JC66OBj, "BENE_NATION_" + i.ToString(), ref isChanged);
                        compareForAMLCheckLog(htgJC66, JC66OBj, "BENE_NAME_" + i.ToString(), ref isChanged);
                        compareForAMLCheckLog(htgJC66, JC66OBj, "BENE_BIRTH_DATE_" + i.ToString(), ref isChanged);
                        compareForAMLCheckLog(htgJC66, JC66OBj, "BENE_ID_" + i.ToString(), ref isChanged);
                        compareForAMLCheckLog(htgJC66, JC66OBj, "BENE_PASSPORT_" + i.ToString(), ref isChanged);
                        compareForAMLCheckLog(htgJC66, JC66OBj, "BENE_PASSPORT_EXP_" + i.ToString(), ref isChanged);
                        compareForAMLCheckLog(htgJC66, JC66OBj, "BENE_RESIDENT_NO_" + i.ToString(), ref isChanged);
                        compareForAMLCheckLog(htgJC66, JC66OBj, "BENE_RESIDENT_EXP_" + i.ToString(), ref isChanged);
                        compareForAMLCheckLog(htgJC66, JC66OBj, "BENE_OTH_CERT_" + i.ToString(), ref isChanged);
                        compareForAMLCheckLog(htgJC66, JC66OBj, "BENE_OTH_CERT_EXP_" + i.ToString(), ref isChanged);
                        compareForAMLCheckLog(htgJC66, JC66OBj, "BENE_JOB_TYPE_1_" + i.ToString(), ref isChanged);
                        compareForAMLCheckLog(htgJC66, JC66OBj, "BENE_JOB_TYPE_2_" + i.ToString(), ref isChanged);
                        compareForAMLCheckLog(htgJC66, JC66OBj, "BENE_JOB_TYPE_3_" + i.ToString(), ref isChanged);
                        compareForAMLCheckLog(htgJC66, JC66OBj, "BENE_JOB_TYPE_4_" + i.ToString(), ref isChanged);
                        compareForAMLCheckLog(htgJC66, JC66OBj, "BENE_JOB_TYPE_5_" + i.ToString(), ref isChanged);
                        compareForAMLCheckLog(htgJC66, JC66OBj, "BENE_JOB_TYPE_6_" + i.ToString(), ref isChanged);
                        compareForAMLCheckLog(htgJC66, JC66OBj, "BENE_LNAM_FLAG" + i.ToString(), ref isChanged);
                        compareForAMLCheckLog(htgJC66, JC66OBj, "BENE_RESERVED_" + i.ToString(), ref isChanged);
                    }                    
                    #endregion
                    if (isChanged)
                    {
                        EntityAML_CHECKLOG eAMLCheckLog = new EntityAML_CHECKLOG();
                        eAMLCheckLog.CORP_NO = JC66OBj["CORP_NO"].ToString().Trim();
                        eAMLCheckLog.TRANS_ID = "CSIPJC66";
                        eAMLCheckLog.LAST_UPD_BRANCH = txtLAST_UPD_BRANCH.Text.Trim();
                        eAMLCheckLog.LAST_UPD_CHECKER = txtLAST_UPD_CHECKER.Text.Trim();
                        eAMLCheckLog.LAST_UPD_MAKER = txtLAST_UPD_MAKER.Text.Trim();
                        eAMLCheckLog.MOD_USERID = eAgentInfo.agent_id;
                        eAMLCheckLog.MOD_DATE = DateTime.Now.ToString("yyyyMMdd");
                        eAMLCheckLog.MOD_TIME = DateTime.Now.ToString("HHmmss");

                        InsertAMLCheckLog(eAMLCheckLog);
                    }
                    #endregion

                    ///使用交易
                    using (OMTransactionScope ts = new OMTransactionScope())
                    {
                        ///電文更新成功 {
                        //更新欄位  hidisNew.Value == "1" 才需要     AML_ADDHEADDOfficereport  OfficeAdd_day -KEY==> YYYYMMDD 統編      
                        if (sIsNew == "1")
                        {
                            EntityAML_AddHeadOfficeReport pObj = new EntityAML_AddHeadOfficeReport();
                            pObj.TaxID = DataObj.BasicTaxID;
                            pObj.OfficeAdd_day = DateTime.Now.ToString("yyyyMMdd");
                            BRAML_AddHeadOfficeReport.Update(pObj);
                        }

                        bool dRet = true;
                        //將1key,2key 資料新增至 目的資料表
                        dRet = BRAML_HeadOffice_Log.InsertByTaxID(DataObj.BasicTaxID);
                        if (!dRet)
                        {
                            strAlertMsg = @"『" + MessageHelper.GetMessage("01_01050101_004") + "』";
                            return;
                        }


                        //清除資料庫  }
                        dRet = BRAML_HeadOffice.DeleteKData(DataObj.BasicTaxID);

                        if (!dRet)
                        {
                            strAlertMsg = @"『" + MessageHelper.GetMessage("01_01050101_004") + "』";
                            return;
                        }

                        dRet = CommonFunction.InsertCustomerLog(dtblUpdateData, eAgentInfo, TaxID, BaseHelper.GetShowText("01_01050100_066").Trim(), (structPageInfo)Session["PageInfo"]);

                        //JC66更新成功後才更新長姓名資料
                        using (BRHTG_JC68 obj = new BRHTG_JC68("P010105020001"))
                        {
                            EntityResult _EntityResult = new EntityResult();
                            int i = 0;//記錄錯誤的行數
                            foreach (EntityHTG_JC68 item in _JC68s)
                            {
                                _EntityResult = obj.Update(item, this.eAgentInfo, "2");
                                if (_EntityResult.Success == false)
                                {
                                    base.strHostMsg += "更新長姓名資料:" + _EntityResult.HostMsg;
                                    base.strClientMsg += "更新長姓名資料:" + _EntityResult.HostMsg;

                                    dtblUpdateData_JC68.Rows[i].Delete();//更新失敗時，從異動比對刪除此筆記錄
                                }

                                i++;
                            }
                        }

                        dtblUpdateData_JC68.AcceptChanges();

                        dRet = CommonFunction.InsertCustomerLog(dtblUpdateData_JC68, eAgentInfo, TaxID, BaseHelper.GetShowText("01_01050100_066").Trim(), (structPageInfo)Session["PageInfo"]);

                        if (dRet)
                        {
                            ts.Complete();
                            strAlertMsg = @"『" + MessageHelper.GetMessage("01_01050101_007") + "』";//HTG電文送出成功，作業完成

                            CommonFunction.SetControlsEnabled(pnlText, false);// 清空網頁中所有的輸入欄位
                            HQlblHCOP_CC_Cname.Text = "";
                            chkSCCD.Checked = false;
                            CustChK01.Checked = false;
                            CustChK02.Checked = false;
                            CustChK03.Checked = false;

                            radBasicGmail.Checked = false;
                            radBasicYahoo.Checked = false;
                            radBasicHotmail.Checked = false;
                            radBasicOther.Checked = false;

                            chkisLongName.Checked = false;
                            chkisLongName_c.Checked = false;
                        }
                        else
                        {
                            strAlertMsg = @"『" + MessageHelper.GetMessage("01_01050101_004") + "』";
                            return;
                        }
                    }
                }
                else
                {
                    //*異動主機資料失敗
                    if (hstExmsP4A["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                    {
                        strAlertMsg = hstExmsP4A["HtgMsg"].ToString();
                        base.strHostMsg += hstExmsP4A["HtgMsg"].ToString();
                        base.strClientMsg += HtgType.P4A_JC66.ToString() + MessageHelper.GetMessage("01_00000000_011");
                    }
                    else
                    {
                        base.strClientMsg += hstExmsP4A["HtgMsg"].ToString();
                    }

                    return;
                }
            }
            else
            {
                strAlertMsg = @"『" + MessageHelper.GetMessage("01_01050101_011") + "』";//JC66 P4A環境主機已存在相同資料,故不做異動
                base.strClientMsg += MessageHelper.GetMessage("01_01050101_011");
                return;
            }
        }
        else
        {
            if (hstQueryP4A.Contains("HtgMsg"))
            {
                if (hstQueryP4A["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    strAlertMsg = hstQueryP4A["HtgMsg"].ToString();
                    base.strHostMsg += hstQueryP4A["HtgMsg"].ToString();
                    base.strClientMsg += MessageHelper.GetMessage("01_01050101_005");//HTG電文送出失敗
                }
                else
                {
                    base.strClientMsg += hstQueryP4A["HtgMsg"].ToString();
                }
            }

            return;
        } 
    }
    #region checkID
    public static bool isIdentificationId(string arg_Identify)
    {
        bool result = false;
        if (arg_Identify.Length == 10)
        {
            arg_Identify = arg_Identify.ToUpper();
            if (arg_Identify[0] >= 0x41 && arg_Identify[0] <= 0x5A)
            {
                int[] a = new int[] { 10, 11, 12, 13, 14, 15, 16, 17, 34, 18, 19, 20, 21, 22, 35, 23, 24, 25, 26, 27, 28, 29, 32, 30, 31, 33 };
                int[] b = new int[11];
                b[1] = a[(arg_Identify[0]) - 65] % 10;
                int c = b[0] = a[(arg_Identify[0]) - 65] / 10;
                for (int i = 1; i <= 9; i++)
                {
                    b[i + 1] = arg_Identify[i] - 48;
                    c += b[i] * (10 - i);
                }
                if (((c % 10) + b[10]) % 10 == 0)
                {
                    result = true;
                }
            }
        }
        return result;
    }
    #endregion
    /// <summary>
    /// 取出指定字典的值
    /// </summary>
    /// <param name="inObj"></param>
    /// <param name="inKey"></param>
    /// <returns></returns>
    private string GetDcValue(string inKey)
    {
        if (DCCommonColl == null)
        {
            buiInfoDict();
        }
        string rtnVal = "";
        if (DCCommonColl.ContainsKey(inKey))
        {
            rtnVal = DCCommonColl[inKey];
        }
        return rtnVal;
    }
    /// <summary>
    /// 實作字典
    /// </summary>
    private void buiInfoDict()
    {
        DCCommonColl = new Dictionary<string, string>();
        // 通用項目
        DCCommonColl.Add("YN_1", "是");
        DCCommonColl.Add("YN_0", "否");
        DCCommonColl.Add("YN_Y", "是");
        DCCommonColl.Add("YN_N", "否");
        DCCommonColl.Add("HS_1", "有");
        DCCommonColl.Add("HS_0", "無");
        DCCommonColl.Add("HS_Y", "有");
        DCCommonColl.Add("HS_N", "無");

        DCCommonColl.Add("YN1_是", "1");
        DCCommonColl.Add("YN1_否", "0");
        DCCommonColl.Add("YNY_是", "Y");
        DCCommonColl.Add("YNY_否", "N");
        DCCommonColl.Add("HS1_有", "1");
        DCCommonColl.Add("HS1_無", "0");
        DCCommonColl.Add("HSY_有", "Y");
        DCCommonColl.Add("HSY_無", "N");
        //開關 OC_ 
        DCCommonColl.Add("OC_O", "OPEN");
        DCCommonColl.Add("OC_C", "Close");
        DCCommonColl.Add("OC_OPEN", "O");
        DCCommonColl.Add("OC_Close", "C");
        setFromCodeType("1"); //國家 
    }
    /// <summary>
    /// 將指定CODETYPE鍵入字典中
    /// </summary>
    /// <param name="codeType"></param>
    private void setFromCodeType(string codeType)
    {
        DataTable result = BRPostOffice_CodeType.GetCodeType(codeType);
        if (result != null && result.Rows.Count > 0)
        {
            for (int i = 0; i < result.Rows.Count; i++)
            {
                string sKey = "CT_" + codeType + "_" + result.Rows[i][0].ToString();
                if (!DCCommonColl.ContainsKey(sKey))
                {
                    DCCommonColl.Add("CT_" + codeType + "_" + result.Rows[i][0].ToString(), result.Rows[i][1].ToString());
                }
            }
        }
    }

    private bool CheckCodeType(DataTable dt)
    {
        bool result = true;
        bool? isValid = dt.Rows[0]["IsValid"] as bool?;
        if (isValid == null)
        {
            result = false;
        }
        else
        {
            if (isValid == false)
                result = false;
        }
        return result;
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
                CtrlName1 = "txtPrincipalNameCH";//人員名稱
                CtrlName2 = "txtPrincipalNameCH_L";//中文長姓名
                CtrlName3 = "txtPrincipalNameCH_Pinyin";//馬拼音
                break;
            case "chkisLongName_c"://聯絡人
                CtrlName1 = "txtBasicContactMan";//人員名稱
                CtrlName2 = "txtBasicContactMan_L";//中文長姓名
                CtrlName3 = "txtBasicContactMan_Pinyin";//羅馬拼音
                break;
            default:
                lid = chk.ID.Replace("chkisLongName_gdv_", "").Trim();

                CtrlName1 = "txtSeniorManagerName_" + lid;//高管人員名稱
                CtrlName2 = "txtName_L_" + lid;//高管中文長姓名
                CtrlName3 = "txtName_Pinyin_" + lid;//高管羅馬拼音
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
            case "txtPrincipalNameCH_L"://負責人
                CtrlName = "chkisLongName";//人員名稱
                CtrlName1 = "txtPrincipalNameCH";//中文長姓名
                break;
            case "txtBasicContactMan_L"://聯絡人
                CtrlName = "chkisLongName_c";//人員名稱
                CtrlName1 = "txtBasicContactMan";//中文長姓名
                break;
            default://高管人員設定(12組)
                lid = txt.ID.Replace("txtName_L_", "").Trim();

                CtrlName = "chkisLongName_gdv_" + lid;//長姓名flag
                CtrlName1 = "txtSeniorManagerName_" + lid;//高管人員名稱
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
    /// 顯示資料時 高管長姓名相關欄位控制
    /// </summary>
    /// <param name="lid"></param>
    private void GVLongNameDisplay(string lid)
    {
        string CtrlName = "chkisLongName_gdv_" + lid;//長姓名flag
        string CtrlName1 = "txtSeniorManagerName_" + lid;//高管人員名稱
        string CtrlName2 = "txtName_L_" + lid;//高管中文長姓名
        string CtrlName3 = "txtName_Pinyin_" + lid;//高管羅馬拼音
        string DisName2 = "lblName_L_" + lid;//中文長姓名label
        string DisName3 = "lblName_Pinyin_" + lid;//羅馬拼音label

        CustCheckBox contisLongName = this.FindControl(CtrlName) as CustCheckBox;
        CustTextBox contNAME = this.FindControl(CtrlName1) as CustTextBox;
        CustTextBox contNameL = this.FindControl(CtrlName2) as CustTextBox;
        CustTextBox contNamePinyin = this.FindControl(CtrlName3) as CustTextBox;
        CustLabel labNameL = this.FindControl(DisName2) as CustLabel;
        CustLabel labNamePinyin = this.FindControl(DisName3) as CustLabel;

        //找不到不處理;
        if (contisLongName == null) { return; }

        //當中文長姓名不為空時，中文長姓名flag要打勾
        if (!contNameL.Text.Trim().Equals(""))
        {
            contisLongName.Checked = true;

            labNameL.Visible = true;
            contNameL.Visible = true;
            labNamePinyin.Visible = true;
            contNamePinyin.Visible = true;
        }

        CheckBox_CheckedChanged(contisLongName, null);        
    }

    /// <summary>
    /// initial高管人員長姓名的欄位狀態
    /// </summary>
    private void GVTextBoxDefault()
    {
        for (int x = 1; x < 13; x++)
        {
            string txt1 = "txtName_L_" + x;//中文長姓名
            string txt2 = "txtName_Pinyin_" + x;//羅馬拼音
            string chk = "chkisLongName_gdv_" + x;
            string DisName2 = "lblName_L_" + x;//中文長姓名label
            string DisName3 = "lblName_Pinyin_" + x;//羅馬拼音label

            CustCheckBox chkLongName = this.FindControl(chk) as CustCheckBox;
            CustTextBox contNameL = this.FindControl(txt1) as CustTextBox;
            CustTextBox contNamePinyin = this.FindControl(txt2) as CustTextBox;
            CustLabel labNameL = this.FindControl(DisName2) as CustLabel;
            CustLabel labNamePinyin = this.FindControl(DisName3) as CustLabel;
            
            chkLongName.Checked = false;
            contNameL.Visible = contNamePinyin.Visible = labNameL.Visible = labNamePinyin.Visible = false;//20190822初始時，不顯示長姓名欄關欄位

        }
    }

    public Hashtable QueryJC68(string strID)
    {
        Hashtable htInput = new Hashtable();
        htInput.Add("TRAN_ID", "JC68");//*查詢
        htInput.Add("PROGRAM_ID", "JCGUA68");//*查詢
        htInput.Add("FUNCTION_CODE", "I");//*查詢
        htInput.Add("ID", strID);//身份ID
        Hashtable htP4A_JC68 = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC68, htInput, false, "1", eAgentInfo);

        return htP4A_JC68;
    }

    private void init()
    {
        chkisLongName.Checked = false;
        chkisLongName_c.Checked = false;

        GVTextBoxDefault();
    }
    #endregion
}