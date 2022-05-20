//******************************************************************
//*  作    者：趙呂梁

//*  功能說明：系統公共方法類

//*  創建日期：2009/08/19
//*  修改記錄：


//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Data;
using CSIPKeyInGUI.EntityLayer;
using System.Collections;
using CSIPCommonModel.EntityLayer;
using Framework.WebControls;
using Framework.Common.Logging;
using System.Drawing;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Message;
using System.Collections.Generic;

/// <summary>
/// 系統公共方法類
/// </summary>
public class CommonFunction : PageBase
{
   
    /// 作者 趙呂梁
    /// 創建日期：2009/09/18
    /// 修改日期：2009/09/18 
    /// <summary>
    /// 比較主機和畫面中的資料
    /// </summary>
    /// <param name="htOutput">主機資料的HashTable</param>
    /// <param name="strInputValue">畫面欄位輸入的值</param>
    /// <param name="strKeyName">主機資料欄位名稱</param>
    /*修訂程式碼的原因 增加函數返回值 Modify by chenjingxian 2009-09-18  Start */
    public static int ContrastData(Hashtable htOutput, string strInputValue, string strKeyName)
    {
        //*畫面欄位輸入的值與主機資料欄位名稱不同
        if (strInputValue.Trim() != htOutput[strKeyName].ToString().Trim())
        {
            htOutput[strKeyName] = strInputValue;
            return 1; //*正常異動
        }
        else
        {
            //*不異動主機訊息
            return 0; //*不異動
        }
    }
    /*修訂程式碼的原因增加函數返回值 Modify by chenjingxian 2009-09-18  End */

    /// <summary>
    /// 比較主機和畫面中的資料
    /// </summary>
    /// <param name="htOutput">主機資料的HashTable</param>
    /// <param name="dtblUpdate">更新主機欄位相關信息的DataTable</param>
    /// <param name="strInputValue">畫面欄位輸入的值</param>
    /// <param name="strKeyName">主機資料欄位名稱</param>
    /// <param name="strFieldName">異動欄位的中文名稱</param>
    public static int ContrastData(Hashtable htOutput, DataTable dtblUpdate, string strInputValue, string strKeyName, string strFieldName)
    {
        DataRow drowRow = dtblUpdate.NewRow();
        //*畫面欄位輸入的值與主機資料欄位名稱不同
        if (strInputValue.Trim() != htOutput[strKeyName].ToString().Trim())
        {
            drowRow[EntityCUSTOMER_LOG.M_field_name] = strFieldName;
            drowRow[EntityCUSTOMER_LOG.M_before] = htOutput[strKeyName].ToString();
            drowRow[EntityCUSTOMER_LOG.M_after] = strInputValue;
            dtblUpdate.Rows.Add(drowRow);
            htOutput[strKeyName] = strInputValue;
            return 1;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// 比較特點資料異動部分主機和畫面中的資料
    /// </summary>
    /// 修改紀錄:20210531_Ares_Stanley-增加欄位資料去空白
    /// <param name="htOutput">主機資料的HashTable</param>
    /// <param name="dtblUpdate">更新主機欄位相關信息的DataTable</param>
    /// <param name="strInputValue">畫面欄位輸入的值</param>
    /// <param name="strKeyName">主機資料欄位名稱</param>
    /// <param name="strFieldName">異動欄位的中文名稱</param>
    public static int ContrastDataTwo(Hashtable htOutput, DataTable dtblUpdate, string strInputValue, string strKeyName, string strFieldName)
    {

        DataRow drowRow = dtblUpdate.NewRow();
        if (strInputValue.ToUpper() != "X" && strInputValue.ToUpper() != "Ｘ")
        {
            if (strInputValue.Trim() != "" && strInputValue.Trim() != htOutput[strKeyName].ToString().Trim())
            {
                drowRow[EntityCUSTOMER_LOG.M_field_name] = strFieldName;
                drowRow[EntityCUSTOMER_LOG.M_before] = htOutput[strKeyName].ToString();
                drowRow[EntityCUSTOMER_LOG.M_after] = strInputValue;
                dtblUpdate.Rows.Add(drowRow);
                htOutput[strKeyName] = strInputValue;
                return 1;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            if (htOutput[strKeyName].ToString().Trim() != "")
            {
                drowRow[EntityCUSTOMER_LOG.M_field_name] = strFieldName;
                drowRow[EntityCUSTOMER_LOG.M_before] = htOutput[strKeyName].ToString();
                drowRow[EntityCUSTOMER_LOG.M_after] = "";
                dtblUpdate.Rows.Add(drowRow);
                htOutput[strKeyName] = "";
            }
            return 2;
        }
    }



    /// <summary>
    /// 比較特點資料異動部分主機和畫面中的資料
    /// </summary>
    /// <param name="htOutput">主機資料的HashTable</param>
    /// <param name="dtblUpdate">更新主機欄位相關信息的DataTable</param>
    /// <param name="strInputValue">畫面欄位輸入的值</param>
    /// <param name="strKeyName">主機資料欄位名稱</param>
    /// <param name="strFieldName">異動欄位的中文名稱</param>
    public static int ContrastDataTwoNew(Hashtable htOutput, DataTable dtblUpdate, string strInputValue, string strKeyName, string strFieldName)
    {

        DataRow drowRow = dtblUpdate.NewRow();
        if (strInputValue.ToUpper() != "X" && strInputValue.ToUpper() != "Ｘ")
        {
            //if (strInputValue != "" && strInputValue != htOutput[strKeyName].ToString().Trim())
            if (strInputValue.Trim() != htOutput[strKeyName].ToString().Trim())
            {
                drowRow[EntityCUSTOMER_LOG.M_field_name] = strFieldName;
                drowRow[EntityCUSTOMER_LOG.M_before] = htOutput[strKeyName].ToString().Trim();
                drowRow[EntityCUSTOMER_LOG.M_after] = strInputValue;
                dtblUpdate.Rows.Add(drowRow);
                htOutput[strKeyName] = strInputValue;
                return 1;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            if (htOutput[strKeyName].ToString().Trim() != "")
            {
                drowRow[EntityCUSTOMER_LOG.M_field_name] = strFieldName;
                drowRow[EntityCUSTOMER_LOG.M_before] = htOutput[strKeyName].ToString().Trim();
                drowRow[EntityCUSTOMER_LOG.M_after] = "";
                dtblUpdate.Rows.Add(drowRow);
                htOutput[strKeyName] = "";
            }
            return 2;
        }
    }



    /// <summary>
    /// 比較主機和畫面中的資料,有異動則修改，無自動則刪除
    /// </summary>
    /// <param name="htOutput">主機資料的HashTable</param>
    /// <param name="dtblUpdate">更新主機欄位相關信息的DataTable</param>
    /// <param name="strInputValue">畫面欄位輸入的值</param>
    /// <param name="strKeyName">主機資料欄位名稱</param>
    /// <param name="strFieldName">異動欄位的中文名稱</param>
    public static int ContrastDataEdit(Hashtable htOutput, DataTable dtblUpdate, string strInputValue, string strKeyName, string strFieldName)
    {
        DataRow drowRow = dtblUpdate.NewRow();
        //*畫面欄位輸入的值與主機資料欄位名稱不同
        if (strInputValue.Trim() != htOutput[strKeyName].ToString().Trim())
        {
            drowRow[EntityCUSTOMER_LOG.M_field_name] = strFieldName;
            drowRow[EntityCUSTOMER_LOG.M_before] = htOutput[strKeyName].ToString();
            drowRow[EntityCUSTOMER_LOG.M_after] = strInputValue;
            dtblUpdate.Rows.Add(drowRow);
            htOutput[strKeyName] = strInputValue;
            return 1;
        }
        else
        {

            htOutput.Remove(strKeyName);
            return 0;
        }
    }


    /// 作者 chenjingxian
    /// 創建日期：2009/09/18
    /// 修改日期：2009/09/18 
    /// <summary>
    /// 比較主機和畫面中的資料
    /// </summary>
    /// <param name="strHtgValue">主機原有的值</param>
    /// <param name="strInputValue">畫面中的輸入值</param>
    /// <param name="strFieldName">異動欄位的中文名稱</param>
    /// <param name="dtblUpdate">更新主機欄位相關信息的DataTable</param>
    /// <param name="htOutput">主機資料的HashTable</param>
    /// <param name="strKeyName">主機資料欄位名稱</param>
    public static int ContrastData(string strHtgValue, string strInputValue, string strFieldName, DataTable dtblUpdate, Hashtable htOutput, string strKeyName)
    {
        DataRow drowRow = dtblUpdate.NewRow(); //*向更新主機欄位相關信息的DataTable中新增的Row   
        //*畫面欄位輸入的值與主機資料欄位名稱不同
        if (strInputValue.Trim() != strHtgValue.Trim())
        {
            //*主機的欄位和輸入的欄位不相同，異動主機訊息
            drowRow[EntityCUSTOMER_LOG.M_field_name] = strFieldName;
            drowRow[EntityCUSTOMER_LOG.M_before] = strHtgValue;
            drowRow[EntityCUSTOMER_LOG.M_after] = strInputValue;
            dtblUpdate.Rows.Add(drowRow);
            htOutput[strKeyName] = strInputValue;
            return 1; //*正常異動
        }
        else
        {
            //*不異動主機訊息
            return 0; //*不異動
        }
    }


    /// 作者 chenjingxian
    /// 創建日期：2009/09/18
    /// 修改日期：2009/09/18 
    /// <summary>
    /// 寫入LogTable記錄
    /// </summary>
    /// <param name="strHtgValue">主機原有的值</param>
    /// <param name="strInputValue">畫面中的輸入值</param>
    /// <param name="strFieldName">異動欄位的中文名稱</param>
    public static int UpdateLog(string strHtgValue, string strInputValue, string strFieldName, DataTable dtblUpdate)
    {

        DataRow drowRow = dtblUpdate.NewRow(); //*向更新主機欄位相關信息的DataTable中新增的Row

        //*主機的欄位和輸入的欄位不相同，異動主機訊息
        drowRow[EntityCUSTOMER_LOG.M_field_name] = strFieldName;
        drowRow[EntityCUSTOMER_LOG.M_before] = strHtgValue;
        drowRow[EntityCUSTOMER_LOG.M_after] = strInputValue;
        dtblUpdate.Rows.Add(drowRow);

        return 1; //*正常異動

    }

    /// <summary>
    /// 得到存儲更新主機信息的表結構
    /// </summary>
    /// <returns>存儲主機信息表</returns>
    public static DataTable GetDataTable()
    {
        DataTable dtblUpdateData = new DataTable();
        dtblUpdateData.Columns.Add(EntityCUSTOMER_LOG.M_field_name);
        dtblUpdateData.Columns.Add(EntityCUSTOMER_LOG.M_before);
        dtblUpdateData.Columns.Add(EntityCUSTOMER_LOG.M_after);
        return dtblUpdateData;
    }

    /// <summary>
    /// 資料檔Customer_Log中寫入更改的主機欄位信息
    /// </summary>
    /// <param name="dtblUpdate">更改的主機欄位信息的DataTable</param>
    /// <param name="eAgentInfo">Session變數集合</param>
    /// <param name="strQueryKey">更改主機欄位信息的標識</param>
    /// <param name="strLogFlag">操作標識</param>
    public static bool InsertCustomerLog(DataTable dtblUpdate, EntityAGENT_INFO eAgentInfo, string strQueryKey, string strLogFlag)
    {
        try
        {
            EntityCUSTOMER_LOG eCustomerLog = new EntityCUSTOMER_LOG();
            for (int i = 0; i < dtblUpdate.Rows.Count; i++)
            {
                eCustomerLog.query_key = strQueryKey;
                eCustomerLog.trans_id = eAgentInfo.dtfunction.Rows[0][1].ToString();
                eCustomerLog.field_name = dtblUpdate.Rows[i][EntityCUSTOMER_LOG.M_field_name].ToString();
                eCustomerLog.before = dtblUpdate.Rows[i][EntityCUSTOMER_LOG.M_before].ToString();
                eCustomerLog.after = dtblUpdate.Rows[i][EntityCUSTOMER_LOG.M_after].ToString();
                eCustomerLog.user_id = eAgentInfo.agent_id;
                eCustomerLog.mod_date = DateTime.Now.ToString("yyyyMMdd");
                eCustomerLog.mod_time = DateTime.Now.ToString("HHmmss");
                eCustomerLog.log_flag = strLogFlag;
                BRCustomer_Log.AddEntity(eCustomerLog);
            }
        }
        catch(Exception ex)
        {
            Logging.Log(ex);
            return false;
        }
        return true;
    }

    /// <summary>
    /// 資料檔Customer_Log中寫入更改的主機欄位信息
    /// </summary>
    /// <param name="dtblUpdate">更改的主機欄位信息的DataTable</param>
    /// <param name="strQueryKey">更改主機欄位信息的標識</param>
    /// <param name="strLogFlag">操作標識</param>
    /// <param name="strUserId">操作人</param>
    /// <param name="strTransId">功能名稱</param>
    public static bool InsertCustomerLog(DataTable dtblUpdate, string strQueryKey, string strLogFlag, string strUserId, string strTransId)
    {
        try
        {
            EntityCUSTOMER_LOG eCustomerLog = new EntityCUSTOMER_LOG();
            for (int i = 0; i < dtblUpdate.Rows.Count; i++)
            {
                eCustomerLog.query_key = strQueryKey;
                eCustomerLog.trans_id = strTransId;
                eCustomerLog.field_name = dtblUpdate.Rows[i][EntityCUSTOMER_LOG.M_field_name].ToString();
                eCustomerLog.before = dtblUpdate.Rows[i][EntityCUSTOMER_LOG.M_before].ToString();
                eCustomerLog.after = dtblUpdate.Rows[i][EntityCUSTOMER_LOG.M_after].ToString();
                eCustomerLog.user_id = strUserId;
                eCustomerLog.mod_date = DateTime.Now.ToString("yyyyMMdd");
                eCustomerLog.mod_time = DateTime.Now.ToString("HHmmss");
                eCustomerLog.log_flag = strLogFlag;
                BRCustomer_Log.AddEntity(eCustomerLog);
            }
        }
        catch
        {
            return false;
        }
        return true;
    }

    public static bool InsertCustomerLog(DataTable dtblUpdate, EntityAGENT_INFO eAgentInfo, string strQueryKey, string strLogFlag, structPageInfo sPageInfo)
    {
        try
        {
            EntityCUSTOMER_LOG eCustomerLog = new EntityCUSTOMER_LOG();
            for (int i = 0; i < dtblUpdate.Rows.Count; i++)
            {
                eCustomerLog.query_key = strQueryKey.Trim();
                //eCustomerLog.trans_id = eAgentInfo.dtfunction.Rows[0][1].ToString();
                eCustomerLog.trans_id = sPageInfo.strPageCode.ToString().Trim();
                eCustomerLog.field_name = dtblUpdate.Rows[i][EntityCUSTOMER_LOG.M_field_name].ToString().Trim();
                eCustomerLog.before = dtblUpdate.Rows[i][EntityCUSTOMER_LOG.M_before].ToString().Trim();
                eCustomerLog.after = dtblUpdate.Rows[i][EntityCUSTOMER_LOG.M_after].ToString().Trim();
                eCustomerLog.user_id = eAgentInfo.agent_id.Trim();
                eCustomerLog.mod_date = DateTime.Now.ToString("yyyyMMdd").Trim();
                eCustomerLog.mod_time = DateTime.Now.ToString("HHmmss").Trim();
                eCustomerLog.log_flag = strLogFlag.Trim();
                BRCustomer_Log.AddEntity(eCustomerLog);
            }
        }
        catch
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 將所在panle的TextBox、Button設置為是否可用，是否清空TextBox
    /// </summary>
    /// <param name="pnlType">控件所在的PANEL</param>
    /// <param name="blnEnabled">True可用，false不可用</param>
    public static void SetControlsEnabled(CustPanel pnlType, bool blnEnabled)
    {
        foreach (System.Web.UI.Control control in pnlType.Controls)
        {
            if (control is CustTextBox)
            {
                CustTextBox txtBox = (CustTextBox)control;
                txtBox.Enabled = blnEnabled;
                txtBox.Text = "";
            }
            if (control is CustButton)
            {
                CustButton btnSubmit = (CustButton)control;
                btnSubmit.Enabled = blnEnabled;
            }
            if (control is CustDropDownList)
            {
                CustDropDownList dropDownList = (CustDropDownList)control;
                dropDownList.Enabled = blnEnabled;
            }
            if (control is CustCheckBox)
            {
                CustCheckBox chkBox = (CustCheckBox)control;
                chkBox.Enabled = blnEnabled;
            }
        }
    }

    /// <summary>
    /// 將所在panle的TextBox、Button設置為是否可用
    /// </summary>
    /// <param name="pnlType">控件所在的PANEL</param>
    /// <param name="blnEnabled">True可用，false不可用</param>
    public static void SetEnabled(CustPanel pnlType, bool blnEnabled)
    {
        foreach (System.Web.UI.Control control in pnlType.Controls)
        {
            if (control is CustTextBox)
            {
                CustTextBox txtBox = (CustTextBox)control;
                txtBox.Enabled = blnEnabled;
            }
            if (control is CustButton)
            {
                CustButton btnSubmit = (CustButton)control;
                btnSubmit.Enabled = blnEnabled;
            }
            if (control is CustDropDownList)
            {
                CustDropDownList dropDownList = (CustDropDownList)control;
                dropDownList.Enabled = blnEnabled;
            }
        }
    }

    /// <summary>
    /// 將所在panle的TextBox文字設置為指定顏色
    /// </summary>
    /// <param name="pnlType">控件所在的PANEL</param>
    /// <param name="foreColor">指定顏色</param>
    public static void SetControlsForeColor(CustPanel pnlType, Color foreColor)
    {
        foreach (System.Web.UI.Control control in pnlType.Controls)
        {
            if (control is CustTextBox)
            {
                CustTextBox txtBox = (CustTextBox)control;
                txtBox.ForeColor = foreColor;
                //2021/03/11_Ares_Stanley-修正粗框問題
                txtBox.BackColor = Color.Empty;
            }
          
        }
    }

    public static void SetCustTextBoxBKColor(CustTextBox objBox, Color BkColor)
    {
        objBox.BackColor = BkColor;
    }

    /// <summary>
    /// 分頁
    /// </summary>
    /// <param name="dtblSource">源數據表</param>
    /// <param name="intPageIndex">頁數</param>
    /// <param name="intPageSize">每頁行數</param>
    /// <returns>分頁后的數據表</returns>
    public static DataTable Pagination(DataTable dtblSource, int intPageIndex, int intPageSize)
    {
        try
        {
            if (dtblSource != null)
            {
                DataTable dtblNew = dtblSource.Clone();

                int intStartIndex = (intPageIndex - 1) * intPageSize;


                for (int i = intStartIndex; i < intPageIndex * intPageSize; i++)
                {
                    if (dtblSource.Rows.Count > i)
                    {
                        object[] objRow = new object[dtblSource.Columns.Count];
                        for (int j = 0; j < dtblSource.Columns.Count; j++)
                        {
                            objRow[j] = dtblSource.Rows[i][j];
                        }

                        dtblNew.Rows.Add(objRow);
                    }
                }
                return dtblNew;
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            return null;
        }
    }

    /// <summary>
    /// 從指定位置開始截取指定長度的字符串
    /// </summary>
    /// <param name="strValue">字符串</param>
    /// <param name="intBegin">指定位置</param>
    /// <param name="intSubLength">截取的長度</param>
    /// <returns>截取的字符串</returns>
    public static string GetSubString(string strValue, int intBegin, int intSubLength)
    {
        if (!string.IsNullOrEmpty(strValue))
        {
            if (intBegin < 0 || intSubLength < 0)
            {
                return "";
            }
            
            int intCount = strValue.Length;
            if (intCount >= intSubLength + intBegin)
            {
                return strValue = strValue.Substring(intBegin, intSubLength);
            }
            else
            {
                if (intCount > intBegin)
                {
                    return strValue = strValue.Substring(intBegin, intCount - intBegin);
                }
                else
                {
                    return "";
                }
            }
        }
        return strValue;
    }

    /// <summary>
    /// 通過P4 JCAA查詢該卡片的卡人CIF資料
    /// </summary>
    /// <param name="strCardNo">卡號</param>
    /// <param name="htResult">得到卡片的卡人CIF資料信息</param>
    /// <param name="eAgentInfo">Session變數集合</param>
    /// <param name="strMsg">返回錯誤信息</param>
    /// <returns>true成功，false失敗</returns>
    public static bool GetJCAAMainframeData(string strCardNo, ref Hashtable htResult, EntityAGENT_INFO eAgentInfo, ref string strMsg)
    {       
        int iCount = 0;
        int iRCount = 0;
        int iNum = 0;
        int iBegin = 0;
        int iUpFlag = 0;

        //*添加上傳主機信息
        Hashtable htInput = new Hashtable();
        htInput.Add("ACCT_NBR", strCardNo);
        htInput.Add("LINE_CNT", "0000");
        htInput.Add("FUNCTION_CODE", "2");

        //*得到主機傳回信息
        Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCAA, htInput, false, "1", eAgentInfo);
     
        if (!htReturn.Contains("HtgMsg"))
        {
            //htReturn["ACCT_NBR"] = htInput["ACCT_NBR"];//* for_xml_test  模擬環境測試，正式環境可以不用賦值
            if (htReturn["LINE_CNT"].ToString() != "")//*取得卡人資料筆數
            {
                iRCount = Convert.ToInt32(htReturn["LINE_CNT"].ToString());
            }
            iNum = iRCount - iCount * 20;

            if (iNum <= 20)
            {
                iUpFlag = 1;
            }
            else
            {
                iNum = 20;
            }

            while (iNum > 0)
            {
                //*為網頁欄位(list)賦值
                for (int i = 1; i < iNum + 1; i++)
                {
                    //*比對其< CARD_NMBR>為畫面中的【卡號】相同
                    if (strCardNo == htReturn["CARD_NMBR" + i.ToString()].ToString().Trim())
                    {
                        htReturn.Add("TYPE", htReturn["TYPE" + i.ToString()].ToString().Trim());
                        htReturn.Add("CARD_NMBR", htReturn["CARD_NMBR" + i.ToString()].ToString().Trim());
                        htReturn.Add("BLOCK_CODE", htReturn["BLOCK_CODE" + i.ToString()].ToString().Trim());
                        htReturn.Add("CUSTID", htReturn["CUSTID" + i.ToString()].ToString().Trim());
                        htReturn.Add("NAME1", htReturn["NAME1" + i.ToString()].ToString().Trim());//*姓名
                        htReturn.Add("USER_CODE_3", htReturn["USER_CODE_3" + i.ToString()].ToString().Trim());
                        htResult = htReturn;
                        return true;
                    }
                }

                if (iUpFlag == 1)
                {
                    //*從JCAA 回傳之 OCCUS資料中 , 其< CARD_NMBR>與畫面中的【卡號】不相同       
                    strMsg = MessageHelper.GetMessage("01_00000000_025");
                    htResult = htReturn;
                    return false;
                }

                iBegin = (iCount + 1) * 20 + 1;
                htInput["ACCT_NBR"] = strCardNo;
                htInput["LINE_CNT"] = iBegin.ToString().PadLeft(4, '0');
                htInput["FUNCTION_CODE"] = "2";

                htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCAA, htInput, false, "1", eAgentInfo);
                if (htReturn.Contains("HtgMsg"))
                {
                    strMsg = htReturn["HtgMsg"].ToString();
                    htResult = htReturn;
                    return false;
                }
                else
                {
                    //htReturn["ACCT_NBR"] = htInput["ACCT_NBR"];//* for_xml_test  模擬環境測試，正式環境可以不用賦值
                    iCount++;
                    iRCount = Convert.ToInt32(htReturn["LINE_CNT"].ToString());
                    iNum = iRCount - iCount * 20;

                    if (iNum <= 20)
                    {
                        iUpFlag = 1;
                    }
                    else
                    {
                        iNum = 20;
                    }
                }
            }

            //*從JCAA 回傳之 OCCUS資料中 , 其< CARD_NMBR>與畫面中的【卡號】不相同       
            strMsg = MessageHelper.GetMessage("01_00000000_025");
            htResult = htReturn;
            return false;       
        }
        else
        {
            strMsg = htReturn["HtgMsg"].ToString();
            htResult = htReturn;
            return false;
        }
    }

    /// <summary>
    /// 通過P4 JCAA查詢該卡片的卡人CIF資料
    /// </summary>
    /// <param name="Initial">初始值</param>
    /// <param name="htInput">傳入參數的HashTable</param>
    /// <param name="htResult">得到卡片的卡人CIF資料信息</param>
    /// <param name="eAgentInfo">Session變數集合</param>
    /// <param name="strMsg">返回錯誤信息</param>
    /// <returns>>傳出參數的HashTable</returns>
    public static bool GetJCAAMainframeData(int iCount, Hashtable htInput, ref Hashtable htResult, EntityAGENT_INFO eAgentInfo, ref string strMsg)
    {        
        int iRCount = 0;
        int iNum = 0;
        //*得到主機傳回信息
        Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCAA, htInput, false, "1", eAgentInfo);

        if (!htReturn.Contains("HtgMsg"))
        {
            htReturn["ACCT_NBR"] = htInput["ACCT_NBR"];//* for_xml_test  模擬環境測試，正式環境可以不用賦值
            if (htReturn["LINE_CNT"].ToString() != "")//*取得卡人資料筆數
            {
                iRCount = Convert.ToInt32(htReturn["LINE_CNT"].ToString());
            }
            iNum = iRCount - iCount * 20;

            while (iNum > 0)
            {
                ////接收筆數小於20就返回
                //if (iNum < 20)
                //{
                //    strMsg = MessageHelper.GetMessage("01_00000000_025");
                //    htResult = htReturn;
                //    return false;
                //}

                for (int i = 1; i < 20; i++)
                {
                    //2011-12-20 檢查是否為正卡
                    //正卡-卡號第二碼為0
                    string strCard = htReturn["USER_CODE_3" + i.ToString()] != null ? htReturn["USER_CODE_3" + i.ToString()].ToString().Trim() : "";

                    if (strCard.Length == 2)
                    {
                        if (!strCard.Contains("90") && strCard.Substring(1, 1) == "0")
                        {
                            htResult = htReturn;
                            return true;
                        }
                    }
                }
                //沒有結果且比數未跑完則繼續
                //若本次皆無正卡且未達總筆數則在查詢下一批                              
                int iBegin = (iCount + 1) * 20 + 1;
                iCount++;
                htInput["LINE_CNT"] = iBegin.ToString().PadLeft(4, '0');
                return GetJCAAMainframeData(iCount, htInput, ref htResult, eAgentInfo, ref strMsg);
            }
            strMsg = MessageHelper.GetMessage("01_01010700_011");
            htResult = htReturn;
            return false;

        }
        else
        {
            strMsg = MessageHelper.GetMessage("01_01010700_011");
            htResult = htReturn;
            return false;
        }
    }

    /// <summary>
    /// 通過P4 JCAA查詢該卡片的卡人CIF資料
    /// </summary>
    /// <param name="strCardNo">卡號</param>
    /// <param name="htResult">得到卡片的卡人CIF資料信息</param>
    /// <param name="eAgentInfo">Session變數集合</param>
    /// <param name="strMsg">返回錯誤信息</param>
    /// <returns>true成功，false失敗</returns>
    public static bool GetJCATMainframeData(string strACCT_NBR,string strCardNo,ref Hashtable htResult, EntityAGENT_INFO eAgentInfo, ref string strMsg)
    {
        int iCount = 0;
        int iRCount = 0;
        int iNum = 0;
        int iBegin = 0;
        int iUpFlag = 0;

        //*添加上傳主機信息
        Hashtable htInput = new Hashtable();
        htInput.Add("ACCT_NBR", strACCT_NBR);
        htInput.Add("LINE_CNT", "0000");
        htInput.Add("FUNCTION_CODE", "1");

        //*得到主機傳回信息
        Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCAT, htInput, false, "1", eAgentInfo);

        if (!htReturn.Contains("HtgMsg"))
        {
            if (htReturn["LINE_CNT"].ToString() != "")//*取得卡人資料筆數
            {
                iRCount = Convert.ToInt32(htReturn["LINE_CNT"].ToString());
            }
            iNum = iRCount - iCount * 20;

            if (iNum <= 20)
            {
                iUpFlag = 1;
            }
            else
            {
                iNum = 20;
            }

            while (iNum > 0)
            {
                //*為網頁欄位(list)賦值
                for (int i = 1; i < iNum + 1; i++)
                {
                    //*比對其< CARD_NMBR>為畫面中的【卡號】相同
                    if (strCardNo == htReturn["CARD_NMBR" + i.ToString()].ToString().Trim())
                    {
                        htReturn.Add("CARD_NAME", htReturn["CARD_NAME" + i.ToString()].ToString().Trim());
                        htReturn.Add("CARD_NMBR", htReturn["CARD_NMBR" + i.ToString()].ToString().Trim());
                        htReturn.Add("EMBOSSER_NAME", htReturn["EMBOSSER_NAME" + i.ToString()].ToString().Trim());
                        htReturn.Add("CM_CARD_TYPE", htReturn["CM_CARD_TYPE" + i.ToString()].ToString().Trim());
                        htReturn.Add("MEMBER_NO", htReturn["MEMBER_NO" + i.ToString()].ToString().Trim());
                        htReturn.Add("EXPIR_DATE", htReturn["EXPIR_DATE" + i.ToString()].ToString().Trim());
                        htResult = htReturn;
                        return true;
                    }
                }

                if (iUpFlag == 1)
                {
                    //*從JCAA 回傳之 OCCUS資料中 , 其< CARD_NMBR>與畫面中的【卡號】不相同       
                    strMsg = MessageHelper.GetMessage("01_00000000_025");
                    htResult = htReturn;
                    return false;
                }

                iBegin = (iCount + 1) * 20 + 1;
                htInput["ACCT_NBR"] = strACCT_NBR;
                htInput["LINE_CNT"] = iBegin.ToString().PadLeft(4, '0');
                htInput["FUNCTION_CODE"] = "1";

                htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCAT, htInput, false, "1", eAgentInfo);
                if (htReturn.Contains("HtgMsg"))
                {
                    strMsg = htReturn["HtgMsg"].ToString();
                    htResult = htReturn;
                    return false;
                }
                else
                {
                    iCount++;
                    iRCount = Convert.ToInt32(htReturn["LINE_CNT"].ToString());
                    iNum = iRCount - iCount * 20;

                    if (iNum <= 20)
                    {
                        iUpFlag = 1;
                    }
                    else
                    {
                        iNum = 20;
                    }
                }
            }

            //*從JCAA 回傳之 OCCUS資料中 , 其< CARD_NMBR>與畫面中的【卡號】不相同       
            strMsg = MessageHelper.GetMessage("01_00000000_025");
            htResult = htReturn;
            return false;
        }
        else
        {
            strMsg = htReturn["HtgMsg"].ToString();
            htResult = htReturn;
            return false;
        }
    }

    /// <summary>
    /// 克隆ViewState對象
    /// </summary>
    /// <param name="obj">對象</param>
    /// <param name="ht">Hashtbale</param>
    public static void GetViewStateHt(object obj, ref Hashtable ht)
    {
        Hashtable htTemp = (Hashtable)obj;

        ht = (Hashtable)htTemp.Clone();
    }
    
    /* ADD by CTCB-Carolyn 99.06.10
       PCAM-依據其商店代號 , 分別設定費率 STATUS 及 RATE 的呈現方式
    */
    public static void SetPCAMFieldsByShopId(string sShopID , CustPanel pnlType,bool blDefaultValue)
    {
       string strNum = GetSubString(sShopID, 2, 3);
       string sChkShopNo = GetSubString(sShopID, 2, 1);
       string sBoxID = "";
			 string sBoxID_IndexNo = "";
			 
			 foreach (System.Web.UI.Control control in pnlType.Controls){
           if (control is CustTextBox){
               CustTextBox txtBox = (CustTextBox)control;
               sBoxID = txtBox.ID;
	               if(sBoxID.IndexOf("txtStatus")>=0 || sBoxID.IndexOf("txtDisRate")>=0){
               	    if(sBoxID.IndexOf("txtStatus")>=0){
               	    	  sBoxID_IndexNo = GetSubString(sBoxID, 9, 2);
               	    }else{
               	    	  sBoxID_IndexNo = GetSubString(sBoxID, 10, 2);
               	    }
               	    if(sChkShopNo == "5"){ //分期店 , STAUS 及 RATE 欄位皆 disabled
               	    	  	txtBox.BackColor = Color.LightGray;
               	    	  	txtBox.ReadOnly  = true;
               	    }else{  //非分期店 (only P4A)
               	       		if (strNum == "251" || strNum == "252" || strNum == "253" || strNum == "254"){  //錢包店
               	       					if(sBoxID_IndexNo=="10" || sBoxID_IndexNo=="11" || sBoxID_IndexNo=="13"){
               	       						  txtBox.BackColor = Color.White;
               	       						  txtBox.ReadOnly  = false;
               	       						  if(blDefaultValue){
               	       						  	txtBox.Text = "0";
               	       						  }
               	       					}else{
               	    	  						txtBox.BackColor = Color.LightGray;
               	    	  						txtBox.ReadOnly  = true;
               	       				  }
               	       	  }else{ //一般
               	       	      if(Convert.ToInt32(sBoxID_IndexNo)<=8){
               	       						  txtBox.BackColor = Color.White;
               	       						  txtBox.ReadOnly  = false;
               	       	      	    if(blDefaultValue){
               	       	      	    	txtBox.Text = "0";
               	       	      	    }
               	       	      }else{
               	    	  						txtBox.BackColor = Color.LightGray;
               	    	  						txtBox.ReadOnly  = true;
               	       	      }
               	       	  }
               	    }
               }
           }
       }  			 
    	
    }

    /// <summary>
    /// 檢測 Email 長度
    /// </summary>
    /// <param name="mailFront">Email帳號</param>
    /// <param name="mailDomain">Email網域</param>
    /// <param name="mailLength">Email長度</param>
    /// <returns>true 符合長度限制, false 不符合長度限制</returns>
    public static bool CheckMailLength(string mailFront, string mailEnd, int mailLength = 50)
    {
        mailFront += "@";
        return (mailFront.Length + mailEnd.Length <= mailLength);
    }

}
