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
        /// �ץ�s��
        /// </summary>
        public static string M_CASE_NO = "CASE_NO";

        private string _CORP_NO;
        
        /// <summary>
        /// �Τ@�s��
        /// </summary>
        public static string M_CORP_NO = "CORP_NO";

        private string _REG_NAME;

        /// <summary>
        /// �n�O�W��
        /// </summary>
        public static string M_REG_NAME = "REG_NAME";
        
        private string _EMAIL;

        /// <summary>
        /// ���ʤ�
        /// </summary>
        public static string M_EMAIL = "EMAIL";

        private string _EXPIRYDATE;
        ///<summary
        // �f�d�����
        ///</summary>
        public static string M_EXPIRYDATE = "EXPIRYDATE";

        private string _BATCHDATE;
        ///<summary
        // �妸���
        ///</summary>
        public static string M_BATCHDATE = "BATCHDATE";

        private string _STATUS;
        ///<summary
        // �o�e���A
        ///</summary>
        public static string M_STATUS = "STATUS";

        private string _SENDTYPE;
        ///<summary
        // 20200803-RQ-2020-021027-001
        // MAIL�H�e����
        ///</summary>
        public static string M_SENDTYPE = "SENDTYPE";

        /// <summary>
        /// �νs
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
        /// �νs�Ǹ�
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
        /// �`���q���A
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
        /// ���ʤ�
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
        /// �f�d�����
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
        /// �妸���
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
        /// �o�e���A
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
        /// MAIL�H�e����
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
