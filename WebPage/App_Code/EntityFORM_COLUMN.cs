using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using System.Data;

namespace CSIPCommonModel.EntityLayer
{
    /// <summary>
    /// EntityFORM_COLUMN
    /// </summary>
    [Serializable()]
    [AttributeTable("FORM_COLUMN")]
    public class EntityFORM_COLUMN : Entity
    {
        private string _ELEMENT_CODE;

        /// <summary>
        /// ����N�X
        /// </summary>
        public static string M_ELEMENT_CODE = "ELEMENT_CODE";

        private string _ELEMENT_NAME;

        /// <summary>
        /// ������W��
        /// </summary>
        public static string M_ELEMENT_NAME = "ELEMENT_NAME";

        private string _ELEMENT_ID;

        /// <summary>
        /// ���ID
        /// </summary>
        public static string M_ELEMENT_ID = "ELEMENT_ID";

        private int _SEQUENCE;

        /// <summary>
        /// ����
        /// </summary>
        public static string M_SEQUENCE = "SEQUENCE";

        private bool _IS_REQUIRED;

        /// <summary>
        /// �O�_����
        /// </summary>
        public static string M_IS_REQUIRED = "IS_REQUIRED";

        private int _CHECK_FLAG;

        /// <summary>
        /// �ˮ֫��A(0.���ˮ� 1.�Ʀr 2.�^��)
        /// </summary>
        public static string M_CHECK_FLAG = "CHECK_FLAG";

        private int _VALUE_LENGTH;

        /// <summary>
        /// ����
        /// </summary>
        public static string M_VALUE_LENGTH = "VALUE_LENGTH";

        private string _DEFAULT_VALUE;

        /// <summary>
        /// �w�]��
        /// </summary>
        public static string M_DEFAULT_VALUE = "DEFAULT_VALUE";

        private string _PROPERTY_CODE;

        /// <summary>
        /// �ϥ����O�s��
        /// </summary>
        public static string M_PROPERTY_CODE = "PROPERTY_CODE";

        private string _REMARK;

        /// <summary>
        /// �Ƶ�
        /// </summary>
        public static string M_REMARK = "REMARK";

        private string _USER_ID;

        /// <summary>
        /// ���s
        /// </summary>
        public static string M_USER_ID = "USER_ID";

        private string _MOD_DATE;

        /// <summary>
        /// ��s���
        /// </summary>
        public static string M_MOD_DATE = "MOD_DATE";

        /// <summary>
        /// ����N�X
        /// </summary>
        [AttributeField("ELEMENT_CODE", "System.String", false, true, false, "String")]
        public string ELEMENT_CODE
        {
            get
            {
                return this._ELEMENT_CODE;
            }
            set
            {
                this._ELEMENT_CODE = value;
            }
        }

        /// <summary>
        /// ������W��
        /// </summary>
        [AttributeField("ELEMENT_NAME", "System.String", false, false, false, "String")]
        public string ELEMENT_NAME
        {
            get
            {
                return this._ELEMENT_NAME;
            }
            set
            {
                this._ELEMENT_NAME = value;
            }
        }

        /// <summary>
        /// ���ID
        /// </summary>
        [AttributeField("ELEMENT_ID", "System.String", false, false, false, "String")]
        public string ELEMENT_ID
        {
            get
            {
                return this._ELEMENT_ID;
            }
            set
            {
                this._ELEMENT_ID = value;
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        [AttributeField("SEQUENCE", "System.Int16", false, false, false, "Int16")]
        public int SEQUENCE
        {
            get
            {
                return this._SEQUENCE;
            }
            set
            {
                this._SEQUENCE = value;
            }
        }

        /// <summary>
        /// �O�_����
        /// </summary>
        [AttributeField("IS_REQUIRED", "System.Boolean", false, false, false, "Boolean")]
        public bool IS_REQUIRED
        {
            get
            {
                return this._IS_REQUIRED;
            }
            set
            {
                this._IS_REQUIRED = value;
            }
        }

        /// <summary>
        /// �ˮ֫��A(0.���ˮ� 1.�Ʀr 2.�^��)
        /// </summary>
        [AttributeField("CHECK_FLAG", "System.Int16", false, false, false, "Int16")]
        public int CHECK_FLAG
        {
            get
            {
                return this._CHECK_FLAG;
            }
            set
            {
                this._CHECK_FLAG = value;
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        [AttributeField("VALUE_LENGTH", "System.Int16", false, false, false, "Int16")]
        public int VALUE_LENGTH
        {
            get
            {
                return this._VALUE_LENGTH;
            }
            set
            {
                this._VALUE_LENGTH = value;
            }
        }

        /// <summary>
        /// �w�]��
        /// </summary>
        [AttributeField("DEFAULT_VALUE", "System.String", false, false, false, "String")]
        public string DEFAULT_VALUE
        {
            get
            {
                return this._DEFAULT_VALUE;
            }
            set
            {
                this._DEFAULT_VALUE = value;
            }
        }

        /// <summary>
        /// �ϥ����O�s��
        /// </summary>
        [AttributeField("PROPERTY_CODE", "System.String", false, false, false, "String")]
        public string PROPERTY_CODE
        {
            get
            {
                return this._PROPERTY_CODE;
            }
            set
            {
                this._PROPERTY_CODE = value;
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
        /// ���s
        /// </summary>
        [AttributeField("USER_ID", "System.String", false, false, false, "String")]
        public string USER_ID
        {
            get
            {
                return this._USER_ID;
            }
            set
            {
                this._USER_ID = value;
            }
        }

        /// <summary>
        /// ��s���
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

        /// <summary>
        /// �ϥ����O�s������W��
        /// </summary>
        public string PROPERTY_CODE_NAME
        {
            get
            {
                string codeName = string.Empty;
                string[] codeArr = this.PROPERTY_CODE.Split('|');
                DataTable dtblType = new DataTable();
                CSIPCommonModel.BusinessRules_new.BRM_PROPERTY_CODE.GetProperty("01", "41", null, ref dtblType);

                foreach (string code in codeArr)
                {
                    DataRow[] row = dtblType.Select("PROPERTY_CODE = '" + code + "'");
                    if (!string.IsNullOrEmpty(codeName)) codeName += "|";
                    codeName += row[0]["PROPERTY_NAME"].ToString();
                }

                return codeName;
            }
        }
    }

    /// <summary>
    /// EntityFORM_COLUMN
    /// </summary>
    [Serializable()]
    public class EntityFORM_COLUMNSet : EntitySet<EntityFORM_COLUMN>
    {
    }
}
