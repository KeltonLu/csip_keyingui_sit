//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:2.0.50727.42
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;


namespace CSIPNewInvoice.EntityLayer_new
{
    /// <summary>
    /// AML_HeadOffice
    /// </summary>
    [Serializable()]
    [AttributeTable("SCDDReport")]
    public class EntitySCDDReport : Entity
    {
        public EntitySCDDReport()
        {
        }
        private string _SR_CASE_NO;
        /// <summary>
        /// SR_CASE_NO
        /// </summary>
        [AttributeRfPage("txtSR_CASE_NO", "CustTextBox", false)]
        public string SR_CASE_NO
        {
            get
            {
                return this._SR_CASE_NO;
            }
            set
            {
                this._SR_CASE_NO = value;
            }
        }
        private string _SR_DateTime;
        /// <summary>
        /// SR_DateTime
        /// </summary>
        [AttributeRfPage("lblSR_DateTime", "CustLabel", false)]
        public string SR_DateTime
        {
            get
            {
                return this._SR_DateTime;
            }
            set
            {
                this._SR_DateTime = value;
            }
        }
        private string _SR_User;
        /// <summary>
        /// SR_User
        /// </summary>
        [AttributeRfPage("txtSR_User", "CustTextBox", false)]
        public string SR_User
        {
            get
            {
                return this._SR_User;
            }
            set
            {
                this._SR_User = value;
            }
        }
        private string _SR_RiskLevel;
        /// <summary>
        /// SR_RiskLevel
        /// </summary>
        [AttributeRfPage("lblSR_RiskLevel", "CustLabel", false)]
        public string SR_RiskLevel
        {
            get
            {
                return this._SR_RiskLevel;
            }
            set
            {
                this._SR_RiskLevel = value;
            }
        }
        private string _SR_RiskItem;
        /// <summary>
        /// SR_RiskItem
        /// </summary>
        [AttributeRfPage("txtSR_RiskItem", "CustTextBox", false)]
        public string SR_RiskItem
        {
            get
            {
                return this._SR_RiskItem;
            }
            set
            {
                this._SR_RiskItem = value;
            }
        }
        private string _SR_RiskNote;
        /// <summary>
        /// SR_RiskNote
        /// </summary>
        [AttributeRfPage("txtSR_RiskNote", "CustTextBox", false)]
        public string SR_RiskNote
        {
            get
            {
                return this._SR_RiskNote;
            }
            set
            {
                this._SR_RiskNote = value;
            }
        }
        private string _SR_Explanation;
        /// <summary>
        /// SR_Explanation
        /// </summary>
        [AttributeRfPage("txtSR_Explanation", "CustTextBox", false)]
        public string SR_Explanation
        {
            get
            {
                return this._SR_Explanation;
            }
            set
            {
                this._SR_Explanation = value;
            }
        }

    }
}