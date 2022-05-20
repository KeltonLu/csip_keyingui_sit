//******************************************************************
//*  作    者：Jeter
//*  功能說明：Artificial_Signing_Detail資料庫信息
//*  創建日期：2014/07/30
//*  修改記錄：2021/04/01_Ares_Stanley-移除MicrosoftExcel
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
using Framework.Common.Logging;
using System.IO;
using System.Web.UI;
using Framework.Common.Message;
using Framework.Common.Utility;


namespace CSIPKeyInGUI.BusinessRules
{
    public class BRArtificial_Signing_Detail : CSIPCommonModel.BusinessRules.BRBase<EntityArtificial_Signing_Detail>
    {
        #region SQL
        //一KEY 明細序號最大號資料查詢
       // public const string SEL_ASBD_1_CheckMaxNo = @"select Max(SN) as MaxNo from Artificial_Signing_Detail ";

        public const string SEL_ASBD_1_CheckMaxNo = @"select Max(SN) as MaxNo from Artificial_Signing_Detail
where Batch_Date = @Batch_Date
and Shop_ID = @Shop_ID
and Batch_NO = @Batch_NO
and Receive_Batch = @Receive_Batch
and Case_Status = @Case_Status
and KeyIn_Flag = @KeyIn_Flag 
and Sign_Type = @Sign_Type";

        public const string SELDetailCase0 = @" SELECT *,case Receipt_Type when '40' then '請款' when '41' then '退款' end as NewReceipt_Type, CONVERT(VARCHAR(10) , Tran_Date, 111 ) as fTran_Date
  from [Artificial_Signing_Detail]
where Batch_Date = @Batch_Date
  and Shop_ID = @Shop_ID
  and Batch_NO = @Batch_NO
  and Receive_Batch = @Receive_Batch
  and Sign_Type = @Sign_Type
  and Case_Status = '0'
  and KeyIn_Flag = @KeyIn_Flag";

        public const string SELDetailCase1 = @" SELECT *,case Receipt_Type when '40' then '請款' when '41' then '退款' end as NewReceipt_Type, CONVERT(VARCHAR(10) , Tran_Date, 111 ) as fTran_Date
  FROM [Artificial_Signing_Detail]
where Batch_Date = @Batch_Date
  and Shop_ID = @Shop_ID
  and Batch_NO = @Batch_NO
  and Receive_Batch = @Receive_Batch
  and Sign_Type = @Sign_Type
  and Case_Status = '1'
  and KeyIn_Flag = @KeyIn_Flag";


        public const string UpdateDetail = @"Update Artificial_Signing_Detail
set Card_No = @Card_No,
	Tran_Date = @Tran_Date,
	Auth_Code = @Auth_Code,
	AMT = @AMT,
	Receipt_Type = @Receipt_Type
where Batch_Date = @Batch_Date
and Shop_ID = @Shop_ID
and Batch_NO = @Batch_NO
and Receive_Batch = @Receive_Batch
and Sign_Type = @Sign_Type
and SN = @SN
and Case_Status = '0'
and KeyIn_Flag = '1'";

        public const string SQLBalance = @"select COUNT(*) as RowsCount,
         sum(case when Case_Status = '0' then 1 else 0 end) as Keyin_Success_Count_All,
		 sum(case when Case_Status = '0' and Receipt_Type = '40' then 1 else 0 end) as Keyin_Success_Count_40,
		 sum(case when Case_Status = '0' and Receipt_Type = '41' then 1 else 0 end) as Keyin_Success_Count_41,
		 sum(case when Case_Status = '0' and Receipt_Type = '40' then AMT else 0 end) -  
		 sum(case when Case_Status = '0' and Receipt_Type = '41' then AMT else 0 end) as Keyin_Success_AMT_All,
		 sum(case when Case_Status = '0' and Receipt_Type = '40' then AMT else 0 end) as Keyin_Success_AMT_40,
		 sum(case when Case_Status = '0' and Receipt_Type = '41' then AMT else 0 end) as Keyin_Success_AMT_41,
		 sum(case when Case_Status = '1' then 1 else 0 end) as Keyin_Reject_Count_All,
		 sum(case when Case_Status = '1' and Receipt_Type = '40' then 1 else 0 end) as Keyin_Reject_Count_40,
		 sum(case when Case_Status = '1' and Receipt_Type = '41' then 1 else 0 end) as Keyin_Reject_Count_41,
		 sum(case when Case_Status = '1' and Receipt_Type = '40' then AMT else 0 end) -
         sum(case when Case_Status = '1' and Receipt_Type = '41' then AMT else 0 end) as Keyin_Reject_AMT_All,
		 sum(case when Case_Status = '1' and Receipt_Type = '40' then AMT else 0 end) as Keyin_Reject_AMT_40,
		 sum(case when Case_Status = '1' and Receipt_Type = '41' then AMT else 0 end) as Keyin_Reject_AMT_41
  FROM [Artificial_Signing_Detail] 
where Batch_Date = @Batch_Date
	AND Shop_ID = @Shop_ID
	AND Batch_NO = @Batch_NO
	AND Receive_Batch = @Receive_Batch
	AND Sign_Type = @Sign_Type
    AND KeyIn_Flag = @KeyIn_Flag ";

        //明細資料修改
        public const string UPDATE_Detail = @" UPDATE Artificial_Signing_Detail
        SET Card_No = @Card_No, Tran_Date = @Tran_Date, Product_Type = @Product_Type, Installment_Periods = @Installment_Periods,
        Auth_Code = @Auth_Code, AMT = @AMT, Receipt_Type = @Receipt_Type, Modify_User = @Modify_User, Modify_DateTime = GETDATE()
        WHERE Batch_Date = @Batch_Date
	        AND Shop_ID = @Shop_ID
	        AND Batch_NO = @Batch_NO
	        AND Receive_Batch = @Receive_Batch
	        AND SN = @SN
	        AND Case_Status = '0'
	        AND Sign_Type = @Sign_Type
	        AND KeyIn_Flag = @KeyIn_Flag ";

        //剔退明細資料修改
        public const string UPDATE_Reject_Detail = @" UPDATE Artificial_Signing_Detail
        SET Card_No = @Card_No, AMT = @AMT, Receipt_Type = @Receipt_Type, Reject_Reason = @Reject_Reason, Modify_User = @Modify_User, Modify_DateTime = GETDATE()
        WHERE Batch_Date = @Batch_Date
	        AND Shop_ID = @Shop_ID
	        AND Batch_NO = @Batch_NO
	        AND Receive_Batch = @Receive_Batch
	        AND SN = @SN
	        AND Case_Status = '1'
	        AND Sign_Type = @Sign_Type
	        AND KeyIn_Flag = @KeyIn_Flag ";

        //新增明細資料
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        public const string INSERT_Detail = @" Insert into [{0}].[dbo].[Artificial_Signing_Detail] 
        ([Batch_Date],[Shop_ID],[Batch_NO],[Receive_Batch],[Sign_Type],[Case_Status],[KeyIn_Flag],[SN],[Card_No],[Tran_Date],[Product_Type],[Auth_Code],[AMT],
            [Receipt_Type],[Reject_Reason],[Installment_Periods],[Create_User],[Create_DateTime]) 
        Values(@Batch_Date,@Shop_ID,@Batch_NO,@Receive_Batch,@Sign_Type,@Case_Status,@KeyIn_Flag,@SN,@Card_No,@Tran_Date,@Product_Type,@Auth_Code,@AMT,
            @Receipt_Type,@Reject_Reason,@Installment_Periods,@Create_User,GETDATE()); ";

        //刪除明細資料
        public const string DELETE_Detail = @" Delete Artificial_Signing_Detail 
            WHERE Batch_Date = @Batch_Date and Shop_ID = @Shop_ID and Batch_NO = @Batch_NO and Receive_Batch = @Receive_Batch and SN = @SN 
                and Case_Status = @Case_Status and Sign_Type = @Sign_Type and KeyIn_Flag = @KeyIn_Flag ";

        //抄寫一Key資料
        public const string COPY1KEY_Detail = @" INSERT INTO [dbo].[Artificial_Signing_Detail]([Batch_Date],[Shop_ID],[Batch_NO],[Receive_Batch],[Sign_Type],[SN],[Card_No],[Tran_Date],[Product_Type],[Installment_Periods],[Auth_Code],[AMT],[Receipt_Type],[Case_Status],[Reject_Reason],[KeyIn_Flag],[Create_User],[Create_DateTime])
            SELECT [Batch_Date],[Shop_ID],[Batch_NO],[Receive_Batch],[Sign_Type],[SN],[Card_No],[Tran_Date],[Product_Type],[Installment_Periods],[Auth_Code],[AMT],[Receipt_Type],[Case_Status],[Reject_Reason],'2',@Create_User as [Create_User],GETDATE() as [Create_DateTime]
            FROM [Artificial_Signing_Detail]
            Where KeyIn_Flag =  @KeyIn_Flag
            and Case_Status = @Case_Status
            and Batch_Date = @Batch_Date
            and Shop_ID = @Shop_ID
            and Batch_NO = @Batch_NO
            and Receive_Batch = @Receive_Batch
            and Sign_Type = @Sign_Type ";

        #endregion

        /// <summary>
        /// 明細序號最大號資料查詢
        /// </summary>
        public static System.Data.DataTable Select_1CheckMaxNo(string strBatch_Date, string strShop_ID, string strBatch_NO, string strReceive_Batch, string strCase_Status, string strKeyIn_Flag, string strSign_Type)
        {
            DataSet ds = null;
            SqlHelper sSql = new SqlHelper();

            SqlCommand sqlComm = new SqlCommand();
            try
            {
                sqlComm.CommandText = SEL_ASBD_1_CheckMaxNo;

                SqlParameter parmBatch_Date = new SqlParameter("@Batch_Date", strBatch_Date.Replace("/", ""));
                SqlParameter parmShop_ID = new SqlParameter("@Shop_ID", strShop_ID);
                SqlParameter parmBatch_NO = new SqlParameter("@Batch_NO", strBatch_NO);
                SqlParameter parmReceive_Batch = new SqlParameter("@Receive_Batch", strReceive_Batch);
                SqlParameter parmCase_Status = new SqlParameter("@Case_Status", strCase_Status);
                SqlParameter parmKeyIn_Flag = new SqlParameter("@KeyIn_Flag", strKeyIn_Flag);
                SqlParameter parmSign_Type = new SqlParameter("@Sign_Type", strSign_Type);

                sqlComm.Parameters.Add(parmBatch_Date);
                sqlComm.Parameters.Add(parmShop_ID);
                sqlComm.Parameters.Add(parmBatch_NO);
                sqlComm.Parameters.Add(parmReceive_Batch);
                sqlComm.Parameters.Add(parmCase_Status);
                sqlComm.Parameters.Add(parmKeyIn_Flag);
                sqlComm.Parameters.Add(parmSign_Type);

                sqlComm.CommandType = CommandType.Text;
                ds = BRArtificial_Signing_Primary.SearchOnDataSet(sqlComm);
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
                throw ex;
            }

            return ds.Tables[0];
        }

        /// <summary>
        /// 第一次平帳畫面上筆數與金額
        /// </summary>
        public static System.Data.DataTable SelectBalanceData(string strBatch_Date, string strShop_ID, string strBatch_NO, string strReceive_Batch, string strSign_Type, string strKeyIn_Flag)
        {
            DataSet ds = null;
            SqlHelper sSql = new SqlHelper();

            SqlCommand sqlComm = new SqlCommand();
            try
            {
                sqlComm.CommandText = SQLBalance;
                sqlComm.CommandType = CommandType.Text;

                SqlParameter parmBatch_Date = new SqlParameter("@Batch_Date", strBatch_Date.Replace("/", ""));
                SqlParameter parmShop_ID = new SqlParameter("@Shop_ID", strShop_ID);                
                SqlParameter parmBatch_NO = new SqlParameter("@Batch_NO", strBatch_NO);
                SqlParameter parmReceive_Batch = new SqlParameter("@Receive_Batch", strReceive_Batch); 
                SqlParameter parmSign_Type = new SqlParameter("@Sign_Type", strSign_Type);
                SqlParameter parmKeyIn_Flag = new SqlParameter("@KeyIn_Flag", strKeyIn_Flag);

                sqlComm.Parameters.Add(parmBatch_Date);
                sqlComm.Parameters.Add(parmShop_ID);
                sqlComm.Parameters.Add(parmBatch_NO);
                sqlComm.Parameters.Add(parmReceive_Batch);
                sqlComm.Parameters.Add(parmSign_Type);
                sqlComm.Parameters.Add(parmKeyIn_Flag);

                ds = BRArtificial_Signing_Detail.SearchOnDataSet(sqlComm);
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
                throw ex;
            }

            return ds.Tables[0];
        }

        /// <summary>
        /// 明細資料查詢
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strShop_ID">商店代號</param>
        /// <param name="strBatch_NO">批號</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <returns></returns>        
        public static System.Data.DataTable SelectDetailCase0(string strBatch_Date, string strShop_ID, string strBatch_NO, string strReceive_Batch, string strSign_Type, string strKeyIn_Flag)
        {
            DataSet ds = null;
            SqlHelper sSql = new SqlHelper();

            SqlCommand sqlComm = new SqlCommand();
            try
            {
                sqlComm.CommandText = SELDetailCase0;
                sqlComm.CommandType = CommandType.Text;

                SqlParameter parmBatch_Date = new SqlParameter("@Batch_Date", strBatch_Date.Replace("/", ""));
                SqlParameter parmShop_ID = new SqlParameter("@Shop_ID", strShop_ID);
                SqlParameter parmBatch_NO = new SqlParameter("@Batch_NO", strBatch_NO);
                SqlParameter parmReceive_Batch = new SqlParameter("@Receive_Batch", strReceive_Batch);
                SqlParameter parmSign_Type = new SqlParameter("@Sign_Type", strSign_Type);
                SqlParameter parmKeyIn_Flag = new SqlParameter("@KeyIn_Flag", strKeyIn_Flag);

                sqlComm.Parameters.Add(parmBatch_Date);
                sqlComm.Parameters.Add(parmShop_ID);
                sqlComm.Parameters.Add(parmBatch_NO);
                sqlComm.Parameters.Add(parmReceive_Batch);
                sqlComm.Parameters.Add(parmSign_Type);
                sqlComm.Parameters.Add(parmKeyIn_Flag);

                ds = BRArtificial_Signing_Batch_Data.SearchOnDataSet(sqlComm);
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
                throw ex;
            }

            return ds.Tables[0];
        }

        /// <summary>
        /// 明細剔退資料查詢
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strShop_ID">商店代號</param>
        /// <param name="strBatch_NO">批號</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <returns></returns>        
        public static System.Data.DataTable SelectDetailCase1(string strBatch_Date, string strShop_ID, string strBatch_NO, string strReceive_Batch, string strSign_Type, string strKeyIn_Flag)
        {
            DataSet ds = null;
            SqlHelper sSql = new SqlHelper();

            SqlCommand sqlComm = new SqlCommand();
            try
            {
                sqlComm.CommandText = SELDetailCase1;
                sqlComm.CommandType = CommandType.Text;

                SqlParameter parmBatch_Date = new SqlParameter("@Batch_Date", strBatch_Date.Replace("/", ""));
                SqlParameter parmShop_ID = new SqlParameter("@Shop_ID", strShop_ID);
                SqlParameter parmBatch_NO = new SqlParameter("@Batch_NO", strBatch_NO);
                SqlParameter parmReceive_Batch = new SqlParameter("@Receive_Batch", strReceive_Batch);
                SqlParameter parmSign_Type = new SqlParameter("@Sign_Type", strSign_Type);
                SqlParameter parmKeyIn_Flag = new SqlParameter("@KeyIn_Flag", strKeyIn_Flag);

                sqlComm.Parameters.Add(parmBatch_Date);
                sqlComm.Parameters.Add(parmShop_ID);
                sqlComm.Parameters.Add(parmBatch_NO);
                sqlComm.Parameters.Add(parmReceive_Batch);
                sqlComm.Parameters.Add(parmSign_Type);
                sqlComm.Parameters.Add(parmKeyIn_Flag);

                ds = BRArtificial_Signing_Batch_Data.SearchOnDataSet(sqlComm);
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
                throw ex;
            }

            return ds.Tables[0];
        }

        /// <summary>
        /// 明細資料修改(正常件)
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strBatch_NO">批號</param>
        /// <param name="strShop_ID">商店代號</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strSN">明細序號</param>
        /// <param name="strSign_Type">簽單類別</param>
        /// <param name="strKeyIn_Flag">一、二KEY註記</param>
        /// <param name="strCard_No">交易卡號</param>
        /// <param name="strTran_Date">交易日期</param>
        /// <param name="strProduct_Type">產品別</param>
        /// <param name="strInstallment_Periods">分期期數</param>
        /// <param name="strAuth_Code">授權號碼</param>
        /// <param name="strAMT">金額/分期總價</param>
        /// <param name="strReceipt_Type">請退款</param>
        /// <param name="strModify_User"> 更新人員</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>true:更新成功；false:更新失敗</returns>
        public static bool Update_Detail(string strBatch_Date, string strBatch_NO, string strShop_ID, string strReceive_Batch, string strSN,
            string strSign_Type, string strKeyIn_Flag, string strCard_No, string strTran_Date, string strProduct_Type, 
            string strInstallment_Periods, string strAuth_Code, string strAMT, string strReceipt_Type, string strModify_User, ref string strMsgID)
        {
            try
            {
                //* 聲明SQL Command變量
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = UPDATE_Detail;
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
                //* 明細序號
                SqlParameter parmSN = new SqlParameter("@SN", strSN);
                sqlcmd.Parameters.Add(parmSN);
                //* 簽單類別
                SqlParameter parmSignType = new SqlParameter("@Sign_Type", strSign_Type);
                sqlcmd.Parameters.Add(parmSignType);
                //* 一、二KEY註記
                SqlParameter parmKeyIn_Flag = new SqlParameter("@KeyIn_Flag", strKeyIn_Flag);
                sqlcmd.Parameters.Add(parmKeyIn_Flag);
                //* 交易卡號
                SqlParameter parmCard_No = new SqlParameter("@Card_No", strCard_No);
                sqlcmd.Parameters.Add(parmCard_No);
                //* 交易日期
                SqlParameter parmTran_Date = new SqlParameter("@Tran_Date", strTran_Date.Replace("/", ""));
                sqlcmd.Parameters.Add(parmTran_Date);

                //* 產品別
                SqlParameter parmProduct_Type = new SqlParameter("@Product_Type", strProduct_Type);
                if (string.IsNullOrEmpty(strProduct_Type))
                    parmProduct_Type = new SqlParameter("@Product_Type", DBNull.Value);
                sqlcmd.Parameters.Add(parmProduct_Type);
                //* 分期期數
                //20141002  分期期數如第一碼為0則去除，EX: 06 需存為 6
                if (!string.IsNullOrEmpty(strInstallment_Periods))
                    strInstallment_Periods = int.Parse(strInstallment_Periods).ToString();
                SqlParameter parmInstallment_Periods = new SqlParameter("@Installment_Periods", strInstallment_Periods);
                if (string.IsNullOrEmpty(strInstallment_Periods))
                    parmInstallment_Periods = new SqlParameter("@Installment_Periods", DBNull.Value);
                sqlcmd.Parameters.Add(parmInstallment_Periods);
                //* 授權號碼
                SqlParameter parmAuth_Code = new SqlParameter("@Auth_Code", strAuth_Code);
                if (string.IsNullOrEmpty(strAuth_Code))
                    parmAuth_Code = new SqlParameter("@Auth_Code", DBNull.Value);
                sqlcmd.Parameters.Add(parmAuth_Code);
                
                //* 金額/分期總價
                SqlParameter parmAMT = new SqlParameter("@AMT", strAMT);
                sqlcmd.Parameters.Add(parmAMT);
                //* 請退款
                SqlParameter parmReceipt_Type = new SqlParameter("@Receipt_Type", strReceipt_Type);
                sqlcmd.Parameters.Add(parmReceipt_Type);
                //* 更新人員
                SqlParameter parmModify_User = new SqlParameter("@Modify_User", strModify_User);
                sqlcmd.Parameters.Add(parmModify_User);

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
        /// 剔退明細資料修改
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strBatch_NO">批號</param>
        /// <param name="strShop_ID">商店代號</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strSN">明細序號</param>
        /// <param name="strSign_Type">簽單類別</param>
        /// <param name="strKeyIn_Flag">一、二KEY註記</param>
        /// <param name="strCard_No">交易卡號</param>
        /// <param name="strAMT">剔退金額</param>
        /// <param name="strReceipt_Type">請退款</param>
        /// <param name="strReject_Reason">剔退原因</param>
        /// <param name="strModify_User"> 更新人員</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>true:更新成功；false:更新失敗</returns>
        public static bool Update_Reject_Detail(string strBatch_Date, string strBatch_NO, string strShop_ID, string strReceive_Batch, string strSN,
            string strSign_Type, string strKeyIn_Flag, string strCard_No, string strAMT, string strReceipt_Type, string strReject_Reason, 
            string strModify_User, ref string strMsgID)
        {
            try
            {
                //* 聲明SQL Command變量
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = UPDATE_Reject_Detail;
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
                //* 明細序號
                SqlParameter parmSN = new SqlParameter("@SN", strSN);
                sqlcmd.Parameters.Add(parmSN);
                //* 簽單類別
                SqlParameter parmSignType = new SqlParameter("@Sign_Type", strSign_Type);
                sqlcmd.Parameters.Add(parmSignType);
                //* 一、二KEY註記
                SqlParameter parmKeyIn_Flag = new SqlParameter("@KeyIn_Flag", strKeyIn_Flag);
                sqlcmd.Parameters.Add(parmKeyIn_Flag);
                //* 交易卡號
                SqlParameter parmCard_No = new SqlParameter("@Card_No", strCard_No);
                sqlcmd.Parameters.Add(parmCard_No);
                //* 剔退金額
                SqlParameter parmAMT = new SqlParameter("@AMT", strAMT);
                sqlcmd.Parameters.Add(parmAMT);
                //* 請退款
                SqlParameter parmReceipt_Type = new SqlParameter("@Receipt_Type", strReceipt_Type);
                sqlcmd.Parameters.Add(parmReceipt_Type);
                //* 剔退原因
                SqlParameter parmReject_Reason = new SqlParameter("@Reject_Reason", strReject_Reason);
                sqlcmd.Parameters.Add(parmReject_Reason);
                //* 更新人員
                SqlParameter parmModify_User = new SqlParameter("@Modify_User", strModify_User);
                sqlcmd.Parameters.Add(parmModify_User);

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
        /// 新增明細資料
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strBatch_NO">批號</param>
        /// <param name="strShop_ID">商店代號</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strSN">明細序號</param>
        /// <param name="strSign_Type">簽單類別</param>
        /// <param name="strCase_Status">案件狀態</param>
        /// <param name="strKeyIn_Flag">一、二KEY註記</param>
        /// <param name="strCard_No">交易卡號</param>
        /// <param name="strTran_Date">交易日期</param>
        /// <param name="strProduct_Type">產品別</param>
        /// <param name="strInstallment_Periods">分期期數</param>
        /// <param name="strAuth_Code">授權號碼</param>
        /// <param name="strAMT">金額/分期總價</param>
        /// <param name="strReceipt_Type">請退款</param>
        /// <param name="strReject_Reason">剔退原因</param>
        /// <param name="strCreate_User"> 新增人員</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>true:新增成功；false:新增失敗</returns>
        public static bool Insert_Detail(string strBatch_Date, string strBatch_NO, string strShop_ID, string strReceive_Batch, string strSN,
            string strSign_Type, string strCase_Status, string strKeyIn_Flag, string strCard_No, string strTran_Date, string strProduct_Type,
            string strInstallment_Periods, string strAuth_Code, string strAMT, string strReceipt_Type, string strReject_Reason, string strCreate_User, ref string strMsgID)
        {
            try
            {
                //* 聲明SQL Command變量
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                sqlcmd.CommandText = string.Format(INSERT_Detail, UtilHelper.GetAppSettings("DB_KeyinGUI"));

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
                //* 明細序號
                SqlParameter parmSN = new SqlParameter("@SN", strSN);
                sqlcmd.Parameters.Add(parmSN);
                //* 簽單類別
                SqlParameter parmSignType = new SqlParameter("@Sign_Type", strSign_Type);
                sqlcmd.Parameters.Add(parmSignType);
                //* 案件狀態
                SqlParameter parmCase_Status = new SqlParameter("@Case_Status", strCase_Status);
                sqlcmd.Parameters.Add(parmCase_Status);
                //* 一、二KEY註記
                SqlParameter parmKeyIn_Flag = new SqlParameter("@KeyIn_Flag", strKeyIn_Flag);
                sqlcmd.Parameters.Add(parmKeyIn_Flag);
                //* 交易卡號
                SqlParameter parmCard_No = new SqlParameter("@Card_No", strCard_No);
                sqlcmd.Parameters.Add(parmCard_No);
                
                //* 交易日期
                SqlParameter parmTran_Date = new SqlParameter("@Tran_Date", strTran_Date.Replace("/", ""));
                if (string.IsNullOrEmpty(strTran_Date))
                    parmTran_Date = new SqlParameter("@Tran_Date", DBNull.Value);
                sqlcmd.Parameters.Add(parmTran_Date);
                //* 產品別
                SqlParameter parmProduct_Type = new SqlParameter("@Product_Type", strProduct_Type);
                if (string.IsNullOrEmpty(strProduct_Type))
                    parmProduct_Type = new SqlParameter("@Product_Type", DBNull.Value);
                sqlcmd.Parameters.Add(parmProduct_Type);
                //* 分期期數
                //20141002  分期期數如第一碼為0則去除，EX: 06 需存為 6
                if (!string.IsNullOrEmpty(strInstallment_Periods))
                    strInstallment_Periods = int.Parse(strInstallment_Periods).ToString();
                SqlParameter parmInstallment_Periods = new SqlParameter("@Installment_Periods", strInstallment_Periods);
                if (string.IsNullOrEmpty(strInstallment_Periods))
                    parmInstallment_Periods = new SqlParameter("@Installment_Periods", DBNull.Value);
                sqlcmd.Parameters.Add(parmInstallment_Periods);
                //* 授權號碼
                SqlParameter parmAuth_Code = new SqlParameter("@Auth_Code", strAuth_Code);
                if (string.IsNullOrEmpty(strAuth_Code))
                    parmAuth_Code = new SqlParameter("@Auth_Code", DBNull.Value);
                sqlcmd.Parameters.Add(parmAuth_Code);
                //* 剔退原因
                SqlParameter parmReject_Reason = new SqlParameter("@Reject_Reason", strReject_Reason);
                if (string.IsNullOrEmpty(strReject_Reason))
                    parmReject_Reason = new SqlParameter("@Reject_Reason", DBNull.Value);
                sqlcmd.Parameters.Add(parmReject_Reason);

                //* 金額/分期總價
                SqlParameter parmAMT = new SqlParameter("@AMT", strAMT);
                sqlcmd.Parameters.Add(parmAMT);
                //* 請退款
                SqlParameter parmReceipt_Type = new SqlParameter("@Receipt_Type", strReceipt_Type);
                sqlcmd.Parameters.Add(parmReceipt_Type);
                //* 新增人員
                SqlParameter parmCreate_User = new SqlParameter("@Create_User", strCreate_User);
                sqlcmd.Parameters.Add(parmCreate_User);

                //新增資料
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
        /// 刪除明細資料
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strBatch_NO">批號</param>
        /// <param name="strShop_ID">商店代號</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strSN">明細序號</param>
        /// <param name="strSign_Type">簽單類別</param>
        /// <param name="strCase_Status">案件狀態</param>
        /// <param name="strKeyIn_Flag">一、二KEY註記</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>true:刪除成功；false:刪除失敗</returns>
        public static bool Delete_Detail(string strBatch_Date, string strBatch_NO, string strShop_ID, string strReceive_Batch, string strSN,
            string strSign_Type, string strCase_Status, string strKeyIn_Flag, ref string strMsgID)
        {
            try
            {
                //* 聲明SQL Command變量
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = DELETE_Detail;

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
                //* 明細序號
                SqlParameter parmSN = new SqlParameter("@SN", strSN);
                sqlcmd.Parameters.Add(parmSN);
                //* 簽單類別
                SqlParameter parmSignType = new SqlParameter("@Sign_Type", strSign_Type);
                sqlcmd.Parameters.Add(parmSignType);
                //* 案件狀態
                SqlParameter parmCase_Status = new SqlParameter("@Case_Status", strCase_Status);
                sqlcmd.Parameters.Add(parmCase_Status);
                //* 一、二KEY註記
                SqlParameter parmKeyIn_Flag = new SqlParameter("@KeyIn_Flag", strKeyIn_Flag);
                sqlcmd.Parameters.Add(parmKeyIn_Flag);

                //刪除資料
                if (BRArtificial_Signing_Primary.Delete(sqlcmd))
                {
                    strMsgID = "01_01060400_025";
                    return true;
                }
                else
                {
                    strMsgID = "01_01060400_024";
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRArtificial_Signing_Primary.SaveLog(exp);
                strMsgID = "01_01060400_024";
                return false;
            }
        }

        /// <summary>
        /// 抄寫一Key資料
        /// </summary>
        /// <param name="strBatch_Date">編批日期</param>
        /// <param name="strBatch_NO">批號</param>
        /// <param name="strShop_ID">商店代號</param>
        /// <param name="strReceive_Batch">收件批次</param>
        /// <param name="strSign_Type">簽單類別</param>
        /// <param name="strCase_Status">案件狀態</param>
        /// <param name="strKeyIn_Flag">一、二KEY註記</param>
        /// <param name="strCreate_User"> 新增人員</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>true:新增成功；false:新增失敗</returns>
        public static bool Copy1Key_Detail(string strBatch_Date, string strBatch_NO, string strShop_ID, string strReceive_Batch,
            string strSign_Type, string strCase_Status, string strKeyIn_Flag, string strCreate_User, ref string strMsgID)
        {
            try
            {
                //* 聲明SQL Command變量
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = COPY1KEY_Detail;

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
                //* 案件狀態
                SqlParameter parmCase_Status = new SqlParameter("@Case_Status", strCase_Status);
                sqlcmd.Parameters.Add(parmCase_Status);
                //* 一、二KEY註記
                SqlParameter parmKeyIn_Flag = new SqlParameter("@KeyIn_Flag", strKeyIn_Flag);
                sqlcmd.Parameters.Add(parmKeyIn_Flag);
                //* 新增人員
                SqlParameter parmCreate_User = new SqlParameter("@Create_User", strCreate_User);
                sqlcmd.Parameters.Add(parmCreate_User);

                //新增資料
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
    }
}
