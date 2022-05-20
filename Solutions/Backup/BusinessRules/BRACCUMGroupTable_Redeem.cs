using System;
using System.Collections.Generic;
using System.Text;
using CSIPKeyInGUI.EntityLayer;
using System.Data;
using Framework.Data.OM.Collections;
using Framework.Data.OM;
using Framework.Data.OM.Transaction;
using System.Data.SqlClient;

namespace CSIPKeyInGUI.BusinessRules
{
    public class BRACCUMGroupTable_Redeem : CSIPCommonModel.BusinessRules.BRBase<EntityACCUMGroupTable_Redeem>
    {
        #region sql
        public const string SQL_DEL = "delete from ACCUMGroupTable_Redeem where ACCU_CODE= @ACCU_CODE";
        #endregion sql

        public static EntitySet<EntityACCUMGroupTable_Redeem> Select(string strACCUCode)
        {
            try
            {
                SqlHelper sqlH = new SqlHelper();
                sqlH.AddCondition(EntityACCUMGroupTable_Redeem.M_ACCU_CODE, Operator.Equal, DataTypeUtils.String, strACCUCode);

                return BRACCUMGroupTable_Redeem.Search(sqlH.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Insert(EntitySet<EntityACCUMGroupTable_Redeem> esACCUMGT_R, string strACCUCode)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    SqlCommand sc = new SqlCommand();
                    sc.CommandType = System.Data.CommandType.Text;
                    sc.CommandText = SQL_DEL;

                    SqlParameter spRN = new SqlParameter(EntityACCUMGroupTable_Redeem.M_ACCU_CODE, strACCUCode);
                    sc.Parameters.Add(spRN);

                    if (!BRACCUMGroupTable_Redeem.Delete(sc))
                    {
                        return false;
                    }

                    if (esACCUMGT_R.Count > 0)
                    {
                        for (int i = 0; i < esACCUMGT_R.Count; i++)
                        {
                            EntityACCUMGroupTable_Redeem eACCUMGT_R = new EntityACCUMGroupTable_Redeem();
                            eACCUMGT_R = esACCUMGT_R.GetEntity(i);

                            if (!BRACCUMGroupTable_Redeem.AddNewEntity(eACCUMGT_R))
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


    }
}
