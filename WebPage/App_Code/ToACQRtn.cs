using System;
using System.Collections.Generic;
using System.Web;
using System.Xml.Serialization;
/// <summary>
/// ToACQRtn 的摘要描述 回傳給ACQ，寫入DB及更新用物件
/// 2020.04.21 Ray Edit 增加XmlElement XmlRoot 節點
/// </summary>
public class ToACQRtn
{
    public ToACQRtn()
    {

        _metDbColl = new List<MetdaDb>();
    }


    private string _rtnMsg;
    /// <summary>
    /// 帶回訊息
    /// </summary>
    public string rtnMsg
    {
        get
        {
            return this._rtnMsg;
        }
        set
        {
            this._rtnMsg = value;
        }
    }

    private int _TotalBatCount;
    /// <summary>
    /// TotalBatCount
    /// </summary>
    //[AttributeRfPage("txtTotalBatCount", "CustTextBox", false)]
    public int TotalBatCount
    {
        get
        {
            return this._TotalBatCount;
        }
        set
        {
            this._TotalBatCount = value;
        }
    }
    private int _TotalapplyCount;
    /// <summary>
    /// TotalapplyCount
    /// </summary>
    //[AttributeRfPage("txtTotalapplyCount", "CustTextBox", false)]
    public int TotalapplyCount
    {
        get
        {
            return this._TotalapplyCount;
        }
        set
        {
            this._TotalapplyCount = value;
        }
    }
    private decimal _TotalapplyAMT;
    /// <summary>
    /// TotalapplyAMT
    /// </summary>
    //[AttributeRfPage("txtTotalapplyAMT", "CustTextBox", false)]
    public decimal TotalapplyAMT
    {
        get
        {
            return this._TotalapplyAMT;
        }
        set
        {
            this._TotalapplyAMT = value;
        }
    }
    List<MetdaDb> _metDbColl;
    [XmlElement("MetDbColl")]
    public List<MetdaDb> MetDbColl
    {
        get
        {
            return _metDbColl;
        }
        set
        {
            this._metDbColl = value;
        }
    }

    public void ADD(MetdaDb tmp)
    {
        _metDbColl.Add(tmp);
    }
}
[XmlRoot("MetDbColl")]
public class MetdaDb
{
    /// <summary>
    /// 建構式，建立物件時將[編批日期]及[收件批次]簽單類別固定帶入
    /// </summary>
    public MetdaDb(string inBatch_Date,string inReceive_Batch,string inSign_Type,string inpage,string inBatch_NO,string inShop_ID
        ,string inReceive_Total_Count,string inReceive_Total_AMT,string inuserid)
    {
        _batch_date = inBatch_Date;
        _Receive_Batch = inReceive_Batch;
        _Sign_Type = inSign_Type;
        _Page = inpage;
        _Batch_NO = inBatch_NO;
        _Shop_ID = inShop_ID;
        _Receive_Total_Count = inReceive_Total_Count;
        _Receive_Total_AMT = inReceive_Total_AMT;
        _Create_User = inuserid;
        _Modify_User = inuserid;
    }
    public MetdaDb() { }
    private Guid _guid;
    /// <summary>
    /// guid
    /// </summary>
    //[AttributeRfPage("txtguid", "CustTextBox", false)]
    public Guid guid
    {
        get
        {
            return this._guid;
        }
        set
        {
            this._guid = value;
        }
    }
    private string _batch_date;
    /// <summary>
    /// batch_date
    /// </summary>
    //[AttributeRfPage("txtbatch_date", "CustTextBox", false)]
    public string batch_date
    {
        get
        {
            return this._batch_date;
        }
        set
        {
            this._batch_date = value;
        }
    }
    private string _Receive_Batch;
    /// <summary>
    /// Receive_Batch
    /// </summary>
    //[AttributeRfPage("txtReceive_Batch", "CustTextBox", false)]
    public string Receive_Batch
    {
        get
        {
            return this._Receive_Batch;
        }
        set
        {
            this._Receive_Batch = value;
        }
    }
    private string _Page;
    /// <summary>
    /// Page
    /// </summary>
    //[AttributeRfPage("txtPage", "CustTextBox", false)]
    public string Page
    {
        get
        {
            return this._Page;
        }
        set
        {
            this._Page = value;
        }
    }
    private string _Batch_NO;
    /// <summary>
    /// Batch_NO
    /// </summary>
    //[AttributeRfPage("txtBatch_NO", "CustTextBox", false)]
    public string Batch_NO
    {
        get
        {
            return this._Batch_NO;
        }
        set
        {
            this._Batch_NO = value;
        }
    }
    private string _Shop_ID;
    /// <summary>
    /// Shop_ID
    /// </summary>
    //[AttributeRfPage("txtShop_ID", "CustTextBox", false)]
    public string Shop_ID
    {
        get
        {
            return this._Shop_ID;
        }
        set
        {
            this._Shop_ID = value;
        }
    }
    private string _Sign_Type;
    /// <summary>
    /// Sign_Type
    /// </summary>
    //[AttributeRfPage("txtSign_Type", "CustTextBox", false)]
    public string Sign_Type
    {
        get
        {
            return this._Sign_Type;
        }
        set
        {
            this._Sign_Type = value;
        }
    }
    private string _Receive_Total_Count;
    /// <summary>
    /// Receive_Total_Count
    /// </summary>
    //[AttributeRfPage("txtReceive_Total_Count", "CustTextBox", false)]
    public string Receive_Total_Count
    {
        get
        {
            return this._Receive_Total_Count;
        }
        set
        {
            this._Receive_Total_Count = value;
        }
    }
    private string _Receive_Total_AMT;
    /// <summary>
    /// Receive_Total_AMT
    /// </summary>
    //[AttributeRfPage("txtReceive_Total_AMT", "CustTextBox", false)]
    public string Receive_Total_AMT
    {
        get
        {
            return this._Receive_Total_AMT;
        }
        set
        {
            this._Receive_Total_AMT = value;
        }
    }
    private string _Process_Flag;
    /// <summary>
    /// Process_Flag
    /// </summary>
    //[AttributeRfPage("txtProcess_Flag", "CustTextBox", false)]
    public string Process_Flag
    {
        get
        {
            return this._Process_Flag;
        }
        set
        {
            this._Process_Flag = value;
        }
    }
    private string _Create_User;
    /// <summary>
    /// Create_User
    /// </summary>
    //[AttributeRfPage("txtCreate_User", "CustTextBox", false)]
    public string Create_User
    {
        get
        {
            return this._Create_User;
        }
        set
        {
            this._Create_User = value;
        }
    }
    private string _Create_DateTime;
    /// <summary>
    /// Create_DateTime
    /// </summary>
    //[AttributeRfPage("txtCreate_DateTime", "CustTextBox", false)]
    public string Create_DateTime
    {
        get
        {
            return this._Create_DateTime;
        }
        set
        {
            this._Create_DateTime = value;
        }
    }
    private string _Modify_User;
    /// <summary>
    /// Modify_User
    /// </summary>
    //[AttributeRfPage("txtModify_User", "CustTextBox", false)]
    public string Modify_User
    {
        get
        {
            return this._Modify_User;
        }
        set
        {
            this._Modify_User = value;
        }
    }
    private string _Modify_DateTime;
    /// <summary>
    /// Modify_DateTime
    /// </summary>
    //[AttributeRfPage("txtModify_DateTime", "CustTextBox", false)]
    public string Modify_DateTime
    {
        get
        {
            return this._Modify_DateTime;
        }
        set
        {
            this._Modify_DateTime = value;
        }
    }

}
