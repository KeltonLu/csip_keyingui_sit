//******************************************************************
//*  作    者：蘇洺葳
//*  功能說明：郵局報表查詢，匯出到Excel
//*  創建日期：2018/10/15
//*  修改紀錄：2021/04/01_Ares_Stanley-移除MicrosoftExcel
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Web;
using System.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using Framework.Common.Logging;
using Framework.Data.OM;
using CSIPCommonModel.BaseItem;
using Framework.Common.Utility;
using DataTable = System.Data.DataTable;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.HSSF.EventUserModel.DummyRecord;
using NPOI.XSSF.UserModel.Charts;


namespace CSIPKeyInGUI.BusinessRules_new
{
    /// <summary>
    /// 報表查詢時，匯出到Excel
    /// 修改日期: 2020/12/10_Ares_Stanley- 變更postOfficeTemp_Fail 資料順序
    /// </summary>
    public class BRExcel_File
    {
        const string postOfficeTemp_Count = @"
SELECT UploadDate, SUM(AllCount) AllCount, SUM(SCount) SCount, SUM(FCount) FCount, SUM(NCount) NCount
FROM (
	SELECT UploadDate,
			COUNT(UploadDate) AllCount,
			0 SCount,
			0 FCount,
			0 NCount
	FROM [dbo].[PostOffice_Temp] a WITH(NOLOCK)
	WHERE a.UploadDate = @UploadDate AND a.IsSendToPost = '1'
	GROUP BY UploadDate
	UNION ALL
	SELECT UploadDate,
			0 AllCount,
			COUNT(UploadDate) SCount,
			0 FCount,
			0 NCount
	FROM [dbo].[PostOffice_Temp] a WITH(NOLOCK)
	WHERE a.UploadDate = @UploadDate AND a.IsSendToPost = '1' AND a.SendHostResultCode = '0000'
	GROUP BY UploadDate
	UNION ALL
	SELECT UploadDate,
			0 AllCount,
			0 SCount,
			COUNT(UploadDate) FCount,
			0 NCount
	FROM [dbo].[PostOffice_Temp] a WITH(NOLOCK)
	WHERE a.UploadDate = @UploadDate AND a.IsSendToPost = '1' AND (a.SendHostResultCode != '' AND a.SendHostResultCode != '0000')
	GROUP BY UploadDate
	UNION ALL
	SELECT UploadDate,
			0 AllCount,
			0 SCount,
			0 FCount,
			COUNT(UploadDate) NCount
	FROM [dbo].[PostOffice_Temp] a WITH(NOLOCK)
	WHERE a.UploadDate = @UploadDate AND a.IsSendToPost = '1' AND a.SendHostResultCode = ''
	GROUP BY UploadDate
) A
GROUP BY A.UploadDate";
        //2021/03/17_Ares_Stanley-DB名稱改為變數
        const string postOfficeTemp_Success = @"
SELECT A.UploadDate, a.ReceiveNumber, a.CusID, a.SendHostResultCode, a.AccNoBank, 
		a.AccNo, ISNULL(b.Pay_Way, '') PayWay, a.AccID, 
	ISNULL(b.bcycle_code, '') bcycle_code, ISNULL(b.Mobile_Phone, '') Mobile_Phone, 
	ISNULL(b.E_Mail, '') E_Mail, ISNULL(b.E_Bill, '') E_Bill, ISNULL(c.[user_name], 'CSIP_SYSTEM') [UserName] 
FROM [dbo].[PostOffice_Temp] a WITH(NOLOCK)
LEFT JOIN [dbo].[Auto_Pay] b WITH(NOLOCK) ON a.ReceiveNumber = b. Receive_Number AND b.KeyIn_Flag = '2'
LEFT JOIN (SELECT DISTINCT user_id, User_name FROM {0}.dbo.M_USER WITH(NOLOCK)) c ON a.[AgentID] = c.[user_id] 
WHERE a.UploadDate = @UploadDate AND a.IsSendToPost = '1' AND a.SendHostResultCode = '0000'";
        //2021/03/17_Ares_Stanley-DB名稱改為變數
        const string postOfficeTemp_Fail = @"
SELECT A.UploadDate, a.ReceiveNumber, a.CusID, 
	CASE a.SendHostResultCode WHEN '0009' THEN '週期件' WHEN '8001' THEN 'PCMC失敗-8001' ELSE 'PCMC失敗' END SendHostResultCode,  a.AccNoBank,
	a.AccNo, ISNULL(b.Pay_Way, '') PayWay, a.AccID, 
	ISNULL(b.bcycle_code, '') bcycle_code, ISNULL(b.Mobile_Phone, '') Mobile_Phone, 
	ISNULL(b.E_Mail, '') E_Mail, ISNULL(b.E_Bill, '') E_Bill, ISNULL(c.[user_name], 'CSIP_SYSTEM') [UserName] 
FROM [dbo].[PostOffice_Temp] a WITH(NOLOCK)
LEFT JOIN [dbo].[Auto_Pay] b WITH(NOLOCK) ON a.ReceiveNumber = b. Receive_Number AND b.KeyIn_Flag = '2'
LEFT JOIN (SELECT DISTINCT user_id, User_name FROM {0}.dbo.M_USER WITH(NOLOCK)) c ON a.[AgentID] = c.[user_id] 
WHERE a.UploadDate = @UploadDate AND a.IsSendToPost = '1' AND (a.SendHostResultCode != '' AND a.SendHostResultCode != '0000')";

        /// <summary>
        /// 檢查路徑是否存在，存在刪除該路徑下所有的文檔資料
        /// </summary>
        /// <param name="strPath"></param>
        public static void CheckDirectory(ref string strPath)
        {
            try
            {
                string strOldPath = strPath;
                // 判斷路徑是否存在
                strPath = strPath + "\\" + DateTime.Now.ToString("yyyyMMdd");
                if (!Directory.Exists(strPath))
                {
                    // 如果不存在，創建路徑
                    Directory.CreateDirectory(strPath);
                }

                // 取該路徑下所有路徑
                string[] strDirectories = Directory.GetDirectories(strOldPath);
                for (int intLoop = 0; intLoop < strDirectories.Length; intLoop++)
                {
                    if (strDirectories[intLoop].ToString() != strPath)
                    {
                        if (Directory.Exists(strDirectories[intLoop]))
                        {
                            // 刪除目錄下的所有文檔
                            DirectoryInfo di = new DirectoryInfo(strDirectories[intLoop]);
                            FileSystemInfo[] fsi = di.GetFileSystemInfos();
                            for (int intIndex = 0; intIndex < fsi.Length; intIndex++)
                            {
                                FileInfo fi = fsi[intIndex] as FileInfo;
                                if (fi != null)
                                {
                                    fi.Delete();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Logging.Log(exp, LogLayer.BusinessRule);
                throw exp;
            }
        }

        /// <summary>
        /// 郵局ACH授權扣款資料清單匯出到Excel
        /// 修改紀錄: 2020/12/03_Ares_Stanley-變更報表產出方式為NPOI; 2020/12/03_Ares_Stanley-增加合計千分位
        /// </summary>
        /// <param name="uploadDate"></param>
        /// <param name="agentName"></param>
        /// <param name="pathFile"></param>
        /// <param name="msgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_ACH(string uploadDate, string agentName, ref string pathFile, ref string msgID)
        {

            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref pathFile);

                // 取要下載的資料
                DataTable dtblData_ACH = BRExcel_File.getData_ACH(uploadDate);
                if (null == dtblData_ACH)
                    return false;
                if (dtblData_ACH.Rows.Count == 0)
                {
                    msgID = "01_03110100_001";
                    return false;
                }

                #region 匯入Excel文檔

                string excelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "PostOffice.xls";
                FileStream fs = new FileStream(excelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ExportExcelForNPOI(dtblData_ACH, ref wb, 8, "郵局授權扣款資料清單"); // Start index = 10
                ISheet sheet = wb.GetSheet("郵局授權扣款資料清單");
                #region 合計筆數格式
                // 合計文字
                HSSFCellStyle total = (HSSFCellStyle)wb.CreateCellStyle();
                //文字置中
                total.VerticalAlignment = VerticalAlignment.Center;
                HSSFFont total_font = (HSSFFont)wb.CreateFont();
                // cell format
                total.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                //字體尺寸
                total_font.FontHeightInPoints = 12;
                total_font.FontName = "新細明體";
                total_font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                total.SetFont(total_font);

                // 合計數字
                HSSFCellStyle total_num = (HSSFCellStyle)wb.CreateCellStyle();
                //文字置中
                total_num.VerticalAlignment = VerticalAlignment.Center;
                total_num.Alignment = HorizontalAlignment.Center;
                HSSFFont total_num_font = (HSSFFont)wb.CreateFont();
                // cell format
                total_num.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                //字體尺寸
                total_num_font.FontHeightInPoints = 12;
                total_num_font.FontName = "新細明體";
                total_num.SetFont(total_num_font);
                #endregion

                // 合計筆數
                sheet.CreateRow(sheet.LastRowNum + 1).CreateCell(0).SetCellValue("合計筆數：");
                sheet.GetRow(sheet.LastRowNum).CreateCell(1).SetCellValue(dtblData_ACH.Rows.Count.ToString("N0"));
                sheet.GetRow(sheet.LastRowNum).GetCell(0).CellStyle = total;
                sheet.GetRow(sheet.LastRowNum).GetCell(1).CellStyle = total_num;

                #region 表頭
                for(int row=3; row < 5; row++)
                {
                    sheet.GetRow(row).CreateCell(1).CellStyle.Alignment = HorizontalAlignment.Center;
                }
                // 拋檔日
                sheet.GetRow(3).GetCell(1).SetCellValue(uploadDate);
                // 列印經辦
                sheet.GetRow(4).GetCell(1).SetCellValue(agentName);
                // 列印日期
                sheet.GetRow(5).GetCell(1).SetCellValue(DateTime.Now.ToString("yyyyMMdd"));
                #endregion
                // 保存文件到程序运行目录下
                pathFile = pathFile + @"\ExcelFile_PostOffice_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(pathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();
                return true;
                # endregion 匯入文檔結束
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
                throw ex;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 郵局R02授權成功/失敗報表
        /// 修改日期: 2020/12/04_Ares_Stanley-修改報表產出方式為NPOI; 2020/12/10_Ares_Stanley-新增總計千分位
        /// </summary>
        /// <param name="uploadDate"></param>
        /// <param name="reportType"></param>
        /// <param name="agentName"></param>
        /// <param name="pathFile"></param>
        /// <param name="msgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_R02(string uploadDate, string sReplyDate, string eReplyDate, string reportType, string postRtnMsg, string agentName, ref string pathFile, ref string msgID, bool blSuccess, bool blFail)
        {

            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref pathFile);

                // 取要下載的資料
                DataTable dtblData_R02 = BRExcel_File.getData_R02(uploadDate, reportType, sReplyDate, eReplyDate, postRtnMsg);
                if (null == dtblData_R02)
                    return false;
                if (dtblData_R02.Rows.Count == 0)
                {
                    msgID = "01_03110200_001";
                    return false;
                }

                #region 匯入Excel文檔

                string excelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "R02PostOffice.xls";
                FileStream fs = new FileStream(excelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ExportExcelForNPOI(dtblData_R02, ref wb, 7, "R02授權成功失敗報表"); // Content start index = 8;
                ISheet sheet = wb.GetSheet("R02授權成功失敗報表");
                Int32 intNumS = 0;
                Int32 intNumF = 0;
                #region 表頭
                // title
                if (blSuccess && blFail || !blSuccess && !blFail)
                {
                    sheet.GetRow(0).GetCell(0).SetCellValue("R02『授權成功/失敗』報表");
                    wb.SetSheetName(0, "R02授權成功失敗報表");
                }
                else if (blSuccess)
                {
                    sheet.GetRow(0).GetCell(0).SetCellValue("R02『授權成功』報表");
                    wb.SetSheetName(0, "R02授權成功報表");
                }
                else if (blFail)
                {
                    sheet.GetRow(0).GetCell(0).SetCellValue("R02『授權失敗』報表");
                    wb.SetSheetName(0, "R02授權失敗報表");
                }
                
                for(int row=2; row<5; row++)
                {
                    sheet.GetRow(row).CreateCell(1).CellStyle.Alignment = HorizontalAlignment.Center;
                }
                // 拋檔日
                sheet.GetRow(2).GetCell(1).SetCellValue(uploadDate);
                // 列印經辦
                sheet.GetRow(3).GetCell(1).SetCellValue(agentName);
                // 列印日期
                sheet.GetRow(4).GetCell(1).SetCellValue(DateTime.Now.ToString("yyyyMMdd"));
                #endregion

                #region 表尾
                #region 筆數文字格式
                // 合計文字
                HSSFCellStyle total = (HSSFCellStyle)wb.CreateCellStyle();
                //文字置中
                total.VerticalAlignment = VerticalAlignment.Center;
                HSSFFont total_font = (HSSFFont)wb.CreateFont();
                // cell format
                total.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                //字體尺寸
                total_font.FontHeightInPoints = 12;
                total_font.FontName = "新細明體";
                total_font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                total.SetFont(total_font);
                #endregion 
                // 成功, 失敗計數
                for(int row=7; row<sheet.LastRowNum+1; row++)
                {
                    if (sheet.GetRow(row).GetCell(7).StringCellValue.ToString() == "成功")
                    {
                        intNumS++;
                    }else if (sheet.GetRow(row).GetCell(7).StringCellValue.ToString() == "失敗")
                    {
                        intNumF++;
                    }
                }

                sheet.CreateRow(sheet.LastRowNum + 2); // 表尾空行
                for (int row = 0; row < 3; row++) 
                {
                    switch (row)
                    {
                        case 0:
                            sheet.CreateRow(sheet.LastRowNum + 1).CreateCell(0).CellStyle = total;
                            sheet.GetRow(sheet.LastRowNum).CreateCell(1).CellStyle.Alignment = HorizontalAlignment.Center;
                            sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("成功筆數：");
                            sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(intNumS.ToString("N0"));
                            break;

                        case 1:
                            sheet.CreateRow(sheet.LastRowNum + 1).CreateCell(0).CellStyle = total;
                            sheet.GetRow(sheet.LastRowNum).CreateCell(1).CellStyle.Alignment = HorizontalAlignment.Center;
                            sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("失敗筆數：");
                            sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(intNumF.ToString("N0"));
                            break;

                        case 2:
                            sheet.CreateRow(sheet.LastRowNum + 1).CreateCell(0).CellStyle = total;
                            sheet.GetRow(sheet.LastRowNum).CreateCell(1).CellStyle.Alignment = HorizontalAlignment.Center;
                            sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("總筆數：");
                            sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(Convert.ToInt32(intNumS + intNumF).ToString("N0"));
                            break;

                    }
                }
                #endregion

                // 保存文件到程序运行目录下
                pathFile = pathFile + @"\ExcelFile_R02_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(pathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();
                return true;
                # endregion 匯入文檔結束
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
                throw ex;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 批次作業量統計報表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="agentName"></param>
        /// <param name="pathFile"></param>
        /// <param name="msgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_Batch(string uploadDate, string type, string agentName, ref string pathFile, ref string msgID)
        {
            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref pathFile);

                // 取要下載的資料
                DataTable dtblData_Batch = BRExcel_File.getData_Batch(uploadDate, type);
                if (null == dtblData_Batch)
                    return false;
                if (dtblData_Batch.Rows.Count == 0)
                {
                    msgID = "01_03110400_001";
                    return false;
                }

                #region 匯入Excel文檔
                FileStream fs = null;
                HSSFWorkbook wb = null;
                string strExcelPathFile = "";
                switch (type)
                {
                    case "1":// 批次作業量統計
                        strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "PostOffice_Batch.xls";
                        fs = new FileStream(strExcelPathFile, FileMode.Open);
                        wb = new HSSFWorkbook(fs);
                        WriteDataToSheet_Batch(wb, agentName, dtblData_Batch, uploadDate);
                        break;
                    case "2":// 批次結果報表_成功
                        strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "PostOffice_BatchSuccess.xls";
                        fs = new FileStream(strExcelPathFile, FileMode.Open);
                        wb = new HSSFWorkbook(fs);
                        WriteDataToSheet_Success(wb, agentName, dtblData_Batch, uploadDate);
                        break;
                    case "3":// 批次結果報表_失敗
                        strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "PostOffice_BatchFail.xls";
                        fs = new FileStream(strExcelPathFile, FileMode.Open);
                        wb = new HSSFWorkbook(fs);
                        WriteDataToSheet_Fail(wb, agentName, dtblData_Batch, uploadDate);
                        break;
                }

                // 保存文件到程序运行目录下
                pathFile = pathFile + @"\ExcelFile_BATCH_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(pathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();
                return true;
                # endregion 匯入文檔結束
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
                throw ex;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 匯出ACH授權扣款資料清單資料時，查詢數據
        /// </summary>
        /// <param name="uploadDate"></param>
        /// <returns></returns>
        public static DataTable getData_ACH(string uploadDate)
        {
            string sql = @"
SELECT a.ReceiveNumber,													 -- 收件編號
       a.AccNoBank,														 -- 收受行(核印行)
	   N'中華郵政' AccNoBankName,										 -- 收受行名稱(核印行)
       a.AccID,															 -- 委繳戶統編\身分證字號
       a.AccNo,															 -- 委繳戶帳號
	   a.CusID,															 -- 持卡人ID
	   CASE WHEN a.ApplyCode = 1 THEN 'A' ELSE 'D' END ApplyType		 -- 申請類別
FROM [dbo].[PostOffice_Temp] a WITH(NOLOCK)
LEFT JOIN [dbo].[PostOffice_Rtn_Info] b WITH(NOLOCK) ON a.ReturnStatusTypeCode = b.PostRtnCode AND b.RtnType = '1' 
LEFT JOIN [dbo].[PostOffice_Rtn_Info] c WITH(NOLOCK) ON a.ReturnCheckFlagCode = c.PostRtnCode AND c.RtnType = '2'
LEFT JOIN [dbo].[PostOffice_Rtn_Info] d WITH(NOLOCK) ON d.RtnType = '3'
LEFT JOIN [dbo].[PostOffice_Rtn_Info] e WITH(NOLOCK) ON e.RtnType = '4' 
LEFT JOIN [dbo].[PostOffice_Detail] f WITH(NOLOCK) ON a.ReceiveNumber = f.ReceiveNumber
WHERE UploadDate = @UploadDate";

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            sqlcmd.Parameters.Add(new SqlParameter("@UploadDate", uploadDate));

            return BRPostOffice_Temp.SearchOnDataSet(sqlcmd).Tables[0];
        }

        /// <summary>
        /// 匯出R02授權成功/失敗報表資料時，查詢數據
        /// </summary>
        /// <param name="uploadDate"></param>
        /// <param name="reportType"></param>
        /// <returns></returns>
        public static DataTable getData_R02(string uploadDate, string reportType, string sReplyDate, string eReplyDate, string postRtnMsg)
        {
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            string sql = string.Format(@"
SELECT a.ReceiveNumber,												-- 收件編號
       a.AccNoBank,													-- 收受行(核印行)
       b.PROPERTY_NAME,												-- 收受行名稱(核印行)
       a.AccID,														-- 委繳戶統編\身分證字號
       a.AccNo,														-- 委繳戶帳號
       a.CusID,														-- 持卡人ID
       CASE WHEN a.ApplyCode = 1 THEN 'A' ELSE 'D' END ApplyType,	-- 申請類別
       CASE WHEN a.ReturnStatusTypeCode = '' 
				AND a.ReturnCheckFlagCode = '' THEN '成功' 
	    ELSE '失敗' END ReturnStatus,								-- 成功/失敗
       CASE WHEN a.ReturnStatusTypeCode = '' 
				AND a.ReturnCheckFlagCode = '' THEN ''
			WHEN a.ReturnStatusTypeCode != '' THEN a.ReturnStatusTypeCode + ':' + c.PostRtnMsg
			WHEN a.ReturnCheckFlagCode != '' THEN a.ReturnCheckFlagCode + ':' + d.PostRtnMsg
		 END PostRtnMsg												-- 回覆訊息
FROM [dbo].[PostOffice_Temp] a WITH(NOLOCK) 
LEFT JOIN [{0}].[dbo].[m_property_code] b WITH(NOLOCK) ON b.FUNCTION_KEY = '01' AND b.PROPERTY_KEY = '50' 
LEFT JOIN [dbo].[PostOffice_Rtn_Info] c WITH(NOLOCK) ON a.ReturnStatusTypeCode = c.PostRtnCode AND c.RtnType = '1' 
LEFT JOIN [dbo].[PostOffice_Rtn_Info] d WITH(NOLOCK) ON a.ReturnCheckFlagCode = d.PostRtnCode AND d.RtnType = '2' 
INNER JOIN [dbo].[PostOffice_Detail] e WITH(NOLOCK) ON a.ReceiveNumber = e.ReceiveNumber AND e.Complete = '1'", UtilHelper.GetAppSettings("DB_CSIP"));

            string sqlWhere = " WHERE a.UploadDate = @UploadDate  ";

            if (sReplyDate != "")
            {
                sqlWhere = " WHERE a.ReturnDate >= @sReplyDate AND a.ReturnDate <= @eReplyDate ";
            }

            if (postRtnMsg.Length == 1)
            {
                sqlWhere = sqlWhere + " AND ReturnCheckFlagCode = @postRtnMsg ";
            }

            if (postRtnMsg.Length == 2)
            {
                sqlWhere = sqlWhere + " AND ReturnStatusTypeCode = @postRtnMsg ";
            }

            // 成功/失敗條件
            string condition = (reportType == "S") ? "AND a.ReturnStatusTypeCode = '' AND a.ReturnCheckFlagCode = ''" : ((reportType == "F") ? "AND (a.ReturnStatusTypeCode != '' OR a.ReturnCheckFlagCode != '')" : "");

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql + sqlWhere + condition;

            if (sReplyDate == "")
            {
                sqlcmd.Parameters.Add(new SqlParameter("@UploadDate", uploadDate));
            }
            else
            {
                sqlcmd.Parameters.Add(new SqlParameter("@sReplyDate", sReplyDate));
                sqlcmd.Parameters.Add(new SqlParameter("@eReplyDate", eReplyDate));
            }

            if (postRtnMsg.Length > 0)
            {
                sqlcmd.Parameters.Add(new SqlParameter("@postRtnMsg", postRtnMsg));
            }

            return BRPostOffice_Temp.SearchOnDataSet(sqlcmd).Tables[0];
        }

        /// <summary>
        /// 批次作業量統計報表
        /// </summary>
        /// <param name="uploadDate"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DataTable getData_Batch(string uploadDate, string type)
        {
            SqlCommand sqlcmd = new SqlCommand();
            string sql = "";

            // 添加查詢條件
            switch (type)
            {
                case "1":// 批次作業量統計
                    sql = postOfficeTemp_Count;
                    break;
                case "2":// 批次結果報表_成功
                    //2021/03/17_Ares_Stanley-DB名稱改為變數
                    sql = string.Format(postOfficeTemp_Success, UtilHelper.GetAppSettings("DB_CSIP"));
                    break;
                case "3":// 批次結果報表_失敗
                    //2021/03/17_Ares_Stanley-DB名稱改為變數
                    sql = string.Format(postOfficeTemp_Fail, UtilHelper.GetAppSettings("DB_CSIP"));
                    break;
            }

            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            sqlcmd.Parameters.Add(new SqlParameter("@UploadDate", uploadDate));

            //return (DataSet)BRPostOffice_Temp.SearchOnDataSet(sqlComm);
            return BRPostOffice_Temp.SearchOnDataSet(sqlcmd).Tables[0];
        }

        /// <summary>
        /// 向Sheet寫入數據(批次作業量統計)
        /// 修改紀錄: 2020/12/10_Ares_Stanley-修改報表產出為NPOI
        /// </summary>
        /// <param name="sheet">寫入對象sheet</param>
        /// <param name="agentName">經辦人</param>
        private static void WriteDataToSheet_Batch(HSSFWorkbook wb, string agentName, DataTable dtblWriteData, string uploadDate)
        {
            ExportExcelForNPOI(dtblWriteData, ref wb, 7, "批次作業量統計表");
            ISheet sheet_batch = wb.GetSheet("批次作業量統計表");

            #region 將資料轉為數值型態以套用 Excel 公式
            for (int row = 7; row < sheet_batch.LastRowNum + 1; row++)
            {
                for (int col = 1; col < 5; col++)
                {
                    sheet_batch.GetRow(row).GetCell(0).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0");
                    sheet_batch.GetRow(row).GetCell(col).SetCellValue(Int32.Parse(sheet_batch.GetRow(row).GetCell(col).StringCellValue.ToString()));
                }
            }
            #endregion

            // 新增總計列
            sheet_batch.CreateRow(sheet_batch.LastRowNum + 1);
            for (int col = 0; col < 5; col++)
            {
                sheet_batch.GetRow(sheet_batch.LastRowNum).CreateCell(col).CellStyle = sheet_batch.GetRow(sheet_batch.LastRowNum - 1).GetCell(0).CellStyle;
            }
            sheet_batch.GetRow(sheet_batch.LastRowNum).GetCell(0).SetCellValue("總計");
            string formu = "";
            for (int i = 1; i < 5; i++)
            {
                switch (i)
                {
                    case 1: formu = string.Format("SUM(B8:B{0})", sheet_batch.LastRowNum); break;
                    case 2: formu = string.Format("SUM(C8:C{0})", sheet_batch.LastRowNum); break;
                    case 3: formu = string.Format("SUM(D8:D{0})", sheet_batch.LastRowNum); break;
                    case 4: formu = string.Format("SUM(E8:E{0})", sheet_batch.LastRowNum); break;
                }
                sheet_batch.GetRow(sheet_batch.LastRowNum).GetCell(i).SetCellType(CellType.Formula);
                sheet_batch.GetRow(sheet_batch.LastRowNum).GetCell(i).SetCellFormula(formu);
                sheet_batch.GetRow(sheet_batch.LastRowNum).GetCell(i).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##0");
            }

            // Excel 開啟前先計算公式值
            HSSFFormulaEvaluator.EvaluateAllFormulaCells(wb);

            //表頭
            sheet_batch.GetRow(2).GetCell(1).SetCellValue(uploadDate);
            sheet_batch.GetRow(2).GetCell(1).CellStyle.Alignment = HorizontalAlignment.Center;
            sheet_batch.GetRow(3).GetCell(1).SetCellValue(agentName);
            string nowDate = DateTime.Now.ToString("yyyyMMdd");
            sheet_batch.GetRow(4).GetCell(1).SetCellValue(nowDate);
        }

        /// <summary>
        /// 向Sheet寫入數據(批次結果報表_成功)
        /// 修改紀錄: 2020/12/10_Ares_Stanley-修改報表產出方式為NPOI
        /// </summary>
        /// <param name="sheet">寫入對象sheet</param>
        /// <param name="agentName">經辦人</param>
        private static void WriteDataToSheet_Success(HSSFWorkbook wb, string agentName, DataTable dtblWriteData, string uploadDate)
        {
            ExportExcelForNPOI(dtblWriteData, ref wb, 7, "批次結果報表");
            ISheet sheet_success = wb.GetSheet("批次結果報表");

            #region 成功總筆數文字格式
            // 成功總筆數文字
            HSSFCellStyle total_success = (HSSFCellStyle)wb.CreateCellStyle();
            //文字置中
            total_success.VerticalAlignment = VerticalAlignment.Center;
            total_success.Alignment = HorizontalAlignment.Center;
            HSSFFont total_success_font = (HSSFFont)wb.CreateFont();
            // cell format
            total_success.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
            //字體尺寸
            total_success_font.FontHeightInPoints = 12;
            total_success_font.FontName = "新細明體";
            total_success_font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            total_success.SetFont(total_success_font);
            #endregion

            // 新增成功總筆數列
            sheet_success.CreateRow(sheet_success.LastRowNum + 3).CreateCell(0).CellStyle = total_success;
            sheet_success.GetRow(sheet_success.LastRowNum).CreateCell(1).CellStyle = total_success;
            sheet_success.GetRow(sheet_success.LastRowNum).GetCell(0).SetCellValue("成功筆數：");
            sheet_success.GetRow(sheet_success.LastRowNum).GetCell(1).SetCellValue(dtblWriteData.Rows.Count.ToString("N0"));


            //表頭
            sheet_success.GetRow(2).GetCell(1).SetCellValue(uploadDate);
            sheet_success.GetRow(3).GetCell(1).SetCellValue(agentName);
            string nowDate = DateTime.Now.ToString("yyyyMMdd");
            sheet_success.GetRow(4).GetCell(1).SetCellValue(nowDate);
            for(int row =2; row < 5; row++)
            {
                sheet_success.GetRow(row).GetCell(1).CellStyle.Alignment = HorizontalAlignment.Center;
            }
        }

        /// <summary>
        /// 向Sheet寫入數據(批次結果報表_失敗)
        /// 修改日期: 2020/12/10_Ares_Stanley-變更報表產出方式為NPOI
        /// </summary>
        /// <param name="sheet">寫入對象sheet</param>
        /// <param name="agentName">經辦人</param>
        private static void WriteDataToSheet_Fail(HSSFWorkbook wb, string agentName, DataTable dtblWriteData, string uploadDate)
        {
            ExportExcelForNPOI(dtblWriteData, ref wb, 7, "批次結果報表");
            ISheet sheet_success = wb.GetSheet("批次結果報表");

            #region 成功總筆數文字格式
            // 成功總筆數文字
            HSSFCellStyle total_success = (HSSFCellStyle)wb.CreateCellStyle();
            //文字置中
            total_success.VerticalAlignment = VerticalAlignment.Center;
            total_success.Alignment = HorizontalAlignment.Center;
            HSSFFont total_success_font = (HSSFFont)wb.CreateFont();
            // cell format
            total_success.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
            //字體尺寸
            total_success_font.FontHeightInPoints = 12;
            total_success_font.FontName = "新細明體";
            total_success_font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            total_success.SetFont(total_success_font);
            #endregion

            // 新增成功總筆數列
            sheet_success.CreateRow(sheet_success.LastRowNum + 3).CreateCell(0).CellStyle = total_success;
            sheet_success.GetRow(sheet_success.LastRowNum).CreateCell(1).CellStyle = total_success;
            sheet_success.GetRow(sheet_success.LastRowNum).GetCell(0).SetCellValue("失敗筆數：");
            sheet_success.GetRow(sheet_success.LastRowNum).GetCell(1).SetCellValue(dtblWriteData.Rows.Count.ToString("N0"));


            //表頭
            sheet_success.GetRow(2).GetCell(1).SetCellValue(uploadDate);
            sheet_success.GetRow(3).GetCell(1).SetCellValue(agentName);
            string nowDate = DateTime.Now.ToString("yyyyMMdd");
            sheet_success.GetRow(4).GetCell(1).SetCellValue(nowDate);
            for (int row = 2; row < 5; row++)
            {
                sheet_success.GetRow(row).GetCell(1).CellStyle.Alignment = HorizontalAlignment.Center;
            }
        }
        #region 共用NPOI
        /// <summary>
        /// 修改紀錄:2020/11/26_Ares_Stanley-新增共用NPOI
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="wb"></param>
        /// <param name="start"></param>
        /// <param name="sheetName"></param>
        private static void ExportExcelForNPOI(DataTable dt, ref HSSFWorkbook wb, Int32 start, String sheetName)
        {
            try
            {
                HSSFCellStyle cs = (HSSFCellStyle)wb.CreateCellStyle();
                cs.BorderBottom = BorderStyle.Thin;
                cs.BorderLeft = BorderStyle.Thin;
                cs.BorderTop = BorderStyle.Thin;
                cs.BorderRight = BorderStyle.Thin;
                //啟動多行文字
                cs.WrapText = true;
                //文字置中
                cs.VerticalAlignment = VerticalAlignment.Center;
                cs.Alignment = HorizontalAlignment.Center;

                HSSFFont font1 = (HSSFFont)wb.CreateFont();
                //字體尺寸
                font1.FontHeightInPoints = 12;
                font1.FontName = "新細明體";
                cs.SetFont(font1);

                if (dt != null && dt.Rows.Count != 0)
                {
                    int count = start;
                    ISheet sheet = wb.GetSheet(sheetName);
                    int cols = dt.Columns.Count;
                    foreach (DataRow dr in dt.Rows)
                    {
                        int cell = 0;
                        IRow row = (IRow)sheet.CreateRow(count);
                        row.CreateCell(0).SetCellValue(count.ToString());
                        for (int i = 0; i < cols; i++)
                        {
                            row.CreateCell(cell).SetCellValue(dr[i].ToString());
                            row.GetCell(cell).CellStyle = cs;
                            cell++;
                        }
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                throw;
            }
        }

        /// <summary>
        /// 修改紀錄: 2020/11/26_Ares_Stanley-新增共用NPOI
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="wb"></param>
        /// <param name="start"></param>
        /// <param name="delColumn"></param>
        /// <param name="sheetName"></param>
        private static void ExportExcelForNPOI_filter(DataTable dt, ref HSSFWorkbook wb, Int32 start, Int32 delColumn, String sheetName)
        {
            try
            {
                HSSFCellStyle cs = (HSSFCellStyle)wb.CreateCellStyle();
                cs.BorderBottom = BorderStyle.Thin;
                cs.BorderLeft = BorderStyle.Thin;
                cs.BorderTop = BorderStyle.Thin;
                cs.BorderRight = BorderStyle.Thin;

                //啟動多行文字
                cs.WrapText = true;
                //文字置中
                cs.VerticalAlignment = VerticalAlignment.Center;
                cs.Alignment = HorizontalAlignment.Center;

                HSSFFont font1 = (HSSFFont)wb.CreateFont();
                //字體尺寸
                font1.FontHeightInPoints = 12;
                font1.FontName = "新細明體";
                cs.SetFont(font1);

                if (dt != null && dt.Rows.Count != 0)
                {
                    int count = start;
                    ISheet sheet = wb.GetSheet(sheetName);
                    int cols = dt.Columns.Count - delColumn;
                    foreach (DataRow dr in dt.Rows)
                    {
                        int cell = 0;
                        IRow row = (IRow)sheet.CreateRow(count);
                        row.CreateCell(0).SetCellValue(count.ToString());
                        for (int i = 0; i < cols; i++)
                        {
                            row.CreateCell(cell).SetCellValue(dr[i].ToString());
                            row.GetCell(cell).CellStyle = cs;
                            cell++;
                        }
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                throw;
            }
        }
        #endregion

    }

}
