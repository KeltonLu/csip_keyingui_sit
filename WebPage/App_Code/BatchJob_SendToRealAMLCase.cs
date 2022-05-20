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
using Quartz;
using CSIPCommonModel.BusinessRules;
using System.Text;
using Framework.Common.Logging;
using Framework.Common.Utility;

/// <summary>
/// BatchJob_SendToAMLCase 的摘要描述
/// </summary>
public class BatchJob_SendToRealAMLCase : Quartz.IJob
{
    protected string FunctionKey = UtilHelper.GetAppSettings("FunctionKey").ToString();
    protected DateTime StartTime = DateTime.Now;// 記錄Job啟動時間
    protected DateTime EndTime;
    protected JobHelper JobHelper = new JobHelper();

    public void Execute(JobExecutionContext context)
    {
        int total = 0;
        int success = 0;
        string jobID = context.JobDetail.JobDataMap["jobid"].ToString();
        JobHelper.strJobID = jobID;
        string fileName = "";
        string fileNamectl = "";
        bool isContinue = true;
        string msgID = string.Empty;
        PostRealAMLCaseService postRealAMLCaseService = new PostRealAMLCaseService(jobID);

        try
        {

            DataTable postData = new DataTable();
            DateTime dateTime = new DateTime();
            string startTime = "";
            string endTime = "";
            bool isComplete = false;

            JobHelper.Write(jobID, "*********** " + jobID + " 傳給AML結案案件 批次 START ************** ", LogState.Info);


            bool adjustDayToExcute = false;

            if (dateTime.DayOfWeek == DayOfWeek.Monday)
            {
                adjustDayToExcute = true;
            }
            else if (dateTime.DayOfWeek == DayOfWeek.Tuesday)
            {
                adjustDayToExcute = true;
            }
            else if (dateTime.DayOfWeek == DayOfWeek.Wednesday)
            {
                adjustDayToExcute = true;
            }
            else if (dateTime.DayOfWeek == DayOfWeek.Thursday)
            {
                adjustDayToExcute = true;
            }
            else if (dateTime.DayOfWeek == DayOfWeek.Friday)
            {
                adjustDayToExcute = true;
            }
            else if (dateTime.DayOfWeek == DayOfWeek.Saturday)
            {
                adjustDayToExcute = true;
            }
            else if (dateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                adjustDayToExcute = true;
            }


            if (adjustDayToExcute)
            {

                // 判斷Job工作狀態(0:停止 1:運行)
                isContinue = CheckJobIsContinue(jobID, ref msgID);

                if (isContinue)
                {
                    // 算起迄時間
                    postRealAMLCaseService.GetSearchTime(this.StartTime, out startTime, out endTime);

                    dateTime = Convert.ToDateTime(endTime.Substring(0, 4) + "-" + endTime.Substring(4, 2) + "-" + endTime.Substring(6, 2));
                    fileName = @"AML_TW_RMMOK_CARD_" + endTime + ".dat";
                    fileNamectl = @"AML_TW_RMMOK_CARD_" + endTime + ".ctl";
                    // 取結過案的資料
                    postData = postRealAMLCaseService.GetSendToPostOfficeData(endTime);

                    // 產檔及上傳到 MFTP
                    isComplete = GenaratorFileAndUploadToMFTP(jobID, fileName, postData, postRealAMLCaseService, dateTime);

                    // 產檔及上傳到 MFTP
                    isComplete = GenaratorCtlFileAndUploadToMFTP(jobID, fileNamectl, postData, postRealAMLCaseService, dateTime);


                    success = postData.Rows.Count;
                    if (isComplete)
                    {
                        total = postData.Rows.Count;

                        // 寫入 BatchLog
                        InsertBatchLog(jobID, total, success, "S", "");

                        // 發送 Email
                        postRealAMLCaseService.SendMail(jobID, "傳給主機結案案件 批次:" + fileName + " 上傳成功！", "總筆數:" + total, "上傳成功", this.StartTime);
                    }
                    else
                    {
                        // 寫入 BatchLog
                        InsertBatchLog(jobID, total, success, "S", "");

                        // 發送 Email
                        postRealAMLCaseService.SendMail(jobID, "傳給主機結案案件 批次:" + fileName + " 成功！", "無資料", "成功", this.StartTime);
                    }

                    JobHelper.Write(jobID, "[SUCCESS] 傳給主機結案案件 批次 END " + fileName, LogState.Info);
                }
            }
            else
            {
                JobHelper.Write(jobID, "[SUCCESS] 今天是" + dateTime.DayOfWeek + "無須產檔", LogState.Info);
            }
        }
        catch (Exception ex)
        {
            // 寫入 BatchLog
            InsertBatchLog(jobID, total, success, "F", "發生錯誤：" + ex.Message);

            // 發送 Email
            postRealAMLCaseService.SendMail(jobID, "傳給主機結案案件 批次:" + fileName + " 上傳失敗！", ex.Message, "上傳失敗", this.StartTime);

            JobHelper.Write(jobID, "[FAIL] 傳給主機結案案件 批次 " + fileName + " 發生錯誤：" + ex.Message);
        }
        finally
        {
            JobHelper.Write(jobID, " 傳給主機結案案件 批次 Job 結束！", LogState.Info);
            JobHelper.Write(jobID, "================================================================================================================================================", LogState.Info);
        }
    }

    /// <summary>
    /// 手動執行
    /// </summary>
    /// <param name="jobID"></param>
    /// <param name="endTime"></param>
    /// <returns></returns>
    public bool ExecuteManual(string jobID, string endTime)
    {
        DateTime dateTime = Convert.ToDateTime(endTime.Substring(0, 4) + "-" + endTime.Substring(4, 2) + "-" + endTime.Substring(6, 2));
        string fileName = @"AML_UPD_CLO_R_" + endTime + ".DAT";
        PostRealAMLCaseService postRealAMLCaseService = new PostRealAMLCaseService(jobID);
        int total = 0;
        int success = 0;
        bool result = false;
        try
        {
            DataTable postData = new DataTable();
            bool isComplete = false;

            JobHelper.Write(jobID, "*********** " + jobID + " 傳給主機結案案件 手動 START ************** ", LogState.Info);

            // 取郵局核印資料
            postData = postRealAMLCaseService.GetSendToPostOfficeData(endTime);

            // 產檔及上傳到 MFTP
            isComplete = GenaratorFileAndUploadToMFTP(jobID, fileName, postData, postRealAMLCaseService, dateTime);

            if (isComplete)
            {
                total = postData.Rows.Count;

                // 寫入 BatchLog
                InsertBatchLog(jobID, total, success, "S", "");

                // 發送 Email
                postRealAMLCaseService.SendMail(jobID, "郵局核印 手動:" + fileName + " 上傳成功！", "總筆數:" + total, "上傳成功", this.StartTime);
            }
            else
            {
                // 寫入 BatchLog
                InsertBatchLog(jobID, total, success, "S", "");

                // 發送 Email
                postRealAMLCaseService.SendMail(jobID, "郵局核印 手動:" + fileName + " 成功！", "無資料", "成功", this.StartTime);
            }

            result = true;

            JobHelper.Write(jobID, "[SUCCESS] 郵局核印 手動 END " + fileName, LogState.Info);

        }
        catch (Exception ex)
        {
            // 發送 Email
            postRealAMLCaseService.SendMail(jobID, "郵局核印 手動:" + fileName + " 上傳失敗！", ex.Message, "上傳失敗", this.StartTime);

            JobHelper.Write(jobID, "[FAIL] 郵局核印 手動 " + fileName + " 發生錯誤：" + ex.Message);
        }
        finally
        {
            JobHelper.Write(jobID, " 郵局核印 手動 Job 結束！", LogState.Info);
            JobHelper.Write(jobID, "================================================================================================================================================", LogState.Info);
        }

        return result;
    }

    // 判斷Job工作狀態(0:停止 1:運行)
    private bool CheckJobIsContinue(string jobID, ref string msgID)
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
                BRL_BATCH_LOG.InsertRunning(FunctionKey, jobID, StartTime, "R", "");
            }
        }
        catch (Exception ex)
        {
            result = false;
            JobHelper.Write(jobID, "[FAIL] " + ex.ToString());
        }

        return result;
    }


    static string CHT_WordPadLeftRight(string org, string RL, int sLen, char padStr)
    {
        string sResult = "";
        //計算轉換過實際的總長
        int orgLen = 0;
        int tLen = 0;
        for (int i = 0; i < org.Length; i++)
        {
            string s = org.Substring(i, 1);
            int vLen = 0;
            //判斷 asc 表是否介於 0~128
            if (Convert.ToInt32(s[0]) > 128 || Convert.ToInt32(s[0]) < 0)
            {
                vLen = 2;
            }
            else
            {
                vLen = 1;
            }
            orgLen += vLen;
            if (orgLen > sLen)
            {
                orgLen -= vLen;
                break;
            }
            sResult += s;
        }
        //計算轉換過後，最後實際的長度
        tLen = sLen - (orgLen - org.Length);
        if (RL == "R")
        {
            return sResult.PadLeft(tLen, padStr);
        }
        else
        {
            return sResult.PadRight(tLen, padStr);
        }
    }

    // 產檔及上傳到 MFTP
    private bool GenaratorFileAndUploadToMFTP(string jobID, string fileName, DataTable postData, PostRealAMLCaseService postRealAMLCaseService, DateTime dateTime)
    {

        string path = string.Empty;
        DataTable localFileInfo = new DataTable();
        StringBuilder sb = new StringBuilder();
        //DataTable sb = new DataTable();
        string pwd = string.Empty;
        bool isUploadToFTP = true;
        bool isComplete = false;

        // 取得上傳檔案存放路徑
        path = GetLocalFilePath(jobID);

        string postDataContent = string.Empty;
        string postDataContentb = string.Empty;
        string postDataContentc = string.Empty;

        if (postData.Rows.Count == 0)
        {

            postDataContent  = CHT_WordPadLeftRight("", "L", 14, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 20, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 14, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 12, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 12, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 4, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += "\r\n";

            sb.Append(postDataContent);
        }
        else
        {

            foreach (DataRow row in postData.Rows)
            {

                postDataContent  = CHT_WordPadLeftRight(row["RMMBatchNo"].ToString(), "L", 14, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["AMLInternalID"].ToString(), "L", 20, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["SourceSystem"].ToString(), "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["DataDate"].ToString(), "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["CustomerID"].ToString(), "L", 14, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight("Y", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["LastUpdateMaker"].ToString(), "L", 12, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["LastUpdateChecker"].ToString(), "L", 12, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["LastUpdateBranch"].ToString(), "L", 4, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["LastUpdateDate"].ToString(), "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += "\r\n";

                sb.Append(postDataContent);

            }
        }
        // 設定上傳檔案資訊欄位
        //localFileInfo = postAMLCaseService.SetLocalFileInfoColumn(path, fileName);



        // 寫入 FileTable
        //isComplete = postAMLCaseService.InsertFileData(fileName, dateTime, postData);

        //if (isComplete)
        //{
        // 郵局核印傳送資料組成(讀取 FileTable)
        //sb = postAMLCaseService.SetInvContent(fileName);

        // 寫入檔案
        postRealAMLCaseService.CreateFile(path, fileName, sb.ToString());
        //postOfficeService.CreateFile(path, fileName, sb);

        // 上送FTP
        isUploadToFTP = postRealAMLCaseService.UploadFileToMFTP(jobID, localFileInfo, path, fileName);

        if (isUploadToFTP)
        {
            JobHelper.Write(jobID, "FileName：" + fileName + "，上送FTP 成功", LogState.Info);
            isComplete = true;
        }
        else
        {
            JobHelper.Write(jobID, "[FAIL] FileName：" + fileName + "，上送FTP 失敗");
            isComplete = true;
        }
        //}
        //return true;

        return isComplete;
    }




    // 產檔及上傳到 MFTP
    private bool GenaratorCtlFileAndUploadToMFTP(string jobID, string fileName, DataTable postData, PostRealAMLCaseService postRealAMLCaseService, DateTime dateTime)
    {

        string path = string.Empty;
        DataTable localFileInfo = new DataTable();
        StringBuilder sb = new StringBuilder();
        //DataTable sb = new DataTable();
        string pwd = string.Empty;
        bool isUploadToFTP = true;
        bool isComplete = false;

        // 取得上傳檔案存放路徑
        path = GetLocalFilePath(jobID);

        string postDataContent = string.Empty;
        string postDataContentb = string.Empty;
        string postDataContentc = string.Empty;


            postDataContent = CHT_WordPadLeftRight((postData.Rows.Count).ToString(), "R", 10, '0');   // 1. 1 資料別          (固定值為1)

            postDataContent += "\r\n";

            sb.Append(postDataContent);
        

        // 設定上傳檔案資訊欄位
        //localFileInfo = postAMLCaseService.SetLocalFileInfoColumn(path, fileName);

        // 寫入 FileTable
        //isComplete = postAMLCaseService.InsertFileData(fileName, dateTime, postData);

        //if (isComplete)
        //{
        // 郵局核印傳送資料組成(讀取 FileTable)
        //sb = postAMLCaseService.SetInvContent(fileName);

        // 寫入檔案
        postRealAMLCaseService.CreateFile(path, fileName, sb.ToString());
        //postOfficeService.CreateFile(path, fileName, sb);

        // 上送FTP
        isUploadToFTP = postRealAMLCaseService.UploadFileToMFTP(jobID, localFileInfo, path, fileName);

        if (isUploadToFTP)
        {
            JobHelper.Write(jobID, "FileName：" + fileName + "，上送FTP 成功", LogState.Info);
            isComplete = true;
        }
        else
        {
            JobHelper.Write(jobID, "[FAIL] FileName：" + fileName + "，上送FTP 失敗");
            isComplete = true;
        }
        //}
        //return true;

        return isComplete;
    }


    // 產檔路徑
    private string GetLocalFilePath(string jobID)
    {
        if (string.IsNullOrEmpty(jobID))
        {
            return string.Empty;
        }

        string strFolderName = jobID + DateTime.Now.ToString("yyyyMMdd");

        return AppDomain.CurrentDomain.BaseDirectory + "FileDownload\\" + jobID + "\\" + strFolderName + "\\";
    }

    private void InsertBatchLog(string jobID, int total, int success, string status, string message)
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
        BRL_BATCH_LOG.Insert("01", jobID, this.StartTime, status, sbMessage.ToString());
    }
}
