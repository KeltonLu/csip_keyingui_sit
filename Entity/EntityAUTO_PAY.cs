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
    /// Auto_Pay
    /// </summary>
    [Serializable()]
    [AttributeTable("Auto_Pay")]
    public class EntityAUTO_PAY : Entity
    {
        
        private string _Cus_ID;
        
        /// <summary>
        /// Cus_ID
        /// </summary>
        public static string M_Cus_ID = "Cus_ID";
        
        private string _Acc_No;
        
        /// <summary>
        /// Acc_No
        /// </summary>
        public static string M_Acc_No = "Acc_No";
        
        private string _Pay_Way;
        
        /// <summary>
        /// Pay_Way
        /// </summary>
        public static string M_Pay_Way = "Pay_Way";
        
        private string _Acc_ID;
        
        /// <summary>
        /// Acc_ID
        /// </summary>
        public static string M_Acc_ID = "Acc_ID";
        
        private string _bcycle_code;
        
        /// <summary>
        /// bcycle_code
        /// </summary>
        public static string M_bcycle_code = "bcycle_code";
        
        private string _Mobile_Phone;
        
        /// <summary>
        /// Mobile_Phone
        /// </summary>
        public static string M_Mobile_Phone = "Mobile_Phone";
        
        private string _E_Mail;
        
        /// <summary>
        /// E_Mail
        /// </summary>
        public static string M_E_Mail = "E_Mail";
        
        private string _E_Bill;
        
        /// <summary>
        /// E_Bill
        /// </summary>
        public static string M_E_Bill = "E_Bill";
        
        private string _KeyIn_Flag;
        
        /// <summary>
        /// KeyIn_Flag
        /// </summary>
        public static string M_KeyIn_Flag = "KeyIn_Flag";
        
        private string _Upload_Flag;
        
        /// <summary>
        /// Upload_Flag
        /// </summary>
        public static string M_Upload_Flag = "Upload_Flag";
        
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
        
        private string _Receive_Number;
        
        /// <summary>
        /// Receive_Number
        /// </summary>
        public static string M_Receive_Number = "Receive_Number";
        
        private string _Add_Flag;
        
        /// <summary>
        /// Add_Flag
        /// </summary>
        public static string M_Add_Flag = "Add_Flag";
        
        private string _Auto_Pay_Setting;
        
        /// <summary>
        /// Auto_Pay_Setting
        /// </summary>
        public static string M_Auto_Pay_Setting = "Auto_Pay_Setting";
        
        private string _CellP_Email_Setting;
        
        /// <summary>
        /// CellP_Email_Setting
        /// </summary>
        public static string M_CellP_Email_Setting = "CellP_Email_Setting";
        
        private string _E_Bill_Setting;
        
        /// <summary>
        /// E_Bill_Setting
        /// </summary>
        public static string M_E_Bill_Setting = "E_Bill_Setting";
        
        private string _OutputByTXT_Setting;
        
        /// <summary>
        /// OutputByTXT_Setting
        /// </summary>
        public static string M_OutputByTXT_Setting = "OutputByTXT_Setting";
        
        private string _Acct_NBR;
        
        /// <summary>
        /// Acct_NBR
        /// </summary>
        public static string M_Acct_NBR = "Acct_NBR";
        
        /// <summary>
        /// Cus_ID
        /// </summary>
        [AttributeField("Cus_ID", "System.String", false, false, false, "String")]
        public string Cus_ID
        {
            get
            {
                return this._Cus_ID;
            }
            set
            {
                this._Cus_ID = value;
            }
        }
        
        /// <summary>
        /// Acc_No
        /// </summary>
        [AttributeField("Acc_No", "System.String", false, false, false, "String")]
        public string Acc_No
        {
            get
            {
                return this._Acc_No;
            }
            set
            {
                this._Acc_No = value;
            }
        }
        
        /// <summary>
        /// Pay_Way
        /// </summary>
        [AttributeField("Pay_Way", "System.String", false, false, false, "String")]
        public string Pay_Way
        {
            get
            {
                return this._Pay_Way;
            }
            set
            {
                this._Pay_Way = value;
            }
        }
        
        /// <summary>
        /// Acc_ID
        /// </summary>
        [AttributeField("Acc_ID", "System.String", false, false, false, "String")]
        public string Acc_ID
        {
            get
            {
                return this._Acc_ID;
            }
            set
            {
                this._Acc_ID = value;
            }
        }
        
        /// <summary>
        /// bcycle_code
        /// </summary>
        [AttributeField("bcycle_code", "System.string", false, false, false, "String")]
        public string bcycle_code
        {
            get
            {
                return this._bcycle_code;
            }
            set
            {
                this._bcycle_code = value;
            }
        }
        
        /// <summary>
        /// Mobile_Phone
        /// </summary>
        [AttributeField("Mobile_Phone", "System.String", false, false, false, "String")]
        public string Mobile_Phone
        {
            get
            {
                return this._Mobile_Phone;
            }
            set
            {
                this._Mobile_Phone = value;
            }
        }
        
        /// <summary>
        /// E_Mail
        /// </summary>
        [AttributeField("E_Mail", "System.String", false, false, false, "String")]
        public string E_Mail
        {
            get
            {
                return this._E_Mail;
            }
            set
            {
                this._E_Mail = value;
            }
        }
        
        /// <summary>
        /// E_Bill
        /// </summary>
        [AttributeField("E_Bill", "System.String", false, false, false, "String")]
        public string E_Bill
        {
            get
            {
                return this._E_Bill;
            }
            set
            {
                this._E_Bill = value;
            }
        }
        
        /// <summary>
        /// KeyIn_Flag
        /// </summary>
        [AttributeField("KeyIn_Flag", "System.string", false, false, false, "String")]
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
        /// Upload_Flag
        /// </summary>
        [AttributeField("Upload_Flag", "System.string", false, false, false, "String")]
        public string Upload_Flag
        {
            get
            {
                return this._Upload_Flag;
            }
            set
            {
                this._Upload_Flag = value;
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
        [AttributeField("mod_date", "System.string", false, false, false, "String")]
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
        /// Receive_Number
        /// </summary>
        [AttributeField("Receive_Number", "System.String", false, false, false, "String")]
        public string Receive_Number
        {
            get
            {
                return this._Receive_Number;
            }
            set
            {
                this._Receive_Number = value;
            }
        }
        
        /// <summary>
        /// Add_Flag
        /// </summary>
        [AttributeField("Add_Flag", "System.String", false, false, false, "String")]
        public string Add_Flag
        {
            get
            {
                return this._Add_Flag;
            }
            set
            {
                this._Add_Flag = value;
            }
        }
        
        /// <summary>
        /// Auto_Pay_Setting
        /// </summary>
        [AttributeField("Auto_Pay_Setting", "System.String", false, false, false, "String")]
        public string Auto_Pay_Setting
        {
            get
            {
                return this._Auto_Pay_Setting;
            }
            set
            {
                this._Auto_Pay_Setting = value;
            }
        }
        
        /// <summary>
        /// CellP_Email_Setting
        /// </summary>
        [AttributeField("CellP_Email_Setting", "System.String", false, false, false, "String")]
        public string CellP_Email_Setting
        {
            get
            {
                return this._CellP_Email_Setting;
            }
            set
            {
                this._CellP_Email_Setting = value;
            }
        }
        
        /// <summary>
        /// E_Bill_Setting
        /// </summary>
        [AttributeField("E_Bill_Setting", "System.String", false, false, false, "String")]
        public string E_Bill_Setting
        {
            get
            {
                return this._E_Bill_Setting;
            }
            set
            {
                this._E_Bill_Setting = value;
            }
        }
        
        /// <summary>
        /// OutputByTXT_Setting
        /// </summary>
        [AttributeField("OutputByTXT_Setting", "System.String", false, false, false, "String")]
        public string OutputByTXT_Setting
        {
            get
            {
                return this._OutputByTXT_Setting;
            }
            set
            {
                this._OutputByTXT_Setting = value;
            }
        }
        
        /// <summary>
        /// Acct_NBR
        /// </summary>
        [AttributeField("Acct_NBR", "System.String", false, false, false, "String")]
        public string Acct_NBR
        {
            get
            {
                return this._Acct_NBR;
            }
            set
            {
                this._Acct_NBR = value;
            }
        }
    }
    
    /// <summary>
    /// Auto_Pay
    /// </summary>
    [Serializable()]
    public class Auto_PaySet : EntitySet<EntityAUTO_PAY>
    {
    }
}
