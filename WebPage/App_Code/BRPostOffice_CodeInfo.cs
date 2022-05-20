//******************************************************************
//*  作    者：陳香琦
//*  功能說明：參數設定menu
//*  創建日期：2019/12/13
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//* 
//*******************************************************************
using System;
using System.Data;
using System.Data.SqlClient;
using CSIPCommonModel.BusinessRules;
using CSIPCommonModel.EntityLayer_new;
using Framework.Common.Logging;

namespace CSIPKeyInGUI.BusinessRules_new
{
    public class BRPostOffice_CodeInfo : BRBase<EntityPostOffice_CodeInfo>
    {
        /// <summary>
        /// 查詢可顯示在畫面上做編輯項目
        /// </summary>
        /// <returns></returns>
        public static DataTable GetCodeInfoType()
        {
            string sql = @"SELECT [TYPE],[TYPE_NAME] FROM [dbo].[PostOffice_CodeInfo] WHERE [IsValid]='1' AND [Editable]='Y' ORDER BY CONVERT(INT,[TYPE]) ";
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
                Logging.Log("查詢生效中且可編輯的AML參數設定：" + ex, LogState.Error, LogLayer.BusinessRule);
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
            string sql = @"SELECT CODE_ID, CODE_NAME, CODE_EN_NAME ,IsValid FROM dbo.PostOffice_CodeType WHERE [TYPE] = @Type and CODE_ID = @CODE_ID";
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
            //if (AddNewEntity(ePostOfficeCodeType))
            //{
            //    strMsg = "新增成功";
            //    result = true;
            //}
            //else
            //    strMsg = "新增失敗";
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
    }
}
