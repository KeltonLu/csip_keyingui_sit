//******************************************************************
//*  作    者：占偉林(James)
//*  功能說明：作業量統計表
//*  創建日期：2009/11/11
//*  修改記錄：
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*******************************************************************
using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using CSIPKeyInGUI.EntityLayer;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Logging;
using Framework.Common.Utility;

namespace CSIPKeyInGUI.BusinessRules
{
    /// <summary>
    /// 作業量比對報表業務類
    /// </summary>
    public class BRStatisticsRpt : CSIPCommonModel.BusinessRules.BRBase<EntityCUSTOMER_LOG>
    {    
        #region SQL 語句
        #region 卡人資料異動
        //* 地址(卡人資料異動)
        private const string SEL_CARD_PERSON_ADDRESS = @"select trans_id,mod_date,substring(mod_time,1,4),count(distinct(query_key)) " + 
                            "from customer_log " +
                            "where mod_date between @SearchStart and @SearchEnd " +
                                "and trans_id in ('01010100','A01') " +
                            "group by trans_id,mod_date,substring(mod_time,1,4) " +
                            "order by mod_date,substring(mod_time,1,4),trans_id ";
        //* 姓名(卡人資料異動)
        private const string SEL_CARD_PERSON_NAME = @"select trans_id,mod_date,substring(mod_time,1,4),count(distinct(query_key)) " + 
                            "from customer_log " +
                            "where mod_date between @SearchStart and @SearchEnd " +
                                "and trans_id in ('01010400','A04','A03') " +
                            "group by trans_id,mod_date,substring(mod_time,1,4) " +
                            "order by mod_date,substring(mod_time,1,4),trans_id ";
        //* 其他(卡人資料異動)
        private const string SEL_CARD_PERSON_OTHER = @"select trans_id,mod_date,substring(mod_time,1,4),count(distinct(query_key)) " + 
                            "from customer_log " +
                            "where mod_date between @SearchStart and @SearchEnd " +
                                "and trans_id in ('01010200','A11') " +
                            "group by trans_id,mod_date,substring(mod_time,1,4) " +
                            "order by mod_date,substring(mod_time,1,4),trans_id ";
        //* 族群碼(卡人資料異動)
        private const string SEL_CARD_PERSON_FAMILY = @"select trans_id,mod_date,substring(mod_time,1,4),count(distinct(query_key)) " +
                            "from customer_log " +
                            "where mod_date between @SearchStart and @SearchEnd " +
                                "and trans_id in ('01010300','A06') " +
                            "group by trans_id,mod_date,substring(mod_time,1,4) " +
                            "order by mod_date,substring(mod_time,1,4),trans_id ";
        //* 他行自扣(卡人資料異動)
        private const string SEL_CARD_PERSON_OTHER_BANK = @"select '1' flag,count(cus_id) " + 
                            "from Other_Bank_Temp " +
                            "where mod_date between @SearchStart and @SearchEnd and keyIn_flag = '1' " +
                            "union " +
                            "select '2' flag,count(cus_id) " +
                            "from Other_Bank_Temp " +
                            "where mod_date between @SearchStart and @SearchEnd and keyIn_flag = '2' " + 
                            "order by flag ";
        //* 中信及郵局自扣(卡人資料異動)
        private const string SEL_CARD_PERSON_CHINA_TRUST = @"select '1' flag,count(cus_id) " +
                            "from auto_pay " +
                            "where mod_date between @SearchStart and @SearchEnd and keyIn_flag = '1' and Add_Flag='0' " + 
                            "union " +
                            "select '2' flag,count(cus_id) " +
                            "from auto_pay " +
                            "where mod_date between @SearchStart and @SearchEnd and keyIn_flag = '2' and Add_Flag='0' " +
                            "order by flag ";
        //* 訊息/更正單自扣(卡人資料異動)
        private const string SEL_CARD_PERSON_MODIFY = @"select '1' flag,count(cus_id) " +
                            "from auto_pay " +
                            "where mod_date between @SearchStart and @SearchEnd and keyIn_flag = '1' and Add_Flag='1' " +
                            "union " +
                            "select '2' flag,count(cus_id) " +
                            "from auto_pay " +
                            "where mod_date between @SearchStart and @SearchEnd and keyIn_flag = '2' and Add_Flag='1' " +
                            "order by flag ";
        #endregion

        #region 卡片資料異動
        //* 註銷(卡片資料異動)
        private const string SEL_CARD_UNREG = @"select trans_id,mod_date,substring(mod_time,1,4),count(distinct(query_key)) " +
                            "from customer_log " +
                            "where mod_date between @SearchStart and @SearchEnd " +
                                "and trans_id in ('01020100','B01') " +
                            "group by trans_id,mod_date,substring(mod_time,1,4) " +
                            "order by mod_date,substring(mod_time,1,4),trans_id ";
        //* 狀況碼(卡片資料異動)
        private const string SEL_CARD_STATUS = @"select trans_id,mod_date,substring(mod_time,1,4),count(distinct(query_key)) " +
                            "from customer_log " +
                            "where mod_date between @SearchStart and @SearchEnd " +
                                "and trans_id in ('01020200','B05') " +
                            "group by trans_id,mod_date,substring(mod_time,1,4) " +
                            "order by mod_date,substring(mod_time,1,4),trans_id ";
        //* 優惠碼(卡片資料異動)
        private const string SEL_CARD_PERCENT = @"select trans_id,mod_date,substring(mod_time,1,4),count(distinct(query_key)) " +
                            "from customer_log " +
                            "where mod_date between @SearchStart and @SearchEnd " +
                                "and trans_id in ('01020500','B04') " +
                            "group by trans_id,mod_date,substring(mod_time,1,4) " +
                            "order by mod_date,substring(mod_time,1,4),trans_id ";
        //* 繳款異動(卡片資料異動)
        private const string SEL_CARD_MONEY = @"select trans_id,mod_date,substring(mod_time,1,4),count(distinct(query_key)) " +
                            "from customer_log " +
                            "where mod_date between @SearchStart and @SearchEnd " +
                                "and trans_id in ('01020300','B02') " +
                            "group by trans_id,mod_date,substring(mod_time,1,4) " +
                            "order by mod_date,substring(mod_time,1,4),trans_id ";
        //* 繳款評等(卡片資料異動)
        private const string SEL_CARD_MONEY_GRADE = @"select trans_id,mod_date,substring(mod_time,1,4),count(distinct(query_key)) " +
                            "from customer_log " +
                            "where mod_date between @SearchStart and @SearchEnd " +
                                "and trans_id = '01020400' " +
                            "group by trans_id,mod_date,substring(mod_time,1,4) " +
                            "order by mod_date,substring(mod_time,1,4),trans_id ";
        //* 掛補(二KEY)
        private const string SEL_KEY2_SURPLY = @"select '1' flag,count(card_no) " +
                            "from ReIssue_Card " +
                            "where mod_date between @SearchStart and @SearchEnd " +
                                "and keyIn_flag = '1' and ReIssue_Type = '1' " +
                            "union " +
                            "select '2' flag,count(card_no) " +
                            "from ReIssue_Card " +
                            "where mod_date between @SearchStart and @SearchEnd " +
                                "and keyIn_flag = '2' and ReIssue_Type = '1' " +
                            "order by flag ";
        //* 毀補(二KEY)
        private const string SEL_KEY2_SURPLY_DESTORY = @"select '1' flag,count(card_no) " + 
                            "from ReIssue_Card " +
                            "where mod_date between @SearchStart and @SearchEnd " + 
                                "and keyIn_flag = '1' and ReIssue_Type = '2' " + 
                            "union " +
                            "select '2' flag,count(card_no) " + 
                            "from ReIssue_Card " + 
                            "where mod_date between @SearchStart and @SearchEnd " +
                                "and keyIn_flag = '2' and ReIssue_Type = '2' " + 
                            "order by flag ";
        //* BlockCode新增/異動(卡片資料異動)
        private const string SEL_BLOCK_CODE_ADD = @"select '1' flag,count(card_no) " +
                            "from Change_BLK " + 
                            "where mod_date between @SearchStart and @SearchEnd " +
                                "and keyIn_flag = '1'  and Change_Type='C' " + 
                            "union " +
                            "select '2' flag,count(card_no) " + 
                            "from Change_BLK " +
                            "where mod_date between @SearchStart and @SearchEnd " +
                                "and keyIn_flag = '2'  and Change_Type='C' " +
                            "order by flag ";
        //* BlockCode解除管制(卡片資料異動)
        private const string SEL_BLOCK_CODE_NOT_CONTROL = @"select '1' flag,count(card_no) " +
                            "from Change_BLK " + 
                            "where mod_date between @SearchStart and @SearchEnd " +
                                "and keyIn_flag = '1' and Change_Type='D' " +
                            "union " +
                            "select '2' flag,count(card_no) " +
                            "from Change_BLK " +
                            "where mod_date between @SearchStart and @SearchEnd " +
                                "and keyIn_flag = '2' and Change_Type='D' " +
                            "order by flag ";
        #endregion

        #region 特店資料
        //* 特店資料異動(資料異動)
        private const string SEL_SHOP_MODIFY = @"select 'S' sTABLE,KEYIN_FLAG,count(shop_id) " +
                            "from SHOP " +
                            "where mod_date between @SearchStart and @SearchEnd and CHANGE_ITEM = '1' " +
            " and(rtrim(ltrim(record_name))<>'' or rtrim(ltrim(business_name))<>'' or rtrim(ltrim(merchant_name))<>'' or rtrim(ltrim(english_name))<>'' or rtrim(ltrim(undertaker))<>'' or" +
            " rtrim(ltrim(undertaker_id))<>'' or rtrim(ltrim(undertaker_engName))<>'' or rtrim(ltrim(undertaker_tel1))<>'' or rtrim(ltrim(undertaker_tel2))<>'' or rtrim(ltrim(undertaker_tel3))<>'' or" +
            " rtrim(ltrim(undertaker_add1))<>'' or rtrim(ltrim(undertaker_add2))<>'' or rtrim(ltrim(undertaker_add3))<>'' or rtrim(ltrim(realperson))<>'' or rtrim(ltrim(realperson_id))<>'' or" +
            " rtrim(ltrim(realperson_tel1))<>'' or rtrim(ltrim(realperson_tel2))<>'' or rtrim(ltrim(realperson_tel3))<>'' or rtrim(ltrim(junctionperson))<>'' or rtrim(ltrim(junctionperson_tel1))<>'' or" +
            " rtrim(ltrim(junctionperson_tel2))<>'' or rtrim(ltrim(junctionperson_tel3 ))<>'' or rtrim(ltrim(junctionperson_fax1))<>'' or rtrim(ltrim(junctionperson_fax2))<>'' or rtrim(ltrim(realperson_add1))<>'' or" +
            " rtrim(ltrim(realperson_add2))<>'' or rtrim(ltrim(realperson_add3))<>'' or rtrim(ltrim(junctionperson_recadd1))<>'' or rtrim(ltrim(junctionperson_recadd2))<>'' or rtrim(ltrim(junctionperson_recadd3))<>'' or" +
            " rtrim(ltrim(realadd_zip))<>'' or rtrim(ltrim(junctionperson_realadd1))<>'' or rtrim(ltrim(junctionperson_realadd2))<>'' or rtrim(ltrim(junctionperson_realadd3))<>'' or rtrim(ltrim(zip))<>'' or" +
            " rtrim(ltrim(commadd1))<>'' or rtrim(ltrim(comaddr2))<>'' or rtrim(ltrim(introduce))<>'' or rtrim(ltrim(introduce_flag))<>'' or rtrim(ltrim(invoice_cycle))<>'' or" +
            " rtrim(ltrim(hold_stmt))<>'' or  rtrim(ltrim(class))<>'' or  rtrim(ltrim(black))<>'') " +
                            "GROUP BY KEYIN_FLAG " +
                            "union " +
                            "select 'U' sTABLE,KEYIN_FLAG,count(shop_id) " +
                            "from SHOP_UPLOAD " +
                            "where mod_date between @SearchStart and @SearchEnd and CHANGE_ITEM = '1' " +
                            "GROUP BY KEYIN_FLAG " +
                            "order by KEYIN_FLAG ";
        //* 特店資料異動(資料異動/請款加批)
        private const string SEL_SHOP_ReqAppro_U = @"select 'S' sTABLE,KEYIN_FLAG,count(shop_id) " +
                            "from SHOP " +
                            "where mod_date between @SearchStart and @SearchEnd and CHANGE_ITEM = '1' " +
                            "GROUP BY KEYIN_FLAG " +
                            "union " +
                            "select 'R' sTABLE,KEYIN_FLAG,count(shop_id) " +
                            "from SHOP_ReqAppro " +
                            "where mod_date between @SearchStart and @SearchEnd and CHANGE_ITEM = '1' " +
                            "GROUP BY KEYIN_FLAG " +
                            "order by KEYIN_FLAG ";
        //* 特店資料異動(費率)
        private const string SEL_SHOP_RATE = @"select 'S' sTABLE,KEYIN_FLAG,count(shop_id) " +
                            "from SHOP " +
                            "where mod_date between @SearchStart and @SearchEnd and CHANGE_ITEM = '2' " +
                            "GROUP BY KEYIN_FLAG " +
                            "union " +
                            "select 'U' sTABLE,KEYIN_FLAG,count(shop_id) " +
                            "from SHOP_UPLOAD " +
                            "where mod_date between @SearchStart and @SearchEnd and CHANGE_ITEM = '2' " +
                            "GROUP BY KEYIN_FLAG " +
                            "order by KEYIN_FLAG ";
        //* 特店資料異動(帳號)
        private const string SEL_SHOP_ACCOUNT = @"select 'S' sTABLE,KEYIN_FLAG,count(shop_id) " + 
                            "from SHOP " +
                            "where mod_date between @SearchStart and @SearchEnd and CHANGE_ITEM = '3' " +
                            "GROUP BY KEYIN_FLAG " +
                            "union " +
                            "select 'U' sTABLE,KEYIN_FLAG,count(shop_id) " +
                            "from SHOP_UPLOAD " +
                            "where mod_date between @SearchStart and @SearchEnd and CHANGE_ITEM = '3' " +
                            "GROUP BY KEYIN_FLAG " +
                            "order by KEYIN_FLAG ";
        //* 特店資料異動(解約)
        private const string SEL_SHOP_FIRE = @"select 'S' sTABLE,KEYIN_FLAG,count(shop_id) " + 
                            "from SHOP " +
                            "where mod_date between @SearchStart and @SearchEnd and CHANGE_ITEM = '4' " +
                            "GROUP BY KEYIN_FLAG " +
                            "union " +
                            "select 'U' sTABLE,KEYIN_FLAG,count(shop_id) " +
                            "from SHOP_UPLOAD " +
                            "where mod_date between @SearchStart and @SearchEnd and CHANGE_ITEM = '4' " +
                            "GROUP BY KEYIN_FLAG " +
                            "order by KEYIN_FLAG ";
        //* 特店資料異動(機器)
        private const string SEL_SHOP_MACHINE = @"select 'S' sTABLE,KEYIN_FLAG,count(shop_id) " +
                            "from SHOP " +
                            "where mod_date between @SearchStart and @SearchEnd and CHANGE_ITEM = '5' " +
                            "GROUP BY KEYIN_FLAG " +
                            "union " +
                            "select 'U' sTABLE,KEYIN_FLAG,count(shop_id) " +
                            "from SHOP_UPLOAD " +
                            "where mod_date between @SearchStart and @SearchEnd and CHANGE_ITEM = '5' " +
                            "GROUP BY KEYIN_FLAG " +
                            "order by KEYIN_FLAG ";

        //* 特店新增 6001
        private const string SEL_SHOP_BASIC = @"select keyin_flag,count(recv_no) " + //取得所有的 1 KEY 筆數
                            "from shop_basic " +
                            "where keyin_day between @SearchStart and @SearchEnd AND keyin_flag='1' " +
                            "and member_service <> 'Y' " +
                            "GROUP BY KEYIN_FLAG " +
                            "union " +
                            "select keyin_flag,count(recv_no) " +    //取得有實際上主機的 2 KEY 筆數
                            "from shop_basic " +
                            "where keyin_day between @SearchStart and @SearchEnd AND keyin_flag='2' and sendhost_flag='Y' " +
                            "and member_service <> 'Y' " +
                            "GROUP BY KEYIN_FLAG " +
                            "order by KEYIN_FLAG ";

        //* 特店資料新增(6001)會員附加服務
        private const string SEL_SHOP_BASIC_M = @"select keyin_flag,count(recv_no) " + //取得所有的 1 KEY 筆數
                            "from shop_basic " +
                            "where keyin_day between @SearchStart and @SearchEnd AND keyin_flag='1' " +
                            "and member_service = 'Y' " +
                            "GROUP BY KEYIN_FLAG " +
                            "union " +
                            "select keyin_flag,count(recv_no) " +    //取得有實際上主機的 2 KEY 筆數
                            "from shop_basic " +
                            "where keyin_day between @SearchStart and @SearchEnd AND keyin_flag='2' and sendhost_flag='Y' " +
                            "and member_service = 'Y' " +
                            "GROUP BY KEYIN_FLAG " +
                            "order by KEYIN_FLAG ";

        //* 特店新增 PCAM
        private const string SEL_SHOP_PCAM = @"select keyin_flag,count(merchant_nbr) " + //取得所有的 1 KEY 筆數
                            "from shop_pcam " +
                            "where keyin_day between @SearchStart and @SearchEnd AND keyin_flag='1' " +
                            "GROUP BY KEYIN_FLAG " +
                            "union " +
                            "select keyin_flag,count(merchant_nbr) " +    //取得有實際上主機的 2 KEY 筆數
                            "from shop_pcam " +
                            "where keyin_day between @SearchStart and @SearchEnd AND keyin_flag='2' and send3270='Y' " +
                            "GROUP BY KEYIN_FLAG " +
                            "order by KEYIN_FLAG ";
        //* 特店新增(特店延伸性/請款加批)
        private const string SEL_SHOP_ReqAppro_I = @"select 'S' sTABLE,KEYIN_FLAG,count(shop_id) " +
                            "from SHOP " +
                            "where mod_date between @SearchStart and @SearchEnd and CHANGE_ITEM = '0' " +
                            "GROUP BY KEYIN_FLAG " +
                            "union " +
                            "select 'R' sTABLE,KEYIN_FLAG,count(shop_id) " +
                            "from SHOP_ReqAppro " +
                            "where mod_date between @SearchStart and @SearchEnd and CHANGE_ITEM = '0' " +
                            "GROUP BY KEYIN_FLAG " +
                            "order by KEYIN_FLAG ";
        #endregion

        #region 餘額轉置
        private const string SEL_BLANCE_Count = @"select 'Y' flag,COUNT(*) from dbo.Balance_Trans
                                                where convert(varchar,Trans_Date,112) between @SearchStart and @SearchEnd
                                                and Process_Flag = 'Y'
                                                union 
                                                select 'N' flag,COUNT(*) from dbo.Balance_Trans
                                                where convert(varchar,Trans_Date,112) between @SearchStart and @SearchEnd
                                                and Process_Flag = 'E' ";
        #endregion

        #region 毀補轉一卡通
        private const string SEL_iPASS = @"select '1' flag,COUNT(*) from dbo.iPASS
                                        where KeyIn_Flag = '1' and mod_date between @SearchStart and @SearchEnd
                                        union
                                        select '2' flag,COUNT(*) from dbo.iPASS
                                        where KeyIn_Flag = '2' and mod_date between @SearchStart and @SearchEnd";

        #endregion

        #endregion

        /// <summary>
        /// 取報表數據
        /// </summary>
        /// <param name="strSearchStart">區間起</param>
        /// <param name="strSearchEnd">區間迄</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>成功時：返回查詢結果；失敗時：null</returns>
        public static DataTable SearchRPTData(string strSearchStart,
                                string strSearchEnd, ref string strMsgID)
        {
            try
            {
                #region 生成要返回的DataTable結構
                DataTable dtblResult = new DataTable();
                //* 序號
                dtblResult.Columns.Add(new DataColumn("trans_name2", Type.GetType("System.String")));
                //* 作業日
                dtblResult.Columns.Add(new DataColumn("trans_name", Type.GetType("System.String")));
                //* 比對值(Key值)
                dtblResult.Columns.Add(new DataColumn("trans_1keysum", Type.GetType("System.Int32")));
                //* 鍵一同仁
                dtblResult.Columns.Add(new DataColumn("trans_sum", Type.GetType("System.Int32")));
                //* 鍵二同仁
                dtblResult.Columns.Add(new DataColumn("rpt_order", Type.GetType("System.Int32")));
                #endregion 生成要返回的DataTable結構

                #region 統計每一種作業業務量，并將查詢結果寫入dtblResult
                Int32 intCount = 0;
                Int32 intCountI = 0;

                StringBuilder sbSqlCommand = new StringBuilder();
                sbSqlCommand.Append(SEL_CARD_PERSON_ADDRESS);
                sbSqlCommand.Append(SEL_CARD_PERSON_NAME);
                sbSqlCommand.Append(SEL_CARD_PERSON_OTHER);

                SqlCommand sqlcmdSearch = new SqlCommand();
                sqlcmdSearch.CommandText = sbSqlCommand.ToString();
                sqlcmdSearch.CommandType = CommandType.Text;

                //* 區間起
                SqlParameter parmSearchStart = new SqlParameter(@"SearchStart", strSearchStart.Replace("/",""));
                sqlcmdSearch.Parameters.Add(parmSearchStart);
                //* 區間迄
                SqlParameter parmSearchEnd = new SqlParameter(@"SearchEnd", strSearchEnd.Replace("/", ""));
                sqlcmdSearch.Parameters.Add(parmSearchEnd);

                //* 執行查詢SQL語句
                DataSet dstSearchResult = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);

                //Add by LuYang 20120621
                DataSet dstSearchResult2 = null;

                if (null == dstSearchResult)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                #region 卡人資料
                //* 地址(卡人資料異動)
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[0].Rows.Count; intLoop++)
                {
                    intCount += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][3]);
                }
                DataRow drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCount;
                drowInsert["trans_name2"] = "卡人資料異動";
                drowInsert["trans_1keysum"] = 0;
                drowInsert["trans_name"] = "地址";
                drowInsert["rpt_order"] = 1;
                dtblResult.Rows.Add(drowInsert);

                //* 姓名(卡人資料異動)
                intCount = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[1].Rows.Count; intLoop++)
                {
                    intCount += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][3]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCount;
                drowInsert["trans_name2"] = "卡人資料異動";
                drowInsert["trans_1keysum"] = 0;
                drowInsert["trans_name"] = "姓名/生日";
                drowInsert["rpt_order"] = 2;
                dtblResult.Rows.Add(drowInsert);

                //* 其他(卡人資料異動)
                intCount = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[2].Rows.Count; intLoop++)
                {
                    intCount += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][3]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCount;
                drowInsert["trans_name2"] = "卡人資料異動";
                drowInsert["trans_1keysum"] = 0;
                drowInsert["trans_name"] = "其他";
                drowInsert["rpt_order"] = 3;
                dtblResult.Rows.Add(drowInsert);

                //*重新查詢
                intCount = 0;
                intCountI = 0;
                sbSqlCommand = new StringBuilder();
                sbSqlCommand.Append(SEL_CARD_PERSON_FAMILY);
                sbSqlCommand.Append(SEL_CARD_PERSON_OTHER_BANK);
                sbSqlCommand.Append(SEL_CARD_PERSON_CHINA_TRUST);
                sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                //* 執行查詢SQL語句
               dstSearchResult = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                //* 族群碼(卡人資料異動)
                intCount = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[0].Rows.Count; intLoop++)
                {
                    intCount += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][3]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCount;
                drowInsert["trans_name2"] = "卡人資料異動";
                drowInsert["trans_1keysum"] = 0;
                drowInsert["trans_name"] = "族群碼";
                drowInsert["rpt_order"] = 4;
                dtblResult.Rows.Add(drowInsert);

                //* 他行自扣(卡人資料異動)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[1].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[1].Rows[intLoop][0].ToString().Trim()=="1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][1]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "卡人資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "他行自扣申請書";
                drowInsert["rpt_order"] = 5;
                dtblResult.Rows.Add(drowInsert);

                //* 中信及郵局自扣(卡人資料異動)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[2].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[2].Rows[intLoop][0].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][1]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "卡人資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "中信及郵局自扣";
                drowInsert["rpt_order"] = 6;
                dtblResult.Rows.Add(drowInsert);

                //*重新查詢
                intCount = 0;
                intCountI = 0;
                sbSqlCommand = new StringBuilder();
                sbSqlCommand.Append(SEL_CARD_PERSON_MODIFY);
                sbSqlCommand.Append(SEL_CARD_UNREG);
                sbSqlCommand.Append(SEL_CARD_STATUS);
                sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                //* 執行查詢SQL語句
                dstSearchResult = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                //* 訊息/更正單自扣(卡人資料異動)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[0].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[0].Rows[intLoop][0].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][1]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "卡人資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "訊息/更正單自扣";
                drowInsert["rpt_order"] = 7;
                dtblResult.Rows.Add(drowInsert);
                #endregion

                #region 卡片資料異動
                //* 註銷(卡片資料異動)
                intCount = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[1].Rows.Count; intLoop++)
                {
                    intCount += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][3]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCount;
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = 0;
                drowInsert["trans_name"] = "註銷";
                drowInsert["rpt_order"] = 8;
                dtblResult.Rows.Add(drowInsert);

                //* 狀況碼(卡片資料異動)
                intCount = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[2].Rows.Count; intLoop++)
                {
                    intCount += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][3]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCount;
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = 0;
                drowInsert["trans_name"] = "狀況碼";
                drowInsert["rpt_order"] = 9;
                dtblResult.Rows.Add(drowInsert);


                //*重新查詢

                intCount = 0;
                intCountI = 0;

                sbSqlCommand = new StringBuilder();

                sbSqlCommand.Append(SEL_CARD_PERCENT);
                sbSqlCommand.Append(SEL_CARD_MONEY);
                sbSqlCommand.Append(SEL_CARD_MONEY_GRADE);

                sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                //* 執行查詢SQL語句
                dstSearchResult = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                //* 優惠碼(卡片資料異動)
                intCount = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[0].Rows.Count; intLoop++)
                {
                    intCount += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][3]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCount;
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = 0;
                drowInsert["trans_name"] = "優惠碼";
                drowInsert["rpt_order"] = 10;
                dtblResult.Rows.Add(drowInsert);

                //* 繳款異動(卡片資料異動)
                intCount = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[1].Rows.Count; intLoop++)
                {
                    intCount += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][3]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCount;
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = 0;
                drowInsert["trans_name"] = "繳款異動";
                drowInsert["rpt_order"] = 11;
                dtblResult.Rows.Add(drowInsert);

                //* 繳款評等(卡片資料異動)
                intCount = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[2].Rows.Count; intLoop++)
                {
                    intCount += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][3]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCount;
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = 0;
                drowInsert["trans_name"] = "繳款評等";
                drowInsert["rpt_order"] = 12;
                dtblResult.Rows.Add(drowInsert);

                //*重新查詢

                intCount = 0;
                intCountI = 0;


                sbSqlCommand = new StringBuilder();

                sbSqlCommand.Append(SEL_KEY2_SURPLY);
                sbSqlCommand.Append(SEL_KEY2_SURPLY_DESTORY);
                sbSqlCommand.Append(SEL_BLOCK_CODE_ADD);


                sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                //* 執行查詢SQL語句
                dstSearchResult = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                //* 掛補(二KEY)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[0].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[0].Rows[intLoop][0].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][1]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "掛補(二KEY)";
                drowInsert["rpt_order"] = 13;
                dtblResult.Rows.Add(drowInsert);

                //* 毀補(二KEY)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[1].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[1].Rows[intLoop][0].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][1]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "毀補(二KEY)";
                drowInsert["rpt_order"] = 14;
                dtblResult.Rows.Add(drowInsert);




                //* BlockCode新增/異動(卡片資料異動)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[2].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[2].Rows[intLoop][0].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][1]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "BLK CODE新增/異動(二KEY)";
                drowInsert["rpt_order"] = 15;
                dtblResult.Rows.Add(drowInsert);


                //*重新查詢

                intCount = 0;
                intCountI = 0;


                sbSqlCommand = new StringBuilder();

                sbSqlCommand.Append(SEL_BLOCK_CODE_NOT_CONTROL);
                sbSqlCommand.Append(SEL_SHOP_MODIFY);
                sbSqlCommand.Append(SEL_SHOP_RATE);


                sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                //* 執行查詢SQL語句
                dstSearchResult = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                //* BlockCode解除管制(卡片資料異動)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[0].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[0].Rows[intLoop][0].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][1]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "BLK CODE解除管制(二KEY)";
                drowInsert["rpt_order"] = 16;
                dtblResult.Rows.Add(drowInsert);
#endregion

                #region 特店資料異動
                //* 特店資料異動(資料異動)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[1].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[1].Rows[intLoop][1].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][2]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][2]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "特店資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "資料異動";
                drowInsert["rpt_order"] = 17;
                dtblResult.Rows.Add(drowInsert);

                
                sbSqlCommand = new StringBuilder();
                sbSqlCommand.Append(SEL_SHOP_ReqAppro_U);
                sbSqlCommand.Append(SEL_SHOP_ReqAppro_I);
                sqlcmdSearch.CommandText = sbSqlCommand.ToString();
                Logging.Log(sbSqlCommand.ToString(), LogLayer.BusinessRule);
                dstSearchResult2 = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult2)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                //* 特店資料異動(資料異動/請款加批)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult2.Tables[0].Rows.Count; intLoop++)
                {
                    if (dstSearchResult2.Tables[0].Rows[intLoop][1].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult2.Tables[0].Rows[intLoop][2]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult2.Tables[0].Rows[intLoop][2]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "特店資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "資料異動/請款加批";
                drowInsert["rpt_order"] = 18;
                dtblResult.Rows.Add(drowInsert);


                //* 特店資料異動(費率)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[2].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[2].Rows[intLoop][1].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][2]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][2]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "特店資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "費率";
                drowInsert["rpt_order"] = 19;
                dtblResult.Rows.Add(drowInsert);

                //*重新查詢

                intCount = 0;
                intCountI = 0;


                sbSqlCommand = new StringBuilder();

                sbSqlCommand.Append(SEL_SHOP_ACCOUNT);
                sbSqlCommand.Append(SEL_SHOP_FIRE);
                sbSqlCommand.Append(SEL_SHOP_MACHINE);


                sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                //* 執行查詢SQL語句
                dstSearchResult = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                //* 特店資料異動(帳號)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[0].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[0].Rows[intLoop][1].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][2]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][2]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "特店資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "帳號";
                drowInsert["rpt_order"] = 20;
                dtblResult.Rows.Add(drowInsert);

                //* 特店資料異動(解約)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[1].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[1].Rows[intLoop][1].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][2]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][2]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "特店資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "解約";
                drowInsert["rpt_order"] = 21;
                dtblResult.Rows.Add(drowInsert);

                //* 特店資料異動(機器)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[2].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[2].Rows[intLoop][1].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][2]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][2]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "特店資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "機器";
                drowInsert["rpt_order"] = 22;
                dtblResult.Rows.Add(drowInsert);

                //*重新查詢

                sbSqlCommand = new StringBuilder();
                sbSqlCommand.Append(SEL_SHOP_BASIC);
                sbSqlCommand.Append(SEL_SHOP_BASIC_M);
                sbSqlCommand.Append(SEL_SHOP_PCAM);
                sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                //* 執行查詢SQL語句
                dstSearchResult = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                //* 特店新增 6001
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[0].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[0].Rows[intLoop][0].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][1]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "特店資料新增";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "6001";
                drowInsert["rpt_order"] = 23;
                dtblResult.Rows.Add(drowInsert);

                //* 特店資料新增(6001)會員附加服務
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[1].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[1].Rows[intLoop][0].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][1]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "特店資料新增";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "6001會員附加服務";
                drowInsert["rpt_order"] = 24;
                dtblResult.Rows.Add(drowInsert);

                //* 特店新增 PCAM
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[2].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[2].Rows[intLoop][0].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][1]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "特店資料新增";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "PCAM";
                drowInsert["rpt_order"] = 25;
                dtblResult.Rows.Add(drowInsert);

                //* 特店資料新增(特店延伸性/請款加批)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult2.Tables[1].Rows.Count; intLoop++)
                {
                    if (dstSearchResult2.Tables[1].Rows[intLoop][1].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult2.Tables[1].Rows[intLoop][2]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult2.Tables[1].Rows[intLoop][2]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "特店資料新增";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "特店延伸性/請款加批";
                drowInsert["rpt_order"] = 26;
                dtblResult.Rows.Add(drowInsert);
                #endregion

                #region 2014/10/29 新增"卡片資料異動"."餘額轉置"統計項目,統計餘額轉置輸入之資料筆數
                sbSqlCommand = new StringBuilder();
                sbSqlCommand.Append(SEL_BLANCE_Count);
                sqlcmdSearch.CommandText = sbSqlCommand.ToString();
                sqlcmdSearch.CommandType = CommandType.Text;

                dstSearchResult = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                intCount = 0;
                intCountI = 0;
                for (int i = 0; i < dstSearchResult.Tables[0].Rows.Count; i++)
                {
                    if (dstSearchResult.Tables[0].Rows[i][0].ToString().Trim() == "Y")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[0].Rows[i][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[0].Rows[i][1]);
                }

                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "餘額轉置";
                drowInsert["rpt_order"] = 27;
                dtblResult.Rows.Add(drowInsert);
                #endregion

                #region 2014/12/09 毀補轉一卡通

                sbSqlCommand = new StringBuilder();
                sbSqlCommand.Append(SEL_iPASS);
                sqlcmdSearch.CommandText = sbSqlCommand.ToString();
                sqlcmdSearch.CommandType = CommandType.Text;

                dstSearchResult = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                intCount = 0;
                intCountI = 0;
                for (int i = 0; i < dstSearchResult.Tables[0].Rows.Count; i++)
                {
                    if (dstSearchResult.Tables[0].Rows[i][0].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[0].Rows[i][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[0].Rows[i][1]);
                }

                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;    
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = intCount;    
                drowInsert["trans_name"] = "毀補轉一卡通";
                drowInsert["rpt_order"] = 28;
                dtblResult.Rows.Add(drowInsert);
                #endregion

                return dtblResult;

                #endregion 依據Request查詢資料庫，將查詢結果寫入dtblResult
            }
            catch (Exception exp)
            {
                BRCompareRpt.SaveLog(exp);
                strMsgID = "01_03020000_002";
                return null;
            }
        }


        /// <summary>
        /// 建立日期:2021/01/19_Ares_Stanley
        /// 取報表數據 for GridView
        /// 修改紀錄:
        /// </summary>
        /// <param name="strSearchStart">區間起</param>
        /// <param name="strSearchEnd">區間迄</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>成功時：返回查詢結果；失敗時：null</returns>
        public static DataTable SearchRPTData(string strSearchStart,
                                string strSearchEnd, ref string strMsgID, int iPageIndex, int iPageSize, ref int iTotalCount)
        {
            try
            {
                #region 生成要返回的DataTable結構
                DataTable dtblResult = new DataTable();
                //* 序號
                dtblResult.Columns.Add(new DataColumn("trans_name2", Type.GetType("System.String")));
                //* 作業日
                dtblResult.Columns.Add(new DataColumn("trans_name", Type.GetType("System.String")));
                //* 比對值(Key值)
                dtblResult.Columns.Add(new DataColumn("trans_1keysum", Type.GetType("System.Int32")));
                //* 鍵一同仁
                dtblResult.Columns.Add(new DataColumn("trans_sum", Type.GetType("System.Int32")));
                //* 鍵二同仁
                dtblResult.Columns.Add(new DataColumn("rpt_order", Type.GetType("System.Int32")));
                #endregion 生成要返回的DataTable結構

                #region 統計每一種作業業務量，并將查詢結果寫入dtblResult
                Int32 intCount = 0;
                Int32 intCountI = 0;

                StringBuilder sbSqlCommand = new StringBuilder();
                sbSqlCommand.Append(SEL_CARD_PERSON_ADDRESS);
                sbSqlCommand.Append(SEL_CARD_PERSON_NAME);
                sbSqlCommand.Append(SEL_CARD_PERSON_OTHER);

                SqlCommand sqlcmdSearch = new SqlCommand();
                sqlcmdSearch.CommandText = sbSqlCommand.ToString();
                sqlcmdSearch.CommandType = CommandType.Text;

                //* 區間起
                SqlParameter parmSearchStart = new SqlParameter(@"SearchStart", strSearchStart.Replace("/", ""));
                sqlcmdSearch.Parameters.Add(parmSearchStart);
                //* 區間迄
                SqlParameter parmSearchEnd = new SqlParameter(@"SearchEnd", strSearchEnd.Replace("/", ""));
                sqlcmdSearch.Parameters.Add(parmSearchEnd);

                //* 執行查詢SQL語句
                DataSet dstSearchResult = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);

                //Add by LuYang 20120621
                DataSet dstSearchResult2 = null;

                if (null == dstSearchResult)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                #region 卡人資料
                //* 地址(卡人資料異動)
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[0].Rows.Count; intLoop++)
                {
                    intCount += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][3]);
                }
                DataRow drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCount;
                drowInsert["trans_name2"] = "卡人資料異動";
                drowInsert["trans_1keysum"] = 0;
                drowInsert["trans_name"] = "地址";
                drowInsert["rpt_order"] = 1;
                dtblResult.Rows.Add(drowInsert);

                //* 姓名(卡人資料異動)
                intCount = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[1].Rows.Count; intLoop++)
                {
                    intCount += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][3]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCount;
                drowInsert["trans_name2"] = "卡人資料異動";
                drowInsert["trans_1keysum"] = 0;
                drowInsert["trans_name"] = "姓名/生日";
                drowInsert["rpt_order"] = 2;
                dtblResult.Rows.Add(drowInsert);

                //* 其他(卡人資料異動)
                intCount = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[2].Rows.Count; intLoop++)
                {
                    intCount += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][3]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCount;
                drowInsert["trans_name2"] = "卡人資料異動";
                drowInsert["trans_1keysum"] = 0;
                drowInsert["trans_name"] = "其他";
                drowInsert["rpt_order"] = 3;
                dtblResult.Rows.Add(drowInsert);

                //*重新查詢
                intCount = 0;
                intCountI = 0;
                sbSqlCommand = new StringBuilder();
                sbSqlCommand.Append(SEL_CARD_PERSON_FAMILY);
                sbSqlCommand.Append(SEL_CARD_PERSON_OTHER_BANK);
                sbSqlCommand.Append(SEL_CARD_PERSON_CHINA_TRUST);
                sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                //* 執行查詢SQL語句
                dstSearchResult = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                //* 族群碼(卡人資料異動)
                intCount = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[0].Rows.Count; intLoop++)
                {
                    intCount += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][3]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCount;
                drowInsert["trans_name2"] = "卡人資料異動";
                drowInsert["trans_1keysum"] = 0;
                drowInsert["trans_name"] = "族群碼";
                drowInsert["rpt_order"] = 4;
                dtblResult.Rows.Add(drowInsert);

                //* 他行自扣(卡人資料異動)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[1].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[1].Rows[intLoop][0].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][1]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "卡人資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "他行自扣申請書";
                drowInsert["rpt_order"] = 5;
                dtblResult.Rows.Add(drowInsert);

                //* 中信及郵局自扣(卡人資料異動)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[2].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[2].Rows[intLoop][0].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][1]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "卡人資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "中信及郵局自扣";
                drowInsert["rpt_order"] = 6;
                dtblResult.Rows.Add(drowInsert);

                //*重新查詢
                intCount = 0;
                intCountI = 0;
                sbSqlCommand = new StringBuilder();
                sbSqlCommand.Append(SEL_CARD_PERSON_MODIFY);
                sbSqlCommand.Append(SEL_CARD_UNREG);
                sbSqlCommand.Append(SEL_CARD_STATUS);
                sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                //* 執行查詢SQL語句
                dstSearchResult = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                //* 訊息/更正單自扣(卡人資料異動)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[0].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[0].Rows[intLoop][0].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][1]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "卡人資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "訊息/更正單自扣";
                drowInsert["rpt_order"] = 7;
                dtblResult.Rows.Add(drowInsert);
                #endregion

                #region 卡片資料異動
                //* 註銷(卡片資料異動)
                intCount = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[1].Rows.Count; intLoop++)
                {
                    intCount += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][3]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCount;
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = 0;
                drowInsert["trans_name"] = "註銷";
                drowInsert["rpt_order"] = 8;
                dtblResult.Rows.Add(drowInsert);

                //* 狀況碼(卡片資料異動)
                intCount = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[2].Rows.Count; intLoop++)
                {
                    intCount += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][3]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCount;
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = 0;
                drowInsert["trans_name"] = "狀況碼";
                drowInsert["rpt_order"] = 9;
                dtblResult.Rows.Add(drowInsert);


                //*重新查詢

                intCount = 0;
                intCountI = 0;

                sbSqlCommand = new StringBuilder();

                sbSqlCommand.Append(SEL_CARD_PERCENT);
                sbSqlCommand.Append(SEL_CARD_MONEY);
                sbSqlCommand.Append(SEL_CARD_MONEY_GRADE);

                sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                //* 執行查詢SQL語句
                dstSearchResult = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                //* 優惠碼(卡片資料異動)
                intCount = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[0].Rows.Count; intLoop++)
                {
                    intCount += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][3]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCount;
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = 0;
                drowInsert["trans_name"] = "優惠碼";
                drowInsert["rpt_order"] = 10;
                dtblResult.Rows.Add(drowInsert);

                //* 繳款異動(卡片資料異動)
                intCount = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[1].Rows.Count; intLoop++)
                {
                    intCount += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][3]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCount;
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = 0;
                drowInsert["trans_name"] = "繳款異動";
                drowInsert["rpt_order"] = 11;
                dtblResult.Rows.Add(drowInsert);

                //* 繳款評等(卡片資料異動)
                intCount = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[2].Rows.Count; intLoop++)
                {
                    intCount += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][3]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCount;
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = 0;
                drowInsert["trans_name"] = "繳款評等";
                drowInsert["rpt_order"] = 12;
                dtblResult.Rows.Add(drowInsert);

                //*重新查詢

                intCount = 0;
                intCountI = 0;


                sbSqlCommand = new StringBuilder();

                sbSqlCommand.Append(SEL_KEY2_SURPLY);
                sbSqlCommand.Append(SEL_KEY2_SURPLY_DESTORY);
                sbSqlCommand.Append(SEL_BLOCK_CODE_ADD);


                sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                //* 執行查詢SQL語句
                dstSearchResult = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                //* 掛補(二KEY)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[0].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[0].Rows[intLoop][0].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][1]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "掛補(二KEY)";
                drowInsert["rpt_order"] = 13;
                dtblResult.Rows.Add(drowInsert);

                //* 毀補(二KEY)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[1].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[1].Rows[intLoop][0].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][1]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "毀補(二KEY)";
                drowInsert["rpt_order"] = 14;
                dtblResult.Rows.Add(drowInsert);




                //* BlockCode新增/異動(卡片資料異動)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[2].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[2].Rows[intLoop][0].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][1]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "BLK CODE新增/異動(二KEY)";
                drowInsert["rpt_order"] = 15;
                dtblResult.Rows.Add(drowInsert);


                //*重新查詢

                intCount = 0;
                intCountI = 0;


                sbSqlCommand = new StringBuilder();

                sbSqlCommand.Append(SEL_BLOCK_CODE_NOT_CONTROL);
                sbSqlCommand.Append(SEL_SHOP_MODIFY);
                sbSqlCommand.Append(SEL_SHOP_RATE);


                sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                //* 執行查詢SQL語句
                dstSearchResult = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                //* BlockCode解除管制(卡片資料異動)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[0].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[0].Rows[intLoop][0].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][1]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "BLK CODE解除管制(二KEY)";
                drowInsert["rpt_order"] = 16;
                dtblResult.Rows.Add(drowInsert);
                #endregion

                #region 特店資料異動
                //* 特店資料異動(資料異動)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[1].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[1].Rows[intLoop][1].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][2]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][2]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "特店資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "資料異動";
                drowInsert["rpt_order"] = 17;
                dtblResult.Rows.Add(drowInsert);


                sbSqlCommand = new StringBuilder();
                sbSqlCommand.Append(SEL_SHOP_ReqAppro_U);
                sbSqlCommand.Append(SEL_SHOP_ReqAppro_I);
                sqlcmdSearch.CommandText = sbSqlCommand.ToString();
                Logging.Log(sbSqlCommand.ToString(), LogLayer.BusinessRule);
                dstSearchResult2 = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult2)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                //* 特店資料異動(資料異動/請款加批)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult2.Tables[0].Rows.Count; intLoop++)
                {
                    if (dstSearchResult2.Tables[0].Rows[intLoop][1].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult2.Tables[0].Rows[intLoop][2]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult2.Tables[0].Rows[intLoop][2]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "特店資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "資料異動/請款加批";
                drowInsert["rpt_order"] = 18;
                dtblResult.Rows.Add(drowInsert);


                //* 特店資料異動(費率)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[2].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[2].Rows[intLoop][1].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][2]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][2]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "特店資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "費率";
                drowInsert["rpt_order"] = 19;
                dtblResult.Rows.Add(drowInsert);

                //*重新查詢

                intCount = 0;
                intCountI = 0;


                sbSqlCommand = new StringBuilder();

                sbSqlCommand.Append(SEL_SHOP_ACCOUNT);
                sbSqlCommand.Append(SEL_SHOP_FIRE);
                sbSqlCommand.Append(SEL_SHOP_MACHINE);


                sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                //* 執行查詢SQL語句
                dstSearchResult = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                //* 特店資料異動(帳號)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[0].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[0].Rows[intLoop][1].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][2]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][2]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "特店資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "帳號";
                drowInsert["rpt_order"] = 20;
                dtblResult.Rows.Add(drowInsert);

                //* 特店資料異動(解約)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[1].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[1].Rows[intLoop][1].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][2]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][2]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "特店資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "解約";
                drowInsert["rpt_order"] = 21;
                dtblResult.Rows.Add(drowInsert);

                //* 特店資料異動(機器)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[2].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[2].Rows[intLoop][1].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][2]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][2]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "特店資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "機器";
                drowInsert["rpt_order"] = 22;
                dtblResult.Rows.Add(drowInsert);

                //*重新查詢

                sbSqlCommand = new StringBuilder();
                sbSqlCommand.Append(SEL_SHOP_BASIC);
                sbSqlCommand.Append(SEL_SHOP_BASIC_M);
                sbSqlCommand.Append(SEL_SHOP_PCAM);
                sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                //* 執行查詢SQL語句
                dstSearchResult = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                //* 特店新增 6001
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[0].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[0].Rows[intLoop][0].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[0].Rows[intLoop][1]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "特店資料新增";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "6001";
                drowInsert["rpt_order"] = 23;
                dtblResult.Rows.Add(drowInsert);

                //* 特店資料新增(6001)會員附加服務
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[1].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[1].Rows[intLoop][0].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[1].Rows[intLoop][1]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "特店資料新增";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "6001會員附加服務";
                drowInsert["rpt_order"] = 24;
                dtblResult.Rows.Add(drowInsert);

                //* 特店新增 PCAM
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[2].Rows.Count; intLoop++)
                {
                    if (dstSearchResult.Tables[2].Rows[intLoop][0].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[2].Rows[intLoop][1]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "特店資料新增";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "PCAM";
                drowInsert["rpt_order"] = 25;
                dtblResult.Rows.Add(drowInsert);

                //* 特店資料新增(特店延伸性/請款加批)
                intCount = 0;
                intCountI = 0;
                for (int intLoop = 0; intLoop < dstSearchResult2.Tables[1].Rows.Count; intLoop++)
                {
                    if (dstSearchResult2.Tables[1].Rows[intLoop][1].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult2.Tables[1].Rows[intLoop][2]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult2.Tables[1].Rows[intLoop][2]);
                }
                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "特店資料新增";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "特店延伸性/請款加批";
                drowInsert["rpt_order"] = 26;
                dtblResult.Rows.Add(drowInsert);
                #endregion

                #region 2014/10/29 新增"卡片資料異動"."餘額轉置"統計項目,統計餘額轉置輸入之資料筆數
                sbSqlCommand = new StringBuilder();
                sbSqlCommand.Append(SEL_BLANCE_Count);
                sqlcmdSearch.CommandText = sbSqlCommand.ToString();
                sqlcmdSearch.CommandType = CommandType.Text;

                dstSearchResult = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                intCount = 0;
                intCountI = 0;
                for (int i = 0; i < dstSearchResult.Tables[0].Rows.Count; i++)
                {
                    if (dstSearchResult.Tables[0].Rows[i][0].ToString().Trim() == "Y")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[0].Rows[i][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[0].Rows[i][1]);
                }

                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "餘額轉置";
                drowInsert["rpt_order"] = 27;
                dtblResult.Rows.Add(drowInsert);
                #endregion

                #region 2014/12/09 毀補轉一卡通

                sbSqlCommand = new StringBuilder();
                sbSqlCommand.Append(SEL_iPASS);
                sqlcmdSearch.CommandText = sbSqlCommand.ToString();
                sqlcmdSearch.CommandType = CommandType.Text;

                dstSearchResult = BRStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult)
                {
                    strMsgID = "01_03020000_002";
                    return null;
                }

                intCount = 0;
                intCountI = 0;
                for (int i = 0; i < dstSearchResult.Tables[0].Rows.Count; i++)
                {
                    if (dstSearchResult.Tables[0].Rows[i][0].ToString().Trim() == "1")
                        intCount += Convert.ToInt32(dstSearchResult.Tables[0].Rows[i][1]);
                    else
                        intCountI += Convert.ToInt32(dstSearchResult.Tables[0].Rows[i][1]);
                }

                drowInsert = dtblResult.NewRow();
                drowInsert["trans_sum"] = intCountI;
                drowInsert["trans_name2"] = "卡片資料異動";
                drowInsert["trans_1keysum"] = intCount;
                drowInsert["trans_name"] = "毀補轉一卡通";
                drowInsert["rpt_order"] = 28;
                dtblResult.Rows.Add(drowInsert);
                #endregion
                iTotalCount = dtblResult.Rows.Count;
                int totalRows = dtblResult.Rows.Count;
                int currentRows = 0;
                // 新增總計欄
                dtblResult.Columns.Add("12KEY_SUM", typeof(int));
                // 計算總計欄
                for (int i = 0; i < dtblResult.Rows.Count; i++)
                {
                    dtblResult.Rows[i]["12KEY_SUM"] = (int.Parse(dtblResult.Rows[i][2].ToString()) + int.Parse(dtblResult.Rows[i][3].ToString()));
                }
                // 移除空值欄
                for (int i = 0; i < totalRows; i++)
                {
                    if(dtblResult.Rows[currentRows][2].ToString()=="0" && dtblResult.Rows[currentRows][3].ToString() == "0")
                    {
                        dtblResult.Rows.Remove(dtblResult.Rows[0]);
                        iTotalCount = dtblResult.Rows.Count;
                    }
                }
                // 頁數計算
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
                strMsgID = "01_03020000_002";
                return null;
            }
        }
    }
}
