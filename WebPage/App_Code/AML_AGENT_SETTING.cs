//******************************************************************
//*  功能說明：案件歷程
//*  作    者：Leon Ho
//*  創建日期：2019/02/12
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using CSIPKeyInGUI.EntityLayer;
using System.Data.SqlClient;
using System.Data;
using CSIPNewInvoice.EntityLayer_new;
using Framework.Common.Utility;


public class AML_AGENT_SETTING : CSIPCommonModel.BusinessRules.BRBase<Entity_AML_AGENT_SETTING>
{

    /// <summary>
    /// 功能說明:新增一筆Job資料
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/26
    /// 修改記錄:
    /// </summary>
    /// <param name="FileInfo"></param>
    /// <returns></returns>
    public static bool Insert(Entity_AML_AGENT_SETTING oAML_AGENT_SETTING)
    {
        if (oAML_AGENT_SETTING != null)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (AML_AGENT_SETTING.AddNewEntity(oAML_AGENT_SETTING))
                    {
                        ts.Complete();
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception exp)
            {
                AML_AGENT_SETTING.SaveLog(exp.Message);
                return false;
            }
        }
        else
            return false;
    }

    /// <summary>
    /// 功能說明:刪除一筆資料
    /// 作    者:Leon Ho
    /// 創建時間:2019/02/12
    /// 修改記錄:
    /// </summary>
    /// <param name="strKey">範例:'00001','00002','00003'</param>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public static bool Delete_Obj(String strKey)
    {
        if (!String.IsNullOrEmpty(strKey))
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {

                    bool result = false;
                    String strSQL_Delete = String.Empty;
                    SqlCommand sqlcmd = new SqlCommand();

                    if (!String.IsNullOrEmpty(strKey))
                    {
                        strSQL_Delete = @" DELETE [dbo].[AML_AGENT_SETTING] WHERE USER_ID IN (@USER_ID)";
                        sqlcmd.Parameters.Add(new SqlParameter("@USER_ID", strKey));
                    }
                    sqlcmd.CommandType = CommandType.Text;
                    sqlcmd.CommandText = strSQL_Delete;

                    try
                    {
                        DataSet resultSet = AML_AGENT_SETTING.SearchOnDataSet(sqlcmd);
                        if (resultSet != null)
                        {
                            ts.Complete();
                            result = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        AML_AGENT_SETTING.SaveLog(ex.Message);
                    }

                    return result;
                }
            }
            catch (Exception exp)
            {
                AML_AGENT_SETTING.SaveLog(exp.Message);
                return false;
            }
        }
        else
            return false;
    }

    /// <summary>
    /// 功能說明:更新一筆資料
    /// 作    者:Leon Ho
    /// 創建時間:2019/02/12
    /// 修改記錄:
    /// </summary>
    /// <param name="oAML_AGENT_SETTING"></param>
    /// <param name="strKey"></param>
    /// <returns></returns>
    public static bool UPDATE_Obj(Entity_AML_AGENT_SETTING oAML_AGENT_SETTING)
    {
        bool result = false;
        String strSQL_UPDATE = String.Empty;
        String strSQL_Where = String.Empty;
        SqlCommand sqlcmd = new SqlCommand();

        try
        {
            using (OMTransactionScope ts = new OMTransactionScope())
            {
                if (oAML_AGENT_SETTING != null && !String.IsNullOrEmpty(oAML_AGENT_SETTING.USER_ID))
                {
                    strSQL_UPDATE = @" UPDATE [dbo].[AML_AGENT_SETTING] SET UPDATE_DATE = GETDATE() ";

                    strSQL_UPDATE += ",ASSIGN_RATE = @ASSIGN_RATE";
                    sqlcmd.Parameters.Add(new SqlParameter("@ASSIGN_RATE", oAML_AGENT_SETTING.ASSIGN_RATE));

                    strSQL_UPDATE += ",MEMO = @MEMO";
                    sqlcmd.Parameters.Add(new SqlParameter("@MEMO", oAML_AGENT_SETTING.MEMO));

                    strSQL_UPDATE += ",STOP_ASSIGN = @STOP_ASSIGN";
                    sqlcmd.Parameters.Add(new SqlParameter("@STOP_ASSIGN", oAML_AGENT_SETTING.STOP_ASSIGN));

                    strSQL_UPDATE += ",USER_STATUS = @USER_STATUS";
                    sqlcmd.Parameters.Add(new SqlParameter("@USER_STATUS", oAML_AGENT_SETTING.USER_STATUS));

                    strSQL_UPDATE += ",MODI_USER_ID = @MODI_USER_ID";
                    sqlcmd.Parameters.Add(new SqlParameter("@MODI_USER_ID", oAML_AGENT_SETTING.MODI_USER_ID));

                    strSQL_UPDATE += ",MODI_USER_NAME = @MODI_USER_NAME";
                    sqlcmd.Parameters.Add(new SqlParameter("@MODI_USER_NAME", oAML_AGENT_SETTING.MODI_USER_NAME));

                    strSQL_UPDATE += ",USER_EXTENSION = @USER_EXTENSION";
                    sqlcmd.Parameters.Add(new SqlParameter("@USER_EXTENSION", oAML_AGENT_SETTING.USER_EXTENSION));

                    strSQL_UPDATE += ",USER_TITLE = @USER_TITLE";
                    sqlcmd.Parameters.Add(new SqlParameter("@USER_TITLE", oAML_AGENT_SETTING.USER_TITLE));

                    strSQL_Where = " WHERE USER_ID = @USER_ID ";
                    sqlcmd.Parameters.Add(new SqlParameter("@USER_ID", oAML_AGENT_SETTING.USER_ID));
                }
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = strSQL_UPDATE + strSQL_Where;

                DataSet resultSet = AML_AGENT_SETTING.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                    ts.Complete();
                }
            }
        }
        catch (Exception exp)
        {
            AML_AGENT_SETTING.SaveLog(exp.Message);
            result = false;
        }
        return result;
    }

    public static bool UPDATE_Obj_V2(Entity_AML_AGENT_SETTING oAML_AGENT_SETTING)
    {
        bool result = false;
        String strSQL_UPDATE = String.Empty;
        String strSQL_Where = String.Empty;
        SqlCommand sqlcmd = new SqlCommand();

        try
        {
            using (OMTransactionScope ts = new OMTransactionScope())
            {
                if (oAML_AGENT_SETTING != null && !String.IsNullOrEmpty(oAML_AGENT_SETTING.USER_ID))
                {
                    strSQL_UPDATE = @" UPDATE [dbo].[AML_AGENT_SETTING] SET UPDATE_DATE = GETDATE() ";

                    strSQL_UPDATE += ",ASSIGN_RATE = @ASSIGN_RATE";
                    sqlcmd.Parameters.Add(new SqlParameter("@ASSIGN_RATE", oAML_AGENT_SETTING.ASSIGN_RATE));

                    strSQL_UPDATE += ",USER_STATUS = @USER_STATUS";
                    sqlcmd.Parameters.Add(new SqlParameter("@USER_STATUS", oAML_AGENT_SETTING.USER_STATUS));

                    //strSQL_UPDATE += ",MEMO = @MEMO";
                    //sqlcmd.Parameters.Add(new SqlParameter("@MEMO", oAML_AGENT_SETTING.MEMO));

                    //strSQL_UPDATE += ",STOP_ASSIGN = @STOP_ASSIGN";
                    //sqlcmd.Parameters.Add(new SqlParameter("@STOP_ASSIGN", oAML_AGENT_SETTING.STOP_ASSIGN));

                    strSQL_UPDATE += ",MODI_USER_ID = @MODI_USER_ID";
                    sqlcmd.Parameters.Add(new SqlParameter("@MODI_USER_ID", oAML_AGENT_SETTING.MODI_USER_ID));

                    strSQL_UPDATE += ",MODI_USER_NAME = @MODI_USER_NAME";
                    sqlcmd.Parameters.Add(new SqlParameter("@MODI_USER_NAME", oAML_AGENT_SETTING.MODI_USER_NAME));

                    //strSQL_UPDATE += ",USER_EXTENSION = @USER_EXTENSION";
                    //sqlcmd.Parameters.Add(new SqlParameter("@USER_EXTENSION", oAML_AGENT_SETTING.USER_EXTENSION));

                    //strSQL_UPDATE += ",USER_TITLE = @USER_TITLE";
                    //sqlcmd.Parameters.Add(new SqlParameter("@USER_TITLE", oAML_AGENT_SETTING.USER_TITLE));

                    strSQL_Where = " WHERE USER_ID = @USER_ID ";
                    sqlcmd.Parameters.Add(new SqlParameter("@USER_ID", oAML_AGENT_SETTING.USER_ID));
                }
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = strSQL_UPDATE + strSQL_Where;

                result = AML_AGENT_SETTING.Delete(sqlcmd);

                if (result)
                    ts.Complete();
            }
        }
        catch (Exception exp)
        {
            AML_AGENT_SETTING.SaveLog(exp.Message);
            result = false;
        }
        return result;
    }

    public static bool UPDATE_Obj_List(List<Entity_AML_AGENT_SETTING> listAML_AGENT_SETTING, ref int iSuccessCount)
    {
        bool result = false;
        String strSQL_UPDATE = String.Empty;
        String strSQL_Where = String.Empty;
        SqlCommand sqlcmd = new SqlCommand();
        iSuccessCount = 0;

        try
        {
            if (listAML_AGENT_SETTING != null && listAML_AGENT_SETTING.Count > 0)
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    foreach (Entity_AML_AGENT_SETTING loopObj in listAML_AGENT_SETTING)
                    {
                        if (GetDataCount(loopObj) > 0)
                        {
                            //修改
                            if (UPDATE_Obj_V2(loopObj))
                                iSuccessCount++;
                        }
                        else
                        {
                            //新增
                            if (Insert(loopObj))
                                iSuccessCount++;
                        }

                        //if (loopObj != null && !String.IsNullOrEmpty(loopObj.USER_ID))
                        //{
                        //    strSQL_UPDATE = @" UPDATE [dbo].[AML_AGENT_SETTING] SET UPDATE_DATE = GETDATE() ";

                        //    strSQL_UPDATE += ",ASSIGN_RATE = @ASSIGN_RATE";
                        //    sqlcmd.Parameters.Add(new SqlParameter("@ASSIGN_RATE", loopObj.ASSIGN_RATE));

                        //    strSQL_UPDATE += ",MEMO = @MEMO";
                        //    sqlcmd.Parameters.Add(new SqlParameter("@MEMO", loopObj.MEMO));

                        //    strSQL_UPDATE += ",STOP_ASSIGN = @STOP_ASSIGN";
                        //    sqlcmd.Parameters.Add(new SqlParameter("@STOP_ASSIGN", loopObj.STOP_ASSIGN));

                        //    strSQL_UPDATE += ",USER_STATUS = @USER_STATUS";
                        //    sqlcmd.Parameters.Add(new SqlParameter("@USER_STATUS", loopObj.USER_STATUS));

                        //    strSQL_UPDATE += ",MODI_USER_ID = @MODI_USER_ID";
                        //    sqlcmd.Parameters.Add(new SqlParameter("@MODI_USER_ID", loopObj.MODI_USER_ID));

                        //    strSQL_UPDATE += ",MODI_USER_NAME = @MODI_USER_NAME";
                        //    sqlcmd.Parameters.Add(new SqlParameter("@MODI_USER_NAME", loopObj.MODI_USER_NAME));

                        //    strSQL_UPDATE += ",USER_EXTENSION = @USER_EXTENSION";
                        //    sqlcmd.Parameters.Add(new SqlParameter("@USER_EXTENSION", loopObj.USER_EXTENSION));

                        //    strSQL_UPDATE += ",USER_TITLE = @USER_TITLE";
                        //    sqlcmd.Parameters.Add(new SqlParameter("@USER_TITLE", loopObj.USER_TITLE));

                        //    strSQL_Where = " WHERE USER_ID = @USER_ID ";
                        //    sqlcmd.Parameters.Add(new SqlParameter("@USER_ID", loopObj.USER_ID));
                        //}
                        //sqlcmd.CommandType = CommandType.Text;
                        //sqlcmd.CommandText = strSQL_UPDATE + strSQL_Where;

                        //result = AML_AGENT_SETTING.Delete(sqlcmd);

                        //if (result)
                        //    iSuccessCount++;
                    }

                    if (iSuccessCount == listAML_AGENT_SETTING.Count)
                    {
                        ts.Complete();
                        result = true;
                    }
                }
            }
        }
        catch (Exception exp)
        {
            AML_AGENT_SETTING.SaveLog(exp.Message);
        }
        return result;
    }

    private static int GetDataCount(Entity_AML_AGENT_SETTING oAML_AGENT_SETTING)
    {
        int iDataCount = 0;

        if (oAML_AGENT_SETTING != null)
        {
            try
            {
                SqlCommand sqlcmd = new SqlCommand();
                string sql = @"SELECT *
                               FROM [dbo].[AML_AGENT_SETTING]
                               WHERE 1 = 1";

                if (!String.IsNullOrEmpty(oAML_AGENT_SETTING.USER_ID))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@USER_ID", oAML_AGENT_SETTING.USER_ID));
                    sql += " AND USER_ID = @USER_ID ";
                }

                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = AML_AGENT_SETTING.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    iDataCount = ds.Tables[0].Rows.Count;
                    ds.Clear();
                    ds.Dispose();
                }
            }
            catch (Exception exp)
            {
                AML_AGENT_SETTING.SaveLog(exp.Message);
            }
        }
        return iDataCount;
    }

    /// <summary>
    /// 功能說明:取得AML_AGENT_SETTING資料
    /// 作    者:Leon Ho
    /// 創建時間:2019/02/12
    /// 修改記錄:
    /// </summary>
    /// <param name="oAML_AGENT_SETTING">查詢條件</param>
    /// <param name="oOrderColumn">排序欄位(可條件、可升降冪)，寫法如SQL，不須加Order by</param>
    /// <param name="tempDataTable">回傳查詢結果</param>
    /// <returns></returns>
    public static bool GetAML_AGENT_SETTINGDataTable(Entity_AML_AGENT_SETTING oAML_AGENT_SETTING, string oOrderColumn, ref  DataTable tempDataTable)
    {
        if (oAML_AGENT_SETTING != null)
        {
            try
            {
                SqlCommand sqlcmd = new SqlCommand();
                string sql = @"SELECT *
                               FROM [dbo].[AML_AGENT_SETTING]
                               WHERE 1 = 1";

                if (!String.IsNullOrEmpty(oAML_AGENT_SETTING.USER_ID))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@USER_ID", oAML_AGENT_SETTING.USER_ID));
                    sql += " AND USER_ID = @USER_ID ";
                }

                if (!String.IsNullOrEmpty(oAML_AGENT_SETTING.USER_STATUS))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@USER_STATUS", oAML_AGENT_SETTING.USER_STATUS));
                    sql += " AND USER_STATUS = @USER_STATUS ";
                }

                if (!String.IsNullOrEmpty(oAML_AGENT_SETTING.STOP_ASSIGN))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@STOP_ASSIGN", oAML_AGENT_SETTING.STOP_ASSIGN));
                    sql += " AND STOP_ASSIGN = @STOP_ASSIGN ";
                }

                if (!String.IsNullOrEmpty(oAML_AGENT_SETTING.USER_NAME))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@USER_NAME", oAML_AGENT_SETTING.USER_NAME));
                    sql += " AND USER_NAME = @USER_NAME ";
                }

                string[] tempArry = oOrderColumn.Split(',');
                string[] arrayOrderColumn = { "USER_ID", "USER_NAME", "ASSIGN_RATE", "STOP_ASSIGN", "USER_STATUS"};
                string strOrder = string.Empty;

                for (int i = 0; i < tempArry.Length; i++)
                {
                    for (int j = 0; j < arrayOrderColumn.Length; j++)
                    {
                        if (tempArry[i].IndexOf(arrayOrderColumn[j]) > -1)
                        {
                            if (strOrder != string.Empty)
                            {
                                strOrder += ",";
                            }
                            strOrder += tempArry[i];
                        }
                    }
                }

                if (strOrder != string.Empty)
                {
                    sql += "ORDER BY " + strOrder;
                }

                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = AML_AGENT_SETTING.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    tempDataTable = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                AML_AGENT_SETTING.SaveLog(exp.Message);
                return false;
            }
        }
        else
            return false;
    }

    /// <summary>
    /// 功能說明:取得AML_AGENT_SETTING資料(Web)
    /// 作    者:Leon Ho
    /// 創建時間:2019/02/12
    /// 修改記錄:
    /// </summary>
    /// <param name="oAML_AGENT_SETTING">查詢條件</param>
    /// <param name="oOrderColumn">排序欄位(可條件、可升降冪)，寫法如SQL，不須加Order by</param>
    /// <param name="tempDataTable">回傳查詢結果</param>
    /// <returns></returns>
    public static bool GetAML_AGENT_SETTING_WebListDataTable(Entity_AML_AGENT_SETTING oAML_AGENT_SETTING, string oRoleID, string oOrderColumn, ref  DataTable tempDataTable)
    {
        if (oAML_AGENT_SETTING != null && !String.IsNullOrEmpty(oRoleID))
        {
            try
            {
                SqlCommand sqlcmd = new SqlCommand();
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                string sql = string.Format(@"SELECT (CASE WHEN TBA.[USER_ID] IS NULL THEN '失效' WHEN TBB.[USER_ID] IS NULL THEN '新增' ELSE '編輯' END) AS [New_STATUS]
                               ,(CASE WHEN TBA.[USER_ID] IS NULL THEN TBB.[USER_ID] ELSE TBA.[USER_ID] END) AS [USER_ID]
                               ,(CASE WHEN TBA.[USER_NAME] IS NULL THEN TBB.[USER_NAME] ELSE TBA.[USER_NAME] END) AS [USER_NAME]
                               ,(CASE WHEN ASSIGN_RATE IS NULL THEN 0 ELSE ASSIGN_RATE END) AS ASSIGN_RATE
                               ,MEMO,STOP_ASSIGN,USER_STATUS,USER_EXTENSION,USER_TITLE
                               ,ADD_USER_ID,ADD_USER_NAME,MODI_USER_ID,MODI_USER_NAME,USER_TYPE,ASSIGN_RATE_MERCHANT,ADD_DATE,UPDATE_DATE
                               FROM (
                                 SELECT [USER_ID],[USER_NAME]
                                 FROM [{1}].[dbo].[M_USER]
                                 WHERE ROLE_ID = @ROLE_ID
                               ) TBA
                               FULL JOIN [{0}].[dbo].[AML_AGENT_SETTING] TBB ON TBA.[USER_ID] = TBB.[USER_ID]
                               WHERE 1 = 1", UtilHelper.GetAppSettings("DB_KeyinGUI"), UtilHelper.GetAppSettings("DB_CSIP"));

                sqlcmd.Parameters.Add(new SqlParameter("@ROLE_ID", oRoleID));

                if (!String.IsNullOrEmpty(oAML_AGENT_SETTING.USER_ID))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@USER_ID", oAML_AGENT_SETTING.USER_ID));
                    sql += " AND USER_ID = @USER_ID ";
                }

                if (!String.IsNullOrEmpty(oAML_AGENT_SETTING.USER_STATUS))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@USER_STATUS", oAML_AGENT_SETTING.USER_STATUS));
                    sql += " AND USER_STATUS = @USER_STATUS ";
                }

                if (!String.IsNullOrEmpty(oAML_AGENT_SETTING.STOP_ASSIGN))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@STOP_ASSIGN", oAML_AGENT_SETTING.STOP_ASSIGN));
                    sql += " AND STOP_ASSIGN = @STOP_ASSIGN ";
                }

                if (!String.IsNullOrEmpty(oAML_AGENT_SETTING.USER_NAME))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@USER_NAME", oAML_AGENT_SETTING.USER_NAME));
                    sql += " AND USER_NAME = @USER_NAME ";
                }

                string[] tempArry = oOrderColumn.Split(',');
                string[] arrayOrderColumn = { "USER_ID", "USER_NAME", "ASSIGN_RATE", "STOP_ASSIGN", "USER_STATUS" };
                string strOrder = string.Empty;

                for (int i = 0; i < tempArry.Length; i++)
                {
                    for (int j = 0; j < arrayOrderColumn.Length; j++)
                    {
                        if (tempArry[i].IndexOf(arrayOrderColumn[j]) > -1)
                        {
                            if (strOrder != string.Empty)
                            {
                                strOrder += ",";
                            }
                            strOrder += tempArry[i];
                        }
                    }
                }

                if (strOrder != string.Empty)
                {
                    sql += "ORDER BY " + strOrder;
                }
                else
                {
                    sql += "ORDER BY (CASE WHEN TBA.[USER_ID] IS NULL THEN '9' WHEN TBB.[USER_ID] IS NULL THEN '0' ELSE '1' END),TBA.[USER_ID],TBB.[USER_ID] ";
                }

                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = AML_AGENT_SETTING.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    tempDataTable = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                AML_AGENT_SETTING.SaveLog(ex.Message);
                return false;
            }
        }
        else
            return false;
    }

    /// <summary>
    /// 功能說明:取得AML_AGENT_SETTING資料(Web)
    /// 作    者:Leon Ho
    /// 創建時間:2019/02/12
    /// 修改記錄:
    /// </summary>
    /// <param name="oAML_AGENT_SETTING">查詢條件</param>
    /// <param name="oOrderColumn">排序欄位(可條件、可升降冪)，寫法如SQL，不須加Order by</param>
    /// <param name="tempDataTable">回傳查詢結果</param>
    /// <returns></returns>
    public static bool GetAML_AGENT_SETTING_WebListDataTable_V2(Entity_AML_AGENT_SETTING oAML_AGENT_SETTING, string oRoleID, string oOrderColumn, ref  DataTable tempDataTable)
    {
        if (oAML_AGENT_SETTING != null && !String.IsNullOrEmpty(oRoleID))
        {
            try
            {
                SqlCommand sqlcmd = new SqlCommand();
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                string sql = string.Format(@"
                               SELECT *
                               FROM(
                                SELECT (CASE WHEN TBA.[USER_ID] IS NULL THEN TBB.[USER_ID] ELSE TBA.[USER_ID] END) AS [USER_ID]
                                ,(CASE WHEN TBA.[USER_NAME] IS NULL THEN TBB.[USER_NAME] ELSE TBA.[USER_NAME] END) AS [USER_NAME]
                                ,(CASE WHEN TBA.[USER_NAME] IS NULL THEN TBB.[USER_NAME] ELSE TBA.[USER_NAME] END) + '(' + (CASE WHEN TBA.[USER_ID] IS NULL THEN TBB.[USER_ID] ELSE TBA.[USER_ID] END) + ')' AS [Show_NAME]
                                ,(CASE WHEN ASSIGN_RATE IS NULL THEN 0 ELSE ASSIGN_RATE END) AS ASSIGN_RATE
                                --,(CASE WHEN TBA.[USER_ID] IS NULL THEN '失效' WHEN TBB.[USER_ID] IS NULL THEN '新增' ELSE '編輯' END) AS [New_STATUS]
                                ,(CASE WHEN TBB.[USER_ID] IS NULL THEN '1' ELSE USER_STATUS END) AS USER_STATUS
                                FROM (
                                    SELECT [USER_ID],[USER_NAME]
                                    FROM [{1}].[dbo].[M_USER]
                                    WHERE ROLE_ID = @ROLE_ID
                                ) TBA
                                FULL JOIN [{0}].[dbo].[AML_AGENT_SETTING] TBB ON TBA.[USER_ID] = TBB.[USER_ID]
                               ) TBM
                               WHERE 1 = 1", UtilHelper.GetAppSettings("DB_KeyinGUI"), UtilHelper.GetAppSettings("DB_CSIP"));

                sqlcmd.Parameters.Add(new SqlParameter("@ROLE_ID", oRoleID));

                if (!String.IsNullOrEmpty(oAML_AGENT_SETTING.USER_ID))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@USER_ID", oAML_AGENT_SETTING.USER_ID));
                    sql += " AND USER_ID = @USER_ID ";
                }

                if (!String.IsNullOrEmpty(oAML_AGENT_SETTING.USER_STATUS))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@USER_STATUS", oAML_AGENT_SETTING.USER_STATUS));
                    sql += " AND USER_STATUS = @USER_STATUS ";
                }

                if (!String.IsNullOrEmpty(oAML_AGENT_SETTING.STOP_ASSIGN))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@STOP_ASSIGN", oAML_AGENT_SETTING.STOP_ASSIGN));
                    sql += " AND STOP_ASSIGN = @STOP_ASSIGN ";
                }

                if (!String.IsNullOrEmpty(oAML_AGENT_SETTING.USER_NAME))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@USER_NAME", oAML_AGENT_SETTING.USER_NAME));
                    sql += " AND USER_NAME = @USER_NAME ";
                }

                string[] tempArry = oOrderColumn.Split(',');
                string[] arrayOrderColumn = { "USER_ID", "USER_NAME", "ASSIGN_RATE", "STOP_ASSIGN", "USER_STATUS" };
                string strOrder = string.Empty;

                for (int i = 0; i < tempArry.Length; i++)
                {
                    for (int j = 0; j < arrayOrderColumn.Length; j++)
                    {
                        if (tempArry[i].IndexOf(arrayOrderColumn[j]) > -1)
                        {
                            if (strOrder != string.Empty)
                            {
                                strOrder += ",";
                            }
                            strOrder += tempArry[i];
                        }
                    }
                }

                if (strOrder != string.Empty)
                {
                    sql += "ORDER BY " + strOrder;
                }
                else
                {
                    sql += "ORDER BY [USER_ID] ";
                }

                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = AML_AGENT_SETTING.SearchOnDataSet(sqlcmd);
                if (ds != null)
                {
                    tempDataTable = ds.Tables[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                AML_AGENT_SETTING.SaveLog(ex.Message);
                return false;
            }
        }
        else
            return false;
    }

    public static bool GetAML_AGENT_SETTING_WebListObj(Entity_AML_AGENT_SETTING oAML_AGENT_SETTING, string oRoleID, string oOrderColumn, ref  List<Entity_AML_AGENT_SETTING> listEntity_AML_AGENT_SETTING)
    {
        DataTable dtTemp = new DataTable();
        try
        {
            if (oAML_AGENT_SETTING != null && !String.IsNullOrEmpty(oRoleID))
            {
                if (GetAML_AGENT_SETTING_WebListDataTable(oAML_AGENT_SETTING, oRoleID, oOrderColumn, ref dtTemp))
                {
                    if (dtTemp != null && dtTemp.Rows.Count > 0)
                    {
                        for(int i = 0;i < dtTemp.Rows.Count;i++)
                        {
                            Entity_AML_AGENT_SETTING loopObj = new Entity_AML_AGENT_SETTING();
                            loopObj.ADD_USER_ID = dtTemp.Rows[i]["ADD_USER_ID"].ToString();
                            loopObj.ADD_USER_NAME = dtTemp.Rows[i]["ADD_USER_NAME"].ToString();
                            loopObj.MEMO = dtTemp.Rows[i]["MEMO"].ToString();
                            loopObj.MODI_USER_ID = dtTemp.Rows[i]["MODI_USER_ID"].ToString();
                            loopObj.MODI_USER_NAME = dtTemp.Rows[i]["MODI_USER_NAME"].ToString();
                            loopObj.STOP_ASSIGN = dtTemp.Rows[i]["STOP_ASSIGN"].ToString();
                            loopObj.USER_EXTENSION = dtTemp.Rows[i]["USER_EXTENSION"].ToString();
                            loopObj.USER_ID = dtTemp.Rows[i]["USER_ID"].ToString();
                            loopObj.USER_NAME = dtTemp.Rows[i]["USER_NAME"].ToString();
                            loopObj.USER_STATUS = dtTemp.Rows[i]["USER_STATUS"].ToString();
                            loopObj.USER_TITLE = dtTemp.Rows[i]["USER_TITLE"].ToString();
                            loopObj.USER_TYPE = dtTemp.Rows[i]["USER_TYPE"].ToString();
                            loopObj.ASSIGN_RATE_MERCHANT = dtTemp.Rows[i]["ASSIGN_RATE_MERCHANT"].ToString();

                            try
                            {
                                if (!String.IsNullOrEmpty(dtTemp.Rows[i]["ASSIGN_RATE"].ToString()))
                                {
                                    loopObj.ASSIGN_RATE = Convert.ToInt32(dtTemp.Rows[i]["ASSIGN_RATE"].ToString());
                                }
                                if (!String.IsNullOrEmpty(dtTemp.Rows[i]["ADD_DATE"].ToString()))
                                {                                    
                                    loopObj.ADD_DATE = Convert.ToDateTime(dtTemp.Rows[i]["ADD_DATE"].ToString());
                                }
                                if (!String.IsNullOrEmpty(dtTemp.Rows[i]["UPDATE_DATE"].ToString()))
                                {
                                    loopObj.UPDATE_DATE = Convert.ToDateTime(dtTemp.Rows[i]["UPDATE_DATE"].ToString());
                                }
                            }
                            catch (Exception ex)
                            {
                                AML_AGENT_SETTING.SaveLog("第" + i.ToString() + "筆轉型錯誤");
                                AML_AGENT_SETTING.SaveLog(ex.Message); 
                            }
                            listEntity_AML_AGENT_SETTING.Add(loopObj);
                        }
                    }
                }
            }

            dtTemp.Clear();
            dtTemp.Dispose();
            return true;
        }
        catch (Exception ex)
        {
            AML_AGENT_SETTING.SaveLog(ex.Message);
            return false;
        }
    }

}
