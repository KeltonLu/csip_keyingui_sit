using System;
using System.Collections.Generic;
using System.Text;
using CSIPCommonModel.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;

namespace CSIPKeyInGUI.BusinessRules
{
    /// <summary>
    /// SHOP_BASIC 業務類
    /// </summary>
    public class BRSHOP_BASIC : CSIPCommonModel.BusinessRules.BRBase<EntitySHOP_BASIC>
    {
        #region SQL

        public const string SEL_CONDITION = "and convert(int, convert(varchar(08),getdate(),112))-convert(int, keyin_day)<=5";

        #endregion
        
        /// <summary>
        /// 根據統一編號(1)、統一編號(2)查詢
        /// </summary>
        /// <param name="strUniNo1">統一編號(1)</param>
        /// <param name="strUniNo2">統一編號(2)</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntitySHOP_BASIC> SelectEntitySet(string strUniNo1, string strUniNo2)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntitySHOP_BASIC.M_uni_no1, Operator.Equal, DataTypeUtils.String, strUniNo1);
                sSql.AddCondition(EntitySHOP_BASIC.M_uni_no2, Operator.Equal, DataTypeUtils.String, strUniNo2);

                return BRSHOP_BASIC.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根據統一編號(1)、統一編號(2)查詢
        /// </summary>
        /// <param name="strUniNo1">統一編號(1)</param>
        /// <param name="strUniNo2">統一編號(2)</param>
        /// <param name="strKeyInFlag">keyin標識</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntitySHOP_BASIC> SelectData(string strUniNo1, string strUniNo2, string strKeyInFlag)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntitySHOP_BASIC.M_uni_no1, Operator.Equal, DataTypeUtils.String, strUniNo1);
                sSql.AddCondition(EntitySHOP_BASIC.M_uni_no2, Operator.Equal, DataTypeUtils.String, strUniNo2);
                sSql.AddCondition(EntitySHOP_BASIC.M_keyin_flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);

                return BRSHOP_BASIC.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// 根據統一編號(1)、統一編號(2)、keyin標識、上送主機標識、KeyInDay查詢
        /// </summary>
        /// <param name="strUniNo1">統一編號(1)</param>
        /// <param name="strUniNo2">統一編號(2)</param>   
        /// <param name="strKeyInFlag">keyin標識</param>
        /// <param name="strSendHostFlag">上送主機標識</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntitySHOP_BASIC> SelectEntitySet(string strUniNo1, string strUniNo2, string strKeyInFlag, string strSendHostFlag)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntitySHOP_BASIC.M_uni_no1, Operator.Equal, DataTypeUtils.String, strUniNo1);
                sSql.AddCondition(EntitySHOP_BASIC.M_uni_no2, Operator.Equal, DataTypeUtils.String, strUniNo2);
                sSql.AddCondition(EntitySHOP_BASIC.M_keyin_flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
                sSql.AddCondition(EntitySHOP_BASIC.M_sendhost_flag, Operator.Equal, DataTypeUtils.String, strSendHostFlag);

                string strSql = sSql.GetFilterCondition();
                return BRSHOP_BASIC.Search(strSql);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根據統一編號(1)、統一編號(2)查詢一、二Key資料
        /// </summary>
        /// <param name="strUniNo1">統一編號(1)</param>
        /// <param name="strUniNo2">統一編號(2)</param>   
        /// <param name="strSendHostFlag">上送主機標識</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntitySHOP_BASIC> SelectEntitySet(string strUniNo1, string strUniNo2, string strSendHostFlag)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntitySHOP_BASIC.M_uni_no1, Operator.Equal, DataTypeUtils.String, strUniNo1);
                sSql.AddCondition(EntitySHOP_BASIC.M_uni_no2, Operator.Equal, DataTypeUtils.String, strUniNo2);
                sSql.AddCondition(EntitySHOP_BASIC.M_sendhost_flag, Operator.Equal, DataTypeUtils.String, strSendHostFlag);
                
                string strSql = sSql.GetFilterCondition();
                return BRSHOP_BASIC.Search(strSql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根據統一編號（1）、統一編號（2）更新SHOP_BASIC
        /// </summary>
        /// <param name="eShopBasic">實體</param>
        /// <param name="strUniNo1">統一編號（1）</param>
        /// <param name="strUniNo2">統一編號（2）</param>
        /// <param name="strFields">要更新列的數組</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Update(EntitySHOP_BASIC eShopBasic, string strUniNo1, string strUniNo2, string[] strFields)
        {
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntitySHOP_BASIC.M_uni_no1, Operator.Equal, DataTypeUtils.String, strUniNo1);
            sSql.AddCondition(EntitySHOP_BASIC.M_uni_no2, Operator.Equal, DataTypeUtils.String, strUniNo2);

            return BRSHOP_BASIC.UpdateEntityByCondition(eShopBasic, sSql.GetFilterCondition(), strFields);
        }

        /// <summary>
        /// 根據統一編號（1）、統一編號（2）更新SHOP_BASIC
        /// </summary>
        /// <param name="eShopBasic">實體</param>
        /// <param name="strUniNo1">統一編號（1）</param>
        /// <param name="strUniNo2">統一編號（2）</param>
        /// <param name="strKeyInFlag">keyin標識</param>
        /// <param name="strSendHostFlag">上送主機標識</param>
        /// <param name="strKeyInDay">keyin日期</param>
        /// <param name="strFields">要更新列的數組</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Update(EntitySHOP_BASIC eShopBasic, string strUniNo1, string strUniNo2, string strKeyInFlag, string strSendHostFlag, string strKeyInDay,string[] strFields)
        {
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntitySHOP_BASIC.M_uni_no1, Operator.Equal, DataTypeUtils.String, strUniNo1);
            sSql.AddCondition(EntitySHOP_BASIC.M_uni_no2, Operator.Equal, DataTypeUtils.String, strUniNo2);
            //sSql.AddCondition(EntitySHOP_BASIC.M_keyin_day, Operator.Equal, DataTypeUtils.String, strKeyInDay);
            sSql.AddCondition(EntitySHOP_BASIC.M_keyin_flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
            sSql.AddCondition(EntitySHOP_BASIC.M_sendhost_flag, Operator.Equal, DataTypeUtils.String, strSendHostFlag);

            return BRSHOP_BASIC.UpdateEntityByCondition(eShopBasic, sSql.GetFilterCondition(), strFields);
        }

        /// <summary>
        /// 更新SHOP_BASIC
        /// </summary>
        /// <param name="eShopBasic">實體</param>
        /// <param name="strUniNo1">統一編號（1）</param>
        /// <param name="strUniNo2">統一編號（2）</param>
        /// <param name="strFields">要更新列的數組</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Update(EntitySHOP_BASIC eShopBasic, string strUniNo1, string strUniNo2, string strKeyInDay, string strKeyInFlag, string strSendHostFlag)
        {
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntitySHOP_BASIC.M_uni_no1, Operator.Equal, DataTypeUtils.String, strUniNo1);
            sSql.AddCondition(EntitySHOP_BASIC.M_uni_no2, Operator.Equal, DataTypeUtils.String, strUniNo2);
            //sSql.AddCondition(EntitySHOP_BASIC.M_keyin_day, Operator.Equal, DataTypeUtils.String, strKeyInDay);
            sSql.AddCondition(EntitySHOP_BASIC.M_keyin_flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
            sSql.AddCondition(EntitySHOP_BASIC.M_sendhost_flag, Operator.Equal, DataTypeUtils.String, strSendHostFlag);

            return eShopBasic.DB_UpdateEntity(sSql.GetFilterCondition());
        }

        /// <summary>
        /// 用Entity方式插入資料庫
        /// </summary>
        /// <param name="eShopBasic">Entity</param>
        /// <returns>true成功,false失敗</returns>
        public static bool AddEntity(EntitySHOP_BASIC eShopBasic)
        {
            try
            {
                return eShopBasic.DB_InsertEntity();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
