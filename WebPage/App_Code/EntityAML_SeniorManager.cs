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
    /// AML_SeniorManager
    /// </summary>
    [Serializable()]
    [AttributeTable("AML_SeniorManager")]
    public class EntityAML_SeniorManager : Entity, IMutiLineClass
    {
        /// <summary>
        /// 多行屬性設定
        /// </summary>
        public bool isMutilime
        {
            get { return true; }
        }
        /// <summary>
        /// 物件對應LIND ID
        /// </summary>
        public string InterFaceLineID
        {
            get { return _LineID; }
        }

        private string _isDEL ="";
        /// <summary>
        /// isDEL
        /// </summary>
        [AttributeRfPage("chkSeniorManagerDelete_", "CustCheckBox", false)]
        public string isDEL
        {
            get
            {
                return this._isDEL;
            }
            set
            {
                this._isDEL = value;
            }
        }


        private string _ID = ""; 
        /// <summary>
        /// ID
        /// </summary>
        /// //20190918 modify by Peggy
        //[AttributeRfPage("hidID_", "CustHiddenField", false)]
        [AttributeRfPage("lblID_", "CustLabel", false)]
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
        private string _BasicTaxID = "";
        /// <summary>
        /// BasicTaxID
        /// </summary>
        [AttributeRfPage("txtBasicTaxID", "CustTextBox", false)]
        [AttributeValidPage(true, "統編不可空白")]
        public string BasicTaxID
        {
            get
            {
                return this._BasicTaxID;
            }
            set
            {
                this._BasicTaxID = value;
            }
        }
        private string _Name = "";
        /// <summary>
        /// Name
        /// </summary>
        [AttributeRfPage("txtSeniorManagerName_", "CustTextBox", false,"", "BENE_NAME")]
        //20200522-高管人姓名要能輸入40個字
        //[AttributeValidPage(false,"",true,20, "經理人姓名")]
        [AttributeValidPage(false, "", true, 40, "經理人姓名")]
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
            }
        }
        private string _Birth = "";
        /// <summary>
        /// Birth
        /// </summary>
        [AttributeRfPage("txtSeniorManagerBirth_", "CustTextBox", false,"", "BENE_BIRTH_DATE")]
        [AttributeValidPage(false, "", true, 8, "經理人生日")]
        public string Birth
        {
            get
            {
                return this._Birth;
            }
            set
            {
                this._Birth = value;
            }
        }
        private string _CountryCode = "";
        /// <summary>
        /// CountryCode
        /// </summary>
        [AttributeRfPage("txtSeniorManagerCountryCode_", "CustTextBox", false,"", "BENE_NATION")]
        [AttributeValidPage(false, "", true, 2, "經理人國籍",true,"CT_1_", "經理人國籍")]
        public string CountryCode
        {
            get
            {
                return this._CountryCode;
            }
            set
            {
                this._CountryCode = value;
            }
        }
        private string _IDNo = "";
        /// <summary>
        /// IDNo
        /// </summary>
        [AttributeRfPage("txtSeniorManagerIDNo_", "CustTextBox", false,"", "BENE_ID")]
        //[AttributeValidPage(false, "", true, 10, "身分證字號")]
        [AttributeValidPage(false, "", true, 22, "身分證字號")]//20200102-20200207RC
        public string IDNo
        {
            get
            {
                return this._IDNo;
            }
            set
            {
                this._IDNo = value;
            }
        }
        private string _IDNoType = "";
        /// <summary>
        /// IDNoType
        /// </summary>
        [AttributeRfPage("txtSeniorManagerIDNoType_", "CustTextBox", false)]
        [AttributeValidPage(false, "", true, 1, "經理人身分證類型", true, "CT_99_", "經理人身分證類型")]
        public string IDNoType
        {
            get
            {
                return this._IDNoType;
            }
            set
            {
                this._IDNoType = value;
            }
        }
        private string _Identity1 = "";
        /// <summary>
        /// Identity
        /// </summary>
        [AttributeRfPage("", "CustTextBox", false, "", "BENE_JOB_TYPE")]
        [AttributeValidPage(false, "", false, 1, "經理人身分別1",true, "LM_YN_", "經理人身分別1")]
        public string Identity1
        {
            get
            {
                return this._Identity1;
            }
            set
            {
                this._Identity1 = value;
            }
        }
        private string _Identity2 = "";
        /// <summary>
        /// Identity
        /// </summary>
        [AttributeRfPage("", "CustTextBox", false, "", "BENE_JOB_TYPE_2")]
        [AttributeValidPage(false, "", false, 1, "經理人身分別1", true, "LM_YN_", "經理人身分別2")]
        public string Identity2
        {
            get
            {
                return this._Identity2;
            }
            set
            {
                this._Identity2 = value;
            }
        }
        private string _Identity3 = "";
        /// <summary>
        /// Identity
        /// </summary>
        [AttributeRfPage("", "CustTextBox", false, "", "BENE_JOB_TYPE_3")]
        [AttributeValidPage(false, "", false, 1, "經理人身分別3", true, "LM_YN_", "經理人身分別3")]
        public string Identity3
        {
            get
            {
                return this._Identity3;
            }
            set
            {
                this._Identity3 = value;
            }
        }
        private string _Identity4 = "";
        /// <summary>
        /// Identity
        /// </summary>
        [AttributeRfPage("", "CustTextBox", false, "", "BENE_JOB_TYPE_4")]
        [AttributeValidPage(false, "", false, 1, "經理人身分別4", true, "LM_YN_", "經理人身分別4")]
        public string Identity4
        {
            get
            {
                return this._Identity4;
            }
            set
            {
                this._Identity4 = value;
            }
        }
        private string _Identity5 = "";
        /// <summary>
        /// Identity
        /// </summary>
        [AttributeRfPage("", "CustTextBox", false, "", "BENE_JOB_TYPE_5")]
        [AttributeValidPage(false, "", false, 1, "經理人身分別1", true, "LM_YN_", "經理人身分別5")]
        public string Identity5
        {
            get
            {
                return this._Identity5;
            }
            set
            {
                this._Identity5 = value;
            }
        }
        private string _Identity6 = "";
        /// <summary>
        /// Identity
        /// </summary>
        [AttributeRfPage("", "CustTextBox", false, "", "BENE_JOB_TYPE_6")]
        [AttributeValidPage(false, "", false, 1, "經理人身分別6", true, "LM_YN_", "經理人身分別6")]
        public string Identity6
        {
            get
            {
                return this._Identity6;
            }
            set
            {
                this._Identity6 = value;
            }
        }
        private string _Expdt = "";
        /// <summary>
        /// Expdt
        /// </summary>
        [AttributeRfPage("txtSeniorManagerExpdt_", "CustTextBox", false)]
        [AttributeValidPage(false, "", true, 8, "到期日")]
        public string Expdt
        {
            get
            {
                return this._Expdt;
            }
            set
            {
                this._Expdt = value;
            }
        }
        private string _keyin_Flag = "";
        /// <summary>
        /// keyin_Flag
        /// </summary>

        public string keyin_Flag
        {
            get
            {
                return this._keyin_Flag;
            }
            set
            {
                this._keyin_Flag = value;
            }
        }
        private string _keyin_day = "";
        /// <summary>
        /// keyin_day
        /// </summary>

        public string keyin_day
        {
            get
            {
                return this._keyin_day;
            }
            set
            {
                this._keyin_day = value;
            }
        }
        private string _keyin_userID = "";
        /// <summary>
        /// keyin_userID
        /// </summary>

        public string keyin_userID
        {
            get
            {
                return this._keyin_userID;
            }
            set
            {
                this._keyin_userID = value;
            }
        }

        private string _LineID = "";
        /// <summary>
        /// ID
        /// </summary>
        [AttributeRfPage("txtID", "CustTextBox", false)]
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
        //20190806-RQ-2019-008595-002-長姓名需求:增開中文長姓名&羅馬拼音 2個欄位 by Peggy
        private string _Name_L = "";
        /// <summary>
        /// Name_L
        /// </summary>
        [AttributeRfPage("txtName_L_", "CustTextBox", false, "", "")]
        [AttributeValidPage(false, "", false, 50, "高管人員-中文長姓名")]
        public string Name_L
        {
            get
            {
                return this._Name_L;
            }
            set
            {
                this._Name_L = value;
            }
        }

        private string _Name_Pinyin = "";
        /// <summary>
        /// Name_Pinyin
        /// </summary>
        [AttributeRfPage("txtName_Pinyin_", "CustTextBox", false, "", "")]
        [AttributeValidPage(false, "", false, 50, "高管人員-羅馬拼音")]
        public string Name_Pinyin
        {
            get
            {
                return this._Name_Pinyin;
            }
            set
            {
                this._Name_Pinyin = value;
            }
        }
        //20190805 長姓名需求，新3個欄位 by Peggy↑

    }
}
