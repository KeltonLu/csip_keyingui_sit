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
        /// �����q�νs
        /// </summary>
        public static string M_CORP_NO = "CORP_NO";

        private string _CORP_SEQ;
        
        /// <summary>
        /// �����q�νs�Ǹ�
        /// </summary>
        public static string M_CORP_SEQ = "CORP_SEQ";

        private string _REG_ENG_NAME;
        
        /// <summary>
        /// �n�O�^��W��
        /// </summary>
        public static string M_REG_ENG_NAME = "REG_ENG_NAME";

        private string _REG_CHI_NAME;

        /// <summary>
        /// �n�O����W��
        /// </summary>
        public static string M_REG_CHI_NAME = "REG_CHI_NAME";

        private string _CREATE_DATE;

        /// <summary>
        /// �̦��}�����
        /// </summary>
        public static string M_CREATE_DATE = "CREATE_DATE";

        private string _STATUS;
        
        /// <summary>
        /// �����q���A
        /// </summary>
        public static string M_STATUS = "STATUS";

        private string _QUALIFY_FLAG;

        /// <summary>
        /// �ŦX���FLAG
        /// </summary>
        public static string M_QUALIFY_FLAG = "QUALIFY_FLAG";

        private string _CIRCULATE_MERCH;

        /// <summary>
        /// �y�q����
        /// </summary>
        public static string M_CIRCULATE_MERCH = "CIRCULATE_MERCH";

        private string _UPDATE_DATE;

        /// <summary>
        /// ���ʤ�
        /// </summary>
        public static string M_UPDATE_DATE = "UPDATE_DATE";

        private string _HQ_CORP_NO;

        ///<summary>
        /// �`���q�νs
        ///</summary>
        public static string M_HQ_CORP_NO = "HQ_CORP_NO";

        private string _HQ_CORP_SEQ;

        ///<summary>
        /// �`���q�νs�Ǹ�
        ///</summary>
        public static string M_HQ_CORP_SEQ = "HQ_CORP_SEQ";

        /// <summary>
        /// �����q�νs
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
        /// �����q�νs�Ǹ�
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
        /// �n�O�^��W��
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
        /// �n�O����W��
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
        /// �̦��}�����
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
        /// �����q���A
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
        /// �y�q����
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
        /// ���ʤ�
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
        /// �`���q�νs
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
        /// �`���q�νs�Ǹ�
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
