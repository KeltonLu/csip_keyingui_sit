using System;
using System.Data;
using Framework.Common.Utility;
using System.IO;
using System.Text;
using Framework.Common.Message;
using Framework.Common.Logging;

/// <summary>
/// AMLCaseRtnService 的摘要描述
/// 收取結案回檔  批次Service
/// 檔名：AML_UPD_CLO_R_yyyyMMdd
/// </summary>
public class AMLCaseRtnService
{
    private string jobID = string.Empty;
    protected JobHelper JobHelper = new JobHelper();
    private string csipSystem = "CSIP_System";

    public AMLCaseRtnService(string jobID)
    {
        this.jobID = jobID;
        JobHelper.strJobID = jobID;
    }

    // 發送 mail
    public bool SendMail(string mailTitle, string mailBody, string status, DateTime startTime)
    {
        string emailMembers = new Com_AutoJob().GetEmailMembers("01", this.jobID);
        emailMembers = emailMembers.Substring(0, emailMembers.Length - 1);

        string[] mailTos = emailMembers.Split(';');

        System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();

        nvc["Name"] = emailMembers.Replace(';', ',');

        nvc["Title"] = mailTitle;

        nvc["StartTime"] = startTime.ToString();

        nvc["EndTime"] = DateTime.Now.ToString();

        nvc["Message"] = mailBody.ToString().Trim();

        nvc["Status"] = status;

        try
        {
            MailService.MailSender(mailTos, 1, nvc, "");
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
        }
        return true;
    }


    public string DownloadFromFTP(string date, string localPath, string extension, ref bool isDownload)//20191230-RQ-2019-030155-002-批次信函調整：增加取得取檔結果的參數 by Peggy)
    {
        string fileName = "";

        try
        {
            DataTable tblFileInfo = BRAML_File_Import.GetFileInfo(jobID);

            //20191230-RQ-2019-030155-002-批次信函調整：增加判斷如果Parameter有值，則使用Parameter的時間  by Peggy
            //如果Parameter有值，則使用Parameter的時間
            if (!string.IsNullOrEmpty(tblFileInfo.Rows[0]["Parameter"].ToString()))
            {
                date = tblFileInfo.Rows[0]["Parameter"].ToString().Trim();
            }

            fileName = tblFileInfo.Rows[0]["FtpFileName"].ToString().Replace("yyyyMMdd", date) + "." + extension;
            string ftpPwd = RedirectHelper.GetDecryptString(tblFileInfo.Rows[0]["FtpPwd"].ToString());

            FTPFactory objFtp = new FTPFactory(tblFileInfo.Rows[0]["FtpIP"].ToString(), "", tblFileInfo.Rows[0]["FtpUserName"].ToString(), ftpPwd, "21", localPath, "Y");

            //20191230-RQ-2019-030155-002-批次信函調整：增加取得取檔結果的參數 by Peggy
            //bool isDownload = objFtp.Download(tblFileInfo.Rows[0]["FtpPath"].ToString(), fileName, localPath, fileName);
            isDownload = objFtp.Download(tblFileInfo.Rows[0]["FtpPath"].ToString(), fileName, localPath, fileName);

            if (isDownload)
            {
                JobHelper.Write(this.jobID, fileName + " FTP 取檔成功", LogState.Info);
            }
            else
            {
                JobHelper.Write(this.jobID, "[FAIL] 檔案: " + fileName + " 不存在， FTP 取檔失敗");
            }
        }
        catch (Exception ex)
        {
            JobHelper.Write(this.jobID, "[FAIL] " + ex.ToString());
        }

        return fileName;
    }


    // 取 主機回應ok 檔資料
    private DataTable GetDatDataTable(FileInfo file, out bool isDatOK, out int total)
    {
        DataTable result = SetDatTableHeader();
        DataRow datRow = null;
        string fileRow = "";

        total = 0;
        isDatOK = true;

        StreamReader streamReader = new StreamReader(file.FullName, System.Text.Encoding.UTF8);

        while ((fileRow = streamReader.ReadLine()) != null)
        {
            datRow = result.NewRow();

            byte[] bytes = Encoding.UTF8.GetBytes(fileRow);

            datRow["RMMBatchNo"] = NewString(bytes, 0, 14).Trim();
            datRow["AMLInternalID"] = NewString(bytes, 14, 20).Trim();
            datRow["SourceSystem"] = NewString(bytes, 34, 8).Trim();
            datRow["DataDate"] = NewString(bytes, 42, 8).Trim();
            datRow["CustomerID"] = NewString(bytes, 50, 14).Trim();
            datRow["RMMFinish"] = NewString(bytes, 64, 1).Trim();
            datRow["LastUpdateMaker"] = NewString(bytes, 65, 12).Trim();
            datRow["LastUpdateChecker"] = NewString(bytes, 77, 12).Trim();
            datRow["LastUpdateBranch"] = NewString(bytes, 89, 4).Trim();
            datRow["LastUpdateDate"] = NewString(bytes, 93, 8).Trim();
            datRow["retuncode"] = NewString(bytes, 101, 2).Trim();

            result.Rows.Add(datRow);
        }

        streamReader.Dispose();
        streamReader.Close();

        return result;
    }

    // 設定 Branch 檔 DataTable 表頭
    private DataTable SetDatTableHeader()
    {
        DataTable result = new DataTable();

        result.Columns.Add("RMMBatchNo", typeof(System.String));          // RMM批號
        result.Columns.Add("AMLInternalID", typeof(System.String));       // AML內部使用的編號
        result.Columns.Add("SourceSystem", typeof(System.String));        // 前端系統
        result.Columns.Add("DataDate", typeof(System.String));            // 資料日期
        result.Columns.Add("CustomerID", typeof(System.String));          // 客戶ID
        result.Columns.Add("RMMFinish", typeof(System.String));           // RMM完成
        result.Columns.Add("LastUpdateMaker", typeof(System.String));     // 資料最後異動Maker
        result.Columns.Add("LastUpdateChecker", typeof(System.String));   // 資料最後異動Checker
        result.Columns.Add("LastUpdateBranch", typeof(System.String));    // 資料最後異動分行
        result.Columns.Add("LastUpdateDate", typeof(System.String));      // 資料最後異動日期
        result.Columns.Add("retuncode", typeof(System.String));           // 回覆碼

        return result;
    }

    // byte[] 轉換成 string
    private string NewString(byte[] bytes, int startPoint, int length)
    {
        string result = "";

        char[] chars = Encoding.UTF8.GetChars(bytes, startPoint, length);

        foreach (char chr in chars)
        {
            result = result + chr;
        }

        return result;
    }


    // 組 AML_IMP_LOG 資料
    private DataTable SetAMLIMPLOGData(DataTable sourceDat, string fileNameDat)
    {
        DataTable result = SetAMLIMPLOGHeader();
        DataRow datRow = result.NewRow();
        DateTime now = DateTime.Now;

        datRow["FILE_TYPE"] = "AMLCaseRtn";
        datRow["FileName"] = fileNameDat;
        datRow["IMP_DATE"] = now.ToString("yyyyMMdd");
        datRow["IMP_TIME"] = now.ToString("HHmmss");
        datRow["Create_User"] = this.csipSystem;
        datRow["Status"] = "Y";
        datRow["IMP_OK_COUNT"] = sourceDat.Rows.Count;
        datRow["IMP_FAIL_COUNT"] = 0;

        result.Rows.Add(datRow);

        return result;
    }

    // 組 AML_Cdata_Import 資料
    private DataTable SetAMLBranchdataImportData(DataTable sourceDat, string fileNameDat)
    {
        DataTable result = SetAMLBranchdataImportHeader();
        DataRow datRow = null;
        DateTime now = DateTime.Now;

        foreach (DataRow row in sourceDat.Rows)
        {
            datRow = result.NewRow();


            datRow["RMMBatchNo"] = row["CASE_NO"].ToString();
            datRow["AMLInternalID"] = row["FileName"].ToString();
            datRow["SourceSystem"] = row["BRCH_BATCH_NO"].ToString();
            datRow["DataDate"] = row["BRCH_INTER_ID"].ToString();
            datRow["CustomerID"] = row["BRCH_SIXM_TOT_AMT"].ToString();
            datRow["RMMFinish"] = row["BRCH_KEY"].ToString();
            datRow["LastUpdateMaker"] = row["BRCH_BRCH_NO"].ToString();
            datRow["LastUpdateChecker"] = row["BRCH_BRCH_SEQ"].ToString();
            datRow["LastUpdateBranch"] = row["BRCH_BRCH_TYPE"].ToString();
            datRow["LastUpdateDate"] = row["BRCH_NATION"].ToString();
            datRow["retuncode"] = row["BRCH_BIRTH_DATE"].ToString();



            result.Rows.Add(datRow);
        }

        return result;
    }


    // 取 Branch 檔內容
    public DataTable GetBranchFileData(string filePath, string fileName, out string errorMsg)
    {
        JobHelper.Write(this.jobID, "讀取要匯入的Branch檔案資料！", LogState.Info);

        DirectoryInfo di = new DirectoryInfo(filePath);
        DataTable dat = new DataTable();

        bool isDatOK = true;
        bool isCtlOK = true;
        int count = 0;
        int total = 0;
        errorMsg = "";

        // 判斷檔案是否已存在
        bool isFileExist = BRAML_File_Import.IsEFileExist(fileName);

        if (!isFileExist)
        {
            foreach (FileInfo file in di.GetFiles())
            {
                try
                {
                    if (file.Extension == ".dat")
                    {
                        // 取 Branch 檔 DataTable 資料
                        dat = GetDatDataTable(file, out isDatOK, out total);
                    }
                    else if (file.Extension == ".ctl")
                    {
                        // 取 Branch 檔 CTL 資料
                        count = GetCtlData(file, out isCtlOK);
                    }
                }
                catch (Exception ex)
                {
                    JobHelper.Write(this.jobID, ex.Message);
                }
            }
        }

        //TODO
        //bool isDuplicate = IsDuplicateData(dat);

        bool isDuplicate = false;
        // 判斷檔案錯誤訊息
        errorMsg = GetErrorMsg(isFileExist, isDuplicate, isDatOK, isCtlOK, dat.Rows.Count, count, total);

        JobHelper.Write(this.jobID, "讀取檔案資料結束！", LogState.Info);

        return dat;
    }



    // 取 Branch 檔 CTL 資料
    private int GetCtlData(FileInfo file, out bool isCtlOK)
    {
        int result = 0;
        string fileRow = "";
        isCtlOK = true;

        StreamReader streamReader = new StreamReader(file.FullName, System.Text.Encoding.Default);

        try
        {
            while ((fileRow = streamReader.ReadLine()) != null)
            {
                result = Convert.ToInt32(fileRow);
            }
        }
        catch (Exception ex)
        {
            isCtlOK = false;

            JobHelper.Write(this.jobID, ex.Message);
        }

        streamReader.Dispose();
        streamReader.Close();

        return result;
    }


    // 判斷檔案錯誤訊息
    private string GetErrorMsg(bool isFileExist, bool isDuplicate, bool isDatOK, bool isCtlOK, int datCount, int ctlCount, int total)
    {
        string result = "";

        if (isFileExist)
        {
            result = "主機結案回復ok檔已存在，如要重跑請刪除AML_IMP_LOG資料表資料";
        }
        else if (isDuplicate)
        {
            result = "主機結案回復ok檔內容有重覆資料";
        }
        else if (!isDatOK)
        {
            result = "主機結案回復ok檔長度不正確";
        }
        else if (!isCtlOK)
        {
            result = "主機結案回復ok檔，ctl檔值有誤";
        }
        else if (datCount == 0)
        {
            result = "主機結案回復ok檔無處理資料";
        }
        else if (datCount != ctlCount)
        {
            result = "主機結案回復ok檔筆數不正確";
        }

        return result;
    }



    // 寫入資料庫
    public bool SetRelationDataTable(DataTable sourceDat, string fileNameDat)
    {
        bool isInsertCdata = false;

        // 組 AML_IMP_LOG 資料
        DataTable dtAMLIMPLOG = SetAMLIMPLOGData(sourceDat, fileNameDat);


        int masterID = BRAML_File_Import.InsertAMLIMPLOG(dtAMLIMPLOG);

        JobHelper.Write(this.jobID, "資料庫寫檔開始！ masterID = " + masterID, LogState.Info);
        JobHelper.Write(this.jobID, "資料庫寫檔結束！", LogState.Info);

        if (masterID > -1)
        {
            isInsertCdata = true;
        }

        return isInsertCdata;
    }


    // 設定 AML_Branchdata_Import 表頭
    private DataTable SetAMLBranchdataImportHeader()
    {
        //todo
        DataTable result = new DataTable();



        result.Columns.Add("RMMBatchNo", typeof(System.String));
        result.Columns.Add("AMLInternalID", typeof(System.String));
        result.Columns.Add("SourceSystem", typeof(System.String));
        result.Columns.Add("DataDate", typeof(System.String));
        result.Columns.Add("CustomerID", typeof(System.String));
        result.Columns.Add("RMMFinish", typeof(System.String));
        result.Columns.Add("LastUpdateMaker", typeof(System.String));
        result.Columns.Add("LastUpdateChecker", typeof(System.String));
        result.Columns.Add("LastUpdateBranch", typeof(System.String));
        result.Columns.Add("LastUpdateDate", typeof(System.String));



        return result;
    }

    // 設定 AML_IMP_LOG 表頭
    private DataTable SetAMLIMPLOGHeader()
    {
        DataTable result = new DataTable();

        result.Columns.Add("FILE_TYPE", typeof(System.String));     // 接收到檔案的檔案類型
        result.Columns.Add("FileName", typeof(System.String));      // 接受到的檔案名稱
        result.Columns.Add("IMP_DATE", typeof(System.String));      // 匯入日期YYYYMMDD
        result.Columns.Add("IMP_TIME", typeof(System.String));      // 匯入時間HHMMSS
        result.Columns.Add("Create_User", typeof(System.String));   // 建立資料者
        result.Columns.Add("Count", typeof(System.Int32));          // 資料筆數
        result.Columns.Add("Status", typeof(System.String));        // 是否檢核成功 Y:成功 N:失敗
        result.Columns.Add("IMP_OK_COUNT", typeof(System.Int32));   // 成功匯入的有幾筆
        result.Columns.Add("IMP_FAIL_COUNT", typeof(System.Int32)); // 失敗匯入的有幾筆

        return result;
    }
}
