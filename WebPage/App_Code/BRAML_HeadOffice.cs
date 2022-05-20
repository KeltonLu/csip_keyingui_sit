//******************************************************************
//*  作    者：林家賜
//*  功能說明：收單特店審查處理  總公司資料
//*  創建日期：2019/01/24
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
///20190806-RQ-2019-008595-002-長姓名需求，增開負責人/聯絡人 中文長姓名與羅馬拼音4個欄位 by Peggy

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
/// BRAML_HeadOffice 的摘要描述
/// </summary>
public class BRAML_HeadOffice : CSIPCommonModel.BusinessRules.BRBase<EntityAML_HeadOffice>
{
    public BRAML_HeadOffice()
    {
    }
    /// <summary>
    /// 讀取1KEY.2KEY 資料
    /// </summary>
    /// <param name="BasicTaxID"></param>
    /// <param name="ID"></param>
    /// <param name="keyin_Flag"></param>
    /// <returns></returns>
    public static EntityAML_HeadOffice Query(string BasicTaxID, string ID,string keyin_Flag, string today)
    {
        string sql = @"
                    SELECT top 1 [ID],[BasicTaxID],[BasicCORP_TYPE] ,[BasicRegistyNameCH] ,[BasicRegistyNameEN] ,[BasicAMLCC] ,[BasicEstablish] ,[BasicCountryCode]
                     ,[BasicCountryStateCode] ,[BasicBookAddr1] ,[BasicBookAddr2] ,[BasicBookAddr3] ,[BasicOfficePhone1] ,[BasicOfficePhone2] ,[BasicOfficePhone3]
                     ,[BasicContactMan] ,[BasicContactAddr1] ,[BasicContactAddr2] ,[BasicContactAddr3] ,[BasicEmail] ,[PrincipalNameCH] ,[PrincipalNameEn]
                     ,[PrincipalBirth] ,[PrincipalCountryCode] ,[PrincipalIDNo] ,[PrincipalIssueDate] ,[PrincipalIssuePlace] ,[PrincipalRedemption] ,[PrincipalHasPic]
                     ,[PrincipalBookAddr1] ,[PrincipalBookAddr2] ,[PrincipalBookAddr3] ,[PrincipalPassportNo] ,[PrincipalPassportExpdt] ,[PrincipalContactAddr1]
                     ,[PrincipalContactAddr2] ,[PrincipalContactAddr3] ,[PrincipalResidentNo] ,[PrincipalResidentExpdt] ,[SCCDOrganization] ,[SCCDCountryCode]
                     ,[SCCDCountryStateCode] ,[SCCDForeign] ,[SCCDForeignCountryStateCode] ,[SCCDOtherOfficeAddr1] ,[SCCDOtherOfficeAddr2] ,[SCCDOtherOfficeAddr3]
                     ,[SCCDOtherCountryCode] ,[SCCDIsSanction] ,[SCCDIsSanctionCountryCode1] ,[SCCDIsSanctionCountryCode2] ,[SCCDIsSanctionCountryCode3] ,[SCCDIsSanctionCountryCode4]
                     ,[SCCDIsSanctionCountryCode5] ,[SCCDEquity] ,[SCCDCanBearerStock] ,[SCCDAlreadyBearerStock] ,[keyin_Flag] ,[keyin_day] ,[keyin_userID],[isNew],[isSCDD]
                     ,[PrincipalNameCH_L], [PrincipalNameCH_Pinyin], [BasicContactMan_L], [BasicContactMan_Pinyin], [REG_ZIP_CODE], [LAST_UPD_MAKER], [LAST_UPD_CHECKER], [LAST_UPD_BRANCH] 
                      FROM  [AML_HeadOffice]
                     WHERE BasicTaxID = @BasicTaxID and keyin_Flag =@keyin_Flag and keyin_day = @today ";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sql;
        sqlcmd.Parameters.Add(new SqlParameter("@BasicTaxID", BasicTaxID));
        //   sqlcmd.Parameters.Add(new SqlParameter("@ID", ID)); //暫未使用
          sqlcmd.Parameters.Add(new SqlParameter("@keyin_Flag", keyin_Flag));  //區分1-2Key
        sqlcmd.Parameters.Add(new SqlParameter("@today", today)); //新增查詢限制, 只能查到當天資料 by Ares Stanley 20211217
        try
        {
            DataTable dt = new DataTable();
            DataSet DS = SearchOnDataSet(sqlcmd);
            if (DS != null && DS.Tables.Count > 0)
            {
                dt = DS.Tables[0];
            }
            EntityAML_HeadOffice rtn = new EntityAML_HeadOffice();
            if (dt.Rows.Count > 0)
            { 
                DataTableConvertor.convSingRow<EntityAML_HeadOffice>(ref rtn, dt.Rows[0]); 
                List<EntityAML_SeniorManager> manaColl = BRAML_SeniorManager.Query(BasicTaxID, "", keyin_Flag);
                if (manaColl != null)
                {
                    rtn.AML_SeniorManagerColl = manaColl;
                }
            } 
            return rtn;
        }
        catch (Exception ex)
        {
            Logging.Log("查詢公司資料失敗：" + ex, LogLayer.BusinessRule);
            return null;
        }


    }
    /// <summary>
    /// 讀取1KEY.2KEY 資料
    /// </summary>
    /// <param name="BasicTaxID"></param>
    /// <param name="ID"></param>
    /// <param name="keyin_Flag"></param>
    /// <returns></returns>
    public static DataSet QueryTB(string BasicTaxID, string today)
    {
        string sql = @"
SELECT top 1 [BasicCORP_TYPE],[BasicRegistyNameCH] ,[BasicRegistyNameEN] ,[BasicAMLCC] ,[BasicEstablish] ,[BasicCountryCode]
 ,[BasicCountryStateCode] ,[BasicBookAddr1] ,[BasicBookAddr2] ,[BasicBookAddr3] ,[BasicOfficePhone1] ,[BasicOfficePhone2] ,[BasicOfficePhone3]
 ,[BasicContactMan] ,[BasicContactAddr1] ,[BasicContactAddr2] ,[BasicContactAddr3] ,[BasicEmail] ,[PrincipalNameCH] ,[PrincipalNameEn]
 ,[PrincipalBirth] ,[PrincipalCountryCode] ,[PrincipalIDNo] ,[PrincipalIssueDate] ,[PrincipalIssuePlace] ,[PrincipalRedemption] ,[PrincipalHasPic]
 ,[PrincipalBookAddr1] ,[PrincipalBookAddr2] ,[PrincipalBookAddr3] ,[PrincipalPassportNo] ,[PrincipalPassportExpdt] ,[PrincipalContactAddr1]
 ,[PrincipalContactAddr2] ,[PrincipalContactAddr3] ,[PrincipalResidentNo] ,[PrincipalResidentExpdt] ,[SCCDOrganization] ,[SCCDCountryCode]
 ,[SCCDCountryStateCode] ,[SCCDForeign] ,[SCCDForeignCountryStateCode] ,[SCCDOtherOfficeAddr1] ,[SCCDOtherOfficeAddr2] ,[SCCDOtherOfficeAddr3]
 ,[SCCDOtherCountryCode] ,[SCCDIsSanction] ,[SCCDIsSanctionCountryCode1] ,[SCCDIsSanctionCountryCode2] ,[SCCDIsSanctionCountryCode3] ,[SCCDIsSanctionCountryCode4]
 ,[SCCDIsSanctionCountryCode5] ,[SCCDEquity] ,[SCCDCanBearerStock] ,[SCCDAlreadyBearerStock],[isSCDD]
 ,[PrincipalNameCH_L], [PrincipalNameCH_Pinyin], [BasicContactMan_L], [BasicContactMan_Pinyin], [REG_ZIP_CODE], [LAST_UPD_MAKER], [LAST_UPD_CHECKER], [LAST_UPD_BRANCH] ,[isNew]
  FROM  [AML_HeadOffice]
  WHERE BasicTaxID = @BasicTaxID and keyin_Flag =1 and keyin_day = @today;

SELECT top 1 [BasicCORP_TYPE],[BasicRegistyNameCH] ,[BasicRegistyNameEN] ,[BasicAMLCC] ,[BasicEstablish] ,[BasicCountryCode]
 ,[BasicCountryStateCode] ,[BasicBookAddr1] ,[BasicBookAddr2] ,[BasicBookAddr3] ,[BasicOfficePhone1] ,[BasicOfficePhone2] ,[BasicOfficePhone3]
 ,[BasicContactMan] ,[BasicContactAddr1] ,[BasicContactAddr2] ,[BasicContactAddr3] ,[BasicEmail] ,[PrincipalNameCH] ,[PrincipalNameEn]
 ,[PrincipalBirth] ,[PrincipalCountryCode] ,[PrincipalIDNo] ,[PrincipalIssueDate] ,[PrincipalIssuePlace] ,[PrincipalRedemption] ,[PrincipalHasPic]
 ,[PrincipalBookAddr1] ,[PrincipalBookAddr2] ,[PrincipalBookAddr3] ,[PrincipalPassportNo] ,[PrincipalPassportExpdt] ,[PrincipalContactAddr1]
 ,[PrincipalContactAddr2] ,[PrincipalContactAddr3] ,[PrincipalResidentNo] ,[PrincipalResidentExpdt] ,[SCCDOrganization] ,[SCCDCountryCode]
 ,[SCCDCountryStateCode] ,[SCCDForeign] ,[SCCDForeignCountryStateCode] ,[SCCDOtherOfficeAddr1] ,[SCCDOtherOfficeAddr2] ,[SCCDOtherOfficeAddr3]
 ,[SCCDOtherCountryCode] ,[SCCDIsSanction] ,[SCCDIsSanctionCountryCode1] ,[SCCDIsSanctionCountryCode2] ,[SCCDIsSanctionCountryCode3] ,[SCCDIsSanctionCountryCode4]
 ,[SCCDIsSanctionCountryCode5] ,[SCCDEquity] ,[SCCDCanBearerStock] ,[SCCDAlreadyBearerStock] ,[isSCDD]
 ,[PrincipalNameCH_L], [PrincipalNameCH_Pinyin], [BasicContactMan_L], [BasicContactMan_Pinyin], [REG_ZIP_CODE], [LAST_UPD_MAKER], [LAST_UPD_CHECKER], [LAST_UPD_BRANCH] ,[isNew] 
  FROM  [AML_HeadOffice]
  WHERE BasicTaxID = @BasicTaxID and keyin_Flag =2 and keyin_day = @today;

SELECT [Name],[Birth],[CountryCode],[IDNo],[IDNoType],[Identity1],[Identity2],[Identity3],[Identity4],[Identity5],[Identity6],[Expdt],[isDEL],[LineID], [Name_L], [Name_Pinyin]
  FROM  [AML_SeniorManager]  WHERE BasicTaxID = @BasicTaxID and keyin_Flag =1 and keyin_day = @today Order By [LineID];

SELECT [Name],[Birth],[CountryCode],[IDNo],[IDNoType],[Identity1],[Identity2],[Identity3],[Identity4],[Identity5],[Identity6],[Expdt],[isDEL],[LineID], [Name_L], [Name_Pinyin]
  FROM  [AML_SeniorManager]  WHERE BasicTaxID = @BasicTaxID and keyin_Flag =2 and keyin_day = @today Order By [LineID];
";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sql;
        sqlcmd.Parameters.Add(new SqlParameter("@BasicTaxID", BasicTaxID));
        sqlcmd.Parameters.Add(new SqlParameter("@today", today));

        try
        { 
            DataSet DS = SearchOnDataSet(sqlcmd); 
            return DS;
        }
        catch (Exception ex)
        {
            Logging.Log("查詢公司1K2K資料失敗：" + ex, LogLayer.BusinessRule);
            return null;
        }


    }
    public static bool Insert(EntityAML_HeadOffice paramObj)
    {
        bool result = false;
        string sSQL = "";
        if (paramObj.ID != "") //更新 Update
        {
            sSQL = @"update AML_HeadOffice 
                    set BasicCORP_TYPE = @BasicCORP_TYPE,BasicTaxID = @BasicTaxID,BasicRegistyNameCH = @BasicRegistyNameCH,BasicRegistyNameEN = @BasicRegistyNameEN,BasicAMLCC = @BasicAMLCC,BasicEstablish = @BasicEstablish,BasicCountryCode = @BasicCountryCode,BasicCountryStateCode = @BasicCountryStateCode,BasicBookAddr1 = @BasicBookAddr1,BasicBookAddr2 = @BasicBookAddr2,BasicBookAddr3 = @BasicBookAddr3,BasicOfficePhone1 = @BasicOfficePhone1,BasicOfficePhone2 = @BasicOfficePhone2,BasicOfficePhone3 = @BasicOfficePhone3,BasicContactMan = @BasicContactMan,BasicContactAddr1 = @BasicContactAddr1,
                    BasicContactAddr2 = @BasicContactAddr2,BasicContactAddr3 = @BasicContactAddr3,BasicEmail = @BasicEmail,PrincipalNameCH = @PrincipalNameCH,PrincipalNameEn = @PrincipalNameEn,PrincipalBirth = @PrincipalBirth,PrincipalCountryCode = @PrincipalCountryCode,PrincipalIDNo = @PrincipalIDNo,PrincipalIssueDate = @PrincipalIssueDate,PrincipalIssuePlace = @PrincipalIssuePlace,PrincipalRedemption = @PrincipalRedemption,PrincipalHasPic = @PrincipalHasPic,PrincipalBookAddr1 = @PrincipalBookAddr1,PrincipalBookAddr2 = @PrincipalBookAddr2,
                    PrincipalBookAddr3 = @PrincipalBookAddr3,PrincipalPassportNo = @PrincipalPassportNo,PrincipalPassportExpdt = @PrincipalPassportExpdt,PrincipalContactAddr1 = @PrincipalContactAddr1,PrincipalContactAddr2 = @PrincipalContactAddr2,PrincipalContactAddr3 = @PrincipalContactAddr3,PrincipalResidentNo = @PrincipalResidentNo,PrincipalResidentExpdt = @PrincipalResidentExpdt,SCCDOrganization = @SCCDOrganization,SCCDCountryCode = @SCCDCountryCode,SCCDCountryStateCode = @SCCDCountryStateCode,SCCDForeign = @SCCDForeign,
                    SCCDForeignCountryStateCode = @SCCDForeignCountryStateCode,SCCDOtherOfficeAddr1 = @SCCDOtherOfficeAddr1,SCCDOtherOfficeAddr2 = @SCCDOtherOfficeAddr2,SCCDOtherOfficeAddr3 = @SCCDOtherOfficeAddr3,SCCDOtherCountryCode = @SCCDOtherCountryCode,SCCDIsSanction = @SCCDIsSanction,SCCDIsSanctionCountryCode1 = @SCCDIsSanctionCountryCode1,SCCDIsSanctionCountryCode2 = @SCCDIsSanctionCountryCode2,SCCDIsSanctionCountryCode3 = @SCCDIsSanctionCountryCode3,SCCDIsSanctionCountryCode4 = @SCCDIsSanctionCountryCode4,
                    SCCDIsSanctionCountryCode5 = @SCCDIsSanctionCountryCode5,SCCDEquity = @SCCDEquity,SCCDCanBearerStock = @SCCDCanBearerStock,SCCDAlreadyBearerStock = @SCCDAlreadyBearerStock,keyin_Flag = @keyin_Flag,keyin_day = @keyin_day,keyin_userID = @keyin_userID ,isNew = @isNew ,isSCDD =@isSCDD ,PrincipalNameCH_L = @PrincipalNameCH_L, PrincipalNameCH_Pinyin = @PrincipalNameCH_Pinyin, BasicContactMan_L = @BasicContactMan_L, BasicContactMan_Pinyin = @BasicContactMan_Pinyin, REG_ZIP_CODE = @REG_ZIP_CODE, LAST_UPD_MAKER = @LAST_UPD_MAKER, LAST_UPD_CHECKER = @LAST_UPD_CHECKER, LAST_UPD_BRANCH = @LAST_UPD_BRANCH
                    where ID = @ID  ;
                    ";
        }
        else  //Insert
        {
            sSQL = @"Insert into AML_HeadOffice 
                        (BasicCORP_TYPE,BasicTaxID,BasicRegistyNameCH,BasicRegistyNameEN,BasicAMLCC,BasicEstablish,BasicCountryCode,BasicCountryStateCode,BasicBookAddr1,BasicBookAddr2,BasicBookAddr3,BasicOfficePhone1,BasicOfficePhone2,BasicOfficePhone3,BasicContactMan,BasicContactAddr1,BasicContactAddr2,BasicContactAddr3,BasicEmail,PrincipalNameCH,PrincipalNameEn,PrincipalBirth,PrincipalCountryCode,PrincipalIDNo,PrincipalIssueDate,PrincipalIssuePlace,PrincipalRedemption,PrincipalHasPic,PrincipalBookAddr1,PrincipalBookAddr2,PrincipalBookAddr3,PrincipalPassportNo,PrincipalPassportExpdt,PrincipalContactAddr1,PrincipalContactAddr2,PrincipalContactAddr3,PrincipalResidentNo,PrincipalResidentExpdt,SCCDOrganization,SCCDCountryCode,SCCDCountryStateCode,SCCDForeign,SCCDForeignCountryStateCode,SCCDOtherOfficeAddr1,SCCDOtherOfficeAddr2,SCCDOtherOfficeAddr3,SCCDOtherCountryCode,SCCDIsSanction,SCCDIsSanctionCountryCode1,SCCDIsSanctionCountryCode2,SCCDIsSanctionCountryCode3,SCCDIsSanctionCountryCode4,SCCDIsSanctionCountryCode5,SCCDEquity,SCCDCanBearerStock,SCCDAlreadyBearerStock,keyin_Flag,keyin_day,keyin_userID,isNew,isSCDD,PrincipalNameCH_L, PrincipalNameCH_Pinyin, BasicContactMan_L, BasicContactMan_Pinyin, REG_ZIP_CODE, LAST_UPD_MAKER, LAST_UPD_CHECKER, LAST_UPD_BRANCH )
                    VALUES(@BasicCORP_TYPE,@BasicTaxID,@BasicRegistyNameCH,@BasicRegistyNameEN,@BasicAMLCC,@BasicEstablish,@BasicCountryCode,@BasicCountryStateCode,@BasicBookAddr1,@BasicBookAddr2,@BasicBookAddr3,@BasicOfficePhone1,@BasicOfficePhone2,@BasicOfficePhone3,@BasicContactMan,@BasicContactAddr1,@BasicContactAddr2,@BasicContactAddr3,@BasicEmail,@PrincipalNameCH,@PrincipalNameEn,@PrincipalBirth,@PrincipalCountryCode,@PrincipalIDNo,@PrincipalIssueDate,@PrincipalIssuePlace,@PrincipalRedemption,@PrincipalHasPic,@PrincipalBookAddr1,@PrincipalBookAddr2,@PrincipalBookAddr3,@PrincipalPassportNo,@PrincipalPassportExpdt,@PrincipalContactAddr1,@PrincipalContactAddr2,@PrincipalContactAddr3,@PrincipalResidentNo,@PrincipalResidentExpdt,@SCCDOrganization,@SCCDCountryCode,@SCCDCountryStateCode,@SCCDForeign,@SCCDForeignCountryStateCode,@SCCDOtherOfficeAddr1,@SCCDOtherOfficeAddr2,@SCCDOtherOfficeAddr3,@SCCDOtherCountryCode,@SCCDIsSanction,@SCCDIsSanctionCountryCode1,@SCCDIsSanctionCountryCode2,@SCCDIsSanctionCountryCode3,@SCCDIsSanctionCountryCode4,@SCCDIsSanctionCountryCode5,@SCCDEquity,@SCCDCanBearerStock,@SCCDAlreadyBearerStock,@keyin_Flag,@keyin_day,@keyin_userID,@isNew,@isSCDD,@PrincipalNameCH_L, @PrincipalNameCH_Pinyin, @BasicContactMan_L, @BasicContactMan_Pinyin, @REG_ZIP_CODE, @LAST_UPD_MAKER, @LAST_UPD_CHECKER, @LAST_UPD_BRANCH);       
                ";
        }

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sSQL;
        sqlcmd.Parameters.Add(new SqlParameter("@ID", paramObj.ID));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicTaxID", paramObj.BasicTaxID));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicCORP_TYPE", paramObj.BasicCORP_TYPE));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicRegistyNameCH", paramObj.BasicRegistyNameCH));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicRegistyNameEN", paramObj.BasicRegistyNameEN));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicAMLCC", paramObj.BasicAMLCC));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicEstablish", paramObj.BasicEstablish));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicCountryCode", paramObj.BasicCountryCode));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicCountryStateCode", paramObj.BasicCountryStateCode));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicBookAddr1", paramObj.BasicBookAddr1));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicBookAddr2", paramObj.BasicBookAddr2));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicBookAddr3", paramObj.BasicBookAddr3));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicOfficePhone1", paramObj.BasicOfficePhone1));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicOfficePhone2", paramObj.BasicOfficePhone2));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicOfficePhone3", paramObj.BasicOfficePhone3));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicContactMan", paramObj.BasicContactMan));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicContactAddr1", paramObj.BasicContactAddr1));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicContactAddr2", paramObj.BasicContactAddr2));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicContactAddr3", paramObj.BasicContactAddr3));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicEmail", paramObj.BasicEmail));
        sqlcmd.Parameters.Add(new SqlParameter("@PrincipalNameCH", paramObj.PrincipalNameCH));
        sqlcmd.Parameters.Add(new SqlParameter("@PrincipalNameEn", paramObj.PrincipalNameEn));
        sqlcmd.Parameters.Add(new SqlParameter("@PrincipalBirth", paramObj.PrincipalBirth));
        sqlcmd.Parameters.Add(new SqlParameter("@PrincipalCountryCode", paramObj.PrincipalCountryCode));
        sqlcmd.Parameters.Add(new SqlParameter("@PrincipalIDNo", paramObj.PrincipalIDNo));
        sqlcmd.Parameters.Add(new SqlParameter("@PrincipalIssueDate", paramObj.PrincipalIssueDate));
        sqlcmd.Parameters.Add(new SqlParameter("@PrincipalIssuePlace", paramObj.PrincipalIssuePlace));
        sqlcmd.Parameters.Add(new SqlParameter("@PrincipalRedemption", paramObj.PrincipalRedemption));
        sqlcmd.Parameters.Add(new SqlParameter("@PrincipalHasPic", paramObj.PrincipalHasPic));
        sqlcmd.Parameters.Add(new SqlParameter("@PrincipalBookAddr1", paramObj.PrincipalBookAddr1));
        sqlcmd.Parameters.Add(new SqlParameter("@PrincipalBookAddr2", paramObj.PrincipalBookAddr2));
        sqlcmd.Parameters.Add(new SqlParameter("@PrincipalBookAddr3", paramObj.PrincipalBookAddr3));
        sqlcmd.Parameters.Add(new SqlParameter("@PrincipalPassportNo", paramObj.PrincipalPassportNo));
        sqlcmd.Parameters.Add(new SqlParameter("@PrincipalPassportExpdt", paramObj.PrincipalPassportExpdt));
        sqlcmd.Parameters.Add(new SqlParameter("@PrincipalContactAddr1", paramObj.PrincipalContactAddr1));
        sqlcmd.Parameters.Add(new SqlParameter("@PrincipalContactAddr2", paramObj.PrincipalContactAddr2));
        sqlcmd.Parameters.Add(new SqlParameter("@PrincipalContactAddr3", paramObj.PrincipalContactAddr3));
        sqlcmd.Parameters.Add(new SqlParameter("@PrincipalResidentNo", paramObj.PrincipalResidentNo));
        sqlcmd.Parameters.Add(new SqlParameter("@PrincipalResidentExpdt", paramObj.PrincipalResidentExpdt));
        sqlcmd.Parameters.Add(new SqlParameter("@SCCDOrganization", paramObj.SCCDOrganization));
        sqlcmd.Parameters.Add(new SqlParameter("@SCCDCountryCode", paramObj.SCCDCountryCode));
        sqlcmd.Parameters.Add(new SqlParameter("@SCCDCountryStateCode", paramObj.SCCDCountryStateCode));
        sqlcmd.Parameters.Add(new SqlParameter("@SCCDForeign", paramObj.SCCDForeign));
        sqlcmd.Parameters.Add(new SqlParameter("@SCCDForeignCountryStateCode", paramObj.SCCDForeignCountryStateCode));
        sqlcmd.Parameters.Add(new SqlParameter("@SCCDOtherOfficeAddr1", paramObj.SCCDOtherOfficeAddr1));
        sqlcmd.Parameters.Add(new SqlParameter("@SCCDOtherOfficeAddr2", paramObj.SCCDOtherOfficeAddr2));
        sqlcmd.Parameters.Add(new SqlParameter("@SCCDOtherOfficeAddr3", paramObj.SCCDOtherOfficeAddr3));
        sqlcmd.Parameters.Add(new SqlParameter("@SCCDOtherCountryCode", paramObj.SCCDOtherCountryCode));
        sqlcmd.Parameters.Add(new SqlParameter("@SCCDIsSanction", paramObj.SCCDIsSanction));
        sqlcmd.Parameters.Add(new SqlParameter("@SCCDIsSanctionCountryCode1", paramObj.SCCDIsSanctionCountryCode1));
        sqlcmd.Parameters.Add(new SqlParameter("@SCCDIsSanctionCountryCode2", paramObj.SCCDIsSanctionCountryCode2));
        sqlcmd.Parameters.Add(new SqlParameter("@SCCDIsSanctionCountryCode3", paramObj.SCCDIsSanctionCountryCode3));
        sqlcmd.Parameters.Add(new SqlParameter("@SCCDIsSanctionCountryCode4", paramObj.SCCDIsSanctionCountryCode4));
        sqlcmd.Parameters.Add(new SqlParameter("@SCCDIsSanctionCountryCode5", paramObj.SCCDIsSanctionCountryCode5));
        sqlcmd.Parameters.Add(new SqlParameter("@SCCDEquity", paramObj.SCCDEquity));
        sqlcmd.Parameters.Add(new SqlParameter("@SCCDCanBearerStock", paramObj.SCCDCanBearerStock));
        sqlcmd.Parameters.Add(new SqlParameter("@SCCDAlreadyBearerStock", paramObj.SCCDAlreadyBearerStock));
        sqlcmd.Parameters.Add(new SqlParameter("@keyin_Flag", paramObj.keyin_Flag));
        sqlcmd.Parameters.Add(new SqlParameter("@keyin_day", paramObj.keyin_day));
        sqlcmd.Parameters.Add(new SqlParameter("@keyin_userID", paramObj.keyin_userID));
        sqlcmd.Parameters.Add(new SqlParameter("@isNew", paramObj.isNew));
        sqlcmd.Parameters.Add(new SqlParameter("@isSCDD", paramObj.isSCDD));

        //20190806-RQ-2019-008595-002-長姓名需求 by Peggy
        sqlcmd.Parameters.Add(new SqlParameter("@PrincipalNameCH_L", paramObj.PrincipalNameCH_L));
        sqlcmd.Parameters.Add(new SqlParameter("@PrincipalNameCH_Pinyin", paramObj.PrincipalNameCH_Pinyin));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicContactMan_L", paramObj.BasicContactMan_L));
        sqlcmd.Parameters.Add(new SqlParameter("@BasicContactMan_Pinyin", paramObj.BasicContactMan_Pinyin));

        // 20210527 EOS_AML(NOVA) 新增欄位 by Ares Dennis
        sqlcmd.Parameters.Add(new SqlParameter("@REG_ZIP_CODE", paramObj.REG_ZIP_CODE));
        sqlcmd.Parameters.Add(new SqlParameter("@LAST_UPD_MAKER", paramObj.LAST_UPD_MAKER));
        sqlcmd.Parameters.Add(new SqlParameter("@LAST_UPD_CHECKER", paramObj.LAST_UPD_CHECKER));
        sqlcmd.Parameters.Add(new SqlParameter("@LAST_UPD_BRANCH", paramObj.LAST_UPD_BRANCH));
        //收集PARAMDEBUG
        //StringBuilder sb = new StringBuilder();
        //foreach (SqlParameter oitm in sqlcmd.Parameters)
        //{
        //    sb.AppendLine(string.Format(@"DECLARE {0} VARCHAR(255); set {0} = '{1}' ;", oitm.ParameterName, oitm.Value));

        //}
        //string df = sb.ToString();


        //將目前子項目建立為SQLCOM
        List<SqlCommand> senColl = getSeniorManagerSqlCom(paramObj.AML_SeniorManagerColl);

        ///使用交易
        using (OMTransactionScope ts = new OMTransactionScope())
        {
            result = Update(sqlcmd);
            if (!result)
            {
                return false;
            }
            //開始寫入
           foreach (SqlCommand scmd in senColl)
            {
                result = Update(scmd);
                if (!result)
                {
                    return false;
                }
            }
            ts.Complete();
        }
        return result;
    }

    /// <summary>
    /// 刪除1K 2K所有資料
    /// </summary>
    /// <param name="dataObj1K"></param>
    /// <param name="dataObj"></param>
    public static bool DeleteKData(string BasicTaxID)
    {
        bool result = false;
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        string sSQL = @"Delete from AML_SeniorManager where BasicTaxID =@BasicTaxID;
                        Delete from AML_HeadOffice where BasicTaxID =@BasicTaxID;
             ";

        sqlcmd.CommandText = sSQL;
        sqlcmd.Parameters.Add(new SqlParameter("@BasicTaxID", BasicTaxID));

        result = Update(sqlcmd);

        return result;
    }

    /// <summary>
    /// 依傳入物件及內容，產生對資料庫操作行為的Sqlcommond並回傳
    /// </summary>
    /// <param name="managerColl"></param>
    /// <returns></returns>
    /// 【修改】20190805 長姓名需求，新3個欄位 by Peggy↓
    private static List<SqlCommand> getSeniorManagerSqlCom(List<EntityAML_SeniorManager> managerColl)
    {
        List<SqlCommand> retColl = new List<SqlCommand>();
        foreach (EntityAML_SeniorManager paramObj in managerColl)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
               string sSQL ="";
            if (paramObj.ID == "") //新增
            {
                //檢核必填 //無必填欄位，略過
                if (string.IsNullOrEmpty(paramObj.IDNo))
                { continue; }

                sSQL = @"Insert into AML_SeniorManager
                        (BasicTaxID,Name,Birth,CountryCode,IDNo,IDNoType,[Identity1],[Identity2],[Identity3],[Identity4],[Identity5],[Identity6],Expdt,keyin_Flag,keyin_day,keyin_userID,LineID,isDEL,Name_L,Name_Pinyin ) 
                        VALUES(@BasicTaxID,@Name,@Birth,@CountryCode,@IDNo,@IDNoType,@Identity1,@Identity2,@Identity3,@Identity4,@Identity5,@Identity6,@Expdt,@keyin_Flag,@keyin_day,@keyin_userID,@LineID,@isDEL,@Name_L,@Name_Pinyin );
                        ";
            }
            else
            {
                //檢核必填 //無必填欄位，以ID刪除 勾選刪除也是
                if (string.IsNullOrEmpty(paramObj.IDNo) || paramObj.isDEL =="1") 
                {
                    sSQL = @"Delete from AML_SeniorManager where ID =@ID ;";
                }
                else  //Update
                {
                    sSQL = @" update AML_SeniorManager 
                             set BasicTaxID = @BasicTaxID,Name = @Name,Birth = @Birth,CountryCode = @CountryCode,IDNo = @IDNo,IDNoType = @IDNoType,[Identity1] = @Identity1,[Identity2] = @Identity2,[Identity3] = @Identity3,[Identity4] = @Identity4,[Identity5] = @Identity5,[Identity6] = @Identity6,Expdt = @Expdt,keyin_Flag = @keyin_Flag,keyin_day = @keyin_day,keyin_userID = @keyin_userID,LineID = @LineID,isDEL = @isDEL ,Name_L = @Name_L ,Name_Pinyin =@Name_Pinyin 
                            where ID = @ID ;";
                }

            }
            sqlcmd.CommandText = sSQL;
            sqlcmd.Parameters.Add(new SqlParameter("@ID", paramObj.ID));
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

            //20190806-RQ-2019-008595-002-長姓名需求 by Peggy
            sqlcmd.Parameters.Add(new SqlParameter("@Name_L", paramObj.Name_L));
            sqlcmd.Parameters.Add(new SqlParameter("@Name_Pinyin", paramObj.Name_Pinyin));
            retColl.Add(sqlcmd);
        }
        return retColl;

    }

    /// <summary>
    /// 刪除1Key 或 2Key非當天資料
    /// </summary>
    /// <param name="BasicTaxID"></param>
    /// <param name="keyin_flag"></param>
    /// <param name="today"></param>
    /// <returns></returns>
    public static bool DeleteNotTodayKData(string BasicTaxID, string keyin_flag, string today)
    {
        bool result = false;
        SqlCommand sqlcmd = new SqlCommand();
        try
        {
            sqlcmd.CommandType = CommandType.Text;
            string sSQL = @"Delete from AML_SeniorManager where BasicTaxID =@BasicTaxID and keyin_day != @today;
                        Delete from AML_HeadOffice where BasicTaxID =@BasicTaxID and keyin_day != @today;
             ";

            sqlcmd.CommandText = sSQL;
            sqlcmd.Parameters.Add(new SqlParameter("@BasicTaxID", BasicTaxID));
            sqlcmd.Parameters.Add(new SqlParameter("@today", today));

            result = Update(sqlcmd);

            return result;
        }
        catch(Exception ex)
        {
            Logging.Log(string.Format("刪除法人 {0} KEY資料發生錯誤！錯誤訊息：{1}", keyin_flag, ex));
            return result;
        }
        
    }
}