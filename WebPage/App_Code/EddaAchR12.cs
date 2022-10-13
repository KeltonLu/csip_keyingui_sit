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
    private readonly string _strFunctionKey =UtilHelper.GetAppSettings("FunctionKey");
    private static string _strJobId;
    private readonly DateTime _dateStart = DateTime.Now; // 開始時間
    private bool _isReRun; // 是否手動執行
    private static JobDataMap jobDataMap = new JobDataMap();

    public void Execute(JobExecutionContext context)
    {
        #region 初始化參數

        string strMsgId = string.Empty;
        string batchLogErr = "";
        int impTotalSuccess = 0;
        int impTotalFail = 0;
        bool executeFlag = false;

        #endregion

        try
        {
            jobDataMap = context.JobDetail.JobDataMap;
            _strJobId = jobDataMap.GetString("jobid").Trim();
            JobHelper.strJobID = _strJobId;

            JobHelper.SaveLog(_strJobId + " JOB啟動", LogState.Info);

            string jobMailTo = jobDataMap.GetString("mail").Trim();

            #region 檢測JOB是否在執行中

            // 判斷Job工作狀態(0:停止 1:運行)
            var isContinue = CheckJobIsContinue(_strJobId, _strFunctionKey, _dateStart, ref strMsgId);
            if (!isContinue)
            {
                return;
            }

            #endregion

            //*開始批次作業

            #region 功能

            executeFlag = true;

            DateTime dt = DateTime.Now;
            string jobDate = string.Format("{0:0000}{1:00}{2:00}", int.Parse(dt.Year.ToString()), dt.Month, dt.Day);

            #region 判斷是否手動設置參數啟動排程

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

            #endregion

            // 是否手動執行(只執行單一個批次日期)
            if (_isReRun)
            {
                string logErrTmp = "";
                FnDomain(_strJobId, jobDate, jobMailTo, ref logErrTmp);
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
            else
            {
                string batchSql = @"SELECT (SELECT MAX(BatchDate) FROM EDDA_ACHR12 WHERE AchFlag = 'Y') AS AchDateY,
                                           (SELECT MIN(BatchDate) FROM EDDA_ACHR12 WHERE AchFlag = 'N') AS AchDateN";

                SqlCommand sqlComm = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = batchSql
                };
                DataSet ds = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    #region【EDDA_ACHR12 無資料】
                    JobHelper.SaveLog("EDDA_ACHR12 查無資料。", LogState.Info);
                    jobDate = DateTime.Now.ToString("yyyyMMdd");//最新批次日期

                    string logErrTmp = "";
                    FnDomain(_strJobId, jobDate, jobMailTo, ref logErrTmp);
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
                    #endregion
                }
                else
                {
                    #region【EDDA_ACHR12 有資料】

                    string achDateY = ds.Tables[0].Rows[0]["AchDateY"].ToString().Trim(); // 法金「有」檔案最後日期
                    string achDateN = ds.Tables[0].Rows[0]["AchDateN"].ToString().Trim(); // 法金「無」檔案最後日期
                    // 判斷開始執行的批次日期
                    if (!string.IsNullOrEmpty(achDateY) && achDateY.Length == 8)
                    {
                        jobDate = string.Format("{0}/{1}/{2}", achDateY.Substring(0, 4), achDateY.Substring(4, 2), achDateY.Substring(6, 2));
                    }
                    else if (!string.IsNullOrEmpty(achDateN) && achDateN.Length == 8)
                    {
                        jobDate = string.Format("{0}/{1}/{2}", achDateN.Substring(0, 4), achDateN.Substring(4, 2), achDateN.Substring(6, 2));
                    }
                    else if (jobDate.Length == 8)
                    {
                        jobDate = string.Format("{0}/{1}/{2}", jobDate.Substring(0, 4), jobDate.Substring(4, 2), jobDate.Substring(6, 2));
                    }

                    DateTime tempDt;
                    DateTime.TryParse(jobDate, out tempDt);
                    TimeSpan ts = DateTime.Now.Subtract(tempDt);
                    int dayCount = ts.Days; // 批次日期與當天相差天數
                    
                    // BatchDate由上次成功日期加一天開始收檔至系統日期，若無上次成功日期則由最早收檔記錄「無檔案」日期開始收檔至系統日期
                    for (int i = 0; i <= dayCount; i++) 
                    {
                        // 若存在下載成功的日期「achDateY」，則由上次成功日期「achDateY」「加一天」開始
                        if (!string.IsNullOrEmpty(achDateY) && dayCount > 1)
                        {
                            tempDt = tempDt.AddDays(1);
                        }
                        // 此條件日期由「achDateN」開始
                        else if (!string.IsNullOrEmpty(achDateN) && dayCount > 1 && i > 0)
                        {
                            tempDt = tempDt.AddDays(1);
                        }

                        jobDate = tempDt.ToString("yyyyMMdd");

                        if (int.Parse(jobDate) <= int.Parse(DateTime.Now.ToString("yyyyMMdd")))
                        {
                            string logErrTmp = "";
                            FnDomain(_strJobId, jobDate, jobMailTo, ref logErrTmp);
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
                    #endregion
                }
            }
            #endregion
        }
        catch (Exception ex)
        {
            // 寫入 BatchLog
            InsertBatchLog(_strJobId, _strFunctionKey, _dateStart, impTotalSuccess, impTotalFail, "F", "發生錯誤：" + ex.Message);
            batchLogErr += ex.Message;
            JobHelper.SaveLog("EddaAchR12_發錯錯誤_" + ex.ToString(), LogState.Error);
        }
        finally
        {
            #region 紀錄下次執行時間

            string strMsg = _strJobId + "執行於:" + DateTime.Parse(context.FireTimeUtc.ToString()).AddHours(8).ToString();
            if (context.NextFireTimeUtc.HasValue)
            {
                strMsg += "  ;下次執行於:" + DateTime.Parse(context.NextFireTimeUtc.ToString()).AddHours(8).ToString();
            }

            JobHelper.SaveLog(strMsg, LogState.Info);

            #endregion

            #region 結束批次作業
            if (executeFlag)
            {
                if (batchLogErr == "")
                {
                    InsertBatchLog(_strJobId, _strFunctionKey, _dateStart, impTotalSuccess, impTotalFail, "S", "");
                }
                else
                {
                    InsertBatchLog(_strJobId, _strFunctionKey, _dateStart, impTotalSuccess, impTotalFail, "F", batchLogErr);
                }
            }

            BRL_BATCH_LOG.Delete(_strFunctionKey, _strJobId, "R");
            JobHelper.SaveLog(_strJobId + " JOB結束", LogState.Info);

            #endregion
        }
    }

    /// <summary>
    /// 記錄【customer_log】
    /// </summary>
    /// <param name="Other_Bank_Code_L">提回行代號</param>
    /// <param name="Other_Bank_Acc_No">委繳戶帳號</param>
    /// <param name="S_DATE">日期(eDDA申請之交易日期)</param>
    /// <param name="strQueryKey">Cus_ID</param>
    /// <param name="strLogFlag"></param>
    /// <param name="strTransId"></param>
    /// <returns></returns>
    public static bool InsertCustomerLog(string Other_Bank_Code_L, string Other_Bank_Acc_No, string S_DATE, string strQueryKey, string strLogFlag, string strTransId)
    {
        try
        {
            DataTable dtblUpdate = CommonFunction.GetDataTable();
            CommonFunction.UpdateLog("", Other_Bank_Code_L.Trim() + "-" + Other_Bank_Acc_No.Trim(), "銀行代碼 + 銀行帳號", dtblUpdate);
            CommonFunction.UpdateLog("", S_DATE.Trim(), "日期", dtblUpdate);

            EntityCUSTOMER_LOG eCustomerLog = new EntityCUSTOMER_LOG();
            for (int i = 0; i < dtblUpdate.Rows.Count; i++)
            {
                eCustomerLog.query_key = strQueryKey; //搜尋關鍵字
                eCustomerLog.trans_id = strTransId; //交易代碼
                eCustomerLog.field_name = dtblUpdate.Rows[i][EntityCUSTOMER_LOG.M_field_name].ToString(); //欄位名稱
                eCustomerLog.before = dtblUpdate.Rows[i][EntityCUSTOMER_LOG.M_before].ToString(); //異動前
                eCustomerLog.after = dtblUpdate.Rows[i][EntityCUSTOMER_LOG.M_after].ToString(); //異動後
                eCustomerLog.user_id = "EDDA";
                eCustomerLog.mod_date = DateTime.Now.ToString("yyyyMMdd");
                eCustomerLog.mod_time = DateTime.Now.ToString("HHmmss");
                eCustomerLog.log_flag = strLogFlag;
                BRCustomer_Log.AddEntity(eCustomerLog);
            }
        }
        catch
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 以批次日期(BatchDate)為條件刪除既有明細資料 EDDA_Auto_Pay
    /// </summary>
    /// <returns></returns>
    private static bool Delete_EDDA_Auto_Pay(string BatchDate)
    {
        StringBuilder sbSql = new StringBuilder("DELETE FROM EDDA_Auto_Pay WHERE BatchDate = @BatchDate AND UploadFlag != 'Y' ");
        SqlCommand sqlcode = new SqlCommand { CommandType = CommandType.Text, CommandText = sbSql.ToString() };
        sqlcode.Parameters.Add(new SqlParameter("@BatchDate", BatchDate)); //批次日期

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
    private static bool InsertAuto_Pay_Auth_Fail(string BatchDate)
    {
        string sqlText = @"DELETE FROM Auto_Pay_Auth_Fail WHERE UploadFlag <> 'Y' AND DataType = '0' AND BatchDate = @BatchDate ;
                        INSERT INTO Auto_Pay_Auth_Fail (BatchDate, SerialNumber, DataType, CustId, ErrorCode, IssueChannel, IssueDate, UploadFlag, CreateDate)
                        SELECT A.BatchDate, A.AuthCode, '0', A.Cus_ID, A.Reply_Info, 'EDDA', A.S_DATE, 'N', GETDATE()
                        FROM EDDA_Auto_Pay A
                        WHERE A.BatchDate = @BatchDate AND A.Reply_Info IN ('A1', 'A2', 'A3', 'A5', 'A6', 'A7', 'A8', 'AB', 'AC', 'AF', 'AG', 'AI', 'AJ')
                              AND NOT EXISTS(SELECT * FROM Auto_Pay_Auth_Fail B WHERE B.SerialNumber = A.AuthCode AND B.CustId = A.Cus_ID); ";
        SqlCommand sqlCommand = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlText };
        sqlCommand.Parameters.Add(new SqlParameter("@BatchDate", BatchDate)); //批次日期

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
    /// <param name="sFile_Path"></param>
    /// <param name="sEddaAchR12File"></param>
    /// <param name="impSuccess"></param>
    /// <param name="impFail"></param>
    /// <param name="strMsg"></param>
    /// <returns></returns>
    private static bool FileImportEDDA_ACHR12(String sFile_Path, String sEddaAchR12File, ref int impSuccess, ref int impFail, ref string strMsg)
    {
        string sqlText = @"INSERT INTO [EDDA_ACHR12]([BOF], [CDATA], [TDATE], [ReceivingUnitCode], [BOF_Remark], [EOF], [TCOUNT], [EOF_Remark], [BatchDate], [AchFlag], [CreateDate] ) 
                        VALUES (@BOF, @CDATA, @TDATE, @ReceivingUnitCode, @BOF_Remark, @EOF, @TCOUNT, @EOF_Remark, @BatchDate, @AchFlag, @CreateDate )";

        try
        {
            String openFile = sFile_Path + sEddaAchR12File;
            string[] arrEddaAchR12File = sEddaAchR12File.Split('_');

            #region 檢查檔案匯入檔是否存在

            if (!File.Exists(openFile))
            {
                strMsg = "檔案不存在(" + openFile + ")";
                return false;
            }

            #endregion

            #region 取得fmt格式

            Dictionary<String, int> fmtParam = GetImportFmtEDDA_ACHR12();
            int fmtTotalLen = 0;
            foreach (var fmt in fmtParam)
            {
                fmtTotalLen += fmt.Value;
            }

            #endregion

            #region 檢查內文長度是否與fmt格式相符

            string[] arrayFile = FileTools.Read(openFile);
            String rowText = string.Empty;
            foreach (var t in arrayFile)
            {
                rowText = t;
                if (t.StartsWith("BOF") || t.StartsWith("EOF"))
                {
                    rowText += t;//首錄加尾錄

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

            //查詢DB資料是否存在
            string sqlSelect = "SELECT * FROM EDDA_ACHR12 WHERE BatchDate = @BatchDate ";
            SqlCommand sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlSelect };
            sqlComm.Parameters.Add(new SqlParameter("@BatchDate", arrEddaAchR12File[1].Trim())); //建立日期

            DataSet ds = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");
            if (ds == null || ds.Tables[0].Rows.Count == 0 || ds.Tables[0].Rows[0][0].ToString() == "") //INSERT
            {
                string fileText = string.Empty;
                foreach (string t in arrayFile)
                {
                    if (t.StartsWith("BOF") || t.StartsWith("EOF"))
                    {
                        fileText += t;//首錄加尾錄

                    }
                }
                sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlText };

                int initNum = 0;
                foreach (var fmt in fmtParam)
                {
                    string keyText = fmt.Key.ToString();
                    int keyLen = fmt.Value;
                    sqlComm.Parameters.Add(new SqlParameter("@" + keyText, fileText.Substring(initNum, keyLen).Trim()));
                    initNum += keyLen;
                }
                sqlComm.Parameters.Add(new SqlParameter("@BatchDate", arrEddaAchR12File[1].ToString().Trim())); //批次日期, 直接取檔名上的日期
                sqlComm.Parameters.Add(new SqlParameter("@AchFlag", "Y")); //法金核印收檔註記
                sqlComm.Parameters.Add(new SqlParameter("@CreateDate", DateTime.Now)); //建立時間

                if (!BRBase<Entity_SP>.Update(sqlComm, "Connection_System"))
                {
                    impFail++;
                }
            }
            else //UPDATE
            {
                sqlText = @"UPDATE [EDDA_ACHR12] SET [BOF] = @BOF, [CDATA] = @CDATA, [TDATE] = @TDATE, [ReceivingUnitCode] = @ReceivingUnitCode, [BOF_Remark] = @BOF_Remark, [EOF] = @EOF, [TCOUNT] = @TCOUNT, [EOF_Remark] = @EOF_Remark, [AchFlag] = @AchFlag, [ModifierDate] = @ModifierDate WHERE BatchDate = @BatchDate ";

                string fileText = string.Empty;
                foreach (string t in arrayFile)
                {
                    if (t.StartsWith("BOF") || t.StartsWith("EOF"))
                    {
                        fileText += t;//首錄加尾錄

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
                sqlComm.Parameters.Add(new SqlParameter("@BatchDate", arrEddaAchR12File[1].ToString().Trim())); //批次日期, 直接取檔名上的日期
                sqlComm.Parameters.Add(new SqlParameter("@AchFlag", "Y")); //法金核印收檔註記
                sqlComm.Parameters.Add(new SqlParameter("@ModifierDate", DateTime.Now)); //修改時間

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
    /// <param name="sFile_Path"></param>
    /// <param name="sEddaAchR12File"></param>
    /// <param name="impSuccess"></param>
    /// <param name="impFail"></param>
    /// <param name="strMsg"></param>
    /// <returns></returns>
    private static bool FileImportEDDA_Auto_Pay(String sFile_Path, String sEddaAchR12File, ref int impSuccess, ref int impFail, ref string strMsg)
    {
        string sqlText = @"INSERT INTO [EDDA_Auto_Pay]([BatchDate], [TDATE], [Deal_S_No], [Deal_No], [Sponsor_ID], [Other_Bank_Code_L], [Other_Bank_Acc_No], [Other_Bank_Cus_ID], [Cus_ID], [Apply_Type], [S_DATE], [AuthCode], [S_Remark], [Deal_Type], [Reply_Info], [Remark], [CreateDate]) 
 VALUES (@BatchDate, @TDATE, @Deal_S_No, @Deal_No, @Sponsor_ID, @Other_Bank_Code_L, @Other_Bank_Acc_No, @Other_Bank_Cus_ID, @Cus_ID, @Apply_Type, @S_DATE, @AuthCode, @S_Remark, @Deal_Type, @Reply_Info, @Remark, @CreateDate)";

        try
        {
            String openFile = sFile_Path + sEddaAchR12File;
            string[] arrEddaAchR12File = sEddaAchR12File.Split('_');

            #region 檢查檔案匯入檔是否存在

            if (!File.Exists(openFile))
            {
                strMsg = "檔案不存在(" + openFile + ")";
                return false;
            }

            #endregion

            #region 取得fmt格式

            Dictionary<String, int> fmtParam = GetImportFmtEDDA_Auto_Pay();
            int fmtTotalLen = 0;
            foreach (var fmt in fmtParam)
            {
                fmtTotalLen += fmt.Value;
            }

            #endregion

            #region 檢查內文長度是否與fmt格式相符
            string TDATE = string.Empty; // 交易日期
            int TCOUNT = 0; // 總筆數
            int contentCount = 0; //檔案明細實際筆數
            string[] arrayFile = FileTools.Read(openFile);
            foreach (var t in arrayFile)
            {
                String rowText = t;
                if (rowText.Length != fmtTotalLen)
                {
                    strMsg = "檔案匯入長度不相符。";
                    return false;
                }
                if (t.StartsWith("BOF"))
                    TDATE = rowText.Substring(10, 8);
                if (t.StartsWith("EOF"))
                    int.TryParse(rowText.Substring(4, 8), out TCOUNT);
                if (!t.StartsWith("BOF") && !t.StartsWith("EOF"))
                    contentCount++;
            }

            //檢核尾錄筆數與實際筆數是否相同
            if (TCOUNT != contentCount)
            {
                strMsg = "尾錄筆數與實際讀取檔案的資料筆數不同";
                return false;
            }

            #endregion

            #region 開始匯入DB
            impSuccess = 0;
            impFail = 0;

            //查詢DB資料是否存在
            string sqlSelect = "SELECT * FROM EDDA_Auto_Pay WHERE BatchDate = @BatchDate ";
            SqlCommand sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlSelect };
            sqlComm.Parameters.Add(new SqlParameter("@BatchDate", arrEddaAchR12File[1].Trim())); //建立日期
            
            DataSet ds = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");
            if (ds == null || ds.Tables[0].Rows.Count == 0 || ds.Tables[0].Rows[0][0].ToString() == "") //DB完全沒有符合批次日期的資料
            {
                foreach (string t in arrayFile)
                {
                    string fileText = t;
                    if (!fileText.StartsWith("BOF") && !fileText.StartsWith("EOF"))
                    {
                        sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlText };

                        int initNum = 0;
                        foreach (var fmt in fmtParam)
                        {
                            string keyText = fmt.Key.ToString();
                            int keyLen = fmt.Value;
                            sqlComm.Parameters.Add(new SqlParameter("@" + keyText, fileText.Substring(initNum, keyLen).Trim()));
                            initNum += keyLen;
                        }
                        sqlComm.Parameters.Add(new SqlParameter("@TDATE", TDATE.Trim())); //主檔交易日期
                        sqlComm.Parameters.Add(new SqlParameter("@BatchDate", arrEddaAchR12File[1].ToString().Trim())); //批次日期, 直接取檔名上的日期
                        sqlComm.Parameters.Add(new SqlParameter("@CreateDate", DateTime.Now)); //建立日期

                        if (BRBase<Entity_SP>.Update(sqlComm, "Connection_System"))
                        {
                            impSuccess++;

                            #region 紀錄 CustomerLog
                            string strMsgId = string.Format("批次日期 : {0}，紀錄 CustomerLog", arrEddaAchR12File[1].ToString().Trim());
                            JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
                            if (!InsertCustomerLog(fileText.Substring(19, 7).Trim(), fileText.Substring(26, 14).Trim(), fileText.Substring(71, 8).Trim(), fileText.Substring(50, 20).Trim(), "P4", "90000001"))
                            {
                                JobHelper.SaveLog(strMsgId + " 失敗!", LogState.Error);
                            }
                            else
                            {
                                JobHelper.SaveLog(strMsgId + " 成功!", LogState.Info);
                            }
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
                foreach (string t in arrayFile)
                {
                    string fileText = t;
                    if (!fileText.StartsWith("BOF") && !fileText.StartsWith("EOF"))
                    {
                        //查詢 UploadFlag = 'Y' 的資料, 比對檔案內容, 相同的不重複寫入
                        string sqlSelectEDDA_Auto_PayIsUpload = "SELECT [TDATE], [Deal_S_No], [Deal_No], [Sponsor_ID], [Other_Bank_Code_L], [Other_Bank_Acc_No], [Other_Bank_Cus_ID], [Cus_ID], [Apply_Type], [S_DATE], [AuthCode], [S_Remark], [Deal_Type], [Reply_Info] FROM EDDA_Auto_Pay WHERE UploadFlag = 'Y' AND BatchDate = @BatchDate ";
                        sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlSelectEDDA_Auto_PayIsUpload };
                        sqlComm.Parameters.Add(new SqlParameter("@BatchDate", arrEddaAchR12File[1].ToString().Trim())); //批次日期, 直接取檔名上的日期
                        DataSet dsEDDA_Auto_PayIsUpload = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");
                        bool fileContentExists = false;
                        if (!(dsEDDA_Auto_PayIsUpload == null || dsEDDA_Auto_PayIsUpload.Tables[0].Rows.Count == 0 || dsEDDA_Auto_PayIsUpload.Tables[0].Rows[0][0].ToString() == ""))
                        {
                            foreach (DataRow dr in dsEDDA_Auto_PayIsUpload.Tables[0].Rows)
                            {
                                string drValues = string.Join("", dr.ItemArray);
                                if (drValues.Replace(" ", "") == (TDATE + fileText).Replace(" ", ""))
                                    fileContentExists = true;
                            }
                        }
                        //檔案的內容存在 EDDA_Auto_Pay 同時 UploadFlag = 'Y'
                        if (fileContentExists)
                            continue;

                        //寫入
                        sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlText };
                        
                        int initNum = 0;
                        foreach (var fmt in fmtParam)
                        {
                            string keyText = fmt.Key.ToString();
                            int keyLen = fmt.Value;
                            sqlComm.Parameters.Add(new SqlParameter("@" + keyText, fileText.Substring(initNum, keyLen)));
                            initNum += keyLen;
                        }
                        sqlComm.Parameters.Add(new SqlParameter("@TDATE", TDATE)); //主檔交易日期
                        sqlComm.Parameters.Add(new SqlParameter("@BatchDate", arrEddaAchR12File[1].ToString().Trim())); //批次日期, 直接取檔名上的日期
                        sqlComm.Parameters.Add(new SqlParameter("@CreateDate", DateTime.Now)); //建立時間

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
    private static Dictionary<String, int> GetImportFmtEDDA_ACHR12()
    {
        Dictionary<String, int> fmtParam = new Dictionary<String, int>
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
    private static Dictionary<String, int> GetImportFmtEDDA_Auto_Pay()
    {
        Dictionary<String, int> fmtParam = new Dictionary<String, int>
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
    /// <param name="strFrom">寄件人</param>
    /// <param name="sAddressee">收件人</param>
    /// <param name="strSubject">信件標題</param>
    /// <param name="strBody">信件內文</param>
    /// <returns></returns>
    private static bool EddaAchR12_SendMail(string strFrom, string[] sAddressee, string strSubject, string strBody)
    {
        try
        {
            JobHelper.SaveLog("開始寄信！", LogState.Info);
            if (JobHelper.SendMail(strFrom, sAddressee, strSubject, strBody))
            {
                JobHelper.SaveLog("寄信成功！", LogState.Info);

                return true;
            }

            JobHelper.SaveLog("寄信失敗！");

            return false;
        }
        catch (Exception exp)
        {
            BRBase<Entity_SP>.SaveLog(exp.Message);
            return false;
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

        string sFilePath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileDownload") + "\\" + strJobId + "\\";
        // 最新的檔案名稱
        string sEddaAchR12File = string.Empty;
        // mail 主旨
        string mailSubject = "重要通知-EDDA_法金ACH他行自扣申請回覆檔";
        // 發件人
        string mailSender = UtilHelper.GetAppSettings("MailSender");
        // 收件人
        string[] sAddressee = { "" };
        // 錯誤訊息
        string errMsg = string.Empty;

        #region 功能

        if (!string.IsNullOrWhiteSpace(jobMailTo))
        {
            sAddressee = jobMailTo.Split(';');
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

            // 模糊查詢ftp上面特定的檔案名稱
            string fileNameRLike = ftpFileName.Replace("yyyyMMdd", jobDate).Replace("HHmmss.txt", "*");

            // 取得路徑下檔案列表
            string[] fileList = ftpFactory.GetFileList(ftpPath, fileNameRLike);
            if (fileList == null)
            {
                batchLogErr = "FTP目標資料夾不存在：" + ftpPath;
                JobHelper.SaveLog(batchLogErr, LogState.Info);
                // 無檔案，寄信通知
                EddaAchR12_SendMail(mailSender, sAddressee, mailSubject, "EDDA法金核印資料「無檔案」，檔案批次日期：" + jobDate);
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
                sEddaAchR12File = fileName;
            }
            
            // 檔案名稱不為空白則進行FTP下載檔案
            if (!string.IsNullOrWhiteSpace(sEddaAchR12File))
            {
                JobHelper.SaveLog(strMsgId + "：來源FTP目錄：" + ftpPath, LogState.Info);
                JobHelper.SaveLog(strMsgId + "：來源FTP檔名：" + sEddaAchR12File, LogState.Info);
                JobHelper.SaveLog(strMsgId + "：目標目錄：" + sFilePath, LogState.Info);
                JobHelper.SaveLog(strMsgId + "：目標檔名：" + sEddaAchR12File, LogState.Info);

                if (ftpFactory.Download(ftpPath, sEddaAchR12File, sFilePath, sEddaAchR12File))
                {
                    JobHelper.SaveLog(strMsgId + "：下載成功！", LogState.Info);
                }
                else
                {
                    JobHelper.SaveLog(strMsgId + "：" + sEddaAchR12File + ",下載失敗！", LogState.Error);
                    batchLogErr = strMsgId + "：" + sEddaAchR12File + ",下載失敗！；";
                }
            }
            else
            {
                // 無檔案，寄信通知
                EddaAchR12_SendMail(mailSender, sAddressee, mailSubject, "EDDA法金核印資料「無檔案」，檔案批次日期：" + jobDate);

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
                if (ds == null || ds.Tables[0].Rows.Count== 0) // 查無存在的批次日期記錄才新增
                {
                    sqlText = @"INSERT INTO [EDDA_ACHR12]([BOF], [CDATA], [TDATE], [ReceivingUnitCode], [BOF_Remark], [EOF], [TCOUNT], [EOF_Remark], [BatchDate], [AchFlag], [CreateDate]) 
                        VALUES ('BOF', 'ACHR12', '', '', '', 'EOF', '0', '', @BatchDate, 'N', @CreateDate)";

                    SqlCommand sqlCommEDDA_ACHR12 = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlText };
                    sqlCommEDDA_ACHR12.Parameters.Add(new SqlParameter("@BatchDate", jobDate)); //批次日期
                    sqlCommEDDA_ACHR12.Parameters.Add(new SqlParameter("@CreateDate", DateTime.Now)); //建立時間
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

        #region 寄信訊息

        string sContentF = "<font size=\"2\">您好!(失敗)<BR><BR>　　您<font color=\"#FF0000\">" + 
                           "</font>EDDA法金核印資料「收檔失敗」，檔名為<font color=\"#FF0000\">" + sEddaAchR12File +
                           "</font>，提醒！請作後續追蹤。";

        #endregion

        if (batchLogErr != "")
        {
            EddaAchR12_SendMail(mailSender, sAddressee, mailSubject, sContentF);
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
            errMsg = strMsgId + "：" + sEddaAchR12File + ",失敗！";
            JobHelper.SaveLog(errMsg, LogState.Error);
            //執行寄信
            EddaAchR12_SendMail(mailSender, sAddressee, mailSubject, errMsg);
            batchLogErr += strMsgId + "：" + sEddaAchR12File + ",失敗！";
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
        if (FileImportEDDA_ACHR12(sFilePath, sEddaAchR12File, ref impSuccess, ref impFail, ref strImpMsg))
        {
            JobHelper.SaveLog(strMsgId + "：成功！", LogState.Info);
        }
        else
        {
            errMsg = "檔案匯入：失敗！" + strImpMsg + "。";
            //匯入失敗
            JobHelper.SaveLog("檔案匯入：失敗！" + strImpMsg + "。", LogState.Error);
            //執行寄信
            EddaAchR12_SendMail(mailSender, sAddressee, mailSubject, errMsg);

            batchLogErr += strMsgId + "：" + sEddaAchR12File + ",失敗！；";
            return;
        }

        // 匯入印核資料明細
        strMsgId = string.Format("批次日期 : {0}，檔案匯入至EDDA_Auto_Pay", jobDate);
        JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
        if (FileImportEDDA_Auto_Pay(sFilePath, sEddaAchR12File, ref impSuccess, ref impFail, ref strImpMsg))
        {
            JobHelper.SaveLog(strMsgId + "：成功！(匯入" + impSuccess + "筆。)", LogState.Info);
        }
        else
        {
            errMsg = "檔案匯入：失敗！" + strImpMsg + "(成功:" + impSuccess + "筆，失敗:" + impFail + "筆。";
            //匯入失敗
            JobHelper.SaveLog(errMsg, LogState.Error);
            //執行寄信
            EddaAchR12_SendMail(mailSender, sAddressee, mailSubject + strImpMsg, errMsg);

            batchLogErr += strMsgId + "：" + sEddaAchR12File + ",失敗！；";
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
            errMsg = strMsgId + ",失敗！";
            JobHelper.SaveLog(errMsg, LogState.Error);
            //執行寄信
            EddaAchR12_SendMail(mailSender, sAddressee, mailSubject, errMsg);
            batchLogErr += strMsgId + ",失敗！";
        }
        #endregion
        
        #endregion
    }


    // 判斷Job工作狀態(0:停止 1:運行)
    public static bool CheckJobIsContinue(string JobID, string strFunctionKey, DateTime dateStart, ref string msgID)
    {
        bool result = true;
        string jobStatus = JobHelper.SerchJobStatus(JobID);
        if (jobStatus.Equals("") || jobStatus.Equals("0"))
        {
            // Job停止
            JobHelper.Write(JobID, "【FAIL】 Job工作狀態為：停止！");

            result = false;
        }

        // 檢測Job是否在執行中
        try
        {
            DataTable dtInfo = BRL_BATCH_LOG.GetRunningDate(strFunctionKey, JobID, "R", ref msgID);
            if (dtInfo == null || dtInfo.Rows.Count > 0) //20210531_Ares_Stanley-修正Job執行檢核條件
            {
                JobHelper.SaveLog("JOB 工作狀態為：正在執行！", LogState.Info);
                // 返回不執行
                result = false;
            }
            else
            {
                // 記錄Job執行資訊
                BRL_BATCH_LOG.InsertRunning(strFunctionKey, JobID, dateStart, "R", "");
            }
        }
        catch (Exception ex)
        {
            result = false;
            JobHelper.Write(JobID, "【FAIL】" + ex.ToString());
        }

        return result;
    }


    public static void InsertBatchLog(string jobID, string strFunctionKey, DateTime dateStart, int success, int fail,
        string status, string message)
    {
        StringBuilder sbMessage = new StringBuilder();
        sbMessage.Append("成功檔案數：" + success.ToString() + "。"); //*成功檔案數
        sbMessage.Append("失敗檔案數：" + fail.ToString() + "。"); //*失敗檔案數

        if (message.Trim() != "")
        {
            sbMessage.Append("失敗訊息：" + message); //*失敗訊息
        }

        BRL_BATCH_LOG.Delete(strFunctionKey, jobID, "R");
        BRL_BATCH_LOG.Insert(strFunctionKey, jobID, dateStart, status, sbMessage.ToString());

    }
}