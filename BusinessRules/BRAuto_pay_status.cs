//******************************************************************
//* 作    者：杨 璐
//* 功能說明：自扣案件處理狀態
//* 創建日期：2012/09/18
//* 修改紀錄：2021/04/01_Ares_Stanley-移除MicrosoftExcel
//* <author>            <time>            <TaskID>                <desc>
//* Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//* Ares_jhun          2022/09/28         RQ-2022-019375-000      EDDA需求調整：調整鍵檔日期、整合EDDA資料
//*******************************************************************

using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using CSIPKeyInGUI.EntityLayer;
using CSIPCommonModel.BusinessRules;
using System.IO;
using Framework.Common.Logging;
using System.Net;
using Framework.Common.Utility;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace CSIPKeyInGUI.BusinessRules
{
    public class BRAuto_pay_status : CSIPCommonModel.BusinessRules.BRBase<EntityAuto_Pay_Status>
    {
        #region SQL語法
        
        // 鍵檔日期若無時間(時分秒)則帶入早上九點(090000)
        private const string SelectWithholdingData = @"
SELECT A.*,
       d.Function_Code,
       d.Popul_NO,
       d.Popul_EmpNo,
       CASE WHEN IsUpdateByTXT = 'Y' THEN 'CSIP' ELSE 'PCTI' END AS DDMEMO,
       d.Case_Class,
       '03'                                                      AS SALES_CHANNEL
FROM (
         -- 中信、郵局
         SELECT a.Receive_Number,
                a.Cus_ID,
                a.Cus_Name,
                a.AccNoBank,
                substring(a.Acc_No, charindex('-', a.Acc_No) + 1, len(a.Acc_No)) AS Acc_No,
                a.Pay_Way,
                a.IsUpdateByTXT,
                a.IsCTCB,
                CASE a.AccNoBank
                    WHEN '042' THEN FORMAT(a.DateTime, 'yyyyMMddHHmmss')
                    ELSE c.ModDate + '090000' END                                AS MATAINDATE,
                b.Acc_ID,
                b.user_id
         FROM auto_pay_status a
                  LEFT JOIN Auto_Pay b ON a.Receive_Number = b.Receive_Number AND b.KeyIn_Flag = '2'
                  LEFT JOIN PostOffice_Temp c ON a.Receive_Number = c.ReceiveNumber
         WHERE a.IsCTCB = 'Y' AND DateTime <= @DateTimeEnd AND DateTime >= @DateTimeStart
         UNION
         -- 他行
         SELECT a.Receive_Number,
                a.Cus_ID,
                a.Cus_Name,
                a.AccNoBank,
                SUBSTRING(a.Acc_No, CHARINDEX('-', a.Acc_No) + 1, LEN(a.Acc_No)) AS Acc_No,
                a.Pay_Way,
                a.IsUpdateByTXT,
                a.IsCTCB,
                convert(varchar(4), substring(c.Build_Date, 1, 4) + 1911) + substring(c.Build_Date, 5, 4) +
                '090000'                                                         AS MATAINDATE,
                c.Other_Bank_Cus_ID                                              AS Acc_ID,
                c.user_id
         FROM auto_pay_status a
                  LEFT JOIN Other_Bank_Temp c ON a.Receive_Number = c.Receive_Number AND c.KeyIn_Flag = '2'
         WHERE IsCTCB = 'N' AND DateTime <= @DateTimeEnd AND DateTime >= @DateTimeStart
         UNION
         -- 訊息更正單
         SELECT a.Receive_Number,
                a.Cus_ID,
                a.Cus_Name,
                a.AccNoBank,
                substring(a.Acc_No, charindex('-', a.Acc_No) + 1, len(a.Acc_No)) AS Acc_No,
                a.Pay_Way,
                a.IsUpdateByTXT,
                a.IsCTCB,
                FORMAT(a.DateTime, 'yyyyMMddHHmmss')                             AS MATAINDATE,
                b.Acc_ID,
                b.user_id
         FROM auto_pay_status a
                  LEFT JOIN Auto_Pay b ON a.Receive_Number = b.Receive_Number AND b.KeyIn_Flag = '2'
         WHERE a.IsCTCB = 'S' AND DateTime <= @DateTimeEnd AND DateTime >= @DateTimeStart
     ) A
         LEFT JOIN Auto_Pay_Popul d ON a.Receive_Number = d.Receive_Number AND d.KeyIn_Flag = '2'
UNION
-- 自扣案件終止作業
SELECT Receive_Number
     , Cus_ID
     , ''                                                       AS Cus_Name
     , CASE WHEN Function_Code = 'D' THEN '' ELSE AccNoBank END AS AccNoBank
     , CASE WHEN Function_Code = 'D' THEN '' ELSE Acc_No END    AS Acc_No
     , 0                                                        AS Pay_Way
     , '-'                                                      AS IsUpdateByTXT
     , '-'                                                      AS IsCTCB
     , FORMAT(mod_date, 'yyyyMMddHHmmss')                       AS MATAINDATE
     , CASE WHEN Function_Code = 'D' THEN '' ELSE Acc_ID END    AS Acc_ID
     , user_id
     , Function_Code
     , Popul_NO
     , Popul_EmpNo
     , CASE WHEN isEnd = 'Y' THEN 'CSIP' ELSE 'PCTI1' END       AS DDMEMO
     , Case_Class
     , '03'                                                     AS SALES_CHANNEL
FROM Auto_Pay_Popul
WHERE KeyIn_Flag = '0' AND mod_date <= @DateTimeEnd AND mod_date >= @DateTimeStart
UNION
-- EDDA
SELECT AUTHCODE,
       CUS_ID,
       ''                                                                    AS Cus_Name,
       OTHER_BANK_CODE_L                                                     AS AccNoBank,
       OTHER_BANK_ACC_NO                                                     AS Acc_No,
       CASE PayWay WHEN '1' THEN '1' ELSE '0' END                            AS Pay_Way,
       'Y'                                                                   AS IsUpdateByTXT,
       '-'                                                                   AS IsCTCB,
       CONCAT(S_DATE + 19110000, '090000')                                   AS MATAINDATE,
       CUS_ID                                                                AS Acc_ID,
       'EDDA'                                                                AS user_id,
       CASE WHEN ComparisonStatus = '1' THEN EBApplyType ELSE Apply_Type END AS Function_Code,
       '00881'                                                               AS Popul_NO,
       SalesEmpoNo                                                           AS Popul_EmpNo,
       'EDDA'                                                                AS DDMEMO,
       ''                                                                    AS Case_Class,
       SalesChannel                                                          AS SALES_CHANNEL
FROM EDDA_Auto_Pay
WHERE UploadFlag = '0' AND ComparisonStatus <> '0' AND Reply_Info IN ('A0', 'A4')";


        #endregion

        #region 自扣案件處理狀態查詢

        /// <summary>
        /// 修改紀錄：20221012_Ares_Jack_ SQL新增週期件代碼
        /// </summary>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <param name="isCTCB"></param>
        /// <param name="isUpdateByTXT"></param>
        /// <param name="custID"></param>
        /// <param name="intPageIndex"></param>
        /// <param name="intPageSize"></param>
        /// <param name="intTotolCount"></param>
        /// <returns></returns>
        public static DataSet GetDataFromtblAuto_Pay_StatusForReport(string dateStart, string dateEnd, string isCTCB, string isUpdateByTXT, string custID,
                                                                        int intPageIndex, int intPageSize, ref int intTotolCount)
        {
            // 若現在時間早于今日晚七點, 且歸檔日期晚于昨日晚七點, 狀態為"待處理";
            // 若現在時間晚于今日晚七點, 且歸檔日期晚于今日晚七點, 狀態為"待處理";
            // 其他情況, 狀態為"產檔完成".
            String dateTimeToday = DateTime.Now.ToString("yyyy-MM-dd") + " 19:00:00";// 今晚七點
            String dateTimeYesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 19:00:00"; // 昨晚七點
            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandType = CommandType.Text;

            #region SQL

            // AccNoBank條件
            string statusAccNoBank = "";
            string populAccNoBank = "";

            if (isCTCB == "Y")
            {
                statusAccNoBank = "and aps.accnoBank = '042' ";
                populAccNoBank = "and app.accnoBank = '042' ";
            }
            else if (isCTCB == "P")
            {
                statusAccNoBank = "and aps.accnoBank = '701' ";
                populAccNoBank = "and app.accnoBank = '701' ";
            }
            else if (isCTCB == "A")
            {
                statusAccNoBank = "and aps.accnoBank in ('042') ";
                populAccNoBank = "and app.accnoBank in ('042') ";
            }

            // AutoPayStatus欄位
            string AutoPayStatusCols = @"
select aps.receive_number, aps.cus_id, aps.cus_name, aps.accnoBank, aps.acc_no, aps.pay_way, 
       case 
            when aps.StatusCode = '9000' then '週期件'
            when aps.StatusCode = '9001' then '週期件(電話更新失敗)'
            when aps.isUpdateByTXT = 'Y' then '由批次作業匯出(變更自扣帳號)' 
            else '由電文直接更新(新增自扣帳號)' end as isUpdateByTXT, 
       case when aps.IsCTCB = 'Y' then '本行' else '他行' end as IsCTCB, convert(varchar(100), DateTime, 111) as DocDate, 
       case when aps.isUpdateByTXT = 'N' then '更新主機完成' 
       else case when ((getDate() < @DateTimeToday and aps.DateTime > @DateTimeYesterday) or 
	                   (getDate() > @DateTimeToday and aps.DateTime > @DateTimeToday)) 
			     then '將待下期帳單結帳前完成設定' else '完成產檔' end end as Status";

            // AutoPay欄位
            string AutoPayCols = @", ap.acc_id, ap.acct_nbr as CardPersonID, convert(varchar(100), convert(datetime, ap.mod_date), 111) as mod_date, ap.bcycle_code, '' as Ach_Batch_date ";

            // 本行或郵局(Auto_Pay_Status.AccNoBank值042為本行701為郵局)
            string IsCTCB = AutoPayStatusCols + AutoPayCols + @"
from Auto_Pay_Status aps 
inner join Auto_Pay ap on aps.receive_number = ap.receive_number and aps.Cus_ID = ap.Cus_ID 
where aps.IsCTCB = 'Y' and aps.IsUpdateByTXT = @IsUpdateByTXT and ap.KeyIn_Flag = '2' 
and ap.mod_date >= @DateStart and ap.mod_date <= @DateEnd and ap.Cus_ID like @Cust_Id " + statusAccNoBank;

            string IsCTCB_AllSetup = AutoPayStatusCols + AutoPayCols + @"
from Auto_Pay_Status aps 
inner join Auto_Pay ap on aps.receive_number = ap.receive_number and aps.Cus_ID = ap.Cus_ID 
where aps.IsCTCB = 'Y' and ap.mod_date >= @DateStart and ap.mod_date <= @DateEnd and ap.KeyIn_Flag = '2' and ap.Cus_ID like @Cust_Id " + statusAccNoBank;

            // 查詢他行加入訊息更正單的資料
            string IsCTCB_S = @"
select aps.receive_number, aps.cus_id, aps.cus_name, aps.accnoBank, aps.acc_no, aps.pay_way, 
       case 
            when aps.StatusCode = '9000' then '週期件'
            when aps.StatusCode = '9001' then '週期件(電話更新失敗)'
            when aps.isUpdateByTXT = 'Y' then '由批次作業匯出(變更自扣帳號)' 
            else '由電文直接更新(新增自扣帳號)' end as isUpdateByTXT, 
       case when aps.IsCTCB = 'Y' then '本行' else '他行' end as IsCTCB, convert(varchar(100),DateTime,111) as DocDate, 
       case when aps.isUpdateByTXT = 'N' then '更新主機完成' 
       else case when ((getDate() < @DateTimeToday and aps.DateTime > @DateTimeYesterday) or 
	                   (getDate() > @DateTimeToday and aps.DateTime > @DateTimeToday)) 
			     then '將待下期帳單結帳前完成設定' else '完成產檔' end end as Status, 
       ap.acc_id, ap.acct_nbr as CardPersonID, convert(varchar(100), convert(datetime, ap.mod_date), 111) as mod_date, ap.bcycle_code, '' as Ach_Batch_date 
from Auto_Pay_Status aps 
inner join Auto_Pay ap on aps.receive_number = ap.receive_number and aps.Cus_ID = ap.Cus_ID 
where aps.IsCTCB = 'S' and ap.KeyIn_Flag = '2' and ap.mod_date >= @DateStart and ap.mod_date <= @DateEnd and ap.Cus_ID like @Cust_Id ";

            // 他行
            string IsNotCTCB = @"
select a.receive_number, a.Other_Bank_Cus_ID as cus_id, a.cus_name, a.Other_Bank_code_S as accnobank, 
       a.Other_Bank_acc_no as acc_no, a.Other_Bank_pay_way as pay_way, 
       case when a.ACH_Return_Code = '0' then 
            case when a.Pcmc_Return_Code = 'PAGE 00 OF 03' or a.Pcmc_Return_Code = 'PAGE 02 OF 03' then '由電文直接更新(新增自扣帳號)' 
                 when a.Pcmc_Return_Code = 'ERROR:0' then '更新失敗-人工刪除' 
                 when a.Pcmc_Return_Code = 'ERROR:O' then 'O檔(ID/帳號異動)' 
                 when a.Pcmc_Return_Code = 'ERROR:10' then '終止件(自來件)' 
                 when a.Pcmc_Return_Code = 'ERROR:9' then '週期件' 
                 when a.Pcmc_Return_Code = '9000' then '週期件' 
                 when a.Pcmc_Return_Code = '9001' then '週期件(電話更新失敗)' 
                 when a.Pcmc_Return_Code = '9002' then '週期件(電文查詢第二卡人檔失敗)' 

            else 
			     case when a.UpFile_Time is NULL then 'PCMC失敗' 
                      when a.UpFile_Time is not NULL then 'PCMC失敗' 
                 else 'PCMC失敗' end 
            end 
            when a.ACH_Return_Code = '1' then 'ACH核印失敗' else 'ACH報送核印中' end as isUpdateByTXT, 
                 '他行' as IsCTCB, substring(mod_date, 1, 4) + '/' + substring(mod_date, 5, 2) + '/' + substring(mod_date, 7, 2) as DocDate, 
       case when a.ACH_Return_Code = '0' then 
            case when a.Pcmc_Return_Code = 'PAGE 00 OF 03' or a.Pcmc_Return_Code = 'PAGE 02 OF 03' then '更新主機完成' 
                 when a.Pcmc_Return_Code = 'ERROR:0' then '人工刪除' 
                 when a.Pcmc_Return_Code = 'ERROR:O' then '待IT完成帳號維護(O),自扣即完成設定' 
                 when a.Pcmc_Return_Code = 'ERROR:10' then '已完成終止' 
                 when a.Pcmc_Return_Code in ('ERROR:9', '9000', '9001', '9002') and a.UpFile_Time is NULL then '自扣帳號將待下期帳單結帳前完成設定' 
                 when a.Pcmc_Return_Code in ('ERROR:9', '9000', '9001', '9002') and a.UpFile_Time is not NULL then '批次產檔完成,自扣帳號將待下期帳單結帳前完成設定' 
            else 
                 case when a.UpFile_Time is NULL then '主機問題件,將以人工處理' 
                      when a.UpFile_Time is not NULL then '主機問題件,將以人工處理' 
                 else '主機問題件,將以人工處理' end 
		    end 
       else b.ACH_Rtn_Msg end as Status, 
       a.Other_Bank_Cus_ID as acc_id, a.acct_nbr as CardPersonID, 
	   substring(mod_date, 1, 4) + '/' + substring(mod_date, 5, 2) + '/' + substring(mod_date, 7, 2) as mod_date, 
       a.bcycle_code, Ach_Batch_date 
from Other_Bank_Temp a 
left join Ach_Rtn_Info b on a.ACH_Return_Code = b.ACH_Rtn_Code 
where a.KeyIn_Flag = '2' and a.apply_type <> 'D' and a.mod_date >= @DateStart and a.mod_date <= @DateEnd and a.Other_Bank_Cus_ID like @Cust_Id ";

            // 本行或郵局新增欄位(Auto_Pay_Popul.AccNoBank值042為本行701為郵局)
            string IsPopul_IsCTCB = @"
select app.receive_number, app.cus_id, app.cus_name, app.accnobank, app.acc_no, '0' as pay_way, 
       '由批次作業匯出(刪除自扣帳號)' as isupdatebytxt, '' as isctcb,convert(varchar(100), app.mod_date,112) as docdate, 
       case when ((getDate() < @DateTimeToday and app.mod_date > @DateTimeYesterday) or 
	              (getDate() > @DateTimeToday and app.mod_date > @DateTimeToday)) 
		    then '將待下期帳單結帳前完成完成自扣帳號刪除' else '完成產檔交由主機處理中' end as Status, 
       app.acc_id, app.cus_id as acct_nbr, app.mod_date, app.bcycle_code, '' as Ach_Batch_date 
from Auto_Pay_Popul app 
left join Auto_Pay ap on app.receive_number = ap.receive_number 
where app.function_code = 'D' and app.KeyIn_Flag = '0' " + populAccNoBank + @"
and convert(varchar(100), app.mod_date, 112) >= @DateStart and convert(varchar(100), app.mod_date, 112) <= @DateEnd and app.Cus_ID like @Cust_Id 
group by app.receive_number, app.cus_id, app.cus_name, app.accnobank, app.acc_no, app.pay_way, app.acc_id, ap.acct_nbr, app.mod_date, app.bcycle_code";

            // 他行新增欄位
            string IsPopul_IsNotCTCB = @"
select a.receive_number, a.cus_id, b.cus_name, a.accnobank, a.acc_no, b.other_bank_pay_way, '由批次作業匯出(刪除自扣帳號)' as isupdatebytxt, 
       '' as isctcb, convert(varchar(100), a.mod_date, 112) as docdate, 
	   case when ((getDate() < @DateTimeToday and a.mod_date > @DateTimeYesterday) or 
	              (getDate() > @DateTimeToday and a.mod_date > @DateTimeToday))  
            then '將待下期帳單結帳前完成完成自扣帳號刪除' else '完成產檔交由主機處理中' end as Status, 
	   a.acc_id, b.acct_nbr, a.mod_date, b.bcycle_code, b.Ach_Batch_date 
from Auto_Pay_Popul a 
left join Other_Bank_Temp b on a.receive_number = b.receive_number 
where a.function_code = 'D' and a.KeyIn_Flag = '0' and a.accNoBank not in ('042', '701') 
and convert(varchar(100), a.mod_date, 112) >= @DateStart and convert(varchar(100), a.mod_date, 112) <= @DateEnd and a.Cus_ID like @Cust_Id 
group by a.receive_number, a.cus_id, b.cus_name, a.accnobank, a.acc_no, b.other_bank_pay_way, a.acc_id, b.acct_nbr, a.mod_date, b.bcycle_code, b.Ach_Batch_date";

            // 郵局 舊資料
            string postoldData = @"select aps.receive_number, aps.cus_id, aps.cus_name, aps.accnoBank, aps.acc_no, aps.pay_way, 
       case 
            when aps.StatusCode = '9000' then '週期件'
            when aps.StatusCode = '9001' then '週期件(電話更新失敗)'
            when aps.isUpdateByTXT = 'Y' then '由批次作業匯出(變更自扣帳號)' 
            else '由電文直接更新(新增自扣帳號)' end as isUpdateByTXT, 
       case when aps.IsCTCB = 'Y' then '本行' else '他行' end as IsCTCB, convert(varchar(100), DateTime, 111) as DocDate, 
       case when aps.isUpdateByTXT = 'N' then '更新主機完成' 
       else case when ((getDate() < @DateTimeToday and aps.DateTime > @DateTimeYesterday) or 
	                   (getDate() > @DateTimeToday and aps.DateTime > @DateTimeToday)) 
			     then '將待下期帳單結帳前完成設定' else '完成產檔' end end as Status, ap.acc_id, ap.acct_nbr as CardPersonID, 
		convert(varchar(100), convert(datetime, ap.mod_date), 111) as mod_date, ap.bcycle_code, '' as Ach_Batch_date
from Auto_Pay_Status aps 
inner join Auto_Pay ap on aps.receive_number = ap.receive_number and aps.Cus_ID = ap.Cus_ID 
where aps.IsCTCB = 'Y' and aps.AccNoBank = '701' and ap.KeyIn_Flag = '2' 
and ap.mod_date >= @DateStart and ap.mod_date <= @DateEnd and ap.mod_date <= '20181130' and ap.Cus_ID like @Cust_Id ";

            // 郵局 上送主機
            string posttohost = @"
select a.ReceiveNumber as receive_number, a.CusID as cus_id, a.CusName as cus_name, a.AccNoBank, a.AccNo acc_no ,b.pay_way,
	case when a.SendHostResult = 'S' then
		case when a.SendHostResultCode = '0000' then '由電文直接更新(新增自扣帳號)'
			when a.SendHostResultCode = '0009' then '週期件'
			when a.SendHostResultCode = '9000' then '週期件'
			when a.SendHostResultCode = '9001' then '週期件(電話更新失敗)'
			when a.SendHostResultCode = '9002' then '週期件(電文查詢第二卡人檔失敗)'
			when a.SendHostResultCode = '0010' then '終止件(自來件)'
			else 'PCMC失敗'
		end
		when a.SendHostResult = 'F' then 'PCMC失敗'
		when a.SendHostResult = 'N' then '郵局核印失敗'
		else '郵局報送核印中'
	end isupdatebytxt,
	N'郵局' IsCTCB,convert(varchar(100), convert(datetime, ModDate), 111) as docdate,
	case when a.SendHostResult = 'S' then
		case when a.SendHostResultCode = '0000' then '更新主機完成'
			when a.SendHostResultCode in ('0009', '9000', '9001', '9002') then '批次產檔完成,自扣帳號將待下期帳單結帳前完成設定'
			when a.SendHostResultCode = '0010' then '已完成終止'
			else '主機問題件,將以人工處理'
		end
        when a.SendHostResult = '' then '郵局尚未回覆'
	else 
	case when c.PostRtnMsg is not null then c.PostRtnMsg
		when d.PostRtnMsg is not null then d.PostRtnMsg end
	end Status,
	a.AccID acc_id,a.CusID as CardPersonID,convert(varchar(100), convert(datetime, ModDate), 111) as mod_date, b.bcycle_code, '' as Ach_Batch_date
from [dbo].[PostOffice_Temp] a with(nolock)
inner join [dbo].[Auto_Pay] b with(nolock) on a.ReceiveNumber = b.Receive_Number and b.KeyIn_Flag = '2' 
left join [dbo].[PostOffice_Rtn_Info] c with(nolock) on a.ReturnStatusTypeCode = c.PostRtnCode and c.RtnType = '1' 
left join [dbo].[PostOffice_Rtn_Info] d with(nolock) on a.ReturnCheckFlagCode = d.PostRtnCode and d.RtnType = '2' 
where a.ModDate >= @DateStart and a.ModDate <= @DateEnd and a.SendHostResultCode not in ('0009', '9000', '9001', '9002') 
	and a.CusID like @Cust_Id";

            // 郵局 上送主機 TEMP
            string posttohostTemp = @"
select a.ReceiveNumber as receive_number, a.CusID as cus_id, a.CusName as cus_name, a.AccNoBank, a.AccNo acc_no ,b.pay_way,
	case when a.SendHostResult = 'S' then
		case when a.SendHostResultCode = '0000' then '由電文直接更新(新增自扣帳號)'
			when a.SendHostResultCode = '0009' then '週期件'
            when a.SendHostResultCode = '9000' then '週期件'
			when a.SendHostResultCode = '9001' then '週期件(電話更新失敗)'
			when a.SendHostResultCode = '9002' then '週期件(電文查詢第二卡人檔失敗)'
			when a.SendHostResultCode = '0010' then '終止件(自來件)'
			else 'PCMC失敗'
		end
		when a.SendHostResult = 'F' then 'PCMC失敗'
		when a.SendHostResult = 'N' then '郵局核印失敗'
		else '郵局報送核印中'
	end isupdatebytxt,
	N'郵局' IsCTCB,convert(varchar(100), convert(datetime, ModDate), 111) as docdate,
	case when a.SendHostResult = 'S' then
		case when a.SendHostResultCode = '0000' then '更新主機完成'
			when a.SendHostResultCode in ('0009', '9000', '9001', '9002') then '批次產檔完成,自扣帳號將待下期帳單結帳前完成設定'
			when a.SendHostResultCode = '0010' then '已完成終止'
			else '主機問題件,將以人工處理'
		end
	else '主機問題件,將以人工處理'
	end Status,
	a.AccID acc_id,a.CusID as CardPersonID,convert(varchar(100), convert(datetime, ModDate), 111) as mod_date, b.bcycle_code, '' as Ach_Batch_date
from [dbo].[PostOffice_Temp] a with(nolock)
inner join [dbo].[Auto_Pay] b with(nolock) on a.ReceiveNumber = b.Receive_Number and b.KeyIn_Flag = '2' 
where a.ModDate >= @DateStart and a.ModDate <= @DateEnd and a.SendHostResultCode in ('0009', '9000', '9001', '9002') 
		and a.CusID like @Cust_Id
union all
select a.Receive_Number receive_number, a.Cus_ID cus_id, '' CusName , a.AccNoBank, a.acc_no acc_no,a.pay_way, 
	'由批次作業匯出(刪除自扣帳號)' as isUpdateByTXT, N'郵局' IsCTCB,convert(varchar(100), mod_date, 111) as docdate,
	case when ((getDate() < @DateTimeToday and a.mod_date > @DateTimeYesterday) or 
			(getDate() > @DateTimeToday and a.mod_date > @DateTimeToday))  
		then '將待下期帳單結帳前完成完成自扣帳號刪除' else '完成產檔交由主機處理中' 
	end as Status,
	a.Acc_ID AccID,a.Cus_ID as acct_nbr,convert(varchar(100), mod_date, 111) as mod_date, '' bcycle_code, '' as Ach_Batch_date
from Auto_Pay_Popul a with(nolock) 
where a.Receive_Number = '' and mod_date >= @DateStart and mod_date <= @DateEnd and a.Cus_ID like @Cust_Id";

            #endregion

            string sql = "";

            // isCTCB = Y(本行自扣), P(郵局自扣), N(他行自扣), A(本行+郵局+他行自扣)
            // isUpdateByTXT = N(上送主機), Y(主機Temp檔), A(上送主機+主機Temp檔)
            //20210616_Ares_Stanley-增加排序，收件編號小至大
            switch (isCTCB)
            {
                case "Y":// 本行自扣
                    if (isUpdateByTXT == "N")// 上送主機
                    {
                        sql = IsCTCB;
                    }
                    else if (isUpdateByTXT == "Y")// 主機Temp檔
                    {
                        sql = IsCTCB + " Union All " + IsPopul_IsCTCB;
                    }
                    else// 上送主機+主機Temp檔
                    {
                        sql = IsCTCB_AllSetup + " Union All " + IsPopul_IsCTCB;
                    }

                    sqlComm.CommandText = sql + " order by receive_number ";
                    sqlComm.Parameters.Add(new SqlParameter("@IsCTCB", isCTCB));

                    break;
                case "P":// 郵局自扣
                    if (isUpdateByTXT == "N")// 上送主機
                    {
                        sql = postoldData + " Union All " + posttohost;
                    }
                    else if (isUpdateByTXT == "Y")// 主機Temp檔
                    {
                        sql = posttohostTemp;
                    }
                    else// 上送主機+主機Temp檔
                    {
                        sql = postoldData + " Union All " + posttohost + " Union All " + posttohostTemp;
                    }

                    sqlComm.CommandText = sql + " order by receive_number ";

                    break;
                case "N":// 他行自扣
                    if (isUpdateByTXT == "N")// 上送主機
                    {
                        sql = IsCTCB_S + " Union All " + IsNotCTCB + " and a.Pcmc_Return_Code not in ('ERROR:9', '9000', '9001', '9002') ";
                    }
                    else if (isUpdateByTXT == "Y")// 主機Temp檔
                    {
                        sql = IsNotCTCB + " and a.Pcmc_Return_Code <> 'PAGE 00 OF 03' and a.Pcmc_Return_Code <> 'PAGE 02 OF 03'" +
                                " Union All " + IsPopul_IsNotCTCB;
                    }
                    else// 上送主機+主機Temp檔
                    {
                        sql = IsCTCB_S + " Union All " + IsNotCTCB + " Union All " + IsPopul_IsNotCTCB;
                    }

                    sqlComm.CommandText = sql + " order by receive_number ";

                    break;
                case "A":// 本行+郵局+他行自扣
                    if (isUpdateByTXT == "N")// 上送主機
                    {
                        sql = IsCTCB_S + " Union All " + postoldData + " Union All " + posttohost + " Union All " + IsCTCB + " Union All " + IsNotCTCB + " and a.Pcmc_Return_Code not in ('ERROR:9', '9000', '9001', '9002')";
                    }
                    else if (isUpdateByTXT == "Y")// 主機Temp檔
                    {
                        sql = IsCTCB + "Union All " + IsNotCTCB + " and a.Pcmc_Return_Code <> 'PAGE 00 OF 03' and a.Pcmc_Return_Code <> 'PAGE 02 OF 03'" +
                              " Union All " + posttohostTemp + " Union All " + IsPopul_IsCTCB + " Union All " + IsPopul_IsNotCTCB;
                    }
                    else// 上送主機+主機Temp檔
                    {
                        sql = IsCTCB_S + " Union All " + postoldData + " Union All " + posttohost + " Union All " + posttohostTemp + " Union All " + IsCTCB_AllSetup + " Union All " + IsNotCTCB + " Union All " + IsPopul_IsCTCB + " Union All " + IsPopul_IsNotCTCB;
                    }

                    sqlComm.CommandText = sql + " order by receive_number ";
                    sqlComm.Parameters.Add(new SqlParameter("@IsCTCB", isCTCB));

                    break;
            }

            sqlComm.Parameters.Add(new SqlParameter("@DateTimeToday", dateTimeToday));
            sqlComm.Parameters.Add(new SqlParameter("@DateTimeYesterday", dateTimeYesterday));
            sqlComm.Parameters.Add(new SqlParameter("@DateEnd", dateEnd));
            sqlComm.Parameters.Add(new SqlParameter("@DateStart", dateStart));
            sqlComm.Parameters.Add(new SqlParameter("@IsUpdateByTXT", isUpdateByTXT));
            sqlComm.Parameters.Add(new SqlParameter("@Cust_Id", "%" + custID + "%"));

            Logging.Log(isCTCB + "," + isUpdateByTXT, LogLayer.Util);
            Logging.Log(sql, LogLayer.Util);

            DataSet ds = new DataSet();
            ds = SearchOnDataSet(sqlComm, intPageIndex, intPageSize, ref intTotolCount);

            return ds;
        }
        /// <summary>
        /// 修改紀錄：20221012_Ares_Jack_ SQL新增週期件代碼
        /// </summary>
        /// <param name="strDateStart"></param>
        /// <param name="strDateEnd"></param>
        /// <param name="strIsCTCB"></param>
        /// <param name="strIsUpdateByTXT"></param>
        /// <param name="strcust_id"></param>
        /// <returns></returns>
        public static DataSet GetDataFromtblAuto_Pay_StatusForReportWithoutPaging(string strDateStart, string strDateEnd, string strIsCTCB, string strIsUpdateByTXT, string strcust_id)
        {
            String strDateTimeToday = DateTime.Now.ToString("yyyy-MM-dd") + " 19:00:00"; //今晚七點
            String strDateTimeYesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 19:00:00"; //昨晚七點
            //若現在時間早于今日晚七點, 且歸檔日期晚于昨日晚七點, 狀態為"待處理";
            //若現在時間晚于今日晚七點, 且歸檔日期晚于今日晚七點, 狀態為"待處理";
            //其他情況, 狀態為"產檔完成".
            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandType = CommandType.Text;
            string strSql_AutoPayStatusCols = "select auto_pay_status.receive_number, auto_pay_status.cus_id, auto_pay_status.cus_name, auto_pay_status.accnoBank, auto_pay_status.acc_no, auto_pay_status.pay_way," +
                        " case " +
                        " when auto_pay_status.StatusCode = '9000' then '週期件' " +
                        " when auto_pay_status.StatusCode = '9001' then '週期件(電話更新失敗)' " +
                        " when auto_pay_status.isUpdateByTXT='Y' then '由批次作業匯出(變更自扣帳號)' " +
                        " else '由電文直接更新(新增自扣帳號)' end as isUpdateByTXT," +
                        " case when auto_pay_status.IsCTCB='Y' then '本行' else '他行' end as IsCTCB, convert(varchar(100),DateTime,111) as DocDate," +
                        " case when auto_pay_status.isUpdateByTXT='N' then '更新主機完成' else case when ((getDate()< @strDateTimeToday and auto_pay_status.DateTime> @strDateTimeYesterday) or(getDate()> @strDateTimeToday and auto_pay_status.DateTime> @strDateTimeToday)) then '將待下期帳單結帳前完成設定' else '完成產檔' end end as Status";

            string strSql_AutoPayCols = ",auto_pay.acc_id,auto_pay.acct_nbr as CardPersonID, convert(varchar(100),convert(datetime,auto_pay.mod_date),111) as mod_date, auto_pay.bcycle_code,'' as Ach_Batch_date";

            string strSql_IsCTCB = strSql_AutoPayStatusCols + strSql_AutoPayCols +
                        " from auto_pay_status  inner join auto_pay on auto_pay_status.receive_number=auto_pay.receive_number and auto_pay_status.Cus_ID=auto_pay.Cus_ID" +
                        " where auto_pay_status.IsCTCB='Y' and auto_pay_status.IsUpdateByTXT=@IsUpdateByTXT  and auto_pay.mod_date>=@DateStart and auto_pay.mod_date<=@DateEnd and auto_pay.KeyIn_Flag='2' and auto_pay.Cus_ID like @Cust_Id ";

            string strSql_IsCTCB_AllSetup = strSql_AutoPayStatusCols + strSql_AutoPayCols +
                        " from auto_pay_status  inner join auto_pay on auto_pay_status.receive_number=auto_pay.receive_number and auto_pay_status.Cus_ID=auto_pay.Cus_ID" +
                        " where auto_pay_status.IsCTCB='Y' and auto_pay.mod_date>=@DateStart and auto_pay.mod_date<=@DateEnd and auto_pay.KeyIn_Flag='2' and auto_pay.Cus_ID like @Cust_Id AND auto_pay_status.accnoBank != '701'";

            //查詢他行加入訊息更正單的資料
            string strSql_IsCTCB_S = strSql_AutoPayStatusCols + strSql_AutoPayCols +
                        " from auto_pay_status  inner join auto_pay on auto_pay_status.receive_number=auto_pay.receive_number and auto_pay_status.Cus_ID=auto_pay.Cus_ID" +
                        " where auto_pay_status.IsCTCB='S' and auto_pay.mod_date>=@DateStart and auto_pay.mod_date<=@DateEnd and auto_pay.KeyIn_Flag='2' and auto_pay.Cus_ID like @Cust_Id ";

            string strSql_IsNotCTCB = " select a.receive_number, " +
                        //" a.cus_id, a.cus_name,a.Other_Bank_code_S as accnobank,a.Other_Bank_acc_no as acc_no,a.Other_Bank_pay_way as pay_way, " +
                        " a.Other_Bank_Cus_ID as cus_id, a.cus_name,a.Other_Bank_code_S as accnobank,a.Other_Bank_acc_no as acc_no,a.Other_Bank_pay_way as pay_way, " +
                        " case when a.ACH_Return_Code = '0' then " +
             "case when a.Pcmc_Return_Code = 'PAGE 00 OF 03' or a.Pcmc_Return_Code='PAGE 02 OF 03' then '由電文直接更新(新增自扣帳號)' " +
                   "when a.Pcmc_Return_Code ='ERROR:0' then '更新失敗-人工刪除'" +
                   "when a.Pcmc_Return_Code ='ERROR:O' then 'O檔(ID/帳號異動)'" +
                   //"when a.Pcmc_Return_Code ='ERROR:10' then '更新失敗-案件類別為D不為P02D'"+
                   "when a.Pcmc_Return_Code ='ERROR:10' then '終止件(自來件)'" +
                   "when a.Pcmc_Return_Code ='ERROR:9'  then '週期件'" +
                   "when a.Pcmc_Return_Code ='9000'  then '週期件'" +
                   "when a.Pcmc_Return_Code ='9001'  then '週期件(電話更新失敗)'" +
                   "when a.Pcmc_Return_Code ='9002'  then '週期件(電文查詢第二卡人檔失敗)'" +
                   "else " +
                "case when a.UpFile_Time is NULL then 'PCMC失敗'" +
                     "when a.UpFile_Time is not NULL then 'PCMC失敗'" +
            "else 'PCMC失敗' end end when a.ACH_Return_Code = '1' then 'ACH核印失敗'  else 'ACH報送核印中' end as isUpdateByTXT, '他行'as IsCTCB,substring(mod_date,1,4)+'/'+substring(mod_date,5,2)+'/'+substring(mod_date,7,2) as DocDate, " +
            " case when a.ACH_Return_Code = '0' then " +
                 " case when a.Pcmc_Return_Code = 'PAGE 00 OF 03' or a.Pcmc_Return_Code='PAGE 02 OF 03' then '更新主機完成' " +
                "	   when a.Pcmc_Return_Code ='ERROR:0' then '人工刪除'" +
                "	   when a.Pcmc_Return_Code ='ERROR:O' then '待IT完成帳號維護(O),自扣即完成設定'" +
                //"	   when a.Pcmc_Return_Code ='ERROR:10' then '案件類別為D不為P02D'" +
                "	   when a.Pcmc_Return_Code ='ERROR:10' then '已完成終止'" +
                "	   when a.Pcmc_Return_Code in ('ERROR:9', '9000', '9001', '9002') and a.UpFile_Time is NULL then '自扣帳號將待下期帳單結帳前完成設定'" +
                "	   when a.Pcmc_Return_Code in ('ERROR:9', '9000', '9001', '9002') and a.UpFile_Time is not NULL then '批次產檔完成,自扣帳號將待下期帳單結帳前完成設定'" +
                " else " +
                "	case when a.UpFile_Time is NULL then '主機問題件,將以人工處理'" +
                "		 when a.UpFile_Time is not NULL then '主機問題件,將以人工處理'" +
                "	else '主機問題件,將以人工處理' end end else b.ACH_Rtn_Msg end as Status," +
            " a.Other_Bank_Cus_ID as acc_id,a.acct_nbr as CardPersonID, substring(mod_date,1,4)+'/'+substring(mod_date,5,2)+'/'+substring(mod_date,7,2) as mod_date, " +
            " a.bcycle_code,Ach_Batch_date from Other_Bank_Temp a " +
            " left join Ach_Rtn_Info b " +
            " on a.ACH_Return_Code = b.ACH_Rtn_Code " +
            " where a.mod_date>=@DateStart and a.mod_date<=@DateEnd and a.KeyIn_Flag='2'  and a.Other_Bank_Cus_ID like @Cust_Id and a.apply_type <>'D' ";


            string strSql_IsPopul_IsCTCB = "select a.receive_number,a.cus_id,a.cus_name,a.accnobank,a.acc_no,'0' as pay_way,'由批次作業匯出(刪除自扣帳號)' as isupdatebytxt " +
                                            " ,'' as isctcb,convert(varchar(100),a.mod_date,112) as docdate,case when ((getDate()< @strDateTimeToday and a.mod_date> @strDateTimeYesterday)  " +
                                            " or(getDate()> @strDateTimeToday and a.mod_date> @strDateTimeToday))  " +
                                            " then '將待下期帳單結帳前完成完成自扣帳號刪除' else '完成產檔交由主機處理中'  end as Status" +
                                            " ,a.acc_id,a.cus_id as acct_nbr,a.mod_date,a.bcycle_code,'' as Ach_Batch_date from Auto_Pay_Popul a left join auto_pay b " +
                                            " on a.receive_number=b.receive_number " +
                                            " where convert(varchar(100),a.mod_date,112)>=@DateStart and convert(varchar(100),a.mod_date,112)<=@DateEnd " +
                                            " and a.function_code='D' and a.KeyIn_Flag='0' and a.Cus_ID like @Cust_Id and a.accNoBank in ('042') " +
                                            " group by a.receive_number,a.cus_id,a.cus_name,a.accnobank,a.acc_no,a.pay_way " +
                                            " ,a.acc_id,b.acct_nbr,a.mod_date,a.bcycle_code ";

            string strSql_IsPopul_IsNotCTCB = "select a.receive_number,a.cus_id,b.cus_name,a.accnobank,a.acc_no,b.other_bank_pay_way,'由批次作業匯出(刪除自扣帳號)' as isupdatebytxt " +
                                              " ,'' as isctcb,convert(varchar(100),a.mod_date,112) as docdate,case when ((getDate()< @strDateTimeToday and a.mod_date> @strDateTimeYesterday)  " +
                                              " or(getDate()> @strDateTimeToday and a.mod_date> @strDateTimeToday))  " +
                                              " then '將待下期帳單結帳前完成完成自扣帳號刪除' else '完成產檔交由主機處理中'  end as Status" +
                                              " ,a.acc_id,b.acct_nbr,a.mod_date,b.bcycle_code,b.Ach_Batch_date from Auto_Pay_Popul a left join Other_Bank_Temp b " +
                                              " on a.receive_number=b.receive_number " +
                                              " where convert(varchar(100),a.mod_date,112)>=@DateStart and convert(varchar(100),a.mod_date,112)<=@DateEnd " +
                                              " and a.function_code='D' and a.KeyIn_Flag='0' and a.Cus_ID like @Cust_Id and a.accNoBank not in ('042','071') " +
                                              " group by a.receive_number,a.cus_id,b.cus_name,a.accnobank,a.acc_no,b.other_bank_pay_way " +
                                              " ,a.acc_id,b.acct_nbr,a.mod_date,b.bcycle_code,b.Ach_Batch_date ";

            string postOldData = @"select auto_pay_status.receive_number, auto_pay_status.cus_id, auto_pay_status.cus_name, auto_pay_status.accnoBank, auto_pay_status.acc_no, auto_pay_status.pay_way, " +
                                              " case " +
                                              " when auto_pay_status.StatusCode = '9000' then '週期件' " +
                                              " when auto_pay_status.StatusCode = '9001' then '週期件(電話更新失敗)' " +
                                              " when auto_pay_status.isUpdateByTXT='Y' then '由批次作業匯出(變更自扣帳號)' " +
                                              " else '由電文直接更新(新增自扣帳號)' end as isUpdateByTXT, " +
                                              " case when auto_pay_status.IsCTCB='Y' then '本行' else '他行' end as IsCTCB, convert(varchar(100),DateTime,111) as DocDate, " +
                                              " case when auto_pay_status.isUpdateByTXT='N' then '更新主機完成' else case when ((getDate()< @strDateTimeToday and auto_pay_status.DateTime> @strDateTimeYesterday) or(getDate()> @strDateTimeToday and auto_pay_status.DateTime> @strDateTimeToday)) then '將待下期帳單結帳前完成設定' else '完成產檔' end end as Status, " +
                                              " auto_pay.acc_id,auto_pay.acct_nbr as CardPersonID, convert(varchar(100),convert(datetime,auto_pay.mod_date),111) as mod_date, auto_pay.bcycle_code,'' as Ach_Batch_date " +
                                              " from auto_pay_status  inner join auto_pay on auto_pay_status.receive_number=auto_pay.receive_number and auto_pay_status.Cus_ID=auto_pay.Cus_ID " +
                                              " where auto_pay_status.IsCTCB='Y' and auto_pay_status.AccNoBank = '701'  " +
                                              " and auto_pay.mod_date>=@DateStart and auto_pay.mod_date<=@DateEnd and auto_pay.KeyIn_Flag='2' and auto_pay.Cus_ID like @Cust_Id ";

            // 郵局 上送主機
            string posttohost = @"
select a.ReceiveNumber as receive_number, a.CusID as cus_id, a.CusName as cus_name, a.AccNoBank, a.AccNo acc_no ,b.pay_way,
	case when a.SendHostResult = 'S' then
		case when a.SendHostResultCode = '0000' then '由電文直接更新(新增自扣帳號)'
			when a.SendHostResultCode = '0009' then '週期件'
			when a.SendHostResultCode = '9000' then '週期件'
			when a.SendHostResultCode = '9001' then '週期件(電話更新失敗)'
			when a.SendHostResultCode = '9002' then '週期件(電文查詢第二卡人檔失敗)'
			when a.SendHostResultCode = '0010' then '終止件(自來件)'
			else 'PCMC失敗'
		end
		when a.SendHostResult = 'F' then 'PCMC失敗'
		when a.SendHostResult = 'N' then '郵局核印失敗'
		else '郵局報送核印中'
	end isupdatebytxt,
	N'郵局' IsCTCB,convert(varchar(100), convert(datetime, ModDate), 111) as docdate,
	case when a.SendHostResult = 'S' then
		case when a.SendHostResultCode = '0000' then '更新主機完成'
			when a.SendHostResultCode in ('0009', '9000', '9001', '9002') then '批次產檔完成,自扣帳號將待下期帳單結帳前完成設定'
			when a.SendHostResultCode = '0010' then '已完成終止'
			else '主機問題件,將以人工處理'
		end
        when a.SendHostResult = '' then '郵局尚未回覆'
	else 
	case when c.PostRtnMsg is not null then c.PostRtnMsg
		when d.PostRtnMsg is not null then d.PostRtnMsg end
	end Status,
	a.AccID acc_id,a.CusID as CardPersonID,convert(varchar(100), convert(datetime, ModDate), 111) as mod_date, b.bcycle_code, '' as Ach_Batch_date
from [dbo].[PostOffice_Temp] a with(nolock)
inner join [dbo].[Auto_Pay] b with(nolock) on a.ReceiveNumber = b.Receive_Number and b.KeyIn_Flag = '2' 
left join [dbo].[PostOffice_Rtn_Info] c with(nolock) on a.ReturnStatusTypeCode = c.PostRtnCode and c.RtnType = '1' 
left join [dbo].[PostOffice_Rtn_Info] d with(nolock) on a.ReturnCheckFlagCode = d.PostRtnCode and d.RtnType = '2' 
where a.ModDate >= @DateStart and a.ModDate <= @DateEnd and a.SendHostResultCode not in ('0009', '9000', '9001', '9002') 
	and a.CusID like @Cust_Id";

            // 郵局 上送主機 TEMP
            string posttohostTemp = @"
select a.ReceiveNumber as receive_number, a.CusID as cus_id, a.CusName as cus_name, a.AccNoBank, a.AccNo acc_no ,b.pay_way,
	case when a.SendHostResult = 'S' then
		case when a.SendHostResultCode = '0000' then '由電文直接更新(新增自扣帳號)'
			when a.SendHostResultCode = '0009' then '週期件'
			when a.SendHostResultCode = '9000' then '週期件'
			when a.SendHostResultCode = '9001' then '週期件(電話更新失敗)'
			when a.SendHostResultCode = '9002' then '週期件(電文查詢第二卡人檔失敗)'
			when a.SendHostResultCode = '0010' then '終止件(自來件)'
			else 'PCMC失敗'
		end
		when a.SendHostResult = 'F' then 'PCMC失敗'
		when a.SendHostResult = 'N' then '郵局核印失敗'
		else '郵局報送核印中'
	end isupdatebytxt,
	N'郵局' IsCTCB,convert(varchar(100), convert(datetime, ModDate), 111) as docdate,
	case when a.SendHostResult = 'S' then
		case when a.SendHostResultCode = '0000' then '更新主機完成'
			when a.SendHostResultCode in ('0009', '9000', '9001', '9002') then '批次產檔完成,自扣帳號將待下期帳單結帳前完成設定'
			when a.SendHostResultCode = '0010' then '已完成終止'
			else '主機問題件,將以人工處理'
		end
	else '主機問題件,將以人工處理'
	end Status,
	a.AccID acc_id,a.CusID as CardPersonID,convert(varchar(100), convert(datetime, ModDate), 111) as mod_date, b.bcycle_code, '' as Ach_Batch_date
from [dbo].[PostOffice_Temp] a with(nolock)
inner join [dbo].[Auto_Pay] b with(nolock) on a.ReceiveNumber = b.Receive_Number and b.KeyIn_Flag = '2' 
where a.ModDate >= @DateStart and a.ModDate <= @DateEnd and a.SendHostResultCode in ('0009', '9000', '9001', '9002') 
		and a.CusID like @Cust_Id
union all
select a.Receive_Number receive_number, a.Cus_ID cus_id, '' CusName , a.AccNoBank, a.acc_no acc_no,a.pay_way, 
	'由批次作業匯出(刪除自扣帳號)' as isUpdateByTXT, N'郵局' IsCTCB,convert(varchar(100), mod_date, 111) as docdate,
	case when ((getDate() < @DateTimeToday and a.mod_date > @DateTimeYesterday) or 
			(getDate() > @DateTimeToday and a.mod_date > @DateTimeToday))  
		then '將待下期帳單結帳前完成完成自扣帳號刪除' else '完成產檔交由主機處理中' 
	end as Status,
	a.Acc_ID AccID,a.Cus_ID as acct_nbr,convert(varchar(100), mod_date, 111) as mod_date, '' bcycle_code, '' as Ach_Batch_date
from Auto_Pay_Popul a with(nolock) 
where a.Receive_Number = '' and mod_date >= @DateStart and mod_date <= @DateEnd and a.Cus_ID like @Cust_Id";

            string strSql = "";
            // 本行自扣、上送主機
            if (strIsCTCB == "Y" && strIsUpdateByTXT == "N")
            {
                strSql = strSql_IsCTCB;
                sqlComm.CommandText = strSql;
                sqlComm.Parameters.Add(new SqlParameter("@DateEnd", strDateEnd));
                sqlComm.Parameters.Add(new SqlParameter("@DateStart", strDateStart));
                sqlComm.Parameters.Add(new SqlParameter("@IsCTCB", strIsCTCB));
                sqlComm.Parameters.Add(new SqlParameter("@IsUpdateByTXT", strIsUpdateByTXT));
                sqlComm.Parameters.Add(new SqlParameter("@Cust_Id", "%" + strcust_id + "%"));

            }
            // 本行自扣、主機Temp檔
            else if (strIsCTCB == "Y" && strIsUpdateByTXT == "Y")
            {
                strSql = strSql_IsCTCB + " Union All " + strSql_IsPopul_IsCTCB;
                sqlComm.CommandText = strSql;
                sqlComm.Parameters.Add(new SqlParameter("@DateEnd", strDateEnd));
                sqlComm.Parameters.Add(new SqlParameter("@DateStart", strDateStart));
                sqlComm.Parameters.Add(new SqlParameter("@IsCTCB", strIsCTCB));
                sqlComm.Parameters.Add(new SqlParameter("@IsUpdateByTXT", strIsUpdateByTXT));
                sqlComm.Parameters.Add(new SqlParameter("@Cust_Id", "%" + strcust_id + "%"));
            }
            // 本行自扣、上送主機+主機Temp檔
            else if (strIsCTCB == "Y" && strIsUpdateByTXT == "A")
            {
                strSql = strSql_IsCTCB_AllSetup + " Union All " + strSql_IsPopul_IsCTCB;
                sqlComm.CommandText = strSql;
                sqlComm.Parameters.Add(new SqlParameter("@DateEnd", strDateEnd));
                sqlComm.Parameters.Add(new SqlParameter("@DateStart", strDateStart));
                sqlComm.Parameters.Add(new SqlParameter("@IsCTCB", strIsCTCB));
                sqlComm.Parameters.Add(new SqlParameter("@Cust_Id", "%" + strcust_id + "%"));

            }
            // 郵局自扣、上送主機
            else if (strIsCTCB == "P" && strIsUpdateByTXT == "N")
            {
                strSql = postOldData + " Union All " + posttohost;
                sqlComm.CommandText = strSql;
                sqlComm.Parameters.Add(new SqlParameter("@DateEnd", strDateEnd));
                sqlComm.Parameters.Add(new SqlParameter("@DateStart", strDateStart));
                sqlComm.Parameters.Add(new SqlParameter("@Cust_Id", "%" + strcust_id + "%"));

            }
            // 郵局自扣、主機Temp檔
            else if (strIsCTCB == "P" && strIsUpdateByTXT == "Y")
            {
                strSql = posttohostTemp;
                sqlComm.CommandText = strSql;
                sqlComm.Parameters.Add(new SqlParameter("@DateEnd", strDateEnd));
                sqlComm.Parameters.Add(new SqlParameter("@DateStart", strDateStart));
                sqlComm.Parameters.Add(new SqlParameter("@Cust_Id", "%" + strcust_id + "%"));
                sqlComm.Parameters.Add(new SqlParameter("@DateTimeToday", strDateTimeToday));
                sqlComm.Parameters.Add(new SqlParameter("@DateTimeYesterday", strDateTimeYesterday));

            }
            // 郵局自扣、上送主機+主機Temp檔
            else if (strIsCTCB == "P" && strIsUpdateByTXT == "A")
            {
                strSql = postOldData + " Union All " + posttohost + " Union All " + posttohostTemp;
                sqlComm.CommandText = strSql;
                sqlComm.Parameters.Add(new SqlParameter("@DateEnd", strDateEnd));
                sqlComm.Parameters.Add(new SqlParameter("@DateStart", strDateStart));
                sqlComm.Parameters.Add(new SqlParameter("@Cust_Id", "%" + strcust_id + "%"));
                sqlComm.Parameters.Add(new SqlParameter("@DateTimeToday", strDateTimeToday));
                sqlComm.Parameters.Add(new SqlParameter("@DateTimeYesterday", strDateTimeYesterday));
            }
            // 他行自扣、主機Temp檔
            else if (strIsCTCB == "N" && strIsUpdateByTXT == "Y")
            {
                strSql = strSql_IsNotCTCB + " and a.Pcmc_Return_Code <> 'PAGE 00 OF 03' and a.Pcmc_Return_Code <> 'PAGE 02 OF 03'  and a.apply_type <>'D' " + " Union All " + strSql_IsPopul_IsNotCTCB;
                sqlComm.CommandText = strSql;
                sqlComm.Parameters.Add(new SqlParameter("@DateEnd", strDateEnd));
                sqlComm.Parameters.Add(new SqlParameter("@DateStart", strDateStart));
                sqlComm.Parameters.Add(new SqlParameter("@Cust_Id", "%" + strcust_id + "%"));

            }
            // 他行自扣、上送主機
            else if (strIsCTCB == "N" && strIsUpdateByTXT == "N")
            {
                strSql = strSql_IsCTCB_S + " Union All " + strSql_IsNotCTCB + " and a.Pcmc_Return_Code not in ('ERROR:9', '9000', '9001', '9002') ";
                sqlComm.CommandText = strSql;
                sqlComm.Parameters.Add(new SqlParameter("@DateEnd", strDateEnd));
                sqlComm.Parameters.Add(new SqlParameter("@DateStart", strDateStart));
                sqlComm.Parameters.Add(new SqlParameter("@Cust_Id", "%" + strcust_id + "%"));

            }
            // 他行自扣、上送主機+主機Temp檔
            else if (strIsCTCB == "N" && strIsUpdateByTXT == "A")
            {
                strSql = strSql_IsCTCB_S + " Union All " + strSql_IsNotCTCB + " Union All " + strSql_IsPopul_IsNotCTCB;
                sqlComm.CommandText = strSql;
                sqlComm.Parameters.Add(new SqlParameter("@DateEnd", strDateEnd));
                sqlComm.Parameters.Add(new SqlParameter("@DateStart", strDateStart));
                sqlComm.Parameters.Add(new SqlParameter("@Cust_Id", "%" + strcust_id + "%"));
            }
            // 本行+郵局+他行自扣、主機Temp檔
            else if (strIsCTCB == "A" && strIsUpdateByTXT == "Y")
            {
                strSql = strSql = strSql_IsCTCB + " Union All " + posttohostTemp + " Union All " + strSql_IsNotCTCB + " and a.Pcmc_Return_Code <> 'PAGE 00 OF 03' and a.Pcmc_Return_Code <> 'PAGE 02 OF 03'  and a.apply_type <>'D' " + " Union All " + strSql_IsPopul_IsCTCB + " Union All " + strSql_IsPopul_IsNotCTCB; ;
                sqlComm.CommandText = strSql;
                sqlComm.Parameters.Add(new SqlParameter("@DateEnd", strDateEnd));
                sqlComm.Parameters.Add(new SqlParameter("@DateStart", strDateStart));
                sqlComm.Parameters.Add(new SqlParameter("@IsCTCB", strIsCTCB));
                sqlComm.Parameters.Add(new SqlParameter("@IsUpdateByTXT", strIsUpdateByTXT));
                sqlComm.Parameters.Add(new SqlParameter("@Cust_Id", "%" + strcust_id + "%"));
                sqlComm.Parameters.Add(new SqlParameter("@DateTimeToday", strDateTimeToday));
                sqlComm.Parameters.Add(new SqlParameter("@DateTimeYesterday", strDateTimeYesterday));
            }
            // 本行+郵局+他行自扣、上送主機
            else if (strIsCTCB == "A" && strIsUpdateByTXT == "N")
            {
                strSql = strSql = strSql_IsCTCB_S + " Union All " + postOldData + " Union All " + posttohost + " Union All " + strSql_IsCTCB + "Union All " + strSql_IsNotCTCB + " and a.Pcmc_Return_Code not in ('ERROR:9', '9000', '9001', '9002') ";
                sqlComm.CommandText = strSql;
                sqlComm.Parameters.Add(new SqlParameter("@DateEnd", strDateEnd));
                sqlComm.Parameters.Add(new SqlParameter("@DateStart", strDateStart));
                sqlComm.Parameters.Add(new SqlParameter("@IsCTCB", strIsCTCB));
                sqlComm.Parameters.Add(new SqlParameter("@IsUpdateByTXT", strIsUpdateByTXT));
                sqlComm.Parameters.Add(new SqlParameter("@Cust_Id", "%" + strcust_id + "%"));
            }
            // 本行+郵局+他行自扣、上送主機+主機Temp檔
            else
            {
                strSql = strSql_IsCTCB_S + " Union All " + postOldData + " Union All " + posttohost + " Union All " + posttohostTemp + " Union All " + strSql_IsCTCB_AllSetup + "Union All " + strSql_IsNotCTCB + " Union All " + strSql_IsPopul_IsCTCB + " Union All " + strSql_IsPopul_IsNotCTCB; ;

                sqlComm.CommandText = strSql;
                sqlComm.Parameters.Add(new SqlParameter("@DateEnd", strDateEnd));
                sqlComm.Parameters.Add(new SqlParameter("@DateStart", strDateStart));
                sqlComm.Parameters.Add(new SqlParameter("@IsCTCB", strIsCTCB));
                sqlComm.Parameters.Add(new SqlParameter("@IsUpdateByTXT", strIsUpdateByTXT));
                sqlComm.Parameters.Add(new SqlParameter("@Cust_Id", "%" + strcust_id + "%"));
                sqlComm.Parameters.Add(new SqlParameter("@DateTimeToday", strDateTimeToday));
                sqlComm.Parameters.Add(new SqlParameter("@DateTimeYesterday", strDateTimeYesterday));
            }

            Logging.Log(strIsCTCB + "," + strIsUpdateByTXT, LogLayer.Util);
            Logging.Log(strSql, LogLayer.Util);

            if (sqlComm.CommandText.Contains("@strDateTimeToday"))
            {
                sqlComm.Parameters.Add(new SqlParameter("@strDateTimeToday", strDateTimeToday));
            }

            if (sqlComm.CommandText.Contains("@strDateTimeYesterday"))
            {
                sqlComm.Parameters.Add(new SqlParameter("@strDateTimeYesterday", strDateTimeYesterday));
            }
            sqlComm.CommandText += " order by receive_number ";
            return SearchOnDataSet(sqlComm);
        }
        #endregion

        #region NPOI-自扣案件處理狀態查詢報表產出
        /// <summary>
        /// 修改日期: 2020/12/02_Ares_Stanley-變更報表產出方式為NPOI
        /// </summary>
        /// <param name="dsData"></param>
        /// <param name="strAgentName"></param>
        /// <param name="strPathFile"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile(DataSet dsData, string strAgentName, ref string strPathFile, ref string strMsgID)
        {
            try
            {
                //* 檢查目錄，并刪除以前的文檔資料
                BRExcel_File.CheckDirectory(ref strPathFile);

                //* 取要下載的資料
                string strInputDate = "";

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "AutoPayStatus.xls";
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("批次結果報表");

                #region 報表格式
                HSSFCellStyle cs = (HSSFCellStyle)wb.CreateCellStyle();
                // 框線
                cs.BorderBottom = BorderStyle.Thin;
                cs.BorderLeft = BorderStyle.Thin;
                cs.BorderTop = BorderStyle.Thin;
                cs.BorderRight = BorderStyle.Thin;
                // 多行文字
                cs.WrapText = true;
                // Vertical, Horizontal Center
                cs.VerticalAlignment = VerticalAlignment.Center;
                cs.Alignment = HorizontalAlignment.Center;
                // cell format
                cs.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                // Font Size
                HSSFFont font1 = (HSSFFont)wb.CreateFont();
                font1.FontHeightInPoints = 12;
                font1.FontName = "新細明體";
                cs.SetFont(font1);
                #endregion

                // getDate
                string strYYYYMMDD = "000" + CSIPCommonModel.BaseItem.Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
                // 列印經辦
                sheet.GetRow(3).GetCell(1).SetCellValue(strAgentName);
                // 列印日期
                sheet.GetRow(4).GetCell(1).SetCellValue(strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8));
                System.Data.DataTable dtb = dsData.Tables[0];
                for(int idx = 0; idx< dtb.Rows.Count; idx++)
                {
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int col = 0; col<14; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = cs;
                    }
                    // 收件編號
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue(dtb.Rows[idx]["receive_number"].ToString());
                    // 持卡人ID, 顯示前7碼
                    string strCardPersonID = dtb.Rows[idx]["CardPersonID"].ToString();
                    sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(strCardPersonID.Length > 7 ? strCardPersonID.Substring(0, 7).PadRight(strCardPersonID.Length, 'X') : strCardPersonID);
                    // 持卡人姓名, 顯示前2字
                    string strCus_Name = dtb.Rows[idx]["cus_name"].ToString();
                    sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue(strCus_Name.Length > 2 ? strCus_Name.Substring(0, 2).PadRight(strCus_Name.Length, 'X') : strCus_Name);
                    // 帳戶ID, 顯示前7碼
                    string strCus_ID = dtb.Rows[idx]["acc_id"].ToString();
                    sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(strCus_ID.Length > 7 ? strCus_ID.Substring(0, 7).PadRight(strCus_ID.Length, 'X') : strCus_ID);
                    // 扣繳行庫
                    sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(dtb.Rows[idx]["accnoBank"].ToString());
                    // 銀行帳號, 顯示前4, 後4碼
                    string strAcc_No = dtb.Rows[idx]["acc_no"].ToString();
                    strAcc_No = strAcc_No.Trim();
                    strAcc_No = strAcc_No.Substring(strAcc_No.IndexOf("-") == -1 ? 0 : strAcc_No.IndexOf("-") + 1);
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue(strAcc_No.Length > 8 ? strAcc_No.Substring(0, 4).PadRight(strAcc_No.Length - 4, 'X') + strAcc_No.Substring(strAcc_No.Length - 4, 4) : strAcc_No);
                    // 扣繳方式
                    sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellValue(dtb.Rows[idx]["pay_way"].ToString());
                    // 帳單週期
                    sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(dtb.Rows[idx]["bcycle_code"].ToString());
                    // 上送主機/主機Temp檔
                    sheet.GetRow(sheet.LastRowNum).GetCell(8).SetCellValue(dtb.Rows[idx]["isUpdateByTXT"].ToString());
                    // 本行/他行
                    sheet.GetRow(sheet.LastRowNum).GetCell(9).SetCellValue(dtb.Rows[idx]["IsCTCB"].ToString());
                    // GUI鍵檔日期
                    sheet.GetRow(sheet.LastRowNum).GetCell(10).SetCellValue(dtb.Rows[idx]["mod_date"].ToString());
                    // 作業進度
                    sheet.GetRow(sheet.LastRowNum).GetCell(11).SetCellValue(dtb.Rows[idx]["Status"].ToString());
                    // 自扣申請書歸檔日期
                    sheet.GetRow(sheet.LastRowNum).GetCell(12).SetCellValue(dtb.Rows[idx]["DocDate"].ToString());
                    //* 預計ACH報送日期
                    sheet.GetRow(sheet.LastRowNum).GetCell(13).SetCellValue(dtb.Rows[idx]["Ach_Batch_date"].ToString());
                }

                #region 總筆數格式
                HSSFCellStyle totalCS = (HSSFCellStyle)wb.CreateCellStyle();
                // cell format
                cs.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                // Font Size
                HSSFFont totalCS_font = (HSSFFont)wb.CreateFont();
                totalCS_font.FontHeightInPoints = 12;
                totalCS_font.FontName = "新細明體";
                totalCS_font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                totalCS.SetFont(totalCS_font);

                #endregion
                sheet.CreateRow(sheet.LastRowNum + 2).CreateCell(0).CellStyle = totalCS;
                sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("總筆數 : 共 " + dtb.Rows.Count + " 筆");


                //BRAuto_pay_status.WriteDataToSheet_Fault(sheet, strAgentName, dsData.Tables[0], strInputDate);

                //* 保存文件到程序运行目录下
                strPathFile = strPathFile + @"\ExcelFile_AutoPayStatus_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();
                return true;

            }
            catch (Exception ex)
            {
                Logging.Log(ex, LogLayer.BusinessRule);
                throw ex;
            }
        }
        #endregion

        #region NPOI-餘額轉置
        /// <summary>
        /// 修改日期: 2020/12/1_Ares_Stanley-將報表產出方式改為NPOI
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="templateFileName"></param>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <param name="sDay"></param>
        /// <param name="eDay"></param>
        /// <param name="total"></param>
        /// <param name="success"></param>
        /// <param name="fail"></param>
        public void Excel(DataSet ds, string templateFileName, string path, string filename, string sDay, string eDay,
            string total, string success, string fail)
        {
            #region 組合Excel內容
            try
            {
                string strPathFile = path + filename;
                HSSFWorkbook wb = new HSSFWorkbook();
                ISheet sheet = wb.CreateSheet("餘額轉置匯出");
                #region 報表格式
                HSSFCellStyle cs = (HSSFCellStyle)wb.CreateCellStyle();
                // 框線
                cs.BorderBottom = BorderStyle.Thin;
                cs.BorderLeft = BorderStyle.Thin;
                cs.BorderTop = BorderStyle.Thin;
                cs.BorderRight = BorderStyle.Thin;
                // 多行文字
                cs.WrapText = true;
                // Vertical, Horizontal Center
                cs.VerticalAlignment = VerticalAlignment.Center;
                cs.Alignment = HorizontalAlignment.Center;
                // cell format
                cs.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                // Font Size
                HSSFFont font1 = (HSSFFont)wb.CreateFont();
                font1.FontHeightInPoints = 12;
                font1.FontName = "新細明體";
                cs.SetFont(font1);
                #endregion

                #region 判斷是否需要建立查詢報表表頭
                if (!string.IsNullOrEmpty(total))
                {
                    sheet.CreateRow(0);
                    for(int col = 0; col<10; col++)
                    {
                        sheet.GetRow(0).CreateCell(col).CellStyle = cs;
                    }
                    sheet.GetRow(0).GetCell(1).SetCellValue("餘額轉置日期:");
                    sheet.GetRow(0).GetCell(2).SetCellValue(string.Format("{0}~{1}", sDay, eDay));
                    sheet.GetRow(0).GetCell(3).SetCellValue(string.Format("總筆數：{0}", total));
                    sheet.GetRow(0).GetCell(5).SetCellValue(string.Format("成功筆數：{0}", success));
                    sheet.GetRow(0).GetCell(7).SetCellValue(string.Format("失敗筆數：{0}", fail));
                }
                #endregion
                
                
                if (!string.IsNullOrEmpty(total))
                {   // 餘額轉置查詢
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int col =0; col<10; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = cs;
                    }
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("序號");
                    sheet.SetColumnWidth(0, (int)((5.5 + 0.72) * 256));
                    sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue("餘轉日期");
                    sheet.SetColumnWidth(1, (int)((15.5 + 0.72) * 256));
                    sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue("卡號");
                    sheet.SetColumnWidth(2, (int)((21.5 + 0.72) * 256));
                    sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue("PID");
                    sheet.SetColumnWidth(3, (int)((18 + 0.72) * 256));
                    sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue("原因碼");
                    sheet.SetColumnWidth(4, (int)((7.5 + 0.72) * 256));
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue("備註");
                    sheet.SetColumnWidth(5, (int)((13.5 + 0.72) * 256));
                    sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellValue("是否已上傳");
                    sheet.SetColumnWidth(6, (int)((12.5 + 0.72) * 256));
                    sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue("處理狀態");
                    sheet.SetColumnWidth(7, (int)((13.5 + 0.72) * 256));
                    sheet.GetRow(sheet.LastRowNum).GetCell(8).SetCellValue("主機處理註記");
                    sheet.SetColumnWidth(8, (int)((15 + 0.72) * 256));
                    sheet.GetRow(sheet.LastRowNum).GetCell(9).SetCellValue("建檔時間");
                    sheet.SetColumnWidth(9, (int)((16 + 0.72) * 256));
                }
                else
                {   // 餘額轉置新增
                    sheet.CreateRow(sheet.LastRowNum);
                    for (int col = 0; col < 8; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = cs;
                    }
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("序號");
                    sheet.SetColumnWidth(0, (int)((5.5 + 0.72) * 256));
                    sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue("餘轉日期");
                    sheet.SetColumnWidth(1, (int)((15.5 + 0.72) * 256));
                    sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue("卡號");
                    sheet.SetColumnWidth(2, (int)((21.5 + 0.72) * 256));
                    sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue("PID");
                    sheet.SetColumnWidth(3, (int)((18 + 0.72) * 256));
                    sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue("原因碼");
                    sheet.SetColumnWidth(4, (int)((7.5 + 0.72) * 256));
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue("備註");
                    sheet.SetColumnWidth(5, (int)((13.5 + 0.72) * 256));
                    sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellValue("狀態");
                    sheet.SetColumnWidth(6, (int)((8 + 0.72) * 256));
                    sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue("建檔時間");
                    
                    sheet.SetColumnWidth(7, (int)((16 + 0.72) * 256));
                }
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for(int col =0; col<6; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = cs;
                    }
                    // cardNo encryption
                    string cardNo = string.Format("{0}******{1}", dr["CardNo"].ToString().Substring(0, 6),
                        dr["CardNo"].ToString().Substring(dr["CardNo"].ToString().Length - 4, 4));
                    // processFlag switch

                    // Set cell value
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue(dr["RowNumber"].ToString());
                    sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(String.Format("{0}/{1}/{2}", dr["Trans_Date"].ToString().Substring(6, 4), dr["Trans_Date"].ToString().Substring(3, 2), dr["Trans_Date"].ToString().Substring(0, 2)));
                    sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue(cardNo);
                    sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(dr["PID"].ToString());
                    sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(dr["Reason_Code"].ToString());
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue(dr["Memo"].ToString());

                    if (!string.IsNullOrEmpty(total))
                    {
                        for(int col =6; col<10; col++)
                        {
                            sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = cs;
                        }
                        sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellValue(dr["Upload_Flag"].ToString());
                        switch (dr["Process_Flag"].ToString())
                        {
                            case "Y":
                                sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue("成功");
                                break;
                            case "E":
                                sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue("失敗");
                                break;
                            case "D":
                                sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue("刪除");
                                break;
                            case "N":
                                sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue("未處理");
                                break;
                        }
                        sheet.GetRow(sheet.LastRowNum).GetCell(8).SetCellValue(dr["Process_Note"].ToString());
                        sheet.GetRow(sheet.LastRowNum).GetCell(9).SetCellValue(String.Format("{0}/{1}/{2}{3}", dr["Create_DateTime"].ToString().Substring(6, 4), dr["Create_DateTime"].ToString().Substring(3, 2), dr["Create_DateTime"].ToString().Substring(0, 2), dr["Create_DateTime"].ToString().Substring(10, 6)));
                    }
                    else
                    {
                        for(int col=6; col<8; col++)
                        {
                            sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = cs;
                        }
                        switch (dr["Process_Flag"].ToString())
                        {
                            case "Y":
                                sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellValue("成功");
                                break;
                            case "E":
                                sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellValue("失敗");
                                break;
                            case "D":
                                sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellValue("刪除");
                                break;
                            case "N":
                                sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellValue("未處理");
                                break;
                        }
                        sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(String.Format("{0}/{1}/{2}{3}", dr["Create_DateTime"].ToString().Substring(6, 4), dr["Create_DateTime"].ToString().Substring(3, 2), dr["Create_DateTime"].ToString().Substring(0, 2), dr["Create_DateTime"].ToString().Substring(10, 6)));


                    }
                }

                FileStream fs = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs);
                fs.Close();
            }
            catch (Exception ex)
            {
                return;
            }
            finally
            {
                GC.Collect();
            }
            #endregion
        }
        #endregion

        #region jobAutoPay
        public static bool CopyFailureRecsFromOther_Bank_TempToAuto_Pay_Status()
        {
            //string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
            //strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);


            #region 以ACH_hold判斷是否須上傳FTP，不需使用時間判斷了
            //string strYYYYMMDD = "000" + Function.MinGuoDate7length(GetLastSuccessDate("01", "jobAutoPay", "S").AddDays(1).ToString("yyyyMMdd"));
            //strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);

            //string strYYYYMMDD_END = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
            //strYYYYMMDD_END = strYYYYMMDD_END.Substring(strYYYYMMDD_END.Length - 8, 8);

            //Logging.SaveLog(ELogLayer.DB,"Batch_No:" + strYYYYMMDD + "~" + strYYYYMMDD_END, ELogType.Info);
            #endregion 以ACH_hold判斷是否須上傳FTP，不需使用時間判斷了

            //2021/03/17_Ares_Stanley-DB名稱改為變數
            /*string strSql = "insert into auto_pay_status(receive_number, cus_id, cus_name, accnobank, acc_no, pay_way, isupdatebytxt, isCTCB, DateTime)" +
                " select a.Receive_Number,a.Cus_ID,isnull(b.[user_name],''),a.other_bank_code_s, a.Other_Bank_Acc_No, a.Other_Bank_Pay_Way,'Y','N',getDate()" +
                " from Other_Bank_Temp a left join (select distinct user_id,User_name from {0}.dbo.M_USER) as  b on a.[user_id] = b.[user_id]" +
                " where Batch_no>=@Batch_no_start and Batch_no<=@Batch_no_end and" +
                //" (Pcmc_Upload_flag ='1' and not (Pcmc_Return_Code='PAGE 00 OF 03' or Pcmc_Return_Code='PAGE 02 OF 03')) and Pcmc_Return_Code='ERROR:9'";
                " Pcmc_Upload_flag ='1' and Pcmc_Return_Code='ERROR:9';" +
                " update Other_Bank_Temp set UpFile_Time = getdate() where Pcmc_Upload_flag ='1' and Pcmc_Return_Code='ERROR:9' " +
                " and Batch_no>=@Batch_no_start and Batch_no<=@Batch_no_end "; */

            //string strSql = "insert into auto_pay_status(receive_number, cus_id, cus_name, accnobank, acc_no, pay_way, isupdatebytxt, isCTCB, DateTime)" +
            //" select a.Receive_Number,a.Cus_ID,a.Cus_name,a.other_bank_code_s, a.Other_Bank_Acc_No, a.Other_Bank_Pay_Way,'Y','N',getDate()" +
            //" from Other_Bank_Temp a" +
            //" where Batch_no>=@Batch_no_start and Batch_no<=@Batch_no_end and" +
            //" Pcmc_Upload_flag ='1' and Pcmc_Return_Code='ERROR:9' and Keyin_Flag='2';" +
            //" update Other_Bank_Temp set UpFile_Time = getdate() where Pcmc_Upload_flag ='1' and Pcmc_Return_Code='ERROR:9' " +
            //" and Batch_no>=@Batch_no_start and Batch_no<=@Batch_no_end ";
              
             //*以ACH_hold判斷是否須上傳FTP
            string strSql = @" insert into auto_pay_status(receive_number, cus_id, cus_name, accnobank, acc_no, pay_way, isupdatebytxt, isCTCB, DateTime)
                 select a.Receive_Number,a.Cus_ID,isnull(a.Cus_name,''),a.other_bank_code_s, a.Other_Bank_Acc_No, a.Other_Bank_Pay_Way,'Y','N',getDate()
                 from Other_Bank_Temp a
                 where ACH_hold = '1' and Pcmc_Upload_flag ='1' and Pcmc_Return_Code='ERROR:9' and Keyin_Flag='2' and Apply_Type <> 'D';
                 
                 update Other_Bank_Temp set ACH_hold = '2', UpFile_Time = getdate() where Pcmc_Upload_flag ='1' and Pcmc_Return_Code='ERROR:9' 
                 and ACH_hold = '1' ; ";

            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandType = CommandType.Text;
            sqlComm.CommandText = strSql;

            #region 以ACH_hold判斷是否須上傳FTP，不需使用時間判斷了
            //sqlComm.Parameters.Add(new SqlParameter("@Batch_no_start", strYYYYMMDD));
            //sqlComm.Parameters.Add(new SqlParameter("@Batch_no_end", strYYYYMMDD_END));
            #endregion 以ACH_hold判斷是否須上傳FTP，不需使用時間判斷了

            return Update(sqlComm);

        }

        /// <summary>
        /// 將他行週期件資料從 Other_Bank_Temp 複製到 Auto_Pay_Status
        /// </summary>
        /// <returns>true or false</returns>
        public static bool CopyOtherBankTempToAutoPayStatus()
        {
            // 以ACH_hold判斷是否須上傳FTP
            string strSql =
                @"INSERT INTO auto_pay_status(receive_number, cus_id, cus_name, accnobank, acc_no, pay_way, isupdatebytxt, isCTCB, DateTime)
                  SELECT A.Receive_Number, A.Cus_ID, isnull(A.Cus_name, ''), A.other_bank_code_s, A.Other_Bank_Acc_No,
                         A.Other_Bank_Pay_Way, 'Y', 'N', getDate()
                  FROM Other_Bank_Temp A
                  WHERE ACH_hold = '1'
                    AND Pcmc_Upload_flag = '1'
                    AND Pcmc_Return_Code IN ('ERROR:9', '9000', '9001', '9002')
                    AND Keyin_Flag = '2'
                    AND Apply_Type <> 'D';

                  UPDATE Other_Bank_Temp
                  SET ACH_hold    = '2', UpFile_Time = getdate()
                  WHERE Pcmc_Upload_flag = '1'
                    AND ACH_hold = '1'
                    AND Pcmc_Return_Code IN ('ERROR:9', '9000', '9001', '9002');";

            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandType = CommandType.Text;
            sqlComm.CommandText = strSql;

            return Update(sqlComm);
        }

        // 修改紀錄：2022/09/28 調整MATAINDATE為【鍵檔日期】 並整合EDDA核印成功的資料 By Ares jhun
        /// <summary>
        /// 取得核印成功需上傳主機的資料
        /// </summary>
        /// <returns>DataSet</returns>
        public static DataSet GetWithholdingData()
        {
            string strDateTimeEnd = DateTime.Now.ToString("yyyy-MM-dd");
            strDateTimeEnd += " 19:00:00";

            string strDateTimeStart = GetLastSuccessDate("01", "jobAutoPay", "S").ToString("yyyy-MM-dd");
            strDateTimeStart += " 19:00:00";
            
            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandType = CommandType.Text;
            sqlComm.CommandText = SelectWithholdingData;
            sqlComm.Parameters.Add(new SqlParameter("@DateTimeStart", strDateTimeStart));
            sqlComm.Parameters.Add(new SqlParameter("@DateTimeEnd", strDateTimeEnd));

            return SearchOnDataSet(sqlComm);
        }

        // 修改紀錄：2022/09/28 整合EDDA核印成功的資料 By Ares jhun
        /// <summary>
        /// 產生withholding_yyyyMMdd.txt檔案
        /// </summary>
        /// <param name="dt">資料</param>
        /// <param name="fileName">檔案名稱(含副檔名)</param>
        public static void BatchOutput(DataTable dt, string fileName)
        {
            string strTXT = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                // 1码 Char(16) ACC-ID       歸戶 ID(左靠右補空白)
                strTXT += row["Cus_ID"].ToString().Trim().ToUpper().PadRight(16, ' ');

                //17码 Char(1) FUNCTION-CODE
                strTXT += row["Function_Code"].ToString().PadRight(1, ' ');

                //18码 Char(3)  BK-ID        自扣銀行(042:自行,701:郵局,其它:ACH)
                string accNoBank = row["AccNoBank"].ToString().Trim();
                if (accNoBank.Length > 3)
                {
                    accNoBank = accNoBank.Substring(0, 3);
                }
                strTXT += accNoBank.PadRight(3, ' ');

                //21码 Char(26) BK-AC        自扣帳號(左靠右補空白)
                strTXT += row["Acc_No"].ToString().Trim().PadRight(26, ' ');

                //47码 Char(1)  DD-TAPE-IND  自扣註記(0,1)
                strTXT += row["Pay_Way"].ToString().Trim().PadRight(1, ' ');

                //48码 Char(11) DD-ID 自扣者 ID(左靠右補空白)
                strTXT += row["Acc_ID"].ToString().Trim().ToUpper().PadRight(11, ' ');

                //59码 Char(14)  MATAIN-DATE(鍵檔日期)  格式:YYYYMMDD(為當日系統日) + MATAIN-TIME Char(6) 格式：HHMMSS(為當日系統時間)
                strTXT += row["MATAINDATE"].ToString().Trim().PadRight(14, ' ');

                //73码 Char(8)  DD-MEMO   自扣通路來源註記(CSIP)
                strTXT += row["DDMEMO"].ToString().Trim().PadRight(8, ' ');

                //81码 Char(2) SALES-CHANNEL 推廣通路代碼 CSIP為'03'、EDDA為「'04' or '05'」
                string salesChannel = row["SALES_CHANNEL"].ToString().Trim();
                if (salesChannel.Length > 2)
                {
                    salesChannel = salesChannel.Substring(0, 2);
                }
                strTXT += salesChannel.PadRight(2, ' ');

                //83码 Char(5) SALES-UNIT  推廣單位代碼
                strTXT += row["Popul_NO"].ToString().PadRight(5, ' ');

                //88码 Char(8) SALES-EMPO-NO  推廣員員編
                strTXT += row["Popul_EmpNo"].ToString().PadRight(8, ' ');

                //96碼 Char(8) DDST-MATAIN-USER 維護人員
                var userId = row["user_id"].ToString();
                if (userId.Length > 8)
                {
                    strTXT += userId.Substring(userId.Length - 8).PadRight(8, ' ');
                }
                else
                {
                    strTXT += userId;
                }

                //104码 Char(7) FILLER       保留欄
                strTXT += "".PadRight(7, ' ');

                strTXT += "\r\n";

            }
            strTXT += "EOF";
            //strTXT += "\r\n";

            string strPah = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileUpload") + "\\" + fileName;
            if (File.Exists(strPah))
            {
                File.Delete(strPah);
            }

            using (FileStream fs = File.Create(strPah, 1024))
            {
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                sw.WriteLine(strTXT);
                sw.Flush();
                sw.Close();
            }

        }

        /// <summary>
        /// 上傳FTP
        /// </summary>
        /// <param name="fileName">檔案名稱(含副檔名)</param>
        public static void UploadToFtp(string fileName)
        {
            string remoteHost; // FTP IP地址
            string remotePath; // FTP 檔案目錄
            string remoteUser; // FTP User
            string remotePass;
            
            // 取得FTP連線資訊
            DataTable dt = BRM_FileInfo.GetFtpInfoByJobId("jobAutoPay");
            if (dt != null && dt.Rows.Count > 0)
            {
                remoteHost = dt.Rows[0]["FtpIP"].ToString().Trim();
                remotePath = dt.Rows[0]["FtpPath"].ToString().Trim();
                remoteUser = dt.Rows[0]["FtpUserName"].ToString().Trim();
                remotePass = RedirectHelper.GetDecryptString(dt.Rows[0]["FtpPwd"].ToString().Trim());
            }
            else
            {
                Logging.Log("FTP上傳失敗：取得FTP連線資訊發生錯誤", LogState.Error, LogLayer.Util);
                return;
            }

            try
            {
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + remoteHost + remotePath + fileName);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Proxy = null;
                request.Credentials = new NetworkCredential(remoteUser, remotePass);


                // Copy the contents of the file to the request stream.
                string localFilePah = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("FileUpload") + "\\" + fileName;
                if (File.Exists(localFilePah))
                {
                    StreamReader sourceStream = new StreamReader(localFilePah);
                    byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                    sourceStream.Close();
                    request.ContentLength = fileContents.Length;

                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(fileContents, 0, fileContents.Length);
                    requestStream.Close();

                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                    Logging.Log("FTP上傳完成, 狀態：" + response.StatusDescription, LogLayer.Util);

                    response.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.Log("FTP上傳失敗：" + ex, LogState.Error, LogLayer.Util);
            }
        }

        /// <summary>
        /// 取得JOB最後成功日期
        /// </summary>
        /// <param name="strFK">功能標識編號</param>
        /// <param name="strJOBID">JOB編號</param>
        /// <param name="strStatus">執行狀態(S)</param>
        /// <returns>DateTime</returns>
        public static DateTime GetLastSuccessDate(string strFK, string strJOBID, string strStatus)
        {
            //*取得JOB最後成功日期(排除JOB執行當天的成功紀錄)
            string strSql_GetLastSuccessDate = @"SELECT Max(END_TIME) FROM L_BATCH_LOG
                          where FUNCTION_KEY = @FUNCTION_KEY and JOB_ID = @JOB_ID and status = @STATUS 
                                and CONVERT(varchar(12), END_TIME, 112) <> CONVERT(varchar(12), getdate(), 112)";

            SqlCommand sqlComm = new SqlCommand();

            sqlComm.CommandText = strSql_GetLastSuccessDate;

            sqlComm.CommandType = CommandType.Text;

            SqlParameter parmFUNCTION_KEY = new SqlParameter("@FUNCTION_KEY", strFK);
            sqlComm.Parameters.Add(parmFUNCTION_KEY);
            SqlParameter parmJOB_ID = new SqlParameter("@JOB_ID", strJOBID);
            sqlComm.Parameters.Add(parmJOB_ID);
            SqlParameter parmSTATUS = new SqlParameter("@STATUS", strStatus);
            sqlComm.Parameters.Add(parmSTATUS);


            DataSet dstProperty = null;

            dstProperty = BRL_BATCH_LOG.SearchOnDataSet(sqlComm, "Connection_CSIP");

            if (dstProperty == null)
            {
                return DateTime.Now.AddDays(-1);
            }
            if (dstProperty.Tables[0].Rows[0][0] == DBNull.Value)
            {
                return DateTime.Now.AddDays(-1);
            }

            return Convert.ToDateTime(dstProperty.Tables[0].Rows[0][0]);
        }
        #endregion
    }
}
