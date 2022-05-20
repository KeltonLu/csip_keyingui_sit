//******************************************************************
//*  作    者：林家賜
//*  功能說明：收單特店審查處理 BRNoteLog資料
//*  創建日期：2019/01/24
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Framework.Data.OM;
using CSIPNewInvoice.EntityLayer_new;
using CSIPKeyInGUI.BusinessRules;
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.BusinessRules;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using System.IO;
using System.Configuration;
using Framework.Common;
using Framework.Common.Logging;
using System.Reflection;
using Framework.Common.Utility;
/// <summary>
/// BRAML_Cdata_Work 的摘要描述
/// </summary>
public class BRNoteLog : CSIPCommonModel.BusinessRules.BRBase<EntityNoteLog>
{
    public BRNoteLog()
    {
    }
    /// <summary>
    /// 讀取 
    /// </summary>
    /// <returns></returns>
    public static List<EntityNoteLog> getNoteLogColl(AML_SessionState parmObj)
    {
        string sSQL = @"  SELECT NL_CASE_NO,NL_SecondKey,NL_DateTime,NL_User,NL_Type,NL_Value,NL_ShowFlag
  FROM  [NoteLog]
  where  NL_CASE_NO =@NL_CASE_NO
 order  by NL_DateTime desc
   ;";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sSQL;
        sqlcmd.Parameters.Add(new SqlParameter("@NL_CASE_NO", parmObj.CASE_NO));

        List<EntityNoteLog> rtnObj = new List<EntityNoteLog>();
        DataTable dt = new DataTable();
        DataSet DS = SearchOnDataSet(sqlcmd);
        if (DS != null && DS.Tables.Count > 0)
        {
            dt = DS.Tables[0];
        }
        if (dt.Rows.Count > 0)
        {
            rtnObj = DataTableConvertor.ConvertCollToObj<EntityNoteLog>(dt);
        }
        return rtnObj;
    }
    /// <summary>
    /// 讀取 
    /// </summary>
    /// <returns></returns>
    public static DataTable getNoteLog(AML_SessionState parmObj,string isShow)
    {
        //20200720-RQ-2020-021027-001，顯示寄送歷程
        //只顯示NL_ShowFlag =1
        //       string sSQL = @" 
        // SELECT distinct A.NL_CASE_NO,A.NL_SecondKey,A.NL_DateTime,b.[USER_NAME] + '(' +  A.NL_User + ')' as NL_User,A.NL_Type,A.NL_Value,A.NL_ShowFlag
        // FROM  [NoteLog] A join [CSIP].[dbo].[M_USER] B on A.NL_User = b.[USER_ID]
        // where  A.NL_CASE_NO =@NL_CASE_NO and A.NL_ShowFlag =@isShow
        //order  by A.NL_DateTime desc
        //  ;";
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        string sSQL = string.Format(@" 
              SELECT distinct A.NL_CASE_NO,A.NL_SecondKey,A.NL_DateTime,b.[USER_NAME] + '(' +  A.NL_User + ')' as NL_User,A.NL_Type,A.NL_Value,A.NL_ShowFlag
              FROM  [NoteLog] A join [{0}].[dbo].[M_USER] B on A.NL_User = b.[USER_ID]
              where  A.NL_CASE_NO =@NL_CASE_NO and A.NL_ShowFlag =@isShow
              UNION 
              SELECT distinct A.NL_CASE_NO,A.NL_SecondKey,A.NL_DateTime,''  as NL_User,A.NL_Type,A.NL_Value,A.NL_ShowFlag
              FROM  [NoteLog] A
              where  a.NL_TYPE IN ('傳送地址條','MERCHANTAML','MERCHANTAMLRESULT','MERCHANTRMM','MERCHANTRMMRESULT') AND A.NL_CASE_NO =@NL_CASE_NO and A.NL_ShowFlag =@isShow
             order  by A.NL_DateTime desc
   ;", UtilHelper.GetAppSettings("DB_CSIP"));

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sSQL;
        sqlcmd.Parameters.Add(new SqlParameter("@NL_CASE_NO", parmObj.CASE_NO));
        sqlcmd.Parameters.Add(new SqlParameter("@isShow", isShow));
        DataTable dt = new DataTable();
        DataSet DS = SearchOnDataSet(sqlcmd);
        if (DS != null && DS.Tables.Count > 0)
        {
            dt = DS.Tables[0];
        }
        return dt;
    }
    /// <summary>
    /// 新增多筆
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool Insert(List<EntityNoteLog> paramObj)
    {
        bool result = false;
        ///使用交易
        using (OMTransactionScope ts = new OMTransactionScope())
        {
            foreach (EntityNoteLog scOBJ in paramObj)
            {
                result = Insert(scOBJ);
                if (!result)
                {
                    return false;
                }
            }
            result = true;
            ts.Complete();
        }
        return result;
    }
    /// <summary>
    /// 新增單筆
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool Insert(EntityNoteLog paramObj)
    {
        bool result = false;

        try
        {

            string sSql = @"Insert into NoteLog
                        (NL_CASE_NO, NL_SecondKey, NL_DateTime, NL_User, NL_Type, NL_Value, NL_ShowFlag)
                        VALUES(@NL_CASE_NO, @NL_SecondKey, @NL_DateTime, @NL_User, @NL_Type, @NL_Value, @NL_ShowFlag);";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text; 
            sqlcmd.CommandText = sSql;

            sqlcmd.Parameters.Add(new SqlParameter("@NL_CASE_NO", paramObj.NL_CASE_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@NL_SecondKey", paramObj.NL_SecondKey));
            sqlcmd.Parameters.Add(new SqlParameter("@NL_DateTime", paramObj.NL_DateTime));
            sqlcmd.Parameters.Add(new SqlParameter("@NL_User", paramObj.NL_User));
            sqlcmd.Parameters.Add(new SqlParameter("@NL_Type", paramObj.NL_Type));
            sqlcmd.Parameters.Add(new SqlParameter("@NL_Value", paramObj.NL_Value));
            sqlcmd.Parameters.Add(new SqlParameter("@NL_ShowFlag", paramObj.NL_ShowFlag));

            result = Add(sqlcmd);
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
        }
        finally
        {

        }

        return result;
    }

}