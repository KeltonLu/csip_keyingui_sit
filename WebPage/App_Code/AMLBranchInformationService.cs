using System;
using System.Data;
using Framework.Common.Utility;
using System.IO;
using System.Text;
using Framework.Common.Message;
using Framework.Common.Logging;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using CSIPCommonModel.EntityLayer_new;
using CSIPCommonModel.EntityLayer;

/// <summary>
/// AMLBranchInformationService 的摘要描述
/// 收總/分公司資料 批次Service
/// 檔名：AML_E_HCOP_yyyyMMdd / AML_E_BRCH_yyyyMMdd 
/// </summary>
public class AMLBranchInformationService
{
    private string jobID = string.Empty;
    protected DateTime StartTime = DateTime.Now;// 記錄Job啟動時間
    protected JobHelper JobHelper = new JobHelper();
    private string csipSystem = "CSIP_System";
    private EntityAGENT_INFO eAgentInfo = new EntityAGENT_INFO();
    private List<string> NoLname_HCOP = new List<string>(); //無對應長姓名資料的總公司 KEY
    private List<string> NoLname_BRCH = new List<string>(); //無對應長姓名資料的分公司 KEY
    private List<string> LNameID_HQ_owner = new List<string>();     //存放 總公司-負責人 需要查長姓名的資料
    private List<string> LNameID_HQ_contact = new List<string>();   //存放 總公司 需要查長姓名的資料
    private List<string> LNameID_BRCH = new List<string>();         //存放 分公司 需要查長姓名的資料
    private List<string> LNameID_Manager = new List<string>();      //存放 高管   需要查長姓名的資料

    public AMLBranchInformationService(string jobID)
    {
        this.jobID = jobID;
        JobHelper.strJobID = jobID;
    }

    public AMLBranchInformationService(string jobID, EntityAGENT_INFO agentInfo)
    {
        this.jobID = jobID;
        this.eAgentInfo = agentInfo;
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




    public string DownloadFromFTPMaster(string date, string localPath, string extension, string filetemplate, ref bool isDownload)//20191230-RQ-2019-030155-002-批次信函調整：增加取得取檔結果的參數 by Peggy)
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

            fileName = filetemplate.Replace("yyyyMMdd", date) + "." + extension;
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
    
    // 取 Branch 檔 DataTable 資料
    private DataTable GetDatDataTableManager(FileInfo file, out bool isDatOK, out int total)
    {
        DataTable dt = new DataTable();
        string fileRow = "";
        total = 0;
        isDatOK = true;
        DateTime now = DateTime.Now;

        StreamReader streamReader = new StreamReader(file.FullName, System.Text.Encoding.Default);
        while ((fileRow = streamReader.ReadLine()) != null)
        {

            byte[] bytes = Encoding.Default.GetBytes(fileRow);


            string HCOP_BATCH_NO2 = NewString(bytes, 0, 14).Trim();
            string HCOP_INTER_ID2 = NewString(bytes, 14, 20).Trim();


            //先刪除高階管理人重複資料
            SqlCommand sqlcmd3 = new SqlCommand();
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            sqlcmd3.CommandText = string.Format(@"
                  delete [{0}].[dbo].[AML_HQ_Manager_Work] where HCOP_BATCH_NO = @batchno and HCOP_INTER_ID = @internalid 
                ", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            sqlcmd3.CommandType = CommandType.Text;
            sqlcmd3.Parameters.Add(new SqlParameter("@batchno", HCOP_BATCH_NO2));
            sqlcmd3.Parameters.Add(new SqlParameter("@internalid", HCOP_INTER_ID2));

            try
            {
                DataSet resultSet3 = BRAML_File_Import.SearchOnDataSet(sqlcmd3);
                if (resultSet3 != null)
                {

                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            for (int i = 0; i <= 11; i++)
            {

                int x = i * 185;
                string FileName = file.Name;
                string HCOP_BATCH_NO = NewString(bytes, 0, 14).Trim();
                string HCOP_INTER_ID = NewString(bytes, 14, 20).Trim();
                string HCOP_SIXM_TOT_AMT = convertnumber(NewString(bytes, 34, 17).Trim());
                string HCOP_KEY = NewString(bytes, 51, 12).Trim();
                string HCOP_BENE_NATION = NewString(bytes, 1183 + x, 2).Trim();
                string HCOP_BENE_NAME = NewString(bytes, 1185 + x, 40).Trim();
                string HCOP_BENE_BIRTH_DATE = NewString(bytes, 1225 + x, 8).Trim();
                string HCOP_BENE_ID = NewString(bytes, 1233 + x, 14).Trim();
                string HCOP_BENE_PASSPORT = NewString(bytes, 1247 + x, 22).Trim();
                string HCOP_BENE_PASSPORT_EXP = NewString(bytes, 1269 + x, 8).Trim();
                string HCOP_BENE_RESIDENT_NO = NewString(bytes, 1277 + x, 22).Trim();
                string HCOP_BENE_RESIDENT_EXP = NewString(bytes, 1299 + x, 8).Trim();
                string HCOP_BENE_OTH_CERT = NewString(bytes, 1307 + x, 22).Trim();
                string HCOP_BENE_OTH_CERT_EXP = NewString(bytes, 1329 + x, 8).Trim();
                string HCOP_BENE_JOB_TYPE = NewString(bytes, 1337 + x, 1).Trim();
                string HCOP_BENE_JOB_TYPE_2 = NewString(bytes, 1338 + x, 1).Trim();
                string HCOP_BENE_JOB_TYPE_3 = NewString(bytes, 1339 + x, 1).Trim();
                string HCOP_BENE_JOB_TYPE_4 = NewString(bytes, 1340 + x, 1).Trim();
                string HCOP_BENE_JOB_TYPE_5 = NewString(bytes, 1341 + x, 1).Trim();
                string HCOP_BENE_JOB_TYPE_6 = NewString(bytes, 1342 + x, 1).Trim();

                //string HCOP_BENE_RESERVED = NewString(bytes, 1339 + x, 29).Trim();
                //判斷若為長姓名時需加入指定陣列-LNameID_Manager
                if (NewString(bytes, 1339 + x, 1).Trim() == "Y")
                {
                    //加入待查長姓名字串: ex: ID|A1XXXXXXX9|HRSB190046400000000000001199641
                    LNameID_Manager.Add(getJC68ID(HCOP_BENE_ID, HCOP_BENE_PASSPORT, HCOP_BENE_RESIDENT_NO, string.Format("{0}{1}", HCOP_BATCH_NO, HCOP_INTER_ID)));
                }

                string HCOP_BENE_RESERVED = NewString(bytes, 1344 + x, 24).Trim();
                SqlCommand sqlcmd = new SqlCommand();


                sqlcmd.CommandText = @"
                INSERT INTO [dbo].[AML_HQ_Manager_Import] ([FileName], [HCOP_BATCH_NO], [HCOP_INTER_ID], 
                [HCOP_SIXM_TOT_AMT], [HCOP_KEY], [HCOP_BENE_NATION], [HCOP_BENE_NAME], [HCOP_BENE_BIRTH_DATE], [HCOP_BENE_ID], [HCOP_BENE_PASSPORT], [HCOP_BENE_PASSPORT_EXP], 
                [HCOP_BENE_RESIDENT_NO], [HCOP_BENE_RESIDENT_EXP], [HCOP_BENE_OTH_CERT], [HCOP_BENE_OTH_CERT_EXP], 
                [HCOP_BENE_JOB_TYPE],[HCOP_BENE_JOB_TYPE_2], [HCOP_BENE_JOB_TYPE_3], [HCOP_BENE_JOB_TYPE_4], [HCOP_BENE_JOB_TYPE_5], [HCOP_BENE_JOB_TYPE_6], [HCOP_BENE_RESERVED], 
                [Create_Time], [Create_User], [Create_Date],[HCOP_BENE_LNAME],[HCOP_BENE_ROMA])
	                                                VALUES (@FileName, @HCOP_BATCH_NO, @HCOP_INTER_ID,
                @HCOP_SIXM_TOT_AMT, @HCOP_KEY, @HCOP_BENE_NATION, @HCOP_BENE_NAME, @HCOP_BENE_BIRTH_DATE, @HCOP_BENE_ID, @HCOP_BENE_PASSPORT, @HCOP_BENE_PASSPORT_EXP, 
                @HCOP_BENE_RESIDENT_NO, @HCOP_BENE_RESIDENT_EXP, @HCOP_BENE_OTH_CERT, @HCOP_BENE_OTH_CERT_EXP, 
                @HCOP_BENE_JOB_TYPE, @HCOP_BENE_JOB_TYPE_2,@HCOP_BENE_JOB_TYPE_3,@HCOP_BENE_JOB_TYPE_4,@HCOP_BENE_JOB_TYPE_5,@HCOP_BENE_JOB_TYPE_6,@HCOP_BENE_RESERVED,
                @Create_Time,@Create_User,@Create_Date,@HCOP_BENE_LNAME,@HCOP_BENE_ROMA)
                ";

                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.Parameters.Add(new SqlParameter("@FileName", FileName));
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BATCH_NO", HCOP_BATCH_NO));
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_INTER_ID", HCOP_INTER_ID));
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_SIXM_TOT_AMT", HCOP_SIXM_TOT_AMT));
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_KEY", HCOP_KEY));
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_NATION", HCOP_BENE_NATION));


                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_NAME", HCOP_BENE_NAME));
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_BIRTH_DATE", HCOP_BENE_BIRTH_DATE));
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_ID", HCOP_BENE_ID));
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_PASSPORT", HCOP_BENE_PASSPORT));

                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_PASSPORT_EXP", HCOP_BENE_PASSPORT_EXP));
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_RESIDENT_NO", HCOP_BENE_RESIDENT_NO));
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_RESIDENT_EXP", HCOP_BENE_RESIDENT_EXP));
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_OTH_CERT", HCOP_BENE_OTH_CERT));


                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_OTH_CERT_EXP", HCOP_BENE_OTH_CERT_EXP));
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE", HCOP_BENE_JOB_TYPE));
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE_2", HCOP_BENE_JOB_TYPE_2));
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE_3", HCOP_BENE_JOB_TYPE_3));

                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE_4", HCOP_BENE_JOB_TYPE_4));
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE_5", HCOP_BENE_JOB_TYPE_5));
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE_6", HCOP_BENE_JOB_TYPE_6));
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_RESERVED", HCOP_BENE_RESERVED));


                sqlcmd.Parameters.Add(new SqlParameter("@Create_Time", now.ToString("HHmmss")));
                sqlcmd.Parameters.Add(new SqlParameter("@Create_User", this.csipSystem));
                sqlcmd.Parameters.Add(new SqlParameter("@Create_Date", now.ToString("yyyyMMdd")));

                //長姓名
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_LNAME", ""));
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_ROMA", ""));

                try
                {
                    DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
                    if (resultSet != null)
                    {
                        //result = true;
                    }
                }
                catch (Exception ex)
                {
                    Logging.Log(ex, LogLayer.BusinessRule);
                }




                SqlCommand sqlcmd2 = new SqlCommand();

                sqlcmd2.CommandText = @"
                INSERT INTO [dbo].[AML_HQ_Manager_Work] ([FileName], [HCOP_BATCH_NO], [HCOP_INTER_ID], 
                [HCOP_SIXM_TOT_AMT], [HCOP_KEY], [HCOP_BENE_NATION], [HCOP_BENE_NAME], [HCOP_BENE_BIRTH_DATE], [HCOP_BENE_ID], [HCOP_BENE_PASSPORT], [HCOP_BENE_PASSPORT_EXP], 
                [HCOP_BENE_RESIDENT_NO], [HCOP_BENE_RESIDENT_EXP], [HCOP_BENE_OTH_CERT], [HCOP_BENE_OTH_CERT_EXP], 
                [HCOP_BENE_JOB_TYPE],[HCOP_BENE_JOB_TYPE_2], [HCOP_BENE_JOB_TYPE_3], [HCOP_BENE_JOB_TYPE_4], [HCOP_BENE_JOB_TYPE_5], [HCOP_BENE_JOB_TYPE_6], [HCOP_BENE_RESERVED], 
                [Create_Time], [Create_User], [Create_Date],[HCOP_BENE_LNAME],[HCOP_BENE_ROMA])
	                                                VALUES (@FileName, @HCOP_BATCH_NO, @HCOP_INTER_ID,
                @HCOP_SIXM_TOT_AMT, @HCOP_KEY, @HCOP_BENE_NATION, @HCOP_BENE_NAME, @HCOP_BENE_BIRTH_DATE, @HCOP_BENE_ID, @HCOP_BENE_PASSPORT, @HCOP_BENE_PASSPORT_EXP, 
                @HCOP_BENE_RESIDENT_NO, @HCOP_BENE_RESIDENT_EXP, @HCOP_BENE_OTH_CERT, @HCOP_BENE_OTH_CERT_EXP, 
                @HCOP_BENE_JOB_TYPE, @HCOP_BENE_JOB_TYPE_2,@HCOP_BENE_JOB_TYPE_3,@HCOP_BENE_JOB_TYPE_4,@HCOP_BENE_JOB_TYPE_5,@HCOP_BENE_JOB_TYPE_6,@HCOP_BENE_RESERVED,
                @Create_Time,@Create_User,@Create_Date,@HCOP_BENE_LNAME,@HCOP_BENE_ROMA)
                ";

                sqlcmd2.CommandType = CommandType.Text;
                sqlcmd2.Parameters.Add(new SqlParameter("@FileName", FileName));
                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BATCH_NO", HCOP_BATCH_NO));
                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_INTER_ID", HCOP_INTER_ID));
                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_SIXM_TOT_AMT", HCOP_SIXM_TOT_AMT));
                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_KEY", HCOP_KEY));
                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BENE_NATION", HCOP_BENE_NATION));


                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BENE_NAME", HCOP_BENE_NAME));
                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BENE_BIRTH_DATE", HCOP_BENE_BIRTH_DATE));
                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BENE_ID", HCOP_BENE_ID));
                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BENE_PASSPORT", HCOP_BENE_PASSPORT));

                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BENE_PASSPORT_EXP", HCOP_BENE_PASSPORT_EXP));
                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BENE_RESIDENT_NO", HCOP_BENE_RESIDENT_NO));
                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BENE_RESIDENT_EXP", HCOP_BENE_RESIDENT_EXP));
                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BENE_OTH_CERT", HCOP_BENE_OTH_CERT));


                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BENE_OTH_CERT_EXP", HCOP_BENE_OTH_CERT_EXP));
                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE", HCOP_BENE_JOB_TYPE));
                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE_2", HCOP_BENE_JOB_TYPE_2));
                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE_3", HCOP_BENE_JOB_TYPE_3));

                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE_4", HCOP_BENE_JOB_TYPE_4));
                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE_5", HCOP_BENE_JOB_TYPE_5));
                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE_6", HCOP_BENE_JOB_TYPE_6));
                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BENE_RESERVED", HCOP_BENE_RESERVED));


                sqlcmd2.Parameters.Add(new SqlParameter("@Create_Time", now.ToString("HHmmss")));
                sqlcmd2.Parameters.Add(new SqlParameter("@Create_User", this.csipSystem));
                sqlcmd2.Parameters.Add(new SqlParameter("@Create_Date", now.ToString("yyyyMMdd")));

                //長姓名
                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BENE_LNAME", ""));
                sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BENE_ROMA", ""));

                try
                {
                    DataSet resultSet2 = BRAML_File_Import.SearchOnDataSet(sqlcmd2);
                    if (resultSet2 != null)
                    {
                        //result = true;
                    }
                }
                catch (Exception ex)
                {
                    Logging.Log(ex, LogLayer.BusinessRule);
                }
            }
        }

        streamReader.Dispose();
        streamReader.Close();

        return dt;
    }




    // 取 Branch 檔 DataTable 資料
    private DataTable GetDatDataTableMaster(FileInfo file, out bool isDatOK, out int total)
    {
        DataTable result = SetDatTableHeaderMaster();
        DataRow datRow = null;
        string fileRow = "";


        total = 0;
        isDatOK = true;
        StreamReader streamReader = new StreamReader(file.FullName, System.Text.Encoding.Default);
        while ((fileRow = streamReader.ReadLine()) != null)
        {

            string fileRow2 = fileRow;

            byte[] bytes = Encoding.GetEncoding("Big5").GetBytes(fileRow);

            datRow = result.NewRow();

            datRow["FileName"] = file.Name;
            datRow["HCOP_BATCH_NO"] = NewString(bytes, 0, 14).Trim();
            datRow["HCOP_INTER_ID"] = NewString(bytes, 14, 20).Trim();
            datRow["HCOP_SIXM_TOT_AMT"] = NewString(bytes, 34, 17).Trim();
            datRow["HCOP_MON_AMT1"] = NewString(bytes, 51, 17).Trim();
            datRow["HCOP_MON_AMT2"] = NewString(bytes, 68, 17).Trim();
            datRow["HCOP_MON_AMT3"] = NewString(bytes, 85, 17).Trim();
            datRow["HCOP_MON_AMT4"] = NewString(bytes, 102, 17).Trim();
            datRow["HCOP_MON_AMT5"] = NewString(bytes, 119, 17).Trim();
            datRow["HCOP_MON_AMT6"] = NewString(bytes, 136, 17).Trim();
            datRow["HCOP_MON_AMT7"] = NewString(bytes, 153, 17).Trim();
            datRow["HCOP_MON_AMT8"] = NewString(bytes, 170, 17).Trim();
            datRow["HCOP_MON_AMT9"] = NewString(bytes, 187, 17).Trim();
            datRow["HCOP_MON_AMT10"] = NewString(bytes, 204, 17).Trim();
            datRow["HCOP_MON_AMT11"] = NewString(bytes, 221, 17).Trim();
            datRow["HCOP_MON_AMT12"] = NewString(bytes, 238, 17).Trim();
            datRow["HCOP_KEY"] = NewString(bytes, 255, 12).Trim();

            string strflag = NewString(bytes, 255, 1).Trim();
            int intTemp;
            if (Int32.TryParse(strflag.Substring(0, 1), out intTemp))//20211122_Ares_Jack_判斷第一個字是數字 
            {
                datRow["HCOP_HEADQUATERS_CORP_NO"] = NewString(bytes, 255, 8).Trim();
                datRow["HCOP_HEADQUATERS_CORP_SEQ"] = NewString(bytes, 263, 4).Trim();
            }
            else
            {
                datRow["HCOP_HEADQUATERS_CORP_NO"] = NewString(bytes, 255, 10).Trim();
                datRow["HCOP_HEADQUATERS_CORP_SEQ"] = NewString(bytes, 265, 2).Trim();
            }

            datRow["HCOP_CORP_TYPE"] = NewString(bytes, 267, 1).Trim();
            datRow["HCOP_REGISTER_NATION"] = NewString(bytes, 268, 2).Trim();
            datRow["HCOP_CORP_REG_ENG_NAME"] = NewString(bytes, 270, 60).Trim();
            datRow["HCOP_REG_NAME"] = NewString(bytes, 330, 122).Trim();
            datRow["HCOP_NAME_0E"] = NewString(bytes, 330, 1).Trim();
            datRow["HCOP_NAME_CHI"] = NewString(bytes, 330, 122).Trim();
            datRow["HCOP_NAME_0F"] = NewString(bytes, 451, 1).Trim();
            datRow["HCOP_BUILD_DATE"] = NewString(bytes, 452, 8).Trim();
            datRow["HCOP_CC"] = NewString(bytes, 460, 12).Trim();
            datRow["HCOP_REG_CITY"] = NewString(bytes, 472, 14).Trim();
            datRow["HCOP_REG_ADDR1"] = NewString(bytes, 486, 30).Trim();
            datRow["HCOP_REG_ADDR2"] = NewString(bytes, 516, 30).Trim();
            datRow["HCOP_EMAIL"] = NewString(bytes, 546, 50).Trim();
            datRow["HCOP_NP_COMPANY_NAME"] = NewString(bytes, 596, 242).Trim();
            datRow["HCOP_OWNER_NATION"] = NewString(bytes, 838, 2).Trim();
            datRow["HCOP_OWNER_CHINESE_NAME"] = NewString(bytes, 840, 40).Trim();
            datRow["HCOP_OWNER_ENGLISH_NAME"] = NewString(bytes, 880, 40).Trim();
            datRow["HCOP_OWNER_BIRTH_DATE"] = NewString(bytes, 920, 8).Trim();
            datRow["HCOP_OWNER_ID"] = NewString(bytes, 928, 10).Trim();
            datRow["HCOP_OWNER_ID_ISSUE_DATE"] = NewString(bytes, 938, 8).Trim();
            datRow["HCOP_OWNER_ID_ISSUE_PLACE"] = NewString(bytes, 946, 20).Trim();
            datRow["HCOP_OWNER_ID_REPLACE_TYPE"] = NewString(bytes, 966, 1).Trim();
            datRow["HCOP_ID_PHOTO_FLAG"] = NewString(bytes, 967, 1).Trim();
            datRow["HCOP_PASSPORT"] = NewString(bytes, 968, 22).Trim();
            datRow["HCOP_PASSPORT_EXP_DATE"] = NewString(bytes, 990, 8).Trim();
            datRow["HCOP_RESIDENT_NO"] = NewString(bytes, 998, 22).Trim();
            datRow["HCOP_RESIDENT_EXP_DATE"] = NewString(bytes, 1020, 8).Trim();
            datRow["HCOP_OTHER_CERT"] = NewString(bytes, 1028, 22).Trim();
            datRow["HCOP_OTHER_CERT_EXP_DATE"] = NewString(bytes, 1050, 8).Trim();
            datRow["HCOP_COMPLEX_STR_CODE"] = NewString(bytes, 1058, 1).Trim();
            datRow["HCOP_ISSUE_STOCK_FLAG"] = NewString(bytes, 1059, 1).Trim();
            datRow["HCOP_COMP_TEL"] = NewString(bytes, 1060, 20).Trim();
            datRow["HCOP_MAILING_CITY"] = NewString(bytes, 1080, 14).Trim();
            datRow["HCOP_MAILING_ADDR1"] = NewString(bytes, 1094, 30).Trim();
            datRow["HCOP_MAILING_ADDR2"] = NewString(bytes, 1124, 30).Trim();
            datRow["HCOP_PRIMARY_BUSI_COUNTRY"] = NewString(bytes, 1154, 2).Trim();
            datRow["HCOP_BUSI_RISK_NATION_FLAG"] = NewString(bytes, 1156, 1).Trim();
            datRow["HCOP_BUSI_RISK_NATION_1"] = NewString(bytes, 1157, 2).Trim();
            datRow["HCOP_BUSI_RISK_NATION_2"] = NewString(bytes, 1159, 2).Trim();
            datRow["HCOP_BUSI_RISK_NATION_3"] = NewString(bytes, 1161, 2).Trim();
            datRow["HCOP_BUSI_RISK_NATION_4"] = NewString(bytes, 1163, 2).Trim();
            datRow["HCOP_BUSI_RISK_NATION_5"] = NewString(bytes, 1165, 2).Trim();
            datRow["HCOP_OVERSEAS_FOREIGN"] = NewString(bytes, 1167, 1).Trim();
            datRow["HCOP_OVERSEAS_FOREIGN_COUNTRY"] = NewString(bytes, 1168, 2).Trim();
            datRow["HCOP_REGISTER_US_STATE"] = NewString(bytes, 1170, 2).Trim();
            datRow["HCOP_BUSINESS_ORGAN_TYPE"] = NewString(bytes, 1172, 2).Trim();
            datRow["HCOP_CREATE_DATE"] = NewString(bytes, 1174, 8).Trim();
            datRow["HCOP_STATUS"] = NewString(bytes, 1182, 1).Trim();
            datRow["HCOP_QUALIFY_FLAG"] = NewString(bytes, 3403, 1).Trim();
            datRow["HCOP_CONTACT_NAME"] = NewString(bytes, 3404, 40).Trim();
            datRow["HCOP_EXAMINE_FLAG"] = NewString(bytes, 3444, 1).Trim();
            datRow["HCOP_ALLOW_ISSUE_STOCK_FLAG"] = NewString(bytes, 3445, 1).Trim();
            datRow["HCOP_CONTACT_TEL"] = NewString(bytes, 3446, 20).Trim();
            datRow["HCOP_UPDATE_DATE"] = NewString(bytes, 3466, 8).Trim();
            datRow["HCOP_CREATE_ID"] = NewString(bytes, 3474, 8).Trim();
            datRow["HCOP_UPDATE_ID"] = NewString(bytes, 3482, 8).Trim();
            datRow["HCOP_OWNER_CITY"] = NewString(bytes, 3490, 14).Trim();
            datRow["HCOP_OWNER_ADDR1"] = NewString(bytes, 3504, 30).Trim();
            datRow["HCOP_OWNER_ADDR2"] = NewString(bytes, 3534, 30).Trim();

            //判斷若為長姓名時需加入指定陣列-LNameID_HQ_owner
            if (NewString(bytes, 3564, 1) == "Y") //HCOP_OWNER_LNAM_FLAG
            {
                //加入待查字串: ex: ID|A12XXXXXX9|228688600000
                LNameID_HQ_owner.Add(getJC68ID(datRow["HCOP_OWNER_ID"].ToString(), datRow["HCOP_PASSPORT"].ToString(), datRow["HCOP_RESIDENT_NO"].ToString(), datRow["HCOP_KEY"].ToString()));
            }
            datRow["HCOP_OWNER_CHINESE_LNAME"] = "";
            datRow["HCOP_OWNER_ROMA"] = "";

            //判斷若為長姓名時需加入指定陣列-LNameID_HQ_contact
            if (NewString(bytes, 3565, 1).Trim() == "Y") //HCOP_CONTACT_LNAM_FLAG
            {
                //加入待字串(統編加流水號): ex: CORP|228688600001|228688600000
                //因聯絡人的key 是 統編+流水號, 所以下面在加入時,剛好代入值都相同
                LNameID_HQ_contact.Add(string.Format("CORP|{0}|{0}", datRow["HCOP_KEY"].ToString()));
            }
            datRow["HCOP_CONTACT_LNAME"] = "";
            datRow["HCOP_CONTACT_ROMA"] = "";

            //20210804 EOS_AML(NOVA) 自然人收單批次 by Ares Dennis
            datRow["HCOP_CC_2"] = NewString(bytes, 3566, 12).Trim();
            datRow["HCOP_CC_3"] = NewString(bytes, 3578, 12).Trim();
            datRow["HCOP_OC"] = NewString(bytes, 3590, 12).Trim();
            datRow["HCOP_INCOME_SOURCE"] = NewString(bytes, 3602, 20).Trim();
            datRow["HCOP_LAST_UPD_MAKER"] = NewString(bytes, 3622, 12).Trim();
            datRow["HCOP_LAST_UPD_CHECKER"] = NewString(bytes, 3634, 12).Trim();
            datRow["HCOP_LAST_UPD_BRANCH"] = NewString(bytes, 3646, 4).Trim();
            datRow["HCOP_LAST_UPDATE_DATE"] = NewString(bytes, 3650, 8).Trim();
            datRow["HCOP_COUNTRY_CODE_2"] = NewString(bytes, 3658, 2).Trim();
            datRow["HCOP_GENDER"] = NewString(bytes, 3660, 1).Trim();
            datRow["HCOP_REG_ZIP_CODE"] = NewString(bytes, 3661, 7).Trim();

            datRow["HCOP_MOBILE"] = NewString(bytes, 3668, 20).Trim();//20211207_Ares_Jack_增加欄位 HCOP_MOBILE

            datRow["HCOP_RESERVED_FILLER"] = NewString(bytes, 3688, 112).Trim();


            result.Rows.Add(datRow);
        }

        streamReader.Dispose();
        streamReader.Close();

        return result;
    }


    private bool ValidateFileLength(FileInfo file, int filerightlength, string filetypeword, string jobid)
    {

        bool isDatOK = true;
        bool isDatOKrtn = true;
        StreamReader streamReader = new StreamReader(file.FullName, System.Text.Encoding.Default);
        int intcouterror = 0;
        string fileRow = "";
        int fileline = 0;

        while ((fileRow = streamReader.ReadLine()) != null)
        {

            isDatOK = true;

            fileline = fileline + 1;

            byte[] bytes = Encoding.Default.GetBytes(fileRow);


            if (bytes.Length != filerightlength)
            {
                intcouterror = intcouterror + 1;
                isDatOK = false;
                isDatOKrtn = false;
            }
        }

        //20211007_Ares_Jack_調整Mail發送時機避免一筆錯誤發一封Mail
        if (isDatOK == false)
        {
            JobHelper.WriteError(this.jobID, "檔案" + file.FullName + filetypeword + "在第" + fileline + "筆發生長度不正確" + "共有" + intcouterror + "筆，長度不正確，正確長度需為" + filerightlength);

            AMLBranchInformationService aMLBranchInformationService = new AMLBranchInformationService(jobid);
            aMLBranchInformationService.SendMail("檔案" + file.FullName + filetypeword + "在第" + fileline + "筆發生長度不正確" + "共有" + intcouterror + "筆，長度不正確，正確長度需為" + filerightlength, "檔案" + file.FullName + filetypeword + "共有" + intcouterror + "筆，長度不正確", "", this.StartTime);

        }

        streamReader.Dispose();
        streamReader.Close();

        return isDatOKrtn;
    }


    // 取 Branch 檔 DataTable 資料
    private DataTable GetDatDataTable(FileInfo file, out bool isDatOK, out int total)
    {
        DataTable result = SetDatTableHeader();
        DataRow datRow = null;
        string fileRow = "";

        total = 0;
        isDatOK = true;

        StreamReader streamReader = new StreamReader(file.FullName, System.Text.Encoding.Default);

        while ((fileRow = streamReader.ReadLine()) != null)
        {


            string fileRow2 = fileRow;

            byte[] bytes = Encoding.Default.GetBytes(fileRow);

            datRow = result.NewRow();

            datRow["CASE_NO"] = "";
            datRow["FileName"] = file.Name;
            datRow["BRCH_BATCH_NO"] = NewString(bytes, 0, 14).Trim();
            datRow["BRCH_INTER_ID"] = NewString(bytes, 14, 20).Trim();
            datRow["BRCH_SIXM_TOT_AMT"] = NewString(bytes, 34, 17).Trim();
            datRow["BRCH_MON_AMT1"] = NewString(bytes, 51, 17).Trim();
            datRow["BRCH_MON_AMT2"] = NewString(bytes, 68, 17).Trim();
            datRow["BRCH_MON_AMT3"] = NewString(bytes, 85, 17).Trim();
            datRow["BRCH_MON_AMT4"] = NewString(bytes, 102, 17).Trim();
            datRow["BRCH_MON_AMT5"] = NewString(bytes, 119, 17).Trim();
            datRow["BRCH_MON_AMT6"] = NewString(bytes, 136, 17).Trim();
            datRow["BRCH_MON_AMT7"] = NewString(bytes, 153, 17).Trim();
            datRow["BRCH_MON_AMT8"] = NewString(bytes, 170, 17).Trim();
            datRow["BRCH_MON_AMT9"] = NewString(bytes, 187, 17).Trim();
            datRow["BRCH_MON_AMT10"] = NewString(bytes, 204, 17).Trim();
            datRow["BRCH_MON_AMT11"] = NewString(bytes, 221, 17).Trim();
            datRow["BRCH_MON_AMT12"] = NewString(bytes, 238, 17).Trim();
            datRow["BRCH_KEY"] = NewString(bytes, 255, 12).Trim();

            datRow["BRCH_NATU_ID"] = NewString(bytes, 638, 10).Trim();//自然人ID

            //20211007_Ares_Jack_BRCH_NATU_ID 不是空為自然人ID, 空為統編
            if (datRow["BRCH_NATU_ID"].ToString() != "")
            {
                    datRow["BRCH_BRCH_NO"] = NewString(bytes, 255, 9).Trim();
                    datRow["BRCH_BRCH_SEQ"] = NewString(bytes, 264, 3).Trim();
            }
            else
            {
                datRow["BRCH_BRCH_NO"] = NewString(bytes, 255, 8).Trim();
                datRow["BRCH_BRCH_SEQ"] = NewString(bytes, 263, 4).Trim();
            }

            datRow["BRCH_BRCH_TYPE"] = NewString(bytes, 267, 1).Trim();
            datRow["BRCH_NATION"] = NewString(bytes, 268, 2).Trim();
            datRow["BRCH_BIRTH_DATE"] = NewString(bytes, 270, 8).Trim();
            datRow["BRCH_PERM_CITY"] = fileRow2.Substring(278, 7).Trim();
            datRow["BRCH_PERM_ADDR1"] = fileRow2.Substring(286, 15).Trim();
            datRow["BRCH_PERM_ADDR2"] = fileRow2.Substring(302, 15).Trim();
            datRow["BRCH_CHINESE_NAME"] = fileRow2.Substring(318, 20).Trim();
            datRow["BRCH_ENGLISH_NAME"] = fileRow2.Substring(339, 20).Trim();
            datRow["BRCH_ID"] = NewString(bytes, 432, 10).Trim();
            datRow["BRCH_OWNER_ID_ISSUE_DATE"] = NewString(bytes, 442, 8).Trim();
            datRow["BRCH_OWNER_ID_ISSUE_PLACE"] = NewString(bytes, 450, 20).Trim();
            datRow["BRCH_OWNER_ID_REPLACE_TYPE"] = NewString(bytes, 470, 1).Trim();
            datRow["BRCH_ID_PHOTO_FLAG"] = NewString(bytes, 471, 1).Trim();
            datRow["BRCH_PASSPORT"] = NewString(bytes, 472, 22).Trim();
            datRow["BRCH_PASSPORT_EXP_DATE"] = NewString(bytes, 494, 8).Trim();
            datRow["BRCH_RESIDENT_NO"] = NewString(bytes, 502, 22).Trim();
            datRow["BRCH_RESIDENT_EXP_DATE"] = NewString(bytes, 524, 8).Trim();
            datRow["BRCH_OTHER_CERT"] = NewString(bytes, 532, 22).Trim();
            datRow["BRCH_OTHER_CERT_EXP_DATE"] = NewString(bytes, 554, 8).Trim();
            datRow["BRCH_COMP_TEL"] = NewString(bytes, 562, 20).Trim();
            datRow["BRCH_CREATE_DATE"] = NewString(bytes, 582, 8).Trim();
            datRow["BRCH_STATUS"] = NewString(bytes, 590, 1).Trim();
            datRow["BRCH_CIRCULATE_MERCH"] = NewString(bytes, 591, 1).Trim();
            datRow["BRCH_HQ_BRCH_NO"] = NewString(bytes, 592, 8).Trim();
            datRow["BRCH_HQ_BRCH_SEQ_NO"] = NewString(bytes, 600, 4).Trim();
            datRow["BRCH_UPDATE_DATE"] = NewString(bytes, 604, 8).Trim();
            datRow["BRCH_QUALIFY_FLAG"] = NewString(bytes, 612, 1).Trim();
            datRow["BRCH_UPDATE_ID"] = NewString(bytes, 613, 8).Trim();
            datRow["BRCH_REAL_CORP"] = NewString(bytes, 621, 8).Trim();

            //datRow["BRCH_RESERVED_FILLER"] = NewString(bytes, 629, 71).Trim();

            //判斷若為長姓名時需加入指定陣列-LNameID_BRCH            
            if (NewString(bytes, 629, 1).Trim() == "Y")
            {
                LNameID_BRCH.Add(getJC68ID(datRow["BRCH_ID"].ToString(), datRow["BRCH_PASSPORT"].ToString(), datRow["BRCH_RESIDENT_NO"].ToString(), datRow["BRCH_KEY"].ToString()));
            }

            //20201026-20201211RC
            //datRow["BRCH_RESERVED_FILLER"] = NewString(bytes, 630, 70).Trim();
            datRow["BRCH_BUILD_DATE"] = NewString(bytes, 630, 8).Trim();//分公司設立日期

            //20210804 EOS_AML(NOVA) 自然人收單批次 by Ares Dennis
            
            datRow["LAST_UPD_MAKER"] = NewString(bytes, 648, 12).Trim();//資料最後異動MAKER
            datRow["LAST_UPD_CHECKER"] = NewString(bytes, 660, 12).Trim();//資料最後異動CHECKER
            datRow["LAST_UPD_BRANCH"] = NewString(bytes, 672, 4).Trim();//資料最後異動分行
            datRow["LAST_UPD_DATE"] = NewString(bytes, 676, 8).Trim();//資料最後異動日期

            //20211007_Ares_Jack_新增欄位
            datRow["BRCH_REG_ENG_NAME"] = NewString(bytes, 684, 60).Trim();//分公司登記英文
            datRow["BRCH_REG_CHI_NAME"] = NewString(bytes, 744, 122).Trim();//分公司登記中文

            datRow["BRCH_RESERVED_FILLER"] = NewString(bytes, 866, 34).Trim();
            
            result.Rows.Add(datRow);
        }

        streamReader.Dispose();
        streamReader.Close();

        return result;
    }





    // 設定 Branch 檔 DataTable 表頭
    private DataTable SetDatTableHeaderMaster()
    {
        DataTable result = new DataTable();


        result.Columns.Add("FileName", typeof(System.String));               // RMM批號
        result.Columns.Add("HCOP_BATCH_NO", typeof(System.String));               // RMM批號
        result.Columns.Add("HCOP_INTER_ID", typeof(System.String));               // RMM批號
        result.Columns.Add("HCOP_SIXM_TOT_AMT", typeof(System.String));            // AML內部使用的編號
        result.Columns.Add("HCOP_MON_AMT1", typeof(System.String));            // AML內部使用的編號
        result.Columns.Add("HCOP_MON_AMT2", typeof(System.String));            // AML內部使用的編號
        result.Columns.Add("HCOP_MON_AMT3", typeof(System.String));            // AML內部使用的編號
        result.Columns.Add("HCOP_MON_AMT4", typeof(System.String));            // AML內部使用的編號
        result.Columns.Add("HCOP_MON_AMT5", typeof(System.String));            // AML內部使用的編號
        result.Columns.Add("HCOP_MON_AMT6", typeof(System.String));            // AML內部使用的編號
        result.Columns.Add("HCOP_MON_AMT7", typeof(System.String));            // AML內部使用的編號
        result.Columns.Add("HCOP_MON_AMT8", typeof(System.String));            // AML內部使用的編號
        result.Columns.Add("HCOP_MON_AMT9", typeof(System.String));            // AML內部使用的編號
        result.Columns.Add("HCOP_MON_AMT10", typeof(System.String));            // AML內部使用的編號
        result.Columns.Add("HCOP_MON_AMT11", typeof(System.String));            // AML內部使用的編號
        result.Columns.Add("HCOP_MON_AMT12", typeof(System.String));            // AML內部使用的編號
        result.Columns.Add("HCOP_KEY", typeof(System.String));                 // 資料日期
        result.Columns.Add("HCOP_HEADQUATERS_CORP_NO", typeof(System.String));               // 客戶ID
        result.Columns.Add("HCOP_HEADQUATERS_CORP_SEQ", typeof(System.String));      // 客戶英文姓名
        result.Columns.Add("HCOP_CORP_TYPE", typeof(System.String));      // 客戶中文姓名
        result.Columns.Add("HCOP_REGISTER_NATION", typeof(System.String));               // AML管理區分
        result.Columns.Add("HCOP_CORP_REG_ENG_NAME", typeof(System.String));      // 原本的風險等級
        result.Columns.Add("HCOP_REG_NAME", typeof(System.String));   // 原本的下次審查日期
        result.Columns.Add("HCOP_NAME_0E", typeof(System.String));           // 最新試算後的風險等級
        result.Columns.Add("HCOP_NAME_CHI", typeof(System.String));        // 最新試算後的下次審查日期
        result.Columns.Add("HCOP_NAME_0F", typeof(System.String));          // 資料最後異動Maker
        result.Columns.Add("HCOP_BUILD_DATE", typeof(System.String));         // 資料最後異動分行
        result.Columns.Add("HCOP_CC", typeof(System.String));           // 資料最後異動日期
        result.Columns.Add("HCOP_REG_CITY", typeof(System.String));               // 歸屬分行
        result.Columns.Add("HCOP_REG_ADDR1", typeof(System.String));                 // 案件種類
        result.Columns.Add("HCOP_REG_ADDR2", typeof(System.String));                 // FiledSAR Flag
        result.Columns.Add("HCOP_EMAIL", typeof(System.String));                   // 保留
        result.Columns.Add("HCOP_NP_COMPANY_NAME", typeof(System.String));   // 原本的下次審查日期
        result.Columns.Add("HCOP_OWNER_NATION", typeof(System.String));           // 最新試算後的風險等級
        result.Columns.Add("HCOP_OWNER_CHINESE_NAME", typeof(System.String));        // 最新試算後的下次審查日期
        result.Columns.Add("HCOP_OWNER_ENGLISH_NAME", typeof(System.String));          // 資料最後異動Maker
        result.Columns.Add("HCOP_OWNER_BIRTH_DATE", typeof(System.String));         // 資料最後異動分行
        result.Columns.Add("HCOP_OWNER_ID", typeof(System.String));           // 資料最後異動日期
        result.Columns.Add("HCOP_OWNER_ID_ISSUE_DATE", typeof(System.String));               // 歸屬分行
        result.Columns.Add("HCOP_OWNER_ID_ISSUE_PLACE", typeof(System.String));                 // 案件種類
        result.Columns.Add("HCOP_OWNER_ID_REPLACE_TYPE", typeof(System.String));                 // FiledSAR Flag
        result.Columns.Add("HCOP_ID_PHOTO_FLAG", typeof(System.String));                   // 保留
        result.Columns.Add("HCOP_PASSPORT", typeof(System.String));                   // 保留
        result.Columns.Add("HCOP_PASSPORT_EXP_DATE", typeof(System.String));               // 歸屬分行
        result.Columns.Add("HCOP_RESIDENT_NO", typeof(System.String));                 // 案件種類
        result.Columns.Add("HCOP_RESIDENT_EXP_DATE", typeof(System.String));                 // FiledSAR Flag
        result.Columns.Add("HCOP_OTHER_CERT", typeof(System.String));                   // 保留
        result.Columns.Add("HCOP_OTHER_CERT_EXP_DATE", typeof(System.String));                   // 保留
        result.Columns.Add("HCOP_COMPLEX_STR_CODE", typeof(System.String));         // 資料最後異動分行
        result.Columns.Add("HCOP_ISSUE_STOCK_FLAG", typeof(System.String));           // 資料最後異動日期
        result.Columns.Add("HCOP_COMP_TEL", typeof(System.String));               // 歸屬分行
        result.Columns.Add("HCOP_MAILING_CITY", typeof(System.String));                 // 案件種類
        result.Columns.Add("HCOP_MAILING_ADDR1", typeof(System.String));                 // FiledSAR Flag
        result.Columns.Add("HCOP_MAILING_ADDR2", typeof(System.String));                   // 保留
        result.Columns.Add("HCOP_PRIMARY_BUSI_COUNTRY", typeof(System.String));                   // 保留
        result.Columns.Add("HCOP_BUSI_RISK_NATION_FLAG", typeof(System.String));               // 歸屬分行
        result.Columns.Add("HCOP_BUSI_RISK_NATION_1", typeof(System.String));                 // 案件種類
        result.Columns.Add("HCOP_BUSI_RISK_NATION_2", typeof(System.String));                 // FiledSAR Flag
        result.Columns.Add("HCOP_BUSI_RISK_NATION_3", typeof(System.String));                   // 保留
        result.Columns.Add("HCOP_BUSI_RISK_NATION_4", typeof(System.String));                   // 保留
        result.Columns.Add("HCOP_BUSI_RISK_NATION_5", typeof(System.String));         // 資料最後異動分行
        result.Columns.Add("HCOP_OVERSEAS_FOREIGN", typeof(System.String));           // 資料最後異動日期
        result.Columns.Add("HCOP_OVERSEAS_FOREIGN_COUNTRY", typeof(System.String));               // 歸屬分行
        result.Columns.Add("HCOP_REGISTER_US_STATE", typeof(System.String));                 // 案件種類
        result.Columns.Add("HCOP_BUSINESS_ORGAN_TYPE", typeof(System.String));                 // FiledSAR Flag
        result.Columns.Add("HCOP_CREATE_DATE", typeof(System.String));                   // 保留
        result.Columns.Add("HCOP_STATUS", typeof(System.String));                   // 保留
        result.Columns.Add("HCOP_QUALIFY_FLAG", typeof(System.String));               // 歸屬分行
        result.Columns.Add("HCOP_CONTACT_NAME", typeof(System.String));                 // 案件種類
        result.Columns.Add("HCOP_EXAMINE_FLAG", typeof(System.String));                 // FiledSAR Flag
        result.Columns.Add("HCOP_ALLOW_ISSUE_STOCK_FLAG", typeof(System.String));                   // 保留
        result.Columns.Add("HCOP_CONTACT_TEL", typeof(System.String));                   // 保留
        result.Columns.Add("HCOP_UPDATE_DATE", typeof(System.String));         // 資料最後異動分行
        result.Columns.Add("HCOP_CREATE_ID", typeof(System.String));           // 資料最後異動日期
        result.Columns.Add("HCOP_UPDATE_ID", typeof(System.String));               // 歸屬分行
        result.Columns.Add("HCOP_OWNER_CITY", typeof(System.String));                 // 案件種類
        result.Columns.Add("HCOP_OWNER_ADDR1", typeof(System.String));                 // FiledSAR Flag
        result.Columns.Add("HCOP_OWNER_ADDR2", typeof(System.String));                   // 保留
        result.Columns.Add("HCOP_RESERVED_FILLER", typeof(System.String));                   // 保留
        result.Columns.Add("Create_Time", typeof(System.String));                 // FiledSAR Flag
        result.Columns.Add("Create_User", typeof(System.String));                   // 保留
        result.Columns.Add("Create_Date", typeof(System.String));                   // 保留
        result.Columns.Add("CaseProcess_User", typeof(System.String));                   // CaseProcess_User
        result.Columns.Add("CaseProcess_Status", typeof(System.String));                   // CaseProcess_User
        result.Columns.Add("CaseOwner_User", typeof(System.String));

        //長姓名 201909
        result.Columns.Add("HCOP_OWNER_CHINESE_LNAME", typeof(System.String));  //負責人 長中文姓名
        result.Columns.Add("HCOP_OWNER_ROMA", typeof(System.String));           //負責人 羅馬拼音
        result.Columns.Add("HCOP_CONTACT_LNAME", typeof(System.String));        //聯絡人 長中文姓名
        result.Columns.Add("HCOP_CONTACT_ROMA", typeof(System.String));         //聯絡人 羅馬拼音

        //20210804 EOS_AML(NOVA) 自然人收單批次 by Ares Dennis
        result.Columns.Add("HCOP_CC_2", typeof(System.String));         //行業別2
        result.Columns.Add("HCOP_CC_3", typeof(System.String));         //行業別3
        result.Columns.Add("HCOP_OC", typeof(System.String));         //職業別
        result.Columns.Add("HCOP_INCOME_SOURCE", typeof(System.String));         //主要收入及資產來源
        result.Columns.Add("HCOP_LAST_UPD_MAKER", typeof(System.String));         //資料最後異動MAKER
        result.Columns.Add("HCOP_LAST_UPD_CHECKER", typeof(System.String));         //資料最後異動CHECKER
        result.Columns.Add("HCOP_LAST_UPD_BRANCH", typeof(System.String));         //資料最後異動分行
        result.Columns.Add("HCOP_LAST_UPDATE_DATE", typeof(System.String));         //資料最後異動日期
        result.Columns.Add("HCOP_COUNTRY_CODE_2", typeof(System.String));         //國籍2
        result.Columns.Add("HCOP_GENDER", typeof(System.String));         //性別
        result.Columns.Add("HCOP_REG_ZIP_CODE", typeof(System.String));         //郵遞區號

        result.Columns.Add("HCOP_MOBILE", typeof(System.String));         ////20211207_Ares_Jack_增加欄位 HCOP_MOBILE

        return result;
    }



    // 設定 Branch 檔 DataTable 表頭
    private DataTable SetDatTableHeader()
    {
        DataTable result = new DataTable();


        result.Columns.Add("CASE_NO", typeof(System.String));               // RMM批號
        result.Columns.Add("FileName", typeof(System.String));               // RMM批號
        result.Columns.Add("BRCH_BATCH_NO", typeof(System.String));               // RMM批號
        result.Columns.Add("BRCH_INTER_ID", typeof(System.String));            // AML內部使用的編號
        result.Columns.Add("BRCH_SIXM_TOT_AMT", typeof(System.String));                 // 資料日期
        result.Columns.Add("BRCH_MON_AMT1", typeof(System.String));                 // 資料日期
        result.Columns.Add("BRCH_MON_AMT2", typeof(System.String));                 // 資料日期
        result.Columns.Add("BRCH_MON_AMT3", typeof(System.String));                 // 資料日期
        result.Columns.Add("BRCH_MON_AMT4", typeof(System.String));                 // 資料日期
        result.Columns.Add("BRCH_MON_AMT5", typeof(System.String));                 // 資料日期
        result.Columns.Add("BRCH_MON_AMT6", typeof(System.String));                 // 資料日期
        result.Columns.Add("BRCH_MON_AMT7", typeof(System.String));                 // 資料日期
        result.Columns.Add("BRCH_MON_AMT8", typeof(System.String));                 // 資料日期
        result.Columns.Add("BRCH_MON_AMT9", typeof(System.String));                 // 資料日期
        result.Columns.Add("BRCH_MON_AMT10", typeof(System.String));                 // 資料日期
        result.Columns.Add("BRCH_MON_AMT11", typeof(System.String));                 // 資料日期
        result.Columns.Add("BRCH_MON_AMT12", typeof(System.String));                 // 資料日期
        result.Columns.Add("BRCH_KEY", typeof(System.String));               // 客戶ID

        result.Columns.Add("BRCH_NATU_ID", typeof(System.String));//自然人ID
        result.Columns.Add("BRCH_BRCH_NO", typeof(System.String));      // 客戶英文姓名
        result.Columns.Add("BRCH_BRCH_SEQ", typeof(System.String));      // 客戶中文姓名
        result.Columns.Add("BRCH_BRCH_TYPE", typeof(System.String));               // AML管理區分
        result.Columns.Add("BRCH_NATION", typeof(System.String));      // 原本的風險等級
        result.Columns.Add("BRCH_BIRTH_DATE", typeof(System.String));   // 原本的下次審查日期
        result.Columns.Add("BRCH_PERM_CITY", typeof(System.String));           // 最新試算後的風險等級
        result.Columns.Add("BRCH_PERM_ADDR1", typeof(System.String));        // 最新試算後的下次審查日期
        result.Columns.Add("BRCH_PERM_ADDR2", typeof(System.String));          // 資料最後異動Maker
        result.Columns.Add("BRCH_CHINESE_NAME", typeof(System.String));         // 資料最後異動分行
        result.Columns.Add("BRCH_ENGLISH_NAME", typeof(System.String));           // 資料最後異動日期
        result.Columns.Add("BRCH_ID", typeof(System.String));               // 歸屬分行
        result.Columns.Add("BRCH_OWNER_ID_ISSUE_DATE", typeof(System.String));                 // 案件種類
        result.Columns.Add("BRCH_OWNER_ID_ISSUE_PLACE", typeof(System.String));                 // FiledSAR Flag
        result.Columns.Add("BRCH_OWNER_ID_REPLACE_TYPE", typeof(System.String));                   // 保留
        result.Columns.Add("BRCH_ID_PHOTO_FLAG", typeof(System.String));   // 原本的下次審查日期
        result.Columns.Add("BRCH_PASSPORT", typeof(System.String));           // 最新試算後的風險等級
        result.Columns.Add("BRCH_PASSPORT_EXP_DATE", typeof(System.String));        // 最新試算後的下次審查日期
        result.Columns.Add("BRCH_RESIDENT_NO", typeof(System.String));          // 資料最後異動Maker
        result.Columns.Add("BRCH_RESIDENT_EXP_DATE", typeof(System.String));         // 資料最後異動分行
        result.Columns.Add("BRCH_OTHER_CERT", typeof(System.String));           // 資料最後異動日期
        result.Columns.Add("BRCH_OTHER_CERT_EXP_DATE", typeof(System.String));               // 歸屬分行
        result.Columns.Add("BRCH_COMP_TEL", typeof(System.String));                 // 案件種類
        result.Columns.Add("BRCH_CREATE_DATE", typeof(System.String));                 // FiledSAR Flag
        result.Columns.Add("BRCH_STATUS", typeof(System.String));                   // 保留
        result.Columns.Add("BRCH_CIRCULATE_MERCH", typeof(System.String));                   // 保留
        result.Columns.Add("BRCH_HQ_BRCH_NO", typeof(System.String));               // 歸屬分行
        result.Columns.Add("BRCH_HQ_BRCH_SEQ_NO", typeof(System.String));                 // 案件種類
        result.Columns.Add("BRCH_UPDATE_DATE", typeof(System.String));                 // FiledSAR Flag
        result.Columns.Add("BRCH_QUALIFY_FLAG", typeof(System.String));                   // 保留

        result.Columns.Add("BRCH_UPDATE_ID", typeof(System.String));                 // FiledSAR Flag
        result.Columns.Add("BRCH_REAL_CORP", typeof(System.String));                   // 保留


        result.Columns.Add("BRCH_RESERVED_FILLER", typeof(System.String));                   // 保留
        result.Columns.Add("Create_Time", typeof(System.String));                 // FiledSAR Flag
        result.Columns.Add("Create_User", typeof(System.String));                   // 保留
        result.Columns.Add("Create_Date", typeof(System.String));                   // 保留

        //長姓名 201909
        result.Columns.Add("BRCH_CHINESE_LNAME", typeof(System.String));  //長中文姓名
        result.Columns.Add("BRCH_ROMA", typeof(System.String));           //羅馬拼音

        //20201026-20201211RC
        result.Columns.Add("BRCH_BUILD_DATE", typeof(System.String));//分公司設立日期

        //20210804 EOS_AML(NOVA) 自然人收單批次 by Ares Dennis
        
        result.Columns.Add("LAST_UPD_MAKER", typeof(System.String));//資料最後異動MAKER
        result.Columns.Add("LAST_UPD_CHECKER", typeof(System.String));//資料最後異動CHECKER
        result.Columns.Add("LAST_UPD_BRANCH", typeof(System.String));//資料最後異動分行
        result.Columns.Add("LAST_UPD_DATE", typeof(System.String));//資料最後異動日期

        //20211007_Ares_Jack_新增欄位
        result.Columns.Add("BRCH_REG_ENG_NAME", typeof(System.String));//分公司登記英文
        result.Columns.Add("BRCH_REG_CHI_NAME", typeof(System.String));//分公司登記中文


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



    // 組 AML_IMP_LOG 資料
    private DataTable SetAMLIMPLOGDataMaster(DataTable sourceDat, string fileNameDat)
    {
        DataTable result = SetAMLIMPLOGHeader();
        DataRow datRow = result.NewRow();
        DateTime now = DateTime.Now;

        datRow["FILE_TYPE"] = "HQ";
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


    // 組 AML_IMP_LOG 資料
    private DataTable SetAMLIMPLOGData(DataTable sourceDat, string fileNameDat)
    {
        DataTable result = SetAMLIMPLOGHeader();
        DataRow datRow = result.NewRow();
        DateTime now = DateTime.Now;

        datRow["FILE_TYPE"] = "BRCH";
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

    public string convertnumber(string originword)
    {
        double i = 0;
        if (originword.IndexOf("{") != -1)
        {

            originword = originword.Replace("{", "");

            if (originword != null && !originword.Equals(""))
            {
                originword = originword.Substring(0, originword.Length - 1) + "." + originword.Substring(originword.Length - 1, 1);
            }
            i = Convert.ToDouble(originword);
        }

        if (originword.IndexOf("}") != -1)
        {
            originword = originword.Replace("}", "");

            if (originword != null && !originword.Equals(""))
            {
                originword = originword.Substring(0, originword.Length - 1) + "." + originword.Substring(originword.Length - 1, 1);
            }

            i = 0 - Convert.ToDouble(originword);
        }
        return i + "";
    }




    // 組 AML_Cdata_Import 資料
    private DataTable SetAMLHQWORKImportData(DataTable sourceDat, string fileNameDat)
    {
        DataTable result = SetDatTableHeaderMaster();
        DataRow datRow = null;
        DateTime now = DateTime.Now;


        DataTable datagent = new DataTable();
        Hashtable ht = new Hashtable();
        Hashtable ht1 = new Hashtable();
        int iagent = 0;
        int totalcasecount = sourceDat.Rows.Count;
        // 判斷檔案是否已存在
        datagent = BRAML_File_Import.GetAgentData();
        if (datagent != null && datagent.Rows.Count > 1)
        {
            foreach (DataRow rowagent in datagent.Rows)
            {
                //AML_AGENT SEETING有設定的話。
                if (datagent.Rows.Count > 1)
                {
                    //代表已經執行到最後一個人
                    if (iagent == datagent.Rows.Count - 1)
                    {
                        //無條件將全部的案件給他
                        //ht的table是數量
                        //ht1為經辦人員id，這個是撈aml_agent_setting的add_user_id使用
                        ht.Add(iagent, totalcasecount);
                        ht1.Add(iagent, rowagent["USER_ID"].ToString());
                    }
                    else
                    {
                        //ht1為經辦人員id，這個是撈aml_agent_setting的add_user_id使用
                        ht.Add(iagent, Convert.ToInt32(Math.Round((sourceDat.Rows.Count * (Convert.ToDouble(rowagent["ASSIGN_RATE"]) / 100)), 0)));
                        ht1.Add(iagent, rowagent["USER_ID"].ToString());
                    }
                }
                //陸續把案件數量減掉
                totalcasecount = totalcasecount - Convert.ToInt32(Math.Round((sourceDat.Rows.Count * (Convert.ToDouble(rowagent["ASSIGN_RATE"]) / 100)), 0));
                iagent = iagent + 1;
            }
        }

        int i9 = ht.Count;
        string CaseOwner_User = "";
        int assignmaker = 0;

        foreach (DataRow row in sourceDat.Rows)
        {
            datRow = result.NewRow();

            //代表著aml_agent_setting有設定才會進來分配，不然caseowner就壓成空
            //等於false才進來分配
            if (BRAML_File_Import.boolnodispatch() == false)
            {
                if (ht[0] != null)
                {
                    //ht[assignmaker]>0 代表經辦還有案件數可供分派

                    if (Convert.ToInt32(ht[assignmaker]) > 0)
                    {
                        //從第一筆把案件分給一號，若一號都分完以後就分給二號，依此類推，因為每個經辦的量已經在上面分好了，所以這裡就用一筆一筆滾的方式就可以了。
                        //caseowner就是把這案件分給這個經辦的意思
                        CaseOwner_User = ht1[assignmaker] + "";
                        ht[assignmaker] = Convert.ToInt32(ht[assignmaker]) - 1;
                    }

                    //因為上一個人已經分完了，找下一個人分配案件。

                    else
                    {
                        assignmaker = assignmaker + 1;
                        //caseowner就是把這案件分給這個經辦的意思
                        CaseOwner_User = ht1[assignmaker] + "";
                        ht[assignmaker] = Convert.ToInt32(ht[assignmaker]) - 1;
                    }
                }
            }
            else
            {
                CaseOwner_User = "";
            }
            datRow["FileName"] = row["FileName"].ToString();
            datRow["HCOP_BATCH_NO"] = row["HCOP_BATCH_NO"].ToString();
            datRow["HCOP_INTER_ID"] = row["HCOP_INTER_ID"].ToString();
            datRow["HCOP_SIXM_TOT_AMT"] = convertnumber(row["HCOP_SIXM_TOT_AMT"].ToString());
            datRow["HCOP_MON_AMT1"] = convertnumber(row["HCOP_MON_AMT1"].ToString());
            datRow["HCOP_MON_AMT2"] = convertnumber(row["HCOP_MON_AMT2"].ToString());
            datRow["HCOP_MON_AMT3"] = convertnumber(row["HCOP_MON_AMT3"].ToString());
            datRow["HCOP_MON_AMT4"] = convertnumber(row["HCOP_MON_AMT4"].ToString());
            datRow["HCOP_MON_AMT5"] = convertnumber(row["HCOP_MON_AMT5"].ToString());
            datRow["HCOP_MON_AMT6"] = convertnumber(row["HCOP_MON_AMT6"].ToString());
            datRow["HCOP_MON_AMT7"] = convertnumber(row["HCOP_MON_AMT7"].ToString());
            datRow["HCOP_MON_AMT8"] = convertnumber(row["HCOP_MON_AMT8"].ToString());
            datRow["HCOP_MON_AMT9"] = convertnumber(row["HCOP_MON_AMT9"].ToString());
            datRow["HCOP_MON_AMT10"] = convertnumber(row["HCOP_MON_AMT10"].ToString());
            datRow["HCOP_MON_AMT11"] = convertnumber(row["HCOP_MON_AMT11"].ToString());
            datRow["HCOP_MON_AMT12"] = convertnumber(row["HCOP_MON_AMT12"].ToString());
            datRow["HCOP_KEY"] = row["HCOP_KEY"].ToString();
            datRow["HCOP_HEADQUATERS_CORP_NO"] = row["HCOP_HEADQUATERS_CORP_NO"].ToString();
            datRow["HCOP_HEADQUATERS_CORP_SEQ"] = row["HCOP_HEADQUATERS_CORP_SEQ"].ToString();
            datRow["HCOP_CORP_TYPE"] = row["HCOP_CORP_TYPE"].ToString();
            datRow["HCOP_REGISTER_NATION"] = row["HCOP_REGISTER_NATION"].ToString();
            datRow["HCOP_CORP_REG_ENG_NAME"] = row["HCOP_CORP_REG_ENG_NAME"].ToString();
            datRow["HCOP_REG_NAME"] = row["HCOP_REG_NAME"].ToString();
            datRow["HCOP_NAME_0E"] = row["HCOP_NAME_0E"].ToString();
            datRow["HCOP_NAME_CHI"] = row["HCOP_NAME_CHI"].ToString();
            datRow["HCOP_NAME_0F"] = row["HCOP_NAME_0F"].ToString();
            datRow["HCOP_BUILD_DATE"] = row["HCOP_BUILD_DATE"].ToString();
            datRow["HCOP_CC"] = row["HCOP_CC"].ToString();
            datRow["HCOP_REG_CITY"] = row["HCOP_REG_CITY"].ToString();
            datRow["HCOP_REG_ADDR1"] = row["HCOP_REG_ADDR1"].ToString();
            datRow["HCOP_REG_ADDR2"] = row["HCOP_REG_ADDR2"].ToString();
            datRow["HCOP_EMAIL"] = row["HCOP_EMAIL"].ToString();
            datRow["HCOP_NP_COMPANY_NAME"] = row["HCOP_NP_COMPANY_NAME"].ToString();
            datRow["HCOP_OWNER_NATION"] = row["HCOP_OWNER_NATION"].ToString();
            datRow["HCOP_OWNER_CHINESE_NAME"] = row["HCOP_OWNER_CHINESE_NAME"].ToString();
            datRow["HCOP_OWNER_ENGLISH_NAME"] = row["HCOP_OWNER_ENGLISH_NAME"].ToString();
            datRow["HCOP_OWNER_BIRTH_DATE"] = row["HCOP_OWNER_BIRTH_DATE"].ToString();
            datRow["HCOP_OWNER_ID"] = row["HCOP_OWNER_ID"].ToString();
            datRow["HCOP_OWNER_ID_ISSUE_DATE"] = row["HCOP_OWNER_ID_ISSUE_DATE"].ToString();
            datRow["HCOP_OWNER_ID_ISSUE_PLACE"] = row["HCOP_OWNER_ID_ISSUE_PLACE"].ToString();
            datRow["HCOP_OWNER_ID_REPLACE_TYPE"] = row["HCOP_OWNER_ID_REPLACE_TYPE"].ToString();
            datRow["HCOP_ID_PHOTO_FLAG"] = row["HCOP_ID_PHOTO_FLAG"].ToString();
            datRow["HCOP_PASSPORT"] = row["HCOP_PASSPORT"].ToString();
            datRow["HCOP_PASSPORT_EXP_DATE"] = row["HCOP_PASSPORT_EXP_DATE"].ToString();
            datRow["HCOP_RESIDENT_NO"] = row["HCOP_RESIDENT_NO"].ToString();
            datRow["HCOP_RESIDENT_EXP_DATE"] = row["HCOP_RESIDENT_EXP_DATE"].ToString();
            datRow["HCOP_OTHER_CERT"] = row["HCOP_OTHER_CERT"].ToString();
            datRow["HCOP_OTHER_CERT_EXP_DATE"] = row["HCOP_OTHER_CERT_EXP_DATE"].ToString();
            datRow["HCOP_COMPLEX_STR_CODE"] = row["HCOP_COMPLEX_STR_CODE"].ToString();
            datRow["HCOP_ISSUE_STOCK_FLAG"] = row["HCOP_ISSUE_STOCK_FLAG"].ToString();
            datRow["HCOP_COMP_TEL"] = row["HCOP_COMP_TEL"].ToString();
            datRow["HCOP_MAILING_CITY"] = row["HCOP_MAILING_CITY"].ToString();
            datRow["HCOP_MAILING_ADDR1"] = row["HCOP_MAILING_ADDR1"].ToString();
            datRow["HCOP_MAILING_ADDR2"] = row["HCOP_MAILING_ADDR2"].ToString();
            datRow["HCOP_PRIMARY_BUSI_COUNTRY"] = row["HCOP_PRIMARY_BUSI_COUNTRY"].ToString();
            datRow["HCOP_BUSI_RISK_NATION_FLAG"] = row["HCOP_BUSI_RISK_NATION_FLAG"].ToString();
            datRow["HCOP_BUSI_RISK_NATION_1"] = row["HCOP_BUSI_RISK_NATION_1"].ToString();
            datRow["HCOP_BUSI_RISK_NATION_2"] = row["HCOP_BUSI_RISK_NATION_2"].ToString();
            datRow["HCOP_BUSI_RISK_NATION_3"] = row["HCOP_BUSI_RISK_NATION_3"].ToString();
            datRow["HCOP_BUSI_RISK_NATION_4"] = row["HCOP_BUSI_RISK_NATION_4"].ToString();
            datRow["HCOP_BUSI_RISK_NATION_5"] = row["HCOP_BUSI_RISK_NATION_5"].ToString();
            datRow["HCOP_OVERSEAS_FOREIGN"] = row["HCOP_OVERSEAS_FOREIGN"].ToString();
            datRow["HCOP_OVERSEAS_FOREIGN_COUNTRY"] = row["HCOP_OVERSEAS_FOREIGN_COUNTRY"].ToString();
            datRow["HCOP_REGISTER_US_STATE"] = row["HCOP_REGISTER_US_STATE"].ToString();
            datRow["HCOP_BUSINESS_ORGAN_TYPE"] = row["HCOP_BUSINESS_ORGAN_TYPE"].ToString();
            datRow["HCOP_CREATE_DATE"] = row["HCOP_CREATE_DATE"].ToString();
            datRow["HCOP_STATUS"] = row["HCOP_STATUS"].ToString();
            datRow["HCOP_QUALIFY_FLAG"] = row["HCOP_QUALIFY_FLAG"].ToString();
            datRow["HCOP_CONTACT_NAME"] = row["HCOP_CONTACT_NAME"].ToString();
            datRow["HCOP_EXAMINE_FLAG"] = row["HCOP_EXAMINE_FLAG"].ToString();
            datRow["HCOP_ALLOW_ISSUE_STOCK_FLAG"] = row["HCOP_ALLOW_ISSUE_STOCK_FLAG"].ToString();
            datRow["HCOP_CONTACT_TEL"] = row["HCOP_CONTACT_TEL"].ToString();
            datRow["HCOP_UPDATE_DATE"] = row["HCOP_UPDATE_DATE"].ToString();
            datRow["HCOP_CREATE_ID"] = row["HCOP_CREATE_ID"].ToString();
            datRow["HCOP_UPDATE_ID"] = row["HCOP_UPDATE_ID"].ToString();
            datRow["HCOP_OWNER_CITY"] = row["HCOP_OWNER_CITY"].ToString();
            datRow["HCOP_OWNER_ADDR1"] = row["HCOP_OWNER_ADDR1"].ToString();
            datRow["HCOP_OWNER_ADDR2"] = row["HCOP_OWNER_ADDR2"].ToString();
            datRow["HCOP_RESERVED_FILLER"] = row["HCOP_RESERVED_FILLER"].ToString();
            datRow["Create_Time"] = now.ToString("HHmmss");
            datRow["Create_User"] = this.csipSystem;
            datRow["Create_Date"] = now.ToString("yyyyMMdd");
            datRow["CaseProcess_User"] = "M1";
            datRow["CaseProcess_Status"] = "0";
            datRow["CaseOwner_User"] = CaseOwner_User;

            //長姓名
            datRow["HCOP_OWNER_CHINESE_LNAME"] = row["HCOP_OWNER_CHINESE_LNAME"].ToString();
            datRow["HCOP_OWNER_ROMA"] = row["HCOP_OWNER_ROMA"].ToString();
            datRow["HCOP_CONTACT_LNAME"] = row["HCOP_CONTACT_LNAME"].ToString();
            datRow["HCOP_CONTACT_ROMA"] = row["HCOP_CONTACT_ROMA"].ToString();

            //20210804 EOS_AML(NOVA) 自然人收單批次 by Ares Dennis
            datRow["HCOP_CC_2"] = row["HCOP_CC_2"].ToString();
            datRow["HCOP_CC_3"] = row["HCOP_CC_3"].ToString();
            datRow["HCOP_OC"] = row["HCOP_OC"].ToString();
            datRow["HCOP_INCOME_SOURCE"] = row["HCOP_INCOME_SOURCE"].ToString();
            datRow["HCOP_LAST_UPD_MAKER"] = row["HCOP_LAST_UPD_MAKER"].ToString();
            datRow["HCOP_LAST_UPD_CHECKER"] = row["HCOP_LAST_UPD_CHECKER"].ToString();
            datRow["HCOP_LAST_UPD_BRANCH"] = row["HCOP_LAST_UPD_BRANCH"].ToString();
            datRow["HCOP_LAST_UPDATE_DATE"] = row["HCOP_LAST_UPDATE_DATE"].ToString();
            datRow["HCOP_COUNTRY_CODE_2"] = row["HCOP_COUNTRY_CODE_2"].ToString();
            datRow["HCOP_GENDER"] = row["HCOP_GENDER"].ToString();
            datRow["HCOP_REG_ZIP_CODE"] = row["HCOP_REG_ZIP_CODE"].ToString();
            //20211209_Ares_Jack_新增欄位 行動電話
            datRow["HCOP_MOBILE"] = row["HCOP_MOBILE"].ToString();
            
            result.Rows.Add(datRow);
        }

        return result;
    }


    // 組 AML_HQ_Import 資料
    private DataTable SetAMLMasterdataImportData(DataTable sourceDat, string fileNameDat)
    {
        DataTable result = SetDatTableHeaderMaster();
        DataRow datRow = null;
        DateTime now = DateTime.Now;

        foreach (DataRow row in sourceDat.Rows)
        {
            datRow = result.NewRow();

            datRow["FileName"] = row["FileName"].ToString();
            datRow["HCOP_BATCH_NO"] = row["HCOP_BATCH_NO"].ToString();
            datRow["HCOP_INTER_ID"] = row["HCOP_INTER_ID"].ToString();
            datRow["HCOP_SIXM_TOT_AMT"] = row["HCOP_SIXM_TOT_AMT"].ToString();
            datRow["HCOP_MON_AMT1"] = row["HCOP_MON_AMT1"].ToString();
            datRow["HCOP_MON_AMT2"] = row["HCOP_MON_AMT2"].ToString();
            datRow["HCOP_MON_AMT3"] = row["HCOP_MON_AMT3"].ToString();
            datRow["HCOP_MON_AMT4"] = row["HCOP_MON_AMT4"].ToString();
            datRow["HCOP_MON_AMT5"] = row["HCOP_MON_AMT5"].ToString();
            datRow["HCOP_MON_AMT6"] = row["HCOP_MON_AMT6"].ToString();
            datRow["HCOP_MON_AMT7"] = row["HCOP_MON_AMT7"].ToString();
            datRow["HCOP_MON_AMT8"] = row["HCOP_MON_AMT8"].ToString();
            datRow["HCOP_MON_AMT9"] = row["HCOP_MON_AMT9"].ToString();
            datRow["HCOP_MON_AMT10"] = row["HCOP_MON_AMT10"].ToString();
            datRow["HCOP_MON_AMT11"] = row["HCOP_MON_AMT11"].ToString();
            datRow["HCOP_MON_AMT12"] = row["HCOP_MON_AMT12"].ToString();
            datRow["HCOP_KEY"] = row["HCOP_KEY"].ToString();
            datRow["HCOP_HEADQUATERS_CORP_NO"] = row["HCOP_HEADQUATERS_CORP_NO"].ToString();
            datRow["HCOP_HEADQUATERS_CORP_SEQ"] = row["HCOP_HEADQUATERS_CORP_SEQ"].ToString();
            datRow["HCOP_CORP_TYPE"] = row["HCOP_CORP_TYPE"].ToString();
            datRow["HCOP_REGISTER_NATION"] = row["HCOP_REGISTER_NATION"].ToString();
            datRow["HCOP_CORP_REG_ENG_NAME"] = row["HCOP_CORP_REG_ENG_NAME"].ToString();
            datRow["HCOP_REG_NAME"] = row["HCOP_REG_NAME"].ToString();
            datRow["HCOP_NAME_0E"] = row["HCOP_NAME_0E"].ToString();
            datRow["HCOP_NAME_CHI"] = row["HCOP_NAME_CHI"].ToString();
            datRow["HCOP_NAME_0F"] = row["HCOP_NAME_0F"].ToString();
            datRow["HCOP_BUILD_DATE"] = row["HCOP_BUILD_DATE"].ToString();
            datRow["HCOP_CC"] = row["HCOP_CC"].ToString();
            datRow["HCOP_REG_CITY"] = row["HCOP_REG_CITY"].ToString();
            datRow["HCOP_REG_ADDR1"] = row["HCOP_REG_ADDR1"].ToString();
            datRow["HCOP_REG_ADDR2"] = row["HCOP_REG_ADDR2"].ToString();
            datRow["HCOP_EMAIL"] = row["HCOP_EMAIL"].ToString();
            datRow["HCOP_NP_COMPANY_NAME"] = row["HCOP_NP_COMPANY_NAME"].ToString();
            datRow["HCOP_OWNER_NATION"] = row["HCOP_OWNER_NATION"].ToString();
            datRow["HCOP_OWNER_CHINESE_NAME"] = row["HCOP_OWNER_CHINESE_NAME"].ToString();
            datRow["HCOP_OWNER_ENGLISH_NAME"] = row["HCOP_OWNER_ENGLISH_NAME"].ToString();
            datRow["HCOP_OWNER_BIRTH_DATE"] = row["HCOP_OWNER_BIRTH_DATE"].ToString();
            datRow["HCOP_OWNER_ID"] = row["HCOP_OWNER_ID"].ToString();
            datRow["HCOP_OWNER_ID_ISSUE_DATE"] = row["HCOP_OWNER_ID_ISSUE_DATE"].ToString();
            datRow["HCOP_OWNER_ID_ISSUE_PLACE"] = row["HCOP_OWNER_ID_ISSUE_PLACE"].ToString();
            datRow["HCOP_OWNER_ID_REPLACE_TYPE"] = row["HCOP_OWNER_ID_REPLACE_TYPE"].ToString();
            datRow["HCOP_ID_PHOTO_FLAG"] = row["HCOP_ID_PHOTO_FLAG"].ToString();
            datRow["HCOP_PASSPORT"] = row["HCOP_PASSPORT"].ToString();
            datRow["HCOP_PASSPORT_EXP_DATE"] = row["HCOP_PASSPORT_EXP_DATE"].ToString();
            datRow["HCOP_RESIDENT_NO"] = row["HCOP_RESIDENT_NO"].ToString();
            datRow["HCOP_RESIDENT_EXP_DATE"] = row["HCOP_RESIDENT_EXP_DATE"].ToString();
            datRow["HCOP_OTHER_CERT"] = row["HCOP_OTHER_CERT"].ToString();
            datRow["HCOP_OTHER_CERT_EXP_DATE"] = row["HCOP_OTHER_CERT_EXP_DATE"].ToString();
            datRow["HCOP_COMPLEX_STR_CODE"] = row["HCOP_COMPLEX_STR_CODE"].ToString();
            datRow["HCOP_ISSUE_STOCK_FLAG"] = row["HCOP_ISSUE_STOCK_FLAG"].ToString();
            datRow["HCOP_COMP_TEL"] = row["HCOP_COMP_TEL"].ToString();
            datRow["HCOP_MAILING_CITY"] = row["HCOP_MAILING_CITY"].ToString();
            datRow["HCOP_MAILING_ADDR1"] = row["HCOP_MAILING_ADDR1"].ToString();
            datRow["HCOP_MAILING_ADDR2"] = row["HCOP_MAILING_ADDR2"].ToString();
            datRow["HCOP_PRIMARY_BUSI_COUNTRY"] = row["HCOP_PRIMARY_BUSI_COUNTRY"].ToString();
            datRow["HCOP_BUSI_RISK_NATION_FLAG"] = row["HCOP_BUSI_RISK_NATION_FLAG"].ToString();
            datRow["HCOP_BUSI_RISK_NATION_1"] = row["HCOP_BUSI_RISK_NATION_1"].ToString();
            datRow["HCOP_BUSI_RISK_NATION_2"] = row["HCOP_BUSI_RISK_NATION_2"].ToString();
            datRow["HCOP_BUSI_RISK_NATION_3"] = row["HCOP_BUSI_RISK_NATION_3"].ToString();
            datRow["HCOP_BUSI_RISK_NATION_4"] = row["HCOP_BUSI_RISK_NATION_4"].ToString();
            datRow["HCOP_BUSI_RISK_NATION_5"] = row["HCOP_BUSI_RISK_NATION_5"].ToString();
            datRow["HCOP_OVERSEAS_FOREIGN"] = row["HCOP_OVERSEAS_FOREIGN"].ToString();
            datRow["HCOP_OVERSEAS_FOREIGN_COUNTRY"] = row["HCOP_OVERSEAS_FOREIGN_COUNTRY"].ToString();
            datRow["HCOP_REGISTER_US_STATE"] = row["HCOP_REGISTER_US_STATE"].ToString();
            datRow["HCOP_BUSINESS_ORGAN_TYPE"] = row["HCOP_BUSINESS_ORGAN_TYPE"].ToString();
            datRow["HCOP_CREATE_DATE"] = row["HCOP_CREATE_DATE"].ToString();
            datRow["HCOP_STATUS"] = row["HCOP_STATUS"].ToString();
            datRow["HCOP_QUALIFY_FLAG"] = row["HCOP_QUALIFY_FLAG"].ToString();
            datRow["HCOP_CONTACT_NAME"] = row["HCOP_CONTACT_NAME"].ToString();
            datRow["HCOP_EXAMINE_FLAG"] = row["HCOP_EXAMINE_FLAG"].ToString();
            datRow["HCOP_ALLOW_ISSUE_STOCK_FLAG"] = row["HCOP_ALLOW_ISSUE_STOCK_FLAG"].ToString();
            datRow["HCOP_CONTACT_TEL"] = row["HCOP_CONTACT_TEL"].ToString();
            datRow["HCOP_UPDATE_DATE"] = row["HCOP_UPDATE_DATE"].ToString();
            datRow["HCOP_CREATE_ID"] = row["HCOP_CREATE_ID"].ToString();
            datRow["HCOP_UPDATE_ID"] = row["HCOP_UPDATE_ID"].ToString();
            datRow["HCOP_OWNER_CITY"] = row["HCOP_OWNER_CITY"].ToString();
            datRow["HCOP_OWNER_ADDR1"] = row["HCOP_OWNER_ADDR1"].ToString();
            datRow["HCOP_OWNER_ADDR2"] = row["HCOP_OWNER_ADDR2"].ToString();
            datRow["HCOP_RESERVED_FILLER"] = row["HCOP_RESERVED_FILLER"].ToString();
            datRow["Create_Time"] = now.ToString("HHmmss");
            datRow["Create_User"] = this.csipSystem;
            datRow["Create_Date"] = now.ToString("yyyyMMdd");
            datRow["CaseProcess_User"] = "M1";
            datRow["CaseProcess_Status"] = "0";

            //長姓名
            datRow["HCOP_OWNER_CHINESE_LNAME"] = row["HCOP_OWNER_CHINESE_LNAME"].ToString();
            datRow["HCOP_OWNER_ROMA"] = row["HCOP_OWNER_ROMA"].ToString();
            datRow["HCOP_CONTACT_LNAME"] = row["HCOP_CONTACT_LNAME"].ToString();
            datRow["HCOP_CONTACT_ROMA"] = row["HCOP_CONTACT_ROMA"].ToString();

            //20210804 EOS_AML(NOVA) 自然人收單批次 by Ares Dennis
            datRow["HCOP_CC_2"] = row["HCOP_CC_2"].ToString();
            datRow["HCOP_CC_3"] = row["HCOP_CC_3"].ToString();
            datRow["HCOP_OC"] = row["HCOP_OC"].ToString();
            datRow["HCOP_INCOME_SOURCE"] = row["HCOP_INCOME_SOURCE"].ToString();
            datRow["HCOP_LAST_UPD_MAKER"] = row["HCOP_LAST_UPD_MAKER"].ToString();
            datRow["HCOP_LAST_UPD_CHECKER"] = row["HCOP_LAST_UPD_CHECKER"].ToString();
            datRow["HCOP_LAST_UPD_BRANCH"] = row["HCOP_LAST_UPD_BRANCH"].ToString();
            datRow["HCOP_LAST_UPDATE_DATE"] = row["HCOP_LAST_UPDATE_DATE"].ToString();
            datRow["HCOP_COUNTRY_CODE_2"] = row["HCOP_COUNTRY_CODE_2"].ToString();
            datRow["HCOP_GENDER"] = row["HCOP_GENDER"].ToString();
            datRow["HCOP_REG_ZIP_CODE"] = row["HCOP_REG_ZIP_CODE"].ToString();

            //20211207_Ares_Jack_新增欄位
            datRow["HCOP_MOBILE"] = row["HCOP_MOBILE"].ToString();//行對電話

            result.Rows.Add(datRow);
        }

        return result;
    }



    // 組 AML_BRCH_Import 資料
    private DataTable SetAMLBRCHdataworkImportData(DataTable sourceDat, string fileNameDat)
    {
        DataTable result = SetAMLBranchdataImportHeader();
        DataRow datRow = null;
        DateTime now = DateTime.Now;

        foreach (DataRow row in sourceDat.Rows)
        {
            datRow = result.NewRow();

            datRow["CASE_NO"] = row["CASE_NO"].ToString();
            datRow["FileName"] = row["FileName"].ToString();
            datRow["BRCH_BATCH_NO"] = row["BRCH_BATCH_NO"].ToString();
            datRow["BRCH_INTER_ID"] = row["BRCH_INTER_ID"].ToString();
            datRow["BRCH_SIXM_TOT_AMT"] = convertnumber(row["BRCH_SIXM_TOT_AMT"].ToString());
            datRow["BRCH_MON_AMT1"] = convertnumber(row["BRCH_MON_AMT1"].ToString());
            datRow["BRCH_MON_AMT2"] = convertnumber(row["BRCH_MON_AMT2"].ToString());
            datRow["BRCH_MON_AMT3"] = convertnumber(row["BRCH_MON_AMT3"].ToString());
            datRow["BRCH_MON_AMT4"] = convertnumber(row["BRCH_MON_AMT4"].ToString());
            datRow["BRCH_MON_AMT5"] = convertnumber(row["BRCH_MON_AMT5"].ToString());
            datRow["BRCH_MON_AMT6"] = convertnumber(row["BRCH_MON_AMT6"].ToString());
            datRow["BRCH_MON_AMT7"] = convertnumber(row["BRCH_MON_AMT7"].ToString());
            datRow["BRCH_MON_AMT8"] = convertnumber(row["BRCH_MON_AMT8"].ToString());
            datRow["BRCH_MON_AMT9"] = convertnumber(row["BRCH_MON_AMT9"].ToString());
            datRow["BRCH_MON_AMT10"] = convertnumber(row["BRCH_MON_AMT10"].ToString());
            datRow["BRCH_MON_AMT11"] = convertnumber(row["BRCH_MON_AMT11"].ToString());
            datRow["BRCH_MON_AMT12"] = convertnumber(row["BRCH_MON_AMT12"].ToString());
            datRow["BRCH_KEY"] = row["BRCH_KEY"].ToString();
            datRow["BRCH_BRCH_NO"] = row["BRCH_BRCH_NO"].ToString();
            datRow["BRCH_BRCH_SEQ"] = row["BRCH_BRCH_SEQ"].ToString();
            datRow["BRCH_BRCH_TYPE"] = row["BRCH_BRCH_TYPE"].ToString();
            datRow["BRCH_NATION"] = row["BRCH_NATION"].ToString();
            datRow["BRCH_BIRTH_DATE"] = row["BRCH_BIRTH_DATE"].ToString();
            datRow["BRCH_PERM_CITY"] = row["BRCH_PERM_CITY"].ToString();
            datRow["BRCH_PERM_ADDR1"] = row["BRCH_PERM_ADDR1"].ToString();
            datRow["BRCH_PERM_ADDR2"] = row["BRCH_PERM_ADDR2"].ToString();
            datRow["BRCH_CHINESE_NAME"] = row["BRCH_CHINESE_NAME"].ToString();
            datRow["BRCH_ENGLISH_NAME"] = row["BRCH_ENGLISH_NAME"].ToString();
            datRow["BRCH_ID"] = row["BRCH_ID"].ToString();
            datRow["BRCH_OWNER_ID_ISSUE_DATE"] = row["BRCH_OWNER_ID_ISSUE_DATE"].ToString();
            datRow["BRCH_OWNER_ID_ISSUE_PLACE"] = row["BRCH_OWNER_ID_ISSUE_PLACE"].ToString();
            datRow["BRCH_OWNER_ID_REPLACE_TYPE"] = row["BRCH_OWNER_ID_REPLACE_TYPE"].ToString();
            datRow["BRCH_ID_PHOTO_FLAG"] = row["BRCH_ID_PHOTO_FLAG"].ToString();
            datRow["BRCH_PASSPORT"] = row["BRCH_PASSPORT"].ToString();
            datRow["BRCH_PASSPORT_EXP_DATE"] = row["BRCH_PASSPORT_EXP_DATE"].ToString();
            datRow["BRCH_RESIDENT_NO"] = row["BRCH_RESIDENT_NO"].ToString();
            datRow["BRCH_RESIDENT_EXP_DATE"] = row["BRCH_RESIDENT_EXP_DATE"].ToString();
            datRow["BRCH_OTHER_CERT"] = row["BRCH_OTHER_CERT"].ToString();
            datRow["BRCH_OTHER_CERT_EXP_DATE"] = row["BRCH_OTHER_CERT_EXP_DATE"].ToString();
            datRow["BRCH_COMP_TEL"] = row["BRCH_COMP_TEL"].ToString();
            datRow["BRCH_CREATE_DATE"] = row["BRCH_CREATE_DATE"].ToString();
            datRow["BRCH_STATUS"] = row["BRCH_STATUS"].ToString();
            datRow["BRCH_CIRCULATE_MERCH"] = row["BRCH_CIRCULATE_MERCH"].ToString();
            datRow["BRCH_HQ_BRCH_NO"] = row["BRCH_HQ_BRCH_NO"].ToString();
            datRow["BRCH_HQ_BRCH_SEQ_NO"] = row["BRCH_HQ_BRCH_SEQ_NO"].ToString();
            datRow["BRCH_UPDATE_DATE"] = row["BRCH_UPDATE_DATE"].ToString();
            datRow["BRCH_QUALIFY_FLAG"] = row["BRCH_QUALIFY_FLAG"].ToString();
            datRow["BRCH_UPDATE_ID"] = row["BRCH_UPDATE_ID"].ToString();
            datRow["BRCH_REAL_CORP"] = row["BRCH_REAL_CORP"].ToString();
            datRow["BRCH_RESERVED_FILLER"] = row["BRCH_RESERVED_FILLER"].ToString();
            datRow["Create_Time"] = now.ToString("HHmmss");
            datRow["Create_User"] = this.csipSystem;
            datRow["Create_Date"] = now.ToString("yyyyMMdd");

            //長姓名
            datRow["BRCH_CHINESE_LNAME"] = row["BRCH_CHINESE_LNAME"].ToString();
            datRow["BRCH_ROMA"] = row["BRCH_ROMA"].ToString();

            //20201029-202012RC 增加分公司設立日期欄位
            datRow["BRCH_BUILD_DATE"] = row["BRCH_BUILD_DATE"].ToString();

            //20210804 EOS_AML(NOVA) 自然人收單批次 by Ares Dennis
            datRow["BRCH_NATU_ID"] = row["BRCH_NATU_ID"].ToString();//自然人ID
            datRow["LAST_UPD_MAKER"] = row["LAST_UPD_MAKER"].ToString();//資料最後異動MAKER
            datRow["LAST_UPD_CHECKER"] = row["LAST_UPD_CHECKER"].ToString();//資料最後異動CHECKER
            datRow["LAST_UPD_BRANCH"] = row["LAST_UPD_BRANCH"].ToString();//資料最後異動分行
            datRow["LAST_UPD_DATE"] = row["LAST_UPD_DATE"].ToString();//資料最後異動日期

            //20211007_Ares_Jack_新增欄位
            datRow["BRCH_REG_ENG_NAME"] = row["BRCH_REG_ENG_NAME"].ToString();//分公司登記英文
            datRow["BRCH_REG_CHI_NAME"] = row["BRCH_REG_CHI_NAME"].ToString();//分公司登記中文

            result.Rows.Add(datRow);
        }

        return result;
    }



    // 組 AML_BRCH_Import 資料
    private DataTable SetAMLBranchdataImportData(DataTable sourceDat, string fileNameDat)
    {
        DataTable result = SetAMLBranchdataImportHeader();
        DataRow datRow = null;
        DateTime now = DateTime.Now;

        foreach (DataRow row in sourceDat.Rows)
        {
            datRow = result.NewRow();

            datRow["CASE_NO"] = row["CASE_NO"].ToString();
            datRow["FileName"] = row["FileName"].ToString();
            datRow["BRCH_BATCH_NO"] = row["BRCH_BATCH_NO"].ToString();
            datRow["BRCH_INTER_ID"] = row["BRCH_INTER_ID"].ToString();
            datRow["BRCH_SIXM_TOT_AMT"] = row["BRCH_SIXM_TOT_AMT"].ToString();
            datRow["BRCH_MON_AMT1"] = row["BRCH_MON_AMT1"].ToString();
            datRow["BRCH_MON_AMT2"] = row["BRCH_MON_AMT2"].ToString();
            datRow["BRCH_MON_AMT3"] = row["BRCH_MON_AMT3"].ToString();
            datRow["BRCH_MON_AMT4"] = row["BRCH_MON_AMT4"].ToString();
            datRow["BRCH_MON_AMT5"] = row["BRCH_MON_AMT5"].ToString();
            datRow["BRCH_MON_AMT6"] = row["BRCH_MON_AMT6"].ToString();
            datRow["BRCH_MON_AMT7"] = row["BRCH_MON_AMT7"].ToString();
            datRow["BRCH_MON_AMT8"] = row["BRCH_MON_AMT8"].ToString();
            datRow["BRCH_MON_AMT9"] = row["BRCH_MON_AMT9"].ToString();
            datRow["BRCH_MON_AMT10"] = row["BRCH_MON_AMT10"].ToString();
            datRow["BRCH_MON_AMT11"] = row["BRCH_MON_AMT11"].ToString();
            datRow["BRCH_MON_AMT12"] = row["BRCH_MON_AMT12"].ToString();
            datRow["BRCH_KEY"] = row["BRCH_KEY"].ToString();
            datRow["BRCH_BRCH_NO"] = row["BRCH_BRCH_NO"].ToString();
            datRow["BRCH_BRCH_SEQ"] = row["BRCH_BRCH_SEQ"].ToString();
            datRow["BRCH_BRCH_TYPE"] = row["BRCH_BRCH_TYPE"].ToString();
            datRow["BRCH_NATION"] = row["BRCH_NATION"].ToString();
            datRow["BRCH_BIRTH_DATE"] = row["BRCH_BIRTH_DATE"].ToString();
            datRow["BRCH_PERM_CITY"] = row["BRCH_PERM_CITY"].ToString();
            datRow["BRCH_PERM_ADDR1"] = row["BRCH_PERM_ADDR1"].ToString();
            datRow["BRCH_PERM_ADDR2"] = row["BRCH_PERM_ADDR2"].ToString();
            datRow["BRCH_CHINESE_NAME"] = row["BRCH_CHINESE_NAME"].ToString();
            datRow["BRCH_ENGLISH_NAME"] = row["BRCH_ENGLISH_NAME"].ToString();
            datRow["BRCH_ID"] = row["BRCH_ID"].ToString();
            datRow["BRCH_OWNER_ID_ISSUE_DATE"] = row["BRCH_OWNER_ID_ISSUE_DATE"].ToString();
            datRow["BRCH_OWNER_ID_ISSUE_PLACE"] = row["BRCH_OWNER_ID_ISSUE_PLACE"].ToString();
            datRow["BRCH_OWNER_ID_REPLACE_TYPE"] = row["BRCH_OWNER_ID_REPLACE_TYPE"].ToString();
            datRow["BRCH_ID_PHOTO_FLAG"] = row["BRCH_ID_PHOTO_FLAG"].ToString();
            datRow["BRCH_PASSPORT"] = row["BRCH_PASSPORT"].ToString();
            datRow["BRCH_PASSPORT_EXP_DATE"] = row["BRCH_PASSPORT_EXP_DATE"].ToString();
            datRow["BRCH_RESIDENT_NO"] = row["BRCH_RESIDENT_NO"].ToString();
            datRow["BRCH_RESIDENT_EXP_DATE"] = row["BRCH_RESIDENT_EXP_DATE"].ToString();
            datRow["BRCH_OTHER_CERT"] = row["BRCH_OTHER_CERT"].ToString();
            datRow["BRCH_OTHER_CERT_EXP_DATE"] = row["BRCH_OTHER_CERT_EXP_DATE"].ToString();
            datRow["BRCH_COMP_TEL"] = row["BRCH_COMP_TEL"].ToString();
            datRow["BRCH_CREATE_DATE"] = row["BRCH_CREATE_DATE"].ToString();
            datRow["BRCH_STATUS"] = row["BRCH_STATUS"].ToString();
            datRow["BRCH_CIRCULATE_MERCH"] = row["BRCH_CIRCULATE_MERCH"].ToString();
            datRow["BRCH_HQ_BRCH_NO"] = row["BRCH_HQ_BRCH_NO"].ToString();
            datRow["BRCH_HQ_BRCH_SEQ_NO"] = row["BRCH_HQ_BRCH_SEQ_NO"].ToString();
            datRow["BRCH_UPDATE_DATE"] = row["BRCH_UPDATE_DATE"].ToString();
            datRow["BRCH_QUALIFY_FLAG"] = row["BRCH_QUALIFY_FLAG"].ToString();


            datRow["BRCH_UPDATE_ID"] = row["BRCH_UPDATE_ID"].ToString();
            datRow["BRCH_REAL_CORP"] = row["BRCH_REAL_CORP"].ToString();

            datRow["BRCH_RESERVED_FILLER"] = row["BRCH_RESERVED_FILLER"].ToString();
            datRow["Create_Time"] = now.ToString("HHmmss");
            datRow["Create_User"] = this.csipSystem;
            datRow["Create_Date"] = now.ToString("yyyyMMdd");

            //長姓名
            datRow["BRCH_CHINESE_LNAME"] = row["BRCH_CHINESE_LNAME"].ToString();
            datRow["BRCH_ROMA"] = row["BRCH_ROMA"].ToString();

            //20201029-202012RC 增加設立日期欄位
            datRow["BRCH_BUILD_DATE"] = row["BRCH_BUILD_DATE"].ToString();

            //20210804 EOS_AML(NOVA) 自然人收單批次 by Ares Dennis
            datRow["BRCH_NATU_ID"] = row["BRCH_NATU_ID"].ToString();//自然人ID
            datRow["LAST_UPD_MAKER"] = row["LAST_UPD_MAKER"].ToString();//資料最後異動MAKER
            datRow["LAST_UPD_CHECKER"] = row["LAST_UPD_CHECKER"].ToString();//資料最後異動CHECKER
            datRow["LAST_UPD_BRANCH"] = row["LAST_UPD_BRANCH"].ToString();//資料最後異動分行
            datRow["LAST_UPD_DATE"] = row["LAST_UPD_DATE"].ToString();//資料最後異動日期

            //20211007_Ares_Jack_新增欄位
            datRow["BRCH_REG_ENG_NAME"] = row["BRCH_REG_ENG_NAME"].ToString();//分公司登記英文
            datRow["BRCH_REG_CHI_NAME"] = row["BRCH_REG_CHI_NAME"].ToString();//分公司登記中文

            result.Rows.Add(datRow);
        }

        return result;
    }



    // 取 總公司 檔內容
    public DataTable GetMasterFileData(string filePath, string fileName, out string errorMsg2)
    {
        JobHelper.Write(this.jobID, "讀取要匯入的總公司檔案資料！", LogState.Info);

        DirectoryInfo di = new DirectoryInfo(filePath);
        DataTable dat = new DataTable();

        bool isDatOK = true;
        bool isCtlOK = true;
        int count = 0;
        int total = 0;
        errorMsg2 = "";

        // 判斷檔案是否已存在
        bool isFileExist = BRAML_File_Import.IsEFileExist(fileName);

        if (!isFileExist)
        {
            foreach (FileInfo file in di.GetFiles())
            {
                try
                {
                    if (file.Name.IndexOf("HCOP") > -1)
                    {
                        if (file.Extension == ".dat")
                        {
                            // 取 Branch 檔 DataTable 資料
                            dat = GetDatDataTableMaster(file, out isDatOK, out total);
                        }
                        else if (file.Extension == ".ctl")
                        {
                            // 取 Branch 檔 CTL 資料
                            count = GetCtlData(file, out isCtlOK);
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

        //刪舊有的重複資料
        foreach (DataRow row in dat.Rows)
        {
            string strHCOP_BATCH_NO = row["HCOP_BATCH_NO"].ToString();
            string strHCOP_INTER_ID = row["HCOP_INTER_ID"].ToString();
            string strHCOP_HEADQUATERS_CORP_NO = row["HCOP_HEADQUATERS_CORP_NO"].ToString();
            string strHCOP_HEADQUATERS_CORP_SEQ = row["HCOP_HEADQUATERS_CORP_SEQ"].ToString();

            BRAML_File_Import.deletehqwork(strHCOP_BATCH_NO, strHCOP_INTER_ID, strHCOP_HEADQUATERS_CORP_NO, strHCOP_HEADQUATERS_CORP_SEQ);
        }

        bool isDuplicate = false;

        // 判斷檔案錯誤訊息
        errorMsg2 = GetMasterErrorMsg(isFileExist, isDuplicate, isDatOK, isCtlOK, dat.Rows.Count, count, total);

        JobHelper.Write(this.jobID, "讀取檔案資料結束！", LogState.Info);

        return dat;
    }



    // 從總公司資料取得高階管理人員檔內容
    public DataTable GetMasterFileManagerData(string filePath, string fileName, out string errorMsg)
    {
        JobHelper.Write(this.jobID, "讀取要匯入的高階管理人員檔案資料！", LogState.Info);

        DirectoryInfo di = new DirectoryInfo(filePath);
        DataTable dat = new DataTable();

        bool isDatOK = true;
        bool isCtlOK = true;
        int count = 0;
        int total = 0;
        errorMsg = "";

        // 判斷檔案是否已存在
        bool isFileExist = false;

        if (!isFileExist)
        {
            foreach (FileInfo file in di.GetFiles())
            {
                try
                {
                    if (file.Name.IndexOf("HCOP") > -1)
                    {
                        if (file.Extension == ".dat")
                        {
                            // 取 Branch 檔 DataTable 資料
                            dat = GetDatDataTableManager(file, out isDatOK, out total);
                        }
                        else if (file.Extension == ".ctl")
                        {
                            // 取 Branch 檔 CTL 資料
                            count = GetCtlData(file, out isCtlOK);
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
        //bool isDuplicate = false;
        // 判斷檔案錯誤訊息
        //errorMsg = GetErrorMsg(isFileExist, isDuplicate, isDatOK, isCtlOK, dat.Rows.Count, count, total);

        JobHelper.Write(this.jobID, "讀取高階管理人員檔案資料結束！", LogState.Info);

        return dat;
    }




    // 取 Branch 檔內容
    public DataTable GetBranchFileData(string filePath, string fileName, out string errorMsg)
    {
        JobHelper.Write(this.jobID, "讀取要匯入的Branch檔案資料！", LogState.Info);

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
                        // 取 Branch 檔 DataTable 資料
                        if (ValidateFileLength(file, 900, "分公司匯入dat檔", this.jobID)) //檔案長度驗證將 700 調整為 900
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
                        // 取 Branch 檔 CTL 資料
                        if (ValidateFileLength(file, 10, "分公司匯入ctl檔", this.jobID))
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


        //刪舊有的重複資料
        foreach (DataRow row in dat.Rows)
        {
            string strBRCH_BATCH_NO = row["BRCH_BATCH_NO"].ToString();
            string strBRCH_INTER_ID = row["BRCH_INTER_ID"].ToString();
            string strBRCH_BRCH_NO = row["BRCH_BRCH_NO"].ToString();
            string strBRCH_BRCH_SEQ = row["BRCH_BRCH_SEQ"].ToString();



            BRAML_File_Import.deletebrchwork(strBRCH_BATCH_NO, strBRCH_INTER_ID, strBRCH_BRCH_NO, strBRCH_BRCH_SEQ);
        }

        bool isDuplicate = false;
        // 判斷檔案錯誤訊息
        errorMsg = GetErrorMsg(isFileExist, isDuplicate, isDatOK, isCtlOK, dat.Rows.Count, count, total);

        JobHelper.Write(this.jobID, "讀取檔案資料結束！", LogState.Info);

        return dat;
    }



    // 取 Branch 檔 CTL 資料
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
            result = "Branch檔已存在，如要重匯請刪除AML_IMP_LOG table那筆資料";
        }
        else if (isDuplicate)
        {
            result = "Branch檔內容有重覆資料";
        }
        else if (!isDatOK)
        {
            result = "Branch檔長度不正確";
        }
        else if (!isCtlOK)
        {
            result = "ctl檔值有誤";
        }
        else if (datCount == 0)
        {
            result = "Branch檔無處理資料";
        }
        else if (datCount != ctlCount)
        {
            result = "Branch檔筆數不正確";
        }

        return result;
    }

    // 判斷檔案錯誤訊息
    private string GetMasterErrorMsg(bool isFileExist, bool isDuplicate, bool isDatOK, bool isCtlOK, int datCount, int ctlCount, int total)
    {
        string result = "";

        if (isFileExist)
        {
            result = "Master檔已存在，如要重匯請刪除AML_IMP_LOG table那筆資料";
        }
        else if (isDuplicate)
        {
            result = "Master檔內容有重覆資料";
        }
        else if (!isDatOK)
        {
            result = "Master檔長度不正確";
        }
        else if (!isCtlOK)
        {
            result = "ctl檔值有誤";
        }
        else if (datCount == 0)
        {
            result = "Master檔無處理資料";
        }
        else if (datCount != ctlCount)
        {
            result = "Master檔筆數不正確";
        }

        return result;
    }


    // 寫入資料庫
    public bool SetRelationDataTableMaster(DataTable sourceDat, string fileNameDat2, string fileNameDat)
    {
        bool isInsertCdata = false;

        // 組 AML_IMP_LOG 資料
        DataTable dtAMLIMPLOG = SetAMLIMPLOGDataMaster(sourceDat, fileNameDat2);

        // 組 AML_HQ_Import 
        DataTable dtAMLCdataImport = SetAMLMasterdataImportData(sourceDat, fileNameDat);
        //組 AML_HQ_Work
        DataTable dtAMLHQWORKImport = SetAMLHQWORKImportData(sourceDat, fileNameDat);


        int masterID = BRAML_File_Import.InsertAMLIMPLOG(dtAMLIMPLOG);

        JobHelper.Write(this.jobID, "資料庫寫檔開始！ masterID = " + masterID, LogState.Info);

        if (masterID > 0)
        {
            // 寫入 AML_HQ_Import
            isInsertCdata = BRAML_File_Import.InsertAMLMasterdataImport("AML_HQ_Import", dtAMLCdataImport);
            if (isInsertCdata == false)
            {
                JobHelper.Write(this.jobID, "DB AML_HQ_Import 寫入失敗 ", LogState.Info);
                Logging.Log("DB AML_HQ_Import 寫入失敗", LogLayer.DB);
            }
                

            if (isInsertCdata)
            {
                // 寫入 AML_HQ_Work
                isInsertCdata = BRAML_File_Import.InsertAMLMasterdataImportWork("AML_HQ_Work", dtAMLHQWORKImport);
                if (isInsertCdata == false)
                {
                    JobHelper.Write(this.jobID, "DB AML_HQ_Work 寫入失敗 ", LogState.Info);
                    Logging.Log("DB AML_HQ_Work 寫入失敗", LogLayer.DB);
                }
                    
            }

            // 還原資料
            if (!isInsertCdata)
            {
                BRAML_File_Import.RecoveryBranchData3(fileNameDat2, fileNameDat);
            }
        }

        JobHelper.Write(this.jobID, "資料庫寫檔結束！", LogState.Info);

        return isInsertCdata;
    }

    // 寫入資料庫
    public bool SetRelationDataTable(DataTable sourceDat, string fileNameDat)
    {
        bool isInsertCdata = false;

        // 組 AML_IMP_LOG 資料
        DataTable dtAMLIMPLOG = SetAMLIMPLOGData(sourceDat, fileNameDat);

        // 組 AML_BRCH_Import 資料
        DataTable dtAMLCdataImport = SetAMLBranchdataImportData(sourceDat, fileNameDat);

        // 組 AML_BRCH_Work 資料
        DataTable dtAMLBRCHdataworkImport = SetAMLBRCHdataworkImportData(sourceDat, fileNameDat);


        int masterID = BRAML_File_Import.InsertAMLIMPLOG(dtAMLIMPLOG);

        JobHelper.Write(this.jobID, "資料庫寫檔開始！ masterID = " + masterID, LogState.Info);

        if (masterID > 0)
        {
            // 寫入 AML_BRCH_Import
            isInsertCdata = BRAML_File_Import.InsertAMLBranchdataImport("AML_BRCH_Import", dtAMLCdataImport);
            if (isInsertCdata == false)
            {
                JobHelper.Write(this.jobID, "DB AML_BRCH_Import寫入失敗 ", LogState.Info);
                Logging.Log("DB AML_BRCH_Import寫入失敗", LogLayer.DB);
            }
                

            if (isInsertCdata)
            {
                // 寫入 AML_BRCH_Work
                isInsertCdata = BRAML_File_Import.InsertAMLBranchdataImport("AML_BRCH_Work", dtAMLBRCHdataworkImport);
                if (isInsertCdata == false)
                {
                    JobHelper.Write(this.jobID, "DB AML_BRCH_Work寫入失敗 ", LogState.Info);
                    Logging.Log("DB AML_BRCH_Work寫入失敗", LogLayer.DB);
                }
                    
            }

            // 還原資料
            if (!isInsertCdata)
            {
                BRAML_File_Import.RecoveryBranchData(fileNameDat);
            }
        }

        JobHelper.Write(this.jobID, "資料庫寫檔結束！", LogState.Info);

        return isInsertCdata;
    }

    // 設定 AML_Branchdata_Import 表頭
    private DataTable SetAMLBranchdataImportHeader()
    {
        //todo
        DataTable result = new DataTable();

        result.Columns.Add("CASE_NO", typeof(System.String));
        result.Columns.Add("FileName", typeof(System.String));
        result.Columns.Add("BRCH_BATCH_NO", typeof(System.String));
        result.Columns.Add("BRCH_INTER_ID", typeof(System.String));
        result.Columns.Add("BRCH_SIXM_TOT_AMT", typeof(System.String));
        result.Columns.Add("BRCH_MON_AMT1", typeof(System.String));
        result.Columns.Add("BRCH_MON_AMT2", typeof(System.String));
        result.Columns.Add("BRCH_MON_AMT3", typeof(System.String));
        result.Columns.Add("BRCH_MON_AMT4", typeof(System.String));
        result.Columns.Add("BRCH_MON_AMT5", typeof(System.String));
        result.Columns.Add("BRCH_MON_AMT6", typeof(System.String));
        result.Columns.Add("BRCH_MON_AMT7", typeof(System.String));
        result.Columns.Add("BRCH_MON_AMT8", typeof(System.String));
        result.Columns.Add("BRCH_MON_AMT9", typeof(System.String));
        result.Columns.Add("BRCH_MON_AMT10", typeof(System.String));
        result.Columns.Add("BRCH_MON_AMT11", typeof(System.String));
        result.Columns.Add("BRCH_MON_AMT12", typeof(System.String));
        result.Columns.Add("BRCH_KEY", typeof(System.String));
        result.Columns.Add("BRCH_BRCH_NO", typeof(System.String));
        result.Columns.Add("BRCH_BRCH_SEQ", typeof(System.String));
        result.Columns.Add("BRCH_BRCH_TYPE", typeof(System.String));
        result.Columns.Add("BRCH_NATION", typeof(System.String));
        result.Columns.Add("BRCH_BIRTH_DATE", typeof(System.String));
        result.Columns.Add("BRCH_PERM_CITY", typeof(System.String));
        result.Columns.Add("BRCH_PERM_ADDR1", typeof(System.String));
        result.Columns.Add("BRCH_PERM_ADDR2", typeof(System.String));
        result.Columns.Add("BRCH_CHINESE_NAME", typeof(System.String));
        result.Columns.Add("BRCH_ENGLISH_NAME", typeof(System.String));
        result.Columns.Add("BRCH_ID", typeof(System.String));
        result.Columns.Add("BRCH_OWNER_ID_ISSUE_DATE", typeof(System.String));
        result.Columns.Add("BRCH_OWNER_ID_ISSUE_PLACE", typeof(System.String));
        result.Columns.Add("BRCH_OWNER_ID_REPLACE_TYPE", typeof(System.String));
        result.Columns.Add("BRCH_ID_PHOTO_FLAG", typeof(System.String));
        result.Columns.Add("BRCH_PASSPORT", typeof(System.String));
        result.Columns.Add("BRCH_PASSPORT_EXP_DATE", typeof(System.String));
        result.Columns.Add("BRCH_RESIDENT_NO", typeof(System.String));
        result.Columns.Add("BRCH_RESIDENT_EXP_DATE", typeof(System.String));
        result.Columns.Add("BRCH_OTHER_CERT", typeof(System.String));
        result.Columns.Add("BRCH_OTHER_CERT_EXP_DATE", typeof(System.String));
        result.Columns.Add("BRCH_COMP_TEL", typeof(System.String));
        result.Columns.Add("BRCH_CREATE_DATE", typeof(System.String));
        result.Columns.Add("BRCH_STATUS", typeof(System.String));
        result.Columns.Add("BRCH_CIRCULATE_MERCH", typeof(System.String));
        result.Columns.Add("BRCH_HQ_BRCH_NO", typeof(System.String));
        result.Columns.Add("BRCH_HQ_BRCH_SEQ_NO", typeof(System.String));
        result.Columns.Add("BRCH_UPDATE_DATE", typeof(System.String));
        result.Columns.Add("BRCH_QUALIFY_FLAG", typeof(System.String));
        result.Columns.Add("BRCH_UPDATE_ID", typeof(System.String));
        result.Columns.Add("BRCH_REAL_CORP", typeof(System.String));
        result.Columns.Add("BRCH_RESERVED_FILLER", typeof(System.String));
        result.Columns.Add("Create_Time", typeof(System.String));
        result.Columns.Add("Create_User", typeof(System.String));
        result.Columns.Add("Create_Date", typeof(System.String));

        //長姓名
        result.Columns.Add("BRCH_CHINESE_LNAME", typeof(System.String));
        result.Columns.Add("BRCH_ROMA", typeof(System.String));

        //20201029-202012RC 增加分公司設立日期
        result.Columns.Add("BRCH_BUILD_DATE", typeof(System.String));

        //20210804 EOS_AML(NOVA) 自然人收單批次 by Ares Dennis
        result.Columns.Add("BRCH_NATU_ID", typeof(System.String));//自然人ID
        result.Columns.Add("LAST_UPD_MAKER", typeof(System.String));//資料最後異動MAKER
        result.Columns.Add("LAST_UPD_CHECKER", typeof(System.String));//資料最後異動CHECKER
        result.Columns.Add("LAST_UPD_BRANCH", typeof(System.String));//資料最後異動分行
        result.Columns.Add("LAST_UPD_DATE", typeof(System.String));//資料最後異動日期 

        //20211007_Ares_Jack_新增欄位
        result.Columns.Add("BRCH_REG_ENG_NAME", typeof(System.String));//分公司登記英文
        result.Columns.Add("BRCH_REG_CHI_NAME", typeof(System.String));//分公司登記中文

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

    /// <summary>
    /// 依ID 查詢對應的長姓名資料
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private EntityHTG_JC68 getJc68Data(string id, string HCOP_KEY)
    {
        EntityHTG_JC68 jc68Data = new EntityHTG_JC68();
        using (BRHTG_JC68 obj = new BRHTG_JC68("AMLBranchInformationService"))
        {
            jc68Data.ID = id;
            jc68Data = obj.getData(jc68Data, eAgentInfo);

            //若有查詢卻沒有資料, 就需要提示並補齊資料            
            if (jc68Data.Success == false || string.IsNullOrEmpty(jc68Data.LONG_NAME) == true)
            {
                if (NoLname_HCOP.Contains(HCOP_KEY) == false)
                {
                    NoLname_HCOP.Add(HCOP_KEY);
                }
            }
        }
        return jc68Data;
    }

    /// <summary>
    /// 取得對應ID和實際查詢ID:依判斷原則(身份證字號-> 護照號碼->居留證號碼): ID|A123456789
    /// </summary>
    /// <param name="id"></param>
    /// <param name="passport"></param>
    /// <param name="residentNo">居留證號碼</param>
    /// <returns>type|ID</returns>
    private string getJC68ID(string id, string passport, string residentNo, string HCOP_KEY)
    {
        string _result = string.Empty;
        string ID, PASSPORT, RESIDENT_NO;
        ID = id ?? "";
        PASSPORT = passport ?? "";
        RESIDENT_NO = residentNo ?? "";
        string _strFormat = "{0}|{1}"; //用| 區分

        if (string.IsNullOrEmpty(ID) == false)
        {
            _result = string.Format(_strFormat, "ID", ID);
        }
        else if (string.IsNullOrEmpty(PASSPORT))
        {
            _result = string.Format(_strFormat, "PASSPORT", PASSPORT);
        }
        else if (string.IsNullOrEmpty(residentNo) == false)
        {
            _result = string.Format(_strFormat, "ResidentNo", RESIDENT_NO);
        }
        //再加入 統編+流水號, 好在查不到對應資料時,要加 LOG 提示哪個公司裡可能會有查不到的長姓名
        _result = string.Format(_strFormat, _result, HCOP_KEY);
        return _result;
    }

    public string[] getNoLnameArray(string type)
    {
        switch (type)
        {
            case "HCOP": //總公司
                return NoLname_HCOP.ToArray();                
            case "BRCH": //分公司
                return NoLname_BRCH.ToArray();
            default:
                string[] result= new string[0];
                return result;
        }        
    }

    private void setKEYByNoLname(string type,string key)
    {
        switch (type)
        {
            case "HCOP": //總公司
                if (NoLname_HCOP.Contains(key) == false)
                {
                    NoLname_HCOP.Add(key);
                }

                break;
            case "BRCH": //分公司
                if (NoLname_BRCH.Contains(key) == false)
                {
                    NoLname_BRCH.Add(key);
                }
                break;            
        }        
    }

    /// <summary>
    /// 修改對應ID的長姓名值
    /// </summary>
    /// <param name="fileName">只針對指定檔名的做長姓名資料回壓,若為空白時,只要符合HCOP_KEY和ID時就會更新</param>
    public void SetLname(string fileName)
    {
        DataTable tblFileInfo = BRAML_File_Import.GetFileInfo(jobID);

        string isSendHTG = string.Empty;
        if (tblFileInfo.Rows[0]["IsSendHTG"] == null || tblFileInfo.Rows[0]["IsSendHTG"].ToString() != "Y")
        {
            JobHelper.Write(this.jobID, "   tbl_FileInfo 設定為不查詢電文！", LogState.Info);
        }
        else {
            BRHTG_JC68 obj = new BRHTG_JC68("AMLBranchInformationService");
            Hashtable HtPars = new Hashtable();

            string brchFileName = string.Empty;
            //若沒給檔名時就不用轉換, 這樣後面才不會加入成 where 條件
            if (string.IsNullOrEmpty(fileName) == false)
            {
                brchFileName = fileName.Replace("HCOP", "BRCH");
            }

            //總公司-負責人
            HtPars.Add("tableName", "AML_HQ_Work");
            HtPars.Add("searchKeyName", "HCOP_KEY");

            HtPars.Add("ID", "HCOP_OWNER_ID");
            HtPars.Add("PASSPORT", "HCOP_PASSPORT");
            HtPars.Add("RESIDENTNO", "HCOP_RESIDENT_NO");
            HtPars.Add("LNAME", "HCOP_OWNER_CHINESE_LNAME");
            HtPars.Add("ROMA", "HCOP_OWNER_ROMA");
            foreach (string item in LNameID_HQ_owner)
            {
                setLnameByOne(ref obj, fileName, item, HtPars);
            }

            //總公司-聯絡人
            //聯絡人是用統編+流水號, 所以不用加入查詢欄位, 只要用 HCOP_KEY 就可以
            HtPars.Clear();
            HtPars.Add("tableName", "AML_HQ_Work");
            HtPars.Add("searchKeyName", "HCOP_KEY");

            HtPars.Add("LNAME", "HCOP_CONTACT_LNAME");
            HtPars.Add("ROMA", "HCOP_CONTACT_ROMA");
            foreach (string item in LNameID_HQ_contact)
            {
                setLnameByOne(ref obj, fileName, item, HtPars);
            }

            //分公司(需注意檔名有差異)        
            HtPars.Clear();
            HtPars.Add("tableName", "AML_BRCH_Work");
            HtPars.Add("searchKeyName", "BRCH_KEY");

            HtPars.Add("ID", "BRCH_ID");
            HtPars.Add("PASSPORT", "BRCH_PASSPORT");
            HtPars.Add("RESIDENTNO", "BRCH_RESIDENT_NO");
            HtPars.Add("LNAME", "BRCH_CHINESE_LNAME");
            HtPars.Add("ROMA", "BRCH_ROMA");
            foreach (string item in LNameID_BRCH)
            {
                //(需注意檔名有差異=brchFileName)
                setLnameByOne(ref obj, brchFileName, item, HtPars);
            }

            //高管
            HtPars.Clear();
            HtPars.Add("tableName", "AML_HQ_Manager_Work");
            HtPars.Add("searchKeyName", "rtrim(HCOP_BATCH_NO)+rtrim(HCOP_INTER_ID)"); //可再考慮用 HCOP_KEY 

            HtPars.Add("ID", "HCOP_BENE_ID");
            HtPars.Add("PASSPORT", "HCOP_BENE_PASSPORT");
            HtPars.Add("RESIDENTNO", "HCOP_BENE_RESIDENT_NO");
            HtPars.Add("LNAME", "HCOP_BENE_LNAME");
            HtPars.Add("ROMA", "HCOP_BENE_ROMA");
            foreach (string item in LNameID_Manager)
            {
                setLnameByOne(ref obj, fileName, item, HtPars);
            }
        }
    }

    private void setLnameByOne(ref BRHTG_JC68 obj, string fileName, string inputStr, Hashtable defHT)
    {
        List<string> _pars = new List<string>();
        string[] tmpArr = inputStr.Split('|'); //ex: PASSPORT|A1234567890|235515440000
        EntityHTG_JC68 jc68Data = new EntityHTG_JC68();
        string IDType = tmpArr[0]; // ID | PASSPORT | RESIDENTNO
        jc68Data.ID = tmpArr[1];// id;
        string HCOP_KEY = tmpArr[2];

        //需再判斷 HT["LNAME"], HT["ROMA"] 是否有對應的KEY? 若沒有就不執行,因為無法異動指定的欄位
        if (defHT.ContainsKey("tableName") == false || defHT.ContainsKey("searchKeyName") == false || defHT.ContainsKey("LNAME") == false || defHT.ContainsKey("ROMA") == false)
        {
            JobHelper.Write(this.jobID, string.Format("  無定義對應欄位,請檢視代入的 defHT!!"));
            return;
        }

        jc68Data = obj.getData(jc68Data, eAgentInfo);
        //若有查詢卻沒有資料, 就需要提示補齊資料
        if (string.IsNullOrEmpty(jc68Data.LONG_NAME) == true)
        {
            //依照檔名決定要放哪個陣列裡
            string type = fileName.IndexOf("HCOP") > -1 ? "HCOP" : "BRCH";
            setKEYByNoLname(type, HCOP_KEY);
            return;
        }

        if (string.IsNullOrEmpty(fileName) == false)
        {
            _pars.Add("[FileName]=@FileName");
        }

        //原則上查詢ID的條件只會用一個(ID or Passport or residentno )
        if (defHT.ContainsKey(IDType.ToUpper()))
        {
            _pars.Add(string.Format("{0}=@ID", defHT[IDType]));
        }

        //有查詢成功時才需修改
        if (jc68Data.MESSAGE_TYPE == "0000" || jc68Data.MESSAGE_TYPE == "0001")
        {
            SqlCommand sqlcmd = new SqlCommand();
            string strSQL = string.Empty;
            strSQL = string.Format("update [dbo].[{0}] ", defHT["tableName"]);
            strSQL += string.Format(" set [{0}]=@LNAME,[{1}]=@ROMA ", defHT["LNAME"], defHT["ROMA"]);
            strSQL += string.Format(" where {0}=@HCOP_KEY ", defHT["searchKeyName"]);
            //合併查詢條件(ex: "[FileName]=@FileName  and HCOP_BENE_ID=@ID" )
            strSQL = string.Format("{0} and {1}", strSQL, string.Join(" and ", _pars.ToArray()));
            //增加 Import 語法
            strSQL = string.Format("{0};{1}", strSQL, strSQL.Replace("_Work", "_Import"));

            sqlcmd.CommandText = strSQL;
            sqlcmd.CommandType = CommandType.Text;

            sqlcmd.Parameters.Add(new SqlParameter("@FileName", fileName));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_KEY", HCOP_KEY));
            sqlcmd.Parameters.Add(new SqlParameter("@ID", jc68Data.ID));
            sqlcmd.Parameters.Add(new SqlParameter("@LNAME", jc68Data.LONG_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@ROMA", jc68Data.PINYIN_NAME));
            try
            {
                DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    //result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }
        }
        //return sqlcmd;
    }

}
