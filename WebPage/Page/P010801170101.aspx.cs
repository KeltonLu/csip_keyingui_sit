// *****************************************************************
//   作    者：Ares Jack
//   功能說明：自然人收單 結案明細SCDD
//   創建日期：2021/10/08   
// <author>            <time>            <TaskID>                <desc>
// ******************************************************************
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
public partial class P010801170101 : PageBase
{
    #region 變數區

    private EntityAGENT_INFO eAgentInfo;
    public string Personnel_HtmlValue = "";
    public string Personnel_HtmlPanel = "display: none;";
    public string Signature_HtmlPanel = "display: none;";
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
        Boolean bHaveData = false;
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合

        if (!IsPostBack)
        {
            AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801140001_SESSION"];
            if (sessionOBJ == null)
            {

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
                GetPersonnel(sessionOBJ);

                if (!String.IsNullOrEmpty(strNewRiskLevel))
                    hidNewRiskLevel.Value = strNewRiskLevel;

                if (!bHaveData)
                {
                    string NavigateUrl = "P010801170001.aspx";
                    string urlString = @"alert('查無資料');location.href='" + NavigateUrl + "';";
                    base.sbRegScript.Append(urlString);
                }                
            }
        }
        
        base.strClientMsg = "";
        base.strHostMsg = "";
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        string NavigateUrl = "P010801170001.aspx";
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
                    this.SetVal<EntityAML_HQ_Work>(hqObj, false);

                    HIDE_REG_ADDR.Text = hqObj.HCOP_REG_ZIP_CODE + hqObj.HCOP_REG_CITY + hqObj.HCOP_REG_ADDR1 + hqObj.HCOP_REG_ADDR2;
                    HIDE_REG_ADDR.Visible = false;
                    HIDE_MAILING_ADDR.Text = hqObj.HCOP_MAILING_CITY + hqObj.HCOP_MAILING_ADDR1 + hqObj.HCOP_MAILING_ADDR2;
                    HIDE_MAILING_ADDR.Visible = false;

                    //身分證換補領查詢結果
                    if (hqObj.OWNER_ID_SreachStatus == "1" || hqObj.OWNER_ID_SreachStatus.ToUpper() == "Y")
                    {
                        lbAML_HQ_Work_OWNER_ID_SreachStatus.Text = "適用";
                    }
                    else if (hqObj.OWNER_ID_SreachStatus == "0" || hqObj.OWNER_ID_SreachStatus.ToUpper() == "N")
                    {
                        lbAML_HQ_Work_OWNER_ID_SreachStatus.Text = "不適用";
                    }

                    if (dtRiskCountry != null && dtRiskCountry.Rows.Count > 0)
                    {
                        //註冊國籍 AML_HQ_Work.HCOP_REGISTER_NATION
                        if (!String.IsNullOrEmpty(hqObj.HCOP_REGISTER_NATION))
                        {
                            //客戶 國籍、戶籍地 或 通訊地 或關聯人任一人 國籍 位於位於高風險國家/地區
                            if (dtRiskCountry.Select("CODE_ID = '" + hqObj.HCOP_REGISTER_NATION + "'").Length > 0)
                            {
                                lblIsRisk.Text = "Y";
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
                            }

                            //客戶 國籍、戶籍地 或 通訊地 位於一般或高度制裁國家/ 地區
                            if (dtGeneralSanctionCountry.Select("CODE_ID = '" + hqObj.HCOP_COUNTRY_CODE_2 + "'").Length > 0 ||
                                dtSanctionCountry.Select("CODE_ID = '" + hqObj.HCOP_COUNTRY_CODE_2 + "'").Length > 0)
                            {
                                lblIsSanction.Text = "Y";
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
                    List<string> temp1 = new List<string>();
                    List<string> temp2 = new List<string>();
                    List<string> temp3 = new List<string>();
                    //行業編號 Type=3
                    dtCC_Item = BRPostOffice_CodeType.GetCodeTypeByCodeID("3", hqObj.HCOP_CC);
                    dtCC_2_Item = BRPostOffice_CodeType.GetCodeTypeByCodeID("3", hqObj.HCOP_CC_2);
                    dtCC_3_Item = BRPostOffice_CodeType.GetCodeTypeByCodeID("3", hqObj.HCOP_CC_3);
                    //CC
                    if (dtCC_Item != null && dtCC_Item.Rows.Count > 0)
                    {
                        temp1.Add(hqObj.HCOP_CC + "(" + dtCC_Item.Rows[0]["CODE_NAME"].ToString() + ")");
                    }
                    else if (!string.IsNullOrEmpty(hqObj.HCOP_CC))
                    {
                        temp1.Add(hqObj.HCOP_CC);
                    }
                    //CC_2
                    if (dtCC_2_Item != null && dtCC_2_Item.Rows.Count > 0)
                    {
                        temp2.Add(hqObj.HCOP_CC_2 + "(" + dtCC_2_Item.Rows[0]["CODE_NAME"].ToString() + ")");
                    }
                    else if (!string.IsNullOrEmpty(hqObj.HCOP_CC_2))
                    {
                        temp2.Add(hqObj.HCOP_CC_2);
                    }
                    //CC_3
                    if (dtCC_3_Item != null && dtCC_3_Item.Rows.Count > 0)
                    {
                        temp3.Add(hqObj.HCOP_CC_3 + "(" + dtCC_3_Item.Rows[0]["CODE_NAME"].ToString() + ")");
                    }
                    else if (!string.IsNullOrEmpty(hqObj.HCOP_CC_3))
                    {
                        temp3.Add(hqObj.HCOP_CC_3);
                    }
                    HQlblHCOP_CC.Text = string.Join(",", temp1);
                    HQlblHCOP_CC_2.Text = string.Join(",", temp2);
                    HQlblHCOP_CC_3.Text = string.Join(",", temp3);
                    dtCC_Item.Clear();
                    dtCC_Item.Dispose();
                    dtCC_2_Item.Clear();
                    dtCC_2_Item.Dispose();
                    dtCC_3_Item.Clear();
                    dtCC_3_Item.Dispose();
                    #endregion                    

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
                DataTable dtIsSanctionCountry = new DataTable();
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

                }
            }
            else // 20220414 若有 BRAML_Cdata_Work_S 的 C 檔資料，則使用 BRAML_Cdata_Work_S 的資料 by Kelton
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
            //}

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
                    objEntity_SCDDReport.SR_EDD_Date = dtSCDDReport.Rows[0]["SR_EDD_Date"].ToString();//20211026_Ares_Jack_EDD完成日期

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
                    lbSR_EDD_Date.Text = "EDD完成日期：" + objEntity_SCDDReport.SR_EDD_Date;//20211026_Ares_Jack_EDD完成日期
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

    private clsSCDDPrintNaturalPerson getPageValue()
    {
        clsSCDDPrintNaturalPerson clsItem = new clsSCDDPrintNaturalPerson();
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
        clsSCDDPrintNaturalPerson citem = getPageValue();
        List<clsMangger> clsObj = new List<clsMangger>();
        if (Session["clsManager"] != null)
        {
            clsObj = Session["clsManager"] as List<clsMangger>;
        }

        citem.lbLabel1 = Label1.Text;//經辦(簽章)
        citem.lbLabel2 = Label2.Text;//經辦(簽章)日期
        citem.lbLabel9 = Label9.Text;//經辦人宣示
        citem.SR_Explanation = lbSR_EDD_Date.Text + "\n " + lbSR_Explanation.Text;//20211126_Ares_Jack_EDD完成日期
        #region 主要收入來源
        List<string> temp = new List<string>();

        citem.HQlblHCOP_INCOME_SOURCE = ""; //客戶收入及資產主要收入來源
        if (chkIncome1.Checked)
            temp.Add(WebHelper.GetShowText("01_01090600_035"));//薪資
        if (chkIncome2.Checked)
            temp.Add(WebHelper.GetShowText("01_01090600_036"));//經營事業收入
        if (chkIncome3.Checked)
            temp.Add(WebHelper.GetShowText("01_01090600_037"));//退休(職)資金
        if (chkIncome4.Checked)
            temp.Add(WebHelper.GetShowText("01_01090600_038"));//遺產繼承(含贈與)
        if (chkIncome5.Checked)
            temp.Add(WebHelper.GetShowText("01_01090600_039"));//買賣房地產
        if (chkIncome6.Checked)
            temp.Add(WebHelper.GetShowText("01_01090600_040"));//投資理財
        if (chkIncome7.Checked)
            temp.Add(WebHelper.GetShowText("01_01090600_041"));//租金收入
        if (chkIncome8.Checked)
            temp.Add(WebHelper.GetShowText("01_01090600_042"));//存款
        if (chkIncome9.Checked)
            temp.Add(WebHelper.GetShowText("01_01090600_043"));//其他

        citem.HQlblHCOP_INCOME_SOURCE = string.Join("、", temp);
        #endregion
        citem.CustLabel17 = WebHelper.GetShowText("01_01090600_033"); //業務往來目的 收單業務
        citem.lbAML_HQ_Work_OWNER_ID_SreachStatus = WebHelper.GetShowText("01_01090600_034"); //身分證換補領查詢結果 適用


        citem.ManagerColl = clsObj;

        //end
        string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
        if (!Directory.Exists(strServerPathFile))
        {
            Directory.CreateDirectory(strServerPathFile);
        }
        string msgID = citem.AML_HQ_Work_HCOP_HEADQUATERS_CORP_NO; //統編
        bool result = BRSCDDReport.CreateExcelFileNaturalPerson(citem, "", ref strServerPathFile, ref msgID, "SCDD_VIEW_NEW_NATURAL.xls");
        if (result)
        {

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
