//******************************************************************
//*  作    者：Ares Stanley
//*  功能說明：讀檔後刪除舊資料並新增新資料至AML_HQ_MFAN
//*  創建日期：2021/11/25
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
using CSIPNewInvoice.EntityLayer_new;
using Framework.Common.IO;
using Framework.Common.Logging;
using Framework.Common.Utility;
using Quartz;

/// <summary>
/// 專案代號:20200031-CSIP EOS AML NOVA 需求擴增
/// 功能說明:讀檔後刪除舊資料並新增新資料至AML_HQ_MFAN
/// 作    者:Ares Stanley
/// 創建時間:2021/11/25
/// 修改紀錄:
/// </summary>
public class BatchJob_MFAN : Quartz.IJob
{
    private static readonly JobHelper JobHelper = new JobHelper();
    private readonly string _strFunctionKey = "01";
    private static string _strJobId;
    DateTime _dateStart; //開始時間
    protected string _MailTitle = "MFAN資料刪除後新增結果 批次：";// 20220407 修改說明:增加此批次執行成功後，發送成功 email by Kelton 

    #region sql

    string del_AML_HQ_MFAN =
        @"DELETE FROM AML_HQ_MFAN";
    string insert_AML_HQ_MFAN = @"INSERT INTO [dbo].[AML_HQ_MFAN]
    ([MFA_ID], 
    [MFA_NAME], 
    [MFA_AREA], 
    [MOD_DATE]) 
    VALUES (
    @MFA_ID, 
    @MFA_NAME, 
    @MFA_AREA, 
    @MOD_DATE);";
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

            string sFile_Path = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileUpload") + "\\" + _strJobId + "\\";

            // 收檔日
            string sDateReceive = string.Empty;

            // YYYYMMDD
            string sToday = DateTime.Now.ToString("yyyyMMdd");

            // 發件人
            string sAddresser = UtilHelper.GetAppSettings("MailSender");
            // 收件人
            string[] sAddressee = { "" };

            DataSet ds = new DataSet();

            string resultMsg = string.Empty;

            #endregion

            // 20220420 發送 Mail 的對像改為由 DB 取得 By Kelton
            //string jobMailTo = jobDataMap.GetString("mail").Trim();
            string jobMailTo = new Com_AutoJob().GetEmailMembers("01", _strJobId);
            jobMailTo = jobMailTo.Substring(0, jobMailTo.Length - 1);

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
            //20200031-CSIP EOS Ares Luke 修改日期:2021/02/22 修改說明:業務需求將日期格式為民國年8碼
            DateTime dt = DateTime.Now;
            sToday = string.Format("{0:0000}{1:00}{2:00}", dt.Year.ToString(), dt.Month, dt.Day);

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
                        sToday = string.Format("{0:0000}{1:00}{2:00}", tempDt.Year.ToString(), tempDt.Month, tempDt.Day);

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

            #region 從FTP取檔
            //建立Job資料夾
            string folderName = string.Empty;
            bool isInsertOK = false;
            JobHelper.CreateFolderName(JobHelper.strJobID, ref folderName);
            string localPath = AppDomain.CurrentDomain.BaseDirectory + "FileDownload\\" + JobHelper.strJobID + "\\" + folderName + "\\";

            string errorMsg = string.Empty;
            bool isDownload = false;
            string fileNameMFAN = DownloadFileFromFTP(JobHelper.strJobID, localPath, sToday, ref isDownload, ref resultMsg);
            if (!isDownload)
            {
                Batch_log("F", resultMsg);
                // 20220420 檔案不存在發送 Mail By Kelton
                JobHelper.SendMail(sAddresser, sAddressee, _MailTitle + "失敗！檔案:" + fileNameMFAN + " 不存在", "檔案:" + fileNameMFAN + " 不存在， FTP 取檔失敗");
                return;
            }
            #endregion

            #region 檢核、取資料
            JobHelper.Write(JobHelper.strJobID, "資料檢核開始");
            DataTable dtMFAN = new DataTable();
            if (!string.IsNullOrEmpty(fileNameMFAN))
            {
                dtMFAN = GetCFileData(localPath, fileNameMFAN, _dateStart.ToString("yyyy-MM-dd HH:mm:ss.fff"), out errorMsg);
                if (!(string.IsNullOrEmpty(errorMsg)))
                {
                    Batch_log("F", errorMsg);
                    JobHelper.Write(JobHelper.strJobID, "讀取要匯入的AML_HQ_MFAN的檔案資料失敗！", LogState.Info);
                    // 20220407 調整發送 mail 的 Title 為統一格式，故將原邏輯註解 Start by Kelton
                    //JobHelper.SendMail(sAddresser, sAddressee, "BatchJob_MFAN通知", fileNameMFAN + " 讀取要匯入的AML_HQ_MFAN的檔案資料失敗！");
                    // 20220407 調整發送 mail 的 Title 為統一格式，故將原邏輯註解 End by Kelton
                    // 20220407 修改說明:調整發送 mail 的 Title 為統一格式 by Kelton 
                    JobHelper.SendMail(sAddresser, sAddressee, _MailTitle + "失敗！總筆數：" + dtMFAN.Rows.Count + "筆", fileNameMFAN + " 讀取要匯入的AML_HQ_MFAN的檔案資料失敗！");
                    return;
                }
            }
            else
            {
                JobHelper.Write(JobHelper.strJobID, "無資料需檢核");
            }
            
            JobHelper.Write(JobHelper.strJobID, "資料檢核結束");
            #endregion

            #region 刪除、新增AML_HQ_MFAN資料
            bool delStatus = false;
            int insertFailCount = 0;
            string insertResultMsg = string.Empty;
            string mailTitle = string.Empty; // 20220414 執行成功發送 Mail 的標題文字 By Kelton
            string mailBody = string.Empty; // 20220414 執行成功發送 Mail 的內容文字 By Kelton

            JobHelper.Write(JobHelper.strJobID, "資料匯入開始", LogState.Info);
            if (fileNameMFAN != "" && dtMFAN.Rows.Count > 0)
            {
                //取資料備份
                DataTable dtBackup = new DataTable();
                dtBackup = GetAML_HQ_MFAN();

                //刪除資料
                JobHelper.Write(JobHelper.strJobID, "Delete AML_HQ_MFAN 開始！", LogState.Info);
                delStatus = Del_AML_HQ_MFAN(del_AML_HQ_MFAN);
                if (!delStatus)
                {
                    Batch_log("F", "Delete AML_HQ_MFAN 失敗！");
                    JobHelper.Write(JobHelper.strJobID, "Delete AML_HQ_MFAN 失敗！", LogState.Info);
                    // 20220407 調整發送 mail 的 Title 為統一格式，故將原邏輯註解 Start by Kelton
                    //JobHelper.SendMail(sAddresser, sAddressee, "BatchJob_MFAN通知", "刪除 AML_HQ_MFAN 資料失敗！");
                    // 20220407 調整發送 mail 的 Title 為統一格式，故將原邏輯註解 End by Kelton
                    // 20220407 修改說明:調整發送 mail 的 Title 為統一格式 by Kelton 
                    JobHelper.SendMail(sAddresser, sAddressee, _MailTitle + "失敗！總筆數：" + dtMFAN.Rows.Count + "筆", "刪除 AML_HQ_MFAN 資料失敗！");
                    return;
                }
                JobHelper.Write(JobHelper.strJobID, "Delete AML_HQ_MFAN 結束！", LogState.Info);

                //寫入資料
                JobHelper.Write(JobHelper.strJobID, "Insert AML_HQ_MFAN 開始！", LogState.Info);
                foreach (DataRow row in dtMFAN.Rows)
                {
                    isInsertOK = InsertAML_HQ_MFANImport(row, insert_AML_HQ_MFAN);
                    if (!isInsertOK)
                        insertFailCount += 1;
                }
                insertResultMsg = string.Format("Insert AML_HQ_MFAN ，成功 {0} 筆，失敗 {1} 筆，共 {2} 筆！", dtMFAN.Rows.Count - insertFailCount, insertFailCount, dtMFAN.Rows.Count);
                resultMsg = string.Format("成功 {0} 筆，失敗 {1} 筆，共 {2} 筆！", dtMFAN.Rows.Count - insertFailCount, insertFailCount, dtMFAN.Rows.Count);
                if (insertFailCount > 0)
                {
                    Batch_log("F", insertResultMsg);
                    JobHelper.Write(JobHelper.strJobID, insertResultMsg, LogState.Info);
                    // 20220407 調整發送 mail 的 Title 為統一格式，有 Insert 資料失敗的情況要等資料還原後才 Return，故將原邏輯註解 Start by Kelton
                    //JobHelper.SendMail(sAddresser, sAddressee, "BatchJob_MFAN通知", insertResultMsg);
                    //return;
                    // 20220407 調整發送 mail 的 Title 為統一格式，有 Insert 資料失敗的情況要等資料還原後才 Return，故將原邏輯註解 End by Kelton
                    // 20220407 修改說明:調整發送 mail 的 Title 為統一格式 by Kelton 
                    JobHelper.SendMail(sAddresser, sAddressee, _MailTitle + "失敗！總筆數：" + dtMFAN.Rows.Count + "筆", insertResultMsg);
                    // 20220407 修改說明:有 Insert 資料失敗的情況要等資料還原後才 Return by Kelton 
                }
                JobHelper.Write(JobHelper.strJobID, "Insert AML_HQ_MFAN 結束！", LogState.Info);
                
                //若insert失敗則復原資料
                if (!isInsertOK)
                {
                    JobHelper.Write(JobHelper.strJobID, "Recovery AML_HQ_MFAN 開始！", LogState.Info);

                    //刪除當前所有資料
                    JobHelper.Write(JobHelper.strJobID, "Recovery - Delete AML_HQ_MFAN 開始！", LogState.Info);
                    delStatus = Del_AML_HQ_MFAN(del_AML_HQ_MFAN);
                    if (!delStatus)
                    {
                        Batch_log("F", "Recovery - Delete AML_HQ_MFAN 失敗！");
                        JobHelper.Write(JobHelper.strJobID, "Recovery - Delete AML_HQ_MFAN 失敗！", LogState.Info);

                        // 20220407 調整發送 mail 的 Title 為統一格式，故將原邏輯註解 Start by Kelton
                        //JobHelper.SendMail(sAddresser, sAddressee, "BatchJob_MFAN通知", "復原 AML_HQ_MFAN 資料前刪除所有資料失敗！");
                        // 20220407 調整發送 mail 的 Title 為統一格式，故將原邏輯註解 End by Kelton
                        // 20220407 修改說明:調整發送 mail 的 Title 為統一格式 by Kelton 
                        JobHelper.SendMail(sAddresser, sAddressee, _MailTitle + "失敗！總筆數：" + dtMFAN.Rows.Count + "筆", "復原 AML_HQ_MFAN 資料前刪除所有資料失敗！");
                        return;
                    }
                    JobHelper.Write(JobHelper.strJobID, "Recovery - Delete AML_HQ_MFAN 結束！", LogState.Info);
                    
                    //復原資料
                    RecoveryBackupData(dtBackup);
                    JobHelper.Write(JobHelper.strJobID, "Recovery AML_HQ_MFAN 結束！", LogState.Info);

                    // 20220407 修改說明:有 Insert 資料失敗且失敗筆數大於 0 的情況要等資料還原後才 Return by Kelton 
                    if (insertFailCount > 0)
                        return;
                }

                // 20220414 修改說明:套用非空檔執行成功要發送 Mail 的標題及內容 by Kelton 
                mailTitle = _MailTitle + "成功！總筆數：" + dtMFAN.Rows.Count + "筆";
                mailBody = _MailTitle + fileNameMFAN + " 成功！總筆數：" + dtMFAN.Rows.Count + "筆，匯入成功 共" + dtMFAN.Rows.Count + "筆，匯入失敗0筆";
            }
            else
            {
                JobHelper.Write(JobHelper.strJobID, "無資料需匯入", LogState.Info);

                // 20220414 修改說明:套用空檔執行成功要發送 Mail 的標題及內容 by Kelton 
                mailTitle = _MailTitle + "成功！今日為空檔，無需處理";
                mailBody = _MailTitle + fileNameMFAN + " 成功！今日為空檔，無需處理";
                resultMsg = "今日為空檔，無需處理";
            }
            JobHelper.Write(JobHelper.strJobID, "資料匯入結束", LogState.Info);
            #endregion

            #endregion



            #region 結束批次作業

            Batch_log("S", resultMsg);
            JobHelper.SaveLog(_strJobId + "JOB結束", LogState.Info);
            // 20220407 修改說明:增加此批次執行成功後，發送成功 email by Kelton 
            JobHelper.SendMail(jobMailTo, mailTitle, mailBody, "成功", this._dateStart);

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
    /// 專案代號:20200031-CSIP EOS AML NOVA 需求擴增
    /// 功能說明:從FTP下載資料檔
    /// 作    者:Ares Stanley
    /// 創建時間:2021/11/25
    /// 修改時間:
    /// </summary>
    /// <param name="jobID"></param>
    /// <param name="localPath"></param>
    /// <param name="date"></param>
    /// <returns></returns>
    private string DownloadFileFromFTP(string jobID, string localPath, string date, ref bool isDownload, ref string resultMsg)
    {
        try
        {
            JobHelper.Write(jobID, "FTP 取檔開始");
            DataTable tblFileInfo = BRAML_File_Import.GetFileInfo(jobID);
            //20191230-RQ-2019-030155-002-批次信函調整：增加判斷如果Parameter有值，則使用Parameter的時間  by Peggy
            //如果Parameter有值，則使用Parameter的時間
            if (!string.IsNullOrEmpty(tblFileInfo.Rows[0]["Parameter"].ToString()))
            {
                date = tblFileInfo.Rows[0]["Parameter"].ToString().Trim();
            }

            string fileName = tblFileInfo.Rows[0]["FtpFileName"].ToString().Replace("yyyyMMdd", date);
            string ftpPwd = RedirectHelper.GetDecryptString(tblFileInfo.Rows[0]["FtpPwd"].ToString());
            FTPFactory objFtp = new FTPFactory(tblFileInfo.Rows[0]["FtpIP"].ToString(), "", tblFileInfo.Rows[0]["FtpUserName"].ToString(), ftpPwd, "21", localPath, "Y");

            //20191230-RQ-2019-030155-002-批次信函調整：增加取得取檔結果的參數 by Peggy
            isDownload = objFtp.Download(tblFileInfo.Rows[0]["FtpPath"].ToString(), fileName, localPath, fileName);

            if (isDownload)
            {
                JobHelper.Write(jobID, fileName + " FTP 取檔成功", LogState.Info);
                return fileName;
            }
            else
            {
                JobHelper.Write(jobID, "[FAIL] 檔案: " + fileName + " 不存在， FTP 取檔失敗");
                resultMsg = "[FAIL] 檔案: " + fileName + " 不存在， FTP 取檔失敗";
                // 20220420 檔案不存在也要回傳檔名，發 Mail 的時候需要用到 by Kelton
                //return "";
                return fileName;
            }

        }
        catch(Exception ex)
        {
            JobHelper.Write(jobID, "[FAIL] " + ex.ToString());
            resultMsg = "[FAIL] " + ex.ToString();
            return "";
        }
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS AML NOVA 需求擴增
    /// 功能說明:從檔案讀取匯入資料
    /// 作    者:Ares Stanley
    /// 創建時間:2021/11/25
    /// 修改時間:
    /// </summary>
    /// <param name="localPath"></param>
    /// <param name="fileNameMFAN"></param>
    /// <param name="_dateStart"></param>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    private DataTable GetCFileData(string localPath, string fileNameMFAN, string _dateStart, out string errorMsg)
    {
        JobHelper.Write(JobHelper.strJobID, "讀取要匯入的AML_HQ_MFAN的檔案資料，開始！", LogState.Info);

        DirectoryInfo di = new DirectoryInfo(localPath);
        DataTable dt = new DataTable();

        errorMsg = "";

        if (!Directory.Exists(localPath))
            Directory.CreateDirectory(localPath);

        foreach (FileInfo file in di.GetFiles())
        {
            try
            {
                if (file.Extension == ".TXT")
                {
                    // 取 MFAN 檔 DataTable 資料
                    if (ValidateFileLength(file, 33, "AML MFAN 檔匯入 TXT 檔", JobHelper.strJobID, _dateStart))
                    {
                        dt = GetDatDataTable(file, _dateStart);
                    }
                    else
                    {
                        errorMsg = "檔案資料異常";
                    }
                }
                else
                {
                    errorMsg = "無TXT檔";
                }
            }
            catch (Exception ex)
            {
                JobHelper.Write(JobHelper.strJobID, ex.Message);
                errorMsg = ex.Message;
            }
        }
        JobHelper.Write(JobHelper.strJobID, "讀取要匯入的AML_HQ_MFAN的檔案資料，結束！", LogState.Info);
        return dt;
    }
    /// <summary>
    /// 專案代號:20200031-CSIP EOS AML NOVA 需求擴增
    /// 功能說明:驗證資料長度
    /// 作    者:Ares Stanley
    /// 創建時間:2021/11/25
    /// 修改時間:
    /// </summary>
    /// <param name="file">檔案</param>
    /// <param name="filerightlength">每筆資料長度</param>
    /// <param name="filetypeword">資料訊息</param>
    /// <param name="jobid">JobID</param>
    /// <returns></returns>
    private bool ValidateFileLength(FileInfo file, int filerightlength, string filetypeword, string jobid, string startTime)
    {

        bool isDataOK = true;
        StreamReader streamReader = new StreamReader(file.FullName, System.Text.Encoding.Default);
        int intcouterror = 0;
        string fileRow = "";

        while ((fileRow = streamReader.ReadLine()) != null)
        {

            byte[] bytes = Encoding.Default.GetBytes(fileRow);


            if (bytes.Length != filerightlength)
            {
                intcouterror = intcouterror + 1;
                isDataOK = false;
            }
        }

        if (isDataOK == false)
        {
            JobHelper.WriteError(JobHelper.strJobID, "檔案" + file.FullName + filetypeword + "共有" + intcouterror + "筆，長度不正確");
        }

        streamReader.Dispose();
        streamReader.Close();

        return isDataOK;
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS AML NOVA 需求擴增
    /// 功能說明:取MFAN檔 DataTable
    /// 作    者:Ares Stanley
    /// 創建時間:2021/11/25
    /// 修改時間:
    /// </summary>
    /// <param name="file"></param>
    /// <param name="isDatOK"></param>
    /// <param name="total"></param>
    /// <returns></returns>
    private DataTable GetDatDataTable(FileInfo file, string _dateStart)
    {
        DataTable result = SetDatTableHeader();
        DataRow dr = null;
        string fileRow = "";


        StreamReader streamReader = new StreamReader(file.FullName, System.Text.Encoding.Default);

        while ((fileRow = streamReader.ReadLine()) != null)
        {
            dr = result.NewRow();

            byte[] bytes = Encoding.Default.GetBytes(fileRow);

            dr["MFA_ID"] = NewString(bytes, 0, 9).Trim();
            dr["MFA_AREA"] = NewString(bytes, 9, 4).Trim();
            dr["MFA_NAME"] = NewString(bytes, 13, 20).Trim();
            dr["MOD_DATE"] = _dateStart;
            result.Rows.Add(dr);
        }

        streamReader.Dispose();
        streamReader.Close();

        return result;
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS AML NOVA 需求擴增
    /// 功能說明:byte[] 轉換成 string
    /// 作    者:Ares Stanley
    /// 創建時間:2021/11/25
    /// 修改時間:
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="startPoint"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    private string NewString(byte[] bytes, int startPoint, int length)
    {
        string result = "";

        char[] chars = Encoding.Default.GetChars(bytes, startPoint, length);

        foreach (char chr in chars)
        {
            result = result + chr;
        }

        return result;
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS AML NOVA 需求擴增
    /// 功能說明:設定MFAN DataTable 表頭
    /// 作    者:Ares Stanley
    /// 創建時間:2021/11/25
    /// 修改時間:
    /// </summary>
    /// <returns></returns>
    private DataTable SetDatTableHeader()
    {
        DataTable result = new DataTable();

        result.Columns.Add("MFA_ID", typeof(System.String));               // MFAID
        result.Columns.Add("MFA_AREA", typeof(System.String));            // MFA區域
        result.Columns.Add("MFA_NAME", typeof(System.String));                 // MFA姓名
        result.Columns.Add("MOD_DATE", typeof(System.String));               // 修改日期
        return result;
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS AML NOVA 需求擴增
    /// 功能說明:寫入AML_HQ_MFAN
    /// 作    者:Ares Stanley
    /// 創建時間:2021/11/25
    /// 修改時間:
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    public static bool InsertAML_HQ_MFANImport(DataRow row, string insert_AML_HQ_MFAN)
    {
        bool result = false;
        DateTime now = DateTime.Now;


        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = insert_AML_HQ_MFAN;
        sqlcmd.CommandType = CommandType.Text;

        sqlcmd.Parameters.Add(new SqlParameter("@MFA_ID", row["MFA_ID"].ToString()));
        sqlcmd.Parameters.Add(new SqlParameter("@MFA_AREA", row["MFA_AREA"].ToString()));
        sqlcmd.Parameters.Add(new SqlParameter("@MFA_NAME", row["MFA_NAME"].ToString()));
        sqlcmd.Parameters.Add(new SqlParameter("@MOD_DATE", row["MOD_DATE"].ToString()));

        try
        {
            DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
            if (resultSet != null)
            {
                result = true;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }
        return result;
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS AML NOVA 需求擴增
    /// 功能說明:取得AML_HQ_MFAN資料
    /// 作    者:Ares Stanley
    /// 創建時間:2021/11/25
    /// 修改時間:
    /// </summary>
    /// <returns></returns>
    private DataTable GetAML_HQ_MFAN()
    {
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"Select * From [dbo].[AML_HQ_MFAN]";
        sqlcmd.CommandType = CommandType.Text;
        try
        {
            DataSet result = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
            if(result != null)
            {
                return result.Tables[0];
            }
            else
            {
                return null;
            }
        }
        catch(Exception ex)
        {
            return null;
        }
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS AML NOVA 需求擴增
    /// 功能說明:復原備份資料
    /// 作    者:Ares Stanley
    /// 創建時間:2021/11/25
    /// 修改時間:
    /// </summary>
    /// <param name="backupData"></param>
    private void RecoveryBackupData(DataTable backupData)
    {
        int failCount = 0;
        bool recoveryStatus = true;
        foreach(DataRow row in backupData.Rows)
        {
            recoveryStatus = InsertAML_HQ_MFANImport(row, insert_AML_HQ_MFAN);
            if (!recoveryStatus)
                failCount += 1;
        }
        string msg = string.Format("Recovery AML_HQ_MFAN ，成功 {0} 筆，失敗 {1} 筆，共 {2} 筆！", backupData.Rows.Count - failCount, failCount, backupData.Rows.Count);
        JobHelper.Write(JobHelper.strJobID, msg, LogState.Info);
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS AML NOVA 需求擴增
    /// 功能說明:刪除AML_HQ_MFAN資料
    /// 作    者:Ares Stanley
    /// 創建時間:2021/11/25
    /// 修改時間:    /// </summary>
    /// <param name="del_AML_HQ_MFAN"></param>
    /// <returns></returns>
    private bool Del_AML_HQ_MFAN(string del_AML_HQ_MFAN)
    {
        bool result = false;
        DateTime now = DateTime.Now;


        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = del_AML_HQ_MFAN;
        sqlcmd.CommandType = CommandType.Text;

        try
        {
            result = BRFORM_COLUMN.Delete(sqlcmd);
            return result;
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
            return result;
        }
    }
}