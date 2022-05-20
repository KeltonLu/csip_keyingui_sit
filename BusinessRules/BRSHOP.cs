//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：SHOP資料庫業務類

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

namespace CSIPKeyInGUI.BusinessRules
{
    /// <summary>
    /// SHOP資料庫業務類
    /// </summary>
    public class BRSHOP : CSIPCommonModel.BusinessRules.BRBase<EntitySHOP>
    {
        #region SQL

        public const string SEL_SHOP_INFO = @"SELECT {0} FROM SHOP WHERE ";

        #endregion

        /// <summary>
        /// 查詢資料庫信息
        /// </summary>
        /// <param name="strShopId">商店代號</param>
        /// <param name="strKeyInFlag">Keyin類別(1-一KEY/2-二KEY)</param>
        /// <param name="strShopType">功能畫面編號</param>
        /// <param name="strColumns">要查詢得到的欄位</param>
        /// <returns>DataSet</returns>
        public static DataSet Select(string strShopId, string strKeyInFlag, string strShopType, string strColumns)
        {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntitySHOP.M_shop_id, Operator.Equal, DataTypeUtils.String, strShopId);
                sSql.AddCondition(EntitySHOP.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
                sSql.AddCondition(EntitySHOP.M_shop_type, Operator.Equal, DataTypeUtils.String, strShopType);

                string strSqlCmd = string.Format(SEL_SHOP_INFO, strColumns) + sSql.GetFilterCondition().Substring(4, sSql.GetFilterCondition().Length - 4);

                return BRSHOP.SearchOnDataSet(strSqlCmd);   
        }

        /// <summary>
        /// 查詢資料庫一、二KEY信息
        /// </summary>
        /// <param name="strShopId">商店代號</param>
        /// <param name="strKeyInFlag">Keyin類別(1-一KEY/2-二KEY)</param>
        /// <param name="strShopType">功能畫面編號</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntitySHOP> SelectEntitySet(string strShopId, string strKeyInFlag, string strShopType)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntitySHOP.M_shop_id, Operator.Equal, DataTypeUtils.String, strShopId);
                sSql.AddCondition(EntitySHOP.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
                sSql.AddCondition(EntitySHOP.M_shop_type, Operator.Equal, DataTypeUtils.String, strShopType);

                return BRSHOP.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 查詢資料庫商店類型信息
        /// </summary>
        /// <param name="strShopId">商店代號</param>
        /// <param name="strShopType">功能畫面編號</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntitySHOP> SelectShopTypeEntitySet(string strShopId, string strShopType)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntitySHOP.M_shop_id, Operator.Equal, DataTypeUtils.String, strShopId);
                sSql.AddCondition(EntitySHOP.M_shop_type, Operator.Equal, DataTypeUtils.String, strShopType);

                return BRSHOP.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 添加SHOP表資料
        /// </summary>
        /// <param name="eShop">SHOP實體</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Add(EntitySHOP eShop)
        {
            return BRSHOP.AddNewEntity(eShop);        
        }

        /// <summary>
        /// 修改SHOP表資料
        /// </summary>
        /// <param name="eShop">SHOP實體</param>
        /// <param name="strCondition">條件</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Update(EntitySHOP eShop, string strCondition)
        {
            return BRSHOP.UpdateEntity(eShop, strCondition);         
        }

        /// <summary>
        /// 根據商店代號、一二KEY標識、功能畫面編號刪除SHOP表資料
        /// </summary>
        /// <param name="eShop">SHOP實體</param>
        /// <param name="strShopId">商店代號</param>
        /// <param name="strKeyInFlag">Keyin類別(1-一KEY/2-二KEY)</param>
        /// <param name="strShopType">功能畫面編號</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Delete(EntitySHOP eShop, string strShopId, string strKeyInFlag, string strShopType)
        {
            SqlHelper Sql = new SqlHelper();

            Sql.AddCondition(EntitySHOP.M_shop_id, Operator.Equal, DataTypeUtils.String, strShopId);
            Sql.AddCondition(EntitySHOP.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
            Sql.AddCondition(EntitySHOP.M_shop_type, Operator.Equal, DataTypeUtils.String, strShopType);

            try
            {
                eShop.DB_DeleteEntity(Sql.GetFilterCondition());
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 根據商店代號、功能畫面編號刪除SHOP表資料
        /// </summary>
        /// <param name="eShop">SHOP實體</param>
        /// <param name="strShopId">商店代號</param>
        /// <param name="strShopType">功能畫面編號</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Delete(EntitySHOP eShop, string strShopId, string strShopType)
        {
            SqlHelper Sql = new SqlHelper();

            Sql.AddCondition(EntitySHOP.M_shop_id, Operator.Equal, DataTypeUtils.String, strShopId);
            Sql.AddCondition(EntitySHOP.M_shop_type, Operator.Equal, DataTypeUtils.String, strShopType);

            return BRSHOP.DeleteEntityByCondition(eShop, Sql.GetFilterCondition());       
        }

        /// <summary>
        /// 先刪除SHOP表資料后再新增
        /// </summary>
        /// <param name="eShop">SHOP實體</param>
        /// <param name="strShopId">商店代號</param>
        /// <param name="strKeyInFlag">Keyin類別(1-一KEY/2-二KEY)</param>
        /// <param name="strShopType">功能畫面編號</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Insert(EntitySHOP eShop, string strShopId, string strKeyInFlag, string strShopType)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (!Delete(eShop, strShopId, strKeyInFlag, strShopType))
                    {
                        return false;
                    }

                    if (!Add(eShop))
                    {
                        return false;
                    }
                    ts.Complete();
                    return true;
                }
            }
            catch
            {
                return false;
            }         
        }
    }
}
