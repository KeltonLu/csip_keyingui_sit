//******************************************************************
//*  作    者：蘇洺葳
//*  功能說明：表單欄位
//*  創建日期：2018/02/08
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*Ares Luke          2021/02/22         20200031-CSIP EOS       調整connectionStr方式
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM;
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.EntityLayer;
using System.Data;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using System.Configuration;
using System.Data.SqlClient;
using Framework.Common;
using Framework.Common.Message;
using Framework.Common.Logging;
using Framework.Common.Utility;

namespace CSIPCommonModel.BusinessRules
{
    public class BRFORM_COLUMN : BRBase<EntityFORM_COLUMN>
    {
        #region sql語句

        private const string SEL_FORM_COLUMN = @"SELECT * FROM [FORM_COLUMN] WHERE PROPERTY_CODE LIKE @PROPERTY_CODE 
                                                ORDER BY SEQUENCE ";

        #endregion

        /// <summary>
        /// 查詢業務類別下的表單欄位
        /// </summary>
        /// <param name="strPropertyCode">業務類別編號</param>
        /// <param name="dtblResult">表單欄位DataTable</param>
        /// <returns></returns>
        public static bool GetProperty(string strPropertyCode, ref DataTable dtblResult)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = SEL_FORM_COLUMN;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@PROPERTY_CODE", strPropertyCode));

            try
            {
                dtblResult = SearchOnDataSet(sqlcmd, "Connection_KeyInGUI").Tables[0];
                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
                return false;
            }
        }

        /// <summary>
        /// 查詢業務類別下的表單欄位
        /// </summary>
        /// <param name="strPropertyCode">業務類別編號</param>
        /// <returns>表單欄位EntitySet</returns>
        public static EntitySet<EntityFORM_COLUMN> SelectEntitySet(string strPropertyCode)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityFORM_COLUMN.M_PROPERTY_CODE, Operator.Like, DataTypeUtils.String, strPropertyCode);

                string strSql = sSql.GetFilterCondition();
                return BRFORM_COLUMN.Search(strSql);
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
                throw ex;
            }
        }

        /// <summary>
        /// 查詢所有表單欄位
        /// </summary>
        /// <returns></returns>
        public static EntitySet<EntityFORM_COLUMN> SelectEntitySet()
        {
            try
            {
                SqlHelper sSql = new SqlHelper();

                string strSql = sSql.GetFilterCondition();
                return BRFORM_COLUMN.Search(strSql);
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
                throw ex;
            }
        }

        /// <summary>
        /// 新增自訂參數
        /// </summary>
        /// <param name="eFormColumn"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool Add(EntityFORM_COLUMN eFormColumn, ref string strMsgID)
        {
            try
            {
                if (!IsRepeat(eFormColumn))
                {
                    if (BRFORM_COLUMN.AddNewEntity(eFormColumn))
                    {
                        strMsgID = "01_05020000_004";
                        return true;
                    }
                    else
                    {
                        strMsgID = "01_05020000_005";
                        return false;
                    }
                }

                strMsgID = "01_05020000_003";
                return false;
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
                throw ex;
            }
        }

        /// <summary>
        /// 修改自訂參數
        /// </summary>
        /// <param name="eFormColumn"></param>
        /// <param name="elementCode"></param>
        /// <returns></returns>
        public static bool Update(EntityFORM_COLUMN eFormColumn, string elementCode)
        {
            try
            {
                SqlHelper Sql = new SqlHelper();
                Sql.AddCondition(EntityFORM_COLUMN.M_ELEMENT_CODE, Operator.Equal, DataTypeUtils.String, elementCode);

                return eFormColumn.DB_UpdateEntity(Sql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
                throw ex;
            }
        }

        /// <summary>
        /// 判斷控制項ID是否重復
        /// </summary>
        /// <param name="eFormColumn"></param>
        /// <returns>Repeat true, unrepeat false</returns>
        public static bool IsRepeat(string elementID)
        {
            SqlHelper sql = new SqlHelper();
            sql.AddCondition(EntityFORM_COLUMN.M_ELEMENT_ID, Operator.Equal, DataTypeUtils.String, elementID);

            if (BRFORM_COLUMN.Search(sql.GetFilterCondition()).Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 判斷是否重復
        /// </summary>
        /// <param name="eFormColumn"></param>
        /// <returns>Repeat true, unrepeat false</returns>
        public static bool IsRepeat(EntityFORM_COLUMN eFormColumn)
        {
            SqlHelper sql = new SqlHelper();
            sql.AddCondition(EntityFORM_COLUMN.M_ELEMENT_CODE, Operator.Equal, DataTypeUtils.String, eFormColumn.ELEMENT_CODE);

            if (BRFORM_COLUMN.Search(sql.GetFilterCondition()).Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 取要上傳郵局核印資料
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static DataTable GetSendToPostOfficeData(string endTime)
        {
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
SELECT ReceiveNumber, AccNo, CusID, AccID, CusName, AccNoBank, ApplyCode FROM [dbo].[PostOffice_Temp] WITH(NOLOCK)
WHERE IsNeedUpload = '1' AND UploadDate = @UploadDate
ORDER BY AccNo";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@UploadDate", endTime));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 判斷是否已有檔案
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsFileExist(string fileName)
        {
            bool isFileExist = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
SELECT COUNT(0) FROM [dbo].[PostOffice_Master] WITH(NOLOCK)
WHERE FileName = @FileName";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@FileName", fileName));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    int masterID = Convert.ToInt32(resultSet.Tables[0].Rows[0][0].ToString());
                    if (masterID > 0)
                    {
                        isFileExist = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return isFileExist;
        }

        /// <summary>
        /// PostMaster insert
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int InsertPostMaster(string fileName, DateTime dateTime, bool complete, string note)
        {
            int masterID = 0;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
INSERT INTO dbo.PostOffice_Master (FileName, FileDate, Complete, Note)
    VALUES (@FileName, @FileDate, @Complete, @Note)
SELECT SCOPE_IDENTITY()";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@FileName", fileName));
            sqlcmd.Parameters.Add(new SqlParameter("@FileDate", dateTime.ToString("yyyy-MM-dd")));
            sqlcmd.Parameters.Add(new SqlParameter("@Complete", complete));
            sqlcmd.Parameters.Add(new SqlParameter("@Note", note));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    masterID = Convert.ToInt32(resultSet.Tables[0].Rows[0][0].ToString());
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return masterID;
        }

        /// <summary>
        /// 大量資料匯入
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dtInvData"></param>
        /// <returns></returns>
        public static bool InsertToInvData(string tableName, DataTable dataTable)
        {
            bool result = false;
            // string connnection = System.Configuration.ConfigurationManager.ConnectionStrings["Connection_System"].ConnectionString;
            string connnection = UtilHelper.GetConnectionStrings("Connection_System");
            SqlConnection conn = new SqlConnection(connnection);
            SqlBulkCopy sbc = new SqlBulkCopy(connnection);
            sbc.DestinationTableName = tableName;

            try
            {
                conn.Open();
                sbc.WriteToServer(dataTable);
                dataTable.Clear();

                result = true;
                return result;
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.DB);
                return result;
            }
            finally
            {
                dataTable.Clear();
                dataTable.Dispose();
                sbc.Close();
                conn.Close();
                conn.Dispose();
            }
        }

        /// <summary>
        ///  PostTrailer insert
        /// </summary>
        /// <param name="postTrailerData"></param>
        /// <returns></returns>
        public static bool InsertPostTrailer(DataTable postTrailerData)
        {
            bool result = false;
            DataRow row = postTrailerData.Rows[0];
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
INSERT INTO dbo.PostOffice_Trailer (MasterID, DataType, AgencyCode, Filler1, FileCreateDate, BatchNo, CreateType, TotalCount, PostCreateDate, FailCount, SuccessCount, Filler2)
	VALUES (@MasterID, @DataType, @AgencyCode, @Filler1, @FileCreateDate, @BatchNo, @CreateType, @TotalCount, @PostCreateDate, @FailCount, @SuccessCount, @Filler2)";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@MasterID", row["MasterID"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@DataType", row["DataType"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@AgencyCode", row["AgencyCode"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@Filler1", row["Filler1"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@FileCreateDate", row["FileCreateDate"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@BatchNo", row["BatchNo"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@CreateType", row["CreateType"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@TotalCount", row["TotalCount"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@PostCreateDate", row["PostCreateDate"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@FailCount", row["FailCount"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@SuccessCount", row["SuccessCount"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@Filler2", row["Filler2"].ToString()));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// AutoPayStatus insert
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static bool InsertAutoPayStatus(string cusID, string acc_No)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
INSERT INTO [dbo].[Auto_Pay_Status] (Receive_Number, Cus_ID, Cus_Name, AccNoBank, Acc_No, Pay_Way, IsUpdateByTXT, IsCTCB, DateTime, Acc_No_O)
SELECT ReceiveNumber, @CusID, CusName, '701' , '701-' + @Acc_No, '0' , 'N', 'Y', GetDate(), ''  FROM [dbo].[PostOffice_Temp] WITH(NOLOCK) WHERE CusID = @CusID AND AccNo = @Acc_No";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@CusID", cusID));
            sqlcmd.Parameters.Add(new SqlParameter("@Acc_No", acc_No));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }


        /// <summary>
        /// 更新 PostOffice_Temp.IsSendToPost 狀態
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static bool UpdatePostOfficeTempIsSendToPost(DataRow row)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
UPDATE [dbo].[PostOffice_Temp] SET IsSendToPost = '1' WHERE ReceiveNumber = @ReceiveNumber";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@ReceiveNumber", row["ReceiveNumber"].ToString()));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 取 PostDetail 資料
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static DataTable GetDetailData(string fileName)
        {
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
SELECT a.ID, a.FileName, a.FileDate, b.DataType, b.AgencyCode, b.Filler1, b.FileCreateDate, b.BatchNo,
	b.RowNo, b.ApplyType, b.AccountType, b.AccountNo, b.CusID, b.AccID, b.StatusType, b.CheckFlag, b.Filler2
FROM [dbo].[PostOffice_Master] a WITH(NOLOCK)
INNER JOIN [dbo].[PostOffice_Detail] b WITH(NOLOCK) ON a.ID = b.MasterID
WHERE a.FileName = @FileName AND a.Complete = '0' Order by b.RowNo";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@FileName", fileName));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 取 PostTrailer 資料
        /// </summary>
        /// <param name="masterID"></param>
        /// <returns></returns>
        public static DataTable GetTrailerData(string masterID)
        {
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
SELECT b.DataType, b.AgencyCode, b.Filler1, b.FileCreateDate, b.BatchNo, b.CreateType, b.TotalCount, b.PostCreateDate, b.FailCount, b.SuccessCount, b.Filler2
FROM [dbo].[PostOffice_Master] a WITH(NOLOCK)
INNER JOIN [dbo].[PostOffice_Trailer] b WITH(NOLOCK) ON a.ID = b.MasterID
WHERE a.ID = @ID AND a.Complete = '0'";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@ID", Convert.ToInt32(masterID)));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 取回覆檔資訊
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DataTable GetReplyInformation(DateTime dateTime)
        {
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"SELECT ID, FileName FROM [dbo].[PostOffice_Master] WITH(NOLOCK) WHERE FileDate = @FileDate AND Complete = '0'";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@FileDate", dateTime.ToString("yyyy-MM-dd")));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 回寫回覆檔 Master
        /// </summary>
        /// <param name="masterID"></param>
        /// <returns></returns>
        public static bool UpDateReplyMaster(string masterID)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"UPDATE [dbo].[PostOffice_Master] SET Complete = '1' WHERE ID = @ID";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@ID", masterID));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 回寫回覆檔 Detail
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static bool UpDateReplyDetail(DataRow row, string masterID)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
UPDATE [dbo].[PostOffice_Detail] 
    SET StatusType = @StatusType, CheckFlag = @CheckFlag, Filler2 = @Filler2, Complete = @Complete
WHERE MasterID = @MasterID AND RowNo = @RowNo AND AccountNo = @AccountNo AND CusID = @CusID AND AccID = @AccID";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@MasterID", masterID));
            sqlcmd.Parameters.Add(new SqlParameter("@RowNo", row["RowNo"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@AccountNo", row["AccountNo"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@CusID", row["CusID"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@AccID", row["AccID"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@StatusType", row["StatusType"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@CheckFlag", row["CheckFlag"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@Filler2", row["Filler2"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@Complete", row["Complete"].ToString()));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 取回覆 PostOffice_Temp 資訊
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static DataTable GetReceiveNumber(DataRow row)
        {
            string accType = row["AccountType"].ToString().Trim();
            string accNo = row["AccountNo"].ToString().Trim();
            string applyType = row["ApplyType"].ToString().Trim();
            string sql = "";
            string sqlWhere = " AND b.Keyin_Flag = '2' ";

            if (applyType == "2")
            {
                sqlWhere = "";
            }

            if (accType == "G")
            {
                accNo = accNo.Substring(6, 8);
            }

            sql = @"
SELECT a.CusID, a.ReceiveNumber, a.CusName, a.AccNoBank, a.AccNo, a.AccID, a.ApplyCode, a.AccType, 
	a.AccDeposit, a.CusNo, a.AgentID, a.ModDate, a.IsNeedUpload, a.UploadDate, b.Pay_Way, b.Bcycle_Code, 
	b.Mobile_Phone, b.E_Mail, b.E_Bill, a.ReturnStatusTypeCode, a.ReturnCheckFlagCode, a.ReturnDate
FROM [dbo].[PostOffice_Temp] a WITH(NOLOCK)
LEFT JOIN [dbo].[Auto_Pay] b WITH(NOLOCK) ON a.ReceiveNumber = b.Receive_Number" + sqlWhere;

            sql = sql + @"
WHERE CusID = @CusID AND AccID = @AccID and AccNo = @AccNo
ORDER BY a.ReceiveNumber DESC";
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sql;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@CusID", row["CusID"].ToString().Trim()));
            sqlcmd.Parameters.Add(new SqlParameter("@AccID", row["AccID"].ToString().Trim()));
            sqlcmd.Parameters.Add(new SqlParameter("@AccNo", accNo));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        public static DataTable GetReceiveNumber(DataRow row, Boolean isByUploadDate)
        {
            string accType = row["AccountType"].ToString().Trim();
            string accNo = row["AccountNo"].ToString().Trim();
            string applyType = row["ApplyType"].ToString().Trim();
            string sql = "";
            string sqlWhere = " AND b.Keyin_Flag = '2' ";

            if (applyType == "2")
            {
                sqlWhere = "";
            }

            if (accType == "G")
            {
                accNo = accNo.Substring(6, 8);
            }

            sql = @"
SELECT a.CusID, a.ReceiveNumber, a.CusName, a.AccNoBank, a.AccNo, a.AccID, a.ApplyCode, a.AccType, 
	a.AccDeposit, a.CusNo, a.AgentID, a.ModDate, a.IsNeedUpload, a.UploadDate, b.Pay_Way, b.Bcycle_Code, 
	b.Mobile_Phone, b.E_Mail, b.E_Bill, a.ReturnStatusTypeCode, a.ReturnCheckFlagCode, a.ReturnDate
FROM [dbo].[PostOffice_Temp] a WITH(NOLOCK)
LEFT JOIN [dbo].[Auto_Pay] b WITH(NOLOCK) ON a.ReceiveNumber = b.Receive_Number " + sqlWhere;

            sql += " WHERE CusID = @CusID AND AccID = @AccID and AccNo = @AccNo ";
            sql += (isByUploadDate == false) ? "" : " and UploadDate = @UploadDate ";
            sql += "ORDER BY a.ReceiveNumber DESC";
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sql;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@CusID", row["CusID"].ToString().Trim()));
            sqlcmd.Parameters.Add(new SqlParameter("@AccID", row["AccID"].ToString().Trim()));
            sqlcmd.Parameters.Add(new SqlParameter("@AccNo", accNo));
            sqlcmd.Parameters.Add(new SqlParameter("@UploadDate", row["FileCreateDate"].ToString().Trim()));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        public static bool UpDatePostTemp(string receiveNumber, DataRow row)
        {
            bool result = false;
            string sendHostResult = "";

            if (row["StatusType"].ToString().Trim() != "" || row["CheckFlag"].ToString().Trim() != "")
            {
                sendHostResult = "N";
            }

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
UPDATE [dbo].[PostOffice_Temp] 
    SET ReturnStatusTypeCode = @ReturnStatusTypeCode , ReturnCheckFlagCode = @ReturnCheckFlagCode, ReturnDate = @ReturnDate, SendHostResult = @SendHostResult
WHERE ReceiveNumber = @ReceiveNumber";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@ReceiveNumber", receiveNumber));
            sqlcmd.Parameters.Add(new SqlParameter("@ReturnStatusTypeCode", row["StatusType"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@ReturnCheckFlagCode", row["CheckFlag"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@SendHostResult", sendHostResult));
            sqlcmd.Parameters.Add(new SqlParameter("@ReturnDate", DateTime.Now.ToString("yyyyMMdd")));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 回寫回覆檔 Trailer
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static bool UpDateReplyTrailer(DataRow row)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
UPDATE [dbo].[PostOffice_Trailer] 
    SET PostCreateDate = @PostCreateDate, FailCount = @FailCount, SuccessCount = @SuccessCount, Filler2 = @Filler2 
WHERE MasterID = @MasterID";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@MasterID", row["MasterID"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@PostCreateDate", row["PostCreateDate"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@FailCount", row["FailCount"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@SuccessCount", row["SuccessCount"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@Filler2", row["Filler2"].ToString()));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 取 FTP FileInfo
        /// </summary>
        /// <param name="ftpIp"></param>
        /// <param name="ftpId"></param>
        /// <param name="FtpPwd"></param>
        /// <param name="FtpPath"></param>
        /// <param name="ZipPwd"></param>
        /// <returns></returns>
        public static bool GetFTPInfo(string jobID, ref string ftpIp, ref string ftpId, ref string FtpPwd, ref string FtpPath, ref string ZipPwd)
        {
            DataTable result = new DataTable();
            string sql = @"SELECT FtpIP, FtpUserName, FtpPwd, FtpPath, ZipPwd FROM [dbo].[tbl_FileInfo] WITH(NOLOCK) WHERE Job_ID = 'BatchJob_SendToPost'";
            try
            {
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;

                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    ftpIp = resultSet.Tables[0].Rows[0][0].ToString();
                    ftpId = resultSet.Tables[0].Rows[0][1].ToString();
                    FtpPwd = RedirectHelper.GetDecryptString(resultSet.Tables[0].Rows[0][2].ToString());
                    FtpPath = resultSet.Tables[0].Rows[0][3].ToString();
                    ZipPwd = RedirectHelper.GetDecryptString(resultSet.Tables[0].Rows[0][4].ToString());
                }

                return true;
            }
            catch (Exception ex)
            {
                SaveLog(ex);
                return false;
            }
        }



        /// <summary>
        /// 取 FTP FileInfo
        /// </summary>
        /// <param name="ftpIp"></param>
        /// <param name="ftpId"></param>
        /// <param name="FtpPwd"></param>
        /// <param name="FtpPath"></param>
        /// <param name="ZipPwd"></param>
        /// <returns></returns>
        public static bool GetFTPInfo2(string jobID, ref string ftpIp, ref string ftpId, ref string FtpPwd, ref string FtpPath, ref string ZipPwd)
        {
            DataTable result = new DataTable();
            string sql = @"SELECT FtpIP, FtpUserName, FtpPwd, FtpPath, ZipPwd FROM [dbo].[tbl_FileInfo] WITH(NOLOCK) WHERE Job_ID = '" + jobID + "'";
            try
            {
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;

                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    ftpIp = resultSet.Tables[0].Rows[0][0].ToString();
                    ftpId = resultSet.Tables[0].Rows[0][1].ToString();
                    FtpPwd = RedirectHelper.GetDecryptString(resultSet.Tables[0].Rows[0][2].ToString());
                    FtpPath = resultSet.Tables[0].Rows[0][3].ToString();
                    ZipPwd = RedirectHelper.GetDecryptString(resultSet.Tables[0].Rows[0][4].ToString());
                }

                return true;
            }
            catch (Exception ex)
            {
                SaveLog(ex);
                return false;
            }
        }

        /// <summary>
        /// 取 FTP FileInfo
        /// </summary>
        /// <param name="jobID"></param>
        /// <param name="ftpIp"></param>
        /// <param name="ftpId"></param>
        /// <param name="FtpPwd"></param>
        /// <param name="FtpPath"></param>
        /// <param name="ZipPwd"></param>
        /// <returns></returns>
        public static bool GetFTPInfoDownload(string jobID, ref string ftpIp, ref string ftpId, ref string FtpPwd, ref string FtpPath, ref string ZipPwd)
        {
            DataTable result = new DataTable();
            string sql = @"SELECT FtpIP, FtpUserName, FtpPwd, FtpPath, ZipPwd FROM [dbo].[tbl_FileInfo] WITH(NOLOCK) WHERE Job_ID = 'BatchJob_PostReply'";
            try
            {
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;

                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    ftpIp = resultSet.Tables[0].Rows[0][0].ToString();
                    ftpId = resultSet.Tables[0].Rows[0][1].ToString();
                    FtpPwd = RedirectHelper.GetDecryptString(resultSet.Tables[0].Rows[0][2].ToString());
                    FtpPath = resultSet.Tables[0].Rows[0][3].ToString();
                    ZipPwd = RedirectHelper.GetDecryptString(resultSet.Tables[0].Rows[0][4].ToString());
                }

                return true;
            }
            catch (Exception ex)
            {
                SaveLog(ex);
                return false;
            }
        }

        /// <summary>
        /// 取郵局主動終止日期
        /// </summary>
        /// <returns></returns>
        public static DataTable GetCatchDate()
        {
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"SELECT PostRtnCode FROM [dbo].[PostOffice_Rtn_Info] WITH(NOLOCK) WHERE RtnType = '9'";
            sqlcmd.CommandType = CommandType.Text;

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 更新 PostOffice_Temp.IsSendToPost 狀態
        /// </summary>
        /// <param name="receiveNumber"></param>
        /// <param name="snedToHostResult"></param>
        /// <returns></returns>
        public static bool UpdatePostOfficeTemp(string receiveNumber, string snedToHostResult, string sendHostResultCode)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
UPDATE [dbo].[PostOffice_Temp] SET SendHostResult = @SendHostResult, IsSendToPost = '1', SendHostResultCode = @SendHostResultCode WHERE ReceiveNumber = @ReceiveNumber";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@SendHostResult", snedToHostResult));
            sqlcmd.Parameters.Add(new SqlParameter("@ReceiveNumber", receiveNumber));
            sqlcmd.Parameters.Add(new SqlParameter("@SendHostResultCode", sendHostResultCode));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 寫入 PostOffice_Detail
        /// </summary>
        /// <param name="masterID"></param>
        /// <param name="cusID"></param>
        /// <param name="accNO"></param>
        /// <param name="accID"></param>
        /// <param name="rowNo"></param>
        /// <returns></returns>
        public static bool InsertPostDetail(int masterID, string cusID, string accNO, string accID, string rowNo, string applyType)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
INSERT INTO dbo.PostOffice_Detail (MasterID, ReceiveNumber, DataType, AgencyCode, Filler1, FileCreateDate, BatchNo, RowNo, ApplyType, AccountType, AccountNo, CusID, AccID, StatusType, CheckFlag, Filler2, Complete)
    VALUES (@MasterID, '', '1', '', '', '', '', @RowNo, @applyType, '', @AccountNo, @CusID, @AccID, '', '', '', '1')";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@MasterID", masterID));
            sqlcmd.Parameters.Add(new SqlParameter("@RowNo", rowNo));
            sqlcmd.Parameters.Add(new SqlParameter("@AccountNo", accNO));
            sqlcmd.Parameters.Add(new SqlParameter("@CusID", cusID));
            sqlcmd.Parameters.Add(new SqlParameter("@AccID", accID));
            sqlcmd.Parameters.Add(new SqlParameter("@applyType", applyType));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 寫入 Auto_Pay_Popul 等晚上跑批次
        /// </summary>
        /// <param name="cusID"></param>
        /// <param name="accNO"></param>
        /// <param name="accID"></param>
        /// <returns></returns>
        public static bool InsertAutoPayPopul(string cusID, string accNO, string accID)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
INSERT INTO dbo.Auto_Pay_Popul (Receive_Number, Cus_Id, Case_Class, Popul_No, Popul_EmpNo, isEnd, FUNCTION_CODE, mod_date, KeyIn_Flag, User_Id, AccNoBank, Acc_No, Acc_Id, Cus_name, Pay_Way)
    VALUES ('', @Cus_Id, '04', '', '', 'Y', 'D', GETDATE(), '0', 'CSIP_SYS', '701', @Acc_No, @Acc_Id, '', '0')";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@Cus_Id", cusID));
            sqlcmd.Parameters.Add(new SqlParameter("@Acc_No", accNO));
            sqlcmd.Parameters.Add(new SqlParameter("@Acc_Id", accID));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 取 PostOffice_Rtn_Info
        /// </summary>
        /// <returns></returns>
        public static DataTable GetPostOfficeRtnInfo()
        {
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"SELECT PostRtnCode, PostRtnMsg FROM [dbo].[PostOffice_Rtn_Info] WITH(NOLOCK) WHERE RtnType IN ('1', '2')";
            sqlcmd.CommandType = CommandType.Text;

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 取 masterID
        /// </summary>
        /// <param name="fileCreateDate"></param>
        /// <returns></returns>
        public static DataTable GetMasterID(string fileCreateDate)
        {
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
SELECT a.ID, a.FileName, a.Complete, b.ReceiveNumber, b.RowNo 
FROM [dbo].[PostOffice_Master] a WITH(NOLOCK)
INNER JOIN [dbo].[PostOffice_Detail] b WITH(NOLOCK) ON a.ID = b.MasterID
WHERE a.FileName = @FileName ";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@FileName", "CTCBAuth" + fileCreateDate + ".DAT"));
            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        public static bool InsertPostOfficeTemp(string cusID, string accNO, string accID)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
INSERT INTO [dbo].[PostOffice_Temp] ([CusID], [ReceiveNumber], [CusName], [AccNoBank], [AccNo], [AccID], [ApplyCode], [AccType], [AccDeposit], [CusNo], [AgentID], [ModDate], [IsNeedUpload], [UploadDate], [ReturnStatusTypeCode], [ReturnCheckFlagCode], [ReturnDate], [IsSendToPost], [SendHostResult], [SendHostResultCode])
	VALUES (@CusID, '', '', '701', @AccNo, @AccID,'3', 'P', @AccNo, @CusID, 'CSIP_SYSTEM', @ModDate, '', '', '', '', @ModDate, '1', 'S', '0010')";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@CusID", cusID));
            sqlcmd.Parameters.Add(new SqlParameter("@AccNo", accNO));
            sqlcmd.Parameters.Add(new SqlParameter("@AccID", accID));
            sqlcmd.Parameters.Add(new SqlParameter("@ModDate", DateTime.Now.ToString("yyyyMMdd")));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }



        /// <summary>
        /// 取要上傳分公司異動需要給主機的資料
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static DataTable GetBranchDataSendToAMLData(string endTime)
        {
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
            SELECT 
            ID, CASE_NO, FileName, BRCH_BATCH_NO, BRCH_INTER_ID, BRCH_SIXM_TOT_AMT, BRCH_MON_AMT1, BRCH_MON_AMT2, BRCH_MON_AMT3, BRCH_MON_AMT4, BRCH_MON_AMT5, BRCH_MON_AMT6, BRCH_MON_AMT7, BRCH_MON_AMT8, BRCH_MON_AMT9, BRCH_MON_AMT10, BRCH_MON_AMT11, BRCH_MON_AMT12, BRCH_KEY, BRCH_BRCH_NO, BRCH_BRCH_SEQ, BRCH_BRCH_TYPE, BRCH_NATION, BRCH_BIRTH_DATE, BRCH_PERM_CITY, BRCH_PERM_ADDR1, BRCH_PERM_ADDR2, BRCH_CHINESE_NAME, BRCH_ENGLISH_NAME, BRCH_ID, BRCH_OWNER_ID_ISSUE_DATE, BRCH_OWNER_ID_ISSUE_PLACE, BRCH_OWNER_ID_REPLACE_TYPE, BRCH_ID_PHOTO_FLAG, BRCH_PASSPORT, BRCH_PASSPORT_EXP_DATE, BRCH_RESIDENT_NO, BRCH_RESIDENT_EXP_DATE, BRCH_OTHER_CERT, BRCH_OTHER_CERT_EXP_DATE, BRCH_COMP_TEL, BRCH_CREATE_DATE, BRCH_STATUS, BRCH_CIRCULATE_MERCH, BRCH_HQ_BRCH_NO, BRCH_HQ_BRCH_SEQ_NO, BRCH_UPDATE_DATE, BRCH_QUALIFY_FLAG,BRCH_UPDATE_ID,BRCH_REAL_CORP,BRCH_RESERVED_FILLER, Create_Time, Create_User, Create_Date, BRCH_ID_Type, BRCH_ID_SreachStatus, BRCH_ExportFileFlag, BRCH_LastExportTime, LAST_UPD_MAKER, LAST_UPD_CHECKER, LAST_UPD_BRANCH
            FROM [dbo].[AML_BRCH_Work] where [BRCH_ExportFileFlag]='1'
            ";

            sqlcmd.CommandType = CommandType.Text;
            //sqlcmd.Parameters.Add(new SqlParameter("@UploadDate", endTime));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }


        /// <summary>
        /// 取得結案異動檔
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static DataTable GetCaseToAMLData(string endTime)
        {
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();

            sqlcmd.CommandText = @"
            SELECT AML_CASE_DATA.CASE_NO,AML_HQ_Work.ID,RMMBatchNo,AMLInternalID,'CARD' as SourceSystem,DataDate,CustomerID,HCOP_HEADQUATERS_CORP_SEQ as corpseq,Close_Date,CaseOwner_User as LastUpdateMaker,nl_user as LastUpdateChecker,LastUpdateBranch as LastUpdateBranch,nl_datetime as LastUpdateDate
            FROM [dbo].[AML_CASE_DATA]
            INNER JOIN AML_Edata_Work ON AML_CASE_DATA.CASE_NO=AML_Edata_Work.CASE_NO
            INNER JOIN AML_HQ_Work ON AML_HQ_Work.CASE_NO=AML_Edata_Work.CASE_NO
            INNER JOIN notelog ON notelog.nl_case_no =AML_Edata_Work.CASE_NO
            WHERE AML_ExportFileFlag='1' and NL_TYPE='Verify' 
            and NL_DateTime in (SELECT max(NL_DateTime) 
            FROM [dbo].[AML_CASE_DATA]
            INNER JOIN AML_Edata_Work ON AML_CASE_DATA.CASE_NO=AML_Edata_Work.CASE_NO
            INNER JOIN AML_HQ_Work ON AML_HQ_Work.CASE_NO=AML_Edata_Work.CASE_NO
            INNER JOIN  notelog ON notelog.nl_case_no =AML_Edata_Work.CASE_NO
            WHERE AML_ExportFileFlag='1' and NL_TYPE='Verify' 
            group by nl_case_no) 
            ";
            sqlcmd.CommandType = CommandType.Text;
            //sqlcmd.Parameters.Add(new SqlParameter("@UploadDate", endTime));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        //20191120-RQ-2018-015749-002 調整RMM檔抓取內容,將異常結案與案件重起的案件皆需納入
        /// <summary>
        /// 取得結案異動檔
        /// </summary>
        /// <param name="type">A:to AML</param>
        /// <returns></returns>
        public static DataTable GetCaseToAMLData_newflow(string type)
        {
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();

            //20200326 修正效能
            /*
            sqlcmd.CommandText = @"
                    WITH RMMCloseFile (CASE_NO,ID,RMMBatchNo,AMLInternalID,SourceSystem,DataDate,CustomerID,corpseq,Close_Date,
                    LastUpdateMaker ,LastUpdateChecker,LastUpdateBranch,LastUpdateDate,NL_TYPE) 
                    AS (
	                    SELECT A.CASE_NO,C.ID,RMMBatchNo,AMLInternalID,'CARD' as SourceSystem,DataDate,CustomerID,HCOP_HEADQUATERS_CORP_SEQ  as corpseq,Close_Date,
	                    CaseOwner_User as LastUpdateMaker,NL_USER as LastUpdateChecker,LastUpdateBranch ,NL_DateTime as LastUpdateDate,NL_TYPE
	                    FROM [dbo].[AML_CASE_DATA] A
	                    INNER JOIN AML_Edata_Work  B ON A.CASE_NO=B.CASE_NO
	                    INNER JOIN AML_HQ_Work  C ON C.CASE_NO=B.CASE_NO
	                    INNER JOIN NoteLog D ON D.NL_CASE_NO =B.CASE_NO
	                    WHERE AML_ExportFileFlag='1' and NL_TYPE IN ('CaseOK' ,'NonCooperatedDone','CaseClosedDone','OtherClosedDone')
                    )
                    SELECT * 
                    FROM RMMCloseFile 
                    WHERE LastUpdateDate IN 
                    (
                        SELECT MAX(LastUpdateDate) as 'NL_DateTime'
                        FROM RMMCloseFile
                        GROUP BY CASE_NO
                    )
            ";*/
            sqlcmd.CommandText = @"
                    SELECT A.CASE_NO,C.ID,RMMBatchNo,AMLInternalID,'CARD' as SourceSystem,DataDate,CustomerID,HCOP_HEADQUATERS_CORP_SEQ  as corpseq,Close_Date,
	                    CaseOwner_User as LastUpdateMaker,NL_USER as LastUpdateChecker,LastUpdateBranch ,NL_DateTime as LastUpdateDate,NL_TYPE,CaseProcess_Status
                        INTO #RMMCloseFile
	                    FROM [dbo].[AML_CASE_DATA] A
	                    INNER JOIN AML_Edata_Work  B ON A.CASE_NO=B.CASE_NO
	                    INNER JOIN AML_HQ_Work  C ON C.CASE_NO=B.CASE_NO
	                    INNER JOIN NoteLog D ON D.NL_CASE_NO =B.CASE_NO
	                    WHERE AML_ExportFileFlag='1' and NL_TYPE IN ('CaseOK' ,'NonCooperatedDone','CaseClosedDone','OtherClosedDone')
                    SELECT * 
                    FROM #RMMCloseFile 
                    WHERE LastUpdateDate IN 
                    (
                        SELECT MAX(LastUpdateDate) as 'NL_DateTime'
                        FROM #RMMCloseFile
                        GROUP BY CASE_NO
                    )
            ";
            if (type.Trim().Equals("A"))
            {
                sqlcmd.CommandText += " AND SUBSTRING (CASE_NO,9,1) NOT IN ('8','9') ";
            }
            else
            {
                //20211221 AML NOVA 功能需求程式碼,註解保留 by Ares Dennis
                //sqlcmd.CommandText += " AND CaseProcess_Status IN ('2','23') ";// 20210527 EOS_AML(NOVA) 加上不合作結案(23)的案件 by Ares Dennis //20200408-RQ-2019-030155-005-送主機結案檔如遇異常結案不必送主機
                sqlcmd.CommandText += " AND CaseProcess_Status IN ('2') ";// 20210527 EOS_AML(NOVA) 加上不合作結案(23)的案件 by Ares Dennis //20200408-RQ-2019-030155-005-送主機結案檔如遇異常結案不必送主機
            }
            sqlcmd.CommandText += " DROP TABLE #RMMCloseFile";
            sqlcmd.CommandType = CommandType.Text;

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        //20200409-RQ-2019-030155-005 調整RMM檔抓取內容,將異常結案與案件重起的案件皆需納入
        /// <summary>
        /// 補跑更新主機資料
        /// </summary>
        public static DataTable GetCaseToMainframeData_OneTime()
        {
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();

            sqlcmd.CommandText = @"
                    SELECT A.CASE_NO,C.ID,RMMBatchNo,AMLInternalID,'CARD' as SourceSystem,DataDate,CustomerID,HCOP_HEADQUATERS_CORP_SEQ  as corpseq,Close_Date,
	                    CaseOwner_User as LastUpdateMaker,NL_USER as LastUpdateChecker,LastUpdateBranch ,NL_DateTime as LastUpdateDate,NL_TYPE,CaseProcess_Status
                        INTO #RMMCloseFile
	                    FROM [dbo].[AML_CASE_DATA] A
	                    INNER JOIN AML_Edata_Work  B ON A.CASE_NO=B.CASE_NO
	                    INNER JOIN AML_HQ_Work  C ON C.CASE_NO=B.CASE_NO
	                    INNER JOIN NoteLog D ON D.NL_CASE_NO =B.CASE_NO
	                    WHERE AML_ExportFileFlag='0' and NL_TYPE = 'CaseOK'
                        AND CONVERT(VARCHAR,A.CLOSE_DATE,112) BETWEEN '20200120' AND '20200325'
                    SELECT * 
                    FROM #RMMCloseFile 
                    WHERE LastUpdateDate IN 
                    (
                        SELECT MAX(LastUpdateDate) as 'NL_DateTime'
                        FROM #RMMCloseFile
                        GROUP BY CASE_NO
                    )
                    DROP TABLE #RMMCloseFile
            ";

            sqlcmd.CommandType = CommandType.Text;

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }


        /// <summary>
        /// 取得開案兩個月之後都還沒做結案的資料 ，條件為AML_CASE_DATA close_date為null代表未結案，然後二次傳檔沒傳過的。
        /// 20191216-案件重審的案件無需再送地址條，故需排除 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        //public static DataTable GetAddressLebelDataTwoMonth(string endTime)
        public static DataTable GetAddressLebelDataTwoMonth()
        {
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            /* 20200106-20200207 RC-發送不合作信函，條件：
                1. 建案年月+2個月 未結案 (既有條件)
                2. 不合作Flag 不等於Y
                3. 商店狀態 不等於CLOSE
                4.不合作信函的商店名稱改抓主機現行的登記名稱
            */
            /*
            sqlcmd.CommandText = @"
            SELECT CASE_NO,ID,HCOP_NAME_CHI,HCOP_MAILING_CITY,HCOP_MAILING_ADDR1,HCOP_MAILING_ADDR2,ZIP_CODE 
            FROM [KeyinGUI].[dbo].[AML_HQ_Work]
            LEFT JOIN [KeyinGUI].[dbo].[szip]
            ON [KeyinGUI].[dbo].[AML_HQ_Work].HCOP_MAILING_CITY =  [KeyinGUI].[dbo].[szip].ZIP_DATA
            where case_no in (
            SELECT case_no
		    FROM [KeyinGUI].[dbo].[AML_CASE_DATA] WHERE CLOSE_DATE IS NULL
		    AND CREATE_YM = LEFT(CONVERT(varchar, DateAdd(m,-2,GetDate()),112),6)
		    ) and AddressLabelTwoMonthFlag is null   AND SUBSTRING(CASE_NO,9,1) NOT IN ('8','9')
            order by ZIP_CODE ASC
            ";
            */
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            //20220127_Ares_Jack_新增條件只搜尋統編
            sqlcmd.CommandText = string.Format(@"
            SELECT CASE_NO,a.ID,d.REG_NAME as 'HCOP_NAME_CHI',HCOP_MAILING_CITY,HCOP_MAILING_ADDR1,HCOP_MAILING_ADDR2,ZIP_CODE 
            FROM [{0}].[dbo].[AML_HQ_Work] a
            LEFT JOIN [{0}].[dbo].[szip] b ON a.HCOP_MAILING_CITY =  b.ZIP_DATA
			JOIN [{0}].[dbo].[AML_Cdata_Work] c ON c.CustomerID = a.HCOP_HEADQUATERS_CORP_NO
	        JOIN [{0}].[dbo].[AML_HCOP_STATUS] d ON d.CORP_NO = a.HCOP_HEADQUATERS_CORP_NO
            where case_no in (
            SELECT case_no
		    FROM [{0}].[dbo].[AML_CASE_DATA] WHERE CLOSE_DATE IS NULL
		    AND CREATE_YM = LEFT(CONVERT(varchar, DateAdd(m,-2,GetDate()),112),6)
		    ) and AddressLabelTwoMonthFlag is null   AND SUBSTRING(CASE_NO,9,1) NOT IN ('8','9')
			AND c.INCORPORATED<> 'Y' AND d.STATUS='O' 
            AND PATINDEX('[0-9]%', a.HCOP_HEADQUATERS_CORP_NO) > 0
            order by ZIP_CODE ASC
            ", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            sqlcmd.CommandType = CommandType.Text;
            //sqlcmd.Parameters.Add(new SqlParameter("@UploadDate", endTime));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }



        /// <summary>
        /// 當系統匯入完E、總公司檔之後，系統讀取資料庫的總公司資料表裡撈取尚未做出檔案的資料，產出地址條
        /// 20191216-案件重審的案件無需再送地址條，故需排除 
        /// BatchJob_SendAddressLebel 呼叫使用
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static DataTable GetAddressLebelData(string endTime)
        {
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();

            //20200224 -RQ-2019-030155-003 mail @ 20200224 11:26 提出需求：調整產定審地址條邏輯，要有收到AML的E檔再產出，當天無E檔，雖有收到總公司檔也不用寄送。及如地址是NULL補齊後要能寄送，判斷FLAG='2'
            /* 
             * 麻煩您調整產定審地址條邏輯，要有收到AML的E檔再產出，當天無E檔，雖有收到總公司檔也不用寄送。
            sqlcmd.CommandText = @"
            SELECT CASE_NO,ID,HCOP_NAME_CHI,HCOP_MAILING_CITY,HCOP_MAILING_ADDR1,HCOP_MAILING_ADDR2,ZIP_CODE 
            FROM [KeyinGUI].[dbo].[AML_HQ_Work]
            LEFT JOIN [KeyinGUI].[dbo].[szip]
            ON [KeyinGUI].[dbo].[AML_HQ_Work].HCOP_MAILING_CITY =  [KeyinGUI].[dbo].[szip].ZIP_DATA
			WHERE AddressLabelflag is null  AND SUBSTRING(CASE_NO,9,1) NOT IN ('8','9') 
			order by zip_code asc
            ";
            */
            //20200224
            /*
            sqlcmd.CommandText = @"SELECT a.CASE_NO,a.ID,HCOP_NAME_CHI,HCOP_MAILING_CITY,HCOP_MAILING_ADDR1,HCOP_MAILING_ADDR2,ZIP_CODE 
            FROM [KeyinGUI].[dbo].[AML_HQ_Work] a LEFT JOIN [KeyinGUI].[dbo].[szip] b
            ON a.HCOP_MAILING_CITY =  b.ZIP_DATA
			JOIN [dbo].[AML_Edata_Work] c on c.CASE_NO = a.CASE_NO
			WHERE SUBSTRING(a.CASE_NO,9,1) NOT IN ('8','9') 
			AND (
			(AddressLabelflag is null  AND (SELECT top 1 CREATE_DATE FROM AML_Edata_Import ORDER BY CREATE_DATE DESC ) = @UploadDate )
			OR (c.CREATE_USER='NonDelivered' AND CONVERT(VARCHAR, @UploadDate,112)<=CONVERT(VARCHAR,DATEADD(MONTH,2,c.CREATE_DATE),112))
			)
			order by zip_code asc
            ";*/
            //20200420
            /*
            sqlcmd.CommandText = @"SELECT a.CASE_NO,a.ID,HCOP_NAME_CHI,HCOP_MAILING_CITY,HCOP_MAILING_ADDR1,HCOP_MAILING_ADDR2,ZIP_CODE 
            FROM [KeyinGUI].[dbo].[AML_HQ_Work] a LEFT JOIN [KeyinGUI].[dbo].[szip] b
            ON a.HCOP_MAILING_CITY =  b.ZIP_DATA
			JOIN [dbo].[AML_Edata_Work] c on c.CASE_NO = a.CASE_NO
			WHERE SUBSTRING(a.CASE_NO,9,1) NOT IN ('8','9') 
			AND (
			(AddressLabelflag is null  AND c.CREATE_DATE  = @UploadDate )
			OR (c.CREATE_USER='NonDelivered' AND CONVERT(VARCHAR,DATEADD(MONTH,2,c.CREATE_DATE),112) >= @UploadDate)
			)
			order by zip_code asc
            ";
            */
            // 20210527 EOS_AML(NOVA) by Ares Dennis
            #region 退版機制
            DataTable dt = new DataTable();
            CSIPCommonModel.BusinessRules_new.BRM_PROPERTY_CODE.GetEnableProperty("01", new string[] { "OLD_VERSION_FLAG" }, ref dt);
            string flag = "";
            if (dt.Rows.Count > 0)
            {
                flag = dt.Rows[0]["PROPERTY_CODE"].ToString();
            }
            #endregion
            if (flag == "N")// 新版程式碼
            {
                //2021/7/6 EOS_AML(NOVA) 每月收E檔作業，針對未結案的統編只能出一份信函 by Ares Dennis
                //20220208_Ares_Jack_新增條件只搜尋統編
                sqlcmd.CommandText = string.Format(@"SELECT a.CASE_NO,a.ID,HCOP_NAME_CHI,HCOP_MAILING_CITY,HCOP_MAILING_ADDR1,HCOP_MAILING_ADDR2,ZIP_CODE 
                         FROM [{0}].[dbo].[AML_HQ_Work] a LEFT JOIN [{0}].[dbo].[szip] b
                         ON a.HCOP_MAILING_CITY =  b.ZIP_DATA
                JOIN [dbo].[AML_Edata_Work] c on c.CASE_NO = a.CASE_NO
                         JOIN AML_CASE_DATA d ON d.CASE_NO = a.CASE_NO 
                WHERE SUBSTRING(a.CASE_NO,9,1) NOT IN ('8','9') AND A.GROUP_NO = A.CASE_NO 
                AND (
                            (AddressLabelflag is null  AND d.Close_Date IS NULL)
                            OR (c.CREATE_USER='NonDelivered' AND CONVERT(VARCHAR,DATEADD(MONTH,2,c.CREATE_DATE),112) >= @UploadDate)
                )
                AND PATINDEX('[0-9]%', a.HCOP_HEADQUATERS_CORP_NO) > 0
                order by zip_code asc
                         ", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            }
            else
            {
                //RQ-2021-004136-001 //2021/04/01_Ares_Stanley-合併檔案後變更DB名稱為變數
                //20220126_Ares_Jack_新增條件只搜尋統編
                sqlcmd.CommandText = string.Format(@"SELECT a.CASE_NO,a.ID,HCOP_NAME_CHI,HCOP_MAILING_CITY,HCOP_MAILING_ADDR1,HCOP_MAILING_ADDR2,ZIP_CODE 
                FROM [{0}].[dbo].[AML_HQ_Work] a LEFT JOIN [{0}].[dbo].[szip] b
                ON a.HCOP_MAILING_CITY =  b.ZIP_DATA
			    JOIN [dbo].[AML_Edata_Work] c on c.CASE_NO = a.CASE_NO
                JOIN AML_CASE_DATA d ON d.CASE_NO = a.CASE_NO 
			    WHERE SUBSTRING(a.CASE_NO,9,1) NOT IN ('8','9') 
			    AND (
			                (AddressLabelflag is null  AND d.Close_Date IS NULL)
			                OR (c.CREATE_USER='NonDelivered' AND CONVERT(VARCHAR,DATEADD(MONTH,2,c.CREATE_DATE),112) >= @UploadDate)
			    )
                AND PATINDEX('[0-9]%', a.HCOP_HEADQUATERS_CORP_NO) > 0
			    order by zip_code asc
                ", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            }

            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@UploadDate", endTime));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }



        /// <summary>
        /// 取得地址條為NULL的地方
        /// 20191216-案件重審的案件無需再送地址條，故需排除 
        /// 20200224 mail @ 20200224 11:26 提出需求：調整產定審地址條邏輯，要有收到AML的E檔再產出，當天無E檔，雖有收到總公司檔也不用寄送。
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static DataTable GetLebelDataNull(string endTime)
        {
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            //20200717
            /*
            sqlcmd.CommandText = @"
		    select HCOP_MAILING_CITY,HCOP_HEADQUATERS_CORP_NO,CASE_NO 
            from 
            (
                SELECT HCOP_HEADQUATERS_CORP_NO,HCOP_NAME_CHI,HCOP_MAILING_CITY,HCOP_MAILING_ADDR1,HCOP_MAILING_ADDR2,ZIP_CODE,AddressLabelflag,CASE_NO 
                FROM [KeyinGUI].[dbo].[AML_HQ_Work]
                LEFT JOIN [KeyinGUI].[dbo].[szip]
                ON [KeyinGUI].[dbo].[AML_HQ_Work].HCOP_MAILING_CITY =  [KeyinGUI].[dbo].[szip].ZIP_DATA
            )  as a
            where a.zip_code is null and AddressLabelflag is null   AND SUBSTRING(CASE_NO,9,1) NOT IN ('8','9') 
            AND (SELECT top 1 CREATE_DATE FROM AML_Edata_Work ORDER BY CREATE_DATE DESC ) = @UploadDate 
            ";*/
            //20210305-RQ-2021-004136-001-調整撈取語法
            /*
            sqlcmd.CommandText = @"
		    select HCOP_MAILING_CITY,HCOP_HEADQUATERS_CORP_NO,CASE_NO 
            from 
            (
                SELECT HCOP_HEADQUATERS_CORP_NO,HCOP_NAME_CHI,HCOP_MAILING_CITY,HCOP_MAILING_ADDR1,HCOP_MAILING_ADDR2,ZIP_CODE,AddressLabelflag,A.CASE_NO,C.CREATE_DATE,c.CREATE_USER 
                FROM  [KeyinGUI].[dbo].[AML_HQ_Work] a 
                               LEFT JOIN [KeyinGUI].[dbo].[szip] b ON a.HCOP_MAILING_CITY =  b.ZIP_DATA
			                   JOIN [dbo].[AML_Edata_Work] c on c.CASE_NO = a.CASE_NO
            )  as a
            where a.zip_code is null AND SUBSTRING(CASE_NO,9,1) NOT IN ('8','9') 
            AND (
				(AddressLabelflag is null  AND CREATE_DATE  = @UploadDate )
				OR (CREATE_USER='NonDelivered' AND CONVERT(VARCHAR,DATEADD(MONTH,2,CREATE_DATE),112) >= @UploadDate)
			)
            ";
            */
            //2021/04/01_Ares_Stanley-合併檔案後變更DB名稱為變數; 2021/05/17_合併新版
            //20220126_Ares_Jack_新增條件只搜尋統編
            sqlcmd.CommandText = string.Format(@"
            SELECT HCOP_MAILING_CITY,HCOP_HEADQUATERS_CORP_NO,CASE_NO 
            FROM 
            (
                SELECT HCOP_HEADQUATERS_CORP_NO,HCOP_NAME_CHI,HCOP_MAILING_CITY,HCOP_MAILING_ADDR1,HCOP_MAILING_ADDR2,ZIP_CODE,AddressLabelflag,A.CASE_NO,C.CREATE_DATE,c.CREATE_USER,d.Close_Date 
                FROM  [{0}].[dbo].[AML_HQ_Work] a 
                               LEFT JOIN [{0}].[dbo].[szip] b ON a.HCOP_MAILING_CITY =  b.ZIP_DATA
			                   JOIN [dbo].[AML_Edata_Work] c on c.CASE_NO = a.CASE_NO
							   JOIN AML_CASE_DATA d ON d.CASE_NO = a.CASE_NO 
            )  as a
            where AddressLabelflag is null  AND a.zip_code is null AND SUBSTRING(CASE_NO,9,1) NOT IN ('8','9') 
            AND (
				        Close_Date IS NULL 
                        OR (CREATE_USER='NonDelivered' AND CONVERT(VARCHAR,DATEADD(MONTH,2,CREATE_DATE),112) >= @UploadDate)
			) AND PATINDEX('[0-9]%', a.HCOP_HEADQUATERS_CORP_NO) > 0 ", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@UploadDate", endTime));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 取得地址條為NULL的資料 條件為AML_CASE_DATA close_date為null代表未結案，然後二次傳檔沒傳過的。
        /// 20191216-案件重審的案件無需再送地址條，故需排除 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        //20200806-RQ-2020-021027-001
        //public static DataTable GetLebelDataNullTwoMonth(string endTime)
        public static DataTable GetLebelDataNullTwoMonth()
        {
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            /* 20200106-20200207 RC-發送不合作信函，條件：
                1. 建案年月+2個月 未結案 (既有條件)
                2. 不合作Flag 不等於Y
                3. 商店狀態 不等於CLOSE
                4.不合作信函的商店名稱改抓主機現行的登記名稱
            */
            /*
            sqlcmd.CommandText = @"
  
		    select distinct(HCOP_MAILING_CITY) from (
            SELECT ID,HCOP_NAME_CHI,HCOP_MAILING_CITY,HCOP_MAILING_ADDR1,HCOP_MAILING_ADDR2,ZIP_CODE,AddressLabelTwoMonthFlag
            FROM [KeyinGUI].[dbo].[AML_HQ_Work] 
            LEFT JOIN [KeyinGUI].[dbo].[szip] 
            ON [KeyinGUI].[dbo].[AML_HQ_Work].HCOP_MAILING_CITY =  [KeyinGUI].[dbo].[szip].ZIP_DATA
            where case_no in (
            SELECT case_no
		    FROM [KeyinGUI].[dbo].[AML_CASE_DATA] WHERE CLOSE_DATE IS NULL
		    AND CREATE_YM = LEFT(CONVERT(varchar, DateAdd(m,-2,GetDate()),112),6)
		    )  and zip_code is null  and AddressLabelTwoMonthFlag is null   AND SUBSTRING(CASE_NO,9,1) NOT IN ('8','9')) as a
            ";
            */
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            //20220127_Ares_Jack_新增條件只搜尋統編
            sqlcmd.CommandText = string.Format(@"  
		        select distinct(HCOP_MAILING_CITY) from 
                (
                        SELECT a.ID,d.REG_NAME as 'HCOP_NAME_CHI',HCOP_MAILING_CITY,HCOP_MAILING_ADDR1,HCOP_MAILING_ADDR2,ZIP_CODE,AddressLabelTwoMonthFlag
                        FROM [{0}].[dbo].[AML_HQ_Work] a
                        LEFT JOIN [{0}].[dbo].[szip] b ON a.HCOP_MAILING_CITY =  b.ZIP_DATA
                        JOIN [{0}].[dbo].[AML_Cdata_Work] c ON c.CustomerID = a.HCOP_HEADQUATERS_CORP_NO
	                    JOIN [{0}].[dbo].[AML_HCOP_STATUS] d ON d.CORP_NO = a.HCOP_HEADQUATERS_CORP_NO
                        where case_no in 
                        (
                            SELECT case_no
		                    FROM [{0}].[dbo].[AML_CASE_DATA] WHERE CLOSE_DATE IS NULL
		                    AND CREATE_YM = LEFT(CONVERT(varchar, DateAdd(m,-2,GetDate()),112),6)
		                )  
                        and zip_code is null  and AddressLabelTwoMonthFlag is null   AND SUBSTRING(CASE_NO,9,1) NOT IN ('8','9')
                        AND c.INCORPORATED<> 'Y' AND d.STATUS='O' 
                        AND PATINDEX('[0-9]%', a.HCOP_HEADQUATERS_CORP_NO) > 0 
                ) as a
            ", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            sqlcmd.CommandType = CommandType.Text;
            //sqlcmd.Parameters.Add(new SqlParameter("@UploadDate", endTime));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }


        /// <summary>
        /// 取得etable 資訊
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static DataTable GetCaseNo(string batchno, string internalid)
        {
            string sql = "";
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            sql = string.Format(@"
            SELECT top 1 *
            FROM [{0}].[dbo].[AML_Edata_Import]
            where Rmmbatchno = @batchno and amlinternalid=@internalid", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sql;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@batchno", batchno));
            sqlcmd.Parameters.Add(new SqlParameter("@internalid", internalid));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }


        /// <summary>
        /// 取得AML_Cdata_Work BY customerid 資訊
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static DataTable GetCdataByCustomerID(string strCustomerID)
        {
            string sql = "";
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            sql = string.Format(@"
            SELECT top 1 *
            FROM [{0}].[dbo].[AML_Cdata_Work]
            where CUSTOMERID=@customerid", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sql;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@customerid", strCustomerID));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }



        /// <summary>
        /// 取得AML_HQ_Manager_Import 資訊
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static DataTable GetManager()
        {
            string sql = "";
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            sql = string.Format(@"
            SELECT HCOP_BATCH_NO,HCOP_INTER_ID
            FROM [{0}].[dbo].[AML_HQ_Manager_Import]  where case_no is null group by HCOP_BATCH_NO,HCOP_INTER_ID
            ", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sql;
            sqlcmd.CommandType = CommandType.Text;


            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }




        /// <summary>
        /// 更新 AML_HQ_Import 的 CASE_NO 號碼
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static bool UpdateHQtable(string caseno, string batchno, string internalid)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
            UPDATE [dbo].[AML_HQ_Import] SET CASE_NO = @CASE_NO WHERE HCOP_BATCH_NO = @batchno and HCOP_INTER_ID=@HCOP_INTER_ID";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", caseno));
            sqlcmd.Parameters.Add(new SqlParameter("@batchno", batchno));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_INTER_ID", internalid));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }


            SqlCommand sqlcmd2 = new SqlCommand();
            sqlcmd2.CommandText = @"
            UPDATE [dbo].[AML_HQ_Work] SET CASE_NO = @CASE_NO WHERE HCOP_BATCH_NO = @batchno and HCOP_INTER_ID=@HCOP_INTER_ID";
            sqlcmd2.CommandType = CommandType.Text;
            sqlcmd2.Parameters.Add(new SqlParameter("@CASE_NO", caseno));
            sqlcmd2.Parameters.Add(new SqlParameter("@batchno", batchno));
            sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_INTER_ID", internalid));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd2);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }


        /// <summary>
        /// AML_HQ_Import, AML_HQ_Work 的 GROUP_NO 儲分組後最小案件編號
        /// </summary>
        /// <param name="corpNo"></param>
        /// <param name="caseProcessStatus"></param>
        /// <returns></returns>        
        public static bool UpdateHQtable(string batchNo, string interID, string corpNo, string caseProcessStatus)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
            UPDATE [dbo].[AML_HQ_Import] SET GROUP_NO = 
            (
                SELECT TOP 1 CASE_NO FROM [dbo].[AML_HQ_Work]
                WHERE HCOP_BATCH_NO = @batchNo
                AND HCOP_INTER_ID = @interID
                AND HCOP_HEADQUATERS_CORP_NO = @corpNo 
                AND CaseProcess_Status=@caseProcessStatus 
                AND CaseProcess_Status not in (2,23,24,25) 
                AND CASE_NO IS NOT NULL
                ORDER BY CASE_NO
            )
            WHERE HCOP_BATCH_NO = @batchNo
            AND HCOP_INTER_ID = @interID 
            AND HCOP_HEADQUATERS_CORP_NO = @corpNo 
            ";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@batchNo", batchNo));
            sqlcmd.Parameters.Add(new SqlParameter("@interID", interID));
            sqlcmd.Parameters.Add(new SqlParameter("@corpNo", corpNo));
            sqlcmd.Parameters.Add(new SqlParameter("@caseProcessStatus", caseProcessStatus));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            SqlCommand sqlcmd2 = new SqlCommand();
            sqlcmd2.CommandText = @"
            UPDATE [dbo].[AML_HQ_Work] SET GROUP_NO = 
            (
                SELECT TOP 1 CASE_NO FROM [dbo].[AML_HQ_Work]
                WHERE HCOP_BATCH_NO = @batchNo
                AND HCOP_INTER_ID = @interID
                AND HCOP_HEADQUATERS_CORP_NO = @corpNo 
                AND CaseProcess_Status=@caseProcessStatus 
                AND CaseProcess_Status not in (2,23,24,25) 
                AND CASE_NO IS NOT NULL
                ORDER BY CASE_NO
            )
            WHERE HCOP_BATCH_NO = @batchNo
            AND HCOP_INTER_ID = @interID 
            AND HCOP_HEADQUATERS_CORP_NO = @corpNo 
            AND CaseProcess_Status=@caseProcessStatus 
            AND CaseProcess_Status not in (2,23,24,25)
            ";
            sqlcmd2.CommandType = CommandType.Text;
            sqlcmd2.Parameters.Add(new SqlParameter("@batchNo", batchNo));
            sqlcmd2.Parameters.Add(new SqlParameter("@interID", interID));
            sqlcmd2.Parameters.Add(new SqlParameter("@corpNo", corpNo));
            sqlcmd2.Parameters.Add(new SqlParameter("@caseProcessStatus", caseProcessStatus));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd2);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// AML_HQ_Manager_Import, AML_HQ_Manager_Work 的 GROUP_NO 壓成和 AML_HQ_Import, AML_HQ_Work 一致
        /// </summary>
        /// <param name="createDate"></param>
        /// <returns></returns>        
        public static bool UpdateHQManagertable(string createDate)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
            UPDATE A
            SET A.GROUP_NO = B.GROUP_NO
            FROM [dbo].[AML_HQ_Manager_Import] AS A
            LEFT JOIN [dbo].[AML_HQ_Import] AS B ON A.CASE_NO = B.CASE_NO AND A.Create_Date = B.Create_Date
            WHERE A.Create_Date = @createDate";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@createDate", createDate));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            SqlCommand sqlcmd2 = new SqlCommand();
            sqlcmd2.CommandText = @"
            UPDATE A
            SET A.GROUP_NO = B.GROUP_NO
            FROM [dbo].[AML_HQ_Manager_Work] AS A
            LEFT JOIN [dbo].[AML_HQ_Work] AS B ON A.CASE_NO = B.CASE_NO AND A.Create_Date = B.Create_Date
            WHERE A.Create_Date = @createDate";
            sqlcmd2.CommandType = CommandType.Text;
            sqlcmd2.Parameters.Add(new SqlParameter("@createDate", createDate));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd2);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }
            return result;
        }

        /// <summary>
        /// 分組案件
        /// </summary>
        /// <param name="batchNo"></param>
        /// <param name="interID"></param>
        /// <param name="corpNo"></param>
        /// <param name="caseProcessStatus"></param>
        /// <returns></returns>        
        public static DataTable GetCases(string batchNo, string interID, string corpNo, string caseProcessStatus)
        {
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
        SELECT CASE_NO, GROUP_NO FROM [dbo].[AML_HQ_Work]
                        WHERE HCOP_BATCH_NO = @batchNo
                    AND HCOP_INTER_ID = @interID 
                    AND HCOP_HEADQUATERS_CORP_NO = @corpNo 
                    AND CaseProcess_Status=@caseProcessStatus 
                    AND CaseProcess_Status not in (2,23,24,25)";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@batchNo", batchNo));
            sqlcmd.Parameters.Add(new SqlParameter("@interID", interID));
            sqlcmd.Parameters.Add(new SqlParameter("@corpNo", corpNo));
            sqlcmd.Parameters.Add(new SqlParameter("@caseProcessStatus", caseProcessStatus));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 更新分公司work資料表
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static bool UpdateBranchworktable(string id)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
            UPDATE [dbo].[AML_BRCH_Work] SET BRCH_ExportFileFlag = 0,BRCH_LastExportTime=GETDATE() WHERE ID = @id";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@id", id));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }
            return result;
        }



        /// <summary>
        /// 更新分公司work資料表
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static bool UpdateHQworktable(string id)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
            UPDATE [dbo].[AML_HQ_Work] SET AML_ExportFileFlag = 0,AML_LastExportTime=GETDATE() WHERE ID = @id";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@id", id));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }
            return result;
        }




        public static bool InsertCdatatableImport(DataRow row, string flag)
        {
            bool result = false;
            DateTime now = DateTime.Now;

            

            SqlCommand sqlcmd2 = new SqlCommand();

            if (flag == "N")// 新版程式碼
            {
                #region 新版程式碼
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                sqlcmd2.CommandText = string.Format(@"
    INSERT INTO [{0}].[dbo].[AML_Cdata_Import]
               ([FileName]
               ,[Datadate]
               ,[CustomerID]
               ,[CustomerEnglishName]
               ,[CustomerChineseName]
               ,[AMLSegment]
               ,[AMLRiskRanking]
               ,[AMLNextReviewDate]
               ,[BlackListHitFlag]
               ,[PEPListHitFlag]
               ,[NNListHitFlag]
               ,[Incorporated]
               ,[IncorporatedDate]
               ,[LastUpdateMaker]
               ,[LastUpdateChecker]
               ,[LastUpdateBranch]
               ,[LastUpdateDate]
               ,[LastUpdateSourceSystem]
               ,[HomeBranch]
               ,[Reason]
               ,[WarningFlag]
               ,[FiledSAR]
               ,[CreditCardBlockCode]
               ,[InternationalOrgPEP]
               ,[DomesticPEP]
               ,[ForeignPEPStakeholder]
               ,[InternationalOrgPEPStakeholder]
               ,[DomesticPEPStakeholder]
               ,[GroupInformationSharingNameListflag]
               ,[Filler]
               ,[Create_Time]
               ,[Create_User]
               ,[Create_Date]
               ,[Dormant_Flag]
               ,[Dormant_Date]
               ,[Incorporated_Source_System]
               ,[AML_Last_Review_Date]
               ,[Risk_Factor_PEP]
               ,[Risk_Factor_RP_PEP]
               ,[Internal_List_Flag]
               ,[High_Risk_Flag_Because_Rpty]
               ,[High_Risk_Flag])
    VALUES
               (@FileName
               ,@Datadate
               ,@CustomerID
               ,@CustomerEnglishName
               ,@CustomerChineseName
               ,@AMLSegment
               ,@AMLRiskRanking
               ,@AMLNextReviewDate
               ,@BlackListHitFlag
               ,@PEPListHitFlag
               ,@NNListHitFlag
               ,@Incorporated
               ,@IncorporatedDate
               ,@LastUpdateMaker
               ,@LastUpdateChecker
               ,@LastUpdateBranch
               ,@LastUpdateDate
               ,@LastUpdateSourceSystem
               ,@HomeBranch
               ,@Reason
               ,@WarningFlag
               ,@FiledSAR
               ,@CreditCardBlockCode
               ,@InternationalOrgPEP
               ,@DomesticPEP
               ,@ForeignPEPStakeholder
               ,@InternationalOrgPEPStakeholder
               ,@DomesticPEPStakeholder
               ,@GroupInformationSharingNameListflag
               ,@Filler
               ,@Create_Time
               ,@Create_User
               ,@Create_Date
               ,@Dormant_Flag
               ,@Dormant_Date
               ,@Incorporated_Source_System
               ,@AML_Last_Review_Date
               ,@Risk_Factor_PEP
               ,@Risk_Factor_RP_PEP
               ,@Internal_List_Flag
               ,@High_Risk_Flag_Because_Rpty
               ,@High_Risk_Flag)
                ", UtilHelper.GetAppSettings("DB_KeyinGUI"));
                #endregion
            }
            else
            {
                #region 舊版程式碼
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                sqlcmd2.CommandText = string.Format(@"
    INSERT INTO [{0}].[dbo].[AML_Cdata_Import]
               ([FileName]
               ,[Datadate]
               ,[CustomerID]
               ,[CustomerEnglishName]
               ,[CustomerChineseName]
               ,[AMLSegment]
               ,[AMLRiskRanking]
               ,[AMLNextReviewDate]
               ,[BlackListHitFlag]
               ,[PEPListHitFlag]
               ,[NNListHitFlag]
               ,[Incorporated]
               ,[IncorporatedDate]
               ,[LastUpdateMaker]
               ,[LastUpdateChecker]
               ,[LastUpdateBranch]
               ,[LastUpdateDate]
               ,[LastUpdateSourceSystem]
               ,[HomeBranch]
               ,[Reason]
               ,[WarningFlag]
               ,[FiledSAR]
               ,[CreditCardBlockCode]
               ,[InternationalOrgPEP]
               ,[DomesticPEP]
               ,[ForeignPEPStakeholder]
               ,[InternationalOrgPEPStakeholder]
               ,[DomesticPEPStakeholder]
               ,[GroupInformationSharingNameListflag]
               ,[Filler]
               ,[Create_Time]
               ,[Create_User]
               ,[Create_Date])
    VALUES
               (@FileName
               ,@Datadate
               ,@CustomerID
               ,@CustomerEnglishName
               ,@CustomerChineseName
               ,@AMLSegment
               ,@AMLRiskRanking
               ,@AMLNextReviewDate
               ,@BlackListHitFlag
               ,@PEPListHitFlag
               ,@NNListHitFlag
               ,@Incorporated
               ,@IncorporatedDate
               ,@LastUpdateMaker
               ,@LastUpdateChecker
               ,@LastUpdateBranch
               ,@LastUpdateDate
               ,@LastUpdateSourceSystem
               ,@HomeBranch
               ,@Reason
               ,@WarningFlag
               ,@FiledSAR
               ,@CreditCardBlockCode
               ,@InternationalOrgPEP
               ,@DomesticPEP
               ,@ForeignPEPStakeholder
               ,@InternationalOrgPEPStakeholder
               ,@DomesticPEPStakeholder
               ,@GroupInformationSharingNameListflag
               ,@Filler
               ,@Create_Time
               ,@Create_User
               ,@Create_Date)
                ", UtilHelper.GetAppSettings("DB_KeyinGUI"));
                #endregion
            }

            sqlcmd2.CommandType = CommandType.Text;
            sqlcmd2.Parameters.Add(new SqlParameter("@FileName", row["FileName"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@Datadate", row["Datadate"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@CustomerID", row["CustomerID"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@CustomerEnglishName", row["CustomerEnglishName"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@CustomerChineseName", row["CustomerChineseName"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@AMLSegment", row["AMLSegment"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@AMLRiskRanking", row["AMLRiskRanking"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@AMLNextReviewDate", row["AMLNextReviewDate"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@BlackListHitFlag", row["BlackListHitFlag"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@PEPListHitFlag", row["PEPListHitFlag"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@NNListHitFlag", row["NNListHitFlag"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@Incorporated", row["Incorporated"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@IncorporatedDate", row["IncorporatedDate"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@LastUpdateMaker", row["LastUpdateMaker"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@LastUpdateChecker", row["LastUpdateChecker"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@LastUpdateBranch", row["LastUpdateBranch"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@LastUpdateDate", row["LastUpdateDate"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@LastUpdateSourceSystem", row["LastUpdateSourceSystem"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@HomeBranch", row["HomeBranch"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@Reason", row["Reason"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@WarningFlag", row["WarningFlag"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@FiledSAR", row["FiledSAR"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@CreditCardBlockCode", row["CreditCardBlockCode"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@InternationalOrgPEP", row["InternationalOrgPEP"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@DomesticPEP", row["DomesticPEP"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@ForeignPEPStakeholder", row["ForeignPEPStakeholder"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@InternationalOrgPEPStakeholder", row["InternationalOrgPEPStakeholder"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@DomesticPEPStakeholder", row["DomesticPEPStakeholder"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@GroupInformationSharingNameListflag", row["GroupInformationSharingNameListflag"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@Filler", row["Filler"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@Create_Time", now.ToString("HHmmss")));
            sqlcmd2.Parameters.Add(new SqlParameter("@Create_User", "CSIP_System"));
            sqlcmd2.Parameters.Add(new SqlParameter("@Create_Date", now.ToString("yyyyMMdd")));
            if (flag == "N")// 新版程式碼
            {
                sqlcmd2.Parameters.Add(new SqlParameter("@Dormant_Flag", row["Dormant_Flag"].ToString()));
                sqlcmd2.Parameters.Add(new SqlParameter("@Dormant_Date", row["Dormant_Date"].ToString()));
                sqlcmd2.Parameters.Add(new SqlParameter("@Incorporated_Source_System", row["Incorporated_Source_System"].ToString()));
                sqlcmd2.Parameters.Add(new SqlParameter("@AML_Last_Review_Date", row["AML_Last_Review_Date"].ToString()));
                sqlcmd2.Parameters.Add(new SqlParameter("@Risk_Factor_PEP", row["Risk_Factor_PEP"].ToString()));
                sqlcmd2.Parameters.Add(new SqlParameter("@Risk_Factor_RP_PEP", row["Risk_Factor_RP_PEP"].ToString()));
                sqlcmd2.Parameters.Add(new SqlParameter("@Internal_List_Flag", row["Internal_List_Flag"].ToString()));
                sqlcmd2.Parameters.Add(new SqlParameter("@High_Risk_Flag_Because_Rpty", row["High_Risk_Flag_Because_Rpty"].ToString()));
                sqlcmd2.Parameters.Add(new SqlParameter("@High_Risk_Flag", row["High_Risk_Flag"].ToString()));
            }

            try
            {
                DataSet resultSet2 = BRFORM_COLUMN.SearchOnDataSet(sqlcmd2);
                if (resultSet2 != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;

        }



        public static bool InsertCdatatable(DataRow row)
        {
            bool result = false;
            DateTime now = DateTime.Now;


            SqlCommand sqlcmd2 = new SqlCommand();
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            sqlcmd2.CommandText = string.Format(@"
INSERT INTO [{0}].[dbo].[AML_Cdata_Import]
           ([FileName]
           ,[Datadate]
           ,[CustomerID]
           ,[CustomerEnglishName]
           ,[CustomerChineseName]
           ,[AMLSegment]
           ,[AMLRiskRanking]
           ,[AMLNextReviewDate]
           ,[BlackListHitFlag]
           ,[PEPListHitFlag]
           ,[NNListHitFlag]
           ,[Incorporated]
           ,[IncorporatedDate]
           ,[LastUpdateMaker]
           ,[LastUpdateChecker]
           ,[LastUpdateBranch]
           ,[LastUpdateDate]
           ,[LastUpdateSourceSystem]
           ,[HomeBranch]
           ,[Reason]
           ,[WarningFlag]
           ,[FiledSAR]
           ,[CreditCardBlockCode]
           ,[InternationalOrgPEP]
           ,[DomesticPEP]
           ,[ForeignPEPStakeholder]
           ,[InternationalOrgPEPStakeholder]
           ,[DomesticPEPStakeholder]
           ,[GroupInformationSharingNameListflag]
           ,[Filler]
           ,[Create_Time]
           ,[Create_User]
           ,[Create_Date])
VALUES
           (@FileName
           ,@Datadate
           ,@CustomerID
           ,@CustomerEnglishName
           ,@CustomerChineseName
           ,@AMLSegment
           ,@AMLRiskRanking
           ,@AMLNextReviewDate
           ,@BlackListHitFlag
           ,@PEPListHitFlag
           ,@NNListHitFlag
           ,@Incorporated
           ,@IncorporatedDate
           ,@LastUpdateMaker
           ,@LastUpdateChecker
           ,@LastUpdateBranch
           ,@LastUpdateDate
           ,@LastUpdateSourceSystem
           ,@HomeBranch
           ,@Reason
           ,@WarningFlag
           ,@FiledSAR
           ,@CreditCardBlockCode
           ,@InternationalOrgPEP
           ,@DomesticPEP
           ,@ForeignPEPStakeholder
           ,@InternationalOrgPEPStakeholder
           ,@DomesticPEPStakeholder
           ,@GroupInformationSharingNameListflag
           ,@Filler
           ,@Create_Time
           ,@Create_User
           ,@Create_Date)
            ", UtilHelper.GetAppSettings("DB_KeyinGUI"));

            sqlcmd2.CommandType = CommandType.Text;
            sqlcmd2.Parameters.Add(new SqlParameter("@FileName", row["FileName"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@Datadate", row["Datadate"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@CustomerID", row["CustomerID"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@CustomerEnglishName", row["CustomerEnglishName"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@CustomerChineseName", row["CustomerChineseName"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@AMLSegment", row["AMLSegment"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@AMLRiskRanking", row["AMLRiskRanking"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@AMLNextReviewDate", row["AMLNextReviewDate"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@BlackListHitFlag", row["BlackListHitFlag"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@PEPListHitFlag", row["PEPListHitFlag"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@NNListHitFlag", row["NNListHitFlag"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@Incorporated", row["Incorporated"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@IncorporatedDate", row["IncorporatedDate"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@LastUpdateMaker", row["LastUpdateMaker"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@LastUpdateChecker", row["LastUpdateChecker"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@LastUpdateBranch", row["LastUpdateBranch"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@LastUpdateDate", row["LastUpdateDate"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@LastUpdateSourceSystem", row["LastUpdateSourceSystem"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@HomeBranch", row["HomeBranch"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@Reason", row["Reason"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@WarningFlag", row["WarningFlag"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@FiledSAR", row["FiledSAR"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@CreditCardBlockCode", row["CreditCardBlockCode"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@InternationalOrgPEP", row["InternationalOrgPEP"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@DomesticPEP", row["DomesticPEP"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@ForeignPEPStakeholder", row["ForeignPEPStakeholder"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@InternationalOrgPEPStakeholder", row["InternationalOrgPEPStakeholder"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@DomesticPEPStakeholder", row["DomesticPEPStakeholder"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@GroupInformationSharingNameListflag", row["GroupInformationSharingNameListflag"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@Filler", row["Filler"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@Create_Time", now.ToString("HHmmss")));
            sqlcmd2.Parameters.Add(new SqlParameter("@Create_User", "CSIP_System"));
            sqlcmd2.Parameters.Add(new SqlParameter("@Create_Date", now.ToString("yyyyMMdd")));


            try
            {
                DataSet resultSet2 = BRFORM_COLUMN.SearchOnDataSet(sqlcmd2);
                if (resultSet2 != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }



            SqlCommand sqlcmd = new SqlCommand();
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            sqlcmd.CommandText = string.Format(@"
            INSERT INTO [{0}].[dbo].[AML_Cdata_Work]
           ([FileName]
           ,[Datadate]
           ,[CustomerID]
           ,[CustomerEnglishName]
           ,[CustomerChineseName]
           ,[AMLSegment]
           ,[AMLRiskRanking]
           ,[AMLNextReviewDate]
           ,[BlackListHitFlag]
           ,[PEPListHitFlag]
           ,[NNListHitFlag]
           ,[Incorporated]
           ,[IncorporatedDate]
           ,[LastUpdateMaker]
           ,[LastUpdateChecker]
           ,[LastUpdateBranch]
           ,[LastUpdateDate]
           ,[LastUpdateSourceSystem]
           ,[HomeBranch]
           ,[Reason]
           ,[WarningFlag]
           ,[FiledSAR]
           ,[CreditCardBlockCode]
           ,[InternationalOrgPEP]
           ,[DomesticPEP]
           ,[ForeignPEPStakeholder]
           ,[InternationalOrgPEPStakeholder]
           ,[DomesticPEPStakeholder]
           ,[GroupInformationSharingNameListflag]
           ,[Filler]
           ,[Create_Time]
           ,[Create_User]
           ,[Create_Date])
     VALUES
           (@FileName
           ,@Datadate
           ,@CustomerID
           ,@CustomerEnglishName
           ,@CustomerChineseName
           ,@AMLSegment
           ,@AMLRiskRanking
           ,@AMLNextReviewDate
           ,@BlackListHitFlag
           ,@PEPListHitFlag
           ,@NNListHitFlag
           ,@Incorporated
           ,@IncorporatedDate
           ,@LastUpdateMaker
           ,@LastUpdateChecker
           ,@LastUpdateBranch
           ,@LastUpdateDate
           ,@LastUpdateSourceSystem
           ,@HomeBranch
           ,@Reason
           ,@WarningFlag
           ,@FiledSAR
           ,@CreditCardBlockCode
           ,@InternationalOrgPEP
           ,@DomesticPEP
           ,@ForeignPEPStakeholder
           ,@InternationalOrgPEPStakeholder
           ,@DomesticPEPStakeholder
           ,@GroupInformationSharingNameListflag
           ,@Filler
           ,@Create_Time
           ,@Create_User
           ,@Create_Date)

            ", UtilHelper.GetAppSettings("DB_KeyinGUI"));

            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@FileName", row["FileName"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@Datadate", row["Datadate"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@CustomerID", row["CustomerID"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@CustomerEnglishName", row["CustomerEnglishName"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@CustomerChineseName", row["CustomerChineseName"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@AMLSegment", row["AMLSegment"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@AMLRiskRanking", row["AMLRiskRanking"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@AMLNextReviewDate", row["AMLNextReviewDate"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@BlackListHitFlag", row["BlackListHitFlag"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@PEPListHitFlag", row["PEPListHitFlag"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@NNListHitFlag", row["NNListHitFlag"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@Incorporated", row["Incorporated"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@IncorporatedDate", row["IncorporatedDate"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@LastUpdateMaker", row["LastUpdateMaker"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@LastUpdateChecker", row["LastUpdateChecker"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@LastUpdateBranch", row["LastUpdateBranch"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@LastUpdateDate", row["LastUpdateDate"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@LastUpdateSourceSystem", row["LastUpdateSourceSystem"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@HomeBranch", row["HomeBranch"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@Reason", row["Reason"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@WarningFlag", row["WarningFlag"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@FiledSAR", row["FiledSAR"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@CreditCardBlockCode", row["CreditCardBlockCode"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@InternationalOrgPEP", row["InternationalOrgPEP"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@DomesticPEP", row["DomesticPEP"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@ForeignPEPStakeholder", row["ForeignPEPStakeholder"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@InternationalOrgPEPStakeholder", row["InternationalOrgPEPStakeholder"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@DomesticPEPStakeholder", row["DomesticPEPStakeholder"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@GroupInformationSharingNameListflag", row["GroupInformationSharingNameListflag"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@Filler", row["Filler"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_Time", now.ToString("HHmmss")));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_User", "CSIP_System"));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_Date", now.ToString("yyyyMMdd")));


            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;

        }


        public static bool UpdateCdatatable(DataRow row)
        {
            bool result = false;
            DateTime now = DateTime.Now;

            SqlCommand sqlcmd2 = new SqlCommand();
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            sqlcmd2.CommandText = string.Format(@"
            UPDATE [{0}].[dbo].[AML_Cdata_Import]
               SET [FileName] = @FileName
              ,[Datadate] = @Datadate
              ,[CustomerID] = @CustomerID
              ,[CustomerEnglishName] = @CustomerEnglishName
              ,[CustomerChineseName] = @CustomerChineseName
              ,[AMLSegment] = @AMLSegment
              ,[AMLRiskRanking] = @AMLRiskRanking
              ,[AMLNextReviewDate] = @AMLNextReviewDate
              ,[BlackListHitFlag] = @BlackListHitFlag
              ,[PEPListHitFlag] = @PEPListHitFlag
              ,[NNListHitFlag] = @NNListHitFlag
              ,[Incorporated] = @Incorporated
              ,[IncorporatedDate] = @IncorporatedDate
              ,[LastUpdateMaker] = @LastUpdateMaker
              ,[LastUpdateChecker] = @LastUpdateChecker
              ,[LastUpdateBranch] = @LastUpdateBranch
              ,[LastUpdateDate] = @LastUpdateDate
              ,[LastUpdateSourceSystem] = @LastUpdateSourceSystem
              ,[HomeBranch] = @HomeBranch
              ,[Reason] = @Reason
              ,[WarningFlag] = @WarningFlag
              ,[FiledSAR] = @FiledSAR
              ,[CreditCardBlockCode] =@CreditCardBlockCode
              ,[InternationalOrgPEP] = @InternationalOrgPEP
              ,[DomesticPEP] = @DomesticPEP
              ,[ForeignPEPStakeholder] = @ForeignPEPStakeholder
              ,[InternationalOrgPEPStakeholder] = @InternationalOrgPEPStakeholder
              ,[DomesticPEPStakeholder] = @DomesticPEPStakeholder
              ,[GroupInformationSharingNameListflag] = @GroupInformationSharingNameListflag
              ,[Filler] = @Filler
              ,[Create_Time] = @Create_Time
              ,[Create_User] = @Create_User
              ,[Create_Date] = @Create_Date
             WHERE CustomerID = @CustomerID
            ", UtilHelper.GetAppSettings("DB_KeyinGUI"));

            sqlcmd2.CommandType = CommandType.Text;
            sqlcmd2.Parameters.Add(new SqlParameter("@FileName", row["FileName"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@Datadate", row["Datadate"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@CustomerID", row["CustomerID"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@CustomerEnglishName", row["CustomerEnglishName"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@CustomerChineseName", row["CustomerChineseName"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@AMLSegment", row["AMLSegment"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@AMLRiskRanking", row["AMLRiskRanking"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@AMLNextReviewDate", row["AMLNextReviewDate"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@BlackListHitFlag", row["BlackListHitFlag"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@PEPListHitFlag", row["PEPListHitFlag"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@NNListHitFlag", row["NNListHitFlag"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@Incorporated", row["Incorporated"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@IncorporatedDate", row["IncorporatedDate"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@LastUpdateMaker", row["LastUpdateMaker"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@LastUpdateChecker", row["LastUpdateChecker"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@LastUpdateBranch", row["LastUpdateBranch"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@LastUpdateDate", row["LastUpdateDate"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@LastUpdateSourceSystem", row["LastUpdateSourceSystem"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@HomeBranch", row["HomeBranch"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@Reason", row["Reason"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@WarningFlag", row["WarningFlag"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@FiledSAR", row["FiledSAR"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@CreditCardBlockCode", row["CreditCardBlockCode"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@InternationalOrgPEP", row["InternationalOrgPEP"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@DomesticPEP", row["DomesticPEP"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@ForeignPEPStakeholder", row["ForeignPEPStakeholder"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@InternationalOrgPEPStakeholder", row["InternationalOrgPEPStakeholder"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@DomesticPEPStakeholder", row["DomesticPEPStakeholder"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@GroupInformationSharingNameListflag", row["GroupInformationSharingNameListflag"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@Filler", row["Filler"].ToString()));
            sqlcmd2.Parameters.Add(new SqlParameter("@Create_Time", now.ToString("HHmmss")));
            sqlcmd2.Parameters.Add(new SqlParameter("@Create_User", "CSIP_System"));
            sqlcmd2.Parameters.Add(new SqlParameter("@Create_Date", now.ToString("yyyyMMdd")));


            try
            {
                DataSet resultSet2 = BRFORM_COLUMN.SearchOnDataSet(sqlcmd2);
                if (resultSet2 != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }



            SqlCommand sqlcmd = new SqlCommand();
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            sqlcmd.CommandText = string.Format(@"
            UPDATE [{0}].[dbo].[AML_Cdata_Work]
               SET [FileName] = @FileName
              ,[Datadate] = @Datadate
              ,[CustomerID] = @CustomerID
              ,[CustomerEnglishName] = @CustomerEnglishName
              ,[CustomerChineseName] = @CustomerChineseName
              ,[AMLSegment] = @AMLSegment
              ,[AMLRiskRanking] = @AMLRiskRanking
              ,[AMLNextReviewDate] = @AMLNextReviewDate
              ,[BlackListHitFlag] = @BlackListHitFlag
              ,[PEPListHitFlag] = @PEPListHitFlag
              ,[NNListHitFlag] = @NNListHitFlag
              ,[Incorporated] = @Incorporated
              ,[IncorporatedDate] = @IncorporatedDate
              ,[LastUpdateMaker] = @LastUpdateMaker
              ,[LastUpdateChecker] = @LastUpdateChecker
              ,[LastUpdateBranch] = @LastUpdateBranch
              ,[LastUpdateDate] = @LastUpdateDate
              ,[LastUpdateSourceSystem] = @LastUpdateSourceSystem
              ,[HomeBranch] = @HomeBranch
              ,[Reason] = @Reason
              ,[WarningFlag] = @WarningFlag
              ,[FiledSAR] = @FiledSAR
              ,[CreditCardBlockCode] =@CreditCardBlockCode
              ,[InternationalOrgPEP] = @InternationalOrgPEP
              ,[DomesticPEP] = @DomesticPEP
              ,[ForeignPEPStakeholder] = @ForeignPEPStakeholder
              ,[InternationalOrgPEPStakeholder] = @InternationalOrgPEPStakeholder
              ,[DomesticPEPStakeholder] = @DomesticPEPStakeholder
              ,[GroupInformationSharingNameListflag] = @GroupInformationSharingNameListflag
              ,[Filler] = @Filler
              ,[Create_Time] = @Create_Time
              ,[Create_User] = @Create_User
              ,[Create_Date] = @Create_Date
             WHERE CustomerID = @CustomerID

            ", UtilHelper.GetAppSettings("DB_KeyinGUI"));

            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@FileName", row["FileName"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@Datadate", row["Datadate"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@CustomerID", row["CustomerID"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@CustomerEnglishName", row["CustomerEnglishName"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@CustomerChineseName", row["CustomerChineseName"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@AMLSegment", row["AMLSegment"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@AMLRiskRanking", row["AMLRiskRanking"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@AMLNextReviewDate", row["AMLNextReviewDate"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@BlackListHitFlag", row["BlackListHitFlag"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@PEPListHitFlag", row["PEPListHitFlag"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@NNListHitFlag", row["NNListHitFlag"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@Incorporated", row["Incorporated"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@IncorporatedDate", row["IncorporatedDate"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@LastUpdateMaker", row["LastUpdateMaker"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@LastUpdateChecker", row["LastUpdateChecker"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@LastUpdateBranch", row["LastUpdateBranch"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@LastUpdateDate", row["LastUpdateDate"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@LastUpdateSourceSystem", row["LastUpdateSourceSystem"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@HomeBranch", row["HomeBranch"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@Reason", row["Reason"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@WarningFlag", row["WarningFlag"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@FiledSAR", row["FiledSAR"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@CreditCardBlockCode", row["CreditCardBlockCode"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@InternationalOrgPEP", row["InternationalOrgPEP"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@DomesticPEP", row["DomesticPEP"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@ForeignPEPStakeholder", row["ForeignPEPStakeholder"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@InternationalOrgPEPStakeholder", row["InternationalOrgPEPStakeholder"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@DomesticPEPStakeholder", row["DomesticPEPStakeholder"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@GroupInformationSharingNameListflag", row["GroupInformationSharingNameListflag"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@Filler", row["Filler"].ToString()));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_Time", now.ToString("HHmmss")));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_User", "CSIP_System"));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_Date", now.ToString("yyyyMMdd")));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;

        }



        public static bool UpdateBRCHtable(string caseno, string batchno, string internalid)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
            UPDATE [dbo].[AML_BRCH_Work] SET CASE_NO = @CASE_NO WHERE BRCH_BATCH_NO = @BRCH_BATCH_NO and BRCH_INTER_ID=@BRCH_INTER_ID";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", caseno));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_BATCH_NO", batchno));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_INTER_ID", internalid));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }


            SqlCommand sqlcmd2 = new SqlCommand();
            sqlcmd2.CommandText = @"
            UPDATE [dbo].[AML_BRCH_Import] SET CASE_NO = @CASE_NO WHERE BRCH_BATCH_NO = @BRCH_BATCH_NO and BRCH_INTER_ID=@BRCH_INTER_ID";
            sqlcmd2.CommandType = CommandType.Text;
            sqlcmd2.Parameters.Add(new SqlParameter("@CASE_NO", caseno));
            sqlcmd2.Parameters.Add(new SqlParameter("@BRCH_BATCH_NO", batchno));
            sqlcmd2.Parameters.Add(new SqlParameter("@BRCH_INTER_ID", internalid));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd2);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }


        public static bool DeleteDulidateBRCHtable()
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            sqlcmd.CommandText = string.Format(@"
            delete [{0}].[dbo].[AML_BRCH_Work] where BRCH_BRCH_NO in (
            SELECT BRCH_BRCH_NO
            FROM [{0}].[dbo].[AML_BRCH_Work] WHERE BRCH_BRCH_NO IN ( SELECT HCOP_HEADQUATERS_CORP_NO
            FROM [{0}].[dbo].[AML_HQ_Work] )
            )
            ", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            sqlcmd.CommandType = CommandType.Text;

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }


            return result;
        }


        public static bool UpdateManagertable(string caseno, string batchno, string internalid)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"
            UPDATE [dbo].[AML_HQ_Manager_Work] SET CASE_NO = @CASE_NO WHERE HCOP_BATCH_NO = @HCOP_BATCH_NO and HCOP_INTER_ID=@HCOP_INTER_ID";
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", caseno));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_BATCH_NO", batchno));
            sqlcmd.Parameters.Add(new SqlParameter("@HCOP_INTER_ID", internalid));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }


            SqlCommand sqlcmd2 = new SqlCommand();
            sqlcmd2.CommandText = @"
            UPDATE [dbo].[AML_HQ_Manager_Import] SET CASE_NO = @CASE_NO WHERE HCOP_BATCH_NO = @HCOP_BATCH_NO and HCOP_INTER_ID=@HCOP_INTER_ID";
            sqlcmd2.CommandType = CommandType.Text;
            sqlcmd2.Parameters.Add(new SqlParameter("@CASE_NO", caseno));
            sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_BATCH_NO", batchno));
            sqlcmd2.Parameters.Add(new SqlParameter("@HCOP_INTER_ID", internalid));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd2);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }


        /// <summary>
        /// 取得 AML_BRCH_Work 資訊 BY BATCH_NO , INTERNAL ID
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static DataTable GetBRCH_WORKDATAByBATCH_NOINTERNAL_ID(string batch_no, string internal_id)
        {
            string sql = "";
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            sql = string.Format(@"
            SELECT top 1 *
            FROM [{0}].[dbo].[AML_BRCH_Work]
            where BRCH_BATCH_NO=@BRCH_BATCH_NO and BRCH_INTER_ID = @BRCH_INTER_ID", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sql;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_BATCH_NO", batch_no));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCH_INTER_ID", internal_id));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }



        public static bool AML_CASE_ACT_LOG(string case_no, string BRCO_BRCH_NO, string BRCO_BRCH_SEQ, string ACT_Type, string ACT_Content)
        {
            bool result = false;
            DateTime now = DateTime.Now;

            SqlCommand sqlcmd = new SqlCommand();
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            sqlcmd.CommandText = string.Format(@"

INSERT INTO [{0}].[dbo].[AML_CASE_ACT_LOG]
           ([CASE_NO]
           ,[BRCO_BRCH_NO]
           ,[BRCO_BRCH_SEQ]
           ,[ACT_Date]
           ,[ACT_Time]
           ,[ACT_Type]
           ,[ACT_UserID]
           ,[ACT_Content])
     VALUES
           (@case_no
           ,@BRCO_BRCH_NO
           ,@BRCO_BRCH_SEQ
           ,@ACT_Date
           ,@ACT_Time
           ,@ACT_Type
           ,@ACT_UserID
           ,@ACT_Content)
", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@case_no", case_no));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCO_BRCH_NO", BRCO_BRCH_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@BRCO_BRCH_SEQ", BRCO_BRCH_SEQ));
            sqlcmd.Parameters.Add(new SqlParameter("@ACT_Date", now.ToString("yyyyMMdd")));
            sqlcmd.Parameters.Add(new SqlParameter("@ACT_Time", now.ToString("HHmmss")));
            sqlcmd.Parameters.Add(new SqlParameter("@ACT_Type", ACT_Type));
            sqlcmd.Parameters.Add(new SqlParameter("@ACT_UserID", "CSIP_SYSTEM"));
            sqlcmd.Parameters.Add(new SqlParameter("@ACT_Content", ACT_Content));


            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }





        public static bool AML_NOTELOG(string case_no, string BRCO_BRCH_NO, string BRCO_BRCH_SEQ, string ACT_Type, string ACT_Content)
        {
            bool result = false;
            DateTime now = DateTime.Now;

            SqlCommand sqlcmd = new SqlCommand();
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            sqlcmd.CommandText = string.Format(@"

            INSERT INTO [{0}].[dbo].[NoteLog]
                       ([NL_CASE_NO]
                       ,[NL_SecondKey]
                       ,[NL_DateTime]
                       ,[NL_User]
                       ,[NL_Type]
                       ,[NL_Value]
                       ,[NL_ShowFlag])
                 VALUES
                       (@NL_CASE_NO
                       ,@NL_SecondKey
                       ,@NL_DateTime
                       ,@NL_User
                       ,@NL_Type
                       ,@NL_Value
                       ,@NL_ShowFlag)
            ", UtilHelper.GetAppSettings("DB_KeyinGUI"));


            List<string> primes = new List<string>(new string[] { "傳送地址條", "MERCHANTAML", "MERCHANTAMLRESULT", "MERCHANTRMM", "MERCHANTRMMRESULT" });
            string _ShowFlag = "0";
            if (!primes.IndexOf(ACT_Type).Equals("-1"))
                _ShowFlag = "1";

            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@NL_CASE_NO", case_no));
            sqlcmd.Parameters.Add(new SqlParameter("@NL_SecondKey", BRCO_BRCH_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@NL_DateTime", now.ToString("yyyy-MM-dd HH:mm:ss")));
            sqlcmd.Parameters.Add(new SqlParameter("@NL_User", "CSIP_SYSTEM"));
            sqlcmd.Parameters.Add(new SqlParameter("@NL_Type", ACT_Type));
            sqlcmd.Parameters.Add(new SqlParameter("@NL_Value", ACT_Content));
            sqlcmd.Parameters.Add(new SqlParameter("@NL_ShowFlag", _ShowFlag));//20200804-RQ-2020-021027-001 批次LOG應不SHOW


            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }




        /// <summary>
        /// 檢查每個案件是否都有總公司資料
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static DataTable checkcase()
        {
            string sql = "";

            //20191125-RQ-2018-015749-002-匯入AML RMM E檔無總公司資料檔！請IT協助的主旨請改成「AML 總公司檔 批次」，內容請改成只檢核當月E檔無總公司資料檔再出EMIAL通知，不要檢核歷史檔
            //sql = @"
            //            select distinct(customerid) from [KeyinGUI].[dbo].[AML_Edata_Work] A LEFT JOIN [KeyinGUI].[dbo].[AML_HQ_Work] B ON A.case_no = B.case_no where b.case_no is null
            //            ";
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            sql = string.Format(@"
                        SELECT DISTINCT(CUSTOMERID) FROM [{0}].[dbo].[AML_Edata_Work] A LEFT JOIN [{0}].[dbo].[AML_HQ_Work] B ON A.CASE_NO = B.CASE_NO 
                        WHERE B.CASE_NO IS NULL AND CONVERT(VARCHAR(6),A.CREATE_DATE ,112) = '", UtilHelper.GetAppSettings("DB_KeyinGUI")) + DateTime.Now.ToString("yyyyMM") + "'  ";

            DataTable result = new DataTable();
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sql;
            sqlcmd.CommandType = CommandType.Text;


            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }


        /// <summary>
        /// 更新 地址條曾經送過的flag
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static bool updateLabelSended(string id)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            sqlcmd.CommandText = string.Format(@"
            update [{0}].[dbo].[AML_HQ_Work] set AddressLabelFlag = 1,AddressLabelFlagTime=GETDATE() where id=@id", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@id", id));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }
            return result;

        }



        /// <summary>
        /// 更新 地址條曾經送過的flag
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static bool updateLabelSended2(string id)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            sqlcmd.CommandText = string.Format(@"
            update [{0}].[dbo].[AML_HQ_Work] set AddressLabelTwoMonthFlag = 1,AddressLabelTwoMonthFlagTime=GETDATE() where id=@id", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@id", id));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }
            return result;

        }

        //20200206-RQ-2019-030155-003
        /// <summary>
        /// 取得WORK_DATE 的工作天數
        /// </summary>
        /// <param name="Days"></param>
        /// <returns></returns>
        public static string GetLastWorkingDateFromWorkDateTable(int Days, string _ExecDate)
        {
            string result = string.Empty;
            //         string sqlCmd = string.Empty;
            //         sqlCmd = @"
            //         SELECT MIN(DATE_TIME) as 'DATE_TIME' FROM (
            //SELECT TOP @Days DATE_TIME FROM WORK_DATE
            //WHERE WORK_FUNCTIONKEY='01' AND CONVERT(VARCHAR(6),DATE_TIME,112) = CONVERT(VARCHAR(6),GETDATE(),112)
            //AND IS_WORKDAY='1'
            //ORDER BY DATE_TIME DESC
            //) A
            //         ";

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = @"DECLARE @Days as Integer SET @Days = " + Days + " SELECT MIN(DATE_TIME) as 'DATE_TIME' FROM ( SELECT TOP (@Days) DATE_TIME FROM WORK_DATE WHERE WORK_FUNCTIONKEY='01' AND CONVERT(VARCHAR(6),DATE_TIME,112) = @ExecDate AND IS_WORKDAY='1' ORDER BY DATE_TIME DESC ) A ";
            sqlcmd.CommandType = CommandType.Text;
            //sqlcmd.Parameters.Add(new SqlParameter("@Days", Days));
            sqlcmd.Parameters.Add(new SqlParameter("@ExecDate", _ExecDate));


            try
            {
                //object resultSet = BRFORM_COLUMN.SearchAValue(sqlCmd, "Connection_CSIP");
                //if (resultSet != null)
                //{
                //    result = resultSet.ToString();
                //}
                DataSet resultSet2 = BRFORM_COLUMN.SearchOnDataSet(sqlcmd, "Connection_CSIP");
                if (resultSet2.Tables[0].Rows.Count > 0)
                {
                    result = resultSet2.Tables[0].Rows[0][0].ToString();
                    // Logging.SaveLog(ELogLayer.BusinessRule, result);
                    Logging.Log(result, LogLayer.BusinessRule);
                }
            }
            catch (Exception ex)
            {
                // Logging.SaveLog(ELogLayer.BusinessRule, ex);
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 20200320-RQ-2019-030155-003
        /// 更新 E檔的FLAG，表示產定審地址條時，郵遞區號為NULL，資料補齊後重下總公司檔，
        /// 因RQ-2019-030155-003RC修改成，如當日無E檔但有總公司時是不會RUN定審地址條的
        /// 故修改此FLAG，屆時即使當日無E檔但有總公司檔時，亦能把資料讀出
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static bool UpdateEdataCREATE_USER(string sCASE_NO, string sStatus)
        {
            bool result = false;

            SqlCommand sqlcmd = new SqlCommand();
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            sqlcmd.CommandText = string.Format(@"
            UPDATE [{0}].[dbo].[AML_Edata_Work] SET CREATE_USER = @Status  WHERE CASE_NO = @CaseNo", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@CaseNo", sCASE_NO));
            if (sStatus.Trim().Equals("F"))//如地址mapping不到郵遞區號，則更新成未發送狀態
            {
                sqlcmd.Parameters.Add(new SqlParameter("@Status", "NonDelivered"));
            }
            else
            {
                //如發送成功，更新回原始狀態
                sqlcmd.Parameters.Add(new SqlParameter("@Status", "CSIP_System"));
            }

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                // Logging.SaveLog(ELogLayer.BusinessRule, ex);
                Logging.Log(ex, LogLayer.BusinessRule);
            }
            return result;

        }

        /// <summary>
        /// 判斷傳入日期是否為工作日
        /// 20200414-RQ-2019-030155-005
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static bool IsWorkingDate(string _date)
        {
            bool result = false;
            string sqlCmd = string.Empty;

            sqlCmd = @"
            SELECT * FROM WORK_DATE WHERE WORK_FUNCTIONKEY='01' AND IS_WORKDAY='1' AND CONVERT(VARCHAR(8),DATE_TIME,112)='" + _date.Trim() + "'";

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlCmd, "Connection_CSIP");
                if (resultSet != null && resultSet.Tables[0].Rows.Count > 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                // Logging.SaveLog(ELogLayer.BusinessRule, ex);
                Logging.Log(ex, LogLayer.BusinessRule);
            }
            return result;

        }

        /// <summary>
        /// 取得上一工作日
        /// 20200415-RQ-2019-030155-005
        /// </summary>
        /// <param name="_ExecuteDate">執行日</param>
        /// <returns></returns>
        public static string GetLastWorkingDate(string _ExecuteDate)
        {
            string result = string.Empty;
            string sqlCmd = string.Empty;
            DateTime _dt1 = DateTime.ParseExact(_ExecuteDate.Trim(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            //20200602 因月初時會抓不到前一工作日
            //sqlCmd = @"
            //WITH _WORKDATETABLE (Sort,DATE_TIME) 
            //AS (
            // SELECT ROW_NUMBER() OVER(ORDER BY DATE_TIME) AS Sort, DATE_TIME
            // FROM WORK_DATE  WHERE SUBSTRING(DATE_TIME,1,6)='" + DateTime.Now.ToString("yyyyMM") + "' AND WORK_FUNCTIONKEY='01'  AND IS_WORKDAY=1 ) SELECT DATE_TIME FROM _WORKDATETABLE  WITH(NOLOCK) WHERE Sort IN  ( SELECT Sort -1  FROM _WORKDATETABLE WITH(NOLOCK)  WHERE DATE_TIME = '" + _ExecuteDate.Trim() + "'  )";
            sqlCmd = @"
            WITH _WORKDATETABLE (Sort,DATE_TIME) 
            AS (
	            SELECT ROW_NUMBER() OVER(ORDER BY DATE_TIME) AS Sort, DATE_TIME
	            FROM WORK_DATE  WHERE SUBSTRING(DATE_TIME,1,6) BETWEEN '" + _dt1.AddMonths(-1).ToString("yyyyMM") + "' AND '" + _dt1.ToString("yyyyMM") + "' AND WORK_FUNCTIONKEY='01'  AND IS_WORKDAY=1 ) SELECT DATE_TIME FROM _WORKDATETABLE  WITH(NOLOCK) WHERE Sort IN  ( SELECT Sort -1  FROM _WORKDATETABLE WITH(NOLOCK)  WHERE DATE_TIME = '" + _ExecuteDate.Trim() + "'  )";
            try
            {
                object resultSet = BRFORM_COLUMN.SearchAValue(sqlCmd, "Connection_CSIP");
                if (resultSet != null)
                {
                    result = resultSet.ToString();

                }
            }
            catch (Exception ex)
            {
                // Logging.SaveLog(ELogLayer.BusinessRule, ex);
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 取得區間日期
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static DataTable GetWork_Date(string _startDate, string _endDate)
        {
            string sql = "";

            sql = @" SELECT DATE_TIME FROM WORK_DATE WHERE WORK_FUNCTIONKEY='01' AND DATE_TIME BETWEEN '" + _startDate.Trim() + "' AND '" + _endDate.Trim() + "'  ";

            DataTable result = new DataTable();
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sql;
            sqlcmd.CommandType = CommandType.Text;

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd, "Connection_CSIP");
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
                // Logging.SaveLog(ELogLayer.BusinessRule, ex);
            }

            return result;
        }

        /// <summary>
        /// 當系統匯入完E、總公司檔之後，系統讀取資料庫的總公司資料表裡撈取尚未做出檔案的資料，產出地址條
        /// BatchJob_SendToRegularAuditMail 呼叫使用
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static DataTable GetRegularAuditMail(string ExecTime)
        {
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            //20200715-修正到期日計算方式
            //20200521-7月RC 若無EMAIL則發送定審地址條
            /*
            sqlcmd.CommandText = @"SELECT a.ID as 'HQ_ID',a.CASE_NO,a.HCOP_HEADQUATERS_CORP_NO as 'CORP_NO', a.HCOP_NAME_CHI as 'REG_NAME',
                                                                a.HCOP_EMAIL as 'EMAIL',Convert(varchar,DATEADD(DD,60,C.Create_Date),112) as 'EXPIRYDATE',CONVERT(VARCHAR,GETDATE(),112) AS 'BATCHDATE','Y' as 'STATUS'
                                                                FROM [KeyinGUI].[dbo].[AML_HQ_Work] a 
                                                                JOIN [dbo].[AML_Edata_Work] c on c.CASE_NO = a.CASE_NO
                                                                WHERE SUBSTRING(a.CASE_NO,9,1) NOT IN ('8','9') 
			                                                    AND (
			                                                            (AddressLabelflag is null  AND c.CREATE_DATE  = @UploadDate )
			                                                            OR (c.CREATE_USER='NonDelivered' AND CONVERT(VARCHAR,DATEADD(MONTH,2,c.CREATE_DATE),112) >= @UploadDate)
			                                                    )   
                                                                AND ISNULL(HCOP_EMAIL,'') <>''
            ";
            */

            //20200803-RQ-2020-021027-001 新增SENDTYPE的條件
            // 20210527 EOS_AML(NOVA) by Ares Dennis
            #region 退版機制
            DataTable dt = new DataTable();
            CSIPCommonModel.BusinessRules_new.BRM_PROPERTY_CODE.GetEnableProperty("01", new string[] { "OLD_VERSION_FLAG" }, ref dt);
            string flag = "";
            if (dt.Rows.Count > 0)
            {
                flag = dt.Rows[0]["PROPERTY_CODE"].ToString();
            }
            #endregion
            if (flag == "N")// 新版程式碼
            {
                //2021/7/6 EOS_AML(NOVA) 每月收E檔作業，針對未結案的統編只能出一份信函 by Ares Dennis
                //20220208_Ares_Jack_新增條件只搜尋統編
                sqlcmd.CommandText = string.Format(@"SELECT a.ID as 'HQ_ID',a.CASE_NO,a.HCOP_HEADQUATERS_CORP_NO as 'CORP_NO', a.HCOP_NAME_CHI as 'REG_NAME',
                                                    a.HCOP_EMAIL as 'EMAIL',Convert(varchar,DATEADD(d,-day(DATEADD(m,2,c.DataDate)),DATEADD(m,1,DATEADD(m,2,c.DataDate))),112) as 'EXPIRYDATE',CONVERT(VARCHAR,GETDATE(),112) AS 'BATCHDATE','Y' as 'STATUS','A' as 'SENDTYPE'  
                                                    FROM [{0}].[dbo].[AML_HQ_Work] a 
                                                    JOIN [dbo].[AML_Edata_Work] c on c.CASE_NO = a.CASE_NO
                                                    WHERE SUBSTRING(a.CASE_NO,9,1) NOT IN ('8','9') AND A.GROUP_NO = A.CASE_NO 
                                           AND (
                                                   (AddressLabelflag is null  AND c.CREATE_DATE  = @UploadDate )
                                                   OR (c.CREATE_USER='NonDelivered' AND CONVERT(VARCHAR,DATEADD(m,2,c.CREATE_DATE),112) >= @UploadDate)
                                           )   
                                                    AND ISNULL(HCOP_EMAIL,'') <>'' 
                                                    AND PATINDEX('[0-9]%', a.HCOP_HEADQUATERS_CORP_NO) > 0
                ", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            }
            else
            {
                //2021/03/09_Ares_Stanley-DB名稱改為變數
                //20220126_Ares_Jack_新增條件只搜尋統編
                sqlcmd.CommandText = string.Format(@"SELECT a.ID as 'HQ_ID',a.CASE_NO,a.HCOP_HEADQUATERS_CORP_NO as 'CORP_NO', a.HCOP_NAME_CHI as 'REG_NAME',
                                                    a.HCOP_EMAIL as 'EMAIL',Convert(varchar,DATEADD(d,-day(DATEADD(m,2,c.DataDate)),DATEADD(m,1,DATEADD(m,2,c.DataDate))),112) as 'EXPIRYDATE',CONVERT(VARCHAR,GETDATE(),112) AS 'BATCHDATE','Y' as 'STATUS','A' as 'SENDTYPE'  
                                                    FROM [{0}].[dbo].[AML_HQ_Work] a 
                                                    JOIN [dbo].[AML_Edata_Work] c on c.CASE_NO = a.CASE_NO
                                                    WHERE SUBSTRING(a.CASE_NO,9,1) NOT IN ('8','9')
			                                        AND (
			                                                (AddressLabelflag is null  AND c.CREATE_DATE  = @UploadDate )
			                                                OR (c.CREATE_USER='NonDelivered' AND CONVERT(VARCHAR,DATEADD(m,2,c.CREATE_DATE),112) >= @UploadDate)
			                                        )   
                                                    AND ISNULL(HCOP_EMAIL,'') <>''
                                                    AND PATINDEX('[0-9]%', a.HCOP_HEADQUATERS_CORP_NO) > 0
                ", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            }

            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@UploadDate", ExecTime));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                //20200831 修改table內容有值才回傳為true
                //if (resultSet != null && resultSet.Tables.Count > 0)
                if (resultSet != null && resultSet.Tables[0].Rows.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                // Logging.SaveLog(ELogLayer.BusinessRule, ex);
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 地址條檢核是否有Mail檔，應排除產檔flag = null的條件
        /// BatchJob_SendAddressLebel 呼叫使用
        /// RQ-2020-021027-003
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static DataTable GetCheckRegularAuditMail(string ExecTime)
        {
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            //20220127_Ares_Jack_新增條件只搜尋統編
            sqlcmd.CommandText = string.Format(@"SELECT a.ID as 'HQ_ID',a.CASE_NO,a.HCOP_HEADQUATERS_CORP_NO as 'CORP_NO', a.HCOP_NAME_CHI as 'REG_NAME',
                                                                a.HCOP_EMAIL as 'EMAIL',Convert(varchar,DATEADD(d,-day(DATEADD(m,2,c.DataDate)),DATEADD(m,1,DATEADD(m,2,c.DataDate))),112) as 'EXPIRYDATE',CONVERT(VARCHAR,GETDATE(),112) AS 'BATCHDATE','Y' as 'STATUS','A' as 'SENDTYPE'  
                                                                FROM [{0}].[dbo].[AML_HQ_Work] a 
                                                                JOIN [dbo].[AML_Edata_Work] c on c.CASE_NO = a.CASE_NO
                                                                WHERE SUBSTRING(a.CASE_NO,9,1) NOT IN ('8','9') 
			                                                    AND (
			                                                                (c.CREATE_DATE  = @ExecDate )
			                                                                OR (c.CREATE_USER='NonDelivered' AND CONVERT(VARCHAR,DATEADD(m,2,c.CREATE_DATE),112) >= @ExecDate )
			                                                             )  
                                                                AND PATINDEX('[0-9]%', a.HCOP_HEADQUATERS_CORP_NO) > 0 
                                                                AND ISNULL(HCOP_EMAIL,'') <>'' ", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@ExecDate", ExecTime));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);

                if (resultSet != null && resultSet.Tables[0].Rows.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                // Logging.SaveLog(ELogLayer.BusinessRule, ex);
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }

        /// <summary>
        /// 判斷取得欲產不合作通知函的資料內，是否有設定email的店家
        /// 20200806-RQ-2020-021027-001
        /// BatchJob_SendTwoMonthAddressLebel 呼叫使用
        /// </summary>
        /// <returns></returns>
        public static bool GetNonCooperationEmail()
        {
            bool isExist = false;

            SqlCommand sqlcmd = new SqlCommand();
            //2021/03/09_Ares_Stanley-DB名稱改為變數
            sqlcmd.CommandText = string.Format(@"
            SELECT CASE_NO,a.ID,d.REG_NAME as 'HCOP_NAME_CHI',HCOP_MAILING_CITY,HCOP_MAILING_ADDR1,HCOP_MAILING_ADDR2,ZIP_CODE 
            FROM [{0}].[dbo].[AML_HQ_Work] a
            LEFT JOIN [{0}].[dbo].[szip] b ON a.HCOP_MAILING_CITY =  b.ZIP_DATA
			JOIN [{0}].[dbo].[AML_Cdata_Work] c ON c.CustomerID = a.HCOP_HEADQUATERS_CORP_NO
	        JOIN [{0}].[dbo].[AML_HCOP_STATUS] d ON d.CORP_NO = a.HCOP_HEADQUATERS_CORP_NO
            where case_no in (
            SELECT case_no
		    FROM [{0}].[dbo].[AML_CASE_DATA] WHERE CLOSE_DATE IS NULL
		    AND CREATE_YM = LEFT(CONVERT(varchar, DateAdd(m,-2,GetDate()),112),6)
		    ) and SUBSTRING(CASE_NO,9,1) NOT IN ('8','9')
			AND c.INCORPORATED<> 'Y' AND d.STATUS='O'  AND ISNULL(a.HCOP_EMAIL,'')<>''
            order by ZIP_CODE ASC
            ", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            sqlcmd.CommandType = CommandType.Text;

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables[0].Rows.Count > 0)//20200831 修改table內容有值才回傳為true
                {
                    isExist = true;
                }

            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
                // Logging.SaveLog(ELogLayer.BusinessRule, ex);
            }

            return isExist;
        }

        /// <summary>
        /// 專案代號:20200031-CSIP作業平台現代化專案 AML NOVA 需求擴增
        /// 功能說明:取AML_CheckLog需要給主機的資料
        /// 作    者:Ares Dennis
        /// 創建時間:2021/07/02        
        /// </summary>        
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static DataTable GetPostToAML_LogData(string endTime)
        {
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();
            //查今天的異動紀錄資料上送主機，同一統編只上送最新一筆
            sqlcmd.CommandText = @"
            SELECT 
            A.CORP_NO, A.MER_NO, LAST_UPD_MAKER, LAST_UPD_CHECKER, LAST_UPD_BRANCH, TRANS_ID, A.MOD_DATE, A.MOD_TIME
            FROM [dbo].[AML_CHECKLOG] A
            INNER JOIN (SELECT CORP_NO, MAX(MOD_TIME) AS MOD_TIME FROM [dbo].[AML_CHECKLOG] WHERE MOD_DATE = CONVERT(VARCHAR(8), @endTime, 112) AND TRANS_ID != 'CSIPJCGQ' GROUP BY CORP_NO) B ON A.CORP_NO = B.CORP_NO AND A.MOD_TIME = B.MOD_TIME
            WHERE A.MOD_DATE = CONVERT(VARCHAR(8), @endTime, 112) AND TRANS_ID != 'CSIPJCGQ' ORDER BY MOD_TIME
            ";

            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@endTime", endTime));

            try
            {
                DataSet resultSet = BRFORM_COLUMN.SearchOnDataSet(sqlcmd);
                if (resultSet != null && resultSet.Tables.Count > 0)
                {
                    result = resultSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
            }

            return result;
        }
    }
}