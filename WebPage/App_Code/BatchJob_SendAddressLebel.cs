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
/// BatchJob_SendAddressLebel 的摘要描述
/// </summary>
public class BatchJob_SendAddressLebel : Quartz.IJob
{
    protected string FunctionKey = UtilHelper.GetAppSettings("FunctionKey").ToString();
    protected DateTime StartTime = DateTime.Now;// 記錄Job啟動時間
    protected DateTime EndTime;
    protected JobHelper JobHelper = new JobHelper();
    protected string _MailTitle = "傳送定審地址條 批次：";//20191227-RQ-2019-030155-002-批次信函調整 by Peggy

    public void Execute(JobExecutionContext context)
    {
        int total = 0;
        int success = 0;
        string jobID = context.JobDetail.JobDataMap["jobid"].ToString();
        JobHelper.strJobID = jobID;
        string fileName = "";
        string fileNamectl = "";
        bool isContinue = true;
        string msgID = string.Empty;
        PostAddLebelService postAddLebelService = new PostAddLebelService(jobID);

        try
        {

            DataTable postData = new DataTable();
            DataTable postlebeldatanull = new DataTable();

            DateTime dateTime = new DateTime();
            string startTime = "";
            string endTime = "";
            bool isComplete = false;
            string _ExecDate = DateTime.Now.ToString("yyyyMMdd").Trim();
            string _ExecDatePrev = DateTime.Now.AddDays(-1).ToString("yyyyMMdd").Trim();
            bool isNeedToWait1082 = false;//是否需要等待OMI BILL回檔(20201118-RQ-2020-021027-003)

            JobHelper.Write(jobID, "*********** " + jobID + " 傳送地址條檔案 批次 START ************** ", LogState.Info);

            // 判斷Job工作狀態(0:停止 1:運行)
            isContinue = CheckJobIsContinue(jobID, ref msgID);

            //判斷執行日當天是否有設定EMAIL檔(20201118-RQ-2020-021027-003)
            DataTable dtRegularAuditMail = PostRegularAuditMailService.CheckRegularAuditMail(_ExecDate);
            if (dtRegularAuditMail.Rows.Count > 0)
            {
                isNeedToWait1082 = true;
            }

            if (isContinue && !isNeedToWait1082)
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





                // 算起迄時間
                postAddLebelService.GetSearchTime(this.StartTime, out startTime, out endTime);
                endTime = DateTime.Now.ToString("yyyyMMdd");
                dateTime = Convert.ToDateTime(endTime.Substring(0, 4) + "-" + endTime.Substring(4, 2) + "-" + endTime.Substring(6, 2));
                endTime = endTime.Substring(2, 6);
               
                fileName = @"JCCPPD80_JC_AMLCSIP1_PA08_0001_D_" + endTime + "_XXXXXX_XXXXXX.BIN";
                fileNamectl = @"JCCPPD80_JC_AMLCSIP1_PA08_0001_D_" + endTime + "_XXXXXX_XXXXXX.HTD";

                //20200714 調整，剔退檔內容為前一天的剔退內容
                //string _ExecDate = DateTime.Now.AddDays(-1).ToString("yyyyMMdd").Trim();
                //20200813-RQ-2020-021027-001 判斷如果執行日當天有email設定的資料，則紙本執行日則延後一日執行
                //string _ExecDate = DateTime.Now.ToString("yyyyMMdd").Trim();
                //string _ExecDatePrev = DateTime.Now.AddDays(-1).ToString("yyyyMMdd").Trim();

                //DataTable dtRegularAuditMail = PostRegularAuditMailService.GetRegularAuditMail(_ExecDate);
                //if (dtRegularAuditMail.Rows.Count > 0)
                //{
                //    _ExecDate = DateTime.Now.AddDays(-1).ToString("yyyyMMdd").Trim();
                //}

                //如果Parameter有值，則使用Parameter的時間
                DataTable tblFileInfo = BRAML_File_Import.GetFileInfo(jobID);
                if (!string.IsNullOrEmpty(tblFileInfo.Rows[0]["Parameter"].ToString()))
                {
                    _ExecDate = tblFileInfo.Rows[0]["Parameter"].ToString().Trim();
                }
                else
                {
                    dtRegularAuditMail = new DataTable();
                    dtRegularAuditMail = PostRegularAuditMailService.CheckRegularAuditMail(_ExecDatePrev);
                    if (dtRegularAuditMail.Rows.Count > 0)
                    {
                        _ExecDate = _ExecDatePrev;
                    }
                }

                //先檢查是否有縣市沒有區碼的，若沒有區碼要去szip TABLE維護
                postlebeldatanull = postAddLebelService.GetLebelDataNull(_ExecDate);

                string lebelnull = "";

                if (postlebeldatanull.Rows.Count > 0)
                {
                    foreach (DataRow rowlebeldata in postlebeldatanull.Rows)
                    {
                        lebelnull = lebelnull + rowlebeldata["HCOP_HEADQUATERS_CORP_NO"].ToString() + "/";
                        lebelnull = lebelnull + rowlebeldata["HCOP_MAILING_CITY"].ToString() + ";";

                        //20200320-RQ-2019-030155-003 若郵遞區號為NULL，則更新Edata_Work.CREATE_USER, 以利資料補齊後能重新產出
                        BRFORM_COLUMN.UpdateEdataCREATE_USER(rowlebeldata["CASE_NO"].ToString().Trim(), "F");
                    }

                    //20200320-RQ-2019-030155-003 於log記錄批次歷程
                    JobHelper.Write(jobID, "[WARNING] HQ_WORK的HCOP_MAILING_CITY有區號為NULL，統編縣市為" + lebelnull + "，請至SZIP TABLE維護 ");
                    // 寫入 BatchLog
                    InsertBatchLog(jobID, postlebeldatanull.Rows.Count, 0, "F", "HQ_WORK的HCOP_MAILING_CITY有區號為NULL");

                    postAddLebelService.SendMail(jobID, "HQ_WORK的HCOP_MAILING_CITY有區號為NULL", "HQ_WORK的HCOP_MAILING_CITY有區號為NULL，統編縣市為" + lebelnull + "，請至SZIP TABLE維護 ", "失敗", this.StartTime);
                }
                else
                {
                    // 取待發送的資料
                    postData = postAddLebelService.GetAddressLebelData(_ExecDate);

                    // 產檔及上傳到 MFTP
                    isComplete = GenaratorFileAndUploadToMFTP(jobID, fileName, postData, postAddLebelService, dateTime);

                    // 產檔及上傳到 MFTP
                    isComplete = GenaratorCtlFileAndUploadToMFTP(jobID, fileNamectl, postData, postAddLebelService, dateTime);

                    success = postData.Rows.Count;
                    if (isComplete)
                    {
                        total = postData.Rows.Count;

                        // 寫入 BatchLog
                        InsertBatchLog(jobID, total, success, "S", "");

                        if (total > 0)
                        {
                            // 發送 Email
                            //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
                            //postAddLebelService.SendMail(jobID, "傳送定期審查地址條檔案 批次:" + fileName + " 上傳成功！", "總筆數:" + total, "上傳成功", this.StartTime);
                            postAddLebelService.SendMail(jobID, _MailTitle + "成功！總筆數：" + total + " 筆", _MailTitle + fileName + "上傳成功！總筆數：" + total + " 筆", "成功", this.StartTime);
                        }
                        else
                        {
                            // 發送 Email
                            //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
                            //postAddLebelService.SendMail(jobID, "傳送定期審查地址條檔案 批次:" + fileName + " 成功！", "無資料", "成功", this.StartTime);
                            postAddLebelService.SendMail(jobID, _MailTitle + "成功！總筆數：0 筆", _MailTitle + fileName + "上傳成功！總筆數：" + total + " 筆", "成功", this.StartTime);
                        }

                        //20200320-RQ-2019-030155-003 若定審地址條正常產出，則更新Edata_Work.CREATE_USER='CSIP_System'
                        foreach (DataRow dr in postData.Rows)
                        {
                            BRFORM_COLUMN.UpdateEdataCREATE_USER(dr["CASE_NO"].ToString().Trim(), "S");
                        }
                    }
                    else
                    {
                        // 寫入 BatchLog
                        InsertBatchLog(jobID, total, 0, "F", "上傳失敗");

                        // 發送 Email
                        //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
                        //postAddLebelService.SendMail(jobID, "傳送定期審查地址條檔案 批次:" + fileName + " 上傳失敗！", ex.Message, "上傳失敗", this.StartTime);
                        postAddLebelService.SendMail(jobID, _MailTitle + "失敗！總筆數：" + total + " 筆，但產出 0 筆，未產出" + total + "筆", _MailTitle + fileName + "上傳失敗！總筆數：" + total + " 筆", "失敗", this.StartTime);
                    }

                    JobHelper.Write(jobID, "[SUCCESS] 傳送定期審查地址條檔案 批次 END " + fileName, LogState.Info);
                }
            }
            else
            {
                // 寫入 BatchLog
                InsertBatchLog(jobID, total, success, "S", "");

                if (!isContinue)
                {
                    // 發送 Email
                    postAddLebelService.SendMail(jobID, _MailTitle + "JOB狀態為停止！", _MailTitle + "JOB狀態設定為停止，請洽維護IT！", "成功", this.StartTime);
                    JobHelper.Write(jobID, "[SUCCESS] " + _MailTitle + "JOB狀態設定為停止，請洽維護IT！");
                }

                if (isNeedToWait1082)
                {
                    // 發送 Email
                    postAddLebelService.SendMail(jobID, _MailTitle + "今日不執行批次！", _MailTitle + "今日有Email檔案，故今日不執行批次！", "成功", this.StartTime);
                    JobHelper.Write(jobID, "[SUCCESS] "+ _MailTitle + "今日有Email檔案，故今日不執行批次！待明日OMI BILL平台回檔後才執行", LogState.Info);
                }

            }
        }
        catch (Exception ex)
        {
            // 寫入 BatchLog
            InsertBatchLog(jobID, total, success, "F", "發生錯誤：" + ex.Message);

            // 發送 Email
            //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
            //postAddLebelService.SendMail(jobID, "傳送定期審查地址條檔案 批次:" + fileName + " 上傳失敗！", ex.Message, "上傳失敗", this.StartTime);
            postAddLebelService.SendMail(jobID, _MailTitle + "失敗！總筆數：" + total + " 筆，但產出 0 筆，未產出" + total + "筆", _MailTitle + fileName + "上傳失敗！總筆數：" + total + " 筆", "上傳失敗，發生錯誤：" + ex.Message, this.StartTime);

            JobHelper.Write(jobID, "[FAIL] 傳送定期審查地址條檔案 批次 " + fileName + " 發生錯誤：" + ex.Message);
        }
        finally
        {
            // 20200031-CSIP EOS Ares Luke 修改日期:2021/03/12 修改說明:業務需求排程結束清空FilInfo參數
            // 清除 FileInfo Parameter值
            BRM_FileInfo.UpdateParam(jobID, "");


            JobHelper.Write(jobID, " 傳送地址條檔案 批次 Job 結束！", LogState.Info);
            JobHelper.Write(jobID, "================================================================================================================================================", LogState.Info);
    }
    }

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
        PostAddLebelService postAddLebelService = new PostAddLebelService(jobID);
        int total = 0;
        int success = 0;
        bool result = false;
        try
        {
            DataTable postData = new DataTable();
            bool isComplete = false;

            JobHelper.Write(jobID, "*********** " + jobID + " 傳送地址條檔案 手動 START ************** ", LogState.Info);

            // 取得應該傳送的地址條
            postData = postAddLebelService.GetSendToPostOfficeData(endTime);

            // 產檔及上傳到 MFTP
            isComplete = GenaratorFileAndUploadToMFTP(jobID, fileName, postData, postAddLebelService, dateTime);

            if (isComplete)
            {
                total = postData.Rows.Count;

                // 寫入 BatchLog
                InsertBatchLog(jobID, total, success, "S", "");

                // 發送 Email
                postAddLebelService.SendMail(jobID, "傳送地址條檔案 手動:" + fileName + " 上傳成功！", "總筆數:" + total, "上傳成功", this.StartTime);
            }
            else
            {
                // 寫入 BatchLog
                InsertBatchLog(jobID, total, success, "S", "");

                // 發送 Email
                postAddLebelService.SendMail(jobID, "傳送地址條檔案 手動:" + fileName + " 成功！", "無資料", "成功", this.StartTime);
            }

            result = true;

            JobHelper.Write(jobID, "[SUCCESS] 傳送地址條檔案 手動 END " + fileName, LogState.Info);

        }
        catch (Exception ex)
        {
            // 發送 Email
            postAddLebelService.SendMail(jobID, "傳送地址條檔案 手動:" + fileName + " 上傳失敗！", ex.Message, "上傳失敗", this.StartTime);

            JobHelper.Write(jobID, "[FAIL] 傳送地址條檔案 手動 " + fileName + " 發生錯誤：" + ex.Message);
        }
        finally
        {
            JobHelper.Write(jobID, " 郵局核印 手動 Job 結束！", LogState.Info);
            JobHelper.Write(jobID, "================================================================================================================================================", LogState.Info);
        }

        return result;
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

    // 產檔及上傳到 MFTP
    private bool GenaratorFileAndUploadToMFTP(string jobID, string fileName, DataTable postData, PostAddLebelService postAddLebelService, DateTime dateTime)
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


        if (postData.Rows.Count == 0)
        {

            postDataContent  = CHT_WordPadLeftRight("", "L", 5, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 14, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 30, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 30, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += CHT_WordPadLeftRight("", "L", 40, ' ');   // 1. 1 資料別          (固定值為1)
            postDataContent += "\r\n";

            sb.Append(postDataContent);
        }
        else
        {
            int rowcountnum = postData.Rows.Count;
            int i = 0;

            foreach (DataRow row in postData.Rows)
            {
                i = i + 1;
                postDataContent =  CHT_WordPadLeftRight(row["ZIP_CODE"].ToString(), "L", 05, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["HCOP_MAILING_CITY"].ToString().Trim(), "L", 14, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["HCOP_MAILING_ADDR1"].ToString().Trim(), "L", 30, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["HCOP_MAILING_ADDR2"].ToString().Trim(), "L", 30, ' ');   // 1. 1 資料別          (固定值為1)
                postDataContent += CHT_WordPadLeftRight(row["HCOP_NAME_CHI"].ToString().Trim(), "L", 40, ' ');   // 1. 1 資料別          (固定值為1)
                if (i != rowcountnum)
                {
                    postDataContent += "\r\n";
                }

                sb.Append(postDataContent);

            }
        }
        // 設定上傳檔案資訊欄位
        //localFileInfo = postAMLCaseService.SetLocalFileInfoColumn(path, fileName);

        // 寫入 FileTable
        //isComplete = postAMLCaseService.InsertFileData(fileName, dateTime, postData);

        //if (isComplete)
        //{
        // 郵局核印傳送資料組成(讀取 FileTable)
        //sb = postAMLCaseService.SetInvContent(fileName);

        //如果是零筆地址條就不傳送，直接回傳 isComplete = true
        if (postData.Rows.Count == 0)
        {
            isComplete = true;
        }
        else
        {
            // 寫入檔案
            postAddLebelService.CreateFile(path, fileName, sb.ToString());
            //postOfficeService.CreateFile(path, fileName, sb);

            // 上送FTP
            isUploadToFTP = postAddLebelService.UploadFileToMFTP(jobID, localFileInfo, path, fileName);

            if (isUploadToFTP)
            {
                JobHelper.Write(jobID, "FileName：" + fileName + "，上送FTP 成功", LogState.Info);
                isComplete = true;

                foreach (DataRow row in postData.Rows)
                {
                    postAddLebelService.updatelabelsended(row["ID"].ToString());

                    //20200805-RQ-2020-021027-001 修改歷程顯示wording
                    //BRFORM_COLUMN.AML_NOTELOG(row["CASE_NO"].ToString(), "", "", "傳送地址條", "傳送定期審查地址條成功");
                    BRFORM_COLUMN.AML_NOTELOG(row["CASE_NO"].ToString(), "", "", "傳送地址條", "紙本定審地址條成功");

                    //2021/7/6 EOS_AML(NOVA) by Ares Dennis
                    #region 退版機制
                    DataTable dt = new DataTable();
                    CSIPCommonModel.BusinessRules_new.BRM_PROPERTY_CODE.GetEnableProperty("01", new string[] { "OLD_VERSION_FLAG" }, ref dt);
                    string flag = "";
                    if (dt.Rows.Count > 0)
                    {
                        flag = dt.Rows[0]["PROPERTY_CODE"].ToString();
                    }
                    #endregion
                    if(flag == "N")//新版程式碼
                    {                        
                        //每月收E檔作業，針對未結案的統編只能出一份信函，但在案件列表及結案列表的所有案件編號要能看到信函LOG
                        //相關聯的案件也要寫信函LOG
                        DataTable dtCaseNo = BRAML_HQ_Work.getRelatedCaseNo(row["CASE_NO"].ToString());
                        for (int i = 0; i < dtCaseNo.Rows.Count; i++)
                        {
                            string relatedCaseNo = dtCaseNo.Rows[i][0].ToString();
                            BRFORM_COLUMN.AML_NOTELOG(relatedCaseNo, "", "", "傳送地址條", "紙本定審地址條成功");
                        }                        
                    }
                }
            }
            else
            {
                JobHelper.Write(jobID, "[FAIL] FileName：" + fileName + "，上送FTP 失敗");
                isComplete = false;//20200121 修正flag
            }
            //}
            //return true;
        }
        return isComplete;
    }




    // 產檔及上傳到 MFTP
    private bool GenaratorCtlFileAndUploadToMFTP(string jobID, string fileName, DataTable postData, PostAddLebelService postAddLebelService, DateTime dateTime)
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


        postDataContent = CHT_WordPadLeftRight((postData.Rows.Count).ToString(), "R", 10, '0');   // 1. 1 資料別          (固定值為1)

        postDataContent += "\r\n";

        sb.Append(postDataContent);
        

        // 設定上傳檔案資訊欄位
        //localFileInfo = postAMLCaseService.SetLocalFileInfoColumn(path, fileName);

        // 寫入 FileTable
        //isComplete = postAMLCaseService.InsertFileData(fileName, dateTime, postData);

        //if (isComplete)
        //{
        // 郵局核印傳送資料組成(讀取 FileTable)
        //sb = postAMLCaseService.SetInvContent(fileName);

        if (postData.Rows.Count == 0)
        {
            isComplete = true;
        }
        else
        {
            // 寫入檔案
            postAddLebelService.CreateFile(path, fileName, sb.ToString());
            //postOfficeService.CreateFile(path, fileName, sb);

            // 上送FTP
            isUploadToFTP = postAddLebelService.UploadFileToMFTP(jobID, localFileInfo, path, fileName);

            if (isUploadToFTP)
            {
                JobHelper.Write(jobID, "FileName：" + fileName + "，上送FTP 成功", LogState.Info);
                isComplete = true;
            }
            else
            {
                JobHelper.Write(jobID, "[FAIL] FileName：" + fileName + "，上送FTP 失敗");
                isComplete = false;//20200121修正flag
            }
            //}
            //return true;
        }
        return isComplete;
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
