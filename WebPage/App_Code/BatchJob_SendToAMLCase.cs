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
using System.IO;
using Framework.Common.Logging;
using Framework.Common.Utility;
using System.Collections.Generic;//20200409-為使用LIST加入的參考

/// <summary>
/// BatchJob_SendToAMLCase 的摘要描述
/// </summary>
public class BatchJob_SendToAMLCase : Quartz.IJob
{
    protected string FunctionKey = UtilHelper.GetAppSettings("FunctionKey").ToString();
    protected DateTime StartTime = DateTime.Now;// 記錄Job啟動時間
    protected DateTime EndTime;
    protected JobHelper JobHelper = new JobHelper();

    //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
    protected string _MailTitle = "結案檔傳送至主機結果 批次：";
    protected string _AMLMailTitle = "RMM OK結案檔傳送到AML結果 批次：";
    protected int CombineOMSCount = 0;//20200106-計算OMS合併檔筆數

    //20200409-RQ-2019-030155-005 ↓
    protected List<string> listDat = new List<string>();//記錄OMS需合併的DATA檔名
    protected List<string> listCtl = new List<string>();//記錄OMS需合併的CONTROL檔名
    protected string ReSend = string.Empty;//是否重送FLAG，為補跑320~325資料而設定，下次變更可移除
    protected DataTable postDataToMainFrame = new DataTable();//取得補跑3/20-3/25的結案檔更新主機資料
    //20200409-RQ-2019-030155-005 ↑

    public void Execute(JobExecutionContext context)
    {
        int total = 0;
        int success = 0;
        string jobID = context.JobDetail.JobDataMap["jobid"].ToString();
        JobHelper.strJobID = jobID;
        string fileName = "";
        bool isContinue = true;
        string msgID = string.Empty;
        PostAMLCaseService postAMLCaseService = new PostAMLCaseService(jobID);
        int sendToAMLCount = 0;//20200106-計算拋送AML的結案檔筆數
        string AMLfileName = string.Empty;//記錄拋送AML的檔名

        try
        {
            DataTable postData = new DataTable();
            DataTable postDataToAML = new DataTable();
            DateTime dateTime = new DateTime();
            string startTime = "";
            string endTime = "";
            bool isComplete = false;
            string strdateTime = DateTime.Now.ToString("yyyyMMdd");
            string exeDate = DateTime.Now.ToString("yyyyMMdd");

            DataTable tblFileInfo = BRAML_File_Import.GetFileInfo(jobID);
            //增加判斷如果Parameter有值，則使用Parameter的時間  by Peggy
            //如果Parameter有值，則使用Parameter的時間
            if (!string.IsNullOrEmpty(tblFileInfo.Rows[0]["Parameter"].ToString()))
            {
                exeDate = tblFileInfo.Rows[0]["Parameter"].ToString().Trim();
            }
            //20200414-RQ-2019-030155-005-因批次改為非營業日改拋空檔，故需判斷執行日當天是否為營業日 ↓
            bool IsWorkingDate = BRFORM_COLUMN.IsWorkingDate(exeDate.Trim());//判斷執行日當天是否為營業日
            string _LastDate = BRFORM_COLUMN.GetLastWorkingDate(exeDate.Trim());//取得上一工作日
            DataTable datOMScase = new DataTable();//存放需抓OMS結案檔的日期數
            //20200414-RQ-2019-030155-005-因批次改為非營業日改拋空檔，故需判斷執行日當天是否為營業日 ↑ 

            JobHelper.Write(jobID, "*********** " + jobID + " 傳給主機結案案件 批次 START ************** ", LogState.Info);

            // 判斷Job工作狀態(0:停止 1:運行)
            isContinue = CheckJobIsContinue(jobID, ref msgID);

            if (isContinue)
            {

                // 算起迄時間
                postAMLCaseService.GetSearchTime(this.StartTime, out startTime, out endTime);
                endTime = DateTime.Now.ToString("yyyyMMdd");
                dateTime = Convert.ToDateTime(endTime.Substring(0, 4) + "-" + endTime.Substring(4, 2) + "-" + endTime.Substring(6, 2));

                if (IsWorkingDate)//20200416-RQ-2019-030155-005 判斷如為營業才往下撈取結案檔資料，反之則拋以空檔
                {
                    // 取結過案的資料( to 主機)
                    postData = postAMLCaseService.GetSendAMLCASE("");

                    //20191120-RQ-2018-015749-002-to AML的案件，要排除解除不合作與案件重審的案件
                    postDataToAML = postAMLCaseService.GetSendAMLCASE("A");//TO AML

                    //20200409-RQ-2019-030155-005 抓取要重送主機資料
                    ReSend = DateTime.Now.ToString("yyyyMMdd").Equals("20200515") ? "Y" : "N"; //僅在執行日為5/15那天才會ON成Y執行
                    if (ReSend.Equals("Y"))
                    {
                        postDataToMainFrame = BRFORM_COLUMN.GetCaseToMainframeData_OneTime();
                    }

                    //OMSCasedataStart

                    string folderName = string.Empty;

                    JobHelper.CreateFolderName("GetOMSCaseFile", ref folderName);

                    string localPath = AppDomain.CurrentDomain.BaseDirectory + "FileDownload\\GetOMSCaseFile\\" + folderName + "\\";

                    /*20200415-RQ-2019-030155-005-因非營業日送空檔問題，變成第一個上班日要抓假日的結案檔資料，故以動態日期抓檔案
                    string fileNameDat = postAMLCaseService.DownloadFromFTP(strdateTime, localPath, "dat");

                    string fileNameCtl = postAMLCaseService.DownloadFromFTP(strdateTime, localPath, "ctl");

                    string filenamedatpath = localPath + fileNameDat;
                    string filenamectlpath = localPath + fileNameCtl;

                    if (fileNameDat.Equals("") || fileNameCtl.Equals(""))
                    {
                        filenamedatpath = "";
                        filenamectlpath = "";
                    }
                    */
                    //20200415-RQ-2019-030155-005-因非營業日送空檔問題，變成第一個上班日要抓假日的結案檔資料，故以動態日期抓檔案
                    DateTime _dt1 = DateTime.ParseExact(_LastDate.Trim(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                    _LastDate = _dt1.AddDays(1).ToString("yyyyMMdd");
                    //2020-04-15-RQ-2019-030155-005 取得上一營業日與執行日之間，所需讀取檔案數
                    datOMScase = BRFORM_COLUMN.GetWork_Date(_LastDate.Trim(), exeDate.Trim());//取得檔案數
                    JobHelper.Write(jobID, "取得OMS合併檔案數，日期： '" + _LastDate.Trim() + "'  ~ '" + exeDate.Trim() + "'", LogState.Info);

                    foreach (DataRow dr in datOMScase.Rows)
                    {
                        string fileNameDat = string.Empty;
                        string fileNameCtl = string.Empty;

                        fileNameDat = postAMLCaseService.DownloadFromFTP(dr["DATE_TIME"].ToString().Trim(), localPath, "dat");
                        fileNameCtl = postAMLCaseService.DownloadFromFTP(dr["DATE_TIME"].ToString().Trim(), localPath, "ctl");

                        if (!fileNameDat.Trim().Equals(""))
                        {
                            listDat.Add(localPath + fileNameDat);
                            JobHelper.Write(jobID, "取得OMS合併檔，DAT檔名： '" + fileNameDat.Trim() + "'", LogState.Info);
                        }
                        if (!fileNameCtl.Trim().Equals(""))
                        {
                            listCtl.Add(localPath + fileNameCtl);
                            JobHelper.Write(jobID, "取得OMS合併檔，CTL檔名： '" + fileNameDat.Trim() + "'", LogState.Info);
                        }
                    }
                    JobHelper.Write(jobID, "取得OMS合併檔案數： '" + listCtl.Count + "'  ， '" + listDat.Count + "'", LogState.Info);
                }
                //根據資料組成檔案，並且透過GetOMSCaseFile的設定抓取OMS檔案進行合併上傳

                // 20210527 EOS_AML(NOVA) by Ares Dennis
                //AMLfileName = @"AML_TW_RMMOK_CARD_" + strdateTime + ".DAT";
                // 20210915 調整檔案名稱 by Ares Stanley
                DataTable tblReailFileInfo = BRAML_File_Import.GetFileInfo("PostToRealAMLCase");
                AMLfileName = tblReailFileInfo.Rows[0]["FtpFileName"].ToString().Replace("yyyyMMdd", strdateTime);

                //20191120-RQ-2018-015749-002-to AML的案件，要排除解除不合作與案件重審的案件
                //isComplete = GenaratorFileAndUploadToMFTP("PostToRealAMLCase", @"AML_TW_RMMOK_CARD_" + strdateTime + ".dat", postData, postAMLCaseService, dateTime, "AML", filenamedatpath, filenamectlpath);
                //20200415-RQ-2019-030155-005-區隔AML/主機結案檔產檔內容
                //isComplete = GenaratorFileAndUploadToMFTP("PostToRealAMLCase", @"AML_TW_RMMOK_CARD_" + strdateTime + ".dat", postDataToAML, postAMLCaseService, dateTime, "AML", filenamedatpath, filenamectlpath);
                isComplete = GenaratorFileAndUploadToMFTP_new("PostToRealAMLCase", AMLfileName + ".dat", postDataToAML, postAMLCaseService, dateTime, "AML");
                if (isComplete)
                {
                    //20191120-RQ-2018-015749-002-to AML的案件，要排除解除不合作與案件重審的案件
                    //isComplete = GenaratorFileAndUploadToMFTP2("PostToRealAMLCase", @"AML_TW_RMMOK_CARD_" + strdateTime + ".ctl", postData, postAMLCaseService, dateTime, filenamectlpath);
                    //20200415-RQ-2019-030155-005-區隔AML/主機結案檔產檔內容
                    //isComplete = GenaratorFileAndUploadToMFTP2("PostToRealAMLCase", @"AML_TW_RMMOK_CARD_" + strdateTime + ".ctl", postDataToAML, postAMLCaseService, dateTime, filenamectlpath);
                    isComplete = GenaratorFileAndUploadToMFTP2_new("PostToRealAMLCase", AMLfileName + ".ctl", postDataToAML, postAMLCaseService);
                    sendToAMLCount = postDataToAML.Rows.Count;
                    if (sendToAMLCount > 0)
                    {
                        JobHelper.Write(jobID, " RMM OK結案檔傳送到AML結果 批次成功：OK檔" + AMLfileName + ".DAT" + "檔案，共計" + sendToAMLCount + "筆數，上傳成功！", LogState.Info);
                        //20200413-RMM OK檔批次MAIL通知要抓RMM的收件者
                        postAMLCaseService.SendMail("PostToRealAMLCase", _AMLMailTitle + "成功，總筆數：" + sendToAMLCount + "筆", _AMLMailTitle + AMLfileName + ".DAT" + " 成功，總筆數：" + sendToAMLCount + "筆", "成功", this.StartTime);
                    }
                    else
                    {
                        JobHelper.Write(jobID, " RMM OK結案檔傳送到AML結果 批次成功，今日無任何需轉出OK檔資料！", LogState.Info);
                        //20200413-RMM OK檔批次MAIL通知要抓RMM的收件者
                        postAMLCaseService.SendMail("PostToRealAMLCase", _AMLMailTitle + "成功，今日無任何需轉出OK檔資料！", _AMLMailTitle + AMLfileName + ".DAT" + " 成功，總筆數：" + sendToAMLCount + "筆", "成功", this.StartTime);
                    }
                }
                else
                {
                    JobHelper.Write(jobID, " RMM OK結案檔傳送到AML結果 批次失敗：OK檔" + AMLfileName + ".DAT" + "檔案，共計" + sendToAMLCount + "筆數，上傳失敗！");
                    //20200413-RMM OK檔批次MAIL通知要抓RMM的收件者
                    postAMLCaseService.SendMail("PostToRealAMLCase", _AMLMailTitle + "失敗，總筆數：" + sendToAMLCount + "筆，次日批次再補傳", _AMLMailTitle + AMLfileName + ".DAT" + " 上傳失敗，總筆數：" + sendToAMLCount + "筆，次日批次再補傳", "失敗", this.StartTime);
                }

                //若不合併OMS使用下列兩條指令。

                //isComplete = GenaratorFileAndUploadToMFTP("PostToRealAMLCase", @"AML_TW_RMMOK_CARD_" + strdateTime + ".dat", postData, postAMLCaseService, dateTime, "AML", "","" );
                //isComplete = GenaratorFileAndUploadToMFTP2("PostToRealAMLCase", @"AML_TW_RMMOK_CARD_" + strdateTime + ".ctl", postData, postAMLCaseService, dateTime, "");
                
                //這部分是傳給主機的，主機檔案不用合併

                if (isComplete)
                {
                    //OMSCASEdataEnd
                    // 20210915 調整檔案名稱 by Ares Stanley
                    //fileName = @"AML_UPD_CLO_" + strdateTime + ".txt";
                    fileName = tblFileInfo.Rows[0]["FtpFileName"].ToString().Replace("yyyyMMdd", strdateTime) + ".txt";
                    // 產檔及上傳到 MFTP
                    //20200416 -RQ-2019-030155-005-因需補跑3/20~3/25的資料，故將上傳AML區分開來
                    //isComplete = GenaratorFileAndUploadToMFTP(jobID, fileName, postData, postAMLCaseService, dateTime, "", "", "");
                    isComplete = GenaratorFileAndUploadToMFTP(jobID, fileName, postData, postAMLCaseService);
                    if (ReSend.Trim().Equals("Y"))
                    {
                        //總共轉出幾筆
                        success = postData.Rows.Count + postDataToMainFrame.Rows.Count;
                    }
                    else
                        //總共轉出幾筆
                        success = postData.Rows.Count;
                }

                if (isComplete)
                {
                    foreach (DataRow row in postData.Rows)
                    {
                        if (postData.Rows.Count > 0)
                        {
                            //儲存LOG檔案
                            BRFORM_COLUMN.UpdateHQworktable(row["ID"].ToString());
                            BRFORM_COLUMN.AML_CASE_ACT_LOG(row["CASE_NO"].ToString(), "", "", "拋送結案檔", "拋送結案檔完成");
                            BRFORM_COLUMN.AML_NOTELOG(row["CASE_NO"].ToString(), row["CustomerID"].ToString(), "", "拋送結案檔", "拋送結案檔完成");
                        }
                    }
                    if (postDataToAML.Rows.Count > 0)//20200414-如遇異常結案僅需拋送AML時，也要更新AML_HQ_WORK的發送FLAG
                    {
                        bool isSucc = false;
                        foreach (DataRow dr in postDataToAML.Rows)
                        {
                            isSucc = false;
                            //儲存LOG檔案
                            JobHelper.Write(jobID, "拋送AML結案檔ID：" + dr["ID"].ToString(), LogState.Info);
                            isSucc = BRFORM_COLUMN.UpdateHQworktable(dr["ID"].ToString());
                            JobHelper.Write(jobID, "拋送AML結案檔更新HQ Table是否成功：" + isSucc, LogState.Info);

                            BRFORM_COLUMN.AML_CASE_ACT_LOG(dr["CASE_NO"].ToString(), "", "", "拋送AML結案檔", "拋送AML結案檔完成");
                            BRFORM_COLUMN.AML_NOTELOG(dr["CASE_NO"].ToString(), dr["CustomerID"].ToString(), "", "拋送AML結案檔", "拋送AML結案檔完成");
                        }
                    }
                    
                    total = postData.Rows.Count;

                    if (total > 0)
                    {
                        // 寫入 BatchLog
                        InsertBatchLog(jobID, total, success, "S", "");

                        // 發送 Email
                        //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
                        JobHelper.Write(jobID, " 案件結案檔 轉出JOB執行成功：OK檔" + fileName + "檔案，共計" + total + "筆數，上傳成功！", LogState.Info);
                        //postAMLCaseService.SendMail(jobID, "案件結案檔 轉出JOB執行成功：OK檔" + fileName + "檔案，共計" + total + "筆數，上傳成功！", "總筆數:" + total, "上傳成功", this.StartTime);                        
                        postAMLCaseService.SendMail(jobID, _MailTitle + "成功，總筆數：" + total + "筆", "結案檔上傳主機批次成功，" + fileName + " 檔案，總筆數：" + total + "筆", "成功", this.StartTime);
                    }
                    else
                    {
                        // 寫入 BatchLog
                        InsertBatchLog(jobID, total, success, "S", "");

                        // 發送 Email
                        //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
                        JobHelper.Write(jobID, "案件結案檔 轉出JOB執行成功：今日無任何需轉出OK檔資料！", LogState.Info);
                        //postAMLCaseService.SendMail(jobID, "案件結案檔 轉出JOB執行成功：今日無任何需轉出OK檔資料！", "", "成功", this.StartTime);
                        postAMLCaseService.SendMail(jobID, _MailTitle + "成功，總筆數：" + total + "筆", "結案檔上傳主機批次成功，今日無任何需轉出OK檔資料！", "成功", this.StartTime);

                    }
                    JobHelper.Write(jobID, "[SUCCESS] 傳給主機結案案件 批次 END " + fileName, LogState.Info);
                }
                else
                {
                    // 寫入 BatchLog
                    InsertBatchLog(jobID, total, success, "F", "");

                    //20191227-RQ-2019-030155-002-批次信函調整 by Peggy                    
                    //postAMLCaseService.SendMail(jobID, "案件結案檔 尚未轉出成功請檢查BatchJob_SendToAMLCase log檔案", "", "BatchJob_SendToAMLCase尚未轉出成功", this.StartTime);
                    postAMLCaseService.SendMail(jobID, _MailTitle + "失敗，總筆數：" + total + "筆，次日批次再補傳", "檔案尚未轉出成功請檢查Batch Log檔案", "失敗", this.StartTime);
                }
            }
        }
        catch (Exception ex)
        {
            // 寫入 BatchLog
            InsertBatchLog(jobID, total, success, "F", "發生錯誤：" + ex.Message);

            // 發送 Email
            //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
            JobHelper.Write(jobID, "[FAIL] 案件結案檔 傳給主機結案案件 批次 " + fileName + " 發生錯誤：" + ex.Message);
            //postAMLCaseService.SendMail(jobID, "案件結案檔 轉出JOB執行失敗：OK檔" + fileName + "檔案，共計" + total + "筆數，上傳失敗，無法完成。請改由人工上傳，並請於晚間11點前重送！", ex.Message, "上傳失敗", this.StartTime);
            postAMLCaseService.SendMail(jobID, _MailTitle + "失敗，總筆數：" + total + "筆，次日批次再補傳", _MailTitle + fileName + " 上傳失敗，無法完成。請改由人工上傳，並請於晚間11點前重送！", "失敗", this.StartTime);


        }
        finally
        {
            JobHelper.Write(jobID, " 傳給主機結案案件 批次 Job 結束！", LogState.Info);
            JobHelper.Write(jobID, "================================================================================================================================================", LogState.Info);
        }
    }

    #region 手動執行，20200416 mark it
    /*
    /// <summary>
    /// 手動執行
    /// </summary>
    /// <param name="jobID"></param>
    /// <param name="endTime"></param>
    /// <returns></returns>
    public bool ExecuteManual(string jobID, string endTime)
    {
        DateTime dateTime = Convert.ToDateTime(endTime.Substring(0, 4) + "-" + endTime.Substring(4, 2) + "-" + endTime.Substring(6, 2));
        string fileName = @"AML_UPD_CLO_R_" + endTime + ".DAT";
        PostAMLCaseService postAMLCaseService = new PostAMLCaseService(jobID);
        int total = 0;
        int success = 0;
        bool result = false;
        try
        {
            DataTable postData = new DataTable();
            bool isComplete = false;

            JobHelper.Write(jobID, "*********** " + jobID + " 傳給主機結案案件 手動 START ************** ", LogState.Info);

            // 取郵局核印資料
            postData = postAMLCaseService.GetSendAMLCASE("");

            // 產檔及上傳到 MFTP
            isComplete = GenaratorFileAndUploadToMFTP(jobID, fileName, postData, postAMLCaseService, dateTime,"","","");

            if (isComplete)
            {
                total = postData.Rows.Count;

                // 寫入 BatchLog
                InsertBatchLog(jobID, total, success, "S", "");

                // 發送 Email
                postAMLCaseService.SendMail(jobID, "案件結案檔 手動:" + fileName + " 上傳成功！", "總筆數:" + total, "上傳成功", this.StartTime);
            }
            else
            {
                // 寫入 BatchLog
                InsertBatchLog(jobID, total, success, "S", "");

                // 發送 Email
                postAMLCaseService.SendMail(jobID, "案件結案檔 手動:" + fileName + " 成功！", "無資料", "成功", this.StartTime);
            }

            result = true;

            JobHelper.Write(jobID, "[SUCCESS] 案件結案檔 手動 END " + fileName, LogState.Info);

        }
        catch (Exception ex)
        {
            // 發送 Email
            postAMLCaseService.SendMail(jobID, "案件結案檔 手動:" + fileName + " 上傳失敗！", ex.Message, "上傳失敗", this.StartTime);

            JobHelper.Write(jobID, "[FAIL] 案件結案檔 手動 " + fileName + " 發生錯誤：" + ex.Message);
        }
        finally
        {
            JobHelper.Write(jobID, " 案件結案檔 手動 Job 結束！", LogState.Info);
            JobHelper.Write(jobID, "================================================================================================================================================", LogState.Info);
        }

        return result;
    }
    */
    #endregion
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


    // 產檔及上傳到 MFTP FOR 拋送主機用
    private bool GenaratorFileAndUploadToMFTP(string jobID, string fileName, DataTable postData, PostAMLCaseService postAMLCaseService)
    {
        string path = string.Empty;
        DataTable localFileInfo = new DataTable();
        StringBuilder sb = new StringBuilder();
        string pwd = string.Empty;
        bool isUploadToFTP = true;
        bool isComplete = false;
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

                postDataContent = CHT_WordPadLeftRight(row["RMMBatchNo"].ToString(), "L", 14, ' ');   // 1. RMM批號 (來自AML原始檔案)
                postDataContent += CHT_WordPadLeftRight(row["AMLInternalID"].ToString(), "L", 20, ' ');   // 2. AML內部使用的編號 (來自AML原始檔案)
                postDataContent += CHT_WordPadLeftRight(row["SourceSystem"].ToString(), "L", 8, ' ');   // 3. 前端系統 (固定值為 'CARD')
                postDataContent += CHT_WordPadLeftRight(row["DataDate"].ToString(), "L", 8, ' ');   // 4. 資料日期
                //給主機要加總部序號，不然他們說他們對應不到
                postDataContent += CHT_WordPadLeftRight(row["CustomerID"].ToString() + row["corpseq"].ToString(), "L", 14, ' ');   // 5. 客戶ID

                //20191218-應姵晴MAIL(2019/12/18 (週三) 上午 11:47)要求，案件先全送C給主機 by Peggy
                //MAIL內容：法遵公告12月新案有排除不合作的高風險因子，但不適用12月前建案的，為讓AML可以重算正確的風險等級，
                // 麻煩您調整結案後送主機的AML審查FLAG都送C，謝謝。
                /*20191218 MARK
                 //20191127-RQ-2018-015749-002 解除不合作的案件(解除不合作流程：當日日期8碼+9+5碼流水號)，送主機時欄位請帶"C"
                 //postDataContent += CHT_WordPadLeftRight("Y", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
                 if (!string.IsNullOrEmpty(row["CASE_NO"].ToString()) && row["CASE_NO"].ToString().Trim().Substring(8, 1).Equals("9"))
                 {
                     postDataContent += CHT_WordPadLeftRight("C", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)}
                 }
                 else
                 {
                     postDataContent += CHT_WordPadLeftRight("Y", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
                 }
                 */

                //20200409-RQ-2019-030155-005：為區分是否報送AML或僅需帶CC至AML，增加FLAG：Z，並將送主機的FLAG改塑C
                //20200318--應姵晴MAIL(2020/03/18 (週三) 下午 16:14)要求,，2020/3/27 RC新增需求_傳送主機結案檔案的AML審查FLAG都送Y
                /*
                 //20200114-RQ-2019-030155-002
                if (systemtype.Equals("AML"))//送AML時，請維持送Y，AML系統根據此欄位做結案
                {
                    postDataContent += CHT_WordPadLeftRight("Y", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
                }
                else//送主機時，審查FLAG送C
                {
                    postDataContent += CHT_WordPadLeftRight("C", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)}
                }
                */
                //20200409-RQ-2019-030155-005：為區分是否報送AML或僅需帶CC至AML，增加FLAG：Z，並將送主機的FLAG改送Z
                //postDataContent += CHT_WordPadLeftRight("Y", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
                //20200602-RQ-2019-030155-000：解除不合作的結案要送主機C，由主機異動送AML審查Y到AML產RMD再重算下次審查日
                //postDataContent += CHT_WordPadLeftRight("Z", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
                if (!string.IsNullOrEmpty(row["CASE_NO"].ToString()) && row["CASE_NO"].ToString().Trim().Substring(8, 1).Equals("9"))
                {
                    postDataContent += CHT_WordPadLeftRight("C", "L", 1, ' ');   // 6. RMM完成 
                }
                //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
                //else if (row["CaseProcess_Status"].ToString() == "23")
                //{
                //    postDataContent += CHT_WordPadLeftRight("X", "L", 1, ' '); // 20210527 EOS_AML(NOVA) 新增不合作欄位註記狀態 by Ares Dennis
                //}
                //20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis
                else
                {
                    postDataContent += CHT_WordPadLeftRight("Z", "L", 1, ' ');   // 6. RMM完成
                }
                postDataContent += CHT_WordPadLeftRight(row["LastUpdateMaker"].ToString(), "L", 12, ' ');   // 7. 資料最後異動Maker
                postDataContent += CHT_WordPadLeftRight(row["LastUpdateChecker"].ToString(), "L", 12, ' ');   // 8. 資料最後異動Checker
                postDataContent += CHT_WordPadLeftRight(row["LastUpdateBranch"].ToString(), "L", 4, ' ');   // 9. 資料最後異動分行
                postDataContent += CHT_WordPadLeftRight(Convert.ToDateTime(row["LastUpdateDate"]).ToString("yyyyMMdd"), "L", 8, ' ');   // 10. 資料最後異動日期

                if (i != rowcountnum)
                {
                    postDataContent += "\n";
                }

                sb.Append(postDataContent);
            }
        }


        //將重送的檔案合併到原本的
        #region ReSend = Y,重送3/20~3/25的資料至主機
        if (ReSend.Trim().Equals("Y"))
        {
            if (postDataToMainFrame.Rows.Count > 0)
            {
                JobHelper.Write(jobID, "進行3/20~3/25 結案檔檔案合併作業", LogState.Info);

                string postDataContent = string.Empty;

                if (sb.ToString() != null && !sb.ToString().Equals(""))
                {
                    postDataContent = "\n";
                    sb.Append(postDataContent);
                }

                int rowcountnum = postDataToMainFrame.Rows.Count;
                int i = 0;
                foreach (DataRow row in postDataToMainFrame.Rows)
                {
                    i = i + 1;

                    postDataContent = string.Empty;
                    postDataContent = CHT_WordPadLeftRight(row["RMMBatchNo"].ToString(), "L", 14, ' ');   // 1. 1 資料別          (固定值為1)
                    postDataContent += CHT_WordPadLeftRight(row["AMLInternalID"].ToString(), "L", 20, ' ');   // 1. 1 資料別          (固定值為1)
                    postDataContent += CHT_WordPadLeftRight(row["SourceSystem"].ToString(), "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
                    postDataContent += CHT_WordPadLeftRight(row["DataDate"].ToString(), "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
                    //給主機要加總部序號，不然他們說他們對應不到
                    postDataContent += CHT_WordPadLeftRight(row["CustomerID"].ToString() + row["corpseq"].ToString(), "L", 14, ' ');   // 1. 1 資料別          (固定值為1)

                    postDataContent += CHT_WordPadLeftRight("Y", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
                    postDataContent += CHT_WordPadLeftRight(row["LastUpdateMaker"].ToString(), "L", 12, ' ');   // 1. 1 資料別          (固定值為1)
                    postDataContent += CHT_WordPadLeftRight(row["LastUpdateChecker"].ToString(), "L", 12, ' ');   // 1. 1 資料別          (固定值為1)
                    postDataContent += CHT_WordPadLeftRight(row["LastUpdateBranch"].ToString(), "L", 4, ' ');   // 1. 1 資料別          (固定值為1)
                    postDataContent += CHT_WordPadLeftRight(Convert.ToDateTime(row["LastUpdateDate"]).ToString("yyyyMMdd"), "L", 8, ' ');   // 1. 1 資料別          (固定值為1)

                    if (i != rowcountnum)
                    {
                        postDataContent += "\n";
                    }

                    sb.Append(postDataContent);
                }
                JobHelper.Write(jobID, "進行3/20~3/25 結案檔檔案合併作業，筆數：" + rowcountnum.ToString(), LogState.Info);
            }            
        }
        #endregion

        try
        {
            // 寫入檔案
            postAMLCaseService.CreateFile(path, fileName, sb.ToString());

            // 上送FTP
            isUploadToFTP = postAMLCaseService.UploadFileToMFTP(jobID, localFileInfo, path, fileName);

            if (isUploadToFTP)
            {
                JobHelper.Write(jobID, "FileName：" + fileName + "，上送FTP 成功", LogState.Info);
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
    #region 20200416 備份原始GenaratorFileAndUploadToMFTP程式
    //private bool GenaratorFileAndUploadToMFTP(string jobID, string fileName, DataTable postData, PostAMLCaseService postAMLCaseService, DateTime dateTime, string systemtype, string OMSCASEfilename, string OMSCASECtlfilename)
    //{

    //    string path = string.Empty;
    //    DataTable localFileInfo = new DataTable();
    //    StringBuilder sb = new StringBuilder();
    //    //DataTable sb = new DataTable();
    //    string pwd = string.Empty;
    //    bool isUploadToFTP = true;
    //    bool isComplete = false;
    //    bool isCompleteOMS = true;
    //    // 取得上傳檔案存放路徑
    //    path = GetLocalFilePath(jobID);

    //    string postDataContentb = string.Empty;
    //    string postDataContentc = string.Empty;

    //    if (postData.Rows.Count == 0)
    //    {

    //        string postDataContent = string.Empty;
    //        /*
    //        postDataContent += CHT_WordPadLeftRight("", "L", 14, ' ');   // 1. 1 資料別          (固定值為1)
    //        postDataContent += CHT_WordPadLeftRight("", "L", 20, ' ');   // 1. 1 資料別          (固定值為1)
    //        postDataContent += CHT_WordPadLeftRight("", "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
    //        postDataContent += CHT_WordPadLeftRight("", "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
    //        postDataContent += CHT_WordPadLeftRight("", "L", 14, ' ');   // 1. 1 資料別          (固定值為1)
    //        postDataContent += CHT_WordPadLeftRight("", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
    //        postDataContent += CHT_WordPadLeftRight("", "L", 12, ' ');   // 1. 1 資料別          (固定值為1)
    //        postDataContent += CHT_WordPadLeftRight("", "L", 12, ' ');   // 1. 1 資料別          (固定值為1)
    //        postDataContent += CHT_WordPadLeftRight("", "L", 4, ' ');   // 1. 1 資料別          (固定值為1)
    //        postDataContent += CHT_WordPadLeftRight("", "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
    //        postDataContent += "\r\n";
    //        */
    //        sb.Append(postDataContent);
    //    }
    //    else
    //    {

    //        int rowcountnum = postData.Rows.Count;
    //        int i = 0;
    //        foreach (DataRow row in postData.Rows)
    //        {
    //            i = i + 1;
    //            string postDataContent = string.Empty;

    //            postDataContent = CHT_WordPadLeftRight(row["RMMBatchNo"].ToString(), "L", 14, ' ');   // 1. 1 資料別          (固定值為1)
    //            postDataContent += CHT_WordPadLeftRight(row["AMLInternalID"].ToString(), "L", 20, ' ');   // 1. 1 資料別          (固定值為1)
    //            postDataContent += CHT_WordPadLeftRight(row["SourceSystem"].ToString(), "L", 8, ' ');   // 1. 1 資料別          (固定值為1)

    //            if (systemtype.Equals("AML"))
    //            {
    //                postDataContent += CHT_WordPadLeftRight(DateTime.Now.ToString("yyyyMMdd"), "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
    //            }else
    //            {
    //                postDataContent += CHT_WordPadLeftRight(row["DataDate"].ToString(), "L", 8, ' ');   // 1. 1 資料別          (固定值為1)
    //            }


    //            if (systemtype.Equals("AML"))
    //            {
    //                postDataContent += CHT_WordPadLeftRight(row["CustomerID"].ToString(), "L", 14, ' ');   // 1. 1 資料別          (固定值為1)
    //            }
    //            else
    //            {
    //                //給主機要加總部序號，不然他們說他們對應不到
    //                postDataContent += CHT_WordPadLeftRight(row["CustomerID"].ToString() + row["corpseq"].ToString(), "L", 14, ' ');   // 1. 1 資料別          (固定值為1)
    //            }

    //            //20191218-應姵晴MAIL(2019/12/18 (週三) 上午 11:47)要求，案件先全送C給主機 by Peggy
    //            //MAIL內容：法遵公告12月新案有排除不合作的高風險因子，但不適用12月前建案的，為讓AML可以重算正確的風險等級，
    //            // 麻煩您調整結案後送主機的AML審查FLAG都送C，謝謝。
    //            /*20191218 MARK
    //             //20191127-RQ-2018-015749-002 解除不合作的案件(解除不合作流程：當日日期8碼+9+5碼流水號)，送主機時欄位請帶"C"
    //             //postDataContent += CHT_WordPadLeftRight("Y", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
    //             if (!string.IsNullOrEmpty(row["CASE_NO"].ToString()) && row["CASE_NO"].ToString().Trim().Substring(8, 1).Equals("9"))
    //             {
    //                 postDataContent += CHT_WordPadLeftRight("C", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)}
    //             }
    //             else
    //             {
    //                 postDataContent += CHT_WordPadLeftRight("Y", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
    //             }
    //             */

    //            //20200318--應姵晴MAIL(2020/03/18 (週三) 下午 16:14)要求,，2020/3/27 RC新增需求_傳送主機結案檔案的AML審查FLAG都送Y
    //            /*
    //             //20200114-RQ-2019-030155-002
    //            if (systemtype.Equals("AML"))//送AML時，請維持送Y，AML系統根據此欄位做結案
    //            {
    //                postDataContent += CHT_WordPadLeftRight("Y", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
    //            }
    //            else//送主機時，審查FLAG送C
    //            {
    //                postDataContent += CHT_WordPadLeftRight("C", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)}
    //            }
    //            */
    //            postDataContent += CHT_WordPadLeftRight("Y", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)


    //            postDataContent += CHT_WordPadLeftRight(row["LastUpdateMaker"].ToString(), "L", 12, ' ');   // 1. 1 資料別          (固定值為1)
    //            postDataContent += CHT_WordPadLeftRight(row["LastUpdateChecker"].ToString(), "L", 12, ' ');   // 1. 1 資料別          (固定值為1)
    //            postDataContent += CHT_WordPadLeftRight(row["LastUpdateBranch"].ToString(), "L", 4, ' ');   // 1. 1 資料別          (固定值為1)
    //            postDataContent += CHT_WordPadLeftRight(Convert.ToDateTime(row["LastUpdateDate"]).ToString("yyyyMMdd"), "L", 8, ' ');   // 1. 1 資料別          (固定值為1)

    //            if (i != rowcountnum)
    //            {
    //                postDataContent += "\n";
    //            }

    //            sb.Append(postDataContent);

    //        }
    //    }


    //    //收OMS檔案合併到原本的
    //    //OMS CASE 合併START
    //    string OMSCASEline;
    //    string OMSCASECtlline;
    //    string OMSAPPDATAAPPLEND = "";
    //    int OMSCASECtlNum = 0;
    //    int OMSCASEFileNum = 0;

    //    if (OMSCASEfilename != null && !OMSCASEfilename.Equals(""))
    //    {
    //        if (OMSCASECtlfilename != null && !OMSCASECtlfilename.Equals(""))
    //        {

    //            JobHelper.Write(jobID, "進行OMS結案檔檔案合併");

    //            if (sb.ToString() != null && !sb.ToString().Equals(""))
    //            {
    //                OMSAPPDATAAPPLEND = "\n";
    //            }

    //            //int OMSCASECtlNum = 0;
    //            //int OMSCASEFileNum = 0;

    //            StreamReader streamReaderCtl = new StreamReader(OMSCASECtlfilename, System.Text.Encoding.UTF8);
    //            while ((OMSCASECtlline = streamReaderCtl.ReadLine()) != null)
    //            {
    //                OMSCASECtlNum = int.Parse(OMSCASECtlline);
    //            }

    //            StreamReader streamReader = new StreamReader(OMSCASEfilename, System.Text.Encoding.UTF8);
    //            while ((OMSCASEline = streamReader.ReadLine()) != null)
    //            {
    //                if (OMSCASEline != null && !OMSCASEline.Equals(""))
    //                {
    //                    OMSAPPDATAAPPLEND = OMSAPPDATAAPPLEND + OMSCASEline + "\n";
    //                    OMSCASEFileNum = OMSCASEFileNum + 1;
    //                }
    //            }

    //            JobHelper.Write(jobID, "OMS資料檔共有:" + OMSCASEFileNum + "筆;OMS CTL檔案共有:" + OMSCASECtlNum+"筆");

    //            //file的筆數要跟ctl的筆數相等
    //            if (OMSCASEFileNum == OMSCASECtlNum)
    //            {
    //                //OMSAPPDATAAPPLEND = OMSAPPDATAAPPLEND.Substring(0, OMSAPPDATAAPPLEND.Length - 2);
    //                sb.Append(OMSAPPDATAAPPLEND);
    //            }
    //            else
    //            {
    //                //筆數不相等發信通知
    //                postAMLCaseService.SendMail(jobID, "OMS FILE檔案與CTL檔筆數不一致，不做合併", "", "OMS FILE檔案與CTL檔筆數不一致，不做合併，合併失敗", this.StartTime);
    //                JobHelper.Write(jobID, "OMS FILE檔案與CTL檔筆數不一致，不做合併");
    //                isCompleteOMS = false;
    //            }
    //        }
    //    }

    //    //OMS CASE 合併END
    //    //如果OMS合併失敗了連FTP都不要上傳以免出錯
    //    if (isCompleteOMS)
    //    {
    //        // 寫入檔案
    //        postAMLCaseService.CreateFile(path, fileName, sb.ToString());
    //        //postOfficeService.CreateFile(path, fileName, sb);
    //        // 上送FTP
    //        isUploadToFTP = postAMLCaseService.UploadFileToMFTP(jobID, localFileInfo, path, fileName);

    //        CombineOMSCount = OMSCASEFileNum;

    //        if (isUploadToFTP)
    //        {
    //            JobHelper.Write(jobID, "FileName：" + fileName + "，上送FTP 成功");
    //            isComplete = true;
    //        }
    //        else
    //        {
    //            JobHelper.Write(jobID, "[FAIL] FileName：" + fileName + "，上送FTP 失敗");
    //            isComplete = false;
    //        }
    //    }
    //    else
    //    {
    //        isComplete = false;
    //    }
    //    return isComplete;
    //}
    

    // 產檔及上傳到 MFTP
    /*
    private bool GenaratorFileAndUploadToMFTP2(string jobID, string fileName, DataTable postData, PostAMLCaseService postAMLCaseService, DateTime dateTime,string OMSCaseDataCtl)
    {

        string path = string.Empty;
        DataTable localFileInfo = new DataTable();
        StringBuilder sb = new StringBuilder();
        //DataTable sb = new DataTable();
        string pwd = string.Empty;
        bool isUploadToFTP = true;
        bool isComplete = false;

        // 取得上傳檔案存放路徑
        path = GetLocalFilePath(jobID);

        string postDataContent = string.Empty;
        string postDataContentb = string.Empty;
        string postDataContentc = string.Empty;

        int omscasecout = 0;
        //OMSCASE合併START
        string linecount;
        int writecount = 0;
        writecount = postData.Rows.Count;

        if (OMSCaseDataCtl != null && !OMSCaseDataCtl.Equals(""))
        {

            JobHelper.Write(jobID, "合併CTL筆數");
            StreamReader streamReader = new StreamReader(OMSCaseDataCtl, System.Text.Encoding.UTF8);
            while ((linecount = streamReader.ReadLine()) != null)
            {
                omscasecout = int.Parse(linecount);
                writecount = writecount + omscasecout;
            }
        }
        
        //OMSCASE合併End

        postDataContent += CHT_WordPadLeftRight(writecount + "", "R", 10, '0');   // 結案案件有幾筆

        sb.Append(postDataContent);

        // 設定上傳檔案資訊欄位
        //localFileInfo = postAMLCaseService.SetLocalFileInfoColumn(path, fileName);

        // 寫入 FileTable
        //isComplete = postAMLCaseService.InsertFileData(fileName, dateTime, postData);

        //if (isComplete)
        //{
        // 郵局核印傳送資料組成(讀取 FileTable)
        //sb = postAMLCaseService.SetInvContent(fileName);

        // 寫入檔案
        postAMLCaseService.CreateFile(path, fileName, sb.ToString());
        //postOfficeService.CreateFile(path, fileName, sb);

        // 上送FTP
        isUploadToFTP = postAMLCaseService.UploadFileToMFTP(jobID, localFileInfo, path, fileName);

        if (isUploadToFTP)
        {
            JobHelper.Write(jobID, "FileName：" + fileName + "，上送FTP 成功");
            isComplete = true;
        }
        else
        {
            JobHelper.Write(jobID, "[FAIL] FileName：" + fileName + "，上送FTP 失敗");
            isComplete = false;
        }
        //}
        //return true;

        return isComplete;
    }
    */
    #endregion


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

    //2020-04-15-RQ-2019-030155-005 取得上一營業日與執行日之間，所需讀取檔案數
    //private DataTable DiffDate(string LastWorkDate, string ExecuteDate)
    //{
    //    DataTable dt_Result = new DataTable();

    //    //日期判斷改由上一層判斷
    //    //DateTime _dt1 = DateTime.ParseExact(LastWorkDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
    //    //dt_Result = BRFORM_COLUMN.GetWork_Date(_dt1.AddDays(1).ToString("yyyyMMdd"), ExecuteDate.Trim());

    //    dt_Result = BRFORM_COLUMN.GetWork_Date(LastWorkDate.Trim(), ExecuteDate.Trim());

    //    return dt_Result;
    //}
    
    //產檔及上傳到 MFTP FOR 拋送AML 及合併OMS結案檔用
    private bool GenaratorFileAndUploadToMFTP_new(string jobID, string fileName, DataTable postData, PostAMLCaseService postAMLCaseService, DateTime dateTime, string systemtype)
    {
        string path = string.Empty;
        DataTable localFileInfo = new DataTable();
        StringBuilder sb = new StringBuilder();
        string pwd = string.Empty;
        bool isUploadToFTP = true;
        bool isComplete = false;
        bool isCompleteOMS = true;
        // 取得上傳檔案存放路徑
        path = GetLocalFilePath(jobID);

        string postDataContentb = string.Empty;
        string postDataContentc = string.Empty;

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

                postDataContent = CHT_WordPadLeftRight(row["RMMBatchNo"].ToString(), "L", 14, ' ');   // 1. RMM批號 (來自AML原始檔案)
                postDataContent += CHT_WordPadLeftRight(row["AMLInternalID"].ToString(), "L", 20, ' ');   // 2. AML內部使用的編號 (來自AML原始檔案)
                postDataContent += CHT_WordPadLeftRight(row["SourceSystem"].ToString(), "L", 8, ' ');   // 3. 前端系統  (固定值為 'CARD')

                if (systemtype.Trim().Equals("AML"))
                {
                    postDataContent += CHT_WordPadLeftRight(DateTime.Now.ToString("yyyyMMdd"), "L", 8, ' ');   // 4. 資料日期
                }
                else
                {
                    postDataContent += CHT_WordPadLeftRight(row["DataDate"].ToString(), "L", 8, ' ');   // 4. 資料日期
                }

                if (systemtype.Trim().Equals("AML"))
                {
                    postDataContent += CHT_WordPadLeftRight(row["CustomerID"].ToString(), "L", 14, ' ');   // 5. 客戶ID
                }
                else
                {
                    //給主機要加總部序號，不然他們說他們對應不到
                    postDataContent += CHT_WordPadLeftRight(row["CustomerID"].ToString() + row["corpseq"].ToString(), "L", 14, ' ');   // 5. 客戶ID
                }

                //20191218-應姵晴MAIL(2019/12/18 (週三) 上午 11:47)要求，案件先全送C給主機 by Peggy
                //MAIL內容：法遵公告12月新案有排除不合作的高風險因子，但不適用12月前建案的，為讓AML可以重算正確的風險等級，
                // 麻煩您調整結案後送主機的AML審查FLAG都送C，謝謝。
                /*20191218 MARK
                 //20191127-RQ-2018-015749-002 解除不合作的案件(解除不合作流程：當日日期8碼+9+5碼流水號)，送主機時欄位請帶"C"
                 //postDataContent += CHT_WordPadLeftRight("Y", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
                 if (!string.IsNullOrEmpty(row["CASE_NO"].ToString()) && row["CASE_NO"].ToString().Trim().Substring(8, 1).Equals("9"))
                 {
                     postDataContent += CHT_WordPadLeftRight("C", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)}
                 }
                 else
                 {
                     postDataContent += CHT_WordPadLeftRight("Y", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
                 }
                 */

                //20200409-RQ-2019-030155-005：為區分是否報送AML或僅需帶CC至AML，增加FLAG：Z，並將送主機的FLAG改塑C
                //20200318--應姵晴MAIL(2020/03/18 (週三) 下午 16:14)要求,，2020/3/27 RC新增需求_傳送主機結案檔案的AML審查FLAG都送Y
                /*
                 //20200114-RQ-2019-030155-002
                if (systemtype.Equals("AML"))//送AML時，請維持送Y，AML系統根據此欄位做結案
                {
                    postDataContent += CHT_WordPadLeftRight("Y", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)
                }
                else//送主機時，審查FLAG送C
                {
                    postDataContent += CHT_WordPadLeftRight("C", "L", 1, ' ');   // 1. 1 資料別          (固定值為1)}
                }
                */
                //20200409-RQ-2019-030155-005：為區分是否報送AML或僅需帶CC至AML，增加FLAG：Z，並將送主機的FLAG改塑C
                postDataContent += CHT_WordPadLeftRight("Y", "L", 1, ' ');   // 6. RMM完成 (固定值為'Y'，此欄位不可異動，否則會被剔退)

                postDataContent += CHT_WordPadLeftRight(row["LastUpdateMaker"].ToString(), "L", 12, ' ');   // 7. 資料最後異動Maker
                postDataContent += CHT_WordPadLeftRight(row["LastUpdateChecker"].ToString(), "L", 12, ' ');   // 8. 資料最後異動Checker
                postDataContent += CHT_WordPadLeftRight(row["LastUpdateBranch"].ToString(), "L", 4, ' ');   // 9. 資料最後異動分行
                postDataContent += CHT_WordPadLeftRight(Convert.ToDateTime(row["LastUpdateDate"]).ToString("yyyyMMdd"), "L", 8, ' ');   // 10. 資料最後異動日期

                if (i != rowcountnum)
                {
                    postDataContent += "\n";
                }

                sb.Append(postDataContent);
            }
        }
        // 20210527 EOS_AML(NOVA) by Ares Dennis
        #region 退版機制
        DataTable dt = new DataTable();
        CSIPCommonModel.BusinessRules_new.BRM_PROPERTY_CODE.GetEnableProperty("01", new string[] { "OLD_VERSION_FLAG" }, ref dt);
        string flag = "";
        if (dt.Rows.Count > 0)
        {
            flag = dt.Rows[0]["PROPERTY_CODE"].ToString();
        }
        #endregion
        if(flag == "N")// 新版程式碼
        {
            // 寫入檔案
            postAMLCaseService.CreateFile(path, fileName, sb.ToString());
            // 上送FTP
            isUploadToFTP = postAMLCaseService.UploadFileToMFTP(jobID, localFileInfo, path, fileName);            

            if (isUploadToFTP)
            {
                JobHelper.Write(jobID, "FileName：" + fileName + "，上送FTP 成功", LogState.Info);
                isComplete = true;
            }
            else
            {
                JobHelper.Write(jobID, "[FAIL] FileName：" + fileName + "，上送FTP 失敗");
                isComplete = false;
            }
        }
        else// 舊版程式碼
        {
            #region 合併OMS系統結案檔
            //收OMS檔案合併到原本的
            //OMS CASE 合併START
            string OMSCASEline;
            string OMSCASECtlline;
            string OMSAPPDATAAPPLEND = "";
            int OMSCASECtlNum = 0;
            int OMSCASEFileNum = 0;

            //file的筆數要跟ctl的筆數相等
            if (listDat.Count > 0 && listCtl.Count > 0 && (listDat.Count == listCtl.Count))
            {
                JobHelper.Write(jobID, "進行OMS結案檔檔案合併", LogState.Info);

                if (sb.ToString() != null && !sb.ToString().Equals(""))
                {
                    OMSAPPDATAAPPLEND = "\n";
                }

                for (int i = 0; i < listDat.Count; i++)
                {
                    StreamReader streamReaderCtl = new StreamReader(listCtl[i], System.Text.Encoding.UTF8);
                    while ((OMSCASECtlline = streamReaderCtl.ReadLine()) != null)
                    {
                        OMSCASECtlNum = OMSCASECtlNum + int.Parse(OMSCASECtlline);
                    }

                    StreamReader streamReader = new StreamReader(listDat[i], System.Text.Encoding.UTF8);
                    while ((OMSCASEline = streamReader.ReadLine()) != null)
                    {
                        if (OMSCASEline != null && !OMSCASEline.Equals(""))
                        {
                            OMSAPPDATAAPPLEND = OMSAPPDATAAPPLEND + OMSCASEline + "\n";
                            OMSCASEFileNum = OMSCASEFileNum + 1;
                        }
                    }
                }

                JobHelper.Write(jobID, "OMS資料檔共有:" + OMSCASEFileNum + "筆;OMS CTL檔案共有:" + OMSCASECtlNum + "筆", LogState.Info);

                //file的筆數要跟ctl的筆數相等
                if (OMSCASEFileNum == OMSCASECtlNum)
                {
                    sb.Append(OMSAPPDATAAPPLEND);
                }
                else
                {
                    //筆數不相等發信通知
                    postAMLCaseService.SendMail(jobID, "OMS FILE檔案 DAT與CTL檔筆數不一致，不做合併", "", "OMS FILE檔案 DAT與CTL檔筆數不一致，不做合併，合併失敗", this.StartTime);
                    JobHelper.Write(jobID, "OMS FILE檔案 DAT與CTL檔筆數不一致，不做合併", LogState.Info);
                    isCompleteOMS = false;
                }
            }

            //OMS CASE 合併END
            //如果OMS合併失敗了連FTP都不要上傳以免出錯
            if (isCompleteOMS)
            {
                // 寫入檔案
                postAMLCaseService.CreateFile(path, fileName, sb.ToString());
                // 上送FTP
                isUploadToFTP = postAMLCaseService.UploadFileToMFTP(jobID, localFileInfo, path, fileName);

                CombineOMSCount = OMSCASEFileNum;

                if (isUploadToFTP)
                {
                    JobHelper.Write(jobID, "FileName：" + fileName + "，上送FTP 成功", LogState.Info);
                    isComplete = true;
                }
                else
                {
                    JobHelper.Write(jobID, "[FAIL] FileName：" + fileName + "，上送FTP 失敗");
                    isComplete = false;
                }
            }
            else
            {
                isComplete = false;
            }
            #endregion
        }

        return isComplete;

    }

    private bool GenaratorFileAndUploadToMFTP2_new(string jobID, string fileName, DataTable postData, PostAMLCaseService postAMLCaseService)
    {
        string path = string.Empty;
        DataTable localFileInfo = new DataTable();
        StringBuilder sb = new StringBuilder();
        string pwd = string.Empty;
        bool isUploadToFTP = true;
        bool isComplete = false;

        // 取得上傳檔案存放路徑
        path = GetLocalFilePath(jobID);

        string postDataContent = string.Empty;
        string postDataContentb = string.Empty;
        string postDataContentc = string.Empty;

        int omscasecout = 0;
        //OMSCASE合併START
        string linecount;
        int writecount = 0;
        writecount = postData.Rows.Count;

        //file的筆數要跟ctl的筆數相等
        if (listCtl.Count > 0)
        {
            JobHelper.Write(jobID, "合併CTL筆數", LogState.Info);

            for (int i = 0; i < listCtl.Count; i++)
            {
                StreamReader streamReader = new StreamReader(listCtl[i], System.Text.Encoding.UTF8);
                while ((linecount = streamReader.ReadLine()) != null)
                {
                    omscasecout = int.Parse(linecount);
                    writecount = writecount + omscasecout;
                }
            }
        }

        //OMSCASE合併End

        postDataContent += CHT_WordPadLeftRight(writecount + "", "R", 10, '0');   // 結案案件有幾筆

        sb.Append(postDataContent);

        // 寫入檔案
        postAMLCaseService.CreateFile(path, fileName, sb.ToString());

        // 上送FTP
        isUploadToFTP = postAMLCaseService.UploadFileToMFTP(jobID, localFileInfo, path, fileName);

        if (isUploadToFTP)
        {
            JobHelper.Write(jobID, "FileName：" + fileName + "，上送FTP 成功", LogState.Info);
            isComplete = true;
        }
        else
        {
            JobHelper.Write(jobID, "[FAIL] FileName：" + fileName + "，上送FTP 失敗");
            isComplete = false;
        }
        return isComplete;
    }
}