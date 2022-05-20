//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：ACH_RTN_INFO資料庫業務類

//*  創建日期：2009/10/08
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
    /// ACH_RTN_INFO資料庫業務類
    /// </summary>
    public class BRACH_RTN_INFO : CSIPCommonModel.BusinessRules.BRBase<EntityACH_RTN_INFO>
    {
        /// <summary>
        /// 查詢ACH回覆碼中文訊息
        /// </summary>
        /// <param name="strAchRtnCode">ACH回覆碼</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntityACH_RTN_INFO> SelectEntitySet(string strAchRtnCode)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityACH_RTN_INFO.M_Ach_Rtn_Code, Operator.Equal, DataTypeUtils.String, strAchRtnCode);

                return BRACH_RTN_INFO.Search(sSql.GetFilterCondition());
            }
            catch(Exception ex)
            {
                BRACH_RTN_INFO.SaveLog(ex);
                throw ex;
            }
        }
    }
}
