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
        public const string SQL_UPDATE = "update Redeem3270 set SEND_FLAG=@SEND_FLAG ,MSG_SEQ= @MSG_SEQ ,MSG_ERR= @MSG_ERR where ID= @ID";

        //add by linhuanhuang start
        public const string SQL_SEL_DETAILREPORT = @"SELECT A.* FROM dbo.Redeem3270 A
                                                INNER JOIN dbo.RedeemSet B ON A.RECEIVE_NUMBER = B.RECEIVE_NUMBER
                                                WHERE B.RECEIVE_NUMBER LIKE @RECEIVE_NUMBER AND B.ReceiveDate LIKE @ReceiveDate
                                                AND A.IN_MERCHANT LIKE @IN_MERCHANT AND B.KEYIN_FLAG = '1'";
        //add by linhuanhuang end
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

        //add by linhuanhuang start
        public static DataSet GetDetailReport(string strRECEIVENUMBER, string strReceiveDate, string strMERCHANT, int iPageIndex, int iPageSize, ref int iTotalCount)
        {
            try
            {
                SqlCommand sqlCmd = new SqlCommand(); 
                sqlCmd.CommandType = System.Data.CommandType.Text;
                sqlCmd.CommandText = SQL_SEL_DETAILREPORT;

                SqlParameter sqlpRECEIVENUMBER = new SqlParameter(EntityRedeemSet.M_RECEIVE_NUMBER, strRECEIVENUMBER);
                sqlCmd.Parameters.Add(sqlpRECEIVENUMBER);

                SqlParameter sqlpReceiveDate = new SqlParameter(EntityRedeemSet.M_ReceiveDate, strReceiveDate);
                sqlCmd.Parameters.Add(sqlpReceiveDate);

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
