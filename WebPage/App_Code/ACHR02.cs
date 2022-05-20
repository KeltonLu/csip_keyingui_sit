//******************************************************************
//*  作    者：Ares Luke
//*  功能說明：stored procedure 移轉至批次
//*  創建日期：2020/12/22
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*Ares Luke          2021/03/18         20200031-CSIP EOS       調整資料連線指定connectionString
//*Ares Luke          2021/03/18         20200031-CSIP EOS       調整LOG紀錄筆數合計結果
//*Ares Luke          2021/04/12         20200031-CSIP EOS       修正邏輯與LOG訊息調整
//*Ares Stanley      2021/06/01                                               修正工作狀態檢核
//*******************************************************************

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using CSIPCommonModel.BusinessRules;
using CSIPNewInvoice.EntityLayer_new;
using Framework.Common.IO;
using Framework.Common.Logging;
using Framework.Common.Utility;
using Quartz;

/// <summary>
/// 專案代號:20200031-CSIP EOS
/// 功能說明:
/// 作    者:Ares Luke
/// 創建時間:2020/12/22
/// </summary>
public class ACHR02 : Quartz.IJob
{
    private static readonly JobHelper JobHelper = new JobHelper();
    private readonly string _strFunctionKey = ConfigurationManager.AppSettings["FunctionKey"].ToString();
    private static string _strJobId;
    private DateTime _dateStart = DateTime.Now; //開始時間

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
            JobDataMap jobDataMap = context.JobDetail.JobDataMap;
            _strJobId = jobDataMap.GetString("jobid").Trim();
            JobHelper.strJobID = _strJobId;

            JobHelper.SaveLog(_strJobId + " JOB啟動", LogState.Info);

            string jobMailTo = jobDataMap.GetString("mail").Trim();

            #region 檢測JOB是否在執行中

            // 判斷Job工作狀態(0:停止 1:運行)
            var isContinue = CheckJobIsContinue(_strJobId, _strFunctionKey, _dateStart, ref strMsgId);

            #endregion

            //*開始批次作業

            #region 功能

            if (isContinue)
            {
                executeFlag = true;
                //20200031-CSIP EOS Ares Luke 修改日期:2021/02/22 修改說明:業務需求將日期格式為民國年8碼
                //20200031-CSIP EOS Ares Luke 修改日期:2021/03/12 修改說明:業務需求 手動RERUN參數
                DateTime dt = DateTime.Now;
                string jobDate = string.Format("{0:0000}{1:00}{2:00}", (Int32.Parse(dt.Year.ToString()) - 1911),
                    dt.Month, dt.Day);

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
                            jobDate = string.Format("{0:0000}{1:00}{2:00}",
                                (Int32.Parse(tempDt.Year.ToString()) - 1911),
                                tempDt.Month, tempDt.Day);
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


                string batchSql =
                    @"SELECT Batch_no,DateInput,TotalCount FROM batch WHERE DateReceive=@sToday AND P02_flag ='Y' AND R02_flag='0'";

                SqlCommand sqlComm = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = batchSql
                };
                sqlComm.Parameters.Add(new SqlParameter("@sToday", jobDate));
                DataSet ds = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    JobHelper.SaveLog("查無資料。", LogState.Info);
                    return;
                }

                foreach (DataRow data in ds.Tables[0].Rows)
                {
                    // 筆數
                    string logErrTmp = "";
                    FnDomain(data, _strFunctionKey, _strJobId, jobDate, jobMailTo, ref logErrTmp);


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

            #endregion
        }
        catch (Exception ex)
        {
            // 寫入 BatchLog
            InsertBatchLog(_strJobId, _strFunctionKey, _dateStart, impTotalSuccess, impTotalFail, "F", "發生錯誤：" + ex.Message);
            JobHelper.SaveLog("ACHR02_發錯錯誤_" + ex.ToString(), LogState.Error);
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


            JobHelper.SaveLog(_strJobId + " JOB結束", LogState.Info);

            #endregion
        }
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:執行過程中斷更新Batch_log
    /// 作    者:Ares Luke
    /// 創建時間:2020/12/22
    /// </summary>
    /// <param name="dateStart">排程時間</param>
    /// <param name="strStatus">狀態</param>
    /// <param name="strRMsg">訊息</param>
    /// <param name="strFunctionKey">FunctionKey</param>
    /// <param name="strJobId">JobId</param>
    // public static void Batch_log(string strFunctionKey, string strJobId, DateTime dateStart, string strStatus,
    //     string strRMsg)
    // {
    //     BRL_BATCH_LOG.Delete(strFunctionKey, strJobId, "R");
    //     BRL_BATCH_LOG.Insert(strFunctionKey, strJobId, dateStart, strStatus, strRMsg);
    // }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:清空 Truncate ACHR02_Tmp
    /// 作    者:Ares Luke
    /// 創建時間:2020/12/22
    /// </summary>
    /// <returns></returns>
    private static bool Truncate_ACHR02_Tmp()
    {
        StringBuilder sbSql = new StringBuilder("Truncate Table ACHR02_Tmp");
        SqlCommand sqlcode = new SqlCommand {CommandType = CommandType.Text, CommandText = sbSql.ToString()};

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
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:新增LOG至ACHFileIO_Log
    /// 作    者:Ares Luke
    /// 創建時間:2020/12/22
    /// </summary>
    /// <param name="fileName">fileName參數</param>
    /// <param name="remark">remark參數</param>
    private static bool Insert_ACHFileIO_Log(string fileName, string remark)
    {
        try
        {
            string sqlText =
                @"insert into ACHFileIO_Log select convert(varchar(8), getdate(), 112), convert(varchar(8), getdate(), 114), @FileName,@Remark";
            SqlCommand sqlComm = new SqlCommand {CommandType = CommandType.Text, CommandText = sqlText};
            sqlComm.Parameters.Add(new SqlParameter("@FileName", fileName));
            sqlComm.Parameters.Add(new SqlParameter("@Remark", remark));
            return BRBase<Entity_SP>.Add(sqlComm);
        }
        catch (Exception exp)
        {
            BRBase<Entity_SP>.SaveLog(exp.Message);
            return false;
        }
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:檔案匯入至DB，格式參考FMT
    /// 作    者:Ares Luke
    /// 創建時間:2020/12/22
    /// </summary>
    /// <param name="sFile_Path">目錄路徑</param>
    /// <param name="sACHR02File">檔案名稱</param>
    /// <param name="strMsg">回傳訊息</param>
    /// <returns></returns>
    private static bool FileImportData(String sFile_Path, String sACHR02File, ref int impSuccess, ref int impFail,
        ref string strMsg)
    {
        string sqlText =
            @"INSERT INTO ACHR02_Tmp (Deal_S_No, Deal_No, Sponsor_ID, Other_Bank_Code_L, Other_Bank_Acc_No,
                                     Other_Bank_Cus_ID, Cus_ID, Apply_Type, S_DATE, S_Bank_ID, S_Remark, Deal_Type,
                                     Reply_Info, Remark)
            VALUES (@Deal_S_No, @Deal_No, @Sponsor_ID, @Other_Bank_Code_L, @Other_Bank_Acc_No, @Other_Bank_Cus_ID, @Cus_ID,
                    @Apply_Type, @S_DATE, @S_Bank_ID, @S_Remark, @Deal_Type, @Reply_Info, @Remark)";

        try
        {
            String openFile = sFile_Path + sACHR02File;

            #region 檢查檔案匯入檔是否存在

            if (!File.Exists(openFile))
            {
                strMsg = "檔案不存在(" + openFile + ")";
                return false;
            }

            #endregion

            #region 取得fmt格式

            Dictionary<String, int> fmtParam = GetImportFmt();
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
                String rowText = t.ToString();
                if (rowText.Length != fmtTotalLen)
                {
                    strMsg = "檔案匯入長度不相符。";
                    return false;
                }
            }

            #endregion

            #region 開始匯入DB

            impSuccess = 0;
            impFail = 0;

            foreach (string t in arrayFile)
            {
                string fileText = t;
                SqlCommand sqlComm = new SqlCommand {CommandType = CommandType.Text, CommandText = sqlText};

                int initNum = 0;
                foreach (var fmt in fmtParam)
                {
                    string keyText = fmt.Key.ToString();
                    int keyLen = fmt.Value;
                    sqlComm.Parameters.Add(new SqlParameter("@" + keyText, fileText.Substring(initNum, keyLen)));
                    initNum += keyLen;
                }

                if (BRBase<Entity_SP>.Add(sqlComm, "Connection_System"))
                {
                    if (!fileText.StartsWith("BOF") && !fileText.StartsWith("EOF"))
                    {
                        //業務需求: BOF表頭與EOF表尾不列入計算
                        impSuccess++;
                    }
                }
                else
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
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:ACHR02 FMT 格式參數
    /// 作    者:Ares Luke
    /// 創建時間:2020/12/22
    /// </summary>
    /// <returns></returns>
    private static Dictionary<String, int> GetImportFmt()
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
            {"S_Bank_ID", 7},
            {"S_Remark", 20},
            {"Deal_Type", 1},
            {"Reply_Info", 1},
            {"Remark", 12}
        };

        return fmtParam;
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:ACHR02 寄信功能並insert ACHFileIO_Log
    /// 作    者:Ares Luke
    /// 創建時間:2020/12/22
    /// </summary>
    /// <param name="strFrom">寄件人</param>
    /// <param name="sAddressee">收件人</param>
    /// <param name="strSubject">信件標題</param>
    /// <param name="strBody">信件內文</param>
    /// <param name="strAchFileIoLog">Insert_ACHFileIO_Log訊息</param>
    /// <returns></returns>
    private static bool ACHR02_SendMail(string strFrom, string[] sAddressee, string strSubject, string strBody,
        string strAchFileIoLog)
    {
        try
        {
            JobHelper.SaveLog("開始寄信！", LogState.Info);
            if (JobHelper.SendMail(strFrom, sAddressee, strSubject, strBody))
            {
                JobHelper.SaveLog("寄信成功！", LogState.Info);
                Insert_ACHFileIO_Log(strAchFileIoLog, "**** 郵件發送成功!! ***");
                return true;
            }

            JobHelper.SaveLog("寄信失敗！", LogState.Error);
            Insert_ACHFileIO_Log(strAchFileIoLog, "**** 郵件發送失敗!! ***");
            return false;
        }
        catch (Exception exp)
        {
            BRBase<Entity_SP>.SaveLog(exp.Message);
            return false;
        }
    }


    public static void FnDomain(DataRow dr1,
        string strFunctionKey,
        string strJobId,
        string jobDate,
        string jobMailTo,
        ref string batchLogErr)
    {
        JobHelper.strJobID = strJobId;

        JobHelper.SaveLog("----------------------------------------------------", LogState.Info);

        string sFilePath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileDownload") + "\\" +
                           strJobId + "\\";
        string sAchr02File = string.Empty;
        // Batch_no
        string sBatchNo = string.Empty;
        // 送檔日
        // string sdateSend = string.Empty;
        // 首錄日
        string sDateInput;
        // 收檔日
        string sDateReceive = string.Empty;
        // YYYYMMDD
        // string sToday = DateTime.Now.ToString("yyyyMMdd");
        string sToday = jobDate;
        // 00000000
        string sYMD = "00000000";
        // 總筆數
        int sTotalCount = 0;
        // 筆數
        // sCount = 0;
        // 發動者統一編號 03077208 
        string sSponsor_ID = "03077208";
        //回覆訊息
        string sReply_Info = string.Empty;
        //收件編號
        string sReceive_Number = string.Empty;
        // 發件人
        string sAddresser = UtilHelper.GetAppSettings("MailSender");
        // 收件人
        string[] sAddressee = {""};
        int dbCount = 0;


        #region 功能

        if (!string.IsNullOrWhiteSpace(jobMailTo))
        {
            sAddressee = jobMailTo.Split(';');
        }

        #region 刪除舊檔

        var strMsgId = "刪除舊檔";
        JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
        if (Directory.Exists(sFilePath))
        {
            foreach (string f in Directory.GetFileSystemEntries(sFilePath))
            {
                if (File.Exists(f))
                {
                    if (Path.GetExtension(f).ToLower() == "txt")
                    {
                        //刪除TXT副檔
                        File.Delete(f);
                        JobHelper.SaveLog(strMsgId + "：檔案", LogState.Info);
                    }
                }
            }
        }

        JobHelper.SaveLog(strMsgId + "：結束", LogState.Info);

        #endregion


        batchLogErr += "";


        SqlCommand sqlComm = new SqlCommand();

        sBatchNo = dr1["Batch_no"].ToString();
        sDateInput = dr1["DateInput"].ToString();
        sTotalCount = Convert.ToInt32(dr1["TotalCount"].ToString());

        #region FTP Download AND Upload

        strMsgId = "FTP下載檔案";
        JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
        DataTable tblFileInfo = BRAML_File_Import.GetFileInfo(strJobId);
        if (tblFileInfo != null && tblFileInfo.Rows.Count > 0)
        {
            string ftpIp = tblFileInfo.Rows[0]["FtpIP"].ToString();
            string ftpId = tblFileInfo.Rows[0]["FtpUserName"].ToString();
            string ftpPwd = RedirectHelper.GetDecryptString(tblFileInfo.Rows[0]["FtpPwd"].ToString());
            string ftpPath = tblFileInfo.Rows[0]["FtpPath"].ToString();
            string ftpFileName = tblFileInfo.Rows[0]["FtpFileName"].ToString();

            sAchr02File = ftpFileName.Replace("sDateInput", sDateInput).Replace("sSponsor_ID", sSponsor_ID);
            FTPFactory objFtp = new FTPFactory(ftpIp, "", ftpId, ftpPwd, "21", ftpPath, "Y");

            JobHelper.SaveLog(strMsgId + "：來源FTP目錄：" + ftpPath, LogState.Info);
            JobHelper.SaveLog(strMsgId + "：來源FTP檔名：" + sAchr02File, LogState.Info);
            JobHelper.SaveLog(strMsgId + "：目標目錄：" + sFilePath, LogState.Info);
            JobHelper.SaveLog(strMsgId + "：目標檔名：" + sAchr02File, LogState.Info);

            if (objFtp.Download(ftpPath, sAchr02File, sFilePath, sAchr02File))
            {
                JobHelper.SaveLog(strMsgId + "：下載成功！", LogState.Info);
                //20210901_Ares_Stanley-註解FTP檔案上傳，待釐清是否需要後再做調整
                //strMsgId = "FTP上傳檔案";
                //JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
                //if (tblFileInfo != null && tblFileInfo.Rows.Count > 0)
                //{
                //    ftpIp = tblFileInfo.Rows[0]["FtpIP"].ToString();
                //    ftpId = tblFileInfo.Rows[0]["FtpUserName"].ToString();
                //    ftpPwd = RedirectHelper.GetDecryptString(tblFileInfo.Rows[0]["FtpPwd"].ToString());
                //    ftpPath = tblFileInfo.Rows[0]["FtpPath"].ToString();
                //    ftpFileName = tblFileInfo.Rows[0]["FtpFileName"].ToString();

                //    sAchr02File = ftpFileName.Replace("sDateInput", sDateInput)
                //        .Replace("sSponsor_ID", sSponsor_ID);
                //    objFtp = new FTPFactory(ftpIp, "", ftpId, ftpPwd, "21", ftpPath, "Y");

                //    JobHelper.SaveLog(strMsgId + "：來源目錄：" + sFilePath, LogState.Info);
                //    JobHelper.SaveLog(strMsgId + "：來源檔名：" + sAchr02File, LogState.Info);
                //    JobHelper.SaveLog(strMsgId + "：目標FTP目錄：" + ftpPath, LogState.Info);
                //    JobHelper.SaveLog(strMsgId + "：目標FTP檔名：" + sAchr02File, LogState.Info);

                //    if (objFtp.Upload(ftpPath, sAchr02File, sFilePath + sAchr02File))
                //    {
                //        JobHelper.SaveLog(strMsgId + "：上傳成功！", LogState.Info);
                //    }
                //    else
                //    {
                //        JobHelper.SaveLog(strMsgId + "：" + sAchr02File + ",上傳失敗！", LogState.Error);
                //        batchLogErr += strMsgId + "：" + sAchr02File + ",上傳失敗！；";
                //    }
                //}
            }
            else {
                JobHelper.SaveLog(strMsgId + "：" + sAchr02File + ",下載失敗！", LogState.Error);
                batchLogErr += strMsgId + "：" + sAchr02File + ",下載失敗！；";
            }
        }

        #endregion

        #region 寄信訊息

        string sContentF = "<font size=\"2\">您好!(失敗)<BR><BR>　　您<font color=\"#FF0000\">" + sDateInput +
                           "</font>提出的授權檔”收檔失敗”，檔名為<font color=\"#FF0000\">" + sAchr02File +
                           "</font>，提醒！請作後續追蹤。";

        string sContentS = "<font size=\"2\">您好!(成功)<BR><BR>　　您<font color=\"#FF0000\">" + sDateInput +
                           "</font>提出的授權檔”收檔成功”，檔名為<font color=\"#FF0000\">" + sAchr02File +
                           "</font>，您已可執行後續作業。";

        #endregion

        if (batchLogErr != "")
        {
            ACHR02_SendMail(sAddresser, sAddressee, "重要通知-ACH信用卡自扣授權檔回覆通知~~", sContentF, sAchr02File);
            return;
        }

        #region 清空暫存檔

        strMsgId = "清空暫存(ACHR02_Tmp)";
        JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
        if (Truncate_ACHR02_Tmp())
        {
            JobHelper.SaveLog(strMsgId + "：成功！", LogState.Info);
        }
        else
        {
            JobHelper.SaveLog(strMsgId + "：" + sAchr02File + ",失敗！", LogState.Error);
            batchLogErr += strMsgId + "：" + sAchr02File + ",失敗！";
            return;
        }

        #endregion

        #region 從檔案匯入資料

        strMsgId = "檔案匯入至ACHR02_TMP";
        JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);

        string strImpMsg = "";
        int impSuccess = 0;
        int impFail = 0;

        if (FileImportData(sFilePath, sAchr02File, ref impSuccess, ref impFail, ref strImpMsg))
        {
            JobHelper.SaveLog(strMsgId + "：成功！(匯入" + impSuccess + "筆。)", LogState.Info);
        }
        else
        {
            //匯入失敗
            Insert_ACHFileIO_Log(sAchr02File, "**** 檔案匯入失敗!! ****");
            JobHelper.SaveLog("檔案匯入：失敗！" + strImpMsg + "(成功:" + impSuccess + "筆，失敗:" + impFail + "筆。", LogState.Error);

            //更新batch的R02_flag狀態
            strMsgId = "Update Batch R02_flag 資料";
            JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
            sqlComm = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"UPDATE batch SET R02_flag='1' WHERE Batch_no = @sBatch_no"
            };
            sqlComm.Parameters.Add(new SqlParameter("@sBatch_no", sBatchNo));
            dbCount = 0;
            if (BRBase<Entity_SP>.UpdateWithCount(sqlComm, "Connection_System", ref dbCount))
            {
                JobHelper.SaveLog(string.Format("{0}:成功！(總共{1}筆)", strMsgId, dbCount), LogState.Info);
            }
            else
            {
                JobHelper.SaveLog(strMsgId + "：" + sAchr02File + ",失敗！", LogState.Error);
                batchLogErr += strMsgId + "："+ sAchr02File + ",失敗！；";
            }

            //執行寄信
            ACHR02_SendMail(sAddresser, sAddressee, "重要通知-ACH信用卡自扣授權檔回覆通知~~", sContentF, sAchr02File);

            strMsgId = "檔案匯入至ACHR02_TMP";
            JobHelper.SaveLog(strMsgId + "：" + sAchr02File + ",失敗！", LogState.Error);
            batchLogErr += strMsgId + "：" + sAchr02File + ",失敗！；";
            return;
        }

        #endregion

        strMsgId = "查詢 ACHR02_Tmp 總筆數";
        JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
        sqlComm = new SqlCommand
        {
            CommandType = CommandType.Text,
            CommandText = @"SELECT COUNT(0) AS COUNT FROM ACHR02_Tmp WHERE Deal_S_No NOT LIKE '%OF%'"
        };

        DataSet ds = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");

        int searchCount = 0;
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            searchCount = Convert.ToInt32(ds.Tables[0].Rows[0]["count"].ToString());
            JobHelper.SaveLog(string.Format("{0}:成功！(總共{1}筆)", strMsgId, searchCount), LogState.Info);
        }

        if (searchCount != sTotalCount)
        {
            Insert_ACHFileIO_Log(sAchr02File, "**** 檔案格式錯誤，匯入失敗!! ***");
            JobHelper.SaveLog("檔案格式錯誤，匯入失敗", LogState.Error);

            //執行寄信
            ACHR02_SendMail(sAddresser, sAddressee, "重要通知-ACH信用卡自扣授權檔回覆通知~~", sContentF, sAchr02File);
            JobHelper.SaveLog(sAchr02File + ",匯入失敗！", LogState.Error);
            batchLogErr += sAchr02File + ",匯入失敗！；";

            //更新batch
            strMsgId = "更新 Batch R02_flag='1'資料";
            JobHelper.SaveLog(strMsgId + "開始", LogState.Info);
            sqlComm = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"UPDATE batch SET R02_flag='1' WHERE Batch_no = @sBatch_no"
            };
            sqlComm.Parameters.Add(new SqlParameter("@sBatch_no", sBatchNo));
            dbCount = 0;
            if (BRBase<Entity_SP>.UpdateWithCount(sqlComm, "Connection_System", ref dbCount))
            {
                JobHelper.SaveLog(string.Format("{0}:成功！(總共{1}筆)", strMsgId, dbCount), LogState.Info);
            }
            else
            {
                JobHelper.SaveLog(strMsgId + "：" + sAchr02File + ",失敗！", LogState.Error);
                batchLogErr += strMsgId + "："+ sAchr02File + ",失敗！；";
            }

            return;
        }

        strMsgId = "查詢ACHR02_Tmp資料-Reply_Info異常筆數";
        JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
        sqlComm = new SqlCommand
        {
            CommandType = CommandType.Text,
            CommandText =
                @"SELECT COUNT(*) AS COUNT FROM ACHR02_Tmp WHERE Deal_S_No NOT LIKE '%OF%' AND Reply_Info = ' '"
        };
        ds = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");

        if (ds == null)
        {
            JobHelper.SaveLog(strMsgId + "：" + sAchr02File + ",失敗！", LogState.Error);
            batchLogErr += strMsgId + "：" + sAchr02File + ",失敗！；";
            return;
        }

        int conditionCount = (int) ds.Tables[0].Rows[0]["count"];
        JobHelper.SaveLog(string.Format("{0}：查詢成功！(總共{1}筆)", strMsgId, conditionCount), LogState.Info);

        if (conditionCount > 0)
        {
            Insert_ACHFileIO_Log(sAchr02File, "**** 檔案回覆訊息未填，匯入失敗!! ***");
            JobHelper.SaveLog(strMsgId + sAchr02File + ",檔案回覆訊息未填，匯入失敗！" + strMsgId, LogState.Error);
            batchLogErr += sAchr02File + ",檔案回覆訊息未填，匯入失敗；";

            //更新batch
            strMsgId = "更新 Batch R02_flag='1'資料";
            JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
            sqlComm = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"UPDATE batch SET R02_flag='1' WHERE Batch_no = @sBatch_no"
            };
            sqlComm.Parameters.Add(new SqlParameter("@sBatch_no", sBatchNo));
            dbCount = 0;
            if (BRBase<Entity_SP>.UpdateWithCount(sqlComm, "Connection_System", ref dbCount))
            {
                JobHelper.SaveLog(string.Format("{0}:成功！(總共{1}筆)", strMsgId, dbCount), LogState.Info);
            }
            else
            {
                JobHelper.SaveLog(strMsgId + "：" + sAchr02File + ",失敗！", LogState.Error);
                batchLogErr += strMsgId + "：" + sAchr02File  + ",失敗！；";
            }

            //執行寄信
            ACHR02_SendMail(sAddresser, sAddressee, "重要通知-ACH信用卡自扣授權檔回覆通知~~", sContentF, sAchr02File);

            return;
        }


        #region "查詢ACHR02_Tmp資料，更新Other_Bank_Temp

        strMsgId = "查詢ACHR02_Tmp資料";
        JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
        sqlComm = new SqlCommand
        {
            CommandType = CommandType.Text,
            CommandText =
                @"SELECT Reply_Info, LEFT(S_Remark, 13) AS Receive_Number FROM ACHR02_Tmp WHERE Deal_S_No NOT LIKE '%OF%'"
        };
        ds = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            JobHelper.SaveLog(string.Format("{0}：查詢成功！(總共{1}筆)", strMsgId, (int) ds.Tables[0].Rows.Count),
                LogState.Info);

            //更新Other_Bank_Temp
            strMsgId = "更新Other_Bank_Temp";
            JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
            int countNum = 0;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                sReply_Info = dr["Reply_Info"].ToString();
                sReceive_Number = dr["Receive_Number"].ToString();


                sqlComm = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText =
                        @"UPDATE Other_Bank_Temp SET ACH_Return_Code=@sReply_Info WHERE Receive_Number=@sReceive_Number AND Batch_no=@sBatch_no"
                };
                sqlComm.Parameters.Add(new SqlParameter("@sReply_Info", sReply_Info));
                sqlComm.Parameters.Add(new SqlParameter("@sReceive_Number", sReceive_Number));
                sqlComm.Parameters.Add(new SqlParameter("@sBatch_no", sBatchNo));

                dbCount = 0;
                if (BRBase<Entity_SP>.UpdateWithCount(sqlComm, "Connection_System", ref dbCount))
                {
                    countNum += dbCount;
                }
                else
                {
                    JobHelper.SaveLog(strMsgId + "：" + sAchr02File + ",失敗！", LogState.Error);
                    batchLogErr += strMsgId + "：" + sAchr02File  + ",失敗！；";
                    return;
                }
            }

            JobHelper.SaveLog(string.Format("{0}:成功！(總共{1}筆)", strMsgId, countNum), LogState.Info);
        }

        #endregion

        //執行寄信
        ACHR02_SendMail(sAddresser, sAddressee, "重要通知-ACH信用卡自扣授權檔回覆通知~~", sContentS, sAchr02File);

        //更新batch
        strMsgId = "更新Batch R02_flag='2'與R02DateReceive資料";
        JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
        sqlComm = new SqlCommand
        {
            CommandType = CommandType.Text,
            CommandText =
                @"UPDATE batch SET R02_flag='2', R02DateReceive=@sToday WHERE Batch_no = @sBatch_no"
        };
        sqlComm.Parameters.Add(new SqlParameter("@sToday", sToday));
        sqlComm.Parameters.Add(new SqlParameter("@sBatch_no", sBatchNo));
        dbCount = 0;
        if (BRBase<Entity_SP>.UpdateWithCount(sqlComm, "Connection_System", ref dbCount))
        {
            JobHelper.SaveLog(string.Format("{0}：成功！(總共{1}筆)", strMsgId, dbCount), LogState.Info);
        }
        else
        {
            JobHelper.SaveLog(string.Format("{0}：失敗！", strMsgId), LogState.Error);
            batchLogErr += strMsgId +"："+ sAchr02File + ",失敗！；";
            return;
        }

        #endregion
    }


    // 判斷Job工作狀態(0:停止 1:運行)
    public static bool CheckJobIsContinue(string JobID, string strFunctionKey, DateTime dateStart, ref string msgID)
    {
        bool result = true;
        if (JobHelper.SerchJobStatus(JobID).Equals("") || JobHelper.SerchJobStatus(JobID).Equals("0"))
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
        sbMessage.Append("成功筆數：" + success.ToString() + "。"); //*成功筆數
        sbMessage.Append("失敗筆數：" + fail.ToString() + "。"); //*失敗筆數

        if (message.Trim() != "")
        {
            sbMessage.Append("失敗訊息：" + message); //*失敗訊息
        }

        BRL_BATCH_LOG.Delete(strFunctionKey, jobID, "R");
        BRL_BATCH_LOG.Insert(strFunctionKey, jobID, dateStart, status, sbMessage.ToString());

    }
}