//******************************************************************
//*  作    者：Ares Luke
//*  功能說明：stored procedure 移轉至批次
//*  創建日期：2020/12/22
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*Ares Luke          2021/03/18         20200031-CSIP EOS       調整資料連線指定connectionString
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
/// 專案代號:20200031-CSIP EOS
/// 功能說明:
/// 作    者:Ares Luke
/// 創建時間:2020/12/16
/// 修改紀錄:20210708_Ares_Stanley-調整SQL空白字元
/// </summary>
public class ACHP02 : Quartz.IJob
{
    private static readonly JobHelper JobHelper = new JobHelper();
    private readonly string _strFunctionKey = "01";
    private static string _strJobId;
    DateTime _dateStart; //開始時間

    #region sql

    string get_batch =
        @"
SELECT Batch_no,DateInput FROM batch WHERE dateSend=@sToday AND P02_flag='N'";

    string insert_ACHP02_Tmp = @"INSERT INTO ACHP02_Tmp (Deal_S_No, Deal_No, Sponsor_ID, Other_Bank_Code_L, Other_Bank_Acc_No, Other_Bank_Cus_ID, Cus_ID, Apply_Type, S_DATE,	S_Bank_ID,	S_Remark,	Deal_Type,	Reply_Info,	Remark )
VALUES('BOFACH', 'P02', @sDateInput + '82', '20901  ', '              ', '          ', '                    ', ' ', '        ', '       ', '                    ', ' ', ' ', '            ' 	)";
    string insert_ACHP02_Tmp_2 =
        @"
INSERT INTO ACHP02_Tmp(Deal_No, Sponsor_ID, Other_Bank_Code_L, Other_Bank_Acc_No, Other_Bank_Cus_ID, Cus_ID, Apply_Type,S_DATE, S_Bank_ID, S_Remark, Deal_Type, Reply_Info, Remark)
(SELECT Deal_No,
       @sSponsor_ID + '  '                                          AS Sponsor_ID,
       Other_Bank_Code_L,
       left(@sOther_Bank_Acc_No, len(@sOther_Bank_Acc_No) - len(Other_Bank_Acc_No)) + Other_Bank_Acc_No,
       Other_Bank_Cus_ID + right(@sOther_Bank_Cus_ID, 10 - len(Other_Bank_Cus_ID)),
       Cus_ID + right(@sCus_ID, 20 - len(Cus_ID)),
       Apply_Type,
       Build_Date                                                   AS S_DATE,
       @sS_Bank_ID                                                  AS S_Bank_ID,
       Receive_Number + right(@sS_Remark, 20 - len(Receive_Number)) AS S_Remark,
       'N'                                                          AS Deal_Type,
       ' '                                                          AS Reply_Info,
       @sRemark
FROM Other_Bank_Temp
WHERE Batch_no = @sBatch_no)";

    string update_ACHP02_Tmp =
        @"
UPDATE ACHP02_Tmp SET Deal_S_No=ID-1 WHERE ID<>1";

    string update_ACHP02_Tmp_2 =
        @"
UPDATE ACHP02_Tmp SET Deal_S_No=Left(@sDeal_S_No,len(@sDeal_S_No)-len(Deal_S_No))+Deal_S_No WHERE ID<>1";

    string insert_ACHP02_Tmp_3 = @"INSERT INTO ACHP02_Tmp(Deal_S_No,Deal_No,Sponsor_ID,Other_Bank_Code_L,Other_Bank_Acc_No,Other_Bank_Cus_ID,Cus_ID,Apply_Type,S_DATE,S_Bank_ID, S_Remark,Deal_Type,Reply_Info,Remark)
VALUES('EOF'+left(@sTotalCount,3),SUBSTRING(@sTotalCount,4,3),right(@sTotalCount,2)+@sDateReceive,'       ','              ','          ','                    ',' ','        ','       ','                    ',' ',' ','            ')";

    #endregion

    public void Execute(JobExecutionContext context)
    {
        //*批次開始執行時間
        _dateStart = DateTime.Now;

        try
        {
            JobDataMap jobDataMap = context.JobDetail.JobDataMap;
            _strJobId = jobDataMap.GetString("jobid").Trim();
            JobHelper.strJobID = _strJobId;

            JobHelper.SaveLog(_strJobId + "JOB啟動", LogState.Info);

            #region 初始化參數

            string strMsgId = "";

            string sFile_Path = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileUpload") + "\\" +
                                _strJobId + "\\";
            string sACHP02File;


            // Batch_no
            string sBatch_no;
            // 送檔日
            // string sdateSend = string.Empty;
            // 首錄日
            string sDateInput;
            // 收檔日
            string sDateReceive = string.Empty;


            // YYYYMMDD
            string sToday = DateTime.Now.ToString("yyyyMMdd");
            // 00000000
            string sYMD = "00000000";
            // 總筆數
            String sTotalCount = string.Empty;
            // 交易序號"000000"
            string sDeal_S_No = "000000";
            // 發動者統一編號 03077208 
            string sSponsor_ID = "03077208";
            // 委繳戶帳號"00000000000000"
            string sOther_Bank_Acc_No = "00000000000000";
            // 委繳戶統一編號"          "
            string sOther_Bank_Cus_ID = "          ";
            // 用戶號碼"                    "
            string sCus_ID = "                    ";
            // 發動者專用區（收件編號）"                    "
            string sS_Remark = "                    ";
            // 發動行代號 8220901
            string sS_Bank_ID = "8220901";
            // Remark"            "
            string sRemark = "            ";


            // // 發送郵件回傳值 1：成功
            // int iRat;
            // 發件人
            string sAddresser = UtilHelper.GetAppSettings("MailSender");
            // 收件人
            string[] sAddressee = {""};

            DataSet ds = new DataSet();

            //匯出總筆數
            int importCount = 0;
            //執行資料異動筆數
            int dbCount = 0;

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

            SqlCommand sqlComm = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = get_batch
            };
            sqlComm.Parameters.Add(new SqlParameter("@sToday", sToday));
            ds = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");


            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                //新增ACHFileIO_Log
                JobHelper.SaveLog("屬性設定本日無須上傳ACHP02!!", LogState.Info);
                Insert_ACHFileIO_Log("屬性設定本日無須上傳ACHP02!!", "**** 屬性設定本日無須上傳ACHP02!! ****");

                //開始寄信
                ACHP02_SendMail(sAddresser, sAddressee,  "屬性設定本日無須上傳ACHP02!!", "屬性設定本日無須上傳ACHP02!!");
            }
            else
            {
                sBatch_no = ds.Tables[0].Rows[0]["Batch_no"].ToString();
                sDateInput = ds.Tables[0].Rows[0]["DateInput"].ToString();

                #region 清空ACHP02_Tmp

                strMsgId = "清空DB ACHP02_Tmp";
                JobHelper.SaveLog("開始" + strMsgId, LogState.Info);
                if (Truncate_ACHP02_Tmp())
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

                //開始新增資料ACHP02_Tmp
                strMsgId = "新增ACHP02_Tmp資料[表頭]";
                JobHelper.SaveLog("開始" + strMsgId, LogState.Info);
                sqlComm = new SqlCommand {CommandType = CommandType.Text, CommandText = insert_ACHP02_Tmp};
                sqlComm.Parameters.Add(new SqlParameter("@sDateInput", sDateInput));

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

                //開始新增資料ACHP02_Tmp-2
                strMsgId = "新增ACHP02_Tmp資料[來源:Other_Bank_Temp]";
                JobHelper.SaveLog("開始" + strMsgId, LogState.Info);
                sqlComm = new SqlCommand {CommandType = CommandType.Text, CommandText = insert_ACHP02_Tmp_2};
                sqlComm.Parameters.Add(new SqlParameter("@sSponsor_ID", sSponsor_ID));
                sqlComm.Parameters.Add(new SqlParameter("@sOther_Bank_Acc_No", sOther_Bank_Acc_No));
                sqlComm.Parameters.Add(new SqlParameter("@sOther_Bank_Cus_ID", sOther_Bank_Cus_ID));
                sqlComm.Parameters.Add(new SqlParameter("@sCus_ID", sCus_ID));
                sqlComm.Parameters.Add(new SqlParameter("@sS_Bank_ID", sS_Bank_ID));
                sqlComm.Parameters.Add(new SqlParameter("@sS_Remark", sS_Remark));
                sqlComm.Parameters.Add(new SqlParameter("@sRemark", sRemark));
                sqlComm.Parameters.Add(new SqlParameter("@sBatch_no", sBatch_no));

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


                //開始更新資料ACHP02_Tmp
                strMsgId = "更新ACHP02_Tmp資料";
                JobHelper.SaveLog("開始" + strMsgId, LogState.Info);

                dbCount = 0;
                if (BRBase<Entity_SP>.AddWithCount(new SqlCommand
                    {CommandType = CommandType.Text, CommandText = update_ACHP02_Tmp}, "Connection_System", ref dbCount))
                {
                    JobHelper.SaveLog(string.Format("{0}:成功！(總共{1}筆)", strMsgId, dbCount), LogState.Info);
                }
                else
                {
                    JobHelper.SaveLog(string.Format("{0}:失敗！", strMsgId), LogState.Error);
                    Batch_log("F", string.Format("{0}:失敗！", strMsgId));
                    return;
                }

                #region 開始更新資料ACHP02_Tmp_2

                strMsgId = "更新ACHP02_Tmp資料";
                JobHelper.SaveLog("開始" + strMsgId, LogState.Info);
                sqlComm = new SqlCommand {CommandType = CommandType.Text, CommandText = update_ACHP02_Tmp_2};
                sqlComm.Parameters.Add(new SqlParameter("@sDeal_S_No", sDeal_S_No));
                if (BRBase<Entity_SP>.UpdateWithCount(sqlComm, "Connection_System", ref importCount))
                {
                    JobHelper.SaveLog(string.Format("{0}:成功！(總共{1}筆)", strMsgId, importCount), LogState.Info);
                }
                else
                {
                    JobHelper.SaveLog(string.Format("{0}:失敗！", strMsgId), LogState.Error);
                    Batch_log("F", string.Format("{0}:失敗！", strMsgId));
                    return;
                }

                sTotalCount = importCount.ToString().PadLeft(8, '0');

                #endregion

                #region 開始更新資料ACHP02_Tmp_3表尾資料

                strMsgId = "新增ACHP02_Tmp資料[表尾]";
                JobHelper.SaveLog("開始" + strMsgId, LogState.Info);
                sqlComm = new SqlCommand {CommandType = CommandType.Text, CommandText = insert_ACHP02_Tmp_3};
                sqlComm.Parameters.Add(new SqlParameter("@sTotalCount", sTotalCount));
                sqlComm.Parameters.Add(new SqlParameter("@sDateReceive", sDateReceive));

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

                #endregion

                //設定參數
                sACHP02File = "ACHP02_" + sDateInput + '_' + sSponsor_ID + ".txt";

                #region 匯出資料至檔案

                //匯出檔案
                strMsgId = "";
                JobHelper.SaveLog("開始匯出檔案", LogState.Info);

                if (GetImportData(sFile_Path, sACHP02File, ref strMsgId))
                {
                    JobHelper.SaveLog("匯出檔案：成功！(" + strMsgId + ")", LogState.Info);
                }
                else
                {
                    //匯出失敗
                    Insert_ACHFileIO_Log(sACHP02File, "**** 檔案匯出失敗!! ****");
                    JobHelper.SaveLog("匯出檔案：失敗！" + strMsgId, LogState.Error);

                    //開始寄信
                    ACHP02_SendMail(sAddresser, sAddressee, "檔案匯出失敗!!", sACHP02File);
                    Batch_log("F", "匯出檔案：失敗！" + strMsgId);
                    return;
                }

                #endregion

                #region FTP (將資料put到至法金download server)

                //刪除舊處理資料夾
                // FileTools.DeleteFolder(sLocalFTPFile);
                strMsgId = "上傳檔案至FTP";
                JobHelper.SaveLog("開始" + strMsgId, LogState.Info);
                if (UploadFileTo_mFtp(_strJobId, sACHP02File, sFile_Path, sACHP02File))
                {
                    JobHelper.SaveLog(strMsgId + "：來源目錄：" + sFile_Path, LogState.Info);
                    JobHelper.SaveLog(strMsgId + "：來源檔案：" + sACHP02File, LogState.Info);
                    JobHelper.SaveLog(strMsgId + "：上傳成功！", LogState.Info);
                }
                else
                {
                    JobHelper.SaveLog(strMsgId + "：上傳失敗！", LogState.Error);
                    Batch_log("F", strMsgId + "：上傳失敗！");
                    return;
                }

                //參數設定
                sTotalCount = sTotalCount == "00000000" ? "0" : importCount.ToString();

                #endregion

                //更新batch
                strMsgId = "更新Batch資料";
                JobHelper.SaveLog("開始" + strMsgId, LogState.Info);
                sqlComm = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = @"UPDATE batch SET P02_flag='Y',TotalCount=@sTotalCount WHERE Batch_no=@sBatch_no"
                };
                sqlComm.Parameters.Add(new SqlParameter("@sTotalCount", sTotalCount));
                sqlComm.Parameters.Add(new SqlParameter("@sBatch_no", sBatch_no));
                dbCount = 0;
                if (BRBase<Entity_SP>.UpdateWithCount(sqlComm, "Connection_System", ref dbCount))
                {
                    JobHelper.SaveLog(string.Format("{0}:成功！(總共{1}筆)", strMsgId, dbCount), LogState.Info);
                }
                else
                {
                    JobHelper.SaveLog(string.Format("{0}:失敗！", strMsgId), LogState.Error);
                    Batch_log("F", string.Format("{0}:失敗！", strMsgId));
                    return;
                }

                //開始寄信
                ACHP02_SendMail(sAddresser, sAddressee,  sACHP02File + "檔案上傳FTP成功!!",
                    sACHP02File);
            }

            #endregion


            #region 紀錄下次執行時間

            string strMsg = _strJobId + "執行於:" + DateTime.Parse(context.FireTimeUtc.ToString()).AddHours(8).ToString();
            if (context.NextFireTimeUtc.HasValue)
            {
                strMsg += "  ;下次執行於:" + DateTime.Parse(context.NextFireTimeUtc.ToString()).AddHours(8).ToString();
            }

            JobHelper.SaveLog(strMsg, LogState.Info);

            #endregion

            #region 結束批次作業

            Batch_log("S", "匯出共" + importCount + "筆。");
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
    /// 功能說明:執行過程中斷更新Batch_log
    /// 作    者:Ares Luke
    /// 創建時間:2020/12/16
    /// </summary>
    /// <param name="strStatus"></param>
    /// <param name="strRMsg"></param>
    private void Batch_log(string strStatus, string strRMsg)
    {
        BRL_BATCH_LOG.Delete(_strFunctionKey, _strJobId, "R");
        BRL_BATCH_LOG.Insert(_strFunctionKey, _strJobId, _dateStart, strStatus, strRMsg);
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:匯出檔案
    /// 作    者:Ares Luke
    /// 創建時間:2020/12/16
    /// </summary>
    /// <param name="sFile_Path"></param>
    /// <param name="sACHP02File"></param>
    /// <param name="strMsg"></param>
    /// <returns></returns>
    private static bool GetImportData(String sFile_Path, String sACHP02File, ref String strMsg)
    {
        String sqlText =
            @"select Deal_S_No, Deal_No, Sponsor_ID, Other_Bank_Code_L, Other_Bank_Acc_No, Other_Bank_Cus_ID, Cus_ID, Apply_Type, S_DATE, S_Bank_ID, S_Remark, Deal_Type, Reply_Info, Remark from ACHP02_Tmp";
        SqlCommand sqlComm = new SqlCommand {CommandType = CommandType.Text, CommandText = sqlText};

        try
        {
            DataSet ds = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");

            //文檔內容
            string strFileContent = string.Empty;

            Dictionary<String, int> fmtParam = GetImportFmt();

            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                strMsg = "查無資料。";
                return false;
            }

            int count = 0;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                count++;
                foreach (var fmt in fmtParam)
                {
                    int lenFmt = fmt.Value;

                    var fmtNewValue = String.Format(String.Format("{{0,-{0}}}", lenFmt),
                        dr[fmt.Key] == null ? "" : dr[fmt.Key].ToString());
                    strFileContent += fmtNewValue;
                }

                if (count != ds.Tables[0].Rows.Count)
                {
                    strFileContent += "\r\n";
                }
            }

            strMsg = "預計匯入" + ds.Tables[0].Rows.Count + "筆。（成功:" + count + "筆）";

            String saveLocalPath = sFile_Path + sACHP02File;

            //檢查並刪除舊檔
            if (File.Exists(saveLocalPath))
            {
                File.Delete(saveLocalPath);
            }

            //創建文件路徑，並寫入內文
            FileTools.Create(sFile_Path + sACHP02File, strFileContent);

            return true;
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
    /// 功能說明:FTM格式範本
    /// 作    者:Ares Luke
    /// 創建時間:2020/12/16
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
    /// 功能說明:清空 Truncate ACHP02_Tmp
    /// 作    者:Ares Luke
    /// 創建時間:2020/12/16
    /// </summary>
    /// <returns></returns>
    private static bool Truncate_ACHP02_Tmp()
    {
        StringBuilder sbSql = new StringBuilder("Truncate Table ACHP02_Tmp");
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
    /// 創建時間:2020/12/16
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="remark"></param>
    private static bool Insert_ACHFileIO_Log(string fileName, string remark)
    {
        try
        {
            string sqlText =
                @"insert into ACHFileIO_Log select convert(varchar(8), getdate(), 112), convert(varchar(8), getdate(), 114), @FileName,@Remark";
            SqlCommand sqlComm = new SqlCommand {CommandType = CommandType.Text, CommandText = sqlText};
            sqlComm.Parameters.Add(new SqlParameter("@FileName", fileName));
            sqlComm.Parameters.Add(new SqlParameter("@Remark", remark));
            return BRBase<Entity_SP>.Add(sqlComm, "Connection_System");
        }
        catch (Exception exp)
        {
            BRBase<Entity_SP>.SaveLog(exp.Message);
            return false;
        }
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:FTP檔案上傳
    /// 作    者:Ares Luke
    /// 創建時間:2020/12/16
    /// </summary>
    /// <param name="jobId"></param>
    /// <param name="ftpFileName"></param>
    /// <param name="localPath"></param>
    /// <param name="localFileName"></param>
    /// <returns></returns>
    private static bool UploadFileTo_mFtp(string jobId, string ftpFileName, string localPath, string localFileName)
    {
        bool result = false;

        try
        {
            DataTable tblFileInfo = BRAML_File_Import.GetFileInfo(jobId);

            string ftpIp = tblFileInfo.Rows[0]["FtpIP"].ToString();
            string ftpId = tblFileInfo.Rows[0]["FtpUserName"].ToString();
            string ftpPwd = RedirectHelper.GetDecryptString(tblFileInfo.Rows[0]["FtpPwd"].ToString());
            string ftpPath = tblFileInfo.Rows[0]["FtpPath"].ToString();

            FTPFactory objFtp = new FTPFactory(ftpIp, "", ftpId, ftpPwd, "21", ftpPath, "Y");

            if (objFtp.Upload(ftpPath, ftpFileName, localPath + "\\" + localFileName))
            {
                result = true;
            }

            return result;
        }
        catch (Exception ex)
        {
            BRBase<Entity_SP>.SaveLog(ex.Message);
            return result;
        }
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:ACHP02 寄信功能並insert ACHFileIO_Log
    /// 作    者:Ares Luke
    /// 創建時間:2020/12/22
    /// 修改時間:2021/03/30 Ares_Luke Log文字調整 
    /// </summary>
    /// <param name="strFrom"></param>
    /// <param name="sAddressee"></param>
    /// <param name="strSubject"></param>
    /// <param name="strBody"></param>
    /// <param name="strAchFileIoLog"></param>
    /// <returns></returns>
    private bool ACHP02_SendMail(string strFrom, string[] sAddressee, string strBody, string strAchFileIoLog)
    {
        try
        {
            //開始寄信
            JobHelper.SaveLog("開始寄信！", LogState.Info);
            if (JobHelper.SendMail(strFrom, sAddressee, "重要通知-ACH信用卡自扣授權檔上傳通知~~", strBody))
            {
                //寄信成功LOG
                JobHelper.SaveLog("寄信成功！", LogState.Info);
                Insert_ACHFileIO_Log(strAchFileIoLog, "**** 郵件發送成功!! ***");
            }
            else
            {
                //寄信失敗LOG
                JobHelper.SaveLog("寄信失敗！", LogState.Error);
                Insert_ACHFileIO_Log(strAchFileIoLog, "**** 郵件發送失敗!! ***");
            }

            return false;
        }
        catch (Exception exp)
        {
            BRBase<Entity_SP>.SaveLog(exp.Message);
            return false;
        }
    }
}