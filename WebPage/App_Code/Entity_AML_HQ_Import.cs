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
    [AttributeTable("AML_HQ_Import")]
    public class Entity_AML_HQ_Import : Framework.Data.OM.Entity
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
        private string _HCOP_HEADQUATERS_CORP_NO;
        /// < summary>
        /// HCOP_HEADQUATERS_CORP_NO
        /// < /summary>
        public static string M_HCOP_HEADQUATERS_CORP_NO = "HCOP_HEADQUATERS_CORP_NO";
        private string _HCOP_HEADQUATERS_CORP_SEQ;
        /// < summary>
        /// HCOP_HEADQUATERS_CORP_SEQ
        /// < /summary>
        public static string M_HCOP_HEADQUATERS_CORP_SEQ = "HCOP_HEADQUATERS_CORP_SEQ";
        private string _HCOP_CORP_TYPE;
        /// < summary>
        /// HCOP_CORP_TYPE
        /// < /summary>
        public static string M_HCOP_CORP_TYPE = "HCOP_CORP_TYPE";
        private string _HCOP_REGISTER_NATION;
        /// < summary>
        /// HCOP_REGISTER_NATION
        /// < /summary>
        public static string M_HCOP_REGISTER_NATION = "HCOP_REGISTER_NATION";
        private string _HCOP_CORP_REG_ENG_NAME;
        /// < summary>
        /// HCOP_CORP_REG_ENG_NAME
        /// < /summary>
        public static string M_HCOP_CORP_REG_ENG_NAME = "HCOP_CORP_REG_ENG_NAME";
        private string _HCOP_REG_NAME;
        /// < summary>
        /// HCOP_REG_NAME
        /// < /summary>
        public static string M_HCOP_REG_NAME = "HCOP_REG_NAME";
        private string _HCOP_NAME_0E;
        /// < summary>
        /// HCOP_NAME_0E
        /// < /summary>
        public static string M_HCOP_NAME_0E = "HCOP_NAME_0E";
        private string _HCOP_NAME_CHI;
        /// < summary>
        /// HCOP_NAME_CHI
        /// < /summary>
        public static string M_HCOP_NAME_CHI = "HCOP_NAME_CHI";
        private string _HCOP_NAME_0F;
        /// < summary>
        /// HCOP_NAME_0F
        /// < /summary>
        public static string M_HCOP_NAME_0F = "HCOP_NAME_0F";
        private string _HCOP_BUILD_DATE;
        /// < summary>
        /// HCOP_BUILD_DATE
        /// < /summary>
        public static string M_HCOP_BUILD_DATE = "HCOP_BUILD_DATE";
        private string _HCOP_CC;
        /// < summary>
        /// HCOP_CC
        /// < /summary>
        public static string M_HCOP_CC = "HCOP_CC";
        private string _HCOP_REG_CITY;
        /// < summary>
        /// HCOP_REG_CITY
        /// < /summary>
        public static string M_HCOP_REG_CITY = "HCOP_REG_CITY";
        private string _HCOP_REG_ADDR1;
        /// < summary>
        /// HCOP_REG_ADDR1
        /// < /summary>
        public static string M_HCOP_REG_ADDR1 = "HCOP_REG_ADDR1";
        private string _HCOP_REG_ADDR2;
        /// < summary>
        /// HCOP_REG_ADDR2
        /// < /summary>
        public static string M_HCOP_REG_ADDR2 = "HCOP_REG_ADDR2";
        private string _HCOP_EMAIL;
        /// < summary>
        /// HCOP_EMAIL
        /// < /summary>
        public static string M_HCOP_EMAIL = "HCOP_EMAIL";
        private string _HCOP_NP_COMPANY_NAME;
        /// < summary>
        /// HCOP_NP_COMPANY_NAME
        /// < /summary>
        public static string M_HCOP_NP_COMPANY_NAME = "HCOP_NP_COMPANY_NAME";
        private string _HCOP_OWNER_NATION;
        /// < summary>
        /// HCOP_OWNER_NATION
        /// < /summary>
        public static string M_HCOP_OWNER_NATION = "HCOP_OWNER_NATION";
        private string _HCOP_OWNER_CHINESE_NAME;
        /// < summary>
        /// HCOP_OWNER_CHINESE_NAME
        /// < /summary>
        public static string M_HCOP_OWNER_CHINESE_NAME = "HCOP_OWNER_CHINESE_NAME";
        private string _HCOP_OWNER_ENGLISH_NAME;
        /// < summary>
        /// HCOP_OWNER_ENGLISH_NAME
        /// < /summary>
        public static string M_HCOP_OWNER_ENGLISH_NAME = "HCOP_OWNER_ENGLISH_NAME";
        private string _HCOP_OWNER_BIRTH_DATE;
        /// < summary>
        /// HCOP_OWNER_BIRTH_DATE
        /// < /summary>
        public static string M_HCOP_OWNER_BIRTH_DATE = "HCOP_OWNER_BIRTH_DATE";
        private string _HCOP_OWNER_ID;
        /// < summary>
        /// HCOP_OWNER_ID
        /// < /summary>
        public static string M_HCOP_OWNER_ID = "HCOP_OWNER_ID";
        private string _HCOP_OWNER_ID_ISSUE_DATE;
        /// < summary>
        /// HCOP_OWNER_ID_ISSUE_DATE
        /// < /summary>
        public static string M_HCOP_OWNER_ID_ISSUE_DATE = "HCOP_OWNER_ID_ISSUE_DATE";
        private string _HCOP_OWNER_ID_ISSUE_PLACE;
        /// < summary>
        /// HCOP_OWNER_ID_ISSUE_PLACE
        /// < /summary>
        public static string M_HCOP_OWNER_ID_ISSUE_PLACE = "HCOP_OWNER_ID_ISSUE_PLACE";
        private string _HCOP_OWNER_ID_REPLACE_TYPE;
        /// < summary>
        /// HCOP_OWNER_ID_REPLACE_TYPE
        /// < /summary>
        public static string M_HCOP_OWNER_ID_REPLACE_TYPE = "HCOP_OWNER_ID_REPLACE_TYPE";
        private string _HCOP_ID_PHOTO_FLAG;
        /// < summary>
        /// HCOP_ID_PHOTO_FLAG
        /// < /summary>
        public static string M_HCOP_ID_PHOTO_FLAG = "HCOP_ID_PHOTO_FLAG";
        private string _HCOP_PASSPORT;
        /// < summary>
        /// HCOP_PASSPORT
        /// < /summary>
        public static string M_HCOP_PASSPORT = "HCOP_PASSPORT";
        private string _HCOP_PASSPORT_EXP_DATE;
        /// < summary>
        /// HCOP_PASSPORT_EXP_DATE
        /// < /summary>
        public static string M_HCOP_PASSPORT_EXP_DATE = "HCOP_PASSPORT_EXP_DATE";
        private string _HCOP_RESIDENT_NO;
        /// < summary>
        /// HCOP_RESIDENT_NO
        /// < /summary>
        public static string M_HCOP_RESIDENT_NO = "HCOP_RESIDENT_NO";
        private string _HCOP_RESIDENT_EXP_DATE;
        /// < summary>
        /// HCOP_RESIDENT_EXP_DATE
        /// < /summary>
        public static string M_HCOP_RESIDENT_EXP_DATE = "HCOP_RESIDENT_EXP_DATE";
        private string _HCOP_OTHER_CERT;
        /// < summary>
        /// HCOP_OTHER_CERT
        /// < /summary>
        public static string M_HCOP_OTHER_CERT = "HCOP_OTHER_CERT";
        private string _HCOP_OTHER_CERT_EXP_DATE;
        /// < summary>
        /// HCOP_OTHER_CERT_EXP_DATE
        /// < /summary>
        public static string M_HCOP_OTHER_CERT_EXP_DATE = "HCOP_OTHER_CERT_EXP_DATE";
        private string _HCOP_COMPLEX_STR_CODE;
        /// < summary>
        /// HCOP_COMPLEX_STR_CODE
        /// < /summary>
        public static string M_HCOP_COMPLEX_STR_CODE = "HCOP_COMPLEX_STR_CODE";
        private string _HCOP_ISSUE_STOCK_FLAG;
        /// < summary>
        /// HCOP_ISSUE_STOCK_FLAG
        /// < /summary>
        public static string M_HCOP_ISSUE_STOCK_FLAG = "HCOP_ISSUE_STOCK_FLAG";
        private string _HCOP_COMP_TEL;
        /// < summary>
        /// HCOP_COMP_TEL
        /// < /summary>
        public static string M_HCOP_COMP_TEL = "HCOP_COMP_TEL";
        private string _HCOP_MAILING_CITY;
        /// < summary>
        /// HCOP_MAILING_CITY
        /// < /summary>
        public static string M_HCOP_MAILING_CITY = "HCOP_MAILING_CITY";
        private string _HCOP_MAILING_ADDR1;
        /// < summary>
        /// HCOP_MAILING_ADDR1
        /// < /summary>
        public static string M_HCOP_MAILING_ADDR1 = "HCOP_MAILING_ADDR1";
        private string _HCOP_MAILING_ADDR2;
        /// < summary>
        /// HCOP_MAILING_ADDR2
        /// < /summary>
        public static string M_HCOP_MAILING_ADDR2 = "HCOP_MAILING_ADDR2";
        private string _HCOP_PRIMARY_BUSI_COUNTRY;
        /// < summary>
        /// HCOP_PRIMARY_BUSI_COUNTRY
        /// < /summary>
        public static string M_HCOP_PRIMARY_BUSI_COUNTRY = "HCOP_PRIMARY_BUSI_COUNTRY";
        private string _HCOP_BUSI_RISK_NATION_FLAG;
        /// < summary>
        /// HCOP_BUSI_RISK_NATION_FLAG
        /// < /summary>
        public static string M_HCOP_BUSI_RISK_NATION_FLAG = "HCOP_BUSI_RISK_NATION_FLAG";
        private string _HCOP_BUSI_RISK_NATION_1;
        /// < summary>
        /// HCOP_BUSI_RISK_NATION_1
        /// < /summary>
        public static string M_HCOP_BUSI_RISK_NATION_1 = "HCOP_BUSI_RISK_NATION_1";
        private string _HCOP_BUSI_RISK_NATION_2;
        /// < summary>
        /// HCOP_BUSI_RISK_NATION_2
        /// < /summary>
        public static string M_HCOP_BUSI_RISK_NATION_2 = "HCOP_BUSI_RISK_NATION_2";
        private string _HCOP_BUSI_RISK_NATION_3;
        /// < summary>
        /// HCOP_BUSI_RISK_NATION_3
        /// < /summary>
        public static string M_HCOP_BUSI_RISK_NATION_3 = "HCOP_BUSI_RISK_NATION_3";
        private string _HCOP_BUSI_RISK_NATION_4;
        /// < summary>
        /// HCOP_BUSI_RISK_NATION_4
        /// < /summary>
        public static string M_HCOP_BUSI_RISK_NATION_4 = "HCOP_BUSI_RISK_NATION_4";
        private string _HCOP_BUSI_RISK_NATION_5;
        /// < summary>
        /// HCOP_BUSI_RISK_NATION_5
        /// < /summary>
        public static string M_HCOP_BUSI_RISK_NATION_5 = "HCOP_BUSI_RISK_NATION_5";
        private string _HCOP_OVERSEAS_FOREIGN;
        /// < summary>
        /// HCOP_OVERSEAS_FOREIGN
        /// < /summary>
        public static string M_HCOP_OVERSEAS_FOREIGN = "HCOP_OVERSEAS_FOREIGN";
        private string _HCOP_OVERSEAS_FOREIGN_COUNTRY;
        /// < summary>
        /// HCOP_OVERSEAS_FOREIGN_COUNTRY
        /// < /summary>
        public static string M_HCOP_OVERSEAS_FOREIGN_COUNTRY = "HCOP_OVERSEAS_FOREIGN_COUNTRY";
        private string _HCOP_REGISTER_US_STATE;
        /// < summary>
        /// HCOP_REGISTER_US_STATE
        /// < /summary>
        public static string M_HCOP_REGISTER_US_STATE = "HCOP_REGISTER_US_STATE";
        private string _HCOP_BUSINESS_ORGAN_TYPE;
        /// < summary>
        /// HCOP_BUSINESS_ORGAN_TYPE
        /// < /summary>
        public static string M_HCOP_BUSINESS_ORGAN_TYPE = "HCOP_BUSINESS_ORGAN_TYPE";
        private string _HCOP_CREATE_DATE;
        /// < summary>
        /// HCOP_CREATE_DATE
        /// < /summary>
        public static string M_HCOP_CREATE_DATE = "HCOP_CREATE_DATE";
        private string _HCOP_STATUS;
        /// < summary>
        /// HCOP_STATUS
        /// < /summary>
        public static string M_HCOP_STATUS = "HCOP_STATUS";
        private string _HCOP_QUALIFY_FLAG;
        /// < summary>
        /// HCOP_QUALIFY_FLAG
        /// < /summary>
        public static string M_HCOP_QUALIFY_FLAG = "HCOP_QUALIFY_FLAG";
        private string _HCOP_CONTACT_NAME;
        /// < summary>
        /// HCOP_CONTACT_NAME
        /// < /summary>
        public static string M_HCOP_CONTACT_NAME = "HCOP_CONTACT_NAME";
        private string _HCOP_EXAMINE_FLAG;
        /// < summary>
        /// HCOP_EXAMINE_FLAG
        /// < /summary>
        public static string M_HCOP_EXAMINE_FLAG = "HCOP_EXAMINE_FLAG";
        private string _HCOP_ALLOW_ISSUE_STOCK_FLAG;
        /// < summary>
        /// HCOP_ALLOW_ISSUE_STOCK_FLAG
        /// < /summary>
        public static string M_HCOP_ALLOW_ISSUE_STOCK_FLAG = "HCOP_ALLOW_ISSUE_STOCK_FLAG";
        private string _HCOP_CONTACT_TEL;
        /// < summary>
        /// HCOP_CONTACT_TEL
        /// < /summary>
        public static string M_HCOP_CONTACT_TEL = "HCOP_CONTACT_TEL";
        private string _HCOP_RESERVED_FILLER;
        /// < summary>
        /// HCOP_RESERVED_FILLER
        /// < /summary>
        public static string M_HCOP_RESERVED_FILLER = "HCOP_RESERVED_FILLER";
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
        /// HCOP_HEADQUATERS_CORP_NO
        /// </summary>
        [AttributeField("HCOP_HEADQUATERS_CORP_NO", "System.String", false, false, false, "String")]
        public string HCOP_HEADQUATERS_CORP_NO
        {
            get
            {
                return this.HCOP_HEADQUATERS_CORP_NO;
            }
            set
            {
                this._HCOP_HEADQUATERS_CORP_NO = value;
            }
        }
        /// < summary>
        /// HCOP_HEADQUATERS_CORP_SEQ
        /// </summary>
        [AttributeField("HCOP_HEADQUATERS_CORP_SEQ", "System.String", false, false, false, "String")]
        public string HCOP_HEADQUATERS_CORP_SEQ
        {
            get
            {
                return this.HCOP_HEADQUATERS_CORP_SEQ;
            }
            set
            {
                this._HCOP_HEADQUATERS_CORP_SEQ = value;
            }
        }
        /// < summary>
        /// HCOP_CORP_TYPE
        /// </summary>
        [AttributeField("HCOP_CORP_TYPE", "System.String", false, false, false, "String")]
        public string HCOP_CORP_TYPE
        {
            get
            {
                return this.HCOP_CORP_TYPE;
            }
            set
            {
                this._HCOP_CORP_TYPE = value;
            }
        }
        /// < summary>
        /// HCOP_REGISTER_NATION
        /// </summary>
        [AttributeField("HCOP_REGISTER_NATION", "System.String", false, false, false, "String")]
        public string HCOP_REGISTER_NATION
        {
            get
            {
                return this.HCOP_REGISTER_NATION;
            }
            set
            {
                this._HCOP_REGISTER_NATION = value;
            }
        }
        /// < summary>
        /// HCOP_CORP_REG_ENG_NAME
        /// </summary>
        [AttributeField("HCOP_CORP_REG_ENG_NAME", "System.String", false, false, false, "String")]
        public string HCOP_CORP_REG_ENG_NAME
        {
            get
            {
                return this.HCOP_CORP_REG_ENG_NAME;
            }
            set
            {
                this._HCOP_CORP_REG_ENG_NAME = value;
            }
        }
        /// < summary>
        /// HCOP_REG_NAME
        /// </summary>
        [AttributeField("HCOP_REG_NAME", "System.String", false, false, false, "String")]
        public string HCOP_REG_NAME
        {
            get
            {
                return this.HCOP_REG_NAME;
            }
            set
            {
                this._HCOP_REG_NAME = value;
            }
        }
        /// < summary>
        /// HCOP_NAME_0E
        /// </summary>
        [AttributeField("HCOP_NAME_0E", "System.String", false, false, false, "String")]
        public string HCOP_NAME_0E
        {
            get
            {
                return this.HCOP_NAME_0E;
            }
            set
            {
                this._HCOP_NAME_0E = value;
            }
        }
        /// < summary>
        /// HCOP_NAME_CHI
        /// </summary>
        [AttributeField("HCOP_NAME_CHI", "System.String", false, false, false, "String")]
        public string HCOP_NAME_CHI
        {
            get
            {
                return this.HCOP_NAME_CHI;
            }
            set
            {
                this._HCOP_NAME_CHI = value;
            }
        }
        /// < summary>
        /// HCOP_NAME_0F
        /// </summary>
        [AttributeField("HCOP_NAME_0F", "System.String", false, false, false, "String")]
        public string HCOP_NAME_0F
        {
            get
            {
                return this.HCOP_NAME_0F;
            }
            set
            {
                this._HCOP_NAME_0F = value;
            }
        }
        /// < summary>
        /// HCOP_BUILD_DATE
        /// </summary>
        [AttributeField("HCOP_BUILD_DATE", "System.String", false, false, false, "String")]
        public string HCOP_BUILD_DATE
        {
            get
            {
                return this.HCOP_BUILD_DATE;
            }
            set
            {
                this._HCOP_BUILD_DATE = value;
            }
        }
        /// < summary>
        /// HCOP_CC
        /// </summary>
        [AttributeField("HCOP_CC", "System.String", false, false, false, "String")]
        public string HCOP_CC
        {
            get
            {
                return this.HCOP_CC;
            }
            set
            {
                this._HCOP_CC = value;
            }
        }
        /// < summary>
        /// HCOP_REG_CITY
        /// </summary>
        [AttributeField("HCOP_REG_CITY", "System.String", false, false, false, "String")]
        public string HCOP_REG_CITY
        {
            get
            {
                return this.HCOP_REG_CITY;
            }
            set
            {
                this._HCOP_REG_CITY = value;
            }
        }
        /// < summary>
        /// HCOP_REG_ADDR1
        /// </summary>
        [AttributeField("HCOP_REG_ADDR1", "System.String", false, false, false, "String")]
        public string HCOP_REG_ADDR1
        {
            get
            {
                return this.HCOP_REG_ADDR1;
            }
            set
            {
                this._HCOP_REG_ADDR1 = value;
            }
        }
        /// < summary>
        /// HCOP_REG_ADDR2
        /// </summary>
        [AttributeField("HCOP_REG_ADDR2", "System.String", false, false, false, "String")]
        public string HCOP_REG_ADDR2
        {
            get
            {
                return this.HCOP_REG_ADDR2;
            }
            set
            {
                this._HCOP_REG_ADDR2 = value;
            }
        }
        /// < summary>
        /// HCOP_EMAIL
        /// </summary>
        [AttributeField("HCOP_EMAIL", "System.String", false, false, false, "String")]
        public string HCOP_EMAIL
        {
            get
            {
                return this.HCOP_EMAIL;
            }
            set
            {
                this._HCOP_EMAIL = value;
            }
        }
        /// < summary>
        /// HCOP_NP_COMPANY_NAME
        /// </summary>
        [AttributeField("HCOP_NP_COMPANY_NAME", "System.String", false, false, false, "String")]
        public string HCOP_NP_COMPANY_NAME
        {
            get
            {
                return this.HCOP_NP_COMPANY_NAME;
            }
            set
            {
                this._HCOP_NP_COMPANY_NAME = value;
            }
        }
        /// < summary>
        /// HCOP_OWNER_NATION
        /// </summary>
        [AttributeField("HCOP_OWNER_NATION", "System.String", false, false, false, "String")]
        public string HCOP_OWNER_NATION
        {
            get
            {
                return this.HCOP_OWNER_NATION;
            }
            set
            {
                this._HCOP_OWNER_NATION = value;
            }
        }
        /// < summary>
        /// HCOP_OWNER_CHINESE_NAME
        /// </summary>
        [AttributeField("HCOP_OWNER_CHINESE_NAME", "System.String", false, false, false, "String")]
        public string HCOP_OWNER_CHINESE_NAME
        {
            get
            {
                return this.HCOP_OWNER_CHINESE_NAME;
            }
            set
            {
                this._HCOP_OWNER_CHINESE_NAME = value;
            }
        }
        /// < summary>
        /// HCOP_OWNER_ENGLISH_NAME
        /// </summary>
        [AttributeField("HCOP_OWNER_ENGLISH_NAME", "System.String", false, false, false, "String")]
        public string HCOP_OWNER_ENGLISH_NAME
        {
            get
            {
                return this.HCOP_OWNER_ENGLISH_NAME;
            }
            set
            {
                this._HCOP_OWNER_ENGLISH_NAME = value;
            }
        }
        /// < summary>
        /// HCOP_OWNER_BIRTH_DATE
        /// </summary>
        [AttributeField("HCOP_OWNER_BIRTH_DATE", "System.String", false, false, false, "String")]
        public string HCOP_OWNER_BIRTH_DATE
        {
            get
            {
                return this.HCOP_OWNER_BIRTH_DATE;
            }
            set
            {
                this._HCOP_OWNER_BIRTH_DATE = value;
            }
        }
        /// < summary>
        /// HCOP_OWNER_ID
        /// </summary>
        [AttributeField("HCOP_OWNER_ID", "System.String", false, false, false, "String")]
        public string HCOP_OWNER_ID
        {
            get
            {
                return this.HCOP_OWNER_ID;
            }
            set
            {
                this._HCOP_OWNER_ID = value;
            }
        }
        /// < summary>
        /// HCOP_OWNER_ID_ISSUE_DATE
        /// </summary>
        [AttributeField("HCOP_OWNER_ID_ISSUE_DATE", "System.String", false, false, false, "String")]
        public string HCOP_OWNER_ID_ISSUE_DATE
        {
            get
            {
                return this.HCOP_OWNER_ID_ISSUE_DATE;
            }
            set
            {
                this._HCOP_OWNER_ID_ISSUE_DATE = value;
            }
        }
        /// < summary>
        /// HCOP_OWNER_ID_ISSUE_PLACE
        /// </summary>
        [AttributeField("HCOP_OWNER_ID_ISSUE_PLACE", "System.String", false, false, false, "String")]
        public string HCOP_OWNER_ID_ISSUE_PLACE
        {
            get
            {
                return this.HCOP_OWNER_ID_ISSUE_PLACE;
            }
            set
            {
                this._HCOP_OWNER_ID_ISSUE_PLACE = value;
            }
        }
        /// < summary>
        /// HCOP_OWNER_ID_REPLACE_TYPE
        /// </summary>
        [AttributeField("HCOP_OWNER_ID_REPLACE_TYPE", "System.String", false, false, false, "String")]
        public string HCOP_OWNER_ID_REPLACE_TYPE
        {
            get
            {
                return this.HCOP_OWNER_ID_REPLACE_TYPE;
            }
            set
            {
                this._HCOP_OWNER_ID_REPLACE_TYPE = value;
            }
        }
        /// < summary>
        /// HCOP_ID_PHOTO_FLAG
        /// </summary>
        [AttributeField("HCOP_ID_PHOTO_FLAG", "System.String", false, false, false, "String")]
        public string HCOP_ID_PHOTO_FLAG
        {
            get
            {
                return this.HCOP_ID_PHOTO_FLAG;
            }
            set
            {
                this._HCOP_ID_PHOTO_FLAG = value;
            }
        }
        /// < summary>
        /// HCOP_PASSPORT
        /// </summary>
        [AttributeField("HCOP_PASSPORT", "System.String", false, false, false, "String")]
        public string HCOP_PASSPORT
        {
            get
            {
                return this.HCOP_PASSPORT;
            }
            set
            {
                this._HCOP_PASSPORT = value;
            }
        }
        /// < summary>
        /// HCOP_PASSPORT_EXP_DATE
        /// </summary>
        [AttributeField("HCOP_PASSPORT_EXP_DATE", "System.String", false, false, false, "String")]
        public string HCOP_PASSPORT_EXP_DATE
        {
            get
            {
                return this.HCOP_PASSPORT_EXP_DATE;
            }
            set
            {
                this._HCOP_PASSPORT_EXP_DATE = value;
            }
        }
        /// < summary>
        /// HCOP_RESIDENT_NO
        /// </summary>
        [AttributeField("HCOP_RESIDENT_NO", "System.String", false, false, false, "String")]
        public string HCOP_RESIDENT_NO
        {
            get
            {
                return this.HCOP_RESIDENT_NO;
            }
            set
            {
                this._HCOP_RESIDENT_NO = value;
            }
        }
        /// < summary>
        /// HCOP_RESIDENT_EXP_DATE
        /// </summary>
        [AttributeField("HCOP_RESIDENT_EXP_DATE", "System.String", false, false, false, "String")]
        public string HCOP_RESIDENT_EXP_DATE
        {
            get
            {
                return this.HCOP_RESIDENT_EXP_DATE;
            }
            set
            {
                this._HCOP_RESIDENT_EXP_DATE = value;
            }
        }
        /// < summary>
        /// HCOP_OTHER_CERT
        /// </summary>
        [AttributeField("HCOP_OTHER_CERT", "System.String", false, false, false, "String")]
        public string HCOP_OTHER_CERT
        {
            get
            {
                return this.HCOP_OTHER_CERT;
            }
            set
            {
                this._HCOP_OTHER_CERT = value;
            }
        }
        /// < summary>
        /// HCOP_OTHER_CERT_EXP_DATE
        /// </summary>
        [AttributeField("HCOP_OTHER_CERT_EXP_DATE", "System.String", false, false, false, "String")]
        public string HCOP_OTHER_CERT_EXP_DATE
        {
            get
            {
                return this.HCOP_OTHER_CERT_EXP_DATE;
            }
            set
            {
                this._HCOP_OTHER_CERT_EXP_DATE = value;
            }
        }
        /// < summary>
        /// HCOP_COMPLEX_STR_CODE
        /// </summary>
        [AttributeField("HCOP_COMPLEX_STR_CODE", "System.String", false, false, false, "String")]
        public string HCOP_COMPLEX_STR_CODE
        {
            get
            {
                return this.HCOP_COMPLEX_STR_CODE;
            }
            set
            {
                this._HCOP_COMPLEX_STR_CODE = value;
            }
        }
        /// < summary>
        /// HCOP_ISSUE_STOCK_FLAG
        /// </summary>
        [AttributeField("HCOP_ISSUE_STOCK_FLAG", "System.String", false, false, false, "String")]
        public string HCOP_ISSUE_STOCK_FLAG
        {
            get
            {
                return this.HCOP_ISSUE_STOCK_FLAG;
            }
            set
            {
                this._HCOP_ISSUE_STOCK_FLAG = value;
            }
        }
        /// < summary>
        /// HCOP_COMP_TEL
        /// </summary>
        [AttributeField("HCOP_COMP_TEL", "System.String", false, false, false, "String")]
        public string HCOP_COMP_TEL
        {
            get
            {
                return this.HCOP_COMP_TEL;
            }
            set
            {
                this._HCOP_COMP_TEL = value;
            }
        }
        /// < summary>
        /// HCOP_MAILING_CITY
        /// </summary>
        [AttributeField("HCOP_MAILING_CITY", "System.String", false, false, false, "String")]
        public string HCOP_MAILING_CITY
        {
            get
            {
                return this.HCOP_MAILING_CITY;
            }
            set
            {
                this._HCOP_MAILING_CITY = value;
            }
        }
        /// < summary>
        /// HCOP_MAILING_ADDR1
        /// </summary>
        [AttributeField("HCOP_MAILING_ADDR1", "System.String", false, false, false, "String")]
        public string HCOP_MAILING_ADDR1
        {
            get
            {
                return this.HCOP_MAILING_ADDR1;
            }
            set
            {
                this._HCOP_MAILING_ADDR1 = value;
            }
        }
        /// < summary>
        /// HCOP_MAILING_ADDR2
        /// </summary>
        [AttributeField("HCOP_MAILING_ADDR2", "System.String", false, false, false, "String")]
        public string HCOP_MAILING_ADDR2
        {
            get
            {
                return this.HCOP_MAILING_ADDR2;
            }
            set
            {
                this._HCOP_MAILING_ADDR2 = value;
            }
        }
        /// < summary>
        /// HCOP_PRIMARY_BUSI_COUNTRY
        /// </summary>
        [AttributeField("HCOP_PRIMARY_BUSI_COUNTRY", "System.String", false, false, false, "String")]
        public string HCOP_PRIMARY_BUSI_COUNTRY
        {
            get
            {
                return this.HCOP_PRIMARY_BUSI_COUNTRY;
            }
            set
            {
                this._HCOP_PRIMARY_BUSI_COUNTRY = value;
            }
        }
        /// < summary>
        /// HCOP_BUSI_RISK_NATION_FLAG
        /// </summary>
        [AttributeField("HCOP_BUSI_RISK_NATION_FLAG", "System.String", false, false, false, "String")]
        public string HCOP_BUSI_RISK_NATION_FLAG
        {
            get
            {
                return this.HCOP_BUSI_RISK_NATION_FLAG;
            }
            set
            {
                this._HCOP_BUSI_RISK_NATION_FLAG = value;
            }
        }
        /// < summary>
        /// HCOP_BUSI_RISK_NATION_1
        /// </summary>
        [AttributeField("HCOP_BUSI_RISK_NATION_1", "System.String", false, false, false, "String")]
        public string HCOP_BUSI_RISK_NATION_1
        {
            get
            {
                return this.HCOP_BUSI_RISK_NATION_1;
            }
            set
            {
                this._HCOP_BUSI_RISK_NATION_1 = value;
            }
        }
        /// < summary>
        /// HCOP_BUSI_RISK_NATION_2
        /// </summary>
        [AttributeField("HCOP_BUSI_RISK_NATION_2", "System.String", false, false, false, "String")]
        public string HCOP_BUSI_RISK_NATION_2
        {
            get
            {
                return this.HCOP_BUSI_RISK_NATION_2;
            }
            set
            {
                this._HCOP_BUSI_RISK_NATION_2 = value;
            }
        }
        /// < summary>
        /// HCOP_BUSI_RISK_NATION_3
        /// </summary>
        [AttributeField("HCOP_BUSI_RISK_NATION_3", "System.String", false, false, false, "String")]
        public string HCOP_BUSI_RISK_NATION_3
        {
            get
            {
                return this.HCOP_BUSI_RISK_NATION_3;
            }
            set
            {
                this._HCOP_BUSI_RISK_NATION_3 = value;
            }
        }
        /// < summary>
        /// HCOP_BUSI_RISK_NATION_4
        /// </summary>
        [AttributeField("HCOP_BUSI_RISK_NATION_4", "System.String", false, false, false, "String")]
        public string HCOP_BUSI_RISK_NATION_4
        {
            get
            {
                return this.HCOP_BUSI_RISK_NATION_4;
            }
            set
            {
                this._HCOP_BUSI_RISK_NATION_4 = value;
            }
        }
        /// < summary>
        /// HCOP_BUSI_RISK_NATION_5
        /// </summary>
        [AttributeField("HCOP_BUSI_RISK_NATION_5", "System.String", false, false, false, "String")]
        public string HCOP_BUSI_RISK_NATION_5
        {
            get
            {
                return this.HCOP_BUSI_RISK_NATION_5;
            }
            set
            {
                this._HCOP_BUSI_RISK_NATION_5 = value;
            }
        }
        /// < summary>
        /// HCOP_OVERSEAS_FOREIGN
        /// </summary>
        [AttributeField("HCOP_OVERSEAS_FOREIGN", "System.String", false, false, false, "String")]
        public string HCOP_OVERSEAS_FOREIGN
        {
            get
            {
                return this.HCOP_OVERSEAS_FOREIGN;
            }
            set
            {
                this._HCOP_OVERSEAS_FOREIGN = value;
            }
        }
        /// < summary>
        /// HCOP_OVERSEAS_FOREIGN_COUNTRY
        /// </summary>
        [AttributeField("HCOP_OVERSEAS_FOREIGN_COUNTRY", "System.String", false, false, false, "String")]
        public string HCOP_OVERSEAS_FOREIGN_COUNTRY
        {
            get
            {
                return this.HCOP_OVERSEAS_FOREIGN_COUNTRY;
            }
            set
            {
                this._HCOP_OVERSEAS_FOREIGN_COUNTRY = value;
            }
        }
        /// < summary>
        /// HCOP_REGISTER_US_STATE
        /// </summary>
        [AttributeField("HCOP_REGISTER_US_STATE", "System.String", false, false, false, "String")]
        public string HCOP_REGISTER_US_STATE
        {
            get
            {
                return this.HCOP_REGISTER_US_STATE;
            }
            set
            {
                this._HCOP_REGISTER_US_STATE = value;
            }
        }
        /// < summary>
        /// HCOP_BUSINESS_ORGAN_TYPE
        /// </summary>
        [AttributeField("HCOP_BUSINESS_ORGAN_TYPE", "System.String", false, false, false, "String")]
        public string HCOP_BUSINESS_ORGAN_TYPE
        {
            get
            {
                return this.HCOP_BUSINESS_ORGAN_TYPE;
            }
            set
            {
                this._HCOP_BUSINESS_ORGAN_TYPE = value;
            }
        }
        /// < summary>
        /// HCOP_CREATE_DATE
        /// </summary>
        [AttributeField("HCOP_CREATE_DATE", "System.String", false, false, false, "String")]
        public string HCOP_CREATE_DATE
        {
            get
            {
                return this.HCOP_CREATE_DATE;
            }
            set
            {
                this._HCOP_CREATE_DATE = value;
            }
        }
        /// < summary>
        /// HCOP_STATUS
        /// </summary>
        [AttributeField("HCOP_STATUS", "System.String", false, false, false, "String")]
        public string HCOP_STATUS
        {
            get
            {
                return this.HCOP_STATUS;
            }
            set
            {
                this._HCOP_STATUS = value;
            }
        }
        /// < summary>
        /// HCOP_QUALIFY_FLAG
        /// </summary>
        [AttributeField("HCOP_QUALIFY_FLAG", "System.String", false, false, false, "String")]
        public string HCOP_QUALIFY_FLAG
        {
            get
            {
                return this.HCOP_QUALIFY_FLAG;
            }
            set
            {
                this._HCOP_QUALIFY_FLAG = value;
            }
        }
        /// < summary>
        /// HCOP_CONTACT_NAME
        /// </summary>
        [AttributeField("HCOP_CONTACT_NAME", "System.String", false, false, false, "String")]
        public string HCOP_CONTACT_NAME
        {
            get
            {
                return this.HCOP_CONTACT_NAME;
            }
            set
            {
                this._HCOP_CONTACT_NAME = value;
            }
        }
        /// < summary>
        /// HCOP_EXAMINE_FLAG
        /// </summary>
        [AttributeField("HCOP_EXAMINE_FLAG", "System.String", false, false, false, "String")]
        public string HCOP_EXAMINE_FLAG
        {
            get
            {
                return this.HCOP_EXAMINE_FLAG;
            }
            set
            {
                this._HCOP_EXAMINE_FLAG = value;
            }
        }
        /// < summary>
        /// HCOP_ALLOW_ISSUE_STOCK_FLAG
        /// </summary>
        [AttributeField("HCOP_ALLOW_ISSUE_STOCK_FLAG", "System.String", false, false, false, "String")]
        public string HCOP_ALLOW_ISSUE_STOCK_FLAG
        {
            get
            {
                return this.HCOP_ALLOW_ISSUE_STOCK_FLAG;
            }
            set
            {
                this._HCOP_ALLOW_ISSUE_STOCK_FLAG = value;
            }
        }
        /// < summary>
        /// HCOP_CONTACT_TEL
        /// </summary>
        [AttributeField("HCOP_CONTACT_TEL", "System.String", false, false, false, "String")]
        public string HCOP_CONTACT_TEL
        {
            get
            {
                return this.HCOP_CONTACT_TEL;
            }
            set
            {
                this._HCOP_CONTACT_TEL = value;
            }
        }
        /// < summary>
        /// HCOP_RESERVED_FILLER
        /// </summary>
        [AttributeField("HCOP_RESERVED_FILLER", "System.String", false, false, false, "String")]
        public string HCOP_RESERVED_FILLER
        {
            get
            {
                return this.HCOP_RESERVED_FILLER;
            }
            set
            {
                this._HCOP_RESERVED_FILLER = value;
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
    public class Entity_AML_HQ_ImportSet : EntitySet<Entity_AML_HQ_Import>
    {
    }
}
