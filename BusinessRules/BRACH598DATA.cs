//******************************************************************
//*  作    者：chenjingxian

//*  功能說明：ACH598DATA申請類別與ID號檢核

//*  創建日期：2009/12/28
//*  修改記錄：


//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************


using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Framework.Data.OM;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;



namespace CSIPKeyInGUI.BusinessRules
{
    public class BRACH598DATA : CSIPCommonModel.BusinessRules.BRBase<EntityACH598DATA>
    {
        public const string SEL_ID = "select distinct ID from Ach598Data where ID = @UserId ";

        /// <summary>
        /// 查詢身分證號碼
        /// </summary>
        /// <param name="strUserId">身分證號碼</param>
        /// <returns>DataSet</returns>
        public static DataSet Select(string strUserId)
        {
            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandText = SEL_ID;
            sqlComm.CommandType = CommandType.Text;

            SqlParameter parmUserId = new SqlParameter("@UserId", strUserId);
            sqlComm.Parameters.Add(parmUserId);

            return BROTHER_BANK_TEMP.SearchOnDataSet(sqlComm);
        }
    }
}
