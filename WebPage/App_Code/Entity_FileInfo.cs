﻿using System;
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
    /// tbl_FileInfo
    /// </summary>
    [Serializable()]
    [AttributeTable("tbl_FileInfo")]
    public class Entity_FileInfo : Framework.Data.OM.Entity
    {
        private int _FileId;

        /// <summary>
        /// FileId
        /// </summary>
        public static string M_FileId = "FileId";

        private string _Job_ID;

        /// <summary>
        /// Job_ID
        /// </summary>
        public static string M_Job_ID = "Job_ID";

        private string _FtpFileName;

        /// <summary>
        /// FtpFileName
        /// </summary>
        public static string M_FtpFileName = "FtpFileName";

        private string _FtpPath;

        /// <summary>
        /// FtpPath
        /// </summary>
        public static string M_FtpPath = "FtpPath";

        private string _ZipPwd;

        /// <summary>
        /// ZipPwd
        /// </summary>
        public static string M_ZipPwd = "ZipPwd";

        private string _FtpIP;

        /// <summary>
        /// FtpIP
        /// </summary>
        public static string M_FtpIP = "FtpIP";

        private string _FtpUserName;

        /// <summary>
        /// FtpUserName
        /// </summary>
        public static string M_FtpUserName = "FtpUserName";

        private string _FtpPwd;

        /// <summary>
        /// FtpPwd
        /// </summary>
        public static string M_FtpPwd = "FtpPwd";

        private string _Status;

        /// <summary>
        /// Status
        /// </summary>
        public static string M_Status = "Status";

        private string _LoopMinutes;

        /// <summary>
        /// ImportDate
        /// </summary>
        public static string M_LoopMinutes = "LoopMinutes";

        private string _Parameter;

        /// <summary>
        /// ImportDate
        /// </summary>
        public static string M_Parameter = "Parameter";

        /// <summary>
        /// FileId
        /// </summary>
        [AttributeField("FileId", "System.Int32", false, true, true, "Int32")]
        public int FileId
        {
            get
            {
                return this._FileId;
            }
            set
            {
                this._FileId = value;
            }
        }

        /// <summary>
        /// Job_ID
        /// </summary>
        [AttributeField("Job_ID", "System.String", false, false, false, "String")]
        public string Job_ID
        {
            get
            {
                return this._Job_ID;
            }
            set
            {
                this._Job_ID = value;
            }
        }

        /// <summary>
        /// FtpFileName
        /// </summary>
        [AttributeField("FtpFileName", "System.String", false, false, false, "String")]
        public string FtpFileName
        {
            get
            {
                return this._FtpFileName;
            }
            set
            {
                this._FtpFileName = value;
            }
        }

        /// <summary>
        /// FtpPath
        /// </summary>
        [AttributeField("FtpPath", "System.String", false, false, false, "String")]
        public string FtpPath
        {
            get
            {
                return this._FtpPath;
            }
            set
            {
                this._FtpPath = value;
            }
        }

        /// <summary>
        /// ZipPwd
        /// </summary>
        [AttributeField("ZipPwd", "System.String", false, false, false, "String")]
        public string ZipPwd
        {
            get
            {
                return this._ZipPwd;
            }
            set
            {
                this._ZipPwd = value;
            }
        }

        /// <summary>
        /// FtpIP
        /// </summary>
        [AttributeField("FtpIP", "System.String", false, false, false, "String")]
        public string FtpIP
        {
            get
            {
                return this._FtpIP;
            }
            set
            {
                this._FtpIP = value;
            }
        }

        /// <summary>
        /// FtpUserName
        /// </summary>
        [AttributeField("FtpUserName", "System.String", false, false, false, "String")]
        public string FtpUserName
        {
            get
            {
                return this._FtpUserName;
            }
            set
            {
                this._FtpUserName = value;
            }
        }

        /// <summary>
        /// FtpPwd
        /// </summary>
        [AttributeField("FtpPwd", "System.String", false, false, false, "String")]
        public string FtpPwd
        {
            get
            {
                return this._FtpPwd;
            }
            set
            {
                this._FtpPwd = value;
            }
        }

        /// <summary>
        /// Status
        /// </summary>
        [AttributeField("Status", "System.String", false, false, false, "String")]
        public string Status
        {
            get
            {
                return this._Status;
            }
            set
            {
                this._Status = value;
            }
        }

        /// <summary>
        /// LoopMinutes
        /// </summary>
        [AttributeField("LoopMinutes", "System.String", false, false, false, "String")]
        public string LoopMinutes
        {
            get
            {
                return this._LoopMinutes;
            }
            set
            {
                this._LoopMinutes = value;
            }
        }

        /// <summary>
        /// Parameter
        /// </summary>
        [AttributeField("Parameter", "System.String", false, false, false, "String")]
        public string Parameter
        {
            get
            {
                return this._Parameter;
            }
            set
            {
                this._Parameter = value;
            }
        }
    }

    /// <summary>
    /// tbl_FileInfo
    /// </summary>
    [Serializable()]
    public class Entity_FileInfoSet : EntitySet<Entity_FileInfo>
    {
    }
}
