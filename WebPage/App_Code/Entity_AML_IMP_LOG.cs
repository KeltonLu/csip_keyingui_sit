using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Collections.Generic;
using System.Text;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;

namespace CSIPNewInvoice.EntityLayer_new
{
    /// <summary>
    /// AML_File_Import
    /// </summary>
    [Serializable()]
    [AttributeTable("AML_IMP_LOG")]
    public class Entity_AML_IMP_LOG : Framework.Data.OM.Entity
    {
        private int _SEQ;
        /// < summary>
        /// SEQ
        /// < /summary>
        public static string M_SEQ = "SEQ";
        private string _FILE_TYPE;
        /// < summary>
        /// FILE_TYPE
        /// < /summary>
        public static string M_FILE_TYPE = "FILE_TYPE";
        private string _FileName;
        /// < summary>
        /// FileName
        /// < /summary>
        public static string M_FileName = "FileName";
        private string _IMP_DATE;
        /// < summary>
        /// IMP_DATE
        /// < /summary>
        public static string M_IMP_DATE = "IMP_DATE";
        private string _IMP_TIME;
        /// < summary>
        /// IMP_TIME
        /// < /summary>
        public static string M_IMP_TIME = "IMP_TIME";
        private string _Create_User;
        /// < summary>
        /// Create_User
        /// < /summary>
        public static string M_Create_User = "Create_User";
        private int _Count;
        /// < summary>
        /// Count
        /// < /summary>
        public static string M_Count = "Count";
        private string _Status;
        /// < summary>
        /// Status
        /// < /summary>
        public static string M_Status = "Status";
        private int _IMP_OK_COUNT;
        /// < summary>
        /// IMP_OK_COUNT
        /// < /summary>
        public static string M_IMP_OK_COUNT = "IMP_OK_COUNT";
        private int _IMP_FAIL_COUNT;
        /// < summary>
        /// IMP_FAIL_COUNT
        /// < /summary>
        public static string M_IMP_FAIL_COUNT = "IMP_FAIL_COUNT";
        /// < summary>
        /// SEQ
        /// </summary>
        [AttributeField("SEQ", "System.Int32", false, false, false, "Int32")]
        public int SEQ
        {
            get
            {
                return this.SEQ;
            }
            set
            {
                this._SEQ = value;
            }
        }
        /// < summary>
        /// FILE_TYPE
        /// </summary>
        [AttributeField("FILE_TYPE", "System.String", false, false, false, "String")]
        public string FILE_TYPE
        {
            get
            {
                return this.FILE_TYPE;
            }
            set
            {
                this._FILE_TYPE = value;
            }
        }
        /// < summary>
        /// FileName
        /// </summary>
        [AttributeField("FileName", "System.String", false, false, false, "String")]
        public string FileName
        {
            get
            {
                return this.FileName;
            }
            set
            {
                this._FileName = value;
            }
        }
        /// < summary>
        /// IMP_DATE
        /// </summary>
        [AttributeField("IMP_DATE", "System.String", false, false, false, "String")]
        public string IMP_DATE
        {
            get
            {
                return this.IMP_DATE;
            }
            set
            {
                this._IMP_DATE = value;
            }
        }
        /// < summary>
        /// IMP_TIME
        /// </summary>
        [AttributeField("IMP_TIME", "System.String", false, false, false, "String")]
        public string IMP_TIME
        {
            get
            {
                return this.IMP_TIME;
            }
            set
            {
                this._IMP_TIME = value;
            }
        }
        /// < summary>
        /// Create_User
        /// </summary>
        [AttributeField("Create_User", "System.String", false, false, false, "String")]
        public string Create_User
        {
            get
            {
                return this.Create_User;
            }
            set
            {
                this._Create_User = value;
            }
        }
        /// < summary>
        /// Count
        /// </summary>
        [AttributeField("Count", "System.Int32", false, false, false, "Int32")]
        public int Count
        {
            get
            {
                return this.Count;
            }
            set
            {
                this._Count = value;
            }
        }
        /// < summary>
        /// Status
        /// </summary>
        [AttributeField("Status", "System.String", false, false, false, "String")]
        public string Status
        {
            get
            {
                return this.Status;
            }
            set
            {
                this._Status = value;
            }
        }
        /// < summary>
        /// IMP_OK_COUNT
        /// </summary>
        [AttributeField("IMP_OK_COUNT", "System.Int32", false, false, false, "Int32")]
        public int IMP_OK_COUNT
        {
            get
            {
                return this.IMP_OK_COUNT;
            }
            set
            {
                this._IMP_OK_COUNT = value;
            }
        }
        /// < summary>
        /// IMP_FAIL_COUNT
        /// </summary>
        [AttributeField("IMP_FAIL_COUNT", "System.Int32", false, false, false, "Int32")]
        public int IMP_FAIL_COUNT
        {
            get
            {
                return this.IMP_FAIL_COUNT;
            }
            set
            {
                this._IMP_FAIL_COUNT = value;
            }
        }
    }

    /// <summary>
    /// tbl_FileInfo
    /// </summary>
    [Serializable()]
    public class Entity_AML_IMP_LOGSet : EntitySet<Entity_AML_IMP_LOG>
    {
    }
}
