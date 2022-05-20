//******************************************************************
//*  作        者：
//*  功能說明：
//*  建立日期：
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*Ares Luke          2021/04/14         20200031-CSIP EOS       調整Log層級
//*******************************************************************

using System;
using System.Data;
using Framework.Common.Utility;
using System.IO;
using Framework.Common.Message;
using Framework.Common.Logging;
using System.Text;

/// <summary>
/// RMMMailResultFromOMIService 的摘要描述
/// </summary>
public class RMMMailResultFromOMIService
{
    private string JobID = string.Empty;
    protected JobHelper JobHelper = new JobHelper();

    public RMMMailResultFromOMIService(string JobID)
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
            //Logging.SaveLog(ex.ToString(), ELogType.Error);
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

            //INIB1083.TXT
            fileName = tblFileInfo.Rows[0]["FtpFileName"].ToString() + "." + extension.Trim().ToUpper();

            string ftpPwd = RedirectHelper.GetDecryptString(tblFileInfo.Rows[0]["FtpPwd"].ToString());

            FTPFactory objFtp = new FTPFactory(tblFileInfo.Rows[0]["FtpIP"].ToString(), "", tblFileInfo.Rows[0]["FtpUserName"].ToString(), ftpPwd, "21", localPath, "Y");
            
            isDownload = objFtp.Download(tblFileInfo.Rows[0]["FtpPath"].ToString(), fileName, localPath, fileName);

            if (isDownload)
            {
                DownLoadMsg = "";

                JobHelper.Write(this.JobID, fileName + " FTP 取檔成功",LogState.Info);
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
                if (Encoding.Default.GetBytes(fileRow).Length < 190)
                {
                    throw new Exception("資料長度不足190");
                }
                byte[] bytes = Encoding.GetEncoding("Big5").GetBytes(fileRow);
                
                datRow["CORP_NO"] = NewString(bytes, 0, 10).Trim();                                        // 統編
                datRow["EMAIL"] = NewString(bytes, 10, 50).Trim();                                            // EMAIL
                datRow["REG_NAME"] = NewString(bytes, 60, 122).Trim();                                // 登記中文名稱
                datRow["BATCHDATE"] = NewString(bytes, 182, 8).Trim();                                // 發送失敗日期

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
        
        result.Columns.Add("CORP_NO", typeof(System.String));                                              // 統編
        result.Columns.Add("EMAIL", typeof(System.String));                                                     // EMAIL
        result.Columns.Add("REG_NAME", typeof(System.String));                                           // 公司登記名稱
        result.Columns.Add("BATCHDATE", typeof(System.String));                                         //批次日期
        
        return result;
    }
        
    // 讀取檔案資料檔內容
    public DataTable GetFileToDataTable(string filePath, string fileName, out string errorMsg)
    {
        JobHelper.Write(this.JobID, "讀取檔案資料開始！", LogState.Info);

        DirectoryInfo di = new DirectoryInfo(filePath);
        DataTable dat = new DataTable();
        
        int total = 0;
        errorMsg = "";
        
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
        if (errorMsg.Trim().Equals(""))
        {
            errorMsg = GetErrorMsg(dat.Rows.Count, total);
        }

        JobHelper.Write(this.JobID, "讀取檔案資料結束！", LogState.Info);

        return dat;
    }
    
    // 判斷檔案錯誤訊息
    private string GetErrorMsg( int datCount, int total)
    {
        string result = "";        
        
        if (datCount == 0)
        {
            result = "資料為空檔";
        }
        else if (datCount != total)
        {
            result = "資料筆數不正確";
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
    //20200820-10月RC-email失敗的統編寫在批次通知信函內
    public bool SetDataTableToDB(DataTable sourceDat, string _ExecDate, ref string errorMsg, ref int Count_0049, ref string _returnCorp)
    {
        bool isUpdateData = false;
        DataTable mailLog = new DataTable();
        _returnCorp = string.Empty;//20200820-10月RC-email失敗的統編寫在批次通知信函內

        try
        {
            JobHelper.Write(this.JobID, "更新AML_AUDITMAILLOG 資料庫開始！ ", LogState.Info);

            mailLog = CSIPKeyInGUI.BusinessRules_new.BRAML_AUDITMAILLOG.GetMailLogData(_ExecDate.Trim(), "R");
            //根據回檔將LOG檔相對應的統編發送狀態更新為N
            foreach (DataRow row in sourceDat.Rows)
            {
                DataRow[] o_dr = mailLog.Select("CORP_NO = '" + row["CORP_NO"].ToString().Trim() + "'");
                if (o_dr.Length > 0)
                {
                    _returnCorp += row["CORP_NO"].ToString().Trim() + "|";
                    isUpdateData = CSIPKeyInGUI.BusinessRules_new.BRAML_AUDITMAILLOG.Update(row["CORP_NO"].ToString().Trim(), _ExecDate.Trim(), "R", ref errorMsg);
                    CSIPCommonModel.BusinessRules.BRFORM_COLUMN.AML_NOTELOG(o_dr[0]["CASE_NO"].ToString(), row["CORP_NO"].ToString(), "", "MERCHANTRMMRESULT", "傳送不合作通知函(Email)失敗");
                    Count_0049++;
                }                
            }
            if (_returnCorp.Length > 0)
            {
                _returnCorp = _returnCorp.Remove(_returnCorp.Length - 1, 1);
            }
            JobHelper.Write(this.JobID, "更新AML_AUDITMAILLOG 資料庫結束！結果： " + isUpdateData, isUpdateData ? LogState.Info : LogState.Error);

            if (isUpdateData)
            {
                JobHelper.Write(this.JobID, "更新AML_HQ_WORK的AddressLabelTwoMonthFlag 資料開始！ ", LogState.Info);

                //再以LOG檔的統編將AML_HQ_WORK的AddressLabelflag更新成NULL，以利發送紙本定審通知函
                isUpdateData = CSIPKeyInGUI.BusinessRules_new.BRAML_AUDITMAILLOG.UpdateHQ_WORKwithRMM(_ExecDate.Trim(), ref errorMsg);
            }

            JobHelper.Write(this.JobID, "更新AML_HQ_WORK的AddressLabelTwoMonthFlag 資料結束！結果：" + isUpdateData, isUpdateData ? LogState.Info : LogState.Error);
        }
        catch (Exception ex)
        {
            errorMsg = ex.Message;
            isUpdateData = false;
            JobHelper.Write(this.JobID, "[FAIL] " + ex.ToString(), LogState.Error);
        }

        return isUpdateData;
    }
}