//******************************************************************
//*  作    者：Ian Huang
//*  功能說明：AwardSet資料庫業務類
//*  創建日期：2010/06/25
//*  修改記錄：

//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM.Collections;
using Framework.Data.OM;
using Framework.Data.OM.Transaction;

namespace CSIPKeyInGUI.BusinessRules
{
    public class BRAwardSet : CSIPCommonModel.BusinessRules.BRBase<EntityAwardSet>
    {
        public static EntitySet<EntityAwardSet> Select(string strReceiveNumber)
        {
            try
            {
                SqlHelper sqlH = new SqlHelper();
                sqlH.AddCondition(EntityAwardSet.M_RECEIVE_NUMBER, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
                sqlH.AddCondition(EntityAwardSet.M_KEYIN_FLAG, Operator.Equal, DataTypeUtils.String, "1");

                return BRAwardSet.Search(sqlH.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static EntitySet<EntityAwardSet> SelectData(string strReceiveNumber, string strKEYINFLAG)
        {
            try
            {
                SqlHelper sqlH = new SqlHelper();
                sqlH.AddCondition(EntityAwardSet.M_RECEIVE_NUMBER, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
                sqlH.AddCondition(EntityAwardSet.M_KEYIN_FLAG, Operator.Equal, DataTypeUtils.String, strKEYINFLAG);

                return BRAwardSet.Search(sqlH.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool InsertKey1(EntityAwardSet eAS, EntitySet<EntityAwardSet_ATypeSet> esAS_ATS)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (!BRAwardSet.AddNewEntity(eAS))
                    {
                        return false;
                    }

                    // 添加 AwardSet_ATypeSet 表中，對應收編的所有的 CardType 資料
                    if (!BRAwardSet_ATypeSet.AddBatch(esAS_ATS))
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

        public static bool UpdateKey1(string strReceiveNumber, EntityAwardSet eAS, EntitySet<EntityAwardSet_ATypeSet> esAS_ATS)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    SqlHelper sqlh = new SqlHelper();
                    sqlh.AddCondition(EntityAwardSet.M_RECEIVE_NUMBER, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
                    sqlh.AddCondition(EntityAwardSet.M_KEYIN_FLAG, Operator.Equal, DataTypeUtils.String, "1");

                    EntityAwardSet eASK1 = new EntityAwardSet();
                    eASK1 = BRAwardSet.SelectData(strReceiveNumber, "1").GetEntity(0);

                    eAS.KEYIN_DATE = eASK1.KEYIN_DATE;

                    if (!BRAwardSet.UpdateEntity(eAS, sqlh.GetFilterCondition()))
                    {
                        return false;
                    }

                    // 刪除 AwardSet_ATypeSet 表中，對應收編的所有的 CardType 資料
                    if (!BRAwardSet_ATypeSet.DelAll(strReceiveNumber, "1"))
                    {
                        return false;
                    }

                    // 從新添加 AwardSet_ATypeSet 表中，對應收編的所有的 CardType 資料
                    if (!BRAwardSet_ATypeSet.AddBatch(esAS_ATS))
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

        public static bool InsertKey2(EntityAwardSet eAS, EntitySet<EntityAwardSet_ATypeSet> esAS_ATS, EntitySet<EntityAwardSet3270> esAS3270)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (!BRAwardSet.AddNewEntity(eAS))
                    {
                        return false;
                    }

                    // 添加 AwardSet_ATypeSet 表中，對應收編的所有的 CardType 資料
                    if (!BRAwardSet_ATypeSet.AddBatch(esAS_ATS))
                    {
                        return false;
                    }

                    //if ("N" == eAS.SEND3270)
                    if ("Y" == eAS.ISSAME)
                    {
                        if (!BRAwardSet3270.AddBatch(esAS3270))
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

        public static bool UpdateKey2(string strReceiveNumber, EntityAwardSet eAS, EntitySet<EntityAwardSet_ATypeSet> esAS_ATS, EntitySet<EntityAwardSet3270> esAS3270)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    SqlHelper sqlh = new SqlHelper();
                    sqlh.AddCondition(EntityAwardSet.M_RECEIVE_NUMBER, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
                    sqlh.AddCondition(EntityAwardSet.M_KEYIN_FLAG, Operator.Equal, DataTypeUtils.String, "2");

                    if (!BRAwardSet.UpdateEntity(eAS, sqlh.GetFilterCondition()))
                    {
                        return false;
                    }

                    // 刪除 AwardSet_ATypeSet 表中，對應收編的所有的 CardType 資料
                    if (!BRAwardSet_ATypeSet.DelAll(strReceiveNumber, "2"))
                    {
                        return false;
                    }

                    // 從新添加 AwardSet_ATypeSet 表中，對應收編的所有的 CardType 資料
                    if (!BRAwardSet_ATypeSet.AddBatch(esAS_ATS))
                    {
                        return false;
                    }

                    //if ("N" == eAS.SEND3270)
                    if ("Y" == eAS.ISSAME)
                    {
                        if (!BRAwardSet3270.AddBatch(esAS3270))
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

    }
}
