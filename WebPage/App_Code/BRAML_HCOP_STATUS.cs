//******************************************************************
//*  作    者：陳香琦
//*  功能說明：HCOP Status檔
//*  創建日期：2019/12/27
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*陳香琦                  2020/02/17                                        多收主機給的不合作註記欄位
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
    public class BRAML_HCOP_STATUS : BRBase<EntityAML_HCOP_STATUS>
    {
        /// <summary>
        /// 讀取案件明細表頭
        /// </summary>
        /// <returns></returns>
        public static EntityAML_HCOP_STATUS getHCOPData_WOrk(AML_SessionState parmObj)
        {
            //20200217-RQ-2019-030155-003-新增不合作註記欄位(NonCooperation)
            string sSQL = @"SELECT [CORP_NO],[CORP_SEQ],[STATUS],[QUALIFY_FLAG],[MOD_DATE],[NonCooperation]   
                                            FROM [dbo].[AML_HCOP_STATUS] WHERE [CORP_NO] = @CORP_NO And [CORP_SEQ] =@CORP_SEQ";

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sSQL;
            sqlcmd.Parameters.Add(new SqlParameter("@CORP_NO", parmObj.HCOP_HEADQUATERS_CORP_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@CORP_SEQ", "0000"));

            EntityAML_HCOP_STATUS rtnObj = new EntityAML_HCOP_STATUS();
            DataTable dt = SearchOnDataSet(sqlcmd).Tables[0];
            if (dt.Rows.Count > 0)
            {
                DataTableConvertor.convSingRow<EntityAML_HCOP_STATUS>(ref rtnObj, dt.Rows[0]);
            }
            return rtnObj;
        }

        /// <summary>
        /// 查詢HCOP資料
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DataTable GetHCOPData(string _corpNo, string _corpSeq)
        {
            //20200217-RQ-2019-030155-003-新增不合作註記欄位(NonCooperation)
            string sql = @"SELECT [CORP_NO],[CORP_SEQ],[STATUS],[QUALIFY_FLAG],[MOD_DATE],[NonCooperation]     
                                        FROM [dbo].[AML_HCOP_STATUS] WHERE [CORP_NO] = @CORP_NO And [CORP_SEQ] =@CORP_SEQ";
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
                Logging.Log("查詢總公司狀態檔資料：" + ex, LogLayer.BusinessRule);
                return null;
            }
        }
        
        public static bool Add(EntityAML_HCOP_STATUS eAML_HCOP_STATUS, ref string strMsg)
        {
            bool result = false;
            if (AddNewEntity(eAML_HCOP_STATUS))
            {
                strMsg = "新增成功";
                result = true;
            }
            else
                strMsg = "新增失敗";
            return result;
        }

        public static bool Update(EntityAML_HCOP_STATUS eAML_HCOP_STATUS, ref string strMsg)
        {
            //20200217-RQ-2019-030155-003-新增不合作註記欄位(NonCooperation)
            bool result = false;
            string sqlCommand = @"UPDATE [dbo].[AML_HCOP_STATUS]
                                                            SET [STATUS] = @STATUS
                                                            ,[QUALIFY_FLAG] = @QUALIFY_FLAG
                                                            ,[MOD_DATE] = GETDATE()
                                                            ,[NonCooperation] = @NonCooperation  
                                                            WHERE [CORP_ NO] =@CORP_NO And [CORP_SEQ] = @CORP_SEQ";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sqlCommand;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@CORP_NO", eAML_HCOP_STATUS.CORP_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@CORP_SEQ", eAML_HCOP_STATUS.CORP_SEQ));
            sqlcmd.Parameters.Add(new SqlParameter("@STATUS", eAML_HCOP_STATUS.STATUS));
            sqlcmd.Parameters.Add(new SqlParameter("@QUALIFY_FLAG", eAML_HCOP_STATUS.QUALIFY_FLAG));
            sqlcmd.Parameters.Add(new SqlParameter("@NonCooperation", eAML_HCOP_STATUS.NonCooperation));
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
            string sqlCommand = @"DELETE FROM [dbo].[AML_HCOP_STATUS]
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

            //20200212 Prod似乎沒有Truncate權限，故改用del的語法
            //string sqlCommand = @"Truncate Table [dbo].[AML_HCOP_STATUS] ";
            string sqlCommand = @"DELETE FROM [dbo].[AML_HCOP_STATUS] ";
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

        // 整批寫入 AML_HCOP_STATUS
        public static bool AML_HCOP_STATUSWithBulkCopy(string tableName, DataTable dataTable, ref string errorMsg)
        {
            //20200217-RQ-2019-030155-003-新增不合作註記欄位(NonCooperation)
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
                sbc.ColumnMappings.Add("STATUS", "STATUS");                                                             // 總公司狀態
                sbc.ColumnMappings.Add("QUALIFY_FLAG", "QUALIFY_FLAG");                                  // QUALIFY_FLAG
                sbc.ColumnMappings.Add("REG_NAME", "REG_NAME");                                               // 登記名稱
                sbc.ColumnMappings.Add("MOD_DATE", "MOD_DATE");                                              // 異動日
                sbc.ColumnMappings.Add("NonCooperation", "NonCooperation");                          // 不合作註記

                sbc.WriteToServer(dataTable);

                result = true;                
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;//20200211 把錯誤訊息pass到外層
                Logging.Log(ex, LogLayer.DB);
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

        //20200215-RQ-2019-030155-003
        /// <summary>
        /// FOR 6001鍵檔時判斷
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static DataTable GetHCOPQualify_Flag(string _CorpNo , string _CorpSeq = "0000")
        {
            string sql = @" SELECT QUALIFY_FLAG,NonCooperation FROM AML_HCOP_STATUS WHERE CORP_NO = @corpno AND CORP_SEQ= @corpseq";
            
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sql;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@corpno", _CorpNo));
            sqlcmd.Parameters.Add(new SqlParameter("@corpseq", _CorpSeq));
            

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
                Logging.Log(ex, LogLayer.DB);
            }

            return result;
        }
    }
}