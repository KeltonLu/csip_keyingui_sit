//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：SHOP_RPT資料庫業務類

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
    /// SHOP_RPT資料庫業務類
    /// </summary>
    public class BRSHOP_RPT : CSIPCommonModel.BusinessRules.BRBase<EntitySHOP_RPT>
    {
        /// <summary>
        /// 添加SHOP_RPT表資料
        /// </summary>
        /// <param name="eShopRpt">SHOP_RPT實體</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Add(EntitySHOP_RPT eShopRpt)
        {
            return BRSHOP_RPT.AddNewEntity(eShopRpt);             
        }

        /// <summary>
        /// 查詢SHOP_RPT表資料
        /// </summary>
        /// <param name="strShopId">商店代號</param>
        /// <param name="strKeyInFlag">一二KEY</param>
        /// <param name="strType">類型</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntitySHOP_RPT> SelectEntitySet(string strShopId, string strKeyInFlag, string strType)
        {          
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntitySHOP_RPT.M_shop_id, Operator.Equal, DataTypeUtils.String, strShopId);
                sSql.AddCondition(EntitySHOP_RPT.M_type, Operator.Equal, DataTypeUtils.String, strType);

                return BRSHOP_RPT.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 用Entity方式插入資料庫
        /// </summary>
        /// <param name="eShopRpt">Entity</param>
        /// <returns>true成功,false失敗</returns>
        public static bool AddEntity(EntitySHOP_RPT eShopRpt)
        {
            try
            {
                return eShopRpt.DB_InsertEntity();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
