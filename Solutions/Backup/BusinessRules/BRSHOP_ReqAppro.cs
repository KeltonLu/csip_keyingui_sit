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
    public class BRSHOP_ReqAppro : CSIPCommonModel.BusinessRules.BRBase<EntitySHOP_ReqAppro>
    {
        /// <summary>
        /// ��Entity�覡���J��Ʈw
        /// </summary>
        /// <param name="eShopUpload">Entity</param>
        /// <returns>true���\,false����</returns>
        public static bool AddEntity(EntitySHOP_ReqAppro eShopReqAppro)
        {
            try
            {
                return eShopReqAppro.DB_InsertEntity();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
