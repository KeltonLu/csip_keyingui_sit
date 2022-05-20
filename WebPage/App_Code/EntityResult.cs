using Framework.Data.OM;
using System;

namespace CSIPCommonModel.EntityLayer_new
{
    public class EntityResult
    {

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

    }
}
