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
/// BRAML_BRCH_Work 的摘要描述
/// </summary>
public class BRAML_BRCH_Work : CSIPCommonModel.BusinessRules.BRBase<EntityAML_BRCH_Work>
{
    public BRAML_BRCH_Work()
    {

    }
    /// <summary>
    /// 讀取案件明細
    /// </summary>
    /// <returns></returns>
    public static List<EntityAML_BRCH_Work> getBRCH_WOrkColl(AML_SessionState paramObj)
    {
        //20200722-8月RC變更-增加分公司登記名稱顯示
        /*
        string sSQL = @" Select
 ID,CASE_NO,FileName,BRCH_BATCH_NO,BRCH_INTER_ID,BRCH_SIXM_TOT_AMT,BRCH_MON_AMT1,BRCH_MON_AMT2,
BRCH_MON_AMT3,BRCH_MON_AMT4,BRCH_MON_AMT5,BRCH_MON_AMT6,BRCH_MON_AMT7,BRCH_MON_AMT8,BRCH_MON_AMT9,
BRCH_MON_AMT10,BRCH_MON_AMT11,BRCH_MON_AMT12,BRCH_KEY,BRCH_BRCH_NO,BRCH_BRCH_SEQ,BRCH_BRCH_TYPE,
BRCH_NATION,BRCH_BIRTH_DATE,BRCH_PERM_CITY,BRCH_PERM_ADDR1,BRCH_PERM_ADDR2,BRCH_CHINESE_NAME,
BRCH_ENGLISH_NAME,BRCH_ID,BRCH_OWNER_ID_ISSUE_DATE,BRCH_OWNER_ID_ISSUE_PLACE,BRCH_OWNER_ID_REPLACE_TYPE,
BRCH_ID_PHOTO_FLAG,BRCH_PASSPORT,BRCH_PASSPORT_EXP_DATE,BRCH_RESIDENT_NO,BRCH_RESIDENT_EXP_DATE,BRCH_OTHER_CERT,
BRCH_OTHER_CERT_EXP_DATE,BRCH_COMP_TEL,BRCH_CREATE_DATE,BRCH_STATUS,BRCH_CIRCULATE_MERCH,BRCH_HQ_BRCH_NO,
BRCH_HQ_BRCH_SEQ_NO,BRCH_UPDATE_DATE,BRCH_QUALIFY_FLAG,BRCH_RESERVED_FILLER,Create_Time,Create_User,Create_Date,
BRCH_ID_Type,BRCH_ID_SreachStatus,BRCH_ExportFileFlag,BRCH_LastExportTime,BRCH_UPDATE_ID,BRCH_REAL_CORP
,BRCH_CHINESE_LNAME,BRCH_ROMA
, BRCH_BATCH_NO + ';' + BRCH_INTER_ID + ';' + Convert(varchar,ID) as ArgNo
  FROM  [AML_BRCH_Work]
   where 1 = 1 ";
        */
        //20211014_Ares_Jack_JOIN AML_BRCH_STATUS拿掉, 直接用新欄位 BRCH_REG_CHI_NAME
        string sSQL = @" Select 
                                        ID,CASE_NO,FileName,BRCH_BATCH_NO,BRCH_INTER_ID,BRCH_SIXM_TOT_AMT,BRCH_MON_AMT1,BRCH_MON_AMT2,
                                        BRCH_MON_AMT3,BRCH_MON_AMT4,BRCH_MON_AMT5,BRCH_MON_AMT6,BRCH_MON_AMT7,BRCH_MON_AMT8,BRCH_MON_AMT9,
                                        BRCH_MON_AMT10,BRCH_MON_AMT11,BRCH_MON_AMT12,BRCH_KEY,BRCH_BRCH_NO,BRCH_BRCH_SEQ,BRCH_BRCH_TYPE,
                                        BRCH_NATION,BRCH_BIRTH_DATE,BRCH_PERM_CITY,BRCH_PERM_ADDR1,BRCH_PERM_ADDR2,BRCH_CHINESE_NAME,
                                        BRCH_ENGLISH_NAME,BRCH_ID,BRCH_OWNER_ID_ISSUE_DATE,BRCH_OWNER_ID_ISSUE_PLACE,BRCH_OWNER_ID_REPLACE_TYPE,
                                        BRCH_ID_PHOTO_FLAG,BRCH_PASSPORT,BRCH_PASSPORT_EXP_DATE,BRCH_RESIDENT_NO,BRCH_RESIDENT_EXP_DATE,BRCH_OTHER_CERT,
                                        BRCH_OTHER_CERT_EXP_DATE,BRCH_COMP_TEL,BRCH_CREATE_DATE,BRCH_STATUS,BRCH_CIRCULATE_MERCH,BRCH_HQ_BRCH_NO,
                                        BRCH_HQ_BRCH_SEQ_NO,BRCH_UPDATE_DATE,BRCH_QUALIFY_FLAG,BRCH_RESERVED_FILLER,Create_Time,Create_User,a.Create_Date,
                                        BRCH_ID_Type,BRCH_ID_SreachStatus,BRCH_ExportFileFlag,BRCH_LastExportTime,BRCH_UPDATE_ID,BRCH_REAL_CORP
                                        ,BRCH_CHINESE_LNAME,BRCH_ROMA
                                        , BRCH_BATCH_NO + ';' + BRCH_INTER_ID + ';' + Convert(varchar,ID) as ArgNo
                                        ,BRCH_REG_CHI_NAME as REG_CHI_NAME
                                        FROM  [AML_BRCH_Work] a 
                                        WHERE 1 = 1 ";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sSQL;
        if (paramObj != null)
        {
            if (!String.IsNullOrEmpty(paramObj.RMMBatchNo))
            {
                sSQL += " AND BRCH_BATCH_NO = @BRCH_BATCH_NO ";
                sqlcmd.Parameters.Add(new SqlParameter("@BRCH_BATCH_NO", paramObj.RMMBatchNo));
            }
            if (!String.IsNullOrEmpty(paramObj.AMLInternalID))
            {
                sSQL += " AND BRCH_INTER_ID = @BRCH_INTER_ID ";
                sqlcmd.Parameters.Add(new SqlParameter("@BRCH_INTER_ID", paramObj.AMLInternalID));
            }
            if (!String.IsNullOrEmpty(paramObj.CASE_NO))
            {
                sSQL += " AND CASE_NO = @CASE_NO ";
                sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", paramObj.CASE_NO));
            }
        }
        sqlcmd.CommandText = sSQL;
        List<EntityAML_BRCH_Work> rtnObj = new List<EntityAML_BRCH_Work>();
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
                dt.Rows[i]["REG_CHI_NAME"] = dt.Rows[i]["REG_CHI_NAME"].ToString().Trim();
            }
            rtnObj = DataTableConvertor.ConvertCollToObj<EntityAML_BRCH_Work>(dt);
        }
        return rtnObj;
    }
    /// <summary>
    /// 讀取案件明細
    /// </summary>
    /// <returns></returns>
    public static EntityAML_BRCH_Work getAML_BRCH_WOrk(AML_SessionState parmObj)
    {
        string sSQL = @" Select
 ID,CASE_NO,FileName,BRCH_BATCH_NO,BRCH_INTER_ID,BRCH_SIXM_TOT_AMT,BRCH_MON_AMT1,BRCH_MON_AMT2,
BRCH_MON_AMT3,BRCH_MON_AMT4,BRCH_MON_AMT5,BRCH_MON_AMT6,BRCH_MON_AMT7,BRCH_MON_AMT8,BRCH_MON_AMT9,
BRCH_MON_AMT10,BRCH_MON_AMT11,BRCH_MON_AMT12,BRCH_KEY,BRCH_BRCH_NO,BRCH_BRCH_SEQ,BRCH_BRCH_TYPE,
BRCH_NATION,BRCH_BIRTH_DATE,BRCH_PERM_CITY,BRCH_PERM_ADDR1,BRCH_PERM_ADDR2,BRCH_CHINESE_NAME,
BRCH_ENGLISH_NAME,BRCH_ID,BRCH_OWNER_ID_ISSUE_DATE,BRCH_OWNER_ID_ISSUE_PLACE,BRCH_OWNER_ID_REPLACE_TYPE,
BRCH_ID_PHOTO_FLAG,BRCH_PASSPORT,BRCH_PASSPORT_EXP_DATE,BRCH_RESIDENT_NO,BRCH_RESIDENT_EXP_DATE,BRCH_OTHER_CERT,
BRCH_OTHER_CERT_EXP_DATE,BRCH_COMP_TEL,BRCH_CREATE_DATE,BRCH_STATUS,BRCH_CIRCULATE_MERCH,BRCH_HQ_BRCH_NO,
BRCH_HQ_BRCH_SEQ_NO,BRCH_UPDATE_DATE,BRCH_QUALIFY_FLAG,BRCH_RESERVED_FILLER,Create_Time,Create_User,Create_Date,
BRCH_ID_Type,BRCH_ID_SreachStatus,BRCH_ExportFileFlag,BRCH_LastExportTime,BRCH_UPDATE_ID,BRCH_REAL_CORP
, BRCH_BATCH_NO + ';' + BRCH_INTER_ID + ';' + Convert(varchar,ID) as ArgNo
  FROM  [AML_BRCH_Work]
  where BRCH_BATCH_NO = @BRCH_BATCH_NO and BRCH_INTER_ID = @BRCH_INTER_ID and   ID = @BRCH_ID;";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sSQL;
        sqlcmd.Parameters.Add(new SqlParameter("@BRCH_BATCH_NO", parmObj.RMMBatchNo));
        sqlcmd.Parameters.Add(new SqlParameter("@BRCH_INTER_ID", parmObj.AMLInternalID));
        sqlcmd.Parameters.Add(new SqlParameter("@BRCH_ID", parmObj.BRCHID));
        EntityAML_BRCH_Work rtnObj = new EntityAML_BRCH_Work();
        DataTable dt = new DataTable();
        DataSet DS = SearchOnDataSet(sqlcmd);
        if (DS != null && DS.Tables.Count > 0)
        {
            dt = DS.Tables[0];
        }
        if (dt.Rows.Count > 0)
        {
            DataTableConvertor.convSingRow<EntityAML_BRCH_Work>(ref rtnObj, dt.Rows[0]);
        }
        return rtnObj;
    }

    //20191112-RQ-2018-015749-002
    //public bool Insert(EntityAML_BRCH_Work paramObj)
    public static bool Insert(EntityAML_BRCH_Work paramObj)
    {
        bool result = false;

        try
        {

            string sSql = @" Insert into AML_BRCH_Work
(CASE_NO, FileName, BRCH_BATCH_NO, BRCH_INTER_ID, BRCH_SIXM_TOT_AMT, BRCH_MON_AMT1, BRCH_MON_AMT2, BRCH_MON_AMT3, BRCH_MON_AMT4, BRCH_MON_AMT5, BRCH_MON_AMT6, BRCH_MON_AMT7, BRCH_MON_AMT8, BRCH_MON_AMT9, BRCH_MON_AMT10, BRCH_MON_AMT11, BRCH_MON_AMT12, BRCH_KEY, BRCH_BRCH_NO, BRCH_BRCH_SEQ, BRCH_BRCH_TYPE, BRCH_NATION, BRCH_BIRTH_DATE, BRCH_PERM_CITY, BRCH_PERM_ADDR1, BRCH_PERM_ADDR2, BRCH_CHINESE_NAME, BRCH_ENGLISH_NAME, BRCH_ID, BRCH_OWNER_ID_ISSUE_DATE, BRCH_OWNER_ID_ISSUE_PLACE, BRCH_OWNER_ID_REPLACE_TYPE, BRCH_ID_PHOTO_FLAG, BRCH_PASSPORT, BRCH_PASSPORT_EXP_DATE, BRCH_RESIDENT_NO, BRCH_RESIDENT_EXP_DATE, BRCH_OTHER_CERT, BRCH_OTHER_CERT_EXP_DATE, BRCH_COMP_TEL, BRCH_CREATE_DATE, BRCH_STATUS, BRCH_CIRCULATE_MERCH, BRCH_HQ_BRCH_NO, BRCH_HQ_BRCH_SEQ_NO, BRCH_UPDATE_DATE, BRCH_QUALIFY_FLAG, BRCH_UPDATE_ID, BRCH_REAL_CORP, BRCH_RESERVED_FILLER, Create_Time, Create_User, Create_Date, BRCH_ID_Type, BRCH_ID_SreachStatus, BRCH_ExportFileFlag,BRCH_CHINESE_LNAME,BRCH_ROMA)
VALUES(  @CASE_NO, @FileName, @BRCH_BATCH_NO, @BRCH_INTER_ID, @BRCH_SIXM_TOT_AMT, @BRCH_MON_AMT1, @BRCH_MON_AMT2, @BRCH_MON_AMT3, @BRCH_MON_AMT4, @BRCH_MON_AMT5, @BRCH_MON_AMT6, @BRCH_MON_AMT7, @BRCH_MON_AMT8, @BRCH_MON_AMT9, @BRCH_MON_AMT10, @BRCH_MON_AMT11, @BRCH_MON_AMT12, @BRCH_KEY, @BRCH_BRCH_NO, @BRCH_BRCH_SEQ, @BRCH_BRCH_TYPE, @BRCH_NATION, @BRCH_BIRTH_DATE, @BRCH_PERM_CITY, @BRCH_PERM_ADDR1, @BRCH_PERM_ADDR2, @BRCH_CHINESE_NAME, @BRCH_ENGLISH_NAME, @BRCH_ID, @BRCH_OWNER_ID_ISSUE_DATE, @BRCH_OWNER_ID_ISSUE_PLACE, @BRCH_OWNER_ID_REPLACE_TYPE, @BRCH_ID_PHOTO_FLAG, @BRCH_PASSPORT, @BRCH_PASSPORT_EXP_DATE, @BRCH_RESIDENT_NO, @BRCH_RESIDENT_EXP_DATE, @BRCH_OTHER_CERT, @BRCH_OTHER_CERT_EXP_DATE, @BRCH_COMP_TEL, @BRCH_CREATE_DATE, @BRCH_STATUS, @BRCH_CIRCULATE_MERCH, @BRCH_HQ_BRCH_NO, @BRCH_HQ_BRCH_SEQ_NO, @BRCH_UPDATE_DATE, @BRCH_QUALIFY_FLAG, @BRCH_UPDATE_ID, @BRCH_REAL_CORP, @BRCH_RESERVED_FILLER, @Create_Time, @Create_User, @Create_Date, @BRCH_ID_Type, @BRCH_ID_SreachStatus, @BRCH_ExportFileFlag,@BRCH_CHINESE_LNAME,@BRCH_ROMA);
";
            SqlCommand sqlcmd = new SqlCommand();

            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sSql;


            sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", paramObj.CASE_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@FileName", paramObj.FileName));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_BATCH_NO", paramObj.BRCH_BATCH_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_INTER_ID", paramObj.BRCH_INTER_ID));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_SIXM_TOT_AMT", paramObj.BRCH_SIXM_TOT_AMT));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_MON_AMT1", paramObj.BRCH_MON_AMT1));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_MON_AMT2", paramObj.BRCH_MON_AMT2));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_MON_AMT3", paramObj.BRCH_MON_AMT3));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_MON_AMT4", paramObj.BRCH_MON_AMT4));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_MON_AMT5", paramObj.BRCH_MON_AMT5));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_MON_AMT6", paramObj.BRCH_MON_AMT6));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_MON_AMT7", paramObj.BRCH_MON_AMT7));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_MON_AMT8", paramObj.BRCH_MON_AMT8));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_MON_AMT9", paramObj.BRCH_MON_AMT9));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_MON_AMT10", paramObj.BRCH_MON_AMT10));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_MON_AMT11", paramObj.BRCH_MON_AMT11));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_MON_AMT12", paramObj.BRCH_MON_AMT12));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_KEY", paramObj.BRCH_KEY));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_BRCH_NO", paramObj.BRCH_BRCH_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_BRCH_SEQ", paramObj.BRCH_BRCH_SEQ));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_BRCH_TYPE", paramObj.BRCH_BRCH_TYPE));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_NATION", paramObj.BRCH_NATION));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_BIRTH_DATE", paramObj.BRCH_BIRTH_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_PERM_CITY", paramObj.BRCH_PERM_CITY));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_PERM_ADDR1", paramObj.BRCH_PERM_ADDR1));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_PERM_ADDR2", paramObj.BRCH_PERM_ADDR2));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_CHINESE_NAME", paramObj.BRCH_CHINESE_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_ENGLISH_NAME", paramObj.BRCH_ENGLISH_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_ID", paramObj.BRCH_ID));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_OWNER_ID_ISSUE_DATE", paramObj.BRCH_OWNER_ID_ISSUE_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_OWNER_ID_ISSUE_PLACE", paramObj.BRCH_OWNER_ID_ISSUE_PLACE));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_OWNER_ID_REPLACE_TYPE", paramObj.BRCH_OWNER_ID_REPLACE_TYPE));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_ID_PHOTO_FLAG", paramObj.BRCH_ID_PHOTO_FLAG));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_PASSPORT", paramObj.BRCH_PASSPORT));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_PASSPORT_EXP_DATE", paramObj.BRCH_PASSPORT_EXP_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_RESIDENT_NO", paramObj.BRCH_RESIDENT_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_RESIDENT_EXP_DATE", paramObj.BRCH_RESIDENT_EXP_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_OTHER_CERT", paramObj.BRCH_OTHER_CERT));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_OTHER_CERT_EXP_DATE", paramObj.BRCH_OTHER_CERT_EXP_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_COMP_TEL", paramObj.BRCH_COMP_TEL));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_CREATE_DATE", paramObj.BRCH_CREATE_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_STATUS", paramObj.BRCH_STATUS));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_CIRCULATE_MERCH", paramObj.BRCH_CIRCULATE_MERCH));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_HQ_BRCH_NO", paramObj.BRCH_HQ_BRCH_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_HQ_BRCH_SEQ_NO", paramObj.BRCH_HQ_BRCH_SEQ_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_UPDATE_DATE", paramObj.BRCH_UPDATE_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_QUALIFY_FLAG", paramObj.BRCH_QUALIFY_FLAG));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_UPDATE_ID", paramObj.BRCH_UPDATE_ID));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_REAL_CORP", paramObj.BRCH_REAL_CORP));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_RESERVED_FILLER", paramObj.BRCH_RESERVED_FILLER));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_Time", paramObj.Create_Time));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_User", paramObj.Create_User));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_Date", paramObj.Create_Date));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_ID_Type", paramObj.BRCH_ID_Type));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_ID_SreachStatus", paramObj.BRCH_ID_SreachStatus));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_ExportFileFlag", paramObj.BRCH_ExportFileFlag));
            //sqlcmd.Parameters.Add(new SqlParameter("@BRCH_LastExportTime", paramObj.BRCH_LastExportTime));
            //20191225 - 補上遺漏長姓名欄位資訊
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_CHINESE_LNAME", paramObj.BRCH_CHINESE_LNAME));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_ROMA", paramObj.BRCH_ROMA));
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
    /// 修改公司資料
    /// </summary>
    /// <returns></returns>
    public static bool UpdateAML_BRCH_WOrk(EntityAML_BRCH_Work paramObj)
    {
        bool result = false;
        try
        {
            //20191226-RQ-2019-030155-002 因編輯模式變更，故在單筆編輯後，即上exportflag，不再一次更新
            //20191202-RQ-2018-015749-002需求：可異動分公司負責人生日欄位(BRCH_BIRTH_DATE)
            //20200113-因異動分公司模式改變，故新增更新flag 及時間欄位
            string sSql = @" update AML_BRCH_Work
                                            set  
                                                    BRCH_OWNER_ID_ISSUE_DATE = @BRCH_OWNER_ID_ISSUE_DATE,
                                                    BRCH_OWNER_ID_ISSUE_PLACE = @BRCH_OWNER_ID_ISSUE_PLACE,
                                                    BRCH_OWNER_ID_REPLACE_TYPE = @BRCH_OWNER_ID_REPLACE_TYPE,
                                                    BRCH_ID_PHOTO_FLAG = @BRCH_ID_PHOTO_FLAG,
                                                    BRCH_NATION = @BRCH_NATION,
                                                    BRCH_PASSPORT_EXP_DATE = @BRCH_PASSPORT_EXP_DATE,
                                                    BRCH_RESIDENT_EXP_DATE = @BRCH_RESIDENT_EXP_DATE,
                                                    BRCH_ID_SreachStatus = @BRCH_ID_SreachStatus,
                                                    BRCH_BIRTH_DATE = @BRCH_BIRTH_DATE ,
                                                    BRCH_ExportFileFlag = @BRCH_ExportFileFlag,
                                                    BRCH_LastExportTime = GETDATE(),
                                                    LAST_UPD_BRANCH = @LAST_UPD_BRANCH,
                                                    LAST_UPD_MAKER = @LAST_UPD_MAKER,
                                                    LAST_UPD_CHECKER = @LAST_UPD_CHECKER
                                              where ID = @ID AND BRCH_BRCH_NO = @BRCH_BRCH_NO ";
            SqlCommand sqlcmd = new SqlCommand();

            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sSql;

            sqlcmd.Parameters.Add(new SqlParameter("@ID", paramObj.ID));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_OWNER_ID_ISSUE_DATE", paramObj.BRCH_OWNER_ID_ISSUE_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_OWNER_ID_ISSUE_PLACE", paramObj.BRCH_OWNER_ID_ISSUE_PLACE));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_OWNER_ID_REPLACE_TYPE", paramObj.BRCH_OWNER_ID_REPLACE_TYPE));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_ID_PHOTO_FLAG", paramObj.BRCH_ID_PHOTO_FLAG));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_NATION", paramObj.BRCH_NATION));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_PASSPORT_EXP_DATE", paramObj.BRCH_PASSPORT_EXP_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_RESIDENT_EXP_DATE", paramObj.BRCH_RESIDENT_EXP_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_BRCH_NO", paramObj.BRCH_BRCH_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_ID_SreachStatus", paramObj.BRCH_ID_SreachStatus));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_BIRTH_DATE", paramObj.BRCH_BIRTH_DATE)); //20191202-RQ-2018-015749-002需求：可異動分公司負責人生日欄位(BRCH_BIRTH_DATE)
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_ExportFileFlag", paramObj.BRCH_ExportFileFlag)); //20191202-RQ-2018-015749-002需求：可異動分公司負責人生日欄位(BRCH_BIRTH_DATE)
            //20210908 by jack 異動分行 = 9999, maker = 登入人ID, checker = 空白
            sqlcmd.Parameters.Add(new SqlParameter("@LAST_UPD_BRANCH", paramObj.LAST_UPD_BRANCH));// 異動分行
            sqlcmd.Parameters.Add(new SqlParameter("@LAST_UPD_MAKER", paramObj.LAST_UPD_MAKER));// 異動MAKER
            sqlcmd.Parameters.Add(new SqlParameter("@LAST_UPD_CHECKER", paramObj.LAST_UPD_CHECKER));// 異動CHECKER
            result = Update(sqlcmd);
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
    /// 更新產檔旗標
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool Update_BRCH_ExportFileFlagr(AML_SessionState paramObj)
    {
        bool result = false;
        if (paramObj != null)
        {
            if (!String.IsNullOrEmpty(paramObj.CASE_NO))
            {
                try
                {
                    string sSql = @" UPDATE [AML_BRCH_Work]
                                    SET BRCH_ExportFileFlag = '1',BRCH_LastExportTime =getdate()
                                   where ID = @BRCH_ID ";
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.CommandType = CommandType.Text;
                    sqlcmd.CommandText = sSql;
                    sqlcmd.Parameters.Add(new SqlParameter("@BRCH_ID", paramObj.BRCHID));

                    ///使用交易
                    using (OMTransactionScope ts = new OMTransactionScope())
                    {
                        result = Update(sqlcmd);
                        if (result)
                            ts.Complete();
                    }
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                }
            }
        }
        return result;
    }

    //20191105-RQ-2018-015749-002 add by Peggy
    /// <summary>
    /// 讀取案件的分公司年度請款金額的每月請款金額明細
    /// </summary>
    /// <returns></returns>
    public static DataTable GetBRCHMonthAMT(string _CaseNo, string _BrchBrchNo)
    {
        string sSQL = @" 
            SELECT [CASE_NO],[BRCH_BRCH_NO]
                        ,[BRCH_SIXM_TOT_AMT]
                        ,[BRCH_MON_AMT1]
                        ,[BRCH_MON_AMT2]
                        ,[BRCH_MON_AMT3]
                        ,[BRCH_MON_AMT4]
                        ,[BRCH_MON_AMT5]
                        ,[BRCH_MON_AMT6]
                        ,[BRCH_MON_AMT7]
                        ,[BRCH_MON_AMT8]
                        ,[BRCH_MON_AMT9]
                        ,[BRCH_MON_AMT10]
                        ,[BRCH_MON_AMT11]
                        ,[BRCH_MON_AMT12]
              FROM [dbo].[AML_BRCH_Work]
              WHERE CASE_NO = @case_no AND BRCH_BRCH_NO = @brch_brch_no";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sSQL;
        sqlcmd.Parameters.Add(new SqlParameter("@case_no", _CaseNo));
        sqlcmd.Parameters.Add(new SqlParameter("@brch_brch_no", _BrchBrchNo));

        DataTable dt = new DataTable();
        DataSet DS = SearchOnDataSet(sqlcmd);
        if (DS != null && DS.Tables.Count > 0)
        {
            dt = DS.Tables[0];
        }

        return dt;

    }

    /// <summary>
    /// 讀取案件明細
    /// </summary>
    /// <returns></returns>
    public static EntityAML_BRCH_Work GetDataAML_BRCH_WORK(AML_SessionState parmObj)
    {
        string sSQL = @" SELECT ID,CASE_NO,[FileName],BRCH_BATCH_NO,BRCH_INTER_ID,BRCH_SIXM_TOT_AMT,BRCH_MON_AMT1,BRCH_MON_AMT2,BRCH_MON_AMT3,BRCH_MON_AMT4,BRCH_MON_AMT5,BRCH_MON_AMT6,BRCH_MON_AMT7,BRCH_MON_AMT8,BRCH_MON_AMT9,BRCH_MON_AMT10,BRCH_MON_AMT11,BRCH_MON_AMT12
                                        ,BRCH_KEY,BRCH_BRCH_NO,BRCH_BRCH_SEQ,BRCH_BRCH_TYPE,BRCH_NATION,BRCH_BIRTH_DATE,BRCH_PERM_CITY,BRCH_PERM_ADDR1,BRCH_PERM_ADDR2,BRCH_CHINESE_NAME,BRCH_ENGLISH_NAME,BRCH_ID,BRCH_OWNER_ID_ISSUE_DATE,BRCH_OWNER_ID_ISSUE_PLACE,BRCH_OWNER_ID_REPLACE_TYPE
                                        ,BRCH_ID_PHOTO_FLAG,BRCH_PASSPORT,BRCH_PASSPORT_EXP_DATE,BRCH_RESIDENT_NO,BRCH_RESIDENT_EXP_DATE,BRCH_OTHER_CERT,BRCH_OTHER_CERT_EXP_DATE,BRCH_COMP_TEL,BRCH_CREATE_DATE,BRCH_STATUS,BRCH_CIRCULATE_MERCH,BRCH_HQ_BRCH_NO,BRCH_HQ_BRCH_SEQ_NO,BRCH_UPDATE_DATE
                                        ,BRCH_QUALIFY_FLAG,BRCH_UPDATE_ID,BRCH_REAL_CORP,BRCH_RESERVED_FILLER,Create_Time,Create_User,Create_Date,BRCH_ID_Type,BRCH_ID_SreachStatus,BRCH_ExportFileFlag,BRCH_LastExportTime,BRCH_CHINESE_LNAME,BRCH_ROMA, BRCH_NATU_ID, BRCH_REG_CHI_NAME, BRCH_REG_ENG_NAME
                                        FROM dbo.AML_BRCH_Work  
                                         WHERE CASE_NO = @CASE_NO;";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sSQL;
        sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", parmObj.CASE_NO));

        EntityAML_BRCH_Work rtnObj = new EntityAML_BRCH_Work();
        DataTable dt = new DataTable();
        DataSet DS = SearchOnDataSet(sqlcmd);
        if (DS != null && DS.Tables.Count > 0)
        {
            dt = DS.Tables[0];
        }
        if (dt.Rows.Count > 0)
        {
            DataTableConvertor.convSingRow<EntityAML_BRCH_Work>(ref rtnObj, dt.Rows[0]);
        }
        return rtnObj;
    }

    /// <summary>
    /// 讀取案件明細
    /// </summary>
    /// <returns></returns>
    public static List<EntityAML_BRCH_Work> getBRCH_WOrkCollList(AML_SessionState paramObj)
    {
        string sSQL = @" Select
         CASE_NO,FileName,BRCH_BATCH_NO,BRCH_INTER_ID,BRCH_SIXM_TOT_AMT,BRCH_MON_AMT1,BRCH_MON_AMT2,
        BRCH_MON_AMT3,BRCH_MON_AMT4,BRCH_MON_AMT5,BRCH_MON_AMT6,BRCH_MON_AMT7,BRCH_MON_AMT8,BRCH_MON_AMT9,
        BRCH_MON_AMT10,BRCH_MON_AMT11,BRCH_MON_AMT12,BRCH_KEY,BRCH_BRCH_NO,BRCH_BRCH_SEQ,BRCH_BRCH_TYPE,
        BRCH_NATION,BRCH_BIRTH_DATE,BRCH_PERM_CITY,BRCH_PERM_ADDR1,BRCH_PERM_ADDR2,BRCH_CHINESE_NAME,
        BRCH_ENGLISH_NAME,BRCH_ID,BRCH_OWNER_ID_ISSUE_DATE,BRCH_OWNER_ID_ISSUE_PLACE,BRCH_OWNER_ID_REPLACE_TYPE,
        BRCH_ID_PHOTO_FLAG,BRCH_PASSPORT,BRCH_PASSPORT_EXP_DATE,BRCH_RESIDENT_NO,BRCH_RESIDENT_EXP_DATE,BRCH_OTHER_CERT,
        BRCH_OTHER_CERT_EXP_DATE,BRCH_COMP_TEL,BRCH_CREATE_DATE,BRCH_STATUS,BRCH_CIRCULATE_MERCH,BRCH_HQ_BRCH_NO,
        BRCH_HQ_BRCH_SEQ_NO,BRCH_UPDATE_DATE,BRCH_QUALIFY_FLAG,BRCH_RESERVED_FILLER,Create_Time,Create_User,Create_Date,
        BRCH_ID_Type,BRCH_ID_SreachStatus,BRCH_ExportFileFlag,BRCH_LastExportTime,BRCH_UPDATE_ID,BRCH_REAL_CORP
        ,BRCH_CHINESE_LNAME,BRCH_ROMA
        , BRCH_BATCH_NO + ';' + BRCH_INTER_ID + ';' + Convert(varchar,ID) as ArgNo
          FROM  [AML_BRCH_Work]
           where 1 = 1 AND CASE_NO = @CASE_NO";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sSQL;
        sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", paramObj.CASE_NO));
        
        List<EntityAML_BRCH_Work> rtnObj = new List<EntityAML_BRCH_Work>();
        DataTable dt = new DataTable();
        DataSet DS = SearchOnDataSet(sqlcmd);
        if (DS != null && DS.Tables.Count > 0)
        {
            dt = DS.Tables[0];
        }
        if (dt.Rows.Count > 0)
        {
            rtnObj = DataTableConvertor.ConvertCollToObj<EntityAML_BRCH_Work>(dt);
        }
        return rtnObj;
    }

    public static DataSet GetCaseNOBrch(string _CaseNO)
    {
        string sSQL = @" SELECT *,'' AS UP_FLAG FROM AML_BRCH_Work WITH(NOLOCK) WHERE 1 = 1 AND CASE_NO = @CASE_NO";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sSQL;
        sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", _CaseNO));
        
        DataTable dt = new DataTable();
        DataSet DS = SearchOnDataSet(sqlcmd);
        return DS;
    }


    /// <summary>
    /// GetToESB_Query 電文-NameCheck
    /// *****************修改記錄*****************
    /// RQ-2019-030155-005
    /// 建立者：Ray
    /// 建立日期：20200414
    /// -----------------------------------
    /// RQ-2019-030155-000
    /// 修改者：Ray
    /// 修改日期：20200609
    /// 修改事項：新增拋查分公司登記資料
    /// *****************修改記錄*****************
    /// </summary>
    /// <param name="_CaseNo"></param>
    /// <returns></returns>
    public static void GetToESB_Query(string _CaseNo, ref DataSet ref_Ds)
    {
        //20211014_Ares_Jack_JOIN AML_BRCH_STATUS拿掉, 直接用新欄位 BRCH_REG_CHI_NAME
        string sSQL = @" 
            --@分公司負責人，身分類型(LA/LA) 
            SELECT A.* , 
            ----Row_No 分公司開頭為20
            20 + ROW_NUMBER() over (order by ITEM) as ROW_NO ,
            ----ESB_TYPE 分公司為2
            '2' as ESB_TYPE FROM 
            (
            --@分公司負責人．抓短姓名跟英文 
            SELECT 
            CASE_NO ,
		    BRCH_REG_CHI_NAME AS CHINESE_NAME ,
		    BRCH_REG_ENG_NAME AS ENGLISH_NAME , 
            BRCH_BIRTH_DATE AS BIRTH_DATE ,
            ----非台灣國籍 居留證號碼  > 護照號碼
            CASE
			WHEN BRCH_NATION <> 'TW' AND BRCH_RESIDENT_NO <> '' THEN BRCH_RESIDENT_NO 
			WHEN BRCH_NATION <> 'TW' AND BRCH_RESIDENT_NO = '' AND BRCH_PASSPORT <> '' THEN BRCH_PASSPORT
			ELSE BRCH_ID
			END ID,
            BRCH_BATCH_NO AS BATCH_NO ,
            BRCH_NATION AS NATION ,
            ----紀錄公司負責人ITEM
            '分公司負責人' + CONVERT(varchar ,(ROW_NUMBER() over (order by ID)))+ '.1' as ITEM
            ----ConnectedPartySubType , ConnectedPartyType 為LA
            ,'LA' as ConnectedPartySubType , 'LA' as ConnectedPartyType  
            ,'' AS HCOP_CorpNO 
            FROM [dbo].[AML_BRCH_WORK] WHERE CASE_NO=@CASE_NO AND (BRCH_CHINESE_NAME <> '' OR BRCH_ENGLISH_NAME <> '')
            UNION
            --@分公司負責人．抓長姓名跟羅馬拼音 
            SELECT 
            CASE_NO ,
		    BRCH_REG_CHI_NAME AS CHINESE_NAME ,
		    BRCH_REG_ENG_NAME AS  ENGLISH_NAME , 
            BRCH_BIRTH_DATE AS BIRTH_DATE ,
            ----非台灣國籍 居留證號碼  > 護照號碼
            CASE
			WHEN BRCH_NATION <> 'TW' AND BRCH_RESIDENT_NO <> '' THEN BRCH_RESIDENT_NO 
			WHEN BRCH_NATION <> 'TW' AND BRCH_RESIDENT_NO = '' AND BRCH_PASSPORT <> '' THEN BRCH_PASSPORT
			ELSE BRCH_ID
			END ID,
            BRCH_BATCH_NO AS BATCH_NO ,
            BRCH_NATION AS NATION ,
            ----紀錄公司負責人ITEM
            '分公司負責人' + CONVERT(varchar ,(ROW_NUMBER() over (order by ID)) + 0.11) as ITEM 
            ----ConnectedPartySubType , ConnectedPartyType 為LA
            ,'LA' as ConnectedPartySubType , 'LA' as ConnectedPartyType
            ,'' AS HCOP_CorpNO   
            FROM [dbo].[AML_BRCH_WORK] WHERE CASE_NO=@CASE_NO 
            UNION
            --@分公司登記名稱
            SELECT 
            W.CASE_NO ,
		    W.BRCH_REG_CHI_NAME AS CHINESE_NAME ,
		    W.BRCH_REG_ENG_NAME AS  ENGLISH_NAME , 
            W.BRCH_BUILD_DATE AS BIRTH_DATE ,
            W.BRCH_BRCH_NO AS ID ,
            W.BRCH_BATCH_NO AS BATCH_NO ,
            W.BRCH_NATION AS NATION ,
            '分公司負責人' + CONVERT(varchar ,(ROW_NUMBER() over (order by ID))) + '.#'  as ITEM 
            ----ConnectedPartySubType為OA , ConnectedPartyType 為APP
            ,'OA' as ConnectedPartySubType , 'APP' as ConnectedPartyType
            ,'' AS HCOP_CorpNO   
            FROM [dbo].[AML_BRCH_WORK] W 
            WHERE W.CASE_NO=@CASE_NO
            ) A WHERE (CHINESE_NAME <> '' OR ENGLISH_NAME <> '') OR CHARINDEX('#',ITEM) <> 0 order by ITEM  ";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sSQL;
        sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", _CaseNo));

        DataSet DS = SearchOnDataSet(sqlcmd);

        if (DS != null && DS.Tables.Count > 0)
        {
            DS.Tables[0].TableName = "AML_BRCH_WORK";
            ref_Ds.Tables.Add(DS.Tables[0].Copy());
        }
    }
}