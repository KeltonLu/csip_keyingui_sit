//******************************************************************
//* 作    者：
//* 功能說明：
//* 創建日期：
//* 修改紀錄：
//* <author>            <time>            <TaskID>                <desc>
//* Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*  Ares_jhun         2021/09/28         RQ-2022-019375-000       EDDA需求調整：將郵局核印失敗資料匯入【Auto_Pay_Auth_Fail】
//*******************************************************************

using System;
using System.Data;
using Quartz;
using CSIPCommonModel.BusinessRules;
using System.Text;
using Framework.Common.Logging;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Utility;

/// <summary>
/// BatchJob_PostReply 的摘要描述
/// </summary>
public class BatchJob_PostReply : Quartz.IJob
{
    protected string FunctionKey = UtilHelper.GetAppSettings("FunctionKey").ToString();
    protected DateTime StartTime = DateTime.Now;// 記錄Job啟動時間
    protected DateTime EndTime;
    protected JobHelper JobHelper = new JobHelper();

    public void Execute(JobExecutionContext context)
    {
        JobDataMap jobDataMap = context.JobDetail.JobDataMap;
        EntityAGENT_INFO eAgentInfo = GetAGENT_INFO(jobDataMap);

        int total = 0;
        int success = 0;
        string jobID = context.JobDetail.JobDataMap["jobid"].ToString();
        JobHelper.strJobID = jobID;
        string masterID = "";
        string fileName = "";
        bool isContinue = true;
        string msgID = string.Empty;
        PostOfficeService postOfficeService = new PostOfficeService(jobID);
        int successCount = 0;
        int failCount = 0;
        int postActiveCount = 0;
        string replyDate = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
        DateTime dateTime;
        string result = "";
        bool hasFile = false;
        string errorMsg = "";
        string sourceName = "";

        try
        {
            JobHelper.Write(jobID, "*********** " + jobID + " 郵局核印回覆 批次 START ************** ", LogState.Info);

            // 判斷Job工作狀態(0:停止 1:運行)
            isContinue = CheckJobIsContinue(jobID, ref msgID);

            if (isContinue)
            {
                dateTime = Convert.ToDateTime(replyDate.Substring(0, 4) + "-" + replyDate.Substring(4, 2) + "-" + replyDate.Substring(6, 2));
                fileName = @"CTCBREAuth" + replyDate;

                result = OperatorReplyData(jobID, masterID, fileName + ".ZIP", postOfficeService, eAgentInfo, out successCount, out failCount, out hasFile, out postActiveCount, out errorMsg, out sourceName, replyDate);

                if (errorMsg == "")
                {
                    // FTP 有回覆檔
                    if (hasFile)
                    {
                        if (successCount == 0 && failCount == 0)
                        {
                            JobHelper.Write(jobID, "[SUCCESS] 郵局核印回覆 批次 END " + fileName + " 沒有資料 ", LogState.Info);
                        }
                        else
                        {
                            SendMail(postOfficeService, "批次", jobID, sourceName, fileName, result, successCount, failCount, postActiveCount, true);

                            JobHelper.Write(jobID, "[SUCCESS] 郵局核印回覆 批次 END " + fileName, LogState.Info);
                        }
                    }
                    else
                    {
                        // 無回覆檔
                        JobHelper.Write(jobID, "[SUCCESS] 郵局核印回覆 批次 END 郵局尚未回覆", LogState.Info);
                    }
                }
                else
                {
                    SendMail(postOfficeService, "批次", jobID, sourceName, fileName, errorMsg, successCount, failCount, postActiveCount, false);

                    JobHelper.Write(jobID, "[SUCCESS] 郵局核印回覆 批次 筆數不符 END " + fileName, LogState.Info);
                }
            }

            // 寫入 BatchLog
            InsertBatchLog(jobID, (successCount + failCount), successCount, "S", "");
        }
        catch (Exception ex)
        {
            // 寫入 BatchLog
            InsertBatchLog(jobID, total, success, "F", "發生錯誤：" + ex.Message);

            // 發送 Email
            postOfficeService.SendMail(jobID, "郵局核印回覆:" + fileName + " 失敗！", ex.Message, "失敗", this.StartTime);

            JobHelper.Write(jobID, "[FAIL] 郵局核印回覆 批次 " + fileName + " 發生錯誤：" + ex.Message);
        }
        finally
        {
            JobHelper.Write(jobID, " 郵局核印回覆 批次 Job 結束！", LogState.Info);
            JobHelper.Write(jobID, "================================================================================================================================================", LogState.Info);
        }
    }

    public bool ExecuteManual(string jobID, string replyDate, EntityAGENT_INFO eAgentInfo)
    {
        string fileName = "";
        PostOfficeService postOfficeService = new PostOfficeService(jobID);
        int successCount = 0;
        int failCount = 0;
        int postActiveCount = 0;
        string masterID = "";
        bool result = false;
        DateTime dateTime = new DateTime();
        string hostResult = "";
        bool hasFile = false;
        string errorMsg = "";
        string sourceName = "";

        try
        {
            JobHelper.Write(jobID, "*********** " + jobID + " 郵局核印回覆 手動 START ************** ", LogState.Info);

            dateTime = Convert.ToDateTime(replyDate.Substring(0, 4) + "-" + replyDate.Substring(4, 2) + "-" + replyDate.Substring(6, 2));

            fileName = @"CTCBREAuth" + replyDate;

            hostResult = OperatorReplyData(jobID, masterID, fileName + ".ZIP", postOfficeService, eAgentInfo, out successCount, out failCount, out hasFile, out postActiveCount, out errorMsg, out sourceName, replyDate);

            if (errorMsg == "")
            {
                if (hasFile)
                {
                    if (successCount == 0 && failCount == 0)
                    {
                        JobHelper.Write(jobID, "[SUCCESS] 郵局核印回覆 批次 END " + fileName + " 沒有資料 ", LogState.Info);
                    }
                    else
                    {
                        SendMail(postOfficeService, "手動", jobID, sourceName, fileName, hostResult, successCount, failCount, postActiveCount, true);

                        result = true;

                        JobHelper.Write(jobID, "[SUCCESS] 郵局核印回覆 手動 END " + fileName, LogState.Info);
                    }
                }
                else
                {
                    // 發送 Email
                    postOfficeService.SendMail(jobID, "郵局核印回覆 手動:" + replyDate + " 郵局尚未回覆 無資料", "總筆數:" + (successCount + failCount) + "\n" + hostResult, "失敗", this.StartTime);

                    result = true;

                    // 無回覆檔
                    JobHelper.Write(jobID, "[SUCCESS] 郵局核印回覆 手動 END 郵局尚未回覆", LogState.Info);
                }
            }
            else
            {
                SendMail(postOfficeService, "手動", jobID, sourceName, fileName, errorMsg, successCount, failCount, postActiveCount, false);

                JobHelper.Write(jobID, "[SUCCESS] 郵局核印回覆 批次 筆數不符 END " + fileName, LogState.Info);
            }

        }
        catch (Exception ex)
        {
            // 發送 Email
            postOfficeService.SendMail(jobID, "郵局核印回覆 手動:" + fileName + " 失敗！", ex.Message, "失敗", this.StartTime);

            result = false;

            JobHelper.Write(jobID, "[FAIL] 郵局核印回覆 手動 " + fileName + " 發生錯誤：" + ex.Message);
        }
        finally
        {
            JobHelper.Write(jobID, " 郵局核印回覆 手動 Job 結束！", LogState.Info);
            JobHelper.Write(jobID, "================================================================================================================================================", LogState.Info);
        }

        return result;
    }

    private EntityAGENT_INFO GetAGENT_INFO(JobDataMap jobDataMap)
    {
        EntityAGENT_INFO eAgentInfo = new EntityAGENT_INFO();

        if (jobDataMap != null && jobDataMap.Count > 0)
        {
            eAgentInfo.agent_id = jobDataMap.GetString("userId");
            eAgentInfo.agent_pwd = jobDataMap.GetString("passWord");
            eAgentInfo.agent_id_racf = jobDataMap.GetString("racfId");
            eAgentInfo.agent_id_racf_pwd = jobDataMap.GetString("racfPassWord");
        }

        return eAgentInfo;
    }

    // 判斷Job工作狀態(0:停止 1:運行)
    private bool CheckJobIsContinue(string jobID, ref string msgID)
    {
        bool result = true;
        if (JobHelper.SerchJobStatus(jobID).Equals("") || JobHelper.SerchJobStatus(jobID).Equals("0"))
        {
            // Job停止
            JobHelper.Write(jobID, "[FAIL] Job工作狀態為：停止！");
            result = false;
        }

        // 檢測Job是否在執行中
        try
        {
            DataTable dtInfo = BRL_BATCH_LOG.GetRunningDate(FunctionKey, jobID, "R", ref msgID);
            if (dtInfo == null || dtInfo.Rows.Count > 0) //20210531_Ares_Stanley-修正Job執行檢核條件
            {
                JobHelper.SaveLog("JOB 工作狀態為：正在執行！", LogState.Info);
                // 返回不執行
                result = false;
            }
            else
            {
                // 記錄Job執行資訊
                BRL_BATCH_LOG.InsertRunning(FunctionKey, jobID, StartTime, "R", "");
            }
        }
        catch (Exception ex)
        {
            result = false;
            JobHelper.Write(jobID, "[FAIL] " + ex.ToString());
        }

        return result;
    }

    private string GetReplyInformation(PostOfficeService postOfficeService, out string masterID)
    {
        DateTime dateTime = new DateTime();
        string replyTime = "";
        masterID = "";
        string fileName = "";

        // 算起迄時間
        postOfficeService.GetReplyTime(this.StartTime, out replyTime);

        dateTime = Convert.ToDateTime(replyTime.Substring(0, 4) + "-" + replyTime.Substring(4, 2) + "-" + replyTime.Substring(6, 2));

        DataTable replyInformation = postOfficeService.GetReplyInformation(dateTime);

        if (replyInformation.Rows.Count > 0)
        {
            masterID = replyInformation.Rows[0]["ID"].ToString();
            fileName = replyInformation.Rows[0]["FileName"].ToString();
        }

        return fileName;
    }

    private string OperatorReplyData(string jobID, string masterID, string fileName, PostOfficeService postOfficeService, EntityAGENT_INFO eAgentInfo, out int successCount, out int failCount, out bool hasFile, out int postActiveCount, out string errorMsg, out string sourceName, string replyDate)
    {
        bool isUnZip = false;
        errorMsg = "";
        DataTable replyData = new DataTable();
        DataTable sendHostData = new DataTable();
        DataTable activeData = new DataTable();
        DataTable receiveNumberData = new DataTable();
        string unZipPwd = "";
        string unZipFileName = fileName.Split('.')[0];
        successCount = 0;
        failCount = 0;
        postActiveCount = 0;
        string sendHostResult = "";
        bool isMatch = true;
        hasFile = false;
        sourceName = "";

        // 下載檔案
        string path = postOfficeService.DownloadFile(jobID, fileName, out unZipPwd);

        if (path != "")
        {
            hasFile = true;

            // 解壓縮
            isUnZip = postOfficeService.DecompressFile(jobID, path, fileName, unZipPwd);

            if (isUnZip)
            {
                JobHelper.Write(jobID, fileName + " 解壓縮成功", LogState.Info);

                // 組回覆檔資料
                replyData = postOfficeService.GetReplyFileStream(path + unZipFileName + ".txt", out activeData);

                // 取 masterID
                if (replyData.Rows.Count > 0)
                {
                    receiveNumberData = postOfficeService.GetMasterID(replyData);

                    if (receiveNumberData.Rows.Count > 0)
                    {
                        masterID = receiveNumberData.Rows[0]["ID"].ToString();
                        sourceName = receiveNumberData.Rows[0]["FileName"].ToString();
                        errorMsg = receiveNumberData.Rows[0]["Complete"].ToString() == "True" ? " 檔案重覆執行 " : "";
                        if (errorMsg == "")
                        {
                            string tempReceiveNo = "";

                            if ((replyData.Rows.Count - 1) == receiveNumberData.Rows.Count)
                            {
                                foreach (DataRow receiveNumberRow in receiveNumberData.Rows)
                                {
                                    foreach (DataRow replyRow in replyData.Rows)
                                    {
                                        tempReceiveNo = "";

                                        if (receiveNumberRow["RowNo"].ToString().Trim() == replyRow["RowNo"].ToString().Trim())
                                        {
                                            tempReceiveNo = "";
                                            break;
                                        }
                                        else
                                        {
                                            tempReceiveNo = receiveNumberRow["ReceiveNumber"].ToString().Trim();
                                        }
                                    }

                                    if (tempReceiveNo != "")
                                    {
                                        errorMsg += tempReceiveNo + "\n";
                                        isMatch = false;
                                    }
                                }
                            }
                        }
                    }
                }

                if (!isMatch)
                {
                    errorMsg = "筆數不符，不執行 缺少收編：\n" + errorMsg;
                }

                if (errorMsg == "")
                {
                    // 回寫回覆檔相關
                    sendHostData = postOfficeService.UpdateReplyRelatedTable(masterID, replyData, activeData, out successCount, out failCount, out postActiveCount, replyDate);
                    
                    if (successCount > 0)
                    {
                        JobHelper.Write(jobID, "回寫回覆檔相關 Table 完成", LogState.Info);

                        // 上送主機後，寫入 SendToHost
                        sendHostResult = postOfficeService.SendToHost(sendHostData, eAgentInfo);

                        JobHelper.Write(jobID, "上送主機結果: " + sendHostResult, LogState.Info);
                    }
                }
            }
        }
        else
        {
            hasFile = false;
        }

        return sendHostResult;
    }

    private void InsertBatchLog(string jobID, int total, int success, string status, string message)
    {
        int fail = total - success;

        StringBuilder sbMessage = new StringBuilder();
        sbMessage.Append("總筆數：" + total.ToString() + "。");//*總筆數
        sbMessage.Append("成功筆數：" + success.ToString() + "。");//*成功筆數
        sbMessage.Append("失敗筆數：" + fail.ToString() + "。");//*失敗筆數

        if (message.Trim() != "")
        {
            sbMessage.Append("失敗訊息：" + message);//*失敗訊息
        }

        BRL_BATCH_LOG.Delete("01", jobID, "R");
        BRL_BATCH_LOG.Insert("01", jobID, this.StartTime, status, sbMessage.ToString());
    }

    private void SendMail(PostOfficeService postOfficeService, string type, string jobID, string sourceName, string fileName, string result, int successCount, int failCount, int postActiveCount, bool isSuccess)
    {
        string title = "郵局核印回覆 " + type + " :";
        string body = "";
        string success = isSuccess ? "成功" : "失敗";
        if (sourceName != "")
        {
            title = title + " 原檔名 : " + sourceName;
        }

        title = title + " 回覆檔檔名 : " + fileName;

        if ((successCount + failCount) > 0)
        {
            title = title + " 成功筆數:" + successCount + "、失敗筆數:" + failCount;
            body = body + "總筆數:" + (successCount + failCount) + " \n";
        }

        if (postActiveCount > 0)
        {
            title = title + " 郵局主動終止: " + postActiveCount + " 筆";
        }

        body = body + result;

        if (isSuccess)
        {
            title += " 成功";
        }
        else
        {
            title += " 失敗";
        }

        // 發送 Email
        postOfficeService.SendMail(jobID, title, body, success, this.StartTime);
    }
}
