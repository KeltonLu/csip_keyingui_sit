using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CSIPKeyInGUI.EntityLayer;
using System.Text;

/// <summary>
/// ESBReplyCustCheckInfoClass 的摘要描述
/// </summary>
namespace CSIPKeyInGUI.BusinessRules
{
    public class BRESBReplyCustCheckInfo : ESBObject
    {
        public cpCstmrEmailUpdRcrdMemoAddRqREQHDR REQHDR { get; set; }
        public cpCstmrEmailUpdRcrdMemoAddRqREQBDY REQBDY { get; set; }

        public string TransactionID { get; set; }

        private string StatusCode_Tag = "ns1:StatusCode";
        private string RspCode_Tag = "ns2:RspCode";
        private string RspMsg_Tag = "ns2:RspMsg";
        private string ErrorCode_Tag = "ns2:ErrorCode";
        private string ErrorMessage_Tag = "ns2:ErrorMessage";


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
            REQHDR.ReturnType = "01";//分頁資料
            REQHDR.ReqPageNum = "1";
            REQHDR.ReqPageRowSize = "1";
            REQHDR.ReqFileNotifyType = "01";

            REQBDY.ReplyCustCheckInfoList.ProcessType = "r";
            REQBDY.ReplyCustCheckInfoList.ProcessTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            REQBDY.ReplyCustCheckInfoList.ProcessChannel = "CSIP";
            REQBDY.ReplyCustCheckInfoList.ProcessStatus = "";

            const string strHerader1 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            const string strHerader2 = "<ns0:ServiceEnvelope xmlns:ns0=\"http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/EMF/ServiceEnvelope\">";
            const string strHerader3 = "<ns1:ServiceHeader xmlns:ns1=\"http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/EMF/ServiceHeader\">";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(strHerader1);
            sb.AppendLine(strHerader2);
            sb.AppendLine(strHerader3);
            sb.AppendLine("<ns1:StandardType>BSMF</ns1:StandardType>");
            sb.AppendLine("<ns1:StandardVersion>01</ns1:StandardVersion>");
            sb.AppendLine("<ns1:ServiceName>cpCstmrEmailUpdRcrdMemoAdd</ns1:ServiceName>");
            sb.AppendLine("<ns1:ServiceVersion>01</ns1:ServiceVersion>");
            sb.AppendLine("<ns1:SourceID>CSIP</ns1:SourceID>");
            sb.AppendLine("<ns1:TransactionID>" + TransactionID + "</ns1:TransactionID>");
            sb.AppendLine("<ns1:RqTimestamp>" + rqTimeStamp + "</ns1:RqTimestamp>");
            sb.AppendLine("</ns1:ServiceHeader>");
            sb.AppendLine("<ns1:ServiceBody xmlns:ns1=\"http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/EMF/ServiceBody\">");
            sb.AppendLine("<ns2:cpCstmrEmailUpdRcrdMemoAddRq xmlns:ns2=\"http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/BSMF/cpCstmrEmailUpdRcrdMemoAddRq/01\" > ");
            sb.AppendLine("<ns2:REQHDR>");
            sb.AppendLine("<ns2:TrnNum>" + REQHDR.TrnNum + "</ns2:TrnNum>");
            sb.AppendLine("<ns2:TrnCode>" + REQHDR.TrnCode + "</ns2:TrnCode>");
            sb.AppendLine("<ns2:SourceID>" + REQHDR.SourceId + "</ns2:SourceID>");
            sb.AppendLine("<ns2:UserID>" + REQHDR.UserID + "</ns2:UserID>");
            sb.AppendLine("<ns2:ReturnType>" + REQHDR.ReturnType + "</ns2:ReturnType>");
            sb.AppendLine("<ns2:ReqPageNum>" + REQHDR.ReqPageNum + "</ns2:ReqPageNum>");
            sb.AppendLine("<ns2:ReqPageRowSize>" + REQHDR.ReqPageRowSize + "</ns2:ReqPageRowSize>");
            sb.AppendLine("<ns2:ReqFileNotifyType>" + REQHDR.ReqFileNotifyType + "</ns2:ReqFileNotifyType>");
            sb.AppendLine("</ns2:REQHDR><ns2:REQBDY>");
            sb.AppendLine("<ns2:TxnSeqNo>" + REQBDY.TxnSeqNo + "</ns2:TxnSeqNo>");
            sb.AppendLine("<ns2:ApplyCustId>" + REQBDY.ApplyCustId + "</ns2:ApplyCustId>");
            sb.AppendLine("<ns2:Email>" + REQBDY.Email + "</ns2:Email>");
            sb.AppendLine("<ns2:OtpNo>" + REQBDY.OtpNo + "</ns2:OtpNo>");
            sb.AppendLine("<ns2:MobileNo>" + REQBDY.MobileNo + "</ns2:MobileNo>");
            sb.AppendLine("<ns2:CheckType>" + REQBDY.CheckType + "</ns2:CheckType>");
            sb.AppendLine("<ns2:ReplyCheckReason>" + REQBDY.ReplyCheckReason + "</ns2:ReplyCheckReason>");
            sb.AppendLine("<ns2:EmpId>" + REQBDY.EmpId + "</ns2:EmpId>");
            sb.AppendLine("<ns2:BranchNo>" + REQBDY.BranchNo + "</ns2:BranchNo>");
            sb.AppendLine("<ns2:ReplyCustCheckInfoList>");
            sb.AppendLine("<ns2:ProcessType>" + REQBDY.ReplyCustCheckInfoList.ProcessType + "</ns2:ProcessType>");
            sb.AppendLine("<ns2:CheckTime>" + REQBDY.ReplyCustCheckInfoList.CheckTime + "</ns2:CheckTime>");
            sb.AppendLine("<ns2:ProcessTime>" + REQBDY.ReplyCustCheckInfoList.ProcessTime + "</ns2:ProcessTime>");
            sb.AppendLine("<ns2:ProcessStatus>" + REQBDY.ReplyCustCheckInfoList.ProcessStatus + "</ns2:ProcessStatus>");
            sb.AppendLine("<ns2:ProcessChannel>" + REQBDY.ReplyCustCheckInfoList.ProcessChannel + "</ns2:ProcessChannel>");
            sb.AppendLine("</ns2:ReplyCustCheckInfoList>");
            sb.AppendLine("</ns2:REQBDY>");
            sb.AppendLine("</ns2:cpCstmrEmailUpdRcrdMemoAddRq>");
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
            //取得StatusCode
            StatusCode = getTagValue(strResult, StatusCode_Tag);

            //取得RspCode
            RspCode = getTagValue(strResult, RspCode_Tag);
            
            //取得RspMsg
            RspMsg = getTagValue(strResult, RspMsg_Tag);
            
            //取得ErrorCode
            ErrorCode = getTagValue(strResult, ErrorCode_Tag);
            
            //取得錯誤訊息
            ErrorMessage = getTagValue(strResult, ErrorMessage_Tag);
        }
    }
}
