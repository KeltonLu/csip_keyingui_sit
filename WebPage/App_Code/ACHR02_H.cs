//******************************************************************
//*  作    者：Ares Luke
//*  功能說明：stored procedure 移轉至批次
//*  創建日期：2020/12/22
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*Ares Luke          2021/03/18         20200031-CSIP EOS       調整資料連線指定connectionString
//*Ares Luke          2021/04/12         20200031-CSIP EOS       修正邏輯與LOG訊息調整
//*Ares Stanley      2021/06/01                                               修正工作狀態檢核
//*******************************************************************


using System;
using System.Data;
using System.Data.SqlClient;
using CSIPCommonModel.BusinessRules;
using CSIPNewInvoice.EntityLayer_new;
using Framework.Common.Logging;
using Quartz;

/// <summary>
/// ACHR02_H 的摘要描述
/// </summary>
public class ACHR02_H : Quartz.IJob
{
    private static readonly JobHelper JobHelper = new JobHelper();
    private readonly string _strFunctionKey = "01";
    private static string _strJobId;
    DateTime _dateStart = DateTime.Now; //開始時間


    public void Execute(JobExecutionContext context)
    {
        #region 初始化參數

        string strMsgId = string.Empty;
        string batchLogErr = "";
        int impTotalSuccess = 0;
        int impTotalFail = 0;
        bool executeFlag = false;

        #endregion

        try
        {
            JobDataMap jobDataMap = context.JobDetail.JobDataMap;
            _strJobId = jobDataMap.GetString("jobid").Trim();
            JobHelper.strJobID = _strJobId;

            JobHelper.SaveLog(_strJobId + " JOB啟動", LogState.Info);

            string jobMailTo = jobDataMap.GetString("mail").Trim();

            #region 檢測JOB是否在執行中

            // 判斷Job工作狀態(0:停止 1:運行)
            var isContinue = ACHR02.CheckJobIsContinue(_strJobId, _strFunctionKey, _dateStart, ref strMsgId);

            #endregion

            //*開始批次作業

            #region 功能

            if (isContinue)
            {
                executeFlag = true;
                //20200031-CSIP EOS Ares Luke 修改日期:2021/02/22 修改說明:業務需求將日期格式為民國年8碼
                //20200031-CSIP EOS Ares Luke 修改日期:2021/03/12 修改說明:業務需求 手動RERUN參數
                DateTime dt = DateTime.Now;
                string jobDate = string.Format("{0:0000}{1:00}{2:00}", (Int32.Parse(dt.Year.ToString()) - 1911),
                    dt.Month, dt.Day);

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
                            jobDate = string.Format("{0:0000}{1:00}{2:00}",
                                (Int32.Parse(tempDt.Year.ToString()) - 1911),
                                tempDt.Month, tempDt.Day);
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

                string batchSql =
                    @"SELECT Batch_no, DateInput, TotalCount FROM batch WHERE DateReceive < @sToday AND P02_flag = 'Y' AND R02_flag IN ('0', '1')";

                SqlCommand sqlComm = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = batchSql
                };
                sqlComm.Parameters.Add(new SqlParameter("@sToday", jobDate));
                DataSet ds = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    JobHelper.SaveLog("查無資料。", LogState.Info);
                    return;
                }

                foreach (DataRow data in ds.Tables[0].Rows)
                {
                    // 筆數
                    string logErrTmp = "";
                    ACHR02.FnDomain(data, _strFunctionKey, _strJobId, jobDate, jobMailTo, ref logErrTmp);

                    if (logErrTmp == "")
                    {
                        impTotalSuccess++;
                    }
                    else
                    {
                        impTotalFail++;
                        batchLogErr += logErrTmp;
                    }
                }
            }

            #endregion
        }
        catch (Exception ex)
        {
            // 寫入 BatchLog
            ACHR02.InsertBatchLog(_strJobId, _strFunctionKey, _dateStart, impTotalSuccess, impTotalFail, "F",
                "發生錯誤：" + ex.Message);
            JobHelper.SaveLog("ACHR02_發錯錯誤_" + ex.ToString(), LogState.Error);
        }
        finally
        {
            #region 紀錄下次執行時間

            string strMsg = _strJobId + "執行於:" + DateTime.Parse(context.FireTimeUtc.ToString()).AddHours(8).ToString();
            if (context.NextFireTimeUtc.HasValue)
            {
                strMsg += "  ;下次執行於:" + DateTime.Parse(context.NextFireTimeUtc.ToString()).AddHours(8).ToString();
            }

            JobHelper.SaveLog(strMsg, LogState.Info);

            #endregion

            #region 結束批次作業
            if (executeFlag)
            {
                if (batchLogErr == "")
                {
                    ACHR02.InsertBatchLog(_strJobId, _strFunctionKey, _dateStart, impTotalSuccess, impTotalFail, "S", "");
                }
                else
                {
                    ACHR02.InsertBatchLog(_strJobId, _strFunctionKey, _dateStart, impTotalSuccess, impTotalFail, "F", batchLogErr);
                }
            }
            else
            {
                JobHelper.SaveLog("JOB未執行(狀態未啟用 or 已在運行中)", LogState.Info);
            }


            JobHelper.SaveLog(_strJobId + " JOB結束", LogState.Info);

            #endregion
        }
    }
}