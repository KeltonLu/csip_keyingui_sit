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
    /// Artificial_Signing_Detail
    /// </summary>
    [Serializable()]
    [AttributeTable("Artificial_Signing_Detail")]
    public class EntityArtificial_Signing_Detail : Entity
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
        
        private System.Nullable<int> _SN;
        
        /// <summary>
        /// SN
        /// </summary>
        public static string M_SN = "SN";
        
        private string _Card_No;
        
        /// <summary>
        /// Card_No
        /// </summary>
        public static string M_Card_No = "Card_No";
        
        private object _Tran_Date;
        
        /// <summary>
        /// Tran_Date
        /// </summary>
        public static string M_Tran_Date = "Tran_Date";
        
        private char _Product_Type;
        
        /// <summary>
        /// Product_Type
        /// </summary>
        public static string M_Product_Type = "Product_Type";
        
        private char _Installment_Periods;
        
        /// <summary>
        /// Installment_Periods
        /// </summary>
        public static string M_Installment_Periods = "Installment_Periods";
        
        private string _Auth_Code;
        
        /// <summary>
        /// Auth_Code
        /// </summary>
        public static string M_Auth_Code = "Auth_Code";
        
        private System.Nullable<decimal> _AMT;
        
        /// <summary>
        /// AMT
        /// </summary>
        public static string M_AMT = "AMT";
        
        private char _Receipt_Type;
        
        /// <summary>
        /// Receipt_Type
        /// </summary>
        public static string M_Receipt_Type = "Receipt_Type";
        
        private char _Case_Status;
        
        /// <summary>
        /// Case_Status
        /// </summary>
        public static string M_Case_Status = "Case_Status";
        
        private char _Reject_Reason;
        
        /// <summary>
        /// Reject_Reason
        /// </summary>
        public static string M_Reject_Reason = "Reject_Reason";
        
        private char _KeyIn_Flag;
        
        /// <summary>
        /// KeyIn_Flag
        /// </summary>
        public static string M_KeyIn_Flag = "KeyIn_Flag";
        
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
        /// Card_No
        /// </summary>
        [AttributeField("Card_No", "System.String", false, false, false, "String")]
        public string Card_No
        {
            get
            {
                return this._Card_No;
            }
            set
            {
                this._Card_No = value;
            }
        }
        
        /// <summary>
        /// Tran_Date
        /// </summary>
        [AttributeField("Tran_Date", "System.Object", false, false, false, "Object")]
        public object Tran_Date
        {
            get
            {
                return this._Tran_Date;
            }
            set
            {
                this._Tran_Date = value;
            }
        }
        
        /// <summary>
        /// Product_Type
        /// </summary>
        [AttributeField("Product_Type", "System.Char", false, false, false, "String")]
        public char Product_Type
        {
            get
            {
                return this._Product_Type;
            }
            set
            {
                this._Product_Type = value;
            }
        }
        
        /// <summary>
        /// Installment_Periods
        /// </summary>
        [AttributeField("Installment_Periods", "System.Char", false, false, false, "String")]
        public char Installment_Periods
        {
            get
            {
                return this._Installment_Periods;
            }
            set
            {
                this._Installment_Periods = value;
            }
        }
        
        /// <summary>
        /// Auth_Code
        /// </summary>
        [AttributeField("Auth_Code", "System.String", false, false, false, "String")]
        public string Auth_Code
        {
            get
            {
                return this._Auth_Code;
            }
            set
            {
                this._Auth_Code = value;
            }
        }
        
        /// <summary>
        /// AMT
        /// </summary>
        [AttributeField("AMT", "System.Nullable`1[System.Decimal]", false, false, false, "Decimal")]
        public System.Nullable<decimal> AMT
        {
            get
            {
                return this._AMT;
            }
            set
            {
                this._AMT = value;
            }
        }
        
        /// <summary>
        /// Receipt_Type
        /// </summary>
        [AttributeField("Receipt_Type", "System.Char", false, false, false, "String")]
        public char Receipt_Type
        {
            get
            {
                return this._Receipt_Type;
            }
            set
            {
                this._Receipt_Type = value;
            }
        }
        
        /// <summary>
        /// Case_Status
        /// </summary>
        [AttributeField("Case_Status", "System.Char", false, false, false, "String")]
        public char Case_Status
        {
            get
            {
                return this._Case_Status;
            }
            set
            {
                this._Case_Status = value;
            }
        }
        
        /// <summary>
        /// Reject_Reason
        /// </summary>
        [AttributeField("Reject_Reason", "System.Char", false, false, false, "String")]
        public char Reject_Reason
        {
            get
            {
                return this._Reject_Reason;
            }
            set
            {
                this._Reject_Reason = value;
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
    /// Artificial_Signing_Detail
    /// </summary>
    [Serializable()]
    public class EntityArtificial_Signing_DetailSet : EntitySet<EntityArtificial_Signing_Detail>
    {
    }
}