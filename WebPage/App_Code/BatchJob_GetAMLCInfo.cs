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
/// BatchJob_GetAMLCInfo 的摘要描述
/// 接收AMLC DATA資料
/// 檔名：AML_TW_C_yyyyMMdd_UTF8
/// </summary>
public class BatchJob_GetAMLCInfo : Quartz.IJob
{
    protected string FunctionKey = UtilHelper.GetAppSettings("FunctionKey").ToString();
    protected DateTime StartTime = DateTime.Now;// 記錄Job啟動時間
    protected DateTime EndTime;
    protected JobHelper JobHelper = new JobHelper();
    protected string _MailTitle = "接收AML C 檔 批次：";//20191227-RQ-2019-030155-002-批次信函調整 by Peggy
    protected string _AMLMailTitle = "傳送AML C 檔OK檔至AML結果 批次：";//20210222
    bool isComplete = false;//20210222
    protected string AMLfileName = string.Empty;//20210222

    public void Execute(JobExecutionContext context)
    {
        string jobID = context.JobDetail.JobDataMap["jobid"].ToString();
        JobHelper.strJobID = jobID;
        string fileName = "";
        bool isContinue = true;
        string msgID = string.Empty;
        int total = 0;

        AMLCInformationService aMLCInformationService = new AMLCInformationService(jobID);

        try
        {
            JobHelper.Write(jobID, "*********** " + jobID + " 取得 AML C File資料資料 批次 START ************** ", LogState.Info);

           

            string date = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");

            //date = "20190222";

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


                DownloadFileAndInsertTable(jobID, aMLCInformationService, date, out total);
            }
        }
        catch (Exception ex)
        {
            JobHelper.Write(jobID, "[FAIL]  取得 AML C檔 資料 批次 " + fileName + " 發生錯誤：" + ex.Message);
            //20210618_Ares_Stanley-Exception時，將L_BATCH_LOG的Status設為"F"，避免Status保持在"R"之後無法運行批次
            BRL_BATCH_LOG.Delete("01", jobID, "R");
            BRL_BATCH_LOG.Insert("01", jobID, this.StartTime, "F", ex.Message);
            // 發送 Email
            //aMLCInformationService.SendMail("AML C 檔 批次:" + fileName + " 失敗！", "總筆數:" + total, ex.Message, this.StartTime);
            aMLCInformationService.SendMail(_MailTitle + "失敗！總筆數：" + total + " 筆", _MailTitle + fileName + " 失敗！總筆數：" + total, ex.Message, this.StartTime);
        }
        finally
        {
            // 20200031-CSIP EOS Ares Luke 修改日期:2021/03/12 修改說明:業務需求排程結束清空FilInfo參數
            // 清除 FileInfo Parameter值
            BRM_FileInfo.UpdateParam(jobID, "");

            JobHelper.Write(jobID, "  取得 AML C 檔資料 批次 Job 結束！", LogState.Info);
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

    private string DownloadFileAndInsertTable(string jobID, AMLCInformationService aMLCInformationService, string date, out int total)
    {

        string folderName = string.Empty;
        string errorDatMsg = "";
        string errorCtlMsg = "";
        string errorMsg = "";
        bool isInsertOK = false;
        DataTable dat = new DataTable();
        total = 0;
        bool isDownloadOK = false; //20191230-RQ-2019-030155-002-批次信函調整：增加取得取檔結果的參數 by Peggy

        JobHelper.CreateFolderName(jobID, ref folderName);

        string localPath = AppDomain.CurrentDomain.BaseDirectory + "FileDownload\\" + jobID + "\\" + folderName + "\\";

        //20191230-RQ-2019-030155-002-批次信函調整：增加取得取檔結果的參數 by Peggy
        /*
        string fileNameDat = aMLCInformationService.DownloadFromFTP(date, localPath, "dat");

        string fileNameCtl = aMLCInformationService.DownloadFromFTP(date, localPath, "ctl");
        */
        string fileNameDat = aMLCInformationService.DownloadFromFTP(date, localPath, "dat", ref isDownloadOK);

        string fileNameCtl = aMLCInformationService.DownloadFromFTP(date, localPath, "ctl", ref isDownloadOK);

        //20191230-RQ-2019-030155-002-批次信函調整：增加取得取檔結果的參數 by Peggy
        if (isDownloadOK)//檔案下載成功才繼續往下
        {
            // 取 C 檔內容
            dat = aMLCInformationService.GetCFileData(localPath, fileNameDat, out errorMsg);

            // 組 AML_IMP_LOG 資料
            DataTable dtAMLIMPLOG = aMLCInformationService.SetAMLIMPLOGData(dat, fileNameDat);

            BRAML_File_Import.InsertAMLIMPLOG(dtAMLIMPLOG);
            JobHelper.Write(jobID, "InsertAMLIMPLOG 結束！", LogState.Info);
            total = dat.Rows.Count;

            BRAML_File_Import.RecoveryCdataimport();
            JobHelper.Write(jobID, "RecoveryCdataimport 結束！", LogState.Info);

            errorMsg = SetErrorMsg(errorDatMsg, errorCtlMsg, errorMsg);

            if (errorMsg == "")
            {

                // 20210527 EOS_AML(NOVA) by Ares Dennis
                #region 退版機制
                DataTable dt2 = new DataTable();
                CSIPCommonModel.BusinessRules_new.BRM_PROPERTY_CODE.GetEnableProperty("01", new string[] { "OLD_VERSION_FLAG" }, ref dt2);
                string flag = "";
                if (dt2.Rows.Count > 0)
                {
                    flag = dt2.Rows[0]["PROPERTY_CODE"].ToString();
                }
                #endregion

                foreach (DataRow row in dat.Rows)
                {
                    string strCustomerID = row["CustomerID"].ToString();
                    DataTable GetCtable = new DataTable();

                    //JobHelper.Write("insertinto", "insert=" + strCustomerID);
                    isInsertOK = BRFORM_COLUMN.InsertCdatatableImport(row, flag);
                    /*
                    GetCtable = BRFORM_COLUMN.GetCdataByCustomerID(strCustomerID);

                    JobHelper.Write("BatchJob_GetAMLCInfo", "SEARCHDATA=" + strCustomerID);
                    //若原本的c檔資料庫就有資料就用update的方式。
                    if (GetCtable.Rows.Count >= 1)
                    {

                        JobHelper.Write("BatchJob_GetAMLCInfo", "UPDATE=" + strCustomerID);

                        isInsertOK = BRFORM_COLUMN.UpdateCdatatable(row);
                    }
                    else //沒有資料insert到 cdata table
                    {
                        JobHelper.Write("BatchJob_GetAMLCInfo", "INSERT=" + strCustomerID);

                        isInsertOK =  BRFORM_COLUMN.InsertCdatatable(row);
                    }
                    */

                }
                JobHelper.Write(jobID, "InsertCdatatableImport 結束！", LogState.Info);

                isInsertOK = BRAML_File_Import.Recoverycdataimport();
                JobHelper.Write(jobID, "Recoverycdataimport 結束！", LogState.Info);

                // 還原資料
                if (!isInsertOK)
                {
                    BRAML_File_Import.RecoveryCData(fileNameDat);
                }
                else//20210222-RQ-2021-004136-001 因AML稽核議題，若收到AML C檔後，要回OK檔給AML 
                {
                    AMLfileName = @"AML_TW_C_OK_CSIP_" + DateTime.Now.ToString("yyyyMMdd") + ".dat";
                    isComplete = GenaratorFileAndUploadToMFTP("SendToAMLCInfoOK", AMLfileName, localPath);//上傳至AML FOLDER
                }
            }
        }
        
        // 發送失敗 mail
        if (errorMsg != "" || !isInsertOK || !isDownloadOK)//20191230-RQ-2019-030155-002-批次信函調整 by Peggy
        {
            if (!isDownloadOK)//20191230-RQ-2019-030155-002-批次信函調整 by Peggy
            {
                InsertBatchLog(jobID, total, 0, "F", "檔案：" + fileNameDat + "不存在，FTP取檔失敗");
                aMLCInformationService.SendMail("AML C 檔批次失敗，未收到Ｃ檔", _MailTitle + fileNameDat + " 失敗 ！未收到Ｃ檔", "失敗 ", this.StartTime);
            }
            else
            {
                BRAML_File_Import.RecoveryClogData(fileNameDat);

                // 寫入 BatchLog
                InsertBatchLog(jobID, total, 0, "F", errorMsg);

                JobHelper.Write("BatchJob_GetAMLCInfo", "AML C 檔 批次:" + fileNameDat + " 失敗！總筆數:" + total);
                // 發送 Email
                //aMLCInformationService.SendMail("AML C 檔 批次:" + fileNameDat + " 失敗！", "總筆數:" + total, errorMsg, this.StartTime);
                aMLCInformationService.SendMail(_MailTitle + "失敗！總筆數：" + total + " 筆", _MailTitle + fileNameDat + " 失敗！總筆數：" + total, errorMsg, this.StartTime);
            }
        }
        else
        {
            // 寫入 BatchLog
            InsertBatchLog(jobID, total, total, "S", "");

            JobHelper.Write("BatchJob_GetAMLCInfo", "AML C 檔 批次:" + fileNameDat + "總筆數:" + total + "，匯入成功共" + total + "筆，匯入失敗O筆", LogState.Info);

            // 發送 Email
            //aMLCInformationService.SendMail("AML C 檔 批次:" + fileNameDat + " 成功！", "總筆數:" + total + "，匯入成功共" + total + "筆，匯入失敗O筆", errorMsg, this.StartTime);
            aMLCInformationService.SendMail(_MailTitle + "成功！總筆數：" + total + " 筆", "總筆數：" + total + "筆，匯入成功共 " + total + " 筆，匯入失敗 O 筆", _MailTitle + fileNameDat + " 成功！", this.StartTime);
            if(isComplete)
                aMLCInformationService.SendMail(_AMLMailTitle + "上傳成功", _AMLMailTitle + AMLfileName + "上傳成功", " 成功！", this.StartTime);//20210222
            else
                aMLCInformationService.SendMail(_AMLMailTitle + "上傳失敗", _AMLMailTitle + AMLfileName + "上傳失敗", " 失敗！", this.StartTime);//20210222
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

    //20210222-RQ-2021-004136-001
    // 產檔及上傳到 MFTP FOR 拋送AML C檔OK
    private bool GenaratorFileAndUploadToMFTP(string jobID, string fileName, string localPath)
    {
        DataTable localFileInfo = new DataTable();
        StringBuilder sb = new StringBuilder();
        string pwd = string.Empty;
        bool isUploadToFTP = true;
        bool isComplete = false;
        PostAMLCaseService postAMLCaseService = new PostAMLCaseService(jobID);
        
        //檔案內容：空檔
        string postDataContent = string.Empty;

        sb.Append(postDataContent);


        try
        {
            // 寫入檔案
            postAMLCaseService.CreateFile(localPath, fileName, sb.ToString());

            // 上送FTP
            isUploadToFTP = postAMLCaseService.UploadFileToMFTP(jobID, localFileInfo, localPath, fileName);

            if (isUploadToFTP)
            {
                JobHelper.Write(jobID, "FileName：" + fileName + "，上送FTP 成功",LogState.Info); //20210524_Ares_Stanley-變更LOG層級為Info
                isComplete = true;
            }
            else
            {
                JobHelper.Write(jobID, "[FAIL] FileName：" + fileName + "，上送FTP 失敗");
                isComplete = false;
            }
        }
        catch (Exception e)
        {
            JobHelper.Write(jobID, "[FAIL] FileName：" + fileName + "，錯誤訊息：" + e.Message);
            isComplete = false;
        }

        return isComplete;

    }
}
