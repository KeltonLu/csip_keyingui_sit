//******************************************************************
//* 作    者：
//* 功能說明：
//* 創建日期：
//* 修改紀錄：
//* <author>            <time>            <TaskID>                <desc>
//* Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*******************************************************************

using System;
using System.Data;
using System.Configuration;
using Quartz;
using CSIPCommonModel.BusinessRules;
using System.Text;
using CSIPCommonModel.EntityLayer;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Logging;
using Framework.Common.Utility;

/// <summary>
/// BatchJob_GetBranchInfo 的摘要描述
/// 收總/分公司資料
/// 檔名：AML_E_HCOP_yyyyMMdd / AML_E_BRCH_yyyyMMdd 
/// 20191125-BatchJob_GetAMLInfo：匯入AML RMM E檔無總公司資料檔！請IT協助的主旨請改成「AML 總公司檔 批次」，內容請改成只檢核當月E檔無總公司資料檔再出EMIAL通知，不要檢核歷史檔
/// </summary>
public class BatchJob_GetBranchInfo : Quartz.IJob
{
    protected string FunctionKey = ConfigurationManager.AppSettings["FunctionKey"].ToString();
    protected DateTime StartTime = DateTime.Now;// 記錄Job啟動時間
    protected DateTime EndTime;
    protected JobHelper JobHelper = new JobHelper();

    //20191227-RQ-2019-030155-002-批次信函調整 by Peggy
    protected string _MailTitleHCOP = "AML總公司檔匯入 批次：";
    protected string _MailTitleBRCH = "AML分公司檔匯入 批次：";

    public void Execute(JobExecutionContext context)
    {
        string jobID = context.JobDetail.JobDataMap["jobid"].ToString();
        JobHelper.strJobID = jobID;
        string fileName = "";
        bool isContinue = true;
        string msgID = string.Empty;
        int total = 0;
        int total2 = 0;

        JobDataMap jobDataMap = context.JobDetail.JobDataMap;
        EntityAGENT_INFO eAgentInfo = GetAGENT_INFO(jobDataMap);

        AMLBranchInformationService aMLBranchInformationService = new AMLBranchInformationService(jobID, eAgentInfo);
        try
        {
            JobHelper.Write(jobID, "*********** " + jobID + " 取得 總分公司(BatchJob_GetBranchInfo) 資料 批次 START ************** ", LogState.Info);

            

            string date = DateTime.Now.ToString("yyyyMMdd");

            //date = "20190324";

            // 判斷Job工作狀態(0:停止 1:運行)
            //確認是否有人員可以提供分派功能，若沒有人員可以分派發信提醒並且不執行匯入分公司總公司功能。

            if (BRAML_File_Import.IsDispatchOK())
            {

                isContinue = CheckJobIsContinue(jobID, ref msgID);

                if (isContinue)
                {
                    // 20200031-CSIP EOS Ares Luke 修改日期:2021/03/12 修改說明:業務需求排程結束清空FilInfo參數
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

                                if (BRM_FileInfo.UpdateParam(jobID, tempDt.ToString("yyyyMMdd")))
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

                    DownloadFileAndInsertTable(jobID, aMLBranchInformationService, date, out total, out total2);
                }
            }
            else
            {
                //加起來不是100%，需要寫出log並且發信請維護人員維護
                JobHelper.Write(jobID, "[FAIL]  自動派案出現問題，加總比率加總沒有設定100%或0%");
                // 發送 Email
                aMLBranchInformationService.SendMail("自動派案出現問題", "自動派案出現問題，加總比率加總沒有設定100%或0%，請至系統維護!!", "", this.StartTime);

            }
        }
        catch (Exception ex)
        {
            JobHelper.Write(jobID, "[FAIL]  取得 總分公司(BatchJob_GetBranchInfo) 資料 批次 " + fileName + " 發生錯誤：" + ex.Message);
            //20210618_Ares_Stanley-Exception時，將L_BATCH_LOG的Status設為"F"，避免Status保持在"R"之後無法運行批次
            BRL_BATCH_LOG.Delete("01", jobID, "R");
            BRL_BATCH_LOG.Insert("01", jobID, this.StartTime, "F", ex.Message);
            // 發送 Email
            aMLBranchInformationService.SendMail("AML 總分公司(BatchJob_GetBranchInfo) 檔 批次:" + fileName + " 失敗！", "總筆數:" + total, ex.Message, this.StartTime);
        }
        finally
        {
            // 20200031-CSIP EOS Ares Luke 修改日期:2021/03/12 修改說明:業務需求排程結束清空FilInfo參數
            // 清除 FileInfo Parameter值
            BRM_FileInfo.UpdateParam(jobID, "");

            JobHelper.Write(jobID, "  取得 AML 總分公司(BatchJob_GetBranchInfo) 檔資料 批次 Job 結束！", LogState.Info);
            JobHelper.Write(jobID, "================================================================================================================================================", LogState.Info);
        }
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

    private string SetErrorMsg(string errorDatMsg, string errorCtlMsg, string errorMsg)
    {
        if (errorDatMsg != "")
        {
            return errorDatMsg;
        }

        if (errorCtlMsg != "")
        {
            return errorCtlMsg;
        }

        if (errorMsg != "")
        {
            return errorMsg;
        }

        return "";
    }

    private string DownloadFileAndInsertTable(string jobID, AMLBranchInformationService aMLBranchInformationService, string date, out int total, out int total2)
    {
        string folderName = string.Empty;
        string errorDatMsg = "";
        string errorCtlMsg = "";
        string errorMsg = "";
        string errorMsg2 = "";

        bool isInsertOK = false;
        DataTable dat = new DataTable();
        DataTable dat2 = new DataTable();
        DataTable datManager = new DataTable();
        DataTable checkcase = new DataTable();

        total = 0; // 記錄分公司檔案筆數
        total2 = 0; // 記錄總公司檔案筆數

        //20191230-RQ-2019-030155-002-批次信函調整：增加取得取檔結果的參數 by Peggy
        bool isDownloadHCOPOK = false; 
        bool isDownloadBRCHOK = false;
        //20201028-202012RC
        int BuildDateErrorCount = 0;
        string _BuildDateError = string.Empty;//記錄分公司設立日期有問題筆數
        bool BuildDateErrorFlag = false;

        JobHelper.CreateFolderName(jobID, ref folderName);

        string localPath = AppDomain.CurrentDomain.BaseDirectory + "FileDownload\\" + jobID + "\\" + folderName + "\\";

        //20191230-RQ-2019-030155-002-批次信函調整：增加取得取檔結果的參數 by Peggy
        /*
        string fileNameDat = aMLBranchInformationService.DownloadFromFTP(date, localPath, "dat");
        string fileNameDat2 = "";
        string fileNameCtl = aMLBranchInformationService.DownloadFromFTP(date, localPath, "ctl");
        */
        string fileNameDat = aMLBranchInformationService.DownloadFromFTP(date, localPath, "dat", ref isDownloadBRCHOK);
        string fileNameCtl = aMLBranchInformationService.DownloadFromFTP(date, localPath, "ctl", ref isDownloadBRCHOK);
        // 取 Branch 檔內容

        dat = aMLBranchInformationService.GetBranchFileData(localPath, fileNameDat, out errorMsg);

        errorMsg = SetErrorMsg(errorDatMsg, errorCtlMsg, errorMsg);

        total = dat.Rows.Count;

        // 取 總公司 檔內容
        //20191230-RQ-2019-030155-002-批次信函調整：增加取得取檔結果的參數 by Peggy
        /*
        string fileNameDat2 = aMLBranchInformationService.DownloadFromFTPMaster(date, localPath, "dat", "AML_E_HCOP_yyyyMMdd");
        string fileNameCtl2 = aMLBranchInformationService.DownloadFromFTPMaster(date, localPath, "ctl", "AML_E_HCOP_yyyyMMdd");
        */
        string fileNameDat2 = aMLBranchInformationService.DownloadFromFTPMaster(date, localPath, "dat", "AML_E_HCOP_yyyyMMdd", ref isDownloadHCOPOK);
        string fileNameCtl2 = aMLBranchInformationService.DownloadFromFTPMaster(date, localPath, "ctl", "AML_E_HCOP_yyyyMMdd", ref isDownloadHCOPOK);

        dat2 = aMLBranchInformationService.GetMasterFileData(localPath, fileNameDat2, out errorMsg2);

        // 判斷分公司檔案是否已存在
        bool isFileExist = BRAML_File_Import.IsEFileExist(fileNameDat);

        // 判斷總公司檔案是否已存在
        bool isFileExist2 = BRAML_File_Import.IsEFileExist(fileNameDat2);

        errorMsg2 = SetErrorMsg(errorDatMsg, errorCtlMsg, errorMsg2);

        if (errorMsg == "" && errorMsg2 == "")
        {
            // 將分公司資訊寫入資料庫
            if (isFileExist == false && isFileExist2 == false)
            {
                isInsertOK = aMLBranchInformationService.SetRelationDataTable(dat, fileNameDat);

                errorMsg = SetErrorMsg(errorDatMsg, errorCtlMsg, errorMsg);

                total2 = dat2.Rows.Count;

                //檢核是否有誤
                if (errorMsg2 == "")
                {
                    // 將總公司資訊寫入資料庫
                    isInsertOK = aMLBranchInformationService.SetRelationDataTableMaster(dat2, fileNameDat2, fileNameDat);
                    //擷取高階管理人資訊並且寫入資料庫
                    datManager = aMLBranchInformationService.GetMasterFileManagerData(localPath, fileNameDat2, out errorMsg);
                }
                else
                {
                    BRAML_File_Import.RecoveryBranchData2(fileNameDat, fileNameDat2);
                }
            }
            else
            {
                if (isFileExist == true)
                {
                    JobHelper.Write(jobID, fileNameDat + "此檔案已經匯入過");
                    aMLBranchInformationService.SendMail("AML 分公司檔 批次:" + fileNameDat + " 失敗！", fileNameDat + "此檔案已經匯入過", errorMsg, this.StartTime);
                }

                if (isFileExist2 == true)
                {
                    JobHelper.Write(jobID, fileNameDat2 + "此檔案已經匯入過");
                    aMLBranchInformationService.SendMail("AML 總公司檔 批次:" + fileNameDat2 + " 失敗！", fileNameDat2 + "此檔案已經匯入過", errorMsg, this.StartTime);
                }
            }


            JobHelper.Write(jobID, "刪除分公司與總公司統編相同的分公司資料", LogState.Info);

            BRFORM_COLUMN.DeleteDulidateBRCHtable();




            JobHelper.Write(jobID, "回壓分公司CASE NO", LogState.Info);
            foreach (DataRow row in dat.Rows)
            {
                string RMMBatchNo = row["BRCH_BATCH_NO"].ToString();
                string AMLInternalID = row["BRCH_INTER_ID"].ToString();
                //20201028-202012RC
                string _BuildDate = row["BRCH_BUILD_DATE"].ToString().Trim();

                DataTable GetEtable = new DataTable();
                GetEtable = BRFORM_COLUMN.GetCaseNo(RMMBatchNo, AMLInternalID);
                if (GetEtable.Rows.Count > 0)
                {
                    BRFORM_COLUMN.UpdateBRCHtable(GetEtable.Rows[0]["CASE_NO"].ToString(), RMMBatchNo, AMLInternalID);
                }

                //20201028
                if (_BuildDate.Trim().Equals("00000000") || string.IsNullOrEmpty(_BuildDate))
                {
                    BuildDateErrorFlag = true;
                    
                    _BuildDateError = _BuildDateError + row["BRCH_BRCH_NO"].ToString().Trim() + " / " + row["BRCH_HQ_BRCH_NO"].ToString().Trim() + " , ";

                    BuildDateErrorCount++;
                }

            }
            JobHelper.Write(jobID, "回壓分公司BATCH NO完成", LogState.Info);

            JobHelper.Write(jobID, "回壓總公司CASE NO", LogState.Info);
            foreach (DataRow row in dat2.Rows)
            {
                string RMMBatchNo = row["HCOP_BATCH_NO"].ToString();
                string AMLInternalID = row["HCOP_INTER_ID"].ToString();

                DataTable GetEtable = new DataTable();
                GetEtable = BRFORM_COLUMN.GetCaseNo(RMMBatchNo, AMLInternalID);

                if (GetEtable.Rows.Count > 0)
                {
                    BRFORM_COLUMN.UpdateHQtable(GetEtable.Rows[0]["CASE_NO"].ToString(), RMMBatchNo, AMLInternalID);
                }
            }

            JobHelper.Write(jobID, "回壓總公司BATCH NO完成", LogState.Info);

            JobHelper.Write(jobID, "回壓高階經理人CASE NO", LogState.Info);

            datManager = BRFORM_COLUMN.GetManager();

            foreach (DataRow row in datManager.Rows)
            {
                string RMMBatchNo = row["HCOP_BATCH_NO"].ToString().Trim();
                string AMLInternalID = row["HCOP_INTER_ID"].ToString().Trim();

                DataTable GetEtable = new DataTable();
                GetEtable = BRFORM_COLUMN.GetCaseNo(RMMBatchNo, AMLInternalID);

                if (GetEtable.Rows.Count > 0)
                {
                    BRFORM_COLUMN.UpdateManagertable(GetEtable.Rows[0]["CASE_NO"].ToString(), RMMBatchNo, AMLInternalID);
                }
            }

            JobHelper.Write(jobID, "回壓高階經理人CASE NO完成", LogState.Info);

            JobHelper.Write(jobID, "回壓 長姓名(負責人,聯絡人,高管) 開始", LogState.Info);
            aMLBranchInformationService.SetLname(fileNameDat2);
            JobHelper.Write(jobID, "回壓 長姓名 結束", LogState.Info);

            // 20210527 EOS_AML(NOVA) by Ares Dennis
            #region 退版機制
            DataTable dt = new DataTable();
            CSIPCommonModel.BusinessRules_new.BRM_PROPERTY_CODE.GetEnableProperty("01", new string[] { "OLD_VERSION_FLAG" }, ref dt);
            string flag = "";
            if (dt.Rows.Count > 0)
            {
                flag = dt.Rows[0]["PROPERTY_CODE"].ToString();
            }
            #endregion
            if (flag == "N")// 新版程式碼
            {
                #region 批次吃檔分組                
                JobHelper.Write(jobID, "批次吃檔分組", LogState.Info);

                // AML_HQ_Work 分組
                foreach (DataRow row in dat2.Rows)
                {
                    string batchNo = row["HCOP_BATCH_NO"].ToString();
                    string interID = row["HCOP_INTER_ID"].ToString();
                    string corpNo = row["HCOP_HEADQUATERS_CORP_NO"].ToString();
                    string caseProcessStatus = row["CaseProcess_Status"].ToString();

                    bool isSuccess = BRFORM_COLUMN.UpdateHQtable(batchNo, interID, corpNo, caseProcessStatus);
                    if (isSuccess)
                    {
                        //取得分組案件寫LOG
                        DataTable dtCases = BRFORM_COLUMN.GetCases(batchNo, interID, corpNo, caseProcessStatus);
                        for (int i = 0; i < dtCases.Rows.Count; i++)
                        {
                            string caseNo = dtCases.Rows[i]["CASE_NO"].ToString();
                            string groupNo = dtCases.Rows[i]["GROUP_NO"].ToString();
                            JobHelper.Write(jobID, "批次吃檔分組 CASE_NO: " + caseNo + ", GROUP_NO: " + groupNo, LogState.Info);
                        }
                    }
                }

                DateTime now = DateTime.Now;
                string createDate = now.ToString("yyyyMMdd");
                // AML_HQ_Manager_Import, AML_HQ_Manager_Work 分組
                BRFORM_COLUMN.UpdateHQManagertable(createDate);

                JobHelper.Write(jobID, "批次吃檔分組完成", LogState.Info);                
                #endregion
            }

            JobHelper.Write(jobID, "檢查每個案件是否都有總公司資料開始", LogState.Info);
            //檢查每個案件是否都有總公司資料
            checkcase = BRFORM_COLUMN.checkcase();
            if (checkcase.Rows.Count > 0)
            {
                string checkcustomerid = "";
                foreach (DataRow rowcase in checkcase.Rows)
                {
                    checkcustomerid = checkcustomerid + rowcase["customerid"].ToString() + ",";
                }

                //20191125-RQ-2018-015749-002 調整mail內容 by Peggy
                //JobHelper.Write(jobID, "匯入AML RMM E檔無總公司資料檔！請IT協助 共" + checkcase.Rows.Count + "件統一編號為:" + checkcustomerid);

                //20210720_Stanley_香琦需求進行調整 
                //JobHelper.Write(jobID, "AML 總公司檔 批次 共" + checkcase.Rows.Count + "件統一編號為:" + checkcustomerid, LogState.Info);
                //JobHelper.Write(jobID, "AML總公司檔匯入與AML E檔檢核 批次 失敗! 有E檔卻無總公司資料檔 共" + checkcase.Rows.Count + "件統一編號為:" + checkcustomerid, LogState.Info);
                JobHelper.Write(jobID, "執行結果：有E檔但主機總公司檔未提供此統編（因無任何分公司屬於此統編），共" + checkcase.Rows.Count + "件，統一編號為:" + checkcustomerid+"\r請確認是否鍵檔統編資料有誤！若是，請修正完成並執行AML關戶作業，完成關戶後，請上AML系統將RMM結案，並註明清楚原因", LogState.Info);

                //20191125-RQ-2018-015749-002 調整mail內容 by Peggy
                //aMLBranchInformationService.SendMail("匯入AML RMM E檔無總公司資料檔！請IT協助", "共" + checkcase.Rows.Count + "件統一編號為:" + checkcustomerid, errorMsg, this.StartTime);

                //20210720_Rick_香琦需求進行調整 
                //aMLBranchInformationService.SendMail("AML 總公司檔 批次", "共" + checkcase.Rows.Count + "件統一編號為:" + checkcustomerid, errorMsg, this.StartTime);
                aMLBranchInformationService.SendMail("AML總公司檔匯入與AML E檔檢核 批次 失敗!總筆數:" + checkcase.Rows.Count + "筆", "有E檔但主機總公司檔未提供此統編（因無任何分公司屬於此統編），共" + checkcase.Rows.Count + "件統一編號為:" + checkcustomerid + "<br>請確認是否鍵檔統編資料有誤！若是，請修正完成並執行AML關戶作業，完成關戶後，請上AML系統將RMM結案，並註明清楚原因", "失敗", this.StartTime);
                /*
                  主旨:AML總公司匯入與AML E檔檢核 批次:失敗 筆數:1筆
                  狀態:失敗
                  執行結果：主機總公司檔未提供此統編，共一件，統一編號為：9667990,
                  請確認是否鍵檔統編資料有誤！若是，請修正完成並執行AML關戶作業，完成關戶後，請上AML系統將RMM結案，並註明清楚原因
                */
            }

            JobHelper.Write(jobID, "檢查每個案件是否都有總公司資料結束", LogState.Info);
        }

        // 發送失敗 mail
        if (errorMsg != "" || errorMsg2 != "" || !isInsertOK || !isDownloadHCOPOK || !isDownloadBRCHOK)//20191230-RQ-2019-030155-002-批次信函調整：增加取得取檔結果的參數 by Peggy
        {
            // 寫入 L_BATCH_LOG
            InsertBatchLog(jobID, total, 0, "F", errorMsg);
            //BRAML_File_Import.RecoveryBranchData2(fileNameDat, fileNameDat2);

            if (isDownloadHCOPOK && isDownloadBRCHOK)//有收到檔案
            {
                JobHelper.Write(jobID, "AML 分公司檔 批次:" + fileNameDat + " 失敗！總筆數:" + total + errorMsg);
                // 發送 Email
                //aMLBranchInformationService.SendMail("AML 分公司檔 批次:" + fileNameDat + " 失敗！", "總筆數:" + total, errorMsg, this.StartTime);
                aMLBranchInformationService.SendMail(_MailTitleBRCH + " 失敗！總筆數：" + total + "筆", _MailTitleBRCH + fileNameDat + "失敗！總筆數：" + total + "筆", errorMsg, this.StartTime);

                JobHelper.Write(jobID, "AML 總公司檔 批次:" + fileNameDat2 + " 失敗！總筆數:" + total2 + errorMsg);
                // 發送 Email
                //aMLBranchInformationService.SendMail("AML 總公司檔 批次:" + fileNameDat2 + " 失敗！", "總筆數:" + total2, errorMsg2, this.StartTime);
                aMLBranchInformationService.SendMail(_MailTitleHCOP + " 失敗！總筆數：" + total2 + "筆", _MailTitleHCOP + fileNameCtl2 + "失敗！總筆數：" + total2 + "筆", errorMsg2, this.StartTime);
            }
            else//未收到檔案//20191230-RQ-2019-030155-002-批次信函調整：增加取得取檔結果的參數 by Peggy
            {
                if (!isDownloadHCOPOK)
                {
                    JobHelper.Write(jobID, "AML 總公司檔 批次:" + fileNameDat2 + " 失敗！未收到總公司檔");
                    // 發送 Email
                    aMLBranchInformationService.SendMail(_MailTitleHCOP + " 失敗！未收到檔案", _MailTitleHCOP + fileNameCtl2 + "匯入批次失敗，未收到總公司檔", "失敗", this.StartTime);
                }
                if (!isDownloadBRCHOK)
                {
                    JobHelper.Write(jobID, "AML 分公司檔 批次:" + fileNameDat + " 失敗！未收到分公司檔");
                    // 發送 Email
                    aMLBranchInformationService.SendMail(_MailTitleBRCH + " 失敗！未收到檔案", _MailTitleBRCH + fileNameDat + " 匯入批次失敗！，未收到分公司檔", "失敗", this.StartTime);
                }
            }
        }
        else
        {

            // 寫入 L_BATCH_LOG
            InsertBatchLog(jobID, total, total, "S", "");

            //20201228-202012RC 如分公司統編為00000000或空白，則於MAIL通知
            string _hasEmptyCorpNo = string.Empty;
            if (BuildDateErrorFlag)
            {
                _hasEmptyCorpNo = "，設立日期有誤共" + BuildDateErrorCount + "筆，分公司統編/總公司統編：" + _BuildDateError;
            }

            JobHelper.Write(jobID, "AML 分公司檔 批次:" + fileNameDat + " 成功！總筆數:" + total + "，匯入成功共" + total + "筆，匯入失敗O筆" + _hasEmptyCorpNo, LogState.Info);
            // 發送 Email
            //aMLBranchInformationService.SendMail("AML 分公司檔 批次:" + fileNameDat + " 成功！", "總筆數:" + total + "，匯入成功共" + total + "筆，匯入失敗O筆", errorMsg, this.StartTime);
            aMLBranchInformationService.SendMail(_MailTitleBRCH + " 成功！總筆數：" + total + "筆", _MailTitleBRCH + fileNameDat + "成功！總筆數：" + total + "筆" + _hasEmptyCorpNo, "成功", this.StartTime);

            JobHelper.Write(jobID, "AML 總公司檔 批次:" + fileNameDat2 + " 成功！總筆數:" + total2 + "，匯入成功共" + total2 + "筆，匯入失敗O筆", LogState.Info);
            // 發送 Email
            //aMLBranchInformationService.SendMail("AML 總公司檔 批次:" + fileNameDat2 + " 成功！", "總筆數:" + total2 + "，匯入成功共" + total2 + "筆，匯入失敗O筆", errorMsg2, this.StartTime);
            aMLBranchInformationService.SendMail(_MailTitleHCOP + " 成功！總筆數：" + total2 + "筆", _MailTitleHCOP + fileNameCtl2 + "成功！總筆數：" + total2 + "筆", "成功", this.StartTime);


        }

        //判斷是否有查長姓名資料但卻沒有資料的ID, 若有就要提示
        string[] _NoLname_HCOP = aMLBranchInformationService.getNoLnameArray("HCOP");
        string[] _NoLname_BRCH = aMLBranchInformationService.getNoLnameArray("BRCH");

        if (_NoLname_HCOP.Length > 0 || _NoLname_BRCH.Length > 0)
        {
            JobHelper.Write(jobID, "有 未對應到長姓名的資料,請參考 .error 內文");
            StringBuilder sb = new StringBuilder();
            string tmpStr = string.Empty;
            sb.AppendLine("");
            if (_NoLname_HCOP.Length > 0)
            {
                tmpStr = string.Format("   AML {0} 批次:{1} 無對應長姓名資料, 其BATCH_NO+INTER_ID:{2} ", "總公司檔", fileNameDat2, string.Join(",", _NoLname_HCOP));
                sb.AppendLine(tmpStr);
                JobHelper.WriteError(jobID, tmpStr);
            }
            if (_NoLname_BRCH.Length > 0)
            {
                tmpStr = string.Format("   AML {0} 批次:{1} 無對應長姓名資料, 其統編:{2} ", "分公司檔", fileNameDat, string.Join(",", _NoLname_BRCH));
                sb.AppendLine(tmpStr);
                JobHelper.WriteError(jobID, tmpStr);
            }

            // 發送 Email
            string mailTitle = "AML 批次 無 對應長姓名資料 :" + DateTime.Now.ToString("yyyyMMdd") + " ";
            string mailBody = "下列統編裡的資料 無對應長姓名資料, 請檢核:" + sb.ToString();
            aMLBranchInformationService.SendMail(mailTitle, mailBody, "", this.StartTime);
        }

        //取總公司資料
        return fileNameDat;

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
}
