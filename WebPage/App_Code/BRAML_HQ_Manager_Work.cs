//******************************************************************
//*  作    者：林家賜
//*  功能說明：收單特店審查處理  總公司資料
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
/// <summary>
/// BRAML_Cdata_Work 的摘要描述
/// </summary>
public class BRAML_HQ_Manager_Work : CSIPCommonModel.BusinessRules.BRBase<EntityAML_HQ_Manager_Work>
{
    public BRAML_HQ_Manager_Work()
    {
    }

    public static List<EntityAML_HQ_Manager_Work> getHQMA_WorkColl(AML_SessionState parmObj)
    {
        string sSQL = @" SELECT  
ID,CASE_NO,FileName,HCOP_BATCH_NO,HCOP_INTER_ID,HCOP_SIXM_TOT_AMT,HCOP_KEY,HCOP_BENE_NATION,HCOP_BENE_NAME,HCOP_BENE_BIRTH_DATE,
HCOP_BENE_ID,HCOP_BENE_PASSPORT,HCOP_BENE_PASSPORT_EXP,HCOP_BENE_RESIDENT_NO,HCOP_BENE_RESIDENT_EXP,HCOP_BENE_OTH_CERT,HCOP_BENE_OTH_CERT_EXP,
HCOP_BENE_JOB_TYPE,HCOP_BENE_JOB_TYPE_2,HCOP_BENE_JOB_TYPE_3,HCOP_BENE_JOB_TYPE_4,HCOP_BENE_JOB_TYPE_5,HCOP_BENE_JOB_TYPE_6,HCOP_BENE_RESERVED,
Create_Date,Create_Time,Create_User,HCOP_BENE_LNAME,HCOP_BENE_ROMA
, HCOP_BATCH_NO + ';' + HCOP_INTER_ID + ';' + Convert(varchar,ID) as ArgNo
  FROM  [AML_HQ_Manager_Work]
  where  1 = 1  ";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        if (parmObj != null)
        {
            if (!String.IsNullOrEmpty(parmObj.RMMBatchNo))
            {
                sSQL += " AND HCOP_BATCH_NO = @HCOP_BATCH_NO ";
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BATCH_NO", parmObj.RMMBatchNo));
            }
            if (!String.IsNullOrEmpty(parmObj.RMMBatchNo))
            {
                sSQL += " AND HCOP_INTER_ID = @HCOP_INTER_ID ";
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_INTER_ID", parmObj.AMLInternalID));
            }
            if (!String.IsNullOrEmpty(parmObj.CASE_NO))
            {
                sSQL += " AND CASE_NO = @CASE_NO ";
                sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", parmObj.CASE_NO));
            }
        }

        sqlcmd.CommandText = sSQL;

        List<EntityAML_HQ_Manager_Work> rtnObj = new List<EntityAML_HQ_Manager_Work>();
        DataTable dt = new DataTable();
        DataSet DS = SearchOnDataSet(sqlcmd);
        if (DS != null && DS.Tables.Count > 0)
        {
            dt = DS.Tables[0];
        }
        if (dt.Rows.Count > 0)
        {
            //2021/03/10_Ares_Stanley-移除空白字元
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["HCOP_BENE_NAME"] = dt.Rows[i]["HCOP_BENE_NAME"].ToString().Trim();
            }
            rtnObj = DataTableConvertor.ConvertCollToObj<EntityAML_HQ_Manager_Work>(dt);
        }

        //EntityAML_HQ_Manager_Work _tmpItem = new EntityAML_HQ_Manager_Work();
        //for (int i = 0; i < 6; i++)
        //{
        //    _tmpItem = new EntityAML_HQ_Manager_Work();
        //    _tmpItem.HCOP_BENE_LNAME = string.Format("測試者的長姓名_{0}", i);
        //    _tmpItem.HCOP_BENE_ROMA = string.Format("ROMAROMAROMAROMA_{0}", i);
        //    _tmpItem.HCOP_BENE_BIRTH_DATE = string.Format("2019/{0}/7", i);
        //    _tmpItem.HCOP_BENE_ID = string.Format("A{0}", i);
        //    _tmpItem.HCOP_BENE_NATION = (i % 2 == 0) ? "TW" : "CN";

        //    rtnObj.Add(_tmpItem);
        //}

        return rtnObj;

    }

    /// <summary>
    /// 新增多筆
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public bool Insert(List<EntityAML_HQ_Manager_Work> paramObj)
    {
        bool result = false;
        ///使用交易
        using (OMTransactionScope ts = new OMTransactionScope())
        {
            foreach (EntityAML_HQ_Manager_Work scOBJ in paramObj)
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
    public bool Insert(EntityAML_HQ_Manager_Work paramObj)
    {
        bool result = false;


        try
        {

            string sSql = @"Insert into AML_HQ_Manager_Work
(CASE_NO,FileName,HCOP_BATCH_NO,HCOP_INTER_ID,HCOP_SIXM_TOT_AMT,HCOP_KEY,HCOP_BENE_NATION,HCOP_BENE_NAME,HCOP_BENE_BIRTH_DATE,HCOP_BENE_ID,HCOP_BENE_PASSPORT,HCOP_BENE_PASSPORT_EXP,HCOP_BENE_RESIDENT_NO,HCOP_BENE_RESIDENT_EXP,HCOP_BENE_OTH_CERT,HCOP_BENE_OTH_CERT_EXP,HCOP_BENE_JOB_TYPE,HCOP_BENE_JOB_TYPE_2,HCOP_BENE_JOB_TYPE_3,HCOP_BENE_JOB_TYPE_4,HCOP_BENE_JOB_TYPE_5,HCOP_BENE_JOB_TYPE_6,HCOP_BENE_RESERVED,Create_Date,Create_Time,Create_User)
VALUES(@CASE_NO,@FileName,@HCOP_BATCH_NO,@HCOP_INTER_ID,@HCOP_SIXM_TOT_AMT,@HCOP_KEY,@HCOP_BENE_NATION,@HCOP_BENE_NAME,@HCOP_BENE_BIRTH_DATE,@HCOP_BENE_ID,@HCOP_BENE_PASSPORT,@HCOP_BENE_PASSPORT_EXP,@HCOP_BENE_RESIDENT_NO,@HCOP_BENE_RESIDENT_EXP,@HCOP_BENE_OTH_CERT,@HCOP_BENE_OTH_CERT_EXP,@HCOP_BENE_JOB_TYPE,@HCOP_BENE_JOB_TYPE_2,@HCOP_BENE_JOB_TYPE_3,@HCOP_BENE_JOB_TYPE_4,@HCOP_BENE_JOB_TYPE_5,@HCOP_BENE_JOB_TYPE_6,@HCOP_BENE_RESERVED,@Create_Date,@Create_Time,@Create_User);
";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sSql;
            //  sqlcmd.Parameters.Add(new SqlParameter("@ID", paramObj.ID));
            sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", paramObj.CASE_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@FileName", paramObj.FileName));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BATCH_NO", paramObj.HCOP_BATCH_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_INTER_ID", paramObj.HCOP_INTER_ID));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_SIXM_TOT_AMT", paramObj.HCOP_SIXM_TOT_AMT));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_KEY", paramObj.HCOP_KEY));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_NATION", paramObj.HCOP_BENE_NATION));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_NAME", paramObj.HCOP_BENE_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_BIRTH_DATE", paramObj.HCOP_BENE_BIRTH_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_ID", paramObj.HCOP_BENE_ID));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_PASSPORT", paramObj.HCOP_BENE_PASSPORT));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_PASSPORT_EXP", paramObj.HCOP_BENE_PASSPORT_EXP));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_RESIDENT_NO", paramObj.HCOP_BENE_RESIDENT_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_RESIDENT_EXP", paramObj.HCOP_BENE_RESIDENT_EXP));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_OTH_CERT", paramObj.HCOP_BENE_OTH_CERT));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_OTH_CERT_EXP", paramObj.HCOP_BENE_OTH_CERT_EXP));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE", paramObj.HCOP_BENE_JOB_TYPE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE_2", paramObj.HCOP_BENE_JOB_TYPE_2));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE_3", paramObj.HCOP_BENE_JOB_TYPE_3));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE_4", paramObj.HCOP_BENE_JOB_TYPE_4));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE_5", paramObj.HCOP_BENE_JOB_TYPE_5));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE_6", paramObj.HCOP_BENE_JOB_TYPE_6));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_RESERVED", paramObj.HCOP_BENE_RESERVED));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_Date", paramObj.Create_Date));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_Time", paramObj.Create_Time));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_User", paramObj.Create_User));

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

    /// <summary>
    /// 讀取案件明細
    /// </summary>
    /// <returns></returns>
    public static EntityAML_HQ_Manager_Work getAML_HQ_Manager_Work(AML_SessionState parmObj)
    {
        string sSQL = @" SELECT ID,CASE_NO,FileName,HCOP_BATCH_NO,HCOP_INTER_ID,HCOP_SIXM_TOT_AMT,HCOP_KEY,HCOP_BENE_NATION,HCOP_BENE_NAME,HCOP_BENE_BIRTH_DATE,HCOP_BENE_ID,HCOP_BENE_PASSPORT
                                        ,HCOP_BENE_PASSPORT_EXP,HCOP_BENE_RESIDENT_NO,HCOP_BENE_RESIDENT_EXP,HCOP_BENE_OTH_CERT,HCOP_BENE_OTH_CERT_EXP,HCOP_BENE_JOB_TYPE,HCOP_BENE_JOB_TYPE_2,HCOP_BENE_JOB_TYPE_3
                                        ,HCOP_BENE_JOB_TYPE_4,HCOP_BENE_JOB_TYPE_5,HCOP_BENE_JOB_TYPE_6,HCOP_BENE_RESERVED,Create_Time,Create_User,Create_Date,HCOP_BENE_LNAME,HCOP_BENE_ROMA  
                                          FROM dbo.AML_HQ_Manager_Work
                                          WHERE 1=1 ";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        if (parmObj != null)
        {
            if (!String.IsNullOrEmpty(parmObj.RMMBatchNo))
            {
                sSQL += " AND HCOP_BATCH_NO = @HCOP_BATCH_NO ";
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BATCH_NO", parmObj.RMMBatchNo));
            }
            if (!String.IsNullOrEmpty(parmObj.RMMBatchNo))
            {
                sSQL += " AND HCOP_INTER_ID = @HCOP_INTER_ID ";
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_INTER_ID", parmObj.AMLInternalID));
            }
            if (!String.IsNullOrEmpty(parmObj.CASE_NO))
            {
                sSQL += " AND CASE_NO = @CASE_NO ";
                sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", parmObj.CASE_NO));
            }
        }

        sqlcmd.CommandText = sSQL;

        EntityAML_HQ_Manager_Work rtnObj = new EntityAML_HQ_Manager_Work();
        DataTable dt = new DataTable();
        DataSet DS = SearchOnDataSet(sqlcmd);
        if (DS != null && DS.Tables.Count > 0)
        {
            dt = DS.Tables[0];
        }
        if (dt.Rows.Count > 0)
        {
            DataTableConvertor.convSingRow<EntityAML_HQ_Manager_Work>(ref rtnObj, dt.Rows[0]);
        }
        return rtnObj;
    }

    /// <summary>
    /// 新增單筆
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool InsertBySingle(EntityAML_HQ_Manager_Work paramObj)
    {
        bool result = false;

        try
        {

            string sSql = @"Insert into AML_HQ_Manager_Work (CASE_NO,FileName,HCOP_BATCH_NO,HCOP_INTER_ID,HCOP_SIXM_TOT_AMT,HCOP_KEY,HCOP_BENE_NATION,HCOP_BENE_NAME,HCOP_BENE_BIRTH_DATE,HCOP_BENE_ID,HCOP_BENE_PASSPORT,HCOP_BENE_PASSPORT_EXP,HCOP_BENE_RESIDENT_NO,HCOP_BENE_RESIDENT_EXP,HCOP_BENE_OTH_CERT,HCOP_BENE_OTH_CERT_EXP,HCOP_BENE_JOB_TYPE,HCOP_BENE_JOB_TYPE_2,HCOP_BENE_JOB_TYPE_3,HCOP_BENE_JOB_TYPE_4,HCOP_BENE_JOB_TYPE_5,HCOP_BENE_JOB_TYPE_6,HCOP_BENE_RESERVED,Create_Time,Create_User,Create_Date,HCOP_BENE_LNAME,HCOP_BENE_ROMA  )
                                            VALUES(@CASE_NO,@FileName,@HCOP_BATCH_NO,@HCOP_INTER_ID,@HCOP_SIXM_TOT_AMT,@HCOP_KEY,@HCOP_BENE_NATION,@HCOP_BENE_NAME,@HCOP_BENE_BIRTH_DATE,@HCOP_BENE_ID,@HCOP_BENE_PASSPORT,@HCOP_BENE_PASSPORT_EXP,@HCOP_BENE_RESIDENT_NO,@HCOP_BENE_RESIDENT_EXP,@HCOP_BENE_OTH_CERT,@HCOP_BENE_OTH_CERT_EXP,@HCOP_BENE_JOB_TYPE,@HCOP_BENE_JOB_TYPE_2,@HCOP_BENE_JOB_TYPE_3,@HCOP_BENE_JOB_TYPE_4,@HCOP_BENE_JOB_TYPE_5,@HCOP_BENE_JOB_TYPE_6,@HCOP_BENE_RESERVED,@Create_Time,@Create_User,@Create_Date,@HCOP_BENE_LNAME,@HCOP_BENE_ROMA);
                                        ";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sSql;

            sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", paramObj.CASE_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@FileName", paramObj.FileName));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BATCH_NO", paramObj.HCOP_BATCH_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_INTER_ID", paramObj.HCOP_INTER_ID));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_SIXM_TOT_AMT", paramObj.HCOP_SIXM_TOT_AMT));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_KEY", paramObj.HCOP_KEY));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_NATION", paramObj.HCOP_BENE_NATION));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_NAME", paramObj.HCOP_BENE_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_BIRTH_DATE", paramObj.HCOP_BENE_BIRTH_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_ID", paramObj.HCOP_BENE_ID));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_PASSPORT", paramObj.HCOP_BENE_PASSPORT));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_PASSPORT_EXP", paramObj.HCOP_BENE_PASSPORT_EXP));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_RESIDENT_NO", paramObj.HCOP_BENE_RESIDENT_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_RESIDENT_EXP", paramObj.HCOP_BENE_RESIDENT_EXP));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_OTH_CERT", paramObj.HCOP_BENE_OTH_CERT));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_OTH_CERT_EXP", paramObj.HCOP_BENE_OTH_CERT_EXP));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE", paramObj.HCOP_BENE_JOB_TYPE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE_2", paramObj.HCOP_BENE_JOB_TYPE_2));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE_3", paramObj.HCOP_BENE_JOB_TYPE_3));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE_4", paramObj.HCOP_BENE_JOB_TYPE_4));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE_5", paramObj.HCOP_BENE_JOB_TYPE_5));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_JOB_TYPE_6", paramObj.HCOP_BENE_JOB_TYPE_6));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_RESERVED", paramObj.HCOP_BENE_RESERVED));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_Time", paramObj.Create_Time));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_User", paramObj.Create_User));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_Date", paramObj.Create_Date));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_LNAME", paramObj.HCOP_BENE_LNAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_ROMA", paramObj.HCOP_BENE_ROMA));

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

    public static List<EntityAML_HQ_Manager_Work> getHQWorkColl(AML_SessionState parmObj)
    {
        string sSQL = @" SELECT ID,CASE_NO,[FileName],HCOP_BATCH_NO,HCOP_INTER_ID,HCOP_SIXM_TOT_AMT,HCOP_KEY,HCOP_BENE_NATION,HCOP_BENE_NAME,HCOP_BENE_BIRTH_DATE,HCOP_BENE_ID,HCOP_BENE_PASSPORT,HCOP_BENE_PASSPORT_EXP
                                        ,HCOP_BENE_RESIDENT_NO,HCOP_BENE_RESIDENT_EXP,HCOP_BENE_OTH_CERT,HCOP_BENE_OTH_CERT_EXP,HCOP_BENE_JOB_TYPE,HCOP_BENE_JOB_TYPE_2,HCOP_BENE_JOB_TYPE_3,HCOP_BENE_JOB_TYPE_4,HCOP_BENE_JOB_TYPE_5,HCOP_BENE_JOB_TYPE_6
                                        ,HCOP_BENE_RESERVED,Create_Time,Create_User,Create_Date,HCOP_BENE_LNAME,HCOP_BENE_ROMA  
                                          FROM dbo.AML_HQ_Manager_Work
                                          WHERE 1=1 ";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        if (parmObj != null)
        {
            if (!String.IsNullOrEmpty(parmObj.RMMBatchNo))
            {
                sSQL += " AND HCOP_BATCH_NO = @HCOP_BATCH_NO ";
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BATCH_NO", parmObj.RMMBatchNo));
            }
            if (!String.IsNullOrEmpty(parmObj.RMMBatchNo))
            {
                sSQL += " AND HCOP_INTER_ID = @HCOP_INTER_ID ";
                sqlcmd.Parameters.Add(new SqlParameter("@HCOP_INTER_ID", parmObj.AMLInternalID));
            }
            if (!String.IsNullOrEmpty(parmObj.CASE_NO))
            {
                sSQL += " AND CASE_NO = @CASE_NO ";
                sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", parmObj.CASE_NO));
            }
        }

        sqlcmd.CommandText = sSQL;

        List<EntityAML_HQ_Manager_Work> rtnObj = new List<EntityAML_HQ_Manager_Work>();
        DataTable dt = new DataTable();
        DataSet DS = SearchOnDataSet(sqlcmd);
        if (DS != null && DS.Tables.Count > 0)
        {
            dt = DS.Tables[0];
        }
        if (dt.Rows.Count > 0)
        {
            rtnObj = DataTableConvertor.ConvertCollToObj<EntityAML_HQ_Manager_Work>(dt);
        }

        return rtnObj;
    }
}