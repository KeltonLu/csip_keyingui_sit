//******************************************************************
//* 作    者：
//* 功能說明：
//* 創建日期：
//* 修改紀錄：
//* <author>            <time>            <TaskID>                <desc>
//* Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//* Ares Luke          2020/04/13         20200031-CSIP EOS       新增LOG資訊與移除未使用using
//*******************************************************************

using System;
using System.Data;
using Quartz;
using Framework.Common.Logging;
using CSIPKeyInGUI.BusinessRules;
using CSIPCommonModel.BusinessRules;
using Framework.Common.IO;
using System.IO;
using Framework.Common.Message;
using Framework.Common.Utility;

/// <summary>
/// BatchJob_UPLOAD 的摘要描述
/// </summary>
class BatchJob_UPLOAD : Quartz.IJob
{
    protected string FunctionKey = UtilHelper.GetAppSettings("FunctionKey").ToString();
    protected JobHelper JobHelper = new JobHelper();


    //add by linhuanhuang 自動 JOB MAIL 發送機制 20100623 start
    private string strMail;
    private string strTitle;
    private string strJobID;
    //add by linhuanhuang 自動 JOB MAIL 發送機制 20100623 end

    #region IJob 成員

    public void Execute(JobExecutionContext context)
    {
        DateTime dateStart = DateTime.Now;//*批次開始執行時間
        string strLocalPath = "";
        //add by linhuanhuang 自動 JOB MAIL 發送機制 20100623 start
        string strMailMsg = "";
        string strStatus = "";
        //add by linhuanhuang 自動 JOB MAIL 發送機制 20100623 end


        try
        {
            //add by linhuanhuang 自動 JOB MAIL 發送機制 20100623 start
            JobDataMap jobDataMap = context.JobDetail.JobDataMap;
            strMail = jobDataMap.GetString("mail").Trim();
            strTitle = jobDataMap.GetString("title").Trim();
            strJobID = jobDataMap.GetString("jobid").Trim();
            //add by linhuanhuang 自動 JOB MAIL 發送機制 20100623 end
            JobHelper.strJobID = strJobID;


            JobHelper.SaveLog("*********** Redeem 點數折抵參數上傳 START ************** ", LogState.Info);

            JobHelper.SaveLog("建檔開始", LogState.Info);
            strLocalPath = CreateTxt();
            JobHelper.SaveLog("建檔結束", LogState.Info);

            //edit by linhuanhuang 自動 JOB MAIL 發送機制 20100623 start
            if ("" != strLocalPath)
            {
                //FTPFactory ftpF = new FTPFactory("RU");


                //if (ftpF.Upload(Path.GetFileName(strLocalPath)))
                //{

                JobHelper.SaveLog("更新RedeemUpload狀態(SEND3270 = 'Y')", LogState.Info);
                BRRedeemUpload.UpdateSendData();


                strMailMsg = "傳輸成功";
                strStatus = "S";
                JobHelper.SaveLog(strMailMsg, LogState.Info);
                //}
                //else
                //{
                //    strMailMsg = "傳輸失敗";
                //    strStatus = "F";
                //}


            }
            else
            {
                strMailMsg = "無比對完成的資料";
                strStatus = "N";
                JobHelper.SaveLog(strMailMsg, LogState.Error);
            }
            //edit by linhuanhuang 自動 JOB MAIL 發送機制 20100623 end

        }
        catch (Exception ex)
        {
            Logging.Log(ex, strJobID);
            strMailMsg = "傳輸失敗";
            JobHelper.SaveLog(strMailMsg,LogState.Error);
        }
        finally
        {
            //*插入L_BATCH_LOG資料庫
            BRL_BATCH_LOG.Insert("01", strJobID, dateStart, strStatus, strMailMsg);
            if ("無比對完成的資料" != strMailMsg)
            {
                JobHelper.SaveLog("執行寄信", LogState.Info);
                SendMail(dateStart, strMailMsg);
            }


            JobHelper.SaveLog("*********** Redeem 點數折抵參數上傳 END ************** ", LogState.Info);
        }
    }

    private string CreateTxt()
    {
        string strLocalPath = "";
        string strContent = "";

        strLocalPath = UtilHelper.GetAppSettings("SubTotalFilesPath_RU").ToString();
        FileTools.EnsurePath(Path.GetDirectoryName(strLocalPath));

        DataTable dtRU = new DataTable();
        dtRU = BRRedeemUpload.SelectTxt().Tables[0];

        // 刪除已有的 txt 檔
        JobHelper.SaveLog("刪除舊txt檔", LogState.Info);
        FileTools.DeleteFile(strLocalPath);


        for (int i = 0; i < dtRU.Rows.Count; i++)
        {
            strContent = dtRU.Rows[i]["DATA_TYPE"].ToString() + "\t" + dtRU.Rows[i]["UPLOAD_DATA"].ToString();
            strContent = strContent.Trim().Trim('\t');
            FileTools.CreateAppend(strLocalPath, strContent);
        }

        if (!File.Exists(strLocalPath))
        {
            strLocalPath = "";
        }

        return strLocalPath;
    }

    //add by linhuanhuang 自動 JOB MAIL 發送機制 20100623 start
    private void SendMail(DateTime dateStart, string strMessage)
    {
        string[] str = strMail.Split(';');



        System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();

        nvc["Name"] = strMail.Replace(';', ',');


        nvc["Title"] = strTitle;

        nvc["StartTime"] = dateStart.ToString();

        nvc["EndTime"] = DateTime.Now.ToString();

        nvc["Message"] = strMessage.Trim();

        try
        {

            string strLocalPath = "";
            strLocalPath = UtilHelper.GetAppSettings("SubTotalFilesPath_RU").ToString();
            MailService.MailSender(str, 2, nvc, strLocalPath);
        }
        catch (Exception ex)
        {
            Logging.Log(ex, strJobID);
        }
    }
    //add by linhuanhuang 自動 JOB MAIL 發送機制 20100623 end
    #endregion
}
