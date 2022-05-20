//******************************************************************
//*  �@    �̡G���f��
//*  �\�໡���GSHOP��Ʈw�~����

//*  �Ыؤ���G2009/07/12
//*  �ק�O���G

//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Framework.Data.OM;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;

namespace CSIPKeyInGUI.BusinessRules
{
    /// <summary>
    /// SHOP��Ʈw�~����
    /// </summary>
    public class BRSHOP : CSIPCommonModel.BusinessRules.BRBase<EntitySHOP>
    {
        #region SQL

        public const string SEL_SHOP_INFO = @"SELECT {0} FROM SHOP WHERE ";

        #endregion

        /// <summary>
        /// �d�߸�Ʈw�H��
        /// </summary>
        /// <param name="strShopId">�ө��N��</param>
        /// <param name="strKeyInFlag">Keyin���O(1-�@KEY/2-�GKEY)</param>
        /// <param name="strShopType">�\��e���s��</param>
        /// <param name="strColumns">�n�d�߱o�쪺���</param>
        /// <returns>DataSet</returns>
        public static DataSet Select(string strShopId, string strKeyInFlag, string strShopType, string strColumns)
        {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntitySHOP.M_shop_id, Operator.Equal, DataTypeUtils.String, strShopId);
                sSql.AddCondition(EntitySHOP.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
                sSql.AddCondition(EntitySHOP.M_shop_type, Operator.Equal, DataTypeUtils.String, strShopType);

                string strSqlCmd = string.Format(SEL_SHOP_INFO, strColumns) + sSql.GetFilterCondition().Substring(4, sSql.GetFilterCondition().Length - 4);

                return BRSHOP.SearchOnDataSet(strSqlCmd);   
        }

        /// <summary>
        /// �d�߸�Ʈw�@�B�GKEY�H��
        /// </summary>
        /// <param name="strShopId">�ө��N��</param>
        /// <param name="strKeyInFlag">Keyin���O(1-�@KEY/2-�GKEY)</param>
        /// <param name="strShopType">�\��e���s��</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntitySHOP> SelectEntitySet(string strShopId, string strKeyInFlag, string strShopType)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntitySHOP.M_shop_id, Operator.Equal, DataTypeUtils.String, strShopId);
                sSql.AddCondition(EntitySHOP.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
                sSql.AddCondition(EntitySHOP.M_shop_type, Operator.Equal, DataTypeUtils.String, strShopType);

                return BRSHOP.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// �d�߸�Ʈw�ө������H��
        /// </summary>
        /// <param name="strShopId">�ө��N��</param>
        /// <param name="strShopType">�\��e���s��</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntitySHOP> SelectShopTypeEntitySet(string strShopId, string strShopType)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntitySHOP.M_shop_id, Operator.Equal, DataTypeUtils.String, strShopId);
                sSql.AddCondition(EntitySHOP.M_shop_type, Operator.Equal, DataTypeUtils.String, strShopType);

                return BRSHOP.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// �K�[SHOP����
        /// </summary>
        /// <param name="eShop">SHOP����</param>
        /// <returns>true���\�Afalse����</returns>
        public static bool Add(EntitySHOP eShop)
        {
            return BRSHOP.AddNewEntity(eShop);        
        }

        /// <summary>
        /// �ק�SHOP����
        /// </summary>
        /// <param name="eShop">SHOP����</param>
        /// <param name="strCondition">����</param>
        /// <returns>true���\�Afalse����</returns>
        public static bool Update(EntitySHOP eShop, string strCondition)
        {
            return BRSHOP.UpdateEntity(eShop, strCondition);         
        }

        /// <summary>
        /// �ھڰө��N���B�@�GKEY���ѡB�\��e���s���R��SHOP����
        /// </summary>
        /// <param name="eShop">SHOP����</param>
        /// <param name="strShopId">�ө��N��</param>
        /// <param name="strKeyInFlag">Keyin���O(1-�@KEY/2-�GKEY)</param>
        /// <param name="strShopType">�\��e���s��</param>
        /// <returns>true���\�Afalse����</returns>
        public static bool Delete(EntitySHOP eShop, string strShopId, string strKeyInFlag, string strShopType)
        {
            SqlHelper Sql = new SqlHelper();

            Sql.AddCondition(EntitySHOP.M_shop_id, Operator.Equal, DataTypeUtils.String, strShopId);
            Sql.AddCondition(EntitySHOP.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
            Sql.AddCondition(EntitySHOP.M_shop_type, Operator.Equal, DataTypeUtils.String, strShopType);

            try
            {
                eShop.DB_DeleteEntity(Sql.GetFilterCondition());
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// �ھڰө��N���B�\��e���s���R��SHOP����
        /// </summary>
        /// <param name="eShop">SHOP����</param>
        /// <param name="strShopId">�ө��N��</param>
        /// <param name="strShopType">�\��e���s��</param>
        /// <returns>true���\�Afalse����</returns>
        public static bool Delete(EntitySHOP eShop, string strShopId, string strShopType)
        {
            SqlHelper Sql = new SqlHelper();

            Sql.AddCondition(EntitySHOP.M_shop_id, Operator.Equal, DataTypeUtils.String, strShopId);
            Sql.AddCondition(EntitySHOP.M_shop_type, Operator.Equal, DataTypeUtils.String, strShopType);

            return BRSHOP.DeleteEntityByCondition(eShop, Sql.GetFilterCondition());       
        }

        /// <summary>
        /// ���R��SHOP���ƦZ�A�s�W
        /// </summary>
        /// <param name="eShop">SHOP����</param>
        /// <param name="strShopId">�ө��N��</param>
        /// <param name="strKeyInFlag">Keyin���O(1-�@KEY/2-�GKEY)</param>
        /// <param name="strShopType">�\��e���s��</param>
        /// <returns>true���\�Afalse����</returns>
        public static bool Insert(EntitySHOP eShop, string strShopId, string strKeyInFlag, string strShopType)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (!Delete(eShop, strShopId, strKeyInFlag, strShopType))
                    {
                        return false;
                    }

                    if (!Add(eShop))
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
    }
}
