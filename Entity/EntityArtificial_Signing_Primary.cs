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
    /// Artificial_Signing_Primary
    /// </summary>
    [Serializable()]
    [AttributeTable("Artificial_Signing_Primary")]
    public class EntityArtificial_Signing_Primary : Entity
    {
        
        private object _guid;
        
        /// <summary>
        /// guid
        /// </summary>
        public static string M_guid = "guid";
        
        private object _Import_Date;
        
        /// <summary>
        /// Import_Date
        /// </summary>
        public static string M_Import_Date = "Import_Date";
        
        private string _Shop_ID;
        
        /// <summary>
        /// Shop_ID
        /// </summary>
        public static string M_Shop_ID = "Shop_ID";
        
        private int _Batch_NO;
        
        /// <summary>
        /// Batch_NO
        /// </summary>
        public static string M_Batch_NO = "Batch_NO";
        
        private int _Receive_Batch;
        
        /// <summary>
        /// Receive_Batch
        /// </summary>
        public static string M_Receive_Batch = "Receive_Batch";
        
        private char _Sign_Type;
        
        /// <summary>
        /// Sign_Type
        /// </summary>
        public static string M_Sign_Type = "Sign_Type";
        
        private System.Nullable<decimal> _Keyin_Success_Count_All;
        
        /// <summary>
        /// Keyin_Success_Count_All
        /// </summary>
        public static string M_Keyin_Success_Count_All = "Keyin_Success_Count_All";
        
        private System.Nullable<decimal> _Keyin_Success_Count_40;
        
        /// <summary>
        /// Keyin_Success_Count_40
        /// </summary>
        public static string M_Keyin_Success_Count_40 = "Keyin_Success_Count_40";
        
        private System.Nullable<decimal> _Keyin_Success_Count_41;
        
        /// <summary>
        /// Keyin_Success_Count_41
        /// </summary>
        public static string M_Keyin_Success_Count_41 = "Keyin_Success_Count_41";
        
        private System.Nullable<decimal> _Keyin_Success_AMT_All;
        
        /// <summary>
        /// Keyin_Success_AMT_All
        /// </summary>
        public static string M_Keyin_Success_AMT_All = "Keyin_Success_AMT_All";
        
        private System.Nullable<decimal> _Keyin_Success_AMT_40;
        
        /// <summary>
        /// Keyin_Success_AMT_40
        /// </summary>
        public static string M_Keyin_Success_AMT_40 = "Keyin_Success_AMT_40";
        
        private System.Nullable<decimal> _Keyin_Success_AMT_41;
        
        /// <summary>
        /// Keyin_Success_AMT_41
        /// </summary>
        public static string M_Keyin_Success_AMT_41 = "Keyin_Success_AMT_41";
        
        private System.Nullable<decimal> _Keyin_Reject_Count_All;
        
        /// <summary>
        /// Keyin_Reject_Count_All
        /// </summary>
        public static string M_Keyin_Reject_Count_All = "Keyin_Reject_Count_All";
        
        private System.Nullable<decimal> _Keyin_Reject_Count_40;
        
        /// <summary>
        /// Keyin_Reject_Count_40
        /// </summary>
        public static string M_Keyin_Reject_Count_40 = "Keyin_Reject_Count_40";
        
        private System.Nullable<decimal> _Keyin_Reject_Count_41;
        
        /// <summary>
        /// Keyin_Reject_Count_41
        /// </summary>
        public static string M_Keyin_Reject_Count_41 = "Keyin_Reject_Count_41";
        
        private System.Nullable<decimal> _Keyin_Reject_AMT_All;
        
        /// <summary>
        /// Keyin_Reject_AMT_All
        /// </summary>
        public static string M_Keyin_Reject_AMT_All = "Keyin_Reject_AMT_All";
        
        private System.Nullable<decimal> _Keyin_Reject_AMT_40;
        
        /// <summary>
        /// Keyin_Reject_AMT_40
        /// </summary>
        public static string M_Keyin_Reject_AMT_40 = "Keyin_Reject_AMT_40";
        
        private System.Nullable<decimal> _Keyin_Reject_AMT_41;
        
        /// <summary>
        /// Keyin_Reject_AMT_41
        /// </summary>
        public static string M_Keyin_Reject_AMT_41 = "Keyin_Reject_AMT_41";
        
        private System.Nullable<decimal> _First_Balance_Count;
        
        /// <summary>
        /// First_Balance_Count
        /// </summary>
        public static string M_First_Balance_Count = "First_Balance_Count";
        
        private System.Nullable<decimal> _First_Balance_AMT;
        
        /// <summary>
        /// First_Balance_AMT
        /// </summary>
        public static string M_First_Balance_AMT = "First_Balance_AMT";
        
        private System.Nullable<decimal> _Adjust_Count;
        
        /// <summary>
        /// Adjust_Count
        /// </summary>
        public static string M_Adjust_Count = "Adjust_Count";
        
        private System.Nullable<decimal> _Adjust_AMT;
        
        /// <summary>
        /// Adjust_AMT
        /// </summary>
        public static string M_Adjust_AMT = "Adjust_AMT";
        
        private System.Nullable<decimal> _Second_Balance_Count;
        
        /// <summary>
        /// Second_Balance_Count
        /// </summary>
        public static string M_Second_Balance_Count = "Second_Balance_Count";
        
        private System.Nullable<decimal> _Second_Balance_AMT;
        
        /// <summary>
        /// Second_Balance_AMT
        /// </summary>
        public static string M_Second_Balance_AMT = "Second_Balance_AMT";
        
        private char _First_Balance_Flag;
        
        /// <summary>
        /// First_Balance_Flag
        /// </summary>
        public static string M_First_Balance_Flag = "First_Balance_Flag";
        
        private char _Second_Balance_Flag;
        
        /// <summary>
        /// Second_Balance_Flag
        /// </summary>
        public static string M_Second_Balance_Flag = "Second_Balance_Flag";
        
        private char _Balance_Flag;
        
        /// <summary>
        /// Balance_Flag
        /// </summary>
        public static string M_Balance_Flag = "Balance_Flag";
        
        private char _KeyIn_Flag;
        
        /// <summary>
        /// KeyIn_Flag
        /// </summary>
        public static string M_KeyIn_Flag = "KeyIn_Flag";
        
        private char _KeyIn_MatchFlag;
        
        /// <summary>
        /// KeyIn_MatchFlag
        /// </summary>
        public static string M_KeyIn_MatchFlag = "KeyIn_MatchFlag";
        
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
        /// Import_Date
        /// </summary>
        [AttributeField("Import_Date", "System.Object", false, true, true, "Object")]
        public object Import_Date
        {
            get
            {
                return this._Import_Date;
            }
            set
            {
                this._Import_Date = value;
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
        /// Keyin_Success_Count_All
        /// </summary>
        [AttributeField("Keyin_Success_Count_All", "System.Nullable`1[System.Decimal]", false, false, false, "Decimal")]
        public System.Nullable<decimal> Keyin_Success_Count_All
        {
            get
            {
                return this._Keyin_Success_Count_All;
            }
            set
            {
                this._Keyin_Success_Count_All = value;
            }
        }
        
        /// <summary>
        /// Keyin_Success_Count_40
        /// </summary>
        [AttributeField("Keyin_Success_Count_40", "System.Nullable`1[System.Decimal]", false, false, false, "Decimal")]
        public System.Nullable<decimal> Keyin_Success_Count_40
        {
            get
            {
                return this._Keyin_Success_Count_40;
            }
            set
            {
                this._Keyin_Success_Count_40 = value;
            }
        }
        
        /// <summary>
        /// Keyin_Success_Count_41
        /// </summary>
        [AttributeField("Keyin_Success_Count_41", "System.Nullable`1[System.Decimal]", false, false, false, "Decimal")]
        public System.Nullable<decimal> Keyin_Success_Count_41
        {
            get
            {
                return this._Keyin_Success_Count_41;
            }
            set
            {
                this._Keyin_Success_Count_41 = value;
            }
        }
        
        /// <summary>
        /// Keyin_Success_AMT_All
        /// </summary>
        [AttributeField("Keyin_Success_AMT_All", "System.Nullable`1[System.Decimal]", false, false, false, "Decimal")]
        public System.Nullable<decimal> Keyin_Success_AMT_All
        {
            get
            {
                return this._Keyin_Success_AMT_All;
            }
            set
            {
                this._Keyin_Success_AMT_All = value;
            }
        }
        
        /// <summary>
        /// Keyin_Success_AMT_40
        /// </summary>
        [AttributeField("Keyin_Success_AMT_40", "System.Nullable`1[System.Decimal]", false, false, false, "Decimal")]
        public System.Nullable<decimal> Keyin_Success_AMT_40
        {
            get
            {
                return this._Keyin_Success_AMT_40;
            }
            set
            {
                this._Keyin_Success_AMT_40 = value;
            }
        }
        
        /// <summary>
        /// Keyin_Success_AMT_41
        /// </summary>
        [AttributeField("Keyin_Success_AMT_41", "System.Nullable`1[System.Decimal]", false, false, false, "Decimal")]
        public System.Nullable<decimal> Keyin_Success_AMT_41
        {
            get
            {
                return this._Keyin_Success_AMT_41;
            }
            set
            {
                this._Keyin_Success_AMT_41 = value;
            }
        }
        
        /// <summary>
        /// Keyin_Reject_Count_All
        /// </summary>
        [AttributeField("Keyin_Reject_Count_All", "System.Nullable`1[System.Decimal]", false, false, false, "Decimal")]
        public System.Nullable<decimal> Keyin_Reject_Count_All
        {
            get
            {
                return this._Keyin_Reject_Count_All;
            }
            set
            {
                this._Keyin_Reject_Count_All = value;
            }
        }
        
        /// <summary>
        /// Keyin_Reject_Count_40
        /// </summary>
        [AttributeField("Keyin_Reject_Count_40", "System.Nullable`1[System.Decimal]", false, false, false, "Decimal")]
        public System.Nullable<decimal> Keyin_Reject_Count_40
        {
            get
            {
                return this._Keyin_Reject_Count_40;
            }
            set
            {
                this._Keyin_Reject_Count_40 = value;
            }
        }
        
        /// <summary>
        /// Keyin_Reject_Count_41
        /// </summary>
        [AttributeField("Keyin_Reject_Count_41", "System.Nullable`1[System.Decimal]", false, false, false, "Decimal")]
        public System.Nullable<decimal> Keyin_Reject_Count_41
        {
            get
            {
                return this._Keyin_Reject_Count_41;
            }
            set
            {
                this._Keyin_Reject_Count_41 = value;
            }
        }
        
        /// <summary>
        /// Keyin_Reject_AMT_All
        /// </summary>
        [AttributeField("Keyin_Reject_AMT_All", "System.Nullable`1[System.Decimal]", false, false, false, "Decimal")]
        public System.Nullable<decimal> Keyin_Reject_AMT_All
        {
            get
            {
                return this._Keyin_Reject_AMT_All;
            }
            set
            {
                this._Keyin_Reject_AMT_All = value;
            }
        }
        
        /// <summary>
        /// Keyin_Reject_AMT_40
        /// </summary>
        [AttributeField("Keyin_Reject_AMT_40", "System.Nullable`1[System.Decimal]", false, false, false, "Decimal")]
        public System.Nullable<decimal> Keyin_Reject_AMT_40
        {
            get
            {
                return this._Keyin_Reject_AMT_40;
            }
            set
            {
                this._Keyin_Reject_AMT_40 = value;
            }
        }
        
        /// <summary>
        /// Keyin_Reject_AMT_41
        /// </summary>
        [AttributeField("Keyin_Reject_AMT_41", "System.Nullable`1[System.Decimal]", false, false, false, "Decimal")]
        public System.Nullable<decimal> Keyin_Reject_AMT_41
        {
            get
            {
                return this._Keyin_Reject_AMT_41;
            }
            set
            {
                this._Keyin_Reject_AMT_41 = value;
            }
        }
        
        /// <summary>
        /// First_Balance_Count
        /// </summary>
        [AttributeField("First_Balance_Count", "System.Nullable`1[System.Decimal]", false, false, false, "Decimal")]
        public System.Nullable<decimal> First_Balance_Count
        {
            get
            {
                return this._First_Balance_Count;
            }
            set
            {
                this._First_Balance_Count = value;
            }
        }
        
        /// <summary>
        /// First_Balance_AMT
        /// </summary>
        [AttributeField("First_Balance_AMT", "System.Nullable`1[System.Decimal]", false, false, false, "Decimal")]
        public System.Nullable<decimal> First_Balance_AMT
        {
            get
            {
                return this._First_Balance_AMT;
            }
            set
            {
                this._First_Balance_AMT = value;
            }
        }
        
        /// <summary>
        /// Adjust_Count
        /// </summary>
        [AttributeField("Adjust_Count", "System.Nullable`1[System.Decimal]", false, false, false, "Decimal")]
        public System.Nullable<decimal> Adjust_Count
        {
            get
            {
                return this._Adjust_Count;
            }
            set
            {
                this._Adjust_Count = value;
            }
        }
        
        /// <summary>
        /// Adjust_AMT
        /// </summary>
        [AttributeField("Adjust_AMT", "System.Nullable`1[System.Decimal]", false, false, false, "Decimal")]
        public System.Nullable<decimal> Adjust_AMT
        {
            get
            {
                return this._Adjust_AMT;
            }
            set
            {
                this._Adjust_AMT = value;
            }
        }
        
        /// <summary>
        /// Second_Balance_Count
        /// </summary>
        [AttributeField("Second_Balance_Count", "System.Nullable`1[System.Decimal]", false, false, false, "Decimal")]
        public System.Nullable<decimal> Second_Balance_Count
        {
            get
            {
                return this._Second_Balance_Count;
            }
            set
            {
                this._Second_Balance_Count = value;
            }
        }
        
        /// <summary>
        /// Second_Balance_AMT
        /// </summary>
        [AttributeField("Second_Balance_AMT", "System.Nullable`1[System.Decimal]", false, false, false, "Decimal")]
        public System.Nullable<decimal> Second_Balance_AMT
        {
            get
            {
                return this._Second_Balance_AMT;
            }
            set
            {
                this._Second_Balance_AMT = value;
            }
        }
        
        /// <summary>
        /// First_Balance_Flag
        /// </summary>
        [AttributeField("First_Balance_Flag", "System.Char", false, false, false, "String")]
        public char First_Balance_Flag
        {
            get
            {
                return this._First_Balance_Flag;
            }
            set
            {
                this._First_Balance_Flag = value;
            }
        }
        
        /// <summary>
        /// Second_Balance_Flag
        /// </summary>
        [AttributeField("Second_Balance_Flag", "System.Char", false, false, false, "String")]
        public char Second_Balance_Flag
        {
            get
            {
                return this._Second_Balance_Flag;
            }
            set
            {
                this._Second_Balance_Flag = value;
            }
        }
        
        /// <summary>
        /// Balance_Flag
        /// </summary>
        [AttributeField("Balance_Flag", "System.Char", false, false, false, "String")]
        public char Balance_Flag
        {
            get
            {
                return this._Balance_Flag;
            }
            set
            {
                this._Balance_Flag = value;
            }
        }
        
        /// <summary>
        /// KeyIn_Flag
        /// </summary>
        [AttributeField("KeyIn_Flag", "System.Char", false, false, false, "String")]
        public char KeyIn_Flag
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
        /// KeyIn_MatchFlag
        /// </summary>
        [AttributeField("KeyIn_MatchFlag", "System.Char", false, false, false, "String")]
        public char KeyIn_MatchFlag
        {
            get
            {
                return this._KeyIn_MatchFlag;
            }
            set
            {
                this._KeyIn_MatchFlag = value;
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
    /// Artificial_Signing_Primary
    /// </summary>
    [Serializable()]
    public class EntityArtificial_Signing_PrimarySet : EntitySet<EntityArtificial_Signing_Primary>
    {
    }
}
