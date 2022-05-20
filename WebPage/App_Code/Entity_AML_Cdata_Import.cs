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
    [AttributeTable("AML_Cdata_Import")]
    public class Entity_AML_Cdata_Import : Framework.Data.OM.Entity
    {
        private int _ID;
        /// < summary>
        /// ID
        /// < /summary>
        public static string M_ID = "ID";
        private string _FileName;
        /// < summary>
        /// FileName
        /// < /summary>
        public static string M_FileName = "FileName";
        private string _Datadate;
        /// < summary>
        /// Datadate
        /// < /summary>
        public static string M_Datadate = "Datadate";
        private string _CustomerID;
        /// < summary>
        /// CustomerID
        /// < /summary>
        public static string M_CustomerID = "CustomerID";
        private string _CustomerEnglishName;
        /// < summary>
        /// CustomerEnglishName
        /// < /summary>
        public static string M_CustomerEnglishName = "CustomerEnglishName";
        private string _CustomerChineseName;
        /// < summary>
        /// CustomerChineseName
        /// < /summary>
        public static string M_CustomerChineseName = "CustomerChineseName";
        private string _AMLSegment;
        /// < summary>
        /// AMLSegment
        /// < /summary>
        public static string M_AMLSegment = "AMLSegment";
        private string _AMLRiskRanking;
        /// < summary>
        /// AMLRiskRanking
        /// < /summary>
        public static string M_AMLRiskRanking = "AMLRiskRanking";
        private string _AMLNextReviewDate;
        /// < summary>
        /// AMLNextReviewDate
        /// < /summary>
        public static string M_AMLNextReviewDate = "AMLNextReviewDate";
        private string _BlackListHitFlag;
        /// < summary>
        /// BlackListHitFlag
        /// < /summary>
        public static string M_BlackListHitFlag = "BlackListHitFlag";
        private string _PEPListHitFlag;
        /// < summary>
        /// PEPListHitFlag
        /// < /summary>
        public static string M_PEPListHitFlag = "PEPListHitFlag";
        private string _NNListHitFlag;
        /// < summary>
        /// NNListHitFlag
        /// < /summary>
        public static string M_NNListHitFlag = "NNListHitFlag";
        private string _Incorporated;
        /// < summary>
        /// Incorporated
        /// < /summary>
        public static string M_Incorporated = "Incorporated";
        private string _IncorporatedDate;
        /// < summary>
        /// IncorporatedDate
        /// < /summary>
        public static string M_IncorporatedDate = "IncorporatedDate";
        private string _LastUpdateMaker;
        /// < summary>
        /// LastUpdateMaker
        /// < /summary>
        public static string M_LastUpdateMaker = "LastUpdateMaker";
        private string _LastUpdateChecker;
        /// < summary>
        /// LastUpdateChecker
        /// < /summary>
        public static string M_LastUpdateChecker = "LastUpdateChecker";
        private string _LastUpdateBranch;
        /// < summary>
        /// LastUpdateBranch
        /// < /summary>
        public static string M_LastUpdateBranch = "LastUpdateBranch";
        private string _LastUpdateDate;
        /// < summary>
        /// LastUpdateDate
        /// < /summary>
        public static string M_LastUpdateDate = "LastUpdateDate";
        private string _LastUpdateSourceSystem;
        /// < summary>
        /// LastUpdateSourceSystem
        /// < /summary>
        public static string M_LastUpdateSourceSystem = "LastUpdateSourceSystem";
        private string _HomeBranch;
        /// < summary>
        /// HomeBranch
        /// < /summary>
        public static string M_HomeBranch = "HomeBranch";
        private string _Reason;
        /// < summary>
        /// Reason
        /// < /summary>
        public static string M_Reason = "Reason";
        private string _WarningFlag;
        /// < summary>
        /// WarningFlag
        /// < /summary>
        public static string M_WarningFlag = "WarningFlag";
        private string _FiledSAR;
        /// < summary>
        /// FiledSAR
        /// < /summary>
        public static string M_FiledSAR = "FiledSAR";
        private string _CreditCardBlockCode;
        /// < summary>
        /// CreditCardBlockCode
        /// < /summary>
        public static string M_CreditCardBlockCode = "CreditCardBlockCode";
        private string _InternationalOrgPEP;
        /// < summary>
        /// InternationalOrgPEP
        /// < /summary>
        public static string M_InternationalOrgPEP = "InternationalOrgPEP";
        private string _DomesticPEP;
        /// < summary>
        /// DomesticPEP
        /// < /summary>
        public static string M_DomesticPEP = "DomesticPEP";
        private string _ForeignPEPStakeholder;
        /// < summary>
        /// ForeignPEPStakeholder
        /// < /summary>
        public static string M_ForeignPEPStakeholder = "ForeignPEPStakeholder";
        private string _InternationalOrgPEPStakeholder;
        /// < summary>
        /// InternationalOrgPEPStakeholder
        /// < /summary>
        public static string M_InternationalOrgPEPStakeholder = "InternationalOrgPEPStakeholder";
        private string _DomesticPEPStakeholder;
        /// < summary>
        /// DomesticPEPStakeholder
        /// < /summary>
        public static string M_DomesticPEPStakeholder = "DomesticPEPStakeholder";
        private string _GroupInformationSharingNameListflag;
        /// < summary>
        /// GroupInformationSharingNameListflag
        /// < /summary>
        public static string M_GroupInformationSharingNameListflag = "GroupInformationSharingNameListflag";
        private string _Filler;
        /// < summary>
        /// Filler
        /// < /summary>
        public static string M_Filler = "Filler";
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
        /// FileName
        /// </summary>
        [AttributeField("FileName", "System.String", false, false, false, "String")]
        public string FileName
        {
            get
            {
                return this.FileName;
            }
            set
            {
                this._FileName = value;
            }
        }
        /// < summary>
        /// Datadate
        /// </summary>
        [AttributeField("Datadate", "System.String", false, false, false, "String")]
        public string Datadate
        {
            get
            {
                return this.Datadate;
            }
            set
            {
                this._Datadate = value;
            }
        }
        /// < summary>
        /// CustomerID
        /// </summary>
        [AttributeField("CustomerID", "System.String", false, false, false, "String")]
        public string CustomerID
        {
            get
            {
                return this.CustomerID;
            }
            set
            {
                this._CustomerID = value;
            }
        }
        /// < summary>
        /// CustomerEnglishName
        /// </summary>
        [AttributeField("CustomerEnglishName", "System.String", false, false, false, "String")]
        public string CustomerEnglishName
        {
            get
            {
                return this.CustomerEnglishName;
            }
            set
            {
                this._CustomerEnglishName = value;
            }
        }
        /// < summary>
        /// CustomerChineseName
        /// </summary>
        [AttributeField("CustomerChineseName", "System.String", false, false, false, "String")]
        public string CustomerChineseName
        {
            get
            {
                return this.CustomerChineseName;
            }
            set
            {
                this._CustomerChineseName = value;
            }
        }
        /// < summary>
        /// AMLSegment
        /// </summary>
        [AttributeField("AMLSegment", "System.String", false, false, false, "String")]
        public string AMLSegment
        {
            get
            {
                return this.AMLSegment;
            }
            set
            {
                this._AMLSegment = value;
            }
        }
        /// < summary>
        /// AMLRiskRanking
        /// </summary>
        [AttributeField("AMLRiskRanking", "System.String", false, false, false, "String")]
        public string AMLRiskRanking
        {
            get
            {
                return this.AMLRiskRanking;
            }
            set
            {
                this._AMLRiskRanking = value;
            }
        }
        /// < summary>
        /// AMLNextReviewDate
        /// </summary>
        [AttributeField("AMLNextReviewDate", "System.String", false, false, false, "String")]
        public string AMLNextReviewDate
        {
            get
            {
                return this.AMLNextReviewDate;
            }
            set
            {
                this._AMLNextReviewDate = value;
            }
        }
        /// < summary>
        /// BlackListHitFlag
        /// </summary>
        [AttributeField("BlackListHitFlag", "System.String", false, false, false, "String")]
        public string BlackListHitFlag
        {
            get
            {
                return this.BlackListHitFlag;
            }
            set
            {
                this._BlackListHitFlag = value;
            }
        }
        /// < summary>
        /// PEPListHitFlag
        /// </summary>
        [AttributeField("PEPListHitFlag", "System.String", false, false, false, "String")]
        public string PEPListHitFlag
        {
            get
            {
                return this.PEPListHitFlag;
            }
            set
            {
                this._PEPListHitFlag = value;
            }
        }
        /// < summary>
        /// NNListHitFlag
        /// </summary>
        [AttributeField("NNListHitFlag", "System.String", false, false, false, "String")]
        public string NNListHitFlag
        {
            get
            {
                return this.NNListHitFlag;
            }
            set
            {
                this._NNListHitFlag = value;
            }
        }
        /// < summary>
        /// Incorporated
        /// </summary>
        [AttributeField("Incorporated", "System.String", false, false, false, "String")]
        public string Incorporated
        {
            get
            {
                return this.Incorporated;
            }
            set
            {
                this._Incorporated = value;
            }
        }
        /// < summary>
        /// IncorporatedDate
        /// </summary>
        [AttributeField("IncorporatedDate", "System.String", false, false, false, "String")]
        public string IncorporatedDate
        {
            get
            {
                return this.IncorporatedDate;
            }
            set
            {
                this._IncorporatedDate = value;
            }
        }
        /// < summary>
        /// LastUpdateMaker
        /// </summary>
        [AttributeField("LastUpdateMaker", "System.String", false, false, false, "String")]
        public string LastUpdateMaker
        {
            get
            {
                return this.LastUpdateMaker;
            }
            set
            {
                this._LastUpdateMaker = value;
            }
        }
        /// < summary>
        /// LastUpdateChecker
        /// </summary>
        [AttributeField("LastUpdateChecker", "System.String", false, false, false, "String")]
        public string LastUpdateChecker
        {
            get
            {
                return this.LastUpdateChecker;
            }
            set
            {
                this._LastUpdateChecker = value;
            }
        }
        /// < summary>
        /// LastUpdateBranch
        /// </summary>
        [AttributeField("LastUpdateBranch", "System.String", false, false, false, "String")]
        public string LastUpdateBranch
        {
            get
            {
                return this.LastUpdateBranch;
            }
            set
            {
                this._LastUpdateBranch = value;
            }
        }
        /// < summary>
        /// LastUpdateDate
        /// </summary>
        [AttributeField("LastUpdateDate", "System.String", false, false, false, "String")]
        public string LastUpdateDate
        {
            get
            {
                return this.LastUpdateDate;
            }
            set
            {
                this._LastUpdateDate = value;
            }
        }
        /// < summary>
        /// LastUpdateSourceSystem
        /// </summary>
        [AttributeField("LastUpdateSourceSystem", "System.String", false, false, false, "String")]
        public string LastUpdateSourceSystem
        {
            get
            {
                return this.LastUpdateSourceSystem;
            }
            set
            {
                this._LastUpdateSourceSystem = value;
            }
        }
        /// < summary>
        /// HomeBranch
        /// </summary>
        [AttributeField("HomeBranch", "System.String", false, false, false, "String")]
        public string HomeBranch
        {
            get
            {
                return this.HomeBranch;
            }
            set
            {
                this._HomeBranch = value;
            }
        }
        /// < summary>
        /// Reason
        /// </summary>
        [AttributeField("Reason", "System.String", false, false, false, "String")]
        public string Reason
        {
            get
            {
                return this.Reason;
            }
            set
            {
                this._Reason = value;
            }
        }
        /// < summary>
        /// WarningFlag
        /// </summary>
        [AttributeField("WarningFlag", "System.String", false, false, false, "String")]
        public string WarningFlag
        {
            get
            {
                return this.WarningFlag;
            }
            set
            {
                this._WarningFlag = value;
            }
        }
        /// < summary>
        /// FiledSAR
        /// </summary>
        [AttributeField("FiledSAR", "System.String", false, false, false, "String")]
        public string FiledSAR
        {
            get
            {
                return this.FiledSAR;
            }
            set
            {
                this._FiledSAR = value;
            }
        }
        /// < summary>
        /// CreditCardBlockCode
        /// </summary>
        [AttributeField("CreditCardBlockCode", "System.String", false, false, false, "String")]
        public string CreditCardBlockCode
        {
            get
            {
                return this.CreditCardBlockCode;
            }
            set
            {
                this._CreditCardBlockCode = value;
            }
        }
        /// < summary>
        /// InternationalOrgPEP
        /// </summary>
        [AttributeField("InternationalOrgPEP", "System.String", false, false, false, "String")]
        public string InternationalOrgPEP
        {
            get
            {
                return this.InternationalOrgPEP;
            }
            set
            {
                this._InternationalOrgPEP = value;
            }
        }
        /// < summary>
        /// DomesticPEP
        /// </summary>
        [AttributeField("DomesticPEP", "System.String", false, false, false, "String")]
        public string DomesticPEP
        {
            get
            {
                return this.DomesticPEP;
            }
            set
            {
                this._DomesticPEP = value;
            }
        }
        /// < summary>
        /// ForeignPEPStakeholder
        /// </summary>
        [AttributeField("ForeignPEPStakeholder", "System.String", false, false, false, "String")]
        public string ForeignPEPStakeholder
        {
            get
            {
                return this.ForeignPEPStakeholder;
            }
            set
            {
                this._ForeignPEPStakeholder = value;
            }
        }
        /// < summary>
        /// InternationalOrgPEPStakeholder
        /// </summary>
        [AttributeField("InternationalOrgPEPStakeholder", "System.String", false, false, false, "String")]
        public string InternationalOrgPEPStakeholder
        {
            get
            {
                return this.InternationalOrgPEPStakeholder;
            }
            set
            {
                this._InternationalOrgPEPStakeholder = value;
            }
        }
        /// < summary>
        /// DomesticPEPStakeholder
        /// </summary>
        [AttributeField("DomesticPEPStakeholder", "System.String", false, false, false, "String")]
        public string DomesticPEPStakeholder
        {
            get
            {
                return this.DomesticPEPStakeholder;
            }
            set
            {
                this._DomesticPEPStakeholder = value;
            }
        }
        /// < summary>
        /// GroupInformationSharingNameListflag
        /// </summary>
        [AttributeField("GroupInformationSharingNameListflag", "System.String", false, false, false, "String")]
        public string GroupInformationSharingNameListflag
        {
            get
            {
                return this.GroupInformationSharingNameListflag;
            }
            set
            {
                this._GroupInformationSharingNameListflag = value;
            }
        }
        /// < summary>
        /// Filler
        /// </summary>
        [AttributeField("Filler", "System.String", false, false, false, "String")]
        public string Filler
        {
            get
            {
                return this.Filler;
            }
            set
            {
                this._Filler = value;
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
    public class Entity_AML_Cdata_ImportSet : EntitySet<Entity_AML_Cdata_Import>
    {
    }
}
