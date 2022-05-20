//******************************************************************
//*  作    者：趙呂梁  

//*  功能說明：卡人資料異動-異動姓名(生日)

//*  創建日期：2009/10/06
//*  修改記錄：
//*  2010/12/21   chaoma      RQ-2010-005537-000      增設(□P4及□P4D)作業選項及【自動翻譯】按鈕 

//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Data;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Logging;
using Framework.Common.Utility;
using CSIPNewInvoice.EntityLayer_new;
using CSIPKeyInGUI.BusinessRules_new;
using System.Collections.Generic;

public partial class P010801080001 : PageBase
{
    #region 變數區

    private EntityAGENT_INFO eAgentInfo;
    public string Personnel_HtmlValue = "";
    public string Personnel_HtmlPanel = "display: none;";
    public string Signature_HtmlPanel = "display: none;";
    private List<int> iRiskLevel = new List<int>();
    private DataTable dtSanctionCountry = new DataTable();
    private DataTable dtRiskCountry = new DataTable();
    Entity_SCDDReport objEntity_SCDDReport = new Entity_SCDDReport();
    private String strNewRiskLevel = String.Empty;
    List<String> listItemsValue = new List<String>();
    private DataTable dtGeneralSanctionCountry = new DataTable();//20200108-RQ-2019-030155-002 記錄一般制裁國家的清單

    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合

        if (!IsPostBack)
        {
            AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
            if (sessionOBJ == null)
            {
                string NavigateUrl = "P010801000001.aspx";
                string urlString = @"alert('查無資料');location.href='" + NavigateUrl + "';";
                base.sbRegScript.Append(urlString);
            }

            if (sessionOBJ != null)
            {
                try {
                    //高風險國家 Type = 12
                    dtRiskCountry = BRPostOffice_CodeType.GetCodeType("12");
                    //高度制裁國家 Type = 13
                    dtSanctionCountry = BRPostOffice_CodeType.GetCodeType("13");

                    //20200108-RQ-2019-030155-002 記錄一般制裁國家的清單
                    //一般制裁國家 Type = 15
                    dtGeneralSanctionCountry = BRPostOffice_CodeType.GetCodeType("15");
                }
                catch (Exception ex)
                {
                    Logging.Log(ex, LogLayer.UI);
                }

                iRiskLevel.Add(1);
                //讀取公司資料 HQ_WORK CDATA,EDATA
                GetAML_HQ_Work(sessionOBJ);
                GetHQ_SCDD(sessionOBJ);
                GetAML_Cdata_Work(sessionOBJ);
                GetSCDDReport(sessionOBJ);
                Personnel_HtmlValue = GetPersonnel(sessionOBJ);

                //進行降冪排序後，取第一個最大值進行判斷
                iRiskLevel.Sort();
                iRiskLevel.Reverse();

                switch (iRiskLevel[0])
                {
                    case 1:
                        strNewRiskLevel += "低風險";
                        break;
                    case 2:
                        strNewRiskLevel += "中風險";
                        break;
                    case 3:
                        strNewRiskLevel += "高風險";
                        break;
                }

                if (!String.IsNullOrEmpty(strNewRiskLevel))
                    hidNewRiskLevel.Value = strNewRiskLevel;
            }
        }
        base.strClientMsg = "";
        base.strHostMsg = "";
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        string NavigateUrl = "P010801020001.aspx";
        string urlString = @"location.href='" + NavigateUrl + "';";
        base.sbRegScript.Append(urlString);
    }

    #endregion

    #region 方法區

    private void GetAML_HQ_Work(AML_SessionState sessionOBJ)
    {
        try {
            if (sessionOBJ != null)
            {
                EntityAML_HQ_Work hqObj = BRAML_HQ_Work.getHQ_WOrk(sessionOBJ);
                DataTable dtHCOP_CC_Name = new DataTable();
                DataTable dtHCOP_BUSINESS_ORGAN_TYPE = new DataTable();
                DataTable dtHCOP_CC_RiskLevel = new DataTable();

                if (hqObj != null)
                {
                    hidAML_HQ_Work_CASE_NO.Value = hqObj.CASE_NO;
                    lbAML_HQ_Work_HCOP_HEADQUATERS_CORP_NO.Text = hqObj.HCOP_HEADQUATERS_CORP_NO;
                    lbAML_HQ_Work_HCOP_REG_NAME.Text = hqObj.HCOP_REG_NAME;
                    lbAML_HQ_Work_HCOP_CORP_REG_ENG_NAME.Text = hqObj.HCOP_CORP_REG_ENG_NAME;
                    lbAML_HQ_Work_HCOP_BUILD_DATE.Text = hqObj.HCOP_BUILD_DATE;
                    lbAML_HQ_Work_HCOP_STATUS.Text = hqObj.HCOP_STATUS;

                    //商店註冊國籍	AML_HQ_Work.HCOP_REGISTER_NATION
                    lbAML_HQ_Work_HCOP_REGISTER_NATION.Text = hqObj.HCOP_REGISTER_NATION;
                    //20200115-RQ-2019-030155-002 -商店註冊國籍或負責人/高管/實質受益人的國籍位於一般制裁國家/地區->高風險
                    if (isGeneralSanctionCountry(hqObj.HCOP_REGISTER_NATION))
                        iRiskLevel.Add(3);
                    lbAML_HQ_Work_HCOP_COMPLEX_STR_CODE.Text = hqObj.HCOP_COMPLEX_STR_CODE;
                    lbAML_HQ_Work_HCOP_ALLOW_ISSUE_STOCK_FLAG.Text = hqObj.HCOP_ALLOW_ISSUE_STOCK_FLAG;
                    lbAML_HQ_Work_HCOP_ISSUE_STOCK_FLAG.Text = hqObj.HCOP_ISSUE_STOCK_FLAG;
                    lbAML_HQ_Work_HCOP_PRIMARY_BUSI_COUNTRY.Text = hqObj.HCOP_PRIMARY_BUSI_COUNTRY;
                    lbAML_HQ_Work_HCOP_OVERSEAS_FOREIGN.Text = hqObj.HCOP_OVERSEAS_FOREIGN;
                    //20190614 Talas 增加母公司對應
                    lbAML_HQ_Work_HCOP_OVERSEAS_FOREIGN_COUNTRY.Text = hqObj.HCOP_OVERSEAS_FOREIGN_COUNTRY;
                    lbAML_HQ_Work_HCOP_OWNER_CHINESE_NAME.Text = hqObj.HCOP_OWNER_CHINESE_NAME;
                    lbAML_HQ_Work_HCOP_OWNER_BIRTH_DATE.Text = hqObj.HCOP_OWNER_BIRTH_DATE;
                    //負責人國籍
                    lbAML_HQ_Work_HCOP_OWNER_NATION.Text = hqObj.HCOP_OWNER_NATION;
                    //20200115-RQ-2019-030155-002 -負責人/高管/實質受益人的國籍為本行所列高度或一般制裁國家->高風險
                    if (isSanctionCountry(hqObj.HCOP_OWNER_NATION))
                        iRiskLevel.Add(3);
                    if (isGeneralSanctionCountry(hqObj.HCOP_OWNER_NATION))
                        iRiskLevel.Add(3);

                    lbAML_HQ_Work_HCOP_OWNER_ENGLISH_NAME.Text = hqObj.HCOP_OWNER_ENGLISH_NAME;
                    hidAML_HQ_Work_OWNER_ID_SreachStatus.Value = hqObj.OWNER_ID_SreachStatus;
                    lbAML_HQ_Work_HCOP_CC.Text = hqObj.HCOP_CC;

                    //2019/04/01 Leon 修正顯示
                    switch (hqObj.HCOP_STATUS.ToUpper())
                    {
                        case "O":
                            lbAML_HQ_Work_HCOP_STATUS.Text = WebHelper.GetShowText("01_08010600_071");
                            break;
                        case "C":
                            lbAML_HQ_Work_HCOP_STATUS.Text = WebHelper.GetShowText("01_08010600_072");
                            break;
                    }

                    if (!String.IsNullOrEmpty(hqObj.HCOP_OWNER_ID))
                        lbAML_HQ_Work_HCOP_OWNER_ID.Text = hqObj.HCOP_OWNER_ID;
                    else
                        lbAML_HQ_Work_HCOP_OWNER_ID.Text = WebHelper.GetShowText("01_08010600_073");

                    if (!String.IsNullOrEmpty(hqObj.HCOP_PASSPORT))
                        lbAML_HQ_Work_HCOP_PASSPORT.Text = hqObj.HCOP_PASSPORT;
                    else
                        lbAML_HQ_Work_HCOP_PASSPORT.Text = WebHelper.GetShowText("01_08010600_073");

                    if (!String.IsNullOrEmpty(hqObj.HCOP_RESIDENT_NO))
                        lbAML_HQ_Work_HCOP_RESIDENT_NO.Text = hqObj.HCOP_RESIDENT_NO;
                    else
                        lbAML_HQ_Work_HCOP_RESIDENT_NO.Text = WebHelper.GetShowText("01_08010600_073");

                    if (!String.IsNullOrEmpty(hqObj.HCOP_REGISTER_US_STATE))
                    {
                        try
                        {
                            DataTable dtHCOP_REGISTER_US_STATE = new DataTable();
                            //美國州別 Type=8
                            dtHCOP_REGISTER_US_STATE = BRPostOffice_CodeType.GetCodeTypeByCodeID("8", hqObj.HCOP_REGISTER_US_STATE);
                            if (dtHCOP_REGISTER_US_STATE != null && dtHCOP_REGISTER_US_STATE.Rows.Count > 0)
                                lbAML_HQ_Work_HCOP_REGISTER_US_STATE.Text = dtHCOP_REGISTER_US_STATE.Rows[0]["CODE_NAME"].ToString();
                            else
                                lbAML_HQ_Work_HCOP_REGISTER_US_STATE.Text += "(" + hqObj.HCOP_REGISTER_US_STATE + ")";
                        }
                        catch (Exception ex)
                        {
                            Logging.Log(ex, LogLayer.UI);
                        }
                    }

                    if (hqObj.OWNER_ID_SreachStatus == "1" || hqObj.OWNER_ID_SreachStatus.ToUpper() == "Y")
                        lbAML_HQ_Work_OWNER_ID_SreachStatus.Text = "正常";
                    else if (hqObj.OWNER_ID_SreachStatus == "0" || hqObj.OWNER_ID_SreachStatus.ToUpper() == "N")
                        lbAML_HQ_Work_OWNER_ID_SreachStatus.Text = "不適用";

                    if (!String.IsNullOrEmpty(hqObj.HCOP_CC))
                    {
                        try
                        {
                            dtHCOP_CC_Name = BRPostOffice_CodeType.GetCodeTypeByCodeID("3", hqObj.HCOP_CC);
                            if (dtHCOP_CC_Name != null && dtHCOP_CC_Name.Rows.Count > 0)
                            {
                                //行業別中文名稱
                                lbAML_HQ_Work_HCOP_CC_Name.Text = dtHCOP_CC_Name.Rows[0]["CODE_NAME"].ToString();
                                dtHCOP_CC_Name.Clear();
                                dtHCOP_CC_Name.Dispose();
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging.Log(ex, LogLayer.UI);
                        }

                        try
                        {
                            //高風險行業別 Type = 11
                            dtHCOP_CC_RiskLevel = BRPostOffice_CodeType.GetCodeTypeByCodeID("11", hqObj.HCOP_CC);
                            //客戶行業為高風險行業->高風險
                            if (dtHCOP_CC_RiskLevel != null && dtHCOP_CC_RiskLevel.Rows.Count > 0)
                                iRiskLevel.Add(3);
                        }
                        catch (Exception ex)
                        {
                            Logging.Log(ex, LogLayer.UI);
                        }
                    }

                    if (!String.IsNullOrEmpty(hqObj.HCOP_BUSINESS_ORGAN_TYPE))
                    {
                        try
                        {
                            dtHCOP_BUSINESS_ORGAN_TYPE = BRPostOffice_CodeType.GetCodeTypeByCodeID("2", hqObj.HCOP_BUSINESS_ORGAN_TYPE);
                            if (dtHCOP_BUSINESS_ORGAN_TYPE != null && dtHCOP_BUSINESS_ORGAN_TYPE.Rows.Count > 0)
                            {
                                //法律形式
                                lbAML_HQ_Work_HCOP_BUSINESS_ORGAN_TYPE.Text = dtHCOP_BUSINESS_ORGAN_TYPE.Rows[0]["CODE_NAME"].ToString();
                                dtHCOP_BUSINESS_ORGAN_TYPE.Clear();
                                dtHCOP_BUSINESS_ORGAN_TYPE.Dispose();
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging.Log(ex, LogLayer.UI);
                        }
                    }

                    //客戶註冊國籍/通訊地國家/永久地國家/主要營業處所國家/高風險營業處所/僑外資外商母公司國別 位於高風險國家地區->中風險 
                    //通訊地國家/永久地國家??
                    if (dtRiskCountry != null && dtRiskCountry.Rows.Count > 0)
                    {
                        //註冊國籍	AML_HQ_Work.HCOP_REGISTER_NATION
                        /* 20200115-RQ-2019-030155-002 判斷改至上方
                        if (!String.IsNullOrEmpty(hqObj.HCOP_REGISTER_NATION))
                        { 
                            if (dtRiskCountry.Select("CODE_ID = '" + hqObj.HCOP_REGISTER_NATION + "'").Length > 0)
                                iRiskLevel.Add(2);
                        }
                        */
                        //主要營業處所國別	AML_HQ_Work.HCOP_PRIMARY_BUSI_COUNTRY
                        if (!String.IsNullOrEmpty(hqObj.HCOP_PRIMARY_BUSI_COUNTRY))
                        {
                            if (dtRiskCountry.Select("CODE_ID = '" + hqObj.HCOP_PRIMARY_BUSI_COUNTRY + "'").Length > 0)
                                iRiskLevel.Add(2);
                        }
                        //僑外資/外商母公司國別	AML_HQ_Work.HCOP_OVERSEAS_FOREIGN_COUNTRY
                        if (!String.IsNullOrEmpty(hqObj.HCOP_OVERSEAS_FOREIGN_COUNTRY))
                        {
                            if (dtRiskCountry.Select("CODE_ID = '" + hqObj.HCOP_OVERSEAS_FOREIGN_COUNTRY + "'").Length > 0)
                                iRiskLevel.Add(2);
                        }
                    }

                    //客戶為複雜股權結構或無記名股份有限公司(無記名=Y)->高風險
                    if (hqObj.HCOP_COMPLEX_STR_CODE == "Y" || hqObj.HCOP_COMPLEX_STR_CODE == "1")
                        iRiskLevel.Add(3);
                    if (hqObj.HCOP_ALLOW_ISSUE_STOCK_FLAG == "Y" || hqObj.HCOP_ALLOW_ISSUE_STOCK_FLAG == "1")
                        iRiskLevel.Add(3);
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
    }

    private void GetHQ_SCDD(AML_SessionState sessionOBJ)
    {
        try
        {
            if (sessionOBJ != null)
            {
                EntityHQ_SCDD sccdObj = BRHQ_SCDD.getSCDDData_WOrk(sessionOBJ);
                DataTable dtIsSanctionCountry = new DataTable();
                if (sccdObj != null)
                {
                    lbHQ_SCDD_Organization_Note.Text = sccdObj.Organization_Note;
                    lbHQ_SCDD_BusinessForeignAddress.Text = sccdObj.BusinessForeignAddress;
                    lbHQ_SCDD_RiskObject.Text = sccdObj.RiskObject;
                    hidHQ_SCDD_NameCheck_Item.Value = sccdObj.NameCheck_Item;
                    lbHQ_SCDD_NameCheck_Note.Text = sccdObj.NameCheck_Note;
                    hidHQ_SCDD_NameCheck_RiskRanking.Value = sccdObj.NameCheck_RiskRanking;
                 //   lbIsSanction.Text = sccdObj.IsSanction == "1" ? "Y" : "N";
                 //Talas 20190624 修正
                    lbIsSanction.Text = sccdObj.IsSanction;
                    if (!String.IsNullOrEmpty(sccdObj.NameCheck_Item))
                    {
                        try
                        {
                            DataTable dtNameCheck_Item = new DataTable();
                            //名單掃描結果項目 Type=5
                            dtNameCheck_Item = BRPostOffice_CodeType.GetCodeTypeByCodeID("5", sccdObj.NameCheck_Item);
                            if (dtNameCheck_Item != null && dtNameCheck_Item.Rows.Count > 0)
                                lbHQ_SCDD_NameCheck_Item.Text = dtNameCheck_Item.Rows[0]["CODE_NAME"].ToString();
                            else
                                lbHQ_SCDD_NameCheck_Item.Text = sccdObj.NameCheck_Item;
                            dtNameCheck_Item.Clear();
                            dtNameCheck_Item.Dispose();
                        }
                        catch (Exception ex)
                        {
                            Logging.Log(ex, LogLayer.UI);
                        }
                    }

                    if (!String.IsNullOrEmpty(sccdObj.Organization_Item))
                    {
                        try
                        {
                            DataTable dtOrganization_Item = new DataTable();
                            //組織運作 Type=6
                            dtOrganization_Item = BRPostOffice_CodeType.GetCodeTypeByCodeID("6", sccdObj.Organization_Item);
                            if (dtOrganization_Item != null && dtOrganization_Item.Rows.Count > 0)
                                lbHQ_SCDD_Organization_Item.Text = dtOrganization_Item.Rows[0]["CODE_NAME"].ToString();
                            else
                                lbHQ_SCDD_Organization_Item.Text = sccdObj.Organization_Item;
                            dtOrganization_Item.Clear();
                            dtOrganization_Item.Dispose();
                        }
                        catch (Exception ex)
                        {
                            Logging.Log(ex, LogLayer.UI);
                        }
                    }

                    if (!String.IsNullOrEmpty(sccdObj.Proof_Item))
                    {
                        try
                        {
                            DataTable dtProof_Item = new DataTable();
                            //存在證明 Type=7
                            //20190614 Talas 修正存在證明誤植為組織運作問題
                            // dtProof_Item = BRPostOffice_CodeType.GetCodeTypeByCodeID("7", sccdObj.Organization_Item);
                            dtProof_Item = BRPostOffice_CodeType.GetCodeTypeByCodeID("7", sccdObj.Proof_Item);
                            if (dtProof_Item != null && dtProof_Item.Rows.Count > 0)
                                lbHQ_SCDD_Proof_Item.Text = dtProof_Item.Rows[0]["CODE_NAME"].ToString();
                            else
                                lbHQ_SCDD_Proof_Item.Text = sccdObj.Proof_Item;
                            dtProof_Item.Clear();
                            dtProof_Item.Dispose();
                        }
                        catch (Exception ex)
                        {
                            Logging.Log(ex, LogLayer.UI);
                        }
                    }

                    //(Name Check)客戶為國外PEP /國外PEP利益關聯人/NN /制裁名單->高風險  
                    //(Name Check)國內PEP/國際組織PEP/國內PEP利益關聯人/國際組織PEP利益關聯人->中風險
                    if (sccdObj.NameCheck_Item == "2")
                        iRiskLevel.Add(3);
                    else if (sccdObj.NameCheck_Item == "3")
                        iRiskLevel.Add(2);
                    if (sccdObj.IsSanction == "Y")//營業處所是否在高風險或制裁國家
                    //     if (sccdObj.IsSanction == "1")
                    {
                        try
                        {
                            dtIsSanctionCountry = BRPostOffice_CodeType.GetCodeType("1");
                            if (dtIsSanctionCountry != null && dtIsSanctionCountry.Rows.Count > 0)
                            {
                                String strTemp = "";
                                for (int i = 1; i <= 5; i++)
                                {
                                    DataRow dr = dtIsSanctionCountry.NewRow();
                                    switch (i)
                                    {
                                        case 1:
                                            if (dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode1 + "'").Length == 1)
                                            {
                                                if (!String.IsNullOrEmpty(strTemp))
                                                    strTemp += ",";
                                                strTemp += dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode1 + "'")[0]["CODE_NAME"].ToString() + "(" + sccdObj.IsSanctionCountryCode1  + ")";
                                            }

                                            if (dtRiskCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode1 + "'").Length > 0)
                                                iRiskLevel.Add(2);
                                            break;
                                        case 2:
                                            if (dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode2 + "'").Length == 1)
                                            {
                                                if (!String.IsNullOrEmpty(strTemp))
                                                    strTemp += ",";
                                                strTemp += dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode2 + "'")[0]["CODE_NAME"].ToString() + "(" + sccdObj.IsSanctionCountryCode2 + ")";
                                            }

                                            if (dtRiskCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode2 + "'").Length > 0)
                                                iRiskLevel.Add(2);
                                            break;
                                        case 3:
                                            if (dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode3 + "'").Length == 1)
                                            {
                                                if (!String.IsNullOrEmpty(strTemp))
                                                    strTemp += ",";
                                                strTemp += dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode3 + "'")[0]["CODE_NAME"].ToString() + "(" + sccdObj.IsSanctionCountryCode3 + ")";
                                            }

                                            if (dtRiskCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode3 + "'").Length > 0)
                                                iRiskLevel.Add(2);
                                            break;
                                        case 4:
                                            if (dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode4 + "'").Length == 1)
                                            {
                                                if (!String.IsNullOrEmpty(strTemp))
                                                    strTemp += ",";
                                                strTemp += dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode4 + "'")[0]["CODE_NAME"].ToString() + "(" + sccdObj.IsSanctionCountryCode4 + ")";
                                            }

                                            if (dtRiskCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode4 + "'").Length > 0)
                                                iRiskLevel.Add(2);
                                            break;
                                        case 5:
                                            if (dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode5 + "'").Length == 1)
                                            {
                                                if (!String.IsNullOrEmpty(strTemp))
                                                    strTemp += ",";
                                                strTemp += dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode5 + "'")[0]["CODE_NAME"].ToString() + "(" + sccdObj.IsSanctionCountryCode5 + ")";
                                            }

                                            if (dtRiskCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode5 + "'").Length > 0)
                                                iRiskLevel.Add(2);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                lbIsSanctionCountryCode.Text = strTemp;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging.Log(ex, LogLayer.UI);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
    }

    private void GetAML_Cdata_Work(AML_SessionState sessionOBJ)
    {
        try
        {
            EntityAML_Cdata_Work cDobj = BRAML_Cdata_Work.getCData_WOrk(sessionOBJ);

            if (cDobj != null)
            {
                lbAML_Cdata_Work_WarningFlag.Text = cDobj.WarningFlag;
                lbAML_Cdata_Work_FiledSAR.Text = cDobj.GroupInformationSharingNameListflag;
                lbAML_Cdata_Work_Incorporated.Text = cDobj.Incorporated;
                
                //(AML C檔)被申報疑似洗錢/信用卡不良註記/AML不合作註記/警示帳戶->高風險
                if (cDobj.GroupInformationSharingNameListflag == "Y")
                    iRiskLevel.Add(3);

                //20191119-RQ-2018-015749-002-因法遵已公告不合作不列入高風險因子，故調整SCDD計算風險等級邏輯。
                //if (cDobj.Incorporated == "Y")//不合作
                //    iRiskLevel.Add(3);
                if (cDobj.WarningFlag == "Y")//警示帳戶
                    iRiskLevel.Add(3);

            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
    }

    private String GetPersonnel(AML_SessionState sessionOBJ)
    {
        String strHtmlValues = String.Empty;
        try
        {
            List<EntityAML_HQ_Manager_Work> listHQ_Manager = BRAML_HQ_Manager_Work.getHQMA_WorkColl(sessionOBJ);
            List<EntityAML_BRCH_Work> listBRCH_Work = BRAML_BRCH_Work.getBRCH_WOrkColl(sessionOBJ);
            List<Personnel_Manager> listPersonnel_Manager = new List<Personnel_Manager>();

            if (listBRCH_Work != null && listBRCH_Work.Count > 0)
            {
                for (int i = 0; i < listBRCH_Work.Count; i++)
                {
                    if (!String.IsNullOrEmpty(listBRCH_Work[i].BRCH_CHINESE_NAME) && !String.IsNullOrEmpty(listBRCH_Work[i].BRCH_NATION))
                    {
                        Personnel_Manager loopObj = new Personnel_Manager();
                        loopObj.Name = listBRCH_Work[i].BRCH_CHINESE_NAME;
                        loopObj.Country = listBRCH_Work[i].BRCH_NATION;
                        listPersonnel_Manager.Add(loopObj);
                    }
                }
            }

            if (listHQ_Manager != null && listHQ_Manager.Count > 0)
            {
                for (int i = 0; i < listHQ_Manager.Count; i++)
                {
                    if (!String.IsNullOrEmpty(listHQ_Manager[i].HCOP_BENE_NAME) && !String.IsNullOrEmpty(listHQ_Manager[i].HCOP_BENE_NATION))
                    {
                        Personnel_Manager loopObj = new Personnel_Manager();
                        loopObj.Name = listHQ_Manager[i].HCOP_BENE_NAME;
                        loopObj.Country = listHQ_Manager[i].HCOP_BENE_NATION;
                        listPersonnel_Manager.Add(loopObj);
                    }
                }
            }

            if (listPersonnel_Manager != null && listPersonnel_Manager.Count > 0)
            {
                int iRowsCount = listPersonnel_Manager.Count / 4;
                int iRemainder = listPersonnel_Manager.Count % 4;

                if (iRemainder > 0)
                    iRowsCount++;

                for (int i = 0; i < iRowsCount; i++)
                {
                    strHtmlValues += "<tr class=\"trOdd\">";
                    for (int j = 0; j < 4; j++)
                    {
                        int k = i * 4 + j;
                        String strLoopHtml = "<td>[Name]</td><td>[Country]</td>";

                        if (k > listPersonnel_Manager.Count - 1)
                        {
                            strLoopHtml = strLoopHtml.Replace("[Name]", "");
                            strLoopHtml = strLoopHtml.Replace("[Country]", "");
                        }
                        else
                        {
                            strLoopHtml = strLoopHtml.Replace("[Name]", listPersonnel_Manager[k].Name);
                            strLoopHtml = strLoopHtml.Replace("[Country]", listPersonnel_Manager[k].Country);

                            if (!String.IsNullOrEmpty(listPersonnel_Manager[k].Country))
                            {
                                //20200115-RQ-2019-030155-002 -負責人/高管/實質受益人的國籍為本行所列高度或一般制裁國家->高風險                         
                                if (isGeneralSanctionCountry(listPersonnel_Manager[i].Country))
                                {
                                    iRiskLevel.Add(3);
                                }

                                //客戶其關聯人為自然人且國籍為本行所列高度制裁國家->高風險，加提示請依指引『風險管控』章節所列核准層級授權k
                                if (dtSanctionCountry.Select("CODE_ID = '" + listPersonnel_Manager[k].Country + "'").Length > 0)
                                    iRiskLevel.Add(3);
                            }
                        }
                        strHtmlValues += strLoopHtml;
                    }
                    strHtmlValues += "</tr>";
                }
                Personnel_HtmlPanel = "";
            }
            
            //有提出下階段要移除重複的人員
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
        return strHtmlValues;
    }

    private void GetSCDDReport(AML_SessionState sessionOBJ)
    {
        DataTable dtSCDDReport = new DataTable();
        try
        {
            if (sessionOBJ != null)
            {
                Entity_SCDDReport searchObj = new Entity_SCDDReport();
                searchObj.SR_CASE_NO = sessionOBJ.CASE_NO;
                SCDDReport.GetSCDDReportDataTable(searchObj, "", ref dtSCDDReport);

                if (dtSCDDReport != null && dtSCDDReport.Rows.Count > 0)
                {
                    objEntity_SCDDReport.SR_CASE_NO = dtSCDDReport.Rows[0]["SR_CASE_NO"].ToString();
                    objEntity_SCDDReport.SR_Explanation = dtSCDDReport.Rows[0]["SR_Explanation"].ToString();
                    objEntity_SCDDReport.SR_RiskItem = dtSCDDReport.Rows[0]["SR_RiskItem"].ToString();
                    objEntity_SCDDReport.SR_RiskLevel = dtSCDDReport.Rows[0]["SR_RiskLevel"].ToString();
                    objEntity_SCDDReport.SR_RiskNote = dtSCDDReport.Rows[0]["SR_RiskNote"].ToString();
                    objEntity_SCDDReport.SR_User = dtSCDDReport.Rows[0]["SR_User"].ToString();

                    lbSR_Explanation.Text = objEntity_SCDDReport.SR_Explanation;
                    Label1.Text = objEntity_SCDDReport.SR_User;
                    Label9.Text = WebHelper.GetShowText("01_08010600_046").Replace("本人__________________(簽名或蓋章)", "本人___" + objEntity_SCDDReport.SR_User + "____(簽名或蓋章)");

                    if (!String.IsNullOrEmpty(dtSCDDReport.Rows[0]["SR_DateTime"].ToString()))
                    {
                        try
                        {
                            objEntity_SCDDReport.SR_DateTime = Convert.ToDateTime(dtSCDDReport.Rows[0]["SR_DateTime"].ToString());
                            Label2.Text = objEntity_SCDDReport.SR_DateTime.ToString("yyyy/MM/dd HH:mm:ss");
                        }
                        catch (Exception ex)
                        {
                            Logging.Log(ex, LogLayer.UI);
                        }
                    }
                }

                switch (objEntity_SCDDReport.SR_RiskItem)
                {
                    case "0":
                        lbSR_RiskItem.Text = objEntity_SCDDReport.SR_RiskLevel;
                        break;
                    case "1":
                        lbSR_RiskItem.Text = "不合作";
                        break;
                    case "2":
                        lbSR_RiskItem.Text = "商店解約";
                        break;
                    case "3":
                        lbSR_RiskItem.Text = "其他";
                        lbSR_RiskNote.Text = objEntity_SCDDReport.SR_RiskNote;
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
    }

    private class Personnel_Manager
    {
        private string _Country;
        public string Country
        {
            get
            {
                return this._Country;
            }
            set
            {
                this._Country = value;
            }
        }

        private string _Name;
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
            }
        }
    }

    //判斷是否為一般制裁國家
    private bool isGeneralSanctionCountry(string _Nation)
    {
        bool isExist = false;

        if (dtGeneralSanctionCountry != null && dtGeneralSanctionCountry.Rows.Count > 0)
        {
            if (!String.IsNullOrEmpty(_Nation))
            {
                if (dtGeneralSanctionCountry.Select("CODE_ID = '" + _Nation + "'").Length > 0)
                {
                    isExist = true;
                }
            }
        }

        return isExist;
    }

    //判斷是否為高度制裁國家
    private bool isSanctionCountry(string _Nation)
    {
        bool isExist = false;

        if (dtSanctionCountry != null && dtSanctionCountry.Rows.Count > 0)
        {
            if (!String.IsNullOrEmpty(_Nation))
            {
                if (dtGeneralSanctionCountry.Select("CODE_ID = '" + _Nation + "'").Length > 0)
                {
                    isExist = true;
                }
            }
        }

        return isExist;
    }

    #endregion
}
