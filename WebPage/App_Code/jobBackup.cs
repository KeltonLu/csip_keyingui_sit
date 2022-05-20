﻿//******************************************************************
//*  作    者：Mars
//*  功能說明：清檔及備份 LOG,LOGXML,上傳檔案
//*  創建日期：2012/12/14
//*  修改紀錄：
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*  Ares JaJa          2021/02/04         20200031-CSIP EOS       補充LOG
//*******************************************************************

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Framework.Data.OM.Collections;
using Framework.Data.OM;
using Framework.WebControls;
using Framework.Common.Utility;
using Framework.Common.Message;
using Framework.Common.JavaScript;
using Framework.Common.Logging;
using CSIPCommonModel.EntityLayer;
using Framework.Data.OM.Transaction;
using CSIPCommonModel.BusinessRules;
using Quartz;
using Quartz.Impl;
using Framework.Common.IO;
using System.Collections;
using System.IO;
using System.Collections.Generic;


/// <summary>
/// jobBackup 的摘要描述
/// </summary>
public class jobBackup : IJob
{
    public jobBackup()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    string strFuncKey = UtilHelper.GetAppSettings("FunctionKey").ToString();
    string strSucc = "";
    DateTime dTimeStart;
    private string strJobID;
    private string strJobMsg;
    //private string strMail;
    //private string strJobTitle;

    //*需處理的來源資料夾
    private readonly List<string> SourceFolder = new List<string>(UtilHelper.GetAppSettings("SourceFolder").Split(','));
    private readonly List<string> AMLSourceFolder = new List<string>(UtilHelper.GetAppSettings("AMLSourceFolder").Split(','));//20200408-RQ-2019-030155-005 增加備份傳檔案
    //*需排除附檔名列表
    private readonly List<string> SkipExtension = new List<string>(UtilHelper.GetAppSettings("SkipExtension").Split(','));

    #region IJob 成員
    /// <summary>
    /// Job 調用入口
    /// </summary>
    /// <param name="context"></param>
    public void Execute(JobExecutionContext context)
    { 
        //string strLdapID;
        //string strLdapPWD;
        //string strRacfID;
        //string strRacfPwd;

        // 獲取JOB開始時間
        dTimeStart = DateTime.Now;

        strJobMsg = "";

        try
        {
            //2021/04/07_Ares_Stanley-修正記錄Log時沒有jobid造成的錯誤
            strJobID = context.JobDetail.JobDataMap.GetString("jobid").Trim();
            Log("*************** 備份及清檔作業開始 ***************");
            //strJobTitle = context.JobDetail.JobDataMap.GetString("title").Trim();
            //strLdapID = context.JobDetail.JobDataMap.GetString("userId").Trim();
            //strLdapPWD = context.JobDetail.JobDataMap.GetString("passWord").Trim();
            //strRacfID = context.JobDetail.JobDataMap.GetString("racfId").Trim();
            //strRacfPwd = context.JobDetail.JobDataMap.GetString("racfPassWord").Trim();
            //strMail = context.JobDetail.JobDataMap.GetString("mail").Trim();
            string strMsgID = "";
            //*查詢資料檔L_BATCH_LOG，查看是否上次作業還未停止
            DataTable dtInfo = BRL_BATCH_LOG.GetRunningDate(strFuncKey, strJobID, "R", ref strMsgID);
            if (dtInfo == null)
            {
                return;
            }
            if (dtInfo.Rows.Count > 0)
            {
                return;
            }
            //*開始批次作業
            if (!InsertNewBatch())
            {
                return;
            }

            //*來源檔案基底位置
            string BaseDir = AppDomain.CurrentDomain.BaseDirectory;

            //*來源資料夾位置
            string SourcePath = "";

            //20200408-RQ-2019-030155-005 增加備份傳檔案
            //AML來源資料夾位置
            string AMLSourcePath = "";

            //*備份資料夾位置
            string BackupPath = "";

            //*原始檔案保存起始日(SourceKeepDay)
            int SourceKeepDay = Convert.ToInt32(UtilHelper.GetAppSettings("SourceKeepDay").ToString());
            //20200408-RQ-2019-030155-005 增加備份傳檔案
            int AMLSourceKeepDay = Convert.ToInt32(UtilHelper.GetAppSettings("AMLSourceKeepDay").ToString());

            //*備份檔案保存起始日(BackupKeepDay)
            int BackupKeepDay = Convert.ToInt32(UtilHelper.GetAppSettings("BackupKeepDay").ToString());
            int AMLBackupKeepDay = Convert.ToInt32(UtilHelper.GetAppSettings("AMLBackupKeepDay").ToString());//20200408-RQ-2019-030155-005 增加備份傳檔案
            //*備份路徑(以執行當天的日期命名)
            string BackupDir = UtilHelper.GetAppSettings("BackupPath").ToString() + dTimeStart.ToString("yyyyMMdd");

            //*執行備份動作，如果BackupInitial=true則把所有檔案都備份一次
            Log("***開始備份***");
            bool BackupALL = Convert.ToBoolean(UtilHelper.GetAppSettings("BackupALL"));
            foreach (string SF in SourceFolder)
            {
                SourcePath = BaseDir + SF;
                BackupPath = BackupDir + "\\" + SF;
                if (Directory.Exists(SourcePath))
                {
                    Log(SourcePath + "”開始掃描”");
                    FileBackup(SourcePath, BackupPath, SourceKeepDay, BackupALL);
                }
                else
                    Log(SourcePath + " 此路徑不存在！");
            }
            if (BackupALL)
                strJobMsg += "備份所有資料OK；";
            else
                strJobMsg += "備份" + SourceKeepDay.ToString() + "天前資料OK；";
            Log("***備份完成***");

            //*清除來源資料夾過期檔案
            Log("***開始清除來源資料夾過期檔案***");
            foreach (string SF in SourceFolder)
            {
                SourcePath = BaseDir + SF;
                if (Directory.Exists(SourcePath))
                {
                    Log(SourcePath + "”開始掃描”");
                    ClearFile(SourcePath, SourceKeepDay);
                }
                else
                    Log(SourcePath + " 此路徑不存在！");
            }
            strJobMsg += "清除" + SourceKeepDay.ToString() + "天前來源資料OK；";
            Log("***來源資料夾過期檔案清除完成***");

            //*清除備份資料夾過期檔案
            Log("***開始清除過期備份資料夾***");
            Log(BackupDir.Substring(0, BackupDir.LastIndexOf("\\")) + "”開始掃描”");
            ClearFolder(BackupDir.Substring(0, BackupDir.LastIndexOf("\\")), BackupKeepDay);
            strJobMsg += "清除" + BackupKeepDay.ToString() + "天前備份資料OK；";
            Log("***過期備份資料夾清除完成***");

            #region AML 備份檔案 //20200408-RQ-2019-030155-005 
            //*執行備份動作，如果BackupInitial=true則把所有檔案都備份一次
            Log("***批次檔案備份" + AMLSourceKeepDay.ToString() + "天前資料開始***");
            foreach (string SF in AMLSourceFolder)
            {
                AMLSourcePath = BaseDir + SF;
                BackupPath = BackupDir + "\\" + SF;
                if (Directory.Exists(AMLSourcePath))
                {
                    Log(AMLSourcePath + "”開始掃描”");
                    FileBackup(AMLSourcePath, BackupPath, AMLSourceKeepDay, BackupALL);
                }
                else
                    Log(AMLSourcePath + " 此路徑不存在！");
            }
            if (BackupALL)
                strJobMsg += "批次檔案備份所有資料OK；";
            else
                strJobMsg += "批次檔案備份" + AMLSourceKeepDay.ToString() + "天前資料OK；";
            Log("***批次檔案備份" + AMLSourceKeepDay.ToString() + "天前資料OK***");

            //*清除來源資料夾過期檔案
            Log("***開始清除批次檔案來源資料夾" + AMLSourceKeepDay.ToString() + "天前來源資料***");
            foreach (string SF in AMLSourceFolder)
            {
                AMLSourcePath = BaseDir + SF;
                if (Directory.Exists(AMLSourcePath))
                {
                    Log(AMLSourcePath + "”開始掃描”");
                    ClearFile(AMLSourcePath, AMLSourceKeepDay);
                }
                else
                    Log(AMLSourcePath + " 此路徑不存在！");
            }
            strJobMsg += "清除批次檔案" + AMLSourceKeepDay.ToString() + "天前來源資料OK；";
            Log("***清除批次檔案來源資料夾" + AMLSourceKeepDay.ToString() + "天前來源資料OK***");

            //*清除備份資料夾過期檔案
            Log("***開始清除批次檔案" + AMLBackupKeepDay.ToString() + "天前備份資料夾***");
            Log(BackupDir.Substring(0, BackupDir.LastIndexOf("\\")) + "”開始掃描”");
            ClearFolder(BackupDir.Substring(0, BackupDir.LastIndexOf("\\")), AMLBackupKeepDay);
            strJobMsg += "清除批次檔案" + AMLBackupKeepDay.ToString() + "天前備份資料OK；";
            Log("***開始清除批次檔案" + AMLBackupKeepDay.ToString() + "天前備份資料夾OK***");
            #endregion

            //*批次完成記錄LOG信息
            string strMsg = strJobID + "執行於:" + DateTime.Parse(context.FireTimeUtc.ToString()).AddHours(8).ToString();
            if (context.NextFireTimeUtc.HasValue)
                strMsg += "  ;下次執行於:" + DateTime.Parse(context.NextFireTimeUtc.ToString()).AddHours(8).ToString();
            Logging.Log(strMsg, strJobID, LogLayer.DB);
            
            strSucc = "S";
            InsertBatchLog(strJobMsg, dTimeStart);

            Log("*************** 備份及清檔作業完成 ***************\r\n===========================================================================================================");
        }
        catch (Exception exp)
        {
            //*批次完成記錄LOG信息
            strSucc = "F";
            InsertBatchLog(strJobMsg + exp.Message.ToString(), dTimeStart);
            Logging.Log(exp, strJobID);
        }
    }
    #endregion

    /// <summary>
    /// 備份檔案
    /// </summary>
    /// <param name="SourcePath">來源路徑</param>
    /// <param name="BackupPath">備份路徑</param>
    /// <param name="BackupDay">需備份資料的日期(幾天前)</param>
    /// <param name="BackupALL">是否備份所有資料</param>
    private void FileBackup(string SourcePath, string BackupPath, int BackupDay, bool BackupALL)
    {
        DirectoryInfo dirinfo;
        dirinfo = new DirectoryInfo(SourcePath);
        FileInfo[] FileList = dirinfo.GetFiles();

        //*檢查資料夾中所有檔案符合備份日期的執行備份
        foreach (FileInfo F in FileList)
        {
            //*BackupALL=true 備份來源資料夾所有資料，否則備份BackupDay天數之前的所有資料(EX. BackupDay=30,備份30天前的所有資料)
            if ((DateTime.Now.Date - F.LastWriteTime.Date).Days > BackupDay || BackupALL)
            {
                if (!SkipExtension.Contains(F.Extension))
                {
                    //*確認備份資料夾
                    if (!Directory.Exists(BackupPath))
                        Directory.CreateDirectory(BackupPath);

                    F.CopyTo(BackupPath + "\\" + F.Name, true);
                    Log("複製 " + F.Name + " 從 " + F.DirectoryName + " 到 " + BackupPath);
                }
            }
        }
        //*處理來源資料夾中的子資料夾
        DirectoryInfo[] Childdirinfo = dirinfo.GetDirectories();
        if (Childdirinfo.Length > 0)
        {
            foreach (DirectoryInfo D in Childdirinfo)
            {
                FileBackup(D.FullName, BackupPath + "\\" + D.Name, BackupDay, BackupALL);
            }
        }
    }

    /// <summary>
    /// 清除過期檔案
    /// </summary>
    /// <param name="ClearPath">需清除的路徑</param>
    /// <param name="ClearDate">保留天數</param>
    private void ClearFile(string ClearPath, int KeepDay)
    {
        DirectoryInfo dirinfo;
        dirinfo = new DirectoryInfo(ClearPath);
        FileInfo[] FileList = dirinfo.GetFiles();

        //*檢查資料夾中所有檔案修改日期小於清除日期執行刪除動作
        foreach (FileInfo F in FileList)
        {
            if ((DateTime.Now.Date - F.LastWriteTime.Date).Days > KeepDay)
            {
                if (!SkipExtension.Contains(F.Extension))
                {
                    F.Delete();
                    Log("刪除 " + F.FullName);
                }
            }
        }
        //*處理子資料夾
        DirectoryInfo[] Childdirinfo = dirinfo.GetDirectories();
        if (Childdirinfo.Length > 0)
        {
            foreach (DirectoryInfo D in Childdirinfo)
            {
                ClearFile(D.FullName, KeepDay);
            }
        }
    }

    /// <summary>
    /// 清除過期資料夾
    /// </summary>
    /// <param name="ClearPath">需清除的路徑</param>
    /// <param name="ClearDate">保留天數</param>
    private void ClearFolder(string ClearPath, int KeepDay)
    {
        DirectoryInfo dirinfo = new DirectoryInfo(ClearPath);
        DirectoryInfo[] DirList = dirinfo.GetDirectories();

        string ClearDate = DateTime.Now.AddDays(-KeepDay).ToString("yyyyMMdd");

        //*檢查資料夾日期小於清除日期執行刪除動作
        foreach (DirectoryInfo D in DirList)
        {
            if (D.Name.CompareTo(ClearDate) <= 0)
            {
                //D.Delete(true);
                //Log("刪除資料夾 " + D.FullName);
                try
                {
                    D.Delete(true);
                    Log("刪除資料夾 " + D.FullName);
                }
                catch (Exception e)
                {
                    Log("資料夾：" + D.Name);
                    Log("刪除資料夾發生錯誤：" + e.Message);
                }                
            }
        }
    }

    /// <summary>
    /// 備份及清檔LOG
    /// </summary>
    /// <param name="strError">JOB失敗信息</param>
    /// <param name="dateStart">JOB開始時間</param>
    private void Log(string LogStr)
    {
        string BackupPath = UtilHelper.GetAppSettings("BackupPath").ToString() + dTimeStart.ToString("yyyyMMdd");
        StreamWriter sw = null;
        LogStr = DateTime.Now.ToLocalTime().ToString() + " ： " + LogStr;
        try
        {
            //*確認備份資料夾
            if (!Directory.Exists(BackupPath))
                Directory.CreateDirectory(BackupPath);
            sw = new StreamWriter(BackupPath + "\\BackupJobLog.txt", true);
            sw.WriteLine(LogStr);
            Logging.Log(LogStr, strJobID);
        }
        catch(Exception exp)
        {
            Logging.Log(exp, strJobID);
        }
        finally
        {
            if (sw != null)
            {
                sw.Close();
            }
        }
    }

    /// <summary>
    /// 插入L_BATCH_LOG資料庫
    /// </summary>
    /// <param name="strError">JOB失敗信息</param>
    /// <param name="dateStart">JOB開始時間</param>
    private void InsertBatchLog(string strError, DateTime dateStart)
    {
        //*插入L_BATCH_LOG資料庫
        BRL_BATCH_LOG.Delete(strFuncKey, strJobID, "R");
        BRL_BATCH_LOG.Insert(strFuncKey, strJobID, dateStart, strSucc, strError);
    }

    /// <summary>
    /// 開始此次作業向Job_Status中插入一筆新的資料
    /// </summary>
    /// <returns>true成功，false失敗</returns>
    private bool InsertNewBatch()
    {
        return BRL_BATCH_LOG.InsertRunning(strFuncKey, strJobID, dTimeStart, "R", "");
    }

    /// <summary>
    /// JOB執行狀態
    /// </summary>
    /// <param name="strStauts">狀態英文名稱</param>
    /// <returns>JOB執行狀態</returns>
    private string GetStatusName(string strStauts)
    {
        switch (strStauts)
        {
            case "F":
                return "失敗";
            case "S":
                return "成功";
            default:
                return "";
        }
    }
}
