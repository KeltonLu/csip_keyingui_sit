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
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;

namespace CSIPKeyInGUI.EntityLayer_new
{
    /// <summary>
    /// ShopChange_LOG
    /// </summary>
    [Serializable()]
    [AttributeTable("ShopChange_LOG")]
    public class EntityShopChange_LOG : Entity
    {
        private string _DOC_ID;

        /// <summary>
        /// shop_id
        /// </summary>
        public static string M_DOC_ID = "DOC_ID";

        private string _CORP_NO;

        /// <summary>
        /// CORP_NO
        /// </summary>
        public static string M_CORP_NO = "CORP_NO";

        private string _CORP_SEQ;

        /// <summary>
        /// CORP_SEQ
        /// </summary>
        public static string M_CORP_SEQ = "CORP_SEQ";
        
        private string _KeyinFLAG;

        /// <summary>
        /// KeyinFLAG
        /// </summary>
        public static string M_KeyinFLAG = "KeyinFLAG";
        
        private string _MOD_USER;

        /// <summary>
        /// MOD_USER
        /// </summary>
        public static string M_MOD_USER = "MOD_USER";

        private string _MOD_DATE;

        /// <summary>
        /// MOD_DATE
        /// </summary>
        public static string M_MOD_DATE = "MOD_DATE";
        

        /// <summary>
        /// DOC_ID
        /// </summary>
        [AttributeField("DOC_ID", "System.String", false, true, false, "String")]
        public string DOC_ID
        {
            get
            {
                return this._DOC_ID;
            }
            set
            {
                this._DOC_ID = value;
            }
        }

        /// <summary>
        /// CORP_NO
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
        /// CORP_SEQ
        /// </summary>
        [AttributeField("CORP_SEQ", "System.String", false, false, false, "String")]
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
        /// KeyinFLAG
        /// </summary>
        [AttributeField("KeyinFLAG", "System.String", false, false, false, "String")]
        public string KeyinFLAG
        {
            get
            {
                return this._KeyinFLAG;
            }
            set
            {
                this._KeyinFLAG = value;
            }
        }
        
        /// <summary>
        /// MOD_USER
        /// </summary>
        [AttributeField("MOD_USER", "System.String", false, false, false, "String")]
        public string MOD_USER
        {
            get
            {
                return this._MOD_USER;
            }
            set
            {
                this._MOD_USER = value;
            }
        }

        /// <summary>
        /// MOD_DATE
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
    }

    /// <summary>
    /// EntityShopChange_LOG
    /// </summary>
    [Serializable()]
    public class EntityShopChange_LOGSet : EntitySet<EntityShopChange_LOG>
    {
    }
}
