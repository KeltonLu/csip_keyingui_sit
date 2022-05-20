using System;
using System.Data;
using CSIPCommonModel.BusinessRules;
using System.Text;
using System.IO;
using Framework.Common.Logging;
using Framework.Common.Message;

/// <summary>
/// BatchJob_SendTwoMonthAddressLebel 不合作通知函Service 的摘要描述
/// </summary>
public class PostTwoMonthAddLebelService
{
    private string jobID = string.Empty;

    public PostTwoMonthAddLebelService(string jobID)
    {
        this.jobID = jobID;
    }

    /// <summary>
    /// 算起迄時間
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    public void GetSearchTime(DateTime dateTime, out string startTime, out string endTime)
    {
        startTime = "";
        endTime = "";
        int day = (int)dateTime.DayOfWeek;
        int addDay = 0;
        DateTime operatorTime;

        switch (day)
        {
            case 5:
                addDay = -6;
                break;
            case 6:
                addDay = -7;
                break;
            case 0:
                addDay = -8;
                break;
            case 1:
                addDay = -9;
                break;
            case 2:
                addDay = -10;
                break;
            case 3:
                addDay = -11;
                break;
            case 4:
                addDay = -12;
                break;
        }

        operatorTime = dateTime.AddDays(addDay);
        startTime = operatorTime.ToString("yyyyMMdd");
        endTime = operatorTime.AddDays(6).ToString("yyyyMMdd");
    }
    
    /// <summary>
    /// 取郵局核印資料
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <returns></returns>
    //public DataTable GetSendToPostOfficeData(string endTime)
    //{
    //    return BRFORM_COLUMN.GetCaseToAMLData(endTime);
    //}


    /// <summary>
    /// 取得開案兩個月之後都還沒做結案的資料
    /// </summary>
    /// <returns></returns>
    public DataTable GetAddressLebelDataTwoMonth()
    {
        return BRFORM_COLUMN.GetAddressLebelDataTwoMonth();
    }
        

    /// <summary>
    /// 更新地址條曾經送過
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <returns></returns>
    public bool updatelabelsended2(string ID)
    {
        return BRFORM_COLUMN.updateLabelSended2(ID);
    }
    
    /// <summary>
    /// 取得不合作通知函地址為NULL的資料
    /// </summary>
    /// <returns></returns>
    public DataTable GetLebelDataNullTwoMonth()
    {
        return BRFORM_COLUMN.GetLebelDataNullTwoMonth();
    }
    

    public bool UploadFileToMFTP(string jobID, DataTable localFileInfo, string localPath, string fileName)
    {
        try
        {
            bool result = true;
            string ftpIP = string.Empty;
            string ftpId = string.Empty;
            string ftpPwd = string.Empty;
            string ftpPath = string.Empty;
            string ZipPwd = string.Empty;

            bool isGet = BRFORM_COLUMN.GetFTPInfo2(jobID, ref ftpIP, ref ftpId, ref ftpPwd, ref ftpPath, ref ZipPwd);

            string itemName = string.Empty;
            bool isSuccess = false;
            int falseCount = 0;

            // 檔案加密
            //bool isFileZip = ZipFile(jobID, localPath, fileName, ZipPwd);

            FTPFactory objFtp = new FTPFactory(ftpIP, "", ftpId, ftpPwd, "21", ftpPath, "Y");

            DirectoryInfo di = new DirectoryInfo(localPath);
            foreach (FileInfo file in di.GetFiles())
            {
                /*
                if (file.Name.IndexOf(".ZIP") >= 0)
                {
                    itemName = file.Name;
                    // FTP目錄名稱+FTP檔案名稱+Local檔案名稱
                    isSuccess = objFtp.Upload(ftpPath, itemName, localPath + itemName);
                    if (!isSuccess)
                    {
                        result = false;
                        Logging.SaveLog(ELogLayer.Util, this.jobID + itemName + "上傳MFTP失敗", ELogType.Debug);
                        falseCount++;
                    }
                }
                */


                isSuccess = objFtp.Upload(ftpPath, fileName, localPath + fileName);
                if (!isSuccess)
                {
                    result = false;
                    Logging.Log(this.jobID + itemName + "上傳MFTP失敗", LogState.Error, LogLayer.Util);
                    falseCount++;
                }

            }

            if (falseCount == 0)
            {
                //20200408 RQ-2019-030155-005 保留原始檔案
                //DeleteFile(localPath);
                result = true;
            }

            return result;
        }
        catch (Exception ex)
        {
            Logging.Log(this.jobID + " 上傳MFTP失敗：" + ex.ToString(), LogState.Error, LogLayer.Util);
            return false;
        }
    }

   
    /*======================================================================== Initiative =====================================================================================*/
    
    /// <summary>
    /// 寫檔
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <param name="content"></param>
    /// <param name="count"></param>
    /// <param name="datetime"></param>
    public void CreateFile(string path, string fileName, string content)
    //public void CreateFile(string path, string fileName, DataTable content)
    {
        MKDir(path);
        FileStream stream1 = new FileStream(path + fileName, FileMode.Append, FileAccess.Write);
        StreamWriter writer1 = new StreamWriter(stream1, Encoding.GetEncoding(950));
        writer1.WriteLine(content);
        writer1.Close();
        stream1.Close();
        //using (TextWriter fileTW = new StreamWriter(path + fileName))
        //{
        //    foreach (DataRow row in content.Rows)
        //    {
        //        fileTW.NewLine = "\n";
        //        fileTW.WriteLine(row["data"].ToString());
        //    }
        //    fileTW.Flush();
        //}
    }

    /// <summary>
    /// 發送 Mail
    /// </summary>
    /// <param name="jobID"></param>
    /// <param name="mailTitle"></param>
    /// <param name="mailBody"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    public bool SendMail(string jobID, string mailTitle, string mailBody, string status, DateTime startTime)
    {
        string emailMembers = new Com_AutoJob().GetEmailMembers("01", jobID);
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

    // 大量 Insert
    private bool BulklyInsertPostDetail(string tableName, DataTable postOfficeDetail)
    {
        bool result = false;

        if (BRFORM_COLUMN.InsertToInvData(tableName, postOfficeDetail))
        {
            result = true;
        }

        return result;
    }
    
    // 建立路徑
    private bool MKDir(string FolderPath)
    {
        try
        {
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);
            return true;
        }
        catch (Exception ex)
        {
            Logging.Log(this.jobID + " 目錄 " + FolderPath + " 建制失敗：" + ex.ToString(), LogState.Error, LogLayer.Util);
            return false;
        }
    }
}