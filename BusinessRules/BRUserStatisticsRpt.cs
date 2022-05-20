//******************************************************************
//*  作    者：占偉林(James)
//*  功能說明：經辦作業量統計
//*  創建日期：2009/11/12
//*  修改記錄：
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*  Ares Luke          2020/12/21         20200031-CSIP EOS       新增GridView查詢功能
//*******************************************************************
using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using CSIPKeyInGUI.EntityLayer;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Logging;
using Framework.Common.Utility;
using System.Diagnostics;

namespace CSIPKeyInGUI.BusinessRules
{
    /// <summary>
    /// 經辦作業量統計報表業務類
    /// </summary>
    public class BRUserStatisticsRpt : CSIPCommonModel.BusinessRules.BRBase<EntityCUSTOMER_LOG>
    {
        #region SQL 語句
        //* 經辦訊息
        //2021/03/17_Ares_Stanley-DB名稱改為變數
        private const string SEL_USER_STATISTICS = @"SELECT A.user_id user_id,B.USER_NAME " +
                "FROM customer_log A,(select distinct user_id,User_name from {0}.dbo.M_USER) as  B " +
                "where A.mod_date between @SearchStart and @SearchEnd " +
                    "AND A.user_id = B.USER_ID " +
                    "AND A.trans_id IN ('01010100', 'A01','01010400','A04','A03','01010200'," +
                        "'A11','01010300','A06','01020100','B01','01020200','B05','01020500'," +
                        "'B04','01020300','B02','01020400') " +
                "UNION " +
                "SELECT C.user_id user_id,D.USER_NAME " +
                "FROM AUTO_PAY C,(select distinct user_id,User_name from {0}.dbo.M_USER) as  D " +
                "where C.mod_date between @SearchStart and @SearchEnd " +
                    "AND C.user_id = D.USER_ID " +
                "UNION " +
                "SELECT M.user_id user_id,N.USER_NAME " +
                "FROM Other_Bank_Temp M,(select distinct user_id,User_name from {0}.dbo.M_USER) as N " +
                "where ( M.KeyIn_Flag='1' or M.KeyIn_Flag='2' ) and M.mod_date between @SearchStart and @SearchEnd " +
                    "AND M.user_id = N.USER_ID " +
                "UNION " +
                "SELECT E.user_id user_id,F.USER_NAME " +
                "FROM ReIssue_Card E,(select distinct user_id,User_name from {0}.dbo.M_USER) as  F " +
                "where E.mod_date between @SearchStart and @SearchEnd " +
                    "AND E.user_id = F.USER_ID " +
                "UNION " +
                "SELECT G.user_id user_id,H.USER_NAME " +
                "FROM Change_BLK G,(select distinct user_id,User_name from {0}.dbo.M_USER) as  H " +
                "where G.mod_date between @SearchStart and @SearchEnd " +
                    "AND G.user_id = H.USER_ID " +
                "UNION " +
                "SELECT I.user_id user_id,J.USER_NAME " +
                "FROM shop I,(select distinct user_id,User_name from {0}.dbo.M_USER) as  J " +
                "where I.mod_date between @SearchStart and @SearchEnd " +
                    "AND I.user_id = J.USER_ID " +
                "UNION " +
                "SELECT K.user_id user_id,L.USER_NAME " +
                "FROM shop_Upload K,(select distinct user_id,User_name from {0}.dbo.M_USER) as  L " +
                "where K.mod_date between @SearchStart and @SearchEnd " +
                    "AND K.user_id = L.USER_ID " +
                "UNION " +
                "SELECT M.keyin_userid user_id,N.USER_NAME " +
                "FROM shop_basic M,(select distinct user_id,User_name from {0}.dbo.M_USER) as  N " +
                "where M.keyin_day between @SearchStart and @SearchEnd " +
                    "AND M.keyin_userid = N.USER_ID " +
                "UNION " +
                "SELECT O.keyin_userid user_id,P.USER_NAME " +
                "FROM shop_pcam O,(select distinct user_id,User_name from {0}.dbo.M_USER) as  P " +
                "where O.keyin_day between @SearchStart and @SearchEnd " +
                    "AND O.keyin_userid = P.USER_ID " +
                "UNION " +
                "SELECT q.Modify_User user_id,r.USER_NAME " +
                "FROM Balance_Trans q,(select distinct user_id,User_name from {0}.dbo.M_USER) as  r " +
                "where q.Trans_Date between @SearchStart and @SearchEnd " +
                    "AND q.Modify_User = r.USER_ID " +
                "UNION " +
                "SELECT s.user_id user_id,t.USER_NAME " +
                "FROM iPASS s,(select distinct user_id,User_name from {0}.dbo.M_USER) as t " +
                "where s.mod_date between @SearchStart and @SearchEnd " +
                    "AND s.user_id = t.USER_ID " +
                "ORDER BY user_id";

        #region 卡人資料異動
        //* 地址（卡人資料異動）
        private const string SEL_CARD_PERSON_ADDRESS = @"select trans_id,mod_date,substring(mod_time,1,4),count(distinct(query_key)) " +
                "from customer_log " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and user_id = @user_id " +
                    "and trans_id in ('01010100','A01') " +
                "group by trans_id,mod_date,substring(mod_time,1,4) ";
        //* 姓名（卡人資料異動）
        private const string SEL_CARD_PERSON_NAME = @"select trans_id,mod_date,substring(mod_time,1,4),count(distinct(query_key)) " +
                "from customer_log " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and user_id = @user_id " +
                    "and trans_id in ('01010400','A04','A03') " +
                "group by trans_id,mod_date,substring(mod_time,1,4) ";
        //* 其他（卡人資料異動）
        private const string SEL_CARD_PERSON_OTHER = @"select trans_id,mod_date,substring(mod_time,1,4),count(distinct(query_key)) " +
                "from customer_log " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and user_id = @user_id " +
                    "and trans_id in ('01010200','A11') " +
                "group by trans_id,mod_date,substring(mod_time,1,4) ";
        //* 族群碼（卡人資料異動）
        private const string SEL_CARD_PERSON_FAMILY = @"select trans_id,mod_date,substring(mod_time,1,4),count(distinct(query_key)) " +
                "from customer_log " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and user_id = @user_id " +
                    "and trans_id in ('01010300','A06') " +
                "group by trans_id,mod_date,substring(mod_time,1,4) ";
        //* 他行自扣一Key、二Key（卡人資料異動）
        private const string SEL_CARD_PERSON_KEY = @"select keyin_flag,count(cus_id) " +
                "from other_bank_temp " +
                "where ( KeyIn_Flag='1' or KeyIn_Flag='2' ) and mod_date between @SearchStart and @SearchEnd " +
                    "and user_id = @user_id " +
                "group by keyin_flag ";
        //* 中信及郵局自扣一Key、二Key（卡人資料異動）
        private const string SEL_CARD_PERSON_CHINA_TRUST = @"select keyin_flag,count(cus_id) " +
                "from auto_pay " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and user_id = @user_id and Add_Flag = '0' " +
                "group by keyin_flag ";
        //* 訊息/更正單自扣一Key、二Key（卡人資料異動）,sql 查詢語句
        private const string SEL_CARD_PERSON_MODIFY = @"select keyin_flag,count(cus_id) " +
                "from auto_pay " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and user_id = @user_id and Add_Flag='1' " +
                "group by keyin_flag ";
        #endregion

        #region 卡片資料異動
        //* 註銷（卡片資料異動）,sql 查詢語句
        private const string SEL_CARD_UNREG = @"select trans_id,mod_date,substring(mod_time,1,4),count(distinct(query_key)) " +
                "from customer_log " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and user_id = @user_id " +
                    "and trans_id in ('01020100','B01') " +
                "group by trans_id,mod_date,substring(mod_time,1,4) ";
        //* 狀況碼（卡片資料異動）,sql 查詢語句
        private const string SEL_CARD_STATUS = @"select trans_id,mod_date,substring(mod_time,1,4),count(distinct(query_key)) " +
                "from customer_log " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and user_id = @user_id " +
                    "and trans_id in ('01020200','B05') " +
                "group by trans_id,mod_date,substring(mod_time,1,4) ";
        //* 優惠碼（卡片資料異動）,sql 查詢語句
        private const string SEL_CARD_PERCENT = @"select trans_id,mod_date,substring(mod_time,1,4),count(distinct(query_key)) " +
                "from customer_log " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and user_id = @user_id " +
                    "and trans_id in ('01020500','B04') " +
                "group by trans_id,mod_date,substring(mod_time,1,4) ";
        //* 繳款異動（卡片資料異動）,sql 查詢語句
        private const string SEL_CARD_MONEY = @"select trans_id,mod_date,substring(mod_time,1,4),count(distinct(query_key)) " +
                "from customer_log " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and user_id = @user_id " +
                    "and trans_id in ('01020300','B02') " +
                "group by trans_id,mod_date,substring(mod_time,1,4) ";
        //* 繳款評等（卡片資料異動）,sql 查詢語句
        private const string SEL_CARD_MONEY_GRADE = @"select trans_id,mod_date,substring(mod_time,1,4),count(distinct(query_key)) " +
                "from customer_log " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and user_id = @user_id " +
                    "and trans_id = '01020400' " +
                "group by trans_id,mod_date,substring(mod_time,1,4) ";

        #region 2014/12/12 一卡通
        private const string SEL_iPASS = @"select KeyIn_Flag,COUNT(Card_No) from dbo.iPASS 
                                        where mod_date between @SearchStart and @SearchEnd and user_id = @user_id
                                        group by KeyIn_Flag ";

        #endregion

        //* 毀補一Key、二Key（卡片資料異動）,sql 查詢語句
        private const string SEL_KEY2_SURPLY_DESTORY = @"select keyin_flag,count(card_no) " +
                "from ReIssue_Card " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and user_id = @user_id " +
                    "and ReIssue_Type = '2' " +
                "group by keyin_flag ";
        //* 掛補一Key、二Key（卡片資料異動）,sql 查詢語句
        private const string SEL_KEY2_SURPLY = @"select keyin_flag,count(card_no) " +
                "from ReIssue_Card " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and user_id = @user_id " +
                    "and ReIssue_Type = '1' " +
                "group by keyin_flag ";
        //* BLKCODE 新增/異動一Key、二Key（卡片資料異動）,sql 查詢語句
        private const string SEL_BLOCK_CODE_ADD = @"select keyin_flag,count(card_no) " +
                "from Change_BLK " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and user_id = @user_id and Change_Type='C' " +
                "group by keyin_flag ";
        //* BlockCode解除管制(卡片資料異動)
        private const string SEL_BLOCK_CODE_NOT_CONTROL = @"select keyin_flag,count(card_no) " +
                "from Change_BLK " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and user_id = @user_id and Change_Type='D' " +
                "group by keyin_flag ";
        #endregion

        #region 特店資料異動
        //* 特店資料異動(資料異動)
        private const string SEL_SHOP_MODIFY = @"select KEYIN_FLAG,count(shop_id),'S' sTABLE " +
                "from SHOP " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and CHANGE_ITEM = '1' " +
                    "and user_id = @user_id " +
                "GROUP BY KEYIN_FLAG " +
                "union " +
                "select KEYIN_FLAG,count(shop_id),'U' sTABLE from SHOP_UPLOAD " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and CHANGE_ITEM = '1' " +
                    "and user_id = @user_id " +
                "GROUP BY KEYIN_FLAG " +
                "order by KEYIN_FLAG ";
        //* 費率一Key、二Key（特店資料異動）,sql 查詢語句
        private const string SEL_SHOP_RATE = @"select KEYIN_FLAG,count(shop_id),'S' sTABLE " +
                "from SHOP " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and CHANGE_ITEM = '2' " +
                    "and user_id = @user_id " +
                "GROUP BY KEYIN_FLAG " +
                "union " +
                "select KEYIN_FLAG,count(shop_id),'U' sTABLE from SHOP_UPLOAD " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and CHANGE_ITEM = '2' " +
                    "and user_id = @user_id " +
                "GROUP BY KEYIN_FLAG " +
                "order by KEYIN_FLAG ";
        //* 帳號一Key、二Key（特店資料異動）,sql 查詢語句
        private const string SEL_SHOP_ACCOUNT = @"select KEYIN_FLAG,count(shop_id),'S' sTABLE " +
                "from SHOP " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and CHANGE_ITEM = '3' " +
                    "and user_id = @user_id " +
                "GROUP BY KEYIN_FLAG " +
                "union " +
                "select KEYIN_FLAG,count(shop_id),'U' sTABLE from SHOP_UPLOAD " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and CHANGE_ITEM = '3' " +
                    "and user_id = @user_id " +
                "GROUP BY KEYIN_FLAG " +
                "order by KEYIN_FLAG ";
        //* 解約一Key、二Key（特店資料異動）,sql 查詢語句
        private const string SEL_SHOP_FIRE = @"select KEYIN_FLAG,count(shop_id),'S' sTABLE " +
                "from SHOP " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and CHANGE_ITEM = '4' " +
                    "and user_id = @user_id " +
                "GROUP BY KEYIN_FLAG " +
                "union " +
                "select KEYIN_FLAG,count(shop_id),'U' sTABLE from SHOP_UPLOAD " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and CHANGE_ITEM = '4' " +
                    "and user_id = @user_id " +
                "GROUP BY KEYIN_FLAG " +
                "order by KEYIN_FLAG ";
        //* 機器一Key、二Key（特店資料異動）,sql 查詢語句
        private const string SEL_SHOP_MACHINE = @"select KEYIN_FLAG,count(shop_id),'S' sTABLE " +
                "from SHOP " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and CHANGE_ITEM = '5' " +
                    "and user_id = @user_id " +
                "GROUP BY KEYIN_FLAG " +
                "union " +
                "select KEYIN_FLAG,count(shop_id),'U' sTABLE from SHOP_UPLOAD " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and CHANGE_ITEM = '5' " +
                    "and user_id = @user_id " +
                "GROUP BY KEYIN_FLAG " +
                "order by KEYIN_FLAG ";

        //*特店資料新增(6001) ,sql 查詢語句
        private const string SEL_SHOP_BASIC = @"select KeyIn_Flag,count(uni_no1+'-'+uni_no2) " +
                "from shop_basic " +
                "where keyin_day between @SearchStart and @SearchEnd " +
                    "and keyin_userid = @user_id " +
                "group by KeyIn_Flag ";

        //*特店資料新增(PCAM) ,sql 查詢語句
        private const string SEL_SHOP_PCAM = @"select KeyIn_Flag,count(merchant_nbr) " +
                "from shop_pcam " +
                "where keyin_day between @SearchStart and @SearchEnd " +
                    "and keyin_userid = @user_id " +
                "group by KeyIn_Flag ";
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
                                string strSearchEnd, ref string strMsgID,
                                ref Int32 count, Int32 idx = -1, Int32 size = -1)
        {
            try
            {
                #region 生成要返回的DataTable結構
                DataTable dtblResult = new DataTable();
                //* 經辦ID
                dtblResult.Columns.Add(new DataColumn("user_id", Type.GetType("System.String")));
                //* 經辦姓名
                dtblResult.Columns.Add(new DataColumn("user_name", Type.GetType("System.String")));

                #region 卡人資料異動
                //* 地址（卡人資料異動）
                dtblResult.Columns.Add(new DataColumn("A01", Type.GetType("System.Int32")));
                //* 姓名（卡人資料異動）
                dtblResult.Columns.Add(new DataColumn("A04", Type.GetType("System.Int32")));
                //* 其他（卡人資料異動）
                dtblResult.Columns.Add(new DataColumn("A11", Type.GetType("System.Int32")));
                //* 族群碼（卡人資料異動）
                dtblResult.Columns.Add(new DataColumn("A06", Type.GetType("System.Int32")));
                //* 他行自扣一Key（卡人資料異動）
                dtblResult.Columns.Add(new DataColumn("A13", Type.GetType("System.Int32")));
                //* 他行自扣二Key（卡人資料異動）
                dtblResult.Columns.Add(new DataColumn("A14", Type.GetType("System.Int32")));
                //* 中信及郵局自扣一Key（卡人資料異動）
                dtblResult.Columns.Add(new DataColumn("A15", Type.GetType("System.Int32")));
                //* 中信及郵局自扣二Key（卡人資料異動）
                dtblResult.Columns.Add(new DataColumn("A16", Type.GetType("System.Int32")));
                //* 訊息/更正單自扣一Key（卡人資料異動）
                dtblResult.Columns.Add(new DataColumn("A17", Type.GetType("System.Int32")));
                //* 訊息/更正單自扣二Key（卡人資料異動）
                dtblResult.Columns.Add(new DataColumn("A18", Type.GetType("System.Int32")));
                #endregion

                #region 卡片資料異動
                //* 註銷（卡片資料異動）
                dtblResult.Columns.Add(new DataColumn("B01", Type.GetType("System.Int32")));
                //* 狀況碼（卡片資料異動）
                dtblResult.Columns.Add(new DataColumn("B05", Type.GetType("System.Int32")));
                //* 優惠碼（卡片資料異動）
                dtblResult.Columns.Add(new DataColumn("B04", Type.GetType("System.Int32")));
                //* 繳款異動（卡片資料異動）
                dtblResult.Columns.Add(new DataColumn("B02", Type.GetType("System.Int32")));
                //* 繳款評等（卡片資料異動）
                dtblResult.Columns.Add(new DataColumn("B17", Type.GetType("System.Int32")));

                #region 2014/12/12 一卡通1key

                dtblResult.Columns.Add(new DataColumn("B03", Type.GetType("System.Int32")));
                // 一卡通2key
                dtblResult.Columns.Add(new DataColumn("B18", Type.GetType("System.Int32")));

                #endregion

                //* 毀補一Key（卡片資料異動）
                dtblResult.Columns.Add(new DataColumn("B09", Type.GetType("System.Int32")));
                //* 毀補二Key（卡片資料異動）
                dtblResult.Columns.Add(new DataColumn("B10", Type.GetType("System.Int32")));
                //* 掛補一Key（卡片資料異動）
                dtblResult.Columns.Add(new DataColumn("B13", Type.GetType("System.Int32")));
                //* 掛補二Key（卡片資料異動）
                dtblResult.Columns.Add(new DataColumn("B14", Type.GetType("System.Int32")));
                //* BLKCODE 新增/異動 一Key （卡片資料異動）
                dtblResult.Columns.Add(new DataColumn("B11", Type.GetType("System.Int32")));
                //* BLKCODE 新增/異動 二Key （卡片資料異動）
                dtblResult.Columns.Add(new DataColumn("B12", Type.GetType("System.Int32")));
                //* BLKCODE 解除管制 一Key （卡片資料異動）
                dtblResult.Columns.Add(new DataColumn("B15", Type.GetType("System.Int32")));
                //* BLKCODE 解除管制 二Key （卡片資料異動）
                dtblResult.Columns.Add(new DataColumn("B16", Type.GetType("System.Int32")));
                #endregion

                #region 特店資料異動
                //* 資料異動一Key （特店資料異動）
                dtblResult.Columns.Add(new DataColumn("D01", Type.GetType("System.Int32")));
                //* 資料異動二Key （特店資料異動）
                dtblResult.Columns.Add(new DataColumn("D02", Type.GetType("System.Int32")));
                //* 費率一Key （特店資料異動）
                dtblResult.Columns.Add(new DataColumn("D03", Type.GetType("System.Int32")));
                //* 費率二Key （特店資料異動）
                dtblResult.Columns.Add(new DataColumn("D04", Type.GetType("System.Int32")));
                //* 帳號一Key （特店資料異動）
                dtblResult.Columns.Add(new DataColumn("D05", Type.GetType("System.Int32")));
                //* 帳號二Key （特店資料異動）
                dtblResult.Columns.Add(new DataColumn("D06", Type.GetType("System.Int32")));
                //* 解約一Key （特店資料異動）
                dtblResult.Columns.Add(new DataColumn("D07", Type.GetType("System.Int32")));
                //* 解約二Key （特店資料異動）
                dtblResult.Columns.Add(new DataColumn("D08", Type.GetType("System.Int32")));
                //* 機器一Key （特店資料異動）
                dtblResult.Columns.Add(new DataColumn("D09", Type.GetType("System.Int32")));
                //* 機器二Key （特店資料異動）
                dtblResult.Columns.Add(new DataColumn("D10", Type.GetType("System.Int32")));
                //*特店資料新增(6001)一Key
                dtblResult.Columns.Add(new DataColumn("D11", Type.GetType("System.Int32")));
                //*特店資料新增(6001)二Key
                dtblResult.Columns.Add(new DataColumn("D12", Type.GetType("System.Int32")));
                //*特店資料新增(PCAM)一Key
                dtblResult.Columns.Add(new DataColumn("D13", Type.GetType("System.Int32")));
                //*特店資料新增(PCAM)二Key
                dtblResult.Columns.Add(new DataColumn("D14", Type.GetType("System.Int32")));
                #endregion

                #endregion 生成要返回的DataTable結構

                #region 依據Request查詢資料庫，將查詢結果寫入dtblResult
                SqlCommand sqlcmdSearch = new SqlCommand();
                //2021/03/17_Ares_Stanley-DB名稱改為變數
                sqlcmdSearch.CommandText = string.Format(SEL_USER_STATISTICS, UtilHelper.GetAppSettings("DB_CSIP"));
                sqlcmdSearch.CommandType = CommandType.Text;
                //2021/05/24_Ares_Stanley-增加Command Timeout至180秒
                sqlcmdSearch.CommandTimeout = 180;

                //* 區間起
                SqlParameter parmSearchStart = new SqlParameter("@SearchStart", strSearchStart.Replace("/", ""));
                sqlcmdSearch.Parameters.Add(parmSearchStart);
                //* 區間迄
                SqlParameter parmSearchEnd = new SqlParameter("@SearchEnd", strSearchEnd.Replace("/", ""));
                sqlcmdSearch.Parameters.Add(parmSearchEnd);
                //* 經辦人ID
                SqlParameter parmUser_ID = new SqlParameter("@user_id", "");
                sqlcmdSearch.Parameters.Add(parmUser_ID);

                //20210517_Ares_Stanley-新增記錄SQL到Default LOG
                #region 記錄SQL到Default Log
                //將參數代入記錄用的SQL
                string recordSQL = sqlcmdSearch.CommandText;
                recordSQL = recordSQL.Replace("@SearchStart", string.Format("'{0}'", strSearchStart.Replace("/", ""))).Replace("@SearchEnd", string.Format("'{0}'", strSearchEnd.Replace("/", ""))).Replace("@user_id","");
                Logging.Log("========== 執行經辦作業量統計-查詢/列印 ==========\r查詢經辦訊息" + recordSQL);
                #endregion 記錄SQL到Default Log
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                //* 執行查詢SQL語句
                var dstSearchResult = idx >= 0 ? SearchOnDataSet(sqlcmdSearch, idx, size, ref count) : SearchOnDataSet(sqlcmdSearch);



                if (null == dstSearchResult)
                {
                    strMsgID = "01_03030000_002";
                    return null;
                }

                string strUserId = "";
                string strUserName = "";
                string loopRecordSQL = "";
                int intA01 = 0; int intA04 = 0; int intA11 = 0; int intA06 = 0; int intA13 = 0;
                int intA14 = 0; int intA15 = 0; int intA16 = 0; int intA17 = 0; int intA18 = 0;
                int intB01 = 0; int intB05 = 0; int intB04 = 0; int intB02 = 0; int intB17 = 0;
                int intB03 = 0; int intB18 = 0; // 2014/12/12 一卡通
                int intB09 = 0; int intB10 = 0; int intB13 = 0; int intB14 = 0; int intB11 = 0;
                int intB12 = 0; int intB15 = 0; int intB16 = 0; int intD01 = 0; int intD02 = 0;
                int intD03 = 0; int intD04 = 0; int intD05 = 0; int intD06 = 0; int intD07 = 0;
                int intD08 = 0; int intD09 = 0; int intD10 = 0; int intD11 = 0; int intD12 = 0;
                int intD13 = 0; int intD14 = 0;

                //* 循環統計每一位經辦的作業量
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[0].Rows.Count; intLoop++)
                {
                    strUserId = dstSearchResult.Tables[0].Rows[intLoop][0].ToString().Trim();
                    strUserName = dstSearchResult.Tables[0].Rows[intLoop][1].ToString().Trim();
                    intA01 = 0; intA04 = 0; intA11 = 0; intA06 = 0; intA13 = 0;
                    intA14 = 0; intA15 = 0; intA16 = 0; intA17 = 0; intA18 = 0;
                    intB01 = 0; intB05 = 0; intB04 = 0; intB02 = 0; intB17 = 0;
                    intB03 = 0; intB18 = 0; // 2014/10/30 餘額轉置
                    intB09 = 0; intB10 = 0; intB13 = 0; intB14 = 0; intB11 = 0;
                    intB12 = 0; intB15 = 0; intB16 = 0; intD01 = 0; intD02 = 0;
                    intD03 = 0; intD04 = 0; intD05 = 0; intD06 = 0; intD07 = 0;
                    intD08 = 0; intD09 = 0; intD10 = 0; intD11 = 0; intD12 = 0;
                    intD13 = 0; intD14 = 0;
                    #region 統計每一種作業業務量

                    #region 卡人資料異動
                    StringBuilder sbSqlCommand = new StringBuilder();
                    sbSqlCommand.Append(SEL_CARD_PERSON_ADDRESS);
                    sbSqlCommand.Append(SEL_CARD_PERSON_NAME);
                    sbSqlCommand.Append(SEL_CARD_PERSON_OTHER);


                    sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                    //20210517_Ares_Stanley-新增記錄SQL到Default LOG
                    #region 記錄SQL到Default Log
                    if (intLoop == 0)
                    {
                        loopRecordSQL = sqlcmdSearch.CommandText;
                        loopRecordSQL = loopRecordSQL.Replace("@SearchStart", string.Format("'{0}'", strSearchStart.Replace("/", ""))).Replace("@SearchEnd", string.Format("'{0}'", strSearchEnd.Replace("/", "")));
                        Logging.Log("========== 執行經辦作業量統計-查詢/列印 ==========\r查詢地址（卡人資料異動）、姓名（卡人資料異動）、其他（卡人資料異動）\r" + loopRecordSQL);
                    }
                    #endregion 記錄SQL到Default Log

                    //* 經辦人ID
                    sqlcmdSearch.Parameters["@user_id"].Value = dstSearchResult.Tables[0].Rows[intLoop][0].ToString().Trim();
                    //* 執行查詢SQL語句
                    // var dstResult = idx >= 0 ? SearchOnDataSet(sqlcmdSearch, idx, size, ref count) : SearchOnDataSet(sqlcmdSearch);
                    DataSet dstResult =  SearchOnDataSet(sqlcmdSearch);


                    if (null == dstResult)
                    {
                        strMsgID = "01_03030000_002";
                        return null;
                    }

                    //* 地址(卡人資料異動)
                    for (int intIndex = 0; intIndex < dstResult.Tables[0].Rows.Count; intIndex++)
                    {
                        intA01 += Convert.ToInt32(dstResult.Tables[0].Rows[intIndex][3]);
                    }
                    //* 姓名(卡人資料異動)
                    for (int intIndex = 0; intIndex < dstResult.Tables[1].Rows.Count; intIndex++)
                    {
                        intA04 += Convert.ToInt32(dstResult.Tables[1].Rows[intIndex][3]);
                    }
                    //* 其他（卡人資料異動）
                    for (int intIndex = 0; intIndex < dstResult.Tables[2].Rows.Count; intIndex++)
                    {
                        intA11 += Convert.ToInt32(dstResult.Tables[2].Rows[intIndex][3]);
                    }


                    //*重新查詢
                    sbSqlCommand = new StringBuilder();
                    sbSqlCommand.Append(SEL_CARD_PERSON_FAMILY);
                    sbSqlCommand.Append(SEL_CARD_PERSON_KEY);
                    sbSqlCommand.Append(SEL_CARD_PERSON_CHINA_TRUST);

                    sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                    //20210517_Ares_Stanley-新增記錄SQL到Default LOG
                    #region 記錄SQL到Default Log
                    if (intLoop == 0)
                    {
                        loopRecordSQL = sqlcmdSearch.CommandText;
                        loopRecordSQL = loopRecordSQL.Replace("@SearchStart", string.Format("'{0}'", strSearchStart.Replace("/", ""))).Replace("@SearchEnd", string.Format("'{0}'", strSearchEnd.Replace("/", "")));
                        Logging.Log("========== 執行經辦作業量統計-查詢/列印 ==========\r查詢族群碼（卡人資料異動）、他行自扣一Key、二Key（卡人資料異動）、中信及郵局自扣一Key、二Key（卡人資料異動）\r" + loopRecordSQL);
                    }                    
                    #endregion 記錄SQL到Default Log

                    //* 執行查詢SQL語句
                    // dstResult = idx >= 0 ? SearchOnDataSet(sqlcmdSearch, idx, size, ref count) : SearchOnDataSet(sqlcmdSearch);
                    dstResult = SearchOnDataSet(sqlcmdSearch);

                    if (null == dstResult)
                    {
                        strMsgID = "01_03030000_002";
                        return null;
                    }


                    //* 族群碼（卡人資料異動）
                    for (int intIndex = 0; intIndex < dstResult.Tables[0].Rows.Count; intIndex++)
                    {
                        intA06 += Convert.ToInt32(dstResult.Tables[0].Rows[intIndex][3]);
                    }
                    //* 他行自扣一Key、二Key（卡人資料異動）
                    for (int intIndex = 0; intIndex < dstResult.Tables[1].Rows.Count; intIndex++)
                    {
                        if (dstResult.Tables[1].Rows[intIndex][0].ToString().Trim() == "1")
                            intA13 += Convert.ToInt32(dstResult.Tables[1].Rows[intIndex][1]);
                        else
                            intA14 += Convert.ToInt32(dstResult.Tables[1].Rows[intIndex][1]);

                    }
                    //* 中信及郵局自扣一Key、二Key（卡人資料異動）
                    for (int intIndex = 0; intIndex < dstResult.Tables[2].Rows.Count; intIndex++)
                    {
                        if (dstResult.Tables[2].Rows[intIndex][0].ToString().Trim() == "1")
                            intA15 += Convert.ToInt32(dstResult.Tables[2].Rows[intIndex][1]);
                        else
                            intA16 += Convert.ToInt32(dstResult.Tables[2].Rows[intIndex][1]);
                    }

                    //*重新查詢
                    sbSqlCommand = new StringBuilder();
                    sbSqlCommand.Append(SEL_CARD_PERSON_MODIFY);
                    sbSqlCommand.Append(SEL_CARD_UNREG);
                    sbSqlCommand.Append(SEL_CARD_STATUS);

                    sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                    //20210517_Ares_Stanley-新增記錄SQL到Default LOG
                    #region 記錄SQL到Default Log
                    if (intLoop == 0)
                    {
                        loopRecordSQL = sqlcmdSearch.CommandText;
                        loopRecordSQL = loopRecordSQL.Replace("@SearchStart", string.Format("'{0}'", strSearchStart.Replace("/", ""))).Replace("@SearchEnd", string.Format("'{0}'", strSearchEnd.Replace("/", "")));
                        Logging.Log("========== 執行經辦作業量統計-查詢/列印 ==========\r查詢訊息/更正單自扣一Key、二Key（卡人資料異動）、註銷（卡片資料異動）、狀況碼（卡片資料異動）\r" + loopRecordSQL);
                    }                    
                    #endregion 記錄SQL到Default Log

                    //* 執行查詢SQL語句
                    // dstResult = idx >= 0 ? SearchOnDataSet(sqlcmdSearch, idx, size, ref count) : SearchOnDataSet(sqlcmdSearch);
                    dstResult = SearchOnDataSet(sqlcmdSearch);

                    if (null == dstResult)
                    {
                        strMsgID = "01_03030000_002";
                        return null;
                    }


                    //* 訊息/更正單自扣一Key、二Key（卡人資料異動）
                    for (int intIndex = 0; intIndex < dstResult.Tables[0].Rows.Count; intIndex++)
                    {
                        if (dstResult.Tables[0].Rows[intIndex][0].ToString().Trim() == "1")
                            intA17 += Convert.ToInt32(dstResult.Tables[0].Rows[intIndex][1]);
                        else
                            intA18 += Convert.ToInt32(dstResult.Tables[0].Rows[intIndex][1]);
                    }
                    #endregion

                    #region 卡片資料異動
                    //* 註銷（卡片資料異動）
                    for (int intIndex = 0; intIndex < dstResult.Tables[1].Rows.Count; intIndex++)
                    {
                        intB01 += Convert.ToInt32(dstResult.Tables[1].Rows[intIndex][3]);
                    }
                    //* 狀況碼（卡片資料異動）
                    for (int intIndex = 0; intIndex < dstResult.Tables[2].Rows.Count; intIndex++)
                    {
                        intB05 += Convert.ToInt32(dstResult.Tables[2].Rows[intIndex][3]);
                    }

                    //*重新查詢
                    sbSqlCommand = new StringBuilder();
                    sbSqlCommand.Append(SEL_CARD_PERCENT);
                    sbSqlCommand.Append(SEL_CARD_MONEY);
                    sbSqlCommand.Append(SEL_CARD_MONEY_GRADE);

                    sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                    //20210517_Ares_Stanley-新增記錄SQL到Default LOG
                    #region 記錄SQL到Default Log
                    if (intLoop == 0)
                    {
                        loopRecordSQL = sqlcmdSearch.CommandText;
                        loopRecordSQL = loopRecordSQL.Replace("@SearchStart", string.Format("'{0}'", strSearchStart.Replace("/", ""))).Replace("@SearchEnd", string.Format("'{0}'", strSearchEnd.Replace("/", "")));
                        Logging.Log("========== 執行經辦作業量統計-查詢/列印 ==========\r查詢優惠碼（卡片資料異動）、繳款異動（卡片資料異動）、繳款評等（卡片資料異動）\r" + loopRecordSQL);
                    }                    
                    #endregion 記錄SQL到Default Log

                    //* 執行查詢SQL語句
                    // dstResult = idx >= 0 ? SearchOnDataSet(sqlcmdSearch, idx, size, ref count) : SearchOnDataSet(sqlcmdSearch);
                    dstResult = SearchOnDataSet(sqlcmdSearch);

                    if (null == dstResult)
                    {
                        strMsgID = "01_03030000_002";
                        return null;
                    }

                    //* 優惠碼（卡片資料異動）
                    for (int intIndex = 0; intIndex < dstResult.Tables[0].Rows.Count; intIndex++)
                    {
                        intB04 += Convert.ToInt32(dstResult.Tables[0].Rows[intIndex][3]);
                    }
                    //* 繳款異動（卡片資料異動）
                    for (int intIndex = 0; intIndex < dstResult.Tables[1].Rows.Count; intIndex++)
                    {
                        intB02 += Convert.ToInt32(dstResult.Tables[1].Rows[intIndex][3]);
                    }
                    //* 繳款評等（卡片資料異動）
                    for (int intIndex = 0; intIndex < dstResult.Tables[2].Rows.Count; intIndex++)
                    {
                        intB17 += Convert.ToInt32(dstResult.Tables[2].Rows[intIndex][3]);
                    }

                    #region 2014/12/12 一卡通

                    sbSqlCommand = new StringBuilder();
                    sbSqlCommand.Append(SEL_iPASS);
                    sqlcmdSearch.CommandText = sbSqlCommand.ToString();
                    // dstResult = idx >= 0 ? SearchOnDataSet(sqlcmdSearch, idx, size, ref count) : SearchOnDataSet(sqlcmdSearch);
                    dstResult = SearchOnDataSet(sqlcmdSearch);

                    if (null == dstResult)
                    {
                        strMsgID = "01_03030000_002";
                        return null;
                    }

                    for (int i = 0; i < dstResult.Tables[0].Rows.Count; i++)
                    {
                        if (dstResult.Tables[0].Rows[i][0].ToString().Trim() == "1")
                            intB03 += Convert.ToInt32(dstResult.Tables[0].Rows[i][1]);
                        else
                            intB18 += Convert.ToInt32(dstResult.Tables[0].Rows[i][1]);
                    }

                    #endregion

                    //*重新查詢
                    sbSqlCommand = new StringBuilder();
                    sbSqlCommand.Append(SEL_KEY2_SURPLY_DESTORY);
                    sbSqlCommand.Append(SEL_KEY2_SURPLY);
                    sbSqlCommand.Append(SEL_BLOCK_CODE_ADD);

                    sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                    //20210517_Ares_Stanley-新增記錄SQL到Default LOG
                    #region 記錄SQL到Default Log
                    if (intLoop == 0)
                    {
                        loopRecordSQL = sqlcmdSearch.CommandText;
                        loopRecordSQL = loopRecordSQL.Replace("@SearchStart", string.Format("'{0}'", strSearchStart.Replace("/", ""))).Replace("@SearchEnd", string.Format("'{0}'", strSearchEnd.Replace("/", "")));
                        Logging.Log("========== 執行經辦作業量統計-查詢/列印 ==========\r查詢毀補一Key、二Key（卡片資料異動）、掛補一Key、二Key（卡片資料異動）、BLKCODE 新增/異動一Key、二Key（卡片資料異動）\r" + loopRecordSQL);
                    }                    
                    #endregion 記錄SQL到Default Log

                    //* 執行查詢SQL語句
                    // dstResult = idx >= 0 ? SearchOnDataSet(sqlcmdSearch, idx, size, ref count) : SearchOnDataSet(sqlcmdSearch);
                    dstResult = SearchOnDataSet(sqlcmdSearch);

                    if (null == dstResult)
                    {
                        strMsgID = "01_03030000_002";
                        return null;
                    }

                    //* 毀補一Key、二Key（卡片資料異動）
                    for (int intIndex = 0; intIndex < dstResult.Tables[0].Rows.Count; intIndex++)
                    {
                        if (dstResult.Tables[0].Rows[intIndex][0].ToString().Trim() == "1")
                            intB09 += Convert.ToInt32(dstResult.Tables[0].Rows[intIndex][1]);
                        else
                            intB10 += Convert.ToInt32(dstResult.Tables[0].Rows[intIndex][1]);
                    }
                    //* 掛補一Key、二Key（卡片資料異動）
                    for (int intIndex = 0; intIndex < dstResult.Tables[1].Rows.Count; intIndex++)
                    {
                        if (dstResult.Tables[1].Rows[intIndex][0].ToString().Trim() == "1")
                            intB13 += Convert.ToInt32(dstResult.Tables[1].Rows[intIndex][1]);
                        else
                            intB14 += Convert.ToInt32(dstResult.Tables[1].Rows[intIndex][1]);
                    }
                    //* BLKCODE 新增/異動一Key、二Key（卡片資料異動）
                    for (int intIndex = 0; intIndex < dstResult.Tables[2].Rows.Count; intIndex++)
                    {
                        if (dstResult.Tables[2].Rows[intIndex][0].ToString().Trim() == "1")
                            intB11 += Convert.ToInt32(dstResult.Tables[2].Rows[intIndex][1]);
                        else
                            intB12 += Convert.ToInt32(dstResult.Tables[2].Rows[intIndex][1]);
                    }


                    //*重新查詢
                    sbSqlCommand = new StringBuilder();
                    sbSqlCommand.Append(SEL_BLOCK_CODE_NOT_CONTROL);
                    sbSqlCommand.Append(SEL_SHOP_MODIFY);
                    sbSqlCommand.Append(SEL_SHOP_RATE);

                    sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                    //20210517_Ares_Stanley-新增記錄SQL到Default LOG
                    #region 記錄SQL到Default Log
                    if (intLoop == 0)
                    {
                        loopRecordSQL = sqlcmdSearch.CommandText;
                        loopRecordSQL = loopRecordSQL.Replace("@SearchStart", string.Format("'{0}'", strSearchStart.Replace("/", ""))).Replace("@SearchEnd", string.Format("'{0}'", strSearchEnd.Replace("/", "")));
                        Logging.Log("========== 執行經辦作業量統計-查詢/列印 ==========\r查詢BlockCode解除管制(卡片資料異動)、特店資料異動(資料異動)、費率一Key、二Key（特店資料異動）\r" + loopRecordSQL);
                    }                    
                    #endregion 記錄SQL到Default Log

                    //* 執行查詢SQL語句
                    // dstResult = idx >= 0 ? SearchOnDataSet(sqlcmdSearch, idx, size, ref count) : SearchOnDataSet(sqlcmdSearch);
                    dstResult = SearchOnDataSet(sqlcmdSearch);

                    if (null == dstResult)
                    {
                        strMsgID = "01_03030000_002";
                        return null;
                    }

                    //* BLKCODE 解除管制一Key、二Key（卡片資料異動）
                    for (int intIndex = 0; intIndex < dstResult.Tables[0].Rows.Count; intIndex++)
                    {
                        if (dstResult.Tables[0].Rows[intIndex][0].ToString().Trim() == "1")
                            intB15 += Convert.ToInt32(dstResult.Tables[0].Rows[intIndex][1]);
                        else
                            intB16 += Convert.ToInt32(dstResult.Tables[0].Rows[intIndex][1]);
                    }
                    #endregion 

                    #region 特店資料異動
                    //* 資料異動一Key、二Key（特店資料異動）
                    for (int intIndex = 0; intIndex < dstResult.Tables[1].Rows.Count; intIndex++)
                    {
                        if (dstResult.Tables[1].Rows[intIndex][0].ToString().Trim() == "1")
                            intD01 += Convert.ToInt32(dstResult.Tables[1].Rows[intIndex][1]);
                        else
                            intD02 += Convert.ToInt32(dstResult.Tables[1].Rows[intIndex][1]);
                    }
                    //* 費率一Key、二Key（特店資料異動）
                    for (int intIndex = 0; intIndex < dstResult.Tables[2].Rows.Count; intIndex++)
                    {
                        if (dstResult.Tables[2].Rows[intIndex][0].ToString().Trim() == "1")
                            intD03 += Convert.ToInt32(dstResult.Tables[2].Rows[intIndex][1]);
                        else
                            intD04 += Convert.ToInt32(dstResult.Tables[2].Rows[intIndex][1]);
                    }

                    //*重新查詢
                    sbSqlCommand = new StringBuilder();
                    sbSqlCommand.Append(SEL_SHOP_ACCOUNT);
                    sbSqlCommand.Append(SEL_SHOP_FIRE);
                    sbSqlCommand.Append(SEL_SHOP_MACHINE);

                    sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                    //20210517_Ares_Stanley-新增記錄SQL到Default LOG
                    #region 記錄SQL到Default Log
                    if (intLoop == 0)
                    {
                        loopRecordSQL = sqlcmdSearch.CommandText;
                        loopRecordSQL = loopRecordSQL.Replace("@SearchStart", string.Format("'{0}'", strSearchStart.Replace("/", ""))).Replace("@SearchEnd", string.Format("'{0}'", strSearchEnd.Replace("/", "")));
                        Logging.Log("========== 執行經辦作業量統計-查詢/列印 ==========\r查詢帳號一Key、二Key（特店資料異動）、解約一Key、二Key（特店資料異動）、機器一Key、二Key（特店資料異動）\r" + loopRecordSQL);
                    }                    
                    #endregion 記錄SQL到Default Log

                    //* 執行查詢SQL語句
                    // dstResult = idx >= 0 ? SearchOnDataSet(sqlcmdSearch, idx, size, ref count) : SearchOnDataSet(sqlcmdSearch);
                    dstResult = SearchOnDataSet(sqlcmdSearch);

                    if (null == dstResult)
                    {
                        strMsgID = "01_03030000_002";
                        return null;
                    }

                    //* 帳號一Key、二Key（特店資料異動）
                    for (int intIndex = 0; intIndex < dstResult.Tables[0].Rows.Count; intIndex++)
                    {
                        if (dstResult.Tables[0].Rows[intIndex][0].ToString().Trim() == "1")
                            intD05 += Convert.ToInt32(dstResult.Tables[0].Rows[intIndex][1]);
                        else
                            intD06 += Convert.ToInt32(dstResult.Tables[0].Rows[intIndex][1]);
                    }
                    //* 解約一Key、二Key（特店資料異動）
                    for (int intIndex = 0; intIndex < dstResult.Tables[1].Rows.Count; intIndex++)
                    {
                        if (dstResult.Tables[1].Rows[intIndex][0].ToString().Trim() == "1")
                            intD07 += Convert.ToInt32(dstResult.Tables[1].Rows[intIndex][1]);
                        else
                            intD08 += Convert.ToInt32(dstResult.Tables[1].Rows[intIndex][1]);
                    }
                    //* 機器一Key、二Key（特店資料異動）
                    for (int intIndex = 0; intIndex < dstResult.Tables[2].Rows.Count; intIndex++)
                    {
                        if (dstResult.Tables[2].Rows[intIndex][0].ToString().Trim() == "1")
                            intD09 += Convert.ToInt32(dstResult.Tables[2].Rows[intIndex][1]);
                        else
                            intD10 += Convert.ToInt32(dstResult.Tables[2].Rows[intIndex][1]);
                    }

                    //*重新查詢
                    sbSqlCommand = new StringBuilder();
                    sbSqlCommand.Append(SEL_SHOP_BASIC);
                    sbSqlCommand.Append(SEL_SHOP_PCAM);

                    sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                    //20210517_Ares_Stanley-新增記錄SQL到Default LOG
                    #region 記錄SQL到Default Log
                    if (intLoop == 0)
                    {
                        loopRecordSQL = sqlcmdSearch.CommandText;
                        loopRecordSQL = loopRecordSQL.Replace("@SearchStart", string.Format("'{0}'", strSearchStart.Replace("/", ""))).Replace("@SearchEnd", string.Format("'{0}'", strSearchEnd.Replace("/", "")));
                        Logging.Log("========== 執行經辦作業量統計-查詢/列印 ==========\r查詢特店資料新增(6001)、特店資料新增(PCAM)\r" + loopRecordSQL);
                    }                    
                    #endregion 記錄SQL到Default Log

                    //* 執行查詢SQL語句
                    // dstResult = idx >= 0 ? SearchOnDataSet(sqlcmdSearch, idx, size, ref count) : SearchOnDataSet(sqlcmdSearch);
                    dstResult = SearchOnDataSet(sqlcmdSearch);

                    if (null == dstResult)
                    {
                        strMsgID = "01_03030000_002";
                        return null;

                    }
                    //*特店資料新增(6001)
                    for (int intIndex = 0; intIndex < dstResult.Tables[0].Rows.Count; intIndex++)
                    {
                        if (dstResult.Tables[0].Rows[intIndex][0].ToString().Trim() == "1")
                            intD11 += Convert.ToInt32(dstResult.Tables[0].Rows[intIndex][1]);
                        else
                            intD12 += Convert.ToInt32(dstResult.Tables[0].Rows[intIndex][1]);
                    }

                    //*特店資料新增(pcam)
                    for (int intIndex = 0; intIndex < dstResult.Tables[1].Rows.Count; intIndex++)
                    {
                        if (dstResult.Tables[1].Rows[intIndex][0].ToString().Trim() == "1")
                            intD13 += Convert.ToInt32(dstResult.Tables[1].Rows[intIndex][1]);
                        else
                            intD14 += Convert.ToInt32(dstResult.Tables[1].Rows[intIndex][1]);
                    }
                    #endregion

                    #endregion 統計每一種作業業務量

                    //* 將取得的數據添加到返回的DataTable中
                    DataRow drowInsert = dtblResult.NewRow();
                    drowInsert["user_id"] = strUserId;
                    drowInsert["user_name"] = strUserName;
                    drowInsert["A01"] = intA01; drowInsert["A04"] = intA04; drowInsert["A11"] = intA11;
                    drowInsert["A06"] = intA06; drowInsert["A13"] = intA13; drowInsert["A14"] = intA14;
                    drowInsert["A15"] = intA15; drowInsert["A16"] = intA16; drowInsert["A17"] = intA17;
                    drowInsert["A18"] = intA18; drowInsert["B01"] = intB01; drowInsert["B05"] = intB05;
                    drowInsert["B04"] = intB04; drowInsert["B02"] = intB02; drowInsert["B17"] = intB17;
                    drowInsert["B03"] = intB03; drowInsert["B18"] = intB18; // 2014/12/12 一卡通
                    drowInsert["B09"] = intB09; drowInsert["B10"] = intB10; drowInsert["B13"] = intB13;
                    drowInsert["B14"] = intB14; drowInsert["B11"] = intB11; drowInsert["B12"] = intB12;
                    drowInsert["B15"] = intB15; drowInsert["B16"] = intB16; drowInsert["D01"] = intD01;
                    drowInsert["D02"] = intD02; drowInsert["D03"] = intD03; drowInsert["D04"] = intD04;
                    drowInsert["D05"] = intD05; drowInsert["D06"] = intD06; drowInsert["D07"] = intD07;
                    drowInsert["D08"] = intD08; drowInsert["D09"] = intD09; drowInsert["D10"] = intD10;
                    drowInsert["D11"] = intD11; drowInsert["D12"] = intD12; drowInsert["D13"] = intD13;
                    drowInsert["D14"] = intD14;
                    dtblResult.Rows.Add(drowInsert);

                    //記錄每一位User的ID以供前面的SQL使用
                    Logging.Log(string.Format("第 {0} 位 user 的 id 為 {1}", (intLoop + 1), dstSearchResult.Tables[0].Rows[intLoop][0].ToString().Trim()));
                    System.Threading.Thread.Sleep(1);
                }
                stopWatch.Stop();
                Logging.Log(string.Format("經辦作業量統計-查詢/列印的資料查詢、組成，耗時： {0} 毫秒", stopWatch.ElapsedMilliseconds)); //20210531_Ares_Stanley-增加經辦作業量統計查詢/列印的資料查詢、組成耗時記錄
                return dtblResult;

                #endregion 依據Request查詢資料庫，將查詢結果寫入dtblResult
            }
            catch (Exception exp)
            {
                BRCompareRpt.SaveLog(exp);
                strMsgID = "01_03030000_002";
                return null;
            }
        }

        public static Boolean SearchGripData(string strSearchStart, string strSearchEnd, 
            Int32 idx, Int32 size, ref Int32 count, ref DataTable dt)
        {
            try
            {
                //* 取報表數據
                string strMsgID = "";
                dt = SearchRPTData(strSearchStart, strSearchEnd, ref strMsgID, ref count, idx, size);
                if (null == dt)
                {
                    //* 取報表數據不成功
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }
    }
}
