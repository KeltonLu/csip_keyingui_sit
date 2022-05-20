//******************************************************************
//*  功能說明：上傳 檔案至FTP，給偽冒系統及主機收取
//*  作    者：林家賜
//*  創建日期：2020/01/6
//*  修改紀錄：
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*******************************************************************

using Framework.Common.IO;
using Framework.Common.Utility;
using Framework.Data;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using CSIPKeyInGUI.BusinessRules;
using CSIPCommonModel.BusinessRules;
using Framework.Common.Logging;
public class JobtoMainFake : IJob
{
    #region job基本參數設置
    protected string strJobId = string.Empty;
    protected string strErrMsg = string.Empty;
    protected DateTime StartTime = DateTime.Now;//記錄job啟動時間
    protected DateTime EndTime;
    protected JobHelper JobHelper = new JobHelper();
    #endregion

    #region 程式入口
    /// <summary>
    /// 功能說明:Job執行入口
    /// 作    者:
    /// 創建時間:
    /// 修改記錄:
    /// </summary>
    /// <param name="context"></param>
    public void Execute(JobExecutionContext context)
    {
        strJobId = context.JobDetail.JobDataMap["jobid"].ToString();
        JobHelper.strJobID = strJobId;
        ExecuteManual(strJobId);
    }
    #endregion
    public string JobID;
    public bool ExecuteManual(string jobID)
    {
        JobID = jobID;
        bool result = true;
        try
        {
            string strMsgID = string.Empty;
            string strRETSTR = string.Empty;
            strJobId = jobID;

            strRETSTR = "*********** " + strJobId + " START **************";
            JobHelper.Write(strJobId, strRETSTR, LogState.Info);

            #region 取得 查詢條件和取值
            string strRunDate = DateTime.Today.ToString("yyyy/MM/dd");
            string strHr = DateTime.Now.Hour.ToString();
            string strRunBat = "1";  //執行的批次 預設第一批，以執行時間區分，若16時啟動，則為第二批
            if (strHr == "16")
            {
                strRunBat = "2";
            }
            if (strHr == "13" || strHr == "16")
            {
                ProcessJob(strRunDate, strRunBat);
            }
            else
            {
                strRETSTR = " 非執行時段，略過 ";
                JobHelper.Write(strJobId, strRETSTR, LogState.Info);
            }


            #endregion
            //記錄job結束時間
            EndTime = DateTime.Now;

            #region job結束日誌記錄            

            //*判斷job完成狀態
            string strJobStatus = "S";
            string strReturnMsg = string.Empty;
            strReturnMsg += string.Format("資料筆數:{0}", "0");

            //執行結果 寫入 DB            
            if (!string.IsNullOrEmpty(strErrMsg))
            {
                strReturnMsg += ". 失敗訊息:" + strErrMsg;
                strJobStatus = "F";
                result = false;
                JobHelper.Write(strJobId, strErrMsg);
            }

            JobHelper.WriteLogToDB(strJobId, StartTime, EndTime, strJobStatus, strReturnMsg);

            #endregion

            strRETSTR = "*********** " + strJobId + " End **************";
            JobHelper.Write(strJobId, strRETSTR, LogState.Info);

        }
        catch (Exception ex)
        {
            result = false;

            JobHelper.Write(strJobId, ex.Message);
            JobHelper.WriteLogToDB(strJobId, StartTime, EndTime, "F", "發生錯誤：" + ex.Message);

            //*JOB失敗發送MAIL 
            strErrMsg = ex.Message;
            //sendMailmsg(strErrMsg);
        }
        finally
        {
            //清空參數值

        }

        // sendMailmsg(strErrMsg);

        return result;
    }

    /// <summary>
    /// 真正處理批次產檔及上傳FTP的地方
    /// </summary>
    /// <param name="strRunDate"></param>
    /// <param name="strRunBat"></param>
    /// <returns></returns>
    public bool ProcessJob(string strRunDate, string strRunBat)
    {
        bool result = false;
        string root = AppDomain.CurrentDomain.BaseDirectory + "\\" + UtilHelper.GetAppSettings("ExportExcelFilePath").ToString();

        string strMsgID = "";
        string RPath1 = root;  //一般檔案產生後回傳檔名
        string RPath2 = root;  //分期檔案產生後回傳檔名
        string FPath1 = root;  //一般檔案產生後回傳檔名  偽冒
        string FPath2 = root;  //分期檔案產生後回傳檔名   偽冒
        bool res1 = expFile(strRunDate, strRunBat, "1", ref strMsgID, ref RPath1, ref FPath1);
        bool res2 = expFile(strRunDate, strRunBat, "2", ref strMsgID, ref RPath2, ref FPath2);
        //發送 一般、分期主機
        bool isSend1 = SendToFTP("BaseToMain", RPath1, RPath2);
        //  發送 一般、分期偽冒
        bool isSend2 = SendToFTP("BaseToFake", FPath1, FPath2);

        JobHelper.Write(JobID, " FTP檔案傳送狀態 : 主機 :" + isSend1 + " 偽冒 :" + isSend2 + "\r\n產檔日期 :" + strRunDate + "產檔批次 :" + strRunBat, LogState.Info);



        return result;
    }

    //依傳入批次產生一般及分期檔案
    private bool expFile(string strBatch_Date, string strReceive_Batch, string strSign_Type, ref string strMsgID, ref string strPathFile, ref string strFPathFile)
    {
        try
        {
            StringBuilder sbCont = new StringBuilder();
            DataSet ds = getData_UploadFile(strBatch_Date, strReceive_Batch, strSign_Type, ref strMsgID);

            string strBatch_Date_TW = BRASExport.ToTWDateString(DateTime.Now).Substring(1, 6);
            //要產出空檔
            if (ds == null)
            {
                //20140922 第一碼固定帶"0"，2-7碼日期請帶系統日(產檔當日)，格式：yyMMdd
                //表頭
                string strHeadN = "0" + strBatch_Date_TW + strReceive_Batch + "0".PadLeft(5, '0') + "0".PadLeft(8, '0') + "0".PadLeft(12, '0') + "00";
                sbCont.AppendLine(strHeadN.PadRight(60, ' '));
                strMsgID = "查無資料";
                if (strSign_Type == "1")
                {
                    //一般主機名稱
                    strPathFile = strPathFile + @"\mpul.txt";
                    //偽冒主機名稱
                    strFPathFile = strFPathFile + @"\mpul" + strReceive_Batch + DateTime.Now.ToString("MMdd") + ".txt";
                }
                else
                {
                    //一般主機名稱
                    strPathFile = strPathFile + @"\ipp" + strReceive_Batch + ".txt";
                    //偽冒主機名稱
                    strFPathFile = strFPathFile + @"\ipp" + strReceive_Batch + DateTime.Now.ToString("MMdd") + ".txt";
                }
                //寫入一般主機名稱
                FileTools.Create(strPathFile, sbCont.ToString().Remove(sbCont.ToString().LastIndexOf("\r\n")));
                //寫入偽冒主機名稱
                FileTools.Create(strFPathFile, sbCont.ToString().Remove(sbCont.ToString().LastIndexOf("\r\n")));

                return true;
            }

            DataTable dtRow2 = ds.Tables["Row2Data"];
            DataTable dtRow3 = ds.Tables["Row3Data"];

            //* 檢查目錄，并刪除以前的文檔資料
            BRExcel_File.CheckDirectory(ref strPathFile);

            //20160531 (U) by Tank, 簽單算總數的title移出來與分期共用
            //20140922 第一碼固定帶"0"，2-7碼日期請帶系統日(產檔當日)，格式：yyMMdd
            //西元年轉民國年 yyyMMdd 共7碼，不足左邊補0
            //string strBatch_Date_TW = BRASExport.ToTWDateString(DateTime.Parse(strBatch_Date));


            //封皮總筆數(查詢總筆數) 長度為5碼
            string strRowsCount = dtRow2.Rows.Count.ToString().PadLeft(5, '0');

            //20140925  帶上傳檔第一碼為2的加總
            //內頁總筆數(加總[人工簽單批次資料檔].[收件-總筆數]) 長度為8碼
            string strSumRTC = dtRow2.Compute("Sum(Receive_Total_Count)", "").ToString().PadLeft(8, '0');

            //20140930  總金額為上傳檔第一碼為2的加總NET值(請款加總 - 退款加總)
            //總金額(加總[人工簽單批次資料檔].[收件-總筆數]) 長度為12碼
            //string strSumRTA = Convert.ToInt64(dtRow2.Compute("Sum(Receive_Total_AMT)", "")).ToString().PadLeft(12, '0');
            Int64 i40 = Convert.ToInt64(dtRow3.Compute("Sum(AMT)", "Receipt_Type='40'") == DBNull.Value ? 0 : dtRow3.Compute("Sum(AMT)", "Receipt_Type='40'"));   //明細資料請款加總
            Int64 i41 = Convert.ToInt64(dtRow3.Compute("Sum(AMT)", "Receipt_Type='41'") == DBNull.Value ? 0 : dtRow3.Compute("Sum(AMT)", "Receipt_Type='41'"));   //明細資料退款加總
                                                                                                                                                                  //20141008  計算後取絕對值
                                                                                                                                                                  //string strSumRTA = Convert.ToString(i40 - i41).PadLeft(12, '0');
            string strSumRTA = Convert.ToString(Math.Abs(i40 - i41)).PadLeft(12, '0');

            //20140922 第一碼固定帶"0"，2-7碼日期請帶系統日(產檔當日)，格式：yyMMdd
            //表頭
            string strHead = "0" + strBatch_Date_TW + strReceive_Batch + strRowsCount + strSumRTC + strSumRTA + "00";
            sbCont.AppendLine(strHead.PadRight(60, ' '));

            if (strSign_Type == "1")
            {
                #region 一般簽單
                //內容
                foreach (DataRow dr2 in dtRow2.Rows)
                {
                    //第二列資料
                    //批號 長度為3碼
                    string strBatch_NO = dr2["Batch_NO"].ToString().PadLeft(3, '0');
                    //該批總筆數 長度為3碼
                    string strReceive_Total_Count = dr2["Receive_Total_Count"].ToString().PadLeft(3, '0');
                    //該批總金額 長度為9碼
                    string strReceive_Total_AMT = Convert.ToInt64(dr2["Receive_Total_AMT"]).ToString().PadLeft(9, '0');
                    //商店代號 長度為10碼
                    string strShop_ID = dr2["Shop_ID"].ToString().PadLeft(10, '0');

                    //組合第二列資料
                    string strRow2 = "100" + strBatch_NO + strReceive_Total_Count + strReceive_Total_AMT + "00" + strShop_ID;
                    sbCont.AppendLine(strRow2.PadRight(60, ' '));

                    //20141014  上傳檔序號另外編碼，由1開始連續累加
                    int iSN = 1;

                    //第三列資料 (明細)
                    foreach (DataRow dr3 in dtRow3.Select("Batch_NO=" + dr2["Batch_NO"].ToString()))
                    {
                        //序號 長度為3碼
                        //20141014  上傳檔序號另外編碼，由1開始連續累加
                        //string strSN = dr3["SN"].ToString().PadLeft(3, '0');
                        string strSN = iSN.ToString().PadLeft(3, '0');
                        iSN++;
                        //卡號 長度為16碼
                        string strCard_No = dr3["Card_No"].ToString().PadLeft(16, '0');
                        //交易金額 長度為7碼
                        string strAMT = Convert.ToInt64(dr3["AMT"]).ToString().PadLeft(7, '0');
                        //請退款 長度為2碼
                        string strReceipt_Type = dr3["Receipt_Type"].ToString().PadLeft(2, '0');
                        //交易日期(西元年:MMDDYY) 長度為6碼
                        string strTran_Date = DateTime.ParseExact(dr3["Tran_Date"].ToString(), "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces).ToString("MMddyy");
                        //授權號碼 長度為6碼
                        string strAuth_Code = dr3["Auth_Code"].ToString().PadLeft(6, '0');

                        //組合第三列資料
                        string strRow3 = "200" + strBatch_NO + strSN + strCard_No + strAMT + "00" + strReceipt_Type + strTran_Date + strAuth_Code;
                        sbCont.AppendLine(strRow3.PadRight(60, ' '));
                    }
                }
                //20141009  檔名更改為mpul
                //一般主機名稱
                strPathFile = strPathFile + @"\mpul.txt";
                //偽冒主機名稱
                strFPathFile = strFPathFile + @"\mpul" + strReceive_Batch + DateTime.Now.ToString("MMdd") + ".txt";
                #endregion 一般簽單
            }
            else
            {
                #region 分期簽單
                foreach (DataRow dr2 in dtRow2.Rows)
                {
                    //H(第一列)資料
                    //商店代號 長度為10碼
                    string strShop_ID = dr2["Shop_ID"].ToString().PadLeft(10, '0');
                    //批號 長度為5碼
                    string strBatch_NO = dr2["Batch_NO"].ToString().PadLeft(5, '0');
                    //該批總筆數 長度為5碼
                    string strReceive_Total_Count = dr2["Receive_Total_Count"].ToString().PadLeft(5, '0');
                    //該批總金額 長度為9碼
                    string strReceive_Total_AMT = Convert.ToInt64(dr2["Receive_Total_AMT"]).ToString().PadLeft(9, '0');

                    //組合第一列資料
                    string strRow2 = "H" + strShop_ID + strBatch_NO + strReceive_Total_Count + strReceive_Total_AMT + strBatch_Date.Replace("/", "") + "P";
                    sbCont.AppendLine(strRow2.PadRight(120, ' '));

                    //B(第二列)資料 (明細)
                    foreach (DataRow dr3 in dtRow3.Select("Shop_ID=" + dr2["Shop_ID"].ToString() + " AND Batch_NO=" + dr2["Batch_NO"].ToString()))
                    {
                        //卡號 長度為16碼
                        string strCard_No = dr3["Card_No"].ToString().PadLeft(16, '0');
                        //分期數 長度為2碼
                        string strINST = dr3["Installment_Periods"].ToString().Trim().PadLeft(2, '0');
                        //產品別 長度為3碼
                        string strProduct_Type = dr3["Product_Type"].ToString().PadLeft(3, '0');
                        //分期總價 長度為8碼
                        string strAMT = Convert.ToInt64(dr3["AMT"]).ToString().PadLeft(8, '0');
                        //交易日期(西元年:MMDDYY) 長度為8碼
                        string strTran_Date = dr3["Tran_Date"].ToString();
                        //授權號碼 長度為6碼
                        string strAuth_Code = dr3["Auth_Code"].ToString().PadLeft(6, '0');

                        //組合第二列資料
                        string strRow3 = "B" + strShop_ID + strBatch_Date.Replace("/", "") + strBatch_NO + "1" + strCard_No + strINST + strProduct_Type + strAMT
                                        + "0000" + strTran_Date + strAuth_Code + "00";
                        sbCont.AppendLine(strRow3.PadRight(120, ' '));
                    }
                }
                // strPathFile = strPathFile + @"\ipp" + strReceive_Batch + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                //一般主機名稱
                strPathFile = strPathFile + @"\ipp" + strReceive_Batch + ".txt";
                //偽冒主機名稱
                strFPathFile = strFPathFile + @"\ipp" + strReceive_Batch + DateTime.Now.ToString("MMdd") + ".txt";
                #endregion 分期簽單
            }
            //  FileTools.Create(strPathFile, sbCont.ToString().Remove(sbCont.ToString().LastIndexOf("\r\n")));
            //寫入一般主機名稱
            FileTools.Create(strPathFile, sbCont.ToString().Remove(sbCont.ToString().LastIndexOf("\r\n")));
            //寫入偽冒主機名稱
            FileTools.Create(strFPathFile, sbCont.ToString().Remove(sbCont.ToString().LastIndexOf("\r\n")));
            strMsgID = "01_01070200_005";
            return true;
        }
        catch (Exception ex)
        {
            strMsgID = "01_01070200_004";
            JobHelper.Write(JobID, "[FAIL] " + ex.ToString());
            throw ex;
        }
    }

    /// <summary>
    /// 產生上傳檔案(一般簽單及分期簽單)時，查詢資料
    /// </summary>
    /// <param name="strBatch_Date">編批日期</param>
    /// <param name="strReceive_Batch">收件批次</param>
    /// <param name="strSign_Type">簽單類別</param>
    /// <param name="strMsgID">返回的錯誤ID號</param>
    /// <returns>成功時：返回查詢結果；失敗時：null</returns>
    public static DataSet getData_UploadFile(string strBatch_Date, string strReceive_Batch, string strSign_Type, ref string strMsgID)
    {
        try
        {
            #region 依據Request查詢資料庫

            //* 聲明SQL Command變量
            SqlCommand sqlcmSearchData = new SqlCommand();
            sqlcmSearchData.CommandType = CommandType.Text;

            if (strSign_Type == "1")
            {
                //一般簽單
                //* 查詢上傳檔案 一般簽單 資料
                //20141008 第二列總金額為NET值：(請款-退款)取絕對值
                //20140925 正常件才列出，並且以正常件重新計算總筆數及總金額
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                sqlcmSearchData.CommandText = string.Format(@"--第二列為該[編批日期]，[收單批號]項下每一批號的資料
                                                SELECT Batch_NO, Shop_ID, count(*) AS Receive_Total_Count, 
                                                    ABS(SUM(CASE WHEN Receipt_Type = '40' THEN AMT ELSE -AMT END)) AS Receive_Total_AMT
                                                FROM (
	                                                SELECT Batch_NO, Shop_ID, AMT, Receipt_Type
	                                                FROM [{0}].[dbo].[Artificial_Signing_Detail]
	                                                WHERE KeyIn_Flag = '2'
		                                                AND Batch_Date = @Batch_Date
		                                                AND Receive_Batch = @Receive_Batch
		                                                AND Sign_Type = @Sign_Type
		                                                AND Case_Status = '0'
	                                                ) a
                                                GROUP BY Batch_NO, Shop_ID
                                                ORDER BY Batch_NO, Shop_ID;

                                                --第三列為第二列的明細資料
                                                SELECT Batch_NO, Shop_ID, SN, Card_No, AMT, Receipt_Type, Tran_Date, Auth_Code
                                                FROM [{0}].[dbo].[Artificial_Signing_Detail]
                                                WHERE KeyIn_Flag = '2'
	                                                AND Batch_Date = @Batch_Date
	                                                AND Receive_Batch = @Receive_Batch
	                                                AND Sign_Type = @Sign_Type
                                                    AND Case_Status = '0'
                                                ORDER BY Batch_NO, Shop_ID;", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            }
            else
            {
                //分期簽單
                //* 查詢上傳檔案 分期簽單 資料
                //20141008 第二列總金額為NET值：(請款-退款)取絕對值
                //20140925 正常件才列出，並且以正常件重新計算總筆數及總金額
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                sqlcmSearchData.CommandText = string.Format(@"--第二列為該[編批日期]，[收單批號]項下每一批號的資料
                                                SELECT Batch_NO, Shop_ID, count(*) AS Receive_Total_Count, 
                                                    ABS(SUM(CASE WHEN Receipt_Type = '40' THEN AMT ELSE -AMT END)) AS Receive_Total_AMT
                                                FROM (
	                                                SELECT Batch_NO, Shop_ID, AMT, Receipt_Type
	                                                FROM [{0}].[dbo].[Artificial_Signing_Detail]
	                                                WHERE KeyIn_Flag = '2'
		                                                AND Batch_Date = @Batch_Date
		                                                AND Receive_Batch = @Receive_Batch
		                                                AND Sign_Type = @Sign_Type
		                                                AND Case_Status = '0'
	                                                ) a
                                                GROUP BY Batch_NO, Shop_ID
                                                ORDER BY Batch_NO, Shop_ID;

                                                --第三列為第二列的明細資料
                                                SELECT Batch_NO, Shop_ID, Card_No, AMT, Tran_Date, Installment_Periods, Product_Type, Auth_Code, Receipt_Type
                                                FROM [{0}].[dbo].[Artificial_Signing_Detail]
                                                WHERE KeyIn_Flag = '2'
	                                                AND Batch_Date = @Batch_Date
	                                                AND Receive_Batch = @Receive_Batch
	                                                AND Sign_Type = @Sign_Type
                                                    AND Case_Status = '0'
                                                ORDER BY Batch_NO, Shop_ID;", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            }
            //* 編批日期
            SqlParameter parmBatchDate = new SqlParameter("@Batch_Date", strBatch_Date.Replace("/", ""));
            sqlcmSearchData.Parameters.Add(parmBatchDate);
            //* 收件批次
            SqlParameter parmReceiveBatch = new SqlParameter("@Receive_Batch", strReceive_Batch);
            sqlcmSearchData.Parameters.Add(parmReceiveBatch);
            //* 簽單類別
            SqlParameter parmSignType = new SqlParameter("@Sign_Type", strSign_Type);
            sqlcmSearchData.Parameters.Add(parmSignType);

            //* 查詢數據
            DataSet dstSearchData = BRASExport.SearchOnDataSet(sqlcmSearchData);
            if (null == dstSearchData)  //* 查詢數據失敗
            {
                strMsgID = "01_01070200_001";
                return null;
            }
            else
            {
                //* 查詢的數據不存在表示通過檢查回傳TRUE
                if (dstSearchData.Tables[0].Rows.Count == 0)
                {
                    strMsgID = "01_01070200_002";
                    return null;
                }
            }
            //* 查詢成功，有數據表示檢核失敗傳回FALSE及訊息
            dstSearchData.Tables[0].TableName = "Row2Data";
            dstSearchData.Tables[1].TableName = "Row3Data";
            strMsgID = "01_01070200_003";
            return dstSearchData;

            #endregion 依據Request查詢資料庫
        }
        catch (Exception exp)
        {
            BRCompareRpt.SaveLog(exp);
            strMsgID = "01_01070200_001";
            return null;
        }
    }


    /// <summary>
    ///  發送FTP
    /// </summary>  
    public bool SendToFTP(string jobID, string filePath1, string FilePath2)
    {

        bool result = true;
        string ftpIP = string.Empty;
        string ftpId = string.Empty;
        string ftpPwd = string.Empty;
        string ftpPath = string.Empty;
        string ZipPwd = string.Empty;

        bool isGet = GetFTPInfo2(jobID, ref ftpIP, ref ftpId, ref ftpPwd, ref ftpPath, ref ZipPwd);

        FTPFactory objFtp = new FTPFactory(ftpIP, "", ftpId, ftpPwd, "21", ftpPath, "Y");
        bool isSuccess = objFtp.Upload(ftpPath, Path.GetFileName(filePath1), filePath1);
        bool isSuccess2 = objFtp.Upload(ftpPath, Path.GetFileName(FilePath2), FilePath2);
        if (isSuccess && isSuccess2)
        {
            result = true;
        }
        JobHelper.Write(jobID, " FTP檔案傳送狀態 : 一般檔案 :" + isSuccess + " 分期檔案 :" + isSuccess2, LogState.Info);

        return result;
    }




    /// <summary>
    /// 取 FTP FileInfo
    /// </summary>
    /// <param name="ftpIp"></param>
    /// <param name="ftpId"></param>
    /// <param name="FtpPwd"></param>
    /// <param name="FtpPath"></param>
    /// <param name="ZipPwd"></param>
    /// <returns></returns>
    public static bool GetFTPInfo2(string jobID, ref string ftpIp, ref string ftpId, ref string FtpPwd, ref string FtpPath, ref string ZipPwd)
    {
        DataTable result = new DataTable();
        string sql = @"SELECT FtpIP, FtpUserName, FtpPwd, FtpPath, ZipPwd FROM [dbo].[tbl_FileInfo] WITH(NOLOCK) WHERE Job_ID = '" + jobID + "'";
        try
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;

            DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
            if (resultSet != null && resultSet.Tables.Count > 0)
            {
                ftpIp = resultSet.Tables[0].Rows[0][0].ToString();
                ftpId = resultSet.Tables[0].Rows[0][1].ToString();
                //測試用密碼不加密
                //FtpPwd = resultSet.Tables[0].Rows[0][2].ToString();
                //FtpPath = resultSet.Tables[0].Rows[0][3].ToString();
                //ZipPwd = resultSet.Tables[0].Rows[0][4].ToString();
                FtpPwd = RedirectHelper.GetDecryptString(resultSet.Tables[0].Rows[0][2].ToString());
                FtpPath = resultSet.Tables[0].Rows[0][3].ToString();
                ZipPwd = RedirectHelper.GetDecryptString(resultSet.Tables[0].Rows[0][4].ToString());
            }

            return true;
        }
        catch (Exception ex)
        {
            string ems = ex.Message;
            return false;
        }
    }






}