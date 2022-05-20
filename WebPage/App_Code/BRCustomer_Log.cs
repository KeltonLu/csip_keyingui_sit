//******************************************************************
//*  作    者：蘇洺葳(Grezz)
//*  功能說明：作業量統計表
//*  創建日期：2018/05/03
//*  修改記錄：此檔案為BusinessRules.BRCustomer_Log的新版
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
using Framework.Common.Utility;

namespace CSIPKeyInGUI.BusinessRules_new
{
    /// <summary>
    /// 記錄查詢業務處理類
    /// </summary>
    public class BRCustomer_Log : CSIPCommonModel.BusinessRules.BRBase<EntityCUSTOMER_LOG>
    {
        #region 查詢記錄的sql

        // 包含公用事業(ETAG業務)
        //2021/03/17_Ares_Stanley-DB名稱改為變數
        private const string SEL_CUSTOMER_LOG = @"select A.Log_Flag, C.FUNCTION_NAME, A.field_name, A.before, A.after
                                                    , (case when B.user_name is null then A.user_id else B.user_name end) as user_name
                                                    , A.mod_date, A.mod_time, A.trans_id, substring(A.mod_time, 1, 4) as timetemp 
                                                    from
                                                    (
	                                                    select query_key
	                                                    , case trans_id when 'A01' then '01010100' when 'A02' then '01010800' 
					                                                    when 'A04' then '01010400' when 'A03' then '01010400' 
					                                                    when 'A11' then '01010200' when 'A06' then '01010300' 
					                                                    when 'A14' then '01010600' when 'B01' then '01020100' 
					                                                    when 'B02' then '01020300' when 'B05' then '01020200' 
					                                                    when 'B04' then '01020500' when 'B10' then '01020700' 
					                                                    when 'B14' then '01021300' when 'B12' then '01020900' 
					                                                    when 'C02' then '01030200' when 'D01' then '01030200' 
	                                                    else trans_id end as trans_id
	                                                    , field_name, before, after, user_id, mod_date, mod_time, log_flag from customer_log 
	                                                    Union all 
	                                                    select query_key
	                                                    , case trans_id when 'A01' then '01010100' when 'A02' then '01010800' 
					                                                    when 'A04' then '01010400' when 'A03' then '01010400' 
					                                                    when 'A11' then '01010200' when 'A06' then '01010300' 
					                                                    when 'A14' then '01010600' when 'B01' then '01020100' 
					                                                    when 'B02' then '01020300' when 'B05' then '01020200' 
					                                                    when 'B04' then '01020500' when 'B10' then '01020700' 
					                                                    when 'B14' then '01021300' when 'B12' then '01020900' 
					                                                    when 'C02' then '01030200' when 'D01' then '01030200' 
	                                                    else trans_id end as trans_id
	                                                    , field_name, before, after, user_id, mod_date, mod_time, log_flag 
	                                                    from customer_log_OtherBankTemp
                                                    ) A 
                                                    left join (select distinct user_id, User_name from {0}.dbo.M_USER) B on A.user_id = B.user_id 
                                                    inner join {0}.dbo.M_FUNCTION C with(nolock) on c.function_id = a.trans_id 
                                                    and C.FUNCTION_ID=A.trans_id and C.FUNCTION_KEY='01' 
                                                    where A.query_key = @query_key 
                                                    union all 
                                                    select A.Log_Flag,'公用事業二Key' + '(' + 
                                                    case substring(A.trans_id, 10, 6) when '001001' then 'eTag儲值' 
									                                                    when '001002' then '臨時停車' 
									                                                    when '001003' then '月租停車' 
									                                                    else '' end + ')' as FUNCTION_NAME
                                                    , A.field_name, A.before, A.after
                                                    , (case when B.user_name is null then A.user_id else B.user_name end) as user_name
                                                    , A.mod_date, A.mod_time, A.trans_id, substring(A.mod_time, 1, 4) as timetemp 
                                                    from customer_log a 
                                                    left join {0}.dbo.M_FUNCTION C with(nolock) on c.function_id = a.trans_id 
                                                    left join (select distinct user_id, User_name from {0}.dbo.M_USER) B on A.user_id = B.user_id
                                                    where A.query_key = @query_key and c.function_id is null
                                                    order by mod_date desc, timetemp desc, trans_id, field_name, Log_Flag ";

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
        public static DataTable SearchCustomer_Log(string strKey, int intPageIndex, int intPageSize, ref int intTotolCount, ref string strMsgID)
        {
            //2021/03/17_Ares_Stanley-DB名稱改為變數
            string strSql = string.Format(SEL_CUSTOMER_LOG, UtilHelper.GetAppSettings("DB_CSIP"));
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = strSql;

            SqlParameter parmQuery_key = new SqlParameter("@" + EntityCUSTOMER_LOG.M_query_key, strKey);
            sqlcmd.Parameters.Add(parmQuery_key);

            DataTable dtblCustomer_Log = new DataTable();
            try
            {
                // 查詢記錄
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


        /// <summary>
        /// 2020.05.14 Ray Add
        /// 查詢Auto_pay 資料是否有異動紀錄(電子帳單)
        /// 2020.07.07 (U) Ray 修正改用count 並加 witch (nolock)
        /// </summary>
        /// <param name="strQuery_key"></param>
        /// <param name="strReceive_Number"></param>
        /// <param name="before"></param>
        /// <returns></returns>
        public static bool GetPostToHostAdapter_ebill(string strQuery_key, string strReceive_Number , string before)
        {

            try
            {
                SqlCommand sqlcmd = new SqlCommand();
                //string sql = @" SELECT query_key
                //                      ,trans_id
                //                      ,field_name
                //                      ,before
                //                      ,after
                //                      ,user_id
                //                      ,mod_date
                //                      ,mod_time
                //                      ,log_flag
                //                      ,Receive_Number
                //                 FROM [dbo].[customer_log] 
                //                 WHERE query_key =@Query_key AND Receive_Number=@Receive_Number AND before=@before
                //                 AND field_name ='電子帳單' AND  trans_id='01010800'  
                //                ";

                string sql = @" SELECT count (*) 
                                 FROM [dbo].[customer_log] with (nolock)
                                 WHERE query_key =@Query_key AND Receive_Number=@Receive_Number AND before=@before
                                 AND field_name ='電子帳單' AND  trans_id='01010800'  
                                ";


                sqlcmd.Parameters.Add(new SqlParameter("@Query_key", strQuery_key));
                sqlcmd.Parameters.Add(new SqlParameter("@Receive_Number", strReceive_Number));
                sqlcmd.Parameters.Add(new SqlParameter("@before", before));
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    // 2020.07.07 Ray 應改用count，修正判斷方式
                    //if (ds.Tables[0].Rows.Count != 0)
                    if (ds.Tables[0].Rows[0][0].ToString() != "0")
                    {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
