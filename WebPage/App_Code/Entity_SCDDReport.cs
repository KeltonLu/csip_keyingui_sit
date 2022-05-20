using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;


namespace CSIPNewInvoice.EntityLayer_new
{
    
    /// <summary>
    /// SCDDReport
    /// </summary>
    [Serializable()]
    [AttributeTable("SCDDReport")]
    public class Entity_SCDDReport : Framework.Data.OM.Entity
    {
        ///// <summary>
        ///// FileId
        ///// </summary>
        //public static string M_FileId = "FileId";
        
        public static string M_SR_CASE_NO = "SR_CASE_NO";

        private string _SR_CASE_NO;
        private DateTime _SR_DateTime;
        private string _SR_User;
        private string _SR_RiskLevel;
        private string _SR_RiskItem;
        private string _SR_RiskNote;
        private string _SR_Explanation;
        private string _SR_EDD_Status;
        private string _SR_EDD_Date;

        /// <summary>
        /// NL_CASE_NO
        /// </summary>
        [AttributeField("SR_CASE_NO", "System.String", false, false, false, "String")]
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

        /// <summary>
        /// SR_DateTime
        /// </summary>
        [AttributeField("SR_DateTime", "System.DateTime", false, false, false, "DateTime")]
        public DateTime SR_DateTime
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

        /// <summary>
        /// SR_User
        /// </summary>
        [AttributeField("SR_User", "System.String", false, false, false, "String")]
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

        /// <summary>
        /// SR_RiskLevel
        /// </summary>
        [AttributeField("SR_RiskLevel", "System.String", false, false, false, "String")]
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

        /// <summary>
        /// SR_RiskItem
        /// </summary>
        [AttributeField("SR_RiskItem", "System.String", false, false, false, "String")]
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

        /// <summary>
        /// SR_RiskNote
        /// </summary>
        [AttributeField("SR_RiskNote", "System.String", false, false, false, "String")]
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

        /// <summary>
        /// SR_Explanation
        /// </summary>
        [AttributeField("SR_Explanation", "System.String", false, false, false, "String")]
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

        /// <summary>
        /// SR_EDD_Status
        /// </summary>
        [AttributeField("SR_EDD_Status", "System.String", false, false, false, "String")]
        public string SR_EDD_Status
        {
            get
            {
                return this._SR_EDD_Status;
            }
            set
            {
                this._SR_EDD_Status = value;
            }
        }

        /// <summary>
        /// SR_EDD_Date
        /// </summary>
        [AttributeField("SR_EDD_Date", "System.String", false, false, false, "String")]
        public string SR_EDD_Date
        {
            get
            {
                return this._SR_EDD_Date;
            }
            set
            {
                this._SR_EDD_Date = value;
            }
        }
    }

    /// <summary>
    /// SCDDReport
    /// </summary>
    [Serializable()]
    public class Entity_SCDDReportSet : EntitySet<Entity_SCDDReport>
    {
    }
}