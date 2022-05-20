//******************************************************************
//*  �@    �̡G���f��
//*  �\�໡���GSHOP_1KEYLOG��Ʈw�~����

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
    /// SHOP_1KEYLOG��Ʈw�~����
    /// </summary>
    public class BRSHOP_1KEYLOG : CSIPCommonModel.BusinessRules.BRBase<EntitySHOP_1KEYLOG>
    {
        /// <summary>
        /// �K�[SHOP_1KEYLOG����
        /// </summary>
        /// <param name="eShop1KeyLog">SHOP_1KEYLOG����</param>
        /// <returns>true���\�Afalse����</returns>
        public static bool Add(EntitySHOP_1KEYLOG eShop1KeyLog)
        {
            return BRSHOP_1KEYLOG.AddNewEntity(eShop1KeyLog);                     
        }

        /// <summary>
        /// ��Entity�覡���J��Ʈw
        /// </summary>
        /// <param name="eShop1KeyLog">Entity</param>
        /// <returns>true���\,false����</returns>
        public static bool AddEntity(EntitySHOP_1KEYLOG eShop1KeyLog)
        {
            try
            {
                return eShop1KeyLog.DB_InsertEntity();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
