using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;

namespace CSIPCommonModel.EntityLayer_new
{
    /// <summary>
    /// EntityPostOffice_CodeType
    /// </summary>
    [Serializable()]
    [AttributeTable("PostOffice_CodeType")]
    public class EntityPostOffice_CodeType : Entity
    {
        private int _ID;

        /// <summary>
        /// IdentityKey
        /// </summary>
        public static string M_ID = "ID";

        private string _TYPE;
        
        /// <summary>
        /// ���O
        /// </summary>
        public static string M_TYPE = "TYPE";

        private string _CODE_ID;
        
        /// <summary>
        /// ����KEY
        /// </summary>
        public static string M_CODE_ID = "CODE_ID";

        private string _CODE_NAME;

        /// <summary>
        /// ����W��
        /// </summary>
        public static string M_CODE_NAME = "CODE_NAME";

        private string _CODE_EN_NAME;
        
        /// <summary>
        /// �^��W��
        /// </summary>
        public static string M_CODE_EN_NAME = "CODE_EN_NAME";

        private int _ORDERBY;

        /// <summary>
        /// �Ƨ�
        /// </summary>
        public static string M_ORDERBY = "ORDERBY";

        private string _DESCRIPTION;

        /// <summary>
        /// ����
        /// </summary>
        public static string M_DESCRIPTION = "DESCRIPTION";

        private bool _IsValid;
        /// <summary>
        /// �O�_�ҥ�
        /// </summary>
        public static string M_IsValid = "IsValid";

        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("ID", "System.Int32", false, true, true, "Int32")]
        public int ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
            }
        }

        /// <summary>
        /// ���O
        /// </summary>
        [AttributeField("TYPE", "System.String", false, false, false, "String")]
        public string TYPE
        {
            get
            {
                return this._TYPE;
            }
            set
            {
                this._TYPE = value;
            }
        }

        /// <summary>
        /// ����KEY
        /// </summary>
        [AttributeField("CODE_ID", "System.String", false, false, false, "String")]
        public string CODE_ID
        {
            get
            {
                return this._CODE_ID;
            }
            set
            {
                this._CODE_ID = value;
            }
        }

        /// <summary>
        /// ����W��
        /// </summary>
        [AttributeField("CODE_NAME", "System.String", false, false, false, "String")]
        public string CODE_NAME
        {
            get
            {
                return this._CODE_NAME;
            }
            set
            {
                this._CODE_NAME = value;
            }
        }

        /// <summary>
        /// �^��W��
        /// </summary>
        [AttributeField("CODE_EN_NAME", "System.String", false, false, false, "String")]
        public string CODE_EN_NAME
        {
            get
            {
                return this._CODE_EN_NAME;
            }
            set
            {
                this._CODE_EN_NAME = value;
            }
        }

        /// <summary>
        /// �Ƨ�
        /// </summary>
        [AttributeField("ORDERBY", "System.Int32", false, false, false, "Int32")]
        public int ORDERBY
        {
            get
            {
                return this._ORDERBY;
            }
            set
            {
                this._ORDERBY = value;
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        [AttributeField("DESCRIPTION", "System.String", false, false, false, "String")]
        public string DESCRIPTION
        {
            get
            {
                return this._DESCRIPTION;
            }
            set
            {
                this._DESCRIPTION = value;
            }
        }

        /// <summary>
        /// �O�_�ҥ�
        /// </summary>
        [AttributeField("IsValid", "System.Boolean", false, false, false, "Boolean")]
        public bool IsValid
        {
            get
            {
                return this._IsValid;
                
            }
            set
            {
                this._IsValid = value;
            }
        }
    }
    
    /// <summary>
    /// EntityPostOffice_CodeType
    /// </summary>
    [Serializable()]
    public class EntityPostOffice_CodeTypeSet : EntitySet<EntityPostOffice_CodeType>
    {
    }
}
