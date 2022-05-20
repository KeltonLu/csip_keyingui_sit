//******************************************************************
//*  作    者：
//*  功能說明：人工簽單-檔案匯出
//*  創建日期：2014/8/11
//*  修改紀錄：2021/01/07_Ares_Stanley-變更報表產出為NPOI; 2021/04/01_Ares_Stanley-移除MicrosoftExcel
//*　<author>            <time>            <TaskID>                <desc>
//*　Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*******************************************************************
using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Globalization;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Logging;
using Framework.Common.IO;
using Framework.Common.Utility;
using DataTable = System.Data.DataTable;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace CSIPKeyInGUI.BusinessRules
{
    /// <summary>
    /// 人工簽單-檔案匯出
    /// </summary>
    public class BRASExport : CSIPCommonModel.BusinessRules.BRBase<EntityM_PROPERTY_CODE>
    {
        #region SQL 語句
        //20140924 新增總計資料，分別加在一般簽單及分期簽單最後一筆資料之後
        //* 請款簽單明細表 查詢
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        private const string SEARCH_AS =
        @"SELECT CASE WHEN a.Sign_Type = '1' THEN '一般簽單' ELSE '分期簽單' END AS Sign_Type, 
	            CASE WHEN a.Sign_Type = '1' THEN REPLICATE('0', 3 - LEN(a.Batch_NO)) + CONVERT(varchar,a.Batch_NO) 
		            ELSE REPLICATE('0', 4 - LEN(a.Batch_NO)) + CONVERT(varchar,a.Batch_NO) END AS Batch_NO, 
	            a.Shop_ID, a.Receive_Total_Count, a.Receive_Total_AMT, 
	            b.Keyin_Success_Count_40, b.Keyin_Success_AMT_40, b.Keyin_Success_Count_41, b.Keyin_Success_AMT_41, 
	            b.Keyin_Reject_Count_40, b.Keyin_Reject_AMT_40, b.Keyin_Reject_Count_41, b.Keyin_Reject_AMT_41, 
	            b.Adjust_Count, b.Adjust_AMT, 
                CASE WHEN a.Process_Flag='01' THEN a.Process_Flag + ' 已匯檔' ELSE a.Process_Flag + ' 已產檔' END AS Process_Flag, 
	            CASE WHEN b.Balance_Flag = 'Y' AND c.Balance_Flag = 'Y' THEN 'Y' ELSE 'N' END AS Balance_Flag, 
	            b.KeyIn_MatchFlag, b.Modify_DateTime
            FROM (
	            SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, Receive_Total_Count, Receive_Total_AMT, Process_Flag
	            FROM [{0}].[dbo].[Artificial_Signing_Batch_Data]
	            WHERE Batch_Date = @Batch_Date and Receive_Batch = @Receive_Batch
	            ) a
            LEFT JOIN (
	            SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, KeyIn_Flag, 
	            Keyin_Success_Count_40, Keyin_Success_AMT_40, Keyin_Success_Count_41, Keyin_Success_AMT_41, 
	            Keyin_Reject_Count_40, Keyin_Reject_AMT_40, Keyin_Reject_Count_41, Keyin_Reject_AMT_41, 
	            Adjust_Count, Adjust_AMT, Balance_Flag, KeyIn_MatchFlag, Modify_DateTime, Sign_Type
	            FROM [{0}].[dbo].[Artificial_Signing_Primary]
	            WHERE KeyIn_Flag = '2'
	            ) b ON a.Batch_Date = b.Batch_Date
	            AND a.Batch_NO = b.Batch_NO
	            AND a.Receive_Batch = b.Receive_Batch
	            AND a.Shop_ID = b.Shop_ID
                AND a.Sign_Type = b.Sign_Type
            LEFT JOIN (
	            SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, KeyIn_Flag, Balance_Flag, Sign_Type
	            FROM [{0}].[dbo].[Artificial_Signing_Primary]
	            WHERE KeyIn_Flag = '1'
	            ) c ON a.Batch_Date = c.Batch_Date
	            AND a.Batch_NO = c.Batch_NO
	            AND a.Receive_Batch = c.Receive_Batch
	            AND a.Shop_ID = c.Shop_ID
                AND a.Sign_Type = c.Sign_Type
        UNION ALL
            SELECT CASE WHEN a.Sign_Type = '1' THEN '一般簽單' ELSE '分期簽單' END AS Sign_Type,
            '總計' AS Batch_NO,'' AS Shop_ID,
            SUM(a.Receive_Total_Count) AS Receive_Total_Count, SUM(a.Receive_Total_AMT) AS Receive_Total_AMT, 
            SUM(b.Keyin_Success_Count_40) AS Keyin_Success_Count_40, SUM(b.Keyin_Success_AMT_40) AS Keyin_Success_AMT_40, 
            SUM(b.Keyin_Success_Count_41) AS Keyin_Success_Count_41, SUM(b.Keyin_Success_AMT_41) AS Keyin_Success_AMT_41, 
            SUM(b.Keyin_Reject_Count_40) AS Keyin_Reject_Count_40, SUM(b.Keyin_Reject_AMT_40) AS Keyin_Reject_AMT_40, 
            SUM(b.Keyin_Reject_Count_41) AS Keyin_Reject_Count_41, SUM(b.Keyin_Reject_AMT_41) AS Keyin_Reject_AMT_41, 
            SUM(b.Adjust_Count) AS Adjust_Count, SUM(b.Adjust_AMT) AS Adjust_AMT, 
            '' AS Process_Flag, '' AS Balance_Flag, '' AS KeyIn_MatchFlag, NULL AS Modify_DateTime
            FROM (
	            SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, Receive_Total_Count, Receive_Total_AMT, Process_Flag
	            FROM [{0}].[dbo].[Artificial_Signing_Batch_Data]
	            WHERE Batch_Date = @Batch_Date
		            AND Receive_Batch = @Receive_Batch
	            ) a
            LEFT JOIN (
	            SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, KeyIn_Flag, 
                Keyin_Success_Count_40, Keyin_Success_AMT_40, Keyin_Success_Count_41, Keyin_Success_AMT_41, 
                Keyin_Reject_Count_40, Keyin_Reject_AMT_40, Keyin_Reject_Count_41, Keyin_Reject_AMT_41, 
                Adjust_Count, Adjust_AMT, Balance_Flag, KeyIn_MatchFlag, Modify_DateTime, Sign_Type
	            FROM [{0}].[dbo].[Artificial_Signing_Primary]
	            WHERE KeyIn_Flag = '2'
	            ) b ON a.Batch_Date = b.Batch_Date
	            AND a.Batch_NO = b.Batch_NO
	            AND a.Receive_Batch = b.Receive_Batch
	            AND a.Shop_ID = b.Shop_ID
	            AND a.Sign_Type = b.Sign_Type
            GROUP BY a.Batch_Date, a.Receive_Batch, a.Sign_Type
        ORDER BY Sign_Type,Batch_NO ";

        //* 請款簽單明細表 查詢明細
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        private const string SEARCH_ASDetail =
        @"SELECT SN, Card_No, Tran_Date, Product_Type, Installment_Periods, AMT, Auth_Code, Receipt_Type, Case_Status, KeyIn_Flag, 
	            CASE WHEN Modify_User IS NULL THEN Create_User ELSE Modify_User END AS Keyin_User, 
	            CASE WHEN Modify_DateTime IS NULL THEN Create_DateTime ELSE Modify_DateTime END AS Modify_DateTime
            FROM [{0}].[dbo].[Artificial_Signing_Detail]
            WHERE Batch_Date = @Batch_Date AND Receive_Batch = @Receive_Batch AND Batch_NO = @Batch_NO AND Shop_ID = @Shop_ID AND Sign_Type = @Sign_Type
	            ";

        //* 產生請款簽單明細表
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        private const string SEARCH_ExportASDetail =
        @"SELECT a.Sign_Type, a.Batch_NO, a.Shop_ID, a.Receive_Total_Count, a.Receive_Total_AMT, b.Adjust_Count, b.Adjust_AMT, 
	            b.Keyin_Reject_Count_All, b.Keyin_Reject_AMT_All, a.Reject_Reason, a.Page
            FROM (
	            SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, Receive_Total_Count, Receive_Total_AMT, Page, 
		            (	SELECT ',' + Reject_Reason
			            FROM [{0}].[dbo].[Artificial_Signing_Detail] x
			            WHERE x.Batch_Date = y.Batch_Date AND x.Receive_Batch = y.Receive_Batch AND x.Batch_NO = y.Batch_NO
				            AND x.Shop_ID = y.Shop_ID AND Case_Status = '1' AND KeyIn_Flag = '2'
			            FOR XML path('')
			            ) AS Reject_Reason
	            FROM [{0}].[dbo].[Artificial_Signing_Batch_Data] y
	            WHERE Batch_Date = @Batch_Date AND Receive_Batch = @Receive_Batch
	            ) a
            LEFT JOIN (
	            SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, 
		            CASE WHEN First_Balance_Count = 0 THEN NULL ELSE Adjust_Count END as Adjust_Count, 
		            CASE WHEN First_Balance_AMT = 0 THEN NULL ELSE Adjust_AMT END as Adjust_AMT,
		            Keyin_Reject_Count_All, Keyin_Reject_AMT_All, Sign_Type
	            FROM [{0}].[dbo].[Artificial_Signing_Primary]
	            WHERE KeyIn_Flag = '2'
	            ) b ON a.Batch_Date = b.Batch_Date
	            AND a.Receive_Batch = b.Receive_Batch
	            AND a.Batch_NO = b.Batch_NO
	            AND a.Shop_ID = b.Shop_ID
	            AND a.Sign_Type = b.Sign_Type
            ORDER BY Sign_Type,Page, Batch_NO ";

        //* 產生一般簽單高收檔及分期簽單高收檔 (以Sign_Type控制)
        //20140918  如果第一次平帳筆數為0，則調整筆數放空值；第一次平帳金額為0，調整金額放空值
        //20141008  加入Net_Count及Net_AMT以便計算 淨請款筆數 及 淨請款金額 (有調整數以調整數為準)
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        private const string SEARCH_ExportAS_ACQ =
        @" SELECT a.Sign_Type, a.Batch_NO, a.Shop_ID, a.Receive_Total_Count, a.Receive_Total_AMT, b.Adjust_Count, b.Adjust_AMT, 
	            b.Keyin_Reject_Count_All, b.Keyin_Reject_AMT_All, a.Reject_Reason, a.Page, 
	            CASE WHEN b.Adjust_Count IS NOT NULL THEN b.Adjust_Count ELSE a.Receive_Total_Count END Net_Count,
	            CASE WHEN b.Adjust_AMT IS NOT NULL THEN b.Adjust_AMT ELSE a.Receive_Total_AMT END Net_AMT
            FROM (
	            SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, Receive_Total_Count, Receive_Total_AMT, Page, 
		            (	SELECT ',' + Reject_Reason
			            FROM [{0}].[dbo].[Artificial_Signing_Detail] x
			            WHERE x.Batch_Date = y.Batch_Date AND x.Receive_Batch = y.Receive_Batch AND x.Batch_NO = y.Batch_NO
				            AND x.Shop_ID = y.Shop_ID AND Case_Status = '1' AND KeyIn_Flag = '2'
			            FOR XML path('')
			            ) AS Reject_Reason
	            FROM [{0}].[dbo].[Artificial_Signing_Batch_Data] y
	            WHERE Batch_Date = @Batch_Date AND Receive_Batch = @Receive_Batch AND Sign_Type = @Sign_Type
	            ) a
            LEFT JOIN (
	            SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID,
                    CASE WHEN First_Balance_Count = 0 THEN NULL ELSE Adjust_Count END as Adjust_Count, 
                    CASE WHEN First_Balance_AMT = 0 THEN NULL ELSE Adjust_AMT END as Adjust_AMT, 
                    Keyin_Reject_Count_All, Keyin_Reject_AMT_All
	            FROM [{0}].[dbo].[Artificial_Signing_Primary]
	            WHERE KeyIn_Flag = '2'
	            ) b ON a.Batch_Date = b.Batch_Date
	            AND a.Receive_Batch = b.Receive_Batch
	            AND a.Batch_NO = b.Batch_NO
	            AND a.Shop_ID = b.Shop_ID
            ORDER BY Page, Batch_NO ";

        //* 上傳檔案(一般簽單及分期簽單)資料檢核
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        private const string CHECK_AS_UploadData =
        @"SELECT TOP 1 *
            FROM (
	            SELECT *
	            FROM [{0}].[dbo].[Artificial_Signing_Batch_Data]
	            WHERE Batch_Date = @Batch_Date
		            AND Receive_Batch = @Receive_Batch
		            AND Sign_Type = @Sign_Type
	            ) a
            LEFT JOIN (
	            SELECT *
	            FROM [{0}].[dbo].[Artificial_Signing_Primary]
	            WHERE KeyIn_Flag = '2'
	            ) b ON a.Batch_Date = b.Batch_Date
	            AND a.Receive_Batch = b.Receive_Batch
	            AND a.Batch_NO = b.Batch_NO
	            AND a.Shop_ID = b.Shop_ID
	            AND a.Sign_Type = b.Sign_Type
            WHERE Balance_Flag = 'N'
	            OR Balance_Flag IS NULL
	            OR KeyIn_MatchFlag = 'N'
	            OR KeyIn_MatchFlag IS NULL
             ";

        //* 查詢上傳檔案 一般簽單 資料
        //20141008 第二列總金額為NET值：(請款-退款)取絕對值
        //20140925 正常件才列出，並且以正常件重新計算總筆數及總金額
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        private const string SEARCH_AS_UploadData =
        @"  --第二列為該[編批日期]，[收單批號]項下每一批號的資料
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
            ORDER BY Batch_NO, Shop_ID;";

        //* 查詢上傳檔案 分期簽單 資料
        //20141008 第二列總金額為NET值：(請款-退款)取絕對值
        //20140925 正常件才列出，並且以正常件重新計算總筆數及總金額
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        private const string SEARCH_AS_UploadData_inst =
        @"  --第二列為該[編批日期]，[收單批號]項下每一批號的資料
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
            SELECT Batch_NO, Shop_ID, Card_No, AMT, Tran_Date, Installment_Periods, Product_Type, Auth_Code
            FROM [{0}].[dbo].[Artificial_Signing_Detail]
            WHERE KeyIn_Flag = '2'
	            AND Batch_Date = @Batch_Date
	            AND Receive_Batch = @Receive_Batch
	            AND Sign_Type = @Sign_Type
                AND Case_Status = '0'
            ORDER BY Batch_NO, Shop_ID;";

        //* 更新[人工簽單批次資料檔] [處理註記]
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        private const string UPDATE_ASBD_Process_Flag =
        @" UPDATE [{0}].[dbo].[Artificial_Signing_Batch_Data]
            SET Process_Flag = @Process_Flag
            WHERE Batch_Date = @Batch_Date
	            AND Receive_Batch = @Receive_Batch
	            AND Sign_Type = @Sign_Type; ";

        //* 重覆件報表查詢
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        private const string SEARCH_RepeatReport =
        @" --一般簽單
        SELECT f.MaxRepeatDay, a.Batch_Date, a.Batch_NO, a.Receive_Batch, a.Shop_ID, 
        CASE WHEN a.Sign_Type='1' THEN '一般' ELSE '分期' END AS Sign_Type, 
        CASE WHEN d.Process_Flag='01' THEN d.Process_Flag + ' 已匯檔' ELSE d.Process_Flag + ' 已產檔' END AS Process_Flag, 
        a.SN, a.Card_No, a.Tran_Date, a.Installment_Periods, a.Auth_Code, a.AMT, 
        CASE WHEN a.Receipt_Type='40' THEN a.Receipt_Type + ' 請款' ELSE a.Receipt_Type + ' 退款' END AS Receipt_Type, 
        (e.Create_User) AS [1Key_user], (a.Create_User) AS [2Key_user]
        FROM (
	        SELECT DISTINCT c.*
	        FROM (
		        SELECT a.*
		        FROM [{2}].[dbo].[Artificial_Signing_Detail] a
		        JOIN (
			        SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, KeyIn_Flag
			        FROM [{2}].[dbo].[Artificial_Signing_Primary]
			        WHERE KeyIn_Flag = '2'
				        AND Balance_Flag = 'Y'
				        AND KeyIn_MatchFlag = 'Y'
				        AND Batch_Date IN (
					        SELECT DISTINCT TOP {0} Batch_Date
					        FROM [{2}].[dbo].[Artificial_Signing_Detail]
					        ORDER BY Batch_Date DESC
					        )
				        AND Sign_Type = '1'
			        ) b ON a.Batch_Date = b.Batch_Date
			        AND a.Receive_Batch = b.Receive_Batch
			        AND a.Batch_NO = b.Batch_NO
			        AND a.Shop_ID = b.Shop_ID
			        AND a.KeyIn_Flag = b.KeyIn_Flag
			        AND a.Sign_Type = b.Sign_Type
		        ) c
	        JOIN (
		        SELECT a.*
		        FROM [{2}].[dbo].[Artificial_Signing_Detail] a
		        JOIN (
			        SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, KeyIn_Flag
			        FROM [{2}].[dbo].[Artificial_Signing_Primary]
			        WHERE KeyIn_Flag = '2'
				        AND Balance_Flag = 'Y'
				        AND KeyIn_MatchFlag = 'Y'
				        AND Batch_Date IN (
					        SELECT DISTINCT TOP {0} Batch_Date
					        FROM [{2}].[dbo].[Artificial_Signing_Detail]
					        ORDER BY Batch_Date DESC
					        )
				        AND Sign_Type = '1'
			        ) b ON a.Batch_Date = b.Batch_Date
			        AND a.Receive_Batch = b.Receive_Batch
			        AND a.Batch_NO = b.Batch_NO
			        AND a.Shop_ID = b.Shop_ID
			        AND a.KeyIn_Flag = b.KeyIn_Flag
			        AND a.Sign_Type = b.Sign_Type
		        ) d ON c.Shop_ID = d.Shop_ID
		        AND c.Card_No = d.Card_No
		        AND c.Tran_Date = d.Tran_Date
		        AND c.Auth_Code = d.Auth_Code
		        AND c.AMT = d.AMT
		        AND c.Receipt_Type = d.Receipt_Type
		        AND (
			        c.Batch_Date <> d.Batch_Date
			        OR c.Receive_Batch <> d.Receive_Batch
			        OR c.Batch_NO <> d.Batch_NO
			        )
	        ) a
        LEFT JOIN [{2}].[dbo].[Artificial_Signing_Batch_Data] d ON a.Batch_Date = d.Batch_Date
	        AND a.Receive_Batch = d.Receive_Batch
	        AND a.Batch_NO = d.Batch_NO
	        AND a.Shop_ID = d.Shop_ID
	        AND a.Sign_Type = d.Sign_Type
        LEFT JOIN [{2}].[dbo].[Artificial_Signing_Detail] e ON e.KeyIn_Flag = '1'
	        AND a.Batch_Date = e.Batch_Date
	        AND a.Shop_ID = e.Shop_ID
	        AND a.Batch_NO = e.Batch_NO
	        AND a.Receive_Batch = e.Receive_Batch
	        AND a.SN = e.SN
	        AND a.CASe_Status = e.CASe_Status
	        AND a.Sign_Type = e.Sign_Type
        LEFT JOIN (
	        SELECT MAX(Batch_Date) MaxRepeatDay, Shop_ID, Card_No, Tran_Date, Auth_Code, AMT, Receipt_Type
	        FROM (
		        SELECT a.*
		        FROM [{2}].[dbo].[Artificial_Signing_Detail] a
		        JOIN (
			        SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, KeyIn_Flag
			        FROM [{2}].[dbo].[Artificial_Signing_Primary]
			        WHERE KeyIn_Flag = '2'
				        AND Balance_Flag = 'Y'
				        AND KeyIn_MatchFlag = 'Y'
				        AND Batch_Date IN (
					        SELECT DISTINCT TOP {0} Batch_Date
					        FROM [{2}].[dbo].[Artificial_Signing_Detail]
					        ORDER BY Batch_Date DESC
					        )
				        AND Sign_Type = '1'
			        ) b ON a.Batch_Date = b.Batch_Date
			        AND a.Receive_Batch = b.Receive_Batch
			        AND a.Batch_NO = b.Batch_NO
			        AND a.Shop_ID = b.Shop_ID
			        AND a.KeyIn_Flag = b.KeyIn_Flag
			        AND a.Sign_Type = b.Sign_Type
		        ) c
	        GROUP BY Shop_ID, Card_No, Tran_Date, Auth_Code, AMT, Receipt_Type
	        HAVING count(*) > 1
	        ) f ON a.Shop_ID = f.Shop_ID
	        AND a.Card_No = f.Card_No
	        AND a.Tran_Date = f.Tran_Date
	        AND a.Auth_Code = f.Auth_Code
	        AND a.AMT = f.AMT
	        AND a.Receipt_Type = f.Receipt_Type
        	
        UNION ALL

        --分期簽單
        SELECT f.MaxRepeatDay, a.Batch_Date, a.Batch_NO, a.Receive_Batch, a.Shop_ID, 
        CASE WHEN a.Sign_Type='1' THEN '一般' ELSE '分期' END AS Sign_Type, 
        CASE WHEN d.Process_Flag='01' THEN d.Process_Flag + ' 已匯檔' ELSE d.Process_Flag + ' 已產檔' END AS Process_Flag, 
        a.SN, a.Card_No, a.Tran_Date, a.Installment_Periods, a.Auth_Code, a.AMT, 
        CASE WHEN a.Receipt_Type='40' THEN a.Receipt_Type + ' 請款' ELSE a.Receipt_Type + ' 退款' END AS Receipt_Type, 
        (e.Create_User) AS [1Key_user], (a.Create_User) AS [2Key_user]
        FROM (
	        SELECT DISTINCT c.*
	        FROM (
		        SELECT a.*
		        FROM [{2}].[dbo].[Artificial_Signing_Detail] a
		        JOIN (
			        SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, KeyIn_Flag
			        FROM [{2}].[dbo].[Artificial_Signing_Primary]
			        WHERE KeyIn_Flag = '2'
				        AND Balance_Flag = 'Y'
				        AND KeyIn_MatchFlag = 'Y'
				        AND Batch_Date IN (
					        SELECT DISTINCT TOP {1} Batch_Date
					        FROM [{2}].[dbo].[Artificial_Signing_Detail]
					        ORDER BY Batch_Date DESC
					        )
				        AND Sign_Type = '2'
			        ) b ON a.Batch_Date = b.Batch_Date
			        AND a.Receive_Batch = b.Receive_Batch
			        AND a.Batch_NO = b.Batch_NO
			        AND a.Shop_ID = b.Shop_ID
			        AND a.KeyIn_Flag = b.KeyIn_Flag
			        AND a.Sign_Type = b.Sign_Type
		        ) c
	        JOIN (
		        SELECT a.*
		        FROM [{2}].[dbo].[Artificial_Signing_Detail] a
		        JOIN (
			        SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, KeyIn_Flag
			        FROM [{2}].[dbo].[Artificial_Signing_Primary]
			        WHERE KeyIn_Flag = '2'
				        AND Balance_Flag = 'Y'
				        AND KeyIn_MatchFlag = 'Y'
				        AND Batch_Date IN (
					        SELECT DISTINCT TOP {1} Batch_Date
					        FROM [{2}].[dbo].[Artificial_Signing_Detail]
					        ORDER BY Batch_Date DESC
					        )
				        AND Sign_Type = '2'
			        ) b ON a.Batch_Date = b.Batch_Date
			        AND a.Receive_Batch = b.Receive_Batch
			        AND a.Batch_NO = b.Batch_NO
			        AND a.Shop_ID = b.Shop_ID
			        AND a.KeyIn_Flag = b.KeyIn_Flag
			        AND a.Sign_Type = b.Sign_Type
		        ) d ON c.Shop_ID = d.Shop_ID
		        AND c.Card_No = d.Card_No
		        AND c.Tran_Date = d.Tran_Date
		        AND c.Auth_Code = d.Auth_Code
		        AND c.AMT = d.AMT
		        AND c.Receipt_Type = d.Receipt_Type
		        AND c.Product_Type = d.Product_Type
		        AND c.Installment_Periods = d.Installment_Periods
		        AND (
			        c.Batch_Date <> d.Batch_Date
			        OR c.Receive_Batch <> d.Receive_Batch
			        OR c.Batch_NO <> d.Batch_NO
			        )
	        ) a
        LEFT JOIN [{2}].[dbo].[Artificial_Signing_Batch_Data] d ON a.Batch_Date = d.Batch_Date
	        AND a.Receive_Batch = d.Receive_Batch
	        AND a.Batch_NO = d.Batch_NO
	        AND a.Shop_ID = d.Shop_ID
	        AND a.Sign_Type = d.Sign_Type
        LEFT JOIN [{2}].[dbo].[Artificial_Signing_Detail] e ON e.KeyIn_Flag = '1'
	        AND a.Batch_Date = e.Batch_Date
	        AND a.Shop_ID = e.Shop_ID
	        AND a.Batch_NO = e.Batch_NO
	        AND a.Receive_Batch = e.Receive_Batch
	        AND a.SN = e.SN
	        AND a.CASe_Status = e.CASe_Status
	        AND a.Sign_Type = e.Sign_Type
        LEFT JOIN (
	        SELECT MAX(Batch_Date) MaxRepeatDay, Shop_ID, Card_No, Tran_Date, Auth_Code, AMT, Receipt_Type, Product_Type, Installment_Periods
	        FROM (
		        SELECT a.*
		        FROM [{2}].[dbo].[Artificial_Signing_Detail] a
		        JOIN (
			        SELECT Batch_Date, Receive_Batch, Batch_NO, Shop_ID, Sign_Type, KeyIn_Flag
			        FROM [{2}].[dbo].[Artificial_Signing_Primary]
			        WHERE KeyIn_Flag = '2'
				        AND Balance_Flag = 'Y'
				        AND KeyIn_MatchFlag = 'Y'
				        AND Batch_Date IN (
					        SELECT DISTINCT TOP {1} Batch_Date
					        FROM [{2}].[dbo].[Artificial_Signing_Detail]
					        ORDER BY Batch_Date DESC
					        )
				        AND Sign_Type = '2'
			        ) b ON a.Batch_Date = b.Batch_Date
			        AND a.Receive_Batch = b.Receive_Batch
			        AND a.Batch_NO = b.Batch_NO
			        AND a.Shop_ID = b.Shop_ID
			        AND a.KeyIn_Flag = b.KeyIn_Flag
			        AND a.Sign_Type = b.Sign_Type
		        ) c
	        GROUP BY Shop_ID, Card_No, Tran_Date, Auth_Code, AMT, Receipt_Type, Product_Type, Installment_Periods
	        HAVING count(*) > 1
	        ) f ON a.Shop_ID = f.Shop_ID
	        AND a.Card_No = f.Card_No
	        AND a.Tran_Date = f.Tran_Date
	        AND a.Auth_Code = f.Auth_Code
	        AND a.AMT = f.AMT
	        AND a.Receipt_Type = f.Receipt_Type
            ORDER BY Sign_Type,Shop_ID,Card_No,Tran_Date,Auth_Code,Installment_Periods,AMT,Receipt_Type,MaxRepeatDay, Batch_Date DESC ";

        #endregion

        /// <summary>
        /// 請款簽單明細表 查詢
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>成功時：返回查詢結果；失敗時：null</returns>
        public static DataTable SearchASData(string strBatch_Date, string strReceive_Batch, ref string strMsgID, int intPageIndex, int intPageSize, ref int intTotolCount)
        {
            try
            {
                #region 依據Request查詢資料庫

                //* 聲明SQL Command變量
                SqlCommand sqlcmSearchData = new SqlCommand();
                sqlcmSearchData.CommandType = CommandType.Text;
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                sqlcmSearchData.CommandText = string.Format(SEARCH_AS, UtilHelper.GetAppSettings("DB_KeyinGUI"));
                //* 編批日期
                SqlParameter parmBatchDate = new SqlParameter("@Batch_Date", strBatch_Date.Replace("/", ""));
                sqlcmSearchData.Parameters.Add(parmBatchDate);
                //* 收件批次
                SqlParameter parmReceiveBatch = new SqlParameter("@Receive_Batch", strReceive_Batch);
                sqlcmSearchData.Parameters.Add(parmReceiveBatch);

                //* 查詢數據
                DataSet dstSearchData = BRASExport.SearchOnDataSet(sqlcmSearchData, intPageIndex, intPageSize, ref intTotolCount);
                if (null == dstSearchData)  //* 查詢數據失敗
                {
                    strMsgID = "01_01070100_001";
                    return null;
                }
                else
                {
                    //* 查詢的數據不存在時
                    if (dstSearchData.Tables[0].Rows.Count == 0)
                    {
                        strMsgID = "01_01070100_002";
                        return null;
                    }
                }
                //* 查詢成功
                strMsgID = "01_01070100_003";
                return dstSearchData.Tables[0];

                #endregion 依據Request查詢資料庫
            }
            catch (Exception exp)
            {
                BRCompareRpt.SaveLog(exp);
                strMsgID = "01_01070100_001";
                return null;
            }
        }

        /// <summary>
        /// 請款簽單明細表 查詢明細
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strBatch_NO">批號</param>
        /// <param name="strShop_ID">商店代號</param>
        /// <param name="strSign_Type">簽單類別</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>成功時：返回查詢結果；失敗時：null</returns>
        public static DataTable SearchASData_Detail(string strBatch_Date, string strReceive_Batch, string strBatch_NO, string strShop_ID, string strSign_Type,
            ref string strMsgID, int intPageIndex, int intPageSize, ref int intTotolCount)
        {
            try
            {
                #region 依據Request查詢資料庫

                //* 聲明SQL Command變量
                SqlCommand sqlcmSearchData = new SqlCommand();
                sqlcmSearchData.CommandType = CommandType.Text;
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                sqlcmSearchData.CommandText = string.Format(SEARCH_ASDetail, UtilHelper.GetAppSettings("DB_KeyinGUI"));
                //* 編批日期
                SqlParameter parmBatchDate = new SqlParameter("@Batch_Date", strBatch_Date.Replace("/", ""));
                sqlcmSearchData.Parameters.Add(parmBatchDate);
                //* 收件批次
                SqlParameter parmReceiveBatch = new SqlParameter("@Receive_Batch", strReceive_Batch);
                sqlcmSearchData.Parameters.Add(parmReceiveBatch);
                //* 批號
                SqlParameter parmBatchNO = new SqlParameter("@Batch_NO", strBatch_NO);
                sqlcmSearchData.Parameters.Add(parmBatchNO);
                //* 商店代號
                SqlParameter parmShopID = new SqlParameter("@Shop_ID", strShop_ID);
                sqlcmSearchData.Parameters.Add(parmShopID);
                //* 簽單類別
                SqlParameter parmSign_Type = new SqlParameter("@Sign_Type", strSign_Type);
                sqlcmSearchData.Parameters.Add(parmSign_Type);

                //* 查詢數據
                DataSet dstSearchData = BRASExport.SearchOnDataSet(sqlcmSearchData, intPageIndex, intPageSize, ref intTotolCount);
                if (null == dstSearchData)  //* 查詢數據失敗
                {
                    strMsgID = "01_01070100_001";
                    return null;
                }
                else
                {
                    //* 查詢的數據不存在時
                    if (dstSearchData.Tables[0].Rows.Count == 0)
                    {
                        strMsgID = "01_01070100_002";
                        return null;
                    }
                }
                //* 查詢成功
                strMsgID = "01_01070100_003";
                return dstSearchData.Tables[0];

                #endregion 依據Request查詢資料庫
            }
            catch (Exception exp)
            {
                BRCompareRpt.SaveLog(exp);
                strMsgID = "01_01070100_001";
                return null;
            }
        }

        #region 產生請款簽單明細表
        /// <summary>
        /// 產生請款簽單明細表Excel
        /// 修改紀錄：2021/01/07_Ares_Stanley-變更報表產出方式為NPOI
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
        public static bool ExportASDetail(string strBatch_Date, string strReceive_Batch, ref string strPathFile, ref string strMsgID)
        {
            //解決"格式太舊或是類型程式庫無效。 (Exception from HRESULT: 0x80028018 (TYPE_E_INVDATAREAD))"問題用
            System.Globalization.CultureInfo oldCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            strMsgID = "";
            //*创建一个 Excel 实例
            try
            {
                #region 新CODE
                //* 檢查目錄，并刪除以前的文檔資料
                CSIPKeyInGUI.BusinessRules.BRExcel_File.CheckDirectory(ref strPathFile);

                //* 取要下載的資料
                DataTable dt = BRASExport.getData_ExportASDetail(strBatch_Date, strReceive_Batch, ref strMsgID);
                if (dt == null)
                    return false;

                #region 匯入Excel文檔

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "ASDetail.xls";
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                //ExportExcelForNPOI(dt, ref wb, 2, "頁1");
                ISheet sheet = wb.GetSheet("頁1");
                // 編批日期
                sheet.GetRow(0).GetCell(1).SetCellValue(strBatch_Date);
                // 收件批次
                sheet.GetRow(0).GetCell(4).SetCellValue(strReceive_Batch);

                #region 計算頁數,預畫表格
                int totalPage = 1;
                int pageRowsCount = 1;
                DataTable dtPage = dt.DefaultView.ToTable(true, new string[] { "Sign_Type", "Page" });
                #region 總頁數計算
                for (int p = 1; p < dt.Rows.Count; p++)
                {
                    pageRowsCount += 1;
                    if (dt.Rows[p]["Page"].ToString() != dt.Rows[p - 1]["Page"].ToString())
                    {
                        totalPage += 1;
                        pageRowsCount = 1;
                    }
                    if (pageRowsCount > 30)
                    {
                        totalPage += 1;
                        pageRowsCount = 1;
                    }
                }
                #endregion 總頁數

                // 建立第一個表格的內容儲存格
                for (int row = 2; row < 32; row++)
                {
                    for (int col = 0; col < 13; col++)
                    {
                        if (sheet.GetRow(row).GetCell(col).StringCellValue == null)
                        {
                            sheet.GetRow(row).CreateCell(col);
                        }
                    }
                }
                // 複製第一個表格
                // 表尾公式
                // B32
                int formuSUM_start = 3;
                int formuSUM_end = 32;
                int formuTOTAL = 33;
                string formulaB32 = "SUM(B{0}:B{1})";
                string formulaE32 = "SUM(E{0}:E{1})";
                string formulaF32 = "SUM(F{0}:F{1})";
                string formulaG32 = "SUM(G{0}:G{1})";
                string formulaH32 = "SUM(H{0}:H{1})";
                string formulaI32 = "SUM(I{0}:I{1})";
                string formulaJ32 = "SUM(J{0}:J{1})";
                string formulaF33 = "E{0}+G{1}-I{2}-SUMIF(G{3}:G{4},\" > 0\",E{5}:E{6})";
                string formulaF34 = "F{0}+H{1}-J{2}-SUMPRODUCT((H{3}:H{4}<>\"\")*F{5}:F{6})";
                for (int pageCount = 1; pageCount < totalPage; pageCount++)
                {
                    for (int row = 0; row < 35; row++)
                    {
                        sheet.CreateRow(sheet.LastRowNum + 1);
                        for (int col = 0; col < 13; col++)
                        {
                            sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = sheet.GetRow(row).GetCell(col).CellStyle;
                            // 判斷儲存格是否為函數類型
                            if (sheet.GetRow(row).GetCell(col).CellType.ToString() == "Formula")
                            {
                                sheet.GetRow(sheet.LastRowNum).GetCell(col).SetCellType(CellType.Formula);
                                if (row == 32)
                                {
                                    switch (col)
                                    {
                                        case 1:
                                            sheet.GetRow(sheet.LastRowNum).GetCell(col).SetCellFormula(string.Format(formulaB32, formuSUM_start + (35 * pageCount), formuSUM_end + (35 * pageCount)));
                                            break;
                                        case 4:
                                            sheet.GetRow(sheet.LastRowNum).GetCell(col).SetCellFormula(string.Format(formulaE32, formuSUM_start + (35 * pageCount), formuSUM_end + (35 * pageCount)));
                                            break;
                                        case 5:
                                            sheet.GetRow(sheet.LastRowNum).GetCell(col).SetCellFormula(string.Format(formulaF32, formuSUM_start + (35 * pageCount), formuSUM_end + (35 * pageCount)));
                                            break;
                                        case 6:
                                            sheet.GetRow(sheet.LastRowNum).GetCell(col).SetCellFormula(string.Format(formulaG32, formuSUM_start + (35 * pageCount), formuSUM_end + (35 * pageCount)));
                                            break;
                                        case 7:
                                            sheet.GetRow(sheet.LastRowNum).GetCell(col).SetCellFormula(string.Format(formulaH32, formuSUM_start + (35 * pageCount), formuSUM_end + (35 * pageCount)));
                                            break;
                                        case 8:
                                            sheet.GetRow(sheet.LastRowNum).GetCell(col).SetCellFormula(string.Format(formulaI32, formuSUM_start + (35 * pageCount), formuSUM_end + (35 * pageCount)));
                                            break;
                                        case 9:
                                            sheet.GetRow(sheet.LastRowNum).GetCell(col).SetCellFormula(string.Format(formulaJ32, formuSUM_start + (35 * pageCount), formuSUM_end + (35 * pageCount)));
                                            break;
                                    }
                                }
                                else if (row == 33 && col == 5)
                                {
                                    sheet.GetRow(sheet.LastRowNum).GetCell(col).SetCellFormula(string.Format(formulaF33, formuTOTAL + (35 * pageCount), formuTOTAL + (35 * pageCount), formuTOTAL + (35 * pageCount), formuSUM_start + (35 * pageCount), formuSUM_end + (35 * pageCount), formuSUM_start + (35 * pageCount), formuSUM_end + (35 * pageCount)));
                                }
                                else if (row == 34 && col == 5)
                                {
                                    sheet.GetRow(sheet.LastRowNum).GetCell(col).SetCellFormula(string.Format(formulaF34, formuTOTAL + (35 * pageCount), formuTOTAL + (35 * pageCount), formuTOTAL + (35 * pageCount), formuSUM_start + (35 * pageCount), formuSUM_end + (35 * pageCount), formuSUM_start + (35 * pageCount), formuSUM_end + (35 * pageCount)));
                                }
                                else
                                {
                                    sheet.GetRow(sheet.LastRowNum).GetCell(col).SetCellFormula(sheet.GetRow(row).GetCell(col).CellFormula);
                                }
                            }
                            else if (sheet.GetRow(row).GetCell(col).CellType.ToString() == "Blank")
                            {
                                sheet.GetRow(sheet.LastRowNum).GetCell(col).SetCellType(CellType.Blank);
                                if (row == 0 && col == 2)
                                {
                                    sheet.GetRow(sheet.LastRowNum).GetCell(col).CellStyle = sheet.GetRow(0).GetCell(1).CellStyle;
                                }
                            }
                            else
                            {
                                sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = sheet.GetRow(row).GetCell(col).CellStyle;
                                if (sheet.GetRow(row).GetCell(col).StringCellValue != "")
                                {
                                    sheet.GetRow(sheet.LastRowNum).GetCell(col).SetCellValue(sheet.GetRow(row).GetCell(col).StringCellValue.ToString());
                                }

                            }
                        }
                    }
                }
                // 覆寫page1的公式
                sheet.GetRow(32).GetCell(1).SetCellFormula("SUM(B3:B32)");
                sheet.GetRow(32).GetCell(4).SetCellFormula("SUM(E3:E32)");
                sheet.GetRow(32).GetCell(5).SetCellFormula("SUM(F3:F32)");
                sheet.GetRow(32).GetCell(6).SetCellFormula("SUM(G3:G32)");
                sheet.GetRow(32).GetCell(7).SetCellFormula("SUM(H3:H32)");
                sheet.GetRow(32).GetCell(8).SetCellFormula("SUM(I3:I32)");
                sheet.GetRow(32).GetCell(9).SetCellFormula("SUM(J3:J32)");
                sheet.GetRow(33).GetCell(5).SetCellFormula("E33+G33-I33-SUMIF(G3:G32,\" > 0\",E3:E32)");
                sheet.GetRow(34).GetCell(5).SetCellFormula("F33+H33-J33-SUMPRODUCT((H3:H32<>\"\")*F3:F32)");
                #endregion

                #region 代入儲存格資料
                int iDataStar = 2;      //每個Page資料開始列數
                int iDataIndex = 0;     //datatable資料位置
                string strPage = string.Empty;  //存放目前頁數

                for (int i = 0; i < totalPage; i++)
                {
                    for (int intLoop = 0; intLoop < 30; intLoop++)
                    {
                        if (iDataIndex > 0 && intLoop > 0)
                        {
                            if (iDataIndex >= dt.Rows.Count)
                            {
                                strPage = dt.Rows[iDataIndex - 1]["Page"].ToString();
                                //for (int x = 0; x < 13; x++)
                                //{
                                //    arrExportData[intLoop, x] = "";
                                //}
                                //continue;
                                break;
                            }
                            if (dt.Rows[iDataIndex]["Page"].ToString() != dt.Rows[iDataIndex - 1]["Page"].ToString())
                            {
                                strPage = dt.Rows[iDataIndex - 1]["Page"].ToString();
                                //for (int x = 0; x < 13; x++)
                                //{
                                //    arrExportData[intLoop, x] = "";
                                //}
                                //continue;
                                break;
                            }
                        }
                        strPage = dt.Rows[iDataIndex]["Page"].ToString();

                        //* 備註 ([簽單類別] , 一般簽單,值:1,分期簽單,值:2)
                        sheet.GetRow(iDataStar + intLoop + (i * 35)).GetCell(0).SetCellValue(dt.Rows[iDataIndex]["Sign_Type"].ToString() == "1" ? "一般簽單" : "分期簽單");
                        //* 批數
                        sheet.GetRow(iDataStar + intLoop + (i * 35)).GetCell(1).SetCellValue("");
                        //* 批號 (一般簽單3碼，分期簽單4碼 補0)
                        if (dt.Rows[iDataIndex]["Sign_Type"].ToString() == "1")
                            sheet.GetRow(iDataStar + intLoop + (i * 35)).GetCell(2).SetCellValue(dt.Rows[iDataIndex]["Batch_NO"].ToString().PadLeft(3, '0'));
                        else
                            sheet.GetRow(iDataStar + intLoop + (i * 35)).GetCell(2).SetCellValue(dt.Rows[iDataIndex]["Batch_NO"].ToString().PadLeft(4, '0'));
                        //* 特店代號
                        sheet.GetRow(iDataStar + intLoop + (i * 35)).GetCell(3).SetCellValue(dt.Rows[iDataIndex]["Shop_ID"].ToString());
                        //* 請款筆數
                        if (dt.Rows[iDataIndex]["Receive_Total_Count"].ToString() != "")
                        {
                            sheet.GetRow(iDataStar + intLoop + (i * 35)).GetCell(4).SetCellValue(Convert.ToInt32(dt.Rows[iDataIndex]["Receive_Total_Count"].ToString()));
                        }
                        //* 請款金額
                        if (dt.Rows[iDataIndex]["Receive_Total_AMT"].ToString() != "")
                        {
                            sheet.GetRow(iDataStar + intLoop + (i * 35)).GetCell(5).SetCellValue(Convert.ToInt32(dt.Rows[iDataIndex]["Receive_Total_AMT"].ToString()));
                        }
                        //* 調整筆數
                        if (dt.Rows[iDataIndex]["Adjust_Count"].ToString() != "")
                        {
                            sheet.GetRow(iDataStar + intLoop + (i * 35)).GetCell(6).SetCellValue(Convert.ToInt32(dt.Rows[iDataIndex]["Adjust_Count"].ToString()));
                        }
                        //* 調整金額
                        if (dt.Rows[iDataIndex]["Adjust_AMT"].ToString() != "")
                        {
                            sheet.GetRow(iDataStar + intLoop + (i * 35)).GetCell(7).SetCellValue(Convert.ToInt32(dt.Rows[iDataIndex]["Adjust_AMT"].ToString()));
                        }
                        //* 剔退筆數
                        if (dt.Rows[iDataIndex]["Keyin_Reject_Count_All"].ToString() != "")
                        {
                            sheet.GetRow(iDataStar + intLoop + (i * 35)).GetCell(8).SetCellValue(Convert.ToInt32(dt.Rows[iDataIndex]["Keyin_Reject_Count_All"].ToString()));
                        }
                        //* 剔退金額
                        if (dt.Rows[iDataIndex]["Keyin_Reject_AMT_All"].ToString() != "")
                        {
                            sheet.GetRow(iDataStar + intLoop + (i * 35)).GetCell(9).SetCellValue(Convert.ToInt32(dt.Rows[iDataIndex]["Keyin_Reject_AMT_All"].ToString()));
                        }
                        //* 更動後請款筆數
                        sheet.GetRow(iDataStar + intLoop + (i * 35)).GetCell(10).SetCellValue("");
                        //* 更動後金額
                        sheet.GetRow(iDataStar + intLoop + (i * 35)).GetCell(11).SetCellValue("");
                        //* 附註 (Substring去掉第一個,)
                        sheet.GetRow(iDataStar + intLoop + (i * 35)).GetCell(12).SetCellValue(dt.Rows[iDataIndex]["Reject_Reason"].ToString().Length > 0 ? dt.Rows[iDataIndex]["Reject_Reason"].ToString().Substring(1) : "");

                        iDataIndex++;
                    }
                    sheet.GetRow(33 + (i * 35)).GetCell(12).SetCellValue(string.Format("【{0}】", strPage));
                }
                #endregion 代入儲存格資料結束
                //HSSFFormulaEvaluator.EvaluateAllFormulaCells(wb);
                //* 保存文件到程序运行目录下
                strPathFile = strPathFile + @"\ASDetail_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();
                strMsgID = "01_01070100_005";
                return true;
                #endregion 匯入文檔結束
                #endregion 新CODE
                #region 舊CODE
                ////* 檢查目錄，并刪除以前的文檔資料
                //BRExcel_File.CheckDirectory(ref strPathFile);

                ////* 取要下載的資料
                //DataTable dt = BRASExport.getData_ExportASDetail(strBatch_Date, strReceive_Batch, ref strMsgID);
                //if (dt == null)
                //    return false;

                //#region 匯入Excel文檔

                //string[,] arrExportData = null;
                //excel.Visible = false;//* 不显示 Excel 文件,如果为 true 则显示 Excel 文件

                //string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "ASDetail.xls";
                //Workbook workbook = excel.Workbooks.Open(strExcelPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                //                        Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                //                        Missing.Value, Missing.Value, Missing.Value, Missing.Value);

                //excel.Application.DisplayAlerts = false;

                //Range range = null;//* 创建一个空的单元格对象
                //int intRowIndexInSheet = 0;
                //Worksheet sheet = null;

                //#region 每30筆顯示為一頁，先計算頁數並畫表格
                //sheet = (Worksheet)workbook.Sheets[1];

                ////先填上 編批日期 及 收件批次
                //range = sheet.get_Range("B1", Missing.Value);
                //range.Value2 = strBatch_Date;
                //range = sheet.get_Range("E1", Missing.Value);
                //range.Value2 = strReceive_Batch;

                ////計算頁數
                ////20140918  改已匯入檔PAGE為主，頁數計算方式需修改
                //int iPageDataRows = 30; //每頁資料筆數
                //int iTotalPage = 0;     //總頁數
                ////if (dt.Rows.Count % iPageDataRows > 0)
                ////    iTotalPage = (dt.Rows.Count / iPageDataRows) + 1;
                ////else
                ////    iTotalPage = (dt.Rows.Count / iPageDataRows);
                //DataTable dtPage = dt.DefaultView.ToTable(true, new string[] { "Sign_Type", "Page" });
                //iTotalPage = dtPage.Rows.Count;

                ////畫表格
                //int iPageRow = 35;  //一個PAGE的總列數
                //range = sheet.get_Range("A1", "M"+iPageRow.ToString());
                //for (int i = 1; i < iTotalPage; i++)
                //{
                //    intRowIndexInSheet = (i * iPageRow) + 1;
                //    range.Copy(sheet.get_Range("A" + intRowIndexInSheet.ToString(), Missing.Value));
                //}
                //#endregion 每30筆顯示為一頁，先計算頁數並畫表格

                //#region 寫入資料
                //int iDataStar = 3;      //每個Page資料開始列數
                //int iDataIndex = 0;     //datatable資料位置
                //int intRowIndexEnd = 0; //區段資料結束列
                //int iFooterRows = 3;    //頁尾列數
                //string strPage = string.Empty;  //存放目前頁數

                //for (int i = 0; i < iTotalPage; i++)
                //{
                //    arrExportData = new string[30, 13];   //每30筆為一個區段
                //    //每一PAGE取30筆資料填入
                //    for (int intLoop = 0; intLoop < 30; intLoop++)
                //    {
                //        //20140918  改為已匯入檔PAGE分頁，30筆一頁
                //        //if (iDataIndex >= dt.Rows.Count)
                //        //    break;
                //        if (iDataIndex > 0 && intLoop > 0)
                //        {
                //            if (iDataIndex >= dt.Rows.Count)
                //            {
                //                strPage = dt.Rows[iDataIndex-1]["Page"].ToString();
                //                for (int x = 0; x < 13; x++)
                //                {
                //                    arrExportData[intLoop, x] = "";
                //                }
                //                continue;
                //            }
                //            if (dt.Rows[iDataIndex]["Page"].ToString() != dt.Rows[iDataIndex - 1]["Page"].ToString())
                //            {
                //                strPage = dt.Rows[iDataIndex - 1]["Page"].ToString();
                //                for (int x = 0; x < 13; x++)
                //                {
                //                    arrExportData[intLoop, x] = "";
                //                }
                //                continue;
                //            }
                //        }
                //        strPage = dt.Rows[iDataIndex]["Page"].ToString();

                //        //* 備註 ([簽單類別] , 一般簽單,值:1,分期簽單,值:2)
                //        arrExportData[intLoop, 0] = dt.Rows[iDataIndex]["Sign_Type"].ToString() == "1" ? "一般簽單" : "分期簽單";
                //        //* 批數
                //        arrExportData[intLoop, 1] = "";
                //        //* 批號 (一般簽單3碼，分期簽單4碼 補0)
                //        if (dt.Rows[iDataIndex]["Sign_Type"].ToString() == "1")
                //            arrExportData[intLoop, 2] = dt.Rows[iDataIndex]["Batch_NO"].ToString().PadLeft(3,'0');
                //        else
                //            arrExportData[intLoop, 2] = dt.Rows[iDataIndex]["Batch_NO"].ToString().PadLeft(4, '0');
                //        //* 特店代號
                //        arrExportData[intLoop, 3] = dt.Rows[iDataIndex]["Shop_ID"].ToString();
                //        //* 請款筆數
                //        arrExportData[intLoop, 4] = dt.Rows[iDataIndex]["Receive_Total_Count"].ToString();
                //        //* 請款金額
                //        arrExportData[intLoop, 5] = dt.Rows[iDataIndex]["Receive_Total_AMT"].ToString();
                //        //* 調整筆數
                //        arrExportData[intLoop, 6] = dt.Rows[iDataIndex]["Adjust_Count"].ToString();
                //        //* 調整金額
                //        arrExportData[intLoop, 7] = dt.Rows[iDataIndex]["Adjust_AMT"].ToString();
                //        //* 剔退筆數
                //        arrExportData[intLoop, 8] = dt.Rows[iDataIndex]["Keyin_Reject_Count_All"].ToString();
                //        //* 剔退金額
                //        arrExportData[intLoop, 9] = dt.Rows[iDataIndex]["Keyin_Reject_AMT_All"].ToString();
                //        //* 更動後請款筆數
                //        arrExportData[intLoop, 10] = "";
                //        //* 更動後金額
                //        arrExportData[intLoop, 11] = "";
                //        //* 附註 (Substring去掉第一個,)
                //        arrExportData[intLoop, 12] = dt.Rows[iDataIndex]["Reject_Reason"].ToString().Length > 0 ? dt.Rows[iDataIndex]["Reject_Reason"].ToString().Substring(1) : "";
                //        iDataIndex++;
                //    }
                //    intRowIndexInSheet = (i * iPageRow) + iDataStar;
                //    intRowIndexEnd = ((i + 1) * iPageRow) - iFooterRows;
                //    range = sheet.get_Range("A" + intRowIndexInSheet.ToString(), "M" + intRowIndexEnd.ToString());
                //    range.Value2 = arrExportData;
                //    range.Value2 = range.Value2;
                //    //頁數
                //    intRowIndexInSheet = ((i + 1) * iPageRow) - 1;
                //    range = sheet.get_Range("M" + intRowIndexInSheet.ToString(), Missing.Value);
                //    //20140918  改用匯入檔PAGE欄位
                //    //range.Value2 = "【" + Convert.ToString((i + 1)) + "】";
                //    range.Value2 = "【" + strPage + "】";
                //}
                //#endregion 寫入資料

                ////* 保存文件到程序运行目录下
                //strPathFile = strPathFile + @"\ASDetail_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                //sheet.SaveAs(strPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                //excel.ActiveWorkbook.Close(false, null, null); //* 关闭 Excel 文件且不保存
                //strMsgID = "01_01070100_005";
                //return true;
                //# endregion 匯入文檔結束
                #endregion 舊CODE

            }
            catch (Exception ex)
            {
                strMsgID = "01_01070100_004";
                Logging.Log(ex, LogLayer.BusinessRule);
                throw ex;
            }
            finally
            {
                //解決"格式太舊或是類型程式庫無效。 (Exception from HRESULT: 0x80028018 (TYPE_E_INVDATAREAD))"問題用
                System.Threading.Thread.CurrentThread.CurrentCulture = oldCI;
            }
        }

        /// <summary>
        /// 產生請款簽單明細表Excel時，查詢數據
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>成功時：返回查詢結果；失敗時：null</returns>
        public static DataTable getData_ExportASDetail(string strBatch_Date, string strReceive_Batch, ref string strMsgID)
        {
            try
            {
                #region 依據Request查詢資料庫

                //* 聲明SQL Command變量
                SqlCommand sqlcmSearchData = new SqlCommand();
                sqlcmSearchData.CommandType = CommandType.Text;
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                sqlcmSearchData.CommandText = string.Format(SEARCH_ExportASDetail, UtilHelper.GetAppSettings("DB_KeyinGUI"));
                //* 編批日期
                SqlParameter parmBatchDate = new SqlParameter("@Batch_Date", strBatch_Date.Replace("/", ""));
                sqlcmSearchData.Parameters.Add(parmBatchDate);
                //* 收件批次
                SqlParameter parmReceiveBatch = new SqlParameter("@Receive_Batch", strReceive_Batch);
                sqlcmSearchData.Parameters.Add(parmReceiveBatch);

                //* 查詢數據
                DataSet dstSearchData = BRASExport.SearchOnDataSet(sqlcmSearchData);
                if (null == dstSearchData)  //* 查詢數據失敗
                {
                    strMsgID = "01_01070100_001";
                    return null;
                }
                else
                {
                    //* 查詢的數據不存在時
                    if (dstSearchData.Tables[0].Rows.Count == 0)
                    {
                        strMsgID = "01_01070100_002";
                        return null;
                    }
                }
                //* 查詢成功
                strMsgID = "01_01070100_003";
                return dstSearchData.Tables[0];

                #endregion 依據Request查詢資料庫
            }
            catch (Exception exp)
            {
                BRCompareRpt.SaveLog(exp);
                strMsgID = "01_01070100_001";
                return null;
            }
        }
        #endregion 產生請款簽單明細表

        #region 產生一般簽單高收檔 或 分期簽單高收檔
        /// <summary>
        /// 產生一般簽單高收檔 或 分期簽單高收檔
        /// 修改紀錄：2021/01/07_Ares_Stanley-變更報表產出為NPOI
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strSign_Type">簽單類別(1:一般；2:分期)</param>
        /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
        public static bool ExportAS_ACQ(string strBatch_Date, string strReceive_Batch, string strSign_Type, ref string strPathFile, ref string strMsgID)
        {
            //解決"格式太舊或是類型程式庫無效。 (Exception from HRESULT: 0x80028018 (TYPE_E_INVDATAREAD))"問題用
            System.Globalization.CultureInfo oldCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            strMsgID = "";
            try
            {
                //* 檢查目錄，并刪除以前的文檔資料
                CSIPKeyInGUI.BusinessRules.BRExcel_File.CheckDirectory(ref strPathFile);

                //* 取要下載的資料
                DataTable dt = BRASExport.getData_ExportAS_ACQ(strBatch_Date, strReceive_Batch, strSign_Type, ref strMsgID);
                if (dt == null)
                    return false;

                #region 新CODE
                HSSFWorkbook wb = new HSSFWorkbook();  //新建workbook
                ISheet sheet = wb.CreateSheet("工作表1");
                #region 儲存格格式
                HSSFCellStyle contentFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
                contentFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
                contentFormat.Alignment = HorizontalAlignment.Center; //水平置中
                contentFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字
                contentFormat.BorderBottom = BorderStyle.Thin; // 儲存格框線
                contentFormat.BorderLeft = BorderStyle.Thin;
                contentFormat.BorderTop = BorderStyle.Thin;
                contentFormat.BorderRight = BorderStyle.Thin;
                HSSFFont contentFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                contentFont.FontHeightInPoints = 12; //字體大小
                contentFont.FontName = "新細明體"; //字型
                                               //contentFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold; //粗體
                contentFormat.SetFont(contentFont); //設定儲存格的文字樣式

                HSSFCellStyle footFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
                footFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
                footFormat.Alignment = HorizontalAlignment.Center; //水平置中
                footFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字
                HSSFFont footFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                footFont.FontHeightInPoints = 12; //字體大小
                footFont.FontName = "新細明體"; //字型
                                            //footFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold; //粗體
                footFormat.SetFont(footFont); //設定儲存格的文字樣式

                HSSFCellStyle red_borderFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
                red_borderFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
                red_borderFormat.Alignment = HorizontalAlignment.Center; //水平置中
                red_borderFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##0"); //將儲存格內容設定為文字
                red_borderFormat.BorderBottom = BorderStyle.Thin; // 儲存格框線
                red_borderFormat.BorderLeft = BorderStyle.Thin;
                red_borderFormat.BorderTop = BorderStyle.Thin;
                red_borderFormat.BorderRight = BorderStyle.Thin;
                HSSFFont red_borderFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                red_borderFont.FontHeightInPoints = 12; //字體大小
                red_borderFont.FontName = "新細明體"; //字型
                red_borderFont.Color = NPOI.HSSF.Util.HSSFColor.Red.Index;
                red_borderFormat.SetFont(red_borderFont);

                HSSFCellStyle red_noborderFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
                red_noborderFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
                red_noborderFormat.Alignment = HorizontalAlignment.Center; //水平置中
                red_noborderFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##0"); //將儲存格內容設定為文字
                HSSFFont red_noborderFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                red_noborderFont.FontHeightInPoints = 12; //字體大小
                red_noborderFont.FontName = "新細明體"; //字型
                red_noborderFont.Color = NPOI.HSSF.Util.HSSFColor.Red.Index;
                red_noborderFormat.SetFont(red_noborderFont);
                #endregion 儲存格格式

                // 表頭
                sheet.CreateRow(0);
                for (int row = 0; row < 2; row++)
                {
                    sheet.CreateRow(row);
                    for (int col = 0; col < 11; col++)
                    {
                        sheet.GetRow(row).CreateCell(col).CellStyle = contentFormat;
                    }
                }
                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 2)); // 合併儲存格
                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 3, 8)); // 合併儲存格
                sheet.GetRow(0).GetCell(0).SetCellValue("編批日期:" + strBatch_Date);
                if (strSign_Type == "1")
                    sheet.GetRow(0).GetCell(3).SetCellValue("一般簽單高收檔");
                else
                    sheet.GetRow(0).GetCell(3).SetCellValue("分期簽單高收檔");
                sheet.GetRow(0).GetCell(9).SetCellValue("處理日:");
                sheet.GetRow(1).GetCell(0).SetCellValue("頁次");
                sheet.GetRow(1).GetCell(1).SetCellValue("批號");
                sheet.GetRow(1).GetCell(2).SetCellValue("特店代號");
                sheet.GetRow(1).GetCell(3).SetCellValue("請款筆數");
                sheet.GetRow(1).GetCell(4).SetCellValue("請款金額");
                sheet.GetRow(1).GetCell(5).SetCellValue("調整筆數");
                sheet.GetRow(1).GetCell(6).SetCellValue("調整金額");
                sheet.GetRow(1).GetCell(7).SetCellValue("剔退筆數");
                sheet.GetRow(1).GetCell(8).SetCellValue("剔退金額");
                sheet.GetRow(1).GetCell(9).SetCellValue("原因碼");
                sheet.GetRow(1).GetCell(10).SetCellValue("正確商店代號");

                // 總頁數計算
                int totalPage = 1;
                int pageRowsCount = 1;
                int idataindex = 0;         //datatable資料位置
                int datarowstart = 3;     //區段資料結束列
                int datarowend = 32;   //從第二列開始寫入資料
                string formu_SUM = "SUM({0}{1}:{2}{3})";

                DataTable dtPage = dt.DefaultView.ToTable(true, new string[] { "Page" });
                for (int p = 1; p < dt.Rows.Count; p++)
                {
                    pageRowsCount += 1;
                    if (dt.Rows[p]["Page"].ToString() != dt.Rows[p - 1]["Page"].ToString())
                    {
                        totalPage += 1;
                        pageRowsCount = 1;
                    }
                    if (pageRowsCount > 30)
                    {
                        totalPage += 1;
                        pageRowsCount = 1;
                    }
                }
                for (int i = 0; i < totalPage; i++)
                {
                    for (int intloop = 0; intloop < 31; intloop++)
                    {
                        if (intloop == 30)
                        {
                            sheet.CreateRow(sheet.LastRowNum + 1);
                            for (int col = 0; col < 11; col++)
                            {
                                sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = contentFormat;
                            }
                            sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("合計");
                            sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellType(CellType.Formula);
                            sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellType(CellType.Formula);
                            sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellType(CellType.Formula);
                            sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellType(CellType.Formula);
                            sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellType(CellType.Formula);
                            sheet.GetRow(sheet.LastRowNum).GetCell(8).SetCellType(CellType.Formula);

                            sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellFormula(string.Format(formu_SUM, "D", datarowstart + (31 * i), "D", datarowend + (31 * i)));
                            sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellFormula(string.Format(formu_SUM, "E", datarowstart + (31 * i), "E", datarowend + (31 * i)));
                            sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellFormula(string.Format(formu_SUM, "F", datarowstart + (31 * i), "F", datarowend + (31 * i)));
                            sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellFormula(string.Format(formu_SUM, "G", datarowstart + (31 * i), "G", datarowend + (31 * i)));
                            sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellFormula(string.Format(formu_SUM, "H", datarowstart + (31 * i), "H", datarowend + (31 * i)));
                            sheet.GetRow(sheet.LastRowNum).GetCell(8).SetCellFormula(string.Format(formu_SUM, "I", datarowstart + (31 * i), "I", datarowend + (31 * i)));
                            break;
                        }

                        if (idataindex > 0 && intloop > 0)
                        {
                            if (idataindex >= dt.Rows.Count)
                            {
                                sheet.CreateRow(sheet.LastRowNum + 1);
                                for (int col = 0; col < 11; col++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(col).SetCellValue("");
                                    sheet.GetRow(sheet.LastRowNum).GetCell(col).CellStyle = contentFormat;
                                }
                                continue;
                            }
                            if (dt.Rows[idataindex]["page"].ToString() != dt.Rows[idataindex - 1]["page"].ToString())
                            {
                                sheet.CreateRow(sheet.LastRowNum + 1);
                                for (int col = 0; col < 11; col++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(col).SetCellValue("");
                                    sheet.GetRow(sheet.LastRowNum).GetCell(col).CellStyle = contentFormat;
                                }
                                continue;
                            }
                        }
                        sheet.CreateRow(sheet.LastRowNum + 1);
                        for (int col = 0; col < 11; col++)
                        {
                            sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = contentFormat;
                        }
                        // 頁次
                        sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue(dt.Rows[idataindex]["page"].ToString());
                        // 批號 (一般簽單3碼，分期簽單4碼 補0)
                        if (dt.Rows[idataindex]["sign_type"].ToString() == "1")
                            sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(dt.Rows[idataindex]["batch_no"].ToString().PadLeft(3, '0'));
                        else
                            sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(dt.Rows[idataindex]["batch_no"].ToString().PadLeft(4, '0'));
                        // 特店代號
                        sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue(dt.Rows[idataindex]["shop_id"].ToString());
                        // 請款筆數
                        if (dt.Rows[idataindex]["receive_total_count"].ToString() != "") { sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(Int32.Parse(dt.Rows[idataindex]["receive_total_count"].ToString())); } else { sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(0); }
                        // 請款金額
                        if (dt.Rows[idataindex]["receive_total_amt"].ToString() != "") { sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(Int32.Parse(dt.Rows[idataindex]["receive_total_amt"].ToString())); } else { sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(0); }
                        // 調整筆數
                        if (dt.Rows[idataindex]["adjust_count"].ToString() != "") { sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue(Int32.Parse(dt.Rows[idataindex]["adjust_count"].ToString())); } else { sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue(0); }
                        // 調整金額
                        if (dt.Rows[idataindex]["adjust_amt"].ToString() != "") { sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellValue(Int32.Parse(dt.Rows[idataindex]["adjust_amt"].ToString())); } else { sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellValue(0); }
                        // 剔退筆數
                        if (dt.Rows[idataindex]["keyin_reject_count_all"].ToString() != "") { sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(Int32.Parse(dt.Rows[idataindex]["keyin_reject_count_all"].ToString())); } else { sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(0); }
                        // 剔退金額
                        if (dt.Rows[idataindex]["keyin_reject_amt_all"].ToString() != "") { sheet.GetRow(sheet.LastRowNum).GetCell(8).SetCellValue(Int32.Parse(dt.Rows[idataindex]["keyin_reject_amt_all"].ToString())); } else { sheet.GetRow(sheet.LastRowNum).GetCell(8).SetCellValue(0); }
                        // 原因碼(substring去掉第一個,); 20141017  ,改以空格取代;
                        sheet.GetRow(sheet.LastRowNum).GetCell(9).SetCellValue(dt.Rows[idataindex]["reject_reason"].ToString().Length > 0 ? dt.Rows[idataindex]["reject_reason"].ToString().Substring(1).Replace(',', ' ') : "");
                        // 正確商店代號
                        sheet.GetRow(sheet.LastRowNum).GetCell(10).SetCellValue("");
                        idataindex++;
                    }
                }
                // 表尾總計
                int sumreceive_total_count_all = dt.Compute("sum(receive_total_count)", "") == DBNull.Value ? 0 : Convert.ToInt32(dt.Compute("sum(receive_total_count)", ""));
                decimal sumreceive_total_amt_all = dt.Compute("sum(receive_total_amt)", "") == DBNull.Value ? 0 : Convert.ToDecimal(dt.Compute("sum(receive_total_amt)", ""));
                int sumadjust_count_all = dt.Compute("sum(adjust_count)", "") == DBNull.Value ? 0 : Convert.ToInt32(dt.Compute("sum(adjust_count)", ""));
                decimal sumadjust_amt_all = dt.Compute("sum(adjust_amt)", "") == DBNull.Value ? 0 : Convert.ToDecimal(dt.Compute("sum(adjust_amt)", ""));
                int sumkeyin_reject_count_all_all = dt.Compute("sum(keyin_reject_count_all)", "") == DBNull.Value ? 0 : Convert.ToInt32(dt.Compute("sum(keyin_reject_count_all)", ""));
                decimal sumkeyin_reject_amt_all_all = dt.Compute("sum(keyin_reject_amt_all)", "") == DBNull.Value ? 0 : Convert.ToDecimal(dt.Compute("sum(keyin_reject_amt_all)", ""));
                int sumnet_count = dt.Compute("sum(net_count)", "") == DBNull.Value ? 0 : Convert.ToInt32(dt.Compute("sum(net_count)", ""));
                decimal sumnet_amt = dt.Compute("sum(net_amt)", "") == DBNull.Value ? 0 : Convert.ToDecimal(dt.Compute("sum(net_amt)", ""));

                // 表尾
                for (int row = 0; row < 4; row++)
                {
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int col = 0; col < 11; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = footFormat;
                    }
                }
                sheet.GetRow(sheet.LastRowNum - 3).GetCell(3).SetCellValue(Int32.Parse(sumreceive_total_count_all.ToString()));
                sheet.GetRow(sheet.LastRowNum - 3).GetCell(4).SetCellValue(Int32.Parse(sumreceive_total_amt_all.ToString()));
                sheet.GetRow(sheet.LastRowNum - 3).GetCell(5).SetCellValue(Int32.Parse(sumadjust_count_all.ToString()));
                sheet.GetRow(sheet.LastRowNum - 3).GetCell(6).SetCellValue(Int32.Parse(sumadjust_amt_all.ToString()));
                sheet.GetRow(sheet.LastRowNum - 3).GetCell(7).SetCellValue(Int32.Parse(sumkeyin_reject_count_all_all.ToString()));
                sheet.GetRow(sheet.LastRowNum - 3).GetCell(8).SetCellValue(Int32.Parse(sumkeyin_reject_amt_all_all.ToString()));
                sheet.GetRow(sheet.LastRowNum - 1).GetCell(3).SetCellValue("淨請款筆數");
                sheet.GetRow(sheet.LastRowNum - 1).GetCell(4).SetCellValue(Int32.Parse((sumnet_count - sumkeyin_reject_count_all_all).ToString()));
                sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue("淨請款金額");
                sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(Int32.Parse((sumnet_amt - sumkeyin_reject_amt_all_all).ToString()));

                // 設定千分位數字格式
                for (int row = 2; row < sheet.LastRowNum + 1; row++)
                {
                    for (int col = 3; col < 9; col++)
                    {
                        if (sheet.GetRow(row).GetCell(col).CellType.ToString() == "Numeric" || sheet.GetRow(row).GetCell(col).CellType.ToString() == "Formula")
                        {
                            sheet.GetRow(row).GetCell(col).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##0");
                        }
                    }
                }

                // 預先計算儲存格值
                HSSFFormulaEvaluator.EvaluateAllFormulaCells(wb);

                // 設定負數為紅色
                for (int row = 0; row < 4; row++)
                {
                    for (int col = 0; col < 11; col++)
                    {
                        if (sheet.GetRow(sheet.LastRowNum - row).GetCell(col).CellType.ToString() == "Numeric")
                        {
                            if (sheet.GetRow(sheet.LastRowNum - row).GetCell(col).NumericCellValue < 0)
                            {
                                sheet.GetRow(sheet.LastRowNum - row).GetCell(col).CellStyle = red_noborderFormat;
                            }
                        }
                    }
                }
                for (int row = 2; row < sheet.LastRowNum - 3; row++)
                {
                    for (int col = 3; col < 9; col++)
                    {
                        if (sheet.GetRow(row).GetCell(col).CellType.ToString() == "Numeric" || sheet.GetRow(row).GetCell(col).CellType.ToString() == "Formula")
                        {
                            if (sheet.GetRow(row).GetCell(col).NumericCellValue < 0)
                            {
                                sheet.GetRow(row).GetCell(col).CellStyle = red_borderFormat;
                            }
                        }
                    }
                }

                // 設定欄寬
                sheet.SetColumnWidth(0, (int)((5 + 0.72) * 256));
                sheet.SetColumnWidth(1, (int)((11 + 0.72) * 256));
                sheet.SetColumnWidth(2, (int)((13 + 0.72) * 256));
                sheet.SetColumnWidth(3, (int)((14 + 0.72) * 256));
                sheet.SetColumnWidth(4, (int)((11 + 0.72) * 256));
                sheet.SetColumnWidth(5, (int)((9 + 0.72) * 256)); ;
                sheet.SetColumnWidth(6, (int)((11 + 0.72) * 256));
                sheet.SetColumnWidth(7, (int)((9 + 0.72) * 256));
                sheet.SetColumnWidth(8, (int)((11 + 0.72) * 256));
                sheet.SetColumnWidth(9, (int)((10 + 0.72) * 256));
                sheet.SetColumnWidth(10, (int)((13 + 0.72) * 256));

                // 檔名
                string strFileName = string.Empty;
                //西元年轉民國年 yyyMMdd 共7碼，不足左邊補0
                string strBatch_Date_TW = BRASExport.ToTWDateString(DateTime.Parse(strBatch_Date));
                if (strSign_Type == "1")
                {
                    //* 一般簽單高收檔匯出檔案的檔名logic 為編批日期YYYMMDD(民國年)+收件批次+”A4”.xls
                    strFileName = strBatch_Date_TW + strReceive_Batch + "A4";
                }
                else
                {
                    //* 分期簽單高收檔匯出檔案的檔名logic 為”P”+編批日期YYYMMDD(民國年)+收件批次+”A4”.xls
                    strFileName = "P" + strBatch_Date_TW + strReceive_Batch + "A4";
                }
                strPathFile = strPathFile + @"\" + strFileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs);
                fs.Close();
                strMsgID = "01_01070100_005";
                return true;
                #endregion 新CODE

                #region 舊CODE
                //#region 匯入Excel文檔

                //string[,] arrExportData = null;
                //excel.Visible = false;//* 不显示 Excel 文件,如果为 true 则显示 Excel 文件
                //workbook = excel.Workbooks.Add(Missing.Value);//* 添加工作簿
                //excel.Application.DisplayAlerts = false;
                //sheet = (Worksheet)workbook.Sheets[1];

                //int intRowIndexInSheet = 0;

                //#region 表頭
                ////*表頭
                //range = sheet.get_Range("A1", "C1");
                //range.Value2 = "編批日期:" + strBatch_Date;
                //range.Merge(Missing.Value);//*合并单元格

                //range = sheet.get_Range("D1", "I1");
                //if(strSign_Type == "1")
                //    range.Value2 = "一般簽單高收檔";
                //else
                //    range.Value2 = "分期簽單高收檔";
                //range.Merge(Missing.Value);//*合并单元格

                //range = sheet.get_Range("J1", Missing.Value);
                //range.Value2 = "處理日:";

                ////* DataTable Title
                //range = sheet.get_Range("A2", Missing.Value);
                //range.Value2 = "頁次";

                //range = sheet.get_Range("B2", Missing.Value);
                //range.Value2 = "批號";

                //range = sheet.get_Range("C2", Missing.Value);
                //range.Value2 = "特店代號";
                //range.Borders.LineStyle = 1;

                //range = sheet.get_Range("D2", Missing.Value);
                //range.Value2 = "請款筆數";

                //range = sheet.get_Range("E2", Missing.Value);
                //range.Value2 = "請款金額";

                //range = sheet.get_Range("F2", Missing.Value);
                //range.Value2 = "調整筆數";

                //range = sheet.get_Range("G2", Missing.Value);
                //range.Value2 = "調整金額";

                //range = sheet.get_Range("H2", Missing.Value);
                //range.Value2 = "剔退筆數";

                //range = sheet.get_Range("I2", Missing.Value);
                //range.Value2 = "剔退金額";

                //range = sheet.get_Range("J2", Missing.Value);
                //range.Value2 = "原因碼";

                //range = sheet.get_Range("K2", Missing.Value);
                //range.Value2 = "正確商店代號";

                //intRowIndexInSheet = 2;
                //#endregion 表頭

                //#region 寫入資料
                //int iTotalPage = 0;         //總頁數
                //int iPageRow = 31;          //一個PAGE的總列數
                //int iDataIndex = 0;         //datatable資料位置
                //int intRowIndexEnd = 0;     //區段資料結束列
                //int intRowIndexStart = 3;   //從第三列開始寫入資料

                //iTotalPage = dt.DefaultView.ToTable(true, new string[] { "Page" }).Rows.Count;
                //for (int i = 0; i < iTotalPage; i++)
                //{
                //    arrExportData = new string[31, 11];     //每30筆資料加上合計為一個區段，共31列
                //    int sumReceive_Total_Count = 0;         //請款筆數 合計
                //    decimal sumReceive_Total_AMT = 0;       //請款金額 合計
                //    int sumAdjust_Count = 0;                //調整筆數 合計
                //    decimal sumAdjust_AMT = 0;              //調整金額 合計
                //    int sumKeyin_Reject_Count_All = 0;      //剔退筆數 合計
                //    decimal sumKeyin_Reject_AMT_All = 0;    //剔退金額 合計
                //    //每一PAGE取30筆資料填入
                //    for (int intLoop = 0; intLoop < 31; intLoop++)
                //    {
                //        if (intLoop == 30)
                //        {
                //            arrExportData[intLoop, 0] = "合計";
                //            arrExportData[intLoop, 1] = "";
                //            arrExportData[intLoop, 2] = "";
                //            arrExportData[intLoop, 3] = sumReceive_Total_Count.ToString();
                //            arrExportData[intLoop, 4] = sumReceive_Total_AMT.ToString();
                //            arrExportData[intLoop, 5] = sumAdjust_Count.ToString();
                //            arrExportData[intLoop, 6] = sumAdjust_AMT.ToString();
                //            arrExportData[intLoop, 7] = sumKeyin_Reject_Count_All.ToString();
                //            arrExportData[intLoop, 8] = sumKeyin_Reject_AMT_All.ToString();
                //            arrExportData[intLoop, 9] = "";
                //            arrExportData[intLoop, 10] = "";
                //            break;
                //        }   

                //        if (iDataIndex > 0 && intLoop > 0)
                //        {
                //            if (iDataIndex >= dt.Rows.Count)
                //            {
                //                arrExportData[intLoop, 0] = dt.Rows[iDataIndex - 1]["Page"].ToString();
                //                for (int x = 1; x < 11; x++)
                //                {
                //                    arrExportData[intLoop, x] = "";
                //                }
                //                continue;
                //            }
                //            if (dt.Rows[iDataIndex]["Page"].ToString() != dt.Rows[iDataIndex - 1]["Page"].ToString())
                //            {
                //                arrExportData[intLoop, 0] = dt.Rows[iDataIndex - 1]["Page"].ToString();
                //                for (int x = 1; x < 11; x++)
                //                {
                //                    arrExportData[intLoop, x] = "";
                //                }
                //                continue;
                //            }
                //        }
                //        //* 頁次
                //        arrExportData[intLoop, 0] = dt.Rows[iDataIndex]["Page"].ToString();
                //        //* 批號 (一般簽單3碼，分期簽單4碼 補0)
                //        if (dt.Rows[iDataIndex]["Sign_Type"].ToString() == "1")
                //            arrExportData[intLoop, 1] = dt.Rows[iDataIndex]["Batch_NO"].ToString().PadLeft(3, '0');
                //        else
                //            arrExportData[intLoop, 1] = dt.Rows[iDataIndex]["Batch_NO"].ToString().PadLeft(4, '0');
                //        //* 特店代號
                //        arrExportData[intLoop, 2] = dt.Rows[iDataIndex]["Shop_ID"].ToString();
                //        //* 請款筆數
                //        arrExportData[intLoop, 3] = dt.Rows[iDataIndex]["Receive_Total_Count"].ToString();
                //        //* 請款金額
                //        arrExportData[intLoop, 4] = dt.Rows[iDataIndex]["Receive_Total_AMT"].ToString();
                //        //* 調整筆數
                //        arrExportData[intLoop, 5] = dt.Rows[iDataIndex]["Adjust_Count"].ToString();
                //        //* 調整金額
                //        arrExportData[intLoop, 6] = dt.Rows[iDataIndex]["Adjust_AMT"].ToString();
                //        //* 剔退筆數
                //        arrExportData[intLoop, 7] = dt.Rows[iDataIndex]["Keyin_Reject_Count_All"].ToString();
                //        //* 剔退金額
                //        arrExportData[intLoop, 8] = dt.Rows[iDataIndex]["Keyin_Reject_AMT_All"].ToString();
                //        //* 原因碼(Substring去掉第一個,)
                //        //20141017  ,改以空格取代
                //        arrExportData[intLoop, 9] = dt.Rows[iDataIndex]["Reject_Reason"].ToString().Length > 0 ? dt.Rows[iDataIndex]["Reject_Reason"].ToString().Substring(1).Replace(',',' ') : "";
                //        //* 正確商店代號
                //        arrExportData[intLoop, 10] = "";

                //        //* 計算合計列資料
                //        sumReceive_Total_Count += arrExportData[intLoop, 3].Equals("") ? 0 : Convert.ToInt32(arrExportData[intLoop, 3]);
                //        sumReceive_Total_AMT += arrExportData[intLoop, 4].Equals("") ? 0 : Convert.ToDecimal(arrExportData[intLoop, 4]);
                //        sumAdjust_Count += arrExportData[intLoop, 5].Equals("") ? 0 : Convert.ToInt32(arrExportData[intLoop, 5]);
                //        sumAdjust_AMT += arrExportData[intLoop, 6].Equals("") ? 0 : Convert.ToDecimal(arrExportData[intLoop, 6]);
                //        sumKeyin_Reject_Count_All += arrExportData[intLoop, 7].Equals("") ? 0 : Convert.ToInt32(arrExportData[intLoop, 7]);
                //        sumKeyin_Reject_AMT_All += arrExportData[intLoop, 8].Equals("") ? 0 : Convert.ToDecimal(arrExportData[intLoop, 8]);
                //        iDataIndex++;             
                //    }
                //    intRowIndexInSheet = (i * iPageRow) + intRowIndexStart;
                //    intRowIndexEnd = intRowIndexInSheet + iPageRow - 1;
                //    range = sheet.get_Range("A" + intRowIndexInSheet.ToString(), "C" + intRowIndexEnd.ToString());
                //    range.NumberFormat = "@";
                //    range = sheet.get_Range("A" + intRowIndexInSheet.ToString(), "K" + intRowIndexEnd.ToString());
                //    range.Value2 = arrExportData;
                //    range.Value2 = range.Value2;
                //}
                ////欄位格式及框線設定
                //range = sheet.get_Range("A1", "K" + intRowIndexEnd.ToString());
                //range.Font.Size = 10;
                //range.Font.Name = "Arail";
                //range.Columns.AutoFit();
                //range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中
                //range.Borders.LineStyle = 1;

                ////表尾總計
                //int sumReceive_Total_Count_ALL = dt.Compute("SUM(Receive_Total_Count)", "") == DBNull.Value ? 0 : Convert.ToInt32(dt.Compute("SUM(Receive_Total_Count)", ""));
                //decimal sumReceive_Total_AMT_ALL = dt.Compute("SUM(Receive_Total_AMT)", "") == DBNull.Value ? 0 : Convert.ToDecimal(dt.Compute("SUM(Receive_Total_AMT)", ""));
                //int sumAdjust_Count_ALL = dt.Compute("SUM(Adjust_Count)", "") == DBNull.Value ? 0 : Convert.ToInt32(dt.Compute("SUM(Adjust_Count)", ""));
                //decimal sumAdjust_AMT_ALL = dt.Compute("SUM(Adjust_AMT)", "") == DBNull.Value ? 0 : Convert.ToDecimal(dt.Compute("SUM(Adjust_AMT)", ""));
                //int sumKeyin_Reject_Count_All_ALL = dt.Compute("SUM(Keyin_Reject_Count_All)", "") == DBNull.Value ? 0 : Convert.ToInt32(dt.Compute("SUM(Keyin_Reject_Count_All)", ""));
                //decimal sumKeyin_Reject_AMT_All_ALL = dt.Compute("SUM(Keyin_Reject_AMT_All)", "") == DBNull.Value ? 0 : Convert.ToDecimal(dt.Compute("SUM(Keyin_Reject_AMT_All)", ""));
                ////20141008  淨請款筆數 及 淨請款金額 另外計算
                //int sumNet_Count = dt.Compute("SUM(Net_Count)", "") == DBNull.Value ? 0 : Convert.ToInt32(dt.Compute("SUM(Net_Count)", ""));
                //decimal sumNet_AMT = dt.Compute("SUM(Net_AMT)", "") == DBNull.Value ? 0 : Convert.ToDecimal(dt.Compute("SUM(Net_AMT)", ""));
                //intRowIndexEnd++;
                //range = sheet.get_Range("D" + intRowIndexEnd.ToString(), Missing.Value);
                //range.Value2 = sumReceive_Total_Count_ALL;
                //range = sheet.get_Range("E" + intRowIndexEnd.ToString(), Missing.Value);
                //range.Value2 = sumReceive_Total_AMT_ALL;
                //range = sheet.get_Range("F" + intRowIndexEnd.ToString(), Missing.Value);
                //range.Value2 = sumAdjust_Count_ALL;
                //range = sheet.get_Range("G" + intRowIndexEnd.ToString(), Missing.Value);
                //range.Value2 = sumAdjust_AMT_ALL;
                //range = sheet.get_Range("H" + intRowIndexEnd.ToString(), Missing.Value);
                //range.Value2 = sumKeyin_Reject_Count_All_ALL;
                //range = sheet.get_Range("I" + intRowIndexEnd.ToString(), Missing.Value);
                //range.Value2 = sumKeyin_Reject_AMT_All_ALL;
                //intRowIndexEnd += 2;
                //range = sheet.get_Range("D" + intRowIndexEnd.ToString(), Missing.Value);
                //range.HorizontalAlignment = XlHAlign.xlHAlignRight;
                //range.Value2 = "淨請款筆數 :";
                //range = sheet.get_Range("E" + intRowIndexEnd.ToString(), Missing.Value);
                ////range.Value2 = sumReceive_Total_Count_ALL + sumAdjust_Count_ALL - sumKeyin_Reject_Count_All_ALL;
                //range.Value2 = sumNet_Count - sumKeyin_Reject_Count_All_ALL;
                //intRowIndexEnd++;
                //range = sheet.get_Range("D" + intRowIndexEnd.ToString(), Missing.Value);
                //range.HorizontalAlignment = XlHAlign.xlHAlignRight;
                //range.Value2 = "淨請款金額 :";
                //range = sheet.get_Range("E" + intRowIndexEnd.ToString(), Missing.Value);
                ////range.Value2 = sumReceive_Total_AMT_ALL + sumAdjust_AMT_ALL - sumKeyin_Reject_AMT_All_ALL;
                //range.Value2 = sumNet_AMT - sumKeyin_Reject_AMT_All_ALL;

                ////設定數字格式
                //range = sheet.get_Range("D3", "I" + intRowIndexEnd.ToString());
                //range.NumberFormatLocal = "#,##0_ ;[紅色]-#,##0";

                //#endregion 寫入資料

                ////* 保存文件到程序运行目录下
                //string strFileName = string.Empty;
                ////西元年轉民國年 yyyMMdd 共7碼，不足左邊補0
                //string strBatch_Date_TW = BRASExport.ToTWDateString(DateTime.Parse(strBatch_Date));
                //if (strSign_Type == "1")
                //{
                //    //* 一般簽單高收檔匯出檔案的檔名logic 為編批日期YYYMMDD(民國年)+收件批次+”A4”.xls
                //    strFileName = strBatch_Date_TW + strReceive_Batch + "A4";
                //}
                //else
                //{
                //    //* 分期簽單高收檔匯出檔案的檔名logic 為”P”+編批日期YYYMMDD(民國年)+收件批次+”A4”.xls
                //    strFileName = "P" + strBatch_Date_TW + strReceive_Batch + "A4";
                //}
                //strPathFile = strPathFile + @"\" + strFileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                //sheet.SaveAs(strPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                //excel.ActiveWorkbook.Close(false, null, null); //* 关闭 Excel 文件且不保存
                //strMsgID = "01_01070100_005";
                //return true;
                //# endregion 匯入文檔結束
                #endregion 舊CODE

            }
            catch (Exception ex)
            {
                strMsgID = "01_01070100_004";
                Logging.Log(ex, LogLayer.BusinessRule);
                throw ex;
            }
            finally
            {
                //解決"格式太舊或是類型程式庫無效。 (Exception from HRESULT: 0x80028018 (TYPE_E_INVDATAREAD))"問題用
                System.Threading.Thread.CurrentThread.CurrentCulture = oldCI;
            }
        }

        /// <summary>
        /// 產生一般簽單高收檔 或 分期簽單高收檔時，查詢數據
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strSign_Type">簽單類別(1:一般；2:分期)</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>成功時：返回查詢結果；失敗時：null</returns>
        public static DataTable getData_ExportAS_ACQ(string strBatch_Date, string strReceive_Batch, string strSign_Type, ref string strMsgID)
        {
            try
            {
                #region 依據Request查詢資料庫

                //* 聲明SQL Command變量
                SqlCommand sqlcmSearchData = new SqlCommand();
                sqlcmSearchData.CommandType = CommandType.Text;
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                sqlcmSearchData.CommandText = string.Format(SEARCH_ExportAS_ACQ, UtilHelper.GetAppSettings("DB_KeyinGUI"));
                //* 編批日期
                SqlParameter parmBatchDate = new SqlParameter("@Batch_Date", strBatch_Date.Replace("/", ""));
                sqlcmSearchData.Parameters.Add(parmBatchDate);
                //* 收件批次
                SqlParameter parmReceiveBatch = new SqlParameter("@Receive_Batch", strReceive_Batch);
                sqlcmSearchData.Parameters.Add(parmReceiveBatch);
                //* 簽單類別(1:一般；2:分期)
                SqlParameter parmSignType = new SqlParameter("@Sign_Type", strSign_Type);
                sqlcmSearchData.Parameters.Add(parmSignType);

                //* 查詢數據
                DataSet dstSearchData = BRASExport.SearchOnDataSet(sqlcmSearchData);
                if (null == dstSearchData)  //* 查詢數據失敗
                {
                    strMsgID = "01_01070100_001";
                    return null;
                }
                else
                {
                    //* 查詢的數據不存在時
                    if (dstSearchData.Tables[0].Rows.Count == 0)
                    {
                        strMsgID = "01_01070100_002";
                        return null;
                    }
                }
                //* 查詢成功
                strMsgID = "01_01070100_003";
                return dstSearchData.Tables[0];

                #endregion 依據Request查詢資料庫
            }
            catch (Exception exp)
            {
                BRCompareRpt.SaveLog(exp);
                strMsgID = "01_01070100_001";
                return null;
            }
        }
        #endregion 產生一般簽單高收檔 或 分期簽單高收檔

        #region 更新[人工簽單批次資料檔] [處理註記]
        /// <summary>
        /// 更新[人工簽單批次資料檔] [處理註記]
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strSign_Type">簽單類別</param>
        /// <param name="strProcess_Flag">處理註記</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>true:更新成功；false:更新失敗</returns>
        public static bool Update_ASBD_Process_Flag(string strBatch_Date, string strReceive_Batch, string strSign_Type, string strProcess_Flag, ref string strMsgID)
        {
            try
            {
                //* 聲明SQL Command變量
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                sqlcmd.CommandText = string.Format(UPDATE_ASBD_Process_Flag, UtilHelper.GetAppSettings("DB_KeyinGUI"));
                //* 編批日期
                SqlParameter parmBatchDate = new SqlParameter("@Batch_Date", strBatch_Date.Replace("/", ""));
                sqlcmd.Parameters.Add(parmBatchDate);
                //* 收件批次
                SqlParameter parmReceiveBatch = new SqlParameter("@Receive_Batch", strReceive_Batch);
                sqlcmd.Parameters.Add(parmReceiveBatch);
                //* 簽單類別
                SqlParameter parmSignType = new SqlParameter("@Sign_Type", strSign_Type);
                sqlcmd.Parameters.Add(parmSignType);
                //* 處理註記
                SqlParameter parmProcessFlag = new SqlParameter("@Process_Flag", strProcess_Flag);
                sqlcmd.Parameters.Add(parmProcessFlag);

                //更新資料
                if (BRASExport.Update(sqlcmd))
                {
                    strMsgID = "01_01070200_009";
                    return true;
                }
                else
                {
                    strMsgID = "01_01070200_008";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRASErrKeyin.SaveLog(exp);
                strMsgID = "01_01070200_008";
                return false;
            }
        }
        #endregion 產生分期簽單高收檔

        #region 產生上傳檔案(一般簽單及分期簽單)
        /// <summary>
        /// 產生上傳檔案(一般簽單及分期簽單)
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strSign_Type">簽單類別</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <param name="strPathFile">服務器端生成的TXT文檔路徑</param>
        public static bool ExportUploadFile(string strBatch_Date, string strReceive_Batch, string strSign_Type, ref string strMsgID, ref string strPathFile)
        {
            try
            {
                StringBuilder sbCont = new StringBuilder();
                DataSet ds = getData_UploadFile(strBatch_Date, strReceive_Batch, strSign_Type, ref strMsgID);
                if (ds == null)
                    return false;

                DataTable dtRow2 = ds.Tables["Row2Data"];
                DataTable dtRow3 = ds.Tables["Row3Data"];

                //* 檢查目錄，并刪除以前的文檔資料
                BRExcel_File.CheckDirectory(ref strPathFile);

                if (strSign_Type == "1")
                {
                    #region 一般簽單
                    //20140922 第一碼固定帶"0"，2-7碼日期請帶系統日(產檔當日)，格式：yyMMdd
                    //西元年轉民國年 yyyMMdd 共7碼，不足左邊補0
                    //string strBatch_Date_TW = BRASExport.ToTWDateString(DateTime.Parse(strBatch_Date));
                    string strBatch_Date_TW = BRASExport.ToTWDateString(DateTime.Now).Substring(1,6);

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
                    //strPathFile = strPathFile + @"\mupl_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                    strPathFile = strPathFile + @"\mpul_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
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
                    strPathFile = strPathFile + @"\ipp" + strReceive_Batch + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                    #endregion 分期簽單
                }
                FileTools.Create(strPathFile, sbCont.ToString().Remove(sbCont.ToString().LastIndexOf("\r\n")));
                strMsgID = "01_01070200_005";
                return true;
            }
            catch (Exception ex)
            {
                strMsgID = "01_01070200_004";
                Logging.Log(ex, LogLayer.BusinessRule);
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
                    //2021/03/09_Ares_Stanley-DB名稱改為變數
                    sqlcmSearchData.CommandText = string.Format(SEARCH_AS_UploadData, UtilHelper.GetAppSettings("DB_KeyinGUI"));
                }
                else
                {
                    //分期簽單
                    //2021/03/09_Ares_Stanley-DB名稱改為變數
                    sqlcmSearchData.CommandText = string.Format(SEARCH_AS_UploadData_inst, UtilHelper.GetAppSettings("DB_KeyinGUI"));
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
        /// 上傳檔案(一般簽單及分期簽單)資料檢核
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strSign_Type">簽單類別</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>true：通過檢核；false：檢核不通過</returns>
        public static bool CheckUploadData(string strBatch_Date, string strReceive_Batch, string strSign_Type, ref string strMsgID)
        {
            try
            {
                #region 依據Request查詢資料庫

                //* 聲明SQL Command變量
                SqlCommand sqlcmSearchData = new SqlCommand();
                sqlcmSearchData.CommandType = CommandType.Text;
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                sqlcmSearchData.CommandText = string.Format(CHECK_AS_UploadData, UtilHelper.GetAppSettings("DB_KeyinGUI"));
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
                    strMsgID = "01_01070200_006";
                    return false;
                }
                else
                {
                    //* 查詢的數據不存在表示通過檢查回傳TRUE
                    if (dstSearchData.Tables[0].Rows.Count == 0)
                    {
                        strMsgID = "01_01070200_002";
                        return true;
                    }
                }
                //* 查詢成功，有數據表示檢核失敗傳回FALSE及訊息
                strMsgID = "01_01070200_007";
                return false;

                #endregion 依據Request查詢資料庫
            }
            catch (Exception exp)
            {
                BRCompareRpt.SaveLog(exp);
                strMsgID = "01_01070200_006";
                return false;
            }
        }
        #endregion 產生上傳檔案(一般簽單及分期簽單)

        #region 重覆件報表
        /// <summary>
        /// 重覆件報表 查詢
        /// </summary>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>成功時：返回查詢結果；失敗時：null</returns>
        public static DataTable SearchRepeatReport(ref string strMsgID, int intPageIndex, int intPageSize, ref int intTotolCount)
        {
            try
            {
                #region 依據Request查詢資料庫

                //* 由屬性表取出 [一定匯檔日數]
                DataTable dtblProperty = null;
                if(!CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("01", "AS_COMPAREDAY", ref dtblProperty))
                {
                    strMsgID = "01_01070300_006";
                    return null;
                }

                //* 聲明SQL Command變量
                SqlCommand sqlcmSearchData = new SqlCommand();
                sqlcmSearchData.CommandType = CommandType.Text;
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                sqlcmSearchData.CommandText = string.Format(SEARCH_RepeatReport, dtblProperty.Select("PROPERTY_CODE = 'General'")[0]["PROPERTY_NAME"]
                                                                            , dtblProperty.Select("PROPERTY_CODE = 'Installment'")[0]["PROPERTY_NAME"], UtilHelper.GetAppSettings("DB_KeyinGUI"));

                //* 查詢數據
                DataSet dstSearchData = BRASExport.SearchOnDataSet(sqlcmSearchData, intPageIndex, intPageSize, ref intTotolCount);
                if (null == dstSearchData)  //* 查詢數據失敗
                {
                    strMsgID = "01_01070300_001";
                    return null;
                }
                else
                {
                    //* 查詢的數據不存在時
                    if (dstSearchData.Tables[0].Rows.Count == 0)
                    {
                        strMsgID = "01_01070300_002";
                        return null;
                    }
                }
                //* 查詢成功
                strMsgID = "01_01070300_003";
                return dstSearchData.Tables[0];

                #endregion 依據Request查詢資料庫
            }
            catch (Exception exp)
            {
                BRCompareRpt.SaveLog(exp);
                strMsgID = "01_01070300_001";
                return null;
            }
        }

        /// <summary>
        /// 產生重覆件報表Excel
        /// </summary>
        /// <param name="strPathFile">服務器端生成的Excel文檔路徑</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>Excel生成成功標示：True--成功；False--失敗</returns>
        /// 修改紀錄:2021/04/01_Ares_Stanley-註解未使用程式碼
        #region 舊版ExportRepeatReport
        //public static bool ExportRepeatReport(ref string strPathFile, ref string strMsgID)
        //{
        //    //解決"格式太舊或是類型程式庫無效。 (Exception from HRESULT: 0x80028018 (TYPE_E_INVDATAREAD))"問題用
        //    System.Globalization.CultureInfo oldCI = System.Threading.Thread.CurrentThread.CurrentCulture;
        //    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        //    strMsgID = "";
        //    //*创建一个 Excel 实例
        //    ExcelApplication excel = new ExcelApplication();
        //    Workbook workbook = null;
        //    Worksheet sheet = null;
        //    Range range = null;
        //    try
        //    {
        //        //* 檢查目錄，并刪除以前的文檔資料
        //        BRExcel_File.CheckDirectory(ref strPathFile);

        //        //* 取要下載的資料
        //        DataTable dt = BRASExport.getData_ExportRepeatReport(ref strMsgID);
        //        if (dt == null)
        //            return false;

        //        #region 匯入Excel文檔

        //        string[,] arrExportData = null;
        //        excel.Visible = false;//* 不显示 Excel 文件,如果为 true 则显示 Excel 文件
        //        workbook = excel.Workbooks.Add(Missing.Value);//* 添加工作簿
        //        excel.Application.DisplayAlerts = false;
        //        int iDataRows = dt.Rows.Count;              //資料總筆數
        //        int intRowIndexInSheet = 0;
        //        arrExportData = new string[iDataRows, 16];  

        //        #region 寫入資料
        //        for (int intLoop = 0; intLoop < dt.Rows.Count; intLoop++)
        //        {
        //            //* 添加新的Sheet
        //            if (intLoop == 0)
        //            {
        //                sheet = (Worksheet)workbook.Sheets.Add(workbook.Sheets[1], Missing.Value, Missing.Value, Missing.Value);

        //                //*表頭
        //                range = sheet.get_Range("A1", "P1");
        //                range.Value2 = "重覆件報表";
        //                range.Merge(Missing.Value);//*合并单元格

        //                //* DataTable Title
        //                range = sheet.get_Range("A2", Missing.Value);
        //                range.Value2 = "最後重覆日";

        //                range = sheet.get_Range("B2", Missing.Value);
        //                range.Value2 = "編批日";

        //                range = sheet.get_Range("C2", Missing.Value);
        //                range.Value2 = "批號";
        //                range.Borders.LineStyle = 1;

        //                range = sheet.get_Range("D2", Missing.Value);
        //                range.Value2 = "收件批次";

        //                range = sheet.get_Range("E2", Missing.Value);
        //                range.Value2 = "商店代號";

        //                range = sheet.get_Range("F2", Missing.Value);
        //                range.Value2 = "簽單類別";

        //                range = sheet.get_Range("G2", Missing.Value);
        //                range.Value2 = "處理註記";

        //                range = sheet.get_Range("H2", Missing.Value);
        //                range.Value2 = "序號";

        //                range = sheet.get_Range("I2", Missing.Value);
        //                range.Value2 = "交易卡號";

        //                range = sheet.get_Range("J2", Missing.Value);
        //                range.Value2 = "交易日期";

        //                range = sheet.get_Range("K2", Missing.Value);
        //                range.Value2 = "分期期數";

        //                range = sheet.get_Range("L2", Missing.Value);
        //                range.Value2 = "授權號碼";

        //                range = sheet.get_Range("M2", Missing.Value);
        //                range.Value2 = "金額/分期總價";

        //                range = sheet.get_Range("N2", Missing.Value);
        //                range.Value2 = "請退款";

        //                range = sheet.get_Range("O2", Missing.Value);
        //                range.Value2 = "一次鍵檔者";

        //                range = sheet.get_Range("P2", Missing.Value);
        //                range.Value2 = "二次鍵檔者";

        //                intRowIndexInSheet = 2;
        //            }
        //            intRowIndexInSheet++;
        //            //* 最後重覆日
        //            arrExportData[intLoop, 0] = DateTime.ParseExact(dt.Rows[intLoop]["MaxRepeatDay"].ToString(), "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces).ToString("yyyy/MM/dd");

        //            //* 匯檔日(編批日期)
        //            arrExportData[intLoop, 1] = DateTime.ParseExact(dt.Rows[intLoop]["Batch_Date"].ToString(), "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces).ToString("yyyy/MM/dd");

        //            //* 批號
        //            arrExportData[intLoop, 2] = dt.Rows[intLoop]["Batch_NO"].ToString();

        //            //* 收件批次
        //            arrExportData[intLoop, 3] = dt.Rows[intLoop]["Receive_Batch"].ToString();

        //            //* 商店代號
        //            arrExportData[intLoop, 4] = dt.Rows[intLoop]["Shop_ID"].ToString();

        //            //* 簽單類別
        //            arrExportData[intLoop, 5] = dt.Rows[intLoop]["Sign_Type"].ToString();

        //            //* 處理註記
        //            arrExportData[intLoop, 6] = dt.Rows[intLoop]["Process_Flag"].ToString();

        //            //* 序號
        //            arrExportData[intLoop, 7] = dt.Rows[intLoop]["SN"].ToString();

        //            //* 交易卡號
        //            arrExportData[intLoop, 8] = BRASExport.strHidden(dt.Rows[intLoop]["Card_No"].ToString(),6,6);

        //            //* 交易日期
        //            arrExportData[intLoop, 9] = DateTime.ParseExact(dt.Rows[intLoop]["Tran_Date"].ToString(), "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces).ToString("yyyy/MM/dd");

        //            //* 分期期數
        //            arrExportData[intLoop, 10] = dt.Rows[intLoop]["Installment_Periods"].ToString();

        //            //* 授權號碼
        //            arrExportData[intLoop, 11] = dt.Rows[intLoop]["Auth_Code"].ToString();

        //            //* 金額/分期總價
        //            arrExportData[intLoop, 12] = Convert.ToDecimal(dt.Rows[intLoop]["AMT"]).ToString("N0");

        //            //* 請退款
        //            arrExportData[intLoop, 13] = dt.Rows[intLoop]["Receipt_Type"].ToString();

        //            //* 一次鍵檔者
        //            arrExportData[intLoop, 14] = dt.Rows[intLoop]["1Key_user"].ToString();

        //            //* 二次鍵檔者
        //            arrExportData[intLoop, 15] = dt.Rows[intLoop]["2Key_user"].ToString();
        //        }
        //        range = sheet.get_Range("A3", "P" + intRowIndexInSheet.ToString());
        //        range.NumberFormat = "@";
        //        range = sheet.get_Range("A3", "P" + intRowIndexInSheet.ToString());
        //        range.Value2 = arrExportData;
        //        range.Value2 = range.Value2;

        //        //欄位格式及框線設定
        //        range = sheet.get_Range("A1", "P" + intRowIndexInSheet.ToString());
        //        range.Font.Size = 10;
        //        range.Font.Name = "Arail";
        //        range.Columns.AutoFit();
        //        range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//* 设置单元格水平居中
        //        range.Borders.LineStyle = 1;

        //        #endregion 寫入資料

        //        //* 保存文件到程序运行目录下
        //        strPathFile = strPathFile + @"\RepeatReport_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
        //        sheet.SaveAs(strPathFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
        //        excel.ActiveWorkbook.Close(false, null, null); //* 关闭 Excel 文件且不保存
        //        strMsgID = "01_01070300_005";
        //        return true;
        //        # endregion 匯入文檔結束
        //    }
        //    catch (Exception ex)
        //    {
        //        strMsgID = "01_01070300_004";
        //        Logging.Log(ex, LogLayer.BusinessRule);
        //        throw ex;
        //    }
        //    finally
        //    {
        //        range = null;
        //        sheet = null;
        //        workbook = null;
        //        excel.Quit(); //* 退出 Excel
        //        excel = null; //* 将 Excel 实例设置为空

        //        //解決"格式太舊或是類型程式庫無效。 (Exception from HRESULT: 0x80028018 (TYPE_E_INVDATAREAD))"問題用
        //        System.Threading.Thread.CurrentThread.CurrentCulture = oldCI;
        //    }
        //}
        #endregion 舊版ExportRepeatReport

        /// <summary>
        /// 產生重覆件報表Excel時，查詢數據
        /// </summary>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>成功時：返回查詢結果；失敗時：null</returns>
        public static DataTable getData_ExportRepeatReport(ref string strMsgID)
        {
            try
            {
                #region 依據Request查詢資料庫

                //* 由屬性表取出 [一定匯檔日數]
                DataTable dtblProperty = null;
                if (!CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty("01", "AS_COMPAREDAY", ref dtblProperty))
                {
                    strMsgID = "01_01070300_006";
                    return null;
                }

                //* 聲明SQL Command變量
                SqlCommand sqlcmSearchData = new SqlCommand();
                sqlcmSearchData.CommandType = CommandType.Text;
                sqlcmSearchData.CommandText = string.Format(SEARCH_RepeatReport, dtblProperty.Select("PROPERTY_CODE = 'General'")[0]["PROPERTY_NAME"]
                                                                            , dtblProperty.Select("PROPERTY_CODE = 'Installment'")[0]["PROPERTY_NAME"]);

                //* 查詢數據
                DataSet dstSearchData = BRASExport.SearchOnDataSet(sqlcmSearchData);
                if (null == dstSearchData)  //* 查詢數據失敗
                {
                    strMsgID = "01_01070300_001";
                    return null;
                }
                else
                {
                    //* 查詢的數據不存在時
                    if (dstSearchData.Tables[0].Rows.Count == 0)
                    {
                        strMsgID = "01_01070300_002";
                        return null;
                    }
                }
                //* 查詢成功
                strMsgID = "01_01070300_003";
                return dstSearchData.Tables[0];

                #endregion 依據Request查詢資料庫
            }
            catch (Exception exp)
            {
                BRCompareRpt.SaveLog(exp);
                strMsgID = "01_01070300_001";
                return null;
            }
        }
        #endregion 重覆件報表

        #region Functuon
        /// <summary>
        /// 西元日期轉民國日期(yyyMMdd) 7碼
        /// </summary>
        /// <param name="SourceDateTime">需轉換的日期</param>
        /// <returns>民國年日期(yyyMMdd)</returns>
        public static string ToTWDateString(DateTime SourceDateTime)
        {
            try
            {
                TaiwanCalendar cal = new TaiwanCalendar();
                string strTWDate = cal.GetYear(SourceDateTime).ToString("000") + cal.GetMonth(SourceDateTime).ToString("00") + cal.GetDayOfMonth(SourceDateTime).ToString("00");
                return strTWDate;
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
                throw ex;
            }
        }

        /// <summary>
        /// 字串隱碼
        /// </summary>
        /// <param name="strInput">要處理的字串資料</param>
        /// <param name="front">前面顯示字數</param>
        /// <param name="rear">後面顯示字數</param>
        /// <returns></returns>
        public static string strHidden(string strInput, int front, int rear)
        {
            try
            {
                if (front > strInput.Length || rear > strInput.Length || front + rear > strInput.Length)
                {
                    throw new ArgumentException("隱碼參數超出輸入字串大小！");
                }
                strInput = strInput.Trim();
                return strInput.Substring(0, front).PadRight(strInput.Length - rear, 'X') + strInput.Substring(strInput.Length - rear, rear);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion Functuon
    }
}
