//******************************************************************
//*  作    者：趙呂梁

//*  功能說明：OTHER_BANK_TEMP資料庫業務類

//*  創建日期：2009/10/08
//*  修改記錄：2021/03/17_Ares_Stanley-DB名稱改為變數
//*  修改記錄：2022/09/90_Ares_jhun-EDDA需求調整：核印成功代碼調整為【0、4】


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
using Framework.Common.Utility;

namespace CSIPKeyInGUI.BusinessRules
{
    /// <summary>
    /// OTHER_BANK_TEMP資料庫業務類
    /// </summary>
    public class BROTHER_BANK_TEMP : CSIPCommonModel.BusinessRules.BRBase<EntityOTHER_BANK_TEMP>
    {
        #region SQL語句

        public const string SEL_OTHERBNKTEMP_BATCH = "select ba.P02_flag,obt.Pcmc_Upload_flag,obt.Ach_Return_Code,obt.Pcmc_Return_Code from Other_Bank_Temp as obt inner join batch as ba on obt.Batch_no=ba.Batch_no where obt.Receive_Number=@ReceiveNumber and obt.Cus_ID=@CusID and obt.Upload_flag=@UploadFlag and obt.KeyIn_Flag=@KeyInFlag";

        public const string SEL_BATCH_INFO = "select Cus_ID,Other_Bank_Code_S,Other_Bank_Acc_No , Other_Bank_Pay_Way,Other_Bank_Cus_ID,bcycle_code , E_Bill,Mobile_Phone,E_Mail ,Receive_Number,Apply_Type,KeyIn_Flag,deal_s_no from Other_Bank_Temp where Receive_Number=@ReceiveNumber  and ACH_Return_Code in ('0', '4') and  ( KeyIn_Flag='2' or KeyIn_Flag='0' )";

        public const string DEL_OTHERBNKTEMP = "delete from other_bank_temp where Receive_Number=@ReceiveNumber and Cus_ID=@CusID and upload_flag =@UploadFlag and (keyin_flag = @KeyinFlagOne or keyin_flag = @KeyinFlagTwo)";

        public const string SEL_OTHERBNKTEMP_COUNT = "select count(*) from other_bank_temp where Batch_no  in  (select batch_no from batch where convert(int,R02_flag) > 1 )  and  ACH_Return_Code in ('0', '4') and (Pcmc_Upload_flag <> '1' or Pcmc_Upload_flag is null) and  ( KeyIn_Flag='2' or KeyIn_Flag='0' )";
        public const string SEL_OTHERBNKTEMP_RECEIVE_LIST = "select distinct Receive_Number from other_bank_temp where Batch_no  in  (select batch_no from batch where convert(int,R02_flag) > 1 )  and  ACH_Return_Code in ('0', '4') and (Pcmc_Upload_flag <> '1' or Pcmc_Upload_flag is null) and  ( KeyIn_Flag='2' or KeyIn_Flag='0' )";

        public const string UPD_OTHERBNKTEMP_STATUS = "update Other_Bank_Temp set Batch_Time=getdate(), Pcmc_Upload_flag=@PcmcUploadFlag where Receive_Number=@ReceiveNumber  and ( KeyIn_Flag='2' or  KeyIn_Flag='0' )";

        public const string UPD_OTHERBNKTEMP_STATUS_ONE = "update Other_Bank_Temp set Batch_Time=getdate(), Pcmc_Return_Code=@PcmcReturnCode ,C1342_Return_Code=@C1342ReturnCode where Receive_Number=@ReceiveNumber  and ( KeyIn_Flag='2' or KeyIn_Flag='0' ) and Deal_S_No=@Deal_S_No";

        public const string UPD_OTHERBNKTEMP_STATUS_TWO = "update Other_Bank_Temp set Batch_Time=getdate(), C1342_Return_Code=@C1342ReturnCode where Receive_Number=@ReceiveNumber  and ( KeyIn_Flag='2' or KeyIn_Flag='0' )";

        public const string UPD_OTHERBNKTEMP_STATUS_THREE = "update Other_Bank_Temp set Batch_Time=getdate(), Pay_Way = @PayWay where Receive_Number=@ReceiveNumber  and ( KeyIn_Flag='2' or KeyIn_Flag='0' )";

        //* 設定Other_Bank_Temp上傳處理狀態(用來判斷是否需產檔上傳FTP)
        public const string UPD_OTHERBNKTEMP_STATUS_FOUR = "update Other_Bank_Temp set Batch_Time=getdate(), ACH_hold=@ACH_hold where Receive_Number=@ReceiveNumber  and ( KeyIn_Flag='2' or KeyIn_Flag='0' ) and Deal_S_No=@Deal_S_No";

        public const string SEL_OTHER_BANK_TEMP = @"SELECT {0} FROM OTHER_BANK_TEMP WHERE ";

        public const string SEL_DIS_OTHER_BANK_TEMP = @"SELECT distinct {0} FROM OTHER_BANK_TEMP WHERE ";



        public const string SEL_OTHER_BANK_TEMP_ACH = @"Select distinct Other_Bank_Temp.Receive_Number,  Other_Bank_Temp.Cus_ID "+
                                                      "  from Other_Bank_Temp where Receive_Number=@query_key and len(Batch_no)<>0 and Pcmc_Upload_flag='1' ";

        public const string SEL_OTHER_BANK_TEMP_ACH_GET = @"Select distinct Other_Bank_Temp.Receive_Number,  Other_Bank_Temp.Cus_ID " +
                                              "  from Other_Bank_Temp where Receive_Number=@query_key "+
                                              "  and  ((Receive_Number  not  in  (select  Receive_Number  from Other_Bank_Temp where KeyIn_Flag ='2'  and len(Batch_no)=0 ))  or  (KeyIn_Flag ='2' and len(Batch_no)=0 ) ) ";
        public const string SEL_QRY_OTHER_BANK_TEMP_ACH1Key = @" 
Select 
Other_Bank_Code_S,Other_Bank_Acc_No,Other_Bank_Pay_Way,Other_Bank_Cus_ID
,bcycle_code,Mobile_Phone,E_Mail,E_Bill,Build_Date,Apply_Type
,Deal_No,ACH_Return_Code,Batch_no,Receive_Number ,user_id 
from Other_Bank_Temp  
where Cus_ID= @query_key_Id 
and Receive_Number = @query_key_Num 
and  KeyIn_Flag='1'  
and Oper_Flag='0'
and len(RTRIM(batch_no))=0 ";
        public const string SEL_OTHER_BANK_TEMP_ACH1Key = @" Select Other_Bank_Code_S,Other_Bank_Acc_No,Other_Bank_Pay_Way,Other_Bank_Cus_ID,bcycle_code,Mobile_Phone,E_Mail,E_Bill,Build_Date,Apply_Type,Deal_No,ACH_Return_Code,Batch_no,Receive_Number ,user_id from Other_Bank_Temp  " +
                                                      "  where Cus_ID= @query_key_Id and Receive_Number = @query_key_Num and  KeyIn_Flag='1'  and Oper_Flag='0' and len(batch_no)=0 ";
        public const string SEL_OTHER_BANK_TEMP_ACH2Key = @" Select Other_Bank_Code_S,Other_Bank_Acc_No,Other_Bank_Pay_Way,Other_Bank_Cus_ID,bcycle_code,Mobile_Phone,E_Mail,E_Bill,Build_Date,Apply_Type,Deal_No,ACH_Return_Code,Batch_no,Receive_Number ,user_id from Other_Bank_Temp  " +
                                                    "  where Cus_ID= @query_key_Id and Receive_Number = @query_key_Num and  KeyIn_Flag='2'  and Oper_Flag='0' and len(batch_no)=0 ";

        
        public const string SEL_OTHER_BANK_TEMP_Key = @"update Other_Bank_Temp set " +
                                                      "Other_Bank_Code_S = @BankCodeS," +
                                                       "Other_Bank_Code_L = @BankCodeL," +
                                                      "Other_Bank_Acc_No = @AccNo," +
                                                     "Other_Bank_Pay_Way =@PayWay," +
                                                      "Other_Bank_Cus_ID = @Bank_Cus_ID," +
                                                     "bcycle_code = @Bcycle," +
                                                     "Mobile_Phone = @Mobile," +
                                                     "E_Mail = @EMail," +
                                                     "E_Bill = @EBill," +
                                                     "Build_Date = @BuildDate," +
                                                     "Receive_Number = @Receive_Number," +
                                                     "Apply_Type = @ApplyType," +
                                                      "Deal_No = @DealNo," +
                                                      "user_id = @UserId," +
                                                      "mod_date = @YearTime" +
                                                     " where Cus_ID = @Cus_ID " +
                                                     " and Receive_Number = @Receive_Number " +
                                                     " and len(batch_no)=0 " +
                                                     " and KeyIn_Flag = @KeyFlag and Oper_Flag='0' ";
        public const string SEL_OTHER_BANK_TEMP_Status = @"update Other_Bank_Temp set Upload_Flag = 'N' " +
                                                    " where Cus_ID = @Cus_ID and Receive_Number = @Receive_Number AND Keyin_Flag = '2'";

        public const string SEL_OTHER_BANK_TEMP_Status_2Key = @"update Other_Bank_Temp set Upload_Flag = @Succ, " +
                                                               " mod_date = @ModTime, Auto_Pay_Setting=@Auto_Pay_Setting, CellP_Email_Setting=@CellP_Email_Setting, E_Bill_Setting=@E_Bill_Setting, OutputByTXT_Setting=@OutputByTXT_Setting, Acct_NBR=@AcctNBR " +                      
                                                               " where Cus_ID = @Cus_ID and Receive_Number = @Receive_Number "+
                                                               " and Oper_Flag = '0' AND  len(batch_no)=0 ";
        public const string SEL_OTHER_BANK_TEMP_RECORD = @"select a.Receive_Number,a.Other_Bank_Code_L,a.Other_Bank_Cus_ID," +
                                "a.Other_Bank_Acc_No,a.Apply_Type,b.P02_flag,b.R02DateReceive,a.ACH_Return_Code,a.Pcmc_Return_Code " +
                            "from Other_Bank_Temp a left join batch b on a.Batch_no = b.Batch_no " +
                            "where ((KeyIn_Flag='0') or ( KeyIn_Flag='2' and Upload_flag='Y')) {0} " +
                            "order by a.Receive_Number ";
      public const string SEL_OTHER_BANK_TEMP_RECORD_R02 = @"select a.Receive_Number,a.Other_Bank_Code_L,c.BankName,a.Other_Bank_Cus_ID," +
                        "a.Other_Bank_Acc_No,a.Cus_ID,a.Apply_Type," +
                        "CASE WHEN a.Ach_Return_Code in ('0', '4') THEN '成功' ELSE '失敗' END AS R02_flag,d.Ach_Rtn_Code,d.Ach_Rtn_Msg " +
                       "from Other_Bank_Temp a left join batch b on a.Batch_no = b.Batch_no " +
                        "left join "+
                        "(select bankl.property_code as BankCodeS,bankl.property_name as BankCodeL,bankn.property_name as BankName  " +
                        "from (select property_code ,property_name from {1}.dbo.m_property_code where function_key='01' and property_key='16') as bankl, " +
                        "(select property_code ,property_name from {1}.dbo.m_property_code where function_key='01' and property_key='17') as bankn " +
                        "where   bankl.property_code= bankn.property_code " +
                        ") as c " +
                        "on a.Other_Bank_Code_L=c.BankCodeL " +
                        "left join Ach_Rtn_Info d on a.ACH_Return_Code=d.Ach_Rtn_Code " +
                       "where {0} order by Receive_Number";

      public const string SEL_OTHER_BANK_TEMP_RECORD_P02 = @"select a.Receive_Number,a.Other_Bank_Code_L,c.BankName," +
                                                           "a.Other_Bank_Cus_ID,a.Other_Bank_Acc_No,a.Cus_ID,a.Apply_Type " +
                                                           "from Other_Bank_Temp a,batch b, " +
                                                           "(select bankl.property_code as BankCodeS,bankl.property_name as BankCodeL,bankn.property_name as BankName  " +
                                                           "from (select property_code ,property_name from {1}.dbo.m_property_code where function_key='01' and property_key='16') as bankl, " +
                                                           "(select property_code ,property_name from {1}.dbo.m_property_code where function_key='01' and property_key='17') as bankn " +
                                                           "where   bankl.property_code= bankn.property_code " +
                                                           ") as c " +
                                                           "where a.Batch_no = b.Batch_no and a.Other_Bank_Code_L=c.BankCodeL and b.R02_flag='0' and R02DateReceive='' " +
                                                           "and (dateReceive<@dateInputtoday or dateReceive=@dateInputtoday) " +
                                                           "and {0} " +
                                                           "order by a.Receive_Number";
        public const string SEL_OTHER_BANK_TEMP_WITH_BATCH = @"Select distinct Other_Bank_Temp.Receive_Number,  Other_Bank_Temp.Cus_ID "+
                                   " from Other_Bank_Temp where Receive_Number=@Receive_Number and len(Batch_no)<>0 and Pcmc_Upload_flag<>'1' ";
        public const string SEL_OTHER_BANK_TEMP_WITH_DIST_BATCH = @"Select  Other_Bank_Code_S,Other_Bank_Acc_No,Other_Bank_Pay_Way,Other_Bank_Cus_ID,bcycle_code,Mobile_Phone,E_Mail,E_Bill,Build_Date,Apply_Type,Deal_No,ACH_Return_Code,Batch_no" +
                                   " from Other_Bank_Temp where Receive_Number=@Receive_Number and len(Batch_no)<>0 and Pcmc_Upload_flag<>'1' ";
        public const string SEL_BANKINFO = "select binfo.short_num,binfo.long_num,binfo.bank_name " +
                                         " from" +
                                         " (select bankl.property_code as short_num,bankl.property_name as long_num,bankn.property_name as bank_name" +
                                         " from (select property_code ,property_name from {0}.dbo.m_property_code where function_key='01' and property_key='16') as bankl," +
                                         "      (select property_code ,property_name from {0}.dbo.m_property_code where function_key='01' and property_key='17') as bankn" +
                                         " where   bankl.property_code= bankn.property_code" +
                                         " ) as binfo" +
                                         " where binfo.short_num like @query_key or binfo.bank_name like @query_key or binfo.short_num like substring(@querys_key,1,3) " +
                                         " order by binfo.short_num";
        public const string SEL_BANKFULLINFO = "select binfo.short_num,binfo.long_num,binfo.bank_name " +
                                               " from" +
                                               " (select bankl.property_code as short_num,bankl.property_name as long_num,bankn.property_name as bank_name" +
                                               " from (select property_code ,property_name from {0}.dbo.m_property_code where function_key='01' and property_key='16') as bankl," +
                                               "      (select property_code ,property_name from {0}.dbo.m_property_code where function_key='01' and property_key='17') as bankn" +
                                               " where   bankl.property_code= bankn.property_code" +
                                               " ) as binfo" +
                                               " where binfo.short_num=@query_key" +
                                               " order by binfo.short_num";

        public const string SEL_AUTO_PAY_ReceiveNumber = @"select Receive_Number,cus_id from Auto_Pay " +
                                                        " where Receive_Number=@Receive_Number " +
                                                        " union all " +
                                                        " select Receive_Number,cus_id from Auto_Pay_Popul " +
                                                        "  where Receive_Number=@Receive_Number" +
                                                        " union all " +
                                                        " select Receive_Number,cus_id from Other_Bank_Temp " +
                                                        "  where Receive_Number=@Receive_Number";


#endregion

        /// <summary>
        /// 驗證收件編號和身份證號碼
        /// </summary>
        /// <param name="strReceiveNumber"></param>
        /// <param name="strAddFlag"></param>
        /// <param name="strUploadFlag"></param>
        /// <param name="strColumns"></param>
        /// <returns></returns>        
        public static DataSet Select1(string strReceiveNumber, string strColumns)
        {
            DataSet dstInfo = null;
            SqlHelper sSql = new SqlHelper();

            //檢查收編是否已用過，檢查AUTO_PAY,AUTO_PAY_POPUL表
            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandText = SEL_AUTO_PAY_ReceiveNumber;
            sqlComm.CommandType = CommandType.Text;

            SqlParameter parmReceiveNumber = new SqlParameter("@" + EntityAUTO_PAY.M_Receive_Number, strReceiveNumber);
            sqlComm.Parameters.Add(parmReceiveNumber);

            dstInfo = BRAUTO_PAY.SearchOnDataSet(sqlComm);

            return dstInfo;
        }

        /// 作者 占偉林
        /// 創建日期：2009/11/19
        /// 修改日期：2009/11/19
        /// 修改記錄: 2020/11/10 Ares Luke 處理白箱報告SQL Injection
        /// <summary>
        /// 查詢ACH授權扣款資料清單記錄
        /// </summary>
        /// <param name="dirInputs">查詢條件</param>
        /// <param name="intPageIndex">當前頁號</param>
        /// <param name="intPageSize">每頁顯示記錄條數</param>
        /// <param name="intTotolCount">記錄總條數</param>
        /// <returns>DataSet</returns>
        public static DataSet SearchACHRecord(Dictionary<string, string> dirInputs, int intPageIndex, 
                int intPageSize, ref int intTotolCount)
        {
            SqlCommand sqlComm = new SqlCommand();
            StringBuilder sbWhere = new StringBuilder("");
            foreach (KeyValuePair<string, string> entry in dirInputs)
            {
                //* 鍵檔起日
                if (entry.Key == "txtBuildDateStart" && entry.Value != "")
                {
                    sbWhere.Append(" and a.Build_Date >= @Build_Date_Start ");
                    SqlParameter parmBuildDateStart = new SqlParameter("@Build_Date_Start", entry.Value);
                    sqlComm.Parameters.Add(parmBuildDateStart);
                }

                //* 鍵檔迄日
                if (entry.Key == "txtBuildDateEnd" && entry.Value != "")
                {
                    sbWhere.Append(" and a.Build_Date <= @Build_Date_End ");
                    SqlParameter parmBuildDateEnd = new SqlParameter("@Build_Date_End", entry.Value);
                    sqlComm.Parameters.Add(parmBuildDateEnd);
                }
                
                //* 首錄起日
                if (entry.Key == "txtInputDateStart" && entry.Value != "")
                {
                    sbWhere.Append(" and a.Batch_no >= @Input_Date_Start ");
                    SqlParameter parmInputDateStart = new SqlParameter("@Input_Date_Start", entry.Value);
                    sqlComm.Parameters.Add(parmInputDateStart);

                }

                //* 首錄迄日
                if (entry.Key == "txtInputDateEnd" && entry.Value != "")
                {
                    sbWhere.Append(" and a.Batch_no <= @Input_Date_End ");
                    SqlParameter parmInputDateEnd = new SqlParameter("@Input_Date_End", entry.Value);
                    sqlComm.Parameters.Add(parmInputDateEnd);
                }

                //* 行庫
                if (entry.Key == "txtBank_Code" && entry.Value != "")
                {
                    sbWhere.Append(" and a.Other_Bank_Code_S = @Other_Bank_Code_S ");
                    SqlParameter parmOther_Bank_Code_S = new SqlParameter("@Other_Bank_Code_S", entry.Value);
                    sqlComm.Parameters.Add(parmOther_Bank_Code_S);
                }
            }
            
            //* 添加查詢條件
            sqlComm.CommandText = string.Format(SEL_OTHER_BANK_TEMP_RECORD, sbWhere.ToString());
            sqlComm.CommandType = CommandType.Text;

            //* 查詢并返回查詢結果
            return BROTHER_BANK_TEMP.SearchOnDataSet(sqlComm, intPageIndex, intPageSize, ref intTotolCount);
        }

        /// 作者 占偉林
        /// 創建日期：2009/12/07
        /// 修改日期：2009/12/07 
        /// <summary>
        /// 查詢R02授權成功/失敗報表
        /// </summary>
        /// <param name="dirInputs">查詢條件</param>
        /// <param name="intPageIndex">當前頁號</param>
        /// <param name="intPageSize">每頁顯示記錄條數</param>
        /// <param name="intTotolCount">記錄總條數</param>
        /// <returns>DataSet</returns>
        public static DataSet SearchR02Record(Dictionary<string, string> dirInputs, int intPageIndex,
                int intPageSize, ref int intTotolCount)
        {
            SqlCommand sqlComm = new SqlCommand();
            StringBuilder sbWhere = new StringBuilder(" b.dateInput between @DateInputStart and @DateInputEnd and ");
            bool blSuccess = false;
            bool blFault = false;
            foreach (KeyValuePair<string, string> entry in dirInputs)
            {
                //* 首錄起日
                if (entry.Key == "txtInputDateStart" && entry.Value != "")
                {
                    SqlParameter parmInputDateStart = new SqlParameter("@DateInputStart", entry.Value);
                    sqlComm.Parameters.Add(parmInputDateStart);
                }

                //* 首錄迄日
                if (entry.Key == "txtInputDateEnd" && entry.Value != "")
                {
                    SqlParameter parmInputDateEnd = new SqlParameter("@DateInputEnd", entry.Value);
                    sqlComm.Parameters.Add(parmInputDateEnd);
                }

                //* 成功
                if (entry.Key == "Success" && entry.Value != "")
                {
                    if (entry.Value == "1")
                    {
                        blSuccess = true;
                    }
                }

                //* 失敗
                if (entry.Key == "Fault" && entry.Value != "")
                {
                    if (entry.Value == "1")
                    {
                        blFault = true;
                    }
                }
            }

            //* 若【成功】和【失敗】都未選擇或都已選擇 
            if (blSuccess && blFault || !blSuccess && !blFault)
            {
                sbWhere.Append(" b.R02_flag in ('1','2','3') ");
            }
            else
            {
                if (blSuccess)
                {
                    sbWhere.Append(" b.R02_flag in ('2','3') and a.Ach_Return_Code in ('0', '4')");
                }
                if (blFault)
                {
                    sbWhere.Append(" (a.Ach_Return_Code != '0' or b.R02_flag ='1') ");
                }
            }

            //* 添加查詢條件
            sqlComm.CommandText = string.Format(SEL_OTHER_BANK_TEMP_RECORD_R02, sbWhere.ToString(), UtilHelper.GetAppSettings("DB_CSIP"));
            sqlComm.CommandType = CommandType.Text;

            //* 查詢并返回查詢結果
            return BROTHER_BANK_TEMP.SearchOnDataSet(sqlComm, intPageIndex, intPageSize, ref intTotolCount);
        }

        /// 作者 占偉林
        /// 創建日期：2009/12/15
        /// 修改日期：2009/12/15 
        /// <summary>
        /// 查詢P02授權未回覆報表
        /// </summary>
        /// <param name="dirInputs">查詢條件</param>
        /// <param name="intPageIndex">當前頁號</param>
        /// <param name="intPageSize">每頁顯示記錄條數</param>
        /// <param name="intTotolCount">記錄總條數</param>
        /// <returns>DataSet</returns>
        public static DataSet SearchP02Record(Dictionary<string, string> dirInputs, int intPageIndex,
                int intPageSize, ref int intTotolCount)
        {
            SqlCommand sqlComm = new SqlCommand();
            StringBuilder sbWhere = new StringBuilder(" b.dateInput between @dateInputStart and @dateInputEnd ");
            foreach (KeyValuePair<string, string> entry in dirInputs)
            {
                //* 首錄起日
                if (entry.Key == "txtInputDateStart" && entry.Value != "")
                {
                    SqlParameter parmInputDateStart = new SqlParameter("@dateInputStart", entry.Value);
                    sqlComm.Parameters.Add(parmInputDateStart);
                }

                //* 首錄迄日
                if (entry.Key == "txtInputDateEnd" && entry.Value != "")
                {
                    SqlParameter parmInputDateEnd = new SqlParameter("@dateInputEnd", entry.Value);
                    sqlComm.Parameters.Add(parmInputDateEnd);
                }

                //* 行庫
                if (entry.Key == "txtBank_Code" && entry.Value != "")
                {
                    sbWhere.Append(" and c.BankCodeS = @BankCodeS ");
                    SqlParameter parmBankCodeS = new SqlParameter("@BankCodeS", entry.Value);
                    sqlComm.Parameters.Add(parmBankCodeS);
                }
            }

            SqlParameter parmInputToday = new SqlParameter("@dateInputtoday", System.DateTime.Now.AddYears(-1911).ToString("yyyyMMdd"));
            sqlComm.Parameters.Add(parmInputToday);

            //* 添加查詢條件
            sqlComm.CommandText = string.Format(SEL_OTHER_BANK_TEMP_RECORD_P02, sbWhere.ToString(), UtilHelper.GetAppSettings("DB_CSIP"));
            sqlComm.CommandType = CommandType.Text;

            //* 查詢并返回查詢結果
            return BROTHER_BANK_TEMP.SearchOnDataSet(sqlComm, intPageIndex, intPageSize, ref intTotolCount);
        }

        /// <summary>
        /// 查詢符合收件編號和身分證號碼的資料
        /// </summary>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strCusID">身分證號碼</param>        
        /// <param name="strUploadFlag">Keyin標示</param>
        /// <param name="strKeyInFlag">Keyin類別(1-一KEY/2-二KEY)</param>
        /// <returns>EntitySet</returns>
        public static EntitySet<EntityOTHER_BANK_TEMP> SelectEntitySet(string strReceiveNumber, string strCusID, string strUploadFlag, string strKeyInFlag)
        {
            try
            {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityOTHER_BANK_TEMP.M_Receive_Number, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
                sSql.AddCondition(EntityOTHER_BANK_TEMP.M_Cus_ID, Operator.Equal, DataTypeUtils.String, strCusID);
                sSql.AddCondition(EntityOTHER_BANK_TEMP.M_Upload_flag, Operator.Equal, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityOTHER_BANK_TEMP.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);

                return BROTHER_BANK_TEMP.Search(sSql.GetFilterCondition());
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 聯接BATCH表查詢符合收件編號和身分證號碼的資料
        /// </summary>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strCusID">身分證號碼</param>        
        /// <param name="strUploadFlag">Keyin標示</param>
        /// <param name="strKeyInFlag">Keyin類別(1-一KEY/2-二KEY)</param>
        /// <returns>DataSet</returns>
        public static DataSet Select(string strReceiveNumber, string strCusID, string strUploadFlag, string strKeyInFlag)
        {
                SqlCommand sqlComm = new SqlCommand();
                sqlComm.CommandText = SEL_OTHERBNKTEMP_BATCH;
                sqlComm.CommandType = CommandType.Text;

                SqlParameter parmReceiveNumber = new SqlParameter("@ReceiveNumber", strReceiveNumber);
                sqlComm.Parameters.Add(parmReceiveNumber);
                SqlParameter parmCusID = new SqlParameter("@CusID", strCusID);
                sqlComm.Parameters.Add(parmCusID);
                SqlParameter parmUploadFlag = new SqlParameter("@UploadFlag", strUploadFlag);
                sqlComm.Parameters.Add(parmUploadFlag);
                SqlParameter parmKeyInFlag = new SqlParameter("@KeyInFlag", strKeyInFlag);
                sqlComm.Parameters.Add(parmKeyInFlag);

                return BROTHER_BANK_TEMP.SearchOnDataSet(sqlComm);
        }

        /// <summary>
        /// 查詢需要異動主機欄位的資料
        /// </summary>
        /// <returns>int</returns>     
        public static int SelectProcessTotalCount()
        {
            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandText = SEL_OTHERBNKTEMP_COUNT;
            sqlComm.CommandType = CommandType.Text;

            DataSet dstAch = BROTHER_BANK_TEMP.SearchOnDataSet(sqlComm);
            if (dstAch != null && dstAch.Tables[0].Rows.Count > 0)
            {
                return int.Parse(dstAch.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 查詢需要異動主機欄位的資料
        /// </summary>
        /// <returns>DataSet</returns>     
        public static DataSet SelectReceiveList()
        {
             SqlCommand sqlComm = new SqlCommand();
             sqlComm.CommandText = SEL_OTHERBNKTEMP_RECEIVE_LIST;
             sqlComm.CommandType = CommandType.Text;

             return BROTHER_BANK_TEMP.SearchOnDataSet(sqlComm);
        }

        /// <summary>
        /// 查詢更新主機資料的訊息

        /// </summary>
        /// <returns>DataSet</returns>
        public static DataSet SelectBatchInfo(string strReceiveNumber)
        {
                SqlCommand sqlComm = new SqlCommand();
                sqlComm.CommandText = SEL_BATCH_INFO;
                sqlComm.CommandType = CommandType.Text;

                SqlParameter parmReceiveNumber = new SqlParameter("@ReceiveNumber", strReceiveNumber);
                sqlComm.Parameters.Add(parmReceiveNumber);

                return BROTHER_BANK_TEMP.SearchOnDataSet(sqlComm);
        }

        /// <summary>
        /// 刪除作業
        /// </summary>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strCusID">身分證號碼</param>
        /// <param name="strUploadFlag">Keyin標示</param>
        /// <param name="strKeyInFlagOne">Keyin類別(1-一KEY)</param>
        /// <param name="strKeyInFlagTwo">Keyin類別(2-二KEY)</param>
        /// <returns>true成功，false失敗</returns>
        public static bool Delete(string strReceiveNumber, string strCusID, string strUploadFlag, string strKeyInFlagOne, string strKeyInFlagTwo)
        {
                SqlCommand sqlComm = new SqlCommand();
                sqlComm.CommandText = DEL_OTHERBNKTEMP;
                sqlComm.CommandType = CommandType.Text;

                SqlParameter parmReceiveNumber = new SqlParameter("@ReceiveNumber", strReceiveNumber);
                sqlComm.Parameters.Add(parmReceiveNumber);
                SqlParameter parmCusID = new SqlParameter("@CusID", strCusID);
                sqlComm.Parameters.Add(parmCusID);
                SqlParameter parmUploadFlag = new SqlParameter("@UploadFlag", strUploadFlag);
                sqlComm.Parameters.Add(parmUploadFlag);
                SqlParameter parmKeyInFlagOne = new SqlParameter("@KeyInFlagOne", strKeyInFlagOne);
                sqlComm.Parameters.Add(parmKeyInFlagOne);
                SqlParameter parmKeyInFlagTwo = new SqlParameter("@KeyInFlagTwo", strKeyInFlagTwo);
                sqlComm.Parameters.Add(parmKeyInFlagTwo);

                return BROTHER_BANK_TEMP.Delete(sqlComm);
        }

        /// <summary>
        /// 修改作業
        /// </summary>
        /// <param name="eOtherBankTemp">OtherBankTemp實體</param>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strCusID">身分證號碼</param>
        /// <param name="strUploadFlag">Keyin標示</param>
        /// <param name="strKeyInFlag">Keyin類別(1-一KEY/2-二KEY)</param>
        /// <param name="strField">要更新欄位的集合</param>
        /// <returns>true 成功， false失敗</returns>
        public static bool Update(EntityOTHER_BANK_TEMP eOtherBankTemp, string strReceiveNumber, string strCusID, string strUploadFlag, string strKeyInFlag, string[] strField)
        {
                SqlHelper sSql = new SqlHelper();
                sSql.AddCondition(EntityOTHER_BANK_TEMP.M_Receive_Number, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
                sSql.AddCondition(EntityOTHER_BANK_TEMP.M_Cus_ID, Operator.Equal, DataTypeUtils.String, strCusID);
                sSql.AddCondition(EntityOTHER_BANK_TEMP.M_Upload_flag, Operator.Equal, DataTypeUtils.String, strUploadFlag);
                sSql.AddCondition(EntityOTHER_BANK_TEMP.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, strKeyInFlag);

                return BROTHER_BANK_TEMP.UpdateEntityByCondition(eOtherBankTemp, sSql.GetFilterCondition(), strField);
        }

        /// <summary>
        /// 更新收件編號的回貼主機動作標志
        /// </summary>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strPcmcUploadFlag">回貼主機動作標志('1'已經進行了回貼主機動作)</param>
        /// <returns>true 成功， false失敗</returns>
        public static bool UpdatePcmcUploadFlag(string strReceiveNumber, string strPcmcUploadFlag)
        {
                SqlCommand sqlComm = new SqlCommand();
                sqlComm.CommandText = UPD_OTHERBNKTEMP_STATUS;
                sqlComm.CommandType = CommandType.Text;

                SqlParameter parmReceiveNumber = new SqlParameter("@ReceiveNumber", strReceiveNumber);
                sqlComm.Parameters.Add(parmReceiveNumber);
                SqlParameter parmPcmcUploadFlag = new SqlParameter("@PcmcUploadFlag", strPcmcUploadFlag);
                sqlComm.Parameters.Add(parmPcmcUploadFlag);

                return BROTHER_BANK_TEMP.Update(sqlComm);
        }

        /// <summary>
        /// 更新收件編號回貼主機 Pcmc返回碼、回貼主機1342
        /// </summary>
        /// <param name="strPcmcReturnCode">回貼主機 Pcmc返回碼</param>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strC1342ReturnCode">回貼主機1342</param>
        /// <returns>true 成功， false失敗</returns>
        public static bool UpdatePcmcReturnCodeAndC1342ReturnCode(string strPcmcReturnCode, string strReceiveNumber, string strC1342ReturnCode, string strDeal_S_No)
        {
                SqlCommand sqlComm = new SqlCommand();
                sqlComm.CommandText = UPD_OTHERBNKTEMP_STATUS_ONE;
                sqlComm.CommandType = CommandType.Text;

                SqlParameter parmPcmcReturnCode = new SqlParameter("@PcmcReturnCode", strPcmcReturnCode);
                sqlComm.Parameters.Add(parmPcmcReturnCode);
                SqlParameter parmReceiveNumber = new SqlParameter("@ReceiveNumber", strReceiveNumber);
                sqlComm.Parameters.Add(parmReceiveNumber);
                SqlParameter parmC1342ReturnCode = new SqlParameter("@C1342ReturnCode", strC1342ReturnCode);
                sqlComm.Parameters.Add(parmC1342ReturnCode);
                SqlParameter parmDeal_S_No = new SqlParameter("@Deal_S_No", strDeal_S_No);
                sqlComm.Parameters.Add(parmDeal_S_No);

                return BROTHER_BANK_TEMP.Update(sqlComm);
        }

        /// <summary>
        /// 更新Other_Bank_Temp上傳處理狀態(用來判斷是否需產檔上傳FTP)
        /// </summary>
        /// <param name="strACH_hold">處理狀態</param>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <returns>true 成功， false失敗</returns>
        public static bool UpdateACHhold(string strACH_hold, string strReceiveNumber, string strDeal_S_No)
        {
            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandText = UPD_OTHERBNKTEMP_STATUS_FOUR;
            sqlComm.CommandType = CommandType.Text;

            SqlParameter parmPcmcReturnCode = new SqlParameter("@ACH_hold", strACH_hold);
            sqlComm.Parameters.Add(parmPcmcReturnCode);
            SqlParameter parmReceiveNumber = new SqlParameter("@ReceiveNumber", strReceiveNumber);
            sqlComm.Parameters.Add(parmReceiveNumber);
            SqlParameter parmDeal_S_No = new SqlParameter("@Deal_S_No", strDeal_S_No);
            sqlComm.Parameters.Add(parmDeal_S_No);

            return BROTHER_BANK_TEMP.Update(sqlComm);
        }

        /// <summary>
        /// 更新收件編號回貼主機1342
        /// </summary>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strC1342ReturnCode">回貼主機1342</param>
        /// <returns>true 成功， false失敗</returns>
        public static bool UpdateC1342ReturnCode(string strReceiveNumber, string strC1342ReturnCode)
        {
                SqlCommand sqlComm = new SqlCommand();
                sqlComm.CommandText = UPD_OTHERBNKTEMP_STATUS_TWO;
                sqlComm.CommandType = CommandType.Text;

                SqlParameter parmReceiveNumber = new SqlParameter("@ReceiveNumber", strReceiveNumber);
                sqlComm.Parameters.Add(parmReceiveNumber);
                SqlParameter parmC1342ReturnCode = new SqlParameter("@C1342ReturnCode", strC1342ReturnCode);
                sqlComm.Parameters.Add(parmC1342ReturnCode);

                return BROTHER_BANK_TEMP.Update(sqlComm);
        }

        /// <summary>
        /// 更新收件編號繳款狀況
        /// </summary>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strC1342ReturnCode">繳款狀況</param>
        /// <returns>true 成功， false失敗</returns>
        public static bool UpdatePayWay(string strReceiveNumber, string strPayWay)
        {
                SqlCommand sqlComm = new SqlCommand();
                sqlComm.CommandText = UPD_OTHERBNKTEMP_STATUS_THREE;
                sqlComm.CommandType = CommandType.Text;

                SqlParameter parmReceiveNumber = new SqlParameter("@ReceiveNumber", strReceiveNumber);
                sqlComm.Parameters.Add(parmReceiveNumber);
                SqlParameter parmPayWay = new SqlParameter("@PayWay", strPayWay);
                sqlComm.Parameters.Add(parmPayWay);

                return BROTHER_BANK_TEMP.Update(sqlComm);
        }


        /// <summary>
        /// 查詢他行 自扣資料
        /// </summary>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strColumns">要查詢得到的欄位</param>
        /// <returns>DataSet</returns>
        public static DataSet SelectSearchData(string strReceiveNumber, string strColumns)
        {
            DataSet dstInfo = null;
            SqlHelper sSql = new SqlHelper();

            sSql.AddCondition(EntityOTHER_BANK_TEMP.M_Receive_Number, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
            sSql.AddCondition(EntityOTHER_BANK_TEMP.M_Batch_no, Operator.NotEqual, DataTypeUtils.String, "0");
            sSql.AddCondition(EntityOTHER_BANK_TEMP.M_Pcmc_Upload_flag, Operator.NotEqual, DataTypeUtils.String, "1");

            string strSqlCmd = string.Format(SEL_OTHER_BANK_TEMP, strColumns) + sSql.GetFilterCondition().Substring(4, sSql.GetFilterCondition().Length - 4);

            dstInfo = BROTHER_BANK_TEMP.SearchOnDataSet(strSqlCmd);

            return dstInfo;
        }

        /// <summary>
        /// 查詢他行 自扣資料
        /// </summary>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strAddFlag">資料類別</param>
        /// <param name="strUploadFlag">異動類別</param>
        /// <param name="strColumns">要查詢得到的欄位</param>
        /// <returns>DataSet</returns>
        public static DataSet Select(string strReceiveNumber, string strColumns)
        {
            DataSet dstInfo = null;
            SqlHelper sSql = new SqlHelper();

            sSql.AddCondition(EntityOTHER_BANK_TEMP.M_Receive_Number, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
            sSql.AddCondition(EntityOTHER_BANK_TEMP.M_Oper_Flag, Operator.Equal, DataTypeUtils.String, "0");
            sSql.AddCondition(EntityOTHER_BANK_TEMP.M_Upload_flag, Operator.Equal, DataTypeUtils.String,"Y");
            sSql.AddCondition(EntityOTHER_BANK_TEMP.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, "2");

            string strSqlCmd = string.Format(SEL_OTHER_BANK_TEMP, strColumns) + sSql.GetFilterCondition().Substring(4, sSql.GetFilterCondition().Length - 4);

            dstInfo = BROTHER_BANK_TEMP.SearchOnDataSet(strSqlCmd);

            return dstInfo;
        }


        /// <summary>
        /// 查詢他行 自扣資料
        /// </summary>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strAddFlag">資料類別</param>
        /// <param name="strUploadFlag">異動類別</param>
        /// <param name="strColumns">要查詢得到的欄位</param>
        /// <returns>DataSet</returns>
        public static DataSet SelectDist(string strReceiveNumber, string strColumns)
        {
            DataSet dstInfo = null;
            SqlHelper sSql = new SqlHelper();

            sSql.AddCondition(EntityOTHER_BANK_TEMP.M_Receive_Number, Operator.Equal, DataTypeUtils.String, strReceiveNumber);
            sSql.AddCondition(EntityOTHER_BANK_TEMP.M_Oper_Flag, Operator.Equal, DataTypeUtils.String, "0");
            sSql.AddCondition(EntityOTHER_BANK_TEMP.M_Upload_flag, Operator.Equal, DataTypeUtils.String, "Y");
            sSql.AddCondition(EntityOTHER_BANK_TEMP.M_KeyIn_Flag, Operator.Equal, DataTypeUtils.String, "2");

            string strSqlCmd = string.Format(SEL_DIS_OTHER_BANK_TEMP, strColumns) + sSql.GetFilterCondition().Substring(4, sSql.GetFilterCondition().Length - 4);

            dstInfo = BROTHER_BANK_TEMP.SearchOnDataSet(strSqlCmd);

            return dstInfo;
        }

        /// <summary>
        /// 查詢他行 自扣資料
        /// </summary>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strAddFlag">資料類別</param>
        /// <param name="strUploadFlag">異動類別</param>
        /// <param name="strColumns">要查詢得到的欄位</param>
        /// <returns>DataSet</returns>
        public static DataSet SelectDistBatch(string strReceiveNumber)
        {
            string strSql = SEL_OTHER_BANK_TEMP_WITH_DIST_BATCH;
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = strSql;

            DataSet ds = new DataSet();

            //* 傳入關鍵字變量

            SqlParameter parmQuery_key = new SqlParameter("@Receive_Number", strReceiveNumber);
            sqlcmd.Parameters.Add(parmQuery_key);

            return BROTHER_BANK_TEMP.SearchOnDataSet(sqlcmd);
        }

        /// <summary>
        /// 查詢他行 自扣資料
        /// </summary>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <param name="strAddFlag">資料類別</param>
        /// <param name="strUploadFlag">異動類別</param>
        /// <param name="strColumns">要查詢得到的欄位</param>
        /// <returns>DataSet</returns>
        public static DataSet SelectWithBatch(string strReceiveNumber)
        {
            string strSql = SEL_OTHER_BANK_TEMP_WITH_BATCH;
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = strSql;
           
            DataSet ds = new DataSet();

            //* 傳入關鍵字變量

            SqlParameter parmQuery_key = new SqlParameter("@Receive_Number", strReceiveNumber);
            sqlcmd.Parameters.Add(parmQuery_key);

            return BROTHER_BANK_TEMP.SearchOnDataSet(sqlcmd);

        }

        /// <summary>
        /// 查詢否已經有完成的ACH 流程的資料
        /// </summary>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <returns>查詢結果:記錄DataTable</returns>
        public static DataTable SearchACH(string strKey)
        {
            string strSql = SEL_OTHER_BANK_TEMP_ACH;
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = strSql;
         
            //* 傳入關鍵字變量

            SqlParameter parmQuery_key = new SqlParameter("@query_key", strKey);
            sqlcmd.Parameters.Add(parmQuery_key);

            DataTable dtbl = new DataTable();
            try
            {
                //* 查詢記錄
                dtbl = BROTHER_BANK_TEMP.SearchOnDataSet(sqlcmd).Tables[0];
                return dtbl;
            }
            catch (Exception exp)
            {
                BRCustomer_Log.SaveLog(exp);
                return null;
            }
        }





        /// <summary>
        /// 輸入的收件編號是否有還未分配批次號的資料
        /// </summary>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <returns>查詢結果:記錄DataTable</returns>
        public static DataTable SearchACHGET(string strKey)
        {
            string strSql = SEL_OTHER_BANK_TEMP_ACH_GET;
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = strSql;

            //* 傳入關鍵字變量

            SqlParameter parmQuery_key = new SqlParameter("@query_key", strKey);
            sqlcmd.Parameters.Add(parmQuery_key);

            DataTable dtbl = new DataTable();
            try
            {
                //* 查詢記錄
                dtbl = BROTHER_BANK_TEMP.SearchOnDataSet(sqlcmd).Tables[0];
                return dtbl;
            }
            catch (Exception exp)
            {
                BRCustomer_Log.SaveLog(exp);
                return null;
            }
        }

        /// <summary>
        /// 他行 自扣一KEY資料
        /// </summary>
        /// <param name="strUserId">身分證號碼</param>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <returns>查詢結果:記錄DataTable</returns>
        public static DataTable SearchACH1Key(string strUserId, string strReceiveNumber)
        {
            string strSql = SEL_OTHER_BANK_TEMP_ACH1Key;
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
                dtbl = BROTHER_BANK_TEMP.SearchOnDataSet(sqlcmd).Tables[0];
                return dtbl;
            }
            catch (Exception exp)
            {
                BRCustomer_Log.SaveLog(exp);
                return null;
            }
        }


        /// <summary>
        /// 他行 自扣一KEY資料
        /// </summary>
        /// <param name="strUserId">身分證號碼</param>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <returns>查詢結果:記錄DataTable</returns>
        public static DataTable SearchQryACH1Key(string strUserId, string strReceiveNumber)
        {
            string strSql = SEL_QRY_OTHER_BANK_TEMP_ACH1Key;
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
                dtbl = BROTHER_BANK_TEMP.SearchOnDataSet(sqlcmd).Tables[0];
                return dtbl;
            }
            catch (Exception exp)
            {
                BRCustomer_Log.SaveLog(exp);
                return null;
            }
        }


        /// <summary>
        /// 他行 自扣二KEY資料
        /// </summary>
        /// <param name="strUserId">身分證號碼</param>
        /// <param name="strReceiveNumber">收件編號</param>
        /// <returns>查詢結果:記錄DataTable</returns>
        public static DataTable SearchACH2Key(string strUserId, string strReceiveNumber)
        {
            string strSql = SEL_OTHER_BANK_TEMP_ACH2Key;
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
                dtbl = BROTHER_BANK_TEMP.SearchOnDataSet(sqlcmd).Tables[0];
                return dtbl;
            }
            catch (Exception exp)
            {
                BRCustomer_Log.SaveLog(exp);
                return null;
            }
        }



        /// <summary>
        /// 模糊查詢銀行訊息
        /// </summary>
        /// <param name="strKey">銀行查詢條件</param>
        /// <returns>查詢結果:記錄DataTable</returns>
        public static DataTable SearchBankInfo(string strKey)
        {
            string strSql = string.Format(SEL_BANKINFO, UtilHelper.GetAppSettings("DB_CSIP"));
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = strSql;

            //* 傳入關鍵字變量

            SqlParameter parmQuery_key = new SqlParameter("@query_key", '%' + strKey + '%');
            SqlParameter parmQuerys_key = new SqlParameter("@querys_key", strKey);
            sqlcmd.Parameters.Add(parmQuery_key);
            sqlcmd.Parameters.Add(parmQuerys_key);

            DataTable dtbl = new DataTable();
            try
            {
                //* 查詢記錄
                dtbl = BROTHER_BANK_TEMP.SearchOnDataSet(sqlcmd).Tables[0];
                return dtbl;
            }
            catch (Exception exp)
            {
                return null;
            }
        }


        /// <summary>
        /// 更新Keyin資料
        /// </summary>
        /// <param name="strBankCodeS">銀行3碼</param>
        /// <param name="strBankCodeL">銀行7碼</param>
        /// <param name="strAccNo">扣繳帳號</param>
        /// <param name="strPayWay">扣繳方式</param>
        /// <param name="strBank_Cus_ID">帳戶ID</param>
        /// <param name="strBcycle">帳單週期</param>
        /// <param name="strMobile">移動電話</param>
        /// <param name="strEMail">電子郵件</param>
        /// <param name="strEBill">電子帳單</param>
        /// <param name="strBuildDate">鍵檔日期</param>
        /// <param name="strReceive_Number">收件編號</param>
        /// <param name="strApplyType">申請類別/param>
        /// <param name="strDealNo">交易代號</param>
        /// <param name="strUserId">維護用戶ID</param>
        /// <param name="strYearTime">維護日期</param>
        /// <param name="strKeyFlag">一、二Key類別</param>
        /// <param name="strCusID">異動身分證號碼</param>
        /// <returns>true 成功， false失敗</returns>
        public static bool UpdateKeyInfo(string strBankCodeS, string strBankCodeL,string strAccNo,string strPayWay,string strBank_Cus_ID,string strBcycle,string strMobile,string strEMail,string strEBill,string strBuildDate,string strReceive_Number,string strApplyType,string strDealNo,string strUserId,string strYearTime,string strKeyFlag,string strCusID)
        {

            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandText = SEL_OTHER_BANK_TEMP_Key;
            sqlComm.CommandType = CommandType.Text;

            SqlParameter parmReceiveNumber = new SqlParameter("@BankCodeS", strBankCodeS);
            sqlComm.Parameters.Add(parmReceiveNumber);

            SqlParameter parmBankCodeL = new SqlParameter("@BankCodeL", strBankCodeL);
            sqlComm.Parameters.Add(parmBankCodeL);

            SqlParameter parmAccNo = new SqlParameter("@AccNo", strAccNo);
            sqlComm.Parameters.Add(parmAccNo);

             SqlParameter parmPayWay = new SqlParameter("@PayWay", strPayWay);
            sqlComm.Parameters.Add(parmPayWay);

            SqlParameter parmBank_Cus_ID = new SqlParameter("@Bank_Cus_ID", strBank_Cus_ID);
            sqlComm.Parameters.Add(parmBank_Cus_ID);

            SqlParameter parmBcycle = new SqlParameter("@Bcycle", strBcycle);
            sqlComm.Parameters.Add(parmBcycle);

            SqlParameter parmMobile = new SqlParameter("@Mobile", strMobile);
            sqlComm.Parameters.Add(parmMobile);

            SqlParameter parmEMail = new SqlParameter("@EMail", strEMail);
            sqlComm.Parameters.Add(parmEMail);

            SqlParameter parmEBill = new SqlParameter("@EBill", strEBill);
            sqlComm.Parameters.Add(parmEBill);

            SqlParameter parmBuildDate = new SqlParameter("@BuildDate", strBuildDate);
            sqlComm.Parameters.Add(parmBuildDate);

            SqlParameter parmReceive_Number = new SqlParameter("@Receive_Number", strReceive_Number);
            sqlComm.Parameters.Add(parmReceive_Number);

            SqlParameter parmApplyType = new SqlParameter("@ApplyType", strApplyType);
            sqlComm.Parameters.Add(parmApplyType);

            SqlParameter parmDealNo = new SqlParameter("@DealNo", strDealNo);
            sqlComm.Parameters.Add(parmDealNo);

            SqlParameter parmUserId = new SqlParameter("@UserId", strUserId);
            sqlComm.Parameters.Add(parmUserId);

            SqlParameter parmYearTime = new SqlParameter("@YearTime", strYearTime);
            sqlComm.Parameters.Add(parmYearTime);

            SqlParameter parmKeyFlag = new SqlParameter("@KeyFlag", strKeyFlag);
            sqlComm.Parameters.Add(parmKeyFlag);

            SqlParameter parmCus_ID = new SqlParameter("@Cus_ID", strCusID);
            sqlComm.Parameters.Add(parmCus_ID);

            return BROTHER_BANK_TEMP.Update(sqlComm);
        }


        /// <summary>
        /// 一Key後更新Keyin資料
        /// </summary>
        /// <param name="strReceive_Number">收件編號</param>
        /// <param name="strCusID">異動身分證號碼</param>
        /// <returns>true 成功， false失敗</returns>
        public static bool UpdateKeyInfo(string strReceive_Number,string strCusID)
        {

            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandText = SEL_OTHER_BANK_TEMP_Status;
            sqlComm.CommandType = CommandType.Text;

           
             SqlParameter parmPayWay = new SqlParameter("@Receive_Number", strReceive_Number);
            sqlComm.Parameters.Add(parmPayWay);



            SqlParameter parmCus_ID = new SqlParameter("@Cus_ID", strCusID);
            sqlComm.Parameters.Add(parmCus_ID);


            return BROTHER_BANK_TEMP.Update(sqlComm);

        }

        /// <summary>
        /// 二Key後更新Keyin資料
        /// </summary>
        /// <param name="strReceive_Number">收件編號</param>
        /// <param name="strCusID">異動身分證號碼</param>
        /// <returns>true 成功， false失敗</returns>
        public static bool Update2KeyInfo(string strReceive_Number, string strCusID,string strSucc, string strAuto_Pay_Setting, string strCellP_Email_Setting, string strE_Bill_Setting, string strOutputByTXT_Setting, string sAcctNBR)
        {

            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandText = SEL_OTHER_BANK_TEMP_Status_2Key;
            sqlComm.CommandType = CommandType.Text;


            SqlParameter parmSucc = new SqlParameter("@Succ", strSucc);
            sqlComm.Parameters.Add(parmSucc);

            SqlParameter parmModTime = new SqlParameter("@ModTime", System.DateTime.Now.ToString("yyyyMMdd"));
            sqlComm.Parameters.Add(parmModTime);

            SqlParameter parmPayWay = new SqlParameter("@Receive_Number", strReceive_Number);
            sqlComm.Parameters.Add(parmPayWay);

            SqlParameter parmCus_ID = new SqlParameter("@Cus_ID", strCusID);
            sqlComm.Parameters.Add(parmCus_ID);

            sqlComm.Parameters.Add(new SqlParameter("@Auto_Pay_Setting", strAuto_Pay_Setting));
            sqlComm.Parameters.Add(new SqlParameter("@CellP_Email_Setting", strCellP_Email_Setting));
            sqlComm.Parameters.Add(new SqlParameter("@E_Bill_Setting", strE_Bill_Setting));
            sqlComm.Parameters.Add(new SqlParameter("@OutputByTXT_Setting", strOutputByTXT_Setting));
            sqlComm.Parameters.Add(new SqlParameter("@AcctNBR", sAcctNBR));

            return BROTHER_BANK_TEMP.Update(sqlComm);

        }

        /// <summary>
        /// 用Entity方式插入資料庫
        /// </summary>
        /// <param name="eOtherBankTemp">Entity</param>
        /// <returns>true成功,false失敗</returns>
        public static bool AddEntity(EntityOTHER_BANK_TEMP eOtherBankTemp)
        {
            try
            { 
                return eOtherBankTemp.DB_InsertEntity();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
