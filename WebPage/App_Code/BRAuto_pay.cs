//******************************************************************
//*  作    者：林矩敬
//*  功能說明：自扣案件資料
//*  創建日期：2019/06/13
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************

using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using System;
using System.Data;

namespace CSIPKeyInGUI.BusinessRules_new
{
    public class BRAuto_pay : CSIPCommonModel.BusinessRules.BRBase<EntityAUTO_PAY>
    {
        /// <summary>
        /// 更新資料
        /// </summary>
        /// <param name="eAutoPay">Auto_Pay實體</param>
        /// <param name="strField">要更新欄位的集合</param>
        /// <param name="strUserId">身分證號碼</param>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strAddFlag">資料類別</param>
        /// <param name="strUploadFlag">異動類別</param>
        /// <param name="strKeyFlag">KeyIn類別</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Update(EntityAUTO_PAY eAutoPay, string[] strField, string strUserId, string strReceiveNumber, string strAddFlag, string strUploadFlag, string strKeyFlag)
        {
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntityAUTO_PAY.M_Cus_ID, Operator.Equal, DataTypeUtils.String, strUserId);
            sSql.AddCondition(EntityAUTO_PAY.M_Receive_Number, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
            sSql.AddCondition(EntityAUTO_PAY.M_Add_Flag, Operator.Equal, DataTypeUtils.String, strAddFlag);
            sSql.AddCondition(EntityAUTO_PAY.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
            sSql.AddCondition(EntityAUTO_PAY.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyFlag);

            return BRAUTO_PAY.UpdateEntityByCondition(eAutoPay, sSql.GetFilterCondition(), strField);

        }

        /// <summary>
        /// 查詢中信及郵局 自扣一KEY資料
        /// </summary>
        /// <param name="strUserId">身分證號碼</param>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strAddFlag">資料類別</param>
        /// <param name="strUploadFlag">異動類別</param>
        /// <param name="strKeyFlag">KeyIn類別</param>
        /// <param name="strColumns">要查詢得到的欄位</param>
        /// <returns>DataSet</returns>
        public static DataSet SelectDetail(string strUserId, string strReceiveNumber, string strAddFlag, string strUploadFlag, string strKeyFlag, string strColumns)
        {
            DataSet dstInfo = null;
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntityAUTO_PAY.M_Cus_ID, Operator.Equal, DataTypeUtils.String, strUserId);
            sSql.AddCondition(EntityAUTO_PAY.M_Receive_Number, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
            sSql.AddCondition(EntityAUTO_PAY.M_Add_Flag, Operator.Equal, DataTypeUtils.String, strAddFlag);
            sSql.AddCondition(EntityAUTO_PAY.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
            sSql.AddCondition(EntityAUTO_PAY.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyFlag);
            string SEL_AUTO_PAY = @"SELECT {0} FROM AUTO_PAY WHERE ";
            string strSqlCmd = string.Format(SEL_AUTO_PAY, strColumns) + sSql.GetFilterCondition().Substring(4, sSql.GetFilterCondition().Length - 4);

            dstInfo = BRAUTO_PAY.SearchOnDataSet(strSqlCmd);

            return dstInfo;
        }


        /// <summary>
        /// 查詢資料庫一、二KEY信息
        /// </summary>
        /// <param name="strUserId">身分證號碼</param>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strAddFlag">資料類別</param>
        /// <param name="strUploadFlag">異動類別</param>
        /// <param name="strKeyFlag">KeyIn類別</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntityAUTO_PAY> SelectEntitySet(string strUserId, string strReceiveNumber, string strAddFlag, string strUploadFlag, string strKeyFlag)
        {
            SqlHelper sSql = new SqlHelper();
            EntitySet<EntityAUTO_PAY> eAutoPay = null;
            try
            {
                sSql.AddCondition(EntityAUTO_PAY.M_Cus_ID, Operator.Equal, DataTypeUtils.String, strUserId);
                sSql.AddCondition(EntityAUTO_PAY.M_Receive_Number, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
                sSql.AddCondition(EntityAUTO_PAY.M_Add_Flag, Operator.Equal, DataTypeUtils.String, strAddFlag);
                sSql.AddCondition(EntityAUTO_PAY.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityAUTO_PAY.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyFlag);

                eAutoPay = BRAUTO_PAY.Search(sSql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return eAutoPay;
        }

    }
}
