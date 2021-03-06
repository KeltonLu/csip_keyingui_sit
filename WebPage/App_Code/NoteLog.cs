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

public class NoteLog : CSIPCommonModel.BusinessRules.BRBase<Entity_NoteLog>
{

    /// <summary>
    /// 功能說明:新增一筆Job資料
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/26
    /// 修改記錄:
    /// </summary>
    /// <param name="FileInfo"></param>
    /// <returns></returns>
    public static bool insert(Entity_NoteLog oNoteLog)
    {
        if (oNoteLog != null)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (NoteLog.AddNewEntity(oNoteLog))
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
                NoteLog.SaveLog(exp.Message);
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
    /// <param name="FileInfo"></param>
    /// <param name="strCondition"></param>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public static bool delete(Entity_NoteLog oNoteLog, string strCondition, ref string strMsgID)
    {
        if (oNoteLog != null && !String.IsNullOrEmpty(strCondition))
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (NoteLog.DeleteEntityByCondition(oNoteLog, strCondition))
                    {
                        ts.Complete();
                        //待完成
                        //strMsgID = "06_06040100_005";
                        return true;
                    }
                    else
                    {
                        //待完成
                        //strMsgID = "06_06040100_006";
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                NoteLog.SaveLog(exp.Message);
                //待完成
                //strMsgID = "06_06040100_006";
                return false;
            }
        }
        else
            return false;
    }

    /// <summary>
    /// 功能說明:更新一筆Job資料
    /// 作    者:Simba Liu
    /// 創建時間:2010/04/26
    /// 修改記錄:
    /// </summary>
    /// <param name="FileInfo"></param>
    /// <param name="strCondition"></param>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public static bool update(Entity_NoteLog oNoteLog, string strCondition, ref string strMsgID, params  string[] FiledSpit)
    {
        if (oNoteLog != null && !String.IsNullOrEmpty(strCondition) && FiledSpit != null)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (NoteLog.UpdateEntityByCondition(oNoteLog, strCondition, FiledSpit))
                    {
                        ts.Complete();
                        //待完成
                        //strMsgID = "06_06040100_003";
                        return true;
                    }
                    else
                    {
                        //待完成
                        //strMsgID = "06_06040100_004";
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                NoteLog.SaveLog(exp.Message);
                //待完成
                //strMsgID = "06_06040100_004";
                return false;
            }
        }
        else
            return false;
    }

    /// <summary>
    /// 功能說明:取得NoteLog資料
    /// 作    者:Leon Ho
    /// 創建時間:2019/02/12
    /// 修改記錄:
    /// </summary>
    /// <param name="oNoteLog">查詢條件</param>
    /// <param name="oOrderColumn">排序欄位(可條件、可升降冪)，寫法如SQL，不須加Order by</param>
    /// <param name="tempDataTable">回傳查詢結果</param>
    /// <returns></returns>
    public static bool GetNoteLogDataTable(Entity_NoteLog oNoteLog, string oOrderColumn, ref  DataTable tempDataTable)
    {
        try
        {
            if (oNoteLog != null)
            {
                SqlCommand sqlcmd = new SqlCommand();
                string sql = @"SELECT *
                               FROM [dbo].[NoteLog]
                               WHERE 1 = 1";

                if (!String.IsNullOrEmpty(oNoteLog.NL_CASE_NO))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@NL_CASE_NO", oNoteLog.NL_CASE_NO));
                    sql += " AND NL_CASE_NO = @NL_CASE_NO ";
                }

                if (!String.IsNullOrEmpty(oNoteLog.NL_SecondKey))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@NL_SecondKey", oNoteLog.NL_SecondKey));
                    sql += " AND NL_SecondKey = @NL_SecondKey ";
                }

                if (!String.IsNullOrEmpty(oNoteLog.NL_Type))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@NL_Type", oNoteLog.NL_Type));
                    sql += " AND NL_Type = @NL_Type ";
                }

                if (!String.IsNullOrEmpty(oNoteLog.NL_ShowFlag))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@NL_ShowFlag", oNoteLog.NL_ShowFlag));
                    sql += " AND NL_ShowFlag = @NL_ShowFlag ";
                }

                if (!String.IsNullOrEmpty(oNoteLog.NL_User))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@NL_User", oNoteLog.NL_User));
                    sql += " AND NL_User = @NL_User ";
                }

                string strOrder = string.Empty;
                string[] arrayOrderColumn = { "NL_CASE_NO", "NL_SecondKey", "NL_Type", "NL_DateTime" };

                if (!String.IsNullOrEmpty(oOrderColumn))
                {
                    string[] tempArry = oOrderColumn.Split(',');

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
                }

                if (strOrder != string.Empty)
                {
                    sql += "ORDER BY " + strOrder;
                }

                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = NoteLog.SearchOnDataSet(sqlcmd);
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
            else
                return false;
        }
        catch (Exception exp)
        {
            NoteLog.SaveLog(exp.Message);
            return false;
        }
    }
}
