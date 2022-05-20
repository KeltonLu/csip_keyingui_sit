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
        /// �����Ҹ��X�βΤ@�Ҹ�
        /// </summary>
        public static string M_CusID = "CusID";

        private string _ReceiveNumber;
        
        /// <summary>
        /// ����s��
        /// </summary>
        public static string M_ReceiveNumber = "ReceiveNumber";

        private string _CusName;

        /// <summary>
        /// �m�W
        /// </summary>
        public static string M_CusName = "CusName";

        private string _AccNoBank;
        
        /// <summary>
        /// �l���N��
        /// </summary>
        public static string M_AccNoBank = "AccNoBank";

        private string _AccNo;

        /// <summary>
        /// �l���b��
        /// </summary>
        public static string M_AccNo = "AccNo";

        private string _AccID;
        
        /// <summary>
        /// �b��ID
        /// </summary>
        public static string M_AccID = "AccID";

        private int _ApplyCode;

        /// <summary>
        /// �ӽХN��(�e�U���c�e��G1.�ӽ� 2.�פ�F�l���^�e�u�b��ܶl����z�פ�v�ɡG3.�l���פ� 4.�~�פ�-�w�^�_���ӽ�)
        /// </summary>
        public static string M_ApplyCode = "ApplyCode";

        private string _AccType;

        /// <summary>
        /// �b��O(P.�sï G.����)
        /// </summary>
        public static string M_AccType = "AccType";

        private string _AccDeposit;

        /// <summary>
        /// �x���b��(�sï�G���b���p14�X�F�����G000000+8�X�b��)
        /// </summary>
        public static string M_AccDeposit = "AccDeposit";

        private string _CusNo;

        /// <summary>
        /// �Τ�s��
        /// </summary>
        public static string M_CusNo = "CusNo";

        private string _AgentID;

        /// <summary>
        /// ���ɤH��
        /// </summary>
        public static string M_AgentID = "AgentID";

        private string _ModDate;

        /// <summary>
        /// ���ɤ��
        /// </summary>
        public static string M_ModDate = "ModDate";

        private bool _IsNeedUpload;

        /// <summary>
        /// �O�_�n�ǰe
        /// </summary>
        public static string M_IsNeedUpload = "IsNeedUpload";

        private string _UploadDate;

        /// <summary>
        /// �ǰe���
        /// </summary>
        public static string M_UploadDate = "UploadDate";

        private string _ReturnStatusTypeCode;

        /// <summary>
        /// ���p�N��
        /// </summary>
        public static string M_ReturnStatusTypeCode = "ReturnStatusTypeCode";

        private string _ReturnCheckFlagCode;

        /// <summary>
        /// �ֹ���O
        /// </summary>
        public static string M_ReturnCheckFlagCode = "ReturnCheckFlagCode";

        private string _ReturnDate;

        /// <summary>
        /// �^�Ф��
        /// </summary>
        public static string M_ReturnDate = "ReturnDate";

        private string _IsSendToPost;

        /// <summary>
        /// �O�_�w�ǰe�l��(0.���ǰe G.�w�ǰe)
        /// </summary>
        public static string M_IsSendToPost = "IsSendToPost";

        private string _SendHostResult;

        /// <summary>
        /// �D���^�ǵ��G
        /// </summary>
        public static string M_SendHostResult = "SendHostResult";

        private string _SendHostResultCode;

        /// <summary>
        /// �D���^�ǵ��G�N�X
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
        /// �����Ҹ��X�βΤ@�Ҹ�
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
        /// ����s��
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
        /// �m�W
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
        /// �l���N��
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
        /// �l���b��
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
        /// �b��ID
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
        /// �ӽХN��(�e�U���c�e��G1.�ӽ� 2.�פ�F�l���^�e�u�b��ܶl����z�פ�v�ɡG3.�l���פ� 4.�~�פ�-�w�^�_���ӽ�)
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
        /// �b��O(P.�sï G.����)
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
        /// �x���b��(�sï�G���b���p14�X�F�����G000000+8�X�b��)
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
        /// �Τ�s��
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
        /// ���ɤH��
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
        /// ���ɤ��
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
        /// �O�_�n�ǰe
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
        /// �ǰe�ɶ�
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
        /// ���p�N��
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
        /// �ֹ���O
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
        /// �^�Ф��
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
        /// �O�_�w�ǰe�l��(0.���ǰe G.�w�ǰe)
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
        /// �D���^�ǵ��G
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
        /// �D���^�ǵ��G�N�X
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
