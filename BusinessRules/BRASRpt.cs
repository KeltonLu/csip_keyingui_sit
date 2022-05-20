//******************************************************************
//*  作    者：
//*  功能說明：人工簽單-報表
//*  創建日期：2014/8/7
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*Ares_Luke           2021/01/20        20200031-CSIP EOS       報表與查詢功能共用SQL
//*******************************************************************
using System;
using System.Data;
using System.Data.SqlClient;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Logging;
using Framework.Common.Utility;

namespace CSIPKeyInGUI.BusinessRules
{
    /// <summary>
    /// 作業量比對報表業務類
    /// </summary>
    public class BRASRpt : CSIPCommonModel.BusinessRules.BRBase<EntityM_PROPERTY_CODE>
    {
		#region SQL 語句
		//* 人工簽單作業量報表
		//2021/03/09_Ares_Stanley-DB名稱改為變數
		private const string SEARCH_rptAS_Work =
        @"SELECT CASE WHEN t.KeyinDate IS NULL THEN d.KeyinDate ELSE t.KeyinDate END KeyinDate, 
                CASE WHEN t.Adjust_Count_general IS NULL THEN 0 ELSE t.Adjust_Count_general END Adjust_Count_general, 
                CASE WHEN t.Adjust_Count_inst IS NULL THEN 0 ELSE t.Adjust_Count_inst END Adjust_Count_inst, 
                CASE WHEN d.Err_Count IS NULL THEN 0 ELSE d.Err_Count END Err_Count
            FROM (
	            SELECT CONVERT(CHAR(10), a.Create_DateTime, 111) AS KeyinDate, 
                        SUM(CASE WHEN a.Sign_Type = 1 THEN 1 ELSE 0 END) AS Adjust_Count_general, 
                        SUM(CASE WHEN a.Sign_Type = 2 THEN 1 ELSE 0 END) AS Adjust_Count_inst
	            FROM (
		            SELECT *
		            FROM [{0}].[dbo].[Artificial_Signing_Primary]
		            WHERE KeyIn_Flag = '2'
			            AND [Create_DateTime] BETWEEN @SearchStart AND @SearchEnd
		            ) a
	            LEFT JOIN [{0}].[dbo].[Artificial_Signing_Batch_Data] b ON a.Batch_Date = b.Batch_Date
		            AND a.Receive_Batch = b.Receive_Batch
		            AND a.Batch_NO = b.Batch_NO
		            AND a.Shop_ID = b.Shop_ID
		            AND a.Sign_Type = b.Sign_Type
	            JOIN (
		            SELECT *
		            FROM [{0}].[dbo].[Artificial_Signing_Detail]
		            WHERE KeyIn_Flag = '2' AND Case_Status = '0'
		            ) c ON a.Batch_Date = c.Batch_Date
		            AND a.Receive_Batch = c.Receive_Batch
		            AND a.Batch_NO = c.Batch_NO
		            AND a.Shop_ID = c.Shop_ID
		            AND a.Sign_Type = c.Sign_Type
	            WHERE b.Process_Flag = '02'
	            GROUP BY CONVERT(CHAR(10), a.Create_DateTime, 111)
	            ) t
            FULL OUTER JOIN (
	            SELECT CONVERT(CHAR(10), Create_DateTime, 111) AS KeyinDate, COUNT(*) AS Err_Count
	            FROM [{0}].[dbo].[Artificial_Signing_Error]
	            WHERE [Create_DateTime] BETWEEN @SearchStart AND @SearchEnd
	            GROUP BY CONVERT(CHAR(10), Create_DateTime, 111)
	            ) d ON t.KeyinDate = d.KeyinDate
            ORDER BY KeyinDate ";

		//* 人工簽單鍵檔錯誤明細
		//20141017  改以錯誤鍵檔日期為搜尋條件
		//2021/03/09_Ares_Stanley-DB名稱改為變數
		private const string SEARCH_rptAS_ErrDetail =
        @"SELECT a.KeyinDate, a.Batch_Date, c.Sign_Type, a.Batch_NO, a.Shop_ID, a.Card_No, a.Tran_Date, Auth_Code, AMT, 
		        a.Receipt_Type, a.Product_Type, a.Installment_Periods, a.[1Key_user], b.[2Key_user], 
		        c.Error_Column, c.Correct_Value, c.Reflect_Source
        FROM (
	        SELECT CONVERT(CHAR(10), Create_DateTime, 111) AS KeyinDate, Batch_Date, Batch_NO, Shop_ID, Card_No, Tran_Date, Auth_Code, AMT, 
			        Receipt_Type, Product_Type, Installment_Periods, (Create_User) AS [1Key_user], Receive_Batch, SN
	        FROM [{0}].[dbo].[Artificial_Signing_Detail]
	        WHERE KeyIn_Flag = '1'
		        AND Case_Status = '0'
	        ) a
        LEFT JOIN (
	        SELECT (Create_User) AS [2Key_user], Batch_Date, Batch_NO, Shop_ID, Receive_Batch, SN
	        FROM [{0}].[dbo].[Artificial_Signing_Detail]
	        WHERE KeyIn_Flag = '2'
		        AND Case_Status = '0'
	        ) b ON a.Batch_Date = b.Batch_Date
	        AND a.Batch_NO = b.Batch_NO
	        AND a.Shop_ID = b.Shop_ID
	        AND a.Receive_Batch = b.Receive_Batch
	        AND a.SN = b.SN
        INNER JOIN (
	        SELECT Sign_Type, Error_Column, Correct_Value, (e.PROPERTY_NAME) AS Reflect_Source, Batch_Date, Batch_NO, Shop_ID, Receive_Batch, SN
	        FROM (SELECT * FROM [{0}].[dbo].[Artificial_Signing_Error] WHERE Create_DateTime BETWEEN @SearchStart AND @SearchEnd) d
	        LEFT JOIN (
		        SELECT *
		        FROM [{1}].[dbo].[M_PROPERTY_CODE]
		        WHERE FUNCTION_KEY = '01'
			        AND PROPERTY_KEY = 'AS_REFLECT_SOURCE'
		        ) e ON d.Reflect_Source = e.PROPERTY_CODE
	        ) c ON a.Batch_Date = c.Batch_Date
	        AND a.Batch_NO = c.Batch_NO
	        AND a.Shop_ID = c.Shop_ID
	        AND a.Receive_Batch = c.Receive_Batch
	        AND a.SN = c.SN ";

		//* 人員鍵檔產能報表
		//20141001  改以明細資料做統計，只要有存檔進DB的就計算
		//20141017  完成平帳提交的資料才計算，以Create_User做統計
		//2021/03/09_Ares_Stanley-DB名稱改為變數
		private const string SEARCH_rptAS_Capacity =
        @"SELECT (u.Create_User) AS Create_User, 
	        CASE WHEN g1.general_1key IS NULL THEN 0 ELSE g1.general_1key END general_1key, 
	        CASE WHEN g2.general_2key IS NULL THEN 0 ELSE g2.general_2key END general_2key, 
	        CASE WHEN i1.inst_1key IS NULL THEN 0 ELSE i1.inst_1key END inst_1key, 
	        CASE WHEN i2.inst_2key IS NULL THEN 0 ELSE i2.inst_2key END inst_2key
        FROM (
	        SELECT DISTINCT Create_User
	        FROM [{0}].[dbo].[Artificial_Signing_Detail]
	        WHERE Create_DateTime BETWEEN @SearchStart AND @SearchEnd
	        ) u
        LEFT JOIN (
	        SELECT Create_User, count(*) AS general_1key
	        FROM (
		        SELECT a.Create_User
		        FROM (
			        SELECT *
			        FROM [{0}].[dbo].[Artificial_Signing_Detail]
			        WHERE Create_DateTime BETWEEN @SearchStart AND @SearchEnd
				        AND Sign_Type = '1'
				        AND KeyIn_Flag = '1'
			        ) a
		        JOIN (
			        SELECT *
			        FROM [{0}].[dbo].[Artificial_Signing_Primary]
			        WHERE Balance_Flag = 'Y'
			        ) b ON a.Batch_Date = b.Batch_Date
			        AND a.Shop_ID = b.Shop_ID
			        AND a.Batch_NO = b.Batch_NO
			        AND a.Receive_Batch = b.Receive_Batch
			        AND a.Sign_Type = b.Sign_Type
			        AND a.KeyIn_Flag = b.KeyIn_Flag
		        ) t
	        GROUP BY Create_User
	        ) g1 ON u.Create_User = g1.Create_User
        LEFT JOIN (
	        SELECT Create_User, count(*) AS general_2key
	        FROM (
		        SELECT a.Create_User
		        FROM (
			        SELECT *
			        FROM [{0}].[dbo].[Artificial_Signing_Detail]
			        WHERE Create_DateTime BETWEEN @SearchStart AND @SearchEnd
				        AND Sign_Type = '1'
				        AND KeyIn_Flag = '2'
			        ) a
		        JOIN (
			        SELECT *
			        FROM [{0}].[dbo].[Artificial_Signing_Primary]
			        WHERE Balance_Flag = 'Y'
			        ) b ON a.Batch_Date = b.Batch_Date
			        AND a.Shop_ID = b.Shop_ID
			        AND a.Batch_NO = b.Batch_NO
			        AND a.Receive_Batch = b.Receive_Batch
			        AND a.Sign_Type = b.Sign_Type
			        AND a.KeyIn_Flag = b.KeyIn_Flag
		        ) t
	        GROUP BY Create_User
	        ) g2 ON u.Create_User = g2.Create_User
        LEFT JOIN (
	        SELECT Create_User, count(*) AS inst_1key
	        FROM (
		        SELECT a.Create_User
		        FROM (
			        SELECT *
			        FROM [{0}].[dbo].[Artificial_Signing_Detail]
			        WHERE Create_DateTime BETWEEN @SearchStart AND @SearchEnd
				        AND Sign_Type = '2'
				        AND KeyIn_Flag = '1'
			        ) a
		        JOIN (
			        SELECT *
			        FROM [{0}].[dbo].[Artificial_Signing_Primary]
			        WHERE Balance_Flag = 'Y'
			        ) b ON a.Batch_Date = b.Batch_Date
			        AND a.Shop_ID = b.Shop_ID
			        AND a.Batch_NO = b.Batch_NO
			        AND a.Receive_Batch = b.Receive_Batch
			        AND a.Sign_Type = b.Sign_Type
			        AND a.KeyIn_Flag = b.KeyIn_Flag
		        ) t
	        GROUP BY Create_User
	        ) i1 ON u.Create_User = i1.Create_User
        LEFT JOIN (
	        SELECT Create_User, count(*) AS inst_2key
	        FROM (
		        SELECT a.Create_User
		        FROM (
			        SELECT *
			        FROM [{0}].[dbo].[Artificial_Signing_Detail]
			        WHERE Create_DateTime BETWEEN @SearchStart AND @SearchEnd
				        AND Sign_Type = '2'
				        AND KeyIn_Flag = '2'
			        ) a
		        JOIN (
			        SELECT *
			        FROM [{0}].[dbo].[Artificial_Signing_Primary]
			        WHERE Balance_Flag = 'Y'
			        ) b ON a.Batch_Date = b.Batch_Date
			        AND a.Shop_ID = b.Shop_ID
			        AND a.Batch_NO = b.Batch_NO
			        AND a.Receive_Batch = b.Receive_Batch
			        AND a.Sign_Type = b.Sign_Type
			        AND a.KeyIn_Flag = b.KeyIn_Flag
		        ) t
	        GROUP BY Create_User
	        ) i2 ON u.Create_User = i2.Create_User ";

		//* 人工簽單每日剔退明細
		//20141001  以2KEY資料為準，取1.2KEY比對完成的剔退資料
		//20141017  以帳務日及批號排序，退款剔退金額帶負號
		//2021/03/09_Ares_Stanley-DB名稱改為變數
		private const string SEARCH_rptAS_RejectDetail =
        @"SELECT a.Batch_Date, (Convert(VARCHAR, a.Batch_NO) + ' / ' + a.Shop_ID) AS Compare, a.Card_No, 
            CASE WHEN a.Receipt_Type = '41' THEN 0-a.AMT ELSE a.AMT END AS AMT, a.Reject_Reason
            FROM (
	            SELECT *
	            FROM [{0}].[dbo].[Artificial_Signing_Detail]
	            WHERE Create_DateTime BETWEEN @SearchStart AND @SearchEnd
		            AND Case_Status = '1'
		            AND KeyIn_Flag = '2'
	            ) a
            LEFT JOIN [{0}].[dbo].[Artificial_Signing_Primary] b 
                ON a.Batch_Date = b.Batch_Date
	            AND a.Shop_ID = b.Shop_ID
	            AND a.Batch_NO = b.Batch_NO
	            AND a.Receive_Batch = b.Receive_Batch
	            AND a.Sign_Type = b.Sign_Type
	            AND a.KeyIn_Flag = b.KeyIn_Flag
            WHERE b.KeyIn_MatchFlag = 'Y' 
            ORDER BY Batch_Date, Compare ";

        #endregion

        /// <summary>
        /// 取報表數據
        /// </summary>
        /// <param name="strProperty">報表種類</param>
        /// <param name="strSearchStart">區間起</param>
        /// <param name="strSearchEnd">區間迄</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>成功時：返回查詢結果；失敗時：null</returns>
        public static DataTable SearchRPTData(string strProperty, string strSearchStart, string strSearchEnd, ref string strMsgID)
        {
            try
            {
                #region 依據Request查詢資料庫
                Int32 count = 0;
                
                DataSet dstSearchData = SearchData( strProperty, strSearchStart, strSearchEnd, ref count);
                if (null == dstSearchData)  //* 查詢數據失敗
                {
                    strMsgID = "01_03100000_004";
                    return null;
                }

				// 2021/03/08_Ares_Stanley-查詢數據為空時，下載空報表
                //* 查詢的數據不存在時
                if (dstSearchData.Tables[0].Rows.Count == 0)
                {
					//2021/04/01_Ares_Stanley-修正XML id重複
	                strMsgID = "01_03100000_014";
					return dstSearchData.Tables[0];
				}

                return dstSearchData.Tables[0];

                #endregion 依據Request查詢資料庫
            }
            catch (Exception exp)
            {
                BRCompareRpt.SaveLog(exp);
                strMsgID = "01_03100000_004";
                return null;
            }
        }


        public static Boolean SearchGripData(string strProperty, string strSearchStart, string strSearchEnd, Int32 idx, Int32 size, ref Int32 count, ref DataTable dt)
        {
            try
            {
                //* 查詢數據
                DataSet ds = SearchData(strProperty, strSearchStart, strSearchEnd, ref count, idx, size);
                if (null != ds)
                {
                    dt = ds.Tables[0];
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }



		private static DataSet SearchData(string strProperty, string strSearchStart, string strSearchEnd, 
	        ref Int32 count, Int32 idx = -1, Int32 size = -1)
        {
	        try
	        {
		        #region 依據Request查詢資料庫
		        
				string strSqlSearch = "";
                switch (strProperty)    //* 業務項目
                {
                    case "rptAS_Work":
						//* 人工簽單作業量報表
						//2021/03/09_Ares_Stanley-DB名稱改為變數
						strSqlSearch = string.Format(SEARCH_rptAS_Work, UtilHelper.GetAppSettings("DB_KeyinGUI"));
						break;

                    case "rptAS_ErrDetail":
						//* 人工簽單鍵檔錯誤明細
						//2021/03/09_Ares_Stanley-DB名稱改為變數
						strSqlSearch = string.Format(SEARCH_rptAS_ErrDetail, UtilHelper.GetAppSettings("DB_KeyinGUI"), UtilHelper.GetAppSettings("DB_CSIP"));
						break;

                    case "rptAS_Capacity":
						//* 人員鍵檔產能報表
						//2021/03/09_Ares_Stanley-DB名稱改為變數
						strSqlSearch = string.Format(SEARCH_rptAS_Capacity, UtilHelper.GetAppSettings("DB_KeyinGUI"));
						break;

                    case "rptAS_RejectDetail":
						//* 人工簽單每日剔退明細
						//2021/03/09_Ares_Stanley-DB名稱改為變數
						strSqlSearch = string.Format(SEARCH_rptAS_RejectDetail, UtilHelper.GetAppSettings("DB_KeyinGUI"));
						break;

                    default:
                        break;
                }

                //* 聲明SQL Command變量
                SqlCommand sqlcmSearchData = new SqlCommand();
                sqlcmSearchData.CommandType = CommandType.Text;
                sqlcmSearchData.CommandText = strSqlSearch;
                //* 區間起
                SqlParameter parmSearchStart = new SqlParameter("@SearchStart", strSearchStart.Replace("/", ""));
                sqlcmSearchData.Parameters.Add(parmSearchStart);
                //* 區間迄
                SqlParameter parmSearchEnd = new SqlParameter("@SearchEnd", strSearchEnd.Replace("/", ""));
                sqlcmSearchData.Parameters.Add(parmSearchEnd);

                if (idx >= 0)
	                return SearchOnDataSet(sqlcmSearchData, idx, size, ref count);
                else
	                return SearchOnDataSet(sqlcmSearchData);
                
                #endregion 依據Request查詢資料庫
	        }
	        catch (Exception ex)
	        {
		        Logging.Log(ex);
		        throw;
	        }
        }

    }
}
