//******************************************************************
//*  作    者：占偉林(James)
//*  功能說明：特店維護經辦作業量統計表
//*  創建日期：2009/11/17
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
    /// 作業量比對報表業務類
    /// </summary>
    public class BRShopUserRpt : CSIPCommonModel.BusinessRules.BRBase<EntityCUSTOMER_LOG>
    {    
        #region SQL 語句
        //* 一次鍵檔
        private const string SEL_SHOP_KEY_1 = @"select A.shop_id,A.shop_name,B.USER_NAME,A.write_date," +
                        "C.bank_name,C.branch_name,C.account,C.check_num," +
                        "C.account1,C.account2,C.cancel_code1,C.cancel_date,C.cancel_code2 " +
                    "from shop_rpt A,(select distinct user_id,User_name from csip.dbo.M_USER) as  B, shop_1keylog C " +
                    "where A.user_id = B.USER_ID " +
                        "and A.write_date between @SearchStart and @SearchEnd " +
                        "and type = '1' " +
                        "and A.shop_id = C.shop_id and A.user_id = C.user_id and A.write_date = C.write_date " +
                    "order by B.USER_NAME,A.write_date,A.shop_id";
        //* 二次鍵檔
        private const string SEL_SHOP_KEY_2 = @"select A.shop_id,A.shop_name,B.USER_NAME,A.write_date," +
                        "C.mod_time, C.field_name, C.before, C.after " +
                    "from shop_rpt A,(select distinct user_id,User_name from csip.dbo.M_USER) as  B,customer_log C " +
                    "where A.user_id = B.USER_ID " +
                        "and A.write_date between @SearchStart and @SearchEnd " +
                        "and type = '2' " +
                        "and A.shop_id = C.query_key " +
                        "and A.write_date = C.mod_date " +
                    "order by B.USER_NAME,A.write_date, A.shop_id";
        #endregion

        /// <summary>
        /// 取報表數據
        /// </summary>
        /// <param name="strSearchStart">區間起</param>
        /// <param name="strSearchEnd">區間迄</param>
        /// <param name="strMsgID">返回的錯誤ID號</param>
        /// <returns>成功時：返回查詢結果；失敗時：null</returns>
        public static DataTable SearchRPTData(string strSearchStart, string strSearchEnd, 
                                string strType ,ref string strMsgID)
        {
            try
            {
                #region 生成要返回的DataTable結構
                DataTable dtblResult = new DataTable();
                //* commnon columns
                //* 商店代號
                dtblResult.Columns.Add(new DataColumn("shop_id", Type.GetType("System.String")));
                //* 商店名稱
                dtblResult.Columns.Add(new DataColumn("shop_name", Type.GetType("System.String")));
                //* 經辦姓名
                dtblResult.Columns.Add(new DataColumn("user_name", Type.GetType("System.String")));
                //* 作業日期
                dtblResult.Columns.Add(new DataColumn("write_date", Type.GetType("System.String")));
                //* 用戶統計的欄位
                dtblResult.Columns.Add(new DataColumn("num", Type.GetType("System.Int32")));
                //* Key1 columns
                //* 銀行名稱
                dtblResult.Columns.Add(new DataColumn("bank_name", Type.GetType("System.String")));
                //* 分行名稱
                dtblResult.Columns.Add(new DataColumn("branch_name", Type.GetType("System.String")));
                //* 戶名
                dtblResult.Columns.Add(new DataColumn("account", Type.GetType("System.String")));
                //* 檢碼
                dtblResult.Columns.Add(new DataColumn("check_num", Type.GetType("System.String")));
                //* 帳號1
                dtblResult.Columns.Add(new DataColumn("account1", Type.GetType("System.String")));
                //* 帳號2
                dtblResult.Columns.Add(new DataColumn("account2", Type.GetType("System.String")));
                //* 解約代號
                dtblResult.Columns.Add(new DataColumn("cancel_code1", Type.GetType("System.String")));
                //* 解約日期
                dtblResult.Columns.Add(new DataColumn("cancel_date", Type.GetType("System.String")));
                //* 解約原因碼
                dtblResult.Columns.Add(new DataColumn("cancel_code2", Type.GetType("System.String")));
                //* Key2 columns
                //* 異動時間
                dtblResult.Columns.Add(new DataColumn("mod_time", Type.GetType("System.String")));
                //* 異動欄位
                dtblResult.Columns.Add(new DataColumn("field_name", Type.GetType("System.String")));
                //* 異動前內容
                dtblResult.Columns.Add(new DataColumn("before", Type.GetType("System.String")));
                //* 異動後內容
                dtblResult.Columns.Add(new DataColumn("after", Type.GetType("System.String")));
                #endregion 生成要返回的DataTable結構

                #region 查詢數據
                SqlCommand sqlcmdSearch = new SqlCommand();
                if (strType == "1")   //* 一次鍵檔
                    sqlcmdSearch.CommandText = SEL_SHOP_KEY_1;
                else                  //* 二次鍵檔
                    sqlcmdSearch.CommandText = SEL_SHOP_KEY_2;
                sqlcmdSearch.CommandType = CommandType.Text;

                //* 區間起
                SqlParameter parmSearchStart = new SqlParameter(@"SearchStart", strSearchStart.Replace("/", ""));
                sqlcmdSearch.Parameters.Add(parmSearchStart);
                //* 區間迄
                SqlParameter parmSearchEnd = new SqlParameter(@"SearchEnd", strSearchEnd.Replace("/", ""));
                sqlcmdSearch.Parameters.Add(parmSearchEnd);

                //* 執行查詢SQL語句
                DataSet dstSearchResult = BRShopUserRpt.SearchOnDataSet(sqlcmdSearch);
                if (null == dstSearchResult)
                {
                    strMsgID = "01_03040000_002";
                    return null;
                }
                else if (dstSearchResult.Tables[0].Rows.Count == 0)
                {
                    strMsgID = "01_03040000_003";
                    return null;
                }
                #endregion 查詢數據

                if (strType == "1")
                {
                    #region 一次鍵檔
                    for (int intLoop = 0; intLoop < dstSearchResult.Tables[0].Rows.Count; intLoop++)
                    {
                        DataRow drowInsert = dtblResult.NewRow();
                        drowInsert["shop_id"] = dstSearchResult.Tables[0].Rows[intLoop][0].ToString();
                        drowInsert["shop_name"] = dstSearchResult.Tables[0].Rows[intLoop][1].ToString();
                        drowInsert["user_name"] = dstSearchResult.Tables[0].Rows[intLoop][2].ToString();
                        drowInsert["write_date"] = dstSearchResult.Tables[0].Rows[intLoop][3].ToString();
                        drowInsert["num"] = 1;
                        drowInsert["bank_name"] = dstSearchResult.Tables[0].Rows[intLoop][4].ToString();
                        drowInsert["branch_name"] = dstSearchResult.Tables[0].Rows[intLoop][5].ToString();
                        drowInsert["account"] = dstSearchResult.Tables[0].Rows[intLoop][6].ToString();
                        drowInsert["check_num"] = dstSearchResult.Tables[0].Rows[intLoop][7].ToString();
                        drowInsert["account1"] = dstSearchResult.Tables[0].Rows[intLoop][8].ToString();
                        drowInsert["account2"] = dstSearchResult.Tables[0].Rows[intLoop][9].ToString();
                        drowInsert["cancel_code1"] = dstSearchResult.Tables[0].Rows[intLoop][10].ToString();
                        drowInsert["cancel_date"] = dstSearchResult.Tables[0].Rows[intLoop][11].ToString();
                        drowInsert["cancel_code2"] = dstSearchResult.Tables[0].Rows[intLoop][12].ToString();
                        dtblResult.Rows.Add(drowInsert);
                    }
                    #endregion 一次鍵檔
                }
                else
                {
                    #region 二次鍵檔
                    for (int intLoop = 0; intLoop < dstSearchResult.Tables[0].Rows.Count; intLoop++)
                    {
                        DataRow drowInsert = dtblResult.NewRow();
                        drowInsert["shop_id"] = dstSearchResult.Tables[0].Rows[intLoop][0].ToString();
                        drowInsert["shop_name"] = dstSearchResult.Tables[0].Rows[intLoop][1].ToString();
                        drowInsert["user_name"] = dstSearchResult.Tables[0].Rows[intLoop][2].ToString();
                        drowInsert["write_date"] = dstSearchResult.Tables[0].Rows[intLoop][3].ToString();
                        drowInsert["num"] = 1;
                        drowInsert["mod_time"] = dstSearchResult.Tables[0].Rows[intLoop][4].ToString();
                        drowInsert["field_name"] = dstSearchResult.Tables[0].Rows[intLoop][5].ToString();
                        drowInsert["before"] = dstSearchResult.Tables[0].Rows[intLoop][6].ToString();
                        drowInsert["after"] = dstSearchResult.Tables[0].Rows[intLoop][7].ToString();
                        dtblResult.Rows.Add(drowInsert);
                    }
                    #endregion 二次鍵檔
                }

                return dtblResult;
            }
            catch (Exception exp)
            {
                BRCompareRpt.SaveLog(exp);
                strMsgID = "01_03040000_002";
                return null;
            }
        }

        /// <summary>
        /// 取符合條件的報表數據，并生成報表
        /// </summary>
        /// <param name="strSearchStart">區間起</param>
        /// <param name="strSearchEnd">區間迄</param>
        /// <param name="strType">strType:1---一次鍵檔;2---二次鍵檔</param>
        /// <param name="rptResult">返回報表對象</param>
        /// <param name="strMsgID">傳回的消息ID號</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report(string strSearchStart, string strSearchEnd,
                            string strType, out ReportDocument rptResult, ref string strMsgID)
        {
            try
            {
                //* 取報表數據
                DataTable dtblSearchResult = SearchRPTData(strSearchStart, strSearchEnd, strType, ref strMsgID);
                if (null == dtblSearchResult)
                {
                    //* 取報表數據不成功
                    rptResult = null;
                    return false;
                }

                //* 生成報表
                ReportDocument rptResultTemp = new ReportDocument();
                string strRPTPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "shop.rpt";
                rptResultTemp.Load(@strRPTPathFile);
                //* 起訖日期
                rptResultTemp.DataDefinition.FormulaFields["qdate"].Text = "'起迄日期：" + strSearchStart.Replace("/","") + "-" + strSearchEnd.Replace("/","") + "'";
                //* 列印經辦
                rptResultTemp.DataDefinition.FormulaFields["puser"].Text = "'列印經辦：" + ((EntityAGENT_INFO)System.Web.HttpContext.Current.Session["Agent"]).agent_name + "'";
                //* 列印日期
                rptResultTemp.DataDefinition.FormulaFields["pdate"].Text = "'列印日期：" + DateTime.Now.ToString("yyyyMMdd") + "'";
                if (strType=="1")
                {
                    //* Title
                    rptResultTemp.DataDefinition.FormulaFields["title"].Text = "'特店維護經辦工作量統計（一次鍵檔）'";
                    //* mdate
                    rptResultTemp.DataDefinition.FormulaFields["mdate"].Text = "'銀行名稱 / 分行名稱 / 戶 名 / 檢 碼 / 帳 號 / 解約代號 / 解約日期 / 解約原因碼'";
                }
                else
                {
                    //* Title
                    rptResultTemp.DataDefinition.FormulaFields["title"].Text = "'特店維護經辦工作量統計（二次鍵檔）'";
                    //* mdate
                    rptResultTemp.DataDefinition.FormulaFields["mdate"].Text = "'異動時間 / 異動欄位 / 異動前內容 / 異動後內容'";
                }
                rptResultTemp.SetDataSource(dtblSearchResult);
                rptResultTemp.Refresh();
                rptResult = rptResultTemp;
                return true;
            }
            catch (Exception exp)
            {
                rptResult = null;
                BRCompareRpt.SaveLog(exp);
                strMsgID = "01_03040000_004";
                return false;
            }
        }
    }
}
