using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;

namespace CSIPKeyInGUI.EntityLayer
{
     /// <summary>
    /// Balance_Trans
    /// </summary>
    [Serializable()]
    [AttributeTable("Balance_Trans")]
    public class EntityAuto_Balance_Trans : Entity
    {
        private Guid _Newid;

        /// <summary>
        /// Newid
        /// </summary>
        public static Guid M_Newid = Guid.NewGuid();


        /// <summary>
        /// Newid
        /// </summary>
        [AttributeField("Newid", "System.GUID", false, false, false, "Guid")]
        public Guid Newid
        {
            get
            {
                return this._Newid;
            }
            set
            {
                this._Newid = value;
            }
        }

        private string _CardNo;

        /// <summary>
        /// CardNo
        /// </summary>
        public static string M_CardNo = "CardNo";


        /// <summary>
        /// CardNo
        /// </summary>
        [AttributeField("CardNo", "System.Guid", false, false, false, "String")]
        public string CardNo
        {
            get
            {
                return this._CardNo;
            }
            set
            {
                this._CardNo = value;
            }
        }

        private string _PID;

        /// <summary>
        /// PID
        /// </summary>
        public static string M_PID = "PID";


        /// <summary>
        /// PID
        /// </summary>
        [AttributeField("PID", "System.String", false, false, false, "String")]
        public string PID
        {
            get
            {
                return this._PID;
            }
            set
            {
                this._PID = value;
            }
        }

        private DateTime _Trans_Date;

        /// <summary>
        /// Trans_Date
        /// </summary>
        public static string M_Trans_Date = "Trans_Date";


        /// <summary>
        /// Trans_Date
        /// </summary>
        [AttributeField("Trans_Date", "System.DateTime", false, false, false, "DateTime")]
        public DateTime Trans_Date
        {
            get
            {
                return this._Trans_Date;
            }
            set
            {
                this._Trans_Date = value;
            }
        }

        private string _Reason_Code;

        /// <summary>
        /// Reason_Code
        /// </summary>
        public static string M_Reason_Code = "Reason_Code";


        /// <summary>
        /// Reason_Code
        /// </summary>
        [AttributeField("Reason_Code", "System.String", false, false, false, "String")]
        public string Reason_Code
        {
            get
            {
                return this._Reason_Code;
            }
            set
            {
                this._Reason_Code = value;
            }
        }

        private string _Memo;

        /// <summary>
        /// Memo
        /// </summary>
        public static string M_Memo = "Memo";


        /// <summary>
        /// Memo
        /// </summary>
        [AttributeField("Memo", "System.String", false, false, false, "String")]
        public string Memo
        {
            get
            {
                return this._Memo;
            }
            set
            {
                this._Memo = value;
            }
        }

        private string _Upload_Flag;

        /// <summary>
        /// Upload_Flag
        /// </summary>
        public static string M_Upload_Flag = "Upload_Flag";


        /// <summary>
        /// Upload_Flag
        /// </summary>
        [AttributeField("Upload_Flag", "System.String", false, false, false, "String")]
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

        private string _Process_Flag;

        /// <summary>
        /// Process_Flag
        /// </summary>
        public static string M_Process_Flag = "Process_Flag";


        /// <summary>
        /// Process_Flag
        /// </summary>
        [AttributeField("Process_Flag", "System.String", false, false, false, "String")]
        public string Process_Flag
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

        private string _Process_Note;

        /// <summary>
        /// Process_Note
        /// </summary>
        public static string M_Process_Note = "Process_Note";


        /// <summary>
        /// Process_Note
        /// </summary>
        [AttributeField("Process_Note", "System.String", false, false, false, "String")]
        public string Process_Note
        {
            get
            {
                return this._Process_Note;
            }
            set
            {
                this._Process_Note = value;
            }
        }

        private string _Create_User;

        /// <summary>
        /// Create_User
        /// </summary>
        public static string M_Create_User = "Create_User";


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

        private DateTime _Create_DateTime;

        /// <summary>
        /// Create_DateTime
        /// </summary>
        public static string M_Create_DateTime = "Create_DateTime";


        /// <summary>
        /// Create_DateTime
        /// </summary>
        [AttributeField("Create_DateTime", "System.DateTime", false, false, false, "DateTime")]
        public DateTime Create_DateTime
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

        private string _Modify_User;

        /// <summary>
        /// Modify_User
        /// </summary>
        public static string M_Modify_User = "Modify_User";


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

        private DateTime _Modify_DateTime;

        /// <summary>
        /// Modify_DateTime
        /// </summary>
        public static string M_Modify_DateTime = "Modify_DateTime";


        /// <summary>
        /// Modify_DateTime
        /// </summary>
        [AttributeField("Modify_DateTime", "System.DateTime", false, false, false, "DateTime")]
        public DateTime Modify_DateTime
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
    /// Balance_Trans
    /// </summary>
    [Serializable()]
    public class Balance_TransSet : EntitySet<EntityAuto_Balance_Trans>
    {
    }
}
