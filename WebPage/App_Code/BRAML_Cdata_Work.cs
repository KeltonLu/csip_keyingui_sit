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
public class BRAML_Cdata_Work : CSIPCommonModel.BusinessRules.BRBase<EntityAML_Cdata_Work>
{
    public BRAML_Cdata_Work()
    { 
    }
    /// <summary>
    /// 讀取案件明細表頭
    /// </summary>
    /// <returns></returns>
    public static EntityAML_Cdata_Work getCData_WOrk(AML_SessionState parmObj)
    {
        // 20210527 EOS_AML(NOVA) by Ares Dennis
        #region 退版機制
        DataTable dt2 = new DataTable();
        CSIPCommonModel.BusinessRules_new.BRM_PROPERTY_CODE.GetEnableProperty("01", new string[] { "OLD_VERSION_FLAG" }, ref dt2);
        string flag = "";
        if(dt2.Rows.Count > 0)
        {
            flag = dt2.Rows[0]["PROPERTY_CODE"].ToString();
        }
        #endregion

        string sSQL = "";
        if(flag == "N")// 新版程式碼
        {
            sSQL = @"  
                select top 1  C.ID,C.[FileName]      ,C.Datadate      ,C.CustomerID      ,C.CustomerEnglishName      ,C.CustomerChineseName      ,C.AMLSegment      ,C.AMLRiskRanking
               ,C.AMLNextReviewDate      ,C.BlackListHitFlag      ,C.PEPListHitFlag      ,C.NNListHitFlag      ,C.Incorporated      ,C.IncorporatedDate      ,C.LastUpdateMaker
               ,C.LastUpdateChecker      ,C.LastUpdateBranch      ,C.LastUpdateDate      ,C.LastUpdateSourceSystem      ,C.HomeBranch      ,C.Reason      ,C.WarningFlag      ,C.FiledSAR
               ,C.CreditCardBlockCode      ,C.InternationalOrgPEP      ,C.DomesticPEP      ,C.ForeignPEPStakeholder      ,C.InternationalOrgPEPStakeholder      ,C.DomesticPEPStakeholder
            ,C.GroupInformationSharingNameListflag      ,C.Filler      ,C.Create_Time      ,C.Create_User      ,C.Create_Date, C.Dormant_Flag, C.Dormant_Date, C.Incorporated_Source_System
               ,C.AML_Last_Review_Date, C.Risk_Factor_PEP, C.Risk_Factor_RP_PEP, C.Internal_List_Flag, C.High_Risk_Flag_Because_Rpty, C.High_Risk_Flag 
               from  AML_Edata_Work B 
               join AML_Cdata_Work C
               on C.CustomerID = b.CustomerID
               where B.[HomeBranch] ='0049'

             ";
        }
        else// 舊版程式碼
        {
            sSQL = @"  
               select top 1  C.ID,C.[FileName]      ,C.Datadate      ,C.CustomerID      ,C.CustomerEnglishName      ,C.CustomerChineseName      ,C.AMLSegment      ,C.AMLRiskRanking
              ,C.AMLNextReviewDate      ,C.BlackListHitFlag      ,C.PEPListHitFlag      ,C.NNListHitFlag      ,C.Incorporated      ,C.IncorporatedDate      ,C.LastUpdateMaker
              ,C.LastUpdateChecker      ,C.LastUpdateBranch      ,C.LastUpdateDate      ,C.LastUpdateSourceSystem      ,C.HomeBranch      ,C.Reason      ,C.WarningFlag      ,C.FiledSAR
              ,C.CreditCardBlockCode      ,C.InternationalOrgPEP      ,C.DomesticPEP      ,C.ForeignPEPStakeholder      ,C.InternationalOrgPEPStakeholder      ,C.DomesticPEPStakeholder
	          ,C.GroupInformationSharingNameListflag      ,C.Filler      ,C.Create_Time      ,C.Create_User      ,C.Create_Date 
              from  AML_Edata_Work B 
              join AML_Cdata_Work C
              on C.CustomerID = b.CustomerID
              where B.[HomeBranch] ='0049'
    
            ";
        }
                       
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
      
        if (parmObj != null)
        {
            if (!String.IsNullOrEmpty(parmObj.RMMBatchNo))
            {
                sSQL += " and b.RMMBatchNo = @RMMBatchNo";
                sqlcmd.Parameters.Add(new SqlParameter("@RMMBatchNo", parmObj.RMMBatchNo));
            }
            if (!String.IsNullOrEmpty(parmObj.AMLInternalID))
            {
                sSQL += " and b.AMLInternalID =@AMLInternalID ";
                sqlcmd.Parameters.Add(new SqlParameter("@AMLInternalID", parmObj.AMLInternalID));
            }
            //if (!String.IsNullOrEmpty(parmObj.ID))
            //{
            //    sSQL += " and c.ID =@ID ";
            //    sqlcmd.Parameters.Add(new SqlParameter("@ID", parmObj.ID));
            //}
        }
        sqlcmd.CommandText = sSQL;
        EntityAML_Cdata_Work rtnObj = new EntityAML_Cdata_Work();
        DataTable dt = new DataTable();
        DataSet DS = SearchOnDataSet(sqlcmd);
        if (DS != null && DS.Tables.Count > 0)
        {
            dt = DS.Tables[0];
        }
        if (dt.Rows.Count > 0)
        {
            DataTableConvertor.convSingRow<EntityAML_Cdata_Work>(ref rtnObj, dt.Rows[0]);
        }
        return rtnObj;
    }
    /// <summary>
    /// 新增多筆
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public bool Insert(List<EntityAML_Cdata_Work> paramObj)
    {
        bool result = false;
        ///使用交易
        using (OMTransactionScope ts = new OMTransactionScope())
        {
            foreach (EntityAML_Cdata_Work scOBJ in paramObj)
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
    public bool Insert(EntityAML_Cdata_Work paramObj)
    {
        bool result = false;
        
            try
            {

                string sSql = @"Insert into AML_Cdata_Work
( FileName,Datadate,CustomerID,CustomerEnglishName,CustomerChineseName,AMLSegment,AMLRiskRanking,AMLNextReviewDate,BlackListHitFlag,PEPListHitFlag,NNListHitFlag,Incorporated,IncorporatedDate,LastUpdateMaker,LastUpdateChecker,LastUpdateBranch,LastUpdateDate,LastUpdateSourceSystem,HomeBranch,Reason,WarningFlag,FiledSAR,CreditCardBlockCode,InternationalOrgPEP,DomesticPEP,ForeignPEPStakeholder,InternationalOrgPEPStakeholder,DomesticPEPStakeholder,GroupInformationSharingNameListflag,Filler,Create_Time,Create_User,Create_Date)
VALUES( @FileName,@Datadate,@CustomerID,@CustomerEnglishName,@CustomerChineseName,@AMLSegment,@AMLRiskRanking,@AMLNextReviewDate,@BlackListHitFlag,@PEPListHitFlag,@NNListHitFlag,@Incorporated,@IncorporatedDate,@LastUpdateMaker,@LastUpdateChecker,@LastUpdateBranch,@LastUpdateDate,@LastUpdateSourceSystem,@HomeBranch,@Reason,@WarningFlag,@FiledSAR,@CreditCardBlockCode,@InternationalOrgPEP,@DomesticPEP,@ForeignPEPStakeholder,@InternationalOrgPEPStakeholder,@DomesticPEPStakeholder,@GroupInformationSharingNameListflag,@Filler,@Create_Time,@Create_User,@Create_Date);
";
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
              
                sqlcmd.CommandText = sSql;

                //    sqlcmd.Parameters.Add(new SqlParameter("@ID", paramObj.ID));
                sqlcmd.Parameters.Add(new SqlParameter("@FileName", paramObj.FileName));
                sqlcmd.Parameters.Add(new SqlParameter("@Datadate", paramObj.Datadate));
                sqlcmd.Parameters.Add(new SqlParameter("@CustomerID", paramObj.CustomerID));
                sqlcmd.Parameters.Add(new SqlParameter("@CustomerEnglishName", paramObj.CustomerEnglishName));
                sqlcmd.Parameters.Add(new SqlParameter("@CustomerChineseName", paramObj.CustomerChineseName));
                sqlcmd.Parameters.Add(new SqlParameter("@AMLSegment", paramObj.AMLSegment));
                sqlcmd.Parameters.Add(new SqlParameter("@AMLRiskRanking", paramObj.AMLRiskRanking));
                sqlcmd.Parameters.Add(new SqlParameter("@AMLNextReviewDate", paramObj.AMLNextReviewDate));
                sqlcmd.Parameters.Add(new SqlParameter("@BlackListHitFlag", paramObj.BlackListHitFlag));
                sqlcmd.Parameters.Add(new SqlParameter("@PEPListHitFlag", paramObj.PEPListHitFlag));
                sqlcmd.Parameters.Add(new SqlParameter("@NNListHitFlag", paramObj.NNListHitFlag));
                sqlcmd.Parameters.Add(new SqlParameter("@Incorporated", paramObj.Incorporated));
                sqlcmd.Parameters.Add(new SqlParameter("@IncorporatedDate", paramObj.IncorporatedDate));
                sqlcmd.Parameters.Add(new SqlParameter("@LastUpdateMaker", paramObj.LastUpdateMaker));
                sqlcmd.Parameters.Add(new SqlParameter("@LastUpdateChecker", paramObj.LastUpdateChecker));
                sqlcmd.Parameters.Add(new SqlParameter("@LastUpdateBranch", paramObj.LastUpdateBranch));
                sqlcmd.Parameters.Add(new SqlParameter("@LastUpdateDate", paramObj.LastUpdateDate));
                sqlcmd.Parameters.Add(new SqlParameter("@LastUpdateSourceSystem", paramObj.LastUpdateSourceSystem));
                sqlcmd.Parameters.Add(new SqlParameter("@HomeBranch", paramObj.HomeBranch));
                sqlcmd.Parameters.Add(new SqlParameter("@Reason", paramObj.Reason));
                sqlcmd.Parameters.Add(new SqlParameter("@WarningFlag", paramObj.WarningFlag));
                sqlcmd.Parameters.Add(new SqlParameter("@FiledSAR", paramObj.FiledSAR));
                sqlcmd.Parameters.Add(new SqlParameter("@CreditCardBlockCode", paramObj.CreditCardBlockCode));
                sqlcmd.Parameters.Add(new SqlParameter("@InternationalOrgPEP", paramObj.InternationalOrgPEP));
                sqlcmd.Parameters.Add(new SqlParameter("@DomesticPEP", paramObj.DomesticPEP));
                sqlcmd.Parameters.Add(new SqlParameter("@ForeignPEPStakeholder", paramObj.ForeignPEPStakeholder));
                sqlcmd.Parameters.Add(new SqlParameter("@InternationalOrgPEPStakeholder", paramObj.InternationalOrgPEPStakeholder));
                sqlcmd.Parameters.Add(new SqlParameter("@DomesticPEPStakeholder", paramObj.DomesticPEPStakeholder));
                sqlcmd.Parameters.Add(new SqlParameter("@GroupInformationSharingNameListflag", paramObj.GroupInformationSharingNameListflag));
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