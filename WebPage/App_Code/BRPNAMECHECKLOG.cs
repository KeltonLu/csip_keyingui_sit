//******************************************************************
//*  作    者：林益慶
//*  功能說明：ESB-NameCheckLog
//*  創建日期：2020/04/20
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//* 
//*******************************************************************
using System.Data;
using System.Data.SqlClient;
using CSIPCommonModel.BusinessRules;
using CSIPCommonModel.EntityLayer_new;
using System;

namespace CSIPKeyInGUI.BusinessRules_new
{
    public class BRPNAMECHECKLOG : BRBase<Entity_NAMECHECKLOG>
    {
        public static bool Add(Entity_NAMECHECKLOG ePostOfficeCodeType, ref string strMsg)
        {
            bool result = false;
            if (AddNewEntity(ePostOfficeCodeType))
            {
                strMsg = "新增成功";
                result = true;
            }
            else
                strMsg = "新增失敗";
            return result;
        }

        public static bool getNameChecklogNoE(AML_SessionState parmObj)
        {
            string sSQL = @"  SELECT
                              CASE_NO ,
                              SEQ,
                              REPCODE,
                              TRNNUM,
                              BankNo,
                              BranchNo,
                              ID,
                              EnglishName,
                              NonEnglishName,
                              DOB,
                              Nationality,
                              TellerName,
                              Type,
                              AddressCountry,
                              ConnectedPartyType,
                              ConnectedPartySubType,
                              MatchedResult,
                              RCScore,
                              ReferenceNumber,
                              AMLReferenceNumber,
                              ERRORDESC,
                              KIND,
                              Query_datetime,
                              ReQueryFlg,
                              CRE_USER,
                              CRE_DATE,
                              MOD_USER,
                              MOD_DATE
                              FROM [NAMECHECKLOG] where CASE_NO =@CASE_NO and KIND ='R' and MatchedResult <> 'E' ;";

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sSQL;
            sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", parmObj.CASE_NO));

            DataTable dt = SearchOnDataSet(sqlcmd).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        public static bool Delete(string CASE_NO , string TRNNUM, ref string strMsg)
        {
            string sqlCommand = @"DELETE FROM [dbo].[NAMECHECKLOG]
                                  Where [CASE_NO] =@CASE_NO And [TRNNUM] =@TRNNUM ";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sqlCommand;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", CASE_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@TRNNUM", TRNNUM));
            bool result = false;
            if (Delete(sqlcmd))
            {
                result = true;
                strMsg = "刪除成功";
            }
            else
                strMsg = "刪除失敗";
            return result;
        }

        // NameCheckLog 明細資料
        public static bool getNameChecklog_Detail(AML_SessionState parmObj, ref DataTable Dt)
        {
            string sSQL = @"  SELECT
                              CASE_NO ,
                              SEQ,
                              REPCODE,
                              TRNNUM,
                              BankNo,
                              BranchNo,
                              ID,
                              EnglishName,
                              NonEnglishName,
                              DOB,
                              Nationality,
                              TellerName,
                              Type,
                              AddressCountry,
                              ConnectedPartyType,
                              ConnectedPartySubType,
                              MatchedResult,
                              RCScore,
                              ReferenceNumber,
                              AMLReferenceNumber,
                              ERRORDESC,
                              KIND,
                              Query_datetime,
                              ReQueryFlg,
                              CRE_USER,
                              CRE_DATE,
                              MOD_USER,
                              MOD_DATE
                              FROM [NAMECHECKLOG] where CASE_NO =@CASE_NO  ORDER BY TRNNUM DESC,SEQ,KIND DESC;";

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sSQL;
            sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", parmObj.CASE_NO));

            DataTable dt = SearchOnDataSet(sqlcmd).Tables[0];
            if (dt.Rows.Count > 0)
            {
                Dt = dt.Copy();
                return true;
            }
            return false;
        }

        /// <summary>
        /// NameCheckLog 比對資料 是否已比對過
        /// 2020.06.29 RQ-2019-030155-000 Ray Add 加入 ConnectedPartyType、ConnectedPartySubType判斷
        /// </summary>
        /// <param name="CASE_NO"></param>
        /// <param name="ID"></param>
        /// <param name="ChinesehName">中文姓名</param>
        /// <param name="EnglishName">英文姓名</param>
        /// <returns></returns>
        public static bool NameChecklog_Compare(string CASE_NO, string ID, string ChinesehName, string EnglishName, string ConnectedPartyType, string ConnectedPartySubType)
        {
            string sSQL = @"  SELECT
                              CASE_NO ,
                              SEQ,
                              REPCODE,
                              TRNNUM,
                              BankNo,
                              BranchNo,
                              ID,
                              EnglishName,
                              NonEnglishName,
                              DOB,
                              Nationality,
                              TellerName,
                              Type,
                              AddressCountry,
                              ConnectedPartyType,
                              ConnectedPartySubType,
                              MatchedResult,
                              RCScore,
                              ReferenceNumber,
                              AMLReferenceNumber,
                              ERRORDESC,
                              KIND,
                              Query_datetime,
                              ReQueryFlg,
                              CRE_USER,
                              CRE_DATE,
                              MOD_USER,
                              MOD_DATE
                              FROM [NAMECHECKLOG]  where CASE_NO =@CASE_NO 
                              and EnglishName=@EnglishName  and  NonEnglishName=@NonEnglishName and ID=@ID
                              and ConnectedPartyType=@ConnectedPartyType and ConnectedPartySubType=@ConnectedPartySubType
                              --將下行回傳MatchedResult為 E 的TRNNUM排除，資料可重新拋查
                              and TRNNUM NOT IN (SELECT TRNNUM FROM [NAMECHECKLOG] where CASE_NO = @CASE_NO  and MatchedResult = 'E') ";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sSQL;
            sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", CASE_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@EnglishName", EnglishName));
            sqlcmd.Parameters.Add(new SqlParameter("@NonEnglishName", ChinesehName));
            sqlcmd.Parameters.Add(new SqlParameter("@ConnectedPartyType", ConnectedPartyType));
            sqlcmd.Parameters.Add(new SqlParameter("@ConnectedPartySubType", ConnectedPartySubType));
            sqlcmd.Parameters.Add(new SqlParameter("@ID", ID));

            DataTable dt = SearchOnDataSet(sqlcmd).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// 作者:Ares_Jack
        /// 創建日期:20220111
        /// 修改日期:20220111
        /// <summary>
        /// 自然人 NameCheckLog 比對資料 是否已比對過
        /// </summary>
        /// <param name="CASE_NO"></param>
        /// <param name="ID"></param>
        /// <param name="ChinesehName">中文姓名</param>
        /// <param name="EnglishName">英文姓名</param>
        /// <param name="CountryCode">國籍</param>
        /// <returns></returns>
        public static bool NameChecklog_Compare_Natural_Person(string CASE_NO, string ID, string ChinesehName, string EnglishName, string ConnectedPartyType, string ConnectedPartySubType, string CountryCode)
        {
            string sSQL = @"  SELECT
                              CASE_NO ,
                              SEQ,
                              REPCODE,
                              TRNNUM,
                              BankNo,
                              BranchNo,
                              ID,
                              EnglishName,
                              NonEnglishName,
                              DOB,
                              Nationality,
                              TellerName,
                              Type,
                              AddressCountry,
                              ConnectedPartyType,
                              ConnectedPartySubType,
                              MatchedResult,
                              RCScore,
                              ReferenceNumber,
                              AMLReferenceNumber,
                              ERRORDESC,
                              KIND,
                              Query_datetime,
                              ReQueryFlg,
                              CRE_USER,
                              CRE_DATE,
                              MOD_USER,
                              MOD_DATE
                              FROM [NAMECHECKLOG]  where CASE_NO =@CASE_NO 
                              and EnglishName=@EnglishName  and  NonEnglishName=@NonEnglishName and ID=@ID
                              and ConnectedPartyType=@ConnectedPartyType and ConnectedPartySubType=@ConnectedPartySubType and Nationality=@Nationality 
                              --將下行回傳MatchedResult為 E 的TRNNUM排除，資料可重新拋查
                              and TRNNUM NOT IN (SELECT TRNNUM FROM [NAMECHECKLOG] where CASE_NO = @CASE_NO  and MatchedResult = 'E') ";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sSQL;
            sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", CASE_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@EnglishName", EnglishName));
            sqlcmd.Parameters.Add(new SqlParameter("@NonEnglishName", ChinesehName));
            sqlcmd.Parameters.Add(new SqlParameter("@ConnectedPartyType", ConnectedPartyType));
            sqlcmd.Parameters.Add(new SqlParameter("@ConnectedPartySubType", ConnectedPartySubType));
            sqlcmd.Parameters.Add(new SqlParameter("@ID", ID));
            sqlcmd.Parameters.Add(new SqlParameter("@Nationality", CountryCode));

            DataTable dt = SearchOnDataSet(sqlcmd).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}