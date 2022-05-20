//******************************************************************
//* 作    者：
//* 功能說明：
//* 創建日期：
//* 修改紀錄：
//* <author>            <time>            <TaskID>                <desc>
//* Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
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
using CSIPCommonModel.BusinessRules;
using Framework.Common.Message;
using System.Text;
using Framework.Common.Logging;
using Framework.Common.IO;
using Framework.Common.Utility;

/// <summary>
/// BatchJob_PostActive 的摘要描述
/// </summary>
public class BatchJob_PostActive : Quartz.IJob
{
    protected string FunctionKey = UtilHelper.GetAppSettings("FunctionKey").ToString();

    protected JobHelper JobHelper = new JobHelper();

    public void Execute(JobExecutionContext context)
    {
        string jobID = context.JobDetail.JobDataMap["jobid"].ToString();
        JobHelper.strJobID = jobID;
        DateTime dateTime = DateTime.Now;
        int total = 0;
        int success = 0;
        string fileName = "";
        bool isContinue = true;
        string msgID = string.Empty;
        PostOfficeService postOfficeService = new PostOfficeService(jobID);
        int count = 0;
        bool hasFile = false;
        bool isFileExist = false;

        try
        {
            bool isCatchDate = false;

            JobHelper.Write(jobID, "*********** " + jobID + " 郵局主動終止 批次 START ************** ", LogState.Info);

            // 判斷Job工作狀態(0:停止 1:運行)
            isContinue = CheckJobIsContinue(jobID, dateTime, ref msgID);

            if (isContinue)
            {
                // 取郵局主動終止日期
                isCatchDate = postOfficeService.IsCatchDate(dateTime.ToString("dd"));

                fileName = "POST0004.ZIP";

                // 判斷檔案是否已存在
                isFileExist = postOfficeService.IsFileExist(fileName);

                if (isCatchDate && !isFileExist)
                {
                    count = OperatorActiveData(jobID, postOfficeService, fileName, out hasFile);

                    if (hasFile)
                    {
                        // 發送 Email
                        postOfficeService.SendMail(jobID, "郵局主動終止:" + fileName + " 總筆數:" + count, "", "成功", dateTime);

                        JobHelper.Write(jobID, "[SUCCESS] 郵局核印回覆 批次 END " + fileName, LogState.Info);
                    }
                    else
                    {
                        // 無回覆檔
                        JobHelper.Write(jobID, "[SUCCESS] 郵局主動終止 批次 END 沒有檔案", LogState.Info);
                    }
                }
                else
                {
                    if (!isCatchDate)
                    {
                        JobHelper.Write(jobID, "[SUCCESS] 郵局主動終止 批次 END 非抓檔日期", LogState.Info);
                    }

                    if (isFileExist)
                    {
                        JobHelper.Write(jobID, "[SUCCESS] 郵局主動終止 批次 END 檔案已存在", LogState.Info);
                    }
                }
            }

            // 寫入 BatchLog
            InsertBatchLog(jobID, dateTime, total, success, "S", "");
        }
        catch (Exception ex)
        {
            // 寫入 BatchLog
            InsertBatchLog(jobID, dateTime, total, success, "F", "發生錯誤：" + ex.Message);

            // 發送 Email
            postOfficeService.SendMail(jobID, "郵局主動終止:" + fileName + " 失敗！", ex.Message, "失敗", dateTime);

            JobHelper.Write(jobID, "[FAIL] 郵局主動終止 批次 " + fileName + " 發生錯誤：" + ex.Message);
        }
        finally
        {
            JobHelper.Write(jobID, " 郵局主動終止 批次 Job 結束！", LogState.Info);
            JobHelper.Write(jobID, "================================================================================================================================================", LogState.Info);
        }
    }


    public bool ExecuteManual(string jobID, string endTime)
    {
        DateTime dateTime = Convert.ToDateTime(endTime.Substring(0, 4) + "-" + endTime.Substring(4, 2) + "-" + endTime.Substring(6, 2));
        string fileName = @"CTCBAuth" + endTime;
        PostOfficeService postOfficeService = new PostOfficeService(jobID);
        int total = 0;
        int success = 0;
        bool result = false;
        bool isCatchDate = false;

        try
        {
            JobHelper.Write(jobID, "*********** " + jobID + " 郵局主動終止 手動 START ************** ", LogState.Info);

            // 取郵局主動終止日期
            isCatchDate = postOfficeService.IsCatchDate(dateTime.ToString("dd"));

            if (isCatchDate)
            {
                // 寫入 BatchLog
                InsertBatchLog(jobID, dateTime, total, success, "S", "");
            }
            else
            {
                JobHelper.Write(jobID, "[SUCCESS] 郵局主動終止 手動 END 非抓檔日期", LogState.Info);
            }

        }
        catch (Exception ex)
        {
            // 發送 Email
            postOfficeService.SendMail(jobID, "郵局主動終止:" + fileName + " 失敗！", ex.Message, "失敗", dateTime);

            JobHelper.Write(jobID, "[FAIL] 郵局主動終止 手動 " + fileName + " 發生錯誤：" + ex.Message);
        }
        finally
        {
            JobHelper.Write(jobID, " 郵局主動終止 手動 Job 結束！", LogState.Info);
            JobHelper.Write(jobID, "================================================================================================================================================", LogState.Info);
        }

        return result;
    }

    // 判斷Job工作狀態(0:停止 1:運行)
    private bool CheckJobIsContinue(string jobID, DateTime startTime, ref string msgID)
    {
        bool result = true;
        if (JobHelper.SerchJobStatus(jobID).Equals("") || JobHelper.SerchJobStatus(jobID).Equals("0"))
        {
            // Job停止
            JobHelper.Write(jobID, "[FAIL] Job工作狀態為：停止！");
            result = false;
        }

        // 檢測Job是否在執行中
        try
        {
            DataTable dtInfo = BRL_BATCH_LOG.GetRunningDate(FunctionKey, jobID, "R", ref msgID);
            if (dtInfo == null || dtInfo.Rows.Count > 0) //20210531_Ares_Stanley-修正Job執行檢核條件
            {
                JobHelper.SaveLog("JOB 工作狀態為：正在執行！", LogState.Info);
                // 返回不執行
                result = false;
            }
            else
            {
                // 記錄Job執行資訊
                BRL_BATCH_LOG.InsertRunning(FunctionKey, jobID, startTime, "R", "");
            }
        }
        catch (Exception ex)
        {
            result = false;
            JobHelper.Write(jobID, "[FAIL] " + ex.ToString());
        }

        return result;
    }

    private int OperatorActiveData(string jobID, PostOfficeService postOfficeService, string fileName, out bool hasFile)
    {
        string unZipPwd = "";
        bool isUnZip = false;
        DataTable activeData = new DataTable();
        string unZipFileName = fileName.Split('.')[0];
        int count = 0;
        hasFile = false;
        // 下載檔案
        string path = postOfficeService.DownloadFile(jobID, fileName, out unZipPwd);

        if (path != "")
        {
            hasFile = true;

            // 解壓縮
            isUnZip = postOfficeService.DecompressFile(jobID, path, fileName, unZipPwd);

            if (isUnZip)
            {
                JobHelper.Write(jobID, fileName + " 解壓縮成功", LogState.Info);

                // 組終止檔資料
                activeData = postOfficeService.GetActiveFileStream(path + unZipFileName + ".txt");

                // 寫入每天晚上批次 TABLE
                count = postOfficeService.UpdateActiveRelatedTable(fileName, activeData);
            }
        }
        else
        {
            hasFile = false;
        }

        return count;
    }

    private void InsertBatchLog(string jobID, DateTime dateTime, int total, int success, string status, string message)
    {
        int fail = total - success;

        StringBuilder sbMessage = new StringBuilder();
        sbMessage.Append("總筆數：" + total.ToString() + "。");//*總筆數
        sbMessage.Append("成功筆數：" + success.ToString() + "。");//*成功筆數
        sbMessage.Append("失敗筆數：" + fail.ToString() + "。");//*失敗筆數

        if (message.Trim() != "")
        {
            sbMessage.Append("失敗訊息：" + message);//*失敗訊息
        }

        BRL_BATCH_LOG.Delete("01", jobID, "R");
        BRL_BATCH_LOG.Insert("01", jobID, dateTime, status, sbMessage.ToString());
    }
}
