//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：BATCH資料庫業務類
//*  創建日期：2009/10/08
//*  修改記錄：

//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Framework.Data.OM;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using Framework.Common.Message;


namespace CSIPKeyInGUI.BusinessRules
{
    /// <summary>
    /// BATCH資料庫業務類
    /// </summary>
    public class BRBATCH : CSIPCommonModel.BusinessRules.BRBase<EntityBATCH>
    {
        #region SQL 語句

        private const string SEL_BATCH_COUNT = @"SELECT dateSend , dateInput , dateReceive " +
                                "FROM batch " +
                                "where dateSend = @dateSend OR dateInput = @dateInput OR dateReceive = @dateReceive";
        private const string INSERT_BATCH = @"INSERT INTO batch (Batch_no,dateSend,dateInput,dateReceive) VALUES" +
                                        "(@Batch_no,@dateSend,@dateInput,@dateReceive)";
                                        
        //20131007 Casper ACH報送時D檔須額外判斷Ach_Batch_Date是否有小於系統執行當日
        //private const string UPD_OTHER_BANK_TEMP = @"Update Other_Bank_Temp set Batch_no=@Batch_no " +
        //                                "where KeyIn_Flag='2' and Oper_Flag='0' and len(Batch_no) = 0 and Upload_flag='Y'";
                                        
        private const string UPD_OTHER_BANK_TEMP = @"Update Other_Bank_Temp set Batch_no=@Batch_no " +
                                        "where Receive_Number in (select  Receive_Number from Other_Bank_Temp  " +
																				" where KeyIn_Flag='2' and Oper_Flag='0' and len(Batch_no) = 0 and Upload_flag='Y' and apply_type <> 'D' " +
																				" union all " +
                                                                                " select  Receive_Number from Other_Bank_Temp  " +
																				" where KeyIn_Flag='2' and Oper_Flag='0' and len(Batch_no) = 0 and Upload_flag='Y' and apply_type = 'D' " +
                                                                                " and convert(varchar(10),convert(datetime,Ach_Batch_Date,111),111)  <= convert(varchar(10),getDate(),111) and (ACH_HOLD='1' or ACH_HOLD='2')" +
                                                                                " ) and Oper_Flag='0' and len(Batch_no) = 0 and Upload_flag='Y' and (KeyIn_Flag='2' or KeyIn_Flag='0')";

	    //20131007 Casper ACH報送時D檔須額外判斷Ach_Batch_Date是否有小於系統執行當日
        //private const string SEL_OTHER_BANK_TEMP = @"select  Cus_ID from Other_Bank_Temp  " +
        //                        "where KeyIn_Flag='2' and Oper_Flag='0' and len(Batch_no) = 0 and Upload_flag='Y'";
          
          private const string SEL_OTHER_BANK_TEMP = @"select  Cus_ID from Other_Bank_Temp  " +
                                "where KeyIn_Flag='2' and Oper_Flag='0' and len(Batch_no) = 0 and Upload_flag='Y' and apply_type <> 'D' " +
																" union all  " +
																" select  Cus_ID from Other_Bank_Temp   " +
																" where KeyIn_Flag='2' and Oper_Flag='0' and len(Batch_no) = 0 and Upload_flag='Y' and apply_type = 'D' " +
                                                                " and convert(varchar(10),convert(datetime,Ach_Batch_Date,111),111)  <= convert(varchar(10),getDate(),111) and (ACH_HOLD='1' or ACH_HOLD='2')";
                                
                                
        private const string SEARCH_BATCH = @"Select ba.dateSend as dateSend, ba.dateInput as dateInput," +
                                "ba.dateReceive as dateReceive,ba.P02_flag as P02_flag,isnull(obt.CountUpdate,0) as CountUpdate ,isnull(obt2.CountTotal,0) as CountTotal " +
                                "from batch as ba  left join  (select count(Cus_ID) as CountUpdate, Batch_no " +
                                "as Batch_no from Other_Bank_Temp where len(Batch_no)>0 " +
                                //"and Pcmc_Upload_flag='1' group by (Batch_no)) as obt " +
                                "and KeyIn_Flag='2' and Pcmc_Upload_flag='1' group by (Batch_no)) as obt " +
                                "on  ba.dateInput=obt.Batch_no " +
                                "left join  (select count(Cus_ID) as CountTotal, Batch_no as Batch_no from Other_Bank_Temp " +
                                "where KeyIn_Flag='2' and len(Batch_no)>0 group by (Batch_no)) as obt2 " +
                                " on ba.dateInput=obt2.Batch_no {0} "+
                                " order by dateSend,dateInput,dateReceive";

        private const string SEL_BATCH_COUNT_RESET = @"SELECT COUNT(dateInput) " +
                                        "FROM batch " +
                                        "where dateInput = @dateInput";


        //private const string SEL_OTHER_BANK_TEMP_COUNT_RESET = @"Select count(Cus_ID) as UpdateCount " +
        //                        "From Other_Bank_Temp " +
        //                        "where Batch_no=@Batch_no and  (pcmc_return_code <> '' or  pcmc_upload_flag is not null) ";


        private const string SEL_OTHER_BANK_TEMP_COUNT_RESET = @"Select count(Cus_ID) as UpdateCount " +
                        "From Other_Bank_Temp " +
                        "where Batch_no=@Batch_no and  (  pcmc_upload_flag='1') ";

        private const string UPD_OTHER_BANK_TEMP_RESET = @"Update Other_Bank_Temp set Batch_no='' where Batch_no = @Batch_no ";
        private const string DEL_BATCH = @"DELETE FROM batch WHERE dateInput = @dateInput";


        #endregion

        /// <summary>
        /// 查詢批次編號信息
        /// </summary>
        /// <param name="strBatchNo">批次編號</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntityBATCH> SelectEntitySet(string strBatchNo)
        {         
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityBATCH.M_Batch_no, Operator.Equal, DataTypeUtils.String, strBatchNo);
                return BRBATCH.Search(sSql.GetFilterCondition());
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 向數據庫中添加批次訊息記錄

        /// </summary>
        /// <param name="strDateSend">送檔日</param>
        /// <param name="strDateInput">首錄日</param>
        /// <param name="strDateReceive">收檔日</param>
        /// <param name="strMsgID">查詢不正確時，返回的錯誤ID號</param>
        /// <returns>返回添加批次訊息結果記錄，成功：True;失敗：False</returns>
        public static bool Add(string strDateSend, string strDateInput,
                string strDateReceive, ref string strMsgID,ref string strRtnMsg)
        {
            strRtnMsg = "";
            SqlCommand sqlcmdBatch = new SqlCommand();
            sqlcmdBatch.CommandType = CommandType.Text;
            sqlcmdBatch.CommandText = SEL_BATCH_COUNT;

            SqlParameter parmDateSend = new SqlParameter("@" + EntityBATCH.M_dateSend, strDateSend);
            sqlcmdBatch.Parameters.Add(parmDateSend);
            SqlParameter parmDateInput = new SqlParameter("@" + EntityBATCH.M_dateInput, strDateInput);
            sqlcmdBatch.Parameters.Add(parmDateInput);
            SqlParameter parmDateReceive = new SqlParameter("@" + EntityBATCH.M_dateReceive, strDateReceive);
            sqlcmdBatch.Parameters.Add(parmDateReceive);

            try
            {
                //* 檢核收檔日、首錄日、送檔日是否在資料庫中已存在

                DataSet dtsBatch = BRBATCH.SearchOnDataSet(sqlcmdBatch);
                if (dtsBatch == null)
                {
                    strMsgID = "00_00000000_000";
                    return false;
                }
                else
                {
                    if (dtsBatch.Tables[0].Rows.Count>0)
                    {
                        //* 已經存在時，提示‘資料庫已存在輸入的收檔日/首錄日/送檔日’,停止作業
                        strMsgID = "01_04030000_003";
                        if (dtsBatch.Tables[0].Rows[0][0].ToString().Trim() == strDateSend)
                        {
                            if (strRtnMsg.Length == 0)
                            {
                                strRtnMsg = strRtnMsg + MessageHelper.GetMessage("01_04030000_018");
                            }
                            else
                            {
                                strRtnMsg = strRtnMsg + "/" + MessageHelper.GetMessage("01_04030000_018");
                            }
                        }

                        if (dtsBatch.Tables[0].Rows[0][1].ToString().Trim() == strDateInput)
                        {
                            if (strRtnMsg.Length == 0)
                            {
                                strRtnMsg = strRtnMsg + MessageHelper.GetMessage("01_04030000_017");
                            }
                            else
                            {
                                strRtnMsg = strRtnMsg + "/" + MessageHelper.GetMessage("01_04030000_017");
                            }
                        }

                        if (dtsBatch.Tables[0].Rows[0][2].ToString().Trim() == strDateReceive)
                        {
                            if (strRtnMsg.Length == 0)
                            {
                                strRtnMsg = strRtnMsg + MessageHelper.GetMessage("01_04030000_016");
                            }
                            else
                            {
                                strRtnMsg = strRtnMsg + "/" + MessageHelper.GetMessage("01_04030000_016");
                            }
                        }

                        if (strRtnMsg.Length != 0)
                        {
                            strRtnMsg = MessageHelper.GetMessage("01_04030000_019") + strRtnMsg;
                        }

                        return false;
                    }
                    else
                    {

                        //*查詢資料庫中是否有可被分配批次的資料
                        sqlcmdBatch.CommandText = SEL_OTHER_BANK_TEMP;
                        DataSet dtsBatchCheck = BRBATCH.SearchOnDataSet(sqlcmdBatch);
                        if (dtsBatchCheck == null)
                        {
                            //*無要報送的資料
                            strMsgID = "00_00000000_000";
                            return false;
                        }

                        if (dtsBatchCheck.Tables[0].Rows.Count <= 0)
                        {
                            strMsgID = "01_04030000_020";
                            return false;
                        }


                        //* 事務處理。

                        using (OMTransactionScope ts = new OMTransactionScope())
                        {
                            sqlcmdBatch.CommandText = INSERT_BATCH;
                            //* 添加參數Batch_no
                            SqlParameter parmBatch_no = new SqlParameter("@" + EntityBATCH.M_Batch_no, strDateInput);
                            sqlcmdBatch.Parameters.Add(parmBatch_no);
                            if (!BRBATCH.Update(sqlcmdBatch))
                            {
                                strMsgID = "01_04030000_004";
                                return false;
                            }

                            //* 更新Table Other_Bank_Temp中Batch_no欄位
                            sqlcmdBatch.CommandText = UPD_OTHER_BANK_TEMP;
                            if (!BRBATCH.Update(sqlcmdBatch))
                            {
                                strMsgID = "01_04030000_006";
                                return false;
                            }

                            //* 數據庫更新成功

                            ts.Complete();
                        }

                        //* 返回更新成功消息ID
                        strMsgID = "";
                        return true;
                    }
                }
            }
            catch (Exception exp)
            {
                BRBATCH.SaveLog(exp);
                strMsgID = "00_00000000_000";
                return false;
            }
        }

        /// <summary>
        /// 取系統目前所有的ACH自扣屬性設定資料

        /// </summary>
        /// <param name="strDateSend">送檔日</param>
        /// <param name="strDateInput">首錄日</param>
        /// <param name="strDateReceive">收檔日</param>
        /// <param name="intPageIndex">查詢的頁號</param>
        /// <param name="intPageSize">每頁顯示的記錄筆數</param>
        /// <param name="intTotolCount">符合條件的記錄中筆數</param>
        /// <param name="strMsgID">查詢不正確時，返回的錯誤ID號</param>
        /// <returns>返回的查詢結果</returns>
        public static DataTable SearchBatch(string strDateSend, string strDateInput,
                string strDateReceive, int intPageIndex, int intPageSize,
                ref int intTotolCount, ref string strMsgID)
        {
            string strSql = SEARCH_BATCH;
            SqlCommand sqlcmdBatch = new SqlCommand();
            sqlcmdBatch.CommandType = CommandType.Text;

            //string strSqlFilter = " where 1=1 ";
            string strSqlFilter = "";
            //* 送檔日

            if (strDateSend != "")
            {
                if (strSqlFilter.Length != 0)
                {
                    strSqlFilter += " And ba.dateSend = @dateSend ";
                }
                else
                {
                    strSqlFilter += " where  ba.dateSend = @dateSend ";
                }
                SqlParameter parmDateSend = new SqlParameter("@" + EntityBATCH.M_dateSend, strDateSend);
                sqlcmdBatch.Parameters.Add(parmDateSend);
            }
            //* 首錄日

            if (strDateInput != "")
            {
                if (strSqlFilter.Length != 0)
                {
                    strSqlFilter += " And ba.dateInput = @dateInput ";
                }
                else
                {
                    strSqlFilter += " where ba.dateInput = @dateInput ";
                }
                
                SqlParameter parmDateInput = new SqlParameter("@" + EntityBATCH.M_dateInput, strDateInput);
                sqlcmdBatch.Parameters.Add(parmDateInput);
            }
            //* 收當日

            if (strDateReceive != "")
            {
                if (strSqlFilter.Length != 0)
                {
                    strSqlFilter += " And ba.dateReceive = @dateReceive ";
                }
                else
                {
                    strSqlFilter += " where ba.dateReceive = @dateReceive ";
                }
                
                SqlParameter parmDateReceive = new SqlParameter("@" + EntityBATCH.M_dateReceive, strDateReceive);
                sqlcmdBatch.Parameters.Add(parmDateReceive);
            }

            //20200031-CSIP EOS Ares Luke 修改日期:2021/03/08 修改說明:白箱報告修正SQL Injection
            strSql = String.Format(strSql, strSqlFilter);
            strSql = BRCommon.EncodeForSQL(strSql, -1);

            sqlcmdBatch.CommandText = strSql;

            try
            {
                DataTable dtblBatch = BRBATCH.SearchOnDataSet(sqlcmdBatch, intPageIndex, intPageSize, ref intTotolCount).Tables[0];
                return dtblBatch;
            }
            catch (Exception exp)
            {
                BRBATCH.SaveLog(exp);
                strMsgID = "00_00000000_000";
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strDateInput">首錄日</param>
        /// <param name="strMsgID">查詢不正確時，返回的錯誤ID號</param>
        /// <returns>返回添加批次訊息結果記錄，成功：True;失敗：False</returns>
        public static bool Reset(string strDateInput, ref string strMsgID)
        {
            SqlCommand sqlcmdBatch = new SqlCommand();
            sqlcmdBatch.CommandType = CommandType.Text;
            sqlcmdBatch.CommandText = SEL_BATCH_COUNT_RESET;

            SqlParameter parmDateInput = new SqlParameter("@" + EntityBATCH.M_dateInput, strDateInput);
            sqlcmdBatch.Parameters.Add(parmDateInput);

            try
            {
                //* 查詢batch中是否有dateInput =’【首錄日】’的資料
                DataSet dtsBatch = BRBATCH.SearchOnDataSet(sqlcmdBatch);
                if (dtsBatch == null)
                {
                    strMsgID = "00_00000000_000";
                    return false;
                }
                else
                {
                    if (Convert.ToInt16(dtsBatch.Tables[0].Rows[0][0]) == 0)
                    {
                        //* 若查詢不出資料，提示"沒有對應的資料！"，停止作業。

                        strMsgID = "01_04030000_007";
                        return false;
                    }
                    else
                    {
                        //* 事務處理。

                        using (OMTransactionScope ts = new OMTransactionScope())
                        {
                            sqlcmdBatch.CommandText = SEL_OTHER_BANK_TEMP_COUNT_RESET;
                            //* 添加參數Batch_no
                            SqlParameter parmBatch_no = new SqlParameter("@" + EntityBATCH.M_Batch_no, strDateInput);
                            sqlcmdBatch.Parameters.Add(parmBatch_no);
                            DataSet dtsOtherBankTemp = BRBATCH.SearchOnDataSet(sqlcmdBatch);
                            if (dtsOtherBankTemp == null)
                            {
                                strMsgID = "00_00000000_000";
                                return false;
                            }
                            else
                            {
                                //* 該首錄日已經回貼主機完畢，不能Reset。停止作業

                                if (Convert.ToInt16(dtsOtherBankTemp.Tables[0].Rows[0][0]) > 0)
                                {
                                    strMsgID = "01_04030000_008";
                                    return false;
                                }

                                //* 更新他行代扣暫存檔記錄

                                sqlcmdBatch.CommandText = UPD_OTHER_BANK_TEMP_RESET;
                                if (!BRBATCH.Update(sqlcmdBatch))
                                {
                                    strMsgID = "01_04030000_009";
                                    return false;
                                }

                                //* 刪除首錄日yyyymmdd的資料

                                sqlcmdBatch.CommandText = DEL_BATCH;
                                if (!BRBATCH.Update(sqlcmdBatch))
                                {
                                    strMsgID = "01_04030000_010";
                                    return false;
                                }

                                //* 數據庫更新成功

                                ts.Complete();
                                
                                //* 返回更新成功消息ID
                                strMsgID = "";
                                return true;
                                
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                BRBATCH.SaveLog(exp);
                strMsgID = "00_00000000_000";
                return false;
            }
        }
    }
}
