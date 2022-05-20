using System;
using System.Collections.Generic;
using System.Text;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using System.Data.SqlClient;
using Framework.Data.OM;

namespace CSIPKeyInGUI.BusinessRules
{
    public class BRAwardSet_ATypeSet : CSIPCommonModel.BusinessRules.BRBase<EntityAwardSet_ATypeSet>
    {
        #region SQL
        public const string SQL_DELALL = "delete from AwardSet_ATypeSet where RECEIVE_NUMBER=@RECEIVE_NUMBER and KEYIN_FLAG=@KEYIN_FLAG";
        #endregion SQL

        public static bool AddBatch(EntitySet<EntityAwardSet_ATypeSet> esAS_ATS)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (esAS_ATS.Count > 0)
                    {
                        for (int i = 0; i < esAS_ATS.Count; i++)
                        {
                            EntityAwardSet_ATypeSet eAS_ATS = new EntityAwardSet_ATypeSet();
                            eAS_ATS = esAS_ATS.GetEntity(i);

                            if (!BRAwardSet_ATypeSet.AddNewEntity(eAS_ATS))
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


        public static bool DelAll(string strReceiveNumber, string strKeyinFlag)
        {
            try
            {
                SqlCommand sc = new SqlCommand();
                sc.CommandType = System.Data.CommandType.Text;
                sc.CommandText = SQL_DELALL;

                SqlParameter spRN = new SqlParameter(EntityAwardSet_ATypeSet.M_RECEIVE_NUMBER, strReceiveNumber);
                sc.Parameters.Add(spRN);
                SqlParameter spKF = new SqlParameter(EntityAwardSet_ATypeSet.M_KEYIN_FLAG, strKeyinFlag);
                sc.Parameters.Add(spKF);

                return BRAwardSet_ATypeSet.Delete(sc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static EntitySet<EntityAwardSet_ATypeSet> SelectData(string strReceiveNumber, string strKeyinFlag)
        {
            try
            {
                SqlHelper sqlh = new SqlHelper();
                sqlh.AddCondition(EntityAwardSet_ATypeSet.M_RECEIVE_NUMBER, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
                sqlh.AddCondition(EntityAwardSet_ATypeSet.M_KEYIN_FLAG, Operator.Equal, DataTypeUtils.String, strKeyinFlag);
                sqlh.AddOrderCondition(EntityAwardSet_ATypeSet.M_TXT_INDEX, ESortType.ASC);

                return BRAwardSet_ATypeSet.Search(sqlh.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
