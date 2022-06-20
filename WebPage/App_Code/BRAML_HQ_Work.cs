//******************************************************************
//*  作    者：林家賜
//*  功能說明：收單特店審查處理  總公司資料
//*  創建日期：2019/01/24
//*  修改紀錄：2020/12/14_Ares_Stanley-新增NPOI報表產出; 2021/04/01_Ares_Stanley-移除MicrosoftExcel
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*******************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CSIPNewInvoice.EntityLayer_new;
using Framework.Data.OM.Transaction;
using System.Configuration;
using Framework.Common.Logging;
using System.Reflection;
using Framework.Common.Utility;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.HSSF.EventUserModel.DummyRecord;
using NPOI.XSSF.UserModel.Charts;
using System.Diagnostics;

/// <summary>
/// BRAML_Cdata_Work 的摘要描述
/// </summary>
public class BRAML_HQ_Work : CSIPCommonModel.BusinessRules.BRBase<EntityAML_HQ_Work>
{

    public BRAML_HQ_Work()
    {    //        // TODO: 在這裡新增建構函式邏輯        //
    }


    public static int getProjectCount(string CaseType, string UserRoll, string Owner_User)
    {


        string sSQL = @" select count(*)  
                                    from AML_HQ_Work A join AML_Edata_Work B
                                    on b.RMMBatchNo = a.HCOP_BATCH_NO and b.AMLInternalID = A.HCOP_INTER_ID and A.CASE_NO = b.CASE_NO
                                    left join AML_Cdata_Work C on C.CustomerID = b.CustomerID
                                    where B.[HomeBranch] ='0049'  ";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;

        //加入案件狀態條件
        switch (CaseType)
        {
            case "new":
                ///經辦人員為空
                sSQL += " and  (A.CaseOwner_User is null or A.CaseOwner_User ='') ";
                break;
            case "procing":
                sSQL += " and  A.CaseProcess_Status ='0' and  (A.CaseOwner_User  is not null and A.CaseOwner_User !=''  )  and A.CaseProcess_User ='M1'   ";
                break;
            case "Master":
                //20191108-RQ-2018-015749-002 modify by Peggy
                //sSQL += " and  A.CaseProcess_Status ='0' and   (A.CaseOwner_User is not null and A.CaseOwner_User !='')  and A.CaseProcess_User !='M1'   ";
                sSQL += " and  A.CaseProcess_Status in ('0','3','4','5')  and ISNULL(A.CaseOwner_User,'') !=''  and A.CaseProcess_User !='M1'   ";
                break;
            case "Reject":
                //20191108-RQ-2018-015749-002 modify by Peggy
                //sSQL += " and  A.CaseProcess_Status ='1' ";
                sSQL += " and  A.CaseProcess_Status in ('1','13','14','15') ";
                break;
            //ALL不設任何條件  排除CaseProcess_Status = 2 已放行的案件 0311追加
            case "ALL":
                //20191108-RQ-2018-015749-002 modify by Peggy
                //sSQL += " and (A.CaseProcess_Status <> '2' or  A.CaseProcess_Status is null or  A.CaseProcess_Status ='' ) ";
                sSQL += " and (A.CaseProcess_Status not in ('2','23','24','25') or  ISNULL(A.CaseProcess_Status,'') ='' ) ";
                break;
        }


        ///加入處理經辦
        if (!string.IsNullOrEmpty(Owner_User))
        {
            sSQL += " and  A.CaseOwner_User = @CaseOwner_User ";
            sqlcmd.Parameters.Add(new SqlParameter("@CaseOwner_User", Owner_User));
        }

        ///加入使用者角色
        if (!string.IsNullOrEmpty(UserRoll))
        {
            string[] tmp = UserRoll.Split(',');
            if (tmp.Length == 1)
            {
                sSQL += " and  A.CaseProcess_User = @CaseProcess_User ";
                sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User", tmp[0]));
            }
            if (tmp.Length == 2)
            {
                sSQL += " and  A.CaseProcess_User in (@CaseProcess_User1 ,@CaseProcess_User2 )";
                sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User1", tmp[0]));
                sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User2", tmp[1]));
            }
            if (tmp.Length == 3)
            {
                sSQL += " and  A.CaseProcess_User in (@CaseProcess_User1 ,@CaseProcess_User2 ,@CaseProcess_User3 )";
                sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User1", tmp[0]));
                sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User2", tmp[1]));
                sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User3", tmp[2]));
            }

        }
        sqlcmd.CommandText = sSQL;
        DataSet DS = SearchOnDataSet(sqlcmd);
        int result = 0;
        if (DS != null && DS.Tables.Count > 0)
        {
            result = int.Parse(DS.Tables[0].Rows[0][0].ToString());
        }
        return result;


    }

    /// <summary>
    /// 讀取案件明細表頭
    /// </summary>
    /// <returns></returns>
    /// 20191112-RQ-2018-015749-002 修改：加上案件編號的篩選條件 by Peggy
    /// 20200227-RQ-2019-030155-003 增加b.OriginalNextReviewDate,b.NewRiskRanking 欄位 by Peggy
    public static AML_SessionState getProjDetailHeader(AML_SessionState parmObj)
    {
        string sSQL = @" select   top 1 A.CASE_NO ,A.HCOP_HEADQUATERS_CORP_NO ,b.OriginalRiskRanking,b.OriginalNextReviewDate,b.NewRiskRanking,  
                       A.CaseExpiryDate ,b.CaseType,B.NewNextReviewDate ,A.HCOP_REG_NAME,b.DataDate, AddressLabelFlagTime,AddressLabelTwoMonthFlagTime
                        from AML_HQ_Work A join AML_Edata_Work B  on b.RMMBatchNo = a.HCOP_BATCH_NO and b.AMLInternalID  = A.HCOP_INTER_ID and A.CASE_NO = b.CASE_NO
                        left join AML_Cdata_Work C on C.CustomerID = b.CustomerID 
                        where B.[HomeBranch] ='0049' and b.RMMBatchNo  = @RMMBatchNo and B.AMLInternalID = @AMLInternalID and A.CASE_NO = @CASE_NO;";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sSQL;
        sqlcmd.Parameters.Add(new SqlParameter("@RMMBatchNo", parmObj.RMMBatchNo));
        sqlcmd.Parameters.Add(new SqlParameter("@AMLInternalID", parmObj.AMLInternalID));
        sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", parmObj.CASE_NO));//20191112-RQ-2018-015749-002 修改：加上案件編號的篩選條件 by Peggy

        DataTable dt = new DataTable();
        DataSet DS = SearchOnDataSet(sqlcmd);
        if (DS != null && DS.Tables.Count > 0)
        {
            dt = DS.Tables[0];
        }
        //2021/03/10_Ares_Stanley-移除空白字元
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["HCOP_REG_NAME"] = dt.Rows[i]["HCOP_REG_NAME"].ToString().Trim();
            }
            DataTableConvertor.convSingRow<AML_SessionState>(ref parmObj, dt.Rows[0]);
        }
        return parmObj;
    }
    /// <summary>
    /// 讀取案件明細
    /// </summary>
    /// <returns></returns>
    public static EntityAML_HQ_Work getHQ_WOrk(AML_SessionState parmObj)
    {
        //20191226-RQ-2019-030155-002 新增聯絡電話欄位(HCOP_MOBILE)
        //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
        //string sSQL = @"  select top 1  ID,CASE_NO,[FileName],HCOP_BATCH_NO,HCOP_INTER_ID,HCOP_SIXM_TOT_AMT,HCOP_MON_AMT1,HCOP_MON_AMT2,HCOP_MON_AMT3,HCOP_MON_AMT4,HCOP_MON_AMT5,
        //                HCOP_MON_AMT6,HCOP_MON_AMT7,HCOP_MON_AMT8,HCOP_MON_AMT9,HCOP_MON_AMT10,HCOP_MON_AMT11,HCOP_MON_AMT12,HCOP_KEY,HCOP_HEADQUATERS_CORP_NO,HCOP_HEADQUATERS_CORP_SEQ,
        //                HCOP_CORP_TYPE,HCOP_REGISTER_NATION,HCOP_CORP_REG_ENG_NAME,HCOP_REG_NAME,HCOP_NAME_0E,HCOP_NAME_CHI,HCOP_NAME_0F,HCOP_BUILD_DATE,HCOP_CC,HCOP_REG_CITY,HCOP_REG_ADDR1,
        //                HCOP_REG_ADDR2,HCOP_EMAIL,HCOP_NP_COMPANY_NAME,HCOP_OWNER_NATION,HCOP_OWNER_CHINESE_NAME,HCOP_OWNER_ENGLISH_NAME,HCOP_OWNER_BIRTH_DATE,HCOP_OWNER_ID,HCOP_OWNER_ID_ISSUE_DATE,
        //                HCOP_OWNER_ID_ISSUE_PLACE,HCOP_OWNER_ID_REPLACE_TYPE,HCOP_ID_PHOTO_FLAG,HCOP_PASSPORT,HCOP_PASSPORT_EXP_DATE,HCOP_RESIDENT_NO,HCOP_RESIDENT_EXP_DATE,HCOP_OTHER_CERT,
        //                HCOP_OTHER_CERT_EXP_DATE,HCOP_COMPLEX_STR_CODE,HCOP_ISSUE_STOCK_FLAG,HCOP_COMP_TEL,HCOP_MAILING_CITY,HCOP_MAILING_ADDR1,HCOP_MAILING_ADDR2,HCOP_PRIMARY_BUSI_COUNTRY,
        //                HCOP_BUSI_RISK_NATION_FLAG,HCOP_BUSI_RISK_NATION_1,HCOP_BUSI_RISK_NATION_2,HCOP_BUSI_RISK_NATION_3,HCOP_BUSI_RISK_NATION_4,HCOP_BUSI_RISK_NATION_5,HCOP_OVERSEAS_FOREIGN,
        //                HCOP_OVERSEAS_FOREIGN_COUNTRY,HCOP_REGISTER_US_STATE,HCOP_BUSINESS_ORGAN_TYPE,HCOP_CREATE_DATE,HCOP_STATUS,HCOP_QUALIFY_FLAG,HCOP_CONTACT_NAME,HCOP_EXAMINE_FLAG,
        //                HCOP_ALLOW_ISSUE_STOCK_FLAG,HCOP_CONTACT_TEL,HCOP_UPDATE_DATE,HCOP_CREATE_ID,HCOP_UPDATE_ID,HCOP_OWNER_CITY,HCOP_OWNER_ADDR1,HCOP_OWNER_ADDR2,HCOP_RESERVED_FILLER,
        //                Create_Time,Create_User,Create_Date,CaseExpiryDate,ReviewCompletedDate,SendLetter_NotCooperating,OWNER_ID_Type,OWNER_ID_SreachStatus,CaseOwner_User,CaseProcess_Status,CaseProcess_User,AML_ExportFileFlag,AML_LastExportTime, AddressLabelFlagTime,AddressLabelTwoMonthFlagTime
        //                ,HCOP_OWNER_CHINESE_LNAME,HCOP_OWNER_ROMA,HCOP_CONTACT_LNAME,HCOP_CONTACT_ROMA,HCOP_MOBILE,GROUP_NO,HCOP_CC_2,HCOP_CC_3,HCOP_OC,HCOP_INCOME_SOURCE,HCOP_LAST_UPD_MAKER,HCOP_LAST_UPD_CHECKER,HCOP_LAST_UPD_BRANCH,HCOP_LAST_UPDATE_DATE,HCOP_COUNTRY_CODE_2,HCOP_GENDER,HCOP_REG_ZIP_CODE
        //                from AML_HQ_Work                        
        //                where 1=1 ";
        //20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis
        string sSQL = @"  select top 1  ID,CASE_NO,[FileName],HCOP_BATCH_NO,HCOP_INTER_ID,HCOP_SIXM_TOT_AMT,HCOP_MON_AMT1,HCOP_MON_AMT2,HCOP_MON_AMT3,HCOP_MON_AMT4,HCOP_MON_AMT5,
                        HCOP_MON_AMT6,HCOP_MON_AMT7,HCOP_MON_AMT8,HCOP_MON_AMT9,HCOP_MON_AMT10,HCOP_MON_AMT11,HCOP_MON_AMT12,HCOP_KEY,HCOP_HEADQUATERS_CORP_NO,HCOP_HEADQUATERS_CORP_SEQ,
                        HCOP_CORP_TYPE,HCOP_REGISTER_NATION,HCOP_CORP_REG_ENG_NAME,HCOP_REG_NAME,HCOP_NAME_0E,HCOP_NAME_CHI,HCOP_NAME_0F,HCOP_BUILD_DATE,HCOP_CC,HCOP_REG_CITY,HCOP_REG_ADDR1,
                        HCOP_REG_ADDR2,HCOP_EMAIL,HCOP_NP_COMPANY_NAME,HCOP_OWNER_NATION,HCOP_OWNER_CHINESE_NAME,HCOP_OWNER_ENGLISH_NAME,HCOP_OWNER_BIRTH_DATE,HCOP_OWNER_ID,HCOP_OWNER_ID_ISSUE_DATE,
                        HCOP_OWNER_ID_ISSUE_PLACE,HCOP_OWNER_ID_REPLACE_TYPE,HCOP_ID_PHOTO_FLAG,HCOP_PASSPORT,HCOP_PASSPORT_EXP_DATE,HCOP_RESIDENT_NO,HCOP_RESIDENT_EXP_DATE,HCOP_OTHER_CERT,
                        HCOP_OTHER_CERT_EXP_DATE,HCOP_COMPLEX_STR_CODE,HCOP_ISSUE_STOCK_FLAG,HCOP_COMP_TEL,HCOP_MAILING_CITY,HCOP_MAILING_ADDR1,HCOP_MAILING_ADDR2,HCOP_PRIMARY_BUSI_COUNTRY,
                        HCOP_BUSI_RISK_NATION_FLAG,HCOP_BUSI_RISK_NATION_1,HCOP_BUSI_RISK_NATION_2,HCOP_BUSI_RISK_NATION_3,HCOP_BUSI_RISK_NATION_4,HCOP_BUSI_RISK_NATION_5,HCOP_OVERSEAS_FOREIGN,
                        HCOP_OVERSEAS_FOREIGN_COUNTRY,HCOP_REGISTER_US_STATE,HCOP_BUSINESS_ORGAN_TYPE,HCOP_CREATE_DATE,HCOP_STATUS,HCOP_QUALIFY_FLAG,HCOP_CONTACT_NAME,HCOP_EXAMINE_FLAG,
                        HCOP_ALLOW_ISSUE_STOCK_FLAG,HCOP_CONTACT_TEL,HCOP_UPDATE_DATE,HCOP_CREATE_ID,HCOP_UPDATE_ID,HCOP_OWNER_CITY,HCOP_OWNER_ADDR1,HCOP_OWNER_ADDR2,HCOP_RESERVED_FILLER,
                        Create_Time,Create_User,Create_Date,CaseExpiryDate,ReviewCompletedDate,SendLetter_NotCooperating,OWNER_ID_Type,OWNER_ID_SreachStatus,CaseOwner_User,CaseProcess_Status,CaseProcess_User,AML_ExportFileFlag,AML_LastExportTime, AddressLabelFlagTime,AddressLabelTwoMonthFlagTime
                        ,HCOP_OWNER_CHINESE_LNAME,HCOP_OWNER_ROMA,HCOP_CONTACT_LNAME,HCOP_CONTACT_ROMA,HCOP_MOBILE,HCOP_CC_2,HCOP_CC_3,HCOP_OC,HCOP_INCOME_SOURCE,HCOP_LAST_UPD_MAKER,HCOP_LAST_UPD_CHECKER,HCOP_LAST_UPD_BRANCH,HCOP_LAST_UPDATE_DATE,HCOP_COUNTRY_CODE_2,HCOP_GENDER,HCOP_REG_ZIP_CODE
                        from AML_HQ_Work                        
                        where 1=1 ";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        if (parmObj != null)
        {
            if (!string.IsNullOrEmpty(parmObj.RMMBatchNo))
            {
                sSQL += " AND HCOP_BATCH_NO  = @RMMBatchNo";
                sqlcmd.Parameters.Add(new SqlParameter("@RMMBatchNo", parmObj.RMMBatchNo));
            }
            if (!string.IsNullOrEmpty(parmObj.AMLInternalID))
            {
                sSQL += " AND HCOP_INTER_ID  = @AMLInternalID";
                sqlcmd.Parameters.Add(new SqlParameter("@AMLInternalID", parmObj.AMLInternalID));
            }
            if (!string.IsNullOrEmpty(parmObj.CASE_NO))
            {
                sSQL += " AND CASE_NO  = @CASE_NO";
                sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", parmObj.CASE_NO));
            }
        }
        sqlcmd.CommandText = sSQL;

        EntityAML_HQ_Work rtnObj = new EntityAML_HQ_Work();
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
                dt.Rows[i]["HCOP_OWNER_CHINESE_NAME"] = dt.Rows[i]["HCOP_OWNER_CHINESE_NAME"].ToString().Trim();
                dt.Rows[i]["HCOP_REG_NAME"] = dt.Rows[i]["HCOP_REG_NAME"].ToString().Trim();
                dt.Rows[i]["HCOP_CONTACT_NAME"] = dt.Rows[i]["HCOP_CONTACT_NAME"].ToString().Trim();
            }
            DataTableConvertor.convSingRow<EntityAML_HQ_Work>(ref rtnObj, dt.Rows[0]);
        }

        //追加讀取高階經理人
        //20191209-RQ-2018-015749-002-因有案件重審流程，故讀取資料時，應加上案件編號的篩選條件 by Peggy
        sSQL = @" SELECT  
ID,CASE_NO,FileName,HCOP_BATCH_NO,HCOP_INTER_ID,HCOP_SIXM_TOT_AMT,HCOP_KEY,HCOP_BENE_NATION,HCOP_BENE_NAME,HCOP_BENE_BIRTH_DATE,
HCOP_BENE_ID,HCOP_BENE_PASSPORT,HCOP_BENE_PASSPORT_EXP,HCOP_BENE_RESIDENT_NO,HCOP_BENE_RESIDENT_EXP,HCOP_BENE_OTH_CERT,HCOP_BENE_OTH_CERT_EXP,
HCOP_BENE_JOB_TYPE,HCOP_BENE_JOB_TYPE_2,HCOP_BENE_JOB_TYPE_3,HCOP_BENE_JOB_TYPE_4,HCOP_BENE_JOB_TYPE_5,HCOP_BENE_JOB_TYPE_6,HCOP_BENE_RESERVED,
Create_Date,Create_Time,Create_User,HCOP_BENE_LNAME,HCOP_BENE_ROMA
, HCOP_BATCH_NO + ';' + HCOP_INTER_ID + ';' + Convert(varchar,ID) as ArgNo
  FROM  [AML_HQ_Manager_Work]
  where HCOP_BATCH_NO = @HCOP_BATCH_NO and HCOP_INTER_ID = @HCOP_INTER_ID and CASE_NO = @CASE_NO ;";

        SqlCommand sqlcmd2 = new SqlCommand();
        sqlcmd2.CommandType = CommandType.Text;
        sqlcmd2.CommandText = sSQL;
        sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BATCH_NO", parmObj.RMMBatchNo));
        sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_INTER_ID", parmObj.AMLInternalID));
        sqlcmd2.Parameters.Add(new SqlParameter("@CASE_NO", parmObj.CASE_NO));
        List<EntityAML_HQ_Manager_Work> rtnObj2 = new List<EntityAML_HQ_Manager_Work>();
        DataTable dt2 = new DataTable();
        DataSet DS2 = SearchOnDataSet(sqlcmd2);
        if (DS2 != null && DS2.Tables.Count > 0)
        {
            dt2 = DS2.Tables[0];
        }
        if (dt2.Rows.Count > 0)
        {
            rtnObj2 = DataTableConvertor.ConvertCollToObj<EntityAML_HQ_Manager_Work>(dt2);
        }
        rtnObj.ManagerColl = rtnObj2;

        /*20200330- 因無作用，故mark，不然會一直跳error訊息
        //讀取 長姓名(高階經理人的可能不用, 因為在 P010801010001 裡的列表無法呈現,或許是提示後再點選)
        sSQL = @"";
        sqlcmd.CommandText = sSQL;

        EntityAML_HQ_Work rtnObj3 = new EntityAML_HQ_Work();
        DataTable dt3 = new DataTable();
        DataSet DS3 = SearchOnDataSet(sqlcmd);
        if (DS3 != null && DS3.Tables.Count > 0)
        {
            dt3 = DS3.Tables[0];
        }
        if (dt3.Rows.Count > 0)
        {
            DataTableConvertor.convSingRow<EntityAML_HQ_Work>(ref rtnObj3, dt3.Rows[0]);
        }
        */
        return rtnObj;
    }


    /// <summary>
    /// 讀取案件列表中所有經辦人
    /// </summary>
    /// <returns></returns>
    public static DataTable getProcUser()
    {
        //2021/03/17_Ares_Stanley-DB名稱改為變數
        string sSQL = string.Format(@" select  CaseOwner_User,b.[USER_NAME] from AML_HQ_Work A
  left join {0}.dbo.M_USER B 
 on A.CaseOwner_User = b.[USER_ID]
 where CaseOwner_User is not null
 group by CaseOwner_User,b.[USER_NAME]
 order by CaseOwner_User ", UtilHelper.GetAppSettings("DB_CSIP"));
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sSQL;
        DataTable dt = new DataTable();
        DataSet DS = SearchOnDataSet(sqlcmd);
        if (DS != null && DS.Tables.Count > 0)
        {
            dt = DS.Tables[0];
        }
        return dt;

    }
    /// <summary>
    /// 讀取已結案案件列表中所有經辦人
    /// </summary>
    /// <returns></returns>
    public static DataTable getMasterProcUser()
    {
        //2021/03/17_Ares_Stanley-DB名稱改為變數
        string sSQL = string.Format(@" select  CaseOwner_User,b.[USER_NAME] from AML_HQ_Work A
  left join {0}.dbo.M_USER B 
 on A.CaseOwner_User = b.[USER_ID]
 where CaseOwner_User is not null  and  A.CaseProcess_Status ='2'
 group by CaseOwner_User,b.[USER_NAME]
 order by CaseOwner_User ", UtilHelper.GetAppSettings("DB_CSIP"));
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sSQL;
        DataTable dt = new DataTable();
        DataSet DS = SearchOnDataSet(sqlcmd);
        if (DS != null && DS.Tables.Count > 0)
        {
            dt = DS.Tables[0];
        }
        return dt;

    }

    /// <summary>
    /// 讀取案件列表
    /// </summary>
    /// <param name="parmObj"></param>
    /// <returns></returns>
    public static DataTable Query(AML_HQQuery parmObj)
    {
        //        string sSQL = @"  select  
        //A.CASE_NO,A.HCOP_HEADQUATERS_CORP_NO,A.HCOP_REG_NAME,b.OriginalRiskRanking
        //,b.DataDate,A.CaseExpiryDate,A.ReviewCompletedDate,A.SendLetter_NotCooperating
        //,A.CaseOwner_User,A.CaseProcess_Status,A.CaseProcess_User,C.IncorporatedDate
        //,b.RMMBatchNo + ';' + b.AMLInternalID + ';' + A.CaseProcess_User as ArgNo  
        //from AML_HQ_Work A join AML_Edata_Work B
        //on b.RMMBatchNo = a.HCOP_BATCH_NO and b.AMLInternalID  = A.HCOP_INTER_ID
        //join AML_Cdata_Work C on C.CustomerID = b.CustomerID
        //where B.[HomeBranch] ='0049'";

        // 20191218-RQ-2019-030155-002：不合作日期在不合作的FLAG為Y時 才顯示
        //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
        //string sSQL = @"    select 
        //A.CASE_NO,A.HCOP_HEADQUATERS_CORP_NO,A.HCOP_REG_NAME,b.OriginalRiskRanking
        //,b.DataDate, 
        // case when A.CaseExpiryDate <> '' then A.CaseExpiryDate else
        // dateadd(D, -1, (DATEADD(M, 5, convert(datetime, substring(B.DataDate, 1, 6) + '01')))) end as CaseExpiryDate
        //,A.ReviewCompletedDate,A.SendLetter_NotCooperating
        //,A.CaseOwner_User,A.CaseProcess_Status,A.CaseProcess_User,
        //(Case When ISNULL(C.Incorporated,'')='Y ' THEN C.IncorporatedDate ELSE '' END ) as 'IncorporatedDate',A.AddressLabelFlagTime,A.AddressLabelTwoMonthFlagTime
        //,b.RMMBatchNo + ';' + b.AMLInternalID + ';' + A.CaseProcess_User + ';' + A.CASE_NO + ';' + A.HCOP_HEADQUATERS_CORP_NO as ArgNo,C.Incorporated, A.GROUP_NO
        //from AML_HQ_Work A join AML_Edata_Work B
        //on b.RMMBatchNo = a.HCOP_BATCH_NO and b.AMLInternalID = A.HCOP_INTER_ID and A.CASE_NO = b.CASE_NO
        //left join AML_Cdata_Work C on C.CustomerID = b.CustomerID
        //where B.[HomeBranch] ='0049'  ";
        //20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis

        // 20220613 新增呈核狀態查詢條件，並顯示在列表資料中 By Kelton start
        //string sSQL = @"    select 
        //A.CASE_NO,A.HCOP_HEADQUATERS_CORP_NO,A.HCOP_REG_NAME,b.OriginalRiskRanking
        //,b.DataDate, 
        // case when A.CaseExpiryDate <> '' then A.CaseExpiryDate else
        // dateadd(D, -1, (DATEADD(M, 5, convert(datetime, substring(B.DataDate, 1, 6) + '01')))) end as CaseExpiryDate
        //,A.ReviewCompletedDate,A.SendLetter_NotCooperating
        //,A.CaseOwner_User,A.CaseProcess_Status,A.CaseProcess_User,
        //(Case When ISNULL(C.Incorporated,'')='Y ' THEN C.IncorporatedDate ELSE '' END ) as 'IncorporatedDate',A.AddressLabelFlagTime,A.AddressLabelTwoMonthFlagTime
        //,b.RMMBatchNo + ';' + b.AMLInternalID + ';' + A.CaseProcess_User + ';' + A.CASE_NO + ';' + A.HCOP_HEADQUATERS_CORP_NO as ArgNo,C.Incorporated
        //from AML_HQ_Work A join AML_Edata_Work B
        //on b.RMMBatchNo = a.HCOP_BATCH_NO and b.AMLInternalID = A.HCOP_INTER_ID and A.CASE_NO = b.CASE_NO
        //left join AML_Cdata_Work C on C.CustomerID = b.CustomerID
        //where B.[HomeBranch] ='0049'  ";

        string sSQL = @"    select 
        A.CASE_NO,A.HCOP_HEADQUATERS_CORP_NO,A.HCOP_REG_NAME,b.OriginalRiskRanking
        ,b.DataDate, 
         case when A.CaseExpiryDate <> '' then A.CaseExpiryDate else
         dateadd(D, -1, (DATEADD(M, 5, convert(datetime, substring(B.DataDate, 1, 6) + '01')))) end as CaseExpiryDate
        ,A.ReviewCompletedDate,A.SendLetter_NotCooperating
        ,A.CaseOwner_User,A.CaseProcess_Status,A.CaseProcess_User,
        (case when A.CaseProcess_User = 'C1' THEN '一階主管'
            when A.CaseProcess_User = 'C2' THEN '二階主管'
            else ' ' end) as 'Status',
        (Case When ISNULL(C.Incorporated,'')='Y ' THEN C.IncorporatedDate ELSE '' END ) as 'IncorporatedDate',A.AddressLabelFlagTime,A.AddressLabelTwoMonthFlagTime
        ,b.RMMBatchNo + ';' + b.AMLInternalID + ';' + A.CaseProcess_User + ';' + A.CASE_NO + ';' + A.HCOP_HEADQUATERS_CORP_NO as ArgNo,C.Incorporated
        from AML_HQ_Work A join AML_Edata_Work B
        on b.RMMBatchNo = a.HCOP_BATCH_NO and b.AMLInternalID = A.HCOP_INTER_ID and A.CASE_NO = b.CASE_NO
        left join AML_Cdata_Work C on C.CustomerID = b.CustomerID
        where B.[HomeBranch] ='0049'  ";
        // 20220613 新增呈核狀態查詢條件，並顯示在列表資料中 By Kelton end

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;

        //加入案件狀態條件
        switch (parmObj.CaseType)
        {
            case "new":
                ///經辦人員為空
                sSQL += " and  (A.CaseOwner_User is null or A.CaseOwner_User ='') ";
                break;
            case "procing":
                sSQL += " and  A.CaseProcess_Status ='0' and  (A.CaseOwner_User  is not null and A.CaseOwner_User !=''  )  and A.CaseProcess_User ='M1'   ";
                break;
            case "Master":
                //20191108-RQ-2018-015749-002 modify by Peggy
                //sSQL += " and  A.CaseProcess_Status ='0' and   (A.CaseOwner_User is not null and A.CaseOwner_User !='')  and A.CaseProcess_User !='M1'   ";
                sSQL += " and  A.CaseProcess_Status in ('0','3','4','5')  and   ISNULL(A.CaseOwner_User,'') !=''  and A.CaseProcess_User !='M1'   ";
                break;
            case "Reject":
                //20191108-RQ-2018-015749-002 modify by Peggy
                //sSQL += " and  A.CaseProcess_Status ='1' ";
                sSQL += " and  A.CaseProcess_Status in ('1','13','14','15') ";
                break;
            //ALL不設任何條件  排除CaseProcess_Status = 2 已放行的案件 0311追加
            case "ALL":
                //20191108-RQ-2018-015749-002 modify by Peggy
                //sSQL += " and (A.CaseProcess_Status <> '2' or  A.CaseProcess_Status is null or  A.CaseProcess_Status ='' ) ";
                sSQL += " and (A.CaseProcess_Status not in ('2','23','24','25') or  ISNULL(A.CaseProcess_Status,'')='') ";
                break;
        }


        ///加入統編查詢條件
        if (!string.IsNullOrEmpty(parmObj.TaxNo))
        {
            sSQL += " and  A.HCOP_HEADQUATERS_CORP_NO = @TaxNo ";
            sqlcmd.Parameters.Add(new SqlParameter("@TaxNo", parmObj.TaxNo));
        }
        ///加入編號查詢
        if (!string.IsNullOrEmpty(parmObj.CaseNo))
        {
            sSQL += " and  A.CASE_NO = @CaseNo ";
            sqlcmd.Parameters.Add(new SqlParameter("@CaseNo", parmObj.CaseNo));
        }
        //建案日期開始
        if (!string.IsNullOrEmpty(parmObj.yearS) && !string.IsNullOrEmpty(parmObj.MonthS))
        {
            string BegDate = parmObj.yearS + "-" + parmObj.MonthS + "-01";
            if (DataTableConvertor.IsDate(BegDate))
            {
                sSQL += " and  datediff(M,b.DataDate,@BegDate) <= 0  ";
                sqlcmd.Parameters.Add(new SqlParameter("@BegDate", BegDate));
            }
        }
        //建案日期結束
        if (!string.IsNullOrEmpty(parmObj.yearE) && !string.IsNullOrEmpty(parmObj.MonthE))
        {
            string EndDate = parmObj.yearE + "-" + parmObj.MonthE + "-01";
            if (DataTableConvertor.IsDate(EndDate))
            {
                sSQL += " and  datediff(M,b.DataDate,@EndDate) >= 0  ";
                sqlcmd.Parameters.Add(new SqlParameter("@EndDate", EndDate));
            }
        }
        ///加入風險等級
        if (!string.IsNullOrEmpty(parmObj.RiskRanking))
        {
            sSQL += " and  B.OriginalRiskRanking = @OriginalRiskRanking ";
            sqlcmd.Parameters.Add(new SqlParameter("@OriginalRiskRanking", parmObj.RiskRanking));
        }
        ///加入處理經辦
        if (!string.IsNullOrEmpty(parmObj.Owner_User))
        {
            sSQL += " and  A.CaseOwner_User = @CaseOwner_User ";
            sqlcmd.Parameters.Add(new SqlParameter("@CaseOwner_User", parmObj.Owner_User));
        }
        
        ///加入使用者角色
        if (!string.IsNullOrEmpty(parmObj.UserRoll))
        {
            string[] tmp = parmObj.UserRoll.Split(',');
            if (tmp.Length == 1)
            {
                sSQL += " and  A.CaseProcess_User = @CaseProcess_User ";
                sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User", tmp[0]));
            }
            if (tmp.Length == 2)
            {
                sSQL += " and  A.CaseProcess_User in (@CaseProcess_User1 ,@CaseProcess_User2 )";
                sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User1", tmp[0]));
                sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User2", tmp[1]));
            }
            if (tmp.Length == 3)
            {
                sSQL += " and  A.CaseProcess_User in (@CaseProcess_User1 ,@CaseProcess_User2 ,@CaseProcess_User3 )";
                sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User1", tmp[0]));
                sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User2", tmp[1]));
                sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User3", tmp[2]));
            }
        }

        // 20220613 勾選全部時，要判斷是否有選擇呈核狀態 By Kelton
        if (parmObj.CaseType == "ALL" && !string.IsNullOrEmpty(parmObj.Status))
        {
            sSQL += " and  A.CaseProcess_User = @CaseProcess_User ";
            sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User", parmObj.Status));
        }

        //20191101 修改：增加不合作註記查詢條件 add by Peggy
        if (!string.IsNullOrEmpty(parmObj.IncorporatedFlag) && !parmObj.IncorporatedFlag.ToUpper().Trim().Equals("X"))
        {
            if (parmObj.IncorporatedFlag.ToUpper().Trim().Equals("Y"))
            {
                sSQL += " and  C.Incorporated = @Incorporated ";
            }
            else
            {
                sSQL += " and  (ISNULL(C.Incorporated,'') = '' OR C.Incorporated = @Incorporated) ";
            }
            
            sqlcmd.Parameters.Add(new SqlParameter("@Incorporated", parmObj.IncorporatedFlag));
        }


        string OrderBy = "";
        if (!string.IsNullOrEmpty(parmObj.OrderBy))
        {
            string OrderAsc = string.IsNullOrEmpty(parmObj.OrderASC) ? "" : parmObj.OrderASC;
            OrderBy = " order by   " + parmObj.OrderBy + " " + parmObj.OrderASC;
        }
        else
        {
            //加入預設排序，依CASE_NO降冪
            OrderBy = " order by CASE_NO desc ";
        }

        string ROrderBy = " ROW_NUMBER() OVER(" + OrderBy + ") AS Row# , ";

        sSQL = "    select  " + ROrderBy + " * from (  " + sSQL + " ) V " + OrderBy;

        sqlcmd.CommandText = sSQL;
        DataTable dt = new DataTable();
        DataSet DS = SearchOnDataSet(sqlcmd);
        if (DS != null && DS.Tables.Count > 0)
        {
            //2021/03/10_Ares_Stanley-移除空白字元
            if (DS.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < DS.Tables[0].Rows.Count; i++)
                {
                    DS.Tables[0].Rows[i]["HCOP_REG_NAME"] = DS.Tables[0].Rows[i]["HCOP_REG_NAME"].ToString().Trim();
                }
            }
            dt = DS.Tables[0];
        }

        return dt;

    }
    /// <summary>
    /// 讀取已結案案件列表
    /// </summary>
    /// <param name="parmObj"></param>
    /// <returns></returns>
    public static DataTable MasterQuery(AML_HQQuery parmObj)
    {
        //20191112-RQ-2018-015749-002 modify:新增結案日期的查詢條件 故多join notelog的table by Peggy
        // 20191218-RQ-2019-030155-002 修正筆數會重覆的、不合作日期在不合作的FLAG為Y時 才顯示
        //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
        //string sSQL = @"    SELECT  
        //                                    A.CASE_NO,A.HCOP_HEADQUATERS_CORP_NO,  RTRIM(LTRIM(A.HCOP_REG_NAME)) as HCOP_REG_NAME ,b.OriginalRiskRanking
        //                                    ,b.DataDate, 
        //                                     case when A.CaseExpiryDate <> '' then A.CaseExpiryDate else
        //                                     dateadd(D, -1, (DATEADD(M, 5, convert(datetime, substring(B.DataDate, 1, 6) + '01')))) end as CaseExpiryDate
        //                                    ,A.ReviewCompletedDate,A.SendLetter_NotCooperating
        //                                    ,A.CaseOwner_User,A.CaseProcess_Status,A.CaseProcess_User,
        //			(Case When ISNULL(C.Incorporated,'')='Y ' THEN C.IncorporatedDate ELSE '' END ) as 'IncorporatedDate',A.AddressLabelFlagTime,A.AddressLabelTwoMonthFlagTime
        //                                    ,b.RMMBatchNo + ';' + b.AMLInternalID + ';' + A.CaseProcess_User + ';' + A.CASE_NO  + ';' + A.HCOP_HEADQUATERS_CORP_NO as ArgNo, C.Incorporated,D.NL_DateTime, A.GROUP_NO
        //                                    FROM AML_HQ_Work A  WITH(NOLOCK) JOIN AML_Edata_Work B  WITH(NOLOCK)  on b.RMMBatchNo = a.HCOP_BATCH_NO and b.AMLInternalID = A.HCOP_INTER_ID and A.CASE_NO = b.CASE_NO
        //                                     left join AML_Cdata_Work C  WITH(NOLOCK)  on C.CustomerID = b.CustomerID
        //                                    JOIN NOTELOG_DISTINCT XL ON XL.CASE_NO = B.CASE_NO 
        //                                     INNER JOIN NoteLog D  WITH(NOLOCK)   ON D.NL_CASE_NO =XL.CASE_NO AND D.NL_DATETIME = XL.NL_DATETIME 
        //                                    WHERE B.[HomeBranch] ='0049'  ";
        //20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis

        string sSQL = @"    SELECT  
                                            A.CASE_NO,A.HCOP_HEADQUATERS_CORP_NO,  RTRIM(LTRIM(A.HCOP_REG_NAME)) as HCOP_REG_NAME ,b.OriginalRiskRanking
                                            ,b.DataDate, 
                                             case when A.CaseExpiryDate <> '' then A.CaseExpiryDate else
                                             dateadd(D, -1, (DATEADD(M, 5, convert(datetime, substring(B.DataDate, 1, 6) + '01')))) end as CaseExpiryDate
                                            ,A.ReviewCompletedDate,A.SendLetter_NotCooperating
                                            ,A.CaseOwner_User,A.CaseProcess_Status,A.CaseProcess_User,
											(Case When ISNULL(C.Incorporated,'')='Y ' THEN C.IncorporatedDate ELSE '' END ) as 'IncorporatedDate',A.AddressLabelFlagTime,A.AddressLabelTwoMonthFlagTime
                                            ,b.RMMBatchNo + ';' + b.AMLInternalID + ';' + A.CaseProcess_User + ';' + A.CASE_NO  + ';' + A.HCOP_HEADQUATERS_CORP_NO as ArgNo, C.Incorporated,D.NL_DateTime
                                            FROM AML_HQ_Work A  WITH(NOLOCK) JOIN AML_Edata_Work B  WITH(NOLOCK)  on b.RMMBatchNo = a.HCOP_BATCH_NO and b.AMLInternalID = A.HCOP_INTER_ID and A.CASE_NO = b.CASE_NO
                                             left join AML_Cdata_Work C  WITH(NOLOCK)  on C.CustomerID = b.CustomerID
                                            JOIN NOTELOG_DISTINCT XL ON XL.CASE_NO = B.CASE_NO 
                                             INNER JOIN NoteLog D  WITH(NOLOCK)   ON D.NL_CASE_NO =XL.CASE_NO AND D.NL_DATETIME = XL.NL_DATETIME 
                                            WHERE B.[HomeBranch] ='0049'  ";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;

        //限定為已結案
        //20191112-RQ-2018-015749-002 modify by Peggy
        //sSQL += " and  [CaseProcess_Status] ='2' ";
        //已結案案件增加三種異常結案的案件
        sSQL += " and  [CaseProcess_Status] IN ('2','23','24','25') ";


        //加入統編查詢條件
        if (!string.IsNullOrEmpty(parmObj.TaxNo))
        {
            sSQL += " and  A.HCOP_HEADQUATERS_CORP_NO = @TaxNo ";
            sqlcmd.Parameters.Add(new SqlParameter("@TaxNo", parmObj.TaxNo));
        }
        ///加入編號查詢
        if (!string.IsNullOrEmpty(parmObj.CaseNo))
        {
            sSQL += " and  A.CASE_NO = @CaseNo ";
            sqlcmd.Parameters.Add(new SqlParameter("@CaseNo", parmObj.CaseNo));
        }

        //建案日期開始
        if (!string.IsNullOrEmpty(parmObj.yearS) && !string.IsNullOrEmpty(parmObj.MonthS))
        {
            string BegDate = parmObj.yearS + "-" + parmObj.MonthS + "-01";
            if (DataTableConvertor.IsDate(BegDate))
            {
                sSQL += " and  datediff(M,b.DataDate,@BegDate) <= 0  ";
                sqlcmd.Parameters.Add(new SqlParameter("@BegDate", BegDate));
            }
        }
        //建案日期結束
        if (!string.IsNullOrEmpty(parmObj.yearE) && !string.IsNullOrEmpty(parmObj.MonthE))
        {
            string EndDate = parmObj.yearE + "-" + parmObj.MonthE + "-01";
            if (DataTableConvertor.IsDate(EndDate))
            {
                sSQL += " and  datediff(M,b.DataDate,@EndDate) >= 0  ";
                sqlcmd.Parameters.Add(new SqlParameter("@EndDate", EndDate));
            }
        }

        ///加入風險等級
        if (!string.IsNullOrEmpty(parmObj.RiskRanking))
        {
            sSQL += " and  B.OriginalRiskRanking = @OriginalRiskRanking ";
            sqlcmd.Parameters.Add(new SqlParameter("@OriginalRiskRanking", parmObj.RiskRanking));
        }
        ///加入處理經辦
        if (!string.IsNullOrEmpty(parmObj.Owner_User))
        {
            sSQL += " and  A.CaseOwner_User = @CaseOwner_User ";
            sqlcmd.Parameters.Add(new SqlParameter("@CaseOwner_User", parmObj.Owner_User));
        }

        //20191115-RQ-2018-015749-002 修改：新增查詢條件  add by Peggy ↓
        //不合作註記
        if (!string.IsNullOrEmpty(parmObj.IncorporatedFlag))
        {
            if (parmObj.IncorporatedFlag.Trim().Equals("Y"))
            {
                sSQL += " and  C.Incorporated = @Incorporated ";
            }
            else
            {
                sSQL += " and  (ISNULL(C.Incorporated,'') = '' OR C.Incorporated = @Incorporated) ";
            }
            
            sqlcmd.Parameters.Add(new SqlParameter("@Incorporated", parmObj.IncorporatedFlag));
        }

        //結案原因
        if (!string.IsNullOrEmpty(parmObj.CloseType))
        {
            sSQL += " and  A.CaseProcess_Status = @CloseType ";
            sqlcmd.Parameters.Add(new SqlParameter("@CloseType", parmObj.CloseType));
        }

        //結案日期
        if (!string.IsNullOrEmpty(parmObj.CloseDateS))
        {
            sSQL += " and  Convert(varchar(8), D.NL_DATETIME,112) >= @CloseDateS ";
            sqlcmd.Parameters.Add(new SqlParameter("@CloseDateS", parmObj.CloseDateS));
        }
        if (!string.IsNullOrEmpty(parmObj.CloseDateE))
        {
            sSQL += " and   Convert(varchar(8), D.NL_DATETIME,112) <= @CloseDateE ";
            sqlcmd.Parameters.Add(new SqlParameter("@CloseDateE", parmObj.CloseDateE));
        }
        //20191115-RQ-2018-015749-002 修改：新增查詢條件  add by Peggy ↑

        ///加入使用者角色
        //if (!string.IsNullOrEmpty(parmObj.UserRoll))
        //{
        //    string[] tmp = parmObj.UserRoll.Split(',');
        //    if (tmp.Length == 1)
        //    {
        //        sSQL += " and  A.CaseProcess_User = @CaseProcess_User ";
        //        sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User", tmp[0]));
        //    }
        //    if (tmp.Length == 2)
        //    {
        //        sSQL += " and  A.CaseProcess_User in (@CaseProcess_User1 ,@CaseProcess_User2 )";
        //        sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User1", tmp[0]));
        //        sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User2", tmp[1]));
        //    }
        //    if (tmp.Length == 3)
        //    {
        //        sSQL += " and  A.CaseProcess_User in (@CaseProcess_User1 ,@CaseProcess_User2 ,@CaseProcess_User3 )";
        //        sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User1", tmp[0]));
        //        sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User2", tmp[1]));
        //        sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User3", tmp[2]));
        //    }

        //}
        string OrderBy = "";
        if (!string.IsNullOrEmpty(parmObj.OrderBy))
        {
            string OrderAsc = string.IsNullOrEmpty(parmObj.OrderASC) ? "" : parmObj.OrderASC;
            OrderBy = " order by   " + parmObj.OrderBy + " " + parmObj.OrderASC;
        }
        else
        {
            //加入預設排序，依CASE_NO降冪
            OrderBy = " order by CASE_NO desc ";
        }

        string ROrderBy = " ROW_NUMBER() OVER(" + OrderBy + ") AS Row# , ";
        /*20191216 修改：多餘的
        string RWhere = @" WHERE NL_DateTime IN 
                    (
                        SELECT  MAX(NL_DateTime) as 'NL_DateTime'

                        FROM NOTELOG

                       WHERE NL_TYPE IN('CaseOK', 'NonCooperatedDone', 'CaseClosedDone', 'OtherClosedDone')

                       GROUP BY NL_CASE_NO
                    )";
        
        sSQL = "    select  " + ROrderBy + " * from (  " + sSQL + " ) V " + RWhere  + OrderBy;
        */
        //20191218-RQ-2019-030155-002 修正修正筆數會重覆的
        string exSQL = @" WITH NOTELOG_DISTINCT (CASE_NO,NL_DATETIME)
	                        AS (
				                        SELECT NL_CASE_NO AS CASE_NO,MAX(NL_DATETIME) NL_DATETIME 
				                        FROM NOTELOG  A WITH(NOLOCK) JOIN AML_HQ_WORK  B WITH(NOLOCK) ON  A.NL_CASE_NO = B.CASE_NO AND B.[CaseProcess_Status] IN ('2','23','24','25')
				                        WHERE NL_TYPE IN ('CaseOK' ,'NonCooperatedDone','CaseClosedDone','OtherClosedDone')
				                        GROUP BY NL_CASE_NO 
	                        )   ";
        //20191218-RQ-2019-030155-002
        //sSQL = "    select  " + ROrderBy + " * from (  " + sSQL + " ) V " + OrderBy;
        sSQL = exSQL + "    select  " + ROrderBy + " * from (  " + sSQL + " ) V " + OrderBy;

        sqlcmd.CommandText = sSQL;
        DataTable dt = new DataTable();
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        DataSet DS = SearchOnDataSet(sqlcmd);
        stopWatch.Stop();
        Logging.Log(string.Format("結案案件列表-查詢，耗時： {0} 毫秒", stopWatch.ElapsedMilliseconds)); //20210601_Ares_Stanley-增加結案案件列表查詢耗時記錄
        //20210601_Ares_Stanley-增加結案案件列表列印SQL記錄
        #region 取得所有SQL參數
        string sqlParam = "";
        for(int i = 0; i < sqlcmd.Parameters.Count; i++)
        {
            sqlParam += string.Format("參數名：{0} 參數值：{1}；", sqlcmd.Parameters[i].ParameterName, sqlcmd.Parameters[i].Value);
        }
        #endregion
        Logging.Log(string.Format("==========結案案件列表-查詢 SQL 語法如下==========\r{0}",sqlcmd.CommandText));
        Logging.Log(string.Format("==========結案案件列表-查詢 SQL 參數如下==========\r{0}", sqlParam));
        if (DS != null && DS.Tables.Count > 0)
        {
            dt = DS.Tables[0];
            //20210616_Ares_Stanley-移除登記名稱空白字符
            for(int i=0; i< dt.Rows.Count; i++)
            {
                dt.Rows[i]["HCOP_REG_NAME"] = dt.Rows[i]["HCOP_REG_NAME"].ToString().Trim();
            }
        }

        return dt;

    }

    //20191120-RQ-2018-015749-002
    /// <summary>
    /// 讀取已結案案件列表
    /// 修改紀錄:2021/01/26_Ares_Stanley-調整SQL
    /// </summary>
    /// <param name="parmObj"></param>
    /// <returns></returns>
    public static DataTable ReportQuery(AML_HQQuery parmObj)
    {
        /*
        string sSQL = @"    
                     WITH NOTELOG_DISTINCT (CASE_NO,NL_DATETIME)
					AS (
					          SELECT NL_CASE_NO AS CASE_NO,MAX(NL_DATETIME) NL_DATETIME 
					          FROM NOTELOG  A JOIN AML_HQ_WORK  B ON  A.NL_CASE_NO = B.CASE_NO AND B.[CaseProcess_Status] IN ('2','23','24','25')
					           WHERE NL_TYPE IN ('CaseOK' ,'NonCooperatedDone','CaseClosedDone','OtherClosedDone')
					           GROUP BY NL_CASE_NO 
					),
                     RMMCloseFile (CASE_NO,CORP_NO,HCOP_REG_NAME,ContactLog,INCORPORATEDDATE,INCORPORATED,LastUpdateMaker,LastUpdateChecker,LastUpdateDate,NL_TYPE,DataDate) 
                    AS (
						SELECT A.CASE_NO,A.HCOP_HEADQUATERS_CORP_NO as CORP_NO,A.HCOP_REG_NAME,
						(SELECT TOP 1 NL_VALUE FROM NOTELOG WITH(NOLOCK) WHERE NL_CASE_NO = B.CASE_NO  AND NL_TYPE='NonCooperated'  ORDER BY NL_DATETIME DESC) AS 'ContactLog'
						,C.INCORPORATEDDATE,C.INCORPORATED,F.[User_name] as LastUpdateMaker
						,E.[User_name] as LastUpdateChecker,Convert(varchar,D.NL_DateTime,111) as LastUpdateDate,
						CASE NL_TYPE WHEN 'NonCooperatedDone' THEN '不合作' WHEN 'CaseClosedDone' THEN '商店解約' WHEN 'OtherClosedDone'  THEN '其他' ELSE '審查完成' END AS 'NL_TYPE',B.DataDate
	                    FROM AML_HQ_Work A join AML_Edata_Work B ON b.RMMBatchNo = a.HCOP_BATCH_NO and b.AMLInternalID = A.HCOP_INTER_ID and A.CASE_NO = b.CASE_NO
	                    LEFT JOIN  AML_Cdata_Work C on C.CustomerID = b.CustomerID
						JOIN NOTELOG_DISTINCT XL ON XL.CASE_NO = B.CASE_NO 
	                    INNER JOIN NoteLog D  ON D.NL_CASE_NO =XL.CASE_NO AND D.NL_DATETIME = XL.NL_DATETIME
                        LEFT JOIN (select distinct [USER_ID],[User_name] from csip.dbo.M_USER)  E on E.[USER_ID] = D.[NL_USER]  
						LEFT JOIN (select distinct [USER_ID],[User_name] from csip.dbo.M_USER)  F on F.[USER_ID] = A.CASEOWNER_USER   
	                    WHERE B.[HomeBranch] ='0049' 
                    ";
                    ";
        */
        //2021/03/17_Ares_Stanley-DB名稱改為變數
        string sSQL = string.Format(@"    
                     WITH NOTELOG_DISTINCT (CASE_NO,NL_DATETIME)
					AS (
					          SELECT NL_CASE_NO AS CASE_NO,MAX(NL_DATETIME) NL_DATETIME 
					          FROM NOTELOG  A JOIN AML_HQ_WORK  B ON  A.NL_CASE_NO = B.CASE_NO AND B.[CaseProcess_Status] IN ('2','23','24','25')
					           WHERE NL_TYPE IN ('CaseOK' ,'NonCooperatedDone','CaseClosedDone','OtherClosedDone')
					           GROUP BY NL_CASE_NO 
					),
                     RMMCloseFile (CaseExpiryDate, OriginalRiskRanking, HCOP_HEADQUATERS_CORP_NO,CASE_NO,CORP_NO,HCOP_REG_NAME,ContactLog,INCORPORATEDDATE,INCORPORATED,LastUpdateMaker,LastUpdateChecker,LastUpdateDate,NL_TYPE,DataDate) 
                    AS (
						SELECT CASE WHEN A.CaseExpiryDate <> '' THEN A.CaseExpiryDate ELSE dateadd(D, - 1,( DATEADD( M, 5, CONVERT ( datetime, SUBSTRING ( B.DataDate, 1, 6 ) + '01' ) ) ) ) END AS CaseExpiryDate, B.OriginalRiskRanking, A.HCOP_HEADQUATERS_CORP_NO, A.CASE_NO,A.HCOP_HEADQUATERS_CORP_NO as CORP_NO,A.HCOP_REG_NAME,
						CASE WHEN A.CaseProcess_Status = '2' THEN G.SR_Explanation ELSE
						(SELECT TOP 1 NL_VALUE FROM NOTELOG WITH(NOLOCK) WHERE NL_CASE_NO = B.CASE_NO  AND NL_TYPE IN ('NonCooperated','CaseClosed','OtherClosed')  ORDER BY NL_DATETIME DESC) END AS 'ContactLog'
						,C.INCORPORATEDDATE,C.INCORPORATED,F.[User_name] as LastUpdateMaker
						,E.[User_name] as LastUpdateChecker,Convert(varchar,D.NL_DateTime,111) as LastUpdateDate,
						CASE NL_TYPE WHEN 'NonCooperatedDone' THEN '不合作' WHEN 'CaseClosedDone' THEN '商店解約' WHEN 'OtherClosedDone'  THEN '其他' ELSE '審查完成' END AS 'NL_TYPE',B.DataDate
	                    FROM AML_HQ_Work A join AML_Edata_Work B ON b.RMMBatchNo = a.HCOP_BATCH_NO and b.AMLInternalID = A.HCOP_INTER_ID and A.CASE_NO = b.CASE_NO
	                    LEFT JOIN  AML_Cdata_Work C on C.CustomerID = b.CustomerID
						JOIN NOTELOG_DISTINCT XL ON XL.CASE_NO = B.CASE_NO 
	                    INNER JOIN NoteLog D  ON D.NL_CASE_NO =XL.CASE_NO AND D.NL_DATETIME = XL.NL_DATETIME
                        LEFT JOIN (select distinct [USER_ID],[User_name] from {0}.dbo.M_USER)  E on E.[USER_ID] = D.[NL_USER]  
						LEFT JOIN (select distinct [USER_ID],[User_name] from {0}.dbo.M_USER)  F on F.[USER_ID] = A.CASEOWNER_USER   
                        LEFT JOIN SCDDReport G ON G.SR_CASE_NO = B.CASE_NO 
	                    WHERE B.[HomeBranch] ='0049' 
                    ", UtilHelper.GetAppSettings("DB_CSIP"));

        string _Filter = string.Empty;
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;

        //限定為已結案
        //已結案案件增加三種異常結案的案件
        sSQL += " and  [CaseProcess_Status] IN ('2','23','24','25') ";
        sSQL += " and NL_TYPE IN ('CaseOK' ,'NonCooperatedDone','CaseClosedDone','OtherClosedDone')";

        ///加入統編查詢條件
        if (!string.IsNullOrEmpty(parmObj.TaxNo))
        {
            sSQL += " and  A.HCOP_HEADQUATERS_CORP_NO = @TaxNo ";
            sqlcmd.Parameters.Add(new SqlParameter("@TaxNo", parmObj.TaxNo));
        }
        ///加入編號查詢
        if (!string.IsNullOrEmpty(parmObj.CaseNo))
        {
            sSQL += " and  A.CASE_NO = @CaseNo ";
            sqlcmd.Parameters.Add(new SqlParameter("@CaseNo", parmObj.CaseNo));
        }

        //建案日期開始
        if (!string.IsNullOrEmpty(parmObj.yearS) && !string.IsNullOrEmpty(parmObj.MonthS))
        {
            string BegDate = parmObj.yearS + "-" + parmObj.MonthS + "-01";
            if (DataTableConvertor.IsDate(BegDate))
            {
                sSQL += " and  datediff(M,b.DataDate,@BegDate) <= 0  ";
                sqlcmd.Parameters.Add(new SqlParameter("@BegDate", BegDate));
            }
        }
        //建案日期結束
        if (!string.IsNullOrEmpty(parmObj.yearE) && !string.IsNullOrEmpty(parmObj.MonthE))
        {
            string EndDate = parmObj.yearE + "-" + parmObj.MonthE + "-01";
            if (DataTableConvertor.IsDate(EndDate))
            {
                sSQL += " and  datediff(M,b.DataDate,@EndDate) >= 0  ";
                sqlcmd.Parameters.Add(new SqlParameter("@EndDate", EndDate));
            }
        }

        ///加入風險等級
        if (!string.IsNullOrEmpty(parmObj.RiskRanking))
        {
            sSQL += " and  B.OriginalRiskRanking = @OriginalRiskRanking ";
            sqlcmd.Parameters.Add(new SqlParameter("@OriginalRiskRanking", parmObj.RiskRanking));
        }
        ///加入處理經辦
        if (!string.IsNullOrEmpty(parmObj.Owner_User))
        {
            sSQL += " and  A.CaseOwner_User = @CaseOwner_User ";
            sqlcmd.Parameters.Add(new SqlParameter("@CaseOwner_User", parmObj.Owner_User));
        }

        //20191115-RQ-2018-015749-002 修改：新增查詢條件 add by Peggy ↓
        //不合作註記
        if (!string.IsNullOrEmpty(parmObj.IncorporatedFlag))
        {
            if (parmObj.IncorporatedFlag.Trim().Equals("Y"))
            {
                sSQL += " and  C.Incorporated = @Incorporated ";
            }
            else
            {
                sSQL += " and  (ISNULL(C.Incorporated,'') = '' OR C.Incorporated = @Incorporated) ";
            }
            
            sqlcmd.Parameters.Add(new SqlParameter("@Incorporated", parmObj.IncorporatedFlag));
        }

        //結案原因
        if (!string.IsNullOrEmpty(parmObj.CloseType))
        {
            sSQL += " and  A.CaseProcess_Status = @CloseType ";
            sqlcmd.Parameters.Add(new SqlParameter("@CloseType", parmObj.CloseType));
        }

        //結案日期
        if (!string.IsNullOrEmpty(parmObj.CloseDateS))
        {
            sSQL += " and  Convert(varchar(8), D.NL_DATETIME,112) >= @CloseDateS ";
            sqlcmd.Parameters.Add(new SqlParameter("@CloseDateS", parmObj.CloseDateS));
        }
        if (!string.IsNullOrEmpty(parmObj.CloseDateE))
        {
            sSQL += " and   Convert(varchar(8), D.NL_DATETIME,112) <= @CloseDateE ";
            sqlcmd.Parameters.Add(new SqlParameter("@CloseDateE", parmObj.CloseDateE));
        }
        //20191115-RQ-2018-015749-002 修改：新增查詢條件 add by Peggy ↑

        string OrderBy = "";
        if (!string.IsNullOrEmpty(parmObj.OrderBy))
        {
            string OrderAsc = string.IsNullOrEmpty(parmObj.OrderASC) ? "" : parmObj.OrderASC;
            OrderBy = " ORDER BY   " + parmObj.OrderBy + " " + parmObj.OrderASC;
        }
        else
        {
            //加入預設排序
            OrderBy = " ORDER BY LastUpdateDate ";
        }

        //20191216 修改結案報表撈取語法
        // string ROrderBy = @" )
        //             SELECT * 
        //             FROM RMMCloseFile 
        //             WHERE LastUpdateDate IN 
        //             (
        //                 SELECT MAX(LastUpdateDate) as 'NL_DateTime'
        //                 FROM RMMCloseFile
        //                 GROUP BY CASE_NO
        //             )
        //";
        string ROrderBy = @" )
                    SELECT * 
                    FROM RMMCloseFile 
					  ";
        sSQL += ROrderBy + OrderBy;
        sqlcmd.CommandTimeout = 180; //20210521_Ares_Stanley-調整CommandTimeout時間為180秒
        sqlcmd.CommandText = sSQL;
        DataTable dt = new DataTable();
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        DataSet DS = SearchOnDataSet(sqlcmd);
        stopWatch.Stop();
        Logging.Log(string.Format("結案案件列表-列印，耗時： {0} 毫秒", stopWatch.ElapsedMilliseconds)); //20210531_Ares_Stanley-增加結案案件列表列印耗時記錄
        //20210601_Ares_Stanley-增加結案案件列表列印SQL記錄
        #region 取得所有SQL參數
        string sqlParam = "";
        for (int i = 0; i < sqlcmd.Parameters.Count; i++)
        {
            sqlParam += string.Format("參數名：{0} 參數值：{1}；", sqlcmd.Parameters[i].ParameterName, sqlcmd.Parameters[i].Value);
        }
        #endregion
        Logging.Log(string.Format("==========結案案件列表-列印 SQL 語法如下==========\r{0}", sqlcmd.CommandText));
        Logging.Log(string.Format("==========結案案件列表-列印 SQL 參數如下==========\r{0}", sqlParam));
        if (DS != null && DS.Tables.Count > 0)
        {
            dt = DS.Tables[0];
            //20210818_Ares_Stanley-移除登記名稱空白字符
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["HCOP_REG_NAME"] = dt.Rows[i]["HCOP_REG_NAME"].ToString().Trim();
            }
        }

        return dt;

    }

    /// <summary>
    /// 新增多筆
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool Insert(List<EntityAML_HQ_Work> paramObj)
    {
        bool result = false;
        ///使用交易
        using (OMTransactionScope ts = new OMTransactionScope())
        {
            foreach (EntityAML_HQ_Work scOBJ in paramObj)
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
    public static bool Insert(EntityAML_HQ_Work paramObj)
    {
        bool result = false;

        try
        {
            //20191226-RQ-2019-030155-002 新增聯絡電話欄位(HCOP_MOBILE)
            string sSql = @"Insert into AML_HQ_Work
( CASE_NO,FileName,HCOP_BATCH_NO,HCOP_INTER_ID,HCOP_SIXM_TOT_AMT,HCOP_MON_AMT1,HCOP_MON_AMT2,HCOP_MON_AMT3,HCOP_MON_AMT4,HCOP_MON_AMT5,HCOP_MON_AMT6,HCOP_MON_AMT7,HCOP_MON_AMT8,HCOP_MON_AMT9,HCOP_MON_AMT10,HCOP_MON_AMT11,HCOP_MON_AMT12,HCOP_KEY,HCOP_HEADQUATERS_CORP_NO,HCOP_HEADQUATERS_CORP_SEQ,HCOP_CORP_TYPE,HCOP_REGISTER_NATION,HCOP_CORP_REG_ENG_NAME,HCOP_REG_NAME,HCOP_NAME_0E,HCOP_NAME_CHI,HCOP_NAME_0F,HCOP_BUILD_DATE,HCOP_CC,HCOP_REG_CITY,HCOP_REG_ADDR1,HCOP_REG_ADDR2,HCOP_REG_ZIP_CODE,HCOP_EMAIL,HCOP_NP_COMPANY_NAME,HCOP_OWNER_NATION,HCOP_OWNER_CHINESE_NAME,HCOP_OWNER_ENGLISH_NAME,HCOP_OWNER_BIRTH_DATE,HCOP_OWNER_ID,HCOP_OWNER_ID_ISSUE_DATE,HCOP_OWNER_ID_ISSUE_PLACE,HCOP_OWNER_ID_REPLACE_TYPE,HCOP_ID_PHOTO_FLAG,HCOP_PASSPORT,HCOP_PASSPORT_EXP_DATE,HCOP_RESIDENT_NO,HCOP_RESIDENT_EXP_DATE,HCOP_OTHER_CERT,HCOP_OTHER_CERT_EXP_DATE,HCOP_COMPLEX_STR_CODE,HCOP_ISSUE_STOCK_FLAG,HCOP_COMP_TEL,HCOP_MAILING_CITY,HCOP_MAILING_ADDR1,HCOP_MAILING_ADDR2,HCOP_PRIMARY_BUSI_COUNTRY,HCOP_BUSI_RISK_NATION_FLAG,HCOP_BUSI_RISK_NATION_1,HCOP_BUSI_RISK_NATION_2,HCOP_BUSI_RISK_NATION_3,HCOP_BUSI_RISK_NATION_4,HCOP_BUSI_RISK_NATION_5,HCOP_OVERSEAS_FOREIGN,HCOP_OVERSEAS_FOREIGN_COUNTRY,HCOP_REGISTER_US_STATE,HCOP_BUSINESS_ORGAN_TYPE,HCOP_CREATE_DATE,HCOP_STATUS,HCOP_QUALIFY_FLAG,HCOP_CONTACT_NAME,HCOP_EXAMINE_FLAG,HCOP_ALLOW_ISSUE_STOCK_FLAG,HCOP_CONTACT_TEL,HCOP_UPDATE_DATE,HCOP_CREATE_ID,HCOP_UPDATE_ID,HCOP_OWNER_CITY,HCOP_OWNER_ADDR1,HCOP_OWNER_ADDR2,HCOP_RESERVED_FILLER,Create_Time,Create_User,Create_Date,CaseExpiryDate,ReviewCompletedDate,SendLetter_NotCooperating,OWNER_ID_Type,OWNER_ID_SreachStatus,CaseOwner_User,CaseProcess_Status,CaseProcess_User,AML_ExportFileFlag,HCOP_OWNER_CHINESE_LNAME,HCOP_OWNER_ROMA,HCOP_CONTACT_LNAME,HCOP_CONTACT_ROMA,HCOP_MOBILE, HCOP_CC_2, HCOP_CC_3, HCOP_OC, HCOP_INCOME_SOURCE, HCOP_LAST_UPD_MAKER, HCOP_LAST_UPD_CHECKER, HCOP_LAST_UPD_BRANCH, HCOP_LAST_UPDATE_DATE, HCOP_COUNTRY_CODE_2, HCOP_GENDER)
VALUES( @CASE_NO,@FileName,@HCOP_BATCH_NO,@HCOP_INTER_ID,@HCOP_SIXM_TOT_AMT,@HCOP_MON_AMT1,@HCOP_MON_AMT2,@HCOP_MON_AMT3,@HCOP_MON_AMT4,@HCOP_MON_AMT5,@HCOP_MON_AMT6,@HCOP_MON_AMT7,@HCOP_MON_AMT8,@HCOP_MON_AMT9,@HCOP_MON_AMT10,@HCOP_MON_AMT11,@HCOP_MON_AMT12,@HCOP_KEY,@HCOP_HEADQUATERS_CORP_NO,@HCOP_HEADQUATERS_CORP_SEQ,@HCOP_CORP_TYPE,@HCOP_REGISTER_NATION,@HCOP_CORP_REG_ENG_NAME,@HCOP_REG_NAME,@HCOP_NAME_0E,@HCOP_NAME_CHI,@HCOP_NAME_0F,@HCOP_BUILD_DATE,@HCOP_CC,@HCOP_REG_CITY,@HCOP_REG_ADDR1,@HCOP_REG_ADDR2,@HCOP_REG_ZIP_CODE,@HCOP_EMAIL,@HCOP_NP_COMPANY_NAME,@HCOP_OWNER_NATION,@HCOP_OWNER_CHINESE_NAME,@HCOP_OWNER_ENGLISH_NAME,@HCOP_OWNER_BIRTH_DATE,@HCOP_OWNER_ID,@HCOP_OWNER_ID_ISSUE_DATE,@HCOP_OWNER_ID_ISSUE_PLACE,@HCOP_OWNER_ID_REPLACE_TYPE,@HCOP_ID_PHOTO_FLAG,@HCOP_PASSPORT,@HCOP_PASSPORT_EXP_DATE,@HCOP_RESIDENT_NO,@HCOP_RESIDENT_EXP_DATE,@HCOP_OTHER_CERT,@HCOP_OTHER_CERT_EXP_DATE,@HCOP_COMPLEX_STR_CODE,@HCOP_ISSUE_STOCK_FLAG,@HCOP_COMP_TEL,@HCOP_MAILING_CITY,@HCOP_MAILING_ADDR1,@HCOP_MAILING_ADDR2,@HCOP_PRIMARY_BUSI_COUNTRY,@HCOP_BUSI_RISK_NATION_FLAG,@HCOP_BUSI_RISK_NATION_1,@HCOP_BUSI_RISK_NATION_2,@HCOP_BUSI_RISK_NATION_3,@HCOP_BUSI_RISK_NATION_4,@HCOP_BUSI_RISK_NATION_5,@HCOP_OVERSEAS_FOREIGN,@HCOP_OVERSEAS_FOREIGN_COUNTRY,@HCOP_REGISTER_US_STATE,@HCOP_BUSINESS_ORGAN_TYPE,@HCOP_CREATE_DATE,@HCOP_STATUS,@HCOP_QUALIFY_FLAG,@HCOP_CONTACT_NAME,@HCOP_EXAMINE_FLAG,@HCOP_ALLOW_ISSUE_STOCK_FLAG,@HCOP_CONTACT_TEL,@HCOP_UPDATE_DATE,@HCOP_CREATE_ID,@HCOP_UPDATE_ID,@HCOP_OWNER_CITY,@HCOP_OWNER_ADDR1,@HCOP_OWNER_ADDR2,@HCOP_RESERVED_FILLER,@Create_Time,@Create_User,@Create_Date,@CaseExpiryDate,@ReviewCompletedDate,@SendLetter_NotCooperating,@OWNER_ID_Type,@OWNER_ID_SreachStatus,@CaseOwner_User,@CaseProcess_Status,@CaseProcess_User,@AML_ExportFileFlag,@HCOP_OWNER_CHINESE_LNAME,@HCOP_OWNER_ROMA,@HCOP_CONTACT_LNAME,@HCOP_CONTACT_ROMA,@HCOP_MOBILE, @HCOP_CC_2, @HCOP_CC_3, @HCOP_OC, @HCOP_INCOME_SOURCE, @HCOP_LAST_UPD_MAKER, @HCOP_LAST_UPD_CHECKER, @HCOP_LAST_UPD_BRANCH, @HCOP_LAST_UPDATE_DATE, @HCOP_COUNTRY_CODE_2, @HCOP_GENDER);
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
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MON_AMT1", paramObj.HCOP_MON_AMT1));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MON_AMT2", paramObj.HCOP_MON_AMT2));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MON_AMT3", paramObj.HCOP_MON_AMT3));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MON_AMT4", paramObj.HCOP_MON_AMT4));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MON_AMT5", paramObj.HCOP_MON_AMT5));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MON_AMT6", paramObj.HCOP_MON_AMT6));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MON_AMT7", paramObj.HCOP_MON_AMT7));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MON_AMT8", paramObj.HCOP_MON_AMT8));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MON_AMT9", paramObj.HCOP_MON_AMT9));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MON_AMT10", paramObj.HCOP_MON_AMT10));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MON_AMT11", paramObj.HCOP_MON_AMT11));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MON_AMT12", paramObj.HCOP_MON_AMT12));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_KEY", paramObj.HCOP_KEY));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_HEADQUATERS_CORP_NO", paramObj.HCOP_HEADQUATERS_CORP_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_HEADQUATERS_CORP_SEQ", paramObj.HCOP_HEADQUATERS_CORP_SEQ));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CORP_TYPE", paramObj.HCOP_CORP_TYPE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REGISTER_NATION", paramObj.HCOP_REGISTER_NATION));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CORP_REG_ENG_NAME", paramObj.HCOP_CORP_REG_ENG_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REG_NAME", paramObj.HCOP_REG_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_NAME_0E", paramObj.HCOP_NAME_0E));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_NAME_CHI", paramObj.HCOP_NAME_CHI));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_NAME_0F", paramObj.HCOP_NAME_0F));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BUILD_DATE", paramObj.HCOP_BUILD_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CC", paramObj.HCOP_CC));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REG_CITY", paramObj.HCOP_REG_CITY));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REG_ADDR1", paramObj.HCOP_REG_ADDR1));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REG_ADDR2", paramObj.HCOP_REG_ADDR2));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REG_ZIP_CODE", paramObj.HCOP_REG_ZIP_CODE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_EMAIL", paramObj.HCOP_EMAIL));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_NP_COMPANY_NAME", paramObj.HCOP_NP_COMPANY_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_NATION", paramObj.HCOP_OWNER_NATION));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_CHINESE_NAME", paramObj.HCOP_OWNER_CHINESE_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ENGLISH_NAME", paramObj.HCOP_OWNER_ENGLISH_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_BIRTH_DATE", paramObj.HCOP_OWNER_BIRTH_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ID", paramObj.HCOP_OWNER_ID));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ID_ISSUE_DATE", paramObj.HCOP_OWNER_ID_ISSUE_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ID_ISSUE_PLACE", paramObj.HCOP_OWNER_ID_ISSUE_PLACE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ID_REPLACE_TYPE", paramObj.HCOP_OWNER_ID_REPLACE_TYPE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_ID_PHOTO_FLAG", paramObj.HCOP_ID_PHOTO_FLAG));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_PASSPORT", paramObj.HCOP_PASSPORT));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_PASSPORT_EXP_DATE", paramObj.HCOP_PASSPORT_EXP_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_RESIDENT_NO", paramObj.HCOP_RESIDENT_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_RESIDENT_EXP_DATE", paramObj.HCOP_RESIDENT_EXP_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OTHER_CERT", paramObj.HCOP_OTHER_CERT));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OTHER_CERT_EXP_DATE", paramObj.HCOP_OTHER_CERT_EXP_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_COMPLEX_STR_CODE", paramObj.HCOP_COMPLEX_STR_CODE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_ISSUE_STOCK_FLAG", paramObj.HCOP_ISSUE_STOCK_FLAG));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_COMP_TEL", paramObj.HCOP_COMP_TEL));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MAILING_CITY", paramObj.HCOP_MAILING_CITY));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MAILING_ADDR1", paramObj.HCOP_MAILING_ADDR1));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MAILING_ADDR2", paramObj.HCOP_MAILING_ADDR2));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_PRIMARY_BUSI_COUNTRY", paramObj.HCOP_PRIMARY_BUSI_COUNTRY));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BUSI_RISK_NATION_FLAG", paramObj.HCOP_BUSI_RISK_NATION_FLAG));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BUSI_RISK_NATION_1", paramObj.HCOP_BUSI_RISK_NATION_1));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BUSI_RISK_NATION_2", paramObj.HCOP_BUSI_RISK_NATION_2));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BUSI_RISK_NATION_3", paramObj.HCOP_BUSI_RISK_NATION_3));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BUSI_RISK_NATION_4", paramObj.HCOP_BUSI_RISK_NATION_4));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BUSI_RISK_NATION_5", paramObj.HCOP_BUSI_RISK_NATION_5));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OVERSEAS_FOREIGN", paramObj.HCOP_OVERSEAS_FOREIGN));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OVERSEAS_FOREIGN_COUNTRY", paramObj.HCOP_OVERSEAS_FOREIGN_COUNTRY));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REGISTER_US_STATE", paramObj.HCOP_REGISTER_US_STATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BUSINESS_ORGAN_TYPE", paramObj.HCOP_BUSINESS_ORGAN_TYPE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CREATE_DATE", paramObj.HCOP_CREATE_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_STATUS", paramObj.HCOP_STATUS));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_QUALIFY_FLAG", paramObj.HCOP_QUALIFY_FLAG));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CONTACT_NAME", paramObj.HCOP_CONTACT_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_EXAMINE_FLAG", paramObj.HCOP_EXAMINE_FLAG));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_ALLOW_ISSUE_STOCK_FLAG", paramObj.HCOP_ALLOW_ISSUE_STOCK_FLAG));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CONTACT_TEL", paramObj.HCOP_CONTACT_TEL));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_UPDATE_DATE", paramObj.HCOP_UPDATE_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CREATE_ID", paramObj.HCOP_CREATE_ID));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_UPDATE_ID", paramObj.HCOP_UPDATE_ID));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_CITY", paramObj.HCOP_OWNER_CITY));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ADDR1", paramObj.HCOP_OWNER_ADDR1));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ADDR2", paramObj.HCOP_OWNER_ADDR2));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_RESERVED_FILLER", paramObj.HCOP_RESERVED_FILLER));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_Time", paramObj.Create_Time));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_User", paramObj.Create_User));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_Date", paramObj.Create_Date));
            sqlcmd.Parameters.Add(new SqlParameter("@CaseExpiryDate", paramObj.CaseExpiryDate));
            sqlcmd.Parameters.Add(new SqlParameter("@ReviewCompletedDate", paramObj.ReviewCompletedDate));
            sqlcmd.Parameters.Add(new SqlParameter("@SendLetter_NotCooperating", paramObj.SendLetter_NotCooperating));
            sqlcmd.Parameters.Add(new SqlParameter("@OWNER_ID_Type", paramObj.OWNER_ID_Type));
            sqlcmd.Parameters.Add(new SqlParameter("@OWNER_ID_SreachStatus", paramObj.OWNER_ID_SreachStatus));
            sqlcmd.Parameters.Add(new SqlParameter("@CaseOwner_User", paramObj.CaseOwner_User));
            sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_Status", paramObj.CaseProcess_Status));
            sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User", paramObj.CaseProcess_User));
            sqlcmd.Parameters.Add(new SqlParameter("@AML_ExportFileFlag", paramObj.AML_ExportFileFlag));
            //sqlcmd.Parameters.Add(new SqlParameter("@AML_LastExportTime", paramObj.AML_LastExportTime));

            //20191113-add by Peggy ,HCOP_OWNER_CHINESE_LNAME,HCOP_OWNER_ROMA,HCOP_CONTACT_LNAME,HCOP_CONTACT_ROMA
            //長姓名
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_CHINESE_LNAME", paramObj.HCOP_OWNER_CHINESE_LNAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ROMA", paramObj.HCOP_OWNER_ROMA));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CONTACT_LNAME", paramObj.HCOP_CONTACT_LNAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CONTACT_ROMA", paramObj.HCOP_CONTACT_ROMA));

            //20191226-RQ-2019-030155-002 新增聯絡電話欄位(HCOP_MOBILE)
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MOBILE", paramObj.HCOP_MOBILE));

            #region 20220107_Ares_Jack_自然人專屬欄位
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CC_2", paramObj.HCOP_CC_2));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CC_3", paramObj.HCOP_CC_3));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OC", paramObj.HCOP_OC));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_INCOME_SOURCE", paramObj.HCOP_INCOME_SOURCE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_COUNTRY_CODE_2", paramObj.HCOP_COUNTRY_CODE_2));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_GENDER", paramObj.HCOP_GENDER));
            #endregion
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_LAST_UPD_MAKER", paramObj.HCOP_LAST_UPD_MAKER));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_LAST_UPD_CHECKER", paramObj.HCOP_LAST_UPD_CHECKER));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_LAST_UPD_BRANCH", paramObj.HCOP_LAST_UPD_BRANCH));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_LAST_UPDATE_DATE", paramObj.HCOP_LAST_UPDATE_DATE));
            result = Add(sqlcmd);

        }
        catch (Exception ex)
        {
            //  string ms = ex.Message;
            Logging.Log(ex);
        }
        finally
        {
            // Close the connection.

        }
        return result;
    }
    /// <summary>
    /// 新增單筆
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool UpdateEdit(EntityAML_HQ_Work paramObj)
    {
        bool result = false;
        try
        {
            //20191226-RQ-2019-030155-002 新增聯絡電話欄位(HCOP_MOBILE)
            //僅更新電文帶回的來值，其他欄位不動，  
            string sSql = @"update AML_HQ_Work 
                       set HCOP_HEADQUATERS_CORP_NO = @HCOP_HEADQUATERS_CORP_NO,HCOP_HEADQUATERS_CORP_SEQ = @HCOP_HEADQUATERS_CORP_SEQ,HCOP_CORP_TYPE = @HCOP_CORP_TYPE,HCOP_REGISTER_NATION = @HCOP_REGISTER_NATION,HCOP_CORP_REG_ENG_NAME = @HCOP_CORP_REG_ENG_NAME,HCOP_REG_NAME = @HCOP_REG_NAME,HCOP_BUILD_DATE = @HCOP_BUILD_DATE,HCOP_CC = @HCOP_CC,HCOP_REG_CITY = @HCOP_REG_CITY,HCOP_REG_ADDR1 = @HCOP_REG_ADDR1,HCOP_REG_ADDR2 = @HCOP_REG_ADDR2,HCOP_EMAIL = @HCOP_EMAIL,HCOP_NP_COMPANY_NAME = @HCOP_NP_COMPANY_NAME,
                        HCOP_OWNER_NATION = @HCOP_OWNER_NATION,HCOP_OWNER_CHINESE_NAME = @HCOP_OWNER_CHINESE_NAME,HCOP_OWNER_ENGLISH_NAME = @HCOP_OWNER_ENGLISH_NAME,HCOP_OWNER_BIRTH_DATE = @HCOP_OWNER_BIRTH_DATE,HCOP_OWNER_ID = @HCOP_OWNER_ID,HCOP_OWNER_ID_ISSUE_DATE = @HCOP_OWNER_ID_ISSUE_DATE,HCOP_OWNER_ID_ISSUE_PLACE = @HCOP_OWNER_ID_ISSUE_PLACE,HCOP_OWNER_ID_REPLACE_TYPE = @HCOP_OWNER_ID_REPLACE_TYPE,HCOP_ID_PHOTO_FLAG = @HCOP_ID_PHOTO_FLAG,HCOP_PASSPORT = @HCOP_PASSPORT,HCOP_PASSPORT_EXP_DATE = @HCOP_PASSPORT_EXP_DATE,HCOP_RESIDENT_NO = @HCOP_RESIDENT_NO,HCOP_RESIDENT_EXP_DATE = @HCOP_RESIDENT_EXP_DATE,HCOP_OTHER_CERT = @HCOP_OTHER_CERT,HCOP_OTHER_CERT_EXP_DATE = @HCOP_OTHER_CERT_EXP_DATE,HCOP_COMPLEX_STR_CODE = @HCOP_COMPLEX_STR_CODE,HCOP_ISSUE_STOCK_FLAG = @HCOP_ISSUE_STOCK_FLAG,HCOP_COMP_TEL = @HCOP_COMP_TEL,HCOP_MAILING_CITY = @HCOP_MAILING_CITY,HCOP_MAILING_ADDR1 = @HCOP_MAILING_ADDR1,HCOP_MAILING_ADDR2 = @HCOP_MAILING_ADDR2,HCOP_PRIMARY_BUSI_COUNTRY = @HCOP_PRIMARY_BUSI_COUNTRY,HCOP_BUSI_RISK_NATION_FLAG = @HCOP_BUSI_RISK_NATION_FLAG,HCOP_BUSI_RISK_NATION_1 = @HCOP_BUSI_RISK_NATION_1,HCOP_BUSI_RISK_NATION_2 = @HCOP_BUSI_RISK_NATION_2,HCOP_BUSI_RISK_NATION_3 = @HCOP_BUSI_RISK_NATION_3,HCOP_BUSI_RISK_NATION_4 = @HCOP_BUSI_RISK_NATION_4,HCOP_BUSI_RISK_NATION_5 = @HCOP_BUSI_RISK_NATION_5,HCOP_OVERSEAS_FOREIGN = @HCOP_OVERSEAS_FOREIGN,HCOP_OVERSEAS_FOREIGN_COUNTRY = @HCOP_OVERSEAS_FOREIGN_COUNTRY,HCOP_REGISTER_US_STATE = @HCOP_REGISTER_US_STATE,HCOP_BUSINESS_ORGAN_TYPE = @HCOP_BUSINESS_ORGAN_TYPE,HCOP_CREATE_DATE = @HCOP_CREATE_DATE,HCOP_STATUS = @HCOP_STATUS,HCOP_OWNER_CITY = @HCOP_OWNER_CITY,HCOP_OWNER_ADDR1 = @HCOP_OWNER_ADDR1,HCOP_OWNER_ADDR2 = @HCOP_OWNER_ADDR2,Create_Time = @Create_Time,Create_User = @Create_User,Create_Date = @Create_Date,OWNER_ID_SreachStatus = @OWNER_ID_SreachStatus 
                        ,HCOP_CONTACT_NAME = @HCOP_CONTACT_NAME, HCOP_CONTACT_TEL=@HCOP_CONTACT_TEL,HCOP_OWNER_CHINESE_LNAME = @HCOP_OWNER_CHINESE_LNAME,HCOP_OWNER_ROMA = @HCOP_OWNER_ROMA,HCOP_CONTACT_LNAME = @HCOP_CONTACT_LNAME,HCOP_CONTACT_ROMA = @HCOP_CONTACT_ROMA
                        ,HCOP_MOBILE = @HCOP_MOBILE,HCOP_ALLOW_ISSUE_STOCK_FLAG = @HCOP_ALLOW_ISSUE_STOCK_FLAG
                        where ID = @ID;
                       ";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sSql;
            sqlcmd.Parameters.Add(new SqlParameter("@ID", paramObj.ID));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_HEADQUATERS_CORP_NO", paramObj.HCOP_HEADQUATERS_CORP_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_HEADQUATERS_CORP_SEQ", paramObj.HCOP_HEADQUATERS_CORP_SEQ));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CORP_TYPE", paramObj.HCOP_CORP_TYPE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REGISTER_NATION", paramObj.HCOP_REGISTER_NATION));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CORP_REG_ENG_NAME", paramObj.HCOP_CORP_REG_ENG_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REG_NAME", paramObj.HCOP_REG_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BUILD_DATE", paramObj.HCOP_BUILD_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CC", paramObj.HCOP_CC));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REG_CITY", paramObj.HCOP_REG_CITY));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REG_ADDR1", paramObj.HCOP_REG_ADDR1));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REG_ADDR2", paramObj.HCOP_REG_ADDR2));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_EMAIL", paramObj.HCOP_EMAIL));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_NP_COMPANY_NAME", paramObj.HCOP_NP_COMPANY_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_NATION", paramObj.HCOP_OWNER_NATION));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_CHINESE_NAME", paramObj.HCOP_OWNER_CHINESE_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ENGLISH_NAME", paramObj.HCOP_OWNER_ENGLISH_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_BIRTH_DATE", paramObj.HCOP_OWNER_BIRTH_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ID", paramObj.HCOP_OWNER_ID));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ID_ISSUE_DATE", paramObj.HCOP_OWNER_ID_ISSUE_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ID_ISSUE_PLACE", paramObj.HCOP_OWNER_ID_ISSUE_PLACE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ID_REPLACE_TYPE", paramObj.HCOP_OWNER_ID_REPLACE_TYPE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_ID_PHOTO_FLAG", paramObj.HCOP_ID_PHOTO_FLAG));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_PASSPORT", paramObj.HCOP_PASSPORT));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_PASSPORT_EXP_DATE", paramObj.HCOP_PASSPORT_EXP_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_RESIDENT_NO", paramObj.HCOP_RESIDENT_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_RESIDENT_EXP_DATE", paramObj.HCOP_RESIDENT_EXP_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OTHER_CERT", paramObj.HCOP_OTHER_CERT));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OTHER_CERT_EXP_DATE", paramObj.HCOP_OTHER_CERT_EXP_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_COMPLEX_STR_CODE", paramObj.HCOP_COMPLEX_STR_CODE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_ISSUE_STOCK_FLAG", paramObj.HCOP_ISSUE_STOCK_FLAG));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_COMP_TEL", paramObj.HCOP_COMP_TEL));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MAILING_CITY", paramObj.HCOP_MAILING_CITY));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MAILING_ADDR1", paramObj.HCOP_MAILING_ADDR1));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MAILING_ADDR2", paramObj.HCOP_MAILING_ADDR2));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_PRIMARY_BUSI_COUNTRY", paramObj.HCOP_PRIMARY_BUSI_COUNTRY));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BUSI_RISK_NATION_FLAG", paramObj.HCOP_BUSI_RISK_NATION_FLAG));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BUSI_RISK_NATION_1", paramObj.HCOP_BUSI_RISK_NATION_1));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BUSI_RISK_NATION_2", paramObj.HCOP_BUSI_RISK_NATION_2));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BUSI_RISK_NATION_3", paramObj.HCOP_BUSI_RISK_NATION_3));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BUSI_RISK_NATION_4", paramObj.HCOP_BUSI_RISK_NATION_4));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BUSI_RISK_NATION_5", paramObj.HCOP_BUSI_RISK_NATION_5));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OVERSEAS_FOREIGN", paramObj.HCOP_OVERSEAS_FOREIGN));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OVERSEAS_FOREIGN_COUNTRY", paramObj.HCOP_OVERSEAS_FOREIGN_COUNTRY));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REGISTER_US_STATE", paramObj.HCOP_REGISTER_US_STATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BUSINESS_ORGAN_TYPE", paramObj.HCOP_BUSINESS_ORGAN_TYPE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CREATE_DATE", paramObj.HCOP_CREATE_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_STATUS", paramObj.HCOP_STATUS));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_CITY", paramObj.HCOP_OWNER_CITY));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ADDR1", paramObj.HCOP_OWNER_ADDR1));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ADDR2", paramObj.HCOP_OWNER_ADDR2));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CONTACT_NAME", paramObj.HCOP_CONTACT_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CONTACT_TEL", paramObj.HCOP_CONTACT_TEL));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_Time", paramObj.Create_Time));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_User", paramObj.Create_User));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_Date", paramObj.Create_Date));
            sqlcmd.Parameters.Add(new SqlParameter("@OWNER_ID_SreachStatus", paramObj.OWNER_ID_SreachStatus));

            //長姓名
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_CHINESE_LNAME", paramObj.HCOP_OWNER_CHINESE_LNAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ROMA", paramObj.HCOP_OWNER_ROMA));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CONTACT_LNAME", paramObj.HCOP_CONTACT_LNAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CONTACT_ROMA", paramObj.HCOP_CONTACT_ROMA));

            //20191226-RQ-2019-030155-002 新增聯絡電話欄位(HCOP_MOBILE)
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MOBILE", paramObj.HCOP_MOBILE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_ALLOW_ISSUE_STOCK_FLAG", paramObj.HCOP_ALLOW_ISSUE_STOCK_FLAG));
            //收集PARAMDEBUG
            //StringBuilder sb = new StringBuilder();
            //foreach (SqlParameter oitm in sqlcmd.Parameters)
            //{
            //    sb.AppendLine(string.Format(@"DECLARE {0} VARCHAR(255); set {0} = '{1}' ;", oitm.ParameterName, oitm.Value));

            //}
            //string df = sb.ToString();

            //組出高階經理人用sqlCommand
            List<SqlCommand> mangColl = getMangEditCommand(paramObj.ManagerColl);
            ///使用交易
            using (OMTransactionScope ts = new OMTransactionScope())
            {
                result = Update(sqlcmd);
                if (!result)
                {
                    return false;
                }
                //開始寫入
                foreach (SqlCommand scmd in mangColl)
                {
                    result = Update(scmd);
                    if (!result)
                    {
                        return false;
                    }
                }
                ts.Complete();
            }

        }
        catch (Exception ex)
        {
            //string ms = ex.Message;
            Logging.Log(ex);
        }
        finally
        {
            // Close the connection.

        }
        return result;
    }
    //20210806 EOS_AML(NOVA) 自然人審查維護 by Ares Dennis
    /// <summary>
    /// 自然人收單維護審查 更新資料
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool UpdateEdit_Natural(EntityAML_HQ_Work paramObj)
    {
        bool result = false;
        try
        {            
            //僅更新電文帶回的來值，其他欄位不動，  
            string sSql = @"update AML_HQ_Work 
                       set HCOP_REG_NAME = @HCOP_REG_NAME,HCOP_CC = @HCOP_CC,HCOP_REG_CITY = @HCOP_REG_CITY,HCOP_REG_ADDR1 = @HCOP_REG_ADDR1,HCOP_REG_ADDR2 = @HCOP_REG_ADDR2,HCOP_EMAIL = @HCOP_EMAIL,HCOP_NP_COMPANY_NAME = @HCOP_NP_COMPANY_NAME
                        ,HCOP_OWNER_NATION = @HCOP_OWNER_NATION,HCOP_OWNER_CHINESE_NAME = @HCOP_OWNER_CHINESE_NAME,HCOP_OWNER_ENGLISH_NAME = @HCOP_OWNER_ENGLISH_NAME,HCOP_OWNER_BIRTH_DATE = @HCOP_OWNER_BIRTH_DATE,HCOP_OWNER_ID = @HCOP_OWNER_ID,HCOP_OWNER_ID_ISSUE_DATE = @HCOP_OWNER_ID_ISSUE_DATE,HCOP_OWNER_ID_ISSUE_PLACE = @HCOP_OWNER_ID_ISSUE_PLACE,HCOP_OWNER_ID_REPLACE_TYPE = @HCOP_OWNER_ID_REPLACE_TYPE,HCOP_ID_PHOTO_FLAG = @HCOP_ID_PHOTO_FLAG,HCOP_MAILING_CITY = @HCOP_MAILING_CITY,HCOP_MAILING_ADDR1 = @HCOP_MAILING_ADDR1,HCOP_MAILING_ADDR2 = @HCOP_MAILING_ADDR2,HCOP_REGISTER_US_STATE = @HCOP_REGISTER_US_STATE
                        ,HCOP_CONTACT_TEL=@HCOP_CONTACT_TEL, HCOP_COMP_TEL=@HCOP_COMP_TEL, HCOP_STATUS=@HCOP_STATUS 
                        ,HCOP_MOBILE = @HCOP_MOBILE
                        ,HCOP_CC_2 = @HCOP_CC_2, HCOP_CC_3 = @HCOP_CC_3, HCOP_OC = @HCOP_OC, HCOP_INCOME_SOURCE = @HCOP_INCOME_SOURCE, HCOP_COUNTRY_CODE_2 = @HCOP_COUNTRY_CODE_2, HCOP_GENDER = @HCOP_GENDER, HCOP_REG_ZIP_CODE = @HCOP_REG_ZIP_CODE 
                        where ID = @ID;
                       ";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sSql;
            sqlcmd.Parameters.Add(new SqlParameter("@ID", paramObj.ID));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REG_NAME", paramObj.HCOP_REG_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CC", paramObj.HCOP_CC));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REG_CITY", paramObj.HCOP_REG_CITY));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REG_ADDR1", paramObj.HCOP_REG_ADDR1));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REG_ADDR2", paramObj.HCOP_REG_ADDR2));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_EMAIL", paramObj.HCOP_EMAIL));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_NP_COMPANY_NAME", paramObj.HCOP_NP_COMPANY_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_NATION", paramObj.HCOP_OWNER_NATION));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_CHINESE_NAME", paramObj.HCOP_OWNER_CHINESE_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ENGLISH_NAME", paramObj.HCOP_OWNER_ENGLISH_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_BIRTH_DATE", paramObj.HCOP_OWNER_BIRTH_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ID", paramObj.HCOP_OWNER_ID));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ID_ISSUE_DATE", paramObj.HCOP_OWNER_ID_ISSUE_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ID_ISSUE_PLACE", paramObj.HCOP_OWNER_ID_ISSUE_PLACE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ID_REPLACE_TYPE", paramObj.HCOP_OWNER_ID_REPLACE_TYPE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_ID_PHOTO_FLAG", paramObj.HCOP_ID_PHOTO_FLAG));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_COMP_TEL", paramObj.HCOP_COMP_TEL));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MAILING_CITY", paramObj.HCOP_MAILING_CITY));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MAILING_ADDR1", paramObj.HCOP_MAILING_ADDR1));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MAILING_ADDR2", paramObj.HCOP_MAILING_ADDR2));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REGISTER_US_STATE", paramObj.HCOP_REGISTER_US_STATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_STATUS", paramObj.HCOP_STATUS));//20211208_Ares_Jack_新增欄位 商店狀態
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CONTACT_TEL", paramObj.HCOP_CONTACT_TEL));
            ////20191226-RQ-2019-030155-002 新增聯絡電話欄位(HCOP_MOBILE)
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MOBILE", paramObj.HCOP_MOBILE));
            //20210806 EOS_AML(NOVA) 自然人審查維護 新增欄位 by Ares Dennis
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CC_2", paramObj.HCOP_CC_2));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CC_3", paramObj.HCOP_CC_3));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OC", paramObj.HCOP_OC));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_INCOME_SOURCE", paramObj.HCOP_INCOME_SOURCE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_COUNTRY_CODE_2", paramObj.HCOP_COUNTRY_CODE_2));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_GENDER", paramObj.HCOP_GENDER));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REG_ZIP_CODE", paramObj.HCOP_REG_ZIP_CODE));

            ////組出高階經理人用sqlCommand
            //List<SqlCommand> mangColl = getMangEditCommand(paramObj.ManagerColl);
            ///使用交易
            using (OMTransactionScope ts = new OMTransactionScope())
            {
                result = Update(sqlcmd);
                if (!result)
                {
                    return false;
                }
                ts.Complete();
            }

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
    /// 依傳入的高級經理人，製成預備執行的SqlCommand
    /// </summary>
    /// <param name="managerColl"></param>
    /// <returns></returns>
    private static List<SqlCommand> getMangEditCommand(List<EntityAML_HQ_Manager_Work> managerColl)
    {
        List<SqlCommand> rtn = new List<SqlCommand>();
        foreach (EntityAML_HQ_Manager_Work paramObj in managerColl)
        {
            string sSQL = "";
            if (string.IsNullOrEmpty(paramObj.ID)) //新增
            {
                sSQL = @"Insert into AML_HQ_Manager_Work
                  (CASE_NO, FileName, HCOP_BATCH_NO, HCOP_INTER_ID, HCOP_SIXM_TOT_AMT, HCOP_KEY, HCOP_BENE_NATION, HCOP_BENE_NAME, HCOP_BENE_BIRTH_DATE, HCOP_BENE_ID, HCOP_BENE_PASSPORT, HCOP_BENE_PASSPORT_EXP, HCOP_BENE_RESIDENT_NO, HCOP_BENE_RESIDENT_EXP, HCOP_BENE_OTH_CERT, HCOP_BENE_OTH_CERT_EXP, HCOP_BENE_JOB_TYPE, HCOP_BENE_JOB_TYPE_2, HCOP_BENE_JOB_TYPE_3, HCOP_BENE_JOB_TYPE_4, HCOP_BENE_JOB_TYPE_5, HCOP_BENE_JOB_TYPE_6, HCOP_BENE_RESERVED,Create_Date, Create_Time, Create_User,HCOP_BENE_LNAME,HCOP_BENE_ROMA)
                VALUES(@CASE_NO, @FileName, @HCOP_BATCH_NO, @HCOP_INTER_ID, @HCOP_SIXM_TOT_AMT, @HCOP_KEY, @HCOP_BENE_NATION, @HCOP_BENE_NAME, @HCOP_BENE_BIRTH_DATE, @HCOP_BENE_ID, @HCOP_BENE_PASSPORT, @HCOP_BENE_PASSPORT_EXP, @HCOP_BENE_RESIDENT_NO, @HCOP_BENE_RESIDENT_EXP, @HCOP_BENE_OTH_CERT, @HCOP_BENE_OTH_CERT_EXP, @HCOP_BENE_JOB_TYPE, @HCOP_BENE_JOB_TYPE_2, @HCOP_BENE_JOB_TYPE_3, @HCOP_BENE_JOB_TYPE_4, @HCOP_BENE_JOB_TYPE_5, @HCOP_BENE_JOB_TYPE_6, @HCOP_BENE_RESERVED,@Create_Date, @Create_Time, @Create_User,@HCOP_BENE_LNAME,@HCOP_BENE_ROMA); 
                ";
            }
            else
            {
                sSQL = @" update AML_HQ_Manager_Work
                    set HCOP_BENE_NATION = @HCOP_BENE_NATION,HCOP_BENE_NAME = @HCOP_BENE_NAME,HCOP_BENE_BIRTH_DATE = @HCOP_BENE_BIRTH_DATE,HCOP_BENE_ID = @HCOP_BENE_ID,HCOP_BENE_PASSPORT = @HCOP_BENE_PASSPORT,HCOP_BENE_PASSPORT_EXP = @HCOP_BENE_PASSPORT_EXP,HCOP_BENE_RESIDENT_NO = @HCOP_BENE_RESIDENT_NO,HCOP_BENE_RESIDENT_EXP = @HCOP_BENE_RESIDENT_EXP,HCOP_BENE_OTH_CERT = @HCOP_BENE_OTH_CERT,HCOP_BENE_OTH_CERT_EXP = @HCOP_BENE_OTH_CERT_EXP,";
                sSQL += @"HCOP_BENE_JOB_TYPE = @HCOP_BENE_JOB_TYPE,HCOP_BENE_JOB_TYPE_2 = @HCOP_BENE_JOB_TYPE_2,HCOP_BENE_JOB_TYPE_3 = @HCOP_BENE_JOB_TYPE_3,HCOP_BENE_JOB_TYPE_4 = @HCOP_BENE_JOB_TYPE_4,HCOP_BENE_JOB_TYPE_5 = @HCOP_BENE_JOB_TYPE_5,HCOP_BENE_JOB_TYPE_6 = @HCOP_BENE_JOB_TYPE_6,HCOP_BENE_RESERVED = @HCOP_BENE_RESERVED,Create_Date = @Create_Date,Create_Time = @Create_Time,Create_User = @Create_User,HCOP_BENE_LNAME = @HCOP_BENE_LNAME,HCOP_BENE_ROMA = @HCOP_BENE_ROMA 
                    where  ID = @ID ;
            ";
            }

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sSQL;

            sqlcmd.Parameters.Add(new SqlParameter("@ID", paramObj.ID));
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

            //長姓名
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_LNAME", paramObj.HCOP_BENE_LNAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BENE_ROMA", paramObj.HCOP_BENE_ROMA));

            rtn.Add(sqlcmd);
        }


        return rtn;
    }

    /// <summary>
    /// 案件送審，變更狀態 (退件，放行通用)
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool Update_Apply(AML_SessionState paramObj, string updtype)
    {
        bool result = false;
        try
        {
            string sSql = "";

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;


            switch (updtype)
            {
                case "1": //ALL 對應送審
                    sSql = @"update AML_HQ_Work 
                            set CaseOwner_User = @CaseOwner_User,CaseProcess_User = @CaseProcess_User,CaseProcess_Status = @CaseProcess_Status
                            where ID = @ID;     ";
                    sqlcmd.Parameters.Add(new SqlParameter("@CaseOwner_User", paramObj.CaseOwner_User));
                    sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User", paramObj.CaseProcess_User));
                    sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_Status", paramObj.CaseProcess_Status));
                    break;
                case "2": //ProcessAndStstus 對應退件
                    sSql = @"update AML_HQ_Work 
                            set  CaseProcess_User = @CaseProcess_User,CaseProcess_Status = @CaseProcess_Status 
                            where ID = @ID;     ";

                    sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_User", paramObj.CaseProcess_User));
                    sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_Status", paramObj.CaseProcess_Status));
                    break;
                case "3": //Status 對應放行
                    sSql = @"update AML_HQ_Work 
                            set   CaseProcess_Status = @CaseProcess_Status 
                            where ID = @ID;     ";
                    sqlcmd.Parameters.Add(new SqlParameter("@CaseProcess_Status", paramObj.CaseProcess_Status));
                    break;
                case "4":  //只改經辦
                    sSql = @"update AML_HQ_Work 
                            set CaseOwner_User = @CaseOwner_User 
                            where ID = @ID;     ";
                    sqlcmd.Parameters.Add(new SqlParameter("@CaseOwner_User", paramObj.CaseOwner_User));
                    break;
            }

            sqlcmd.CommandText = sSql;
            sqlcmd.Parameters.Add(new SqlParameter("@ID", paramObj.ID));

            result = Update(sqlcmd);
        }
        catch (Exception ex)
        {
            Logging.Log(ex.Message + ex.InnerException.Message, LogState.Error, LogLayer.None);

        }
        finally
        {
            // Close the connection.

        }
        return result;
    }

    /// <summary>
    /// 僅變更案件經辦人員
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool Update_CaseOwner_User(EntityAML_HQ_Work paramObj)
    {
        bool result = false;
        if (paramObj != null)
        {
            if (!String.IsNullOrEmpty(paramObj.CASE_NO) && !String.IsNullOrEmpty(paramObj.CaseOwner_User))
            {
                try
                {
                    string sSql = @"UPDATE [AML_HQ_Work]
                                    SET CaseOwner_User = @CaseOwner_User
                                    WHERE CASE_NO = @CASE_NO ";
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.CommandType = CommandType.Text;
                    sqlcmd.CommandText = sSql;
                    sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", paramObj.CASE_NO));
                    sqlcmd.Parameters.Add(new SqlParameter("@CaseOwner_User", paramObj.CaseOwner_User));

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

    /// <summary>
    /// 更新產檔旗標
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool Update_AML_ExportFileFlagr(AML_SessionState paramObj)
    {
        bool result = false;
        if (paramObj != null)
        {
            if (!String.IsNullOrEmpty(paramObj.CASE_NO))
            {
                try
                {
                    //20190531 Talas 調整增加ReviewCompletedDate欄位以八碼日期更新
                    string strReviewCompletedDate = DateTime.Now.ToString("yyyyMMdd");
                    string sSql = @"UPDATE [AML_HQ_Work]
                                    SET AML_ExportFileFlag = '1',AML_LastExportTime =getdate() 
                                     ,ReviewCompletedDate = @ReviewCompletedDate
                                    WHERE CASE_NO = @CASE_NO ";
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.CommandType = CommandType.Text;
                    sqlcmd.CommandText = sSql;
                    sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", paramObj.CASE_NO));
                    sqlcmd.Parameters.Add(new SqlParameter("@ReviewCompletedDate", strReviewCompletedDate));

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="oAML_AGENT_SETTING"></param>
    /// <param name="oRoleID"></param>
    /// <param name="oOrderColumn"></param>
    /// <param name="tempDataTable"></param>
    /// <returns></returns>
    public static bool GetAML_HQ_Work_ForP010801130001(EntityAML_HQ_Work oAML_HQ_Work, string oRoleID, string oOrderColumn, ref DataTable tempDataTable)
    {
        if (oAML_HQ_Work != null && !String.IsNullOrEmpty(oRoleID))
        {
            try
            {
                //固定條件:不查詢已結案的案件 & 目前停留在經辦階段(暫定)
                SqlCommand sqlcmd = new SqlCommand();
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                string sql = string.Format(@"SELECT Row_Number() Over ( ORDER BY CASE_NO )  as tempRowNum
                               ,[ID],[CASE_NO],[HCOP_HEADQUATERS_CORP_NO],[HCOP_REG_NAME],[USER_ID],[USER_NAME],[USER_NAME] + '(' + [USER_ID] + ')' AS [Show_NAME]
                               FROM [{0}].[dbo].[AML_HQ_Work] TBA
                               LEFT JOIN [{0}].[dbo].[AML_AGENT_SETTING] TBB ON TBA.CaseOwner_User = TBB.[USER_ID]
                               WHERE CaseProcess_Status <> '2' 
                               AND CaseProcess_User = 'M1' ", UtilHelper.GetAppSettings("DB_KeyinGUI"));

                if (!String.IsNullOrEmpty(oAML_HQ_Work.CASE_NO))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", oAML_HQ_Work.CASE_NO));
                    sql += " AND CASE_NO = @CASE_NO ";
                }

                if (!String.IsNullOrEmpty(oAML_HQ_Work.CaseOwner_User))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@CaseOwner_User", oAML_HQ_Work.CaseOwner_User));
                    sql += " AND CaseOwner_User = @CaseOwner_User ";
                }

                if (!String.IsNullOrEmpty(oAML_HQ_Work.HCOP_HEADQUATERS_CORP_NO))
                {
                    sqlcmd.Parameters.Add(new SqlParameter("@HCOP_HEADQUATERS_CORP_NO", oAML_HQ_Work.HCOP_HEADQUATERS_CORP_NO));
                    sql += " AND HCOP_HEADQUATERS_CORP_NO = @HCOP_HEADQUATERS_CORP_NO ";
                }

                //if (!String.IsNullOrEmpty(oAML_HQ_Work.CaseOwner_User))
                //{
                //    sqlcmd.Parameters.Add(new SqlParameter("@CaseOwner_User", oAML_HQ_Work.CaseOwner_User));
                //    sql += " AND CaseOwner_User = @CaseOwner_User ";
                //}

                string[] tempArry = oOrderColumn.Split(',');
                string[] arrayOrderColumn = { "CASE_NO" };
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
                else
                {
                    sql += "ORDER BY CASE_NO ";
                }

                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = SearchOnDataSet(sqlcmd);
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
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }
        else
            return false;
    }

    public static bool UPDATE_Obj_CaseOwner_User(List<EntityAML_HQ_Work> listEntityAML_HQ_Work, String strNewCaseOwner_User, List<Entity_NoteLog> listEntity_NoteLog, ref int iSuccessCount)
    {
        bool result = false;
        SqlCommand sqlcmd = new SqlCommand();

        try
        {
            if (listEntityAML_HQ_Work != null && listEntityAML_HQ_Work.Count > 0)
            {
                iSuccessCount = 0;
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    int iLoop = 0;
                    foreach (EntityAML_HQ_Work loopObj_M in listEntityAML_HQ_Work)
                    {
                        result = BRAML_HQ_Work.Update_CaseOwner_User(loopObj_M);

                        if (result == false)
                            return result;
                        iLoop++;
                    }

                    if (result)
                    {
                        int i = 0;
                        foreach (Entity_NoteLog loopObj in listEntity_NoteLog)
                        {
                            if (NoteLog.insert(loopObj))
                                iSuccessCount++;
                        }

                        if (listEntity_NoteLog.Count == iSuccessCount)
                            result = true;
                        i++;
                    }


                    if (result)
                        ts.Complete();
                }
            }
        }
        catch (Exception exp)
        {
            AML_AGENT_SETTING.SaveLog(exp.Message);
        }
        return result;
    }

    //20191105-RQ-2018-015749-002 add by Peggy
    #region RQ-2018-015749-002
    /// <summary>
    /// 讀取案件的總公司年度請款金額的每月請款金額明細
    /// </summary>
    /// <returns></returns>
    public static DataTable GetHQMonthAMT(string _CaseNo)
    {
        string sSQL = @" 
            SELECT [CASE_NO],[HCOP_HEADQUATERS_CORP_NO]
                  ,[HCOP_SIXM_TOT_AMT]
                  ,[HCOP_MON_AMT1]
                  ,[HCOP_MON_AMT2]
                  ,[HCOP_MON_AMT3]
                  ,[HCOP_MON_AMT4]
                  ,[HCOP_MON_AMT5]
                  ,[HCOP_MON_AMT6]
                  ,[HCOP_MON_AMT7]
                  ,[HCOP_MON_AMT8]
                  ,[HCOP_MON_AMT9]
                  ,[HCOP_MON_AMT10]
                  ,[HCOP_MON_AMT11]
                  ,[HCOP_MON_AMT12]
              FROM [dbo].[AML_HQ_Work]
              WHERE CASE_NO = @case_no ";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sSQL;
        sqlcmd.Parameters.Add(new SqlParameter("@case_no", _CaseNo));

        DataTable dt = new DataTable();
        DataSet DS = SearchOnDataSet(sqlcmd);
        if (DS != null && DS.Tables.Count > 0)
        {
            dt = DS.Tables[0];
        }

        return dt;

    }

    /// <summary>
    /// 匯出成Excel
    /// 修改紀錄: 2020/12/14_Ares_Stanley-變更報表產出方式為NPOI
    /// </summary>
    /// <param name="dtData"></param>
    /// <param name="strAgentName"></param>
    /// <param name="strPathFile"></param>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    public static bool CreateExcelFile(DataTable oDT, string strAgentName, string strExcelTemplete, ref string strPathFile, ref string strMsgID)
    {
        string strBuildDate = string.Empty;
        try
        {
            // 檢查目錄，並刪除以前的文檔資料
            CSIPKeyInGUI.BusinessRules_new.BRExcel_File.CheckDirectory(ref strPathFile);

            if (null == oDT)
                return false;
            if (oDT.Rows.Count == 0)
            {
                strMsgID = "01_03040000_003";//沒有對應的資料！
                return false;
            }


            string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + strExcelTemplete.Trim();
            FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
            HSSFWorkbook wb = new HSSFWorkbook(fs);
            ISheet sheet = wb.GetSheet("工作表1");
            #region 文字格式
            //粗體
            HSSFFont boldFont = (HSSFFont)wb.CreateFont();
            boldFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            boldFont.FontHeightInPoints = 12;
            boldFont.FontName = "新細明體";
            //置中, 轉為文字
            HSSFCellStyle contentCenter = (HSSFCellStyle)wb.CreateCellStyle();
            contentCenter.VerticalAlignment = VerticalAlignment.Center;
            contentCenter.Alignment = HorizontalAlignment.Center;
            contentCenter.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
            HSSFFont contentFont = (HSSFFont)wb.CreateFont();
            contentFont.FontHeightInPoints = 12;
            contentFont.FontName = "新細明體";
            contentCenter.SetFont(contentFont);
            #endregion

            // 表頭
            sheet.GetRow(3).CreateCell(1).SetCellValue(strAgentName);
            sheet.GetRow(3).GetCell(1).CellStyle = contentCenter;
            sheet.GetRow(4).CreateCell(1).SetCellValue(DateTime.Now.ToString("yyyy/MM/dd"));
            sheet.GetRow(4).GetCell(1).CellStyle = contentCenter;
            for (int row=0; row < oDT.Rows.Count; row++)
            {
                sheet.CreateRow(sheet.LastRowNum+1);
                for(int col=0; col<10; col++)
                {
                    sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = contentCenter;
                }
                sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue(oDT.Rows[row]["DataDate"].ToString().Trim());
                sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(oDT.Rows[row]["CASE_NO"].ToString().Trim());
                sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue(oDT.Rows[row]["CORP_NO"].ToString().Trim());
                sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(oDT.Rows[row]["HCOP_REG_NAME"].ToString().Trim());
                sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(oDT.Rows[row]["ContactLog"].ToString().Trim());
                sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue(oDT.Rows[row]["LastUpdateDate"].ToString().Trim());
                sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellValue(oDT.Rows[row]["NL_TYPE"].ToString().Trim());
                sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(oDT.Rows[row]["INCORPORATED"].ToString().Trim());
                sheet.GetRow(sheet.LastRowNum).GetCell(8).SetCellValue(oDT.Rows[row]["LastUpdateMaker"].ToString().Trim());
                sheet.GetRow(sheet.LastRowNum).GetCell(9).SetCellValue(oDT.Rows[row]["LastUpdateChecker"].ToString().Trim());
            }

            sheet.CreateRow(sheet.LastRowNum+2).CreateCell(0).SetCellValue("總筆數 : 共 " + oDT.Rows.Count + " 筆");
            sheet.GetRow(sheet.LastRowNum).GetCell(0).CellStyle.SetFont(boldFont);

            // 保存文件到程序運行目錄下
            strPathFile = strPathFile + @"\" + "結案報表_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            return true;
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
            throw ex;
        }
        finally
        {
        }
    }

    /// <summary>
    ///  特店資料異動(依統編)明細清單匯出到Excel
    /// </summary>
    /// <param name="sheet"></param>
    /// <param name="strAgentName"></param>
    /// <param name="dtblWriteData"></param>
    /// <param name="strInputDate"></param>
    /// 修改紀錄:2021/04/01_Ares_Stanley-註解未使用的程式碼
    #region 未使用程式碼-WriteDataToSheet_Fault
    //private static void WriteDataToSheet_Fault(Microsoft.Office.Interop.Excel.Worksheet sheet, string strAgentName, System.Data.DataTable dtblWriteData, string strInputDate)
    //{
    //    Microsoft.Office.Interop.Excel.Range range = null;
    //    int intRowIndexInSheet = 0;
    //    string[,] arrExportData = null;

    //    arrExportData = new string[dtblWriteData.Rows.Count, 10];//確認欄位數後，修改之

    //    //*Page Title
    //    //range = sheet.get_Range("B4", Missing.Value);//起迄日期：
    //    //range.Value2 = strInputDate;

    //    range = sheet.get_Range("B4", Missing.Value);//列印經辦：
    //    range.Value2 = strAgentName;

    //    range = sheet.get_Range("B5", Missing.Value);//列印日期：
    //    range.Value2 = DateTime.Now.ToString("yyyy/MM/dd");


    //    intRowIndexInSheet = 7;

    //    for (int intLoop = 0; intLoop < dtblWriteData.Rows.Count; intLoop++)
    //    {
    //        intRowIndexInSheet++;

    //        //建案日
    //        arrExportData[intRowIndexInSheet - 8, 0] = dtblWriteData.Rows[intLoop]["DataDate"].ToString().Trim();
    //        // 案件編號
    //        arrExportData[intRowIndexInSheet - 8, 1] = dtblWriteData.Rows[intLoop]["CASE_NO"].ToString().Trim();
    //        //統一編號
    //        arrExportData[intRowIndexInSheet - 8, 2] = dtblWriteData.Rows[intLoop]["CORP_NO"].ToString().Trim();
    //        //登記名稱
    //        arrExportData[intRowIndexInSheet - 8, 3] = dtblWriteData.Rows[intLoop]["HCOP_REG_NAME"].ToString().Trim();
    //        //聯絡結果
    //        arrExportData[intRowIndexInSheet - 8, 4] = dtblWriteData.Rows[intLoop]["ContactLog"].ToString().Trim();
    //        //審查完成日
    //        arrExportData[intRowIndexInSheet - 8, 5] = dtblWriteData.Rows[intLoop]["LastUpdateDate"].ToString().Trim();
    //        //結案原因
    //        arrExportData[intRowIndexInSheet - 8, 6] = dtblWriteData.Rows[intLoop]["NL_TYPE"].ToString().Trim();
    //        //不合作註註
    //        arrExportData[intRowIndexInSheet - 8, 7] = dtblWriteData.Rows[intLoop]["INCORPORATED"].ToString().Trim();
    //        //經辦
    //        arrExportData[intRowIndexInSheet - 8, 8] = dtblWriteData.Rows[intLoop]["LastUpdateMaker"].ToString().Trim();
    //        //主管
    //        arrExportData[intRowIndexInSheet - 8, 9] = dtblWriteData.Rows[intLoop]["LastUpdateChecker"].ToString().Trim();
    //    }

    //    range = sheet.get_Range("A8", "J" + intRowIndexInSheet.ToString());


    //    range.Font.Size = 12;// 字體大小
    //    range.Font.Name = "新細明體";
    //    range.Value2 = arrExportData;
    //    range.Borders.LineStyle = 1;
    //    range.EntireColumn.AutoFit();

    //    // 總筆數
    //    range = sheet.get_Range("A" + Convert.ToString(intRowIndexInSheet + 2), Missing.Value);
    //    range.Value2 = "總筆數 : 共 " + dtblWriteData.Rows.Count + " 筆";
    //    range.Font.Size = 12;// 字體大小
    //    range.Font.Name = "新細明體";
    //    range.Font.Bold = true;
    //}
    #endregion 未使用程式碼-WriteDataToSheet_Fault


    /// <summary>
    /// RQ-2018-015749-002， 重起案件時，給予新的案件編號
    /// </summary>
    /// <param name="CaseType">8:案件重審 9:解除不合作</param>
    /// <returns></returns>
    public static int GetRemandCaseNo(string CaseType)
    {

        string _newCaseNo = DateTime.Now.ToString("yyyyMMdd") + CaseType.Trim();

        string sSQL = @" SELECT ISNULL(COUNT(CASE_NO),0)+1 AS CNT 
                                         FROM AML_HQ_WORK
                                         WHERE CASE_NO LIKE '" + _newCaseNo + "%'";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;

        sqlcmd.CommandText = sSQL;
        DataSet DS = SearchOnDataSet(sqlcmd);
        int result = 0;
        if (DS != null && DS.Tables.Count > 0)
        {
            result = int.Parse(DS.Tables[0].Rows[0][0].ToString());
        }
        return result;
    }
    #endregion

    //20191226-RQ-2019-030155-002
    /// <summary>
    /// 從主機取得最新的總公司狀態，更新於案件中
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool UpdateFromJC66(EntityAML_HQ_Work paramObj)
    {
        bool result = false;
        try
        {
            //僅更新電文帶回的來值，其他欄位不動
            string sSql = @"update AML_HQ_Work 
                       set 
                                HCOP_HEADQUATERS_CORP_NO = @HCOP_HEADQUATERS_CORP_NO
                                ,HCOP_HEADQUATERS_CORP_SEQ = @HCOP_HEADQUATERS_CORP_SEQ,
                                HCOP_CORP_TYPE = @HCOP_CORP_TYPE,
                                HCOP_REGISTER_NATION = @HCOP_REGISTER_NATION,
                                HCOP_CORP_REG_ENG_NAME = @HCOP_CORP_REG_ENG_NAME,
                                HCOP_REG_NAME = @HCOP_REG_NAME,
                                HCOP_BUILD_DATE = @HCOP_BUILD_DATE,
                                HCOP_CC = @HCOP_CC,
                                HCOP_REG_CITY = @HCOP_REG_CITY,
                                HCOP_REG_ADDR1 = @HCOP_REG_ADDR1,
                                HCOP_REG_ADDR2 = @HCOP_REG_ADDR2,
                                HCOP_EMAIL = @HCOP_EMAIL,
                                HCOP_NP_COMPANY_NAME = @HCOP_NP_COMPANY_NAME,
                                HCOP_OWNER_NATION = @HCOP_OWNER_NATION,
                                HCOP_OWNER_CHINESE_NAME = @HCOP_OWNER_CHINESE_NAME,
                                HCOP_OWNER_ENGLISH_NAME = @HCOP_OWNER_ENGLISH_NAME,
                                HCOP_OWNER_BIRTH_DATE = @HCOP_OWNER_BIRTH_DATE,
                                HCOP_OWNER_ID = @HCOP_OWNER_ID,
                                HCOP_OWNER_ID_ISSUE_DATE = @HCOP_OWNER_ID_ISSUE_DATE,
                                HCOP_OWNER_ID_ISSUE_PLACE = @HCOP_OWNER_ID_ISSUE_PLACE,
                                HCOP_OWNER_ID_REPLACE_TYPE = @HCOP_OWNER_ID_REPLACE_TYPE,
                                HCOP_ID_PHOTO_FLAG = @HCOP_ID_PHOTO_FLAG,
                                HCOP_PASSPORT = @HCOP_PASSPORT,
                                HCOP_PASSPORT_EXP_DATE = @HCOP_PASSPORT_EXP_DATE,
                                HCOP_RESIDENT_NO = @HCOP_RESIDENT_NO,
                                HCOP_RESIDENT_EXP_DATE = @HCOP_RESIDENT_EXP_DATE,
                                HCOP_OTHER_CERT = @HCOP_OTHER_CERT,
                                HCOP_OTHER_CERT_EXP_DATE = @HCOP_OTHER_CERT_EXP_DATE,
                                HCOP_COMPLEX_STR_CODE = @HCOP_COMPLEX_STR_CODE,
                                HCOP_ISSUE_STOCK_FLAG = @HCOP_ISSUE_STOCK_FLAG,
                                HCOP_COMP_TEL = @HCOP_COMP_TEL,
                                HCOP_MAILING_CITY = @HCOP_MAILING_CITY,
                                HCOP_MAILING_ADDR1 = @HCOP_MAILING_ADDR1,
                                HCOP_MAILING_ADDR2 = @HCOP_MAILING_ADDR2,
                                HCOP_PRIMARY_BUSI_COUNTRY = @HCOP_PRIMARY_BUSI_COUNTRY,
                                HCOP_OVERSEAS_FOREIGN = @HCOP_OVERSEAS_FOREIGN,
                                HCOP_OVERSEAS_FOREIGN_COUNTRY = @HCOP_OVERSEAS_FOREIGN_COUNTRY,
                                HCOP_REGISTER_US_STATE = @HCOP_REGISTER_US_STATE,
                                HCOP_BUSINESS_ORGAN_TYPE = @HCOP_BUSINESS_ORGAN_TYPE,HCOP_CREATE_DATE = @HCOP_CREATE_DATE,
                                HCOP_STATUS = @HCOP_STATUS,HCOP_OWNER_CITY = @HCOP_OWNER_CITY,
                                HCOP_OWNER_ADDR1 = @HCOP_OWNER_ADDR1,
                                HCOP_OWNER_ADDR2 = @HCOP_OWNER_ADDR2,
                                Create_Time = @Create_Time,
                                Create_User = @Create_User,
                                Create_Date = @Create_Date,                                
                                HCOP_CONTACT_NAME = @HCOP_CONTACT_NAME, 
                                HCOP_CONTACT_TEL=@HCOP_CONTACT_TEL,
                                HCOP_OWNER_CHINESE_LNAME = @HCOP_OWNER_CHINESE_LNAME,
                                HCOP_OWNER_ROMA = @HCOP_OWNER_ROMA,
                                HCOP_CONTACT_LNAME = @HCOP_CONTACT_LNAME,
                                HCOP_CONTACT_ROMA = @HCOP_CONTACT_ROMA,
                                HCOP_MOBILE = @HCOP_MOBILE,
                                HCOP_OC = @HCOP_OC,
                                HCOP_INCOME_SOURCE = @HCOP_INCOME_SOURCE,
                                HCOP_GENDER = @HCOP_GENDER,
                                HCOP_COUNTRY_CODE_2 = @HCOP_COUNTRY_CODE_2,
                                HCOP_CC_2 = @HCOP_CC_2,
                                HCOP_CC_3 = @HCOP_CC_3,
                                HCOP_REG_ZIP_CODE = @HCOP_REG_ZIP_CODE,
                                HCOP_LAST_UPD_MAKER = @HCOP_LAST_UPD_MAKER,
                                HCOP_LAST_UPD_CHECKER = @HCOP_LAST_UPD_CHECKER,
                                HCOP_LAST_UPD_BRANCH = @HCOP_LAST_UPD_BRANCH,
                                HCOP_LAST_UPDATE_DATE = @HCOP_LAST_UPDATE_DATE
                                

                       where ID = @ID;
                       ";
            
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sSql;
            sqlcmd.Parameters.Add(new SqlParameter("@ID", paramObj.ID));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_HEADQUATERS_CORP_NO", paramObj.HCOP_HEADQUATERS_CORP_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_HEADQUATERS_CORP_SEQ", paramObj.HCOP_HEADQUATERS_CORP_SEQ));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CORP_TYPE", paramObj.HCOP_CORP_TYPE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REGISTER_NATION", paramObj.HCOP_REGISTER_NATION));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CORP_REG_ENG_NAME", paramObj.HCOP_CORP_REG_ENG_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REG_NAME", paramObj.HCOP_REG_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BUILD_DATE", paramObj.HCOP_BUILD_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CC", paramObj.HCOP_CC));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REG_CITY", paramObj.HCOP_REG_CITY));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REG_ADDR1", paramObj.HCOP_REG_ADDR1));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REG_ADDR2", paramObj.HCOP_REG_ADDR2));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_EMAIL", paramObj.HCOP_EMAIL));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_NP_COMPANY_NAME", paramObj.HCOP_NP_COMPANY_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_NATION", paramObj.HCOP_OWNER_NATION));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_CHINESE_NAME", paramObj.HCOP_OWNER_CHINESE_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ENGLISH_NAME", paramObj.HCOP_OWNER_ENGLISH_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_BIRTH_DATE", paramObj.HCOP_OWNER_BIRTH_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ID", paramObj.HCOP_OWNER_ID));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ID_ISSUE_DATE", paramObj.HCOP_OWNER_ID_ISSUE_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ID_ISSUE_PLACE", paramObj.HCOP_OWNER_ID_ISSUE_PLACE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ID_REPLACE_TYPE", paramObj.HCOP_OWNER_ID_REPLACE_TYPE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_ID_PHOTO_FLAG", paramObj.HCOP_ID_PHOTO_FLAG));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_PASSPORT", paramObj.HCOP_PASSPORT));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_PASSPORT_EXP_DATE", paramObj.HCOP_PASSPORT_EXP_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_RESIDENT_NO", paramObj.HCOP_RESIDENT_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_RESIDENT_EXP_DATE", paramObj.HCOP_RESIDENT_EXP_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OTHER_CERT", paramObj.HCOP_OTHER_CERT));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OTHER_CERT_EXP_DATE", paramObj.HCOP_OTHER_CERT_EXP_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_COMPLEX_STR_CODE", paramObj.HCOP_COMPLEX_STR_CODE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_ISSUE_STOCK_FLAG", paramObj.HCOP_ISSUE_STOCK_FLAG));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_COMP_TEL", paramObj.HCOP_COMP_TEL));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MAILING_CITY", paramObj.HCOP_MAILING_CITY));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MAILING_ADDR1", paramObj.HCOP_MAILING_ADDR1));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MAILING_ADDR2", paramObj.HCOP_MAILING_ADDR2));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_PRIMARY_BUSI_COUNTRY", paramObj.HCOP_PRIMARY_BUSI_COUNTRY));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OVERSEAS_FOREIGN", paramObj.HCOP_OVERSEAS_FOREIGN));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OVERSEAS_FOREIGN_COUNTRY", paramObj.HCOP_OVERSEAS_FOREIGN_COUNTRY));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REGISTER_US_STATE", paramObj.HCOP_REGISTER_US_STATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BUSINESS_ORGAN_TYPE", paramObj.HCOP_BUSINESS_ORGAN_TYPE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CREATE_DATE", paramObj.HCOP_CREATE_DATE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_STATUS", paramObj.HCOP_STATUS));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_CITY", paramObj.HCOP_OWNER_CITY));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ADDR1", paramObj.HCOP_OWNER_ADDR1));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ADDR2", paramObj.HCOP_OWNER_ADDR2));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CONTACT_NAME", paramObj.HCOP_CONTACT_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CONTACT_TEL", paramObj.HCOP_CONTACT_TEL));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_Time", paramObj.Create_Time));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_User", paramObj.Create_User));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_Date", paramObj.Create_Date));

            //長姓名
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_CHINESE_LNAME", paramObj.HCOP_OWNER_CHINESE_LNAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OWNER_ROMA", paramObj.HCOP_OWNER_ROMA));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CONTACT_LNAME", paramObj.HCOP_CONTACT_LNAME));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CONTACT_ROMA", paramObj.HCOP_CONTACT_ROMA));

            //20191226-RQ-2019-030155-002 新增聯絡電話欄位(HCOP_MOBILE)
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_MOBILE", paramObj.HCOP_MOBILE));

            //20211104_Ares_Jack_補上 職稱編號, 性別, 收入來源
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_OC", paramObj.HCOP_OC));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_INCOME_SOURCE", paramObj.HCOP_INCOME_SOURCE));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_GENDER", paramObj.HCOP_GENDER));

            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_COUNTRY_CODE_2", paramObj.HCOP_COUNTRY_CODE_2));//國籍二
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CC_2", paramObj.HCOP_CC_2));//行業別編號二
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_CC_3", paramObj.HCOP_CC_3));//行業別編號三
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_REG_ZIP_CODE", paramObj.HCOP_REG_ZIP_CODE));//郵遞區號
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_LAST_UPD_MAKER", paramObj.HCOP_LAST_UPD_MAKER));//MAKER
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_LAST_UPD_CHECKER", paramObj.HCOP_LAST_UPD_CHECKER));//CHECKER
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_LAST_UPD_BRANCH", paramObj.HCOP_LAST_UPD_BRANCH));//BRANCH
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_LAST_UPDATE_DATE", paramObj.HCOP_LAST_UPDATE_DATE));//UPDATE_DATE

            //組出高階經理人用sqlCommand
            List<SqlCommand> mangColl = getMangEditCommand(paramObj.ManagerColl);
            ///使用交易
            using (OMTransactionScope ts = new OMTransactionScope())
            {
                result = Update(sqlcmd);
                if (!result)
                {
                    return false;
                }
                //開始寫入
                foreach (SqlCommand scmd in mangColl)
                {
                    result = Update(scmd);
                    if (!result)
                    {
                        return false;
                    }
                }
                ts.Complete();
            }
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
    
    public static DataTable getRelatedCaseNo(string groupNo)
    {
        string sSQL = string.Format(@" select distinct(CASE_NO), ID
    from AML_HQ_Work A 
     where CASE_NO is not null and GROUP_NO = @GROUP_NO
     order by CASE_NO ", UtilHelper.GetAppSettings("DB_CSIP"));
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sSQL;
        sqlcmd.Parameters.Add(new SqlParameter("@GROUP_NO", groupNo));
        DataTable dt = new DataTable();
        DataSet DS = SearchOnDataSet(sqlcmd);
        if (DS != null && DS.Tables.Count > 0)
        {
            dt = DS.Tables[0];
        }
        return dt;

    }    

    #region 共用NPOI
    /// <summary>
    /// 修改紀錄:2020/12/14_Ares_Stanley-新增共用NPOI
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="wb"></param>
    /// <param name="start"></param>
    /// <param name="sheetName"></param>
    private static void ExportExcelForNPOI(DataTable dt, ref HSSFWorkbook wb, Int32 start, String sheetName)
    {
        try
        {
            HSSFCellStyle cs = (HSSFCellStyle)wb.CreateCellStyle();
            cs.BorderBottom = BorderStyle.Thin;
            cs.BorderLeft = BorderStyle.Thin;
            cs.BorderTop = BorderStyle.Thin;
            cs.BorderRight = BorderStyle.Thin;
            //啟動多行文字
            cs.WrapText = true;
            //文字置中
            cs.VerticalAlignment = VerticalAlignment.Center;
            cs.Alignment = HorizontalAlignment.Center;

            HSSFFont font1 = (HSSFFont)wb.CreateFont();
            //字體尺寸
            font1.FontHeightInPoints = 12;
            font1.FontName = "新細明體";
            cs.SetFont(font1);

            if (dt != null && dt.Rows.Count != 0)
            {
                int count = start;
                ISheet sheet = wb.GetSheet(sheetName);
                int cols = dt.Columns.Count;
                foreach (DataRow dr in dt.Rows)
                {
                    int cell = 0;
                    IRow row = (IRow)sheet.CreateRow(count);
                    row.CreateCell(0).SetCellValue(count.ToString());
                    for (int i = 0; i < cols; i++)
                    {
                        row.CreateCell(cell).SetCellValue(dr[i].ToString());
                        row.GetCell(cell).CellStyle = cs;
                        cell++;
                    }
                    count++;
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }

    /// <summary>
    /// 修改紀錄: 2020/12/14_Ares_Stanley-新增共用NPOI
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="wb"></param>
    /// <param name="start"></param>
    /// <param name="delColumn"></param>
    /// <param name="sheetName"></param>
    private static void ExportExcelForNPOI_filter(DataTable dt, ref HSSFWorkbook wb, Int32 start, Int32 delColumn, String sheetName)
    {
        try
        {
            HSSFCellStyle cs = (HSSFCellStyle)wb.CreateCellStyle();
            cs.BorderBottom = BorderStyle.Thin;
            cs.BorderLeft = BorderStyle.Thin;
            cs.BorderTop = BorderStyle.Thin;
            cs.BorderRight = BorderStyle.Thin;

            //啟動多行文字
            cs.WrapText = true;
            //文字置中
            cs.VerticalAlignment = VerticalAlignment.Center;
            cs.Alignment = HorizontalAlignment.Center;

            HSSFFont font1 = (HSSFFont)wb.CreateFont();
            //字體尺寸
            font1.FontHeightInPoints = 12;
            font1.FontName = "新細明體";
            cs.SetFont(font1);

            if (dt != null && dt.Rows.Count != 0)
            {
                int count = start;
                ISheet sheet = wb.GetSheet(sheetName);
                int cols = dt.Columns.Count - delColumn;
                foreach (DataRow dr in dt.Rows)
                {
                    int cell = 0;
                    IRow row = (IRow)sheet.CreateRow(count);
                    row.CreateCell(0).SetCellValue(count.ToString());
                    for (int i = 0; i < cols; i++)
                    {
                        row.CreateCell(cell).SetCellValue(dr[i].ToString());
                        row.GetCell(cell).CellStyle = cs;
                        cell++;
                    }
                    count++;
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }
    #endregion
}