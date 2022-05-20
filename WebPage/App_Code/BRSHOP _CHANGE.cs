//******************************************************************
//*  功能說明：SHOP_CHANGE 資料庫業務類
//*  創建日期：2019/09/12
//*  修改記錄：

//*<author>            <time>            <TaskID>                <desc>
//*Ares Luke          2021/02/22         20200031-CSIP EOS       調整connectionStr方式
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Framework.Data.OM;
using CSIPKeyInGUI.EntityLayer_new;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using System.Data.SqlClient;
using Framework.Common.Logging;
using Framework.Common.Utility;

namespace CSIPKeyInGUI.BusinessRules_new
{
    /// <summary>
    /// SHOP_CHANGE資料庫業務類
    /// </summary>
    public class BRSHOP_CHANGE : CSIPCommonModel.BusinessRules.BRBase<EntitySHOP_CHANGE>
    {
        #region SQL

        public const string SEL_SHOP_INFO = @"SELECT {0} FROM SHOP_CHANGE with(nolock) WHERE ";
        

        #endregion

        /// <summary>
        /// 查詢資料庫信息
        /// </summary>
        /// <param name="strUniNo">統一編號</param>
        /// <param name="strKeyInFlag">Keyin類別(1-一KEY/2-二KEY)</param>
        /// <param name="strColumns">要查詢得到的欄位</param>
        /// <returns>DataSet</returns>
        public static DataSet Select(string strUniNo, string strKeyInFlag, string strColumns, string strCheckFlag)
        {
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntitySHOP_CHANGE.M_UNI_NO1, Operator.Equal, DataTypeUtils.String, strUniNo);
            sSql.AddCondition(EntitySHOP_CHANGE.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
            sSql.AddCondition(EntitySHOP_CHANGE.M_isCHECK, Operator.Equal, DataTypeUtils.String, strCheckFlag);

            string strSqlCmd = string.Format(SEL_SHOP_INFO, strColumns) + sSql.GetFilterCondition().Substring(4, sSql.GetFilterCondition().Length - 4);

            return BRSHOP_CHANGE.SearchOnDataSet(strSqlCmd);
        }

        /// <summary>
        /// 查詢資料庫一、二KEY信息
        /// </summary>
        /// <param name="strUniNo">統一編號</param>
        /// <param name="strKeyInFlag">Keyin類別(1-一KEY/2-二KEY)</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntitySHOP_CHANGE> SelectEntitySet(string strUniNo, string strKeyInFlag)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntitySHOP_CHANGE.M_UNI_NO1, Operator.Equal, DataTypeUtils.String, strUniNo);
                sSql.AddCondition(EntitySHOP_CHANGE.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);

                return BRSHOP_CHANGE.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 查詢資料庫商店類型信息
        /// </summary>
        /// <param name="strUniNo">統一編號</param>
        /// <param name="strKeyInFlag">Keyin類別(1-一KEY/2-二KEY)</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntitySHOP_CHANGE> SelectShopTypeEntitySet(string strUniNo, string strKeyInFlag)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntitySHOP_CHANGE.M_UNI_NO1, Operator.Equal, DataTypeUtils.String, strUniNo);
                sSql.AddCondition(EntitySHOP_CHANGE.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);

                return BRSHOP_CHANGE.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 添加SHOP_CHANGE表資料
        /// </summary>
        /// <param name="eShopChange">SHOP_CHANGE實體</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Add(EntitySHOP_CHANGE eShopChange)
        {
            return BRSHOP_CHANGE.AddNewEntity(eShopChange);        
        }

        /// <summary>
        /// 修改SHOP_CHANGE表資料
        /// </summary>
        /// <param name="strUniNo">統一編號</param>
        /// <param name="strKeyinFlag">keyin flag</param>
        /// <param name="">更新內容</param>
        /// <returns>true成功，false失敗</returns>
        public static bool UpdateShopChangeFlag(string strUniNo,  string strKeyinFlag, string _value)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
            UPDATE [dbo].[SHOP_CHANGE] SET ISCHECK = @value  WHERE UNI_NO1 = @UniNo1 AND KeyIn_Flag = @KeyinFlag";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@UniNo1", strUniNo));
            sqlcmd.Parameters.Add(new SqlParameter("@KeyinFlag", strKeyinFlag));
            sqlcmd.Parameters.Add(new SqlParameter("@value", _value));

            try
            {
                DataSet resultSet = BRSHOP_CHANGE.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }
            return result;
        }

        /// <summary>
        /// 根據統一編號、KeyinFlag刪除SHOP_CHANGE表資料
        /// </summary>
        /// <param name="eShopChange">SHOP實體</param>
        /// <param name="strUniNo">統一編號</param>
        /// <param name="strKeyinFlag">KEYIN FLAG</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Delete(EntitySHOP_CHANGE eShopChange, string strUniNo, string strKeyinFlag)
        {
            SqlHelper Sql = new SqlHelper();

            Sql.AddCondition(EntitySHOP_CHANGE.M_UNI_NO1, Operator.Equal, DataTypeUtils.String, strUniNo);
            Sql.AddCondition(EntitySHOP_CHANGE.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyinFlag);

            return BRSHOP_CHANGE.DeleteEntityByCondition(eShopChange, Sql.GetFilterCondition());       
        }

        /// <summary>
        /// 先刪除SHOP表資料后再新增
        /// </summary>
        /// <param name="eShopChange">SHOP實體</param>
        /// <param name="strUNI_NO">商店代號</param>
        /// <param name="strKeyInFlag">Keyin類別(1-一KEY/2-二KEY)</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Insert(EntitySHOP_CHANGE eShopChange, string strUNI_NO, string strKeyInFlag)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (!Delete(eShopChange, strUNI_NO, strKeyInFlag))
                    {
                        return false;
                    }

                    if (!Add(eShopChange))
                    {
                        return false;
                    }
                    ts.Complete();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static DataSet GetDataFor304(string strUniNo)
        {
            //2021/03/17_Ares_Stanley-Change DB name to a parameter
            string SEL_SHOPCHANGE_INFO = string.Format(@"SELECT UNI_NO1,[1] AS 'USER1',[2] AS 'USER2'
                                    FROM 
                                    (
	                                    SELECT UNI_NO1,KEYIN_FLAG,MAX(b.[USER_NAME]) AS MAXUSER
	                                    FROM SHOP_CHANGE a WITH(NOLOCK) JOIN {0}.dbo.M_USER b WITH(NOLOCK) ON b.[USER_ID]=a.MOD_USER
	                                    WHERE ISCHECK='Y'
	                                    GROUP BY UNI_NO1,KEYIN_FLAG
                                    ) AS GDV_TABLE
                                    PIVOT
                                    (
	                                    MAX(MAXUSER)
	                                    FOR KEYIN_FLAG IN ([1],[2])
                                    ) AS PIVOTTABLE
                                        ", UtilHelper.GetAppSettings("DB_CSIP"));

            //2021/03/17_Ares_Stanley-Change DB name to a parameter
            string strSqlCmd = string.Format(SEL_SHOPCHANGE_INFO, strUniNo);

            return BRSHOP_CHANGE.SearchOnDataSet(strSqlCmd);
        }

        /// <summary>
        /// 確認資料異動by統編該統編是否於流程中
        /// </summary>
        /// <param name="strUniNo">統一編號</param>
        /// <param name="strKeyInFlag">Keyin類別(1-一KEY/2-二KEY)</param>
        /// <param name="strColumns">要查詢得到的欄位</param>
        /// <returns>DataSet</returns>
        public static bool checkUniNoInFlow(string strUniNo, string strColumns)
        {
            DataSet dsResult = new DataSet();
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntitySHOP_CHANGE.M_UNI_NO1, Operator.Equal, DataTypeUtils.String, strUniNo);
            sSql.AddCondition(EntitySHOP_CHANGE.M_isCHECK, Operator.Equal, DataTypeUtils.String, "Y");

            string strSqlCmd = string.Format(SEL_SHOP_INFO, strColumns) + sSql.GetFilterCondition().Substring(4, sSql.GetFilterCondition().Length - 4);
            dsResult = BRSHOP_CHANGE.SearchOnDataSet(strSqlCmd);

            if (dsResult != null && dsResult.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        // 寫入 MAINFRAME_IMP_LOG
        public static bool InsertMAINFRAME_IMP_LOGData(string tableName, DataTable dataTable)
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

                sbc.ColumnMappings.Add("FILENAME", "FILENAME");//檔案名稱
                sbc.ColumnMappings.Add("CORP_NO", "CORP_NO"); // 統一編號1
                sbc.ColumnMappings.Add("CORP_SEQ", "CORP_SEQ"); // 統一編號2
                sbc.ColumnMappings.Add("MERCH_NO", "MERCH_NO");// ;商店代號
                sbc.ColumnMappings.Add("STATUS_CODE", "STATUS_CODE");// STATUS_CODE
                sbc.ColumnMappings.Add("TERMINATE_DATE", "TERMINATE_DATE");// 解約日期
                sbc.ColumnMappings.Add("TERMINATE_CODE", "TERMINATE_CODE"); // 解約CODE
                sbc.ColumnMappings.Add("UPDATE_CNT", "UPDATE_CNT");// 更新筆數
                sbc.ColumnMappings.Add("BATCH_DATE", "BATCH_DATE");// 作業日(批次處理日)

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

        // 判斷檔案是否已存在
        public static bool IsFileExist(string fileName)
        {
            bool isFileExist = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
                                                                SELECT top 1 FILENAME FROM [dbo].[MAINFRAME_IMP_LOG] WITH(NOLOCK)
                                                                WHERE FileName = @FileName";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@FileName", fileName));

            try
            {
                DataSet resultSet = BRSHOP_CHANGE.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables[0].Rows.Count > 0)
                {
                    isFileExist = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return isFileExist;
        }
        
        /// <summary>
        /// 確認放行同仁非一、二key同仁
        /// </summary>
        /// <param name="strUniNo">統一編號</param>
        /// <param name="streAgent_id">登入同仁</param>
        /// <param name="">更新內容</param>
        /// <returns>true成功，false失敗</returns>
        public static bool CheckKeyinFlagNotDup(string strUniNo, string streAgent_id)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
            SELECT ISNULL(MOD_USER,'') AS 'MOD_USER' FROM SHOP_CHANGE with(nolock) WHERE UNI_NO1 = @UniNo1 AND MOD_USER = @AgentId";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@UniNo1", strUniNo));
            sqlcmd.Parameters.Add(new SqlParameter("@AgentId", streAgent_id));

            try
            {
                DataSet resultSet = BRSHOP_CHANGE.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables[0].Rows.Count > 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }
            return result;
        }

    }
}