using Framework.Data.OM;
using Framework.Data.OM.OMAttribute;
using System;

namespace CSIPCommonModel.EntityLayer_new
{
    /// <summary>
    /// EntityL_BATCH_LOG
    /// </summary>
    [Serializable()]
    [AttributeTable("L_AP_LOG")]
    public class EntityL_AP_LOG : Entity
    {

        private string _System_Code;
        /// <summary>
        /// �t�ΧO
        /// </summary>        
        [AttributeField("System_Code", "System.String", false, false, false, "String")]
        public string System_Code
        {
            get
            {
                return this._System_Code;
            }
            set
            {
                this._System_Code = value;
            }
        }
        private string _Login_Account_Nbr;
        /// <summary>
        /// �ϥΪ̱b��(���u�s��)
        /// </summary>        
        [AttributeField("Login_Account_Nbr", "System.String", false, false, false, "String")]
        public string Login_Account_Nbr
        {
            get
            {
                return this._Login_Account_Nbr;
            }
            set
            {
                this._Login_Account_Nbr = value;
            }
        }
        private string _Query_Datetime;
        /// <summary>
        /// �d�ߤ���ɶ�
        /// </summary>        
        [AttributeField("Query_Datetime", "System.DateTime", false, false, false, "DateTime")]
        public string Query_Datetime
        {
            get
            {
                return this._Query_Datetime;
            }
            set
            {
                this._Query_Datetime = value;
            }
        }
        private string _AP_Txn_Code;
        /// <summary>
        /// ����N���εe���N�X
        /// </summary>        
        [AttributeField("AP_Txn_Code", "System.String", false, false, false, "String")]
        public string AP_Txn_Code
        {
            get
            {
                return this._AP_Txn_Code;
            }
            set
            {
                this._AP_Txn_Code = value;
            }
        }
        private string _Server_Name;
        /// <summary>
        /// �ҳs�u�ؼи�Ʈw/���A��IP
        /// </summary>        
        [AttributeField("Server_Name", "System.String", false, false, false, "String")]
        public string Server_Name
        {
            get
            {
                return this._Server_Name;
            }
            set
            {
                this._Server_Name = value;
            }
        }
        private string _User_Terminal;
        /// <summary>
        /// �ϥΪ̲׺ݳ]��Id
        /// </summary>
        [AttributeField("User_Terminal", "System.String", false, false, false, "String")]
        public string User_Terminal
        {
            get
            {
                return this._User_Terminal;
            }
            set
            {
                this._User_Terminal = value;
            }
        }
        private string _AP_Account_Nbr;
        /// <summary>
        /// �ϥΤ�AP�b��
        /// </summary>
        [AttributeField("AP_Account_Nbr", "System.String", false, false, false, "String")]
        public string AP_Account_Nbr
        {
            get
            {
                return this._AP_Account_Nbr;
            }
            set
            {
                this._AP_Account_Nbr = value;
            }
        }
        private string _Txn_Type_Code;
        /// <summary>
        /// �ʧ@���O
        /// </summary>
        [AttributeField("Txn_Type_Code", "System.String", false, false, false, "String")]
        public string Txn_Type_Code
        {
            get
            {
                return this._Txn_Type_Code;
            }
            set
            {
                this._Txn_Type_Code = value;
            }
        }
        private string _Statement_Text;
        /// <summary>
        /// ����Ѽ�: �� | �Ϲj,�Y�L�Ȥ��ݱa,���O���n���t , 
        /// </summary>
        [AttributeField("Statement_Text", "System.String", false, false, false, "String")]
        public string Statement_Text
        {
            get
            {
                return this._Statement_Text;
            }
            set
            {
                this._Statement_Text = value;
            }
        }
        private string _Object_Name;
        /// <summary>
        /// �Ҧs�����ɮ�/����W��
        /// </summary>
        [AttributeField("Object_Name", "System.String", false, false, false, "String")]
        public string Object_Name
        {
            get
            {
                return this._Object_Name;
            }
            set
            {
                this._Object_Name = value;
            }
        }
        private string _Txn_Status_Code;
        /// <summary>
        /// ���檬�A:Y-���\,N-����,T:TIME OUT
        /// </summary>
        [AttributeField("Txn_Status_Code", "System.String", false, false, false, "String")]
        public string Txn_Status_Code
        {
            get
            {
                return this._Txn_Status_Code;
            }
            set
            {
                this._Txn_Status_Code = value;
            }
        }
        private string _Customer_Id;
        /// <summary>
        /// Customer_Id:�Ȥ�ID/ �νs
        /// </summary>
        [AttributeField("Customer_Id", "System.String", false, false, false, "String")]
        public string Customer_Id
        {
            get
            {
                return this._Customer_Id;
            }
            set
            {
                this._Customer_Id = value;
            }
        }
        private string _Account_Nbr;
        /// <summary>
        /// ����b��/�d��:AC_NO
        /// </summary>
        [AttributeField("Account_Nbr", "System.String", false, false, false, "String")]
        public string Account_Nbr
        {
            get
            {
                return this._Account_Nbr;
            }
            set
            {
                this._Account_Nbr = value;
            }
        }
        private string _Branch_Nbr;
        /// <summary>
        /// �b�Ȥ���O
        /// </summary>
        [AttributeField("Branch_Nbr", "System.String", false, false, false, "String")]
        public string Branch_Nbr
        {
            get
            {
                return this._Branch_Nbr;
            }
            set
            {
                this._Branch_Nbr = value;
            }
        }
        private string _Role_Id;
        /// <summary>
        /// �n�J�t�αb��������(�ݪ`�N����)
        /// </summary>        
        [AttributeField("Role_Id", "System.String", false, false, false, "String")]
        public string Role_Id
        {
            get
            {
                return this._Role_Id;
            }
            set
            {
                this._Role_Id = value;
            }
        }
        private string _Import_Source;
        /// <summary>
        /// ��ƨӷ�
        /// </summary>
        [AttributeField("Import_Source", "System.String", false, false, false, "String")]
        public string Import_Source
        {
            get
            {
                return this._Import_Source;
            }
            set
            {
                this._Import_Source = value;
            }
        }
        private string _As_Of_Date;
        /// <summary>
        /// ��Ƥ��:��Ƥ�YYYYMMDD�A�Ҧp�G����12/2��骺��ơA�����h��J��20151202
        /// </summary>
        [AttributeField("As_Of_Date", "System.String", false, false, false, "String")]
        public string As_Of_Date
        {
            get
            {
                return this._As_Of_Date;
            }
            set
            {
                this._As_Of_Date = value;
            }
        }

    }    
}
