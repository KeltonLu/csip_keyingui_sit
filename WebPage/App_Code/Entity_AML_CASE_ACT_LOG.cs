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
    [AttributeTable("AML_CASE_ACT_LOG")]
    public class Entity_AML_CASE_ACT_LOG : Framework.Data.OM.Entity
    {
        private string _CASE_NO;
        /// < summary>
        /// CASE_NO
        /// < /summary>
        public static string M_CASE_NO = "CASE_NO";
        private string _ACT_Date;
        /// < summary>
        /// ACT_Date
        /// < /summary>
        public static string M_ACT_Date = "ACT_Date";
        private string _ACT_Time;
        /// < summary>
        /// ACT_Time
        /// < /summary>
        public static string M_ACT_Time = "ACT_Time";
        private string _ACT_Type;
        /// < summary>
        /// ACT_Type
        /// < /summary>
        public static string M_ACT_Type = "ACT_Type";
        private string _ACT_UserID;
        /// < summary>
        /// ACT_UserID
        /// < /summary>
        public static string M_ACT_UserID = "ACT_UserID";
        private string _ACT_Content;
        /// < summary>
        /// ACT_Content
        /// < /summary>
        public static string M_ACT_Content = "ACT_Content";
        /// < summary>
        /// CASE_NO
        /// </summary>
        [AttributeField("CASE_NO", "System.String", false, false, false, "String")]
        public string CASE_NO
        {
            get
            {
                return this.CASE_NO;
            }
            set
            {
                this._CASE_NO = value;
            }
        }
        /// < summary>
        /// ACT_Date
        /// </summary>
        [AttributeField("ACT_Date", "System.String", false, false, false, "String")]
        public string ACT_Date
        {
            get
            {
                return this.ACT_Date;
            }
            set
            {
                this._ACT_Date = value;
            }
        }
        /// < summary>
        /// ACT_Time
        /// </summary>
        [AttributeField("ACT_Time", "System.String", false, false, false, "String")]
        public string ACT_Time
        {
            get
            {
                return this.ACT_Time;
            }
            set
            {
                this._ACT_Time = value;
            }
        }
        /// < summary>
        /// ACT_Type
        /// </summary>
        [AttributeField("ACT_Type", "System.String", false, false, false, "String")]
        public string ACT_Type
        {
            get
            {
                return this.ACT_Type;
            }
            set
            {
                this._ACT_Type = value;
            }
        }
        /// < summary>
        /// ACT_UserID
        /// </summary>
        [AttributeField("ACT_UserID", "System.String", false, false, false, "String")]
        public string ACT_UserID
        {
            get
            {
                return this.ACT_UserID;
            }
            set
            {
                this._ACT_UserID = value;
            }
        }
        /// < summary>
        /// ACT_Content
        /// </summary>
        [AttributeField("ACT_Content", "System.String", false, false, false, "String")]
        public string ACT_Content
        {
            get
            {
                return this.ACT_Content;
            }
            set
            {
                this._ACT_Content = value;
            }
        }
    }

    /// <summary>
    /// tbl_FileInfo
    /// </summary>
    [Serializable()]
    public class Entity_AML_CASE_ACT_LOGSet : EntitySet<Entity_AML_CASE_ACT_LOG>
    {
    }
}
