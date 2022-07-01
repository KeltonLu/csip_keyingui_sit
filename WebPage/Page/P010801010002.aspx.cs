//******************************************************************
//*  作    者：陳香琦
//*  功能說明：異常結案時，編輯log
//*  創建日期：2019/11/04
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Message;
using CSIPNewInvoice.EntityLayer_new;
using Framework.Data.OM.Transaction;
using System.Collections;
using Framework.Common.Logging;//20200413
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Framework.Common.Utility;

public partial class Page_P010801010002 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    private EntityAGENT_INFO eAgentInfo;
    private string _HCOP_STATUS = string.Empty;//商店狀態
    private string _HCOP_QUALIFY_FLAG = string.Empty;//符合資格
    private List<string> ID = new List<string>();//關聯案件序號ID
    private int iRiskLevel;
    private String strNewRiskLevel = String.Empty;
    private EntityAML_Cdata_Work_S tempcDSobj = new EntityAML_Cdata_Work_S(); // 20220413 暫存查詢到的 C 檔資料，送審時要寫入 AML_Cdata_Work_S By Kelton
    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合

        if (!IsPostBack)
        {
            AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
            RiskLevelCalculateService riskLevelCalculateService = new RiskLevelCalculateService();

            if (sessionOBJ == null)
            {
                string NavigateUrl = "P010801000001.aspx";
                string urlString = @"alert('查無資料');location.href='" + NavigateUrl + "';";
                base.sbRegScript.Append(urlString);
            }

            if (sessionOBJ != null)
            {
                //讀取公司資料 HQ_WORK CDATA,EDATA
                EntityAML_HQ_Work hqObj = BRAML_HQ_Work.getHQ_WOrk(sessionOBJ);
                lbAML_HQ_Work_CASE_NO.Text = hqObj.CASE_NO;
                hidAML_HQ_Work_CASE_NO.Value = hqObj.CASE_NO;
                lbAML_HQ_Work_HCOP_HEADQUATERS_CORP_NO.Text = hqObj.HCOP_HEADQUATERS_CORP_NO;
                lbAML_HQ_Work_HCOP_REG_NAME.Text = hqObj.HCOP_REG_NAME;

                //案件退件時，default帶入原先的結案方式
                switch (hqObj.CaseProcess_Status.Trim())
                {
                    case "13":
                        rblAbnormal_I.Checked = true;
                        break;
                    case "14":
                        rblAbnormal_C.Checked = true;
                        break;
                    case "15":
                        rblAbnormal_O.Checked = true;
                        break;
                    default:
                        break;

                }

                //20220311_Ares_Jack_計算風險等級
                string LastPage = "P010801010001.aspx";//法人
                if (!string.IsNullOrEmpty(Session["P010801010001_Last"].ToString()))
                {
                    LastPage = Session["P010801010001_Last"].ToString();
                }
                if (LastPage == "P010801010001.aspx")//法人
                    iRiskLevel = riskLevelCalculateService.RiskLevelCalculate(sessionOBJ);
                if (LastPage == "P010801150001.aspx")//自然人
                    iRiskLevel = riskLevelCalculateService.NaturalPersonRiskLevelCalculate(sessionOBJ);

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

                if (!String.IsNullOrEmpty(strNewRiskLevel))
                    this.lbCalculatingRiskLevel.Text = strNewRiskLevel;

                // 2021/7/6 EOS_AML(NOVA) by Ares Dennis
                //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
                //if (!string.IsNullOrEmpty(hqObj.GROUP_NO))
                //{                   
                //    DataTable dtCaseNo = BRAML_HQ_Work.getRelatedCaseNo(hqObj.GROUP_NO);
                //    for (int i = 0; i < dtCaseNo.Rows.Count; i++)
                //    {                        
                //        ID.Add(dtCaseNo.Rows[i][1].ToString());
                //    }                    
                //}
                //20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis


                // 20220413 將查詢到的 C 檔資料暫存，最後送審時要寫入 AML_Cdata_Work_S By Kelton
                //讀取風險資料 Cdata_Work
                EntityAML_Cdata_Work cDobj = BRAML_Cdata_Work.getCData_WOrk(sessionOBJ);

                tempcDSobj = cDobj.toSMode();
                tempcDSobj.CASE_NO = sessionOBJ.CASE_NO;
                this.Session["P010801010002_cDs"] = tempcDSobj;
            }
        }
        base.strClientMsg = "";
        base.strHostMsg = "";
    }

    /// <summary>
    /// 提交事件
    /// </summary>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        //送審要變更 CaseOwner_User 經辦，以目前帶入 MI等級變更為C1
        AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];

        if (sessionOBJ == null || eAgentInfo == null)
        {
            string strAlertMsg = MessageHelper.GetMessages("01_01080101_002");//作業逾時，請重試
            sbRegScript.Append("alert('" + strAlertMsg + "');window.location.href = 'P010801000001.aspx';");
            return;
        }

        string bMsg = string.Empty;//記錄主機回傳訊息
        //20210423-RQ-2021-004136-003-經姵晴確認，異常結案僅限解約結案時需打主機
        if (rblAbnormal_C.Checked)//解約結案
        {
            //讀取主機資料資料 ，確認特店即時狀態後判斷是否可解約
            bool bResult = false;//記錄主機是否有資料
            GetHTGMsg(lbAML_HQ_Work_HCOP_HEADQUATERS_CORP_NO.Text.Trim(), ref bResult, ref bMsg);
            if (bResult)
            {
                //若商店狀態為open(HCOP-STATUS=Open and HCOP-QUALIFY-FLAG=Y)則出現錯誤訊息：商店未解約，無法結案，不能按申請送審
                if (rblAbnormal_C.Checked && (_HCOP_STATUS.ToUpper().Trim().Equals("O") && _HCOP_QUALIFY_FLAG.ToUpper().Trim().Equals("Y")))
                {
                    string strAlertMsg = MessageHelper.GetMessages("01_01080101_009");//商店未解約，無法結案
                    sbRegScript.Append("alert('" + strAlertMsg + "');");
                    return;
                }
            }
            else
            {
                sbRegScript.Append("alert('" + bMsg + "');");
                return; //如果找不到資料，即退出
            }
        }

        sessionOBJ.CaseProcess_User = "C1"; //一級主管
        sessionOBJ.CaseOwner_User = eAgentInfo.agent_id;
        //處理中, 運用Status來區分結案累別 3:不合作結案，4:商店解約結案 5:其他
        string _NLType = string.Empty;
        if (rblAbnormal_I.Checked)
        {
            sessionOBJ.CaseProcess_Status = "3";
            _NLType = "NonCooperated";
        }

        if (rblAbnormal_C.Checked)
        {
            sessionOBJ.CaseProcess_Status = "4";
            _NLType = "CaseClosed";
        }

        if (rblAbnormal_O.Checked)
        {
            sessionOBJ.CaseProcess_Status = "5";
            _NLType = "OtherClosed";
        }


        
        //20220311_Ares_Jack_寫入SCDDReport
        Entity_SCDDReport objEdit = new Entity_SCDDReport();
        string urlString = @"alert('[MSG]');";
        String strCheckMsg = String.Empty;


        if (!String.IsNullOrEmpty(strCheckMsg))
        {
            urlString = urlString.Replace("[MSG]", strCheckMsg);
            base.sbRegScript.Append(urlString);
            return;
        }
        else
        {
            Boolean blSubmitStatus = false;
            DataTable dtSCDDReport = new DataTable();

            objEdit.SR_CASE_NO = hidAML_HQ_Work_CASE_NO.Value;

            SCDDReport.GetSCDDReportDataTable(objEdit, "", ref dtSCDDReport);

            objEdit.SR_DateTime = DateTime.Now;
            objEdit.SR_Explanation = "";
            objEdit.SR_RiskLevel = this.lbCalculatingRiskLevel.Text.Trim();//風險等級
            objEdit.SR_RiskItem = "0";
            objEdit.SR_RiskNote = "";
            objEdit.SR_User = eAgentInfo.agent_id;
            objEdit.SR_EDD_Date = "";//EDD 完成日期
            objEdit.SR_EDD_Status = "";


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

            if (!blSubmitStatus)
            {
                urlString = urlString.Replace("[MSG]", WebHelper.GetShowText("01_08010600_065"));
                base.sbRegScript.Append(urlString);
                return;
            }

        }
        

        bool result;
        using (OMTransactionScope ts = new OMTransactionScope())
        {

            result = BRAML_HQ_Work.Update_Apply(sessionOBJ, "1"); //送審不動經辦

            //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
            //2021/7/6 EOS_AML(NOVA) 關聯案件一併送出 by Ares Dennis 
            //foreach (string id in ID)
            //{
            //    sessionOBJ.ID = id;
            //    result = BRAML_HQ_Work.Update_Apply(sessionOBJ, "1"); //送審不動經辦
            //}
            //20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis

            if (!result)
            {
                string strAlertMsg = MessageHelper.GetMessages("01_01080101_005");
                sbRegScript.Append("alert('" + strAlertMsg + "');");
                return;
            }

            //寫入案件歷程
            EntityNoteLog notrLog = new EntityNoteLog();
            notrLog.NL_CASE_NO = sessionOBJ.CASE_NO;
            notrLog.NL_SecondKey = "";
            notrLog.NL_ShowFlag = "1";
            notrLog.NL_DateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            notrLog.NL_User = eAgentInfo.agent_id;
            notrLog.NL_Type = _NLType.Trim();
            // 20220620 調整將文字內容的 \r\n 換行符號替換成頁面顯示換行用的 <br /> By Kelton
            //notrLog.NL_Value = txtNoteLog_NL_Value.Text.Trim();
            string noteLog = txtNoteLog_NL_Value.Text;
            if (noteLog.Contains("\r\n"))
            {
                noteLog = noteLog.Replace("\r\n", "<br />").Trim();
            }
            if (noteLog.Contains("\n"))
            {
                noteLog = noteLog.Replace("\n", "<br />").Trim();
            }
            notrLog.NL_Value = noteLog;
            result = BRNoteLog.Insert(notrLog);
            //調整為提示訊息
            if (!result)
            {
                string strAlertMsg = MessageHelper.GetMessages("01_01080101_006");
                sbRegScript.Append("alert('" + strAlertMsg + "');");

                return;
            }

            // 20220413 先將 AML_Cdata_Work_S 已存在的資料刪除，再將 C 檔資料寫入 AML_Cdata_Work_S by Kelton
            BRAML_Cdata_Work_S.Delete(new AML_SessionState { CASE_NO = sessionOBJ.CASE_NO });
            tempcDSobj = (EntityAML_Cdata_Work_S)this.Session["P010801010002_cDs"];
            BRAML_Cdata_Work_S.Insert(tempcDSobj);

            ts.Complete();
        }

        //提示作業完成
        string strAlertMsgR = MessageHelper.GetMessages("01_01080101_007");//送審存檔完成
        //sbRegScript.Append("alert('" + strAlertMsgR + "');window.opener.location.href = 'P010801000001.aspx';window.close();");
        //20210628_Ares_Stanley-導頁到特店AML資料-定期審查案件列表
        string NavigateUrl = "P010801000001.aspx";
        Response.Write("<script language='javascript'>window.alert('" + strAlertMsgR + "');window.location='" + NavigateUrl + "';</script>");

    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        // string urlString = @"window.close();";
        // base.sbRegScript.Append(urlString);


        //20210426_Ares_Luke-調整頁面轉向方法(避免造成瀏覽器雙開被剔除)
        string lastPage = Session["P010801010001_Last"].ToString();
        //取得上一頁
        if (lastPage != "")
        {
            Response.Redirect(lastPage, false);
        }
        else
        {
            Response.Redirect("P010801000001.aspx", false);
        }

    }
    #endregion

    #region 方法區
    //讀取總公司即時資訊
    private void GetHTGMsg(string TaxID, ref bool bResult,ref string _Msg)
    {
        EntityAML_HeadOffice rtn = new EntityAML_HeadOffice();

        //建立HASHTABLE
        Hashtable JC66OBj = new Hashtable();
        JC66OBj.Add("FUNCTION_CODE", "I");
        if (!(this.lbAML_HQ_Work_HCOP_HEADQUATERS_CORP_NO.Text.Trim().Substring(0,1).All(char.IsDigit)) )
        {
            //20220107_Ares_Jack_自然人
            JC66OBj.Add("CORP_NO", lbAML_HQ_Work_HCOP_HEADQUATERS_CORP_NO.Text.Trim().Substring(0, 8));
            JC66OBj.Add("CORP_SEQ", lbAML_HQ_Work_HCOP_HEADQUATERS_CORP_NO.Text.Trim().Substring(8, 2));
        }
        else
        {
            JC66OBj.Add("CORP_NO", lbAML_HQ_Work_HCOP_HEADQUATERS_CORP_NO.Text.Trim());
            JC66OBj.Add("CORP_SEQ", "0000");
        }

        //上送主機資料
        Hashtable hstExmsP4A;
        hstExmsP4A = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC66, JC66OBj, false, "11", eAgentInfo);

        //20200413-RQ-2019-030155-005-增加主機回傳訊息
        try
        {
            //檢核回傳欄位MESSAGE_TYPE 0000表示以存在總公司
            string rMsg = hstExmsP4A["MESSAGE_TYPE"].ToString();
            switch (rMsg)
            {
                case "0000":
                    bResult = true;
                    _HCOP_STATUS = hstExmsP4A["STATUS"].ToString().Trim();
                    _HCOP_QUALIFY_FLAG = hstExmsP4A["QUALIFY_FLAG"].ToString().Trim();
                    break;
                default:
                    bResult = false;
                    if (hstExmsP4A.ContainsKey("HtgMsg"))
                    {
                        _Msg = hstExmsP4A["HtgMsg"].ToString();
                    }
                    else
                    {
                        _Msg = @"『" + MessageHelper.GetMessage("01_01080114_011") + "』";
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            if (!string.IsNullOrEmpty(hstExmsP4A["HtgMsg"].ToString()))//20200410-RQ-2019-030155-005
            {
                if (hstExmsP4A["HtgMsg"].ToString().Contains("704"))
                {
                    _Msg = MessageHelper.GetMessages("00_00000000_040");
                }
                else if (hstExmsP4A["HtgMsg"].ToString().Contains("705"))
                {
                    _Msg = MessageHelper.GetMessages("00_00000000_039");
                }
                else if (hstExmsP4A["HtgMsg"].ToString().Contains("799"))
                {
                    _Msg = MessageHelper.GetMessages("00_00000000_041");
                }
                else
                {
                    _Msg = MessageHelper.GetMessages("01_01080103_020");
                }
            }
        }
        finally
        {
        }
    }
    #endregion
}
