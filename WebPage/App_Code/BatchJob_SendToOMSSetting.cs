using System;
using System.Data;
using System.Configuration;
using Quartz;
using CSIPCommonModel.BusinessRules;
using System.Text;
using Framework.Common.Logging;

/// <summary>
/// BatchJob_SendToMainframeOGCC 的摘要描述
/// 傳送給OMS國籍代號及中文對照表
/// 檔名：CSIP_citizenship.txt
/// </summary>
public class BatchJob_SendToOMSSetting : Quartz.IJob
{
    protected string FunctionKey = ConfigurationManager.AppSettings["FunctionKey"].ToString();
    protected DateTime StartTime = DateTime.Now;// 記錄Job啟動時間
    protected JobHelper JobHelper = new JobHelper();
    protected string _MailTitle = "傳送給OMS國籍代號設定檔 批次：";

    public void Execute(JobExecutionContext context)
    {
        string JobID = context.JobDetail.JobDataMap["jobid"].ToString();
        string fileName = "";
        bool isContinue = true;
        string msgID = string.Empty;
        int total = 0;

        PostMainframeOGCCService _PostMainframeOGCCService = new PostMainframeOGCCService(JobID);

        try
        {
            JobHelper.strJobID = JobID;
            JobHelper.Write(JobID, "*********** " + JobID + " 傳送給OMS國籍代號設定檔 批次 START ************** ", LogState.Info);

            DataTable postData = new DataTable();
            string PostFileName = string.Empty;//拋送的檔名
            bool isComplete = false;

            // 判斷Job工作狀態(0:停止 1:運行)
            isContinue = CheckJobIsContinue(JobID, ref msgID);

            if (isContinue)
            {
                // 取得國籍設定檔資料( to OMS)
                postData = PostMainframeOGCCService.SendToOMSCountrySetting();

                //根據資料組成檔案，並上傳至FTP上
                PostFileName = "CSIP_CITIZENSHIP.TXT";
                isComplete = GenaratorFileAndUploadToMFTP("SendToOMSSetting", PostFileName, postData, _PostMainframeOGCCService);
                total = postData.Rows.Count;
                if (total > 0)
                {
                    InsertBatchLog(JobID, total, total, "S", "");

                    JobHelper.Write(JobID, _MailTitle+ PostFileName + " 成功。總筆數：" + total + "筆數，上傳成功！", LogState.Info);
                    _PostMainframeOGCCService.SendMail( _MailTitle + " 成功，總筆數：" + total + "筆", _MailTitle + PostFileName + " 成功，總筆數：" + total + "筆", "上傳成功", this.StartTime);
                }
                else
                {
                    InsertBatchLog(JobID, total, total, "F", "無國籍設定檔資料");

                    JobHelper.Write(JobID, _MailTitle + " 失敗，今日無國籍設定檔資料！", LogState.Info);
                    _PostMainframeOGCCService.SendMail(_MailTitle + " 失敗，無國籍設定檔資料！", _MailTitle + PostFileName + " 失敗，請確認CSIP的國籍設定檔資料", "上傳失敗", this.StartTime);
                }
            }
        }
        catch (Exception ex)
        {
            InsertBatchLog(JobID, total, total, "F", "發生錯誤：" + ex.Message);

            JobHelper.Write(JobID, "【FAIL】傳送給OMS國籍代號設定檔 批次" + fileName + " 發生錯誤：" + ex.Message);
            // 發送 Email
            _PostMainframeOGCCService.SendMail(_MailTitle + " 失敗！總筆數：" + total, _MailTitle + fileName + " 失敗！總筆數：" + total + "，發生錯誤：" + ex.Message, "失敗", this.StartTime);
        }
        finally
        {
            JobHelper.Write(JobID, "傳送給OMS國籍代號設定檔 批次 Job 結束！", LogState.Info);
            JobHelper.Write(JobID, "================================================================================================================================================", LogState.Info);
        }
    }

    // 判斷Job工作狀態(0:停止 1:運行)
    private bool CheckJobIsContinue(string JobID, ref string msgID)
    {
        bool result = true;
        if (JobHelper.SerchJobStatus(JobID).Equals("") || JobHelper.SerchJobStatus(JobID).Equals("0"))
        {
            // Job停止
            JobHelper.Write(JobID, "【FAIL】 Job工作狀態為：停止！", LogState.Info);
            result = false;
        }

        // 檢測Job是否在執行中
        try
        {
            DataTable dtInfo = BRL_BATCH_LOG.GetRunningDate(FunctionKey, JobID, "R", ref msgID);
            if (dtInfo == null || dtInfo.Rows.Count > 0) //20210531_Ares_Stanley-修正Job執行檢核條件
            {
                JobHelper.SaveLog("JOB 工作狀態為：正在執行！", LogState.Info);
                // 返回不執行
                result = false;
            }
            else
            {
                // 記錄Job執行資訊
                BRL_BATCH_LOG.InsertRunning(FunctionKey, JobID, StartTime, "R", "");
            }
        }
        catch (Exception ex)
        {
            result = false;
            JobHelper.Write(JobID, "【FAIL】" + ex.ToString());
        }

        return result;
    }

    // 產檔及上傳到 MFTP
    private bool GenaratorFileAndUploadToMFTP(string jobID, string fileName, DataTable postData, PostMainframeOGCCService _PostMainframeOGCCService)
    {

        string path = string.Empty;
        DataTable localFileInfo = new DataTable();
        StringBuilder sb = new StringBuilder();
        string pwd = string.Empty;
        bool isUploadToFTP = true;
        bool isComplete = false;
        // 取得上傳檔案存放路徑
        path = GetLocalFilePath(jobID);

        string postDataContentb = string.Empty;
        string postDataContentc = string.Empty;

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

                postDataContent = CHT_WordPadLeftRight(row["TYPE"].ToString(), "L", 2, ' ');                        // 1.資料別
                postDataContent += CHT_WordPadLeftRight(row["CODE_ID"].ToString(), "L", 12, ' ');           // 2.設定代碼
                postDataContent += CHT_WordPadLeftRight(ToWide(row["CODE_NAME"].ToString().Trim()), "L", 100, ' ');   // 3.代碼中文
                postDataContent += CHT_WordPadLeftRight(ToWide(row["CODE_EN_NAME"].ToString().Trim()), "L", 100, ' ');   // 4.代碼英文

                if (i != rowcountnum)
                {
                    postDataContent += "\n";
                }

                sb.Append(postDataContent);
            }
        }

        // 寫入檔案
        _PostMainframeOGCCService.CreateFile(path, fileName, sb.ToString());
        // 上送FTP
        isUploadToFTP = _PostMainframeOGCCService.UploadFileToMFTP(jobID, localFileInfo, path, fileName);


        if (isUploadToFTP)
        {
            JobHelper.Write(jobID, "FileName：" + fileName + "，上送FTP 成功", LogState.Info);
            isComplete = true;
        }
        else
        {
            JobHelper.Write(jobID, "[FAIL] FileName：" + fileName + "，上送FTP 失敗");
            isComplete = false;
        }

        return isComplete;
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

    ///字串轉全形
    ///</summary>
    ///<param name="input">任一字元串</param>
    ///<returns>全形字元串</returns>
    public string ToWide(string input)
    {
        //半形轉全形：
        char[] c = input.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            //全形空格為12288，半形空格為32
            if (c[i] == 32)
            {
                c[i] = (char)12288;
                continue;
            }
            //其他字元半形(33-126)與全形(65281-65374)的對應關係是：均相差65248
            if (c[i] < 127)
                c[i] = (char)(c[i] + 65248);
        }
        return new string(c);
    }

    private void InsertBatchLog(string JobID, int total, int success, string status, string message)
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

        BRL_BATCH_LOG.Delete("01", JobID, "R");
        BRL_BATCH_LOG.Insert("01", JobID, this.StartTime, status, sbMessage.ToString());
    }
}