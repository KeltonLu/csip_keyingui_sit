//******************************************************************
//*  �@    �̡G���f��
//*  �\�໡���GCHANGE_BLK��Ʈw�~����

//*  �Ыؤ���G2009/09/14
//*  �ק�O���G

//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using System.Data;

namespace CSIPKeyInGUI.BusinessRules
{
    /// <summary>
    /// CHANGE_BLK��Ʈw�~����
    /// </summary>
    public class BRCHANGE_BLK : CSIPCommonModel.BusinessRules.BRBase<EntityCHANGE_BLK>
    {
        #region SQL
        public const string SEL_CHANGE_BLK_INFO = @"SELECT {0} FROM CHANGE_BLK WHERE ";
        #endregion

        /// <summary>
        /// �d�߸�Ʈw�@�B�GKEY�H��
        /// </summary>
        /// <param name="strCardNo">�d��</param>
        /// <param name="strKeyInFlag">Keyin���O(1-�@KEY/2-�GKEY)</param>
        /// <param name="strUploadFlag">Keyin�Х�</param>
        /// <param name="strChangeType">��������</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntityCHANGE_BLK> SelectEntitySet(string strCardNo, string strKeyInFlag, string strUploadFlag, string strChangeType)
        {           
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityCHANGE_BLK.M_Card_No, Operator.Equal, DataTypeUtils.String, strCardNo);
                sSql.AddCondition(EntityCHANGE_BLK.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
                sSql.AddCondition(EntityCHANGE_BLK.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityCHANGE_BLK.M_Change_Type, Operator.Equal, DataTypeUtils.String, strChangeType);

                return BRCHANGE_BLK.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// �d�߸�Ʈw�@�B�GKEY���w���H��
        /// </summary>
        /// <param name="strCardNo">�d��</param>
        /// <param name="strKeyInFlag">Keyin���O(1-�@KEY/2-�GKEY)</param>
        /// <param name="strUploadFlag">Keyin�Х�</param>
        /// <param name="strChangeType">��������</param>
        /// <param name="strColumns">�n�d�߱o�쪺���</param>
        /// <returns>DataSet</returns>
        public static DataSet Select(string strCardNo, string strKeyInFlag, string strUploadFlag, string strChangeType, string strColumns)
        {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityCHANGE_BLK.M_Card_No, Operator.Equal, DataTypeUtils.String, strCardNo);
                sSql.AddCondition(EntityCHANGE_BLK.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
                sSql.AddCondition(EntityCHANGE_BLK.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityCHANGE_BLK.M_Change_Type, Operator.Equal, DataTypeUtils.String, strChangeType);

                string strSqlCmd = string.Format(SEL_CHANGE_BLK_INFO, strColumns) + sSql.GetFilterCondition().Substring(4, sSql.GetFilterCondition().Length - 4);

                return BRCHANGE_BLK.SearchOnDataSet(strSqlCmd);        
        }

        /// <summary>
        /// ��sCHANGE_BLK��@KEY�ΤGKEY���
        /// </summary>
        /// <param name="eReissueCard">CHANGE_BLK����</param>
        /// <param name="strCardNo">�d��</param>
        /// <param name="strKeyInFlag">Keyin���O(1-�@KEY/2-�GKEY)</param>
        /// <param name="strUploadFlag">Keyin�Х�</param>
        /// <param name="strChangeType">��������</param>
        /// <param name="strField">�n��s��쪺���X</param>
        /// <returns>true���\�Afalse����</returns>
        public static bool Update(EntityCHANGE_BLK eChangeBlk, string strCardNo, string strKeyInFlag, string strUploadFlag, string strChangeType, string[] strField)
        {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityCHANGE_BLK.M_Card_No, Operator.Equal, DataTypeUtils.String, strCardNo);
                sSql.AddCondition(EntityCHANGE_BLK.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
                sSql.AddCondition(EntityCHANGE_BLK.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityCHANGE_BLK.M_Change_Type, Operator.Equal, DataTypeUtils.String, strChangeType);

                return BRCHANGE_BLK.UpdateEntityByCondition(eChangeBlk, sSql.GetFilterCondition(), strField);
        }

        /// <summary>
        /// ��sCHANGE_BLK��@KEY�M�GKEY���
        /// </summary>
        /// <param name="eReissueCard">CHANGE_BLK����</param>
        /// <param name="strCardNo">�d��</param>
        /// <param name="strUploadFlag">Keyin�Х�</param>
        /// <param name="strChangeType">��������</param>
        /// <param name="strField">�n��s��쪺���X</param>
        /// <returns>rue���\�Afalse����</returns>
        public static bool Update(EntityCHANGE_BLK eChangeBlk, string strCardNo, string strUploadFlag, string strChangeType, string[] strField)
        {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityCHANGE_BLK.M_Card_No, Operator.Equal, DataTypeUtils.String, strCardNo);
                sSql.AddCondition(EntityCHANGE_BLK.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityCHANGE_BLK.M_Change_Type, Operator.Equal, DataTypeUtils.String, strChangeType);

                return BRCHANGE_BLK.UpdateEntityByCondition(eChangeBlk, sSql.GetFilterCondition(), strField);    
        }

        /// <summary>
        /// ���R��CHANGE_BLK���ƦZ�A�s�W
        /// </summary>
        /// <param name="eShop">CHANGE_BLK����</param>
        /// <param name="strCardNo">�d��</param>
        /// <param name="strKeyInFlag">Keyin���O(1-�@KEY/2-�GKEY)</param>
        /// <param name="strUploadFlag">Keyin�Х�</param>
        /// <param name="strChangeType">��������</param>
        /// <returns>true���\�Afalse����</returns>
        public static bool Insert(EntityCHANGE_BLK eChangeBlk, string strCardNo, string strKeyInFlag , string strUploadFlag, string strChangeType)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityCHANGE_BLK.M_Card_No, Operator.Equal, DataTypeUtils.String, strCardNo);
                sSql.AddCondition(EntityCHANGE_BLK.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
                sSql.AddCondition(EntityCHANGE_BLK.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityCHANGE_BLK.M_Change_Type, Operator.Equal, DataTypeUtils.String, strChangeType);
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (!DeleteEntityByCondition(eChangeBlk, sSql.GetFilterCondition()))
                    {
                        return false;
                    }

                    if (!eChangeBlk.DB_InsertEntity())
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

        /// <summary>
        /// ��Entity�覡���J��Ʈw
        /// </summary>
        /// <param name="eChangeBlk">Entity</param>
        /// <returns>true���\,false����</returns>
        public static bool AddEntity(EntityCHANGE_BLK eChangeBlk)
        {
            try
            {
                return eChangeBlk.DB_InsertEntity();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
