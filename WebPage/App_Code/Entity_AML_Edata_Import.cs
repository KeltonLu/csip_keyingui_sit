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
    [AttributeTable("AML_Edata_Import")]
    public class Entity_AML_Edata_Import : Framework.Data.OM.Entity
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
        private string _RMMBatchNo;
        /// < summary>
        /// RMMBatchNo
        /// < /summary>
        public static string M_RMMBatchNo = "RMMBatchNo";
        private string _AMLInternalID;
        /// < summary>
        /// AMLInternalID
        /// < /summary>
        public static string M_AMLInternalID = "AMLInternalID";
        private string _DataDate;
        /// < summary>
        /// DataDate
        /// < /summary>
        public static string M_DataDate = "DataDate";
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
        private string _OriginalRiskRanking;
        /// < summary>
        /// OriginalRiskRanking
        /// < /summary>
        public static string M_OriginalRiskRanking = "OriginalRiskRanking";
        private string _OriginalNextReviewDate;
        /// < summary>
        /// OriginalNextReviewDate
        /// < /summary>
        public static string M_OriginalNextReviewDate = "OriginalNextReviewDate";
        private string _NewRiskRanking;
        /// < summary>
        /// NewRiskRanking
        /// < /summary>
        public static string M_NewRiskRanking = "NewRiskRanking";
        private string _NewNextReviewDate;
        /// < summary>
        /// NewNextReviewDate
        /// < /summary>
        public static string M_NewNextReviewDate = "NewNextReviewDate";
        private string _LastUpdateMaker;
        /// < summary>
        /// LastUpdateMaker
        /// < /summary>
        public static string M_LastUpdateMaker = "LastUpdateMaker";
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
        private string _HomeBranch;
        /// < summary>
        /// HomeBranch
        /// < /summary>
        public static string M_HomeBranch = "HomeBranch";
        private string _CaseType;
        /// < summary>
        /// CaseType
        /// < /summary>
        public static string M_CaseType = "CaseType";
        private string _FiledSAR;
        /// < summary>
        /// FiledSAR
        /// < /summary>
        public static string M_FiledSAR = "FiledSAR";
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
        /// RMMBatchNo
        /// </summary>
        [AttributeField("RMMBatchNo", "System.String", false, false, false, "String")]
        public string RMMBatchNo
        {
            get
            {
                return this.RMMBatchNo;
            }
            set
            {
                this._RMMBatchNo = value;
            }
        }
        /// < summary>
        /// AMLInternalID
        /// </summary>
        [AttributeField("AMLInternalID", "System.String", false, false, false, "String")]
        public string AMLInternalID
        {
            get
            {
                return this.AMLInternalID;
            }
            set
            {
                this._AMLInternalID = value;
            }
        }
        /// < summary>
        /// DataDate
        /// </summary>
        [AttributeField("DataDate", "System.String", false, false, false, "String")]
        public string DataDate
        {
            get
            {
                return this.DataDate;
            }
            set
            {
                this._DataDate = value;
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
        /// OriginalRiskRanking
        /// </summary>
        [AttributeField("OriginalRiskRanking", "System.String", false, false, false, "String")]
        public string OriginalRiskRanking
        {
            get
            {
                return this.OriginalRiskRanking;
            }
            set
            {
                this._OriginalRiskRanking = value;
            }
        }
        /// < summary>
        /// OriginalNextReviewDate
        /// </summary>
        [AttributeField("OriginalNextReviewDate", "System.String", false, false, false, "String")]
        public string OriginalNextReviewDate
        {
            get
            {
                return this.OriginalNextReviewDate;
            }
            set
            {
                this._OriginalNextReviewDate = value;
            }
        }
        /// < summary>
        /// NewRiskRanking
        /// </summary>
        [AttributeField("NewRiskRanking", "System.String", false, false, false, "String")]
        public string NewRiskRanking
        {
            get
            {
                return this.NewRiskRanking;
            }
            set
            {
                this._NewRiskRanking = value;
            }
        }
        /// < summary>
        /// NewNextReviewDate
        /// </summary>
        [AttributeField("NewNextReviewDate", "System.String", false, false, false, "String")]
        public string NewNextReviewDate
        {
            get
            {
                return this.NewNextReviewDate;
            }
            set
            {
                this._NewNextReviewDate = value;
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
        /// CaseType
        /// </summary>
        [AttributeField("CaseType", "System.String", false, false, false, "String")]
        public string CaseType
        {
            get
            {
                return this.CaseType;
            }
            set
            {
                this._CaseType = value;
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
    public class Entity_AML_Edata_ImportSet : EntitySet<Entity_AML_Edata_Import>
    {
    }
}
