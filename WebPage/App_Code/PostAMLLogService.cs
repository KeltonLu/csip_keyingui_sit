using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CSIPCommonModel.BusinessRules;
using System.Text;
using System.IO;
using Framework.Common.Logging;
using Framework.Common.Message;
using Framework.Common.IO;
using Framework.Common.Utility;
using Framework.Data.OM.Transaction;
using CSIPCommonModel.EntityLayer;

/// <summary>
/// PostOfficeService 的摘要描述
/// </summary>
public class PostAMLLogService
{
    private string jobID = string.Empty;

    public PostAMLLogService(string jobID)
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
    /// 取異動紀錄
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <returns></returns>
    public DataTable GetPostToAML_LogData(string endTime)
    {
        return BRFORM_COLUMN.GetPostToAML_LogData(endTime);
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
                //20200408 RQ-2019-030155-005保留原始檔案
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
        StreamWriter writer1 = new StreamWriter(stream1, System.Text.Encoding.GetEncoding("BIG5"));
        writer1.Write(content);
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
        emailMembers = emailMembers != "" ? emailMembers.Substring(0, emailMembers.Length - 1) : "";

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
