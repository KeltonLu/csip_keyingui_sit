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
using Quartz;
using CSIPCommonModel.BusinessRules;
using System.Text;
using Framework.Common.Logging;
using Framework.Common.Utility;

/// <summary>
/// BatchJob_SendToAMLBranch 的摘要描述
/// </summary>
public class BatchJob_SendToAMLBranch : Quartz.IJob
{
    protected string FunctionKey = UtilHelper.GetAppSettings("FunctionKey").ToString();
    protected DateTime StartTime = DateTime.Now;// 記錄Job啟動時間
    protected DateTime EndTime;
    protected JobHelper JobHelper = new JobHelper();
    protected string _MailTitle = "分公司異動檔傳送主機結果 批次：";//20191227-RQ-2019-030155-002-批次信函調整 by Peggy

    public void Execute(JobExecutionContext context)
    {
        int total = 0;
        int success = 0;
        string jobID = context.JobDetail.JobDataMap["jobid"].ToString();
        JobHelper.strJobID = jobID;
        string fileName = "";
        bool isContinue = true;
        string msgID = string.Empty;
        PostBranchService postBranchService = new PostBranchService(jobID);

        try
        {
            DataTable postData = new DataTable();
            DateTime dateTime = new DateTime();
            string startTime = "";
            string endTime = "";
            bool isComplete = false;

            JobHelper.Write(jobID, "*********** " + jobID + " 分公司異動檔上傳 批次 START ************** ", LogState.Info);

            // 判斷Job工作狀態(0:停止 1:運行)
            isContinue = CheckJobIsContinue(jobID, ref msgID);

            if (isContinue)
            {
                // 算起迄時間
                postBranchService.GetSearchTime(this.StartTime, out startTime, out endTime);

                endTime = DateTime.Now.ToString("yyyyMMdd");
                dateTime = Convert.ToDateTime(endTime.Substring(0, 4) + "-" + endTime.Substring(4, 2) + "-" + endTime.Substring(6, 2));

                string strdateTime = DateTime.Now.ToString("yyyyMMdd");

                //fileName = @"AML_UPD_BR_" + strdateTime + ".txt";
                //20210917 檔案名稱調整為抓DB的方式 by Ares Jack
                DataTable tblRealFileInfo = BRAML_File_Import.GetFileInfo("PostBranchToAML");
                fileName = tblRealFileInfo.Rows[0]["FtpFileName"].ToString().Replace("yyyyMMdd", strdateTime) + ".txt";

                // 取分公司異動檔
                postData = postBranchService.GetBranchDataSendToAMLData(strdateTime);

                // 產檔及上傳到 MFTP
                isComplete = GenaratorFileAndUploadToMFTP(jobID, fileName, postData, postBranchService, dateTime);

                if (isComplete)
                {
                    success = postData.Rows.Count;

                    if (postData.Rows.Count > 0)
                    {
                        foreach (DataRow row in postData.Rows)
                        {

                            BRFORM_COLUMN.UpdateBranchworktable(row["ID"].ToString());
                            BRFORM_COLUMN.AML_CASE_ACT_LOG("", row["BRCH_BATCH_NO"].ToString(), row["BRCH_INTER_ID"].ToString(), "拋送分公司檔", "拋送分公司檔完成");
                            BRFORM_COLUMN.AML_NOTELOG(row["CASE_NO"].ToString(), row["BRCH_BATCH_NO"].ToString(), "", "拋送分公司檔", "拋送分公司檔完成");

                        }
                    }

                    total = postData.Rows.Count;


                    if (total > 0)
                    {
                        // 寫入 BatchLog
                        InsertBatchLog(jobID, total, success, "S", "");
                        
                        // 發送 Email
                        //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
                        JobHelper.Write(jobID, "分公司異動檔 轉出JOB執行成功：分公司異動檔" + fileName + "檔案，共計" + total + "筆數，上傳成功！", LogState.Info);
                        //postBranchService.SendMail(jobID, "分公司異動檔 轉出JOB執行成功：分公司異動檔" + fileName + "檔案，共計" + total + "筆數，上傳成功！", "總筆數:" + total, "上傳成功", this.StartTime);
                        postBranchService.SendMail(jobID, _MailTitle + "成功，總筆數：" + total + "筆", _MailTitle + fileName + " 成功，總筆數：" + total + "筆", "成功", this.StartTime);

                    }
                    else
                    {
                        // 寫入 BatchLog
                        InsertBatchLog(jobID, total, success, "S", "");

                        // 發送 Email
                        //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
                        JobHelper.Write(jobID, "分公司異動檔檔 轉出JOB執行成功：今日無任何需轉出分公司異動檔資料！", LogState.Info);
                        //postBranchService.SendMail(jobID, "分公司異動檔檔 轉出JOB執行成功：今日無任何需轉出分公司異動檔資料！", "", "成功", this.StartTime);
                        postBranchService.SendMail(jobID, _MailTitle + "成功，總筆數：" + total + "筆", _MailTitle + "成功，今日無任何需轉出分公司異動檔資料！", "成功", this.StartTime);
                    }
                }

                JobHelper.Write(jobID, "[SUCCESS] 分公司異動檔 批次 END " + fileName, LogState.Info);
            }
        }
        catch (Exception ex)
        {
            // 寫入 BatchLog
            InsertBatchLog(jobID, total, success, "F", "發生錯誤：" + ex.Message);
            // 發送 Email
            JobHelper.Write(jobID, "[FAIL] 分公司異動檔批次 " + fileName + " 發生錯誤：" + ex.Message);

            //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
            //postBranchService.SendMail(jobID, "分公司異動檔轉出JOB執行失敗：分公司檔" + fileName + "檔案，共計" + total + "筆數，上傳失敗，無法完成。請改由人工上傳，並請於晚間11點前重送！", ex.Message, "上傳失敗", this.StartTime);
            postBranchService.SendMail(jobID, _MailTitle + "失敗，總筆數：" + total + "筆，次日批次再補傳", _MailTitle + fileName + " 上傳失敗，無法完成。請改由人工上傳，並請於晚間11點前重送！", "失敗", this.StartTime);
        }
        finally
        {
            JobHelper.Write(jobID, " 分公司異動檔 批次 Job 結束！", LogState.Info);
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
        string fileName = @"CTCBAuth" + endTime + ".DAT";
        PostBranchService postBranchService = new PostBranchService(jobID);
        int total = 0;
        int success = 0;
        bool result = false;
        try
        {
            DataTable postData = new DataTable();
            bool isComplete = false;

            JobHelper.Write(jobID, "*********** " + jobID + " 分公司異動檔上傳  手動 START ************** ", LogState.Info);

            // 取郵局核印資料
            postData = postBranchService.GetSendToPostOfficeData(endTime);

            // 產檔及上傳到 MFTP
            isComplete = GenaratorFileAndUploadToMFTP(jobID, fileName, postData, postBranchService, dateTime);

            if (isComplete)
            {
                success = postData.Rows.Count;
                total = postData.Rows.Count;

                // 寫入 BatchLog
                InsertBatchLog(jobID, total, success, "S", "");

                // 發送 Email
                postBranchService.SendMail(jobID, "分公司異動檔 手動:" + fileName + " 上傳成功！", "總筆數:" + total, "上傳成功", this.StartTime);
            }
            else
            {
                // 寫入 BatchLog
                InsertBatchLog(jobID, total, success, "S", "");

                // 發送 Email
                postBranchService.SendMail(jobID, "分公司異動檔 手動:" + fileName + " 成功！", "無資料", "成功", this.StartTime);
            }

            result = true;

            JobHelper.Write(jobID, "[SUCCESS] 分公司異動檔 手動 END " + fileName, LogState.Info);

        }
        catch (Exception ex)
        {
            // 發送 Email
            postBranchService.SendMail(jobID, "分公司異動檔 手動:" + fileName + " 上傳失敗！", ex.Message, "上傳失敗", this.StartTime);

            JobHelper.Write(jobID, "[FAIL] 分公司異動檔 手動 " + fileName + " 發生錯誤：" + ex.Message);
        }
        finally
        {
            JobHelper.Write(jobID, " 分公司異動檔 手動 Job 結束！", LogState.Info);
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

    static string CHT_WordPadLeftRight2(string org, string RL, int sLen, char padStr)
    {
        if (org.Equals(""))
        {
            org = "　";
        }
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
    private bool GenaratorFileAndUploadToMFTP(string jobID, string fileName, DataTable postData, PostBranchService postBranchService, DateTime dateTime)
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


        if (postData.Rows.Count == 0)
        {

            string postDataContent = string.Empty;
            /*
            postDataContent += CHT_WordPadLeftRight("", "L", 14, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 20, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 4, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 2, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 14, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 30, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 30, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 40, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 40, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 10, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 20, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 22, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 22, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 22, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 20, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 4, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 152, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += "\r\n";
            */
            sb.Append(postDataContent);

        }
        else
        {
            int rowcountnum = postData.Rows.Count;
            int i = 0;

            foreach (DataRow row in postData.Rows)
            {
                i = i + 1;
                string postDataContent = string.Empty;

                postDataContent = CHT_WordPadLeftRight(row["BRCH_BATCH_NO"].ToString(), "L", 14, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_INTER_ID"].ToString(), "L", 20, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_BRCH_NO"].ToString(), "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_BRCH_SEQ"].ToString(), "L", 4, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_BRCH_TYPE"].ToString(), "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_NATION"].ToString(), "L", 2, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_BIRTH_DATE"].ToString(), "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight2(row["BRCH_PERM_CITY"].ToString(), "L", 12, ' ') + "!";
                postDataContent += CHT_WordPadLeftRight2(row["BRCH_PERM_ADDR1"].ToString(), "L", 28, ' ') + "!";
                postDataContent += CHT_WordPadLeftRight2(row["BRCH_PERM_ADDR2"].ToString(), "L", 28, ' ') + "!";
                postDataContent += CHT_WordPadLeftRight2(row["BRCH_CHINESE_NAME"].ToString(), "L", 38, ' ');
                postDataContent += CHT_WordPadLeftRight(row["BRCH_ENGLISH_NAME"].ToString(), "L", 40, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_ID"].ToString(), "L", 10, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_OWNER_ID_ISSUE_DATE"].ToString(), "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight2(row["BRCH_OWNER_ID_ISSUE_PLACE"].ToString(), "L", 18, ' ');
                postDataContent += CHT_WordPadLeftRight(row["BRCH_OWNER_ID_REPLACE_TYPE"].ToString(), "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_ID_PHOTO_FLAG"].ToString(), "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_PASSPORT"].ToString(), "L", 22, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_PASSPORT_EXP_DATE"].ToString(), "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_RESIDENT_NO"].ToString(), "L", 22, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_RESIDENT_EXP_DATE"].ToString(), "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_OTHER_CERT"].ToString(), "L", 22, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_OTHER_CERT_EXP_DATE"].ToString(), "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_COMP_TEL"].ToString(), "L", 20, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_CREATE_DATE"].ToString(), "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_STATUS"].ToString(), "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_CIRCULATE_MERCH"].ToString(), "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_HQ_BRCH_NO"].ToString(), "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_HQ_BRCH_SEQ_NO"].ToString(), "L", 4, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_UPDATE_DATE"].ToString(), "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_QUALIFY_FLAG"].ToString(), "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_UPDATE_ID"].ToString(), "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_REAL_CORP"].ToString(), "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["LAST_UPD_MAKER"].ToString(), "L", 12, ' ');   // 1. 1 資料別          (固定值為1) //20210806 EOS_AML(NOVA) 增加拋送三個欄位(最後異動分行、異動經辦員編、資料最後異動主管)  by Ares Dennis
                postDataContent += CHT_WordPadLeftRight(row["LAST_UPD_CHECKER"].ToString(), "L", 12, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["LAST_UPD_BRANCH"].ToString(), "L", 4, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["BRCH_RESERVED_FILLER"].ToString(), "L", 149, ' ');   // 1. 1 資料別          (固定值為1)

                if (i != rowcountnum)
                {
                    postDataContent += "\n";
                }
                sb.Append(postDataContent);
            }
        }
        // 寫入檔案
        postBranchService.CreateFile(path, fileName, sb.ToString());
        //postOfficeService.CreateFile(path, fileName, sb);

        // 上送FTP
        isUploadToFTP = postBranchService.UploadFileToMFTP(jobID, localFileInfo, path, fileName);

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