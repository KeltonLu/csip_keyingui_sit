//******************************************************************
//*  作    者：占偉林(James)
//*  功能說明：作業量比對報表
//*  創建日期：2009/10/28
//*  修改記錄：2021/03/17_Ares_Stanley-DB名稱改為變數
//* <author>            <time>            <TaskID>                <desc>
//* Ge.Song            2010/05/18         20090023                將現有4個子系統修改爲讀取Common中取得屬性的方法
//* Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*******************************************************************
using System;
using System.Data;
using System.Data.SqlClient;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Utility;

namespace CSIPKeyInGUI.BusinessRules
{
    /// <summary>
    /// 作業量比對報表業務類
    /// </summary>
    public class BRCompareRpt : CSIPCommonModel.BusinessRules.BRBase<EntityM_PROPERTY_CODE>
    {
        #region SQL 語句
        //* 他行自扣(卡人資料異動)
        //增加【異動後自扣銀行帳號】欄位 add by xiongxiaofeng 130523 start
        //private const string SEARCH_OTHER_BANK_CARD = @"select B.USER_NAME,A.Cus_ID,A.Other_Bank_Acc_No ," +
        private const string SEARCH_OTHER_BANK_CARD = @"select B.USER_NAME,A.Cus_ID,A.Other_Bank_Acc_No As Acc_No," +
        //增加【異動後自扣銀行帳號】欄位 add by xiongxiaofeng 130523 end
                        "A.Other_Bank_Pay_Way,A.Other_Bank_Cus_ID,A.bcycle_code,A.Mobile_Phone,A.E_Mail," +
                        "A.E_Bill,A.KeyIn_Flag,A.Upload_flag,A.user_id,A.mod_date,A.Receive_Number,A.Auto_Pay_Setting,A.CellP_Email_Setting,A.E_Bill_Setting,A.OutputByTXT_Setting " +
                    "FROM Other_Bank_Temp A,(select distinct user_id,User_name from {0}.dbo.M_USER) as  B " +
                    "where ( A.KeyIn_Flag='1' or A.KeyIn_Flag='2' ) and A.mod_date between @SearchStart and @SearchEnd " +
                        "and A.user_id = B.USER_ID and A.upload_flag = 'Y' " +
                    "order by Receive_Number,mod_date,cus_id,upload_flag desc," +
                        "keyin_flag,Other_Bank_Acc_No,Other_Bank_Pay_Way," +
                        "Other_Bank_Cus_ID,bcycle_code,mobile_phone,e_mail,e_bill";
        //* 中信及郵局自扣(卡人資料異動)
        private const string SEARCH_CHINA_TRUST = @"select B.USER_NAME,A.* " +
                    "FROM AUTO_PAY A,(select distinct user_id,User_name from {0}.dbo.M_USER) as B " +
                    "where A.mod_date  between @SearchStart and @SearchEnd " +
                        "and A.user_id = B.USER_ID and A.upload_flag = 'Y' and Add_Flag='0' " +
                    "order by Receive_Number,mod_date,cus_id,upload_flag desc," +
                        "keyin_flag,acc_no,pay_way,acc_id,bcycle_code,mobile_phone,e_mail,e_bill";
        //* 訊息/更正單自扣(卡人資料異動)
        private const string SEARCH_UPDATE_INFO = @"select B.USER_NAME,A.* " +
                    "FROM AUTO_PAY A,(select distinct user_id,User_name from {0}.dbo.M_USER) as B " +
                    "where A.mod_date between @SearchStart and @SearchEnd " +
                        "and A.user_id = B.USER_ID and A.upload_flag = 'Y' and Add_Flag='1' " +
                    "order by Receive_Number,mod_date,cus_id,upload_flag desc, " +
                        "keyin_flag,acc_no,pay_way,acc_id,bcycle_code,mobile_phone,e_mail,e_bill ";
        //* 毀補(卡片資料異動)
        private const string SEL_CARD_SURPLY_1 = @"select B.USER_NAME,A.Card_No,A.Eng_Name," +
                        "A.GetCard_code,A.Member_No,A.ReIssue_Type,'','','',A.KeyIn_Flag," + 
                        "A.Upload_Flag,A.user_id,A.mod_date " +
                    "FROM ReIssue_Card A,(select distinct user_id,User_name from {0}.dbo.M_USER) as B " +
                    "where A.mod_date between @SearchStart and @SearchEnd " +
                        "and A.ReIssue_Type = '2' " +
                        "and A.user_id = B.USER_ID and A.upload_flag = 'Y' " +
                    "order by A.mod_date,A.Card_No,A.upload_flag desc "+
                    ",A.keyin_flag,A.Eng_Name,A.GetCard_code,A.Member_No ";
        //* 掛補(卡片資料異動)
        private const string SEL_CARD_SURPLY_2 = @"select B.USER_NAME,A.Card_No,A.Eng_Name,A.GetCard_code," +
                        "A.Member_No,A.ReIssue_Type,'','','',A.KeyIn_Flag,A.Upload_Flag,A.user_id,A.mod_date " +
                    "FROM ReIssue_Card A,(select distinct user_id,User_name from {0}.dbo.M_USER) as B " +
                    "where A.mod_date between @SearchStart and @SearchEnd " +
                        "and A.ReIssue_Type = '1' " +
                        "and A.user_id = B.USER_ID and A.upload_flag = 'Y' " +
                    "order by A.mod_date,A.Card_No,A.upload_flag desc "+
                    ",A.keyin_flag,A.Eng_Name,A.GetCard_code,A.Member_No";
        //* BlockCode新增/異動(卡片資料異動)
        private const string SEL_BLOCKCODE_ADD = @"select B.USER_NAME,A.* " +
                    "FROM Change_BLK A,(select distinct user_id,User_name from {0}.dbo.M_USER) as B " +
                    "where A.mod_date between @SearchStart and @SearchEnd " +
                        "and A.user_id = B.USER_ID and A.upload_flag = 'Y'  and Change_Type='C' " +
                    "order by mod_date,Card_No,upload_flag desc,keyin_flag,BLOCK_CODE,PURGE_DATE," +
                        "MEMO,REASON_CODE,ACTION_CODE,CWB_REGIONS,Change_Type";
        //* BlockCode解除管制(卡片資料異動)
        private const string SEL_BLOCKCODE_NOT_CONTROL = @"select B.USER_NAME,A.* " +
                    "FROM Change_BLK A,(select distinct user_id,User_name from {0}.dbo.M_USER) as B " +
                    "where A.mod_date between @SearchStart and @SearchEnd " +
                        "and A.user_id = B.USER_ID and A.upload_flag = 'Y'  and Change_Type='D' " +
                    "order by mod_date,Card_No,upload_flag desc,keyin_flag,BLOCK_CODE,PURGE_DATE," +
                        "MEMO,REASON_CODE,ACTION_CODE,CWB_REGIONS,Change_Type";
        //* 特店資料異動(資料異動)
        private const string SEL_SHOP_MODIFY = @"select D.USER_NAME,C.shop_id,C.Change_Item," +
                        "'','','','','','',C.KeyIn_Flag,'Y' upload_flag,C.user_id,C.mod_date " +
                    "from shop_Upload C,(select distinct user_id,User_name from {0}.dbo.M_USER) as D " +
                    "where C.mod_date between @SearchStart and @SearchEnd " +
                        "and C.user_id = D.USER_ID  and C.Change_Item = '1' " +
                    "order by mod_date,substring(C.mod_time,1,4),shop_id,upload_flag,KeyIn_Flag ";
        //* 特店資料異動(資料異動/請款加批)
        private const string SEL_SHOP_ReqAppro = @"select b.mod_date,b.shop_id, b.before, b.after, b.record_name, b.business_name, b.merchant_name, d.user_name " +
                      "from (select * from shop_reqappro where keyin_flag='2' and newCreate_flag='N') as b, (select distinct user_id,User_name from {0}.dbo.M_USER) as d " +
                      "where b.mod_date between @SearchStart and @SearchEnd and b.user_id=d.user_id " +
                      "order by b.mod_date,substring(b.mod_time,1,4),b.shop_id";
        //* 特店資料異動(費率)
        private const string SEL_SHOP_RATE = @"select D.USER_NAME,C.shop_id,C.Change_Item," + 
                        "'','','','','','',C.KeyIn_Flag,'Y' upload_flag,C.user_id,C.mod_date " +
                    "from shop_Upload C,(select distinct user_id,User_name from {0}.dbo.M_USER) as D " +
                    "where C.mod_date between @SearchStart and @SearchEnd " +
                        "and C.user_id = D.USER_ID  and C.Change_Item = '2' " +
                    "order by mod_date,substring(C.mod_time,1,4),shop_id,upload_flag,KeyIn_Flag ";
        //* 特店資料異動(帳號)
        private const string SEL_SHOP_ACCOUNT = @"select D.USER_NAME,C.shop_id,C.Change_Item," + 
                        "'','','','','','',C.KeyIn_Flag,'Y' upload_flag,C.user_id,C.mod_date " +
                    "from shop_Upload C,(select distinct user_id,User_name from {0}.dbo.M_USER) as D " +
                    "where C.mod_date between @SearchStart and @SearchEnd " +
                        "and C.user_id = D.USER_ID  and C.Change_Item = '3' " +
                    "order by mod_date,substring(C.mod_time,1,4),shop_id,upload_flag,KeyIn_Flag ";
        //* 特店資料異動(解約)
        private const string SEL_SHOP_FIRE = @"select D.USER_NAME,C.shop_id,C.Change_Item," +
                        "'','','','','','', C.KeyIn_Flag,'Y' upload_flag,C.user_id,C.mod_date " +
                    "from shop_Upload C,(select distinct user_id,User_name from {0}.dbo.M_USER) as D " +
                    "where C.mod_date between @SearchStart and @SearchEnd " +
                        "and C.user_id = D.USER_ID  and C.Change_Item = '4' " +
                    "order by mod_date,substring(C.mod_time,1,4),shop_id,upload_flag,KeyIn_Flag ";
        //* 特店資料異動(機器)
        private const string SEL_SHOP_MACHINE = @"select D.user_name,C.shop_id,C.Change_Item," + 
                        "'','','','','','',C.KeyIn_Flag,'Y' upload_flag,C.user_id,C.mod_date " +
                    "from shop_Upload C,(select distinct user_id,User_name from {0}.dbo.M_USER) as D " +
                    "where C.mod_date between @SearchStart and @SearchEnd " +
                        "and C.user_id = D.USER_ID  and C.Change_Item = '5' " +
                    "order by mod_date,substring(C.mod_time,1,4),shop_id,upload_flag,KeyIn_Flag ";
        //* 其他查詢資料的SQL
        private const string SEL_COMMON_SQL = @"select B.USER_NAME,A.query_key,A.trans_id," +
                        "substring(A.mod_time,1,4),'','','',A.mod_date " +
                    "from customer_log A,(select distinct user_id,User_name from {1}.dbo.M_USER) as B " +
                    "where A.mod_date between @SearchStart and @SearchEnd " +
                        "and A.user_id = B.USER_ID {0}" +
                    "group by A.query_key,A.trans_id,B.user_name,A.mod_date,substring(A.mod_time,1,4) " +
                    "order by A.mod_date,substring(A.mod_time,1,4),A.query_key";

        //* 特店資料新增(6001)
        private const string SEL_SHOP_BASIC = @"select B.USER_NAME,recv_no + '/'+ A.uni_no1+'-'+A.uni_no2 as MerNo,'','','','','','',''," +
                                                "A.KeyIn_Flag,A.sendhost_flag,A.keyin_userid,A.keyin_day " +
                                                "FROM shop_basic A,(select distinct user_id,User_name from {0}.dbo.M_USER) as  B " +
                                                "where A.keyin_day between @SearchStart and @SearchEnd and " +
                                                "A.keyin_userid = B.USER_ID and A.sendhost_flag = 'Y' " +
                                                "and A.member_service <> 'Y' " +
                                                "order by keyin_day,MerNo,keyin_flag ";

        //* 特店資料新增(6001)會員附加服務
        private const string SEL_SHOP_BASIC_M = @"select B.USER_NAME,recv_no + '/'+ A.uni_no1+'-'+A.uni_no2 + '-' + A.member_service as MerNo,'','','','','','',''," +
                                                "A.KeyIn_Flag,A.sendhost_flag,A.keyin_userid,A.keyin_day " +
                                                "FROM shop_basic A,(select distinct user_id,User_name from {0}.dbo.M_USER) as  B " +
                                                "where A.keyin_day between @SearchStart and @SearchEnd and " +
                                                "A.keyin_userid = B.USER_ID and A.sendhost_flag = 'Y' " +
                                                "and A.member_service = 'Y' " +
                                                "order by keyin_day,MerNo,keyin_flag ";

        //* 特店資料新增(PCAM)
        private const string SEL_SHOP_PCAM = @"select B.USER_NAME,A.merchant_nbr,'','','','','','',''," +
                                                "A.KeyIn_Flag,A.send3270,A.keyin_userid,A.keyin_day " +
                                                "FROM shop_pcam A,(select distinct user_id,User_name from {0}.dbo.M_USER) as  B " +
                                                "where A.keyin_day between @SearchStart and @SearchEnd and " +
                                                "A.keyin_userid = B.USER_ID and A.send3270 = 'Y' " +
                                                "order by keyin_day,merchant_nbr,keyin_flag ";

        //* 特店資料新增(特店延伸性/請款加批)
        private const string SEL_SHOP_ReqAppro_I = @"select b.mod_date,b.shop_id, b.after, b.record_name, b.business_name, b.merchant_name, d.user_name " +
                      "from (select * from shop_reqappro where keyin_flag='2' and newCreate_flag='Y') as b, (select distinct user_id,User_name from {0}.dbo.M_USER) as d " +
                      "where b.mod_date between @SearchStart and @SearchEnd and b.user_id=d.user_id " +
                      "order by b.mod_date,substring(b.mod_time,1,4),b.shop_id";

        #region 新增自扣終止（卡人資料異動）edit by xiongxiaofeng 130524 start
        /*private const string SEARCH_AUTO_PAY_POPUL = @"select B.USER_NAME ,Convert(varchar(8),A.mod_date,112) AS mod_date, " +
                    " A.Receive_Number,A.Cus_Id,'' As Acc_No, 'Y' AS auto_pay_setting,'N' As cellP_Email_Setting, "+
                    " 'N' As E_Bill_Setting,'N' AS OutputByTXT_Setting,keyin_flag " +
                    " FROM Auto_Pay_Popul A ,(select distinct user_id,User_name from {0}.dbo.M_USER) as  B  " +
                    //" where ( A.KeyIn_Flag='1' or A.KeyIn_Flag='2' ) and " +
                    " where ( A.KeyIn_Flag='0') and " +
                    " A.mod_date  between @SearchStart and @SearchEnd " +
                    " and A.user_id = B.USER_ID and A.isEnd='Y' " +
                    "order by mod_date,cus_id,Receive_Number desc," +
                        "keyin_flag";*/
        private const string SEARCH_AUTO_PAY_POPUL = @"select B.USER_NAME as user_key ,Convert(varchar(8),A.mod_date,112) AS mod_date, " +
                    " A.receive_number,stuff(A.Cus_Id,8,3,'xxx') as Cus_Id,stuff(acc_no,4,3,'xxx') as acc_no, case when FUNCTION_CODE ='D' then 'Y' else '' end AS auto_pay_setting, " +
                    " case when FUNCTION_CODE ='C' then 'Y' else '' end AS auto_pay_setting1 ,keyin_flag " +
                    " FROM Auto_Pay_Popul A ,(select distinct user_id,User_name from {0}.dbo.M_USER) as  B  " +
                    " where ( A.FUNCTION_CODE='C' or A.FUNCTION_CODE='D') and " +
                    " A.mod_date  between @SearchStart and @SearchEnd " +
                    " and A.user_id = B.USER_ID " +
                    "order by Receive_Number,mod_date,cus_id desc," +
                        "keyin_flag";
        #endregion 新增自扣終止（卡人資料異動）edit by xiongxiaofeng 130524 end 

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

        #endregion

        //* REQ-20090023 將現有4個子系統修改爲讀取Common中取得屬性的方法 Add by Ge.Song 2010/05/18 Start
        ///// <summary>
        /////  查詢業務項目
        ///// </summary>
        ///// <returns>EntitySet</returns>
        //public static EntitySet<EntityM_PROPERTY_CODE> SelectEntitySet()
        //{
        //    SqlHelper sSql = new SqlHelper();
        //    EntitySet<EntityM_PROPERTY_CODE> eProperty_CodeSet = null;
        //    try
        //    {
        //        sSql.AddCondition(EntityM_PROPERTY_CODE.M_FUNCTION_KEY, Operator.Equal, DataTypeUtils.String, "01");
        //        sSql.AddCondition(EntityM_PROPERTY_CODE.M_PROPERTY_KEY, Operator.Equal, DataTypeUtils.String, "11");
        //        eProperty_CodeSet = BRCompareRpt.Search(sSql.GetFilterCondition(), "Connection_CSIP");
        //        //* 按SEQUENCE欄位排序
        //        eProperty_CodeSet.Sort(EntityM_PROPERTY_CODE.M_SEQUENCE, ESortType.ASC);
        //    }
        //    catch(Exception exp)
        //    {
        //        BRCompareRpt.SaveLog(exp);
        //        return null;
        //    }
        //    return eProperty_CodeSet;
        //}
        //* REQ-20090023 將現有4個子系統修改爲讀取Common中取得屬性的方法 Add by Ge.Song 2010/05/18 End

        /// <summary>
        /// 取報表數據
        /// </summary>
        /// <param name="strProperty">業務項目</param>
        /// <param name="strSearchStart">區間起</param>
        /// <param name="strSearchEnd">區間迄</param>
        /// <param name="lngTotal">總量</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>成功時：返回查詢結果；失敗時：null</returns>
        public static DataTable SearchRPTData(string strProperty, string strSearchStart,
                                string strSearchEnd, ref Int64 lngTotal, ref string strMsgID)
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
                else if (strProperty == "01010800" || strProperty == "01010600" || strProperty=="01011000")
                {
                   
                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));
                    dtblResult.Columns.Add(new DataColumn("mod_date", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("query_key", Type.GetType("System.String")));

                    dtblResult.Columns.Add(new DataColumn("auto_pay_setting", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("cellP_Email_Setting", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("E_Bill_Setting", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("OutputByTXT_Setting", Type.GetType("System.String")));

                    dtblResult.Columns.Add(new DataColumn("user_1key", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("user_2key", Type.GetType("System.String")));

                    //增加【收件編號】和【異動後自扣銀行帳號】欄位 add by xiongxiaofeng 130523 start
                    dtblResult.Columns.Add(new DataColumn("receive_number", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("acc_no", Type.GetType("System.String")));
                    //增加【收件編號】和【異動後自扣銀行帳號】欄位 add by xiongxiaofeng 130523 end 
                }
                //新增自扣終止（卡人資料異動） edit by xiongxiaofeng 130524 start
                else if (strProperty == "01011200")
                {

                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));
                    dtblResult.Columns.Add(new DataColumn("mod_date", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("query_key", Type.GetType("System.String")));

                    dtblResult.Columns.Add(new DataColumn("auto_pay_setting", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("auto_pay_setting1", Type.GetType("System.String")));
                   /* dtblResult.Columns.Add(new DataColumn("cellP_Email_Setting", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("E_Bill_Setting", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("OutputByTXT_Setting", Type.GetType("System.String")));
                    */
                    dtblResult.Columns.Add(new DataColumn("user_key", Type.GetType("System.String")));

                    dtblResult.Columns.Add(new DataColumn("receive_number", Type.GetType("System.String")));
                    dtblResult.Columns.Add(new DataColumn("acc_no", Type.GetType("System.String")));
                }
                // 毀補轉一卡通
                else if (strProperty == "01021500")
                {
                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));
                    dtblResult.Columns.Add(new DataColumn("Co_Marketing_Com", Type.GetType("System.String")));  // 協銷公司
                    dtblResult.Columns.Add(new DataColumn("Card_No", Type.GetType("System.String")));           // 卡號
                    dtblResult.Columns.Add(new DataColumn("Chinese_Name", Type.GetType("System.String")));      // 製卡中文姓名
                    dtblResult.Columns.Add(new DataColumn("Change_Eng_Name", Type.GetType("System.String")));   // 有無異動製卡英文名(Y/N)
                    dtblResult.Columns.Add(new DataColumn("Eng_Name", Type.GetType("System.String")));          // 異動後製卡英文名
                    dtblResult.Columns.Add(new DataColumn("GetCard_code", Type.GetType("System.String")));      // 取卡方式(0/1)
                    dtblResult.Columns.Add(new DataColumn("Member_No", Type.GetType("System.String")));         // 會員編號
                    dtblResult.Columns.Add(new DataColumn("Co_Marketing", Type.GetType("System.String")));      // 協銷註記
                    dtblResult.Columns.Add(new DataColumn("KeyIn_Flag1", Type.GetType("System.String")));       // 鍵一記録(員編/鍵檔時間)
                    dtblResult.Columns.Add(new DataColumn("KeyIn_Flag2", Type.GetType("System.String")));       // 鍵二記録(員編/鍵檔時間)
                }
                //新增自扣終止（卡人資料異動） edit by xiongxiaofeng 130524 end
                else
                {
                    //* 序號
                    dtblResult.Columns.Add(new DataColumn("s_no", Type.GetType("System.Int32")));
                    //* 作業日
                    dtblResult.Columns.Add(new DataColumn("mod_date", Type.GetType("System.String")));
                    //* 比對值(Key值)
                    dtblResult.Columns.Add(new DataColumn("query_key", Type.GetType("System.String")));
                    //* 鍵一同仁
                    dtblResult.Columns.Add(new DataColumn("user_1key", Type.GetType("System.String")));
                    //* 鍵二同仁
                    dtblResult.Columns.Add(new DataColumn("user_2key", Type.GetType("System.String")));
                }
                #endregion 生成要返回的DataTable結構

                #region 依據Request查詢資料庫，將查詢結果寫入dtblResult
                string strIs2Key = "";
                string strAction = "";
                string strSqlSearch = "";
                switch (strProperty)    //* 業務項目
                {
                    case "01010100":
                        //* 地址(卡人資料異動)
                        strAction = " AND A.trans_id in ('01010100','A01') ";
                        strIs2Key = "N";
                        break;
                    case "01010400":
                        //* 姓名(卡人資料異動)
                        strAction = " AND A.trans_id in ('01010400','A04','A03') ";
                        strIs2Key = "N";
                        break;
                    case "01010200":
                        //其他(卡人資料異動)
                        strAction = " AND A.trans_id in ('01010200','A11') ";
                        strIs2Key = "N";
                        break;
                    case "01010300":
                        //族群碼(卡人資料異動)
                        strAction = " AND A.trans_id in ('01010300','A06') ";
                        strIs2Key = "N";
                        break;
                    case "01010600":
                        //他行自扣(卡人資料異動)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEARCH_OTHER_BANK_CARD, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01010800":
                        //中信及郵局自扣(卡人資料異動)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEARCH_CHINA_TRUST, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01011000":
                        //訊息/更正單自扣(卡人資料異動)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEARCH_UPDATE_INFO, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    //新增自扣終止（卡人資料異動） edit by xiongxiaofeng 130524 start
                    case "01011200":
                        //自扣終止與推廣員異動
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEARCH_AUTO_PAY_POPUL, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    //新增自扣終止（卡人資料異動） edit by xiongxiaofeng 130524 end
                    case "01020100":
                        //註銷(卡片資料異動)
                        strAction = " AND A.trans_id in ('01020100','B01') ";
                        strIs2Key = "N";
                        break;
                    case "01020300":
                        //繳款異動(卡片資料異動)
                        strAction = " AND A.trans_id in ('01020300','B02') ";
                        strIs2Key = "N";
                        break;
                    case "01020400":
                        //繳款評等(卡片資料異動)
                        strAction = " AND A.trans_id in ('01020400') ";
                        strIs2Key = "N";
                        break;
                    case "01020200":
                        //狀況碼(卡片資料異動)
                        strAction = " AND A.trans_id in ('01020200','B05') ";
                        strIs2Key = "N";
                        break;
                    case "01020500":
                        //優惠碼(卡片資料異動)
                        strAction = " AND A.trans_id in ('01020500','B04') ";
                        strIs2Key = "N";
                        break;
                    case "01020700":
                        //毀補(卡片資料異動)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_CARD_SURPLY_1, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01021300":
                        //掛補(卡片資料異動)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_CARD_SURPLY_2, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01020900":
                        // BlockCode新增/異動(卡片資料異動)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_BLOCKCODE_ADD, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01021100":
                        // BlockCode解除管制(卡片資料異動)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_BLOCKCODE_NOT_CONTROL, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01030200":
                        //特店資料異動(資料異動)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_MODIFY, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01030100":
                        //特店資料異動(資料異動/請款加批)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_ReqAppro, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01030201":
                        //特店資料異動(費率)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_RATE, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01030202":
                        //特店資料異動(帳號)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_ACCOUNT, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01030203":
                        //特店資料異動(解約)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_FIRE, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01030204":
                        //特店資料異動(機器)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_MACHINE, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01040101":
                        //特店資料新增(6001)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_BASIC, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01040101M":
                        //特店資料新增(6001)會員附加服務
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_BASIC_M, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01040201":
                        //特店資料新增(PCAM)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_PCAM, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01041100":
                        //特店資料新增(特店延伸性/請款加批)
                        strIs2Key = "Y";
                        strSqlSearch = string.Format(SEL_SHOP_ReqAppro_I, UtilHelper.GetAppSettings("DB_CSIP"));
                        break;
                    case "01021500":
                        // 毀補轉一卡通
                        strIs2Key = "Y";
                        strSqlSearch = SEL_iPASS;
                        break;
                    default:
                        break;
                }

                if (strIs2Key != "Y")
                {
                    strSqlSearch = string.Format(SEL_COMMON_SQL, strAction, UtilHelper.GetAppSettings("DB_CSIP"));
                }

                //* 聲明SQL Command變量
                SqlCommand sqlcmSearchData = new SqlCommand();
                sqlcmSearchData.CommandType = CommandType.Text;
                sqlcmSearchData.CommandText = strSqlSearch;
                //* 區間起
                SqlParameter parmSearchStart = new SqlParameter("@SearchStart", strSearchStart.Replace("/", ""));
                sqlcmSearchData.Parameters.Add(parmSearchStart);
                //* 區間迄
                SqlParameter parmSearchEnd = new SqlParameter("@SearchEnd", strSearchEnd.Replace("/", ""));
                sqlcmSearchData.Parameters.Add(parmSearchEnd);

                //* 查詢數據
                DataSet dstSearchData = BRCompareRpt.SearchOnDataSet(sqlcmSearchData);
                if (null == dstSearchData)  //* 查詢數據失敗
                {
                    strMsgID = "01_03010000_004";
                    return null;
                }
                else
                {
                    //* 查詢的數據不存在時
                    if (dstSearchData.Tables[0].Rows.Count == 0)
                    {
                        strMsgID = "01_03010000_005";
                        return null;
                    }
                }

                #region 依據查詢的記錄，產出報表資料
                #region 特店資料異動(資料異動/請款加批)
                if (strProperty == "01030100")
                {
                    int intCount = 0;
                    for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                    {
                        intCount = intCount + 1;
                        //* 寫入  dtblResult  
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
                }
                #endregion
                #region 特店資料新增(特店延伸性/請款加批)
                else if (strProperty == "01041100")
                {
                    int intCount = 0;
                    for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                    {
                        intCount = intCount + 1;
                        //* 寫入  dtblResult  
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
                }
                #endregion
                #region 新增自扣終止（卡人資料異動） edit by xiongxiaofeng 130524 start
                else if (strProperty == "01011200")
                {
                    Int64 lng1Key = 0;
                    Int64 lng2Key = 0;
                    lngTotal = 0;
                    int intCounterI = 0;
                    string[,] arrayUser = new string[100, 2];
                    string strColumnQueryKey = "";
                    string strColumnModDate = "";
                    //string strAuto_pay_setting = "";
                    //string strCellP_Email_Setting = "";
                    //string strE_Bill_Setting = "";
                   // string strOutputByTXT_Setting = "";
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

                    //* 向dtblResult中寫入資料

                    lngTotal = dstSearchData.Tables[0].Rows.Count;

                    if (lngTotal > 0)
                    {
                        lngTotal = lngTotal / 2;
                    }
                    //* 循環處理查出的每一筆資料
                    for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                    {

                        if (strColumnQueryKey == dstSearchData.Tables[0].Rows[intLoop][3].ToString()
                                && strColumnModDate == dstSearchData.Tables[0].Rows[intLoop][1].ToString()
                                //&& strColumnUpLoad == dstSearchData.Tables[0].Rows[intLoop][10].ToString()                               
                                && strReceive_Number == dstSearchData.Tables[0].Rows[intLoop]["receive_number"].ToString()
                                && strAuto_pay_setting == dstSearchData.Tables[0].Rows[intLoop]["auto_pay_setting"].ToString()
                                && strAuto_pay_setting1 == dstSearchData.Tables[0].Rows[intLoop]["auto_pay_setting1"].ToString()
                                && strUserKey == dstSearchData.Tables[0].Rows[intLoop]["user_key"].ToString()
                                && strAcc_No == dstSearchData.Tables[0].Rows[intLoop]["acc_no"].ToString()
                           )
                        {
                            //if (dstSearchData.Tables[0].Rows[intLoop][9].ToString().Trim() == "1")
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
                                        //* 寫入  dtblResult
                                        DataRow drowInsert = dtblResult.NewRow();
                                        /*drowInsert["s_no"] = intCounterI;
                                        drowInsert["query_key"] = strColumnQueryKey;
                                        drowInsert["user_Key"] = arrayUser[intIndex, 0];
                                        drowInsert["mod_date"] = strColumnModDate;
                                        drowInsert["auto_pay_setting"] = "Y";
                                        drowInsert["cellp_email_setting"] = "N";
                                        drowInsert["e_bill_setting"] = "N";
                                        drowInsert["outputbyTXT_setting"] = "N";
                                        drowInsert["Receive_Number"] = strReceive_Number;
                                        drowInsert["Acc_No"] = "";*/
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
                                        //* 寫入  dtblResult 
                                        DataRow drowInsert = dtblResult.NewRow();
                                        /*drowInsert["s_no"] = intCounterI;
                                        drowInsert["query_key"] = strColumnQueryKey;
                                        drowInsert["user_Key"] = arrayUser[intIndex, 0];
                                        drowInsert["mod_date"] = strColumnModDate;
                                        drowInsert["auto_pay_setting"] = "Y";
                                        drowInsert["cellp_email_setting"] = "N";
                                        drowInsert["e_bill_setting"] = "N";
                                        drowInsert["outputbyTXT_setting"] = "N";
                                        drowInsert["Receive_Number"] = strReceive_Number;
                                        drowInsert["Acc_No"] = "";*/
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
                            }//end for If(lng1Key > 0 || lng2Key > 0)

                            strColumnQueryKey = dstSearchData.Tables[0].Rows[intLoop][3].ToString();
                            strColumnModDate = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                            //strAuto_pay_setting = "Y";
                            //strCellP_Email_Setting = "N";
                            //strE_Bill_Setting = "N";
                            //strOutputByTXT_Setting = "N";
                            strReceive_Number = dstSearchData.Tables[0].Rows[intLoop]["receive_number"].ToString();
                            strAuto_pay_setting = dstSearchData.Tables[0].Rows[intLoop]["auto_pay_setting"].ToString();
                            strAuto_pay_setting1 = dstSearchData.Tables[0].Rows[intLoop]["auto_pay_setting1"].ToString();
                            strUserKey = dstSearchData.Tables[0].Rows[intLoop]["user_key"].ToString();
                            strAcc_No = dstSearchData.Tables[0].Rows[intLoop]["acc_no"].ToString();
                            

                            lng1Key = 0;
                            lng2Key = 0;

                            //* 清空arrayUser；
                            for (int intIndex = 0; intIndex < 100; intIndex++)
                            {
                                arrayUser[intIndex, 0] = "";
                                arrayUser[intIndex, 1] = "";
                            }

                            //if (dstSearchData.Tables[0].Rows[intLoop][9].ToString().Trim() == "1")
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
                    }//結束循環處理查出的每一筆資料

                    if (lng1Key > 0 || lng2Key > 0)
                    {
                        if (lng1Key < lng2Key)
                        {
                            for (int intIndex = 1; intIndex <= lng2Key; intIndex++)
                            {
                                intCounterI = intCounterI + 1;
                                //* 寫入  dtblResult
                                DataRow drowInsert = dtblResult.NewRow();
                                /*drowInsert["s_no"] = intCounterI;
                                drowInsert["query_key"] = strColumnQueryKey;
                                drowInsert["user_Key"] = arrayUser[intIndex, 0];
                                drowInsert["mod_date"] = strColumnModDate;
                                drowInsert["auto_pay_setting"] = "Y";
                                drowInsert["cellp_email_setting"] = "N";
                                drowInsert["e_bill_setting"] = "N";
                                drowInsert["outputbyTXT_setting"] = "N";
                                drowInsert["Receive_Number"] = strReceive_Number;
                                drowInsert["Acc_No"] = "";*/
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
                                //* 寫入  dtblResult
                                DataRow drowInsert = dtblResult.NewRow();
                                /*drowInsert["s_no"] = intCounterI;
                                drowInsert["query_key"] = strColumnQueryKey;
                                drowInsert["user_Key"] = arrayUser[intIndex, 0];
                                drowInsert["mod_date"] = strColumnModDate;
                                drowInsert["auto_pay_setting"] = "Y";
                                drowInsert["cellp_email_setting"] = "N";
                                drowInsert["e_bill_setting"] = "N";
                                drowInsert["outputbyTXT_setting"] = "N";
                                drowInsert["Receive_Number"] = strReceive_Number;
                                drowInsert["Acc_No"] = "";*/
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
                    }//end for If(lng1Key > 0 || lng2Key > 0)

                }
                #endregion
                #region 新增自扣終止（卡人資料異動） edit by xiongxiaofeng 130524 end
                else if (strProperty == "01010800" || strProperty == "01010600" || strProperty == "01011000")
                {
                    Int64 lng1Key = 0;
                    Int64 lng2Key = 0;
                    int intCount = 0;
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
                    //增加【收件編號】和【異動後自扣銀行帳號】欄位 add by xiongxiaofeng 130522 start
                    string strReceive_Number = "";
                    string strAcc_No = "";  // 顯示在報表中的銀行帳號（敏資處理）
                    string strOldAcc_No = ""; //資料庫中銀行帳號
                    string strNewAcc_No = "";
                    //增加【收件編號】和【異動後自扣銀行帳號】欄位 add by xiongxiaofeng 130522 end

                    for (int intIndex = 0; intIndex < 100; intIndex++)
                    {
                        arrayUser[intIndex, 0] = "";
                        arrayUser[intIndex, 1] = "";
                    }

                    //* 向dtblResult中寫入資料

                    lngTotal = dstSearchData.Tables[0].Rows.Count;

                    if (lngTotal > 0)
                    {
                        lngTotal = lngTotal / 2;
                    }
                    //* 循環處理查出的每一筆資料
                    for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                    {

                        if (strColumnQueryKey == dstSearchData.Tables[0].Rows[intLoop][1].ToString()
                                && strColumnModDate == dstSearchData.Tables[0].Rows[intLoop][12].ToString()
                                && strColumnUpLoad == dstSearchData.Tables[0].Rows[intLoop][10].ToString()
                                && strAuto_pay_setting == dstSearchData.Tables[0].Rows[intLoop]["Auto_Pay_Setting"].ToString()
                                && strCellP_Email_Setting == dstSearchData.Tables[0].Rows[intLoop]["CellP_Email_Setting"].ToString()
                                && strE_Bill_Setting == dstSearchData.Tables[0].Rows[intLoop]["E_Bill_Setting"].ToString()
                            //增加【收件編號】和【異動後自扣銀行帳號】欄位 add by xiongxiaofeng 130522 start
                                && strReceive_Number == dstSearchData.Tables[0].Rows[intLoop]["Receive_Number"].ToString()
                                && strOldAcc_No == dstSearchData.Tables[0].Rows[intLoop]["Acc_No"].ToString()
                            //增加【收件編號】和【異動後自扣銀行帳號】欄位 add by xiongxiaofeng 130522 end
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
                                        //* 寫入  dtblResult
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
                                        //增加【收件編號】和【異動後自扣銀行帳號】欄位 add by xiongxiaofeng 130522 start
                                        drowInsert["Receive_Number"] = strReceive_Number;
                                        drowInsert["Acc_No"] = strAcc_No;
                                        //增加【收件編號】和【異動後自扣銀行帳號】欄位 add by xiongxiaofeng 130522 end
                                        dtblResult.Rows.Add(drowInsert);
                                    }
                                }
                                else
                                {
                                    for (int intIndex = 1; intIndex <= lng1Key; intIndex++)
                                    {
                                        intCounterI = intCounterI + 1;
                                        //* 寫入  dtblResult 
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
                                        //增加【收件編號】和【異動後自扣銀行帳號】欄位 add by xiongxiaofeng 130522 start
                                        drowInsert["Receive_Number"] = strReceive_Number;
                                        drowInsert["Acc_No"] = strAcc_No;
                                        //增加【收件編號】和【異動後自扣銀行帳號】欄位 add by xiongxiaofeng 130522 end
                                        dtblResult.Rows.Add(drowInsert);
                                    }
                                }
                            }//end for If(lng1Key > 0 || lng2Key > 0)

                            //strColumnQueryKey = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                            strNewColumnQueryKey = dstSearchData.Tables[0].Rows[intLoop][1].ToString();
                            strColumnQueryKey = strNewColumnQueryKey.Substring(0, 5).PadRight(strNewColumnQueryKey.Length, 'X');
                            
                            strColumnModDate = dstSearchData.Tables[0].Rows[intLoop][12].ToString();
                            strColumnUpLoad = dstSearchData.Tables[0].Rows[intLoop][10].ToString();
                            strAuto_pay_setting = dstSearchData.Tables[0].Rows[intLoop]["Auto_Pay_Setting"].ToString();
                            strCellP_Email_Setting = dstSearchData.Tables[0].Rows[intLoop]["CellP_Email_Setting"].ToString();
                            strE_Bill_Setting = dstSearchData.Tables[0].Rows[intLoop]["E_Bill_Setting"].ToString();
                            strOutputByTXT_Setting = dstSearchData.Tables[0].Rows[intLoop]["OutputByTXT_Setting"].ToString();
                            //增加【收件編號】和【異動後自扣銀行帳號】欄位 add by xiongxiaofeng 130522 start
                            strReceive_Number = dstSearchData.Tables[0].Rows[intLoop]["Receive_Number"].ToString();
                            strOldAcc_No = dstSearchData.Tables[0].Rows[intLoop]["Acc_No"].ToString();
                            strNewAcc_No = strOldAcc_No.Substring(strOldAcc_No.IndexOf("-") == -1 ? 0 : strOldAcc_No.IndexOf("-") + 1);
                            strAcc_No = strNewAcc_No.Length > 8 ? strNewAcc_No.Substring(0, 4).PadRight(strNewAcc_No.Length - 4, 'X') + strNewAcc_No.Substring(strNewAcc_No.Length - 4, 4) : strNewAcc_No;
                            //增加【收件編號】和【異動後自扣銀行帳號】欄位 add by xiongxiaofeng 130522 end
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
                    }//結束循環處理查出的每一筆資料

                    if (lng1Key > 0 || lng2Key > 0)
                    {
                        if (lng1Key < lng2Key)
                        {
                            for (int intIndex = 1; intIndex <= lng2Key; intIndex++)
                            {
                                intCounterI = intCounterI + 1;
                                //* 寫入  dtblResult
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
                                //增加【收件編號】和【異動後自扣銀行帳號】欄位 add by xiongxiaofeng 130522 start
                                drowInsert["Receive_Number"] = strReceive_Number;
                                drowInsert["Acc_No"] = strAcc_No;
                                //增加【收件編號】和【異動後自扣銀行帳號】欄位 add by xiongxiaofeng 130522 end
                                dtblResult.Rows.Add(drowInsert);
                            }
                        }
                        else
                        {
                            for (int intIndex = 1; intIndex <= lng1Key; intIndex++)
                            {
                                intCounterI = intCounterI + 1;
                                //* 寫入  dtblResult
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
                                //增加【收件編號】和【異動後自扣銀行帳號】欄位 add by xiongxiaofeng 130522 start
                                drowInsert["Receive_Number"] = strReceive_Number;
                                drowInsert["Acc_No"] = strAcc_No;
                                //增加【收件編號】和【異動後自扣銀行帳號】欄位 add by xiongxiaofeng 130522 end
                                dtblResult.Rows.Add(drowInsert);
                            }
                        }
                    }//end for If(lng1Key > 0 || lng2Key > 0)

                }
#endregion
                #region 毀補轉一卡通
                else if (strProperty == "01021500")
                {
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
                }
                #endregion
                #region else
                else
                {
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

                    //* 向dtblResult中寫入資料
                    if (strIs2Key != "Y")
                    {
                        //* 循環處理查出的每一筆資料
                        for (int intLoop = 0; intLoop < dstSearchData.Tables[0].Rows.Count; intLoop++)
                        {
                            intCount = intCount + 1;
                            //* 寫入  dtblResult  
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
                        //* 循環處理查出的每一筆資料
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
                                            //* 寫入  dtblResult
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
                                            //* 寫入  dtblResult 
                                            DataRow drowInsert = dtblResult.NewRow();
                                            drowInsert["s_no"] = intCounterI;
                                            drowInsert["query_key"] = strColumnQueryKey;
                                            drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                            drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                            drowInsert["mod_date"] = strColumnModDate;
                                            dtblResult.Rows.Add(drowInsert);
                                        }
                                    }
                                }//end for If(lng1Key > 0 || lng2Key > 0)

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
                        }//結束循環處理查出的每一筆資料

                        if (lng1Key > 0 || lng2Key > 0)
                        {
                            if (lng1Key < lng2Key)
                            {
                                for (int intIndex = 1; intIndex <= lng2Key; intIndex++)
                                {
                                    intCounterI = intCounterI + 1;
                                    //* 寫入  dtblResult
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
                                    //* 寫入  dtblResult
                                    DataRow drowInsert = dtblResult.NewRow();
                                    drowInsert["s_no"] = intCounterI;
                                    drowInsert["query_key"] = strColumnQueryKey;
                                    drowInsert["user_1Key"] = arrayUser[intIndex, 0];
                                    drowInsert["user_2Key"] = arrayUser[intIndex, 1];
                                    drowInsert["mod_date"] = strColumnModDate;
                                    dtblResult.Rows.Add(drowInsert);
                                }
                            }
                        }//end for If(lng1Key > 0 || lng2Key > 0)
                    }
                }
                #endregion
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

    }
}
