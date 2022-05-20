//******************************************************************
//*  作    者：
//*  功能說明：Artificial_Signing_Batch_Data資料庫信息
//*  創建日期：2014/07/30
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Framework.Data.OM;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using System.Data.SqlClient;
using Microsoft.Office.Interop.Excel;
using Framework.Common.Logging;
using System.IO;

namespace CSIPKeyInGUI.BusinessRules
{
    public class BRArtificial_Signing_Batch_Data : CSIPCommonModel.BusinessRules.BRBase<EntityArtificial_Signing_Batch_Data>
    {

        #region SQL

        //*以[編批日期]及[收件批次],加上匯入檔中的批號,檢查[人工簽單批次資料檔]是否已有資料
        public const string SEL_ASBD_Import_Check = @"Select Shop_ID from Artificial_Signing_Batch_Data Where Batch_Date = @Batch_Date and Receive_Batch = @Receive_Batch and Batch_NO = @Batch_NO and Sign_Type = @Sign_Type ";

        //*一KEY 資料查詢
        public const string SEL_ASBD_1KEY = @"Select * from Artificial_Signing_Batch_Data Where Batch_Date = @Batch_Date and Receive_Batch = @Receive_Batch and Batch_NO = @Batch_NO and Shop_ID = @Shop_ID and Sign_Type = @Sign_Type ";
        public const string SEL_ASBD_1KEY_2 = @"Select * from Artificial_Signing_Batch_Data Where Batch_Date = @Batch_Date and Sign_Type = @Sign_Type ";
        #endregion

        /// <summary>
        /// 由EXCEL檔匯入資料
        /// </summary>
        /// <param name="strPath">EXCEL檔路徑</param>
        /// <param name="FileName">EXCEL檔名</param>
        /// <param name="userid">匯入人員ID</param>
        /// <param name="strMsgID">匯入作業訊息</param>
        /// <param name="total">匯入檔總筆數</param>
        /// <param name="insertcount">新增筆數</param>
        /// <param name="updatecount">更新筆數</param>
        /// <param name="insertfail">新增失敗筆數</param>
        /// <param name="updatefail">更新失敗筆數</param>
        /// <returns>true:匯入成功,false:匯入失敗</returns>
        public static bool Import(string strPath, string userid, ref string strMsgID, ref int total, ref int insertcount, ref int updatecount, ref int insertfail, ref int updatefail)
        {
            //解決"格式太舊或是類型程式庫無效。 (Exception from HRESULT: 0x80028018 (TYPE_E_INVDATAREAD))"問題用
            System.Globalization.CultureInfo oldCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            int iresult = 0;        //int.TryParse使用
            string FileName = string.Empty; //檔名(不包含副檔名)

            Microsoft.Office.Interop.Excel.Application xlApp = null;
            Workbook wb = null;
            Worksheet ws = null;
            Range aRange = null;

            string importDatetime = DateTime.Now.ToString();
            FileName = Path.GetFileNameWithoutExtension(strPath);

            //從檔名取出[編批日期]及[收件批次]
            string Batch_Date = string.Empty;   //編批日期(yyyyMMdd)
            string Receive_Batch = string.Empty;//收件批次
            string Sign_Type = string.Empty;    //簽單類別；一般：1，分期：2
            if (FileName.Substring(0, 1) == "P")
            {
                //*分期
                Batch_Date = ChangCalender(FileName.Substring(1, 7));
                Receive_Batch = FileName.Substring(8, 1);
                Sign_Type = "2";
            }
            else
            {
                //*一般
                Batch_Date = ChangCalender(FileName.Substring(0, 7));
                Receive_Batch = FileName.Substring(7, 1);
                Sign_Type = "1";
            }
            if (Batch_Date == string.Empty)
            {
                //*編批日期格式錯誤
                strMsgID = "01_01060100_004";
                return false;
            }
            if (!int.TryParse(Receive_Batch,out iresult))
            {
                //*收件批次格式錯誤
                strMsgID = "01_01060100_005";
                return false;
            }

            //20140904  編批日期不可小於當日
            //if (Convert.ToDateTime(Batch_Date.Substring(0,4) + "/" + Batch_Date.Substring(4,2) + "/" + Batch_Date.Substring(6,2)).CompareTo(DateTime.Now.Date) < 0)
            if (DateTime.ParseExact(Batch_Date, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces).CompareTo(DateTime.Now.Date) < 0)
            {
                strMsgID = "01_01060100_006";
                return false;
            }

            try
            {
                if (xlApp == null)
                {
                    xlApp = new Microsoft.Office.Interop.Excel.Application();
                }
                //打開Server上的Excel檔案
                xlApp.Workbooks.Open(strPath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                wb = xlApp.Workbooks[1];//第一個Workbook

                #region 從第一個Worksheet讀資料寫入DB
                ws = (Worksheet)xlApp.Worksheets[1];
                //要開始讀取的起始列(微軟Worksheet是從1開始算)
                int rowIndex = 1; 

                //取得一列的範圍
                aRange = ws.get_Range("A" + rowIndex.ToString(), "E" + rowIndex.ToString());

                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    //判斷Row範圍裡第1格有值的話，迴圈就往下跑
                    while (((object[,])aRange.Value2)[1, 1] != null)
                    {
                        //頁次
                        string page = ((object[,])aRange.Value2)[1, 1] != null ? ((object[,])aRange.Value2)[1, 1].ToString() : "";
                        //批號
                        string Batch_NO = ((object[,])aRange.Value2)[1, 2] != null ? ((object[,])aRange.Value2)[1, 2].ToString() : "";
                        //特店代號
                        string Shop_ID = ((object[,])aRange.Value2)[1, 3] != null ? ((object[,])aRange.Value2)[1, 3].ToString() : "";
                        //請款筆數 (收件-總筆數)
                        string Receive_Total_Count = ((object[,])aRange.Value2)[1, 4] != null ? ((object[,])aRange.Value2)[1, 4].ToString() : "";
                        //請款金額 (收件-總金額)
                        string Receive_Total_AMT = ((object[,])aRange.Value2)[1, 5] != null ? ((object[,])aRange.Value2)[1, 5].ToString() : "";

                        //頁次不為數值或特店代號為空不需處理
                        if (int.TryParse(page, out iresult) && Shop_ID != "")
                        {
                            //統計匯入檔總筆數，排除頁次不為數值或特店代號為空
                            total++;

                            SqlCommand sqlcmm = new SqlCommand();

                            //若無資料則新增,若有資料則只更新該筆資料的商店代號
                            DataSet ds = BRArtificial_Signing_Batch_Data.Select_Import_Check(Batch_Date, Receive_Batch, Batch_NO, Sign_Type);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                if (ds.Tables[0].Rows[0][0].ToString() != Shop_ID)
                                {
                                    //有資料且商店代號不同只更新該筆資料的商店代號，產生UPDATE語法
                                    sqlcmm.CommandText = string.Format("Update [KeyinGUI].[dbo].[Artificial_Signing_Batch_Data] Set [Shop_ID] = '{0}',[Modify_User] = '{1}',[Modify_DateTime] = '{2}' Where Batch_Date = '{3}' and Receive_Batch = {4} and Batch_NO = {5} and Sign_Type = {6};"
                                        , Shop_ID, userid, importDatetime, Batch_Date, Receive_Batch, Batch_NO, Sign_Type);
                                    if (BRArtificial_Signing_Batch_Data.Update(sqlcmm))
                                    {
                                        updatecount++;
                                    }
                                    else
                                    {
                                        updatefail++;
                                    }
                                }
                            }
                            else
                            {
                                //若無資料則新增，產生INSERT語法
                                sqlcmm.CommandText = string.Format("Insert into [KeyinGUI].[dbo].[Artificial_Signing_Batch_Data] ([Batch_Date],[Receive_Batch],[Page],[Batch_NO],[Shop_ID],[Sign_Type],[Receive_Total_Count],[Receive_Total_AMT],[Process_Flag],[Create_User],[Create_DateTime]) Values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}');"
                                    , Batch_Date, Receive_Batch, page, Batch_NO, Shop_ID, Sign_Type, Receive_Total_Count, Receive_Total_AMT, "01", userid, importDatetime);
                                if (BRArtificial_Signing_Batch_Data.Add(sqlcmm))
                                {
                                    insertcount++;
                                }
                                else
                                {
                                    insertfail++;
                                }
                            }
                        }
                        //往下抓一列Excel範圍
                        rowIndex++;
                        aRange = ws.get_Range("A" + rowIndex.ToString(), "E" + rowIndex.ToString());
                    }
                    ts.Complete();
                    //匯入成功訊息
                    strMsgID = "01_01060100_001";
                }
                #endregion
                if ((insertfail + updatefail) > 0)
                {
                    strMsgID = "01_01060100_002";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Logging.SaveLog(ELogLayer.BusinessRule, ex);
                //匯入失敗訊息
                strMsgID = "01_01060100_002";
                return false;
            }
            finally
            {
                #region 關閉EXCEL
                wb.Close(false, Type.Missing, Type.Missing);
                xlApp.Workbooks.Close();
                xlApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ws);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(aRange);
                xlApp = null;
                wb = null;
                ws = null;
                aRange = null;
                GC.Collect();
                #endregion

                //是否刪除Server上的Excel檔
                bool isDeleteFileFromServer = false;
                if (isDeleteFileFromServer)
                {
                    System.IO.File.Delete(strPath);
                }

                //解決"格式太舊或是類型程式庫無效。 (Exception from HRESULT: 0x80028018 (TYPE_E_INVDATAREAD))"問題用
                System.Threading.Thread.CurrentThread.CurrentCulture = oldCI;
                
            }
        }

        /// <summary>
        /// 匯入時查詢是否已有資料
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strBatch_NO">批號</param>
        /// <returns></returns>        
        public static DataSet Select_Import_Check(string strBatch_Date, string strReceive_Batch, string strBatch_NO, string strSign_Type)
        {
            DataSet ds = null;
            SqlHelper sSql = new SqlHelper();

            SqlCommand sqlComm = new SqlCommand();
            try
            {
                sqlComm.CommandText = SEL_ASBD_Import_Check;
                sqlComm.CommandType = CommandType.Text;

                SqlParameter parmBatch_Date = new SqlParameter("@Batch_Date", strBatch_Date);
                SqlParameter parmReceive_Batch = new SqlParameter("@Receive_Batch", strReceive_Batch);
                SqlParameter parmBatch_NO = new SqlParameter("@Batch_NO", strBatch_NO);
                SqlParameter parmSign_Type = new SqlParameter("@Sign_Type", strSign_Type);

                sqlComm.Parameters.Add(parmBatch_Date);
                sqlComm.Parameters.Add(parmReceive_Batch);
                sqlComm.Parameters.Add(parmBatch_NO);
                sqlComm.Parameters.Add(parmSign_Type);

                ds = BRArtificial_Signing_Batch_Data.SearchOnDataSet(sqlComm);
            }
            catch (Exception ex)
            {
                Logging.SaveLog(ELogLayer.BusinessRule, ex);
                return null;
            }

            return ds;
        }

        /// <summary>
        /// 民國轉西元(yyyyMMdd)
        /// </summary>
        /// <param name="TwDate"></param>
        /// <returns></returns>
        public static string ChangCalender(string TwDate)
        {
            string NewDate = string.Empty;
            int b;
            if (!int.TryParse(TwDate, out b))
            {
                //日期格式轉換錯誤!
                return NewDate;
            }

            int vYear = b / 10000;
            int vMonth = (b % 10000) / 100;
            int vDay = (b % 100);
            DateTime vDate = DateTime.Parse(string.Format("{0}/{1}/{2} 00:00:00", vYear + 1911, vMonth, vDay));
            NewDate = vDate.ToString("yyyyMMdd");
            return NewDate;
        }


        /// <summary>
        /// 一KEY資料查詢
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strBatch_NO">批號</param>
        /// <param name="strShop_ID">商店代號</param>
        /// <param name="strSign_Type">1:一般簽單；2:分期簽單</param>
        /// <returns>成功時：返回查詢結果；失敗時：null</returns>        
        public static System.Data.DataTable Select_1KEY(string strBatch_Date, string strReceive_Batch, string strBatch_NO, string strShop_ID, string strSign_Type)
        {
            DataSet ds = null;
            SqlHelper sSql = new SqlHelper();

            SqlCommand sqlComm = new SqlCommand();
            try
            {
                sqlComm.CommandText = SEL_ASBD_1KEY;
                sqlComm.CommandType = CommandType.Text;

                SqlParameter parmBatch_Date = new SqlParameter("@Batch_Date", strBatch_Date);
                SqlParameter parmReceive_Batch = new SqlParameter("@Receive_Batch", strReceive_Batch);
                SqlParameter parmBatch_NO = new SqlParameter("@Batch_NO", strBatch_NO);
                SqlParameter parmShop_ID = new SqlParameter("@Shop_ID", strShop_ID);
                SqlParameter parmSign_Type = new SqlParameter("@Sign_Type", strSign_Type);

                sqlComm.Parameters.Add(parmBatch_Date);
                sqlComm.Parameters.Add(parmReceive_Batch);
                sqlComm.Parameters.Add(parmBatch_NO);
                sqlComm.Parameters.Add(parmShop_ID);
                sqlComm.Parameters.Add(parmSign_Type);

                ds = BRArtificial_Signing_Batch_Data.SearchOnDataSet(sqlComm);
                if (null == ds)  //* 查詢失敗
                {
                    return null;
                }
                else
                {
                    //* 查無資料
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        return ds.Tables[0];
                    }
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                Logging.SaveLog(ELogLayer.BusinessRule, ex);
                return null;
            }  
        }

        public static System.Data.DataTable Select_1KEY(string strBatch_Date, string strSign_Type)
        {
            DataSet ds = null;
            SqlHelper sSql = new SqlHelper();

            SqlCommand sqlComm = new SqlCommand();
            try
            {
                sqlComm.CommandText = SEL_ASBD_1KEY_2;
                sqlComm.CommandType = CommandType.Text;

                SqlParameter parmBatch_Date = new SqlParameter("@Batch_Date", strBatch_Date);
                SqlParameter parmSign_Type = new SqlParameter("@Sign_Type", strSign_Type);

                sqlComm.Parameters.Add(parmBatch_Date);
                sqlComm.Parameters.Add(parmSign_Type);

                ds = BRArtificial_Signing_Batch_Data.SearchOnDataSet(sqlComm);
                if (null == ds)  //* 查詢失敗
                {
                    return null;
                }
                else
                {
                    //* 查無資料
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        return ds.Tables[0];
                    }
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                Logging.SaveLog(ELogLayer.BusinessRule, ex);
                return null;
            }
        }

    }
}
