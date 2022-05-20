//******************************************************************
//* 作    者：
//* 功能說明：
//* 創建日期：
//* 修改紀錄：
//* <author>            <time>            <TaskID>                <desc>
//* Ares Stanley        2021/04/16                                 增加批次執行LOG
//*******************************************************************

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Quartz;
using Quartz.Impl;
using Framework.Common.Logging;
using Framework.Data.OM.Collections;
using CSIPKeyInGUI.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using System.Text;
using CSIPCommonModel.BusinessRules;
using System.Collections;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Message;

/// <summary>
/// BatchJob_LGAT 的摘要描述
/// </summary>
class BatchJob_LGAT : Quartz.IJob
{
    private string strSessionId = "";
    //add by linhuanhuang 自動 JOB MAIL 發送機制 20100623 start
    private string strMail;
    private string strTitle;
    private string strJobID;
    DateTime dateStart;
    //add by linhuanhuang 自動 JOB MAIL 發送機制 20100623 end
    protected JobHelper JobHelper = new JobHelper();

    #region IJob 成員

    public void Execute(JobExecutionContext context)
    {
        int intTotal = 0;//*記錄批次總筆數
        int intSuccess = 0;//*記錄批次成功筆數 
        int intNonce = 0;//*當前執行的筆數
        dateStart = DateTime.Now;//*批次開始執行時間
        string strMsgID = "";
        string JobID = context.JobDetail.JobDataMap["jobid"].ToString();
        JobHelper.strJobID = JobID;

        try
        {
            JobHelper.Write(JobID, "*********** " + JobID + " Award 點數回饋參數設定 批次 START ************** ", LogState.Info);
            //add by linhuanhuang 自動 JOB MAIL 發送機制 20100623 start
            JobDataMap jobDataMap = context.JobDetail.JobDataMap;
            strMail = jobDataMap.GetString("mail").Trim();
            strTitle = jobDataMap.GetString("title").Trim();
            strJobID = jobDataMap.GetString("jobid").Trim();
            //add by linhuanhuang 自動 JOB MAIL 發送機制 20100623 end

            //*查詢資料檔L_BATCH_LOG，查看是否上次作業還未停止
            DataTable dtInfo = BRL_BATCH_LOG.GetRunningDate("01", strJobID, "R", ref strMsgID);

            if (dtInfo == null)
            {

                return;
            }
            if (dtInfo.Rows.Count > 0)
            {
                return;
            }

            //*開始批次作業
            if (!InsertNewBatch())
            {
                return;
            }

            EntitySet<EntityAwardSet3270> esAS3270 = new EntitySet<EntityAwardSet3270>();
            esAS3270 = BRAwardSet3270.GetSendData();
            intTotal = esAS3270.Count;

            if (intTotal > 0)
            {
                JobHelper.Write(JobID, "[INFO]  Award 點數回饋參數設定 批次 資料上傳，開始！", LogState.Info);
                for (int i = 0; i < esAS3270.Count; i++)
                {
                    System.Threading.Thread.Sleep(3000);
                    intNonce++;

                    EntityAwardSet3270 eAS3270 = new EntityAwardSet3270();
                    eAS3270 = esAS3270.GetEntity(i);

                    Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4L_LGAT, GetUploadHtgInfo(eAS3270, strSessionId, GetAgentInfo(context)), false, "100", GetAgentInfo(context), strJobID);

                    if (!htResult.Contains("HtgMsg"))
                    {
                        //*獲得主機sessionId
                        strSessionId = htResult["sessionId"].ToString();
                        BRAwardSet3270.UpdateSendData(eAS3270.ID, htResult["SEQ_NO"].ToString(), htResult["MSG_TYPE"].ToString(), "Y");
                        intSuccess++;
                    }
                    else
                    {
                        if (htResult["HtgMsgFlag"].ToString() == "0")
                        {
                            BRAwardSet3270.UpdateSendData(eAS3270.ID, htResult["SEQ_NO"].ToString(), htResult["MSG_TYPE"].ToString(), "F");
                            continue;
                        }
                        else
                        {
                            InsertBatchLog(intTotal, intSuccess, intNonce, htResult["HtgMsg"].ToString(), dateStart);
                            JobHelper.Write(JobID, "[FAIL]  Award 點數回饋參數設定 批次 電文發生錯誤：" + htResult["HtgMsg"].ToString());
                            return;
                        }
                    }
                }
            }
            else
            {
                JobHelper.Write(JobID, "[INFO]  Award 點數回饋參數設定 批次 資料為：" + intTotal +" 筆，無須執行！", LogState.Info);
            }

            //*批次完成記錄LOG信息
            InsertBatchLog(intTotal, intSuccess, 0, "", dateStart);
            JobHelper.Write(JobID, "[INFO]  Award 點數回饋參數設定 批次 資料上傳，結束！ 總筆數：" + intTotal + " 筆 成功筆數：" + intSuccess + " 筆 失敗筆數：" + (intTotal - intSuccess) + " 筆", LogState.Info);
        }
        catch (Exception ex)
        {
            //*批次完成記錄LOG信息
            InsertBatchLog(intTotal, intSuccess, intNonce, ex.ToString(), dateStart);
            //Logging.Log(ex, strJobID);
            JobHelper.Write(JobID, "[FAIL]  Award 點數回饋參數設定 批次 發生錯誤：" + ex.Message);
        }
        finally
        {
            UpdateBatch();
            MainFrameInfo.ClearHtgSessionJob(ref strSessionId, strJobID);
            JobHelper.Write(JobID, "[INFO]  Award 點數回饋參數設定 批次 資料上傳 總筆數：" + intTotal + " 筆 成功筆數：" + intSuccess + " 筆 失敗筆數：" + (intTotal - intSuccess) + " 筆", LogState.Info);
            JobHelper.Write(JobID, "  Award 點數回饋參數設定 批次 Job 結束！", LogState.Info);
            JobHelper.Write(JobID, "================================================================================================================================================", LogState.Info);
        }
    }

    #endregion

    private Hashtable GetUploadHtgInfo(EntityAwardSet3270 eAS3270, string strSessionId, EntityAGENT_INFO eAI)
    {
        Hashtable htInput = new Hashtable();

        htInput.Add("TRAN_ID", "LGAT");
        htInput.Add("PROGRAM_ID", "CLGU109");
        htInput.Add("FUNCTION_CODE", eAS3270.FUNCTION_CODE.Trim());
        htInput.Add("MSG_SEQ", "");
        htInput.Add("MSG_TYPE", "");
        htInput.Add("ORG", eAS3270.ORG.Trim());
        htInput.Add("PROG_ID", eAS3270.PROG_ID.Trim());
        htInput.Add("PARTNER_ID", eAS3270.PARTNER_ID.Trim());
        htInput.Add("TYPE", eAS3270.ACCUMULATION_TYPE.Trim());
        htInput.Add("STATUS", "0");
        htInput.Add("COUNTRY_CODE_IND", eAS3270.COUNTRY_CODE_IND.Trim());
        htInput.Add("CYCLE_DUE", "9");
        htInput.Add("CARD_IDENT_POS", "00");
        htInput.Add("CARD_IDENT_VAL", "0");
        htInput.Add("CARD_IDENT_IND", "N");
        htInput.Add("BASIC_CALC_MTHD", eAS3270.BASIC_CALC_MTHD.Trim());
        htInput.Add("BASIC_CALC_IND", eAS3270.BASIC_CALC_IND.Trim());
        htInput.Add("SUPP_BASIC_CALC_MTHD", eAS3270.SUPP_BASIC_CAL_MTHD.Trim());
        htInput.Add("SUPP_BASIC_CALC_IND", eAS3270.SUPP_BASIC_CAL_IND.Trim());
        htInput.Add("PROMO_START_DTE", eAS3270.PROMO_START_DTE.Trim());
        htInput.Add("PROMO_END_DTE", eAS3270.PROMO_END_DTE.Trim());
        htInput.Add("PROMO_CALC_MTHD", eAS3270.PROMO_CALC_MTHD.Trim());
        htInput.Add("PROMO_CALC_IND", eAS3270.PROMO_CALC_IND.Trim());
        htInput.Add("SUPPR_FROM_DTE", "0");
        htInput.Add("SUPPR_TO_DTE", "0");
        htInput.Add("BTHDTE_FLAG", eAS3270.BTHDTE_FLAG.Trim());
        htInput.Add("BTHDTE_START", eAS3270.BTHDTE_START.Trim());
        htInput.Add("BTHDTE_END", eAS3270.BTHDTE_END.Trim());
        htInput.Add("BIRTH_CALC_MTHD", eAS3270.BTHDTE_CALC_MTHD.Trim());
        htInput.Add("BTHDTE_CALC_IND", eAS3270.BTHDTE_CALC_IND.Trim());
        htInput.Add("JOIDTE_FLAG", "");
        htInput.Add("JOIDTE_MONTH", "00");
        htInput.Add("JOIDTE_CALC_MTHD", "1");
        htInput.Add("JOIDTE_CALC_IND", "0");
        htInput.Add("M_SIGNON_NAME", eAI.agent_id.Trim());
        htInput.Add("M_TERM_ID", "CSIP");
        htInput.Add("TC_CODE1", BlankProcess(eAS3270.TC_CODE1.Trim(), "0", 2));
        htInput.Add("TC_OP1", "+");
        htInput.Add("TC_CODE2", BlankProcess(eAS3270.TC_CODE2.Trim(), "0", 2));
        htInput.Add("TC_OP2", "-");
        htInput.Add("TC_CODE3", "00");
        htInput.Add("TC_OP3", "+");
        htInput.Add("TC_CODE4", "00");
        htInput.Add("TC_OP4", "-");
        htInput.Add("TC_CODE5", "00");
        htInput.Add("TC_OP5", "+");
        htInput.Add("TC_CODE6", "00");
        htInput.Add("TC_OP6", "-");
        htInput.Add("TC_CODE7", "00");
        htInput.Add("TC_OP7", "+");
        htInput.Add("TC_CODE8", "00");
        htInput.Add("TC_OP8", "-");
        htInput.Add("TC_CODE9", "00");
        htInput.Add("TC_OP9", "+");
        htInput.Add("TC_CODE10", "00");
        htInput.Add("TC_OP10", "-");
        htInput.Add("MCC_FROM1", BlankProcess(eAS3270.MCC_FROM1.Trim(), "0", 4));
        htInput.Add("MCC_TO1", BlankProcess(eAS3270.MCC_TO1.Trim(), "0", 4));
        htInput.Add("COUNTRY_CODE1", BlankProcess(eAS3270.COUNTRY_CODE1.Trim(), "", 0));
        htInput.Add("MCC_FROM2", BlankProcess(eAS3270.MCC_FROM2.Trim(), "0", 4));
        htInput.Add("MCC_TO2", BlankProcess(eAS3270.MCC_TO2.Trim(), "0", 4));
        htInput.Add("COUNTRY_CODE2", BlankProcess(eAS3270.COUNTRY_CODE2.Trim(), "", 0));
        htInput.Add("MCC_FROM3", BlankProcess(eAS3270.MCC_FROM3.Trim(), "0", 4));
        htInput.Add("MCC_TO3", BlankProcess(eAS3270.MCC_TO3.Trim(), "0", 4));
        htInput.Add("COUNTRY_CODE3", BlankProcess(eAS3270.COUNTRY_CODE3.Trim(), "", 0));
        htInput.Add("MCC_FROM4", BlankProcess(eAS3270.MCC_FROM4.Trim(), "0", 4));
        htInput.Add("MCC_TO4", BlankProcess(eAS3270.MCC_TO4.Trim(), "0", 4));
        htInput.Add("COUNTRY_CODE4", BlankProcess(eAS3270.COUNTRY_CODE4.Trim(), "", 0));
        htInput.Add("MCC_FROM5", BlankProcess(eAS3270.MCC_FROM5.Trim(), "0", 4));
        htInput.Add("MCC_TO5", BlankProcess(eAS3270.MCC_TO5.Trim(), "0", 4));
        htInput.Add("COUNTRY_CODE5", BlankProcess(eAS3270.COUNTRY_CODE5.Trim(), "", 0));
        htInput.Add("MCC_FROM6", BlankProcess(eAS3270.MCC_FROM6.Trim(), "0", 4));
        htInput.Add("MCC_TO6", BlankProcess(eAS3270.MCC_TO6.Trim(), "0", 4));
        htInput.Add("COUNTRY_CODE6", BlankProcess(eAS3270.COUNTRY_CODE6.Trim(), "", 0));
        htInput.Add("MCC_FROM7", BlankProcess(eAS3270.MCC_FROM7.Trim(), "0", 4));
        htInput.Add("MCC_TO7", BlankProcess(eAS3270.MCC_TO7.Trim(), "0", 4));
        htInput.Add("COUNTRY_CODE7", BlankProcess(eAS3270.COUNTRY_CODE7.Trim(), "", 0));
        htInput.Add("MCC_FROM8", BlankProcess(eAS3270.MCC_FROM8.Trim(), "0", 4));
        htInput.Add("MCC_TO8", BlankProcess(eAS3270.MCC_TO8.Trim(), "0", 4));
        htInput.Add("COUNTRY_CODE8", BlankProcess(eAS3270.COUNTRY_CODE8.Trim(), "", 0));
        htInput.Add("MCC_FROM9", BlankProcess(eAS3270.MCC_FROM9.Trim(), "0", 4));
        htInput.Add("MCC_TO9", BlankProcess(eAS3270.MCC_TO9.Trim(), "0", 4));
        htInput.Add("COUNTRY_CODE9", BlankProcess(eAS3270.COUNTRY_CODE9.Trim(), "", 0));
        htInput.Add("MCC_FROM10", BlankProcess(eAS3270.MCC_FROM10.Trim(), "0", 4));
        htInput.Add("MCC_TO10", BlankProcess(eAS3270.MCC_TO10.Trim(), "0", 4));
        htInput.Add("COUNTRY_CODE10", BlankProcess(eAS3270.COUNTRY_CODE10.Trim(), "", 0));
        htInput.Add("BLOCK_CODE1", "0");
        htInput.Add("BLOCK_CODE2", "0");
        htInput.Add("BLOCK_CODE3", "0");
        htInput.Add("BLOCK_CODE4", "0");
        htInput.Add("BLOCK_CODE5", "0");
        htInput.Add("BLOCK_CODE6", "0");
        htInput.Add("BLOCK_CODE7", "0");
        htInput.Add("BLOCK_CODE8", "0");
        htInput.Add("BLOCK_CODE9", "0");
        htInput.Add("BLOCK_CODE10", "0");
        htInput.Add("BLOCK_CODE11", "0");
        htInput.Add("BLOCK_CODE12", "0");
        htInput.Add("BLOCK_CODE13", "0");
        htInput.Add("BLOCK_CODE14", "0");
        htInput.Add("BLOCK_CODE15", "0");
        htInput.Add("BLOCK_CODE16", "0");
        htInput.Add("BLOCK_CODE17", "0");
        htInput.Add("BLOCK_CODE18", "0");
        htInput.Add("BLOCK_CODE19", "0");
        htInput.Add("BLOCK_CODE20", "0");
        htInput.Add("BLOCK_CODE21", "0");
        htInput.Add("BLOCK_CODE22", "0");
        htInput.Add("BLOCK_CODE23", "0");
        htInput.Add("BLOCK_CODE24", "0");
        htInput.Add("BLOCK_CODE25", "0");
        htInput.Add("BLOCK_CODE26", "0");
        htInput.Add("BASIC_TIER_AMT1", BlankProcess(eAS3270.BASIC_TIER_AMT1.Trim(), "0", 9));
        htInput.Add("BASIC_TIER_RATE1", BlankProcess(eAS3270.BASIC_TIER_RATE1.Trim(), "0", 4));
        htInput.Add("SUPP_BASIC_TIER_AMT1", BlankProcess(eAS3270.SUPP_BASIC_TIER_AMT1.Trim(), "0", 9));
        htInput.Add("SUPP_BASIC_TIER_RATE1", BlankProcess(eAS3270.SUPP_BASIC_TIER_RATE1.Trim(), "0", 4));
        htInput.Add("PROMO_TIER_AMT1", BlankProcess(eAS3270.PROMO_TIER_AMT1.Trim(), "0", 9));
        htInput.Add("PROMO_TIER_RATE1", BlankProcess(eAS3270.PROMO_TIER_RATE1.Trim(), "0", 4));
        htInput.Add("BTHDTE_TIER_AMT1", BlankProcess(eAS3270.BTHDTE_TIER_AMT1.Trim(), "0", 9));
        htInput.Add("BTHDTE_TIER_RATE1", BlankProcess(eAS3270.BTHDTE_TIER_RATE1.Trim(), "0", 4));
        htInput.Add("JOIDTE_TIER_AMT1", "0");
        htInput.Add("JOIDTE_TIER_RATE1", "0");
        htInput.Add("BASIC_TIER_AMT2", BlankProcess(eAS3270.BASIC_TIER_AMT2.Trim(), "0", 9));
        htInput.Add("BASIC_TIER_RATE2", BlankProcess(eAS3270.BASIC_TIER_RATE2.Trim(), "0", 4));
        htInput.Add("SUPP_BASIC_TIER_AMT2", BlankProcess(eAS3270.SUPP_BASIC_TIER_AMT2.Trim(), "0", 9));
        htInput.Add("SUPP_BASIC_TIER_RATE2", BlankProcess(eAS3270.SUPP_BASIC_TIER_RATE2.Trim(), "0", 4));
        htInput.Add("PROMO_TIER_AMT2", BlankProcess(eAS3270.PROMO_TIER_AMT2.Trim(), "0", 9));
        htInput.Add("PROMO_TIER_RATE2", BlankProcess(eAS3270.PROMO_TIER_RATE2.Trim(), "0", 4));
        htInput.Add("BTHDTE_TIER_AMT2", BlankProcess(eAS3270.BTHDTE_TIER_AMT2.Trim(), "0", 9));
        htInput.Add("BTHDTE_TIER_RATE2", BlankProcess(eAS3270.BTHDTE_TIER_RATE2.Trim(), "0", 4));
        htInput.Add("JOIDTE_TIER_AMT2", "0");
        htInput.Add("JOIDTE_TIER_RATE2", "0");
        htInput.Add("BASIC_TIER_AMT3", BlankProcess(eAS3270.BASIC_TIER_AMT3.Trim(), "0", 9));
        htInput.Add("BASIC_TIER_RATE3", BlankProcess(eAS3270.BASIC_TIER_RATE3.Trim(), "0", 4));
        htInput.Add("SUPP_BASIC_TIER_AMT3", BlankProcess(eAS3270.SUPP_BASIC_TIER_AMT3.Trim(), "0", 9));
        htInput.Add("SUPP_BASIC_TIER_RATE3", BlankProcess(eAS3270.SUPP_BASIC_TIER_RATE3.Trim(), "0", 4));
        htInput.Add("PROMO_TIER_AMT3", BlankProcess(eAS3270.PROMO_TIER_AMT3.Trim(), "0", 9));
        htInput.Add("PROMO_TIER_RATE3", BlankProcess(eAS3270.PROMO_TIER_RATE3.Trim(), "0", 4));
        htInput.Add("BTHDTE_TIER_AMT3", BlankProcess(eAS3270.BTHDTE_TIER_AMT3.Trim(), "0", 9));
        htInput.Add("BTHDTE_TIER_RATE3", BlankProcess(eAS3270.BTHDTE_TIER_RATE3.Trim(), "0", 4));
        htInput.Add("JOIDTE_TIER_AMT3", "0");
        htInput.Add("JOIDTE_TIER_RATE3", "0");
        htInput.Add("BASIC_TIER_AMT4", BlankProcess(eAS3270.BASIC_TIER_AMT4.Trim(), "0", 9));
        htInput.Add("BASIC_TIER_RATE4", BlankProcess(eAS3270.BASIC_TIER_RATE4.Trim(), "0", 4));
        htInput.Add("SUPP_BASIC_TIER_AMT4", BlankProcess(eAS3270.SUPP_BASIC_TIER_AMT4.Trim(), "0", 9));
        htInput.Add("SUPP_BASIC_TIER_RATE4", BlankProcess(eAS3270.SUPP_BASIC_TIER_RATE4.Trim(), "0", 4));
        htInput.Add("PROMO_TIER_AMT4", BlankProcess(eAS3270.PROMO_TIER_AMT4.Trim(), "0", 9));
        htInput.Add("PROMO_TIER_RATE4", BlankProcess(eAS3270.PROMO_TIER_RATE4.Trim(), "0", 4));
        htInput.Add("BTHDTE_TIER_AMT4", BlankProcess(eAS3270.BTHDTE_TIER_AMT4.Trim(), "0", 9));
        htInput.Add("BTHDTE_TIER_RATE4", BlankProcess(eAS3270.BTHDTE_TIER_RATE4.Trim(), "0", 4));
        htInput.Add("JOIDTE_TIER_AMT4", "0");
        htInput.Add("JOIDTE_TIER_RATE4", "0");

        htInput.Add("sessionId", strSessionId);
        return htInput;
    }

    private string BlankProcess(string strCheck, string strReplace, int iLength)
    {
        string strR = "";

        if ("" == strCheck.Trim())
        {
            if ("" == strReplace.Trim())
            {
                strR = "";
            }
            else
            {
                for (int i = 0; i < iLength; i++)
                {
                    strR = strR + strReplace;
                }
            }
        }
        else
        {
            strR = strCheck;
        }

        return strR;
    }

    /// <summary>
    /// 開始此次作業向Job_Status中插入一筆新的資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool InsertNewBatch()
    {
        //try
        //{
        //    //*向Job_Status中插入一筆新的資料
        //    EntityJOB_STATUS eJobStatus = new EntityJOB_STATUS();
        //    eJobStatus.job_name = "Job_LGAT";
        //    eJobStatus.job_status = "1";
        //    eJobStatus.start_time = DateTime.Now;
        //    eJobStatus.end_time = DBNull.Value;
        //    eJobStatus.data_count = iDataCount;
        //    return BRJOB_STATUS.AddEntity(eJobStatus);
        //}
        //catch
        //{
        //    return false;
        //}

        //*向L_BATCH_LOG中插入一筆新的資料
        return BRL_BATCH_LOG.InsertRunning("01", strJobID, dateStart, "R", "");
    }

    /// <summary>
    /// 異動Job_Status中批次狀態
    /// </summary>
    private void UpdateBatch()
    {
        //EntityJOB_STATUS eJobStatus = new EntityJOB_STATUS();
        //eJobStatus.end_time = DateTime.Now;
        //eJobStatus.job_status = "2";
        //string[] strField = { EntityJOB_STATUS.M_end_time, EntityJOB_STATUS.M_job_status };
        //BRJOB_STATUS.Update(eJobStatus, "Job_LGAT", "1", strField);
    }

    /// <summary>
    /// 插入L_BATCH_LOG資料庫
    /// </summary>
    /// <param name="intTotal">總筆數</param>
    /// <param name="intSuccess">成功筆數</param>
    /// <param name="intNonce">當前執行的筆數</param>
    /// <param name="strError">JOB失敗信息</param>
    /// <param name="dateStart">JOB開始時間</param>
    private void InsertBatchLog(int intTotal, int intSuccess, int intNonce, string strError, DateTime dateStart)
    {
        //string strStatus = GetStatus(intTotal, intSuccess);
        int intNo = 0;//*未完成筆數

        if (intNonce > 0)//*當intNonce > 0表示主機返回異常，intNonce為記錄當前異常的筆數
        {
            intNo = intTotal - (intNonce - 1);
        }

        int intFail = intTotal - intSuccess - intNo;//*失敗筆數

        //add by linhuanhuang 自動 JOB MAIL 發送機制 20100623 start
        string strStatus = GetStatus(intTotal, intSuccess, intFail, intNo);
        //add by linhuanhuang 自動 JOB MAIL 發送機制 20100623 end

        StringBuilder sbMessage = new StringBuilder();
        sbMessage.Append("總筆數：" + intTotal.ToString() + "。");//*總筆數
        sbMessage.Append("成功筆數：" + intSuccess.ToString() + "。");//*成功筆數
        sbMessage.Append("失敗筆數：" + intFail.ToString() + "。");//*失敗筆數
        sbMessage.Append("未完成筆數：" + intNo.ToString() + "。");//*未完成筆數
        //edit by linhuanhuang 自動 JOB MAIL 發送機制 20100623 start
        if (strError.Trim() != "")
        {
            sbMessage.Append("失敗訊息：" + strError);//*失敗訊息
        }
        //edit by linhuanhuang 自動 JOB MAIL 發送機制 20100623 end

        //*插入L_BATCH_LOG資料庫
        BRL_BATCH_LOG.Delete("01", strJobID, "R");
        BRL_BATCH_LOG.Insert("01", strJobID, dateStart, strStatus, sbMessage.ToString());

        //add by linhuanhuang 自動 JOB MAIL 發送機制 20100623 start
        if (intTotal > 0)
        {
            string[] str = strMail.Split(';');



            System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();

            nvc["Name"] = strMail.Replace(';', ',');

            nvc["Title"] = strTitle;

            nvc["StartTime"] = dateStart.ToString();

            nvc["EndTime"] = DateTime.Now.ToString();

            nvc["Message"] = sbMessage.ToString().Trim();

            nvc["Status"] = GetStatusName(strStatus);

            try
            {
                MailService.MailSender(str, 1, nvc, "");
            }
            catch (Exception ex)
            {
                Logging.Log(ex, strJobID);
            }
        }
        //add by linhuanhuang 自動 JOB MAIL 發送機制 20100623 end
    }

    //add by linhuanhuang 自動 JOB MAIL 發送機制 20100623 start
    /// <summary>
    /// JOB執行狀態
    /// </summary>
    /// <param name="strStauts">狀態英文名稱</param>

    /// <returns>JOB執行狀態</returns>
    private string GetStatusName(string strStauts)
    {
        switch (strStauts)
        {
            case "P":
                return "批次部分成功";
            case "F":
                return "批次失敗";
            case "S":
                return "批次成功";
            default:
                return "";
        }
    }

    /// <summary>
    /// JOB執行狀態
    /// </summary>
    /// <param name="intTotal">總筆數</param>
    /// <param name="intSuccess">成功筆數</param>
    /// <param name="intFail">失敗筆數</param>
    /// <param name="intNo">未完成筆數</param>
    /// <returns>JOB執行狀態</returns>
    private string GetStatus(int intTotal, int intSuccess, int intFail, int intNo)
    {
        if (intTotal == 0)
        {
            return "N";
        }
        else if (intTotal == (intSuccess + intFail))
        {
            return "S";
        }
        else if (intTotal == intNo)
        {
            return "F";
        }
        else
        {
            return "P";
        }


    }
    //add by linhuanhuang 自動 JOB MAIL 發送機制 20100623 end

    /// <summary>
    /// JOB執行狀態
    /// </summary>
    /// <param name="intTotal">總筆數</param>
    /// <param name="intSuccess">成功筆數</param>
    /// <returns>JOB執行狀態</returns>
    private string GetStatus(int intTotal, int intSuccess)
    {
        if (intSuccess > 0)
        {
            if ((intTotal - intSuccess) > 0)
            {
                return "P";
            }
            else
            {
                return "S";
            }
        }
        else
        {
            return "F";
        }
    }

    /// <summary>
    /// 得到登陸主機信息
    /// </summary>
    /// <returns>EntityAGENT_INFO</returns>
    private EntityAGENT_INFO GetAgentInfo(Quartz.JobExecutionContext context)
    {
        JobDataMap jobDataMap = context.JobDetail.JobDataMap;
        EntityAGENT_INFO eAgentInfo = new EntityAGENT_INFO();
        if (jobDataMap != null && jobDataMap.Count > 0)
        {
            eAgentInfo.agent_id = jobDataMap.GetString("userId");
            eAgentInfo.agent_pwd = jobDataMap.GetString("passWord");
            eAgentInfo.agent_id_racf = jobDataMap.GetString("racfId");
            eAgentInfo.agent_id_racf_pwd = jobDataMap.GetString("racfPassWord");
        }
        return eAgentInfo;
    }

}
