using System;
using System.Data;
using Framework.Common.Utility;
using System.IO;
using Framework.Common.Message;
using Framework.Common.Logging;
using System.Text;

/// <summary>
/// MainFrameMFAFService 的摘要描述
/// 取得主機MFA資訊
/// </summary>
public class MainFrameMFAFService
{
    private string JobID = string.Empty;
    protected JobHelper JobHelper = new JobHelper();

    public MainFrameMFAFService(string jobID)
    {
        this.JobID = jobID;
        JobHelper.strJobID = jobID;
    }
    
    // 發送 mail
    public bool SendMail(string mailTitle, string mailBody, string status, DateTime startTime)
    {
        string emailMembers = new Com_AutoJob().GetEmailMembers("01", this.JobID);
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


    public string DownloadFromFTP(string date, string localPath, string extension, ref string DownLoadMsg, ref bool isDownload)
    {
        string fileName = "";

        try
        {
            DataTable tblFileInfo = BRAML_File_Import.GetFileInfo(JobID);

            //如果Parameter有值，則使用Parameter的時間
            if (!string.IsNullOrEmpty(tblFileInfo.Rows[0]["Parameter"].ToString()))
            {
                date = tblFileInfo.Rows[0]["Parameter"].ToString().Trim();
            }

            //MFAF_yyyyMMdd
            fileName = tblFileInfo.Rows[0]["FtpFileName"].ToString().Replace("yyyyMMdd", date) + "." + extension.Trim().ToUpper();

            string ftpPwd = RedirectHelper.GetDecryptString(tblFileInfo.Rows[0]["FtpPwd"].ToString());

            FTPFactory objFtp = new FTPFactory(tblFileInfo.Rows[0]["FtpIP"].ToString(), "", tblFileInfo.Rows[0]["FtpUserName"].ToString(), ftpPwd, "21", localPath, "Y");
            
            isDownload = objFtp.Download(tblFileInfo.Rows[0]["FtpPath"].ToString(), fileName, localPath, fileName);

            if (isDownload)
            {
                DownLoadMsg = "";

                JobHelper.Write(this.JobID, fileName + " FTP 取檔成功", LogState.Info);
            }
            else
            {
                DownLoadMsg = "【JOB：" + this.JobID + "】[FAIL] 檔案：" + fileName + " 不存在， FTP 取檔失敗";
                JobHelper.Write(this.JobID, "[FAIL] 檔案: " + fileName + " 不存在， FTP 取檔失敗");
            }
        }
        catch (Exception ex)
        {
            JobHelper.Write(this.JobID, "[FAIL] " + ex.ToString());
        }

        return fileName;
    }


    // 取 DataTable 資料
    private DataTable GetDatDataTable(FileInfo file, ref string ErrMsg, out int total)
    {
        DataTable result = SetDatTableHeader(); 
        
        DataRow datRow = null;
        string fileRow = "";
        total = 0;
        
        StreamReader streamReader = new StreamReader(file.FullName, Encoding.Default);
        while ((fileRow = streamReader.ReadLine()) != null)
        {
            total++;
            datRow = result.NewRow();
            byte[] bytes = Encoding.GetEncoding("Big5").GetBytes(fileRow);

            datRow["CORP_NO"] = NewString(bytes, 0, 8).Trim();                                                    // 統編
            datRow["CORP_SEQ"] = NewString(bytes, 8, 4).Trim();                                                  // 統編序號
            datRow["MFAF_ID"] = NewString(bytes, 12, 9).Trim();                                                  // MFAF員編
            datRow["MFAF_NAME"] = NewString(bytes, 21, 20).Trim();                                        // MFAF姓名
            datRow["MFAF_AREA"] = NewString(bytes, 41, 30).Trim();                                          // MFAF區域中心
            datRow["MFAF_UPDATE_DATE"] = NewString(bytes, 71, 8).Trim();                           // MFAF生效日
            datRow["MOD_DATE"] = DateTime.Now;                                                                        // 異動日

            result.Rows.Add(datRow);
        }

        streamReader.Dispose();
        streamReader.Close();

        return result;
    }
    
    // 設定DataTable 表頭
    private DataTable SetDatTableHeader()
    {
        DataTable result = new DataTable();
        
        result.Columns.Add("CORP_NO", typeof(System.String));                                                    // 統編
        result.Columns.Add("CORP_SEQ", typeof(System.String));                                                  // 統編序號
        result.Columns.Add("MFAF_ID", typeof(System.String));                                                      // MFAF員編
        result.Columns.Add("MFAF_NAME", typeof(System.String));                                              // MFAF姓名
        result.Columns.Add("MFAF_AREA", typeof(System.String));                                                // MFAF區域中心
        result.Columns.Add("MFAF_UPDATE_DATE", typeof(System.String));                               // MFAF生效日
        result.Columns.Add("MOD_DATE", typeof(System.DateTime));                                                 // 異動日
        
        return result;
    }
        
    // 讀取檔案資料檔內容
    public DataTable GetFileToDataTable(string filePath, string fileName, out string errorMsg)
    {
        JobHelper.Write(this.JobID, "讀取檔案資料開始！", LogState.Info);

        DirectoryInfo di = new DirectoryInfo(filePath);
        DataTable dat = new DataTable();

        bool isDatOK = true;
        int total = 0;
        errorMsg = "";

        //因是設定檔資料。故重覆匯入是ok的，故不判斷檔案是否存在
        foreach (FileInfo file in di.GetFiles())
        {
            try
            {
                if (file.Extension.ToUpper() == ".TXT")
                {
                    // 取DataTable 資料
                    dat = GetDatDataTable(file, ref errorMsg, out total);
                }
            }
            catch (Exception ex)
            {
                JobHelper.Write(this.JobID, ex.Message);
            }
        }
        
        // 判斷檔案錯誤訊息
        if (errorMsg.Trim().Equals(""))
        {
            errorMsg = GetErrorMsg(isDatOK, dat.Rows.Count, total);
        }

        JobHelper.Write(this.JobID, "讀取檔案資料結束！", LogState.Info);

        return dat;
    }
    
    // 判斷檔案錯誤訊息
    private string GetErrorMsg(bool isDatOK, int datCount, int total)
    {
        string result = "";        

        if (!isDatOK)
        {
            result = " 資料長度不正確";
        }
        else if (datCount == 0)
        {
            result = " 資料為空檔，請確認";
        }

        return result;
    }

    private string NewString(byte[] bytes, int startPoint, int length)
    {
        string result = "";

        char[] chars = System.Text.Encoding.Default.GetChars(bytes, startPoint, length);

        foreach (char chr in chars)
        {
            result = result + chr;
        }

        return result;
    }


    // 寫入資料庫
    public bool SetDataTableToDB(DataTable sourceDat, ref string errorMsg)
    {
        bool isInsertIMPdata = false;
        try
        {
            
            JobHelper.Write(this.JobID, "AML_HQ_MFAF資料庫寫檔開始！ ", LogState.Info);

            isInsertIMPdata = CSIPKeyInGUI.BusinessRules_new.BRAML_HQ_MFAF.AML_HQ_MFAFWithBulkCopy("AML_HQ_MFAF", sourceDat, ref errorMsg);

            JobHelper.Write(this.JobID, "AML_HQ_MFAF資料庫寫檔結束！", LogState.Info);

            errorMsg = "";
        }
        catch(Exception ex)
        {
            errorMsg = ex.Message;
            isInsertIMPdata = false;
            JobHelper.Write(this.JobID, "[FAIL] " + ex.ToString());
        }

        return isInsertIMPdata;
    }
    
}