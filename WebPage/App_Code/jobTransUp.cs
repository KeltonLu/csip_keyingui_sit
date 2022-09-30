//******************************************************************
//* 作    者：
//* 功能說明：
//* 創建日期：
//* 修改紀錄：
//* <author>            <time>            <TaskID>                <desc>
//* Ares Luke          2020/12/29         20200031-CSIP EOS       新增Log訊息
//*******************************************************************

using System;
using System.Data;
using Quartz;
using Framework.Common.Logging;
using CSIPCommonModel.BusinessRules;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Utility;
using Framework.Common.Message;

/// <summary>
/// jobAutoBalance_Trans 的摘要描述
/// </summary>
public class jobTransUp : IJob
{
    private static readonly JobHelper JobHelper = new JobHelper();

    #region IJob 成員
    public void Execute(JobExecutionContext context)
    {
        string strFuncKey = "01";
        string strJobID = "jobTransUp";
        string strJobTitle;
        string strLdapID;
        string strLdapPWD;
        string strRacfID;
        string strRacfPwd;
        string strMailList;
        string status = ""; // 0:成功,1:無資料,2:產檔失敗,3:上傳失敗,4:更新失敗,5:更新參數失敗
        string parameter = "";
        DateTime dtStart = DateTime.Now;       //  BRL_BATCH_LOG 紀錄job起始時間
        BRAuto_Balance_Trans bbt = new BRAuto_Balance_Trans();

        strJobID = context.JobDetail.JobDataMap.GetString("jobid").Trim();
        strJobTitle = context.JobDetail.JobDataMap.GetString("title").Trim();
        strLdapID = context.JobDetail.JobDataMap.GetString("userId").Trim();
        strLdapPWD = context.JobDetail.JobDataMap.GetString("passWord").Trim();
        strRacfID = context.JobDetail.JobDataMap.GetString("racfId").Trim();
        strRacfPwd = context.JobDetail.JobDataMap.GetString("racfPassWord").Trim();
        strMailList = context.JobDetail.JobDataMap.GetString("mail").Trim();
        string strMsgID = "";
        // 20220815 調整若無資料則產生空檔並上傳至 FTP by Kelton
        // 發件人
        string sAddresser = UtilHelper.GetAppSettings("MailSender");

        JobHelper.strJobID = strJobID;
        JobHelper.SaveLog(strJobID + "JOB啟動", LogState.Info);

        try
        {
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


            DataSet ds = new DataSet();
            ds = bbt.GetFTPinfo(strJobID);

            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                parameter = ds.Tables[0].Rows[0]["Parameter"] != null
                    ? ds.Tables[0].Rows[0]["Parameter"].ToString()
                    : DateTime.Now.ToString("yyyyMMdd");
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

            ds = new DataSet();
            /*
            ds = string.IsNullOrEmpty(parameter) ? BRAuto_Balance_Trans.GetDataFromtblAuto_Balance_Trans(DateTime.Now.ToShortDateString())
                : BRAuto_Balance_Trans.GetDataFromtblAuto_Balance_Trans(string.Format("{0}/{1}/{2}",
                parameter.Substring(0, 4), parameter.Substring(4, 2), parameter.Substring(6, 2)));
            */
            ds = string.IsNullOrEmpty(parameter)
                ? BRAuto_Balance_Trans.GetDataFromtblAuto_Balance_Trans(string.Format("{0:yyyy/MM/dd}", DateTime.Now))
                : BRAuto_Balance_Trans.GetDataFromtblAuto_Balance_Trans(string.Format("{0}/{1}/{2}",
                    parameter.Substring(0, 4), parameter.Substring(4, 2), parameter.Substring(6, 2)));
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {

                JobHelper.SaveLog("匯出O317.txt檔案,開始！", LogState.Info);
                if (BRAuto_Balance_Trans.BatchOutput(ds.Tables[0]))
                {
                    JobHelper.SaveLog("匯出O317.txt檔案,成功！", LogState.Info);

                    JobHelper.SaveLog("上傳O317.txt檔案,開始！", LogState.Info);
                    if (bbt.UploadToFTP(strJobID))
                    {
                        JobHelper.SaveLog("上傳O317.txt檔案,成功！", LogState.Info);

                        JobHelper.SaveLog("更新Balance_Trans參數,開始！", LogState.Info);
                        if (BRAuto_Balance_Trans.UpdateModifyTimeAndFalg(strJobID, parameter))
                        {
                            JobHelper.SaveLog("更新Balance_Trans參數,成功！", LogState.Info);
                            status = "0";
                        }
                        else
                        {
                            JobHelper.SaveLog("更新Balance_Trans參數,失敗！", LogState.Error);
                            status = "4";
                        }
                    }
                    else
                    {
                        JobHelper.SaveLog("上傳O317.txt檔案,失敗！", LogState.Error);
                        status = "3";
                    }
                }
                else
                {
                    JobHelper.SaveLog("匯出O317.txt檔案,失敗！", LogState.Error);
                    status = "2";
                }
            }
            else
            {
                // 20220815 調整若無資料則產生空檔並上傳至 FTP by Kelton
                //JobHelper.SaveLog("查無資料！", LogState.Info);
                status = "1";
                JobHelper.SaveLog("匯出空的O317.txt檔案,開始！", LogState.Info);
                BRAuto_Balance_Trans.BatchOutputEmpty();
                JobHelper.SaveLog("匯出空的O317.txt檔案,成功！", LogState.Info);

                JobHelper.SaveLog("上傳空的O317.txt檔案,開始！", LogState.Info);
                if (bbt.UploadToFTP(strJobID))
                {
                    JobHelper.SaveLog("上傳空的O317.txt檔案,成功！", LogState.Info);
                }
                else
                {
                    JobHelper.SaveLog("上傳空的O317.txt檔案,失敗！", LogState.Error);
                    status = "3";
                }
            }

            if (!string.IsNullOrEmpty(parameter))
                bbt.UpdateParameter(strJobID);

            JobDataMap jobDataMap = context.JobDetail.JobDataMap;
            string strMsg = strJobID + "執行於:" + DateTime.Parse(
                context.FireTimeUtc.ToString()).AddHours(8).ToString();

            if (context.NextFireTimeUtc.HasValue)
            {
                strMsg += "  ;下次執行於:" + DateTime.Parse(
                    context.NextFireTimeUtc.ToString()).AddHours(8).ToString();
            }

            JobHelper.SaveLog(strMsg, LogState.Info);

            Logging.Log(strMsg, strJobID, LogLayer.DB);
            BRL_BATCH_LOG.Delete(strFuncKey, strJobID, "R");
            switch (status)
            {
                case "0":
                    //2021/04/07_Ares_Stanley-增加string format缺少的參數
                    JobHelper.SaveLog(
                        string.Format("餘轉日期{0},資料庫更新成功,共{1}筆",
                            DateTime.Parse(ds.Tables[0].Rows[0]["Trans_Date"].ToString()).ToString("yyyyMMdd"), ds.Tables[0].Rows.Count)
                        , LogState.Info);
                    BRL_BATCH_LOG.Insert(strFuncKey, strJobID, dtStart, "S",
                        string.Format("餘轉日期{0},資料庫更新成功,共{1}筆",
                            DateTime.Parse(ds.Tables[0].Rows[0]["Trans_Date"].ToString()).ToString("yyyyMMdd")
                            , ds.Tables[0].Rows.Count));
                    break;
                case "1":  
                    JobHelper.SaveLog("無符合資料須上傳", LogState.Info);
                    BRL_BATCH_LOG.Insert(strFuncKey, strJobID, dtStart, "S", "無符合資料須上傳");
                    // 20220815 調整若無資料則產生空檔並上傳至 FTP by Kelton
                    JobHelper.SendMail(strMailList, strJobID + " 批次執行", MessageHelper.GetMessage("00_00000s000_043"), "成功", dtStart);
                    break;
                case "2":
                    JobHelper.SaveLog("產生txt檔案失敗", LogState.Error);
                    BRL_BATCH_LOG.Insert(strFuncKey, strJobID, dtStart, "F", "產生txt檔案失敗");
                    break;
                case "3":
                    JobHelper.SaveLog("FTP上傳失敗", LogState.Error);
                    BRL_BATCH_LOG.Insert(strFuncKey, strJobID, dtStart, "F", "FTP上傳失敗");
                    break;
                case "4":
                    JobHelper.SaveLog("資料庫更新失敗", LogState.Error);
                    BRL_BATCH_LOG.Insert(strFuncKey, strJobID, dtStart, "F", "資料庫更新失敗");
                    break;
            }

            JobHelper.SaveLog(strMsgID + "JOB結束", LogState.Info);
        }
        catch (Exception ex)
        {
            BRL_BATCH_LOG.Delete(strFuncKey, strJobID, "R");
            BRL_BATCH_LOG.Insert(strFuncKey, strJobID, dtStart, "F", ex.Message);
            Logging.Log(ex, strJobID, LogLayer.DB);
        }
    }
    #endregion

}
