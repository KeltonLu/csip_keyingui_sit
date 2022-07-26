//******************************************************************
//*  作        者：
//*  功能說明：
//*  建立日期：
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*Ares Luke          2021/02/22         20200031-CSIP EOS       調整connectionStr方式
//*******************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM;
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.EntityLayer;
using System.Data;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using System.Configuration;
using System.Data.SqlClient;
using Framework.Common;
using Framework.Common.Message;
using Framework.Common.Logging;
using Framework.Common.Utility;
using CSIPNewInvoice.EntityLayer_new;
using CSIPCommonModel.BusinessRules;

/// <summary>
/// BRAML_File_Import 的摘要描述
/// </summary>
public class BRAML_File_Import : BRBase<Entity_AML_IMP_LOG>
{
    // 取 FTP 資訊
    public static DataTable GetFileInfo(string job_ID)
    {
        DataTable result = new DataTable();

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"
SELECT [Job_ID], [FtpFileName], [FtpPath], [ZipPwd], [FtpIP], [FtpUserName], [FtpPwd], [Status], [LoopMinutes], [Parameter] ,[IsSendHTG]
FROM [dbo].[tbl_FileInfo] WITH(NOLOCK)
WHERE Job_ID = @Job_ID";
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@Job_ID", job_ID));

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null && resultSet.Tables.Count > 0)
            {
                result = resultSet.Tables[0];
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return result;
    }

    // 判斷檔案是否已存在
    public static bool IsEFileExist(string fileName)
    {
        bool isFileExist = false;

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"
SELECT COUNT(0) FROM [dbo].[AML_IMP_LOG] WITH(NOLOCK)
WHERE FileName = @FileName AND [Status] = 'Y'";
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@FileName", fileName));

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null && resultSet.Tables.Count > 0)
            {
                int masterID = Convert.ToInt32(resultSet.Tables[0].Rows[0][0].ToString());
                if (masterID > 0)
                {
                    isFileExist = true;
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return isFileExist;
    }

    // 寫入 AML_Edata_Temp
    public static bool InsertAMLEdataTemp(string tableName, DataTable dataTable)
    {
        bool result = false;
        // string connnection = System.Configuration.ConfigurationManager.ConnectionStrings["Connection_System"].ConnectionString;
        string connnection = UtilHelper.GetConnectionStrings("Connection_System");
        SqlConnection conn = new SqlConnection(connnection);
        SqlBulkCopy sbc = new SqlBulkCopy(connnection);
        sbc.DestinationTableName = tableName;

        try
        {
            conn.Open();

            sbc.ColumnMappings.Add("RMMBatchNo", "RMMBatchNo");
            sbc.ColumnMappings.Add("AMLInternalID", "AMLInternalID");

            sbc.WriteToServer(dataTable);

            result = true;
            return result;
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.DB);
            return result;
        }
        finally
        {
            sbc.Close();
            conn.Close();
            conn.Dispose();
        }
    }

    // 判斷資料內容是否重覆
    public static bool IsDuplicateData()
    {
        bool IsDuplicate = false;

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"
SELECT COUNT(*) FROM [dbo].[AML_Edata_Import] a WITH(NOLOCK)
INNER JOIN [dbo].[AML_Edata_Temp] b WITH(NOLOCK) ON a.RMMBatchNo = b.RMMBatchNo AND a.AMLInternalID = b.AMLInternalID";
        sqlcmd.CommandType = CommandType.Text;

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null && resultSet.Tables.Count > 0)
            {
                int masterID = Convert.ToInt32(resultSet.Tables[0].Rows[0][0].ToString());
                if (masterID > 0)
                {
                    IsDuplicate = true;
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return IsDuplicate;
    }


    // 判斷資料內容是否重覆
    public static string IsDuplicateDataCaseNo()
    {
        string strCaseNo = "";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"
SELECT a.rmmbatchno,a.amlinternalid FROM [dbo].[AML_Edata_Import] a WITH(NOLOCK)
INNER JOIN [dbo].[AML_Edata_Temp] b WITH(NOLOCK) ON a.RMMBatchNo = b.RMMBatchNo AND a.AMLInternalID = b.AMLInternalID

";
        sqlcmd.CommandType = CommandType.Text;

        try
        {
            DataTable result = new DataTable();

            DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
            if (resultSet != null && resultSet.Tables.Count > 0)
            {
                result = resultSet.Tables[0];
            }


            foreach (DataRow row in result.Rows)
            {
                strCaseNo = strCaseNo + "rmmbatchno=>"+row["rmmbatchno"].ToString() + "amlinternalid=>" + row["amlinternalid"].ToString()+";\r\n";

            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return strCaseNo;
    }


    // 寫入 AML_IMP_LOG
    public static int InsertAMLIMPLOG(DataTable sourceDT)
    {
        int masterID = 0;

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"
INSERT INTO [dbo].[AML_IMP_LOG] ([FILE_TYPE], [FileName], [IMP_DATE], [IMP_TIME], [Create_User], [Count], [Status], [IMP_OK_COUNT], [IMP_FAIL_COUNT])
	VALUES (@FILE_TYPE, @FileName, @IMP_DATE, @IMP_TIME, @Create_User, @Count, @Status, @IMP_OK_COUNT, @IMP_FAIL_COUNT)
SELECT SCOPE_IDENTITY()";
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@FILE_TYPE", sourceDT.Rows[0]["FILE_TYPE"].ToString()));
        sqlcmd.Parameters.Add(new SqlParameter("@FileName", sourceDT.Rows[0]["FileName"].ToString()));
        sqlcmd.Parameters.Add(new SqlParameter("@IMP_DATE", sourceDT.Rows[0]["IMP_DATE"].ToString()));
        sqlcmd.Parameters.Add(new SqlParameter("@IMP_TIME", sourceDT.Rows[0]["IMP_TIME"].ToString()));
        sqlcmd.Parameters.Add(new SqlParameter("@Create_User", sourceDT.Rows[0]["Create_User"].ToString()));
        sqlcmd.Parameters.Add(new SqlParameter("@Count", sourceDT.Rows[0]["Count"].ToString()));
        sqlcmd.Parameters.Add(new SqlParameter("@Status", sourceDT.Rows[0]["Status"].ToString()));
        sqlcmd.Parameters.Add(new SqlParameter("@IMP_OK_COUNT", sourceDT.Rows[0]["IMP_OK_COUNT"].ToString()));
        sqlcmd.Parameters.Add(new SqlParameter("@IMP_FAIL_COUNT", sourceDT.Rows[0]["IMP_FAIL_COUNT"].ToString()));

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null && resultSet.Tables.Count > 0)
            {

                masterID = Convert.ToInt32(resultSet.Tables[0].Rows[0][0].ToString());
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return masterID;
    }

    // 寫入 AML_Edata_Import、AML_Edata_Work
    public static bool InsertAMLEdataImport(string tableName, DataTable dataTable)
    {
        bool result = false;
        //string connnection = System.Configuration.ConfigurationManager.ConnectionStrings["Connection_System"].ConnectionString;
        string connnection = UtilHelper.GetConnectionStrings("Connection_System");

        SqlConnection conn = new SqlConnection(connnection);
        SqlBulkCopy sbc = new SqlBulkCopy(connnection);
        sbc.DestinationTableName = tableName;

        try
        {
            conn.Open();

            sbc.ColumnMappings.Add("CASE_NO", "CASE_NO");
            sbc.ColumnMappings.Add("FileName", "FileName");
            sbc.ColumnMappings.Add("RMMBatchNo", "RMMBatchNo");
            sbc.ColumnMappings.Add("AMLInternalID", "AMLInternalID");
            sbc.ColumnMappings.Add("DataDate", "DataDate");
            sbc.ColumnMappings.Add("CustomerID", "CustomerID");
            sbc.ColumnMappings.Add("CustomerEnglishName", "CustomerEnglishName");
            sbc.ColumnMappings.Add("CustomerChineseName", "CustomerChineseName");
            sbc.ColumnMappings.Add("AMLSegment", "AMLSegment");
            sbc.ColumnMappings.Add("OriginalRiskRanking", "OriginalRiskRanking");
            sbc.ColumnMappings.Add("OriginalNextReviewDate", "OriginalNextReviewDate");
            sbc.ColumnMappings.Add("NewRiskRanking", "NewRiskRanking");
            sbc.ColumnMappings.Add("NewNextReviewDate", "NewNextReviewDate");
            sbc.ColumnMappings.Add("LastUpdateMaker", "LastUpdateMaker");
            sbc.ColumnMappings.Add("LastUpdateBranch", "LastUpdateBranch");
            sbc.ColumnMappings.Add("LastUpdateDate", "LastUpdateDate");
            sbc.ColumnMappings.Add("HomeBranch", "HomeBranch");
            sbc.ColumnMappings.Add("CaseType", "CaseType");
            sbc.ColumnMappings.Add("FiledSAR", "FiledSAR");
            sbc.ColumnMappings.Add("Filler", "Filler");
            sbc.ColumnMappings.Add("Create_Time", "Create_Time");
            sbc.ColumnMappings.Add("Create_User", "Create_User");
            sbc.ColumnMappings.Add("Create_Date", "Create_Date");

            sbc.WriteToServer(dataTable);

            result = true;
            return result;
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.DB);
            return result;
        }
        finally
        {
            sbc.Close();
            conn.Close();
            conn.Dispose();
        }
    }

    // 寫入 AML_CASE_DATA
    public static bool InsertAMLCASEDATAData(string tableName, DataTable dataTable)
    {
        bool result = false;
        // string connnection = System.Configuration.ConfigurationManager.ConnectionStrings["Connection_System"].ConnectionString;
        string connnection = UtilHelper.GetConnectionStrings("Connection_System");
        SqlConnection conn = new SqlConnection(connnection);
        SqlBulkCopy sbc = new SqlBulkCopy(connnection);
        sbc.DestinationTableName = tableName;

        try
        {
            conn.Open();

            sbc.ColumnMappings.Add("CASE_NO", "CASE_NO");
            sbc.ColumnMappings.Add("CUST_ID", "CUST_ID");
            sbc.ColumnMappings.Add("CUST_NAME", "CUST_NAME");
            sbc.ColumnMappings.Add("Source", "Source");
            sbc.ColumnMappings.Add("RISK_RANKING", "RISK_RANKING");
            sbc.ColumnMappings.Add("CASE_TYPE", "CASE_TYPE");
            sbc.ColumnMappings.Add("STATUS", "STATUS");
            sbc.ColumnMappings.Add("Create_YM", "Create_YM");
            sbc.ColumnMappings.Add("Create_Date", "Create_Date");
            sbc.ColumnMappings.Add("Due_Date", "Due_Date");

            sbc.WriteToServer(dataTable);

            result = true;
            return result;
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.DB);
            return result;
        }
        finally
        {
            sbc.Close();
            conn.Close();
            conn.Dispose();
        }
    }

    // 還原 AML_IMP_LOG、AML_Edata_Import、AML_Edata_Work
    public static bool RecoveryEData(string fileNameDat)
    {
        bool result = false;

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"
UPDATE [dbo].[AML_IMP_LOG] SET [Status] = 'N' WHERE [FileName] = @FileName AND [Status] = 'Y';
DELETE [dbo].[AML_Edata_Import] WHERE [FileName] = @FileName;
DELETE [dbo].[AML_Edata_Work] WHERE [FileName] = @FileName;
DELETE [dbo].[AML_CASE_DATA] WHERE [FileName] = @FileName;
";

        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@FileName", fileNameDat));

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null)
            {
                result = true;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return result;
    }



    // cdataimport
    public static bool RecoveryCdataimport()
    {
        bool result = false;

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"
DELETE [dbo].[AML_Cdata_Import];
";

        sqlcmd.CommandType = CommandType.Text;

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null)
            {
                result = true;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return result;
    }




    // delete 重複的資料
    public static bool Recoverycdataimport()
    {
        bool result = false;

        // 20210527 EOS_AML(NOVA) by Ares Dennis
        #region 退版機制
        DataTable dt2 = new DataTable();
        CSIPCommonModel.BusinessRules_new.BRM_PROPERTY_CODE.GetEnableProperty("01", new string[] { "OLD_VERSION_FLAG" }, ref dt2);
        string flag = "";
        if (dt2.Rows.Count > 0)
        {
            flag = dt2.Rows[0]["PROPERTY_CODE"].ToString();
        }
        #endregion

        SqlCommand sqlcmd = new SqlCommand();

        if (flag == "N")// 新版程式碼
        {
            #region 新版程式碼
            sqlcmd.CommandText = @"
DELETE [dbo].[AML_Cdata_Work] where customerid in (select customerid from AML_Cdata_Import);
INSERT INTO [dbo].[AML_Cdata_Work]
SELECT 
           [FileName]
           ,[Datadate]
           ,[CustomerID]
           ,[CustomerEnglishName]
           ,[CustomerChineseName]
           ,[AMLSegment]
           ,[AMLRiskRanking]
           ,[AMLNextReviewDate]
           ,[BlackListHitFlag]
           ,[PEPListHitFlag]
           ,[NNListHitFlag]
           ,[Incorporated]
           ,[IncorporatedDate]
           ,[LastUpdateMaker]
           ,[LastUpdateChecker]
           ,[LastUpdateBranch]
           ,[LastUpdateDate]
           ,[LastUpdateSourceSystem]
           ,[HomeBranch]
           ,[Reason]
           ,[WarningFlag]
           ,[FiledSAR]
           ,[CreditCardBlockCode]
           ,[InternationalOrgPEP]
           ,[DomesticPEP]
           ,[ForeignPEPStakeholder]
           ,[InternationalOrgPEPStakeholder]
           ,[DomesticPEPStakeholder]
           ,[GroupInformationSharingNameListflag]
           ,[Filler]
           ,[Create_Time]
           ,[Create_User]
           ,[Create_Date]
           ,[Dormant_Flag]
           ,[Dormant_Date]
           ,[Incorporated_Source_System]
           ,[AML_Last_Review_Date]
           ,[Risk_Factor_PEP]
           ,[Risk_Factor_RP_PEP]
           ,[Internal_List_Flag]
           ,[High_Risk_Flag_Because_Rpty]
           ,[High_Risk_Flag]
 FROM [dbo].[AML_Cdata_Import];
";
            #endregion
        }
        else
        {
            #region 舊版程式碼
            sqlcmd.CommandText = @"
DELETE [dbo].[AML_Cdata_Work] where customerid in (select customerid from AML_Cdata_Import);
INSERT INTO [dbo].[AML_Cdata_Work]
SELECT 
           [FileName]
           ,[Datadate]
           ,[CustomerID]
           ,[CustomerEnglishName]
           ,[CustomerChineseName]
           ,[AMLSegment]
           ,[AMLRiskRanking]
           ,[AMLNextReviewDate]
           ,[BlackListHitFlag]
           ,[PEPListHitFlag]
           ,[NNListHitFlag]
           ,[Incorporated]
           ,[IncorporatedDate]
           ,[LastUpdateMaker]
           ,[LastUpdateChecker]
           ,[LastUpdateBranch]
           ,[LastUpdateDate]
           ,[LastUpdateSourceSystem]
           ,[HomeBranch]
           ,[Reason]
           ,[WarningFlag]
           ,[FiledSAR]
           ,[CreditCardBlockCode]
           ,[InternationalOrgPEP]
           ,[DomesticPEP]
           ,[ForeignPEPStakeholder]
           ,[InternationalOrgPEPStakeholder]
           ,[DomesticPEPStakeholder]
           ,[GroupInformationSharingNameListflag]
           ,[Filler]
           ,[Create_Time]
           ,[Create_User]
           ,[Create_Date]
 FROM [dbo].[AML_Cdata_Import];
";
            #endregion
        }

        sqlcmd.CommandType = CommandType.Text;

        try
        {
            // 20220711 調整將執行的 SQL 包在 Transaction 裡 By Kelton
            //DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            //if (resultSet != null)
            //{
            //    result = true;
            //}
            using (OMTransactionScope ts = new OMTransactionScope())
            {
                DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }

                ts.Complete();
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return result;
    }




    // 還原 AML_IMP_LOG、AML_Edata_Import、AML_Edata_Work
    public static bool RecoveryBranchData2(string fileNameDat, string fileNameDat2)
    {
        bool result = false;

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"
        UPDATE [dbo].[AML_IMP_LOG] SET [Status] = 'N' WHERE [FileName] = @FileName AND [Status] = 'Y';
        UPDATE [dbo].[AML_IMP_LOG] SET [Status] = 'N' WHERE [FileName] = @fileNameDat2 AND [Status] = 'Y';
        ";

        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@FileName", fileNameDat));
        sqlcmd.Parameters.Add(new SqlParameter("@fileNameDat2", fileNameDat2));

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null)
            {
                result = true;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return result;
    }



    // 還原 AML_IMP_LOG、AML_Edata_Import、AML_Edata_Work
    public static bool RecoveryBranchData3(string fileNameDat, string fileNameDat2)
    {
        bool result = false;

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"
        UPDATE [dbo].[AML_IMP_LOG] SET [Status] = 'N' WHERE [FileName] = @FileName AND [Status] = 'Y';
        UPDATE [dbo].[AML_IMP_LOG] SET [Status] = 'N' WHERE [FileName] = @fileNameDat2 AND [Status] = 'Y';
        DELETE [dbo].[AML_BRCH_Import] WHERE [FileName] = @FileName;
        DELETE [dbo].[AML_BRCH_Work] WHERE [FileName] = @FileName;
        DELETE [dbo].[AML_HQ_Import] WHERE [FileName] = @fileNameDat2;
        DELETE [dbo].[AML_HQ_Manager_Import] WHERE [FileName] = @fileNameDat2;
        DELETE [dbo].[AML_HQ_Manager_Work] WHERE [FileName] = @fileNameDat2;
        DELETE [dbo].[AML_HQ_Work] WHERE [FileName] = @fileNameDat2;
        ";

        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@FileName", fileNameDat));
        sqlcmd.Parameters.Add(new SqlParameter("@fileNameDat2", fileNameDat2));

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null)
            {
                result = true;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return result;
    }



    // 還原 AML_IMP_LOG、AML_Edata_Import、AML_Edata_Work
    public static bool RecoveryEDataTemp()
    {
        bool result = false;

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"
DELETE [dbo].[AML_Edata_Temp];
";

        sqlcmd.CommandType = CommandType.Text;

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null)
            {
                result = true;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return result;
    }



    // 還原 AML_IMP_LOG、AML_Edata_Import、AML_Edata_Work
    public static bool RecoveryEDatalog(string filename)
    {
        bool result = false;

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"
UPDATE [dbo].[AML_IMP_LOG] SET [Status] = 'N' WHERE [FileName] = @FileName AND [Status] = 'Y';
";

        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@FileName", filename));

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null)
            {
                result = true;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return result;
    }


    // 還原 AML_IMP_LOG、AML_Edata_Import、AML_Edata_Work
    public static bool RecoveryCData(string fileNameDat)
    {
        bool result = false;

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"
UPDATE [dbo].[AML_IMP_LOG] SET [Status] = 'N' WHERE [FileName] = @FileName AND [Status] = 'Y';
DELETE [dbo].[AML_Cdata_Import] WHERE [FileName] = @FileName;
DELETE [dbo].[AML_Cdata_Work] WHERE [FileName] = @FileName;";
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@FileName", fileNameDat));

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null)
            {
                result = true;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return result;
    }

    // 還原 AML_IMP_LOG、AML_Edata_Import、AML_Edata_Work
    public static bool RecoveryClogData(string fileNameDat)
    {
        bool result = false;

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"
UPDATE [dbo].[AML_IMP_LOG] SET [Status] = 'N' WHERE [FileName] = @FileName AND [Status] = 'Y';"
            ;
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@FileName", fileNameDat));

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null)
            {
                result = true;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return result;
    }


    // 寫入 AML_Edata_Import、AML_Edata_Work
    public static bool InsertAMLCdataImport(string tableName, DataTable dataTable)
    {
        bool result = false;
        // string connnection = System.Configuration.ConfigurationManager.ConnectionStrings["Connection_System"].ConnectionString;
        string connnection = UtilHelper.GetConnectionStrings("Connection_System");
        SqlConnection conn = new SqlConnection(connnection);
        SqlBulkCopy sbc = new SqlBulkCopy(connnection);
        sbc.DestinationTableName = tableName;

        try
        {
            conn.Open();

            sbc.ColumnMappings.Add("FileName", "FileName");
            sbc.ColumnMappings.Add("Datadate", "Datadate");
            sbc.ColumnMappings.Add("CustomerID", "CustomerID");
            sbc.ColumnMappings.Add("CustomerEnglishName", "CustomerEnglishName");
            sbc.ColumnMappings.Add("CustomerChineseName", "CustomerChineseName");
            sbc.ColumnMappings.Add("AMLSegment", "AMLSegment");
            sbc.ColumnMappings.Add("AMLRiskRanking", "AMLRiskRanking");
            sbc.ColumnMappings.Add("AMLNextReviewDate", "AMLNextReviewDate");
            sbc.ColumnMappings.Add("BlackListHitFlag", "BlackListHitFlag");
            sbc.ColumnMappings.Add("PEPListHitFlag", "PEPListHitFlag");
            sbc.ColumnMappings.Add("NNListHitFlag", "NNListHitFlag");
            sbc.ColumnMappings.Add("Incorporated", "Incorporated");
            sbc.ColumnMappings.Add("IncorporatedDate", "IncorporatedDate");
            sbc.ColumnMappings.Add("LastUpdateMaker", "LastUpdateMaker");
            sbc.ColumnMappings.Add("LastUpdateChecker", "LastUpdateChecker");
            sbc.ColumnMappings.Add("LastUpdateBranch", "LastUpdateBranch");
            sbc.ColumnMappings.Add("LastUpdateDate", "LastUpdateDate");
            sbc.ColumnMappings.Add("LastUpdateSourceSystem", "LastUpdateSourceSystem");
            sbc.ColumnMappings.Add("HomeBranch", "HomeBranch");
            sbc.ColumnMappings.Add("Reason", "Reason");
            sbc.ColumnMappings.Add("WarningFlag", "WarningFlag");
            sbc.ColumnMappings.Add("FiledSAR", "FiledSAR");
            sbc.ColumnMappings.Add("CreditCardBlockCode", "CreditCardBlockCode");
            sbc.ColumnMappings.Add("InternationalOrgPEP", "InternationalOrgPEP");
            sbc.ColumnMappings.Add("DomesticPEP", "DomesticPEP");
            sbc.ColumnMappings.Add("ForeignPEPStakeholder", "ForeignPEPStakeholder");
            sbc.ColumnMappings.Add("InternationalOrgPEPStakeholder", "InternationalOrgPEPStakeholder");
            sbc.ColumnMappings.Add("DomesticPEPStakeholder", "DomesticPEPStakeholder");
            sbc.ColumnMappings.Add("GroupInformationSharingNameListflag", "GroupInformationSharingNameListflag");
            sbc.ColumnMappings.Add("Filler", "Filler");
            sbc.ColumnMappings.Add("Create_Time", "Create_Time");
            sbc.ColumnMappings.Add("Create_User", "Create_User");
            sbc.ColumnMappings.Add("Create_Date", "Create_Date");


            sbc.WriteToServer(dataTable);

            result = true;
            return result;
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.DB);
            return result;
        }
        finally
        {
            sbc.Close();
            conn.Close();
            conn.Dispose();
        }
    }




    // 寫入 AML_BRCH_Import、AML_BRCH_Work
    public static bool InsertAMLBranchdataImport(string tableName, DataTable dataTable)
    {
        bool result = false;
        // string connnection = System.Configuration.ConfigurationManager.ConnectionStrings["Connection_System"].ConnectionString;
        string connnection = UtilHelper.GetConnectionStrings("Connection_System");
        SqlConnection conn = new SqlConnection(connnection);
        SqlBulkCopy sbc = new SqlBulkCopy(connnection);
        sbc.DestinationTableName = tableName;

        try
        {
            conn.Open();

            sbc.ColumnMappings.Add("FileName", "FileName");
            sbc.ColumnMappings.Add("BRCH_BATCH_NO", "BRCH_BATCH_NO");
            sbc.ColumnMappings.Add("BRCH_INTER_ID", "BRCH_INTER_ID");
            sbc.ColumnMappings.Add("BRCH_SIXM_TOT_AMT", "BRCH_SIXM_TOT_AMT");

            sbc.ColumnMappings.Add("BRCH_MON_AMT1", "BRCH_MON_AMT1");
            sbc.ColumnMappings.Add("BRCH_MON_AMT2", "BRCH_MON_AMT2");
            sbc.ColumnMappings.Add("BRCH_MON_AMT3", "BRCH_MON_AMT3");
            sbc.ColumnMappings.Add("BRCH_MON_AMT4", "BRCH_MON_AMT4");
            sbc.ColumnMappings.Add("BRCH_MON_AMT5", "BRCH_MON_AMT5");
            sbc.ColumnMappings.Add("BRCH_MON_AMT6", "BRCH_MON_AMT6");
            sbc.ColumnMappings.Add("BRCH_MON_AMT7", "BRCH_MON_AMT7");
            sbc.ColumnMappings.Add("BRCH_MON_AMT8", "BRCH_MON_AMT8");
            sbc.ColumnMappings.Add("BRCH_MON_AMT9", "BRCH_MON_AMT9");
            sbc.ColumnMappings.Add("BRCH_MON_AMT10", "BRCH_MON_AMT10");
            sbc.ColumnMappings.Add("BRCH_MON_AMT11", "BRCH_MON_AMT11");
            sbc.ColumnMappings.Add("BRCH_MON_AMT12", "BRCH_MON_AMT12");

            sbc.ColumnMappings.Add("BRCH_KEY", "BRCH_KEY");
            sbc.ColumnMappings.Add("BRCH_BRCH_NO", "BRCH_BRCH_NO");
            sbc.ColumnMappings.Add("BRCH_BRCH_SEQ", "BRCH_BRCH_SEQ");
            sbc.ColumnMappings.Add("BRCH_BRCH_TYPE", "BRCH_BRCH_TYPE");
            sbc.ColumnMappings.Add("BRCH_NATION", "BRCH_NATION");
            sbc.ColumnMappings.Add("BRCH_BIRTH_DATE", "BRCH_BIRTH_DATE");
            sbc.ColumnMappings.Add("BRCH_PERM_CITY", "BRCH_PERM_CITY");
            sbc.ColumnMappings.Add("BRCH_PERM_ADDR1", "BRCH_PERM_ADDR1");
            sbc.ColumnMappings.Add("BRCH_PERM_ADDR2", "BRCH_PERM_ADDR2");
            sbc.ColumnMappings.Add("BRCH_CHINESE_NAME", "BRCH_CHINESE_NAME");
            sbc.ColumnMappings.Add("BRCH_ENGLISH_NAME", "BRCH_ENGLISH_NAME");
            sbc.ColumnMappings.Add("BRCH_ID", "BRCH_ID");
            sbc.ColumnMappings.Add("BRCH_OWNER_ID_ISSUE_DATE", "BRCH_OWNER_ID_ISSUE_DATE");
            sbc.ColumnMappings.Add("BRCH_OWNER_ID_ISSUE_PLACE", "BRCH_OWNER_ID_ISSUE_PLACE");
            sbc.ColumnMappings.Add("BRCH_OWNER_ID_REPLACE_TYPE", "BRCH_OWNER_ID_REPLACE_TYPE");
            sbc.ColumnMappings.Add("BRCH_ID_PHOTO_FLAG", "BRCH_ID_PHOTO_FLAG");
            sbc.ColumnMappings.Add("BRCH_PASSPORT", "BRCH_PASSPORT");
            sbc.ColumnMappings.Add("BRCH_PASSPORT_EXP_DATE", "BRCH_PASSPORT_EXP_DATE");
            sbc.ColumnMappings.Add("BRCH_RESIDENT_NO", "BRCH_RESIDENT_NO");
            sbc.ColumnMappings.Add("BRCH_RESIDENT_EXP_DATE", "BRCH_RESIDENT_EXP_DATE");
            sbc.ColumnMappings.Add("BRCH_OTHER_CERT", "BRCH_OTHER_CERT");
            sbc.ColumnMappings.Add("BRCH_OTHER_CERT_EXP_DATE", "BRCH_OTHER_CERT_EXP_DATE");
            sbc.ColumnMappings.Add("BRCH_COMP_TEL", "BRCH_COMP_TEL");
            sbc.ColumnMappings.Add("BRCH_CREATE_DATE", "BRCH_CREATE_DATE");
            sbc.ColumnMappings.Add("BRCH_STATUS", "BRCH_STATUS");
            sbc.ColumnMappings.Add("BRCH_CIRCULATE_MERCH", "BRCH_CIRCULATE_MERCH");
            sbc.ColumnMappings.Add("BRCH_HQ_BRCH_NO", "BRCH_HQ_BRCH_NO");
            sbc.ColumnMappings.Add("BRCH_HQ_BRCH_SEQ_NO", "BRCH_HQ_BRCH_SEQ_NO");
            sbc.ColumnMappings.Add("BRCH_UPDATE_DATE", "BRCH_UPDATE_DATE");
            sbc.ColumnMappings.Add("BRCH_QUALIFY_FLAG", "BRCH_QUALIFY_FLAG");
            sbc.ColumnMappings.Add("BRCH_UPDATE_ID", "BRCH_UPDATE_ID");
            sbc.ColumnMappings.Add("BRCH_REAL_CORP", "BRCH_REAL_CORP");
            sbc.ColumnMappings.Add("BRCH_RESERVED_FILLER", "BRCH_RESERVED_FILLER");
            sbc.ColumnMappings.Add("Create_Time", "Create_Time");
            sbc.ColumnMappings.Add("Create_User", "Create_User");
            sbc.ColumnMappings.Add("Create_Date", "Create_Date");

            //長姓名
            sbc.ColumnMappings.Add("BRCH_CHINESE_LNAME", "BRCH_CHINESE_LNAME");
            sbc.ColumnMappings.Add("BRCH_ROMA", "BRCH_ROMA");

            //20201029-202012RC 增加設立日期欄位
            sbc.ColumnMappings.Add("BRCH_BUILD_DATE", "BRCH_BUILD_DATE");

            //20210804 EOS_AML(NOVA) 自然人收單批次 by Ares Dennis
            sbc.ColumnMappings.Add("BRCH_NATU_ID", "BRCH_NATU_ID");//自然人ID
            sbc.ColumnMappings.Add("LAST_UPD_MAKER", "LAST_UPD_MAKER");//資料最後異動MAKER
            sbc.ColumnMappings.Add("LAST_UPD_CHECKER", "LAST_UPD_CHECKER");//資料最後異動CHECKER
            sbc.ColumnMappings.Add("LAST_UPD_BRANCH", "LAST_UPD_BRANCH");//資料最後異動分行
            sbc.ColumnMappings.Add("LAST_UPD_DATE", "LAST_UPD_DATE");//資料最後異動日期            

            //20211007_Ares_Jack_新增欄位
            sbc.ColumnMappings.Add("BRCH_REG_ENG_NAME", "BRCH_REG_ENG_NAME");//分公司登記英文
            sbc.ColumnMappings.Add("BRCH_REG_CHI_NAME", "BRCH_REG_CHI_NAME");//分公司登記中文  

            sbc.WriteToServer(dataTable);

            result = true;
            return result;
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.DB);
            return result;
        }
        finally
        {
            sbc.Close();
            conn.Close();
            conn.Dispose();
        }
    }
    

    // 寫入 AML_HQ_Import
    public static bool InsertAMLMasterdataImport(string tableName, DataTable dataTable)
    {
        bool result = false;
        // string connnection = System.Configuration.ConfigurationManager.ConnectionStrings["Connection_System"].ConnectionString;
        string connnection = UtilHelper.GetConnectionStrings("Connection_System");
        SqlConnection conn = new SqlConnection(connnection);
        SqlBulkCopy sbc = new SqlBulkCopy(connnection);
        sbc.DestinationTableName = tableName;

        try
        {
            conn.Open();

            sbc.ColumnMappings.Add("FileName", "FileName");
            sbc.ColumnMappings.Add("HCOP_BATCH_NO", "HCOP_BATCH_NO");
            sbc.ColumnMappings.Add("HCOP_INTER_ID", "HCOP_INTER_ID");
            sbc.ColumnMappings.Add("HCOP_SIXM_TOT_AMT", "HCOP_SIXM_TOT_AMT");
            sbc.ColumnMappings.Add("HCOP_MON_AMT1", "HCOP_MON_AMT1");
            sbc.ColumnMappings.Add("HCOP_MON_AMT2", "HCOP_MON_AMT2");
            sbc.ColumnMappings.Add("HCOP_MON_AMT3", "HCOP_MON_AMT3");
            sbc.ColumnMappings.Add("HCOP_MON_AMT4", "HCOP_MON_AMT4");
            sbc.ColumnMappings.Add("HCOP_MON_AMT5", "HCOP_MON_AMT5");
            sbc.ColumnMappings.Add("HCOP_MON_AMT6", "HCOP_MON_AMT6");
            sbc.ColumnMappings.Add("HCOP_MON_AMT7", "HCOP_MON_AMT7");
            sbc.ColumnMappings.Add("HCOP_MON_AMT8", "HCOP_MON_AMT8");
            sbc.ColumnMappings.Add("HCOP_MON_AMT9", "HCOP_MON_AMT9");
            sbc.ColumnMappings.Add("HCOP_MON_AMT10", "HCOP_MON_AMT10");
            sbc.ColumnMappings.Add("HCOP_MON_AMT11", "HCOP_MON_AMT11");
            sbc.ColumnMappings.Add("HCOP_MON_AMT12", "HCOP_MON_AMT12");
            sbc.ColumnMappings.Add("HCOP_KEY", "HCOP_KEY");
            sbc.ColumnMappings.Add("HCOP_HEADQUATERS_CORP_NO", "HCOP_HEADQUATERS_CORP_NO");
            sbc.ColumnMappings.Add("HCOP_HEADQUATERS_CORP_SEQ", "HCOP_HEADQUATERS_CORP_SEQ");
            sbc.ColumnMappings.Add("HCOP_CORP_TYPE", "HCOP_CORP_TYPE");
            sbc.ColumnMappings.Add("HCOP_REGISTER_NATION", "HCOP_REGISTER_NATION");
            sbc.ColumnMappings.Add("HCOP_CORP_REG_ENG_NAME", "HCOP_CORP_REG_ENG_NAME");
            sbc.ColumnMappings.Add("HCOP_REG_NAME", "HCOP_REG_NAME");
            sbc.ColumnMappings.Add("HCOP_NAME_0E", "HCOP_NAME_0E");
            sbc.ColumnMappings.Add("HCOP_NAME_CHI", "HCOP_NAME_CHI");
            sbc.ColumnMappings.Add("HCOP_NAME_0F", "HCOP_NAME_0F");
            sbc.ColumnMappings.Add("HCOP_BUILD_DATE", "HCOP_BUILD_DATE");
            sbc.ColumnMappings.Add("HCOP_CC", "HCOP_CC");
            sbc.ColumnMappings.Add("HCOP_REG_CITY", "HCOP_REG_CITY");
            sbc.ColumnMappings.Add("HCOP_REG_ADDR1", "HCOP_REG_ADDR1");
            sbc.ColumnMappings.Add("HCOP_REG_ADDR2", "HCOP_REG_ADDR2");
            sbc.ColumnMappings.Add("HCOP_EMAIL", "HCOP_EMAIL");
            sbc.ColumnMappings.Add("HCOP_NP_COMPANY_NAME", "HCOP_NP_COMPANY_NAME");
            sbc.ColumnMappings.Add("HCOP_OWNER_NATION", "HCOP_OWNER_NATION");
            sbc.ColumnMappings.Add("HCOP_OWNER_CHINESE_NAME", "HCOP_OWNER_CHINESE_NAME");
            sbc.ColumnMappings.Add("HCOP_OWNER_ENGLISH_NAME", "HCOP_OWNER_ENGLISH_NAME");
            sbc.ColumnMappings.Add("HCOP_OWNER_BIRTH_DATE", "HCOP_OWNER_BIRTH_DATE");
            sbc.ColumnMappings.Add("HCOP_OWNER_ID", "HCOP_OWNER_ID");
            sbc.ColumnMappings.Add("HCOP_OWNER_ID_ISSUE_DATE", "HCOP_OWNER_ID_ISSUE_DATE");
            sbc.ColumnMappings.Add("HCOP_OWNER_ID_ISSUE_PLACE", "HCOP_OWNER_ID_ISSUE_PLACE");
            sbc.ColumnMappings.Add("HCOP_OWNER_ID_REPLACE_TYPE", "HCOP_OWNER_ID_REPLACE_TYPE");
            sbc.ColumnMappings.Add("HCOP_ID_PHOTO_FLAG", "HCOP_ID_PHOTO_FLAG");
            sbc.ColumnMappings.Add("HCOP_PASSPORT", "HCOP_PASSPORT");
            sbc.ColumnMappings.Add("HCOP_PASSPORT_EXP_DATE", "HCOP_PASSPORT_EXP_DATE");
            sbc.ColumnMappings.Add("HCOP_RESIDENT_NO", "HCOP_RESIDENT_NO");
            sbc.ColumnMappings.Add("HCOP_RESIDENT_EXP_DATE", "HCOP_RESIDENT_EXP_DATE");
            sbc.ColumnMappings.Add("HCOP_OTHER_CERT", "HCOP_OTHER_CERT");
            sbc.ColumnMappings.Add("HCOP_OTHER_CERT_EXP_DATE", "HCOP_OTHER_CERT_EXP_DATE");
            sbc.ColumnMappings.Add("HCOP_COMPLEX_STR_CODE", "HCOP_COMPLEX_STR_CODE");
            sbc.ColumnMappings.Add("HCOP_ISSUE_STOCK_FLAG", "HCOP_ISSUE_STOCK_FLAG");
            sbc.ColumnMappings.Add("HCOP_COMP_TEL", "HCOP_COMP_TEL");
            sbc.ColumnMappings.Add("HCOP_MAILING_CITY", "HCOP_MAILING_CITY");
            sbc.ColumnMappings.Add("HCOP_MAILING_ADDR1", "HCOP_MAILING_ADDR1");
            sbc.ColumnMappings.Add("HCOP_MAILING_ADDR2", "HCOP_MAILING_ADDR2");
            sbc.ColumnMappings.Add("HCOP_PRIMARY_BUSI_COUNTRY", "HCOP_PRIMARY_BUSI_COUNTRY");
            sbc.ColumnMappings.Add("HCOP_BUSI_RISK_NATION_FLAG", "HCOP_BUSI_RISK_NATION_FLAG");
            sbc.ColumnMappings.Add("HCOP_BUSI_RISK_NATION_1", "HCOP_BUSI_RISK_NATION_1");
            sbc.ColumnMappings.Add("HCOP_BUSI_RISK_NATION_2", "HCOP_BUSI_RISK_NATION_2");
            sbc.ColumnMappings.Add("HCOP_BUSI_RISK_NATION_3", "HCOP_BUSI_RISK_NATION_3");
            sbc.ColumnMappings.Add("HCOP_BUSI_RISK_NATION_4", "HCOP_BUSI_RISK_NATION_4");
            sbc.ColumnMappings.Add("HCOP_BUSI_RISK_NATION_5", "HCOP_BUSI_RISK_NATION_5");
            sbc.ColumnMappings.Add("HCOP_OVERSEAS_FOREIGN", "HCOP_OVERSEAS_FOREIGN");
            sbc.ColumnMappings.Add("HCOP_OVERSEAS_FOREIGN_COUNTRY", "HCOP_OVERSEAS_FOREIGN_COUNTRY");
            sbc.ColumnMappings.Add("HCOP_REGISTER_US_STATE", "HCOP_REGISTER_US_STATE");
            sbc.ColumnMappings.Add("HCOP_BUSINESS_ORGAN_TYPE", "HCOP_BUSINESS_ORGAN_TYPE");
            sbc.ColumnMappings.Add("HCOP_CREATE_DATE", "HCOP_CREATE_DATE");
            sbc.ColumnMappings.Add("HCOP_STATUS", "HCOP_STATUS");
            sbc.ColumnMappings.Add("HCOP_QUALIFY_FLAG", "HCOP_QUALIFY_FLAG");
            sbc.ColumnMappings.Add("HCOP_CONTACT_NAME", "HCOP_CONTACT_NAME");
            sbc.ColumnMappings.Add("HCOP_EXAMINE_FLAG", "HCOP_EXAMINE_FLAG");
            sbc.ColumnMappings.Add("HCOP_ALLOW_ISSUE_STOCK_FLAG", "HCOP_ALLOW_ISSUE_STOCK_FLAG");
            sbc.ColumnMappings.Add("HCOP_CONTACT_TEL", "HCOP_CONTACT_TEL");
            sbc.ColumnMappings.Add("HCOP_UPDATE_DATE", "HCOP_UPDATE_DATE");
            sbc.ColumnMappings.Add("HCOP_CREATE_ID", "HCOP_CREATE_ID");
            sbc.ColumnMappings.Add("HCOP_UPDATE_ID", "HCOP_UPDATE_ID");
            sbc.ColumnMappings.Add("HCOP_OWNER_CITY", "HCOP_OWNER_CITY");
            sbc.ColumnMappings.Add("HCOP_OWNER_ADDR1", "HCOP_OWNER_ADDR1");
            sbc.ColumnMappings.Add("HCOP_OWNER_ADDR2", "HCOP_OWNER_ADDR2");
            sbc.ColumnMappings.Add("HCOP_RESERVED_FILLER", "HCOP_RESERVED_FILLER");
            sbc.ColumnMappings.Add("Create_Time", "Create_Time");
            sbc.ColumnMappings.Add("Create_User", "Create_User");
            sbc.ColumnMappings.Add("Create_Date", "Create_Date");
            
            //長姓名
            sbc.ColumnMappings.Add("HCOP_OWNER_CHINESE_LNAME", "HCOP_OWNER_CHINESE_LNAME");
            sbc.ColumnMappings.Add("HCOP_OWNER_ROMA", "HCOP_OWNER_ROMA");
            sbc.ColumnMappings.Add("HCOP_CONTACT_LNAME", "HCOP_CONTACT_LNAME");
            sbc.ColumnMappings.Add("HCOP_CONTACT_ROMA", "HCOP_CONTACT_ROMA");

            //20210804 EOS_AML(NOVA) 自然人收單批次 by Ares Dennis
            sbc.ColumnMappings.Add("HCOP_CC_2", "HCOP_CC_2");//行業別2
            sbc.ColumnMappings.Add("HCOP_CC_3", "HCOP_CC_3");//行業別3
            sbc.ColumnMappings.Add("HCOP_OC", "HCOP_OC");//職業別
            sbc.ColumnMappings.Add("HCOP_INCOME_SOURCE", "HCOP_INCOME_SOURCE");//主要收入及資產來源
            sbc.ColumnMappings.Add("HCOP_LAST_UPD_MAKER", "HCOP_LAST_UPD_MAKER");//資料最後異動MAKER
            sbc.ColumnMappings.Add("HCOP_LAST_UPD_CHECKER", "HCOP_LAST_UPD_CHECKER");//資料最後異動CHECKER
            sbc.ColumnMappings.Add("HCOP_LAST_UPD_BRANCH", "HCOP_LAST_UPD_BRANCH");//資料最後異動分行
            sbc.ColumnMappings.Add("HCOP_LAST_UPDATE_DATE", "HCOP_LAST_UPDATE_DATE");//資料最後異動日期
            sbc.ColumnMappings.Add("HCOP_COUNTRY_CODE_2", "HCOP_COUNTRY_CODE_2");//國籍2
            sbc.ColumnMappings.Add("HCOP_GENDER", "HCOP_GENDER");//性別
            sbc.ColumnMappings.Add("HCOP_REG_ZIP_CODE", "HCOP_REG_ZIP_CODE");//郵遞區號            

            //20211207_Ares_Jack_新增欄位
            sbc.ColumnMappings.Add("HCOP_MOBILE", "HCOP_MOBILE");//行對電話
            sbc.WriteToServer(dataTable);

            result = true;
            return result;
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.DB);
            return result;
        }
        finally
        {
            sbc.Close();
            conn.Close();
            conn.Dispose();
        }
    }
    

    // 寫入 AML_HQ_Work
    public static bool InsertAMLMasterdataImportWork(string tableName, DataTable dataTable)
    {
        bool result = false;
        // string connnection = System.Configuration.ConfigurationManager.ConnectionStrings["Connection_System"].ConnectionString;
        string connnection = UtilHelper.GetConnectionStrings("Connection_System");
        SqlConnection conn = new SqlConnection(connnection);
        SqlBulkCopy sbc = new SqlBulkCopy(connnection);
        sbc.DestinationTableName = tableName;

        try
        {
            conn.Open();

            sbc.ColumnMappings.Add("FileName", "FileName");
            sbc.ColumnMappings.Add("HCOP_BATCH_NO", "HCOP_BATCH_NO");
            sbc.ColumnMappings.Add("HCOP_INTER_ID", "HCOP_INTER_ID");
            sbc.ColumnMappings.Add("HCOP_SIXM_TOT_AMT", "HCOP_SIXM_TOT_AMT");
            sbc.ColumnMappings.Add("HCOP_MON_AMT1", "HCOP_MON_AMT1");
            sbc.ColumnMappings.Add("HCOP_MON_AMT2", "HCOP_MON_AMT2");
            sbc.ColumnMappings.Add("HCOP_MON_AMT3", "HCOP_MON_AMT3");
            sbc.ColumnMappings.Add("HCOP_MON_AMT4", "HCOP_MON_AMT4");
            sbc.ColumnMappings.Add("HCOP_MON_AMT5", "HCOP_MON_AMT5");
            sbc.ColumnMappings.Add("HCOP_MON_AMT6", "HCOP_MON_AMT6");
            sbc.ColumnMappings.Add("HCOP_MON_AMT7", "HCOP_MON_AMT7");
            sbc.ColumnMappings.Add("HCOP_MON_AMT8", "HCOP_MON_AMT8");
            sbc.ColumnMappings.Add("HCOP_MON_AMT9", "HCOP_MON_AMT9");
            sbc.ColumnMappings.Add("HCOP_MON_AMT10", "HCOP_MON_AMT10");
            sbc.ColumnMappings.Add("HCOP_MON_AMT11", "HCOP_MON_AMT11");
            sbc.ColumnMappings.Add("HCOP_MON_AMT12", "HCOP_MON_AMT12");
            sbc.ColumnMappings.Add("HCOP_KEY", "HCOP_KEY");
            sbc.ColumnMappings.Add("HCOP_HEADQUATERS_CORP_NO", "HCOP_HEADQUATERS_CORP_NO");
            sbc.ColumnMappings.Add("HCOP_HEADQUATERS_CORP_SEQ", "HCOP_HEADQUATERS_CORP_SEQ");
            sbc.ColumnMappings.Add("HCOP_CORP_TYPE", "HCOP_CORP_TYPE");
            sbc.ColumnMappings.Add("HCOP_REGISTER_NATION", "HCOP_REGISTER_NATION");
            sbc.ColumnMappings.Add("HCOP_CORP_REG_ENG_NAME", "HCOP_CORP_REG_ENG_NAME");
            sbc.ColumnMappings.Add("HCOP_REG_NAME", "HCOP_REG_NAME");
            sbc.ColumnMappings.Add("HCOP_NAME_0E", "HCOP_NAME_0E");
            sbc.ColumnMappings.Add("HCOP_NAME_CHI", "HCOP_NAME_CHI");
            sbc.ColumnMappings.Add("HCOP_NAME_0F", "HCOP_NAME_0F");
            sbc.ColumnMappings.Add("HCOP_BUILD_DATE", "HCOP_BUILD_DATE");
            sbc.ColumnMappings.Add("HCOP_CC", "HCOP_CC");
            sbc.ColumnMappings.Add("HCOP_REG_CITY", "HCOP_REG_CITY");
            sbc.ColumnMappings.Add("HCOP_REG_ADDR1", "HCOP_REG_ADDR1");
            sbc.ColumnMappings.Add("HCOP_REG_ADDR2", "HCOP_REG_ADDR2");
            sbc.ColumnMappings.Add("HCOP_EMAIL", "HCOP_EMAIL");
            sbc.ColumnMappings.Add("HCOP_NP_COMPANY_NAME", "HCOP_NP_COMPANY_NAME");
            sbc.ColumnMappings.Add("HCOP_OWNER_NATION", "HCOP_OWNER_NATION");
            sbc.ColumnMappings.Add("HCOP_OWNER_CHINESE_NAME", "HCOP_OWNER_CHINESE_NAME");
            sbc.ColumnMappings.Add("HCOP_OWNER_ENGLISH_NAME", "HCOP_OWNER_ENGLISH_NAME");
            sbc.ColumnMappings.Add("HCOP_OWNER_BIRTH_DATE", "HCOP_OWNER_BIRTH_DATE");
            sbc.ColumnMappings.Add("HCOP_OWNER_ID", "HCOP_OWNER_ID");
            sbc.ColumnMappings.Add("HCOP_OWNER_ID_ISSUE_DATE", "HCOP_OWNER_ID_ISSUE_DATE");
            sbc.ColumnMappings.Add("HCOP_OWNER_ID_ISSUE_PLACE", "HCOP_OWNER_ID_ISSUE_PLACE");
            sbc.ColumnMappings.Add("HCOP_OWNER_ID_REPLACE_TYPE", "HCOP_OWNER_ID_REPLACE_TYPE");
            sbc.ColumnMappings.Add("HCOP_ID_PHOTO_FLAG", "HCOP_ID_PHOTO_FLAG");
            sbc.ColumnMappings.Add("HCOP_PASSPORT", "HCOP_PASSPORT");
            sbc.ColumnMappings.Add("HCOP_PASSPORT_EXP_DATE", "HCOP_PASSPORT_EXP_DATE");
            sbc.ColumnMappings.Add("HCOP_RESIDENT_NO", "HCOP_RESIDENT_NO");
            sbc.ColumnMappings.Add("HCOP_RESIDENT_EXP_DATE", "HCOP_RESIDENT_EXP_DATE");
            sbc.ColumnMappings.Add("HCOP_OTHER_CERT", "HCOP_OTHER_CERT");
            sbc.ColumnMappings.Add("HCOP_OTHER_CERT_EXP_DATE", "HCOP_OTHER_CERT_EXP_DATE");
            sbc.ColumnMappings.Add("HCOP_COMPLEX_STR_CODE", "HCOP_COMPLEX_STR_CODE");
            sbc.ColumnMappings.Add("HCOP_ISSUE_STOCK_FLAG", "HCOP_ISSUE_STOCK_FLAG");
            sbc.ColumnMappings.Add("HCOP_COMP_TEL", "HCOP_COMP_TEL");
            sbc.ColumnMappings.Add("HCOP_MAILING_CITY", "HCOP_MAILING_CITY");
            sbc.ColumnMappings.Add("HCOP_MAILING_ADDR1", "HCOP_MAILING_ADDR1");
            sbc.ColumnMappings.Add("HCOP_MAILING_ADDR2", "HCOP_MAILING_ADDR2");
            sbc.ColumnMappings.Add("HCOP_PRIMARY_BUSI_COUNTRY", "HCOP_PRIMARY_BUSI_COUNTRY");
            sbc.ColumnMappings.Add("HCOP_BUSI_RISK_NATION_FLAG", "HCOP_BUSI_RISK_NATION_FLAG");
            sbc.ColumnMappings.Add("HCOP_BUSI_RISK_NATION_1", "HCOP_BUSI_RISK_NATION_1");
            sbc.ColumnMappings.Add("HCOP_BUSI_RISK_NATION_2", "HCOP_BUSI_RISK_NATION_2");
            sbc.ColumnMappings.Add("HCOP_BUSI_RISK_NATION_3", "HCOP_BUSI_RISK_NATION_3");
            sbc.ColumnMappings.Add("HCOP_BUSI_RISK_NATION_4", "HCOP_BUSI_RISK_NATION_4");
            sbc.ColumnMappings.Add("HCOP_BUSI_RISK_NATION_5", "HCOP_BUSI_RISK_NATION_5");
            sbc.ColumnMappings.Add("HCOP_OVERSEAS_FOREIGN", "HCOP_OVERSEAS_FOREIGN");
            sbc.ColumnMappings.Add("HCOP_OVERSEAS_FOREIGN_COUNTRY", "HCOP_OVERSEAS_FOREIGN_COUNTRY");
            sbc.ColumnMappings.Add("HCOP_REGISTER_US_STATE", "HCOP_REGISTER_US_STATE");
            sbc.ColumnMappings.Add("HCOP_BUSINESS_ORGAN_TYPE", "HCOP_BUSINESS_ORGAN_TYPE");
            sbc.ColumnMappings.Add("HCOP_CREATE_DATE", "HCOP_CREATE_DATE");
            sbc.ColumnMappings.Add("HCOP_STATUS", "HCOP_STATUS");
            sbc.ColumnMappings.Add("HCOP_QUALIFY_FLAG", "HCOP_QUALIFY_FLAG");
            sbc.ColumnMappings.Add("HCOP_CONTACT_NAME", "HCOP_CONTACT_NAME");
            sbc.ColumnMappings.Add("HCOP_EXAMINE_FLAG", "HCOP_EXAMINE_FLAG");
            sbc.ColumnMappings.Add("HCOP_ALLOW_ISSUE_STOCK_FLAG", "HCOP_ALLOW_ISSUE_STOCK_FLAG");
            sbc.ColumnMappings.Add("HCOP_CONTACT_TEL", "HCOP_CONTACT_TEL");
            sbc.ColumnMappings.Add("HCOP_UPDATE_DATE", "HCOP_UPDATE_DATE");
            sbc.ColumnMappings.Add("HCOP_CREATE_ID", "HCOP_CREATE_ID");
            sbc.ColumnMappings.Add("HCOP_UPDATE_ID", "HCOP_UPDATE_ID");
            sbc.ColumnMappings.Add("HCOP_OWNER_CITY", "HCOP_OWNER_CITY");
            sbc.ColumnMappings.Add("HCOP_OWNER_ADDR1", "HCOP_OWNER_ADDR1");
            sbc.ColumnMappings.Add("HCOP_OWNER_ADDR2", "HCOP_OWNER_ADDR2");
            sbc.ColumnMappings.Add("HCOP_RESERVED_FILLER", "HCOP_RESERVED_FILLER");
            sbc.ColumnMappings.Add("Create_Time", "Create_Time");
            sbc.ColumnMappings.Add("Create_User", "Create_User");
            sbc.ColumnMappings.Add("Create_Date", "Create_Date");
            sbc.ColumnMappings.Add("CaseProcess_User", "CaseProcess_User");
            sbc.ColumnMappings.Add("CaseProcess_Status", "CaseProcess_Status");
            sbc.ColumnMappings.Add("CaseOwner_User", "CaseOwner_User");

            //長姓名
            sbc.ColumnMappings.Add("HCOP_OWNER_CHINESE_LNAME", "HCOP_OWNER_CHINESE_LNAME");
            sbc.ColumnMappings.Add("HCOP_OWNER_ROMA", "HCOP_OWNER_ROMA");
            sbc.ColumnMappings.Add("HCOP_CONTACT_LNAME", "HCOP_CONTACT_LNAME");
            sbc.ColumnMappings.Add("HCOP_CONTACT_ROMA", "HCOP_CONTACT_ROMA");

            //20210804 EOS_AML(NOVA) 自然人收單批次 by Ares Dennis
            sbc.ColumnMappings.Add("HCOP_CC_2", "HCOP_CC_2");//行業別2
            sbc.ColumnMappings.Add("HCOP_CC_3", "HCOP_CC_3");//行業別3
            sbc.ColumnMappings.Add("HCOP_OC", "HCOP_OC");//職業別
            sbc.ColumnMappings.Add("HCOP_INCOME_SOURCE", "HCOP_INCOME_SOURCE");//主要收入及資產來源
            sbc.ColumnMappings.Add("HCOP_LAST_UPD_MAKER", "HCOP_LAST_UPD_MAKER");//資料最後異動MAKER
            sbc.ColumnMappings.Add("HCOP_LAST_UPD_CHECKER", "HCOP_LAST_UPD_CHECKER");//資料最後異動CHECKER
            sbc.ColumnMappings.Add("HCOP_LAST_UPD_BRANCH", "HCOP_LAST_UPD_BRANCH");//資料最後異動分行
            sbc.ColumnMappings.Add("HCOP_LAST_UPDATE_DATE", "HCOP_LAST_UPDATE_DATE");//資料最後異動日期
            sbc.ColumnMappings.Add("HCOP_COUNTRY_CODE_2", "HCOP_COUNTRY_CODE_2");//國籍2
            sbc.ColumnMappings.Add("HCOP_GENDER", "HCOP_GENDER");//性別
            sbc.ColumnMappings.Add("HCOP_REG_ZIP_CODE", "HCOP_REG_ZIP_CODE");//郵遞區號    

            //20211209_Ares_Jack_新增欄位
            sbc.ColumnMappings.Add("HCOP_MOBILE", "HCOP_MOBILE");//行對電話

            sbc.WriteToServer(dataTable);

            result = true;
            return result;
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.DB);
            return result;
        }
        finally
        {
            sbc.Close();
            conn.Close();
            conn.Dispose();
        }
    }


    // 還原 AML_IMP_LOG、AML_Edata_Import、AML_Edata_Work
    public static bool RecoveryBranchData(string fileNameDat)
    {
        bool result = false;

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"
UPDATE [dbo].[AML_IMP_LOG] SET [Status] = 'N' WHERE [FileName] = @FileName AND [Status] = 'Y';
DELETE [dbo].[AML_BRCH_Import] WHERE [FileName] = @FileName;
DELETE [dbo].[AML_BRCH_Work] WHERE [FileName] = @FileName;
DELETE [dbo].[AML_HQ_Import] WHERE [FileName] = @FileName;
DELETE [dbo].[AML_HQ_Work] WHERE [FileName] = @FileName;
DELETE [dbo].[AML_HQ_Manager_Work] WHERE [FileName] = @FileName;
DELETE [dbo].[AML_HQ_Manager_Import] WHERE [FileName] = @FileName;";            
            
;


        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@FileName", fileNameDat));

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null)
            {
                result = true;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return result;
    }

    public static string GetCaseNo(string date)
    {
        string result = "";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"
SELECT 
CASE WHEN MAX(CONVERT(BIGINT, CASE_NO)) + 1 IS NULL THEN 0
ELSE MAX(CONVERT(BIGINT, CASE_NO)) + 1 END CASE_NO
 FROM [dbo].[AML_Edata_Import]
WHERE DataDate = @DataDate";
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@DataDate", date));

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null && resultSet.Tables.Count > 0)
            {
                result = resultSet.Tables[0].Rows[0][0].ToString();

                if (result == "0")
                {
                    result = date + "000001";
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return result;
    }


    /// <summary>
    /// 取得Edata資訊 by RMMBatchNo,AMLInternalID
    /// 20200115-RQ-2019-030155-002 增加篩選條件，判斷其案件是否為0049的案件
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    public static DataTable GetEdataByRMMBatchNoandAMLInternalID(string RMMBatchNo, string AMLInternalID)
    {
        string sql = "";
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        sql = string.Format(@"
            SELECT top 1 *
            FROM [{0}].[dbo].[AML_Edata_Import]
            where RMMBatchNo=@RMMBatchNo and AMLInternalID = @AMLInternalID AND HomeBranch='0049'", UtilHelper.GetAppSettings("DB_KeyinGUI"));
        DataTable result = new DataTable();

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = sql;
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@RMMBatchNo", RMMBatchNo));
        sqlcmd.Parameters.Add(new SqlParameter("@AMLInternalID", AMLInternalID));

        try
        {
            DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
            if (resultSet != null && resultSet.Tables.Count > 0)
            {
                result = resultSet.Tables[0];
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return result;
    }


    //20200821-10月RC-2020/8/20 (週四) 下午 08:29 MAIL提及『請調整收到RMM OK的F檔不論剔退原因為何，都不用異動RMM OK產檔flag』
    // 寫入 NoteLog
    /*
    public static int updateAML_ExportFileFlag(string caseno)
    {
        int masterID = 0;

        string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"
UPDATE [KeyinGUI].[dbo].[AML_HQ_Work] SET AML_ExportFileFlag = 1 where CASE_NO=@NL_CASE_NO
";
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@NL_CASE_NO", caseno));

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null && resultSet.Tables.Count > 0)
            {

                masterID = Convert.ToInt32(resultSet.Tables[0].Rows[0][0].ToString());
            }
        }
        catch (Exception ex)
        {
            Logging.SaveLog(ELogLayer.BusinessRule, ex);
        }

        return masterID;
    }
    */

    // 寫入 NoteLog
    public static int InsertCaseRtnLog(string caseno, string retuncode)
    {
        int masterID = 0;

        string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"
INSERT INTO [dbo].[NoteLog] ([NL_CASE_NO],[NL_SecondKey], [NL_DateTime], [NL_User], [NL_Type], [NL_Value], [NL_ShowFlag])
	VALUES (@NL_CASE_NO,@NL_SecondKey, @NL_DateTime, @NL_User, @NL_Type, @NL_Value, @NL_ShowFlag)
SELECT SCOPE_IDENTITY()";
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@NL_CASE_NO", caseno));
        sqlcmd.Parameters.Add(new SqlParameter("@NL_SecondKey", ""));
        sqlcmd.Parameters.Add(new SqlParameter("@NL_DateTime", date));
        sqlcmd.Parameters.Add(new SqlParameter("@NL_User", "CSIPSYSTEM"));
        sqlcmd.Parameters.Add(new SqlParameter("@NL_Type", retuncode));
        sqlcmd.Parameters.Add(new SqlParameter("@NL_Value","Y"));
        sqlcmd.Parameters.Add(new SqlParameter("@NL_ShowFlag", "1"));

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null && resultSet.Tables.Count > 0)
            {

                masterID = Convert.ToInt32(resultSet.Tables[0].Rows[0][0].ToString());
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return masterID;
    }




    // 查詢時間區間有幾件案件
    public static int queryCaseExportNumber(string starttime, string endtime)
    {
        int masterID = 0;

        string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        SqlCommand sqlcmd = new SqlCommand();
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        sqlcmd.CommandText = string.Format(@"
SELECT COUNT(*)
  FROM [{0}].[dbo].[AML_HQ_Work] where AML_LastExportTime >=@starttime and  AML_LastExportTime <=@endtime
", UtilHelper.GetAppSettings("DB_KeyinGUI"));
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@starttime", starttime+" 00:00:00"));
        sqlcmd.Parameters.Add(new SqlParameter("@endtime", endtime+" 00:00:00"));

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null && resultSet.Tables.Count > 0)
            {

                masterID = Convert.ToInt32(resultSet.Tables[0].Rows[0][0].ToString());
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return masterID;
    }



    // 查詢時間區間有幾件案件
    public static int queryBrchExportNumber(string starttime, string endtime)
    {
        int masterID = 0;

        string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        SqlCommand sqlcmd = new SqlCommand();
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        sqlcmd.CommandText = string.Format(@"
SELECT COUNT(*)
  FROM [{0}].[dbo].[AML_BRCH_Work] where BRCH_LastExportTime >=@starttime and  BRCH_LastExportTime <=@endtime
", UtilHelper.GetAppSettings("DB_KeyinGUI"));
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@starttime", starttime + " 00:00:00"));
        sqlcmd.Parameters.Add(new SqlParameter("@endtime", endtime + " 00:00:00"));

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null && resultSet.Tables.Count > 0)
            {

                masterID = Convert.ToInt32(resultSet.Tables[0].Rows[0][0].ToString());
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return masterID;
    }


    // 寫入 NoteLog
    public static int InsertAFileRtnLog(string ErrorCode, string ExceptionField, string ExceptionReason, string SourceData, string fileNameDat, string ExceptionReasonChi)
    {

        int masterID = 0;

        string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"
INSERT INTO [dbo].[AML_AFileImport] ([ErrorCode],[ExceptionField],[ExceptionReason],[ExceptionReasonChi], [SourceData], [Create_Date],[FileName])
	VALUES (@ErrorCode,@ExceptionField, @ExceptionReason,@ExceptionReasonChi, @SourceData, @Create_Date,@fileNameDat)
SELECT SCOPE_IDENTITY()";
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@ErrorCode", ErrorCode));
        sqlcmd.Parameters.Add(new SqlParameter("@ExceptionField", ExceptionField));
        sqlcmd.Parameters.Add(new SqlParameter("@ExceptionReason", ExceptionReason));
        sqlcmd.Parameters.Add(new SqlParameter("@ExceptionReasonChi", ExceptionReasonChi));
        sqlcmd.Parameters.Add(new SqlParameter("@SourceData", SourceData));
        sqlcmd.Parameters.Add(new SqlParameter("@Create_Date", date));
        sqlcmd.Parameters.Add(new SqlParameter("@fileNameDat", fileNameDat));

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null && resultSet.Tables.Count > 0)
            {

                masterID = Convert.ToInt32(resultSet.Tables[0].Rows[0][0].ToString());
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return masterID;



    }

    // 寫入 NoteLog
    public static int InsertCaseRtnLog2(string caseno,string branchid, string retuncode)
    {
        int masterID = 0;

        string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"
INSERT INTO [dbo].[NoteLog] ([NL_CASE_NO],[NL_SecondKey],[NL_DateTime], [NL_User], [NL_Type], [NL_Value], [NL_ShowFlag])
	VALUES (@NL_CASE_NO,@NL_SecondKey, @NL_DateTime, @NL_User, @NL_Type, @NL_Value, @NL_ShowFlag)
SELECT SCOPE_IDENTITY()";
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@NL_CASE_NO", caseno));
        sqlcmd.Parameters.Add(new SqlParameter("@NL_SecondKey", branchid));
        sqlcmd.Parameters.Add(new SqlParameter("@NL_DateTime", date));
        sqlcmd.Parameters.Add(new SqlParameter("@NL_User", "CSIPSYSTEM"));
        sqlcmd.Parameters.Add(new SqlParameter("@NL_Type", retuncode));
        sqlcmd.Parameters.Add(new SqlParameter("@NL_Value", "Y"));
        sqlcmd.Parameters.Add(new SqlParameter("@NL_ShowFlag", "1"));

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null && resultSet.Tables.Count > 0)
            {

                masterID = Convert.ToInt32(resultSet.Tables[0].Rows[0][0].ToString());
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return masterID;
    }




    public static bool AML_CASE_ACT_LOG_Rtn(string RMM_Batch_No, string AML_Internal_ID, string BRCO_BRCH_NO, string BRCO_BRCH_SEQ, string RETURN_CODE)
    {
        bool result = false;
        DateTime now = DateTime.Now;

        SqlCommand sqlcmd = new SqlCommand();
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        sqlcmd.CommandText = string.Format(@"

INSERT INTO [{0}].[dbo].[AML_CASE_ACT_RTN_LOG]
           ([RMM_Batch_No]
           ,[AML_Internal_ID]
           ,[BRCO_BRCH_NO]
           ,[BRCO_BRCH_SEQ]
           ,[RETURN_CODE]
           ,[CreateTime]
           ,[CreateUser])
     VALUES
           (@RMM_Batch_No
           ,@AML_Internal_ID
           ,@BRCO_BRCH_NO
           ,@BRCO_BRCH_SEQ
           ,@RETURN_CODE
           ,@CreateTime
           ,@CreateUser)
", UtilHelper.GetAppSettings("DB_KeyinGUI"));
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@RMM_Batch_No", RMM_Batch_No));
        sqlcmd.Parameters.Add(new SqlParameter("@AML_Internal_ID", AML_Internal_ID));
        sqlcmd.Parameters.Add(new SqlParameter("@BRCO_BRCH_NO", BRCO_BRCH_NO));
        sqlcmd.Parameters.Add(new SqlParameter("@BRCO_BRCH_SEQ", BRCO_BRCH_SEQ));
        sqlcmd.Parameters.Add(new SqlParameter("@RETURN_CODE", RETURN_CODE));
        sqlcmd.Parameters.Add(new SqlParameter("@CreateTime", now.ToString("yyyy-MM-dd HH:mm:ss")));
        sqlcmd.Parameters.Add(new SqlParameter("@CreateUser", "CSIP_SYSTEM"));


        try
        {
            DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
            if (resultSet != null)
            {
                result = true;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return result;
    }


    public static bool AML_CASE_ACT_LOG_RtnAML(string RMM_Batch_No, string AML_Internal_ID, string BRCO_BRCH_NO, string BRCO_BRCH_SEQ, string RETURN_CODE)
    {
        bool result = false;
        DateTime now = DateTime.Now;

        SqlCommand sqlcmd = new SqlCommand();
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        sqlcmd.CommandText = string.Format(@"

INSERT INTO [{0}].[dbo].[AML_CASE_ACT_RTN_LOG]
           ([RMM_Batch_No]
           ,[AML_Internal_ID]
           ,[BRCO_BRCH_NO]
           ,[BRCO_BRCH_SEQ]
           ,[RETURN_CODE]
           ,[CreateTime]
           ,[CreateUser])
     VALUES
           (@RMM_Batch_No
           ,@AML_Internal_ID
           ,@BRCO_BRCH_NO
           ,@BRCO_BRCH_SEQ
           ,@RETURN_CODE
           ,@CreateTime
           ,@CreateUser)
", UtilHelper.GetAppSettings("DB_KeyinGUI"));
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@RMM_Batch_No", RMM_Batch_No));
        sqlcmd.Parameters.Add(new SqlParameter("@AML_Internal_ID", AML_Internal_ID));
        sqlcmd.Parameters.Add(new SqlParameter("@BRCO_BRCH_NO", BRCO_BRCH_NO));
        sqlcmd.Parameters.Add(new SqlParameter("@BRCO_BRCH_SEQ", BRCO_BRCH_SEQ));
        sqlcmd.Parameters.Add(new SqlParameter("@RETURN_CODE", RETURN_CODE));
        sqlcmd.Parameters.Add(new SqlParameter("@CreateTime", now.ToString("yyyy-MM-dd HH:mm:ss")));
        sqlcmd.Parameters.Add(new SqlParameter("@CreateUser", "CSIP_SYSTEMAML"));


        try
        {
            DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
            if (resultSet != null)
            {
                result = true;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return result;
    }


    /// <summary>
    /// 取得AgentData資訊
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    public static DataTable GetAgentData()
    {
        string sql = "";
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        sql = string.Format(@"
            SELECT *
            FROM [{0}].[dbo].[AML_AGENT_SETTING] WITH(NOLOCK) where user_status=1 and ASSIGN_RATE>0
            order by USER_ID ASC
            ", UtilHelper.GetAppSettings("DB_KeyinGUI"));
        DataTable result = new DataTable();

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = sql;
        sqlcmd.CommandType = CommandType.Text;


        try
        {
            DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
            if (resultSet != null && resultSet.Tables.Count > 0)
            {
                result = resultSet.Tables[0];
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return result;
    }

    // 判斷能分派的人員所有比率加總是不是100% ，若不是回傳FALSE
    public static bool IsDispatchOK()
    {
        bool IsDispatchOK = false;

        SqlCommand sqlcmd = new SqlCommand();
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        sqlcmd.CommandText = string.Format(@"
        SELECT SUM(ASSIGN_RATE)AS DISPATHRATE
        FROM [{0}].[dbo].[AML_AGENT_SETTING] WITH(NOLOCK) where user_status=1", UtilHelper.GetAppSettings("DB_KeyinGUI"));
        sqlcmd.CommandType = CommandType.Text;

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null && resultSet.Tables.Count > 0)
            {

                if (resultSet.Tables[0].Rows[0]["DISPATHRATE"] != Convert.DBNull)
                {
                    int masterID = Convert.ToInt32(resultSet.Tables[0].Rows[0]["DISPATHRATE"].ToString());
                    if (masterID == 100 || masterID == 0)
                    {
                        IsDispatchOK = true;
                    }
                }
                else
                {
                    IsDispatchOK = true;
                }
            }
            else
            {
                //都找不到資料的狀況
                IsDispatchOK = true;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return IsDispatchOK;
    }

    // 若是加總起來是零或是沒設就不分配 ， 與上面差了100%的判斷。
    public static bool boolnodispatch()
    {
        bool Isnodispatch = false;

        SqlCommand sqlcmd = new SqlCommand();
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        sqlcmd.CommandText = string.Format(@"
        SELECT SUM(ASSIGN_RATE)AS DISPATHRATE
        FROM [{0}].[dbo].[AML_AGENT_SETTING] WITH(NOLOCK) where user_status=1", UtilHelper.GetAppSettings("DB_KeyinGUI"));
        sqlcmd.CommandType = CommandType.Text;

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null && resultSet.Tables.Count > 0)
            {
                if (resultSet.Tables[0].Rows[0]["DISPATHRATE"] != Convert.DBNull)
                {
                    int masterID = Convert.ToInt32(resultSet.Tables[0].Rows[0]["DISPATHRATE"].ToString());
                    if (masterID == 0)
                    {
                        Isnodispatch = true;
                    }
                }
                else
                {
                    Isnodispatch = true;
                }
            }
            else
            {
                //都找不到資料的狀況
                Isnodispatch = true;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }
        return Isnodispatch;
    }




    public static bool deletehqwork(string batchno, string internalid, string hqno, string hqseq)
    {
        bool result = false;
        SqlCommand sqlcmd = new SqlCommand();
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        sqlcmd.CommandText = string.Format(@"
        delete [{0}].[dbo].[AML_HQ_Work] where HCOP_BATCH_NO = @batchno and HCOP_INTER_ID = @internalid and  HCOP_HEADQUATERS_CORP_NO = @hqno and HCOP_HEADQUATERS_CORP_SEQ = @hqseq
        ", UtilHelper.GetAppSettings("DB_KeyinGUI"));
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@batchno", batchno));
        sqlcmd.Parameters.Add(new SqlParameter("@internalid", internalid));
        sqlcmd.Parameters.Add(new SqlParameter("@hqno", hqno));
        sqlcmd.Parameters.Add(new SqlParameter("@hqseq", hqseq));

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null)
            {
                result = true;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return result;
    }




    public static bool deletebrchwork(string batchno, string internalid, string brchno, string brchseq)
    {
        bool result = false;
        SqlCommand sqlcmd = new SqlCommand();
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        sqlcmd.CommandText = string.Format(@"
        delete [{0}].[dbo].[AML_BRCH_Work] where BRCH_BATCH_NO = @batchno and BRCH_INTER_ID = @internalid and  BRCH_BRCH_NO = @brchno and BRCH_HQ_BRCH_SEQ_NO = @brchseq
        ", UtilHelper.GetAppSettings("DB_KeyinGUI"));
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@batchno", batchno));
        sqlcmd.Parameters.Add(new SqlParameter("@internalid", internalid));
        sqlcmd.Parameters.Add(new SqlParameter("@brchno", brchno));
        sqlcmd.Parameters.Add(new SqlParameter("@brchseq", brchseq));

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null)
            {
                result = true;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return result;
    }



    public static bool deletemanagerwork(string batchno, string internalid)
    {
        bool result = false;
        SqlCommand sqlcmd = new SqlCommand();
        //2021/03/09_Ares_Stanley-DB名稱改為變數
        sqlcmd.CommandText = string.Format(@"
  delete [{0}].[dbo].[AML_HQ_Manager_Work] where HCOP_BATCH_NO = @batchno and HCOP_INTER_ID = @internalid 
", UtilHelper.GetAppSettings("DB_KeyinGUI"));
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@batchno", batchno));
        sqlcmd.Parameters.Add(new SqlParameter("@internalid", internalid));

        try
        {
            DataSet resultSet = BRAML_File_Import.SearchOnDataSet(sqlcmd);
            if (resultSet != null)
            {
                result = true;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }

        return result;
    }


}
