using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;


namespace CSIPNewInvoice.EntityLayer_new
{
    
    /// <summary>
    /// NoteLog
    /// </summary>
    [Serializable()]
    [AttributeTable("NoteLog")]
    public class Entity_NoteLog : Framework.Data.OM.Entity
    {

        //private int _FileId;

        ///// <summary>
        ///// FileId
        ///// </summary>
        //public static string M_FileId = "FileId";

        private string _NL_CASE_NO;

        /// <summary>
        /// NL_CASE_NO
        /// </summary>
        public static string M_NL_CASE_NO = "NL_CASE_NO";

        private string _NL_SecondKey;

        /// <summary>
        /// NL_SecondKey
        /// </summary>
        public static string M_NL_SecondKey = "NL_SecondKey";

        private DateTime _NL_DateTime;

        /// <summary>
        /// NL_DateTime
        /// </summary>
        public static DateTime M_NL_DateTime = DateTime.MinValue;

        private string _NL_User;

        /// <summary>
        /// NL_User
        /// </summary>
        public static string M_NL_User = "NL_User";

        private string _NL_Type;

        /// <summary>
        /// NL_Type
        /// </summary>
        public static string M_NL_Type = "NL_Type";

        private string _NL_Value;

        /// <summary>
        /// NL_Value
        /// </summary>
        public static string M_NL_Value = "NL_Value";

        private string _NL_ShowFlag;

        /// <summary>
        /// NL_ShowFlag
        /// </summary>
        public static string M_NL_ShowFlag = "NL_ShowFlag";

        /// <summary>
        /// NL_CASE_NO
        /// </summary>
        [AttributeField("NL_CASE_NO", "System.String", false, false, false, "String")]
        public string NL_CASE_NO
        {
            get
            {
                return this._NL_CASE_NO;
            }
            set
            {
                this._NL_CASE_NO = value;
            }
        }

        /// <summary>
        /// NL_SecondKey
        /// </summary>
        [AttributeField("NL_SecondKey", "System.String", false, false, false, "String")]
        public string NL_SecondKey
        {
            get
            {
                return this._NL_SecondKey;
            }
            set
            {
                this._NL_SecondKey = value;
            }
        }

        /// <summary>
        /// NL_DateTime
        /// </summary>
        [AttributeField("NL_DateTime", "System.DateTime", false, false, false, "DateTime")]
        public DateTime NL_DateTime
        {
            get
            {
                return this._NL_DateTime;
            }
            set
            {
                this._NL_DateTime = value;
            }
        }

        /// <summary>
        /// NL_User
        /// </summary>
        [AttributeField("NL_User", "System.string", false, false, false, "String")]
        public string NL_User
        {
            get
            {
                return this._NL_User;
            }
            set
            {
                this._NL_User = value;
            }
        }

        /// <summary>
        /// NL_Type
        /// </summary>
        [AttributeField("NL_Type", "System.string", false, false, false, "String")]
        public string NL_Type
        {
            get
            {
                return this._NL_Type;
            }
            set
            {
                this._NL_Type = value;
            }
        }

        /// <summary>
        /// NL_Value
        /// </summary>
        [AttributeField("NL_Value", "System.string", false, false, false, "String")]
        public string NL_Value
        {
            get
            {
                return this._NL_Value;
            }
            set
            {
                this._NL_Value = value;
            }
        }

        /// <summary>
        /// NL_ShowFlag
        /// </summary>
        [AttributeField("NL_ShowFlag", "System.string", false, false, false, "String")]
        public string NL_ShowFlag
        {
            get
            {
                return this._NL_ShowFlag;
            }
            set
            {
                this._NL_ShowFlag = value;
            }
        }
    }

    /// <summary>
    /// NoteLog
    /// </summary>
    [Serializable()]
    public class Entity_NoteLogSet : EntitySet<Entity_NoteLog>
    {
    }
}