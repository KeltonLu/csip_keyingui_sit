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


public class SCDDReport : CSIPCommonModel.BusinessRules.BRBase<Entity_SCDDReport>
{

    /// <summary>
    /// 功能說明:新增一筆Job資料
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/26
    /// 修改記錄:
    /// </summary>
    /// <param name="FileInfo"></param>
    /// <returns></returns>
    public static bool Insert(Entity_SCDDReport oSCDDReport)
    {
        if (oSCDDReport != null)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (SCDDReport.AddNewEntity(oSCDDReport))
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
                SCDDReport.SaveLog(exp.Message);
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
                        strSQL_Delete = @" DELETE [dbo].[SCDDReport] WHERE SR_CASE_NO IN (@SR_CASE_NO)";
                        sqlcmd.Parameters.Add(new SqlParameter("@SR_CASE_NO", strKey));
                    }
                    sqlcmd.CommandType = CommandType.Text;
                    sqlcmd.CommandText = strSQL_Delete;

                    try
                    {
                        DataSet resultSet = SCDDReport.SearchOnDataSet(sqlcmd);
                        if (resultSet != null)
                        {
                            ts.Complete();
                            result = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        SCDDReport.SaveLog(ex.Message);
                    }

                    return result;
                }
            }
            catch (Exception exp)
            {
                SCDDReport.SaveLog(exp.Message);
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
    /// <param name="oSCDDReport_UPDATE"></param>
    /// <param name="strKey"></param>
    /// <returns></returns>
    public static bool UPDATE_Obj(Entity_SCDDReport oSCDDReport_UPDATE, String strKey)
    {
        bool result = false;
        String strSQL_UPDATE = String.Empty;
        String strSQL_Where = String.Empty;
        SqlCommand sqlcmd = new SqlCommand();

        try
        {
            using (OMTransactionScope ts = new OMTransactionScope())
            {
                if (oSCDDReport_UPDATE != null && !String.IsNullOrEmpty(strKey))
                {
                    strSQL_UPDATE = @" UPDATE [dbo].[SCDDReport] SET SR_DateTime = GETDATE() ";

                    strSQL_UPDATE += ",SR_Explanation = @SR_Explanation";
                    sqlcmd.Parameters.Add(new SqlParameter("@SR_Explanation", oSCDDReport_UPDATE.SR_Explanation));
                    strSQL_UPDATE += ",SR_RiskItem = @SR_RiskItem";
                    sqlcmd.Parameters.Add(new SqlParameter("@SR_RiskItem", oSCDDReport_UPDATE.SR_RiskItem));
                    strSQL_UPDATE += ",SR_RiskLevel = @SR_RiskLevel";
                    sqlcmd.Parameters.Add(new SqlParameter("@SR_RiskLevel", oSCDDReport_UPDATE.SR_RiskLevel));
                    strSQL_UPDATE += ",SR_RiskNote = @SR_RiskNote";
                    sqlcmd.Parameters.Add(new SqlParameter("@SR_RiskNote", oSCDDReport_UPDATE.SR_RiskNote));
                    strSQL_UPDATE += ",SR_User = @SR_User";
                    sqlcmd.Parameters.Add(new SqlParameter("@SR_User", oSCDDReport_UPDATE.SR_User));
                    strSQL_UPDATE += ",SR_EDD_Status = @SR_EDD_Status";
                    sqlcmd.Parameters.Add(new SqlParameter("@SR_EDD_Status", oSCDDReport_UPDATE.SR_EDD_Status));
                    //20211026_Ares_Jack_EDD完成日期
                    strSQL_UPDATE += ",SR_EDD_Date = @SR_EDD_Date";
                    sqlcmd.Parameters.Add(new SqlParameter("@SR_EDD_Date", oSCDDReport_UPDATE.SR_EDD_Date));
                    strSQL_Where = " WHERE SR_CASE_NO = @SR_CASE_NO ";
                    sqlcmd.Parameters.Add(new SqlParameter("@SR_CASE_NO", strKey));
                }
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = strSQL_UPDATE + strSQL_Where;

                DataSet resultSet = SCDDReport.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                    ts.Complete();
                }
            }
        }
        catch (Exception exp)
        {
            SCDDReport.SaveLog(exp.Message);
            result = false;
        }
        return result;
    }

    public static bool UPDATE_Obj_V2(Entity_SCDDReport oSCDDReport_UPDATE, String strKey)
    {
        bool result = false;
        String strSQL_UPDATE = String.Empty;
        String strSQL_Where = String.Empty;
        SqlCommand sqlcmd = new SqlCommand();

        try
        {
            using (OMTransactionScope ts = new OMTransactionScope())
            {
                if (oSCDDReport_UPDATE != null && !String.IsNullOrEmpty(strKey))
                {
                    strSQL_UPDATE = @" UPDATE [dbo].[SCDDReport] SET SR_DateTime = GETDATE() ";

                    strSQL_UPDATE += ",SR_Explanation = @SR_Explanation";
                    sqlcmd.Parameters.Add(new SqlParameter("@SR_Explanation", oSCDDReport_UPDATE.SR_Explanation));
                    strSQL_UPDATE += ",SR_RiskItem = @SR_RiskItem";
                    sqlcmd.Parameters.Add(new SqlParameter("@SR_RiskItem", oSCDDReport_UPDATE.SR_RiskItem));
                    strSQL_UPDATE += ",SR_RiskLevel = @SR_RiskLevel";
                    sqlcmd.Parameters.Add(new SqlParameter("@SR_RiskLevel", oSCDDReport_UPDATE.SR_RiskLevel));
                    strSQL_UPDATE += ",SR_RiskNote = @SR_RiskNote";
                    sqlcmd.Parameters.Add(new SqlParameter("@SR_RiskNote", oSCDDReport_UPDATE.SR_RiskNote));
                    strSQL_UPDATE += ",SR_User = @SR_User";
                    sqlcmd.Parameters.Add(new SqlParameter("@SR_User", oSCDDReport_UPDATE.SR_User));
                    strSQL_UPDATE += ",SR_EDD_Status = @SR_EDD_Status";
                    sqlcmd.Parameters.Add(new SqlParameter("@SR_EDD_Status", oSCDDReport_UPDATE.SR_EDD_Status));

                    strSQL_Where = " WHERE SR_CASE_NO = @SR_CASE_NO ";
                    sqlcmd.Parameters.Add(new SqlParameter("@SR_CASE_NO", strKey));
                }
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = strSQL_UPDATE + strSQL_Where;

                result = SCDDReport.Delete(sqlcmd);

                if (result)
                    ts.Complete();
            }
        }
        catch (Exception exp)
        {
            SCDDReport.SaveLog(exp.Message);
            result = false;
        }
        return result;
    }

    /// <summary>
    /// 功能說明:取得SCDDReport資料
    /// 作    者:Leon Ho
    /// 創建時間:2019/02/12
    /// 修改記錄:
    /// </summary>
    /// <param name="oSCDDReport">查詢條件</param>
    /// <param name="oOrderColumn">排序欄位(可條件、可升降冪)，寫法如SQL，不須加Order by</param>
    /// <param name="tempDataTable">回傳查詢結果</param>
    /// <returns></returns>
    public static bool GetSCDDReportDataTable(Entity_SCDDReport oSCDDReport, string oOrderColumn, ref  DataTable tempDataTable)
    {
        if (oSCDDReport != null)
        {
            try
            {
                SqlCommand sqlcmd = new SqlCommand();
                string sql = @"SELECT *
                               FROM [dbo].[SCDDReport]
                               WHERE 1 = 1";

                if (!String.IsNullOrEmpty(oSCDDReport.SR_CASE_NO))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@SR_CASE_NO", oSCDDReport.SR_CASE_NO));
                    sql += " AND SR_CASE_NO = @SR_CASE_NO ";
                }

                if (!String.IsNullOrEmpty(oSCDDReport.SR_User))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@SR_User", oSCDDReport.SR_User));
                    sql += " AND SR_User = @SR_User ";
                }

                if (!String.IsNullOrEmpty(oSCDDReport.SR_RiskLevel))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@SR_RiskLevel", oSCDDReport.SR_RiskLevel));
                    sql += " AND SR_RiskLevel = @SR_RiskLevel ";
                }

                if (!String.IsNullOrEmpty(oSCDDReport.SR_RiskItem))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@SR_RiskItem", oSCDDReport.SR_RiskItem));
                    sql += " AND SR_RiskItem = @SR_RiskItem ";
                }

                if (!String.IsNullOrEmpty(oSCDDReport.SR_EDD_Status))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@SR_EDD_Status", oSCDDReport.SR_EDD_Status));
                    sql += " AND SR_EDD_Status = @SR_EDD_Status ";
                }

                string[] tempArry = oOrderColumn.Split(',');
                string[] arrayOrderColumn = { "SR_CASE_NO", "SR_User", "SR_RiskLevel", "SR_RiskItem", "SR_DateTime", "SR_EDD_Status" };
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
                DataSet ds = SCDDReport.SearchOnDataSet(sqlcmd);
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
                SCDDReport.SaveLog(exp.Message);
                return false;
            }
        }
        else
            return false;
    }

}
