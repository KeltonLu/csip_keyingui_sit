//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：SHOP_1KEYLOG資料庫業務類

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
    /// SHOP_1KEYLOG資料庫業務類
    /// </summary>
    public class BRSHOP_1KEYLOG : CSIPCommonModel.BusinessRules.BRBase<EntitySHOP_1KEYLOG>
    {
        /// <summary>
        /// 添加SHOP_1KEYLOG表資料
        /// </summary>
        /// <param name="eShop1KeyLog">SHOP_1KEYLOG實體</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Add(EntitySHOP_1KEYLOG eShop1KeyLog)
        {
            return BRSHOP_1KEYLOG.AddNewEntity(eShop1KeyLog);                     
        }

        /// <summary>
        /// 用Entity方式插入資料庫
        /// </summary>
        /// <param name="eShop1KeyLog">Entity</param>
        /// <returns>true成功,false失敗</returns>
        public static bool AddEntity(EntitySHOP_1KEYLOG eShop1KeyLog)
        {
            try
            {
                return eShop1KeyLog.DB_InsertEntity();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
