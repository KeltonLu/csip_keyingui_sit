//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:2.0.50727.42
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;


namespace CSIPNewInvoice.EntityLayer_new
{
    /// <summary>
    /// AML_HeadOffice
    /// </summary>
    [Serializable()]
    [AttributeTable("NoteLog")]
    public class EntityNoteLog : Entity
    {
        public EntityNoteLog()
        {
        }
        private string _NL_CASE_NO;
        /// <summary>
        /// NL_CASE_NO
        /// </summary>
        [AttributeRfPage("NotelblNL_CASE_NO", "CustLabel", false)]
        public string NL_CASE_NO
        {
            get
            {
                return this._NL_CASE_NO;
            }
            set
            {
                this._NL_CASE_NO = value;
            }
        }
        private string _NL_SecondKey;
        /// <summary>
        /// NL_SecondKey
        /// </summary>
        [AttributeRfPage("NotelblNL_SecondKey", "CustLabel", false)]
        public string NL_SecondKey
        {
            get
            {
                return this._NL_SecondKey;
            }
            set
            {
                this._NL_SecondKey = value;
            }
        }
        private string _NL_DateTime;
        /// <summary>
        /// NL_DateTime
        /// </summary>
        [AttributeRfPage("NotelblNL_DateTime", "CustLabel", false)]
        public string NL_DateTime
        {
            get
            {
                return this._NL_DateTime;
            }
            set
            {
                this._NL_DateTime = value;
            }
        }
        private string _NL_User;
        /// <summary>
        /// NL_User
        /// </summary>
        [AttributeRfPage("NotelblNL_User", "CustLabel", false)]
        public string NL_User
        {
            get
            {
                return this._NL_User;
            }
            set
            {
                this._NL_User = value;
            }
        }
        private string _NL_Type;
        /// <summary>
        /// NL_Type
        /// </summary>
        [AttributeRfPage("NotelblNL_Type", "CustLabel", false)]
        public string NL_Type
        {
            get
            {
                return this._NL_Type;
            }
            set
            {
                this._NL_Type = value;
            }
        }
        private string _NL_Value;
        /// <summary>
        /// NL_Value
        /// </summary>
        [AttributeRfPage("NotelblNL_Value", "CustLabel", false)]
        public string NL_Value
        {
            get
            {
                return this._NL_Value;
            }
            set
            {
                this._NL_Value = value;
            }
        }
        private string _NL_ShowFlag;
        /// <summary>
        /// NL_ShowFlag
        /// </summary>
        [AttributeRfPage("NotelblNL_ShowFlag", "CustLabel", false)]
        public string NL_ShowFlag
        {
            get
            {
                return this._NL_ShowFlag;
            }
            set
            {
                this._NL_ShowFlag = value;
            }
        }



        /// <summary>
        /// 轉換型別
        /// </summary>
        /// <returns></returns>
        public EntityNoteLog_edit toEditMode()
        {
            EntityNoteLog_edit returnObj = new EntityNoteLog_edit();
            DataTableConvertor.Clone2Other<EntityNoteLog, EntityNoteLog_edit>(this, ref returnObj);
            return returnObj;
        }
    }
}