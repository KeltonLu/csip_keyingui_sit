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
/// BatchJob_GetBranchModifyRtnInfo 的摘要描述
/// 收取分公司異動檔結果
/// 檔名：AML_UPD_BR_R_yyyyMMdd
/// </summary>
public class BatchJob_GetBranchModifyRtnInfo : Quartz.IJob
{
    protected string FunctionKey = UtilHelper.GetAppSettings("FunctionKey").ToString();
    protected DateTime StartTime = DateTime.Now;// 記錄Job啟動時間
    protected DateTime EndTime;
    protected JobHelper JobHelper = new JobHelper();
    protected string _MailTitle = "主機回覆分公司異動檔結果 批次：";//20191229-RQ-2019-030155-002-批次信函調整 by Peggy

    public void Execute(JobExecutionContext context)
    {
        string jobID = context.JobDetail.JobDataMap["jobid"].ToString();
        JobHelper.strJobID = jobID;
        string fileName = "";
        bool isContinue = true;
        string msgID = string.Empty;
        int total = 0;

        AMLBranchModifyInfoRtnService aMLBranchModifyInfoRtnService = new AMLBranchModifyInfoRtnService(jobID);

        try
        {
            JobHelper.Write(jobID, "*********** " + jobID + " 取得 分公司異動回檔是否ok 資料 批次 START ************** ", LogState.Info);

            
            string date = DateTime.Now.ToString("yyyyMMdd");

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


                DownloadFileAndInsertTable(jobID, aMLBranchModifyInfoRtnService, date, out total);
            }
        }
        catch (Exception ex)
        {
            JobHelper.Write(jobID, "[FAIL]  取得 分公司異動回檔是否ok 批次 " + fileName + " 發生錯誤：" + ex.Message);
            //20210618_Ares_Stanley-Exception時，將L_BATCH_LOG的Status設為"F"，避免Status保持在"R"之後無法運行批次
            BRL_BATCH_LOG.Delete("01", jobID, "R");
            BRL_BATCH_LOG.Insert("01", jobID, this.StartTime, "F", ex.Message);
            // 發送 Email
            //20191230-RQ-2019-030155-002-批次信函調整 by Peggy
            //aMLBranchModifyInfoRtnService.SendMail("分公司異動回檔是否ok 批次:" + fileName + " 失敗！", "總筆數:" + total, ex.Message, this.StartTime);
            aMLBranchModifyInfoRtnService.SendMail(_MailTitle + fileName + " 失敗！", "發生錯誤：" + ex.Message, "失敗", this.StartTime);
        }
        finally
        {
            // 20200031-CSIP EOS Ares Luke 修改日期:2021/03/12 修改說明:業務需求排程結束清空FilInfo參數
            // 清除 FileInfo Parameter值
            BRM_FileInfo.UpdateParam(jobID, "");

            JobHelper.Write(jobID, "  取得 分公司異動回檔是否ok 批次 Job 結束！", LogState.Info);
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

    private string DownloadFileAndInsertTable(string jobID, AMLBranchModifyInfoRtnService aMLBranchModifyInfoRtnService, string date, out int total)
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
         string fileNameDat = aMLBranchModifyInfoRtnService.DownloadFromFTP(date, localPath, "dat");

        string fileNameCtl = aMLBranchModifyInfoRtnService.DownloadFromFTP(date, localPath, "ctl");
         */
        string fileNameDat = aMLBranchModifyInfoRtnService.DownloadFromFTP(date, localPath, "dat", ref isDownloadOK);

        string fileNameCtl = aMLBranchModifyInfoRtnService.DownloadFromFTP(date, localPath, "ctl", ref isDownloadOK);

        //20191230-RQ-2019-030155-002-批次信函調整：增加取得取檔結果的參數 by Peggy
        if (isDownloadOK)//檔案下載成功才繼續往下
        {
            // 取 Branch 檔內容
            dat = aMLBranchModifyInfoRtnService.GetBranchFileData(localPath, fileNameDat, out errorMsg);

            total = dat.Rows.Count;

            errorMsg = SetErrorMsg(errorDatMsg, errorCtlMsg, errorMsg);

            if (errorMsg == "")
            {

                foreach (DataRow row in dat.Rows)
                {
                    string BRCH_BATCH_NO = row["BRCH_BATCH_NO"].ToString();
                    string BRCH_INTER_ID = row["BRCH_INTER_ID"].ToString();
                    string BRCO_BRCH_NO = row["BRCO_BRCH_NO"].ToString();
                    string BRCO_BRCH_SEQ = row["BRCO_BRCH_SEQ"].ToString();
                    string BRCO_RETURN_CODE = row["BRCO_RETURN_CODE"].ToString();

                    //parse出來就可以寫資料庫
                    //正常:00
                    //先查詢此BRCH_NO與BRCH_SEQ屬於哪個CASE NO

                    DataTable datbrch_work = new DataTable();
                    datbrch_work = BRFORM_COLUMN.GetBRCH_WORKDATAByBATCH_NOINTERNAL_ID(BRCH_BATCH_NO, BRCH_INTER_ID);
                    string case_no = "";
                    string BRCH_BRCH_NO = "";

                    if (datbrch_work.Rows.Count > 0)
                    {
                        foreach (DataRow brch_workrow in datbrch_work.Rows)
                        {
                            case_no = brch_workrow["CASE_NO"].ToString();
                            BRCH_BRCH_NO = brch_workrow["BRCH_BRCH_NO"].ToString();
                        }
                    }
                    //用case_no查詢 notelog 裡面有沒有 若沒有insert 若有update

                    if (BRCO_RETURN_CODE.Equals("00"))
                    {
                        BRAML_File_Import.InsertCaseRtnLog2(case_no, BRCH_BRCH_NO, "RtnOK");

                        BRAML_File_Import.AML_CASE_ACT_LOG_Rtn(BRCH_BATCH_NO, BRCH_INTER_ID, BRCO_BRCH_NO, BRCO_BRCH_SEQ, BRCO_RETURN_CODE);
                    }
                    else
                    {
                        BRAML_File_Import.InsertCaseRtnLog2(case_no, BRCH_BRCH_NO, "RtnFail");

                        BRAML_File_Import.AML_CASE_ACT_LOG_Rtn(BRCH_BATCH_NO, BRCH_INTER_ID, BRCO_BRCH_NO, BRCO_BRCH_SEQ, BRCO_RETURN_CODE);

                    }
                }
                // 寫入資料庫
                isInsertOK = aMLBranchModifyInfoRtnService.SetRelationDataTable(dat, fileNameDat);
            }
        }
        // 發送失敗 mail
        if (errorMsg != "" || !isInsertOK || !isDownloadOK)//20191230-RQ-2019-030155-002-批次信函調整 by Peggy
        {
            if (!isDownloadOK)//20191230-RQ-2019-030155-002-批次信函調整 by Peggy
            {
                //取得主機分公司異動結果批次失敗，未收到檔案
                InsertBatchLog(jobID, total, 0, "F", "檔案：" + fileNameDat + "不存在，FTP取檔失敗");
                aMLBranchModifyInfoRtnService.SendMail(_MailTitle + " 失敗！未收到檔案", _MailTitle + fileNameDat + " 失敗 ！未收到檔案", "失敗 ", this.StartTime);
            }
            else if (errorMsg.Equals("主機回覆分公司結案ok檔無處理資料"))
            //if (errorMsg.Equals("主機回覆分公司結案ok檔無處理資料"))//20191230-RQ-2019-030155-002-批次信函調整 by Peggy
            {
                string startdate = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
                string enddate = DateTime.Now.ToString("yyyyMMdd");
                int queryBrchexport = BRAML_File_Import.queryBrchExportNumber(startdate, enddate);
                if (queryBrchexport != 0)
                {
                    InsertBatchLog(jobID, total, 0, "F", errorMsg);

                    JobHelper.Write(jobID, "取得結案主機分公司回檔 檔 批次:" + fileNameDat + " 失敗 !應該要有" + queryBrchexport + "件,但主機分公司回應結案檔只有:" + total + errorMsg);

                    // 發送 Email
                    //20191230-RQ-2019-030155-002-批次信函調整 by Peggy
                    //aMLBranchModifyInfoRtnService.SendMail("取得結案主機分公司回檔 檔 批次:" + fileNameDat + " 失敗 !應該要有" + queryBrchexport + "件,但主機分公司回應結案檔只有:" + total, "總筆數:" + total, errorMsg, this.StartTime);
                    aMLBranchModifyInfoRtnService.SendMail(_MailTitle + " 失敗！", _MailTitle + fileNameDat + " 失敗 ！總筆數：" + total + " 筆，應匯入 " + queryBrchexport + " 件(主機合併周五/周六/周日的結案在次周一回覆)", errorMsg, this.StartTime);
                }
                else
                {
                    // 20200031-CSIP EOS Ares Rick 修改日期:2021/04/12 修改說明:增加成功狀態回寫
                    InsertBatchLog(jobID, total, total, "S", "");
                    JobHelper.Write(jobID, "取得結案主機分公司回檔 檔 批次:" + fileNameDat + " 成功 !總筆數：" + total + "，匯入成功共:" + total + "筆，匯入失敗0筆");
                    aMLBranchModifyInfoRtnService.SendMail(_MailTitle + " 成功！主機回覆分公司結案ok檔無處理資料", _MailTitle + fileNameDat + " 成功!總筆數：" + total + "筆", "成功", this.StartTime);
                }
            }
            else
            {
                InsertBatchLog(jobID, total, 0, "F", errorMsg);

                JobHelper.Write(jobID, "取得主機分公司回覆異動 檔 批次:" + fileNameDat + " 失敗！總筆數:" + total + errorMsg);
                // 發送 Email
                //20191230-RQ-2019-030155-002-批次信函調整 by Peggy
                //aMLBranchModifyInfoRtnService.SendMail("取得主機分公司回覆異動 檔 批次:" + fileNameDat + " 失敗！", "總筆數:" + total, errorMsg, this.StartTime);
                aMLBranchModifyInfoRtnService.SendMail(_MailTitle + " 失敗！", _MailTitle + fileNameDat + " 失敗 ！總筆數：" + total + " 筆(主機合併周五/周六/周日的結案在次周一回覆)", errorMsg, this.StartTime);
            }
        }
        else
        {
            // 寫入 BatchLog
            InsertBatchLog(jobID, total, total, "S", "");

            JobHelper.Write(jobID, "取得主機分公司回覆異動 檔 批次:" + fileNameDat + " 成功！總筆數:" + total + "，匯入成功共" + total + "筆，匯入失敗O筆", LogState.Info);

            // 發送 Email
            //20191230-RQ-2019-030155-002-批次信函調整 by Peggy
            //aMLBranchModifyInfoRtnService.SendMail("取得主機分公司回覆異動 檔 批次:" + fileNameDat + " 成功！", "總筆數:" + total + "，匯入成功共" + total + "筆，匯入失敗O筆", errorMsg, this.StartTime);
            aMLBranchModifyInfoRtnService.SendMail(_MailTitle + " 成功！總筆數：" + total + " 筆，匯入成功共 " + total + "筆，匯入失敗O筆", _MailTitle + fileNameDat + " 成功 ！總筆數：" + total + " 筆，匯入成功共 " + total + "筆，匯入失敗O筆 (主機合併周五/周六/周日的結案在次周一回覆)", "成功", this.StartTime);
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