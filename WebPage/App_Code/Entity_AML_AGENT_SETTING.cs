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
    [AttributeTable("AML_AGENT_SETTING")]
    public class Entity_AML_AGENT_SETTING : Framework.Data.OM.Entity
    {
        private String _USER_ID;
        /// < summary>
        /// USER_ID
        /// < /summary>
        public static string M_USER_ID = "USER_ID";
        private string _USER_NAME;
        /// < summary>
        /// USER_NAME
        /// < /summary>
        public static string M_USER_NAME = "USER_NAME";
        private int _ASSIGN_RATE;
        /// < summary>
        /// ASSIGN_RATE
        /// < /summary>
        public static string M_ASSIGN_RATE = "ASSIGN_RATE";
        private string _MEMO;
        /// < summary>
        /// MEMO
        /// < /summary>
        public static string M_MEMO = "MEMO";
        private string _STOP_ASSIGN;
        /// < summary>
        /// STOP_ASSIGN
        /// < /summary>
        public static string M_STOP_ASSIGN = "STOP_ASSIGN";
        private string _USER_STATUS;
        /// < summary>
        /// USER_STATUS
        /// < /summary>
        public static string M_USER_STATUS = "USER_STATUS";
        private string _ADD_USER_ID;
        /// < summary>
        /// ADD_USER_ID
        /// < /summary>
        public static string M_ADD_USER_ID = "ADD_USER_ID";
        private string _ADD_USER_NAME;
        /// < summary>
        /// ADD_USER_NAME
        /// < /summary>
        public static string M_ADD_USER_NAME = "ADD_USER_NAME";
        private DateTime _ADD_DATE;
        /// < summary>
        /// ADD_DATE
        /// < /summary>
        public static string M_ADD_DATE = "ADD_DATE";
        private string _MODI_USER_ID;
        /// < summary>
        /// MODI_USER_ID
        /// < /summary>
        public static string M_MODI_USER_ID = "MODI_USER_ID";
        private string _MODI_USER_NAME;
        /// < summary>
        /// MODI_USER_NAME
        /// < /summary>
        public static string M_MODI_USER_NAME = "MODI_USER_NAME";
        private DateTime _UPDATE_DATE;
        /// < summary>
        /// UPDATE_DATE
        /// < /summary>
        public static string M_UPDATE_DATE = "UPDATE_DATE";
        private string _USER_TYPE;
        /// < summary>
        /// USER_TYPE
        /// < /summary>
        public static string M_USER_TYPE = "USER_TYPE";
        private string _USER_EXTENSION;
        /// < summary>
        /// USER_EXTENSION
        /// < /summary>
        public static string M_USER_EXTENSION = "USER_EXTENSION";
        private string _USER_TITLE;
        /// < summary>
        /// USER_TITLE
        /// < /summary>
        public static string M_USER_TITLE = "USER_TITLE";
        private string _ASSIGN_RATE_MERCHANT;
        /// < summary>
        /// ASSIGN_RATE_MERCHANT
        /// < /summary>
        public static string M_ASSIGN_RATE_MERCHANT = "ASSIGN_RATE_MERCHANT";

        /// < summary>
        /// 經辦帳號
        /// </summary>
        [AttributeField("USER_ID", "System.Int32", false, false, false, "String")]
        public String USER_ID
        {
            get
            {
                return this._USER_ID;
            }
            set
            {
                this._USER_ID = value;
            }
        }
        /// < summary>
        /// 經辦名稱
        /// </summary>
        [AttributeField("USER_NAME", "System.String", false, false, false, "String")]
        public string USER_NAME
        {
            get
            {
                return this._USER_NAME;
            }
            set
            {
                this._USER_NAME = value;
            }
        }
        /// < summary>
        /// 派案比例
        /// </summary>
        [AttributeField("ASSIGN_RATE", "System.Int32", false, false, false, "Int32")]
        public int ASSIGN_RATE
        {
            get
            {
                return this._ASSIGN_RATE;
            }
            set
            {
                this._ASSIGN_RATE = value;
            }
        }
        /// < summary>
        /// 備註說明
        /// </summary>
        [AttributeField("MEMO", "System.String", false, false, false, "String")]
        public string MEMO
        {
            get
            {
                return this._MEMO;
            }
            set
            {
                this._MEMO = value;
            }
        }
        /// < summary>
        /// 停止派案    1:停止 0:不停止
        /// </summary>
        [AttributeField("STOP_ASSIGN", "System.String", false, false, false, "String")]
        public string STOP_ASSIGN
        {
            get
            {
                return this._STOP_ASSIGN;
            }
            set
            {
                this._STOP_ASSIGN = value;
            }
        }
        /// < summary>
        /// 使用者狀態	1:正常 0:失效
        /// </summary>
        [AttributeField("USER_STATUS", "System.String", false, false, false, "String")]
        public string USER_STATUS
        {
            get
            {
                return this._USER_STATUS;
            }
            set
            {
                this._USER_STATUS = value;
            }
        }
        /// < summary>
        /// 新增資料的人員帳號
        /// </summary>
        [AttributeField("ADD_USER_ID", "System.String", false, false, false, "String")]
        public string ADD_USER_ID
        {
            get
            {
                return this._ADD_USER_ID;
            }
            set
            {
                this._ADD_USER_ID = value;
            }
        }
        /// < summary>
        /// 新增資料的人員名稱
        /// </summary>
        [AttributeField("ADD_USER_NAME", "System.String", false, false, false, "String")]
        public string ADD_USER_NAME
        {
            get
            {
                return this._ADD_USER_NAME;
            }
            set
            {
                this._ADD_USER_NAME = value;
            }
        }
        /// < summary>
        /// 新增資料的時間
        /// </summary>
        [AttributeField("ADD_DATE", "System.DateTime", false, false, false, "DateTime")]
        public DateTime ADD_DATE
        {
            get
            {
                return this._ADD_DATE;
            }
            set
            {
                this._ADD_DATE = value;
            }
        }
        /// < summary>
        /// 修改資料的人員帳號
        /// </summary>
        [AttributeField("MODI_USER_ID", "System.String", false, false, false, "String")]
        public string MODI_USER_ID
        {
            get
            {
                return this._MODI_USER_ID;
            }
            set
            {
                this._MODI_USER_ID = value;
            }
        }
        /// < summary>
        /// 修改資料的人員名稱
        /// </summary>
        [AttributeField("MODI_USER_NAME", "System.String", false, false, false, "String")]
        public string MODI_USER_NAME
        {
            get
            {
                return this._MODI_USER_NAME;
            }
            set
            {
                this._MODI_USER_NAME = value;
            }
        }
        /// < summary>
        /// 修改資料的時間
        /// </summary>
        [AttributeField("UPDATE_DATE", "System.DateTime", false, false, false, "DateTime")]
        public DateTime UPDATE_DATE
        {
            get
            {
                return this._UPDATE_DATE;
            }
            set
            {
                this._UPDATE_DATE = value;
            }
        }
        /// < summary>
        /// 未定義 預設為0
        /// </summary>
        [AttributeField("USER_TYPE", "System.String", false, false, false, "String")]
        public string USER_TYPE
        {
            get
            {
                return this._USER_TYPE;
            }
            set
            {
                this._USER_TYPE = value;
            }
        }
        /// < summary>
        /// 經辦分機
        /// </summary>
        [AttributeField("USER_EXTENSION", "System.String", false, false, false, "String")]
        public string USER_EXTENSION
        {
            get
            {
                return this._USER_EXTENSION;
            }
            set
            {
                this._USER_EXTENSION = value;
            }
        }
        /// < summary>
        /// 經辦稱謂
        /// </summary>
        [AttributeField("USER_TITLE", "System.String", false, false, false, "String")]
        public string USER_TITLE
        {
            get
            {
                return this._USER_TITLE;
            }
            set
            {
                this._USER_TITLE = value;
            }
        }
        /// < summary>
        /// 派案比例(備用，暫不使用)
        /// </summary>
        [AttributeField("ASSIGN_RATE_MERCHANT", "System.String", false, false, false, "String")]
        public string ASSIGN_RATE_MERCHANT
        {
            get
            {
                return this._ASSIGN_RATE_MERCHANT;
            }
            set
            {
                this._ASSIGN_RATE_MERCHANT = value;
            }
        }

        public Entity_AML_AGENT_SETTING()
        {
            _ASSIGN_RATE = 0;
            _UPDATE_DATE = DateTime.Now;
            _ADD_DATE = DateTime.MinValue;
        }
    }

    /// <summary>
    /// tbl_FileInfo
    /// </summary>
    [Serializable()]
    public class Entity_AML_AGENT_SETTINGSet : EntitySet<Entity_AML_AGENT_SETTING>
    {
    }
}
