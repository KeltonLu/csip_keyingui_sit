using System;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;

namespace CSIPCommonModel.EntityLayer_new
{
    /// <summary>
    /// EntityAML_BRCH_STATUS
    /// </summary>
    [Serializable()]
    [AttributeTable("EntityAML_BRCH_STATUS")]
    public class EntityAML_BRCH_STATUS : Entity
    {
        private string _CORP_NO;

        /// <summary>
        /// 分公司統編
        /// </summary>
        public static string M_CORP_NO = "CORP_NO";

        private string _CORP_SEQ;
        
        /// <summary>
        /// 分公司統編序號
        /// </summary>
        public static string M_CORP_SEQ = "CORP_SEQ";

        private string _REG_ENG_NAME;
        
        /// <summary>
        /// 登記英文名稱
        /// </summary>
        public static string M_REG_ENG_NAME = "REG_ENG_NAME";

        private string _REG_CHI_NAME;

        /// <summary>
        /// 登記中文名稱
        /// </summary>
        public static string M_REG_CHI_NAME = "REG_CHI_NAME";

        private string _CREATE_DATE;

        /// <summary>
        /// 最早開店日期
        /// </summary>
        public static string M_CREATE_DATE = "CREATE_DATE";

        private string _STATUS;
        
        /// <summary>
        /// 分公司狀態
        /// </summary>
        public static string M_STATUS = "STATUS";

        private string _QUALIFY_FLAG;

        /// <summary>
        /// 符合資料FLAG
        /// </summary>
        public static string M_QUALIFY_FLAG = "QUALIFY_FLAG";

        private string _CIRCULATE_MERCH;

        /// <summary>
        /// 流通的店
        /// </summary>
        public static string M_CIRCULATE_MERCH = "CIRCULATE_MERCH";

        private string _UPDATE_DATE;

        /// <summary>
        /// 異動日
        /// </summary>
        public static string M_UPDATE_DATE = "UPDATE_DATE";

        private string _HQ_CORP_NO;

        ///<summary>
        /// 總公司統編
        ///</summary>
        public static string M_HQ_CORP_NO = "HQ_CORP_NO";

        private string _HQ_CORP_SEQ;

        ///<summary>
        /// 總公司統編序號
        ///</summary>
        public static string M_HQ_CORP_SEQ = "HQ_CORP_SEQ";

        /// <summary>
        /// 分公司統編
        /// </summary>
        //[AttributeField("CORP_NO", "System.String", false, true, false, "String")]
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
        /// 分公司統編序號
        /// </summary>
        //[AttributeField("CORP_SEQ", "System.String", false, true, false, "String")]
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
        /// 登記英文名稱
        /// </summary>
        //[AttributeField("REG_ENG_NAME", "System.String", false, true, false, "String")]
        public string REG_ENG_NAME
        {
            get
            {
                return this._REG_ENG_NAME;
            }
            set
            {
                this._REG_ENG_NAME = value;
            }
        }

        /// <summary>
        /// 登記中文名稱
        /// </summary>
        //[AttributeField("REG_CHI_NAME", "System.String", false, true, false, "String")]
        public string REG_CHI_NAME
        {
            get
            {
                return this._REG_CHI_NAME;
            }
            set
            {
                this._REG_CHI_NAME = value;
            }
        }

        /// <summary>
        /// 最早開店日期
        /// </summary>
        //[AttributeField("CREATE_DATE", "System.String", false, true, false, "String")]
        public string CREATE_DATE
        {
            get
            {
                return this._CREATE_DATE;
            }
            set
            {
                this._CREATE_DATE = value;
            }
        }

        /// <summary>
        /// 分公司狀態
        /// </summary>
        //[AttributeField("STATUS", "System.String", false, false, false, "String")]
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
        //[AttributeField("QUALIFY_FLAG", "System.String", false, false, false, "String")]
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
        /// 流通的店
        /// </summary>
        //[AttributeField("CIRCULATE_MERCH", "System.String", false, false, false, "String")]
        public string CIRCULATE_MERCH
        {
            get
            {
                return this._CIRCULATE_MERCH;
            }
            set
            {
                this._CIRCULATE_MERCH = value;
            }
        }

        /// <summary>
        /// 異動日
        /// </summary>
        //[AttributeField("UPDATE_DATE", "System.String", false, false, false, "String")]
        public string UPDATE_DATE
        {
            get
            {
                return this._UPDATE_DATE;
            }
            set
            {
                this._UPDATE_DATE = value;
            }
        }
        
        /// <summary>
        /// 總公司統編
        /// </summary>
        //[AttributeField("HQ_CORP_NO", "System.String", false, false, false, "String")]
        public string HQ_CORP_NO
        {
            get
            {
                return this._HQ_CORP_NO;
            }
            set
            {
                this._HQ_CORP_NO = value;
            }
        }

        /// <summary>
        /// 總公司統編序號
        /// </summary>
        //[AttributeField("HQ_CORP_SEQ", "System.String", false, false, false, "String")]
        public string HQ_CORP_SEQ
        {
            get
            {
                return this._HQ_CORP_SEQ;
            }
            set
            {
                this._HQ_CORP_SEQ = value;
            }
        }

    }

    /// <summary>
    /// EntityAML_BRCH_STATUS
    /// </summary>
    [Serializable()]
    public class EntityAML_BRCH_STATUSSet : EntitySet<EntityAML_BRCH_STATUS>
    {
    }
}
