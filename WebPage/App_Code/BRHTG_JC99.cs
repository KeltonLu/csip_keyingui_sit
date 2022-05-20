//******************************************************************
//*  功能說明：電文維護-JC99
//*  作    者：James
//*  創建日期：2019/07/22
//*  修改紀錄：
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*******************************************************************

using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.EntityLayer_new;
using Framework.Common.Logging;
using Framework.Common.Message;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Reflection;
using Framework.Common.Utility;

public class BRHTG_JC99 : IDisposable
{
    private string _PageCode;
    public string PageCode
    {
        get
        {
            return this._PageCode;
        }
        set
        {
            this._PageCode = value;
        }
    }

    public BRHTG_JC99()
    {
    }
    public BRHTG_JC99(string pageCode)
    {
        this._PageCode = pageCode;
    }

    private void writeLog(string msg)
    {
        if (string.IsNullOrEmpty(msg) == false)
        {
            msg = string.Format("PageCode:{0}=>{1}\r\n", _PageCode, msg);
            Logging.Log(msg, LogLayer.BusinessRule);
        }
    }

    public string getHGTValue(Hashtable hgt, string key)
    {
        string result = string.Empty;

        if (hgt.ContainsKey(key))
        {
            result = hgt[key].ToString();
        }

        return result;
    }

    public T ConvertToClass<T>(Hashtable hgt) where T : new()
    {
        T result = new T();

        Type v = result.GetType();  //取的型別實體
        PropertyInfo[] props = v.GetProperties(); //取出所有公開屬性(可以被外部存取得 
        string propName = string.Empty;
        string propVal = string.Empty;
        foreach (PropertyInfo prop in props)
        {
            propName = prop.Name; //屬性名稱，用來對應
            propVal = getHGTValue(hgt, propName);
            switch (prop.PropertyType.Name)
            {
                case "Int32":
                    prop.SetValue(result, int.Parse(propVal), null);
                    break;

                case "String":
                    prop.SetValue(result, propVal, null);
                    break;
            }
        }
        return result;
    }

    /// <summary>
    /// 模擬測試用電文回傳 : 需再注意
    /// </summary>
    /// <returns></returns>
    public Hashtable GetTestMainFrameInfo(string TranID)
    {
        //讀取D:\JC66_Download_TEST.txt做成HSAHTABLE
        string _fileName = string.Format(@"d:\{0}_Download_TEST.txt", TranID);
        string[] lincoll = System.IO.File.ReadAllLines(_fileName, System.Text.Encoding.Default);
        Hashtable _result = new Hashtable();
        foreach (string oitem in lincoll)
        {
            string[] tmpColl = oitem.Split(new string[] { "<!>" }, StringSplitOptions.None);
            if (tmpColl.Length == 2)
            {
                _result.Add(tmpColl[0], tmpColl[1]);
            }
        }
        return _result;
    }

    /// <summary>
    /// 判斷電文指定欄位是否為長姓名
    /// </summary>
    /// <param name="hgt"></param>
    /// <returns></returns>
    public bool getIsLongNameFlag(Hashtable hgt)
    {
        bool _result = false;
        string _htgKey = "NAME_1_2";
        if (hgt.ContainsKey(_htgKey) == true)
        {
            string _name_1_2 = hgt[_htgKey].ToString().Trim();
            _result = _name_1_2.Length > 0 ? (_name_1_2.Substring(_name_1_2.Length - 1, 1).ToUpper() == ("Y") ? true : false) : false;
        }
        return _result;
    }

    /// <summary>
    /// 依原電文值和輸入值(中文長姓名,羅馬拼音) 決定 Function_Code
    /// </summary>
    /// <param name="hgt"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public string getFunctionCode(Hashtable hgt, EntityHTG_JC99 obj)
    {
        string result = string.Empty;

        //只要原電文值的長姓名flag = true ,其function_code 就是 C,若不是且(中文長姓名長度大於5 or 羅馬拼音非空白時) 則是 A
        if (this.getIsLongNameFlag(hgt) == true)
        {
            result= "C";
        }
        else
        {
            //中文長姓名長度大於5 或 羅馬拼音非空白時 為 A
            if (obj.NAME.Length > 5 || string.IsNullOrEmpty(obj.ROMA) == false)
            {
                result = "A";
            }
        }
        return result;
    }

    /// <summary>
    /// 指定系統代碼
    /// </summary>
    /// <param name="htInput"></param>
    /// <param name="eAgentInfo"></param>
    private void setSystemCode(ref Hashtable htInput,EntityAGENT_INFO eAgentInfo)
    {
        string _systemCode = string.Format("CSIP{0}", "000");
        htInput.Add("IN_CHANNEL", _systemCode);        
    }

    /// <summary>
    /// 取得長姓名電文資料
    /// </summary>
    /// <returns></returns>
    public EntityHTG_JC99 getData(EntityHTG_JC99 data, EntityAGENT_INFO eAgentInfo)
    {
        EntityHTG_JC99 _result = new EntityHTG_JC99();
        Hashtable htReturnJC99 = new Hashtable();
        bool isTest = (UtilHelper.GetAppSettings("isTest") == "Y");
        if (isTest)
        {
            //測試用模擬資料                        
            htReturnJC99 = GetTestMainFrameInfo("JC99"); //模擬取得電文
        }
        else
        {
            string strType = "12";//*查詢電文環境

            //*添加上傳主機信息
            Hashtable htInput = new Hashtable();
            this.setSystemCode(ref htInput, eAgentInfo);
            htInput.Add("IDNO_NO", data.IDNO_NO);
            htInput.Add("LINE_CNT", "0000");            
            htInput.Add("FUNCTION_CODE", "I");
            

            //上送主機資料           
            htReturnJC99 = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JC99, htInput, false, strType, eAgentInfo);
        }

        _result = this.ConvertToClass<EntityHTG_JC99>(htReturnJC99);
        this.writeLog("JC99 GetData()");
        return _result;
    }

    /// <summary>
    /// 更新長姓名資料
    /// </summary>
    public EntityResult Update(EntityHTG_JC99 data, EntityAGENT_INFO eAgentInfo)
    {
        //*添加上傳主機信息
        Hashtable htInput = new Hashtable();
        this.setSystemCode(ref htInput, eAgentInfo);
        htInput.Add("FUNCTION_CODE", data.FUNCTION_CODE); //I:查詢, C:異動, A:新增
        htInput.Add("IDNO_NO", data.IDNO_NO);
        htInput.Add("IN_CFLAG", data.IN_CFLAG);
        htInput.Add("NAME", data.NAME);
        htInput.Add("ROMA", data.ROMA);
        string _type = "1"; //1是指什麼意思?
        //*得到主機傳回信息
        Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4_JC99, htInput, false, _type, eAgentInfo);
        EntityResult _result = new EntityResult();

        if (!htReturn.Contains("HtgMsg"))
        {
            htReturn["ID"] = htInput["IDNO_NO"];//* for_xml_test
            htReturn["MESSAGE_TYPE"] = "";
            
            _result.ClientMsg += HtgType.P4_JC99.ToString() + MessageHelper.GetMessage("01_00000000_014");
            _result.Success = true;          
        }
        else
        {
            _result.ClientMsg += htReturn["HtgMsg"].ToString();
            _result.HostMsg = htReturn["HtgMsg"].ToString();
            _result.Success = false;
        }

        if (_result.Success == false)
        {
            this.writeLog(_result.ClientMsg);
        }
        return _result;
    }

    //測試
    public T ConvertToClass<T>(T obj, Hashtable hgt)
    {
        T result = obj;

        Type v = result.GetType();  //取的型別實體
        PropertyInfo[] props = v.GetProperties(); //取出所有公開屬性(可以被外部存取得 
        string propName = string.Empty;
        string propVal = string.Empty;
        foreach (PropertyInfo prop in props)
        {
            propName = prop.Name; //屬性名稱，用來對應
            propVal = this.getHGTValue(hgt, propName);
            switch (prop.PropertyType.Name)
            {
                case "Int32":
                    prop.SetValue(result, int.Parse(propVal), null);
                    break;

                case "String":
                    prop.SetValue(result, propVal, null);
                    break;
            }
        }
        return result;
    }

    public void Dispose()
    {

    }
}
