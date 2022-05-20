using System;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;

namespace CSIPNewInvoice.EntityLayer_new
{
    /// <summary>
    /// AML_File_Import
    /// </summary>
    [Serializable()]
    [AttributeTable("AML_CASE_DATA")]
    public class Entity_AML_CASE_DATA : Entity
    {
        public Entity_AML_CASE_DATA()
        {
        }
        
        private int _ID;
        /// < summary>
        /// ID
        /// </summary>
        //[AttributeField("ID", "System.Int32", false, false, false, "Int32")]
        public int ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
            }
        }

        private string _CASE_NO;
        /// < summary>
        /// CASE_NO
        /// </summary>
        //[AttributeField("CASE_NO", "System.String", false, false, false, "String")]
        public string CASE_NO
        {
            get
            {
                return this._CASE_NO;
            }
            set
            {
                this._CASE_NO = value;
            }
        }

        private string _CUST_ID;
        /// < summary>
        /// CUST_ID
        /// </summary>
        //[AttributeField("CUST_ID", "System.String", false, false, false, "String")]
        public string CUST_ID
        {
            get
            {
                return this._CUST_ID;
            }
            set
            {
                this._CUST_ID = value;
            }
        }

        private string _CUST_NAME;
        /// < summary>
        /// CUST_NAME
        /// </summary>
        //[AttributeField("CUST_NAME", "System.String", false, false, false, "String")]
        public string CUST_NAME
        {
            get
            {
                return this._CUST_NAME;
            }
            set
            {
                this._CUST_NAME = value;
            }
        }

        private string _Source;
        /// < summary>
        /// Source
        /// </summary>
        //[AttributeField("Source", "System.String", false, false, false, "String")]
        public string Source
        {
            get
            {
                return this._Source;
            }
            set
            {
                this._Source = value;
            }
        }

        private string _RISK_RANKING;
        /// < summary>
        /// RISK_RANKING
        /// </summary>
        //[AttributeField("RISK_RANKING", "System.String", false, false, false, "String")]
        public string RISK_RANKING
        {
            get
            {
                return this._RISK_RANKING;
            }
            set
            {
                this._RISK_RANKING = value;
            }
        }

        private string _CASE_TYPE;
        /// < summary>
        /// CASE_TYPE
        /// </summary>
        //[AttributeField("CASE_TYPE", "System.String", false, false, false, "String")]
        public string CASE_TYPE
        {
            get
            {
                return this._CASE_TYPE;
            }
            set
            {
                this._CASE_TYPE = value;
            }
        }

        private string _STATUS;
        /// < summary>
        /// STATUS
        /// </summary>
        //[AttributeField("STATUS", "System.String", false, false, false, "String")]
        public string STATUS
        {
            get
            {
                return this._STATUS;
            }
            set
            {
                this._STATUS = value;
            }
        }

        private string _Create_YM;
        /// < summary>
        /// Create_YM
        /// </summary>
        //[AttributeField("Create_YM", "System.String", false, false, false, "String")]
        public string Create_YM
        {
            get
            {
                return this._Create_YM;
            }
            set
            {
                this._Create_YM = value;
            }
        }

        private string _Create_Date;
        /// < summary>
        /// Create_Date
        /// </summary>
        //[AttributeField("Create_Date", "System.String", false, false, false, "String")]
        public string Create_Date
        {
            get
            {
                return this._Create_Date;
            }
            set
            {
                this._Create_Date = value;
            }
        }

        private string _Due_Date;
        /// < summary>
        /// Due_Date
        /// </summary>
        //[AttributeField("Due_Date", "System.String", false, false, false, "String")]
        public string Due_Date
        {
            get
            {
                return this._Due_Date;
            }
            set
            {
                this._Due_Date = value;
            }
        }

        private DateTime _CTI_Send_Date;
        /// < summary>
        /// CTI_Send_Date
        /// </summary>
        //[AttributeField("CTI_Send_Date", "System.DateTime", false, false, false, "DateTime")]
        public DateTime CTI_Send_Date
        {
            get
            {
                return this._CTI_Send_Date;
            }
            set
            {
                this._CTI_Send_Date = value;
            }
        }

        private DateTime _CTI_Return_Date;
        /// < summary>
        /// CTI_Return_Date
        /// </summary>
        //[AttributeField("CTI_Return_Date", "System.DateTime", false, false, false, "DateTime")]
        public DateTime CTI_Return_Date
        {
            get
            {
                return this._CTI_Return_Date;
            }
            set
            {
                this._CTI_Return_Date = value;
            }
        }

        private DateTime _Pockii_Send_Date;
        /// < summary>
        /// Pockii_Send_Date
        /// </summary>
        //[AttributeField("Pockii_Send_Date", "System.DateTime", false, false, false, "DateTime")]
        public DateTime Pockii_Send_Date
        {
            get
            {
                return this._Pockii_Send_Date;
            }
            set
            {
                this._Pockii_Send_Date = value;
            }
        }

        private DateTime _Pockii_Return_Date;
        /// < summary>
        /// Pockii_Return_Date
        /// </summary>
        //[AttributeField("Pockii_Return_Date", "System.DateTime", false, false, false, "DateTime")]
        public DateTime Pockii_Return_Date
        {
            get
            {
                return this._Pockii_Return_Date;
            }
            set
            {
                this._Pockii_Return_Date = value;
            }
        }

        private string _RMM_UserID;
        /// < summary>
        /// RMM_UserID
        /// </summary>
        //[AttributeField("RMM_UserID", "System.String", false, false, false, "String")]
        public string RMM_UserID
        {
            get
            {
                return this._RMM_UserID;
            }
            set
            {
                this._RMM_UserID = value;
            }
        }

        private DateTime _Last_Date;
        /// < summary>
        /// Last_Date
        /// </summary>
        //[AttributeField("Last_Date", "System.DateTime", false, false, false, "DateTime")]
        public DateTime Last_Date
        {
            get
            {
                return this._Last_Date;
            }
            set
            {
                this._Last_Date = value;
            }
        }

        private DateTime _Trans_Date;
        /// < summary>
        /// Trans_Date
        /// </summary>
        //[AttributeField("Trans_Date", "System.DateTime", false, false, false, "DateTime")]
        public DateTime Trans_Date
        {
            get
            {
                return this._Trans_Date;
            }
            set
            {
                this._Trans_Date = value;
            }
        }

        private string _Assign_UserID;
        /// < summary>
        /// Assign_UserID
        /// </summary>
        //[AttributeField("Assign_UserID", "System.String", false, false, false, "String")]
        public string Assign_UserID
        {
            get
            {
                return this._Assign_UserID;
            }
            set
            {
                this._Assign_UserID = value;
            }
        }

        private string _Incorporated;
        /// < summary>
        /// Incorporated
        /// </summary>
        //[AttributeField("Incorporated", "System.String", false, false, false, "String")]
        public string Incorporated
        {
            get
            {
                return this._Incorporated;
            }
            set
            {
                this._Incorporated = value;
            }
        }

        private DateTime _Incorporated_Date;
        /// < summary>
        /// Incorporated_Date
        /// </summary>
        //[AttributeField("Incorporated_Date", "System.DateTime", false, false, false, "DateTime")]
        public DateTime Incorporated_Date
        {
            get
            {
                return this._Incorporated_Date;
            }
            set
            {
                this._Incorporated_Date = value;
            }
        }

        private string _Pockii_Closed;
        /// < summary>
        /// Pockii_Closed
        /// </summary>
        //[AttributeField("Pockii_Closed", "System.String", false, false, false, "String")]
        public string Pockii_Closed
        {
            get
            {
                return this._Pockii_Closed;
            }
            set
            {
                this._Pockii_Closed = value;
            }
        }

        private DateTime _Close_Date;
        /// < summary>
        /// Close_Date
        /// </summary>
        //[AttributeField("Close_Date", "System.DateTime", false, false, false, "DateTime")]
        public DateTime Close_Date
        {
            get
            {
                return this._Close_Date;
            }
            set
            {
                this._Close_Date = value;
            }
        }
    }

    ///// <summary>
    ///// tbl_FileInfo
    ///// </summary>
    //[Serializable()]
    //public class Entity_AML_CASE_DATASet : EntitySet<Entity_AML_CASE_DATA>
    //{
    //}
}
