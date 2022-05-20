//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：SHOP_PCAM資料庫業務類

//*  創建日期：2009/11/02
//*  修改記錄：

//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using System.Data;
using System.Data.SqlClient;




namespace CSIPKeyInGUI.BusinessRules
{
    /// <summary>
    /// SHOP_PCAM業務類
    /// </summary>
    public class BRSHOP_PCAM : CSIPCommonModel.BusinessRules.BRBase<EntitySHOP_PCAM>
    {

        public const string UPDATE_2Key = @"update shop_pcam set send3270 = 'Y'  where merchant_nbr = @Shop_No  and send3270 <>'Y' ";

        /// <summary>
        /// 根據商店代號查詢資料
        /// </summary>
        /// <param name="strMerchantNumber">商店代號</param>
        /// <returns>EntitySHOP_PCAM</returns>
        public static EntitySet<EntitySHOP_PCAM> SelectEntitySet(string strMerchantNumber)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntitySHOP_PCAM.M_merchant_nbr, Operator.Equal, DataTypeUtils.String, strMerchantNumber);

                return BRSHOP_PCAM.Search(sSql.GetFilterCondition());
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根據商店代號和keyin_flag查詢資料
        /// </summary>
        /// <param name="strMerchantNumber">商店代號</param>
        /// <param name="strKeyinFlag">Key 入標示</param>
        /// <returns>EntitySHOP_PCAM</returns>
        public static EntitySet<EntitySHOP_PCAM> SelectEntitySet(string strMerchantNumber,string strKeyinFlag)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();

                sSql.AddCondition(EntitySHOP_PCAM.M_merchant_nbr, Operator.Equal, DataTypeUtils.String, strMerchantNumber);
                sSql.AddCondition(EntitySHOP_PCAM.M_keyin_flag, Operator.Equal, DataTypeUtils.String, strKeyinFlag);
                sSql.AddCondition(EntitySHOP_PCAM.M_send3270, Operator.NotEqual, DataTypeUtils.String, "Y");

                return BRSHOP_PCAM.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根據商店代號和keyin_flag查詢資料
        /// </summary>
        /// <param name="strMerchantNumber">商店代號</param>
        /// <param name="strKeyinFlag">Key 入標示</param>
        /// <returns>EntitySHOP_PCAM</returns>
        public static EntitySet<EntitySHOP_PCAM> SelectData(string strMerchantNumber, string strKeyinFlag)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();

                sSql.AddCondition(EntitySHOP_PCAM.M_merchant_nbr, Operator.Equal, DataTypeUtils.String, strMerchantNumber);
                sSql.AddCondition(EntitySHOP_PCAM.M_keyin_flag, Operator.Equal, DataTypeUtils.String, strKeyinFlag);

                return BRSHOP_PCAM.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根據商店代號更新SHOP_PCAM
        /// </summary>
        /// <param name="eShopPcam">實體</param>
        /// <param name="strMerchantNumber">商店代號</param>
        /// <param name="strFields">要更新列的數組</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Update(EntitySHOP_PCAM eShopPcam, string strMerchantNumber, string[] strFields)
        {
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntitySHOP_PCAM.M_merchant_nbr, Operator.Equal, DataTypeUtils.String, strMerchantNumber);
            
            return BRSHOP_PCAM.UpdateEntityByCondition(eShopPcam, sSql.GetFilterCondition(), strFields);
        }

        /// <summary>
        /// 根據商店代號、統一編號更新SHOP_PCAM
        /// </summary>
        /// <param name="eShopPcam">實體</param>
        /// <param name="strMerchantNumber">商店代號</param>
        /// <param name="strFields">要更新列的數組</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Update(EntitySHOP_PCAM eShopPcam, string strMerchantNumber, string strCorp, string[] strFields)
        {
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntitySHOP_PCAM.M_merchant_nbr, Operator.Equal, DataTypeUtils.String, strMerchantNumber);
            sSql.AddCondition(EntitySHOP_PCAM.M_corp, Operator.Equal, DataTypeUtils.String, strCorp);

            return BRSHOP_PCAM.UpdateEntityByCondition(eShopPcam, sSql.GetFilterCondition(), strFields);
        }


        /// <summary>
        /// 更新SHOP_PCAM
        /// </summary>
        /// <param name="EntitySHOP_PCAM">實體</param>
        /// <param name="strUniNo1">統一編號（1）</param>
        /// <param name="strUniNo2">統一編號（2）</param>
        /// <param name="strFields">要更新列的數組</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Update(EntitySHOP_PCAM eShopPcam, string strMerchantNumber, string strKeyInFlag)
        {
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntitySHOP_PCAM.M_merchant_nbr, Operator.Equal, DataTypeUtils.String, strMerchantNumber);
            sSql.AddCondition(EntitySHOP_PCAM.M_keyin_flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
            sSql.AddCondition(EntitySHOP_PCAM.M_send3270, Operator.NotEqual, DataTypeUtils.String, "Y");


            return eShopPcam.DB_UpdateEntity(sSql.GetFilterCondition());
        }


        /// <summary>
        /// 二Key後更新Keyin資料
        /// </summary>
        /// <param name="strReceive_Number">收件編號</param>
        /// <param name="strCusID">異動身分證號碼</param>
        /// <returns>true 成功， false失敗</returns>
        public static bool Update2KeyInfo(string strShopNO)
        {

            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandText = UPDATE_2Key;
            sqlComm.CommandType = CommandType.Text;

            SqlParameter parmPayWay = new SqlParameter("@Shop_No", strShopNO);
            sqlComm.Parameters.Add(parmPayWay);


            return BRSHOP_PCAM.Update(sqlComm);

        }

        /// <summary>
        /// 用Entity方式插入資料庫
        /// </summary>
        /// <param name="eShopPcam">Entity</param>
        /// <returns>true成功,false失敗</returns>
        public static bool AddEntity(EntitySHOP_PCAM eShopPcam)
        {
            try
            {
                return eShopPcam.DB_InsertEntity();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
