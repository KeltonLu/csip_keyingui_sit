﻿//******************************************************************
//*  作    者：
//*  功能說明：
//*  創建日期：
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*Ares Luke           2021/01/27        20200031-CSIP EOS       調整Log層級
//*******************************************************************

using System;
using System.Data;
using System.Configuration;
using Quartz;
using CSIPCommonModel.BusinessRules;
using System.Text;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Logging;

/// <summary>
/// BatchJob_GetBRCHStatus 的摘要描述
/// 每日收主機的最新的分公司狀態檔
/// 預計運行模式：每日收檔後，先DELETE 後Insert
/// 修改日期:2021/04/08 修改說明:調整LOG
/// </summary>
public class BatchJob_GetBRCHStatus : Quartz.IJob
{
    protected string FunctionKey = ConfigurationManager.AppSettings["FunctionKey"].ToString();
    protected DateTime StartTime = DateTime.Now;// 記錄Job啟動時間
    protected DateTime EndTime;
    protected JobHelper JobHelper = new JobHelper();
    protected string _MailTitle = "最新分公司狀態檔 批次：";

    public void Execute(JobExecutionContext context)
    {
        string JobID = context.JobDetail.JobDataMap["jobid"].ToString();
        string fileName = "";
        bool isContinue = true;
        string msgID = string.Empty;
        int total = 0;

        MainFrameBRCHStatusService _MainFrameBRCHStatusService = new MainFrameBRCHStatusService(JobID);

        try
        {
            JobHelper.strJobID = JobID;
            JobHelper.Write(JobID, "*********** " + JobID + " 最新分公司狀態檔 批次 START ************** ", LogState.Info);

            
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


                DownloadFileAndInsertTable(JobID, _MainFrameBRCHStatusService, date, out total);
            }
        }
        catch (Exception ex)
        {
            JobHelper.Write(JobID, "[FAIL]  最新分公司狀態檔 批次" + fileName + " 發生錯誤：" + ex.Message);
            //20210618_Ares_Stanley-Exception時，將L_BATCH_LOG的Status設為"F"，避免Status保持在"R"之後無法運行批次
            BRL_BATCH_LOG.Delete("01", JobID, "R");
            BRL_BATCH_LOG.Insert("01", JobID, this.StartTime, "F", ex.Message);
            // 發送 Email
            _MainFrameBRCHStatusService.SendMail(_MailTitle + " 失敗！總筆數：" + total, _MailTitle + fileName + " 失敗！總筆數：" + total + "，發生錯誤：" + ex.Message, "失敗", this.StartTime);
        }
        finally
        {
            // 20200031-CSIP EOS Ares Luke 修改日期:2021/03/12 修改說明:業務需求排程結束清空FilInfo參數
            // 清除 FileInfo Parameter值
            BRM_FileInfo.UpdateParam(JobID, "");

            JobHelper.Write(JobID, "  最新分公司狀態檔 批次 Job 結束！", LogState.Info);
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
    
    private string SetErrorMsg(string errorDownLoadMsg, string errorMsg)
    {
        if (errorDownLoadMsg != "")//檔案下載訊息
        {
            return errorDownLoadMsg;
        }

        if (errorMsg != "")//資料讀取訊息
        {
            return errorMsg;
        }

        return "";
    }

    private string DownloadFileAndInsertTable(string JobID, MainFrameBRCHStatusService _RLService, string date, out int total)
    {
        string folderName = string.Empty;
        string errorDownLoadMsg = "";//記錄下載結果訊息
        string errorMsg = "";
        bool isInsertOK = false;
        bool isDownloadSucc = false;
        DataTable dat = new DataTable();
        total = 0;


        JobHelper.CreateFolderName(JobID, ref folderName);

        string localPath = AppDomain.CurrentDomain.BaseDirectory + "FileDownload\\" + JobID + "\\" + folderName + "\\";

        string fileName = _RLService.DownloadFromFTP(date, localPath, "txt", ref errorDownLoadMsg, ref isDownloadSucc);

        if (isDownloadSucc)//若檔案下載成功才將資料寫入DataTable中
        {
            // 取檔案內容
            dat = _RLService.GetFileToDataTable(localPath, fileName, out errorMsg);

            total = dat.Rows.Count;
        }

        errorMsg = SetErrorMsg(errorDownLoadMsg, errorMsg);

        if (errorMsg == "")
        {
            bool isDelOK = false;

            if (total > 0)//筆數大於0時才刪舊資料，避免上傳空檔時，舊檔案也刪了
            {
                //先刪除
                isDelOK = CSIPKeyInGUI.BusinessRules_new.BRAML_BRCH_STATUS.DelALL(ref errorMsg);

                if (isDelOK)//20200211 truncate成功後才進行insert動作
                {
                    // 寫入資料庫
                    isInsertOK = _RLService.SetDataTableToDB(dat, ref errorMsg);
                }
            }
        }

        // 發送失敗 mail
        if (errorMsg != "" || !isInsertOK || !isDownloadSucc)
        {
            InsertBatchLog(JobID, total, 0, "F", errorMsg);
            
            if (!isDownloadSucc)
            {
                JobHelper.Write(JobID, "最新分公司狀態檔 批次：" + fileName + " 失敗！總筆數:" + total + "，失敗原因：未收到總公司狀態檔", LogState.Info);
                // 發送 Email
                _RLService.SendMail(_MailTitle + "失敗！未收到檔案", _MailTitle + fileName + " 失敗！總筆數：" + total + "，失敗原因：未收到總公司狀態檔", "失敗", this.StartTime);
            }
            else
            {
                JobHelper.Write(JobID, "最新分公司狀態檔 批次：" + fileName + " 失敗！總筆數:" + total + errorMsg, LogState.Info);
                // 發送 Email
                _RLService.SendMail(_MailTitle + "失敗！，總筆數：" + total, _MailTitle + fileName + " 失敗！總筆數：" + total + "，失敗原因：" + errorMsg, "失敗", this.StartTime);
            }
        }
        else
        {
            // 寫入 BatchLog
            InsertBatchLog(JobID, total, total, "S", "");

            JobHelper.Write(JobID, "最新分公司狀態檔 批次：" + fileName + " 成功！總筆數:" + total + "，匯入成功共" + total + "筆，匯入失敗O筆", LogState.Info);

            // 發送 Email
            _RLService.SendMail(_MailTitle + "成功！總筆數：" + total, _MailTitle + fileName + " 成功！總筆數：" + total + "，匯入成功共" + total + "筆，匯入失敗O筆", "成功", this.StartTime);
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