//******************************************************************
//*  作    者：占偉林(WeilinZhan)
//*  功能說明：報表查詢時，匯出到Excel
//*  創建日期：2009/11/24
//*  修改記錄：2020/12/07_Ares_Stanley-新增NPOI; 2021/03/17_Ares_Stanley-DB名稱改為變數; 2021/04/01_Ares_Stanley-移除MicrosoftExcel; 2021/04/12_Ares_Stanley-修改ACH批次結果報表(全部報表)
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
                       "from Other_Bank_Temp a left join {1}.dbo.M_PROPERTY_CODE b on a.Other_Bank_Code_S = b.PROPERTY_CODE " +
                       "where ((a.KeyIn_Flag='0') or (a.KeyIn_Flag='2' and a.Upload_flag='Y')) and b.FUNCTION_KEY='01'  and    b.PROPERTY_KEY='17'   {0} " +
                       "order by a.Receive_Number";

        public const string SEL_ACH_OTHER_BANK_TEMP_R02 = @"select a.Receive_Number,a.Other_Bank_Code_L,c.BankName,a.Other_Bank_Cus_ID," +
                                                          "a.Other_Bank_Acc_No,a.Cus_ID,a.Apply_Type," +
                                                          "CASE WHEN a.Ach_Return_Code = '0' THEN '成功' ELSE '失敗' END AS R02_flag,d.Ach_Rtn_Code,d.Ach_Rtn_Msg " +
                                                          "from Other_Bank_Temp a left join batch b on a.Batch_no = b.Batch_no " +
                                                          "left join " +
                                                          "(select bankl.property_code as BankCodeS,bankl.property_name as BankCodeL,bankn.property_name as BankName  " +
                                                          "from (select property_code ,property_name from {1}.dbo.m_property_code where function_key='01' and property_key='16') as bankl, " +
                                                          "(select property_code ,property_name from {1}.dbo.m_property_code where function_key='01' and property_key='17') as bankn " +
                                                          "where   bankl.property_code= bankn.property_code " +
                                                          ") as c " +
                                                          "on a.Other_Bank_Code_L=c.BankCodeL " +
                                                          "left join Ach_Rtn_Info d on a.ACH_Return_Code=d.Ach_Rtn_Code " +
                                                          "where {0} order by Receive_Number";

        public const string SEL_BATCH = @"select batch_no,sum(sCount) as sCount,sum(fCount) as fCount,sum(nCount) as nCount,sum(AllCount) as AllCount " +
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
        public const string SEL_BATCH_SUCCESS = @"select a.Batch_no,a.Receive_Number,a.Cus_ID,a.C1342_Return_Code," +
                            "a.Other_Bank_Code_S,a.Other_Bank_Acc_No,a.Other_Bank_Pay_Way,a.Other_Bank_Cus_ID,a.bcycle_code," +
                            "a.Mobile_Phone,a.E_Mail,E_Bill,isnull(b.[user_name],'') as [user_name], a.Pcmc_Return_Code " + 
            " from Other_Bank_Temp a left join (select distinct user_id,User_name from {0}.dbo.M_USER) as  b on a.[user_id] = b.[user_id] " +
                        "where Batch_no>=@Batch_no_start and Batch_no<=@Batch_no_end and (Pcmc_Return_Code='PAGE 00 OF 03' or Pcmc_Return_Code='PAGE 02 OF 03') " +
                        "order by a.Batch_no ";
        public const string SEL_BATCH_FAULT = @"select a.Batch_no,a.Receive_Number,a.Cus_ID," +
                            "a.Pcmc_Return_Code,a.Other_Bank_Code_S,a.Other_Bank_Acc_No,a.Other_Bank_Pay_Way," +
                            "a.Other_Bank_Cus_ID,a.bcycle_code,a.Mobile_Phone,a.E_Mail,E_Bill,isnull(b.[user_name],'') as [user_name],a.C1342_Return_Code " +
                        "from Other_Bank_Temp a left join (select distinct user_id,User_name from {0}.dbo.M_USER) as  b on a.[user_id] = b.[user_id] " +
                        "where Batch_no>=@Batch_no_start and Batch_no<=@Batch_no_end and (Pcmc_Upload_flag ='1' and not (Pcmc_Return_Code='PAGE 00 OF 03' or Pcmc_Return_Code='PAGE 02 OF 03')) " +
                        "order by a.Batch_no ";
        public const string SEL_BATCH_NO_COMPLETE = @"select a.Batch_no,a.Receive_Number,a.Cus_ID," +
                            "a.Pcmc_Return_Code,a.Other_Bank_Code_S,a.Other_Bank_Acc_No,a.Other_Bank_Pay_Way," +
                            "a.Other_Bank_Cus_ID,a.bcycle_code,a.Mobile_Phone,a.E_Mail,E_Bill,isnull(b.[user_name],'') as [user_name],a.C1342_Return_Code " +
                        "from Other_Bank_Temp a left join (select distinct user_id,User_name from {0}.dbo.M_USER) as  b on a.[user_id] = b.[user_id] " +
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
                Logging.Log(exp, LogLayer.BusinessRule);
                throw exp;
            }
        }

        #region NPOI-ACH授權扣款資料清單
        /// <summary>
        /// 修改日期: 2020/12/02_Ares_Stanley-變更報表產出為NPOI; 2020/12/10_Ares_Stanley-新增總計千分位; 2021/04/12_Ares_Stanley-補首錄日
        /// </summary>
        /// <param name="dirValues"></param>
        /// <param name="strAgentName"></param>
        /// <param name="strPathFile"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_ACH(Dictionary<string, string> dirValues,
                        string strAgentName, ref string strPathFile, ref string strMsgID)
        {
            //*创建一个 Excel 实例
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

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "ACHOtherBank.xls";
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ExportExcelForNPOI(dtblData_ACH, ref wb, 10, "ACH授權扣款資料清單"); // Start index = 10
                ISheet sheet = wb.GetSheet("ACH授權扣款資料清單");
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
                #region 依行庫由小至大排序格式
                HSSFCellStyle rank = (HSSFCellStyle)wb.CreateCellStyle();
                //文字置中
                rank.VerticalAlignment = VerticalAlignment.Center;
                HSSFFont rank_font = (HSSFFont)wb.CreateFont();
                // cell format
                rank.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                //字體尺寸
                rank_font.FontHeightInPoints = 12;
                rank_font.FontName = "新細明體";
                rank_font.Color = NPOI.HSSF.Util.HSSFColor.Red.Index;
                rank.SetFont(rank_font);
                #endregion

                // 鍵檔區間
                sheet.GetRow(2).GetCell(1).SetCellValue(strBuildDate);
                // 送檔日

                // 首錄日
                sheet.GetRow(4).GetCell(1).SetCellValue(strBatchNo);
                // 收檔日

                // 列印經辦
                sheet.GetRow(6).GetCell(1).SetCellValue(strAgentName);
                // 列印日期
                string strYYYYMMDD = "000" + CSIPCommonModel.BaseItem.Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
                sheet.GetRow(7).GetCell(1).SetCellValue(strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8));
                // 合計筆數
                sheet.CreateRow(sheet.LastRowNum + 2).CreateCell(0).SetCellValue("合計筆數：");
                sheet.GetRow(sheet.LastRowNum).CreateCell(1).SetCellValue(dtblData_ACH.Rows.Count.ToString("N0"));
                sheet.GetRow(sheet.LastRowNum).GetCell(0).CellStyle = total;
                sheet.GetRow(sheet.LastRowNum).GetCell(1).CellStyle = total_num;
                // 依行庫由小至大排序
                sheet.CreateRow(sheet.LastRowNum + 4).CreateCell(0).SetCellValue("*依行庫由小至大排序");
                sheet.GetRow(sheet.LastRowNum).GetCell(0).CellStyle = rank;
        
                //* 保存文件到程序运行目录下
                strPathFile = strPathFile + @"\ExcelFile_ACH_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
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
        #endregion

        #region NPOI-R02授權成功/失敗報表
        /// <summary>
        /// 修改時間: 2020/12/02_Ares_Stanley-修改報表產出方式為NPOI; 2020/12/10_Ares_Stanley-新增總計千分位
        /// </summary>
        /// <param name="dirValues"></param>
        /// <param name="strAgentName"></param>
        /// <param name="strPathFile"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_R02(Dictionary<string, string> dirValues,
                        string strAgentName, ref string strPathFile, ref string strMsgID)
        {
            //*创建一个 Excel 实例
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

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "R02SF.xls";
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("R02授權成功失敗報表");
                //  成功/失敗總數初始化
                Int32 intNumS = 0;
                Int32 intNumF = 0;
                #region 文字格式
                HSSFCellStyle cs = (HSSFCellStyle)wb.CreateCellStyle();
                // 框線
                cs.BorderBottom = BorderStyle.Thin;
                cs.BorderLeft = BorderStyle.Thin;
                cs.BorderTop = BorderStyle.Thin;
                cs.BorderRight = BorderStyle.Thin;
                // 多行文字
                cs.WrapText = true;
                // Vertical, Horizontal Center
                cs.VerticalAlignment = VerticalAlignment.Center;
                cs.Alignment = HorizontalAlignment.Center;
                // cell format
                cs.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                // Font Size
                HSSFFont font1 = (HSSFFont)wb.CreateFont();
                font1.FontHeightInPoints = 12;
                font1.FontName = "新細明體";
                cs.SetFont(font1);

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
                #region 依行庫由小至大排序格式
                HSSFCellStyle rank = (HSSFCellStyle)wb.CreateCellStyle();
                //文字置中
                rank.VerticalAlignment = VerticalAlignment.Center;
                HSSFFont rank_font = (HSSFFont)wb.CreateFont();
                // cell format
                rank.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                //字體尺寸
                rank_font.FontHeightInPoints = 12;
                rank_font.FontName = "新細明體";
                rank_font.Color = NPOI.HSSF.Util.HSSFColor.Red.Index;
                rank.SetFont(rank_font);
                #endregion
                #endregion

                for (int idx = 0; idx < dtblData_R02.Rows.Count; idx++)
                {
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for(int col=0; col<9; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = cs;
                    }
                    // 收件編號
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue(dtblData_R02.Rows[idx]["Receive_Number"].ToString());
                    // 收受行（金支的7碼）
                    sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(dtblData_R02.Rows[idx]["Other_Bank_Code_L"].ToString());
                    // 收受行名稱(核印行)
                    sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue(dtblData_R02.Rows[idx]["BankName"].ToString());
                    // 委繳戶統編/身分證字號
                    sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(dtblData_R02.Rows[idx]["Other_Bank_Cus_ID"].ToString());
                    // 委繳戶帳號
                    sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(dtblData_R02.Rows[idx]["Other_Bank_Acc_No"].ToString());
                    // 持卡人ID
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue(dtblData_R02.Rows[idx]["Cus_ID"].ToString());
                    // 申請類別
                    sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellValue(dtblData_R02.Rows[idx]["Apply_Type"].ToString());
                    // 成功/失敗
                    sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(dtblData_R02.Rows[idx]["R02_flag"].ToString());
                    // 回覆訊息
                    sheet.GetRow(sheet.LastRowNum).GetCell(8).SetCellValue(dtblData_R02.Rows[idx]["Ach_Rtn_Code"].ToString().Trim() + "：" + dtblData_R02.Rows[idx]["Ach_Rtn_Msg"].ToString().Trim());
                    if (sheet.GetRow(sheet.LastRowNum).GetCell(8).StringCellValue.ToString() == "：")
                    {
                        sheet.GetRow(sheet.LastRowNum).GetCell(8).SetCellValue("系統內無該錯誤代碼對應訊息");
                    }
                    if (dtblData_R02.Rows[idx]["R02_flag"].ToString().Trim() == "成功")
                    {
                        intNumS++;
                    }
                    else
                    {
                        intNumF++;
                    }
                }
                //空行
                sheet.CreateRow(sheet.LastRowNum + 2);
                // 成功筆數, 失敗筆數, 總筆數
                for (int totalRow = 0; totalRow < 3; totalRow++)
                {
                    sheet.CreateRow(sheet.LastRowNum+1).CreateCell(0).CellStyle = total;
                    sheet.GetRow(sheet.LastRowNum).CreateCell(1).CellStyle = total_num;
                    switch (totalRow)
                    {
                        case 0: 
                            sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("成功筆數：");
                            sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(intNumS.ToString("N0"));
                            break;
                        case 1:
                            sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("失敗筆數：");
                            sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(intNumF.ToString("N0"));
                            break;
                        case 2:
                            sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("總筆數：");
                            sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(Convert.ToInt32(intNumS + intNumF).ToString("N0"));
                            break;
                    }
                }

                // *依行庫由小至大排序
                sheet.CreateRow(sheet.LastRowNum + 4).CreateCell(0).CellStyle = rank;
                sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("*依行庫由小至大排序");

                // 修改表頭
                if (blSuccess && blFault || !blSuccess && !blFault)
                {
                   sheet.GetRow(0).GetCell(0).SetCellValue("R02『授權成功/失敗』報表");
                    wb.SetSheetName(0, "R02授權成功失敗報表");
                }
                else if (blSuccess)
                {
                    sheet.GetRow(0).GetCell(0).SetCellValue("RO2『授權成功』報表");
                    wb.SetSheetName(0, "R02授權成功報表");
                }
                else if (blFault)
                {
                    sheet.GetRow(0).GetCell(0).SetCellValue("R02『授權失敗』報表");
                    wb.SetSheetName(0, "R02授權失敗報表");
                }
                // 首錄日
                sheet.GetRow(2).CreateCell(1).SetCellValue(strInputDate);
                // 列印經辦
                sheet.GetRow(3).CreateCell(1).SetCellValue(strAgentName);
                // 列印日期
                string strYYYYMMDD = "000" + CSIPCommonModel.BaseItem.Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
                sheet.GetRow(4).CreateCell(1).SetCellValue(strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8));

                //* 保存文件到程序运行目录下
                strPathFile = strPathFile + @"\ExcelFile_R02_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
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
        #endregion

        #region NPOI-批次作業量統計
        /// <summary>
        /// 批次作業量統計報表
        /// 修改日期: 2020/12/02_Ares_Stanley-更改報表產出方式為NPOI
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
                FileStream fs = null;
                HSSFWorkbook wb = null;
                string strExcelPathFile = "";
                switch (strType)
                {
                    case "1"://* 批次作業量統計
                        strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "Batch.xls";
                        fs = new FileStream(strExcelPathFile, FileMode.Open);
                        wb = new HSSFWorkbook(fs);
                        WriteDataToSheet_Batch(wb, strAgentName, dtblData_Batch.Tables[0], strInputDate);
                        break;

                    case "2"://* 批次結果報表_成功
                        strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "BatchSuccess.xls";
                        fs = new FileStream(strExcelPathFile, FileMode.Open);
                        wb = new HSSFWorkbook(fs);
                        WriteDataToSheet_Success(wb, strAgentName, dtblData_Batch.Tables[0], strInputDate);
                        break;

                    case "3"://* 批次結果報表_失敗
                        strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "BatchFault.xls";
                        fs = new FileStream(strExcelPathFile, FileMode.Open);
                        wb = new HSSFWorkbook(fs);
                        WriteDataToSheet_Fault(wb, strAgentName, dtblData_Batch.Tables[0], strInputDate);
                        break;

                    case "4"://* 批次結果報表_未完成
                        strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "BatchNoComplete.xls";
                        fs = new FileStream(strExcelPathFile, FileMode.Open);
                        wb = new HSSFWorkbook(fs);
                        WriteDataToSheet_No_Complete(wb, strAgentName, dtblData_Batch.Tables[0], strInputDate);
                        break;

                    case "5"://* 批次結果報表_全部
                        strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "BatchAll.xls";
                        fs = new FileStream(strExcelPathFile, FileMode.Open);
                        wb = new HSSFWorkbook(fs);
                        // 成功
                        WriteDataToSheet_Success(wb, strAgentName, dtblData_Batch.Tables[0], strInputDate);
                        // 失敗
                        WriteDataToSheet_Fault(wb, strAgentName, dtblData_Batch.Tables[1], strInputDate);
                        // 未完成
                        WriteDataToSheet_No_Complete(wb, strAgentName, dtblData_Batch.Tables[2], strInputDate);
                        break;
                }

                //* 保存文件到程序运行目录下
                strPathFile = strPathFile + @"\ExcelFile_BATCH_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
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
        #endregion

        #region NPOI-批次作業量統計_批次
        /// <summary>
        /// 向Sheet寫入數據(批次作業量統計)
        /// 修改日期: 2020/12/03_Ares_Stanley-變更報表產出方式為NPOI; 2020/12/10_Ares_Stanley-新增總計千分位
        /// </summary>
        /// <param name="sheet">寫入對象sheet</param>
        /// <param name="strAgentName">經辦人</param>
        private static void WriteDataToSheet_Batch(HSSFWorkbook wb, string strAgentName, DataTable dtblWriteData, string strInputDate)
        {
            ExportExcelForNPOI_filter(dtblWriteData, ref wb, 7, 1, "批次作業量統計表");
            ISheet sheet_batch = wb.GetSheet("批次作業量統計表");
            //轉換資料為數值格式
            for (int row = 7; row < sheet_batch.LastRowNum + 1; row++)
            {
                for (int col = 1; col < 4; col++)
                {
                    sheet_batch.GetRow(row).GetCell(col).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0");
                    sheet_batch.GetRow(row).GetCell(col).SetCellValue(Int32.Parse(sheet_batch.GetRow(row).GetCell(col).StringCellValue.ToString()));
                }
            }

            // 新增總計列
            sheet_batch.CreateRow(sheet_batch.LastRowNum + 1);
            for (int col = 0; col < 4; col++)
            {
                sheet_batch.GetRow(sheet_batch.LastRowNum).CreateCell(col).CellStyle = sheet_batch.GetRow(sheet_batch.LastRowNum - 1).GetCell(0).CellStyle;
            }
            sheet_batch.GetRow(sheet_batch.LastRowNum).GetCell(0).SetCellValue("總計");
            string formu = "";
            for (int i = 1; i < 4; i++)
            {
                switch (i)
                {
                    case 1: formu = string.Format("SUM(B8:B{0})", sheet_batch.LastRowNum); break;
                    case 2: formu = string.Format("SUM(C8:C{0})", sheet_batch.LastRowNum); break;
                    case 3: formu = string.Format("SUM(D8:D{0})", sheet_batch.LastRowNum); break;
                }
                sheet_batch.GetRow(sheet_batch.LastRowNum).GetCell(i).SetCellType(CellType.Formula);
                sheet_batch.GetRow(sheet_batch.LastRowNum).GetCell(i).SetCellFormula(formu);
                sheet_batch.GetRow(sheet_batch.LastRowNum).GetCell(i).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##0");
            }
            // Excel 開啟前先計算公式值
            HSSFFormulaEvaluator.EvaluateAllFormulaCells(wb);

            // 表頭
            for (int row = 2; row < 5; row++)
            {
                sheet_batch.GetRow(row).CreateCell(1);
                switch (row)
                {
                    case 2:
                        sheet_batch.GetRow(row).GetCell(1).SetCellValue(strInputDate);
                        break;
                    case 3:
                        sheet_batch.GetRow(row).GetCell(1).SetCellValue(strAgentName);
                        break;
                    case 4:
                        string strYYYYMMDD = "000" + CSIPCommonModel.BaseItem.Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
                        sheet_batch.GetRow(row).GetCell(1).SetCellValue(strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8));
                        break;
                }
            }
        }
        #endregion

        /// <summary>
        /// 向Sheet寫入數據(批次結果報表_未完成報表)
        /// 修改日期: 2020/12/03_Ares_Stanley-變更報表產出方式為NPOI; 2020/12/10_Ares_Stanley-新增總計千分位
        /// </summary>
        /// <param name="sheet">寫入對象sheet</param>
        /// <param name="strAgentName">經辦人</param>
        private static void WriteDataToSheet_No_Complete(HSSFWorkbook wb,
                    string strAgentName, DataTable dtblWriteData, string strInputDate)
        {
            ExportExcelForNPOI_filter(dtblWriteData, ref wb, 7, 1, "批次結果未完成報表");
            ISheet sheet_noComplete = wb.GetSheet("批次結果未完成報表");

            // 失敗訊息
            for (int row = 7; row < sheet_noComplete.LastRowNum + 1; row++)
            {
                sheet_noComplete.GetRow(row).GetCell(3).SetCellValue("");
            }


            #region 失敗總筆數文字格式
            // 成功總筆數文字
            HSSFCellStyle total_noComplete = (HSSFCellStyle)wb.CreateCellStyle();
            //文字置中
            total_noComplete.VerticalAlignment = VerticalAlignment.Center;
            total_noComplete.Alignment = HorizontalAlignment.Center;
            HSSFFont total_noComplete_font = (HSSFFont)wb.CreateFont();
            // cell format
            total_noComplete.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
            //字體尺寸
            total_noComplete_font.FontHeightInPoints = 12;
            total_noComplete_font.FontName = "新細明體";
            total_noComplete_font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            total_noComplete.SetFont(total_noComplete_font);
            #endregion

            // 新增未完成總筆數列
            sheet_noComplete.CreateRow(sheet_noComplete.LastRowNum + 3).CreateCell(0).CellStyle = total_noComplete;
            sheet_noComplete.GetRow(sheet_noComplete.LastRowNum).CreateCell(1).CellStyle = total_noComplete;
            sheet_noComplete.GetRow(sheet_noComplete.LastRowNum).GetCell(0).SetCellValue("未完成筆數：");
            sheet_noComplete.GetRow(sheet_noComplete.LastRowNum).GetCell(1).SetCellValue(dtblWriteData.Rows.Count.ToString("N0"));

            // 表頭
            for (int row = 2; row < 5; row++)
            {
                sheet_noComplete.GetRow(row).CreateCell(1).CellStyle = total_noComplete;
            }
            // 批次日
            sheet_noComplete.GetRow(2).GetCell(1).SetCellValue(strInputDate);
            // 列印經辦
            sheet_noComplete.GetRow(3).GetCell(1).SetCellValue(strAgentName);
            // 列印日期
            string strYYYYMMDD = "000" + CSIPCommonModel.BaseItem.Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
            sheet_noComplete.GetRow(4).GetCell(1).SetCellValue(strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8));
        }

        /// <summary>
        /// 向Sheet寫入數據(批次結果報表_失敗)
        /// 修改日期: 2020/12/03_Ares_Stanley-變更報表產出方式為NPOI; 2020/12/10_Ares_Stanley-新增總計千分位
        /// </summary>
        /// <param name="sheet">寫入對象sheet</param>
        /// <param name="strAgentName">經辦人</param>
        private static void WriteDataToSheet_Fault(HSSFWorkbook wb,
                    string strAgentName, DataTable dtblWriteData, string strInputDate)
        {
            ExportExcelForNPOI_filter(dtblWriteData, ref wb, 7, 1, "批次結果失敗報表");
            ISheet sheet_fault = wb.GetSheet("批次結果失敗報表");

            #region 失敗訊息
            for (int row = 7; row < sheet_fault.LastRowNum + 1; row++)
            {
                if (!(sheet_fault.GetRow(row).GetCell(3).StringCellValue.ToString() == "PAGE 00 OF 03" || sheet_fault.GetRow(row).GetCell(3).StringCellValue.ToString() == "PAGE 02 OF 03"))
                {
                    if (sheet_fault.GetRow(row).GetCell(3).StringCellValue.ToString() == "ERROR:10")
                    {
                        sheet_fault.GetRow(row).GetCell(3).SetCellValue("案件類別為D不為P02D");
                    }
                    else if (sheet_fault.GetRow(row).GetCell(3).StringCellValue.ToString() == "ERROR:9")
                    {
                        sheet_fault.GetRow(row).GetCell(3).SetCellValue("週期件");
                    }
                    else if (sheet_fault.GetRow(row).GetCell(3).StringCellValue.ToString() == "ERROR:0")
                    {
                        sheet_fault.GetRow(row).GetCell(3).SetCellValue("人工刪除");
                    }
                    else if (sheet_fault.GetRow(row).GetCell(3).StringCellValue.ToString() == "ERROR:O")
                    {
                        sheet_fault.GetRow(row).GetCell(3).SetCellValue("案件類別為 \"O\" 類");
                    }
                    else
                    {
                        sheet_fault.GetRow(row).GetCell(3).SetCellValue("PCMC失敗");
                    }
                }
            }
            #endregion

            #region 失敗總筆數文字格式
            // 成功總筆數文字
            HSSFCellStyle total_fault = (HSSFCellStyle)wb.CreateCellStyle();
            //文字置中
            total_fault.VerticalAlignment = VerticalAlignment.Center;
            total_fault.Alignment = HorizontalAlignment.Center;
            HSSFFont total_fault_font = (HSSFFont)wb.CreateFont();
            // cell format
            total_fault.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
            //字體尺寸
            total_fault_font.FontHeightInPoints = 12;
            total_fault_font.FontName = "新細明體";
            total_fault_font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            total_fault.SetFont(total_fault_font);
            #endregion

            // 新增失敗總筆數列
            sheet_fault.CreateRow(sheet_fault.LastRowNum + 3).CreateCell(0).CellStyle = total_fault;
            sheet_fault.GetRow(sheet_fault.LastRowNum).CreateCell(1).CellStyle = total_fault;
            sheet_fault.GetRow(sheet_fault.LastRowNum).GetCell(0).SetCellValue("失敗筆數：");
            sheet_fault.GetRow(sheet_fault.LastRowNum).GetCell(1).SetCellValue(dtblWriteData.Rows.Count.ToString("N0"));

            // 表頭
            for (int row = 2; row < 5; row++)
            {
                sheet_fault.GetRow(row).CreateCell(1).CellStyle = total_fault;
            }
            // 批次日
            sheet_fault.GetRow(2).GetCell(1).SetCellValue(strInputDate);
            // 列印經辦
            sheet_fault.GetRow(3).GetCell(1).SetCellValue(strAgentName);
            // 列印日期
            string strYYYYMMDD = "000" + CSIPCommonModel.BaseItem.Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
            sheet_fault.GetRow(4).GetCell(1).SetCellValue(strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8));
        }

        /// <summary>
        /// 向Sheet寫入數據(批次結果報表_成功)
        /// 修改日期: 2020/12/03_Ares_Stanley-變更產出方式為NPOI; 2020/12/10_Ares_Stanley-新增總計千分位
        /// </summary>
        /// <param name="sheet">寫入對象sheet</param>
        /// <param name="strAgentName">經辦人</param>
        private static void WriteDataToSheet_Success(HSSFWorkbook wb,
                    string strAgentName, DataTable dtblWriteData, string strInputDate)
        {
            ExportExcelForNPOI_filter(dtblWriteData, ref wb, 7, 1, "批次結果成功報表");
            ISheet sheet_success = wb.GetSheet("批次結果成功報表");

            // 檢測1342值
            for (int row = 7; row < sheet_success.LastRowNum + 1; row++)
            {
                if (sheet_success.GetRow(row).GetCell(3).StringCellValue.ToString() == "0001" || sheet_success.GetRow(row).GetCell(3).StringCellValue.ToString() == "")
                {
                    sheet_success.GetRow(row).GetCell(3).SetCellValue("");
                }
                else
                {
                    sheet_success.GetRow(row).GetCell(3).SetCellValue("1342失敗");
                }
            }


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

            #region 粗體文字格式
            HSSFCellStyle bold = (HSSFCellStyle)wb.CreateCellStyle();
            HSSFFont bold_font = (HSSFFont)wb.CreateFont();
            // cell format
            total_success.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
            //字體尺寸
            bold_font.FontHeightInPoints = 12;
            bold_font.FontName = "新細明體";
            bold_font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            bold.SetFont(bold_font);
            #endregion

            // 新增成功總筆數列
            sheet_success.CreateRow(sheet_success.LastRowNum + 3).CreateCell(0).CellStyle = total_success;
            sheet_success.GetRow(sheet_success.LastRowNum).CreateCell(1).CellStyle = total_success;
            sheet_success.GetRow(sheet_success.LastRowNum).GetCell(0).SetCellValue("成功筆數：");
            sheet_success.GetRow(sheet_success.LastRowNum).GetCell(1).SetCellValue(dtblWriteData.Rows.Count.ToString("N0"));

            // 表頭
            for (int row = 2; row < 5; row++)
            {
                sheet_success.GetRow(row).CreateCell(1).CellStyle = bold;
            }
            // 批次日
            sheet_success.GetRow(2).GetCell(1).SetCellValue(strInputDate);
            // 列印經辦
            sheet_success.GetRow(3).GetCell(1).SetCellValue(strAgentName);
            // 列印日期
            string strYYYYMMDD = "000" + CSIPCommonModel.BaseItem.Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
            sheet_success.GetRow(4).GetCell(1).SetCellValue(strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8));
        }

        #region NPOI-P02授權未回覆報表
        /// <summary>
        /// 修改日期:2020/12/02_Ares_Stanley-變更報表產出方式為NPOI; 2020/12/10_Ares_Stanley-新增總計千分位
        /// </summary>
        /// <param name="dirValues"></param>
        /// <param name="strAgentName"></param>
        /// <param name="strPathFile"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_P02(Dictionary<string, string> dirValues,
                        string strAgentName, ref string strPathFile, ref string strMsgID)
        {
            //*创建一个 Excel 实例
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

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "P02NOReply.xls";
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("P02授權未回覆報表");
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
                #region 依行庫由小至大排序格式
                HSSFCellStyle rank = (HSSFCellStyle)wb.CreateCellStyle();
                //文字置中
                rank.VerticalAlignment = VerticalAlignment.Center;
                HSSFFont rank_font = (HSSFFont)wb.CreateFont();
                // cell format
                rank.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                //字體尺寸
                rank_font.FontHeightInPoints = 12;
                rank_font.FontName = "新細明體";
                rank_font.Color = NPOI.HSSF.Util.HSSFColor.Red.Index;
                rank.SetFont(rank_font);
                #endregion

                ExportExcelForNPOI(dtblData_P02, ref wb, 7, "P02授權未回覆報表");
                // 總筆數
                sheet.CreateRow(sheet.LastRowNum + 5).CreateCell(0).CellStyle = total;
                sheet.GetRow(sheet.LastRowNum).CreateCell(1).CellStyle = total_num;
                sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("總筆數：");
                sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(dtblData_P02.Rows.Count.ToString("N0"));
                // *依行庫由小至大排序
                sheet.CreateRow(sheet.LastRowNum + 4).CreateCell(0).CellStyle = rank;
                sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("*依行庫由小至大排序");
                // 表頭
                for(int row =2; row<5; row++)
                {
                    switch (row)
                    {
                        case 2:
                            sheet.GetRow(row).CreateCell(1).SetCellValue(strInputDate);
                            break;
                        case 3:
                            sheet.GetRow(row).CreateCell(1).SetCellValue(strAgentName);
                            break;
                        case 4:
                            string strYYYYMMDD = "000" + CSIPCommonModel.BaseItem.Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
                            sheet.GetRow(row).CreateCell(1).SetCellValue(strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8));
                            break;
                    }
                }

                //* 保存文件到程序运行目录下
                strPathFile = strPathFile + @"\ExcelFile_P02_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
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
        #endregion

        /// <summary>
        /// 點數系統參數設定匯總報表
        /// 修改日期:2020/12/24_Ares_Stanley-變更報表產出方式為NPOI
        /// </summary>
        /// <param name="strSDATE"></param>
        /// <param name="strEDATE"></param>
        /// <param name="strPathFile"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_PointSummary(string strSDATE, string strEDATE,
                                            ref string strPathFile, ref string strMsgID)
        {
            //*创建一个 Excel 实例
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

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "PointSummary.xls";
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("P02授權未回覆報表");

                #region 文字格式
                HSSFCellStyle contentFormat = (HSSFCellStyle)wb.CreateCellStyle();
                // 文字置中
                contentFormat.VerticalAlignment = VerticalAlignment.Center;
                contentFormat.Alignment = HorizontalAlignment.Center;
                contentFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                // 字體設定
                HSSFFont contentFont = (HSSFFont)wb.CreateFont();
                contentFont.FontHeightInPoints = 12;
                contentFont.FontName = "新細明體";
                contentFormat.SetFont(contentFont);
                #endregion 文字格式

                // 表格內容
                for (int intLoop = 0; intLoop < dtblData.Rows.Count; intLoop++)
                {
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for(int col =0; col<9; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle.Alignment = HorizontalAlignment.Center;
                        sheet.GetRow(sheet.LastRowNum).GetCell(col).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                        sheet.GetRow(sheet.LastRowNum).GetCell(col).CellStyle = contentFormat;
                    }
                    // 收件編號
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue(dtblData.Rows[intLoop]["RECEIVE_NUMBER"].ToString());
                    // 作業種類 (Award/Redeem)
                    sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(dtblData.Rows[intLoop]["WTYPE"].ToString());
                    // 1key 經辦
                    sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue(dtblData.Rows[intLoop]["1KEYUID"].ToString());
                    // 1key總使用時間
                    sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(dtblData.Rows[intLoop]["1KEYUTIME"].ToString());
                    // 2key 經辦
                    sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(dtblData.Rows[intLoop]["2KEYUID"].ToString());
                    // 2key總使用時間
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue(dtblData.Rows[intLoop]["2KEYUTIME"].ToString());
                    // 是否一致
                    sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellValue(dtblData.Rows[intLoop]["ISSAME"].ToString());
                    // 不一致欄位數
                    sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(dtblData.Rows[intLoop]["DIFFNUM"].ToString());
                    // 更改經辦
                    sheet.GetRow(sheet.LastRowNum).GetCell(8).SetCellValue(dtblData.Rows[intLoop]["EDITUID"].ToString());
                }
                // 首錄日
                sheet.GetRow(2).CreateCell(1).CellStyle.Alignment = HorizontalAlignment.Left;
                sheet.GetRow(2).GetCell(1).SetCellValue(strSDATE.Trim() + "-" + strEDATE.Trim());

                //* 保存文件到程序运行目录下
                strPathFile = strPathFile + @"\ExcelFile_PointSummary_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
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
        #region NPOI-Redeem 交易結果明細表
        /// <summary>
        /// 修改日期: 2020/11/26_Ares_Stanley-變更報表產出方式為NPOI; 2021/03/08_Ares_Stanley-查詢單一日期改為查詢日期區間
        /// </summary>
        /// <param name="strRECEIVENUMBER">收件編號</param>
        /// <param name="strReceiveDate">收件日期</param>
        /// <param name="strMERCHANT">MERCHANT-NO(9碼)</param>
        /// <param name="strPathFile"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_DetailRedeem(string strRECEIVENUMBER, string strReceiveDateStart, string strReceiveDateEnd, string strMERCHANT,
                                            ref string strPathFile, ref string strMsgID, bool withDATE)
        {
            //*创建一个 Excel 实例
            try
            {
                //* 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                //* 取要下載的資料
                int iTCount = 0;
                DataTable dtblData = BRRedeem3270.GetDetailReport(strRECEIVENUMBER, strReceiveDateStart, strReceiveDateEnd, strMERCHANT, 0, 0, ref iTCount, withDATE).Tables[0];
                if (null == dtblData)
                    return false;
                if (dtblData.Rows.Count == 0)
                {
                    strMsgID = "01_03050300_001";
                    return false;
                }

                #region 匯入Excel文檔

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "DetailRedeem.xls";
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ExportExcelForNPOI(dtblData, ref wb, 3, "Redeem 交易結果明細表");

                //* 保存文件到程序运行目录下
                strPathFile = strPathFile + @"\ExcelFile_DetailRedeem_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();
                return true;

                #endregion 匯入Excel文檔
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
        #endregion


        #region NPOI-Award 交易結果明細表
        public static bool CreateExcelFile_DetailAward(string strRECEIVENUMBER, string strReceiveDate,
                                            ref string strPathFile, ref string strMsgID)
        {
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

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "DetailAward.xls";

                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ExportExcelForNPOI(dtblData, ref wb, 4, "Award 交易結果明細表");
                ISheet sheet = wb.GetSheet("Award 交易結果明細表");
                sheet.CreateRow(1);

                //* 保存文件到程序运行目录下
                strPathFile = strPathFile + @"\ExcelFile_DetailAward_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();
                return true;

                #endregion 匯入Excel文檔
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
        #endregion


        /// <summary>
        /// 鍵檔歷程明細表
        /// 修改紀錄:2021/01/18_Ares_Stanley-變更報表產出方式為NPOI; 2021/02/01_Ares_Stanley-收件日期調整為區間; 2021/02/03_Ares_Stanley-移除舊程式
        /// </summary>
        /// <param name="strRECEIVENUMBER"></param>
        /// <param name="strReceiveDateStart"></param>
        /// <param name="strReceiveDateEnd"></param>
        /// <param name="strMERCHANT"></param>
        /// <param name="strPathFile"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_FileDetail(string strRECEIVENUMBER, string strReceiveDateStart, string strReceiveDateEnd, string strMERCHANT,
                                            ref string strPathFile, ref string strMsgID)
        {
            try
            {
                //* 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                //* 取要下載的資料
                int iTCount = 0;
                DataTable dtblData = BRRedeemSet.GetFileDetail(strRECEIVENUMBER, strReceiveDateStart, strReceiveDateEnd, strMERCHANT, 0, 0, ref iTCount).Tables[0];
                if (null == dtblData)
                    return false;
                if (dtblData.Rows.Count == 0)
                {
                    strMsgID = "01_03050300_001";
                    return false;
                }

                #region 匯入Excel文檔
                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "FileDetail.xls";
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("工作表1");
                #region 文字格式
                HSSFCellStyle contentFormat = (HSSFCellStyle)wb.CreateCellStyle();
                // 文字置中
                contentFormat.VerticalAlignment = VerticalAlignment.Center;
                contentFormat.Alignment = HorizontalAlignment.Center;
                contentFormat.BorderLeft = BorderStyle.Thin;
                contentFormat.BorderTop = BorderStyle.Thin;
                contentFormat.BorderRight = BorderStyle.Thin;
                contentFormat.BorderBottom = BorderStyle.Thin;
                contentFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                // 字體設定
                HSSFFont contentFont = (HSSFFont)wb.CreateFont();
                contentFont.FontHeightInPoints = 12;
                contentFont.FontName = "新細明體";
                contentFormat.SetFont(contentFont);
                #endregion 文字格式

                for(int i = 0; i< dtblData.Rows.Count; i++)
                {
                    // 建立新資料列
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int col = 0; col < 8; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = contentFormat;
                    }
                    // 收件編號
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue(dtblData.Rows[i]["RECEIVE_NUMBER"].ToString().Trim());
                    // 作業種類
                    if (dtblData.Rows[i]["RECEIVE_NUMBER"].ToString().ToUpper().Trim().StartsWith("RC") || dtblData.Rows[i]["RECEIVE_NUMBER"].ToString().ToUpper().Trim().StartsWith("AU"))
                    {
                        sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue("C");
                    }
                    else
                    {
                        sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue("A");
                    }
                    // 收件時間
                    sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue(dtblData.Rows[i]["RTime"].ToString().Trim());
                    // 一KEY經辦
                    sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(dtblData.Rows[i]["USER_ID1"].ToString().Trim());
                    // 提交時間-一KEY經辦
                    sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(dtblData.Rows[i]["STime1"].ToString().Trim());
                    // 二KEY經辦
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue(dtblData.Rows[i]["USER_ID2"].ToString().Trim());
                    // 提交時間-二KEY經辦
                    sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellValue(dtblData.Rows[i]["STime2"].ToString().Trim());
                    // 比對結果是否一致
                    sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(dtblData.Rows[i]["ISSAME"].ToString().Trim());
                }

                //* 保存文件到程序运行目录下
                strPathFile = strPathFile + @"\ExcelFile_FileDetail_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();
                return true;

                #endregion 匯入Excel文檔
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
            sqlComm.CommandText = string.Format(SEL_ACH_OTHER_BANK_TEMP, sbWhere.ToString(), UtilHelper.GetAppSettings("DB_CSIP"));
            sqlComm.CommandType = CommandType.Text;

            //* 查詢并返回查詢結果
            return ((DataSet)BROTHER_BANK_TEMP.SearchOnDataSet(sqlComm)).Tables[0];
        }

        /// <summary>
        /// 功能說明: 匯出R02授權成功/失敗報表資料時，查詢數據
        /// 作    者:
        /// 創建時間:
        /// 修改記錄: 2020/11/10 Ares Luke 處理白箱報告SQL 
        /// </summary>
        /// <param name="dirValues">查詢條件</param>
        /// <param name="strBuildDate">鍵檔起迄日</param>
        /// <param name="strBatchNo">首錄起迄日</param>
        /// <param name="strBank_Code">行庫</param>
        /// 
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
            sqlComm.CommandText = string.Format(SEL_ACH_OTHER_BANK_TEMP_R02, sbWhere.ToString(), UtilHelper.GetAppSettings("DB_CSIP"));
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

            //* 添加查詢條件 //2021/04/08_Ares_Stanley-補充string format參數
            sqlComm.CommandText = string.Format(BROTHER_BANK_TEMP.SEL_OTHER_BANK_TEMP_RECORD_P02, sbWhere.ToString(), UtilHelper.GetAppSettings("DB_CSIP"));
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
                    sqlComm.CommandText = string.Format(SEL_BATCH_SUCCESS, UtilHelper.GetAppSettings("DB_CSIP"));
                    break;
                case "3"://* 批次結果報表_失敗
                    sqlComm.CommandText = string.Format(SEL_BATCH_FAULT, UtilHelper.GetAppSettings("DB_CSIP"));
                    break;
                case "4"://* 批次結果報表_未完成
                    sqlComm.CommandText = string.Format(SEL_BATCH_NO_COMPLETE, UtilHelper.GetAppSettings("DB_CSIP"));
                    break;
                case "5"://* 批次結果報表_全部
                    sqlComm.CommandText = string.Format(SEL_BATCH_SUCCESS, UtilHelper.GetAppSettings("DB_CSIP")) + string.Format(SEL_BATCH_FAULT, UtilHelper.GetAppSettings("DB_CSIP"))+ string.Format(SEL_BATCH_NO_COMPLETE, UtilHelper.GetAppSettings("DB_CSIP"));
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
        public static bool Download_File_Trans(string strSeverPathFile, string strLocalPathFile, System.Web.UI.Page page)
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
                Logging.Log(ex, LogLayer.BusinessRule);
                throw ex;
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

        #region 資料隱碼
        /// <summary>
        /// 建立日期:2021/08/17_Ares_Stanley
        /// </summary>
        /// <param name="data">隱碼資料</param>
        /// <param name="symbol">隱碼符號</param>
        /// <param name="start">起始位置</param>
        /// <param name="count">隱碼字數</param>
        /// <returns>隱碼後資料</returns>
        public static string addHideCode(string data, string symbol = "X", int start = 6, int count = 4)
        {
            try
            {
                data = data.Trim();
                if (string.IsNullOrEmpty(data))
                {
                    return data;
                }
                if (data.Length < (start + count) || data.Length < start || data.Length < count)
                {
                    return data;
                }
                string hideData = string.Empty;
                string head = data.Substring(0, start);
                string foot = data.Substring((start + count), data.Length - (start + count));
                string hideCode = string.Empty;
                for (int i = 0; i < count; i++)
                {
                    hideCode += symbol;
                }
                hideData = head + hideCode + foot;
                return hideData;
            }
            catch(Exception ex)
            {
                Logging.Log(ex);
                return data;
            }
        }
        #endregion
    }
}
