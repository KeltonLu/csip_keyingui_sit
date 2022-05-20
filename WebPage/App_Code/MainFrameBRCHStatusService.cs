//******************************************************************
//*  作    者：
//*  功能說明：
//*  創建日期：
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*Ares Luke           2021/04/14        20200031-CSIP EOS       調整Log層級
//*******************************************************************

using System;
using System.Data;
using Framework.Common.Utility;
using System.IO;
using Framework.Common.Message;
using Framework.Common.Logging;
using System.Text;

/// <summary>
/// MainFrameBRCHStatusService 的摘要描述
/// </summary>
public class MainFrameBRCHStatusService
{
    private string JobID = string.Empty;
    protected JobHelper JobHelper = new JobHelper();

    public MainFrameBRCHStatusService(string JobID)
    {
        this.JobID = JobID;
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
            Logging.Log(ex.ToString(), LogState.Error, LogLayer.BusinessRule);
            //Logging.SaveLog(ex.ToString(), ELogType.Error);
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
                JobHelper.Write(this.JobID, "[FAIL] 檔案: " + fileName + " 不存在， FTP 取檔失敗", LogState.Error);
            }
        }
        catch (Exception ex)
        {
            JobHelper.Write(this.JobID, "[FAIL] " + ex.ToString(), LogState.Error);
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

        try
        {            
            while ((fileRow = streamReader.ReadLine()) != null)
            {
                total++;
                datRow = result.NewRow();
                if (Encoding.Default.GetBytes(fileRow).Length < 225)
                {
                    throw new Exception("資料長度不足225");
                }

                byte[] bytes = Encoding.GetEncoding("Big5").GetBytes(fileRow);

                datRow["CORP_NO"] = NewString(bytes, 0, 8).Trim();                                        //分公司統編
                datRow["CORP_SEQ"] = NewString(bytes, 8, 4).Trim();                                      // 分公司統編序號
                datRow["REG_ENG_NAME"] = NewString(bytes, 12, 60).Trim();                      // 登記英文名稱
                datRow["REG_CHI_NAME"] = NewString(bytes, 72, 122).Trim();                                // 登記中文名稱
                datRow["CREATE_DATE"] = NewString(bytes, 194, 8).Trim();                                            // 最早開店日期
                datRow["STATUS"] = NewString(bytes, 202, 1).Trim();                                                      // STATUS
                datRow["QUALIFY_FLAG"] = NewString(bytes, 203, 1).Trim();                                         // 符合資料FLAG
                datRow["CIRCULATE_MERCH"] = NewString(bytes, 204, 1).Trim();                                              //  流通的店
                datRow["UPDATE_DATE"] = NewString(bytes, 205, 8).Trim();                                               // 異動日
                datRow["HQ_CORP_NO"] = NewString(bytes, 213, 8).Trim();                  // 總公司統編
                datRow["HQ_CORP_SEQ"] = NewString(bytes, 221, 4).Trim();               // 總公司統編序號
                datRow["IMPORT_DATE"] = DateTime.Now;                                                                    // 匯入日期
                
                result.Rows.Add(datRow);
            }
        }
        catch (Exception e)
        {
            ErrMsg = e.Message;
        }
        finally
        {
            streamReader.Dispose();
            streamReader.Close();
        }
        

        return result;
    }
    
    // 設定DataTable 表頭
    private DataTable SetDatTableHeader()
    {
        DataTable result = new DataTable();
        
        result.Columns.Add("CORP_NO", typeof(System.String));                                 // 分公司統編
        result.Columns.Add("CORP_SEQ", typeof(System.String));                               // 分公司統編序號
        result.Columns.Add("REG_ENG_NAME", typeof(System.String));                    // 登記英文名稱
        result.Columns.Add("REG_CHI_NAME", typeof(System.String));                              //登記中文名稱
        result.Columns.Add("CREATE_DATE", typeof(System.String));                                         //最早開店日期
        result.Columns.Add("STATUS", typeof(System.String));                                          // STATUS
        result.Columns.Add("QUALIFY_FLAG", typeof(System.String));                                    //符合資料FLAG
        result.Columns.Add("CIRCULATE_MERCH", typeof(System.String));                                          //流通的店
        result.Columns.Add("UPDATE_DATE", typeof(System.String));                                          //異動日
        result.Columns.Add("HQ_CORP_NO", typeof(System.String));       // 總公司統編
        result.Columns.Add("HQ_CORP_SEQ", typeof(System.String));             //總公司統編序號
        result.Columns.Add("IMPORT_DATE", typeof(System.DateTime));                              //匯入日期
        
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
                JobHelper.Write(this.JobID, ex.Message, LogState.Error);
            }
        }
        
        // 判斷檔案錯誤訊息
        if (!errorMsg.Trim().Equals(""))
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
        else if (datCount != total)
        {
            result = " 資料筆數不正確";
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
            JobHelper.Write(this.JobID, "AML_BRCH_STATUS資料庫寫檔開始！ ", LogState.Info);

            isInsertIMPdata = CSIPKeyInGUI.BusinessRules_new.BRAML_BRCH_STATUS.AML_BRCH_STATUSWithBulkCopy("AML_BRCH_STATUS", sourceDat, ref errorMsg);

            JobHelper.Write(this.JobID, "AML_BRCH_STATUS資料庫寫檔結束！", LogState.Info);
        }
        catch (Exception ex)
        {
            errorMsg = ex.Message;
            isInsertIMPdata = false;
            JobHelper.Write(this.JobID, "[FAIL] " + ex.ToString(), LogState.Error);
        }

        return isInsertIMPdata;
    }
    
}