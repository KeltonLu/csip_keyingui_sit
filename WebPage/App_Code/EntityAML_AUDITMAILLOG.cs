using System;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;

namespace CSIPCommonModel.EntityLayer_new
{
    /// <summary>
    /// EntityAML_AUDITMAILLOG
    /// </summary>
    [Serializable()]
    [AttributeTable("EntityAML_AUDITMAILLOG")]
    public class EntityAML_AUDITMAILLOG : Entity
    {
        private string _HQ_ID;

        /// <summary>
        /// HQ_ID
        /// </summary>
        public static string M_HQ_ID = "HQ_ID";

        private string _CASE_NO;
        
        /// <summary>
        /// 案件編號
        /// </summary>
        public static string M_CASE_NO = "CASE_NO";

        private string _CORP_NO;
        
        /// <summary>
        /// 統一編號
        /// </summary>
        public static string M_CORP_NO = "CORP_NO";

        private string _REG_NAME;

        /// <summary>
        /// 登記名稱
        /// </summary>
        public static string M_REG_NAME = "REG_NAME";
        
        private string _EMAIL;

        /// <summary>
        /// 異動日
        /// </summary>
        public static string M_EMAIL = "EMAIL";

        private string _EXPIRYDATE;
        ///<summary
        // 審查到期日
        ///</summary>
        public static string M_EXPIRYDATE = "EXPIRYDATE";

        private string _BATCHDATE;
        ///<summary
        // 批次日期
        ///</summary>
        public static string M_BATCHDATE = "BATCHDATE";

        private string _STATUS;
        ///<summary
        // 發送狀態
        ///</summary>
        public static string M_STATUS = "STATUS";

        private string _SENDTYPE;
        ///<summary
        // 20200803-RQ-2020-021027-001
        // MAIL寄送類型
        ///</summary>
        public static string M_SENDTYPE = "SENDTYPE";

        /// <summary>
        /// 統編
        /// </summary>
        [AttributeField("HQ_ID", "System.String", false, true, false, "String")]
        public string HQ_ID
        {
            get
            {
                return this._HQ_ID;
            }
            set
            {
                this._HQ_ID = value;
            }
        }

        /// <summary>
        /// 統編序號
        /// </summary>
        [AttributeField("CASE_NO", "System.String", false, true, false, "String")]
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

        /// <summary>
        /// 總公司狀態
        /// </summary>
        [AttributeField("CORP_NO", "System.String", false, false, false, "String")]
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
        /// REG_NAME
        /// </summary>
        [AttributeField("REG_NAME", "System.String", false, false, false, "String")]
        public string REG_NAME
        {
            get
            {
                return this._REG_NAME;
            }
            set
            {
                this._REG_NAME = value;
            }
        }
        
        /// <summary>
        /// 異動日
        /// </summary>
        [AttributeField("EMAIL", "System.String", false, false, false, "String")]
        public string EMAIL
        {
            get
            {
                return this._EMAIL;
            }
            set
            {
                this._EMAIL = value;
            }
        }

        /// <summary>
        /// 審查到期日
        /// </summary>
        [AttributeField("EXPIRYDATE", "System.String", false, false, false, "String")]
        public string EXPIRYDATE
        {
            get
            {
                return this._EXPIRYDATE;
            }
            set
            {
                this._EXPIRYDATE = value;
            }
        }

        /// <summary>
        /// 批次日期
        /// </summary>
        [AttributeField("BATCHDATE", "System.String", false, false, false, "String")]
        public string BATCHDATE
        {
            get
            {
                return this._BATCHDATE;
            }
            set
            {
                this._BATCHDATE = value;
            }
        }

        /// <summary>
        /// 發送狀態
        /// </summary>
        [AttributeField("STATUS", "System.String", false, false, false, "String")]
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
        /// 20200803-RQ-2020-021027-001
        /// MAIL寄送類型
        /// </summary>
        [AttributeField("SENDTYPE", "System.String", false, false, false, "String")]
        public string SENDTYPE
        {
            get
            {
                return this._SENDTYPE;
            }
            set
            {
                this._SENDTYPE = value;
            }
        }
    }


    /// <summary>
    /// EntityAML_AUDITMAILLOG
    /// </summary>
    [Serializable()]
    public class EntityAML_AUDITMAILLOGSet : EntitySet<EntityAML_AUDITMAILLOG>
    {
    }
}
