//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：SHOP_UPLOAD資料庫信息

//*  創建日期：2009/07/13
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
    public class BRSHOP_UPLOAD : CSIPCommonModel.BusinessRules.BRBase<EntitySHOP_UPLOAD>
    {
        /// <summary>
        /// 用Entity方式插入資料庫
        /// </summary>
        /// <param name="eShopUpload">Entity</param>
        /// <returns>true成功,false失敗</returns>
        public static bool AddEntity(EntitySHOP_UPLOAD eShopUpload)
        {
            try
            {
                return eShopUpload.DB_InsertEntity();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
