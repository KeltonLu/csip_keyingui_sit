//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:2.0.50727.5477
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
    /// Artificial_Signing_Batch_Data
    /// </summary>
    [Serializable()]
    [AttributeTable("Artificial_Signing_Batch_Data")]
    public class EntityArtificial_Signing_Batch_Data : Entity
    {
        
        private object _guid;
        
        /// <summary>
        /// guid
        /// </summary>
        public static string M_guid = "guid";
        
        private object _Batch_Date;
        
        /// <summary>
        /// Batch_Date
        /// </summary>
        public static string M_Batch_Date = "Batch_Date";
        
        private int _Receive_Batch;
        
        /// <summary>
        /// Receive_Batch
        /// </summary>
        public static string M_Receive_Batch = "Receive_Batch";
        
        private System.Nullable<int> _Page;
        
        /// <summary>
        /// Page
        /// </summary>
        public static string M_Page = "Page";
        
        private int _Batch_NO;
        
        /// <summary>
        /// Batch_NO
        /// </summary>
        public static string M_Batch_NO = "Batch_NO";
        
        private string _Shop_ID;
        
        /// <summary>
        /// Shop_ID
        /// </summary>
        public static string M_Shop_ID = "Shop_ID";
        
        private char _Sign_Type;
        
        /// <summary>
        /// Sign_Type
        /// </summary>
        public static string M_Sign_Type = "Sign_Type";
        
        private System.Nullable<decimal> _Receive_Total_Count;
        
        /// <summary>
        /// Receive_Total_Count
        /// </summary>
        public static string M_Receive_Total_Count = "Receive_Total_Count";
        
        private System.Nullable<decimal> _Receive_Total_AMT;
        
        /// <summary>
        /// Receive_Total_AMT
        /// </summary>
        public static string M_Receive_Total_AMT = "Receive_Total_AMT";
        
        private char _Process_Flag;
        
        /// <summary>
        /// Process_Flag
        /// </summary>
        public static string M_Process_Flag = "Process_Flag";
        
        private string _Create_User;
        
        /// <summary>
        /// Create_User
        /// </summary>
        public static string M_Create_User = "Create_User";
        
        private object _Create_DateTime;
        
        /// <summary>
        /// Create_DateTime
        /// </summary>
        public static string M_Create_DateTime = "Create_DateTime";
        
        private string _Modify_User;
        
        /// <summary>
        /// Modify_User
        /// </summary>
        public static string M_Modify_User = "Modify_User";
        
        private object _Modify_DateTime;
        
        /// <summary>
        /// Modify_DateTime
        /// </summary>
        public static string M_Modify_DateTime = "Modify_DateTime";
        
        /// <summary>
        /// guid
        /// </summary>
        [AttributeField("guid", "System.Object", false, false, false, "Object")]
        public object guid
        {
            get
            {
                return this._guid;
            }
            set
            {
                this._guid = value;
            }
        }
        
        /// <summary>
        /// Batch_Date
        /// </summary>
        [AttributeField("Batch_Date", "System.Object", false, true, true, "Object")]
        public object Batch_Date
        {
            get
            {
                return this._Batch_Date;
            }
            set
            {
                this._Batch_Date = value;
            }
        }
        
        /// <summary>
        /// Receive_Batch
        /// </summary>
        [AttributeField("Receive_Batch", "System.Int32", false, true, true, "Int32")]
        public int Receive_Batch
        {
            get
            {
                return this._Receive_Batch;
            }
            set
            {
                this._Receive_Batch = value;
            }
        }
        
        /// <summary>
        /// Page
        /// </summary>
        [AttributeField("Page", "System.Nullable`1[System.Int32]", false, false, false, "Int32")]
        public System.Nullable<int> Page
        {
            get
            {
                return this._Page;
            }
            set
            {
                this._Page = value;
            }
        }
        
        /// <summary>
        /// Batch_NO
        /// </summary>
        [AttributeField("Batch_NO", "System.Int32", false, true, true, "Int32")]
        public int Batch_NO
        {
            get
            {
                return this._Batch_NO;
            }
            set
            {
                this._Batch_NO = value;
            }
        }
        
        /// <summary>
        /// Shop_ID
        /// </summary>
        [AttributeField("Shop_ID", "System.String", false, true, true, "String")]
        public string Shop_ID
        {
            get
            {
                return this._Shop_ID;
            }
            set
            {
                this._Shop_ID = value;
            }
        }
        
        /// <summary>
        /// Sign_Type
        /// </summary>
        [AttributeField("Sign_Type", "System.Char", false, false, false, "String")]
        public char Sign_Type
        {
            get
            {
                return this._Sign_Type;
            }
            set
            {
                this._Sign_Type = value;
            }
        }
        
        /// <summary>
        /// Receive_Total_Count
        /// </summary>
        [AttributeField("Receive_Total_Count", "System.Nullable`1[System.Decimal]", false, false, false, "Decimal")]
        public System.Nullable<decimal> Receive_Total_Count
        {
            get
            {
                return this._Receive_Total_Count;
            }
            set
            {
                this._Receive_Total_Count = value;
            }
        }
        
        /// <summary>
        /// Receive_Total_AMT
        /// </summary>
        [AttributeField("Receive_Total_AMT", "System.Nullable`1[System.Decimal]", false, false, false, "Decimal")]
        public System.Nullable<decimal> Receive_Total_AMT
        {
            get
            {
                return this._Receive_Total_AMT;
            }
            set
            {
                this._Receive_Total_AMT = value;
            }
        }
        
        /// <summary>
        /// Process_Flag
        /// </summary>
        [AttributeField("Process_Flag", "System.Char", false, false, false, "String")]
        public char Process_Flag
        {
            get
            {
                return this._Process_Flag;
            }
            set
            {
                this._Process_Flag = value;
            }
        }
        
        /// <summary>
        /// Create_User
        /// </summary>
        [AttributeField("Create_User", "System.String", false, false, false, "String")]
        public string Create_User
        {
            get
            {
                return this._Create_User;
            }
            set
            {
                this._Create_User = value;
            }
        }
        
        /// <summary>
        /// Create_DateTime
        /// </summary>
        [AttributeField("Create_DateTime", "System.DateTime", false, false, false, "DateTime")]
        public object Create_DateTime
        {
            get
            {
                return this._Create_DateTime;
            }
            set
            {
                this._Create_DateTime = value;
            }
        }
        
        /// <summary>
        /// Modify_User
        /// </summary>
        [AttributeField("Modify_User", "System.String", false, false, false, "String")]
        public string Modify_User
        {
            get
            {
                return this._Modify_User;
            }
            set
            {
                this._Modify_User = value;
            }
        }
        
        /// <summary>
        /// Modify_DateTime
        /// </summary>
        [AttributeField("Modify_DateTime", "System.DateTime", false, false, false, "DateTime")]
        public object Modify_DateTime
        {
            get
            {
                return this._Modify_DateTime;
            }
            set
            {
                this._Modify_DateTime = value;
            }
        }
    }
    
    /// <summary>
    /// Artificial_Signing_Batch_Data
    /// </summary>
    [Serializable()]
    public class EntityArtificial_Signing_Batch_DataSet : EntitySet<EntityArtificial_Signing_Batch_Data>
    {
    }
}
