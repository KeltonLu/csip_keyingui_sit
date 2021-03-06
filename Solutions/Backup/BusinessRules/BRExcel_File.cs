//******************************************************************
//*  作    者：占偉林(WeilinZhan)
//*  功能說明：報表查詢時，匯出到Excel
//*  創建日期：2009/11/24
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
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
using Microsoft.Office.Interop.Excel;
using ExcelApplication = Microsoft.Office.Interop.Excel.ApplicationClass;
using DataTable = System.Data.DataTable;




namespace CSIPKeyInGUI.BusinessRules
{
    /// <summary>
    /// 報表查詢時，匯出到Excel
    /// </summary>
    public class BRExcel_File
    {
        #region sql語句
        public const string SEL_ACH_OTHER_BANK_TEMP = " select a.Receive_Number,a.Other_Bank_Code_L,b.PROPERTY_NAME," +
                           "a.Other_Bank_Cus_ID,a.Other_Bank_Acc_No,a.Cus_ID,a.Apply_Type " +
                       "from Other_Bank_Temp a left join CSIP.dbo.M_PROPERTY_CODE b on a.Other_Bank_Code_S = b.PROPERTY_CODE " +
                       "where ((a.KeyIn_Flag='0') or (a.KeyIn_Flag='2' and a.Upload_flag='Y')) and b.FUNCTION_KEY='01'  and    b.PROPERTY_KEY='17'   {0} " +
                       "order by a.Receive_Number";

        public const string SEL_ACH_OTHER_BANK_TEMP_R02 = @"select a.Receive_Number,a.Other_Bank_Code_L,c.BankName,a.Other_Bank_Cus_ID," +
                        "a.Other_Bank_Acc_No,a.Cus_ID,a.Apply_Type," +
                        "CASE WHEN a.Ach_Return_Code = '0' THEN '成功' ELSE '失敗' END AS R02_flag,d.Ach_Rtn_Code,d.Ach_Rtn_Msg " +
                       "from Other_Bank_Temp a left join batch b on a.Batch_no = b.Batch_no " +
                        "left join " +
                        "(select bankl.property_code as BankCodeS,bankl.property_name as BankCodeL,bankn.property_name as BankName  " +
                        "from (select property_code ,property_name from csip.dbo.m_property_code where function_key='01' and property_key='16') as bankl, " +
                        "(select property_code ,property_name from csip.dbo.m_property_code where function_key='01' and property_key='17') as bankn " +
                        "where   bankl.property_code= bankn.property_code " +
                        ") as c " +
                        "on a.Other_Bank_Code_L=c.BankCodeL " +
                        "left join Ach_Rtn_Info d on a.ACH_Return_Code=d.Ach_Rtn_Code " +
                       "where {0} order by Receive_Number";

        public const string SEL_BATCH = @"select batch_no,sum(AllCount) as AllCount,sum(sCount) as sCount,sum(fCount) as fCount,sum(nCount) as nCount " +
                        "From " +
                            "(select batch_no,count(batch_no) as AllCount,'' as sCount,'' as fCount,'' as nCount from Other_Bank_Temp " +
                            "where Batch_no>=@Batch_no_start and Batch_no<=@Batch_no_end group by batch_no " +
                        "Union All " +
                        "select batch_no,'' as AllCount,count(batch_no) as SCount,'' as fCount,'' as nCount from Other_Bank_Temp " +
                        "where Batch_no>=@Batch_no_start and Batch_no<=@Batch_no_end and " +
                            "(Pcmc_Return_Code='PAGE 00 OF 03' or Pcmc_Return_Code='PAGE 02 OF 03') group by batch_no " +
                        "Union All " +
                        "select batch_no,'' as AllCount,'' as SCount,count(batch_no) as fCount,'' as nCount from Other_Bank_Temp " +
                        "where Batch_no>=@Batch_no_start and Batch_no<=@Batch_no_end and " +
                            "(Pcmc_Upload_flag ='1' and  not (Pcmc_Return_Code='PAGE 00 OF 03'  or Pcmc_Return_Code='PAGE 02 OF 03')) " +
                            "group by batch_no " +
                        "Union All " +
                        "select batch_no,'' as AllCount,'' as SCount,'' as fCount,count(batch_no) as nCount from Other_Bank_Temp " +
                        "where Batch_no>=@Batch_no_start and Batch_no<=@Batch_no_end and " +
                            "ISNULL(Pcmc_Upload_flag, '')  <> '1' and ACH_Return_Code = '0' " +
                        "group by batch_no) a group by batch_no ";
        public const string SEL_BATCH_SUCCESS = @"select a.Batch_no,a.Receive_Number,a.Cus_ID,a.C1342_Return_Code,a.Pcmc_Return_Code," +
                            "a.Other_Bank_Code_S,a.Other_Bank_Acc_No,a.Other_Bank_Pay_Way,a.Other_Bank_Cus_ID,a.bcycle_code," +
                            "a.Mobile_Phone,a.E_Mail,E_Bill,isnull(b.[user_name],'') as [user_name] " +
                        "from Other_Bank_Temp a left join (select distinct user_id,User_name from csip.dbo.M_USER) as  b on a.[user_id] = b.[user_id] " +
                        "where Batch_no>=@Batch_no_start and Batch_no<=@Batch_no_end and (Pcmc_Return_Code='PAGE 00 OF 03' or Pcmc_Return_Code='PAGE 02 OF 03') " +
                        "order by a.Batch_no ";
        public const string SEL_BATCH_FAULT = @"select a.Batch_no,a.Receive_Number,a.Cus_ID,a.C1342_Return_Code," +
                            "a.Pcmc_Return_Code,a.Other_Bank_Code_S,a.Other_Bank_Acc_No,a.Other_Bank_Pay_Way," +
                            "a.Other_Bank_Cus_ID,a.bcycle_code,a.Mobile_Phone,a.E_Mail,E_Bill,isnull(b.[user_name],'') as [user_name] " +
                        "from Other_Bank_Temp a left join (select distinct user_id,User_name from csip.dbo.M_USER) as  b on a.[user_id] = b.[user_id] " +
                        "where Batch_no>=@Batch_no_start and Batch_no<=@Batch_no_end and (Pcmc_Upload_flag ='1' and not (Pcmc_Return_Code='PAGE 00 OF 03' or Pcmc_Return_Code='PAGE 02 OF 03')) " +
                        "order by a.Batch_no ";
        public const string SEL_BATCH_NO_COMPLETE = @"select a.Batch_no,a.Receive_Number,a.Cus_ID,a.C1342_Return_Code," +
                            "a.Pcmc_Return_Code,a.Other_Bank_Code_S,a.Other_Bank_Acc_No,a.Other_Bank_Pay_Way," +
                            "a.Other_Bank_Cus_ID,a.bcycle_code,a.Mobile_Phone,a.E_Mail,E_Bill,isnull(b.[user_name],'') as [user_name] " +
                        "from Other_Bank_Temp a left join (select distinct user_id,User_name from csip.dbo.M_USER) as  b on a.[user_id] = b.[user_id] " +
                        "where Batch_no>=@Batch_no_start and Batch_no<=@Batch_no_end and ISNULL(Pcmc_Upload_flag, '')  <> '1' and ACH_Return_Code = '0' " +
                        "order by a.Batch_no ";

        #endregion

        /// <summary>
        /// 下載文檔默認名稱
        /// </summary>
        public static string DownloadFileName = "";

        /// <summary>
        /// 檢查路徑是否存在，存在刪除該路徑下所有的文檔資料
        /// </summary>
        /// <param name="strPath"></param>
        public static void CheckDirectory(ref string strPath)
        {
            try
            {
                string strOldPath = strPath;
                //* 判斷路徑是否存在
                strPath = strPath + "\\" + DateTime.Now.ToString("yyyyMMdd");
                if (!Directory.Exists(strPath))
                {
                    //* 如果不存在，創建路徑
                    Directory.CreateDirectory(strPath);
                }

                //* 取該路徑下所有路徑
                string[] strDirectories = Directory.GetDirectories(strOldPath);
                for (int intLoop = 0; intLoop < strDirectories.Length; intLoop++)
                {
                    if (strDirectories[intLoop].ToString() != strPath)
                    {
                        if (Directory.Exists(strDirectories[intLoop]))
                        {
                            // * 刪除目錄下的所有文檔
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
                Logging.SaveLog(ELogLayer.BusinessRule, exp);
                throw exp;
            }
        }

        /// <summary>
        /// ACH授權扣款資料清單匯出到Excel
        /// </summary>
        /// <param name="dirValues">查詢條件</param>
        /// <param name="strAgentName">組別代號</param>
        /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
        /// <param name="strMsgID">返回消息ID</param>
        /// <param name="strPathFile">服務器端生成的Excel文檔</param>
        /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
        public static bool CreateExcelFile_ACH(Dictionary<string, string> dirValues,
                        string strAgentName, ref string strPathFile, ref string strMsgID)
        {
            //*创建一个 Excel 实例
            ExcelApplication excel = new ExcelApplication();
            try
            {
                //* 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                //* 取要下載的資料
                string strBuildDate = "";
                string strBatchNo = "";
                string strBank_Code = "";
                DataTable dtblData_ACH = BRExcel_File.getData_ACH(dirValues, ref strBuildDate, ref strBatchNo, ref strBank_Code);
                if (null == dtblData_ACH)
                    return false;
                if (dtblData_ACH.Rows.Count == 0)
                {
                    strMsgID = "01_03050100_001";
                    return false;
                }

                #region 匯入Excel文檔

                excel.Visible = false;//* 不显示 Excel 文件,如果为 true 则显示 Excel 文件
                excel.Application.DisplayAlerts = false;
                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "ACHOtherBank.xls";
                Workbook workbook = excel.Workbooks.Open(strExcelPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value);


                Range range = null;//* 创建一个空的单元格对象
                int intRowIndexInSheet = 0;
                Worksheet sheet = null;
                string[,] arrExportData = null;
                arrExportData = new string[dtblData_ACH.Rows.Count, 7];
                for (int intLoop = 0; intLoop < dtblData_ACH.Rows.Count; intLoop++)
                {
                    //* 添加新的Sheet
                    if (intLoop == 0)
                    {
                        sheet = (Worksheet)workbook.Sheets[1];

                        //*Page Title
                        range = sheet.get_Range("B3", Missing.Value);
                        range.Value2 = strBuildDate;

                        range = sheet.get_Range("B5", Missing.Value);
                        range.Value2 = strBatchNo;

                        range = sheet.get_Range("B7", Missing.Value);
                        range.Value2 = strAgentName;

                        string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));

                        range = sheet.get_Range("B8", Missing.Value);
                        range.Value2 = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);

                        intRowIndexInSheet = 10;
                    }

                    intRowIndexInSheet++;
                    //* 收件編號
                    arrExportData[intRowIndexInSheet - 11, 0] = dtblData_ACH.Rows[intLoop]["Receive_Number"].ToString();
                    //* 收受行代號(核印行)
                    arrExportData[intRowIndexInSheet - 11, 1] = dtblData_ACH.Rows[intLoop]["Other_Bank_Code_L"].ToString();
                    //* 收受行名稱(核印行)
                    arrExportData[intRowIndexInSheet - 11, 2] = dtblData_ACH.Rows[intLoop]["PROPERTY_NAME"].ToString();
                    //* 委繳戶統編/身分證字號
                    arrExportData[intRowIndexInSheet - 11, 3] = dtblData_ACH.Rows[intLoop]["Other_Bank_Cus_ID"].ToString();
                    //* 委繳戶帳號
                    arrExportData[intRowIndexInSheet - 11, 4] = dtblData_ACH.Rows[intLoop]["Other_Bank_Acc_No"].ToString();
                    //* 持卡人ID
                    arrExportData[intRowIndexInSheet - 11, 5] = dtblData_ACH.Rows[intLoop]["Cus_ID"].ToString();
                    //* 申請類別
                    arrExportData[intRowIndexInSheet - 11, 6] = dtblData_ACH.Rows[intLoop]["Apply_Type"].ToString();
                }

                range = sheet.get_Range("A11", "G" + intRowIndexInSheet.ToString());
                range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中
                range.Font.Size = 12;//字體大小
                range.Font.Name = "新細明體";
                range.Value2 = arrExportData;
                range.Borders.LineStyle = 1;
                range.EntireColumn.AutoFit();
                range.Font.Bold = false;
                range.Font.ColorIndex = 1;

                intRowIndexInSheet += 2;
                //* 合計筆數
                range = sheet.get_Range("A" + intRowIndexInSheet.ToString(), Missing.Value);
                range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中
                range.Value2 = "合計筆數：";
                range.Font.Name = "新細明體";
                range.Font.Size = "12";
                range.Font.Bold = true;

                range = sheet.get_Range("B" + intRowIndexInSheet.ToString(), Missing.Value);
                range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中
                range.Value2 = dtblData_ACH.Rows.Count.ToString("N0");
                range.Font.Name = "新細明體";
                range.Font.Size = "12";


                intRowIndexInSheet += 4;
                //* 依行庫由小至大排

                range = sheet.get_Range("A" + intRowIndexInSheet.ToString(), Missing.Value);
                range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中
                range.Value2 = "*依行庫由小至大排序";
                range.Font.Name = "Times New Roman";
                range.Font.Size = "12";
                range.Font.ColorIndex = 3;

                //* 保存文件到程序运行目录下
                strPathFile = strPathFile + @"\ExcelFile_ACH_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                sheet.SaveAs(strPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                excel.ActiveWorkbook.Close(false, null, null); //* 关闭 Excel 文件且不保存
                return true;
                # endregion 匯入文檔結束
            }
            catch (Exception ex)
            {
                Logging.SaveLog(ELogLayer.BusinessRule, ex);
                throw ex;
            }
            finally
            {
                excel.Quit(); //* 退出 Excel
                excel = null; //* 将 Excel 实例设置为空
            }
        }

        /// <summary>
        /// R02授權成功/失敗報表
        /// </summary>
        /// <param name="dirValues">查詢條件</param>
        /// <param name="strAgentName">組別代號</param>
        /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
        /// <param name="strMsgID">返回消息ID</param>
        /// <param name="strPathFile">服務器端生成的Excel文檔</param>
        /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
        public static bool CreateExcelFile_R02(Dictionary<string, string> dirValues,
                        string strAgentName, ref string strPathFile, ref string strMsgID)
        {
            //*创建一个 Excel 实例
            ExcelApplication excel = new ExcelApplication();
            try
            {
                //* 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                //* 取要下載的資料
                string strInputDate = "";
                bool blSuccess = false;
                bool blFault = false;
                DataTable dtblData_R02 = BRExcel_File.getData_R02(dirValues, ref strInputDate, ref blSuccess, ref blFault);
                if (null == dtblData_R02)
                    return false;
                if (dtblData_R02.Rows.Count == 0)
                {
                    strMsgID = "01_03050200_001";
                    return false;
                }

                #region 匯入Excel文檔

                excel.Visible = false;//* 不显示 Excel 文件,如果为 true 则显示 Excel 文件
                excel.Application.DisplayAlerts = false;
                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "R02SF.xls";
                Workbook workbook = excel.Workbooks.Open(strExcelPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value);

                Range range = null;//* 创建一个空的单元格对象
                int intRowIndexInSheet = 0;
                Worksheet sheet = null;
                string[,] arrExportData = null;
                arrExportData = new string[dtblData_R02.Rows.Count, 9];
                Int32 intNumS = 0;
                Int32 intNumF = 0;
                for (int intLoop = 0; intLoop < dtblData_R02.Rows.Count; intLoop++)
                {
                    //* 添加新的Sheet
                    if (intLoop == 0)
                    {
                        sheet = (Worksheet)workbook.Sheets[1];

                        //*Page Title
                        range = sheet.get_Range("A1", "I1");
                        if (blSuccess && blFault || !blSuccess && !blFault)
                        {
                            range.Value2 = "R02『授權成功/失敗』報表";
                            sheet.Name = "R02授權成功失敗報表";
                        }
                        else if (blSuccess)
                        {
                            range.Value2 = "RO2『授權成功』報表";
                            sheet.Name = "R02授權成功報表";
                        }
                        else if (blFault)
                        {
                            range.Value2 = "R02『授權失敗』報表";
                            sheet.Name = "R02授權失敗報表";
                        }

                        range = sheet.get_Range("B3", Missing.Value);
                        range.Value2 = strInputDate;

                        range = sheet.get_Range("B4", Missing.Value);
                        range.Value2 = strAgentName;

                        string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
                        range = sheet.get_Range("B5", Missing.Value);
                        range.Value2 = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);

                        intRowIndexInSheet = 7;
                    }

                    intRowIndexInSheet++;

                    //* 收件編號
                    arrExportData[intRowIndexInSheet - 8, 0] = dtblData_R02.Rows[intLoop]["Receive_Number"].ToString();
                    //* 收受行（金支的7碼）
                    arrExportData[intRowIndexInSheet - 8, 1] = dtblData_R02.Rows[intLoop]["Other_Bank_Code_L"].ToString();
                    //* 收受行名稱(核印行)
                    arrExportData[intRowIndexInSheet - 8, 2] = dtblData_R02.Rows[intLoop]["BankName"].ToString();
                    //* 委繳戶統編/身分證字號
                    arrExportData[intRowIndexInSheet - 8, 3] = dtblData_R02.Rows[intLoop]["Other_Bank_Cus_ID"].ToString();
                    //* 委繳戶帳號
                    arrExportData[intRowIndexInSheet - 8, 4] = dtblData_R02.Rows[intLoop]["Other_Bank_Acc_No"].ToString();
                    //* 持卡人ID
                    arrExportData[intRowIndexInSheet - 8, 5] = dtblData_R02.Rows[intLoop]["Cus_ID"].ToString();
                    //* 申請類別
                    arrExportData[intRowIndexInSheet - 8, 6] = dtblData_R02.Rows[intLoop]["Apply_Type"].ToString();
                    //* 成功/失敗
                    arrExportData[intRowIndexInSheet - 8, 7] = dtblData_R02.Rows[intLoop]["R02_flag"].ToString();
                    //* 回覆訊息
                    arrExportData[intRowIndexInSheet - 8, 8] = dtblData_R02.Rows[intLoop]["Ach_Rtn_Code"].ToString().Trim() + "：" + dtblData_R02.Rows[intLoop]["Ach_Rtn_Msg"].ToString().Trim();
                    if (arrExportData[intRowIndexInSheet - 8, 8] == "：")
                    {
                        arrExportData[intRowIndexInSheet - 8, 8] = "系統內無該錯誤代碼對應訊息";
                    }

                    if (dtblData_R02.Rows[intLoop]["R02_flag"].ToString().Trim() == "成功")
                    {
                        intNumS++;
                    }
                    else
                    {
                        intNumF++;
                    }
                }

                range = sheet.get_Range("A8", "A" + intRowIndexInSheet.ToString());
                range.NumberFormatLocal = "@";

                range = sheet.get_Range("B8", "B" + intRowIndexInSheet.ToString());
                range.NumberFormatLocal = "@";

                range = sheet.get_Range("E8", "E" + intRowIndexInSheet.ToString());
                range.NumberFormatLocal = "@";

                range = sheet.get_Range("A8", "I" + intRowIndexInSheet.ToString());
                range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中
                range.Font.Size = 12;//字體大小
                range.Font.Name = "新細明體";
                range.Value2 = arrExportData;
                range.Borders.LineStyle = 1;
                range.EntireColumn.AutoFit();

                intRowIndexInSheet += 3;
                //* 成功筆數
                range = sheet.get_Range("A" + intRowIndexInSheet.ToString(), Missing.Value);
                range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中
                range.Value2 = "成功筆數：";
                range.Font.Name = "新細明體";
                range.Font.Size = "12";
                range.Font.Bold = true;

                range = sheet.get_Range("B" + intRowIndexInSheet.ToString(), Missing.Value);
                range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中
                range.Value2 = intNumS.ToString("N0");
                range.Font.Name = "新細明體";
                range.Font.Size = "12";

                //* 失敗筆數
                intRowIndexInSheet++;
                range = sheet.get_Range("A" + intRowIndexInSheet.ToString(), Missing.Value);
                range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中
                range.Value2 = "失敗筆數：";
                range.Font.Name = "新細明體";
                range.Font.Size = "12";
                range.Font.Bold = true;

                range = sheet.get_Range("B" + intRowIndexInSheet.ToString(), Missing.Value);
                range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中
                range.Value2 = intNumF.ToString("N0");
                range.Font.Name = "新細明體";
                range.Font.Size = "12";


                //* 合計筆數
                intRowIndexInSheet++;
                range = sheet.get_Range("A" + intRowIndexInSheet.ToString(), Missing.Value);
                range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中
                range.Value2 = "總筆數：";
                range.Font.Name = "新細明體";
                range.Font.Size = "12";
                range.Font.Bold = true;

                range = sheet.get_Range("B" + intRowIndexInSheet.ToString(), Missing.Value);
                range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中
                range.Value2 = Convert.ToInt32(intNumS + intNumF).ToString("N0");
                range.Font.Name = "新細明體";
                range.Font.Size = "12";


                intRowIndexInSheet += 4;
                //* 合計筆數
                range = sheet.get_Range("A" + intRowIndexInSheet.ToString(), Missing.Value);
                range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中
                range.Value2 = "*依行庫由小至大排序";
                range.Font.Name = "Times New Roman";
                range.Font.Size = "12";
                range.Font.ColorIndex = 3;

                //* 保存文件到程序运行目录下
                strPathFile = strPathFile + @"\ExcelFile_R02_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                sheet.SaveAs(strPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                excel.ActiveWorkbook.Close(false, null, null); //* 关闭 Excel 文件且不保存
                return true;
                # endregion 匯入文檔結束
            }
            catch (Exception ex)
            {
                Logging.SaveLog(ELogLayer.BusinessRule, ex);
                throw ex;
            }
            finally
            {
                excel.Quit(); //* 退出 Excel
                excel = null; //* 将 Excel 实例设置为空
            }
        }

        /// <summary>
        /// 批次作業量統計報表
        /// </summary>
        /// <param name="dirValues">查詢條件</param>
        /// <param name="strAgentName">組別代號</param>
        /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
        /// <param name="strMsgID">返回消息ID</param>
        /// <param name="strPathFile">服務器端生成的Excel文檔</param>
        /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
        public static bool CreateExcelFile_Batch(Dictionary<string, string> dirValues, string strType,
                        string strAgentName, ref string strPathFile, ref string strMsgID)
        {
            //*创建一个 Excel 实例
            ExcelApplication excel = new ExcelApplication();
            try
            {
                //* 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                //* 取要下載的資料
                string strInputDate = "";
                DataSet dtblData_Batch = BRExcel_File.getData_Batch(dirValues, strType, ref strInputDate);
                if (null == dtblData_Batch)
                    return false;
                if (dtblData_Batch.Tables.Count == 1 && dtblData_Batch.Tables[0].Rows.Count == 0 ||
                    dtblData_Batch.Tables.Count == 3 && dtblData_Batch.Tables[0].Rows.Count == 0 &&
                    dtblData_Batch.Tables[1].Rows.Count == 0 && dtblData_Batch.Tables[2].Rows.Count == 0)
                {
                    strMsgID = "01_03050400_001";
                    return false;
                }

                #region 匯入Excel文檔

                excel.Visible = false;//* 不显示 Excel 文件,如果为 true 则显示 Excel 文件
                excel.Application.DisplayAlerts = false;
                Workbook workbook = null;
                Worksheet sheet = null;
                string strExcelPathFile = "";
                switch (strType)
                {
                    case "1"://* 批次作業量統計
                        strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "Batch.xls";
                        workbook = excel.Workbooks.Open(strExcelPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                        sheet = (Worksheet)workbook.Sheets[1];
                        WriteDataToSheet_Batch(sheet, strAgentName, dtblData_Batch.Tables[0], strInputDate);
                        break;
                    case "2"://* 批次結果報表_成功
                        strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "BatchSuccess.xls";
                        workbook = excel.Workbooks.Open(strExcelPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                        sheet = (Worksheet)workbook.Sheets[1];
                        WriteDataToSheet_Success(sheet, strAgentName, dtblData_Batch.Tables[0], strInputDate);
                        break;
                    case "3"://* 批次結果報表_失敗
                        strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "BatchFault.xls";
                        workbook = excel.Workbooks.Open(strExcelPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                        sheet = (Worksheet)workbook.Sheets[1];
                        WriteDataToSheet_Fault(sheet, strAgentName, dtblData_Batch.Tables[0], strInputDate);
                        break;
                    case "4"://* 批次結果報表_未完成
                        strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "BatchNoComplete.xls";
                        workbook = excel.Workbooks.Open(strExcelPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                        sheet = (Worksheet)workbook.Sheets[1];
                        WriteDataToSheet_No_Complete(sheet, strAgentName, dtblData_Batch.Tables[0], strInputDate);
                        break;
                    case "5"://* 批次結果報表_全部
                        strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "BatchAll.xls";
                        workbook = excel.Workbooks.Open(strExcelPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                        //* 成功
                        sheet = (Worksheet)workbook.Sheets[1];
                        WriteDataToSheet_Success(sheet, strAgentName, dtblData_Batch.Tables[0], strInputDate);

                        //* 失敗
                        sheet = (Worksheet)workbook.Sheets[2];
                        WriteDataToSheet_Fault(sheet, strAgentName, dtblData_Batch.Tables[1], strInputDate);

                        //* 未完成
                        sheet = (Worksheet)workbook.Sheets[3];
                        WriteDataToSheet_No_Complete(sheet, strAgentName, dtblData_Batch.Tables[2], strInputDate);
                        break;
                }

                //* 保存文件到程序运行目录下
                strPathFile = strPathFile + @"\ExcelFile_BATCH_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                sheet.SaveAs(strPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                excel.ActiveWorkbook.Close(false, null, null); //* 关闭 Excel 文件且不保存
                return true;
                # endregion 匯入文檔結束
            }
            catch (Exception ex)
            {
                Logging.SaveLog(ELogLayer.BusinessRule, ex);
                throw ex;
            }
            finally
            {
                excel.Quit(); //* 退出 Excel
                excel = null; //* 将 Excel 实例设置为空
            }
        }

        /// <summary>
        /// 向Sheet寫入數據(批次作業量統計)
        /// </summary>
        /// <param name="sheet">寫入對象sheet</param>
        /// <param name="strAgentName">經辦人</param>
        private static void WriteDataToSheet_Batch(Worksheet sheet,
                    string strAgentName, DataTable dtblWriteData, string strInputDate)
        {
            Range range = null;//* 创建一个空的单元格对象
            int intRowIndexInSheet = 0;
            string[,] arrExportData = null;
            arrExportData = new string[dtblWriteData.Rows.Count + 1, 4];
            int intSuccess = 0;
            int intFault = 0;
            int intNoComplete = 0;
            for (int intLoop = 0; intLoop < dtblWriteData.Rows.Count; intLoop++)
            {
                //* 添加新的Sheet
                if (intLoop == 0)
                {
                    range = sheet.get_Range("B3", Missing.Value);
                    range.Value2 = strInputDate;

                    range = sheet.get_Range("B4", Missing.Value);
                    range.Value2 = strAgentName;

                    string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
                    range = sheet.get_Range("B5", Missing.Value);
                    range.Value2 = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);

                    intRowIndexInSheet = 7;
                }

                intRowIndexInSheet++;

                //* R02檔名日期
                arrExportData[intRowIndexInSheet - 8, 0] = dtblWriteData.Rows[intLoop]["batch_no"].ToString();
                //* 成功筆數
                arrExportData[intRowIndexInSheet - 8, 1] = Convert.ToInt32(dtblWriteData.Rows[intLoop]["sCount"]).ToString("N0");
                intSuccess += Convert.ToInt32(dtblWriteData.Rows[intLoop]["sCount"]);
                //* 失敗筆數
                arrExportData[intRowIndexInSheet - 8, 2] = Convert.ToInt32(dtblWriteData.Rows[intLoop]["fCount"]).ToString("N0");
                intFault += Convert.ToInt32(dtblWriteData.Rows[intLoop]["fCount"]);
                //* 未完成筆數
                arrExportData[intRowIndexInSheet - 8, 3] = Convert.ToInt32(dtblWriteData.Rows[intLoop]["nCount"]).ToString("N0");
                intNoComplete += Convert.ToInt32(dtblWriteData.Rows[intLoop]["nCount"]);
            }
            intRowIndexInSheet++;
            arrExportData[intRowIndexInSheet - 8, 0] = "總計";
            arrExportData[intRowIndexInSheet - 8, 1] = intSuccess.ToString("N0");
            arrExportData[intRowIndexInSheet - 8, 2] = intFault.ToString("N0");
            arrExportData[intRowIndexInSheet - 8, 3] = intNoComplete.ToString("N0");

            range = sheet.get_Range("A8", "A" + intRowIndexInSheet.ToString());
            range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中
            range.NumberFormatLocal = "@";

            range = sheet.get_Range("B8", "B" + intRowIndexInSheet.ToString());
            range.HorizontalAlignment = XlHAlign.xlHAlignRight;//* 设置单元格水平居中

            range = sheet.get_Range("C8", "C" + intRowIndexInSheet.ToString());
            range.HorizontalAlignment = XlHAlign.xlHAlignRight;//* 设置单元格水平居中

            range = sheet.get_Range("D8", "D" + intRowIndexInSheet.ToString());
            range.HorizontalAlignment = XlHAlign.xlHAlignRight;//* 设置单元格水平居中

            range = sheet.get_Range("A8", "D" + intRowIndexInSheet.ToString());
            range.Font.Size = 12;//字體大小
            range.Font.Name = "新細明體";
            range.Value2 = arrExportData;
            range.Borders.LineStyle = 1;
            range.EntireColumn.AutoFit();
        }

        /// <summary>
        /// 向Sheet寫入數據(批次結果報表_未完成報表)
        /// </summary>
        /// <param name="sheet">寫入對象sheet</param>
        /// <param name="strAgentName">經辦人</param>
        private static void WriteDataToSheet_No_Complete(Worksheet sheet,
                    string strAgentName, DataTable dtblWriteData, string strInputDate)
        {
            Range range = null;//* 创建一个空的单元格对象
            int intRowIndexInSheet = 0;
            string[,] arrExportData = null;
            arrExportData = new string[dtblWriteData.Rows.Count, 13];

            range = sheet.get_Range("B3", Missing.Value);
            range.Value2 = strInputDate;

            range = sheet.get_Range("B4", Missing.Value);
            range.Value2 = strAgentName;

            string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
            range = sheet.get_Range("B5", Missing.Value);
            range.Value2 = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);

            intRowIndexInSheet = 7;

            for (int intLoop = 0; intLoop < dtblWriteData.Rows.Count; intLoop++)
            {
                intRowIndexInSheet++;

                //* R02檔名日期
                arrExportData[intRowIndexInSheet - 8, 0] = dtblWriteData.Rows[intLoop]["Batch_no"].ToString();
                //* 收件編號
                arrExportData[intRowIndexInSheet - 8, 1] = dtblWriteData.Rows[intLoop]["Receive_Number"].ToString();
                //* 持卡人ID
                arrExportData[intRowIndexInSheet - 8, 2] = dtblWriteData.Rows[intLoop]["Cus_ID"].ToString();
                //* 失敗訊息
                arrExportData[intRowIndexInSheet - 8, 3] = "";
                //* 銀行代碼(行內3碼)
                arrExportData[intRowIndexInSheet - 8, 4] = dtblWriteData.Rows[intLoop]["Other_Bank_Code_S"].ToString();
                //* 帳號
                arrExportData[intRowIndexInSheet - 8, 5] = dtblWriteData.Rows[intLoop]["Other_Bank_Acc_No"].ToString();
                //* 扣款方式
                arrExportData[intRowIndexInSheet - 8, 6] = dtblWriteData.Rows[intLoop]["Other_Bank_Pay_Way"].ToString();
                //* 帳戶ID
                arrExportData[intRowIndexInSheet - 8, 7] = dtblWriteData.Rows[intLoop]["Other_Bank_Cus_ID"].ToString();
                //* 帳單週期
                arrExportData[intRowIndexInSheet - 8, 8] = dtblWriteData.Rows[intLoop]["bcycle_code"].ToString();
                //* 行動電話
                arrExportData[intRowIndexInSheet - 8, 9] = dtblWriteData.Rows[intLoop]["Mobile_Phone"].ToString();
                //* E-MAIL
                arrExportData[intRowIndexInSheet - 8, 10] = dtblWriteData.Rows[intLoop]["E_Mail"].ToString();
                //* 電子帳單
                arrExportData[intRowIndexInSheet - 8, 11] = dtblWriteData.Rows[intLoop]["E_Bill"].ToString();
                //* 鍵檔員姓名
                arrExportData[intRowIndexInSheet - 8, 12] = dtblWriteData.Rows[intLoop]["user_name"].ToString();
            }
            range = sheet.get_Range("A8", "A" + intRowIndexInSheet.ToString());
            range.NumberFormatLocal = "@";
            range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中

            range = sheet.get_Range("B8", "B" + intRowIndexInSheet.ToString());
            range.NumberFormatLocal = "@";
            range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中

            range = sheet.get_Range("C8", "C" + intRowIndexInSheet.ToString());
            range.NumberFormatLocal = "@";
            range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中

            range = sheet.get_Range("E8", "E" + intRowIndexInSheet.ToString());
            range.NumberFormatLocal = "@";
            range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中

            range = sheet.get_Range("F8", "F" + intRowIndexInSheet.ToString());
            range.NumberFormatLocal = "@";
            range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中

            range = sheet.get_Range("A8", "M" + intRowIndexInSheet.ToString());
            range.Font.Size = 12;//字體大小
            range.Font.Name = "新細明體";
            range.Value2 = arrExportData;
            range.Borders.LineStyle = 1;
            range.EntireColumn.AutoFit();

            intRowIndexInSheet = intRowIndexInSheet + 3;
            range = sheet.get_Range("A" + intRowIndexInSheet.ToString(), Missing.Value);
            range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中
            range.Value2 = "未完成筆數：";
            range.Font.Name = "新細明體";
            range.Font.Size = "12";
            range.Font.Bold = true;

            range = sheet.get_Range("B" + intRowIndexInSheet.ToString(), Missing.Value);
            range.HorizontalAlignment = XlHAlign.xlHAlignRight;//* 设置单元格水平居中
            range.Value2 = dtblWriteData.Rows.Count.ToString("N0");
            range.Font.Name = "新細明體";
            range.Font.Size = "12";
            range.Font.Bold = true;
        }

        /// <summary>
        /// 向Sheet寫入數據(批次結果報表_失敗)
        /// </summary>
        /// <param name="sheet">寫入對象sheet</param>
        /// <param name="strAgentName">經辦人</param>
        private static void WriteDataToSheet_Fault(Worksheet sheet,
                    string strAgentName, DataTable dtblWriteData, string strInputDate)
        {
            Range range = null;//* 创建一个空的单元格对象
            int intRowIndexInSheet = 0;
            string[,] arrExportData = null;
            arrExportData = new string[dtblWriteData.Rows.Count, 13];

            range = sheet.get_Range("B3", Missing.Value);
            range.Value2 = strInputDate;

            range = sheet.get_Range("B4", Missing.Value);
            range.Value2 = strAgentName;

            string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
            range = sheet.get_Range("B5", Missing.Value);
            range.Value2 = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);

            intRowIndexInSheet = 7;

            for (int intLoop = 0; intLoop < dtblWriteData.Rows.Count; intLoop++)
            {
                intRowIndexInSheet++;

                //* R02檔名日期
                arrExportData[intRowIndexInSheet - 8, 0] = dtblWriteData.Rows[intLoop]["Batch_no"].ToString();
                //* 收件編號
                arrExportData[intRowIndexInSheet - 8, 1] = dtblWriteData.Rows[intLoop]["Receive_Number"].ToString();
                //* 持卡人ID
                arrExportData[intRowIndexInSheet - 8, 2] = dtblWriteData.Rows[intLoop]["Cus_ID"].ToString();
                //* 失敗訊息
                if (!(dtblWriteData.Rows[intLoop]["Pcmc_Return_Code"].ToString().Trim() == "PAGE 00 OF 03" || dtblWriteData.Rows[intLoop]["Pcmc_Return_Code"].ToString().Trim() == "PAGE 02 OF 03"))
                {
                    if (dtblWriteData.Rows[intLoop]["Pcmc_Return_Code"].ToString().Trim() == "ERROR:10")
                    {
                        arrExportData[intRowIndexInSheet - 8, 3] = "案件類別為D不為P02D";
                    }
                    else if (dtblWriteData.Rows[intLoop]["Pcmc_Return_Code"].ToString().Trim() == "ERROR:9")
                    {
                        arrExportData[intRowIndexInSheet - 8, 3] = "週期件";
                    }
                    else if (dtblWriteData.Rows[intLoop]["Pcmc_Return_Code"].ToString().Trim() == "ERROR:0")
                    {
                        arrExportData[intRowIndexInSheet - 8, 3] = "人工刪除";
                    }
                    else if (dtblWriteData.Rows[intLoop]["Pcmc_Return_Code"].ToString().Trim() == "ERROR:O")
                    {
                        arrExportData[intRowIndexInSheet - 8, 3] = "案件類別為 \"O\" 類";
                    }
                    else
                    {
                        arrExportData[intRowIndexInSheet - 8, 3] = "PCMC失敗";
                    }
                }
                //* 銀行代碼(行內3碼)
                arrExportData[intRowIndexInSheet - 8, 4] = dtblWriteData.Rows[intLoop]["Other_Bank_Code_S"].ToString();
                //* 帳號
                arrExportData[intRowIndexInSheet - 8, 5] = dtblWriteData.Rows[intLoop]["Other_Bank_Acc_No"].ToString();
                //* 扣款方式
                arrExportData[intRowIndexInSheet - 8, 6] = dtblWriteData.Rows[intLoop]["Other_Bank_Pay_Way"].ToString();
                //* 帳戶ID
                arrExportData[intRowIndexInSheet - 8, 7] = dtblWriteData.Rows[intLoop]["Other_Bank_Cus_ID"].ToString();
                //* 帳單週期
                arrExportData[intRowIndexInSheet - 8, 8] = dtblWriteData.Rows[intLoop]["bcycle_code"].ToString();
                //* 行動電話
                arrExportData[intRowIndexInSheet - 8, 9] = dtblWriteData.Rows[intLoop]["Mobile_Phone"].ToString();
                //* E-MAIL
                arrExportData[intRowIndexInSheet - 8, 10] = dtblWriteData.Rows[intLoop]["E_Mail"].ToString();
                //* 電子帳單
                arrExportData[intRowIndexInSheet - 8, 11] = dtblWriteData.Rows[intLoop]["E_Bill"].ToString();
                //* 鍵檔員姓名
                arrExportData[intRowIndexInSheet - 8, 12] = dtblWriteData.Rows[intLoop]["user_name"].ToString();
            }
            range = sheet.get_Range("A8", "A" + intRowIndexInSheet.ToString());
            range.NumberFormatLocal = "@";
            range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中

            range = sheet.get_Range("B8", "B" + intRowIndexInSheet.ToString());
            range.NumberFormatLocal = "@";
            range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中

            range = sheet.get_Range("C8", "C" + intRowIndexInSheet.ToString());
            range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中

            range = sheet.get_Range("D8", "D" + intRowIndexInSheet.ToString());
            range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中

            range = sheet.get_Range("E8", "E" + intRowIndexInSheet.ToString());
            range.NumberFormatLocal = "@";
            range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中

            range = sheet.get_Range("F8", "F" + intRowIndexInSheet.ToString());
            range.NumberFormatLocal = "@";
            range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中

            range = sheet.get_Range("A8", "M" + intRowIndexInSheet.ToString());
            range.Font.Size = 12;//字體大小
            range.Font.Name = "新細明體";
            range.Value2 = arrExportData;
            range.Borders.LineStyle = 1;
            range.EntireColumn.AutoFit();

            intRowIndexInSheet = intRowIndexInSheet + 3;
            range = sheet.get_Range("A" + intRowIndexInSheet.ToString(), Missing.Value);
            range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中
            range.Value2 = "失敗筆數：";
            range.Font.Name = "新細明體";
            range.Font.Size = "12";
            range.Font.Bold = true;

            range = sheet.get_Range("B" + intRowIndexInSheet.ToString(), Missing.Value);
            range.HorizontalAlignment = XlHAlign.xlHAlignRight;//* 设置单元格水平居中
            range.Value2 = dtblWriteData.Rows.Count.ToString("N0");
            range.Font.Name = "新細明體";
            range.Font.Size = "12";
            range.Font.Bold = true;
        }

        /// <summary>
        /// 向Sheet寫入數據(批次結果報表_成功)
        /// </summary>
        /// <param name="sheet">寫入對象sheet</param>
        /// <param name="strAgentName">經辦人</param>
        private static void WriteDataToSheet_Success(Worksheet sheet,
                    string strAgentName, DataTable dtblWriteData, string strInputDate)
        {
            Range range = null;//* 创建一个空的单元格对象
            int intRowIndexInSheet = 0;
            string[,] arrExportData = null;
            arrExportData = new string[dtblWriteData.Rows.Count, 13];

            range = sheet.get_Range("B3", Missing.Value);
            range.Value2 = strInputDate;

            range = sheet.get_Range("B4", Missing.Value);
            range.Value2 = strAgentName;

            string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
            range = sheet.get_Range("B5", Missing.Value);
            range.Value2 = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);

            intRowIndexInSheet = 7;

            for (int intLoop = 0; intLoop < dtblWriteData.Rows.Count; intLoop++)
            {
                intRowIndexInSheet++;

                //* R02檔名日期
                arrExportData[intRowIndexInSheet - 8, 0] = dtblWriteData.Rows[intLoop]["Batch_no"].ToString();
                //* 收件編號
                arrExportData[intRowIndexInSheet - 8, 1] = dtblWriteData.Rows[intLoop]["Receive_Number"].ToString();
                //* 持卡人ID
                arrExportData[intRowIndexInSheet - 8, 2] = dtblWriteData.Rows[intLoop]["Cus_ID"].ToString();
                //* 1342
                if (dtblWriteData.Rows[intLoop]["C1342_Return_Code"].ToString().Trim() == "0001" || dtblWriteData.Rows[intLoop]["C1342_Return_Code"].ToString().Trim() == "")
                    arrExportData[intRowIndexInSheet - 8, 3] = "";
                else
                    arrExportData[intRowIndexInSheet - 8, 3] = "1342失敗";
                //* 銀行代碼(行內3碼)
                arrExportData[intRowIndexInSheet - 8, 4] = dtblWriteData.Rows[intLoop]["Other_Bank_Code_S"].ToString();
                //* 帳號
                arrExportData[intRowIndexInSheet - 8, 5] = dtblWriteData.Rows[intLoop]["Other_Bank_Acc_No"].ToString();
                //* 扣款方式
                arrExportData[intRowIndexInSheet - 8, 6] = dtblWriteData.Rows[intLoop]["Other_Bank_Pay_Way"].ToString();
                //* 帳戶ID
                arrExportData[intRowIndexInSheet - 8, 7] = dtblWriteData.Rows[intLoop]["Other_Bank_Cus_ID"].ToString();
                //* 帳單週期
                arrExportData[intRowIndexInSheet - 8, 8] = dtblWriteData.Rows[intLoop]["bcycle_code"].ToString();
                //* 行動電話
                arrExportData[intRowIndexInSheet - 8, 9] = dtblWriteData.Rows[intLoop]["Mobile_Phone"].ToString();
                //* E-MAIL
                arrExportData[intRowIndexInSheet - 8, 10] = dtblWriteData.Rows[intLoop]["E_Mail"].ToString();
                //* 電子帳單
                arrExportData[intRowIndexInSheet - 8, 11] = dtblWriteData.Rows[intLoop]["E_Bill"].ToString();
                //* 鍵檔員姓名
                arrExportData[intRowIndexInSheet - 8, 12] = dtblWriteData.Rows[intLoop]["user_name"].ToString();
            }
            range = sheet.get_Range("A8", "A" + intRowIndexInSheet.ToString());
            range.NumberFormatLocal = "@";
            range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中

            range = sheet.get_Range("B8", "B" + intRowIndexInSheet.ToString());
            range.NumberFormatLocal = "@";
            range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中

            range = sheet.get_Range("C8", "C" + intRowIndexInSheet.ToString());
            range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中

            range = sheet.get_Range("D8", "D" + intRowIndexInSheet.ToString());
            range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中

            range = sheet.get_Range("E8", "E" + intRowIndexInSheet.ToString());
            range.NumberFormatLocal = "@";
            range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中

            range = sheet.get_Range("F8", "F" + intRowIndexInSheet.ToString());
            range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中

            range = sheet.get_Range("H8", "H" + intRowIndexInSheet.ToString());
            range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中

            range = sheet.get_Range("M8", "M" + intRowIndexInSheet.ToString());
            range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中

            range = sheet.get_Range("A8", "M" + intRowIndexInSheet.ToString());
            range.Font.Size = 12;//字體大小
            range.Font.Name = "新細明體";
            range.Value2 = arrExportData;
            range.Borders.LineStyle = 1;
            range.EntireColumn.AutoFit();

            intRowIndexInSheet = intRowIndexInSheet + 3;
            range = sheet.get_Range("A" + intRowIndexInSheet.ToString(), Missing.Value);
            range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中
            range.Value2 = "成功筆數：";
            range.Font.Name = "新細明體";
            range.Font.Size = "12";
            range.Font.Bold = true;

            range = sheet.get_Range("B" + intRowIndexInSheet.ToString(), Missing.Value);
            range.HorizontalAlignment = XlHAlign.xlHAlignRight;//* 设置单元格水平居中
            range.Value2 = dtblWriteData.Rows.Count.ToString("N0");
            range.Font.Name = "新細明體";
            range.Font.Size = "12";
            range.Font.Bold = true;
        }


        /// <summary>
        /// P02授權未回覆報表
        /// </summary>
        /// <param name="dirValues">查詢條件</param>
        /// <param name="strAgentName">組別代號</param>
        /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
        /// <param name="strMsgID">返回消息ID</param>
        /// <param name="strPathFile">服務器端生成的Excel文檔</param>
        /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
        public static bool CreateExcelFile_P02(Dictionary<string, string> dirValues,
                        string strAgentName, ref string strPathFile, ref string strMsgID)
        {
            //*创建一个 Excel 实例
            ExcelApplication excel = new ExcelApplication();
            try
            {
                //* 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                //* 取要下載的資料
                string strInputDate = "";
                DataTable dtblData_P02 = BRExcel_File.getData_P02(dirValues, ref strInputDate);
                if (null == dtblData_P02)
                    return false;
                if (dtblData_P02.Rows.Count == 0)
                {
                    strMsgID = "01_03050300_001";
                    return false;
                }

                #region 匯入Excel文檔

                excel.Visible = false;//* 不显示 Excel 文件,如果为 true 则显示 Excel 文件
                excel.Application.DisplayAlerts = false;
                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "P02NOReply.xls";
                Workbook workbook = excel.Workbooks.Open(strExcelPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value);

                Range range = null;//* 创建一个空的单元格对象
                int intRowIndexInSheet = 0;
                Worksheet sheet = null;
                string[,] arrExportData = null;
                arrExportData = new string[dtblData_P02.Rows.Count, 9];
                for (int intLoop = 0; intLoop < dtblData_P02.Rows.Count; intLoop++)
                {
                    //* 添加新的Sheet
                    if (intLoop == 0)
                    {
                        sheet = (Worksheet)workbook.Sheets[1];

                        range = sheet.get_Range("B3", Missing.Value);
                        range.Value2 = strInputDate;

                        range = sheet.get_Range("B4", Missing.Value);
                        range.Value2 = strAgentName;

                        string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
                        range = sheet.get_Range("B5", Missing.Value);
                        range.Value2 = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);

                        intRowIndexInSheet = 7;
                    }

                    intRowIndexInSheet++;

                    //* 收件編號
                    arrExportData[intRowIndexInSheet - 8, 0] = dtblData_P02.Rows[intLoop]["Receive_Number"].ToString();
                    //* 收受行（金支的7碼）
                    arrExportData[intRowIndexInSheet - 8, 1] = dtblData_P02.Rows[intLoop]["Other_Bank_Code_L"].ToString();
                    //* 收受行名稱(核印行)
                    arrExportData[intRowIndexInSheet - 8, 2] = dtblData_P02.Rows[intLoop]["BankName"].ToString();
                    //* 委繳戶統編/身分證字號
                    arrExportData[intRowIndexInSheet - 8, 3] = dtblData_P02.Rows[intLoop]["Other_Bank_Cus_ID"].ToString();
                    //* 委繳戶帳號
                    arrExportData[intRowIndexInSheet - 8, 4] = dtblData_P02.Rows[intLoop]["Other_Bank_Acc_No"].ToString();
                    //* 持卡人ID
                    arrExportData[intRowIndexInSheet - 8, 5] = dtblData_P02.Rows[intLoop]["Cus_ID"].ToString();
                    //* 申請類別
                    arrExportData[intRowIndexInSheet - 8, 6] = dtblData_P02.Rows[intLoop]["Apply_Type"].ToString();
                }

                range = sheet.get_Range("A8", "A" + intRowIndexInSheet.ToString());
                range.NumberFormatLocal = "@";

                range = sheet.get_Range("B8", "B" + intRowIndexInSheet.ToString());
                range.NumberFormatLocal = "@";

                range = sheet.get_Range("E8", "E" + intRowIndexInSheet.ToString());
                range.NumberFormatLocal = "@";

                range = sheet.get_Range("A8", "G" + intRowIndexInSheet.ToString());
                range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中
                range.Font.Size = 12;//字體大小
                range.Font.Name = "新細明體";
                range.Value2 = arrExportData;
                range.Borders.LineStyle = 1;
                range.EntireColumn.AutoFit();

                intRowIndexInSheet += 5;
                //* 成功筆數
                range = sheet.get_Range("A" + intRowIndexInSheet.ToString(), Missing.Value);
                range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中
                range.Value2 = "總筆數：";
                range.Font.Name = "新細明體";
                range.Font.Size = "12";
                range.Font.Bold = true;

                range = sheet.get_Range("B" + intRowIndexInSheet.ToString(), Missing.Value);
                range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中
                range.Value2 = dtblData_P02.Rows.Count.ToString("N0");
                range.Font.Name = "新細明體";
                range.Font.Size = "12";
                range.Font.Bold = true;

                intRowIndexInSheet += 4;

                range = sheet.get_Range("A" + intRowIndexInSheet.ToString(), Missing.Value);
                range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中
                range.Value2 = "*依行庫由小至大排序";
                range.Font.Name = "Times New Roman";
                range.Font.Size = "12";
                range.Font.ColorIndex = 3;

                //* 保存文件到程序运行目录下
                strPathFile = strPathFile + @"\ExcelFile_P02_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                sheet.SaveAs(strPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                excel.ActiveWorkbook.Close(false, null, null); //* 关闭 Excel 文件且不保存
                return true;
                # endregion 匯入文檔結束
            }
            catch (Exception ex)
            {
                Logging.SaveLog(ELogLayer.BusinessRule, ex);
                throw ex;
            }
            finally
            {
                excel.Quit(); //* 退出 Excel
                excel = null; //* 将 Excel 实例设置为空
            }
        }

        public static bool CreateExcelFile_PointSummary(string strSDATE, string strEDATE,
                                            ref string strPathFile, ref string strMsgID)
        {
            //*创建一个 Excel 实例
            ExcelApplication excel = new ExcelApplication();
            try
            {
                //* 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                //* 取要下載的資料                
                int iTCount = 0;
                DataTable dtblData = BRRedeemSet.SelectReport(strSDATE, strEDATE, 0, 0, ref iTCount);
                if (null == dtblData)
                    return false;
                if (dtblData.Rows.Count == 0)
                {
                    strMsgID = "01_03050300_001";
                    return false;
                }

                #region 匯入Excel文檔

                excel.Visible = false;//* 不显示 Excel 文件,如果为 true 则显示 Excel 文件
                excel.Application.DisplayAlerts = false;
                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "PointSummary.xls";
                Workbook workbook = excel.Workbooks.Open(strExcelPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value);

                Range range = null;//* 创建一个空的单元格对象
                int intRowIndexInSheet = 0;
                Worksheet sheet = null;
                string[,] arrExportData = null;
                arrExportData = new string[dtblData.Rows.Count, 9];
                for (int intLoop = 0; intLoop < dtblData.Rows.Count; intLoop++)
                {
                    //* 添加新的Sheet
                    if (intLoop == 0)
                    {
                        sheet = (Worksheet)workbook.Sheets[1];

                        range = sheet.get_Range("B3", Missing.Value);
                        range.Value2 = strSDATE.Trim() + "-" + strEDATE.Trim();

                        intRowIndexInSheet = 5;
                    }

                    intRowIndexInSheet++;

                    //* 收件編號
                    arrExportData[intRowIndexInSheet - 6, 0] = dtblData.Rows[intLoop]["RECEIVE_NUMBER"].ToString();
                    //* 作業種類(Award or Redeem)
                    arrExportData[intRowIndexInSheet - 6, 1] = dtblData.Rows[intLoop]["WTYPE"].ToString();
                    //* 1Key經辦
                    arrExportData[intRowIndexInSheet - 6, 2] = dtblData.Rows[intLoop]["1KEYUID"].ToString();
                    //* 總使用時間
                    arrExportData[intRowIndexInSheet - 6, 3] = dtblData.Rows[intLoop]["1KEYUTIME"].ToString();
                    //* 2Key經辦
                    arrExportData[intRowIndexInSheet - 6, 4] = dtblData.Rows[intLoop]["2KEYUID"].ToString();
                    //* 總使用時間
                    arrExportData[intRowIndexInSheet - 6, 5] = dtblData.Rows[intLoop]["2KEYUTIME"].ToString();
                    //* 是否一致
                    arrExportData[intRowIndexInSheet - 6, 6] = dtblData.Rows[intLoop]["ISSAME"].ToString();
                    //* 不一致欄位數
                    arrExportData[intRowIndexInSheet - 6, 7] = dtblData.Rows[intLoop]["DIFFNUM"].ToString();
                    //* 更改經辦
                    arrExportData[intRowIndexInSheet - 6, 8] = dtblData.Rows[intLoop]["EDITUID"].ToString();

                }

                range = sheet.get_Range("D6", "D" + intRowIndexInSheet.ToString());
                range.NumberFormatLocal = "@";

                range = sheet.get_Range("F6", "F" + intRowIndexInSheet.ToString());
                range.NumberFormatLocal = "@";

                range = sheet.get_Range("H6", "H" + intRowIndexInSheet.ToString());
                range.NumberFormatLocal = "@";

                range = sheet.get_Range("A6", "I" + intRowIndexInSheet.ToString());
                range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中
                range.Font.Size = 12;//字體大小
                range.Font.Name = "新細明體";
                range.Value2 = arrExportData;
                range.Borders.LineStyle = 1;
                range.EntireColumn.AutoFit();

                //intRowIndexInSheet += 5;
                ////* 成功筆數
                //range = sheet.get_Range("A" + intRowIndexInSheet.ToString(), Missing.Value);
                //range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中
                //range.Value2 = "總筆數：";
                //range.Font.Name = "新細明體";
                //range.Font.Size = "12";
                //range.Font.Bold = true;

                //range = sheet.get_Range("B" + intRowIndexInSheet.ToString(), Missing.Value);
                //range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中
                //range.Value2 = dtblData.Rows.Count.ToString("N0");
                //range.Font.Name = "新細明體";
                //range.Font.Size = "12";
                //range.Font.Bold = true;

                //intRowIndexInSheet += 4;

                //range = sheet.get_Range("A" + intRowIndexInSheet.ToString(), Missing.Value);
                //range.HorizontalAlignment = XlHAlign.xlHAlignLeft;//* 设置单元格水平居中
                //range.Value2 = "*依行庫由小至大排序";
                //range.Font.Name = "Times New Roman";
                //range.Font.Size = "12";
                //range.Font.ColorIndex = 3;

                //* 保存文件到程序运行目录下
                strPathFile = strPathFile + @"\ExcelFile_PointSummary_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                sheet.SaveAs(strPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                excel.ActiveWorkbook.Close(false, null, null); //* 关闭 Excel 文件且不保存
                return true;
                # endregion 匯入文檔結束
            }
            catch (Exception ex)
            {
                Logging.SaveLog(ELogLayer.BusinessRule, ex);
                throw ex;
            }
            finally
            {
                excel.Quit(); //* 退出 Excel
                excel = null; //* 将 Excel 实例设置为空
            }
        }

        //add by linhuanhuang start
        public static bool CreateExcelFile_DetailRedeem(string strRECEIVENUMBER, string strReceiveDate, string strMERCHANT,
                                            ref string strPathFile, ref string strMsgID)
        {
            //*创建一个 Excel 实例
            ExcelApplication excel = new ExcelApplication();
            try
            {
                //* 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                //* 取要下載的資料
                int iTCount = 0;
                DataTable dtblData = BRRedeem3270.GetDetailReport(strRECEIVENUMBER, strReceiveDate, strMERCHANT, 0, 0, ref iTCount).Tables[0];
                if (null == dtblData)
                    return false;
                if (dtblData.Rows.Count == 0)
                {
                    strMsgID = "01_03050300_001";
                    return false;
                }

                #region 匯入Excel文檔

                excel.Visible = false;//* 不显示 Excel 文件,如果为 true 则显示 Excel 文件
                excel.Application.DisplayAlerts = false;
                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "DetailRedeem.xls";
                Workbook workbook = excel.Workbooks.Open(strExcelPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value);

                Range range = null;//* 创建一个空的单元格对象
                int intRowIndexInSheet = 0;
                Worksheet sheet = null;
                string[,] arrExportData = null;
                arrExportData = new string[dtblData.Rows.Count, 26];

                for (int intLoop = 0; intLoop < dtblData.Rows.Count; intLoop++)
                {
                    //* 添加新的Sheet
                    if (intLoop == 0)
                    {
                        sheet = (Worksheet)workbook.Sheets[1];

                        intRowIndexInSheet = 3;
                    }

                    intRowIndexInSheet++;

                    //收件編號	
                    arrExportData[intRowIndexInSheet - 4, 0] = dtblData.Rows[intLoop]["RECEIVE_NUMBER"].ToString().Trim();
                    //作業種類	
                    arrExportData[intRowIndexInSheet - 4, 1] = dtblData.Rows[intLoop]["FUNCTION_CODE"].ToString().Trim();
                    //MSG-SEQ	
                    arrExportData[intRowIndexInSheet - 4, 2] = dtblData.Rows[intLoop]["MSG_SEQ"].ToString().Trim();
                    //MSG-ERR	
                    arrExportData[intRowIndexInSheet - 4, 3] = dtblData.Rows[intLoop]["MSG_ERR"].ToString().Trim();
                    //ORG	
                    arrExportData[intRowIndexInSheet - 4, 4] = dtblData.Rows[intLoop]["IN_ORG"].ToString().Trim();
                    //MERCHANT	
                    arrExportData[intRowIndexInSheet - 4, 5] = dtblData.Rows[intLoop]["IN_MERCHANT"].ToString().Trim();
                    //PROD-CODE	
                    arrExportData[intRowIndexInSheet - 4, 6] = dtblData.Rows[intLoop]["IN_PROD_CODE"].ToString().Trim();
                    //CARD-TYPE	
                    arrExportData[intRowIndexInSheet - 4, 7] = dtblData.Rows[intLoop]["IN_CARD_TYPE"].ToString().Trim();
                    //PROGRAM	
                    arrExportData[intRowIndexInSheet - 4, 8] = dtblData.Rows[intLoop]["PROGID"].ToString().Trim();
                    //本行分攤比例%	
                    arrExportData[intRowIndexInSheet - 4, 9] = dtblData.Rows[intLoop]["MER_RATE"].ToString().Trim();
                    //長期活動 - 折抵上限	
                    arrExportData[intRowIndexInSheet - 4, 10] = dtblData.Rows[intLoop]["LIMITR"].ToString().Trim();
                    //長期活動 - 點數/金額抵用比率	
                    arrExportData[intRowIndexInSheet - 4, 11] = dtblData.Rows[intLoop]["CHPOINT"].ToString().Trim();
                    //長期活動 - 可折抵金額	
                    arrExportData[intRowIndexInSheet - 4, 12] = dtblData.Rows[intLoop]["CHAMT"].ToString().Trim();
                    //短期活動 - 類型	
                    arrExportData[intRowIndexInSheet - 4, 13] = dtblData.Rows[intLoop]["USER_EXIT"].ToString().Trim();
                    //短期活動 - 活動起日	
                    arrExportData[intRowIndexInSheet - 4, 14] = dtblData.Rows[intLoop]["STARTU"].ToString().Trim();
                    //短期活動 - 活動迄日	
                    arrExportData[intRowIndexInSheet - 4, 15] = dtblData.Rows[intLoop]["ENDU"].ToString().Trim();
                    //短期活動 - 指定時間之短期促銷	
                    arrExportData[intRowIndexInSheet - 4, 16] = dtblData.Rows[intLoop]["CYLCO"].ToString().Trim();
                    //短期活動 - 折抵上限	
                    arrExportData[intRowIndexInSheet - 4, 17] = dtblData.Rows[intLoop]["LIMITU"].ToString().Trim();
                    //短期活動 - 點數/金額抵用比率	
                    arrExportData[intRowIndexInSheet - 4, 18] = dtblData.Rows[intLoop]["CHPOINTU"].ToString().Trim();
                    //短期活動 - 可折抵金額	
                    arrExportData[intRowIndexInSheet - 4, 19] = dtblData.Rows[intLoop]["CHAMTU"].ToString().Trim();
                    //生日活動 - 類型	
                    arrExportData[intRowIndexInSheet - 4, 20] = dtblData.Rows[intLoop]["BIRTH"].ToString().Trim();
                    //生日活動 - 活動起日	
                    arrExportData[intRowIndexInSheet - 4, 21] = dtblData.Rows[intLoop]["STARTB"].ToString().Trim();
                    //生日活動 - 活動迄日	
                    arrExportData[intRowIndexInSheet - 4, 22] = dtblData.Rows[intLoop]["ENDB"].ToString().Trim();
                    //生日活動 - 折抵上限	
                    arrExportData[intRowIndexInSheet - 4, 23] = dtblData.Rows[intLoop]["LIMITB"].ToString().Trim();
                    //生日活動 - 點數/金額抵用比率	
                    arrExportData[intRowIndexInSheet - 4, 24] = dtblData.Rows[intLoop]["CHPOINTB"].ToString().Trim();
                    //生日活動 - 可折抵金額	
                    arrExportData[intRowIndexInSheet - 4, 25] = dtblData.Rows[intLoop]["CHAMTB"].ToString().Trim();

                }

                range = sheet.get_Range("A4", "Z" + intRowIndexInSheet.ToString());
                range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中
                range.Font.Size = 12;//字體大小
                range.Font.Name = "新細明體";
                range.Value2 = arrExportData;
                range.Borders.LineStyle = 1;
                range.EntireColumn.AutoFit();

                //* 保存文件到程序运行目录下
                strPathFile = strPathFile + @"\ExcelFile_DetailRedeem_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                sheet.SaveAs(strPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                excel.ActiveWorkbook.Close(false, null, null); //* 关闭 Excel 文件且不保存
                return true;

                #endregion 匯入Excel文檔
            }
            catch (Exception ex)
            {
                Logging.SaveLog(ELogLayer.BusinessRule, ex);
                throw ex;
            }
            finally
            {
                excel.Quit(); //* 退出 Excel
                excel = null; //* 将 Excel 实例设置为空
            }
        }

        public static bool CreateExcelFile_DetailAward(string strRECEIVENUMBER, string strReceiveDate,
                                            ref string strPathFile, ref string strMsgID)
        {
            //*创建一个 Excel 实例
            ExcelApplication excel = new ExcelApplication();
            try
            {
                //* 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                //* 取要下載的資料
                int iTCount = 0;
                DataTable dtblData = BRAwardSet3270.GetDetailReport(strRECEIVENUMBER, strReceiveDate, 0, 0, ref iTCount).Tables[0];
                if (null == dtblData)
                    return false;
                if (dtblData.Rows.Count == 0)
                {
                    strMsgID = "01_03050300_001";
                    return false;
                }

                #region 匯入Excel文檔

                excel.Visible = false;//* 不显示 Excel 文件,如果为 true 则显示 Excel 文件
                excel.Application.DisplayAlerts = false;
                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "DetailAward.xls";
                Workbook workbook = excel.Workbooks.Open(strExcelPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value);

                Range range = null;//* 创建一个空的单元格对象
                int intRowIndexInSheet = 0;
                Worksheet sheet = null;
                string[,] arrExportData = null;
                arrExportData = new string[dtblData.Rows.Count, 71];

                for (int intLoop = 0; intLoop < dtblData.Rows.Count; intLoop++)
                {
                    //* 添加新的Sheet
                    if (intLoop == 0)
                    {
                        sheet = (Worksheet)workbook.Sheets[1];

                        intRowIndexInSheet = 3;
                    }

                    intRowIndexInSheet++;

                    //收件編號	
                    arrExportData[intRowIndexInSheet - 4, 0] = dtblData.Rows[intLoop]["RECEIVE_NUMBER"].ToString().Trim();
                    //作業種類	
                    arrExportData[intRowIndexInSheet - 4, 1] = dtblData.Rows[intLoop]["FUNCTION_CODE"].ToString().Trim();
                    //MSG-SEQ	
                    arrExportData[intRowIndexInSheet - 4, 2] = dtblData.Rows[intLoop]["MSG_SEQ"].ToString().Trim();
                    //MSG-ERR	
                    arrExportData[intRowIndexInSheet - 4, 3] = dtblData.Rows[intLoop]["MSG_TYPE"].ToString().Trim();
                    //ORG	
                    arrExportData[intRowIndexInSheet - 4, 4] = dtblData.Rows[intLoop]["ORG"].ToString().Trim();
                    //PROG-NO	
                    arrExportData[intRowIndexInSheet - 4, 5] = dtblData.Rows[intLoop]["PROG_ID"].ToString().Trim();
                    //PARTNER-NO	
                    arrExportData[intRowIndexInSheet - 4, 6] = dtblData.Rows[intLoop]["PARTNER_ID"].ToString().Trim();
                    //CARD-TYPE	
                    arrExportData[intRowIndexInSheet - 4, 7] = dtblData.Rows[intLoop]["ACCUMULATION_TYPE"].ToString().Trim();
                    //TC-CODE(+)	
                    arrExportData[intRowIndexInSheet - 4, 8] = dtblData.Rows[intLoop]["TC_CODE1"].ToString().Trim();
                    //TC-CODE(-)	
                    arrExportData[intRowIndexInSheet - 4, 9] = dtblData.Rows[intLoop]["TC_CODE2"].ToString().Trim();
                    //MCC CODE - 1	
                    arrExportData[intRowIndexInSheet - 4, 10] = dtblData.Rows[intLoop]["MCC_FROM_1"].ToString().Trim();
                    //MCC CODE - 2	
                    arrExportData[intRowIndexInSheet - 4, 11] = dtblData.Rows[intLoop]["MCC_FROM_2"].ToString().Trim();
                    //MCC CODE - 3	
                    arrExportData[intRowIndexInSheet - 4, 12] = dtblData.Rows[intLoop]["MCC_FROM_3"].ToString().Trim();
                    //MCC CODE - 4	
                    arrExportData[intRowIndexInSheet - 4, 13] = dtblData.Rows[intLoop]["MCC_FROM_4"].ToString().Trim();
                    //MCC CODE - 5	
                    arrExportData[intRowIndexInSheet - 4, 14] = dtblData.Rows[intLoop]["MCC_FROM_5"].ToString().Trim();
                    //MCC CODE - 6	
                    arrExportData[intRowIndexInSheet - 4, 15] = dtblData.Rows[intLoop]["MCC_FROM_6"].ToString().Trim();
                    //MCC CODE - 7	
                    arrExportData[intRowIndexInSheet - 4, 16] = dtblData.Rows[intLoop]["MCC_FROM_7"].ToString().Trim();
                    //MCC CODE - 8	
                    arrExportData[intRowIndexInSheet - 4, 17] = dtblData.Rows[intLoop]["MCC_FROM_8"].ToString().Trim();
                    //MCC CODE - 9	
                    arrExportData[intRowIndexInSheet - 4, 18] = dtblData.Rows[intLoop]["MCC_FROM_9"].ToString().Trim();
                    //MCC CODE - 10	
                    arrExportData[intRowIndexInSheet - 4, 19] = dtblData.Rows[intLoop]["MCC_FROM_10"].ToString().Trim();
                    //消費地區FLAG	
                    arrExportData[intRowIndexInSheet - 4, 20] = dtblData.Rows[intLoop]["COUNTRY_CODE_IND"].ToString().Trim();
                    //消費地區代碼 - 1	
                    arrExportData[intRowIndexInSheet - 4, 21] = dtblData.Rows[intLoop]["COUNTRY_CODE1"].ToString().Trim();
                    //消費地區代碼 - 2	
                    arrExportData[intRowIndexInSheet - 4, 22] = dtblData.Rows[intLoop]["COUNTRY_CODE2"].ToString().Trim();
                    //消費地區代碼 - 3	
                    arrExportData[intRowIndexInSheet - 4, 23] = dtblData.Rows[intLoop]["COUNTRY_CODE3"].ToString().Trim();
                    //消費地區代碼 - 4	
                    arrExportData[intRowIndexInSheet - 4, 24] = dtblData.Rows[intLoop]["COUNTRY_CODE4"].ToString().Trim();
                    //消費地區代碼 - 5	
                    arrExportData[intRowIndexInSheet - 4, 25] = dtblData.Rows[intLoop]["COUNTRY_CODE5"].ToString().Trim();
                    //消費地區代碼 - 6	
                    arrExportData[intRowIndexInSheet - 4, 26] = dtblData.Rows[intLoop]["COUNTRY_CODE6"].ToString().Trim();
                    //消費地區代碼 - 7	
                    arrExportData[intRowIndexInSheet - 4, 27] = dtblData.Rows[intLoop]["COUNTRY_CODE7"].ToString().Trim();
                    //消費地區代碼 - 8	
                    arrExportData[intRowIndexInSheet - 4, 28] = dtblData.Rows[intLoop]["COUNTRY_CODE8"].ToString().Trim();
                    //消費地區代碼 - 9	
                    arrExportData[intRowIndexInSheet - 4, 29] = dtblData.Rows[intLoop]["COUNTRY_CODE9"].ToString().Trim();
                    //消費地區代碼 - 10	
                    arrExportData[intRowIndexInSheet - 4, 30] = dtblData.Rows[intLoop]["COUNTRY_CODE10"].ToString().Trim();
                    //長期活動-正卡 - 給點方式	
                    arrExportData[intRowIndexInSheet - 4, 31] = dtblData.Rows[intLoop]["BASIC_CALC_IND"].ToString().Trim();
                    //長期活動-正卡 - AMT1	
                    arrExportData[intRowIndexInSheet - 4, 32] = dtblData.Rows[intLoop]["BASIC_TIER_AMT1"].ToString().Trim();
                    //長期活動-正卡 - RATE1	
                    arrExportData[intRowIndexInSheet - 4, 33] = dtblData.Rows[intLoop]["BASIC_TIER_RATE1"].ToString().Trim();
                    //長期活動-正卡 - AMT2	
                    arrExportData[intRowIndexInSheet - 4, 34] = dtblData.Rows[intLoop]["BASIC_TIER_AMT2"].ToString().Trim();
                    //長期活動-正卡 - RATE2	
                    arrExportData[intRowIndexInSheet - 4, 35] = dtblData.Rows[intLoop]["BASIC_TIER_RATE2"].ToString().Trim();
                    //長期活動-正卡 - AMT3	
                    arrExportData[intRowIndexInSheet - 4, 36] = dtblData.Rows[intLoop]["BASIC_TIER_AMT3"].ToString().Trim();
                    //長期活動-正卡 - RATE3	
                    arrExportData[intRowIndexInSheet - 4, 37] = dtblData.Rows[intLoop]["BASIC_TIER_RATE3"].ToString().Trim();
                    //長期活動-正卡 - AMT4	
                    arrExportData[intRowIndexInSheet - 4, 38] = dtblData.Rows[intLoop]["BASIC_TIER_AMT4"].ToString().Trim();
                    //長期活動-正卡 - RATE4	
                    arrExportData[intRowIndexInSheet - 4, 39] = dtblData.Rows[intLoop]["BASIC_TIER_RATE4"].ToString().Trim();
                    //長期活動-附卡 - 給點方式	
                    arrExportData[intRowIndexInSheet - 4, 40] = dtblData.Rows[intLoop]["SUPP_BASIC_CAL_IND"].ToString().Trim();
                    //長期活動-附卡 - AMT1	
                    arrExportData[intRowIndexInSheet - 4, 41] = dtblData.Rows[intLoop]["SUPP_BASIC_TIER_AMT1"].ToString().Trim();
                    //長期活動-附卡 - RATE1	
                    arrExportData[intRowIndexInSheet - 4, 42] = dtblData.Rows[intLoop]["SUPP_BASIC_TIER_RATE1"].ToString().Trim();
                    //長期活動-附卡 - AMT2	
                    arrExportData[intRowIndexInSheet - 4, 43] = dtblData.Rows[intLoop]["SUPP_BASIC_TIER_AMT2"].ToString().Trim();
                    //長期活動-附卡 - RATE2	
                    arrExportData[intRowIndexInSheet - 4, 44] = dtblData.Rows[intLoop]["SUPP_BASIC_TIER_RATE2"].ToString().Trim();
                    //長期活動-附卡 - AMT3	
                    arrExportData[intRowIndexInSheet - 4, 45] = dtblData.Rows[intLoop]["SUPP_BASIC_TIER_AMT3"].ToString().Trim();
                    //長期活動-附卡 - RATE3	
                    arrExportData[intRowIndexInSheet - 4, 46] = dtblData.Rows[intLoop]["SUPP_BASIC_TIER_RATE3"].ToString().Trim();
                    //長期活動-附卡 - AMT4	
                    arrExportData[intRowIndexInSheet - 4, 47] = dtblData.Rows[intLoop]["SUPP_BASIC_TIER_AMT4"].ToString().Trim();
                    //長期活動-附卡 - RATE4	
                    arrExportData[intRowIndexInSheet - 4, 48] = dtblData.Rows[intLoop]["SUPP_BASIC_TIER_RATE4"].ToString().Trim();
                    //促銷活動-短期活動 - 活動起日	
                    arrExportData[intRowIndexInSheet - 4, 49] = dtblData.Rows[intLoop]["PROMO_START_DTE"].ToString().Trim();
                    //促銷活動-短期活動 - 活動迄日	
                    arrExportData[intRowIndexInSheet - 4, 50] = dtblData.Rows[intLoop]["PROMO_END_DTE"].ToString().Trim();
                    //促銷活動-短期活動 - 給點方式	
                    arrExportData[intRowIndexInSheet - 4, 51] = dtblData.Rows[intLoop]["PROMO_CALC_IND"].ToString().Trim();
                    //促銷活動-短期活動 - AMT1	
                    arrExportData[intRowIndexInSheet - 4, 52] = dtblData.Rows[intLoop]["PROMO_TIER_AMT1"].ToString().Trim();
                    //促銷活動-短期活動 - RATE1	
                    arrExportData[intRowIndexInSheet - 4, 53] = dtblData.Rows[intLoop]["PROMO_TIER_RATE1"].ToString().Trim();
                    //促銷活動-短期活動 - AMT2	
                    arrExportData[intRowIndexInSheet - 4, 54] = dtblData.Rows[intLoop]["PROMO_TIER_AMT2"].ToString().Trim();
                    //促銷活動-短期活動 - RATE2	
                    arrExportData[intRowIndexInSheet - 4, 55] = dtblData.Rows[intLoop]["PROMO_TIER_RATE2"].ToString().Trim();
                    //促銷活動-短期活動 - AMT3	
                    arrExportData[intRowIndexInSheet - 4, 56] = dtblData.Rows[intLoop]["PROMO_TIER_AMT3"].ToString().Trim();
                    //促銷活動-短期活動 - RATE3	
                    arrExportData[intRowIndexInSheet - 4, 57] = dtblData.Rows[intLoop]["PROMO_TIER_RATE3"].ToString().Trim();
                    //促銷活動-短期活動 - AMT4	
                    arrExportData[intRowIndexInSheet - 4, 58] = dtblData.Rows[intLoop]["PROMO_TIER_AMT4"].ToString().Trim();
                    //促銷活動-短期活動 - RATE4	
                    arrExportData[intRowIndexInSheet - 4, 59] = dtblData.Rows[intLoop]["PROMO_TIER_RATE4"].ToString().Trim();
                    //促銷活動-生日活動 - 活動起日	
                    arrExportData[intRowIndexInSheet - 4, 60] = dtblData.Rows[intLoop]["BTHDTE_START"].ToString().Trim();
                    //促銷活動-生日活動 - 活動迄日	
                    arrExportData[intRowIndexInSheet - 4, 61] = dtblData.Rows[intLoop]["BTHDTE_END"].ToString().Trim();
                    //促銷活動-生日活動 - 給點方式	
                    arrExportData[intRowIndexInSheet - 4, 62] = dtblData.Rows[intLoop]["BTHDTE_CALC_IND"].ToString().Trim();
                    //促銷活動-生日活動 - AMT1	
                    arrExportData[intRowIndexInSheet - 4, 63] = dtblData.Rows[intLoop]["BTHDTE_TIER_AMT1"].ToString().Trim();
                    //促銷活動-生日活動 - RATE1	
                    arrExportData[intRowIndexInSheet - 4, 64] = dtblData.Rows[intLoop]["BTHDTE_TIER_RATE1"].ToString().Trim();
                    //促銷活動-生日活動 - AMT2	
                    arrExportData[intRowIndexInSheet - 4, 65] = dtblData.Rows[intLoop]["BTHDTE_TIER_AMT2"].ToString().Trim();
                    //促銷活動-生日活動 - RATE2	
                    arrExportData[intRowIndexInSheet - 4, 66] = dtblData.Rows[intLoop]["BTHDTE_TIER_RATE2"].ToString().Trim();
                    //促銷活動-生日活動 - AMT3	
                    arrExportData[intRowIndexInSheet - 4, 67] = dtblData.Rows[intLoop]["BTHDTE_TIER_AMT3"].ToString().Trim();
                    //促銷活動-生日活動 - RATE3	
                    arrExportData[intRowIndexInSheet - 4, 68] = dtblData.Rows[intLoop]["BTHDTE_TIER_RATE3"].ToString().Trim();
                    //促銷活動-生日活動 - AMT4	
                    arrExportData[intRowIndexInSheet - 4, 69] = dtblData.Rows[intLoop]["BTHDTE_TIER_AMT4"].ToString().Trim();
                    //促銷活動-生日活動 - RATE4	
                    arrExportData[intRowIndexInSheet - 4, 70] = dtblData.Rows[intLoop]["BTHDTE_TIER_RATE4"].ToString().Trim();

                }

                range = sheet.get_Range("A4", "BS" + intRowIndexInSheet.ToString());
                range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中
                range.Font.Size = 12;//字體大小
                range.Font.Name = "新細明體";
                range.Value2 = arrExportData;
                range.Borders.LineStyle = 1;
                range.EntireColumn.AutoFit();

                //* 保存文件到程序运行目录下
                strPathFile = strPathFile + @"\ExcelFile_DetailAward_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                sheet.SaveAs(strPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                excel.ActiveWorkbook.Close(false, null, null); //* 关闭 Excel 文件且不保存
                return true;

                #endregion 匯入Excel文檔
            }
            catch (Exception ex)
            {
                Logging.SaveLog(ELogLayer.BusinessRule, ex);
                throw ex;
            }
            finally
            {
                excel.Quit(); //* 退出 Excel
                excel = null; //* 将 Excel 实例设置为空
            }
        }

        public static bool CreateExcelFile_FileDetail(string strRECEIVENUMBER, string strReceiveDate, string strMERCHANT,
                                            ref string strPathFile, ref string strMsgID)
        {
            //*创建一个 Excel 实例
            ExcelApplication excel = new ExcelApplication();
            try
            {
                //* 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                //* 取要下載的資料
                int iTCount = 0;
                DataTable dtblData = BRRedeemSet.GetFileDetail(strRECEIVENUMBER, strReceiveDate, strMERCHANT, 0, 0, ref iTCount).Tables[0];
                if (null == dtblData)
                    return false;
                if (dtblData.Rows.Count == 0)
                {
                    strMsgID = "01_03050300_001";
                    return false;
                }

                #region 匯入Excel文檔

                excel.Visible = false;//* 不显示 Excel 文件,如果为 true 则显示 Excel 文件
                excel.Application.DisplayAlerts = false;
                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "FileDetail.xls";
                Workbook workbook = excel.Workbooks.Open(strExcelPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                        Missing.Value, Missing.Value, Missing.Value, Missing.Value);

                Range range = null;//* 创建一个空的单元格对象
                int intRowIndexInSheet = 0;
                Worksheet sheet = null;
                string[,] arrExportData = null;
                arrExportData = new string[dtblData.Rows.Count, 8];

                for (int intLoop = 0; intLoop < dtblData.Rows.Count; intLoop++)
                {
                    //* 添加新的Sheet
                    if (intLoop == 0)
                    {
                        sheet = (Worksheet)workbook.Sheets[1];

                        intRowIndexInSheet = 2;
                    }

                    intRowIndexInSheet++;

                    //收件編號	
                    arrExportData[intRowIndexInSheet - 3, 0] = dtblData.Rows[intLoop]["RECEIVE_NUMBER"].ToString().Trim();
                    //作業種類	
                    if (dtblData.Rows[intLoop]["RECEIVE_NUMBER"].ToString().ToUpper().Trim().StartsWith("RC") || dtblData.Rows[intLoop]["RECEIVE_NUMBER"].ToString().ToUpper().Trim().StartsWith("AU"))
                    {
                        arrExportData[intRowIndexInSheet - 3, 1] = "C";
                    }
                    else
                    {
                        arrExportData[intRowIndexInSheet - 3, 1] = "A";
                    }                    
                    //收件時間
                    arrExportData[intRowIndexInSheet - 3, 2] = dtblData.Rows[intLoop]["RTime"].ToString().Trim();
                    //一KEY經辦
                    arrExportData[intRowIndexInSheet - 3, 3] = dtblData.Rows[intLoop]["USER_ID1"].ToString().Trim();
                    //提交時間 - 一KEY經辦
                    arrExportData[intRowIndexInSheet - 3, 4] = dtblData.Rows[intLoop]["STime1"].ToString().Trim();
                    //二KEY經辦
                    arrExportData[intRowIndexInSheet - 3, 5] = dtblData.Rows[intLoop]["USER_ID2"].ToString().Trim();
                    //提交時間 - 二KEY經辦
                    arrExportData[intRowIndexInSheet - 3, 6] = dtblData.Rows[intLoop]["STime2"].ToString().Trim();
                    //比對結果是否一致
                    arrExportData[intRowIndexInSheet - 3, 7] = dtblData.Rows[intLoop]["ISSAME"].ToString().Trim();                    

                }

                range = sheet.get_Range("A3", "H" + intRowIndexInSheet.ToString());
                range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中
                range.Font.Size = 12;//字體大小
                range.Font.Name = "新細明體";
                range.Value2 = arrExportData;
                range.Borders.LineStyle = 1;
                range.EntireColumn.AutoFit();

                //* 保存文件到程序运行目录下
                strPathFile = strPathFile + @"\ExcelFile_FileDetail_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                sheet.SaveAs(strPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                excel.ActiveWorkbook.Close(false, null, null); //* 关闭 Excel 文件且不保存
                return true;

                #endregion 匯入Excel文檔
            }
            catch (Exception ex)
            {
                Logging.SaveLog(ELogLayer.BusinessRule, ex);
                throw ex;
            }
            finally
            {
                excel.Quit(); //* 退出 Excel
                excel = null; //* 将 Excel 实例设置为空
            }
        }
        //add by linhuanhuang end

        /// <summary>
        /// 匯出ACH授權扣款資料清單資料時，查詢數據
        /// </summary>
        /// <param name="dirValues">查詢條件</param>
        /// <param name="strBuildDate">鍵檔起迄日</param>
        /// <param name="strBatchNo">首錄起迄日</param>
        /// <param name="strBank_Code">行庫</param>
        /// <returns>DataTable(查詢結果)</returns>
        public static DataTable getData_ACH(Dictionary<string, string> dirValues,
                        ref string strBuildDate, ref string strBatchNo, ref string strBank_Code)
        {
            SqlCommand sqlComm = new SqlCommand();
            StringBuilder sbWhere = new StringBuilder("");
            foreach (KeyValuePair<string, string> entry in dirValues)
            {
                //* 鍵檔起日
                if (entry.Key == "txtBuildDateStart" && entry.Value != "")
                {
                    sbWhere.Append(" and a.Build_Date >= @Build_Date_Start ");
                    SqlParameter parmBuildDateStart = new SqlParameter("@Build_Date_Start", entry.Value);
                    sqlComm.Parameters.Add(parmBuildDateStart);
                    strBuildDate = entry.Value;
                }

                //* 鍵檔迄日
                if (entry.Key == "txtBuildDateEnd" && entry.Value != "")
                {
                    sbWhere.Append(" and a.Build_Date <= @Build_Date_End ");
                    SqlParameter parmBuildDateEnd = new SqlParameter("@Build_Date_End", entry.Value);
                    sqlComm.Parameters.Add(parmBuildDateEnd);
                    strBuildDate += "~" + entry.Value;
                }

                //* 首錄起日
                if (entry.Key == "txtInputDateStart" && entry.Value != "")
                {
                    sbWhere.Append(" and a.Batch_no >= @Input_Date_Start ");
                    SqlParameter parmInputDateStart = new SqlParameter("@Input_Date_Start", entry.Value);
                    sqlComm.Parameters.Add(parmInputDateStart);
                    strBatchNo = entry.Value;
                }

                //* 首錄迄日
                if (entry.Key == "txtInputDateEnd" && entry.Value != "")
                {
                    sbWhere.Append(" and a.Batch_no <= @Input_Date_End ");
                    SqlParameter parmInputDateEnd = new SqlParameter("@Input_Date_End", entry.Value);
                    sqlComm.Parameters.Add(parmInputDateEnd);
                    strBatchNo += "~" + entry.Value;
                }

                //* 行庫
                if (entry.Key == "txtBank_Code" && entry.Value != "")
                {
                    sbWhere.Append(" and a.Other_Bank_Code_S = @Other_Bank_Code_S ");
                    SqlParameter parmOther_Bank_Code_S = new SqlParameter("@Other_Bank_Code_S", entry.Value);
                    sqlComm.Parameters.Add(parmOther_Bank_Code_S);
                    strBank_Code = entry.Value;
                }
            }

            //* 添加查詢條件
            sqlComm.CommandText = string.Format(SEL_ACH_OTHER_BANK_TEMP, sbWhere.ToString());
            sqlComm.CommandType = CommandType.Text;

            //* 查詢并返回查詢結果
            return ((DataSet)BROTHER_BANK_TEMP.SearchOnDataSet(sqlComm)).Tables[0];
        }

        /// <summary>
        /// 匯出R02授權成功/失敗報表資料時，查詢數據
        /// </summary>
        /// <param name="dirValues">查詢條件</param>
        /// <param name="strBuildDate">鍵檔起迄日</param>
        /// <param name="strBatchNo">首錄起迄日</param>
        /// <param name="strBank_Code">行庫</param>
        /// <returns>DataTable(查詢結果)</returns>
        public static DataTable getData_R02(Dictionary<string, string> dirValues,
                    ref string strInputDate, ref bool blSuccess, ref bool blFault)
        {
            SqlCommand sqlComm = new SqlCommand();
            StringBuilder sbWhere = new StringBuilder(" b.dateInput between @DateInputStart and @DateInputEnd and ");
            blSuccess = false;
            blFault = false;
            strInputDate = "";
            foreach (KeyValuePair<string, string> entry in dirValues)
            {
                //* 首錄起日
                if (entry.Key == "txtInputDateStart" && entry.Value != "")
                {
                    SqlParameter parmInputDateStart = new SqlParameter("@DateInputStart", entry.Value);
                    sqlComm.Parameters.Add(parmInputDateStart);
                    strInputDate = entry.Value;
                }

                //* 首錄迄日
                if (entry.Key == "txtInputDateEnd" && entry.Value != "")
                {
                    SqlParameter parmInputDateEnd = new SqlParameter("@DateInputEnd", entry.Value);
                    sqlComm.Parameters.Add(parmInputDateEnd);
                    strInputDate += "~" + entry.Value;
                }

                //* 成功
                if (entry.Key == "Success" && entry.Value != "")
                {
                    if (entry.Value == "1")
                    {
                        blSuccess = true;
                    }
                }

                //* 失敗
                if (entry.Key == "Fault" && entry.Value != "")
                {
                    if (entry.Value == "1")
                    {
                        blFault = true;
                    }
                }
            }

            //* 若【成功】和【失敗】都未選擇或都已選擇 
            if (blSuccess && blFault || !blSuccess && !blFault)
            {
                sbWhere.Append(" b.R02_flag in ('1','2','3') ");
            }
            else
            {
                if (blSuccess)
                {
                    sbWhere.Append(" b.R02_flag in ('2','3') and a.Ach_Return_Code = '0'");
                }
                if (blFault)
                {
                    sbWhere.Append(" (a.Ach_Return_Code != '0' or b.R02_flag ='1') ");
                }
            }

            //* 添加查詢條件
            sqlComm.CommandText = string.Format(SEL_ACH_OTHER_BANK_TEMP_R02, sbWhere.ToString());
            sqlComm.CommandType = CommandType.Text;

            //* 查詢并返回查詢結果
            return ((DataSet)BROTHER_BANK_TEMP.SearchOnDataSet(sqlComm)).Tables[0];
        }

        /// <summary>
        /// P02授權未回覆報表
        /// </summary>
        /// <param name="dirValues">查詢條件</param>
        /// <param name="strBuildDate">鍵檔起迄日</param>
        /// <param name="strBatchNo">首錄起迄日</param>
        /// <param name="strBank_Code">行庫</param>
        /// <returns>DataTable(查詢結果)</returns>
        public static DataTable getData_P02(Dictionary<string, string> dirValues, ref string strInputDate)
        {
            SqlCommand sqlComm = new SqlCommand();
            StringBuilder sbWhere = new StringBuilder(" b.dateInput between @DateInputStart and @DateInputEnd ");
            strInputDate = "";
            foreach (KeyValuePair<string, string> entry in dirValues)
            {
                //* 首錄起日
                if (entry.Key == "txtInputDateStart" && entry.Value != "")
                {
                    SqlParameter parmInputDateStart = new SqlParameter("@DateInputStart", entry.Value);
                    sqlComm.Parameters.Add(parmInputDateStart);
                    strInputDate = entry.Value;
                }

                //* 首錄迄日
                if (entry.Key == "txtInputDateEnd" && entry.Value != "")
                {
                    SqlParameter parmInputDateEnd = new SqlParameter("@DateInputEnd", entry.Value);
                    sqlComm.Parameters.Add(parmInputDateEnd);
                    strInputDate += "~" + entry.Value;
                }

                //* 行庫
                if (entry.Key == "txtBank_Code" && entry.Value != "")
                {
                    sbWhere.Append(" and c.BankCodeS = @BankCodeS ");
                    SqlParameter parmBankCodeS = new SqlParameter("@BankCodeS", entry.Value);
                    sqlComm.Parameters.Add(parmBankCodeS);
                }
            }

            SqlParameter parmInputToday = new SqlParameter("@dateInputtoday", System.DateTime.Now.AddYears(-1911).ToString("yyyyMMdd"));
            sqlComm.Parameters.Add(parmInputToday);

            //* 添加查詢條件
            sqlComm.CommandText = string.Format(BROTHER_BANK_TEMP.SEL_OTHER_BANK_TEMP_RECORD_P02, sbWhere.ToString());
            sqlComm.CommandType = CommandType.Text;

            //* 查詢并返回查詢結果
            return ((DataSet)BROTHER_BANK_TEMP.SearchOnDataSet(sqlComm)).Tables[0];
        }

        /// <summary>
        /// 批次作業量統計報表
        /// </summary>
        /// <param name="dirValues">查詢條件</param>
        /// <param name="strBuildDate">鍵檔起迄日</param>
        /// <param name="strBatchNo">首錄起迄日</param>
        /// <param name="strBank_Code">行庫</param>
        /// <returns>DataSet(查詢結果)</returns>
        public static DataSet getData_Batch(Dictionary<string, string> dirValues,
                            string strType, ref string strInputDate)
        {
            SqlCommand sqlComm = new SqlCommand();
            foreach (KeyValuePair<string, string> entry in dirValues)
            {
                //* 首錄起日
                if (entry.Key == "txtInputDateStart" && entry.Value != "")
                {
                    SqlParameter parmInputDateStart = new SqlParameter("@Batch_no_start", entry.Value);
                    sqlComm.Parameters.Add(parmInputDateStart);
                    strInputDate = entry.Value;
                }

                //* 首錄迄日
                if (entry.Key == "txtInputDateEnd" && entry.Value != "")
                {
                    SqlParameter parmInputDateEnd = new SqlParameter("@Batch_no_end", entry.Value);
                    sqlComm.Parameters.Add(parmInputDateEnd);
                    strInputDate += "~" + entry.Value;
                }
            }

            //* 添加查詢條件
            switch (strType)
            {
                case "1"://* 批次作業量統計
                    sqlComm.CommandText = SEL_BATCH;
                    break;
                case "2"://* 批次結果報表_成功
                    sqlComm.CommandText = SEL_BATCH_SUCCESS;
                    break;
                case "3"://* 批次結果報表_失敗
                    sqlComm.CommandText = SEL_BATCH_FAULT;
                    break;
                case "4"://* 批次結果報表_未完成
                    sqlComm.CommandText = SEL_BATCH_NO_COMPLETE;
                    break;
                case "5"://* 批次結果報表_全部
                    sqlComm.CommandText = SEL_BATCH_SUCCESS + SEL_BATCH_FAULT + SEL_BATCH_NO_COMPLETE;
                    break;
            }
            sqlComm.CommandType = CommandType.Text;

            //* 查詢并返回查詢結果
            return (DataSet)BROTHER_BANK_TEMP.SearchOnDataSet(sqlComm);
        }

        /// <summary>
        /// 下載服務器文檔到本地（TransmitFile实现方法）
        /// </summary>
        /// <param name="strSeverPathFile">服務器上要下載的文檔</param>
        /// <param name="strLocalPathFile">本地保存名稱</param>
        /// <param name="page">呼叫頁面</param>
        /// <returns></returns>
        public static bool Download_File_Trans(string strSeverPathFile, string strLocalPathFile, Page page)
        {
            try
            {
                //* 本地文檔名稱沒有輸入
                if (strLocalPathFile.Trim() == "")
                {
                    strLocalPathFile = DownloadFileName;
                }

                //* 檢查服務器端文檔是否存在
                if (!File.Exists(strSeverPathFile))
                {
                    //* 如果不存在，不能下載，返回下載不成功
                    return false;
                }
                page.Response.ContentType = "application/vnd.ms-excel";
                page.Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(strLocalPathFile, Encoding.UTF8));
                page.Response.TransmitFile(strSeverPathFile);

                //* 返回下載成功
                return true;
            }
            catch (Exception ex)
            {
                Logging.SaveLog(ELogLayer.BusinessRule, ex);
                throw ex;
            }
        }
    }
}
