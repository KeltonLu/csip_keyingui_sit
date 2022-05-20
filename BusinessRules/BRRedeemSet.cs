//******************************************************************
//*  作    者：Ian Huang
//*  功能說明：RedeemSet資料庫業務類
//*  創建日期：2010/06/25
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
using System.Data.SqlClient;

namespace CSIPKeyInGUI.BusinessRules
{
    public class BRRedeemSet : CSIPCommonModel.BusinessRules.BRBase<EntityRedeemSet>
    {
        #region sql
        public const string SQL_SQL_REPORT = @"select RECEIVE_NUMBER,'Redeem' as WTYPE,USER_ID,KEYIN_FLAG,USE_TIME,ISSAME,DIFF_NUM,EDIT_USER_ID from RedeemSet 
                                                where RECEIVE_NUMBER in
                                                (select RECEIVE_NUMBER from RedeemSet 
                                                where RECEIVE_NUMBER in
                                                (select RECEIVE_NUMBER from RedeemSet 
                                                WHERE UPDATE_DATE between @SDATE and @EDATE and KEYIN_FLAG = '1')
                                                and KEYIN_FLAG = '2' and SEND3270 = 'N')
                                                union all
                                                select RECEIVE_NUMBER,'Award' as WTYPE,USER_ID,KEYIN_FLAG,USE_TIME,ISSAME,DIFF_NUM,EDIT_USER_ID from AwardSet
                                                where RECEIVE_NUMBER in
                                                (select RECEIVE_NUMBER from AwardSet 
                                                where RECEIVE_NUMBER in
                                                (select RECEIVE_NUMBER from AwardSet 
                                                WHERE UPDATE_DATE between @SDATE and @EDATE and KEYIN_FLAG = '1')
                                                and KEYIN_FLAG = '2' and SEND3270 = 'N')
                                                union all
                                                select distinct RECEIVE_NUMBER,'Redeem' as WTYPE,USER_ID,KEYIN_FLAG,USE_TIME,ISSAME,DIFF_NUM,EDIT_USER_ID from RedeemUpload
                                                where RECEIVE_NUMBER in
                                                (select RECEIVE_NUMBER from RedeemUpload 
                                                where RECEIVE_NUMBER in
                                                (select RECEIVE_NUMBER from RedeemUpload 
                                                WHERE UPDATE_DATE between @SDATE and @EDATE and KEYIN_FLAG = '1')
                                                and KEYIN_FLAG = '2' and SEND3270 = 'N') order by RECEIVE_NUMBER,KEYIN_FLAG";
        /// <summary>
        /// 修改紀錄:2021/02/01_Ares_Stanley-收件日期調整為區間;  2021/04/10_Ares_Stanley-增加排序
        /// </summary>
        public const string SQL_SQL_FILE_DETAIL_HASMER = @"select a.RECEIVE_NUMBER,a.ReceiveDate+' '+a.ReceiveTime as RTime,a.USER_ID as USER_ID1,a.SubmitDate+' '+a.SubmitTime as STime1,
                                                            b.USER_ID as USER_ID2,b.SubmitDate+' '+b.SubmitTime as STime2,b.ISSAME 
                                                            from RedeemSet a 
                                                            inner join RedeemSet b on a.RECEIVE_NUMBER = b.RECEIVE_NUMBER
                                                            where a.RECEIVE_NUMBER like @RECEIVE_NUMBER and a.ReceiveDate BETWEEN @ReceiveDateStart AND @ReceiveDateEnd and a.KEYIN_FLAG = '1' and 
                                                            b.RECEIVE_NUMBER like @RECEIVE_NUMBER and b.KEYIN_FLAG = '2' and 
                                                            (a.MERCHANT_NO1 like @MERCHANT_NO or
                                                            a.MERCHANT_NO2 like @MERCHANT_NO or
                                                            a.MERCHANT_NO3 like @MERCHANT_NO or
                                                            a.MERCHANT_NO4 like @MERCHANT_NO or
                                                            a.MERCHANT_NO5 like @MERCHANT_NO or
                                                            a.MERCHANT_NO6 like @MERCHANT_NO or
                                                            a.MERCHANT_NO7 like @MERCHANT_NO or
                                                            a.MERCHANT_NO8 like @MERCHANT_NO or
                                                            a.MERCHANT_NO9 like @MERCHANT_NO or
                                                            a.MERCHANT_NO10 like @MERCHANT_NO) ORDER BY RTime";
        /// <summary>
        /// 修改紀錄:2021/02/01_Ares_Stanley-收件日期調整為區間; 2021/04/10_Ares_Stanley-增加排序
        /// </summary>
        public const string SQL_SQL_FILE_DETAIL_NOMER = @"select a.RECEIVE_NUMBER,a.ReceiveDate+' '+a.ReceiveTime as RTime,a.USER_ID as USER_ID1,a.SubmitDate+' '+a.SubmitTime as STime1,
                                                        b.USER_ID as USER_ID2,b.SubmitDate+' '+b.SubmitTime as STime2,b.ISSAME 
                                                        from RedeemSet a 
                                                        inner join RedeemSet b on a.RECEIVE_NUMBER = b.RECEIVE_NUMBER
                                                        where a.RECEIVE_NUMBER like @RECEIVE_NUMBER and a.ReceiveDate BETWEEN @ReceiveDateStart AND @ReceiveDateEnd and a.KEYIN_FLAG = '1' and 
                                                        b.RECEIVE_NUMBER like @RECEIVE_NUMBER and b.KEYIN_FLAG = '2' 
                                                        union all
                                                        select a.RECEIVE_NUMBER,a.ReceiveDate+' '+a.ReceiveTime as RTime,a.USER_ID as USER_ID1,a.SubmitDate+' '+a.SubmitTime as STime1,
                                                        b.USER_ID as USER_ID2,b.SubmitDate+' '+b.SubmitTime as STime2,b.ISSAME 
                                                        from AwardSet a 
                                                        inner join AwardSet b on a.RECEIVE_NUMBER = b.RECEIVE_NUMBER
                                                        where a.RECEIVE_NUMBER like @RECEIVE_NUMBER and a.ReceiveDate BETWEEN @ReceiveDateStart AND @ReceiveDateEnd and a.KEYIN_FLAG = '1' and 
                                                        b.RECEIVE_NUMBER like @RECEIVE_NUMBER and b.KEYIN_FLAG = '2' ORDER BY RTime";
        #endregion sql

        public static EntitySet<EntityRedeemSet> Select(string strReceiveNumber)
        {
            try
            {
                SqlHelper sqlh = new SqlHelper();
                sqlh.AddCondition(EntityRedeemSet.M_RECEIVE_NUMBER, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
                sqlh.AddCondition(EntityRedeemSet.M_KEYIN_FLAG, Operator.Equal, DataTypeUtils.String, "1");

                return BRRedeemSet.Search(sqlh.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static EntitySet<EntityRedeemSet> SelectData(string strReceiveNumber, string strKEYINFLAG)
        {
            try
            {
                SqlHelper sqlh = new SqlHelper();
                sqlh.AddCondition(EntityRedeemSet.M_RECEIVE_NUMBER, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
                sqlh.AddCondition(EntityRedeemSet.M_KEYIN_FLAG, Operator.Equal, DataTypeUtils.String, strKEYINFLAG);

                return BRRedeemSet.Search(sqlh.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static bool UpdateKey1(string strReceiveNumber, EntityRedeemSet eRS, EntitySet<EntityRedeemSet_ATypeSet> esRS_ATS)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    SqlHelper sqlh = new SqlHelper();
                    sqlh.AddCondition(EntityRedeemSet.M_RECEIVE_NUMBER, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
                    sqlh.AddCondition(EntityRedeemSet.M_KEYIN_FLAG, Operator.Equal, DataTypeUtils.String, "1");

                    EntityRedeemSet eRSK1 = new EntityRedeemSet();
                    eRSK1 = BRRedeemSet.SelectData(strReceiveNumber, "1").GetEntity(0);

                    eRS.KEYIN_DATE = eRSK1.KEYIN_DATE;

                    if (!BRRedeemSet.UpdateEntity(eRS, sqlh.GetFilterCondition()))
                    {
                        return false;
                    }

                    // 刪除 RedeemSet_ATypeSet 表中，對應收編的所有的 CardType 資料
                    if (!BRRedeemSet_ATypeSet.DelAll(strReceiveNumber, "1"))
                    {
                        return false;
                    }

                    // 從新添加 RedeemSet_ATypeSet 表中，對應收編的所有的 CardType 資料
                    if (!BRRedeemSet_ATypeSet.AddBatch(esRS_ATS))
                    {
                        return false;
                    }

                    ts.Complete();
                }

                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool InsertKey1(EntityRedeemSet eRS, EntitySet<EntityRedeemSet_ATypeSet> esRS_ATS)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (!BRRedeemSet.AddNewEntity(eRS))
                    {
                        return false;
                    }

                    // 添加 RedeemSet_ATypeSet 表中，對應收編的所有的 CardType 資料
                    if (!BRRedeemSet_ATypeSet.AddBatch(esRS_ATS))
                    {
                        return false;
                    }

                    ts.Complete();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool InsertKey2(EntityRedeemSet eRS, EntitySet<EntityRedeemSet_ATypeSet> esRS_ATS, EntitySet<EntityRedeem3270> esR3270)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (!BRRedeemSet.AddNewEntity(eRS))
                    {
                        return false;
                    }

                    // 添加 RedeemSet_ATypeSet 表中，對應收編的所有的 CardType 資料
                    if (!BRRedeemSet_ATypeSet.AddBatch(esRS_ATS))
                    {
                        return false;
                    }

                    //if ("N" == eRS.SEND3270)
                    if ("Y" == eRS.ISSAME)
                    {
                        // 添加 RedeemSet_ATypeSet 表中，對應收編的所有的 CardType 資料
                        if (!BRRedeem3270.AddBatch(esR3270))
                        {
                            return false;
                        }
                    }

                    ts.Complete();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool UpdateKey2(string strReceiveNumber, EntityRedeemSet eRS, EntitySet<EntityRedeemSet_ATypeSet> esRS_ATS, EntitySet<EntityRedeem3270> esR3270)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    EntityRedeemSet eRSK2 = new EntityRedeemSet();
                    eRSK2 = BRRedeemSet.SelectData(strReceiveNumber, "2").GetEntity(0);

                    SqlHelper sqlh = new SqlHelper();
                    sqlh.AddCondition(EntityRedeemSet.M_RECEIVE_NUMBER, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
                    sqlh.AddCondition(EntityRedeemSet.M_KEYIN_FLAG, Operator.Equal, DataTypeUtils.String, "2");

                    if (!BRRedeemSet.UpdateEntity(eRS, sqlh.GetFilterCondition()))
                    {
                        return false;
                    }

                    // 刪除 RedeemSet_ATypeSet 表中，對應收編的所有的 CardType 資料
                    if (!BRRedeemSet_ATypeSet.DelAll(strReceiveNumber, "2"))
                    {
                        return false;
                    }

                    // 從新添加 RedeemSet_ATypeSet 表中，對應收編的所有的 CardType 資料
                    if (!BRRedeemSet_ATypeSet.AddBatch(esRS_ATS))
                    {
                        return false;
                    }

                    //if ("N" == eRS.SEND3270)
                    if ("Y" == eRS.ISSAME)
                    {
                        // 添加 RedeemSet_ATypeSet 表中，對應收編的所有的 CardType 資料
                        if (!BRRedeem3270.AddBatch(esR3270))
                        {
                            return false;
                        }
                    }

                    ts.Complete();
                }

                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static DataTable SelectReport(string strSDATE, string strEDATE, int iPageIndex, int iPageSize, ref int iTotalCount)
        {
            try
            {
                SqlCommand sc = new SqlCommand();
                sc.CommandType = CommandType.Text;
                sc.CommandText = SQL_SQL_REPORT;

                SqlParameter spS = new SqlParameter("SDATE", strSDATE);
                sc.Parameters.Add(spS);

                SqlParameter spE = new SqlParameter("EDATE", strEDATE);
                sc.Parameters.Add(spE);

                DataTable dt = new DataTable();
                DataTable dtReturn = new DataTable();

                if (0 == iPageSize)
                {
                    dt = BRRedeemSet.SearchOnDataSet(sc).Tables[0];
                    iTotalCount = dt.Rows.Count;
                }
                else
                {
                    dt = BRRedeemSet.SearchOnDataSet(sc, iPageIndex, iPageSize * 2, ref iTotalCount).Tables[0];
                }

                iTotalCount = iTotalCount / 2;

                dtReturn.Columns.Add(new DataColumn("RECEIVE_NUMBER", typeof(string)));
                dtReturn.Columns.Add(new DataColumn("WTYPE", typeof(string)));
                dtReturn.Columns.Add(new DataColumn("1KEYUID", typeof(string)));
                dtReturn.Columns.Add(new DataColumn("1KEYUTIME", typeof(string)));
                dtReturn.Columns.Add(new DataColumn("2KEYUID", typeof(string)));
                dtReturn.Columns.Add(new DataColumn("2KEYUTIME", typeof(string)));
                dtReturn.Columns.Add(new DataColumn("ISSAME", typeof(string)));
                dtReturn.Columns.Add(new DataColumn("DIFFNUM", typeof(int)));
                dtReturn.Columns.Add(new DataColumn("EDITUID", typeof(string)));

                for (int i = 0; i < dt.Rows.Count; i = i + 2)
                {
                    DataRow dr = dtReturn.NewRow();

                    dr["RECEIVE_NUMBER"] = dt.Rows[i]["RECEIVE_NUMBER"].ToString();
                    dr["WTYPE"] = dt.Rows[i]["WTYPE"].ToString();
                    dr["1KEYUID"] = dt.Rows[i]["USER_ID"].ToString();
                    dr["1KEYUTIME"] = dt.Rows[i]["USE_TIME"].ToString();
                    dr["2KEYUID"] = dt.Rows[i + 1]["USER_ID"].ToString();
                    dr["2KEYUTIME"] = dt.Rows[i + 1]["USE_TIME"].ToString();
                    dr["ISSAME"] = dt.Rows[i + 1]["ISSAME"].ToString();
                    dr["DIFFNUM"] = dt.Rows[i + 1]["DIFF_NUM"].ToString();
                    dr["EDITUID"] = dt.Rows[i + 1]["EDIT_USER_ID"].ToString();

                    dtReturn.Rows.Add(dr);
                }

                return dtReturn;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// 修改紀錄:2021/02/01_Ares_Stanley-收件日期調整為區間
        /// </summary>
        /// <param name="strRECEIVENUMBER"></param>
        /// <param name="strReceiveDateStart"></param>
        /// <param name="strReceiveDateEnd"></param>
        /// <param name="strMERCHANT"></param>
        /// <param name="iPageIndex"></param>
        /// <param name="iPageSize"></param>
        /// <param name="iTotalCount"></param>
        /// <returns></returns>
        public static DataSet GetFileDetail(string strRECEIVENUMBER, string strReceiveDateStart, string strReceiveDateEnd, string strMERCHANT, int iPageIndex, int iPageSize, ref int iTotalCount)
        {
            try
            {
                SqlCommand sc = new SqlCommand();
                sc.CommandType = CommandType.Text;
                if ("" == strMERCHANT.Trim())
                {
                    sc.CommandText = SQL_SQL_FILE_DETAIL_NOMER;
                }
                else
                {
                    sc.CommandText = SQL_SQL_FILE_DETAIL_HASMER;
                }


                SqlParameter spRN = new SqlParameter("RECEIVE_NUMBER", strRECEIVENUMBER);
                sc.Parameters.Add(spRN);

                SqlParameter spRD_start = new SqlParameter("ReceiveDateStart", strReceiveDateStart);
                sc.Parameters.Add(spRD_start);

                SqlParameter spRD_end = new SqlParameter("ReceiveDateEnd", strReceiveDateEnd);
                sc.Parameters.Add(spRD_end);

                if ("" != strMERCHANT.Trim())                
                {
                    SqlParameter spMN = new SqlParameter("MERCHANT_NO", strMERCHANT);
                    sc.Parameters.Add(spMN);
                }

                if (0 == iPageSize)
                {
                    DataSet ds = BRRedeemSet.SearchOnDataSet(sc);
                    iTotalCount = ds.Tables[0].Rows.Count;
                    return ds;
                }
                else
                {
                    return BRRedeemSet.SearchOnDataSet(sc, iPageIndex, iPageSize, ref iTotalCount);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
