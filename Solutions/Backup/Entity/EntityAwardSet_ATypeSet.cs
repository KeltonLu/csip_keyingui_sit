//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:2.0.50727.3603
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
    /// AwardSet_ATypeSet
    /// </summary>
    [Serializable()]
    [AttributeTable("AwardSet_ATypeSet")]
    public class EntityAwardSet_ATypeSet : Entity
    {
        
        private string _RECEIVE_NUMBER;
        
        /// <summary>
        /// RECEIVE_NUMBER
        /// </summary>
        public static string M_RECEIVE_NUMBER = "RECEIVE_NUMBER";
        
        private string _CARD_TYPE;
        
        /// <summary>
        /// CARD_TYPE
        /// </summary>
        public static string M_CARD_TYPE = "CARD_TYPE";

        private string _TXT_INDEX;
        
        /// <summary>
        /// CARD_TYPE
        /// </summary>
        public static string M_TXT_INDEX = "TXT_INDEX";

        private string _KEYIN_FLAG;

        /// <summary>
        /// KEYIN_FLAG
        /// </summary>
        public static string M_KEYIN_FLAG = "KEYIN_FLAG";
        
        /// <summary>
        /// RECEIVE_NUMBER
        /// </summary>
        [AttributeField("RECEIVE_NUMBER", "System.String", false, false, false, "String")]
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
        /// CARD_TYPE
        /// </summary>
        [AttributeField("CARD_TYPE", "System.String", false, false, false, "String")]
        public string CARD_TYPE
        {
            get
            {
                return this._CARD_TYPE;
            }
            set
            {
                this._CARD_TYPE = value;
            }
        }

        /// <summary>
        /// TXT_INDEX
        /// </summary>
        [AttributeField("TXT_INDEX", "System.String", false, false, false, "String")]
        public string TXT_INDEX
        {
            get
            {
                return this._TXT_INDEX;
            }
            set
            {
                this._TXT_INDEX = value;
            }
        }

        /// <summary>
        /// KEYIN_FLAG
        /// </summary>
        [AttributeField("KEYIN_FLAG", "System.String", false, false, false, "String")]
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
    }
    
    /// <summary>
    /// AwardSet_ATypeSet
    /// </summary>
    [Serializable()]
    public class EntityAwardSet_ATypeSetSet : EntitySet<EntityAwardSet_ATypeSet>
    {
    }
}
