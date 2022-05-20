//******************************************************************
//*  作    者：Ares Luke
//*  功能說明：stored procedure 移轉至批次
//*  創建日期：2020/12/22
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*Ares Luke          2021/03/18         20200031-CSIP EOS       調整資料連線指定connectionString
//*Ares Luke          2021/03/18         20200031-CSIP EOS       調整LOG紀錄筆數合計結果、表頭表尾不列入筆數計算、檢核匯入總數
//*Ares Luke          2021/06/07         20200031-CSIP EOS       新增未更新銀行代碼清單訊息
//*******************************************************************

using System;
using System.Collections.Generic;
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
/// ACHP02D 的摘要描述
/// </summary>
public class ACHP02D : Quartz.IJob
{
    private static readonly JobHelper JobHelper = new JobHelper();
    private static readonly string _strFunctionKey = "01";
    private static string _strJobId;
    static DateTime _dateStart; //開始時間

    public void Execute(JobExecutionContext context)
    {
        
        try
        {
            JobDataMap jobDataMap = context.JobDetail.JobDataMap;
            _strJobId = jobDataMap.GetString("jobid").Trim();
            JobHelper.strJobID = _strJobId;

            JobHelper.SaveLog(_strJobId + "JOB啟動", LogState.Info);

            #region 初始化參數

            string strMsgId = "";

            //*批次開始執行時間
            _dateStart = DateTime.Now;

            string sFilePath = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileDownload") + "\\" + _strJobId + "\\";
            string sACHP02DFile = "";

            //收件編號
            string sReceive_Number = string.Empty;
            //流水號
            string Sequence_Number = string.Empty;
            //交易序號
            string sDeal_S_No = "598";
            //交易代號
            string sDeal_No = string.Empty;
            //收受行代號(7碼)
            string sOther_Bank_Code_L = string.Empty;
            //收受行代號(3碼)
            string sOther_Bank_Code_S = string.Empty;
            // 送檔日
            string sDateSend = string.Empty;
            // 首錄日
            string sDateInput = string.Empty;
            // 收檔日
            string sDateReceive = string.Empty;

            // 總筆數
            int sTotalCount = 0;
            // 匯入筆數
            int impTotalCount = 0;


            // YYYYMMDD
            string sToday = DateTime.Now.ToString("yyyyMMdd");
            // 00000000
            string sYMD = "00000000";

            // 發件人
            string sAddresser = UtilHelper.GetAppSettings("MailSender");
            // 收件人
            string[] sAddressee = { "" };
            // 成功郵件內容
            string sContentS = string.Empty;
            // 失敗郵件內容
            string sContentF = string.Empty;

            //LOG
            int JobCountS = 0;



            #endregion

            string jobMailTo = jobDataMap.GetString("mail").Trim();
            if (!string.IsNullOrWhiteSpace(jobMailTo))
            {
                sAddressee = jobMailTo.Split(';');
            }

            #region 檢測JOB是否在執行中

            DataTable dtInfo = BRL_BATCH_LOG.GetRunningDate(_strFunctionKey, _strJobId, "R", ref strMsgId);
            if (dtInfo == null)
            {
                JobHelper.SaveLog("JOB 工作狀態為：正在執行！", LogState.Info);
                return;
            }

            if (dtInfo.Rows.Count > 0)
            {
                JobHelper.SaveLog("JOB 工作狀態為：正在執行！", LogState.Info);
                return;
            }

            #endregion

            //*開始批次作業
            if (!BRL_BATCH_LOG.InsertRunning(_strFunctionKey, _strJobId, _dateStart, "R", ""))
            {
                return;
            }

            #region 功能

            //設定日期格式
            // sToday = sYMD.Substring(1, sYMD.Length - sToday.Length) + sToday;

            //20200031-CSIP EOS Ares Luke 修改日期:2021/02/22 修改說明:業務需求將日期格式為民國年8碼
            DateTime dt = DateTime.Now;
            sToday = string.Format("{0:0000}{1:00}{2:00}", (Int32.Parse(dt.Year.ToString()) - 1911), dt.Month, dt.Day);


            // 20200031-CSIP EOS Ares Luke 修改日期:2021/03/12 修改說明:業務需求 手動RERUN參數
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
                        sToday = string.Format("{0:0000}{1:00}{2:00}", (Int32.Parse(tempDt.Year.ToString()) - 1911), tempDt.Month, tempDt.Day);

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





            #region 刪除舊檔

            strMsgId = "刪除舊檔";
            JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
            if (Directory.Exists(sFilePath))
            {
                foreach (string f in Directory.GetFileSystemEntries(sFilePath))
                {
                    if (File.Exists(f))
                    {
                        if (Path.GetExtension(f).ToLower() == ".txt")
                        {
                            //刪除TXT副檔
                            File.Delete(f);
                            JobHelper.SaveLog(strMsgId + "：TXT檔", LogState.Info);
                        }
                    }
                }
            }

            JobHelper.SaveLog(strMsgId + "：結束", LogState.Info);

            #endregion

            #region FTP Download

            strMsgId = "FTP下載檔案";
            JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
            DataTable tblFileInfo = BRAML_File_Import.GetFileInfo(_strJobId);
            if (tblFileInfo != null && tblFileInfo.Rows.Count > 0)
            {
                string ftpIp = tblFileInfo.Rows[0]["FtpIP"].ToString();
                string ftpId = tblFileInfo.Rows[0]["FtpUserName"].ToString();
                string ftpPwd = RedirectHelper.GetDecryptString(tblFileInfo.Rows[0]["FtpPwd"].ToString());
                string ftpPath = tblFileInfo.Rows[0]["FtpPath"].ToString();
                string ftpFileName = tblFileInfo.Rows[0]["FtpFileName"].ToString();

                sACHP02DFile = ftpFileName;


                FTPFactory objFtp = new FTPFactory(ftpIp, "", ftpId, ftpPwd, "21", ftpPath, "Y");

                JobHelper.SaveLog(strMsgId + "：來源FTP目錄：" + ftpPath, LogState.Info);
                JobHelper.SaveLog(strMsgId + "：來源FTP檔名：" + sACHP02DFile, LogState.Info);
                JobHelper.SaveLog(strMsgId + "：目標目錄：" + sFilePath, LogState.Info);
                JobHelper.SaveLog(strMsgId + "：目標檔名：" + sACHP02DFile, LogState.Info);

                if (objFtp.Download(ftpPath, sACHP02DFile, sFilePath, sACHP02DFile))
                {
                    JobHelper.SaveLog(strMsgId + "：下載成功！", LogState.Info);
                }
                else
                {
                    JobHelper.SaveLog(strMsgId + "：下載失敗！", LogState.Error);
                    Batch_log("F", string.Format("{0}:下載失敗！", strMsgId));
                    return;
                }
            }

            #endregion

            #region 信件格式

            sContentS = "<font size=\"2\">您好!(成功)<BR><BR>　　您<font color=\"#FF0000\">" + sToday + "</font>收取的P02D檔（" +
                        sACHP02DFile + "）成功，您已可執行後續作業。";
            sContentF = "<font size=\"2\">您好!(失敗)<BR><BR>　　您<font color=\"#FF0000\">'" + sToday + "</font>收取的P02D檔（" +
                        sACHP02DFile + "）失敗，提醒！請作後續追蹤。";

            #endregion

            #region 清空暫存檔

            strMsgId = "清空暫存(ACHP02D_Tmp)";
            JobHelper.SaveLog(strMsgId + "：開始", LogState.Info);
            if (Truncate_ACHP02D_Tmp())
            {
                JobHelper.SaveLog(strMsgId + "：成功！", LogState.Info);
            }
            else
            {
                JobHelper.SaveLog(strMsgId + "：失敗！", LogState.Error);
                Batch_log("F", strMsgId + "：失敗！");
                return;
            }

            #endregion

            #region 從檔案匯入資料

            strMsgId = "檔案匯入至ACHR02_TMP";
            JobHelper.SaveLog(strMsgId + "開始", LogState.Info);
            if (FileImportData(sFilePath, sACHP02DFile, ref strMsgId))
            {
                JobHelper.SaveLog("匯入檔案：成功！" + strMsgId, LogState.Info);
            }
            else
            {
                //匯入失敗
                Insert_ACHFileIO_Log(sACHP02DFile, "**** 檔案匯入失敗!! ****");
                JobHelper.SaveLog("檔案匯入：失敗！" + strMsgId, LogState.Error);


                //執行寄信
                ACHP02D_SendMail(sAddresser, sAddressee, "重要通知-ACH收取P02D檔結果通知~~", sContentF, sACHP02DFile);

                strMsgId = "檔案匯入至ACHR02_TMP";
                JobHelper.SaveLog(string.Format("{0}:失敗！", strMsgId), LogState.Error);
                Batch_log("F", string.Format("{0}:失敗！", strMsgId));
                return;
            }

            #endregion

            #region 查詢ACHP02D_Tmp資料，更新ACHP02D_Tmp

            strMsgId = "更新ACHP02D_Tmp銀行代碼資料";
            JobHelper.SaveLog(strMsgId + "開始", LogState.Info);
            SqlCommand sqlComm = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"SELECT DISTINCT Other_Bank_Code_L FROM ACHP02D_Tmp WHERE Deal_S_No NOT LIKE '%OF%'"
            };
            DataSet ds = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");
            var dbCount = 0;
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    sOther_Bank_Code_L = dr["Other_Bank_Code_L"].ToString();

                    sqlComm = new SqlCommand
                    {
                        CommandType = CommandType.Text,
                        CommandText = @"SELECT BankCodeS FROM BankCodeCont WHERE BankCodeL = @sOther_Bank_Code_L"
                    };
                    sqlComm.Parameters.Add(new SqlParameter("@sOther_Bank_Code_L", sOther_Bank_Code_L));
                    DataSet ds2 = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");

                    if (ds2 != null && ds2.Tables[0].Rows.Count > 0)
                    {
                        sOther_Bank_Code_S = ds2.Tables[0].Rows[0]["BankCodeS"].ToString();

                        sqlComm = new SqlCommand
                        {
                            CommandType = CommandType.Text,
                            CommandText =
                                @"UPDATE ACHP02D_Tmp SET Other_Bank_Code_S=@sOther_Bank_Code_S WHERE Other_Bank_Code_L = @sOther_Bank_Code_L"
                        };
                        sqlComm.Parameters.Add(new SqlParameter("@sOther_Bank_Code_S", sOther_Bank_Code_S));
                        sqlComm.Parameters.Add(new SqlParameter("@sOther_Bank_Code_L", sOther_Bank_Code_L));

                        dbCount = 0;
                        if (BRBase<Entity_SP>.UpdateWithCount(sqlComm, "Connection_System",ref dbCount))
                        {
                            impTotalCount += dbCount;
                            sOther_Bank_Code_S = null;
                        }
                        else
                        {
                            JobHelper.SaveLog(string.Format("{0}:失敗！", strMsgId), LogState.Error);
                            Batch_log("F", string.Format("{0}:失敗！", strMsgId));
                            return;
                        }
                    }
                }
                JobHelper.SaveLog(string.Format("{0}:成功！(總共{1}筆)", strMsgId, impTotalCount), LogState.Info);



                sqlComm = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = @"
                                    SELECT ISNULL(SPONSOR_ID, '') AS SPONSOR_ID,
                                           ISNULL(LEFT(CUS_ID, LEN(CUS_ID) / 2) + SUBSTRING('XXXXXXXXXXXXXXXXXXXX', 1, LEN(CUS_ID) - LEN(CUS_ID) / 2),
                                                  '') AS CUS_ID
                                    FROM ACHP02D_TMP TMP
                                             LEFT JOIN BANKCODECONT SCC ON SCC.BANKCODEL = TMP.OTHER_BANK_CODE_L
                                    WHERE DEAL_S_NO NOT LIKE '%OF%'
                                      AND BANKCODES IS NULL
                                    "
                };
                DataSet ds9 = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");
                foreach (DataRow dr9 in ds9.Tables[0].Rows)
                {
                    JobHelper.SaveLog(
                        string.Format("{0}:未更新銀行代碼名單！ 發動者統一編號{1} , 用戶號碼{2}", strMsgId, dr9["SPONSOR_ID"],
                            dr9["Cus_ID"]), LogState.Info);
                }
            }

            #endregion

            #region 查詢 sDateInput 與 sDateSend

            sqlComm = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText =
                    @"
                    SELECT SUBSTRING(Deal_S_No + Deal_No + Sponsor_ID + Other_Bank_Acc_No,10,8) as sDateInput,
                           SUBSTRING(Deal_S_No + Deal_No + Sponsor_ID + Other_Bank_Acc_No,10,8) + 19110000  as sDateSend
                    FROM ACHP02D_Tmp WHERE Deal_S_No LIKE '%BOF%'
                    "
            };
            ds = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");

            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                Insert_ACHFileIO_Log(sACHP02DFile, "**** 檔案格式錯誤，匯入失敗!! ***");
                JobHelper.SaveLog("檔案格式錯誤，匯入失敗", LogState.Error);

                //執行寄信
                ACHP02D_SendMail(sAddresser, sAddressee, "重要通知-ACH收取P02D檔結果通知~~", sContentF, sACHP02DFile);

                JobHelper.SaveLog(string.Format("匯入失敗！"), LogState.Error);
                Batch_log("F", string.Format("匯入失敗！", strMsgId));
                return;
            }
            else
            {
                sDateInput = ds.Tables[0].Rows[0]["sDateInput"].ToString();
                sDateSend = ds.Tables[0].Rows[0]["sDateSend"].ToString();
            }

            // sDateInput = sDateInput.Substring(9, 8);

            #endregion

            #region 查詢 sDateReceive

            // DateTime temp = Convert.ToDateTime(sDateSend.ToString()).AddDays(-1);
            DateTime temp = DateTime.ParseExact(sDateSend.ToString(), "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces).AddDays(-1);
            sDateSend = (temp.Year - 1911).ToString() + temp.ToString("yyyyMMdd").Substring(4, 4);
            sDateSend = sDateSend.PadLeft(8, '0');


            // string sYesterday = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
            // sDateSend = sYMD.Substring(1, sYMD.Length - sYesterday.Length) + sYesterday;

            sqlComm = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText =
                    @"SELECT Deal_S_No + Deal_No + Sponsor_ID + Other_Bank_Acc_No as sDateReceive FROM ACHP02D_Tmp WHERE Deal_S_No LIKE '%EOF%'"
            };
            ds = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");

            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                Insert_ACHFileIO_Log(sACHP02DFile, "**** 檔案格式錯誤，匯入失敗!! ***");
                JobHelper.SaveLog("檔案格式錯誤，匯入失敗", LogState.Error);

                //執行寄信
                ACHP02D_SendMail(sAddresser, sAddressee, "重要通知-ACH收取P02D檔結果通知~~", sContentF, sACHP02DFile);

                JobHelper.SaveLog(string.Format("匯入失敗！"), LogState.Error);
                Batch_log("F", string.Format("匯入失敗！", strMsgId));
                return;
            }
            else
            {
                sDateReceive = ds.Tables[0].Rows[0]["sDateReceive"].ToString();
            }

            #endregion

            sTotalCount = Convert.ToInt32(sDateReceive.Substring(3, 8));
            sTotalCount = Convert.ToInt32(sTotalCount.ToString().Trim());

            // 開始檢核檔案匯入筆數與表尾總數是否相符

            //JobHelper.SaveLog("開始檢核檔案匯入筆數與表尾總數是否相符," + "開始", LogState.Info);
            //if (sTotalCount == impTotalCount)
            //{
            //    JobHelper.SaveLog("檢核結果:符合", LogState.Info);
            //}
            //else
            //{
            //    JobHelper.SaveLog("檢核結果:不符合", LogState.Error);
            //}




            sDateReceive = sDateReceive.Substring(11, 8);


            #region Deal_S_No

            sqlComm = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText =
                    @"SELECT Deal_S_No FROM ACHP02D_Tmp WHERE Deal_S_No NOT LIKE '%OF%' ORDER BY Deal_S_No ASC"
            };
            ds = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                strMsgId = "新增至Other_Bank_Temp";
                JobHelper.SaveLog(strMsgId + "開始", LogState.Info);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    sDeal_S_No = dr["Deal_S_No"].ToString();

                    #region sReceive_Number

                        sqlComm = new SqlCommand
                    {
                        CommandType = CommandType.Text,
                        CommandText =
                            @"SELECT @sToday + right('00000' + convert(varchar, (isnull(max(substring(Receive_Number, 9, 5)), 0) + 1)), 5) as sReceive_Number
                             FROM Other_Bank_Temp WHERE substring(Receive_Number, 1, 8) = @sToday"
                    };
                    sqlComm.Parameters.Add(new SqlParameter("@sToday", sToday));
                    DataSet ds2 = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");

                    if (ds2 != null && ds2.Tables[0].Rows.Count > 0)
                    {
                        sReceive_Number = ds2.Tables[0].Rows[0]["sReceive_Number"].ToString();

                        #region INSERT INTO Other_Bank_Temp
                        sqlComm = new SqlCommand
                        {
                            CommandType = CommandType.Text,
                            CommandText =
                                @"INSERT INTO Other_Bank_Temp (Deal_S_No, Deal_No, Other_Bank_Code_L, Other_Bank_Code_S, Other_Bank_Acc_No,
                                                             Other_Bank_Cus_ID, Cus_ID, Apply_Type, Receive_Number, Build_Date, KeyIn_Flag,
                                                             Batch_no)
                                SELECT Deal_S_No,
                                       Deal_No,
                                       Other_Bank_Code_L,
                                       Other_Bank_Code_S,
                                       Other_Bank_Acc_No,
                                       Other_Bank_Cus_ID,
                                       Cus_ID,
                                       Apply_Type,
                                       @sReceive_Number AS Receive_Number,
                                       S_DATE,
                                       '0'              AS KeyIn_Flag,
                                       @sDateInput      AS Batch_no
                                FROM ACHP02D_Tmp
                                WHERE Deal_S_No = @sDeal_S_No"
                        };
                        sqlComm.Parameters.Add(new SqlParameter("@sReceive_Number", sReceive_Number));
                        sqlComm.Parameters.Add(new SqlParameter("@sDateInput", sDateInput));
                        sqlComm.Parameters.Add(new SqlParameter("@sDeal_S_No", sDeal_S_No));

                        dbCount = 0;
                        if (BRBase<Entity_SP>.AddWithCount(sqlComm, "Connection_System", ref dbCount))
                        {
                            JobCountS += dbCount;
                        }
                        else
                        {
                            JobHelper.SaveLog(string.Format("{0}:失敗！", strMsgId), LogState.Error);
                            Batch_log("F", string.Format("{0}:失敗！", strMsgId));
                            return;
                        }

                        #endregion
                    }
                    #endregion
                }
                JobHelper.SaveLog(string.Format("{0}:成功！(總共{1}筆)", strMsgId, JobCountS), LogState.Info);

            }
            #endregion




            //新增batch
            strMsgId = "新增Batch資料";
            JobHelper.SaveLog("開始" + strMsgId, LogState.Info);
            sqlComm = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"INSERT INTO batch(Batch_no, dateSend, dateInput, dateReceive, P02_flag, TotalCount)
                                VALUES (@sDateInput, @sDateSend, @sDateInput, @sDateReceive, 'N', @sTotalCount)"
            };
            sqlComm.Parameters.Add(new SqlParameter("@sDateInput", sDateInput));
            sqlComm.Parameters.Add(new SqlParameter("@sDateSend", sDateSend));
            sqlComm.Parameters.Add(new SqlParameter("@sDateReceive", sDateReceive));
            sqlComm.Parameters.Add(new SqlParameter("@sTotalCount", sTotalCount));

            dbCount = 0;
            if (BRBase<Entity_SP>.AddWithCount(sqlComm, "Connection_System", ref dbCount))
            {
                JobHelper.SaveLog(string.Format("{0}:成功！(總共{1}筆)", strMsgId, dbCount), LogState.Info);
            }
            else
            {
                JobHelper.SaveLog(string.Format("{0}:失敗！", strMsgId), LogState.Error);
                Batch_log("F", string.Format("{0}:失敗！", strMsgId));
                return;
            }

            //執行寄信
            ACHP02D_SendMail(sAddresser, sAddressee, "重要通知-ACH收取P02D檔結果通知~~", sContentS, sACHP02DFile);


            #endregion

            #region 紀錄下次執行時間

            string strMsg = _strJobId + "執行於:" +
                            DateTime.Parse(context.FireTimeUtc.ToString()).AddHours(8).ToString();
            if (context.NextFireTimeUtc.HasValue)
            {
                strMsg += "  ;下次執行於:" + DateTime.Parse(context.NextFireTimeUtc.ToString()).AddHours(8).ToString();
            }

            JobHelper.SaveLog(strMsg, LogState.Info);

            #endregion

            #region 結束批次作業

            Batch_log("S", "匯入共" + JobCountS + "筆。");
            JobHelper.SaveLog(_strJobId + "JOB結束", LogState.Info);

            #endregion
        }
        catch (Exception ex)
        {
            Batch_log("F", "CommonModel_發錯錯誤_" + ex.ToString());
            BRL_BATCH_LOG.SaveLog(ex);
        }
    }


    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:更新Batch_log
    /// 作    者:Ares Luke
    /// 創建時間:2020/12/22
    /// </summary>
    /// <param name="strStatus">狀態</param>
    /// <param name="strRMsg">訊息</param>
    private void Batch_log(string strStatus, string strRMsg)
    {
        BRL_BATCH_LOG.Delete(_strFunctionKey, _strJobId, "R");
        BRL_BATCH_LOG.Insert(_strFunctionKey, _strJobId, _dateStart, strStatus, strRMsg);
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:清空 Truncate ACHP02D_Tmp
    /// 作    者:Ares Luke
    /// 創建時間:2020/12/22
    /// </summary>
    /// <returns></returns>
    private static bool Truncate_ACHP02D_Tmp()
    {
        StringBuilder sbSql = new StringBuilder("Truncate Table ACHP02D_Tmp");
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
    /// 功能說明:檔案匯入至DB，格式參考FMT
    /// 作    者:Ares Luke
    /// 創建時間:2020/12/22
    /// </summary>
    /// <param name="sFile_Path">目錄路徑</param>
    /// <param name="sACHR02File">檔案名稱</param>
    /// <param name="strMsg">回傳訊息</param>
    /// <returns></returns>
    private static bool FileImportData(String sFile_Path, String sACHR02File, ref String strMsg)
    {
        string sqlText =
            @"INSERT INTO ACHP02D_Tmp (Deal_S_No, Deal_No, Sponsor_ID, Other_Bank_Code_L, Other_Bank_Acc_No,
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

            int impSuccess = 0;
            int impFail = 0;


            foreach (string t in arrayFile)
            {
                string fileText = t;
                SqlCommand sqlComm = new SqlCommand {CommandType = CommandType.Text, CommandText = sqlText};

                int initNum = 0;
                
                // 判斷是否為標頭或標尾，不列入筆數計算
                bool isTitle = false;

                foreach (var fmt in fmtParam)
                {
                    string keyText = fmt.Key.ToString();
                    int keyLen = fmt.Value;
                    string keyValue = fileText.Substring(initNum, keyLen);
                    sqlComm.Parameters.Add(new SqlParameter("@" + keyText, keyValue));

                    if (keyText == "Deal_S_No" && (keyValue.StartsWith("BOFACH") || keyValue.StartsWith("EOF")))
                    {
                        isTitle = true;
                    }
                    initNum += keyLen;
                }

                if (BRBase<Entity_SP>.Add(sqlComm, "Connection_System"))
                {
                    if(!isTitle){
                        impSuccess++;
                    }
                }
                else
                {
                    impFail++;
                }
            }

            #endregion

            string temp = "檔案預計匯入總筆數(含表頭表尾)共:" + arrayFile.Length + "筆。" + "(資料明細 成功:" + impSuccess + "筆，失敗:" + impFail + "筆)";

            if (arrayFile.Length == impSuccess + 2)
            {
                strMsg = "全部匯入成功，" + temp;
                return true;
            }
            else if (impSuccess > 0 && impFail > 0)
            {
                strMsg = "部分匯入成功，" + temp;
                return false;
            }
            else
            {
                strMsg = "匯入失敗，" + temp;
                return false;
            }
        }
        catch (Exception exp)
        {
            BRBase<Entity_SP>.SaveLog(exp.Message);
            strMsg = exp.Message.ToString();
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
            return BRBase<Entity_SP>.Add(sqlComm,"Connection_System");
        }
        catch (Exception exp)
        {
            BRBase<Entity_SP>.SaveLog(exp.Message);
            return false;
        }
    }


    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:ACHR02D 寄信功能並insert ACHFileIO_Log
    /// 作    者:Ares Luke
    /// 創建時間:2020/12/22
    /// </summary>
    /// <param name="strFrom">寄件人</param>
    /// <param name="sAddressee">收件人</param>
    /// <param name="strSubject">信件標題</param>
    /// <param name="strBody">信件內文</param>
    /// <param name="strAchFileIoLog">Insert_ACHFileIO_Log訊息</param>
    /// <returns></returns>
    private static bool ACHP02D_SendMail(string strFrom, string[] sAddressee, string strSubject, string strBody,
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
}