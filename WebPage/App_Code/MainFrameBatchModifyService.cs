using System;
using System.Data;
using Framework.Common.Utility;
using System.IO;
using System.Text;
using Framework.Common.Message;
using Framework.Common.Logging;


/// <summary>
/// MainFrameBatchModifyService 的摘要描述
/// </summary>
public class MainFrameBatchModifyService
{
    private string jobID = string.Empty;//MainFrameBatchResult
    protected JobHelper JobHelper = new JobHelper();

    public MainFrameBatchModifyService(string jobID)
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


    public string DownloadFromFTP(string date, string localPath, string extension, ref string DownLoadMsg, ref bool isDownload)
    {
        string fileName = "";

        try
        {
            DataTable tblFileInfo = BRAML_File_Import.GetFileInfo(jobID);

            //如果Parameter有值，則使用Parameter的時間
            if (!string.IsNullOrEmpty(tblFileInfo.Rows[0]["Parameter"].ToString()))
            {
                date = tblFileInfo.Rows[0]["Parameter"].ToString().Trim();
            }

            fileName = tblFileInfo.Rows[0]["FtpFileName"].ToString().Replace("yyyyMMdd", date) + "." + extension;

            string ftpPwd = RedirectHelper.GetDecryptString(tblFileInfo.Rows[0]["FtpPwd"].ToString());

            FTPFactory objFtp = new FTPFactory(tblFileInfo.Rows[0]["FtpIP"].ToString(), "", tblFileInfo.Rows[0]["FtpUserName"].ToString(), ftpPwd, "21", localPath, "Y");
            
            isDownload = objFtp.Download(tblFileInfo.Rows[0]["FtpPath"].ToString(), fileName, localPath, fileName);

            if (isDownload)
            {
                DownLoadMsg = "";
                JobHelper.Write(this.jobID, fileName + " FTP 取檔成功", LogState.Info);
            }
            else
            {
                DownLoadMsg = "【JOB：" + this.jobID + "】[FAIL] 檔案：" + fileName + " 不存在， FTP 取檔失敗";
                JobHelper.Write(this.jobID, "[FAIL] 檔案: " + fileName + " 不存在， FTP 取檔失敗");
            }
        }
        catch (Exception ex)
        {
            JobHelper.Write(this.jobID, "[FAIL] " + ex.ToString());
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

        StreamReader streamReader = new StreamReader(file.FullName, System.Text.Encoding.UTF8);
        
        while ((fileRow = streamReader.ReadLine()) != null)
        {
            total++;
            datRow = result.NewRow();

            byte[] bytes = Encoding.UTF8.GetBytes(fileRow);
            if (fileRow.Length != 52)//確認每筆資料長度
            {
                ErrMsg = "第" + total.ToString() + "行資料長度不正確，請確認/n";
            }
            else
            {
                string[]_Col = fileRow.Split(',');
                datRow["FILENAME"] = file.Name;//檔案名稱
                datRow["CORP_NO"] = _Col[1].ToString().Trim();//統一編號1
                datRow["CORP_SEQ"] = _Col[2].ToString().Trim();//統一編號2
                datRow["MERCH_NO"] = _Col[3].ToString().Trim();//商店代碼
                datRow["STATUS_CODE"] = _Col[4].ToString().Trim();//狀態碼
                datRow["TERMINATE_DATE"] = _Col[6].ToString().Trim();//解約日期
                datRow["TERMINATE_CODE"] = _Col[5].ToString().Trim();//解約原因
                datRow["UPDATE_CNT"] = _Col[7].ToString().Trim();//更新筆數
                datRow["BATCH_DATE"] = _Col[0].ToString().Trim();//作業日(批次處理日)

                result.Rows.Add(datRow);
            }
        }

        streamReader.Dispose();
        streamReader.Close();

        return result;
    }

    // 設定DataTable 表頭
    private DataTable SetDatTableHeader()
    {
        DataTable result = new DataTable();

        result.Columns.Add("FILENAME", typeof(System.String));                            //檔案名稱
        result.Columns.Add("CORP_NO", typeof(System.String));                            //統一編號1
        result.Columns.Add("CORP_SEQ", typeof(System.String));                          //統一編號2
        result.Columns.Add("MERCH_NO", typeof(System.String));                        //商店代號
        result.Columns.Add("STATUS_CODE", typeof(System.String));                  //狀態碼
        result.Columns.Add("TERMINATE_DATE", typeof(System.String));           // 解約日期
        result.Columns.Add("TERMINATE_CODE", typeof(System.String));          // 解約CODE
        result.Columns.Add("UPDATE_CNT", typeof(System.String));                     //更新筆數
        result.Columns.Add("BATCH_DATE", typeof(System.String));                    // 作業日(批次處理日)

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
    
    // 讀取檔案資料檔內容
    public DataTable GetFileToDataTable(string filePath, string fileName, out string errorMsg)
    {
        JobHelper.Write(this.jobID, "讀取檔案資料開始！", LogState.Info);

        DirectoryInfo di = new DirectoryInfo(filePath);
        DataTable dat = new DataTable();

        bool isDatOK = true;
        int total = 0;
        errorMsg = "";

        // 判斷檔案是否已存在
        bool isFileExist = CSIPKeyInGUI.BusinessRules_new.BRSHOP_CHANGE.IsFileExist(fileName);

        if (!isFileExist)
        {
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
                    JobHelper.Write(this.jobID, ex.Message);
                }
            }
        }

        
        // 判斷檔案錯誤訊息
        if (errorMsg.Trim().Equals(""))
        {
            errorMsg = GetErrorMsg(isFileExist, isDatOK, dat.Rows.Count, total);
        }

        JobHelper.Write(this.jobID, "讀取檔案資料結束！", LogState.Info);

        return dat;
    }

    // 判斷檔案錯誤訊息
    private string GetErrorMsg(bool isFileExist, bool isDatOK, int datCount,int total)
    {
        string result = "";

        if (isFileExist)
        {
            result = "整批異動特約商店資料回檔已存在，如需重匯請刪除MAINFRAME_IMP_LOG table那筆資料";
        }
        else if (!isDatOK)
        {
            result = "整批異動特約商店資料回檔資料長度不正確";
        }
        else if (datCount == 0)
        {
            result = "整批異動特約商店資料回檔為空檔，請確認";
        }
        else if (datCount != total)
        {
            result = "整批異動特約商店資料回檔筆數不正確";
        }

        return result;
    }



    // 寫入資料庫
    public bool SetDataTableToDB(DataTable sourceDat, out string errorMsg)
    {
        bool isInsertIMPdata = false;

        try
        {
            JobHelper.Write(this.jobID, "資料庫寫檔開始！ ", LogState.Info);

            isInsertIMPdata = CSIPKeyInGUI.BusinessRules_new.BRSHOP_CHANGE.InsertMAINFRAME_IMP_LOGData("MAINFRAME_IMP_LOG", sourceDat);

            JobHelper.Write(this.jobID, "資料庫寫檔結束！", LogState.Info);

            errorMsg = "";
        }
        catch (Exception ex)
        {
            errorMsg = "【寫入資料庫失敗】" + ex.Message;
            isInsertIMPdata = false;
            JobHelper.Write(this.jobID, "Fail：" + ex.Message);
        }

        return isInsertIMPdata;
    }
}