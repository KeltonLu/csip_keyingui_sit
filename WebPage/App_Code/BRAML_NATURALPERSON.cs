using System;
using System.Collections.Generic;
using System.Text;
using CSIPCommonModel.BusinessRules;
using CSIPKeyInGUI.EntityLayer_new;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using Framework.Common.Logging;
using System.Data.SqlClient;
using System.Data;

namespace CSIPKeyInGUI.BusinessRules_new
{
    /// <summary>
    /// AML_NATURALPERSON 業務類
    /// </summary>
    public class BRAML_NATURALPERSON : CSIPCommonModel.BusinessRules.BRBase<EntityAML_NATURALPERSON>
    {
        /// <summary>
        /// 根據自然人身分ID查詢
        /// 調整紀錄：新增查詢條件, MOD_DATE 須為當天日期(yyyyMMdd) by Ares Stanley 20211217
        /// </summary>
        /// <param name="ownerID">自然人身分ID</param>        
        /// <returns>EntitySet</returns>
        public static EntitySet<EntityAML_NATURALPERSON> SelectEntitySet(string ownerID, string keyInFlag, string today)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityAML_NATURALPERSON.M_OWNER_ID, Operator.Equal, DataTypeUtils.String, ownerID);
                sSql.AddCondition(EntityAML_NATURALPERSON.M_KEYIN_FLAG, Operator.Equal, DataTypeUtils.String, keyInFlag);
                sSql.AddCondition(EntityAML_NATURALPERSON.M_MOD_DATE, Operator.Equal, DataTypeUtils.String, today);

                return BRAML_NATURALPERSON.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }        

        /// <summary>
        /// 用Entity方式插入資料庫
        /// </summary>
        /// <param name="eShopBasic">Entity</param>
        /// <returns>true成功,false失敗</returns>
        public static bool AddEntity(EntityAML_NATURALPERSON eAMLNaturalPerson)
        {
            try
            {
                return eAMLNaturalPerson.DB_InsertEntity();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新SHOP_BASIC
        /// </summary>
        /// <param name="eShopBasic">實體</param>
        /// <param name="strUniNo1">統一編號（1）</param>
        /// <param name="strUniNo2">統一編號（2）</param>
        /// <param name="strFields">要更新列的數組</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Update(EntityAML_NATURALPERSON eAMLNaturalPerson, string ownerID, string strKeyInFlag)
        {
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntityAML_NATURALPERSON.M_OWNER_ID, Operator.Equal, DataTypeUtils.String, ownerID);
            sSql.AddCondition(EntityAML_NATURALPERSON.M_KEYIN_FLAG, Operator.Equal, DataTypeUtils.String, strKeyInFlag);            

            return eAMLNaturalPerson.DB_UpdateEntity(sSql.GetFilterCondition());
        }

        /// <summary>
        /// 讀取1KEY.2KEY 資料
        /// </summary>
        /// <param name="BasicTaxID"></param>
        /// <param name="ID"></param>
        /// <param name="keyin_Flag"></param>
        /// <returns></returns>
        public static EntityAML_NATURALPERSON Query(string OWNER_ID, string SNO, string keyin_Flag, string today)
        {
            string sql = @"
                    SELECT top 1 [SNO],[OWNER_ID],[NameCH] ,[NameEN] ,[NameCH_L] ,[NameCH_Pinyin] ,[NameCH_OLD] ,[BIRTH_DATE]
                     ,[GENDER] ,[CountryCode] ,[CountryCode2] ,[CountryStateCode] ,[ID_ISSUEDATE] ,[ID_ISSUEPLACE] ,[ID_REPLACETYPE]
                     ,[ID_PHOTOFLAG] ,[REG_ZIPCODE] ,[REG_CITY] ,[REG_ADDR1] ,[REG_ADDR2] ,[MAILING_CITY] ,[MAILING_ADDR1]
                     ,[MAILING_ADDR2] ,[EMAIL] ,[COMP_TEL1] ,[COMP_TEL2] ,[COMP_TEL3], [MOBILE] ,[NP_COMPANY_NAME] ,[Industry1]
                     ,[Industry2] ,[Industry3] ,[CC] ,[CC2] ,[CC3]
                     ,[TITLE] ,[OC] ,[INCOME_SOURCE] ,[OWNER_ID_OLD] ,[LAST_UPD_MAKER] ,[LAST_UPD_CHECKER]
                     ,[LAST_UPD_BRANCH] ,[isSCDD] ,[KEYIN_FLAG] ,[MOD_DATE] ,[MOD_USERID]
                      FROM  [AML_NATURALPERSON]
                     WHERE OWNER_ID = @OWNER_ID and KEYIN_FLAG =@KEYIN_FLAG and MOD_DATE = @today ";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            sqlcmd.Parameters.Add(new SqlParameter("@OWNER_ID", OWNER_ID));
            //   sqlcmd.Parameters.Add(new SqlParameter("@SNO", SNO)); //暫未使用
            sqlcmd.Parameters.Add(new SqlParameter("@KEYIN_FLAG", keyin_Flag));  //區分1-2Key
            sqlcmd.Parameters.Add(new SqlParameter("@today", today)); //新增查詢限制, 只能查到當天資料 by Ares Stanley 20211217

            try
            {
                DataTable dt = new DataTable();
                DataSet DS = SearchOnDataSet(sqlcmd);
                if (DS != null && DS.Tables.Count > 0)
                {
                    dt = DS.Tables[0];
                }
                EntityAML_NATURALPERSON rtn = new EntityAML_NATURALPERSON();
                if (dt.Rows.Count > 0)
                {
                    DataTableConvertor.convSingRow<EntityAML_NATURALPERSON>(ref rtn, dt.Rows[0]);                    
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
        /// 刪除1K 2K所有資料
        /// </summary>
        /// <param name="dataObj1K"></param>
        /// <param name="dataObj"></param>
        public static bool DeleteKData(string OWNER_ID)
        {
            bool result = false;
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            string sSQL = @"Delete from AML_NATURALPERSON where OWNER_ID =@OWNER_ID;                        
             ";

            sqlcmd.CommandText = sSQL;
            sqlcmd.Parameters.Add(new SqlParameter("@OWNER_ID", OWNER_ID));

            result = Update(sqlcmd);

            return result;
        }

        /// <summary>
        /// 刪除1Key 或 2Key非當天資料
        /// </summary>
        /// <param name="dataObj1K"></param>
        /// <param name="dataObj"></param>
        public static bool DeleteNotTodayKData(string OWNER_ID, string keyin_flag, string today)
        {
            bool result = false;
            SqlCommand sqlcmd = new SqlCommand();
            try
            {
                sqlcmd.CommandType = CommandType.Text;
                string sSQL = @"Delete from AML_NATURALPERSON where OWNER_ID =@OWNER_ID and MOD_DATE != @today;                        
             ";

                sqlcmd.CommandText = sSQL;
                sqlcmd.Parameters.Add(new SqlParameter("@OWNER_ID", OWNER_ID));
                sqlcmd.Parameters.Add(new SqlParameter("@today", today));

                result = Update(sqlcmd);

                return result;
            }
            catch(Exception ex)
            {
                Logging.Log(string.Format("刪除自然人 {0} KEY資料發生錯誤！錯誤訊息：{1}", keyin_flag, ex));
                return result;
            }
        }

    }
}
