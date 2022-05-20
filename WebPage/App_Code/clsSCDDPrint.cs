using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// clsSCDDPrint 的摘要描述
/// </summary>
[Serializable()]
public class clsSCDDPrint
{
    /// <summary>
    /// 初始化時建立高階經理人、分公司負責人
    /// </summary>
    public clsSCDDPrint()
    {
        _ManagerColl = new List<clsMangger>();
    }
    /// <summary>
    /// 統一編號
    /// </summary>
    private string _AML_HQ_Work_HCOP_HEADQUATERS_CORP_NO;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_HEADQUATERS_CORP_NO", "B2")]
    public string AML_HQ_Work_HCOP_HEADQUATERS_CORP_NO
    {
        get { return _AML_HQ_Work_HCOP_HEADQUATERS_CORP_NO; }
        set { _AML_HQ_Work_HCOP_HEADQUATERS_CORP_NO = value; }
    }
    /// <summary>
    /// 登記名稱
    /// </summary>
    private string _AML_HQ_Work_HCOP_REG_NAME;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_REG_NAME", "D2")]
    public string AML_HQ_Work_HCOP_REG_NAME
    {
        get { return _AML_HQ_Work_HCOP_REG_NAME; }
        set { _AML_HQ_Work_HCOP_REG_NAME = value; }
    }
    /// <summary>
    /// 登記名稱(英文)
    /// </summary>
    private string _AML_HQ_Work_HCOP_CORP_REG_ENG_NAME;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_CORP_REG_ENG_NAME", "F2")]
    public string AML_HQ_Work_HCOP_CORP_REG_ENG_NAME
    {
        get { return _AML_HQ_Work_HCOP_CORP_REG_ENG_NAME; }
        set { _AML_HQ_Work_HCOP_CORP_REG_ENG_NAME = value; }
    }
    /// <summary>
    /// 設立日期
    /// </summary>
    private string _AML_HQ_Work_HCOP_BUILD_DATE;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_BUILD_DATE", "H2")]
    public string AML_HQ_Work_HCOP_BUILD_DATE
    {
        get { return _AML_HQ_Work_HCOP_BUILD_DATE; }
        set { _AML_HQ_Work_HCOP_BUILD_DATE = value; }
    }
    /// <summary>
    /// 商店狀態 
    /// </summary>
    private string _AML_HQ_Work_HCOP_STATUS;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_STATUS", "B3")]
    public string AML_HQ_Work_HCOP_STATUS
    {
        get { return _AML_HQ_Work_HCOP_STATUS; }
        set { _AML_HQ_Work_HCOP_STATUS = value; }
    }

    /// <summary>
    /// 註冊國籍 
    /// </summary>
    private string _AML_HQ_Work_HCOP_REGISTER_NATION;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_REGISTER_NATION", "D3")]
    public string AML_HQ_Work_HCOP_REGISTER_NATION
    {
        get { return _AML_HQ_Work_HCOP_REGISTER_NATION; }
        set { _AML_HQ_Work_HCOP_REGISTER_NATION = value; }
    }

    /// <summary>
    /// 美國州別
    /// </summary>
    private string _AML_HQ_Work_HCOP_REGISTER_US_STATE;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_REGISTER_US_STATE", "F3")]
    public string AML_HQ_Work_HCOP_REGISTER_US_STATE
    {
        get { return _AML_HQ_Work_HCOP_REGISTER_US_STATE; }
        set { _AML_HQ_Work_HCOP_REGISTER_US_STATE = value; }
    }
    /// <summary>
    /// 行業別
    /// </summary>
    private string _AML_HQ_Work_HCOP_CC;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_CC", "B4")]
    public string AML_HQ_Work_HCOP_CC
    {
        get { return _AML_HQ_Work_HCOP_CC; }
        set { _AML_HQ_Work_HCOP_CC = value; }
    }

    /// <summary>
    /// 行業別中文名稱
    /// </summary>
    private string _AML_HQ_Work_HCOP_CC_Name;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_CC_Name", "D4")]
    public string AML_HQ_Work_HCOP_CC_Name
    {
        get { return _AML_HQ_Work_HCOP_CC_Name; }
        set { _AML_HQ_Work_HCOP_CC_Name = value; }
    }
    /// <summary>
    /// 法律形式
    /// </summary>
    private string _AML_HQ_Work_HCOP_BUSINESS_ORGAN_TYPE;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_BUSINESS_ORGAN_TYPE", "B5")]
    public string AML_HQ_Work_HCOP_BUSINESS_ORGAN_TYPE
    {
        get { return _AML_HQ_Work_HCOP_BUSINESS_ORGAN_TYPE; }
        set { _AML_HQ_Work_HCOP_BUSINESS_ORGAN_TYPE = value; }
    }
    /// <summary>
    /// 複雜股權結構
    /// </summary>
    private string _AML_HQ_Work_HCOP_COMPLEX_STR_CODE;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_COMPLEX_STR_CODE", "D5")]
    public string AML_HQ_Work_HCOP_COMPLEX_STR_CODE
    {
        get { return _AML_HQ_Work_HCOP_COMPLEX_STR_CODE; }
        set { _AML_HQ_Work_HCOP_COMPLEX_STR_CODE = value; }
    }
    /// <summary>
    /// 是否可發行無記名股票
    /// </summary>
    private string _AML_HQ_Work_HCOP_ALLOW_ISSUE_STOCK_FLAG;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_ALLOW_ISSUE_STOCK_FLAG", "F5")]
    public string AML_HQ_Work_HCOP_ALLOW_ISSUE_STOCK_FLAG
    {
        get { return _AML_HQ_Work_HCOP_ALLOW_ISSUE_STOCK_FLAG; }
        set { _AML_HQ_Work_HCOP_ALLOW_ISSUE_STOCK_FLAG = value; }
    }
    /// <summary>
    /// 是否已發行無記名股票
    /// </summary>
    private string _AML_HQ_Work_HCOP_ISSUE_STOCK_FLAG;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_ISSUE_STOCK_FLAG", "H5")]
    public string AML_HQ_Work_HCOP_ISSUE_STOCK_FLAG
    {
        get { return _AML_HQ_Work_HCOP_ISSUE_STOCK_FLAG; }
        set { _AML_HQ_Work_HCOP_ISSUE_STOCK_FLAG = value; }
    }
    /// <summary>
    /// 組織運作
    /// </summary>
    private string _HQ_SCDD_Organization_Item;
    [AttributeRFPrint("lbHQ_SCDD_Organization_Item", "B6")]
    public string HQ_SCDD_Organization_Item
    {
        get { return _HQ_SCDD_Organization_Item; }
        set { _HQ_SCDD_Organization_Item = value; }
    }
    /// <summary>
    /// 組織運作NOTE
    /// </summary>
    private string _HQ_SCDD_Organization_Note;
    [AttributeRFPrint("lbHQ_SCDD_Organization_Note", "B7")]
    public string HQ_SCDD_Organization_Note
    {
        get { return _HQ_SCDD_Organization_Note; }
        set { _HQ_SCDD_Organization_Note = value; }
    }
    /// <summary>
    /// 存在證明
    /// </summary>
    private string _HQ_SCDD_Proof_Item;
    [AttributeRFPrint("lbHQ_SCDD_Proof_Item", "B8")]
    public string HQ_SCDD_Proof_Item
    {
        get { return _HQ_SCDD_Proof_Item; }
        set { _HQ_SCDD_Proof_Item = value; }
    }
    /// <summary>
    /// 台灣以外主要之營業處所地址
    /// </summary>
    private string _HQ_SCDD_BusinessForeignAddress;
    [AttributeRFPrint("lbHQ_SCDD_BusinessForeignAddress", "B9")]
    public string HQ_SCDD_BusinessForeignAddress
    {
        get { return _HQ_SCDD_BusinessForeignAddress; }
        set { _HQ_SCDD_BusinessForeignAddress = value; }
    }

    /// <summary>
    /// 主要之營業處所國別
    /// </summary>
    private string _AML_HQ_Work_HCOP_PRIMARY_BUSI_COUNTRY;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_PRIMARY_BUSI_COUNTRY", "H9")]
    public string AML_HQ_Work_HCOP_PRIMARY_BUSI_COUNTRY
    {
        get { return _AML_HQ_Work_HCOP_PRIMARY_BUSI_COUNTRY; }
        set { _AML_HQ_Work_HCOP_PRIMARY_BUSI_COUNTRY = value; }
    }
    /// <summary>
    /// 營業處所是否在高風險或制裁國家
    /// </summary>
    private string _IsSanction;
    [AttributeRFPrint("lbIsSanction", "B10")]
    public string IsSanction
    {
        get { return _IsSanction; }
        set { _IsSanction = value; }
    }

    /// <summary>
    /// 營業處所是否在高風險或制裁國家
    /// </summary>
    private string _IsSanctionCountryCode;
    [AttributeRFPrint("lbIsSanctionCountryCode", "B11")]
    public string IsSanctionCountryCode
    {
        get { return _IsSanctionCountryCode; }
        set { _IsSanctionCountryCode = value; }
    }
    /// <summary>
    /// 業務往來目的
    /// </summary>
    private string _CustLabel34;
    [AttributeRFPrint("CustLabel34", "B12")]
    public string lbCustLabel34
    {
        get { return _CustLabel34; }
        set { _CustLabel34 = value; }
    }


    /// <summary>
    /// 中/高風險客戶交易往來對象(請客戶提供主要客戶或供應商
    /// </summary>
    private string _CustLabel30;
    [AttributeRFPrint("CustLabel30", "B13")]
    public string lbCustLabel30
    {
        get { return _CustLabel30; }
        set { _CustLabel30 = value; }
    }
    /// <summary>
    /// 中/高風險客戶交易往來對象(請客戶提供主要客戶或供應商
    /// </summary>
    private string _HQ_SCDD_RiskObject;
    [AttributeRFPrint("lbHQ_SCDD_RiskObject", "B14")]
    public string HQ_SCDD_RiskObject
    {
        get { return _HQ_SCDD_RiskObject; }
        set { _HQ_SCDD_RiskObject = value; }
    }
    /// <summary>
    /// -僑外資/外商
    /// </summary>
    private string _AML_HQ_Work_HCOP_OVERSEAS_FOREIGN;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_OVERSEAS_FOREIGN", "B15")]
    public string AML_HQ_Work_HCOP_OVERSEAS_FOREIGN
    {
        get { return _AML_HQ_Work_HCOP_OVERSEAS_FOREIGN; }
        set { _AML_HQ_Work_HCOP_OVERSEAS_FOREIGN = value; }
    }
    /// <summary>
    /// -僑外資/外商
    /// </summary>
    private string _AML_HQ_Work_HCOP_OVERSEAS_FOREIGN_COUNTRY;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_OVERSEAS_FOREIGN_COUNTRY", "D15")]
    public string AML_HQ_Work_HCOP_OVERSEAS_FOREIGN_COUNTRY
    {
        get { return _AML_HQ_Work_HCOP_OVERSEAS_FOREIGN_COUNTRY; }
        set { _AML_HQ_Work_HCOP_OVERSEAS_FOREIGN_COUNTRY = value; }
    }
    /// <summary>
    /// 警示帳戶或衍生警示帳戶
    /// </summary>
    private string _AML_Cdata_Work_WarningFlag;
    [AttributeRFPrint("lbAML_Cdata_Work_WarningFlag", "B16")]
    public string AML_Cdata_Work_WarningFlag
    {
        get { return _AML_Cdata_Work_WarningFlag; }
        set { _AML_Cdata_Work_WarningFlag = value; }
    }
    /// <summary>
    /// 曾被申報過疑似洗錢Filed SAR註記
    /// </summary>
    private string _AML_Cdata_Work_FiledSAR;
    [AttributeRFPrint("lbAML_Cdata_Work_FiledSAR", "D16")]
    public string AML_Cdata_Work_FiledSAR
    {
        get { return _AML_Cdata_Work_FiledSAR; }
        set { _AML_Cdata_Work_FiledSAR = value; }
    }

    /// <summary>
    /// 不合作/拒絕提供資訊
    /// </summary>
    private string _AML_Cdata_Work_Incorporated;
    [AttributeRFPrint("lbAML_Cdata_Work_Incorporated", "F16")]
    public string AML_Cdata_Work_Incorporated
    {
        get { return _AML_Cdata_Work_Incorporated; }
        set { _AML_Cdata_Work_Incorporated = value; }
    }
    /// <summary>
    /// AML名單掃描案件編號
    /// 20191101-RQ-2018-015749-002 add by Peggy
    /// </summary>
    private string _HQ_SCDD_NameCheck_No;
    [AttributeRFPrint("lblHQ_SCDD_NameCheck_No", "B17")]
    public string HQ_SCDD_NameCheck_No
    {
        get { return _HQ_SCDD_NameCheck_No; }
        set { _HQ_SCDD_NameCheck_No = value; }
    }
    /// <summary>
    /// AML名單掃描結果1
    /// 20191101-RQ-2018-015749-002 -因新增案件編號欄位，故既有欄位往後加 by Peggy
    /// </summary>
    private string _HQ_SCDD_NameCheck_Item;
    //[AttributeRFPrint("lbHQ_SCDD_NameCheck_Item", "B17")]
    [AttributeRFPrint("lbHQ_SCDD_NameCheck_Item", "F17")]
    public string HQ_SCDD_NameCheck_Item
    {
        get { return _HQ_SCDD_NameCheck_Item + _HQ_SCDD_NameCheck_Note; }
        set { _HQ_SCDD_NameCheck_Item = value; }
    }
    /// <summary>
    /// AML名單掃描結果2
    /// </summary>
    private string _HQ_SCDD_NameCheck_Note;
    [AttributeRFPrint("lbHQ_SCDD_NameCheck_Note", "")]
    public string HQ_SCDD_NameCheck_Note
    {
        get { return _HQ_SCDD_NameCheck_Note; }
        set { _HQ_SCDD_NameCheck_Note = value; }
    }

    /// <summary>
    /// 負責人姓名
    /// </summary>
    private string _AML_HQ_Work_HCOP_OWNER_CHINESE_NAME;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_OWNER_CHINESE_NAME", "B19")]
    public string AML_HQ_Work_HCOP_OWNER_CHINESE_NAME
    {
        get { return _AML_HQ_Work_HCOP_OWNER_CHINESE_NAME; }
        set { _AML_HQ_Work_HCOP_OWNER_CHINESE_NAME = value; }
    }
    /// <summary>
    /// 證件類型/號碼 身分證
    /// </summary>
    private string _AML_HQ_Work_HCOP_OWNER_ID;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_OWNER_ID", "D18")]
    public string AML_HQ_Work_HCOP_OWNER_ID
    {
        get { return "身分證 " + _AML_HQ_Work_HCOP_OWNER_ID; }
        set { _AML_HQ_Work_HCOP_OWNER_ID = value; }
    }
    /// <summary>
    /// 證件類型/號碼 護照
    /// </summary>
    private string _AML_HQ_Work_HCOP_PASSPORT;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_PASSPORT", "D19")]
    public string AML_HQ_Work_HCOP_PASSPORT
    {
        get { return "護照 " + _AML_HQ_Work_HCOP_PASSPORT; }
        set { _AML_HQ_Work_HCOP_PASSPORT = value; }
    }
    /// <summary>
    /// 證件類型/號碼 居留證
    /// </summary>
    private string _AML_HQ_Work_HCOP_RESIDENT_NO;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_RESIDENT_NO", "D20")]
    public string AML_HQ_Work_HCOP_RESIDENT_NO
    {
        //20200410-RQ-2019-030155-005-居留證號更名為統一證號
        //get { return "居留證 " + _AML_HQ_Work_HCOP_RESIDENT_NO; }
        get { return "統一證號 " + _AML_HQ_Work_HCOP_RESIDENT_NO; }
        set { _AML_HQ_Work_HCOP_RESIDENT_NO = value; }
    }
    /// <summary>
    /// 負責人生日
    /// </summary>
    private string _AML_HQ_Work_HCOP_OWNER_BIRTH_DATE;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_OWNER_BIRTH_DATE", "F19")]
    public string AML_HQ_Work_HCOP_OWNER_BIRTH_DATE
    {
        get { return _AML_HQ_Work_HCOP_OWNER_BIRTH_DATE; }
        set { _AML_HQ_Work_HCOP_OWNER_BIRTH_DATE = value; }
    }
    /// <summary>
    /// 負責人國籍
    /// </summary>
    private string _AML_HQ_Work_HCOP_OWNER_NATION;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_OWNER_NATION", "H19")]
    public string AML_HQ_Work_HCOP_OWNER_NATION
    {
        get { return _AML_HQ_Work_HCOP_OWNER_NATION; }
        set { _AML_HQ_Work_HCOP_OWNER_NATION = value; }
    }
    /// <summary>
    /// 負責人英文姓名
    /// </summary>
    private string _AML_HQ_Work_HCOP_OWNER_ENGLISH_NAME;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_OWNER_ENGLISH_NAME", "B21")]
    public string AML_HQ_Work_HCOP_OWNER_ENGLISH_NAME
    {
        get { return _AML_HQ_Work_HCOP_OWNER_ENGLISH_NAME; }
        set { _AML_HQ_Work_HCOP_OWNER_ENGLISH_NAME = value; }
    }
    /// <summary>
    /// 負責人英文姓名
    /// </summary>
    private string _AML_HQ_Work_OWNER_ID_SreachStatus;
    [AttributeRFPrint("lbAML_HQ_Work_OWNER_ID_SreachStatus", "D21")]
    public string AML_HQ_Work_OWNER_ID_SreachStatus
    {
        get { return _AML_HQ_Work_OWNER_ID_SreachStatus; }
        set { _AML_HQ_Work_OWNER_ID_SreachStatus = value; }
    }
    /// <summary>
    /// 計算風險等級或異常結案 Foot!B1
    /// </summary>
    private string _SR_RiskItem;
    [AttributeRFPrint("lbSR_RiskItem", "")]
    public string SR_RiskItem
    {
        get { return _SR_RiskItem + _SR_RiskNote; }
        set { _SR_RiskItem = value; }
    }
    /// <summary>
    /// 計算風險等級或異常結案
    /// </summary>
    private string _SR_RiskNote;
    [AttributeRFPrint("lbSR_RiskNote", "")]
    public string SR_RiskNote
    {
        get { return _SR_RiskNote; }
        set { _SR_RiskNote = value; }
    }
    /// <summary>
    /// 綜合說明及審查意見  Foot!B2
    /// </summary>
    private string _SR_Explanation;
    [AttributeRFPrint("lbSR_Explanation", "")]
    public string SR_Explanation
    {
        get { return _SR_Explanation; }
        set { _SR_Explanation = value; }
    }
    /// <summary>
    /// EDD完成日期
    /// </summary>
    private string _SR_EDD_Date;
    [AttributeRFPrint("lbSR_EDD_Date", "")]
    public string SR_EDD_Date
    {
        get { return _SR_EDD_Date; }
        set { _SR_EDD_Date = value; }
    }
    /// <summary>
    /// 經辦人宣示 Foot!B3
    /// </summary>
    private string _lbLabel9;
    [AttributeRFPrint("Label9", "")]
    public string lbLabel9
    {
        get { return _lbLabel9; }
        set { _lbLabel9 = value; }
    }
    /// <summary>
    /// 經辦(簽章)
    /// </summary>
    private string _lbLabel1;
    [AttributeRFPrint("Label1", "")]
    public string lbLabel1
    {
        get { return _lbLabel1; }
        set { _lbLabel1 = value; }
    }
    /// <summary>
    /// 經辦 日期
    /// </summary>
    private string _lbLabel2;
    [AttributeRFPrint("Label2", "")]
    public string lbLabel2
    {
        get { return _lbLabel2; }
        set { _lbLabel2 = value; }
    }
    List<clsMangger> _ManagerColl;
    public List<clsMangger> ManagerColl
    {
        get { return _ManagerColl; }
        set { _ManagerColl = value; }
    }

   /// <summary>
   /// 負責人長姓名
   /// </summary>
    private string _HQlblHCOP_OWNER_CHINESE_LNAME;
    [AttributeRFPrint("HQlblHCOP_OWNER_CHINESE_LNAME", "B22")]
    public string HCOP_OWNER_CHINESE_LNAME
    {
        get { return _HQlblHCOP_OWNER_CHINESE_LNAME; }
        set { _HQlblHCOP_OWNER_CHINESE_LNAME = value; }
    }
    /// <summary>
    /// 負責人羅馬
    /// </summary>
    private string _HQlblHCOP_OWNER_ROMA;
    [AttributeRFPrint("HQlblHCOP_OWNER_ROMA", "B23")]
    public string HCOP_OWNER_ROMA
    {
        get { return _HQlblHCOP_OWNER_ROMA; }
        set { _HQlblHCOP_OWNER_ROMA = value; }
    } 
}
public class clsSCDDPrintNaturalPerson
{
    /// <summary>
    /// 計算風險等級或異常結案 Foot!B1
    /// </summary>
    private string _SR_RiskItem;
    [AttributeRFPrint("lbSR_RiskItem", "")]
    public string SR_RiskItem
    {
        get { return _SR_RiskItem + _SR_RiskNote; }
        set { _SR_RiskItem = value; }
    }
    /// <summary>
    /// 計算風險等級或異常結案
    /// </summary>
    private string _SR_RiskNote;
    [AttributeRFPrint("lbSR_RiskNote", "")]
    public string SR_RiskNote
    {
        get { return _SR_RiskNote; }
        set { _SR_RiskNote = value; }
    }
    /// <summary>
    /// 統一編號
    /// </summary>
    private string _AML_HQ_Work_HCOP_HEADQUATERS_CORP_NO;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_HEADQUATERS_CORP_NO", "B2")]
    public string AML_HQ_Work_HCOP_HEADQUATERS_CORP_NO
    {
        get { return _AML_HQ_Work_HCOP_HEADQUATERS_CORP_NO; }
        set { _AML_HQ_Work_HCOP_HEADQUATERS_CORP_NO = value; }
    }
    /// <summary>
    /// 綜合說明及審查意見  Foot!B2
    /// </summary>
    private string _SR_Explanation;
    [AttributeRFPrint("lbSR_Explanation", "")]
    public string SR_Explanation
    {
        get { return _SR_Explanation; }
        set { _SR_Explanation = value; }
    }
    /// <summary>
    /// 經辦人宣示 Foot!B3
    /// </summary>
    private string _lbLabel9;
    [AttributeRFPrint("Label9", "")]
    public string lbLabel9
    {
        get { return _lbLabel9; }
        set { _lbLabel9 = value; }
    }
    /// <summary>
    /// 經辦(簽章)
    /// </summary>
    private string _lbLabel1;
    [AttributeRFPrint("Label1", "")]
    public string lbLabel1
    {
        get { return _lbLabel1; }
        set { _lbLabel1 = value; }
    }
    /// <summary>
    /// 經辦 日期
    /// </summary>
    private string _lbLabel2;
    [AttributeRFPrint("Label2", "")]
    public string lbLabel2
    {
        get { return _lbLabel2; }
        set { _lbLabel2 = value; }
    }
    List<clsMangger> _ManagerColl;
    public List<clsMangger> ManagerColl
    {
        get { return _ManagerColl; }
        set { _ManagerColl = value; }
    }

    /// <summary>
    /// 戶名
    /// </summary>
    private string _HQlblHCOP_OWNER_CHINESE_NAME;
    [AttributeRFPrint("HQlblHCOP_OWNER_CHINESE_NAME", "B3")]
    public string HQlblHCOP_OWNER_CHINESE_NAME
    {
        get { return _HQlblHCOP_OWNER_CHINESE_NAME; }
        set { _HQlblHCOP_OWNER_CHINESE_NAME = value; }
    }
    /// <summary>
    /// 身分證號碼
    /// </summary>
    private string _HQlblHCOP_OWNER_ID;
    [AttributeRFPrint("HQlblHCOP_OWNER_ID", "E3")]
    public string HQlblHCOP_OWNER_ID
    {
        get { return _HQlblHCOP_OWNER_ID; }
        set { _HQlblHCOP_OWNER_ID = value; }
    }
    
    /// <summary>
    /// 戶籍地址
    /// </summary>
    private string _HIDE_REG_ADDR;
    [AttributeRFPrint("HIDE_REG_ADDR", "B4")]
    public string HIDE_REG_ADDR
    {
        get { return _HIDE_REG_ADDR; }
        set { _HIDE_REG_ADDR = value; }
    }
    /// <summary>
    /// 通訊地址
    /// </summary>
    private string _HIDE_MAILING_ADDR;
    [AttributeRFPrint("HIDE_MAILING_ADDR", "B5")]
    public string HIDE_MAILING_ADDR
    {
        get { return _HIDE_MAILING_ADDR; }
        set { _HIDE_MAILING_ADDR = value; }
    }
    /// <summary>
    /// 商店狀態 
    /// </summary>
    private string _AML_HQ_Work_HCOP_STATUS;
    [AttributeRFPrint("lbAML_HQ_Work_HCOP_STATUS", "B6")]
    public string AML_HQ_Work_HCOP_STATUS
    {
        get { return _AML_HQ_Work_HCOP_STATUS; }
        set { _AML_HQ_Work_HCOP_STATUS = value; }
    }
    /// <summary>
    /// 國籍1
    /// </summary>
    private string _HQlblHCOP_OWNER_NATION;
    [AttributeRFPrint("HQlblHCOP_OWNER_NATION", "E6")]
    public string HQlblHCOP_OWNER_NATION
    {
        get { return _HQlblHCOP_OWNER_NATION; }
        set { _HQlblHCOP_OWNER_NATION = value; }
    }
    /// <summary>
    /// 國籍2
    /// </summary>
    private string _HQlblHCOP_COUNTRY_CODE_2;
    [AttributeRFPrint("HQlblHCOP_COUNTRY_CODE_2", "H6")]
    public string HQlblHCOP_COUNTRY_CODE_2
    {
        get { return _HQlblHCOP_COUNTRY_CODE_2; }
        set { _HQlblHCOP_COUNTRY_CODE_2 = value; }
    }

    /// <summary>
    /// 任職公司
    /// </summary>
    private string _HQlblHCOP_NP_COMPANY_NAME;
    [AttributeRFPrint("HQlblHCOP_NP_COMPANY_NAME", "B7")]
    public string HQlblHCOP_NP_COMPANY_NAME
    {
        get { return _HQlblHCOP_NP_COMPANY_NAME; }
        set { _HQlblHCOP_NP_COMPANY_NAME = value; }
    }
    /// <summary>
    /// 行業編號
    /// </summary>
    private string _HQlblHCOP_CC;
    [AttributeRFPrint("HQlblHCOP_CC", "E7")]
    public string HQlblHCOP_CC
    {
        get { return _HQlblHCOP_CC; }
        set { _HQlblHCOP_CC = value; }
    }
    /// <summary>
    /// 行業編號2
    /// </summary>
    private string _HQlblHCOP_CC_2;
    [AttributeRFPrint("HQlblHCOP_CC_2", "E8")]
    public string HQlblHCOP_CC_2
    {
        get { return _HQlblHCOP_CC_2; }
        set { _HQlblHCOP_CC_2 = value; }
    }
    /// <summary>
    /// 行業編號3
    /// </summary>
    private string _HQlblHCOP_CC_3;
    [AttributeRFPrint("HQlblHCOP_CC_3", "E9")]
    public string HQlblHCOP_CC_3
    {
        get { return _HQlblHCOP_CC_3; }
        set { _HQlblHCOP_CC_3 = value; }
    }
    /// <summary>
    /// 職業編號
    /// </summary>
    private string _HQlblHCOP_OC;
    [AttributeRFPrint("HQlblHCOP_OC", "H7")]
    public string HQlblHCOP_OC
    {
        get { return _HQlblHCOP_OC; }
        set { _HQlblHCOP_OC = value; }
    }
    /// <summary>
    /// 客戶收入及資產主要來源
    /// </summary>
    private string _HQlblHCOP_INCOME_SOURCE;
    [AttributeRFPrint("HQlblHCOP_INCOME_SOURCE", "B10")]
    public string HQlblHCOP_INCOME_SOURCE
    {
        get { return _HQlblHCOP_INCOME_SOURCE; }
        set { _HQlblHCOP_INCOME_SOURCE = value; }
    }
    /// <summary>
    /// 業務往來目的
    /// </summary>
    private string _CustLabel17;
    [AttributeRFPrint("CustLabel17", "B11")]
    public string CustLabel17
    {
        get { return _CustLabel17; }
        set { _CustLabel17 = value; }
    }
    /// <summary>
    /// 身分證換補領查詢結果
    /// </summary>
    private string _lbAML_HQ_Work_OWNER_ID_SreachStatus;
    [AttributeRFPrint("lbAML_HQ_Work_OWNER_ID_SreachStatus", "H11")]
    public string lbAML_HQ_Work_OWNER_ID_SreachStatus
    {
        get { return _lbAML_HQ_Work_OWNER_ID_SreachStatus; }
        set { _lbAML_HQ_Work_OWNER_ID_SreachStatus = value; }
    }
    /// <summary>
    /// 客戶「國籍」、「戶籍地」或「通訊地」或關聯人任一「國籍」位於高風險國家/地區
    /// </summary>
    private string _lblIsRisk;
    [AttributeRFPrint("lblIsRisk", "H13")]
    public string lblIsRisk
    {
        get { return _lblIsRisk; }
        set { _lblIsRisk = value; }
    }
    /// <summary>
    /// 「國籍」、「戶籍地」或「通訊地」位於一般或高度制裁國家/地區
    /// </summary>
    private string _lblIsSanction;
    [AttributeRFPrint("lblIsSanction", "H14")]
    public string lblIsSanction
    {
        get { return _lblIsSanction; }
        set { _lblIsSanction = value; }
    }
    /// <summary>
    /// 國外PEP
    /// </summary>
    private string _CDlblForeignPEPStakeholder;
    [AttributeRFPrint("CDlblForeignPEPStakeholder", "H15")]
    public string CDlblForeignPEPStakeholder
    {
        get { return _CDlblForeignPEPStakeholder; }
        set { _CDlblForeignPEPStakeholder = value; }
    }
    /// <summary>
    /// 負面新聞(NN)
    /// </summary>
    private string _CDlblNNListHitFlag;
    [AttributeRFPrint("CDlblNNListHitFlag", "H16")]
    public string CDlblNNListHitFlag
    {
        get { return _CDlblNNListHitFlag; }
        set { _CDlblNNListHitFlag = value; }
    }
    /// <summary>
    /// 客戶帳戶為警示或衍生帳戶
    /// </summary>
    private string _CDlblWarningFlag;
    [AttributeRFPrint("CDlblWarningFlag", "H17")]
    public string CDlblWarningFlag
    {
        get { return _CDlblWarningFlag; }
        set { _CDlblWarningFlag = value; }
    }
    /// <summary>
    /// 客戶信用卡有不良註記
    /// </summary>
    private string _CDlblCreditCardBlockCode;
    [AttributeRFPrint("CDlblCreditCardBlockCode", "H18")]
    public string CDlblCreditCardBlockCode
    {
        get { return _CDlblCreditCardBlockCode; }
        set { _CDlblCreditCardBlockCode = value; }
    }
    /// <summary>
    /// 集團關注名單
    /// </summary>
    private string _CDlblGroupInformationSharingNameListflag;
    [AttributeRFPrint("CDlblGroupInformationSharingNameListflag", "H19")]
    public string CDlblGroupInformationSharingNameListflag
    {
        get { return _CDlblGroupInformationSharingNameListflag; }
        set { _CDlblGroupInformationSharingNameListflag = value; }
    }
    /// <summary>
    /// 不合作/拒絕提供資訊
    /// </summary>
    private string _CDlblIncorporated;
    [AttributeRFPrint("CDlblIncorporated", "H20")]
    public string CDlblIncorporated
    {
        get { return _CDlblIncorporated; }
        set { _CDlblIncorporated = value; }
    }

}

[Serializable()]
public class clsMangger
{
    /// <summary>
    /// 經理人，分公司負責人用
    /// </summary>
    public clsMangger()
    {

    }
    //姓名
    private string _Name;
    public string Name
    {
        get { return _Name; }
        set { _Name = value; }
    }
    //國籍
    private string _Nation;
    public string Nation
    {
        get { return _Nation; }
        set { _Nation = value; }
    }
    //長姓名
    private string _Lname;
    public string Lname
    {
        get
        {
            return this._Lname;
        }
        set
        {
            this._Lname = value;
        }
    }
    //長ROMA
    private string _Romaname;
    public string Romaname
    {
        get
        {
            return this._Romaname;
        }
        set
        {
            this._Romaname = value;
        }
    }
}

[System.Serializable]
public class AttributeRFPrint : Attribute
{
    #region Constructed Function

    /// <summary>
    /// 指定位置，欄位型態
    /// </summary>
    /// <param name="CellRange"></param>
    /// <param name="CellType"></param>
    public AttributeRFPrint(string labelName, string CellRange, string CellType)
    {
        _labelName = labelName;
        _CellRange = CellRange;
        _CellType = CellType;
    }
    /// <summary>
    /// 只有位址,欄位型態預設string
    /// </summary>
    /// <param name="CellRange"></param>
    public AttributeRFPrint(string labelName, string CellRange)
    {
        _CellRange = CellRange;
        _labelName = labelName;


    }
    #endregion
    /// <summary>
    /// 所在的CELL位址 EX A2 ,A3
    /// </summary>
    private string _labelName;
    public string labelName
    {
        set { _labelName = value; }
        get { return _labelName; }
    }
    /// <summary>
    /// 所在的CELL位址 EX A2 ,A3
    /// </summary>
    private string _CellRange;
    public string CellRange
    {
        set { _CellRange = value; }
        get { return _CellRange; }
    }

    /// <summary>
    /// 欄位型態
    /// </summary>
    private string _CellType;
    public string CellType
    {
        set { _CellType = value; }
        get { return _CellType; }
    }
}