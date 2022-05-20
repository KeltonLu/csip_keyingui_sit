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
/// BatchJob_LGOR 的摘要描述
/// </summary>
class BatchJob_LGOR : Quartz.IJob
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
            JobHelper.Write(JobID, "*********** " + JobID + " Redeem 點數折抵參數設定 批次 START ************** ", LogState.Info);
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

            EntitySet<EntityRedeem3270> esR3270 = new EntitySet<EntityRedeem3270>();
            esR3270 = BRRedeem3270.GetSendData();
            intTotal = esR3270.Count;

            JobHelper.Write(JobID, "[INFO]  Redeem 點數折抵參數設定 批次 資料上傳，開始！", LogState.Info);
            if (intTotal > 0)
            {
                for (int i = 0; i < esR3270.Count; i++)
                {
                    System.Threading.Thread.Sleep(3000);
                    intNonce++;

                    EntityRedeem3270 eR3270 = new EntityRedeem3270();
                    eR3270 = esR3270.GetEntity(i);
                    
                    Hashtable htResult = MainFrameInfo.GetMainFrameInfo(HtgType.P4L_LGOR, GetUploadHtgInfo(eR3270, strSessionId), false, "100", GetAgentInfo(context), strJobID);

                    if (!htResult.Contains("HtgMsg"))
                    {
                        //*獲得主機sessionId
                        strSessionId = htResult["sessionId"].ToString();
                        BRRedeem3270.UpdateSendData(eR3270.ID, htResult["MSG_SEQ"].ToString(), htResult["MSG_ERR"].ToString(), "Y");
                        intSuccess++;
                    }
                    else
                    {
                        if (htResult["HtgMsgFlag"].ToString() == "0")
                        {
                            BRRedeem3270.UpdateSendData(eR3270.ID, htResult["MSG_SEQ"].ToString(), htResult["MSG_ERR"].ToString(), "F");
                            continue;
                        }
                        else
                        {
                            InsertBatchLog(intTotal, intSuccess, intNonce, htResult["HtgMsg"].ToString(), dateStart);
                            JobHelper.Write(JobID, "[FAIL]  Redeem 點數折抵參數設定 批次 電文發生錯誤：" + htResult["HtgMsg"].ToString());
                            return;
                        }
                    }
                }
            }
            else
            {
                JobHelper.Write(JobID, "[INFO]  Redeem 點數折抵參數設定 批次 資料為：" + intTotal + " 筆，無須執行！", LogState.Info);
            }

            //*批次完成記錄LOG信息
            InsertBatchLog(intTotal, intSuccess, 0, "", dateStart);
            JobHelper.Write(JobID, "[INFO]  Redeem 點數折抵參數設定 批次 資料上傳，結束！ 總筆數：" + intTotal + " 筆 成功筆數：" + intSuccess + " 筆 失敗筆數：" + (intTotal - intSuccess) + " 筆", LogState.Info);
        }
        catch (Exception ex)
        {
            //*批次完成記錄LOG信息
            InsertBatchLog(intTotal, intSuccess, intNonce, ex.ToString(), dateStart);
            Logging.Log(ex, strJobID);
            JobHelper.Write(JobID, "[FAIL]  Redeem 點數折抵參數設定 批次 發生錯誤：" + ex.Message);
        }
        finally
        {
            UpdateBatch();
            MainFrameInfo.ClearHtgSessionJob(ref strSessionId, strJobID);
            JobHelper.Write(JobID, "[INFO]  Redeem 點數折抵參數設定 批次 資料上傳 總筆數：" + intTotal + " 筆 成功筆數：" + intSuccess + " 筆 失敗筆數：" + (intTotal - intSuccess) + " 筆", LogState.Info);
            JobHelper.Write(JobID, "  Redeem 點數折抵參數設定 批次 Job 結束！", LogState.Info);
            JobHelper.Write(JobID, "================================================================================================================================================", LogState.Info);
        }
    }

    #endregion

    private Hashtable GetUploadHtgInfo(EntityRedeem3270 eR3270, string strSessionId)
    {
        Hashtable htInput = new Hashtable();

        htInput.Add("FUNCTION_CODE", eR3270.FUNCTION_CODE.Trim());
        htInput.Add("MSG_SEQ", "");
        htInput.Add("MSG_ERR", "");
        htInput.Add("IN_ORG", eR3270.IN_ORG.Trim());
        htInput.Add("IN_MERCHANT", eR3270.IN_MERCHANT.Trim());
        htInput.Add("IN_PROD_CODE", eR3270.IN_PROD_CODE.Trim());
        htInput.Add("IN_CARD_TYPE", eR3270.IN_CARD_TYPE.Trim());
        htInput.Add("PROGID", eR3270.PROGID.Trim());
        htInput.Add("MERRATE", eR3270.MER_RATE.Trim());
        htInput.Add("LIMITR", eR3270.LIMITR.Trim());
        htInput.Add("CHPOINT", eR3270.CHPOINT.Trim());
        htInput.Add("CHAMT", eR3270.CHAMT.Trim());
        htInput.Add("USER_EXIT", eR3270.USER_EXIT.Trim());
        htInput.Add("CYLCO", eR3270.CYLCO.Trim());
        htInput.Add("STARTU", eR3270.STARTU.Trim());
        htInput.Add("ENDU", eR3270.ENDU.Trim());
        htInput.Add("LIMITU", eR3270.LIMITU.Trim());
        htInput.Add("CHPOINTU", eR3270.CHPOINTU.Trim());
        htInput.Add("CHAMTU", eR3270.CHAMTU.Trim());
        htInput.Add("BIRTH", eR3270.BIRTH.Trim());
        htInput.Add("STARTB", eR3270.STARTB.Trim());
        htInput.Add("ENDB", eR3270.ENDB.Trim());
        htInput.Add("LIMITB", eR3270.LIMITB.Trim());
        htInput.Add("CHPOINTB", eR3270.CHPOINTB.Trim());
        htInput.Add("CHAMTB", eR3270.CHAMTB.Trim());

        htInput.Add("sessionId", strSessionId);

        return htInput;
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
        //    eJobStatus.job_name = "Job_LGOR";
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
        //BRJOB_STATUS.Update(eJobStatus, "Job_LGOR", "1", strField);
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
                MailService.MailSender(str, 1, nvc,"");
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
