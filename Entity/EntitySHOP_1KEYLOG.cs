//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:2.0.50727.42
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;


namespace CSIPKeyInGUI.EntityLayer
{
    
    
    /// <summary>
    /// shop_1keylog
    /// </summary>
    [Serializable()]
    [AttributeTable("shop_1keylog")]
    public class EntitySHOP_1KEYLOG : Entity
    {
        
        private string _shop_id;
        
        /// <summary>
        /// shop_id
        /// </summary>
        public static string M_shop_id = "shop_id";
        
        private string _user_id;
        
        /// <summary>
        /// user_id
        /// </summary>
        public static string M_user_id = "user_id";
        
        private string _write_date;
        
        /// <summary>
        /// write_date
        /// </summary>
        public static string M_write_date = "write_date";
        
        private string _bank_name;
        
        /// <summary>
        /// bank_name
        /// </summary>
        public static string M_bank_name = "bank_name";
        
        private string _branch_name;
        
        /// <summary>
        /// branch_name
        /// </summary>
        public static string M_branch_name = "branch_name";
        
        private string _account;
        
        /// <summary>
        /// account
        /// </summary>
        public static string M_account = "account";
        
        private string _check_num;
        
        /// <summary>
        /// check_num
        /// </summary>
        public static string M_check_num = "check_num";
        
        private string _account1;
        
        /// <summary>
        /// account1
        /// </summary>
        public static string M_account1 = "account1";
        
        private string _account2;
        
        /// <summary>
        /// account2
        /// </summary>
        public static string M_account2 = "account2";
        
        private string _cancel_code1;
        
        /// <summary>
        /// cancel_code1
        /// </summary>
        public static string M_cancel_code1 = "cancel_code1";
        
        private string _cancel_date;
        
        /// <summary>
        /// cancel_date
        /// </summary>
        public static string M_cancel_date = "cancel_date";
        
        private string _cancel_code2;
        
        /// <summary>
        /// cancel_code2
        /// </summary>
        public static string M_cancel_code2 = "cancel_code2";
        
        /// <summary>
        /// shop_id
        /// </summary>
        [AttributeField("shop_id", "System.String", false, false, false, "String")]
        public string shop_id
        {
            get
            {
                return this._shop_id;
            }
            set
            {
                this._shop_id = value;
            }
        }
        
        /// <summary>
        /// user_id
        /// </summary>
        [AttributeField("user_id", "System.String", false, false, false, "String")]
        public string user_id
        {
            get
            {
                return this._user_id;
            }
            set
            {
                this._user_id = value;
            }
        }
        
        /// <summary>
        /// write_date
        /// </summary>
        [AttributeField("write_date", "System.String", false, false, false, "String")]
        public string write_date
        {
            get
            {
                return this._write_date;
            }
            set
            {
                this._write_date = value;
            }
        }
        
        /// <summary>
        /// bank_name
        /// </summary>
        [AttributeField("bank_name", "System.String", false, false, false, "String")]
        public string bank_name
        {
            get
            {
                return this._bank_name;
            }
            set
            {
                this._bank_name = value;
            }
        }
        
        /// <summary>
        /// branch_name
        /// </summary>
        [AttributeField("branch_name", "System.String", false, false, false, "String")]
        public string branch_name
        {
            get
            {
                return this._branch_name;
            }
            set
            {
                this._branch_name = value;
            }
        }
        
        /// <summary>
        /// account
        /// </summary>
        [AttributeField("account", "System.String", false, false, false, "String")]
        public string account
        {
            get
            {
                return this._account;
            }
            set
            {
                this._account = value;
            }
        }
        
        /// <summary>
        /// check_num
        /// </summary>
        [AttributeField("check_num", "System.String", false, false, false, "String")]
        public string check_num
        {
            get
            {
                return this._check_num;
            }
            set
            {
                this._check_num = value;
            }
        }
        
        /// <summary>
        /// account1
        /// </summary>
        [AttributeField("account1", "System.String", false, false, false, "String")]
        public string account1
        {
            get
            {
                return this._account1;
            }
            set
            {
                this._account1 = value;
            }
        }
        
        /// <summary>
        /// account2
        /// </summary>
        [AttributeField("account2", "System.String", false, false, false, "String")]
        public string account2
        {
            get
            {
                return this._account2;
            }
            set
            {
                this._account2 = value;
            }
        }
        
        /// <summary>
        /// cancel_code1
        /// </summary>
        [AttributeField("cancel_code1", "System.String", false, false, false, "String")]
        public string cancel_code1
        {
            get
            {
                return this._cancel_code1;
            }
            set
            {
                this._cancel_code1 = value;
            }
        }
        
        /// <summary>
        /// cancel_date
        /// </summary>
        [AttributeField("cancel_date", "System.String", false, false, false, "String")]
        public string cancel_date
        {
            get
            {
                return this._cancel_date;
            }
            set
            {
                this._cancel_date = value;
            }
        }
        
        /// <summary>
        /// cancel_code2
        /// </summary>
        [AttributeField("cancel_code2", "System.String", false, false, false, "String")]
        public string cancel_code2
        {
            get
            {
                return this._cancel_code2;
            }
            set
            {
                this._cancel_code2 = value;
            }
        }
    }
    
    /// <summary>
    /// shop_1keylog
    /// </summary>
    [Serializable()]
    public class EntitySHOP_1KEYLOGSet : EntitySet<EntitySHOP_1KEYLOG>
    {
    }
}
