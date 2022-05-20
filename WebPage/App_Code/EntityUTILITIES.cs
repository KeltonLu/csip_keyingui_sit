using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;

namespace CSIPCommonModel.EntityLayer
{
    /// <summary>
    /// EntityUTILITIES
    /// </summary>
    [Serializable()]
    [AttributeTable("UTILITIES")]
    public class EntityUTILITIES : Entity
    {
        private string _BUSINESS_CATEGORY;

        /// <summary>
        /// �~�����O
        /// </summary>
        public static string M_BUSINESS_CATEGORY = "BUSINESS_CATEGORY";

        private string _PRIMARY_CARDHOLDER_ID;
        
        /// <summary>
        /// ���d�HID
        /// </summary>
        public static string M_PRIMARY_CARDHOLDER_ID = "PRIMARY_CARDHOLDER_ID";

        private string _NAME;
        
        /// <summary>
        /// �m�W
        /// </summary>
        public static string M_NAME = "NAME";

        private string _RECEIVE_NUMBER;
        
        /// <summary>
        /// ����s��
        /// </summary>
        public static string M_RECEIVE_NUMBER = "RECEIVE_NUMBER";

        private string _MOBILE_PHONE;
        
        /// <summary>
        /// ��ʹq��
        /// </summary>
        public static string M_MOBILE_PHONE = "MOBILE_PHONE";

        private string _APPLY_TYPE;

        /// <summary>
        /// �ӽЧO(1.�ӽ� 2.�פ�)
        /// </summary>
        public static string M_APPLY_TYPE = "APPLY_TYPE";

        private string _PLATE_NO1;
        
        /// <summary>
        /// ���P���X(�e)
        /// </summary>
        public static string M_PLATE_NO1 = "PLATE_NO1";

        private string _PLATE_NO2;

        /// <summary>
        /// ���P���X(��)
        /// </summary>
        public static string M_PLATE_NO2 = "PLATE_NO2";

        private string _OWNERS_ID;

        /// <summary>
        /// ���DID�βνs
        /// </summary>
        public static string M_OWNERS_ID = "OWNERS_ID";

        private string _MONTHLY_PARKING_FEE;

        /// <summary>
        /// �믲�����O�Nú
        /// </summary>
        public static string M_MONTHLY_PARKING_FEE = "MONTHLY_PARKING_FEE";

        private string _POPUL_NO;

        /// <summary>
        /// ���s�N��
        /// </summary>
        public static string M_POPUL_NO = "POPUL_NO";

        private string _POPUL_EMP_NO;

        /// <summary>
        /// �ؤl(���s���s)
        /// </summary>
        public static string M_POPUL_EMP_NO = "POPUL_EMP_NO";

        private string _INTRODUCER_CARD_ID;

        /// <summary>
        /// �ؤl
        /// </summary>
        public static string M_INTRODUCER_CARD_ID = "INTRODUCER_CARD_ID";

        private string _SIGNATURE;

        /// <summary>
        /// ñ�W(1.�P 2.���P 3.�Sñ)
        /// </summary>
        public static string M_SIGNATURE = "SIGNATURE";

        private string _ERROR_INFORM_TYPE;

        /// <summary>
        /// ���~�q���覡(1.²�T 2.E-mail 3.�H�� 4.��L)
        /// </summary>
        public static string M_ERROR_INFORM_TYPE = "ERROR_INFORM_TYPE";

        private string _REMARK;

        /// <summary>
        /// �Ƶ�
        /// </summary>
        public static string M_REMARK = "REMARK";

        private string _AGENT_ID;

        /// <summary>
        /// ���ɭ��s
        /// </summary>
        public static string M_AGENT_ID = "AGENT_ID";

        private string _KEYIN_FLAG;

        /// <summary>
        /// �����ѧO�X
        /// </summary>
        public static string M_KEYIN_FLAG = "KEYIN_FLAG";

        private string _KEYIN_DATE;

        /// <summary>
        /// ���ɤ�
        /// </summary>
        public static string M_KEYIN_DATE = "KEYIN_DATE";

        private string _SENDHOST_FLAG;

        /// <summary>
        /// �W�ǥD���ѧO�X
        /// </summary>
        public static string M_SENDHOST_FLAG = "SENDHOST_FLAG";

        /// <summary>
        /// �~�����O
        /// </summary>
        [AttributeField("BUSINESS_CATEGORY", "System.String", false, true, false, "String")]
        public string BUSINESS_CATEGORY
        {
            get
            {
                return this._BUSINESS_CATEGORY;
            }
            set
            {
                this._BUSINESS_CATEGORY = value;
            }
        }

        /// <summary>
        /// ���d�HID
        /// </summary>
        [AttributeField("PRIMARY_CARDHOLDER_ID", "System.String", false, false, false, "String")]
        public string PRIMARY_CARDHOLDER_ID
        {
            get
            {
                return this._PRIMARY_CARDHOLDER_ID;
            }
            set
            {
                this._PRIMARY_CARDHOLDER_ID = value;
            }
        }

        /// <summary>
        /// �m�W
        /// </summary>
        [AttributeField("NAME", "System.String", false, false, false, "String")]
        public string NAME
        {
            get
            {
                return this._NAME;
            }
            set
            {
                this._NAME = value;
            }
        }

        /// <summary>
        /// ����s��
        /// </summary>
        [AttributeField("RECEIVE_NUMBER", "System.String", false, true, false, "String")]
        public string RECEIVE_NUMBER
        {
            get
            {
                return this._RECEIVE_NUMBER;
            }
            set
            {
                this._RECEIVE_NUMBER = value;
            }
        }

        /// <summary>
        /// ��ʹq��
        /// </summary>
        [AttributeField("MOBILE_PHONE", "System.String", false, false, false, "String")]
        public string MOBILE_PHONE
        {
            get
            {
                return this._MOBILE_PHONE;
            }
            set
            {
                this._MOBILE_PHONE = value;
            }
        }

        /// <summary>
        /// �ӽЧO(1.�ӽ� 2.�פ�)
        /// </summary>
        [AttributeField("APPLY_TYPE", "System.String", false, false, false, "String")]
        public string APPLY_TYPE
        {
            get
            {
                return this._APPLY_TYPE;
            }
            set
            {
                this._APPLY_TYPE = value;
            }
        }

        /// <summary>
        /// ���P���X(�e)
        /// </summary>
        [AttributeField("PLATE_NO1", "System.String", false, false, false, "String")]
        public string PLATE_NO1
        {
            get
            {
                return this._PLATE_NO1;
            }
            set
            {
                this._PLATE_NO1 = value;
            }
        }

        /// <summary>
        /// ���P���X(��)
        /// </summary>
        [AttributeField("PLATE_NO2", "System.String", false, false, false, "String")]
        public string PLATE_NO2
        {
            get
            {
                return this._PLATE_NO2;
            }
            set
            {
                this._PLATE_NO2 = value;
            }
        }

        /// <summary>
        /// ���DID�βνs
        /// </summary>
        [AttributeField("OWNERS_ID", "System.String", false, false, false, "String")]
        public string OWNERS_ID
        {
            get
            {
                return this._OWNERS_ID;
            }
            set
            {
                this._OWNERS_ID = value;
            }
        }

        /// <summary>
        /// �믲�����O�Nú
        /// </summary>
        [AttributeField("MONTHLY_PARKING_FEE", "System.String", false, false, false, "String")]
        public string MONTHLY_PARKING_FEE
        {
            get
            {
                return this._MONTHLY_PARKING_FEE;
            }
            set
            {
                this._MONTHLY_PARKING_FEE = value;
            }
        }

        /// <summary>
        /// ���s�N��
        /// </summary>
        [AttributeField("POPUL_NO", "System.String", false, false, false, "String")]
        public string POPUL_NO
        {
            get
            {
                return this._POPUL_NO;
            }
            set
            {
                this._POPUL_NO = value;
            }
        }

        /// <summary>
        /// ���s���s
        /// </summary>
        [AttributeField("POPUL_EMP_NO", "System.String", false, false, false, "String")]
        public string POPUL_EMP_NO
        {
            get
            {
                return this._POPUL_EMP_NO;
            }
            set
            {
                this._POPUL_EMP_NO = value;
            }
        }

        /// <summary>
        /// �ؤl
        /// </summary>
        [AttributeField("INTRODUCER_CARD_ID", "System.String", false, false, false, "String")]
        public string INTRODUCER_CARD_ID
        {
            get
            {
                return this._INTRODUCER_CARD_ID;
            }
            set
            {
                this._INTRODUCER_CARD_ID = value;
            }
        }

        /// <summary>
        /// ñ�W(1.�P 2.���P 3.�Sñ)
        /// </summary>
        [AttributeField("SIGNATURE", "System.String", false, false, false, "String")]
        public string SIGNATURE
        {
            get
            {
                return this._SIGNATURE;
            }
            set
            {
                this._SIGNATURE = value;
            }
        }

        /// <summary>
        /// ���~�q���覡(1.²�T 2.E-mail 3.�H�� 4.��L)
        /// </summary>
        [AttributeField("ERROR_INFORM_TYPE", "System.String", false, false, false, "String")]
        public string ERROR_INFORM_TYPE
        {
            get
            {
                return this._ERROR_INFORM_TYPE;
            }
            set
            {
                this._ERROR_INFORM_TYPE = value;
            }
        }

        /// <summary>
        /// �Ƶ�
        /// </summary>
        [AttributeField("REMARK", "System.String", false, false, false, "String")]
        public string REMARK
        {
            get
            {
                return this._REMARK;
            }
            set
            {
                this._REMARK = value;
            }
        }

        /// <summary>
        /// ���ɤH���s
        /// </summary>
        [AttributeField("AGENT_ID", "System.String", false, false, false, "String")]
        public string AGENT_ID
        {
            get
            {
                return this._AGENT_ID;
            }
            set
            {
                this._AGENT_ID = value;
            }
        }

        /// <summary>
        /// �����ѧO�X
        /// </summary>
        [AttributeField("KEYIN_FLAG", "System.String", false, true, false, "String")]
        public string KEYIN_FLAG
        {
            get
            {
                return this._KEYIN_FLAG;
            }
            set
            {
                this._KEYIN_FLAG = value;
            }
        }

        /// <summary>
        /// ���ɤ�
        /// </summary>
        [AttributeField("KEYIN_DATE", "System.String", false, false, false, "String")]
        public string KEYIN_DATE
        {
            get
            {
                return this._KEYIN_DATE;
            }
            set
            {
                this._KEYIN_DATE = value;
            }
        }

        /// <summary>
        /// �W�ǥD���ѧO�X
        /// </summary>
        [AttributeField("SENDHOST_FLAG", "System.String", false, false, false, "String")]
        public string SENDHOST_FLAG
        {
            get
            {
                return this._SENDHOST_FLAG;
            }
            set
            {
                this._SENDHOST_FLAG = value;
            }
        }
    }
    
    /// <summary>
    /// EntityUTILITIES
    /// </summary>
    [Serializable()]
    public class EntityUTILITIESSet : EntitySet<EntityUTILITIES>
    {
    }
}
