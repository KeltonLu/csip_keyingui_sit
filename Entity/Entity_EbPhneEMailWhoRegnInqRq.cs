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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/BSMF/ebPhneEMailWhoRegnInqRq/01")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/BSMF/ebPhneEMailWhoRegnInqRq/01", IsNullable = false)]
    public partial class ebPhneEMailWhoRegnInqRq
    {

        private ebPhneEMailWhoRegnInqRqREQHDR rEQHDRField;

        private ebPhneEMailWhoRegnInqRqREQBDY rEQBDYField;

        /// <remarks/>
        public ebPhneEMailWhoRegnInqRqREQHDR REQHDR
        {
            get
            {
                return this.rEQHDRField;
            }
            set
            {
                this.rEQHDRField = value;
            }
        }

        /// <remarks/>
        public ebPhneEMailWhoRegnInqRqREQBDY REQBDY
        {
            get
            {
                return this.rEQBDYField;
            }
            set
            {
                this.rEQBDYField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/BSMF/ebPhneEMailWhoRegnInqRq/01")]
    public partial class ebPhneEMailWhoRegnInqRqREQHDR
    {

        private string trnNumField;

        private string sourceIdField;

        private string userIdField;

        private string rowCountField;

        private string rowSizeField;

        /// <remarks/>
        public string TrnNum
        {
            get
            {
                return this.trnNumField;
            }
            set
            {
                this.trnNumField = value;
            }
        }

        /// <remarks/>
        public string SourceId
        {
            get
            {
                return this.sourceIdField;
            }
            set
            {
                this.sourceIdField = value;
            }
        }

        /// <remarks/>
        public string UserId
        {
            get
            {
                return this.userIdField;
            }
            set
            {
                this.userIdField = value;
            }
        }

        /// <remarks/>
        public string RowCount
        {
            get
            {
                return this.rowCountField;
            }
            set
            {
                this.rowCountField = value;
            }
        }

        /// <remarks/>
        public string RowSize
        {
            get
            {
                return this.rowSizeField;
            }
            set
            {
                this.rowSizeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ns.chinatrust.com.tw/XSD/CTCB/ESB/Message/BSMF/ebPhneEMailWhoRegnInqRq/01")]
    public partial class ebPhneEMailWhoRegnInqRqREQBDY
    {

        private string mobileNoField;

        private string emailField;

        private string otpNoField;

        private string custIdField;

        private string emailAllStatusField;

        /// <remarks/>
        public string MobileNo
        {
            get
            {
                return this.mobileNoField;
            }
            set
            {
                this.mobileNoField = value;
            }
        }

        /// <remarks/>
        public string Email
        {
            get
            {
                return this.emailField;
            }
            set
            {
                this.emailField = value;
            }
        }

        /// <remarks/>
        public string OtpNo
        {
            get
            {
                return this.otpNoField;
            }
            set
            {
                this.otpNoField = value;
            }
        }

        /// <remarks/>
        public string CustId
        {
            get
            {
                return this.custIdField;
            }
            set
            {
                this.custIdField = value;
            }
        }

        /// <remarks/>
        public string EmailAllStatus
        {
            get
            {
                return this.emailAllStatusField;
            }
            set
            {
                this.emailAllStatusField = value;
            }
        }
    }
}

