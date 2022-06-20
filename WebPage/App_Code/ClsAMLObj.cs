using System;
using System.Collections.Generic;
using System.Web;
using Framework.Data.OM.OMAttribute;

#region 查詢 AML_HQQuery 物件
/// <summary>
/// 查詢用Class物件
/// </summary>
public class AML_HQQuery
{
    public AML_HQQuery() { }
    /// <summary>
    /// 案件類型
    /// </summary>
    private string _CaseType;
    public string CaseType
    {
        get { return _CaseType; }
        set { _CaseType = value; }
    }
    /// <summary>
    /// 案件編號
    /// </summary>
    private string _CaseNo;
    public string CaseNo
    {
        get { return _CaseNo; }
        set { _CaseNo = value; }
    }
    /// <summary>
    /// 統編
    /// </summary>
    private string _TaxNo;
    public string TaxNo
    {
        get { return _TaxNo; }
        set { _TaxNo = value; }
    }
    /// <summary>
    /// 建案年 開始
    /// </summary>
    private string _yearS;
    public string yearS
    {
        get { return _yearS; }
        set { _yearS = value; }
    }
    /// <summary>
    /// 建案年 結束
    /// </summary>
    private string _yearE;
    public string yearE
    {
        get { return _yearE; }
        set { _yearE = value; }
    }
    /// <summary>
    /// 建案月 開始
    /// </summary>
    private string _MonthS;
    public string MonthS
    {
        get { return _MonthS; }
        set { _MonthS = value; }
    }
    /// <summary>
    /// 建案月 結束
    /// </summary>
    private string _MonthE;
    public string MonthE
    {
        get { return _MonthE; }
        set { _MonthE = value; }
    }
    /// <summary>
    /// 風險值
    /// </summary>
    private string _RiskRanking;
    public string RiskRanking
    {
        get { return _RiskRanking; }
        set { _RiskRanking = value; }
    }
    /// <summary>
    /// 處理經辦
    /// </summary>
    private string _Owner_User;
    public string Owner_User
    {
        get { return _Owner_User; }
        set { _Owner_User = value; }
    }
    /// <summary>
    /// 排序
    /// </summary>
    private string _OrderBy;
    public string OrderBy
    {
        get { return _OrderBy; }
        set { _OrderBy = value; }
    }

    /// <summary>
    /// 排序 --升降冪
    /// </summary>
    private string _OrderASC;
    public string OrderASC
    {
        get { return _OrderASC; }
        set { _OrderASC = value; }
    }

    private string _UserRoll;
    public string UserRoll
    {
        get { return _UserRoll; }
        set { _UserRoll = value; }
    }

    //20191115-RQ-2018-015749-002-新增查詢條件
    /// <summary>
    /// 不合作註記
    /// </summary>
    private string _IncorporatedFlag;
    public string IncorporatedFlag
    {
        get { return _IncorporatedFlag; }
        set { _IncorporatedFlag = value; }
    }
    /// <summary>
    /// 結案原因
    /// </summary>
    private string _CloseType;
    public string CloseType
    {
        get { return _CloseType; }
        set { _CloseType = value; }
    }
    /// <summary>
    /// 結案日期(起)
    /// </summary>
    private string _CloseDateS;
    public string CloseDateS
    {
        get { return _CloseDateS; }
        set { _CloseDateS = value; }
    }
    /// <summary>
    /// 結案日期(迄)
    /// </summary>
    private string _CloseDateE;
    public string CloseDateE
    {
        get { return _CloseDateE; }
        set { _CloseDateE = value; }
    }

    // 20220613 新增呈核狀態查詢絛件 By Kelton
    /// <summary>
    /// 呈核狀態
    /// </summary>
    private string _Status;
    public string Status
    {
        get { return _Status; }
        set { _Status = value; }
    }
}
#endregion

#region 資料傳遞用AML_HQ_Query物件
/// <summary>
///  資料傳遞用AML_HQ_Query物件
/// </summary>
[Serializable]
public class AML_SessionState 
{
    /// <summary>
    /// 流水號
    /// </summary>
    public string _ID;
    public string ID
    {
        get { return _ID; }
        set { _ID = value; }
    }
    /// <summary>
    /// 分公司用流水號
    /// </summary>
    public string _BRCHID;
    public string BRCHID
    {
        get { return _BRCHID; }
        set { _BRCHID = value; }
    }
    /// <summary>
    /// 批號
    /// </summary>
    public string _RMMBatchNo; 
    public string RMMBatchNo
    {
        get { return _RMMBatchNo; }
        set { _RMMBatchNo = value; }
    }
    /// <summary>
    /// 序號
    /// </summary>
    private string _AMLInternalID;
    public string AMLInternalID
    {
        get { return _AMLInternalID; }
        set { _AMLInternalID = value; }
    }
    /// <summary>
    /// 案號
    /// </summary>
    private string _CASE_NO;
    [AttributeRfPage("hlblCaseNo", "CustLabel", false)]
    public string CASE_NO
    {
        get { return _CASE_NO; }
        set { _CASE_NO = value; }
    } /// <summary>
      /// 建案日期
      /// </summary>
    private string _DataDate;
    [AttributeRfPage("hlblDataDate", "CustLabel", false)]
    public string DataDate
    {
        get { return _DataDate; }
        set { _DataDate = value; }
    }
    /// <summary>
    /// 統編
    /// </summary>
    private string _HCOP_HEADQUATERS_CORP_NO;
    [AttributeRfPage("hlblHCOP_HEADQUATERS_CORP_NO", "CustLabel", false)]
    public string HCOP_HEADQUATERS_CORP_NO
    {
        get { return _HCOP_HEADQUATERS_CORP_NO; }
        set { _HCOP_HEADQUATERS_CORP_NO = value; }
    }
    /// <summary>
    /// 原本的風險等級
    /// </summary>
    private string _OriginalRiskRanking;
    [AttributeRfPage("hlblOriginalRiskRanking", "CustLabel", false)]
    public string OriginalRiskRanking
    {
        get { return _OriginalRiskRanking; }
        set { _OriginalRiskRanking = value; }
    }
    /// <summary>
    /// 案件類型
    /// </summary>
    private string _CaseType;
    [AttributeRfPage("hlblCaseType", "CustLabel", false)]
    public string CaseType
    {
        get { return _CaseType; }
        set { _CaseType = value; }
    }
    /// <summary>
    /// 審查到期日
    /// </summary>
    private string _CaseExpiryDate;
    [AttributeRfPage("hlblCaseExpiryDate", "CustLabel", false)]
    public string CaseExpiryDate
    {
        get { return _CaseExpiryDate; }
        set { _CaseExpiryDate = value; }
    }
    /// <summary>
    /// 最新試算後的下次審查日期
    /// </summary>
    private string _NewNextReviewDate;
    [AttributeRfPage("hlblNewNextReviewDate", "CustLabel", false)]
    public string NewNextReviewDate
    {
        get { return _NewNextReviewDate; }
        set { _NewNextReviewDate = value; }
    }
    /// <summary>
    /// 註冊名稱
    /// </summary>
    private string _HCOP_REG_NAME;
    [AttributeRfPage("hlblHCOP_REG_NAME", "CustLabel", false)]
    public string HCOP_REG_NAME
    {
        get { return _HCOP_REG_NAME; }
        set { _HCOP_REG_NAME = value; }
    }

    /// <summary>
    /// 案件階段，注意此值須跟隨異動調整，送審，退件需變更
    /// </summary>
    private string _CaseProcess_User; 
    public string CaseProcess_User
    {
        get { return _CaseProcess_User; }
        set { _CaseProcess_User = value; }
    }
    /// <summary>
    /// 經辦人員，注意此值須跟隨異動調整，送審，退件需變更
    /// </summary>
    private string _CaseOwner_User;
    public string CaseOwner_User
    {
        get { return _CaseOwner_User; }
        set { _CaseOwner_User = value; }
    }
    /// <summary>
    /// 處理狀態 注意此值須跟隨異動調整，送審，退件需變更
    /// </summary>
    private string _CaseProcess_Status;
    public string CaseProcess_Status
    {
        get { return _CaseProcess_Status; }
        set { _CaseProcess_Status = value; }
    }

    //20200227-RQ-2019-030155-003
    /// <summary>
    /// 最新試算後的風險等級
    /// </summary>
    private string _NewRiskRanking;
    [AttributeRfPage("hlblNewRiskRanking", "CustLabel", false)]
    public string NewRiskRanking
    {
        get { return _NewRiskRanking; }
        set { _NewRiskRanking = value; }
    }

    /// <summary>
    /// 原本的下次審查日期
    /// </summary>
    private string _OriginalNextReviewDate;
    [AttributeRfPage("hlblOriginalNextReviewDate", "CustLabel", false)]
    public string OriginalNextReviewDate
    {
        get { return _OriginalNextReviewDate; }
        set { _OriginalNextReviewDate = value; }
    }

    //20200508-RQ-2019-030155-005
    /// <summary>
    /// 拋查NameCheck所需英文登記名稱
    /// </summary>
    private string _HCOP_CORP_REG_ENG_NAME;
    public string HCOP_CORP_REG_ENG_NAME
    {
        get { return _HCOP_CORP_REG_ENG_NAME; }
        set { _HCOP_CORP_REG_ENG_NAME = value; }
    }
}

#endregion