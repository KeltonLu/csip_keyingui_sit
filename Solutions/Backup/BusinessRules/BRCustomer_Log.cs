//******************************************************************
//*  作    者：占偉林

//*  功能說明：其他作業-記錄查詢
//*  創建日期：2009/10/22
//*  修改記錄：

//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using CSIPCommonModel.BaseItem;
using CSIPKeyInGUI.EntityLayer;

namespace CSIPKeyInGUI.BusinessRules
{
    /// <summary>
    /// 記錄查詢業務處理類

    /// </summary>
    public class BRCustomer_Log : CSIPCommonModel.BusinessRules.BRBase<EntityCUSTOMER_LOG>
    {
        #region 查詢記錄的sql
        //包含他行Customer_log_OtherBank
        private const string SEL_CUSTOMER_LOG = @"select  A.Log_Flag,C.FUNCTION_NAME,A.field_name,A.before,A.after, " +
            "(case when B.user_name is null then A.user_id else B.user_name end) as user_name,A.mod_date,A.mod_time ,A.trans_id,substring(A.mod_time,1,4) as timetemp " +
            "from (select  query_key," +
                "case trans_id " +
                "when 'A01' then '01010100' " +
                "when 'A02' then '01010800' " +
                "when 'A04' then '01010400' " +
                "when 'A03' then '01010400' " +
                "when 'A11' then '01010200' " +
                "when 'A06' then '01010300' " +
                "when 'A14' then '01010600' " +
                "when 'B01' then '01020100' " +
                "when 'B02' then '01020300' " +
                "when 'B05' then '01020200' " +
                "when 'B04' then '01020500' " +
                "when 'B10' then '01020700' " +
                "when 'B14' then '01021300' " +
                "when 'B12' then '01020900' " +
                "when 'C02' then '01030200' " +
                "when 'D01' then '01030200' " +
                " else trans_id " +
                "end as trans_id," +
                "field_name,before,after,user_id,mod_date,mod_time,log_flag from customer_log " +
                " Union all " +
                "select  query_key," +
                "case trans_id " +
                "when 'A01' then '01010100' " +
                "when 'A02' then '01010800' " +
                "when 'A04' then '01010400' " +
                "when 'A03' then '01010400' " +
                "when 'A11' then '01010200' " +
                "when 'A06' then '01010300' " +
                "when 'A14' then '01010600' " +
                "when 'B01' then '01020100' " +
                "when 'B02' then '01020300' " +
                "when 'B05' then '01020200' " +
                "when 'B04' then '01020500' " +
                "when 'B10' then '01020700' " +
                "when 'B14' then '01021300' " +
                "when 'B12' then '01020900' " +
                "when 'C02' then '01030200' " +
                "when 'D01' then '01030200' " +
                " else trans_id " +
                "end as trans_id," +
                "field_name,before,after,user_id,mod_date,mod_time,log_flag from customer_log_OtherBankTemp " +
                ") " +
                "A LEFT join (select distinct user_id,User_name from csip.dbo.M_USER) B on A.user_id = B.user_id , " +
                "csip.dbo.M_FUNCTION C " +
            //"where A.user_id = B.USER_ID " +
            //"and A.query_key = @query_key and C.FUNCTION_ID=A.trans_id and C.FUNCTION_KEY='01' " +
            "where A.query_key = @query_key and C.FUNCTION_ID=A.trans_id and C.FUNCTION_KEY='01' " +
            "order by mod_date desc,timetemp desc,trans_id,field_name,Log_Flag ";

        #endregion 
        
        /// <summary>
        /// 查詢記錄
        /// </summary>
        /// <param name="strKey">關鍵字</param>
        /// <param name="intPageIndex">當前頁號</param>
        /// <param name="intPageSize">每頁顯示記錄條數</param>
        /// <param name="intTotolCount">記錄總條數</param>
        /// <param name="strMsgID">查詢記錄出錯時，返回錯誤ID號</param>
        /// <returns>查詢結果:記錄DataTable</returns>
        public static DataTable SearchCustomer_Log(string strKey, int intPageIndex, int intPageSize, 
                ref int intTotolCount, ref string strMsgID)
        {
            string strSql = SEL_CUSTOMER_LOG;
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = strSql;
            //* 傳入關鍵字變量

            SqlParameter parmQuery_key = new SqlParameter("@" + EntityCUSTOMER_LOG.M_query_key, strKey);
            sqlcmd.Parameters.Add(parmQuery_key);

            DataTable dtblCustomer_Log = new DataTable();
            try
            {
                //* 查詢記錄
                dtblCustomer_Log = BRCustomer_Log.SearchOnDataSet(sqlcmd, intPageIndex, intPageSize, ref intTotolCount).Tables[0];
                return dtblCustomer_Log;
            }
            catch (Exception exp)
            {
                BRCustomer_Log.SaveLog(exp);
                strMsgID = "00_00000000_000";
                return null;
            }
        }

        /// <summary>
        /// 用Entity方式插入資料庫
        /// </summary>
        /// <param name="eCustomerLog">Entity</param>
        /// <returns>true成功,false失敗</returns>
        public static bool AddEntity(EntityCUSTOMER_LOG eCustomerLog)
        {
            try
            {
                return eCustomerLog.DB_InsertEntity();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
