using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace Framework.Data.OM.OMAttribute
{
    /// <summary>
    /// Mapping Field Attribute
    /// </summary>     
    [System.Serializable]
    public class AttributeValidPage : Attribute
    {
        #region Private Field

        private bool _CanBeEmpty; //不可為空白  true 要檢核 False 不檢核
        /// <summary>
        /// 可以為空白
        /// </summary>
        public bool CanBeEmpty
        {
            set { _CanBeEmpty = value; }
            get { return _CanBeEmpty; }
        }

        /// <summary>
        /// 是否檢查長度 true 要檢核 False 不檢核
        /// </summary>
        private bool _isCheckLen;
        /// <summary>
        /// 是否檢查長度
        /// </summary>
        public bool isCheckLen
        {
            get { return _isCheckLen; }
            set { _isCheckLen = value; }
        }

        /// <summary>
        /// 長度限制
        /// </summary>
        private int _cLength;
        /// <summary>
        /// 長度限制
        /// </summary>
        public int cLength
        {
            get { return _cLength; }
            set { _cLength = value; }
        }

        /// <summary>
        /// 是否檢查字典 true 要檢核 False 不檢核
        /// </summary>
        private bool _isCheckDC;
        /// <summary>
        ///是否檢查字典
        /// </summary>
        public bool isCheckDC
        {
            get { return _isCheckDC; }
            set { _isCheckDC = value; }
        }

        /// <summary>
        /// 字典的字首，組合Value後查詢字典Key是否存在
        /// </summary>
        private string _sDC_Head;
        /// <summary>
        /// 長度限制
        /// </summary>
        public string sDC_Head
        {
            get { return _sDC_Head; }
            set { _sDC_Head = value; }
        }


        /// <summary>
        /// 驗證失敗要提示的訊息
        /// </summary>
        private string _sShowMsgEmpty;
        /// <summary>
        /// 驗證失敗要提示的訊息
        /// </summary>
        public string sShowMsgEmpty
        {
            get { return _sShowMsgEmpty; }
            set { _sShowMsgEmpty = value; }
        }
        /// <summary>
        /// 驗證失敗要提示的訊息
        /// </summary>
        private string _sShowMsgLen;
        /// <summary>
        /// 驗證失敗要提示的訊息
        /// </summary>
        public string sShowMsgLen
        {
            get { return _sShowMsgLen; }
            set { _sShowMsgLen = value; }
        }
        /// <summary>
        /// 驗證失敗要提示的訊息
        /// </summary>
        private string _sShowMsgDC;
        /// <summary>
        /// 驗證失敗要提示的訊息
        /// </summary>
        public string sShowMsgDC
        {
            get { return _sShowMsgDC; }
            set { _sShowMsgDC = value; }
        }

        #endregion

        #region Constructed Function


        /// <summary>
        /// 只檢查必須有值
        /// </summary>
        /// <param name="CanBeEmpty"></param>
        /// <param name="Msg"></param>
        public AttributeValidPage(bool CanBeEmpty, string MsgEmpty) 
        {
            this._CanBeEmpty = CanBeEmpty;
            this._sShowMsgEmpty = MsgEmpty;
        }
        /// <summary>
        /// 檢查必須有值，檢查長度限制
        /// </summary>
        /// <param name="CanBeEmpty"></param>
        /// <param name="isCheckLen"></param>
        /// <param name="cLength"></param>
        public AttributeValidPage(bool CanBeEmpty, string MsgEmpty,  bool isCheckLen, int cLength ,string MsgLen)
        {
            this._CanBeEmpty = CanBeEmpty;
            this._sShowMsgEmpty = MsgEmpty;
            this._isCheckLen = isCheckLen;
            this._cLength = cLength;
            this._sShowMsgLen = MsgLen;
        }

      /// <summary>
      /// 檢查是否符合字典項目
      /// </summary>
      /// <param name="CanBeEmpty"></param>
      /// <param name="MsgEmpty"></param>
      /// <param name="isCheckLen"></param>
      /// <param name="cLength"></param>
      /// <param name="MsgLen"></param>
      /// <param name="isCheckDC"></param>
      /// <param name="sDC_Head"></param>
      /// <param name="MsgDC"></param>
        public AttributeValidPage(bool CanBeEmpty, string MsgEmpty, bool isCheckLen, int cLength, string MsgLen,bool isCheckDC,string sDC_Head, string MsgDC)
        {
            this._CanBeEmpty = CanBeEmpty;
            this._sShowMsgEmpty = MsgEmpty;
            this._isCheckLen = isCheckLen;
            this._cLength = cLength;
            this._sShowMsgLen = MsgLen;
            this._isCheckDC = isCheckDC;
            this._sDC_Head = sDC_Head;
            this._sShowMsgDC = MsgDC;
        }
        #endregion

    }
}
