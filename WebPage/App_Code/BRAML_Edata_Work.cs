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
public class BRAML_Edata_Work : CSIPCommonModel.BusinessRules.BRBase<EntityAML_Edata_Work>
{
    public BRAML_Edata_Work()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }
    /// <summary>
    /// 新增多筆
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public bool Insert(List<EntityAML_Edata_Work> paramObj)
    {
        bool result = false;
        ///使用交易
        using (OMTransactionScope ts = new OMTransactionScope())
        {
            foreach (EntityAML_Edata_Work scOBJ in paramObj)
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
    public bool Insert(EntityAML_Edata_Work paramObj)
    {
        bool result = false; 
            try
            {

                string sSql = @"Insert into AML_Edata_Work 
( CASE_NO,FileName,RMMBatchNo,AMLInternalID,DataDate,CustomerID,CustomerEnglishName,CustomerChineseName,AMLSegment,OriginalRiskRanking,OriginalNextReviewDate,NewRiskRanking,NewNextReviewDate,LastUpdateMaker,LastUpdateBranch,LastUpdateDate,HomeBranch,CaseType,FiledSAR,Filler,Create_Time,Create_User,Create_Date)
VALUES( @CASE_NO,@FileName,@RMMBatchNo,@AMLInternalID,@DataDate,@CustomerID,@CustomerEnglishName,@CustomerChineseName,@AMLSegment,@OriginalRiskRanking,@OriginalNextReviewDate,@NewRiskRanking,@NewNextReviewDate,@LastUpdateMaker,@LastUpdateBranch,@LastUpdateDate,@HomeBranch,@CaseType,@FiledSAR,@Filler,@Create_Time,@Create_User,@Create_Date);
";
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
             
                sqlcmd.CommandText = sSql;


                //   sqlcmd.Parameters.Add(new SqlParameter("@ID", paramObj.ID));
                sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", paramObj.CASE_NO));
                sqlcmd.Parameters.Add(new SqlParameter("@FileName", paramObj.FileName));
                sqlcmd.Parameters.Add(new SqlParameter("@RMMBatchNo", paramObj.RMMBatchNo));
                sqlcmd.Parameters.Add(new SqlParameter("@AMLInternalID", paramObj.AMLInternalID));
                sqlcmd.Parameters.Add(new SqlParameter("@DataDate", paramObj.DataDate));
                sqlcmd.Parameters.Add(new SqlParameter("@CustomerID", paramObj.CustomerID));
                sqlcmd.Parameters.Add(new SqlParameter("@CustomerEnglishName", paramObj.CustomerEnglishName));
                sqlcmd.Parameters.Add(new SqlParameter("@CustomerChineseName", paramObj.CustomerChineseName));
                sqlcmd.Parameters.Add(new SqlParameter("@AMLSegment", paramObj.AMLSegment));
                sqlcmd.Parameters.Add(new SqlParameter("@OriginalRiskRanking", paramObj.OriginalRiskRanking));
                sqlcmd.Parameters.Add(new SqlParameter("@OriginalNextReviewDate", paramObj.OriginalNextReviewDate));
                sqlcmd.Parameters.Add(new SqlParameter("@NewRiskRanking", paramObj.NewRiskRanking));
                sqlcmd.Parameters.Add(new SqlParameter("@NewNextReviewDate", paramObj.NewNextReviewDate));
                sqlcmd.Parameters.Add(new SqlParameter("@LastUpdateMaker", paramObj.LastUpdateMaker));
                sqlcmd.Parameters.Add(new SqlParameter("@LastUpdateBranch", paramObj.LastUpdateBranch));
                sqlcmd.Parameters.Add(new SqlParameter("@LastUpdateDate", paramObj.LastUpdateDate));
                sqlcmd.Parameters.Add(new SqlParameter("@HomeBranch", paramObj.HomeBranch));
                sqlcmd.Parameters.Add(new SqlParameter("@CaseType", paramObj.CaseType));
                sqlcmd.Parameters.Add(new SqlParameter("@FiledSAR", paramObj.FiledSAR));
                sqlcmd.Parameters.Add(new SqlParameter("@Filler", paramObj.Filler));
                sqlcmd.Parameters.Add(new SqlParameter("@Create_Time", paramObj.Create_Time));
                sqlcmd.Parameters.Add(new SqlParameter("@Create_User", paramObj.Create_User));
                sqlcmd.Parameters.Add(new SqlParameter("@Create_Date", paramObj.Create_Date));

                result = Add(sqlcmd);
            }
            catch (Exception ex)
            {
                string ms = ex.Message;
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
    public static EntityAML_Edata_Work getAML_Edata_Work(AML_SessionState parmObj)
    {
        

        string sSQL = @" SELECT ID, CASE_NO,[FileName], RMMBatchNo, AMLInternalID, DataDate, CustomerID, CustomerEnglishName, CustomerChineseName, AMLSegment, OriginalRiskRanking, OriginalNextReviewDate
                                        , NewRiskRanking, NewNextReviewDate, LastUpdateMaker, LastUpdateBranch, LastUpdateDate, HomeBranch, CaseType, FiledSAR, Filler, Create_Time, Create_User, Create_Date  
                                          FROM dbo.AML_Edata_Work
                                          WHERE 1=1 ";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        if (parmObj != null)
        {
            if (!String.IsNullOrEmpty(parmObj.CASE_NO))
            {
                sSQL += " AND CASE_NO = @CASE_NO ";
                sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", parmObj.CASE_NO));
            }
        }

        sqlcmd.CommandText = sSQL;

        EntityAML_Edata_Work rtnObj = new EntityAML_Edata_Work();
        DataTable dt = new DataTable();
        DataSet DS = SearchOnDataSet(sqlcmd);
        if (DS != null && DS.Tables.Count > 0)
        {
            dt = DS.Tables[0];
        }
        if (dt.Rows.Count > 0)
        {
            DataTableConvertor.convSingRow<EntityAML_Edata_Work>(ref rtnObj, dt.Rows[0]);
        }
        return rtnObj;
    }

    /// <summary>
    /// 新增單筆
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool InsertBySingle(EntityAML_Edata_Work paramObj)
    {
        bool result = false;

        try
        {

            string sSql = @"INSERT INTO dbo.AML_Edata_Work
                                           (CASE_NO
                                           ,FileName
                                           ,RMMBatchNo
                                           ,AMLInternalID
                                           ,DataDate
                                           ,CustomerID
                                           ,CustomerEnglishName
                                           ,CustomerChineseName
                                           ,AMLSegment
                                           ,OriginalRiskRanking
                                           ,OriginalNextReviewDate
                                           ,NewRiskRanking
                                           ,NewNextReviewDate
                                           ,LastUpdateMaker
                                           ,LastUpdateBranch
                                           ,LastUpdateDate
                                           ,HomeBranch
                                           ,CaseType
                                           ,FiledSAR
                                           ,Filler
                                           ,Create_Time
                                           ,Create_User
                                           ,Create_Date) 
                                            VALUES(@CASE_NO,@FileName,@RMMBatchNo,@AMLInternalID,@DataDate,@CustomerID,@CustomerEnglishName ,@CustomerChineseName,@AMLSegment,@OriginalRiskRanking,@OriginalNextReviewDate,@NewRiskRanking,@NewNextReviewDate ,@LastUpdateMaker ,@LastUpdateBranch ,@LastUpdateDate ,@HomeBranch ,@CaseType ,@FiledSAR ,@Filler ,@Create_Time ,@Create_User ,@Create_Date);
                                        ";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sSql;

            sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", paramObj.CASE_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@FileName", paramObj.FileName));
            sqlcmd.Parameters.Add(new SqlParameter("@RMMBatchNo", paramObj.RMMBatchNo));
            sqlcmd.Parameters.Add(new SqlParameter("@AMLInternalID", paramObj.AMLInternalID));
            sqlcmd.Parameters.Add(new SqlParameter("@DataDate", paramObj.DataDate));
            sqlcmd.Parameters.Add(new SqlParameter("@CustomerID", paramObj.CustomerID));
            sqlcmd.Parameters.Add(new SqlParameter("@CustomerEnglishName", paramObj.CustomerEnglishName));
            sqlcmd.Parameters.Add(new SqlParameter("@CustomerChineseName", paramObj.CustomerChineseName));
            sqlcmd.Parameters.Add(new SqlParameter("@AMLSegment", paramObj.AMLSegment));
            sqlcmd.Parameters.Add(new SqlParameter("@OriginalRiskRanking", paramObj.OriginalRiskRanking));
            sqlcmd.Parameters.Add(new SqlParameter("@OriginalNextReviewDate", paramObj.OriginalNextReviewDate));
            sqlcmd.Parameters.Add(new SqlParameter("@NewRiskRanking", paramObj.NewRiskRanking));
            sqlcmd.Parameters.Add(new SqlParameter("@NewNextReviewDate", paramObj.NewNextReviewDate));
            sqlcmd.Parameters.Add(new SqlParameter("@LastUpdateMaker", paramObj.LastUpdateMaker));
            sqlcmd.Parameters.Add(new SqlParameter("@LastUpdateBranch", paramObj.LastUpdateBranch));
            sqlcmd.Parameters.Add(new SqlParameter("@LastUpdateDate", paramObj.LastUpdateDate));
            sqlcmd.Parameters.Add(new SqlParameter("@HomeBranch", paramObj.HomeBranch));
            sqlcmd.Parameters.Add(new SqlParameter("@CaseType", paramObj.CaseType));
            sqlcmd.Parameters.Add(new SqlParameter("@FiledSAR", paramObj.FiledSAR));
            sqlcmd.Parameters.Add(new SqlParameter("@Filler", paramObj.Filler));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_Time", paramObj.Create_Time));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_User", paramObj.Create_User));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_Date", paramObj.Create_Date));

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