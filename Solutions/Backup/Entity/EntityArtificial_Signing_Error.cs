//------------------------------------------------------------------------------
// <auto-generated>
//     �o�q�{���X�O�Ѥu�㲣�ͪ��C
//     ���涥�q����:2.0.50727.5477
//
//     ��o���ɮשҰ����ܧ�i��|�y�����~���欰�A�ӥB�p�G���s���͵{���X�A
//     �ܧ�N�|�򥢡C
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
    /// Artificial_Signing_Error
    /// </summary>
    [Serializable()]
    [AttributeTable("Artificial_Signing_Error")]
    public class EntityArtificial_Signing_Error : Entity
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
        
        private System.Nullable<int> _SN;
        
        /// <summary>
        /// SN
        /// </summary>
        public static string M_SN = "SN";
        
        private string _Error_Column;
        
        /// <summary>
        /// Error_Column
        /// </summary>
        public static string M_Error_Column = "Error_Column";
        
        private string _Correct_Value;
        
        /// <summary>
        /// Correct_Value
        /// </summary>
        public static string M_Correct_Value = "Correct_Value";
        
        private string _Reflect_Source;
        
        /// <summary>
        /// Reflect_Source
        /// </summary>
        public static string M_Reflect_Source = "Reflect_Source";
        
        private string _Error_Value;
        
        /// <summary>
        /// Error_Value
        /// </summary>
        public static string M_Error_Value = "Error_Value";
        
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
        /// SN
        /// </summary>
        [AttributeField("SN", "System.Nullable`1[System.Int32]", false, false, false, "Int32")]
        public System.Nullable<int> SN
        {
            get
            {
                return this._SN;
            }
            set
            {
                this._SN = value;
            }
        }
        
        /// <summary>
        /// Error_Column
        /// </summary>
        [AttributeField("Error_Column", "System.String", false, false, false, "String")]
        public string Error_Column
        {
            get
            {
                return this._Error_Column;
            }
            set
            {
                this._Error_Column = value;
            }
        }
        
        /// <summary>
        /// Correct_Value
        /// </summary>
        [AttributeField("Correct_Value", "System.String", false, false, false, "String")]
        public string Correct_Value
        {
            get
            {
                return this._Correct_Value;
            }
            set
            {
                this._Correct_Value = value;
            }
        }
        
        /// <summary>
        /// Reflect_Source
        /// </summary>
        [AttributeField("Reflect_Source", "System.String", false, false, false, "String")]
        public string Reflect_Source
        {
            get
            {
                return this._Reflect_Source;
            }
            set
            {
                this._Reflect_Source = value;
            }
        }
        
        /// <summary>
        /// Error_Value
        /// </summary>
        [AttributeField("Error_Value", "System.String", false, false, false, "String")]
        public string Error_Value
        {
            get
            {
                return this._Error_Value;
            }
            set
            {
                this._Error_Value = value;
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
    /// Artificial_Signing_Error
    /// </summary>
    [Serializable()]
    public class EntityArtificial_Signing_ErrorSet : EntitySet<EntityArtificial_Signing_Error>
    {
    }
}