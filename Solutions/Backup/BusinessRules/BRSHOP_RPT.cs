//******************************************************************
//*  �@    �̡G���f��
//*  �\�໡���GSHOP_RPT��Ʈw�~����

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
    /// SHOP_RPT��Ʈw�~����
    /// </summary>
    public class BRSHOP_RPT : CSIPCommonModel.BusinessRules.BRBase<EntitySHOP_RPT>
    {
        /// <summary>
        /// �K�[SHOP_RPT����
        /// </summary>
        /// <param name="eShopRpt">SHOP_RPT����</param>
        /// <returns>true���\�Afalse����</returns>
        public static bool Add(EntitySHOP_RPT eShopRpt)
        {
            return BRSHOP_RPT.AddNewEntity(eShopRpt);             
        }

        /// <summary>
        /// �d��SHOP_RPT����
        /// </summary>
        /// <param name="strShopId">�ө��N��</param>
        /// <param name="strKeyInFlag">�@�GKEY</param>
        /// <param name="strType">����</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntitySHOP_RPT> SelectEntitySet(string strShopId, string strKeyInFlag, string strType)
        {          
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntitySHOP_RPT.M_shop_id, Operator.Equal, DataTypeUtils.String, strShopId);
                sSql.AddCondition(EntitySHOP_RPT.M_type, Operator.Equal, DataTypeUtils.String, strType);

                return BRSHOP_RPT.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// ��Entity�覡���J��Ʈw
        /// </summary>
        /// <param name="eShopRpt">Entity</param>
        /// <returns>true���\,false����</returns>
        public static bool AddEntity(EntitySHOP_RPT eShopRpt)
        {
            try
            {
                return eShopRpt.DB_InsertEntity();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
