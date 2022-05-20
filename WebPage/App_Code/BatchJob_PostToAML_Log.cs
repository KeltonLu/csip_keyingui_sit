//******************************************************************
//* 作    者：Ares Dennis
//* 功能說明：異動記錄報送AML排程
//* 創建日期：2021/05/27
//* 修改紀錄：
//*******************************************************************

using System;
using System.Data;
using System.Configuration;
using Quartz;
using CSIPCommonModel.BusinessRules;
using System.Text;
using Framework.Common.Logging;
using Framework.Common.Utility;
using CSIPKeyInGUI.BusinessRules;

/// <summary>
/// BatchJob_PostToAML_Log 的摘要描述
/// </summary>
public class BatchJob_PostToAML_Log : Quartz.IJob
{
    protected string FunctionKey = UtilHelper.GetAppSettings("FunctionKey").ToString();
    protected DateTime StartTime = DateTime.Now;// 記錄Job啟動時間
    protected DateTime EndTime;
    protected JobHelper JobHelper = new JobHelper();
    protected string _MailTitle = "MAKER、CHECKER異動記錄傳送主機結果 批次：";

    public void Execute(JobExecutionContext context)
    {
        int total = 0;
        int success = 0;
        string jobID = context.JobDetail.JobDataMap["jobid"].ToString();
        JobHelper.strJobID = jobID;
        string fileName = "";
        bool isContinue = true;
        string msgID = string.Empty;
        PostAMLLogService postAMLLogService = new PostAMLLogService(jobID);

        try
        {
            DataTable postData = new DataTable();
            DateTime dateTime = new DateTime();
            string startTime = "";
            string endTime = "";
            bool isComplete = false;

            JobHelper.Write(jobID, "*********** " + jobID + " MAKER、CHECKER異動記錄傳送主機 批次 START ************** ", LogState.Info);

            // 判斷Job工作狀態(0:停止 1:運行)
            isContinue = CheckJobIsContinue(jobID, ref msgID);
            
            if (isContinue)
            {
                //20211104_Ares_Jack_ReRun功能
                #region 判斷是否手動設置參數啟動排程
                JobHelper.SaveLog("判斷是否手動輸入參數 啟動排程：開始！", LogState.Info);

                if (context.JobDetail.JobDataMap["param"] != null)
                {
                    JobHelper.SaveLog("手動輸入參數啟動排程：是！", LogState.Info);
                    JobHelper.SaveLog("檢核輸入參數：開始！", LogState.Info);

                    string strParam = context.JobDetail.JobDataMap["param"].ToString();

                    if (strParam.Length == 10)
                    {
                        DateTime tempDt;
                        if (DateTime.TryParse(strParam, out tempDt))
                        {
                            JobHelper.SaveLog("檢核參數：成功！ 參數：" + strParam, LogState.Info);

                            if (BRM_FileInfo.UpdateParam(jobID, tempDt.ToString("yyyyMMdd")))
                            {
                                JobHelper.SaveLog("更新參數至FileInfo：成功！ 參數：" + tempDt.ToString("yyyyMMdd"), LogState.Info);
                            }
                            else
                            {
                                JobHelper.SaveLog("更新參數至FileInfo：失敗！ 參數：" + tempDt.ToString("yyyyMMdd"), LogState.Error);
                                return;
                            }
                        }
                        else
                        {
                            JobHelper.SaveLog("檢核參數：異常！ 參數：" + strParam, LogState.Error);
                            return;
                        }
                    }
                    else
                    {
                        JobHelper.SaveLog("檢核參數：異常！ 參數：" + strParam, LogState.Error);
                        return;
                    }

                    JobHelper.SaveLog("檢核輸入參數：結束！", LogState.Info);
                }
                else
                {
                    JobHelper.SaveLog("手動輸入參數啟動排程：否！", LogState.Info);
                }

                JobHelper.SaveLog("判斷是否手動輸入參數 啟動排程：結束！", LogState.Info);

                #endregion

                // 算起迄時間
                postAMLLogService.GetSearchTime(this.StartTime, out startTime, out endTime);

                endTime = DateTime.Now.ToString("yyyyMMdd");
                dateTime = Convert.ToDateTime(endTime.Substring(0, 4) + "-" + endTime.Substring(4, 2) + "-" + endTime.Substring(6, 2));

                string strdateTime = DateTime.Now.ToString("yyyyMMdd");

                DataTable tblFileInfo = BRAML_File_Import.GetFileInfo(jobID);
                //20211104_Ares_Jack_如果Parameter有值，則使用Parameter的時間
                if (!string.IsNullOrEmpty(tblFileInfo.Rows[0]["Parameter"].ToString()))
                {
                    strdateTime = tblFileInfo.Rows[0]["Parameter"].ToString().Trim();
                }

                fileName = tblFileInfo.Rows[0]["FtpFileName"].ToString().Replace("YYYYMMDD", strdateTime);                                

                // 取AML_CheckLog
                postData = postAMLLogService.GetPostToAML_LogData(strdateTime);

                // 產檔及上傳到 MFTP
                isComplete = GenaratorFileAndUploadToMFTP(jobID, fileName, postData, postAMLLogService, dateTime);

                if (isComplete)
                {
                    success = postData.Rows.Count;                    

                    total = postData.Rows.Count;


                    if (total > 0)
                    {
                        // 寫入 BatchLog
                        InsertBatchLog(jobID, total, success, "S", "");

                        // 發送 Email                        
                        JobHelper.Write(jobID, "MAKER、CHECKER異動記錄傳送主機批次 執行成功：" + fileName + "檔案，共計" + total + "筆數，上傳成功！", LogState.Info);
                        postAMLLogService.SendMail(jobID, _MailTitle + "成功，總筆數：" + total + "筆", _MailTitle + fileName + " 成功，總筆數：" + total + "筆", "成功", this.StartTime);

                    }
                    else
                    {
                        // 寫入 BatchLog
                        InsertBatchLog(jobID, total, success, "S", "");
                        // 發送 Email                        
                        JobHelper.Write(jobID, "MAKER、CHECKER異動記錄傳送主機批次 執行成功：今日無任何需轉出 MAKER、CHECKER異動記錄傳送主機！", LogState.Info);
                        postAMLLogService.SendMail(jobID, _MailTitle + "成功，總筆數：" + total + "筆", _MailTitle + "成功，今日無任何需轉出MAKER、CHECKER異動記錄傳送主機！", "成功", this.StartTime);
                    }
                }

                JobHelper.Write(jobID, "[SUCCESS] MAKER、CHECKER異動記錄傳送主機 批次 END " + fileName, LogState.Info);
            }
        }
        catch (Exception ex)
        {
            // 寫入 BatchLog
            InsertBatchLog(jobID, total, success, "F", "發生錯誤：" + ex.Message);
            // 發送 Email
            JobHelper.Write(jobID, "[FAIL] MAKER、CHECKER異動記錄傳送主機批次 " + fileName + " 發生錯誤：" + ex.Message);            
            postAMLLogService.SendMail(jobID, _MailTitle + "失敗，總筆數：" + total + "筆，次日批次再補傳", _MailTitle + fileName + " 上傳失敗，無法完成。請改由人工上傳，並請於晚間11點前重送！", "失敗", this.StartTime);
        }
        finally
        {
            // 清除 FileInfo Parameter值
            BRM_FileInfo.UpdateParam(jobID, "");

            JobHelper.Write(jobID, " MAKER、CHECKER異動記錄傳送主機 批次 Job 結束！", LogState.Info);
            JobHelper.Write(jobID, "================================================================================================================================================", LogState.Info);
        }
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
            if (dtInfo == null && dtInfo.Rows.Count > 0)
            {
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
    private bool GenaratorFileAndUploadToMFTP(string jobID, string fileName, DataTable postData, PostAMLLogService postAMLLogService, DateTime dateTime)
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
                
                postDataContent = CHT_WordPadLeftRight(row["CORP_NO"].ToString(), "L", 14, ' ');   // 統一編號/自然人ID
                postDataContent += CHT_WordPadLeftRight(row["MER_NO"].ToString(), "L", 10, ' ');   // 資料最後異動Maker
                postDataContent += CHT_WordPadLeftRight(row["LAST_UPD_MAKER"].ToString(), "L", 12, ' ');   // 資料最後異動Maker
                postDataContent += CHT_WordPadLeftRight(row["LAST_UPD_CHECKER"].ToString(), "L", 12, ' ');   // 資料最後異動Checker
                postDataContent += CHT_WordPadLeftRight(row["LAST_UPD_BRANCH"].ToString(), "L", 4, ' ');   // 資料最後異動分行
                postDataContent += CHT_WordPadLeftRight(row["TRANS_ID"].ToString().Trim(), "L", 8, ' ');   // 交易代碼
                postDataContent += CHT_WordPadLeftRight(row["MOD_DATE"].ToString(), "L", 8, ' ');   // 資料最後異動日期

                if (i != rowcountnum)
                {
                    postDataContent += "\n";
                }
                sb.Append(postDataContent);
            }
        }
        // 寫入檔案
        postAMLLogService.CreateFile(path, fileName, sb.ToString());
        //postOfficeService.CreateFile(path, fileName, sb);

        // 上送FTP
        isUploadToFTP = postAMLLogService.UploadFileToMFTP(jobID, localFileInfo, path, fileName);

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