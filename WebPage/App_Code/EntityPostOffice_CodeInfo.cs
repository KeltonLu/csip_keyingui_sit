using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;

namespace CSIPCommonModel.EntityLayer_new
{
    /// <summary>
    /// EntityPostOffice_CodeInfo
    /// </summary>
    [Serializable()]
    [AttributeTable("PostOffice_CodeInfo")]
    public class EntityPostOffice_CodeInfo : Entity
    {
        private int _ID;

        /// <summary>
        /// IdentityKey
        /// </summary>
        public static string M_ID = "ID";

        private string _TYPE;
        
        /// <summary>
        /// 類別
        /// </summary>
        public static string M_TYPE = "TYPE";

        private string _TYPE_NAME;
        
        /// <summary>
        /// 類別名稱
        /// </summary>
        public static string M_TYPE_NAME = "TYPE_NAME";

        private string _DESCRIPTION;

        /// <summary>
        /// 說明
        /// </summary>
        public static string M_DESCRIPTION = "DESCRIPTION";
        
        private int _ORDERBY;

        /// <summary>
        /// 排序
        /// </summary>
        public static string M_ORDERBY = "ORDERBY";
        
        private bool _IsValid;
        /// <summary>
        /// 是否啟用
        /// </summary>
        public static string M_IsValid = "IsValid";
        
        private string _Editable;

        /// <summary>
        /// 是否可編輯，為Y, 表示顯示在頁面上可供修改
        /// </summary>
        public static string M_Editable = "Editable";

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
        /// 類別
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
        /// 類別名稱
        /// </summary>
        [AttributeField("TYPE_NAME", "System.String", false, false, false, "String")]
        public string TYPE_NAME
        {
            get
            {
                return this._TYPE_NAME;
            }
            set
            {
                this._TYPE_NAME = value;
            }
        }

        /// <summary>
        /// 說明
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
        /// 排序
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
        /// 是否啟用
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

        /// <summary>
        /// 是否可編輯，為Y, 表示顯示在頁面上可供修改
        /// </summary>
        [AttributeField("Editable", "System.String", false, false, false, "String")]
        public string Editable
        {
            get
            {
                return this._Editable;
            }
            set
            {
                this._Editable = value;
            }
        }

    }

    /// <summary>
    /// EntityPostOffice_CodeType
    /// </summary>
    [Serializable()]
    public class EntityPostOffice_CodeInfoSet : EntitySet<EntityPostOffice_CodeInfo>
    {
    }
}
