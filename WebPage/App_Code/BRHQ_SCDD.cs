//******************************************************************
//*  作    者：林家賜
//*  功能說明：收單特店審查處理  SCCD資料
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
/// BRAML_Cdata_Work 的摘要描述
/// </summary>
public class BRHQ_SCDD : CSIPCommonModel.BusinessRules.BRBase<EntityHQ_SCDD>
{
    public BRHQ_SCDD()
    {
    }
    /// <summary>
    /// 讀取案件明細表頭
    /// </summary>
    /// <returns></returns>
    public static EntityHQ_SCDD getSCDDData_WOrk(AML_SessionState parmObj)
    {
        string sSQL = @"  SELECT
CASE_NO,CORP_NO,NameCheck_No,NameCheck_Item,NameCheck_Note,NameCheck_RiskRanking,BusinessForeignAddress,RiskObject,Organization_Item,Organization_Note,Proof_Item,IsSanction,IsSanctionCountryCode1,IsSanctionCountryCode2,IsSanctionCountryCode3,IsSanctionCountryCode4,IsSanctionCountryCode5
FROM  [HQ_SCDD] where CASE_NO =@CASE_NO 
   ;";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandType = CommandType.Text;
        sqlcmd.CommandText = sSQL;
        sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", parmObj.CASE_NO));

        EntityHQ_SCDD rtnObj = new EntityHQ_SCDD();
        DataTable dt = SearchOnDataSet(sqlcmd).Tables[0];
        if (dt.Rows.Count > 0)
        {
            DataTableConvertor.convSingRow<EntityHQ_SCDD>(ref rtnObj, dt.Rows[0]);
        }
        return rtnObj;
    }

    /// <summary>
    /// 新增多筆
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool Insert(List<EntityHQ_SCDD> paramObj)
    {
        bool result = false;
        ///使用交易
        using (OMTransactionScope ts = new OMTransactionScope())
        {
            foreach (EntityHQ_SCDD scOBJ in paramObj)
            {
                result = Insert(scOBJ);
                if (!result)
                {
                    return false;
                }
            }
            result = true;
            ts.Complete();
        }
        return result;
    }
    /// <summary>
    /// 新增單筆
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool Insert(EntityHQ_SCDD paramObj)
    {
        bool result = false;
       
            try
            {

                string sSql = @"Insert into HQ_SCDD
(CASE_NO,CORP_NO,NameCheck_No,NameCheck_Item,NameCheck_Note,NameCheck_RiskRanking,BusinessForeignAddress,RiskObject,Organization_Item,Organization_Note,Proof_Item,IsSanction,IsSanctionCountryCode1,IsSanctionCountryCode2,IsSanctionCountryCode3,IsSanctionCountryCode4,IsSanctionCountryCode5)
VALUES(@CASE_NO,@CORP_NO,@NameCheck_No,@NameCheck_Item,@NameCheck_Note,@NameCheck_RiskRanking,@BusinessForeignAddress,@RiskObject,@Organization_Item,@Organization_Note,@Proof_Item,@IsSanction,@IsSanctionCountryCode1,@IsSanctionCountryCode2,@IsSanctionCountryCode3,@IsSanctionCountryCode4,@IsSanctionCountryCode5);
";
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
              
                sqlcmd.CommandText = sSql;

            sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", paramObj.CASE_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@CORP_NO", paramObj.CORP_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@NameCheck_No", paramObj.NameCheck_No));
            sqlcmd.Parameters.Add(new SqlParameter("@NameCheck_Item", paramObj.NameCheck_Item));
            sqlcmd.Parameters.Add(new SqlParameter("@NameCheck_Note", paramObj.NameCheck_Note));
            sqlcmd.Parameters.Add(new SqlParameter("@NameCheck_RiskRanking", paramObj.NameCheck_RiskRanking));
            sqlcmd.Parameters.Add(new SqlParameter("@BusinessForeignAddress", paramObj.BusinessForeignAddress));
            sqlcmd.Parameters.Add(new SqlParameter("@RiskObject", paramObj.RiskObject));
            sqlcmd.Parameters.Add(new SqlParameter("@Organization_Item", paramObj.Organization_Item));
            sqlcmd.Parameters.Add(new SqlParameter("@Organization_Note", paramObj.Organization_Note));
            sqlcmd.Parameters.Add(new SqlParameter("@Proof_Item", paramObj.Proof_Item));
            sqlcmd.Parameters.Add(new SqlParameter("@IsSanction", paramObj.IsSanction));
            sqlcmd.Parameters.Add(new SqlParameter("@IsSanctionCountryCode1", paramObj.IsSanctionCountryCode1));
            sqlcmd.Parameters.Add(new SqlParameter("@IsSanctionCountryCode2", paramObj.IsSanctionCountryCode2));
            sqlcmd.Parameters.Add(new SqlParameter("@IsSanctionCountryCode3", paramObj.IsSanctionCountryCode3));
            sqlcmd.Parameters.Add(new SqlParameter("@IsSanctionCountryCode4", paramObj.IsSanctionCountryCode4));
            sqlcmd.Parameters.Add(new SqlParameter("@IsSanctionCountryCode5", paramObj.IsSanctionCountryCode5));



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
    /// 新增單筆
    /// </summary>
    /// <param name="paramObj"></param>
    /// <returns></returns>
    public static bool Update(EntityHQ_SCDD paramObj)
    {
        bool result = false;
        try
        {

            string sSql = @" update HQ_SCDD 
set  NameCheck_No = @NameCheck_No,NameCheck_Item = @NameCheck_Item,NameCheck_Note = @NameCheck_Note,NameCheck_RiskRanking = @NameCheck_RiskRanking,BusinessForeignAddress = @BusinessForeignAddress,RiskObject = @RiskObject,Organization_Item = @Organization_Item,Organization_Note = @Organization_Note,Proof_Item = @Proof_Item,IsSanction = @IsSanction,IsSanctionCountryCode1 = @IsSanctionCountryCode1,IsSanctionCountryCode2 = @IsSanctionCountryCode2,IsSanctionCountryCode3 = @IsSanctionCountryCode3,IsSanctionCountryCode4 = @IsSanctionCountryCode4,IsSanctionCountryCode5 = @IsSanctionCountryCode5 
where  CASE_NO = @CASE_NO and CORP_NO = @CORP_NO ;
";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text; 
            sqlcmd.CommandText = sSql; 
            sqlcmd.Parameters.Add(new SqlParameter("@CASE_NO", paramObj.CASE_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@CORP_NO", paramObj.CORP_NO));
            sqlcmd.Parameters.Add(new SqlParameter("@NameCheck_No", paramObj.NameCheck_No));
            sqlcmd.Parameters.Add(new SqlParameter("@NameCheck_Item", paramObj.NameCheck_Item));
            sqlcmd.Parameters.Add(new SqlParameter("@NameCheck_Note", paramObj.NameCheck_Note));
            sqlcmd.Parameters.Add(new SqlParameter("@NameCheck_RiskRanking", paramObj.NameCheck_RiskRanking));
            sqlcmd.Parameters.Add(new SqlParameter("@BusinessForeignAddress", paramObj.BusinessForeignAddress));
            sqlcmd.Parameters.Add(new SqlParameter("@RiskObject", paramObj.RiskObject));
            sqlcmd.Parameters.Add(new SqlParameter("@Organization_Item", paramObj.Organization_Item));
            sqlcmd.Parameters.Add(new SqlParameter("@Organization_Note", paramObj.Organization_Note));
            sqlcmd.Parameters.Add(new SqlParameter("@Proof_Item", paramObj.Proof_Item));
            sqlcmd.Parameters.Add(new SqlParameter("@IsSanction", paramObj.IsSanction));
            sqlcmd.Parameters.Add(new SqlParameter("@IsSanctionCountryCode1", paramObj.IsSanctionCountryCode1));
            sqlcmd.Parameters.Add(new SqlParameter("@IsSanctionCountryCode2", paramObj.IsSanctionCountryCode2));
            sqlcmd.Parameters.Add(new SqlParameter("@IsSanctionCountryCode3", paramObj.IsSanctionCountryCode3));
            sqlcmd.Parameters.Add(new SqlParameter("@IsSanctionCountryCode4", paramObj.IsSanctionCountryCode4));
            sqlcmd.Parameters.Add(new SqlParameter("@IsSanctionCountryCode5", paramObj.IsSanctionCountryCode5)); 
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