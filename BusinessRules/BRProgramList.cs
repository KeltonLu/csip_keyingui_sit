//******************************************************************
//*  作    者：Ian Huang
//*  功能說明：ProgramList資料庫業務類
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
    public class BRProgramList : CSIPCommonModel.BusinessRules.BRBase<EntityProgramList>
    {
        #region SQL語句
        private const string SEL_DEL = "delete from ProgramList where CODE=@CODE";
        #endregion

        public static EntitySet<EntityProgramList> Select(string strCODE)
        {
            try
            {
                SqlHelper sqlh = new SqlHelper();
                sqlh.AddCondition(EntityProgramList.M_CODE, Operator.Equal, DataTypeUtils.String, strCODE);

                return BRProgramList.Search(sqlh.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static bool Add(string strCODE, string strMEMO, ref string strMSG)
        {
            try
            {
                if (BRProgramList.Select(strCODE).Count > 0)
                {
                    strMSG = "01_04070000_001";
                    return false;
                }
                else
                {
                    EntityProgramList ePL = new EntityProgramList();

                    ePL.CODE = strCODE;
                    ePL.MEMO = strMEMO;

                    return BRProgramList.AddNewEntity(ePL);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Remove(string strCode)
        {
            try
            {
                SqlCommand sqlC = new SqlCommand();
                sqlC.CommandText = SEL_DEL;
                sqlC.CommandType = CommandType.Text;

                SqlParameter sqlP = new SqlParameter("@CODE", strCode);
                sqlC.Parameters.Add(sqlP);

                return BRProgramList.Delete(sqlC);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
