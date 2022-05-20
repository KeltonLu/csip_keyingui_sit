//******************************************************************
//*  作    者：林家賜 
//*  功能說明： 結案案件SCDD預覽
//*  創建日期：2019/05/10
//*  修改記錄：
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*  Ares Stanley       2021/04/29                                 修正按鈕失效問題
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Web.UI.WebControls;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Logging;
using Framework.Common.Utility;
using CSIPNewInvoice.EntityLayer_new;
using CSIPKeyInGUI.BusinessRules_new;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Framework.WebControls;
using System.Xml;
using System.Xml.Serialization;
using System.Web;
public partial class P010801140201 : PageBase
{
    #region 變數區

    private EntityAGENT_INFO eAgentInfo;
    public string Personnel_HtmlValue = "";
    public string Personnel_HtmlPanel = "display: none;";
    public string Signature_HtmlPanel = "display: none;";
    private DataTable dtSanctionCountry = new DataTable();
    private DataTable dtRiskCountry = new DataTable();
    Entity_SCDDReport objEntity_SCDDReport = new Entity_SCDDReport();
    List<String> listItemsValue = new List<String>();
    private DataTable dtGeneralSanctionCountry = new DataTable();//20200108-RQ-2019-030155-002 記錄一般制裁國家的清單

    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {
        Boolean bHaveData = false;
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合

        if (!IsPostBack)
        {
            AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801140001_SESSION"];
            if (sessionOBJ == null)
            {
                //以下為開發時測試用
                //sessionOBJ = new AML_SessionState();
                //sessionOBJ.CASE_NO = "20190116000001";
                //sessionOBJ.RMMBatchNo = "12345678901234";
                //sessionOBJ.AMLInternalID = "00001000000000000000";

                //以下為正式CODE，開發時暫時註解
                string NavigateUrl = "P010801140001.aspx";
                string urlString = @"alert('查無資料');location.href='" + NavigateUrl + "';";
                base.sbRegScript.Append(urlString);
            }

            if (sessionOBJ != null)
            {
                try
                {
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

                //讀取公司資料 HQ_WORK CDATA,EDATA
                GetAML_HQ_Work(sessionOBJ);
                GetHQ_SCDD(sessionOBJ);
                GetAML_Cdata_Work(sessionOBJ);
                bHaveData = GetSCDDReport(sessionOBJ);
                //Personnel_HtmlValue = GetPersonnel(sessionOBJ);
                GetPersonnel(sessionOBJ);

                if (!bHaveData)
                {
                    string NavigateUrl = "P010801140101.aspx";
                    string urlString = @"alert('查無資料');location.href='" + NavigateUrl + "';";
                    base.sbRegScript.Append(urlString);
                }


                if (!String.IsNullOrEmpty(hidPersonnelHTML.Value))
                {
                    Personnel_HtmlPanel = "";
                    //Personnel_HtmlValue = hidPersonnelHTML.Value;
                    Literal1.Text = hidPersonnelHTML.Value;
                }
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(Literal1.Text))
            {
                Personnel_HtmlPanel = "";
            }
        }
        base.strClientMsg = "";
        base.strHostMsg = "";
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        string NavigateUrl = "P010801140101.aspx";
        string urlString = @"location.href='" + NavigateUrl + "';";
        base.sbRegScript.Append(urlString);
    }

    #endregion

    #region 方法區

    private void GetAML_HQ_Work(AML_SessionState sessionOBJ)
    {
        try
        {
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

                    lbAML_HQ_Work_HCOP_COMPLEX_STR_CODE.Text = hqObj.HCOP_COMPLEX_STR_CODE;
                    lbAML_HQ_Work_HCOP_ALLOW_ISSUE_STOCK_FLAG.Text = hqObj.HCOP_ALLOW_ISSUE_STOCK_FLAG;
                    lbAML_HQ_Work_HCOP_ISSUE_STOCK_FLAG.Text = hqObj.HCOP_ISSUE_STOCK_FLAG;
                    lbAML_HQ_Work_HCOP_PRIMARY_BUSI_COUNTRY.Text = hqObj.HCOP_PRIMARY_BUSI_COUNTRY;
                    lbAML_HQ_Work_HCOP_OVERSEAS_FOREIGN.Text = hqObj.HCOP_OVERSEAS_FOREIGN;
                    //20190614 Talas 增加母公司對應
                    lbAML_HQ_Work_HCOP_OVERSEAS_FOREIGN_COUNTRY.Text = hqObj.HCOP_OVERSEAS_FOREIGN_COUNTRY;
                    lbAML_HQ_Work_HCOP_OWNER_CHINESE_NAME.Text = hqObj.HCOP_OWNER_CHINESE_NAME;
                    lbAML_HQ_Work_HCOP_OWNER_ID.Text = hqObj.HCOP_OWNER_ID;
                    lbAML_HQ_Work_HCOP_PASSPORT.Text = hqObj.HCOP_PASSPORT;
                    lbAML_HQ_Work_HCOP_OWNER_BIRTH_DATE.Text = hqObj.HCOP_OWNER_BIRTH_DATE;
                    //國籍
                    lbAML_HQ_Work_HCOP_OWNER_NATION.Text = hqObj.HCOP_OWNER_NATION;

                    lbAML_HQ_Work_HCOP_OWNER_ENGLISH_NAME.Text = hqObj.HCOP_OWNER_ENGLISH_NAME;
                    hidAML_HQ_Work_OWNER_ID_SreachStatus.Value = hqObj.OWNER_ID_SreachStatus;
                    lbAML_HQ_Work_HCOP_CC.Text = hqObj.HCOP_CC;

                    HQlblHCOP_OWNER_CHINESE_LNAME.Text = hqObj.HCOP_OWNER_CHINESE_LNAME;
                    HQlblHCOP_OWNER_ROMA.Text = hqObj.HCOP_OWNER_ROMA;
                    //公司負責人長姓名顯示
                    if (!string.IsNullOrEmpty(HQlblHCOP_OWNER_CHINESE_LNAME.Text) || !string.IsNullOrEmpty(HQlblHCOP_OWNER_ROMA.Text))
                    {
                        cmpRname.Style["display"] = "";
                        cmpLname.Style["display"] = "";
                    }
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
                    //  lbIsSanction.Text = sccdObj.IsSanction == "1" ? "Y" : "N";
                    lbIsSanction.Text = sccdObj.IsSanction;
                    lblHQ_SCDD_NameCheck_No.Text = sccdObj.NameCheck_No;//20191101-RQ-2018-015749-002 add by Peggy

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
                                                strTemp += dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode1 + "'")[0]["CODE_NAME"].ToString() + "(" + sccdObj.IsSanctionCountryCode1 + ")";
                                            }

                                            break;
                                        case 2:
                                            if (dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode2 + "'").Length == 1)
                                            {
                                                if (!String.IsNullOrEmpty(strTemp))
                                                    strTemp += ",";
                                                strTemp += dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode2 + "'")[0]["CODE_NAME"].ToString() + "(" + sccdObj.IsSanctionCountryCode2 + ")";
                                            }

                                            break;
                                        case 3:
                                            if (dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode3 + "'").Length == 1)
                                            {
                                                if (!String.IsNullOrEmpty(strTemp))
                                                    strTemp += ",";
                                                strTemp += dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode3 + "'")[0]["CODE_NAME"].ToString() + "(" + sccdObj.IsSanctionCountryCode3 + ")";
                                            }

                                            break;
                                        case 4:
                                            if (dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode4 + "'").Length == 1)
                                            {
                                                if (!String.IsNullOrEmpty(strTemp))
                                                    strTemp += ",";
                                                strTemp += dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode4 + "'")[0]["CODE_NAME"].ToString() + "(" + sccdObj.IsSanctionCountryCode4 + ")";
                                            }

                                            break;
                                        case 5:
                                            if (dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode5 + "'").Length == 1)
                                            {
                                                if (!String.IsNullOrEmpty(strTemp))
                                                    strTemp += ",";
                                                strTemp += dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode5 + "'")[0]["CODE_NAME"].ToString() + "(" + sccdObj.IsSanctionCountryCode5 + ")";
                                            }

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
            // 20220415 個/法人的SCDD C檔TABLE 主管頁面看到的要是KEEP住經辦送出時的資料，故將原邏輯註解 Start By Kelton
            //EntityAML_Cdata_Work cDobj = BRAML_Cdata_Work.getCData_WOrk(sessionOBJ);

            //if (cDobj != null)
            //{
            //    lbAML_Cdata_Work_WarningFlag.Text = cDobj.WarningFlag;
            //    lbAML_Cdata_Work_FiledSAR.Text = cDobj.GroupInformationSharingNameListflag;
            //    lbAML_Cdata_Work_Incorporated.Text = cDobj.Incorporated;
            //}
            // 20220415 個/法人的SCDD C檔TABLE 主管頁面看到的要是KEEP住經辦送出時的資料，故將原邏輯註解 End By Kelton

            // 20220414 先查詢 BRAML_Cdata_Work_S by Kelton
            EntityAML_Cdata_Work_S cDSobj = BRAML_Cdata_Work_S.getCData_WOrk(new AML_SessionState { CASE_NO = sessionOBJ.CASE_NO });

            // 20220414 若 BRAML_Cdata_Work_S 沒有對應資料才查詢 BRAML_Cdata_Work 的 C 檔資料 by Kelton
            if (cDSobj == null)
            {
                EntityAML_Cdata_Work cDobj = BRAML_Cdata_Work.getCData_WOrk(sessionOBJ);

                if (cDobj != null)
                {
                    lbAML_Cdata_Work_WarningFlag.Text = cDobj.WarningFlag;
                    lbAML_Cdata_Work_FiledSAR.Text = cDobj.GroupInformationSharingNameListflag;
                    lbAML_Cdata_Work_Incorporated.Text = cDobj.Incorporated;
                }
            }
            else // 20220414 若有 BRAML_Cdata_Work_S 的 C 檔資料，則使用 BRAML_Cdata_Work_S 的資料 by Kelton
            {
                lbAML_Cdata_Work_WarningFlag.Text = cDSobj.WarningFlag;
                lbAML_Cdata_Work_FiledSAR.Text = cDSobj.GroupInformationSharingNameListflag;
                lbAML_Cdata_Work_Incorporated.Text = cDSobj.Incorporated;
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
            //報表用
            List<clsMangger> listManager = new List<clsMangger>();
            if (listBRCH_Work != null && listBRCH_Work.Count > 0)
            {
                for (int i = 0; i < listBRCH_Work.Count; i++)
                {
                    //多了以下判斷式，會導致資料缺少，所以移除
                    //if (!String.IsNullOrEmpty(listBRCH_Work[i].BRCH_CHINESE_NAME) && !String.IsNullOrEmpty(listBRCH_Work[i].BRCH_NATION))
                    //{
                    Personnel_Manager loopObj = new Personnel_Manager();
                    loopObj.Name = listBRCH_Work[i].BRCH_CHINESE_NAME;
                    loopObj.Country = listBRCH_Work[i].BRCH_NATION;
                    //增加長姓名及羅馬
                    loopObj.Lname = listBRCH_Work[i].BRCH_CHINESE_LNAME;
                    loopObj.Romaname = listBRCH_Work[i].BRCH_ROMA;
                    listPersonnel_Manager.Add(loopObj);

                    //加入報表用物件
                    clsMangger prtObj = new clsMangger();
                    prtObj.Name = listBRCH_Work[i].BRCH_CHINESE_NAME;
                    prtObj.Nation = listBRCH_Work[i].BRCH_NATION;
                    //增加長姓名及羅馬
                    prtObj.Lname = listBRCH_Work[i].BRCH_CHINESE_LNAME;
                    prtObj.Romaname = listBRCH_Work[i].BRCH_ROMA;
                    listManager.Add(prtObj);
                    //}
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
                        //增加長姓名及羅馬
                        loopObj.Lname = listHQ_Manager[i].HCOP_BENE_LNAME;
                        loopObj.Romaname = listHQ_Manager[i].HCOP_BENE_ROMA;
                        listPersonnel_Manager.Add(loopObj);

                        //加入報表用物件
                        clsMangger prtObj = new clsMangger();
                        //prtObj.Name = listBRCH_Work[i].BRCH_CHINESE_NAME;
                        //prtObj.Nation = listBRCH_Work[i].BRCH_NATION;
                        prtObj.Name = listHQ_Manager[i].HCOP_BENE_NAME;
                        prtObj.Nation = listHQ_Manager[i].HCOP_BENE_NATION;
                        //增加長姓名及羅馬
                        prtObj.Lname = listHQ_Manager[i].HCOP_BENE_LNAME;
                        prtObj.Romaname = listHQ_Manager[i].HCOP_BENE_ROMA;
                        listManager.Add(prtObj);
                    }
                }
            }

            //20190614 Talas 無論有無，都要更新SESSION，避免被上一筆留存
            //if (listManager.Count > 0)
            //{
            Session["clsManager"] = listManager;
            // }

            if (listPersonnel_Manager != null && listPersonnel_Manager.Count > 0)
            {
                for (int i = 0; i < listPersonnel_Manager.Count; i++)
                {
                    strHtmlValues += "";

                    String strLoopHtml = "<tr class=\"trOdd\"><td width=\"80\">[Name]</td><td width=\"80\">[Country]</td><td>[LName]</td><td>[ROMA]</td></tr>";

                    strLoopHtml = strLoopHtml.Replace("[Name]", listPersonnel_Manager[i].Name);
                    strLoopHtml = strLoopHtml.Replace("[Country]", listPersonnel_Manager[i].Country);
                    strLoopHtml = strLoopHtml.Replace("[LName]", listPersonnel_Manager[i].Lname);
                    strLoopHtml = strLoopHtml.Replace("[ROMA]", listPersonnel_Manager[i].Romaname);

                    strHtmlValues += strLoopHtml;

                }
                Personnel_HtmlPanel = "";
            }

            //有提出下階段要移除重複的人員
            hidPersonnelHTML.Value = HttpUtility.UrlDecode(strHtmlValues);
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
        return strHtmlValues;
    }

    private Boolean GetSCDDReport(AML_SessionState sessionOBJ)
    {
        DataTable dtSCDDReport = new DataTable();
        Boolean bHaveData = false;
        try
        {
            if (sessionOBJ != null)
            {
                Entity_SCDDReport searchObj = new Entity_SCDDReport();
                searchObj.SR_CASE_NO = sessionOBJ.CASE_NO;
                SCDDReport.GetSCDDReportDataTable(searchObj, "", ref dtSCDDReport);

                if (dtSCDDReport != null && dtSCDDReport.Rows.Count > 0)
                {
                    bHaveData = true;
                    objEntity_SCDDReport.SR_CASE_NO = dtSCDDReport.Rows[0]["SR_CASE_NO"].ToString();
                    objEntity_SCDDReport.SR_Explanation = dtSCDDReport.Rows[0]["SR_Explanation"].ToString();
                    objEntity_SCDDReport.SR_RiskItem = dtSCDDReport.Rows[0]["SR_RiskItem"].ToString();
                    objEntity_SCDDReport.SR_RiskLevel = dtSCDDReport.Rows[0]["SR_RiskLevel"].ToString();
                    objEntity_SCDDReport.SR_RiskNote = dtSCDDReport.Rows[0]["SR_RiskNote"].ToString();
                    objEntity_SCDDReport.SR_User = dtSCDDReport.Rows[0]["SR_User"].ToString();
                    objEntity_SCDDReport.SR_EDD_Date = dtSCDDReport.Rows[0]["SR_EDD_Date"].ToString();//20211125_Ares_Jack_EDD完成日期

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

                if (lbSR_RiskItem.Text == "高風險")
                {
                    lbSR_EDD_Date.Text = "EDD完成日期：" + objEntity_SCDDReport.SR_EDD_Date;//2021112526_Ares_Jack_EDD完成日期
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
        return bHaveData;
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
        //長姓名
        private string _Lname;
        public string Lname
        {
            get
            {
                return this._Lname;
            }
            set
            {
                this._Lname = value;
            }
        }
        //長ROMA
        private string _Romaname;
        public string Romaname
        {
            get
            {
                return this._Romaname;
            }
            set
            {
                this._Romaname = value;
            }
        }
    }
    #endregion

    private clsSCDDPrint getPageValue()
    {
        clsSCDDPrint clsItem = new clsSCDDPrint();
        Type v = clsItem.GetType();  //取的型別實體
        PropertyInfo[] props = v.GetProperties(); //取出所有公開屬性(可以被外部存取得 
                                                  //  string LineID = ""; //若有分行，則用來帶出LINEID

        foreach (PropertyInfo prop in props)
        {
            object[] attrs = prop.GetCustomAttributes(true); //取得自訂屬性，第一個物件
            AttributeRFPrint authAttr;
            for (int xi = 0; xi < attrs.Length; xi++)
            {
                if (attrs[xi] is AttributeRFPrint)
                {
                    authAttr = attrs[xi] as AttributeRFPrint;
                    string propVla = "";
                    string propName = prop.Name;
                    string authID = authAttr.labelName;
                    object controllS = this.FindControl(authID);
                    if (controllS == null)
                    { continue; } //找不到對應物件，略過
                    if (authID.Substring(0, 4) == "Cust")
                    {
                        CustLabel CuslblText = controllS as CustLabel; //轉型
                        propVla = CuslblText.Text;
                    }

                    else
                    {
                        Label CuslblText = controllS as Label; //轉型
                        propVla = CuslblText.Text;
                    }

                    if (propVla != null)  //有取到值
                    {
                        prop.SetValue(clsItem, propVla, null);
                    }
                }
            }
        }
        return clsItem;
    }
    /// <summary>
    /// Serializes an object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializableObject"></param>
    /// <param name="fileName"></param>
    public void SerializeObject<T>(T serializableObject, string fileName)
    {
        if (serializableObject == null) { return; }

        try
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, serializableObject);
                stream.Position = 0;
                xmlDocument.Load(stream);
                xmlDocument.Save(fileName);
            }
        }
        catch (Exception ex)
        {

            string exM = ex.Message;
        }
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        clsSCDDPrint citem = getPageValue();
        List<clsMangger> clsObj = new List<clsMangger>();
        if (Session["clsManager"] != null)
        {
            clsObj = Session["clsManager"] as List<clsMangger>;
        }
        citem.lbCustLabel30 = WebHelper.GetShowText("01_08010600_055");
        citem.lbCustLabel34 = WebHelper.GetShowText("01_08010600_054");
        citem.SR_Explanation = lbSR_EDD_Date.Text + "\n " + lbSR_Explanation.Text;//20211126_Ares_Jack_EDD完成日期

        citem.ManagerColl = clsObj;
        //序列化輸出，測試用
        // SerializeObject<clsSCDDPrint>(citem, @"D:\prtTestxml.xml");
        //end
        string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
        if (!Directory.Exists(strServerPathFile))
        {
            Directory.CreateDirectory(strServerPathFile);
        }
        string msgID = citem.AML_HQ_Work_HCOP_HEADQUATERS_CORP_NO; //統編
        bool result = BRSCDDReport.CreateExcelFile(citem, "", ref strServerPathFile, ref msgID);
        if (result)
        {

            //  strServerPathFile = @"D:\share\ExcelFile_AutoPayStatus_20190409094654.xls";
            FileInfo fs = new FileInfo(strServerPathFile);
            Session["ServerFile"] = strServerPathFile;
            //Session["ClientFile"] = fs.Name;
            Session["ClientFile"] = "SCDDReport_" + DateTime.Now.ToString("yyyyMMdd") + "_" + msgID + ".xls";
            string urlString = @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);
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
}
