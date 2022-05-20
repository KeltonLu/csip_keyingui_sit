using System;
using System.Collections.Generic;
using System.Text;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM.Collections;
using Framework.Data.OM;
using Framework.Data.OM.Transaction;
using System.Data.SqlClient;
using System.Data;

namespace CSIPKeyInGUI.BusinessRules
{
    public class BRRedeemUpload : CSIPCommonModel.BusinessRules.BRBase<EntityRedeemUpload>
    {
        #region sql
        public const string SQL_DEL = "delete from RedeemUpload where RECEIVE_NUMBER=@RECEIVE_NUMBER and KEYIN_FLAG='1'";

        public const string SQL_DEL2 = "delete from RedeemUpload where RECEIVE_NUMBER=@RECEIVE_NUMBER and KEYIN_FLAG='2'";

        public const string SQL_SEL = @"select *,case DATA_TYPE when 'S' then 1 when 'T' then 2 when 'M' then 3 when 'E' then 4 end as step
                                        from RedeemUpload 
                                        where KEYIN_FLAG = '2' and SEND3270 = 'N'
                                        order by RECEIVE_NUMBER,step,UPLOAD_DATA";

        public const string SQL_UPDATE = "update RedeemUpload set SEND3270='Y' where KEYIN_FLAG = '2' and SEND3270 = 'N'";

        public const string SQL_SEL_LOG = @"select * from RedeemUpload
                                            where KEYIN_FLAG = '2' and UPDATE_DATE between @DATES and @DATEE 
                                            and LINE_INDEX = 0 and RECEIVE_NUMBER like @RECEIVE_NUMBER ";
        #endregion sql

        public static DataSet SelectTxt()
        {
            try
            {
                return BRRedeemUpload.SearchOnDataSet(SQL_SEL);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static EntitySet<EntityRedeemUpload> SelectData(string strReceiveNumber, string strKEYINFLAG)
        {
            try
            {
                SqlHelper sqlh = new SqlHelper();
                sqlh.AddCondition(EntityRedeemUpload.M_RECEIVE_NUMBER, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
                sqlh.AddCondition(EntityRedeemUpload.M_KEYIN_FLAG, Operator.Equal, DataTypeUtils.String, strKEYINFLAG);

                return BRRedeemUpload.Search(sqlh.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static bool AddBatch(EntitySet<EntityRedeemUpload> esRU)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (esRU.Count > 0)
                    {
                        for (int i = 0; i < esRU.Count; i++)
                        {
                            EntityRedeemUpload eRU = new EntityRedeemUpload();
                            eRU = esRU.GetEntity(i);

                            if (!BRRedeemUpload.AddNewEntity(eRU))
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

        public static bool DelData(string strReceiveNumber)
        {
            try
            {
                SqlCommand sqlC = new SqlCommand();
                sqlC.CommandType = System.Data.CommandType.Text;
                sqlC.CommandText = SQL_DEL;

                SqlParameter spRN = new SqlParameter(EntityRedeemUpload.M_RECEIVE_NUMBER, strReceiveNumber);
                sqlC.Parameters.Add(spRN);

                return BRRedeemUpload.Delete(sqlC);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool AddBatchU(EntitySet<EntityRedeemUpload> esRU, string strKeyInDate)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (esRU.Count > 0)
                    {
                        for (int i = 0; i < esRU.Count; i++)
                        {
                            EntityRedeemUpload eRU = new EntityRedeemUpload();
                            eRU = esRU.GetEntity(i);
                            eRU.KEYIN_DATE = strKeyInDate;

                            if (!BRRedeemUpload.AddNewEntity(eRU))
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

        public static bool UpdateKey(string strReceiveNumber, EntitySet<EntityRedeemUpload> esRU)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    EntitySet<EntityRedeemUpload> esRUKey1 = new EntitySet<EntityRedeemUpload>();
                    esRUKey1 = BRRedeemUpload.SelectData(strReceiveNumber, "1");

                    if (!BRRedeemUpload.DelData(strReceiveNumber))
                    {
                        return false;
                    }

                    if (!BRRedeemUpload.AddBatchU(esRU, esRUKey1.GetEntity(0).KEYIN_DATE))
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

        public static bool UpdateSendData()
        {
            try
            {
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = System.Data.CommandType.Text;
                sqlCmd.CommandText = SQL_UPDATE;                

                return BRRedeemUpload.Update(sqlCmd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static bool AddBatchU2(EntitySet<EntityRedeemUpload> esRU, EntityRedeemUpload eRUK2)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (esRU.Count > 0)
                    {
                        for (int i = 0; i < esRU.Count; i++)
                        {
                            EntityRedeemUpload eRU = new EntityRedeemUpload();
                            eRU = esRU.GetEntity(i);

                            eRU.USER_ID = eRUK2.USER_ID;
                            eRU.KEYIN_DATE = eRUK2.KEYIN_DATE;
                            eRU.USE_TIME = eRUK2.USE_TIME;
                            eRU.ISSAME = eRUK2.ISSAME;
                            eRU.DIFF_NUM = eRUK2.DIFF_NUM;

                            if (!BRRedeemUpload.AddNewEntity(eRU))
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

        public static bool UpdateKey2(string strReceiveNumber, EntitySet<EntityRedeemUpload> esRU)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    EntityRedeemUpload eRUK2 = new EntityRedeemUpload();
                    eRUK2 = BRRedeemUpload.SelectData(strReceiveNumber, "2").GetEntity(0);

                    if (!BRRedeemUpload.DelData2(strReceiveNumber))
                    {
                        return false;
                    }

                    if (!BRRedeemUpload.AddBatchU2(esRU, eRUK2))
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

        public static bool DelData2(string strReceiveNumber)
        {
            try
            {
                SqlCommand sqlC = new SqlCommand();
                sqlC.CommandType = System.Data.CommandType.Text;
                sqlC.CommandText = SQL_DEL2;

                SqlParameter spRN = new SqlParameter(EntityRedeemUpload.M_RECEIVE_NUMBER, strReceiveNumber);
                sqlC.Parameters.Add(spRN);

                return BRRedeemUpload.Delete(sqlC);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet SelectLOG(string strDateS, string strDateE, string strReceiveNumber, int iPageIndex, int iPageSize, ref int iTotalCount)
        {
            try
            {
                if ("" == strReceiveNumber.Trim())
                {
                    strReceiveNumber = "%";
                }

                SqlCommand sc = new SqlCommand();
                sc.CommandType = CommandType.Text;
                sc.CommandText = SQL_SEL_LOG;

                SqlParameter spDS = new SqlParameter("DATES", strDateS);
                sc.Parameters.Add(spDS);
                SqlParameter spDE = new SqlParameter("DATEE", strDateE);
                sc.Parameters.Add(spDE);
                SqlParameter spRN = new SqlParameter(EntityRedeemUpload.M_RECEIVE_NUMBER, strReceiveNumber);
                sc.Parameters.Add(spRN);

                return BRREISSUE_CARD.SearchOnDataSet(sc, iPageIndex, iPageSize, ref iTotalCount);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
