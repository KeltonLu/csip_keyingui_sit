using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using Framework.Common.Logging;
using CSIPKeyInGUI.EntityLayer;
using System.Data;
using System.Data.SqlClient;
using Framework.Common.Utility;

/// <summary>
/// ESBCustCheckInfo 的摘要描述
/// </summary>
namespace CSIPKeyInGUI.BusinessRules
{
    public class BRESBCustCheckInfo : ESBObject
    {
        public ebPhneEMailWhoRegnInqRqREQHDR REQHDR { get; set; }
        public ebPhneEMailWhoRegnInqRqREQBDY REQBDY { get; set; }
        public string SameFlag { get; set; }
        public string TransactionID { get; set; }

        private string StatusCode_Tag = "ns1:StatusCode";
        private string RspCode_Tag = "ns2:RspCode";
        private string RspMsg_Tag = "ns2:RspMsg";
        private string ErrorCode_Tag = "ns2:ErrorCode";
        private string ErrorMessage_Tag = "ns2:ErrorMessage";
        private string TotalRowExId_Tag = "ns2:TotalRowExId";

        /// 作者 Ares Stanley
        /// 創建日期：2021/10/19
        /// 修改日期：2021/10/19
        /// <summary>
        /// 取得ESB上行電文
        /// </summary>
        /// <returns></returns>
        public override string getXML()
        {
            string strXml = string.Empty;
            string strXML = string.Empty;
            DateTime now = DateTime.Now;
            string rqTimeStamp = now.ToString("yyyy-MM-dd") + "T" + now.ToString("HH:mm:ss.ff") + "+08:00";

            REQHDR.SourceId = "CSIP";
            REQHDR.RowCount = "1";
            REQHDR.RowSize = "1";

            const string strHerader1 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            const string strHerader2 = "<ns0:ServiceEnvelope xmlns:ns0=\"http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/EMF/ServiceEnvelope\">";
            const string strHerader3 = "<ns1:ServiceHeader xmlns:ns1=\"http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/EMF/ServiceHeader\">";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(strHerader1);
            sb.AppendLine(strHerader2);
            sb.AppendLine(strHerader3);
            sb.AppendLine("<ns1:StandardType>BSMF</ns1:StandardType>");
            sb.AppendLine("<ns1:StandardVersion>01</ns1:StandardVersion>");
            sb.AppendLine("<ns1:ServiceName>ebPhneEMailWhoRegnInq</ns1:ServiceName>");
            sb.AppendLine("<ns1:ServiceVersion>01</ns1:ServiceVersion>");
            sb.AppendLine("<ns1:SourceID>CSIP</ns1:SourceID>");
            sb.AppendLine("<ns1:TransactionID>" + TransactionID + "</ns1:TransactionID>");
            sb.AppendLine("<ns1:RqTimestamp>" + rqTimeStamp + "</ns1:RqTimestamp>");
            sb.AppendLine("</ns1:ServiceHeader>");
            sb.AppendLine("<ns1:ServiceBody xmlns:ns1=\"http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/EMF/ServiceBody\">");
            sb.AppendLine("<ns2:ebPhneEMailWhoRegnInqRq xmlns:ns2=\"http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/BSMF/ebPhneEMailWhoRegnInqRq/01\" > ");
            sb.AppendLine("<ns2:REQHDR>");
            sb.AppendLine("<ns2:TrnNum>" + REQHDR.TrnNum + "</ns2:TrnNum>");
            sb.AppendLine("<ns2:SourceID>" + REQHDR.SourceId + "</ns2:SourceID>");
            sb.AppendLine("<ns2:UserId>" + REQHDR.UserId + "</ns2:UserId>");
            sb.AppendLine("<ns2:RowCount>" + REQHDR.RowCount + "</ns2:RowCount>");
            sb.AppendLine("<ns2:RowSize>" + REQHDR.RowSize + "</ns2:RowSize>");
            sb.AppendLine("</ns2:REQHDR><ns2:REQBDY>");
            sb.AppendLine("<ns2:MobileNo>" + REQBDY.MobileNo + "</ns2:MobileNo>");
            sb.AppendLine("<ns2:Email>" + REQBDY.Email + "</ns2:Email>");
            sb.AppendLine("<ns2:OtpNo>" + REQBDY.OtpNo + "</ns2:OtpNo>");
            sb.AppendLine("<ns2:CustId>" + REQBDY.CustId + "</ns2:CustId>");
            sb.AppendLine("<ns2:EmailAllStatus>" + REQBDY.EmailAllStatus + "</ns2:EmailAllStatus>");
            sb.AppendLine("</ns2:REQBDY>");
            sb.AppendLine("</ns2:ebPhneEMailWhoRegnInqRq>");
            sb.AppendLine("</ns1:ServiceBody>");
            sb.AppendLine("</ns0:ServiceEnvelope>");
            strXML = sb.ToString();
            return strXML;
        }

        /// 作者 Ares Stanley
        /// 創建日期：2021/10/19
        /// 修改日期：2021/10/19
        /// <summary>
        /// 透過XML tag 取得資料
        /// </summary>
        /// <param name="strResult"></param>
        public override void getResult(string strResult)
        {
            //取得排除本人ID筆數，0非多人共用，>0多人共用
            int totalRowExId;
            bool success = Int32.TryParse(getTagValue(strResult, TotalRowExId_Tag), out totalRowExId);
            if (success)
            {
                SameFlag = totalRowExId > 0 ? "T" : "F";
            }
            else
            {
                SameFlag = "F";
            }

            //取得StatusCode
            StatusCode = getTagValue(strResult, StatusCode_Tag);

            //取得RspCode
            RspCode = getTagValue(strResult, RspCode_Tag);
            if (RspCode == "C001")
                SameFlag = "F";

            //取得RspMsg
            RspMsg = getTagValue(strResult, RspMsg_Tag);

            //取得ErrorCode
            ErrorCode = getTagValue(strResult, ErrorCode_Tag);

            //取得錯誤訊息
            ErrorMessage = getTagValue(strResult, ErrorMessage_Tag);
        }

        /// 作者 Ares Stanley
        /// 創建日期：2021/10/19
        /// 修改日期：2021/10/19
        /// <summary>
        /// 字串左補0至指定長度
        /// </summary>
        /// <param name="oriString">原始字串</param>
        /// <param name="totalLength">預期字串長度</param>
        /// <returns></returns>
        public static string InsertZeroToLeft(string oriString, Int32 totalLength = 8)
        {
            string result = string.Empty;
            string insertString = new string('0', totalLength - oriString.Length);
            result = insertString + oriString;
            return result;
        }
    }
}
