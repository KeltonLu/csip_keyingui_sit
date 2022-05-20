﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Framework.Common.Utility;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using TIBCO.EMS;
using System.IO;
using System.Xml;
using System.Data;
using CSIPCommonModel.BusinessRules;
using CSIPKeyInGUI.EntityLayer;

namespace CSIPKeyInGUI.BusinessRules
{
    /// <summary>
    /// ConntoESB 的摘要描述
    /// </summary>
    public class ConntoESB
    {/// <summary>
     /// 
     /// </summary>
     /// <param name="group">ESB連線組別</param>
     /// <param name="strXML">電文XML</param>
     /// <param name="msgNull"></param>
     /// <param name="ConnEndTime"></param>
     /// <param name="SendupEndTime"></param>
     /// <param name="ReceDownEndTime"></param>
     /// <param name="ConnColseEndTime"></param>
     /// <param name="Uat"></param>
     /// <param name="timeout"></param>
     /// <returns></returns>

     /// <summary>
     /// ESB電文處理
     /// </summary>
     /// <param name="esbObj">ESB 電文物件</param>/// 
     /// <param name="group">預設電文組別</param>/// 
        public static string ConnESB(ESBObject esbObj)
        {
            //將預計上送的XML轉出並記錄
            string strXML = esbObj.getXML();

            //紀錄上送的LOG
            SaveESBLog(strXML.Replace("><", ">\r\n<"), "REQ");

            //抓ReTry 的次數, 若為0則只跑1次, 不重跑
            int RTDSRetry = Convert.ToInt32(UtilHelper.GetAppSettings("RTDSRetry").ToString());

            string ConnEndTime = "";//  連線結束時間
            string SendupEndTime = "";//發送上行結束時間
            string ReceDownEndTime = "";//收到下行結束時間
            string ConnColseEndTime = "";//關閉連接結束時間
            string strESBMsg = "";
            bool msgNull = false;
            bool esbtimeout = false; // 紀錄ESB是否Timeout
            string group = "1"; //ESB電文連線預設為第一組, 若要切換請更改屬性維護值

            DataTable dt = new DataTable();
            BRM_PROPERTY_KEY.GetEnableProperty("01", "ESBConnection", ref dt);
            if (dt.Rows.Count <= 0 || dt == null)
            {
                group = "1";
            }
            else
            {
                group = dt.Rows[0]["PROPERTY_NAME"].ToString();
            }

            #region params
            //第一組
            string ServerUrl = string.Empty;
            string ServerPort = string.Empty;
            string UserName = string.Empty;
            string Password = string.Empty;
            string ESBSendQueueName = string.Empty;
            string ESBReceiveQueueName = string.Empty;
            #endregion
            if (group != "2")
            {
                // 第一組
                ServerUrl = UtilHelper.GetAppSettings("ESB_ServerUrl").ToString();
                ServerPort = UtilHelper.GetAppSettings("ESB_ServerPort").ToString();
                UserName = UtilHelper.GetAppSettings("ESB_UserName").ToString();
                Password = UtilHelper.GetAppSettings("ESB_Password").ToString();
                ESBSendQueueName = UtilHelper.GetAppSettings("ESB_SendQueueName").ToString();
                ESBReceiveQueueName = UtilHelper.GetAppSettings("ESB_ReceiveQueueName").ToString();

            }
            else
            {
                // 第二組
                ServerUrl = UtilHelper.GetAppSettings("ESB_ServerUrl_1").ToString();
                ServerPort = UtilHelper.GetAppSettings("ESB_ServerPort_1").ToString();
                UserName = UtilHelper.GetAppSettings("ESB_UserName_1").ToString();
                Password = UtilHelper.GetAppSettings("ESB_Password_1").ToString();
                ESBSendQueueName = UtilHelper.GetAppSettings("ESB_SendQueueName_1").ToString();
                ESBReceiveQueueName = UtilHelper.GetAppSettings("ESB_ReceiveQueueName_1").ToString();
            }
            //當線路1 連線錯誤　& TimeOut msgNull = ture 跑線路2
            msgNull = false;
            string strResult = string.Empty;
            string _url = string.Empty;
            string _messageid = string.Empty;
            string tagValue = string.Empty;
            // ESB 設定Timeout秒數
            int ESBTimeout = Convert.ToInt32(UtilHelper.GetAppSettings("ESBTimeout").ToString());
            _url = "tcp://" + ServerUrl + ":" + ServerPort;
            /* 方法二,直接使用QueueConnectionFactory */

            QueueConnectionFactory factory = null;
            QueueConnection connection = null;
            try
            {
                for (int i = 0; i <= RTDSRetry; i++)
                {
                    Logging.Log(string.Format("發查ESB電文，連線編號：{0}，連線IP：{1}，連線埠號：{2}，使用者名稱：{3}", group, ServerUrl, ServerPort, UserName));
                    factory = new TIBCO.EMS.QueueConnectionFactory(_url);
                    connection = factory.CreateQueueConnection(UserName, Password);
                    QueueSession session = connection.CreateQueueSession(false, TIBCO.EMS.Session.AUTO_ACKNOWLEDGE);
                    TIBCO.EMS.Queue queue = session.CreateQueue(ESBSendQueueName);
                    QueueSender qsender = session.CreateSender(queue);
                    /* send messages */
                    TextMessage message = session.CreateTextMessage();
                    message.Text = strXML;
                    //一定要設定要reply的queue,這樣才收得到
                    message.ReplyTo = (TIBCO.EMS.Destination)session.CreateQueue(ESBReceiveQueueName);
                    ConnEndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    qsender.Send(message);
                    SendupEndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    _messageid = message.MessageID;

                    //receive message
                    String messageselector = null;
                    messageselector = "JMSCorrelationID = '" + _messageid + "'";
                    TIBCO.EMS.Queue receivequeue = session.CreateQueue(ESBReceiveQueueName);
                    QueueReceiver receiver = session.CreateReceiver(receivequeue, messageselector);
                    connection.Start();
                    //set up timeout 
                    TIBCO.EMS.Message msg = receiver.Receive(ESBTimeout * 1000);
                    //確認是否成功連線
                    if (msg == null)
                    {
                        //此狀況無法取得下行電文，StatusCode/RspCode/ErrorCode空值為正常
                        esbObj.ConnStatus = "F";
                        msgNull = true;
                        strResult = string.Format("CSIP 接收ESB 訊息無回應， 超過 timeout 設定:   {0}   秒", ESBTimeout);
                        esbObj.ErrorMessage = strResult;
                        Logging.Log(string.Format("CSIP 接收ESB 訊息無回應， 超過 timeout 設定:   {0}   秒", ESBTimeout), LogLayer.UI);
                    }
                    else
                    {
                        msg.Acknowledge();
                        if (msg is TextMessage)
                        {
                            TextMessage tm = (TextMessage)msg;
                            strResult = tm.Text;
                            //連線結果塞回物件
                            esbObj.getResult(strResult);
                            if (esbObj.StatusCode == "0")
                            {
                                esbObj.ConnStatus = "S";
                                //連線成功，紀錄RTN
                                SaveESBLog(strResult, "RTN");
                                break;
                            }
                            else
                            {
                                esbObj.ConnStatus = "F";
                            }
                        }
                        else
                        {
                            strResult = msg.ToString();
                        }
                    }

                    Logging.Log(" 發查次數：" + i.ToString() + "；ConnESB(" + _url + ") 電文 Result：" + strResult, LogLayer.UI);
                    SaveESBLog(strResult, "RTN");
                }
                return strResult;
            }
            catch (Exception ex)
            {
                esbObj.ConnStatus = "F";
                Logging.Log("ESB連線錯誤\r\n" + ex.ToString(), LogLayer.UI);
                return "ESB連線錯誤";
            }
            finally
            {
                if (connection != null)
                    connection.Close();

                #region ESB 電文結果LOG
                string resultRspCode = esbObj.RspCode;
                string resultErrorMsg = esbObj.ErrorMessage;
                string resultErrorCode = esbObj.ErrorCode;
                //20211019_待確認Log是否OK
                if (esbObj.ConnStatus == "S")
                {
                    switch (resultRspCode)
                    {
                        case "0000":
                            Logging.Log(String.Format("StatusCode：{0}；RspCode：{1}；CallESB SUCCESS；KeyInGUI ESB  Success", esbObj.StatusCode, resultRspCode), LogState.Info, LogLayer.UI);
                            break;
                        case "C001":
                            Logging.Log(String.Format("StatusCode：{0}；RspCode：{1}；ErrorCode：{2}；Message：{3}", esbObj.StatusCode, resultRspCode, resultErrorCode, resultErrorMsg), LogState.Info, LogLayer.UI);
                            break;
                        case "9200":
                            esbObj.ResultAlertMessage = string.Format("ESB電文系統維護中，狀態代碼：{0}，請重新操作", resultRspCode);
                            Logging.Log(String.Format("StatusCode：{0}；RspCode：{1}；ErrorCode：{2}；Message：{3}", esbObj.StatusCode, resultRspCode, resultErrorCode, resultErrorMsg), LogState.Info, LogLayer.UI);
                            break;
                        case "9999":
                            esbObj.ResultAlertMessage = string.Format("ESB電文處理異常，錯誤代碼：{0}，錯誤訊息：{1}，請重新操作", resultErrorCode, resultErrorMsg);
                            Logging.Log(String.Format("StatusCode：{0}；RspCode：{1}；ErrorCode：{2}；Message：{3}", esbObj.StatusCode, resultRspCode, resultErrorCode, resultErrorMsg), LogState.Error, LogLayer.UI);
                            break;
                        default:
                            esbObj.ResultAlertMessage = string.Format("ESB電文處理異常，錯誤代碼：{0}，錯誤訊息：{1}，請重新操作", resultErrorCode, resultErrorMsg);
                            Logging.Log(String.Format("StatusCode：{0}；RspCode：{1}；ErrorCode：{2}；Message：{3}", esbObj.StatusCode, resultRspCode, resultErrorCode, resultErrorMsg), LogState.Error, LogLayer.UI);
                            break;
                    }
                }
                else
                {
                    esbObj.ResultAlertMessage = string.Format("ESB電文發送失敗：{0}；錯誤代碼：{1}", resultErrorMsg, resultErrorCode);
                    Logging.Log(String.Format("StatusCode：{0}；RspCode：{1}；ErrorCode：{2}；Message：{3}", esbObj.StatusCode, resultRspCode, resultErrorCode, resultErrorMsg), LogState.Error, LogLayer.UI);
                }
                #endregion
            }
        }

        /// <summary>
        /// 紀錄ESB上下行電文
        /// </summary>
        /// <param name="strESBSerializ">ESB電文內容</param>
        /// <param name="strNameCheckType">REQ、RTN</param>
        public static void SaveESBLog(string strESBSerializ, string strNameCheckType)
        {
            string strMsg = string.Empty;
            // 添加日期
            strMsg = "\r\n[" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss:fff") + "] ---------------- " + strNameCheckType + " --------------------------\r\n";
            // 調整斷行符號避免斷行失效
            strMsg += strESBSerializ.Replace("\r\n", "$S").Replace("\n", "$S").Replace("$S", "\r\n");
            // 替換inputXML標籤符號
            strMsg = strMsg.Replace("&lt;", "<").Replace("&gt;", ">").Replace("><", ">\r\n<");
            Logging.Log(strMsg, "ESB", LogState.Info);
        }

        public static string getTagValue(string xmlSource, string tagName)
        {
            string tagValue = string.Empty;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlSource);
                if (doc.GetElementsByTagName(tagName).Count > 0)
                {
                    tagValue = doc.GetElementsByTagName(tagName)[0].InnerText;
                }
                return tagValue;
            }
            catch (Exception ex)
            {
                Logging.Log("無法取得代碼", LogState.Info, LogLayer.UI);
                return tagValue;
            }
        }
    }
}
