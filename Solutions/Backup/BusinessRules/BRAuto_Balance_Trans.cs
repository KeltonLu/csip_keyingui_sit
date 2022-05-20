using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using System.Net;
using System.Web.UI;
using System.Reflection;

using CSIPKeyInGUI.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.BusinessRules;

using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using Framework.Common.Utility;
using Framework.Common.Cryptography;
using Framework.Common;
using Framework.Common.Logging;
using Framework.Common.IO;

using Microsoft.Office.Interop.Excel;
using ExcelApplication = Microsoft.Office.Interop.Excel.ApplicationClass;


namespace CSIPKeyInGUI.BusinessRules
{
    public class BRAuto_Balance_Trans : CSIPCommonModel.BusinessRules.BRBase<EntityAuto_Balance_Trans>
    {
        public static DataSet GetDataFromtblAuto_Balance_Trans(string date)
        {
            string strSql = @"select * from dbo.Balance_Trans
                            where CONVERT(varchar,Trans_Date,111) = CONVERT(varchar,@date,111)
                            and Upload_Flag = 'N' and Process_Flag = 'N' ";

            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandType = CommandType.Text;
            sqlComm.CommandText = strSql;

            SqlParameter spCD = new SqlParameter("date", date);
            sqlComm.Parameters.Add(spCD);
            return SearchOnDataSet(sqlComm);
        }

        public static DataSet SearchDataMatch(string CARD_ID, string PID, string UPLOAD_DATE)
        {
            string strSql = @"select * from dbo.Balance_Trans
                            where CardNo = @Card_Id and PID = @PID 
                            and CONVERT(varchar,Trans_Date,111) = CONVERT(varchar,@UPLOAD_DATE,111)
                            and Process_Flag <> 'D' ";

            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandType = CommandType.Text;
            sqlComm.CommandText = strSql;

            SqlParameter spCD = new SqlParameter("Card_Id", CARD_ID);
            sqlComm.Parameters.Add(spCD);
            SqlParameter spPI = new SqlParameter("PID", PID);
            sqlComm.Parameters.Add(spPI);
            SqlParameter spSC = new SqlParameter("UPLOAD_DATE", UPLOAD_DATE);
            sqlComm.Parameters.Add(spSC);

            return SearchOnDataSet(sqlComm);

        }

        public static bool UpdateModifyTimeAndFalg(string jobID, string parms)
        {
            string strSql = @"update dbo.Balance_Trans set Modify_DateTime = GETDATE(),Upload_Flag = 'Y',
                            Modify_User = @jobID
                            where CONVERT(varchar,Trans_Date,112) = CONVERT(varchar,@date,111)
                            and Process_Flag = 'N'";

            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandType = CommandType.Text;
            sqlComm.CommandText = strSql;
            sqlComm.Parameters.Add(new SqlParameter("@jobID", jobID));
            sqlComm.Parameters.Add(new SqlParameter("@date", parms));

            return Update(sqlComm);
        }

        public static bool UpdateRtnBalanceTrans(string CARD_ID, string PID, string Fail_Reason, string UPLOAD_DATE,
            string jobID)
        {
            string strSql = @"update dbo.Balance_Trans set Process_Flag = 'E',Process_Note = @Fail_Reason,
                            Modify_DateTime = GETDATE(),Modify_User = @jobID 
                            where CONVERT(varchar,Trans_Date,111) = CONVERT(varchar,@UPLOAD_DATE,111) 
                            and CardNo = @Card_Id and PID = @PID and Process_Flag <> 'D' ";

            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandType = CommandType.Text;
            sqlComm.CommandText = strSql;

            SqlParameter spCD = new SqlParameter("Card_Id", CARD_ID);
            sqlComm.Parameters.Add(spCD);
            SqlParameter spPI = new SqlParameter("PID", PID);
            sqlComm.Parameters.Add(spPI);
            SqlParameter spDT = new SqlParameter("UPLOAD_DATE", UPLOAD_DATE);
            sqlComm.Parameters.Add(spDT);
            SqlParameter spFR = new SqlParameter("Fail_Reason", Fail_Reason);
            sqlComm.Parameters.Add(spFR);
            SqlParameter spjb = new SqlParameter("jobID", jobID);
            sqlComm.Parameters.Add(spjb);

            return Update(sqlComm);
        }

        public bool UpdateParameter(string jobID)
        {
            string strSql = @"update dbo.tbl_FileInfo set Parameter = '' where Job_ID = @jobID ";

            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandType = CommandType.Text;
            sqlComm.CommandText = strSql;

            SqlParameter spjb = new SqlParameter("jobID", jobID);
            sqlComm.Parameters.Add(spjb);

            return Update(sqlComm);
        }

        public static bool UpdateProcessFlag(string UPLOAD_DATE, string jobID, string isY, ref int s_cnt)
        {
            string strSql = "";
            bool check = false;
            SqlConnection sql_conn = new SqlConnection(
                System.Web.Configuration.WebConfigurationManager.ConnectionStrings["Connection_System"].ToString());

            if (isY == "Y")
            {
                strSql = @"update dbo.Balance_Trans set Process_Flag = 'Y',Modify_DateTime = GETDATE(),
                            Modify_User = @jobID,Process_Note = '' 
                            where CONVERT(varchar,Trans_Date,111) = CONVERT(varchar,@UPLOAD_DATE,111)
                            and Process_Flag <> 'D' ";
            }
            else
            {
                strSql = @"update dbo.Balance_Trans set Process_Flag = 'N',Modify_DateTime = GETDATE(),
                            Modify_User = @jobID 
                            where CONVERT(varchar,Trans_Date,111) = CONVERT(varchar,@UPLOAD_DATE,111)";
            }

            SqlCommand sqlComm = new SqlCommand();
            //sqlComm.CommandType = CommandType.Text;
            //sqlComm.CommandText = strSql;
            sqlComm = new SqlCommand(strSql, sql_conn);

            SqlParameter spDT = new SqlParameter("UPLOAD_DATE", UPLOAD_DATE);
            sqlComm.Parameters.Add(spDT);
            SqlParameter spFR = new SqlParameter("jobID", jobID);
            sqlComm.Parameters.Add(spFR);

            try
            {
                sql_conn.Open();
                s_cnt = sqlComm.ExecuteNonQuery();
                sql_conn.Close();
                check = true;
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                check = false;
            }

            //return Update(sqlComm);
            return check;
        }

        public static bool BatchOutput(System.Data.DataTable dt)
        {
            bool check = false;
            string code = "0824406C";
            string strTXT = "";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //表頭”H”+餘額轉置日期(YYYYMMDD)+” 0824406C”( 成本中心代號設為參數)
                if (i == 0)
                {
                    strTXT += string.Format("H{0}{1}{2}", DateTime.Parse(dt.Rows[0]["Trans_Date"].ToString()).ToString("yyyyMMdd"), code,
                            Environment.NewLine);

                }

                // 卡號16碼+PID16碼+原因碼1碼
                strTXT += string.Format("{0}{1}{2}{3}",
                    dt.Rows[i]["CardNo"].ToString().Trim().ToUpper().PadRight(16, ' '),
                    dt.Rows[i]["PID"].ToString().Trim().ToUpper().PadRight(16, ' '),
                    dt.Rows[i]["Reason_Code"].ToString().Trim().PadRight(1, ' '), Environment.NewLine);
            }
            strTXT += "EOF";
            //strTXT += "\r\n";

            string strPah = AppDomain.CurrentDomain.BaseDirectory + "\\Page\\" +
                 UtilHelper.GetAppSettings("ExportExcelFilePath");  // +"O317.txt";
            BRExcel_File.CheckDirectory(ref strPah);
            strPah += @"\O317.txt";

            using (FileStream fs = File.Create(strPah, 1024))
            {
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                sw.WriteLine(strTXT);
                sw.Flush();
                sw.Close();
                check = true;
            }

            return check;
        }

        public static void BatchReturnOutput(System.Data.DataTable dt)
        {
            string strTxt = "";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                // 1码 Char(16) REPT-D-CARD-NO-H      卡號(左靠右補空白)
                strTxt += dt.Rows[i]["REPT-D-CARD-NO-H"].ToString().ToUpper().PadRight(16, ' ');

                // 17码 Char(2) FILLER      保留欄位空白(左靠右補空白)
                strTxt += dt.Rows[i]["FILLER"].ToString().ToUpper().PadRight(2, ' ');

                // 19码 Char(16) REPT-D-PURSE-ID-H      PID(左靠右補空白)
                strTxt += dt.Rows[i]["REPT-D-PURSE-ID-H"].ToString().ToUpper().PadRight(16, ' ');

                // 35码 Char(2) FILLER      保留欄位空白(左靠右補空白)
                strTxt += dt.Rows[i]["FILLER"].ToString().ToUpper().PadRight(2, ' ');

                // 37码 Char(24) REPT-D-REASON-H      失敗原因(左靠右補空白)
                strTxt += dt.Rows[i]["REPT-D-REASON-H"].ToString().ToUpper().PadRight(24, ' ');

                // 61码 Char(40) FILLER      保留欄位空白(左靠右補空白)
                strTxt += dt.Rows[i]["FILLER"].ToString().ToUpper().PadRight(40, ' ');

                strTxt += "\r\n";

                // 1码 Char(16) REPT-D-CARD-NO      卡號(左靠右補空白)
                strTxt += dt.Rows[i]["REPT-D-CARD-NO"].ToString().ToUpper().PadRight(16, ' ');

                // 17码 Char(2) FILLER      保留欄位空白(左靠右補空白)
                strTxt += dt.Rows[i]["FILLER"].ToString().ToUpper().PadRight(2, ' ');

                // 19码 Char(16) REPT-D-PURSE-ID      PID(左靠右補空白)
                strTxt += dt.Rows[i]["REPT-D-PURSE-ID"].ToString().ToUpper().PadRight(16, ' ');

                // 35码 Char(2) FILLER      保留欄位空白(左靠右補空白)
                strTxt += dt.Rows[i]["FILLER"].ToString().ToUpper().PadRight(2, ' ');

                // 37码 Char(24) REPT-D-REASON      失敗原因(左靠右補空白)
                strTxt += dt.Rows[i]["REPT-D-REASON"].ToString().ToUpper().PadRight(24, ' ');

                // 61码 Char(40) FILLER      保留欄位空白(左靠右補空白)
                strTxt += dt.Rows[i]["FILLER"].ToString().ToUpper().PadRight(40, ' ');

                strTxt += "\r\n";

            }

            //固定為： 上傳日期 :
            strTxt += dt.Rows[0]["REPT-UPLOAD-DATE-CH"].ToString().ToUpper().PadRight(11, ' ');

            //YYYY
            strTxt += dt.Rows[0]["REPT-UPLOAD-YY"].ToString().ToUpper().PadRight(4, ' ');

            //MM
            strTxt += dt.Rows[0]["REPT-UPLOAD-MM"].ToString().ToUpper().PadRight(2, ' ');

            //DD
            strTxt += dt.Rows[0]["REPT-UPLOAD-DD"].ToString().ToUpper().PadRight(2, ' ');

            //固定顯示【 上傳筆數 :】
            strTxt += dt.Rows[0]["REPT-UPLOAD-CNT-CH"].ToString().ToUpper().PadRight(11, ' ');

            //上傳筆數值
            strTxt += dt.Rows[0]["REPT-UPLOAD-CNT"].ToString().ToUpper().PadRight(6, ' ');

            //固定顯示【 成功筆數 :】
            strTxt += dt.Rows[0]["REPT-SUCC-CNT-CH"].ToString().ToUpper().PadRight(11, ' ');

            //成功筆數值
            strTxt += dt.Rows[0]["REPT-SUCC-CNT"].ToString().ToUpper().PadRight(6, ' ');

            //固定顯示【 失敗筆數 :】
            strTxt += dt.Rows[0]["REPT-FAIL-CNT-CH"].ToString().ToUpper().PadRight(11, ' ');

            //失敗筆數值
            strTxt += dt.Rows[0]["REPT-FAIL-CNT"].ToString().ToUpper().PadRight(6, ' ');

            //固定顯示【 上傳單位】
            strTxt += dt.Rows[0]["REPT-UPLOAD-DEPT-CH"].ToString().ToUpper().PadRight(11, ' ');

            //上傳單位代碼
            strTxt += dt.Rows[0]["REPT-UPLOAD-DEPT"].ToString().ToUpper().PadRight(16, ' ');

            // Char(3) FILLER      保留欄位空白(左靠右補空白)
            strTxt += dt.Rows[0]["FILLER"].ToString().ToUpper().PadRight(3, ' ');

            //strTXT += "EOF";
            //strTXT += "\r\n";

            string strPah = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "O318.txt";
            if (File.Exists(strPah))
            {
                File.Delete(strPah);
            }

            using (FileStream fs = File.Create(strPah, 1024))
            {
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                sw.WriteLine(strTxt);
                sw.Flush();
                sw.Close();
            }

        }

        public static System.Data.DataTable GetTable()
        {
            //
            // Here we create a DataTable with four columns.
            //
            System.Data.DataTable table = new System.Data.DataTable();
            table.Columns.Add("Data1", typeof(string));
            table.Columns.Add("Data2", typeof(string));
            table.Columns.Add("Data3", typeof(string));

            return table;
        }

        public DataSet GetFTPinfo(string jobID)
        {
            string strSql_GetLastSuccessDate = @"select * from dbo.tbl_FileInfo where job_ID = @jobID";

            SqlCommand sqlComm = new SqlCommand();

            sqlComm.CommandText = strSql_GetLastSuccessDate;
            sqlComm.CommandType = CommandType.Text;

            SqlParameter parmFUNCTION_KEY = new SqlParameter("@jobID", jobID);
            sqlComm.Parameters.Add(parmFUNCTION_KEY);

            DataSet ds = null;

            ds = BRL_BATCH_LOG.SearchOnDataSet(sqlComm);

            if (ds != null)
                return ds;
            else
                return null;
        }

        public bool FTPDownload(string jobID, string parameter, DataSet dstProperty,ref string msg)
        {
            bool check = false;

            if (dstProperty != null && dstProperty.Tables[0].Rows.Count > 0)
            {
                string remoteHost = dstProperty.Tables[0].Rows[0]["FtpIP"].ToString().Trim();   // 127.0.0.1
                string remotePath = dstProperty.Tables[0].Rows[0]["FtpPath"].ToString().Trim(); // Common//RptTemplet//
                string remoteUser = dstProperty.Tables[0].Rows[0]["FtpUserName"].ToString().Trim();
                string remotePass = RedirectHelper.GetDecryptString(dstProperty.Tables[0].Rows[0]["FtpPwd"].ToString().Trim());
                string remoteFile = "";

                if (string.IsNullOrEmpty(parameter))
                    //remoteFile = string.Format("{0}{1}{2}", dstProperty.Tables[0].Rows[0]["FtpFileName"].ToString().Trim(),DateTime.Now.Month, DateTime.Now.Day); // O318MMdd
                    remoteFile = string.Format("{0}{1}", dstProperty.Tables[0].Rows[0]["FtpFileName"].ToString().Trim(), String.Format("{0:MMdd}", System.DateTime.Now)); // O318MMdd
                else
                    remoteFile = string.Format("{0}{1}{2}", dstProperty.Tables[0].Rows[0]["FtpFileName"].ToString().Trim(),
                      parameter.Substring(4, 2), parameter.Substring(6, 2)); // O318MMdd

                if (dstProperty.Tables[0].Rows[0]["ZipPwd"] == null ||
                    string.IsNullOrEmpty(dstProperty.Tables[0].Rows[0]["ZipPwd"].ToString()))
                    remoteFile += ".TXT";
                else
                    remoteFile += ".EXE";

                WebClient request = new WebClient();
                request.Credentials = new NetworkCredential(remoteUser, remotePass);
                if (remotePath.Substring(remotePath.Length - 1) != "/")
                    remotePath += "/";
                try
                {
                    byte[] fildData = request.DownloadData(string.Format("ftp://{0}/{1}{2}", remoteHost, remotePath,
                        remoteFile));
                    //FileStream file = File.Create(@"d:\temp" + "O318.txt");
                    string path = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory,
                        UtilHelper.GetAppSettings("UpLoadFilePath");
                    FileStream file = File.Create(string.Format("{0}\\{1}", path, remoteFile));
                    file.Write(fildData, 0, fildData.Length);
                    file.Close();
                    check = true;
                }
                catch(Exception ex)
                {
                    check = false;
                    string[] ex1 = ex.ToString().Split(':');
                    string[] ex2 = ex1[2].Split('。');
                    msg = string.Format("{0}{1}", ex1[1], ex2[0]);
                    Logging.SaveLog(ELogLayer.Util, "FTP下載失敗:" + ex.ToString(), ELogType.Error);
                }
            }
            return check;
        }

        public bool CheckData0318(string FilePath, string FileName, string jobID, string zipPW, ref int cnt,
            ref int s_cnt,ref string msg)
        {


            

            if (FilePath.Substring(FilePath.Length - 1) != "\\")
                FilePath += "\\";

            int ZipCount = 0;
            bool check = false;

            try
            {
                bool blnResult = true;
                string[] subName = FileName.Split('.');

                if (subName[1].ToLower() == "zip" || subName[1].ToLower() == "exe")
                {
                    //blnResult = ZipExeFile(FilePath, FilePath + FileName, FileName, zipPW, ref ZipCount);
                    blnResult = ZipExeFile(FilePath, FileName, zipPW);
                }
                    
                

                if (blnResult)
                {
                    string[] file = FileName.Split('.');
                    FileName = string.Format("{0}{1}.TXT", FilePath, file[0]);
                    //File.Copy(FilePath + file[0], FileName, true);

                    if (File.Exists(FileName))
                    {
                        //File.Delete(FilePath + file[0]);
                        using (StreamReader sr = new StreamReader(FileName, System.Text.Encoding.Default))
                        {
                            DataSet ds = new DataSet();
                            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                            string line = sr.ReadToEnd();

                            if (line != null)
                            {
                                string[] str = line.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                                if (str.Length > 1)
                                {
                                    //string transDate = str[str.Length - 2].Substring(9, 8);
                                    string[] _linedata = str[str.Length-1].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                                    string transDate = _linedata[1].Substring(0, 8);
                                    transDate = string.Format("{0}/{1}/{2}", transDate.Substring(0, 4),transDate.Substring(4, 2), transDate.Substring(6, 2));

                                    #region 以檔案內的上傳日期為key值,逐一將失敗的案件更新至資料庫中,
                                    // 處理狀態=E(失敗), 及主機處理註記(ProcessNote),
                                    // 再以上傳日期為key值更新其他資料(排除己刪除的資料)處理狀態=Y(成功)
                                    if (UpdateProcessFlag(transDate, jobID, "Y", ref s_cnt))
                                    {
                                        for (int i = 0; i < str.Length - 2; i++)
                                        {
                                            string[] words = str[i].Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                                            ds = SearchDataMatch(words[0], words[1], transDate);

                                            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                                            {
                                                // words[0]=卡號, words[1]=PID, words[2]=主機處理註記
                                                UpdateRtnBalanceTrans(words[0], words[1], words[2], transDate, jobID);
                                                cnt++;
                                            }

                                        }
                                        check = true;
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                    else
                        msg = string.Format("查無檔案:{0}.", FileName);
                }
            }
            catch (Exception ex)
            {
                check = false;
                msg += string.Format("讀{0}檔失敗", FileName);
                Logging.SaveLog(ELogLayer.Util, "讀txt檔失敗：" + ex.ToString(), ELogType.Error);
            }
            return check;
        }

        public bool ZipExeFile(string destFolder, string srcZipFile, string fileName,string password, ref int ZipCount)
        {
            if (string.IsNullOrEmpty(destFolder) || string.IsNullOrEmpty(srcZipFile))
                return false;

            if (destFolder.Substring(destFolder.Length - 1) != "\\")
                destFolder += "\\";

            try
            {
                string[] subName = srcZipFile.Split('.');

                if (subName[1].ToLower() == "zip")
                    ZipCount = CompressToZip.Unzip(destFolder, srcZipFile, password);

                if (subName[1].ToLower() == "exe")
                {
                    // arj解縮縮指令
                    string arg = string.Format("-g{0} -y {1}", password, destFolder);
                                        
                    if (CompressToZip.ZipExeFile(destFolder, fileName, arg))
                        ZipCount = 1;
                    else
                        ZipCount = 0;
                }

                if (ZipCount > 0)
                {
                    //*解壓成功刪除壓縮文檔
                    //File.Delete(srcZipFile);
                    //SaveLog(Resources.JobResource.Job0000001);
                    return true;
                }
                else
                {
                    //this.SaveLog(Resources.JobResource.Job0000002);
                    return false;
                }
            }
            catch (Exception exp)
            {
                //SaveLog(Resources.JobResource.Job0000003 + exp.Message);
                Logging.SaveLog(ELogLayer.Util, "解壓縮失敗:" + exp, ELogType.Error);

                return false;
            }
        }



        public bool ZipExeFile(string destFolder, string srcZipFile, string password)
        {
            string strTXTFileName = string.Empty;
            string strExeFileName = srcZipFile.Substring(0, srcZipFile.Trim().Length - 4);


            strTXTFileName = srcZipFile.Replace("EXE", "TXT");

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            //设定程序名
            p.StartInfo.FileName = "cmd.exe";
            //关闭Shell的使用
            p.StartInfo.UseShellExecute = false;
            //重定向标准输入
            p.StartInfo.RedirectStandardInput = true;
            //重定向标准输出
            p.StartInfo.RedirectStandardOutput = true;
            //设置不显示窗口
            p.StartInfo.CreateNoWindow = true;
            //执行VER命令
            p.Start();
            string strCommand1 = " " + destFolder + srcZipFile + " -g" + password + " -y " + destFolder;
            p.StandardInput.WriteLine(strCommand1);
            string strCommand2 = " ren " + destFolder + strExeFileName + " " + strTXTFileName;
            p.StandardInput.WriteLine(strCommand2);
            p.StandardInput.WriteLine("exit");
            p.WaitForExit(3000);
            p.Close();
            if (File.Exists(destFolder + strTXTFileName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UploadToFTP(string jobID)
        {
            bool isOk = false;
            DataSet dstProperty = GetFTPinfo(jobID);

            if (dstProperty != null && dstProperty.Tables[0].Rows.Count > 0)
            {
                string remoteHost = dstProperty.Tables[0].Rows[0]["FtpIP"].ToString().Trim();
                string remotePath = dstProperty.Tables[0].Rows[0]["FtpPath"].ToString().Trim();
                string remoteUser = dstProperty.Tables[0].Rows[0]["FtpUserName"].ToString().Trim();
                string remotePass = RedirectHelper.GetDecryptString(dstProperty.Tables[0].Rows[0]["FtpPwd"].ToString().Trim());
                string remoteFile = dstProperty.Tables[0].Rows[0]["FtpFileName"].ToString().Trim();

                // Get the object used to communicate with the server.
                if (remotePath.Substring(remotePath.Length - 1) != "/")
                    remotePath += "/";
                try
                {
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Format("ftp://{0}/{1}{2}.txt",
                    remoteHost, remotePath, remoteFile));
                    request.Method = WebRequestMethods.Ftp.UploadFile;

                    // This example assumes the FTP site uses anonymous logon.
                    request.Proxy = null;
                    request.Credentials = new NetworkCredential(remoteUser, remotePass);

                    // Copy the contents of the file to the request stream.
                    string strlocalFilePah = string.Format("{0}Page\\{1}", AppDomain.CurrentDomain.BaseDirectory,
                        UtilHelper.GetAppSettings("ExportExcelFilePath"));

                    BRExcel_File.CheckDirectory(ref strlocalFilePah);
                    strlocalFilePah += @"\O317.txt";
                    if (File.Exists(strlocalFilePah))
                    {
                        StreamReader sourceStream = new StreamReader(strlocalFilePah);
                        byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                        sourceStream.Close();
                        request.ContentLength = fileContents.Length;

                        Stream requestStream = request.GetRequestStream();
                        requestStream.Write(fileContents, 0, fileContents.Length);
                        requestStream.Close();

                        FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                        Logging.SaveLog(ELogLayer.Util, "FTP上傳完成, 狀態：" + response.StatusDescription, ELogType.Debug);
                        isOk = true;
                        response.Close();
                    }
                }
                catch (Exception ex)
                {
                    Logging.SaveLog(ELogLayer.Util, "FTP上傳失敗：" + ex.ToString(), ELogType.Debug);
                }
            }
            return isOk;
        }

        /// <summary>
        /// 取得JOB最後成功日期
        /// </summary>
        /// <param name="strFK">功能標識編號</param>
        /// <param name="strJOBID">JOB編號</param>
        /// <param name="strStatus">執行狀態(S)</param>
        /// <returns>DateTime</returns>
        public static DateTime GetLastSuccessDate(string strFK, string strJOBID, string strStatus)
        {
            //*取得JOB最後成功日期(排除JOB執行當天的成功紀錄)
            string strSql_GetLastSuccessDate = @"SELECT Max(END_TIME) FROM L_BATCH_LOG
                          where FUNCTION_KEY = @FUNCTION_KEY and JOB_ID = @JOB_ID and status = @STATUS 
                                and CONVERT(varchar(12), END_TIME, 112) <> CONVERT(varchar(12), getdate(), 112)";

            SqlCommand sqlComm = new SqlCommand();

            sqlComm.CommandText = strSql_GetLastSuccessDate;

            sqlComm.CommandType = CommandType.Text;

            SqlParameter parmFUNCTION_KEY = new SqlParameter("@FUNCTION_KEY", strFK);
            sqlComm.Parameters.Add(parmFUNCTION_KEY);
            SqlParameter parmJOB_ID = new SqlParameter("@JOB_ID", strJOBID);
            sqlComm.Parameters.Add(parmJOB_ID);
            SqlParameter parmSTATUS = new SqlParameter("@STATUS", strStatus);
            sqlComm.Parameters.Add(parmSTATUS);


            DataSet dstProperty = null;

            dstProperty = BRL_BATCH_LOG.SearchOnDataSet(sqlComm, "Connection_CSIP");

            if (dstProperty == null)
            {
                return DateTime.Now.AddDays(-1);
            }
            if (dstProperty.Tables[0].Rows[0][0] == DBNull.Value)
            {
                return DateTime.Now.AddDays(-1);
            }

            return Convert.ToDateTime(dstProperty.Tables[0].Rows[0][0]);
        }

        /// <summary>
        /// 用Entity方式插入資料庫
        /// </summary>
        /// <param name="eOtherBankTemp">Entity</param>
        /// <returns>true成功,false失敗</returns>
        public static bool AddEntity(EntityAuto_Balance_Trans eOtherBankTemp)
        {
            try
            {
                return eOtherBankTemp.DB_InsertEntity();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
