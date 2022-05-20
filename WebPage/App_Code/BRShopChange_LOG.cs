//******************************************************************
//*  功能說明：ShopChange_LOG 資料庫業務類
//*  創建日期：2019/09/12
//*  修改紀錄：2021/01/19_Ares_Stanley-新增NPOI; 2021/04/01_Ares_Stanley-移除MicrosoftExcel
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Framework.Data.OM;
using CSIPKeyInGUI.EntityLayer_new;
using Framework.Data.OM.Transaction;
using System.Data.SqlClient;
using Framework.Common.Logging;
using System.Reflection;
using Framework.Common.Utility;
using System.IO;
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
    /// ShopChange_LOG資料庫業務類
    /// </summary>
    public class BRShopChange_LOG : CSIPCommonModel.BusinessRules.BRBase<EntityShopChange_LOG>
    {
        #region SQL

        public const string SEL_SHOP_INFO = @"SELECT {0} FROM ShopChange_LOG with(nolock) WHERE ";
        //2021/03/17_Ares_Stanley-DB名稱改為變數
        public const string SEL_CHANGELOG = @"
                        SELECT ROW_NUMBER() OVER (ORDER BY MOD_DATE,CORP_NO) as 'SEQ',CORP_NO,[1] AS 'USER1',[2] AS 'USER2',[3] AS 'USER3',CONVERT(VARCHAR,MOD_DATE,111) AS 'MOD_DATE'
                        FROM 
                        (
	                        SELECT CORP_NO,KEYINFLAG,MAX(b.[USER_NAME]) AS MAXUSER,MOD_DATE
	                        FROM SHOPCHANGE_LOG a WITH(NOLOCK) JOIN {0}.dbo.M_USER b WITH(NOLOCK) ON b.[USER_ID]=a.MOD_USER
	                        WHERE MOD_DATE BETWEEN @DATE_BEGIN AND  @DATE_END
	                        GROUP BY DOC_ID,CORP_NO,KEYINFLAG,MOD_DATE
                        ) AS GDV_TABLE
                        PIVOT
                        (
	                        MAX(MAXUSER)
	                        FOR KEYINFLAG IN ([1],[2],[3])
                        ) AS PIVOTTABLE";
        public const string SEL_MAINFRAMELOG = @"
                            SELECT CORP_NO,CORP_SEQ,MERCH_NO,STATUS_CODE,TERMINATE_DATE,TERMINATE_CODE,UPDATE_CNT,BATCH_DATE
	                        FROM MAINFRAME_IMP_LOG WITH(NOLOCK)
	                        WHERE BATCH_DATE BETWEEN @DATE_BEGIN AND  @DATE_END
	                        ORDER BY BATCH_DATE,CORP_NO,CORP_SEQ";
        #endregion

        /// <summary>
        /// 查詢資料庫信息
        /// </summary>
        /// <param name="strUniNo">統一編號</param>
        /// <param name="strKeyInFlag">Keyin類別(1-一KEY/2-二KEY)</param>
        /// <param name="strColumns">要查詢得到的欄位</param>
        /// <returns>DataSet</returns>
        public static DataSet Select(string strUniNo, string strKeyInFlag, string strColumns, string strCheckFlag)
        {
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntityShopChange_LOG.M_DOC_ID, Operator.Equal, DataTypeUtils.String, strUniNo);
            sSql.AddCondition(EntityShopChange_LOG.M_KeyinFLAG, Operator.Equal, DataTypeUtils.String, strKeyInFlag);

            string strSqlCmd = string.Format(SEL_SHOP_INFO, strColumns) + sSql.GetFilterCondition().Substring(4, sSql.GetFilterCondition().Length - 4);

            return BRShopChange_LOG.SearchOnDataSet(strSqlCmd);
        }
        
        /// <summary>
        /// 添加ShopChange_LOG表資料
        /// </summary>
        /// <param name="eShopChange">ShopChange_LOG實體</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Add(EntityShopChange_LOG eShopChange)
        {
            return BRShopChange_LOG.AddNewEntity(eShopChange);        
        }
        

        /// <summary>
        /// 根據統一編號、KeyinFlag刪除ShopChange_LOG表資料
        /// </summary>
        /// <param name="eShopChange">SHOP實體</param>
        /// <param name="strUniNo">統一編號</param>
        /// <param name="strKeyinFlag">KEYIN FLAG</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Delete(EntityShopChange_LOG eChangeLog, string strDOC_ID, string strKeyinFlag)
        {
            SqlHelper Sql = new SqlHelper();

            Sql.AddCondition(EntityShopChange_LOG.M_DOC_ID, Operator.Equal, DataTypeUtils.String, strDOC_ID);
            Sql.AddCondition(EntityShopChange_LOG.M_KeyinFLAG, Operator.Equal, DataTypeUtils.String, strKeyinFlag);

            return BRShopChange_LOG.DeleteEntityByCondition(eChangeLog, Sql.GetFilterCondition());       
        }

        /// <summary>
        /// 先刪除SHOP表資料后再新增
        /// </summary>
        /// <param name="eShopChange">SHOP實體</param>
        /// <param name="strUNI_NO">商店代號</param>
        /// <param name="strKeyInFlag">Keyin類別(1-一KEY/2-二KEY)</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Insert(EntityShopChange_LOG eChangeLog, string strDOC_ID, string strKeyInFlag)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (!Delete(eChangeLog, strDOC_ID, strKeyInFlag))
                    {
                        return false;
                    }

                    if (!Add(eChangeLog))
                    {
                        return false;
                    }
                    ts.Complete();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 匯出特店資料異動(依統編)鍵檔同仁明細/主機更新明細成Excel
        /// 修改紀錄:2021/01/19_Ares_Stanley-變更報表產出為NPOI; 2021/02/19_Ares_Stanley-調整內容靠左對齊
        /// </summary>
        /// <param name="dtData"></param>
        /// <param name="strAgentName"></param>
        /// <param name="strPathFile"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile(Dictionary<string, string> dirValues, string strAgentName,string strExcelTemplete,string strProperty, ref string strPathFile, ref string strMsgID)
        {
            string strBuildDate = string.Empty;
            try
            {
                // 檢查目錄，並刪除以前的文檔資料
                BRExcel_File.CheckDirectory(ref strPathFile);

                //取得資料
                System.Data.DataTable dtblData_ACH = BRShopChange_LOG.getData_ChangeLog(dirValues, strProperty, ref strBuildDate);

                if (null == dtblData_ACH)
                    return false;
                if (dtblData_ACH.Rows.Count == 0)
                {
                    strMsgID = "01_03040000_003";//沒有對應的資料！
                    return false;
                }

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + strExcelTemplete.Trim();
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet;
                if (strProperty.Trim().Equals("01030501"))
                {
                    sheet = wb.GetSheet("特店資料異動(依統編)明細");
                }
                else
                {
                    sheet = wb.GetSheet("特店資料異動(依統編)-主機異動明細");
                }
                #region 資料靠左對齊
                HSSFCellStyle titleFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
                titleFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
                titleFormat.Alignment = HorizontalAlignment.Left; //水平置中
                titleFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字

                HSSFFont titleFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                titleFont.FontHeightInPoints = 12; //字體大小
                titleFont.FontName = "新細明體"; //字型
                titleFormat.SetFont(titleFont); //設定儲存格的文字樣式
                #endregion 資料靠左對齊
                #region 資料粗體靠左對齊
                HSSFCellStyle boldLeftFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
                boldLeftFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
                boldLeftFormat.Alignment = HorizontalAlignment.Left; //水平置中
                boldLeftFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字

                HSSFFont boldLeftFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                boldLeftFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold; //粗體
                boldLeftFont.FontHeightInPoints = 12; //字體大小
                boldLeftFont.FontName = "新細明體"; //字型
                boldLeftFormat.SetFont(boldLeftFont); //設定儲存格的文字樣式
                #endregion 資料粗體靠左對齊
                #region 表格內容格式
                HSSFCellStyle contentFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
                contentFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
                contentFormat.Alignment = HorizontalAlignment.Left; //水平置中
                contentFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##0"); //將儲存格內容設定為文字
                contentFormat.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin; // 儲存格框線
                contentFormat.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                contentFormat.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                contentFormat.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

                HSSFFont contentFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                contentFont.FontHeightInPoints = 12; //字體大小
                contentFont.FontName = "新細明體"; //字型
                contentFormat.SetFont(contentFont); //設定儲存格的文字樣式
                #endregion 表格內容格式

                #region 表頭
                for (int row = 3; row < 6; row++)
                {
                    sheet.GetRow(row).CreateCell(1).CellStyle = titleFormat;
                }
                // 起訖日
                sheet.GetRow(3).GetCell(1).SetCellValue(strBuildDate);
                // 列印經辦
                sheet.GetRow(4).GetCell(1).SetCellValue(strAgentName);
                // 列印日期
                sheet.GetRow(5).GetCell(1).SetCellValue(DateTime.Now.ToString("yyyy/MM/dd"));
                #endregion

                #region 表身
                for (int intLoop = 0; intLoop < dtblData_ACH.Rows.Count; intLoop++)
                {
                    if (strProperty.Trim().Equals("01030501")) // 鍵檔同仁明細
                    {
                        // 建立新一列
                        sheet.CreateRow(sheet.LastRowNum + 1);
                        for (int col = 0; col < 6; col++)
                        {
                            sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = contentFormat;
                        }
                        //* 序號
                        sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue(dtblData_ACH.Rows[intLoop]["SEQ"].ToString());
                        //* 統一編號
                        sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(dtblData_ACH.Rows[intLoop]["CORP_NO"].ToString());
                        //* 鍵一同仁
                        sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue(dtblData_ACH.Rows[intLoop]["USER1"].ToString());
                        //* 鍵二同仁
                        sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(dtblData_ACH.Rows[intLoop]["USER2"].ToString());
                        //* 覆核同仁
                        sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(dtblData_ACH.Rows[intLoop]["USER3"].ToString());
                        //* 異動日期
                        sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue(dtblData_ACH.Rows[intLoop]["MOD_DATE"].ToString());
                    }
                    else // 主機異動明細
                    {
                        // 建立新一列
                        sheet.CreateRow(sheet.LastRowNum + 1);
                        for (int col = 0; col < 8; col++)
                        {
                            sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = contentFormat;
                        }
                        //* 作業日
                        sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue(dtblData_ACH.Rows[intLoop]["BATCH_DATE"].ToString());
                        //* 統一編號
                        sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(dtblData_ACH.Rows[intLoop]["CORP_NO"].ToString());
                        //* 統編序號
                        sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue(dtblData_ACH.Rows[intLoop]["CORP_SEQ"].ToString());
                        //* 商店代號
                        sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(dtblData_ACH.Rows[intLoop]["MERCH_NO"].ToString());
                        //* 狀態碼
                        sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(dtblData_ACH.Rows[intLoop]["STATUS_CODE"].ToString());
                        //* 解約原因
                        sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue(dtblData_ACH.Rows[intLoop]["TERMINATE_CODE"].ToString());
                        //* 解約日期
                        sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellValue(dtblData_ACH.Rows[intLoop]["TERMINATE_DATE"].ToString());
                        //* 更新筆數
                        sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(dtblData_ACH.Rows[intLoop]["UPDATE_CNT"].ToString());
                    }
                }
                // 總筆數列
                sheet.CreateRow(sheet.LastRowNum + 2).CreateCell(0).CellStyle=boldLeftFormat;
                sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue(string.Format("總筆數：共 {0} 筆", dtblData_ACH.Rows.Count));
                #endregion

                // 保存文件到程序運行目錄下
                strPathFile = strPathFile + @"\"+ strProperty.Trim() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();
                return true;
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
        ///  特店資料異動(依統編)明細清單匯出到Excel
        ///  修改紀錄:2021/01/19_Ares_Stanley-註解舊程式
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="strAgentName"></param>
        /// <param name="dtblWriteData"></param>
        /// <param name="strInputDate"></param>
        //private static void WriteDataToSheet_Fault(Worksheet sheet, string strAgentName, System.Data.DataTable dtblWriteData, string strInputDate,string strProperty)
        //{
            //Range range = null;
            //int intRowIndexInSheet = 0;
            //string[,] arrExportData = null;
            //if (strProperty.Trim().Equals("01030501"))
            //{
            //    arrExportData = new string[dtblWriteData.Rows.Count, 6];
            //}
            //else
            //{
            //    arrExportData = new string[dtblWriteData.Rows.Count, 8];
            //}

            ////*Page Title
            //range = sheet.get_Range("B4", Missing.Value);//起迄日期：
            //range.Value2 = strInputDate;

            //range = sheet.get_Range("B5", Missing.Value);//列印經辦：
            //range.Value2 = strAgentName;

            //range = sheet.get_Range("B6", Missing.Value);//列印日期：
            //range.Value2 = DateTime.Now.ToString("yyyy/MM/dd");
            

            //intRowIndexInSheet = 8;

            //for (int intLoop = 0; intLoop < dtblWriteData.Rows.Count; intLoop++)
            //{
            //    intRowIndexInSheet++;
            //    if (strProperty.Trim().Equals("01030501"))
            //    {
            //        //* 序號
            //        arrExportData[intRowIndexInSheet - 9, 0] = dtblWriteData.Rows[intLoop]["SEQ"].ToString();
            //        //* 統一編號
            //        arrExportData[intRowIndexInSheet - 9, 1] = dtblWriteData.Rows[intLoop]["CORP_NO"].ToString();
            //        //* 鍵一同仁
            //        arrExportData[intRowIndexInSheet - 9, 2] = dtblWriteData.Rows[intLoop]["USER1"].ToString();
            //        //* 鍵二同仁
            //        arrExportData[intRowIndexInSheet - 9, 3] = dtblWriteData.Rows[intLoop]["USER2"].ToString();
            //        //* 覆核同仁
            //        arrExportData[intRowIndexInSheet - 9, 4] = dtblWriteData.Rows[intLoop]["USER3"].ToString();
            //        //* 異動日期
            //        arrExportData[intRowIndexInSheet - 9, 5] = dtblWriteData.Rows[intLoop]["MOD_DATE"].ToString();
            //    }
            //    else
            //    {
            //        //* 作業日
            //        arrExportData[intRowIndexInSheet - 9, 0] = dtblWriteData.Rows[intLoop]["BATCH_DATE"].ToString();
            //        //* 統一編號
            //        arrExportData[intRowIndexInSheet - 9, 1] = dtblWriteData.Rows[intLoop]["CORP_NO"].ToString();
            //        //* 統編序號
            //        arrExportData[intRowIndexInSheet - 9, 2] = dtblWriteData.Rows[intLoop]["CORP_SEQ"].ToString();
            //        //* 商店代號
            //        arrExportData[intRowIndexInSheet - 9, 3] = dtblWriteData.Rows[intLoop]["MERCH_NO"].ToString();
            //        //* 狀態碼
            //        arrExportData[intRowIndexInSheet - 9, 4] = dtblWriteData.Rows[intLoop]["STATUS_CODE"].ToString();
            //        //* 解約原因
            //        arrExportData[intRowIndexInSheet - 9, 5] = dtblWriteData.Rows[intLoop]["TERMINATE_CODE"].ToString();
            //        //* 解約日期
            //        arrExportData[intRowIndexInSheet - 9, 6] = dtblWriteData.Rows[intLoop]["TERMINATE_DATE"].ToString();
            //        //* 更新筆數
            //        arrExportData[intRowIndexInSheet - 9, 7] = dtblWriteData.Rows[intLoop]["UPDATE_CNT"].ToString();
            //    }
            //}

            //if (strProperty.Trim().Equals("01030501"))
            //{
            //    range = sheet.get_Range("A9", "F" + intRowIndexInSheet.ToString());
            //}
            //else
            //{
            //    range = sheet.get_Range("A9", "H" + intRowIndexInSheet.ToString());
            //}
                
            //range.Font.Size = 12;// 字體大小
            //range.Font.Name = "新細明體";
            //range.Value2 = arrExportData;
            //range.Borders.LineStyle = 1;
            //range.EntireColumn.AutoFit();

            //// 總筆數
            //range = sheet.get_Range("A" + Convert.ToString(intRowIndexInSheet + 2), Missing.Value);
            //range.Value2 = "總筆數 : 共 " + dtblWriteData.Rows.Count + " 筆";
            //range.Font.Size = 12;// 字體大小
            //range.Font.Name = "新細明體";
            //range.Font.Bold = true;
        //}
        

        /// <summary>
        /// 匯出ACH授權扣款資料清單資料時，查詢數據
        /// </summary>
        /// <param name="dirValues">查詢條件</param>
        /// <param name="strBuildDate">鍵檔起迄日</param>
        /// <param name="strBatchNo">首錄起迄日</param>
        /// <param name="strBank_Code">行庫</param>
        /// <returns>DataTable(查詢結果)</returns>
        public static System.Data.DataTable getData_ChangeLog(Dictionary<string, string> dirValues,
                        string strProperty,
                        ref string strBuildDate)
        {
            SqlCommand sqlComm = new SqlCommand();
            StringBuilder sbWhere = new StringBuilder("");
            string dateBegin = "";
            string dateEnd = "";
            foreach (KeyValuePair<string, string> entry in dirValues)
            {
                //* 鍵檔起日
                if (entry.Key == "@DATE_BEGIN" && entry.Value != "")
                {
                    SqlParameter parmBuildDateStart = new SqlParameter("@DATE_BEGIN", entry.Value);
                    sqlComm.Parameters.Add(parmBuildDateStart);
                    strBuildDate = entry.Value;
                    dateBegin = entry.Value;
                }

                //* 鍵檔迄日
                if (entry.Key == "@DATE_END" && entry.Value != "")
                {
                    SqlParameter parmBuildDateEnd = new SqlParameter("@DATE_END", entry.Value);
                    sqlComm.Parameters.Add(parmBuildDateEnd);
                    strBuildDate += "~" + entry.Value;
                    dateEnd = entry.Value;
                }

            }

            //* 添加查詢條件
            if (strProperty.Trim().Equals("01030501"))
            {
                //2021/03/17_Ares_Stanley-DB名稱改為變數
                sqlComm.CommandText = string.Format(SEL_CHANGELOG, UtilHelper.GetAppSettings("DB_CSIP"));
            }
            else
            {
                sqlComm.CommandText = string.Format(SEL_MAINFRAMELOG);
            }
            
            sqlComm.CommandType = CommandType.Text;

            //20210517_Ares_Stanley-新增記錄SQL到Default LOG
            #region 記錄SQL到Default Log
            string recordSQL = sqlComm.CommandText;
            recordSQL = recordSQL.Replace("@DATE_BEGIN", string.Format("'{0}'", dateBegin)).Replace("@DATE_END", string.Format("'{0}'", dateEnd));
            Logging.Log("========== 執行作業量比對表-列印 ==========\r" + recordSQL);
            #endregion 記錄SQL到Default Log  

            //* 查詢并返回查詢結果
            return ((DataSet)CSIPKeyInGUI.BusinessRules.BRSHOP.SearchOnDataSet(sqlComm)).Tables[0];
        }
        /// <summary>
        /// 建立時間:2021/01/20_Ares_Stanley
        /// 匯出ACH授權扣款資料清單資料時，查詢數據_僅供查詢使用
        /// </summary>
        /// <param name="dirValues">查詢條件</param>
        /// <param name="strBuildDate">鍵檔起迄日</param>
        /// <param name="strBatchNo">首錄起迄日</param>
        /// <param name="strBank_Code">行庫</param>
        /// <returns>DataTable(查詢結果)</returns>
        public static System.Data.DataTable getData_ChangeLog_Search(Dictionary<string, string> dirValues, string strProperty, ref string strBuildDate, int iPageIndex, int iPageSize, ref int iTotalCount)
        {
            SqlCommand sqlComm = new SqlCommand();
            StringBuilder sbWhere = new StringBuilder("");
            string dateBegin = "";
            string dateEnd = "";
            foreach (KeyValuePair<string, string> entry in dirValues)
            {
                //* 鍵檔起日
                if (entry.Key == "@DATE_BEGIN" && entry.Value != "")
                {
                    SqlParameter parmBuildDateStart = new SqlParameter("@DATE_BEGIN", entry.Value);
                    sqlComm.Parameters.Add(parmBuildDateStart);
                    strBuildDate = entry.Value;
                    dateBegin = entry.Value;
                }

                //* 鍵檔迄日
                if (entry.Key == "@DATE_END" && entry.Value != "")
                {
                    SqlParameter parmBuildDateEnd = new SqlParameter("@DATE_END", entry.Value);
                    sqlComm.Parameters.Add(parmBuildDateEnd);
                    strBuildDate += "~" + entry.Value;
                    dateEnd = entry.Value;
                }

            }

            //* 添加查詢條件
            if (strProperty.Trim().Equals("01030501"))
            {
                //2021/03/17_Ares_Stanley-DB名稱改為變數
                sqlComm.CommandText = string.Format(SEL_CHANGELOG, UtilHelper.GetAppSettings("DB_CSIP"));
            }
            else
            {
                sqlComm.CommandText = string.Format(SEL_MAINFRAMELOG);
            }

            sqlComm.CommandType = CommandType.Text;

            //20210517_Ares_Stanley-新增記錄SQL到Default LOG
            #region 記錄SQL到Default Log
            string recordSQL = sqlComm.CommandText;
            recordSQL = recordSQL.Replace("@DATE_BEGIN", string.Format("'{0}'", dateBegin)).Replace("@DATE_END", string.Format("'{0}'", dateEnd));
            Logging.Log("========== 執行作業量比對表-查詢 ==========\r" + recordSQL);
            #endregion 記錄SQL到Default Log            

            //* 查詢并返回查詢結果
            System.Data.DataTable dtblResult = new System.Data.DataTable();
            dtblResult = ((DataSet)CSIPKeyInGUI.BusinessRules.BRSHOP.SearchOnDataSet(sqlComm)).Tables[0];
            iTotalCount = dtblResult.Rows.Count;
            // 判斷頁次
            for (int i = 0; i < 10 * (iPageIndex - 1); i++)
            {
                dtblResult.Rows.Remove(dtblResult.Rows[0]);
            }
            return dtblResult;
        }
    }
}