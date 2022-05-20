//******************************************************************
//*  作    者：林家賜
//*  功能說明：收單特店審查處理  高階經理人資料
//*  創建日期：2019/01/24
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
//20190806-RQ-2019-008595-002-長姓名需求，增開中文長姓名與羅馬拼音2個欄位 by Peggy

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Framework.Data.OM;
using CSIPKeyInGUI.EntityLayer;
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

/// <summary>
/// BRAML_SeniorManager 的摘要描述
/// </summary>
public class BRAML_SeniorManager : CSIPCommonModel.BusinessRules.BRBase<EntityAML_SeniorManager>
{
    public BRAML_SeniorManager()
    {

    }

    public static List<EntityAML_SeniorManager> Query(string BasicTaxID, string ID,string keyin_Flag)
    {
        string sql = @"
SELECT  [ID] ,[BasicTaxID] ,[Name] ,[Birth] ,[CountryCode] ,[IDNo] ,[IDNoType] ,[Identity1] ,[Identity2] ,[Identity3] ,[Identity4] ,[Identity5] ,[Identity6] ,[Expdt]
  ,[keyin_Flag] ,[keyin_day] ,[keyin_userID] ,[LineID] ,[isDEL] ,[Name_L] ,[Name_Pinyin] FROM [AML_SeniorManager]
 WHERE BasicTaxID = @BasicTaxID   ";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
       
        sqlcmd.Parameters.Add(new SqlParameter("@BasicTaxID", BasicTaxID));
         if (!string.IsNullOrEmpty(ID))
        {
            sql += " and [ID] = @ID ";
            sqlcmd.Parameters.Add(new SqlParameter("@ID", ID));  
        }
        if (!string.IsNullOrEmpty(keyin_Flag))
        {
            sql += " and [keyin_Flag] = @keyin_Flag ";
            sqlcmd.Parameters.Add(new SqlParameter("@keyin_Flag", keyin_Flag));  
        }


        sqlcmd.CommandText = sql;
        try
        {
            DataTable dt = new DataTable();
            DataSet DS = SearchOnDataSet(sqlcmd);
            if (DS != null && DS.Tables.Count > 0)
            {
                dt = DS.Tables[0];
            }
            List<EntityAML_SeniorManager> rtn = DataTableConvertor.ConvertCollToObj<EntityAML_SeniorManager>(dt);
            return rtn;
        }
        catch (Exception ex)
        {
            Logging.Log("查詢公司高級經理人資料失敗：" + ex, LogLayer.BusinessRule);
            return null;
        }


    }

    /// <summary>
    /// 新增單筆
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool Insert(EntityAML_SeniorManager paramObj)
    {
        bool result = false;


      string  sSQL = @"Insert into AML_SeniorManager
                        (BasicTaxID,Name,Birth,CountryCode,IDNo,IDNoType,[Identity1],[Identity2],[Identity3],[Identity4],[Identity5],[Identity6],Expdt,keyin_Flag,keyin_day,keyin_userID,LineID,isDEL,Name_L,Name_Pinyin )
                        VALUES(@BasicTaxID,@Name,@Birth,@CountryCode,@IDNo,@IDNoType,@Identity1,@Identity2,@Identity3,@Identity4,@Identity5,@Identity6,@Expdt,@keyin_Flag,@keyin_day,@keyin_userID,@LineID,@isDEL,@Name_L,@Name_Pinyin );
                        ";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sSQL;
        sqlcmd.Parameters.Add(new SqlParameter("@BasicTaxID", paramObj.BasicTaxID));
        sqlcmd.Parameters.Add(new SqlParameter("@Name", paramObj.Name));
        sqlcmd.Parameters.Add(new SqlParameter("@Birth", paramObj.Birth));
        sqlcmd.Parameters.Add(new SqlParameter("@CountryCode", paramObj.CountryCode));
        sqlcmd.Parameters.Add(new SqlParameter("@IDNo", paramObj.IDNo));
        sqlcmd.Parameters.Add(new SqlParameter("@IDNoType", paramObj.IDNoType));
        sqlcmd.Parameters.Add(new SqlParameter("@Identity1", paramObj.Identity1));
        sqlcmd.Parameters.Add(new SqlParameter("@Identity2", paramObj.Identity2));
        sqlcmd.Parameters.Add(new SqlParameter("@Identity3", paramObj.Identity3));
        sqlcmd.Parameters.Add(new SqlParameter("@Identity4", paramObj.Identity4));
        sqlcmd.Parameters.Add(new SqlParameter("@Identity5", paramObj.Identity5));
        sqlcmd.Parameters.Add(new SqlParameter("@Identity6", paramObj.Identity6));
        sqlcmd.Parameters.Add(new SqlParameter("@Expdt", paramObj.Expdt));
        sqlcmd.Parameters.Add(new SqlParameter("@keyin_Flag", paramObj.keyin_Flag));
        sqlcmd.Parameters.Add(new SqlParameter("@keyin_day", paramObj.keyin_day));
        sqlcmd.Parameters.Add(new SqlParameter("@keyin_userID", paramObj.keyin_userID));
        sqlcmd.Parameters.Add(new SqlParameter("@LineID", paramObj.LineID));
        sqlcmd.Parameters.Add(new SqlParameter("@isDEL", paramObj.isDEL));
        //20190806-RQ-2019-008595-002-長姓名需求，增開中文長姓名與羅馬拼音2個欄位 by Peggy
        sqlcmd.Parameters.Add(new SqlParameter("@Name_L", paramObj.Name_L));
        sqlcmd.Parameters.Add(new SqlParameter("@Name_Pinyin", paramObj.Name_Pinyin));

        result = Update(sqlcmd);
        return result;

    }

    /// <summary>
    /// 新增集合
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool Insert(List<EntityAML_SeniorManager> paramObj)
    {
        bool result = false;
        bool isErr = false;
        //取得 BasicTaxID 
        string BasicTaxID = paramObj[0].BasicTaxID;
        ///新增前先全刪
        clearByBasicTaxID(BasicTaxID);
        foreach (EntityAML_SeniorManager oitem in paramObj)
        { 
            //檢核必填
            if (string.IsNullOrEmpty(oitem.IDNo))
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
    private static void clearByBasicTaxID(string BasicTaxID)
    {
        //INS之前，先刪除
        string sSQL = @" delete from  AML_SeniorManager where BasicTaxID = @BasicTaxID ";
        SqlCommand sqlcmdD = new SqlCommand();
        sqlcmdD.CommandType = CommandType.Text;
        sqlcmdD.CommandText = sSQL;
        sqlcmdD.Parameters.Add(new SqlParameter("@BasicTaxID", BasicTaxID));
        Update(sqlcmdD);
    }
}