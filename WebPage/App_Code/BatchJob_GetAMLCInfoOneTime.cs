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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Quartz;
using CSIPCommonModel.BusinessRules;
using Framework.Common.Message;
using System.Text;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Logging;
using Framework.Common.IO;
using Framework.Common.Utility;
using Framework.Data.OM.Transaction;

/// <summary>
/// BatchJob_GetAMLInformation 的摘要描述
/// </summary>
public class BatchJob_GetAMLCInfoOneTime : Quartz.IJob
{
    protected string FunctionKey = UtilHelper.GetAppSettings("FunctionKey").ToString();
    protected DateTime StartTime = DateTime.Now;// 記錄Job啟動時間
    protected DateTime EndTime;
    protected JobHelper JobHelper = new JobHelper();

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
            JobHelper.Write(jobID, "*********** " + jobID + " 取得 一次性AML C File資料資料 批次 START ************** ", LogState.Info);

            string date = DateTime.Now.ToString("yyyyMMdd");

            //date = "20190223";

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
            JobHelper.Write(jobID, "[FAIL]  取得 一次性AML C檔 資料 批次 " + fileName + " 發生錯誤：" + ex.Message);
            //20210618_Ares_Stanley-Exception時，將L_BATCH_LOG的Status設為"F"，避免Status保持在"R"之後無法運行批次
            BRL_BATCH_LOG.Delete("01", jobID, "R");
            BRL_BATCH_LOG.Insert("01", jobID, this.StartTime, "F", ex.Message);
            // 發送 Email
            aMLCInformationService.SendMail("AML 一次性C 檔 批次:" + fileName + " 失敗！", "總筆數:" + total, ex.Message, this.StartTime);
        }
        finally
        {
            // 20200031-CSIP EOS Ares Luke 修改日期:2021/03/12 修改說明:業務需求排程結束清空FilInfo參數
            // 清除 FileInfo Parameter值
            BRM_FileInfo.UpdateParam(jobID, "");

            JobHelper.Write(jobID, "  取得 AML 一次性C 檔資料 批次 Job 結束！", LogState.Info);
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
        bool isInsertOK = true;
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

        // 取 C 檔內容
        dat = aMLCInformationService.GetCFileData(localPath, fileNameDat, out errorMsg);

        // 組 AML_IMP_LOG 資料
        DataTable dtAMLIMPLOG = aMLCInformationService.SetAMLIMPLOGData(dat, fileNameDat);

        BRAML_File_Import.InsertAMLIMPLOG(dtAMLIMPLOG);
        total = dat.Rows.Count;

        BRAML_File_Import.RecoveryCdataimport();

        errorMsg = SetErrorMsg(errorDatMsg, errorCtlMsg, errorMsg);

        if (errorMsg == "")
        {
            // 20220112 大檔案匯入改成使用交易 by Ares Dennis start
            int pageSize = 3000;//每次交易的資料筆數
            int pageNum = 1;//第幾次交易
            int counter = 0;            

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

            //i is pageNum and start from 1
            for (int i = 1; i <= (total / pageSize) + 1; i++)
            {
                pageNum = i;

                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    //j is index of data and start from 0
                    for (int j = 0 + pageSize * (pageNum - 1); j < pageSize * pageNum - 1; j++)
                    {
                        if (j >= total)
                        {
                            break;
                        }
                        
                        isInsertOK = BRFORM_COLUMN.InsertCdatatableImport(dat.Rows[j], flag) && isInsertOK;
                        counter++;
                    }
                    ts.Complete();
                }
            }
            // 20220112 大檔案匯入改成使用交易 by Ares Dennis end
            
            if (isInsertOK)
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    //刪除重複資料
                    isInsertOK = BRAML_File_Import.Recoverycdataimport() && isInsertOK;
                    ts.Complete();
                }                    
            }
            
            // 還原資料
            if (!isInsertOK)
            {
                BRAML_File_Import.RecoveryCData(fileNameDat);
            }
        }


        // 發送失敗 mail
        if (errorMsg != "" || !isInsertOK)
        {

            BRAML_File_Import.RecoveryClogData(fileNameDat);

            // 寫入 BatchLog
            InsertBatchLog(jobID, total, 0, "F", errorMsg);

            JobHelper.Write("BatchJob_GetAMLCInfo", "一次性AML C 檔 批次:" + fileNameDat + " 失敗！總筆數:" + total);
            
            // 發送 Email
            aMLCInformationService.SendMail("一次性AML C 檔 批次:" + fileNameDat + " 失敗！", "總筆數:" + total, errorMsg, this.StartTime);
        }
        else
        {
            // 寫入 BatchLog
            InsertBatchLog(jobID, total, total, "S", "");

            JobHelper.Write("BatchJob_GetAMLCInfo", "一次性AML C 檔 批次:" + fileNameDat + "總筆數:" + total + "，匯入成功共" + total + "筆，匯入失敗O筆", LogState.Info);

            // 發送 Email
            aMLCInformationService.SendMail("一次性AML C 檔 批次:" + fileNameDat + " 成功！", "總筆數:" + total + "，匯入成功共" + total + "筆，匯入失敗O筆", errorMsg, this.StartTime);
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
