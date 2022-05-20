//******************************************************************
//*  作        者：陳香琦
//*  功能說明：審查/不合作通知檔函發送記錄
//*  建立日期：2020/06/23
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*Ares Luke          2021/02/22         20200031-CSIP EOS       調整connectionStr方式
//*******************************************************************
using System;
using System.Data;
using System.Data.SqlClient;
using CSIPCommonModel.BusinessRules;
using CSIPCommonModel.EntityLayer_new;
using Framework.Common.Logging;
using Framework.Common.Utility;

namespace CSIPKeyInGUI.BusinessRules_new
{
    public class BRAML_AUDITMAILLOG : BRBase<EntityAML_AUDITMAILLOG>
    {
        public static bool Update(string strCORP_NO,string strExecDate, string _sendType, ref string strMsg)
        {
            bool result = false;
            //20200803-RQ-2020-021027-001 新增SENDTYPE的條件
            string sqlCommand = @"UPDATE [dbo].[AML_AUDITMAILLOG]
                                                            SET [STATUS] = 'N'
                                                            WHERE [CORP_NO] = @CORP_NO AND BATCHDATE=@BATCHDATE AND SENDTYPE = @SENDTYPE";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sqlCommand;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@CORP_NO", strCORP_NO.Trim()));
            sqlcmd.Parameters.Add(new SqlParameter("@BATCHDATE", strExecDate.Trim()));
            sqlcmd.Parameters.Add(new SqlParameter("@SENDTYPE", _sendType.Trim()));//20200803-RQ-2020-021027-001

            if (Update(sqlcmd))
            {
                result = true;
            }
            else
                strMsg = "更新AML_AUDITMAILLOG 狀態欄位失敗";

            return result;
        }

        //若EMAIL發送失敗，需將發送記錄FLAG更新回NULL，以利發送紙本定審通知函
        public static bool UpdateHQ_WORK(string strExecDate, ref string strMsg)
        {
            bool result = false;

            //20200803-RQ-2020-021027-001 新增SENDTYPE的條件
            string sqlCommand = @"UPDATE B SET AddressLabelflag=null FROM AML_AUDITMAILLOG A JOIN AML_HQ_WORK B ON B.ID=A.HQ_ID WHERE A.STATUS='N' AND BATCHDATE='" + strExecDate.Trim() + "' AND A.SENDTYPE = 'A' ";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sqlCommand;
            sqlcmd.CommandType = CommandType.Text;

            if (Update(sqlcmd))
            {
                result = true;
            }
            else
                strMsg = "更新AML_HQ_WORK的AddressLabelflag 失敗";

            return result;
        }
        
        
        // 整批寫入 AML_AUDITMAILLOG
        public static bool AML_AUDITMAILLOGWithBulkCopy(string tableName, DataTable dataTable, ref string errorMsg)
        {
            bool result = false;
            //string connnection = System.Configuration.ConfigurationManager.ConnectionStrings["Connection_System"].ConnectionString;
            string connnection = UtilHelper.GetConnectionStrings("Connection_System");
            SqlConnection conn = new SqlConnection(connnection);
            SqlBulkCopy sbc = new SqlBulkCopy(connnection);
            sbc.DestinationTableName = tableName;

            try
            {
                conn.Open();

                sbc.ColumnMappings.Add("HQ_ID", "HQ_ID");                                                    // HQ_ID
                sbc.ColumnMappings.Add("CASE_NO", "CASE_NO");                                        // 案件編號
                sbc.ColumnMappings.Add("CORP_NO", "CORP_NO");                                      // 統一編號
                sbc.ColumnMappings.Add("REG_NAME", "REG_NAME");                                  // 登記名稱
                sbc.ColumnMappings.Add("EMAIL", "EMAIL");                                                     // EMAIL
                sbc.ColumnMappings.Add("EXPIRYDATE", "EXPIRYDATE");                              // 審查到期日
                sbc.ColumnMappings.Add("BATCHDATE", "BATCHDATE");                              // 批次日期
                sbc.ColumnMappings.Add("STATUS", "STATUS");                                               // 發送狀態
                //20200803-RQ-2020-021027-001
                sbc.ColumnMappings.Add("SENDTYPE", "SENDTYPE");                                     // MAIL寄送類型

                sbc.WriteToServer(dataTable);

                result = true;                
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                //Logging.SaveLog(ELogLayer.DB, ex.ToString());
                Logging.Log(ex);
                return result;
            }
            finally
            {
                sbc.Close();
                conn.Close();
                conn.Dispose();
            }

            return result;
        }

        // omi bill平台回覆檔時，取得前一日的發送記錄
        public static DataTable GetMailLogData(string _batchDate, string _sendType)
        {
            //20200803-RQ-2020-021027-001 新增SENDTYPE的條件
            string sSQL = @"SELECT * FROM [dbo].[AML_AUDITMAILLOG] WHERE BATCHDATE = @BATCHDATE AND SENDTYPE = @SENDTYPE ";

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sSQL;
            sqlcmd.CommandType = CommandType.Text;
            //SqlParameter parmQuery = new SqlParameter("@BATCHDATE", _batchDate.Trim());
            //sqlcmd.Parameters.Add(parmQuery);
            //parmQuery = new SqlParameter("@SENDTYPE", _sendType.Trim());//20200803-RQ-2020-021027-001
            //sqlcmd.Parameters.Add(parmQuery);

            sqlcmd.Parameters.Add(new SqlParameter("@BATCHDATE", _batchDate.Trim()));
            sqlcmd.Parameters.Add(new SqlParameter("@SENDTYPE", _sendType.Trim()));//20200803-RQ-2020-021027-001

            try
            {
                DataTable dt = BRPostOffice_Temp.SearchOnDataSet(sqlcmd).Tables[0];

                return dt;
            }
            catch (Exception ex)
            {
                //Logging.SaveLog(ELogLayer.BusinessRule, "取得上一日定審發送mail記錄檔資料：" + ex);
                Logging.Log("取得上一日定審發送mail記錄檔資料：" + ex);
                return null;
            }
        }

        /// <summary>
        /// 取得待發送不合作地址條清單
        /// 20200806-RQ-2020-021027-001
        /// BatchJob_SendToRMMAuditMail 呼叫使用
        /// </summary>
        /// <param name="ExecTime"></param>
        /// <returns></returns>
        public static DataTable GetRMMAuditMail(string ExeDate)
        {
            DataTable result = new DataTable();

            SqlCommand sqlcmd = new SqlCommand();

            //20220126_Ares_Jack_新增條件只搜尋統編
            sqlcmd.CommandText = string.Format(@"
            SELECT a.ID as 'HQ_ID',a.CASE_NO,a.HCOP_HEADQUATERS_CORP_NO as 'CORP_NO', d.REG_NAME,a.HCOP_EMAIL as 'EMAIL'
            ,Convert(varchar,DATEADD(d,-day(DATEADD(m,2,getdate())),DATEADD(m,1,DATEADD(m,2,getdate()))),112) as 'EXPIRYDATE'
            ,CONVERT(VARCHAR,GETDATE(),112) AS 'BATCHDATE','Y' as 'STATUS','R' as 'SENDTYPE'   
            FROM [{0}].[dbo].[AML_HQ_Work] a
            LEFT JOIN [{0}].[dbo].[szip] b ON a.HCOP_MAILING_CITY =  b.ZIP_DATA
			JOIN [{0}].[dbo].[AML_Cdata_Work] c ON c.CustomerID = a.HCOP_HEADQUATERS_CORP_NO
	        JOIN [{0}].[dbo].[AML_HCOP_STATUS] d ON d.CORP_NO = a.HCOP_HEADQUATERS_CORP_NO
            where case_no in 
            (
                SELECT case_no
		        FROM [{0}].[dbo].[AML_CASE_DATA] WHERE CLOSE_DATE IS NULL AND CREATE_YM = @ExeDate
		    )  
            AND AddressLabelTwoMonthFlag is null   AND SUBSTRING(CASE_NO,9,1) NOT IN ('8','9') 
			AND c.INCORPORATED<> 'Y' AND d.STATUS='O'   
            AND ISNULL(a.HCOP_EMAIL,'') <>'' 
            AND PATINDEX('[0-9]%', a.HCOP_HEADQUATERS_CORP_NO) > 0
            order by ZIP_CODE ASC 
            ", UtilHelper.GetAppSettings("DB_KeyinGUI"));
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@ExeDate", ExeDate));

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
                // Logging.SaveLog(ELogLayer.BusinessRule, ex);
                Logging.Log(ex);
            }

            return result;
        }

        //20200806-RQ-2020-021027-001
        //若EMAIL發送失敗，需將發送記錄FLAG更新回NULL，以利發送紙本不合作通知函
        public static bool UpdateHQ_WORKwithRMM(string strExecDate, ref string strMsg)
        {
            bool result = false;
            
            string sqlCommand = @"UPDATE B SET AddressLabelTwoMonthFlag=null FROM AML_AUDITMAILLOG A JOIN AML_HQ_WORK B ON B.ID=A.HQ_ID WHERE A.STATUS='N' AND BATCHDATE='" + strExecDate.Trim() + "' AND A.SENDTYPE = 'R' ";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sqlCommand;
            sqlcmd.CommandType = CommandType.Text;

            if (Update(sqlcmd))
            {
                result = true;
            }
            else
                strMsg = "AddressLabelTwoMonthFlag 失敗";

            return result;
        }
    }
}