﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.42000
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.8.3928.0.
// 
namespace CSIPKeyInGUI.EntityLayer
{
    using System.Xml.Serialization;
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/BSMF/cpCstmrEmailUpdRcrdMemoAddR" +
        "q/01")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/BSMF/cpCstmrEmailUpdRcrdMemoAddR" +
        "q/01", IsNullable=false)]
    public partial class cpCstmrEmailUpdRcrdMemoAddRq {
        
        private cpCstmrEmailUpdRcrdMemoAddRqREQHDR rEQHDRField;
        
        private cpCstmrEmailUpdRcrdMemoAddRqREQBDY[] rEQBDYField;
        
        /// <remarks/>
        public cpCstmrEmailUpdRcrdMemoAddRqREQHDR REQHDR {
            get {
                return this.rEQHDRField;
            }
            set {
                this.rEQHDRField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("REQBDY")]
        public cpCstmrEmailUpdRcrdMemoAddRqREQBDY[] REQBDY {
            get {
                return this.rEQBDYField;
            }
            set {
                this.rEQBDYField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/BSMF/cpCstmrEmailUpdRcrdMemoAddR" +
        "q/01")]
    public partial class cpCstmrEmailUpdRcrdMemoAddRqREQHDR {
        
        private string trnNumField;
        
        private string trnCodeField;
        
        private string sourceIdField;
        
        private string userIDField;
        
        private string returnTypeField;
        
        private string reqPageNumField;
        
        private string reqPageRowSizeField;
        
        private string reqFileNotifyTypeField;
        
        /// <remarks/>
        public string TrnNum {
            get {
                return this.trnNumField;
            }
            set {
                this.trnNumField = value;
            }
        }
        
        /// <remarks/>
        public string TrnCode {
            get {
                return this.trnCodeField;
            }
            set {
                this.trnCodeField = value;
            }
        }
        
        /// <remarks/>
        public string SourceId {
            get {
                return this.sourceIdField;
            }
            set {
                this.sourceIdField = value;
            }
        }
        
        /// <remarks/>
        public string UserID {
            get {
                return this.userIDField;
            }
            set {
                this.userIDField = value;
            }
        }
        
        /// <remarks/>
        public string ReturnType {
            get {
                return this.returnTypeField;
            }
            set {
                this.returnTypeField = value;
            }
        }
        
        /// <remarks/>
        public string ReqPageNum {
            get {
                return this.reqPageNumField;
            }
            set {
                this.reqPageNumField = value;
            }
        }
        
        /// <remarks/>
        public string ReqPageRowSize {
            get {
                return this.reqPageRowSizeField;
            }
            set {
                this.reqPageRowSizeField = value;
            }
        }
        
        /// <remarks/>
        public string ReqFileNotifyType {
            get {
                return this.reqFileNotifyTypeField;
            }
            set {
                this.reqFileNotifyTypeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/BSMF/cpCstmrEmailUpdRcrdMemoAddR" +
        "q/01")]
    public partial class cpCstmrEmailUpdRcrdMemoAddRqREQBDY {
        
        private string txnSeqNoField;
        
        private string applyCustIdField;
        
        private string emailField;
        
        private string otpNoField;
        
        private string mobileNoField;
        
        private string checkTypeField;
        
        private string replyCheckReasonField;
        
        private string empIdField;
        
        private string branchNoField;
        
        private cpCstmrEmailUpdRcrdMemoAddRqREQBDYReplyCustCheckInfoList replyCustCheckInfoListField;
        
        /// <remarks/>
        public string TxnSeqNo {
            get {
                return this.txnSeqNoField;
            }
            set {
                this.txnSeqNoField = value;
            }
        }
        
        /// <remarks/>
        public string ApplyCustId {
            get {
                return this.applyCustIdField;
            }
            set {
                this.applyCustIdField = value;
            }
        }
        
        /// <remarks/>
        public string Email {
            get {
                return this.emailField;
            }
            set {
                this.emailField = value;
            }
        }
        
        /// <remarks/>
        public string OtpNo {
            get {
                return this.otpNoField;
            }
            set {
                this.otpNoField = value;
            }
        }
        
        /// <remarks/>
        public string MobileNo {
            get {
                return this.mobileNoField;
            }
            set {
                this.mobileNoField = value;
            }
        }
        
        /// <remarks/>
        public string CheckType {
            get {
                return this.checkTypeField;
            }
            set {
                this.checkTypeField = value;
            }
        }
        
        /// <remarks/>
        public string ReplyCheckReason {
            get {
                return this.replyCheckReasonField;
            }
            set {
                this.replyCheckReasonField = value;
            }
        }
        
        /// <remarks/>
        public string EmpId {
            get {
                return this.empIdField;
            }
            set {
                this.empIdField = value;
            }
        }
        
        /// <remarks/>
        public string BranchNo {
            get {
                return this.branchNoField;
            }
            set {
                this.branchNoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ReplyCustCheckInfoList")]
        public cpCstmrEmailUpdRcrdMemoAddRqREQBDYReplyCustCheckInfoList ReplyCustCheckInfoList {
            get {
                return this.replyCustCheckInfoListField;
            }
            set {
                this.replyCustCheckInfoListField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/BSMF/cpCstmrEmailUpdRcrdMemoAddR" +
        "q/01")]
    public partial class cpCstmrEmailUpdRcrdMemoAddRqREQBDYReplyCustCheckInfoList {
        
        private string processTypeField;
        
        private string checkTimeField;
        
        private string processTimeField;
        
        private string processStatusField;
        
        private string processChannelField;
        
        /// <remarks/>
        public string ProcessType {
            get {
                return this.processTypeField;
            }
            set {
                this.processTypeField = value;
            }
        }
        
        /// <remarks/>
        public string CheckTime {
            get {
                return this.checkTimeField;
            }
            set {
                this.checkTimeField = value;
            }
        }
        
        /// <remarks/>
        public string ProcessTime {
            get {
                return this.processTimeField;
            }
            set {
                this.processTimeField = value;
            }
        }
        
        /// <remarks/>
        public string ProcessStatus {
            get {
                return this.processStatusField;
            }
            set {
                this.processStatusField = value;
            }
        }
        
        /// <remarks/>
        public string ProcessChannel {
            get {
                return this.processChannelField;
            }
            set {
                this.processChannelField = value;
            }
        }
    }
}
