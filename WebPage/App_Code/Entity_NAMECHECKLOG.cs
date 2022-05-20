using System;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;

namespace CSIPCommonModel.EntityLayer_new
{
    /// <summary>
    /// Entity_NAMECHECKLOG
    /// </summary>
    [Serializable()]
    [AttributeTable("NAMECHECKLOG")]
    public class Entity_NAMECHECKLOG : Entity
    {
        // CTBC.CMS.Models.NAMECHECKLOG
        private string _CASE_NO;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("CASE_NO", "System.String", false, true, false, "String")]
        public static string M_CASE_NO = "CASE_NO";
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

        private string _SEQ;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("SEQ", "System.String", false, true, false, "String")]
        public static string M_SEQ = "SEQ";
        public string SEQ
        {
            get
            {
                return this._SEQ;
            }
            set
            {
                this._SEQ = value;
            }
        }

        private string _REPCODE;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("REPCODE", "System.String", false, false, false, "String")]
        public static string M_REPCODE = "REPCODE";
        public string REPCODE
        {
            get
            {
                return this._REPCODE;
            }
            set
            {
                this._REPCODE = value;
            }
        }


        private string _TRNNUM;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("TRNNUM", "System.String", false, false, false, "String")]
        public static string M_TRNNUM = "TRNNUM";
        public string TRNNUM
        {
            get
            {
                return this._TRNNUM;
            }
            set
            {
                this._TRNNUM = value;
            }
        }


        private string _BankNo;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("BankNo", "System.String", false, false, false, "String")]
        public static string M_BankNo = "BankNo";
        public string BankNo
        {
            get
            {
                return this._BankNo;
            }
            set
            {
                this._BankNo = value;
            }
        }


        private string _BranchNo;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("BranchNo", "System.String", false, false, false, "String")]
        public static string M_BranchNo = "BranchNo";
        public string BranchNo
        {
            get
            {
                return this._BranchNo;
            }
            set
            {
                this._BranchNo = value;
            }
        }


        private string _ID;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("ID", "System.String", false, false, false, "String")]
        public static string M_ID = "ID";
        public string ID
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


        private string _EnglishName;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("EnglishName", "System.String", false, false, false, "String")]
        public static string M_EnglishName = "EnglishName";
        public string EnglishName
        {
            get
            {
                return this._EnglishName;
            }
            set
            {
                this._EnglishName = value;
            }
        }


        private string _NonEnglishName;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("NonEnglishName", "System.String", false, false, false, "String")]
        public static string M_NonEnglishName = "NonEnglishName";
        public string NonEnglishName
        {
            get
            {
                return this._NonEnglishName;
            }
            set
            {
                this._NonEnglishName = value;
            }
        }

        //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
        //private string _EnglishName2;
        ///// <summary>
        ///// IdentityKey
        ///// </summary>
        //[AttributeField("EnglishName2", "System.String", false, false, false, "String")]
        //public static string M_EnglishName2 = "EnglishName2";
        //public string EnglishName2
        //{
        //    get
        //    {
        //        return this._EnglishName2;
        //    }
        //    set
        //    {
        //        this._EnglishName2 = value;
        //    }
        //}
        //20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis

        private string _DOB;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("DOB", "System.String", false, false, false, "String")]
        public static string M_DOB = "DOB";
        public string DOB
        {
            get
            {
                return this._DOB;
            }
            set
            {
                this._DOB = value;
            }
        }


        private string _Nationality;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("Nationality", "System.String", false, false, false, "String")]
        public static string M_Nationality = "Nationality";
        public string Nationality
        {
            get
            {
                return this._Nationality;
            }
            set
            {
                this._Nationality = value;
            }
        }


        private string _TellerName;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("TellerName", "System.String", false, false, false, "String")]
        public static string M_TellerName = "TellerName";
        public string TellerName
        {
            get
            {
                return this._TellerName;
            }
            set
            {
                this._TellerName = value;
            }
        }


        private string _Type;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("Type", "System.String", false, false, false, "String")]
        public static string M_Type = "Type";
        public string Type
        {
            get
            {
                return this._Type;
            }
            set
            {
                this._Type = value;
            }
        }


        private string _AddressCountry;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("AddressCountry", "System.String", false, false, false, "String")]
        public static string M_AddressCountry = "AddressCountry";
        public string AddressCountry
        {
            get
            {
                return this._AddressCountry;
            }
            set
            {
                this._AddressCountry = value;
            }
        }


        private string _ConnectedPartyType;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("ConnectedPartyType", "System.String", false, false, false, "String")]
        public static string M_ConnectedPartyType = "ConnectedPartyType";
        public string ConnectedPartyType
        {
            get
            {
                return this._ConnectedPartyType;
            }
            set
            {
                this._ConnectedPartyType = value;
            }
        }


        private string _ConnectedPartySubType;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("ConnectedPartySubType", "System.String", false, false, false, "String")]
        public static string M_ConnectedPartySubType = "ConnectedPartySubType";
        public string ConnectedPartySubType
        {
            get
            {
                return this._ConnectedPartySubType;
            }
            set
            {
                this._ConnectedPartySubType = value;
            }
        }

        //20211221 AML NOVA 功能需求程式碼,註解保留 start by Ares Dennis
        //private string _Gender;
        ///// <summary>
        ///// IdentityKey
        ///// </summary>
        //[AttributeField("Gender", "System.String", false, false, false, "String")]
        //public static string M_Gender = "Gender";
        //public string Gender
        //{
        //    get
        //    {
        //        return this._Gender;
        //    }
        //    set
        //    {
        //        this._Gender = value;
        //    }
        //}

        //private string _CustType;
        ///// <summary>
        ///// IdentityKey
        ///// </summary>
        //[AttributeField("CustType", "System.String", false, false, false, "String")]
        //public static string M_CustType = "CustType";
        //public string CustType
        //{
        //    get
        //    {
        //        return this._CustType;
        //    }
        //    set
        //    {
        //        this._CustType = value;
        //    }
        //}


        //private string _BroadNameSearch;
        ///// <summary>
        ///// IdentityKey
        ///// </summary>
        //[AttributeField("BroadNameSearch", "System.String", false, false, false, "String")]
        //public static string M_CBroadNameSearch = "BroadNameSearch";
        //public string BroadNameSearch
        //{
        //    get
        //    {
        //        return this._BroadNameSearch;
        //    }
        //    set
        //    {
        //        this._BroadNameSearch = value;
        //    }
        //}
        //20211221 AML NOVA 功能需求程式碼,註解保留 end by Ares Dennis

        private string _MatchedResult;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("MatchedResult", "System.String", false, false, false, "String")]
        public static string M_MatchedResult = "MatchedResult";
        public string MatchedResult
        {
            get
            {
                return this._MatchedResult;
            }
            set
            {
                this._MatchedResult = value;
            }
        }


        private string _RCScore;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("RCScore", "System.String", false, false, false, "String")]
        public static string M_RCScore = "RCScore";
        public string RCScore
        {
            get
            {
                return this._RCScore;
            }
            set
            {
                this._RCScore = value;
            }
        }


        private string _ReferenceNumber;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("ReferenceNumber", "System.String", false, false, false, "String")]
        public static string M_ReferenceNumber = "ReferenceNumber";
        public string ReferenceNumber
        {
            get
            {
                return this._ReferenceNumber;
            }
            set
            {
                this._ReferenceNumber = value;
            }
        }


        private string _AMLReferenceNumber;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("AMLReferenceNumber", "System.String", false, false, false, "String")]
        public static string M_AMLReferenceNumber = "AMLReferenceNumber";
        public string AMLReferenceNumber
        {
            get
            {
                return this._AMLReferenceNumber;
            }
            set
            {
                this._AMLReferenceNumber = value;
            }
        }


        private string _ERRORDESC;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("ERRORDESC", "System.String", false, false, false, "String")]
        public static string M_ERRORDESC = "ERRORDESC";
        public string ERRORDESC
        {
            get
            {
                return this._ERRORDESC;
            }
            set
            {
                this._ERRORDESC = value;
            }
        }


        private string _KIND;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("KIND", "System.String", false, true, false, "String")]
        public static string M_KIND = "KIND";
        public string KIND
        {
            get
            {
                return this._KIND;
            }
            set
            {
                this._KIND = value;
            }
        }


        private string _Query_datetime;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("Query_datetime", "System.DateTime", false, false, false, "DateTime")]
        public static string M_Query_datetime = "Query_datetime";
        public string Query_datetime
        {
            get
            {
                return this._Query_datetime;
            }
            set
            {
                this._Query_datetime = value;
            }
        }


        private string _ReQueryFlg;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("ReQueryFlg", "System.String", false, false, false, "String")]
        public static string M_ReQueryFlg = "ReQueryFlg";
        public string ReQueryFlg
        {
            get
            {
                return this._ReQueryFlg;
            }
            set
            {
                this._ReQueryFlg = value;
            }
        }


        private string _CRE_USER;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("CRE_USER", "System.String", false, false, false, "String")]
        public static string M_CRE_USER = "CRE_USER";
        public string CRE_USER
        {
            get
            {
                return this._CRE_USER;
            }
            set
            {
                this._CRE_USER = value;
            }
        }


        private string _CRE_DATE;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("CRE_DATE", "System.DateTime", false, false, false, "DateTime")]
        public static string M_CRE_DATE = "CRE_DATE";
        public string CRE_DATE
        {
            get
            {
                return this._CRE_DATE;
            }
            set
            {
                this._CRE_DATE = value;
            }
        }


        private string _MOD_USER;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("MOD_USER", "System.String", false, false, false, "String")]
        public static string M_MOD_USER = "MOD_USER";
        public string MOD_USER
        {
            get
            {
                return this._MOD_USER;
            }
            set
            {
                this._MOD_USER = value;
            }
        }


        private string _MOD_DATE;
        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("MOD_DATE", "System.DateTime", false, false, false, "DateTime")]
        public static string M_MOD_DATE = "MOD_DATE";
        public string MOD_DATE
        {
            get
            {
                return this._MOD_DATE;
            }
            set
            {
                this._MOD_DATE = value;
            }
        }
    }

    /// <summary>
    /// EntityEntity_NAMECHECKLOG
    /// </summary>
    [Serializable()]
    public class Entity_NAMECHECKLOGSet : EntitySet<Entity_NAMECHECKLOG>
    {
    }

    public class Entity_ESBNameCheck_DateObj : Entity
    {
        private string _CHINESE_NAME;
        public string CHINESE_NAME
        {
            get
            {
                return this._CHINESE_NAME;
            }
            set
            {
                this._CHINESE_NAME = value;
            }
        }

        private string _ENGLISH_NAME;
        public string ENGLISH_NAME
        {
            get
            {
                return this._ENGLISH_NAME;
            }
            set
            {
                this._ENGLISH_NAME = value;
            }
        }

        //20220118 AML NOVA 功能需求程式碼,註解保留 start by Ares Jack
        //private string _ENGLISH_NAME2;
        //public string ENGLISH_NAME2
        //{
        //    get
        //    {
        //        return this._ENGLISH_NAME2;
        //    }
        //    set
        //    {
        //        this._ENGLISH_NAME2 = value;
        //    }
        //}
        //20220118 AML NOVA 功能需求程式碼,註解保留 end by Ares Jack

        private string _ID;
        public string ID
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

        private string _BIRTH_DATE;
        public string BIRTH_DATE
        {
            get
            {
                return this._BIRTH_DATE;
            }
            set
            {
                this._BIRTH_DATE = value;
            }
        }

        private string _BATCH_NO;
        public string BATCH_NO
        {
            get
            {
                return this._BATCH_NO;
            }
            set
            {
                this._BATCH_NO = value;
            }
        }

        private string _NATION;
        public string NATION
        {
            get
            {
                return this._NATION;
            }
            set
            {
                this._NATION = value;
            }
        }

        private string _ITEM;
        public string ITEM
        {
            get
            {
                return this._ITEM;
            }
            set
            {
                this._ITEM = value;
            }
        }

        private string _ROW_NO;
        public string ROW_NO
        {
            get
            {
                return this._ROW_NO;
            }
            set
            {
                this._ROW_NO = value;
            }
        }

        private string _ConnectedPartySubType;
        public string ConnectedPartySubType
        {
            get
            {
                return this._ConnectedPartySubType;
            }
            set
            {
                this._ConnectedPartySubType = value;
            }
        }

        private string _ConnectedPartyType;
        public string ConnectedPartyType
        {
            get
            {
                return this._ConnectedPartyType;
            }
            set
            {
                this._ConnectedPartyType = value;
            }
        }

        private string _ESB_TYPE;
        public string ESB_TYPE
        {
            get
            {
                return this._ESB_TYPE;
            }
            set
            {
                this._ESB_TYPE = value;
            }
        }

        //記載總公司統編//20200926-RQ-2020-021027-002

        private string _HCOP_CorpNO;
        public string HCOP_CorpNO
        {
            get
            {
                return this._HCOP_CorpNO;
            }
            set
            {
                this._HCOP_CorpNO = value;
            }
        }
    }
}