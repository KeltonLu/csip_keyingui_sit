//******************************************************************
//*  作    者：Ares_Jack
//*  功能說明：上傳EDDA核印失敗資料給卡主機
//*  創建日期：2022/09/27
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************

using System;
using System.Data;
using Quartz;
using Framework.Common.Logging;
using CSIPCommonModel.BusinessRules;
using CSIPKeyInGUI.BusinessRules;
using System.Data.SqlClient;
using CSIPNewInvoice.EntityLayer_new;
using System.IO;
using Framework.Common.Utility;
using System.Net;
using System.Text;

public class EddaUploadFailData : IJob
{
    private static readonly JobHelper JobHelper = new JobHelper();
    private string strFuncKey = UtilHelper.GetAppSettings("FunctionKey");
    private string strJobID = "EddaUploadFailData";
    private string strJobTitle = string.Empty;
    private string strMailList = string.Empty;
    private string jobDate = string.Empty;

    public void Execute(JobExecutionContext context)
    {
        DateTime dtStart = DateTime.Now;
        string batchLogMsg = string.Empty;
        string status = "F";
        try
        {
            strJobID = context.JobDetail.JobDataMap.GetString("jobid").Trim();
            strJobTitle = context.JobDetail.JobDataMap.GetString("title").Trim();
            strMailList = context.JobDetail.JobDataMap.GetString("mail").Trim();
            jobDate = DateTime.Now.ToString("yyyMMdd");

            // 判斷Job工作狀態(0:停止 1:運行)
            var isContinue = CheckJobIsContinue(strJobID, strFuncKey, dtStart, ref batchLogMsg);
            if (!isContinue) return;

            #region 判斷是否手動設置參數啟動排程

            Logging.Log("判斷是否手動輸入參數 啟動排程：開始！", strJobID, LogState.Info, LogLayer.BusinessRule);

            if (context.JobDetail.JobDataMap["param"] != null)
            {
                Logging.Log("手動輸入參數啟動排程：是！", strJobID, LogState.Info, LogLayer.BusinessRule);
                Logging.Log("檢核輸入參數：開始！", strJobID, LogState.Info, LogLayer.BusinessRule);

                string strParam = context.JobDetail.JobDataMap["param"].ToString();

                if (strParam.Length == 10)
                {
                    DateTime tempDt;
                    if (DateTime.TryParse(strParam, out tempDt))
                    {
                        Logging.Log("檢核參數：成功！ 參數：" + strParam, strJobID, LogState.Info, LogLayer.BusinessRule);
                        jobDate = string.Format("{0:0000}{1:00}{2:00}", (Int32.Parse(tempDt.Year.ToString())),
                            tempDt.Month, tempDt.Day);
                    }
                    else
                    {
                        Logging.Log("檢核參數：異常！ 參數：" + strParam, strJobID, LogState.Error, LogLayer.BusinessRule);
                        status = "F";
                        batchLogMsg = "手動設置參數啟動排程 檢核參數：異常！ 參數：" + strParam;
                        EddaUploadFailData_SendMail(strJobTitle, batchLogMsg);
                        return;
                    }
                }
                else
                {
                    Logging.Log("檢核參數：異常！ 參數：" + strParam, strJobID, LogState.Error, LogLayer.BusinessRule);
                    status = "F";
                    batchLogMsg = "手動設置參數啟動排程 檢核參數：異常！ 參數：" + strParam;
                    EddaUploadFailData_SendMail(strJobTitle, batchLogMsg);
                    return;
                }

                Logging.Log("檢核輸入參數：結束！", strJobID, LogState.Info, LogLayer.BusinessRule);
            }
            else
            {
                Logging.Log("手動輸入參數啟動排程：否！", strJobID, LogState.Info, LogLayer.BusinessRule);
            }

            Logging.Log("判斷是否手動輸入參數 啟動排程：結束！", strJobID, LogState.Info, LogLayer.BusinessRule);

            #endregion

            DataSet ds = GetDataFromEDDA_CSIP();
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                Logging.Log(string.Format("匯出 ach-rej-{0}.txt 檔案,開始！", jobDate), strJobID, LogState.Info, LogLayer.BusinessRule);
                if (BatchOutput(ds.Tables[0]))
                {
                    Logging.Log(string.Format("匯出 ach-rej-{0}.txt 檔案,成功！", jobDate), strJobID, LogState.Info, LogLayer.BusinessRule);

                    Logging.Log(string.Format("上傳 ach-rej-{0}.txt 檔案,開始！", jobDate), strJobID, LogState.Info, LogLayer.BusinessRule);
                    if (UploadToFTP(strJobID, string.Format("ach-rej-{0}", jobDate)))
                    {
                        Logging.Log(string.Format("上傳 ach-rej-{0}.txt 檔案,成功！", jobDate), strJobID, LogState.Info, LogLayer.BusinessRule);
                    }
                    else
                    {
                        Logging.Log(string.Format("上傳 ach-rej-{0}.txt 檔案,失敗！", jobDate), strJobID, LogState.Error, LogLayer.BusinessRule);
                        status = "F";
                        batchLogMsg = string.Format("上傳 ach-rej-{0}.txt 檔案,失敗！", jobDate);
                        EddaUploadFailData_SendMail(strJobTitle, batchLogMsg);
                        return;
                    }
                }
                else
                {
                    Logging.Log(string.Format("匯出 ach-rej-{0}.txt 檔案,失敗！", jobDate), strJobID, LogState.Error, LogLayer.BusinessRule);
                    status = "F";
                    batchLogMsg = string.Format("匯出 ach-rej-{0}.txt 檔案,失敗！", jobDate);
                    EddaUploadFailData_SendMail(strJobTitle, batchLogMsg);
                    return;
                }

                Logging.Log(string.Format("匯出 ach-rej-h-{0}.txt 檔案,開始！", jobDate), strJobID, LogState.Info, LogLayer.BusinessRule);
                if (BatchOutput_hFile(ds.Tables[0]))
                {
                    Logging.Log(string.Format("匯出 ach-rej-h-{0}.txt 檔案,成功！", jobDate), strJobID, LogState.Info, LogLayer.BusinessRule);

                    Logging.Log(string.Format("上傳 ach-rej-h-{0}.txt 檔案,開始！", jobDate), strJobID, LogState.Info, LogLayer.BusinessRule);
                    if (UploadToFTP(strJobID, string.Format("ach-rej-h-{0}", jobDate)))
                    {
                        Logging.Log(string.Format("上傳 ach-rej-h-{0}.txt 檔案,成功！", jobDate), strJobID, LogState.Info, LogLayer.BusinessRule);

                        Logging.Log("更新核印失敗資料的上傳註記與時間,開始！", strJobID, LogState.Info, LogLayer.BusinessRule);
                        if (UpdateAuto_Pay_Auth_Fail())
                        {
                            Logging.Log("更新核印失敗資料的上傳註記與時間,成功！", strJobID, LogState.Info, LogLayer.BusinessRule);
                        }
                        else
                        {
                            Logging.Log("更新核印失敗資料的上傳註記與時間,失敗！", strJobID, LogState.Error, LogLayer.BusinessRule);
                            status = "F";
                            batchLogMsg = "更新核印失敗資料的上傳註記與時間,失敗！";
                            EddaUploadFailData_SendMail(strJobTitle, batchLogMsg);
                            return;
                        }

                        Logging.Log("更新EDDA其他核印失敗資料的上傳註記與時間,開始！", strJobID, LogState.Info, LogLayer.BusinessRule);
                        if (UpdateEDDA_Auto_Pay())
                        {
                            Logging.Log("更新EDDA其他核印失敗資料的上傳註記與時間,成功！", strJobID, LogState.Info, LogLayer.BusinessRule);
                            batchLogMsg = "上傳EDDA核印失敗資料給卡主機,成功！";
                            status = "S";
                        }
                        else
                        {
                            Logging.Log("更新EDDA其他核印失敗資料的上傳註記與時間,失敗！", strJobID, LogState.Error, LogLayer.BusinessRule);
                            status = "F";
                            batchLogMsg = "更新EDDA其他核印失敗資料的上傳註記與時間,失敗！";
                            EddaUploadFailData_SendMail(strJobTitle, "更新EDDA其他核印失敗資料的上傳註記與時間,失敗！");
                            return;
                        }
                    }
                    else
                    {
                        Logging.Log(string.Format("上傳 ach-rej-h-{0}.txt 檔案,失敗！", jobDate), strJobID, LogState.Error, LogLayer.BusinessRule);
                        status = "F";
                        batchLogMsg = string.Format("上傳 ach-rej-h-{0}.txt 檔案,失敗！", jobDate);
                        EddaUploadFailData_SendMail(strJobTitle, batchLogMsg);
                        return;
                    }
                }
                else
                {
                    Logging.Log(string.Format(string.Format("匯出 ach-rej-h-{0}.txt 檔案,失敗！", jobDate), strJobID, LogState.Error, LogLayer.BusinessRule));
                    status = "F";
                    batchLogMsg = string.Format("匯出 ach-rej-h-{0}.txt 檔案,失敗！", jobDate);
                    EddaUploadFailData_SendMail(strJobTitle, batchLogMsg);
                    return;
                }
            }
            else
            {
                Logging.Log("查無資料！", strJobID, LogState.Info, LogLayer.BusinessRule);
                batchLogMsg = "查無資料！";
                status = "S";
            }


            string strMsg = strJobID + "執行於:" + DateTime.Parse(context.FireTimeUtc.ToString()).AddHours(8).ToString();
            if (context.NextFireTimeUtc.HasValue)
            {
                strMsg += "  ;下次執行於:" + DateTime.Parse(context.NextFireTimeUtc.ToString()).AddHours(8).ToString();
            }
            Logging.Log(strMsg, strJobID, LogState.Info, LogLayer.DB);
        }
        catch (Exception ex)
        {
            status = "F";
            batchLogMsg += ex.ToString();
            Logging.Log(ex.ToString(), strJobID, LogState.Error, LogLayer.DB);
        }
        finally
        {
            BRL_BATCH_LOG.Delete(strFuncKey, strJobID, "R");
            BRL_BATCH_LOG.Insert(strFuncKey, strJobID, dtStart, status, batchLogMsg);
        }
    }


    /// <summary>
    /// 查出EDDA、CSIP(他行、郵局)核印失敗未上傳的資料
    /// </summary>
    /// <returns></returns>
    public static DataSet GetDataFromEDDA_CSIP()
    {
        //* 新增 MATAINDATE
        string strSql = @"SELECT APAF.CustId,          -- 客戶ID
                                   APAF.ErrorCode,       -- 失敗代碼
                                   APAF.IssueChannel,    -- 申請通路
                                   APAF.IssueDate,       -- 申請日期
                                   CASE APAF.DataType
                                       WHEN '0' THEN ARI1.EDDA_Rtn_Msg
                                       WHEN '1' THEN ARI2.Ach_Rtn_Msg
                                       WHEN '2' THEN PRI.PostRtnMsg
                                       END AS FailReason -- 失敗原因
                            FROM Auto_Pay_Auth_Fail APAF
                            LEFT JOIN Ach_Rtn_Info ARI1 ON ARI1.EDDA_Rtn_Code = APAF.ErrorCode
                            LEFT JOIN Ach_Rtn_Info ARI2 ON ARI2.Ach_Rtn_Code = APAF.ErrorCode
                            LEFT JOIN PostOffice_Rtn_Info PRI ON PRI.PostRtnCode = APAF.ErrorCode
                            WHERE APAF.UploadFlag <> 'Y'; ";

        SqlCommand sqlComm = new SqlCommand();
        sqlComm.CommandType = CommandType.Text;
        sqlComm.CommandText = strSql;

        return BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");
    }

    /// <summary>
    /// 組明細檔
    /// </summary>
    /// <param name="dt"></param>
    public bool BatchOutput(DataTable dt)
    {
        try
        {
            string strTXT = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                strTXT += dt.Rows[i]["CustId"].ToString().Trim().ToUpper().PadRight(16, ' '); //客戶ID
                strTXT += dt.Rows[i]["ErrorCode"].ToString().PadRight(2, ' '); //失敗代碼
                strTXT += dt.Rows[i]["IssueChannel"].ToString().Trim().PadRight(4, ' '); //申請通路
                strTXT += dt.Rows[i]["IssueDate"].ToString().Trim().PadRight(8, ' '); //申請日期
                strTXT += dt.Rows[i]["FailReason"].ToString().Trim().PadRight(30, ' '); //失敗原因
                strTXT += "\r\n";
            }

            strTXT += "EOF";

            string uploadFolder = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileUpload");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            string strPah = uploadFolder + @"\" + string.Format("ach-rej-{0}.txt", jobDate);
            if (File.Exists(strPah))
            {
                File.Delete(strPah);
            }

            using (FileStream fs = File.Create(strPah, 1024))
            {
                StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("BIG5"));
                sw.Write(strTXT);
                sw.Flush();
                sw.Close();
            }

            return true;
        }
        catch (Exception ex)
        {
            Logging.Log(ex.ToString(), strJobID, LogState.Error, LogLayer.DB);
            return false;
        }
    }

    /// <summary>
    /// 組Header檔
    /// </summary>
    /// <param name="dt"></param>
    public bool BatchOutput_hFile(System.Data.DataTable dt)
    {
        try
        {
            string strTXT = "";
            strTXT += jobDate; //執行日期
            strTXT += dt.Rows.Count.ToString().PadLeft(5, '0'); //筆數
            strTXT += "\r\n";
            strTXT += "EOF";

            string uploadFolder = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileUpload");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            string strPah = uploadFolder + @"\" + string.Format("ach-rej-h-{0}.txt", jobDate);
            if (File.Exists(strPah))
            {
                File.Delete(strPah);
            }

            using (FileStream fs = File.Create(strPah, 1024))
            {
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("BIG5"));
                sw.Write(strTXT);
                sw.Flush();
                sw.Close();
            }

            return true;
        }
        catch (Exception ex)
        {
            Logging.Log(ex.ToString(), strJobID, LogState.Error, LogLayer.DB);
            return false;
        }
    }

    public static bool UpdateAuto_Pay_Auth_Fail()
    {
        string strSql = @"
                        UPDATE Auto_Pay_Auth_Fail
                        SET UploadFlag = 'Y', UploadTime = GETDATE()
                        WHERE UploadFlag <> 'Y'; 

                        UPDATE A
                        SET A.UploadFlag = '1', A.UploadTime = GETDATE()
                        FROM EDDA_Auto_Pay AS A
                        INNER JOIN Auto_Pay_Auth_Fail AS B ON A.BatchDate = B.BatchDate AND A.AuthCode = B.SerialNumber AND A.Cus_ID = B.CustId
                        WHERE A.UploadFlag = '0';";

        SqlCommand sqlComm = new SqlCommand();
        sqlComm.CommandType = CommandType.Text;
        sqlComm.CommandText = strSql;

        return BRBase<Entity_SP>.Update(sqlComm, "Connection_System");
    }

    public static bool UpdateEDDA_Auto_Pay()
    {
        string strSql = @"UPDATE EDDA_Auto_Pay
                        SET UploadFlag = '2', UploadTime = GETDATE()
                        WHERE Reply_Info NOT IN ('A0', 'A1', 'A2', 'A3', 'A4', 'A5', 'A6', 'A7', 'A8', 'AB', 'AC', 'AF', 'AG', 'AI', 'AJ') AND UploadFlag = '0'; ";

        SqlCommand sqlComm = new SqlCommand();
        sqlComm.CommandType = CommandType.Text;
        sqlComm.CommandText = strSql;

        return BRBase<Entity_SP>.Update(sqlComm, "Connection_System");
    }

    // 判斷Job工作狀態(0:停止 1:運行)
    public static bool CheckJobIsContinue(string JobID, string strFunctionKey, DateTime dateStart, ref string msgID)
    {
        bool result = true;
        string jobStatus = JobHelper.SerchJobStatus(JobID);
        if (jobStatus.Equals("") || jobStatus.Equals("0"))
        {
            // Job停止
            Logging.Log("【FAIL】 Job工作狀態為：停止！", JobID, LogState.Info, LogLayer.BusinessRule);

            result = false;
        }

        // 檢測Job是否在執行中
        try
        {
            DataTable dtInfo = BRL_BATCH_LOG.GetRunningDate(strFunctionKey, JobID, "R", ref msgID);
            if (dtInfo == null || dtInfo.Rows.Count > 0) //20210531_Ares_Stanley-修正Job執行檢核條件
            {
                Logging.Log("JOB 工作狀態為：正在執行！", JobID, LogState.Info, LogLayer.BusinessRule);
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
            Logging.Log("【FAIL】" + ex.ToString(), JobID, LogState.Error, LogLayer.BusinessRule);
        }

        return result;
    }

    /// <summary>
    /// EddaUploadFailData 寄信功能
    /// </summary>
    /// <param name="strSubject">信件標題</param>
    /// <param name="strBody">信件內文</param>
    /// <returns></returns>
    private bool EddaUploadFailData_SendMail(string strSubject, string strBody)
    {
        try
        {
            string strFrom = UtilHelper.GetAppSettings("MailSender"); // 發件人
            string[] sAddressee = { "" };
            if (!string.IsNullOrWhiteSpace(strMailList))
            {
                sAddressee = strMailList.Split(';');
            }

            Logging.Log("開始寄信！", strJobID, LogState.Info, LogLayer.BusinessRule);
            if (JobHelper.SendMail(strFrom, sAddressee, strSubject, strBody))
            {
                Logging.Log("寄信成功！", strJobID, LogState.Info, LogLayer.BusinessRule);

                return true;
            }

            Logging.Log("寄信失敗！", strJobID, LogState.Error, LogLayer.BusinessRule);

            return false;
        }
        catch (Exception exp)
        {
            BRBase<Entity_SP>.SaveLog(exp.Message);
            return false;
        }
    }

    private bool UploadToFTP(string jobId, string fileName)
    {
        bool isOk = false;
        DataTable dt = BRM_FileInfo.GetFtpInfoByJobId(jobId);
        if (dt != null && dt.Rows.Count > 0)
        {
            string remoteHost = dt.Rows[0]["FtpIP"].ToString().Trim();
            string remotePath = dt.Rows[0]["FtpPath"].ToString().Trim();
            string remoteUser = dt.Rows[0]["FtpUserName"].ToString().Trim();
            string remotePass = RedirectHelper.GetDecryptString(dt.Rows[0]["FtpPwd"].ToString().Trim());
            string remoteFile = fileName.Trim();

            // Get the object used to communicate with the server.
            if (remotePath.Substring(remotePath.Length - 1) != "/")
            {
                remotePath += "/";
            }

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Format("ftp://{0}/{1}{2}.txt",
                    remoteHost, remotePath, remoteFile));
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Proxy = null;
                request.Credentials = new NetworkCredential(remoteUser, remotePass);

                // Copy the contents of the file to the request stream.
                string strlocalFilePah =
                    AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileUpload");

                strlocalFilePah += (@"\" + remoteFile + ".txt");
                if (File.Exists(strlocalFilePah))
                {
                    StreamReader sourceStream =
                        new StreamReader(strlocalFilePah, Encoding.GetEncoding("BIG5"));
                    byte[] fileContents = Encoding.GetEncoding("BIG5").GetBytes(sourceStream.ReadToEnd());
                    sourceStream.Close();
                    request.ContentLength = fileContents.Length;

                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(fileContents, 0, fileContents.Length);
                    requestStream.Close();

                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                    Logging.Log("FTP上傳完成, 狀態：" + response.StatusDescription, LogLayer.Util);
                    isOk = true;
                    response.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.Log("FTP上傳失敗：" + ex.ToString(), LogState.Error, LogLayer.Util);
            }
        }

        return isOk;
    }
}