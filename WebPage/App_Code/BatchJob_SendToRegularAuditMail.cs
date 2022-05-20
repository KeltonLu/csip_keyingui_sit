using System;
using System.Data;
using System.Configuration;
using Quartz;
using CSIPCommonModel.BusinessRules;
using System.Text;
using Framework.Common.Logging;
using CSIPKeyInGUI.BusinessRules;

/// <summary>
/// BatchJob_SendToRegularAuditMail 的摘要描述
/// 傳送給Omi Bill平台審查通知函名單以寄送定審email
/// 檔名：MERCHANTAML.DAT、MERCHANTAML.CTL
/// 撈檔邏輯：AddressLabelflag =null AND HOCP_EMAIL <> ''
/// </summary>
public class BatchJob_SendToRegularAuditMail : Quartz.IJob
{
    protected string FunctionKey = ConfigurationManager.AppSettings["FunctionKey"].ToString();
    protected DateTime StartTime = DateTime.Now;// 記錄Job啟動時間
    protected JobHelper JobHelper = new JobHelper();
    protected string _MailTitle = "傳送至Omi Bill平台-審查通知函名單 批次：";
    string AMLfileName = string.Empty;//記錄拋送的檔名

    public void Execute(JobExecutionContext context)
    {
        string JobID = context.JobDetail.JobDataMap["jobid"].ToString();
        string FileNameCtl = string.Empty;//拋送的檔名
        string FileNameDat = string.Empty;//拋送的檔名
        bool isContinue = true;
        string msgID = string.Empty;
        int total = 0;

        PostRegularAuditMailService _PostRegularAuditMailService = new PostRegularAuditMailService(JobID);

        try
        {
            JobHelper.strJobID = JobID;
            JobHelper.Write(JobID, "*********** " + JobID + " 傳送至Omi Bill平台-審查通知函名單 批次 START ************** ", LogState.Info);

            DataTable postData = new DataTable();
            
            bool isComplete = false;

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
                string _ExecDate = DateTime.Now.ToString("yyyyMMdd");

                //如果Parameter有值，則使用Parameter的時間
                if (!string.IsNullOrEmpty(tblFileInfo.Rows[0]["Parameter"].ToString()))
                {
                    _ExecDate = tblFileInfo.Rows[0]["Parameter"].ToString().Trim();
                }

                // 取待發送審核通知函名單
                postData = PostRegularAuditMailService.GetRegularAuditMail(_ExecDate);
                total = postData.Rows.Count;

                if (total > 0)
                {
                    //根據資料組成檔案，並上傳至FTP上
                    FileNameDat = @"MERCHANTAML.DAT";
                    isComplete = GenaratorFileAndUploadToMFTP("SendRegularAuditMail", FileNameDat, postData, _PostRegularAuditMailService);

                    if (isComplete)
                    {
                        FileNameCtl = @"MERCHANTAML.CTL";
                        isComplete = GenaratorCTLFile("SendRegularAuditMail", FileNameCtl, postData, _PostRegularAuditMailService);
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
                        JobHelper.Write(JobID, _MailTitle + FileNameDat + " 成功。總筆數：" + total + "筆數，上傳成功！", LogState.Info);
                        _PostRegularAuditMailService.SendMail(_MailTitle + " 成功，總筆數：" + total + "筆", _MailTitle + FileNameDat + " 成功，總筆數：" + total + "筆", "上傳成功", this.StartTime);
                    }
                    else
                    {
                        JobHelper.Write(JobID, _MailTitle + FileNameDat + " 執行成功。今日無任何需上傳審查通知函名單檔資料！", LogState.Info);
                        _PostRegularAuditMailService.SendMail(_MailTitle + " 成功，今日無任何需上傳審查通知函名單檔資料！", _MailTitle + " 成功，總筆數：" + total + "筆", "成功", this.StartTime);

                    }                    
                }
                else
                {
                    InsertBatchLog(JobID, total, 0, "F", "審查通知函名單上傳至Omi Bill平台失敗");

                    JobHelper.Write(JobID, _MailTitle + " 失敗，審查通知函名單上傳至Omi Bill平台失敗！");
                    _PostRegularAuditMailService.SendMail( _MailTitle + "失敗！總筆數：" + total + " 筆，但產出 0 筆，未產出" + total + "筆", _MailTitle + FileNameDat + "上傳失敗！，請確認審查通知函名單資料", "上傳失敗", this.StartTime);
                }
            }
        }
        catch (Exception ex)
        {
            InsertBatchLog(JobID, total, total, "F", "發生錯誤：" + ex.Message);

            JobHelper.Write(JobID, "【FAIL】傳送至Omi Bill平台-審查通知函名單 批次" + FileNameDat + " 發生錯誤：" + ex.Message);
            // 發送 Email
            _PostRegularAuditMailService.SendMail(_MailTitle + " 失敗！總筆數：" + total, _MailTitle + FileNameDat + " 失敗！總筆數：" + total + "，發生錯誤：" + ex.Message, "失敗", this.StartTime);
        }
        finally
        {
            // 20200031-CSIP EOS Ares Luke 修改日期:2021/03/12 修改說明:業務需求排程結束清空FilInfo參數
            // 清除 FileInfo Parameter值
            BRM_FileInfo.UpdateParam(JobID, "");

            JobHelper.Write(JobID, "傳送至Omi Bill平台-審查通知函名單 批次 Job 結束！", LogState.Info);
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
    private bool GenaratorFileAndUploadToMFTP(string jobID, string fileName, DataTable postData, PostRegularAuditMailService _PostRegularAuditMailService)
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
        _PostRegularAuditMailService.CreateFile(path, fileName, sb.ToString());

        // 上傳FTP
        isUploadToFTP = UploadToMFTP(jobID, fileName, _PostRegularAuditMailService);

        string errorMsg = "";

        if (isUploadToFTP)//上傳成功即更新產檔Flag
        {
            //將發送記錄寫進LOG檔
            CSIPKeyInGUI.BusinessRules_new.BRAML_AUDITMAILLOG.AML_AUDITMAILLOGWithBulkCopy("AML_AUDITMAILLOG", postData, ref errorMsg);

            //更新AML_HQ_WORK的AML_HQ_WORK的AddressLabelflag更新成1，
            foreach (DataRow row in postData.Rows)
            {
                BRFORM_COLUMN.updateLabelSended(row["HQ_ID"].ToString());

                //20200804-RQ-2020-021027-001 調整寫入NOTELOG的TYPE
                //BRFORM_COLUMN.AML_NOTELOG(row["CASE_NO"].ToString(), row["CORP_NO"].ToString(), "", "傳送地址條", "傳送定期審查地址條成功(Email)");
                BRFORM_COLUMN.AML_NOTELOG(row["CASE_NO"].ToString(), row["CORP_NO"].ToString(), "", "MERCHANTAML", "傳送定期審查地址條(Email)成功");
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

    private bool GenaratorCTLFile(string jobID, string fileName, DataTable postData, PostRegularAuditMailService _PostRegularAuditMailService)
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
        _PostRegularAuditMailService.CreateFile(path, fileName, sb.ToString());

        // 上傳FTP
        isUploadToFTP = UploadToMFTP(jobID, fileName, _PostRegularAuditMailService);
        
        return isUploadToFTP;
    }

    private bool UploadToMFTP(string jobID, string fileName, PostRegularAuditMailService _PostRegularAuditMailService)
    {
        string path = string.Empty;
        StringBuilder sb = new StringBuilder();
        bool isUploadToFTP = false;

        // 取得上傳檔案存放路徑
        path = GetLocalFilePath(jobID);

        // 上送FTP
        isUploadToFTP = _PostRegularAuditMailService.UploadFileToMFTP(jobID, path, fileName);

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