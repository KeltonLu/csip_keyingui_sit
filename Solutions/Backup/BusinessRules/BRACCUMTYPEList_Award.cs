//******************************************************************
//*  作    者：Ian Huang
//*  功能說明：ACCUMTYPEList_Award資料庫業務類
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
    public class BRACCUMTYPEList_Award : CSIPCommonModel.BusinessRules.BRBase<EntityACCUMTYPEList_Award>
    {
        #region SQL語句
        private const string SEL_ALL = "select ACCU_CODE as CODE,MEMO from ACCUMTYPEList_Award";

        private const string SEL_DEL = "delete from ACCUMTYPEList_Award where ACCU_CODE=@ACCU_CODE";
        #endregion

        public static DataSet Select(int iPageIndex, int iPageSize, string strCardCode, ref int iTotalCount)
        {
            try
            {
                string strWhere = "";

                if ("" != strCardCode.Trim())
                {
                    strWhere = " where " + EntityACCUMTYPEList_Award.M_ACCU_CODE + "='" + strCardCode + "'";
                }

                return BRACCUMTYPEList_Award.SearchOnDataSet(SEL_ALL + strWhere, iPageIndex, iPageSize, ref iTotalCount);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet SelectAll()
        {
            return BRACCUMTYPEList_Award.SearchOnDataSet(SEL_ALL);
        }


        public static EntitySet<EntityACCUMTYPEList_Award> Select(string strACCUCode)
        {
            try
            {
                SqlHelper sqlh = new SqlHelper();

                sqlh.AddCondition(EntityACCUMTYPEList_Award.M_ACCU_CODE, Operator.Equal, DataTypeUtils.String, strACCUCode);

                return BRACCUMTYPEList_Award.Search(sqlh.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Add(string strACCUCode, string strMEMO, ref string strMsg)
        {
            try
            {
                if (BRACCUMTYPEList_Award.Select(strACCUCode).Count > 0)
                {
                    strMsg = "01_04070000_001";
                    return false;
                }
                else
                {
                    EntityACCUMTYPEList_Award eATA = new EntityACCUMTYPEList_Award();

                    eATA.ACCU_CODE = strACCUCode;
                    eATA.MEMO = strMEMO;

                    return BRACCUMTYPEList_Award.AddNewEntity(eATA);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Remove(string strACCUCode)
        {
            try
            {
                SqlCommand sqlC = new SqlCommand();
                sqlC.CommandText = SEL_DEL;
                sqlC.CommandType = CommandType.Text;

                SqlParameter sqlP = new SqlParameter("@ACCU_CODE", strACCUCode);
                sqlC.Parameters.Add(sqlP);

                return BRACCUMTYPEList_Award.Delete(sqlC);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
