using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Framework.Data.OM;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;

namespace CSIPKeyInGUI.BusinessRules
{
    public class BRAuto_Pay_Popul : CSIPCommonModel.BusinessRules.BRBase<EntityAuto_Pay_Popul>
    {
        
        #region SQL

        public const string SEL_QRY_Auto_Pay_Popul_ACH1Key = @" Select Popul_EmpNo,Popul_No,Case_Class from dbo.Auto_Pay_Popul  
 where Cus_ID= @query_key_Id and Receive_Number = @query_key_Num and  KeyIn_Flag='1'";

        public const string SEL_UP_Auto_Pay_Popul_ACH1Key = @"update Auto_Pay_Popul set " +
                                                             "Popul_EmpNo = @Popul_EmpNo," +
                                                             "Popul_No = @Popul_No," +
                                                             "user_id = @UserId," +
                                                             "mod_date = getDate()," +
                                                             "FUNCTION_CODE = @FunctionCode," +

                                                             "Case_Class = @Case_Class" +
                                                             " where Cus_ID = @Cus_Id " +
                                                             " and Receive_Number = @Receive_Number " +
                                                             " and KeyIn_Flag = @KeyFlag ";

        public const string SEL_QRY_Auto_Pay_Popul_ACH2Key = @" Select Popul_EmpNo,Popul_No,Case_Class from dbo.Auto_Pay_Popul  " +
                                                               "  where Cus_ID= @query_key_Id and Receive_Number = @query_key_Num and  KeyIn_Flag='2'";

        public const string SEL_QRY_Auto_Pay_Popul_ACH2Key_All = @" Select * from dbo.Auto_Pay_Popul  " +
                                                               "  where Cus_ID= @query_key_Id and Receive_Number = @query_key_Num and  KeyIn_Flag='2'";


        public const string UP_Auto_Pay_Popul_2Key = @"update Auto_Pay_Popul set " +
                                                             "Popul_EmpNo = @Popul_EmpNo," +
                                                             "Popul_No = @Popul_No," +
                                                             "user_id = @UserId," +
                                                             "mod_date = getDate()," +
                                                             "Case_Class = @Case_Class" +
                                                             " where Cus_ID = @Cus_Id " +
                                                             " and Receive_Number = @Receive_Number " +
                                                             " and KeyIn_Flag = @KeyFlag ";
        public const string SEL_AUTO_PAY = @"SELECT {0} FROM AUTO_PAY WHERE ";
        //public const string SEL_AUTO_PAY_POPUL = @"SELECT {0} FROM Auto_Pay_Popul WHERE ";
        public const string SEL_AUTO_PAY_POPUL = @"SELECT TOP(1) {0} FROM Auto_Pay_Popul WHERE ";
        #endregion

        /// <summary>
        /// </summary>
        /// <param name="strUserId">身分證號碼</param>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <returns>查詢結果:記錄DataTable</returns>
        public static DataTable SearchQryACH1Key(string strUserId, string strReceiveNumber)
        {
            string strSql = SEL_QRY_Auto_Pay_Popul_ACH1Key;
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = strSql;

            //* 傳入關鍵字變量

            SqlParameter parmQuery_Num = new SqlParameter("@query_key_Num", strReceiveNumber);
            sqlcmd.Parameters.Add(parmQuery_Num);

            SqlParameter parmQuery_Id = new SqlParameter("@query_key_Id", strUserId);
            sqlcmd.Parameters.Add(parmQuery_Id);

            DataTable dtbl = new DataTable();
            try
            {
                //* 查詢記錄
                dtbl = BRAuto_Pay_Popul.SearchOnDataSet(sqlcmd).Tables[0];
                return dtbl;
            }
            catch (Exception exp)
            {
                BRCustomer_Log.SaveLog(exp);
                return null;
            }
        }

        /// <summary>
        /// 更新Keyin資料
        /// </summary>

        /// <returns>true 成功， false失敗</returns>
        public static bool UpdateKeyInfo(string strReceive_Number, string strCus_Id, string strCase_Class, string strPopul_No, string strPopul_EmpNo, string strKeyIn_Flag, string strYearTime, string strUserId, string strFunctionCode, string strIsEnad)
        {

            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandText = SEL_UP_Auto_Pay_Popul_ACH1Key;
            sqlComm.CommandType = CommandType.Text;

            SqlParameter parmReceive_Number = new SqlParameter("@Receive_Number", strReceive_Number);
            sqlComm.Parameters.Add(parmReceive_Number);

            SqlParameter parmCus_ID = new SqlParameter("@Cus_Id", strCus_Id);
            sqlComm.Parameters.Add(parmCus_ID);



            SqlParameter parmPopul_EmpNo = new SqlParameter("@Popul_EmpNo", strPopul_EmpNo);
            sqlComm.Parameters.Add(parmPopul_EmpNo);


            SqlParameter parmstrPopul_No = new SqlParameter("@Popul_No", strPopul_No);
            sqlComm.Parameters.Add(parmstrPopul_No);

            SqlParameter parmCase_Class = new SqlParameter("@Case_Class", strCase_Class);
            sqlComm.Parameters.Add(parmCase_Class);

            SqlParameter parmKeyIn_Flag = new SqlParameter("@KeyFlag", strKeyIn_Flag);
            sqlComm.Parameters.Add(parmKeyIn_Flag);

            SqlParameter parmUserId = new SqlParameter("@UserId", strUserId);
            sqlComm.Parameters.Add(parmUserId);

            SqlParameter parmYearTime = new SqlParameter("@YearTime", strYearTime);
            sqlComm.Parameters.Add(parmYearTime);

            SqlParameter parmFunctionCode = new SqlParameter("@FunctionCode", strFunctionCode);
            sqlComm.Parameters.Add(parmFunctionCode);

            SqlParameter parmIsEnd = new SqlParameter("@IsEnd", strIsEnad);
            sqlComm.Parameters.Add(parmIsEnd);

            return BRAuto_Pay_Popul.Update(sqlComm);
        }

        /// <summary>
        /// 用Entity方式插入資料庫
        /// </summary>
        /// <param name="eOtherBankTemp">Entity</param>
        /// <returns>true成功,false失敗</returns>
        public static bool AddEntity(EntityAuto_Pay_Popul eOtherBankTemp)
        {
            try
            {
                return eOtherBankTemp.DB_InsertEntity();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="strUserId">身分證號碼</param>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <returns>查詢結果:記錄DataTable</returns>
        public static DataTable SearchQryACH2Key(string strUserId, string strReceiveNumber)
        {
            string strSql = SEL_QRY_Auto_Pay_Popul_ACH2Key;
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = strSql;

            //* 傳入關鍵字變量

            SqlParameter parmQuery_Num = new SqlParameter("@query_key_Num", strReceiveNumber);
            sqlcmd.Parameters.Add(parmQuery_Num);

            SqlParameter parmQuery_Id = new SqlParameter("@query_key_Id", strUserId);
            sqlcmd.Parameters.Add(parmQuery_Id);

            DataTable dtbl = new DataTable();
            try
            {
                //* 查詢記錄
                dtbl = BRAuto_Pay_Popul.SearchOnDataSet(sqlcmd).Tables[0];
                return dtbl;
            }
            catch (Exception exp)
            {
                BRCustomer_Log.SaveLog(exp);
                return null;
            }
        }


        public static DataTable SearchQryACH2KeyAll(string strUserId, string strReceiveNumber)
        {
            string strSql = SEL_QRY_Auto_Pay_Popul_ACH2Key_All;
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = strSql;

            //* 傳入關鍵字變量

            SqlParameter parmQuery_Num = new SqlParameter("@query_key_Num", strReceiveNumber);
            sqlcmd.Parameters.Add(parmQuery_Num);

            SqlParameter parmQuery_Id = new SqlParameter("@query_key_Id", strUserId);
            sqlcmd.Parameters.Add(parmQuery_Id);

            DataTable dtbl = new DataTable();
            try
            {
                //* 查詢記錄
                dtbl = BRAuto_Pay_Popul.SearchOnDataSet(sqlcmd).Tables[0];
                return dtbl;
            }
            catch (Exception exp)
            {
                BRCustomer_Log.SaveLog(exp);
                return null;
            }
        }

        /// <summary>
        /// 更新2Keyin資料
        /// </summary>

        /// <returns>true 成功， false失敗</returns>
        public static bool Update2KeyInfo(string strReceive_Number, string strCus_Id, string strCase_Class, string strPopul_No, string strPopul_EmpNo, string strKeyIn_Flag, string strYearTime, string strUserId)
        {

            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandText = UP_Auto_Pay_Popul_2Key;
            sqlComm.CommandType = CommandType.Text;

            SqlParameter parmReceive_Number = new SqlParameter("@Receive_Number", strReceive_Number);
            sqlComm.Parameters.Add(parmReceive_Number);

            SqlParameter parmCus_ID = new SqlParameter("@Cus_Id", strCus_Id);
            sqlComm.Parameters.Add(parmCus_ID);

            SqlParameter parmPopul_EmpNo = new SqlParameter("@Popul_EmpNo", strPopul_EmpNo);
            sqlComm.Parameters.Add(parmPopul_EmpNo);


            SqlParameter parmstrPopul_No = new SqlParameter("@Popul_No", strPopul_No);
            sqlComm.Parameters.Add(parmstrPopul_No);

            SqlParameter parmCase_Class = new SqlParameter("@Case_Class", strCase_Class);
            sqlComm.Parameters.Add(parmCase_Class);

            SqlParameter parmKeyIn_Flag = new SqlParameter("@KeyFlag", strKeyIn_Flag);
            sqlComm.Parameters.Add(parmKeyIn_Flag);

            SqlParameter parmUserId = new SqlParameter("@UserId", strUserId);
            sqlComm.Parameters.Add(parmUserId);

            SqlParameter parmYearTime = new SqlParameter("@YearTime", strYearTime);
            sqlComm.Parameters.Add(parmYearTime);



            return BRAuto_Pay_Popul.Update(sqlComm);
        }


        /// <summary>
        /// Auto_Pay_Popul表修改數據
        /// </summary>
        /// <param name="eAutoPay"></param>
        /// <param name="strField"></param>
        /// <param name="strUserId"></param>
        /// <param name="strAddFlag"></param>
        /// <param name="strUploadFlag"></param>
        /// <param name="strKeyFlag"></param>
        /// <returns></returns>
        public static bool Update(EntityAuto_Pay_Popul eAutoPayPopul, string[] strField, string strCusId, string strKeyInFlag, string strReceiveNumber)
        {
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntityAuto_Pay_Popul.M_Cus_Id, Operator.Equal, DataTypeUtils.String, strCusId);
            sSql.AddCondition(EntityAuto_Pay_Popul.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);
            sSql.AddCondition(EntityAuto_Pay_Popul.M_Receive_Number, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
            return BRAuto_Pay_Popul.UpdateEntityByCondition(eAutoPayPopul, sSql.GetFilterCondition(), strField);
        }

        /// <summary>
        /// 驗證收件編號和身份證號碼
        /// </summary>
        /// <param name="strUserId">銀行信息</param>
        /// <returns>DataSet</returns>
        public static DataSet SelectDetail(string strUserId, string strAddFlag, string strUploadFlag, string strKeyFlag, string strReceiveNumber, string strColumns)
        {
            DataSet dstInfo = null;
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntityAUTO_PAY.M_Cus_ID, Operator.Equal, DataTypeUtils.String, strUserId);
            sSql.AddCondition(EntityAUTO_PAY.M_Add_Flag, Operator.Equal, DataTypeUtils.String, strAddFlag);
            sSql.AddCondition(EntityAUTO_PAY.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);
            sSql.AddCondition(EntityAUTO_PAY.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyFlag);
            sSql.AddCondition(EntityAUTO_PAY.M_Receive_Number, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
            string strSqlCmd = string.Format(SEL_AUTO_PAY, strColumns) + sSql.GetFilterCondition().Substring(4, sSql.GetFilterCondition().Length - 4);

            dstInfo = BRAUTO_PAY.SearchOnDataSet(strSqlCmd);

            return dstInfo;
        }

        /// <summary>
        /// 驗證收件編號和身份證號碼
        /// </summary>
        /// <param name="strReceiveNumber"></param>
        /// <param name="strAddFlag"></param>
        /// <param name="strUploadFlag"></param>
        /// <param name="strColumns"></param>
        /// <returns></returns>
        //public static DataSet Select(string strReceiveNumber, string strAddFlag, string strUploadFlag, string strColumns)
        public static DataSet Select(string strReceiveNumber, string strColumns)
        {
            DataSet dstInfo = null;
            SqlHelper sSql = new SqlHelper();

            //檢查收編是否已用過，只檢查AUTO_PAY_POPUL表
            sSql.AddCondition(EntityAUTO_PAY.M_Receive_Number, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
            //sSql.AddCondition(EntityAUTO_PAY.M_Add_Flag, Operator.Equal, DataTypeUtils.String, strAddFlag);
            //sSql.AddCondition(EntityAUTO_PAY.M_Upload_Flag, Operator.Equal, DataTypeUtils.String, strUploadFlag);

            //string strSqlCmd = string.Format(SEL_AUTO_PAY, strColumns) + sSql.GetFilterCondition().Substring(4, sSql.GetFilterCondition().Length - 4);
            string strSqlCmd = string.Format(SEL_AUTO_PAY_POPUL, strColumns) + sSql.GetFilterCondition().Substring(4, sSql.GetFilterCondition().Length - 4);

            dstInfo = BRAUTO_PAY.SearchOnDataSet(strSqlCmd);

            return dstInfo;
        }

        /// <summary>
        /// 驗證收件編號和身份證號碼
        /// </summary>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strAddFlag">身份證號碼</param>
        /// <param name="strUploadFlag"></param>
        /// <param name="strColumns"></param>
        /// <returns></returns>
        public static DataSet SelectFlag(string strReceiveNumber, string strAddFlag, string strUploadFlag, string strColumns)
        {
            DataSet dstInfo = null;
            SqlHelper sSql = new SqlHelper();

            sSql.AddCondition(EntityAUTO_PAY.M_Receive_Number, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
            sSql.AddCondition(EntityAUTO_PAY.M_Add_Flag, Operator.Equal, DataTypeUtils.String, strAddFlag);
            sSql.AddCondition(EntityAUTO_PAY.M_Upload_Flag, Operator.NotEqual, DataTypeUtils.String, strUploadFlag);

            string strSqlCmd = string.Format(SEL_AUTO_PAY, strColumns) + sSql.GetFilterCondition().Substring(4, sSql.GetFilterCondition().Length - 4);

            dstInfo = BRAUTO_PAY.SearchOnDataSet(strSqlCmd);

            return dstInfo;
        }

        /// <summary>
        /// 查詢推廣員信息
        /// </summary>
        /// <param name="strUserId"></param>
        /// <param name="strReceiveNumber"></param>
        /// <param name="strColumns"></param>
        /// <returns></returns>
        //public static DataSet SelectDetail(string strUserId, string strReceiveNumber, string strKeyFlag, string strColumns)
        public static DataSet SelectDetail(string strUserId, string strColumns)
        {
            DataSet dstInfo = null;
            SqlHelper sSql = new SqlHelper();
            sSql.AddCondition(EntityAUTO_PAY.M_Cus_ID, Operator.Equal, DataTypeUtils.String, strUserId);
            //sSql.AddCondition(EntityAUTO_PAY.M_Receive_Number, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
            //sSql.AddCondition(EntityAUTO_PAY.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyFlag);
            sSql.AddOrderCondition(EntityAUTO_PAY.M_mod_date, ESortType.DESC);
            string strSqlCmd = string.Format(SEL_AUTO_PAY_POPUL, strColumns) + sSql.GetFilterCondition().Substring(4, sSql.GetFilterCondition().Length - 4);

            dstInfo = BRAuto_Pay_Popul.SearchOnDataSet(strSqlCmd);

            return dstInfo;
        }

    }
     
}
