//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：JOB_STATUS資料庫業務類

//*  創建日期：2009/10/16
//*  修改記錄：

//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;

namespace CSIPKeyInGUI.BusinessRules
{
    
    /// <summary>
    /// JOB_STATUS資料庫業務類
    /// </summary>
    public class BRJOB_STATUS : CSIPCommonModel.BusinessRules.BRBase<EntityJOB_STATUS>
    {
        /// <summary>
        /// 查看是否上次作業還未停止
        /// </summary>
        /// <param name="strJobStatus">作業狀態</param>
        /// <param name="strJobName">作業名稱</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntityJOB_STATUS> SelectEntitySet(string strJobStatus, string strJobName)
        {        
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityJOB_STATUS.M_job_status, Operator.Equal, DataTypeUtils.String, strJobStatus);
                sSql.AddCondition(EntityJOB_STATUS.M_job_name, Operator.Equal, DataTypeUtils.String, strJobName); 

                return BRJOB_STATUS.Search(sSql.GetFilterCondition());
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 修改作業狀態
        /// </summary>
        /// <param name="eJobStatus">實體</param>
        /// <param name="strJobName">作業名稱</param>
        /// <param name="strJobStatus">作業狀態</param>
        /// <param name="strField">更新列的數組</param>
        /// <returns>true 成功， false失敗</returns>
        public static bool Update(EntityJOB_STATUS eJobStatus, string strJobName, string strJobStatus, string[] strField)
        {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityJOB_STATUS.M_job_name, Operator.Equal, DataTypeUtils.String, strJobName);
                sSql.AddCondition(EntityJOB_STATUS.M_job_status, Operator.Equal, DataTypeUtils.String, strJobStatus);

                return BRJOB_STATUS.UpdateEntityByCondition(eJobStatus, sSql.GetFilterCondition(), strField);
        }

        /// <summary>
        /// 用Entity方式插入資料庫
        /// </summary>
        /// <param name="eJobStatus">Entity</param>
        /// <returns>true成功,false失敗</returns>
        public static bool AddEntity(EntityJOB_STATUS eJobStatus)
        {
            try
            {
                return eJobStatus.DB_InsertEntity();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
