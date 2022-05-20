//******************************************************************
//*  作    者：
//*  功能說明：
//*  創建日期：
//*  修改記錄：

//*<author>            <time>               <TaskID>                <desc>
//* Ares_Luke          2021/03/04           20200031-CSIP EOS       調整LOG層級
//* Ares_Luke          2021/04/13           20200031-CSIP EOS       新增下載LOG訊息
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
/// BatchJob_GetMailResultFromOMI 的摘要描述
/// 每日收OMI BILL平台MAIL發送失敗清單
/// RC單：RQ-2019-030155-000
/// 建立日期：2020/06/23
/// 檔案規則：omi bill平台每日於11點下檔，收完檔即刪掉FTP上的檔案
/// 檔名固定為：INIB1082.TXT 
/// 窗口：官鴻
/// </summary>

public class BatchJob_GetMailResultFromOMI : Quartz.IJob
{
    protected string FunctionKey = ConfigurationManager.AppSettings["FunctionKey"].ToString();
    protected DateTime StartTime = DateTime.Now;// 記錄Job啟動時間
    protected DateTime EndTime;
    protected JobHelper JobHelper = new JobHelper();
    protected string _MailTitle = "接收Omi Bill平台-審查通知函名單回覆檔 批次：";
    protected string _ExecDate = string.Empty;

    public void Execute(JobExecutionContext context)
    {
        string JobID = context.JobDetail.JobDataMap["jobid"].ToString();
        string fileName = "";
        bool isContinue = true;
        string msgID = string.Empty;
        int total = 0;

        MailResultFromOMIService _MailResultFromOMIService = new MailResultFromOMIService(JobID);

        try
        {
            JobHelper.Write(JobID, "*********** " + JobID + _MailTitle + "START ************** ", LogState.Info);
            JobHelper.strJobID = JobID;


            string date = DateTime.Now.ToString("yyyyMMdd");
            
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
                _ExecDate = DateTime.Now.AddDays(-1).ToString("yyyyMMdd").Trim();//20200714 調整，剔退檔內容為前一天的剔退內容

                //如果Parameter有值，則使用Parameter的時間
                if (!string.IsNullOrEmpty(tblFileInfo.Rows[0]["Parameter"].ToString()))
                {
                    _ExecDate = tblFileInfo.Rows[0]["Parameter"].ToString().Trim();
                }

                DownloadFileAndInsertTable(JobID, _MailResultFromOMIService, date, out total);
            }
        }
        catch (Exception ex)
        {
            JobHelper.Write(JobID, "【FAIL】" + _MailTitle + fileName + " 發生錯誤：" + ex.Message);
            //20210618_Ares_Stanley-Exception時，將L_BATCH_LOG的Status設為"F"，避免Status保持在"R"之後無法運行批次
            BRL_BATCH_LOG.Delete("01", JobID, "R");
            BRL_BATCH_LOG.Insert("01", JobID, this.StartTime, "F", ex.Message);
            // 發送 Email
            _MailResultFromOMIService.SendMail(_MailTitle + " 失敗！總筆數：" + total, _MailTitle + fileName + " 失敗！總筆數：" + total + "，發生錯誤：" + ex.Message, "失敗", this.StartTime);
        }
        finally
        {
            // 20200031-CSIP EOS Ares Luke 修改日期:2021/03/12 修改說明:業務需求排程結束清空FilInfo參數
            // 清除 FileInfo Parameter值
            BRM_FileInfo.UpdateParam(JobID, "");

            JobHelper.Write(JobID, _MailTitle + " Job 結束！", LogState.Info);
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
            JobHelper.Write(JobID, "[FAIL] Job工作狀態為：停止！", LogState.Info);
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
            JobHelper.Write(JobID, "[FAIL] " + ex.ToString());
        }

        return result;
    }
    
    private string SetErrorMsg(string errorCtlMsg, string errorMsg)
    {
        if (errorCtlMsg != "")//檔案下載訊息
        {
            return errorCtlMsg;
        }

        if (errorMsg != "")//資料讀取訊息
        {
            return errorMsg;
        }

        return "";
    }

    private string DownloadFileAndInsertTable(string JobID, MailResultFromOMIService _RLService, string date, out int total)
    {
        string folderName = string.Empty;
        string errorDownloadMsg = "";//記錄下載結果訊息
        string errorMsg = "";
        bool isUpdateOK = false;
        bool isDownloadSucc = false;
        DataTable dat = new DataTable();
        total = 0;
        string SendFailCorp = string.Empty;//20200820-10月RC-email失敗的統編寫在批次通知信函內

        JobHelper.CreateFolderName(JobID, ref folderName);

        string localPath = AppDomain.CurrentDomain.BaseDirectory + "FileDownload\\" + JobID + "\\" + folderName + "\\";

        JobHelper.SaveLog("檔案下載,開始！", LogState.Info);

        string fileName = _RLService.DownloadFromFTP(date, localPath, "TXT", ref errorDownloadMsg, ref isDownloadSucc);

        if (isDownloadSucc)//若檔案下載成功才將資料寫入DataTable中
        {
            JobHelper.SaveLog("檔案下載：成功！", LogState.Info);
            // 取檔案內容
            dat = _RLService.GetFileToDataTable(localPath, fileName, out errorMsg);
        }
        else
        {
            JobHelper.SaveLog("檔案下載：失敗！", LogState.Info);

        }
        JobHelper.SaveLog("檔案下載,結束！", LogState.Info);

        errorMsg = SetErrorMsg(errorDownloadMsg, errorMsg);

        if (errorMsg == "")
        {
            isUpdateOK = _RLService.SetDataTableToDB(dat, _ExecDate, ref errorMsg, ref total, ref SendFailCorp);//20200820-10月RC-email失敗的統編寫在批次通知信函內
        }

        // 發送失敗 mail
        if (errorMsg != "" || !isUpdateOK || !isDownloadSucc)
        {
            InsertBatchLog(JobID, total, 0, "F", errorMsg);
            
            if (!isDownloadSucc)
            {
                JobHelper.Write(JobID, _MailTitle + fileName + " 失敗！總筆數:" + total + "，失敗原因：未收到OMI BILL平台-定審通知函回檔", LogState.Info);
                // 發送 Email
                _RLService.SendMail(_MailTitle + "失敗！未收到檔案", _MailTitle + fileName + " 失敗！失敗原因：未收到OMI BILL平台-定審通知函回檔", "失敗", this.StartTime);
            }
            else if (errorMsg.Trim().Equals("資料為空檔"))
            {
                InsertBatchLog(JobID, total, total, "S", "");
                JobHelper.Write(JobID, _MailTitle + fileName + " 成功！總筆數:" + total + "，匯入成功共" + total + "筆，匯入失敗O筆", LogState.Info);
                _RLService.SendMail(_MailTitle + "成功，無筆數", _MailTitle + fileName + " 成功！無筆數", "成功", this.StartTime);
            }
            else
            {
                JobHelper.Write(JobID, _MailTitle + fileName + " 失敗！總筆數:" + total + errorMsg, LogState.Error);
                // 發送 Email
                _RLService.SendMail(_MailTitle + "失敗！，總筆數：" + total, _MailTitle + fileName + " 失敗！總筆數：" + total + "，失敗原因：" + errorMsg, "失敗", this.StartTime);
            }
        }
        else
        {
            // 寫入 BatchLog
            InsertBatchLog(JobID, total, total, "S", "");

            JobHelper.Write(JobID, "傳送至Omi Bill平台-審查通知函名單回覆檔 批次：" + fileName + " 成功！總筆數:" + total + "，匯入成功共" + total + "筆，匯入失敗O筆", LogState.Info);

            // 發送 Email
            //20200820-10月RC-email失敗的統編寫在批次通知信函內
            _RLService.SendMail(_MailTitle + "成功！總筆數：" + total, _MailTitle + fileName + " 成功！總筆數：" + total + "，匯入成功共" + total + "筆，匯入失敗O筆 ，email寄送失敗統編：" + SendFailCorp, "成功", this.StartTime);
        }

        return fileName;
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