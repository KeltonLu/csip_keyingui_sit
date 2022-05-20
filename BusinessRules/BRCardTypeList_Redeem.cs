//******************************************************************
//*  作    者：Ian Huang
//*  功能說明：CardTypeList_Redeem資料庫業務類
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
    public class BRCardTypeList_Redeem : CSIPCommonModel.BusinessRules.BRBase<EntityCardTypeList_Redeem>
    {
        #region SQL語句
        private const string SEL_ALL = "select Card_CODE as CODE,MEMO from CardTypeList_Redeem";

        private const string SEL_DEL = "delete from CardTypeList_Redeem where Card_CODE=@Card_CODE";
        #endregion

        public static DataSet Select(int iPageIndex, int iPageSize, string strCardCode, ref int iTotalCount)
        {
            try
            {
                string strWhere = "";

                if ("" != strCardCode.Trim())
                {
                    strWhere = " where " + EntityCardTypeList_Redeem.M_Card_CODE + "='" + strCardCode + "'";
                }

                return BRCardTypeList_Redeem.SearchOnDataSet(SEL_ALL + strWhere, iPageIndex, iPageSize, ref iTotalCount);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static EntitySet<EntityCardTypeList_Redeem> Select(string strCardCode)
        {
            try
            {
                SqlHelper sqlh = new SqlHelper();

                sqlh.AddCondition(EntityCardTypeList_Redeem.M_Card_CODE, Operator.Equal, DataTypeUtils.String, strCardCode);

                return BRCardTypeList_Redeem.Search(sqlh.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Add(string strCardCode, string strMEMO, ref string strMsg)
        {
            try
            {
                if (BRCardTypeList_Redeem.Select(strCardCode).Count > 0)
                {
                    strMsg = "01_04070000_001";
                    return false;
                }
                else
                {
                    EntityCardTypeList_Redeem eCTR = new EntityCardTypeList_Redeem();

                    eCTR.Card_CODE = strCardCode;
                    eCTR.MEMO = strMEMO;

                    return BRCardTypeList_Redeem.AddNewEntity(eCTR);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Remove(string strCardCode)
        {
            try
            {
                SqlCommand sqlC = new SqlCommand();
                sqlC.CommandText = SEL_DEL;
                sqlC.CommandType = CommandType.Text;

                SqlParameter sqlP = new SqlParameter("@Card_CODE", strCardCode);
                sqlC.Parameters.Add(sqlP);

                return BRCardTypeList_Redeem.Delete(sqlC);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
