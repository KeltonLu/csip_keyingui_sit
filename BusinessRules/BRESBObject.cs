using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Framework.Common.Logging;

/// <summary>
/// ESBObject 的摘要描述
/// </summary>
public class ESBObject
{
    public ESBObject()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }

    public string ConnStatus { get; set; }
    public string StatusCode { get; set; }
    public string RspCode { get; set; }
    public string RspMsg { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public string ResultAlertMessage { get; set; }

    public virtual string getXML()
    {
        return "";
    }

    public virtual bool CheckResult(string strResult)
    {
        return false;
    }

    public virtual void getResult(string strResult)
    {

    }

    public virtual string getStatusCodeTag()
    {
        return "";
    }
    public virtual string getRspCodeTag()
    {
        return "";
    }
    public virtual string getErrorCodeTag()
    {
        return "";
    }
    public virtual string getErrorMessageTag()
    {
        return "";
    }

    public static string getTagValue(string xmlSource, string tagName)
    {
        string tagValue = string.Empty;
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlSource);
            if (doc.GetElementsByTagName(tagName).Count > 0)
            {
                tagValue = doc.GetElementsByTagName(tagName)[0].InnerText;
            }
            return tagValue;
        }
        catch (Exception ex)
        {
            Logging.Log("無法取得代碼", LogState.Info, LogLayer.UI);
            return tagValue;
        }
    }
}