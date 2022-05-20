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


namespace CSIPNewInvoice.EntityLayer_new
{
    /// <summary>
    /// AML_HeadOffice
    /// </summary>
    [Serializable()]
    [AttributeTable("AML_Edata_Work")]
    public class EntityAML_Edata_Work_edit : Entity
    {
        public EntityAML_Edata_Work_edit()
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
        private string _RMMBatchNo;
        /// <summary>
        /// RMMBatchNo
        /// </summary>
        [AttributeRfPage("txtRMMBatchNo", "CustTextBox", false)]
        public string RMMBatchNo
        {
            get
            {
                return this._RMMBatchNo;
            }
            set
            {
                this._RMMBatchNo = value;
            }
        }
        private string _AMLInternalID;
        /// <summary>
        /// AMLInternalID
        /// </summary>
        [AttributeRfPage("txtAMLInternalID", "CustTextBox", false)]
        public string AMLInternalID
        {
            get
            {
                return this._AMLInternalID;
            }
            set
            {
                this._AMLInternalID = value;
            }
        }
        private string _DataDate;
        /// <summary>
        /// DataDate
        /// </summary>
        [AttributeRfPage("txtDataDate", "CustTextBox", false)]
        public string DataDate
        {
            get
            {
                return this._DataDate;
            }
            set
            {
                this._DataDate = value;
            }
        }
        private string _CustomerID;
        /// <summary>
        /// CustomerID
        /// </summary>
        [AttributeRfPage("txtCustomerID", "CustTextBox", false)]
        public string CustomerID
        {
            get
            {
                return this._CustomerID;
            }
            set
            {
                this._CustomerID = value;
            }
        }
        private string _CustomerEnglishName;
        /// <summary>
        /// CustomerEnglishName
        /// </summary>
        [AttributeRfPage("txtCustomerEnglishName", "CustTextBox", false)]
        public string CustomerEnglishName
        {
            get
            {
                return this._CustomerEnglishName;
            }
            set
            {
                this._CustomerEnglishName = value;
            }
        }
        private string _CustomerChineseName;
        /// <summary>
        /// CustomerChineseName
        /// </summary>
        [AttributeRfPage("txtCustomerChineseName", "CustTextBox", false)]
        public string CustomerChineseName
        {
            get
            {
                return this._CustomerChineseName;
            }
            set
            {
                this._CustomerChineseName = value;
            }
        }
        private string _AMLSegment;
        /// <summary>
        /// AMLSegment
        /// </summary>
        [AttributeRfPage("txtAMLSegment", "CustTextBox", false)]
        public string AMLSegment
        {
            get
            {
                return this._AMLSegment;
            }
            set
            {
                this._AMLSegment = value;
            }
        }
        private string _OriginalRiskRanking;
        /// <summary>
        /// OriginalRiskRanking
        /// </summary>
        [AttributeRfPage("txtOriginalRiskRanking", "CustTextBox", false)]
        public string OriginalRiskRanking
        {
            get
            {
                return this._OriginalRiskRanking;
            }
            set
            {
                this._OriginalRiskRanking = value;
            }
        }
        private string _OriginalNextReviewDate;
        /// <summary>
        /// OriginalNextReviewDate
        /// </summary>
        [AttributeRfPage("txtOriginalNextReviewDate", "CustTextBox", false)]
        public string OriginalNextReviewDate
        {
            get
            {
                return this._OriginalNextReviewDate;
            }
            set
            {
                this._OriginalNextReviewDate = value;
            }
        }
        private string _NewRiskRanking;
        /// <summary>
        /// NewRiskRanking
        /// </summary>
        [AttributeRfPage("txtNewRiskRanking", "CustTextBox", false)]
        public string NewRiskRanking
        {
            get
            {
                return this._NewRiskRanking;
            }
            set
            {
                this._NewRiskRanking = value;
            }
        }
        private string _NewNextReviewDate;
        /// <summary>
        /// NewNextReviewDate
        /// </summary>
        [AttributeRfPage("txtNewNextReviewDate", "CustTextBox", false)]
        public string NewNextReviewDate
        {
            get
            {
                return this._NewNextReviewDate;
            }
            set
            {
                this._NewNextReviewDate = value;
            }
        }
        private string _LastUpdateMaker;
        /// <summary>
        /// LastUpdateMaker
        /// </summary>
        [AttributeRfPage("txtLastUpdateMaker", "CustTextBox", false)]
        public string LastUpdateMaker
        {
            get
            {
                return this._LastUpdateMaker;
            }
            set
            {
                this._LastUpdateMaker = value;
            }
        }
        private string _LastUpdateBranch;
        /// <summary>
        /// LastUpdateBranch
        /// </summary>
        [AttributeRfPage("txtLastUpdateBranch", "CustTextBox", false)]
        public string LastUpdateBranch
        {
            get
            {
                return this._LastUpdateBranch;
            }
            set
            {
                this._LastUpdateBranch = value;
            }
        }
        private string _LastUpdateDate;
        /// <summary>
        /// LastUpdateDate
        /// </summary>
        [AttributeRfPage("txtLastUpdateDate", "CustTextBox", false)]
        public string LastUpdateDate
        {
            get
            {
                return this._LastUpdateDate;
            }
            set
            {
                this._LastUpdateDate = value;
            }
        }
        private string _HomeBranch;
        /// <summary>
        /// HomeBranch
        /// </summary>
        [AttributeRfPage("txtHomeBranch", "CustTextBox", false)]
        public string HomeBranch
        {
            get
            {
                return this._HomeBranch;
            }
            set
            {
                this._HomeBranch = value;
            }
        }
        private string _CaseType;
        /// <summary>
        /// CaseType
        /// </summary>
        [AttributeRfPage("txtCaseType", "CustTextBox", false)]
        public string CaseType
        {
            get
            {
                return this._CaseType;
            }
            set
            {
                this._CaseType = value;
            }
        }
        private string _FiledSAR;
        /// <summary>
        /// FiledSAR
        /// </summary>
        [AttributeRfPage("txtFiledSAR", "CustTextBox", false)]
        public string FiledSAR
        {
            get
            {
                return this._FiledSAR;
            }
            set
            {
                this._FiledSAR = value;
            }
        }
        private string _Filler;
        /// <summary>
        /// Filler
        /// </summary>
        [AttributeRfPage("txtFiller", "CustTextBox", false)]
        public string Filler
        {
            get
            {
                return this._Filler;
            }
            set
            {
                this._Filler = value;
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

        private string _AML_ExportFileFlag;
        /// <summary>
        /// CaseProcess_User
        /// </summary>
        [AttributeRfPage("txtAML_ExportFileFlag", "CustTextBox", false)]
        public string AML_ExportFileFlag
        {
            get
            {
                return this._AML_ExportFileFlag;
            }
            set
            {
                this._AML_ExportFileFlag = value;
            }
        }
        private string _AML_LastExportTime;
        /// <summary>
        /// CaseProcess_User
        /// </summary>
        [AttributeRfPage("txtAML_LastExportTime", "CustTextBox", false)]
        public string AML_LastExportTime
        {
            get
            {
                return this._AML_LastExportTime;
            }
            set
            {
                this._AML_LastExportTime = value;
            }
        }

        /// <summary>
        /// 模式切換
        /// </summary>
        /// <returns></returns>
        public EntityAML_Edata_Work toShowMode()
        {
            EntityAML_Edata_Work returnObj = new EntityAML_Edata_Work();
            DataTableConvertor.Clone2Other<EntityAML_Edata_Work_edit, EntityAML_Edata_Work>(this, ref returnObj);
            return returnObj;
        }

    }
}