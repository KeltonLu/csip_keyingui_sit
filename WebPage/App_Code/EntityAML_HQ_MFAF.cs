using System;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;

namespace CSIPCommonModel.EntityLayer_new
{
    /// <summary>
    /// EntityAML_HQ_MFAF
    /// </summary>
    [Serializable()]
    [AttributeTable("EntityAML_HQ_MFAF")]
    public class EntityAML_HQ_MFAF : Entity
    {
        private string _CORP_NO;

        /// <summary>
        /// 参絪
        /// </summary>
        public static string M_CORP_NO = "CORP_NO";

        private string _CORP_SEQ;
        
        /// <summary>
        /// 参絪腹
        /// </summary>
        public static string M_CORP_SEQ = "CORP_SEQ";

        private string _MFAF_ID;
        
        /// <summary>
        /// MFAF絪
        /// </summary>
        public static string M_MFAF_ID = "MFAF_ID";

        private string _MFAF_NAME;

        /// <summary>
        /// MFAF﹎
        /// </summary>
        public static string M_MFAF_NAME = "MFAF_NAME";

        private string _MFAF_AREA;

        /// <summary>
        /// MFAF_跋办いみ
        /// </summary>
        public static string M_MFAF_AREA = "MFAF_AREA";

        private string _MFAF_UPDATE_DATE;

        /// <summary>
        /// MFAFネら
        /// </summary>
        public static string M_MFAF_UPDATE_DATE = "MFAF_UPDATE_DATE";

        private string _MOD_DATE;

        /// <summary>
        /// 钵笆ら
        /// </summary>
        public static string M_MOD_DATE = "MOD_DATE";


        /// <summary>
        /// 参絪
        /// </summary>
        [AttributeField("ID", "System.String", false, true, false, "String")]
        public string CORP_NO
        {
            get
            {
                return this._CORP_NO;
            }
            set
            {
                this._CORP_NO = value;
            }
        }

        /// <summary>
        /// 参絪腹
        /// </summary>
        [AttributeField("CORP_SEQ", "System.String", false, true, false, "String")]
        public string CORP_SEQ
        {
            get
            {
                return this._CORP_SEQ;
            }
            set
            {
                this._CORP_SEQ = value;
            }
        }

        /// <summary>
        /// MFAF絪
        /// </summary>
        [AttributeField("MFAF_ID", "System.String", false, false, false, "String")]
        [AttributeRfPage("HQlblMFAF_ID", "CustLabel", false)]
        public string MFAF_ID
        {
            get
            {
                return this._MFAF_ID;
            }
            set
            {
                this._MFAF_ID = value;
            }
        }

        /// <summary>
        /// MFAF﹎
        /// </summary>
        [AttributeField("MFAF_NAME", "System.String", false, false, false, "String")]
        [AttributeRfPage("HQlblMFAF_NAME", "CustLabel", false)]
        public string MFAF_NAME
        {
            get
            {
                return this._MFAF_NAME;
            }
            set
            {
                this._MFAF_NAME = value;
            }
        }

        /// <summary>
        /// MFAF跋办いみ
        /// </summary>
        [AttributeField("MFAF_AREA", "System.String", false, false, false, "String")]
        [AttributeRfPage("HQlblMFAF_AREA", "CustLabel", false)]
        public string MFAF_AREA
        {
            get
            {
                return this._MFAF_AREA;
            }
            set
            {
                this._MFAF_AREA = value;
            }
        }

        /// <summary>
        /// MFAFネら
        /// </summary>
        [AttributeField("MFAF_UPDATE_DATE", "System.String", false, false, false, "String")]
        public string MFAF_UPDATE_DATE
        {
            get
            {
                return this._MFAF_UPDATE_DATE;
            }
            set
            {
                this._MFAF_UPDATE_DATE = value;
            }
        }

        /// <summary>
        /// 钵笆ら
        /// </summary>
        [AttributeField("MOD_DATE", "System.String", false, false, false, "String")]
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
    /// EntityAML_HQ_MFAF
    /// </summary>
    [Serializable()]
    public class EntityAML_HQ_MFAFSet : EntitySet<EntityAML_HQ_MFAF>
    {
    }
}
