//******************************************************************
//*  作    者：Ares_Jack
//*  功能說明：EDDA_下載網銀申請資料檔
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


public class EddaEBankApplyData : Quartz.IJob
{
    private static readonly JobHelper JobHelper = new JobHelper();
    private readonly string _strFunctionKey = UtilHelper.GetAppSettings("FunctionKey");
    private static string _strJobId;
    private readonly DateTime _dateStart = DateTime.Now; //開始時間
    private bool _isReRun; //是否手動執行
    private static JobDataMap jobDataMap = new JobDataMap();

    public void Execute(JobExecutionContext context)
    {
        #region 初始化參數

        string strMsgId = string.Empty;
        string batchLogErr = "";
        int impTotalSuccess = 0;
        int impTotalFail = 0;
        bool executeFlag = false;
        int contentCount = 0; //網銀筆數
        #endregion

        try
        {
            jobDataMap = context.JobDetail.JobDataMap;
            _strJobId = jobDataMap.GetString("jobid").Trim();
            JobHelper.strJobID = _strJobId;

            JobHelper.SaveLog(_strJobId + " JOB啟動", LogState.Info);

            string jobMailTo = jobDataMap.GetString("mail").Trim();// 收件人
            string sAddresser = UtilHelper.GetAppSettings("MailSender");// 發件人
            string[] sAddressee = { "" };
            if (!string.IsNullOrWhiteSpace(jobMailTo))
            {
                sAddressee = jobMailTo.Split(';');
            }

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
            string jobDate = string.Format("{0:0000}{1:00}{2:00}", (Int32.Parse(dt.Year.ToString())), dt.Month, dt.Day);

            #region 判斷是否手動設置參數啟動排程

            JobHelper.SaveLog("判斷是否手動輸入參數 啟動排程：開始！", LogState.Info);

            if (context.JobDetail.JobDataMap["param"] != null)
            {
                JobHelper.SaveLog("手動輸入參數啟動排程：是！", LogState.Info);
                JobHelper.SaveLog("檢核輸入參數：開始！", LogState.Info);

                string strParam = context.JobDetail.JobDataMap["param"].ToString();

                if (strParam.Length == 10)  // 日期 例如：2022/09/01
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
                FnDomain(_strJobId, jobDate, jobMailTo, ref logErrTmp, ref contentCount);

                if (logErrTmp == "")
                {
                    impTotalSuccess++;
                }
                else
                {
                    impTotalFail++;
                    batchLogErr = logErrTmp;
                }
                if (logErrTmp == "無檔案")
                {
                    jobDate = "";
                    contentCount = 0;
                }
                // 異常資料記錄log並發Email通知
                AbnormalDataSendMail(jobDate, sAddresser, sAddressee, contentCount.ToString());
            }
            else
            {
                string batchSql = @"SELECT (SELECT MAX(BatchDate) FROM EDDA_ACHR12 WHERE EBankFlag = 'Y') AS EBankDateY,
                                           (SELECT MIN(BatchDate) FROM EDDA_ACHR12 WHERE EBankFlag = 'N') AS EBankDateN";

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
                    FnDomain(_strJobId, jobDate, jobMailTo, ref logErrTmp, ref contentCount);

                    if (logErrTmp == "")
                    {
                        impTotalSuccess++;
                    }
                    else
                    {
                        impTotalFail++;
                        batchLogErr = logErrTmp;
                    }
                    if (logErrTmp == "無檔案")
                    {
                        jobDate = "";
                        contentCount = 0;
                    }
                    #endregion
                    // 異常資料記錄log並發Email通知
                    AbnormalDataSendMail(jobDate, sAddresser, sAddressee, contentCount.ToString());
                }
                else
                {
                    #region【EDDA_ACHR12 有資料】
                    string jobDateMultiple = string.Empty; //執行成功的批次日期
                    string contentCountMultiple = string.Empty; //執行成功的核印筆數
                    
                    string eBankDateY = ds.Tables[0].Rows[0]["EBankDateY"].ToString().Trim(); // 網銀「有」檔案最後日期
                    string eBankDateN = ds.Tables[0].Rows[0]["EBankDateN"].ToString().Trim(); // 網銀「無」檔案最後日期
                    // 判斷開始執行的批次日期
                    if (!string.IsNullOrEmpty(eBankDateY) && eBankDateY.Length == 8)
                    {
                        jobDate = string.Format("{0}/{1}/{2}", eBankDateY.Substring(0, 4), eBankDateY.Substring(4, 2), eBankDateY.Substring(6, 2));
                    }
                    else if (!string.IsNullOrEmpty(eBankDateN) && eBankDateN.Length == 8)
                    {
                        jobDate = string.Format("{0}/{1}/{2}", eBankDateN.Substring(0, 4), eBankDateN.Substring(4, 2), eBankDateN.Substring(6, 2));
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
                        // 若存在下載成功的日期「eBankDateY」，則由上次成功日期「eBankDateY」「加一天」開始
                        if (!string.IsNullOrEmpty(eBankDateY) && dayCount > 1)
                        {
                            tempDt = tempDt.AddDays(1);
                        }
                        // 此條件日期由「eBankDateN」開始
                        else if (!string.IsNullOrEmpty(eBankDateN) && dayCount > 1 && i > 0)
                        {
                            tempDt = tempDt.AddDays(1);
                        }
                            
                        jobDate = tempDt.ToString("yyyyMMdd");

                        string logErrTmp = "";
                        FnDomain(_strJobId, jobDate, jobMailTo, ref logErrTmp, ref contentCount);

                        if (logErrTmp == "")
                        {
                            impTotalSuccess++;
                        }
                        else
                        {
                            impTotalFail++;
                            batchLogErr = logErrTmp;
                        }
                        if (logErrTmp == "無檔案")
                        {
                            jobDate = "";
                            contentCount = 0;
                        }

                        if (i == 0)
                        {
                            jobDateMultiple = jobDate;
                            contentCountMultiple = contentCount.ToString();
                        }
                        else
                        {
                            jobDateMultiple = string.Join(",", jobDateMultiple, jobDate);
                            contentCountMultiple = string.Join(",", contentCountMultiple, contentCount.ToString());
                        }
                        
                    }
                    #endregion
                    // 異常資料記錄log並發Email通知
                    AbnormalDataSendMail(jobDateMultiple, sAddresser, sAddressee, contentCountMultiple);
                }
            }
            
            #endregion
        }
        catch (Exception ex)
        {
            // 寫入 BatchLog
            InsertBatchLog(_strJobId, _strFunctionKey, _dateStart, impTotalSuccess, impTotalFail, "F", "發生錯誤：" + ex.Message);
            batchLogErr += ex.Message;
            JobHelper.SaveLog("EddaEBankApplyData_發錯錯誤_" + ex.ToString());
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
    /// <param name="accNoBank">銀行代碼</param>
    /// <param name="accNo">銀行帳號</param>
    /// <param name="payWay">繳費方式</param>
    /// <param name="applyDate">申請日期</param>
    /// <param name="applyTime">申請時間</param>
    /// <param name="salesChannel">推廣通路代碼</param>
    /// <param name="strQueryKey">關鍵字查詢</param>
    /// <param name="strLogFlag"></param>
    /// <param name="strTransId">交易代號</param>
    /// <returns></returns>
    public static bool InsertCustomerLog(string accNoBank, string accNo, string payWay, string applyDate, string applyTime, string salesChannel, string strQueryKey, string strLogFlag, string strTransId)
    {
        try
        {
            // 推廣通路
            salesChannel = salesChannel.Trim();
            switch (salesChannel)
            {
                case "04":
                    salesChannel = "04:網銀(包含官網)";
                    break;
                case "05":
                    salesChannel = "05:行銀";
                    break;
            }
            
            // 繳費方式
            payWay = payWay.Trim();
            switch (payWay)
            {
                case "0":
                    payWay = "0(繳全額)";
                    break;
                case "1":
                    payWay = "1(繳最低額)";
                    break;
            }

            DataTable dt = CommonFunction.GetDataTable();
            CommonFunction.UpdateLog("", accNoBank.Trim() + "-" + accNo.Trim(), "銀行代碼 + 銀行帳號", dt);
            CommonFunction.UpdateLog("", payWay, "繳費方式", dt);
            CommonFunction.UpdateLog("", applyDate.Trim() + "-" + applyTime.Trim(), "申請日期+時間", dt);
            CommonFunction.UpdateLog("", salesChannel, "推廣通路代碼", dt);

            EntityCUSTOMER_LOG eCustomerLog = new EntityCUSTOMER_LOG();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                eCustomerLog.query_key = strQueryKey; //搜尋關鍵字
                eCustomerLog.trans_id = strTransId; //交易代碼
                eCustomerLog.field_name = dt.Rows[i][EntityCUSTOMER_LOG.M_field_name].ToString(); //欄位名稱
                eCustomerLog.before = dt.Rows[i][EntityCUSTOMER_LOG.M_before].ToString(); //異動前
                eCustomerLog.after = dt.Rows[i][EntityCUSTOMER_LOG.M_after].ToString(); //異動後
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
    ///  整合【EDDA_Auto_Pay. ComparisonStatus = '2'】、【EDDA_EBApply_Fail】當天的異常資料記錄log並發Email通知
    /// </summary>
    /// <param name="jobDate">批次日期</param>
    /// <param name="sAddresser">發件人</param>
    /// <param name="sAddressee">收件人</param>
    /// <param name="contentCount">網銀筆數</param>
    private void AbnormalDataSendMail(string jobDate, string sAddresser, string[] sAddressee, string contentCount)
    {
        try
        {
            int EDDA_Auto_PayCount = 0; //核印筆數
            int comparisonStatusEqualTo2Count = 0; //缺少的網銀資料計算
            int EDDA_EBApply_FailCount = 0; //比對不符多出的網銀資料計算
            int total = 0;
            string[] arrJobDate = jobDate.Split(','); //批次日期
            string[] arrcontentCount = contentCount.Split(','); //網銀筆數
            string msg = string.Empty;
            int i = 0;
            foreach (string strJobDate in arrJobDate)
            {
                string sqlText = "SELECT * FROM EDDA_Auto_Pay WHERE BatchDate = @BatchDate";
                SqlCommand sqlCommAbnormal = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlText };
                sqlCommAbnormal.Parameters.Add(new SqlParameter("@BatchDate", strJobDate)); //建立日期
                DataSet dsAbnormal = BRBase<Entity_SP>.SearchOnDataSet(sqlCommAbnormal, "Connection_System");
                if (dsAbnormal == null || dsAbnormal.Tables[0].Rows.Count == 0 || dsAbnormal.Tables[0].Rows[0][0].ToString() == "")
                    EDDA_Auto_PayCount = 0;
                else
                    EDDA_Auto_PayCount = dsAbnormal.Tables[0].Rows.Count;

                sqlText = "SELECT * FROM EDDA_Auto_Pay WHERE ComparisonStatus = '2' AND PayWay = '0' AND (EBAuthCode IS NULL OR EBAuthCode = '') AND UploadFlag <> 'Y' AND BatchDate = @BatchDate";
                sqlCommAbnormal = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlText };
                sqlCommAbnormal.Parameters.Add(new SqlParameter("@BatchDate", strJobDate)); //建立日期
                dsAbnormal = BRBase<Entity_SP>.SearchOnDataSet(sqlCommAbnormal, "Connection_System");
                if (dsAbnormal == null || dsAbnormal.Tables[0].Rows.Count == 0 || dsAbnormal.Tables[0].Rows[0][0].ToString() == "")
                    comparisonStatusEqualTo2Count = 0;
                else
                    comparisonStatusEqualTo2Count = dsAbnormal.Tables[0].Rows.Count;

                sqlText = "SELECT * FROM EDDA_EBApply_Fail WHERE BatchDate = @BatchDate";
                sqlCommAbnormal = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlText };
                sqlCommAbnormal.Parameters.Add(new SqlParameter("@BatchDate", strJobDate)); //建立日期
                dsAbnormal = BRBase<Entity_SP>.SearchOnDataSet(sqlCommAbnormal, "Connection_System");
                if (dsAbnormal == null || dsAbnormal.Tables[0].Rows.Count == 0 || dsAbnormal.Tables[0].Rows[0][0].ToString() == "")
                    EDDA_EBApply_FailCount = 0;
                else
                    EDDA_EBApply_FailCount = dsAbnormal.Tables[0].Rows.Count;

                total += (EDDA_EBApply_FailCount + comparisonStatusEqualTo2Count);
                if (strJobDate != "")
                {
                    if (EDDA_EBApply_FailCount > 0 || comparisonStatusEqualTo2Count > 0)
                    {
                        msg += string.Format("\n\r EDDA網銀資料收檔異常【批次日期 : {0}】：核印 {1} 筆、網銀 {2} 筆 <BR>", strJobDate, EDDA_Auto_PayCount, arrcontentCount[i]);
                    }
                    else
                    {
                        msg += string.Format("\n\r EDDA網銀資料收檔正常【批次日期 : {0}】 <BR>", strJobDate);
                    }
                }
                
                comparisonStatusEqualTo2Count = 0;
                EDDA_EBApply_FailCount = 0;
                i++;
            }

            if (msg != "")
            {
                if (total > 0)
                {
                    JobHelper.SaveLog(msg, LogState.Error);
                    EddaEBankApplyData_SendMail(sAddresser, sAddressee, "重要通知-EDDA_下載網銀申請資料檔", msg);
                }
                else
                {
                    JobHelper.SaveLog(msg, LogState.Info);
                    EddaEBankApplyData_SendMail(sAddresser, sAddressee, "重要通知-EDDA_下載網銀申請資料檔", msg);
                }
            }
        }
        catch(Exception exp)
        {
            BRBase<Entity_SP>.SaveLog(exp.Message);
        }
    }

    /// <summary>
    /// 清除暫存資料【EDDA_EBApply_Temp】
    /// </summary>
    /// <returns></returns>
    private static bool Truncate_EDDA_EBApply_Temp()
    {
        StringBuilder sbSql = new StringBuilder("Truncate Table EDDA_EBApply_Temp");
        SqlCommand sqlcode = new SqlCommand { CommandType = CommandType.Text, CommandText = sbSql.ToString() };

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
    /// EDDA_Auto_Pay更新網銀資料
    /// </summary>
    /// <param name="impSuccess"></param>
    /// <param name="impFail"></param>
    /// <param name="strMsg"></param>
    /// <returns></returns>
    private static bool UpdateEDDA_Auto_Pay(string jobDate, ref int impSuccess, ref int impFail, ref string strMsg)
    {
        string sqlTextNormal = @"UPDATE A
					SET A.ApplyID          = B.ApplyID,
						A.AccNoBank        = B.AccNoBank,
						A.AccNo            = B.AccNo,
						A.EBAuthCode       = B.EBAuthCode,
						A.PayWay           = B.PayWay,
						A.EBApplyType      = B.EBApplyType,
						A.ApplyDate        = B.ApplyDate,
						A.ApplyTime        = B.ApplyTime,
						A.UserNo           = B.UserNo,
						A.AccID            = B.AccID,
						A.SalesChannel     = B.SalesChannel,
						A.SalesUnit        = B.SalesUnit,
						A.SalesEmpoNo      = B.SalesEmpoNo,
						A.MatainUser       = B.MatainUser,
						A.ComparisonStatus = '1' --資料狀態：正常
					FROM EDDA_Auto_Pay AS A
                    INNER JOIN EDDA_EBApply_Temp AS B ON A.AuthCode = B.EBAuthCode AND A.Cus_ID = B.ApplyID AND A.UploadFlag = '0'";

        try
        {
            #region 開始更新DB
            impSuccess = 0;
            impFail = 0;

            SqlCommand sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlTextNormal };
            if (BRBase<Entity_SP>.Update(sqlComm, "Connection_System"))
            {
                impSuccess++;
            }
            else
            {
                impFail++;
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
    /// 比對不符合 EDDA_Auto_Pay 的資料更新資料狀態【ComparisonStatus】的值為「2」
    /// </summary>
    /// <param name="impSuccess"></param>
    /// <param name="impFail"></param>
    /// <param name="strMsg"></param>
    /// <returns></returns>
    private static bool UpdateEDDA_Auto_PayComparisonStatus(string jobDate, ref int impSuccess, ref int impFail, ref string strMsg)
    {
        string sqlTextAbnormalR = @"UPDATE EDDA_Auto_Pay
                    SET ComparisonStatus = '2', --資料狀態：缺少網銀資料
                        PayWay = '0'            -- 全額扣繳
                    WHERE (EBAuthCode IS NULL OR EBAuthCode = '') AND UploadFlag = '0' ";

        try
        {
            #region 開始更新DB
            impSuccess = 0;
            impFail = 0;

            SqlCommand sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlTextAbnormalR };
            if (BRBase<Entity_SP>.Update(sqlComm, "Connection_System"))
            {
                impSuccess++;
            }
            else
            {
                impFail++;
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
    /// 比對不符合的網銀資料新增至【EDDA_EBApply_Fail】
    /// </summary>
    /// <param name="jobDate"></param>
    /// <param name="impSuccess"></param>
    /// <param name="impFail"></param>
    /// <param name="strMsg"></param>
    /// <returns></returns>
    private static bool InsertEDDA_EBApply_Fail(string jobDate, ref int impSuccess, ref int impFail, ref string strMsg)
    {
        string sqlTextAbnormalL = @"DELETE FROM  EDDA_EBApply_Fail WHERE BatchDate = @BatchDate;
                    INSERT INTO EDDA_EBApply_Fail
                    SELECT A.* , @BatchDate, GETDATE()
                    FROM EDDA_EBApply_Temp A
                    WHERE NOT EXISTS(SELECT * FROM EDDA_Auto_Pay B WHERE B.EBAuthCode = A.EBAuthCode AND B.ApplyID = A.ApplyID) ";

        try
        {
            #region 開始寫入DB
            impSuccess = 0;
            impFail = 0;

            SqlCommand sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlTextAbnormalL };
            sqlComm.Parameters.Add(new SqlParameter("@BatchDate", jobDate)); //建立日期
            if (BRBase<Entity_SP>.Update(sqlComm, "Connection_System"))
            {
                impSuccess++;
            }
            else
            {
                impFail++;
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
    /// 網銀申請資料匯入【EDDA_EBApply_Temp】
    /// </summary>
    /// <param name="strJobId"></param>
    /// <param name="sFile_Path"></param>
    /// <param name="sEddaEBankApplyDataFile"></param>
    /// <param name="impSuccess"></param>
    /// <param name="impFail"></param>
    /// <param name="strMsg"></param>
    /// <param name="contentCount"></param>
    /// <returns></returns>
    private static bool FileImportEDDA_EBApply_Temp(string strJobId, String sFile_Path, String sEddaEBankApplyDataFile, ref int impSuccess, ref int impFail, ref string strMsg, ref int contentCount)
    {
        string sqlText = @"INSERT INTO [EDDA_EBApply_Temp]([ApplyID], [AccNoBank], [AccNo], [EBAuthCode], [PayWay], [EBApplyType], [ApplyDate], [ApplyTime], [UserNo], [AccID], [SalesChannel], [SalesUnit], [SalesEmpoNo], [MatainUser]) 
                        VALUES (@ApplyID, @AccNoBank, @AccNo, @EBAuthCode, @PayWay, @EBApplyType, @ApplyDate, @ApplyTime, @UserNo, @AccID, @SalesChannel, @SalesUnit, @SalesEmpoNo, @MatainUser)";

        try
        {
            String openFile = sFile_Path + sEddaEBankApplyDataFile;

            #region 檢查檔案匯入檔是否存在

            if (!File.Exists(openFile))
            {
                strMsg = "檔案不存在(" + openFile + ")";
                return false;
            }

            #endregion

            #region 取得fmt格式
            Dictionary<String, int> fmtParam = GetImportFmtEDDA_EBApply_Temp();
            #endregion

            #region 確認檔案內容第一行記錄的「資料筆數」與實際讀取的資料筆數是否相同
            int TCOUNT = 0; // EDDA網銀資料總筆數
            contentCount = 0; //實際筆數
            string[] arrayFile = FileTools.Read(openFile);
            int.TryParse(arrayFile[0], out TCOUNT);
            contentCount = arrayFile.Length - 2;

            //檢核尾錄筆數與實際筆數是否相同
            if (TCOUNT != contentCount)
            {
                strMsg = "尾錄筆數與實際讀取檔案的資料筆數不同";
                return false;
            }

            #endregion

            #region 取得內文
            DataTable dt = new DataTable();

            using (StreamReader sr = new StreamReader(openFile, Encoding.Default))
            {
                foreach (var fmt in fmtParam)
                {
                    dt.Columns.Add(fmt.Key);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(',');
                    if (rows.Length > 1)
                    {
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < rows.Length; i++)
                        {
                            dr[i] = rows[i];
                        }
                        dt.Rows.Add(dr);
                    }
                }

                if (sr.BaseStream.Length > 0 && dt.Rows.Count == 0)
                {
                    strMsg = "格式錯誤";
                    return false;
                }

            }

            #endregion

            #region 開始匯入DB
            impSuccess = 0;
            impFail = 0;

            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                SqlCommand sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlText };
                int j = 0;
                foreach (var fmt in fmtParam)
                {
                    string keyText = fmt.Key.ToString();
                    sqlComm.Parameters.Add(new SqlParameter("@" + keyText, dt.Rows[i][j].ToString().Trim()));
                    j++;
                }

                if (!BRBase<Entity_SP>.Update(sqlComm, "Connection_System"))
                {
                    impFail++;
                }
                else
                {
                    impSuccess++;
                    #region 紀錄 CustomerLog
                    string strMsgId = string.Format("批次日期 : {0}，紀錄 CustomerLog", strJobId.Trim());
                    JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
                    if (!InsertCustomerLog(dt.Rows[i][1].ToString().Trim(), dt.Rows[i][2].ToString().Trim(), dt.Rows[i][4].ToString().Trim(), dt.Rows[i][6].ToString().Trim(), dt.Rows[i][7].ToString().Trim(), dt.Rows[i][10].ToString().Trim(), dt.Rows[i][0].ToString().Trim(), "P4", "90000002"))
                    {
                        JobHelper.SaveLog(strMsgId + " 失敗!", LogState.Error);
                    }
                    else
                    {
                        JobHelper.SaveLog(strMsgId + " 成功!", LogState.Info);
                    }
                    #endregion
                }
            }
            #endregion
            #region 查詢匯入筆數
            SqlCommand sqlCommSelect = new SqlCommand { CommandType = CommandType.Text, CommandText = "SELECT * FROM EDDA_EBApply_Temp " };
            DataSet ds = BRBase<Entity_SP>.SearchOnDataSet(sqlCommSelect, "Connection_System");
            if (ds == null || ds.Tables[0].Rows.Count == 0 || ds.Tables[0].Rows[0][0].ToString() == "")
                contentCount = 0;
            else
                contentCount = ds.Tables[0].Rows.Count;
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
    /// 更新 EDDA_ACHR12 網銀收擋註記
    /// </summary>
    /// <param name="sFile_Path"></param>
    /// <param name="sEddaEBankApplyDataFile"></param>
    /// <param name="impSuccess"></param>
    /// <param name="impFail"></param>
    /// <param name="strMsg"></param>
    /// <returns></returns>
    private static bool FileImportEDDA_ACHR12(String sFile_Path, String sEddaEBankApplyDataFile, ref int impSuccess, ref int impFail, ref string strMsg)
    {
        string sqlText = @"INSERT INTO [EDDA_ACHR12]([BOF], [CDATA], [TDATE], [ReceivingUnitCode], [BOF_Remark], [EOF], [TCOUNT], [EOF_Remark], [BatchDate], [EBankFlag], [CreateDate] ) 
                        VALUES ('BOF', 'ACHR12', '', '', '', 'EOF', '', '', @BatchDate, 'Y', @CreateDate )";
        

        try
        {
            String openFile = sFile_Path + sEddaEBankApplyDataFile;
            string[] arrEddaAchR12File = sEddaEBankApplyDataFile.Split('_');

            #region 更新EDDA_ACHR12
            impSuccess = 0;
            impFail = 0;

            //查詢DB資料是否存在
            string sqlSelect = "SELECT * FROM EDDA_ACHR12 WHERE BatchDate = @BatchDate ";
            SqlCommand sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlSelect };
            sqlComm.Parameters.Add(new SqlParameter("@BatchDate", arrEddaAchR12File[1].ToString().Substring(0, 8).Trim())); //建立日期

            DataSet ds = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");
            if (ds == null || ds.Tables[0].Rows.Count == 0 || ds.Tables[0].Rows[0][0].ToString() == "") //INSERT
            {
                string fileText = string.Empty;

                sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlText };
                sqlComm.Parameters.Add(new SqlParameter("@BatchDate", arrEddaAchR12File[1].ToString().Substring(0, 8).Trim())); //批次日期, 直接取檔名上的日期
                sqlComm.Parameters.Add(new SqlParameter("@CreateDate", DateTime.Now)); //建立時間

                if (!BRBase<Entity_SP>.Update(sqlComm, "Connection_System"))
                {
                    impFail++;
                }
            }
            else
            {
                sqlText = @"UPDATE EDDA_ACHR12 SET EBankFlag = 'Y', ModifierDate = @ModifierDate WHERE BatchDate = @BatchDate ";
                sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlText };
                sqlComm.Parameters.Add(new SqlParameter("@ModifierDate", DateTime.Now));
                sqlComm.Parameters.Add(new SqlParameter("@BatchDate", arrEddaAchR12File[1].ToString().Substring(0, 8).Trim())); //批次日期, 直接取檔名上的日期
                if (!BRBase<Entity_SP>.Update(sqlComm, "Connection_System"))
                {
                    impFail++;
                }
                else
                {
                    impSuccess++;
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
    /// EDDA_EBApply_Temp FMT 格式參數
    /// </summary>
    /// <returns></returns>
    private static Dictionary<String, int> GetImportFmtEDDA_EBApply_Temp()
    {
        Dictionary<String, int> fmtParam = new Dictionary<String, int>
        {
            {"ApplyID", 11},
            {"AccNoBank", 3},
            {"AccNo", 16},
            {"EBAuthCode", 7},
            {"PayWay", 1},
            {"EBApplyType", 1},
            {"ApplyDate", 8},
            {"ApplyTime", 6},
            {"UserNo", 11},
            {"AccID", 11},
            {"SalesChannel", 2},
            {"SalesUnit", 5},
            {"SalesEmpoNo", 0},//保留欄位
            {"MatainUser", 4}
        };

        return fmtParam;
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
    /// EddaEBankApplyData 寄信功能
    /// </summary>
    /// <param name="strFrom">寄件人</param>
    /// <param name="sAddressee">收件人</param>
    /// <param name="strSubject">信件標題</param>
    /// <param name="strBody">信件內文</param>
    /// <returns></returns>
    private static bool EddaEBankApplyData_SendMail(string strFrom, string[] sAddressee, string strSubject, string strBody)
    {
        try
        {
            JobHelper.SaveLog("開始寄信！", LogState.Info);
            if (JobHelper.SendMail(strFrom, sAddressee, strSubject, strBody))
            {
                JobHelper.SaveLog("寄信成功！", LogState.Info);

                return true;
            }

            JobHelper.SaveLog("寄信失敗！", LogState.Error);

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
    /// <param name="contentCount">網銀資料筆數</param>
    private static void FnDomain(string strJobId, string jobDate, string jobMailTo, ref string batchLogErr, ref int contentCount)
    {
        JobHelper.strJobID = strJobId;

        JobHelper.SaveLog("----------------------------------------------------", LogState.Info);

        string sFilePath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileDownload") + "\\" + strJobId + "\\";
        string sEddaEBankApplyDataFile = string.Empty;
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
            string fileNameRLike = ftpFileName.Replace("yyyyMMdd", jobDate) + "*";
            
            // 取得路徑下檔案列表
            string[] fileList = ftpFactory.GetFileList(ftpPath, fileNameRLike);
            if (fileList == null)
            {
                batchLogErr = "FTP目標資料夾不存在：" + ftpPath;
                JobHelper.SaveLog(batchLogErr, LogState.Info);
                return;
            }

            string newestFileName = string.Empty;
            bool okFileExist = false;
            foreach (string file in fileList)
            {
                // 判斷ok檔是否存在
                if (file == ftpFileName.Replace("yyyyMMdd", jobDate) + ".ok")
                    okFileExist = true;
                // 判斷txt檔是否存在
                if (file == ftpFileName.Replace("yyyyMMdd", jobDate) + ".txt")
                    newestFileName = file;
            }
            
            if (!okFileExist)
            {
                batchLogErr = "無檔案";
                errMsg += ftpFileName.Replace("yyyyMMdd", jobDate) + ".ok" + "不存在  <BR>";
                JobHelper.SaveLog(ftpFileName.Replace("yyyyMMdd", jobDate) + ".ok" + "不存在");
            }

            if (!string.IsNullOrEmpty(newestFileName))
            {
                //最新檔名
                sEddaEBankApplyDataFile = newestFileName;

                JobHelper.SaveLog(strMsgId + "：來源FTP目錄：" + ftpPath, LogState.Info);
                JobHelper.SaveLog(strMsgId + "：來源FTP檔名：" + sEddaEBankApplyDataFile, LogState.Info);
                JobHelper.SaveLog(strMsgId + "：目標目錄：" + sFilePath, LogState.Info);
                JobHelper.SaveLog(strMsgId + "：目標檔名：" + sEddaEBankApplyDataFile, LogState.Info);

                if (ftpFactory.Download(ftpPath, sEddaEBankApplyDataFile, sFilePath, sEddaEBankApplyDataFile))
                {
                    JobHelper.SaveLog(strMsgId + "：下載成功！", LogState.Info);
                }
                else
                {
                    JobHelper.SaveLog(strMsgId + "：" + sEddaEBankApplyDataFile + ",下載失敗！");
                    batchLogErr = strMsgId + "：" + sEddaEBankApplyDataFile + ",下載失敗！；";
                }
            }
            else
            {
                batchLogErr = "無檔案";
                errMsg += ftpFileName.Replace("yyyyMMdd", jobDate) + ".txt" + "不存在 <BR>";
                JobHelper.SaveLog(ftpFileName.Replace("yyyyMMdd", jobDate) + ".txt" + "不存在");
            }

            if (!okFileExist || string.IsNullOrEmpty(newestFileName))
            {
                EddaEBankApplyData_SendMail(mailSender, sAddressee, "重要通知-EDDA_下載網銀申請資料檔 檔案不存在", errMsg);
                return;
            }
        }

        #endregion

        #region 寄信訊息

        string sContentF = "<font size=\"2\">您好!(失敗)<BR><BR>　　您<font color=\"#FF0000\">" +
                           "</font>提出的授權檔”收檔失敗”，檔名為<font color=\"#FF0000\">" + sEddaEBankApplyDataFile +
                           "</font>，提醒！請作後續追蹤。";

        string sContentS = "<font size=\"2\">您好!(成功)<BR><BR>　　您<font color=\"#FF0000\">" +
                           "</font>提出的授權檔”收檔成功”，檔名為<font color=\"#FF0000\">" + sEddaEBankApplyDataFile +
                           "</font>，您已可執行後續作業。";

        #endregion

        if (batchLogErr != "")
        {
            EddaEBankApplyData_SendMail(mailSender, sAddressee, "重要通知-EDDA_下載網銀申請資料檔", sContentF);
            return;
        }

        #region 刪除既有資料主檔
        strMsgId = string.Format("批次日期 : {0}，刪除(EDDA_EBApply_Temp)", jobDate);
        JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
        if (Truncate_EDDA_EBApply_Temp())
        {
            JobHelper.SaveLog(strMsgId + "：成功！", LogState.Info);
        }
        else
        {
            JobHelper.SaveLog(strMsgId + "：" + sEddaEBankApplyDataFile + ",失敗！");
            batchLogErr += strMsgId + "：" + sEddaEBankApplyDataFile + ",失敗！";
            return;
        }

        #endregion

        #region 從檔案匯入資料
        string strImpMsg = "";
        int impSuccess = 0;
        int impFail = 0;
        errMsg = string.Empty;

        strMsgId = string.Format("批次日期 : {0}，檔案匯入至EDDA_EBApply_Temp", jobDate);
        JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
        if (FileImportEDDA_EBApply_Temp(strJobId, sFilePath, sEddaEBankApplyDataFile, ref impSuccess, ref impFail, ref strImpMsg, ref contentCount))
        {
            JobHelper.SaveLog(strMsgId + "：成功！(匯入" + impSuccess + "筆。)", LogState.Info);
        }
        else
        {
            //匯入失敗
            errMsg = strMsgId + "：失敗！" + strImpMsg + "(成功:" + impSuccess + "筆，失敗:" + impFail + "筆。)";
            JobHelper.SaveLog(errMsg, LogState.Error);
            //執行寄信
            EddaEBankApplyData_SendMail(mailSender, sAddressee, "重要通知-EDDA_下載網銀申請資料檔" + strImpMsg + " ,失敗！", errMsg);

            batchLogErr += strMsgId + "：" + sEddaEBankApplyDataFile + ",失敗！；";
            return;
        }

        #region 比對ACH核印資料並更新網銀資料至【EDDA_Auto_Pay】
        strMsgId = string.Format("批次日期 : {0}，更新網銀資料至 EDDA_Auto_Pay", jobDate);
        JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
        if (UpdateEDDA_Auto_Pay(jobDate, ref impSuccess, ref impFail, ref strImpMsg))
        {
            JobHelper.SaveLog(strMsgId + " ,成功！", LogState.Info);
        }
        else
        {
            //更新失敗
            errMsg = strMsgId + " ,失敗！";
            JobHelper.SaveLog(errMsg);
            //執行寄信
            EddaEBankApplyData_SendMail(mailSender, sAddressee, "重要通知-EDDA_下載網銀申請資料檔" + strMsgId + " ,失敗！", errMsg);

            batchLogErr += strMsgId + "：" + sEddaEBankApplyDataFile + ",失敗！；";
            return;
        }

        //比對不符合 EDDA_Auto_Pay 的資料更新資料狀態【ComparisonStatus】的值為「2」
        strMsgId = string.Format("批次日期 : {0}，更新比對不符合的資料至 EDDA_Auto_Pay", jobDate);
        JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
        if (UpdateEDDA_Auto_PayComparisonStatus(jobDate, ref impSuccess, ref impFail, ref strImpMsg))
        {
            JobHelper.SaveLog(strMsgId + " ,成功！", LogState.Info);
        }
        else
        {
            //更新失敗
            errMsg = strMsgId + " ,失敗！";
            JobHelper.SaveLog(errMsg, LogState.Error);
            //執行寄信
            EddaEBankApplyData_SendMail(mailSender, sAddressee, "重要通知-EDDA_下載網銀申請資料檔" + strMsgId + " ,失敗！", errMsg);

            batchLogErr += strMsgId + "：" + sEddaEBankApplyDataFile + ",失敗！；";
            return;
        }

        //比對不符合的網銀資料新增至【EDDA_EBApply_Fail】
        strMsgId = string.Format("批次日期 : {0}，新增網銀資料至 EDDA_EBApply_Fail", jobDate);
        JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
        if (InsertEDDA_EBApply_Fail(jobDate, ref impSuccess, ref impFail, ref strImpMsg))
        {
            JobHelper.SaveLog(strMsgId + " ,成功！", LogState.Info);
        }
        else
        {
            //更新失敗
            errMsg = strMsgId + " ,失敗！";
            JobHelper.SaveLog(errMsg, LogState.Error);
            //執行寄信
            EddaEBankApplyData_SendMail(mailSender, sAddressee, "重要通知-EDDA_下載網銀申請資料檔" + strMsgId + " ,失敗！", errMsg);

            batchLogErr += strMsgId + "：" + sEddaEBankApplyDataFile + ",失敗！；";
            return;
        }
        #endregion

        strMsgId = string.Format("批次日期 : {0}，更新EDDA_ACHR12", jobDate);
        JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
        if (FileImportEDDA_ACHR12(sFilePath, sEddaEBankApplyDataFile, ref impSuccess, ref impFail, ref strImpMsg))
        {
            JobHelper.SaveLog(strMsgId + " ,成功！", LogState.Info);
        }
        else
        {
            //更新失敗
            errMsg = strMsgId + " ,失敗！";
            JobHelper.SaveLog(errMsg, LogState.Error);
            //執行寄信
            EddaEBankApplyData_SendMail(mailSender, sAddressee, "重要通知-EDDA_下載網銀申請資料檔" + strMsgId + " ,失敗！", errMsg);

            batchLogErr += strMsgId + "：" + sEddaEBankApplyDataFile + ",失敗！；";
            return;
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