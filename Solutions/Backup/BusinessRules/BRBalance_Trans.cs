using System;
using System.Collections.Generic;
using System.Text;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM.Collections;
using Framework.Data.OM;
using Framework.Data.OM.Transaction;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace CSIPKeyInGUI.BusinessRules
{
    public class BRBalance_Trans : CSIPCommonModel.BusinessRules.BRBase<EntityAuto_Balance_Trans>
    {
        #region sql
        public const string SQL_SEL = @"SELECT *, ROW_NUMBER() OVER(ORDER BY Modify_DateTime DESC) AS 'RowNumber'
                                          FROM dbo.Balance_Trans WITH (NOLOCK) ";
                                          
        public const string SQL_SEL_ADD = @"SELECT *, ROW_NUMBER() OVER(ORDER BY Modify_DateTime DESC) AS 'RowNumber'
                                            FROM dbo.Balance_Trans WITH (NOLOCK)
                                            where CardNo = @CardNo and PID = @PID and Upload_Flag = 'N' ";                                            

        #endregion sql

        public static DataTable SelectLOG(string strDateS, string strDateE, string CARD_ID, string PID,
            string p_flag, string u_flag, int iPageIndex, int iPageSize, ref int iTotalCount)
        {
            try
            {
                string where = "";
                SqlCommand sc = new SqlCommand();
                sc.CommandType = CommandType.Text;
                sc.CommandText = SQL_SEL;

                DataTable dtBalance = new DataTable();

                if (!string.IsNullOrEmpty(CARD_ID))
                {
                    where += "where CardNo = @Card_id ";
                    SqlParameter spCD = new SqlParameter("Card_id", CARD_ID);
                    sc.Parameters.Add(spCD);
                }

                if (!string.IsNullOrEmpty(PID))
                {
                    if (string.IsNullOrEmpty(where))
                        where += "where PID = @PID ";
                    else
                        where += "and PID = @PID ";
                    SqlParameter spPI = new SqlParameter("PID", PID);
                    sc.Parameters.Add(spPI);
                }

                #region 餘額轉置日期
                if (!string.IsNullOrEmpty(strDateS))
                {
                    SqlParameter spDS = new SqlParameter("DateS", Convert.ToDateTime(strDateS.Substring(0, 4) + "/" + strDateS.Substring(4, 2) + "/" + strDateS.Substring(6, 2)));
                    sc.Parameters.Add(spDS);
                }

                if (!string.IsNullOrEmpty(strDateE))
                {
                    string eDay = string.Format("{0}/{1}/{2} 23:59:59", strDateE.Substring(0, 4), 
                        strDateE.Substring(4, 2), strDateE.Substring(6, 2));
                    //SqlParameter spDE = new SqlParameter("DateE", Convert.ToDateTime(strDateE.Substring(0, 4) 
                    //    + "/" + strDateE.Substring(4, 2) + "/" + strDateE.Substring(6, 2)));
                    SqlParameter spDE = new SqlParameter("DateE", eDay);
                    sc.Parameters.Add(spDE);
                }

                if (!string.IsNullOrEmpty(strDateS) && !string.IsNullOrEmpty(strDateE))
                {
                    if (string.IsNullOrEmpty(where))
                        where += "where Trans_Date between @DateS and @DateE ";
                    else
                        where += "and Trans_Date between @DateS and @DateE ";
                }

                if (!string.IsNullOrEmpty(strDateS) && string.IsNullOrEmpty(strDateE))
                {
                    if (string.IsNullOrEmpty(where))
                        where += "where Trans_Date > @DateS ";
                    else
                        where += "and Trans_Date > @DateS ";
                }

                if (string.IsNullOrEmpty(strDateS) && !string.IsNullOrEmpty(strDateE))
                {
                    if (string.IsNullOrEmpty(where))
                        where += "where Trans_Date < @DateE ";
                    else
                        where += "and Trans_Date < @DateE ";
                }
                #endregion

                sc.CommandText += where;

                if (p_flag != "0")
                {
                    sc.CommandText += "and Process_Flag = @p_flag ";
                    SqlParameter spSC = new SqlParameter("p_flag", p_flag);
                    sc.Parameters.Add(spSC);
                }

                if (u_flag != "0")
                {
                    sc.CommandText += "and Upload_Flag = @u_flag ";
                    SqlParameter spUP = new SqlParameter("u_flag", u_flag);
                    sc.Parameters.Add(spUP);
                }

                sc.CommandText += "order by RowNumber,Modify_DateTime desc ";

                dtBalance = BRREISSUE_CARD.SearchOnDataSet(sc, iPageIndex, iPageSize, ref iTotalCount).Tables[0];

                return dtBalance;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable SelectLOG(string strDateS, string strDateE, string CARD_ID, string PID,
            string p_flag, string u_flag)
        {
            try
            {
                string where = "";
                SqlCommand sc = new SqlCommand();
                sc.CommandType = CommandType.Text;
                sc.CommandText = SQL_SEL;

                DataTable dtBalance = new DataTable();

                if (!string.IsNullOrEmpty(CARD_ID))
                {
                    where += "where CardNo = @Card_id ";
                    SqlParameter spCD = new SqlParameter("Card_id", CARD_ID);
                    sc.Parameters.Add(spCD);
                }

                if (!string.IsNullOrEmpty(PID))
                {
                    if (string.IsNullOrEmpty(where))
                        where += "where PID = @PID ";
                    else
                        where += "and PID = @PID ";
                    SqlParameter spPI = new SqlParameter("PID", PID);
                    sc.Parameters.Add(spPI);
                }

                #region 餘額轉置日期
                if (!string.IsNullOrEmpty(strDateS))
                {
                    SqlParameter spDS = new SqlParameter("DateS", Convert.ToDateTime(strDateS.Substring(0, 4) + "/" + strDateS.Substring(4, 2) + "/" + strDateS.Substring(6, 2)));
                    sc.Parameters.Add(spDS);
                }

                if (!string.IsNullOrEmpty(strDateE))
                {
                    string eDay = string.Format("{0}/{1}/{2} 23:59:59", strDateE.Substring(0, 4),
                        strDateE.Substring(4, 2), strDateE.Substring(6, 2));
                    //SqlParameter spDE = new SqlParameter("DateE", Convert.ToDateTime(strDateE.Substring(0, 4) 
                    //    + "/" + strDateE.Substring(4, 2) + "/" + strDateE.Substring(6, 2)));
                    SqlParameter spDE = new SqlParameter("DateE", eDay);
                    sc.Parameters.Add(spDE);
                }

                if (!string.IsNullOrEmpty(strDateS) && !string.IsNullOrEmpty(strDateE))
                {
                    if (string.IsNullOrEmpty(where))
                        where += "where Trans_Date between @DateS and @DateE ";
                    else
                        where += "and Trans_Date between @DateS and @DateE ";
                }

                if (!string.IsNullOrEmpty(strDateS) && string.IsNullOrEmpty(strDateE))
                {
                    if (string.IsNullOrEmpty(where))
                        where += "where Trans_Date > @DateS ";
                    else
                        where += "and Trans_Date > @DateS ";
                }

                if (string.IsNullOrEmpty(strDateS) && !string.IsNullOrEmpty(strDateE))
                {
                    if (string.IsNullOrEmpty(where))
                        where += "where Trans_Date < @DateE ";
                    else
                        where += "and Trans_Date < @DateE ";
                }
                #endregion

                sc.CommandText += where;

                if (p_flag != "0")
                {
                    sc.CommandText += "and Process_Flag = @p_flag ";
                    SqlParameter spSC = new SqlParameter("p_flag", p_flag);
                    sc.Parameters.Add(spSC);
                }

                if (u_flag != "0")
                {
                    sc.CommandText += "and Upload_Flag = @u_flag ";
                    SqlParameter spUP = new SqlParameter("u_flag", u_flag);
                    sc.Parameters.Add(spUP);
                }

                sc.CommandText += "order by RowNumber,Modify_DateTime desc ";

                dtBalance = BRREISSUE_CARD.SearchOnDataSet(sc).Tables[0];

                return dtBalance;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool SelectLOG( string CARD_ID, string PID, DateTime workDate)
        {
            try
            {
                string date = workDate.ToString("yyyy/MM/dd");
                string where = "";
                SqlCommand sc = new SqlCommand();
                sc.CommandType = CommandType.Text;
                sc.CommandText = SQL_SEL;
                sc.CommandText += @"where CONVERT(varchar,Trans_Date,111) = CONVERT(varchar,@workDate,111) 
                                    and Process_Flag <> 'D' ";

                DataTable dtBalance = new DataTable();

                if (!string.IsNullOrEmpty(PID))
                {
                    sc.CommandText += "and PID = @PID ";
                    SqlParameter spPI = new SqlParameter("PID", PID);
                    sc.Parameters.Add(spPI);
                }

                SqlParameter spwd = new SqlParameter("workDate", date);
                sc.Parameters.Add(spwd);

                sc.CommandText += where;
                sc.CommandText += "order by RowNumber,Modify_DateTime desc ";
                dtBalance = BRREISSUE_CARD.SearchOnDataSet(sc).Tables[0];

                if (dtBalance != null && dtBalance.Rows.Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return false;
                //throw ex;
            }
        }

        public static DataTable BindAddValue(string cardNo, string pid, string ids)
        {
            try
            {
                SqlCommand sc = new SqlCommand();
                sc.CommandType = CommandType.Text;
                sc.CommandText = SQL_SEL_ADD;

                if (!string.IsNullOrEmpty(ids))
                    sc.CommandText += string.Format("or Newid in ({0}) order by RowNumber,Modify_DateTime desc,CardNo", ids);
                else
                    sc.CommandText += "order by RowNumber,Modify_DateTime desc,CardNo ";

                DataTable dtBalance = new DataTable();

                SqlParameter spCD = new SqlParameter("CardNo", cardNo);
                sc.Parameters.Add(spCD);
                SqlParameter spPI = new SqlParameter("PID", pid);
                sc.Parameters.Add(spPI);

                dtBalance = BRREISSUE_CARD.SearchOnDataSet(sc).Tables[0];

                return dtBalance;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable BindImport(DateTime start, DateTime end, string ids)
        {
            try
            {
                SqlCommand sc = new SqlCommand();
                sc.CommandType = CommandType.Text;
                sc.CommandText = @"SELECT *, ROW_NUMBER() OVER(ORDER BY Modify_DateTime DESC) AS 'RowNumber'
                                            FROM dbo.Balance_Trans WITH (NOLOCK)
                                            where Modify_DateTime between @start and @end and Upload_Flag = 'N' ";

                if (!string.IsNullOrEmpty(ids))
                    sc.CommandText += string.Format("or Newid in ({0}) ", ids);

                sc.CommandText += "order by RowNumber,Modify_DateTime desc,CardNo";

                DataTable dtBalance = new DataTable();
                SqlParameter spCD = new SqlParameter("start", start);
                sc.Parameters.Add(spCD);
                SqlParameter spPI = new SqlParameter("end", end);
                sc.Parameters.Add(spPI);

                dtBalance = BRREISSUE_CARD.SearchOnDataSet(sc).Tables[0];

                return dtBalance;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet BindNewids(string newIDs)
        {
            try
            {
                SqlCommand sc = new SqlCommand();
                sc.CommandType = CommandType.Text;
                sc.CommandText = @"SELECT *, ROW_NUMBER() OVER(ORDER BY Modify_DateTime DESC) AS 'RowNumber'
                                            FROM dbo.Balance_Trans WITH (NOLOCK)
                                            where Newid in (" + newIDs + @") and Upload_Flag = 'N'
                                            order by RowNumber,Modify_DateTime desc,CardNo";

                DataSet dsBalance = new DataSet();
                dsBalance = BRREISSUE_CARD.SearchOnDataSet(sc);

                return dsBalance;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet BindNewidsForExcel(string newIDs)
        {
            try
            {
                SqlCommand sc = new SqlCommand();
                sc.CommandType = CommandType.Text;
                sc.CommandText = @"SELECT *, ROW_NUMBER() OVER(ORDER BY Modify_DateTime DESC) AS 'RowNumber'
                                            FROM dbo.Balance_Trans WITH (NOLOCK)
                                            where Newid in (" + newIDs + @") 
                                            order by RowNumber,Modify_DateTime desc,CardNo";

                DataSet dsBalance = new DataSet();
                dsBalance = BRREISSUE_CARD.SearchOnDataSet(sc);

                return dsBalance;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable BindSearch(string cardNo, string pid)
        {
            try
            {
                string where = "";
                SqlCommand sc = new SqlCommand();
                sc.CommandType = CommandType.Text;
                sc.CommandText = @"SELECT *, ROW_NUMBER() OVER(ORDER BY Modify_DateTime DESC) AS 'RowNumber'
                                            FROM dbo.Balance_Trans WITH (NOLOCK)
                                            where Upload_Flag = 'N' ";

                DataTable dtBalance = new DataTable();

                if (!string.IsNullOrEmpty(cardNo))
                {
                    SqlParameter spCD = new SqlParameter("CardNo", cardNo);
                    sc.Parameters.Add(spCD);
                    where += "and CardNo = @CardNo ";
                }

                if (!string.IsNullOrEmpty(pid))
                {
                    SqlParameter spPI = new SqlParameter("PID", pid);
                    sc.Parameters.Add(spPI);
                    where += "and PID = @PID ";
                }

                where += "order by RowNumber,Modify_DateTime desc,CardNo ";

                sc.CommandText += where;
                //dtBalance = BRREISSUE_CARD.SearchOnDataSet(sc, iPageIndex, iPageSize, ref iTotalCount).Tables[0];
                dtBalance = BRREISSUE_CARD.SearchOnDataSet(sc).Tables[0];

                return dtBalance;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool UpdateProcess_Flag(string newID)
        {
            string strSql = @"update dbo.Balance_Trans set Process_Flag = 'D'
                                where Newid = @newID";

            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandType = CommandType.Text;
            sqlComm.CommandText = strSql;

            SqlParameter spUD = new SqlParameter("newID", newID);
            sqlComm.Parameters.Add(spUD);

            return Update(sqlComm);
        }

        public static DataTable SearchAllJob(ref string strMsgID)
        {
            try
            {
                string sql = @"select JOB_ID,JOB_ID + ' ' + DESCRIPTION as DESCRIPTION from M_AUTOJOB 
                                where FUNCTION_KEY='01' order by DESCRIPTION";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;

                DataTable dtTmp = new DataTable();
                DataSet ds = BRREISSUE_CARD.SearchOnDataSet(sqlcmd, "Connection_CSIP");
                if (null != ds && ds.Tables[0].Rows.Count > 0)
                {
                    strMsgID = string.Empty;
                    return ds.Tables[0];
                }
                else
                {
                    strMsgID = "00_00000000_030";
                    return dtTmp;
                }
            }
            catch (Exception exp)
            {
                BRREISSUE_CARD.SaveLog(exp.Message);
                strMsgID = "00_00000000_030";
                DataTable dtTmps = new DataTable();
                return dtTmps;
            }
        }

    }
}
