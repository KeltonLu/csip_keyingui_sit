//******************************************************************
//*  作    者：Ares Dennis
//*  功能說明：AML_CHECKLOG資料庫業務類

//*  創建日期：2021/05/27
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
    /// AML_CHECKLOG資料庫業務類
    /// </summary>
    public class BRAML_CHECKLOG : CSIPCommonModel.BusinessRules.BRBase<EntityAML_CHECKLOG>
    {
        /// <summary>
        /// 添加AML_CHECKLOG表資料
        /// </summary>
        /// <param name="eAMLCheckLog">AML_CHECKLOG實體</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Add(EntityAML_CHECKLOG eAMLCheckLog)
        {
            return BRAML_CHECKLOG.AddNewEntity(eAMLCheckLog);
        }

        /// <summary>
        /// 用Entity方式插入資料庫
        /// </summary>
        /// <param name="eAMLCheckLog">Entity</param>
        /// <returns>true成功,false失敗</returns>
        public static bool AddEntity(EntityAML_CHECKLOG eAMLCheckLog)
        {
            try
            {
                return eAMLCheckLog.DB_InsertEntity();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
