using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Framework.Data.OM;
using CSIPNewInvoice.EntityLayer_new;
using CSIPKeyInGUI.BusinessRules;
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.BusinessRules;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using System.IO;
using System.Configuration;
using Framework.Common;
using Framework.Common.Logging;
using System.Reflection;

/// <summary>
/// BRAML_CASE_DATA 的摘要描述
/// </summary>
public class BRAML_CASE_DATA : CSIPCommonModel.BusinessRules.BRBase<Entity_AML_CASE_DATA>
{
    public BRAML_CASE_DATA()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 案件放行時，把AML_CASE_DATA的Close_Date填入Now
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool Update_CaseDataCloseDate(string caseno)
    {
        bool result = false;

        try
        {

            string sSql = @"Update AML_CASE_DATA set Close_Date=@Close_Date
                                Where CASE_NO=@CASE_NO";

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sSql;

            sqlcmd.Parameters.Add(new SqlParameter("@Close_Date", DateTime.Now));
            sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", caseno));
            result = Add(sqlcmd);
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
        }
        finally
        {

        }

        return result;
    }

    /// <summary>
    /// 讀取案件明細
    /// </summary>
    /// <returns></returns>
    public static Entity_AML_CASE_DATA getAML_CASE_DATA(AML_SessionState parmObj)
    {


        string sSQL = @" SELECT ID,CASE_NO,CUST_ID,CUST_NAME,[Source],RISK_RANKING,CASE_TYPE,[STATUS],Create_YM,Create_Date,Due_Date,CTI_Send_Date,CTI_Return_Date,Pockii_Send_Date,Pockii_Return_Date
                                        ,RMM_UserID,Last_Date,Trans_Date,Assign_UserID,Incorporated,Incorporated_Date,Pockii_Closed,Close_Date 
                                          FROM dbo.AML_CASE_DATA
                                          WHERE 1=1 ";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        if (parmObj != null)
        {
            if (!String.IsNullOrEmpty(parmObj.CASE_NO))
            {
                sSQL += " AND CASE_NO = @CASE_NO ";
                sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", parmObj.CASE_NO));
            }
        }

        sqlcmd.CommandText = sSQL;

        Entity_AML_CASE_DATA rtnObj = new Entity_AML_CASE_DATA();
        DataTable dt = new DataTable();
        DataSet DS = SearchOnDataSet(sqlcmd);
        if (DS != null && DS.Tables.Count > 0)
        {
            dt = DS.Tables[0];
        }
        if (dt.Rows.Count > 0)
        {
            DataTableConvertor.convSingRow<Entity_AML_CASE_DATA>(ref rtnObj, dt.Rows[0]);
        }
        return rtnObj;
    }

    /// <summary>
    /// 新增單筆
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool InsertBySingle(Entity_AML_CASE_DATA paramObj)
    {
        bool result = false;

        try
        {

            string sSql = @"INSERT INTO dbo.AML_CASE_DATA
                                           (CASE_NO
                                           ,CUST_ID
                                           ,CUST_NAME
                                           ,Source
                                           ,RISK_RANKING
                                           ,CASE_TYPE
                                           ,STATUS
                                           ,Create_YM
                                           ,Create_Date
                                           ,Due_Date
                                           )  
                                            VALUES(@CASE_NO,@CUST_ID,@CUST_NAME,@Source,@RISK_RANKING,@CASE_TYPE,@STATUS,@Create_YM,@Create_Date,@Due_Date);
                                        ";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = sSql;

            sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", paramObj.CASE_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@CUST_ID", paramObj.CUST_ID));
            sqlcmd.Parameters.Add(new SqlParameter("@CUST_NAME", paramObj.CUST_NAME));
            sqlcmd.Parameters.Add(new SqlParameter("@Source", paramObj.Source));
            sqlcmd.Parameters.Add(new SqlParameter("@RISK_RANKING", paramObj.RISK_RANKING));
            sqlcmd.Parameters.Add(new SqlParameter("@CASE_TYPE", paramObj.CASE_TYPE));
            sqlcmd.Parameters.Add(new SqlParameter("@STATUS", paramObj.STATUS));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_YM", paramObj.Create_YM));
            sqlcmd.Parameters.Add(new SqlParameter("@Create_Date", paramObj.Create_Date));
            sqlcmd.Parameters.Add(new SqlParameter("@Due_Date", paramObj.Due_Date));

            result = Add(sqlcmd);

        }
        catch (Exception ex)
        {
            Logging.Log(ex);
        }
        finally
        {

        }

        return result;
    }
}
