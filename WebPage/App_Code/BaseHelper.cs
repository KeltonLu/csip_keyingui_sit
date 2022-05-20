//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：CS27基本操作
//*  創建日期：2009/08/07
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//* Ge.Song         2010/05/18          20090023                將現有4個子系統修改爲讀取Common中取得屬性的方法
//*******************************************************************
using System;
using System.Text;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CSIPCommonModel.BaseItem;
using Framework.Common.Utility;
using Framework.Common.Message;
using CSIPCommonModel.BusinessRules;
using System.Data.SqlClient;
using CSIPCommonModel.EntityLayer;
using Framework.Data;

/// <summary>
/// Summary description for BaseHelper
/// </summary>
public sealed class BaseHelper
{
    #region GetScript
    /// <summary>
    /// 蚚誧Session隍囮眳綴泐蛌善HomePage珜醱褐掛
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public static string GetScriptForUserSessionOut(Page page)
    {
        StringBuilder sbScript = new StringBuilder();
        sbScript.Append("alert('")
        .Append(MessageHelper.GetMessage("0040"))
        .Append("');")
        .Append("window.location.href = '")
        .Append(page.ResolveUrl("~/Default.aspx")).Append("';");
        return sbScript.ToString();
    }

    /// <summary>
    /// 壽敕赻撩甜芃陔虜珜醱
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public static string GetScriptForUserSessionOut_CloseMe(Page page)
    {
        StringBuilder sbScript = new StringBuilder();
        sbScript.Append("if(window.opener != null && window.opener != undefined)")
        .Append("window.opener.location.reload();")
        .Append("window.close();");
        return sbScript.ToString();
    }
    public static string GetScriptForCloseMeAndGotoURL(Page page, string URL)
    {
        StringBuilder sbScript = new StringBuilder();
        sbScript.Append("if(window.opener != null && window.opener != undefined)")
        .Append("window.opener.location.replace('" + URL + "');")
        .Append("window.close();");
        return sbScript.ToString();
    }


    #endregion

    #region Set Control
    /// <summary>
    /// 扢离Cancel偌聽枑尨
    /// </summary>
    /// <param name="btnCancel"></param>
    public static void SetCancelBtn(Framework.WebControls.CustButton btnCancel)
    {
        btnCancel.ConfirmMsg = MessageHelper.GetMessage("00_00000000_021");
    }


    /// <summary>
    /// 獲取控件顯示值
    /// </summary>
    /// <param name="ShowID"></param>
    public static string GetShowText(string ShowID)
    {
        return WebHelper.GetShowText(ShowID);
    }

    /// <summary>
    /// 顯示端末信息
    /// </summary>
    /// <param name="strMsgID"></param>
    public static string ClientMsgShow(string strMsgID)
    {
        // return "ClientMsgShow('" + MessageHelper.GetMessage(strMsgID) + "');";
        return "window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage(strMsgID) + "' }, '*');";
    }

    /// <summary>
    /// 顯示主機信息
    /// </summary>
    /// <param name="strMsgID"></param>
    public static string HostMsgShow(string strMsgID)
    {
        // return "HostMsgShow('" + MessageHelper.GetMessage(strMsgID) + "');";
        return "window.parent.postMessage({ func: 'HostMsgShow', data: '" + MessageHelper.GetMessage(strMsgID) + "' }, '*');";
    }

    /// <summary>
    /// 設置焦點
    /// </summary>
    /// <param name="strControlID"></param>
    public static string SetFocus(string strControlID)
    {
        return @"setTimeout(""document.getElementById('" + strControlID + @"').focus();"",250);";
    }

    /// <summary>
    /// 得到Common資料庫(CSIP)中的屬性
    /// </summary>
    /// <param name="strFunctionKey">屬性KEY</param>
    /// <param name="strPropertyKey">屬性標識</param>
    /// <param name="dtblResult">返回的DataTable</param>
    /// <returns>True查詢成功,False查詢失敗</returns>
    public static bool GetCommonProperty(string strFunctionKey, string strPropertyKey, ref DataTable dtblResult)
    {
        //* REQ-20090023 將現有4個子系統修改爲讀取Common中取得屬性的方法 Add by Ge.Song 2010/05/18 Start 
        //string strSql = "SELECT PROPERTY_CODE, PROPERTY_NAME FROM CSIP.dbo.M_PROPERTY_CODE WHERE FUNCTION_KEY= @FUNCTION_KEY AND PROPERTY_KEY= @PROPERTY_KEY ORDER BY SEQUENCE";
        //SqlCommand sqlComm = new SqlCommand();
        //sqlComm.CommandText = strSql;
        //sqlComm.CommandType = CommandType.Text;
        //SqlParameter parmFunctionKey = new SqlParameter("@FUNCTION_KEY", strFunctionKey);
        //sqlComm.Parameters.Add(parmFunctionKey);
        //SqlParameter parmPropertyKey = new SqlParameter("@PROPERTY_KEY", strPropertyKey);
        //sqlComm.Parameters.Add(parmPropertyKey);

        //DataSet dstProperty = BRM_PROPERTY_CODE.SearchOnDataSet(sqlComm);

        //if (dstProperty != null)
        //{
        //    dtblResult = dstProperty.Tables[0];
        //    return true;
        //}
        //else
        //{
        //    dtblResult = null;
        //    return false;
        //}

        //* 改為讀屬性表公共方法
        return CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty(strFunctionKey, strPropertyKey, ref dtblResult);

        //* REQ-20090023 將現有4個子系統修改爲讀取Common中取得屬性的方法 Add by Ge.Song 2010/05/18 End 
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="取得下一個營業日"></param>
    /// <param name="2014/10/20"></param>
    /// <returns></returns>
    public static DataTable GetWORK_DATE(string strFunctionKey, string strDate)
    {
        DataSet ds = new DataSet();
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.CommandText = @"select top 1 DATE_TIME 
                            from dbo.WORK_DATE
                            where DATE_TIME > @strDate
                            and WORK_FUNCTIONKEY= @strFunctionKey and IS_WORKDAY='1' ";

        SqlParameter parmBDate = new SqlParameter("@strFunctionKey", strFunctionKey);
        sqlcmd.Parameters.Add(parmBDate);

        SqlParameter parmEDate = new SqlParameter("@strDate", strDate);
        sqlcmd.Parameters.Add(parmEDate);

        DataHelper dh = new DataHelper("Connection_CSIP");
        try
        {
            ds = dh.ExecuteDataSet(sqlcmd);
        }
        catch (Exception exp)
        {
            throw exp;
        }

        return ds.Tables[0];
    }

    /// <summary>
    /// 取得JOB資料
    /// </summary>
    /// <param name="strFunctionKey">屬性KEY</param>
    /// <param name="strMsgID">錯誤信息</param>
    /// <returns>DataTable</returns>
    public static DataTable GetJobData(string strFunctionKey, ref string strMsgID)
    {
        SqlCommand sqlcmd = new SqlCommand();
        //2021/03/17_Ares_Stanley-DB名稱改為變數
        sqlcmd.CommandText = string.Format(@"SELECT RUN_SECONDS , RUN_MINUTES,RUN_HOURS,RUN_DAY_OF_MONTH,RUN_MONTH,RUN_DAY_OF_WEEK,EXEC_PROG, STATUS, RUN_USER_LDAPID,RUN_USER_LDAPPWD, RUN_USER_RACFID,RUN_USER_RACFPWD,MAIL_TO,DESCRIPTION,CHANGED_USER,CONVERT(varchar, CHANGED_TIME, 120 ) as  CHANGED_TIME,JOB_ID  FROM  {0}.dbo.M_AUTOJOB WHERE   FUNCTION_KEY= @FUNCTION_KEY ", UtilHelper.GetAppSettings("DB_CSIP"));

        sqlcmd.CommandType = CommandType.Text;
        SqlParameter parmKey = new SqlParameter("@FUNCTION_KEY", strFunctionKey);

        sqlcmd.Parameters.Add(parmKey);

        DataSet dstProperty = BRM_AUTOJOB.SearchOnDataSet(sqlcmd);
        if (dstProperty != null)
        {
            return dstProperty.Tables[0];
        }
        else
        {
            strMsgID = "00_00000000_000";
            return null;
        }
    }

    /// <summary>
    /// 判斷Name數否存在於M_PROPERTY_CODE中
    /// </summary>
    /// <param name="strFunctionKey">屬性KEY</param>
    /// <param name="strPropertyKey">屬性標識</param>
    /// <param name="strPropertyName">屬性名稱</param>
    /// <returns>返回查詢的DataSet</returns>
    public static DataSet CheckCommonPropertySet(string strFunctionKey, string strPropertyKey, string strPropertyName)
    {
        //* REQ-20090023 將現有4個子系統修改爲讀取Common中取得屬性的方法 Add by Ge.Song 2010/05/18 Start
        //string strSql = "SELECT PROPERTY_NAME FROM CSIP.dbo.M_PROPERTY_CODE WHERE FUNCTION_KEY= @FUNCTION_KEY AND PROPERTY_KEY= @PROPERTY_KEY and PROPERTY_NAME=@PROPERTY_NAME ORDER BY SEQUENCE";
        //SqlCommand sqlComm = new SqlCommand();
        //sqlComm.CommandText = strSql;
        //sqlComm.CommandType = CommandType.Text;
        //SqlParameter parmFunctionKey = new SqlParameter("@FUNCTION_KEY", strFunctionKey);
        //sqlComm.Parameters.Add(parmFunctionKey);
        //SqlParameter parmPropertyKey = new SqlParameter("@PROPERTY_KEY", strPropertyKey);
        //sqlComm.Parameters.Add(parmPropertyKey);
        //SqlParameter parmPropertyName = new SqlParameter("@PROPERTY_NAME", strPropertyName);
        //sqlComm.Parameters.Add(parmPropertyName);

        //return BRM_PROPERTY_CODE.SearchOnDataSet(sqlComm);

        DataTable dtblResult = null;
        if (!CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty(strFunctionKey, strPropertyKey, ref dtblResult))
        {
            //* 失敗
            return null;
        }

        DataView dvResult = dtblResult.DefaultView;
        dvResult.RowFilter = " PROPERTY_NAME = '" + strPropertyName+ "'";
        DataSet dtsResult = new DataSet();
        dtsResult.Tables.Add(dvResult.ToTable());
        return dtsResult;

        //* REQ-20090023 將現有4個子系統修改爲讀取Common中取得屬性的方法 Add by Ge.Song 2010/05/18 End
    }

    /// <summary>
    /// 查找屬性名稱
    /// </summary>
    /// <param name="strFunctionKey">功能KEY</param>
    /// <param name="strPropertyKey">屬性標識</param>
    /// <param name="strPropertyKey">屬性Key</param>
    /// <returns>返回查詢的DataSet</returns>
    public static DataSet GetCommonPropertySet(string strFunctionKey, string strPropertyKey, string strPropertyCode)
    {
        //* REQ-20090023 將現有4個子系統修改爲讀取Common中取得屬性的方法 Add by Ge.Song 2010/05/18 Start
        //string strSql = "SELECT PROPERTY_NAME FROM CSIP.dbo.M_PROPERTY_CODE WHERE FUNCTION_KEY= @FUNCTION_KEY AND PROPERTY_KEY= @PROPERTY_KEY and PROPERTY_CODE=@PROPERTY_CODE";
        //SqlCommand sqlComm = new SqlCommand();
        //sqlComm.CommandText = strSql;
        //sqlComm.CommandType = CommandType.Text;
        //SqlParameter parmFunctionKey = new SqlParameter("@FUNCTION_KEY", strFunctionKey);
        //sqlComm.Parameters.Add(parmFunctionKey);
        //SqlParameter parmPropertyKey = new SqlParameter("@PROPERTY_KEY", strPropertyKey);
        //sqlComm.Parameters.Add(parmPropertyKey);
        //SqlParameter parmPropertyCode = new SqlParameter("@PROPERTY_CODE", strPropertyCode);
        //sqlComm.Parameters.Add(parmPropertyCode);
        //return BRM_PROPERTY_CODE.SearchOnDataSet(sqlComm);

        DataTable dtblResult = null;
        if (!CSIPCommonModel.BusinessRules.BRM_PROPERTY_KEY.GetEnableProperty(strFunctionKey, strPropertyKey, ref dtblResult))
        {
            //* 失敗
            return null;
        }

        DataView dvResult = dtblResult.DefaultView;
        dvResult.RowFilter = " PROPERTY_CODE = '" + strPropertyCode + "'";
        DataSet dtsResult = new DataSet();
        dtsResult.Tables.Add(dvResult.ToTable());
        return dtsResult;

        //* REQ-20090023 將現有4個子系統修改爲讀取Common中取得屬性的方法 Add by Ge.Song 2010/05/18 End
    }


    /// <summary>
    /// 在M_PROPERTY_CODE中匯入銀行代碼屬性
    /// </summary>
    /// <param name="strPropertyKey">屬性標識</param>
    /// <param name="strPropertyCode">屬性代碼</param>
    /// <param name="strPropertyName">屬性名稱</param>
    /// <param name="intIndex">屬性序號</param>
    /// <param name="strChangeUser">操作人</param>
    /// <returns>True成功,False失敗</returns>
    public static bool InsertBankCode(string strPropertyKey, string strPropertyCode, string strPropertyName, int intIndex, string strChangeUser)
    {
        EntityM_PROPERTY_CODE ePropertyCode = new EntityM_PROPERTY_CODE();
        ePropertyCode.CHANGED_TIME = DateTime.Now;
        ePropertyCode.CHANGED_USER = strChangeUser;
        ePropertyCode.FUNCTION_KEY = "01";
        ePropertyCode.PROPERTY_CODE = strPropertyCode;
        ePropertyCode.PROPERTY_KEY = strPropertyKey;
        ePropertyCode.PROPERTY_NAME = strPropertyName;
        ePropertyCode.SEQUENCE = intIndex;
        //* REQ-20090023 將現有4個子系統修改爲讀取Common中取得屬性的方法 Add by Ge.Song 2010/05/18 Start
        ePropertyCode.OFF_FLAG = "1";
        //* REQ-20090023 將現有4個子系統修改爲讀取Common中取得屬性的方法 Add by Ge.Song 2010/05/18 End
        return BRBase<EntityM_PROPERTY_CODE>.AddNewEntity(ePropertyCode, "Connection_CSIP");
    }

    /// <summary>
    /// 刪除M_PROPERTY_CODE中匯入銀行代碼屬性
    /// </summary>
    /// <returns>True成功,False失敗</returns>
    public static bool DeleBankCode()
    {
        string strSql = "DELETE FROM M_PROPERTY_CODE WHERE FUNCTION_KEY='01' AND (PROPERTY_KEY='16' OR PROPERTY_KEY='17' OR PROPERTY_KEY='18')";
        SqlCommand sqlComm = new SqlCommand();
        sqlComm.CommandText = strSql;
        sqlComm.CommandType = CommandType.Text;

        return BRBase<EntityM_PROPERTY_CODE>.Delete(sqlComm, "Connection_CSIP");
    }

    /// <summary>
    /// 更新M_PROPERTY_KEY資料庫
    /// </summary>
    /// <param name="strFunctionKey">屬性KEY</param>
    /// <param name="strPropertyKey">屬性標識</param>
    /// <param name="strChangeUser">操作人</param>
    /// <returns>True成功,False失敗</returns>
    public static bool UpdateBankCodeLog(string strFunctionKey, string strPropertyKey, string strChangeUser)
    {
        EntityM_PROPERTY_KEY ePropertyKey = new EntityM_PROPERTY_KEY();
        ePropertyKey.CHANGED_TIME = DateTime.Now;
        ePropertyKey.CHANGED_USER = strChangeUser;
        ePropertyKey.FUNCTION_KEY = strFunctionKey;
        ePropertyKey.PROPERTY_KEY = strPropertyKey;
        string[] strFields = { EntityM_PROPERTY_KEY.M_CHANGED_TIME, EntityM_PROPERTY_KEY.M_CHANGED_USER };
        return BRBase<EntityM_PROPERTY_KEY>.UpdateEntityByCondition(null, ePropertyKey, "Connection_CSIP", strFields);
    }

    /// <summary>
    /// 更新M_PROPERTY_KEY資料庫
    /// </summary>
    /// <param name="strFunctionKey">屬性KEY</param>
    /// <param name="strPropertyKey">屬性標識</param>
    /// <param name="strChangeUser">操作人</param>
    /// <returns>True成功,False失敗</returns>
    public static bool UpdateBankCodeLog(string strChangeUser)
    {
        string strSql = "UPDATE M_PROPERTY_KEY SET CHANGED_USER = @CHANGED_USER, CHANGED_TIME = GETDATE() WHERE     FUNCTION_KEY = '01' AND (PROPERTY_KEY='16' OR PROPERTY_KEY='17' OR PROPERTY_KEY='18')";
        SqlCommand sqlComm = new SqlCommand();
        sqlComm.CommandText = strSql;
        sqlComm.CommandType = CommandType.Text;
        SqlParameter parmChangeUser = new SqlParameter("@CHANGED_USER", strChangeUser);
        sqlComm.Parameters.Add(parmChangeUser);
        return BRBase<EntityM_PROPERTY_KEY>.Update(sqlComm, "Connection_CSIP");
    }

    /// <summary>
    /// 根據默認的編碼取得字符串的字節長度
    /// </summary>
    /// <param name="text">字符串對象</param>
    /// <returns>int</returns>
    public static int GetByteLength(string text)
    {
        return System.Text.Encoding.Default.GetBytes(text).Length;
    }

    #endregion
}
