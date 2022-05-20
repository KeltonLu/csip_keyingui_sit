//******************************************************************
//*  作    者：陳香琦
//*  功能說明：MFAF設定檔
//*  創建日期：2019/12/27
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
    public class BRAML_HQ_MFAF : BRBase<EntityAML_HQ_MFAF>
    {
        /// <summary>
        /// 讀取案件明細表頭
        /// </summary>
        /// <returns></returns>
        public static EntityAML_HQ_MFAF getMFAFData_WOrk(AML_SessionState parmObj)
        {
            string sSQL = @"SELECT [CORP_NO],[CORP_SEQ],[MFAF_ID],[MFAF_NAME],[MFAF_AREA],[MFAF_UPDATE_DATE],[MOD_DATE]  
                                        FROM [dbo].[AML_HQ_MFAF] WHERE [CORP_NO] = @CORP_NO And [CORP_SEQ] =@CORP_SEQ";

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sSQL;
            int intTemp;
            //開頭是數字為法人，否則為自然人
            if (Int32.TryParse(parmObj.HCOP_HEADQUATERS_CORP_NO.Substring(0, 1), out intTemp))
            {
                sqlcmd.Parameters.Add(new SqlParameter("@CORP_NO", parmObj.HCOP_HEADQUATERS_CORP_NO));
                sqlcmd.Parameters.Add(new SqlParameter("@CORP_SEQ", "0000"));
            }
            else
            {
                sqlcmd.Parameters.Add(new SqlParameter("@CORP_NO", parmObj.HCOP_HEADQUATERS_CORP_NO.Substring(0, 8) ));
                sqlcmd.Parameters.Add(new SqlParameter("@CORP_SEQ", parmObj.HCOP_HEADQUATERS_CORP_NO.Substring(8, 2)));
            }
            
            
            EntityAML_HQ_MFAF rtnObj = new EntityAML_HQ_MFAF();
            DataTable dt = SearchOnDataSet(sqlcmd).Tables[0];
            if (dt.Rows.Count > 0)
            {
                DataTableConvertor.convSingRow<EntityAML_HQ_MFAF>(ref rtnObj, dt.Rows[0]);
            }
            return rtnObj;
        }

        /// <summary>
        /// 查詢MFAF資料
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DataTable GetCodeType(string _corpNo, string _corpSeq)
        {
            string sql = @"SELECT [CORP_NO],[CORP_SEQ],[MFAF_ID],[MFAF_NAME],[MFAF_AREA],[MFAF_UPDATE_DATE],[MOD_DATE]  
                                        FROM [dbo].[AML_HQ_MFAF] WHERE [CORP_NO] = @CORP_NO And [CORP_SEQ] =@CORP_SEQ";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
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
                Logging.Log("查詢MFAF資料：" + ex, LogLayer.BusinessRule);
                return null;
            }
        }
        
        public static bool Add(EntityAML_HQ_MFAF ePostOfficeCodeType, ref string strMsg)
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

        public static bool Update(EntityAML_HQ_MFAF eAML_HQ_MFAF, ref string strMsg)
        {
            bool result = false;
            string sqlCommand = @"UPDATE [dbo].[AML_HQ_MFAF]
                                                            SET [MFAF_ID] = @MFAF_ID
                                                            ,[MFAF_NAME] = @MFAF_NAME
                                                            ,[MFAF_AREA] = @MFAF_AREA
                                                            ,[MFAF_UPDATE_DATE] = @MFAF_UPDATE_DATE
                                                            ,[MOD_DATE] = GETDATE()
                                                            WHERE [CORP_ NO] =@CORP_NO And [CORP_SEQ] = @CORP_SEQ";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sqlCommand;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@CORP_NO", eAML_HQ_MFAF.CORP_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@CORP_SEQ", eAML_HQ_MFAF.CORP_SEQ));
            sqlcmd.Parameters.Add(new SqlParameter("@MFAF_ID", eAML_HQ_MFAF.MFAF_ID));
            sqlcmd.Parameters.Add(new SqlParameter("@MFAF_NAME", eAML_HQ_MFAF.MFAF_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@MFAF_AREA", eAML_HQ_MFAF.MFAF_AREA));
            sqlcmd.Parameters.Add(new SqlParameter("@MFAF_UPDATE_DATE", eAML_HQ_MFAF.MFAF_UPDATE_DATE));
            if (Update(sqlcmd))
            {
                strMsg = "更新成功";
                result = true;
            }
            else
                strMsg = "更新失敗";
            return result;
        }

        public static bool Delete(string _corpNo, string _corpSeq, ref string strMsg)
        {
            string sqlCommand = @"DELETE FROM [dbo].[AML_HQ_MFAF]
                                                             WHERE [CORP_ NO] =@CORP_NO And [CORP_SEQ] = @CORP_SEQ";
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

        public static bool Truncate(ref string strMsg)
        {
            bool result = false;

            //20200212 Prod似手沒有Truncate權限，故改用del的語法
            //string sqlCommand = @"Truncate Table [dbo].[AML_HQ_MFAF] ";
            string sqlCommand = @"DELETE FROM [dbo].[AML_HQ_MFAF] ";
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
                strMsg = exp.Message;//20200211 將錯誤訊息回傳至前端
                Logging.Log(exp, LogLayer.DB);
                result = false;
            }

            return result;
        }

        // 整批寫入 AML_HQ_MFAF
        public static bool AML_HQ_MFAFWithBulkCopy(string tableName, DataTable dataTable, ref string errorMsg)
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

                sbc.ColumnMappings.Add("CORP_NO", "CORP_NO");                                                    // 統編
                sbc.ColumnMappings.Add("CORP_SEQ", "CORP_SEQ");                                                  // 統編序號
                sbc.ColumnMappings.Add("MFAF_ID", "MFAF_ID");                                                         // MFAF員編
                sbc.ColumnMappings.Add("MFAF_NAME", "MFAF_NAME");                                         // MFAF姓名
                sbc.ColumnMappings.Add("MFAF_AREA", "MFAF_AREA");                                             // MFAF區域中心
                sbc.ColumnMappings.Add("MFAF_UPDATE_DATE", "MFAF_UPDATE_DATE");          // MFAF生效日
                sbc.ColumnMappings.Add("MOD_DATE", "MOD_DATE");                                              // 異動日

                sbc.WriteToServer(dataTable);

                result = true;
                
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;//20200211 把錯誤訊息pass到外層
                result = false;
                Logging.Log(ex, LogLayer.DB);
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