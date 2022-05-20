//******************************************************************
//*  作    者：DarrenWang
//*  功能說明：表單欄位
//*  創建日期：2018/11/22
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using CSIPCommonModel.EntityLayer;
using System.Data;
using System.Data.SqlClient;
using Framework.Common.Logging;

namespace CSIPCommonModel.BusinessRules
{
    public class BRUtilities_Etag : BRBase<EntityUTILITIES>
    {
        public static bool DuplicateReceiveNumber(string primaryCardID, string receiveNumber, string keyinFlag)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
SELECT COUNT(0) FROM [dbo].[Utilities_Etag] WITH(NOLOCK) 
WHERE PRIMARY_CARD_ID != @PRIMARY_CARD_ID AND RECEIVE_NUMBER = @RECEIVE_NUMBER AND KEYIN_FLAG = @KEYIN_FLAG";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@PRIMARY_CARD_ID", primaryCardID));
            sqlcmd.Parameters.Add(new SqlParameter("@RECEIVE_NUMBER", receiveNumber));
            sqlcmd.Parameters.Add(new SqlParameter("@KEYIN_FLAG", keyinFlag));

            try
            {
                DataSet resultSet = BRUtilities_Etag.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    int count = Convert.ToInt32(resultSet.Tables[0].Rows[0][0].ToString());
                    if (count > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        public static bool IsExistReceiveNumber(string primaryCardID, string receiveNumber, string keyinFlag)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
SELECT COUNT(0) FROM [dbo].[Utilities_Etag] WITH(NOLOCK) 
WHERE PRIMARY_CARD_ID = @PRIMARY_CARD_ID AND RECEIVE_NUMBER = @RECEIVE_NUMBER AND KEYIN_FLAG = @KEYIN_FLAG";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@PRIMARY_CARD_ID", primaryCardID));
            sqlcmd.Parameters.Add(new SqlParameter("@RECEIVE_NUMBER", receiveNumber));
            sqlcmd.Parameters.Add(new SqlParameter("@KEYIN_FLAG", keyinFlag));

            try
            {
                DataSet resultSet = BRUtilities_Etag.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    int count = Convert.ToInt32(resultSet.Tables[0].Rows[0][0].ToString());
                    if (count > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 新增Etag資料
        /// </summary>
        /// <param name="receiveNumber"></param>
        /// <param name="primaryCardID"></param>
        /// <param name="name"></param>
        /// <param name="etagFlag"></param>
        /// <param name="templateParkFlag"></param>
        /// <param name="monthlyParkFlag"></param>
        /// <param name="applyType"></param>
        /// <param name="ownersID"></param>
        /// <param name="monthlyParkingNO"></param>
        /// <param name="populNo"></param>
        /// <param name="populEmpNo"></param>
        /// <param name="introducerCardID"></param>
        /// <param name="agentID"></param>
        /// <param name="keyInDate"></param>
        /// <param name="keyInFlag"></param>
        /// <returns></returns>
        public static bool InsertIntoUtilities_Etag(string receiveNumber, string primaryCardID, string name, bool etagFlag, bool templateParkFlag, bool monthlyParkFlag, 
                                                    string applyType, string ownersID, string monthlyParkingNO, string populNo, string populEmpNo, string introducerCardID, 
                                                    string agentID, string keyInDate, string keyInFlag)
        {
            bool addResult = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
INSERT INTO [dbo].[Utilities_Etag] (RECEIVE_NUMBER, PRIMARY_CARD_ID, NAME, ETAG_FLAG, TEMPLATE_PARK_FLAG, MONTHLY_PARK_FLAG, APPLY_TYPE, OWNERS_ID, 
                                    MONTHLY_PARKING_NO, POPUL_NO, POPUL_EMP_NO, INTRODUCER_CARD_ID, AGENT_ID, KEYIN_DATE, KEYIN_FLAG) 
VALUES (@RECEIVE_NUMBER, @PRIMARY_CARD_ID, @NAME, @ETAG_FLAG, @TEMPLATE_PARK_FLAG, @MONTHLY_PARK_FLAG, @APPLY_TYPE, @OWNERS_ID, @MONTHLY_PARKING_NO, 
        @POPUL_NO, @POPUL_EMP_NO, @INTRODUCER_CARD_ID, @AGENT_ID, @KEYIN_DATE, @KEYIN_FLAG)";

            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@RECEIVE_NUMBER", receiveNumber));
            sqlcmd.Parameters.Add(new SqlParameter("@PRIMARY_CARD_ID", primaryCardID));
            sqlcmd.Parameters.Add(new SqlParameter("@NAME", name));
            sqlcmd.Parameters.Add(new SqlParameter("@ETAG_FLAG", etagFlag));
            sqlcmd.Parameters.Add(new SqlParameter("@TEMPLATE_PARK_FLAG", templateParkFlag));
            sqlcmd.Parameters.Add(new SqlParameter("@MONTHLY_PARK_FLAG", monthlyParkFlag));
            sqlcmd.Parameters.Add(new SqlParameter("@APPLY_TYPE", applyType));
            sqlcmd.Parameters.Add(new SqlParameter("@OWNERS_ID", ownersID));
            sqlcmd.Parameters.Add(new SqlParameter("@MONTHLY_PARKING_NO", monthlyParkingNO));
            sqlcmd.Parameters.Add(new SqlParameter("@POPUL_NO", populNo));
            sqlcmd.Parameters.Add(new SqlParameter("@POPUL_EMP_NO", populEmpNo));
            sqlcmd.Parameters.Add(new SqlParameter("@INTRODUCER_CARD_ID", introducerCardID));
            sqlcmd.Parameters.Add(new SqlParameter("@AGENT_ID", agentID));
            sqlcmd.Parameters.Add(new SqlParameter("@KEYIN_DATE", keyInDate));
            sqlcmd.Parameters.Add(new SqlParameter("@KEYIN_FLAG", keyInFlag));

            try
            {
                addResult = BRUtilities_Etag.Add(sqlcmd);
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return addResult;
        }

        /// <summary>
        /// 修改Etag資料
        /// </summary>
        /// <param name="receiveNumber"></param>
        /// <param name="primaryCardID"></param>
        /// <param name="name"></param>
        /// <param name="etagFlag"></param>
        /// <param name="templateParkFlag"></param>
        /// <param name="monthlyParkFlag"></param>
        /// <param name="applyType"></param>
        /// <param name="ownersID"></param>
        /// <param name="monthlyParkingNO"></param>
        /// <param name="populNo"></param>
        /// <param name="populEmpNo"></param>
        /// <param name="introducerCardID"></param>
        /// <param name="agentID"></param>
        /// <param name="keyInDate"></param>
        /// <param name="keyInFlag"></param>
        /// <returns></returns>
        public static bool UpdateUtilities_Etag(string receiveNumber, string primaryCardID, string name, bool etagFlag, bool templateParkFlag, bool monthlyParkFlag,
                                                    string applyType, string ownersID, string monthlyParkingNO, string populNo, string populEmpNo, string introducerCardID,
                                                    string agentID, string keyInDate, string keyInFlag)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
UPDATE [dbo].[Utilities_Etag]
SET ETAG_FLAG = @ETAG_FLAG, TEMPLATE_PARK_FLAG = @TEMPLATE_PARK_FLAG, MONTHLY_PARK_FLAG = @MONTHLY_PARK_FLAG, APPLY_TYPE = @APPLY_TYPE, OWNERS_ID = @OWNERS_ID, 
    MONTHLY_PARKING_NO = @MONTHLY_PARKING_NO, POPUL_NO = @POPUL_NO, POPUL_EMP_NO = @POPUL_EMP_NO, INTRODUCER_CARD_ID = @INTRODUCER_CARD_ID, AGENT_ID = @AGENT_ID, 
    KEYIN_DATE = @KEYIN_DATE
WHERE RECEIVE_NUMBER = @RECEIVE_NUMBER AND PRIMARY_CARD_ID = @PRIMARY_CARD_ID AND KEYIN_FLAG = @KEYIN_FLAG";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@RECEIVE_NUMBER", receiveNumber));
            sqlcmd.Parameters.Add(new SqlParameter("@PRIMARY_CARD_ID", primaryCardID));
            sqlcmd.Parameters.Add(new SqlParameter("@NAME", name));
            sqlcmd.Parameters.Add(new SqlParameter("@ETAG_FLAG", etagFlag));
            sqlcmd.Parameters.Add(new SqlParameter("@TEMPLATE_PARK_FLAG", templateParkFlag));
            sqlcmd.Parameters.Add(new SqlParameter("@MONTHLY_PARK_FLAG", monthlyParkFlag));
            sqlcmd.Parameters.Add(new SqlParameter("@APPLY_TYPE", applyType));
            sqlcmd.Parameters.Add(new SqlParameter("@OWNERS_ID", ownersID));
            sqlcmd.Parameters.Add(new SqlParameter("@MONTHLY_PARKING_NO", monthlyParkingNO));
            sqlcmd.Parameters.Add(new SqlParameter("@POPUL_NO", populNo));
            sqlcmd.Parameters.Add(new SqlParameter("@POPUL_EMP_NO", populEmpNo));
            sqlcmd.Parameters.Add(new SqlParameter("@INTRODUCER_CARD_ID", introducerCardID));
            sqlcmd.Parameters.Add(new SqlParameter("@AGENT_ID", agentID));
            sqlcmd.Parameters.Add(new SqlParameter("@KEYIN_DATE", keyInDate));
            sqlcmd.Parameters.Add(new SqlParameter("@KEYIN_FLAG", keyInFlag));

            try
            {
                result = BRUtilities_Etag.Add(sqlcmd);
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        public static bool InsertIntoUtilities_Etag_PlateNo(string receiveNumber, string keyInFlag, string plateNo, string sendhostFlag, string agentID, string keyInDate, string type, string rowNumber)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
INSERT INTO [dbo].[Utilities_Etag_PlateNo] (RECEIVE_NUMBER, KEYIN_FLAG, PLATE_NO, SENDHOST_FLAG, AGENT_ID, KEYIN_DATE, TYPE, ROWNUMBER)
VALUES (@RECEIVE_NUMBER, @KEYIN_FLAG, @PLATE_NO, @SENDHOST_FLAG, @AGENT_ID, @KEYIN_DATE, @TYPE, @ROWNUMBER)";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@RECEIVE_NUMBER", receiveNumber));
            sqlcmd.Parameters.Add(new SqlParameter("@KEYIN_FLAG", keyInFlag));
            sqlcmd.Parameters.Add(new SqlParameter("@PLATE_NO", plateNo));
            sqlcmd.Parameters.Add(new SqlParameter("@SENDHOST_FLAG", sendhostFlag));
            sqlcmd.Parameters.Add(new SqlParameter("@AGENT_ID", agentID));
            sqlcmd.Parameters.Add(new SqlParameter("@KEYIN_DATE", keyInDate));
            sqlcmd.Parameters.Add(new SqlParameter("@TYPE", type));
            sqlcmd.Parameters.Add(new SqlParameter("@ROWNUMBER", rowNumber));

            try
            {
                result = BRUtilities_Etag.Add(sqlcmd);
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 新增Etag車牌資料
        /// </summary>
        /// <param name="receiveNumber"></param>
        /// <param name="keyInFlag"></param>
        /// <param name="plateNO"></param>
        /// <param name="sendHostFlag"></param>
        /// <param name="agentID"></param>
        /// <param name="keyInDate"></param>
        /// <returns></returns>
        public static bool InsertIntoUtilities_Etag_PlateNo(string receiveNumber, string keyInFlag, string plateNO, string sendHostFlag, 
                                                            string agentID, string keyInDate, string type)
        {
            bool addResult = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
INSERT INTO [dbo].[Utilities_Etag_PlateNo] (RECEIVE_NUMBER, KEYIN_FLAG, PLATE_NO, SENDHOST_FLAG, AGENT_ID, KEYIN_DATE, TYPE) 
VALUES (@RECEIVE_NUMBER, @KEYIN_FLAG, @PLATE_NO, @SENDHOST_FLAG, @AGENT_ID, @KEYIN_DATE, @TYPE)";

            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@RECEIVE_NUMBER", receiveNumber));
            sqlcmd.Parameters.Add(new SqlParameter("@KEYIN_FLAG", keyInFlag));
            sqlcmd.Parameters.Add(new SqlParameter("@PLATE_NO", plateNO));
            sqlcmd.Parameters.Add(new SqlParameter("@SENDHOST_FLAG", sendHostFlag));
            sqlcmd.Parameters.Add(new SqlParameter("@AGENT_ID", agentID));
            sqlcmd.Parameters.Add(new SqlParameter("@KEYIN_DATE", keyInDate));
            sqlcmd.Parameters.Add(new SqlParameter("@TYPE", type));

            try
            {
                addResult = BRUtilities_Etag.Add(sqlcmd);
            }
            catch (Exception ex)
            {
                Logging.Log("車牌號碼：" + plateNO + ex, LogState.Error, LogLayer.DB);
            }

            return addResult;
        }

        /// <summary>
        /// 刪除Etag資料
        /// </summary>
        /// <param name="receiveNumber"></param>
        /// <param name="keyInFlag"></param>
        /// <returns></returns>
        public static bool DeleteUtilities_Etag(string receiveNumber, string keyInFlag)
        {
            bool delResult = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"DELETE FROM [dbo].[Utilities_Etag] WHERE RECEIVE_NUMBER = @RECEIVE_NUMBER AND KEYIN_FLAG = @KEYIN_FLAG";

            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@RECEIVE_NUMBER", receiveNumber));
            sqlcmd.Parameters.Add(new SqlParameter("@KEYIN_FLAG", keyInFlag));

            try
            {
                delResult = BRUtilities_Etag.Add(sqlcmd);
            }
            catch (Exception ex)
            {
                Logging.Log("刪除Etag資料失敗，收件編號：" + receiveNumber + ex, LogState.Error, LogLayer.DB);
            }

            return delResult;
        }

        /// <summary>
        /// 刪除Etag車牌資料
        /// </summary>
        /// <param name="receiveNumber"></param>
        /// <param name="keyInFlag"></param>
        /// <returns></returns>
        public static bool DeleteUtilities_Etag_PlateNo(string receiveNumber, string keyInFlag)
        {
            bool delResult = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"DELETE FROM [dbo].[Utilities_Etag_PlateNo] WHERE RECEIVE_NUMBER = @RECEIVE_NUMBER AND KEYIN_FLAG = @KEYIN_FLAG";

            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@RECEIVE_NUMBER", receiveNumber));
            sqlcmd.Parameters.Add(new SqlParameter("@KEYIN_FLAG", keyInFlag));

            try
            {
                delResult = BRUtilities_Etag.Add(sqlcmd);
            }
            catch (Exception ex)
            {
                Logging.Log("刪除車牌資料失敗，收件編號：" + receiveNumber + ex, LogState.Error, LogLayer.DB);
            }

            return delResult;
        }

        /// <summary>
        /// 查詢一Key資料
        /// </summary>
        /// <param name="receiveNumber"></param>
        /// <param name="keyInFlag"></param>
        /// <returns></returns>
        public static DataTable GetUtilities_Etag(string receiveNumber, string keyInFlag)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
SELECT RECEIVE_NUMBER, PRIMARY_CARD_ID, NAME, ETAG_FLAG, TEMPLATE_PARK_FLAG, MONTHLY_PARK_FLAG, APPLY_TYPE, 
        OWNERS_ID, MONTHLY_PARKING_NO, POPUL_NO, POPUL_EMP_NO, INTRODUCER_CARD_ID, AGENT_ID, KEYIN_DATE, KEYIN_FLAG 
FROM [dbo].[Utilities_Etag] WITH(NOLOCK) 
WHERE RECEIVE_NUMBER = @RECEIVE_NUMBER AND KEYIN_FLAG = @KEYIN_FLAG";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@RECEIVE_NUMBER", receiveNumber));
            sqlcmd.Parameters.Add(new SqlParameter("@KEYIN_FLAG", keyInFlag));

            try
            {
                DataSet resultSet = BRUtilities_Etag.SearchOnDataSet(sqlcmd);
                return resultSet.Tables[0];
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return null;
        }

        public static DataTable GetUtilities_Etag(string primaryCardID, string receiveNumber, string keyInFlag)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
SELECT a.RECEIVE_NUMBER, a.PRIMARY_CARD_ID, a.NAME, a.ETAG_FLAG, a.TEMPLATE_PARK_FLAG, 
        a.MONTHLY_PARK_FLAG, a.APPLY_TYPE, a.OWNERS_ID, a.MONTHLY_PARKING_NO, a.POPUL_NO, a.POPUL_EMP_NO, 
        a.INTRODUCER_CARD_ID, a.AGENT_ID, a.KEYIN_DATE, a.KEYIN_FLAG, b.PLATE_NO, b.ROWNUMBER
FROM [dbo].[Utilities_Etag] a WITH(NOLOCK) 
INNER JOIN [dbo].[Utilities_Etag_PlateNo] b WITH(NOLOCK) ON a.RECEIVE_NUMBER = b.RECEIVE_NUMBER AND a.KEYIN_FLAG = b.KEYIN_FLAG
WHERE a.PRIMARY_CARD_ID = @PRIMARY_CARD_ID AND a.RECEIVE_NUMBER = @RECEIVE_NUMBER AND a.KEYIN_FLAG = @KEYIN_FLAG
ORDER BY b.ROWNUMBER";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@PRIMARY_CARD_ID", primaryCardID));
            sqlcmd.Parameters.Add(new SqlParameter("@RECEIVE_NUMBER", receiveNumber));
            sqlcmd.Parameters.Add(new SqlParameter("@KEYIN_FLAG", keyInFlag));

            try
            {
                DataSet resultSet = BRUtilities_Etag.SearchOnDataSet(sqlcmd);
                return resultSet.Tables[0];
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return null;
        }

        /// <summary>
        /// 查詢一Key車牌資料
        /// </summary>
        /// <param name="receiveNumber"></param>
        /// <param name="keyInFlag"></param>
        /// <returns></returns>
        public static DataTable GetUtilities_Etag_PlateNo(string receiveNumber, string keyInFlag)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
SELECT RECEIVE_NUMBER, KEYIN_FLAG, PLATE_NO, SENDHOST_FLAG, AGENT_ID, KEYIN_DATE, TYPE 
FROM [dbo].[Utilities_Etag_PlateNo] WITH(NOLOCK) 
WHERE RECEIVE_NUMBER = @RECEIVE_NUMBER AND KEYIN_FLAG = @KEYIN_FLAG";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@RECEIVE_NUMBER", receiveNumber));
            sqlcmd.Parameters.Add(new SqlParameter("@KEYIN_FLAG", keyInFlag));

            try
            {
                DataSet resultSet = BRUtilities_Etag.SearchOnDataSet(sqlcmd);
                return resultSet.Tables[0];
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return null;
        }

        /// <summary>
        /// 更新Etag資料
        /// </summary>
        /// <param name="receiveNumber"></param>
        /// <param name="primaryCardID"></param>
        /// <param name="name"></param>
        /// <param name="etagFlag"></param>
        /// <param name="templateParkFlag"></param>
        /// <param name="monthlyParkFlag"></param>
        /// <param name="applyType"></param>
        /// <param name="ownersID"></param>
        /// <param name="monthlyParkingNO"></param>
        /// <param name="populNo"></param>
        /// <param name="populEmpNo"></param>
        /// <param name="introducerCardID"></param>
        /// <param name="agentID"></param>
        /// <param name="keyInDate"></param>
        /// <param name="keyInFlag"></param>
        /// <returns></returns>
        public static bool UpdateUtilities_Etag(string receiveNumber, bool etagFlag, bool templateParkFlag, bool monthlyParkFlag, string applyType, string ownersID, 
                                                string monthlyParkingNO, string populNo, string populEmpNo, string introducerCardID, string agentID, string keyInDate, string keyInFlag)
        {
            bool addResult = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
UPDATE [dbo].[Utilities_Etag] 
SET ETAG_FLAG = @ETAG_FLAG, TEMPLATE_PARK_FLAG = @TEMPLATE_PARK_FLAG, MONTHLY_PARK_FLAG = @MONTHLY_PARK_FLAG, APPLY_TYPE = @APPLY_TYPE, 
    OWNERS_ID = @OWNERS_ID, MONTHLY_PARKING_NO = @MONTHLY_PARKING_NO, POPUL_NO = @POPUL_NO, POPUL_EMP_NO = @POPUL_EMP_NO, 
    INTRODUCER_CARD_ID = @INTRODUCER_CARD_ID, AGENT_ID = @AGENT_ID, KEYIN_DATE = @KEYIN_DATE, NAKEYIN_FLAGME = @KEYIN_FLAG 
WHERE RECEIVE_NUMBER = @RECEIVE_NUMBER";

            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@RECEIVE_NUMBER", receiveNumber));
            sqlcmd.Parameters.Add(new SqlParameter("@ETAG_FLAG", etagFlag));
            sqlcmd.Parameters.Add(new SqlParameter("@TEMPLATE_PARK_FLAG", templateParkFlag));
            sqlcmd.Parameters.Add(new SqlParameter("@MONTHLY_PARK_FLAG", monthlyParkFlag));
            sqlcmd.Parameters.Add(new SqlParameter("@APPLY_TYPE", applyType));
            sqlcmd.Parameters.Add(new SqlParameter("@OWNERS_ID", ownersID));
            sqlcmd.Parameters.Add(new SqlParameter("@MONTHLY_PARKING_NO", monthlyParkingNO));
            sqlcmd.Parameters.Add(new SqlParameter("@POPUL_NO", populNo));
            sqlcmd.Parameters.Add(new SqlParameter("@POPUL_EMP_NO", populEmpNo));
            sqlcmd.Parameters.Add(new SqlParameter("@INTRODUCER_CARD_ID", introducerCardID));
            sqlcmd.Parameters.Add(new SqlParameter("@AGENT_ID", agentID));
            sqlcmd.Parameters.Add(new SqlParameter("@KEYIN_DATE", keyInDate));
            sqlcmd.Parameters.Add(new SqlParameter("@KEYIN_FLAG", keyInFlag));

            try
            {
                addResult = BRUtilities_Etag.Add(sqlcmd);
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return addResult;
        }

        /// <summary>
        /// 刪除Etag車牌資料
        /// </summary>
        /// <param name="receiveNumber"></param>
        /// <param name="keyInFlag"></param>
        /// <returns></returns>
        public static bool DeleteUtilities_Etag_PlateNo(string receiveNumber, string plateNO, string sendHostFlag, string keyInFlag)
        {
            bool delResult = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
DELETE FROM [dbo].[Utilities_Etag_PlateNo] 
WHERE RECEIVE_NUMBER = @RECEIVE_NUMBER AND PLATE_NO = @PLATE_NO, SENDHOST_FLAG = @SENDHOST_FLAG, KEYIN_FLAG = @KEYIN_FLAG";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@RECEIVE_NUMBER", receiveNumber));
            sqlcmd.Parameters.Add(new SqlParameter("@KEYIN_FLAG", keyInFlag));

            try
            {
                delResult = BRUtilities_Etag.Add(sqlcmd);
            }
            catch (Exception ex)
            {
                Logging.Log("刪除車牌資料失敗，收件編號：" + receiveNumber + ex, LogState.Error, LogLayer.DB);
            }

            return delResult;
        }



        /// <summary>
        /// 修改Etag車牌資料
        /// </summary>
        /// <param name="receiveNumber"></param>
        /// <param name="keyInFlag"></param>
        /// <returns></returns>
        public static bool UpdateUtilities_Etag_PlateNo(string receiveNumber, string plateNO, string sendHostFlag, string keyInFlag)
        {
            bool delResult = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
Update [dbo].[Utilities_Etag_PlateNo] set SENDHOST_FLAG=@SENDHOST_FLAG
WHERE RECEIVE_NUMBER = @RECEIVE_NUMBER AND PLATE_NO = @PLATE_NO AND KEYIN_FLAG = @KEYIN_FLAG";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@RECEIVE_NUMBER", receiveNumber));
            sqlcmd.Parameters.Add(new SqlParameter("@PLATE_NO", plateNO));
            sqlcmd.Parameters.Add(new SqlParameter("@SENDHOST_FLAG", sendHostFlag));
            sqlcmd.Parameters.Add(new SqlParameter("@KEYIN_FLAG", keyInFlag));

            try
            {
                delResult = BRUtilities_Etag.Add(sqlcmd);
            }
            catch (Exception ex)
            {
                Logging.Log("修改車牌資料失敗，收件編號：" + receiveNumber + ex, LogState.Error, LogLayer.DB);
            }

            return delResult;
        }


    }
}
