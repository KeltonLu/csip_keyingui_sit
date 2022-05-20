//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：CHANGE_BLK資料庫業務類

//*  創建日期：2009/09/14
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
using System.Data;

namespace CSIPKeyInGUI.BusinessRules
{
    /// <summary>
    /// CHANGE_BLK資料庫業務類
    /// </summary>
    public class BRCHANGE_BLK : CSIPCommonModel.BusinessRules.BRBase<EntityCHANGE_BLK>
    {
        #region SQL
        public const string SEL_CHANGE_BLK_INFO = @"SELECT {0} FROM CHANGE_BLK WHERE ";
        #endregion

        /// <summary>
        /// 查詢資料庫一、二KEY信息
        /// </summary>
        /// <param name="strCardNo">卡號</param>
        /// <param name="strKeyInFlag">Keyin類別(1-一KEY/2-二KEY)</param>
        /// <param name="strUploadFlag">Keyin標示</param>
        /// <param name="strChangeType">異動類型</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntityCHANGE_BLK> SelectEntitySet(string strCardNo, string strKeyInFlag, string strUploadFlag, string strChangeType)
        {           
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityCHANGE_BLK.M_Card_No, Operator.Equal, DataTypeUtils.String, strCardNo);
                sSql.AddCondition(EntityCHANGE_BLK.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
                sSql.AddCondition(EntityCHANGE_BLK.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityCHANGE_BLK.M_Change_Type, Operator.Equal, DataTypeUtils.String, strChangeType);

                return BRCHANGE_BLK.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 查詢資料庫一、二KEY指定欄位信息
        /// </summary>
        /// <param name="strCardNo">卡號</param>
        /// <param name="strKeyInFlag">Keyin類別(1-一KEY/2-二KEY)</param>
        /// <param name="strUploadFlag">Keyin標示</param>
        /// <param name="strChangeType">異動類型</param>
        /// <param name="strColumns">要查詢得到的欄位</param>
        /// <returns>DataSet</returns>
        public static DataSet Select(string strCardNo, string strKeyInFlag, string strUploadFlag, string strChangeType, string strColumns)
        {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityCHANGE_BLK.M_Card_No, Operator.Equal, DataTypeUtils.String, strCardNo);
                sSql.AddCondition(EntityCHANGE_BLK.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
                sSql.AddCondition(EntityCHANGE_BLK.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityCHANGE_BLK.M_Change_Type, Operator.Equal, DataTypeUtils.String, strChangeType);

                string strSqlCmd = string.Format(SEL_CHANGE_BLK_INFO, strColumns) + sSql.GetFilterCondition().Substring(4, sSql.GetFilterCondition().Length - 4);

                return BRCHANGE_BLK.SearchOnDataSet(strSqlCmd);        
        }

        /// <summary>
        /// 更新CHANGE_BLK表一KEY或二KEY資料
        /// </summary>
        /// <param name="eReissueCard">CHANGE_BLK實體</param>
        /// <param name="strCardNo">卡號</param>
        /// <param name="strKeyInFlag">Keyin類別(1-一KEY/2-二KEY)</param>
        /// <param name="strUploadFlag">Keyin標示</param>
        /// <param name="strChangeType">異動類型</param>
        /// <param name="strField">要更新欄位的集合</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Update(EntityCHANGE_BLK eChangeBlk, string strCardNo, string strKeyInFlag, string strUploadFlag, string strChangeType, string[] strField)
        {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityCHANGE_BLK.M_Card_No, Operator.Equal, DataTypeUtils.String, strCardNo);
                sSql.AddCondition(EntityCHANGE_BLK.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
                sSql.AddCondition(EntityCHANGE_BLK.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityCHANGE_BLK.M_Change_Type, Operator.Equal, DataTypeUtils.String, strChangeType);

                return BRCHANGE_BLK.UpdateEntityByCondition(eChangeBlk, sSql.GetFilterCondition(), strField);
        }

        /// <summary>
        /// 更新CHANGE_BLK表一KEY和二KEY資料
        /// </summary>
        /// <param name="eReissueCard">CHANGE_BLK實體</param>
        /// <param name="strCardNo">卡號</param>
        /// <param name="strUploadFlag">Keyin標示</param>
        /// <param name="strChangeType">異動類型</param>
        /// <param name="strField">要更新欄位的集合</param>
        /// <returns>rue成功，false失敗</returns>
        public static bool Update(EntityCHANGE_BLK eChangeBlk, string strCardNo, string strUploadFlag, string strChangeType, string[] strField)
        {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityCHANGE_BLK.M_Card_No, Operator.Equal, DataTypeUtils.String, strCardNo);
                sSql.AddCondition(EntityCHANGE_BLK.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityCHANGE_BLK.M_Change_Type, Operator.Equal, DataTypeUtils.String, strChangeType);

                return BRCHANGE_BLK.UpdateEntityByCondition(eChangeBlk, sSql.GetFilterCondition(), strField);    
        }

        /// <summary>
        /// 先刪除CHANGE_BLK表資料后再新增
        /// </summary>
        /// <param name="eShop">CHANGE_BLK實體</param>
        /// <param name="strCardNo">卡號</param>
        /// <param name="strKeyInFlag">Keyin類別(1-一KEY/2-二KEY)</param>
        /// <param name="strUploadFlag">Keyin標示</param>
        /// <param name="strChangeType">異動類型</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Insert(EntityCHANGE_BLK eChangeBlk, string strCardNo, string strKeyInFlag , string strUploadFlag, string strChangeType)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityCHANGE_BLK.M_Card_No, Operator.Equal, DataTypeUtils.String, strCardNo);
                sSql.AddCondition(EntityCHANGE_BLK.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
                sSql.AddCondition(EntityCHANGE_BLK.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityCHANGE_BLK.M_Change_Type, Operator.Equal, DataTypeUtils.String, strChangeType);
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (!DeleteEntityByCondition(eChangeBlk, sSql.GetFilterCondition()))
                    {
                        return false;
                    }

                    if (!eChangeBlk.DB_InsertEntity())
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

        /// <summary>
        /// 用Entity方式插入資料庫
        /// </summary>
        /// <param name="eChangeBlk">Entity</param>
        /// <returns>true成功,false失敗</returns>
        public static bool AddEntity(EntityCHANGE_BLK eChangeBlk)
        {
            try
            {
                return eChangeBlk.DB_InsertEntity();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
