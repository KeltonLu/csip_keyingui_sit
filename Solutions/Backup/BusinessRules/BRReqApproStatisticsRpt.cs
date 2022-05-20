//******************************************************************
//*  作    者：LuYang 
//*  功能說明：經辦作業量統計(請款加批)
//*  創建日期：2012/06/25
//*  修改記錄：

//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using CrystalDecisions.CrystalReports.Engine;
using CSIPKeyInGUI.EntityLayer;
using CSIPCommonModel.EntityLayer;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using Framework.Data.OM;

namespace CSIPKeyInGUI.BusinessRules
{
    /// <summary>
    /// 經辦作業量統計報表業務類
    /// </summary>
    public class BRReqApproStatisticsRpt : CSIPCommonModel.BusinessRules.BRBase<EntityCUSTOMER_LOG>
    {
        #region SQL 語句
        //* 經辦訊息
        private const string SEL_USER_STATISTICS = @"SELECT A.user_id user_id,B.USER_NAME " +
                "FROM customer_log A,(select distinct user_id,User_name from csip.dbo.M_USER) as  B " +
                "where A.mod_date between @SearchStart and @SearchEnd " +
                    "AND A.user_id = B.USER_ID " +
                    "AND A.trans_id IN ('01010100', 'A01','01010400','A04','A03','01010200'," +
                        "'A11','01010300','A06','01020100','B01','01020200','B05','01020500'," +
                        "'B04','01020300','B02','01020400') " +
                "UNION " +
                "SELECT I.user_id user_id,J.USER_NAME " +
                "FROM shop I,(select distinct user_id,User_name from csip.dbo.M_USER) as  J " +
                "where I.mod_date between @SearchStart and @SearchEnd " +
                    "AND I.user_id = J.USER_ID " +
                "UNION " +
                "SELECT I.user_id user_id,J.USER_NAME " +
                "FROM shop_reqAppro I,(select distinct user_id,User_name from csip.dbo.M_USER) as  J " +
                "where I.mod_date between @SearchStart and @SearchEnd " +
                    "AND I.user_id = J.USER_ID " +
                "UNION " +
                "SELECT K.user_id user_id,L.USER_NAME " +
                "FROM shop_Upload K,(select distinct user_id,User_name from csip.dbo.M_USER) as  L " +
                "where K.mod_date between @SearchStart and @SearchEnd " +
                    "AND K.user_id = L.USER_ID " +
                "UNION " +
                "SELECT M.keyin_userid user_id,N.USER_NAME " +
                "FROM shop_basic M,(select distinct user_id,User_name from csip.dbo.M_USER) as  N " +
                "where M.keyin_day between @SearchStart and @SearchEnd " +
                    "AND M.keyin_userid = N.USER_ID " +
                "UNION " +
                "SELECT O.keyin_userid user_id,P.USER_NAME " +
                "FROM shop_pcam O,(select distinct user_id,User_name from csip.dbo.M_USER) as  P " +
                "where O.keyin_day between @SearchStart and @SearchEnd " +
                    "AND O.keyin_userid = P.USER_ID " +
                "ORDER BY user_id";
      
  
        //* BlockCode解除管制(卡片資料異動)
        private const string SEL_BLOCK_CODE_NOT_CONTROL = @"select keyin_flag,count(card_no) " +
                "from Change_BLK " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and user_id = @user_id and Change_Type='D' " +
                "group by keyin_flag ";
        //* 特店資料異動(資料異動)
        private const string SEL_SHOP_MODIFY = @"select KEYIN_FLAG,count(shop_id),'S' sTABLE " +
                "from SHOP " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and CHANGE_ITEM = '1' " +
                    "and(rtrim(ltrim(record_name))<>'' or rtrim(ltrim(business_name))<>'' or rtrim(ltrim(merchant_name))<>'' or rtrim(ltrim(english_name))<>'' or rtrim(ltrim(undertaker))<>'' or"+
            " rtrim(ltrim(undertaker_id))<>'' or rtrim(ltrim(undertaker_engName))<>'' or rtrim(ltrim(undertaker_tel1))<>'' or rtrim(ltrim(undertaker_tel2))<>'' or rtrim(ltrim(undertaker_tel3))<>'' or"+
            " rtrim(ltrim(undertaker_add1))<>'' or rtrim(ltrim(undertaker_add2))<>'' or rtrim(ltrim(undertaker_add3))<>'' or rtrim(ltrim(realperson))<>'' or rtrim(ltrim(realperson_id))<>'' or"+
            " rtrim(ltrim(realperson_tel1))<>'' or rtrim(ltrim(realperson_tel2))<>'' or rtrim(ltrim(realperson_tel3))<>'' or rtrim(ltrim(junctionperson))<>'' or rtrim(ltrim(junctionperson_tel1))<>'' or"+
            " rtrim(ltrim(junctionperson_tel2))<>'' or rtrim(ltrim(junctionperson_tel3 ))<>'' or rtrim(ltrim(junctionperson_fax1))<>'' or rtrim(ltrim(junctionperson_fax2))<>'' or rtrim(ltrim(realperson_add1))<>'' or"+
            " rtrim(ltrim(realperson_add2))<>'' or rtrim(ltrim(realperson_add3))<>'' or rtrim(ltrim(junctionperson_recadd1))<>'' or rtrim(ltrim(junctionperson_recadd2))<>'' or rtrim(ltrim(junctionperson_recadd3))<>'' or"+
            " rtrim(ltrim(realadd_zip))<>'' or rtrim(ltrim(junctionperson_realadd1))<>'' or rtrim(ltrim(junctionperson_realadd2))<>'' or rtrim(ltrim(junctionperson_realadd3))<>'' or rtrim(ltrim(zip))<>'' or"+
            " rtrim(ltrim(commadd1))<>'' or rtrim(ltrim(comaddr2))<>'' or rtrim(ltrim(introduce))<>'' or rtrim(ltrim(introduce_flag))<>'' or rtrim(ltrim(invoice_cycle))<>'' or"+
            " rtrim(ltrim(hold_stmt))<>'' or  rtrim(ltrim(class))<>'' or  rtrim(ltrim(black))<>'') "+
                    "and user_id = @user_id " +
                "GROUP BY KEYIN_FLAG " +
                "union " +
                "select KEYIN_FLAG,count(shop_id),'U' sTABLE from SHOP_UPLOAD " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and CHANGE_ITEM = '1' " +
                    "and user_id = @user_id " +
                "GROUP BY KEYIN_FLAG " +
                "order by KEYIN_FLAG ";
        //* 特店資料異動(請款加批異動)
        private const string SEL_ReqAppro_U = @"select KEYIN_FLAG,count(shop_id),'S' sTABLE " +
                "from SHOP " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and CHANGE_ITEM = '1' " +
                    "and user_id = @user_id " +
                "GROUP BY KEYIN_FLAG " +
                "union " +
                "select KEYIN_FLAG,count(shop_id),'U' sTABLE from SHOP_ReqAppro " +
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
                    "and keyin_userid = @user_id and member_service <> 'Y' " +
                "group by KeyIn_Flag ";

        //*特店資料新增(6001會員附加服務) ,sql 查詢語句
        private const string SEL_SHOP_BASIC_M = @"select KeyIn_Flag,count(uni_no1+'-'+uni_no2) " +
                "from shop_basic " +
                "where keyin_day between @SearchStart and @SearchEnd " +
                    "and keyin_userid = @user_id and member_service = 'Y' " +
                "group by KeyIn_Flag ";

        //*特店資料新增(PCAM) ,sql 查詢語句
        private const string SEL_SHOP_PCAM = @"select KeyIn_Flag,count(merchant_nbr) " +
                "from shop_pcam " +
                "where keyin_day between @SearchStart and @SearchEnd " +
                    "and keyin_userid = @user_id " +
                "group by KeyIn_Flag ";

        //* 特店資料新增(請款加批)
        private const string SEL_ReqAppro_I = @"select KEYIN_FLAG,count(shop_id),'S' sTABLE " +
                "from SHOP " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and CHANGE_ITEM = '0' " +
                    "and user_id = @user_id " +
                "GROUP BY KEYIN_FLAG " +
                "union " +
                "select KEYIN_FLAG,count(shop_id),'U' sTABLE from SHOP_ReqAppro " +
                "where mod_date between @SearchStart and @SearchEnd " +
                    "and CHANGE_ITEM = '0' " +
                    "and user_id = @user_id " +
                "GROUP BY KEYIN_FLAG " +
                "order by KEYIN_FLAG ";

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
                //* 經辦ID
                dtblResult.Columns.Add(new DataColumn("user_id", Type.GetType("System.String")));
                //* 經辦姓名
                dtblResult.Columns.Add(new DataColumn("user_name", Type.GetType("System.String")));
                //* 資料異動一Key （特店資料異動）
                dtblResult.Columns.Add(new DataColumn("D01", Type.GetType("System.Int32")));
                //* 資料異動一Key （特店資料異動）
                dtblResult.Columns.Add(new DataColumn("D02", Type.GetType("System.Int32")));
                //* 請款加批一Key （特店資料異動）
                dtblResult.Columns.Add(new DataColumn("D15", Type.GetType("System.Int32")));
                //* 請款加批二Key （特店資料異動）
                dtblResult.Columns.Add(new DataColumn("D16", Type.GetType("System.Int32")));
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

                //*特店資料新增(6001會員附加服務)一Key
                dtblResult.Columns.Add(new DataColumn("D19", Type.GetType("System.Int32")));
                //*特店資料新增(6001會員附加服務)二Key
                dtblResult.Columns.Add(new DataColumn("D20", Type.GetType("System.Int32")));

                //*特店資料新增(PCAM)一Key
                dtblResult.Columns.Add(new DataColumn("D13", Type.GetType("System.Int32")));
                //*特店資料新增(PCAM)二Key
                dtblResult.Columns.Add(new DataColumn("D14", Type.GetType("System.Int32")));
                //* 請款加批一Key （特店資料新增）
                dtblResult.Columns.Add(new DataColumn("D17", Type.GetType("System.Int32")));
                //* 請款加批二Key （特店資料新增）
                dtblResult.Columns.Add(new DataColumn("D18", Type.GetType("System.Int32")));

                #endregion 生成要返回的DataTable結構

                #region 依據Request查詢資料庫，將查詢結果寫入dtblResult
                SqlCommand sqlcmdSearch = new SqlCommand();
                sqlcmdSearch.CommandText = SEL_USER_STATISTICS;
                sqlcmdSearch.CommandType = CommandType.Text;

                //* 區間起
                SqlParameter parmSearchStart = new SqlParameter("@SearchStart", strSearchStart.Replace("/", ""));
                sqlcmdSearch.Parameters.Add(parmSearchStart);
                //* 區間迄
                SqlParameter parmSearchEnd = new SqlParameter("@SearchEnd", strSearchEnd.Replace("/", ""));
                sqlcmdSearch.Parameters.Add(parmSearchEnd);
                //* 經辦人ID
                SqlParameter parmUser_ID = new SqlParameter("@user_id", "");
                sqlcmdSearch.Parameters.Add(parmUser_ID);

                //* 執行查詢SQL語句
                DataSet dstSearchResult = BRReqApproStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult)
                {
                    strMsgID = "01_03030000_002";
                    return null;
                }

                string strUserId = "";
                string strUserName = "";
                int intD01 = 0; int intD02 = 0;
                int intD03 = 0; int intD04 = 0; int intD05 = 0; int intD06 = 0; int intD07 = 0;
                int intD08 = 0; int intD09 = 0; int intD10 = 0; int intD11 = 0; int intD12 = 0;
                int intD13 = 0; int intD14 = 0; int intD15 = 0; int intD16 = 0; int intD17 = 0;
                int intD18 = 0; int intD19 = 0; int intD20 = 0;

                //* 循環統計每一位經辦的作業量
                for (int intLoop = 0; intLoop < dstSearchResult.Tables[0].Rows.Count; intLoop++)
                {
                    strUserId = dstSearchResult.Tables[0].Rows[intLoop][0].ToString().Trim();
                    strUserName = dstSearchResult.Tables[0].Rows[intLoop][1].ToString().Trim();
                    intD01 = 0; intD02 = 0;
                    intD03 = 0; intD04 = 0; intD05 = 0; intD06 = 0; intD07 = 0;
                    intD08 = 0; intD09 = 0; intD10 = 0; intD11 = 0; intD12 = 0;
                    intD13 = 0; intD14 = 0; intD15 = 0; intD16 = 0; intD17 = 0;
                    intD18 = 0; intD19 = 0; intD20 = 0;

                    #region 統計每一種作業業務量
                    StringBuilder sbSqlCommand = new StringBuilder();

                    sqlcmdSearch.CommandText = sbSqlCommand.ToString();
                    //* 經辦人ID
                    sqlcmdSearch.Parameters["@user_id"].Value = dstSearchResult.Tables[0].Rows[intLoop][0].ToString().Trim();
                    //* 執行查詢SQL語句
                    DataSet dstResult = BRUserStatisticsRpt.SearchOnDataSet(sqlcmdSearch);

                    sbSqlCommand.Append(SEL_BLOCK_CODE_NOT_CONTROL);
                    sbSqlCommand.Append(SEL_SHOP_MODIFY);
                    sbSqlCommand.Append(SEL_SHOP_RATE);

                    sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                    //* 執行查詢SQL語句
                    dstResult = BRReqApproStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                    if (null == dstResult)
                    {
                        strMsgID = "01_03030000_002";
                        return null;
                    }

                
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
                    sbSqlCommand.Append(SEL_ReqAppro_U);
                    sbSqlCommand.Append(SEL_ReqAppro_I);

                    sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                    //* 執行查詢SQL語句
                    dstResult = BRReqApproStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
                    if (null == dstResult)
                    {
                        strMsgID = "01_03030000_002";
                        return null;
                    }

                    //* 資料異動一Key、二Key（請款加批異動）
                    for (int intIndex = 0; intIndex < dstResult.Tables[0].Rows.Count; intIndex++)
                    {
                        if (dstResult.Tables[0].Rows[intIndex][0].ToString().Trim() == "1")
                            intD15 += Convert.ToInt32(dstResult.Tables[0].Rows[intIndex][1]);
                        else
                            intD16 += Convert.ToInt32(dstResult.Tables[0].Rows[intIndex][1]);
                    }

                    //* 資料新增一Key、二Key（請款加批新增）
                    for (int intIndex = 0; intIndex < dstResult.Tables[1].Rows.Count; intIndex++)
                    {
                        if (dstResult.Tables[1].Rows[intIndex][0].ToString().Trim() == "1")
                            intD17 += Convert.ToInt32(dstResult.Tables[1].Rows[intIndex][1]);
                        else
                            intD18 += Convert.ToInt32(dstResult.Tables[1].Rows[intIndex][1]);
                    }


                    //*重新查詢
                    sbSqlCommand = new StringBuilder();
                    sbSqlCommand.Append(SEL_SHOP_ACCOUNT);
                    sbSqlCommand.Append(SEL_SHOP_FIRE);
                    sbSqlCommand.Append(SEL_SHOP_MACHINE);

                    sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                    //* 執行查詢SQL語句
                    dstResult = BRReqApproStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
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
                    sbSqlCommand.Append(SEL_SHOP_BASIC_M);
                    sbSqlCommand.Append(SEL_SHOP_PCAM);

                    sqlcmdSearch.CommandText = sbSqlCommand.ToString();

                    //* 執行查詢SQL語句
                    dstResult = BRReqApproStatisticsRpt.SearchOnDataSet(sqlcmdSearch);
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

                    //*特店資料新增(6001會員附加服務)
                    for (int intIndex = 0; intIndex < dstResult.Tables[1].Rows.Count; intIndex++)
                    {
                        if (dstResult.Tables[1].Rows[intIndex][0].ToString().Trim() == "1")
                            intD19 += Convert.ToInt32(dstResult.Tables[1].Rows[intIndex][1]);
                        else
                            intD20 += Convert.ToInt32(dstResult.Tables[1].Rows[intIndex][1]);
                    }

                    //*特店資料新增(pcam)
                    for (int intIndex = 0; intIndex < dstResult.Tables[2].Rows.Count; intIndex++)
                    {
                        if (dstResult.Tables[2].Rows[intIndex][0].ToString().Trim() == "1")
                            intD13 += Convert.ToInt32(dstResult.Tables[2].Rows[intIndex][1]);
                        else
                            intD14 += Convert.ToInt32(dstResult.Tables[2].Rows[intIndex][1]);
                    }

                    #endregion 統計每一種作業業務量

                    //* 將取得的數據添加到返回的DataTable中
                    DataRow drowInsert = dtblResult.NewRow();
                    drowInsert["user_id"] = strUserId;
                    drowInsert["user_name"] = strUserName;
                    drowInsert["D01"] = intD01;
                    drowInsert["D02"] = intD02; drowInsert["D03"] = intD03; drowInsert["D04"] = intD04;
                    drowInsert["D05"] = intD05; drowInsert["D06"] = intD06; drowInsert["D07"] = intD07;
                    drowInsert["D08"] = intD08; drowInsert["D09"] = intD09; drowInsert["D10"] = intD10;
                    drowInsert["D11"] = intD11; drowInsert["D12"] = intD12; drowInsert["D13"] = intD13;
                    drowInsert["D14"] = intD14; drowInsert["D15"] = intD15; drowInsert["D16"] = intD16;
                    drowInsert["D17"] = intD17; drowInsert["D18"] = intD18; drowInsert["D19"] = intD19;
                    drowInsert["D20"] = intD20;
                    dtblResult.Rows.Add(drowInsert);

                    System.Threading.Thread.Sleep(1);
                }

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

        /// <summary>
        /// 取符合條件的報表數據，并生成報表
        /// </summary>
        /// <param name="strSearchStart">區間起</param>
        /// <param name="strSearchEnd">區間迄</param>
        /// <param name="rptResult">返回報表對象</param>
        /// <param name="strMsgID">傳回的消息ID號</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report(string strSearchStart, string strSearchEnd,
                            out ReportDocument rptResult, ref string strMsgID)
        {
            try
            {
                //* 取報表數據
                DataTable dtblSearchResult = SearchRPTData(strSearchStart, strSearchEnd, ref strMsgID);
                if (null == dtblSearchResult)
                {
                    //* 取報表數據不成功
                    rptResult = null;
                    return false;
                }

                //* 生成報表
                ReportDocument rptResultTemp = new ReportDocument();
                string strRPTPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "newworkReqAppro_list.rpt";
                rptResultTemp.Load(@strRPTPathFile);
                //* 起訖日期
                rptResultTemp.DataDefinition.FormulaFields["qdate"].Text = "'起迄日期：" + strSearchStart.Replace("/", "") + "-" + strSearchEnd.Replace("/", "") + "'";
                //* 列印經辦
                rptResultTemp.DataDefinition.FormulaFields["puser"].Text = "'列印經辦：" + ((EntityAGENT_INFO)System.Web.HttpContext.Current.Session["Agent"]).agent_name + "'";
                //* 列印日期
                rptResultTemp.DataDefinition.FormulaFields["pdate"].Text = "'列印日期：" + DateTime.Now.ToString("yyyyMMdd") + "'";
                rptResultTemp.SetDataSource(dtblSearchResult);
                rptResultTemp.Refresh();
                rptResult = rptResultTemp;
                return true;
            }
            catch (Exception exp)
            {
                rptResult = null;
                BRCompareRpt.SaveLog(exp);
                strMsgID = "01_03030000_004";
                return false;
            }
        }
    }
}
