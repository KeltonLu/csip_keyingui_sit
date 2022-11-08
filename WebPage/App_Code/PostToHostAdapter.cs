//******************************************************************
//*  作    者：
//*  功能說明：
//*  創建日期：
//*  修改紀錄：
//*  <author>          <time>              <TaskID>                <desc>
//* Ares_jhun          2022/09/19          RQ-2022-019375-000      EDDA需求調整：停發PCTI電文統一週期件處理(withholding.txt)
//*******************************************************************

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Logging;

/// <summary>
/// PostToHostAdapter 的摘要描述
/// </summary>
public class PostToHostAdapter
{
    string userID = string.Empty;
    string receiveNumber = string.Empty;
    string cusName = string.Empty;
    string accNoBank = string.Empty;
    string accNo = string.Empty;
    string payWay = string.Empty;
    string accID = string.Empty;
    string bcycleCode = string.Empty;
    string mobilePhone = string.Empty;
    string eMail = string.Empty;
    string eBill = string.Empty;
    EntityAGENT_INFO eAgentInfo;
    string hostMsg = string.Empty;
    string clientMsg = string.Empty;

    public PostToHostAdapter(string userID, string receiveNumber, string cusName, string accNoBank, string accNo, string payWay, string accID,
                                string bcycleCode, string mobilePhone, string eMail, string eBill, EntityAGENT_INFO eAgentInfo)
    {
        this.userID = userID;
        this.receiveNumber = receiveNumber;
        this.cusName = cusName;
        this.accNoBank = accNoBank;
        this.accNo = accNo;
        this.payWay = payWay;
        this.accID = accID;
        this.bcycleCode = bcycleCode;
        this.mobilePhone = mobilePhone;
        this.eMail = eMail;
        this.eBill = eBill;
        this.eAgentInfo = eAgentInfo;
    }

    /// 修改紀錄：20211021 不上送Email、電子賬單 By Ares Stanley
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool SendToHost()
    {
        int intSubmit = 0;// 記錄異動主機成功的次數
        string sUserID = CommonFunction.GetSubString(userID, 0, 16);
        bool result = false;

        string PCTIResultCode = "";
        string JCDKResultCode = "";
        bool htDataExistFlag = true;
        #region 獲取主機資料

        // 讀取第一卡人檔
        Hashtable htOutputP4_JCF6 = new Hashtable();// 查詢第一卡人檔下行
        htOutputP4_JCF6 = GetP4JCF6(sUserID, htOutputP4_JCF6);
        //if (htOutputP4_JCF6 == null) return false;
        if (htOutputP4_JCF6 == null)
        {
            htDataExistFlag = false;
            PCTIResultCode = "_009"; //抓取失敗
        };

        // 讀取第二卡人檔
        Hashtable htOutputP4_JCDK = new Hashtable();// 查詢第二卡人檔下行
        htOutputP4_JCDK = GetP4JCDK(sUserID, htOutputP4_JCDK);
        //if (htOutputP4_JCDK == null) return false;
        if (htOutputP4_JCDK == null)
        {
            htDataExistFlag = false;
            JCDKResultCode = "_003"; //抓取失敗
        };

        #endregion


        //bool resultPCTIP4S = false;
        //bool resultP4_JCDKS = false;
        EntityAuto_Pay_Status eAuto_Pay_Status = new EntityAuto_Pay_Status();
        string accNoBank_No = "";// 扣繳帳號(銀行代號)-銀行帳號
        string sendHostResultCode = "0000"; //0000 | 0009 | pcmc
        string snedToHostResult = "F";

        if (htDataExistFlag == true)
        {
            // 異動customer_log記錄的Table
            DataTable dtblUpdateData = CommonFunction.GetDataTable();

            #region 異動主機資料
            Hashtable htInputPCTIP4S = new Hashtable();// 更新第一卡人檔上行
            ArrayList arrayName = new ArrayList(new object[] { "ACCT_NBR", "CO_OWNER", "CO_TAX_ID_TYPE", "DD_ID", "BILLING_CYCLE", "OFF_PHONE_FLAG" });
            MainFrameInfo.ChangeJCF6toPCTI(htOutputP4_JCF6, htInputPCTIP4S, arrayName);// 將異動主機的訊息預設為從主機讀取的訊息

            Hashtable htInputP4_JCDKS = new Hashtable();// 更新第二卡人檔上行
            CommonFunction.GetViewStateHt(htOutputP4_JCDK, ref htInputP4_JCDKS);

            #region Auto_Pay_Status

            //第一卡人檔 : htOutputP4_JCF6
            string acctNBR = htOutputP4_JCF6["ACCT_NBR"].ToString().Trim();// Auto_Pay_Status欄位
            string htgAcctNO = htOutputP4_JCF6["CO_OWNER"].ToString().Trim(); //701-0311373XXXX => 有時前面不一定會有銀行代碼(701-)
            string hgtBillingCycle = htOutputP4_JCF6["BILLING_CYCLE"].ToString().Trim(); //帳單週期
            string hgtEBill = htOutputP4_JCF6["OFF_PHONE_FLAG"].ToString().Trim(); //電子郵註=電子帳單=[ 0 | 1]
            // 2020.05.06 Ray Add 
            string hgtPayWay = htOutputP4_JCF6["CO_TAX_ID_TYPE"].ToString();   // 繳款狀況
            //第二卡人檔 : htInputP4_JCDKS
            string htgEmail = htInputP4_JCDKS["EMAIL"].ToString().Trim();
            string htgMobile = htInputP4_JCDKS["MOBILE_PHONE"].ToString().Trim();

            accNoBank_No = accNoBank + "-" + accNo;// 扣繳帳號(銀行代號)-銀行帳號
            //int hostAcctNOLength = 0; //但沒實際用到
            sendHostResultCode = "0000"; //0000 | 0009 | pcmc            

            //--- Start----------------------
            #region 新判斷邏輯
            eAuto_Pay_Status.IsUpdateByTXT = "";
            //要判斷的值=> 手機(mobilePhone),電子郵件(eMail), 電子帳單(eBill) ,繳款狀況(payWay)     
            List<string> _validateArr = new List<string>();
            string _strformat = "{0},{1},{2}"; //舊值(主機(電文)):htgxxxx  , 新值: 郵局回覆        
            _validateArr.Add(string.Format(_strformat, "mobile", htgMobile, this.mobilePhone));
            //_validateArr.Add(string.Format(_strformat, "email", htgEmail, this.eMail));
            //_validateArr.Add(string.Format(_strformat, "ebill", hgtEBill, this.eBill));
            // 2020.05.06 Ray Add  繳款狀況
            _validateArr.Add(string.Format(_strformat, "payWay", hgtPayWay, this.payWay));
            // 帳單週期(bcycleCode),
            //_validateArr.Add(string.Format(_strformat, "billingCycle", hgtBillingCycle, this.bcycleCode));

            string[] _tmpArr;
            for (int i = 0; i < _validateArr.Count; i++)
            {
                //判斷值有無異動
                //主機(電文) && DB資料 需有值且不一樣就列入PCMC
                _tmpArr = _validateArr[i].Split(',');
                if (string.IsNullOrEmpty(_tmpArr[1]) == false && string.IsNullOrEmpty(_tmpArr[2]) == false
                && _tmpArr[1] != _tmpArr[2])
                {
                    //  非電子帳單比對不一樣視為異動
                    if (_tmpArr[0] != "ebill")
                    {
                        sendHostResultCode = "pcmc";
                        break;
                    }
                    // 電子帳單特例判斷 
                    // 主機(電文) 值0 & 2  郵局資料 值7 多判斷異動紀錄
                    if ((_tmpArr[1] == "0" || _tmpArr[1] == "2") && _tmpArr[2] == "7")
                    {
                        // 判斷customer_log before異動紀錄是否符合，沒異動紀錄視為異動
                        if (!CSIPKeyInGUI.BusinessRules_new.BRCustomer_Log.GetPostToHostAdapter_ebill(userID, receiveNumber, _tmpArr[1]))
                            sendHostResultCode = "pcmc";
                    }
                    else
                    {
                        sendHostResultCode = "pcmc";
                    }
                }
            }

            if (sendHostResultCode != "pcmc")
            {
                //避免sendHostResultCode沒有值，預設為PCMC失敗
                sendHostResultCode = "pcmc";
                //若舊值為空白=新增帳號,所以還得再 異動扣繳帳號和銀行帳號
                if (string.IsNullOrEmpty(htgAcctNO) == true)
                {
                    if (string.IsNullOrEmpty(accNoBank_No) != true)
                    {
                        sendHostResultCode = "0000";
                        eAuto_Pay_Status.IsUpdateByTXT = "N";// 上送主機
                        CommonFunction.ContrastDataEdit(htInputPCTIP4S, dtblUpdateData, accNoBank_No, "BK_ID_AC", "扣繳帳號");
                        // 2020.06.03 Ray 將原先 CompareHostEdit if (intSubmit == 0) 改由放在新增
                        // 繳款狀況(扣繳方式)
                        CommonFunction.ContrastDataEdit(htInputPCTIP4S, dtblUpdateData, payWay, "LAST_CR_LINE_IND", BaseHelper.GetShowText("01_01010800_008"));
                        // 帳戶ID
                        CommonFunction.ContrastDataEdit(htInputPCTIP4S, dtblUpdateData, accID, "DD_ID", BaseHelper.GetShowText("01_01010800_009"));
                    }
                }
                else
                {
                    //銀行帳號有變
                    if (htgAcctNO != accNoBank_No)
                    {
                        // 帳單週期無異動
                        //20190906(U) 拿掉帳單週期的判斷條件
                        //20200217(U) 回覆帳單週期的判斷條件
                        //代表是為週期件 0009
                        sendHostResultCode = "0009";
                        eAuto_Pay_Status.IsUpdateByTXT = "Y";// 主機Temp檔
                        //intSubmit++;//這樣才會更新 Auto_pay,Auto_pay_status, 然後再產生於 withholding.txt 裡
                    }
                }
            }

            //if (sendHostResultCode != "pcmc")
            //{
            //    //判斷銀行是否有不同            
            //    if (string.IsNullOrEmpty(htgAcctNO) == false && htgAcctNO != accNoBank_No)
            //    {
            //        //代表是為週期件 0009
            //        sendHostResultCode = "0009";
            //        eAuto_Pay_Status.IsUpdateByTXT = "Y";// 主機Temp檔                    
            //        intSubmit++;//這樣才會更新 Auto_pay,Auto_pay_status, 然後再產生於 withholding.txt 裡
            //    }
            //    else
            //    {
            //        sendHostResultCode = "0000";
            //        eAuto_Pay_Status.IsUpdateByTXT = "N";// 上送主機

            //        //若舊值為空白, 還得再 異動扣繳帳號和銀行帳號
            //        if (string.IsNullOrEmpty(htgAcctNO) == true)
            //        {
            //            CommonFunction.ContrastDataEdit(htInputPCTIP4S, dtblUpdateData, accNoBank_No, "BK_ID_AC", "扣繳帳號");
            //        }
            //    }
            //}
            #endregion

            #region 舊判斷邏輯
            //if (htgAcctNO != "")
            //{
            //    //hostAcctNOLength = CommonFunction.GetSubString(htgAcctNO, 4, htgAcctNO.Length - 4).Length;            

            //    //應是指 資料都沒異動, 所以就要打主機, 但這邊少了個 帳單週期
            //    if (accNoBank_No == htgAcctNO && mobilePhone == htgMobile && eMail == htgEmail)
            //    {
            //        eAuto_Pay_Status.IsUpdateByTXT = "N";// 上送主機
            //    }
            //    else
            //    {
            //        if (accNoBank_No != htgAcctNO)
            //        {
            //            if (mobilePhone != htgMobile || eMail != htgEmail)
            //            {
            //                sendHostResultCode = "pcmc";
            //            }
            //            else
            //            {
            //                sendHostResultCode = "0009";

            //                eAuto_Pay_Status.IsUpdateByTXT = "Y";// 主機Temp檔
            //                intSubmit++;
            //            }

            //        }
            //        else
            //        {
            //            sendHostResultCode = "pcmc";
            //        }
            //    }

            //}
            //else
            //{
            //    eAuto_Pay_Status.IsUpdateByTXT = "N";

            //    // 異動扣繳帳號和銀行帳號
            //    CommonFunction.ContrastDataEdit(htInputPCTIP4S, dtblUpdateData, accNoBank_No, "BK_ID_AC", "扣繳帳號");
            //}
            #endregion

            #endregion

            //eAuto_Pay_Status => 這個沒有指定預設值, 所以 IsUpdateByTXT的預設值應是 "" 或 null ?            
            //if (eAuto_Pay_Status.IsUpdateByTXT == "N")
            if (sendHostResultCode != "pcmc")
            {
                // 異動第二卡人檔
                bool resultP4_JCDKS = UpdateP4_JCDKS(dtblUpdateData, htOutputP4_JCDK, htInputP4_JCDKS, ref intSubmit, out JCDKResultCode);

                if (JCDKResultCode == "0000")
                {
                    // 異動第一卡人檔
                    bool resultPCTIP4S = UpdatePCTIP4S(dtblUpdateData, htOutputP4_JCF6, htInputPCTIP4S, ref intSubmit, out PCTIResultCode );
                }
            }

            #endregion
            // 2020.05.14 ADD
            // 當主機電文有問題時，0009 IsUpdateByTXT=狀態改為N
            if (PCTIResultCode != "0000" && PCTIResultCode != "")
            {
                if (sendHostResultCode == "0009")
                {
                    eAuto_Pay_Status.IsUpdateByTXT = "N";
                }
                sendHostResultCode = PCTIResultCode;
            }
            else if (JCDKResultCode != "0000" && JCDKResultCode != "")
            {
                if (sendHostResultCode == "0009")
                {
                    eAuto_Pay_Status.IsUpdateByTXT = "N";
                }
                sendHostResultCode = JCDKResultCode;
            }


            #region 異動主機資料後，更新資料庫異動狀態
            //string snedToHostResult = "F";
            //bool result1 = false;
            if (intSubmit > 0) //異動主機成功的次數
            {
                eAuto_Pay_Status.Receive_Number = receiveNumber;
                eAuto_Pay_Status.Cus_ID = userID;
                eAuto_Pay_Status.Cus_Name = cusName;
                eAuto_Pay_Status.AccNoBank = accNoBank;
                eAuto_Pay_Status.Acc_No = accNoBank_No;
                eAuto_Pay_Status.Pay_Way = payWay;
                eAuto_Pay_Status.IsCTCB = "Y";
                eAuto_Pay_Status.DateTime = DateTime.Now;
                eAuto_Pay_Status.Acc_No_O = htgAcctNO;

                // 更新資料庫異動狀態:Table:Auto_Pay_Status,Auto_Pay
                result = UpdateAutoPayStatus(htOutputP4_JCF6, htInputP4_JCDKS, eAuto_Pay_Status, receiveNumber, userID, accNo, eMail, eBill, mobilePhone, acctNBR);
                snedToHostResult = "S";
            }
            #endregion
        }
        else
        {
            //沒有更新到 Auto_Pay , 需用別種代碼表示?
            //snedToHostResult = "F";  //default: "F"
            if (PCTIResultCode != "0000" && PCTIResultCode != "")
            {
                sendHostResultCode = PCTIResultCode;
            }
            else if (JCDKResultCode != "0000" && JCDKResultCode != "")
            {
                sendHostResultCode = JCDKResultCode;
            }
        }

        result = BRFORM_COLUMN.UpdatePostOfficeTemp(receiveNumber, snedToHostResult, sendHostResultCode);

        return result;

    }
    
    /// EDDA需求新增：20220928 批次處理停發PCTI電文統一週期件處理(withholding.txt) By Ares jhun
    /// <summary>
    /// 郵局核印資料處理
    /// </summary>
    /// <returns>F：系統發生未預期錯誤、9000：週期件、9001：週期件(電話更新失敗)、9002：週期件(電文查詢第二卡人檔失敗)</returns>
    public string ProcessAuthData()
    {
        string sUserID = CommonFunction.GetSubString(userID, 0, 16);
        bool jcdkQuery = true;  // 第二卡人檔「查詢」狀態
        bool jcdkUpdate = true; // 第二卡人檔「更新」狀態
        
        // 讀取第二卡人檔
        Hashtable htOutputP4_JCDK = new Hashtable(); // 查詢第二卡人檔下行
        htOutputP4_JCDK = GetP4JCDK(sUserID, htOutputP4_JCDK);
        if (htOutputP4_JCDK == null)
        {
            jcdkQuery = false;
        }

        // 異動customer_log記錄的Table
        DataTable dtUpdateData = CommonFunction.GetDataTable();
        
        Hashtable htInputP4_JCDKS = new Hashtable(); // 更新使用的第二卡人檔上行
        
        bool mobilePhoneIsDiff = false; // 行動電話是否有異動
        
        // 若查詢第二卡人檔成功，則異動第二卡人檔(行動電話)，若更新失敗不影響後續流程
        if (jcdkQuery)
        {
            CommonFunction.GetViewStateHt(htOutputP4_JCDK, ref htInputP4_JCDKS); // 複製第二卡人檔資料
            
            //*比對<行動電話>
            CommonFunction.ContrastData(htInputP4_JCDKS, dtUpdateData, mobilePhone, "MOBILE_PHONE", BaseHelper.GetShowText("01_01010800_011"));

            if (dtUpdateData.Rows.Count > 0) // 行動電話比對後有異動
            {
                mobilePhoneIsDiff = true;
                jcdkUpdate = UpdateP4_JCDKS(dtUpdateData, htInputP4_JCDKS);
            }
        }
        
        string accNoBank_No = accNoBank + "-" + accNo; // 扣繳帳號(銀行代號)-銀行帳號
        
        EntityAuto_Pay_Status eAuto_Pay_Status = new EntityAuto_Pay_Status
        {
            IsUpdateByTXT = "Y", // withholding.txt Y=CSIP、N=PCTI
            Receive_Number = receiveNumber,
            Cus_ID = userID,
            Cus_Name = cusName,
            AccNoBank = accNoBank,
            Acc_No = accNoBank_No,
            Pay_Way = payWay,
            IsCTCB = "Y",
            DateTime = DateTime.Now
        };

        // 更新資料庫異動狀態:Table:Auto_Pay_Status,Auto_Pay
        var result = UpdateAutoPayStatus(mobilePhoneIsDiff, eAuto_Pay_Status, receiveNumber, userID);
        if (!result)
        {
            return "F";
        }

        string sendHostResultCode; // 9000(週期件) | 9001(週期件(電話更新失敗)) | 9002(週期件(電文查詢第二卡人檔失敗))
        
        // 判斷電話異動與JCDK電文狀態
        if (!jcdkQuery)
        {
            sendHostResultCode = "9002"; // 週期件(電文查詢第二卡人檔失敗)：電文查詢第二卡人檔失敗
        }
        else if (mobilePhoneIsDiff)
        {
            if (jcdkUpdate)
            {
                sendHostResultCode = "9000"; // 週期件：電話有異動，發JCDK電文更新「成功」
            }
            else
            {
                sendHostResultCode = "9001"; // 週期件(電話更新失敗)：電話有異動，發JCDK電文更新「失敗」
            }
        }
        else
        {
            sendHostResultCode = "9000"; // 週期件：電話無異動，「不用」發JCDK電文
        }

        result = BRFORM_COLUMN.UpdatePostOfficeTemp(receiveNumber, "S", sendHostResultCode);

        if (result) // 若【PostOffice_Temp】資料更新
        {
            // 將「扣繳帳號、DD_ID、扣繳方式」寫入 customer_log
            DataTable logData = CommonFunction.GetDataTable();
            CommonFunction.UpdateLog("", accNoBank_No, "扣繳帳號", logData); // 扣繳帳號
            CommonFunction.UpdateLog("", payWay, BaseHelper.GetShowText("01_01010800_008"), logData); // 扣繳方式
            CommonFunction.UpdateLog("", accID, BaseHelper.GetShowText("01_01010800_009"), logData); // DD_ID
            
            // 記錄異動欄位
            RecordChangeColumns(logData, eAgentInfo, userID, "P4", "01010800", receiveNumber);
            
            return sendHostResultCode;
        }

        return "F";
    }

    #region 獲取主機資料

    /// <summary>
    /// 讀取第一卡人檔
    /// </summary>
    /// <param name="sUserID"></param>
    /// <param name="htOutputP4_JCF6"></param>
    /// <returns></returns>
    private Hashtable GetP4JCF6(string sUserID, Hashtable htOutputP4_JCF6)
    {
        try
        {
            Hashtable htInputP4_JCF6 = new Hashtable();// 查詢第一卡人檔上行
            htInputP4_JCF6.Add("ACCT_NBR", sUserID);
            htInputP4_JCF6.Add("FUNCTION_CODE", "1");
            htInputP4_JCF6.Add("LINE_CNT", "0000");

            htOutputP4_JCF6 = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCF6, htInputP4_JCF6, false, "1", eAgentInfo);

            if (htOutputP4_JCF6.Contains("HtgMsg"))
            {
                // 若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                if (htOutputP4_JCF6["HtgMsgFlag"].ToString() == "0")
                {
                    hostMsg = htOutputP4_JCF6["HtgMsg"].ToString();
                    clientMsg = "抓取主機資料失敗－－第一卡人檔";// 01_01010800_016
                }
                else
                {
                    clientMsg = htOutputP4_JCF6["HtgMsg"].ToString();
                }

                return null;
            }

            if (htOutputP4_JCF6["NAME_1"].ToString().Trim() == "")
            {
                clientMsg = "VD卡不可申請扣款";// 01_01010800_017
                return null;
            }

            //現行不透過批次異動電子帳單欄位故註解此判斷式 by Ares Stanley 20211207
            //if (htOutputP4_JCF6["OFF_PHONE_FLAG"].ToString() != eBill)
            //{
            //    if (!(htOutputP4_JCF6["OFF_PHONE_FLAG"].ToString() == "0" || htOutputP4_JCF6["OFF_PHONE_FLAG"].ToString() == "2"))
            //    {
            //        clientMsg = "電子帳單(MAIL)值=0或2才可以修改,其他不能異動！";// 01_01010800_006
            //        return null;
            //    }
            //}

            hostMsg = htOutputP4_JCF6["HtgSuccess"].ToString();// 主機返回成功訊息

            return htOutputP4_JCF6;
        }
        catch (Exception ex)
        {
            Logging.Log("讀取第一卡人檔失敗，" + ex.Message, LogState.Error, LogLayer.None);
            return null;
        }
    }

    /// <summary>
    /// 讀取第二卡人檔
    /// </summary>
    /// <param name="sUserID"></param>
    /// <param name="htOutputP4_JCDK"></param>
    /// <returns></returns>
    private Hashtable GetP4JCDK(string sUserID, Hashtable htOutputP4_JCDK)
    {
        try
        {
            Hashtable htInputP4_JCDK = new Hashtable();// 查詢第二卡人檔上行
            htInputP4_JCDK.Add("ACCT_NBR", sUserID);
            htInputP4_JCDK.Add("FUNCTION_CODE", "1");
            htInputP4_JCDK.Add("LINE_CNT", "0000");

            htOutputP4_JCDK = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCDK, htInputP4_JCDK, false, "1", eAgentInfo);

            if (htOutputP4_JCDK.Contains("HtgMsg"))
            {
                // 若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                if (htOutputP4_JCDK["HtgMsgFlag"].ToString() == "0")
                {
                    hostMsg = htOutputP4_JCDK["HtgMsg"].ToString();
                    clientMsg = "抓取主機資料失敗－－第二卡人檔";// 01_01010800_003
                }
                else
                {
                    clientMsg = htOutputP4_JCDK["HtgMsg"].ToString();
                }

                return null;
            }

            hostMsg += htOutputP4_JCDK["HtgSuccess"].ToString();// 主機返回成功訊息

            htOutputP4_JCDK["ACCT_NBR"] = htInputP4_JCDK["ACCT_NBR"];
            htOutputP4_JCDK["MESSAGE_TYPE"] = "";
            htOutputP4_JCDK["FUNCTION_CODE"] = "2";
            htOutputP4_JCDK["LINE_CNT"] = "0000";

            return htOutputP4_JCDK;
        }
        catch (Exception ex)
        {
            Logging.Log("讀取第二卡人檔失敗，" + ex.Message, LogState.Error, LogLayer.None);
            return null;
        }
    }

    #endregion

    #region 異動主機資料

    /// <summary>
    /// 異動第一卡人檔
    /// </summary>
    /// <param name="dtblUpdateData"></param>
    /// <param name="htOutputP4_JCF6"></param>
    /// <param name="htInputPCTIP4S"></param>
    /// <param name="intSubmit"></param>
    /// <returns></returns>
    private bool UpdatePCTIP4S(DataTable dtblUpdateData, Hashtable htOutputP4_JCF6, Hashtable htInputPCTIP4S, ref int intSubmit, out string PCTIResultCode)
    {
        PCTIResultCode = "";

        try
        {
            Hashtable htOutputPCTIP4S;// 更新第一卡人檔下行

            // 比較主機資料
            CompareHostEdit(intSubmit, htInputPCTIP4S, dtblUpdateData, payWay, accID, bcycleCode, eBill);

            // 提交修改主機資料
            htInputPCTIP4S.Add("FUNCTION_ID", "PCMC1");
            htOutputPCTIP4S = MainFrameInfo.GetMainFrameInfo(HtgType.P4_PCTI, htInputPCTIP4S, false, "2", eAgentInfo);

            if (!htOutputPCTIP4S.Contains("HtgMsg"))
            {
                // 記錄異動欄位
                RecordChangeColumns(dtblUpdateData, eAgentInfo, userID, "P4", "01010800", receiveNumber);
                intSubmit++;
                hostMsg += htOutputPCTIP4S["HtgSuccess"].ToString();// 主機返回成功訊息

                PCTIResultCode = "0000";
            }
            else
            {
                // 異動主機資料失敗
                if (htOutputPCTIP4S["HtgMsgFlag"].ToString() == "0")// 若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    PCTIResultCode = "_013";
                    hostMsg += htOutputPCTIP4S["HtgMsg"].ToString();
                    Logging.Log("交易失敗,請確認資料是否正確", LogState.Error, LogLayer.None);// 01_01010800_012
                }
                else
                {
                    Logging.Log(htOutputPCTIP4S["HtgMsg"].ToString(), LogLayer.HTG);
                }

                // 2020.07.07 (A) Ray 避免PCTIResultCode 空值情況
                if (string.IsNullOrEmpty(PCTIResultCode))
                    PCTIResultCode = "_013";
            }
            return true;
        }
        catch (Exception ex)
        {
            Logging.Log("讀取第一卡人檔失敗，" + ex.Message, LogState.Error, LogLayer.None);
            PCTIResultCode = "_013";
            return false;
        }
    }

    /// <summary>
    /// 異動第二卡人檔
    /// </summary>
    /// <param name="dtblUpdateData"></param>
    /// <param name="htOutputP4_JCDK"></param>
    /// <param name="htInputP4_JCDKS"></param>
    /// <param name="intSubmit"></param>
    /// <returns></returns>
    private bool UpdateP4_JCDKS(DataTable dtblUpdateData, Hashtable htOutputP4_JCDK, Hashtable htInputP4_JCDKS, ref int intSubmit, out string JCDKResultCode)
    {
        JCDKResultCode = "";

        try
        {
            Hashtable htOutputP4_JCDKS;// 更新第二卡人檔下行

            dtblUpdateData.Clear();

            // 比較主機資料
            CompareHost(htInputP4_JCDKS, dtblUpdateData, mobilePhone, eMail);

            // 提交修改主機資料
            htOutputP4_JCDKS = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCDK, htInputP4_JCDKS, false, "2", eAgentInfo);

            if (!htOutputP4_JCDKS.Contains("HtgMsg"))
            {
                JCDKResultCode = "0000";
                // 記錄異動欄位
                RecordChangeColumns(dtblUpdateData, eAgentInfo, userID, "P4", "01010800", receiveNumber);
                intSubmit++;
                hostMsg += htOutputP4_JCDKS["HtgSuccess"].ToString();// 主機返回成功訊息
            }
            else
            {
                // 異動主機資料失敗
                if (htOutputP4_JCDKS["HtgMsgFlag"].ToString() == "0")// 若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    JCDKResultCode = htOutputP4_JCDKS["MESSAGE_TYPE"].ToString().Trim();
                    hostMsg += htOutputP4_JCDKS["HtgMsg"].ToString();
                    clientMsg = "第二卡人檔異動失敗";// 01_01010800_014
                }
                else
                {
                    clientMsg += htOutputP4_JCDKS["HtgMsg"].ToString();
                }

                // 2020.07.07 (A) Ray 避免JCDKResultCode 空值情況
                if (string.IsNullOrEmpty(JCDKResultCode))
                    JCDKResultCode = "_014";
            }

            return true;
        }
        catch (Exception ex)
        {
            Logging.Log("讀取第二卡人檔失敗，" + ex.Message, LogState.Error, LogLayer.None);
            JCDKResultCode = "_014";
            return false;
        }
    }
    
    /// EDDA需求新增 By Ares jhun
    /// <summary>
    /// 異動第二卡人檔(行動電話)
    /// </summary>
    /// <param name="dtUpdateData"></param>
    /// <param name="htInputP4_JCDKS"></param>
    /// <returns></returns>
    private bool UpdateP4_JCDKS(DataTable dtUpdateData, Hashtable htInputP4_JCDKS)
    {
        string JCDKResultCode = string.Empty;

        try
        {
            // 提交修改主機資料
            Hashtable htOutputP4_JCDKS = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JCDK, htInputP4_JCDKS, false, "2", eAgentInfo);

            if (!htOutputP4_JCDKS.Contains("HtgMsg"))
            {
                JCDKResultCode = "0000";
                // 記錄異動欄位
                RecordChangeColumns(dtUpdateData, eAgentInfo, userID, "P4", "01010800", receiveNumber);
                hostMsg += htOutputP4_JCDKS["HtgSuccess"].ToString();// 主機返回成功訊息
            }
            else
            {
                // 異動主機資料失敗
                if (htOutputP4_JCDKS["HtgMsgFlag"].ToString() == "0")// 若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    JCDKResultCode = htOutputP4_JCDKS["MESSAGE_TYPE"].ToString().Trim();
                    hostMsg += htOutputP4_JCDKS["HtgMsg"].ToString();
                    clientMsg = "第二卡人檔異動失敗";// 01_01010800_014
                }
                else
                {
                    clientMsg += htOutputP4_JCDKS["HtgMsg"].ToString();
                }

                // 2020.07.07 (A) Ray 避免JCDKResultCode 空值情況
                if (string.IsNullOrEmpty(JCDKResultCode))
                    JCDKResultCode = "_014";
            }
        }
        catch (Exception ex)
        {
            Logging.Log("讀取第二卡人檔失敗，" + ex.Message, LogState.Error, LogLayer.None);
            JCDKResultCode = "_014";
        }

        return JCDKResultCode == "0000";
    }

    #endregion

    /// <summary>
    /// 比較主機資料，有異動則修改，無自動則刪除
    /// </summary>
    /// <param name="intSubmit"></param>
    /// <param name="htInputPCTIP4S"></param>
    /// <param name="dtblUpdateData"></param>
    /// <param name="payWay"></param>
    /// <param name="accID"></param>
    /// <param name="bcycleCode"></param>
    /// <param name="eBill"></param>
    private void CompareHostEdit(int intSubmit, Hashtable htInputPCTIP4S, DataTable dtblUpdateData, string payWay, string accID, string bcycleCode, string eBill)
    {
        // 2020.06.03 Ray Make 
        // 因發電文順序顛倒且sendHostResultCode=0009也要發電文，導致intSubmit == 0 無法正確判斷
        // 將移到判斷當sendHostResultCode = 0000 時，加入繳款狀況及帳戶ID
        //if (intSubmit == 0)
        //{
        //    // 繳款狀況(扣繳方式)
        //    CommonFunction.ContrastDataEdit(htInputPCTIP4S, dtblUpdateData, payWay, "LAST_CR_LINE_IND", BaseHelper.GetShowText("01_01010800_008"));
        //    // 帳戶ID
        //    CommonFunction.ContrastDataEdit(htInputPCTIP4S, dtblUpdateData, accID, "DD_ID", BaseHelper.GetShowText("01_01010800_009"));
        //}

        // 帳單週期
        CommonFunction.ContrastDataEdit(htInputPCTIP4S, dtblUpdateData, bcycleCode, "BILLING_CYCLE", BaseHelper.GetShowText("01_01010800_010"));
        // 電子帳單
        //CommonFunction.ContrastDataEdit(htInputPCTIP4S, dtblUpdateData, eBill, "MAIL", BaseHelper.GetShowText("01_01010800_013")); // 20211021 不異動電子帳單 By Ares Stanley
    }

    /// <summary>
    /// 比較主機資料
    /// </summary>
    /// <param name="htInputP4_JCDKS"></param>
    /// <param name="dtblUpdateData"></param>
    /// <param name="mobilePhone"></param>
    /// <param name="eMail"></param>
    private void CompareHost(Hashtable htInputP4_JCDKS, DataTable dtblUpdateData, string mobilePhone, string eMail = "")
    {
        // 行動電話
        CommonFunction.ContrastData(htInputP4_JCDKS, dtblUpdateData, mobilePhone, "MOBILE_PHONE", BaseHelper.GetShowText("01_01010800_011"));
        // Email
        //CommonFunction.ContrastData(htInputP4_JCDKS, dtblUpdateData, eMail, "EMAIL", BaseHelper.GetShowText("01_01010800_012")); // 20211021 不異動Email By Ares Stanley
    }

    /// <summary>
    /// 記錄異動欄位
    /// </summary>
    /// <param name="dtblUpdateData"></param>
    /// <param name="eAgentInfo"></param>
    /// <param name="upperUserID"></param>
    /// <param name="htgType"></param>
    /// <param name="pageInfo"></param>
    /// <param name="receiveNumber"></param>
    private void RecordChangeColumns(DataTable dtblUpdateData, EntityAGENT_INFO eAgentInfo, string upperUserID, string htgType, string pageInfo, string receiveNumber)
    {
        if (!this.InsertCustomerLog(dtblUpdateData, eAgentInfo, upperUserID, htgType, pageInfo, receiveNumber))
        {
            Logging.Log("記錄異動欄位失敗", LogState.Error, LogLayer.None);
        }
    }

    private bool InsertCustomerLog(DataTable dtblUpdate, EntityAGENT_INFO eAgentInfo, string strQueryKey, string strLogFlag, string pageInfo, string strReceiveNumber)
    {
        try
        {
            EntityCUSTOMER_LOG eCustomerLog = new EntityCUSTOMER_LOG();
            for (int i = 0; i < dtblUpdate.Rows.Count; i++)
            {
                eCustomerLog.query_key = strQueryKey;
                eCustomerLog.trans_id = pageInfo;
                eCustomerLog.field_name = dtblUpdate.Rows[i][EntityCUSTOMER_LOG.M_field_name].ToString();
                eCustomerLog.before = dtblUpdate.Rows[i][EntityCUSTOMER_LOG.M_before].ToString();
                eCustomerLog.after = dtblUpdate.Rows[i][EntityCUSTOMER_LOG.M_after].ToString();
                eCustomerLog.user_id = eAgentInfo.agent_id;
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

    /// <summary>
    /// 更新資料庫異動狀態
    /// </summary>
    /// <param name="htOutputP4_JCF6"></param>
    /// <param name="htInputP4_JCDKS"></param>
    /// <param name="eAuto_Pay_Status"></param>
    /// <param name="eAutoPay"></param>
    /// <param name="receiveNumber"></param>
    /// <param name="cusID"></param>
    /// <param name="accNo"></param>
    /// <param name="eMail"></param>
    /// <param name="eBill"></param>
    /// <param name="mobilePhone"></param>
    /// <param name="modDate"></param>
    /// <param name="acctNBR"></param>
    private bool UpdateAutoPayStatus(Hashtable htOutputP4_JCF6, Hashtable htInputP4_JCDKS, EntityAuto_Pay_Status eAuto_Pay_Status, string receiveNumber,
                                        string cusID, string accNo, string eMail, string eBill, string mobilePhone, string acctNBR)
    {
        // 記錄電子賬單是否被異動
        bool bIsEBill_Updated = false; //(htOutputP4_JCF6["OFF_PHONE_FLAG"].ToString().Trim() != eBill); 20211021 不異動電子賬單 by Ares Stanley
        // 記錄第二卡人資料是否被異動
        bool bIsP4_JCDK_Updated = ((htInputP4_JCDKS["MOBILE_PHONE"].ToString().Trim() != mobilePhone));// || ((htInputP4_JCDKS["EMAIL"].ToString().Trim() != eMail))); 20211021 不異動eMail 不異動Mail By Ares Stanley

        if (BRAuto_pay_status.AddNewEntity(eAuto_Pay_Status))
        {
            if (eAuto_Pay_Status.IsUpdateByTXT == "Y")
            {
                hostMsg += "該筆異動後銀行帳號已經存放置[自扣案件處理狀態]表中";// 01_01010800_018
            }
        }

        Logging.Log("主機資料修改完畢");// 01_01010800_015

        if (!BRTRANS_NUM.UpdateTransNum("A14"))
        {
            Logging.Log("TRANS_NUM資料庫業務類失敗", LogState.Error, LogLayer.None);
        }

        // 更新資料庫異動狀態
        EntityAUTO_PAY eAutoPay = new EntityAUTO_PAY();
        eAutoPay.Upload_Flag = "Y";
        eAutoPay.mod_date = DateTime.Now.ToString("yyyyMMdd");

        // 自扣設定(銀行帳號原本為空者，新增帳號後該欄位填入Y)
        if (htOutputP4_JCF6["CO_OWNER"].ToString().Trim() == "" && accNo != "")
            eAutoPay.Auto_Pay_Setting = "Y";
        else
            eAutoPay.Auto_Pay_Setting = "N";

        // 手機及E-MAIL設定(第二卡人檔有被異動時(行動電話、E-MAIL，新增或異動後該欄位填入Y)
        if (bIsP4_JCDK_Updated)
            eAutoPay.CellP_Email_Setting = "Y";
        else
            eAutoPay.CellP_Email_Setting = "N";

        // 電子帳單設定(電子帳單被異動時該欄位填入Y)
        if (bIsEBill_Updated)
            eAutoPay.E_Bill_Setting = "Y";
        else
            eAutoPay.E_Bill_Setting = "N";

        // 記錄持卡人ID
        eAutoPay.Acct_NBR = acctNBR;

        // 週期件註記(二KEY完自扣資料被送至主機TEMP檔者)
        eAutoPay.OutputByTXT_Setting = eAuto_Pay_Status.IsUpdateByTXT;

        string[] updateColumns = { EntityAUTO_PAY.M_Upload_Flag, EntityAUTO_PAY.M_mod_date, EntityAUTO_PAY.M_Auto_Pay_Setting,
                                   EntityAUTO_PAY.M_CellP_Email_Setting, EntityAUTO_PAY.M_E_Bill_Setting,
                                   EntityAUTO_PAY.M_OutputByTXT_Setting, EntityAUTO_PAY.M_Acct_NBR };

        if (!BRAUTO_PAY.UpdateSucc(eAutoPay, updateColumns, cusID, "0", "Y", receiveNumber))
        {
            Logging.Log("Auto_Pay更新失敗", LogState.Error, LogLayer.None);
            return false;
        }

        return true;
    }
    
    /// EDDA需求新增 By Ares jhun
    /// <summary>
    /// 更新資料庫異動狀態
    /// </summary>
    /// <param name="mobilePhoneIsDiff">行動電話是否有異動</param>
    /// <param name="eAuto_Pay_Status"></param>
    /// <param name="receiveNumber">收件編號</param>
    /// <param name="cusID">客戶ID</param>
    private bool UpdateAutoPayStatus(bool mobilePhoneIsDiff, EntityAuto_Pay_Status eAuto_Pay_Status, string receiveNumber, string cusID)
    {
        // 新增資料【Auto_Pay_Status】
        if (BRAuto_pay_status.AddNewEntity(eAuto_Pay_Status))
        {
            if (eAuto_Pay_Status.IsUpdateByTXT == "Y")
            {
                hostMsg += "該筆異動後銀行帳號已經存放置[自扣案件處理狀態]表中"; // 01_01010800_018
            }
        }

        Logging.Log("主機資料修改完畢"); // 01_01010800_015

        if (!BRTRANS_NUM.UpdateTransNum("A14"))
        {
            Logging.Log("TRANS_NUM資料庫業務類失敗", LogState.Error, LogLayer.None);
        }

        // 更新資料庫異動狀態
        EntityAUTO_PAY eAutoPay = new EntityAUTO_PAY
        {
            Upload_Flag = "Y",
            mod_date = DateTime.Now.ToString("yyyyMMdd"), // 手機(第二卡人檔有被異動時(行動電話，新增或異動後該欄位填入Y)
            CellP_Email_Setting = mobilePhoneIsDiff ? "Y" : "N", // 週期件註記(二KEY完自扣資料被送至主機TEMP檔者)
            OutputByTXT_Setting = eAuto_Pay_Status.IsUpdateByTXT
        };

        string[] updateColumns = { EntityAUTO_PAY.M_Upload_Flag, EntityAUTO_PAY.M_mod_date,
                                   EntityAUTO_PAY.M_CellP_Email_Setting, EntityAUTO_PAY.M_OutputByTXT_Setting };

        if (BRAUTO_PAY.UpdateSucc(eAutoPay, updateColumns, cusID, "0", "Y", receiveNumber)) return true;
        
        Logging.Log("Auto_Pay更新失敗", LogState.Error, LogLayer.None);
        return false;
    }

    #region 2020.05.14 Ray 測試回傳訊息
    //public string SendToHost_Ttest()
    //{
    //    //int intSubmit = 0;// 記錄異動主機成功的次數
    //    string sUserID = CommonFunction.GetSubString(userID, 0, 16);
    //    //bool result = false;

    //    string PCTIResultCode = "";
    //    string JCDKResultCode = "";
    //    bool htDataExistFlag = true;
    //    #region 獲取主機資料

    //    // 讀取第一卡人檔
    //    Hashtable htOutputP4_JCF6 = new Hashtable();// 查詢第一卡人檔下行
    //    htOutputP4_JCF6 = GetP4JCF6(sUserID, htOutputP4_JCF6);
    //    //if (htOutputP4_JCF6 == null) return false;
    //    if (htOutputP4_JCF6 == null)
    //    {
    //        htDataExistFlag = false;
    //        PCTIResultCode = "_009"; //抓取失敗
    //    };

    //    // 讀取第二卡人檔
    //    Hashtable htOutputP4_JCDK = new Hashtable();// 查詢第二卡人檔下行
    //    htOutputP4_JCDK = GetP4JCDK(sUserID, htOutputP4_JCDK);
    //    //if (htOutputP4_JCDK == null) return false;
    //    if (htOutputP4_JCDK == null)
    //    {
    //        htDataExistFlag = false;
    //        JCDKResultCode = "_003"; //抓取失敗
    //    };

    //    #endregion


    //    //bool resultPCTIP4S = false;
    //    //bool resultP4_JCDKS = false;
    //    EntityAuto_Pay_Status eAuto_Pay_Status = new EntityAuto_Pay_Status();
    //    string accNoBank_No = "";// 扣繳帳號(銀行代號)-銀行帳號
    //    string sendHostResultCode = "0000"; //0000 | 0009 | pcmc
    //    //string snedToHostResult = "F";

    //    if (htDataExistFlag == true)
    //    {
    //        // 異動customer_log記錄的Table
    //        DataTable dtblUpdateData = CommonFunction.GetDataTable();
    //        #region 異動主機資料
    //        Hashtable htInputPCTIP4S = new Hashtable();// 更新第一卡人檔上行
    //        ArrayList arrayName = new ArrayList(new object[] { "ACCT_NBR", "CO_OWNER", "CO_TAX_ID_TYPE", "DD_ID", "BILLING_CYCLE", "OFF_PHONE_FLAG" });
    //        MainFrameInfo.ChangeJCF6toPCTI(htOutputP4_JCF6, htInputPCTIP4S, arrayName);// 將異動主機的訊息預設為從主機讀取的訊息

    //        Hashtable htInputP4_JCDKS = new Hashtable();// 更新第二卡人檔上行
    //        CommonFunction.GetViewStateHt(htOutputP4_JCDK, ref htInputP4_JCDKS);
    //        #endregion
    //        #region Auto_Pay_Status

    //        //第一卡人檔 : htOutputP4_JCF6
    //        string acctNBR = htOutputP4_JCF6["ACCT_NBR"].ToString().Trim();// Auto_Pay_Status欄位
    //        string htgAcctNO = htOutputP4_JCF6["CO_OWNER"].ToString().Trim(); //701-0311373XXXX => 有時前面不一定會有銀行代碼(701-)
    //        string hgtBillingCycle = htOutputP4_JCF6["BILLING_CYCLE"].ToString().Trim(); //帳單週期
    //        string hgtEBill = htOutputP4_JCF6["OFF_PHONE_FLAG"].ToString().Trim(); //電子郵註=電子帳單=[ 0 | 1]
    //        // 2020.05.06 Ray Add 
    //        string hgtPayWay = htOutputP4_JCF6["CO_TAX_ID_TYPE"].ToString();   // 繳款狀況
    //        //第二卡人檔 : htInputP4_JCDKS
    //        string htgEmail = htInputP4_JCDKS["EMAIL"].ToString().Trim();
    //        string htgMobile = htInputP4_JCDKS["MOBILE_PHONE"].ToString().Trim();

    //        accNoBank_No = accNoBank + "-" + accNo;// 扣繳帳號(銀行代號)-銀行帳號
    //        //int hostAcctNOLength = 0; //但沒實際用到
    //        sendHostResultCode = "0000"; //0000 | 0009 | pcmc            

    //        //--- Start----------------------
    //        #region 新判斷邏輯
    //        eAuto_Pay_Status.IsUpdateByTXT = "";
    //        //要判斷的值=> 手機(mobilePhone),電子郵件(eMail), 電子帳單(eBill)        
    //        List<string> _validateArr = new List<string>();
    //        string _strformat = "{0},{1},{2}"; //舊值(主機(電文)):htgxxxx  , 新值: 郵局回覆        
    //        _validateArr.Add(string.Format(_strformat, "mobile", htgMobile, this.mobilePhone));
    //        _validateArr.Add(string.Format(_strformat, "email", htgEmail, this.eMail));
    //        _validateArr.Add(string.Format(_strformat, "ebill", hgtEBill, this.eBill));
    //        // 2020.05.06 Ray Add  繳款狀況
    //        _validateArr.Add(string.Format(_strformat, "payWay", hgtPayWay, this.payWay));
    //        // 帳單週期(bcycleCode),
    //        //_validateArr.Add(string.Format(_strformat, "billingCycle", hgtBillingCycle, this.bcycleCode));

    //        string[] _tmpArr;
    //        for (int i = 0; i < _validateArr.Count; i++)
    //        {
    //            //判斷值有無異動
    //            //主機(電文) && DB資料 需有值且不一樣就列入PCMC
    //            _tmpArr = _validateArr[i].Split(',');
    //            if (string.IsNullOrEmpty(_tmpArr[1]) == false && string.IsNullOrEmpty(_tmpArr[2]) == false
    //            && _tmpArr[1] != _tmpArr[2])
    //            {
    //                //  非電子帳單比對不一樣視為異動
    //                if (_tmpArr[0] != "ebill")
    //                {
    //                    sendHostResultCode = "pcmc";
    //                    break;
    //                }
    //                // 電子帳單特例判斷 
    //                // 主機(電文) 值非 0 & 2 視為異動
    //                if ((_tmpArr[1] != "0" && _tmpArr[1] != "2"))
    //                {
    //                    sendHostResultCode = "pcmc";
    //                }
    //                // 
    //                else if ((_tmpArr[1] == "0" || _tmpArr[1] == "2") && _tmpArr[2] == "7")
    //                {
    //                    // 主機(電文) 值0 & 2  郵局資料 值7 多判斷異動紀錄
    //                    // 判斷customer_log 是否有異動紀錄，沒異動紀錄視為異動
    //                    if (!CSIPKeyInGUI.BusinessRules_new.BRCustomer_Log.GetPostToHostAdapter_ebill(userID, receiveNumber))
    //                        sendHostResultCode = "pcmc";
    //                }
    //            }
    //        }

    //        if (sendHostResultCode != "pcmc")
    //        {
    //            //避免sendHostResultCode沒有值，預設為PCMC失敗
    //            sendHostResultCode = "pcmc";
    //            //若舊值為空白=新增帳號,所以還得再 異動扣繳帳號和銀行帳號
    //            if (string.IsNullOrEmpty(htgAcctNO) == true)
    //            {
    //                if (string.IsNullOrEmpty(accNoBank_No) != true)
    //                {
    //                    sendHostResultCode = "0000";
    //                    eAuto_Pay_Status.IsUpdateByTXT = "N";// 上送主機
    //                    // Ray 測試 make
    //                    //CommonFunction.ContrastDataEdit(htInputPCTIP4S, dtblUpdateData, accNoBank_No, "BK_ID_AC", "扣繳帳號");
    //                }
    //            }
    //            else
    //            {
    //                //銀行帳號有變
    //                if (htgAcctNO != accNoBank_No)
    //                {
    //                    // 帳單週期無異動
    //                    //20190906(U) 拿掉帳單週期的判斷條件
    //                    //20200217(U) 回覆帳單週期的判斷條件
    //                    //代表是為週期件 0009
    //                    sendHostResultCode = "0009";
    //                    eAuto_Pay_Status.IsUpdateByTXT = "Y";// 主機Temp檔
    //                    //intSubmit++;//這樣才會更新 Auto_pay,Auto_pay_status, 然後再產生於 withholding.txt 裡
    //                }
    //            }
    //        }



    //        //eAuto_Pay_Status => 這個沒有指定預設值, 所以 IsUpdateByTXT的預設值應是 "" 或 null ?            
    //        //if (eAuto_Pay_Status.IsUpdateByTXT == "N")
    //        if (sendHostResultCode != "pcmc")
    //        {
    //            // 異動第二卡人檔
    //            // bool resultP4_JCDKS = UpdateP4_JCDKS(dtblUpdateData, htOutputP4_JCDK, htInputP4_JCDKS, ref intSubmit, out JCDKResultCode);

    //            if (JCDKResultCode == "0000")
    //            {
    //                // 異動第一卡人檔
    //                //  bool resultPCTIP4S = UpdatePCTIP4S(dtblUpdateData, htOutputP4_JCF6, htInputPCTIP4S, ref intSubmit, out PCTIResultCode);
    //            }
    //        }

    //        #endregion
    //        // 當主機電文有問題時，0009 狀態改為N
    //        if (PCTIResultCode != "0000" && PCTIResultCode != "")
    //        {
    //            if (sendHostResultCode == "0009")
    //            {
    //                eAuto_Pay_Status.IsUpdateByTXT = "N";
    //            }
    //            sendHostResultCode = PCTIResultCode;
    //        }
    //        else if (JCDKResultCode != "0000" && JCDKResultCode != "")
    //        {
    //            if (sendHostResultCode == "0009")
    //            {
    //                eAuto_Pay_Status.IsUpdateByTXT = "N";
    //            }
    //            sendHostResultCode = JCDKResultCode;
    //        }
    //        #endregion
    //    }
    //    return sendHostResultCode;
    //}
    #endregion
}
