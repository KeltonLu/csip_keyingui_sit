//******************************************************************
//*  功能說明：電文維護-JC68(長姓名)
//*  作    者：James
//*  創建日期：2019/08/17
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
using System.Reflection;
using Framework.Common.Utility;

public class BRHTG_JC68 : IDisposable
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

    public BRHTG_JC68()
    {
    }
    public BRHTG_JC68(string pageCode)
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
    /// 依 id 查詢(中文長姓名,羅馬拼音) 決定 Function_Code
    /// </summary>
    /// <param name="data"></param>
    /// <param name="eAgentInfo"></param>
    /// <returns></returns>
    public string getFunctionCode(EntityHTG_JC68 data, EntityAGENT_INFO eAgentInfo)
    {
        EntityHTG_JC68 _HtgResult = getData(data, eAgentInfo, "12");
        string result = string.Empty;

        //判斷回傳的MESSAGE_TYPE 是否為 0000, 是就給C, 不是就一律給A
        //result = (_HtgResult.MESSAGE_TYPE == "0000") ? "C" : "A";
        //return result;

        //20190910 修改：如message_type非0000、0001、0006 即回傳錯訊息至前端 by Peggy
        switch (_HtgResult.MESSAGE_TYPE.Trim())
        {
            case "0000":
            case "0001":
                return result = "C";
            case "0006":
                return result = "A";
            default:
                return result = MainFrameInfo.GetMessageType(HtgType.P4A_JC68, _HtgResult.MESSAGE_TYPE);
        }
    }

    /// <summary>
    /// 指定系統代碼
    /// </summary>
    /// <param name="htInput"></param>
    /// <param name="eAgentInfo"></param>
    private void setSystemCode(ref Hashtable htInput, EntityAGENT_INFO eAgentInfo)
    {
        //JC68 目前暫沒限制
        //string _systemCode = string.Format("CSIP{0}", "000");
        //htInput.Add("IN_CHANNEL", _systemCode);
    }

    /// <summary>
    /// 取得長姓名電文資料
    /// </summary>
    /// <returns></returns>
    public EntityHTG_JC68 getData(EntityHTG_JC68 data, EntityAGENT_INFO eAgentInfo)
    {
        return this.getData(data, eAgentInfo, "12");
    }

    /// <summary>
    /// 取得長姓名電文資料
    /// 修改 by Peggy
    /// </summary>
    /// <returns></returns>
    public EntityHTG_JC68 getData(EntityHTG_JC68 data, EntityAGENT_INFO eAgentInfo, string _type)
    {
        EntityHTG_JC68 _result = new EntityHTG_JC68();
        Hashtable htReturn = new Hashtable();
        bool isTest = (UtilHelper.GetAppSettings("isTest") == "Y");
        if (isTest)
        {
            //測試用模擬資料                        
            htReturn = GetTestMainFrameInfo("JC68"); //模擬取得電文
        }
        else
        {
            //string strType = "12";//*查詢電文環境

            //*添加上傳主機信息
            Hashtable htInput = new Hashtable();
            this.setSystemCode(ref htInput, eAgentInfo);
            htInput.Add("ID", data.ID);//查詢KEY值
            htInput.Add("LINE_CNT", "0000");
            htInput.Add("FUNCTION_CODE", "I");//I:查詢


            //上送主機資料           
            htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC68, htInput, false, _type, eAgentInfo);
        }

        _result = this.ConvertToClass<EntityHTG_JC68>(htReturn);
        string[] _successTypes = "0000,0001,0006".Split(',');
        if(Array.IndexOf(_successTypes, _result.MESSAGE_TYPE) > -1)
        {
            _result.Success = true;
        }
            
        //2019009 修改
        this.writeLog("JC68 GetData()" + htReturn["MESSAGE_CHI"].ToString().Trim());//新增主機回傳訊息顯示        
        return _result;
    }

    /// <summary>
    /// 更新長姓名資料
    /// </summary>
    public EntityResult Update(EntityHTG_JC68 data, EntityAGENT_INFO eAgentInfo , string _type)
    {
        //*添加上傳主機信息
        Hashtable htInput = new Hashtable();
        EntityResult _result = new EntityResult();
        this.setSystemCode(ref htInput, eAgentInfo);
        string _FunctionCode = getFunctionCode(data, eAgentInfo);

        if (_FunctionCode.Trim().Equals("C") || _FunctionCode.Trim().Equals("A"))
        {
            htInput.Add("FUNCTION_CODE", _FunctionCode); //I:查詢, C:異動, A:新增
            htInput.Add("ID", data.ID);
            htInput.Add("LONG_NAME", data.LONG_NAME);
            htInput.Add("PINYIN_NAME", data.PINYIN_NAME);
            //string _type = "12"; //1是指什麼意思?
            //*得到主機傳回信息
            Hashtable htReturn = MainFrameInfo.GetMainFrameInfo(HtgType.P4A_JC68, htInput, false, _type, eAgentInfo);

            if (!htReturn.Contains("HtgMsg"))
            {
                htReturn["ID"] = htInput["ID"];//* for_xml_test
                htReturn["MESSAGE_TYPE"] = "";

                _result.ClientMsg += "P4A_JC68 :" + MessageHelper.GetMessage("01_00000000_014");
                _result.Success = true;
            }
            else
            {
                _result.ClientMsg += htReturn["HtgMsg"].ToString();
                _result.HostMsg = htReturn["HtgMsg"].ToString();
                _result.Success = false;
            }
        }
        else
        {
            _result.Success = false;
            _result.ClientMsg += _FunctionCode.Trim();
            _result.HostMsg = _FunctionCode.Trim();
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