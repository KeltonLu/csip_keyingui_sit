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
    public class BRAwardSet3270 : CSIPCommonModel.BusinessRules.BRBase<EntityAwardSet3270>
    {
        #region sql
        // 修改紀錄: 2020/12/04_Ares_Stanley-變更DETAILREPORT查詢方式
        public const string SQL_UPDATE = "update AwardSet3270 set SEND_FLAG= @SEND_FLAG ,MSG_SEQ= @MSG_SEQ ,MSG_TYPE= @MSG_TYPE where ID= @ID";

        public const string SQL_SEL_DETAILREPORT = @"
                                                    SELECT
                                                    a.RECEIVE_NUMBER,
	                                                a.FUNCTION_CODE,
	                                                a.MSG_SEQ,
	                                                a.MSG_TYPE,
	                                                a.ORG,
	                                                a.PROG_ID,
	                                                a.PARTNER_ID,
	                                                a.ACCUMULATION_TYPE,
	                                                a.TC_CODE1,
	                                                a.TC_CODE2,
	                                                MCC_FROM1 + '-' + MCC_TO1 AS MCC_FROM_1,
	                                                MCC_FROM2 + '-' + MCC_TO2 AS MCC_FROM_2,
	                                                MCC_FROM3 + '-' + MCC_TO3 AS MCC_FROM_3,
	                                                MCC_FROM4 + '-' + MCC_TO4 AS MCC_FROM_4,
	                                                MCC_FROM5 + '-' + MCC_TO5 AS MCC_FROM_5,
	                                                MCC_FROM6 + '-' + MCC_TO6 AS MCC_FROM_6,
	                                                MCC_FROM7 + '-' + MCC_TO7 AS MCC_FROM_7,
	                                                MCC_FROM8 + '-' + MCC_TO8 AS MCC_FROM_8,
	                                                MCC_FROM9 + '-' + MCC_TO9 AS MCC_FROM_9,
	                                                MCC_FROM10 + '-' + MCC_TO10 AS MCC_FROM_10,
	                                                a.COUNTRY_CODE_IND,
	                                                a.COUNTRY_CODE1,
	                                                a.COUNTRY_CODE2,
	                                                a.COUNTRY_CODE3,
	                                                a.COUNTRY_CODE4,
	                                                a.COUNTRY_CODE5,
	                                                a.COUNTRY_CODE6,
	                                                a.COUNTRY_CODE7,
	                                                a.COUNTRY_CODE8,
	                                                a.COUNTRY_CODE9,
	                                                a.COUNTRY_CODE10,
	                                                a.BASIC_CALC_IND,
	                                                a.BASIC_TIER_AMT1,
	                                                a.BASIC_TIER_RATE1,
	                                                a.BASIC_TIER_AMT2,
	                                                a.BASIC_TIER_RATE2,
	                                                a.BASIC_TIER_AMT3,
	                                                a.BASIC_TIER_RATE3,
	                                                a.BASIC_TIER_AMT4,
	                                                a.BASIC_TIER_RATE4,
	                                                a.SUPP_BASIC_CAL_IND,
	                                                a.SUPP_BASIC_TIER_AMT1,
	                                                a.SUPP_BASIC_TIER_RATE1,
	                                                a.SUPP_BASIC_TIER_AMT2,
	                                                a.SUPP_BASIC_TIER_RATE2,
	                                                a.SUPP_BASIC_TIER_AMT3,
	                                                a.SUPP_BASIC_TIER_RATE3,
	                                                a.SUPP_BASIC_TIER_AMT4,
	                                                a.SUPP_BASIC_TIER_RATE4,
	                                                a.PROMO_START_DTE,
	                                                a.PROMO_END_DTE,
	                                                a.PROMO_CALC_IND,
	                                                a.PROMO_TIER_AMT1,
	                                                a.PROMO_TIER_RATE1,
	                                                a.PROMO_TIER_AMT2,
	                                                a.PROMO_TIER_RATE2,
	                                                a.PROMO_TIER_AMT3,
	                                                a.PROMO_TIER_RATE3,
	                                                a.PROMO_TIER_AMT4,
	                                                a.PROMO_TIER_RATE4,
	                                                a.BTHDTE_START,
	                                                a.BTHDTE_END,
	                                                a.BTHDTE_CALC_IND,
	                                                a.BTHDTE_TIER_AMT1,
	                                                a.BTHDTE_TIER_RATE1,
	                                                a.BTHDTE_TIER_AMT2,
	                                                a.BTHDTE_TIER_RATE2,
	                                                a.BTHDTE_TIER_AMT3,
	                                                a.BTHDTE_TIER_RATE3,
	                                                a.BTHDTE_TIER_AMT4,
	                                                a.BTHDTE_TIER_RATE4
                                                    FROM dbo.AwardSet3270 A
                                                    INNER JOIN dbo.AwardSet B ON A.RECEIVE_NUMBER = B.RECEIVE_NUMBER
                                                    WHERE B.RECEIVE_NUMBER LIKE @RECEIVE_NUMBER AND B.ReceiveDate LIKE @ReceiveDate AND B.KEYIN_FLAG = '1'";
        #endregion sql

        public static bool AddBatch(EntitySet<EntityAwardSet3270> esAS3270)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (esAS3270.Count > 0)
                    {
                        for (int i = 0; i < esAS3270.Count; i++)
                        {
                            EntityAwardSet3270 eAS3270 = new EntityAwardSet3270();
                            eAS3270 = esAS3270.GetEntity(i);

                            if (!BRAwardSet3270.AddNewEntity(eAS3270))
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

        public static EntitySet<EntityAwardSet3270> GetSendData()
        {
            try
            {
                SqlHelper sqlH = new SqlHelper();
                sqlH.AddCondition(EntityAwardSet3270.M_SEND_FLAG, Operator.Equal, DataTypeUtils.String, "N");

                return BRAwardSet3270.Search(sqlH.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static bool UpdateSendData(int iID, string strMSG_SEQ, string strMSG_TYPE, string strSEND_FLAG)
        {
            try
            {
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = System.Data.CommandType.Text;
                sqlCmd.CommandText = SQL_UPDATE;

                SqlParameter sqlpMSG_SEQ = new SqlParameter(EntityAwardSet3270.M_MSG_SEQ, strMSG_SEQ);
                sqlCmd.Parameters.Add(sqlpMSG_SEQ);
                SqlParameter sqlpMSG_TYPE = new SqlParameter(EntityAwardSet3270.M_MSG_TYPE, strMSG_TYPE);
                sqlCmd.Parameters.Add(sqlpMSG_TYPE);
                SqlParameter sqlpSEND_FLAG = new SqlParameter(EntityAwardSet3270.M_SEND_FLAG, strSEND_FLAG);
                sqlCmd.Parameters.Add(sqlpSEND_FLAG);

                SqlParameter sqlpID = new SqlParameter(EntityAwardSet3270.M_ID, iID);
                sqlCmd.Parameters.Add(sqlpID);

                return BRAwardSet3270.Update(sqlCmd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //add by linhuanhuang start
        public static DataSet GetDetailReport(string strRECEIVENUMBER, string strReceiveDate, int iPageIndex, int iPageSize, ref int iTotalCount)
        {
            try
            {
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = System.Data.CommandType.Text;
                sqlCmd.CommandText = SQL_SEL_DETAILREPORT;

                SqlParameter sqlpRECEIVENUMBER = new SqlParameter(EntityAwardSet.M_RECEIVE_NUMBER, strRECEIVENUMBER);
                sqlCmd.Parameters.Add(sqlpRECEIVENUMBER);

                SqlParameter sqlpReceiveDate = new SqlParameter(EntityAwardSet.M_ReceiveDate, strReceiveDate);
                sqlCmd.Parameters.Add(sqlpReceiveDate);

                if (0 == iPageSize)
                {
                    DataSet ds = BRAwardSet3270.SearchOnDataSet(sqlCmd);
                    iTotalCount = ds.Tables[0].Rows.Count;
                    return ds;
                }
                else
                {
                    return BRAwardSet3270.SearchOnDataSet(sqlCmd, iPageIndex, iPageSize, ref iTotalCount);
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
