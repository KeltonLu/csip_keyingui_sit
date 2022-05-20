//******************************************************************
//*  作    者：蘇洺葳
//*  功能說明：郵局自扣建檔
//*  創建日期：2018/10/22
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//* 
//*******************************************************************
using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Text;
//using CSIPCommonModel.BaseItem;
using CSIPCommonModel.BusinessRules;
using CSIPCommonModel.EntityLayer_new;
using CSIPKeyInGUI.BusinessRules;
using Framework.Data;
using Framework.Common;
using Framework.Common.Message;
using Framework.Common.Logging;
using Framework.Common.Utility;

namespace CSIPKeyInGUI.BusinessRules_new
{
    public class BRPostOffice_Temp : BRBase<EntityPostOffice_Temp>
    {
        /// <summary>
        /// 查詢已傳送郵局資料
        /// </summary>
        /// <param name="receiveNumber"></param>
        /// <returns></returns>
        public static DataTable GetPostOffice(string receiveNumber)
        {
            string sql = @"
SELECT ID, CusID, ReceiveNumber, IsSendToPost 
FROM dbo.PostOffice_Temp 
WHERE ReceiveNumber = @ReceiveNumber AND IsSendToPost = '1'";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            SqlParameter parmQuery_Num = new SqlParameter("@ReceiveNumber", receiveNumber);
            sqlcmd.Parameters.Add(parmQuery_Num);

            try
            {
                DataTable dt = BRPostOffice_Temp.SearchOnDataSet(sqlcmd).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                Logging.Log("查詢已傳送郵局資料失敗：" + ex, LogState.Error, LogLayer.BusinessRule);
                return null;
            }
        }

        /// <summary>
        /// 查詢郵局資料
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="receiveNumber"></param>
        /// <returns></returns>
        public static DataTable GetPostOffice(string userID, string receiveNumber)
        {
            string sql = @"
SELECT ID, CusID, ReceiveNumber, CusName, AccNoBank, AccNo, AccID, ApplyCode, AccType, AccDeposit, CusNo, AgentID, ModDate, IsNeedUpload, 
       UploadDate, ReturnStatusTypeCode, ReturnCheckFlagCode, ReturnDate, IsSendToPost, SendHostResult, SendHostResultCode 
FROM dbo.PostOffice_Temp 
WHERE CusID = @CusID AND ReceiveNumber = @ReceiveNumber";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            SqlParameter parmQuery_Id = new SqlParameter("@CusID", userID);
            sqlcmd.Parameters.Add(parmQuery_Id);
            SqlParameter parmQuery_Num = new SqlParameter("@ReceiveNumber", receiveNumber);
            sqlcmd.Parameters.Add(parmQuery_Num);
            
            try
            {
                DataTable dt = BRPostOffice_Temp.SearchOnDataSet(sqlcmd).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                Logging.Log("查詢郵局自扣資料失敗：" + ex, LogState.Error, LogLayer.BusinessRule);
                return null;
            }
        }

        /// <summary>
        /// 新增郵局自扣資料
        /// </summary>
        /// <param name="cusID">身分證號碼或統一證號</param>
        /// <param name="receiveNumber">收件編號</param>
        /// <param name="cusName">姓名</param>
        /// <param name="accNoBank">扣繳帳號(銀行代號)</param>
        /// <param name="accNo">銀行帳號</param>
        /// <param name="accID">帳戶ID</param>
        /// <param name="applyCode">申請代號(委託機構送件：1.申請 2.終止；郵局回送「帳戶至郵局辦理終止」檔：3.郵局終止 4.誤終止-已回復為申請)</param>
        /// <param name="accType">帳戶別(P.存簿 G.劃撥)</param>
        /// <param name="accDeposit">儲金帳號(存簿：局帳號計14碼；劃撥：000000+8碼帳號)</param>
        /// <param name="cusNo">用戶編號</param>
        /// <param name="agentID">鍵檔人員</param>
        /// <param name="modDate">鍵檔日期</param>
        /// <returns></returns>
        public static bool InsertIntoPostOffice(string cusID, string receiveNumber, string cusName, string accNoBank, string accNo, string accID, string applyCode, 
                                                string accType, string accDeposit, string cusNo, string agentID, string modDate)
        {
            bool addResult = false;
            string uploadDate = GetUploadDate(DateTime.Now);

            try
            {
                string sql = @"
INSERT INTO dbo.PostOffice_Temp (CusID, ReceiveNumber, CusName, AccNoBank, AccNo, AccID, ApplyCode, AccType, AccDeposit, CusNo, AgentID, ModDate, IsNeedUpload, 
                                 UploadDate, ReturnStatusTypeCode, ReturnCheckFlagCode, ReturnDate, IsSendToPost, SendHostResult, SendHostResultCode) 
VALUES (@CusID, @ReceiveNumber, @CusName, @AccNoBank, @AccNo, @AccID, @ApplyCode, @AccType, @AccDeposit, @CusNo, @AgentID, @ModDate, 1, @UploadDate, '', '', '', '0', '', '')";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                sqlcmd.Parameters.Add(new SqlParameter("@CusID", cusID));                // 身分證號碼或統一證號
                sqlcmd.Parameters.Add(new SqlParameter("@ReceiveNumber", receiveNumber));// 收件編號
                sqlcmd.Parameters.Add(new SqlParameter("@CusName", cusName));            // 姓名
                sqlcmd.Parameters.Add(new SqlParameter("@AccNoBank", accNoBank));        // 郵局代號
                sqlcmd.Parameters.Add(new SqlParameter("@AccNo", accNo));                // 郵局帳號
                sqlcmd.Parameters.Add(new SqlParameter("@AccID", accID));                // 帳戶ID
                sqlcmd.Parameters.Add(new SqlParameter("@ApplyCode", applyCode));        // 申請代號(委託機構送件：1.申請 2.終止；郵局回送「帳戶至郵局辦理終止」檔：3.郵局終止 4.誤終止-已回復為申請)
                sqlcmd.Parameters.Add(new SqlParameter("@AccType", accType));            // 帳戶別(P.存簿 G.劃撥)
                sqlcmd.Parameters.Add(new SqlParameter("@AccDeposit", accDeposit));      // 儲金帳號(存簿：局帳號計14碼；劃撥：000000+8碼帳號)
                sqlcmd.Parameters.Add(new SqlParameter("@CusNo", cusNo));                // 用戶編號
                sqlcmd.Parameters.Add(new SqlParameter("@AgentID", agentID));            // 鍵檔人員
                sqlcmd.Parameters.Add(new SqlParameter("@ModDate", modDate));            // 鍵檔日期
                sqlcmd.Parameters.Add(new SqlParameter("@UploadDate", uploadDate));      // 傳送日期
                addResult = BRPostOffice_Temp.Add(sqlcmd);

                return addResult;
            }
            catch (Exception ex)
            {
                Logging.Log("收件編號 " + receiveNumber + " 郵局自扣資料新增失敗：" + ex, LogState.Error, LogLayer.BusinessRule);
                return addResult;
            }
        }

        /// <summary>
        /// 更新郵局自扣資料
        /// </summary>
        /// <param name="cusID"></param>
        /// <param name="receiveNumber"></param>
        /// <param name="cusName"></param>
        /// <param name="accNoBank"></param>
        /// <param name="accNo"></param>
        /// <param name="accID"></param>
        /// <param name="accType"></param>
        /// <param name="accDeposit"></param>
        /// <param name="cusNo"></param>
        /// <param name="agentID"></param>
        /// <param name="modDate"></param>
        /// <returns></returns>
        public static bool UpdatePostOffice(string cusID, string receiveNumber, string cusName, string accNoBank, string accNo, string accID, 
                                            string applyCode, string accType, string accDeposit, string cusNo, string agentID, string modDate)
        {
            bool addResult = false;

            try
            {
                string sql = @"
UPDATE dbo.PostOffice_Temp 
SET CusName = @CusName , AccNoBank = @AccNoBank, AccNo = @AccNo, AccID = @AccID, ApplyCode = @ApplyCode, 
    AccType = @AccType, AccDeposit = @AccDeposit, CusNo = @CusNo, AgentID = @AgentID, ModDate = @ModDate 
WHERE CusID = @CusID AND ReceiveNumber = @ReceiveNumber";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                sqlcmd.Parameters.Add(new SqlParameter("@CusID", cusID));                // 身分證號碼或統一證號
                sqlcmd.Parameters.Add(new SqlParameter("@ReceiveNumber", receiveNumber));// 收件編號
                sqlcmd.Parameters.Add(new SqlParameter("@CusName", cusName));            // 姓名
                sqlcmd.Parameters.Add(new SqlParameter("@AccNoBank", accNoBank));        // 郵局代號
                sqlcmd.Parameters.Add(new SqlParameter("@AccNo", accNo));                // 郵局帳號
                sqlcmd.Parameters.Add(new SqlParameter("@AccID", accID));                // 帳戶ID
                sqlcmd.Parameters.Add(new SqlParameter("@ApplyCode", applyCode));        // 申請代號(委託機構送件：1.申請 2.終止)
                sqlcmd.Parameters.Add(new SqlParameter("@AccType", accType));            // 帳戶別(P.存簿 G.劃撥)
                sqlcmd.Parameters.Add(new SqlParameter("@AccDeposit", accDeposit));      // 儲金帳號(存簿：局帳號計14碼；劃撥：000000+8碼帳號)
                sqlcmd.Parameters.Add(new SqlParameter("@CusNo", cusNo));                // 用戶編號
                sqlcmd.Parameters.Add(new SqlParameter("@AgentID", agentID));            // 鍵檔人員
                sqlcmd.Parameters.Add(new SqlParameter("@ModDate", modDate));            // 鍵檔日期
                addResult = BRPostOffice_Temp.Add(sqlcmd);

                return addResult;
            }
            catch (Exception ex)
            {
                Logging.Log("收件編號 " + receiveNumber + " 郵局自扣資料新增失敗：" + ex, LogState.Error, LogLayer.BusinessRule);
                return addResult;
            }
        }

        /// <summary>
        /// 查詢郵局授權扣款資料清單記錄
        /// </summary>
        /// <param name="uploadDate">拋檔日期</param>
        /// <param name="intPageIndex">當前頁號</param>
        /// <param name="intPageSize">每頁顯示記錄條數</param>
        /// <param name="intTotolCount">記錄總條數</param>
        /// <returns>DataSet</returns>
        public static DataSet SearchACHRecord(string uploadDate, int intPageIndex, int intPageSize, ref int intTotolCount)
        {
            string sql = @"
SELECT a.ReceiveNumber,													 -- 收件編號
       a.AccNoBank,														 -- 收受行(核印行)
       a.AccID,															 -- 委繳戶統編\身分證字號
       a.AccNo,															 -- 委繳戶帳號
	   CASE WHEN a.ApplyCode = 1 THEN 'A' ELSE 'D' END ApplyType,		 -- 申請類別
	   CASE WHEN a.IsSendToPost = '1' THEN 'Y' ELSE '' END IsSendToPost, -- 處理註記
       a.ReturnDate,													 -- 回覆日期
       CASE WHEN f.Complete = '0' OR f.Complete IS NULL THEN '' 
			WHEN a.ReturnStatusTypeCode != '' THEN b.PostRtnMsg
			WHEN a.ReturnCheckFlagCode != '' THEN c.PostRtnMsg
	   ELSE d.PostRtnMsg  END PostRtnMsg,								 -- 郵局回覆碼
       CASE WHEN a.ReturnDate != '' AND f.StatusType = '' 
					AND f.CheckFlag = '' THEN 'Y' 
			ELSE 'N' END IsUpload										 -- 生效碼【Y/N】
FROM [dbo].[PostOffice_Temp] a WITH(NOLOCK)
LEFT JOIN [dbo].[PostOffice_Rtn_Info] b WITH(NOLOCK) ON a.ReturnStatusTypeCode = b.PostRtnCode AND b.RtnType = '1' 
LEFT JOIN [dbo].[PostOffice_Rtn_Info] c WITH(NOLOCK) ON a.ReturnCheckFlagCode = c.PostRtnCode AND c.RtnType = '2'
LEFT JOIN [dbo].[PostOffice_Rtn_Info] d WITH(NOLOCK) ON d.RtnType = '3'
LEFT JOIN [dbo].[PostOffice_Rtn_Info] e WITH(NOLOCK) ON e.RtnType = '4' 
LEFT JOIN [dbo].[PostOffice_Detail] f WITH(NOLOCK) ON a.ReceiveNumber = f.ReceiveNumber
WHERE UploadDate = @UploadDate
ORDER BY a.ApplyCode DESC, a.ReceiveNumber";

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            sqlcmd.Parameters.Add(new SqlParameter("@UploadDate", uploadDate));

            return BRPostOffice_Temp.SearchOnDataSet(sqlcmd, intPageIndex, intPageSize, ref intTotolCount);
        }

        /// <summary>
        /// 查詢R02授權成功/失敗報表
        /// </summary>
        /// <param name="uploadDate">拋檔日期</param>
        /// <param name="sReplyDate"></param>
        /// <param name="eReplyDate"></param>
        /// <param name="reportType">成功/失敗(S.成功 F.失敗 A.成功+失敗)</param>
        /// <param name="postRtnMsg"></param>
        /// <param name="intPageIndex">當前頁號</param>
        /// <param name="intPageSize">每頁顯示記錄條數</param>
        /// <param name="intTotolCount">記錄總條數</param>
        /// <returns>DataSet</returns>
        public static DataSet SearchR02Record(string uploadDate, string sReplyDate, string eReplyDate, string reportType, string postRtnMsg, int intPageIndex, int intPageSize, ref int intTotolCount)
        {
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            string sql = string.Format(@"
SELECT a.ReceiveNumber,												-- 收件編號
       a.AccNoBank,													-- 收受行(核印行)
       b.PROPERTY_NAME,												-- 收受行名稱(核印行)
       a.AccID,														-- 委繳戶統編\身分證字號
       a.AccNo,														-- 委繳戶帳號
       a.CusID,														-- 持卡人ID
       CASE WHEN a.ApplyCode = 1 THEN 'A' ELSE 'D' END ApplyType,	-- 申請類別
       CASE WHEN a.ReturnStatusTypeCode = '' 
				AND a.ReturnCheckFlagCode = '' THEN '成功' 
		ELSE '失敗' END ReturnStatus,								-- 成功/失敗
       CASE WHEN a.ReturnStatusTypeCode = '' 
				AND a.ReturnCheckFlagCode = '' THEN ''
			WHEN a.ReturnStatusTypeCode != '' THEN a.ReturnStatusTypeCode + ':' + c.PostRtnMsg
			WHEN a.ReturnCheckFlagCode != '' THEN a.ReturnCheckFlagCode + ':' + d.PostRtnMsg
		 END PostRtnMsg												-- 回覆訊息
FROM [dbo].[PostOffice_Temp] a WITH(NOLOCK) 
LEFT JOIN [{0}].[dbo].[m_property_code] b WITH(NOLOCK) ON b.FUNCTION_KEY = '01' AND b.PROPERTY_KEY = '50' 
LEFT JOIN [dbo].[PostOffice_Rtn_Info] c WITH(NOLOCK) ON a.ReturnStatusTypeCode = c.PostRtnCode AND c.RtnType = '1' 
LEFT JOIN [dbo].[PostOffice_Rtn_Info] d WITH(NOLOCK) ON a.ReturnCheckFlagCode = d.PostRtnCode AND d.RtnType = '2' 
INNER JOIN [dbo].[PostOffice_Detail] e WITH(NOLOCK) ON a.ReceiveNumber = e.ReceiveNumber AND e.Complete = '1'", UtilHelper.GetAppSettings("DB_CSIP"));

            string sqlWhere = " WHERE a.UploadDate = @UploadDate ";

            if (sReplyDate != "")
            {
                sqlWhere = " WHERE a.ReturnDate >= @sReplyDate AND a.ReturnDate <= @eReplyDate ";
            }

            if (postRtnMsg.Length == 1)
            {
                sqlWhere = sqlWhere + " AND ReturnCheckFlagCode = @postRtnMsg ";
            }

            if (postRtnMsg.Length == 2)
            {
                sqlWhere = sqlWhere + " AND ReturnStatusTypeCode = @postRtnMsg ";
            }

            // 成功/失敗條件
            string condition = (reportType == "S") ? "AND a.ReturnStatusTypeCode = '' AND a.ReturnCheckFlagCode = ''" : ((reportType == "F") ? "AND (a.ReturnStatusTypeCode != '' OR a.ReturnCheckFlagCode != '')" : "");

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql + sqlWhere + condition;

            if (sReplyDate == "")
            {
                sqlcmd.Parameters.Add(new SqlParameter("@UploadDate", uploadDate));
            }
            else
            {
                sqlcmd.Parameters.Add(new SqlParameter("@sReplyDate", sReplyDate));
                sqlcmd.Parameters.Add(new SqlParameter("@eReplyDate", eReplyDate));
            }

            if (postRtnMsg.Length > 0)
            {
                sqlcmd.Parameters.Add(new SqlParameter("@postRtnMsg", postRtnMsg));
            }

            return BRPostOffice_Temp.SearchOnDataSet(sqlcmd, intPageIndex, intPageSize, ref intTotolCount);
        }

        /// <summary>
        /// 傳送日期
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private static string GetUploadDate(DateTime dateTime)
        {
            int day = (int)dateTime.DayOfWeek;
            int addDay = 0;
            DateTime operatorTime;

            switch (day)
            {
                case 5:
                    addDay = 0;
                    break;
                case 6:
                    addDay = 6;
                    break;
                case 0:
                    addDay = 5;
                    break;
                case 1:
                    addDay = 4;
                    break;
                case 2:
                    addDay = 3;
                    break;
                case 3:
                    addDay = 2;
                    break;
                case 4:
                    addDay = 1;
                    break;
            }

            operatorTime = dateTime.AddDays(addDay);
            return operatorTime.ToString("yyyyMMdd");
        }

        /// <summary>
        /// 查詢要刪除的郵局自扣資料
        /// </summary>
        /// <param name="cusID">身分證號碼</param>
        /// <param name="receiveNumber">收件編號</param>
        /// <returns></returns>
        public static DataTable GetPostOfficeDeleteItem(string cusID, string receiveNumber)
        {
            // IsSendToPost = '1' 已傳送郵局，不能刪除
            string sql = @"
SELECT a.AccNo,													             -- 郵局帳號
	   CASE WHEN a.AccDeposit = 'P' THEN '存簿' ELSE '劃撥' END AccDeposit, --儲金帳號
       a.AccType,													         -- 帳戶別
       g.Pay_Way,													         -- 扣款方式
       g.Acc_ID,													         -- 扣款帳號所有人之ID
       g.bcycle_code,												 	     -- 帳單週期
       g.Mobile_Phone,											 		     -- 行動電話
       g.E_Mail,											 		         -- E-MAIL
       g.E_Bill,													         -- 電子帳單
       a.ModDate,													         -- 鍵檔日期
	   CASE WHEN a.ApplyCode = 1 THEN 'A' ELSE 'D' END ApplyType,		     -- 申請類別
	   CASE WHEN a.IsSendToPost = '1' THEN 'Y' ELSE '' END IsSendToPost,     -- 處理註記
       a.ReturnDate,													     -- 回覆日期
       CASE WHEN f.Complete = '0' OR f.Complete IS NULL THEN '' 
            WHEN a.ReturnStatusTypeCode != '' THEN b.PostRtnMsg
            WHEN a.ReturnCheckFlagCode != '' THEN c.PostRtnMsg
	   ELSE d.PostRtnMsg  END PostRtnMsg,                                    -- 郵局回覆碼
       CASE WHEN a.ReturnDate != '' AND f.StatusType = '' 
	        AND f.CheckFlag = '' THEN 'Y' 
            ELSE 'N' END IsUpload                                            -- 生效碼【Y/N】
FROM [dbo].[PostOffice_Temp] a WITH(NOLOCK)
LEFT JOIN [dbo].[PostOffice_Rtn_Info] b WITH(NOLOCK) ON a.ReturnStatusTypeCode = b.PostRtnCode AND b.RtnType = '1' 
LEFT JOIN [dbo].[PostOffice_Rtn_Info] c WITH(NOLOCK) ON a.ReturnCheckFlagCode = c.PostRtnCode AND c.RtnType = '2'
LEFT JOIN [dbo].[PostOffice_Rtn_Info] d WITH(NOLOCK) ON d.RtnType = '3'
LEFT JOIN [dbo].[PostOffice_Rtn_Info] e WITH(NOLOCK) ON e.RtnType = '4' 
LEFT JOIN [dbo].[PostOffice_Detail] f WITH(NOLOCK) ON a.ReceiveNumber = f.ReceiveNumber 
LEFT JOIN [dbo].[Auto_Pay] g WITH(NOLOCK) ON a.ReceiveNumber = g.Receive_Number AND a.CusID = g.Cus_ID 
WHERE a.ReceiveNumber = @ReceiveNumber AND a.CusID = @CusID AND a.IsSendToPost != '1' AND g.KeyIn_Flag = '2'";

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            sqlcmd.Parameters.Add(new SqlParameter("@CusID", cusID));
            sqlcmd.Parameters.Add(new SqlParameter("@ReceiveNumber", receiveNumber));

            return BRPostOffice_Temp.SearchOnDataSet(sqlcmd).Tables[0];
        }

        /// <summary>
        /// 刪除郵局自扣資料
        /// </summary>
        /// <param name="cusID"></param>
        /// <param name="receiveNumber"></param>
        /// <returns>true成功，false失敗</returns>
        public static bool DeletePostOfficeItem(string cusID, string receiveNumber)
        {
            string sql = @"DELETE FROM dbo.PostOffice_Temp WHERE ReceiveNumber = @ReceiveNumber AND CusID = @CusID ";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            sqlcmd.Parameters.Add(new SqlParameter("@CusID", cusID));
            sqlcmd.Parameters.Add(new SqlParameter("@ReceiveNumber", receiveNumber));

            return BRPostOffice_Temp.Delete(sqlcmd);
        }

        /// <summary>
        /// 更新SendHostResultCode
        /// </summary>
        /// <param name="cusID"></param>
        /// <param name="receiveNumber"></param>
        /// <param name="sendHostResultCode"></param>
        /// <returns></returns>
        public static bool UpdateSendHostResultCode(string cusID, string receiveNumber, string sendHostResultCode)
        {
            bool addResult = false;

            try
            {
                string sql = @"UPDATE dbo.PostOffice_Temp SET SendHostResultCode = @SendHostResultCode WHERE CusID = @CusID AND ReceiveNumber = @ReceiveNumber";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                sqlcmd.Parameters.Add(new SqlParameter("@CusID", cusID));                          // 身分證號碼或統一證號
                sqlcmd.Parameters.Add(new SqlParameter("@ReceiveNumber", receiveNumber));          // 收件編號
                sqlcmd.Parameters.Add(new SqlParameter("@SendHostResultCode", sendHostResultCode));// 主機回傳結果代碼
                addResult = BRPostOffice_Temp.Add(sqlcmd);

                return addResult;
            }
            catch (Exception ex)
            {
                Logging.Log(receiveNumber + "更新SendHostResultCode失敗：" + ex, LogState.Error, LogLayer.BusinessRule);
                return addResult;
            }
        }

        /// <summary>
        /// 取得郵局自扣資料
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static DataTable GetPostOfficeByCusID(string userID)
        {
            string sql = @"
SELECT ID, CusID, ReceiveNumber, CusName, AccNoBank, AccNo, AccID, ApplyCode, AccType, AccDeposit, CusNo, AgentID, ModDate, IsNeedUpload, 
       UploadDate, ReturnStatusTypeCode, ReturnCheckFlagCode, ReturnDate, IsSendToPost, SendHostResult, SendHostResultCode 
FROM dbo.PostOffice_Temp 
WHERE CusID = @CusID";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sql;
            SqlParameter parmQuery_Id = new SqlParameter("@CusID", userID);
            sqlcmd.Parameters.Add(parmQuery_Id);

            try
            {
                DataTable dt = BRPostOffice_Temp.SearchOnDataSet(sqlcmd).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                Logging.Log("查詢郵局自扣資料失敗：" + ex, LogState.Error, LogLayer.BusinessRule);
                return null;
            }
        }

        /// <summary>
        /// 更新自扣申請代號(委託機構送件：1.申請 2.終止)
        /// </summary>
        /// <param name="cusID"></param>
        /// <param name="applyType"></param>
        /// <returns></returns>
        public static bool UpdateApplyType(string cusID, string applyCode)
        {
            bool addResult = false;

            try
            {
                string sql = @"UPDATE dbo.PostOffice_Temp SET ApplyCode = @ApplyCode WHERE CusID = @CusID";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                sqlcmd.Parameters.Add(new SqlParameter("@CusID", cusID));        // 身分證號碼或統一證號
                sqlcmd.Parameters.Add(new SqlParameter("@ApplyCode", applyCode));// 申請代號
                addResult = BRPostOffice_Temp.Add(sqlcmd);

                return addResult;
            }
            catch (Exception ex)
            {
                Logging.Log(cusID + "更新ApplyType失敗：" + ex, LogState.Error, LogLayer.BusinessRule);
                return addResult;
            }
        }
    }
}
