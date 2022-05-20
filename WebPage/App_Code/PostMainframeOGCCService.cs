using System;
using System.Data;
using Framework.Common.Utility;
using System.IO;
using Framework.Common.Message;
using Framework.Common.Logging;
using System.Text;

/// <summary>
/// PostMainframeOGCCService 的摘要描述
/// </summary>
public class PostMainframeOGCCService
{
    private string JobID = string.Empty;
    protected JobHelper JobHelper = new JobHelper();

    public PostMainframeOGCCService(string JobID)
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
            Logging.Log(ex);
            // Logging.SaveLog(ex.ToString(), ELogType.Error);
        }
        return true;
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

    /// <summary>
    ///  取得法律形式及行業別設定，同步至主機
    ///  2020/02/13
    ///  RQ-2019-030155-003
    /// <returns></returns>
    public static DataTable SendPostOfficeTypeSetting()
    {
        return CSIPKeyInGUI.BusinessRules_new.BRPostOffice_CodeType.SendPostOfficeTypeSetting();
    }

    /// <summary>
    ///  取得國籍檔設定，同步至OMS
    ///  2021/01/05
    ///  RQ-2019-030155-003
    /// <returns></returns>
    public static DataTable SendToOMSCountrySetting()
    {
        return CSIPKeyInGUI.BusinessRules_new.BRPostOffice_CodeType.SendToOMSCountrySetting();
    }

    /// <summary>
    /// 寫檔
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <param name="content"></param>
    /// <param name="count"></param>
    /// <param name="datetime"></param>
    public void CreateFile(string path, string fileName, string content)
    {
        MKDir(path);
        FileStream stream1 = new FileStream(path + fileName, FileMode.Create, FileAccess.Write);
        StreamWriter writer1 = new StreamWriter(stream1, System.Text.Encoding.GetEncoding("BIG5"));
        writer1.Write(content);
        writer1.Close();
        stream1.Close();

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
            //Logging.SaveLog(ELogLayer.Util, this.JobID + " 目錄 " + FolderPath + " 建制失敗：" + ex.ToString(), ELogType.Debug);
            JobHelper.Write(this.JobID, "[FAIL] " + " 目錄 " + FolderPath + " 建制失敗：" + ex.ToString());
            return false;
        }
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

            bool isGet = CSIPCommonModel.BusinessRules.BRFORM_COLUMN.GetFTPInfo2(jobID, ref ftpIP, ref ftpId, ref ftpPwd, ref ftpPath, ref ZipPwd);

            string itemName = string.Empty;
            bool isSuccess = false;
            int falseCount = 0;
            
            FTPFactory objFtp = new FTPFactory(ftpIP, "", ftpId, ftpPwd, "21", ftpPath, "Y");

            DirectoryInfo di = new DirectoryInfo(localPath);
            foreach (FileInfo file in di.GetFiles())
            {
                isSuccess = objFtp.Upload(ftpPath, fileName, localPath + fileName);
                if (!isSuccess)
                {
                    result = false;
                    //Logging.SaveLog(ELogLayer.Util, this.JobID + itemName + "上傳MFTP失敗", ELogType.Debug);
                    Logging.Log(this.JobID + itemName + "上傳MFTP失敗", LogState.Debug, LogLayer.Util);

                    falseCount++;
                }
            }

            if (falseCount == 0)
            {
                result = true;
            }

            return result;
        }
        catch (Exception ex)
        {
            //Logging.SaveLog(ELogLayer.Util, this.JobID + " 上傳MFTP失敗：" + ex.ToString(), ELogType.Debug);
            JobHelper.Write(this.JobID, "[FAIL] " + " 上傳MFTP失敗：" + ex.ToString());
            return false;
        }
    }

}