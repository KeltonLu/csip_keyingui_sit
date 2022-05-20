//******************************************************************
//*  作    者：蘇洺葳
//*  功能說明：郵局報表-批次作業量統計報表
//*  創建日期：2018/10/02
//*  修改紀錄：
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Text;
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using Framework.Common.Logging;
using Framework.Common.Utility;

public partial class P010311040001 : PageBase
{
    #region event

    /// <summary>
    /// 畫面裝載時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        EnableButtons();
        if (!IsPostBack)
        {
            // 拋檔日期
            this.txtSendFileDate.Text = DateTime.Now.ToString("yyyyMMdd");
            this.txtSendFileDate.Focus();
            this.radBatchStatics.Text = BaseHelper.GetShowText("01_03110400_004");
            this.radBatchResult.Text = BaseHelper.GetShowText("01_03110400_006");
        }

        base.strHostMsg += "";
    }

    /// <summary>
    /// 列印
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        // 匯出到Excel.
        OutputExcel("1");
    }

    /// <summary>
    /// 成功報表
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrintSuccess_Click(object sender, EventArgs e)
    {
        // 匯出到Excel.
        OutputExcel("2");
    }

    /// <summary>
    /// 失敗報表
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrintFault_Click(object sender, EventArgs e)
    {
        // 匯出到Excel.
        OutputExcel("3");
    }
    
    /// <summary>
    /// 將查詢結果匯出到Excel文檔
    /// </summary>
    protected void OutputExcel(string type)
    {
        try
        {
            string msgID = "";
            string serverPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
            string agentName = ((EntityAGENT_INFO)this.Session["Agent"]).agent_name;
            string sendFileDate = this.txtSendFileDate.Text.Trim().Replace("/", "");
            // 服務器端，生成Excel文檔
            if (!CSIPKeyInGUI.BusinessRules_new.BRExcel_File.CreateExcelFile_Batch(sendFileDate, type, agentName, ref serverPathFile, ref msgID))
            {
                if (msgID != "")
                    base.strClientMsg += MessageHelper.GetMessage(msgID);
                else
                    base.strClientMsg += MessageHelper.GetMessage("01_03110400_004");
                return;
            }

            // 將服務器端生成的文檔，下載到本地。
            string nowDate = DateTime.Now.ToString("yyyyMMdd");
            string fileName = "";
            switch (type)
            {
                case "1":
                    fileName = BaseHelper.GetShowText("01_03110400_001") + nowDate + ".xls";
                    break;
                case "2":
                    fileName = BaseHelper.GetShowText("01_03110400_011") + nowDate + ".xls";
                    break;
                case "3":
                    fileName = BaseHelper.GetShowText("01_03110400_012") + nowDate + ".xls";
                    break;
            }

            // 顯示提示訊息：匯出到Excel文檔資料成功
            this.Session["ServerFile"] = serverPathFile;
            this.Session["ClientFile"] = fileName;
            // string urlString = @"ClientMsgShow('" + MessageHelper.GetMessage("01_03110400_005") + "');";
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("01_03110400_005") + "' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            base.strClientMsg += BaseHelper.ClientMsgShow("01_03110400_004");
        }
    }

    /// <summary>
    /// 【批次作業量統計】RadioButton點選時
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void radBatchStatics_CheckedChanged(object sender, EventArgs e)
    {
        this.radBatchResult.Checked = !this.radBatchStatics.Checked;
        EnableButtons();
    }

    /// <summary>
    /// 【批次結果報表】RadioButton點選時
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void radBatchResult_CheckedChanged(object sender, EventArgs e)
    {
        this.radBatchStatics.Checked = !this.radBatchResult.Checked;
        EnableButtons();
    }

    #endregion event

    #region Function

    /// <summary>
    /// 設置畫面Button按鈕的Enable屬性
    /// </summary>
    private void EnableButtons()
    {
        if (!this.radBatchStatics.Checked)
        {
            this.btnPrint.Enabled = false;
        }
        else
        {
            this.btnPrint.Enabled = true;
        }

        if (!this.radBatchResult.Checked)
        {
            this.btnPrintFault.Enabled = false;
            this.btnPrintSuccess.Enabled = false;
        }
        else
        {
            this.btnPrintFault.Enabled = true;
            this.btnPrintSuccess.Enabled = true;
        }
    }

    #endregion
}
