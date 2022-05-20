using System;
using System.Collections.Generic;
using System.Text;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using System.Data.SqlClient;

namespace CSIPKeyInGUI.BusinessRules
{
    public class BRACCUMGroupTable_Award : CSIPCommonModel.BusinessRules.BRBase<EntityACCUMGroupTable_Award>
    {
        #region sql
        public const string SQL_DEL = "delete from ACCUMGroupTable_Award where ACCU_CODE= @ACCU_CODE";
        #endregion sql

        public static EntitySet<EntityACCUMGroupTable_Award> Select(string strACCUCode)
        {
            try
            {
                strACCUCode = EntityACCUMGroupTable_Award.M_ACCU_CODE + "='" + strACCUCode + "'";

                return BRACCUMGroupTable_Award.Search(strACCUCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Insert(EntitySet<EntityACCUMGroupTable_Award> esACCUMGT_A, string strACCUCode)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    SqlCommand sc = new SqlCommand();
                    sc.CommandType = System.Data.CommandType.Text;
                    sc.CommandText = SQL_DEL;

                    SqlParameter spRN = new SqlParameter(EntityACCUMGroupTable_Award.M_ACCU_CODE, strACCUCode);
                    sc.Parameters.Add(spRN);

                    if (!BRACCUMGroupTable_Award.Delete(sc))
                    {
                        return false;
                    }

                    if (esACCUMGT_A.Count > 0)
                    {
                        for (int i = 0; i < esACCUMGT_A.Count; i++)
                        {
                            EntityACCUMGroupTable_Award eACCUMGT_A = new EntityACCUMGroupTable_Award();
                            eACCUMGT_A = esACCUMGT_A.GetEntity(i);

                            if (!BRACCUMGroupTable_Award.AddNewEntity(eACCUMGT_A))
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
