using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Framework.Common.Utility;
using System.IO;
using System.Text;
using Framework.Common.Message;
using Framework.Common.Logging;

/// <summary>
/// AMLInformationService 的摘要描述
/// </summary>
public class AMLInformationService
{
    private string jobID = string.Empty;
    private string csipSystem = "CSIP_System";

    protected JobHelper JobHelper = new JobHelper();

    public AMLInformationService(string jobID)
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

    public string DownloadFromFTP(string date, string localPath, string extension, out string errormsg)
    {
        string fileName = "";
        errormsg = "";

        try
        {
            DataTable tblFileInfo = BRAML_File_Import.GetFileInfo(jobID);

            //20200306-優化，批次重新執行的method
            //如果Parameter有值，則使用Parameter的時間
            if (!string.IsNullOrEmpty(tblFileInfo.Rows[0]["Parameter"].ToString()))
            {
                date = tblFileInfo.Rows[0]["Parameter"].ToString().Trim();
            }

            fileName = tblFileInfo.Rows[0]["FtpFileName"].ToString().Replace("yyyyMMdd", date) + "." + extension;
            string ftpPwd = RedirectHelper.GetDecryptString(tblFileInfo.Rows[0]["FtpPwd"].ToString());

            FTPFactory objFtp = new FTPFactory(tblFileInfo.Rows[0]["FtpIP"].ToString(), "", tblFileInfo.Rows[0]["FtpUserName"].ToString(), ftpPwd, "21", localPath, "Y");

            bool isDownload = objFtp.Download(tblFileInfo.Rows[0]["FtpPath"].ToString(), fileName, localPath, fileName);

            if (isDownload)
            {
                JobHelper.Write(this.jobID, fileName + " FTP 取檔成功", LogState.Info);
            }
            else
            {
                errormsg = "檔案: " + fileName + " 不存在， FTP 取檔失敗";

                JobHelper.Write(this.jobID, "[FAIL] " + errormsg);
                
            }
        }
        catch (Exception ex)
        {
            errormsg = ex.ToString();

            JobHelper.Write(this.jobID, "[FAIL] " + ex.ToString());
        }

        return fileName;
    }

    // 取 E 檔內容
    public DataTable GetEFileData(string filePath, string fileName, out string errorMsg)
    {
        JobHelper.Write(this.jobID, "讀取要匯入的檔案資料！", LogState.Info);

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
                        // 取 E 檔 DataTable 資料
                        dat = GetDatDataTable(file, out isDatOK, out total);
                    }
                    else if (file.Extension == ".ctl")
                    {
                        // 取 E 檔 CTL 資料
                        count = GetCtlData(file, out isCtlOK);
                    }
                }
                catch (Exception ex)
                {
                    JobHelper.Write(this.jobID, ex.Message);
                }
            }
        }

        bool isDuplicate = IsDuplicateData(dat);

        // 判斷檔案錯誤訊息
        errorMsg = GetErrorMsg(isFileExist, isDuplicate, isDatOK, isCtlOK, dat.Rows.Count, count, total);

        JobHelper.Write(this.jobID, "讀取檔案資料結束！", LogState.Info);

        return dat;
    }

    // 寫入資料庫
    public bool SetRelationDataTable(DataTable sourceDat, string fileNameDat)
    {
        bool isInsertEdata = false;

        // 組 AML_IMP_LOG 資料
        DataTable dtAMLIMPLOG = SetAMLIMPLOGData(sourceDat, fileNameDat);

        // 組 AML_Edata_Import 資料
        DataTable dtAMLEdataImport = SetAMLEdataImportData(sourceDat, fileNameDat);

        // 組 AML_CASE_DATA 資料
        DataTable dtAMLCASEDATA = SetAMLCASEDATAData(dtAMLEdataImport);

        int masterID = BRAML_File_Import.InsertAMLIMPLOG(dtAMLIMPLOG);

        JobHelper.Write(this.jobID, "資料庫寫檔開始！ masterID = " + masterID, LogState.Info);

        if (masterID > 0)
        {
            // 寫入 AML_Edata_Import
            isInsertEdata = BRAML_File_Import.InsertAMLEdataImport("AML_Edata_Import", dtAMLEdataImport);

            if (isInsertEdata)
            {
                // 寫入 AML_Edata_Work
                isInsertEdata = BRAML_File_Import.InsertAMLEdataImport("AML_Edata_Work", dtAMLEdataImport);
            }

            if (isInsertEdata)
            {
                // 寫入 AML_CASE_DATA
                isInsertEdata = BRAML_File_Import.InsertAMLCASEDATAData("AML_CASE_DATA", dtAMLCASEDATA);
            }

            // 還原資料
            if (!isInsertEdata)
            {
                BRAML_File_Import.RecoveryEData(fileNameDat);
            }
        }

        JobHelper.Write(this.jobID, "資料庫寫檔結束！", LogState.Info);

        return isInsertEdata;
    }

    private bool IsDuplicateData(DataTable sourceDat)
    {
        DataTable checkData = new DataTable();
        DataRow datRow = null;

        checkData.Columns.Add("RMMBatchNo", typeof(System.String));     // RMM批號
        checkData.Columns.Add("AMLInternalID", typeof(System.String));  // AML內部使用的編號

        foreach (DataRow row in sourceDat.Rows)
        {
            datRow = checkData.NewRow();

            datRow["RMMBatchNo"] = row["RMMBatchNo"].ToString();
            datRow["AMLInternalID"] = row["AMLInternalID"].ToString();

            checkData.Rows.Add(datRow);
        }

        // 寫入 AML_Edata_Temp
        BRAML_File_Import.InsertAMLEdataTemp("AML_Edata_Temp", checkData);

        // 判斷資料內容是否重覆

        string strCaseNo = BRAML_File_Import.IsDuplicateDataCaseNo();

        AMLInformationService aMLInformationService = new AMLInformationService("BatchJob_GetAMLInfo");

        if (strCaseNo != null && !strCaseNo.Equals(""))
        {

            JobHelper.Write(this.jobID, "AML匯入E檔資料重複資料為:" + strCaseNo);
            aMLInformationService.SendMail("AML匯入E檔資料重複", "資料為:" + strCaseNo, "", DateTime.Now);

        }
        bool duplicate = false;

        if (!strCaseNo.Equals(""))
        {
            duplicate = true;
        }

        return duplicate;
    }

    // 取 E 檔 DataTable 資料
    private DataTable GetDatDataTable(FileInfo file, out bool isDatOK, out int total)
    {
        DataTable result = SetDatTableHeader();
        DataRow datRow = null;
        int length = 286;
        string AMLbranch = "0049";
        string fileRow = "";
        string rMMBatchNo = "";
        string errorMsg = "";
        string homeBranch = "";
        string dataDate = "";
        string tempDate = "";
        string maxID = "";


        total = 0;
        isDatOK = true;

        StreamReader streamReader = new StreamReader(file.FullName, System.Text.Encoding.Default);

        while ((fileRow = streamReader.ReadLine()) != null)
        {
            datRow = result.NewRow();
            total = total + 1;

            byte[] bytes = Encoding.Default.GetBytes(fileRow);

            rMMBatchNo = NewString(bytes, 0, 14).Trim();
            homeBranch = NewString(bytes, 280, 4).Trim();
            dataDate = NewString(bytes, 34, 8).Trim();

            errorMsg = "";

            if (homeBranch == AMLbranch)
            {
                if (bytes.Length != length)
                {
                    errorMsg = rMMBatchNo + "長度不正確";
                    isDatOK = false;
                }

                if (tempDate != dataDate)
                {
                    tempDate = dataDate;
                    maxID = BRAML_File_Import.GetCaseNo(dataDate);
                }
                else
                {
                    maxID = (Convert.ToInt64(maxID) + 1).ToString();
                }

                datRow["CASE_NO"] = maxID;
                datRow["RMMBatchNo"] = rMMBatchNo;
                datRow["AMLInternalID"] = NewString(bytes, 14, 20).Trim();
                datRow["DataDate"] = dataDate;
                datRow["CustomerID"] = NewString(bytes, 42, 14).Trim();
                datRow["CustomerEnglishName"] = NewString(bytes, 56, 60).Trim();
                datRow["CustomerChineseName"] = NewString(bytes, 116, 120).Trim();
                datRow["AMLSegment"] = NewString(bytes, 236, 2).Trim();
                datRow["OriginalRiskRanking"] = NewString(bytes, 238, 1).Trim();
                datRow["OriginalNextReviewDate"] = NewString(bytes, 239, 8).Trim();
                datRow["NewRiskRanking"] = NewString(bytes, 247, 1).Trim();
                datRow["NewNextReviewDate"] = NewString(bytes, 248, 8).Trim();
                datRow["LastUpdateMaker"] = NewString(bytes, 256, 12).Trim();
                datRow["LastUpdateBranch"] = NewString(bytes, 268, 4).Trim();
                datRow["LastUpdateDate"] = NewString(bytes, 272, 8).Trim();
                datRow["HomeBranch"] = homeBranch;
                datRow["CaseType"] = NewString(bytes, 284, 2).Trim();
                datRow["FiledSAR"] = "";
                datRow["Filler"] = "";
                datRow["ErrorMsg"] = errorMsg;

                result.Rows.Add(datRow);
            }
        }

        streamReader.Dispose();
        streamReader.Close();

        return result;
    }

    // 取 E 檔 CTL 資料
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
            result = "E檔已存在，如要重匯請刪除AML_IMP_LOG table那筆資料";
        }
        else if (isDuplicate)
        {
            result = "E檔內容有重覆資料";
        }
        else if (!isDatOK)
        {
            result = "E檔長度不正確";
        }
        else if (!isCtlOK)
        {
            result = "ctl檔值有誤";
        }
        else if (datCount == 0)
        {
            //20191125-RQ-2018-015749-002 調整mail內容 by Peggy
            //result = "E檔無處理資料";
            result = "E檔收檔成功，無0049資料";
        }
        else if (total != ctlCount)
        {
            result = "E檔筆數不正確";
        }

        return result;
    }

    // 組 AML_IMP_LOG 資料
    private DataTable SetAMLIMPLOGData(DataTable sourceDat, string fileNameDat)
    {
        DataTable result = SetAMLIMPLOGHeader();
        DataRow datRow = result.NewRow();
        DateTime now = DateTime.Now;

        datRow["FILE_TYPE"] = "E";
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

    // 組 AML_Edata_Import 資料
    private DataTable SetAMLEdataImportData(DataTable sourceDat, string fileNameDat)
    {
        DataTable result = SetAMLEdataImportHeader();
        DataRow datRow = null;
        DateTime now = DateTime.Now;

        foreach (DataRow row in sourceDat.Rows)
        {
            datRow = result.NewRow();

            datRow["CASE_NO"] = row["CASE_NO"].ToString();
            datRow["FileName"] = fileNameDat;
            datRow["RMMBatchNo"] = row["RMMBatchNo"].ToString();
            datRow["AMLInternalID"] = row["AMLInternalID"].ToString();
            datRow["DataDate"] = row["DataDate"].ToString();
            datRow["CustomerID"] = row["CustomerID"].ToString();
            datRow["CustomerEnglishName"] = row["CustomerEnglishName"].ToString().Trim();
            datRow["CustomerChineseName"] = row["CustomerChineseName"].ToString().Trim();
            datRow["AMLSegment"] = row["AMLSegment"].ToString();
            datRow["OriginalRiskRanking"] = row["OriginalRiskRanking"].ToString();
            datRow["OriginalNextReviewDate"] = row["OriginalNextReviewDate"].ToString();
            datRow["NewRiskRanking"] = row["NewRiskRanking"].ToString();
            datRow["NewNextReviewDate"] = row["NewNextReviewDate"].ToString();
            datRow["LastUpdateMaker"] = row["LastUpdateMaker"].ToString();
            datRow["LastUpdateBranch"] = row["LastUpdateBranch"].ToString();
            datRow["LastUpdateDate"] = row["LastUpdateDate"].ToString();
            datRow["HomeBranch"] = row["HomeBranch"].ToString();
            datRow["CaseType"] = row["CaseType"].ToString();
            datRow["FiledSAR"] = row["FiledSAR"].ToString();
            datRow["Filler"] = row["Filler"].ToString();
            datRow["Create_Time"] = now.ToString("HHmmss");
            datRow["Create_User"] = this.csipSystem;
            datRow["Create_Date"] = now.ToString("yyyyMMdd");

            result.Rows.Add(datRow);
        }

        return result;
    }

    // 組 AML_CASE_DATA 資料 有???????
    private DataTable SetAMLCASEDATAData(DataTable amlEdataImport)
    {
        DataTable result = SetAMLCASEDATAHeader();
        DataRow datRow = null;
        DateTime now = DateTime.Now;

        foreach (DataRow row in amlEdataImport.Rows)
        {
            datRow = result.NewRow();

            datRow["CASE_NO"] = row["CASE_NO"].ToString();
            datRow["CUST_ID"] = row["CustomerID"].ToString();
            datRow["CUST_NAME"] = row["CustomerChineseName"].ToString();
            datRow["Source"] = row["HomeBranch"].ToString();
            datRow["RISK_RANKING"] = row["NewRiskRanking"].ToString();
            datRow["CASE_TYPE"] = row["CaseType"].ToString();
            datRow["STATUS"] = "0";
            datRow["Create_YM"] = row["DataDate"].ToString().Substring(0, 6);
            datRow["Create_Date"] = row["DataDate"].ToString();
            datRow["Due_Date"] = row["DataDate"].ToString();           // ???????????????????????????????

            result.Rows.Add(datRow);
        }

        return result;
    }

    // 設定 E 檔 DataTable 表頭
    private DataTable SetDatTableHeader()
    {
        DataTable result = new DataTable();


        result.Columns.Add("CASE_NO", typeof(System.String));                  // CASE_NO
        result.Columns.Add("RMMBatchNo", typeof(System.String));               // RMM批號
        result.Columns.Add("AMLInternalID", typeof(System.String));            // AML內部使用的編號
        result.Columns.Add("DataDate", typeof(System.String));                 // 資料日期
        result.Columns.Add("CustomerID", typeof(System.String));               // 客戶ID
        result.Columns.Add("CustomerEnglishName", typeof(System.String));      // 客戶英文姓名
        result.Columns.Add("CustomerChineseName", typeof(System.String));      // 客戶中文姓名
        result.Columns.Add("AMLSegment", typeof(System.String));               // AML管理區分
        result.Columns.Add("OriginalRiskRanking", typeof(System.String));      // 原本的風險等級
        result.Columns.Add("OriginalNextReviewDate", typeof(System.String));   // 原本的下次審查日期
        result.Columns.Add("NewRiskRanking", typeof(System.String));           // 最新試算後的風險等級
        result.Columns.Add("NewNextReviewDate", typeof(System.String));        // 最新試算後的下次審查日期
        result.Columns.Add("LastUpdateMaker", typeof(System.String));          // 資料最後異動Maker
        result.Columns.Add("LastUpdateBranch", typeof(System.String));         // 資料最後異動分行
        result.Columns.Add("LastUpdateDate", typeof(System.String));           // 資料最後異動日期
        result.Columns.Add("HomeBranch", typeof(System.String));               // 歸屬分行
        result.Columns.Add("CaseType", typeof(System.String));                 // 案件種類
        result.Columns.Add("FiledSAR", typeof(System.String));                 // FiledSAR Flag
        result.Columns.Add("Filler", typeof(System.String));                   // 保留
        result.Columns.Add("ErrorMsg", typeof(System.String));                 // 錯誤訊息

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

    // 設定 AML_Edata_Import 表頭
    private DataTable SetAMLEdataImportHeader()
    {
        DataTable result = new DataTable();

        result.Columns.Add("CASE_NO", typeof(System.String));               // 案件開出來後這個欄位需要回填
        result.Columns.Add("FileName", typeof(System.String));              // 匯入的檔案名稱
        result.Columns.Add("RMMBatchNo", typeof(System.String));            // RMM批號
        result.Columns.Add("AMLInternalID", typeof(System.String));         // AML內部使用的編號
        result.Columns.Add("DataDate", typeof(System.String));              // 資料日期
        result.Columns.Add("CustomerID", typeof(System.String));            // 客戶ID如果是公司就是統編
        result.Columns.Add("CustomerEnglishName", typeof(System.String));   // 客戶英文姓名
        result.Columns.Add("CustomerChineseName", typeof(System.String));   // 客戶的中文名字
        result.Columns.Add("AMLSegment", typeof(System.String));            // AML管理區分
        result.Columns.Add("OriginalRiskRanking", typeof(System.String));   // 原本的風險等級  H/M/L
        result.Columns.Add("OriginalNextReviewDate", typeof(System.String));// 原本的下次審查日期
        result.Columns.Add("NewRiskRanking", typeof(System.String));        // 最新試算後的風險等級
        result.Columns.Add("NewNextReviewDate", typeof(System.String));     // 最新試算後的下次審查日期
        result.Columns.Add("LastUpdateMaker", typeof(System.String));       // 資料最後異動Maker
        result.Columns.Add("LastUpdateBranch", typeof(System.String));      // 資料最後異動分行
        result.Columns.Add("LastUpdateDate", typeof(System.String));        // 資料最後異動日期
        result.Columns.Add("HomeBranch", typeof(System.String));            // 歸屬分行
        result.Columns.Add("CaseType", typeof(System.String));              // 案件種類
        result.Columns.Add("FiledSAR", typeof(System.String));              // FiledSAR Flag
        result.Columns.Add("Filler", typeof(System.String));                // 保留
        result.Columns.Add("Create_Time", typeof(System.String));           // 資料建立時間
        result.Columns.Add("Create_User", typeof(System.String));           // 資料建立人員
        result.Columns.Add("Create_Date", typeof(System.String));           // 資料建立日期

        return result;
    }

    // 設定 AML_CASE_DATA 表頭
    private DataTable SetAMLCASEDATAHeader()
    {
        DataTable result = new DataTable();

        result.Columns.Add("CASE_NO", typeof(System.String));           // 案件編號
        result.Columns.Add("CUST_ID", typeof(System.String));           // AML_Edata_Import.Customer ID
        result.Columns.Add("CUST_NAME", typeof(System.String));         // AML_Edata_Import.CustomerChineseName
        result.Columns.Add("Source", typeof(System.String));            // AML_Edata_Import.HomeBranch
        result.Columns.Add("RISK_RANKING", typeof(System.String));      // AML_Edata_Import.NewRiskRanking
        result.Columns.Add("CASE_TYPE", typeof(System.String));         // AML_Edata_Import.CaseType
        result.Columns.Add("STATUS", typeof(System.String));            // 新增的時候塞0
        result.Columns.Add("Create_YM", typeof(System.String));         // AML_Edata_Import.DATA_DATE之前6碼(YYYYMM)
        result.Columns.Add("Create_Date", typeof(System.String));       // AML_Edata_Import.DATA_DATE
        result.Columns.Add("Due_Date", typeof(System.String));          // AML_Edata_Import.DATA_DATE +審查天數(參數設定，區分高/中風險)，再計算至當月月底例 : 2017/01/01加60天再計算至月底為2017/03/31

        return result;
    }

    // byte[] 轉換成 string
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
}
