//******************************************************************
//*  �@    �̡G���f��
//*  �\�໡���GTRANS_NUM��Ʈw�~����

//*  �Ыؤ���G2009/07/14
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
using System.Data.SqlClient;

namespace CSIPKeyInGUI.BusinessRules
{
    /// <summary>
    /// TRANS_NUM��Ʈw�~����
    /// </summary>
    public class BRTRANS_NUM : CSIPCommonModel.BusinessRules.BRBase<EntityTRANS_NUM>
    {
        public const string UPD_TRANS_NUM = @"UPDATE TRANS_NUM SET TRANS_NUM = TRANS_NUM+1 WHERE ";//TRANS_CODE='{0}' AND TRANS_DATE='{1}'

        /// <summary>
        /// ��sTransNum��Ʈw�H��
        /// </summary>
        /// <param name="strTransCode">�N��</param>
        /// <returns>true���\�Afalse����</returns>
        public static bool UpdateTransNum(string strTransCode)
        {
            try
            {
                SqlHelper Sql = new SqlHelper();
                Sql.AddCondition(EntityTRANS_NUM.M_trans_code, Operator.Equal, DataTypeUtils.String, strTransCode);
                Sql.AddCondition(EntityTRANS_NUM.M_trans_date, Operator.Equal, DataTypeUtils.String, DateTime.Now.ToString("yyyyMMdd"));
                string strSqlCmd = UPD_TRANS_NUM + Sql.GetFilterCondition().Substring(4, Sql.GetFilterCondition().Length - 4);

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandText = strSqlCmd;
                sqlcmd.CommandType = CommandType.Text;

                return BRTRANS_NUM.Update(sqlcmd);
            }
            catch
            {
                return false;
            }
        }
    }
}
