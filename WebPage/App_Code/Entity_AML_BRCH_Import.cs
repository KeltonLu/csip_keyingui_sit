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
    [AttributeTable("AML_BRCH_Import")]
    public class Entity_AML_BRCH_Import : Framework.Data.OM.Entity
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
        private string _BRCH_KEY;
        /// < summary>
        /// BRCH_KEY
        /// < /summary>
        public static string M_BRCH_KEY = "BRCH_KEY";
        private string _BRCH_BRCH_NO;
        /// < summary>
        /// BRCH_BRCH_NO
        /// < /summary>
        public static string M_BRCH_BRCH_NO = "BRCH_BRCH_NO";
        private string _BRCH_BRCH_SEQ;
        /// < summary>
        /// BRCH_BRCH_SEQ
        /// < /summary>
        public static string M_BRCH_BRCH_SEQ = "BRCH_BRCH_SEQ";
        private string _BRCH_BRCH_TYPE;
        /// < summary>
        /// BRCH_BRCH_TYPE
        /// < /summary>
        public static string M_BRCH_BRCH_TYPE = "BRCH_BRCH_TYPE";
        private string _BRCH_NATION;
        /// < summary>
        /// BRCH_NATION
        /// < /summary>
        public static string M_BRCH_NATION = "BRCH_NATION";
        private string _BRCH_BIRTH_DATE;
        /// < summary>
        /// BRCH_BIRTH_DATE
        /// < /summary>
        public static string M_BRCH_BIRTH_DATE = "BRCH_BIRTH_DATE";
        private string _BRCH_PERM_CITY;
        /// < summary>
        /// BRCH_PERM_CITY
        /// < /summary>
        public static string M_BRCH_PERM_CITY = "BRCH_PERM_CITY";
        private string _BRCH_PERM_ADDR1;
        /// < summary>
        /// BRCH_PERM_ADDR1
        /// < /summary>
        public static string M_BRCH_PERM_ADDR1 = "BRCH_PERM_ADDR1";
        private string _BRCH_PERM_ADDR2;
        /// < summary>
        /// BRCH_PERM_ADDR2
        /// < /summary>
        public static string M_BRCH_PERM_ADDR2 = "BRCH_PERM_ADDR2";
        private string _BRCH_CHINESE_NAME;
        /// < summary>
        /// BRCH_CHINESE_NAME
        /// < /summary>
        public static string M_BRCH_CHINESE_NAME = "BRCH_CHINESE_NAME";
        private string _BRCH_ENGLISH_NAME;
        /// < summary>
        /// BRCH_ENGLISH_NAME
        /// < /summary>
        public static string M_BRCH_ENGLISH_NAME = "BRCH_ENGLISH_NAME";
        private string _BRCH_ID;
        /// < summary>
        /// BRCH_ID
        /// < /summary>
        public static string M_BRCH_ID = "BRCH_ID";
        private string _BRCH_OWNER_ID_ISSUE_DATE;
        /// < summary>
        /// BRCH_OWNER_ID_ISSUE_DATE
        /// < /summary>
        public static string M_BRCH_OWNER_ID_ISSUE_DATE = "BRCH_OWNER_ID_ISSUE_DATE";
        private string _BRCH_OWNER_ID_ISSUE_PLACE;
        /// < summary>
        /// BRCH_OWNER_ID_ISSUE_PLACE
        /// < /summary>
        public static string M_BRCH_OWNER_ID_ISSUE_PLACE = "BRCH_OWNER_ID_ISSUE_PLACE";
        private string _BRCH_OWNER_ID_REPLACE_TYPE;
        /// < summary>
        /// BRCH_OWNER_ID_REPLACE_TYPE
        /// < /summary>
        public static string M_BRCH_OWNER_ID_REPLACE_TYPE = "BRCH_OWNER_ID_REPLACE_TYPE";
        private string _BRCH_ID_PHOTO_FLAG;
        /// < summary>
        /// BRCH_ID_PHOTO_FLAG
        /// < /summary>
        public static string M_BRCH_ID_PHOTO_FLAG = "BRCH_ID_PHOTO_FLAG";
        private string _BRCH_PASSPORT;
        /// < summary>
        /// BRCH_PASSPORT
        /// < /summary>
        public static string M_BRCH_PASSPORT = "BRCH_PASSPORT";
        private string _BRCH_PASSPORT_EXP_DATE;
        /// < summary>
        /// BRCH_PASSPORT_EXP_DATE
        /// < /summary>
        public static string M_BRCH_PASSPORT_EXP_DATE = "BRCH_PASSPORT_EXP_DATE";
        private string _BRCH_RESIDENT_NO;
        /// < summary>
        /// BRCH_RESIDENT_NO
        /// < /summary>
        public static string M_BRCH_RESIDENT_NO = "BRCH_RESIDENT_NO";
        private string _BRCH_RESIDENT_EXP_DATE;
        /// < summary>
        /// BRCH_RESIDENT_EXP_DATE
        /// < /summary>
        public static string M_BRCH_RESIDENT_EXP_DATE = "BRCH_RESIDENT_EXP_DATE";
        private string _BRCH_OTHER_CERT;
        /// < summary>
        /// BRCH_OTHER_CERT
        /// < /summary>
        public static string M_BRCH_OTHER_CERT = "BRCH_OTHER_CERT";
        private string _BRCH_OTHER_CERT_EXP_DATE;
        /// < summary>
        /// BRCH_OTHER_CERT_EXP_DATE
        /// < /summary>
        public static string M_BRCH_OTHER_CERT_EXP_DATE = "BRCH_OTHER_CERT_EXP_DATE";
        private string _BRCH_COMP_TEL;
        /// < summary>
        /// BRCH_COMP_TEL
        /// < /summary>
        public static string M_BRCH_COMP_TEL = "BRCH_COMP_TEL";
        private string _BRCH_CREATE_DATE;
        /// < summary>
        /// BRCH_CREATE_DATE
        /// < /summary>
        public static string M_BRCH_CREATE_DATE = "BRCH_CREATE_DATE";
        private string _BRCH_STATUS;
        /// < summary>
        /// BRCH_STATUS
        /// < /summary>
        public static string M_BRCH_STATUS = "BRCH_STATUS";
        private string _BRCH_CIRCULATE_MERCH;
        /// < summary>
        /// BRCH_CIRCULATE_MERCH
        /// < /summary>
        public static string M_BRCH_CIRCULATE_MERCH = "BRCH_CIRCULATE_MERCH";
        private string _BRCH_HQ_BRCH_NO;
        /// < summary>
        /// BRCH_HQ_BRCH_NO
        /// < /summary>
        public static string M_BRCH_HQ_BRCH_NO = "BRCH_HQ_BRCH_NO";
        private string _BRCH_HQ_BRCH_SEQ_NO;
        /// < summary>
        /// BRCH_HQ_BRCH_SEQ_NO
        /// < /summary>
        public static string M_BRCH_HQ_BRCH_SEQ_NO = "BRCH_HQ_BRCH_SEQ_NO";
        private string _BRCH_UPDATE_DATE;
        /// < summary>
        /// BRCH_UPDATE_DATE
        /// < /summary>
        public static string M_BRCH_UPDATE_DATE = "BRCH_UPDATE_DATE";
        private string _BRCH_QUALIFY_FLAG;
        /// < summary>
        /// BRCH_QUALIFY_FLAG
        /// < /summary>
        public static string M_BRCH_QUALIFY_FLAG = "BRCH_QUALIFY_FLAG";
        private string _BRCH_RESERVED_FILLER;
        /// < summary>
        /// BRCH_RESERVED_FILLER
        /// < /summary>
        public static string M_BRCH_RESERVED_FILLER = "BRCH_RESERVED_FILLER";
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
        /// BRCH_KEY
        /// </summary>
        [AttributeField("BRCH_KEY", "System.String", false, false, false, "String")]
        public string BRCH_KEY
        {
            get
            {
                return this.BRCH_KEY;
            }
            set
            {
                this._BRCH_KEY = value;
            }
        }
        /// < summary>
        /// BRCH_BRCH_NO
        /// </summary>
        [AttributeField("BRCH_BRCH_NO", "System.String", false, false, false, "String")]
        public string BRCH_BRCH_NO
        {
            get
            {
                return this.BRCH_BRCH_NO;
            }
            set
            {
                this._BRCH_BRCH_NO = value;
            }
        }
        /// < summary>
        /// BRCH_BRCH_SEQ
        /// </summary>
        [AttributeField("BRCH_BRCH_SEQ", "System.String", false, false, false, "String")]
        public string BRCH_BRCH_SEQ
        {
            get
            {
                return this.BRCH_BRCH_SEQ;
            }
            set
            {
                this._BRCH_BRCH_SEQ = value;
            }
        }
        /// < summary>
        /// BRCH_BRCH_TYPE
        /// </summary>
        [AttributeField("BRCH_BRCH_TYPE", "System.String", false, false, false, "String")]
        public string BRCH_BRCH_TYPE
        {
            get
            {
                return this.BRCH_BRCH_TYPE;
            }
            set
            {
                this._BRCH_BRCH_TYPE = value;
            }
        }
        /// < summary>
        /// BRCH_NATION
        /// </summary>
        [AttributeField("BRCH_NATION", "System.String", false, false, false, "String")]
        public string BRCH_NATION
        {
            get
            {
                return this.BRCH_NATION;
            }
            set
            {
                this._BRCH_NATION = value;
            }
        }
        /// < summary>
        /// BRCH_BIRTH_DATE
        /// </summary>
        [AttributeField("BRCH_BIRTH_DATE", "System.String", false, false, false, "String")]
        public string BRCH_BIRTH_DATE
        {
            get
            {
                return this.BRCH_BIRTH_DATE;
            }
            set
            {
                this._BRCH_BIRTH_DATE = value;
            }
        }
        /// < summary>
        /// BRCH_PERM_CITY
        /// </summary>
        [AttributeField("BRCH_PERM_CITY", "System.String", false, false, false, "String")]
        public string BRCH_PERM_CITY
        {
            get
            {
                return this.BRCH_PERM_CITY;
            }
            set
            {
                this._BRCH_PERM_CITY = value;
            }
        }
        /// < summary>
        /// BRCH_PERM_ADDR1
        /// </summary>
        [AttributeField("BRCH_PERM_ADDR1", "System.String", false, false, false, "String")]
        public string BRCH_PERM_ADDR1
        {
            get
            {
                return this.BRCH_PERM_ADDR1;
            }
            set
            {
                this._BRCH_PERM_ADDR1 = value;
            }
        }
        /// < summary>
        /// BRCH_PERM_ADDR2
        /// </summary>
        [AttributeField("BRCH_PERM_ADDR2", "System.String", false, false, false, "String")]
        public string BRCH_PERM_ADDR2
        {
            get
            {
                return this.BRCH_PERM_ADDR2;
            }
            set
            {
                this._BRCH_PERM_ADDR2 = value;
            }
        }
        /// < summary>
        /// BRCH_CHINESE_NAME
        /// </summary>
        [AttributeField("BRCH_CHINESE_NAME", "System.String", false, false, false, "String")]
        public string BRCH_CHINESE_NAME
        {
            get
            {
                return this.BRCH_CHINESE_NAME;
            }
            set
            {
                this._BRCH_CHINESE_NAME = value;
            }
        }
        /// < summary>
        /// BRCH_ENGLISH_NAME
        /// </summary>
        [AttributeField("BRCH_ENGLISH_NAME", "System.String", false, false, false, "String")]
        public string BRCH_ENGLISH_NAME
        {
            get
            {
                return this.BRCH_ENGLISH_NAME;
            }
            set
            {
                this._BRCH_ENGLISH_NAME = value;
            }
        }
        /// < summary>
        /// BRCH_ID
        /// </summary>
        [AttributeField("BRCH_ID", "System.String", false, false, false, "String")]
        public string BRCH_ID
        {
            get
            {
                return this.BRCH_ID;
            }
            set
            {
                this._BRCH_ID = value;
            }
        }
        /// < summary>
        /// BRCH_OWNER_ID_ISSUE_DATE
        /// </summary>
        [AttributeField("BRCH_OWNER_ID_ISSUE_DATE", "System.String", false, false, false, "String")]
        public string BRCH_OWNER_ID_ISSUE_DATE
        {
            get
            {
                return this.BRCH_OWNER_ID_ISSUE_DATE;
            }
            set
            {
                this._BRCH_OWNER_ID_ISSUE_DATE = value;
            }
        }
        /// < summary>
        /// BRCH_OWNER_ID_ISSUE_PLACE
        /// </summary>
        [AttributeField("BRCH_OWNER_ID_ISSUE_PLACE", "System.String", false, false, false, "String")]
        public string BRCH_OWNER_ID_ISSUE_PLACE
        {
            get
            {
                return this.BRCH_OWNER_ID_ISSUE_PLACE;
            }
            set
            {
                this._BRCH_OWNER_ID_ISSUE_PLACE = value;
            }
        }
        /// < summary>
        /// BRCH_OWNER_ID_REPLACE_TYPE
        /// </summary>
        [AttributeField("BRCH_OWNER_ID_REPLACE_TYPE", "System.String", false, false, false, "String")]
        public string BRCH_OWNER_ID_REPLACE_TYPE
        {
            get
            {
                return this.BRCH_OWNER_ID_REPLACE_TYPE;
            }
            set
            {
                this._BRCH_OWNER_ID_REPLACE_TYPE = value;
            }
        }
        /// < summary>
        /// BRCH_ID_PHOTO_FLAG
        /// </summary>
        [AttributeField("BRCH_ID_PHOTO_FLAG", "System.String", false, false, false, "String")]
        public string BRCH_ID_PHOTO_FLAG
        {
            get
            {
                return this.BRCH_ID_PHOTO_FLAG;
            }
            set
            {
                this._BRCH_ID_PHOTO_FLAG = value;
            }
        }
        /// < summary>
        /// BRCH_PASSPORT
        /// </summary>
        [AttributeField("BRCH_PASSPORT", "System.String", false, false, false, "String")]
        public string BRCH_PASSPORT
        {
            get
            {
                return this.BRCH_PASSPORT;
            }
            set
            {
                this._BRCH_PASSPORT = value;
            }
        }
        /// < summary>
        /// BRCH_PASSPORT_EXP_DATE
        /// </summary>
        [AttributeField("BRCH_PASSPORT_EXP_DATE", "System.String", false, false, false, "String")]
        public string BRCH_PASSPORT_EXP_DATE
        {
            get
            {
                return this.BRCH_PASSPORT_EXP_DATE;
            }
            set
            {
                this._BRCH_PASSPORT_EXP_DATE = value;
            }
        }
        /// < summary>
        /// BRCH_RESIDENT_NO
        /// </summary>
        [AttributeField("BRCH_RESIDENT_NO", "System.String", false, false, false, "String")]
        public string BRCH_RESIDENT_NO
        {
            get
            {
                return this.BRCH_RESIDENT_NO;
            }
            set
            {
                this._BRCH_RESIDENT_NO = value;
            }
        }
        /// < summary>
        /// BRCH_RESIDENT_EXP_DATE
        /// </summary>
        [AttributeField("BRCH_RESIDENT_EXP_DATE", "System.String", false, false, false, "String")]
        public string BRCH_RESIDENT_EXP_DATE
        {
            get
            {
                return this.BRCH_RESIDENT_EXP_DATE;
            }
            set
            {
                this._BRCH_RESIDENT_EXP_DATE = value;
            }
        }
        /// < summary>
        /// BRCH_OTHER_CERT
        /// </summary>
        [AttributeField("BRCH_OTHER_CERT", "System.String", false, false, false, "String")]
        public string BRCH_OTHER_CERT
        {
            get
            {
                return this.BRCH_OTHER_CERT;
            }
            set
            {
                this._BRCH_OTHER_CERT = value;
            }
        }
        /// < summary>
        /// BRCH_OTHER_CERT_EXP_DATE
        /// </summary>
        [AttributeField("BRCH_OTHER_CERT_EXP_DATE", "System.String", false, false, false, "String")]
        public string BRCH_OTHER_CERT_EXP_DATE
        {
            get
            {
                return this.BRCH_OTHER_CERT_EXP_DATE;
            }
            set
            {
                this._BRCH_OTHER_CERT_EXP_DATE = value;
            }
        }
        /// < summary>
        /// BRCH_COMP_TEL
        /// </summary>
        [AttributeField("BRCH_COMP_TEL", "System.String", false, false, false, "String")]
        public string BRCH_COMP_TEL
        {
            get
            {
                return this.BRCH_COMP_TEL;
            }
            set
            {
                this._BRCH_COMP_TEL = value;
            }
        }
        /// < summary>
        /// BRCH_CREATE_DATE
        /// </summary>
        [AttributeField("BRCH_CREATE_DATE", "System.String", false, false, false, "String")]
        public string BRCH_CREATE_DATE
        {
            get
            {
                return this.BRCH_CREATE_DATE;
            }
            set
            {
                this._BRCH_CREATE_DATE = value;
            }
        }
        /// < summary>
        /// BRCH_STATUS
        /// </summary>
        [AttributeField("BRCH_STATUS", "System.String", false, false, false, "String")]
        public string BRCH_STATUS
        {
            get
            {
                return this.BRCH_STATUS;
            }
            set
            {
                this._BRCH_STATUS = value;
            }
        }
        /// < summary>
        /// BRCH_CIRCULATE_MERCH
        /// </summary>
        [AttributeField("BRCH_CIRCULATE_MERCH", "System.String", false, false, false, "String")]
        public string BRCH_CIRCULATE_MERCH
        {
            get
            {
                return this.BRCH_CIRCULATE_MERCH;
            }
            set
            {
                this._BRCH_CIRCULATE_MERCH = value;
            }
        }
        /// < summary>
        /// BRCH_HQ_BRCH_NO
        /// </summary>
        [AttributeField("BRCH_HQ_BRCH_NO", "System.String", false, false, false, "String")]
        public string BRCH_HQ_BRCH_NO
        {
            get
            {
                return this.BRCH_HQ_BRCH_NO;
            }
            set
            {
                this._BRCH_HQ_BRCH_NO = value;
            }
        }
        /// < summary>
        /// BRCH_HQ_BRCH_SEQ_NO
        /// </summary>
        [AttributeField("BRCH_HQ_BRCH_SEQ_NO", "System.String", false, false, false, "String")]
        public string BRCH_HQ_BRCH_SEQ_NO
        {
            get
            {
                return this.BRCH_HQ_BRCH_SEQ_NO;
            }
            set
            {
                this._BRCH_HQ_BRCH_SEQ_NO = value;
            }
        }
        /// < summary>
        /// BRCH_UPDATE_DATE
        /// </summary>
        [AttributeField("BRCH_UPDATE_DATE", "System.String", false, false, false, "String")]
        public string BRCH_UPDATE_DATE
        {
            get
            {
                return this.BRCH_UPDATE_DATE;
            }
            set
            {
                this._BRCH_UPDATE_DATE = value;
            }
        }
        /// < summary>
        /// BRCH_QUALIFY_FLAG
        /// </summary>
        [AttributeField("BRCH_QUALIFY_FLAG", "System.String", false, false, false, "String")]
        public string BRCH_QUALIFY_FLAG
        {
            get
            {
                return this.BRCH_QUALIFY_FLAG;
            }
            set
            {
                this._BRCH_QUALIFY_FLAG = value;
            }
        }
        /// < summary>
        /// BRCH_RESERVED_FILLER
        /// </summary>
        [AttributeField("BRCH_RESERVED_FILLER", "System.String", false, false, false, "String")]
        public string BRCH_RESERVED_FILLER
        {
            get
            {
                return this.BRCH_RESERVED_FILLER;
            }
            set
            {
                this._BRCH_RESERVED_FILLER = value;
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
    public class Entity_AML_BRCH_ImportSet : EntitySet<Entity_AML_BRCH_Import>
    {
    }
}
