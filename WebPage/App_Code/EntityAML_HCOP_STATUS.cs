using System;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;

namespace CSIPCommonModel.EntityLayer_new
{
    /// <summary>
    /// EntityAML_HCOP_STATUS
    /// </summary>
    [Serializable()]
    [AttributeTable("EntityAML_HCOP_STATUS")]
    public class EntityAML_HCOP_STATUS : Entity
    {
        private string _CORP_NO;

        /// <summary>
        /// 統編
        /// </summary>
        public static string M_CORP_NO = "CORP_NO";

        private string _CORP_SEQ;
        
        /// <summary>
        /// 統編序號
        /// </summary>
        public static string M_CORP_SEQ = "CORP_SEQ";

        private string _STATUS;
        
        /// <summary>
        /// 總公司狀態
        /// </summary>
        public static string M_STATUS = "STATUS";

        private string _QUALIFY_FLAG;

        /// <summary>
        /// QUALIFY_FLAG
        /// </summary>
        public static string M_QUALIFY_FLAG = "QUALIFY_FLAG";
        
        private string _MOD_DATE;

        /// <summary>
        /// 異動日
        /// </summary>
        public static string M_MOD_DATE = "MOD_DATE";

        private string _NonCooperation;
        ///<summary
        //20200217-RQ-2019-030155-003-新增不合作註記欄位(NonCooperation)
        ///</summary>
        public static string M_NonCooperation = "NonCooperation";

        /// <summary>
        /// 統編
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
        /// 統編序號
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
        /// 總公司狀態
        /// </summary>
        [AttributeField("STATUS", "System.String", false, false, false, "String")]
        //[AttributeRfPage("HQlblMFAF_ID", "CustLabel", false)]
        public string STATUS
        {
            get
            {
                return this._STATUS;
            }
            set
            {
                this._STATUS = value;
            }
        }

        /// <summary>
        /// QUALIFY_FLAG
        /// </summary>
        [AttributeField("QUALIFY_FLAG", "System.String", false, false, false, "String")]
        //[AttributeRfPage("HQlblMFAF_NAME", "CustLabel", false)]
        public string QUALIFY_FLAG
        {
            get
            {
                return this._QUALIFY_FLAG;
            }
            set
            {
                this._QUALIFY_FLAG = value;
            }
        }
        
        /// <summary>
        /// 異動日
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
        //20200217-RQ-2019-030155-003-新增不合作註記欄位(NonCooperation)
        /// <summary>
        /// 異動日
        /// </summary>
        [AttributeField("NonCooperation", "System.String", false, false, false, "String")]
        public string NonCooperation
        {
            get
            {
                return this._NonCooperation;
            }
            set
            {
                this._NonCooperation = value;
            }
        }
    }

    /// <summary>
    /// EntityAML_HCOP_STATUS
    /// </summary>
    [Serializable()]
    public class EntityAML_HCOP_STATUSSet : EntitySet<EntityAML_HCOP_STATUS>
    {
    }
}
