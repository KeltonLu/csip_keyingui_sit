using System;
using System.Collections.Generic;
using System.Text;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using Framework.Data.OM;
using System.Data.SqlClient;
using System.Data;
using CSIPCommonModel.EntityLayer;

namespace CSIPKeyInGUI.BusinessRules
{
    public class BRRedeemSet_ATypeSet : CSIPCommonModel.BusinessRules.BRBase<EntityRedeemSet_ATypeSet>
    {
        #region SQL
        public const string SQL_DELALL = "delete from RedeemSet_ATypeSet where RECEIVE_NUMBER=@RECEIVE_NUMBER and KEYIN_FLAG=@KEYIN_FLAG";

        public const string SQL_3270_CTAll = @"select *,@ACTIVITY AS ACTIVITY from 
                                                (select a.CARD_TYPE,max(a.STEP) as STEP from RedeemSet_ATypeSet a
                                                inner join CardTypeList_Redeem b on a.CARD_TYPE=b.Card_CODE
                                                where a.RECEIVE_NUMBER= @RECEIVE_NUMBER and a.ACTIVITY= @ACTIVITY and a.KEYIN_FLAG=@KEYIN_FLAG
                                                group by a.CARD_TYPE) c
                                                where CARD_TYPE in (select CARD_TYPE from RedeemSet_ATypeSet
                                                where RECEIVE_NUMBER= @RECEIVE_NUMBER and ACTIVITY= @ACTIVITY and KEYIN_FLAG=@KEYIN_FLAG
                                                and STEP='1')";

        public const string SQL_3270_CT = @"select a.* from RedeemSet_ATypeSet a
                                            inner join CardTypeList_Redeem b on a.CARD_TYPE=b.Card_CODE
                                            where a.RECEIVE_NUMBER= @RECEIVE_NUMBER and a.ACTIVITY= @ACTIVITY and a.KEYIN_FLAG=@KEYIN_FLAG and a.STEP='1'";
        #endregion SQL

        public static bool AddBatch(EntitySet<EntityRedeemSet_ATypeSet> esRS_ATS)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (esRS_ATS.Count > 0)
                    {
                        for (int i = 0; i < esRS_ATS.Count; i++)
                        {
                            EntityRedeemSet_ATypeSet eRS_ATS = esRS_ATS.GetEntity(i);

                            if (!BRRedeemSet_ATypeSet.AddNewEntity(eRS_ATS))
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

        public static EntitySet<EntityRedeemSet_ATypeSet> SelectData(string strReceiveNumbers, string strKeyinFlag)
        {
            try
            {
                SqlHelper sqlh = new SqlHelper();
                sqlh.AddCondition(EntityRedeemSet_ATypeSet.M_RECEIVE_NUMBER, Operator.Equal, DataTypeUtils.String, strReceiveNumbers);
                sqlh.AddCondition(EntityRedeemSet_ATypeSet.M_KEYIN_FLAG, Operator.Equal, DataTypeUtils.String, strKeyinFlag);
                sqlh.AddOrderCondition(EntityRedeemSet_ATypeSet.M_RECEIVE_NUMBER, ESortType.ASC);
                sqlh.AddOrderCondition(EntityRedeemSet_ATypeSet.M_ACTIVITY, ESortType.ASC);
                sqlh.AddOrderCondition(EntityRedeemSet_ATypeSet.M_STEP, ESortType.ASC);
                sqlh.AddOrderCondition(EntityRedeemSet_ATypeSet.M_TXT_INDEX, ESortType.ASC);

                return BRRedeemSet_ATypeSet.Search(sqlh.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool DelAll(string strReceiveNumbers, string strKeyinFlag)
        {
            try
            {
                SqlCommand sc = new SqlCommand();
                sc.CommandType = System.Data.CommandType.Text;
                sc.CommandText = SQL_DELALL;

                SqlParameter spRN = new SqlParameter(EntityRedeemSet_ATypeSet.M_RECEIVE_NUMBER, strReceiveNumbers);
                sc.Parameters.Add(spRN);
                SqlParameter spKF = new SqlParameter(EntityRedeemSet_ATypeSet.M_KEYIN_FLAG, strKeyinFlag);
                sc.Parameters.Add(spKF);

                return BRRedeemSet_ATypeSet.Delete(sc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 全卡種(重覆的部分應以後訂的值為依據(3>2>1))
        /// </summary>
        /// <returns></returns>
        public static DataSet Select3270_CTAll(string strReceiveNumbers, string strACTIVITY, string strKeyinFlag)
        {
            try
            {
                SqlCommand sc = new SqlCommand();
                sc.CommandType = System.Data.CommandType.Text;
                sc.CommandText = SQL_3270_CTAll;

                SqlParameter spRN = new SqlParameter(EntityRedeemSet_ATypeSet.M_RECEIVE_NUMBER, strReceiveNumbers);
                sc.Parameters.Add(spRN);
                SqlParameter spACT = new SqlParameter(EntityRedeemSet_ATypeSet.M_ACTIVITY, strACTIVITY.ToUpper());
                sc.Parameters.Add(spACT);
                SqlParameter spKF = new SqlParameter(EntityRedeemSet_ATypeSet.M_KEYIN_FLAG, strKeyinFlag);
                sc.Parameters.Add(spKF);

                return BRRedeemSet_ATypeSet.SearchOnDataSet(sc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 非全卡種
        /// </summary>
        /// <param name="strReceiveNumbers"></param>
        /// <param name="strACTIVITY"></param>
        /// <returns></returns>
        public static DataSet Select3270_CT(string strReceiveNumbers, string strACTIVITY, string strKeyinFlag)
        {
            try
            {
                SqlCommand sc = new SqlCommand();
                sc.CommandType = System.Data.CommandType.Text;
                sc.CommandText = SQL_3270_CT;

                SqlParameter spRN = new SqlParameter(EntityRedeemSet_ATypeSet.M_RECEIVE_NUMBER, strReceiveNumbers);
                sc.Parameters.Add(spRN);
                SqlParameter spACT = new SqlParameter(EntityRedeemSet_ATypeSet.M_ACTIVITY, strACTIVITY.ToUpper());
                sc.Parameters.Add(spACT);
                SqlParameter spKF = new SqlParameter(EntityRedeemSet_ATypeSet.M_KEYIN_FLAG, strKeyinFlag);
                sc.Parameters.Add(spKF);

                return BRRedeemSet_ATypeSet.SearchOnDataSet(sc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
