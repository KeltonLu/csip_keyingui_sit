//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：SHOP_6063業務類

//*  創建日期：2009/11/12
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
    /// <summary>
    /// SHOP_6063業務類
    /// </summary>
    public class BRSHOP_6063 : CSIPCommonModel.BusinessRules.BRBase<EntitySHOP_6063>
    {
        #region SQL

        public const string SEL_SHOP6063_INFO = @"SELECT SHOP_NO,CARD_TYPE,IDENTIFY_NO,FAVOUR_FEE,BATCH_DEPICT FROM  SHOP_6063 WHERE SHOP_NO =@SHOP_NO AND SEND3270 = @SEND3270";
        public const string SEL_SHOP6063 = @"SELECT SHOP_NO,CARD_TYPE,IDENTIFY_NO,FAVOUR_FEE,BATCH_DEPICT,KEYIN_USERID FROM  SHOP_6063 WHERE SHOP_NO =@SHOP_NO AND SEND3270 = @SEND3270  and  keyin_flag=@KEYIN ";
        public const string SEL_SHOP = @"SELECT *  FROM  SHOP_6063 WHERE SHOP_NO =@SHOP_NO AND SEND3270 = @SEND3270  and  keyin_flag=@KEYIN  and card_type=@CardType and  identify_no=@IDNO";
        public const string UPDATE_SHOP = @"UPDATE  SHOP_6063  SET SEND3270='Y' WHERE SHOP_NO =@SHOP_NO AND SEND3270 = @SEND3270   and card_type=@CardType and  identify_no=@IDNO";

        #endregion


        /// <summary>
        /// 二Key後更新Keyin資料
        /// </summary>
        /// <param name="strShop">商店代號</param>
        /// <param name="strSend3270">異動標志</param>
        /// <param name="strCardType">Card Type</param>
        /// <param name="identify_no">IdNo</param>
        /// <returns>true 成功， false失敗</returns>
        public static bool Update2KeyInfo(string strShop,string strSend3270, string strCardType,string strIdNo)
        {

            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandText = UPDATE_SHOP;
            sqlComm.CommandType = CommandType.Text;

            SqlParameter parmShopNo= new SqlParameter("@Shop_No", strShop);
            sqlComm.Parameters.Add(parmShopNo);

            SqlParameter parmSend3270 = new SqlParameter("@SEND3270", strSend3270);
            sqlComm.Parameters.Add(parmSend3270);

            SqlParameter parmCardType = new SqlParameter("@CardType", strCardType);
            sqlComm.Parameters.Add(parmCardType);

            SqlParameter parmIDNO = new SqlParameter("@IDNO", strIdNo);
            sqlComm.Parameters.Add(parmIDNO);


            return BROTHER_BANK_TEMP.Update(sqlComm);

        }

        /// <summary>
        /// 刪除選中資料
        /// </summary>
        /// <param name="strShopNo">特店編號</param>
        /// <param name="strCardType">Card-Type</param>
        /// <param name="strIdentifyNo">認同代號</param>
        /// <returns>true成功,false失敗</returns>
        public static bool Delete(string strShopNo, string strCardType, string strIdentifyNo)
        {
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntitySHOP_6063.M_shop_no, Operator.Equal, DataTypeUtils.String, strShopNo);
            sSql.AddCondition(EntitySHOP_6063.M_card_type, Operator.Equal, DataTypeUtils.String, strCardType);
            sSql.AddCondition(EntitySHOP_6063.M_identify_no, Operator.Equal, DataTypeUtils.String, strIdentifyNo);

            EntitySHOP_6063 eShop = new EntitySHOP_6063();
            try
            {
                eShop.DB_DeleteEntity(sSql.GetFilterCondition());
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        /// <summary>
        /// 刪除選中資料
        /// </summary>
        /// <param name="strShopNo">特店編號</param>
        /// <param name="strCardType">Card-Type</param>
        /// <param name="strIdentifyNo">認同代號</param>
        /// <returns>true成功,false失敗</returns>
        public static bool DeleteInfo(string strShopNo, string strCardType, string strIdentifyNo)
        {
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntitySHOP_6063.M_shop_no, Operator.Equal, DataTypeUtils.String, strShopNo);
            sSql.AddCondition(EntitySHOP_6063.M_card_type, Operator.Equal, DataTypeUtils.String, strCardType);
            sSql.AddCondition(EntitySHOP_6063.M_identify_no, Operator.Equal, DataTypeUtils.String, strIdentifyNo);
            sSql.AddCondition(EntitySHOP_6063.M_send3270, Operator.Equal, DataTypeUtils.String, "N");

            EntitySHOP_6063 eShop = new EntitySHOP_6063();
            try
            {
                eShop.DB_DeleteEntity(sSql.GetFilterCondition());
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        /// <summary>
        /// 刪除選中資料
        /// </summary>
        /// <param name="strShopNo">特店編號</param>
        /// <param name="strCardType">Card-Type</param>
        /// <param name="strIdentifyNo">認同代號</param>
        /// <returns>true成功,false失敗</returns>
        public static bool Delete(string strShopNo, string strCardType, string strIdentifyNo,string strKeyin)
        {
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntitySHOP_6063.M_shop_no, Operator.Equal, DataTypeUtils.String, strShopNo);
            sSql.AddCondition(EntitySHOP_6063.M_card_type, Operator.Equal, DataTypeUtils.String, strCardType);
            sSql.AddCondition(EntitySHOP_6063.M_identify_no, Operator.Equal, DataTypeUtils.String, strIdentifyNo);
            sSql.AddCondition(EntitySHOP_6063.M_keyin_flag, Operator.Equal, DataTypeUtils.String, strKeyin);
            sSql.AddCondition(EntitySHOP_6063.M_send3270, Operator.Equal, DataTypeUtils.String, "N");

            EntitySHOP_6063 eShop = new EntitySHOP_6063();
            try
            {
                eShop.DB_DeleteEntity(sSql.GetFilterCondition());
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        /// <summary>
        ///根據特店編號、上傳標識查詢資料庫
        /// </summary>
        /// <param name="strShopNo">特店編號</param>
        /// <param name="strSend3270">上傳標識</param>
        /// <returns>DataSet</returns>
        public static DataSet Select(string strShopNo, string strSend3270)
        {
                SqlCommand sqlComm = new SqlCommand();
                sqlComm.CommandText = SEL_SHOP6063_INFO;
                sqlComm.CommandType = CommandType.Text;

                SqlParameter parmShopNo = new SqlParameter("@SHOP_NO", strShopNo);
                sqlComm.Parameters.Add(parmShopNo);
                SqlParameter parmSend3270 = new SqlParameter("@SEND3270", strSend3270);
                sqlComm.Parameters.Add(parmSend3270);

                return BRSHOP_6063.SearchOnDataSet(sqlComm);
        }


        /// <summary>
        ///根據特店編號、上傳標識查詢資料庫
        /// </summary>
        /// <param name="strShopNo">特店編號</param>
        /// <param name="strSend3270">上傳標識</param>
        /// <returns>DataSet</returns>
        public static DataSet SelectInfo(string strShopNo, string strSend3270,string strKeyin)
        {
            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandText = SEL_SHOP6063;
            sqlComm.CommandType = CommandType.Text;

            SqlParameter parmShopNo = new SqlParameter("@SHOP_NO", strShopNo);
            sqlComm.Parameters.Add(parmShopNo);
            SqlParameter parmSend3270 = new SqlParameter("@SEND3270", strSend3270);
            sqlComm.Parameters.Add(parmSend3270);
            SqlParameter parmKeyin = new SqlParameter("@KEYIN", strKeyin);
            sqlComm.Parameters.Add(parmKeyin);

            return BRSHOP_6063.SearchOnDataSet(sqlComm);
        }


        /// <summary>
        ///根據特店編號、上傳標識查詢資料庫
        /// </summary>
        /// <param name="strShopNo">特店編號</param>
        /// <param name="strSend3270">上傳標識</param>
        /// <returns>DataSet</returns>
        public static DataSet SelectInfo(string strShopNo, string strSend3270, string strKeyin,string strCardNo,string strIdNo)
        {
            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandText = SEL_SHOP;
            sqlComm.CommandType = CommandType.Text;

            SqlParameter parmShopNo = new SqlParameter("@SHOP_NO", strShopNo);
            sqlComm.Parameters.Add(parmShopNo);
            SqlParameter parmSend3270 = new SqlParameter("@SEND3270", strSend3270);
            sqlComm.Parameters.Add(parmSend3270);
            SqlParameter parmKeyin = new SqlParameter("@KEYIN", strKeyin);
            sqlComm.Parameters.Add(parmKeyin);
            SqlParameter parmCardNo = new SqlParameter("@CardType", strCardNo);
            sqlComm.Parameters.Add(parmCardNo);
            SqlParameter parmIdNo = new SqlParameter("@IDNO", strIdNo);
            sqlComm.Parameters.Add(parmIdNo);

            return BRSHOP_6063.SearchOnDataSet(sqlComm);
        }


        /// <summary>
        /// 用Entity方式插入資料庫
        /// </summary>
        /// <param name="eShop6063">Entity</param>
        /// <returns>true成功,false失敗</returns>
        public static bool AddEntity(EntitySHOP_6063 eShop6063)
        {
            try
            {
                return eShop6063.DB_InsertEntity();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 用Entity方式插入資料庫
        /// </summary>
        /// <param name="eShop6063">Entity</param>
        /// <returns>true成功,false失敗</returns>
        public static bool UpdateEntity(EntitySHOP_6063 eShop6063,string strShopNo,string strKeyin,string strCardType,string strIdNo)
        {
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntitySHOP_6063.M_shop_no, Operator.Equal, DataTypeUtils.String, strShopNo);
            sSql.AddCondition(EntitySHOP_6063.M_keyin_flag, Operator.Equal, DataTypeUtils.String, strKeyin);
            sSql.AddCondition(EntitySHOP_6063.M_card_type, Operator.Equal, DataTypeUtils.String, strCardType);
            sSql.AddCondition(EntitySHOP_6063.M_identify_no, Operator.Equal, DataTypeUtils.String, strIdNo);
            sSql.AddCondition(EntitySHOP_6063.M_send3270, Operator.Equal, DataTypeUtils.String, "N");

            return eShop6063.DB_UpdateEntity(sSql.GetFilterCondition());
        }
    }
}
