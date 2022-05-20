//******************************************************************
//*  作    者：蘇洺葳(Grezz)
//*  功能說明：作業量比對報表
//*  創建日期：2018/4/23
//*  修改記錄：此檔案為BusinessRules.BRCompareRpt的新版; 2021/03/17_Ares_Stanley-DB名稱改為變數
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*******************************************************************
using System;
using System.Data;
using System.Data.SqlClient;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Utility;
using Framework.Common.Logging;

namespace CSIPKeyInGUI.BusinessRules_new
{
    /// <summary>
    /// 作業量比對報表業務類
    /// </summary>
    public class BRCompareRpt : CSIPCommonModel.BusinessRules.BRBase<EntityM_PROPERTY_CODE>
    {
        #region SQL 語句
        // 卡人資料異動(他行自扣)
        private const string SEARCH_OTHER_BANK_CARD = @"select B.USER_NAME,A.Cus_ID,A.Other_Bank_Acc_No As Acc_No," +
                        "A.Other_Bank_Pay_Way,A.Other_Bank_Cus_ID,A.bcycle_code,A.Mobile_Phone,A.E_Mail," +
                        "A.E_Bill,A.KeyIn_Flag,A.Upload_flag,A.user_id,A.mod_date,A.Receive_Number,A.Auto_Pay_Setting,A.CellP_Email_Setting,A.E_Bill_Setting,A.OutputByTXT_Setting " +
                    "FROM Other_Bank_Temp A,(select distinct user_id,User_name from {0}.dbo.M_USER) as  B " +
                    "where ( A.KeyIn_Flag='1' or A.KeyIn_Flag='2' ) and A.mod_date between @SearchStart and @SearchEnd " +
                        "and A.user_id = B.USER_ID and A.upload_flag = 'Y' " +
                    "order by Receive_Number,mod_date,cus_id,upload_flag desc," +
                        "keyin_flag,Other_Bank_Acc_No,Other_Bank_Pay_Way," +
                        "Other_Bank_Cus_ID,bcycle_code,mobile_phone,e_mail,e_bill";

        // 卡人資料異動(中信及郵局自扣)
        private const string SEARCH_CHINA_TRUST = @"select B.USER_NAME,A.* " +
                    "FROM AUTO_PAY A,(select distinct user_id,User_name from {0}.dbo.M_USER) as B " +
                    "where A.mod_date  between @SearchStart and @SearchEnd " +
                        "and A.user_id = B.USER_ID and A.upload_flag = 'Y' and Add_Flag='0' " +
                    "order by Receive_Number,mod_date,cus_id,upload_flag desc," +
                        "keyin_flag,acc_no,pay_way,acc_id,bcycle_code,mobile_phone,e_mail,e_bill";

        // 卡人資料異動(訊息/更正單自扣)
        private const string SEARCH_UPDATE_INFO = @"select B.USER_NAME,A.* " +
                    "FROM AUTO_PAY A,(select distinct user_id,User_name from {0}.dbo.M_USER) as B " +
                    "where A.mod_date between @SearchStart and @SearchEnd " +
                        "and A.user_id = B.USER_ID and A.upload_flag = 'Y' and Add_Flag='1' " +
                    "order by Receive_Number,mod_date,cus_id,upload_flag desc, " +
                        "keyin_flag,acc_no,pay_way,acc_id,bcycle_code,mobile_phone,e_mail,e_bill ";

        // 卡人資料異動(毀補)
        private const string SEL_CARD_SURPLY_1 = @"select B.USER_NAME,A.Card_No,A.Eng_Name," +
                        "A.GetCard_code,A.Member_No,A.ReIssue_Type,'','','',A.KeyIn_Flag," +
                        "A.Upload_Flag,A.user_id,A.mod_date " +
                    "FROM ReIssue_Card A,(select distinct user_id,User_name from {0}.dbo.M_USER) as B " +
                    "where A.mod_date between @SearchStart and @SearchEnd " +
                        "and A.ReIssue_Type = '2' " +
                        "and A.user_id = B.USER_ID and A.upload_flag = 'Y' " +
                    "order by A.mod_date,A.Card_No,A.upload_flag desc " +
                    ",A.keyin_flag,A.Eng_Name,A.GetCard_code,A.Member_No ";

        // 卡人資料異動(掛補)
        private const string SEL_CARD_SURPLY_2 = @"select B.USER_NAME,A.Card_No,A.Eng_Name,A.GetCard_code," +
                        "A.Member_No,A.ReIssue_Type,'','','',A.KeyIn_Flag,A.Upload_Flag,A.user_id,A.mod_date " +
                    "FROM ReIssue_Card A,(select distinct user_id,User_name from {0}.dbo.M_USER) as B " +
                    "where A.mod_date between @SearchStart and @SearchEnd " +
                        "and A.ReIssue_Type = '1' " +
                        "and A.user_id = B.USER_ID and A.upload_flag = 'Y' " +
                    "order by A.mod_date,A.Card_No,A.upload_flag desc " +
                    ",A.keyin_flag,A.Eng_Name,A.GetCard_code,A.Member_No";

        // 卡人資料異動(BlockCode新增/異動)
        private const string SEL_BLOCKCODE_ADD = @"select B.USER_NAME,A.* " +
                    "FROM Change_BLK A,(select distinct user_id,User_name from {0}.dbo.M_USER) as B " +
                    "where A.mod_date between @SearchStart and @SearchEnd " +
                        "and A.user_id = B.USER_ID and A.upload_flag = 'Y'  and Change_Type='C' " +
                    "order by mod_date,Card_No,upload_flag desc,keyin_flag,BLOCK_CODE,PURGE_DATE," +
                        "MEMO,REASON_CODE,ACTION_CODE,CWB_REGIONS,Change_Type";

        // 卡人資料異動(BlockCode解除管制)
        private const string SEL_BLOCKCODE_NOT_CONTROL = @"select B.USER_NAME,A.* " +
                    "FROM Change_BLK A,(select distinct user_id,User_name from {0}.dbo.M_USER) as B " +
                    "where A.mod_date between @SearchStart and @SearchEnd " +
                        "and A.user_id = B.USER_ID and A.upload_flag = 'Y'  and Change_Type='D' " +
                    "order by mod_date,Card_No,upload_flag desc,keyin_flag,BLOCK_CODE,PURGE_DATE," +
                        "MEMO,REASON_CODE,ACTION_CODE,CWB_REGIONS,Change_Type";

        // 特店資料異動(資料異動)
        private const string SEL_SHOP_MODIFY = @"select D.USER_NAME,C.shop_id,C.Change_Item," +
                        "'','','','','','',C.KeyIn_Flag,'Y' upload_flag,C.user_id,C.mod_date " +
                    "from shop_Upload C,(select distinct user_id,User_name from {0}.dbo.M_USER) as D " +
                    "where C.mod_date between @SearchStart and @SearchEnd " +
                        "and C.user_id = D.USER_ID  and C.Change_Item = '1' " +
                    "order by mod_date,substring(C.mod_time,1,4),shop_id,upload_flag,KeyIn_Flag ";

        // 特店資料異動(資料異動/請款加批)
        private const string SEL_SHOP_ReqAppro = @"select b.mod_date,b.shop_id, b.before, b.after, b.record_name, b.business_name, b.merchant_name, d.user_name " +
                      "from (select * from shop_reqappro where keyin_flag='2' and newCreate_flag='N') as b, (select distinct user_id,User_name from {0}.dbo.M_USER) as d " +
                      "where b.mod_date between @SearchStart and @SearchEnd and b.user_id=d.user_id " +
                      "order by b.mod_date,substring(b.mod_time,1,4),b.shop_id";

        // 特店資料異動(費率)
        private const string SEL_SHOP_RATE = @"select D.USER_NAME,C.shop_id,C.Change_Item," +
                        "'','','','','','',C.KeyIn_Flag,'Y' upload_flag,C.user_id,C.mod_date " +
                    "from shop_Upload C,(select distinct user_id,User_name from {0}.dbo.M_USER) as D " +
                    "where C.mod_date between @SearchStart and @SearchEnd " +
                        "and C.user_id = D.USER_ID  and C.Change_Item = '2' " +
                    "order by mod_date,substring(C.mod_time,1,4),shop_id,upload_flag,KeyIn_Flag ";

        // 特店資料異動(帳號)
        private const string SEL_SHOP_ACCOUNT = @"select D.USER_NAME,C.shop_id,C.Change_Item," +
                        "'','','','','','',C.KeyIn_Flag,'Y' upload_flag,C.user_id,C.mod_date " +
                    "from shop_Upload C,(select distinct user_id,User_name from {0}.dbo.M_USER) as D " +
                    "where C.mod_date between @SearchStart and @SearchEnd " +
                        "and C.user_id = D.USER_ID  and C.Change_Item = '3' " +
                    "order by mod_date,substring(C.mod_time,1,4),shop_id,upload_flag,KeyIn_Flag ";

        // 特店資料異動(解約) //20211118_Ares_Jack_加入status
        //private const string SEL_SHOP_FIRE = @"select D.USER_NAME,C.shop_id,C.Change_Item, " +
        //                "'','','','','','', C.KeyIn_Flag,'Y' upload_flag,C.user_id,C.mod_date, E.after " +
        //            "from shop_Upload C,(select distinct user_id,User_name from {0}.dbo.M_USER) as D, customer_log E " +
        //            "where C.mod_date between @SearchStart and @SearchEnd " +
        //            "and C.user_id = D.USER_ID  and C.Change_Item = '4' " +
        //            "and ( (E.field_name = '解約代號' or E.field_name = '解約還原') and C.shop_id = E.query_key and C.mod_date = E.mod_date )  " +
        //            "order by mod_date,substring(C.mod_time,1,4),shop_id,upload_flag,KeyIn_Flag ";
        // 特店資料異動(解約)調整 by Ares Stanley 20220314
        private const string SEL_SHOP_FIRE = @"SELECT 
USER_NAME,
 shop_id,
 Change_Item,
 '',
 '',
 '',
 '',
 '',
 '',
 KeyIn_Flag,
upload_flag,
user_id,
mod_date,
after
FROM
(
SELECT 
ROW_NUMBER() OVER(partition by 
 D.USER_NAME,
 C.shop_id,
 C.Change_Item,
 C.KeyIn_Flag,
 e.after
,c.mod_time
ORDER BY C.mod_date,SUBSTRING ( C.mod_time, 1, 4 ),shop_id,KeyIn_Flag ) AS ROWID,
 D.USER_NAME,
 C.shop_id,
 C.Change_Item,
C.mod_date,
C.mod_time,
 C.KeyIn_Flag,
 'Y' upload_flag,
 C.user_id,
 e.after
FROM
 shop_Upload C,
 ( SELECT DISTINCT user_id, User_name FROM {0}.dbo.M_USER ) AS D,
 customer_log E 
WHERE
 C.mod_date between @SearchStart and @SearchEnd
 AND C.user_id = D.USER_ID 
 AND C.Change_Item = '4' 
 AND ( ( E.field_name = '解約代號' OR E.field_name = '解約還原' ) AND C.shop_id = E.query_key AND C.mod_date = E.mod_date ) 
 AND SUBSTRING(c.mod_time, 1, 4)= SUBSTRING(e.mod_time, 1, 4)
)V 
WHERE ROWID = 1
ORDER BY mod_date, after, SUBSTRING ( mod_time, 1, 4 ),shop_id,KeyIn_Flag ";

        // 特店資料異動(機器)
        private const string SEL_SHOP_MACHINE = @"select D.user_name,C.shop_id,C.Change_Item," +
                        "'','','','','','',C.KeyIn_Flag,'Y' upload_flag,C.user_id,C.mod_date " +
                    "from shop_Upload C,(select distinct user_id,User_name from {0}.dbo.M_USER) as D " +
                    "where C.mod_date between @SearchStart and @SearchEnd " +
                        "and C.user_id = D.USER_ID  and C.Change_Item = '5' " +
                    "order by mod_date,substring(C.mod_time,1,4),shop_id,upload_flag,KeyIn_Flag ";

        // 其他查詢資料的SQL
        private const string SEL_COMMON_SQL = @"select B.USER_NAME,A.query_key,A.trans_id," +
                        "substring(A.mod_time,1,4),'','','',A.mod_date " +
                    "from customer_log A,(select distinct user_id,User_name from {1}.dbo.M_USER) as B " +
                    "where A.mod_date between @SearchStart and @SearchEnd " +
                        "and A.user_id = B.USER_ID {0}" +
                    "group by A.query_key,A.trans_id,B.user_name,A.mod_date,substring(A.mod_time,1,4) " +
                    "order by A.mod_date,substring(A.mod_time,1,4),A.query_key";

        // 特店資料新增(6001)
        private const string SEL_SHOP_BASIC = @"select B.USER_NAME,recv_no + '/'+ A.uni_no1+'-'+A.uni_no2 as MerNo,'','','','','','',''," +
                                                "A.KeyIn_Flag,A.sendhost_flag,A.keyin_userid,A.keyin_day " +
                                                "FROM shop_basic A,(select distinct user_id,User_name from {0}.dbo.M_USER) as  B " +
                                                "where A.keyin_day between @SearchStart and @SearchEnd and " +
                                                "A.keyin_userid = B.USER_ID and A.sendhost_flag = 'Y' " +
                                                "and A.member_service <> 'Y' " +
                                                "order by keyin_day,MerNo,keyin_flag ";

        // 特店資料新增(6001會員附加服務)
        private const string SEL_SHOP_BASIC_M = @"select B.USER_NAME,recv_no + '/'+ A.uni_no1+'-'+A.uni_no2 + '-' + A.member_service as MerNo,'','','','','','',''," +
                                                "A.KeyIn_Flag,A.sendhost_flag,A.keyin_userid,A.keyin_day " +
                                                "FROM shop_basic A,(select distinct user_id,User_name from {0}.dbo.M_USER) as  B " +
                                                "where A.keyin_day between @SearchStart and @SearchEnd and " +
                                                "A.keyin_userid = B.USER_ID and A.sendhost_flag = 'Y' " +
                                                "and A.member_service = 'Y' " +
                                                "order by keyin_day,MerNo,keyin_flag ";

        // 特店資料新增(PCAM)
        //20191007 modify by Peggy
        //private const string SEL_SHOP_PCAM = @"select B.USER_NAME,A.merchant_nbr,'','','','','','',''," +
        private const string SEL_SHOP_PCAM = @"select B.USER_NAME,A.merchant_nbr+'  /  '+A.corp as 'merchant_nbr','','','','','','',''," +
                                                "A.KeyIn_Flag,A.send3270,A.keyin_userid,A.keyin_day " +
                                                "FROM shop_pcam A,(select distinct user_id,User_name from {0}.dbo.M_USER) as  B " +
                                                "where A.keyin_day between @SearchStart and @SearchEnd and " +
                                                "A.keyin_userid = B.USER_ID and A.send3270 = 'Y' " +
                                                "order by keyin_day,merchant_nbr,keyin_flag ";

        // 特店資料新增(特店延伸性/請款加批)
        private const string SEL_SHOP_ReqAppro_I = @"select b.mod_date,b.shop_id, b.after, b.record_name, b.business_name, b.merchant_name, d.user_name " +
                      "from (select * from shop_reqappro where keyin_flag='2' and newCreate_flag='Y') as b, (select distinct user_id,User_name from {0}.dbo.M_USER) as d " +
                      "where b.mod_date between @SearchStart and @SearchEnd and b.user_id=d.user_id " +
                      "order by b.mod_date,substring(b.mod_time,1,4),b.shop_id";

        #region 卡人資料異動(新增自扣終止)

        private const string SEARCH_AUTO_PAY_POPUL = @"select B.USER_NAME as user_key ,Convert(varchar(8),A.mod_date,112) AS mod_date, " +
                    " A.receive_number,stuff(A.Cus_Id,8,3,'xxx') as Cus_Id,stuff(acc_no,4,3,'xxx') as acc_no, case when FUNCTION_CODE ='D' then 'Y' else '' end AS auto_pay_setting, " +
                    " case when FUNCTION_CODE ='C' then 'Y' else '' end AS auto_pay_setting1 ,keyin_flag " +
                    " FROM Auto_Pay_Popul A ,(select distinct user_id,User_name from {0}.dbo.M_USER) as  B  " +
                    " where ( A.FUNCTION_CODE='C' or A.FUNCTION_CODE='D') and " +
                    " A.mod_date  between @SearchStart and @SearchEnd " +
                    " and A.user_id = B.USER_ID " +
                    "order by Receive_Number,mod_date,cus_id desc," +
                        "keyin_flag";

        #endregion

        #region 毀補轉一卡通

        private const string SEL_iPASS = @"select case when b.Co_Marketing_Com = 'T' then 'T-悠遊' else 'K-一卡通' end Co_Marketing_Com,
                    b.Card_No,b.Chinese_Name,
                    case when c.before <> '' then 'Y' else 'N' end Change_Eng_Name,
                    b.GetCard_code,b.Member_No,
                    a.Co_Marketing,a.Create_User + '/' + convert(varchar,a.Create_DateTime,112) KeyIn_Flag1,
                    b.Create_User + '/' + convert(varchar,b.Create_DateTime,112) KeyIn_Flag2
                    from dbo.iPASS a
                    join dbo.iPASS b
                    on a.Card_No = b.Card_No
                    left join dbo.customer_log c
                    on c.query_key = b.Card_No
                    where a.KeyIn_Flag = '1' and b.KeyIn_Flag = '2'
                    and c.trans_id in ('01021400','01021500') and c.field_name = '製卡英文姓名' 
                    and replace(convert(varchar,b.Create_DateTime,111),'/','') between @SearchStart and @SearchEnd ";
        #endregion

        #region 公用事業：1641(eTag儲值001001)、1642(臨時停車001002)、1643(月租停車001003)
        //20210607_Ares_Stanley-eTag報表資料改以二KEY為主
        private const string SEL_UTILITIES = @"SELECT b.KEYIN_DATE, b.RECEIVE_NUMBER, b.PRIMARY_CARD_ID AS PRIMARY_CARDHOLDER_ID, '' AS BUSINESS_CATEGORY, 
	CASE WHEN b.ETAG_FLAG = '1' THEN 'V' ELSE '' END AS '1641',
	CASE WHEN b.TEMPLATE_PARK_FLAG = '1' THEN 'V' ELSE '' END AS '1642',
	CASE WHEN b.MONTHLY_PARK_FLAG = '1' THEN 'V' ELSE '' END AS '1643',
	ISNULL(a.AGENT_ID, '') AS AGENT_ID_k1, ISNULL(b.AGENT_ID, '') AS AGENT_ID_k2,
	ISNULL(a.USER_NAME, '') AS USER_NAME_k1, ISNULL(b.USER_NAME, '') AS USER_NAME_k2
FROM 
(
	SELECT e.RECEIVE_NUMBER, e.PRIMARY_CARD_ID, e.NAME, e.ETAG_FLAG, e.TEMPLATE_PARK_FLAG, e.MONTHLY_PARK_FLAG, e.APPLY_TYPE, e.OWNERS_ID, 
		e.MONTHLY_PARKING_NO, e.POPUL_NO, e.POPUL_EMP_NO, e.INTRODUCER_CARD_ID, e.AGENT_ID, e.KEYIN_DATE, e.KEYIN_FLAG, u.USER_NAME
	FROM [dbo].[Utilities_Etag] e WITH(NOLOCK)
	INNER JOIN (SELECT DISTINCT USER_ID, USER_NAME FROM {0}.dbo.M_USER WITH(NOLOCK)) u ON e.AGENT_ID = u.USER_ID
	WHERE e.KEYIN_FLAG = '1'
) a
RIGHT JOIN 
(
	SELECT e.RECEIVE_NUMBER, e.PRIMARY_CARD_ID, e.NAME, e.ETAG_FLAG, e.TEMPLATE_PARK_FLAG, e.MONTHLY_PARK_FLAG, e.APPLY_TYPE, e.OWNERS_ID, 
		e.MONTHLY_PARKING_NO, e.POPUL_NO, e.POPUL_EMP_NO, e.INTRODUCER_CARD_ID, e.AGENT_ID, e.KEYIN_DATE, e.KEYIN_FLAG, u.USER_NAME
	FROM [dbo].[Utilities_Etag] e WITH(NOLOCK)
	INNER JOIN (SELECT DISTINCT USER_ID, USER_NAME FROM {0}.dbo.M_USER WITH(NOLOCK)) u ON e.AGENT_ID = u.USER_ID
	WHERE e.KEYIN_FLAG = '2' AND e.KEYIN_DATE BETWEEN @SearchStart AND @SearchEnd
) b ON a.RECEIVE_NUMBER = b.RECEIVE_NUMBER";

        #endregion

        #region 特店AML資料

//        private const string AML_HeadOffice = @"SELECT a.keyin_day, a.BasicTaxID, a.BasicRegistyNameEN, a.BasicRegistyNameCH, a.KEYIN_FLAG, a.USER_NAME user_1Key, b.USER_NAME user_2Key 
//FROM
//(
//	SELECT e.keyin_day, e.BasicTaxID, e.BasicRegistyNameEN, e.BasicRegistyNameCH, e.KEYIN_FLAG, u.USER_NAME
//	FROM [dbo].[AML_HeadOffice] e WITH(NOLOCK)
//	INNER JOIN (SELECT DISTINCT USER_ID, USER_NAME FROM csip.dbo.M_USER WITH(NOLOCK)) u ON e.keyin_userID = u.USER_ID
//	WHERE e.KEYIN_FLAG = '1' AND e.keyin_day BETWEEN @SearchStart AND @SearchEnd
//)  a
//LEFT JOIN 
//(
//	SELECT e.keyin_day, e.BasicTaxID, e.BasicRegistyNameEN, e.BasicRegistyNameCH, e.KEYIN_FLAG, u.USER_NAME
//	FROM [dbo].[AML_HeadOffice] e WITH(NOLOCK)
//	INNER JOIN (SELECT DISTINCT USER_ID, USER_NAME FROM csip.dbo.M_USER WITH(NOLOCK)) u ON e.keyin_userID = u.USER_ID
//	WHERE e.KEYIN_FLAG = '2' AND e.keyin_day BETWEEN @SearchStart AND @SearchEnd
//) b ON a.BasicTaxID = b.BasicTaxID";

        private const string AML_HeadOffice = @"SELECT  [ID],[BasicTaxID],[keyin_day],[BasicRegistyNameEN],[BasicRegistyNameCH]
        ,[user_1Key],[user_2Key] FROM AML_HeadOffice_Log
        where keyin_day BETWEEN @SearchStart AND @SearchEnd";

        #endregion

        #endregion

        /// <summary>
        /// 取報表數據
        /// 修改紀錄:2021/01/11_Ares_Stanley-變更資料順序
        /// </summary>
        /// <param name="strProperty">業務項目</param>
        /// <param name="strSearchStart">區間起</param>
        /// <param name="strSearchEnd">區間迄</param>
        /// <param name="lngTotal">總量</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>成功時：返回查詢結果；失敗時：null</returns>
        public static DataTable SearchRPTData(string strProperty, string strSearchStart, string strSearchEnd, ref Int64 lngTotal, ref string strMsgID)
        {
            try
            {
                #region 生成要返回的DataTable結構

                DataTable dtblResult = new DataTable();

                if (strProperty == "01030100")
                {
                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));
                    dtblResult.Columns.Add(new DataColumn("mod_date", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("shop_id", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("before", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("after", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("record_name", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("business_name", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("merchant_name", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("user_name", Type.GetType("System.String")));
                }
                else if (strProperty == "01041100")
                {
                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));
                    dtblResult.Columns.Add(new DataColumn("mod_date", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("shop_id", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("after", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("record_name", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("business_name", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("merchant_name", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("user_name", Type.GetType("System.String")));
                }
                else if (strProperty == "01010800" || strProperty == "01010600" || strProperty == "01011000")
                {
                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));
                    dtblResult.Columns.Add(new DataColumn("mod_date", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("receive_number", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("query_key", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("acc_no", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("auto_pay_setting", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("E_Bill_Setting", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("cellP_Email_Setting", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("OutputByTXT_Setting", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("user_1key", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("user_2key", Type.GetType("System.String")));

                    
                }
                else if (strProperty == "01011200")
                {
                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));
                    dtblResult.Columns.Add(new DataColumn("mod_date", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("receive_number", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("query_key", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("acc_no", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("auto_pay_setting", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("auto_pay_setting1", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("user_key", Type.GetType("System.String")));

                }
                else if (strProperty == "01021500")
                {
                    // 毀補轉一卡通
                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));
                    dtblResult.Columns.Add(new DataColumn("Co_Marketing_Com", Type.GetType("System.String")));// 協銷公司
                    dtblResult.Columns.Add(new DataColumn("Card_No", Type.GetType("System.String")));// 卡號
                    dtblResult.Columns.Add(new DataColumn("Chinese_Name", Type.GetType("System.String")));// 製卡中文姓名
                    dtblResult.Columns.Add(new DataColumn("Change_Eng_Name", Type.GetType("System.String")));// 有無異動製卡英文名(Y/N)
                    dtblResult.Columns.Add(new DataColumn("Eng_Name", Type.GetType("System.String")));// 異動後製卡英文名
                    dtblResult.Columns.Add(new DataColumn("GetCard_code", Type.GetType("System.String")));// 取卡方式(0/1)
                    dtblResult.Columns.Add(new DataColumn("Member_No", Type.GetType("System.String")));// 會員編號
                    dtblResult.Columns.Add(new DataColumn("Co_Marketing", Type.GetType("System.String")));// 協銷註記
                    dtblResult.Columns.Add(new DataColumn("KeyIn_Flag1", Type.GetType("System.String")));// 鍵一記録(員編/鍵檔時間)
                    dtblResult.Columns.Add(new DataColumn("KeyIn_Flag2", Type.GetType("System.String")));// 鍵二記録(員編/鍵檔時間)
                }
                else if (strProperty == "001")
                {
                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));// 序號
                    dtblResult.Columns.Add(new DataColumn("mod_date", Type.GetType("System.String")));// 作業日
                    dtblResult.Columns.Add(new DataColumn("receive_number", Type.GetType("System.String")));// 收件編號
                    dtblResult.Columns.Add(new DataColumn("query_key", Type.GetType("System.String")));// 比對值(key值)
                    dtblResult.Columns.Add(new DataColumn("1641", Type.GetType("System.String")));// eTag儲值
                    dtblResult.Columns.Add(new DataColumn("1642", Type.GetType("System.String")));// 臨時停車
                    dtblResult.Columns.Add(new DataColumn("1643", Type.GetType("System.String")));// 月租停車
                    dtblResult.Columns.Add(new DataColumn("user_1Key", Type.GetType("System.String")));// 鍵一同仁
                    dtblResult.Columns.Add(new DataColumn("user_2Key", Type.GetType("System.String")));// 鍵二同仁
                }
                else if (strProperty == "05010001")
                {
                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));// 序號
                    dtblResult.Columns.Add(new DataColumn("mod_date", Type.GetType("System.String")));// 作業日
                    dtblResult.Columns.Add(new DataColumn("query_key", Type.GetType("System.String")));// 統一編號
                    dtblResult.Columns.Add(new DataColumn("user_1Key", Type.GetType("System.String")));// 鍵一同仁
                    dtblResult.Columns.Add(new DataColumn("user_2Key", Type.GetType("System.String")));// 鍵二同仁
                }
                else if (strProperty == "01030203")//20211118_Ares_Jack_特店資料異動(解約)
                {
                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));// 序號
                    dtblResult.Columns.Add(new DataColumn("mod_date", Type.GetType("System.String")));// 作業日
                    dtblResult.Columns.Add(new DataColumn("query_key", Type.GetType("System.String")));// 比對值(key值)
                    dtblResult.Columns.Add(new DataColumn("after", Type.GetType("System.String")));// 比對值(key值)
                    dtblResult.Columns.Add(new DataColumn("user_1Key", Type.GetType("System.String")));// 鍵一同仁
                    dtblResult.Columns.Add(new DataColumn("user_2Key", Type.GetType("System.String")));// 鍵二同仁
                }
                else
                {
                    // 序號
                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));
                    // 作業日
                    dtblResult.Columns.Add(new DataColumn("mod_date", Type.GetType("System.String")));
                    // 比對值(Key值)
                    dtblResult.Columns.Add(new DataColumn("query_key", Type.GetType("System.String")));
                    // 鍵一同仁
                    dtblResult.Columns.Add(new DataColumn("user_1key", Type.GetType("System.String")));
                    // 鍵二同仁
                    dtblResult.Columns.Add(new DataColumn("user_2key", Type.GetType("System.String")));
                }

                #endregion 生成要返回的DataTable結構

                #region 依據Request查詢資料庫，將查詢結果寫入dtblResult

                string strIs2Key = "";
                string strAction = "";
                string strSqlSearch = "";

                #region 業務項目

                switch (strProperty)
                {
                    case "01010100":
                        // 卡人資料異動(地址)
                        strAction = " AND A.trans_id in ('01010100','A01') ";
                        strIs2Key = "N";
                        break;
                    case "01010400":
                        // 卡人資料異動(姓名)
                        strAction = " AND A.trans_id in ('01010400','A04','A03') ";
                        strIs2Key = "N";
                        break;
                    case "01010200":
                        // 卡人資料異動(其他)
                        strAction = " AND A.trans_id in ('01010200','A11') ";
                        strIs2Key = "N";
                        break;
                    case "01010300":
                        // 卡人資料異動(族群碼)
                        strAction = " AND A.trans_id in ('01010300','A06') ";
                        strIs2Key = "N";
                        break;
                    case "01010600":
                        // 卡人資料異動(他行自扣)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEARCH_OTHER_BANK_CARD, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01010800":
                        // 卡人資料異動(中信及郵局自扣)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEARCH_CHINA_TRUST, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01011000":
                        // 卡人資料異動(訊息/更正單自扣)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEARCH_UPDATE_INFO, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    // 卡人資料異動(新增自扣終止）edit by xiongxiaofeng 130524 start
                    case "01011200":
                        // 自扣終止與推廣員異動
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEARCH_AUTO_PAY_POPUL, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    // 卡人資料異動(新增自扣終止）edit by xiongxiaofeng 130524 end
                    case "01020100":
                        // 卡人資料異動(註銷)
                        strAction = " AND A.trans_id in ('01020100','B01') ";
                        strIs2Key = "N";
                        break;
                    case "01020300":
                        // 卡片資料異動(繳款異動)
                        strAction = " AND A.trans_id in ('01020300','B02') ";
                        strIs2Key = "N";
                        break;
                    case "01020400":
                        // 卡片資料異動(繳款評等)
                        strAction = " AND A.trans_id in ('01020400') ";
                        strIs2Key = "N";
                        break;
                    case "01020200":
                        // 卡片資料異動(狀況碼)
                        strAction = " AND A.trans_id in ('01020200','B05') ";
                        strIs2Key = "N";
                        break;
                    case "01020500":
                        // 卡片資料異動(優惠碼)
                        strAction = " AND A.trans_id in ('01020500','B04') ";
                        strIs2Key = "N";
                        break;
                    case "01020700":
                        // 卡片資料異動(毀補)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_CARD_SURPLY_1, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01021300":
                        // 卡片資料異動(掛補)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_CARD_SURPLY_2, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01020900":
                        // 卡片資料異動(BlockCode新增/異動)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_BLOCKCODE_ADD, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01021100":
                        // 卡片資料異動(BlockCode解除管制)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_BLOCKCODE_NOT_CONTROL, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01030200":
                        // 特店資料異動(資料異動)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_MODIFY, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01030100":
                        // 特店資料異動(資料異動/請款加批)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_ReqAppro, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01030201":
                        // 特店資料異動(費率)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_RATE, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01030202":
                        // 特店資料異動(帳號)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_ACCOUNT, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01030203":
                        // 特店資料異動(解約)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_FIRE, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01030204":
                        // 特店資料異動(機器)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_MACHINE, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01040101":
                        // 特店資料新增(6001)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_BASIC, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01040101M":
                        // 特店資料新增(6001會員附加服務)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_BASIC_M, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01040201":
                        // 特店資料新增(PCAM)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_PCAM, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01041100":
                        // 特店資料新增(特店延伸性/請款加批)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_ReqAppro_I, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01021500":
                        // 毀補轉一卡通
                        strIs2Key = "Y";
                        strSqlSearch = SEL_iPASS;
                        break;
                    case "001":
                        // 公用事業(ETAG業務)：001001(ETAG儲值1641)、001002(臨時停車1642)、001003(月租停車1643)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_UTILITIES, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "05010001":
                        // 公用事業(ETAG業務)：001001(ETAG儲值1641)、001002(臨時停車1642)、001003(月租停車1643)
                        strIs2Key = "Y";
                        strSqlSearch = AML_HeadOffice;
                        break;
                    default:
                        break;
                }

                #endregion

                if (strIs2Key != "Y")
                {
                    strSqlSearch = string.Format(SEL_COMMON_SQL, strAction, UtilHelper.GetAppSettings("DB_CSIP"));
                }

                // 聲明SQL Command變量
                SqlCommand sqlcmSearchData = new SqlCommand();
                sqlcmSearchData.CommandType = CommandType.Text;
                sqlcmSearchData.CommandText = strSqlSearch;
                // 區間起
                SqlParameter parmSearchStart = new SqlParameter("@SearchStart", strSearchStart.Replace("/", ""));
                sqlcmSearchData.Parameters.Add(parmSearchStart);
                // 區間迄
                SqlParameter parmSearchEnd = new SqlParameter("@SearchEnd", strSearchEnd.Replace("/", ""));
                sqlcmSearchData.Parameters.Add(parmSearchEnd);

                //20210517_Ares_Stanley-新增記錄SQL到Default LOG
                #region 記錄SQL到Default Log
                //將參數代入記錄用的SQL
                string recordSQL = sqlcmSearchData.CommandText;
                recordSQL = recordSQL.Replace("@SearchStart", string.Format("'{0}'", strSearchStart.Replace("/", ""))).Replace("@SearchEnd", string.Format("'{0}'", strSearchEnd.Replace("/", "")));
                Logging.Log("========== 執行作業量比對表-列印 ==========\r" + recordSQL);
                #endregion 記錄SQL到Default Log

                // 查詢數據
                DataSet dstSearchData = BRCompareRpt.SearchOnDataSet(sqlcmSearchData);
                if (null == dstSearchData)
                {
                    // 查詢數據失敗
                    strMsgID = "01_03010000_004";
                    return null;
                }
                else
                {
                    // 查詢的數據不存在時
                    if (dstSearchData.Tables[0].Rows.Count == 0)
                    {
                        strMsgID = "01_03010000_005";
                        return null;
                    }
                }

                #region 依據查詢的記錄，產出報表資料

                if (strProperty == "01030100")
                {
                    #region 特店資料異動(資料異動/請款加批)

                    int intCount = 0;

                    for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                    {
                        intCount = intCount + 1;
                        // 寫入dtblResult
                        DataRow drowInsert = dtblResult.NewRow();
                        drowInsert["s_no"] = intCount;
                        drowInsert["mod_date"] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                        drowInsert["shop_id"] = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                        drowInsert["before"] = dstSearchData.Tables[0].Rows[intLoop][2].ToString();
                        drowInsert["after"] = dstSearchData.Tables[0].Rows[intLoop][3].ToString();
                        drowInsert["record_name"] = dstSearchData.Tables[0].Rows[intLoop][4].ToString();
                        drowInsert["business_name"] = dstSearchData.Tables[0].Rows[intLoop][5].ToString();
                        drowInsert["merchant_name"] = dstSearchData.Tables[0].Rows[intLoop][6].ToString();
                        drowInsert["user_name"] = dstSearchData.Tables[0].Rows[intLoop][7].ToString();
                        dtblResult.Rows.Add(drowInsert);
                    }

                    lngTotal = intCount;

                    #endregion
                }
                else if (strProperty == "01041100")
                {
                    #region 特店資料新增(特店延伸性/請款加批)

                    int intCount = 0;

                    for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                    {
                        intCount = intCount + 1;
                        // 寫入dtblResult
                        DataRow drowInsert = dtblResult.NewRow();
                        drowInsert["s_no"] = intCount;
                        drowInsert["mod_date"] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                        drowInsert["shop_id"] = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                        drowInsert["after"] = dstSearchData.Tables[0].Rows[intLoop][2].ToString();
                        drowInsert["record_name"] = dstSearchData.Tables[0].Rows[intLoop][3].ToString();
                        drowInsert["business_name"] = dstSearchData.Tables[0].Rows[intLoop][4].ToString();
                        drowInsert["merchant_name"] = dstSearchData.Tables[0].Rows[intLoop][5].ToString();
                        drowInsert["user_name"] = dstSearchData.Tables[0].Rows[intLoop][6].ToString();
                        dtblResult.Rows.Add(drowInsert);
                    }

                    lngTotal = intCount;

                    #endregion
                }
                else if (strProperty == "01011200")
                {
                    #region

                    Int64 lng1Key = 0;
                    Int64 lng2Key = 0;
                    lngTotal = 0;
                    int intCounterI = 0;
                    string[,] arrayUser = new string[100, 2];
                    string strColumnQueryKey = "";
                    string strColumnModDate = "";
                    string strReceive_Number = "";
                    string strAuto_pay_setting = "";
                    string strAuto_pay_setting1 = "";
                    string strUserKey = "";
                    string strAcc_No = "";

                    for (int intIndex = 0; intIndex < 100; intIndex++)
                    {
                        arrayUser[intIndex, 0] = "";
                        arrayUser[intIndex, 1] = "";
                    }

                    // 向dtblResult中寫入資料
                    lngTotal = dstSearchData.Tables[0].Rows.Count;

                    if (lngTotal > 0)
                    {
                        lngTotal = lngTotal / 2;
                    }

                    for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                    {
                        if (strColumnQueryKey == dstSearchData.Tables[0].Rows[intLoop][3].ToString()
                            && strColumnModDate == dstSearchData.Tables[0].Rows[intLoop][1].ToString()                               
                            && strReceive_Number == dstSearchData.Tables[0].Rows[intLoop]["receive_number"].ToString()
                            && strAuto_pay_setting == dstSearchData.Tables[0].Rows[intLoop]["auto_pay_setting"].ToString()
                            && strAuto_pay_setting1 == dstSearchData.Tables[0].Rows[intLoop]["auto_pay_setting1"].ToString()
                            && strUserKey == dstSearchData.Tables[0].Rows[intLoop]["user_key"].ToString()
                            && strAcc_No == dstSearchData.Tables[0].Rows[intLoop]["acc_no"].ToString()
                           )
                        {
                            if (dstSearchData.Tables[0].Rows[intLoop][7].ToString().Trim() == "1")
                            {
                                lng1Key = lng1Key + 1;
                                arrayUser[lng1Key, 0] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                            }
                            else
                            {
                                lng2Key = lng2Key + 1;
                                arrayUser[lng2Key, 1] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                            }
                        }
                        else
                        {
                            if (lng1Key > 0 || lng2Key > 0)
                            {
                                if (lng1Key < lng2Key)
                                {
                                    for (int intIndex = 1; intIndex <= lng2Key; intIndex++)
                                    {
                                        intCounterI = intCounterI + 1;
                                        // 寫入dtblResult
                                        DataRow drowInsert = dtblResult.NewRow();
                                        drowInsert["s_no"] = intCounterI;
                                        drowInsert["query_key"] = strColumnQueryKey;
                                        drowInsert["user_key"] = strUserKey;
                                        drowInsert["mod_date"] = strColumnModDate;
                                        drowInsert["auto_pay_setting"] = strAuto_pay_setting;
                                        drowInsert["auto_pay_setting1"] = strAuto_pay_setting1;
                                        drowInsert["receive_number"] = strReceive_Number;
                                        drowInsert["acc_no"] = strAcc_No;
                                        dtblResult.Rows.Add(drowInsert);
                                    }
                                }
                                else
                                {
                                    for (int intIndex = 1; intIndex <= lng1Key; intIndex++)
                                    {
                                        intCounterI = intCounterI + 1;
                                        // 寫入dtblResult 
                                        DataRow drowInsert = dtblResult.NewRow();
                                        drowInsert["s_no"] = intCounterI;
                                        drowInsert["query_key"] = strColumnQueryKey;
                                        drowInsert["user_key"] = strUserKey;
                                        drowInsert["mod_date"] = strColumnModDate;
                                        drowInsert["auto_pay_setting"] = strAuto_pay_setting;
                                        drowInsert["auto_pay_setting1"] = strAuto_pay_setting1;
                                        drowInsert["receive_number"] = strReceive_Number;
                                        drowInsert["acc_no"] = strAcc_No;
                                        dtblResult.Rows.Add(drowInsert);
                                    }
                                }
                            }

                            strColumnQueryKey = dstSearchData.Tables[0].Rows[intLoop][3].ToString();
                            strColumnModDate = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                            strReceive_Number = dstSearchData.Tables[0].Rows[intLoop]["receive_number"].ToString();
                            strAuto_pay_setting = dstSearchData.Tables[0].Rows[intLoop]["auto_pay_setting"].ToString();
                            strAuto_pay_setting1 = dstSearchData.Tables[0].Rows[intLoop]["auto_pay_setting1"].ToString();
                            strUserKey = dstSearchData.Tables[0].Rows[intLoop]["user_key"].ToString();
                            strAcc_No = dstSearchData.Tables[0].Rows[intLoop]["acc_no"].ToString();

                            lng1Key = 0;
                            lng2Key = 0;

                            // 清空arrayUser
                            for (int intIndex = 0; intIndex < 100; intIndex++)
                            {
                                arrayUser[intIndex, 0] = "";
                                arrayUser[intIndex, 1] = "";
                            }

                            if (dstSearchData.Tables[0].Rows[intLoop][7].ToString().Trim() == "1")
                            {
                                lng1Key = lng1Key + 1;
                                arrayUser[lng1Key, 0] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                            }
                            else
                            {
                                lng2Key = lng2Key + 1;
                                arrayUser[lng2Key, 1] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                            }
                        }
                    }

                    if (lng1Key > 0 || lng2Key > 0)
                    {
                        if (lng1Key < lng2Key)
                        {
                            for (int intIndex = 1; intIndex <= lng2Key; intIndex++)
                            {
                                intCounterI = intCounterI + 1;
                                // 寫入dtblResult
                                DataRow drowInsert = dtblResult.NewRow();
                                drowInsert["s_no"] = intCounterI;
                                drowInsert["query_key"] = strColumnQueryKey;
                                drowInsert["user_key"] = strUserKey;
                                drowInsert["mod_date"] = strColumnModDate;
                                drowInsert["auto_pay_setting"] = strAuto_pay_setting;
                                drowInsert["auto_pay_setting1"] = strAuto_pay_setting1;
                                drowInsert["receive_number"] = strReceive_Number;
                                drowInsert["acc_no"] = strAcc_No;
                                dtblResult.Rows.Add(drowInsert);
                            }
                        }
                        else
                        {
                            for (int intIndex = 1; intIndex <= lng1Key; intIndex++)
                            {
                                intCounterI = intCounterI + 1;
                                // 寫入dtblResult
                                DataRow drowInsert = dtblResult.NewRow();
                                drowInsert["s_no"] = intCounterI;
                                drowInsert["query_key"] = strColumnQueryKey;
                                drowInsert["user_key"] = strUserKey;
                                drowInsert["mod_date"] = strColumnModDate;
                                drowInsert["auto_pay_setting"] = strAuto_pay_setting;
                                drowInsert["auto_pay_setting1"] = strAuto_pay_setting1;
                                drowInsert["receive_number"] = strReceive_Number;
                                drowInsert["acc_no"] = strAcc_No;
                                dtblResult.Rows.Add(drowInsert);
                            }
                        }
                    }

                    #endregion
                }
                else if (strProperty == "01010800" || strProperty == "01010600" || strProperty == "01011000")
                {
                    #region

                    Int64 lng1Key = 0;
                    Int64 lng2Key = 0;
                    lngTotal = 0;
                    int intCounterI = 0;
                    string[,] arrayUser = new string[100, 2];
                    string strColumnQueryKey = "";
                    string strNewColumnQueryKey = "";
                    string strColumnModDate = "";
                    string strColumnUpLoad = "";
                    string strAuto_pay_setting = "";
                    string strCellP_Email_Setting = "";
                    string strE_Bill_Setting = "";
                    string strOutputByTXT_Setting = "";
                    string strReceive_Number = "";
                    string strAcc_No = "";// 顯示在報表中的銀行帳號(敏資處理)
                    string strOldAcc_No = "";// 資料庫中銀行帳號
                    string strNewAcc_No = "";

                    for (int intIndex = 0; intIndex < 100; intIndex++)
                    {
                        arrayUser[intIndex, 0] = "";
                        arrayUser[intIndex, 1] = "";
                    }

                    // 向dtblResult中寫入資料
                    lngTotal = dstSearchData.Tables[0].Rows.Count;

                    if (lngTotal > 0)
                    {
                        lngTotal = lngTotal / 2;
                    }

                    // 循環處理查出的每一筆資料
                    for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                    {
                        if (strColumnQueryKey == dstSearchData.Tables[0].Rows[intLoop][1].ToString()
                                && strColumnModDate == dstSearchData.Tables[0].Rows[intLoop][12].ToString()
                                && strColumnUpLoad == dstSearchData.Tables[0].Rows[intLoop][10].ToString()
                                && strAuto_pay_setting == dstSearchData.Tables[0].Rows[intLoop]["Auto_Pay_Setting"].ToString()
                                && strCellP_Email_Setting == dstSearchData.Tables[0].Rows[intLoop]["CellP_Email_Setting"].ToString()
                                && strE_Bill_Setting == dstSearchData.Tables[0].Rows[intLoop]["E_Bill_Setting"].ToString()
                                && strReceive_Number == dstSearchData.Tables[0].Rows[intLoop]["Receive_Number"].ToString()
                                && strOldAcc_No == dstSearchData.Tables[0].Rows[intLoop]["Acc_No"].ToString()
                                && strOutputByTXT_Setting == dstSearchData.Tables[0].Rows[intLoop]["OutputByTXT_Setting"].ToString())
                        {
                            if (dstSearchData.Tables[0].Rows[intLoop][9].ToString().Trim() == "1")
                            {
                                lng1Key = lng1Key + 1;
                                arrayUser[lng1Key, 0] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                            }
                            else
                            {
                                lng2Key = lng2Key + 1;
                                arrayUser[lng2Key, 1] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                            }
                        }
                        else
                        {
                            if (lng1Key > 0 || lng2Key > 0)
                            {
                                if (lng1Key < lng2Key)
                                {
                                    for (int intIndex = 1; intIndex <= lng2Key; intIndex++)
                                    {
                                        intCounterI = intCounterI + 1;
                                        // 寫入dtblResult
                                        DataRow drowInsert = dtblResult.NewRow();
                                        drowInsert["s_no"] = intCounterI;
                                        drowInsert["query_key"] = strColumnQueryKey;
                                        drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                        drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                        drowInsert["mod_date"] = strColumnModDate;
                                        drowInsert["auto_pay_setting"] = strAuto_pay_setting;
                                        drowInsert["cellp_email_setting"] = strCellP_Email_Setting;
                                        drowInsert["e_bill_setting"] = strE_Bill_Setting;
                                        drowInsert["outputbyTXT_setting"] = strOutputByTXT_Setting;
                                        drowInsert["Receive_Number"] = strReceive_Number;
                                        drowInsert["Acc_No"] = strAcc_No;
                                        dtblResult.Rows.Add(drowInsert);
                                    }
                                }
                                else
                                {
                                    for (int intIndex = 1; intIndex <= lng1Key; intIndex++)
                                    {
                                        intCounterI = intCounterI + 1;
                                        // 寫入dtblResult 
                                        DataRow drowInsert = dtblResult.NewRow();
                                        drowInsert["s_no"] = intCounterI;
                                        drowInsert["query_key"] = strColumnQueryKey;
                                        drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                        drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                        drowInsert["mod_date"] = strColumnModDate;
                                        drowInsert["auto_pay_setting"] = strAuto_pay_setting;
                                        drowInsert["cellp_email_setting"] = strCellP_Email_Setting;
                                        drowInsert["e_bill_setting"] = strE_Bill_Setting;
                                        drowInsert["outputbyTXT_setting"] = strOutputByTXT_Setting;
                                        drowInsert["Receive_Number"] = strReceive_Number;
                                        drowInsert["Acc_No"] = strAcc_No;
                                        dtblResult.Rows.Add(drowInsert);
                                    }
                                }
                            }

                            strNewColumnQueryKey = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                            strColumnQueryKey = strNewColumnQueryKey.Substring(0, 5).PadRight(strNewColumnQueryKey.Length, 'X');

                            strColumnModDate = dstSearchData.Tables[0].Rows[intLoop][12].ToString();
                            strColumnUpLoad = dstSearchData.Tables[0].Rows[intLoop][10].ToString();
                            strAuto_pay_setting = dstSearchData.Tables[0].Rows[intLoop]["Auto_Pay_Setting"].ToString();
                            strCellP_Email_Setting = dstSearchData.Tables[0].Rows[intLoop]["CellP_Email_Setting"].ToString();
                            strE_Bill_Setting = dstSearchData.Tables[0].Rows[intLoop]["E_Bill_Setting"].ToString();
                            strOutputByTXT_Setting = dstSearchData.Tables[0].Rows[intLoop]["OutputByTXT_Setting"].ToString();
                            strReceive_Number = dstSearchData.Tables[0].Rows[intLoop]["Receive_Number"].ToString();
                            strOldAcc_No = dstSearchData.Tables[0].Rows[intLoop]["Acc_No"].ToString();
                            strNewAcc_No = strOldAcc_No.Substring(strOldAcc_No.IndexOf("-") == -1 ? 0 : strOldAcc_No.IndexOf("-") + 1);
                            strAcc_No = strNewAcc_No.Length > 8 ? strNewAcc_No.Substring(0, 4).PadRight(strNewAcc_No.Length - 4, 'X') + strNewAcc_No.Substring(strNewAcc_No.Length - 4, 4) : strNewAcc_No;
                            lng1Key = 0;
                            lng2Key = 0;

                            // 清空arrayUser；
                            for (int intIndex = 0; intIndex < 100; intIndex++)
                            {
                                arrayUser[intIndex, 0] = "";
                                arrayUser[intIndex, 1] = "";
                            }

                            if (dstSearchData.Tables[0].Rows[intLoop][9].ToString().Trim() == "1")
                            {
                                lng1Key = lng1Key + 1;
                                arrayUser[lng1Key, 0] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                            }
                            else
                            {
                                lng2Key = lng2Key + 1;
                                arrayUser[lng2Key, 1] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                            }
                        }
                    }

                    if (lng1Key > 0 || lng2Key > 0)
                    {
                        if (lng1Key < lng2Key)
                        {
                            for (int intIndex = 1; intIndex <= lng2Key; intIndex++)
                            {
                                intCounterI = intCounterI + 1;
                                // 寫入dtblResult
                                DataRow drowInsert = dtblResult.NewRow();
                                drowInsert["s_no"] = intCounterI;
                                drowInsert["query_key"] = strColumnQueryKey;
                                drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                drowInsert["mod_date"] = strColumnModDate;
                                drowInsert["auto_pay_setting"] = strAuto_pay_setting;
                                drowInsert["cellp_email_setting"] = strCellP_Email_Setting;
                                drowInsert["e_bill_setting"] = strE_Bill_Setting;
                                drowInsert["outputbyTXT_setting"] = strOutputByTXT_Setting;
                                drowInsert["Receive_Number"] = strReceive_Number;
                                drowInsert["Acc_No"] = strAcc_No;
                                dtblResult.Rows.Add(drowInsert);
                            }
                        }
                        else
                        {
                            for (int intIndex = 1; intIndex <= lng1Key; intIndex++)
                            {
                                intCounterI = intCounterI + 1;
                                // 寫入dtblResult
                                DataRow drowInsert = dtblResult.NewRow();
                                drowInsert["s_no"] = intCounterI;
                                drowInsert["query_key"] = strColumnQueryKey;
                                drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                drowInsert["mod_date"] = strColumnModDate;
                                drowInsert["auto_pay_setting"] = strAuto_pay_setting;
                                drowInsert["cellp_email_setting"] = strCellP_Email_Setting;
                                drowInsert["e_bill_setting"] = strE_Bill_Setting;
                                drowInsert["outputbyTXT_setting"] = strOutputByTXT_Setting;
                                drowInsert["Receive_Number"] = strReceive_Number;
                                drowInsert["Acc_No"] = strAcc_No;
                                dtblResult.Rows.Add(drowInsert);
                            }
                        }
                    }

                    #endregion
                }
                else if (strProperty == "01021500")
                {
                    #region 毀補轉一卡通

                    int intCount = 0;
                    string str_temp = "";

                    for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                    {
                        intCount = intCount + 1;
                        DataRow drowInsert = dtblResult.NewRow();

                        drowInsert["s_no"] = intCount;
                        drowInsert["Co_Marketing_Com"] = dstSearchData.Tables[0].Rows[intLoop][0] != null ?
                            dstSearchData.Tables[0].Rows[intLoop][0].ToString() : "";

                        if (dstSearchData.Tables[0].Rows[intLoop][1] != null &&
                            !string.IsNullOrEmpty(dstSearchData.Tables[0].Rows[intLoop][1].ToString()))
                        {
                            str_temp = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                            if (str_temp.Length == 16)
                            {
                                str_temp = string.Format("{0}-****-{1}-{2}", str_temp.Substring(0, 4),
                                    str_temp.Substring(8, 4), str_temp.Substring(12, 4));
                                drowInsert["Card_No"] = str_temp;
                            }
                            else
                                drowInsert["Card_No"] = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                        }
                        else
                            drowInsert["Card_No"] = "";

                        if (dstSearchData.Tables[0].Rows[intLoop][2] != null &&
                            !string.IsNullOrEmpty(dstSearchData.Tables[0].Rows[intLoop][2].ToString()))
                        {
                            str_temp = dstSearchData.Tables[0].Rows[intLoop][2].ToString();
                            str_temp = string.Format("{0}*{1}", str_temp.Substring(0, 1), str_temp.Substring(str_temp.Length - 1, 1));
                            drowInsert["Chinese_Name"] = str_temp;
                        }
                        else
                            drowInsert["Chinese_Name"] = "";

                        drowInsert["Change_Eng_Name"] = dstSearchData.Tables[0].Rows[intLoop][3] != null ?
                            dstSearchData.Tables[0].Rows[intLoop][3].ToString() : "";
                        drowInsert["GetCard_code"] = dstSearchData.Tables[0].Rows[intLoop][4] != null ?
                            dstSearchData.Tables[0].Rows[intLoop][4].ToString() : "";
                        drowInsert["Member_No"] = dstSearchData.Tables[0].Rows[intLoop][5] != null ?
                            dstSearchData.Tables[0].Rows[intLoop][5].ToString() : "";
                        drowInsert["Co_Marketing"] = dstSearchData.Tables[0].Rows[intLoop][6] != null ?
                            dstSearchData.Tables[0].Rows[intLoop][6].ToString() : "";
                        drowInsert["KeyIn_Flag1"] = dstSearchData.Tables[0].Rows[intLoop][7] != null ?
                            dstSearchData.Tables[0].Rows[intLoop][7].ToString() : "";
                        drowInsert["KeyIn_Flag2"] = dstSearchData.Tables[0].Rows[intLoop][8] != null ?
                            dstSearchData.Tables[0].Rows[intLoop][8].ToString() : "";
                        dtblResult.Rows.Add(drowInsert);
                    }

                    lngTotal = intCount;

                    #endregion
                }
                else if (strProperty == "05010001")
                {
                    #region 特店AML資料

                    int intCount = 0;
                    DataRow _dr;
                    for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                    {
                        intCount = intCount + 1;
                        DataRow drowInsert = dtblResult.NewRow();
                        _dr = dstSearchData.Tables[0].Rows[intLoop];

                        drowInsert["s_no"] = intCount;
                        //drowInsert["mod_date"] = dstSearchData.Tables[0].Rows[intLoop]["keyin_day"].ToString();
                        ////drowInsert["query_key"] = dstSearchData.Tables[0].Rows[intLoop]["BasicTaxID"].ToString() + "".PadRight(5) + dstSearchData.Tables[0].Rows[intLoop]["BasicRegistyNameEN"].ToString();
                        //drowInsert["query_key"] = dstSearchData.Tables[0].Rows[intLoop]["BasicTaxID"].ToString() + "".PadRight(5) + dstSearchData.Tables[0].Rows[intLoop]["BasicRegistyNameCH"].ToString();
                        //drowInsert["user_1Key"] = dstSearchData.Tables[0].Rows[intLoop]["user_1Key"].ToString();
                        //drowInsert["user_2Key"] = dstSearchData.Tables[0].Rows[intLoop]["user_2Key"].ToString();

                        drowInsert["mod_date"] = _dr["keyin_day"].ToString();
                        drowInsert["query_key"] = string.Format("{0}{1}{2}", _dr["BasicTaxID"].ToString(), "".PadRight(5), _dr["BasicRegistyNameCH"].ToString());
                        drowInsert["user_1Key"] = _dr["user_1Key"].ToString();
                        drowInsert["user_2Key"] = _dr["user_2Key"].ToString();

                        dtblResult.Rows.Add(drowInsert);
                    }

                    lngTotal = intCount;

                    #endregion
                }
                else if (strProperty == "01030203")
                {
                    //20211122_Ares_Jack_特店資料異動(解約)
                    //調整特店資料異動(解約)鍵二同仁未顯示問題 by Ares Stanley 20211125
                    #region 特店資料異動(解約)
                    Int64 lng1Key = 0;
                    Int64 lng2Key = 0;
                    int intCount = 0;
                    lngTotal = 0;
                    int intCounterI = 0;
                    string[,] arrayUser = new string[dstSearchData.Tables[0].Rows.Count, 2];
                    string strColumnQueryKey = "";
                    string strColumnModDate = "";
                    string strColumnUpLoad = "";
                    string strColumnAfter = "";

                    for (int intIndex = 0; intIndex < dstSearchData.Tables[0].Rows.Count; intIndex++)
                    {
                        arrayUser[intIndex, 0] = "";
                        arrayUser[intIndex, 1] = "";
                    }

                    // 向dtblResult中寫入資料
                    if (strIs2Key != "Y")
                    {
                        // 循環處理查出的每一筆資料
                        for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                        {
                            intCount = intCount + 1;
                            // 寫入dtblResult
                            DataRow drowInsert = dtblResult.NewRow();
                            drowInsert["s_no"] = intCount;
                            drowInsert["query_key"] = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                            drowInsert["user_1Key"] = "";
                            drowInsert["user_2Key"] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                            drowInsert["mod_date"] = dstSearchData.Tables[0].Rows[intLoop][7].ToString();
                            dtblResult.Rows.Add(drowInsert);
                        }

                        lngTotal = intCount;
                    }
                    else
                    {
                        lngTotal = dstSearchData.Tables[0].Rows.Count;

                        if (lngTotal > 0)
                        {
                            lngTotal = lngTotal / 2;
                        }

                        // 循環處理查出的每一筆資料
                        for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                        {

                            if (strColumnQueryKey == dstSearchData.Tables[0].Rows[intLoop][1].ToString()
                                    && strColumnModDate == dstSearchData.Tables[0].Rows[intLoop][12].ToString()
                                    && strColumnUpLoad == dstSearchData.Tables[0].Rows[intLoop][10].ToString()
                                    && !(lng1Key > 0 && lng2Key > 0 && lng1Key == lng2Key))
                            {
                                if (dstSearchData.Tables[0].Rows[intLoop][9].ToString().Trim() == "1")
                                {
                                    lng1Key = lng1Key + 1;
                                    arrayUser[lng1Key, 0] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                                }
                                else
                                {
                                    lng2Key = lng2Key + 1;
                                    arrayUser[lng2Key, 1] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                                }
                            }
                            else
                            {
                                if (lng1Key > 0 || lng2Key > 0)
                                {
                                    if (lng1Key < lng2Key)
                                    {
                                        for (int intIndex = 1; intIndex <= lng2Key; intIndex++)
                                        {
                                            intCounterI = intCounterI + 1;
                                            // 寫入dtblResult
                                            DataRow drowInsert = dtblResult.NewRow();
                                            drowInsert["s_no"] = intCounterI;
                                            drowInsert["query_key"] = strColumnQueryKey;
                                            drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                            drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                            drowInsert["mod_date"] = strColumnModDate;
                                            drowInsert["after"] = strColumnAfter;
                                            dtblResult.Rows.Add(drowInsert);
                                        }
                                    }
                                    else
                                    {
                                        for (int intIndex = 1; intIndex <= lng1Key; intIndex++)
                                        {
                                            intCounterI = intCounterI + 1;
                                            // 寫入dtblResult 
                                            DataRow drowInsert = dtblResult.NewRow();
                                            drowInsert["s_no"] = intCounterI;
                                            drowInsert["query_key"] = strColumnQueryKey;
                                            drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                            drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                            drowInsert["mod_date"] = strColumnModDate;
                                            drowInsert["after"] = strColumnAfter;
                                            dtblResult.Rows.Add(drowInsert);
                                        }
                                    }
                                }

                                strColumnQueryKey = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                                strColumnModDate = dstSearchData.Tables[0].Rows[intLoop][12].ToString();
                                strColumnUpLoad = dstSearchData.Tables[0].Rows[intLoop][10].ToString();
                                strColumnAfter = dstSearchData.Tables[0].Rows[intLoop]["after"].ToString();
                                lng1Key = 0;
                                lng2Key = 0;

                                //* 清空arrayUser；
                                for (int intIndex = 0; intIndex < dstSearchData.Tables[0].Rows.Count; intIndex++)
                                {
                                    arrayUser[intIndex, 0] = "";
                                    arrayUser[intIndex, 1] = "";
                                }

                                if (dstSearchData.Tables[0].Rows[intLoop][9].ToString().Trim() == "1")
                                {
                                    lng1Key = lng1Key + 1;
                                    arrayUser[lng1Key, 0] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                                }
                                else
                                {
                                    lng2Key = lng2Key + 1;
                                    arrayUser[lng2Key, 1] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                                }
                            }
                        }

                        if (lng1Key > 0 || lng2Key > 0)
                        {
                            if (lng1Key < lng2Key)
                            {
                                for (int intIndex = 1; intIndex <= lng2Key; intIndex++)
                                {
                                    intCounterI = intCounterI + 1;
                                    // 寫入dtblResult
                                    DataRow drowInsert = dtblResult.NewRow();
                                    drowInsert["s_no"] = intCounterI;
                                    drowInsert["query_key"] = strColumnQueryKey;
                                    drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                    drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                    drowInsert["mod_date"] = strColumnModDate;
                                    drowInsert["after"] = strColumnAfter;
                                    dtblResult.Rows.Add(drowInsert);
                                }
                            }
                            else
                            {
                                for (int intIndex = 1; intIndex <= lng1Key; intIndex++)
                                {
                                    intCounterI = intCounterI + 1;
                                    // 寫入dtblResult
                                    DataRow drowInsert = dtblResult.NewRow();
                                    drowInsert["s_no"] = intCounterI;
                                    drowInsert["query_key"] = strColumnQueryKey;
                                    drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                    drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                    drowInsert["mod_date"] = strColumnModDate;
                                    drowInsert["after"] = strColumnAfter;
                                    dtblResult.Rows.Add(drowInsert);
                                }
                            }
                        }
                    }
                    #endregion

                }
                else
                {
                    #region else

                    Int64 lng1Key = 0;
                    Int64 lng2Key = 0;
                    int intCount = 0;
                    lngTotal = 0;
                    int intCounterI = 0;
                    string[,] arrayUser = new string[100, 2];
                    string strColumnQueryKey = "";
                    string strColumnModDate = "";
                    string strColumnUpLoad = "";

                    for (int intIndex = 0; intIndex < 100; intIndex++)
                    {
                        arrayUser[intIndex, 0] = "";
                        arrayUser[intIndex, 1] = "";
                    }

                    // 向dtblResult中寫入資料
                    if (strIs2Key != "Y")
                    {
                        // 循環處理查出的每一筆資料
                        for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                        {
                            intCount = intCount + 1;
                            // 寫入dtblResult
                            DataRow drowInsert = dtblResult.NewRow();
                            drowInsert["s_no"] = intCount;
                            drowInsert["query_key"] = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                            drowInsert["user_1Key"] = "";
                            drowInsert["user_2Key"] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                            drowInsert["mod_date"] = dstSearchData.Tables[0].Rows[intLoop][7].ToString();
                            dtblResult.Rows.Add(drowInsert);
                        }

                        lngTotal = intCount;
                    }
                    else
                    {
                        if (strProperty == "001")
                        {
                            // 公用事業(ETAG業務)
                            string space = " ";
                            DataTable dtUtilities = dstSearchData.Tables[0];

                            for (int i = 0; dtUtilities.Rows.Count != 0; i++)
                            {
                                string str1641 = "";
                                string str1642 = "";
                                string str1643 = "";
                                string strCondition = "receive_number = '" + dtUtilities.Rows[0]["receive_number"] +
                                                        "' and primary_cardholder_id = '" + dtUtilities.Rows[0]["primary_cardholder_id"] + "'";
                                DataRow[] dataRows = dtUtilities.Select(strCondition);

                                foreach (DataRow dataRow in dataRows)
                                {
                                    //if (dataRow["business_category"].ToString() == "001001")
                                    //    str1641 = dataRow["1641"].ToString();
                                    //if (dataRow["business_category"].ToString() == "001002")
                                    //    str1642 = dataRow["1642"].ToString();
                                    //if (dataRow["business_category"].ToString() == "001003")
                                    //    str1643 = dataRow["1643"].ToString();
                                    if (dataRow["1641"].ToString() == "V")
                                        str1641 = dataRow["1641"].ToString();
                                    if (dataRow["1642"].ToString() == "V")
                                        str1642 = dataRow["1642"].ToString();
                                    if (dataRow["1643"].ToString() == "V")
                                        str1643 = dataRow["1643"].ToString();
                                }

                                // 寫入dtblResult
                                DataRow dtRow = dtblResult.NewRow();
                                intCounterI = intCounterI + 1;
                                dtRow["s_no"] = intCounterI;
                                dtRow["mod_date"] = dtUtilities.Rows[0]["keyin_date"];
                                // 收件編號、比對值(key值)、eTag儲值、臨時停車、月租停車組成一個欄位
                                //dtRow["query_key"] = dtUtilities.Rows[0]["receive_number"]
                                //                    + space.PadRight(5) + dtUtilities.Rows[0]["primary_cardholder_id"]
                                //                    + space.PadRight(12) + str1641
                                //                    + space.PadRight(17) + str1642
                                //                    + space.PadRight(17) + str1643;
                                dtRow["receive_number"] = dtUtilities.Rows[0]["receive_number"];
                                dtRow["query_key"] = dtUtilities.Rows[0]["primary_cardholder_id"];
                                dtRow["1641"] = dtUtilities.Rows[0]["1641"];// eTag儲值
                                dtRow["1642"] = dtUtilities.Rows[0]["1642"];// 臨時停車
                                dtRow["1643"] = dtUtilities.Rows[0]["1643"];// 月租停車
                                dtRow["user_1Key"] = dtUtilities.Rows[0]["user_name_k1"];// 鍵一同仁
                                dtRow["user_2Key"] = dtUtilities.Rows[0]["user_name_k2"];// 鍵二同仁
                                dtblResult.Rows.Add(dtRow);

                                // 移除已加入的Row
                                dtUtilities.Rows.Remove(dtUtilities.Rows[0]);
                                //foreach (DataRow row in dataRows)
                                //{
                                //    dtUtilities.Rows.Remove(row);
                                //}
                                lngTotal = intCounterI; //2021/03/08_Ares_Stanley-ETAGE業務總量
                            }
                        }
                        else
                        {
                            lngTotal = dstSearchData.Tables[0].Rows.Count;

                            if (lngTotal > 0)
                            {
                                lngTotal = lngTotal / 2;
                            }

                            // 循環處理查出的每一筆資料
                            for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                            {

                                if (strColumnQueryKey == dstSearchData.Tables[0].Rows[intLoop][1].ToString()
                                        && strColumnModDate == dstSearchData.Tables[0].Rows[intLoop][12].ToString()
                                        && strColumnUpLoad == dstSearchData.Tables[0].Rows[intLoop][10].ToString())
                                {
                                    if (dstSearchData.Tables[0].Rows[intLoop][9].ToString().Trim() == "1")
                                    {
                                        lng1Key = lng1Key + 1;
                                        arrayUser[lng1Key, 0] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                                    }
                                    else
                                    {
                                        lng2Key = lng2Key + 1;
                                        arrayUser[lng2Key, 1] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                                    }
                                }
                                else
                                {
                                    if (lng1Key > 0 || lng2Key > 0)
                                    {
                                        if (lng1Key < lng2Key)
                                        {
                                            for (int intIndex = 1; intIndex <= lng2Key; intIndex++)
                                            {
                                                intCounterI = intCounterI + 1;
                                                // 寫入dtblResult
                                                DataRow drowInsert = dtblResult.NewRow();
                                                drowInsert["s_no"] = intCounterI;
                                                drowInsert["query_key"] = strColumnQueryKey;
                                                drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                                drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                                drowInsert["mod_date"] = strColumnModDate;
                                                dtblResult.Rows.Add(drowInsert);
                                            }
                                        }
                                        else
                                        {
                                            for (int intIndex = 1; intIndex <= lng1Key; intIndex++)
                                            {
                                                intCounterI = intCounterI + 1;
                                                // 寫入dtblResult 
                                                DataRow drowInsert = dtblResult.NewRow();
                                                drowInsert["s_no"] = intCounterI;
                                                drowInsert["query_key"] = strColumnQueryKey;
                                                drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                                drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                                drowInsert["mod_date"] = strColumnModDate;
                                                dtblResult.Rows.Add(drowInsert);
                                            }
                                        }
                                    }

                                    strColumnQueryKey = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                                    strColumnModDate = dstSearchData.Tables[0].Rows[intLoop][12].ToString();
                                    strColumnUpLoad = dstSearchData.Tables[0].Rows[intLoop][10].ToString();
                                    lng1Key = 0;
                                    lng2Key = 0;

                                    //* 清空arrayUser；
                                    for (int intIndex = 0; intIndex < 100; intIndex++)
                                    {
                                        arrayUser[intIndex, 0] = "";
                                        arrayUser[intIndex, 1] = "";
                                    }

                                    if (dstSearchData.Tables[0].Rows[intLoop][9].ToString().Trim() == "1")
                                    {
                                        lng1Key = lng1Key + 1;
                                        arrayUser[lng1Key, 0] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                                    }
                                    else
                                    {
                                        lng2Key = lng2Key + 1;
                                        arrayUser[lng2Key, 1] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                                    }
                                }
                            }

                            if (lng1Key > 0 || lng2Key > 0)
                            {
                                if (lng1Key < lng2Key)
                                {
                                    for (int intIndex = 1; intIndex <= lng2Key; intIndex++)
                                    {
                                        intCounterI = intCounterI + 1;
                                        // 寫入dtblResult
                                        DataRow drowInsert = dtblResult.NewRow();
                                        drowInsert["s_no"] = intCounterI;
                                        drowInsert["query_key"] = strColumnQueryKey;
                                        drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                        drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                        drowInsert["mod_date"] = strColumnModDate;
                                        dtblResult.Rows.Add(drowInsert);
                                    }
                                }
                                else
                                {
                                    for (int intIndex = 1; intIndex <= lng1Key; intIndex++)
                                    {
                                        intCounterI = intCounterI + 1;
                                        // 寫入dtblResult
                                        DataRow drowInsert = dtblResult.NewRow();
                                        drowInsert["s_no"] = intCounterI;
                                        drowInsert["query_key"] = strColumnQueryKey;
                                        drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                        drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                        drowInsert["mod_date"] = strColumnModDate;
                                        dtblResult.Rows.Add(drowInsert);
                                    }
                                }
                            }
                        }
                    }

                    #endregion
                }

                #endregion 依據查詢的記錄，產出報表資料

                return dtblResult;

                #endregion 依據Request查詢資料庫，將查詢結果寫入dtblResult
            }
            catch (Exception exp)
            {
                BRCompareRpt.SaveLog(exp);
                strMsgID = "01_03010000_004";
                return null;
            }
        }

        /// <summary>
        /// 建立日期:2021/01/19_Ares_Stanley
        /// 資料查詢 for GridView
        /// 修改日期:
        /// </summary>
        /// <param name="strProperty"></param>
        /// <param name="strSearchStart"></param>
        /// <param name="strSearchEnd"></param>
        /// <param name="lngTotal"></param>
        /// <param name="strMsgID"></param>
        /// <param name="iPageIndex"></param>
        /// <param name="iPageSize"></param>
        /// <param name="iTotalCount"></param>
        /// <returns></returns>
        public static DataTable SearchRPTData_Search(string strProperty, string strSearchStart, string strSearchEnd, ref Int64 lngTotal, ref string strMsgID, int iPageIndex, int iPageSize, ref int iTotalCount)
        {
            try
            {
                #region 生成要返回的DataTable結構

                DataTable dtblResult = new DataTable();

                if (strProperty == "01030100")
                {
                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));
                    dtblResult.Columns.Add(new DataColumn("mod_date", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("shop_id", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("before", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("after", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("record_name", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("business_name", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("merchant_name", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("user_name", Type.GetType("System.String")));
                }
                else if (strProperty == "01041100")
                {
                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));
                    dtblResult.Columns.Add(new DataColumn("mod_date", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("shop_id", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("after", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("record_name", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("business_name", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("merchant_name", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("user_name", Type.GetType("System.String")));
                }
                else if (strProperty == "01010800" || strProperty == "01010600" || strProperty == "01011000")
                {
                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));
                    dtblResult.Columns.Add(new DataColumn("mod_date", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("receive_number", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("query_key", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("acc_no", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("auto_pay_setting", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("E_Bill_Setting", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("cellP_Email_Setting", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("OutputByTXT_Setting", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("user_1key", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("user_2key", Type.GetType("System.String")));


                }
                else if (strProperty == "01011200")
                {
                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));
                    dtblResult.Columns.Add(new DataColumn("mod_date", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("receive_number", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("query_key", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("acc_no", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("auto_pay_setting", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("auto_pay_setting1", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("user_key", Type.GetType("System.String")));

                }
                else if (strProperty == "01021500")
                {
                    // 毀補轉一卡通
                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));
                    dtblResult.Columns.Add(new DataColumn("Co_Marketing_Com", Type.GetType("System.String")));// 協銷公司
                    dtblResult.Columns.Add(new DataColumn("Card_No", Type.GetType("System.String")));// 卡號
                    dtblResult.Columns.Add(new DataColumn("Chinese_Name", Type.GetType("System.String")));// 製卡中文姓名
                    dtblResult.Columns.Add(new DataColumn("Change_Eng_Name", Type.GetType("System.String")));// 有無異動製卡英文名(Y/N)
                    dtblResult.Columns.Add(new DataColumn("Eng_Name", Type.GetType("System.String")));// 異動後製卡英文名
                    dtblResult.Columns.Add(new DataColumn("GetCard_code", Type.GetType("System.String")));// 取卡方式(0/1)
                    dtblResult.Columns.Add(new DataColumn("Member_No", Type.GetType("System.String")));// 會員編號
                    dtblResult.Columns.Add(new DataColumn("Co_Marketing", Type.GetType("System.String")));// 協銷註記
                    dtblResult.Columns.Add(new DataColumn("KeyIn_Flag1", Type.GetType("System.String")));// 鍵一記録(員編/鍵檔時間)
                    dtblResult.Columns.Add(new DataColumn("KeyIn_Flag2", Type.GetType("System.String")));// 鍵二記録(員編/鍵檔時間)
                }
                else if (strProperty == "001")
                {
                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));// 序號
                    dtblResult.Columns.Add(new DataColumn("mod_date", Type.GetType("System.String")));// 作業日
                    dtblResult.Columns.Add(new DataColumn("receive_number", Type.GetType("System.String")));// 收件編號
                    dtblResult.Columns.Add(new DataColumn("query_key", Type.GetType("System.String")));// 比對值(key值)
                    dtblResult.Columns.Add(new DataColumn("1641", Type.GetType("System.String")));// eTag儲值
                    dtblResult.Columns.Add(new DataColumn("1642", Type.GetType("System.String")));// 臨時停車
                    dtblResult.Columns.Add(new DataColumn("1643", Type.GetType("System.String")));// 月租停車
                    dtblResult.Columns.Add(new DataColumn("user_1Key", Type.GetType("System.String")));// 鍵一同仁
                    dtblResult.Columns.Add(new DataColumn("user_2Key", Type.GetType("System.String")));// 鍵二同仁
                }
                else if (strProperty == "05010001")
                {
                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));// 序號
                    dtblResult.Columns.Add(new DataColumn("mod_date", Type.GetType("System.String")));// 作業日
                    dtblResult.Columns.Add(new DataColumn("query_key", Type.GetType("System.String")));// 統一編號
                    dtblResult.Columns.Add(new DataColumn("user_1Key", Type.GetType("System.String")));// 鍵一同仁
                    dtblResult.Columns.Add(new DataColumn("user_2Key", Type.GetType("System.String")));// 鍵二同仁
                }
                else if (strProperty == "01030203")//20211118_Ares_Jack_特店資料異動(解約)
                {
                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));// 序號
                    dtblResult.Columns.Add(new DataColumn("mod_date", Type.GetType("System.String")));// 作業日
                    dtblResult.Columns.Add(new DataColumn("query_key", Type.GetType("System.String")));// 比對值(key值)
                    dtblResult.Columns.Add(new DataColumn("after", Type.GetType("System.String")));// 比對值(key值)
                    dtblResult.Columns.Add(new DataColumn("user_1Key", Type.GetType("System.String")));// 鍵一同仁
                    dtblResult.Columns.Add(new DataColumn("user_2Key", Type.GetType("System.String")));// 鍵二同仁
                }
                else
                {
                    // 序號
                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));
                    // 作業日
                    dtblResult.Columns.Add(new DataColumn("mod_date", Type.GetType("System.String")));
                    // 比對值(Key值)
                    dtblResult.Columns.Add(new DataColumn("query_key", Type.GetType("System.String")));
                    // 鍵一同仁
                    dtblResult.Columns.Add(new DataColumn("user_1key", Type.GetType("System.String")));
                    // 鍵二同仁
                    dtblResult.Columns.Add(new DataColumn("user_2key", Type.GetType("System.String")));
                }

                #endregion 生成要返回的DataTable結構

                #region 依據Request查詢資料庫，將查詢結果寫入dtblResult

                string strIs2Key = "";
                string strAction = "";
                string strSqlSearch = "";

                #region 業務項目

                switch (strProperty)
                {
                    case "01010100":
                        // 卡人資料異動(地址)
                        strAction = " AND A.trans_id in ('01010100','A01') ";
                        strIs2Key = "N";
                        break;
                    case "01010400":
                        // 卡人資料異動(姓名)
                        strAction = " AND A.trans_id in ('01010400','A04','A03') ";
                        strIs2Key = "N";
                        break;
                    case "01010200":
                        // 卡人資料異動(其他)
                        strAction = " AND A.trans_id in ('01010200','A11') ";
                        strIs2Key = "N";
                        break;
                    case "01010300":
                        // 卡人資料異動(族群碼)
                        strAction = " AND A.trans_id in ('01010300','A06') ";
                        strIs2Key = "N";
                        break;
                    case "01010600":
                        // 卡人資料異動(他行自扣)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEARCH_OTHER_BANK_CARD, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01010800":
                        // 卡人資料異動(中信及郵局自扣)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEARCH_CHINA_TRUST, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01011000":
                        // 卡人資料異動(訊息/更正單自扣)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEARCH_UPDATE_INFO, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    // 卡人資料異動(新增自扣終止）edit by xiongxiaofeng 130524 start
                    case "01011200":
                        // 自扣終止與推廣員異動
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEARCH_AUTO_PAY_POPUL, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    // 卡人資料異動(新增自扣終止）edit by xiongxiaofeng 130524 end
                    case "01020100":
                        // 卡人資料異動(註銷)
                        strAction = " AND A.trans_id in ('01020100','B01') ";
                        strIs2Key = "N";
                        break;
                    case "01020300":
                        // 卡片資料異動(繳款異動)
                        strAction = " AND A.trans_id in ('01020300','B02') ";
                        strIs2Key = "N";
                        break;
                    case "01020400":
                        // 卡片資料異動(繳款評等)
                        strAction = " AND A.trans_id in ('01020400') ";
                        strIs2Key = "N";
                        break;
                    case "01020200":
                        // 卡片資料異動(狀況碼)
                        strAction = " AND A.trans_id in ('01020200','B05') ";
                        strIs2Key = "N";
                        break;
                    case "01020500":
                        // 卡片資料異動(優惠碼)
                        strAction = " AND A.trans_id in ('01020500','B04') ";
                        strIs2Key = "N";
                        break;
                    case "01020700":
                        // 卡片資料異動(毀補)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_CARD_SURPLY_1, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01021300":
                        // 卡片資料異動(掛補)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_CARD_SURPLY_2, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01020900":
                        // 卡片資料異動(BlockCode新增/異動)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_BLOCKCODE_ADD, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01021100":
                        // 卡片資料異動(BlockCode解除管制)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_BLOCKCODE_NOT_CONTROL, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01030200":
                        // 特店資料異動(資料異動)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_MODIFY, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01030100":
                        // 特店資料異動(資料異動/請款加批)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_ReqAppro, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01030201":
                        // 特店資料異動(費率)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_RATE, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01030202":
                        // 特店資料異動(帳號)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_ACCOUNT, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01030203":
                        // 特店資料異動(解約)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_FIRE, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01030204":
                        // 特店資料異動(機器)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_MACHINE, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01040101":
                        // 特店資料新增(6001)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_BASIC, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01040101M":
                        // 特店資料新增(6001會員附加服務)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_BASIC_M, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01040201":
                        // 特店資料新增(PCAM)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_PCAM, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01041100":
                        // 特店資料新增(特店延伸性/請款加批)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_ReqAppro_I, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01021500":
                        // 毀補轉一卡通
                        strIs2Key = "Y";
                        strSqlSearch = SEL_iPASS;
                        break;
                    case "001":
                        // 公用事業(ETAG業務)：001001(ETAG儲值1641)、001002(臨時停車1642)、001003(月租停車1643)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_UTILITIES, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "05010001":
                        // 公用事業(ETAG業務)：001001(ETAG儲值1641)、001002(臨時停車1642)、001003(月租停車1643)
                        strIs2Key = "Y";
                        strSqlSearch = AML_HeadOffice;
                        break;
                    default:
                        break;
                }

                #endregion

                if (strIs2Key != "Y")
                {
                    strSqlSearch = string.Format(SEL_COMMON_SQL, strAction, UtilHelper.GetAppSettings("DB_CSIP"));
                }

                // 聲明SQL Command變量
                SqlCommand sqlcmSearchData = new SqlCommand();
                sqlcmSearchData.CommandType = CommandType.Text;
                sqlcmSearchData.CommandText = strSqlSearch;
                // 區間起
                SqlParameter parmSearchStart = new SqlParameter("@SearchStart", strSearchStart.Replace("/", ""));
                sqlcmSearchData.Parameters.Add(parmSearchStart);
                // 區間迄
                SqlParameter parmSearchEnd = new SqlParameter("@SearchEnd", strSearchEnd.Replace("/", ""));
                sqlcmSearchData.Parameters.Add(parmSearchEnd);

                //20210517_Ares_Stanley-新增記錄SQL到Default LOG
                #region 記錄SQL到Default Log
                //將參數代入記錄用的SQL
                string recordSQL = sqlcmSearchData.CommandText;
                recordSQL = recordSQL.Replace("@SearchStart", string.Format("'{0}'", strSearchStart.Replace("/", ""))).Replace("@SearchEnd", string.Format("'{0}'", strSearchEnd.Replace("/", "")));
                Logging.Log("========== 執行作業量比對表-查詢 ==========\r" + recordSQL);
                #endregion 記錄SQL到Default Log

                // 查詢數據
                DataSet dstSearchData = BRCompareRpt.SearchOnDataSet(sqlcmSearchData);
                if (null == dstSearchData)
                {
                    // 查詢數據失敗
                    strMsgID = "01_03010000_004";
                    return null;
                }
                else
                {
                    // 查詢的數據不存在時
                    if (dstSearchData.Tables[0].Rows.Count == 0)
                    {
                        strMsgID = "01_03010000_005";
                        return null;
                    }
                }

                #region 依據查詢的記錄，產出報表資料

                if (strProperty == "01030100")
                {
                    #region 特店資料異動(資料異動/請款加批)

                    int intCount = 0;

                    for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                    {
                        intCount = intCount + 1;
                        // 寫入dtblResult
                        DataRow drowInsert = dtblResult.NewRow();
                        drowInsert["s_no"] = intCount;
                        drowInsert["mod_date"] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                        drowInsert["shop_id"] = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                        drowInsert["before"] = dstSearchData.Tables[0].Rows[intLoop][2].ToString();
                        drowInsert["after"] = dstSearchData.Tables[0].Rows[intLoop][3].ToString();
                        drowInsert["record_name"] = dstSearchData.Tables[0].Rows[intLoop][4].ToString();
                        drowInsert["business_name"] = dstSearchData.Tables[0].Rows[intLoop][5].ToString();
                        drowInsert["merchant_name"] = dstSearchData.Tables[0].Rows[intLoop][6].ToString();
                        drowInsert["user_name"] = dstSearchData.Tables[0].Rows[intLoop][7].ToString();
                        dtblResult.Rows.Add(drowInsert);
                    }

                    lngTotal = intCount;

                    #endregion
                }
                else if (strProperty == "01041100")
                {
                    #region 特店資料新增(特店延伸性/請款加批)

                    int intCount = 0;

                    for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                    {
                        intCount = intCount + 1;
                        // 寫入dtblResult
                        DataRow drowInsert = dtblResult.NewRow();
                        drowInsert["s_no"] = intCount;
                        drowInsert["mod_date"] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                        drowInsert["shop_id"] = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                        drowInsert["after"] = dstSearchData.Tables[0].Rows[intLoop][2].ToString();
                        drowInsert["record_name"] = dstSearchData.Tables[0].Rows[intLoop][3].ToString();
                        drowInsert["business_name"] = dstSearchData.Tables[0].Rows[intLoop][4].ToString();
                        drowInsert["merchant_name"] = dstSearchData.Tables[0].Rows[intLoop][5].ToString();
                        drowInsert["user_name"] = dstSearchData.Tables[0].Rows[intLoop][6].ToString();
                        dtblResult.Rows.Add(drowInsert);
                    }

                    lngTotal = intCount;

                    #endregion
                }
                else if (strProperty == "01011200")
                {
                    #region

                    Int64 lng1Key = 0;
                    Int64 lng2Key = 0;
                    lngTotal = 0;
                    int intCounterI = 0;
                    string[,] arrayUser = new string[100, 2];
                    string strColumnQueryKey = "";
                    string strColumnModDate = "";
                    string strReceive_Number = "";
                    string strAuto_pay_setting = "";
                    string strAuto_pay_setting1 = "";
                    string strUserKey = "";
                    string strAcc_No = "";

                    for (int intIndex = 0; intIndex < 100; intIndex++)
                    {
                        arrayUser[intIndex, 0] = "";
                        arrayUser[intIndex, 1] = "";
                    }

                    // 向dtblResult中寫入資料
                    lngTotal = dstSearchData.Tables[0].Rows.Count;

                    if (lngTotal > 0)
                    {
                        lngTotal = lngTotal / 2;
                    }

                    for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                    {
                        if (strColumnQueryKey == dstSearchData.Tables[0].Rows[intLoop][3].ToString()
                            && strColumnModDate == dstSearchData.Tables[0].Rows[intLoop][1].ToString()
                            && strReceive_Number == dstSearchData.Tables[0].Rows[intLoop]["receive_number"].ToString()
                            && strAuto_pay_setting == dstSearchData.Tables[0].Rows[intLoop]["auto_pay_setting"].ToString()
                            && strAuto_pay_setting1 == dstSearchData.Tables[0].Rows[intLoop]["auto_pay_setting1"].ToString()
                            && strUserKey == dstSearchData.Tables[0].Rows[intLoop]["user_key"].ToString()
                            && strAcc_No == dstSearchData.Tables[0].Rows[intLoop]["acc_no"].ToString()
                           )
                        {
                            if (dstSearchData.Tables[0].Rows[intLoop][7].ToString().Trim() == "1")
                            {
                                lng1Key = lng1Key + 1;
                                arrayUser[lng1Key, 0] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                            }
                            else
                            {
                                lng2Key = lng2Key + 1;
                                arrayUser[lng2Key, 1] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                            }
                        }
                        else
                        {
                            if (lng1Key > 0 || lng2Key > 0)
                            {
                                if (lng1Key < lng2Key)
                                {
                                    for (int intIndex = 1; intIndex <= lng2Key; intIndex++)
                                    {
                                        intCounterI = intCounterI + 1;
                                        // 寫入dtblResult
                                        DataRow drowInsert = dtblResult.NewRow();
                                        drowInsert["s_no"] = intCounterI;
                                        drowInsert["query_key"] = strColumnQueryKey;
                                        drowInsert["user_key"] = strUserKey;
                                        drowInsert["mod_date"] = strColumnModDate;
                                        drowInsert["auto_pay_setting"] = strAuto_pay_setting;
                                        drowInsert["auto_pay_setting1"] = strAuto_pay_setting1;
                                        drowInsert["receive_number"] = strReceive_Number;
                                        drowInsert["acc_no"] = strAcc_No;
                                        dtblResult.Rows.Add(drowInsert);
                                    }
                                }
                                else
                                {
                                    for (int intIndex = 1; intIndex <= lng1Key; intIndex++)
                                    {
                                        intCounterI = intCounterI + 1;
                                        // 寫入dtblResult 
                                        DataRow drowInsert = dtblResult.NewRow();
                                        drowInsert["s_no"] = intCounterI;
                                        drowInsert["query_key"] = strColumnQueryKey;
                                        drowInsert["user_key"] = strUserKey;
                                        drowInsert["mod_date"] = strColumnModDate;
                                        drowInsert["auto_pay_setting"] = strAuto_pay_setting;
                                        drowInsert["auto_pay_setting1"] = strAuto_pay_setting1;
                                        drowInsert["receive_number"] = strReceive_Number;
                                        drowInsert["acc_no"] = strAcc_No;
                                        dtblResult.Rows.Add(drowInsert);
                                    }
                                }
                            }

                            strColumnQueryKey = dstSearchData.Tables[0].Rows[intLoop][3].ToString();
                            strColumnModDate = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                            strReceive_Number = dstSearchData.Tables[0].Rows[intLoop]["receive_number"].ToString();
                            strAuto_pay_setting = dstSearchData.Tables[0].Rows[intLoop]["auto_pay_setting"].ToString();
                            strAuto_pay_setting1 = dstSearchData.Tables[0].Rows[intLoop]["auto_pay_setting1"].ToString();
                            strUserKey = dstSearchData.Tables[0].Rows[intLoop]["user_key"].ToString();
                            strAcc_No = dstSearchData.Tables[0].Rows[intLoop]["acc_no"].ToString();

                            lng1Key = 0;
                            lng2Key = 0;

                            // 清空arrayUser
                            for (int intIndex = 0; intIndex < 100; intIndex++)
                            {
                                arrayUser[intIndex, 0] = "";
                                arrayUser[intIndex, 1] = "";
                            }

                            if (dstSearchData.Tables[0].Rows[intLoop][7].ToString().Trim() == "1")
                            {
                                lng1Key = lng1Key + 1;
                                arrayUser[lng1Key, 0] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                            }
                            else
                            {
                                lng2Key = lng2Key + 1;
                                arrayUser[lng2Key, 1] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                            }
                        }
                    }

                    if (lng1Key > 0 || lng2Key > 0)
                    {
                        if (lng1Key < lng2Key)
                        {
                            for (int intIndex = 1; intIndex <= lng2Key; intIndex++)
                            {
                                intCounterI = intCounterI + 1;
                                // 寫入dtblResult
                                DataRow drowInsert = dtblResult.NewRow();
                                drowInsert["s_no"] = intCounterI;
                                drowInsert["query_key"] = strColumnQueryKey;
                                drowInsert["user_key"] = strUserKey;
                                drowInsert["mod_date"] = strColumnModDate;
                                drowInsert["auto_pay_setting"] = strAuto_pay_setting;
                                drowInsert["auto_pay_setting1"] = strAuto_pay_setting1;
                                drowInsert["receive_number"] = strReceive_Number;
                                drowInsert["acc_no"] = strAcc_No;
                                dtblResult.Rows.Add(drowInsert);
                            }
                        }
                        else
                        {
                            for (int intIndex = 1; intIndex <= lng1Key; intIndex++)
                            {
                                intCounterI = intCounterI + 1;
                                // 寫入dtblResult
                                DataRow drowInsert = dtblResult.NewRow();
                                drowInsert["s_no"] = intCounterI;
                                drowInsert["query_key"] = strColumnQueryKey;
                                drowInsert["user_key"] = strUserKey;
                                drowInsert["mod_date"] = strColumnModDate;
                                drowInsert["auto_pay_setting"] = strAuto_pay_setting;
                                drowInsert["auto_pay_setting1"] = strAuto_pay_setting1;
                                drowInsert["receive_number"] = strReceive_Number;
                                drowInsert["acc_no"] = strAcc_No;
                                dtblResult.Rows.Add(drowInsert);
                            }
                        }
                    }

                    #endregion
                }
                else if (strProperty == "01010800" || strProperty == "01010600" || strProperty == "01011000")
                {
                    #region

                    Int64 lng1Key = 0;
                    Int64 lng2Key = 0;
                    lngTotal = 0;
                    int intCounterI = 0;
                    string[,] arrayUser = new string[100, 2];
                    string strColumnQueryKey = "";
                    string strNewColumnQueryKey = "";
                    string strColumnModDate = "";
                    string strColumnUpLoad = "";
                    string strAuto_pay_setting = "";
                    string strCellP_Email_Setting = "";
                    string strE_Bill_Setting = "";
                    string strOutputByTXT_Setting = "";
                    string strReceive_Number = "";
                    string strAcc_No = "";// 顯示在報表中的銀行帳號(敏資處理)
                    string strOldAcc_No = "";// 資料庫中銀行帳號
                    string strNewAcc_No = "";

                    for (int intIndex = 0; intIndex < 100; intIndex++)
                    {
                        arrayUser[intIndex, 0] = "";
                        arrayUser[intIndex, 1] = "";
                    }

                    // 向dtblResult中寫入資料
                    lngTotal = dstSearchData.Tables[0].Rows.Count;

                    if (lngTotal > 0)
                    {
                        lngTotal = lngTotal / 2;
                    }

                    // 循環處理查出的每一筆資料
                    for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                    {
                        if (strColumnQueryKey == dstSearchData.Tables[0].Rows[intLoop][1].ToString()
                                && strColumnModDate == dstSearchData.Tables[0].Rows[intLoop][12].ToString()
                                && strColumnUpLoad == dstSearchData.Tables[0].Rows[intLoop][10].ToString()
                                && strAuto_pay_setting == dstSearchData.Tables[0].Rows[intLoop]["Auto_Pay_Setting"].ToString()
                                && strCellP_Email_Setting == dstSearchData.Tables[0].Rows[intLoop]["CellP_Email_Setting"].ToString()
                                && strE_Bill_Setting == dstSearchData.Tables[0].Rows[intLoop]["E_Bill_Setting"].ToString()
                                && strReceive_Number == dstSearchData.Tables[0].Rows[intLoop]["Receive_Number"].ToString()
                                && strOldAcc_No == dstSearchData.Tables[0].Rows[intLoop]["Acc_No"].ToString()
                                && strOutputByTXT_Setting == dstSearchData.Tables[0].Rows[intLoop]["OutputByTXT_Setting"].ToString())
                        {
                            if (dstSearchData.Tables[0].Rows[intLoop][9].ToString().Trim() == "1")
                            {
                                lng1Key = lng1Key + 1;
                                arrayUser[lng1Key, 0] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                            }
                            else
                            {
                                lng2Key = lng2Key + 1;
                                arrayUser[lng2Key, 1] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                            }
                        }
                        else
                        {
                            if (lng1Key > 0 || lng2Key > 0)
                            {
                                if (lng1Key < lng2Key)
                                {
                                    for (int intIndex = 1; intIndex <= lng2Key; intIndex++)
                                    {
                                        intCounterI = intCounterI + 1;
                                        // 寫入dtblResult
                                        DataRow drowInsert = dtblResult.NewRow();
                                        drowInsert["s_no"] = intCounterI;
                                        drowInsert["query_key"] = strColumnQueryKey;
                                        drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                        drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                        drowInsert["mod_date"] = strColumnModDate;
                                        drowInsert["auto_pay_setting"] = strAuto_pay_setting;
                                        drowInsert["cellp_email_setting"] = strCellP_Email_Setting;
                                        drowInsert["e_bill_setting"] = strE_Bill_Setting;
                                        drowInsert["outputbyTXT_setting"] = strOutputByTXT_Setting;
                                        drowInsert["Receive_Number"] = strReceive_Number;
                                        drowInsert["Acc_No"] = strAcc_No;
                                        dtblResult.Rows.Add(drowInsert);
                                    }
                                }
                                else
                                {
                                    for (int intIndex = 1; intIndex <= lng1Key; intIndex++)
                                    {
                                        intCounterI = intCounterI + 1;
                                        // 寫入dtblResult 
                                        DataRow drowInsert = dtblResult.NewRow();
                                        drowInsert["s_no"] = intCounterI;
                                        drowInsert["query_key"] = strColumnQueryKey;
                                        drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                        drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                        drowInsert["mod_date"] = strColumnModDate;
                                        drowInsert["auto_pay_setting"] = strAuto_pay_setting;
                                        drowInsert["cellp_email_setting"] = strCellP_Email_Setting;
                                        drowInsert["e_bill_setting"] = strE_Bill_Setting;
                                        drowInsert["outputbyTXT_setting"] = strOutputByTXT_Setting;
                                        drowInsert["Receive_Number"] = strReceive_Number;
                                        drowInsert["Acc_No"] = strAcc_No;
                                        dtblResult.Rows.Add(drowInsert);
                                    }
                                }
                            }

                            strNewColumnQueryKey = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                            strColumnQueryKey = strNewColumnQueryKey.Substring(0, 5).PadRight(strNewColumnQueryKey.Length, 'X');

                            strColumnModDate = dstSearchData.Tables[0].Rows[intLoop][12].ToString();
                            strColumnUpLoad = dstSearchData.Tables[0].Rows[intLoop][10].ToString();
                            strAuto_pay_setting = dstSearchData.Tables[0].Rows[intLoop]["Auto_Pay_Setting"].ToString();
                            strCellP_Email_Setting = dstSearchData.Tables[0].Rows[intLoop]["CellP_Email_Setting"].ToString();
                            strE_Bill_Setting = dstSearchData.Tables[0].Rows[intLoop]["E_Bill_Setting"].ToString();
                            strOutputByTXT_Setting = dstSearchData.Tables[0].Rows[intLoop]["OutputByTXT_Setting"].ToString();
                            strReceive_Number = dstSearchData.Tables[0].Rows[intLoop]["Receive_Number"].ToString();
                            strOldAcc_No = dstSearchData.Tables[0].Rows[intLoop]["Acc_No"].ToString();
                            strNewAcc_No = strOldAcc_No.Substring(strOldAcc_No.IndexOf("-") == -1 ? 0 : strOldAcc_No.IndexOf("-") + 1);
                            strAcc_No = strNewAcc_No.Length > 8 ? strNewAcc_No.Substring(0, 4).PadRight(strNewAcc_No.Length - 4, 'X') + strNewAcc_No.Substring(strNewAcc_No.Length - 4, 4) : strNewAcc_No;
                            lng1Key = 0;
                            lng2Key = 0;

                            // 清空arrayUser；
                            for (int intIndex = 0; intIndex < 100; intIndex++)
                            {
                                arrayUser[intIndex, 0] = "";
                                arrayUser[intIndex, 1] = "";
                            }

                            if (dstSearchData.Tables[0].Rows[intLoop][9].ToString().Trim() == "1")
                            {
                                lng1Key = lng1Key + 1;
                                arrayUser[lng1Key, 0] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                            }
                            else
                            {
                                lng2Key = lng2Key + 1;
                                arrayUser[lng2Key, 1] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                            }
                        }
                    }

                    if (lng1Key > 0 || lng2Key > 0)
                    {
                        if (lng1Key < lng2Key)
                        {
                            for (int intIndex = 1; intIndex <= lng2Key; intIndex++)
                            {
                                intCounterI = intCounterI + 1;
                                // 寫入dtblResult
                                DataRow drowInsert = dtblResult.NewRow();
                                drowInsert["s_no"] = intCounterI;
                                drowInsert["query_key"] = strColumnQueryKey;
                                drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                drowInsert["mod_date"] = strColumnModDate;
                                drowInsert["auto_pay_setting"] = strAuto_pay_setting;
                                drowInsert["cellp_email_setting"] = strCellP_Email_Setting;
                                drowInsert["e_bill_setting"] = strE_Bill_Setting;
                                drowInsert["outputbyTXT_setting"] = strOutputByTXT_Setting;
                                drowInsert["Receive_Number"] = strReceive_Number;
                                drowInsert["Acc_No"] = strAcc_No;
                                dtblResult.Rows.Add(drowInsert);
                            }
                        }
                        else
                        {
                            for (int intIndex = 1; intIndex <= lng1Key; intIndex++)
                            {
                                intCounterI = intCounterI + 1;
                                // 寫入dtblResult
                                DataRow drowInsert = dtblResult.NewRow();
                                drowInsert["s_no"] = intCounterI;
                                drowInsert["query_key"] = strColumnQueryKey;
                                drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                drowInsert["mod_date"] = strColumnModDate;
                                drowInsert["auto_pay_setting"] = strAuto_pay_setting;
                                drowInsert["cellp_email_setting"] = strCellP_Email_Setting;
                                drowInsert["e_bill_setting"] = strE_Bill_Setting;
                                drowInsert["outputbyTXT_setting"] = strOutputByTXT_Setting;
                                drowInsert["Receive_Number"] = strReceive_Number;
                                drowInsert["Acc_No"] = strAcc_No;
                                dtblResult.Rows.Add(drowInsert);
                            }
                        }
                    }

                    #endregion
                }
                else if (strProperty == "01021500")
                {
                    #region 毀補轉一卡通

                    int intCount = 0;
                    string str_temp = "";

                    for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                    {
                        intCount = intCount + 1;
                        DataRow drowInsert = dtblResult.NewRow();

                        drowInsert["s_no"] = intCount;
                        drowInsert["Co_Marketing_Com"] = dstSearchData.Tables[0].Rows[intLoop][0] != null ?
                            dstSearchData.Tables[0].Rows[intLoop][0].ToString() : "";

                        if (dstSearchData.Tables[0].Rows[intLoop][1] != null &&
                            !string.IsNullOrEmpty(dstSearchData.Tables[0].Rows[intLoop][1].ToString()))
                        {
                            str_temp = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                            if (str_temp.Length == 16)
                            {
                                str_temp = string.Format("{0}-****-{1}-{2}", str_temp.Substring(0, 4),
                                    str_temp.Substring(8, 4), str_temp.Substring(12, 4));
                                drowInsert["Card_No"] = str_temp;
                            }
                            else
                                drowInsert["Card_No"] = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                        }
                        else
                            drowInsert["Card_No"] = "";

                        if (dstSearchData.Tables[0].Rows[intLoop][2] != null &&
                            !string.IsNullOrEmpty(dstSearchData.Tables[0].Rows[intLoop][2].ToString()))
                        {
                            str_temp = dstSearchData.Tables[0].Rows[intLoop][2].ToString();
                            str_temp = string.Format("{0}*{1}", str_temp.Substring(0, 1), str_temp.Substring(str_temp.Length - 1, 1));
                            drowInsert["Chinese_Name"] = str_temp;
                        }
                        else
                            drowInsert["Chinese_Name"] = "";

                        drowInsert["Change_Eng_Name"] = dstSearchData.Tables[0].Rows[intLoop][3] != null ?
                            dstSearchData.Tables[0].Rows[intLoop][3].ToString() : "";
                        drowInsert["GetCard_code"] = dstSearchData.Tables[0].Rows[intLoop][4] != null ?
                            dstSearchData.Tables[0].Rows[intLoop][4].ToString() : "";
                        drowInsert["Member_No"] = dstSearchData.Tables[0].Rows[intLoop][5] != null ?
                            dstSearchData.Tables[0].Rows[intLoop][5].ToString() : "";
                        drowInsert["Co_Marketing"] = dstSearchData.Tables[0].Rows[intLoop][6] != null ?
                            dstSearchData.Tables[0].Rows[intLoop][6].ToString() : "";
                        drowInsert["KeyIn_Flag1"] = dstSearchData.Tables[0].Rows[intLoop][7] != null ?
                            dstSearchData.Tables[0].Rows[intLoop][7].ToString() : "";
                        drowInsert["KeyIn_Flag2"] = dstSearchData.Tables[0].Rows[intLoop][8] != null ?
                            dstSearchData.Tables[0].Rows[intLoop][8].ToString() : "";
                        dtblResult.Rows.Add(drowInsert);
                    }

                    lngTotal = intCount;

                    #endregion
                }
                else if (strProperty == "05010001")
                {
                    #region 特店AML資料

                    int intCount = 0;
                    DataRow _dr;
                    for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                    {
                        intCount = intCount + 1;
                        DataRow drowInsert = dtblResult.NewRow();
                        _dr = dstSearchData.Tables[0].Rows[intLoop];

                        drowInsert["s_no"] = intCount;
                        //drowInsert["mod_date"] = dstSearchData.Tables[0].Rows[intLoop]["keyin_day"].ToString();
                        ////drowInsert["query_key"] = dstSearchData.Tables[0].Rows[intLoop]["BasicTaxID"].ToString() + "".PadRight(5) + dstSearchData.Tables[0].Rows[intLoop]["BasicRegistyNameEN"].ToString();
                        //drowInsert["query_key"] = dstSearchData.Tables[0].Rows[intLoop]["BasicTaxID"].ToString() + "".PadRight(5) + dstSearchData.Tables[0].Rows[intLoop]["BasicRegistyNameCH"].ToString();
                        //drowInsert["user_1Key"] = dstSearchData.Tables[0].Rows[intLoop]["user_1Key"].ToString();
                        //drowInsert["user_2Key"] = dstSearchData.Tables[0].Rows[intLoop]["user_2Key"].ToString();

                        drowInsert["mod_date"] = _dr["keyin_day"].ToString();
                        drowInsert["query_key"] = string.Format("{0}{1}{2}", _dr["BasicTaxID"].ToString(), "".PadRight(5), _dr["BasicRegistyNameCH"].ToString());
                        drowInsert["user_1Key"] = _dr["user_1Key"].ToString();
                        drowInsert["user_2Key"] = _dr["user_2Key"].ToString();

                        dtblResult.Rows.Add(drowInsert);
                    }

                    lngTotal = intCount;

                    #endregion
                }
                else if (strProperty == "01030203")//20211118_Ares_Jack_特店資料異動(解約)
                {
                    //調整特店資料異動(解約)鍵二同仁未顯示問題 by Ares Stanley 20211125
                    #region 特店資料異動(解約)
                    Int64 lng1Key = 0;
                    Int64 lng2Key = 0;
                    int intCount = 0;
                    lngTotal = 0;
                    int intCounterI = 0;
                    string[,] arrayUser = new string[dstSearchData.Tables[0].Rows.Count, 2];
                    string strColumnQueryKey = "";
                    string strColumnModDate = "";
                    string strColumnUpLoad = "";
                    string strColumnAfter = "";

                    for (int intIndex = 0; intIndex < dstSearchData.Tables[0].Rows.Count; intIndex++)
                    {
                        arrayUser[intIndex, 0] = "";
                        arrayUser[intIndex, 1] = "";
                    }

                    // 向dtblResult中寫入資料
                    if (strIs2Key != "Y")
                    {
                        // 循環處理查出的每一筆資料
                        for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                        {
                            intCount = intCount + 1;
                            // 寫入dtblResult
                            DataRow drowInsert = dtblResult.NewRow();
                            drowInsert["s_no"] = intCount;
                            drowInsert["query_key"] = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                            drowInsert["user_1Key"] = "";
                            drowInsert["user_2Key"] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                            drowInsert["mod_date"] = dstSearchData.Tables[0].Rows[intLoop][7].ToString();
                            dtblResult.Rows.Add(drowInsert);
                        }

                        lngTotal = intCount;
                    }
                    else
                    {
                        lngTotal = dstSearchData.Tables[0].Rows.Count;

                        if (lngTotal > 0)
                        {
                            lngTotal = lngTotal / 2;
                        }

                        // 循環處理查出的每一筆資料
                        for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                        {

                            if (strColumnQueryKey == dstSearchData.Tables[0].Rows[intLoop][1].ToString()
                                    && strColumnModDate == dstSearchData.Tables[0].Rows[intLoop][12].ToString()
                                    && strColumnUpLoad == dstSearchData.Tables[0].Rows[intLoop][10].ToString()
                                    && !(lng1Key>0&& lng2Key>0&& lng1Key==lng2Key))
                            {
                                if (dstSearchData.Tables[0].Rows[intLoop][9].ToString().Trim() == "1")
                                {
                                    lng1Key = lng1Key + 1;
                                    arrayUser[lng1Key, 0] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                                }
                                else
                                {
                                    lng2Key = lng2Key + 1;
                                    arrayUser[lng2Key, 1] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                                }
                            }
                            else
                            {
                                if (lng1Key > 0 || lng2Key > 0)
                                {
                                    if (lng1Key < lng2Key)
                                    {
                                        for (int intIndex = 1; intIndex <= lng2Key; intIndex++)
                                        {
                                            intCounterI = intCounterI + 1;
                                            // 寫入dtblResult
                                            DataRow drowInsert = dtblResult.NewRow();
                                            drowInsert["s_no"] = intCounterI;
                                            drowInsert["query_key"] = strColumnQueryKey;
                                            drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                            drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                            drowInsert["mod_date"] = strColumnModDate;
                                            drowInsert["after"] = strColumnAfter;
                                            dtblResult.Rows.Add(drowInsert);
                                        }
                                    }
                                    else
                                    {
                                        for (int intIndex = 1; intIndex <= lng1Key; intIndex++)
                                        {
                                            intCounterI = intCounterI + 1;
                                            // 寫入dtblResult 
                                            DataRow drowInsert = dtblResult.NewRow();
                                            drowInsert["s_no"] = intCounterI;
                                            drowInsert["query_key"] = strColumnQueryKey;
                                            drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                            drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                            drowInsert["mod_date"] = strColumnModDate;
                                            drowInsert["after"] = strColumnAfter;
                                            dtblResult.Rows.Add(drowInsert);
                                        }
                                    }
                                }

                                strColumnQueryKey = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                                strColumnModDate = dstSearchData.Tables[0].Rows[intLoop][12].ToString();
                                strColumnUpLoad = dstSearchData.Tables[0].Rows[intLoop][10].ToString();
                                strColumnAfter = dstSearchData.Tables[0].Rows[intLoop]["after"].ToString();
                                lng1Key = 0;
                                lng2Key = 0;

                                //* 清空arrayUser；
                                for (int intIndex = 0; intIndex < dstSearchData.Tables[0].Rows.Count; intIndex++)
                                {
                                    arrayUser[intIndex, 0] = "";
                                    arrayUser[intIndex, 1] = "";
                                }

                                if (dstSearchData.Tables[0].Rows[intLoop][9].ToString().Trim() == "1")
                                {
                                    lng1Key = lng1Key + 1;
                                    arrayUser[lng1Key, 0] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                                }
                                else
                                {
                                    lng2Key = lng2Key + 1;
                                    arrayUser[lng2Key, 1] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                                }
                            }
                        }

                        if (lng1Key > 0 || lng2Key > 0)
                        {
                            if (lng1Key < lng2Key)
                            {
                                for (int intIndex = 1; intIndex <= lng2Key; intIndex++)
                                {
                                    intCounterI = intCounterI + 1;
                                    // 寫入dtblResult
                                    DataRow drowInsert = dtblResult.NewRow();
                                    drowInsert["s_no"] = intCounterI;
                                    drowInsert["query_key"] = strColumnQueryKey;
                                    drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                    drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                    drowInsert["mod_date"] = strColumnModDate;
                                    drowInsert["after"] = strColumnAfter;
                                    dtblResult.Rows.Add(drowInsert);
                                }
                            }
                            else
                            {
                                for (int intIndex = 1; intIndex <= lng1Key; intIndex++)
                                {
                                    intCounterI = intCounterI + 1;
                                    // 寫入dtblResult
                                    DataRow drowInsert = dtblResult.NewRow();
                                    drowInsert["s_no"] = intCounterI;
                                    drowInsert["query_key"] = strColumnQueryKey;
                                    drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                    drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                    drowInsert["mod_date"] = strColumnModDate;
                                    drowInsert["after"] = strColumnAfter;
                                    dtblResult.Rows.Add(drowInsert);
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region else

                    Int64 lng1Key = 0;
                    Int64 lng2Key = 0;
                    int intCount = 0;
                    lngTotal = 0;
                    int intCounterI = 0;
                    string[,] arrayUser = new string[100, 2];
                    string strColumnQueryKey = "";
                    string strColumnModDate = "";
                    string strColumnUpLoad = "";

                    for (int intIndex = 0; intIndex < 100; intIndex++)
                    {
                        arrayUser[intIndex, 0] = "";
                        arrayUser[intIndex, 1] = "";
                    }

                    // 向dtblResult中寫入資料
                    if (strIs2Key != "Y")
                    {
                        // 循環處理查出的每一筆資料
                        for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                        {
                            intCount = intCount + 1;
                            // 寫入dtblResult
                            DataRow drowInsert = dtblResult.NewRow();
                            drowInsert["s_no"] = intCount;
                            drowInsert["query_key"] = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                            drowInsert["user_1Key"] = "";
                            drowInsert["user_2Key"] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                            drowInsert["mod_date"] = dstSearchData.Tables[0].Rows[intLoop][7].ToString();
                            dtblResult.Rows.Add(drowInsert);
                        }

                        lngTotal = intCount;
                    }
                    else
                    {
                        if (strProperty == "001")
                        {
                            // 公用事業(ETAG業務)
                            string space = " ";
                            DataTable dtUtilities = dstSearchData.Tables[0];

                            for (int i = 0; dtUtilities.Rows.Count != 0; i++)
                            {
                                string str1641 = "";
                                string str1642 = "";
                                string str1643 = "";
                                string strCondition = "receive_number = '" + dtUtilities.Rows[0]["receive_number"] +
                                                        "' and primary_cardholder_id = '" + dtUtilities.Rows[0]["primary_cardholder_id"] + "'";
                                DataRow[] dataRows = dtUtilities.Select(strCondition);

                                foreach (DataRow dataRow in dataRows)
                                {
                                    //if (dataRow["business_category"].ToString() == "001001")
                                    //    str1641 = dataRow["1641"].ToString();
                                    //if (dataRow["business_category"].ToString() == "001002")
                                    //    str1642 = dataRow["1642"].ToString();
                                    //if (dataRow["business_category"].ToString() == "001003")
                                    //    str1643 = dataRow["1643"].ToString();
                                    if (dataRow["1641"].ToString() == "V")
                                        str1641 = dataRow["1641"].ToString();
                                    if (dataRow["1642"].ToString() == "V")
                                        str1642 = dataRow["1642"].ToString();
                                    if (dataRow["1643"].ToString() == "V")
                                        str1643 = dataRow["1643"].ToString();
                                }

                                // 寫入dtblResult
                                DataRow dtRow = dtblResult.NewRow();
                                intCounterI = intCounterI + 1;
                                dtRow["s_no"] = intCounterI;
                                dtRow["mod_date"] = dtUtilities.Rows[0]["keyin_date"];
                                // 收件編號、比對值(key值)、eTag儲值、臨時停車、月租停車組成一個欄位
                                //dtRow["query_key"] = dtUtilities.Rows[0]["receive_number"]
                                //                    + space.PadRight(5) + dtUtilities.Rows[0]["primary_cardholder_id"]
                                //                    + space.PadRight(12) + str1641
                                //                    + space.PadRight(17) + str1642
                                //                    + space.PadRight(17) + str1643;
                                dtRow["receive_number"] = dtUtilities.Rows[0]["receive_number"];
                                dtRow["query_key"] = dtUtilities.Rows[0]["primary_cardholder_id"];
                                dtRow["1641"] = dtUtilities.Rows[0]["1641"];// eTag儲值
                                dtRow["1642"] = dtUtilities.Rows[0]["1642"];// 臨時停車
                                dtRow["1643"] = dtUtilities.Rows[0]["1643"];// 月租停車
                                dtRow["user_1Key"] = dtUtilities.Rows[0]["user_name_k1"];// 鍵一同仁
                                dtRow["user_2Key"] = dtUtilities.Rows[0]["user_name_k2"];// 鍵二同仁
                                dtblResult.Rows.Add(dtRow);

                                // 移除已加入的Row
                                dtUtilities.Rows.Remove(dtUtilities.Rows[0]);
                                //foreach (DataRow row in dataRows)
                                //{
                                //    dtUtilities.Rows.Remove(row);
                                //}
                            }
                        }
                        else
                        {
                            lngTotal = dstSearchData.Tables[0].Rows.Count;

                            if (lngTotal > 0)
                            {
                                lngTotal = lngTotal / 2;
                            }

                            // 循環處理查出的每一筆資料
                            for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                            {

                                if (strColumnQueryKey == dstSearchData.Tables[0].Rows[intLoop][1].ToString()
                                        && strColumnModDate == dstSearchData.Tables[0].Rows[intLoop][12].ToString()
                                        && strColumnUpLoad == dstSearchData.Tables[0].Rows[intLoop][10].ToString())
                                {
                                    if (dstSearchData.Tables[0].Rows[intLoop][9].ToString().Trim() == "1")
                                    {
                                        lng1Key = lng1Key + 1;
                                        arrayUser[lng1Key, 0] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                                    }
                                    else
                                    {
                                        lng2Key = lng2Key + 1;
                                        arrayUser[lng2Key, 1] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                                    }
                                }
                                else
                                {
                                    if (lng1Key > 0 || lng2Key > 0)
                                    {
                                        if (lng1Key < lng2Key)
                                        {
                                            for (int intIndex = 1; intIndex <= lng2Key; intIndex++)
                                            {
                                                intCounterI = intCounterI + 1;
                                                // 寫入dtblResult
                                                DataRow drowInsert = dtblResult.NewRow();
                                                drowInsert["s_no"] = intCounterI;
                                                drowInsert["query_key"] = strColumnQueryKey;
                                                drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                                drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                                drowInsert["mod_date"] = strColumnModDate;
                                                dtblResult.Rows.Add(drowInsert);
                                            }
                                        }
                                        else
                                        {
                                            for (int intIndex = 1; intIndex <= lng1Key; intIndex++)
                                            {
                                                intCounterI = intCounterI + 1;
                                                // 寫入dtblResult 
                                                DataRow drowInsert = dtblResult.NewRow();
                                                drowInsert["s_no"] = intCounterI;
                                                drowInsert["query_key"] = strColumnQueryKey;
                                                drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                                drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                                drowInsert["mod_date"] = strColumnModDate;
                                                dtblResult.Rows.Add(drowInsert);
                                            }
                                        }
                                    }

                                    strColumnQueryKey = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                                    strColumnModDate = dstSearchData.Tables[0].Rows[intLoop][12].ToString();
                                    strColumnUpLoad = dstSearchData.Tables[0].Rows[intLoop][10].ToString();
                                    lng1Key = 0;
                                    lng2Key = 0;

                                    //* 清空arrayUser；
                                    for (int intIndex = 0; intIndex < 100; intIndex++)
                                    {
                                        arrayUser[intIndex, 0] = "";
                                        arrayUser[intIndex, 1] = "";
                                    }

                                    if (dstSearchData.Tables[0].Rows[intLoop][9].ToString().Trim() == "1")
                                    {
                                        lng1Key = lng1Key + 1;
                                        arrayUser[lng1Key, 0] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                                    }
                                    else
                                    {
                                        lng2Key = lng2Key + 1;
                                        arrayUser[lng2Key, 1] = dstSearchData.Tables[0].Rows[intLoop][0].ToString();
                                    }
                                }
                            }

                            if (lng1Key > 0 || lng2Key > 0)
                            {
                                if (lng1Key < lng2Key)
                                {
                                    for (int intIndex = 1; intIndex <= lng2Key; intIndex++)
                                    {
                                        intCounterI = intCounterI + 1;
                                        // 寫入dtblResult
                                        DataRow drowInsert = dtblResult.NewRow();
                                        drowInsert["s_no"] = intCounterI;
                                        drowInsert["query_key"] = strColumnQueryKey;
                                        drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                        drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                        drowInsert["mod_date"] = strColumnModDate;
                                        dtblResult.Rows.Add(drowInsert);
                                    }
                                }
                                else
                                {
                                    for (int intIndex = 1; intIndex <= lng1Key; intIndex++)
                                    {
                                        intCounterI = intCounterI + 1;
                                        // 寫入dtblResult
                                        DataRow drowInsert = dtblResult.NewRow();
                                        drowInsert["s_no"] = intCounterI;
                                        drowInsert["query_key"] = strColumnQueryKey;
                                        drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                        drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                        drowInsert["mod_date"] = strColumnModDate;
                                        dtblResult.Rows.Add(drowInsert);
                                    }
                                }
                            }
                        }
                    }

                    #endregion
                }

                #endregion 依據查詢的記錄，產出報表資料
                iTotalCount = dtblResult.Rows.Count;
                // 判斷頁次
                for (int i = 0; i < 10 * (iPageIndex - 1); i++)
                {
                    dtblResult.Rows.Remove(dtblResult.Rows[0]);
                }
                return dtblResult;

                #endregion 依據Request查詢資料庫，將查詢結果寫入dtblResult
            }
            catch (Exception exp)
            {
                BRCompareRpt.SaveLog(exp);
                strMsgID = "01_03010000_004";
                return null;
            }
        }
    }
}
