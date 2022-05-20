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
/// BatchJob_GetAMLCaseRtnInfo 的摘要描述
/// 收取AML結案回檔
/// </summary>
public class BatchJob_GetAMLCaseRtnInfo : Quartz.IJob
{
    protected string FunctionKey = UtilHelper.GetAppSettings("FunctionKey").ToString();
    protected DateTime StartTime = DateTime.Now;// 記錄Job啟動時間
    protected DateTime EndTime;
    protected JobHelper JobHelper = new JobHelper();
    //20191031 修改：mail內容不分開發送 by Peggy
    string _Msg = string.Empty;
    string _MailFeedback = string.Empty;
    protected string _MailTitle = "AML系統回覆RMM OK F檔結果：";//20191227-RQ-2019-030155-002-批次信函調整 by Peggy

    public void Execute(JobExecutionContext context)
    {
        string jobID = context.JobDetail.JobDataMap["jobid"].ToString();
        JobHelper.strJobID = jobID;
        string fileName = "";
        bool isContinue = true;
        string msgID = string.Empty;
        int total = 0;
        
        AMLCaseRtnServiceFromAML aMLCaseRtnService = new AMLCaseRtnServiceFromAML(jobID);

        try
        {
            JobHelper.Write(jobID, "*********** " + jobID + " 取得 結案AML結案回檔 資料 批次 START ************** ", LogState.Info);

            
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


                DownloadFileAndInsertTable(jobID, aMLCaseRtnService, date, out total);
            }
        }
        catch (Exception ex)
        {
            JobHelper.Write(jobID, "[FAIL]  取得 結案AML結案回檔 資料 批次 " + fileName + " 發生錯誤：" + ex.Message);
            //20210618_Ares_Stanley-Exception時，將L_BATCH_LOG的Status設為"F"，避免Status保持在"R"之後無法運行批次
            BRL_BATCH_LOG.Delete("01", jobID, "R");
            BRL_BATCH_LOG.Insert("01", jobID, this.StartTime, "F", ex.Message);
            // 發送 Email
            //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
            //aMLCaseRtnService.SendMail("結案AML結案回檔 檔 批次:" + fileName + " 失敗！", "總筆數:" + total, ex.Message, this.StartTime);
            aMLCaseRtnService.SendMail(_MailTitle + "失敗，總筆數：" + total + "筆", _MailTitle + fileName + " 失敗！" + ex.Message, "失敗", this.StartTime);
        }
        finally
        {
            // 20200031-CSIP EOS Ares Luke 修改日期:2021/03/12 修改說明:業務需求排程結束清空FilInfo參數
            // 清除 FileInfo Parameter值
            BRM_FileInfo.UpdateParam(jobID, "");

            JobHelper.Write(jobID, "  取得 結案AML結案回檔 檔資料 批次 Job 結束！", LogState.Info);
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

    private string DownloadFileAndInsertTable(string jobID, AMLCaseRtnServiceFromAML aMLCaseRtnService, string date, out int total)
    {
        string folderName = string.Empty;
        string errorDatMsg = "";
        string errorCtlMsg = "";
        string errorMsg = "";
        bool isInsertOK = false;
        DataTable dat = new DataTable();
        total = 0;
        //20200218-RQ-2019-030155-003，區別是源自CSIP / OMS的剔退檔
        int OMSCount = 0;//記錄OMS剔退筆數
        int CSIPCount = 0;//記錄CSIP剔退筆數

        bool isDownloadOK = false; //20191230-RQ-2019-030155-002-批次信函調整：增加取得取檔結果的參數 by Peggy

        JobHelper.CreateFolderName(jobID, ref folderName);

        string localPath = AppDomain.CurrentDomain.BaseDirectory + "FileDownload\\" + jobID + "\\" + folderName + "\\";

        //20191230-RQ-2019-030155-002-批次信函調整：增加取得取檔結果的參數 by Peggy
        /*
        string fileNameDat = aMLCaseRtnService.DownloadFromFTP(date, localPath, "dat");

        string fileNameCtl = aMLCaseRtnService.DownloadFromFTP(date, localPath, "ctl");
        */

        string fileNameDat = aMLCaseRtnService.DownloadFromFTP(date, localPath, "dat", ref isDownloadOK);

        string fileNameCtl = aMLCaseRtnService.DownloadFromFTP(date, localPath, "ctl", ref isDownloadOK);

        //20191230-RQ-2019-030155-002-批次信函調整：增加取得取檔結果的參數 by Peggy
        if (isDownloadOK)//檔案下載成功才繼續往下
        {
            // 取得結案回檔內容
            dat = aMLCaseRtnService.GetBranchFileData(localPath, fileNameDat, out errorMsg);

            total = dat.Rows.Count;

            errorMsg = SetErrorMsg(errorDatMsg, errorCtlMsg, errorMsg);


            string file01caseno = "";
            string file02caseno = "";

            string file01 = "";
            string file02 = "";

            //20200218-RQ-2019-030155-003，區別是源自CSIP / OMS的剔退檔
            string OMSFile = "";//記錄若E檔找不到CASE_NO，則判定為OMS的結案剔退

            if (errorMsg == "")
            {
                foreach (DataRow row in dat.Rows)
                {

                    string RMMBatchNo = row["RMMBatchNo"].ToString();
                    string AMLInternalID = row["AMLInternalID"].ToString();
                    string SourceSystem = row["SourceSystem"].ToString();
                    string DataDate = row["DataDate"].ToString();
                    string CustomerID = row["CustomerID"].ToString();
                    string retuncode = row["retuncode"].ToString();

                    //parse出來就可以寫資料庫
                    //正常:00
                    DataTable GetCtable = new DataTable();

                    //為取得剔退的案件編號，故用RMMBatchNo & AMLInternalID 回溯其對應的CASE_NO
                    GetCtable = BRAML_File_Import.GetEdataByRMMBatchNoandAMLInternalID(RMMBatchNo, AMLInternalID);

                    if (GetCtable.Rows.Count > 0)
                    {
                        CSIPCount++;
                        foreach (DataRow rowDATA in GetCtable.Rows)
                        {

                            string case_no = rowDATA["CASE_NO"].ToString(); ;

                            //01:有Pending CDC
                            //02:其他


                            if (retuncode.Equals("01"))
                            {
                                //20200821-10月RC-2020/8/20 (週四) 下午 08:29 MAIL提及『請調整收到RMM OK的F檔不論剔退原因為何，都不用異動RMM OK產檔flag』
                                //BRAML_File_Import.updateAML_ExportFileFlag(case_no);
                                //將結果insert到notelog
                                BRAML_File_Import.InsertCaseRtnLog(case_no, "AMLRtnFail");
                                //insert AML_CASE_ACT_LOG_Rtn
                                BRAML_File_Import.AML_CASE_ACT_LOG_RtnAML(RMMBatchNo, AMLInternalID, "", "", retuncode);

                                //組資料讓file01發信
                                file01 = "Y";
                                file01caseno = file01caseno + case_no + "|";


                            }//還是異常
                            else if (retuncode.Equals("02"))
                            {
                                //將結果insert到notelog
                                BRAML_File_Import.InsertCaseRtnLog(case_no, "AMLRtnFail");
                                //insert AML_CASE_ACT_LOG_Rtn
                                BRAML_File_Import.AML_CASE_ACT_LOG_RtnAML(RMMBatchNo, AMLInternalID, "", "", retuncode);

                                //組資料讓file01發信
                                file02 = "Y";
                                file02caseno = file02caseno + case_no + "|";
                            }
                        }//foreach 結束
                    }
                    else //20200218-RQ-2019-030155-003
                    {
                        //統計OMS資料
                        OMSFile = "Y";
                        OMSCount++;
                    } 
                }//踢退的datatable foreach 結束

                //20191031-RQ-2018-015749-002 Mail通知信件一起發，此段僅組mail內容字串
                if (file01.Equals("Y"))
                {
                    //20191031-RQ-2018-015749-002 mark by Peggy
                    //aMLCaseRtnService.SendMail("取得結案AML結案回檔 請先完成CDC案件 ！", "案件編號:" + file01caseno, errorMsg, this.StartTime);
                    //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
                    //_MailFeedback = "案件編號：" + file01caseno + "結案異常， 請先完成CDC案件 ！";
                    _MailFeedback += "案件編號：" + file01caseno + "結案異常，請完成pending的CDC ！";
                }

                //20200115-RQ-2019-030155-002 因一個檔案裡，return code 可能有01、02同時存在
                //else if (file02.Equals("Y"))
                if (file02.Equals("Y"))
                {
                    //20191031-RQ-2018-015749-002 mark by Peggy
                    //aMLCaseRtnService.SendMail("取得結案AML結案回檔 其他原因，請洽維護IT！", "案件編號:" + file02caseno, errorMsg, this.StartTime);
                    _MailFeedback += "案件編號：" + file02caseno + "結案異常，其他原因，請洽維護IT！";
                }

                if (OMSFile.Trim().Equals("Y"))
                {
                    _MailFeedback += "OMS：" + OMSCount + " 筆！";
                }

                // 寫入資料庫
                isInsertOK = aMLCaseRtnService.SetRelationDataTable(dat, fileNameDat);

            }//errMSG結束
        }

        // 發送失敗 mail
        if (errorMsg != "" || !isInsertOK || !isDownloadOK)//20191230-RQ-2019-030155-002-批次信函調整 by Peggy
        {
            if (!isDownloadOK)//20191230-RQ-2019-030155-002-批次信函調整 by Peggy
            {
                //未收到檔案：RMM OK結案批次成功，無剔退資料
                InsertBatchLog(jobID, total, 0, "F", "AML系統回覆RMM OK F檔 批次：" + fileNameDat + "失敗，未收到檔案");
                aMLCaseRtnService.SendMail(_MailTitle + " 失敗！未收到檔案", _MailTitle + fileNameDat + " 失敗 ！未收到檔案，維護IT洽AML IT", "失敗 ", this.StartTime);
            }
            else if (errorMsg.Equals("結案AML結案回檔無處理資料"))
            {
                string startdate = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
                string enddate = DateTime.Now.ToString("yyyyMMdd");
                int querycaseexport = BRAML_File_Import.queryCaseExportNumber(startdate, enddate);
                if (querycaseexport != 0)
                {
                    InsertBatchLog(jobID, total, 0, "S", errorMsg);

                    //20191031  modify by Peggy
                    _Msg = _MailFeedback.Trim();

                    //20191031-RQ-2018-015749-002 調整mail/log 訊息內容 by Peggy
                    //JobHelper.Write(jobID, "取得結案AML結案回檔 檔 批次:" + fileNameDat + " 失敗 !應該要有" + querycaseexport + "件,但AML結案回應結案檔只有:" + total + errorMsg);
                    JobHelper.Write(jobID, "AML系統回覆RMM OK F檔 批次:" + fileNameDat + " 成功 !" + _Msg + errorMsg, LogState.Info);

                    // 發送 Email
                    //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
                    //aMLCaseRtnService.SendMail("取得結案AML結案回檔 檔 批次:" + fileNameDat + " 失敗 !應該要有" + querycaseexport + "件,但AML結案回應結案檔只有:" + total, "總筆數:" + total, errorMsg, this.StartTime);
                    //aMLCaseRtnService.SendMail("AML系統回覆RMM OK檔結果 批次:" + fileNameDat + " 失敗 !", _Msg.Trim(), errorMsg, this.StartTime);
                    aMLCaseRtnService.SendMail(_MailTitle + "成功！無0049剔退", "RMM OK 結案結果批次：" + fileNameDat + " 成功 ！無0049剔退" + _Msg.Trim(), errorMsg, this.StartTime);

                }
            }
            else
            {
                InsertBatchLog(jobID, total, 0, "F", errorMsg);

                //20191031-RQ-2018-015749-002  modify by Peggy
                _Msg = _MailFeedback.Trim();

                //JobHelper.Write(jobID, "取得結案AML結案回檔 檔 批次:" + fileNameDat + " 失敗！總筆數:" + total + errorMsg);
                JobHelper.Write(jobID, "AML系統回覆RMM OK檔結果 批次:" + fileNameDat + " 失敗！" + _Msg + errorMsg);

                // 發送 Email
                //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
                //aMLCaseRtnService.SendMail("取得結案AML結案回檔 檔 批次:" + fileNameDat + " 失敗！", "總筆數:" + total, errorMsg, this.StartTime);
                //aMLCaseRtnService.SendMail("AML系統回覆RMM OK檔結果 批次:" + fileNameDat + " 失敗！"Ｓ, _Msg.Trim(), errorMsg, this.StartTime);
                //20200218-RQ-2019-030155-003 優化-區別CSIP /OMS失敗筆數
                //aMLCaseRtnService.SendMail(_MailTitle + "失敗！總筆數：" + total + "筆 ", "RMM OK 結案結果批次：" + fileNameDat + " 失敗！失敗筆數" + total + " 筆，" + _Msg.Trim(), errorMsg, this.StartTime);
                aMLCaseRtnService.SendMail(_MailTitle + "失敗！總筆數：" + total + " 筆，CSIP：" + CSIPCount + " 筆，OMS：" + OMSCount + " 筆", "RMM OK 結案結果批次：" + fileNameDat + " 失敗！失敗筆數：CSIP：" + CSIPCount + " 筆，" + _Msg.Trim(), errorMsg, this.StartTime);
            }
        }        
        else //非CSIP的剔退檔時顯示
        {
            //20200115
            //狀況為，有踢退資料，無檢核錯誤訊息，也下載、insert成功時
            if (!_MailFeedback.Trim().Equals(""))
            {
                //寫入 BatchLog
                InsertBatchLog(jobID, total, 0, "F", errorMsg);

                _Msg = _MailFeedback.Trim();
                JobHelper.Write(jobID, "AML系統回覆RMM OK檔結果 批次:" + fileNameDat + " 失敗！" + _Msg + errorMsg);
                //20200218-RQ-2019-030155-003 優化-區別CSIP /OMS失敗筆數
                //aMLCaseRtnService.SendMail(_MailTitle + "失敗！總筆數：" + total + "筆", "RMM OK 結案結果批次：" + fileNameDat + " 失敗 ！失敗筆數" + total + " 筆，" + _Msg.Trim(), "失敗", this.StartTime);
                aMLCaseRtnService.SendMail(_MailTitle + "失敗！總筆數：" + total + " 筆，CSIP：" + CSIPCount + " 筆，OMS：" + OMSCount + " 筆", "RMM OK 結案結果批次：" + fileNameDat + " 失敗 ！失敗筆數：CSIP：" + CSIPCount + " 筆，" + _Msg.Trim(), "失敗", this.StartTime);
            }
            else
            {
                //寫入 BatchLog
                InsertBatchLog(jobID, total, total, "S", "");

                //20191031  modify by Peggy
                //_Msg = "E檔總筆數：" + total + "，0049 匯入成功共" + total + "筆，匯入失敗O筆 ";
                _Msg = "RMM OK結案批次成功，無0049剔退";

                // 發送 Email
                //20191031-RQ-2018-015749-002 MODIFY BY Peggy
                //JobHelper.Write(jobID, "取得結案AML結案回檔 檔 批次:" + fileNameDat + " 成功！總筆數:" + total + "，匯入成功共" + total + "筆，匯入失敗O筆");
                //aMLCaseRtnService.SendMail("取得結案AML結案回檔 檔 批次:" + fileNameDat + " 成功！", "總筆數:" + total + "，匯入成功共" + total + "筆，匯入失敗O筆", errorMsg, this.StartTime);
                JobHelper.Write(jobID, _Msg.Trim(), LogState.Info);
                //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
                //aMLCaseRtnService.SendMail("AML系統回覆RMM OK檔結果 批次:" + fileNameDat + " 成功！", _Msg.Trim(), errorMsg, this.StartTime);
                aMLCaseRtnService.SendMail(_MailTitle + "成功，無0049剔退", _Msg.Trim(), "成功", this.StartTime);
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