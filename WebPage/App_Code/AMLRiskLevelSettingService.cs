using System;
using System.Data;
using Framework.Common.Utility;
using System.IO;
using Framework.Common.Message;
using Framework.Common.Logging;


/// <summary>
/// AMLRiskLevelSettingService 的摘要描述
/// </summary>
public class AMLRiskLevelSettingService
{
    private string JobID = string.Empty;//AMLRiskLevelSettingService
    protected JobHelper JobHelper = new JobHelper();

    public AMLRiskLevelSettingService(string jobID)
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

            //AML_TW_H_yyyyMMdd_UTF8
            fileName = tblFileInfo.Rows[0]["FtpFileName"].ToString().Replace("yyyyMMdd", date) + "." + extension;

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
        DataTable dt_Country = new DataTable();
        
        DataRow datRow = null;
        string fileRow = "";
        total = 0;

        StreamReader streamReader = new StreamReader(file.FullName, System.Text.Encoding.UTF8);
        string _CodeType = string.Empty;

        switch (this.JobID.Trim())
        {
            case "GetAMLRiskLevel_H"://高風險國家清單
                _CodeType = "12";
                break;
            case "GetAMLRiskLevel_I"://高度制裁國家清單
                _CodeType = "13";
                break;
            case "GetAMLRiskLevel_J": //一般制裁國家清單
                _CodeType = "15";
                break;
        }

        dt_Country = new DataTable();
        dt_Country = CSIPKeyInGUI.BusinessRules_new.BRPostOffice_CodeType.GetCodeType("1");

        while ((fileRow = streamReader.ReadLine()) != null)
        {
            if (result.Select("CODE_ID = '" + fileRow.Trim().Substring(0, 2) + "'").Length > 0)//如遇重覆國籍，即pass
                continue;
            else
            {
                datRow = result.NewRow();

                total++;
                datRow["TYPE"] = _CodeType;//類別
                datRow["CODE_ID"] = fileRow.Trim().Substring(0, 2);//對應KEY
                datRow["ORDERBY"] = total;//排序
                datRow["DESCRIPTION"] = "";//說明
                datRow["IsValid"] = "1";//是否啟用

                //因高風險設定檔僅有國家代碼，故抓國籍設定的中文名稱、英文名稱，用以存入db使用
                //Query DB一次，然後資料從DataTable篩選
                DataRow[] dr_Country = dt_Country.Select("CODE_ID = '" + fileRow.Trim().Substring(0, 2) + "'");
                foreach (DataRow row in dr_Country)
                {
                    datRow["CODE_NAME"] = row["CODE_NAME"];//中文名稱
                    datRow["CODE_EN_NAME"] = row["CODE_EN_NAME"];//英文名稱
                }

                result.Rows.Add(datRow);
            }
        }

        streamReader.Dispose();
        streamReader.Close();

        return result;
    }

    //讀取control檔裡的筆數資料
    private int GetCTLData(FileInfo file)
    {
        string fileRow = "";
        int total = 0;

        StreamReader streamReader = new StreamReader(file.FullName, System.Text.Encoding.UTF8);
        
        while ((fileRow = streamReader.ReadLine()) != null)
        {
            total = Convert.ToInt32(fileRow.Trim());
        }

        streamReader.Dispose();
        streamReader.Close();

        return total;
    }

    // 設定DataTable 表頭
    private DataTable SetDatTableHeader()
    {
        DataTable result = new DataTable();
        
        result.Columns.Add("TYPE", typeof(System.String));                            //類別
        result.Columns.Add("CODE_ID", typeof(System.String));                            //對應KEY
        result.Columns.Add("CODE_NAME", typeof(System.String));                          //中文名稱
        result.Columns.Add("CODE_EN_NAME", typeof(System.String));                        //英文名稱
        result.Columns.Add("ORDERBY", typeof(System.Int32));                  //排序
        result.Columns.Add("DESCRIPTION", typeof(System.String));           // 說明
        result.Columns.Add("IsValid", typeof(System.Int16));          // 是否啟用

        return result;
    }
        
    // 讀取檔案資料檔內容
    public DataTable GetFileToDataTable(string filePath, string fileName, out string errorMsg)
    {
        JobHelper.Write(this.JobID, "讀取檔案資料開始！", LogState.Info);

        DirectoryInfo di = new DirectoryInfo(filePath);
        DataTable dat = new DataTable();
        int ctlCNT = 0;//讀取control檔筆數

        bool isDatOK = true;
        int total = 0;
        errorMsg = "";

        //因是設定檔資料。故重覆匯入是ok的，故不判斷檔案是否存在
        foreach (FileInfo file in di.GetFiles())
        {
            try
            {
                if (file.Extension.ToUpper() == ".CTL")
                {
                    //讀取ctl檔案筆數
                    ctlCNT = GetCTLData(file);
                }

                if (file.Extension.ToUpper() == ".DAT")
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

        //確認檔案內容是否有重覆資料
        bool isDuplicate = false;

        // 判斷檔案錯誤訊息
        if (errorMsg.Trim().Equals(""))
        {
            errorMsg = GetErrorMsg(isDuplicate, isDatOK, dat.Rows.Count, total, ctlCNT);
        }

        JobHelper.Write(this.JobID, "讀取檔案資料結束！", LogState.Info);

        return dat;
    }
    
    // 判斷檔案錯誤訊息
    private string GetErrorMsg(bool isDuplicate, bool isDatOK, int datCount,int total , int ctlCNT)
    {
        string result = "";
        string _JobName = string.Empty;

        switch (this.JobID.Trim())
        {
            case "GetAMLRiskLevel_H"://高風險國家清單
                _JobName = "高風險國家清單";
                break;
            case "GetAMLRiskLevel_I"://高度制裁國家清單
                _JobName = "高度制裁國家清單";
                break;
            case "GetAMLRiskLevel_J": //一般制裁國家清單
                _JobName = "一般制裁國家清單";
                break;
        }

        if (isDuplicate)
        {
            result = _JobName + " 資料內容有重覆資料";
        }
        else if (!isDatOK)
        {
            result = _JobName + " 資料長度不正確";
        }
        else if (datCount == 0)
        {
            result = _JobName + " 資料為空檔，請確認";
        }
        //else if (datCount != total)
        //{
        //    result = _JobName + " 資料筆數不正確";
        //}
        //else if (ctlCNT != datCount)
        //{
        //    result = _JobName + "ctl檔筆數與資料內容筆數不相符";
        //}

        return result;
    }



    // 寫入資料庫
    public bool SetDataTableToDB(DataTable sourceDat, out string errorMsg)
    {
        bool isInsertIMPdata = false;

        try
        {
            JobHelper.Write(this.JobID, "資料庫寫檔開始！ ", LogState.Info);

            isInsertIMPdata = CSIPKeyInGUI.BusinessRules_new.BRPostOffice_CodeType.PostOffice_CodeTypeWithBulkCopy("PostOffice_CodeType", sourceDat);

            JobHelper.Write(this.JobID, "資料庫寫檔結束！", LogState.Info);

            errorMsg = "";
        }
        catch (Exception ex)
        {
            errorMsg = ex.Message;
            isInsertIMPdata = false;
            JobHelper.Write(this.JobID, "[FAIL] " + ex.ToString());
        }
        return isInsertIMPdata;
    }    
}