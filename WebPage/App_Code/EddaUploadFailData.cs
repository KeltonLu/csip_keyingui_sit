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
    private readonly JobHelper _jobHelper = new JobHelper();
    private readonly string _functionKey = UtilHelper.GetAppSettings("FunctionKey");
    private string _jobId = "EddaUploadFailData";
    private string _strJobTitle = string.Empty;
    private string _strMailList = string.Empty;
    private readonly string _jobDate = DateTime.Now.ToString("yyyMMdd");
    private readonly string _fileNameHeader = string.Format("ach-rej-h-{0:yyyyMMdd}.txt", DateTime.Now);
    private readonly string _fileNameDetail = string.Format("ach-rej-{0:yyyyMMdd}.txt", DateTime.Now);

    public void Execute(JobExecutionContext context)
    {
        DateTime dtStart = DateTime.Now;
        string batchLogMsg = string.Empty;
        string status = "F"; // S:成功、F：失敗、R：上一個排程執行中(需略過)
        int totalCount = 0;
        try
        {
            // 取得排程參數
            _jobId = context.JobDetail.JobDataMap.GetString("jobid").Trim();
            _strJobTitle = context.JobDetail.JobDataMap.GetString("title").Trim();
            _strMailList = context.JobDetail.JobDataMap.GetString("mail").Trim();

            // 判斷排程是否要繼續執行
            var isContinue = CheckJobIsContinue(_jobId, _functionKey, dtStart, ref batchLogMsg, ref status);
            if (!isContinue) return;

            DataTable dt = new DataTable();
            DataSet ds = GetAuthFailData();
            if (ds == null || ds.Tables[0].Rows.Count == 0)  // 查無核印失敗的資料
            {
                WriteLog("查無資料！");
            }
            else
            {
                dt = ds.Tables[0];
                totalCount = dt.Rows.Count;
            }
            
            // 產生明細檔案
            if (!CreateDetailFile(dt))
            {
                status = "F";
                batchLogMsg = string.Format("產生 {0} 檔案,失敗！ 核印失敗資料筆數：{1}", _fileNameDetail, dt.Rows.Count);
                SendEmail(_strJobTitle + batchLogMsg, batchLogMsg);
                return;
            }
                
            // 產生header檔案
            if (!CreateHeaderFile(dt))
            {
                status = "F";
                batchLogMsg = string.Format("產生 {0} 檔案,失敗！", _fileNameHeader);
                SendEmail(_strJobTitle + batchLogMsg, batchLogMsg);
                return;
            }
                
            // 上傳明細檔案
            if (!UploadToFtp(_fileNameDetail))
            {
                status = "F";
                batchLogMsg = string.Format("上傳 {0} 檔案,失敗！", _fileNameDetail);
                SendEmail(_strJobTitle + batchLogMsg, batchLogMsg);
                return;
            }
                
            // 上傳header檔案
            if (!UploadToFtp(_fileNameHeader))
            {
                status = "F";
                batchLogMsg = string.Format("上傳 {0} 檔案,失敗！", _fileNameHeader);
                SendEmail(_strJobTitle + batchLogMsg, batchLogMsg);
                return;
            }
            
            // 更新核印失敗資料的上傳註記與時間
            string logMsg = "更新核印失敗資料的上傳註記與時間";
            if (!UpdateAutoPayAuthFail(logMsg))
            {
                status = "F";
                batchLogMsg = string.Format("{0},失敗！", logMsg);
                SendEmail(_strJobTitle + batchLogMsg, batchLogMsg);
                return;
            }
            
            // 更新EDDA其他核印失敗資料的上傳註記與時間
            logMsg = "更新EDDA其他核印失敗資料的上傳註記與時間";
            if (!UpdateEddaAutoPay(logMsg))
            {
                status = "F";
                batchLogMsg = string.Format("{0},失敗！", logMsg);
                SendEmail(_strJobTitle + batchLogMsg, batchLogMsg);
                return;
            }

            // header、明細 產檔、上傳、資料更新皆成功
            status = "S";
            batchLogMsg = "核印失敗資料筆數：" + totalCount;
            
            string strMsg = _jobId + "執行於:" + DateTime.Parse(context.FireTimeUtc.ToString()).AddHours(8);
            if (context.NextFireTimeUtc.HasValue)
            {
                strMsg += "  ;下次執行於:" + DateTime.Parse(context.NextFireTimeUtc.ToString()).AddHours(8);
            }
            Logging.Log(strMsg, _jobId, LogState.Info, LogLayer.DB);
        }
        catch (Exception ex)
        {
            status = "F";
            batchLogMsg += ex.ToString();
            Logging.Log(ex.ToString(), _jobId, LogState.Error, LogLayer.DB);
        }
        finally
        {
            // 若上一個排程仍執行中且不超過一小時
            if (!status.Equals("R"))
            {
                BRL_BATCH_LOG.Delete(_functionKey, _jobId, "R");
                BRL_BATCH_LOG.Insert(_functionKey, _jobId, dtStart, status, batchLogMsg);
            }
        }
    }
    
    /// <summary>
    /// 查出EDDA、CSIP(他行、郵局)核印失敗未上傳的資料
    /// </summary>
    /// <returns></returns>
    private DataSet GetAuthFailData()
    {
        //* 新增 MATAINDATE
        string strSql = @"SELECT APAF.CustId,          -- 客戶ID
                                   APAF.ErrorCode,       -- 失敗代碼
                                   APAF.IssueChannel,    -- 申請通路
                                   APAF.IssueDate,       -- 申請日期
                                   CASE APAF.DataType
                                       WHEN '0' THEN ERI.SendHostMsg
                                       WHEN '1' THEN ARI.SendHostMsg
                                       WHEN '2' THEN PRI.SendHostMsg
                                       END AS FailReason -- 失敗原因
                            FROM Auto_Pay_Auth_Fail APAF
                            LEFT JOIN EDDA_Rtn_Info ERI ON ERI.EddaRtnCode = APAF.ErrorCode
                            LEFT JOIN Ach_Rtn_Info ARI ON ARI.Ach_Rtn_Code = APAF.ErrorCode
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
    /// <param name="dt">核印失敗資料</param>
    private bool CreateDetailFile(DataTable dt)
    {
        WriteLog(string.Format("產生 {0} 檔案,開始！", _fileNameDetail));

        try
        {
            string content = "";
            foreach (DataRow row in dt.Rows)
            {
                content += SubstringAndPadRight(row["CustId"].ToString().Trim().ToUpper(), 16); // 客戶ID
                content += SubstringAndPadRight(row["ErrorCode"].ToString().Trim(), 2); // 失敗代碼
                content += SubstringAndPadRight(row["IssueChannel"].ToString().Trim(), 4); // 申請通路
                content += SubstringAndPadRight(row["IssueDate"].ToString().Trim(), 8); // 申請日期

                //失敗原因
                var failReason = row["FailReason"].ToString().Trim();
                var failReasonLength = GetFailReasonLength(failReason); // 取得包含中文的實際長度
                if (failReasonLength > 20)
                {
                    failReason = ChineseSubStr(failReason, 0, 20);
                }
                else
                {
                    failReason += "".PadRight(20 - failReasonLength, ' ');
                }
                content += failReason;
                content += "\r\n";
            }

            string uploadFolder = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileUpload");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            string strPah = uploadFolder + @"\" + _fileNameDetail;
            if (File.Exists(strPah))
            {
                File.Delete(strPah);
            }

            using (FileStream fs = File.Create(strPah, 1024))
            {
                StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("BIG5"));
                sw.Write(content);
                sw.Flush();
                sw.Close();
            }

            WriteLog(string.Format("產生 {0} 檔案,成功！ 核印失敗資料筆數：{1}", _fileNameDetail, dt.Rows.Count));
            return true;
        }
        catch (Exception ex)
        {
            Logging.Log(ex.ToString(), _jobId, LogState.Error, LogLayer.DB);
            WriteLog(string.Format("產生 {0} 檔案,失敗！ 核印失敗資料筆數：{1}", _fileNameDetail, dt.Rows.Count), LogState.Error);
            return false;
        }
    }

    /// <summary>
    /// 組Header檔
    /// </summary>
    /// <param name="dt">核印失敗資料</param>
    private bool CreateHeaderFile(DataTable dt)
    {
        WriteLog(string.Format("產生 {0} 檔案,開始！", _fileNameHeader));
        
        try
        {
            string content = _jobDate; // 執行日期+資料筆數(向左補零至五位數)
            if (dt.Rows.Count > 0)
            {
                content += dt.Rows.Count.ToString().PadLeft(5, '0'); //筆數
            }
            else
            {
                content += "00000"; //筆數
            }

            string uploadFolder = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileUpload");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            string strPah = uploadFolder + @"\" + _fileNameHeader;
            if (File.Exists(strPah))
            {
                File.Delete(strPah);
            }

            using (FileStream fs = File.Create(strPah, 1024))
            {
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("BIG5"));
                sw.Write(content);
                sw.Flush();
                sw.Close();
            }

            WriteLog(string.Format("產生 {0} 檔案,成功！", _fileNameHeader));
            return true;
        }
        catch (Exception ex)
        {
            Logging.Log(ex.ToString(), _jobId, LogState.Error, LogLayer.DB);
            WriteLog(string.Format("產生 {0} 檔案,失敗！", _fileNameHeader), LogState.Error);
            return false;
        }
    }
    
    /// <summary>
    /// 更新核印失敗資料的上傳註記與時間
    /// </summary>
    /// <param name="logMsg">log message</param>
    /// <returns></returns>
    private bool UpdateAutoPayAuthFail(string logMsg)
    {
        WriteLog(string.Format("{0},開始！", logMsg));
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
        bool result = BRBase<Entity_SP>.Update(sqlComm, "Connection_System");
        WriteLog(result ? string.Format("{0},成功！", logMsg) : string.Format("{0},失敗！", logMsg), result ? LogState.Info : LogState.Error);
        return result;
    }

    /// <summary>
    /// 更新EDDA其他核印失敗資料的上傳註記與時間
    /// </summary>
    /// <param name="logMsg">log message</param>
    /// <returns></returns>
    private bool UpdateEddaAutoPay(string logMsg)
    {
        WriteLog(string.Format("{0},開始！", logMsg));
        string strSql = @"UPDATE EDDA_Auto_Pay
                          SET UploadFlag = '2', UploadTime = GETDATE()
                          WHERE UploadFlag = '0'
                          AND Reply_Info NOT IN (SELECT EddaRtnCode FROM EDDA_Rtn_Info WHERE NeedSendHost = 'Y' OR EddaRtnCode IN ('A0', 'A4'));";

        SqlCommand sqlComm = new SqlCommand();
        sqlComm.CommandType = CommandType.Text;
        sqlComm.CommandText = strSql;
        bool result = BRBase<Entity_SP>.Update(sqlComm, "Connection_System");
        WriteLog(result ? string.Format("{0},成功！", logMsg) : string.Format("{0},失敗！", logMsg), result ? LogState.Info : LogState.Error);
        return result;
    }

    /// <summary>
    /// 判斷排程是否要繼續執行
    /// </summary>
    /// <param name="jobId">排程代號</param>
    /// <param name="functionKey">功能代號</param>
    /// <param name="dateStart">排程開姞時間</param>
    /// <param name="msgId">訊息代碼(Common/XML/Message.xml)</param>
    /// <param name="rtnStatus">若判斷上一個排程仍執行中回傳值為「R」</param>
    /// <returns>true or false</returns>
    private bool CheckJobIsContinue(string jobId, string functionKey, DateTime dateStart, ref string msgId, ref string rtnStatus)
    {
        bool result = true;
        string jobStatus = _jobHelper.SerchJobStatus(jobId);
        if (jobStatus.Equals("") || jobStatus.Equals("0"))
        {
            // Job停止
            Logging.Log("【FAIL】 Job工作狀態為：停止！", jobId, LogState.Info, LogLayer.BusinessRule);

            result = false;
        }

        // 檢測Job是否在執行中
        try
        {
            DataTable dtInfo = BRL_BATCH_LOG.GetRunningDate(functionKey, jobId, "R", ref msgId);
            if (dtInfo == null || dtInfo.Rows.Count > 0)
            {
                //判斷執行時間超過一小時
                if (dtInfo != null)
                {
                    DateTime tempDt;
                    DateTime.TryParse(dtInfo.Rows[0][2].ToString(), out tempDt);
                    TimeSpan ts = DateTime.Now.Subtract(tempDt);
                    int dayCount = ts.Hours; // 執行中紀錄的開始時間與當天執行時間相差小時
                    if (dayCount > 1)
                    {
                        BRL_BATCH_LOG.Delete(functionKey, jobId, "R");
                        BRL_BATCH_LOG.InsertRunning(functionKey, jobId, dateStart, "R", "");
                        return true;
                    }
                }
                Logging.Log("JOB 工作狀態為：正在執行！", jobId, LogState.Info, LogLayer.BusinessRule);
                // 返回不執行
                rtnStatus = "R";
                result = false;
            }
            else
            {
                // 記錄Job執行資訊
                BRL_BATCH_LOG.InsertRunning(functionKey, jobId, dateStart, "R", "");
            }
        }
        catch (Exception ex)
        {
            result = false;
            Logging.Log("【FAIL】" + ex, jobId, LogState.Error, LogLayer.BusinessRule);
        }

        return result;
    }

    /// <summary>
    /// EddaUploadFailData 寄信功能
    /// </summary>
    /// <param name="strSubject">信件標題</param>
    /// <param name="strBody">信件內文</param>
    /// <returns></returns>
    private bool SendEmail(string strSubject, string strBody)
    {
        try
        {
            string strFrom = UtilHelper.GetAppSettings("MailSender"); // 發件人
            string[] sAddressee = { "" };
            if (!string.IsNullOrWhiteSpace(_strMailList))
            {
                sAddressee = _strMailList.Split(';');
            }
        
            Logging.Log("開始寄信！", _jobId, LogState.Info, LogLayer.BusinessRule);
            if (_jobHelper.SendMail(strFrom, sAddressee, strSubject, strBody))
            {
                Logging.Log("寄信成功！", _jobId, LogState.Info, LogLayer.BusinessRule);
        
                return true;
            }
        
            Logging.Log("寄信失敗！", _jobId, LogState.Error, LogLayer.BusinessRule);
        
            return false;
        }
        catch (Exception exp)
        {
            BRBase<Entity_SP>.SaveLog(exp.Message);
            return false;
        }
    }

    /// <summary>
    /// FTP上傳
    /// </summary>
    /// <param name="fileName">檔案名稱(含副檔名)</param>
    /// <returns></returns>
    private bool UploadToFtp(string fileName)
    {
        WriteLog(string.Format("上傳 {0} 檔案,開始！", fileName));
        
        bool isOk = false;
        DataTable dt = BRM_FileInfo.GetFtpInfoByJobId(_jobId);
        if (dt != null && dt.Rows.Count > 0)
        {
            string remoteHost = dt.Rows[0]["FtpIP"].ToString().Trim();
            string remotePath = dt.Rows[0]["FtpPath"].ToString().Trim();
            string remoteUser = dt.Rows[0]["FtpUserName"].ToString().Trim();
            string remotePass = RedirectHelper.GetDecryptString(dt.Rows[0]["FtpPwd"].ToString().Trim());
            // string remoteFile = fileName.Trim();

            // Get the object used to communicate with the server.
            if (remotePath.Substring(remotePath.Length - 1) != "/")
            {
                remotePath += "/";
            }

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Format("ftp://{0}/{1}{2}", remoteHost, remotePath, fileName));
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Proxy = null;
                request.Credentials = new NetworkCredential(remoteUser, remotePass);

                // Copy the contents of the file to the request stream.
                string localFilePah = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileUpload");

                localFilePah += @"\" + fileName;
                if (File.Exists(localFilePah))
                {
                    StreamReader sourceStream = new StreamReader(localFilePah, Encoding.GetEncoding("BIG5"));
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
                    
                    WriteLog(string.Format("上傳 {0} 檔案,成功！", fileName));
                }
            }
            catch (Exception ex)
            {
                Logging.Log("FTP上傳失敗：" + ex, LogState.Error, LogLayer.Util);
                WriteLog(string.Format("上傳 {0} 檔案,失敗！", fileName), LogState.Error);
            }
        }

        return isOk;
    }

    /// <summary>
    /// 通用log
    /// </summary>
    /// <param name="msg">log訊息</param>
    /// <param name="logState">LogState</param>
    private void WriteLog(string msg, LogState logState = LogState.Info)
    {
        Logging.Log(msg, _jobId, logState, LogLayer.BusinessRule);
    }
    
    /// <summary>
    /// 若字串超過「指定長度」則進行substring並且將字串左靠右補空白至「指定長度」
    /// </summary>
    /// <param name="str"></param>
    /// <param name="len"></param>
    /// <returns></returns>
    private static string SubstringAndPadRight(string str, int len)
    {
        var rtnString = str.Length > len ? str.Substring(0, len) : str;
        return rtnString.PadRight(len, ' ');
    }

    /// <summary>
    /// 取得失敗原因(中文)長度
    /// </summary>
    /// <returns></returns>
    private static int GetFailReasonLength(string str)
    {
        // 將字串轉為byte 
        var byteStr = Encoding.GetEncoding("BIG5").GetBytes(str);
        // 取byte的長度
        return byteStr.Length; 
    }
    
    /// <summary>
    /// 針對中文使用substring
    /// </summary>
    /// <param name="str">文字</param>
    /// <param name="startIndex">開始index</param>
    /// <param name="length">截取長度</param>
    /// <returns></returns>
    private static string ChineseSubStr(string str, int startIndex, int length)
    {
        var encoding = Encoding.GetEncoding("BIG5", new EncoderExceptionFallback(), new DecoderReplacementFallback(""));
        var bytes = encoding.GetBytes(str);

        if (length <= 0)
            return "";

        if (startIndex + 1 > bytes.Length)
            return "";

        if (startIndex + length > bytes.Length)
        {
            length = bytes.Length - startIndex;
        }

        return encoding.GetString(bytes, startIndex, length);
    }
}