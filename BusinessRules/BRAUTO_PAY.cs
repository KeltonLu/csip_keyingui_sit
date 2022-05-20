//******************************************************************
//*  作    者：chenjingxian
//*  功能說明：AUTO_PAY資料庫信息

//*  創建日期：2009/07/12
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
namespace CSIPKeyInGUI.BusinessRules
{
    public class BRAUTO_PAY : CSIPCommonModel.BusinessRules.BRBase<EntityAUTO_PAY>
    {

        #region SQL

        //20131008 Casper 檢核收件編號要包含Other_Bank_Temp,AUTO_Pay_Popul
        public const string SEL_AUTO_PAY = @"SELECT {0} FROM AUTO_PAY WHERE ";

        public const string SEL_AUTO_PAY_1 = @"select Receive_Number,cus_id from Auto_Pay " +
                                                        " where Receive_Number=@ReceiveNumber " +
                                                        " union all " +
                                                        " select Receive_Number,cus_id from Auto_Pay_Popul " +
                                                        "  where Receive_Number=@ReceiveNumber" +
                                                        " union all " +
                                                        " select Receive_Number,cus_id from Other_Bank_Temp " +
                                                        "  where Receive_Number=@ReceiveNumber";

        #endregion
        /// <summary>
        /// 驗證收件編號和身份證號碼
        /// </summary>
        /// <param name="strReceiveNumber"></param>
        /// <param name="strAddFlag"></param>
        /// <param name="strUploadFlag"></param>
        /// <param name="strColumns"></param>
        /// <returns></returns>        
        public static DataSet Select(string strReceiveNumber)
        {
            DataSet dstInfo = null;
            SqlHelper sSql = new SqlHelper();

            //檢查收編是否已用過，檢查AUTO_PAY,AUTO_PAY_POPUL表
            SqlCommand sqlComm = new SqlCommand();
            try
            {
            sqlComm.CommandText = SEL_AUTO_PAY_1;
            sqlComm.CommandType = CommandType.Text;

            SqlParameter parmReceiveNumber = new SqlParameter("@ReceiveNumber", strReceiveNumber);
            sqlComm.Parameters.Add(parmReceiveNumber);
            
            dstInfo = BRAUTO_PAY.SearchOnDataSet(sqlComm);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dstInfo;
        }
        /// <summary>
        /// 查詢中信及郵局 自扣一KEY資料
        /// </summary>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strAddFlag">資料類別</param>
        /// <param name="strUploadFlag">異動類別</param>
        /// <param name="strColumns">要查詢得到的欄位</param>
        /// <returns>DataSet</returns>
        public static DataSet Select(string strReceiveNumber, string strAddFlag, string strUploadFlag, string strColumns)
        {
            DataSet dstInfo = null;
            SqlHelper sSql = new SqlHelper();

            sSql.AddCondition(EntityAUTO_PAY.M_Receive_Number, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
            sSql.AddCondition(EntityAUTO_PAY.M_Add_Flag, Operator.Equal, DataTypeUtils.String, strAddFlag);
            sSql.AddCondition(EntityAUTO_PAY.M_Upload_Flag, Operator.Equal, DataTypeUtils.String, strUploadFlag);

            string strSqlCmd = string.Format(SEL_AUTO_PAY, strColumns) + sSql.GetFilterCondition().Substring(4, sSql.GetFilterCondition().Length - 4);

            dstInfo = BRAUTO_PAY.SearchOnDataSet(strSqlCmd);

            return dstInfo;
        }

        /// <summary>
        /// 查詢中信及郵局 自扣一KEY資料
        /// </summary>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strAddFlag">資料類別</param>
        /// <param name="strUploadFlag">異動類別</param>
        /// <param name="strColumns">要查詢得到的欄位</param>
        /// <returns>DataSet</returns>
        public static DataSet SelectFlag(string strReceiveNumber, string strAddFlag, string strUploadFlag, string strColumns)
        {
            DataSet dstInfo = null;
            SqlHelper sSql = new SqlHelper();

            sSql.AddCondition(EntityAUTO_PAY.M_Receive_Number, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
            sSql.AddCondition(EntityAUTO_PAY.M_Add_Flag, Operator.Equal, DataTypeUtils.String, strAddFlag);
            sSql.AddCondition(EntityAUTO_PAY.M_Upload_Flag, Operator.NotEqual , DataTypeUtils.String, strUploadFlag);

            string strSqlCmd = string.Format(SEL_AUTO_PAY, strColumns) + sSql.GetFilterCondition().Substring(4, sSql.GetFilterCondition().Length - 4);

            dstInfo = BRAUTO_PAY.SearchOnDataSet(strSqlCmd);

            return dstInfo;
        }

        /// <summary>
        /// 查詢中信及郵局 自扣一KEY資料
        /// </summary>
        /// <param name="strUserId">身分證號碼</param>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strAddFlag">資料類別</param>
        /// <param name="strUploadFlag">異動類別</param>
        /// <param name="strKeyFlag">KeyIn類別</param>
        /// <param name="strColumns">要查詢得到的欄位</param>
        /// <returns>DataSet</returns>
        public static DataSet SelectDetail(string strUserId, string strReceiveNumber, string strAddFlag, string strUploadFlag, string strKeyFlag, string strColumns)
        {
            DataSet dstInfo = null;
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntityAUTO_PAY.M_Cus_ID, Operator.Equal, DataTypeUtils.String, strUserId);
            sSql.AddCondition(EntityAUTO_PAY.M_Receive_Number, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
            sSql.AddCondition(EntityAUTO_PAY.M_Add_Flag, Operator.Equal , DataTypeUtils.String, strAddFlag);
            sSql.AddCondition(EntityAUTO_PAY.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
            sSql.AddCondition(EntityAUTO_PAY.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyFlag);

            string strSqlCmd = string.Format(SEL_AUTO_PAY, strColumns) + sSql.GetFilterCondition().Substring(4, sSql.GetFilterCondition().Length - 4);

            dstInfo = BRAUTO_PAY.SearchOnDataSet(strSqlCmd);

            return dstInfo;
        }

        /// <summary>
        /// 查詢中信及郵局 自扣一KEY資料
        /// </summary>
        /// <param name="strUserId">身分證號碼</param>
        /// <param name="strAddFlag">資料類別</param>
        /// <param name="strUploadFlag">異動類別</param>
        /// <param name="strKeyFlag">KeyIn類別</param>
        /// <param name="strColumns">要查詢得到的欄位</param>
        /// <returns>DataSet</returns>
        public static DataSet SelectDetail(string strUserId, string strAddFlag, string strUploadFlag, string strKeyFlag, string strColumns)
        {
            DataSet dstInfo = null;
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntityAUTO_PAY.M_Cus_ID, Operator.Equal, DataTypeUtils.String, strUserId);
            sSql.AddCondition(EntityAUTO_PAY.M_Add_Flag, Operator.Equal, DataTypeUtils.String, strAddFlag);
            sSql.AddCondition(EntityAUTO_PAY.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
            sSql.AddCondition(EntityAUTO_PAY.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyFlag);

            string strSqlCmd = string.Format(SEL_AUTO_PAY, strColumns) + sSql.GetFilterCondition().Substring(4, sSql.GetFilterCondition().Length - 4);

            dstInfo = BRAUTO_PAY.SearchOnDataSet(strSqlCmd);

            return dstInfo;
        }

        /// <summary>
        /// 查詢資料庫一、二KEY信息
        /// </summary>
        /// <param name="strUserId">身分證號碼</param>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strAddFlag">資料類別</param>
        /// <param name="strUploadFlag">異動類別</param>
        /// <param name="strKeyFlag">KeyIn類別</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntityAUTO_PAY> SelectEntitySet(string strUserId, string strReceiveNumber, string strAddFlag, string strUploadFlag, string strKeyFlag)
        {
            SqlHelper sSql = new SqlHelper();
            EntitySet<EntityAUTO_PAY> eAutoPay = null;
            try
            {
                sSql.AddCondition(EntityAUTO_PAY.M_Cus_ID, Operator.Equal, DataTypeUtils.String, strUserId);
                sSql.AddCondition(EntityAUTO_PAY.M_Receive_Number, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
                sSql.AddCondition(EntityAUTO_PAY.M_Add_Flag, Operator.Equal, DataTypeUtils.String, strAddFlag);
                sSql.AddCondition(EntityAUTO_PAY.M_Upload_Flag, Operator.NotEqual , DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityAUTO_PAY.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyFlag);

                eAutoPay = BRAUTO_PAY.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return eAutoPay;
        }

        /// <summary>
        /// 查詢資料庫一KEY信息
        /// </summary>
        /// <param name="strUserId">身分證號碼</param>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strAddFlag">資料類別</param>
        /// <param name="strUploadFlag">異動類別</param>
        /// <param name="strKeyFlag">KeyIn類別</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntityAUTO_PAY> SelectEntitySet1Key(string strUserId, string strAddFlag, string strUploadFlag, string strKeyFlag)
        {
            SqlHelper sSql = new SqlHelper();
            EntitySet<EntityAUTO_PAY> eAutoPay = null;
            try
            {
                sSql.AddCondition(EntityAUTO_PAY.M_Cus_ID, Operator.Equal, DataTypeUtils.String, strUserId);
                sSql.AddCondition(EntityAUTO_PAY.M_Add_Flag, Operator.Equal, DataTypeUtils.String, strAddFlag);
                sSql.AddCondition(EntityAUTO_PAY.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityAUTO_PAY.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyFlag);

                eAutoPay = BRAUTO_PAY.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return eAutoPay;
        }


        /// <summary>
        /// 查詢資料庫二KEY信息
        /// </summary>
        /// <param name="strUserId">身分證號碼</param>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strAddFlag">資料類別</param>
        /// <param name="strUploadFlag">異動類別</param>
        /// <param name="strKeyFlag">KeyIn類別</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntityAUTO_PAY> SelectEntitySet2Key(string strUserId, string strAddFlag, string strUploadFlag, string strKeyFlag)
        {
            SqlHelper sSql = new SqlHelper();
            EntitySet<EntityAUTO_PAY> eAutoPay = null;
            try
            {
                sSql.AddCondition(EntityAUTO_PAY.M_Cus_ID, Operator.Equal, DataTypeUtils.String, strUserId);
                sSql.AddCondition(EntityAUTO_PAY.M_Add_Flag, Operator.Equal, DataTypeUtils.String, strAddFlag);
                sSql.AddCondition(EntityAUTO_PAY.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityAUTO_PAY.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyFlag);

                eAutoPay = BRAUTO_PAY.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return eAutoPay;
        }

        /// <summary>
        /// 更新一KEY或二KEY資料
        /// </summary>
        /// <param name="eAutoPay">Auto_Pay實體</param>
        /// <param name="strField">要更新欄位的集合</param>
        /// <param name="strUserId">身分證號碼</param>
        /// <param name="strAddFlag">資料類別</param>
        /// <param name="strUploadFlag">異動類別</param>
        /// <param name="strKeyFlag">KeyIn類別</param>
        /// <returns>true成功，false失敗</returns>
        
        public static bool Update(EntityAUTO_PAY eAutoPay, string[] strField, string strUserId, string strAddFlag, string strUploadFlag, string strKeyFlag)
        {
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntityAUTO_PAY.M_Cus_ID, Operator.Equal, DataTypeUtils.String, strUserId);
            sSql.AddCondition(EntityAUTO_PAY.M_Add_Flag, Operator.Equal, DataTypeUtils.String, strAddFlag);
            sSql.AddCondition(EntityAUTO_PAY.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
            sSql.AddCondition(EntityAUTO_PAY.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyFlag);

            return BRAUTO_PAY.UpdateEntityByCondition(eAutoPay, sSql.GetFilterCondition(), strField);

        }


        /// <summary>
        /// 異動主機成功后，更新資料
        /// </summary>
        /// <param name="eAutoPay">Auto_Pay實體</param>
        /// <param name="strField">要更新欄位的集合</param>
        /// <param name="strUserId">身分證號碼</param>
        /// <param name="strAddFlag">資料類別</param>
        /// <param name="strUploadFlag">異動類別</param>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <returns>true成功，false失敗</returns>

        public static bool UpdateSucc(EntityAUTO_PAY eAutoPay, string[] strField, string strUserId, string strAddFlag, string strUploadFlag, string strReceiveNumber)
        {
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntityAUTO_PAY.M_Cus_ID, Operator.Equal, DataTypeUtils.String, strUserId);
            sSql.AddCondition(EntityAUTO_PAY.M_Add_Flag, Operator.Equal, DataTypeUtils.String, strAddFlag);
            sSql.AddCondition(EntityAUTO_PAY.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
            sSql.AddCondition(EntityAUTO_PAY.M_Receive_Number, Operator.Equal, DataTypeUtils.String, strReceiveNumber);

            return BRAUTO_PAY.UpdateEntityByCondition(eAutoPay, sSql.GetFilterCondition(), strField);
        }

        /// <summary>
        /// 用Entity方式插入資料庫
        /// </summary>
        /// <param name="eAutoPay">Entity</param>
        /// <returns>true成功,false失敗</returns>
        public static bool AddEntity(EntityAUTO_PAY eAutoPay)
        {
            try
            {
                return eAutoPay.DB_InsertEntity();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
