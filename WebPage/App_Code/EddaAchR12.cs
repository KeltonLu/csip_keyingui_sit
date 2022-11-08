//******************************************************************
//*  作    者：Ares_Jack
//*  功能說明：EDDA_法金ACH他行自扣申請回覆檔
//*  創建日期：2022/09/23
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using CSIPCommonModel.BusinessRules;
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using CSIPNewInvoice.EntityLayer_new;
using Framework.Common.IO;
using Framework.Common.Logging;
using Framework.Common.Utility;
using Quartz;

public class EddaAchR12 : IJob
{
    private static readonly JobHelper JobHelper = new JobHelper();
    private readonly string _functionKey = UtilHelper.GetAppSettings("FunctionKey");
    private readonly DateTime _dateStart = DateTime.Now; // 開始時間
    private bool _isReRun; // 是否手動執行
    private static JobDataMap _jobDataMap = new JobDataMap();
    private static int impContentSuccess = 0; //匯入DB成功筆數
    private static int impContentFail = 0; //匯入DB失敗筆數

    public void Execute(JobExecutionContext context)
    {
        #region 初始化參數

        string jobId = string.Empty;
        string strMsgId = string.Empty;
        string batchLogErr = "";
        int impTotalSuccess = 0;
        int impTotalFail = 0;
        string jobStatus = string.Empty; // R：上一個排程執行中且未超過一小時(需略過)

        #endregion

        try
        {
            _jobDataMap = context.JobDetail.JobDataMap;
            jobId = _jobDataMap.GetString("jobid").Trim();
            JobHelper.strJobID = jobId;

            JobHelper.SaveLog(jobId + " JOB啟動", LogState.Info);

            string jobMailTo = _jobDataMap.GetString("mail").Trim();

            // 判斷JOB是否在執行中
            var isContinue = CheckJobIsContinue(jobId, _functionKey, _dateStart, ref strMsgId, ref jobStatus);
            if (!isContinue) return;

            impContentSuccess = 0;
            impContentFail = 0;

            // 開始批次作業
            DateTime dt = DateTime.Now;
            // 批次日期
            string jobDate = string.Format("{0:0000}{1:00}{2:00}", int.Parse(dt.Year.ToString()), dt.Month, dt.Day);

            // 檢查排程參數
            CheckParamAndReRun(context, ref jobDate);

            // 批次日期清單
            var jobDateList = new List<string>();

            // 若為手動執行(只執行單一個批次日期)
            if (_isReRun)
            {
                jobDateList.Add(jobDate);
            }
            else
            {
                // 取得執行批次日期清單
                GetJobDateList(ref jobDateList);
            }

            // 開始FTP下載與匯入作業
            foreach (var batchDate in jobDateList)
            {
                string logErrTmp = "";
                FnDomain(jobId, batchDate, jobMailTo, ref logErrTmp);
                if (logErrTmp != "無檔案")
                {
                    if (logErrTmp == "")
                    {
                        impTotalSuccess++;
                    }
                    else
                    {
                        impTotalFail++;
                        batchLogErr += logErrTmp;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // 寫入 BatchLog
            InsertBatchLog(jobId, _functionKey, _dateStart, impTotalSuccess, impTotalFail, "F", "發生錯誤：" + ex.Message);
            batchLogErr += ex.Message;
            JobHelper.SaveLog("EddaAchR12_發錯錯誤_" + ex);
        }
        finally
        {
            // 若上一個排程仍執行中且不超過一小時
            if (!jobStatus.Equals("R"))
            {
                #region 紀錄下次執行時間

                var strMsg = jobId + "執行於:" + DateTime.Parse(context.FireTimeUtc.ToString()).AddHours(8);
                if (context.NextFireTimeUtc.HasValue)
                {
                    strMsg += "  ;下次執行於:" + DateTime.Parse(context.NextFireTimeUtc.ToString()).AddHours(8);
                }

                JobHelper.SaveLog(strMsg, LogState.Info);

                #endregion

                #region 結束批次作業
                if (batchLogErr == "")
                {
                    InsertBatchLog(jobId, _functionKey, _dateStart, impTotalSuccess, impTotalFail, "S", "");
                }
                else
                {
                    InsertBatchLog(jobId, _functionKey, _dateStart, impTotalSuccess, impTotalFail, "F", batchLogErr);
                }

                BRL_BATCH_LOG.Delete(_functionKey, jobId, "R");
                JobHelper.SaveLog(jobId + " JOB結束", LogState.Info);

                #endregion
            }
        }
    }

    /// <summary>
    /// 記錄【customer_log】
    /// </summary>
    /// <param name="otherBankCodeL">提回行代號</param>
    /// <param name="otherBankAccNo">委繳戶帳號</param>
    /// <param name="sDate">日期(eDDA申請之交易日期)</param>
    /// <param name="queryKey">Cus_ID</param>
    /// <param name="logFlag"></param>
    /// <param name="transId"></param>
    /// <returns></returns>
    private static void InsertCustomerLog(string otherBankCodeL, string otherBankAccNo, string sDate, string queryKey, string logFlag, string transId)
    {
        try
        {
            DataTable dt = CommonFunction.GetDataTable();
            CommonFunction.UpdateLog("", otherBankCodeL.Trim() + "-" + otherBankAccNo.Trim(), "銀行代碼 + 銀行帳號", dt);
            CommonFunction.UpdateLog("", sDate.Trim(), "日期", dt);

            EntityCUSTOMER_LOG eCustomerLog = new EntityCUSTOMER_LOG();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                eCustomerLog.query_key = queryKey; //搜尋關鍵字
                eCustomerLog.trans_id = transId; //交易代碼
                eCustomerLog.field_name = dt.Rows[i][EntityCUSTOMER_LOG.M_field_name].ToString(); //欄位名稱
                eCustomerLog.before = dt.Rows[i][EntityCUSTOMER_LOG.M_before].ToString(); //異動前
                eCustomerLog.after = dt.Rows[i][EntityCUSTOMER_LOG.M_after].ToString(); //異動後
                eCustomerLog.user_id = "EDDA";
                eCustomerLog.mod_date = DateTime.Now.ToString("yyyyMMdd");
                eCustomerLog.mod_time = DateTime.Now.ToString("HHmmss");
                eCustomerLog.log_flag = logFlag;
                BRCustomer_Log.AddEntity(eCustomerLog);
            }
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// 以批次日期(BatchDate)為條件刪除既有明細資料 EDDA_Auto_Pay
    /// </summary>
    /// <returns></returns>
    private static bool Delete_EDDA_Auto_Pay(string batchDate)
    {
        StringBuilder sbSql = new StringBuilder("DELETE FROM EDDA_Auto_Pay WHERE BatchDate = @BatchDate AND UploadFlag != 'Y' ");
        SqlCommand sqlcode = new SqlCommand { CommandType = CommandType.Text, CommandText = sbSql.ToString() };
        sqlcode.Parameters.Add(new SqlParameter("@BatchDate", batchDate)); //批次日期

        try
        {
            return BRBase<Entity_SP>.Delete(sqlcode, "Connection_System");
        }
        catch (Exception exp)
        {
            BRBase<Entity_SP>.SaveLog(exp.Message);
            return false;
        }
    }

    /// <summary>
    /// 核印失敗資料匯入【Auto_Pay_Auth_Fail】
    /// </summary>
    /// <returns></returns>
    private static bool InsertAuto_Pay_Auth_Fail(string batchDate)
    {
        string sqlText = @"DELETE FROM Auto_Pay_Auth_Fail WHERE UploadFlag <> 'Y' AND DataType = '0' AND BatchDate = @BatchDate ;
                        INSERT INTO Auto_Pay_Auth_Fail (BatchDate, SerialNumber, DataType, CustId, ErrorCode, IssueChannel, IssueDate, UploadFlag, CreateDate)
                        SELECT A.BatchDate, A.AuthCode, '0', A.Cus_ID, A.Reply_Info, 'EDDA', A.S_DATE + 19110000, 'N', GETDATE()
                        FROM EDDA_Auto_Pay A
                        WHERE A.BatchDate = @BatchDate AND A.Reply_Info IN (SELECT EddaRtnCode FROM EDDA_Rtn_Info WHERE NeedSendHost = 'Y')
                              AND NOT EXISTS(SELECT * FROM Auto_Pay_Auth_Fail B WHERE B.SerialNumber = A.AuthCode AND B.CustId = A.Cus_ID AND B.BatchDate = @BatchDate);";
        SqlCommand sqlCommand = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlText };
        sqlCommand.Parameters.Add(new SqlParameter("@BatchDate", batchDate)); //批次日期

        try
        {
            return BRBase<Entity_SP>.Update(sqlCommand, "Connection_System");
        }
        catch (Exception exp)
        {
            BRBase<Entity_SP>.SaveLog(exp.Message);
            return false;
        }
    }

    /// <summary>
    /// 首錄尾錄寫進EDDA_ACHR12
    /// </summary>
    /// <param name="jobDate">批次日期</param>
    /// <param name="filePath">檔案資料夾路徑</param>
    /// <param name="fileName">檔案名稱(含副檔名)</param>
    /// <param name="impSuccess">匯入成功筆數</param>
    /// <param name="impFail">匯入失敗筆數</param>
    /// <param name="strMsg">錯誤訊息</param>
    /// <returns></returns>
    private static bool FileImportEddaAchR12(string jobDate, string filePath, string fileName, ref int impSuccess, ref int impFail, ref string strMsg)
    {
        string sqlText = @"INSERT INTO [EDDA_ACHR12]([BOF], [CDATA], [TDATE], [ReceivingUnitCode], [BOF_Remark], [EOF], [TCOUNT], [EOF_Remark], [BatchDate], [AchFlag], [CreateDate] ) 
                        VALUES (@BOF, @CDATA, @TDATE, @ReceivingUnitCode, @BOF_Remark, @EOF, @TCOUNT, @EOF_Remark, @BatchDate, @AchFlag, @CreateDate )";

        try
        {
            String openFile = filePath + fileName;

            #region 檢查檔案匯入檔是否存在

            if (!File.Exists(openFile))
            {
                strMsg = "檔案不存在(" + openFile + ")";
                return false;
            }

            #endregion

            #region 取得fmt格式

            Dictionary<string, int> fmtParam = GetImportFmtEDDA_ACHR12();
            int fmtTotalLen = 0;
            foreach (var fmt in fmtParam)
            {
                fmtTotalLen += fmt.Value;
            }

            #endregion

            #region 檢查內文長度是否與fmt格式相符

            string[] arrayFile = FileTools.Read(openFile);
            foreach (var t in arrayFile)
            {
                var rowText = t;
                if (t.StartsWith("BOF") || t.StartsWith("EOF"))
                {
                    rowText += t; // 首錄加尾錄

                    if (rowText.Length != fmtTotalLen)
                    {
                        strMsg = "檔案匯入長度不相符。";
                        return false;
                    }
                }
            }
            #endregion

            #region 開始匯入DB
            impSuccess = 0;
            impFail = 0;

            // 查詢DB資料是否存在
            string sqlSelect = "SELECT * FROM EDDA_ACHR12 WHERE BatchDate = @BatchDate ";
            SqlCommand sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlSelect };
            sqlComm.Parameters.Add(new SqlParameter("@BatchDate", jobDate)); // 批次日期

            DataSet ds = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");
            if (ds == null || ds.Tables[0].Rows.Count == 0 || ds.Tables[0].Rows[0][0].ToString() == "") // INSERT
            {
                var fileText = string.Empty;
                foreach (var t in arrayFile)
                {
                    if (t.StartsWith("BOF") || t.StartsWith("EOF"))
                    {
                        fileText += t; // 首錄加尾錄
                    }
                }
                sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlText };

                int initNum = 0;
                foreach (var fmt in fmtParam)
                {
                    string keyText = fmt.Key;
                    int keyLen = fmt.Value;
                    sqlComm.Parameters.Add(new SqlParameter("@" + keyText, fileText.Substring(initNum, keyLen).Trim()));
                    initNum += keyLen;
                }
                sqlComm.Parameters.Add(new SqlParameter("@BatchDate", jobDate)); // 批次日期
                sqlComm.Parameters.Add(new SqlParameter("@AchFlag", "Y")); // 法金核印收檔註記
                sqlComm.Parameters.Add(new SqlParameter("@CreateDate", DateTime.Now)); // 建立時間

                if (!BRBase<Entity_SP>.Update(sqlComm, "Connection_System"))
                {
                    impFail++;
                }
            }
            else // UPDATE
            {
                sqlText = @"UPDATE [EDDA_ACHR12] SET [BOF] = @BOF, [CDATA] = @CDATA, [TDATE] = @TDATE, [ReceivingUnitCode] = @ReceivingUnitCode, [BOF_Remark] = @BOF_Remark, [EOF] = @EOF, [TCOUNT] = @TCOUNT, [EOF_Remark] = @EOF_Remark, [AchFlag] = @AchFlag, [ModifierDate] = @ModifierDate WHERE BatchDate = @BatchDate ";

                string fileText = string.Empty;
                foreach (string t in arrayFile)
                {
                    if (t.StartsWith("BOF") || t.StartsWith("EOF"))
                    {
                        fileText += t; // 首錄加尾錄
                    }
                }
                sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlText };

                int initNum = 0;
                foreach (var fmt in fmtParam)
                {
                    string keyText = fmt.Key.ToString();
                    int keyLen = fmt.Value;
                    sqlComm.Parameters.Add(new SqlParameter("@" + keyText, fileText.Substring(initNum, keyLen)));
                    initNum += keyLen;
                }
                sqlComm.Parameters.Add(new SqlParameter("@BatchDate", jobDate)); // 批次日期
                sqlComm.Parameters.Add(new SqlParameter("@AchFlag", "Y")); // 法金核印收檔註記
                sqlComm.Parameters.Add(new SqlParameter("@ModifierDate", DateTime.Now)); // 修改時間

                if (!BRBase<Entity_SP>.Update(sqlComm, "Connection_System"))
                {
                    impFail++;
                }
            }

            #endregion

            return impFail == 0;
        }
        catch (Exception exp)
        {
            BRBase<Entity_SP>.SaveLog(exp.Message);
            return false;
        }
    }

    /// <summary>
    /// 明細錄寫進EDDA_Auto_Pay
    /// </summary>
    /// <param name="jobDate">批次日期</param>
    /// <param name="filePath">檔案資料夾路徑</param>
    /// <param name="fileName">檔案名稱(含副檔名)</param>
    /// <param name="impSuccess">匯入成功筆數</param>
    /// <param name="impFail">匯入失敗筆數</param>
    /// <param name="strMsg">錯誤訊息</param>
    /// <returns></returns>
    private static bool FileImportEddaAutoPay(string jobDate, string filePath, string fileName, ref int impSuccess, ref int impFail, ref string strMsg)
    {
        string sqlText = @"INSERT INTO [EDDA_Auto_Pay]([BatchDate], [TDATE], [Deal_S_No], [Deal_No], [Sponsor_ID], [Other_Bank_Code_L], [Other_Bank_Acc_No], [Other_Bank_Cus_ID], [Cus_ID], [Apply_Type], [S_DATE], [AuthCode], [S_Remark], [Deal_Type], [Reply_Info], [Remark], [CreateDate]) 
 VALUES (@BatchDate, @TDATE, @Deal_S_No, @Deal_No, @Sponsor_ID, @Other_Bank_Code_L, @Other_Bank_Acc_No, @Other_Bank_Cus_ID, @Cus_ID, @Apply_Type, @S_DATE, @AuthCode, @S_Remark, @Deal_Type, @Reply_Info, @Remark, @CreateDate)";

        try
        {
            var openFile = filePath + fileName;

            #region 檢查檔案匯入檔是否存在

            if (!File.Exists(openFile))
            {
                strMsg = "檔案不存在(" + openFile + ")";
                return false;
            }

            #endregion

            #region 取得fmt格式

            Dictionary<string, int> fmtParam = GetImportFmtEDDA_Auto_Pay();
            int fmtTotalLen = 0;
            foreach (var fmt in fmtParam)
            {
                fmtTotalLen += fmt.Value;
            }

            #endregion

            #region 檢查內文長度是否與fmt格式相符
            string TDATE = string.Empty; // 交易日期
            int TCOUNT = 0; // 總筆數
            int contentCount = 0; // 檔案明細實際筆數
            string[] arrayFile = FileTools.Read(openFile);
            foreach (var t in arrayFile)
            {
                if (t.Length != fmtTotalLen)
                {
                    strMsg = "檔案匯入長度不相符。";
                    return false;
                }
                if (t.StartsWith("BOF"))
                {
                    TDATE = t.Substring(9, 8);
                }
                if (t.StartsWith("EOF"))
                {
                    int.TryParse(t.Substring(4, 8), out TCOUNT);
                }
                if (!t.StartsWith("BOF") && !t.StartsWith("EOF"))
                {
                    contentCount++;
                }
            }

            // 檢核尾錄筆數與實際筆數是否相同
            if (TCOUNT != contentCount)
            {
                strMsg = "尾錄筆數與實際讀取檔案的資料筆數不同";
                return false;
            }

            #endregion

            #region 開始匯入DB
            impSuccess = 0;
            impFail = 0;

            // 查詢DB資料是否存在
            string sqlSelect = "SELECT * FROM EDDA_Auto_Pay WHERE BatchDate = @BatchDate ";
            SqlCommand sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlSelect };
            sqlComm.Parameters.Add(new SqlParameter("@BatchDate", jobDate)); // 批次日期

            DataSet ds = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");
            if (ds == null || ds.Tables[0].Rows.Count == 0 || ds.Tables[0].Rows[0][0].ToString() == "") // DB完全沒有符合批次日期的資料
            {
                foreach (var t in arrayFile)
                {
                    if (!t.StartsWith("BOF") && !t.StartsWith("EOF"))
                    {
                        sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlText };

                        int initNum = 0;
                        foreach (var fmt in fmtParam)
                        {
                            string keyText = fmt.Key.ToString();
                            int keyLen = fmt.Value;
                            if (t.Contains("Non-Reply") && keyText == "Reply_Info") //匯入檔內容 Remark 為 "Non-Reply" 時, Reply_Info 為 一格空白
                                keyLen = 1;
                            sqlComm.Parameters.Add(new SqlParameter("@" + keyText, t.Substring(initNum, keyLen).Trim()));
                            initNum += keyLen;
                        }
                        sqlComm.Parameters.Add(new SqlParameter("@TDATE", TDATE.Trim())); // 主檔交易日期
                        sqlComm.Parameters.Add(new SqlParameter("@BatchDate", jobDate)); // 批次日期
                        sqlComm.Parameters.Add(new SqlParameter("@CreateDate", DateTime.Now)); // 建立日期

                        if (BRBase<Entity_SP>.Update(sqlComm, "Connection_System"))
                        {
                            impSuccess++;

                            #region 紀錄 CustomerLog
                            InsertCustomerLog(t.Substring(19, 7).Trim(), t.Substring(26, 14).Trim(), t.Substring(71, 8).Trim(), t.Substring(50, 20).Trim(), "P4", "90000001");
                            #endregion
                        }
                        else
                        {
                            impFail++;
                        }
                    }
                }
            }
            else
            {
                foreach (var t in arrayFile)
                {
                    if (!t.StartsWith("BOF") && !t.StartsWith("EOF"))
                    {
                        // 查詢 UploadFlag = 'Y' 的資料, 比對檔案內容, 相同的不重複寫入
                        string sqlSelectEDDA_Auto_PayIsUpload = "SELECT [TDATE], [Deal_S_No], [Deal_No], [Sponsor_ID], [Other_Bank_Code_L], [Other_Bank_Acc_No], [Other_Bank_Cus_ID], [Cus_ID], [Apply_Type], [S_DATE], [AuthCode], [S_Remark], [Deal_Type], [Reply_Info] FROM EDDA_Auto_Pay WHERE UploadFlag = 'Y' AND BatchDate = @BatchDate ";
                        sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlSelectEDDA_Auto_PayIsUpload };
                        sqlComm.Parameters.Add(new SqlParameter("@BatchDate", jobDate)); // 批次日期
                        DataSet dsEDDA_Auto_PayIsUpload = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");
                        bool fileContentExists = false;
                        if (!(dsEDDA_Auto_PayIsUpload == null || dsEDDA_Auto_PayIsUpload.Tables[0].Rows.Count == 0 || dsEDDA_Auto_PayIsUpload.Tables[0].Rows[0][0].ToString() == ""))
                        {
                            foreach (DataRow dr in dsEDDA_Auto_PayIsUpload.Tables[0].Rows)
                            {
                                string drValues = string.Join("", dr.ItemArray);
                                if (drValues.Replace(" ", "") == (TDATE + t).Replace(" ", ""))
                                    fileContentExists = true;
                            }
                        }
                        // 檔案的內容存在 EDDA_Auto_Pay 同時 UploadFlag = 'Y'
                        if (fileContentExists)
                            continue;

                        // 寫入
                        sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlText };

                        int initNum = 0;
                        foreach (var fmt in fmtParam)
                        {
                            string keyText = fmt.Key.ToString();
                            int keyLen = fmt.Value;
                            if (t.Contains("Non-Reply") && keyText == "Reply_Info") //匯入檔內容 Remark 為 "Non-Reply" 時, Reply_Info 為 一格空白
                                keyLen = 1;
                            sqlComm.Parameters.Add(new SqlParameter("@" + keyText, t.Substring(initNum, keyLen)));
                            initNum += keyLen;
                        }
                        sqlComm.Parameters.Add(new SqlParameter("@TDATE", TDATE)); // 主檔交易日期
                        sqlComm.Parameters.Add(new SqlParameter("@BatchDate", jobDate)); // 批次日期
                        sqlComm.Parameters.Add(new SqlParameter("@CreateDate", DateTime.Now)); // 建立時間

                        if (BRBase<Entity_SP>.Update(sqlComm, "Connection_System"))
                        {
                            impSuccess++;
                        }
                        else
                        {
                            impFail++;
                        }
                    }
                }
            }

            #endregion

            return impFail == 0;
        }
        catch (Exception exp)
        {
            BRBase<Entity_SP>.SaveLog(exp.Message);
            return false;
        }
    }

    /// <summary>
    /// EDDA_ACHR12 FMT 格式參數
    /// </summary>
    /// <returns></returns>
    private static Dictionary<string, int> GetImportFmtEDDA_ACHR12()
    {
        Dictionary<string, int> fmtParam = new Dictionary<string, int>
        {
            {"BOF", 3},
            {"CDATA", 6},
            {"TDATE", 8},
            {"ReceivingUnitCode", 7},
            {"BOF_Remark", 96},
            {"EOF", 3},
            {"TCOUNT", 8},
            {"EOF_Remark", 109}
        };

        return fmtParam;
    }

    /// <summary>
    /// EDDA_Auto_Pay FMT 格式參數
    /// </summary>
    /// <returns></returns>
    private static Dictionary<string, int> GetImportFmtEDDA_Auto_Pay()
    {
        Dictionary<string, int> fmtParam = new Dictionary<string, int>
        {
            {"Deal_S_No", 6},
            {"Deal_No", 3},
            {"Sponsor_ID", 10},
            {"Other_Bank_Code_L", 7},
            {"Other_Bank_Acc_No", 14},
            {"Other_Bank_Cus_ID", 10},
            {"Cus_ID", 20},
            {"Apply_Type", 1},
            {"S_DATE", 8},
            {"AuthCode", 7},
            {"S_Remark", 20},
            {"Deal_Type", 1},
            {"Reply_Info", 2},
            {"Remark", 11},
        };

        return fmtParam;
    }

    /// <summary>
    /// EddaAchR12 寄信功能
    /// </summary>
    /// <param name="mailFrom">寄件人</param>
    /// <param name="mailTo">收件人</param>
    /// <param name="subject">信件標題</param>
    /// <param name="body">信件內文</param>
    /// <returns></returns>
    private static void SendMail(string mailFrom, string[] mailTo, string subject, string body)
    {
        var result = false;
        try
        {
            JobHelper.SaveLog("開始寄信！", LogState.Info);
            result = JobHelper.SendMail(mailFrom, mailTo, subject, body);
        }
        catch (Exception ex)
        {
            BRBase<Entity_SP>.SaveLog(ex.Message);
        }
        finally
        {
            if (result)
            {
                JobHelper.SaveLog("寄信成功！", LogState.Info);
            }
            else
            {
                JobHelper.SaveLog("寄信失敗！");
            }
        }
    }

    /// <summary>
    /// FTP下載與匯入作業
    /// </summary>
    /// <param name="strJobId">批次ID</param>
    /// <param name="jobDate">批次日期</param>
    /// <param name="jobMailTo">寄信通知人員</param>
    /// <param name="batchLogErr">錯誤訊息</param>
    private static void FnDomain(string strJobId, string jobDate, string jobMailTo, ref string batchLogErr)
    {
        JobHelper.strJobID = strJobId;

        JobHelper.SaveLog("----------------------------------------------------", LogState.Info);

        // 檔案下載資料夾路徑
        string folderPath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileDownload") + "\\" + strJobId + "\\";
        // 最新的檔案名稱
        string eddaAchR12File = string.Empty;
        // mail 主旨
        string mailSubject = "重要通知-EDDA_法金ACH他行自扣申請回覆檔";
        // 發件人
        string mailSender = UtilHelper.GetAppSettings("MailSender");
        // 收件人
        string[] mailTo = { "" };
        // 錯誤訊息
        string errorMsg;

        if (!string.IsNullOrWhiteSpace(jobMailTo))
        {
            mailTo = jobMailTo.Split(';');
        }

        batchLogErr += "";


        #region FTP Download
        var strMsgId = "FTP下載檔案";
        JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
        DataTable dt = BRM_FileInfo.GetFtpInfoByJobId(strJobId);
        if (dt != null && dt.Rows.Count > 0)
        {
            string ftpIp = dt.Rows[0]["FtpIP"].ToString().Trim();
            string ftpId = dt.Rows[0]["FtpUserName"].ToString().Trim();
            string ftpPwd = RedirectHelper.GetDecryptString(dt.Rows[0]["FtpPwd"].ToString()).Trim();
            string ftpPath = dt.Rows[0]["FtpPath"].ToString().Trim();
            string ftpFileName = dt.Rows[0]["FtpFileName"].ToString().Trim();

            FTPFactory ftpFactory = new FTPFactory(ftpIp, "", ftpId, ftpPwd, "21", ftpPath, "Y");

            // 法金檔案名稱中的日期為「批次日期減一天」
            string achFileDate = DateTime.ParseExact(jobDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces).AddDays(-1).ToString("yyyyMMdd");

            // 模糊查詢ftp上面特定的檔案名稱
            string fileNameRLike = ftpFileName.Replace("yyyyMMdd", achFileDate).Replace("HHmmss.txt", "*");

            // 取得路徑下檔案列表
            string[] fileList = ftpFactory.GetFileList(ftpPath, fileNameRLike);
            if (fileList == null)
            {
                batchLogErr = "FTP目標資料夾不存在：" + ftpPath;
                JobHelper.SaveLog(batchLogErr, LogState.Info);
                // 無檔案，寄信通知
                SendMail(mailSender, mailTo, mailSubject, "EDDA法金核印資料「無檔案」，檔案批次日期：" + jobDate);
                return;
            }

            int max = 0;
            foreach (var fileName in fileList)
            {
                if (string.IsNullOrWhiteSpace(fileName)) continue;

                int iFileListName;
                string[] splFileListName = fileName.Split('_');
                string hhmmss = splFileListName[3].Substring(0, 6);
                int.TryParse(hhmmss, out iFileListName);
                if (iFileListName <= max) continue;

                max = iFileListName;
                eddaAchR12File = fileName;
            }

            // 檔案名稱不為空白則進行FTP下載檔案
            if (!string.IsNullOrWhiteSpace(eddaAchR12File))
            {
                JobHelper.SaveLog(strMsgId + "：來源FTP目錄：" + ftpPath, LogState.Info);
                JobHelper.SaveLog(strMsgId + "：來源FTP檔名：" + eddaAchR12File, LogState.Info);
                JobHelper.SaveLog(strMsgId + "：目標目錄：" + folderPath, LogState.Info);
                JobHelper.SaveLog(strMsgId + "：目標檔名：" + eddaAchR12File, LogState.Info);

                if (ftpFactory.Download(ftpPath, eddaAchR12File, folderPath, eddaAchR12File))
                {
                    JobHelper.SaveLog(strMsgId + "：下載成功！", LogState.Info);
                }
                else
                {
                    JobHelper.SaveLog(strMsgId + "：" + eddaAchR12File + ",下載失敗！", LogState.Error);
                    batchLogErr = strMsgId + "：" + eddaAchR12File + ",下載失敗！；";
                }
            }
            else
            {
                // 無檔案，寄信通知
                SendMail(mailSender, mailTo, mailSubject, "EDDA法金核印資料「無檔案」，檔案批次日期：" + jobDate);

                // FTP無檔案
                JobHelper.SaveLog(jobDate + "無檔案", LogState.Info);
                string sqlText = string.Empty;

                sqlText = @"SELECT * FROM [EDDA_ACHR12] WHERE [BatchDate] = @BatchDate";

                SqlCommand sqlCommand = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = sqlText
                };
                sqlCommand.Parameters.Add(new SqlParameter("@BatchDate", jobDate)); //批次日期
                DataSet ds = BRBase<Entity_SP>.SearchOnDataSet(sqlCommand, "Connection_System");
                if (ds == null || ds.Tables[0].Rows.Count == 0) // 查無存在的批次日期記錄才新增
                {
                    sqlText = @"INSERT INTO [EDDA_ACHR12]([BOF], [CDATA], [TDATE], [ReceivingUnitCode], [BOF_Remark], [EOF], [TCOUNT], [EOF_Remark], [BatchDate], [AchFlag], [CreateDate]) 
                        VALUES ('BOF', 'ACHR12', '', '', '', 'EOF', '0', '', @BatchDate, 'N', @CreateDate)";

                    SqlCommand sqlCommEDDA_ACHR12 = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlText };
                    sqlCommEDDA_ACHR12.Parameters.Add(new SqlParameter("@BatchDate", jobDate)); // 批次日期
                    sqlCommEDDA_ACHR12.Parameters.Add(new SqlParameter("@CreateDate", DateTime.Now)); // 建立時間
                    if (!BRBase<Entity_SP>.Update(sqlCommEDDA_ACHR12, "Connection_System"))
                    {
                        batchLogErr = "EDDA_ACHR12 寫入失敗";
                        JobHelper.SaveLog(batchLogErr);
                        return;
                    }

                    batchLogErr = "無檔案";
                }

                batchLogErr = "無檔案";
                return;
            }
        }

        #endregion

        string sContentF = "<font size=\"2\">您好!(失敗)<BR><BR>　　您<font color=\"#FF0000\">" +
                           "</font>EDDA法金核印資料「收檔失敗」，檔名為<font color=\"#FF0000\">" + eddaAchR12File +
                           "</font>，提醒！請作後續追蹤。";

        if (batchLogErr != "")
        {
            SendMail(mailSender, mailTo, mailSubject + "收檔失敗", sContentF);
            return;
        }

        #region 刪除既有明細資料【EDDA_Auto_Pay】
        strMsgId = string.Format("批次日期 : {0}，刪除(EDDA_Auto_Pay)", jobDate);
        JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
        if (Delete_EDDA_Auto_Pay(jobDate))
        {
            JobHelper.SaveLog(strMsgId + "：成功！", LogState.Info);
        }
        else
        {
            errorMsg = strMsgId + "：" + eddaAchR12File + ",失敗！";
            JobHelper.SaveLog(errorMsg);
            // 執行寄信
            SendMail(mailSender, mailTo, mailSubject + errorMsg, errorMsg);
            batchLogErr += strMsgId + "：" + eddaAchR12File + ",失敗！";
            return;
        }
        #endregion

        #region 從檔案匯入資料
        string strImpMsg = "";
        int impSuccess = 0;
        int impFail = 0;
        // 匯入ACHR12首錄/尾錄資料
        strMsgId = string.Format("批次日期 : {0}，檔案匯入至EDDA_ACHR12(首錄/尾錄資料)", jobDate);
        JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
        if (FileImportEddaAchR12(jobDate, folderPath, eddaAchR12File, ref impSuccess, ref impFail, ref strImpMsg))
        {
            JobHelper.SaveLog(strMsgId + "：成功！", LogState.Info);
        }
        else
        {
            errorMsg = "檔案匯入：失敗！" + strImpMsg + "。";
            // 匯入失敗
            JobHelper.SaveLog("檔案匯入：失敗！" + strImpMsg + "。");
            // 執行寄信
            SendMail(mailSender, mailTo, mailSubject + errorMsg, errorMsg);

            batchLogErr += strMsgId + "：" + eddaAchR12File + ",失敗！；";
            return;
        }

        // 匯入印核資料明細
        strMsgId = string.Format("批次日期 : {0}，檔案匯入至EDDA_Auto_Pay", jobDate);
        JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
        if (FileImportEddaAutoPay(jobDate, folderPath, eddaAchR12File, ref impSuccess, ref impFail, ref strImpMsg))
        {
            JobHelper.SaveLog(strMsgId + "：成功！(匯入" + impSuccess + "筆。)", LogState.Info);
            impContentSuccess += impSuccess;
        }
        else
        {
            errorMsg = "檔案匯入：失敗！" + strImpMsg + "(成功:" + impSuccess + "筆，失敗:" + impFail + "筆。";
            // 匯入失敗
            JobHelper.SaveLog(errorMsg);
            // 執行寄信
            SendMail(mailSender, mailTo, mailSubject + strImpMsg, errorMsg);

            batchLogErr += strMsgId + "：" + eddaAchR12File + ",失敗！；";
            impContentSuccess += impSuccess;
            impContentFail += impFail;
            return;
        }

        #endregion

        #region 核印失敗資料匯入【Auto_Pay_Auth_Fail】
        strMsgId = string.Format("批次日期 : {0}，核印失敗資料匯入 Auto_Pay_Auth_Fail", jobDate);
        JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
        if (InsertAuto_Pay_Auth_Fail(jobDate))
        {
            JobHelper.SaveLog(strMsgId + "：成功！", LogState.Info);
        }
        else
        {
            errorMsg = strMsgId + ",失敗！";
            JobHelper.SaveLog(errorMsg);
            // 執行寄信
            SendMail(mailSender, mailTo, mailSubject + errorMsg, errorMsg);

            batchLogErr += strMsgId + ",失敗！";
        }
        #endregion
    }

    /// <summary>
    /// 判斷排程是否要繼續執行
    /// </summary>
    /// <param name="jobId">排程代號</param>
    /// <param name="strFunctionKey">功能代號</param>
    /// <param name="dateStart">排程開姞時間</param>
    /// <param name="msgId">訊息代碼(Common/XML/Message.xml)</param>
    /// <param name="rtnStatus">若判斷上一個排程仍執行中回傳值為「R」</param>
    /// <returns></returns>
    private static bool CheckJobIsContinue(string jobId, string strFunctionKey, DateTime dateStart, ref string msgId, ref string rtnStatus)
    {
        bool result = true;
        string jobStatus = JobHelper.SerchJobStatus(jobId);
        if (jobStatus.Equals("") || jobStatus.Equals("0"))
        {
            // Job停止
            JobHelper.Write(jobId, "【FAIL】 Job工作狀態為：停止！");

            result = false;
        }

        // 檢測Job是否在執行中
        try
        {
            DataTable dtInfo = BRL_BATCH_LOG.GetRunningDate(strFunctionKey, jobId, "R", ref msgId);
            if (dtInfo == null || dtInfo.Rows.Count > 0)
            {
                //判斷執行時間超過一小時
                if (dtInfo != null)
                {
                    DateTime tempDt;
                    DateTime.TryParse(dtInfo.Rows[0][2].ToString(), out tempDt);
                    TimeSpan ts = DateTime.Now.Subtract(tempDt);
                    var dayCount = ts.Hours; // 執行中紀錄的開始時間與當天執行時間相差小時
                    if (dayCount > 1)
                    {
                        BRL_BATCH_LOG.Delete(strFunctionKey, jobId, "R");
                        BRL_BATCH_LOG.InsertRunning(strFunctionKey, jobId, dateStart, "R", "");
                        return true;
                    }
                }
                JobHelper.SaveLog("JOB 工作狀態為：正在執行！", LogState.Info);
                // 返回不執行
                rtnStatus = "R";
                result = false;
            }
            else
            {
                // 記錄Job執行資訊
                BRL_BATCH_LOG.InsertRunning(strFunctionKey, jobId, dateStart, "R", "");
            }
        }
        catch (Exception ex)
        {
            result = false;
            JobHelper.Write(jobId, "【FAIL】" + ex);
        }

        return result;
    }

    /// <summary>
    /// 新增批次Log
    /// </summary>
    /// <param name="jobId">批次ID</param>
    /// <param name="strFunctionKey">功能代號</param>
    /// <param name="dateStart">批次開始時間</param>
    /// <param name="success">成功筆數</param>
    /// <param name="fail">失敗筆數</param>
    /// <param name="status">批次執行狀態</param>
    /// <param name="message">批次訊息</param>
    private static void InsertBatchLog(string jobId, string strFunctionKey, DateTime dateStart, int success, int fail,
        string status, string message)
    {
        StringBuilder sbMessage = new StringBuilder();
        sbMessage.Append("成功檔案數：" + success + "。"); //*成功檔案數
        sbMessage.Append("失敗檔案數：" + fail + "。"); //*失敗檔案數
        sbMessage.Append("明細檔匯入 成功：" + impContentSuccess + "筆，失敗：" + impContentFail + "筆。"); //*明細檔匯入筆數

        if (message.Trim() != "")
        {
            sbMessage.Append("失敗訊息：" + message); //*失敗訊息
        }

        BRL_BATCH_LOG.Delete(strFunctionKey, jobId, "R");
        BRL_BATCH_LOG.Insert(strFunctionKey, jobId, dateStart, status, sbMessage.ToString());

    }

    /// <summary>
    /// 取得法金ACHR12收檔資訊
    /// </summary>
    /// <returns></returns>
    private DataSet GetEddaAchr12Info()
    {
        string batchSql = @"SELECT (SELECT MAX(BatchDate) FROM EDDA_ACHR12 WHERE AchFlag = 'Y') AS AchDateY,
                                           (SELECT MIN(BatchDate) FROM EDDA_ACHR12 WHERE AchFlag = 'N') AS AchDateN";

        SqlCommand sqlComm = new SqlCommand
        {
            CommandType = CommandType.Text,
            CommandText = batchSql
        };

        return BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");
    }

    /// <summary>
    /// 取得執行批次日期清單
    /// </summary>
    /// <param name="jobDateList">執行批次日期清單</param>
    private void GetJobDateList(ref List<string> jobDateList)
    {
        var jobDate = DateTime.Now.ToString("yyyyMMdd"); // 預設批次日期(系統日期)

        // 取得法金ACHR12收檔資訊
        DataSet ds = GetEddaAchr12Info();
        if (ds == null || ds.Tables[0].Rows.Count == 0) // 無收檔資訊
        {
            jobDateList.Add(jobDate);
            return;
        }

        // 有收檔資訊
        string achDateY = ds.Tables[0].Rows[0]["AchDateY"].ToString().Trim(); // 法金「有」檔案最後日期
        string achDateN = ds.Tables[0].Rows[0]["AchDateN"].ToString().Trim(); // 法金「無」檔案最後日期

        // 判斷開始執行的批次日期
        if (!string.IsNullOrEmpty(achDateY) && achDateY.Length == 8)
        {
            jobDate = achDateY;
        }
        else if (!string.IsNullOrEmpty(achDateN) && achDateN.Length == 8)
        {
            jobDate = achDateN;
        }

        DateTime tempDt = DateTime.ParseExact(jobDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
        TimeSpan ts = DateTime.Now.Subtract(tempDt);
        int dayCount = ts.Days; // 批次日期與系統日期相差天數
        for (var i = 0; i <= dayCount; i++)
        {
            // 「批次日期與系統日期相差不等於0」且存在下載成功的日期「achDateY」，則由上次成功日期「achDateY」「加一天」開始
            if (dayCount != 0 && !string.IsNullOrEmpty(achDateY))
            {
                tempDt = tempDt.AddDays(1);
            }
            // 「批次日期與系統日期相差不等於0」且存在下載失敗日期「achDateN」開始
            else if (dayCount != 0 && !string.IsNullOrEmpty(achDateN) && i > 0)
            {
                tempDt = tempDt.AddDays(1);
            }

            jobDate = tempDt.ToString("yyyyMMdd");

            if (int.Parse(jobDate) <= int.Parse(DateTime.Now.ToString("yyyyMMdd")))
            {
                jobDateList.Add(jobDate);
            }
        }
    }

    /// <summary>
    /// 檢查排程參數
    /// </summary>
    /// <param name="context">JobExecutionContext</param>
    /// <param name="jobDate">批次日期</param>
    private void CheckParamAndReRun(JobExecutionContext context, ref string jobDate)
    {
        JobHelper.SaveLog("判斷是否手動輸入參數 啟動排程：開始！", LogState.Info);

        if (context.JobDetail.JobDataMap["param"] != null)
        {
            JobHelper.SaveLog("手動輸入參數啟動排程：是！", LogState.Info);
            JobHelper.SaveLog("檢核輸入參數：開始！", LogState.Info);

            string strParam = context.JobDetail.JobDataMap["param"].ToString();

            if (strParam.Length == 10) // 日期：2022/09/01
            {
                DateTime tempDt;
                if (DateTime.TryParse(strParam, out tempDt))
                {
                    JobHelper.SaveLog("檢核參數：成功！ 參數：" + strParam, LogState.Info);
                    // 手動輸入的參數日期
                    string paramDate = string.Format("{0:0000}{1:00}{2:00}", int.Parse(tempDt.Year.ToString()), tempDt.Month, tempDt.Day);
                    // 若系統日期「不等於」參數日期
                    if (jobDate != paramDate)
                    {
                        _isReRun = true;
                        jobDate = paramDate;
                    }
                }
                else
                {
                    JobHelper.SaveLog("檢核參數：異常！ 參數：" + strParam);
                    return;
                }
            }
            else
            {
                JobHelper.SaveLog("檢核參數：異常！ 參數：" + strParam);
                return;
            }

            JobHelper.SaveLog("檢核輸入參數：結束！", LogState.Info);
        }
        else
        {
            JobHelper.SaveLog("手動輸入參數啟動排程：否！", LogState.Info);
        }

        JobHelper.SaveLog("判斷是否手動輸入參數 啟動排程：結束！", LogState.Info);
    }
}