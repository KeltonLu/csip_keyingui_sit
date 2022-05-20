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
using CSIPCommonModel.EntityLayer;
using CSIPKeyInGUI.EntityLayer;
using Framework.Common.Message;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Logging;
using Framework.Common.Utility;
using CSIPKeyInGUI.EntityLayer_new;
using CSIPNewInvoice.EntityLayer_new;
using CSIPKeyInGUI.BusinessRules_new;
using System.Collections.Generic;

public partial class P010801060001 : PageBase
{
    #region 變數區
    private EntityAGENT_INFO eAgentInfo;
    public string Personnel_HtmlValue = "";
    public string Personnel_HtmlPanel = "display: none;";
    public string Signature_HtmlPanel = "display: none;";
    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
    //private List<int> iRiskLevel = new List<int>();
    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
    private int iRiskLevel;
    private DataTable dtSanctionCountry = new DataTable();
    private DataTable dtRiskCountry = new DataTable();
    Entity_SCDDReport objEntity_SCDDReport = new Entity_SCDDReport();
    private String strNewRiskLevel = String.Empty;
    List<String> listItemsValue = new List<String>();
    private string strCheckRiskMsg = String.Empty;
    private DataTable dtGeneralSanctionCountry = new DataTable();//20200108-RQ-2019-030155-002 記錄一般制裁國家的清單

    #endregion

    #region 事件區
    /// <summary>
    /// 修改紀錄:2021/01/25_Ares_Stanley-調整頁面資料避免按鈕失效
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合

        //HTML範例
        //Personnel_HtmlValue = @"<tr class=""itemTitle""><td align=""left"" colspan=""8"">123</td></tr>";
        String strMsg = String.Empty;
        RiskLevelCalculateService riskLevelCalculateService = new RiskLevelCalculateService();

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
                //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                //iRiskLevel.Add(1);
                //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
                //讀取公司資料 HQ_WORK CDATA,EDATA
                GetAML_HQ_Work(sessionOBJ);
                GetHQ_SCDD(sessionOBJ);
                GetAML_Cdata_Work(sessionOBJ);
                GetSCDDReport(sessionOBJ);
                Personnel_HtmlValue = GetPersonnel(sessionOBJ, ref strMsg);
                hidID_SreachStatus_Msg.Value = strMsg;
                iRiskLevel = riskLevelCalculateService.RiskLevelCalculate(sessionOBJ);

                //20220127_Ares_Jack_風險等級只由經辦頁面計算
                //20220209_Ares_Jack_EDD完成日期、風險等級勾選只由經辦頁面修改
                string NavigateUrl = "P010801010001.aspx";
                if (!string.IsNullOrEmpty(Session["P010801010001_Last"].ToString()))
                {
                    NavigateUrl = Session["P010801010001_Last"].ToString();
                }
                if (NavigateUrl == "P010801010001.aspx")
                {
                    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                    //進行降冪排序後，取第一個最大值進行判斷
                    //iRiskLevel.Sort();
                    //iRiskLevel.Reverse();
                    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End

                    switch (iRiskLevel)
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

                    ViewState["NewRiskLevel"] = iRiskLevel;

                    if (objEntity_SCDDReport != null)
                    {
                        if (!String.IsNullOrEmpty(objEntity_SCDDReport.SR_RiskLevel))
                        {
                            btnCalculatingRiskLevel.Visible = false;
                            cbCalculatingRiskLevel.Visible = true;
                            cbCalculatingRiskLevel.Text = objEntity_SCDDReport.SR_RiskLevel;

                            if (strNewRiskLevel != objEntity_SCDDReport.SR_RiskLevel)
                            {
                                cbCalculatingRiskLevel.Text += " 調整為 " + strNewRiskLevel;
                            }

                        }
                        else
                        {
                            btnCalculatingRiskLevel.Visible = true;
                            cbCalculatingRiskLevel.Visible = false;
                            cbCalculatingRiskLevel.Text = strNewRiskLevel;
                        }
                    }
                    else
                    {
                        btnCalculatingRiskLevel.Visible = true;
                        cbCalculatingRiskLevel.Visible = false;
                        cbCalculatingRiskLevel.Text = strNewRiskLevel;
                    }

                    if (!String.IsNullOrEmpty(strNewRiskLevel))
                        hidNewRiskLevel.Value = strNewRiskLevel;
                }
                else
                {
                    //主管頁面
                    btnCalculatingRiskLevel.Visible = false;
                    cbCalculatingRiskLevel.Visible = true;
                    cbCalculatingRiskLevel.Text = objEntity_SCDDReport.SR_RiskLevel;
                    cbCalculatingRiskLevel.Enabled = false;
                    hidNewRiskLevel.Value = objEntity_SCDDReport.SR_RiskLevel;
                    strNewRiskLevel = objEntity_SCDDReport.SR_RiskLevel;
                    switch (objEntity_SCDDReport.SR_RiskLevel)
                    {
                        case "低風險":
                            ViewState["NewRiskLevel"] = 1;
                            break;
                        case "中風險":
                            ViewState["NewRiskLevel"] = 2;
                            break;
                        case "高風險":
                            ViewState["NewRiskLevel"] = 3;
                            break;
                    }
                    txtEDDFinished.Enabled = false;
                    chbSR_EDD_Status.Enabled = false;
                }

            }
            if (strNewRiskLevel != "高風險")
            {
                chbSR_EDD_Status.Checked = false;
                //chbSR_EDD_Status.Enabled = false; 
            }
            if (!String.IsNullOrEmpty(strCheckRiskMsg))
            {
                string urlString = @"alert('[MSG]');";
                urlString = urlString.Replace("[MSG]", strCheckRiskMsg);
                base.sbRegScript.Append(urlString);
            }
        }
        else
        {
            if (!String.IsNullOrEmpty(hidPersonnelHTML.Value))
            {
                Personnel_HtmlPanel = "";
                Personnel_HtmlValue = HttpUtility.UrlDecode(hidPersonnelHTML.Value);
            }
        }
        base.strClientMsg = "";
        base.strHostMsg = "";

    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        //bool blSubmitStatus = false;
        Entity_SCDDReport objEdit = new Entity_SCDDReport();
        string urlString = @"alert('[MSG]');";
        String strCheckMsg = String.Empty;

        try
        {
            strCheckMsg = CheckInputData();

            if (!String.IsNullOrEmpty(strCheckMsg))
            {
                urlString = urlString.Replace("[MSG]", strCheckMsg);
            }
            else
            {
                Boolean blSubmitStatus = false;
                DataTable dtSCDDReport = new DataTable();

                objEdit.SR_CASE_NO = hidAML_HQ_Work_CASE_NO.Value;

                SCDDReport.GetSCDDReportDataTable(objEdit, "", ref dtSCDDReport);

                objEdit.SR_DateTime = DateTime.Now;
                objEdit.SR_Explanation = txtSR_Explanation.Text;
                objEdit.SR_RiskLevel = hidNewRiskLevel.Value;
                objEdit.SR_RiskItem = listItemsValue[0].ToString();
                //20191202-RQ-2018-015749-002 mark by Peggy
                //if (listItemsValue[0].ToString() == "3")
                //    objEdit.SR_RiskNote = txtSR_RiskNote.Text;
                //else
                objEdit.SR_RiskNote = "";
                objEdit.SR_User = eAgentInfo.agent_id;
                if (chbSR_EDD_Status.Checked)
                    objEdit.SR_EDD_Status = "1";
                else
                    objEdit.SR_EDD_Status = "0";

                //20211125_Ares_Jack_ EDD 完成日期
                objEdit.SR_EDD_Date = txtEDDFinished.Text;
                if (cbCalculatingRiskLevel.Text == "高風險")
                {
                    if (txtEDDFinished.Text == "")
                        urlString = urlString.Replace("[MSG]", WebHelper.GetShowText("01_08010600_076"));//高風險 EDD完成日期為必填
                }

                if (dtSCDDReport != null && dtSCDDReport.Rows.Count > 0)
                {
                    //修改
                    blSubmitStatus = SCDDReport.UPDATE_Obj(objEdit, hidAML_HQ_Work_CASE_NO.Value);
                }
                else
                {
                    //新增
                    blSubmitStatus = SCDDReport.AddNewEntity(objEdit);
                }

                if (blSubmitStatus)
                {
                    urlString = urlString.Replace("[MSG]", WebHelper.GetShowText("01_08010600_064"));
                    try
                    {
                        //修改 AML_HQ_Work.CaseOwner_User
                        if (!String.IsNullOrEmpty(hidAML_HQ_Work_CASE_NO.Value) && !String.IsNullOrEmpty(eAgentInfo.agent_id))
                        {
                            try
                            {
                                EntityAML_HQ_Work objHQ_WorkEdit = new EntityAML_HQ_Work();
                                objHQ_WorkEdit.CASE_NO = hidAML_HQ_Work_CASE_NO.Value;
                                objHQ_WorkEdit.CaseOwner_User = eAgentInfo.agent_id;
                                bool result = BRAML_HQ_Work.Update_CaseOwner_User(objHQ_WorkEdit);
                            }
                            catch (Exception ex)
                            {
                                Logging.Log(ex);
                            }
                        }

                        Entity_NoteLog objEntity_NoteLog = new Entity_NoteLog();
                        objEntity_NoteLog.NL_CASE_NO = hidAML_HQ_Work_CASE_NO.Value;
                        objEntity_NoteLog.NL_SecondKey = "";
                        objEntity_NoteLog.NL_DateTime = DateTime.Now;
                        objEntity_NoteLog.NL_User = eAgentInfo.agent_id;
                        objEntity_NoteLog.NL_Type = "SCDD";
                        objEntity_NoteLog.NL_Value = eAgentInfo.agent_id + " 於 " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " 編輯SCDD表單-更新成功";
                        objEntity_NoteLog.NL_ShowFlag = "1";
                        blSubmitStatus = NoteLog.AddNewEntity(objEntity_NoteLog);
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(ex);
                    }
                }
                else
                {
                    urlString = urlString.Replace("[MSG]", WebHelper.GetShowText("01_08010600_065"));
                    try
                    {
                        Entity_NoteLog objEntity_NoteLog = new Entity_NoteLog();
                        objEntity_NoteLog.NL_CASE_NO = hidAML_HQ_Work_CASE_NO.Value;
                        objEntity_NoteLog.NL_SecondKey = "";
                        objEntity_NoteLog.NL_DateTime = DateTime.Now;
                        objEntity_NoteLog.NL_User = eAgentInfo.agent_id;
                        objEntity_NoteLog.NL_Type = "SCDD";
                        objEntity_NoteLog.NL_Value = eAgentInfo.agent_id + "於" + DateTime.Now.ToString("yyyy/MM/dd HHmmss") + "編輯SCDD表單-更新失敗";
                        objEntity_NoteLog.NL_ShowFlag = "1";
                        blSubmitStatus = NoteLog.AddNewEntity(objEntity_NoteLog);
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(ex);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
        }

        base.sbRegScript.Append(urlString);
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        string NavigateUrl = "P010801010001.aspx";
        //有可能是由主管頁來，需判斷
        if (!string.IsNullOrEmpty(Session["P010801010001_Last"].ToString()))
        {
            NavigateUrl = Session["P010801010001_Last"].ToString();
        }
        string urlString = @"location.href='" + NavigateUrl + "';";
        base.sbRegScript.Append(urlString);
    }

    protected void btnCalculatingRiskLevel_Click(object sender, EventArgs e)//計算風險等級或異常結案
    {
        Boolean bCheckData = true;
        String strMsg = String.Empty;
        try
        {
            if (String.IsNullOrEmpty(hidHQ_SCDD_NameCheck_Item.Value))
            {
                bCheckData = false;
                strMsg += WebHelper.GetShowText("01_08010600_062");//名單掃描結果無紀錄
            }
            if (String.IsNullOrEmpty(hidAML_HQ_Work_OWNER_ID_SreachStatus.Value))
            {
                bCheckData = false;
                if (!String.IsNullOrEmpty(strMsg))
                    strMsg += "\\n";
                strMsg += WebHelper.GetShowText("01_08010600_063");//總公司負責人身分證換補領查詢無紀錄
            }

            if (bCheckData)
            {
                cbCalculatingRiskLevel.Visible = true;
                btnCalculatingRiskLevel.Visible = false;
            }
            else
            {
                string urlString = @"alert('" + strMsg + "');";
                base.sbRegScript.Append(urlString);
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
        }
    }

    protected void btnView_Click(object sender, EventArgs e)
    {
        string NavigateUrl = "P010801090001.aspx";
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

                    //商店註冊國籍	AML_HQ_Work.HCOP_REGISTER_NATION
                    lbAML_HQ_Work_HCOP_REGISTER_NATION.Text = hqObj.HCOP_REGISTER_NATION;
                    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                    //20200115-RQ-2019-030155-002 -商店註冊國籍或負責人/高管/實質受益人的國籍位於一般制裁國家/地區->高風險
                    //if (isGeneralSanctionCountry(hqObj.HCOP_REGISTER_NATION))
                    //    iRiskLevel.Add(3);
                    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
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
                    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                    //20200115-RQ-2019-030155-002 -負責人/高管/實質受益人的國籍為本行所列高度或一般制裁國家->高風險
                    //if (isSanctionCountry(hqObj.HCOP_OWNER_NATION))
                    //    iRiskLevel.Add(3);
                    //if (isGeneralSanctionCountry(hqObj.HCOP_OWNER_NATION))
                    //    iRiskLevel.Add(3);
                    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End

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
                        //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                        //try
                        //{
                        //    //高風險行業別 Type = 11
                        //    dtHCOP_CC_RiskLevel = BRPostOffice_CodeType.GetCodeTypeByCodeID("11", hqObj.HCOP_CC);
                        //    //客戶行業為高風險行業->高風險
                        //    //if (dtHCOP_CC_RiskLevel != null && dtHCOP_CC_RiskLevel.Rows.Count > 0)
                        //    //    iRiskLevel.Add(3);
                        //}
                        //catch (Exception ex)
                        //{
                        //    Logging.Log(ex, LogLayer.UI);
                        //}
                        //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
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
                    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                    //客戶註冊國籍/通訊地國家/永久地國家/主要營業處所國家/高風險營業處所/僑外資外商母公司國別 位於高風險國家地區->中風險 
                    //通訊地國家/永久地國家??
                    //if (dtRiskCountry != null && dtRiskCountry.Rows.Count > 0)
                    //{
                    //註冊國籍	AML_HQ_Work.HCOP_REGISTER_NATION
                    /* 20200115-RQ-2019-030155-002 判斷改至上方
                    if (!String.IsNullOrEmpty(hqObj.HCOP_REGISTER_NATION))
                    {
                        if (dtRiskCountry.Select("CODE_ID = '" + hqObj.HCOP_REGISTER_NATION + "'").Length > 0)
                            iRiskLevel.Add(2);
                    }
                    */
                    ////主要營業處所國別	AML_HQ_Work.HCOP_PRIMARY_BUSI_COUNTRY
                    //if (!String.IsNullOrEmpty(hqObj.HCOP_PRIMARY_BUSI_COUNTRY))
                    //{
                    //    if (dtRiskCountry.Select("CODE_ID = '" + hqObj.HCOP_PRIMARY_BUSI_COUNTRY + "'").Length > 0)
                    //        iRiskLevel.Add(2);
                    //}
                    ////僑外資/外商母公司國別	AML_HQ_Work.HCOP_OVERSEAS_FOREIGN_COUNTRY
                    //if (!String.IsNullOrEmpty(hqObj.HCOP_OVERSEAS_FOREIGN_COUNTRY))
                    //{
                    //    if (dtRiskCountry.Select("CODE_ID = '" + hqObj.HCOP_OVERSEAS_FOREIGN_COUNTRY + "'").Length > 0)
                    //        iRiskLevel.Add(2);
                    //}
                    //}

                    //客戶為複雜股權結構或無記名股份有限公司(無記名=Y)->高風險
                    //if (hqObj.HCOP_COMPLEX_STR_CODE == "Y" || hqObj.HCOP_COMPLEX_STR_CODE == "1")
                    //    iRiskLevel.Add(3);
                    //if (hqObj.HCOP_ALLOW_ISSUE_STOCK_FLAG == "Y" || hqObj.HCOP_ALLOW_ISSUE_STOCK_FLAG == "1")
                    //    iRiskLevel.Add(3);
                    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
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
                    lbIsSanction.Text = sccdObj.IsSanction;// == "1" ? "Y" : "N";
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
                    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                    //(Name Check)客戶為國外PEP /國外PEP利益關聯人/NN /制裁名單->高風險  
                    //(Name Check)國內PEP/國際組織PEP/國內PEP利益關聯人/國際組織PEP利益關聯人->中風險
                    //if (sccdObj.NameCheck_Item == "2")
                    //    iRiskLevel.Add(3);
                    //else if (sccdObj.NameCheck_Item == "3")
                    //    iRiskLevel.Add(3);                       
                    // 20211203  依據USER需求調整風險等級  (Name Check)國內PEP/國際組織PEP/國內PEP利益關聯人/國際組織PEP利益關聯人->高風險   Edit by Ares Rick
                    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
                    if (sccdObj.IsSanction == "Y")//營業處所是否在高風險或制裁國家
                    //   if (sccdObj.IsSanction == "1")
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
                                            //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                                            //if (dtRiskCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode1 + "'").Length > 0)
                                            //    iRiskLevel.Add(2);
                                            //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
                                            break;
                                        case 2:
                                            if (dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode2 + "'").Length == 1)
                                            {
                                                if (!String.IsNullOrEmpty(strTemp))
                                                    strTemp += ",";
                                                strTemp += dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode2 + "'")[0]["CODE_NAME"].ToString() + "(" + sccdObj.IsSanctionCountryCode2 + ")";
                                            }
                                            //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                                            //if (dtRiskCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode2 + "'").Length > 0)
                                            //    iRiskLevel.Add(2);
                                            //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
                                            break;
                                        case 3:
                                            if (dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode3 + "'").Length == 1)
                                            {
                                                if (!String.IsNullOrEmpty(strTemp))
                                                    strTemp += ",";
                                                strTemp += dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode3 + "'")[0]["CODE_NAME"].ToString() + "(" + sccdObj.IsSanctionCountryCode3 + ")";
                                            }
                                            //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                                            //if (dtRiskCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode3 + "'").Length > 0)
                                            //    iRiskLevel.Add(2);
                                            //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
                                            break;
                                        case 4:
                                            if (dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode4 + "'").Length == 1)
                                            {
                                                if (!String.IsNullOrEmpty(strTemp))
                                                    strTemp += ",";
                                                strTemp += dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode4 + "'")[0]["CODE_NAME"].ToString() + "(" + sccdObj.IsSanctionCountryCode4 + ")";
                                            }
                                            //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                                            //if (dtRiskCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode4 + "'").Length > 0)
                                            //    iRiskLevel.Add(2);
                                            //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
                                            break;
                                        case 5:
                                            if (dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode5 + "'").Length == 1)
                                            {
                                                if (!String.IsNullOrEmpty(strTemp))
                                                    strTemp += ",";
                                                strTemp += dtIsSanctionCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode5 + "'")[0]["CODE_NAME"].ToString() + "(" + sccdObj.IsSanctionCountryCode5 + ")";
                                            }
                                            //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                                            //if (dtRiskCountry.Select("CODE_ID = '" + sccdObj.IsSanctionCountryCode5 + "'").Length > 0)
                                            //    iRiskLevel.Add(2);
                                            //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                lbIsSanctionCountryCode.Text = strTemp;
                            }
                            dtRiskCountry.Clear();
                            dtRiskCountry.Dispose();
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
            //    lbAML_Cdata_Work_WarningFlag.Text = cDobj.WarningFlag; ;
            //    lbAML_Cdata_Work_FiledSAR.Text = cDobj.GroupInformationSharingNameListflag;
            //    lbAML_Cdata_Work_Incorporated.Text = cDobj.Incorporated;
            //    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
            //    //(AML C檔)被申報疑似洗錢/信用卡不良註記/AML不合作註記/警示帳戶->高風險
            //    //if (cDobj.GroupInformationSharingNameListflag == "Y")
            //    //    iRiskLevel.Add(3);
            //    //20191119-RQ-2018-015749-002-因法遵已公告不合作不列入高風險因子，故調整SCDD計算風險等級邏輯。
            //    //if (cDobj.Incorporated == "Y")//不合作
            //    //    iRiskLevel.Add(3);
            //    //if (cDobj.WarningFlag == "Y")//警示帳戶
            //    //    iRiskLevel.Add(3);
            //    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
            //}
            // 20220415 個/法人的SCDD C檔TABLE 主管頁面看到的要是KEEP住經辦送出時的資料，故將原邏輯註解 End By Kelton

            // 20220415 取得來源頁面路徑，用來判斷是經辦還是主管頁 by Kelton
            string NavigateUrl = "P010801010001.aspx";
            if (!string.IsNullOrEmpty(Session["P010801010001_Last"].ToString()))
            {
                NavigateUrl = Session["P010801010001_Last"].ToString();
            }

            EntityAML_Cdata_Work_S cDSobj = null;

            // 20220413 判斷是主管頁面則要先查詢 BRAML_Cdata_Work_S by Kelton
            if (NavigateUrl != "P010801010001.aspx")
                cDSobj = BRAML_Cdata_Work_S.getCData_WOrk(new AML_SessionState { CASE_NO = sessionOBJ.CASE_NO });

            // 20220413 若 BRAML_Cdata_Work_S 沒有對應資料才查詢 BRAML_Cdata_Work 的 C 檔資料 by Kelton
            if (cDSobj == null)
            {
                EntityAML_Cdata_Work cDobj = BRAML_Cdata_Work.getCData_WOrk(sessionOBJ);

                if (cDobj != null)
                {
                    lbAML_Cdata_Work_WarningFlag.Text = cDobj.WarningFlag; ;
                    lbAML_Cdata_Work_FiledSAR.Text = cDobj.GroupInformationSharingNameListflag;
                    lbAML_Cdata_Work_Incorporated.Text = cDobj.Incorporated;
                    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                    //(AML C檔)被申報疑似洗錢/信用卡不良註記/AML不合作註記/警示帳戶->高風險
                    //if (cDobj.GroupInformationSharingNameListflag == "Y")
                    //    iRiskLevel.Add(3);
                    //20191119-RQ-2018-015749-002-因法遵已公告不合作不列入高風險因子，故調整SCDD計算風險等級邏輯。
                    //if (cDobj.Incorporated == "Y")//不合作
                    //    iRiskLevel.Add(3);
                    //if (cDobj.WarningFlag == "Y")//警示帳戶
                    //    iRiskLevel.Add(3);
                    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
                }
            }
            else // 20220413 若有 BRAML_Cdata_Work_S 的 C 檔資料，則使用 BRAML_Cdata_Work_S 的資料 by Kelton
            {
                lbAML_Cdata_Work_WarningFlag.Text = cDSobj.WarningFlag; ;
                lbAML_Cdata_Work_FiledSAR.Text = cDSobj.GroupInformationSharingNameListflag;
                lbAML_Cdata_Work_Incorporated.Text = cDSobj.Incorporated;

                this.Session["P010801060001_cDs"] = cDSobj;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
    }

    private String GetPersonnel(AML_SessionState sessionOBJ, ref String strMsg)
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
                    //多了以下判斷式，會導致資料缺少，所以移除
                    //if (!String.IsNullOrEmpty(listBRCH_Work[i].BRCH_CHINESE_NAME) && !String.IsNullOrEmpty(listBRCH_Work[i].BRCH_NATION))
                    //{
                    Personnel_Manager loopObj = new Personnel_Manager();
                    loopObj.Name = listBRCH_Work[i].BRCH_CHINESE_NAME;
                    loopObj.Country = listBRCH_Work[i].BRCH_NATION;
                    loopObj.Type = "BRCH";
                    loopObj.ID_SreachStatus = listBRCH_Work[i].BRCH_ID_SreachStatus;
                    //增加長姓名及羅馬
                    loopObj.Lname = listBRCH_Work[i].BRCH_CHINESE_LNAME;
                    loopObj.Romaname = listBRCH_Work[i].BRCH_ROMA;
                    listPersonnel_Manager.Add(loopObj);

                    if (String.IsNullOrEmpty(loopObj.ID_SreachStatus))
                    {
                        if (!String.IsNullOrEmpty(strMsg))
                            strMsg += ",";
                        strMsg += loopObj.Name;
                    }
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
                        loopObj.Type = "Manager";
                        loopObj.ID_SreachStatus = String.Empty;
                        //增加長姓名及羅馬
                        loopObj.Lname = listHQ_Manager[i].HCOP_BENE_LNAME;
                        loopObj.Romaname = listHQ_Manager[i].HCOP_BENE_ROMA;
                        listPersonnel_Manager.Add(loopObj);
                    }
                }
            }

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
                    if (!String.IsNullOrEmpty(listPersonnel_Manager[i].Country))
                    {
                        //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                        //20200115-RQ-2019-030155-002 -負責人/高管/實質受益人的國籍為本行所列高度或一般制裁國家->高風險                         
                        //if (isGeneralSanctionCountry(listPersonnel_Manager[i].Country))
                        //{
                        //    iRiskLevel.Add(3);
                        //}
                        //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
                        //客戶其關聯人為自然人且國籍為本行所列高度制裁國家->高風險，加提示請依指引『風險管控』章節所列核准層級授權
                        if (dtSanctionCountry.Select("CODE_ID = '" + listPersonnel_Manager[i].Country + "'").Length > 0)
                        {
                            //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                            //iRiskLevel.Add(3);
                            //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
                            if (!String.IsNullOrEmpty(strCheckRiskMsg))
                                strCheckRiskMsg += "\\n";
                            strCheckRiskMsg += WebHelper.GetShowText("01_08010600_068");
                        }
                    }

                    strHtmlValues += strLoopHtml;

                }

            }
            Personnel_HtmlPanel = "";
            //有提出下階段要移除重複的人員

            hidPersonnelHTML.Value = strHtmlValues;
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
        return strHtmlValues;
    }

    private void GetSCDDReport(AML_SessionState sessionOBJ)
    {
        try
        {
            if (sessionOBJ != null)
            {
                DataTable dtSCDDReport = new DataTable();
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
                    objEntity_SCDDReport.SR_EDD_Status = dtSCDDReport.Rows[0]["SR_EDD_Status"].ToString();
                    objEntity_SCDDReport.SR_EDD_Date = dtSCDDReport.Rows[0]["SR_EDD_Date"].ToString();//20211125_Ares_Jack_EDD完成日期

                    //txtSR_RiskNote.Text = objEntity_SCDDReport.SR_RiskNote;//20191202-RQ-2018-015749-002 mark by Peggy
                    txtSR_Explanation.Text = objEntity_SCDDReport.SR_Explanation;
                    Label1.Text = objEntity_SCDDReport.SR_User;

                    txtEDDFinished.Text = objEntity_SCDDReport.SR_EDD_Date;//20211125_Ares_Jack_EDD完成日期

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

                    if (objEntity_SCDDReport.SR_EDD_Status == "1")
                        chbSR_EDD_Status.Checked = true;
                    else
                        chbSR_EDD_Status.Checked = false;
                }

                switch (objEntity_SCDDReport.SR_RiskItem)
                {
                    case "0":
                        cbCalculatingRiskLevel.Checked = true;
                        break;
                        //20191129-RQ-2018-015749-002-因已有異常結案流程，故此處拿掉↓
                        /*
                        case "1":
                            cbCalculatingRiskLevel1.Items[0].Selected = true;
                            break;
                        case "2":
                            cbCalculatingRiskLevel1.Items[1].Selected = true;
                            break;
                        case "3":
                            cbCalculatingRiskLevel1.Items[2].Selected = true;
                            break;
                            */
                        //20191129-RQ-2018-015749-002-因已有異常結案流程，故此處拿掉↑
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

        private string _Type;
        public string Type
        {
            get
            {
                return this._Type;
            }
            set
            {
                this._Type = value;
            }
        }

        private string _ID_SreachStatus;
        public string ID_SreachStatus
        {
            get
            {
                return this._ID_SreachStatus;
            }
            set
            {
                this._ID_SreachStatus = value;
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

    private String CheckInputData()
    {
        String strReturnMsg = String.Empty;
        //20191129-RQ-2018-015749-002-因已有異常結案流程，故此處拿掉↓
        //foreach (ListItem oItem in cbCalculatingRiskLevel1.Items)
        //{
        //    if (oItem.Selected == true)
        //    {
        //        listItemsValue.Add(oItem.Value);
        //    }
        //}
        //20191129-RQ-2018-015749-002-因已有異常結案流程，故此處拿掉↑
        if (cbCalculatingRiskLevel.Checked)
            listItemsValue.Add("0");

        if (listItemsValue.Count > 1)
        {
            strReturnMsg += WebHelper.GetShowText("01_08010600_058");
        }
        else if (listItemsValue.Count == 0)
            strReturnMsg += WebHelper.GetShowText("01_08010600_059");


        if (hidNewRiskLevel.Value == "高風險")
        {
            //20191129-RQ-2018-015749-002-因已有異常結案流程，故此處拿掉↓
            //Talas 20190718 修正當高風險，已勾選不合作、商店解約或其他選項時不需要填綜合說明及審查意見
            /*
            if (cbCalculatingRiskLevel1.Items[0].Selected == false && cbCalculatingRiskLevel1.Items[1].Selected == false && cbCalculatingRiskLevel1.Items[2].Selected == false)
            {
                if (String.IsNullOrEmpty(txtSR_Explanation.Text))
                {
                    if (!String.IsNullOrEmpty(strReturnMsg))
                        strReturnMsg += "\\n";
                    strReturnMsg += WebHelper.GetShowText("01_08010600_060");
                }
            }
            */
            //20191129-RQ-2018-015749-002-因已有異常結案流程，故此處拿掉↑

            //EDD未完成，鎖定不可儲存
            //if (!chbSR_EDD_Status.Checked)
            if ((!chbSR_EDD_Status.Checked) && cbCalculatingRiskLevel.Checked && Convert.ToInt32(ViewState["NewRiskLevel"]) == 3)
            {
                if (!String.IsNullOrEmpty(strReturnMsg))
                    strReturnMsg += "\\n";
                strReturnMsg += WebHelper.GetShowText("01_08010600_069");
            }
        }
        //20211126_EDD已完成勾選
        if (this.chbSR_EDD_Status.Checked)
        {
            if (this.txtEDDFinished.Text == "")
            {
                if (!String.IsNullOrEmpty(strReturnMsg))
                    strReturnMsg += "\\n";
                strReturnMsg += WebHelper.GetShowText("01_08010600_076");//請輸入EDD完成日期
            }
            else if (this.txtEDDFinished.Text.Length != 8 || !checkDateTime(this.txtEDDFinished.Text))
            {
                if (!String.IsNullOrEmpty(strReturnMsg))
                    strReturnMsg += "\\n";
                strReturnMsg += WebHelper.GetShowText("01_08010600_077");//EDD完成日期錯誤
            }
        }

        //20191129-RQ-2018-015749-002-因已有異常結案流程，故此處拿掉↓
        /*
        if (cbCalculatingRiskLevel1.Items.Count >= 3 && cbCalculatingRiskLevel1.Items[2].Value == "3")
        {
            if (cbCalculatingRiskLevel1.Items[2].Selected == false && !String.IsNullOrEmpty(txtSR_RiskNote.Text))
            {
                if (!String.IsNullOrEmpty(strReturnMsg))
                    strReturnMsg += "\\n";
                strReturnMsg += WebHelper.GetShowText("01_08010600_061");
            }
            if (cbCalculatingRiskLevel1.Items[2].Selected == true && String.IsNullOrEmpty(txtSR_RiskNote.Text))
            {
                if (String.IsNullOrEmpty(strReturnMsg))
                    strReturnMsg += "\\n";
                strReturnMsg += WebHelper.GetShowText("01_08010600_066");
            }
        }
        */
        //20191129-RQ-2018-015749-002-因已有異常結案流程，故此處拿掉↑

        if (txtSR_Explanation.Text.Length > 300)
        {
            if (String.IsNullOrEmpty(strReturnMsg))
                strReturnMsg += "\\n";
            strReturnMsg += WebHelper.GetShowText("01_08010600_067");
        }

        //2019/04/01 Leon 修正
        if (cbCalculatingRiskLevel.Checked == true)
        {
            if (String.IsNullOrEmpty(hidAML_HQ_Work_OWNER_ID_SreachStatus.Value))
            {
                if (!String.IsNullOrEmpty(strReturnMsg))
                    strReturnMsg += "\\n";
                strReturnMsg += WebHelper.GetShowText("01_08010600_063");
            }

            if (!String.IsNullOrEmpty(hidID_SreachStatus_Msg.Value))
            {
                if (!String.IsNullOrEmpty(strReturnMsg))
                    strReturnMsg += "\\n";
                strReturnMsg += WebHelper.GetShowText("01_08010600_074") + hidID_SreachStatus_Msg.Value;
            }
        }
        return strReturnMsg;
    }

    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
    //判斷是否為一般制裁國家
    //private bool isGeneralSanctionCountry(string _Nation)
    //{
    //    bool isExist = false;

    //    if (dtGeneralSanctionCountry != null && dtGeneralSanctionCountry.Rows.Count > 0)
    //    {
    //        if (!String.IsNullOrEmpty(_Nation))
    //        {
    //            if (dtGeneralSanctionCountry.Select("CODE_ID = '" + _Nation + "'").Length > 0)
    //            {
    //                isExist = true;
    //            }
    //        }
    //    }

    //    return isExist;
    //}

    //判斷是否為高度制裁國家
    //private bool isSanctionCountry(string _Nation)
    //{
    //    bool isExist = false;

    //    if (dtSanctionCountry != null && dtSanctionCountry.Rows.Count > 0)
    //    {
    //        if (!String.IsNullOrEmpty(_Nation))
    //        {
    //            if (dtGeneralSanctionCountry.Select("CODE_ID = '" + _Nation + "'").Length > 0)
    //            {
    //                isExist = true;
    //            }
    //        }
    //    }

    //    return isExist;
    //}
    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
    #endregion
}
