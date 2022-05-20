using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Collections.Generic;
using System.Text;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;

namespace CSIPNewInvoice.EntityLayer_new
{
    /// <summary>
    /// AML_File_Import
    /// </summary>
    [Serializable()]
    [AttributeTable("AML_HQ_Manager_Import")]
    public class Entity_AML_HQ_Manager_Import : Framework.Data.OM.Entity
    {
        private int _ID;
        /// < summary>
        /// ID
        /// < /summary>
        public static string M_ID = "ID";
        private string _FILENAME;
        /// < summary>
        /// FILENAME
        /// < /summary>
        public static string M_FILENAME = "FILENAME";
        private string _HCOP_KEY;
        /// < summary>
        /// HCOP_KEY
        /// < /summary>
        public static string M_HCOP_KEY = "HCOP_KEY";
        private string _HCOP_BENE_NATION;
        /// < summary>
        /// HCOP_BENE_NATION
        /// < /summary>
        public static string M_HCOP_BENE_NATION = "HCOP_BENE_NATION";
        private string _HCOP_BENE_NAME;
        /// < summary>
        /// HCOP_BENE_NAME
        /// < /summary>
        public static string M_HCOP_BENE_NAME = "HCOP_BENE_NAME";
        private string _HCOP_BENE_BIRTH_DATE;
        /// < summary>
        /// HCOP_BENE_BIRTH_DATE
        /// < /summary>
        public static string M_HCOP_BENE_BIRTH_DATE = "HCOP_BENE_BIRTH_DATE";
        private string _HCOP_BENE_ID;
        /// < summary>
        /// HCOP_BENE_ID
        /// < /summary>
        public static string M_HCOP_BENE_ID = "HCOP_BENE_ID";
        private string _HCOP_BENE_PASSPORT;
        /// < summary>
        /// HCOP_BENE_PASSPORT
        /// < /summary>
        public static string M_HCOP_BENE_PASSPORT = "HCOP_BENE_PASSPORT";
        private string _HCOP_BENE_PASSPORT_EXP;
        /// < summary>
        /// HCOP_BENE_PASSPORT_EXP
        /// < /summary>
        public static string M_HCOP_BENE_PASSPORT_EXP = "HCOP_BENE_PASSPORT_EXP";
        private string _HCOP_BENE_RESIDENT_NO;
        /// < summary>
        /// HCOP_BENE_RESIDENT_NO
        /// < /summary>
        public static string M_HCOP_BENE_RESIDENT_NO = "HCOP_BENE_RESIDENT_NO";
        private string _HCOP_BENE_RESIDENT_EXP;
        /// < summary>
        /// HCOP_BENE_RESIDENT_EXP
        /// < /summary>
        public static string M_HCOP_BENE_RESIDENT_EXP = "HCOP_BENE_RESIDENT_EXP";
        private string _HCOP_BENE_OTH_CERT;
        /// < summary>
        /// HCOP_BENE_OTH_CERT
        /// < /summary>
        public static string M_HCOP_BENE_OTH_CERT = "HCOP_BENE_OTH_CERT";
        private string _HCOP_BENE_OTH_CERT_EXP;
        /// < summary>
        /// HCOP_BENE_OTH_CERT_EXP
        /// < /summary>
        public static string M_HCOP_BENE_OTH_CERT_EXP = "HCOP_BENE_OTH_CERT_EXP";
        private string _HCOP_BENE_JOB_TYPE;
        /// < summary>
        /// HCOP_BENE_JOB_TYPE
        /// < /summary>
        public static string M_HCOP_BENE_JOB_TYPE = "HCOP_BENE_JOB_TYPE";
        private string _HCOP_BENE_JOB_TYPE_2;
        /// < summary>
        /// HCOP_BENE_JOB_TYPE_2
        /// < /summary>
        public static string M_HCOP_BENE_JOB_TYPE_2 = "HCOP_BENE_JOB_TYPE_2";
        private string _HCOP_BENE_JOB_TYPE_3;
        /// < summary>
        /// HCOP_BENE_JOB_TYPE_3
        /// < /summary>
        public static string M_HCOP_BENE_JOB_TYPE_3 = "HCOP_BENE_JOB_TYPE_3";
        private string _HCOP_BENE_JOB_TYPE_4;
        /// < summary>
        /// HCOP_BENE_JOB_TYPE_4
        /// < /summary>
        public static string M_HCOP_BENE_JOB_TYPE_4 = "HCOP_BENE_JOB_TYPE_4";
        private string _HCOP_BENE_JOB_TYPE_5;
        /// < summary>
        /// HCOP_BENE_JOB_TYPE_5
        /// < /summary>
        public static string M_HCOP_BENE_JOB_TYPE_5 = "HCOP_BENE_JOB_TYPE_5";
        private string _HCOP_BENE_RESERVED;
        /// < summary>
        /// HCOP_BENE_RESERVED
        /// < /summary>
        public static string M_HCOP_BENE_RESERVED = "HCOP_BENE_RESERVED";
        private DateTime _Create_Time;
        /// < summary>
        /// Create_Time
        /// < /summary>
        public static string M_Create_Time = "Create_Time";
        private string _Create_User;
        /// < summary>
        /// Create_User
        /// < /summary>
        public static string M_Create_User = "Create_User";
        /// < summary>
        /// ID
        /// </summary>
        [AttributeField("ID", "System.Int32", false, false, false, "Int32")]
        public int ID
        {
            get
            {
                return this.ID;
            }
            set
            {
                this._ID = value;
            }
        }
        /// < summary>
        /// FILENAME
        /// </summary>
        [AttributeField("FILENAME", "System.String", false, false, false, "String")]
        public string FILENAME
        {
            get
            {
                return this.FILENAME;
            }
            set
            {
                this._FILENAME = value;
            }
        }
        /// < summary>
        /// HCOP_KEY
        /// </summary>
        [AttributeField("HCOP_KEY", "System.String", false, false, false, "String")]
        public string HCOP_KEY
        {
            get
            {
                return this.HCOP_KEY;
            }
            set
            {
                this._HCOP_KEY = value;
            }
        }
        /// < summary>
        /// HCOP_BENE_NATION
        /// </summary>
        [AttributeField("HCOP_BENE_NATION", "System.String", false, false, false, "String")]
        public string HCOP_BENE_NATION
        {
            get
            {
                return this.HCOP_BENE_NATION;
            }
            set
            {
                this._HCOP_BENE_NATION = value;
            }
        }
        /// < summary>
        /// HCOP_BENE_NAME
        /// </summary>
        [AttributeField("HCOP_BENE_NAME", "System.String", false, false, false, "String")]
        public string HCOP_BENE_NAME
        {
            get
            {
                return this.HCOP_BENE_NAME;
            }
            set
            {
                this._HCOP_BENE_NAME = value;
            }
        }
        /// < summary>
        /// HCOP_BENE_BIRTH_DATE
        /// </summary>
        [AttributeField("HCOP_BENE_BIRTH_DATE", "System.String", false, false, false, "String")]
        public string HCOP_BENE_BIRTH_DATE
        {
            get
            {
                return this.HCOP_BENE_BIRTH_DATE;
            }
            set
            {
                this._HCOP_BENE_BIRTH_DATE = value;
            }
        }
        /// < summary>
        /// HCOP_BENE_ID
        /// </summary>
        [AttributeField("HCOP_BENE_ID", "System.String", false, false, false, "String")]
        public string HCOP_BENE_ID
        {
            get
            {
                return this.HCOP_BENE_ID;
            }
            set
            {
                this._HCOP_BENE_ID = value;
            }
        }
        /// < summary>
        /// HCOP_BENE_PASSPORT
        /// </summary>
        [AttributeField("HCOP_BENE_PASSPORT", "System.String", false, false, false, "String")]
        public string HCOP_BENE_PASSPORT
        {
            get
            {
                return this.HCOP_BENE_PASSPORT;
            }
            set
            {
                this._HCOP_BENE_PASSPORT = value;
            }
        }
        /// < summary>
        /// HCOP_BENE_PASSPORT_EXP
        /// </summary>
        [AttributeField("HCOP_BENE_PASSPORT_EXP", "System.String", false, false, false, "String")]
        public string HCOP_BENE_PASSPORT_EXP
        {
            get
            {
                return this.HCOP_BENE_PASSPORT_EXP;
            }
            set
            {
                this._HCOP_BENE_PASSPORT_EXP = value;
            }
        }
        /// < summary>
        /// HCOP_BENE_RESIDENT_NO
        /// </summary>
        [AttributeField("HCOP_BENE_RESIDENT_NO", "System.String", false, false, false, "String")]
        public string HCOP_BENE_RESIDENT_NO
        {
            get
            {
                return this.HCOP_BENE_RESIDENT_NO;
            }
            set
            {
                this._HCOP_BENE_RESIDENT_NO = value;
            }
        }
        /// < summary>
        /// HCOP_BENE_RESIDENT_EXP
        /// </summary>
        [AttributeField("HCOP_BENE_RESIDENT_EXP", "System.String", false, false, false, "String")]
        public string HCOP_BENE_RESIDENT_EXP
        {
            get
            {
                return this.HCOP_BENE_RESIDENT_EXP;
            }
            set
            {
                this._HCOP_BENE_RESIDENT_EXP = value;
            }
        }
        /// < summary>
        /// HCOP_BENE_OTH_CERT
        /// </summary>
        [AttributeField("HCOP_BENE_OTH_CERT", "System.String", false, false, false, "String")]
        public string HCOP_BENE_OTH_CERT
        {
            get
            {
                return this.HCOP_BENE_OTH_CERT;
            }
            set
            {
                this._HCOP_BENE_OTH_CERT = value;
            }
        }
        /// < summary>
        /// HCOP_BENE_OTH_CERT_EXP
        /// </summary>
        [AttributeField("HCOP_BENE_OTH_CERT_EXP", "System.String", false, false, false, "String")]
        public string HCOP_BENE_OTH_CERT_EXP
        {
            get
            {
                return this.HCOP_BENE_OTH_CERT_EXP;
            }
            set
            {
                this._HCOP_BENE_OTH_CERT_EXP = value;
            }
        }
        /// < summary>
        /// HCOP_BENE_JOB_TYPE
        /// </summary>
        [AttributeField("HCOP_BENE_JOB_TYPE", "System.String", false, false, false, "String")]
        public string HCOP_BENE_JOB_TYPE
        {
            get
            {
                return this.HCOP_BENE_JOB_TYPE;
            }
            set
            {
                this._HCOP_BENE_JOB_TYPE = value;
            }
        }
        /// < summary>
        /// HCOP_BENE_JOB_TYPE_2
        /// </summary>
        [AttributeField("HCOP_BENE_JOB_TYPE_2", "System.String", false, false, false, "String")]
        public string HCOP_BENE_JOB_TYPE_2
        {
            get
            {
                return this.HCOP_BENE_JOB_TYPE_2;
            }
            set
            {
                this._HCOP_BENE_JOB_TYPE_2 = value;
            }
        }
        /// < summary>
        /// HCOP_BENE_JOB_TYPE_3
        /// </summary>
        [AttributeField("HCOP_BENE_JOB_TYPE_3", "System.String", false, false, false, "String")]
        public string HCOP_BENE_JOB_TYPE_3
        {
            get
            {
                return this.HCOP_BENE_JOB_TYPE_3;
            }
            set
            {
                this._HCOP_BENE_JOB_TYPE_3 = value;
            }
        }
        /// < summary>
        /// HCOP_BENE_JOB_TYPE_4
        /// </summary>
        [AttributeField("HCOP_BENE_JOB_TYPE_4", "System.String", false, false, false, "String")]
        public string HCOP_BENE_JOB_TYPE_4
        {
            get
            {
                return this.HCOP_BENE_JOB_TYPE_4;
            }
            set
            {
                this._HCOP_BENE_JOB_TYPE_4 = value;
            }
        }
        /// < summary>
        /// HCOP_BENE_JOB_TYPE_5
        /// </summary>
        [AttributeField("HCOP_BENE_JOB_TYPE_5", "System.String", false, false, false, "String")]
        public string HCOP_BENE_JOB_TYPE_5
        {
            get
            {
                return this.HCOP_BENE_JOB_TYPE_5;
            }
            set
            {
                this._HCOP_BENE_JOB_TYPE_5 = value;
            }
        }
        /// < summary>
        /// HCOP_BENE_RESERVED
        /// </summary>
        [AttributeField("HCOP_BENE_RESERVED", "System.String", false, false, false, "String")]
        public string HCOP_BENE_RESERVED
        {
            get
            {
                return this.HCOP_BENE_RESERVED;
            }
            set
            {
                this._HCOP_BENE_RESERVED = value;
            }
        }
        /// < summary>
        /// Create_Time
        /// </summary>
        [AttributeField("Create_Time", "System.DateTime", false, false, false, "DateTime")]
        public DateTime Create_Time
        {
            get
            {
                return this.Create_Time;
            }
            set
            {
                this._Create_Time = value;
            }
        }
        /// < summary>
        /// Create_User
        /// </summary>
        [AttributeField("Create_User", "System.String", false, false, false, "String")]
        public string Create_User
        {
            get
            {
                return this.Create_User;
            }
            set
            {
                this._Create_User = value;
            }
        }
    }

    /// <summary>
    /// tbl_FileInfo
    /// </summary>
    [Serializable()]
    public class Entity_AML_HQ_Manager_ImportSet : EntitySet<Entity_AML_HQ_Manager_Import>
    {
    }
}
