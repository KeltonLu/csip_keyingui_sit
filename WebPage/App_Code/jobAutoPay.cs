//******************************************************************
//*  作    者：
//*  功能說明：
//*  創建日期：
//*  修改紀錄：
//*  <author>          <time>              <TaskID>                <desc>
//* Ares_jhun          2022/09/28          RQ-2022-019375-000      EDDA需求調整：調整鍵檔日期、整合EDDA資料
//*******************************************************************

using System;
using System.Data;
using System.Data.SqlClient;
using Quartz;
using Framework.Common.Logging;
using CSIPCommonModel.BusinessRules;
using CSIPKeyInGUI.BusinessRules;
using CSIPNewInvoice.EntityLayer_new;

/// <summary>
/// 排程-信用卡自動扣款資料上傳給卡主機
/// </summary>
public class jobAutoPay : IJob
{
    public void Execute(JobExecutionContext context)
    {
        const string strFuncKey = "01";
        string strJobId = "";
        DateTime dtStart = DateTime.Now;
        string fileName = string.Format("withholding_{0:yyyyMMdd}.txt", dtStart);

        try
        {
            strJobId = context.JobDetail.JobDataMap.GetString("job" + "id").Trim();
            string strMsgId = "";

            // 查詢資料檔L_BATCH_LOG，查看是否上次作業還未停止
            DataTable dtInfo = BRL_BATCH_LOG.GetRunningDate(strFuncKey, strJobId, "R", ref strMsgId);
            if (dtInfo == null || dtInfo.Rows.Count > 0)
            {
                return;
            }

            // 判斷是否為假日
            if (!BRWORK_DATE.IS_WORKDAY("01", DateTime.Now.ToString("yyyyMMdd")))
            {
                return;
            }

            // 開始批次作業
            if (!BRL_BATCH_LOG.InsertRunning(strFuncKey, strJobId, dtStart, "R", ""))
            {
                return;
            }
            
            // 將 【Other_Bank_Temp.Pcmc_Return_Code】 IN ('ERROR:9', '9000', '9001', '9002') 資料新增到 【Auto_Pay_Status】
            // 上述條件中 ERROR:9 為EDDA改版前週期件
            if (BRAuto_pay_status.CopyOtherBankTempToAutoPayStatus())
            {
                // 取得核印成功需上傳主機的資料
                DataSet ds = BRAuto_pay_status.GetWithholdingData();
                // 產生給卡主機的檔案 withholding_yyyyMMdd.txt
                BRAuto_pay_status.BatchOutput(ds.Tables[0], fileName);
                // 將檔案上傳至ftp
                BRAuto_pay_status.UploadToFtp(fileName);
                // 上傳成功後更新【EDDA_Auto_Pay】
                UpdateEddaAutoPayUploadFlag();
            }

            string strMsg = strJobId + "執行於:" + DateTime.Parse(context.FireTimeUtc.ToString()).AddHours(8).ToString();
            if (context.NextFireTimeUtc.HasValue)
            {
                strMsg += "  ;下次執行於:" + DateTime.Parse(context.NextFireTimeUtc.ToString()).AddHours(8).ToString();
            }

            Logging.Log(strMsg, strJobId, LogLayer.DB);

            // 刪除排程執行中的記錄
            BRL_BATCH_LOG.Delete(strFuncKey, strJobId, "R");
            // 新增排程執行成功的記錄
            BRL_BATCH_LOG.Insert(strFuncKey, strJobId, dtStart, "S", "");
        }
        catch (Exception ex)
        {
            // 刪除排程執行中的記錄
            BRL_BATCH_LOG.Delete(strFuncKey, strJobId, "R");
            // 新增排程執行失敗的記錄
            BRL_BATCH_LOG.Insert(strFuncKey, strJobId, dtStart, "F", ex.Message);
            Logging.Log(ex, strJobId, LogLayer.DB);
        }
    }

    /// <summary>
    /// 更新 EDDA_Auto_Pay 上傳狀態與上傳時間
    /// </summary>
    /// <returns></returns>
    private void UpdateEddaAutoPayUploadFlag()
    {
        string strSql = @"UPDATE EDDA_Auto_Pay SET UploadFlag = '1', UploadTime = GETDATE()
                          WHERE UploadFlag = '0' AND ComparisonStatus <> '0' AND Reply_Info IN ('A0', 'A4');";

        SqlCommand sqlComm = new SqlCommand();
        sqlComm.CommandType = CommandType.Text;
        sqlComm.CommandText = strSql;

        BRBase<Entity_SP>.Update(sqlComm, "Connection_System");
    }
}