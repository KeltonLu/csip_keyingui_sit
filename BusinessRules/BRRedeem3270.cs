using System;
using System.Collections.Generic;
using System.Text;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using Framework.Data.OM;
using System.Data.SqlClient;
using System.Data;

namespace CSIPKeyInGUI.BusinessRules
{
    public class BRRedeem3270 : CSIPCommonModel.BusinessRules.BRBase<EntityRedeem3270>
    {
        #region sql
        // 修改日期:2020/12/04_Ares_Stanley-變更DETAILREPORT查詢方式
        public const string SQL_UPDATE = "update Redeem3270 set SEND_FLAG=@SEND_FLAG ,MSG_SEQ= @MSG_SEQ ,MSG_ERR= @MSG_ERR where ID= @ID";

        //add by linhuanhuang start
        //修改日期:2021/03/08_Ares_Stanley-查詢單一日期改為查詢日期區間
        public const string SQL_SEL_DETAILREPORT = @"SELECT a.RECEIVE_NUMBER,
                                                                                    a.FUNCTION_CODE,
                                                                                    a.MSG_SEQ,
                                                                                    a.MSG_ERR,
                                                                                    a.IN_ORG,
                                                                                    a.IN_MERCHANT,
                                                                                    a.IN_PROD_CODE,
                                                                                    a.IN_CARD_TYPE,
                                                                                    a.PROGID,
                                                                                    a.MER_RATE,
                                                                                    a.LIMITR,
                                                                                    a.CHPOINT,
                                                                                    a.CHAMT,
                                                                                    a.USER_EXIT,
                                                                                    a.STARTU,
                                                                                    a.ENDU,
                                                                                    a.CYLCO,
                                                                                    a.LIMITU,
                                                                                    a.CHPOINTU,
                                                                                    a.CHAMTU,
                                                                                    a.BIRTH,
                                                                                    a.STARTB,
                                                                                    a.ENDB,
                                                                                    a.LIMITB,
                                                                                    a.CHPOINTB,
                                                                                    a.CHAMTB  
                                                FROM dbo.Redeem3270 A
                                                INNER JOIN dbo.RedeemSet B ON A.RECEIVE_NUMBER = B.RECEIVE_NUMBER
                                                WHERE B.RECEIVE_NUMBER LIKE @RECEIVE_NUMBER
                                                AND A.IN_MERCHANT LIKE @IN_MERCHANT AND B.KEYIN_FLAG = '1'";
        //add by linhuanhuang end
        //2021/04/08_Ares_Stanley-增加排序
        public const string SQL_SEL_DETAILREPORT_withDATE = @" AND B.ReceiveDate BETWEEN @ReceiveDateStart AND @ReceiveDateEnd ORDER BY B.ReceiveDate, A.RECEIVE_NUMBER";
        #endregion sql

        public static bool AddBatch(EntitySet<EntityRedeem3270> esR3270)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (esR3270.Count > 0) 
                    {
                        for (int i = 0; i < esR3270.Count; i++)
                        {
                            EntityRedeem3270 eR3270 = new EntityRedeem3270();
                            eR3270 = esR3270.GetEntity(i);

                            if (!BRRedeem3270.AddNewEntity(eR3270))
                            {
                                return false;
                            }
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

        public static EntitySet<EntityRedeem3270> GetSendData()
        {
            try
            {
                SqlHelper sqlH = new SqlHelper();
                sqlH.AddCondition(EntityRedeem3270.M_SEND_FLAG, Operator.Equal, DataTypeUtils.String, "N");

                return BRRedeem3270.Search(sqlH.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool UpdateSendData(int iID, string strMSG_SEQ, string strMSG_ERR, string strSEND_FLAG)
        {
            try
            {
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = System.Data.CommandType.Text;
                sqlCmd.CommandText = SQL_UPDATE;

                SqlParameter sqlpMSG_SEQ = new SqlParameter(EntityRedeem3270.M_MSG_SEQ, strMSG_SEQ);
                sqlCmd.Parameters.Add(sqlpMSG_SEQ);
                SqlParameter sqlpMSG_ERR = new SqlParameter(EntityRedeem3270.M_MSG_ERR, strMSG_ERR);
                sqlCmd.Parameters.Add(sqlpMSG_ERR);
                SqlParameter sqlpSEND_FLAG = new SqlParameter(EntityRedeem3270.M_SEND_FLAG, strSEND_FLAG);
                sqlCmd.Parameters.Add(sqlpSEND_FLAG);

                SqlParameter sqlpID = new SqlParameter(EntityRedeem3270.M_ID, iID);
                sqlCmd.Parameters.Add(sqlpID);

                return BRRedeem3270.Update(sqlCmd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 修改紀錄:2021/03/08_Ares_Stanley-查詢單一日期改為查詢日期區間
        /// </summary>
        /// <param name="strRECEIVENUMBER"></param>
        /// <param name="strReceiveDateStart"></param>
        /// <param name="strReceiveDateEnd"></param>
        /// <param name="strMERCHANT"></param>
        /// <param name="iPageIndex"></param>
        /// <param name="iPageSize"></param>
        /// <param name="iTotalCount"></param>
        /// <param name="withDATE"></param>
        /// <returns></returns>
        //add by linhuanhuang start
        public static DataSet GetDetailReport(string strRECEIVENUMBER, string strReceiveDateStart, string strReceiveDateEnd, string strMERCHANT, int iPageIndex, int iPageSize, ref int iTotalCount, bool withDATE)
        {
            try
            {
                SqlCommand sqlCmd = new SqlCommand(); 
                sqlCmd.CommandType = System.Data.CommandType.Text;
                sqlCmd.CommandText = SQL_SEL_DETAILREPORT;

                if (withDATE)
                {
                    sqlCmd.CommandText += SQL_SEL_DETAILREPORT_withDATE;
                    SqlParameter sqlpReceiveDateStart = new SqlParameter("@ReceiveDateStart", strReceiveDateStart);
                    sqlCmd.Parameters.Add(sqlpReceiveDateStart);

                    SqlParameter sqlpReceiveDateEnd = new SqlParameter("@ReceiveDateEnd", strReceiveDateEnd);
                    sqlCmd.Parameters.Add(sqlpReceiveDateEnd);
                }

                SqlParameter sqlpRECEIVENUMBER = new SqlParameter(EntityRedeemSet.M_RECEIVE_NUMBER, strRECEIVENUMBER);
                sqlCmd.Parameters.Add(sqlpRECEIVENUMBER);

                SqlParameter sqlpMERCHANT = new SqlParameter(EntityRedeem3270.M_IN_MERCHANT, strMERCHANT);
                sqlCmd.Parameters.Add(sqlpMERCHANT);

                if (0 == iPageSize)
                {
                    DataSet ds = BRRedeem3270.SearchOnDataSet(sqlCmd);
                    iTotalCount = ds.Tables[0].Rows.Count;
                    return ds;
                }
                else
                {
                    return BRRedeem3270.SearchOnDataSet(sqlCmd, iPageIndex, iPageSize, ref iTotalCount);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //add by linhuanhuang end

    }
}
