using System;
using System.Data;
using Quartz;
using Framework.Common.Logging;
using CSIPCommonModel.BusinessRules;
using CSIPKeyInGUI.BusinessRules;

/// <summary>
/// jobTest 的摘要描述

/// </summary>
public class jobAutoPay : IJob
{
    public jobAutoPay()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }
    
    #region IJob 成員
    public void Execute(JobExecutionContext context)
    {
        string strFuncKey = "01";
        string strJobID = "jobAutoPay";
        string strJobTitle;
        string strLdapID;
        string strLdapPWD;
        string strRacfID;
        string strRacfPwd;
        string strMailList;
        DateTime dtStart = DateTime.Now;

        try
        {
            strJobID = context.JobDetail.JobDataMap.GetString("jobid").Trim();
            strJobTitle = context.JobDetail.JobDataMap.GetString("title").Trim();
            strLdapID = context.JobDetail.JobDataMap.GetString("userId").Trim();
            strLdapPWD = context.JobDetail.JobDataMap.GetString("passWord").Trim();
            strRacfID = context.JobDetail.JobDataMap.GetString("racfId").Trim();
            strRacfPwd = context.JobDetail.JobDataMap.GetString("racfPassWord").Trim();
            strMailList = context.JobDetail.JobDataMap.GetString("mail").Trim();
            string strMsgID = "";
            //*查詢資料檔L_BATCH_LOG，查看是否上次作業還未停止

            DataTable dtInfo = BRL_BATCH_LOG.GetRunningDate(strFuncKey, strJobID, "R", ref strMsgID);
            if (dtInfo == null)
            {
                return;
            }
            if (dtInfo.Rows.Count > 0)
            {
                return;
            }

            //*判斷是否為假日
            if (!BRWORK_DATE.IS_WORKDAY("01", DateTime.Now.ToString("yyyyMMdd")))
            {
                return;
            }

            //*開始批次作業
            if (!BRL_BATCH_LOG.InsertRunning(strFuncKey, strJobID, dtStart, "R", ""))
            {
                return;
            }

            if (BRAuto_pay_status.CopyFailureRecsFromOther_Bank_TempToAuto_Pay_Status())
            {
                DataSet ds=BRAuto_pay_status.GetDataFromtblAuto_Pay_Status();
                //if (ds != null && ds.Tables[0].Rows.Count > 0)
                //{
                    BRAuto_pay_status.BatchOutput(ds.Tables[0]);
                    BRAuto_pay_status.UploadToFTP();
                //}
            }

            JobDataMap jobDataMap = context.JobDetail.JobDataMap;
            string strMsg = strJobID + "執行於:" + DateTime.Parse(context.FireTimeUtc.ToString()).AddHours(8).ToString();
            if (context.NextFireTimeUtc.HasValue)
                strMsg += "  ;下次執行於:" + DateTime.Parse(context.NextFireTimeUtc.ToString()).AddHours(8).ToString();
            Logging.Log(strMsg, strJobID, LogLayer.DB);

            BRL_BATCH_LOG.Delete(strFuncKey, strJobID, "R");
            BRL_BATCH_LOG.Insert(strFuncKey, strJobID, dtStart, "S", "");

        }
        catch (Exception ex)
        {
            BRL_BATCH_LOG.Delete(strFuncKey, strJobID, "R");
            BRL_BATCH_LOG.Insert(strFuncKey, strJobID, dtStart, "F", ex.Message);
            Logging.Log(ex, strJobID, LogLayer.DB);
        }
    }
    #endregion
}
