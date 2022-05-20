using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;

namespace CSIPCommonModel.EntityLayer_new
{
    /// <summary>
    /// EntityPostOffice_Temp
    /// </summary>
    [Serializable()]
    [AttributeTable("PostOffice_Temp")]
    public class EntityPostOffice_Temp : Entity
    {
        private int _ID;

        /// <summary>
        /// IdentityKey
        /// </summary>
        public static string M_ID = "ID";

        private string _CusID;
        
        /// <summary>
        /// 身分證號碼或統一證號
        /// </summary>
        public static string M_CusID = "CusID";

        private string _ReceiveNumber;
        
        /// <summary>
        /// 收件編號
        /// </summary>
        public static string M_ReceiveNumber = "ReceiveNumber";

        private string _CusName;

        /// <summary>
        /// 姓名
        /// </summary>
        public static string M_CusName = "CusName";

        private string _AccNoBank;
        
        /// <summary>
        /// 郵局代號
        /// </summary>
        public static string M_AccNoBank = "AccNoBank";

        private string _AccNo;

        /// <summary>
        /// 郵局帳號
        /// </summary>
        public static string M_AccNo = "AccNo";

        private string _AccID;
        
        /// <summary>
        /// 帳戶ID
        /// </summary>
        public static string M_AccID = "AccID";

        private int _ApplyCode;

        /// <summary>
        /// 申請代號(委託機構送件：1.申請 2.終止；郵局回送「帳戶至郵局辦理終止」檔：3.郵局終止 4.誤終止-已回復為申請)
        /// </summary>
        public static string M_ApplyCode = "ApplyCode";

        private string _AccType;

        /// <summary>
        /// 帳戶別(P.存簿 G.劃撥)
        /// </summary>
        public static string M_AccType = "AccType";

        private string _AccDeposit;

        /// <summary>
        /// 儲金帳號(存簿：局帳號計14碼；劃撥：000000+8碼帳號)
        /// </summary>
        public static string M_AccDeposit = "AccDeposit";

        private string _CusNo;

        /// <summary>
        /// 用戶編號
        /// </summary>
        public static string M_CusNo = "CusNo";

        private string _AgentID;

        /// <summary>
        /// 鍵檔人員
        /// </summary>
        public static string M_AgentID = "AgentID";

        private string _ModDate;

        /// <summary>
        /// 鍵檔日期
        /// </summary>
        public static string M_ModDate = "ModDate";

        private bool _IsNeedUpload;

        /// <summary>
        /// 是否要傳送
        /// </summary>
        public static string M_IsNeedUpload = "IsNeedUpload";

        private string _UploadDate;

        /// <summary>
        /// 傳送日期
        /// </summary>
        public static string M_UploadDate = "UploadDate";

        private string _ReturnStatusTypeCode;

        /// <summary>
        /// 狀況代號
        /// </summary>
        public static string M_ReturnStatusTypeCode = "ReturnStatusTypeCode";

        private string _ReturnCheckFlagCode;

        /// <summary>
        /// 核對註記
        /// </summary>
        public static string M_ReturnCheckFlagCode = "ReturnCheckFlagCode";

        private string _ReturnDate;

        /// <summary>
        /// 回覆日期
        /// </summary>
        public static string M_ReturnDate = "ReturnDate";

        private string _IsSendToPost;

        /// <summary>
        /// 是否已傳送郵局(0.未傳送 G.已傳送)
        /// </summary>
        public static string M_IsSendToPost = "IsSendToPost";

        private string _SendHostResult;

        /// <summary>
        /// 主機回傳結果
        /// </summary>
        public static string M_SendHostResult = "SendHostResult";

        private string _SendHostResultCode;

        /// <summary>
        /// 主機回傳結果代碼
        /// </summary>
        public static string M_SendHostResultCode = "SendHostResultCode";

        /// <summary>
        /// IdentityKey
        /// </summary>
        [AttributeField("ID", "System.Int32", false, true, true, "Int32")]
        public int ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
            }
        }

        /// <summary>
        /// 身分證號碼或統一證號
        /// </summary>
        [AttributeField("CusID", "System.String", false, false, false, "String")]
        public string CusID
        {
            get
            {
                return this._CusID;
            }
            set
            {
                this._CusID = value;
            }
        }

        /// <summary>
        /// 收件編號
        /// </summary>
        [AttributeField("ReceiveNumber", "System.String", false, true, false, "String")]
        public string ReceiveNumber
        {
            get
            {
                return this._ReceiveNumber;
            }
            set
            {
                this._ReceiveNumber = value;
            }
        }

        /// <summary>
        /// 姓名
        /// </summary>
        [AttributeField("CusName", "System.String", false, true, false, "String")]
        public string CusName
        {
            get
            {
                return this._CusName;
            }
            set
            {
                this._CusName = value;
            }
        }

        /// <summary>
        /// 郵局代號
        /// </summary>
        [AttributeField("AccNoBank", "System.String", false, false, false, "String")]
        public string AccNoBank
        {
            get
            {
                return this._AccNoBank;
            }
            set
            {
                this._AccNoBank = value;
            }
        }

        /// <summary>
        /// 郵局帳號
        /// </summary>
        [AttributeField("AccNo", "System.String", false, false, false, "String")]
        public string AccNo
        {
            get
            {
                return this._AccNo;
            }
            set
            {
                this._AccNo = value;
            }
        }

        /// <summary>
        /// 帳戶ID
        /// </summary>
        [AttributeField("AccID", "System.String", false, false, false, "String")]
        public string AccID
        {
            get
            {
                return this._AccID;
            }
            set
            {
                this._AccID = value;
            }
        }

        /// <summary>
        /// 申請代號(委託機構送件：1.申請 2.終止；郵局回送「帳戶至郵局辦理終止」檔：3.郵局終止 4.誤終止-已回復為申請)
        /// </summary>
        [AttributeField("ApplyCode", "System.Int32", false, false, false, "Int32")]
        public int ApplyCode
        {
            get
            {
                return this._ApplyCode;
            }
            set
            {
                this._ApplyCode = value;
            }
        }

        /// <summary>
        /// 帳戶別(P.存簿 G.劃撥)
        /// </summary>
        [AttributeField("AccType", "System.String", false, false, false, "String")]
        public string AccType
        {
            get
            {
                return this._AccType;
            }
            set
            {
                this._AccType = value;
            }
        }

        /// <summary>
        /// 儲金帳號(存簿：局帳號計14碼；劃撥：000000+8碼帳號)
        /// </summary>
        [AttributeField("AccDeposit", "System.String", false, false, false, "String")]
        public string AccDeposit
        {
            get
            {
                return this._AccDeposit;
            }
            set
            {
                this._AccDeposit = value;
            }
        }

        /// <summary>
        /// 用戶編號
        /// </summary>
        [AttributeField("CusNo", "System.String", false, false, false, "String")]
        public string CusNo
        {
            get
            {
                return this._CusNo;
            }
            set
            {
                this._CusNo = value;
            }
        }

        /// <summary>
        /// 鍵檔人員
        /// </summary>
        [AttributeField("AgentID", "System.String", false, false, false, "String")]
        public string AgentID
        {
            get
            {
                return this._AgentID;
            }
            set
            {
                this._AgentID = value;
            }
        }

        /// <summary>
        /// 鍵檔日期
        /// </summary>
        [AttributeField("ModDate", "System.String", false, false, false, "String")]
        public string ModDate
        {
            get
            {
                return this._ModDate;
            }
            set
            {
                this._ModDate = value;
            }
        }

        /// <summary>
        /// 是否要傳送
        /// </summary>
        [AttributeField("IsNeedUpload", "System.Boolean", false, false, false, "Boolean")]
        public bool IsNeedUpload
        {
            get
            {
                return this._IsNeedUpload;
            }
            set
            {
                this._IsNeedUpload = value;
            }
        }

        /// <summary>
        /// 傳送時間
        /// </summary>
        [AttributeField("UploadDate", "System.String", false, false, false, "String")]
        public string UploadDate
        {
            get
            {
                return this._UploadDate;
            }
            set
            {
                this._UploadDate = value;
            }
        }

        /// <summary>
        /// 狀況代號
        /// </summary>
        [AttributeField("ReturnStatusTypeCode", "System.String", false, false, false, "String")]
        public string ReturnStatusTypeCode
        {
            get
            {
                return this._ReturnStatusTypeCode;
            }
            set
            {
                this._ReturnStatusTypeCode = value;
            }
        }

        /// <summary>
        /// 核對註記
        /// </summary>
        [AttributeField("ReturnCheckFlagCode", "System.String", false, false, false, "String")]
        public string ReturnCheckFlagCode
        {
            get
            {
                return this._ReturnCheckFlagCode;
            }
            set
            {
                this._ReturnCheckFlagCode = value;
            }
        }

        /// <summary>
        /// 回覆日期
        /// </summary>
        [AttributeField("ReturnDate", "System.String", false, false, false, "String")]
        public string ReturnDate
        {
            get
            {
                return this._ReturnDate;
            }
            set
            {
                this._ReturnDate = value;
            }
        }

        /// <summary>
        /// 是否已傳送郵局(0.未傳送 G.已傳送)
        /// </summary>
        [AttributeField("IsSendToPost", "System.String", false, false, false, "String")]
        public string IsSendToPost
        {
            get
            {
                return this._IsSendToPost;
            }
            set
            {
                this._IsSendToPost = value;
            }
        }

        /// <summary>
        /// 主機回傳結果
        /// </summary>
        [AttributeField("SendHostResult", "System.String", false, false, false, "String")]
        public string SendHostResult
        {
            get
            {
                return this._SendHostResult;
            }
            set
            {
                this._SendHostResult = value;
            }
        }

        /// <summary>
        /// 主機回傳結果代碼
        /// </summary>
        [AttributeField("SendHostResultCode", "System.String", false, false, false, "String")]
        public string SendHostResultCode
        {
            get
            {
                return this._SendHostResultCode;
            }
            set
            {
                this._SendHostResultCode = value;
            }
        }
    }
    
    /// <summary>
    /// EntityPostOffice_Temp
    /// </summary>
    [Serializable()]
    public class EntityPostOffice_TempSet : EntitySet<EntityPostOffice_Temp>
    {
    }
}
