//******************************************************************
//*  作    者：蘇洺葳
//*  功能說明：表單欄位
//*  創建日期：2018/02/08
//*  修改紀錄：2021/04/01_Ares_Stanley-移除MicrosoftExcel
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*******************************************************************
using System;
using Framework.Data.OM;
using CSIPCommonModel.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using System.Data;
using Framework.Data.OM.Collections;
using System.Data.SqlClient;
using Framework.Common.Logging;
using System.Reflection;
using Framework.Common.Utility;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace CSIPCommonModel.BusinessRules
{
    public class BRUTILITIES : BRBase<EntityUTILITIES>
    {
        #region sql語句
        /// <summary>
        /// 修改日期:2020/12/28_Ares_Stanley-調整SQL查詢資料順序
        /// </summary>
        private const string SEL_M_UTILITIES = @" SELECT RECEIVE_NUMBER as APPLY_NO, BUSINESS_CATEGORY, PRIMARY_CARDHOLDER_ID as ACCT_ID, '' as REASON 
                                                , ERROR_INFORM_TYPE as INFORM_TYPE, REMARK 
                                                --欄位來源未知
                                                , '' as INFORM_DATE 
                                                FROM [UTILITIES] WHERE BUSINESS_CATEGORY = '001004' ";

        #endregion

        /// <summary>
        /// 查詢一、二Key資料
        /// 根據正卡人ID、收件編號、上傳主機識別碼
        /// </summary>
        /// <param name="eUtilitiesSet"></param>
        /// <param name="sendHostFlag"></param>
        /// <returns></returns>
        public static EntitySet<EntityUTILITIES> SelectEntitySet(EntityUTILITIES eUtilitiesSet, string sendHostFlag)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityUTILITIES.M_BUSINESS_CATEGORY, Operator.Equal, DataTypeUtils.String, eUtilitiesSet.BUSINESS_CATEGORY);
                sSql.AddCondition(EntityUTILITIES.M_RECEIVE_NUMBER, Operator.Equal, DataTypeUtils.String, eUtilitiesSet.RECEIVE_NUMBER);
                sSql.AddCondition(EntityUTILITIES.M_SENDHOST_FLAG, Operator.Equal, DataTypeUtils.String, sendHostFlag);

                return BRUTILITIES.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 查詢一、二Key資料
        /// 根據業務類別、收件編號、一二Key識別碼
        /// </summary>
        /// <param name="businessCategory"></param>
        /// <param name="recevieNumber"></param>
        /// <param name="keyInFlag"></param>
        /// <returns></returns>
        public static EntitySet<EntityUTILITIES> SelectEntitySet(string businessCategory, string recevieNumber, string keyInFlag)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityUTILITIES.M_BUSINESS_CATEGORY, Operator.Equal, DataTypeUtils.String, businessCategory);
                sSql.AddCondition(EntityUTILITIES.M_RECEIVE_NUMBER, Operator.Equal, DataTypeUtils.String, recevieNumber);
                sSql.AddCondition(EntityUTILITIES.M_KEYIN_FLAG, Operator.Equal, DataTypeUtils.String, keyInFlag);

                return BRUTILITIES.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 查詢一、二Key資料
        /// 根據業務類別、正卡人ID、收件編號、一二Key識別碼
        /// </summary>
        /// <param name="businessCategory"></param>
        /// <param name="id"></param>
        /// <param name="recevieNumber"></param>
        /// <param name="keyInFlag"></param>
        /// <returns></returns>
        public static EntitySet<EntityUTILITIES> SelectEntitySet(string businessCategory, string id, string recevieNumber, string keyInFlag)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityUTILITIES.M_BUSINESS_CATEGORY, Operator.Equal, DataTypeUtils.String, businessCategory);
                sSql.AddCondition(EntityUTILITIES.M_PRIMARY_CARDHOLDER_ID, Operator.Equal, DataTypeUtils.String, id);
                sSql.AddCondition(EntityUTILITIES.M_RECEIVE_NUMBER, Operator.Equal, DataTypeUtils.String, recevieNumber);
                sSql.AddCondition(EntityUTILITIES.M_KEYIN_FLAG, Operator.Equal, DataTypeUtils.String, keyInFlag);

                return BRUTILITIES.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 查詢一、二Key資料
        /// 收件編號、一二Key識別碼
        /// </summary>
        /// <param name="eUtilitiesSet"></param>
        /// <param name="sendHostFlag"></param>
        /// <returns></returns>
        public static EntitySet<EntityUTILITIES> SelectEntitySetByReceiveNumber(string receiveNumber, string keyInFlag)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityUTILITIES.M_RECEIVE_NUMBER, Operator.Equal, DataTypeUtils.String, receiveNumber);
                sSql.AddCondition(EntityUTILITIES.M_KEYIN_FLAG, Operator.Equal, DataTypeUtils.String, keyInFlag);

                return BRUTILITIES.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新公用事業
        /// </summary>
        /// <param name="eUtilities"></param>
        /// <param name="businessCategory"></param>
        /// <param name="receiveNumber"></param>
        /// <param name="keyInFlag"></param>
        /// <returns></returns>
        public static bool Update(EntityUTILITIES eUtilities, string businessCategory, string receiveNumber, string keyInFlag)
        {
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntityUTILITIES.M_BUSINESS_CATEGORY, Operator.Equal, DataTypeUtils.String, businessCategory);
            sSql.AddCondition(EntityUTILITIES.M_RECEIVE_NUMBER, Operator.Equal, DataTypeUtils.String, receiveNumber);
            sSql.AddCondition(EntityUTILITIES.M_KEYIN_FLAG, Operator.Equal, DataTypeUtils.String, keyInFlag);

            return eUtilities.DB_UpdateEntity(sSql.GetFilterCondition());
        }

        /// <summary>
        /// 新增公用事業
        /// </summary>
        /// <param name="eUtilities"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Add(EntityUTILITIES eUtilities, ref string strMsgID)
        {
            try
            {
                if (!IsRepeat(eUtilities))
                {
                    if (BRUTILITIES.AddNewEntity(eUtilities))
                    {
                        if (eUtilities.KEYIN_FLAG == "1")
                            strMsgID = "01_01011200_009";
                        else
                            strMsgID = "01_01011300_009";
                        
                        return true;
                    }
                }

                strMsgID = "01_01011200_013";
                return false;
            }
            catch (Exception ex)
            {
                if (eUtilities.KEYIN_FLAG == "1")
                    strMsgID = "01_01011200_010";
                else
                    strMsgID = "01_01011300_010";

                throw ex;
            }
        }

        /// <summary>
        /// 問題件查詢
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="plateNO"></param>
        /// <param name="keyInDate"></param>
        /// <param name="dtblResult"></param>
        /// <param name="intPageIndex"></param>
        /// <param name="intPageSize"></param>
        /// <param name="intTotolCount"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool GetUtilities(string ID, string proType, string spc, string spcID, ref System.Data.DataTable dtblResult
                                        , int intPageIndex, int intPageSize, ref int intTotolCount, ref string strMsgID)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = SEL_M_UTILITIES;
            sqlcmd.CommandType = CommandType.Text;

            // 正卡人ID
            if (!string.IsNullOrEmpty(ID))
            {
                sqlcmd.CommandText += "AND PRIMARY_CARDHOLDER_ID = @PRIMARY_CARDHOLDER_ID ";
                sqlcmd.Parameters.Add(new SqlParameter("@PRIMARY_CARDHOLDER_ID", ID));
            }

            // 產品別
            //if (!string.IsNullOrEmpty(proType))
            //{
            //    sqlcmd.CommandText += "AND BUSINESS_CATEGORY = @PRO_TYPE ";
            //    sqlcmd.Parameters.Add(new SqlParameter("@PRO_TYPE", proType));
            //}

            // 推廣代號
            if (!string.IsNullOrEmpty(spc))
            {
                sqlcmd.CommandText += "AND POPUL_NO = @POPUL_NO ";
                sqlcmd.Parameters.Add(new SqlParameter("@POPUL_NO", spc));
            }

            // (種子)推廣員編
            if (!string.IsNullOrEmpty(spcID))
            {
                sqlcmd.CommandText += "AND POPUL_EMP_NO = @POPUL_EMP_NO ";
                sqlcmd.Parameters.Add(new SqlParameter("@POPUL_EMP_NO", spcID));
            }

            try
            {
                dtblResult = SearchOnDataSet(sqlcmd, intPageIndex, intPageSize, ref intTotolCount).Tables[0];
                return true;
            }
            catch (Exception ex)
            {
                SaveLog(ex);
                strMsgID = "00_00000000_000";
                return false;
            }
        }

        /// <summary>
        /// 判斷是否重復
        /// </summary>
        /// <param name="eUtilities"></param>
        /// <returns>Repeat true, unrepeat false</returns>
        public static bool IsRepeat(EntityUTILITIES eUtilities)
        {
            SqlHelper sql = new SqlHelper();

            sql.AddCondition(EntityUTILITIES.M_BUSINESS_CATEGORY, Operator.Equal, DataTypeUtils.String, eUtilities.BUSINESS_CATEGORY);
            sql.AddCondition(EntityUTILITIES.M_RECEIVE_NUMBER, Operator.Equal, DataTypeUtils.String, eUtilities.RECEIVE_NUMBER);
            sql.AddCondition(EntityUTILITIES.M_KEYIN_FLAG, Operator.Equal, DataTypeUtils.String, eUtilities.KEYIN_FLAG);

            if (BRUTILITIES.Search(sql.GetFilterCondition()).Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 匯出成Excel
        /// 修改記錄：2020/12/28_Ares_Stanley-變更報表產出方式為NPOI
        /// </summary>
        /// <param name="dtData"></param>
        /// <param name="strAgentName"></param>
        /// <param name="strPathFile"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile(System.Data.DataTable dtData, string strAgentName, ref string strPathFile, ref string strMsgID)
        {
            try
            {
                // 檢查目錄，並刪除以前的文檔資料
                BRExcel_File.CheckDirectory(ref strPathFile);

                // 取要下載的資料
                string strInputDate = "";


                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "Utilities.xls";
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open); //開啟範本
                HSSFWorkbook wb = new HSSFWorkbook(fs);  //新建workbook
                #region 總計文字格式
                HSSFCellStyle contentFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
                contentFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
                contentFormat.Alignment = HorizontalAlignment.Left; //水平置左
                contentFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字
                HSSFFont contentFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                contentFont.FontHeightInPoints = 12; //字體大小
                contentFont.FontName = "新細明體"; //字型
                contentFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold; //粗體
                contentFormat.SetFont(contentFont); //設定儲存格的文字樣式
                #endregion 總計文字格式
                ISheet sheet = wb.GetSheet("批次結果報表");
                ExportExcelForNPOI(dtData, ref wb, 7, "批次結果報表"); // 使用ExportExcelForNPOI function逐筆寫入資料
                // 表頭
                sheet.GetRow(3).GetCell(1).CellStyle = sheet.GetRow(3).GetCell(0).CellStyle;
                sheet.GetRow(3).GetCell(1).SetCellValue(strAgentName);
                sheet.GetRow(4).GetCell(1).CellStyle = sheet.GetRow(4).GetCell(0).CellStyle;
                string strYYYYMMDD = "000" + BaseItem.Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
                sheet.GetRow(4).GetCell(1).SetCellValue(strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8));
                // 表尾
                sheet.CreateRow(sheet.LastRowNum + 2).CreateCell(0).SetCellValue("總筆數 : 共 " + dtData.Rows.Count.ToString() + " 筆");
                sheet.GetRow(sheet.LastRowNum).GetCell(0).CellStyle = contentFormat;


                // 保存文件到程序運行目錄下
                strPathFile = strPathFile + @"\ExcelFile_Utilities_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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
        /// 寫入Excel
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="strAgentName"></param>
        /// <param name="dtblWriteData"></param>
        /// <param name="strInputDate"></param>
        /// 修改紀錄:2021/04/01_Ares_Stanley-註解未使用程式碼
        #region 未使用程式碼-WriteDataToSheet_Fault
        //private static void WriteDataToSheet_Fault(Worksheet sheet, string strAgentName, System.Data.DataTable dtblWriteData, string strInputDate)
        //{
        //    Range range = null;
        //    int intRowIndexInSheet = 0;
        //    string[,] arrExportData = null;
        //    arrExportData = new string[dtblWriteData.Rows.Count, 7];

        //    range = sheet.get_Range("B3", Missing.Value);
        //    range.Value2 = strInputDate;

        //    range = sheet.get_Range("B4", Missing.Value);
        //    range.Value2 = strAgentName;

        //    string strYYYYMMDD = "000" + BaseItem.Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
        //    range = sheet.get_Range("B5", Missing.Value);
        //    range.Value2 = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);

        //    intRowIndexInSheet = 7;

        //    for (int intLoop = 0; intLoop < dtblWriteData.Rows.Count; intLoop++)
        //    {
        //        intRowIndexInSheet++;

        //        // 收件編號
        //        arrExportData[intRowIndexInSheet - 8, 0] = dtblWriteData.Rows[intLoop]["APPLY_NO"].ToString();

        //        // 產品別 
        //        arrExportData[intRowIndexInSheet - 8, 1] = dtblWriteData.Rows[intLoop]["BUSINESS_CATEGORY"].ToString();

        //        // 正卡人ID
        //        arrExportData[intRowIndexInSheet - 8, 2] = dtblWriteData.Rows[intLoop]["ACCT_ID"].ToString();

        //        // 問題件原因
        //        arrExportData[intRowIndexInSheet - 8, 3] = dtblWriteData.Rows[intLoop]["INFORM_TYPE"].ToString();

        //        // 通知方式
        //        arrExportData[intRowIndexInSheet - 8, 4] = dtblWriteData.Rows[intLoop]["REMARK"].ToString();

        //        // 備註
        //        arrExportData[intRowIndexInSheet - 8, 5] = dtblWriteData.Rows[intLoop]["REASON"].ToString();

        //        // 通知日期
        //        arrExportData[intRowIndexInSheet - 8, 6] = dtblWriteData.Rows[intLoop]["INFORM_DATE"].ToString();
        //    }

        //    range = sheet.get_Range("A8", "G" + intRowIndexInSheet.ToString());
        //    range.Font.Size = 12;// 字體大小
        //    range.Font.Name = "新細明體";
        //    range.Value2 = arrExportData;
        //    range.Borders.LineStyle = 1;
        //    range.EntireColumn.AutoFit();

        //    // 總筆數
        //    range = sheet.get_Range("A" + Convert.ToString(intRowIndexInSheet + 2), Missing.Value);
        //    //range.Value2 = "總筆數 : 共 " + dtblWriteData.Rows.Count + " 筆";
        //    range.Font.Size = 12;// 字體大小
        //    range.Font.Name = "新細明體";
        //    range.Font.Bold = true;
        //}
        #endregion 未使用程式碼-WriteDataToSheet_Fault

        #region 共用NPOI
        /// <summary>
        /// 修改紀錄:2020/12/28_Ares_Stanley-新增共用NPOI
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="wb"></param>
        /// <param name="start"></param>
        /// <param name="sheetName"></param>
        private static void ExportExcelForNPOI(System.Data.DataTable dt, ref HSSFWorkbook wb, Int32 start, String sheetName)
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
        /// 修改紀錄: 2020/12/28_Ares_Stanley-新增共用NPOI
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="wb"></param>
        /// <param name="start"></param>
        /// <param name="delColumn"></param>
        /// <param name="sheetName"></param>
        private static void ExportExcelForNPOI_filter(System.Data.DataTable dt, ref HSSFWorkbook wb, Int32 start, Int32 delColumn, String sheetName)
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
