//******************************************************************
//*  �@    �̡G���f��
//*  �\�໡���GSHOP_UPLOAD��Ʈw�H��

//*  �Ыؤ���G2009/07/13
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
    public class BRSHOP_UPLOAD : CSIPCommonModel.BusinessRules.BRBase<EntitySHOP_UPLOAD>
    {
        /// <summary>
        /// ��Entity�覡���J��Ʈw
        /// </summary>
        /// <param name="eShopUpload">Entity</param>
        /// <returns>true���\,false����</returns>
        public static bool AddEntity(EntitySHOP_UPLOAD eShopUpload)
        {
            try
            {
                return eShopUpload.DB_InsertEntity();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
