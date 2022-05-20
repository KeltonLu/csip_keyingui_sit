//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：ACH批次作業
//*  創建日期：2009/10/16
//*  修改記錄：

//*<author>            <time>               <TaskID>                <desc>
//* Ares_Luke          2020/12/04           20200031-CSIP EOS       新增Log機制與調整FunctionKey用法。
//* Ares_Stanley       2021/11/01           20200031-CSIP EOS       理專十誡，移除異動Email、電子賬單相關。
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
    private int intOne;
    private string strNoteNumber;
    private string strJobID;
    DateTime dateStart;
    /// 作者 趙呂梁
    /// 創建日期：2009/10/16
    /// 修改日期：2009/10/16 
    public void Execute(Quartz.JobExecutionContext context)
    {
        int intTotal = 0;//*記錄批次總筆數
        int intSuccess = 0;//*記錄批次成功筆數 
        int intNonce = 0;//*當前執行的筆數
        string strMsgID = "";
        string strBankAccNo = "";
        string strApplyType = "";
        string strKeyInFlag = "";
        string strBankCusNoID = "";
        string strPayWay = "";
        string strBcycleCode = "";
        string strMobilePhone = "";
        //string strEmail = "";
        //string strEbill = "";
        string strDeal_S_No = "";

        dateStart = DateTime.Now;//*批次開始執行時間
        
        try
        {
            JobDataMap jobDataMap = context.JobDetail.JobDataMap;
            strMail=jobDataMap.GetString("mail").Trim();
            strTitle = jobDataMap.GetString("title").Trim();
            strJobID = jobDataMap.GetString("jobid").Trim();
            //strJobID = jobDataMap.GetString("JOBID").Trim();

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

            DataSet dstInfo = null;


            //*開始批次作業
            if (!InsertNewBatch())
            {
                return;
            }

            ArrayList arrayListAch = new ArrayList();
            arrayListAch.Clear();

            //*取得需要異動主機欄位的資料總筆數 
            intTotal = BROTHER_BANK_TEMP.SelectProcessTotalCount();

            //*得到需要異動主機欄位的資料
            DataSet dstAch = BROTHER_BANK_TEMP.SelectReceiveList();

            if (intTotal > 0)
            {
                for (int i = 0; i < intTotal; i++)
                {
                    arrayListAch.Add(dstAch.Tables[0].Rows[i][0].ToString().Trim());
                }

                strSessionId = "";
                foreach (string strReceiveNumber in arrayListAch)
                {
                    intOne = 0;
                    System.Threading.Thread.Sleep(100);
                    intNonce++;
                    BROTHER_BANK_TEMP.UpdatePcmcUploadFlag(strReceiveNumber, "1");
                    strNoteNumber = strReceiveNumber;
                    DataSet dstBatchInfo = BROTHER_BANK_TEMP.SelectBatchInfo(strReceiveNumber);

                    for(int j=0;j<dstBatchInfo.Tables[0].Rows.Count;j++)
                    //if (dstBatchInfo != null && dstBatchInfo.Tables[0].Rows.Count > 0)
                    {
                        //*資料庫中的扣繳帳號
                        strBankAccNo = dstBatchInfo.Tables[0].Rows[j][EntityOTHER_BANK_TEMP.M_Other_Bank_Code_S].ToString().Trim() + "-" + dstBatchInfo.Tables[0].Rows[0][EntityOTHER_BANK_TEMP.M_Other_Bank_Acc_No].ToString().Trim();
                        //*資料庫中的申請類別
                        strApplyType = dstBatchInfo.Tables[0].Rows[j][EntityOTHER_BANK_TEMP.M_Apply_Type].ToString().Trim();
                        //*資料庫中的鍵檔來源
                        strKeyInFlag = dstBatchInfo.Tables[0].Rows[j][EntityOTHER_BANK_TEMP.M_KeyIn_Flag].ToString().Trim();
                        //*資料庫中的帳戶ID
                        strBankCusNoID = dstBatchInfo.Tables[0].Rows[j][EntityOTHER_BANK_TEMP.M_Other_Bank_Cus_ID].ToString().Trim();
                        //*資料檔中的繳款狀況
                        strPayWay = dstBatchInfo.Tables[0].Rows[j][EntityOTHER_BANK_TEMP.M_Other_Bank_Pay_Way].ToString().Trim();
                        //*資料檔中的帳單週期
                        strBcycleCode = dstBatchInfo.Tables[0].Rows[j][EntityOTHER_BANK_TEMP.M_bcycle_code].ToString().Trim();
                        //*資料檔中的行動電話
                        strMobilePhone = dstBatchInfo.Tables[0].Rows[j][EntityOTHER_BANK_TEMP.M_Mobile_Phone].ToString().Trim();
                        //*資料檔中的E-MAIL
                        //strEmail = dstBatchInfo.Tables[0].Rows[j][EntityOTHER_BANK_TEMP.M_E_Mail].ToString().Trim();
                        //*資料檔中的電子賬單
                        //strEbill = dstBatchInfo.Tables[0].Rows[j][EntityOTHER_BANK_TEMP.M_E_Bill].ToString().Trim();
                        //*資料檔中的Deal_S_No (for P02D unique 識別使用)
                        strDeal_S_No = dstBatchInfo.Tables[0].Rows[j][EntityOTHER_BANK_TEMP.M_Deal_S_No].ToString().Trim();

                        //*當自扣申請類別為D且來源為P02D，需將GUI PCMC的扣繳帳號(行庫代號+帳號)、帳戶ID欄位值清空回貼主機
                        if (strApplyType == "D" && strKeyInFlag == "0")
                        {
                            strBankAccNo = "";
                            strBankCusNoID = "";
                        }

                        //*申請類別若是O類執行批次作業時要作踢退
                        if (strApplyType == "O")
                        {
                            BROTHER_BANK_TEMP.UpdatePcmcReturnCodeAndC1342ReturnCode("ERROR:O", strReceiveNumber, "", strDeal_S_No);
                            continue;
                        }

                        //*若自扣申請類別為D且來源為GUI使用者鍵檔(不為P02D) 視為回貼主機失敗，不回貼主機
                        if (strApplyType == "D" && strKeyInFlag != "0")
                        {
                            BROTHER_BANK_TEMP.UpdatePcmcReturnCodeAndC1342ReturnCode("ERROR:10", strReceiveNumber, "", strDeal_S_No);
                            continue;
                        }

                        if (!(strApplyType == "D" && strKeyInFlag == "0"))
                        {
                            //*檢核銀行代碼是否輸入正確
                            string strOtherBankCodeS = dstBatchInfo.Tables[0].Rows[j][EntityOTHER_BANK_TEMP.M_Other_Bank_Code_S].ToString().Trim();
                            dstInfo = BaseHelper.CheckCommonPropertySet(strFunctionKey, "18", strOtherBankCodeS);

                            if (dstInfo == null)
                            {
                                //*查詢資料庫時發生錯誤
                                BROTHER_BANK_TEMP.UpdatePcmcReturnCodeAndC1342ReturnCode("ERROR:8", strReceiveNumber, "", strDeal_S_No);
                                continue;

                            }
                            else
                            {
                                if (dstInfo.Tables[0].Rows.Count == 0 || strOtherBankCodeS == "042" || strOtherBankCodeS == "701")
                                {
                                    BROTHER_BANK_TEMP.UpdatePcmcReturnCodeAndC1342ReturnCode("ERROR:8", strReceiveNumber, "", strDeal_S_No);
                                    continue;
                                }
                            }
                        }

                        //*身分證號碼
                        string strCusId = dstBatchInfo.Tables[0].Rows[j][EntityOTHER_BANK_TEMP.M_Cus_ID].ToString().Trim();

                        // strSessionId = "";
                        //*得到第一卡人檔資料
                        Hashtable htOneCardResult = new Hashtable();
                        if (!GetOneCardData(strCusId, strReceiveNumber, strDeal_S_No, ref htOneCardResult, context, ref strSessionId))
                        {
                            
                            if (htOneCardResult["HtgMsgFlag"].ToString() == "0")
                            {
                                continue;
                            }
                            else
                            {
                                InsertBatchLog(intTotal, intSuccess, intNonce, htOneCardResult["HtgMsg"].ToString(), dateStart, strDeal_S_No);
                                return;
                            }
                        }

                        //*獲得主機sessionId
                        //strSessionId = htOneCardResult["sessionId"].ToString();

                        string strUserId = GetAgentInfo(context).agent_id.Trim();//*USER_ID

                        Hashtable htTwoCardResult = new Hashtable();
                        string strPEbill = "";//*主機<電子帳單>
                        if (strKeyInFlag == "2")
                        {
                            //*得到第二卡人檔資料
                            if (!GetTwoCardData(strCusId, strReceiveNumber, strDeal_S_No, ref htTwoCardResult, context, ref strSessionId))
                            {
                                if (htTwoCardResult["HtgMsgFlag"].ToString() == "0")
                                {
                                    continue;
                                }
                                else
                                {
                                    InsertBatchLog(intTotal, intSuccess, intNonce, htTwoCardResult["HtgMsg"].ToString(), dateStart, strDeal_S_No);
                                    return;
                                }
                            }

                            if (htOneCardResult["OFF_PHONE_FLAG"] != null)
                            {
                                strPEbill = htOneCardResult["OFF_PHONE_FLAG"].ToString();
                            }
                            else
                            {
                                BROTHER_BANK_TEMP.UpdatePcmcReturnCodeAndC1342ReturnCode("ERROR:7", strReceiveNumber, "", strDeal_S_No);
                                continue;
                            }
                        }

                        if (strKeyInFlag == "2")
                        {
                            //*使用"JCDK"異動主機欄位<行動電話>、< E-MAIL>
                            DataTable dtblUpdateTwoCard = CommonFunction.GetDataTable();

                            //*比對<行動電話>
                            CommonFunction.ContrastData(htTwoCardResult, dtblUpdateTwoCard, strMobilePhone, "MOBILE_PHONE", "行動電話");
                            //*比對< E-MAIL> //20211021 不異動Email By Ares Stanley
                            //CommonFunction.ContrastData(htTwoCardResult, dtblUpdateTwoCard, strEmail, "EMAIL", "E-MAIL");

                            // htTwoCardResult.Add("sessionId", strSessionId);
                            GetNewHashTable(htTwoCardResult, "sessionId", strSessionId);

                            if (dtblUpdateTwoCard.Rows.Count > 0)
                            {
                                htTwoCardResult["FUNCTION_CODE"] = 2;
                                //*異動主機欄位
                                Hashtable htResultJCDK = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCDK, htTwoCardResult, false, "200", GetAgentInfo(context), strJobID);

                                if (!htResultJCDK.Contains("HtgMsg"))
                                {
                                    //*更新資料庫欄位
                                    BROTHER_BANK_TEMP.UpdateC1342ReturnCode(strReceiveNumber, "0001");
                                    //*獲得主機sessionId
                                    strSessionId = htResultJCDK["sessionId"].ToString();
                                    //*將異動的欄位寫入 customer_log
                                    if (!InsertCustomerLog(dtblUpdateTwoCard, strCusId, "P4", strUserId, "01010600", strReceiveNumber))
                                    {
                                        Logging.Log(strCusId + ":" + MessageHelper.GetMessage("00_00000000_017"), strJobID, LogState.Error);
                                    }
                                }
                                else
                                {
                                    if (htResultJCDK["HtgMsgFlag"].ToString() == "0")
                                    {
                                        //*更新資料庫欄位
                                        if (htResultJCDK["MESSAGE_TYPE"].ToString().Trim() == "9999" || htResultJCDK["MESSAGE_TYPE"].ToString().Trim() == "8888" || htResultJCDK["MESSAGE_TYPE"].ToString().Trim() == "8001")
                                        {
                                            BROTHER_BANK_TEMP.UpdateC1342ReturnCode(strReceiveNumber, htResultJCDK["MESSAGE_TYPE"].ToString().Trim());
                                        }
                                        else
                                        {
                                            BROTHER_BANK_TEMP.UpdateC1342ReturnCode(strReceiveNumber, "ERROR:1:" + htResultJCDK["MESSAGE_TYPE"].ToString().Trim());
                                        }
                                        continue;
                                    }
                                    else
                                    {
                                        intSuccess--;//*主機返回異常，故沒有成功減1；
                                        InsertBatchLog(intTotal, intSuccess, intNonce, htResultJCDK["HtgMsg"].ToString(), dateStart, strDeal_S_No);
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                BROTHER_BANK_TEMP.UpdateC1342ReturnCode(strReceiveNumber, "");
                                //continue;
                            }
                        }                        

                        //*使用PCMC_SessionA_Submit異動主機欄位<扣繳帳號>、<繳款狀況>、<帳戶ID>、<帳單週期>
                        DataTable dtblUpdateOneCard = CommonFunction.GetDataTable();

                        ArrayList arrayName = new ArrayList(new object[] { "ACCT_NBR", "CO_TAX_ID_TYPE", "DD_ID", "BILLING_CYCLE", "CO_OWNER", "OFF_PHONE_FLAG" });
                        Hashtable htOutput = new Hashtable();
                        MainFrameInfo.ChangeJCF6toPCTI(htOneCardResult, htOutput, arrayName);

                        //*比對<扣繳帳號>
                        CommonFunction.ContrastDataEdit(htOutput, dtblUpdateOneCard, strBankAccNo, "BK_ID_AC", "扣繳帳號");
                        //*比對<繳款狀況>
                        CommonFunction.ContrastDataEdit(htOutput, dtblUpdateOneCard, strPayWay, "LAST_CR_LINE_IND", "繳款狀況");
                        //*比對<帳戶ID>
                        CommonFunction.ContrastDataEdit(htOutput, dtblUpdateOneCard, strBankCusNoID, "DD_ID", "帳戶ID");

                        if ((strApplyType == "D" && strKeyInFlag == "0"))
                        {
                            //*去掉<帳單週期>
                            htOutput.Remove("BILLING_CYCLE");
                           
                            //*去掉<電子帳單>
                            htOutput.Remove("MAIL");
                         
                        }
                        else
                        {
                            //*比對<帳單週期>
                            CommonFunction.ContrastDataEdit(htOutput, dtblUpdateOneCard, strBcycleCode, "BILLING_CYCLE", "帳單週期");
                            //*比對<電子帳單> //20211021 不比對電子帳單 By Ares Stanley
                            //CommonFunction.ContrastDataEdit(htOutput, dtblUpdateOneCard, strEbill, "MAIL", "電子帳單");

                        }

                        //*若選Other_Bank_Temp的欄位KeyIn_Flag！= "0" 
                        if (strKeyInFlag != "0")
                        {
                            //*主機中的<扣繳帳號>欄位不為空
                            string strHtgBankAccNo = htOneCardResult["CO_OWNER"].ToString().Trim();
                            if (CommonFunction.GetSubString(strHtgBankAccNo, 4, strHtgBankAccNo.Length - 4).Length != 0)
                            {
                                BROTHER_BANK_TEMP.UpdatePcmcReturnCodeAndC1342ReturnCode("ERROR:9", strReceiveNumber, "", strDeal_S_No);
                                //* 更新Other_Bank_Temp上傳處理狀態
                                BROTHER_BANK_TEMP.UpdateACHhold("1", strReceiveNumber, strDeal_S_No);
                                if (dtblUpdateOneCard.Rows.Count > 0)
                                {
                                    //*針對<電子帳單>及<帳單週期>做特別處理
									DataTable dtblUpdateMAIL = CommonFunction.GetDataTable();
									Hashtable htOutputMAIL = new Hashtable();
									MainFrameInfo.ChangeJCF6toPCTI(htOneCardResult, htOutputMAIL, arrayName);
									if ((strApplyType == "D" && strKeyInFlag == "0"))
									{
                                        //*去掉<帳單週期>
                                        htOutput.Remove("BILLING_CYCLE");
										//*去掉<電子帳單>
										htOutput.Remove("MAIL");
									}
									else
									{
                                        //*比對<帳單週期>
                                        CommonFunction.ContrastDataEdit(htOutputMAIL, dtblUpdateMAIL, strBcycleCode, "BILLING_CYCLE", "帳單週期");
                                        //*比對<電子帳單> //20211021 不比對電子帳單 By Ares Stanley
                                        //CommonFunction.ContrastDataEdit(htOutputMAIL, dtblUpdateMAIL, strEbill, "MAIL", "電子帳單");
                                    }
                                    //*更新主機
                                    if (dtblUpdateMAIL.Rows.Count > 0)
									{
										GetNewHashTable(htOutputMAIL, "sessionId", strSessionId);
										htOutputMAIL.Add("FUNCTION_ID", "PCMC1");
										//*異動主機欄位
										MainFrameInfo.GetMainFrameInfo(HtgType.P4_PCTI, htOutputMAIL, false, "200", GetAgentInfo(context), strJobID);
									}
									
									
                                    //*將異動的欄位寫入 customer_log
                                    if (!InsertCustomerLog(dtblUpdateOneCard, strCusId, "P4", strUserId, "01010600", strReceiveNumber))
                                    {
                                        Logging.Log(strCusId + ":" + MessageHelper.GetMessage("00_00000000_017"), strJobID, LogState.Error);
                                    }
                                }
                                continue;
                            }
                        }

                        if (dtblUpdateOneCard.Rows.Count > 0)
                        {
                            //htOutput.Add("sessionId", strSessionId);
                            GetNewHashTable(htOutput, "sessionId", strSessionId);
                            htOutput.Add("FUNCTION_ID", "PCMC1");
                            //*異動主機欄位
                            Hashtable htResultPcmc = MainFrameInfo.GetMainFrameInfo(HtgType.P4_PCTI, htOutput, false, "200", GetAgentInfo(context), strJobID);

                            if (!htResultPcmc.Contains("HtgMsg"))
                            {

                                //*獲得主機sessionId
                                strSessionId = htResultPcmc["sessionId"].ToString();

                                //*更新資料庫欄位
                                BROTHER_BANK_TEMP.UpdatePcmcReturnCodeAndC1342ReturnCode("PAGE 02 OF 03", strReceiveNumber, "", strDeal_S_No);
                                if (htOutput.Contains("MAIL"))
                                {
                                    BROTHER_BANK_TEMP.UpdatePayWay(strReceiveNumber, "Y");
                                }
                                //*將異動的欄位寫入 customer_log
                                if (!InsertCustomerLog(dtblUpdateOneCard, strCusId, "P4", strUserId, "01010600",strReceiveNumber))
                                {
                                    Logging.Log(strCusId + ":" + MessageHelper.GetMessage("00_00000000_017"), strJobID, LogState.Error);
                                }
                                intSuccess++;
                                intOne = 1;
                            }
                            else
                            {
                                if (htResultPcmc["HtgMsgFlag"].ToString() == "0")
                                {
                                    //*更新資料庫欄位
                                    BROTHER_BANK_TEMP.UpdatePcmcReturnCodeAndC1342ReturnCode("ERROR:13:" + htResultPcmc["HtgMsg"].ToString(), strReceiveNumber, "", strDeal_S_No);
                                    continue;
                                }
                                else
                                {
                                    InsertBatchLog(intTotal, intSuccess, intNonce, htResultPcmc["HtgMsg"].ToString(), dateStart, strDeal_S_No);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            BROTHER_BANK_TEMP.UpdatePcmcReturnCodeAndC1342ReturnCode("", strReceiveNumber, "", strDeal_S_No);
                            continue;
                        }

                        
                    }
                }
            }
            //*批次完成記錄LOG信息
            InsertBatchLog(intTotal, intSuccess, 0, "", dateStart, strDeal_S_No);


            JobHelper.SaveLog("總筆數:" + intTotal + "筆。", LogState.Info);
            JobHelper.SaveLog("成功筆數:" + intSuccess + "筆。", LogState.Info);
        }
        catch (Exception ex)
        {
            //*批次完成記錄LOG信息
            InsertBatchLog(intTotal, intSuccess, intNonce, ex.ToString(), dateStart, strDeal_S_No);
            Logging.Log(ex, strJobID);
        }
        finally
        {
            UpdateBatch();
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
    /// 異動Job_Status中批次狀態
    /// </summary>
    private void UpdateBatch()
    {

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
    /// 修改日期：2009/10/17 
    /// <summary>
    /// JOB執行狀態
    /// </summary>
    /// <param name="intTotal">總筆數</param>
    /// <param name="intSuccess">成功筆數</param>
    /// <returns>JOB執行狀態</returns>
    private string GetStatus(int intTotal, int intSuccess)
    {
        if (intSuccess > 0)
        {
            if ((intTotal - intSuccess) > 0)
            {
                return "P";
            }
            else
            {
                return "S";
            }
        }
        else
        {
            return "F";
        }
    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/17
    /// 修改日期：2009/10/17 
    /// <summary>
    /// JOB執行狀態
    /// </summary>
    /// <param name="intTotal">總筆數</param>
    /// <param name="intSuccess">成功筆數</param>
    /// <param name="intFail">失敗筆數</param>
    /// <param name="intNo">未完成筆數</param>
    /// <returns>JOB執行狀態</returns>
    private string GetStatus(int intTotal, int intSuccess, int intFail,int intNo)
    {
        if (intTotal == 0)
        {
            return "N";
        }
        else if (intTotal ==( intSuccess + intFail))
        {
            return "S";
        }
        else if (intTotal == intNo)
        {
            return "F";
        }
        else
        {
            return "P";
        }


    }

    /// 作者 趙呂梁
    /// 創建日期：2009/10/17
    /// 修改日期：2009/10/17 
    /// <summary>
    /// JOB執行狀態
    /// </summary>
    /// <param name="strStauts">狀態英文名稱</param>

    /// <returns>JOB執行狀態</returns>
    private string GetStatusName(string strStauts)
    {
        switch (strStauts)
        {
            case "P":
                return "批次部分成功";
            case "F":
                return "批次失敗";
            case "S":
                return "批次成功";
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
    /// 修改日期：2009/10/17 
    /// <summary>
    /// 插入L_BATCH_LOG資料庫
    /// </summary>
    /// <param name="intTotal">總筆數</param>
    /// <param name="intSuccess">成功筆數</param>
    /// <param name="intNonce">當前執行的筆數</param>
    /// <param name="strError">JOB失敗信息</param>
    /// <param name="dateStart">JOB開始時間</param>
    private void InsertBatchLog(int intTotal, int intSuccess, int intNonce, string strError, DateTime dateStart, string strDeal_S_No)
    { 
       // string strStatus = GetStatus(intTotal, intSuccess);
        int intNo = 0;//*未完成筆數

        if (intNonce > 0)//*當intNonce > 0表示主機返回異常，intNonce為記錄當前異常的筆數
        {
            if (intOne == 0)
            {
                intNo = intTotal - (intNonce - 1);
                BROTHER_BANK_TEMP.UpdatePcmcUploadFlag(strNoteNumber , "");
                BROTHER_BANK_TEMP.UpdatePcmcReturnCodeAndC1342ReturnCode("", strNoteNumber, "", strDeal_S_No);
            }
            else
            {
                intNo = intTotal - (intNonce);
            }
        }

        int intFail = intTotal - intSuccess - intNo;//*失敗筆數



        string strStatus = GetStatus(intTotal, intSuccess, intFail, intNo);

        StringBuilder sbMessage = new StringBuilder();
        sbMessage.Append("總筆數：" + intTotal.ToString() + "。");//*總筆數
        sbMessage.Append("成功筆數：" + intSuccess.ToString() + "。");//*成功筆數
        sbMessage.Append("失敗筆數：" + intFail.ToString() + "。");//*失敗筆數
        sbMessage.Append("未完成筆數：" + intNo.ToString() + "。");//*未完成筆數
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