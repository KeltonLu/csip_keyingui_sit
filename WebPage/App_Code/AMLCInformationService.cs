using System;
using System.Data;
using Framework.Common.Utility;
using System.IO;
using System.Text;
using Framework.Common.Message;
using Framework.Common.Logging;
using System.Collections.Generic;

/// <summary>
/// AMLCInformationService 的摘要描述
/// 接收AMLC DATA資料 批次Service
/// 檔名：AML_TW_C_yyyyMMdd_UTF8
/// </summary>
public class AMLCInformationService
{
    private string jobID = string.Empty;
    protected JobHelper JobHelper = new JobHelper();
    private string csipSystem = "CSIP_System";
    protected DateTime StartTime = DateTime.Now;// 記錄Job啟動時間

    public AMLCInformationService(string jobID)
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


    private bool ValidateFileLength(FileInfo file, int filerightlength, string filetypeword, string jobid)
    {

        bool isDatOK = true;
        StreamReader streamReader = new StreamReader(file.FullName, System.Text.Encoding.UTF8);
        int intcouterror = 0;
        string fileRow = "";

        while ((fileRow = streamReader.ReadLine()) != null)
        {

            byte[] bytes = Encoding.UTF8.GetBytes(fileRow);


            if (bytes.Length != filerightlength)
            {
                intcouterror = intcouterror + 1;
                isDatOK = false;
            }
        }

        // 20210917 調整Mail發送時機避免一筆錯誤發一封Mail By Ares Stanley
        if (isDatOK == false)
        {
            JobHelper.WriteError(this.jobID, "檔案" + file.FullName + filetypeword + "共有" + intcouterror + "筆，長度不正確");

            AMLCInformationService aMLCInformationService = new AMLCInformationService(jobid);
            aMLCInformationService.SendMail("檔案" + file.FullName + filetypeword + "共有" + intcouterror + "筆，長度不正確", "檔案" + file.FullName + filetypeword + "共有" + intcouterror + "筆，長度不正確", "", this.StartTime);

        }

        streamReader.Dispose();
        streamReader.Close();

        return isDatOK;
    }

    // 取 C 檔 DataTable 資料
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

            string chomebranch = "";

            chomebranch = NewString(bytes, 269, 4).Trim();


            string CustomerID = "";

            CustomerID = NewString(bytes, 8, 14).Trim();

            datRow["FileName"] = file.Name;
            datRow["Datadate"] = NewString(bytes, 0, 8).Trim();
            datRow["CustomerID"] = NewString(bytes, 8, 14).Trim();
            datRow["CustomerEnglishName"] = NewString(bytes, 22, 60).Trim();
            datRow["CustomerChineseName"] = NewString(bytes, 82, 120).Trim();
            datRow["AMLSegment"] = NewString(bytes, 202, 2).Trim();
            datRow["AMLRiskRanking"] = NewString(bytes, 204, 1).Trim();
            datRow["AMLNextReviewDate"] = NewString(bytes, 205, 8).Trim();
            datRow["BlackListHitFlag"] = NewString(bytes, 213, 1).Trim();
            datRow["PEPListHitFlag"] = NewString(bytes, 214, 1).Trim();
            datRow["NNListHitFlag"] = NewString(bytes, 215, 1).Trim();
            datRow["Incorporated"] = NewString(bytes, 216, 1).Trim();
            datRow["IncorporatedDate"] = NewString(bytes, 217, 8).Trim();
            datRow["LastUpdateMaker"] = NewString(bytes, 225, 12).Trim();
            datRow["LastUpdateChecker"] = NewString(bytes, 237, 12).Trim();
            datRow["LastUpdateBranch"] = NewString(bytes, 249, 4).Trim();
            datRow["LastUpdateDate"] = NewString(bytes, 253, 8).Trim();
            datRow["LastUpdateSourceSystem"] = NewString(bytes, 261, 8).Trim();
            datRow["HomeBranch"] = NewString(bytes, 269, 4).Trim();
            datRow["Reason"] = NewString(bytes, 273, 2).Trim();
            datRow["WarningFlag"] = NewString(bytes, 275, 1).Trim();
            datRow["FiledSAR"] = NewString(bytes, 276, 1).Trim();
            datRow["CreditCardBlockCode"] = NewString(bytes, 277, 10).Trim();
            datRow["InternationalOrgPEP"] = NewString(bytes, 287, 1).Trim();
            datRow["DomesticPEP"] = NewString(bytes, 288, 1).Trim();
            datRow["ForeignPEPStakeholder"] = NewString(bytes, 289, 1).Trim();
            datRow["InternationalOrgPEPStakeholder"] = NewString(bytes, 290, 1).Trim();
            datRow["DomesticPEPStakeholder"] = NewString(bytes, 291, 1).Trim();
            datRow["GroupInformationSharingNameListflag"] = NewString(bytes, 292, 1).Trim();
            datRow["Filler"] = NewString(bytes, 293, 37).Trim();
            // 20210527 EOS_AML(NOVA) by Ares Dennis
            #region 退版機制
            DataTable dt = new DataTable();
            CSIPCommonModel.BusinessRules_new.BRM_PROPERTY_CODE.GetEnableProperty("01", new string[] { "OLD_VERSION_FLAG" }, ref dt);
            string flag = "";
            if (dt.Rows.Count > 0)
            {
                flag = dt.Rows[0]["PROPERTY_CODE"].ToString();
            }
            #endregion            
            if (flag == "N")// 新版程式碼
            {
                datRow["Dormant_Flag"] = NewString(bytes, 293, 1).Trim();
                datRow["Dormant_Date"] = NewString(bytes, 294, 8).Trim();
                datRow["Incorporated_Source_System"] = NewString(bytes, 302, 8).Trim();
                datRow["AML_Last_Review_Date"] = NewString(bytes, 310, 8).Trim();
                datRow["Risk_Factor_PEP"] = NewString(bytes, 318, 1).Trim();
                datRow["Risk_Factor_RP_PEP"] = NewString(bytes, 319, 1).Trim();
                datRow["Internal_List_Flag"] = NewString(bytes, 320, 1).Trim();
                datRow["High_Risk_Flag_Because_Rpty"] = NewString(bytes, 321, 1).Trim();
                datRow["High_Risk_Flag"] = NewString(bytes, 322, 1).Trim();
                datRow["Filler"] = NewString(bytes, 323, 7).Trim();

                //20210527 EOS_AML(NOVA) 增收自然人ID by Ares Dennis                
                result.Rows.Add(datRow);                
            }
            else
            {                
                //20210527 EOS_AML(NOVA)  增收自然人ID by Ares Dennis                
                result.Rows.Add(datRow);                
            }
        }

        streamReader.Dispose();
        streamReader.Close();

        return result;
    }

    // 設定 C 檔 DataTable 表頭
    private DataTable SetDatTableHeader()
    {
        DataTable result = new DataTable();

        result.Columns.Add("FileName", typeof(System.String));               // RMM批號
        result.Columns.Add("DataDate", typeof(System.String));               // RMM批號
        result.Columns.Add("CustomerID", typeof(System.String));            // AML內部使用的編號
        result.Columns.Add("CustomerEnglishName", typeof(System.String));                 // 資料日期
        result.Columns.Add("CustomerChineseName", typeof(System.String));               // 客戶ID
        result.Columns.Add("AMLSegment", typeof(System.String));      // 客戶英文姓名
        result.Columns.Add("AMLRiskRanking", typeof(System.String));      // 客戶中文姓名
        result.Columns.Add("AMLNextReviewDate", typeof(System.String));               // AML管理區分
        result.Columns.Add("BlackListHitFlag", typeof(System.String));      // 原本的風險等級
        result.Columns.Add("PEPListHitFlag", typeof(System.String));   // 原本的下次審查日期
        result.Columns.Add("NNListHitFlag", typeof(System.String));           // 最新試算後的風險等級
        result.Columns.Add("Incorporated", typeof(System.String));        // 最新試算後的下次審查日期
        result.Columns.Add("IncorporatedDate", typeof(System.String));          // 資料最後異動Maker
        result.Columns.Add("LastUpdateMaker", typeof(System.String));         // 資料最後異動分行
        result.Columns.Add("LastUpdateChecker", typeof(System.String));           // 資料最後異動日期
        result.Columns.Add("LastUpdateBranch", typeof(System.String));               // 歸屬分行
        result.Columns.Add("LastUpdateDate", typeof(System.String));                 // 案件種類
        result.Columns.Add("LastUpdateSourceSystem", typeof(System.String));                 // FiledSAR Flag
        result.Columns.Add("HomeBranch", typeof(System.String));                   // 保留
        result.Columns.Add("Reason", typeof(System.String));   // 原本的下次審查日期
        result.Columns.Add("WarningFlag", typeof(System.String));           // 最新試算後的風險等級
        result.Columns.Add("FiledSAR", typeof(System.String));        // 最新試算後的下次審查日期
        result.Columns.Add("CreditCardBlockCode", typeof(System.String));          // 資料最後異動Maker
        result.Columns.Add("InternationalOrgPEP", typeof(System.String));         // 資料最後異動分行
        result.Columns.Add("DomesticPEP", typeof(System.String));           // 資料最後異動日期
        result.Columns.Add("ForeignPEPStakeholder", typeof(System.String));               // 歸屬分行
        result.Columns.Add("InternationalOrgPEPStakeholder", typeof(System.String));                 // 案件種類
        result.Columns.Add("DomesticPEPStakeholder", typeof(System.String));                 // FiledSAR Flag
        result.Columns.Add("GroupInformationSharingNameListflag", typeof(System.String));                   // 保留
        result.Columns.Add("Filler", typeof(System.String));                   // 保留
        // 20210527 EOS_AML(NOVA) by Ares Dennis
        #region 退版機制
        DataTable dt = new DataTable();
        CSIPCommonModel.BusinessRules_new.BRM_PROPERTY_CODE.GetEnableProperty("01", new string[] { "OLD_VERSION_FLAG" }, ref dt);
        string flag = "";
        if (dt.Rows.Count > 0)
        {
            flag = dt.Rows[0]["PROPERTY_CODE"].ToString();
        }
        #endregion
        if(flag == "N")// 新版程式碼
        {
            result.Columns.Add("Dormant_Flag", typeof(System.String));
            result.Columns.Add("Dormant_Date", typeof(System.String));
            result.Columns.Add("Incorporated_Source_System", typeof(System.String));
            result.Columns.Add("AML_Last_Review_Date", typeof(System.String));
            result.Columns.Add("Risk_Factor_PEP", typeof(System.String));
            result.Columns.Add("Risk_Factor_RP_PEP", typeof(System.String));
            result.Columns.Add("Internal_List_Flag", typeof(System.String));
            result.Columns.Add("High_Risk_Flag_Because_Rpty", typeof(System.String));
            result.Columns.Add("High_Risk_Flag", typeof(System.String));
        }
             
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
    public DataTable SetAMLIMPLOGData(DataTable sourceDat, string fileNameDat)
    {
        DataTable result = SetAMLIMPLOGHeader();
        DataRow datRow = result.NewRow();
        DateTime now = DateTime.Now;

        datRow["FILE_TYPE"] = "C";
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
    private DataTable SetAMLCdataImportData(DataTable sourceDat, string fileNameDat)
    {
        DataTable result = SetAMLCdataImportHeader();
        DataRow datRow = null;
        DateTime now = DateTime.Now;

        foreach (DataRow row in sourceDat.Rows)
        {
            datRow = result.NewRow();


            datRow["FileName"] = row["FileName"].ToString();
            datRow["Datadate"] = row["Datadate"].ToString();
            datRow["CustomerID"] = row["CustomerID"].ToString();
            datRow["CustomerEnglishName"] = row["CustomerEnglishName"].ToString();
            datRow["CustomerChineseName"] = row["CustomerChineseName"].ToString();
            datRow["AMLSegment"] = row["AMLSegment"].ToString();
            datRow["AMLRiskRanking"] = row["AMLRiskRanking"].ToString();
            datRow["AMLNextReviewDate"] = row["AMLNextReviewDate"].ToString();
            datRow["BlackListHitFlag"] = row["BlackListHitFlag"].ToString();
            datRow["PEPListHitFlag"] = row["PEPListHitFlag"].ToString();
            datRow["NNListHitFlag"] = row["NNListHitFlag"].ToString();
            datRow["Incorporated"] = row["Incorporated"].ToString();
            datRow["IncorporatedDate"] = row["IncorporatedDate"].ToString();
            datRow["LastUpdateMaker"] = row["LastUpdateMaker"].ToString();
            datRow["LastUpdateChecker"] = row["LastUpdateChecker"].ToString();
            datRow["LastUpdateBranch"] = row["LastUpdateBranch"].ToString();
            datRow["LastUpdateDate"] = row["LastUpdateDate"].ToString();
            datRow["LastUpdateSourceSystem"] = row["LastUpdateSourceSystem"].ToString();
            datRow["HomeBranch"] = row["HomeBranch"].ToString();
            datRow["Reason"] = row["Reason"].ToString();
            datRow["WarningFlag"] = row["WarningFlag"].ToString();
            datRow["FiledSAR"] = row["FiledSAR"].ToString();
            datRow["CreditCardBlockCode"] = row["CreditCardBlockCode"].ToString();
            datRow["InternationalOrgPEP"] = row["InternationalOrgPEP"].ToString();
            datRow["DomesticPEP"] = row["DomesticPEP"].ToString();
            datRow["ForeignPEPStakeholder"] = row["ForeignPEPStakeholder"].ToString();
            datRow["InternationalOrgPEPStakeholder"] = row["InternationalOrgPEPStakeholder"].ToString();
            datRow["DomesticPEPStakeholder"] = row["DomesticPEPStakeholder"].ToString();
            datRow["GroupInformationSharingNameListflag"] = row["GroupInformationSharingNameListflag"].ToString();
            datRow["Filler"] = row["Filler"].ToString();
            datRow["Create_Time"] = now.ToString("HHmmss");
            datRow["Create_User"] = this.csipSystem;
            datRow["Create_Date"] = now.ToString("yyyyMMdd");
            // 20210527 EOS_AML(NOVA) by Ares Dennis
            #region 退版機制
            DataTable dt = new DataTable();
            CSIPCommonModel.BusinessRules_new.BRM_PROPERTY_CODE.GetEnableProperty("01", new string[] { "OLD_VERSION_FLAG" }, ref dt);
            string flag = "";
            if (dt.Rows.Count > 0)
            {
                flag = dt.Rows[0]["PROPERTY_CODE"].ToString();
            }
            #endregion
            if (flag == "N")// 新版程式碼
            {
                datRow["Dormant_Flag"] = row["Dormant_Flag"].ToString();
                datRow["Dormant_Date"] = row["Dormant_Date"].ToString();
                datRow["Incorporated_Source_System"] = row["Incorporated_Source_System"].ToString();
                datRow["AML_Last_Review_Date"] = row["AML_Last_Review_Date"].ToString();
                datRow["Risk_Factor_PEP"] = row["Risk_Factor_PEP"].ToString();
                datRow["Risk_Factor_RP_PEP"] = row["Risk_Factor_RP_PEP"].ToString();
                datRow["Internal_List_Flag"] = row["Internal_List_Flag"].ToString();
                datRow["High_Risk_Flag_Because_Rpty"] = row["High_Risk_Flag_Because_Rpty"].ToString();
                datRow["High_Risk_Flag"] = row["High_Risk_Flag"].ToString();
            }
            

            result.Rows.Add(datRow);
        }

        return result;
    }


    // 取 C 檔內容
    public DataTable GetCFileData(string filePath, string fileName, out string errorMsg)
    {
        JobHelper.Write(this.jobID, "讀取要匯入的C檔案資料！", LogState.Info);

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
                        // 取 C 檔 DataTable 資料
                        if (ValidateFileLength(file, 330, "AML C檔匯入dat檔", this.jobID))
                        {
                            dat = GetDatDataTable(file, out isDatOK, out total);
                        }
                        else
                        {
                            isDatOK = false;
                        }
                    }
                    else if (file.Extension == ".ctl")
                    {
                        // 取 C 檔 CTL 資料
                        if (ValidateFileLength(file, 10, "AML C檔匯入ctl檔", this.jobID))
                        {
                            count = GetCtlData(file, out isCtlOK);
                        }
                        else
                        {
                            isDatOK = false;
                        }
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
        errorMsg = GetErrorMsg(isFileExist, isDuplicate, isDatOK, isCtlOK, count, count, total);

        JobHelper.Write(this.jobID, "讀取檔案資料結束！", LogState.Info);

        return dat;
    }



    // 取 C 檔 CTL 資料
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
            result = "C檔已存在，如要重匯請刪除AML_IMP_LOG table那筆資料";
        }
        else if (isDuplicate)
        {
            result = "C檔內容有重覆資料";
        }
        else if (!isDatOK)
        {
            result = "C檔長度不正確";
        }
        else if (!isCtlOK)
        {
            result = "ctl檔值有誤";
        }
        else if (datCount == 0)
        {
            result = "C檔無處理資料";
        }
        else if (datCount != ctlCount)
        {
            result = "C檔筆數不正確";
        }

        return result;
    }



    // 寫入資料庫
    public bool SetRelationDataTable(DataTable sourceDat, string fileNameDat)
    {
        bool isInsertCdata = false;

        // 組 AML_IMP_LOG 資料
        DataTable dtAMLIMPLOG = SetAMLIMPLOGData(sourceDat, fileNameDat);

        // 組 AML_Cdata_Import 資料
        DataTable dtAMLCdataImport = SetAMLCdataImportData(sourceDat, fileNameDat);

        int masterID = BRAML_File_Import.InsertAMLIMPLOG(dtAMLIMPLOG);

        JobHelper.Write(this.jobID, "資料庫寫檔開始！ masterID = " + masterID, LogState.Info);



        if (masterID > 0)
        {
            // 寫入 AML_Edata_Import
            isInsertCdata = BRAML_File_Import.InsertAMLCdataImport("AML_Cdata_Import", dtAMLCdataImport);


            if (isInsertCdata)
            {
                // 寫入 AML_Edata_Work
                isInsertCdata = BRAML_File_Import.InsertAMLCdataImport("AML_Cdata_Work", dtAMLCdataImport);
            }

            // 還原資料
            if (!isInsertCdata)
            {
                BRAML_File_Import.RecoveryCData(fileNameDat);
            }
        }

        JobHelper.Write(this.jobID, "資料庫寫檔結束！", LogState.Info);

        return isInsertCdata;
    }

    // 設定 AML_Cdata_Import 表頭
    private DataTable SetAMLCdataImportHeader()
    {
        //todo
        DataTable result = new DataTable();

        result.Columns.Add("FileName", typeof(System.String));
        result.Columns.Add("Datadate", typeof(System.String));
        result.Columns.Add("CustomerID", typeof(System.String));
        result.Columns.Add("CustomerEnglishName", typeof(System.String));
        result.Columns.Add("CustomerChineseName", typeof(System.String));
        result.Columns.Add("AMLSegment", typeof(System.String));
        result.Columns.Add("AMLRiskRanking", typeof(System.String));
        result.Columns.Add("AMLNextReviewDate", typeof(System.String));
        result.Columns.Add("BlackListHitFlag", typeof(System.String));
        result.Columns.Add("PEPListHitFlag", typeof(System.String));
        result.Columns.Add("NNListHitFlag", typeof(System.String));
        result.Columns.Add("Incorporated", typeof(System.String));
        result.Columns.Add("IncorporatedDate", typeof(System.String));
        result.Columns.Add("LastUpdateMaker", typeof(System.String));
        result.Columns.Add("LastUpdateChecker", typeof(System.String));       
        result.Columns.Add("LastUpdateBranch", typeof(System.String));     
        result.Columns.Add("LastUpdateDate", typeof(System.String));
        result.Columns.Add("LastUpdateSourceSystem", typeof(System.String));
        result.Columns.Add("HomeBranch", typeof(System.String));
        result.Columns.Add("Reason", typeof(System.String));
        result.Columns.Add("WarningFlag", typeof(System.String));
        result.Columns.Add("FiledSAR", typeof(System.String));
        result.Columns.Add("CreditCardBlockCode", typeof(System.String));
        result.Columns.Add("InternationalOrgPEP", typeof(System.String));
        result.Columns.Add("DomesticPEP", typeof(System.String));
        result.Columns.Add("ForeignPEPStakeholder", typeof(System.String));
        result.Columns.Add("InternationalOrgPEPStakeholder", typeof(System.String));
        result.Columns.Add("DomesticPEPStakeholder", typeof(System.String));
        result.Columns.Add("GroupInformationSharingNameListflag", typeof(System.String));
        result.Columns.Add("Filler", typeof(System.String));
        result.Columns.Add("Create_Time", typeof(System.String));
        result.Columns.Add("Create_User", typeof(System.String));
        result.Columns.Add("Create_Date", typeof(System.String));
        // 20210527 EOS_AML(NOVA) by Ares Dennis
        #region 退版機制
        DataTable dt = new DataTable();
        CSIPCommonModel.BusinessRules_new.BRM_PROPERTY_CODE.GetEnableProperty("01", new string[] { "OLD_VERSION_FLAG" }, ref dt);
        string flag = "";
        if (dt.Rows.Count > 0)
        {
            flag = dt.Rows[0]["PROPERTY_CODE"].ToString();
        }
        #endregion
        if (flag == "N")// 新版程式碼
        {
            result.Columns.Add("Dormant_Flag", typeof(System.String));
            result.Columns.Add("Dormant_Date", typeof(System.String));
            result.Columns.Add("Incorporated_Source_System", typeof(System.String));
            result.Columns.Add("AML_Last_Review_Date", typeof(System.String));
            result.Columns.Add("Risk_Factor_PEP", typeof(System.String));
            result.Columns.Add("Risk_Factor_RP_PEP", typeof(System.String));
            result.Columns.Add("Internal_List_Flag", typeof(System.String));
            result.Columns.Add("High_Risk_Flag_Because_Rpty", typeof(System.String));
            result.Columns.Add("High_Risk_Flag", typeof(System.String));
        }        

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
