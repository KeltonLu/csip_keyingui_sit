//******************************************************************
//* 作    者：
//* 功能說明：
//* 創建日期：
//* 修改紀錄：
//* <author>            <time>            <TaskID>                <desc>
//* Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//* Ares Luke          2020/12/29         20200031-CSIP EOS       新增Log訊息
//*******************************************************************

using System;
using System.Data;
using Quartz;
using Framework.Common.Logging;
using Framework.Common.Utility;
using CSIPCommonModel.BusinessRules;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Message;
using ThreadingTimer = System.Threading.Timer;

/// <summary>
/// jobReturnBalance_Trans 的摘要描述
/// 修改紀錄:2021/03/03_Ares_Stanley-增加LOG
/// </summary>
public class jobTransDown : IJob
{
    private static readonly JobHelper JobHelper = new JobHelper();

    #region IJob 成員
    public void Execute(JobExecutionContext context)
    {
        string zipPW = "";
        string fileName = "";
        string strFuncKey = "01";
        string strJobID = "jobTransDown";
        string strJobTitle;
        string strLdapID;
        string strLdapPWD;
        string strRacfID;
        string strRacfPwd;
        string strMailList;
        double loopMinutes = 0;
        string strMessage = "";	//EMAIL訊息
        DateTime dtStart = DateTime.Now;
        DateTime endTime = DateTime.Now;

        strJobID = context.JobDetail.JobDataMap.GetString("jobid").Trim();
        strJobTitle = context.JobDetail.JobDataMap.GetString("title").Trim();
        strLdapID = context.JobDetail.JobDataMap.GetString("userId").Trim();
        strLdapPWD = context.JobDetail.JobDataMap.GetString("passWord").Trim();
        strRacfID = context.JobDetail.JobDataMap.GetString("racfId").Trim();
        strRacfPwd = context.JobDetail.JobDataMap.GetString("racfPassWord").Trim();
        strMailList = context.JobDetail.JobDataMap.GetString("mail").Trim();
        string strMsgID = "";
        string parameter = "";
        string status = "";
        DateTime loopTime = DateTime.Now;

        JobHelper.strJobID = strJobID;
        JobHelper.SaveLog(strJobID + "JOB啟動", LogState.Info);

        BRAuto_Balance_Trans bbt = new BRAuto_Balance_Trans();

        //*查詢資料檔L_BATCH_LOG，查看是否上次作業還未停止
        DataTable dtInfo = BRL_BATCH_LOG.GetRunningDate(strFuncKey, strJobID, "R", ref strMsgID);
        if (dtInfo == null || dtInfo.Rows.Count > 0)
        {
            JobHelper.SaveLog("JOB 工作狀態為：正在執行！", LogState.Info);
            return;
        }


        // 20200031-CSIP EOS Ares Luke 修改日期:2021/03/18 修改說明:業務需求排程結束清空FilInfo參數
        #region 判斷是否手動設置參數啟動排程

        JobHelper.SaveLog("判斷是否手動輸入參數 啟動排程：開始！", LogState.Info);

        if (context.JobDetail.JobDataMap["param"] != null)
        {
            JobHelper.SaveLog("手動輸入參數啟動排程：是！", LogState.Info);
            JobHelper.SaveLog("檢核輸入參數：開始！", LogState.Info);

            string strParam = context.JobDetail.JobDataMap["param"].ToString();

            if (strParam.Length == 10)
            {
                DateTime tempDt;
                if (DateTime.TryParse(strParam, out tempDt))
                {
                    JobHelper.SaveLog("檢核參數：成功！ 參數：" + strParam, LogState.Info);

                    if (BRM_FileInfo.UpdateParam(strJobID, tempDt.ToString("yyyyMMdd")))
                    {
                        JobHelper.SaveLog("更新參數至FileInfo：成功！ 參數：" + tempDt.ToString("yyyyMMdd"), LogState.Info);
                    }
                    else
                    {
                        JobHelper.SaveLog("更新參數至FileInfo：失敗！ 參數：" + tempDt.ToString("yyyyMMdd"), LogState.Error);
                        return;
                    }
                }
                else
                {
                    JobHelper.SaveLog("檢核參數：異常！ 參數：" + strParam, LogState.Error);
                    return;
                }
            }
            else
            {
                JobHelper.SaveLog("檢核參數：異常！ 參數：" + strParam, LogState.Error);
                return;
            }

            JobHelper.SaveLog("檢核輸入參數：結束！", LogState.Info);
        }
        else
        {
            JobHelper.SaveLog("手動輸入參數啟動排程：否！", LogState.Info);
        }

        JobHelper.SaveLog("判斷是否手動輸入參數 啟動排程：結束！", LogState.Info);

        #endregion



        DataSet ds = bbt.GetFTPinfo(strJobID);

        if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
        {
            parameter = ds.Tables[0].Rows[0]["Parameter"] != null ? ds.Tables[0].Rows[0]["Parameter"].ToString() : "";
            zipPW = RedirectHelper.GetDecryptString(ds.Tables[0].Rows[0]["ZipPwd"].ToString());

            if (string.IsNullOrEmpty(parameter))
                //fileName = string.Format("{0}{1}{2}", ds.Tables[0].Rows[0]["FtpFileName"].ToString().Trim(),DateTime.Now.Month, DateTime.Now.Day); // O318MMdd
                fileName = string.Format("{0}{1}", ds.Tables[0].Rows[0]["FtpFileName"].ToString().Trim(), String.Format("{0:MMdd}",System.DateTime.Now)); // O318MMdd
            else
                fileName = string.Format("{0}{1}{2}", ds.Tables[0].Rows[0]["FtpFileName"].ToString().Trim(),
                  parameter.Substring(4, 2), parameter.Substring(6, 2)); // O318MMdd

            if (string.IsNullOrEmpty(zipPW))
                fileName += ".TXT";
            else
                fileName += ".EXE";

            loopMinutes = double.TryParse(ds.Tables[0].Rows[0]["LoopMinutes"].ToString(), out loopMinutes)
                ? double.Parse(ds.Tables[0].Rows[0]["LoopMinutes"].ToString()) : loopMinutes;
            endTime = DateTime.Now.AddMinutes(loopMinutes);
        }

        if (string.IsNullOrEmpty(parameter))
        {
            //*判斷是否為假日
            if (!BRWORK_DATE.IS_WORKDAY("01", DateTime.Now.ToString("yyyyMMdd")))
            {
                JobHelper.SaveLog("非營業日不可執行！", LogState.Info);
                BRL_BATCH_LOG.InsertRunning(strFuncKey, strJobID, dtStart, "F", "非營業日不可執行");
                return;
            }
        }
        else
        {
            if (!BRWORK_DATE.IS_WORKDAY("01", parameter))
            {
                JobHelper.SaveLog("非營業日不可執行！", LogState.Info);
                BRL_BATCH_LOG.InsertRunning(strFuncKey, strJobID, dtStart, "F", "非營業日不可執行");
                return;
            }
        }

        //*開始批次作業
        if (!BRL_BATCH_LOG.InsertRunning(strFuncKey, strJobID, dtStart, "R", ""))
        {
            return;
        }

        try
        {
            int cnt = 0;
            int s_cnt = 0;
            string msg = "";
            int currentRound = 1;
            //string jobName = System.Threading.Thread.CurrentThread.Name;
            //ThreadingTimer = new ThreadingTimer(new System.Threading.TimerCallback(JobDownload), jobName,
            //    5, int.Parse(loopMinutes.ToString()));
            JobHelper.SaveLog("\r==============================" + "執行檔案下載/解壓縮迴圈，開始！" + string.Format("迴圈開始時間: {0} 迴圈預估結束時間: {1} ", dtStart, endTime) + "==============================", LogState.Info);
            for (DateTime i = dtStart; i <= endTime; i = loopTime)
            {
                JobHelper.SaveLog("\r==============================" + "目前執行圈數" + currentRound + "==============================", LogState.Info);
                if (i.ToString("yyyyMMdd HH:mm") == DateTime.Now.ToString("yyyyMMdd HH:mm"))
                {
                    msg = "";
                    #region
                    JobHelper.SaveLog("下載檔案,開始！", LogState.Info);
                    if (bbt.FTPDownload(strJobID, parameter, ds, ref msg))
                    {
                        JobHelper.SaveLog("下載檔案,成功！", LogState.Info);
                        string path = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("UpLoadFilePath");

                        JobHelper.SaveLog("檔案解壓縮並讀檔,開始！", LogState.Info);
                        if (bbt.CheckData0318(path, fileName, strJobID, zipPW, ref cnt, ref s_cnt, ref msg))
                        {
                            JobHelper.SaveLog("檔案解壓縮並讀檔,成功！", LogState.Info);

                            #region
                            JobDataMap jobDataMap = context.JobDetail.JobDataMap;

                            string strMsg = strJobID + "執行於:" + DateTime.Parse(
                                context.FireTimeUtc.ToString()).AddHours(8).ToString();

                            if (context.NextFireTimeUtc.HasValue)
                            {
                                strMsg += "  ;下次執行於:" + DateTime.Parse(
                                    context.NextFireTimeUtc.ToString()).AddHours(8).ToString();
                            }

                            Logging.Log(strMsg, strJobID, LogLayer.DB);
                            JobHelper.SaveLog(strMsg, LogState.Info);
                            #endregion
                            status = "S";
                        }
                        else
                        {
                            JobHelper.SaveLog("檔案解壓縮並讀檔,失敗！", LogState.Error);
                            status = "F";
                        }
                        JobHelper.SaveLog("\r==============================" + "下載成功，中斷迴圈！" + "==============================", LogState.Info);
                        currentRound += 1;
                        break;  // 若已下載成功的話就結束下載檔案
                    }
                    else
                    {
                        JobHelper.SaveLog("下載檔案,失敗！", LogState.Error);
                        status = "N";
                    }

                    #endregion
                    loopTime = i.AddMinutes(5);
                    TimeSpan ts = new TimeSpan(loopTime.Ticks - endTime.Ticks);
                    //if (loopTime.ToString("yyyyMMdd HH:mm") != endTime.ToString("yyyyMMdd HH:mm"))
                    if (ts.Hours < 0 || ts.Minutes < 0)
                        System.Threading.Thread.Sleep(295000);
                    else
                    {
                        ts = new TimeSpan(endTime.Ticks - DateTime.Now.Ticks);
                        if (ts.TotalSeconds > 0)
                            System.Threading.Thread.Sleep(Convert.ToInt32(ts.TotalSeconds > 5 ? ts.TotalSeconds - 5 : ts.TotalSeconds) * 1000);
                    }
                }
                currentRound += 1;
            }

            JobHelper.SaveLog("\r==============================" + "執行檔案下載/解壓縮迴圈，結束！" + string.Format("迴圈開始時間: {0} 迴圈實際結束時間: {1} ", dtStart, DateTime.Now) + "==============================", LogState.Info);
            JobHelper.SaveLog("\r==============================" + "迴圈執行總圈數：" + (currentRound - 1) + "==============================", LogState.Info);

            if (!string.IsNullOrEmpty(parameter))
                bbt.UpdateParameter(strJobID);

            BRL_BATCH_LOG.Delete(strFuncKey, strJobID, "R");
            switch (status)
            {
                case "S":
                    JobHelper.SaveLog(string.Format("檔案名稱:{0},匯入資料庫成功,共{1}筆", fileName, s_cnt), LogState.Info);
                    BRL_BATCH_LOG.Insert(strFuncKey, strJobID, dtStart, "S", string.Format("檔案名稱:{0},匯入資料庫成功,共{1}筆", fileName, s_cnt));
                    break;
                case "N":
                    JobHelper.SaveLog(strMessage, LogState.Error);
                    BRL_BATCH_LOG.Insert(strFuncKey, strJobID, dtStart, "F", strMessage);
                    break;
                case "F":
                    strMessage = string.IsNullOrEmpty(msg) ? "匯入資料庫失敗" : msg;
                    JobHelper.SaveLog(strMessage, LogState.Error);
                    BRL_BATCH_LOG.Insert(strFuncKey, strJobID, dtStart, "F", strMessage);
                    break;
            }
        }
        catch (Exception ex)
        {
            BRL_BATCH_LOG.Delete(strFuncKey, strJobID, "R");
            BRL_BATCH_LOG.Insert(strFuncKey, strJobID, dtStart, "F", ex.Message);
            Logging.Log(ex, strJobID, LogLayer.DB);
            strMessage = ex.Message;
        }

        finally
        {
            // 20200031-CSIP EOS Ares Luke 修改日期:2021/03/18 修改說明:業務需求排程結束清空FilInfo參數
            // 清除 FileInfo Parameter值
            BRM_FileInfo.UpdateParam(strJobID, "");


            //JOB不成功發送EMAIL
            if (status != "S")
            {
                string strMail = context.JobDetail.JobDataMap.GetString("mail").Trim();
                string strTitle = context.JobDetail.JobDataMap.GetString("title").Trim();
                string[] str = strMail.Split(';');

                System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();
                nvc["Name"] = strMail.Replace(';', ',');
                nvc["Title"] = strTitle;
                nvc["StartTime"] = dtStart.ToString();
                nvc["EndTime"] = DateTime.Now.ToString();
                nvc["Message"] = strMessage.Trim();


                JobHelper.SaveLog("排程執行失敗,開始寄信。", LogState.Info);
                try
                {
                    MailService.MailSender(str, 2, nvc, "");
                    JobHelper.SaveLog("排程執行失敗,寄信成功。", LogState.Info);
                }
                catch (Exception ex)
                {
                    Logging.Log(ex, strJobID);
                    JobHelper.SaveLog("排程執行失敗,寄信失敗。" + ex.ToString(), LogState.Error);
                }
            }

            JobHelper.SaveLog(strMsgID + "JOB結束", LogState.Info);
        }

    }

    //public void JobDownload()
    //{
    //    if (bbt.FTPDownload(strJobID, parameter))
    //    {
    //        string path = AppDomain.CurrentDomain.BaseDirectory +
    //                        ConfigurationManager.AppSettings["UpLoadFilePath"];

    //        if (bbt.CheckData0318(path, fileName, strJobID, zipPW, ref cnt, ref s_cnt))
    //        {
    //            #region
    //            JobDataMap jobDataMap = context.JobDetail.JobDataMap;

    //            string strMsg = strJobID + "執行於:" + DateTime.Parse(
    //                context.FireTimeUtc.ToString()).AddHours(8).ToString();

    //            if (context.NextFireTimeUtc.HasValue)
    //            {
    //                strMsg += "  ;下次執行於:" + DateTime.Parse(
    //                    context.NextFireTimeUtc.ToString()).AddHours(8).ToString();
    //            }

    //            Logging.SaveLog(ELogLayer.DB, strMsg, ELogType.Info);
    //            #endregion
    //            if (cnt == 0)
    //            {
    //                status = "F";
    //            }
    //            else
    //            {
    //                status = "S";
    //            }
    //        }
    //        else
    //        {
    //            status = "F";
    //        }
    //        break;  // 若已下載成功的話就結束下載檔案
    //    }
    //    else
    //    {
    //        status = "N";
    //    }
    //}

    #endregion
}
