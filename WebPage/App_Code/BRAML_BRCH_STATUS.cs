//******************************************************************
//*  作    者：陳香琦
//*  功能說明：BRCH Status檔
//*  創建日期：2020/05/29
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*Ares Luke          2021/02/22         20200031-CSIP EOS       調整connectionStr方式
//*******************************************************************
using System;
using System.Data;
using System.Data.SqlClient;
using CSIPCommonModel.BusinessRules;
using CSIPCommonModel.EntityLayer_new;
using Framework.Data;
using Framework.Common.Logging;
using Framework.Common.Utility;

namespace CSIPKeyInGUI.BusinessRules_new
{
    public class BRAML_BRCH_STATUS : BRBase<EntityAML_BRCH_STATUS>
    {
        /// <summary>
        /// 查詢HCOP資料
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DataTable GetHCOPData(string _corpNo, string _corpSeq)
        {
            string sSQL = @"SELECT [CORP_NO]
                                          ,[CORP_SEQ]
                                          ,[REG_ENG_NAME]
                                          ,[REG_CHI_NAME]
                                          ,[CREATE_DATE]
                                          ,[STATUS]
                                          ,[QUALIFY_FLAG]
                                          ,[CIRCULATE_MERCH]
                                          ,[UPDATE_DATE]
                                          ,[HQ_CORP_NO]
                                          ,[HQ_CORP_SEQ]
                                          ,[IMPORT_DATE]
                                      FROM [dbo].[AML_BRCH_STATUS] 
                                    WHERE [CORP_NO] = @CORP_NO And [CORP_SEQ] =@CORP_SEQ";

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sSQL;
            SqlParameter parmQuery = new SqlParameter("@CORP_NO", _corpNo);
            SqlParameter parmQuery2 = new SqlParameter("@CORP_SEQ", _corpSeq);
            sqlcmd.Parameters.Add(parmQuery);
            sqlcmd.Parameters.Add(parmQuery2);

            try
            {
                DataTable dt = BRPostOffice_Temp.SearchOnDataSet(sqlcmd).Tables[0];

                return dt;
            }
            catch (Exception ex)
            {
                //Logging.SaveLog(ELogLayer.BusinessRule, "查詢分公司狀態檔資料：" + ex);
                Logging.Log("查詢分公司狀態檔資料：" + ex, LogState.Error, LogLayer.BusinessRule);
                return null;
            }
        }
        
        public static bool Add(EntityAML_BRCH_STATUS eAML_BRCH_STATUS, ref string strMsg)
        {
            bool result = false;
            if (AddNewEntity(eAML_BRCH_STATUS))
            {
                strMsg = "新增成功";
                result = true;
            }
            else
                strMsg = "新增失敗";
            return result;
        }

        public static bool Delete(string _corpNo, string _corpSeq, ref string strMsg)
        {
            string sqlCommand = @"DELETE FROM [dbo].[AML_BRCH_STATUS]
                                                             WHERE [CORP_NO] =@CORP_NO And [CORP_SEQ] = @CORP_SEQ";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sqlCommand;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@CORP_NO", _corpNo));
            sqlcmd.Parameters.Add(new SqlParameter("@CORP_SEQ", _corpSeq));

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

        public static bool DelALL(ref string strMsg)
        {
            bool result = false;
            
            string sqlCommand = @"DELETE FROM [dbo].[AML_BRCH_STATUS] ";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sqlCommand;
            sqlcmd.CommandType = CommandType.Text;

            DataHelper dh = new DataHelper();
            try
            {
                dh.ExecuteNonQuery(sqlcmd);
                result = true;
            }
            catch (Exception exp)
            {
                strMsg = exp.Message;
                //Logging.SaveLog(ELogLayer.DB, exp.ToString());
                Logging.Log("查詢分公司狀態檔資料：" + exp, LogState.Error, LogLayer.DB);
                result = false;
            }

            return result;
        }

        // 整批寫入 AML_BRCH_STATUS
        public static bool AML_BRCH_STATUSWithBulkCopy(string tableName, DataTable dataTable, ref string errorMsg)
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

                sbc.ColumnMappings.Add("CORP_NO", "CORP_NO");                           // 分公司統編
                sbc.ColumnMappings.Add("CORP_SEQ", "CORP_SEQ");                        // 分公司統編序號
                sbc.ColumnMappings.Add("REG_ENG_NAME", "REG_ENG_NAME"); // 登記英文名稱
                sbc.ColumnMappings.Add("REG_CHI_NAME", "REG_CHI_NAME");                     // 登記中文名稱
                sbc.ColumnMappings.Add("CREATE_DATE", "CREATE_DATE");                                           // 最早開店日期
                sbc.ColumnMappings.Add("STATUS", "STATUS");                                                           // 分公司狀態
                sbc.ColumnMappings.Add("QUALIFY_FLAG", "QUALIFY_FLAG");                                // 符合資格FLAG
                sbc.ColumnMappings.Add("CIRCULATE_MERCH", "CIRCULATE_MERCH");                                            // 流通的店
                sbc.ColumnMappings.Add("UPDATE_DATE", "UPDATE_DATE");                                              // 異動日
                sbc.ColumnMappings.Add("HQ_CORP_NO", "HQ_CORP_NO");     // 總公司統編
                sbc.ColumnMappings.Add("HQ_CORP_SEQ", "HQ_CORP_SEQ");  // 總公司統編序號
                sbc.ColumnMappings.Add("IMPORT_DATE", "IMPORT_DATE");                                   // 匯入日期

                sbc.WriteToServer(dataTable);

                result = true;                
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                //Logging.SaveLog(ELogLayer.DB, ex.ToString());
                Logging.Log( ex.ToString(), LogState.Error, LogLayer.DB);
                return result;
            }
            finally
            {
                sbc.Close();
                conn.Close();
                conn.Dispose();
            }

            return result;
        }
        
    }
}