//******************************************************************
//*  作    者：蘇洺葳
//*  功能說明：國籍+組織資訊
//*  創建日期：2018/10/22
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*Ares Luke          2021/02/22         20200031-CSIP EOS       調整connectionStr方式
//*******************************************************************
using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Text;
//using CSIPCommonModel.BaseItem;
using CSIPCommonModel.BusinessRules;
using CSIPCommonModel.EntityLayer_new;
using CSIPKeyInGUI.BusinessRules;
using Framework.Data;
using Framework.Common;
using Framework.Common.Message;
using Framework.Common.Logging;
using Framework.Common.Utility;

namespace CSIPKeyInGUI.BusinessRules_new
{
    public class BRPostOffice_CodeType : BRBase<EntityPostOffice_CodeType>
    {
        /// 作者 Ares Jack
        /// 創建日期：2021/11/22
        /// 修改日期：2021/11/26
        /// <summary>
        /// 查詢MAKER, CHECKER檢核欄位 
        /// </summary>
        /// <param name="MFA_ID"></param>
        /// <returns></returns>
        public static bool GetMFA_ID(string MFA_ID )
        {
            string sql = @"SELECT * FROM [dbo].[AML_HQ_MFAN] WHERE [MFA_ID] = @MFA_ID ";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            SqlParameter parmQuery = new SqlParameter("@MFA_ID", MFA_ID);
            sqlcmd.Parameters.Add(parmQuery);

            try
            {
                DataTable dt = BRPostOffice_Temp.SearchOnDataSet(sqlcmd).Tables[0];
                if (dt.Rows.Count > 0)
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                Logging.Log("查詢MAKER, CHECKER檢核欄位資料失敗：" + ex, LogState.Error, LogLayer.BusinessRule);
                return false;
            }
        }

        /// <summary>
        /// 查詢國籍或組織資料
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DataTable GetCodeType(string type)
        {
            string sql = @"SELECT CODE_ID, CODE_NAME, CODE_EN_NAME,CODE_ID, CODE_ID+' : '+CODE_NAME AS 'CODE_IDNAME' FROM dbo.PostOffice_CodeType WHERE [TYPE] = @Type And [IsValid] !=0";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            SqlParameter parmQuery = new SqlParameter("@Type", type);
            sqlcmd.Parameters.Add(parmQuery);

            try
            {
                DataTable dt = BRPostOffice_Temp.SearchOnDataSet(sqlcmd).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                Logging.Log("查詢已傳送郵局資料失敗：" + ex, LogState.Error, LogLayer.BusinessRule);
                return null;
            }
        }

        /// <summary>
        /// 以CODE及類別取得中英文名稱
        /// </summary>
        /// <param name="type"></param>
        /// <param name="codeID"></param>
        /// <returns></returns>
        public static DataTable GetCodeTypeByCodeID(string type,string codeID)
        {
            string sql = @"SELECT CODE_ID, CODE_NAME, CODE_EN_NAME ,IsValid, DESCRIPTION FROM dbo.PostOffice_CodeType WHERE [TYPE] = @Type and CODE_ID = @CODE_ID";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            SqlParameter parmQuery = new SqlParameter("@Type", type);
            SqlParameter parmQuery2 = new SqlParameter("@CODE_ID", codeID);
            sqlcmd.Parameters.Add(parmQuery);
            sqlcmd.Parameters.Add(parmQuery2);

            try
            {
                DataTable dt = BRPostOffice_Temp.SearchOnDataSet(sqlcmd).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                Logging.Log("查詢已傳送郵局資料失敗：" + ex, LogState.Error, LogLayer.BusinessRule);
                return null;
            }
        }

        public static bool Add(EntityPostOffice_CodeType ePostOfficeCodeType, ref string strMsg)
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

        public static bool Update(EntityPostOffice_CodeType ePostOfficeCodeType, ref string strMsg)
        {
            bool result = false;
            string sqlCommand = @"UPDATE [dbo].[PostOffice_CodeType]
                                SET [CODE_ID] = @CODE_ID
                                ,[CODE_NAME] = @CODE_NAME
                                ,[CODE_EN_NAME] = @CODE_EN_NAME
                                ,[ORDERBY] = @ORDERBY
                                ,[DESCRIPTION] = @DESCRIPTION
                                ,[IsValid] = @IsValid
                                WHERE [ID] =@ID And [TYPE] = @TYPE";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sqlCommand;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@CODE_ID", ePostOfficeCodeType.CODE_ID));
            sqlcmd.Parameters.Add(new SqlParameter("@CODE_NAME", ePostOfficeCodeType.CODE_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@CODE_EN_NAME", ePostOfficeCodeType.CODE_EN_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@ORDERBY", ePostOfficeCodeType.ORDERBY));
            sqlcmd.Parameters.Add(new SqlParameter("@IsValid", ePostOfficeCodeType.IsValid));
            sqlcmd.Parameters.Add(new SqlParameter("@ID", ePostOfficeCodeType.ID));
            sqlcmd.Parameters.Add(new SqlParameter("@TYPE", ePostOfficeCodeType.TYPE));
            sqlcmd.Parameters.Add(new SqlParameter("@DESCRIPTION", ePostOfficeCodeType.DESCRIPTION));//20211207_Ares_Jack_增加描述
            if (Update(sqlcmd))
            {
                strMsg = "更新成功";
                result = true;
            }
            else
                strMsg = "更新失敗";
            return result;
        }

        public static bool Delete(string codeType, string id, ref string strMsg)
        {
            string sqlCommand = @"DELETE FROM [dbo].[PostOffice_CodeType]
                                  Where [ID] =@ID And [TYPE] = @TYPE";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sqlCommand;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@ID", id));
            sqlcmd.Parameters.Add(new SqlParameter("@TYPE", codeType));
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

        public static bool CheckRepeat(string codeType, string id, string codeId)
        {
            bool isRepeat = false;
            string sql = @"SELECT ID FROM dbo.PostOffice_CodeType  WHERE [TYPE] = @Type and CODE_ID = @CODE_ID";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            SqlParameter parmQuery = new SqlParameter("@Type", codeType);
            SqlParameter parmQuery2 = new SqlParameter("@CODE_ID", codeId);
            sqlcmd.Parameters.Add(parmQuery);
            sqlcmd.Parameters.Add(parmQuery2);
            try
            {
                DataTable dt = BRPostOffice_Temp.SearchOnDataSet(sqlcmd).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(id))
                    {
                        isRepeat = true;
                    }
                    else
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (!dt.Rows[i]["ID"].ToString().Equals(id))
                            {
                                isRepeat = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log("查詢已傳送郵局資料失敗：" + ex, LogState.Error, LogLayer.BusinessRule);
            }
            return isRepeat;
        }

        public static int DeleteCodeTypeByType(string codeType)
        {
            int sResult = 0;
            string sql = @"DELETE FROM [PostOffice_CodeType] WHERE  [TYPE] = @TYPE ";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            SqlParameter parmQuery = new SqlParameter("@TYPE", codeType);;
            sqlcmd.Parameters.Add(parmQuery);
            try
            {
                DataHelper dh = new DataHelper();
                sResult = dh.ExecuteNonQuery(sqlcmd);
            }
            catch (Exception ex)
            {
                Logging.Log("已刪除 PostOffice_CodeType.[TYPE] = '" + codeType + "' 失敗：" + ex, LogState.Error, LogLayer.BusinessRule);
            }
            return sResult;
        }

        // 整批寫入 PostOffice_CodeType
        public static bool PostOffice_CodeTypeWithBulkCopy(string tableName, DataTable dataTable)
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

                sbc.ColumnMappings.Add("TYPE", "TYPE");                                                // 類別
                sbc.ColumnMappings.Add("CODE_ID", "CODE_ID");                                // 對應KEY
                sbc.ColumnMappings.Add("CODE_NAME", "CODE_NAME");                // 中文名稱
                sbc.ColumnMappings.Add("CODE_EN_NAME", "CODE_EN_NAME"); // 英文名稱
                sbc.ColumnMappings.Add("ORDERBY", "ORDERBY");                            // 排序
                sbc.ColumnMappings.Add("DESCRIPTION", "DESCRIPTION");            // 說明
                sbc.ColumnMappings.Add("IsValid", "IsValid");                                       // 是否啟用

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

        /// <summary>
        ///  取得法律形式及行業別設定，同步至主機
        ///  2020/02/13
        ///  RQ-2019-030155-003
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DataTable SendPostOfficeTypeSetting()
        {
            string sql = @"SELECT (CASE TYPE WHEN '2' THEN 'OG' WHEN '3'  THEN 'CC' ELSE 'OHTER' END) AS 'TYPE',CODE_ID,CODE_NAME 
					 FROM dbo.PostOffice_CodeType WHERE TYPE IN ('2','3') And [IsValid] !=0 
					 ORDER BY TYPE,CODE_ID ";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;

            try
            {
                DataTable dt = BRPostOffice_Temp.SearchOnDataSet(sqlcmd).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                Logging.Log("查詢PostOffice_CodeType設定檔資料失敗：" + ex, LogLayer.BusinessRule);
                return null;
            }
        }

        /// <summary>
        ///  取得國籍設定，同步至OMS系統
        ///  2020/01/04
        ///  RQ-2020-028727-006
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DataTable SendToOMSCountrySetting()
        {
            string sql = @"SELECT '1' AS 'TYPE',CODE_ID,CODE_NAME,CODE_EN_NAME  
					 FROM dbo.PostOffice_CodeType WHERE TYPE = '1'  And [IsValid] !=0 
					 ORDER BY TYPE,CODE_ID ";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;

            try
            {
                DataTable dt = BRPostOffice_Temp.SearchOnDataSet(sqlcmd).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                Logging.Log("查詢PostOffice_CodeType-國籍設定檔資料失敗：" + ex, LogLayer.BusinessRule);
                return null;
            }
        }
    }
}