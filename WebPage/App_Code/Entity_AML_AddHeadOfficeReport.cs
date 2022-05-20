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
    [AttributeTable("AML_AddHeadOfficeReport")]
    public class Entity_AML_AddHeadOfficeReport : Framework.Data.OM.Entity
    {
        private int _ID;
        /// < summary>
        /// ID
        /// < /summary>
        public static string M_ID = "ID";
        private string _TaxID;
        /// < summary>
        /// TaxID
        /// < /summary>
        public static string M_TaxID = "TaxID";
        private string _Branch_TaxID;
        /// < summary>
        /// Branch_TaxID
        /// < /summary>
        public static string M_Branch_TaxID = "Branch_TaxID";
        private string _Branch_No;
        /// < summary>
        /// Branch_No
        /// < /summary>
        public static string M_Branch_No = "Branch_No";
        private string _Recv_no;
        /// < summary>
        /// Recv_no
        /// < /summary>
        public static string M_Recv_no = "Recv_no";
        private string _Create_day;
        /// < summary>
        /// Create_day
        /// < /summary>
        public static string M_Create_day = "Create_day";
        private string _OfficeAdd_day;
        /// < summary>
        /// OfficeAdd_day
        /// < /summary>
        public static string M_OfficeAdd_day = "OfficeAdd_day";
        
        /// < summary>
        /// ID
        /// </summary>
        [AttributeField("ID", "System.Int32", false, false, false, "Int32")]
        public int ID
        {
            get
            {
                return this.ID;
            }
            set
            {
                this._ID = value;
            }
        }
        /// < summary>
        /// TaxID
        /// </summary>
        [AttributeField("TaxID", "System.String", false, false, false, "String")]
        public string TaxID
        {
            get
            {
                return this.TaxID;
            }
            set
            {
                this._TaxID = value;
            }
        }
        /// < summary>
        /// Branch_TaxID
        /// </summary>
        [AttributeField("Branch_TaxID", "System.String", false, false, false, "String")]
        public string Branch_TaxID
        {
            get
            {
                return this.Branch_TaxID;
            }
            set
            {
                this._Branch_TaxID = value;
            }
        }
        /// < summary>
        /// Branch_No
        /// </summary>
        [AttributeField("Branch_No", "System.String", false, false, false, "String")]
        public string Branch_No
        {
            get
            {
                return this.Branch_No;
            }
            set
            {
                this._Branch_No = value;
            }
        }
        /// < summary>
        /// Recv_no
        /// </summary>
        [AttributeField("Recv_no", "System.String", false, false, false, "String")]
        public string Recv_no
        {
            get
            {
                return this.Recv_no;
            }
            set
            {
                this._Recv_no = value;
            }
        }
        /// < summary>
        /// Create_day
        /// </summary>
        [AttributeField("Create_day", "System.String", false, false, false, "String")]
        public string Create_day
        {
            get
            {
                return this.Create_day;
            }
            set
            {
                this._Create_day = value;
            }
        }
        /// < summary>
        /// OfficeAdd_day
        /// </summary>
        [AttributeField("OfficeAdd_day", "System.String", false, false, false, "String")]
        public string OfficeAdd_day
        {
            get
            {
                return this.OfficeAdd_day;
            }
            set
            {
                this._OfficeAdd_day = value;
            }
        }
    }

    /// <summary>
    /// tbl_FileInfo
    /// </summary>
    [Serializable()]
    public class Entity_AML_AddHeadOfficeReportSet : EntitySet<Entity_AML_AddHeadOfficeReport>
    {
    }
}