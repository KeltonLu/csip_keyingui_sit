// *****************************************************************
//   作    者：Ares Dennis
//   功能說明：自然人收單SCDD
//   創建日期：2021/08/06   
// <author>            <time>            <TaskID>                <desc>
// ******************************************************************

using System;
using System.Data;
using System.Web.UI.WebControls;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Logging;
using Framework.Common.Utility;
using CSIPNewInvoice.EntityLayer_new;
using CSIPKeyInGUI.BusinessRules_new;
using System.Collections.Generic;

public partial class P010109060001 : PageBase
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
    private DataTable dtGeneralSanctionCountry = new DataTable();//記錄一般制裁國家的清單

    #endregion

    #region 事件區
    /// <summary>    
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
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
                    //一般制裁國家 Type = 15
                    dtGeneralSanctionCountry = BRPostOffice_CodeType.GetCodeType("15");

                    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                    //iRiskLevel.Add(1);
                    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
                    //讀取公司資料 HQ_WORK CDATA,EDATA
                    GetAML_HQ_Work(sessionOBJ);
                    GetHQ_SCDD(sessionOBJ);
                    GetAML_Cdata_Work(sessionOBJ);
                    GetSCDDReport(sessionOBJ);
                    Personnel_HtmlValue = GetPersonnel(sessionOBJ, ref strMsg);
                    iRiskLevel = riskLevelCalculateService.NaturalPersonRiskLevelCalculate(sessionOBJ);
                }
                catch (Exception ex)
                {
                    Logging.Log(ex, LogLayer.UI);
                }

                //20220127_Ares_Jack_風險等級只由經辦頁面計算
                //20220209_Ares_Jack_EDD完成日期、風險等級勾選只由經辦頁面修改
                string NavigateUrl = "P010801150001.aspx";
                if (!string.IsNullOrEmpty(Session["P010801150001_Last"].ToString()))
                {
                    NavigateUrl = Session["P010801150001_Last"].ToString();
                }
                if (NavigateUrl == "P010801150001.aspx")
                {
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
            }
            if (!String.IsNullOrEmpty(strCheckRiskMsg))
            {
                string urlString = @"alert('[MSG]');";
                urlString = urlString.Replace("[MSG]", strCheckRiskMsg);
                base.sbRegScript.Append(urlString);
            }
        }

        base.strClientMsg = "";
        base.strHostMsg = "";

    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
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
                objEdit.SR_RiskNote = "";
                objEdit.SR_User = eAgentInfo.agent_id;
                if (chbSR_EDD_Status.Checked)
                    objEdit.SR_EDD_Status = "1";
                else
                    objEdit.SR_EDD_Status = "0";

                //20211026_Ares_Jack_ EDD 完成日期
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
        string NavigateUrl = "P010801150001.aspx";
        //有可能是由主管頁來，需判斷
        if (!string.IsNullOrEmpty(Session["P010801150001_Last"].ToString()))
        {
            NavigateUrl = Session["P010801150001_Last"].ToString();
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

        string NavigateUrl = "P010109600001.aspx";
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

                    this.SetVal<EntityAML_HQ_Work>(hqObj, false);

                    if (dtRiskCountry != null && dtRiskCountry.Rows.Count > 0)
                    {
                        //註冊國籍 AML_HQ_Work.HCOP_REGISTER_NATION
                        if (!String.IsNullOrEmpty(hqObj.HCOP_REGISTER_NATION))
                        {
                            //客戶 國籍、戶籍地 或 通訊地 或關聯人任一人 國籍 位於位於高風險國家/地區
                            if (dtRiskCountry.Select("CODE_ID = '" + hqObj.HCOP_REGISTER_NATION + "'").Length > 0)
                            {
                                lblIsRisk.Text = "Y";
                                //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                                //iRiskLevel.Add(3);
                                //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
                            }
                            else
                            {
                                lblIsRisk.Text = "N";
                            }

                            //客戶 國籍、戶籍地 或 通訊地 位於一般或高度制裁國家/ 地區
                            if (dtGeneralSanctionCountry.Select("CODE_ID = '" + hqObj.HCOP_REGISTER_NATION + "'").Length > 0 ||
                                dtSanctionCountry.Select("CODE_ID = '" + hqObj.HCOP_REGISTER_NATION + "'").Length > 0)
                            {
                                lblIsSanction.Text = "Y";
                                //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                                //iRiskLevel.Add(3);
                                //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
                            }
                            else
                            {
                                lblIsSanction.Text = "N";
                            }
                        }
                        //20220111_Ares_Jack_加上國籍2判斷風險等級
                        if (!String.IsNullOrEmpty(hqObj.HCOP_COUNTRY_CODE_2))
                        {
                            //客戶 國籍、戶籍地 或 通訊地 或關聯人任一人 國籍 位於位於高風險國家/地區
                            if (dtRiskCountry.Select("CODE_ID = '" + hqObj.HCOP_COUNTRY_CODE_2 + "'").Length > 0)
                            {
                                lblIsRisk.Text = "Y";
                                //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                                //iRiskLevel.Add(3);
                                //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
                            }

                            //客戶 國籍、戶籍地 或 通訊地 位於一般或高度制裁國家/ 地區
                            if (dtGeneralSanctionCountry.Select("CODE_ID = '" + hqObj.HCOP_COUNTRY_CODE_2 + "'").Length > 0 ||
                                dtSanctionCountry.Select("CODE_ID = '" + hqObj.HCOP_COUNTRY_CODE_2 + "'").Length > 0)
                            {
                                lblIsSanction.Text = "Y";
                                //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                                //iRiskLevel.Add(3);
                                //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
                            }
                        }
                    }

                    #region 職業別
                    DataTable dtOC_Item = new DataTable();
                    //職業別 Type=16
                    dtOC_Item = BRPostOffice_CodeType.GetCodeTypeByCodeID("16", hqObj.HCOP_OC);
                    if (dtOC_Item != null && dtOC_Item.Rows.Count > 0)
                        HQlblHCOP_OC.Text = hqObj.HCOP_OC + "(" + dtOC_Item.Rows[0]["CODE_NAME"].ToString() + ")";
                    else
                        HQlblHCOP_OC.Text = hqObj.HCOP_OC;
                    dtOC_Item.Clear();
                    dtOC_Item.Dispose();
                    #endregion
                    #region 主要收入來源                    
                    //收入及資產來源
                    if (!string.IsNullOrEmpty(hqObj.HCOP_INCOME_SOURCE))
                    {
                        string incomeSource = hqObj.HCOP_INCOME_SOURCE;
                        for (int i = 1; i < 10; i++)
                        {
                            if (incomeSource.IndexOf('1', i - 1, 1) != -1)//被勾選的是1
                            {
                                incomeSource = incomeSource.Remove(i - 1, 1).Insert(i - 1, i.ToString());// ex:101000000 => 1,3
                            }
                        }
                        incomeSource = string.Join(",", incomeSource.Replace("0", "").ToCharArray());//清除0,剩餘的用逗號分開 ex:1,2,3

                        string[] incomes = incomeSource.Split(',');
                        foreach (string income in incomes)
                        {
                            for (int i = 1; i <= 9; i++)
                            {
                                if (income == i.ToString())
                                {
                                    CheckBox checkBox = (CheckBox)FindControl("chkIncome" + i.ToString());
                                    checkBox.Checked = true;
                                }
                            }
                        }
                    }
                    #endregion
                    #region 行業別
                    DataTable dtCC_Item = new DataTable();
                    DataTable dtCC_2_Item = new DataTable();
                    DataTable dtCC_3_Item = new DataTable();
                    List<string> temp = new List<string>();
                    //行業編號 Type=3 
                    dtCC_Item = BRPostOffice_CodeType.GetCodeTypeByCodeID("3", hqObj.HCOP_CC);
                    dtCC_2_Item = BRPostOffice_CodeType.GetCodeTypeByCodeID("3", hqObj.HCOP_CC_2);
                    dtCC_3_Item = BRPostOffice_CodeType.GetCodeTypeByCodeID("3", hqObj.HCOP_CC_3);
                    //CC
                    if (dtCC_Item != null && dtCC_Item.Rows.Count > 0)
                    {
                        temp.Add(hqObj.HCOP_CC + "(" + dtCC_Item.Rows[0]["CODE_NAME"].ToString() + ")");

                    }
                    else if (!string.IsNullOrEmpty(hqObj.HCOP_CC))
                    {
                        temp.Add(hqObj.HCOP_CC);
                    }
                    //CC_2
                    if (dtCC_2_Item != null && dtCC_2_Item.Rows.Count > 0)
                    {
                        temp.Add(hqObj.HCOP_CC_2 + "(" + dtCC_2_Item.Rows[0]["CODE_NAME"].ToString() + ")");

                    }
                    else if (!string.IsNullOrEmpty(hqObj.HCOP_CC_2))
                    {
                        temp.Add(hqObj.HCOP_CC_2);
                    }
                    //CC_3
                    if (dtCC_3_Item != null && dtCC_3_Item.Rows.Count > 0)
                    {
                        temp.Add(hqObj.HCOP_CC_3 + "(" + dtCC_3_Item.Rows[0]["CODE_NAME"].ToString() + ")");

                    }
                    else if (!string.IsNullOrEmpty(hqObj.HCOP_CC_3))
                    {
                        temp.Add(hqObj.HCOP_CC_3);
                    }
                    HQlblHCOP_CC.Text = string.Join(",", temp);
                    dtCC_Item.Clear();
                    dtCC_Item.Dispose();
                    dtCC_2_Item.Clear();
                    dtCC_2_Item.Dispose();
                    dtCC_3_Item.Clear();
                    dtCC_3_Item.Dispose();
                    #endregion

                    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                    //DataTable result = new DataTable();
                    //#region 20220104_Area_Jack_高風險行業別編號 高風險職稱編號
                    //string amlcc = string.Empty;
                    //result = BRPostOffice_CodeType.GetCodeType("21");
                    //if (result != null && result.Rows.Count > 0)
                    //{
                    //    for (int i = 0; i < result.Rows.Count; i++)
                    //    {
                    //        amlcc += result.Rows[i][0].ToString() + ":";
                    //    }
                    //    this.hidAMLCC.Value = amlcc;
                    //}
                    //string[] allAMLCC = hidAMLCC.Value.Split(':');

                    //string oc = string.Empty;
                    //result = BRPostOffice_CodeType.GetCodeType("20");
                    //if (result != null && result.Rows.Count > 0)
                    //{
                    //    for (int i = 0; i < result.Rows.Count; i++)
                    //    {
                    //        oc += result.Rows[i][0].ToString() + ":";
                    //    }
                    //    this.hidOC.Value = oc;
                    //}
                    //string[] allAMLOC = hidOC.Value.Split(':');

                    //if (hqObj.HCOP_CC.Trim() != "" && hqObj.HCOP_OC.Trim() != "")
                    //{
                    //    if (Array.IndexOf(allAMLCC, hqObj.HCOP_CC.Trim()) != -1 && Array.IndexOf(allAMLOC, hqObj.HCOP_OC.Trim()) != -1)//行業別編號1為高風險 && 職稱編號為高風險
                    //        iRiskLevel.Add(3);
                    //}
                    //if (hqObj.HCOP_CC_2.Trim() != "" && hqObj.HCOP_OC.Trim() != "")
                    //{
                    //    if (Array.IndexOf(allAMLCC, hqObj.HCOP_CC_2.Trim()) != -1 && Array.IndexOf(allAMLOC, hqObj.HCOP_OC.Trim()) != -1)//行業別編號2為高風險 && 職稱編號為高風險
                    //        iRiskLevel.Add(3);
                    //}
                    //if (hqObj.HCOP_CC_3.Trim() != "" && hqObj.HCOP_OC.Trim() != "")
                    //{
                    //    if (Array.IndexOf(allAMLCC, hqObj.HCOP_CC_3.Trim()) != -1 && Array.IndexOf(allAMLOC, hqObj.HCOP_OC.Trim()) != -1)//行業別編號3為高風險 && 職稱編號為高風險
                    //        iRiskLevel.Add(3);
                    //}
                    //#endregion
                    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End

                    //20211208_Ares_Jack_新增欄位 商店狀態
                    switch (hqObj.HCOP_STATUS.ToUpper())
                    {
                        case "O":
                            lbAML_HQ_Work_HCOP_STATUS.Text = WebHelper.GetShowText("01_08010600_071");//OPEN
                            break;
                        case "C":
                            lbAML_HQ_Work_HCOP_STATUS.Text = WebHelper.GetShowText("01_08010600_072");//CLOSE
                            break;
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
                //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                //DataTable dtIsSanctionCountry = new DataTable();
                //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End

                if (sccdObj != null)
                {
                    this.SetVal<EntityHQ_SCDD>(sccdObj, false);

                    hidHQ_SCDD_NameCheck_Item.Value = sccdObj.NameCheck_Item; //20211018_Ares_Jack_取消註解 名單掃描紀錄
                    lbHQ_SCDD_NameCheck_Note.Text = sccdObj.NameCheck_Note;

                    if (!String.IsNullOrEmpty(sccdObj.NameCheck_Item))
                    {
                        try
                        {
                            DataTable dtNameCheck_Item = new DataTable();
                            //名單掃描結果項目 Type=5
                            dtNameCheck_Item = BRPostOffice_CodeType.GetCodeTypeByCodeID("5", sccdObj.NameCheck_Item);
                            //AML名單掃描結果
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
                    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                    //(Name Check)客戶為國外PEP /國外PEP利益關聯人/NN /制裁名單->高風險  
                    //(Name Check)國內PEP/國際組織PEP/國內PEP利益關聯人/國際組織PEP利益關聯人->中風險 //20211022_Ares_Jack_調整為高風險
                    //if (sccdObj.NameCheck_Item == "2")
                    //    iRiskLevel.Add(3);
                    //else if (sccdObj.NameCheck_Item == "3")
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

    private void GetAML_Cdata_Work(AML_SessionState sessionOBJ)
    {
        try
        {
            // 20220415 個/法人的SCDD C檔TABLE 主管頁面看到的要是KEEP住經辦送出時的資料，故將原邏輯註解 Start By Kelton
            //EntityAML_Cdata_Work cDobj = BRAML_Cdata_Work.getCData_WOrk(sessionOBJ);

            //if (cDobj != null)
            //{
            //    this.SetVal<EntityAML_Cdata_Work>(cDobj, false);

            //    CDlblWarningFlag.Text = cDobj.WarningFlag; //客戶帳戶為警示或衍生帳戶
            //    CDlblGroupInformationSharingNameListflag.Text = cDobj.GroupInformationSharingNameListflag; //集團關注名單
            //    CDlblIncorporated.Text = cDobj.Incorporated; //不合作/拒絕提供資訊
            //    if (cDobj.CreditCardBlockCode == null || cDobj.CreditCardBlockCode.Trim() == "")
            //    {
            //        CDlblCreditCardBlockCode.Text = "";
            //    }
            //    else
            //    {
            //        CDlblCreditCardBlockCode.Text = cDobj.CreditCardBlockCode == "0000000000" ? "N" : "Y";//信用卡Block Code 正常戶:10個0
            //    }
            //    CDlblNNListHitFlag.Text = cDobj.NNListHitFlag;//20211026_Ares_Jack_負面新聞

            //    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
            //    ////(AML C檔)被申報疑似洗錢/信用卡不良註記/AML不合作註記/警示帳戶->高風險
            //    //if (cDobj.GroupInformationSharingNameListflag == "Y")
            //    //    iRiskLevel.Add(3);
            //    //if (cDobj.WarningFlag == "Y")//警示帳戶
            //    //    iRiskLevel.Add(3);
            //    //if (this.CDlblCreditCardBlockCode.Text == "Y")//20211026_Ares_Jack_信用卡不良註記
            //    //    iRiskLevel.Add(3);
            //    //if (cDobj.NNListHitFlag == "Y")//20211026_Ares_Jack_負面新聞
            //    //    iRiskLevel.Add(3);
            //    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
            //}
            // 20220415 個/法人的SCDD C檔TABLE 主管頁面看到的要是KEEP住經辦送出時的資料，故將原邏輯註解 End By Kelton

            // 20220415 取得來源頁面路徑，用來判斷是經辦還是主管頁 by Kelton
            string NavigateUrl = "P010801150001.aspx";
            if (!string.IsNullOrEmpty(Session["P010801150001_Last"].ToString()))
            {
                NavigateUrl = Session["P010801150001_Last"].ToString();
            }

            EntityAML_Cdata_Work_S cDSobj = null;

            // 20220413 判斷是主管頁面則要先查詢 BRAML_Cdata_Work_S by Kelton
            if (NavigateUrl != "P010801150001.aspx")
                cDSobj = BRAML_Cdata_Work_S.getCData_WOrk(new AML_SessionState { CASE_NO = sessionOBJ.CASE_NO });

            // 20220413 若 BRAML_Cdata_Work_S 沒有對應資料才查詢 BRAML_Cdata_Work 的 C 檔資料 by Kelton
            if (cDSobj == null)
            {
                EntityAML_Cdata_Work cDobj = BRAML_Cdata_Work.getCData_WOrk(sessionOBJ);

                if (cDobj != null)
                {
                    this.SetVal<EntityAML_Cdata_Work>(cDobj, false);

                    CDlblWarningFlag.Text = cDobj.WarningFlag; //客戶帳戶為警示或衍生帳戶
                    CDlblGroupInformationSharingNameListflag.Text = cDobj.GroupInformationSharingNameListflag; //集團關注名單
                    CDlblIncorporated.Text = cDobj.Incorporated; //不合作/拒絕提供資訊
                    if (cDobj.CreditCardBlockCode == null || cDobj.CreditCardBlockCode.Trim() == "")
                    {
                        CDlblCreditCardBlockCode.Text = "";
                    }
                    else
                    {
                        CDlblCreditCardBlockCode.Text = cDobj.CreditCardBlockCode == "0000000000" ? "N" : "Y";//信用卡Block Code 正常戶:10個0
                    }
                    CDlblNNListHitFlag.Text = cDobj.NNListHitFlag;//20211026_Ares_Jack_負面新聞

                    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 Start
                    ////(AML C檔)被申報疑似洗錢/信用卡不良註記/AML不合作註記/警示帳戶->高風險
                    //if (cDobj.GroupInformationSharingNameListflag == "Y")
                    //    iRiskLevel.Add(3);
                    //if (cDobj.WarningFlag == "Y")//警示帳戶
                    //    iRiskLevel.Add(3);
                    //if (this.CDlblCreditCardBlockCode.Text == "Y")//20211026_Ares_Jack_信用卡不良註記
                    //    iRiskLevel.Add(3);
                    //if (cDobj.NNListHitFlag == "Y")//20211026_Ares_Jack_負面新聞
                    //    iRiskLevel.Add(3);
                    //20220311_Ares_Jack_風險等級計算已調整為共用function , 故將原計算邏輯註解 End
                }
            }
            else // 20220413 若有 BRAML_Cdata_Work_S 的 C 檔資料，則使用 BRAML_Cdata_Work_S 的資料 by Kelton
            {
                this.SetVal<EntityAML_Cdata_Work_S>(cDSobj, false);

                CDlblWarningFlag.Text = cDSobj.WarningFlag; //客戶帳戶為警示或衍生帳戶
                CDlblGroupInformationSharingNameListflag.Text = cDSobj.GroupInformationSharingNameListflag; //集團關注名單
                CDlblIncorporated.Text = cDSobj.Incorporated; //不合作/拒絕提供資訊
                if (cDSobj.CreditCardBlockCode == null || cDSobj.CreditCardBlockCode.Trim() == "")
                {
                    CDlblCreditCardBlockCode.Text = "";
                }
                else
                {
                    CDlblCreditCardBlockCode.Text = cDSobj.CreditCardBlockCode == "0000000000" ? "N" : "Y";//信用卡Block Code 正常戶:10個0
                }
                CDlblNNListHitFlag.Text = cDSobj.NNListHitFlag;//20211026_Ares_Jack_負面新聞

                this.Session["P010109060001_cDs"] = cDSobj;
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
            List<EntityAML_BRCH_Work> listBRCH_Work = BRAML_BRCH_Work.getBRCH_WOrkColl(sessionOBJ);
            List<Personnel_Manager> listPersonnel_Manager = new List<Personnel_Manager>();

            if (listBRCH_Work != null && listBRCH_Work.Count > 0)
            {
                for (int i = 0; i < listBRCH_Work.Count; i++)
                {
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
                        //負責人/高管/實質受益人的國籍為本行所列高度或一般制裁國家->高風險                         
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
                    objEntity_SCDDReport.SR_EDD_Date = dtSCDDReport.Rows[0]["SR_EDD_Date"].ToString();//20211026_Ares_Jack_EDD完成日期

                    txtSR_Explanation.Text = objEntity_SCDDReport.SR_Explanation;

                    txtEDDFinished.Text = objEntity_SCDDReport.SR_EDD_Date;//20211026_Ares_Jack_EDD完成日期                    

                    if (!String.IsNullOrEmpty(dtSCDDReport.Rows[0]["SR_DateTime"].ToString()))
                    {
                        try
                        {
                            objEntity_SCDDReport.SR_DateTime = Convert.ToDateTime(dtSCDDReport.Rows[0]["SR_DateTime"].ToString());
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
            //EDD未完成，鎖定不可儲存            
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

        if (txtSR_Explanation.Text.Length > 300)
        {
            if (String.IsNullOrEmpty(strReturnMsg))
                strReturnMsg += "\\n";
            strReturnMsg += WebHelper.GetShowText("01_08010600_067");
        }
        //20211129_Ares_Jack_ 與香琦討論後註解換領補查詢紀錄檢核        
        //if (cbCalculatingRiskLevel.Checked == true)
        //{
        //    if (String.IsNullOrEmpty(hidAML_HQ_Work_OWNER_ID_SreachStatus.Value))
        //    {
        //        if (!String.IsNullOrEmpty(strReturnMsg))
        //            strReturnMsg += "\\n";
        //        strReturnMsg += WebHelper.GetShowText("01_08010600_063");
        //    }

        //    //if (!String.IsNullOrEmpty(hidID_SreachStatus_Msg.Value))
        //    //{
        //    //    if (!String.IsNullOrEmpty(strReturnMsg))
        //    //        strReturnMsg += "\\n";
        //    //    strReturnMsg += WebHelper.GetShowText("01_08010600_074") + hidID_SreachStatus_Msg.Value;
        //    //}
        //}
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

    ////判斷是否為高度制裁國家
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
