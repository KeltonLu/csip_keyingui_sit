//******************************************************************
//*  作    者：林家賜
//*  功能說明：收單特店審查處理  SCDDReport
//*  創建日期：2019/01/24
//*  修改紀錄：2021/01/25_Ares_Stanley-新增NPOI; 2021/04/01_Ares_Stanley-移除MicrosoftExcel
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*******************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Framework.Data.OM;
using CSIPNewInvoice.EntityLayer_new;
using CSIPKeyInGUI.BusinessRules;
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.BusinessRules;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using System.IO;
using System.Configuration;
using Framework.Common;
using Framework.Common.Logging;
using System.Reflection;
using Framework.Common.Utility;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.HSSF.EventUserModel.DummyRecord;
using NPOI.XSSF.UserModel.Charts;
/// <summary>
/// BRAML_Cdata_Work 的摘要描述
/// </summary>
public class BRSCDDReport : CSIPCommonModel.BusinessRules.BRBase<EntitySCDDReport>
{
    public BRSCDDReport()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }
    /// <summary>
    /// 讀取案件明細表頭
    /// </summary>
    /// <returns></returns>
    public static EntitySCDDReport getSCDDReport(AML_SessionState parmObj)
    {
        string sSQL = @"  
       select SR_CASE_NO,SR_DateTime,SR_User,SR_RiskLevel,SR_RiskItem,SR_RiskNote,SR_Explanation
from [SCDDReport] where 1=1    ";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;

        if (parmObj != null)
        {
            if (!String.IsNullOrEmpty(parmObj.CASE_NO))
            {
                sSQL += " and  SR_CASE_NO = @SR_CASE_NO";
                sqlcmd.Parameters.Add(new SqlParameter("@SR_CASE_NO", parmObj.CASE_NO));
            } 
        }
        sqlcmd.CommandText = sSQL;
        EntitySCDDReport rtnObj = new EntitySCDDReport();
        System.Data.DataTable dt = new System.Data.DataTable();
        DataSet DS = SearchOnDataSet(sqlcmd);
        if (DS != null && DS.Tables.Count > 0)
        {
            dt = DS.Tables[0];
        }
        if (dt.Rows.Count > 0)
        {
            DataTableConvertor.convSingRow<EntitySCDDReport>(ref rtnObj, dt.Rows[0]);
        }
        return rtnObj;
    }
    /// <summary>
    /// SCDD報表列印
    /// 修改紀錄:2021/01/25_Ares_Stanley-變更報表產出方式為NPOI
    /// </summary>
    /// <param name="dsData"></param>
    /// <param name="strAgentName"></param>
    /// <param name="strPathFile"></param>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public static bool CreateExcelFile(clsSCDDPrint dsData, string strAgentName, ref string strPathFile, ref string strMsgID, string fileName = "")
    {
        try
        {
            //* 檢查目錄，并刪除以前的文檔資料
            BRExcel_File.CheckDirectory(ref strPathFile);

            //* 取要下載的資料
            string strInputDate = "";            
            string excelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + (fileName == "" ? "SCDD_VIEW_NEW.xls" : fileName);//20210806 EOS_AML(NOVA) 自然人SCDD列印範本 fileName為空是原本SCDD範本 by Ares Dennis
            FileStream fs = new FileStream(excelPathFile, FileMode.Open); //開啟範本
            HSSFWorkbook wb = new HSSFWorkbook(fs);  //新建workbook
            ISheet sheet1 = wb.GetSheetAt(0);
            ISheet sheet_Foot = wb.GetSheetAt(1);
            #region 儲存格樣式
            // 關聯人國籍格式
            HSSFCellStyle contentFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
            contentFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
            contentFormat.Alignment = HorizontalAlignment.Left; //水平置中
            contentFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字
            contentFormat.BorderBottom = BorderStyle.Thin; // 儲存格框線
            contentFormat.BorderLeft = BorderStyle.Thin;
            contentFormat.BorderTop = BorderStyle.Thin;
            contentFormat.BorderRight = BorderStyle.Thin;
            HSSFFont contentFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
            contentFont.FontHeightInPoints = 9; //字體大小
            contentFont.FontName = "新細明體"; //字型
            contentFormat.SetFont(contentFont);
            // 經辦簽章日期格式
            HSSFCellStyle dateFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
            dateFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
            dateFormat.Alignment = HorizontalAlignment.Right; //水平置中
            dateFormat.BorderLeft = BorderStyle.Thin;
            dateFormat.BorderRight = BorderStyle.Thin;
            dateFormat.BorderBottom = BorderStyle.Thin;
            HSSFFont dateFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
            dateFont.FontHeightInPoints = 9; //字體大小
            dateFont.FontName = "新細明體"; //字型
            dateFormat.SetFont(dateFont);
            #endregion
            #region 寫入上半部資料(關連人國籍前)
            Type v = dsData.GetType();
            PropertyInfo[] props = v.GetProperties();

            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true); //取得自訂屬性，第一個物件
                AttributeRFPrint authAttr;
                for (int xi = 0; xi < attrs.Length; xi++)
                {
                    if (attrs[xi] is AttributeRFPrint)
                    {
                        authAttr = attrs[xi] as AttributeRFPrint;

                        string cellRange = authAttr.CellRange;
                        string exVal = prop.GetValue(dsData, null) as string;
                        //有設定欄位,將欄位值寫入
                        if (!string.IsNullOrEmpty(cellRange))
                        {
                            var cr = new CellReference(cellRange);
                            sheet1.GetRow(cr.Row).GetCell(cr.Col).SetCellValue(exVal);
                        }
                    }
                }
            }
            #endregion 寫入上半部資料(關連人國籍前)
            #region 寫入下半部資料(關連人國籍後)            
            int managerCollCount = 25; // 關聯人起始列
            if (dsData.ManagerColl.Count > 0 && fileName == "")//20210806 EOS_AML(NOVA) 自然人SCDD列印範本 fileName為空是原本SCDD範本 by Ares Dennis
            {
                sheet1.ShiftRows(25, 33, dsData.ManagerColl.Count);
                foreach (clsMangger crM in dsData.ManagerColl)
                {
                    // 建立新列
                    sheet1.CreateRow(managerCollCount);
                    for (int col = 0; col < 8; col++)
                    {
                        sheet1.GetRow(managerCollCount).CreateCell(col).CellStyle = contentFormat;
                    }
                    // 合併儲存格
                    sheet1.AddMergedRegion(new CellRangeAddress(managerCollCount, managerCollCount, 2, 4));
                    sheet1.AddMergedRegion(new CellRangeAddress(managerCollCount, managerCollCount, 5, 7));
                    // 姓名
                    sheet1.GetRow(managerCollCount).GetCell(0).SetCellValue(crM.Name);
                    // 國籍
                    sheet1.GetRow(managerCollCount).GetCell(1).SetCellValue(crM.Nation);
                    // 中文長姓名
                    sheet1.GetRow(managerCollCount).GetCell(2).SetCellValue(crM.Lname);
                    // 羅馬姓名
                    sheet1.GetRow(managerCollCount).GetCell(5).SetCellValue(crM.Romaname);
                    managerCollCount += 1;
                }
            }
            // 計算風險等級
            sheet1.GetRow(sheet1.LastRowNum - 7).GetCell(1).SetCellValue(dsData.SR_RiskItem);
            // 綜合說明及審查意見(高風險客戶業務往來必填)
            sheet1.GetRow(sheet1.LastRowNum - 6).GetCell(1).SetCellValue(dsData.SR_Explanation);
            // 本人OOO(簽名或蓋章)已就上述客戶進行盡職審查。憑著本表格內所填報的資料，依本人所知道及相信，本人認為客戶及其資金來源根據中國信託內部政策及程序及於有關國家/地區適用的防制洗錢法律均屬合法及可接受。
            sheet1.GetRow(sheet1.LastRowNum - 5).GetCell(0).CellStyle = contentFormat;
            sheet1.GetRow(sheet1.LastRowNum - 5).GetCell(0).SetCellValue(dsData.lbLabel9);
            sheet1.GetRow(sheet1.LastRowNum - 5).Height = 32 * 20;
            // 經辦(簽章)
            sheet1.GetRow(sheet1.LastRowNum - 4).GetCell(1).SetCellValue(dsData.lbLabel1);
            // 經辦(簽章)日期
            sheet1.GetRow(sheet1.LastRowNum).GetCell(1).CellStyle = dateFormat;
            sheet1.GetRow(sheet1.LastRowNum).GetCell(1).SetCellValue(dsData.lbLabel2.Substring(0, 16));
            #endregion 寫入下半部資料(關連人國籍後)
            
            {
                
            }


            //* 保存文件到程序运行目录下
            strPathFile = strPathFile + @"\SCDDReport_" + DateTime.Now.ToString("yyyy") + ".xls";
            //舊檔案存在則先刪除
            if (File.Exists(strPathFile))
            {
                File.Delete(strPathFile);
            }
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
            //throw ex;
            return false;
        }
        finally
        {
        }
    }

    /// 20211129_Ares_Jack
    /// <summary>
    /// SCDD自然人報表列印
    /// </summary>
    /// <param name="dsData"></param>
    /// <param name="strAgentName"></param>
    /// <param name="strPathFile"></param>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public static bool CreateExcelFileNaturalPerson(clsSCDDPrintNaturalPerson dsData, string strAgentName, ref string strPathFile, ref string strMsgID, string fileName = "")
    {
        try
        {
            //* 檢查目錄，并刪除以前的文檔資料
            BRExcel_File.CheckDirectory(ref strPathFile);

            //* 取要下載的資料
            string strInputDate = "";
            string excelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + (fileName == "" ? "SCDD_VIEW_NEW.xls" : fileName);//20210806 EOS_AML(NOVA) 自然人SCDD列印範本 fileName為空是原本SCDD範本 by Ares Dennis
            FileStream fs = new FileStream(excelPathFile, FileMode.Open); //開啟範本
            HSSFWorkbook wb = new HSSFWorkbook(fs);  //新建workbook
            ISheet sheet1 = wb.GetSheetAt(0);
            ISheet sheet_Foot = wb.GetSheetAt(1);
            #region 儲存格樣式
            // 關聯人國籍格式
            HSSFCellStyle contentFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
            contentFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
            contentFormat.Alignment = HorizontalAlignment.Left; //水平置中
            contentFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字
            contentFormat.BorderBottom = BorderStyle.Thin; // 儲存格框線
            contentFormat.BorderLeft = BorderStyle.Thin;
            contentFormat.BorderTop = BorderStyle.Thin;
            contentFormat.BorderRight = BorderStyle.Thin;
            HSSFFont contentFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
            contentFont.FontHeightInPoints = 9; //字體大小
            contentFont.FontName = "新細明體"; //字型
            contentFormat.SetFont(contentFont);
            // 經辦簽章日期格式
            HSSFCellStyle dateFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
            dateFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
            dateFormat.Alignment = HorizontalAlignment.Right; //水平置中
            dateFormat.BorderLeft = BorderStyle.Thin;
            dateFormat.BorderRight = BorderStyle.Thin;
            dateFormat.BorderBottom = BorderStyle.Thin;
            HSSFFont dateFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
            dateFont.FontHeightInPoints = 9; //字體大小
            dateFont.FontName = "新細明體"; //字型
            dateFormat.SetFont(dateFont);
            #endregion
            #region 寫入上半部資料(關連人國籍前)
            Type v = dsData.GetType();
            PropertyInfo[] props = v.GetProperties();

            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true); //取得自訂屬性，第一個物件
                AttributeRFPrint authAttr;
                for (int xi = 0; xi < attrs.Length; xi++)
                {
                    if (attrs[xi] is AttributeRFPrint)
                    {
                        authAttr = attrs[xi] as AttributeRFPrint;

                        string cellRange = authAttr.CellRange;
                        string exVal = prop.GetValue(dsData, null) as string;
                        //有設定欄位,將欄位值寫入
                        if (!string.IsNullOrEmpty(cellRange))
                        {
                            var cr = new CellReference(cellRange);
                            sheet1.GetRow(cr.Row).GetCell(cr.Col).SetCellValue(exVal);
                        }
                    }
                }
            }
            #endregion 寫入上半部資料(關連人國籍前)
            #region 寫入下半部資料(關連人國籍後)            
            int managerCollCount = 25; // 關聯人起始列
            if (dsData.ManagerColl.Count > 0 && fileName == "")//20210806 EOS_AML(NOVA) 自然人SCDD列印範本 fileName為空是原本SCDD範本 by Ares Dennis
            {
                sheet1.ShiftRows(25, 33, dsData.ManagerColl.Count);
                foreach (clsMangger crM in dsData.ManagerColl)
                {
                    // 建立新列
                    sheet1.CreateRow(managerCollCount);
                    for (int col = 0; col < 8; col++)
                    {
                        sheet1.GetRow(managerCollCount).CreateCell(col).CellStyle = contentFormat;
                    }
                    // 合併儲存格
                    sheet1.AddMergedRegion(new CellRangeAddress(managerCollCount, managerCollCount, 2, 4));
                    sheet1.AddMergedRegion(new CellRangeAddress(managerCollCount, managerCollCount, 5, 7));
                    // 姓名
                    sheet1.GetRow(managerCollCount).GetCell(0).SetCellValue(crM.Name);
                    // 國籍
                    sheet1.GetRow(managerCollCount).GetCell(1).SetCellValue(crM.Nation);
                    // 中文長姓名
                    sheet1.GetRow(managerCollCount).GetCell(2).SetCellValue(crM.Lname);
                    // 羅馬姓名
                    sheet1.GetRow(managerCollCount).GetCell(5).SetCellValue(crM.Romaname);
                    managerCollCount += 1;
                }
            }
            // 計算風險等級
            sheet1.GetRow(sheet1.LastRowNum - 7).GetCell(1).SetCellValue(dsData.SR_RiskItem);
            // 綜合說明及審查意見(高風險客戶業務往來必填)
            sheet1.GetRow(sheet1.LastRowNum - 6).GetCell(1).SetCellValue(dsData.SR_Explanation);
            // 本人OOO(簽名或蓋章)已就上述客戶進行盡職審查。憑著本表格內所填報的資料，依本人所知道及相信，本人認為客戶及其資金來源根據中國信託內部政策及程序及於有關國家/地區適用的防制洗錢法律均屬合法及可接受。
            sheet1.GetRow(sheet1.LastRowNum - 5).GetCell(0).CellStyle = contentFormat;
            sheet1.GetRow(sheet1.LastRowNum - 5).GetCell(0).SetCellValue(dsData.lbLabel9);
            sheet1.GetRow(sheet1.LastRowNum - 5).Height = 32 * 20;
            // 經辦(簽章)
            sheet1.GetRow(sheet1.LastRowNum - 4).GetCell(1).SetCellValue(dsData.lbLabel1);
            // 經辦(簽章)日期
            sheet1.GetRow(sheet1.LastRowNum).GetCell(1).CellStyle = dateFormat;
            sheet1.GetRow(sheet1.LastRowNum).GetCell(1).SetCellValue(dsData.lbLabel2.Substring(0, 16));
            #endregion 寫入下半部資料(關連人國籍後)

            //* 保存文件到程序运行目录下
            strPathFile = strPathFile + @"\SCDDReport_" + DateTime.Now.ToString("yyyy") + ".xls";
            //舊檔案存在則先刪除
            if (File.Exists(strPathFile))
            {
                File.Delete(strPathFile);
            }
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
            //throw ex;
            return false;
        }
        finally
        {
        }
    }
}