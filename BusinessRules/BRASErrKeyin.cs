//******************************************************************
//*  作    者：
//*  功能說明：人工簽單-錯誤登錄
//*  創建日期：2014/8/21
//*  修改記錄：2021/04/01_Ares_Stanley-移除MicrosoftExcel
//*<author>            <time>            <TaskID>                <desc>
//* 
//*******************************************************************
using System;
using System.Data;
using System.Data.SqlClient;
using CSIPKeyInGUI.EntityLayer;
using Framework.Common.Utility;

namespace CSIPKeyInGUI.BusinessRules
{
    /// <summary>
    /// 人工簽單-檔案匯出
    /// </summary>
    public class BRASErrKeyin : CSIPCommonModel.BusinessRules.BRBase<EntityArtificial_Signing_Error>
    {
        #region SQL 語句
        //* 錯誤登錄 查詢(畫面1)
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        private const string SEARCH_ASErr1 =
        @"SELECT a.Shop_ID, c.Card_No, c.Tran_Date, c.Product_Type, c.Installment_Periods, c.Auth_Code, c.AMT, c.Receipt_Type, b.[1Key_user], c.[2Key_user], a.Receive_Batch, c.SN
            FROM (
	            SELECT Batch_Date, Batch_NO, Shop_ID, Receive_Batch
	            FROM [{0}].[dbo].[Artificial_Signing_Batch_Data]
	            WHERE Batch_Date = @BatchDate
		            AND Batch_NO = @BatchNO
		            AND Shop_ID = @ShopID and Sign_Type = @SignType
		            AND Process_Flag = '02'
	            ) a
            JOIN (
	            SELECT DISTINCT Batch_Date, Batch_NO, Shop_ID, (Create_User) AS [1Key_user]
	            FROM [{0}].[dbo].[Artificial_Signing_Detail]
	            WHERE KeyIn_Flag = '1'
		            AND Case_Status = '0'
	            ) b ON a.Batch_Date = b.Batch_Date
	            AND a.Batch_NO = b.Batch_NO
	            AND a.Shop_ID = b.Shop_ID
            JOIN (
	            SELECT Batch_Date, Batch_NO, Shop_ID, Card_No, Tran_Date, Product_Type, Installment_Periods, Auth_Code, AMT, Receipt_Type, (Create_User) AS [2Key_user], SN
	            FROM [{0}].[dbo].[Artificial_Signing_Detail]
	            WHERE KeyIn_Flag = '2'
		            AND Case_Status = '0'
	            ) c ON a.Batch_Date = c.Batch_Date
	            AND a.Batch_NO = c.Batch_NO
	            AND a.Shop_ID = c.Shop_ID
            ORDER BY SN; ";

        //* 錯誤登錄 查詢(畫面2)
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        private const string SEARCH_ASErr2 =
        @"SELECT Shop_ID, SN, Error_Column, Error_Value, Correct_Value, (Create_User) AS Create_User, Create_DateTime, Receive_Batch
            FROM [{0}].[dbo].[Artificial_Signing_Error]
            WHERE Batch_Date = @BatchDate
	            AND Batch_NO = @BatchNO
	            AND Shop_ID = @ShopID
            ORDER BY SN; ";

        //* 錯誤登錄檔寫入前檢查是否重覆
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        private const string CHECK_INSERT_ASErr =
        @" SELECT [guid]
            FROM [{0}].[dbo].[Artificial_Signing_Error]
            WHERE Batch_Date = @BatchDate
	            AND Batch_NO = @BatchNO
	            AND Shop_ID = @ShopID
	            AND Error_Column = @ErrorColumn
                AND SN = @SN";

        //* 資料寫入人工簽單錯誤登錄檔
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        private const string INSERT_ASErr =
        @" INSERT INTO [{0}].[dbo].[Artificial_Signing_Error] 
            (Batch_Date, Shop_ID, Batch_NO, Receive_Batch, Sign_Type, SN, Error_Column, Correct_Value, Reflect_Source, Error_Value, Create_User, Create_DateTime)
            VALUES 
            (@BatchDate, @ShopID, @BatchNO, @ReceiveBatch, @SignType, @SN, @ErrorColumn, @CorrectValue, @ReflectSource, @ErrorValue, @CreateUser, GETDATE()) ";

        //* 刪除人工簽單錯誤登錄檔資料
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        private const string DELETE_ASErr =
        @" DELETE [{0}].[dbo].[Artificial_Signing_Error]
            WHERE Batch_Date = @BatchDate
	            AND Shop_ID = @ShopID
	            AND Batch_NO = @BatchNO
	            AND Receive_Batch = @ReceiveBatch
	            AND Sign_Type = @SignType
	            AND SN = @SN
	            AND Error_Column = @ErrorColumn ";

        #endregion

        /// <summary>
        /// 錯誤登錄 查詢
        /// </summary>
        /// <param name="strBatchDate">編批日期</param>
        /// <param name="strBatchNO">批號</param>
        /// <param name="strShopID">商店代號</param>
        /// <param name="strSignType">簽單類別</param>
        /// <param name="strArea">查詢範圍(ASErr1、ASErr2、ALL)</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>成功時：返回查詢結果；失敗時：null</returns>
        public static DataSet SearchASErrData(string strBatchDate, string strBatchNO, string strShopID, string strSignType, string strArea, ref string strMsgID)
        {
            try
            {
                #region 依據Request查詢資料庫

                //* 聲明SQL Command變量
                SqlCommand sqlcmSearchData = new SqlCommand();
                sqlcmSearchData.CommandType = CommandType.Text;
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                if (strArea == "ASErr1")
                    sqlcmSearchData.CommandText = string.Format(SEARCH_ASErr1, UtilHelper.GetAppSettings("DB_KeyinGUI"));
                else if (strArea == "ASErr2")
                    sqlcmSearchData.CommandText = string.Format(SEARCH_ASErr2, UtilHelper.GetAppSettings("DB_KeyinGUI"));
                else
                    sqlcmSearchData.CommandText = string.Format(SEARCH_ASErr1, UtilHelper.GetAppSettings("DB_KeyinGUI")) + string.Format(SEARCH_ASErr2, UtilHelper.GetAppSettings("DB_KeyinGUI"));
                //* 編批日期
                SqlParameter parmBatchDate = new SqlParameter("@BatchDate", strBatchDate.Replace("/", ""));
                sqlcmSearchData.Parameters.Add(parmBatchDate);
                //* 批號
                SqlParameter parmBatchNO = new SqlParameter("@BatchNO", strBatchNO);
                sqlcmSearchData.Parameters.Add(parmBatchNO);
                //* 商店代號
                SqlParameter parmShopID = new SqlParameter("@ShopID", strShopID);
                sqlcmSearchData.Parameters.Add(parmShopID);
                //* 簽單類別(1:一般；2:分期)
                SqlParameter parmSignType = new SqlParameter("@SignType", strSignType);
                sqlcmSearchData.Parameters.Add(parmSignType);

                //* 查詢數據
                DataSet dstSearchData = BRASErrKeyin.SearchOnDataSet(sqlcmSearchData);
                if (null == dstSearchData)  //* 查詢數據失敗
                {
                    strMsgID = "01_01060600_001";
                    return null;
                }
                else
                {
                    //* 查詢的數據不存在時
                    if (dstSearchData.Tables[0].Rows.Count == 0)
                    {
                        strMsgID = "01_01060600_002";
                        return null;
                    }
                }
                if (strArea == "ASErr1")
                    dstSearchData.Tables[0].TableName = "ASErr1";
                else if (strArea == "ASErr2")
                    dstSearchData.Tables[0].TableName = "ASErr2";
                else
                {
                    dstSearchData.Tables[0].TableName = "ASErr1";
                    dstSearchData.Tables[1].TableName = "ASErr2";
                }
                
                //* 查詢成功
                strMsgID = "01_01060600_003";
                return dstSearchData;

                #endregion 依據Request查詢資料庫
            }
            catch (Exception exp)
            {
                BRASErrKeyin.SaveLog(exp);
                strMsgID = "01_01060600_001";
                return null;
            }
        }

        /// <summary>
        /// 錯誤登錄檔寫入前檢查是否重覆
        /// </summary>
        /// <param name="strBatchDate">編批日期</param>
        /// <param name="strBatchNO">批號</param>
        /// <param name="strShopID">商店代號</param>
        /// <param name="strErrorColumn">錯誤欄位名稱</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>true:資料無重覆；false:資料重覆或查詢失敗</returns>
        public static bool Check_Insert_ASErr(string strBatchDate, string strBatchNO, string strShopID, string strErrorColumn, string strSN, ref string strMsgID)
        {
            try
            {
                #region 依據Request查詢資料庫

                //* 聲明SQL Command變量
                SqlCommand sqlcmSearchData = new SqlCommand();
                sqlcmSearchData.CommandType = CommandType.Text;
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                sqlcmSearchData.CommandText = string.Format(CHECK_INSERT_ASErr, UtilHelper.GetAppSettings("DB_KeyinGUI"));
                //* 編批日期
                SqlParameter parmBatchDate = new SqlParameter("@BatchDate", strBatchDate.Replace("/", ""));
                sqlcmSearchData.Parameters.Add(parmBatchDate);
                //* 批號
                SqlParameter parmBatchNO = new SqlParameter("@BatchNO", strBatchNO);
                sqlcmSearchData.Parameters.Add(parmBatchNO);
                //* 商店代號
                SqlParameter parmShopID = new SqlParameter("@ShopID", strShopID);
                sqlcmSearchData.Parameters.Add(parmShopID);
                //* 錯誤欄位
                SqlParameter parmErrorColumn = new SqlParameter("@ErrorColumn", strErrorColumn);
                sqlcmSearchData.Parameters.Add(parmErrorColumn);
                //* 明細序號
                SqlParameter parmSN = new SqlParameter("@SN", strSN);
                sqlcmSearchData.Parameters.Add(parmSN);

                //* 查詢數據
                DataSet dstSearchData = BRASErrKeyin.SearchOnDataSet(sqlcmSearchData);
                if (null == dstSearchData)  //* 查詢數據失敗
                {
                    strMsgID = "01_01060600_001";
                    return false;
                }
                else
                {
                    //* 無重覆資料回傳true
                    if (dstSearchData.Tables[0].Rows.Count == 0)
                    {
                        return true;
                    }
                }
                //* 有重覆資料回傳false及資料重覆訊息
                strMsgID = "01_01060600_007";
                return false;

                #endregion 依據Request查詢資料庫
            }
            catch (Exception exp)
            {
                BRASErrKeyin.SaveLog(exp);
                strMsgID = "01_01060600_001";
                return false;
            }
        }

        /// <summary>
        /// 寫入錯誤登錄檔
        /// </summary>
        /// <param name="strBatchDate">編批日期</param>
        /// <param name="strBatchNO">批號</param>
        /// <param name="strShopID">商店代號</param>
        /// <param name="strReceiveBatch">收件批次</param>
        /// <param name="strSignType">簽單類別</param>
        /// <param name="strSN">明細序號</param>
        /// <param name="strErrorColumn">錯誤欄位名稱</param>
        /// <param name="strCorrectValue">正確值</param>
        /// <param name="strReflectSource">反應來源</param>
        /// <param name="strErrorValue">錯誤值</param>
        /// <param name="strCreateUser">計錯人員</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>true:寫入成功；false:寫入失敗</returns>
        public static bool Insert_ASErr(string strBatchDate, string strBatchNO, string strShopID, string strReceiveBatch, string strSignType,
            string strSN, string strErrorColumn, string strCorrectValue, string strReflectSource, string strErrorValue, string strCreateUser, ref string strMsgID)
        {
            try
            {
                //* 聲明SQL Command變量
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                sqlcmd.CommandText = string.Format(INSERT_ASErr, UtilHelper.GetAppSettings("DB_KeyinGUI"));
                //* 編批日期
                SqlParameter parmBatchDate = new SqlParameter("@BatchDate", strBatchDate.Replace("/", ""));
                sqlcmd.Parameters.Add(parmBatchDate);
                //* 批號
                SqlParameter parmBatchNO = new SqlParameter("@BatchNO", strBatchNO);
                sqlcmd.Parameters.Add(parmBatchNO);
                //* 商店代號
                SqlParameter parmShopID = new SqlParameter("@ShopID", strShopID);
                sqlcmd.Parameters.Add(parmShopID);
                //* 收件批次
                SqlParameter parmReceiveBatch = new SqlParameter("@ReceiveBatch", strReceiveBatch);
                sqlcmd.Parameters.Add(parmReceiveBatch);
                //* 簽單類別
                SqlParameter parmSignType = new SqlParameter("@SignType", strSignType);
                sqlcmd.Parameters.Add(parmSignType);
                //* 明細序號
                SqlParameter parmSN = new SqlParameter("@SN", strSN);
                sqlcmd.Parameters.Add(parmSN);
                //* 錯誤欄位
                SqlParameter parmErrorColumn = new SqlParameter("@ErrorColumn", strErrorColumn);
                sqlcmd.Parameters.Add(parmErrorColumn);
                //* 正確值
                SqlParameter parmCorrectValue = new SqlParameter("@CorrectValue", strCorrectValue);
                sqlcmd.Parameters.Add(parmCorrectValue);
                //* 反應來源
                SqlParameter parmReflectSource = new SqlParameter("@ReflectSource", strReflectSource);
                sqlcmd.Parameters.Add(parmReflectSource);
                //* 錯誤值
                SqlParameter parmErrorValue = new SqlParameter("@ErrorValue", strErrorValue);
                sqlcmd.Parameters.Add(parmErrorValue);
                //* 計錯人員
                SqlParameter parmCreateUser = new SqlParameter("@CreateUser", strCreateUser);
                sqlcmd.Parameters.Add(parmCreateUser);

                //寫入資料
                if (BRASErrKeyin.Add(sqlcmd))
                {
                    strMsgID = "01_01060600_005";
                    return true;
                }
                else
                {
                    strMsgID = "01_01060600_006";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRASErrKeyin.SaveLog(exp);
                strMsgID = "01_01060600_005";
                return false;
            }
        }

        /// <summary>
        /// 刪除錯誤登錄檔資料
        /// </summary>
        /// <param name="strBatchDate">編批日期</param>
        /// <param name="strBatchNO">批號</param>
        /// <param name="strShopID">商店代號</param>
        /// <param name="strReceiveBatch">收件批次</param>
        /// <param name="strSignType">簽單類別</param>
        /// <param name="SN">明細序號</param>
        /// <param name="strErrorColumn">錯誤欄位名稱</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>true:刪除成功；false:刪除失敗</returns>
        public static bool Delete_ASErr(string strBatchDate, string strBatchNO, string strShopID, string strReceiveBatch, string strSignType,
            string strSN, string strErrorColumn, ref string strMsgID)
        {
            try
            {
                //* 聲明SQL Command變量
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                sqlcmd.CommandText = string.Format(DELETE_ASErr, UtilHelper.GetAppSettings("DB_KeyinGUI"));
                //* 編批日期
                SqlParameter parmBatchDate = new SqlParameter("@BatchDate", strBatchDate.Replace("/", ""));
                sqlcmd.Parameters.Add(parmBatchDate);
                //* 批號
                SqlParameter parmBatchNO = new SqlParameter("@BatchNO", strBatchNO);
                sqlcmd.Parameters.Add(parmBatchNO);
                //* 商店代號
                SqlParameter parmShopID = new SqlParameter("@ShopID", strShopID);
                sqlcmd.Parameters.Add(parmShopID);
                //* 收件批次
                SqlParameter parmReceiveBatch = new SqlParameter("@ReceiveBatch", strReceiveBatch);
                sqlcmd.Parameters.Add(parmReceiveBatch);
                //* 簽單類別
                SqlParameter parmSignType = new SqlParameter("@SignType", strSignType);
                sqlcmd.Parameters.Add(parmSignType);
                //* 明細序號
                SqlParameter parmSN = new SqlParameter("@SN", strSN);
                sqlcmd.Parameters.Add(parmSN);
                //* 錯誤欄位
                SqlParameter parmErrorColumn = new SqlParameter("@ErrorColumn", strErrorColumn);
                sqlcmd.Parameters.Add(parmErrorColumn);

                //刪除資料
                if (BRASErrKeyin.Delete(sqlcmd))
                {
                    strMsgID = "01_01060600_009";
                    return true;
                }
                else
                {
                    strMsgID = "01_01060600_010";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRASErrKeyin.SaveLog(exp);
                strMsgID = "01_01060600_010";
                return false;
            }
        }
    }
}
