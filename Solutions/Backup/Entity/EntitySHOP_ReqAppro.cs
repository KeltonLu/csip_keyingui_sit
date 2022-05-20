using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;


namespace CSIPKeyInGUI.EntityLayer
{
    
    
    /// <summary>
    /// shop_Upload
    /// </summary>
    [Serializable()]
    [AttributeTable("shop_ReqAppro")]
    public class EntitySHOP_ReqAppro : Entity
    {
        
        private string _shop_id;
        
        /// <summary>
        /// shop_id
        /// </summary>
        public static string M_shop_id = "shop_id";
        
        private string _Change_Item;
        
        /// <summary>
        /// Change_Item
        /// </summary>
        public static string M_Change_Item = "Change_Item";
        
        private string _KeyIn_Flag;
        
        /// <summary>
        /// KeyIn_Flag
        /// </summary>
        public static string M_KeyIn_Flag = "KeyIn_Flag";
        
        private string _user_id;
        
        /// <summary>
        /// user_id
        /// </summary>
        public static string M_user_id = "user_id";
        
        private string _mod_date;
        
        /// <summary>
        /// mod_date
        /// </summary>
        public static string M_mod_date = "mod_date";
        
        private string _mod_time;
        
        /// <summary>
        /// mod_time
        /// </summary>
        public static string M_mod_time = "mod_time";

        private string _NewCreate_Flag;

        /// <summary>
        /// KeyIn_Flag
        /// </summary>
        public static string M_NewCreate_Flag = "NewCreate_Flag";

        private string _Record_Name;

        /// <summary>
        /// Record_Name
        /// </summary>
        public static string M_Record_Name = "Record_Name";

        private string _Business_Name;

        /// <summary>
        /// Business_Name
        /// </summary>
        public static string M_Business_Name = "Business_Name";

        private string _Merchant_Name;

        /// <summary>
        /// Merchant_Name
        /// </summary>
        public static string M_Merchant_Name = "Merchant_Name";

        private string _Before;

        /// <summary>
        /// before
        /// </summary>
        public static string M_Before = "Before";

        private string _After;

        /// <summary>
        /// after
        /// </summary>
        public static string M_After = "After";
        
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
        /// Change_Item
        /// </summary>
        [AttributeField("Change_Item", "System.String", false, false, false, "String")]
        public string Change_Item
        {
            get
            {
                return this._Change_Item;
            }
            set
            {
                this._Change_Item = value;
            }
        }
        
        /// <summary>
        /// KeyIn_Flag
        /// </summary>
        [AttributeField("KeyIn_Flag", "System.String", false, false, false, "String")]
        public string KeyIn_Flag
        {
            get
            {
                return this._KeyIn_Flag;
            }
            set
            {
                this._KeyIn_Flag = value;
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
        /// mod_date
        /// </summary>
        [AttributeField("mod_date", "System.String", false, false, false, "String")]
        public string mod_date
        {
            get
            {
                return this._mod_date;
            }
            set
            {
                this._mod_date = value;
            }
        }
        
        /// <summary>
        /// mod_time
        /// </summary>
        [AttributeField("mod_time", "System.String", false, false, false, "String")]
        public string mod_time
        {
            get
            {
                return this._mod_time;
            }
            set
            {
                this._mod_time = value;
            }
        }

        [AttributeField("NewCreate_Flag", "System.String", false, false, false, "String")]
        public string NewCreate_Flag
        {
            get
            {
                return this._NewCreate_Flag;
            }
            set
            {
                this._NewCreate_Flag = value;
            }
        }

        [AttributeField("Record_Name", "System.String", false, false, false, "String")]
        public string Record_Name
        {
            get
            {
                return this._Record_Name;
            }
            set
            {
                this._Record_Name = value;
            }
        }

        [AttributeField("Business_Name", "System.String", false, false, false, "String")]
        public string Business_Name
        {
            get
            {
                return this._Business_Name;
            }
            set
            {
                this._Business_Name = value;
            }
        }

        [AttributeField("Merchant_Name", "System.String", false, false, false, "String")]
        public string Merchant_Name
        {
            get
            {
                return this._Merchant_Name;
            }
            set
            {
                this._Merchant_Name = value;
            }
        }

        [AttributeField("Before", "System.String", false, false, false, "String")]
        public string Before
        {
            get
            {
                return this._Before;
            }
            set
            {
                this._Before = value;
            }
        }

        [AttributeField("After", "System.String", false, false, false, "String")]
        public string After
        {
            get
            {
                return this._After;
            }
            set
            {
                this._After = value;
            }
        }

    }
    
    /// <summary>
    /// shop_ReqAppro
    /// </summary>
    [Serializable()]
    public class EntitySHOP_ReqApproSet : EntitySet<EntitySHOP_ReqAppro>
    {
    }
}
