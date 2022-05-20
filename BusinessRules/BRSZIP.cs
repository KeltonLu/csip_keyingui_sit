//******************************************************************
//*  �@    �̡G���f��
//*  �\�໡���GSZIP��Ʈw�~����

//*  �Ыؤ���G2009/07/14
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
    /// SZIP��Ʈw�~����
    /// </summary>
    public class BRSZIP : CSIPCommonModel.BusinessRules.BRBase<EntitySZIP>
    {
        /// <summary>
        /// �o��szip��Ʈw�l���ϸ��N�X
        /// </summary>
        /// <param name="strZipData">�a�}</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntitySZIP> SelectEntitySet(string strZipData)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntitySZIP.M_zip_data, Operator.Equal, DataTypeUtils.String, strZipData);

                return BRSZIP.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
