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
    [AttributeTable("AML_Cdata_Work")]
    public class EntityAML_Cdata_Work_edit : Entity
    {
        public EntityAML_Cdata_Work_edit()
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
        private string _Datadate;
        /// <summary>
        /// Datadate
        /// </summary>
        [AttributeRfPage("txtDatadate", "CustTextBox", false)]
        public string Datadate
        {
            get
            {
                return this._Datadate;
            }
            set
            {
                this._Datadate = value;
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
        private string _AMLRiskRanking;
        /// <summary>
        /// AMLRiskRanking
        /// </summary>
        [AttributeRfPage("txtAMLRiskRanking", "CustTextBox", false)]
        public string AMLRiskRanking
        {
            get
            {
                return this._AMLRiskRanking;
            }
            set
            {
                this._AMLRiskRanking = value;
            }
        }
        private string _AMLNextReviewDate;
        /// <summary>
        /// AMLNextReviewDate
        /// </summary>
        [AttributeRfPage("txtAMLNextReviewDate", "CustTextBox", false)]
        public string AMLNextReviewDate
        {
            get
            {
                return this._AMLNextReviewDate;
            }
            set
            {
                this._AMLNextReviewDate = value;
            }
        }
        private string _BlackListHitFlag;
        /// <summary>
        /// BlackListHitFlag
        /// </summary>
        [AttributeRfPage("txtBlackListHitFlag", "CustTextBox", false)]
        public string BlackListHitFlag
        {
            get
            {
                return this._BlackListHitFlag;
            }
            set
            {
                this._BlackListHitFlag = value;
            }
        }
        private string _PEPListHitFlag;
        /// <summary>
        /// PEPListHitFlag
        /// </summary>
        [AttributeRfPage("txtPEPListHitFlag", "CustTextBox", false)]
        public string PEPListHitFlag
        {
            get
            {
                return this._PEPListHitFlag;
            }
            set
            {
                this._PEPListHitFlag = value;
            }
        }
        private string _NNListHitFlag;
        /// <summary>
        /// NNListHitFlag
        /// </summary>
        [AttributeRfPage("txtNNListHitFlag", "CustTextBox", false)]
        public string NNListHitFlag
        {
            get
            {
                return this._NNListHitFlag;
            }
            set
            {
                this._NNListHitFlag = value;
            }
        }
        private string _Incorporated;
        /// <summary>
        /// Incorporated
        /// </summary>
        [AttributeRfPage("txtIncorporated", "CustTextBox", false)]
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
        private string _IncorporatedDate;
        /// <summary>
        /// IncorporatedDate
        /// </summary>
        [AttributeRfPage("txtIncorporatedDate", "CustTextBox", false)]
        public string IncorporatedDate
        {
            get
            {
                return this._IncorporatedDate;
            }
            set
            {
                this._IncorporatedDate = value;
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
        private string _LastUpdateChecker;
        /// <summary>
        /// LastUpdateChecker
        /// </summary>
        [AttributeRfPage("txtLastUpdateChecker", "CustTextBox", false)]
        public string LastUpdateChecker
        {
            get
            {
                return this._LastUpdateChecker;
            }
            set
            {
                this._LastUpdateChecker = value;
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
        private string _LastUpdateSourceSystem;
        /// <summary>
        /// LastUpdateSourceSystem
        /// </summary>
        [AttributeRfPage("txtLastUpdateSourceSystem", "CustTextBox", false)]
        public string LastUpdateSourceSystem
        {
            get
            {
                return this._LastUpdateSourceSystem;
            }
            set
            {
                this._LastUpdateSourceSystem = value;
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
        private string _Reason;
        /// <summary>
        /// Reason
        /// </summary>
        [AttributeRfPage("txtReason", "CustTextBox", false)]
        public string Reason
        {
            get
            {
                return this._Reason;
            }
            set
            {
                this._Reason = value;
            }
        }
        private string _WarningFlag;
        /// <summary>
        /// WarningFlag
        /// </summary>
        [AttributeRfPage("txtWarningFlag", "CustTextBox", false)]
        public string WarningFlag
        {
            get
            {
                return this._WarningFlag;
            }
            set
            {
                this._WarningFlag = value;
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
        private string _CreditCardBlockCode;
        /// <summary>
        /// CreditCardBlockCode
        /// </summary>
        [AttributeRfPage("txtCreditCardBlockCode", "CustTextBox", false)]
        public string CreditCardBlockCode
        {
            get
            {
                return this._CreditCardBlockCode;
            }
            set
            {
                this._CreditCardBlockCode = value;
            }
        }
        private string _InternationalOrgPEP;
        /// <summary>
        /// InternationalOrgPEP
        /// </summary>
        [AttributeRfPage("txtInternationalOrgPEP", "CustTextBox", false)]
        public string InternationalOrgPEP
        {
            get
            {
                return this._InternationalOrgPEP;
            }
            set
            {
                this._InternationalOrgPEP = value;
            }
        }
        private string _DomesticPEP;
        /// <summary>
        /// DomesticPEP
        /// </summary>
        [AttributeRfPage("txtDomesticPEP", "CustTextBox", false)]
        public string DomesticPEP
        {
            get
            {
                return this._DomesticPEP;
            }
            set
            {
                this._DomesticPEP = value;
            }
        }
        private string _ForeignPEPStakeholder;
        /// <summary>
        /// ForeignPEPStakeholder
        /// </summary>
        [AttributeRfPage("txtForeignPEPStakeholder", "CustTextBox", false)]
        public string ForeignPEPStakeholder
        {
            get
            {
                return this._ForeignPEPStakeholder;
            }
            set
            {
                this._ForeignPEPStakeholder = value;
            }
        }
        private string _InternationalOrgPEPStakeholder;
        /// <summary>
        /// InternationalOrgPEPStakeholder
        /// </summary>
        [AttributeRfPage("txtInternationalOrgPEPStakeholder", "CustTextBox", false)]
        public string InternationalOrgPEPStakeholder
        {
            get
            {
                return this._InternationalOrgPEPStakeholder;
            }
            set
            {
                this._InternationalOrgPEPStakeholder = value;
            }
        }
        private string _DomesticPEPStakeholder;
        /// <summary>
        /// DomesticPEPStakeholder
        /// </summary>
        [AttributeRfPage("txtDomesticPEPStakeholder", "CustTextBox", false)]
        public string DomesticPEPStakeholder
        {
            get
            {
                return this._DomesticPEPStakeholder;
            }
            set
            {
                this._DomesticPEPStakeholder = value;
            }
        }
        private string _GroupInformationSharingNameListflag;
        /// <summary>
        /// GroupInformationSharingNameListflag
        /// </summary>
        [AttributeRfPage("txtGroupInformationSharingNameListflag", "CustTextBox", false)]
        public string GroupInformationSharingNameListflag
        {
            get
            {
                return this._GroupInformationSharingNameListflag;
            }
            set
            {
                this._GroupInformationSharingNameListflag = value;
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
        /// <summary>
          /// 模式切換
          /// </summary>
          /// <returns></returns>
        public EntityAML_Cdata_Work toShowMode()
        {
            EntityAML_Cdata_Work returnObj = new EntityAML_Cdata_Work();
            DataTableConvertor.Clone2Other<EntityAML_Cdata_Work_edit, EntityAML_Cdata_Work>(this, ref returnObj);
            return returnObj;
        }

    }
}