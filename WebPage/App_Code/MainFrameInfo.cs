//******************************************************************
//*  作    者：趙呂梁
//*  功能說明：主機信息作業
//*  創建日期：2009/07/28
//*  修改紀錄：
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*  Ares_Stanley       2021/11/01         20200031-CSIP EOS       理專十誡，移除異動Email、電子賬單相關。
//*******************************************************************
//20161202 (U) by Tank, add LGOR主機回覆代碼:900401
//20211005 (U) by Nash, 配合RQ-2021-016475-002無記名悠遊卡轉換，掛毀補交易設定增加回覆訊息JCAW(4023,4024,4025),JCEM(6055,6056,6057)

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Xml;
using CSIPCommonModel.EntityLayer;
using Framework.Common.HTG;
using Framework.Common.JavaScript;
using CSIPCommonModel.BaseItem;
using System.IO;
using Framework.Common.Logging;
using Framework.Common.Message;
using Framework.Common;
using Framework.Common.Utility;

/// <summary>
/// 主機操作類
/// </summary>
public class MainFrameInfo
{
    /// <summary>
    /// 上傳并取得主機資料信息
    /// </summary>
    /// <param name="type">電文枚舉類型</param>
    /// <param name="htInput">傳入參數的HashTable</param>
    /// <param name="blnIsClose">是否關閉主機Session</param>
    /// <param name="strType">電文欄位檢核類型</param>
    /// <returns>傳出參數的HashTable</returns>
    public static Hashtable GetMainFrameInfo(HtgType type, Hashtable htInput, bool blnIsClose, string strType, EntityAGENT_INFO eAgentInfo, String jobid = "")
    {
        MainFrameInfo.AddSession(htInput, eAgentInfo, type);
        string strTransactionId = type.ToString();

        string strHtgMessage = "";

        //string strErrorMessage = GetHtgMessage(type, strType) + MessageHelper.GetMessage("01_00000000_028");//*電文錯誤提示信息
        string strErrorMessage = type.ToString();

        string strIsOnLine = GetStr(type, "ISONLINE");//*是否上線

        ArrayList arrRet = new ArrayList();
        Hashtable htOutput = new Hashtable();
        HTGCommunicator hc = new HTGCommunicator(jobid);
        string strMsg = "";

        #region 2015/2/2 by Eric

        if (strIsOnLine == null)
        {
            strMsg = string.Format(MessageHelper.GetMessage("00_00000000_038"), type);
            htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");
            htOutput["HtgMsgFlag"] = "1";//*顯示主機訊息標識
            Logging.Log("ERRInfo=" + strMsg, LogLayer.HTG);

            return htOutput;
        }

        #endregion

        string strFileName = "";
        if ((type == HtgType.P4_PCTI) || (type == HtgType.P4D_PCTI))
        {
            strFileName = Configure.HTGTempletPath + "req" + strTransactionId + "_" + htInput["FUNCTION_ID"].ToString().Substring(0, 4) + ".xml";
        }
        else
        {
            strFileName = Configure.HTGTempletPath + "req" + strTransactionId + ".xml";
        }

        string SessionId = "";

        //*取得電文的SessionId
        if (htInput.Contains("sessionId"))
        {
            SessionId = htInput["sessionId"].ToString();
        }

        //*如果SessionId為空,需要連接主機得到電文SessionId
        if (SessionId == "")
        {
            if (!hc.LogonAuth(htInput, ref strMsg, strIsOnLine))
            {
                htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");

                if (strType == "100" || strType == "200")//*判斷批次作業錯誤類型
                {
                    htOutput.Add("HtgMsgFlag", "2");
                }
                else
                {
                    htOutput.Add("HtgMsgFlag", "0");//*顯示端末訊息標識
                }
                return htOutput;
            }
            else
            {
                htOutput.Add("sessionId", hc.SessionId);
                htInput["sessionId"] = hc.SessionId;
            }
        }
        else
        {
            hc.SessionId = SessionId;
        }

        if (HttpContext.Current != null)
        {
            HttpContext.Current.Session["sessionId"] = hc.SessionId;
        }

        #region 建立reqHost物件

        HTGhostgateway reqHost = new HTGhostgateway();
        try
        {
            if ((type == HtgType.P4_PCTI) || (type == HtgType.P4D_PCTI))
            {
                hc.RequestHostCreatorDnc(strFileName, ref reqHost, htInput);
            }
            else
            {
                hc.RequestHostCreator(strFileName, ref reqHost, htInput);
            }
        }
        catch
        {
            strMsg = "req" + strTransactionId + ".xml格式不正確或文件不存在";
            Logging.Log(strErrorMessage + ":" + strMsg, LogState.Error, LogLayer.HTG);
            htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");
            if (strType == "100" || strType == "200")//*判斷批次作業錯誤類型
            {
                htOutput.Add("HtgMsgFlag", "2");
            }
            else
            {
                htOutput.Add("HtgMsgFlag", "1");//*顯示端末訊息標識
            }
            return htOutput;
        }
        #endregion

        HTGhostgateway rtnHost = new HTGhostgateway();
        try
        {
            #region 取得rtnHost物件
            Logging.Log("getRtnHostBegin", LogLayer.HTG);
            Logging.Log(strIsOnLine, LogLayer.HTG);
            Logging.Log(htInput.Count.ToString(), LogLayer.HTG);
            Logging.Log(reqHost.body.data.Count.ToString(), LogLayer.HTG);
            strMsg = hc.QueryHTG(UtilHelper.GetAppSettings("HtgHttp").ToString(), reqHost, ref rtnHost, htInput, strIsOnLine);
            Logging.Log("getRtnHostEnd", LogLayer.HTG);
            if (htOutput.Contains("sessionId"))
            {
                htOutput["sessionId"] = hc.SessionId;
            }
            else
            {
                htOutput.Add("sessionId", hc.SessionId);
            }

            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session["sessionId"] = hc.SessionId;
            }

            if (strMsg != "")
            {
                //*"Session超時,..."
                htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");
                if (strType == "100" || strType == "200")//*判斷批次作業錯誤類型
                {
                    htOutput.Add("HtgMsgFlag", "2");
                }
                else
                {
                    htOutput.Add("HtgMsgFlag", "0");//*顯示端末訊息標識
                }
                return htOutput;
            }

            #endregion

            #region 判別rtnHost是否正確
            if (rtnHost.body != null)
            {
                strMsg = "主機連線失敗:" + rtnHost.body.msg.Value;
                Logging.Log(strErrorMessage + ":" + strMsg, LogState.Error, LogLayer.HTG);
                htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");
                if (strType == "100" || strType == "200")//*判斷批次作業錯誤類型
                {
                    htOutput.Add("HtgMsgFlag", "2");
                }
                else
                {
                    htOutput.Add("HtgMsgFlag", "0");//*顯示端末訊息標識
                }
                return htOutput;
            }
            #endregion
            #region 處理主機錯誤訊息
            //*主機錯誤公共判斷
            if (!hc.HTGMsgParser(rtnHost, ref strMsg))
            {
                //*"下行電文為空"
                htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");
                if (strType == "100" || strType == "200")//*判斷批次作業錯誤類型
                {
                    htOutput.Add("HtgMsgFlag", "2");
                }
                else
                {
                    htOutput.Add("HtgMsgFlag", "0");//*顯示端末訊息標識
                }
                Logging.Log(strErrorMessage + ":" + strMsg, LogState.Error, LogLayer.HTG);
                return htOutput;
            }

            #region 將資料塞入HashTable
            for (int i = 0; i < rtnHost.line.Count; i++)
            {
                if (rtnHost.line[i].msgBody.data != null)
                {
                    for (int j = 0; j < rtnHost.line[i].msgBody.data.Count; j++)
                    {
                        htOutput.Add(rtnHost.line[i].msgBody.data[j].ID.Trim(), rtnHost.line[0].msgBody.data[j].Value);
                    }
                }
            }
            #endregion

            string strLog = "";//*記錄錯誤日志變數
            switch (type)
            {
                case HtgType.P4_JCAX:
                    //*OASA
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "RTN_CD", "ERROR_DESC", "SOURCE_CODE",
                                                                                "FUNCTION_CODE", "ACCT_NBR", "INHOUSE_INQ_FLAG", "NCCC_INQ_FLAG", "COUNTERFEIT_FLAG",
                                                                                "OASA_BRAND", "FILLER5", "OASA_DEST1", "OASA_DEST2", "OASA_DEST3", "OASA_DEST4", "OASA_BLOCK_CODE",
                                                                                "OASA_PURGE_DATE", "OASA_MEMO", "OASA_REASON_CODE", "OASA_ACTION_CODE", "OASA_DATE_REPORT",
                                                                                "OASA_TIME_REPORT", "OASA_LAST_CHNG_DT", "OASA_OPID", "OASA_USER_ID", "OASA_RTNCD1", "OASA_RTNCD2",
                                                                                "OASA_RTNCD3", "OASA_RTNCD4", "FILLER6", "GUAX_VISA_FILLER", "MASTER_VIP_AMT", "VISA_CWB_REGION1", "VISA_CWB_REGION2",
                                                                                "VISA_CWB_REGION3", "VISA_CWB_REGION4", "VISA_CWB_REGION5", "VISA_CWB_REGION6","VISA_CWB_REGION7", "VISA_CWB_REGION8",
                                                                                "VISA_CWB_REGION9", "MASTER_M_PURGE_DT1", "MASTER_M_EFT_DT1", "MASTER_USER_PURGE_DT1", "MASTER_FILLER", "JCB_CRB_REGION1",
                                                                                "MASTER_M_PURGE_DT2", "MASTER_M_EFT_DT2", "MASTER_USER_PURGE_DT2","JCB_CRB_REGION2",
                                                                                "MASTER_USER_PURGE_DT3", "MASTER_M_EFT_DT3", "MASTER_M_PURGE_DT3","JCB_CRB_REGION3",
                                                                                "MASTER_USER_PURGE_DT4", "MASTER_M_EFT_DT4", "MASTER_M_PURGE_DT4", "JCB_CRB_REGION4",
                                                                                "MASTER_USER_PURGE_DT5", "MASTER_M_EFT_DT5", "MASTER_M_PURGE_DT5", "JCB_CRB_REGION5",
                                                                                "MASTER_USER_PURGE_DT6", "MASTER_M_EFT_DT6", "MASTER_M_PURGE_DT6"});
                    }

                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "RTN_CD", "ERROR_DESC" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["RTN_CD"].ToString() != "0001" && htOutput["RTN_CD"].ToString() != "0000")
                    {
                        if (htOutput["ERROR_DESC"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgMsg", htOutput["ERROR_DESC"].ToString());
                        }
                        else
                        {
                            htOutput.Add("HtgMsg", MessageHelper.GetMessage("01_00000000_030"));
                        }
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["RTN_CD"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        //htOutput.Add("HtgSuccess", htOutput["RTN_CD"].ToString() + " " + GetMessageType(type, htOutput["RTN_CD"].ToString()));
                        if (htOutput["ERROR_DESC"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgSuccess", htOutput["RTN_CD"].ToString() + " " + htOutput["ERROR_DESC"].ToString());
                        }
                        else
                        {
                            htOutput.Add("HtgSuccess", htOutput["RTN_CD"].ToString() + " " + MessageHelper.GetMessage("01_00000000_029"));
                        }
                    }
                    break;

                case HtgType.P4_JCAW:
                    //*EXMS1231
                    if (strType == "1")
                    {
                        //edit by mel 20141203 因應一卡通 毁補轉一卡通悠遊需求 9-毀補轉悠遊或一卡通
                        /*
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE", "LINE_CNT", 
                                                                                "CARD_NO", "SELF_TAKE", "EMBNAME", "EMBTYPE", "EXPDATE_MM", 
                                                                                "EXPDATE_YY", "MEMNO", "CARD_NO_NEW"});
                         */
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE",
                                                                                "FUN9_CONV", "TSCC_SALES_FLAG","HEAD_FILLER",
                                                                               "CARD_NO", "SELF_TAKE", "EMBNAME", "EMBTYPE", "EXPDATE_MM",
                                                                                "EXPDATE_YY", "MEMNO", "CARD_NO_NEW","TSCC_AUTOLOAD_FLAG"});

                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0001" && htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;

                case HtgType.P4_JCF7:
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "PROGRAM_VERSION","USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE", "LINE_CNT", "ACCT_NBR",
                                                             "PYMT_FLAG","FIXED_PYMT_AMNT","CURR_DUE","PAST_DUE","30DAYS_DELQ","60DAYS_DELQ","90DAYS_DELQ",
                                                             "120DAYS_DELQ","150DAYS_DELQ","180DAYS_DELQ","210DAYS_DELQ","USER_CODE","USER_CODE_2",
                                                             "BLOCK_CODE","CHGOFF_STATUS_FLAG","SHORT_NAME","CARD_EXPIR_DTE","DELQ_HIST1","DELQ_HIST2",
                                                              "DELQ_HIST3","DELQ_HIST4","DELQ_HIST5","DELQ_HIST6","DELQ_HIST7","DELQ_HIST8","DELQ_HIST9",
                                                              "DELQ_HIST10","DELQ_HIST11","DELQ_HIST12","DELQ_HIST13","DELQ_HIST14",
                                                              "DELQ_HIST15","DELQ_HIST16","DELQ_HIST17","DELQ_HIST18","DELQ_HIST19","DELQ_HIST20","DELQ_HIST21",
                                                              "DELQ_HIST22","DELQ_HIST23","DELQ_HIST24"});
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (!(htOutput["MESSAGE_TYPE"].ToString() == "0000" || htOutput["MESSAGE_TYPE"].ToString() == "0001"))
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;

                case HtgType.P4A_JCGX:
                case HtgType.P4_JCGX:
                    //*EXMS 6063
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE", "LINE_CNT","MERCH_ACCT", "CARD_TYPE", "AGENT_BANK_NMBR",
                                                                                "DISCOUNT_RATE", "AGENT_DESC", "MESSAGE_CHI","AGENT_BANK_NMBR1","DISCOUNT_RATE1","CARD_TYPE1","CHANGE_ID1",
                                                                                "CHANGE_DATE1","CHANGE_ID_B1","CHANGE_DATE_B1","AGENT_DESC1","AGENT_BANK_NMBR2","DISCOUNT_RATE2","CARD_TYPE2",
                                                                                "CHANGE_ID2","CHANGE_DATE2","CHANGE_ID_B2","CHANGE_DATE_B2","AGENT_DESC2","AGENT_BANK_NMBR3","DISCOUNT_RATE3",
                                                                                "CARD_TYPE3","CHANGE_ID3","CHANGE_DATE3","CHANGE_ID_B3","CHANGE_DATE_B3","AGENT_DESC3","AGENT_BANK_NMBR4",
                                                                                "DISCOUNT_RATE4","CARD_TYPE4","CHANGE_ID4","CHANGE_DATE4","CHANGE_ID_B4","CHANGE_DATE_B4","AGENT_DESC4","AGENT_BANK_NMBR5",
                                                                                "DISCOUNT_RATE5","CARD_TYPE5","CHANGE_ID5","CHANGE_DATE5","CHANGE_ID_B5","CHANGE_DATE_B5","AGENT_DESC5","LINE_CNT"});
                    }

                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE", "MESSAGE_CHI" });
                    }
                    if (strType == "21")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE", "MESSAGE_CHI" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0000" && htOutput["MESSAGE_TYPE"].ToString() != "0001")
                    {
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgMsg", htOutput["MESSAGE_CHI"].ToString());
                        }
                        else
                        {
                            htOutput.Add("HtgMsg", MessageHelper.GetMessage("01_00000000_030"));
                        }
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        //htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["MESSAGE_CHI"].ToString());
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["MESSAGE_CHI"].ToString() + " ");
                        }
                        else
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("01_00000000_029") + " ");
                        }

                    }
                    break;

                case HtgType.P4A_JCGQ:
                case HtgType.P4_JCGQ:
                    //*EXMS 6001
                    if (strType == "11")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "MESSAGE_CHI", "FUNCTION_CODE", "FORCE_FLAG", "CORP_NO", "APPL_NO",
                                                                                "BUILD_DATE","CREDIT_NO","CAPITAL","REG_NAME","ORGAN_TYPE","BUSINESS_NAME","RISK_FLAG","MARGIN_FLAG",
                                                                                "OWNER_NAME","OWNER_ID","OWNER_PHONE_AREA","OWNER_PHONE_NO","OWNER_PHONE_EXT","CHANGE_DATE1",
                                                                               "CHANGE_FLAG1","BIRTHDAY1","PHOTO_FLAG1","AT1","OWNER_CITY","OWNER_ADDR1",
                                                                               "OWNER_ADDR2","MANAGER_NAME","MANAGER_ID","MANAGER_PHONE_AREA","MANAGER_PHONE_NO",
                                                                               "MANAGER_PHONE_EXT","CHANGE_DATE2","CHANGE_FLAG2","BIRTHDAY2","PHOTO_FLAG2","AT2","CONTACT_NAME",
                                                                               "CONTACT_PHONE_AREA","CONTACT_PHONE_NO","CONTACT_PHONE_EXT","FAX_AREA",
                                                                               "FAX_PHONE_NO","REMITTANCE_NAME","REMITTANCE_ID","CHANGE_DATE3","CHANGE_FLAG3",
                                                                               "BIRTHDAY3","PHOTO_FLAG3","AT3","REG_CITY","REG_ADDR1","REG_ADDR2","REAL_CITY","REAL_ADDR1","REAL_ADDR2",
                                                                               "REAL_ZIP","MARKETING_FLAG1","MARKETING_FLAG2","NOSIGN_AMT","JCIC_CODE","DDA_BANK_NAME",
                                                                               "DDA_BANK_BRANCH","DDA_BANK_NAME_3RD","DDA_ACCT_NAME","DDA_BANK_BRANCH_3RD",
                                                                               "DDA_ACCT_NAME_3RD","IPMR_PREV_DESC","SALE_NAME","INVOICE_CYCLE","REDEEM_CYCLE",
                                                                               "UPDATE_DATE","UPDATE_USER","CREATE_DATE","CREATE_USER","MPOS_FLAG","GRANT_FEE_FLAG","HEADQUARTER_CORPNO",
                                                                               "COUNTRY_CODE","PASSPORT_NO","PASSPORT_EXPDT","RESIDENT_NO","RESIDENT_EXPDT","EMAIL","OWNER_LNAM_FLAG","CONTACT_LNAM_FLAG"});
                    }
                    if (strType == "21")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE", "MESSAGE_CHI" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (!(htOutput["MESSAGE_TYPE"].ToString() == "0001" || htOutput["MESSAGE_TYPE"].ToString() == "0000"))
                    {
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgMsg", htOutput["MESSAGE_CHI"].ToString());
                        }
                        else
                        {
                            htOutput.Add("HtgMsg", MessageHelper.GetMessage("01_00000000_030"));
                        }

                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        //htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["MESSAGE_CHI"].ToString());
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["MESSAGE_CHI"].ToString() + " ");
                        }
                        else
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("01_00000000_029") + " ");
                        }

                    }
                    break;

                case HtgType.P4_JCHO:
                    //* EXMS  1255
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE", "ID", "NAME" });
                    }
                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0001" && htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        strHtgMessage = "國旅卡維護：" + htOutput["ERROR_MESSAGE"].ToString() + "．";
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;

                case HtgType.P4D_JCF6:
                case HtgType.P4_JCF6:
                    //* JCF6
                    if (strType == "1" || strType == "12" || strType == "100")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "PROGRAM_VERSION", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE",
                                                              "LINE_CNT","ACCT_NBR","NAME_1","CITY","ADDR_1","ADDR_2","EMPLOYER","HOME_PHONE","OFFICE_PHONE",
                                                              "MANAGE_ZIP_CODE","ZIP","MEMO_1","MEMO_2","CO_OWNER","CO_TAX_ID_TYPE","DD_ID","BILLING_CYCLE",
                                                              "BIRTH_DATE","NAME_1_2","EU_NBR_OF_DEPS","MEMBER_SINCE","OFF_PHONE_FLAG","EU_DIRECT_MAIL","WAIVE_FLAG",
                                                             "EU_CUSTOMER_CLASS","GRADUATE_YYMM","SHORT_NAME1","SHORT_NAME2","SHORT_NAME3","SHORT_NAME4","SHORT_NAME5",
                                                             "SHORT_NAME6","SHORT_NAME7","SHORT_NAME8","SHORT_NAME9","SHORT_NAME10","SHORT_NAME11","SHORT_NAME12",
                                                             "SHORT_NAME13","SHORT_NAME14","SHORT_NAME15","SHORT_NAME16","SHORT_NAME17","SHORT_NAME18","EMBOSSER_NAME_11",
                                                             "EMBOSSER_NAME_12","EMBOSSER_NAME_13","EMBOSSER_NAME_14","EMBOSSER_NAME_15","EMBOSSER_NAME_16",
                                                              "EMBOSSER_NAME_17","EMBOSSER_NAME_18","EMBOSSER_NAME_19","EMBOSSER_NAME_110","EMBOSSER_NAME_111","EMBOSSER_NAME_112",
                                                               "EMBOSSER_NAME_113","EMBOSSER_NAME_114","EMBOSSER_NAME_115","EMBOSSER_NAME_116","EMBOSSER_NAME_117",
                                                              "EMBOSSER_NAME_118","CARD_NMBR1","CARD_NMBR2","CARD_NMBR3","CARD_NMBR4","CARD_NMBR5","CARD_NMBR6","CARD_NMBR7",
                                                               "CARD_NMBR8","CARD_NMBR9","CARD_NMBR10","CARD_NMBR11","CARD_NMBR12","CARD_NMBR13","CARD_NMBR14","CARD_NMBR15",
                                                               "CARD_NMBR16","CARD_NMBR17","CARD_NMBR18","CARD_ID1","CARD_ID2","CARD_ID3","CARD_ID4","CARD_ID5","CARD_ID6",
                                                             "CARD_ID7","CARD_ID8","CARD_ID9","CARD_ID10","CARD_ID11","CARD_ID12","CARD_ID13","CARD_ID14","CARD_ID15",
                                                             "CARD_ID16","CARD_ID17","CARD_ID18","TYPE1","TYPE2","TYPE3","TYPE4","TYPE5","TYPE6","TYPE7","TYPE8","TYPE9",
                                                             "TYPE10","TYPE11","TYPE12" ,"TYPE13","TYPE14","TYPE15","TYPE16","TYPE17","TYPE18"});
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        if (strType == "100")//*判斷批次作業錯誤類型
                        {
                            htOutput["HtgMsgFlag"] = "2";
                        }
                        return htOutput;
                    }

                    if (!(htOutput["MESSAGE_TYPE"].ToString() == "0000" || htOutput["MESSAGE_TYPE"].ToString() == "0001"))
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;

                case HtgType.P4_JCDK:
                    //*P4_JCDK
                    if (strType == "1" || strType == "100")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "PROGRAM_VERSION", "USER_ID", "MESSAGE_TYPE" ,
                                                                                    "FUNCTION_CODE","LINE_CNT","ACCT_NBR", "MOBILE_PHONE","8DCALL","PAGER","EMAIL",
                                                                                    "TITLE","TEL_PERM","E_SERVICE_CODE","CUS2_PERM_ZIP","CUS2_PERM_CITY","CUS2_PERM_ADDR_1",
                                                                                    "CUS2_PERM_ADDR_2","CUS2_INFORMANT_NAME_1","CUS2_INFORMANT_ZONE_1","CUS2_INFORMANT_NO_1",
                                                                                    "CUS2_RELATIVE_1","CUS2_INFORMANT_NAME_2","CUS2_INFORMANT_ZONE_2","CUS2_INFORMANT_NO_2",
                                                                                    "CUS2_RELATIVE_2","CUS2_PARENT_NAME","CUS2_PARENT_ZIP","CUS2_PARENT_CITY","CUS2_PARENT_ADDR_1",
                                                                                    "CUS2_PARENT_ADDR_2","CUS2_PARENT_ZONE","CUS2_PARENT_NO"});
                    }

                    if (strType == "2" || strType == "200")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        if (strType == "100" || strType == "200")//*判斷批次作業錯誤類型
                        {
                            htOutput["HtgMsgFlag"] = "2";
                        }
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0001" && htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;

                case HtgType.P4_JCBG:
                    //*P4_JCBG
                    if (strType == "1" || strType == "100")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE", "LINE_CNT", "ACCT_NBR", "CON_RPT", "MOBILE_PHONE", "EMAIL", "TITLE", "E_SERVICE_CODE", "FAX_NO", "PERM_ZIP", "PERM_CITY", "PERM_ADDR_1", "PERM_ADDR_2", "INFORMANT_NAME_1", "INFORMANT_TEL_NO_1", "INFORMANT_NAME_2", "INFORMANT_TEL_NO_2", "TEL_PERM", "STUS_ID", "STUS_TELO", "STUS_TELH", "STUS_TELP", "STUS_FAX", "STUS_MOBIL", "STUS_ADDB", "STUS_ADDP", "STUS_EMAIL", "GRADE_SCHOOL", "GRADE_SCHOOL_LOCATE", "OVERSEA_PHONE", "SMS_NP_FLAG"
                        });
                        //"TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE", "LINE_CNT", "ACCT_NBR", "CON_RPT", "MOBILE_PHONE", "EMAIL", "TITLE", "E_SERVICE_CODE", "FAX_NO", "PERM_ZIP", "PERM_CITY", "PERM_ADDR_1", "PERM_ADDR_2", "INFORMANT_NAME_1", "INFORMANT_TEL_NO_1", "INFORMANT_NAME_2", "INFORMANT_TEL_NO_2", "TEL_PERM", "STUS_ID", "STUS_TELO", "STUS_TELH", "STUS_TELP", "STUS_FAX", "STUS_MOBIL", "STUS_ADDB", "STUS_ADDP", "STUS_EMAIL", "GRADE_SCHOOL", "GRADE_SCHOOL_LOCATE", "OVERSEA_COUNTRY_CODE", "OVERSEA_PHONE_NO", "SMS_NP_FLAG", "VALID_SEL", "TMAL_E_MAIL", "TMAL_E_MAIL_STATUS", "TMAL_CREATE_DATE", "SAME_FLAG", "EBILL_FLAG", "VERIFY_SEQ_NO"
                    }

                    if (strType == "2" || strType == "200")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        if (strType == "100" || strType == "200")//*判斷批次作業錯誤類型
                        {
                            htOutput["HtgMsgFlag"] = "2";
                        }
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0001" && htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;

                case HtgType.P4_PCTI:
                    if (strType == "2" || strType == "200")
                    {
                        arrRet = new ArrayList(new object[] { "ERR_MSG_DATA", "ERR_MSG_NO", "ERR_MSG_CODE" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        if (strType == "200")//*判斷批次作業錯誤類型
                        {
                            htOutput["HtgMsgFlag"] = "2";
                        }
                        return htOutput;
                    }

                    if (htOutput["ERR_MSG_DATA"].ToString() != "00999")
                    {
                        strHtgMessage = GetMessageType(type, htOutput["ERR_MSG_DATA"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["ERR_MSG_DATA"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["ERR_MSG_DATA"].ToString() + " " + GetMessageType(type, htOutput["ERR_MSG_DATA"].ToString()));
                    }
                    break;

                case HtgType.P4D_PCTI:
                    if (strType == "22")
                    {
                        arrRet = new ArrayList(new object[] { "ERR_MSG_DATA", "ERR_MSG_NO", "ERR_MSG_CODE" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["ERR_MSG_DATA"].ToString() != "00999")
                    {
                        strHtgMessage = GetMessageType(type, htOutput["ERR_MSG_DATA"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["ERR_MSG_DATA"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["ERR_MSG_DATA"].ToString() + " " + GetMessageType(type, htOutput["ERR_MSG_DATA"].ToString()));
                    }
                    break;

                case HtgType.P4_JCIL:
                    //*EXMS_P4_sessionA
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE",
                                                                                    "LINE_CNT", "MER_NO", "MER_NEME", "CORP_NO", "OWNER_NAME", "BANK_NAME",
                                                                                    "ACCT_NEME", "ACCT_NO", "CONTACT_TEL", "CONTACT_FAX", "ADDRESS1", "ADDRESS2",
                                                                                    "ADDRESS3", "DESCRIPTION", "CREATE_DATE", "CREATE_USERID" });
                    }
                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0001" && htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;

                case HtgType.P4_JCAA:

                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE",
                                                                                    "LINE_CNT", "ACCT_NBR", "DTE_BIRTH","HOME_PHONE","EMPLOYER","CUST_GRADE",
                                                                                    "EU_TITLE_1","AVAIL_CASH","OFFICE_PHON","TITLE","DELQ","PASSWORD","SHORT_NAME",
                                                                                    "MOBILE_PHONE","CITY","ADDR_1","ADDR_2","CO_OWNER", "SINCE", "CRG","CYCLE","EMAIL",
                                                                                    "ARG","SERV_GRAD","E_SERVICE_CODE","ZIP","ADDR_DATE","SMSA","FAX_NO", "TYPE1","TYPE2",
                                                                                    "TYPE3","TYPE4","TYPE5","TYPE6","TYPE7","TYPE8","TYPE9","TYPE10","TYPE11","TYPE12","TYPE13",
                                                                                    "TYPE14","TYPE15","TYPE16","TYPE17","TYPE18","TYPE19","TYPE20","CARD_NMBR1","CARD_NMBR2",
                                                                                    "CARD_NMBR3","CARD_NMBR4","CARD_NMBR5","CARD_NMBR6","CARD_NMBR7","CARD_NMBR8",
                                                                                    "CARD_NMBR9","CARD_NMBR10","CARD_NMBR11","CARD_NMBR12","CARD_NMBR13","CARD_NMBR14",
                                                                                    "CARD_NMBR15","CARD_NMBR16","CARD_NMBR17","CARD_NMBR18","CARD_NMBR19","CARD_NMBR20",
                                                                                    "BLOCK_CODE1","BLOCK_CODE2","BLOCK_CODE3","BLOCK_CODE4","BLOCK_CODE5","BLOCK_CODE6",
                                                                                    "BLOCK_CODE7","BLOCK_CODE8","BLOCK_CODE9","BLOCK_CODE10","BLOCK_CODE11","BLOCK_CODE12",
                                                                                    "BLOCK_CODE13","BLOCK_CODE14","BLOCK_CODE15","BLOCK_CODE16","BLOCK_CODE17","BLOCK_CODE18",
                                                                                    "BLOCK_CODE19","BLOCK_CODE20","CUSTID1","CUSTID2","CUSTID3","CUSTID4","CUSTID5","CUSTID6","CUSTID7",
                                                                                    "CUSTID8","CUSTID9","CUSTID10","CUSTID11","CUSTID12","CUSTID13","CUSTID14","CUSTID15","CUSTID16","CUSTID17",
                                                                                    "CUSTID18","CUSTID19","CUSTID20","NAME11","NAME12","NAME13","NAME14","NAME15","NAME16","NAME17","NAME18",
                                                                                    "NAME19","NAME110","NAME111","NAME112","NAME113","NAME114","NAME115","NAME116","NAME117","NAME118",
                                                                                    "NAME119","NAME120","USER_CODE_31","USER_CODE_32","USER_CODE_33","USER_CODE_34","USER_CODE_35","USER_CODE_36",
                                                                                    "USER_CODE_37","USER_CODE_38","USER_CODE_39","USER_CODE_310","USER_CODE_311","USER_CODE_312","USER_CODE_313",
                                                                                    "USER_CODE_314","USER_CODE_315","USER_CODE_316","USER_CODE_317","USER_CODE_318","USER_CODE_319","USER_CODE_320"});
                    }
                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0001" && htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;

                case HtgType.P4_JCAT:

                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE","LINE_CNT", "ACCT_NBR",
                            "CM_CARD_TYPE20","CM_CARD_TYPE19","CM_CARD_TYPE18","CM_CARD_TYPE17","CM_CARD_TYPE16","CM_CARD_TYPE15","CM_CARD_TYPE14","CM_CARD_TYPE13",
                            "CM_CARD_TYPE12","CM_CARD_TYPE11","CM_CARD_TYPE10","CM_CARD_TYPE9","CM_CARD_TYPE8","CM_CARD_TYPE7","CM_CARD_TYPE6","CM_CARD_TYPE5",
                            "CM_CARD_TYPE4","CM_CARD_TYPE3","CM_CARD_TYPE2","CM_CARD_TYPE1","MEMBER_NO20","MEMBER_NO19","MEMBER_NO18","MEMBER_NO17","MEMBER_NO16",
                            "MEMBER_NO15","MEMBER_NO14","MEMBER_NO13","MEMBER_NO12","MEMBER_NO11","MEMBER_NO10","MEMBER_NO9","MEMBER_NO8","MEMBER_NO7","MEMBER_NO6",
                            "MEMBER_NO5","MEMBER_NO4","MEMBER_NO3","MEMBER_NO2","MEMBER_NO1","CARD_NMBR20","CARD_NMBR19","CARD_NMBR18","CARD_NMBR17","CARD_NMBR16",
                            "CARD_NMBR15","CARD_NMBR14","CARD_NMBR13","CARD_NMBR12","CARD_NMBR11","CARD_NMBR10","CARD_NMBR9","CARD_NMBR8","CARD_NMBR7","CARD_NMBR6",
                            "CARD_NMBR5","CARD_NMBR4","CARD_NMBR3","CARD_NMBR2","CARD_NMBR1","EMBOSSER_NAME20","EMBOSSER_NAME19","EMBOSSER_NAME18","EMBOSSER_NAME17",
                            "EMBOSSER_NAME16","EMBOSSER_NAME15","EMBOSSER_NAME14","EMBOSSER_NAME13","EMBOSSER_NAME12","EMBOSSER_NAME11","EMBOSSER_NAME10","EMBOSSER_NAME9",
                            "EMBOSSER_NAME8","EMBOSSER_NAME7","EMBOSSER_NAME6","EMBOSSER_NAME5","EMBOSSER_NAME4","EMBOSSER_NAME3","EMBOSSER_NAME2","EMBOSSER_NAME1",
                            "CARD_NAME20","CARD_NAME19","CARD_NAME18","CARD_NAME17","CARD_NAME16","CARD_NAME15","CARD_NAME14","CARD_NAME13","CARD_NAME12","CARD_NAME11",
                            "CARD_NAME10","CARD_NAME9","CARD_NAME8","CARD_NAME7","CARD_NAME6","CARD_NAME5","CARD_NAME4","CARD_NAME3","CARD_NAME2","CARD_NAME1"
                        });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0001" && htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;

                case HtgType.P4_JCEM:

                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE",
                                                                                    "LINE_CNT", "ACCT_NBR", "CARD_TAKE","EMBOSS_NAME","EMBOSS_TYPE",
                                                                                    "EXPIRE_DATE","XFR_CARD_NO","BLOCK_CODE","STATUS","PREV_ACTION",
                                                                                    "DTE_LST_REQ","MEM_NO","CARDHOLDER_ID","CARDHOLDER_NAME","CUSTOMER_NAME",
                                                                                    "CITY","ADDR_1","ADDR_2","EMBOSS_DATE","EMBOSS_TIME"});
                    }
                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0001" && htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;

                case HtgType.P4A_JCGR:
                    //*EXMS_P4A_sessionB
                    if (strType == "11")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "MESSAGE_CHI", "FUNCTION_CODE",
                                                                                    "MERCHANT_NO", "BUILD_DATE", "MERCHANDISE", "CREDIT_NO", "CAPITAL", "REG_NAME" ,
                                                                                    "ORGAN_TYPE","MERCH_PICT","BUSINESS_NAME","RISK_FLAG","DISOBEY","DDA_BANK_NAME",
                                                                                    "DDA_BANK_BRANCH","CHECK_CODE","MERCH_VCR","DDA_ACCT_NAME","OWNER_NAME",
                                                                                    "OWNER_ID","OWNER_PHONE_AREA","OWNER_PHONE_NO","OWNER_PHONE_EXT","OWNER_CITY",
                                                                                    "OWNER_ADDR1","OWNER_ADDR2","MANAGER_NAME","MANAGER_ID","MANAGER_PHONE_AREA",
                                                                                    "MANAGER_PHONE_NO","MANAGER_PHONE_EXT","REMITTANCE_NAME","REMITTANCE_ID","PROJECT_NO",
                                                                                    "CONTACT_NAME","CONTACT_PHONE_AREA","CONTACT_PHONE_NO","CONTACT_PHONE_EXT","FAX_AREA",
                                                                                    "FAX_PHONE_NO","REG_CITY","REG_ADDR1","REG_ADDR2","REAL_CITY","REAL_ADDR1","REAL_ADDR2",
                                                                                    "REAL_ZIP","REDEEM_CYCLE","IMPRINTER_TYPE1","IMPRINTER_TYPE2","IMPRINTER_QTY1","IMPRINTER_QTY2",
                                                                                    "IMPRINTER_DEPO","POS_TYPE1","POS_TYPE2","POS_QTY1","POS_QTY2","POS_DEPO","EDC_TYPE1","EDC_TYPE2",
                                                                                    "EDC_QTY1","EDC_QTY2","EDC_DEPO","BLACK_CODE","BLACK_QTY","SALE_NAME","JOINT_TYPE","INVOICE_CYCLKE",
                                                                                    "CORP_NO","CORP_SEQ","APPL_NO","MERCHANT_TYPE","MARGIN_FLAG","USER_DATA","FILE_NO","UPDATE_DATE",
                                                                                    "UPDATE_USER","APPROVED_DATE","OWNER_LNAM_FLAG","CONTACT_LNAM_FLAG"});
                    }
                    if (strType == "21")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE", "MESSAGE_CHI" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0000" && htOutput["MESSAGE_TYPE"].ToString() != "0001")
                    {
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgMsg", htOutput["MESSAGE_CHI"].ToString());
                        }
                        else
                        {
                            htOutput.Add("HtgMsg", MessageHelper.GetMessage("01_00000000_030"));
                        }
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        //htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["MESSAGE_CHI"].ToString());
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["MESSAGE_CHI"].ToString() + " ");
                        }
                        else
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("01_00000000_029") + " ");
                        }
                    }
                    break;

                case HtgType.P4A_JC66:
                    //*EXMS_P4A_sessionB
                    if (strType == "11")
                    {
                        //將RTN內所有BODY的XML做成陣列，以便利主機回傳值轉換HASHTABLE
                        //arrRet = new ArrayList(new object[] { "USER_ID","MESSAGE_TYPE","MESSAGE_CHI","FUNCTION_CODE","BRCH_CORP_NO",
                        //"BRCH_SEQ","HCOP_CORP_NO","HCOP_CORP_SEQ","CORP_TYPE","NATION",
                        //"BIRTH_DATE","PERM_CITY","PERM_ADDR1","PERM_ADDR2","CHINESE_NAME",
                        //"ENGLISH_NAME","ID","OWNER_ID_ISSUE_DATE","OWNER_ID_ISSUE_PLACE",
                        //"OWNER_ID_REPLACE_TYPE","ID_PHOTO_FLAG","PASSPORT","PASSPORT_EXP_DATE",
                        //"RESIDENT_NO","RESIDENT_EXP_DATE","OTHER_CERT","OTHER_CERT_EXP_DATE",
                        //"COMP_TEL"
                        //});

                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID","MESSAGE_TYPE","MESSAGE_CHI","FUNCTION_CODE","CORP_NO",
                        "CORP_SEQ","CORP_TYPE","REGISTER_NATION","CORP_REG_ENG_NAME","REG_NAME","BUILD_DATE","CC","REG_CITY","REG_ADDR1","REG_ADDR2",
                        "EMAIL","NP_COMPANY_NAME","OWNER_NATION","OWNER_CHINESE_NAME","OWNER_ENGLISH_NAME","OWNER_BIRTH_DATE","OWNER_ID","OWNER_ID_ISSUE_DATE",
                        "OWNER_ID_ISSUE_PLACE","OWNER_ID_REPLACE_TYPE","ID_PHOTO_FLAG","PASSPORT","PASSPORT_EXP_DATE","RESIDENT_NO","RESIDENT_EXP_DATE",
                        "OTHER_CERT","OTHER_CERT_EXP_DATE","COMPLEX_STR_CODE","ISSUE_STOCK_FLAG","COMP_TEL","MAILING_CITY","MAILING_ADDR1","MAILING_ADDR2",
                        "PRIMARY_BUSI_COUNTRY","BUSI_RISK_NATION_FLAG","BUSI_RISK_NATION_1","BUSI_RISK_NATION_2","BUSI_RISK_NATION_3","BUSI_RISK_NATION_4",
                        "BUSI_RISK_NATION_5","OVERSEAS_FOREIGN","OVERSEAS_FOREIGN_COUNTRY","REGISTER_US_STATE","BUSINESS_ORGAN_TYPE","CREATE_DATE","STATUS",
                        "BENE_NATION1","BENE_NATION2","BENE_NATION3","BENE_NATION4","BENE_NATION5","BENE_NATION6","BENE_NATION7","BENE_NATION8","BENE_NATION9",
                        "BENE_NATION10","BENE_NATION11","BENE_NATION12",
                        "BENE_NAME1","BENE_NAME2","BENE_NAME3","BENE_NAME4","BENE_NAME5","BENE_NAME6","BENE_NAME7","BENE_NAME8","BENE_NAME9","BENE_NAME10",
                        "BENE_NAME11","BENE_NAME12",
                        "BENE_BIRTH_DATE1","BENE_BIRTH_DATE2","BENE_BIRTH_DATE3","BENE_BIRTH_DATE4","BENE_BIRTH_DATE5","BENE_BIRTH_DATE6","BENE_BIRTH_DATE7",
                        "BENE_BIRTH_DATE8","BENE_BIRTH_DATE9","BENE_BIRTH_DATE10","BENE_BIRTH_DATE11","BENE_BIRTH_DATE12",
                        "BENE_ID1","BENE_ID2","BENE_ID3","BENE_ID4","BENE_ID5","BENE_ID6","BENE_ID7","BENE_ID8","BENE_ID9","BENE_ID10","BENE_ID11","BENE_ID12",
                        "BENE_PASSPORT1","BENE_PASSPORT2","BENE_PASSPORT3","BENE_PASSPORT4","BENE_PASSPORT5","BENE_PASSPORT6","BENE_PASSPORT7","BENE_PASSPORT8",
                        "BENE_PASSPORT9","BENE_PASSPORT10","BENE_PASSPORT11","BENE_PASSPORT12",
                        "BENE_PASSPORT_EXP1","BENE_PASSPORT_EXP2","BENE_PASSPORT_EXP3","BENE_PASSPORT_EXP4","BENE_PASSPORT_EXP5","BENE_PASSPORT_EXP6",
                        "BENE_PASSPORT_EXP7","BENE_PASSPORT_EXP8","BENE_PASSPORT_EXP9","BENE_PASSPORT_EXP10","BENE_PASSPORT_EXP11","BENE_PASSPORT_EXP12",
                        "BENE_RESIDENT_NO1","BENE_RESIDENT_NO2","BENE_RESIDENT_NO3","BENE_RESIDENT_NO4","BENE_RESIDENT_NO5","BENE_RESIDENT_NO6",
                        "BENE_RESIDENT_NO7","BENE_RESIDENT_NO8","BENE_RESIDENT_NO9","BENE_RESIDENT_NO10","BENE_RESIDENT_NO11","BENE_RESIDENT_NO12",
                        "BENE_RESIDENT_EXP1","BENE_RESIDENT_EXP2","BENE_RESIDENT_EXP3","BENE_RESIDENT_EXP4","BENE_RESIDENT_EXP5","BENE_RESIDENT_EXP6",
                        "BENE_RESIDENT_EXP7","BENE_RESIDENT_EXP8","BENE_RESIDENT_EXP9","BENE_RESIDENT_EXP10","BENE_RESIDENT_EXP11","BENE_RESIDENT_EXP12",
                        "BENE_OTH_CERT1","BENE_OTH_CERT2","BENE_OTH_CERT3","BENE_OTH_CERT4","BENE_OTH_CERT5","BENE_OTH_CERT6","BENE_OTH_CERT7","BENE_OTH_CERT8",
                        "BENE_OTH_CERT9","BENE_OTH_CERT10","BENE_OTH_CERT11","BENE_OTH_CERT12",
                        "BENE_OTH_CERT_EXP1","BENE_OTH_CERT_EXP2","BENE_OTH_CERT_EXP3","BENE_OTH_CERT_EXP4","BENE_OTH_CERT_EXP5","BENE_OTH_CERT_EXP6",
                        "BENE_OTH_CERT_EXP7","BENE_OTH_CERT_EXP8","BENE_OTH_CERT_EXP9","BENE_OTH_CERT_EXP10","BENE_OTH_CERT_EXP11","BENE_OTH_CERT_EXP12",
                        "BENE_JOB_TYPE1","BENE_JOB_TYPE2","BENE_JOB_TYPE3","BENE_JOB_TYPE4","BENE_JOB_TYPE5","BENE_JOB_TYPE6","BENE_JOB_TYPE7","BENE_JOB_TYPE8",
                        "BENE_JOB_TYPE9","BENE_JOB_TYPE10","BENE_JOB_TYPE11","BENE_JOB_TYPE12",
                        "BENE_JOB_TYPE_21","BENE_JOB_TYPE_22","BENE_JOB_TYPE_23","BENE_JOB_TYPE_24","BENE_JOB_TYPE_25","BENE_JOB_TYPE_26","BENE_JOB_TYPE_27",
                        "BENE_JOB_TYPE_28","BENE_JOB_TYPE_29","BENE_JOB_TYPE_210","BENE_JOB_TYPE_211","BENE_JOB_TYPE_212",
                        "BENE_JOB_TYPE_31","BENE_JOB_TYPE_32","BENE_JOB_TYPE_33","BENE_JOB_TYPE_34","BENE_JOB_TYPE_35","BENE_JOB_TYPE_36","BENE_JOB_TYPE_37",
                        "BENE_JOB_TYPE_38","BENE_JOB_TYPE_39","BENE_JOB_TYPE_310","BENE_JOB_TYPE_311","BENE_JOB_TYPE_312",
                        "BENE_JOB_TYPE_41","BENE_JOB_TYPE_42","BENE_JOB_TYPE_43","BENE_JOB_TYPE_44","BENE_JOB_TYPE_45","BENE_JOB_TYPE_46","BENE_JOB_TYPE_47",
                        "BENE_JOB_TYPE_48","BENE_JOB_TYPE_49","BENE_JOB_TYPE_410","BENE_JOB_TYPE_411","BENE_JOB_TYPE_412",
                        "BENE_JOB_TYPE_51","BENE_JOB_TYPE_52","BENE_JOB_TYPE_53","BENE_JOB_TYPE_54","BENE_JOB_TYPE_55","BENE_JOB_TYPE_56","BENE_JOB_TYPE_57",
                        "BENE_JOB_TYPE_58","BENE_JOB_TYPE_59","BENE_JOB_TYPE_510","BENE_JOB_TYPE_511","BENE_JOB_TYPE_512",
                        "BENE_JOB_TYPE_61","BENE_JOB_TYPE_62","BENE_JOB_TYPE_63","BENE_JOB_TYPE_64","BENE_JOB_TYPE_65","BENE_JOB_TYPE_66","BENE_JOB_TYPE_67",
                        "BENE_JOB_TYPE_68","BENE_JOB_TYPE_69","BENE_JOB_TYPE_610","BENE_JOB_TYPE_611","BENE_JOB_TYPE_612",
                        "BENE_RESERVED1","BENE_RESERVED2","BENE_RESERVED3","BENE_RESERVED4","BENE_RESERVED5","BENE_RESERVED6","BENE_RESERVED7","BENE_RESERVED8",
                        "BENE_RESERVED9","BENE_RESERVED10","BENE_RESERVED11","BENE_RESERVED12",
                        "QUALIFY_FLAG","CONTACT_NAME","EXAMINE_FLAG","ALLOW_ISSUE_STOCK_FLAG","CONTACT_TEL","UPDATE_DATE","CREATE_ID","UPDATE_ID","OWNER_CITY",
                        "OWNER_ADDR1","OWNER_ADDR2","OWNER_LNAM_FLAG","CONTACT_LNAM_FLAG",
                            "BENE_LNAM_FLAG1","BENE_LNAM_FLAG2","BENE_LNAM_FLAG3","BENE_LNAM_FLAG4","BENE_LNAM_FLAG5","BENE_LNAM_FLAG6",
                            "BENE_LNAM_FLAG7","BENE_LNAM_FLAG8","BENE_LNAM_FLAG9","BENE_LNAM_FLAG10","BENE_LNAM_FLAG11","BENE_LNAM_FLAG12"
                        });


                    }
                    if (strType == "21")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE", "MESSAGE_CHI" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    //if (htOutput["MESSAGE_TYPE"].ToString() != "0000" && htOutput["MESSAGE_TYPE"].ToString() != "0001")
                    if (htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgMsg", htOutput["MESSAGE_CHI"].ToString());
                        }
                        else
                        {
                            htOutput.Add("HtgMsg", MessageHelper.GetMessage("01_00000000_030"));
                        }
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        //htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["MESSAGE_CHI"].ToString());
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["MESSAGE_CHI"].ToString() + " ");
                        }
                        else
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("01_00000000_029") + " ");
                        }
                    }
                    break;

                case HtgType.P4_JCIJ:
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE","LINE_CNT", "MER_NO", "MER_NEME",
                                                                                    "CORP_NO", "OWNER_NAME", "BANK_NAME", "ACCT_NEME", "ACCT_NO", "CONTACT_TEL",
                                                                                    "CONTACT_FAX", "ADDRESS1", "ADDRESS2", "ADDRESS3", "DESCRIPTION", "CREATE_DATE", "CREATE_USERID"
                                                                                  });
                    }

                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0001" && htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;

                case HtgType.P4A_JCHQ:
                case HtgType.P4_JCHQ:
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "MESSAGE_CHI", "FUNCTION_CODE", "ORGN",
                                                                                    "ACCT", "ID_NAME", "ID_CITY", "CONTACT", "MERCH_MEMO", "PHONE_NMBR1", "PHONE_NMBR2", "PHONE_NMBR3",
                                                                                    "OFFICER_ID", "DB_ACCT_NMBR","MERCH_TYPE","MERCHANT_NAME","CHAIN_STORE","ADDRESS1","HOLD_STMT_FLAG","ADDRESS2",
                                                                                    "ADDRESS3","ZIP_CODE","MCC","CHAIN_MER_NBR","CHAIN_MER_LEVEL","NBR_IMPRINTER1","NBR_IMPRINTER2",
                                                                                    "NBR_IMPRINTER3","NBR_POS_DEV1","NBR_POS_DEV2","NBR_POS_DEV3","PROJ_AVG_TKT","PROJ_MTH_VOLUME",
                                                                                    "AGENT_BANK","BRANCH","ROUTE_TRANSIT","CHAIN_STMT_IND","CHAIN_REPRT_IND","CHAIN_SETT_IND","CHAIN_DISC_IND",
                                                                                    "CHAIN_FEES_IND","CHAIN_DD_IND","USER_DATA1","USER_DATA2","VISA_INTCHG_FLAG","VISA_SPECL_COND_1","VISA_SPECL_COND_2",
                                                                                    "VISA_MAIL_PHONE_IND","POS_CAP","POS_MODE","AUTH_SOURCE","CH_ID","MC_INTCHG_FLAG","CARD_STATUS1","CARD_DISC_RATE1",
                                                                                    "CARD_STATUS2","CARD_DISC_RATE2","CARD_STATUS3","CARD_DISC_RATE3","CARD_STATUS4","CARD_DISC_RATE4","CARD_STATUS5",
                                                                                    "CARD_DISC_RATE5","CARD_STATUS6","CARD_DISC_RATE6","CARD_STATUS7","CARD_DISC_RATE7","CARD_STATUS8","CARD_DISC_RATE8",
                                                                                    "STATUS_FLAG","DTE_LST_RTE_ADJ","DTE_USER_1","DTE_USER_2","USER_CODE_1","USER_CODE_2","USER_CODE_3","AVG_TKT_RANGES1",
                                                                                    "AVG_TKT_RANGES2","AVG_TKT_RANGES3","AVG_TKT_RANGES4","STMT_MSG_SUPPRESS","INTCHG_REJECT","CARD_STATUS9","CARD_DISC_RATE9",
                                                                                    "CARD_STATUS10","CARD_DISC_RATE10","CARD_STATUS11","CARD_DISC_RATE11","CARD_STATUS12","CARD_DISC_RATE12","CARD_STATUS13",
                                                                                    "CARD_DISC_RATE13","CARD_STATUS14","CARD_DISC_RATE14","CARD_STATUS15","CARD_DISC_RATE15"});
                    }

                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE", "MESSAGE_CHI" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgMsg", htOutput["MESSAGE_CHI"].ToString());
                        }
                        else
                        {
                            htOutput.Add("HtgMsg", MessageHelper.GetMessage("01_00000000_030"));
                        }
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["MESSAGE_CHI"].ToString() + " ");
                        }
                        else
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("01_00000000_029") + " ");
                        }
                    }
                    break;

                case HtgType.P4A_JCHR:
                case HtgType.P4_JCHR:
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "MESSAGE_CHI", "FUNCTION_CODE", "ORGN",
                                                                                    "ACCT", "STATUS_FLAG","ID_NAME", "ID_CITY", "CONTACT", "MERCH_MEMO", "PHONE_NMBR1", "PHONE_NMBR2",
                                                                                    "PHONE_NMBR3", "OFFICER_ID", "DB_ACCT_NMBR","MERCH_TYPE","MERCHANT_NAME","ADDRESS1","HOLD_STMT_FLAG","ADDRESS2",
                                                                                    "ADDRESS3","ZIP_CODE","MCC","CHAIN_MER_NBR","CHAIN_MER_LEVEL","NBR_IMPRINTER1","NBR_IMPRINTER2",
                                                                                    "NBR_IMPRINTER3","NBR_POS_DEV1","NBR_POS_DEV2","NBR_POS_DEV3", "DTE_LST_RTE_ADJ","DTE_USER_1",
                                                                                    "DTE_USER_2","USER_CODE_1","USER_CODE_2","USER_CODE_3","PROJ_AVG_TKT","PROJ_MTH_VOLUME",
                                                                                    "AVG_TKT_RANGES1","AVG_TKT_RANGES2","AVG_TKT_RANGES3","AVG_TKT_RANGES4","AGENT_BANK",
                                                                                    "BRANCH","ROUTE_TRANSIT","STMT_MSG_SUPPRESS","INTCHG_REJECT","CHAIN_STMT_IND","CHAIN_REPRT_IND",
                                                                                    "CHAIN_SETT_IND","CHAIN_DISC_IND","CHAIN_FEES_IND","CHAIN_DD_IND","USER_DATA1","USER_DATA2",
                                                                                    "VISA_INTCHG_FLAG","VISA_SPECL_COND_1","VISA_SPECL_COND_2","VISA_MAIL_PHONE_IND","POS_CAP",
                                                                                    "POS_MODE","AUTH_SOURCE","CH_ID","MC_INTCHG_FLAG","CARD_STATUS1","CARD_STATUS2","CARD_STATUS3",
                                                                                    "CARD_STATUS4","CARD_STATUS5","CARD_STATUS6","CARD_STATUS7","CARD_STATUS8","CARD_DISC_RATE1",
                                                                                    "CARD_DISC_RATE2","CARD_DISC_RATE3","CARD_DISC_RATE4","CARD_DISC_RATE5","CARD_DISC_RATE6",
                                                                                    "CARD_DISC_RATE7","CARD_DISC_RATE8","CARD_STATUS9","CARD_DISC_RATE9","CARD_STATUS10",
                                                                                    "CARD_DISC_RATE10","CARD_STATUS11","CARD_DISC_RATE11","CARD_STATUS12","CARD_DISC_RATE12",
                                                                                    "CARD_STATUS13","CARD_DISC_RATE13","CARD_STATUS14","CARD_DISC_RATE14","CARD_STATUS15","CARD_DISC_RATE15"
                                                               });
                    }
                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE", "MESSAGE_CHI" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgMsg", htOutput["MESSAGE_CHI"].ToString());
                        }
                        else
                        {
                            htOutput.Add("HtgMsg", MessageHelper.GetMessage("01_00000000_030"));
                        }
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["MESSAGE_CHI"].ToString() + " ");
                        }
                        else
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("01_00000000_029") + " ");
                        }
                    }
                    break;

                case HtgType.P4A_JCG1:
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID","PROGRAM_ID","USER_ID","MESSAGE_TYPE","FUNCTION_CODE","MERCH_ACCT",
                                                                                    "MERCH_NAME","BUSINESS_NAME","MESSAGE_CHI","CARD_STATUS1","CARD_DESCR1",
                                                                                    "CARD_DESCR_SHORT1","DISC_RATE1","CARD_UPDATE_DATE1","CARD_STATUS2","CARD_DESCR2",
                                                                                    "CARD_DESCR_SHORT2","DISC_RATE2","CARD_UPDATE_DATE2","CARD_STATUS3","CARD_DESCR3",
                                                                                    "CARD_DESCR_SHORT3","DISC_RATE3","CARD_UPDATE_DATE3","CARD_STATUS4","CARD_DESCR4",
                                                                                    "CARD_DESCR_SHORT4","DISC_RATE4","CARD_UPDATE_DATE4","CARD_STATUS5","CARD_DESCR5",
                                                                                    "CARD_DESCR_SHORT5","DISC_RATE5","CARD_UPDATE_DATE5","CARD_STATUS6","CARD_DESCR6",
                                                                                    "CARD_DESCR_SHORT6","DISC_RATE6","CARD_UPDATE_DATE6","CARD_STATUS7","CARD_DESCR7",
                                                                                    "CARD_DESCR_SHORT7","DISC_RATE7","CARD_UPDATE_DATE7","CARD_STATUS8","CARD_DESCR8",
                                                                                    "CARD_DESCR_SHORT8","DISC_RATE8","CARD_UPDATE_DATE8","CARD_STATUS9","CARD_DESCR9",
                                                                                    "CARD_DESCR_SHORT9","DISC_RATE9","CARD_UPDATE_DATE9","CARD_STATUS10","CARD_DESCR10",
                                                                                    "CARD_DESCR_SHORT10","DISC_RATE10","CARD_UPDATE_DATE10","CREATE_USER","CREATE_DATE",
                                                                                    "CREATE_TIME","MAINTAIN_USER","MAINTAIN_DATE","MAINTAIN_TIME"
                                                               });
                    }
                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE", "MESSAGE_CHI" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (htOutput["MESSAGE_TYPE"].ToString() != "0001")
                    {
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgMsg", htOutput["MESSAGE_CHI"].ToString());
                        }
                        else
                        {
                            htOutput.Add("HtgMsg", MessageHelper.GetMessage("01_00000000_030"));
                        }
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["MESSAGE_CHI"].ToString() + " ");
                        }
                        else
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("01_00000000_029") + " ");
                        }
                    }
                    break;
                case HtgType.P4L_LGOR:
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID","PROGRAM_ID","USER_ID","FUNCTION_CODE","MSG_SEQ","MSG_ERR","IN_ORG","IN_MERCHANT",
                                                                "IN_PROD_CODE","IN_CARD_TYPE","PROGID","MERRATE","LIMITR","CHPOINT","CHAMT","USER_EXIT","CYLCO",
                                                                "STARTU","ENDU","LIMITU","CHPOINTU","CHAMTU","BIRTH","STARTB","ENDB", "LIMITB","CHPOINTB","CHAMTB"
                                                               });
                    }
                    if ("100" == strType)
                    {
                        arrRet = new ArrayList(new object[] { "MSG_SEQ", "MSG_ERR" });
                    }
                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }
                    if (!(htOutput["MSG_SEQ"].ToString() == "0000" && htOutput["MSG_ERR"].ToString() == "00"))
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MSG_SEQ"].ToString() + htOutput["MSG_ERR"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MSG_SEQ"].ToString() + htOutput["MSG_ERR"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MSG_SEQ"].ToString() + htOutput["MSG_ERR"].ToString() + " " + GetMessageType(type, htOutput["MSG_SEQ"].ToString() + htOutput["MSG_ERR"].ToString()));
                    }

                    break;
                case HtgType.P4L_LGAT:
                    if ("100" == strType)
                    {
                        arrRet = new ArrayList(new object[] { "SEQ_NO", "MSG_TYPE" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }

                    if (!(htOutput["SEQ_NO"].ToString() == "0000" && htOutput["MSG_TYPE"].ToString() == "00"))
                    {
                        strHtgMessage = GetMessageType(type, htOutput["SEQ_NO"].ToString() + htOutput["MSG_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["SEQ_NO"].ToString() + htOutput["MSG_TYPE"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["SEQ_NO"].ToString() + htOutput["MSG_TYPE"].ToString() + " " + GetMessageType(type, htOutput["SEQ_NO"].ToString() + htOutput["MSG_TYPE"].ToString()));
                    }
                    break;
                case HtgType.P8_000401:
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "CUST_ID_NO" });
                    }
                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }
                    break;
                case HtgType.P4A_JCPA:
                    htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString());
                    if (htOutput["MESSAGE_TYPE"].ToString().Trim() == "N999" || htOutput["MESSAGE_TYPE"].ToString().Trim() == "N889" ||
                       htOutput["MESSAGE_TYPE"].ToString().Trim() == "N998" || htOutput["MESSAGE_TYPE"].ToString().Trim() == "N997" ||
                       htOutput["MESSAGE_TYPE"].ToString().Trim() == "N996" || htOutput["MESSAGE_TYPE"].ToString().Trim() == "N995" ||
                       htOutput["MESSAGE_TYPE"].ToString().Trim() == "N885" || htOutput["MESSAGE_TYPE"].ToString().Trim() == "N884" ||
                       htOutput["MESSAGE_TYPE"].ToString().Trim() == "N994" || htOutput["MESSAGE_TYPE"].ToString().Trim() == "N882" ||
                       htOutput["MESSAGE_TYPE"].ToString().Trim() == "N881" || htOutput["MESSAGE_TYPE"].ToString().Trim() == "N993" ||
                       htOutput["MESSAGE_TYPE"].ToString().Trim() == "N992" || htOutput["MESSAGE_TYPE"].ToString().Trim() == "N991" ||
                       htOutput["MESSAGE_TYPE"].ToString().Trim() == "N990" || htOutput["MESSAGE_TYPE"].ToString().Trim() == "N899" ||
                       htOutput["MESSAGE_TYPE"].ToString().Trim() == "N898" || htOutput["MESSAGE_TYPE"].ToString().Trim() == "N897" ||
                       htOutput["MESSAGE_TYPE"].ToString().Trim() == "N887" || htOutput["MESSAGE_TYPE"].ToString().Trim() == "N886" ||
                       htOutput["MESSAGE_TYPE"].ToString().Trim() == "N883" || htOutput["MESSAGE_TYPE"].ToString().Trim() == "N896" ||
                       htOutput["MESSAGE_TYPE"].ToString().Trim() == "N895" || htOutput["MESSAGE_TYPE"].ToString().Trim() == "N894" ||
                       htOutput["MESSAGE_TYPE"].ToString().Trim() == "N893")
                    {
                        htOutput.Add("HtgMsg", htOutput["MESSAGE_TYPE"].ToString().Trim());
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                    }
                    break;

                case HtgType.P4_JCFA:
                case HtgType.P4D_JCFA:
                    //餘額轉置作業
                    if (strType == "1" || strType == "12" || strType == "100")
                    {
                        //arrRet = new ArrayList(new object[] { "GUFA-TRAN-ID-OK", "GUFA-PROGRAM-ID-OK", "GUFA-USER-ID", "GUFA-MESSAGE-TYPE", "GUFA-FUNCTION-QUERY-ID",
                        //                                      "GUFA-FUNCTION-QUERY-ACCT", "GUFA-LINE-CNT", "GUFA-SVCP-ID", "GUFA-CARD-NMBR", "GUFA-ISSUE-CNT", "GUFA-PURSE-ID",
                        //                                      "GUFA-KEY-NMBR", "GUFA-ID-IDN", "GUFA-EXPIR-YY1", "GUFA-EXPIR-YY2", "GUFA-EXPIR-MM", "GUFA-CARD-STATUS",
                        //                                      "GUFA-CARD-STATUS-CODE", "GUFA-PURSE-STATUS", "GUFA-BLOCK-CODE", "GUFA-BLOCK-DATE", "GUFA-DETAIL-ISSUE-CNT"});
                        arrRet = new ArrayList(new object[]{"FUNCTION_CODE","SVCP_ID","USER_ID","MESSAGE_TYPE",
                            "PURSE_ID","TRAN_ID","CARD_NMBR","PROGRAM_ID","LINE_CNT","ISSUE_CNT"});

                        for (int i = 1; i <= 20; i++)
                        {
                            arrRet.Add(string.Format("CARD_STATUS{0}", i));
                            arrRet.Add(string.Format("CARD_STATUS_CODE{0}", i));
                            arrRet.Add(string.Format("PURSE_STATUS{0}", i));
                            arrRet.Add(string.Format("BLOCK_CODE{0}", i));
                            arrRet.Add(string.Format("BLOCK_DATE{0}", i));
                            arrRet.Add(string.Format("ID_IND{0}", i));
                            arrRet.Add(string.Format("KEY_NMBER{0}", i));
                            arrRet.Add(string.Format("EXPIRE_DATE{0}", i));
                            arrRet.Add(string.Format("ISSUE_CNT{0}", i));
                        }
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        if (strType == "100")//*判斷批次作業錯誤類型
                        {
                            htOutput["HtgMsgFlag"] = "2";
                        }
                        return htOutput;
                    }

                    if (!(htOutput["MESSAGE_TYPE"].ToString() == "0000" || htOutput["MESSAGE_TYPE"].ToString() == "0001"))
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;
                case HtgType.P4_JCLD:
                    // JCLD
                    arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE", "TYPE", "LPR_NO_A", "LPR_NO_B"
                                                            , "CUSTID", "ACCT_ID", "VENDOR_CODE", "APPLY_NO", "STATUS", "SPC", "SPC_ID", "INTRODUCER_CARD_ID"
                                                            , "FILLER_00", "ACCLINKID", "D_STATUS", "RESULT_A_CODE", "RESULT_D_CODE", "SERVICE_A_DATE"
                                                            , "SERVICE_D_DATE", "UPDATE_USER", "UPDATE_DATE", "UPDATE_TIME", "EFF_DATE", "SEND_DATE", "APPLY_NO_R"
                                                            , "D_SPC", "D_SPC_ID", "D_INTRODUCER_CARD_ID", "SOURCE", "KEYIN_DATE", "VENDOR_NAME", "CHANNEL", "FILLER"});

                    // 檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        if (strType == "100")// 判斷批次作業錯誤類型
                        {
                            htOutput["HtgMsgFlag"] = "2";
                        }
                        return htOutput;
                    }

                    if (!(htOutput["MESSAGE_TYPE"].ToString() == "0000" || htOutput["MESSAGE_TYPE"].ToString() == "0001" || htOutput["MESSAGE_TYPE"].ToString() == "0002" || htOutput["MESSAGE_TYPE"].ToString() == "0003"))
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");// 顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;
                case HtgType.P4_JCLB:
                    // JCLB
                    arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE", "LINE_CNT", "ACCTID"
                                                            , "LPR_NO", "KEYIN_DATE", "CASE_FLAG", "FILLER", "TYPE", "ACCT_ID", "LPR_NO", "CUSTID"
                                                            , "STATUS", "NO", "SERVICE_A_DATE", "SERVICE_D_DATE", "RESULT_A_CODE", "RESULT_D_CODE"
                                                            , "APPLY_NO", "KEYIN_DATE", "UPDATE_USER", "UPDATE_DATE", "UPDATE_TIME", "SEND_DATE"
                                                            , "SPC", "SPC_ID", "INTRODUCER_CARD_ID", "EFF_DATE", "SOURCE", "CHANNEL", "VENDOR_NAME"});

                    #region 主機回傳資料一次五筆，欄位後面帶有數字1-5造成欄位檢核錯誤，因此跳過主機欄位驗證

                    //// 檢核主機欄位
                    //if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    //{
                    //    if (strType == "100")// 判斷批次作業錯誤類型
                    //    {
                    //        htOutput["HtgMsgFlag"] = "2";
                    //    }
                    //    return htOutput;
                    //}

                    #endregion

                    if (!(htOutput["MESSAGE_TYPE"].ToString() == "0000" || htOutput["MESSAGE_TYPE"].ToString() == "0001"))
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");// 顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;
                case HtgType.P4_JCLE:
                    // JCLE
                    arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE", "LINE_CNT", "ACCTID"
                                                            , "LPR_NO", "UPDATE_DATE_S", "UPDATE_DATE_E", "KEYIN_DATE_S", "KEYIN_DATE_E", "CASE_FLAG"
                                                            , "APPLY_NO", "FILLER_00", "ACTION", "TYPE", "ACCT_ID", "LPR_NO", "CUSTID"
                                                            , "STATUS", "NO", "SERVICE_A_DATE", "SERVICE_D_DATE", "RESULT_A_CODE", "RESULT_D_CODE"
                                                            , "APPLY_NO", "KEYIN_DATE", "UPDATE_USER", "UPDATE_DATE", "UPDATE_TIME", "SEND_DATE"
                                                            , "SPC", "SPC_ID", "INTRODUCER_CARD_ID", "EFF_DATE", "SOURCE", "CHANNEL", "VENDOR_NAME"});

                    #region 主機回傳資料一次五筆，欄位後面帶有數字1-5造成欄位檢核錯誤，因此跳過主機欄位驗證

                    //// 檢核主機欄位
                    //if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    //{
                    //    if (strType == "100")// 判斷批次作業錯誤類型
                    //    {
                    //        htOutput["HtgMsgFlag"] = "2";
                    //    }
                    //    return htOutput;
                    //}

                    #endregion

                    if (!(htOutput["MESSAGE_TYPE"].ToString() == "0000" || htOutput["MESSAGE_TYPE"].ToString() == "0001"))
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");// 顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;

                case HtgType.P4_JC99:
                    //* JC99
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE",
                                                              "NAME","ROMA","PARENT_NAME","PARENT_ROMA","IN_CFLAG","IN_CHANNEL",
                                                              "IN_PFLAG","FILLER"});
                    }
                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE", "MESSAGE_CHI" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        if (strType == "100")//*判斷批次作業錯誤類型
                        {
                            htOutput["HtgMsgFlag"] = "2";
                        }
                        return htOutput;
                    }

                    if (!(htOutput["MESSAGE_TYPE"].ToString() == "0000" || htOutput["MESSAGE_TYPE"].ToString() == "0001"))
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;
                case HtgType.P4A_JC68:
                    if (strType == "1")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE",
                                                              "MESSAGE_CHI","LONG_NAME","PINYIN_NAME","ID"});
                    }
                    if (strType == "2")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE", "MESSAGE_CHI" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        if (strType == "100")//*判斷批次作業錯誤類型
                        {
                            htOutput["HtgMsgFlag"] = "2";
                        }
                        return htOutput;
                    }

                    if (!(htOutput["MESSAGE_TYPE"].ToString() == "0000" || htOutput["MESSAGE_TYPE"].ToString() == "0001"))
                    {
                        strHtgMessage = GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(type, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;
                case HtgType.P4A_JC69: //AML收單分公司維護作業--20190918 by Peggy
                    if (strType == "11")
                    {
                        arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID","MESSAGE_TYPE","FUNCTION_CODE","CORP_NO",
                        "CORP_SEQ","MESSAGE_CHI","HEAD_CORP_NO","HEAD_CORP_SEQ","CC","BUILD_DATE","BUSINESS_ORGAN_TYPE","REGISTER_NATION","REGISTER_US_STATE","REG_NAME","CORP_REG_ENG_NAME","REG_CITY",
                        "REG_ADDR1","REG_ADDR2","TEL","OWNER_CHINESE_NAME","OWNER_ENGLISH_NAME","OWNER_ID","OWNER_BIRTH_DATE","OWNER_ID_ISSUE_DATE",
                        "OWNER_ID_ISSUE_PLACE","OWNER_ID_REPLACE_TYPE","OWNER_NATION","ID_PHOTO_FLAG","PASSPORT","PASSPORT_EXP_DATE","RESIDENT_NO",
                        "RESIDENT_EXP_DATE","OWNER_TEL","OWNER_CITY","OWNER_ADDR1","OWNER_ADDR2","LONG_NAME_FLAG","DDA_BANK_NAME","DDA_BANK_BRANCH",
                        "DDA_BANK_ACCT","DDA_ACCT_NAME","UPDATE_DATE","CREATE_ID","UPDATE_ID","ARCHIVE_NO","CHECK_CODE"
                        });
                    }
                    if (strType == "21")
                    {
                        arrRet = new ArrayList(new object[] { "MESSAGE_TYPE", "MESSAGE_CHI" });
                    }

                    //*檢核主機欄位
                    if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
                    {
                        return htOutput;
                    }
                    
                    if (htOutput["MESSAGE_TYPE"].ToString() != "0000")
                    {
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgMsg", htOutput["MESSAGE_CHI"].ToString());
                        }
                        else
                        {
                            htOutput.Add("HtgMsg", MessageHelper.GetMessage("01_00000000_030"));
                        }
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["HtgMsg"].ToString();
                    }
                    else
                    {
                        if (htOutput["MESSAGE_CHI"].ToString().Trim() != "")
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + htOutput["MESSAGE_CHI"].ToString() + " ");
                        }
                        else
                        {
                            htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("01_00000000_029") + " ");
                        }
                    }
                    break;
            }

            if (htOutput.Contains("HtgMsg"))
            {
                /*
                if (type == HtgType.P4_PCTI || type == HtgType.P4D_PCTI)
                {
                    htOutput["HtgMsg"] = htInput["FUNCTION_ID"].ToString().Substring(0, 4) + ":" + strLog;
                }
                else
                {
                    htOutput["HtgMsg"] = type.ToString() + ":" + strLog;
                }
                */
                htOutput["HtgMsg"] = type.ToString() + ":" + strLog;

                Logging.Log(strErrorMessage + ":" + strMsg, LogState.Error, LogLayer.HTG);
                return htOutput;
            }
            else
            {
                /*
                if (type == HtgType.P4_PCTI || type == HtgType.P4D_PCTI)
                {
                    htOutput["HtgSuccess"] = htInput["FUNCTION_ID"].ToString().Substring(0, 4) + ":" + htOutput["HtgSuccess"].ToString();
                }
                else
                {
                    htOutput["HtgSuccess"] = type.ToString() + ":" + htOutput["HtgSuccess"].ToString();
                }
                 */
                if (htOutput["HtgSuccess"] != null)
                    htOutput["HtgSuccess"] = type.ToString() + ":" + htOutput["HtgSuccess"].ToString();
            }
            #endregion
        }
        catch (Exception ex)
        {
            strMsg = hc.ExceptionHandler(ex, "主機電文錯誤:");
            htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");

            if (strType == "100" || strType == "200")//*判斷批次作業錯誤類型
            {
                if (!htOutput.Contains("HtgMsgFlag"))
                {
                    htOutput.Add("HtgMsgFlag", "2");//*顯示主機訊息標識
                }
                else
                {
                    htOutput["HtgMsgFlag"] = "2";//*顯示主機訊息標識
                }
            }
            else
            {
                if (!htOutput.Contains("HtgMsgFlag"))
                {
                    htOutput.Add("HtgMsgFlag", "1");//*顯示主機訊息標識
                }
                else
                {
                    htOutput["HtgMsgFlag"] = "1";//*顯示主機訊息標識
                }
            }
            Logging.Log(strErrorMessage + ":" + strMsg, LogState.Error, LogLayer.HTG);
            return htOutput;
        }
        finally
        {
            //*根據需求可在下行電文后關閉或者不關閉連接
            if (blnIsClose)
            {
                if (!hc.CloseSession(ref strMsg))
                {
                    if (htOutput.Contains("HtgMsg"))
                    {
                        htOutput["HtgMsg"] = htOutput["HtgMsg"].ToString() + "  " + strMsg;
                    }
                    else
                    {
                        htOutput.Add("HtgMsg", strMsg);
                    }
                }
                else
                {
                    if (htOutput.Contains("sessionId"))
                    {
                        htOutput["sessionId"] = "";
                    }
                    else
                    {
                        htOutput.Add("sessionId", "");
                    }
                }
            }
        }
        return htOutput;
    }

    /// <summary>
    /// 2011-12-22 ADD 銀行(Bancs)
    /// 上傳并取得主機資料信息(無分頁)
    /// </summary>
    /// <param name="type">電文枚舉類型</param>
    /// <param name="htInput">傳入參數的HashTable</param>
    /// <param name="blnIsClose">是否關閉主機Session</param>
    /// <returns>傳出參數的HashTable</returns>
    public static Hashtable GetMainframeDataLU0(Hashtable htInput, HtgType Type, ref string strMsg, bool blnIsClose, string[] stringArray, String jobid = "")
    {
        string strIsOnLine = GetStr(Type, "ISONLINE");           //* 該電文是否上線
        string strAuthOnLine = GetStr(Type, "AUTHONLINE");       //* HTG登入登出是否上線
        EntityAGENT_INFO eAgentInfo = new EntityAGENT_INFO();
        eAgentInfo = (EntityAGENT_INFO)System.Web.HttpContext.Current.Session["Agent"]; //*Session變數集合
        #region 添加上傳主機信息
        //*添加上傳主機信息
        htInput.Add("userId", eAgentInfo.agent_id);
        htInput.Add("passWord", eAgentInfo.agent_pwd);
        htInput.Add("racfId", eAgentInfo.agent_id_racf);
        htInput.Add("racfPassWord", eAgentInfo.agent_id_racf_pwd);
        htInput.Add("signOnLU0", "yes");

        Hashtable htOutput = new Hashtable();
        HTGCommunicator hc = new HTGCommunicator(jobid);
        string strFileName = Configure.HTGTempletPath + "req" + Type + ".xml";
        #endregion

        #region 取得電文的SessionId
        string SessionId = "";
        //*取得電文的SessionId
        if (System.Web.HttpContext.Current.Session["sessionIdLU0"] != null && System.Web.HttpContext.Current.Session["sessionIdLU0"].ToString() != "")
            SessionId = System.Web.HttpContext.Current.Session["sessionIdLU0"].ToString();

        //*如果Session為空,第一次需要連接主機得到電文SessionId
        //再回填Session回去
        if (SessionId == "")
        {
            if (!hc.LogonAuth(htInput, ref strMsg, strAuthOnLine))
            {
                Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
                return htOutput;
            }
            else
            {
                //Session不關閉,則回存
                if (!blnIsClose)
                {
                    System.Web.HttpContext.Current.Session["sessionIdLU0"] = hc.SessionId;
                }
            }
        }
        else
        {
            hc.SessionId = SessionId;
        }

        if (htInput.Contains("sessionId"))
        {
            htInput.Remove("sessionId");
        }
        htInput.Add("sessionId", SessionId);
        #endregion

        #region 建立reqHost物件

        HTGhostgateway reqHost = new HTGhostgateway();
        try
        {
            hc.RequestHostCreator(strFileName, ref reqHost, htInput);
        }
        catch
        {
            strMsg = "req" + Type + ".xml格式不正確或文件不存在";
            Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
            return htOutput;
        }

        #endregion

        HTGhostgateway rtnHost = new HTGhostgateway();
        try
        {
            #region 取得rtnHost物件

            strMsg = hc.QueryHTG(UtilHelper.GetAppSettings("HtgHttp").ToString(), reqHost, ref rtnHost, htInput, strIsOnLine);
            if (strMsg != "")
            {
                Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
                return htOutput;
            }
            if (htOutput.Contains("sessionId"))
            {
                htOutput["sessionId"] = hc.SessionId;
            }
            else
            {
                htOutput.Add("sessionId", hc.SessionId);
            }

            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session["sessionIdLU0"] = hc.SessionId;
            }
            #endregion

            #region 判別rtnHost是否正確
            if (rtnHost.body != null)
            {
                strMsg = "主機連線失敗:" + rtnHost.body.msg.Value;
                Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
                return htOutput;
            }
            #endregion

            #region 處理主機錯誤訊息
            //*主機錯誤公共判斷
            if (!hc.HTGMsgParser(rtnHost, ref strMsg))
            {
                Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
                return htOutput;
            }
            #endregion

            #region 通過MessageType得到錯誤訊息
            //string strMESSAGE_TYPE = GetMessageType(Type, "MESSAGE_TYPE");

            //string strMessageType = "";
            //for (int i = 0; i < rtnHost.line.Count; i++)
            //{
            //    rtnHost.line[i].msgHeader.data.QueryDataByID(strMESSAGE_TYPE, ref strMessageType);
            //    if (strMessageType != "")
            //    {
            //        break;
            //    }
            //}
            ////2011-12-22 ADD
            //string strType = HtgType.P8_000401.ToString();

            //switch (strType)
            //{
            //     //訊息待確認
            //    //*主機000401作業
            //    case "00040X":
            //        switch (strMessageType)
            //        {
            //            case "067000":
            //                break;
            //            case "067050":
            //                rtnHost.line[0].msgBody.data.QueryDataByID("ERRORMESSAGETEXT_OC01", ref strMsg);
            //                return htOutput;
            //        }
            //        break;
            //}
            #endregion

            #region 將資料塞入HashTable
            for (int i = 0; i < rtnHost.line.Count; i++)
            {
                for (int j = 0; j < rtnHost.line[i].msgBody.data.Count; j++)
                {
                    if (rtnHost.line[i].msgBody.data[j].ID == "outputCode" && rtnHost.line[i].msgBody.data[j].Value == "03")
                    {
                        for (int k = 0; k < rtnHost.line[i].msgBody.data.Count; k++)
                        {
                            htOutput.Add(rtnHost.line[i].msgBody.data[k].ID, rtnHost.line[i].msgBody.data[k].Value);
                        }
                    }
                }
            }
            #endregion

            #region 判斷所需傳回欄位是否都已傳回
            for (int i = 0; i < stringArray.Length; i++)
            {
                if (!htOutput.Contains(stringArray[i]))
                {
                    strMsg = "獲取主機資料失敗-";
                    Logging.Log(strMsg + stringArray[i], LogState.Error, LogLayer.HTG);
                    return htOutput;
                }
            }
            #endregion
        }
        catch (Exception ex)
        {
            strMsg = hc.ExceptionHandler(ex, "主機電文錯誤:");
            Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
            return htOutput;
        }
        finally
        {
            //*根據需求可在下行電文后關閉或者不關閉連接
            if (blnIsClose)
            {
                string strMessage = "";
                if (!hc.CloseSession(ref strMessage, strAuthOnLine))
                {
                    strMsg = strMsg + "  " + strMessage;
                }
                System.Web.HttpContext.Current.Session["sessionIdLU0"] = "";
            }
        }
        return htOutput;
    }

    /// <summary>
    /// 檢核主機回傳欄位
    /// </summary>
    /// <param name="htOutput">主機回傳Hashtbale</param>
    /// <param name="arrRet">欄位集合</param>
    /// <param name="strErrorMessage">錯誤提示信息</param>
    /// <returns>true成功，false失敗</returns>
    public static bool CheckHtgColumn(ref Hashtable htOutput, ArrayList arrRet, string strErrorMessage)
    {
        foreach (string strTemp in arrRet)
        {
            if (!htOutput.ContainsKey(strTemp))
            {
                string strMsg = "主機電文錯誤";
                htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");
                if (!htOutput.Contains("HtgMsgFlag"))
                {
                    htOutput.Add("HtgMsgFlag", "1");//*顯示主機訊息標識
                }
                else
                {
                    htOutput["HtgMsgFlag"] = "1";//*顯示主機訊息標識
                }
                Logging.Log(strErrorMessage + ":" + strMsg, LogState.Error, LogLayer.HTG);
                return false;
            }
        }
        return true;
    }

    //*作者 趙呂梁
    //*創建日期：2009/12/22
    //*修改日期：2009/12/22
    /// <summary>
    /// 獲取信息說明中文內容
    /// </summary>
    /// <param name="type">電文枚舉類型</param>
    /// <param name="strMessageCode">代碼</param>
    /// <returns>中文說明</returns>
    public static string GetMessageType(HtgType type, string strMessageCode)
    {
        string strMessage = "";
        if (strMessageCode == "")
        {
            strMessage = "主機回覆碼為空，請確認主機回傳是否正確 ";
            return strMessage;
        }

        switch (type)
        {
            case HtgType.P4D_PCTI:
            case HtgType.P4_PCTI:
                switch (strMessageCode)
                {
                    case "00999":
                        strMessage = "主機作業成功! ";
                        break;
                    case "01007":
                        strMessage = "自扣帳號有誤! ";
                        break;
                    case "01022":
                        strMessage = "公司電話有誤! ";
                        break;
                    case "01027":
                        strMessage = "註記欄位長度過長! ";
                        break;
                    case "01042":
                        strMessage = "畢業年月有誤! ";
                        break;
                    case "01300":
                        strMessage = "格式不正確! ";
                        break;
                    case "01301":
                        strMessage = "ORG/TYPE 不存在! ";
                        break;
                    case "01303":
                        strMessage = "卡片類別錯誤! ";
                        break;
                    case "01304":
                        strMessage = "卡號不存在! ";
                        break;
                    case "01305":
                        strMessage = "資料已存在! ";
                        break;
                    case "01307":
                        strMessage = "已換卡! ";
                        break;
                    case "01310":
                        strMessage = "其他錯誤! ";
                        break;
                    case "01311":
                        strMessage = "無調整額度的權限。必須將此操作者的權限設定後，才可以調整額度 ";
                        break;
                    default:
                        strMessage = "系統異常 ! ";
                        break;
                }
                break;

            case HtgType.P4D_JCF6:
                switch (strMessageCode)
                {
                    case "0001":
                        strMessage = "卡人資料查詢成功．";
                        break;
                    case "0000":
                        strMessage = "卡人資料查詢成功．";
                        break;
                    case "8888":
                        strMessage = "查無卡人資料．";
                        break;
                    case "9999":
                        strMessage = "NOT OPEN．";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4_JCF6:
                switch (strMessageCode)
                {
                    case "0001":
                        strMessage = "卡人資料查詢成功．";
                        break;
                    case "0000":
                        strMessage = "卡人資料查詢成功．";
                        break;
                    case "8888":
                        strMessage = "查無卡人資料．";
                        break;
                    case "9999":
                        strMessage = "NOT OPEN．";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;
            case HtgType.P4_JCAX:
                switch (strMessageCode)
                {
                    case "0000":
                    case "0001":
                        strMessage = "OK. ";
                        break;
                    case "7777":
                        strMessage = "ERROR. ";
                        break;
                    case "8888":
                        strMessage = "NOT FOUND. ";
                        break;
                    case "9999":
                        strMessage = "NOT OPEN. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4_JCF7:
                switch (strMessageCode)
                {
                    case "0000":
                    case "0001":
                        strMessage = "OK. ";
                        break;
                    case "8888":
                        strMessage = "NOT FOUND. ";
                        break;
                    case "9999":
                        strMessage = "NOT OPEN. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4_JCHO:
                switch (strMessageCode)
                {
                    case "0000":
                    case "0001":
                        strMessage = "成功. ";
                        break;
                    case "8888":
                        strMessage = "查無資料";
                        break;
                    case "9999":
                        strMessage = "系統異常! ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4_JCGQ:
            case HtgType.P4A_JCGQ:
                switch (strMessageCode)
                {
                    case "0000":
                    case "0001":
                        strMessage = "成功. ";
                        break;
                    case "9999":
                        strMessage = "系統異常 ! ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4_JCAA:
                switch (strMessageCode)
                {
                    case "0001":
                    case "0000":
                        strMessage = "成功. ";
                        break;
                    case "6001":
                        strMessage = "名下已無卡片. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;
            case HtgType.P4_JCAT:
                switch (strMessageCode)
                {
                    case "0001":
                    case "0000":
                        strMessage = "成功. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4_JCEM:
                switch (strMessageCode)
                {
                    case "0000":
                    case "0001":
                        strMessage = "成功. ";
                        break;
                    case "6016":
                        strMessage = "卡號有誤. "; // 2014/12/22 by 周智仁
                        break;
                    case "6054":
                        strMessage = "聯名團體解約不可掛毀補請執行１８８０";
                        break;
                    //20211005 (U) by Nash, 配合RQ-2021-016475-002無記名悠遊卡轉換，掛毀補交易設定增加回覆訊息JCAW(4023,4024,4025),JCEM(6055,6056,6057)
                    case "6055":
                        strMessage = "無記名悠遊卡不可掛毀補請執行１８８０";
                        break;
                    case "6056":
                        strMessage = "記名式悠遊聯名卡 ID 檔未開啟";
                        break;
                    case "6057":
                        strMessage = "記名式悠遊聯名卡 ID 檔異常";
                        break;
                    case "9999":
                        strMessage = "系統異常! ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4_JCIL:
                switch (strMessageCode)
                {
                    case "0000":
                    case "0001":
                        strMessage = "成功. ";
                        break;
                    case "7001":
                        strMessage = "特店檔JCVKIPMR未開. ";
                        break;
                    case "8001":
                        strMessage = "查無此特店檔JCVKIPMR資料. ";
                        break;
                    case "9999":
                        strMessage = "系統異常! ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4A_JCGR:
                switch (strMessageCode)
                {
                    case "0001":
                    case "0000":
                        strMessage = "成功. ";
                        break;
                    case "8888":
                        strMessage = "異常. ";
                        break;
                    case "9999":
                        strMessage = "檔案未開啟. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;
            case HtgType.P4A_JC66:
                switch (strMessageCode)
                {
                    case "0001":
                    case "0000":
                        strMessage = "成功. ";
                        break;
                    case "8888":
                        strMessage = "異常. ";
                        break;
                    case "9999":
                        strMessage = "檔案未開啟. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;
            case HtgType.P4A_JC68:
                switch (strMessageCode)
                {
                    case "0001":
                    case "0000":
                        strMessage = "成功. ";
                        break;
                    case "8888":
                        strMessage = "異常. ";
                        break;
                    case "9999":
                        strMessage = "檔案未開啟. ";
                        break;
                    case "0033":
                        strMessage = "中文長姓名不得空白. ";
                        break;
                    case "0034":
                        strMessage = "羅馬拼音輸入有誤. ";
                        break;
                    case "0031":
                        strMessage = "檢核１９００無維護權限！. ";
                        break;
                    case "0007":
                        strMessage = "長姓名檔案未開啟，請確認. ";
                        break;
                    case "0008":
                        strMessage = "長姓名檔案異常，請確認. ";
                        break;
                    case "0006":
                        strMessage = "無此筆查詢資料，請重新輸入. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;
            case HtgType.P4_JCIJ:
                switch (strMessageCode)
                {
                    case "0000":
                    case "0001":
                        strMessage = "成功. ";
                        break;
                    case "7001":
                        strMessage = "特店檔JCVKIPMR未開. ";
                        break;
                    case "8001":
                        strMessage = "查無此特店檔JCVKIPMR資料. ";
                        break;
                    case "9999":
                        strMessage = "系統異常 ! ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4A_JCGX:
            case HtgType.P4_JCGX:
                switch (strMessageCode)
                {
                    case "0001":
                    case "0000":
                        strMessage = "成功. ";
                        break;
                    case "8888":
                        strMessage = "異常. ";
                        break;
                    case "9999":
                        strMessage = "檔案未開啟. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4A_JCG1:
                switch (strMessageCode)
                {
                    case "0000":
                    case "0001":
                        strMessage = "成功. ";
                        break;
                    case "8888":
                        strMessage = "NOT FOUND. ";
                        break;
                    case "9999":
                        strMessage = "ERROR. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4_JCAW:
                switch (strMessageCode)
                {
                    #region
                    case "0000":
                    case "0001":
                        strMessage = "正常. ";
                        break;
                    case "1001":
                        strMessage = "取卡方式不正確. ";
                        break;
                    case "1002":
                        strMessage = "製卡姓名不正確. ";
                        break;
                    case "1003":
                        strMessage = "製卡式樣不正確. ";
                        break;
                    case "1004":
                        strMessage = "有效期限不正確. ";
                        break;
                    case "1005":
                        strMessage = "卡號不正確. ";
                        break;
                    case "1006":
                        strMessage = "卡號型別無法判別. ";
                        break;
                    case "1007":
                        strMessage = "卡號型別有誤(TYPE=160). ";
                        break;
                    case "1008":
                        strMessage = "世足卡（含手錶）須為WAVE卡. ";
                        break;
                    case "1009":
                        strMessage = "世足手錶不可掛補. ";
                        break;
                    case "1010":
                        strMessage = "世足手錶超過一年，已不能毀補. ";
                        break;
                    case "1011":
                        strMessage = "世足卡（含手錶）不能展期. ";
                        break;
                    case "2001":
                        strMessage = "卡片檔內資料不存在. ";
                        break;
                    case "2002":
                        strMessage = "掛失資料未移轉. ";
                        break;
                    case "2003":
                        strMessage = "製卡檔資料不存在. ";
                        break;
                    case "2004":
                        strMessage = "卡人檔內資料不存在. ";
                        break;
                    case "2005":
                        strMessage = "百事達會員編號檔內資料不存在. ";
                        break;
                    case "2006":
                        strMessage = "系統檔資料不存在. ";
                        break;
                    case "2007":
                        strMessage = "補發卡號檔內資料不存在. ";
                        break;
                    case "2008":
                        strMessage = "認同代碼不存在. ";
                        break;
                    case "3001":
                        strMessage = "寫入製卡檔資料重覆. ";
                        break;
                    case "3002":
                        strMessage = "掛補卡號維護資料重覆寫入. ";
                        break;
                    case "3003":
                        strMessage = "製卡姓名維護資料重覆寫入. ";
                        break;
                    case "3004":
                        strMessage = "製卡有效期維護資料重覆寫入. ";
                        break;
                    case "3005":
                        strMessage = "MAINTLOG檔資料重覆寫入. ";
                        break;
                    case "4001":
                        strMessage = "卡片狀態不為1OR2. ";
                        break;
                    case "4002":
                        strMessage = "已完成掛補STATUS=1. ";
                        break;
                    case "4003":
                        strMessage = "已完成掛補STATUS=2. ";
                        break;
                    case "4004":
                        //strMessage = "卡片檔ＡＣＴＩＯＮ不等於０. ";
                        strMessage = "當日重覆件. ";   // 2014/12/22 by 周智仁
                        break;
                    case "4005":
                        strMessage = "卡片BLOCK需為LORX. ";
                        break;
                    case "4006":
                        strMessage = "卡片有催收狀態. ";
                        break;
                    case "4007":
                        strMessage = "卡片非相片卡，製卡樣式不得為01. ";
                        break;
                    case "4008":
                        strMessage = "出帳單日不可做掛失. ";
                        break;
                    case "4009":
                        strMessage = "卡片BLOCKCODE不符. ";
                        break;
                    case "4010":
                        strMessage = "此卡已作過補發. ";
                        break;
                    case "4011":
                        strMessage = "此卡未改過密碼不能補發密碼函. ";
                        break;
                    case "4012":
                        strMessage = "百視達會員編號須為空白. ";
                        break;
                    case "4013":
                        strMessage = "百視達會員編號有誤. ";
                        break;
                    case "4017":
                        strMessage = "聯名團體解約不可掛毀補請執行１２９６. ";
                        break;
                    case "4022":
                        strMessage = "聯名團體解約不可掛毀補請執行１８８０";
                        break;
                    //20211005 (U) by Nash, 配合RQ-2021-016475-002無記名悠遊卡轉換，掛毀補交易設定增加回覆訊息JCAW(4023,4024,4025),JCEM(6055,6056,6057)
                    case "4023":
                        strMessage = "無記名悠遊卡不可掛毀補請執行１８８０";
                        break;
                    case "4024":
                        strMessage = "記名式悠遊聯名卡 ID 檔未開啟";
                        break;
                    case "4025":
                        strMessage = "記名式悠遊聯名卡 ID 檔異常";
                        break;
                    //edit by mel 20141204  
                    case "6034":
                        strMessage = "MAINT LOG 檔資料重覆寫入. ";
                        break;
                    case "8001":
                        strMessage = "卡片檔未開啟. ";
                        break;
                    case "8002":
                        strMessage = "製卡檔未開啟. ";
                        break;
                    case "8003":
                        strMessage = "卡人檔未開啟資料不存在. ";
                        break;
                    case "8004":
                        strMessage = "百事達會員檔未開啟. ";
                        break;
                    case "8005":
                        strMessage = "系統檔未開啟資料. ";
                        break;
                    case "8006":
                        strMessage = "ＭＡＩＮＴ檔未開啟. ";
                        break;
                    case "8007":
                        strMessage = "補發卡號檔未開啟. ";
                        break;
                    case "8008":
                        strMessage = "偽卡檔未開啟. ";
                        break;
                    case "8009":
                        strMessage = "開卡檔未開啟. ";
                        break;
                    case "8010":
                        strMessage = "CPCMA未開啟. ";
                        break;
                    case "8011":
                        strMessage = "認同代碼檔未開啟. ";
                        break;
                    case "8012":
                        strMessage = "MTC2 檔未開啟. ";
                        break;
                    case "8013":
                        strMessage = "MTC1 檔未開啟. ";
                        break;
                    //edit by mel 20141204
                    case "8014":
                        strMessage = "IPKF 檔未開啟. ";
                        break;
                    case "8015":
                        strMessage = "IPKF 檔資料不存在. ";
                        break;
                    case "8016":
                        strMessage = "原卡片需為磁條才能執行磁轉ＷＡＶＥ作業. ";
                        break;
                    case "8017":
                        strMessage = "EXMS 1181  卡片種類不為 W/T,  無法磁轉ＷＡＶＥ. ";
                        break;
                    case "8018":
                        strMessage = "磁轉ＷＡＶＥ FLAG 需為 Y 或 N. ";
                        break;
                    case "8019":
                        strMessage = "PIDS 檔未開啟. ";
                        break;
                    case "8020":
                        strMessage = "毀補轉悠遊或一卡通註記  需為 T 或 K. ";
                        break;
                    case "8021":
                        strMessage = "EXMS 1181  卡片種類不為 K/B,  無法轉一卡通. ";
                        break;
                    case "8022":
                        strMessage = "悠遊卡和一卡通不能互轉. ";
                        break;
                    case "8023":
                        strMessage = "悠遊公司行銷註記僅允許 Y 或 N . ";
                        break;
                    case "8024":
                        strMessage = "EXMS 1181  卡片種類不為 T/B,  無法轉悠遊. ";
                        break;
                    case "8025":
                        strMessage = "TLOG 檔未開啟. ";
                        break;
                    case "8026":
                        strMessage = "寫入悠遊 TLOG 檔資料重覆 . ";
                        break;
                    case "8027":
                        strMessage = "悠遊 TLOG 資料不存在. ";
                        break;
                    case "8028":
                        strMessage = "原卡號製卡樣式需為磁條、 WAVE 、 QPAY 才能毀補轉換悠遊或一卡通. ";
                        break;
                    case "8029":
                        strMessage = "悠遊公司自動加值功能開啟方式僅允許 Y 或 N. ";
                        break;
                    case "9001":
                        strMessage = "製卡檔LENGERR. ";
                        break;
                    case "9002":
                        strMessage = "卡人檔LENGERR. ";
                        break;
                    case "9003":
                        strMessage = "寫入百視達檔ERROR. ";
                        break;
                    //edit by mel 20141204 
                    case "9998":
                        strMessage = "卡片狀態不符  (CM-STATUS 為 6 ). ";
                        break;
                    case "9999":
                        strMessage = "系統異常. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4_JCDK:
                switch (strMessageCode)
                {
                    case "0001":
                    case "0000":
                        strMessage = "OK. ";
                        break;
                    case "8888":
                        strMessage = "NOT FOUND. ";
                        break;
                    case "8001":
                        strMessage = "電話檢核錯誤. ";
                        break;
                    case "9999":
                        strMessage = " NOT OPEN. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4A_JCHQ:
            case HtgType.P4_JCHQ:
                switch (strMessageCode)
                {
                    case "0000":
                        strMessage = "成功. ";
                        break;
                    case "7001":
                        strMessage = "特店檔JCVKIPMR未開. ";
                        break;
                    case "8001":
                        strMessage = "查無此特店檔JCVKIPMR資料. ";
                        break;
                    case "9999":
                        strMessage = "系統異常 ! ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;

            case HtgType.P4A_JCHR:
            case HtgType.P4_JCHR:
                switch (strMessageCode)
                {
                    case "0000":
                        strMessage = "成功. ";
                        break;
                    case "7001":
                        strMessage = "特店檔JCVKIPMR未開. ";
                        break;
                    case "8001":
                        strMessage = "查無此特店檔JCVKIPMR資料. ";
                        break;
                    case "9999":
                        strMessage = "系統異常 ! ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;
            case HtgType.P4L_LGOR:
                switch (strMessageCode)
                {
                    case "000000":
                        strMessage = "執行結果正確，成功返回";
                        break;
                    case "900101":
                        strMessage = "若NOTOPEN或DISABLED";
                        break;
                    case "900102":
                        strMessage = "NOTFND";
                        break;
                    case "900103":
                        strMessage = "讀檔異常";
                        break;
                    case "900104":
                        strMessage = "讀檔異常";
                        break;
                    case "900105":
                        strMessage = "讀檔異常";
                        break;
                    case "900106":
                        strMessage = "讀檔異常";
                        break;
                    case "900107":
                        strMessage = "讀檔異常";
                        break;
                    case "900108":
                        strMessage = "讀檔異常";
                        break;
                    case "900109":
                        strMessage = "讀檔異常";
                        break;
                    case "900112":
                        strMessage = "P4L商店資料未入，請隔天再送";
                        break;
                    case "900201":
                        strMessage = "NOTFND";
                        break;
                    case "900202":
                        strMessage = "若NOTOPEN或DISABLED或其他讀檔錯誤";
                        break;
                    case "900203":
                        strMessage = "功能碼為A(新增時)，檔案資料已存在";
                        break;
                    case "900204":
                        strMessage = "寫檔或覆寫檔案錯誤";
                        break;
                    case "900301":
                        strMessage = "NOTFND";
                        break;
                    case "900302":
                        strMessage = "NOTOPEN或DISABLED或其他讀檔錯誤";
                        break;
                    //20161202 (U) by Tank, add LGOR主機回覆代碼:900401
                    case "900401":
                        strMessage = "maintain log寫檔異常";
                        break;
                    case "999001":
                        strMessage = "程式錯誤";
                        break;
                    case "999901":
                        strMessage = "系統異常";
                        break;
                    case "000101":
                        strMessage = "ORGANISATION需為數值且非0、非999";
                        break;
                    case "000201":
                        strMessage = "MERCHANT需為數值且非0、非999999999";
                        break;
                    case "000301":
                        strMessage = "CARDTYPE需為數值且非0";
                        break;
                    case "000401":
                        strMessage = "PRODCODE需為數值且非0";
                        break;
                    case "000501":
                        strMessage = "功能碼需為A、C、I";
                        break;
                    case "000601":
                        strMessage = "PROGRAMID需為數值";
                        break;
                    case "000701":
                        strMessage = "MERCHANT%需為數值且大於0且小於10000";
                        break;
                    case "000801":
                        strMessage = "REDEMPTIONLIMIT%需為數值且大於0且小於10000";
                        break;
                    case "000901":
                        strMessage = "C/HPOINTS需為數值";
                        break;
                    case "001001":
                        strMessage = "C/HAMOUNT需為數值";
                        break;
                    case "001101":
                        strMessage = "BIRTHMONTHFUNCTION需為00或01或02或03";
                        break;
                    case "001201":
                        strMessage = "BIRTHMONTHFUNCTION起始日期需為數值";
                        break;
                    case "001202":
                        strMessage = "BIRTHMONTHFUNCTION起始日期需符合日期格式";
                        break;
                    case "001301":
                        strMessage = "BIRTHMONTHFUNCTION結束日期需為數值";
                        break;
                    case "001302":
                        strMessage = "BIRTHMONTHFUNCTION結束日期需符合日期格式";
                        break;
                    case "001303":
                        strMessage = "BIRTHMONTHFUNCTION起始日期不可大於結束日期";
                        break;
                    case "001401":
                        strMessage = "BIRTHMONTHFUNCTIONLIMIT%需為數值且大於0且小於10000";
                        break;
                    case "001501":
                        strMessage = "BIRTHMONTHFUNCTIONC/HPOINTS需為數值";
                        break;
                    case "001601":
                        strMessage = "BIRTHMONTHFUNCTIONC/HAMOUNT需為數值";
                        break;
                    case "001701":
                        strMessage = "USREXITFUNCTION需為00或01或02或03";
                        break;
                    case "001801":
                        strMessage = "USREXITCYCLECODE(第一碼需為'M'OR'W'OR'D')或為'000000'";
                        break;
                    case "001901":
                        strMessage = "USREXITFUNCTION起始日期需為數值";
                        break;
                    case "001902":
                        strMessage = "USREXITFUNCTION起始日期需符合日期格式";
                        break;
                    case "002001":
                        strMessage = "USREXITFUNCTION結束日期需為數值";
                        break;
                    case "002002":
                        strMessage = "USREXITFUNCTION結束日期需符合日期格式";
                        break;
                    case "002003":
                        strMessage = "USREXITFUNCTION起始日期不可大於結束日期";
                        break;
                    case "002101":
                        strMessage = "USREXITFUNCTIONLIMIT%需為數值且大於0且小於10000";
                        break;
                    case "002201":
                        strMessage = "USREXITFUNCTIONC/HPOINTS需為數值";
                        break;
                    case "002301":
                        strMessage = "USREXITFUNCTIONC/HAMOUNT需為數值";
                        break;
                    case "002701":
                        strMessage = "程式代碼需為CLGU110";
                        break;
                    case "002801":
                        strMessage = "USERID不可為空白或LOW-VALUES";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;
            case HtgType.P4L_LGAT:
                switch (strMessageCode)
                {
                    case "000000":
                        strMessage = "執行結果正確，成功返回";
                        break;
                    case "900101":
                        strMessage = "讀LBSYS檔，NOTOPEN或DISABLED";
                        break;
                    case "900102":
                        strMessage = "NOTFND,THE ORG/TYPE NOT ON LBSYS FILE";
                        break;
                    case "900103":
                        strMessage = "OTHER(NOT NORMAL)，LBSYS讀檔錯誤";
                        break;
                    case "900201":
                        strMessage = "讀LBPGPNR檔，NOTOPEN或DISABLED";
                        break;
                    case "900202":
                        strMessage = "NOTFND,THIS ORG/PROG/PARTNER NOT ON FILE";
                        break;
                    case "900203":
                        strMessage = "OTHER(NOT NORMAL)，LBPGPNR讀檔錯誤";
                        break;
                    case "900301":
                        strMessage = "讀LBGRCT檔，NOTOPEN或DISABLED";
                        break;
                    case "900302":
                        strMessage = "NOTFND，查詢-查無資料";
                        break;
                    case "900303":
                        strMessage = "NORMAL，新增-資料已存在";
                        break;
                    case "900304":
                        strMessage = "OTHER(NOT NORMAL)，LBGRCT讀檔錯誤";
                        break;
                    case "900401":
                        strMessage = "讀LBGRCT檔，NOTOPEN或DISABLED";
                        break;
                    case "900402":
                        strMessage = "寫LBGRCT檔，DUPREC";
                        break;
                    case "900403":
                        strMessage = "OTHER(NOT NORMAL)，LBGRCT寫檔錯誤";
                        break;
                    case "900501":
                        strMessage = "寫LBMNT檔，NOTOPEN或DISABLED";
                        break;
                    case "900502":
                        strMessage = "寫LBMNT檔，DUPREC";
                        break;
                    case "900503":
                        strMessage = "OTHER(NOT NORMAL)，LBMNT寫檔錯誤";
                        break;
                    case "999001":
                        strMessage = "程式錯誤";
                        break;
                    case "999901":
                        strMessage = "系統異常";
                        break;
                    case "000101":
                        strMessage = "ORG非數值";
                        break;
                    case "000102":
                        strMessage = "ORG等於0";
                        break;
                    case "000103":
                        strMessage = "ORG等於999";
                        break;
                    case "000201":
                        strMessage = "PROG非數值";
                        break;
                    case "000202":
                        strMessage = "PROG等於0";
                        break;
                    case "000203":
                        strMessage = "PROG等於999";
                        break;
                    case "000301":
                        strMessage = "PARTNER非數值";
                        break;
                    case "000302":
                        strMessage = "PATTNER等於999999999";
                        break;
                    case "000303":
                        strMessage = "THIS ORG/PROG/PARTNER PENDING ADD";
                        break;
                    case "000401":
                        strMessage = "TYPE非數值";
                        break;
                    case "000402":
                        strMessage = "TYPE等於0";
                        break;
                    case "000403":
                        strMessage = "TYPE等於999";
                        break;
                    case "000601":
                        strMessage = "新增參數STATUS預設為0，不接受輸入";
                        break;
                    case "000701":
                        strMessage = "ERR. TC-1 MUST BE NUMERIC";
                        break;
                    case "000702":
                        strMessage = "TC-CODE ERR. THIS TC IS RESERVED,ENTER AGAIN";
                        break;
                    case "000703":
                        strMessage = "ERR. TC-1 NOT IN LBSYS";
                        break;
                    case "000801":
                        strMessage = "ERR. OPERATOR-1 MUST BE + OR -";
                        break;
                    case "000901":
                        strMessage = "ERR. TC-1 MUST BE NUMERIC";
                        break;
                    case "000902":
                        strMessage = "ERR. TC-1 ERROR";
                        break;
                    case "000903":
                        strMessage = "不在LBSYS定義的TC-CODE中";
                        break;
                    case "001001":
                        strMessage = "ERR. OPERATOR-1 MUST BE + OR -";
                        break;
                    case "001101":
                        strMessage = "ERR. TC-1 MUST BE NUMERIC";
                        break;
                    case "001201":
                        strMessage = "ERR. OPERATOR-1 MUST BE + OR -";
                        break;
                    case "001301":
                        strMessage = "ERR. TC-1 MUST BE NUMERIC";
                        break;
                    case "001401":
                        strMessage = "ERR. OPERATOR-1 MUST BE + OR -";
                        break;
                    case "001501":
                        strMessage = "ERR. TC-1 MUST BE NUMERIC";
                        break;
                    case "001601":
                        strMessage = "ERR. OPERATOR-1 MUST BE + OR -";
                        break;
                    case "001701":
                        strMessage = "ERR. TC-1 MUST BE NUMERIC";
                        break;
                    case "001801":
                        strMessage = "ERR. OPERATOR-1 MUST BE + OR -";
                        break;
                    case "001901":
                        strMessage = "ERR. TC-1 MUST BE NUMERIC";
                        break;
                    case "002001":
                        strMessage = "ERR. OPERATOR-1 MUST BE + OR -";
                        break;
                    case "002101":
                        strMessage = "ERR. TC-1 MUST BE NUMERIC";
                        break;
                    case "002201":
                        strMessage = "ERR. OPERATOR-1 MUST BE + OR -";
                        break;
                    case "002301":
                        strMessage = "ERR. TC-1 MUST BE NUMERIC";
                        break;
                    case "002401":
                        strMessage = "ERR. OPERATOR-1 MUST BE + OR -";
                        break;
                    case "002501":
                        strMessage = "ERR. TC-1 MUST BE NUMERIC";
                        break;
                    case "002601":
                        strMessage = "ERR. OPERATOR-1 MUST BE + OR -";
                        break;
                    case "002701":
                        strMessage = "ERR. MCC-FROM-1 MUST BE NUMERIC";
                        break;
                    case "002801":
                        strMessage = "ERR. MCC-TO-1 MUST BE NUMERIC";
                        break;
                    case "002802":
                        strMessage = "ERR. MCC-FROM-1 > MCC-TO-1";
                        break;
                    case "002901":
                        strMessage = "ERR. MCC-FROM-1 MUST BE NUMERIC";
                        break;
                    case "003001":
                        strMessage = "ERR. MCC-TO-1 MUST BE NUMERIC";
                        break;
                    case "003101":
                        strMessage = "ERR. MCC-FROM-1 MUST BE NUMERIC";
                        break;
                    case "003201":
                        strMessage = "ERR. MCC-TO-1 MUST BE NUMERIC";
                        break;
                    case "003301":
                        strMessage = "ERR. MCC-FROM-1 MUST BE NUMERIC";
                        break;
                    case "003401":
                        strMessage = "ERR. MCC-TO-1 MUST BE NUMERIC";
                        break;
                    case "003501":
                        strMessage = "ERR. MCC-FROM-1 MUST BE NUMERIC";
                        break;
                    case "003601":
                        strMessage = "ERR. MCC-TO-1 MUST BE NUMERIC";
                        break;
                    case "003701":
                        strMessage = "ERR. MCC-FROM-1 MUST BE NUMERIC";
                        break;
                    case "003801":
                        strMessage = "ERR. MCC-TO-1 MUST BE NUMERIC";
                        break;
                    case "003901":
                        strMessage = "ERR. MCC-FROM-1 MUST BE NUMERIC";
                        break;
                    case "004001":
                        strMessage = "ERR. MCC-TO-1 MUST BE NUMERIC";
                        break;
                    case "004101":
                        strMessage = "ERR. MCC-FROM-1 MUST BE NUMERIC";
                        break;
                    case "004201":
                        strMessage = "ERR. MCC-TO-1 MUST BE NUMERIC";
                        break;
                    case "004301":
                        strMessage = "ERR. MCC-FROM-1 MUST BE NUMERIC";
                        break;
                    case "004401":
                        strMessage = "ERR. MCC-TO-1 MUST BE NUMERIC";
                        break;
                    case "004501":
                        strMessage = "ERR. MCC-FROM-1 MUST BE NUMERIC";
                        break;
                    case "004601":
                        strMessage = "ERR. MCC-TO-1 MUST BE NUMERIC";
                        break;
                    case "004701":
                        strMessage = "COUNTRY CODE IND MUST BE I OR X OR N";
                        break;
                    case "004801":
                        strMessage = "ERR. MUST SPECIFY AT LEAST ONE COUNTRY CODE";
                        break;
                    case "004901":
                        strMessage = "無判斷條件";
                        break;
                    case "005001":
                        strMessage = "無判斷條件";
                        break;
                    case "005101":
                        strMessage = "無判斷條件";
                        break;
                    case "005201":
                        strMessage = "無判斷條件";
                        break;
                    case "005301":
                        strMessage = "無判斷條件";
                        break;
                    case "005401":
                        strMessage = "無判斷條件";
                        break;
                    case "005501":
                        strMessage = "無判斷條件";
                        break;
                    case "005601":
                        strMessage = "無判斷條件";
                        break;
                    case "005701":
                        strMessage = "無判斷條件";
                        break;
                    case "005801":
                        strMessage = "ERR. CYC DUE MUST BE NUMERIC";
                        break;
                    case "005901":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "006001":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "006101":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "006201":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "006301":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "006401":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "006501":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "006601":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "006701":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "006801":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "006901":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "007001":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "007101":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "007201":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "007301":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "007401":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "007501":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "007601":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "007701":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "007801":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "007901":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "008001":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "008101":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "008201":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "008301":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "008401":
                        strMessage = "ERR.  BLOCK CODE A MUST BE 0 OR 1";
                        break;
                    case "008501":
                        strMessage = "ERR. CARDHOLDER POSTION MUST BE NUMERIC";
                        break;
                    case "008502":
                        strMessage = "ERR. CARDHOLDER POSTION MUST BETWEEN 1 TO 16";
                        break;
                    case "008601":
                        strMessage = "ERR. MUST BE NUMERIC";
                        break;
                    case "008701":
                        strMessage = "ERR. CARD IDENTIFICATION IND MUST BE I/X/N";
                        break;
                    case "008702":
                        strMessage = "ERR. CARD POSTION HAS VALUE.PLS ENTER VALUE,IND!";
                        break;
                    case "008801":
                        strMessage = "ERR. INVALID PRIN  METHOD, 0/1/2!";
                        break;
                    case "008901":
                        strMessage = "ERR. INVALID PRIN CALC MUST BE 0,1";
                        break;
                    case "009001":
                        strMessage = "ERR. PRIN AMT  1 MUST BE NUMERIC!";
                        break;
                    case "009101":
                        strMessage = "ERR. PRIN  RATE 1 NOT NUMERIC";
                        break;
                    case "009102":
                        strMessage = "ERR. INV TIER 1 PRIN AMOUNT AND RATE";
                        break;
                    case "009201":
                        strMessage = "ERR. PRIN AMT  2 MUST BE NUMERIC!";
                        break;
                    case "009202":
                        strMessage = "ERR. PRINCIPLE TIER NOT IN ASCENDING ORDER";
                        break;
                    case "009301":
                        strMessage = "ERR. PRIN  RATE 2 NOT NUMERIC";
                        break;
                    case "009302":
                        strMessage = "ERR. INV TIER 2 PRIN AMOUNT AND RATE";
                        break;
                    case "009401":
                        strMessage = "ERR. PRIN AMT  3 MUST BE NUMERIC!";
                        break;
                    case "009402":
                        strMessage = "ERR. PRINCIPLE TIER NOT IN ASCENDING ORDER";
                        break;
                    case "009501":
                        strMessage = "ERR. PRIN  RATE 3 NOT NUMERIC";
                        break;
                    case "009502":
                        strMessage = "ERR. INV TIER 3 PRIN AMOUNT AND RATE";
                        break;
                    case "009601":
                        strMessage = "ERR. PRIN AMT  4 MUST BE NUMERIC!";
                        break;
                    case "009602":
                        strMessage = "ERR. PRINCIPLE TIER NOT IN ASCENDING ORDER";
                        break;
                    case "009701":
                        strMessage = "ERR. PRIN  RATE 4 NOT NUMERIC";
                        break;
                    case "009702":
                        strMessage = "ERR. INV TIER 4 PRIN AMOUNT AND RATE";
                        break;
                    case "009801":
                        strMessage = "ERR. INVALID SUPP METHOD !";
                        break;
                    case "009901":
                        strMessage = "ERR. SUPP CALC IND MUST BE 0,1";
                        break;
                    case "010001":
                        strMessage = "ERR. SUPP  AMT-1 MUST BE NUMERIC!";
                        break;
                    case "010101":
                        strMessage = "ERR. SUPPL  RATE 1 NOT NUMERIC    !";
                        break;
                    case "010102":
                        strMessage = "ERR. INV SUPP TIER 1 AMOUNT AND RATE ";
                        break;
                    case "010201":
                        strMessage = "ERR. SUPP  AMT-2 MUST BE NUMERIC!";
                        break;
                    case "010202":
                        strMessage = "ERR. SUPPLEMENTARY TIER NOT IN ASCENDING ORDER";
                        break;
                    case "010301":
                        strMessage = "ERR. SUPPL  RATE 2 NOT NUMERIC    !";
                        break;
                    case "010302":
                        strMessage = "ERR. INV SUPP TIER 2 AMOUNT AND RATE ";
                        break;
                    case "010401":
                        strMessage = "ERR. SUPP  AMT-3 MUST BE NUMERIC!";
                        break;
                    case "010402":
                        strMessage = "ERR. SUPPLEMENTARY TIER NOT IN ASCENDING ORDER";
                        break;
                    case "010501":
                        strMessage = "ERR. SUPPL  RATE 3 NOT NUMERIC    !";
                        break;
                    case "010502":
                        strMessage = "ERR. INV SUPP TIER 3 AMOUNT AND RATE";
                        break;
                    case "010601":
                        strMessage = "ERR. SUPP  AMT-4 MUST BE NUMERIC!";
                        break;
                    case "010602":
                        strMessage = "ERR. SUPPLEMENTARY TIER NOT IN ASCENDING ORDER";
                        break;
                    case "010701":
                        strMessage = "ERR. SUPPL  RATE 4 NOT NUMERIC    !";
                        break;
                    case "010702":
                        strMessage = "ERR. INV SUPP TIER 4 AMOUNT AND RATE";
                        break;
                    case "010801":
                        strMessage = "ERR. PROMOTION START DTE MUST BE NUMERIC";
                        break;
                    case "010802":
                        strMessage = "ERR. INVALID PROMOTION DATE !";
                        break;
                    case "010901":
                        strMessage = "ERR. PROMOTION END DTE MUST BE NUMERIC";
                        break;
                    case "010902":
                        strMessage = "ERR. INVALID PROMOTION DATE !";
                        break;
                    case "010903":
                        strMessage = "ERR. PROMO START DATE > END DATE!";
                        break;
                    case "011001":
                        strMessage = "ERR. INVALID PROMO METHOD, 0/1/2";
                        break;
                    case "011101":
                        strMessage = "ERR. SUPP CALC IND MUST BE 0,1";
                        break;
                    case "011102":
                        strMessage = "ERR. SUPP CALC IND MUST BE 0,1";
                        break;
                    case "011201":
                        strMessage = "ERR. PROMO AMT-1 MUST BE NUMERIC!";
                        break;
                    case "011301":
                        strMessage = "ERR. PROMO RATE-1 NOT NUMERIC !";
                        break;
                    case "011302":
                        strMessage = "ERR. INV TIER-1 PROMOTE AMOUNT AND RATE !";
                        break;
                    case "011401":
                        strMessage = "ERR. PROMO AMT-2 MUST BE NUMERIC!";
                        break;
                    case "011402":
                        strMessage = "ERR. PROMOTION TIER NOT IN ASCENDING ORDER";
                        break;
                    case "011501":
                        strMessage = "ERR. PROMO RATE-2 NOT NUMERIC";
                        break;
                    case "011502":
                        strMessage = "ERR. INV TIER-2 PROMOTE AMOUNT AND RATE";
                        break;
                    case "011601":
                        strMessage = "ERR. PROMO AMT-3 MUST BE NUMERIC!";
                        break;
                    case "011602":
                        strMessage = "ERR. PROMOTION TIER NOT IN ASCENDING ORDER";
                        break;
                    case "011701":
                        strMessage = "ERR. PROMO RATE-3 NOT NUMERIC";
                        break;
                    case "011702":
                        strMessage = "ERR. INV TIER-3 PROMOTE AMOUNT AND RATE";
                        break;
                    case "011801":
                        strMessage = "ERR. PROMO AMT-4 MUST BE NUMERIC!";
                        break;
                    case "011802":
                        strMessage = "ERR. PROMOTION TIER NOT IN ASCENDING ORDER";
                        break;
                    case "011901":
                        strMessage = "ERR. PROMO RATE-4 NOT NUMERIC";
                        break;
                    case "011902":
                        strMessage = "ERR. INV TIER-4 PROMOTE AMOUNT AND RATE";
                        break;
                    case "012001":
                        strMessage = "ERR. SUPPRESS START DATE MUST BE NUMERIC";
                        break;
                    case "012002":
                        strMessage = "ERR. INVALID SUPPRESS START DATE";
                        break;
                    case "012101":
                        strMessage = "ERR. SUPPRESS END DATE MUST BE NUMERIC";
                        break;
                    case "012102":
                        strMessage = "ERR. INVALID  SUPPRESTION END DATE";
                        break;
                    case "012103":
                        strMessage = "ERR. SUPP START DATE IS GREATER THAN END DATE";
                        break;
                    case "012201":
                        strMessage = "ERR. INVALID BIRTH DATE FLAG MUST BE Y OR SPACE";
                        break;
                    case "012301":
                        strMessage = "ERR. BTHDTE START DTE MUST BE NUMBERIC";
                        break;
                    case "012302":
                        strMessage = "ERR. BTHDTE START DTE MUST BE NUMBERIC";
                        break;
                    case "012303":
                        strMessage = "ERR. SYSTEM DATE > BTHDTE START DATE!";
                        break;
                    case "012401":
                        strMessage = "ERR. BTHDTE END DTE MUST BE NUMBERIC";
                        break;
                    case "012402":
                        strMessage = "ERR. BTHDTE END DTE MUST BE NUMBERIC";
                        break;
                    case "012403":
                        strMessage = "ERR. SYSTEM DATE > BTHDTE END DATE !";
                        break;
                    case "012404":
                        strMessage = "ERR. BTHDTE START DATE > END DATE!";
                        break;
                    case "012501":
                        strMessage = "ERR. INVALID BIRTH DATE MTHD MUST BE 0,1,2";
                        break;
                    case "012601":
                        strMessage = "ERR. INVALID BIRTH DATE CALC MUST BE 0,1";
                        break;
                    case "012701":
                        strMessage = "ERR. PRIN  AMT 1 MUST BE NUMERIC!";
                        break;
                    case "012801":
                        strMessage = "ERR. PRIN  RATE 1 NOT NUMERIC";
                        break;
                    case "012802":
                        strMessage = "ERR. PRIN BASIC RATE 1>100%!";
                        break;
                    case "012901":
                        strMessage = "ERR. PRIN BASIC RATE 1>100%!";
                        break;
                    case "013001":
                        strMessage = "ERR. PRIN BASIC RATE 1>100%!";
                        break;
                    case "013101":
                        strMessage = "ERR. PRIN BASIC RATE 1>100%!";
                        break;
                    case "013201":
                        strMessage = "ERR. PRIN BASIC RATE 1>100%!";
                        break;
                    case "013301":
                        strMessage = "ERR. PRIN BASIC RATE 1>100%!";
                        break;
                    case "013401":
                        strMessage = "ERR. PRIN BASIC RATE 1>100%!";
                        break;
                    case "013501":
                        strMessage = "ERR. INVALID JOIN DATE FLAG MUST BE Y OR SPACE";
                        break;
                    case "013601":
                        strMessage = "ERR. JOIDTE MONTH DTE MUST BE NUMBERIC";
                        break;
                    case "013701":
                        strMessage = "ERR. INVALID JOIN DATE MTHD MUST BE 0,1,2";
                        break;
                    case "013801":
                        strMessage = "ERR. INVALID JOIN DATE CALC MUST BE 0,1";
                        break;
                    case "013901":
                        strMessage = "ERR. PRIN  AMT 1 MUST BE NUMERIC!";
                        break;
                    case "014001":
                        strMessage = "ERR. PRIN  RATE 1 NOT NUMERIC";
                        break;
                    case "014002":
                        strMessage = "ERR. PRIN BASIC RATE 1>100%!";
                        break;
                    case "014101":
                        strMessage = "ERR. PRIN BASIC RATE 1>100%!";
                        break;
                    case "014201":
                        strMessage = "ERR. PRIN BASIC RATE 1>100%!";
                        break;
                    case "014301":
                        strMessage = "ERR. PRIN BASIC RATE 1>100%!";
                        break;
                    case "014401":
                        strMessage = "ERR. PRIN BASIC RATE 1>100%!";
                        break;
                    case "014501":
                        strMessage = "ERR. PRIN BASIC RATE 1>100%!";
                        break;
                    case "014601":
                        strMessage = "ERR. PRIN BASIC RATE 1>100%!";
                        break;

                    default:
                        //strMessage = MessageHelper.GetMessage("01_00000000_030");
                        strMessage = "主機回覆不明的錯誤.";
                        break;
                        #endregion
                }
                break;

            case HtgType.P4D_JCFA:
            case HtgType.P4_JCFA:
                switch (strMessageCode)
                {
                    case "0001":
                        strMessage = "卡人資料查詢成功．";
                        break;
                    case "0000":
                        strMessage = "卡人資料查詢成功．";
                        break;
                    case "8888":
                        strMessage = "查無卡人資料．";
                        break;
                    case "9999":
                        strMessage = "NOT OPEN．";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;
            case HtgType.P4_JCLD:
                switch (strMessageCode)
                {
                    case "9001":
                        strMessage = "電文交易名稱不為JCLD";
                        break;
                    case "9002":
                        strMessage = "電文程式名稱不為JCGU851";
                        break;
                    case "9003":
                        strMessage = "電文使用者ID為空白,或非指定通路";
                        break;
                    case "9004":
                        //20211213-依需求單號：RQ-2021-034274-000  ，調整顯示訊息 BY Peggy
                        //strMessage = "功能碼不為1、2、3、4";
                        strMessage = "功能鍵選擇錯誤，請選擇正確的功能鍵！";
                        break;
                    case "9005":
                        strMessage = "業務識別碼不為01、02、03";
                        break;
                    case "9006":
                        //20211213-依需求單號：RQ-2021-034274-000  ，調整顯示訊息 BY Peggy
                        //strMessage = "車號、車主ID、正卡人ID為空白";
                        strMessage = "請輸入車號資料！車主、正卡人資料須輸入，請確認！";
                        break;
                    case "9007":
                        //20211213-依需求單號：RQ-2021-034274-000  ，調整顯示訊息 BY Peggy
                        //strMessage = "停車場代碼為空白";
                        strMessage = "車主、正卡人、停車場資料須輸入，請確認！";
                        break;
                    case "9101":
                        strMessage = "已報送過媒體或出帳不可刪除";
                        break;
                    case "9102":
                        strMessage = "此身分證字號不存在，請重新輸入";
                        break;
                    case "9103":
                        strMessage = "此筆代扣資料重覆，請重新輸入";
                        break;
                    case "9104":
                        strMessage = "此人名下正卡目前均為非流通";
                        break;
                    case "9105":
                        strMessage = "此人是附卡人，無正卡";
                        break;
                    case "9106":
                        strMessage = "車號資料輸入錯誤，請重新輸入";
                        break;
                    case "9107":
                        strMessage = "請輸入要修改的資料";
                        break;
                    case "9108":
                        strMessage = "修改資料欄位皆未異動";
                        break;
                    case "9109":
                        strMessage = "做終止不能異動ID";
                        break;
                    case "9110":
                        strMessage = "尚未收到異動回覆資料，不可處理";
                        break;
                    case "9111":
                        strMessage = "9+ID 不可申請代繳，請重新輸入";
                        break;
                    case "9112":
                        strMessage = "狀態碼需為1、2";
                        break;
                    case "9113":
                        strMessage = "外國人不可申請代繳，請重新輸入";
                        break;
                    case "9114":
                        strMessage = "推廣通路代號只可輸入數字";
                        break;
                    case "9115":
                        strMessage = "推廣員編只可輸入數字";
                        break;
                    case "9116":
                        strMessage = "收件編號輸入錯誤，請重新輸入";
                        break;
                    case "9117":
                        strMessage = "狀態碼輸入錯誤，請重新輸入";
                        break;
                    case "9118":
                        //20211213-依需求單號：RQ-2021-034274-000  ，調整顯示訊息 BY Peggy
                        //strMessage = "查無有效停車代扣資料，請先確認儲值服務申請";
                        strMessage = "查無有效停車儲值代扣資料，請到１６４１確認！";
                        break;
                    case "9119":
                        //20211213-依需求單號：RQ-2021-034274-000  ，調整顯示訊息 BY Peggy
                        //strMessage = "查無此代扣記錄";
                        strMessage = "查無此資料！查無此停車場！ ";
                        break;
                    case "9120":
                        strMessage = "此停車場代碼已終止，請確認";
                        break;
                    case "9121":
                        //20211213-依需求單號：RQ-2021-034274-000  ，調整顯示訊息 BY Peggy
                        //strMessage = "此車號已在申請中不能重複鍵檔！";
                        strMessage = "此車號已在申請中不能重覆鍵檔！";
                        break;
                    case "9122":
                        //20211213-依需求單號：RQ-2021-034274-000  ，調整顯示訊息 BY Peggy
                        //strMessage = "此為批次上傳申請書，不可異動<收件編號>";
                        strMessage = "此件為申請書進件不可異動收件編號 ! ";
                        break;
                    case "9123":
                        //20211213-依需求單號：RQ-2021-034274-000  ，調整顯示訊息 BY Peggy
                        //strMessage = "此為批次上傳申請書，不可刪除";
                        strMessage = "此件為申請書進件不得刪除 ! ";
                        break;
                    case "0001":
                        strMessage = "新增成功";
                        break;
                    case "0002":
                        strMessage = "查詢成功";
                        break;
                    case "0003":
                        strMessage = "修改成功";
                        break;
                    case "0004":
                        strMessage = "刪除成功";
                        break;
                    case "7777":
                        //20211213-依需求單號：RQ-2021-034274-000  ，調整顯示訊息 BY Peggy
                        //strMessage = "檔案關閉或異常";
                        strMessage = "檔案未開啟，請通知資訊處！";
                        break;
                    case "8888":
                        //20211213-依需求單號：RQ-2021-034274-000  ，調整顯示訊息 BY Peggy
                        //strMessage = "查無符合資料";
                        strMessage = "無符合條件資料，請重新輸入！";
                        break;
                    case "9999":
                        strMessage = "其他錯誤";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;
            case HtgType.P4_JCLB:
                switch (strMessageCode)
                {
                    case "9001":
                        strMessage = "電文交易名稱不為JCLB";
                        break;
                    case "9002":
                        strMessage = "電文程式名稱不為JCGU850";
                        break;
                    case "9003":
                        strMessage = "電文使用者ID為空白,或非指定通路";
                        break;
                    case "9004":
                        strMessage = "功能碼不為1、2、3";
                        break;
                    case "9005":
                        strMessage = "正卡ID為空白、Low value";
                        break;
                    case "9006":
                        strMessage = "車號為空白、Low value";
                        break;
                    case "9007":
                        strMessage = "鍵檔日為空白、Low value";
                        break;
                    case "0001":
                        strMessage = "查詢成功";
                        break;
                    case "7777":
                        strMessage = "檔案關閉或異常";
                        break;
                    case "8888":
                        strMessage = "資料不存在";
                        break;
                    case "9999":
                        strMessage = "其他錯誤";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;
            case HtgType.P4_JCLE:
                switch (strMessageCode)
                {
                    case "9001":
                        strMessage = "電文交易名稱不為JCLE";
                        break;
                    case "9002":
                        strMessage = "電文程式名稱不為JCGU852";
                        break;
                    case "9003":
                        strMessage = "電文使用者ID為空白,或非指定通路";
                        break;
                    case "9004":
                        strMessage = "功能碼不為1";
                        break;
                    case "9005":
                        strMessage = "正卡ID為空白、Low value";
                        break;
                    case "9006":
                        strMessage = "鍵檔日及異動日必須有任一帶日期區間";
                        break;
                    case "9007":
                        strMessage = "輸入日期區間需為6個月內";
                        break;
                    case "0001":
                        strMessage = "查詢成功";
                        break;
                    case "7777":
                        strMessage = "檔案關閉或異常";
                        break;
                    case "8888":
                        strMessage = "資料不存在";
                        break;
                    case "9999":
                        strMessage = "其他錯誤";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;
            case HtgType.P4_JC99:
                if (strMessageCode.Substring(0, 1) == "1")
                {
                    switch (strMessageCode)
                    {
                        case "1003":
                            strMessage = "羅馬拼音未符合規範";
                            break;

                        default:
                            strMessage = "輸入文字不符合規範";
                            break;
                    }
                }
                else
                {
                    strMessage = MessageHelper.GetMessage("01_00000000_030");
                }

                break;
            case HtgType.P4A_JC69://20190918 by Peggy
                switch (strMessageCode)
                {
                    case "0001":
                    case "0000":
                        strMessage = "成功. ";
                        break;
                    case "8888":
                        strMessage = "異常. ";
                        break;
                    case "9999":
                        strMessage = "檔案未開啟. ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;
            default:
                switch (strMessageCode)
                {
                    case "0001":
                    case "0000":
                        strMessage = "成功. ";
                        break;
                    case "9999":
                        strMessage = "系統異常 ! ";
                        break;
                    default:
                        strMessage = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;
        }
        return strMessage;
    }

    //*作者 趙呂梁
    //*創建日期：2009/12/22
    //*修改日期：2009/12/22 
    /// <summary>
    /// 得到操作電文提示信息
    /// </summary>
    /// <param name="type">電文枚舉類型</param>
    /// <param name="strType">電文檢核類型</param>
    /// <returns>提示信息</returns>
    public static string GetHtgMessage(HtgType type, string strType)
    {
        string strCondition = "";//*操作電文的環境
        string strWorkType = "";//*操作電文的類型

        switch (strType)
        {
            case "1":
                strWorkType = "查詢";
                strCondition = "P4";
                break;

            case "11":
                strWorkType = "查詢";
                strCondition = "P4A";
                break;

            case "12":
                strWorkType = "查詢";
                strCondition = "P4D";
                break;

            case "2":
                strWorkType = "異動";
                strCondition = "P4";
                break;

            case "21":
                strWorkType = "異動";
                strCondition = "P4A";
                break;

            case "22":
                strWorkType = "異動";
                strCondition = "P4D";
                break;
        }
        return strWorkType + type.ToString() + strCondition;
    }

    //*作者 趙呂梁
    //*創建日期：2009/12/22
    //*修改日期：2009/12/22
    /// <summary>
    /// 取得電文參數
    /// </summary>
    /// <param name="type">電文枚舉類型</param>
    /// <param name="strTemp">傳入參數</param>
    /// <returns>返回的字符串</returns>
    public static string GetStr(HtgType type, string strTemp)
    {
        switch (type)
        {
            case HtgType.P4_PCTI:
                #region P4_PCTI
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_PCTI_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4D_PCTI:
                #region P4D_PCTI
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4D_PCTI_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4_JCF7:
                #region P4_JCF7
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCF7_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4_JCAW:
                #region P4_JCAW
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCAW_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4_JCAX:
                #region P4_JCAX
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCAX_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4_JCHO:
                #region P4_JCHO
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCHO_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4A_JCGQ:
                #region P4A_JCGQ
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4A_JCGQ_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4_JCGQ:
                #region P4_JCGQ
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCGQ_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4A_JCGX:
                #region P4A_JCGX
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4A_JCGX_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4_JCGX:
                #region P4_JCGX
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCGX_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4_JCIL:
                #region P4_JCIL
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCIL_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4A_JCGR:
                #region P4A_JCGR
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4A_JCGR_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion
            case HtgType.P4A_JC66:
                #region P4A_JC66
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4A_JC66_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion
            case HtgType.P4A_JC67:
                #region P4A_JC67
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4A_JC67_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion
            case HtgType.P4_JCIJ:
                #region P4_JCIJ
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCIJ_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4_JCAA:
                #region P4_JCAA
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCAA_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4_JCAT:
                #region P4_JCAT
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCAT_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4_JCEM:
                #region P4_JCEM
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCEM_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4D_JCF6:
                #region P4D_JCF6
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4D_JCF6_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4_JCF6:
                #region P4_JCF6
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCF6_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4_JCDK:
                #region P4_JCDK
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCDK_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4_JCHR:
                #region P4_JCHR
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCHR_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4A_JCHR:
                #region P4A_JCHR
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4A_JCHR_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4A_JCHQ:
                #region P4A_JCHR
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4A_JCHQ_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4_JCHQ:
                #region P4_JCHQ
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCHQ_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4A_JCG1:
                #region P4A_JCG1
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4A_JCG1_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4L_LGOR:
                #region P4L_LGOR
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4L_LGOR_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                    default:
                        break;
                }
                break;
            #endregion P4L_LGOR

            case HtgType.P4L_LGAT:
                #region P4L_LGAT
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4L_LGAT_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                    default:
                        break;
                }
                break;
            #endregion P4L_LGAT
            //2011.12.12
            case HtgType.P8_000401:
                #region Bancs
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("000401_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                    default:
                        break;
                }
                break;
            #endregion Bancs
            case HtgType.P4A_JCPA:
                #region P4_JCAB
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4A_JCPA_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4D_JCFA:
                #region P4D_JCFA
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4D_JCFA_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4_JCFA:
                #region P4_JCFA
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCFA_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4_JCLB:
                #region P4_JCLB
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCLB_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4_JCLD:
                #region P4_JCLD
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCLD_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4_JCLE:
                #region P4_JCLE
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCLE_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion

            case HtgType.P4_JC99:
                #region 
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JC99_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion
            case HtgType.P4A_JC68:
                #region 
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4A_JC68_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion
            case HtgType.P4A_JC69://20190919 by Peggy
                #region P4A_JC69
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4A_JC69_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion
            case HtgType.P4_JCBG:
                #region P4_JCBG
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCDK_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion
            case HtgType.P4A_JC70:
                #region P4A_JC70
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4A_JC70_IsOnLine");

                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
            #endregion
            default:
                #region
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "ISONLINE":
                        //* 是否已經上線
                        return "FALSE";

                    case "AUTHONLINE":
                        return "FALSE";
                    default:
                        return "";
                }
                #endregion
        }
        return "";
    }

    /// <summary>
    /// 添加上傳主機SESSION信息
    /// </summary>
    /// <param name="htInput">上傳主機HashTable</param>
    /// <param name="eAgentInfo">Session變數集合</param>
    /// <param name="type">電文類型</param>
    public static void AddSession(Hashtable htInput, EntityAGENT_INFO eAgentInfo, HtgType type)
    {
        if (htInput.ContainsKey("userId"))
        {
            htInput["userId"] = eAgentInfo.agent_id;
        }
        else
        {
            htInput.Add("userId", eAgentInfo.agent_id);
        }

        if (htInput.ContainsKey("passWord"))
        {
            htInput["passWord"] = eAgentInfo.agent_pwd;
        }
        else
        {
            htInput.Add("passWord", eAgentInfo.agent_pwd);
        }

        if (htInput.ContainsKey("racfId"))
        {
            htInput["racfId"] = eAgentInfo.agent_id_racf;
        }
        else
        {
            htInput.Add("racfId", eAgentInfo.agent_id_racf);
        }

        if (htInput.ContainsKey("racfPassWord"))
        {
            htInput["racfPassWord"] = eAgentInfo.agent_id_racf_pwd;
        }
        else
        {
            htInput.Add("racfPassWord", eAgentInfo.agent_id_racf_pwd);
        }

        if (htInput.ContainsKey("USER_ID"))
        {
            if (htInput.ContainsKey("TRAN_ID"))
            {
                if (htInput["TRAN_ID"].ToString() != "JCLD" && htInput["TRAN_ID"].ToString() != "JCLB" && htInput["TRAN_ID"].ToString() != "JCLE")
                {
                    htInput["USER_ID"] = eAgentInfo.agent_id_racf;
                }
            }
        }
        else
        {
            htInput.Add(GetStr(type, "USER_ID"), eAgentInfo.agent_id_racf);
        }

        if (HttpContext.Current != null)//*判斷是JOB還是網頁中上傳主機
        {
            if (HttpContext.Current.Session["sessionId"] != null)
            {
                if (htInput.ContainsKey("sessionId"))
                {
                    htInput["sessionId"] = HttpContext.Current.Session["sessionId"].ToString().Trim();
                }
                else
                {
                    htInput.Add("sessionId", HttpContext.Current.Session["sessionId"].ToString().Trim());
                }
            }
            else
            {
                if (htInput.ContainsKey("sessionId"))
                {
                    htInput["sessionId"] = "";
                }
                else
                {
                    htInput.Add("sessionId", "");
                }
            }
        }
        else
        {
            if (!htInput.ContainsKey("sessionId"))
            {
                htInput.Add("sessionId", "");
            }
        }
    }

    /// <summary>
    /// 添加上傳主機SESSION信息
    /// </summary>
    /// <param name="htInput">上傳主機HashTable</param>
    /// <param name="eAgentInfo">Session變數集合</param>
    /// <param name="type">電文類型</param>
    public static void AddSessionJob(Hashtable htInput, EntityAGENT_INFO eAgentInfo, HtgType type, string strSession)
    {
        if (htInput.ContainsKey("userId"))
        {
            htInput["userId"] = eAgentInfo.agent_id;
        }
        else
        {
            htInput.Add("userId", eAgentInfo.agent_id);
        }

        if (htInput.ContainsKey("passWord"))
        {
            htInput["passWord"] = eAgentInfo.agent_pwd;
        }
        else
        {
            htInput.Add("passWord", eAgentInfo.agent_pwd);
        }

        if (htInput.ContainsKey("racfId"))
        {
            htInput["racfId"] = eAgentInfo.agent_id_racf;
        }
        else
        {
            htInput.Add("racfId", eAgentInfo.agent_id_racf);
        }

        if (htInput.ContainsKey("racfPassWord"))
        {
            htInput["racfPassWord"] = eAgentInfo.agent_id_racf_pwd;
        }
        else
        {
            htInput.Add("racfPassWord", eAgentInfo.agent_id_racf_pwd);
        }

        if (htInput.ContainsKey("USER_ID"))
        {
            htInput["USER_ID"] = eAgentInfo.agent_id_racf;
        }
        else
        {
            htInput.Add(GetStr(type, "USER_ID"), eAgentInfo.agent_id_racf);
        }

        if (HttpContext.Current != null)//*判斷是JOB還是網頁中上傳主機
        {
            if (HttpContext.Current.Session["sessionId"] != null)
            {
                if (htInput.ContainsKey("sessionId"))
                {
                    htInput["sessionId"] = HttpContext.Current.Session["sessionId"].ToString().Trim();
                }
                else
                {
                    htInput.Add("sessionId", HttpContext.Current.Session["sessionId"].ToString().Trim());
                }
            }
            else
            {
                if (htInput.ContainsKey("sessionId"))
                {
                    htInput["sessionId"] = strSession;
                }
                else
                {
                    htInput.Add("sessionId", strSession);
                }
            }
        }
        else
        {
            if (!htInput.ContainsKey("sessionId"))
            {
                htInput.Add("sessionId", "");
            }
        }
    }

    /// <summary>
    /// 清空主機Session
    /// </summary>
    public static bool ClearHtgSession()
    {
        //string strMsg = "";
        //string strIsOnline = UtilHelper.GetAppSettings("AUTH_IsOnLine");
        //try
        //{
        //    //* 取得Session中存的主機SessionID
        //    string strSessionID = (HttpContext.Current.Session["sessionId"] + "").Trim();
        //    if (!string.IsNullOrEmpty(strSessionID))
        //    {
        //        //* 如果不爲空.發送主機電文,關掉
        //        HTGCommunicator hc = new HTGCommunicator();
        //        hc.SessionId = strSessionID;
        //        hc.CloseSession(ref strMsg, strIsOnline);
        //        //* 清空Session中的主機SessionID
        //        //System.Web.HttpContext.Current.Session["sessionId"] = "";
        //        HttpContext.Current.Session["sessionId"] = null;
        //    }

        //}
        //catch
        //{
        //    Logging.SaveLog(ELogLayer.HTG, strMsg, ELogType.Fatal);
        //    return false;
        //}
        return true;
    }

    /// <summary>
    /// 清空主機Session
    /// </summary>
    public static bool ClearHtgSessionJob(ref string SessionID, String jobid = "")
    {
        string strMsg = "";
        string strIsOnline = UtilHelper.GetAppSettings("AUTH_IsOnLine");
        try
        {
            //* 取得Session中存的主機SessionID
            string strSessionID = SessionID;
            if (!string.IsNullOrEmpty(strSessionID))
            {
                //* 如果不爲空.發送主機電文,關掉
                HTGCommunicator hc = new HTGCommunicator(jobid);
                hc.SessionId = strSessionID;
                hc.CloseSession(ref strMsg, strIsOnline);
                //* 清空Session中的主機SessionID
                SessionID = "";

            }
        }
        catch
        {
            Logging.Log(strMsg, LogState.Error, LogLayer.HTG);
            return false;
        }
        return true;
    }

    /// <summary>
    /// 依據JCF6組合PCTI
    /// </summary>
    public static void ChangeJCF6toPCTI(Hashtable htInput, Hashtable output, ArrayList arrayName)
    {

        foreach (string strTemp in arrayName)
        {
            string strValue = htInput[strTemp].ToString().Trim();
            switch (strTemp)
            {
                case "sessionId":
                    GetNewHashTable(output, "sessionId", strValue);
                    break;

                case "userId":
                    GetNewHashTable(output, "userId", strValue);
                    break;

                case "passWord":
                    GetNewHashTable(output, "passWord", strValue);
                    break;

                case "racfId":
                    GetNewHashTable(output, "racfId", strValue);
                    break;

                case "racfPassWord":
                    GetNewHashTable(output, "racfPassWord", strValue);
                    break;

                case "USER_ID":
                    GetNewHashTable(output, "USER_ID", strValue);
                    break;

                case "ACCT_NBR": //*ID
                    GetNewHashTable(output, "ID_DATA", "822" + strValue);
                    break;

                case "CITY": //*城市名
                    GetNewHashTable(output, "CITY", strValue);
                    break;

                case "ADDR_1"://*帳單地址第一段
                    GetNewHashTable(output, "ADDR_LINE_1", strValue);
                    break;

                case "ADDR_2"://*帳單地址第二段
                    GetNewHashTable(output, "ADDR_LINE_2", strValue);
                    break;

                case "EMPLOYER"://*公司名稱
                    GetNewHashTable(output, "OWNER", strValue);
                    break;

                case "HOME_PHONE"://*住家電話
                    GetNewHashTable(output, "TELEPHONE", strValue);
                    break;

                case "OFFICE_PHONE"://*公司電話
                    GetNewHashTable(output, "WORK_PHONE", strValue);
                    break;

                case "MANAGE_ZIP_CODE":  //*管理郵區
                    GetNewHashTable(output, "MAIL_IND", strValue);
                    break;
                case "ZIP"://*郵遞區號
                    GetNewHashTable(output, "ZIP", strValue);
                    break;

                case "MEMO_1"://*註記一
                    GetNewHashTable(output, "MEMO_1", strValue);
                    break;

                case "MEMO_2"://*註記二
                    GetNewHashTable(output, "MEMO_2", strValue);
                    break;

                case "BIRTH_DATE"://*生日
                    GetNewHashTable(output, "BIRTH_DATE", strValue);
                    break;

                case "NAME_1"://*姓名
                    GetNewHashTable(output, "NAME_LINE_1", strValue);
                    break;

                case "NAME_1_2"://*別名
                    GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "OFF_PHONE_FLAG"://*電子帳單
                    GetNewHashTable(output, "MAIL", strValue);
                    break;

                case "CO_OWNER"://*扣繳帳號第一段+第二段
                    GetNewHashTable(output, "BK_ID_AC", strValue);
                    break;

                case "CO_TAX_ID_TYPE"://*繳款狀況
                    GetNewHashTable(output, "LAST_CR_LINE_IND", strValue);
                    break;

                case "DD_ID"://*帳戶ID
                    GetNewHashTable(output, "DD_ID", strValue);
                    break;

                case "BILLING_CYCLE"://*帳單週期
                    GetNewHashTable(output, "BILLING_CYCLE", strValue);
                    break;

                case "EU_CUSTOMER_CLASS"://*學歷
                    GetNewHashTable(output, "EDUCATION_CODE", strValue);
                    break;

                case "GRADUATE_YYMM"://*畢業西元年月
                    GetNewHashTable(output, "GRADUATION_DATE", strValue);
                    break;

                case "EU_NBR_OF_DEPS"://*族群碼
                    GetNewHashTable(output, "EU_NBR_OF_DEPS", strValue);
                    break;

                default://*族群碼
                    break;

            }
        }
    }

    /// <summary>
    /// 依據JCDH組合PCTI
    /// </summary>
    public static void ChangeJCF7toPCTI(Hashtable htInput, Hashtable output, ArrayList arrayName, string strType)
    {
        foreach (string strTemp in arrayName)
        {
            string strValue = htInput[strTemp].ToString().Trim();
            switch (strTemp)
            {
                case "sessionId":
                    GetNewHashTable(output, "sessionId", strValue);
                    break;

                case "userId":
                    GetNewHashTable(output, "userId", strValue);
                    break;
                case "passWord":
                    GetNewHashTable(output, "passWord", strValue);
                    break;

                case "racfId":
                    GetNewHashTable(output, "racfId", strValue);
                    break;

                case "racfPassWord":
                    GetNewHashTable(output, "racfPassWord", strValue);
                    break;

                case "USER_ID":
                    GetNewHashTable(output, "USER_ID", strValue);
                    break;

                case "ACCT_NBR": //*ID
                    GetNewHashTable(output, "CARD_DATA", "822" + strType + strValue);
                    break;

                case "PYMT_FLAG"://*繳款方式
                    GetNewHashTable(output, "PAYMENT_TYPE", strValue);
                    break;

                case "FIXED_PYMT_AMNT"://*固定繳款額
                    GetNewHashTable(output, "FIXED_PAYMENT", strValue);
                    break;

                case "CURR_DUE"://*本期應繳
                    GetNewHashTable(output, "CURR", strValue);
                    break;

                case "PAST_DUE"://*預期一個月內
                    GetNewHashTable(output, "XDAY", strValue);
                    break;

                case "30DAYS_DELQ"://*30 DAYS
                    GetNewHashTable(output, "30DAY", strValue);
                    break;

                case "60DAYS_DELQ"://*60 DAYS
                    GetNewHashTable(output, "60DAY", strValue);
                    break;

                case "90DAYS_DELQ"://*90 DAYS
                    GetNewHashTable(output, "90DAY", strValue);
                    break;

                case "120DAYS_DELQ"://*120 DAYS
                    GetNewHashTable(output, "120DAY", strValue);
                    break;

                case "150DAYS_DELQ"://*150 DAYS
                    GetNewHashTable(output, "150DAY", strValue);
                    break;

                case "180DAYS_DELQ"://*180 DAYS
                    GetNewHashTable(output, "180DAY", strValue);
                    break;

                case "210DAYS_DELQ"://*210 DAYS
                    GetNewHashTable(output, "210DAY", strValue);
                    break;

                case "USER_CODE"://*優惠碼
                    GetNewHashTable(output, "CHPM_CODE", strValue);
                    break;

                case "USER_CODE_2"://*卡人類別
                    GetNewHashTable(output, "CATEGORY", strValue);
                    break;

                case "CHGOFF_STATUS_FLAG"://*BLK CODE
                    GetNewHashTable(output, "STATUS_FLAG", strValue);
                    break;

                case "SHORT_NAME"://*姓名
                    GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "NAME_1":
                    GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "EMBOSSER_NAME_11"://*英文姓名
                    GetNewHashTable(output, "E1", strValue);
                    break;

                case "CARD_EXPIR_DTE"://*卡片有效日
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST1"://*繳款評等1-24
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST2":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;
                case "DELQ_HIST3":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST4":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST5":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST6":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST7":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST8":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST9":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST10":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST11":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST12":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST13":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST14":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST15":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST16":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST17":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST18":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST19":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST20":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST21":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST22":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST23":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;

                case "DELQ_HIST24":
                    //GetNewHashTable(output, "SHORT_NAME", strValue);
                    break;
            }
        }
    }

    /// <summary>
    /// 依據JCDH組合PCTI
    /// </summary>
    public static void ChangeJCAAtoPCTI(Hashtable htInput, Hashtable output, ArrayList arrayName, string strType)
    {
        foreach (string strTemp in arrayName)
        {
            string strValue = htInput[strTemp].ToString().Trim();
            switch (strTemp)
            {
                case "sessionId":
                    GetNewHashTable(output, "sessionId", strValue);
                    break;

                case "userId":
                    GetNewHashTable(output, "userId", strValue);
                    break;
                case "passWord":
                    GetNewHashTable(output, "passWord", strValue);
                    break;

                case "racfId":
                    GetNewHashTable(output, "racfId", strValue);
                    break;

                case "racfPassWord":
                    GetNewHashTable(output, "racfPassWord", strValue);
                    break;

                case "USER_ID":
                    GetNewHashTable(output, "USER_ID", strValue);
                    break;

                case "ACCT_NBR": //*ID
                    GetNewHashTable(output, "CARD_DATA", "822" + strType + strValue);
                    break;

                case "BLOCK_CODE":
                    GetNewHashTable(output, "STATUS_FLAG", strValue);
                    break;
            }
        }
    }

    /// <summary>
    /// 依據JCEM組合JCAW
    /// </summary>
    public static void ChangeJCEMtoJCAW(Hashtable htInput, Hashtable output, ArrayList arrayName)
    {
        foreach (string strTemp in arrayName)
        {
            string strValue = htInput[strTemp].ToString().Trim();
            switch (strTemp)
            {
                case "sessionId":
                    GetNewHashTable(output, "sessionId", strValue);
                    break;

                case "userId":
                    GetNewHashTable(output, "userId", strValue);
                    break;
                case "passWord":
                    GetNewHashTable(output, "passWord", strValue);
                    break;

                case "racfId":
                    GetNewHashTable(output, "racfId", strValue);
                    break;

                case "racfPassWord":
                    GetNewHashTable(output, "racfPassWord", strValue);
                    break;

                case "USER_ID":
                    GetNewHashTable(output, "USER_ID", strValue);
                    break;

                case "ACCT_NBR": //*卡號
                    GetNewHashTable(output, "CARD_NO", strValue);
                    break;

                case "CARD_TAKE"://*取卡方式
                    GetNewHashTable(output, "SELF_TAKE", strValue);
                    break;

                case "EMBOSS_NAME"://*製卡英文姓名
                    GetNewHashTable(output, "EMBNAME", strValue);
                    break;

                case "EMBOSS_TYPE"://*製卡樣式
                    GetNewHashTable(output, "EMBTYPE", strValue);
                    break;

                case "MEM_NO"://*會員編號
                    GetNewHashTable(output, "MEMNO", strValue);
                    break;

                case "EXPIRE_DATE"://*卡片有效月年
                    if (strValue.Length == 4)
                    {
                        GetNewHashTable(output, "EXPDATE_MM", strValue.Substring(0, 2));
                        GetNewHashTable(output, "EXPDATE_YY", strValue.Substring(2, 2));
                    }
                    //GetNewHashTable(output, "EXPIRE_DATE", strValue);
                    break;
            }
        }
    }

    /// <summary>
    /// 依據JCAT組合JCAW
    /// </summary>
    public static void ChangeJCATtoJCAW(Hashtable htInput, Hashtable output, ArrayList arrayName)
    {
        foreach (string strTemp in arrayName)
        {
            string strValue = htInput[strTemp] != null ? htInput[strTemp].ToString().Trim() : "";
            switch (strTemp)
            {
                case "sessionId":
                    GetNewHashTable(output, "sessionId", strValue);
                    break;

                case "userId":
                    GetNewHashTable(output, "userId", strValue);
                    break;

                case "passWord":
                    GetNewHashTable(output, "passWord", strValue);
                    break;

                case "racfId":
                    GetNewHashTable(output, "racfId", strValue);
                    break;

                case "racfPassWord":
                    GetNewHashTable(output, "racfPassWord", strValue);
                    break;

                case "USER_ID":
                    GetNewHashTable(output, "USER_ID", strValue);
                    break;

                case "CARD_NMBR": //*卡號
                    GetNewHashTable(output, "CARD_NO", strValue);
                    break;

                case "CARD_TAKE": //*取卡方式
                    GetNewHashTable(output, "SELF_TAKE", strValue);
                    break;

                case "EMBOSSER_NAME"://*製卡英文姓名
                    GetNewHashTable(output, "EMBNAME", strValue);
                    break;

                case "CM_CARD_TYPE"://*製卡樣式
                    GetNewHashTable(output, "EMBTYPE", strValue);
                    break;

                case "MEMBER_NO"://*會員編號
                    GetNewHashTable(output, "MEMNO", strValue);
                    break;

                case "EXPIR_DATE"://*卡片有效月年
                    if (strValue.Length == 4)
                    {
                        GetNewHashTable(output, "EXPDATE_MM", strValue.Substring(0, 2));
                        GetNewHashTable(output, "EXPDATE_YY", strValue.Substring(2, 2));
                    }
                    //GetNewHashTable(output, "EXPIRE_DATE", strValue);
                    break;
            }
        }
    }

    /// <summary>
    /// 依據JCGR組合JCIL
    /// </summary>
    public static void ChangeJCGRtoJCIL(Hashtable htInput, Hashtable output, ArrayList arrayName)
    {
        foreach (string strTemp in arrayName)
        {
            string strValue = htInput[strTemp].ToString().Trim();
            switch (strTemp)
            {
                case "sessionId":
                    GetNewHashTable(output, "sessionId", strValue);
                    break;

                case "userId":
                    GetNewHashTable(output, "userId", strValue);
                    break;
                case "passWord":
                    GetNewHashTable(output, "passWord", strValue);
                    break;

                case "racfId":
                    GetNewHashTable(output, "racfId", strValue);
                    break;

                case "racfPassWord":
                    GetNewHashTable(output, "racfPassWord", strValue);
                    break;

                case "USER_ID":
                    GetNewHashTable(output, "USER_ID", strValue);
                    break;

                case "MERCHANT_NO": //*商店代號
                    GetNewHashTable(output, "MER_NO", strValue);
                    break;

                case "BUSINESS_NAME"://*商店營業名稱
                    GetNewHashTable(output, "MER_NEME", strValue);
                    break;

                case "CONTACT_NAME"://*聯絡人姓名
                    GetNewHashTable(output, "OWNER_NAME", strValue);
                    break;

                case "CONTACT_PHONE_AREA"://*聯絡人電話區域號碼 + 聯絡人電話號碼 + 聯絡人電話分機號碼
                    strValue = htInput["CONTACT_PHONE_AREA"].ToString() + "-" + htInput["CONTACT_PHONE_NO"].ToString() + "-" + htInput["CONTACT_PHONE_EXT"].ToString();
                    GetNewHashTable(output, "CONTACT_TEL", strValue);
                    break;

                case "FAX_AREA"://*聯絡人fax
                    strValue = htInput["FAX_AREA"].ToString() + "-" + htInput["FAX_PHONE_NO"].ToString();
                    GetNewHashTable(output, "CONTACT_FAX", strValue);
                    break;

                case "REAL_CITY"://*商店營業地址1
                    GetNewHashTable(output, "ADDRESS1", strValue);
                    break;

                case "REAL_ADDR1"://*商店營業地址2
                    GetNewHashTable(output, "ADDRESS2", strValue);
                    break;

                case "REAL_ADDR2"://*商店營業地址3
                    GetNewHashTable(output, "ADDRESS3", strValue);
                    break;

                case "DDA_BANK_NAME"://*銀行名稱
                    strValue = htInput["DDA_BANK_NAME"].ToString() + htInput["DDA_BANK_BRANCH"].ToString();
                    GetNewHashTable(output, "BANK_NAME", strValue);
                    break;

                case "DDA_ACCT_NAME"://*戶名
                    GetNewHashTable(output, "ACCT_NEME", strValue);
                    break;

                case "USER_DATA"://*帳號(2)
                    GetNewHashTable(output, "ACCT_NO", strValue);
                    break;
            }
        }
    }

    /// <summary>
    /// 添加轉化后的HashTable
    /// </summary>
    /// <param name="htOutput">HashTable</param>
    /// <param name="strKey">鍵</param>
    /// <param name="strValue">值</param>
    public static void GetNewHashTable(Hashtable htOutput, string strKey, string strValue)
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

    /// <summary>
    /// 重覆查詢電文
    /// </summary>
    /// <param name="type">電文枚舉類型</param>
    /// <param name="htInput">傳入參數的HashTable</param>
    /// <param name="htOutput">傳出參數的HashTable</param>
    /// <param name="dt">組成的GridView資料</param>
    /// <param name="uploadCount">重覆查詢次數</param>
    /// <param name="eAgentInfo">Session變數集合</param>
    public static void GetHtgOutput(HtgType type, Hashtable htInput, ref Hashtable htOutput, ref DataTable dt, ref int uploadCount, ref EntityAGENT_INFO eAgentInfo)
    {
        try
        {
            DataRow dr = dt.NewRow();
            string dataID = string.Empty;
            string colName = string.Empty;

            // 上傳時的起始行數為前一次回傳的行數，第一次為0000
            if (uploadCount > 0)
            {
                htInput["LINE_CNT"] = htOutput["LINE_CNT"].ToString();
            }

            // 下行
            htOutput = MainFrameInfo.GetMainFrameInfo(type, htInput, false, "1", eAgentInfo);

            // 只有第一次查詢時將欄位加入DataTable
            if (uploadCount == 0) AddHtgColumnToDataTable(type, ref dt);

            // 主機沒有回傳錯誤訊息才進行下一次查詢
            if (!htOutput.Contains("HtgMsg"))
            {
                // 是否繼續往下查詢
                bool isSendReq = true;

                // 主機回傳資料顯示規則
                // 1. 非最後一次回傳
                // 2. 最後一次回傳且最後筆數(LINE_CNT)不為5的倍數
                // 3. 倒數最後一次回傳筆數不到5筆，要排除最後一次回傳的資料
                if (htInput["LINE_CNT"].ToString() != htOutput["LINE_CNT"].ToString()
                    || (htInput["LINE_CNT"].ToString() == htOutput["LINE_CNT"].ToString() && int.Parse(htOutput["LINE_CNT"].ToString()) % 5 != 0))
                {
                    // 檢查電文內包含數字1-5的欄位，分別取出組成屬於第1筆至第5筆的資料
                    for (int i = 1; i <= 5; i++)
                    {
                        if (htOutput["ACCT_ID" + i.ToString()].ToString() == "")
                        {
                            // 回傳筆數不滿5筆表示後面無資料，不再繼續往下查詢
                            isSendReq = false;
                            continue;
                        }

                        dr = dt.NewRow();
                        dr["COUNT"] = 5 * uploadCount + i;

                        foreach (DictionaryEntry entry in htOutput)
                        {
                            dataID = entry.Key.ToString();

                            if (dataID.Contains(i.ToString()))
                            {
                                colName = dataID.Substring(0, dataID.Length - 1);
                                dr[colName] = entry.Value.ToString();
                            }
                        }

                        dt.Rows.Add(dr);
                    }

                    uploadCount++;
                }

                // 回傳的起始行數和前一次不同時才查詢
                if (htInput["LINE_CNT"].ToString() == "0000" || htInput["LINE_CNT"].ToString() != htOutput["LINE_CNT"].ToString())
                {
                    if (isSendReq)
                    {
                        // 重覆查詢電文
                        GetHtgOutput(type, htInput, ref htOutput, ref dt, ref uploadCount, ref eAgentInfo);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.HTG);
        }
    }

    /// <summary>
    /// 將畫面要顯示的但電文欄位加入DataTable
    /// </summary>
    /// <param name="type"></param>
    /// <param name="dt"></param>
    public static void AddHtgColumnToDataTable(HtgType type, ref DataTable dt)
    {
        switch (type)
        {
            case HtgType.P4_JCLB:
                dt.Columns.Add("COUNT", typeof(System.Int32));
                dt.Columns.Add("MESSAGE_TYPE");
                dt.Columns.Add("FILLER");
                dt.Columns.Add("PROGRAM_ID");
                dt.Columns.Add("CASE_FLAG");
                dt.Columns.Add("TRAN_ID");
                dt.Columns.Add("ACCTID");
                dt.Columns.Add("USER_ID");
                dt.Columns.Add("LINE_CNT");
                dt.Columns.Add("FUNCTION_CODE");
                //dt.Columns.Add("KEYIN_DATE");
                //dt.Columns.Add("LPR_NO");

                dt.Columns.Add("TYPE");
                dt.Columns.Add("ACCT_ID");
                dt.Columns.Add("LPR_NO");
                dt.Columns.Add("CUSTID");
                dt.Columns.Add("STATUS");
                dt.Columns.Add("NO");
                dt.Columns.Add("SERVICE_A_DATE");
                dt.Columns.Add("SERVICE_D_DATE");
                dt.Columns.Add("RESULT_A_CODE");
                dt.Columns.Add("RESULT_D_CODE");
                dt.Columns.Add("APPLY_NO");
                dt.Columns.Add("KEYIN_DATE");
                dt.Columns.Add("UPDATE_USER");
                dt.Columns.Add("UPDATE_DATE");
                dt.Columns.Add("UPDATE_TIME");
                dt.Columns.Add("SEND_DATE");
                dt.Columns.Add("SPC");
                dt.Columns.Add("SPC_ID");
                dt.Columns.Add("INTRODUCER_CARD_ID");
                dt.Columns.Add("EFF_DATE");
                dt.Columns.Add("SOURCE");
                dt.Columns.Add("CHANNEL");
                dt.Columns.Add("VENDOR_NAME");
                break;
            case HtgType.P4_JCLE:
                dt.Columns.Add("COUNT", typeof(System.Int32));
                dt.Columns.Add("TRAN_ID");
                dt.Columns.Add("PROGRAM_ID");
                dt.Columns.Add("USER_ID");
                dt.Columns.Add("MESSAGE_TYPE");
                dt.Columns.Add("FUNCTION_CODE");
                dt.Columns.Add("LINE_CNT");
                dt.Columns.Add("ACCTID");
                dt.Columns.Add("UPDATE_DATE_S");
                dt.Columns.Add("UPDATE_DATE_E");
                dt.Columns.Add("KEYIN_DATE_S");
                dt.Columns.Add("KEYIN_DATE_E");
                dt.Columns.Add("FILLER_00");
                //dt.Columns.Add("LPR_NO");
                //dt.Columns.Add("APPLY_NO");

                dt.Columns.Add("ACTION");
                dt.Columns.Add("TYPE");
                dt.Columns.Add("ACCT_ID");
                dt.Columns.Add("LPR_NO");
                dt.Columns.Add("CUSTID");
                dt.Columns.Add("STATUS");
                dt.Columns.Add("NO");
                dt.Columns.Add("SERVICE_A_DATE");
                dt.Columns.Add("SERVICE_D_DATE");
                dt.Columns.Add("RESULT_A_CODE");
                dt.Columns.Add("RESULT_D_CODE");
                dt.Columns.Add("APPLY_NO");
                dt.Columns.Add("KEYIN_DATE");
                dt.Columns.Add("UPDATE_USER");
                dt.Columns.Add("UPDATE_DATE");
                dt.Columns.Add("UPDATE_TIME");
                dt.Columns.Add("SEND_DATE");
                dt.Columns.Add("SPC");
                dt.Columns.Add("SPC_ID");
                dt.Columns.Add("INTRODUCER_CARD_ID");
                dt.Columns.Add("EFF_DATE");
                dt.Columns.Add("SOURCE");
                dt.Columns.Add("CHANNEL");
                dt.Columns.Add("VENDOR_NAME");
                break;
        }
    }
}