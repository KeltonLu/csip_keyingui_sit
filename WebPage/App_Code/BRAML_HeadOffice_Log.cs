//******************************************************************
//*  作    者：林矩敬
//*  功能說明：收單特店電文更新成功後的 資料處理
//*  創建日期：2019/04/30
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************

using CSIPNewInvoice.EntityLayer_new;
using Framework.Common.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Framework.Common.Utility;

/// <summary>
/// BRAML_FinalDataLog 的摘要描述
/// </summary>
public class BRAML_HeadOffice_Log : CSIPCommonModel.BusinessRules.BRBase<EntityAML_HeadOffice_Log>
{
    private const string _tableName = "[AML_HeadOffice_Log]";
    public BRAML_HeadOffice_Log()
    {

    }

    public static List<EntityAML_HeadOffice_Log> Query(EntityAML_HeadOffice_Log paramObj,string SearchStart,string SearchEnd)
    {
        string sql = @"SELECT  [ID],[BasicTaxID],[keyin_day],[BasicRegistyNameEN],[BasicRegistyNameCH]
      ,[user_1key],[user_2key] FROM  " + _tableName;
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;

        #region 查詢條件
        List<string> _par = new List<string>();
        if (!string.IsNullOrEmpty(paramObj.BasicTaxID))
        {
            _par.Add("BasicTaxID = @BasicTaxID ");
            sqlcmd.Parameters.Add(new SqlParameter("@BasicTaxID", paramObj.BasicTaxID));
        }

        if (!string.IsNullOrEmpty(paramObj.ID))
        {
            _par.Add("[ID] = @ID ");
            sqlcmd.Parameters.Add(new SqlParameter("@ID", paramObj.ID ));
        }
        if (!string.IsNullOrEmpty(SearchStart) && !string.IsNullOrEmpty(SearchEnd))
        {
            _par.Add("[keyin_day] BETWEEN @SearchStart AND @SearchEnd");
            sqlcmd.Parameters.Add(new SqlParameter("@SearchStart", SearchStart));
            sqlcmd.Parameters.Add(new SqlParameter("@SearchEnd", SearchEnd));
        }
        if (_par.Count > 0)
        {
            sql = string.Format("{0} where {1}", sql, string.Join(" and ", _par.ToArray()));
        }
        #endregion

        sqlcmd.CommandText = sql;
        try
        {
            DataTable dt = new DataTable();
            DataSet DS = SearchOnDataSet(sqlcmd);
            if (DS != null && DS.Tables.Count > 0)
            {
                dt = DS.Tables[0];
            }
            List<EntityAML_HeadOffice_Log> rtn = DataTableConvertor.ConvertCollToObj<EntityAML_HeadOffice_Log>(dt);
            return rtn;
        }
        catch (Exception ex)
        {
            Logging.Log("查詢收單特店電文報表資料失敗：" + ex, LogLayer.BusinessRule);
            return null;
        }


    }

    public static bool InsertByTaxID(string BasicTaxID)
    {
        bool result = false;
        //2021/03/17_Ares_Stanley-DB名稱改為變數
        string sSQL = string.Format("Insert into {0} ", _tableName) +
string.Format(@"([keyin_day],[BasicTaxID],[BasicRegistyNameEN],[BasicRegistyNameCH],[user_1key],[user_2key],[CHANGED_TIME])

SELECT a.keyin_day, a.BasicTaxID, a.BasicRegistyNameEN, a.BasicRegistyNameCH, u1.[USER_NAME] user_1Key, u2.[USER_NAME] user_2Key
,getdate() 
FROM [dbo].[AML_HeadOffice] a WITH(NOLOCK)

LEFT JOIN 
(
	SELECT BasicTaxID,keyin_userid FROM [dbo].[AML_HeadOffice] WITH(NOLOCK)	
	WHERE BasicTaxID =@BasicTaxID  and KEYIN_FLAG = '2' 
) b ON a.BasicTaxID = b.BasicTaxID
INNER JOIN (SELECT distinct [USER_ID],[USER_NAME] FROM {0}.dbo.M_USER WITH(NOLOCK)) u1 ON a.keyin_userID = u1.USER_ID
INNER JOIN (SELECT distinct [USER_ID],[USER_NAME] FROM {0}.dbo.M_USER WITH(NOLOCK)) u2 ON b.keyin_userID = u2.USER_ID
where a.BasicTaxID =@BasicTaxID and KEYIN_FLAG = '1' ", UtilHelper.GetAppSettings("DB_CSIP"));

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sSQL;
        
        sqlcmd.Parameters.Add(new SqlParameter("@BasicTaxID", BasicTaxID));
        
        result = Update(sqlcmd);
        return result;

    }

    /// <summary>
    /// 新增單筆
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool Insert(EntityAML_HeadOffice_Log paramObj)
    {
        bool result = false;

        string sSQL = string.Format("Insert into {0}",_tableName) +
"(BasicTaxID,keyin_day,BasicRegistyNameEN,BasicRegistyNameCH,userID_1key,userID_2key,LineID,isDEL)" +
"VALUES(@BasicTaxID,@keyin_day,@BasicRegistyNameEN,@BasicRegistyNameCH,@userID_1key,@userID_2key,@LineID,@isDEL);";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sSQL;
        //sqlcmd.Parameters.Add(new SqlParameter("@ID", paramObj.ID));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicTaxID", paramObj.BasicTaxID));
        sqlcmd.Parameters.Add(new SqlParameter("@keyin_day", paramObj.keyin_day));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicRegistyNameEN", paramObj.BasicRegistyNameEN));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicRegistyNameCH", paramObj.BasicRegistyNameCH));
        sqlcmd.Parameters.Add(new SqlParameter("@userID_1key", paramObj.userID_1key));
        sqlcmd.Parameters.Add(new SqlParameter("@userID_2key", paramObj.userID_2key));

        result = Update(sqlcmd);
        return result;

    }

    /// <summary>
    /// 新增集合
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool Insert(List<EntityAML_HeadOffice_Log> paramObj)
    {
        bool result = false;
        bool isErr = false;
        //取得 BasicTaxID 
        string BasicTaxID = paramObj[0].BasicTaxID;
        
        clearByBasicTaxID(BasicTaxID);

        foreach (EntityAML_HeadOffice_Log oitem in paramObj)
        {
            //檢核必填
            if (string.IsNullOrEmpty(oitem.ID))
            { continue; }
            result = Insert(oitem);
            if (!result)
            {
                isErr = true;
                break;
            }
        }
        if (isErr)
        {
            clearByBasicTaxID(BasicTaxID);
            result = false;

        }
        return result;
    }

    /// <summary>
    /// 刪除原有的資料?
    /// </summary>
    /// <param name="BasicTaxID"></param>
    private static void clearByBasicTaxID(string BasicTaxID)
    {
        //INS之前，先刪除
        string sSQL = string.Format(" delete from  {0} where BasicTaxID = @BasicTaxID ", _tableName);
        SqlCommand sqlcmdD = new SqlCommand();
        sqlcmdD.CommandType = CommandType.Text;
        sqlcmdD.CommandText = sSQL;
        sqlcmdD.Parameters.Add(new SqlParameter("@BasicTaxID", BasicTaxID));
        Update(sqlcmdD);
    }
}