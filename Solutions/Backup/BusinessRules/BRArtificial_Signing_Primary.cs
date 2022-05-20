//******************************************************************
//*  作    者：
//*  功能說明：Artificial_Signing_Primary資料庫信息
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
using System.Web.UI;
using Framework.Common.Message;


namespace CSIPKeyInGUI.BusinessRules
{
    public class BRArtificial_Signing_Primary : CSIPCommonModel.BusinessRules.BRBase<EntityArtificial_Signing_Primary>
    {
        #region SQL
        //主檔資料查詢
        public const string SEL_AS_Primary = @"Select * from Artificial_Signing_Primary 
                Where Batch_Date = @Batch_Date and Receive_Batch = @Receive_Batch and Batch_NO = @Batch_NO and Shop_ID = @Shop_ID 
                    and Sign_Type = @Sign_Type and KeyIn_Flag = @KeyIn_Flag";

        //新增主檔資料
        public const string INSERT_AS_Primary = @"INSERT INTO [KeyinGUI].[dbo].[Artificial_Signing_Primary] 
        ([Batch_Date], [Shop_ID], [Batch_NO], [Receive_Batch], [Sign_Type], [KeyIn_Flag], [Create_User], [Create_Datetime],[Modify_User],[Modify_DateTime])
        VALUES (@Batch_Date, @Shop_ID, @Batch_NO, @Receive_Batch, @Sign_Type, @KeyIn_Flag, @Create_User, GETDATE(),@Create_User, GETDATE()) ";

        //更新第一次平帳資料
        public const string UPDATE_ASP_First_Balance = @" UPDATE [KeyinGUI].[dbo].[Artificial_Signing_Primary]
            SET Keyin_Success_Count_All = @Keyin_Success_Count_All, Keyin_Success_AMT_All = @Keyin_Success_AMT_All, Keyin_Success_Count_40 = @Keyin_Success_Count_40, 
                Keyin_Success_AMT_40 = @Keyin_Success_AMT_40, Keyin_Success_Count_41 = @Keyin_Success_Count_41, Keyin_Success_AMT_41 = @Keyin_Success_AMT_41, 
                Keyin_Reject_Count_All = @Keyin_Reject_Count_All, Keyin_Reject_AMT_All = @Keyin_Reject_AMT_All, Keyin_Reject_Count_40 = @Keyin_Reject_Count_40, 
                Keyin_Reject_AMT_40 = @Keyin_Reject_AMT_40, Keyin_Reject_Count_41 = @Keyin_Reject_Count_41, Keyin_Reject_AMT_41 = @Keyin_Reject_AMT_41, 
                First_Balance_Flag = @First_Balance_Flag, First_Balance_Count = @First_Balance_Count, First_Balance_AMT = @First_Balance_AMT, 
                Modify_User = @Modify_User, Modify_DateTime = GETDATE()
            WHERE Batch_Date = @Batch_Date
	            AND Shop_ID = @Shop_ID
	            AND Batch_NO = @Batch_NO
	            AND Receive_Batch = @Receive_Batch
	            AND Sign_Type = @Sign_Type
	            AND KeyIn_Flag = @KeyIn_Flag ";

        //更新第一次平帳動作
        public const string UPDATE_ASP_First_BalanceFlag = @" UPDATE [KeyinGUI].[dbo].[Artificial_Signing_Primary]
            SET First_Balance_Flag = @First_Balance_Flag, Modify_User = @Modify_User, Modify_DateTime = GETDATE()
            WHERE Batch_Date = @Batch_Date
	            AND Shop_ID = @Shop_ID
	            AND Batch_NO = @Batch_NO
	            AND Receive_Batch = @Receive_Batch
	            AND Sign_Type = @Sign_Type
	            AND KeyIn_Flag = @KeyIn_Flag ";

        //更新第二次平帳動作
        public const string UPDATE_ASP_Second_Balance = @" UPDATE [KeyinGUI].[dbo].[Artificial_Signing_Primary]
            SET Second_Balance_Flag = @Second_Balance_Flag, Modify_User = @Modify_User, Modify_DateTime = GETDATE()
            WHERE Batch_Date = @Batch_Date
	            AND Shop_ID = @Shop_ID
	            AND Batch_NO = @Batch_NO
	            AND Receive_Batch = @Receive_Batch
	            AND Sign_Type = @Sign_Type
	            AND KeyIn_Flag = @KeyIn_Flag ";

        //更新平帳資料
        public const string UPDATE_ASP_Balance = @" UPDATE [KeyinGUI].[dbo].[Artificial_Signing_Primary]
            SET Keyin_Success_Count_All = @Keyin_Success_Count_All, Keyin_Success_AMT_All = @Keyin_Success_AMT_All, Keyin_Success_Count_40 = @Keyin_Success_Count_40, 
                Keyin_Success_AMT_40 = @Keyin_Success_AMT_40, Keyin_Success_Count_41 = @Keyin_Success_Count_41, Keyin_Success_AMT_41 = @Keyin_Success_AMT_41, 
                Keyin_Reject_Count_All = @Keyin_Reject_Count_All, Keyin_Reject_AMT_All = @Keyin_Reject_AMT_All, Keyin_Reject_Count_40 = @Keyin_Reject_Count_40, 
                Keyin_Reject_AMT_40 = @Keyin_Reject_AMT_40, Keyin_Reject_Count_41 = @Keyin_Reject_Count_41, Keyin_Reject_AMT_41 = @Keyin_Reject_AMT_41, 
                First_Balance_Flag = @First_Balance_Flag, First_Balance_Count = @First_Balance_Count, First_Balance_AMT = @First_Balance_AMT, 
                Adjust_Count = @Adjust_Count, Adjust_AMT = @Adjust_AMT, Second_Balance_Count = @Second_Balance_Count, Second_Balance_AMT = @Second_Balance_AMT, 
                Second_Balance_Flag = @Second_Balance_Flag, Balance_Flag = @Balance_Flag, Modify_User = @Modify_User, Modify_DateTime = GETDATE()
            WHERE Batch_Date = @Batch_Date
	            AND Shop_ID = @Shop_ID
	            AND Batch_NO = @Batch_NO
	            AND Receive_Batch = @Receive_Batch
	            AND Sign_Type = @Sign_Type
	            AND KeyIn_Flag = @KeyIn_Flag ";

        //更新比對註記
        public const string UPDATE_ASP_KeyIn_MatchFlag = @" UPDATE [KeyinGUI].[dbo].[Artificial_Signing_Primary]
            SET KeyIn_MatchFlag = @KeyIn_MatchFlag, Modify_User = @Modify_User, Modify_DateTime = GETDATE()
            WHERE Batch_Date = @Batch_Date
	            AND Shop_ID = @Shop_ID
	            AND Batch_NO = @Batch_NO
	            AND Receive_Batch = @Receive_Batch
	            AND Sign_Type = @Sign_Type
	            AND KeyIn_Flag = @KeyIn_Flag ";
        #endregion

        /// <summary>
        /// 主檔資料查詢
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strBatch_NO">批號</param>
        /// <param name="strShop_ID">商店代號</param>
        /// <param name="strSign_Type">1:一般簽單；2:分期簽單</param>
        /// <returns></returns>        
        public static System.Data.DataSet Select_Primary(string strBatch_Date, string strReceive_Batch, string strBatch_NO, string strShop_ID, string strSign_Type, string strKeyIn_Flag)
        {
            DataSet ds = null;
            SqlHelper sSql = new SqlHelper();

            SqlCommand sqlComm = new SqlCommand();
            try
            {
                sqlComm.CommandText = SEL_AS_Primary;
                sqlComm.CommandType = CommandType.Text;

                SqlParameter parmBatch_Date = new SqlParameter("@Batch_Date", strBatch_Date.Replace("/", ""));
                SqlParameter parmReceive_Batch = new SqlParameter("@Receive_Batch", strReceive_Batch);
                SqlParameter parmBatch_NO = new SqlParameter("@Batch_NO", strBatch_NO);
                SqlParameter parmShop_ID = new SqlParameter("@Shop_ID", strShop_ID);
                SqlParameter parmSign_Type = new SqlParameter("@Sign_Type", strSign_Type);

                sqlComm.Parameters.Add(parmBatch_Date);
                sqlComm.Parameters.Add(parmReceive_Batch);
                sqlComm.Parameters.Add(parmBatch_NO);
                sqlComm.Parameters.Add(parmShop_ID);
                sqlComm.Parameters.Add(parmSign_Type);

                SqlParameter parmKeyIn_Flag = new SqlParameter("@KeyIn_Flag", strKeyIn_Flag);
                sqlComm.Parameters.Add(parmKeyIn_Flag);

                ds = BRArtificial_Signing_Primary.SearchOnDataSet(sqlComm);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        /// <summary>
        /// 新增主檔資料
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strBatch_NO">批號</param>
        /// <param name="strShop_ID">商店代號</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strSign_Type">簽單類別</param>
        /// <param name="strKeyIn_Flag">一、二KEY註記</param>
        /// <param name="strCreate_User"> 新增人員</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>true:寫入成功；false:寫入失敗</returns>
        public static bool Insert_AS_Primary(string strBatch_Date, string strBatch_NO, string strShop_ID, string strReceive_Batch, string strSign_Type,
            string strKeyIn_Flag, string strCreate_User, ref string strMsgID)
        {
            try
            {
                //* 聲明SQL Command變量
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = INSERT_AS_Primary;
                //* 編批日期
                SqlParameter parmBatchDate = new SqlParameter("@Batch_Date", strBatch_Date.Replace("/", ""));
                sqlcmd.Parameters.Add(parmBatchDate);
                //* 批號
                SqlParameter parmBatchNO = new SqlParameter("@Batch_NO", strBatch_NO);
                sqlcmd.Parameters.Add(parmBatchNO);
                //* 商店代號
                SqlParameter parmShopID = new SqlParameter("@Shop_ID", strShop_ID);
                sqlcmd.Parameters.Add(parmShopID);
                //* 收件批次
                SqlParameter parmReceiveBatch = new SqlParameter("@Receive_Batch", strReceive_Batch);
                sqlcmd.Parameters.Add(parmReceiveBatch);
                //* 簽單類別
                SqlParameter parmSignType = new SqlParameter("@Sign_Type", strSign_Type);
                sqlcmd.Parameters.Add(parmSignType);
                //* 一、二KEY註記
                SqlParameter parmKeyIn_Flag = new SqlParameter("@KeyIn_Flag", strKeyIn_Flag);
                sqlcmd.Parameters.Add(parmKeyIn_Flag);
                //* 新增人員
                SqlParameter parmCreate_User = new SqlParameter("@Create_User", strCreate_User);
                sqlcmd.Parameters.Add(parmCreate_User);

                //寫入資料
                if (BRArtificial_Signing_Primary.Add(sqlcmd))
                {
                    strMsgID = "01_01060400_021";
                    return true;
                }
                else
                {
                    strMsgID = "01_01060400_020";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRArtificial_Signing_Primary.SaveLog(exp);
                strMsgID = "01_01060400_020";
                return false;
            }
        }

        /// <summary>
        /// 更新第一次平帳資料
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strBatch_NO">批號</param>
        /// <param name="strShop_ID">商店代號</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strSign_Type">簽單類別</param>
        /// <param name="strKeyIn_Flag">一、二KEY註記</param>
        /// <param name="strModify_User"> 更新人員</param>
        /// <param name="strKeyin_Success_Count_All"> 鍵檔內頁-成功總筆數</param>
        /// <param name="strKeyin_Success_AMT_All"> 鍵檔內頁-成功總金額</param>
        /// <param name="strKeyin_Success_Count_40"> 鍵檔內頁-40(+)總筆數</param>
        /// <param name="strKeyin_Success_AMT_40"> 鍵檔內頁-40(+)總金額</param>
        /// <param name="strKeyin_Success_Count_41"> 鍵檔內頁-41(-)總筆數</param>
        /// <param name="strKeyin_Success_AMT_41"> 鍵檔內頁-41(-)總金額</param>
        /// <param name="strKeyin_Reject_Count_All"> 鍵檔內頁-剔退總筆數</param>
        /// <param name="strKeyin_Reject_AMT_All"> 鍵檔內頁-剔退總金額</param>
        /// <param name="strKeyin_Reject_Count_40"> (剔退)鍵檔內頁-40(+)總筆數</param>
        /// <param name="strKeyin_Reject_AMT_40"> (剔退)鍵檔內頁-40(+)總金額</param>
        /// <param name="strKeyin_Reject_Count_41"> (剔退)鍵檔內頁-41(-)總筆數</param>
        /// <param name="strKeyin_Reject_AMT_41"> (剔退)鍵檔內頁-41(-)總金額</param>
        /// <param name="strFirst_Balance_Flag"> 第一次平帳動作</param>
        /// <param name="strFirst_Balance_Count"> 第一次平帳差異筆數(收件-鍵檔)</param>
        /// <param name="strFirst_Balance_AMT"> 第一次平帳差異金額(收件-鍵檔)</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>true:更新成功；false:更新失敗</returns>
        public static bool Update_ASP_First_Balance(string strBatch_Date, string strBatch_NO, string strShop_ID, string strReceive_Batch, string strSign_Type,
            string strKeyIn_Flag, string strModify_User, string strKeyin_Success_Count_All, string strKeyin_Success_AMT_All, string strKeyin_Success_Count_40,
            string strKeyin_Success_AMT_40, string strKeyin_Success_Count_41, string strKeyin_Success_AMT_41, string strKeyin_Reject_Count_All, string strKeyin_Reject_AMT_All,
            string strKeyin_Reject_Count_40, string strKeyin_Reject_AMT_40, string strKeyin_Reject_Count_41, string strKeyin_Reject_AMT_41, string strFirst_Balance_Flag,
            string strFirst_Balance_Count, string strFirst_Balance_AMT, ref string strMsgID)
        {
            try
            {
                //* 聲明SQL Command變量
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = UPDATE_ASP_First_Balance;
                //* 編批日期
                SqlParameter parmBatchDate = new SqlParameter("@Batch_Date", strBatch_Date.Replace("/", ""));
                sqlcmd.Parameters.Add(parmBatchDate);
                //* 批號
                SqlParameter parmBatchNO = new SqlParameter("@Batch_NO", strBatch_NO);
                sqlcmd.Parameters.Add(parmBatchNO);
                //* 商店代號
                SqlParameter parmShopID = new SqlParameter("@Shop_ID", strShop_ID);
                sqlcmd.Parameters.Add(parmShopID);
                //* 收件批次
                SqlParameter parmReceiveBatch = new SqlParameter("@Receive_Batch", strReceive_Batch);
                sqlcmd.Parameters.Add(parmReceiveBatch);
                //* 簽單類別
                SqlParameter parmSignType = new SqlParameter("@Sign_Type", strSign_Type);
                sqlcmd.Parameters.Add(parmSignType);
                //* 一、二KEY註記
                SqlParameter parmKeyIn_Flag = new SqlParameter("@KeyIn_Flag", strKeyIn_Flag);
                sqlcmd.Parameters.Add(parmKeyIn_Flag);
                //* 更新人員
                SqlParameter parmModify_User = new SqlParameter("@Modify_User", strModify_User);
                sqlcmd.Parameters.Add(parmModify_User);

                //* 鍵檔內頁-成功總筆數
                SqlParameter parmKeyin_Success_Count_All = new SqlParameter("@Keyin_Success_Count_All", strKeyin_Success_Count_All);
                sqlcmd.Parameters.Add(parmKeyin_Success_Count_All);
                //* 鍵檔內頁-成功總金額
                SqlParameter parmKeyin_Success_AMT_All = new SqlParameter("@Keyin_Success_AMT_All", strKeyin_Success_AMT_All);
                sqlcmd.Parameters.Add(parmKeyin_Success_AMT_All);
                //* 鍵檔內頁-40(+)總筆數
                SqlParameter parmKeyin_Success_Count_40 = new SqlParameter("@Keyin_Success_Count_40", strKeyin_Success_Count_40);
                sqlcmd.Parameters.Add(parmKeyin_Success_Count_40);
                //* 鍵檔內頁-40(+)總金額
                SqlParameter parmKeyin_Success_AMT_40 = new SqlParameter("@Keyin_Success_AMT_40", strKeyin_Success_AMT_40);
                sqlcmd.Parameters.Add(parmKeyin_Success_AMT_40);
                //* 鍵檔內頁-41(-)總筆數
                SqlParameter parmKeyin_Success_Count_41 = new SqlParameter("@Keyin_Success_Count_41", strKeyin_Success_Count_41);
                sqlcmd.Parameters.Add(parmKeyin_Success_Count_41);
                //* 鍵檔內頁-41(-)總金額
                SqlParameter parmKeyin_Success_AMT_41 = new SqlParameter("@Keyin_Success_AMT_41", strKeyin_Success_AMT_41);
                sqlcmd.Parameters.Add(parmKeyin_Success_AMT_41);
                //* 鍵檔內頁-剔退總筆數
                SqlParameter parmKeyin_Reject_Count_All = new SqlParameter("@Keyin_Reject_Count_All", strKeyin_Reject_Count_All);
                sqlcmd.Parameters.Add(parmKeyin_Reject_Count_All);
                //* 鍵檔內頁-剔退總金額
                SqlParameter parmKeyin_Reject_AMT_All = new SqlParameter("@Keyin_Reject_AMT_All", strKeyin_Reject_AMT_All);
                sqlcmd.Parameters.Add(parmKeyin_Reject_AMT_All);
                //* (剔退)鍵檔內頁-40(+)總筆數
                SqlParameter parmKeyin_Reject_Count_40 = new SqlParameter("@Keyin_Reject_Count_40", strKeyin_Reject_Count_40);
                sqlcmd.Parameters.Add(parmKeyin_Reject_Count_40);
                //* (剔退)鍵檔內頁-40(+)總金額
                SqlParameter parmKeyin_Reject_AMT_40 = new SqlParameter("@Keyin_Reject_AMT_40", strKeyin_Reject_AMT_40);
                sqlcmd.Parameters.Add(parmKeyin_Reject_AMT_40);
                //* (剔退)鍵檔內頁-41(-)總筆數
                SqlParameter parmKeyin_Reject_Count_41 = new SqlParameter("@Keyin_Reject_Count_41", strKeyin_Reject_Count_41);
                sqlcmd.Parameters.Add(parmKeyin_Reject_Count_41);
                //* (剔退)鍵檔內頁-41(-)總金額
                SqlParameter parmKeyin_Reject_AMT_41 = new SqlParameter("@Keyin_Reject_AMT_41", strKeyin_Reject_AMT_41);
                sqlcmd.Parameters.Add(parmKeyin_Reject_AMT_41);

                //* 第一次平帳動作
                SqlParameter parmFirst_Balance_Flag = new SqlParameter("@First_Balance_Flag", strFirst_Balance_Flag);
                sqlcmd.Parameters.Add(parmFirst_Balance_Flag);
                //* 第一次平帳差異筆數(收件-鍵檔)
                SqlParameter parmFirst_Balance_Count = new SqlParameter("@First_Balance_Count", strFirst_Balance_Count);
                sqlcmd.Parameters.Add(parmFirst_Balance_Count);
                //* 第一次平帳差異金額(收件-鍵檔)
                SqlParameter parmFirst_Balance_AMT = new SqlParameter("@First_Balance_AMT", strFirst_Balance_AMT);
                sqlcmd.Parameters.Add(parmFirst_Balance_AMT);

                //更新資料
                if (BRArtificial_Signing_Primary.Update(sqlcmd))
                {
                    strMsgID = "01_01060400_023";
                    return true;
                }
                else
                {
                    strMsgID = "01_01060400_022";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRArtificial_Signing_Primary.SaveLog(exp);
                strMsgID = "01_01060400_022";
                return false;
            }
        }

        /// <summary>
        /// 更新第一次平帳動作
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strBatch_NO">批號</param>
        /// <param name="strShop_ID">商店代號</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strSign_Type">簽單類別</param>
        /// <param name="strKeyIn_Flag">一、二KEY註記</param>
        /// <param name="strModify_User"> 更新人員</param>
        /// <param name="strFirst_Balance_Flag"> 第一次平帳動作(Y/N)</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>true:更新成功；false:更新失敗</returns>
        public static bool Update_ASP_First_BalanceFlag(string strBatch_Date, string strBatch_NO, string strShop_ID, string strReceive_Batch, string strSign_Type,
            string strKeyIn_Flag, string strModify_User, string strFirst_Balance_Flag, ref string strMsgID)
        {
            try
            {
                //* 聲明SQL Command變量
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = UPDATE_ASP_First_BalanceFlag;
                //* 編批日期
                SqlParameter parmBatchDate = new SqlParameter("@Batch_Date", strBatch_Date.Replace("/", ""));
                sqlcmd.Parameters.Add(parmBatchDate);
                //* 批號
                SqlParameter parmBatchNO = new SqlParameter("@Batch_NO", strBatch_NO);
                sqlcmd.Parameters.Add(parmBatchNO);
                //* 商店代號
                SqlParameter parmShopID = new SqlParameter("@Shop_ID", strShop_ID);
                sqlcmd.Parameters.Add(parmShopID);
                //* 收件批次
                SqlParameter parmReceiveBatch = new SqlParameter("@Receive_Batch", strReceive_Batch);
                sqlcmd.Parameters.Add(parmReceiveBatch);
                //* 簽單類別
                SqlParameter parmSignType = new SqlParameter("@Sign_Type", strSign_Type);
                sqlcmd.Parameters.Add(parmSignType);
                //* 一、二KEY註記
                SqlParameter parmKeyIn_Flag = new SqlParameter("@KeyIn_Flag", strKeyIn_Flag);
                sqlcmd.Parameters.Add(parmKeyIn_Flag);
                //* 更新人員
                SqlParameter parmModify_User = new SqlParameter("@Modify_User", strModify_User);
                sqlcmd.Parameters.Add(parmModify_User);

                //* 第一次平帳動作
                SqlParameter parmFirst_Balance_Flag = new SqlParameter("@First_Balance_Flag", strFirst_Balance_Flag);
                sqlcmd.Parameters.Add(parmFirst_Balance_Flag);

                //更新資料
                if (BRArtificial_Signing_Primary.Update(sqlcmd))
                {
                    strMsgID = "01_01060400_023";
                    return true;
                }
                else
                {
                    strMsgID = "01_01060400_022";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRArtificial_Signing_Primary.SaveLog(exp);
                strMsgID = "01_01060400_022";
                return false;
            }
        }

        /// <summary>
        /// 更新第二次平帳動作
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strBatch_NO">批號</param>
        /// <param name="strShop_ID">商店代號</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strSign_Type">簽單類別</param>
        /// <param name="strKeyIn_Flag">一、二KEY註記</param>
        /// <param name="strModify_User"> 更新人員</param>
        /// <param name="strFirst_Balance_Flag"> 第二次平帳動作(Y/N)</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>true:更新成功；false:更新失敗</returns>
        public static bool Update_ASP_Second_BalanceFlag(string strBatch_Date, string strBatch_NO, string strShop_ID, string strReceive_Batch, string strSign_Type,
            string strKeyIn_Flag, string strModify_User, string strSecond_Balance_Flag, ref string strMsgID)
        {
            try
            {
                //* 聲明SQL Command變量
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = UPDATE_ASP_Second_Balance;
                //* 編批日期
                SqlParameter parmBatchDate = new SqlParameter("@Batch_Date", strBatch_Date.Replace("/", ""));
                sqlcmd.Parameters.Add(parmBatchDate);
                //* 批號
                SqlParameter parmBatchNO = new SqlParameter("@Batch_NO", strBatch_NO);
                sqlcmd.Parameters.Add(parmBatchNO);
                //* 商店代號
                SqlParameter parmShopID = new SqlParameter("@Shop_ID", strShop_ID);
                sqlcmd.Parameters.Add(parmShopID);
                //* 收件批次
                SqlParameter parmReceiveBatch = new SqlParameter("@Receive_Batch", strReceive_Batch);
                sqlcmd.Parameters.Add(parmReceiveBatch);
                //* 簽單類別
                SqlParameter parmSignType = new SqlParameter("@Sign_Type", strSign_Type);
                sqlcmd.Parameters.Add(parmSignType);
                //* 一、二KEY註記
                SqlParameter parmKeyIn_Flag = new SqlParameter("@KeyIn_Flag", strKeyIn_Flag);
                sqlcmd.Parameters.Add(parmKeyIn_Flag);
                //* 更新人員
                SqlParameter parmModify_User = new SqlParameter("@Modify_User", strModify_User);
                sqlcmd.Parameters.Add(parmModify_User);

                //* 第二次平帳動作
                SqlParameter parmSecond_Balance_Flag = new SqlParameter("@Second_Balance_Flag", strSecond_Balance_Flag);
                sqlcmd.Parameters.Add(parmSecond_Balance_Flag);

                //更新資料
                if (BRArtificial_Signing_Primary.Update(sqlcmd))
                {
                    strMsgID = "01_01060400_023";
                    return true;
                }
                else
                {
                    strMsgID = "01_01060400_022";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRArtificial_Signing_Primary.SaveLog(exp);
                strMsgID = "01_01060400_022";
                return false;
            }
        }

        /// <summary>
        /// 更新平帳資料
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strBatch_NO">批號</param>
        /// <param name="strShop_ID">商店代號</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strSign_Type">簽單類別</param>
        /// <param name="strKeyIn_Flag">一、二KEY註記</param>
        /// <param name="strModify_User"> 更新人員</param>
        /// <param name="strKeyin_Success_Count_All"> 鍵檔內頁-成功總筆數</param>
        /// <param name="strKeyin_Success_AMT_All"> 鍵檔內頁-成功總金額</param>
        /// <param name="strKeyin_Success_Count_40"> 鍵檔內頁-40(+)總筆數</param>
        /// <param name="strKeyin_Success_AMT_40"> 鍵檔內頁-40(+)總金額</param>
        /// <param name="strKeyin_Success_Count_41"> 鍵檔內頁-41(-)總筆數</param>
        /// <param name="strKeyin_Success_AMT_41"> 鍵檔內頁-41(-)總金額</param>
        /// <param name="strKeyin_Reject_Count_All"> 鍵檔內頁-剔退總筆數</param>
        /// <param name="strKeyin_Reject_AMT_All"> 鍵檔內頁-剔退總金額</param>
        /// <param name="strKeyin_Reject_Count_40"> (剔退)鍵檔內頁-40(+)總筆數</param>
        /// <param name="strKeyin_Reject_AMT_40"> (剔退)鍵檔內頁-40(+)總金額</param>
        /// <param name="strKeyin_Reject_Count_41"> (剔退)鍵檔內頁-41(-)總筆數</param>
        /// <param name="strKeyin_Reject_AMT_41"> (剔退)鍵檔內頁-41(-)總金額</param>
        /// <param name="strFirst_Balance_Flag"> 第一次平帳動作</param>
        /// <param name="strFirst_Balance_Count"> 第一次平帳差異筆數(收件-鍵檔)</param>
        /// <param name="strFirst_Balance_AMT"> 第一次平帳差異金額(收件-鍵檔)</param>
        /// <param name="strAdjust_Count"> 調整後收件正確總筆數</param>
        /// <param name="strAdjust_AMT"> 調整後收件正確總金額</param>
        /// <param name="strSecond_Balance_Flag"> 第二次平帳動作</param>
        /// <param name="strSecond_Balance_Count"> 第二次平帳差異筆數(收件-鍵檔)</param>
        /// <param name="strSecond_Balance_AMT"> 第二次平帳差異金額(收件-鍵檔)</param>
        /// <param name="strBalance_Flag"> 平帳註記</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>true:更新成功；false:更新失敗</returns>
        public static bool Update_ASP_Balance(string strBatch_Date, string strBatch_NO, string strShop_ID, string strReceive_Batch, string strSign_Type,
            string strKeyIn_Flag, string strModify_User, string strKeyin_Success_Count_All, string strKeyin_Success_AMT_All, string strKeyin_Success_Count_40,
            string strKeyin_Success_AMT_40, string strKeyin_Success_Count_41, string strKeyin_Success_AMT_41, string strKeyin_Reject_Count_All, string strKeyin_Reject_AMT_All,
            string strKeyin_Reject_Count_40, string strKeyin_Reject_AMT_40, string strKeyin_Reject_Count_41, string strKeyin_Reject_AMT_41, string strFirst_Balance_Flag,
            string strFirst_Balance_Count, string strFirst_Balance_AMT, string strAdjust_Count, string strAdjust_AMT, string strSecond_Balance_Flag,
            string strSecond_Balance_Count, string strSecond_Balance_AMT, string strBalance_Flag, ref string strMsgID)
        {
            try
            {
                //* 聲明SQL Command變量
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = UPDATE_ASP_Balance;
                //* 編批日期
                SqlParameter parmBatchDate = new SqlParameter("@Batch_Date", strBatch_Date.Replace("/", ""));
                sqlcmd.Parameters.Add(parmBatchDate);
                //* 批號
                SqlParameter parmBatchNO = new SqlParameter("@Batch_NO", strBatch_NO);
                sqlcmd.Parameters.Add(parmBatchNO);
                //* 商店代號
                SqlParameter parmShopID = new SqlParameter("@Shop_ID", strShop_ID);
                sqlcmd.Parameters.Add(parmShopID);
                //* 收件批次
                SqlParameter parmReceiveBatch = new SqlParameter("@Receive_Batch", strReceive_Batch);
                sqlcmd.Parameters.Add(parmReceiveBatch);
                //* 簽單類別
                SqlParameter parmSignType = new SqlParameter("@Sign_Type", strSign_Type);
                sqlcmd.Parameters.Add(parmSignType);
                //* 一、二KEY註記
                SqlParameter parmKeyIn_Flag = new SqlParameter("@KeyIn_Flag", strKeyIn_Flag);
                sqlcmd.Parameters.Add(parmKeyIn_Flag);
                //* 更新人員
                SqlParameter parmModify_User = new SqlParameter("@Modify_User", strModify_User);
                sqlcmd.Parameters.Add(parmModify_User);

                //* 鍵檔內頁-成功總筆數
                SqlParameter parmKeyin_Success_Count_All = new SqlParameter("@Keyin_Success_Count_All", strKeyin_Success_Count_All);
                sqlcmd.Parameters.Add(parmKeyin_Success_Count_All);
                //* 鍵檔內頁-成功總金額
                SqlParameter parmKeyin_Success_AMT_All = new SqlParameter("@Keyin_Success_AMT_All", strKeyin_Success_AMT_All);
                sqlcmd.Parameters.Add(parmKeyin_Success_AMT_All);
                //* 鍵檔內頁-40(+)總筆數
                SqlParameter parmKeyin_Success_Count_40 = new SqlParameter("@Keyin_Success_Count_40", strKeyin_Success_Count_40);
                sqlcmd.Parameters.Add(parmKeyin_Success_Count_40);
                //* 鍵檔內頁-40(+)總金額
                SqlParameter parmKeyin_Success_AMT_40 = new SqlParameter("@Keyin_Success_AMT_40", strKeyin_Success_AMT_40);
                sqlcmd.Parameters.Add(parmKeyin_Success_AMT_40);
                //* 鍵檔內頁-41(-)總筆數
                SqlParameter parmKeyin_Success_Count_41 = new SqlParameter("@Keyin_Success_Count_41", strKeyin_Success_Count_41);
                sqlcmd.Parameters.Add(parmKeyin_Success_Count_41);
                //* 鍵檔內頁-41(-)總金額
                SqlParameter parmKeyin_Success_AMT_41 = new SqlParameter("@Keyin_Success_AMT_41", strKeyin_Success_AMT_41);
                sqlcmd.Parameters.Add(parmKeyin_Success_AMT_41);
                //* 鍵檔內頁-剔退總筆數
                SqlParameter parmKeyin_Reject_Count_All = new SqlParameter("@Keyin_Reject_Count_All", strKeyin_Reject_Count_All);
                sqlcmd.Parameters.Add(parmKeyin_Reject_Count_All);
                //* 鍵檔內頁-剔退總金額
                SqlParameter parmKeyin_Reject_AMT_All = new SqlParameter("@Keyin_Reject_AMT_All", strKeyin_Reject_AMT_All);
                sqlcmd.Parameters.Add(parmKeyin_Reject_AMT_All);
                //* (剔退)鍵檔內頁-40(+)總筆數
                SqlParameter parmKeyin_Reject_Count_40 = new SqlParameter("@Keyin_Reject_Count_40", strKeyin_Reject_Count_40);
                sqlcmd.Parameters.Add(parmKeyin_Reject_Count_40);
                //* (剔退)鍵檔內頁-40(+)總金額
                SqlParameter parmKeyin_Reject_AMT_40 = new SqlParameter("@Keyin_Reject_AMT_40", strKeyin_Reject_AMT_40);
                sqlcmd.Parameters.Add(parmKeyin_Reject_AMT_40);
                //* (剔退)鍵檔內頁-41(-)總筆數
                SqlParameter parmKeyin_Reject_Count_41 = new SqlParameter("@Keyin_Reject_Count_41", strKeyin_Reject_Count_41);
                sqlcmd.Parameters.Add(parmKeyin_Reject_Count_41);
                //* (剔退)鍵檔內頁-41(-)總金額
                SqlParameter parmKeyin_Reject_AMT_41 = new SqlParameter("@Keyin_Reject_AMT_41", strKeyin_Reject_AMT_41);
                sqlcmd.Parameters.Add(parmKeyin_Reject_AMT_41);

                //* 第一次平帳動作
                SqlParameter parmFirst_Balance_Flag = new SqlParameter("@First_Balance_Flag", strFirst_Balance_Flag);
                sqlcmd.Parameters.Add(parmFirst_Balance_Flag);
                //* 第一次平帳差異筆數(收件-鍵檔)
                SqlParameter parmFirst_Balance_Count = new SqlParameter("@First_Balance_Count", strFirst_Balance_Count);
                sqlcmd.Parameters.Add(parmFirst_Balance_Count);
                //* 第一次平帳差異金額(收件-鍵檔)
                SqlParameter parmFirst_Balance_AMT = new SqlParameter("@First_Balance_AMT", strFirst_Balance_AMT);
                sqlcmd.Parameters.Add(parmFirst_Balance_AMT);


                //* 無第二次平帳時，轉換空字串為0
                strAdjust_Count = strAdjust_Count.Equals("") ? "0" : strAdjust_Count;
                strAdjust_AMT = strAdjust_AMT.Equals("") ? "0" : strAdjust_AMT;
                strSecond_Balance_Count = strSecond_Balance_Count.Equals("") ? "0" : strSecond_Balance_Count;
                strSecond_Balance_AMT = strSecond_Balance_AMT.Equals("") ? "0" : strSecond_Balance_AMT;

                //* 調整後收件正確總筆數
                SqlParameter parmAdjust_Count = new SqlParameter("@Adjust_Count", strAdjust_Count);
                sqlcmd.Parameters.Add(parmAdjust_Count);
                //* 調整後收件正確總金額
                SqlParameter parmAdjust_AMT = new SqlParameter("@Adjust_AMT", strAdjust_AMT);
                sqlcmd.Parameters.Add(parmAdjust_AMT);

                //* 第二次平帳動作
                SqlParameter parmSecond_Balance_Flag = new SqlParameter("@Second_Balance_Flag", strSecond_Balance_Flag);
                sqlcmd.Parameters.Add(parmSecond_Balance_Flag);
                //* 第二次平帳差異筆數(收件-鍵檔)
                SqlParameter parmSecond_Balance_Count = new SqlParameter("@Second_Balance_Count", strSecond_Balance_Count);
                sqlcmd.Parameters.Add(parmSecond_Balance_Count);
                //* 第二次平帳差異金額(收件-鍵檔)
                SqlParameter parmSecond_Balance_AMT = new SqlParameter("@Second_Balance_AMT", strSecond_Balance_AMT);
                sqlcmd.Parameters.Add(parmSecond_Balance_AMT);

                //* 平帳註記
                SqlParameter parmBalance_Flag = new SqlParameter("@Balance_Flag", strBalance_Flag);
                sqlcmd.Parameters.Add(parmBalance_Flag);
                

                //更新資料
                if (BRArtificial_Signing_Primary.Update(sqlcmd))
                {
                    strMsgID = "01_01060400_023";
                    return true;
                }
                else
                {
                    strMsgID = "01_01060400_022";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRArtificial_Signing_Primary.SaveLog(exp);
                strMsgID = "01_01060400_022";
                return false;
            }
        }

        /// <summary>
        /// 更新比對結果註記
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strBatch_NO">批號</param>
        /// <param name="strShop_ID">商店代號</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strSign_Type">簽單類別</param>
        /// <param name="strKeyIn_Flag">一、二KEY註記</param>
        /// <param name="strModify_User"> 更新人員</param>
        /// <param name="strKeyIn_MatchFlag"> 一二KEY比對結果註記(Y/N)</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>true:更新成功；false:更新失敗</returns>
        public static bool Update_ASP_KeyIn_MatchFlag(string strBatch_Date, string strBatch_NO, string strShop_ID, string strReceive_Batch, string strSign_Type,
            string strKeyIn_Flag, string strModify_User, string strKeyIn_MatchFlag, ref string strMsgID)
        {
            try
            {
                //* 聲明SQL Command變量
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = UPDATE_ASP_KeyIn_MatchFlag;
                //* 編批日期
                SqlParameter parmBatchDate = new SqlParameter("@Batch_Date", strBatch_Date.Replace("/", ""));
                sqlcmd.Parameters.Add(parmBatchDate);
                //* 批號
                SqlParameter parmBatchNO = new SqlParameter("@Batch_NO", strBatch_NO);
                sqlcmd.Parameters.Add(parmBatchNO);
                //* 商店代號
                SqlParameter parmShopID = new SqlParameter("@Shop_ID", strShop_ID);
                sqlcmd.Parameters.Add(parmShopID);
                //* 收件批次
                SqlParameter parmReceiveBatch = new SqlParameter("@Receive_Batch", strReceive_Batch);
                sqlcmd.Parameters.Add(parmReceiveBatch);
                //* 簽單類別
                SqlParameter parmSignType = new SqlParameter("@Sign_Type", strSign_Type);
                sqlcmd.Parameters.Add(parmSignType);
                //* 一、二KEY註記
                SqlParameter parmKeyIn_Flag = new SqlParameter("@KeyIn_Flag", strKeyIn_Flag);
                sqlcmd.Parameters.Add(parmKeyIn_Flag);
                //* 更新人員
                SqlParameter parmModify_User = new SqlParameter("@Modify_User", strModify_User);
                sqlcmd.Parameters.Add(parmModify_User);

                //* 一二KEY比對結果註記
                SqlParameter parmKeyIn_MatchFlag = new SqlParameter("@KeyIn_MatchFlag", strKeyIn_MatchFlag);
                sqlcmd.Parameters.Add(parmKeyIn_MatchFlag);

                //更新資料
                if (BRArtificial_Signing_Primary.Update(sqlcmd))
                {
                    strMsgID = "01_01060400_023";
                    return true;
                }
                else
                {
                    strMsgID = "01_01060400_022";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRArtificial_Signing_Primary.SaveLog(exp);
                strMsgID = "01_01060400_022";
                return false;
            }
        }
    }
}
