//******************************************************************
//*  作    者：占偉林(James)
//*  功能說明：報表-批次作業量統計報表
//*  創建日期：2009/12/16
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

public partial class P010305040001 : PageBase
{
    #region event

    /// 作者 占偉林


    /// 創建日期：2009/12/16
    /// 修改日期：2009/12/16 
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
            string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
            strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
            //* 【首錄起迄日】起
            this.txtInputDateStart.Text = strYYYYMMDD;
            //* 【首錄起迄日】迄
            this.txtInputDateEnd.Text = strYYYYMMDD;
            //* 設置光標
            this.txtInputDateStart.Focus();
            this.radBatchStatics.Text = BaseHelper.GetShowText("01_03050400_004");
            this.radBatchResult.Text = BaseHelper.GetShowText("01_03050400_006");
        }
        base.strHostMsg += "";
    }

    /// 作者 占偉林

    /// 創建日期：2009/12/16
    /// 修改日期：2009/12/16 
    /// <summary>
    /// 點選畫面【列印】按鈕時的處理

    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        Dictionary<string, string> dirValues = new Dictionary<string, string>();

        //* 首錄起迄日

        dirValues.Add("txtInputDateStart", this.txtInputDateStart.Text.Trim());
        dirValues.Add("txtInputDateEnd", this.txtInputDateEnd.Text.Trim());

        string strType = "1";
        //* 匯出到Excel.
        OutputExcel(dirValues, strType);
    }

    /// 作者 占偉林

    /// 創建日期：2009/12/16
    /// 修改日期：2009/12/16 
    /// <summary>
    /// 點選畫面【成功報表】按鈕時的處理

    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrintSuccess_Click(object sender, EventArgs e)
    {
        Dictionary<string, string> dirValues = new Dictionary<string, string>();

        //* 首錄起迄日

        dirValues.Add("txtInputDateStart", this.txtInputDateStart.Text.Trim());
        dirValues.Add("txtInputDateEnd", this.txtInputDateEnd.Text.Trim());

        string strType = "2";
        //* 匯出到Excel.
        OutputExcel(dirValues, strType);
    }

    /// 作者 占偉林

    /// 創建日期：2009/12/16
    /// 修改日期：2009/12/16 
    /// <summary>
    /// 點選畫面【失敗報表】按鈕時的處理

    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrintFault_Click(object sender, EventArgs e)
    {
        Dictionary<string, string> dirValues = new Dictionary<string, string>();

        //* 首錄起迄日

        dirValues.Add("txtInputDateStart", this.txtInputDateStart.Text.Trim());
        dirValues.Add("txtInputDateEnd", this.txtInputDateEnd.Text.Trim());

        string strType = "3";
        //* 匯出到Excel.
        OutputExcel(dirValues, strType);
    }

    /// 作者 占偉林

    /// 創建日期：2009/12/16
    /// 修改日期：2009/12/16 
    /// <summary>
    /// 點選畫面【未完成報表】按鈕時的處理

    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrintNoComplete_Click(object sender, EventArgs e)
    {
        Dictionary<string, string> dirValues = new Dictionary<string, string>();

        //* 首錄起迄日

        dirValues.Add("txtInputDateStart", this.txtInputDateStart.Text.Trim());
        dirValues.Add("txtInputDateEnd", this.txtInputDateEnd.Text.Trim());

        string strType = "4";
        //* 匯出到Excel.
        OutputExcel(dirValues, strType);
    }

    /// 作者 占偉林

    /// 創建日期：2009/12/16
    /// 修改日期：2009/12/16 
    /// <summary>
    /// 點選畫面【全部】按鈕時的處理

    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrintAll_Click(object sender, EventArgs e)
    {
        Dictionary<string, string> dirValues = new Dictionary<string, string>();

        //* 首錄起迄日

        dirValues.Add("txtInputDateStart", this.txtInputDateStart.Text.Trim());
        dirValues.Add("txtInputDateEnd", this.txtInputDateEnd.Text.Trim());

        string strType = "5";
        //* 匯出到Excel.
        OutputExcel(dirValues, strType);
    }
    
    /// <summary>
    /// 將查詢結果匯出到Excel文檔。

    /// </summary>
    /// <param name="dirValues">Dictionary<查詢條件></param>
    /// <param name="strType">匯出的文檔類型</param>
    protected void OutputExcel(Dictionary<string, string> dirValues, string strType)
    {
        try
        {
            string strMsgID = "";
            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
            //* 服務器端，生成Excel文檔
            if (!BRExcel_File.CreateExcelFile_Batch(dirValues,
                            strType,
                            ((EntityAGENT_INFO)this.Session["Agent"]).agent_name,
                            ref strServerPathFile,
                            ref strMsgID))
            {
                if (strMsgID != "")
                    base.strClientMsg += MessageHelper.GetMessage(strMsgID);
                else
                    base.strClientMsg += MessageHelper.GetMessage("01_03050400_004");
                return;
            }

            //* 將服務器端生成的文檔，下載到本地。

            string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
            strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
            string strFileName = "";
            switch (strType)
            {
                case "1":
                    strFileName = BaseHelper.GetShowText("01_03050400_001") + strYYYYMMDD + ".xls";
                    break;
                case "2":
                    strFileName = BaseHelper.GetShowText("01_03050400_011") + strYYYYMMDD + ".xls";
                    break;
                case "3":
                    strFileName = BaseHelper.GetShowText("01_03050400_012") + strYYYYMMDD + ".xls";
                    break;
                case "4":
                    strFileName = BaseHelper.GetShowText("01_03050400_013") + strYYYYMMDD + ".xls";
                    break;
                case "5":
                    strFileName = BaseHelper.GetShowText("01_03050400_014") + strYYYYMMDD + ".xls";
                    break;
            }

            //* 顯示提示訊息：匯出到Excel文檔資料成功
            this.Session["ServerFile"] = strServerPathFile;
            this.Session["ClientFile"] = strFileName;
            // string urlString = @"ClientMsgShow('" + MessageHelper.GetMessage("01_03050400_005") + "');";
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("01_03050400_005") + "' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            base.strClientMsg += BaseHelper.ClientMsgShow("01_03050400_004");
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
            this.btnPrintAll.Enabled = false;
            this.btnPrintFault.Enabled = false;
            this.btnPrintNoComplete.Enabled = false;
            this.btnPrintSuccess.Enabled = false;
        }
        else
        {
            this.btnPrintAll.Enabled = true;
            this.btnPrintFault.Enabled = true;
            this.btnPrintNoComplete.Enabled = true;
            this.btnPrintSuccess.Enabled = true;
        }
    }
    #endregion

}
