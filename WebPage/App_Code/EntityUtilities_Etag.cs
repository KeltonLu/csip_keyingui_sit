using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using System.Data;

namespace CSIPCommonModel.EntityLayer
{
    /// <summary>
    /// EntityUtilities_Etag
    /// </summary>
    [Serializable()]
    [AttributeTable("Utilities_Etag")]
    public class EntityUtilities_Etag : Entity
    {
        private string _RECEIVE_NUMBER;

        /// <summary>
        /// 收件編號
        /// </summary>
        public static string M_RECEIVE_NUMBER = "RECEIVE_NUMBER";

        private string _PRIMARY_CARD_ID;

        /// <summary>
        /// 正卡人ID
        /// </summary>
        public static string M_PRIMARY_CARD_ID = "PRIMARY_CARD_ID";

        private string _NAME;

        /// <summary>
        /// 姓名
        /// </summary>
        public static string M_NAME = "NAME";

        private bool _ETAG_FLAG;

        /// <summary>
        /// ETAG儲值
        /// </summary>
        public static string M_ETAG_FLAG = "ETAG_FLAG";

        private bool _TEMPLATE_PARK_FLAG;

        /// <summary>
        /// 臨時停車
        /// </summary>
        public static string M_TEMPLATE_PARK_FLAG = "TEMPLATE_PARK_FLAG";

        private bool _MONTHLY_PARK_FLAG;

        /// <summary>
        /// 月租停車
        /// </summary>
        public static string M_MONTHLY_PARK_FLAG = "MONTHLY_PARK_FLAG";

        private string _MOBILE_PHONE;

        /// <summary>
        /// 行動電話
        /// </summary>
        public static string M_MOBILE_PHONE = "MOBILE_PHONE";

        private string _APPLY_TYPE;

        /// <summary>
        /// 申請別(1.申請 2.終止)
        /// </summary>
        public static string M_APPLY_TYPE = "APPLY_TYPE";

        private string _PLATE_NO1;

        /// <summary>
        /// 車牌號碼
        /// </summary>
        public static string M_PLATE_NO1 = "PLATE_NO1";

        private string _OWNERS_ID;

        /// <summary>
        /// 車主ID或統編
        /// </summary>
        public static string M_OWNERS_ID = "OWNERS_ID";

        private string _MONTHLY_PARKING_FEE;

        /// <summary>
        /// 月租停車費代繳
        /// </summary>
        public static string M_MONTHLY_PARKING_FEE = "MONTHLY_PARKING_FEE";

        private string _POPUL_NO;

        /// <summary>
        /// 推廣代號
        /// </summary>
        public static string M_POPUL_NO = "POPUL_NO";

        private string _POPUL_EMP_NO;

        /// <summary>
        /// 推廣員編
        /// </summary>
        public static string M_POPUL_EMP_NO = "POPUL_EMP_NO";

        private string _INTRODUCER_CARD_ID;

        /// <summary>
        /// 種子
        /// </summary>
        public static string M_INTRODUCER_CARD_ID = "INTRODUCER_CARD_ID";

        private string _AGENT_ID;

        /// <summary>
        /// 鍵檔員編
        /// </summary>
        public static string M_AGENT_ID = "AGENT_ID";

        private string _KEYIN_DATE;

        /// <summary>
        /// 鍵檔日
        /// </summary>
        public static string M_KEYIN_DATE = "KEYIN_DATE";

        private string _KEYIN_FLAG;

        /// <summary>
        /// 一二KEY
        /// </summary>
        public static string M_KEYIN_FLAG = "KEYIN_FLAG";

        private string _PLATE_NO2;

        /// <summary>
        /// 車牌號碼2
        /// </summary>
        public static string M_PLATE_NO2 = "PLATE_NO2";

        private string _PLATE_NO3;

        /// <summary>
        /// 車牌號碼3
        /// </summary>
        public static string M_PLATE_NO3 = "PLATE_NO3";

        private string _PLATE_NO4;

        /// <summary>
        /// 車牌號碼4
        /// </summary>
        public static string M_PLATE_NO4 = "PLATE_NO4";

        /// <summary>
        /// 收件編號
        /// </summary>
        [AttributeField("RECEIVE_NUMBER", "System.String", false, true, false, "String")]
        public string RECEIVE_NUMBER
        {
            get
            {
                return this._RECEIVE_NUMBER;
            }
            set
            {
                this._RECEIVE_NUMBER = value;
            }
        }

        /// <summary>
        /// 正卡人ID
        /// </summary>
        [AttributeField("PRIMARY_CARD_ID", "System.String", false, false, false, "String")]
        public string PRIMARY_CARD_ID
        {
            get
            {
                return this._PRIMARY_CARD_ID;
            }
            set
            {
                this._PRIMARY_CARD_ID = value;
            }
        }

        /// <summary>
        /// 姓名
        /// </summary>
        [AttributeField("NAME", "System.String", false, false, false, "String")]
        public string NAME
        {
            get
            {
                return this._NAME;
            }
            set
            {
                this._NAME = value;
            }
        }

        /// <summary>
        /// ETAG儲值
        /// </summary>
        [AttributeField("ETAG_FLAG", "System.Boolean", false, false, false, "Boolean")]
        public bool ETAG_FLAG
        {
            get
            {
                return this._ETAG_FLAG;
            }
            set
            {
                this._ETAG_FLAG = value;
            }
        }

        /// <summary>
        /// 臨時停車
        /// </summary>
        [AttributeField("TEMPLATE_PARK_FLAG", "System.Boolean", false, false, false, "Boolean")]
        public bool TEMPLATE_PARK_FLAG
        {
            get
            {
                return this._TEMPLATE_PARK_FLAG;
            }
            set
            {
                this._TEMPLATE_PARK_FLAG = value;
            }
        }

        /// <summary>
        /// 月租停車
        /// </summary>
        [AttributeField("MONTHLY_PARK_FLAG", "System.Boolean", false, false, false, "Boolean")]
        public bool MONTHLY_PARK_FLAG
        {
            get
            {
                return this._MONTHLY_PARK_FLAG;
            }
            set
            {
                this._MONTHLY_PARK_FLAG = value;
            }
        }

        /// <summary>
        /// 行動電話
        /// </summary>
        [AttributeField("MOBILE_PHONE", "System.String", false, false, false, "String")]
        public string MOBILE_PHONE
        {
            get
            {
                return this._MOBILE_PHONE;
            }
            set
            {
                this._MOBILE_PHONE = value;
            }
        }

        /// <summary>
        /// 申請別(1.申請 2.終止)
        /// </summary>
        [AttributeField("APPLY_TYPE", "System.String", false, false, false, "String")]
        public string APPLY_TYPE
        {
            get
            {
                return this._APPLY_TYPE;
            }
            set
            {
                this._APPLY_TYPE = value;
            }
        }

        /// <summary>
        /// 車牌號碼
        /// </summary>
        [AttributeField("PLATE_NO1", "System.String", false, false, false, "String")]
        public string PLATE_NO1
        {
            get
            {
                return this._PLATE_NO1;
            }
            set
            {
                this._PLATE_NO1 = value;
            }
        }

        /// <summary>
        /// 車主ID或統編
        /// </summary>
        [AttributeField("OWNERS_ID", "System.String", false, false, false, "String")]
        public string OWNERS_ID
        {
            get
            {
                return this._OWNERS_ID;
            }
            set
            {
                this._OWNERS_ID = value;
            }
        }

        /// <summary>
        /// 月租停車費代繳
        /// </summary>
        [AttributeField("MONTHLY_PARKING_FEE", "System.String", false, false, false, "String")]
        public string MONTHLY_PARKING_FEE
        {
            get
            {
                return this._MONTHLY_PARKING_FEE;
            }
            set
            {
                this._MONTHLY_PARKING_FEE = value;
            }
        }

        /// <summary>
        /// 推廣代號
        /// </summary>
        [AttributeField("POPUL_NO", "System.String", false, false, false, "String")]
        public string POPUL_NO
        {
            get
            {
                return this._POPUL_NO;
            }
            set
            {
                this._POPUL_NO = value;
            }
        }

        /// <summary>
        /// 推廣員編
        /// </summary>
        [AttributeField("POPUL_EMP_NO", "System.String", false, false, false, "String")]
        public string POPUL_EMP_NO
        {
            get
            {
                return this._POPUL_EMP_NO;
            }
            set
            {
                this._POPUL_EMP_NO = value;
            }
        }

        /// <summary>
        /// 種子
        /// </summary>
        [AttributeField("INTRODUCER_CARD_ID", "System.String", false, false, false, "String")]
        public string INTRODUCER_CARD_ID
        {
            get
            {
                return this._INTRODUCER_CARD_ID;
            }
            set
            {
                this._INTRODUCER_CARD_ID = value;
            }
        }

        /// <summary>
        /// 鍵檔員編
        /// </summary>
        [AttributeField("AGENT_ID", "System.String", false, false, false, "String")]
        public string AGENT_ID
        {
            get
            {
                return this._AGENT_ID;
            }
            set
            {
                this._AGENT_ID = value;
            }
        }

        /// <summary>
        /// 鍵檔日
        /// </summary>
        [AttributeField("KEYIN_DATE", "System.String", false, false, false, "String")]
        public string KEYIN_DATE
        {
            get
            {
                return this._KEYIN_DATE;
            }
            set
            {
                this._KEYIN_DATE = value;
            }
        }

        /// <summary>
        /// 一二KEY
        /// </summary>
        [AttributeField("KEYIN_FLAG", "System.String", false, false, false, "String")]
        public string KEYIN_FLAG
        {
            get
            {
                return this._KEYIN_FLAG;
            }
            set
            {
                this._KEYIN_FLAG = value;
            }
        }

        /// <summary>
        /// 車牌號碼2
        /// </summary>
        [AttributeField("PLATE_NO2", "System.String", false, false, false, "String")]
        public string PLATE_NO2
        {
            get
            {
                return this._PLATE_NO2;
            }
            set
            {
                this._PLATE_NO2 = value;
            }
        }

        /// <summary>
        /// 車牌號碼3
        /// </summary>
        [AttributeField("PLATE_NO3", "System.String", false, false, false, "String")]
        public string PLATE_NO3
        {
            get
            {
                return this._PLATE_NO3;
            }
            set
            {
                this._PLATE_NO3 = value;
            }
        }

        /// <summary>
        /// 車牌號碼4
        /// </summary>
        [AttributeField("PLATE_NO4", "System.String", false, false, false, "String")]
        public string PLATE_NO4
        {
            get
            {
                return this._PLATE_NO4;
            }
            set
            {
                this._PLATE_NO4 = value;
            }
        }
    }

    /// <summary>
    /// EntityUtilities_Etag
    /// </summary>
    [Serializable()]
    public class EntityUtilities_EtagSet : EntitySet<EntityUtilities_Etag>
    {
    }
}
