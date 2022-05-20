﻿//------------------------------------------------------------------------------
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
    [AttributeTable("HQ_SCDD")]
    public class EntityHQ_SCDD_edit : Entity
    {
        public EntityHQ_SCDD_edit()
        {
        }
        private string _CASE_NO;
        /// <summary>
        /// CASE_NO
        /// </summary>
        [AttributeRfPage("hidCASE_NO", "CustHiddenField", false)]
        public string CASE_NO
        {
            get
            {
                return this._CASE_NO;
            }
            set
            {
                this._CASE_NO = value;
            }
        }
        private string _CORP_NO;
        /// <summary>
        /// CORP_NO
        /// </summary>
        [AttributeRfPage("hidCORP_NO", "CustHiddenField", false)]
        public string CORP_NO
        {
            get
            {
                return this._CORP_NO;
            }
            set
            {
                this._CORP_NO = value;
            }
        }
        private string _NameCheck_No;
        /// <summary>
        /// NameCheck_No
        /// 20191101-RQ-2018-015749-002，擴案件編號欄位至70 by Peggy
        /// </summary>
        [AttributeRfPage("txtNameCheck_No", "CustTextBox", false)]
        //[AttributeValidPage(true, "名單掃描案件編號", true, 50, "名單掃描案件編號")]
        [AttributeValidPage(true, "名單掃描案件編號", true, 70, "名單掃描案件編號")]
        public string NameCheck_No
        {
            get
            {
                return this._NameCheck_No;
            }
            set
            {
                this._NameCheck_No = value;
            }
        }
        private string _NameCheck_Item;
        /// <summary>
        /// NameCheck_Item
        /// </summary>
        [AttributeRfPage("txtNameCheck_Item", "CustTextBox", false)]
        [AttributeValidPage(true, "名單掃描案件結果", true, 2, "名單掃描案件結果",true,"CT_5_", "名單掃描案件結果")]
        public string NameCheck_Item
        {
            get
            {
                return this._NameCheck_Item;
            }
            set
            {
                this._NameCheck_Item = value;
            }
        }
        private string _NameCheck_Note;
        /// <summary>
        /// NameCheck_Note
        /// </summary>
        [AttributeRfPage("txtNameCheck_Note", "CustTextBox", false)]
        [AttributeValidPage(false, "名單掃描案件NOTE", true, 500, "名單掃描案件NOTE")]
        public string NameCheck_Note
        {
            get
            {
                return this._NameCheck_Note;
            }
            set
            {
                this._NameCheck_Note = value;
            }
        }
        private string _NameCheck_RiskRanking;
        /// <summary>
        /// NameCheck_RiskRanking
        /// </summary>
        [AttributeRfPage("txtNameCheck_RiskRanking", "CustTextBox", false)]
        public string NameCheck_RiskRanking
        {
            get
            {
                return this._NameCheck_RiskRanking;
            }
            set
            {
                this._NameCheck_RiskRanking = value;
            }
        }
        private string _BusinessForeignAddress;
        /// <summary>
        /// BusinessForeignAddress
        /// </summary>
        [AttributeRfPage("txtBusinessForeignAddress", "CustTextBox", false)]
        [AttributeValidPage(false, "台灣以外主要之營業處所地址", true, 72, "台灣以外主要之營業處所地址")]
        public string BusinessForeignAddress
        {
            get
            {
                return this._BusinessForeignAddress;
            }
            set
            {
                this._BusinessForeignAddress = value;
            }
        }
        private string _RiskObject;
        /// <summary>
        /// RiskObject
        /// </summary>
        [AttributeRfPage("txtRiskObject", "CustTextBox", false)]
        [AttributeValidPage(false, "中/高風險客戶交易往來對象", true, 98, "中/高風險客戶交易往來對象")]
        public string RiskObject
        {
            get
            {
                return this._RiskObject;
            }
            set
            {
                this._RiskObject = value;
            }
        }
        private string _Organization_Item;
        /// <summary>
        /// Organization_Item
        /// </summary>
        [AttributeRfPage("txtOrganization_Item", "CustTextBox", false)]
        [AttributeValidPage(true, "組織運作", true, 2, "組織運作", true, "CT_6_", "組織運作")]
        public string Organization_Item
        {
            get
            {
                return this._Organization_Item;
            }
            set
            {
                this._Organization_Item = value;
            }
        }
        private string _Organization_Note;
        /// <summary>
        /// Organization_Note
        /// </summary>
        [AttributeRfPage("txtOrganization_Note", "CustTextBox", false)]
        [AttributeValidPage(false, "組織運作Note", true, 100, "組織運作Note")]
        public string Organization_Note
        {
            get
            {
                return this._Organization_Note;
            }
            set
            {
                this._Organization_Note = value;
            }
        }
        private string _Proof_Item;
        /// <summary>
        /// Proof_Item
        /// </summary>
        [AttributeRfPage("txtProof_Item", "CustTextBox", false)]
        [AttributeValidPage(true, "存在證明", true, 2, "存在證明", true, "CT_7_", "存在證明")]
        public string Proof_Item
        {
            get
            {
                return this._Proof_Item;
            }
            set
            {
                this._Proof_Item = value;
            }
        }
        private string _IsSanction;
        /// <summary>
        /// IsSanction
        /// </summary>
        [AttributeRfPage("txtIsSanction", "CustTextBox", false, "", "BUSI_RISK_NATION_FLAG")]
        [AttributeValidPage(true, "營業處所是否在高風險或制裁國家", true, 2, "營業處所是否在高風險或制裁國家", true, "LM_YN_", "營業處所是否在高風險或制裁國家")]
        public string IsSanction
        {
            get
            {
                return this._IsSanction;
            }
            set
            {
                this._IsSanction = value;
            }
        }
        private string _IsSanctionCountryCode1;
        /// <summary>
        /// IsSanctionCountryCode1
        /// </summary>
        [AttributeRfPage("txtIsSanctionCountryCode1", "CustTextBox", false,"", "BUSI_RISK_NATION_1")]
        [AttributeValidPage(false, "", true, 2, "高風險或制裁國家1", true, "CT_NA_", "高風險或制裁國家1 非高風險國家")]
        public string IsSanctionCountryCode1
        {
            get
            {
                return this._IsSanctionCountryCode1;
            }
            set
            {
                this._IsSanctionCountryCode1 = value;
            }
        }
        private string _IsSanctionCountryCode2;
        /// <summary>
        /// IsSanctionCountryCode2
        /// </summary>
        [AttributeRfPage("txtIsSanctionCountryCode2", "CustTextBox", false,"", "BUSI_RISK_NATION_2")]
        [AttributeValidPage(false, "", true, 2, "高風險或制裁國家2", true, "CT_NA_", "高風險或制裁國家2")]
        public string IsSanctionCountryCode2
        {
            get
            {
                return this._IsSanctionCountryCode2;
            }
            set
            {
                this._IsSanctionCountryCode2 = value;
            }
        }
        private string _IsSanctionCountryCode3;
        /// <summary>
        /// IsSanctionCountryCode3
        /// </summary>
        [AttributeRfPage("txtIsSanctionCountryCode3", "CustTextBox", false,"", "BUSI_RISK_NATION_3")]
        [AttributeValidPage(false, "", true, 2, "高風險或制裁國家3", true, "CT_NA_", "高風險或制裁國家3")]
        public string IsSanctionCountryCode3
        {
            get
            {
                return this._IsSanctionCountryCode3;
            }
            set
            {
                this._IsSanctionCountryCode3 = value;
            }
        }
        private string _IsSanctionCountryCode4;
        /// <summary>
        /// IsSanctionCountryCode4
        /// </summary>
        [AttributeRfPage("txtIsSanctionCountryCode4", "CustTextBox", false, "", "BUSI_RISK_NATION_4")]
        [AttributeValidPage(false, "", true, 2, "高風險或制裁國家4", true, "CT_NA_", "高風險或制裁國家4")]
        public string IsSanctionCountryCode4
        {
            get
            {
                return this._IsSanctionCountryCode4;
            }
            set
            {
                this._IsSanctionCountryCode4 = value;
            }
        }
        private string _IsSanctionCountryCode5;
        /// <summary>
        /// IsSanctionCountryCode5
        /// </summary>
        [AttributeRfPage("txtIsSanctionCountryCode5", "CustTextBox", false, "", "BUSI_RISK_NATION_5")]
        [AttributeValidPage(false, "", true, 2, "高風險或制裁國家5", true, "CT_NA_", "高風險或制裁國家5")]
        public string IsSanctionCountryCode5
        {
            get
            {
                return this._IsSanctionCountryCode5;
            }
            set
            {
                this._IsSanctionCountryCode5 = value;
            }
        }
         
        /// <summary>
        /// 模式切換
        /// </summary>
        /// <returns></returns>
        public EntityHQ_SCDD toShowMode()
        {
            EntityHQ_SCDD returnObj = new EntityHQ_SCDD();
            DataTableConvertor.Clone2Other<EntityHQ_SCDD_edit, EntityHQ_SCDD>(this, ref returnObj);
            return returnObj;
        }
    }
}