using Framework.Data.OM;
using System;

namespace CSIPCommonModel.EntityLayer_new
{
    /// <summary>
    /// ���m�W JC99�q��
    /// </summary>
    [Serializable()]
    public class EntityHTG_JC99 : Entity
    {
        #region HTGResult

        private bool _Success;

        public bool Success
        {
            get
            {
                return this._Success;
            }
            set
            {
                this._Success = value;
            }
        }

        private string _HostMsg;

        public string HostMsg
        {
            get
            {
                return this._HostMsg;
            }
            set
            {
                this._HostMsg = value;
            }
        }

        private string _ClientMsg;

        public string ClientMsg
        {
            get
            {
                return this._ClientMsg;
            }
            set
            {
                this._ClientMsg = value;
            }
        }
        #endregion

        private string _USER_ID;

        /// <summary>
        /// �t�ΥN��
        /// </summary>
        public string USER_ID
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


        private string _MESSAGE_TYPE;
        /// <summary>
        /// �T���O:0001(���`), 8888(not found), 9999(not open)
        /// </summary>
        public string MESSAGE_TYPE
        {
            get
            {
                return this._MESSAGE_TYPE;
            }
            set
            {
                this._MESSAGE_TYPE = value;
            }
        }

        private string _FUNCTION_CODE;
        /// <summary>
        /// �\��O
        /// </summary>
        public string FUNCTION_CODE
        {
            get
            {
                return this._FUNCTION_CODE;
            }
            set
            {
                this._FUNCTION_CODE = value;
            }
        }

        private string _IDNO_NO;
        /// <summary>
        /// �Ȥ�ID
        /// </summary>
        public string IDNO_NO
        {
            get
            {
                return this._IDNO_NO;
            }
            set
            {
                this._IDNO_NO = value;
            }
        }

        private string _IN_FILLER;
        public string IN_FILLER
        {
            get
            {
                return this._IN_FILLER;
            }
            set
            {
                this._IN_FILLER = value;
            }
        }

        private string _IN_CFLAG;
        /// <summary>
        /// ���ʫ��d�H�m�W
        /// </summary>
        public string IN_CFLAG
        {
            get
            {
                return this._IN_CFLAG;
            }
            set
            {
                this._IN_CFLAG = value;
            }
        }

        private string _IN_PFLAG;
        /// <summary>
        /// ���ʮa���m�W
        /// </summary>
        public string IN_PFLAG
        {
            get
            {
                return this._IN_PFLAG;
            }
            set
            {
                this._IN_PFLAG = value;
            }
        }

        private string _IN_CHANNEL;
        /// <summary>
        /// �t�ΥN�X
        /// </summary>
        public string IN_CHANNEL
        {
            get
            {
                return this._IN_CHANNEL;
            }
            set
            {
                this._IN_CHANNEL = value;
            }
        }
        private string _RETURN_REC;
        /// <summary>
        /// �U��q��
        /// </summary>
        public string RETURN_REC
        {
            get
            {
                return this._RETURN_REC;
            }
            set
            {
                this._RETURN_REC = value;
            }
        }

        private string _NAME;
        /// <summary>
        /// ���d�H�m�W
        /// </summary>
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

        private string _ROMA;
        /// <summary>
        /// ù������
        /// </summary>
        public string ROMA
        {
            get
            {
                return this._ROMA;
            }
            set
            {
                this._ROMA = value;
            }
        }

        private string _PARENT_NAME;
        /// <summary>
        /// �a���m�W
        /// </summary>
        public string PARENT_NAME
        {
            get
            {
                return this._PARENT_NAME;
            }
            set
            {
                this._PARENT_NAME = value;
            }
        }

        private string _PARENT_ROMA;
        /// <summary>
        /// �a��ù������
        /// </summary>
        public string PARENT_ROMA
        {
            get
            {
                return this._PARENT_ROMA;
            }
            set
            {
                this._PARENT_ROMA = value;
            }
        }
    }
        
}
