//******************************************************************
//* 作    者：
//* 功能說明：
//* 創建日期：
//* 修改紀錄：
//* <author>            <time>            <TaskID>                <desc>
//* Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//* Ares Luke          2021/03/18         20200031-CSIP EOS       調整執行順序，優先檢核排程是否執行中在檢核是否為執行日
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
using Framework.Common.Logging;
using Framework.Common.IO;
using Framework.Common.Utility;
using CSIPKeyInGUI.BusinessRules;

/// <summary>
/// BatchJob_SendTwoMonthAddressLebel 的摘要描述
/// 修改：執行時間為月底前的5個工作日執行，改由判斷WORK_DATE的TABLE @20200206 by Peggy
/// 修改：當發送內容有email時，執行日為月底前的6個工作日執行；發送內容無email時，執行日則為月底前7個工作日執行 @20200806 by Peggy
/// </summary>
public class BatchJob_SendTwoMonthAddressLebel : Quartz.IJob
{
    protected string FunctionKey = ConfigurationManager.AppSettings["FunctionKey"].ToString();
    protected DateTime StartTime = DateTime.Now;// 記錄Job啟動時間
    protected DateTime EndTime;
    protected JobHelper JobHelper = new JobHelper();
    protected string _MailTitle = "傳送不合作通知地址條：";//20191227-RQ-2019-030155-002-批次信函調整 by Peggy

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
        PostTwoMonthAddLebelService postTwoMonthAddLebelService = new PostTwoMonthAddLebelService(jobID);

        try
        {

            JobHelper.Write(jobID, "*********** " + jobID + " 傳送不合作信函地址條檔案 批次 START ************** ", LogState.Info);

            DataTable postData = new DataTable();
            DataTable postlebeldatanull = new DataTable();

            DateTime dateTime = new DateTime();
            string startTime = "";
            string endTime = "";

            string endTime2 = "";

            bool isComplete = false;
            int days = 7;
            string SQLDate = DateTime.Now.ToString("yyyyMM");

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


                /*20200806 修改：因為用到參數，所以註解
                String yy = DateTime.Now.Year.ToString();
                String mm = DateTime.Now.Month.ToString();
                String days = DateTime.DaysInMonth(int.Parse(yy), int.Parse(mm)).ToString();

                DateTime LastDay = DateTime.Parse(yy + "/" + mm + "/" + days);

                DateTime LastDayMINUS = AddBusinessDays(LastDay, -5);

                string strlastdayminus = LastDayMINUS.ToString("yyyyMMdd");
                */
                endTime2 = DateTime.Now.ToString("yyyyMMdd");

                //20200806-RQ-2020-021027-001 判斷資料內是否有設定email參數的
                if (BRFORM_COLUMN.GetNonCooperationEmail())
                {
                    //有email資料，則批次執行日為月底前6個工作日
                    days = 6;
                }
                else
                {
                    //無email資料，則批次執行日為月底前7個工作日
                    days = 7;
                }

                //取得月底前5個工作天日期
                string workingDate = BRFORM_COLUMN.GetLastWorkingDateFromWorkDateTable(days, SQLDate);

                DataTable tblFileInfo = BRAML_File_Import.GetFileInfo(jobID);

                //如果Parameter有值，則使用Parameter的時間
                if (!string.IsNullOrEmpty(tblFileInfo.Rows[0]["Parameter"].ToString()))
                {
                    endTime2 = tblFileInfo.Rows[0]["Parameter"].ToString().Trim();
                }
                //產出不合作信函「自動」排程規則如下
                //寄送不合作信函案件規則= 建案日期+2個月月底前5個工作天(只有排除六日，不排除國定假日，六日補班日不算在工作日)。例如建案日期為2019/5/11，不合作在2019/7/31前5個工作天=2019/7/24產地址條給委外寄出不合作信函

                //比對是否為可以執行的日期
                //20200206 修改：將計算月底前5個工作天的計算方式，改抓work_date倒數第5個工作天 by Peggy
                if (workingDate == endTime2)
                {
                    // 算起迄時間
                    postTwoMonthAddLebelService.GetSearchTime(this.StartTime, out startTime, out endTime);
                    endTime = DateTime.Now.ToString("yyyyMMdd");
                    dateTime = Convert.ToDateTime(endTime.Substring(2, 4) + "-" + endTime.Substring(4, 2) + "-" +
                                                  endTime.Substring(6, 2));
                    endTime = endTime.Substring(2, 6);

                    fileName = @"JCCPPD81_JC_AMLCSIP2_PA07_0001_D_" + endTime + "_XXXXXX_XXXXXX.BIN";
                    fileNamectl = @"JCCPPD81_JC_AMLCSIP2_PA07_0001_D_" + endTime + "_XXXXXX_XXXXXX.HTD";

                    //20200806-RQ-2020-021027-001
                    //取得不合作通知函地址為NULL的資料
                    //postlebeldatanull = postTwoMonthAddLebelService.GetLebelDataNullTwoMonth(endTime);
                    postlebeldatanull = postTwoMonthAddLebelService.GetLebelDataNullTwoMonth();

                    string lebelnull = "";

                    if (postlebeldatanull.Rows.Count > 0)
                    {
                        foreach (DataRow rowlebeldata in postlebeldatanull.Rows)
                        {
                            lebelnull = lebelnull + rowlebeldata["HCOP_MAILING_CITY"].ToString() + ";";
                        }

                        postTwoMonthAddLebelService.SendMail(jobID, "兩個月未結案HQ_WORK的HCOP_MAILING_CITY有區號為NULL",
                            "兩個月未結案HQ_WORK的HCOP_MAILING_CITY有區號為NULL，縣市為" + lebelnull, "，請至SZIP TABLE維護",
                            this.StartTime);
                    }
                    else
                    {
                        // 取得開案兩個月之後都還沒做結案的資料
                        //20200806-RQ-2020-021027-001
                        //postData = postTwoMonthAddLebelService.GetAddressLebelDataTwoMonth(endTime);
                        postData = postTwoMonthAddLebelService.GetAddressLebelDataTwoMonth();

                        // 產檔及上傳到 MFTP
                        isComplete = GenaratorFileAndUploadToMFTP(jobID, fileName, postData,
                            postTwoMonthAddLebelService, dateTime);

                        // 產檔及上傳到 MFTP
                        isComplete = GenaratorCtlFileAndUploadToMFTP(jobID, fileNamectl, postData,
                            postTwoMonthAddLebelService, dateTime);

                        success = postData.Rows.Count;

                        total = postData.Rows.Count;
                        if (isComplete)
                        {
                            // 寫入 BatchLog
                            InsertBatchLog(jobID, total, success, "S", "");

                            // 發送 Email
                            //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
                            //postTwoMonthAddLebelService.SendMail(jobID, "傳送不合作信函地址條檔案 批次:" + fileName + " 上傳成功！", "總筆數：" + total, "上傳成功", this.StartTime);
                            postTwoMonthAddLebelService.SendMail(jobID, _MailTitle + "成功！總筆數：" + total + " 筆",
                                "傳送不合作信函地址條檔案 批次：" + fileName + " 上傳成功！總筆數：" + total + " 筆", "上傳成功",
                                this.StartTime);
                        }
                        else
                        {
                            // 寫入 BatchLog
                            InsertBatchLog(jobID, total, success, "S", "");

                            // 發送 Email
                            //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
                            //postTwoMonthAddLebelService.SendMail(jobID, "傳送不合作信函地址條檔案 批次:" + fileName + " 成功！", "無資料", "成功", this.StartTime);
                            postTwoMonthAddLebelService.SendMail(jobID, _MailTitle + "成功！總筆數：" + total + " 筆",
                                "傳送不合作信函地址條檔案 批次:" + fileName + " 上傳成功！總筆數：" + total + " 筆", "上傳成功",
                                this.StartTime);
                        }

                        JobHelper.Write(jobID, "[SUCCESS] 傳送不合作信函地址條檔案 批次 END " + fileName, LogState.Info);
                    }
                }
                else
                {

                    // 20200031-CSIP EOS Ares Rick 修改日期:2021/04/12 修改說明:修正批次狀態未結束BUG
                    InsertBatchLog(jobID, total, success, "S", "");

                    //20200806
                    //JobHelper.Write(jobID, "今天不是月底前五個工作天，所以不執行不合作地址條匯出");
                    JobHelper.Write(jobID, "今天不是月底前'" + days + "'個工作天，所以不執行不合作地址條匯出", LogState.Info);
                }
            }
        }
        catch (Exception ex)
        {
            // 寫入 BatchLog
            InsertBatchLog(jobID, total, success, "F", "發生錯誤：" + ex.Message);

            // 發送 Email
            //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
            //postTwoMonthAddLebelService.SendMail(jobID, "傳送不合作信函地址條檔案 批次:" + fileName + " 上傳失敗！", ex.Message, "上傳失敗", this.StartTime);
            postTwoMonthAddLebelService.SendMail(jobID, _MailTitle + "失敗！總筆數：" + total + " 筆", "傳送不合作信函地址條檔案 批次:" + fileName + " 上傳失敗！總筆數：" + total + " 筆，未產出０筆", "上傳失敗", this.StartTime);

            JobHelper.Write(jobID, "[FAIL] 傳送不合作信函地址條檔案 批次 " + fileName + " 發生錯誤：" + ex.Message);
        }
        finally
        {
            // 20200031-CSIP EOS Ares Luke 修改日期:2021/03/12 修改說明:業務需求排程結束清空FilInfo參數
            // 清除 FileInfo Parameter值
            BRM_FileInfo.UpdateParam(jobID, "");

            JobHelper.Write(jobID, " 傳送不合作信函地址條檔案 批次 Job 結束！", LogState.Info);
            JobHelper.Write(jobID, "================================================================================================================================================", LogState.Info);
        }
    }

    /// <summary>
    /// 手動執行
    /// 按鈕位於定期審查 案件列表(P010801000001.aspx)中
    /// </summary>
    /// <param name="jobID"></param>
    /// <param name="endTime"></param>
    /// <returns></returns>
    public bool ExecuteManual()
    {
        int total = 0;
        int success = 0;
        string jobID = "SendlTwoMonthLabel";
        string fileName = "";
        string fileNamectl = "";
        bool isContinue = true;
        bool manualresult = false;
        string msgID = string.Empty;
        PostTwoMonthAddLebelService postTwoMonthAddLebelService = new PostTwoMonthAddLebelService("SendlTwoMonthLabel");

        try
        {
            DataTable postData = new DataTable();
            DataTable postlebeldatanull = new DataTable();

            DateTime dateTime = new DateTime();
            string startTime = "";
            string endTime = "";

            string endTime2 = "";

            bool isComplete = false;
            
            /*20200806 mark
            String yy = DateTime.Now.Year.ToString();
            String mm = DateTime.Now.Month.ToString();
            String days = DateTime.DaysInMonth(int.Parse(yy), int.Parse(mm)).ToString();

            DateTime LastDay = DateTime.Parse(yy + "/" + mm + "/" + days);

            DateTime LastDayMINUS = AddBusinessDays(LastDay, -5);

            string strlastdayminus = LastDayMINUS.ToString("yyyyMMdd");
            */
            endTime2 = DateTime.Now.ToString("yyyyMMdd");

            //產出不合作信函「自動」排程規則如下
            //寄送不合作信函案件規則= 建案日期+2個月月底前5個工作天(只有排除六日，不排除國定假日，六日補班日不算在工作日)。例如建案日期為2019/5/11，不合作在2019/7/31前5個工作天=2019/7/24產地址條給委外寄出不合作信函
            //比對是否為可以執行的日期
            
            JobHelper.Write(jobID, "*********** " + jobID + " 手動傳送不合作信函地址條檔案 批次 START ************** ", LogState.Info);

            // 判斷Job工作狀態(0:停止 1:運行)
            isContinue = CheckJobIsContinue(jobID, ref msgID);

            if (isContinue)
            {
                // 算起迄時間
                postTwoMonthAddLebelService.GetSearchTime(this.StartTime, out startTime, out endTime);
                endTime = DateTime.Now.ToString("yyyyMMdd");
                dateTime = Convert.ToDateTime(endTime.Substring(2, 4) + "-" + endTime.Substring(4, 2) + "-" + endTime.Substring(6, 2));
                endTime = endTime.Substring(2, 6);

                fileName = @"JCCPPD81_JC_AMLCSIP2_PA07_0001_D_" + endTime + "_XXXXXX_XXXXXX.BIN";
                fileNamectl = @"JCCPPD81_JC_AMLCSIP2_PA07_0001_D_" + endTime + "_XXXXXX_XXXXXX.HTD";

                //20200806-RQ-2020-021027-001
                //取得不合作通知函地址為NULL的資料
                //postlebeldatanull = postTwoMonthAddLebelService.GetLebelDataNullTwoMonth(endTime);
                postlebeldatanull = postTwoMonthAddLebelService.GetLebelDataNullTwoMonth();

                string lebelnull = "";

                if (postlebeldatanull.Rows.Count > 0)
                {
                    foreach (DataRow rowlebeldata in postlebeldatanull.Rows)
                    {
                        lebelnull = lebelnull + rowlebeldata["HCOP_MAILING_CITY"].ToString() + ";";
                    }
                    postTwoMonthAddLebelService.SendMail(jobID, "兩個月未結案HQ_WORK的HCOP_MAILING_CITY有區號為NULL", "兩個月未結案HQ_WORK的HCOP_MAILING_CITY有區號為NULL，縣市為" + lebelnull, "，請至SZIP TABLE維護", this.StartTime);
                }
                else
                {
                    // 取得開案兩個月之後都還沒做結案的資料
                    //20200806-RQ-2020-021027-001
                    //postData = postTwoMonthAddLebelService.GetAddressLebelDataTwoMonth(endTime);
                    postData = postTwoMonthAddLebelService.GetAddressLebelDataTwoMonth();

                    // 產檔及上傳到 MFTP
                    isComplete = GenaratorFileAndUploadToMFTP(jobID, fileName, postData, postTwoMonthAddLebelService, dateTime);

                    // 產檔及上傳到 MFTP
                    isComplete = GenaratorCtlFileAndUploadToMFTP(jobID, fileNamectl, postData, postTwoMonthAddLebelService, dateTime);

                    success = postData.Rows.Count;
                    if (isComplete)
                    {
                        total = postData.Rows.Count;

                        // 寫入 BatchLog
                        InsertBatchLog(jobID, total, success, "S", "");

                        // 發送 Email
                        //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
                        //postTwoMonthAddLebelService.SendMail(jobID, "傳送不合作信函地址條檔案 批次:" + fileName + " 上傳成功！", "總筆數:" + total, "上傳成功", this.StartTime);
                        postTwoMonthAddLebelService.SendMail(jobID, _MailTitle + "成功！總筆數：" + total + " 筆", "傳送不合作信函地址條檔案 批次:" + fileName + " 上傳成功！總筆數：" + total + " 筆", "上傳成功", this.StartTime);

                        manualresult = true;
                    }
                    else
                    {
                        // 寫入 BatchLog
                        InsertBatchLog(jobID, total, success, "S", "");

                        // 發送 Email
                        //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
                        //postTwoMonthAddLebelService.SendMail(jobID, "傳送不合作信函地址條檔案 批次:" + fileName + " 成功！", "無資料", "成功", this.StartTime);
                        postTwoMonthAddLebelService.SendMail(jobID, _MailTitle + "成功！總筆數：" + total + " 筆", "傳送不合作信函地址條檔案 批次:" + fileName + " 上傳成功！總筆數：" + total + " 筆", "上傳成功", this.StartTime);
                    }

                    JobHelper.Write(jobID, "[SUCCESS] 傳送不合作信函地址條檔案 批次 END " + fileName, LogState.Info);
                }
            }
        }
        catch (Exception ex)
        {
            // 寫入 BatchLog
            InsertBatchLog(jobID, total, success, "F", "發生錯誤：" + ex.Message);

            // 發送 Email
            //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
            //postTwoMonthAddLebelService.SendMail(jobID, "傳送不合作信函地址條檔案 批次:" + fileName + " 上傳失敗！", ex.Message, "上傳失敗", this.StartTime);
            postTwoMonthAddLebelService.SendMail(jobID, _MailTitle + "失敗！總筆數：" + total + " 筆", "傳送不合作信函地址條檔案 批次:" + fileName + " 上傳失敗！總筆數：" + total + " 筆，未產出０筆", "上傳失敗", this.StartTime);

            JobHelper.Write(jobID, "[FAIL] 傳送不合作信函地址條檔案 批次 " + fileName + " 發生錯誤：" + ex.Message);
        }
        finally
        {

            JobHelper.Write(jobID, " 傳送不合作信函地址條檔案 批次 Job 結束！", LogState.Info);
            JobHelper.Write(jobID, "================================================================================================================================================", LogState.Info);
        }
        return manualresult;
    }


    //public static DateTime AddBusinessDays( /*string connString,*/DateTime current, int days)
    //{
    //    //var holidayData = new RDIHolidayData(connString);
    //    //var allHolidays = holidayData.GetAllFederalHolidays(current.Year);
    //    int sign = Math.Sign(days);
    //    decimal unsignedDays = Math.Abs(days);
    //    for (int i = 0; i < unsignedDays; i++)
    //    {
    //        do
    //        {
    //            current = current.AddDays(sign);
    //        }
    //        while (current.DayOfWeek == DayOfWeek.Saturday || current.DayOfWeek == DayOfWeek.Sunday); // || allHolidays.Contains(current));
    //    }

    //    return current;
    //}

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
        PostTwoMonthAddLebelService postTwoMonthAddLebelService = new PostTwoMonthAddLebelService(jobID);
        int total = 0;
        int success = 0;
        bool result = false;
        try
        {
            DataTable postData = new DataTable();
            bool isComplete = false;

            JobHelper.Write(jobID, "*********** " + jobID + " 傳送地址條檔案 手動 START ************** ");

            // 取得應該傳送的地址條
            postData = postTwoMonthAddLebelService.GetSendToPostOfficeData(endTime);

            // 產檔及上傳到 MFTP
            isComplete = GenaratorFileAndUploadToMFTP(jobID, fileName, postData, postTwoMonthAddLebelService, dateTime);

            if (isComplete)
            {
                total = postData.Rows.Count;

                // 寫入 BatchLog
                InsertBatchLog(jobID, total, success, "S", "");

                // 發送 Email
                postTwoMonthAddLebelService.SendMail(jobID, "傳送地址條檔案 手動:" + fileName + " 上傳成功！", "總筆數:" + total, "上傳成功", this.StartTime);
            }
            else
            {
                // 寫入 BatchLog
                InsertBatchLog(jobID, total, success, "S", "");

                // 發送 Email
                postTwoMonthAddLebelService.SendMail(jobID, "傳送地址條檔案 手動:" + fileName + " 成功！", "無資料", "成功", this.StartTime);
            }

            result = true;

            JobHelper.Write(jobID, "[SUCCESS] 傳送地址條檔案 手動 END " + fileName);

        }
        catch (Exception ex)
        {
            // 發送 Email
            postTwoMonthAddLebelService.SendMail(jobID, "傳送地址條檔案 手動:" + fileName + " 上傳失敗！", ex.Message, "上傳失敗", this.StartTime);

            JobHelper.Write(jobID, "[FAIL] 傳送地址條檔案 手動 " + fileName + " 發生錯誤：" + ex.Message);
        }
        finally
        {
            JobHelper.Write(jobID, " 郵局核印 手動 Job 結束！");
            JobHelper.Write(jobID, "================================================================================================================================================");
        }

        return result;
    }
    */

    // 判斷Job工作狀態(0:停止 1:運行)
    private bool CheckJobIsContinue(string jobID, ref string msgID)
    {
        bool result = true;
        if (JobHelper.SerchJobStatus(jobID).Equals("") || JobHelper.SerchJobStatus(jobID).Equals("0"))
        {
            // Job停止
            JobHelper.Write(jobID, "[FAIL] Job工作狀態為：停止！", LogState.Info);
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
    private bool GenaratorFileAndUploadToMFTP(string jobID, string fileName, DataTable postData, PostTwoMonthAddLebelService postTwoMonthAddLebelService, DateTime dateTime)
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
            postDataContent = CHT_WordPadLeftRight("", "L", 5, ' ');   // 1. 1 資料別          (固定值為1)
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
                postDataContent = CHT_WordPadLeftRight(row["ZIP_CODE"].ToString(), "L", 05, ' ');   // 1. 1 資料別          (固定值為1)
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
            postTwoMonthAddLebelService.CreateFile(path, fileName, sb.ToString());
            //postOfficeService.CreateFile(path, fileName, sb);

            // 上送FTP
            isUploadToFTP = postTwoMonthAddLebelService.UploadFileToMFTP(jobID, localFileInfo, path, fileName);

            if (isUploadToFTP)
            {
                JobHelper.Write(jobID, "FileName：" + fileName + "，上送FTP 成功", LogState.Info);
                isComplete = true;

                foreach (DataRow row in postData.Rows)
                {
                    postTwoMonthAddLebelService.updatelabelsended2(row["ID"].ToString());

                    //20200805-RQ-2020-021027-001 修改歷程顯示wording
                    //BRFORM_COLUMN.AML_NOTELOG(row["CASE_NO"].ToString(), "", "", "傳送地址條", "傳送兩個月未結案地址條成功");
                    BRFORM_COLUMN.AML_NOTELOG(row["CASE_NO"].ToString(), "", "", "傳送地址條", "紙本不合作通知函地址條成功");
                }
            }
            else
            {
                JobHelper.Write(jobID, "[FAIL] FileName：" + fileName + "，上送FTP 失敗");
                isComplete = true;
            }
            //}
            //return true;
        }
        return isComplete;
    }

    // 產檔及上傳到 MFTP
    private bool GenaratorCtlFileAndUploadToMFTP(string jobID, string fileName, DataTable postData, PostTwoMonthAddLebelService postTwoMonthAddLebelService, DateTime dateTime)
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
            postTwoMonthAddLebelService.CreateFile(path, fileName, sb.ToString());
            //postOfficeService.CreateFile(path, fileName, sb);

            // 上送FTP
            isUploadToFTP = postTwoMonthAddLebelService.UploadFileToMFTP(jobID, localFileInfo, path, fileName);

            if (isUploadToFTP)
            {
                JobHelper.Write(jobID, "FileName：" + fileName + "，上送FTP 成功", LogState.Info);
                isComplete = true;
            }
            else
            {
                JobHelper.Write(jobID, "[FAIL] FileName：" + fileName + "，上送FTP 失敗");
                isComplete = true;
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