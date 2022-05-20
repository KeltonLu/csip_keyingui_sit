//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：REISSUE_CARD業務類

//*  創建日期：2009/08/17
//*  修改記錄：

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
    /// REISSUE_CARD業務類
    /// </summary>
    public class BRREISSUE_CARD:CSIPCommonModel.BusinessRules.BRBase<EntityREISSUE_CARD>
    {
        /// <summary>
        /// 查詢資料庫一、二KEY信息
        /// </summary>
        /// <param name="strCardNo">卡號</param>
        /// <param name="strKeyInFlag">Keyin類別(1-一KEY/2-二KEY)</param>
        /// <param name="strUploadFlag">Keyin標示</param>
        /// <param name="strReIssueType">補發種類</param>
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
        /// 更新REISSUE_CARD表一KEY或二KEY資料
        /// </summary>
        /// <param name="eReissueCard">REISSUE_CARD實體</param>
        /// <param name="strCardNo">卡號</param>
        /// <param name="strKeyInFlag">Keyin類別(1-一KEY/2-二KEY)</param>
        /// <param name="strUploadFlag">Keyin標示</param>
        /// <param name="strReIssueType">補發種類</param>
        /// <param name="strField">要更新欄位的集合</param>
        /// <returns>true成功，false失敗</returns>
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
        /// 更新REISSUE_CARD表一KEY和二KEY資料
        /// </summary>
        /// <param name="eReissueCard">REISSUE_CARD實體</param>
        /// <param name="strCardNo">卡號</param>
        /// <param name="strUploadFlag">Keyin標示</param>
        /// <param name="strReIssueType">補發種類</param>
        /// <param name="strField">要更新欄位的集合</param>
        /// <returns>rue成功，false失敗</returns>
        public static bool Update(EntityREISSUE_CARD eReissueCard, string strCardNo,  string strUploadFlag, string strReIssueType, string[] strField)
        {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityREISSUE_CARD.M_Card_No, Operator.Equal, DataTypeUtils.String, strCardNo);
                sSql.AddCondition(EntityREISSUE_CARD.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityREISSUE_CARD.M_ReIssue_Type, Operator.Equal, DataTypeUtils.String, strReIssueType);

                return BRREISSUE_CARD.UpdateEntityByCondition(eReissueCard, sSql.GetFilterCondition(), strField);
        }

        /// <summary>
        /// 用Entity方式插入資料庫
        /// </summary>
        /// <param name="eReissueCard">Entity</param>
        /// <returns>true成功,false失敗</returns>
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
