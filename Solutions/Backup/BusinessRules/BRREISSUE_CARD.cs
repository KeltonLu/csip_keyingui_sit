//******************************************************************
//*  �@    �̡G���f��
//*  �\�໡���GREISSUE_CARD�~����

//*  �Ыؤ���G2009/08/17
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
    /// REISSUE_CARD�~����
    /// </summary>
    public class BRREISSUE_CARD:CSIPCommonModel.BusinessRules.BRBase<EntityREISSUE_CARD>
    {
        /// <summary>
        /// �d�߸�Ʈw�@�B�GKEY�H��
        /// </summary>
        /// <param name="strCardNo">�d��</param>
        /// <param name="strKeyInFlag">Keyin���O(1-�@KEY/2-�GKEY)</param>
        /// <param name="strUploadFlag">Keyin�Х�</param>
        /// <param name="strReIssueType">�ɵo����</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntityREISSUE_CARD> SelectEntitySet(string strCardNo, string strKeyInFlag, string strUploadFlag, string strReIssueType)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityREISSUE_CARD.M_Card_No, Operator.Equal, DataTypeUtils.String, strCardNo);
                sSql.AddCondition(EntityREISSUE_CARD.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
                sSql.AddCondition(EntityREISSUE_CARD.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityREISSUE_CARD.M_ReIssue_Type, Operator.Equal, DataTypeUtils.String, strReIssueType);

                return BRREISSUE_CARD.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// ��sREISSUE_CARD��@KEY�ΤGKEY���
        /// </summary>
        /// <param name="eReissueCard">REISSUE_CARD����</param>
        /// <param name="strCardNo">�d��</param>
        /// <param name="strKeyInFlag">Keyin���O(1-�@KEY/2-�GKEY)</param>
        /// <param name="strUploadFlag">Keyin�Х�</param>
        /// <param name="strReIssueType">�ɵo����</param>
        /// <param name="strField">�n��s��쪺���X</param>
        /// <returns>true���\�Afalse����</returns>
        public static bool Update(EntityREISSUE_CARD eReissueCard, string strCardNo, string strKeyInFlag, string strUploadFlag, string strReIssueType, string[] strField)
        {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityREISSUE_CARD.M_Card_No, Operator.Equal, DataTypeUtils.String, strCardNo);
                sSql.AddCondition(EntityREISSUE_CARD.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
                sSql.AddCondition(EntityREISSUE_CARD.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityREISSUE_CARD.M_ReIssue_Type, Operator.Equal, DataTypeUtils.String, strReIssueType);

                return BRREISSUE_CARD.UpdateEntityByCondition(eReissueCard, sSql.GetFilterCondition(), strField);       
        }

        /// <summary>
        /// ��sREISSUE_CARD��@KEY�M�GKEY���
        /// </summary>
        /// <param name="eReissueCard">REISSUE_CARD����</param>
        /// <param name="strCardNo">�d��</param>
        /// <param name="strUploadFlag">Keyin�Х�</param>
        /// <param name="strReIssueType">�ɵo����</param>
        /// <param name="strField">�n��s��쪺���X</param>
        /// <returns>rue���\�Afalse����</returns>
        public static bool Update(EntityREISSUE_CARD eReissueCard, string strCardNo,  string strUploadFlag, string strReIssueType, string[] strField)
        {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityREISSUE_CARD.M_Card_No, Operator.Equal, DataTypeUtils.String, strCardNo);
                sSql.AddCondition(EntityREISSUE_CARD.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityREISSUE_CARD.M_ReIssue_Type, Operator.Equal, DataTypeUtils.String, strReIssueType);

                return BRREISSUE_CARD.UpdateEntityByCondition(eReissueCard, sSql.GetFilterCondition(), strField);
        }

        /// <summary>
        /// ��Entity�覡���J��Ʈw
        /// </summary>
        /// <param name="eReissueCard">Entity</param>
        /// <returns>true���\,false����</returns>
        public static bool AddEntity(EntityREISSUE_CARD eReissueCard)
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
