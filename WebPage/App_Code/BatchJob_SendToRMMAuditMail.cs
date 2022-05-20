//******************************************************************
//* 作    者：
//* 功能說明：
//* 創建日期：
//* 修改紀錄：
//* <author>            <time>            <TaskID>                <desc>
//* Ares Luke          2021/03/18         20200031-CSIP EOS       調整執行順序，優先檢核排程是否執行中在檢核是否為執行日
//* Ares Stanley      2021/04/15                                  新增 JobHelper.strJobID
//*******************************************************************

using System;
using System.Data;
using System.Configuration;
using Quartz;
using CSIPCommonModel.BusinessRules;
using System.Text;
using Framework.Common.Logging;
using CSIPKeyInGUI.BusinessRules;

/// <summary>
/// BatchJob_SendToRMMAuditMail 的摘要描述
/// 傳送給Omi Bill平台不合作通知函名單以寄送email
/// 檔名：MERCHANTRMM.DAT、MERCHANTRMM.CTL
/// 撈檔邏輯：AddressLabelTwoMonthFlag =null AND HOCP_EMAIL <> ''
/// </summary>
public class BatchJob_SendToRMMAuditMail : Quartz.IJob
{
    protected string FunctionKey = ConfigurationManager.AppSettings["FunctionKey"].ToString();
    protected DateTime StartTime = DateTime.Now;// 記錄Job啟動時間
    protected JobHelper JobHelper = new JobHelper();
    protected string _MailTitle = "傳送至Omi Bill平台-不合作通知函名單 批次：";
    string AMLfileName = string.Empty;//記錄拋送的檔名

    public void Execute(JobExecutionContext context)
    {
        string JobID = context.JobDetail.JobDataMap["jobid"].ToString();
        string FileNameCtl = string.Empty;//拋送的檔名
        string FileNameDat = string.Empty;//拋送的檔名
        bool isContinue = true;
        string msgID = string.Empty;
        int total = 0;
        JobHelper.strJobID = JobID;

        PostRMMAuditMailService _PostToRmmMailService = new PostRMMAuditMailService(JobID);

        try
        {
            JobHelper.Write(JobID, "*********** " + JobID + " 傳送至Omi Bill平台-不合作通知函名單 批次 START ************** ", LogState.Info);

            DataTable postData = new DataTable();
            bool isComplete = false;

            int days = 7;
            string Today = DateTime.Now.ToString("yyyyMMdd");//用以跟執行日，日期比對用
            string SQLDate = DateTime.Now.ToString("yyyyMM");
            string _ExecDate = DateTime.Now.AddMonths(-2).ToString("yyyyMM");
            
            // 判斷Job工作狀態(0:停止 1:運行)
            isContinue = CheckJobIsContinue(JobID, ref msgID);

            if (isContinue)
            {
                // 20200031-CSIP EOS Ares Luke 修改日期:2021/03/12 修改說明:業務需求排程結束清空FilInfo參數
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

                            if (BRM_FileInfo.UpdateParam(JobID, tempDt.ToString("yyyyMMdd")))
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


                DataTable tblFileInfo = BRAML_File_Import.GetFileInfo(JobID);

                //如果Parameter有值，則使用Parameter的時間
                if (!string.IsNullOrEmpty(tblFileInfo.Rows[0]["Parameter"].ToString()))
                {
                    _ExecDate = tblFileInfo.Rows[0]["Parameter"].ToString().Trim();
                    Today = tblFileInfo.Rows[0]["Parameter"].ToString().Trim();

                    DateTime _dt1 = DateTime.ParseExact(_ExecDate.Trim(), "yyyyMMdd",
                        System.Globalization.CultureInfo.InvariantCulture);
                    _ExecDate = _dt1.AddMonths(-2).ToString("yyyyMMdd");

                    //若於設定檔設定為yyyyMMdd，則取前六碼
                    if (tblFileInfo.Rows[0]["Parameter"].ToString().Trim().Length > 6)
                    {
                        _ExecDate = _ExecDate.Substring(0, 6).Trim();
                        SQLDate = Today.Substring(0, 6).Trim();
                    }
                }

                //取得月底前N 個工作天日期, N = days 的設定
                string workingDate = BRFORM_COLUMN.GetLastWorkingDateFromWorkDateTable(days, SQLDate);

                //判斷今天是否為月底前7個工作日
                if (workingDate == Today)
                {
                    // 取待發送審核通知函名單
                    postData = PostRMMAuditMailService.GetRMMAuditMail(_ExecDate);
                    total = postData.Rows.Count;

                    if (total > 0)
                    {
                        //根據資料組成檔案，並上傳至FTP上
                        FileNameDat = @"MERCHANTRMM.DAT";
                        isComplete = GenaratorFileAndUploadToMFTP("SendRMMAuditMail", FileNameDat, postData,
                            _PostToRmmMailService);

                        if (isComplete)
                        {
                            FileNameCtl = @"MERCHANTRMM.CTL";
                            isComplete = GenaratorCTLFile("SendRMMAuditMail", FileNameCtl, postData,
                                _PostToRmmMailService);
                        }
                    }
                    else
                    {
                        isComplete = true;
                    }

                    if (isComplete)
                    {
                        // 寫入 BatchLog
                        InsertBatchLog(JobID, total, total, "S", "");
                        if (total > 0)
                        {
                            JobHelper.Write(JobID, _MailTitle + FileNameDat + " 成功。總筆數：" + total + "筆數，上傳成功！",
                                LogState.Info);
                            _PostToRmmMailService.SendMail(_MailTitle + " 成功，總筆數：" + total + "筆",
                                _MailTitle + FileNameDat + " 成功，總筆數：" + total + "筆", "上傳成功", this.StartTime);
                        }
                        else
                        {
                            JobHelper.Write(JobID, _MailTitle + FileNameDat + " 執行成功。今日無任何需上傳不合作通知函名單檔資料！",
                                LogState.Info);
                            _PostToRmmMailService.SendMail(_MailTitle + " 成功，今日無任何需上傳不合作通知函名單檔資料！",
                                _MailTitle + " 成功，總筆數：" + total + "筆", "成功", this.StartTime);
                        }
                    }
                    else
                    {
                        InsertBatchLog(JobID, total, 0, "F", "不合作通知函名單上傳至Omi Bill平台失敗");

                        JobHelper.Write(JobID, _MailTitle + " 失敗，不合作通知函名單上傳至Omi Bill平台失敗！");
                        _PostToRmmMailService.SendMail(
                            _MailTitle + "失敗！總筆數：" + total + " 筆，但產出 0 筆，未產出" + total + "筆",
                            _MailTitle + FileNameDat + "上傳失敗！，請確認不合作通知函名單資料", "上傳失敗", this.StartTime);
                    }
                }
                else
                {
                    InsertBatchLog(JobID, total, total, "S", "今天不是月底前" + days + "個工作天，所以不執行不合作地址條(Eamil)產出");

                    JobHelper.Write(JobID, "今天不是月底前" + days + "個工作天，所以不執行不合作地址條(Eamil)產出");
                }
            }
        }
        catch (Exception ex)
        {
            InsertBatchLog(JobID, total, total, "F", "發生錯誤：" + ex.Message);

            JobHelper.Write(JobID, "【FAIL】傳送至Omi Bill平台-不合作通知函名單 批次" + FileNameDat + " 發生錯誤：" + ex.Message);
            // 發送 Email
            _PostToRmmMailService.SendMail(_MailTitle + " 失敗！總筆數：" + total,
                _MailTitle + FileNameDat + " 失敗！總筆數：" + total + "，發生錯誤：" + ex.Message, "失敗", this.StartTime);
        }
        finally
        {
            // 20200031-CSIP EOS Ares Luke 修改日期:2021/03/12 修改說明:業務需求排程結束清空FilInfo參數
            // 清除 FileInfo Parameter值
            BRM_FileInfo.UpdateParam(JobID, "");

            JobHelper.Write(JobID, "傳送至Omi Bill平台-不合作通知函名單 批次 Job 結束！", LogState.Info);
            JobHelper.Write(JobID,
                "================================================================================================================================================",
                LogState.Info);
        }
    }

    // 判斷Job工作狀態(0:停止 1:運行)
    private bool CheckJobIsContinue(string JobID, ref string msgID)
    {
        bool result = true;
        if (JobHelper.SerchJobStatus(JobID).Equals("") || JobHelper.SerchJobStatus(JobID).Equals("0"))
        {
            // Job停止
            JobHelper.Write(JobID, "【FAIL】 Job工作狀態為：停止！");
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
    private bool GenaratorFileAndUploadToMFTP(string jobID, string fileName, DataTable postData, PostRMMAuditMailService _PostToRmmMailService)
    {
        string path = string.Empty;
        StringBuilder sb = new StringBuilder();
        bool isUploadToFTP = true;

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

                postDataContent += CHT_WordPadLeftRight(row["CORP_NO"].ToString(), "L", 10, ' ');                         // 1.客戶統編
                postDataContent += CHT_WordPadLeftRight((row["EMAIL"].ToString().Trim()), "L", 50, ' ');                 // 2.Email
                postDataContent += CHT_WordPadLeftRight((row["REG_NAME"].ToString().Trim()), "L", 122, ' ');     // 3.公司登記名稱
                postDataContent += CHT_WordPadLeftRight((row["EXPIRYDATE"].ToString().Trim()), "L", 8, ' ');        // 4.審查到期日(yyyyMMdd)

                if (i != rowcountnum)
                {
                    postDataContent += "\n";
                }

                sb.Append(postDataContent);
            }
        }

        // 寫入檔案
        _PostToRmmMailService.CreateFile(path, fileName, sb.ToString());

        // 上傳FTP
        isUploadToFTP = UploadToMFTP(jobID, fileName, _PostToRmmMailService);

        string errorMsg = "";

        if (isUploadToFTP)//上傳成功即更新產檔Flag
        {
            //將發送記錄寫進LOG檔
            CSIPKeyInGUI.BusinessRules_new.BRAML_AUDITMAILLOG.AML_AUDITMAILLOGWithBulkCopy("AML_AUDITMAILLOG", postData, ref errorMsg);

            //更新AML_HQ_WORK的AML_HQ_WORK的AddressLabelflag更新成1，
            foreach (DataRow row in postData.Rows)
            {
                BRFORM_COLUMN.updateLabelSended2(row["HQ_ID"].ToString());
                BRFORM_COLUMN.AML_NOTELOG(row["CASE_NO"].ToString(), row["CORP_NO"].ToString(), "", "MERCHANTRMM", "傳送不合作通知函(Email)成功");
            }
        }

        return isUploadToFTP;
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

    private bool GenaratorCTLFile(string jobID, string fileName, DataTable postData, PostRMMAuditMailService _PostToRmmMailService)
    {
        string path = string.Empty;
        StringBuilder sb = new StringBuilder();
        bool isUploadToFTP = true;

        // 取得上傳檔案存放路徑
        path = GetLocalFilePath(jobID);

        string postDataContent = string.Empty;
        int writecount = 0;
        writecount = postData.Rows.Count;

        postDataContent = CHT_WordPadLeftRight(writecount + "", "R", 10, '0');

        sb.Append(postDataContent);

        // 寫入檔案
        _PostToRmmMailService.CreateFile(path, fileName, sb.ToString());

        // 上傳FTP
        isUploadToFTP = UploadToMFTP(jobID, fileName, _PostToRmmMailService);
        
        return isUploadToFTP;
    }

    private bool UploadToMFTP(string jobID, string fileName, PostRMMAuditMailService _PostToRmmMailService)
    {
        string path = string.Empty;
        StringBuilder sb = new StringBuilder();
        bool isUploadToFTP = false;

        // 取得上傳檔案存放路徑
        path = GetLocalFilePath(jobID);

        // 上送FTP
        isUploadToFTP = _PostToRmmMailService.UploadFileToMFTP(jobID, path, fileName);

        if (isUploadToFTP)
        {
            JobHelper.Write(jobID, "FileName：" + fileName + "，上傳FTP 成功", LogState.Info);
        }
        else
        {
            JobHelper.Write(jobID, "[FAIL] FileName：" + fileName + "，上傳FTP 失敗");
        }
        return isUploadToFTP;
    }
}