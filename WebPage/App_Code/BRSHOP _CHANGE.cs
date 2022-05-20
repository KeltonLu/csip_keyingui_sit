//******************************************************************
//*  �\�໡���GSHOP_CHANGE ��Ʈw�~����
//*  �Ыؤ���G2019/09/12
//*  �ק�O���G

//*<author>            <time>            <TaskID>                <desc>
//*Ares Luke          2021/02/22         20200031-CSIP EOS       �վ�connectionStr�覡
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
    /// SHOP_CHANGE��Ʈw�~����
    /// </summary>
    public class BRSHOP_CHANGE : CSIPCommonModel.BusinessRules.BRBase<EntitySHOP_CHANGE>
    {
        #region SQL

        public const string SEL_SHOP_INFO = @"SELECT {0} FROM SHOP_CHANGE with(nolock) WHERE ";
        

        #endregion

        /// <summary>
        /// �d�߸�Ʈw�H��
        /// </summary>
        /// <param name="strUniNo">�Τ@�s��</param>
        /// <param name="strKeyInFlag">Keyin���O(1-�@KEY/2-�GKEY)</param>
        /// <param name="strColumns">�n�d�߱o�쪺���</param>
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
        /// �d�߸�Ʈw�@�B�GKEY�H��
        /// </summary>
        /// <param name="strUniNo">�Τ@�s��</param>
        /// <param name="strKeyInFlag">Keyin���O(1-�@KEY/2-�GKEY)</param>
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
        /// �d�߸�Ʈw�ө������H��
        /// </summary>
        /// <param name="strUniNo">�Τ@�s��</param>
        /// <param name="strKeyInFlag">Keyin���O(1-�@KEY/2-�GKEY)</param>
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
        /// �K�[SHOP_CHANGE����
        /// </summary>
        /// <param name="eShopChange">SHOP_CHANGE����</param>
        /// <returns>true���\�Afalse����</returns>
        public static bool Add(EntitySHOP_CHANGE eShopChange)
        {
            return BRSHOP_CHANGE.AddNewEntity(eShopChange);        
        }

        /// <summary>
        /// �ק�SHOP_CHANGE����
        /// </summary>
        /// <param name="strUniNo">�Τ@�s��</param>
        /// <param name="strKeyinFlag">keyin flag</param>
        /// <param name="">��s���e</param>
        /// <returns>true���\�Afalse����</returns>
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
        /// �ھڲΤ@�s���BKeyinFlag�R��SHOP_CHANGE����
        /// </summary>
        /// <param name="eShopChange">SHOP����</param>
        /// <param name="strUniNo">�Τ@�s��</param>
        /// <param name="strKeyinFlag">KEYIN FLAG</param>
        /// <returns>true���\�Afalse����</returns>
        public static bool Delete(EntitySHOP_CHANGE eShopChange, string strUniNo, string strKeyinFlag)
        {
            SqlHelper Sql = new SqlHelper();

            Sql.AddCondition(EntitySHOP_CHANGE.M_UNI_NO1, Operator.Equal, DataTypeUtils.String, strUniNo);
            Sql.AddCondition(EntitySHOP_CHANGE.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyinFlag);

            return BRSHOP_CHANGE.DeleteEntityByCondition(eShopChange, Sql.GetFilterCondition());       
        }

        /// <summary>
        /// ���R��SHOP���ƦZ�A�s�W
        /// </summary>
        /// <param name="eShopChange">SHOP����</param>
        /// <param name="strUNI_NO">�ө��N��</param>
        /// <param name="strKeyInFlag">Keyin���O(1-�@KEY/2-�GKEY)</param>
        /// <returns>true���\�Afalse����</returns>
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
        /// �T�{��Ʋ���by�νs�Ӳνs�O�_��y�{��
        /// </summary>
        /// <param name="strUniNo">�Τ@�s��</param>
        /// <param name="strKeyInFlag">Keyin���O(1-�@KEY/2-�GKEY)</param>
        /// <param name="strColumns">�n�d�߱o�쪺���</param>
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

        // �g�J MAINFRAME_IMP_LOG
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

                sbc.ColumnMappings.Add("FILENAME", "FILENAME");//�ɮצW��
                sbc.ColumnMappings.Add("CORP_NO", "CORP_NO"); // �Τ@�s��1
                sbc.ColumnMappings.Add("CORP_SEQ", "CORP_SEQ"); // �Τ@�s��2
                sbc.ColumnMappings.Add("MERCH_NO", "MERCH_NO");// ;�ө��N��
                sbc.ColumnMappings.Add("STATUS_CODE", "STATUS_CODE");// STATUS_CODE
                sbc.ColumnMappings.Add("TERMINATE_DATE", "TERMINATE_DATE");// �Ѭ����
                sbc.ColumnMappings.Add("TERMINATE_CODE", "TERMINATE_CODE"); // �Ѭ�CODE
                sbc.ColumnMappings.Add("UPDATE_CNT", "UPDATE_CNT");// ��s����
                sbc.ColumnMappings.Add("BATCH_DATE", "BATCH_DATE");// �@�~��(�妸�B�z��)

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

        // �P�_�ɮ׬O�_�w�s�b
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
        /// �T�{���P���D�@�B�Gkey�P��
        /// </summary>
        /// <param name="strUniNo">�Τ@�s��</param>
        /// <param name="streAgent_id">�n�J�P��</param>
        /// <param name="">��s���e</param>
        /// <returns>true���\�Afalse����</returns>
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