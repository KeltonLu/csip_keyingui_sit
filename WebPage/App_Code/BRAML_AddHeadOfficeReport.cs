//******************************************************************
//*  作    者：林家賜
//*  功能說明：收單特店審查處理  總公司資料建檔回報
//*  創建日期：2019/01/24
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************

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
/// BRAML_HeadOffice 的摘要描述
/// </summary>
public class BRAML_AddHeadOfficeReport : CSIPCommonModel.BusinessRules.BRBase<EntityAML_AddHeadOfficeReport>
{
    public BRAML_AddHeadOfficeReport()
    {
    }

    // 取尚未建立總公司資料
    public static DataSet GetAddHeadOfficeReport(string sDate, string eDate, string taxID, int intPageIndex, int intPageSize, ref int intTotolCount)
    {
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"
SELECT ID, TaxID, Branch_TaxID, Branch_No, Recv_no, Create_day, ISNULL(OfficeAdd_day, '') OfficeAdd_day 
FROM [dbo].[AML_AddHeadOfficeReport] WITH(NOLOCK)
WHERE OfficeAdd_day IS NULL
	AND (((ISNULL(@SDate, '') = '' ) OR (ISNULL(@EDate, '') = '' )
	OR (Create_day >= @SDate AND Create_day <= @EDate))
	AND (TaxID = @TaxID OR ISNULL(@TaxID, '') = ''))";
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.Parameters.Add(new SqlParameter("@SDate", sDate));
        sqlcmd.Parameters.Add(new SqlParameter("@EDate", eDate));
        sqlcmd.Parameters.Add(new SqlParameter("@TaxID", taxID));

        return BRAML_AddHeadOfficeReport.SearchOnDataSet(sqlcmd, intPageIndex, intPageSize, ref intTotolCount);
    }

    public static EntityAML_AddHeadOfficeReport Query(string BasicTaxID, string ID, string keyinFlag)
    {
        string sql = @"
  select ID,TaxID,Branch_TaxID,Branch_No,Recv_no,Create_day,OfficeAdd_day
 FROM  [AML_AddHeadOfficeReport]
 WHERE BasicTaxID = @BasicTaxID  ";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sql;
        sqlcmd.Parameters.Add(new SqlParameter("@BasicTaxID", BasicTaxID));
        //   sqlcmd.Parameters.Add(new SqlParameter("@ID", ID)); //暫未使用

        try
        {
            DataTable dt = new DataTable();
            DataSet DS = SearchOnDataSet(sqlcmd);
            if (DS != null && DS.Tables.Count > 0)
            {
                dt = DS.Tables[0];
            }
            EntityAML_AddHeadOfficeReport rtn = new EntityAML_AddHeadOfficeReport();
            DataTableConvertor.convSingRow<EntityAML_AddHeadOfficeReport>(ref rtn, dt.Rows[0]);

            return rtn;
        }
        catch (Exception ex)
        {
            Logging.Log("查詢公司資料建檔回報資料失敗：" + ex, LogLayer.BusinessRule);
            return null;
        }


    }
    /// <summary>
    /// 更新建檔回報的建檔日
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool Update(EntityAML_AddHeadOfficeReport paramObj)
    {
        bool result = false;
        string sql = @"Update AML_AddHeadOfficeReport set OfficeAdd_day = @OfficeAdd_day
        where TaxID = @TaxID   ; 
        ";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sql; 
        sqlcmd.Parameters.Add(new SqlParameter("@TaxID", paramObj.TaxID)); 
        sqlcmd.Parameters.Add(new SqlParameter("@OfficeAdd_day", paramObj.OfficeAdd_day));
        try
        {
            result = Update(sqlcmd);
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


