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
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Logging;
using Framework.Common.Utility;

/// <summary>
/// BatchJob_GetAMLAFileRtnInfo 的摘要描述
/// AML系統回覆RMM OK A檔格式剔退
/// 檔名：AML_TW_RMMOK_A_CARD_yyyyMMdd
/// </summary>
public class BatchJob_GetAMLAFileRtnInfo : Quartz.IJob
{
    protected string FunctionKey = UtilHelper.GetAppSettings("FunctionKey").ToString();
    protected DateTime StartTime = DateTime.Now;// 記錄Job啟動時間
    protected DateTime EndTime;
    protected JobHelper JobHelper = new JobHelper();
    protected string _MailTitle = "AML系統回覆RMM OK A檔格式剔退 批次：";//20191227-RQ-2019-030155-002-批次信函調整 by Peggy

    public void Execute(JobExecutionContext context)
    {
        string jobID = context.JobDetail.JobDataMap["jobid"].ToString();
        JobHelper.strJobID = jobID;
        string fileName = "";
        bool isContinue = true;
        string msgID = string.Empty;
        int total = 0;

        AMLAFileRtnServiceFromAML aMLAFileRtnService = new AMLAFileRtnServiceFromAML(jobID);

        try
        {
            JobHelper.Write(jobID, "*********** " + jobID + " 取得 結案AML AFile回檔 資料 批次 START ************** ", LogState.Info);

            string date = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");

            //date = "20190307";

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

                DownloadFileAndInsertTable(jobID, aMLAFileRtnService, date, out total);
            }
        }
        catch (Exception ex)
        {
            JobHelper.Write(jobID, "[FAIL]  取得 結案AML AFile回檔 資料 批次 " + fileName + " 發生錯誤：" + ex.Message);
            //20210618_Ares_Stanley-Exception時，將L_BATCH_LOG的Status設為"F"，避免Status保持在"R"之後無法運行批次
            BRL_BATCH_LOG.Delete("01", jobID, "R");
            BRL_BATCH_LOG.Insert("01", jobID, this.StartTime, "F", ex.Message);
            // 發送 Email
            //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
            //aMLAFileRtnService.SendMail("結案AML AFile回檔 批次:" + fileName + " 失敗！", "總筆數:" + total, ex.Message, this.StartTime);
            aMLAFileRtnService.SendMail(_MailTitle + fileName + " 失敗！", "發生錯誤：" + ex.Message, "失敗", this.StartTime);
        }
        finally
        {
            // 20200031-CSIP EOS Ares Luke 修改日期:2021/03/12 修改說明:業務需求排程結束清空FilInfo參數
            // 清除 FileInfo Parameter值
            BRM_FileInfo.UpdateParam(jobID, "");

            JobHelper.Write(jobID, "  取得 結案AML AFile回檔 檔資料 批次 Job 結束！", LogState.Info);
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

    private string SetErrorMsg(string errorDatMsg, string errorCtlMsg, string errorMsg)
    {
        if (errorDatMsg != "")
        {
            return errorDatMsg;
        }

        if (errorCtlMsg != "")
        {
            return errorCtlMsg;
        }

        if (errorMsg != "")
        {
            return errorMsg;
        }

        return "";
    }

    private string DownloadFileAndInsertTable(string jobID, AMLAFileRtnServiceFromAML aMLAFileRtnService, string date, out int total)
    {
        string folderName = string.Empty;
        string errorDatMsg = "";
        string errorCtlMsg = "";
        string errorMsg = "";
        bool isInsertOK = false;
        DataTable dat = new DataTable();
        total = 0;
        string ErrorChi = string.Empty;
        //20200227-RQ-2019-030155-003，區別是源自CSIP / OMS的剔退檔
        int OMSCount = 0;//記錄OMS剔退筆數
        int CSIPCount = 0;//記錄CSIP剔退筆數
        // 20220411 修改說明:發送成功 email，若有 OMS 筆數，則內容需增加對應筆數的身份證字號 by Kelton 
        string CustomerIDs = string.Empty; // 紀錄所有 OMS 資料的身份證字號

        bool isDownloadOK = false; //20191230-RQ-2019-030155-002-批次信函調整：增加取得取檔結果的參數 by Peggy

        JobHelper.CreateFolderName(jobID, ref folderName);

        string localPath = AppDomain.CurrentDomain.BaseDirectory + "FileDownload\\" + jobID + "\\" + folderName + "\\";

        //20191230-RQ-2019-030155-002-批次信函調整：增加取得取檔結果的參數 by Peggy
        /*
        string fileNameDat = aMLAFileRtnService.DownloadFromFTP(date, localPath, "dat");

        string fileNameCtl = aMLAFileRtnService.DownloadFromFTP(date, localPath, "ctl");
        */

        string fileNameDat = aMLAFileRtnService.DownloadFromFTP(date, localPath, "dat", ref isDownloadOK);

        string fileNameCtl = aMLAFileRtnService.DownloadFromFTP(date, localPath, "ctl", ref isDownloadOK);

        //20191230-RQ-2019-030155-002-批次信函調整：增加取得取檔結果的參數 by Peggy
        if (isDownloadOK)//檔案下載成功才繼續往下
        {
            // 取得A FILE回檔內容
            dat = aMLAFileRtnService.GetBranchFileData(localPath, fileNameDat, out errorMsg);

            total = dat.Rows.Count;

            errorMsg = SetErrorMsg(errorDatMsg, errorCtlMsg, errorMsg);

            string mailreason1 = "";
            string mailreason2 = "";
            //20200227 將ErrorCode拋出來
            string ErrorCode = string.Empty;
            // 20220411 修改說明:發送成功 email，若有 OMS 筆數，則內容需增加對應筆數的身份證字號 by Kelton 
            string TempCustomerID = string.Empty; //擷取並暫存每筆 OMS 資料的身份證字號

            if (errorMsg == "")
            {
                foreach (DataRow row in dat.Rows)
                {

                    if (row["ErrorCode"].Equals(""))
                    {

                        string ExceptionField = row["ExceptionField"].ToString();
                        string ExceptionReason = row["ExceptionReason"].ToString();
                        string SourceData = row["SourceData"].ToString();
                        string ExceptionReasonChi = "";


                        if (ExceptionReason.Equals("1"))
                        {
                            ExceptionReasonChi = "1. 資料必填未提供";

                        }
                        else if (ExceptionReason.Equals("2"))
                        {
                            ExceptionReasonChi = "2. 資料格式";

                        }
                        else if (ExceptionReason.Equals("3"))
                        {
                            ExceptionReasonChi = "3. 長度有誤";

                        }

                        else if (ExceptionReason.Equals("4"))
                        {
                            ExceptionReasonChi = "4. 非在清單內";

                        }

                        else if (ExceptionReason.Equals("5"))
                        {
                            ExceptionReasonChi = "5. 帳戶或關聯人所屬客戶ID+生日….不存在";

                        }
                        else if (ExceptionReason.Equals("6"))
                        {
                            ExceptionReasonChi = "6. 資料重複。";

                        }
                        else if (ExceptionReason.Equals("7"))
                        {
                            ExceptionReasonChi = "7.該客戶非舊戶，不可送U/O";

                        }
                        else if (ExceptionReason.Equals("8"))
                        {
                            ExceptionReasonChi = "8.該欄位不允許放$";

                        }
                        else if (ExceptionReason.Equals("9"))
                        {
                            ExceptionReasonChi = "9.新戶不允許必填欄位放$";

                        }
                        BRAML_File_Import.InsertAFileRtnLog("", ExceptionField, ExceptionReason, SourceData, fileNameDat, ExceptionReasonChi);

                        mailreason1 = "Y";

                        //20200227 計算OMS / CSIP剔退筆數
                        DataTable GetCtable = new DataTable();

                        //為取得剔退的案件編號，故用RMMBatchNo & AMLInternalID 回溯其對應的CASE_NO
                        GetCtable = BRAML_File_Import.GetEdataByRMMBatchNoandAMLInternalID(SourceData.Trim().Substring(0, 14), SourceData.Trim().Substring(14, 20));
                        if (GetCtable.Rows.Count > 0)
                        {
                            CSIPCount++;
                        }
                        else
                        {
                            OMSCount++;
                            // 20220411 修改說明:發送成功 email，若有 OMS 筆數，則內容需增加對應筆數的身份證字號 by Kelton 
                            TempCustomerID = SourceData.Trim().Substring(50, 10);
                            if (!string.IsNullOrEmpty(CustomerIDs))
                                CustomerIDs = CustomerIDs + "/";
                            CustomerIDs = CustomerIDs + string.Format("{0}***{1}", TempCustomerID.Substring(0, 4), TempCustomerID.Substring(7, 3));
                        }
                    }
                    else
                    {
                        ErrorCode = row["ErrorCode"].ToString().Trim();

                        BRAML_File_Import.InsertAFileRtnLog(ErrorCode, "", "", "", fileNameDat, "");

                        mailreason2 = "Y";
                    }
                }

                // 寫入資料庫
                isInsertOK = aMLAFileRtnService.SetRelationDataTable(dat, fileNameDat);

                if (mailreason1.Equals("Y"))
                {
                    JobHelper.Write(jobID, "取得結案AML AFILE回檔批次:" + fileNameDat + " 失敗！有出現A檔案_資料異常:請至AML_AFileImport查詢");
                }
                if (mailreason2.Equals("Y"))
                {
                    //20200227-RQ-2019-030155-003，區別是源自CSIP / OMS的剔退檔
                    if (ErrorCode.Trim().Equals("991"))
                    {
                        ErrorChi = " 【A檔案_資料異常(991-未收到檔案)】";
                    }
                    if (ErrorCode.Trim().Equals("992"))
                    {
                        ErrorChi = " 【A檔案_資料異常(992-CTRL檔的筆數跟實際資料檔的筆數不合)】";
                    }
                    //JobHelper.Write(jobID, "取得結案AML AFILE回檔批次:" + fileNameDat + "A檔案_資料異常(筆數不合或未收到檔案)");
                    JobHelper.Write(jobID, "取得結案AML AFILE回檔批次：" + fileNameDat + ErrorChi);
                }
            }
        }

        // 發送mail
        if (errorMsg != "" || !isInsertOK || !isDownloadOK)//20191230-RQ-2019-030155-002-批次信函調整 by Peggy
        {
            if (!isDownloadOK)//20191230-RQ-2019-030155-002-批次信函調整 by Peggy
            {
                //RMM OK A檔格式檢核失敗，未收到RMM OK A檔剔退
                InsertBatchLog(jobID, total, 0, "F", "檔案：" + fileNameDat + "不存在，FTP取檔失敗");
                aMLAFileRtnService.SendMail(_MailTitle + " 失敗！未收到RMM OK A檔剔退", _MailTitle + fileNameDat + " 失敗 ！未收到RMM OK A檔剔退", "失敗 ", this.StartTime);
            }
            else if (errorMsg.Trim().Equals("結案AML結案回檔無處理資料"))//20191227-AML系統回覆RMM OK A檔格式剔退成功！無筆數
            //if (errorMsg.Trim().Equals("結案AML結案回檔無處理資料"))
            {
                InsertBatchLog(jobID, total, total, "S", "");
                JobHelper.Write(jobID, "取得結案AML AFILE回檔 批次:" + fileNameDat + " 成功！總筆數:" + total + "，匯入成功共" + total + "筆，匯入失敗O筆", LogState.Info);
                aMLAFileRtnService.SendMail(_MailTitle + "成功，無筆數", "RMM OK A檔格式檢核：" + fileNameDat + " 成功！無筆數", "成功", this.StartTime);
            }
            else
            {
                InsertBatchLog(jobID, total, 0, "F", errorMsg);

                JobHelper.Write(jobID, "取得結案AML AFILE回檔 批次:" + fileNameDat + " 失敗！總筆數:" + total + errorMsg);

                // 發送 Email
                //20191227-RQ-2019-030155-002-批次信函調整 by Peggy：(2)RMM OK A檔格式檢核: AML_TW_RMMOK_A_CARD_YYYYMMDD.dat 失敗！共X筆，列示失敗的統編註明在MAIL內。
                //aMLAFileRtnService.SendMail("取得結案AML AFILE回檔 批次:" + fileNameDat + " 失敗！", "總筆數:" + total, errorMsg, this.StartTime);
                aMLAFileRtnService.SendMail(_MailTitle + "失敗，總筆數：" + total, "RMM OK A檔格式檢核：" + fileNameDat + " 失敗！總筆數：" + total, errorMsg, this.StartTime);
            }
        }
        else
        {
            

            // 發送 Email
            if (!string.IsNullOrEmpty(ErrorChi))
            {
                // 寫入 BatchLog
                InsertBatchLog(jobID, 0, 0, "F", ErrorChi);

                JobHelper.Write(jobID, "取得結案AML AFILE回檔 批次:" + fileNameDat + " 失敗！異常訊息：" + ErrorChi);

                aMLAFileRtnService.SendMail(_MailTitle + ErrorChi, "RMM OK A檔格式檢核：" + fileNameDat + ErrorChi, "異常", this.StartTime);
            }
            else
            {
                // 寫入 BatchLog
                InsertBatchLog(jobID, total, total, "S", "");

                JobHelper.Write(jobID, "取得結案AML AFILE回檔 批次:" + fileNameDat + " 成功！總筆數:" + total + "，匯入成功共" + total + "筆，匯入失敗O筆");

                // 20220411 修改說明:發送成功 email，若有 OMS 筆數，則內容需增加對應筆數的身份證字號 by Kelton 
                if (OMSCount > 0)
                    CustomerIDs = "\r\n\r\n         OMS案件：" + CustomerIDs;

                //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
                //aMLAFileRtnService.SendMail("取得結案AML AFILE回檔 批次:" + fileNameDat + " 成功！", "總筆數:" + total + "，匯入成功共" + total + "筆，匯入失敗O筆", errorMsg, this.StartTime);
                //20200227-RQ-2019-030155-003，區別是源自CSIP / OMS的剔退檔
                //aMLAFileRtnService.SendMail(_MailTitle + "成功，總筆數：" + total, "RMM OK A檔格式檢核：" + fileNameDat + " 成功！總筆數：" + total, "成功", this.StartTime);

                // 20220411 修改說明:發送成功 email，若有 OMS 筆數，則內容需增加對應筆數的身份證字號，故將原邏輯註解 Start by Kelton 
                //aMLAFileRtnService.SendMail(_MailTitle + "成功，總筆數：" + total + " 筆，CSIP：" + CSIPCount + " 筆，OMS：" + OMSCount + " 筆", "RMM OK A檔格式檢核：" + fileNameDat + " 成功！總筆數：" + total + " 筆，CSIP：" + CSIPCount + " 筆，OMS：" + OMSCount + " 筆", "成功", this.StartTime);
                // 20220411 修改說明:發送成功 email，若有 OMS 筆數，則內容需增加對應筆數的身份證字號，故將原邏輯註解 End by Kelton 
                // 20220411 修改說明:發送成功 email，若有 OMS 筆數，則內容需增加對應筆數的身份證字號 by Kelton 
                aMLAFileRtnService.SendMail(_MailTitle + "成功，總筆數：" + total + " 筆，CSIP：" + CSIPCount + " 筆，OMS：" + OMSCount + " 筆", 
                    "RMM OK A檔格式檢核：" + fileNameDat + " 成功！總筆數：" + total + " 筆，CSIP：" + CSIPCount + " 筆，OMS：" + OMSCount + " 筆" + CustomerIDs, "成功", this.StartTime);
            }
        }

        return fileNameDat;

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