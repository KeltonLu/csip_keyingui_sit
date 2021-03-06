//******************************************************************
//*  作    者：蘇洺葳
//*  功能說明：屬性
//*  創建日期：2018/02/13
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
using Framework.Common.Message;

namespace CSIPCommonModel.BusinessRules_new
{
    public class BRM_PROPERTY_CODE : CSIPCommonModel.BusinessRules.BRBase<EntityM_PROPERTY_CODE>
    {
        #region SQL

        /// <summary>
        /// 查詢自動化表單細項內的業務類別
        /// </summary>
        private const string SEL_FROM_BUSINESS = @"SELECT PROPERTY_CODE, PROPERTY_NAME, OFF_FLAG FROM [M_PROPERTY_CODE] 
                                                    WHERE FUNCTION_KEY = @FUNCKEY AND PROPERTY_KEY = @PROPERTY_KEY AND LEN(PROPERTY_CODE) = 6 
                                                    AND SUBSTRING(PROPERTY_CODE, 1, 3) = @PARENT_CODE ORDER BY SEQUENCE ";

        /// <summary>
        /// 查詢所有業務類別
        /// </summary>
        private const string SEL_FROM_BUSINESS_ALL = @"SELECT PROPERTY_CODE, PROPERTY_NAME, OFF_FLAG FROM [M_PROPERTY_CODE] 
                                                        WHERE FUNCTION_KEY = @FUNCKEY AND PROPERTY_KEY = @PROPERTY_KEY AND LEN(PROPERTY_CODE) = 6 
                                                        ORDER BY PROPERTY_CODE ";

        /// <summary>
        /// 查詢啟用的屬性
        /// </summary>
        private const string SEL_ENABLEPROPROPERTY = @"SELECT PROPERTY_CODE, PROPERTY_NAME FROM [M_PROPERTY_CODE] MC, [M_PROPERTY_KEY] MK 
                                                        WHERE MC.FUNCTION_KEY = MK.FUNCTION_KEY 
                                                        AND MC.PROPERTY_KEY = MK.PROPERTY_KEY 
                                                        AND MC.FUNCTION_KEY = @FUNCTION_KEY 
                                                        AND MC.OFF_FLAG = @OFF_FLAG 
                                                        AND MK.OFF_FLAG = @OFF_FLAG 
                                                        AND MC.PROPERTY_KEY in (";

        #endregion

        /// <summary>
        /// 通過子系統ID和屬性KEY取得某個屬性Key下所有的資料
        /// </summary>
        /// <param name="strFuncKey">功能標識編號</param>
        /// <param name="strPropertyKey">屬性標識編號</param>
        /// <param name="dtblResult">業務類別DataTable</param>
        /// <returns></returns>
        public static bool GetProperty(string strFuncKey, string strPropertyKey, string strParentCode, ref DataTable dtblResult)
        {
            SqlCommand sqlcmd = new SqlCommand();

            if (string.IsNullOrEmpty(strParentCode))
            {
                sqlcmd.CommandText = SEL_FROM_BUSINESS_ALL;
            }
            else
            {
                sqlcmd.CommandText = SEL_FROM_BUSINESS;
                sqlcmd.Parameters.Add(new SqlParameter("@PARENT_CODE", strParentCode));
            }

            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@FUNCKEY", strFuncKey));
            sqlcmd.Parameters.Add(new SqlParameter("@PROPERTY_KEY", strPropertyKey));

            try
            {
                dtblResult = SearchOnDataSet(sqlcmd, "Connection_CSIP").Tables[0];
                return true;
            }
            catch (Exception ex)
            {
                SaveLog(ex);
                return false;
            }
        }

        /// <summary>
        /// 功能:通過子系統ID和屬性KEY取得某個屬性Key下所有處於啟用狀態的屬性資料
        /// </summary>
        /// <param name="strFuncKey"></param>
        /// <param name="strPropertyKey"></param>
        /// <param name="dtblResult"></param>
        /// <returns></returns>
        public static bool GetEnableProperty(string strFuncKey, string[] strPropertyKey, ref DataTable dtblResult)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = SEL_ENABLEPROPROPERTY;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@FUNCTION_KEY", strFuncKey));
            sqlcmd.Parameters.Add(new SqlParameter("@OFF_FLAG", "1"));

            int count = 0;
            foreach (string item in strPropertyKey)
            {
                sqlcmd.Parameters.Add(new SqlParameter("@PROPERTY_KEY" + count, item));
                sqlcmd.CommandText += "@PROPERTY_KEY" + count + ",";

                count++;
            }
            sqlcmd.CommandText = sqlcmd.CommandText.Remove(sqlcmd.CommandText.Length - 1, 1);

            sqlcmd.CommandText += ") ORDER BY SEQUENCE ";

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
