//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：SZIP資料庫業務類

//*  創建日期：2009/07/14
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
    /// SZIP資料庫業務類
    /// </summary>
    public class BRSZIP : CSIPCommonModel.BusinessRules.BRBase<EntitySZIP>
    {
        /// <summary>
        /// 得到szip資料庫郵遞區號代碼
        /// </summary>
        /// <param name="strZipData">地址</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntitySZIP> SelectEntitySet(string strZipData)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntitySZIP.M_zip_data, Operator.Equal, DataTypeUtils.String, strZipData);

                return BRSZIP.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
