﻿//------------------------------------------------------------------------------
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


namespace CSIPNewInvoice.EntityLayer_new {

    /// <summary>
    /// AML_BRCH_Work
    /// </summary>
    [Serializable()]
    [AttributeTable("AML_BRCH_Work")]
    public class EntityAML_BRCH_Work_edit : Entity
    {
        public EntityAML_BRCH_Work_edit()
        {
            
        }
        private string _ID;
        /// <summary>
        /// ID
        /// </summary>
        [AttributeRfPage("txtID", "CustTextBox", false)]
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
        private string _CASE_NO;
        /// <summary>
        /// CASE_NO
        /// </summary>
        [AttributeRfPage("txtCASE_NO", "CustTextBox", false)]
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
        private string _FileName;
        /// <summary>
        /// FileName
        /// </summary>
        [AttributeRfPage("txtFileName", "CustTextBox", false)]
        public string FileName
        {
            get
            {
                return this._FileName;
            }
            set
            {
                this._FileName = value;
            }
        }
        private string _BRCH_BATCH_NO;
        /// <summary>
        /// BRCH_BATCH_NO
        /// </summary>
        [AttributeRfPage("txtBRCH_BATCH_NO", "CustTextBox", false)]
        public string BRCH_BATCH_NO
        {
            get
            {
                return this._BRCH_BATCH_NO;
            }
            set
            {
                this._BRCH_BATCH_NO = value;
            }
        }
        private string _BRCH_INTER_ID;
        /// <summary>
        /// BRCH_INTER_ID
        /// </summary>
        [AttributeRfPage("txtBRCH_INTER_ID", "CustTextBox", false)]
        public string BRCH_INTER_ID
        {
            get
            {
                return this._BRCH_INTER_ID;
            }
            set
            {
                this._BRCH_INTER_ID = value;
            }
        }
        private string _BRCH_SIXM_TOT_AMT;
        /// <summary>
        /// BRCH_SIXM_TOT_AMT
        /// </summary>
        [AttributeRfPage("txtBRCH_SIXM_TOT_AMT", "CustTextBox", false)]
        public string BRCH_SIXM_TOT_AMT
        {
            get
            {
                return this._BRCH_SIXM_TOT_AMT;
            }
            set
            {
                this._BRCH_SIXM_TOT_AMT = value;
            }
        }
        private string _BRCH_MON_AMT1;
        /// <summary>
        /// BRCH_MON_AMT1
        /// </summary>
        [AttributeRfPage("txtBRCH_MON_AMT1", "CustTextBox", false)]
        public string BRCH_MON_AMT1
        {
            get
            {
                return this._BRCH_MON_AMT1;
            }
            set
            {
                this._BRCH_MON_AMT1 = value;
            }
        }
        private string _BRCH_MON_AMT2;
        /// <summary>
        /// BRCH_MON_AMT2
        /// </summary>
        [AttributeRfPage("txtBRCH_MON_AMT2", "CustTextBox", false)]
        public string BRCH_MON_AMT2
        {
            get
            {
                return this._BRCH_MON_AMT2;
            }
            set
            {
                this._BRCH_MON_AMT2 = value;
            }
        }
        private string _BRCH_MON_AMT3;
        /// <summary>
        /// BRCH_MON_AMT3
        /// </summary>
        [AttributeRfPage("txtBRCH_MON_AMT3", "CustTextBox", false)]
        public string BRCH_MON_AMT3
        {
            get
            {
                return this._BRCH_MON_AMT3;
            }
            set
            {
                this._BRCH_MON_AMT3 = value;
            }
        }
        private string _BRCH_MON_AMT4;
        /// <summary>
        /// BRCH_MON_AMT4
        /// </summary>
        [AttributeRfPage("txtBRCH_MON_AMT4", "CustTextBox", false)]
        public string BRCH_MON_AMT4
        {
            get
            {
                return this._BRCH_MON_AMT4;
            }
            set
            {
                this._BRCH_MON_AMT4 = value;
            }
        }
        private string _BRCH_MON_AMT5;
        /// <summary>
        /// BRCH_MON_AMT5
        /// </summary>
        [AttributeRfPage("txtBRCH_MON_AMT5", "CustTextBox", false)]
        public string BRCH_MON_AMT5
        {
            get
            {
                return this._BRCH_MON_AMT5;
            }
            set
            {
                this._BRCH_MON_AMT5 = value;
            }
        }
        private string _BRCH_MON_AMT6;
        /// <summary>
        /// BRCH_MON_AMT6
        /// </summary>
        [AttributeRfPage("txtBRCH_MON_AMT6", "CustTextBox", false)]
        public string BRCH_MON_AMT6
        {
            get
            {
                return this._BRCH_MON_AMT6;
            }
            set
            {
                this._BRCH_MON_AMT6 = value;
            }
        }
        private string _BRCH_MON_AMT7;
        /// <summary>
        /// BRCH_MON_AMT7
        /// </summary>
        [AttributeRfPage("txtBRCH_MON_AMT7", "CustTextBox", false)]
        public string BRCH_MON_AMT7
        {
            get
            {
                return this._BRCH_MON_AMT7;
            }
            set
            {
                this._BRCH_MON_AMT7 = value;
            }
        }
        private string _BRCH_MON_AMT8;
        /// <summary>
        /// BRCH_MON_AMT8
        /// </summary>
        [AttributeRfPage("txtBRCH_MON_AMT8", "CustTextBox", false)]
        public string BRCH_MON_AMT8
        {
            get
            {
                return this._BRCH_MON_AMT8;
            }
            set
            {
                this._BRCH_MON_AMT8 = value;
            }
        }
        private string _BRCH_MON_AMT9;
        /// <summary>
        /// BRCH_MON_AMT9
        /// </summary>
        [AttributeRfPage("txtBRCH_MON_AMT9", "CustTextBox", false)]
        public string BRCH_MON_AMT9
        {
            get
            {
                return this._BRCH_MON_AMT9;
            }
            set
            {
                this._BRCH_MON_AMT9 = value;
            }
        }
        private string _BRCH_MON_AMT10;
        /// <summary>
        /// BRCH_MON_AMT10
        /// </summary>
        [AttributeRfPage("txtBRCH_MON_AMT10", "CustTextBox", false)]
        public string BRCH_MON_AMT10
        {
            get
            {
                return this._BRCH_MON_AMT10;
            }
            set
            {
                this._BRCH_MON_AMT10 = value;
            }
        }
        private string _BRCH_MON_AMT11;
        /// <summary>
        /// BRCH_MON_AMT11
        /// </summary>
        [AttributeRfPage("txtBRCH_MON_AMT11", "CustTextBox", false)]
        public string BRCH_MON_AMT11
        {
            get
            {
                return this._BRCH_MON_AMT11;
            }
            set
            {
                this._BRCH_MON_AMT11 = value;
            }
        }
        private string _BRCH_MON_AMT12;
        /// <summary>
        /// BRCH_MON_AMT12
        /// </summary>
        [AttributeRfPage("txtBRCH_MON_AMT12", "CustTextBox", false)]
        public string BRCH_MON_AMT12
        {
            get
            {
                return this._BRCH_MON_AMT12;
            }
            set
            {
                this._BRCH_MON_AMT12 = value;
            }
        }
        private string _BRCH_KEY;
        /// <summary>
        /// BRCH_KEY
        /// </summary>
        [AttributeRfPage("txtBRCH_KEY", "CustTextBox", false)]
        public string BRCH_KEY
        {
            get
            {
                return this._BRCH_KEY;
            }
            set
            {
                this._BRCH_KEY = value;
            }
        }
        private string _BRCH_BRCH_NO;
        /// <summary>
        /// BRCH_BRCH_NO
        /// </summary>
        [AttributeRfPage("BrlblBRCH_BRCH_NO", "CustLabel", false)]
        public string BRCH_BRCH_NO
        {
            get
            {
                return this._BRCH_BRCH_NO;
            }
            set
            {
                this._BRCH_BRCH_NO = value;
            }
        }
        private string _BRCH_BRCH_SEQ;
        /// <summary>
        /// BRCH_BRCH_SEQ
        /// </summary>
        [AttributeRfPage("txtBRCH_BRCH_SEQ", "CustTextBox", false)]
        public string BRCH_BRCH_SEQ
        {
            get
            {
                return this._BRCH_BRCH_SEQ;
            }
            set
            {
                this._BRCH_BRCH_SEQ = value;
            }
        }
        private string _BRCH_BRCH_TYPE;
        /// <summary>
        /// BRCH_BRCH_TYPE
        /// </summary>
        [AttributeRfPage("txtBRCH_BRCH_TYPE", "CustTextBox", false)]
        public string BRCH_BRCH_TYPE
        {
            get
            {
                return this._BRCH_BRCH_TYPE;
            }
            set
            {
                this._BRCH_BRCH_TYPE = value;
            }
        }
        private string _BRCH_NATION;
        /// <summary>
        /// BRCH_NATION  
        /// </summary>
        [AttributeRfPage("txtBRCH_NATION", "CustTextBox", false)]
        [AttributeValidPage(true, "國籍", true,2, "國籍", true, "CT_1_", "註冊國籍")]
        public string BRCH_NATION
        {
            get
            {
                return this._BRCH_NATION;
            }
            set
            {
                this._BRCH_NATION = value;
            }
        }
        private string _BRCH_BIRTH_DATE;
        /// <summary>
        /// BRCH_BIRTH_DATE
        /// </summary>
        //[AttributeRfPage("BrlblBRCH_BIRTH_DATE", "CustLabel", false)]
        [AttributeRfPage("txtBRCH_BIRTH_DATE", "CustTextBox", false)]
        public string BRCH_BIRTH_DATE
        {
            get
            {
                return this._BRCH_BIRTH_DATE;
            }
            set
            {
                this._BRCH_BIRTH_DATE = value;
            }
        }
        private string _BRCH_PERM_CITY;
        /// <summary>
        /// BRCH_PERM_CITY
        /// </summary>
        [AttributeRfPage("txtBRCH_PERM_CITY", "CustTextBox", false)]
        public string BRCH_PERM_CITY
        {
            get
            {
                return this._BRCH_PERM_CITY;
            }
            set
            {
                this._BRCH_PERM_CITY = value;
            }
        }
        private string _BRCH_PERM_ADDR1;
        /// <summary>
        /// BRCH_PERM_ADDR1
        /// </summary>
        [AttributeRfPage("txtBRCH_PERM_ADDR1", "CustTextBox", false)]
        public string BRCH_PERM_ADDR1
        {
            get
            {
                return this._BRCH_PERM_ADDR1;
            }
            set
            {
                this._BRCH_PERM_ADDR1 = value;
            }
        }
        private string _BRCH_PERM_ADDR2;
        /// <summary>
        /// BRCH_PERM_ADDR2
        /// </summary>
        [AttributeRfPage("txtBRCH_PERM_ADDR2", "CustTextBox", false)]
        public string BRCH_PERM_ADDR2
        {
            get
            {
                return this._BRCH_PERM_ADDR2;
            }
            set
            {
                this._BRCH_PERM_ADDR2 = value;
            }
        }
        private string _BRCH_CHINESE_NAME;
        /// <summary>
        /// BRCH_CHINESE_NAME
        /// </summary>
        [AttributeRfPage("BrlblBRCH_CHINESE_NAME", "CustLabel", false)]
        public string BRCH_CHINESE_NAME
        {
            get
            {
                return this._BRCH_CHINESE_NAME;
            }
            set
            {
                this._BRCH_CHINESE_NAME = value;
            }
        }
        private string _BRCH_ENGLISH_NAME;
        /// <summary>
        /// BRCH_ENGLISH_NAME
        /// </summary>
        [AttributeRfPage("BrlblBRCH_ENGLISH_NAME", "CustLabel", false)]
        public string BRCH_ENGLISH_NAME
        {
            get
            {
                return this._BRCH_ENGLISH_NAME;
            }
            set
            {
                this._BRCH_ENGLISH_NAME = value;
            }
        }
        private string _BRCH_ID;
        /// <summary>
        /// BRCH_ID
        /// </summary>
        [AttributeRfPage("BrlblBRCH_ID", "CustLabel", false)]
        [AttributeValidPage(false, "身分證字號", true, 10, "身分證字號")]
        public string BRCH_ID
        {
            get
            {
                return this._BRCH_ID;
            }
            set
            {
                this._BRCH_ID = value;
            }
        }
        private string _BRCH_OWNER_ID_ISSUE_DATE;
        /// <summary>
        /// BRCH_OWNER_ID_ISSUE_DATE 注意 ! 檢核以存檔檢核，故此欄位須設定為西元年
        /// </summary>
        [AttributeRfPage("txtBRCH_OWNER_ID_ISSUE_DATE", "CustTextBox", false)]
        [AttributeValidPage(false, "", true, 8, "身分證發證日期",true, "isDateTime", "身分證發證日期")]
        public string BRCH_OWNER_ID_ISSUE_DATE
        {
            get
            {
                return this._BRCH_OWNER_ID_ISSUE_DATE;
            }
            set
            {
                this._BRCH_OWNER_ID_ISSUE_DATE = value;
            }
        }
        private string _BRCH_OWNER_ID_ISSUE_PLACE;
        /// <summary>
        /// BRCH_OWNER_ID_ISSUE_PLACE
        /// </summary>
        [AttributeRfPage("txtBRCH_OWNER_ID_ISSUE_PLACE", "CustTextBox", false)]
        [AttributeValidPage(false, "", true, 10, "換證地點")]
        public string BRCH_OWNER_ID_ISSUE_PLACE
        {
            get
            {
                return this._BRCH_OWNER_ID_ISSUE_PLACE;
            }
            set
            {
                this._BRCH_OWNER_ID_ISSUE_PLACE = value;
            }
        }
        private string _BRCH_OWNER_ID_REPLACE_TYPE;
        /// <summary>
        /// BRCH_OWNER_ID_REPLACE_TYPE
        /// </summary>
        [AttributeRfPage("txtBRCH_OWNER_ID_REPLACE_TYPE", "CustTextBox", false)]
        [AttributeValidPage(false, "", true, 1, "領補換", true, "CT_4_", "領補換")]
        public string BRCH_OWNER_ID_REPLACE_TYPE
        {
            get
            {
                return this._BRCH_OWNER_ID_REPLACE_TYPE;
            }
            set
            {
                this._BRCH_OWNER_ID_REPLACE_TYPE = value;
            }
        }
        private string _BRCH_ID_PHOTO_FLAG;
        /// <summary>
        /// BRCH_ID_PHOTO_FLAG
        /// </summary>
        [AttributeRfPage("txtBRCH_ID_PHOTO_FLAG", "CustTextBox", false)]
        [AttributeValidPage(false, "", true, 1, "有無照片", true, "LM_10_", "有無照片")]
        public string BRCH_ID_PHOTO_FLAG
        {
            get
            {
                return this._BRCH_ID_PHOTO_FLAG;
            }
            set
            {
                this._BRCH_ID_PHOTO_FLAG = value;
            }
        }
        private string _BRCH_PASSPORT;
        /// <summary>
        /// BRCH_PASSPORT
        /// </summary>
        [AttributeRfPage("BrlblBRCH_PASSPORT", "CustLabel", false)]
        public string BRCH_PASSPORT
        {
            get
            {
                return this._BRCH_PASSPORT;
            }
            set
            {
                this._BRCH_PASSPORT = value;
            }
        }
        private string _BRCH_PASSPORT_EXP_DATE;
        /// <summary>
        /// BRCH_PASSPORT_EXP_DATE ! 檢核以存檔檢核，故此欄位須設定為西元年
        /// </summary>
        [AttributeRfPage("txtBRCH_PASSPORT_EXP_DATE", "CustTextBox", false)]
        [AttributeValidPage(false, "", true, 8, "護照到期日",true, "isDateTime", "護照到期日")]
        public string BRCH_PASSPORT_EXP_DATE
        {
            get
            {
                return this._BRCH_PASSPORT_EXP_DATE;
            }
            set
            {
                this._BRCH_PASSPORT_EXP_DATE = value;
            }
        }
        private string _BRCH_RESIDENT_NO;
        /// <summary>
        /// BRCH_RESIDENT_NO
        /// </summary>
        [AttributeRfPage("BrlblBRCH_RESIDENT_NO", "CustLabel", false)]
        public string BRCH_RESIDENT_NO
        {
            get
            {
                return this._BRCH_RESIDENT_NO;
            }
            set
            {
                this._BRCH_RESIDENT_NO = value;
            }
        }
        private string _BRCH_RESIDENT_EXP_DATE;
        /// <summary>
        /// BRCH_RESIDENT_EXP_DATE ! 檢核以存檔檢核，故此欄位須設定為西元年
        /// 20200410-RQ-2019-030155-005-居留證號更名為統一證號
        /// </summary>
        [AttributeRfPage("txtBRCH_RESIDENT_EXP_DATE", "CustTextBox", false)]
        //[AttributeValidPage(false, "", true, 8, "居留證到期日", true, "isDateTime", "居留證到期日")]
        [AttributeValidPage(false, "", true, 8, "統一證號到期日", true, "isDateTime", "統一證號到期日")]
        public string BRCH_RESIDENT_EXP_DATE
        {
            get
            {
                return this._BRCH_RESIDENT_EXP_DATE;
            }
            set
            {
                this._BRCH_RESIDENT_EXP_DATE = value;
            }
        }
        private string _BRCH_OTHER_CERT;
        /// <summary>
        /// BRCH_OTHER_CERT
        /// </summary>
        [AttributeRfPage("txtBRCH_OTHER_CERT", "CustTextBox", false)]
        public string BRCH_OTHER_CERT
        {
            get
            {
                return this._BRCH_OTHER_CERT;
            }
            set
            {
                this._BRCH_OTHER_CERT = value;
            }
        }
        private string _BRCH_OTHER_CERT_EXP_DATE;
        /// <summary>
        /// BRCH_OTHER_CERT_EXP_DATE
        /// </summary>
        [AttributeRfPage("txtBRCH_OTHER_CERT_EXP_DATE", "CustTextBox", false)]
        public string BRCH_OTHER_CERT_EXP_DATE
        {
            get
            {
                return this._BRCH_OTHER_CERT_EXP_DATE;
            }
            set
            {
                this._BRCH_OTHER_CERT_EXP_DATE = value;
            }
        }
        private string _BRCH_COMP_TEL;
        /// <summary>
        /// BRCH_COMP_TEL
        /// </summary>
        [AttributeRfPage("txtBRCH_COMP_TEL", "CustTextBox", false)]
        public string BRCH_COMP_TEL
        {
            get
            {
                return this._BRCH_COMP_TEL;
            }
            set
            {
                this._BRCH_COMP_TEL = value;
            }
        }
        private string _BRCH_CREATE_DATE;
        /// <summary>
        /// BRCH_CREATE_DATE
        /// </summary>
        [AttributeRfPage("txtBRCH_CREATE_DATE", "CustTextBox", false)]
        public string BRCH_CREATE_DATE
        {
            get
            {
                return this._BRCH_CREATE_DATE;
            }
            set
            {
                this._BRCH_CREATE_DATE = value;
            }
        }
        private string _BRCH_STATUS;
        /// <summary>
        /// BRCH_STATUS
        /// </summary>
        [AttributeRfPage("txtBRCH_STATUS", "CustTextBox", false)]
        public string BRCH_STATUS
        {
            get
            {
                return this._BRCH_STATUS;
            }
            set
            {
                this._BRCH_STATUS = value;
            }
        }
        private string _BRCH_CIRCULATE_MERCH;
        /// <summary>
        /// BRCH_CIRCULATE_MERCH
        /// </summary>
        [AttributeRfPage("txtBRCH_CIRCULATE_MERCH", "CustTextBox", false)]
        public string BRCH_CIRCULATE_MERCH
        {
            get
            {
                return this._BRCH_CIRCULATE_MERCH;
            }
            set
            {
                this._BRCH_CIRCULATE_MERCH = value;
            }
        }
        private string _BRCH_HQ_BRCH_NO;
        /// <summary>
        /// BRCH_HQ_BRCH_NO
        /// </summary>
        [AttributeRfPage("txtBRCH_HQ_BRCH_NO", "CustTextBox", false)]
        public string BRCH_HQ_BRCH_NO
        {
            get
            {
                return this._BRCH_HQ_BRCH_NO;
            }
            set
            {
                this._BRCH_HQ_BRCH_NO = value;
            }
        }
        private string _BRCH_HQ_BRCH_SEQ_NO;
        /// <summary>
        /// BRCH_HQ_BRCH_SEQ_NO
        /// </summary>
        [AttributeRfPage("txtBRCH_HQ_BRCH_SEQ_NO", "CustTextBox", false)]
        public string BRCH_HQ_BRCH_SEQ_NO
        {
            get
            {
                return this._BRCH_HQ_BRCH_SEQ_NO;
            }
            set
            {
                this._BRCH_HQ_BRCH_SEQ_NO = value;
            }
        }
        private string _BRCH_UPDATE_DATE;
        /// <summary>
        /// BRCH_UPDATE_DATE
        /// </summary>
        [AttributeRfPage("txtBRCH_UPDATE_DATE", "CustTextBox", false)]
        public string BRCH_UPDATE_DATE
        {
            get
            {
                return this._BRCH_UPDATE_DATE;
            }
            set
            {
                this._BRCH_UPDATE_DATE = value;
            }
        }
        private string _BRCH_QUALIFY_FLAG;
        /// <summary>
        /// BRCH_QUALIFY_FLAG
        /// </summary>
        [AttributeRfPage("txtBRCH_QUALIFY_FLAG", "CustTextBox", false)]
        public string BRCH_QUALIFY_FLAG
        {
            get
            {
                return this._BRCH_QUALIFY_FLAG;
            }
            set
            {
                this._BRCH_QUALIFY_FLAG = value;
            }
        }
        private string _BRCH_UPDATE_ID;
        /// <summary>
        /// BRCH_UPDATE_ID
        /// </summary>
        [AttributeRfPage("txtBRCH_UPDATE_ID", "CustTextBox", false)]
        public string BRCH_UPDATE_ID
        {
            get
            {
                return this._BRCH_UPDATE_ID;
            }
            set
            {
                this._BRCH_UPDATE_ID = value;
            }
        }
        private string _BRCH_REAL_CORP;
        /// <summary>
        /// BRCH_REAL_CORP
        /// </summary>
        [AttributeRfPage("txtBRCH_REAL_CORP", "CustTextBox", false)]
        public string BRCH_REAL_CORP
        {
            get
            {
                return this._BRCH_REAL_CORP;
            }
            set
            {
                this._BRCH_REAL_CORP = value;
            }
        }

        private string _BRCH_RESERVED_FILLER;
        /// <summary>
        /// BRCH_RESERVED_FILLER
        /// </summary>
        [AttributeRfPage("txtBRCH_RESERVED_FILLER", "CustTextBox", false)]
        public string BRCH_RESERVED_FILLER
        {
            get
            {
                return this._BRCH_RESERVED_FILLER;
            }
            set
            {
                this._BRCH_RESERVED_FILLER = value;
            }
        }
        private string _Create_Time;
        /// <summary>
        /// Create_Time
        /// </summary>
        [AttributeRfPage("txtCreate_Time", "CustTextBox", false)]
        public string Create_Time
        {
            get
            {
                return this._Create_Time;
            }
            set
            {
                this._Create_Time = value;
            }
        }
        private string _Create_User;
        /// <summary>
        /// Create_User
        /// </summary>
        [AttributeRfPage("txtCreate_User", "CustTextBox", false)]
        public string Create_User
        {
            get
            {
                return this._Create_User;
            }
            set
            {
                this._Create_User = value;
            }
        }
        private string _Create_Date;
        /// <summary>
        /// Create_Date
        /// </summary>
        [AttributeRfPage("txtCreate_Date", "CustTextBox", false)]
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
        private string _BRCH_ID_Type;
        /// <summary>
        /// BRCH_ID_Type
        /// </summary>
        [AttributeRfPage("txtBRCH_ID_Type", "CustTextBox", false)]
        public string BRCH_ID_Type
        {
            get
            {
                return this._BRCH_ID_Type;
            }
            set
            {
                this._BRCH_ID_Type = value;
            }
        }
        private string _BRCH_ID_SreachStatus;
        /// <summary>
        /// BRCH_ID_SreachStatus
        /// </summary>
        [AttributeRfPage("txtBRCH_ID_SreachStatus", "CustTextBox", false)]
        [AttributeValidPage(true, "ID換補領查詢結果 ")]
        public string BRCH_ID_SreachStatus
        {
            get
            {
                return this._BRCH_ID_SreachStatus;
            }
            set
            {
                this._BRCH_ID_SreachStatus = value;
            }
        }
        private string _BRCH_ExportFileFlag;
        /// <summary>
        /// BRCH_ExportFileFlag
        /// </summary>
        [AttributeRfPage("txtBRCH_ExportFileFlag", "CustTextBox", false)]
        public string BRCH_ExportFileFlag
        {
            get
            {
                return this._BRCH_ExportFileFlag;
            }
            set
            {
                this._BRCH_ExportFileFlag = value;
            }
        }
        private string _BRCH_LastExportTime;
        /// <summary>
        /// BRCH_LastExportTime
        /// </summary>
        [AttributeRfPage("txtBRCH_LastExportTime", "CustTextBox", false)]
        public string BRCH_LastExportTime
        {
            get
            {
                return this._BRCH_LastExportTime;
            }
            set
            {
                this._BRCH_LastExportTime = value;
            }
        }
        ///編輯用參數
        private string _ArgNo;
     
        public string ArgNo
        {
            get
            {
                return this._ArgNo;
            }
            set
            {
                this._ArgNo = value;
            }
        }
        /// <summary>
        /// 模式切換
        /// </summary>
        /// <returns></returns>
        public EntityAML_BRCH_Work toShowMode()
        {
            EntityAML_BRCH_Work returnObj = new EntityAML_BRCH_Work();
            DataTableConvertor.Clone2Other<EntityAML_BRCH_Work_edit, EntityAML_BRCH_Work>(this, ref returnObj);
            return returnObj;
        }

    }
}