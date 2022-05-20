//******************************************************************
//*  �@    �̡G���f��
//*  �\�໡���GiPASS

//*  �Ыؤ���G2014/11/06
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

namespace CSIPKeyInGUI.BusinessRules
{
    /// <summary>
    /// iPASS�~����
    /// </summary>
    public class BRiPASS : CSIPCommonModel.BusinessRules.BRBase<EntityiPASS>
    {
        /// <summary>
        /// �d�߸�Ʈw�@�B�GKEY�H��
        /// </summary>
        /// <param name="strCardNo">�d��</param>
        /// <param name="strKeyInFlag">Keyin���O(1-�@KEY/2-�GKEY)</param>
        /// <param name="strUploadFlag">Keyin�Х�</param>
        /// <param name="strReIssueType">�ɵo����</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntityiPASS> SelectEntitySet(string strCardNo, string strKeyInFlag, string strUploadFlag, string strReIssueType)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityiPASS.M_Card_No, Operator.Equal, DataTypeUtils.String, strCardNo);
                sSql.AddCondition(EntityiPASS.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
                sSql.AddCondition(EntityiPASS.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityiPASS.M_ReIssue_Type, Operator.Equal, DataTypeUtils.String, strReIssueType);

                return BRiPASS.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// ��siPASS��@KEY�ΤGKEY���
        /// </summary>
        /// <param name="eReissueCard">iPASS����</param>
        /// <param name="strCardNo">�d��</param>
        /// <param name="strKeyInFlag">Keyin���O(1-�@KEY/2-�GKEY)</param>
        /// <param name="strUploadFlag">Keyin�Х�</param>
        /// <param name="strReIssueType">�ɵo����</param>
        /// <param name="strField">�n��s��쪺���X</param>
        /// <returns>true���\�Afalse����</returns>
        public static bool Update(EntityiPASS eReissueCard, string strCardNo, string strKeyInFlag, 
            string strUploadFlag, string strReIssueType, string[] strField)
        {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityiPASS.M_Card_No, Operator.Equal, DataTypeUtils.String, strCardNo);
                sSql.AddCondition(EntityiPASS.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
                sSql.AddCondition(EntityiPASS.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityiPASS.M_ReIssue_Type, Operator.Equal, DataTypeUtils.String, strReIssueType);

                return BRiPASS.UpdateEntityByCondition(eReissueCard, sSql.GetFilterCondition(), strField);       
        }

        /// <summary>
        /// ��siPASS��@KEY�M�GKEY���
        /// </summary>
        /// <param name="eReissueCard">iPASS����</param>
        /// <param name="strCardNo">�d��</param>
        /// <param name="strUploadFlag">Keyin�Х�</param>
        /// <param name="strReIssueType">�ɵo����</param>
        /// <param name="strField">�n��s��쪺���X</param>
        /// <returns>rue���\�Afalse����</returns>
        public static bool Update(EntityiPASS eReissueCard, string strCardNo,  string strUploadFlag, string strReIssueType, string[] strField)
        {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityiPASS.M_Card_No, Operator.Equal, DataTypeUtils.String, strCardNo);
                sSql.AddCondition(EntityiPASS.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityiPASS.M_ReIssue_Type, Operator.Equal, DataTypeUtils.String, strReIssueType);

                return BRiPASS.UpdateEntityByCondition(eReissueCard, sSql.GetFilterCondition(), strField);
        }

        /// <summary>
        /// ��Entity�覡���J��Ʈw
        /// </summary>
        /// <param name="eReissueCard">Entity</param>
        /// <returns>true���\,false����</returns>
        public static bool AddEntity(EntityiPASS eReissueCard)
        {
            try
            {
                return eReissueCard.DB_InsertEntity();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
