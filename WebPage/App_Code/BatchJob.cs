//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：ACH批次作業
//*  創建日期：2009/10/16
//*  修改記錄：

//*<author>            <time>               <TaskID>                <desc>
//* Ares_Luke          2020/12/04           20200031-CSIP EOS       新增Log機制與調整FunctionKey用法。
//* Ares_Stanley       2021/11/01           20200031-CSIP EOS       理專十誡，移除異動Email、電子賬單相關。
//* Ares_jhun          2022/09/19           RQ-2022-019375-000      EDDA需求調整：停發PCTI電文統一週期件處理(withholding.txt)
//*******************************************************************
using System;
using System.Text;
using Quartz;
using System.Collections;
using System.Data;
using CSIPKeyInGUI.EntityLayer;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BusinessRules;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Logging;
using Framework.Common.Message;

/// <summary>
/// 批次作業
/// </summary>    
class BatchJob : Quartz.IJob
{
    private JobHelper JobHelper = new JobHelper();
    private string strFunctionKey = "01";

    private string strSessionId;
    private string strMail;
    private string strTitle;
    private string strJobID;
    DateTime dateStart;
    
    /// 作者 趙呂梁
    /// 創建日期：2009/10/16
    /// 修改日期：2022/09/19
    public void Execute(Quartz.JobExecutionContext context)
    {
        int intTotal = 0;       // 記錄批次總筆數
        int intCount1 = 0;      // 電話無異動，「不用」發JCDK電文
        int intCount2 = 0;      // 電文查詢第二卡人檔失敗
        int intCount3 = 0;      // 電話有異動，發JCDK電文「成功」
        int intCount4 = 0;      // 電話有異動，發JCDK電文「失敗」
        int intError = 0;       // 其他異常資料
        
        string strMsgID = "";
        string strBankAccNo = "";
        string strApplyType = "";
        string strKeyInFlag = "";
        string strBankCusNoID = "";
        string strPayWay = "";  // 扣繳方式
        string strMobilePhone = ""; // 行動電話
        string strDeal_S_No = "";

        dateStart = DateTime.Now;//*批次開始執行時間
        
        try
        {
            JobDataMap jobDataMap = context.JobDetail.JobDataMap;
            strMail=jobDataMap.GetString("mail").Trim();
            strTitle = jobDataMap.GetString("title").Trim();
            strJobID = jobDataMap.GetString("jobid").Trim();

            JobHelper.strJobID = strJobID;
            JobHelper.SaveLog(strJobID + "JOB啟動", LogState.Info);

            //*查詢資料檔Job_Status，查看是否上次作業還未停止
            DataTable dtInfo = BRL_BATCH_LOG.GetRunningDate(strFunctionKey, strJobID, "R", ref strMsgID);
            if (dtInfo == null)
            {
                JobHelper.SaveLog("JOB 工作狀態為：正在執行！", LogState.Info);
                return;
            }
            if (dtInfo.Rows.Count > 0)
            {
                JobHelper.SaveLog("JOB 工作狀態為：正在執行！", LogState.Info);
                return;
            }

            //*開始批次作業
            if (!InsertNewBatch())
            {
                return;
            }

            //*取得需要異動主機欄位的資料總筆數 
            intTotal = BROTHER_BANK_TEMP.SelectProcessTotalCount();
            if (intTotal == 0)
            {
                //*批次完成記錄LOG信息
                InsertBatchLog(0, 0, 0, 0, 0, 0, "", dateStart, strDeal_S_No);
                JobHelper.SaveLog("總筆數:0筆。", LogState.Info);
                return;
            }

            //*得到需要異動主機欄位的資料
            DataSet dstAch = BROTHER_BANK_TEMP.SelectReceiveList();

            ArrayList arrayListAch = new ArrayList();
            for (int i = 0; i < intTotal; i++)
            {
                arrayListAch.Add(dstAch.Tables[0].Rows[i][0].ToString().Trim());
            }

            strSessionId = "";
            foreach (string strReceiveNumber in arrayListAch)
            {
                System.Threading.Thread.Sleep(100);
                BROTHER_BANK_TEMP.UpdatePcmcUploadFlag(strReceiveNumber, "1");
                
                DataSet dstBatchInfo = BROTHER_BANK_TEMP.SelectBatchInfo(strReceiveNumber);
                for(int j = 0; dstBatchInfo != null && j < dstBatchInfo.Tables[0].Rows.Count; j++)
                {
                    DataRow dataRow = dstBatchInfo.Tables[0].Rows[j];
                    //*資料庫中的扣繳帳號
                    strBankAccNo = dataRow[EntityOTHER_BANK_TEMP.M_Other_Bank_Code_S].ToString().Trim() + "-" + dataRow[EntityOTHER_BANK_TEMP.M_Other_Bank_Acc_No].ToString().Trim();
                    //*資料庫中的申請類別
                    strApplyType = dataRow[EntityOTHER_BANK_TEMP.M_Apply_Type].ToString().Trim();
                    //*資料庫中的鍵檔來源
                    strKeyInFlag = dataRow[EntityOTHER_BANK_TEMP.M_KeyIn_Flag].ToString().Trim();
                    //*資料庫中的帳戶ID
                    strBankCusNoID = dataRow[EntityOTHER_BANK_TEMP.M_Other_Bank_Cus_ID].ToString().Trim();
                    //*資料檔中的繳款狀況
                    strPayWay = dataRow[EntityOTHER_BANK_TEMP.M_Other_Bank_Pay_Way].ToString().Trim();
                    //*資料檔中的行動電話
                    strMobilePhone = dataRow[EntityOTHER_BANK_TEMP.M_Mobile_Phone].ToString().Trim();
                    //*資料檔中的Deal_S_No (for P02D unique 識別使用)
                    strDeal_S_No = dataRow[EntityOTHER_BANK_TEMP.M_Deal_S_No].ToString().Trim();

                    //*申請類別若是O類執行批次作業時要作踢退
                    if (strApplyType == "O")
                    {
                        BROTHER_BANK_TEMP.UpdatePcmcReturnCodeAndC1342ReturnCode("ERROR:O", strReceiveNumber, "", strDeal_S_No);
                        intError++; // 其他異常資料筆數
                        continue;
                    }

                    //*若自扣申請類別為D且來源為GUI使用者鍵檔(不為P02D) 視為回貼主機失敗，不回貼主機
                    if (strApplyType == "D" && strKeyInFlag != "0")
                    {
                        BROTHER_BANK_TEMP.UpdatePcmcReturnCodeAndC1342ReturnCode("ERROR:10", strReceiveNumber, "", strDeal_S_No);
                        intError++; // 其他異常資料筆數
                        continue;
                    }
                    
                    // 申請類別不是「刪除」且 不是自扣案件終止
                    if (strApplyType != "D" && strKeyInFlag != "0")
                    {
                        //*檢核銀行代碼是否輸入正確
                        string strOtherBankCodeS = dataRow[EntityOTHER_BANK_TEMP.M_Other_Bank_Code_S].ToString().Trim();
                        DataSet dsInfo = BaseHelper.CheckCommonPropertySet(strFunctionKey, "18", strOtherBankCodeS);

                        if (dsInfo == null)
                        {
                            //*查詢資料庫時發生錯誤
                            BROTHER_BANK_TEMP.UpdatePcmcReturnCodeAndC1342ReturnCode("ERROR:8", strReceiveNumber, "", strDeal_S_No);
                            intError++; // 其他異常資料筆數
                            continue;
                        }

                        if (dsInfo.Tables[0].Rows.Count == 0 || strOtherBankCodeS == "042" || strOtherBankCodeS == "701")
                        {
                            BROTHER_BANK_TEMP.UpdatePcmcReturnCodeAndC1342ReturnCode("ERROR:8", strReceiveNumber, "", strDeal_S_No);
                            intError++; // 其他異常資料筆數
                            continue;
                        }
                    }

                    //*身分證號碼
                    string strCusId = dataRow[EntityOTHER_BANK_TEMP.M_Cus_ID].ToString().Trim();
                    
                    //*USER_ID
                    string strUserId = GetAgentInfo(context).agent_id.Trim();

                    //*得到第二卡人檔資料
                    Hashtable htTwoCardResult = new Hashtable();
                    // 第二卡人檔「查詢」狀態
                    bool jcdkQuery = GetTwoCardData(strCusId, strReceiveNumber, strDeal_S_No, ref htTwoCardResult, context, ref strSessionId);
                    // 第二卡人檔「更新」狀態
                    bool jcdkUpdate = true;

                    #region 第二卡人檔資料，比對行動電話若有異動則發電文JCDK更新

                    string c1342ReturnCode = string.Empty;
                    
                    bool mobilePhoneIsDiff = false; // 行動電話是否有異動
                    if (jcdkQuery) // 若查詢第二卡人檔沒有發生錯誤
                    {
                        //*使用"JCDK"異動主機欄位<行動電話>
                        DataTable dtUpdateTwoCard = CommonFunction.GetDataTable();

                        //*比對<行動電話>
                        CommonFunction.ContrastData(htTwoCardResult, dtUpdateTwoCard, strMobilePhone, "MOBILE_PHONE", "行動電話");

                        GetNewHashTable(htTwoCardResult, "sessionId", strSessionId);

                        if (dtUpdateTwoCard.Rows.Count > 0) // 行動電話比對後有異動
                        {
                            mobilePhoneIsDiff = true;
                            
                            htTwoCardResult["FUNCTION_CODE"] = 2;
                            //*異動主機欄位
                            Hashtable htResultJCDK = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCDK, htTwoCardResult, false, "200", GetAgentInfo(context), strJobID);

                            if (!htResultJCDK.Contains("HtgMsg"))
                            {
                                //*更新資料庫欄位
                                c1342ReturnCode = "0001";
                                BROTHER_BANK_TEMP.UpdateC1342ReturnCode(strReceiveNumber, c1342ReturnCode);
                                //*獲得主機sessionId
                                strSessionId = htResultJCDK["sessionId"].ToString();
                                //*將異動的欄位寫入 customer_log
                                if (!InsertCustomerLog(dtUpdateTwoCard, strCusId, "P4", strUserId, "01010600", strReceiveNumber))
                                {
                                    Logging.Log(strCusId + ":" + MessageHelper.GetMessage("00_00000000_017"), strJobID, LogState.Error);
                                }
                            }
                            else if (htResultJCDK["HtgMsgFlag"].ToString() == "0")
                            {
                                jcdkUpdate = false; // 電文JCDK更新「行動電話」失敗
                                
                                //*更新資料庫欄位
                                if (htResultJCDK["MESSAGE_TYPE"].ToString().Trim() == "9999" || htResultJCDK["MESSAGE_TYPE"].ToString().Trim() == "8888" || htResultJCDK["MESSAGE_TYPE"].ToString().Trim() == "8001")
                                {
                                    c1342ReturnCode = htResultJCDK["MESSAGE_TYPE"].ToString().Trim();
                                    BROTHER_BANK_TEMP.UpdateC1342ReturnCode(strReceiveNumber, c1342ReturnCode);
                                }
                                else
                                {
                                    c1342ReturnCode = "ERROR:1:" + htResultJCDK["MESSAGE_TYPE"].ToString().Trim();
                                    BROTHER_BANK_TEMP.UpdateC1342ReturnCode(strReceiveNumber, c1342ReturnCode);
                                }
                                
                                // 即使電文發生錯誤，後續流程仍需進行
                            }
                            else
                            {
                                jcdkUpdate = false; // 電文JCDK更新「行動電話」失敗
                                
                                // 即使電文發生錯誤，後續流程仍需進行
                            }
                        }
                    }
                    
                    #endregion

                    // 判斷電話異動與JCDK電文狀態
                    string pcmcReturnCode;
                    if (!jcdkQuery)
                    {
                        pcmcReturnCode = "9002";
                        intCount2++; // 電文查詢第二卡人檔失敗
                    }
                    else if (mobilePhoneIsDiff)
                    {
                        if (jcdkUpdate)
                        {
                            pcmcReturnCode = "9000";
                            intCount3++; // 電話有異動，發JCDK電文更新「成功」
                        }
                        else
                        {
                            pcmcReturnCode = "9001";
                            intCount4++; // 電話有異動，發JCDK電文更新「失敗」
                        }
                    }
                    else
                    {
                        pcmcReturnCode = "9000";
                        intCount1++; // 電話無異動，「不用」發JCDK電文
                    }
                    
                    // 更新Pcmc_Return_Code
                    // 9000	週期件
                    // 9001	週期件(電話更新失敗)
                    // 9002	週期件(電文查詢第二卡人檔失敗)
                    BROTHER_BANK_TEMP.UpdatePcmcReturnCodeAndC1342ReturnCode(pcmcReturnCode, strReceiveNumber, c1342ReturnCode, strDeal_S_No);
                    
                    // 更新Other_Bank_Temp上傳處理狀態
                    BROTHER_BANK_TEMP.UpdateACHhold("1", strReceiveNumber, strDeal_S_No);
                    
                    // 將「扣繳帳號、DD_ID、扣繳方式」寫入 customer_log
                    DataTable logData = CommonFunction.GetDataTable();
                    CommonFunction.UpdateLog("", strBankAccNo, "扣繳帳號", logData);
                    CommonFunction.UpdateLog("", strPayWay, "繳款狀況", logData);
                    CommonFunction.UpdateLog("", strBankCusNoID, "帳戶ID", logData);
                    if (!InsertCustomerLog(logData, strCusId, "P4", strUserId, "01010600",strReceiveNumber))
                    {
                        Logging.Log(strCusId + ":" + MessageHelper.GetMessage("00_00000000_017"), strJobID, LogState.Error);
                    }
                }
            }

            //*批次完成記錄LOG信息
            InsertBatchLog(intTotal, intCount1, intCount2, intCount3, intCount4, intError, "", dateStart, strDeal_S_No);
            JobHelper.SaveLog("總筆數:" + intTotal + "筆。", LogState.Info);
            JobHelper.SaveLog("電話無異動筆數:" + intCount1 + "筆。", LogState.Info);
            JobHelper.SaveLog("電文查詢第二卡人檔失敗筆數(不影響後續流程):" + intCount2 + "筆。", LogState.Info);
            JobHelper.SaveLog("電話有異動更新成功筆數:" + intCount3 + "筆。", LogState.Info);
            JobHelper.SaveLog("電話有異動更新失敗筆數(不影響後續流程):" + intCount4 + "筆。", LogState.Info);
            JobHelper.SaveLog("其他異常資料筆數(人工判斷):" + intError + "筆。", LogState.Info);
        }
        catch (Exception ex)
        {
            //*批次完成記錄LOG信息
            InsertBatchLog(intTotal, intCount1, intCount2, intCount3, intCount4, intError, ex.ToString(), dateStart, strDeal_S_No);
            Logging.Log(ex, strJobID);
        }
        finally
        {
            JobHelper.SaveLog("清空主機Session。", LogState.Info);
            MainFrameInfo.ClearHtgSessionJob(ref strSessionId, strJobID);
            JobHelper.SaveLog("JOB結束！", LogState.Info);
        }
    }

    /// 作者 趙呂梁 
    /// 創建日期：2009/10/17
    /// 修改日期：2009/10/17 
    /// <summary>
    /// 開始此次作業向Job_Status中插入一筆新的資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool InsertNewBatch()
    {

        //*向L_BATCH_LOG中插入一筆新的資料
        return BRL_BATCH_LOG.InsertRunning(strFunctionKey, strJobID, dateStart, "R", "");

    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/17
    /// 修改日期：2009/10/17 
    /// <summary>
    /// 得到第一卡人檔資料
    /// </summary>
    /// <param name="strCusId">身份證號碼</param>
    /// <param name="strReceiveNumber">收件編號</param>
    /// <param name="htResult">主機傳回欄位信息的HashTable</param>
    /// <param name="context">job信息</param>
    /// <returns>true成功，false失敗</returns>
    private bool GetOneCardData(string strCusId, string strReceiveNumber, string strDeal_S_No,ref Hashtable htResult, Quartz.JobExecutionContext context, ref string strSessionId)
    {
        Hashtable htInput = AddUploadJcdgAndJcdkHtgInfo(strCusId, "1", strSessionId);

        Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCF6, htInput, false, "100", GetAgentInfo(context), strJobID);
        htResult = htReturn;

        if (!htReturn.Contains("HtgMsg"))
        {

            //*獲得主機sessionId
            strSessionId = htReturn["sessionId"].ToString();

            string strName = htReturn["NAME_1"].ToString();//*得到主機中的<姓名>欄位
            htReturn["ACCT_NBR"] = htInput["ACCT_NBR"];
            if (strName == "")
            {
                BROTHER_BANK_TEMP.UpdatePcmcReturnCodeAndC1342ReturnCode("ERROR:5", strReceiveNumber, "", strDeal_S_No);
                return false;
            }
            return true;
        }
        else
        {
            BROTHER_BANK_TEMP.UpdatePcmcReturnCodeAndC1342ReturnCode("ERROR:4", strReceiveNumber, "", strDeal_S_No);
            return false;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/17
    /// 修改日期：2009/10/17 
    /// <summary>
    /// 得到第二卡人檔資料
    /// </summary>
    /// <param name="strCusId">身份證號碼</param>
    /// <param name="strReceiveNumber">收件編號</param>
    /// <param name="htResult">主機傳回欄位信息的HashTable</param>
    /// <param name="context">job信息</param>
    /// <returns>true成功，false失敗</returns>
    private bool GetTwoCardData(string strCusId, string strReceiveNumber, string strDeal_S_No,ref Hashtable htResult, Quartz.JobExecutionContext context, ref string strSessionId)
    {
        Hashtable htInput = AddUploadJcdgAndJcdkHtgInfo(strCusId, "1", strSessionId);

        Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCDK, htInput, false, "100", GetAgentInfo(context), strJobID);
        htResult = htReturn;
        if (!htReturn.Contains("HtgMsg"))
        {
            //*獲得主機sessionId
            strSessionId = htReturn["sessionId"].ToString();
            htReturn["ACCT_NBR"] = htInput["ACCT_NBR"];
            return true;
        }
        else
        {
            BROTHER_BANK_TEMP.UpdatePcmcReturnCodeAndC1342ReturnCode("ERROR:6", strReceiveNumber, "", strDeal_S_No);
            return false;
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/17
    /// 修改日期：2009/10/17 
    /// <summary>
    /// 添加上傳主機信息
    /// </summary>      
    private Hashtable AddUploadJcdgAndJcdkHtgInfo(string strCusId, string strFunctionCode, string strSessionId)
    {
        Hashtable htInput = new Hashtable();
        htInput.Add("ACCT_NBR", CommonFunction.GetSubString(strCusId, 0, 16));//*添加上傳主機身份證號碼
        htInput.Add("LINE_CNT", "0000");
        htInput.Add("FUNCTION_CODE", strFunctionCode);
        htInput.Add("sessionId", strSessionId);
        return htInput;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/17
    /// 修改日期：2009/10/17 
    /// <summary>
    /// 得到登陸主機信息
    /// </summary>
    /// <returns>EntityAGENT_INFO</returns>
    private EntityAGENT_INFO GetAgentInfo(Quartz.JobExecutionContext context)
    {
        JobDataMap jobDataMap = context.JobDetail.JobDataMap;
        EntityAGENT_INFO eAgentInfo = new EntityAGENT_INFO();
        if (jobDataMap != null && jobDataMap.Count > 0)
        {
            eAgentInfo.agent_id = jobDataMap.GetString("userId");
            eAgentInfo.agent_pwd = jobDataMap.GetString("passWord");
            eAgentInfo.agent_id_racf = jobDataMap.GetString("racfId");
            eAgentInfo.agent_id_racf_pwd = jobDataMap.GetString("racfPassWord");
        }
        return eAgentInfo;
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/17
    /// 修改日期：2022/09/19 
    /// <summary>
    /// JOB執行狀態
    /// </summary>
    /// <param name="intTotal">總筆數</param>
    /// <param name="intSuccess">intCount1 + intCount3(電話無異動筆數 + 電話有異動更新成功筆數)</param>
    /// <param name="intError">其他異常資料筆數</param>
    /// <returns>JOB執行狀態 S：執行成功、F：批次失敗、P：批次部分成功、N：無資料</returns>
    private string GetStatus(int intTotal, int intSuccess, int intError)
    {
        if (intTotal == 0)
        {
            return "N";
        }

        if (intTotal == intSuccess)
        {
            return "S";
        }

        return intTotal == intError ? "F" : "P";
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/17
    /// 修改日期：2022/09/19 
    /// <summary>
    /// JOB執行狀態
    /// </summary>
    /// <param name="strStatus">狀態英文名稱</param>
    /// <returns>JOB執行狀態</returns>
    private string GetStatusName(string strStatus)
    {
        switch (strStatus)
        {
            case "P":
                return "批次部分成功";
            case "F":
                return "批次失敗";
            case "S":
                return "批次成功";
            case "N":
                return "批次成功(沒有資料)";
            default:
                return "";
        }
    }


    /// <summary>
    /// 添加轉化后的HashTable
    /// </summary>
    /// <param name="htOutput">HashTable</param>
    /// <param name="strKey">鍵</param>
    /// <param name="strValue">值</param>
    private  void GetNewHashTable(Hashtable htOutput, string strKey, string strValue)
    {
        if (htOutput.ContainsKey(strKey))
        {
            htOutput[strKey] = strValue;
        }
        else
        {
            htOutput.Add(strKey, strValue);
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/17
    /// 修改日期：2022/09/19 
    /// <summary>
    /// 插入L_BATCH_LOG資料庫
    /// </summary>
    /// <param name="intTotal">總筆數</param>
    /// <param name="intCount1">電話無異動筆數</param>
    /// <param name="intCount2">電文查詢第二卡人檔失敗筆數</param>
    /// <param name="intCount3">電話有異動更新成功筆數</param>
    /// <param name="intCount4">電話有異動更新失敗筆數</param>
    /// <param name="intError">其他異常資料筆數</param>
    /// <param name="strError">JOB失敗信息</param>
    /// <param name="dateStart">JOB開始時間</param>
    /// <param name="strDeal_S_No">交易序號</param>
    private void InsertBatchLog(int intTotal, int intCount1, int intCount2, int intCount3, int intCount4, int intError, string strError, DateTime dateStart, string strDeal_S_No)
    {
        // 取得JOB執行狀態
        string strStatus = GetStatus(intTotal, intCount1 + intCount3, intError);
        
        // 未完成筆數
        int intNo = intTotal - intCount1 - intCount2 - intCount3 - intCount4 - intError;
        
        // 組成信件主要內容
        StringBuilder sbMessage = new StringBuilder();
        sbMessage.Append("總筆數：" + intTotal + "。");
        sbMessage.Append("電話無異動筆數：" + intCount1 + "。");
        sbMessage.Append("電文查詢第二卡人檔失敗筆數(不影響後續流程)：" + intCount2 + "。");
        sbMessage.Append("電話有異動更新成功筆數：" + intCount3 + "。");
        sbMessage.Append("電話有異動更新失敗筆數(不影響後續流程)：" + intCount4 + "。");
        sbMessage.Append("其他異常資料筆數(人工判斷)：" + intError + "。");
        sbMessage.Append("未完成筆數：" + intNo + "。");
        if (strError.Trim() != "")
        {
            sbMessage.Append("失敗訊息：" + strError);//*失敗訊息
        }
        
        //*插入L_BATCH_LOG資料庫
        BRL_BATCH_LOG.Delete(strFunctionKey, strJobID, "R");
        BRL_BATCH_LOG.Insert(strFunctionKey, strJobID, dateStart, strStatus, sbMessage.ToString());

        if (intTotal > 0)
        {
            string[] str = strMail.Split(';');

            System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();

            nvc["Name"] = strMail.Replace(';', ',');

            nvc["Title"] = strTitle;

            nvc["StartTime"] = dateStart.ToString();

            nvc["EndTime"] = DateTime.Now.ToString();

            nvc["Message"] = sbMessage.ToString().Trim();

            nvc["Status"] = GetStatusName(strStatus);

            try
            {
                MailService.MailSender(str, 1, nvc,"");
            }
            catch (Exception ex)
            {
                Logging.Log(ex, strJobID);
            }
        }
    }

    //BatchJOB
    public static bool InsertCustomerLog(DataTable dtblUpdate, string strQueryKey, string strLogFlag, string strUserId, string strTransId, string strReceiveNumber)
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
                eCustomerLog.Receive_Number = strReceiveNumber;
                BRCustomer_Log.AddEntity(eCustomerLog);
            }
        }
        catch
        {
            return false;
        }
        return true;
    }


}