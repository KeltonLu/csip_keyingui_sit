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
    /// trans_num
    /// </summary>
    [Serializable()]
    [AttributeTable("trans_num")]
    public class EntityTRANS_NUM : Entity
    {
        
        private string _trans_code;
        
        /// <summary>
        /// trans_code
        /// </summary>
        public static string M_trans_code = "trans_code";
        
        private int _trans_num;
        
        /// <summary>
        /// trans_num
        /// </summary>
        public static string M_trans_num = "trans_num";
        
        private string _trans_date;
        
        /// <summary>
        /// trans_date
        /// </summary>
        public static string M_trans_date = "trans_date";
        
        /// <summary>
        /// trans_code
        /// </summary>
        [AttributeField("trans_code", "System.String", false, true, false, "String")]
        public string trans_code
        {
            get
            {
                return this._trans_code;
            }
            set
            {
                this._trans_code = value;
            }
        }
        
        /// <summary>
        /// trans_num
        /// </summary>
        [AttributeField("trans_num", "System.Int32", false, false, false, "Int32")]
        public int trans_num
        {
            get
            {
                return this._trans_num;
            }
            set
            {
                this._trans_num = value;
            }
        }
        
        /// <summary>
        /// trans_date
        /// </summary>
        [AttributeField("trans_date", "System.String", false, true, false, "String")]
        public string trans_date
        {
            get
            {
                return this._trans_date;
            }
            set
            {
                this._trans_date = value;
            }
        }
    }
    
    /// <summary>
    /// trans_num
    /// </summary>
    [Serializable()]
    public class EntityTRANS_NUMSet : EntitySet<EntityTRANS_NUM>
    {
    }
}
