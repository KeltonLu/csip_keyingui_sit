//******************************************************************
//*  作    者：蘇洺葳
//*  功能說明：屬性標識
//*  創建日期：2018/08/03
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM;
using CSIPCommonModel.EntityLayer;
using System.Data;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using System.Data.SqlClient;
using CSIPCommonModel.BusinessRules;

namespace CSIPCommonModel.BusinessRules_new
{
    public class BRM_PROPERTY_KEY : BRBase<EntityM_PROPERTY_KEY>
    {
        #region SQL

        /// <summary>
        /// 查詢自動化表單大項內的表單類別
        /// </summary>
        private const string SEL_FORM = @"SELECT PROPERTY_CODE,PROPERTY_NAME ,OFF_FLAG OFF_FLAG FROM [M_PROPERTY_CODE] WHERE 
                                            FUNCTION_KEY = @FUNCKEY AND PROPERTY_KEY = @PROPERTY_KEY AND LEN(PROPERTY_CODE) = 3 
                                            ORDER BY SEQUENCE ";

        #endregion

        /// <summary>
        /// 通過子系統ID和屬性KEY取得某個屬性Key下所有的資料
        /// </summary>
        /// <param name="strFuncKey">子系統ID</param>
        /// <param name="strPropertyKey">屬性KEY</param>
        /// <param name="dtblResult">屬性列表</param>
        /// <returns>True - 成功 / False - 失敗</returns>
        public static bool GetProperty(string strFuncKey, string strPropertyKey, ref DataTable dtblResult)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = SEL_FORM;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@FUNCKEY", strFuncKey));
            sqlcmd.Parameters.Add(new SqlParameter("@PROPERTY_KEY", strPropertyKey));
            
            try
            {
                dtblResult = SearchOnDataSet(sqlcmd, "Connection_CSIP").Tables[0];
                return true;
            }
            catch (Exception exp)
            {
                SaveLog(exp);
                return false;
            }
        }
    }
}
