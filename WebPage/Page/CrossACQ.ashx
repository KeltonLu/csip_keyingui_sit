<%@ WebHandler Language="C#" Class="CrossACQ" %>

using System;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;
using System.IO;
using System.Data;
using CSIPCommonModel.EntityLayer;
using System.Web.SessionState;
using Framework.Common.Message;
using Framework.Common.Logging;
using System.Data.SqlClient;
using Framework.Data;
using Framework.Common.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
public class CrossACQ : IHttpHandler, IRequiresSessionState
{
    DataHelper dh = new DataHelper("Connection_System");
    DataHelper dh_CSIP = new DataHelper("Connection_CSIP");
    SqlCommand sqlcmd = new SqlCommand();
    DataSet ds = new DataSet();
    string hexKey = "";
    string zipKey = "";
    public void ProcessRequest(HttpContext context)
    {


        //加密用常數，規則為 來源 + 日期 + ACT + hexKey後計算MD5 與 checkSum欄位比對


        hexKey = ConfigurationManager.AppSettings["CrossHasKey"].ToString();
        zipKey = ConfigurationManager.AppSettings["CrossZipKey"].ToString();
        string Source = context.Request.Form["Source"];
        string sDate = context.Request.Form["sDate"];
        string SAct = context.Request.Form["ACT"];
        string CheckSum = context.Request.Form["CheckSum"];

        string CompKey = Source + sDate + SAct + hexKey;

        //驗證不符，回傳 成功則既續流程
        if (ENCSHA1(CompKey) != CheckSum)
        {
            ExportReault(context, "不合法的存取");
            context.Response.End();
            return;
        }

        string[] paramColl = SAct.Split('|');
        // Logging.SaveLog("取得伺服器路徑", ELogType.Info);
        Logging.Log("取得伺服器路徑");
        string exchangPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["exchangePath"].ToString());
        //Logging.SaveLog("取得伺服器路徑" + exchangPath, ELogType.Info);
        Logging.Log("取得伺服器路徑" + exchangPath);
        if (!Directory.Exists(exchangPath))
        {
            Directory.CreateDirectory(exchangPath);
        }
        else
        {
            //先執行HOUSTKEEPING
            DropFiles(exchangPath, "7");
        }
        if (!string.IsNullOrEmpty(paramColl[1]))
        {
            //傳過來是原檔名，不能直接存，所以要先把附檔改成ZIP

            string zipName = "fromACQ.zip"; // paramColl[1].ToString().Replace(Path.GetExtension(paramColl[1]).ToString(), ".zip");

            HttpPostedFile hpfUploadFile = context.Request.Files["FILE1"];
            string savePath = Path.Combine(exchangPath, zipName);
            //存在相同檔名，先刪除
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            hpfUploadFile.SaveAs(savePath);
            //Logging.SaveLog("存檔完成" + savePath, ELogType.Info);
            Logging.Log("存檔完成" + savePath);

            //由於傳過來是ZIP，需解壓縮
            CompressToZip.Unzip(exchangPath, savePath, zipKey);

            //解壓縮後刪除
            File.Delete(savePath);
        }

        switch (paramColl[0])
        {
            case "Import":
                ProcessImport(context, paramColl[1], paramColl[2], paramColl[3], exchangPath);
                break;


        }


    }
    /// <summary>
    /// 依傳入路徑，檔案建立日期，超過7天直接刪除
    /// </summary>
    /// <param name="expDay"></param>
    public void DropFiles(string sPath, string expDay)
    {
        int delDay = 7;
        int.TryParse(expDay, out delDay);
        string relDelDt = DateTime.Today.AddDays(delDay * -1).ToString("yyyyMMdd");
        long delDt = 0;
        long.TryParse(relDelDt, out delDt);
        string[] fireColl = Directory.GetFiles(sPath);
        foreach (string firName in fireColl)
        {
            FileInfo drInf = new FileInfo(firName);
            long drName = 0;
            long.TryParse(drInf.CreationTime.ToString("yyyyMMdd"), out drName);
            //已逾期，刪除
            if (drName < delDt)
            {
                drInf.Delete();
            }

        }

    }
    /// <summary>
    /// 輸出最終結果
    /// </summary>
    /// <param name="context"></param>
    /// <param name="Msg"></param>
    private void ExportReault(HttpContext context, string Msg)
    {
        context.Response.ContentType = "text/html";
        //string msgTmp = "<script type='text/javascript' language='JavaScript'>alert('{0}');window.oprner=null;window.close();</script>";
        //msgTmp = string.Format(msgTmp, Msg);
        //context.Response.Write(msgTmp);
        context.Response.Write(Msg);

    }
    /// <summary>
    /// 計算CHECKSUM
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private string ENCSHA1(string input)
    {
        SHA1 sha1 = new SHA1CryptoServiceProvider();
        string resultSha1 = Convert.ToBase64String(sha1.ComputeHash(Encoding.Default.GetBytes(input)), Base64FormattingOptions.InsertLineBreaks);
        return resultSha1;
    }
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
    /// <summary>
    /// 處理ACQ匯入
    /// </summary>
    /// <param name="context"></param>
    /// <param name="FileName"></param>
    /// <param name="BatchNo"></param>
    private void ProcessImport(HttpContext context, string FileName, string BatchNo, string AgentID, string exchangPath)
    {
        //無法跨系統抓取，改為參數傳入
        EntityAGENT_INFO eAgentInfo = new EntityAGENT_INFO();
        eAgentInfo.agent_id = AgentID;

        string importFile = Path.Combine(exchangPath, FileName);
        if (!File.Exists(importFile))
        {
            ExportReault(context, "交換檔案路徑不存在匯入檔名");
            return;
        }
        PageBase.structPageInfo sInfo = new PageBase.structPageInfo();
        sInfo.strPageCode = "CrossACQ";
        int total = 0;          //匯入檔總筆數
        int insertcount = 0;    //匯入筆數
        int updatecount = 0;    //更新筆數
        int insertfail = 0;     //匯入失敗筆數
        int updatefail = 0;     //更新失敗筆數
        string strMsgID = "";
        string MsgBody = @" 匯入檔名 : {0} " + "\r\n" + "處理結果 : {1}";
        string presult = "";
        string pHeader = "";
        ToACQRtn JsonObj = new ToACQRtn();

        try
        {
            if (BRArtificial_Signing_Batch_Data.ImportCross(importFile, eAgentInfo.agent_id, ref strMsgID,
                 ref total, ref insertcount, ref updatecount, ref insertfail, ref updatefail, ref JsonObj))
            {
                DataTable dtlog = CommonFunction.GetDataTable();
                string LogTxt = "匯入處理完成" + "總批號數：" + JsonObj.TotalBatCount.ToString() + "\r\n 總請款筆數：" + JsonObj.TotalapplyCount.ToString() + "\r\n 總請款金額:" + JsonObj.TotalapplyAMT.ToString();
                CommonFunction.UpdateLog("新增筆數：" + insertcount.ToString(), "更新筆數：" + updatecount.ToString(), "", dtlog);
                CommonFunction.InsertCustomerLog(dtlog, eAgentInfo, Path.GetFileNameWithoutExtension(importFile), "DB", sInfo);
                presult = LogTxt;
                pHeader = "匯入處理完成";
                MsgBody = string.Format(MsgBody, FileName, presult);
                // Logging.SaveLog("匯入完成 ", ELogType.Info);
                Logging.Log("匯入完成 ");
                SendMail(pHeader, MsgBody);
                //將匯入結果改以JSON字串回傳，帶回寫入LOG資料
                // 20200421 Ray Edit 因正式機不支援FW3.5 改用轉拋XML代替
                //string pJson = JsonConvert.SerializeObject(JsonObj);
                string pJson = XMLSerialize(JsonObj);
                // ExportReault(context, presult);
                ExportReault(context, pJson);
            }
            else
            {
                pHeader = "匯入異常";
                presult = "匯入異常 " + MessageHelper.GetMessage(strMsgID);
                MsgBody = string.Format(MsgBody, FileName, presult);
                // Logging.SaveLog("匯入異常 ", ELogType.Info);
                Logging.Log("匯入異常 ");
                SendMail(pHeader, MsgBody);
                //匯入異常
                // ExportReault(context, "匯入異常 " + MessageHelper.GetMessage(strMsgID));
                // 20200421 Ray Edit 因正式機不支援FW3.5 改用轉拋XML代替
                //string pJson = JsonConvert.SerializeObject(JsonObj);
                string pJson = XMLSerialize(JsonObj);
                // ExportReault(context, presult);
                ExportReault(context, pJson);
            }
        }
        catch (System.Exception ex)
        {
            // Logging.SaveLog(ex.Message, ELogType.Error);
            Logging.Log(ex);

        }
    }

    /// <summary>
    /// 功能說明:發送mail
    /// 作    者:Tank
    /// 創建時間:2018/01/16
    /// 修改記錄:
    /// </summary>
    /// <param name="MerchName"></param>
    /// <param name="strBodyMsg"></param>
    /// <returns></returns>
    private bool SendMail(string resu, string strBodyMsg)
    {
        string strnMsg = string.Empty;
        string Subject = string.Format("【ACQ 編批綜合報表 -外包明細檔】檔案轉入結果通知 {0}", resu);
        try
        {
            string strMailUsers = GetMailUser();
            if (!string.IsNullOrEmpty(strMailUsers))
            {
                MailService.MailSenderBasic(strMailUsers, "", Subject, strBodyMsg, null);
                strnMsg = Subject + "，Mail通知成功！";
                // Logging.SaveLog(strnMsg, ELogType.Info);
                Logging.Log(strnMsg);
                return true;
            }
            else
            {
                // Logging.SaveLog(strnMsg, ELogType.Error);
                Logging.Log(strnMsg, LogState.Error, LogLayer.BusinessRule);
                strnMsg = Subject + "，Mail通知失敗！ 未取得寄送清單";
                return false;
            }
        }
        catch
        {
            // Logging.SaveLog(strnMsg, ELogType.Error);
            Logging.Log(strnMsg, LogState.Error, LogLayer.BusinessRule);
            strnMsg = Subject + "，Mail通知失敗！";
            return false;
        }
    }
    private string GetMailUser()
    {
        string strMailUsers = string.Empty;
        ds = new DataSet();
        try
        {
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandTimeout = 30000;
            sqlcmd.CommandText = @"select property_name from M_PROPERTY_CODE where function_key='01' and property_key='P070203020102'  and off_flag='1' order by SEQUENCE ";

            ds = dh_CSIP.ExecuteDataSet(sqlcmd);

            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string[] mailuser = ds.Tables[0].Rows[i][0].ToString().Split(':');  //林鴻揚:tank.lin@ctbcbank.com
                    strMailUsers += "," + mailuser[1].ToString();
                }
            }
            //修正當抓不到清單時長度問題會報錯
            if (strMailUsers.Length > 1)
            {
                return strMailUsers.Substring(1);
            }
        }
        catch (System.Exception ex)
        {
            // Logging.SaveLog(ex.Message, ELogType.Error);
            Logging.Log(ex.Message, LogState.Error, LogLayer.BusinessRule);
            // throw ex;
        }
        return "";
    }
    /// <summary>
    /// 將ToACQRtn 轉成序列XML格式
    /// 2020.04.21 Ray Add
    /// </summary>
    /// <param name="o">Object</param>
    /// <returns></returns>
    private static string XMLSerialize(object o)
    {
        XmlSerializer ser = new XmlSerializer(o.GetType());
        StringBuilder sb = new StringBuilder();
        StringWriter writer = new StringWriter(sb);
        ser.Serialize(writer, o);
        return sb.ToString();
    }
}