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
using CSIPKeyInGUI.BusinessRules;

/// <summary>
/// BatchJob_GetMainFrameRtnInfo 的摘要描述
/// 每日收主機下傳的更新檔
/// </summary>
public class BatchJob_GetMainFrameRtnInfo : Quartz.IJob
{
    protected string FunctionKey = UtilHelper.GetAppSettings("FunctionKey").ToString();
    protected DateTime StartTime = DateTime.Now;// 記錄Job啟動時間
    protected DateTime EndTime;
    protected JobHelper JobHelper = new JobHelper();
    protected string _MailTitle = "整批異動特約商店資料回檔 批次：";//2020026

    public void Execute(JobExecutionContext context)
    {
        string jobID = context.JobDetail.JobDataMap["jobid"].ToString();
        JobHelper.strJobID = jobID;
        string fileName = "";
        bool isContinue = true;
        string msgID = string.Empty;
        int total = 0;
        
        MainFrameBatchModifyService MFService = new MainFrameBatchModifyService(jobID);

        try
        {
            JobHelper.Write(jobID, "*********** " + jobID + " 取得【整批異動特約商店資料回檔】批次 START ************** ", LogState.Info);

            


            //假設狀況：主機10/03晚間放檔，CSIP10/04收檔
            string date = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
            // 判斷Job工作狀態(0:停止 1:運行)
            isContinue = CheckJobIsContinue(jobID, ref msgID);

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


                DownloadFileAndInsertTable(jobID, MFService, date, out total);
            }
        }
        catch (Exception ex)
        {
            JobHelper.Write(jobID, "[FAIL]  取得【整批異動特約商店資料回檔】批次 " + fileName + " 發生錯誤：" + ex.Message);
            //20210618_Ares_Stanley-Exception時，將L_BATCH_LOG的Status設為"F"，避免Status保持在"R"之後無法運行批次
            BRL_BATCH_LOG.Delete("01", jobID, "R");
            BRL_BATCH_LOG.Insert("01", jobID, this.StartTime, "F", ex.Message);
            // 發送 Email
            //MFService.SendMail("【整批異動特約商店資料回檔】批次:" + fileName + " 失敗！", "總筆數:" + total, ex.Message, this.StartTime);
            MFService.SendMail(_MailTitle + " 失敗！總筆數：" + total, _MailTitle + fileName + "失敗！總筆數：" + total + "，發生錯誤：" + ex.Message, "失敗", this.StartTime);
        }
        finally
        {
            // 20200031-CSIP EOS Ares Luke 修改日期:2021/03/12 修改說明:業務需求排程結束清空FilInfo參數
            // 清除 FileInfo Parameter值
            BRM_FileInfo.UpdateParam(jobID, "");


            JobHelper.Write(jobID, "  取得【整批異動特約商店資料回檔】批次 Job 結束！", LogState.Info);
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

    private string DownloadFileAndInsertTable(string jobID, MainFrameBatchModifyService _MFService, string date, out int total)
    {
        string folderName = string.Empty;
        string errorCtlMsg = "";//記錄下載結果訊息
        string errorMsg = "";
        bool isInsertOK = false;
        bool isDownloadSucc = false;
        DataTable dat = new DataTable();
        total = 0;


        JobHelper.CreateFolderName(jobID, ref folderName);

        string localPath = AppDomain.CurrentDomain.BaseDirectory + "FileDownload\\" + jobID + "\\" + folderName + "\\";

        string fileName = _MFService.DownloadFromFTP(date, localPath, "TXT", ref errorCtlMsg ,ref isDownloadSucc);

        if (isDownloadSucc)//若檔案下載成功才將資料寫入DataTable中
        {
            // 取檔案內容
            dat = _MFService.GetFileToDataTable(localPath, fileName, out errorMsg);

            total = dat.Rows.Count;
        }        


        errorMsg = SetErrorMsg(errorCtlMsg, errorMsg);

        if (errorMsg == "")
        {
            // 寫入資料庫
            isInsertOK = _MFService.SetDataTableToDB(dat, out errorMsg); ;
        }

        // 發送失敗 mail
        if (errorMsg != "" || !isInsertOK || !isDownloadSucc)
        {
            if (!isDownloadSucc)
            {
                InsertBatchLog(jobID, total, 0, "F", "FTP 取檔失敗");

                JobHelper.Write(jobID, _MailTitle + fileName + " 失敗！ FTP 取檔失敗，未收到整批異動特約商店資料回檔");

                // 發送 Email
                _MFService.SendMail(_MailTitle + " FTP 取檔失敗！未收到檔案", _MailTitle + fileName + " 不存在！未收到整批異動特約商店資料回檔", " FTP 取檔失敗", this.StartTime);
            }
            else
            {
                InsertBatchLog(jobID, total, 0, "F", errorMsg);

                JobHelper.Write(jobID, _MailTitle + fileName + " 失敗！總筆數：" + total + "，" + errorMsg);

                // 發送 Email
                if (total == 0)
                {
                    _MFService.SendMail(_MailTitle + " 失敗！回檔為空檔！", _MailTitle + fileName + " 失敗！ ", errorMsg, this.StartTime);
                }
                else
                {
                    _MFService.SendMail(_MailTitle + " 失敗！總筆數：" + total, _MailTitle + fileName + " 失敗！ 總筆數：" + total, errorMsg, this.StartTime);
                }
            }            
        }
        else
        {
            // 寫入 BatchLog
            InsertBatchLog(jobID, total, total, "S", "");

            JobHelper.Write(jobID, _MailTitle + fileName + " 成功！總筆數：" + total + "，匯入成功共" + total + "筆，匯入失敗O筆",LogState.Info);

            // 發送 Email
            _MFService.SendMail(_MailTitle + " 成功！總筆數：" + total, _MailTitle + fileName + " 成功！總筆數：" + total + "，匯入成功共" + total + "筆，匯入失敗O筆", "成功", this.StartTime);
        }

        return fileName;
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
