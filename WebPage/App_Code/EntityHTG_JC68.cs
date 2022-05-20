using Framework.Data.OM;
using System;

namespace CSIPCommonModel.EntityLayer_new
{
    /// <summary>
    /// JC68 ���m�W �q��
    /// </summary>
    [Serializable()]
    public class EntityHTG_JC68 : Entity
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

        private string _LineID;
        /// <summary>
        /// �������LIND ID
        /// </summary>
        public string LineID
        {
            get
            {
                return this._LineID;
            }
            set
            {
                this._LineID = value;
            }
        }


        private string _ID;
        /// <summary>
        /// ���m�WID
        /// </summary>
        public string ID
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

        private string _LONG_NAME;
        /// <summary>
        /// ���m�W
        /// </summary>
        public string LONG_NAME
        {
            get
            {
                return this._LONG_NAME;
            }
            set
            {
                this._LONG_NAME = value;
            }
        }

        private string _PINYIN_NAME;
        /// <summary>
        /// ù������
        /// </summary>
        public string PINYIN_NAME
        {
            get
            {
                return this._PINYIN_NAME;
            }
            set
            {
                this._PINYIN_NAME = value;
            }
        }
    }
}
