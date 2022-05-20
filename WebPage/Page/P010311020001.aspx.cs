//******************************************************************
//*  作    者：蘇洺葳
//*  功能說明：郵局報表-R02授權成功/失敗報表
//*  創建日期：2018/10/02
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
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

public partial class P010311020001 : PageBase
{
    #region event

    /// <summary>
    /// 畫面裝載時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 綁定GridView列頭訊息
            ShowControlsText();
            // 拋檔日期
            this.txtSendFileDate.Text = DateTime.Now.ToString("yyyyMMdd");
            this.txtSendFileDate.Focus();
            this.gpList.RecordCount = 0;
            this.gpList.Visible = false;
            this.gvpbR02Record.DataSource = null;
            this.gvpbR02Record.DataBind();

            DataTable dtblPropertyKey = CSIPCommonModel.BusinessRules.BRFORM_COLUMN.GetPostOfficeRtnInfo();
            this.dropPostRtnMsg.Items.Clear();
            ListItem listItem = new ListItem();
            listItem.Text = "";
            listItem.Value = "";
            this.dropPostRtnMsg.Items.Add(listItem);

            for (int i = 0; i < dtblPropertyKey.Rows.Count; i++)
            {
                listItem = new ListItem();
                listItem.Text = dtblPropertyKey.Rows[i][1].ToString();
                listItem.Value = dtblPropertyKey.Rows[i][0].ToString();
                this.dropPostRtnMsg.Items.Add(listItem);
            }
        }

        base.strHostMsg += "";
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.gpList.Visible = true;
        this.gpList.CurrentPageIndex = 0;
        BindGridView();
    }

    /// <summary>
    /// 列印
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        // 匯出到Excel
        OutputExcel();
    }

    /// <summary>
    /// 將查詢結果匯出到Excel文檔
    /// 修改日期: 2020/12/04_Ares_Stanley-新增checkBox參數
    /// </summary>
    protected void OutputExcel()
    {
        try
        {
            string msgID = "";
            string serverPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
            string agentName = ((EntityAGENT_INFO)this.Session["Agent"]).agent_name;
            string sendFileDate = this.txtSendFileDate.Text.Trim().Replace("/", "");
            string sReplyDate = this.txtSReplyDate.Text.Trim().Replace("/", "");
            string eReplyDate = this.txtEReplyDate.Text.Trim().Replace("/", "");
            string postRtnMsg = this.dropPostRtnMsg.SelectedValue;
            string reportType = string.Empty;
            bool successChecked = this.radlSearchType.Items[0].Selected;
            bool failChecked = this.radlSearchType.Items[1].Selected;
            // S.成功 F.失敗 A.成功+失敗
            if (this.radlSearchType.Items[0].Selected)
            {
                reportType = (this.radlSearchType.Items[1].Selected) ? "A" : "S";
            }
            else
            {
                reportType = (this.radlSearchType.Items[1].Selected) ? "F" : "A";
            }

            // 服務器端，生成Excel文檔
            if (!CSIPKeyInGUI.BusinessRules_new.BRExcel_File.CreateExcelFile_R02(sendFileDate, sReplyDate, eReplyDate, reportType, postRtnMsg, agentName, ref serverPathFile, ref msgID, successChecked, failChecked))
            {
                if (msgID != "")
                    base.strClientMsg += MessageHelper.GetMessage(msgID);
                else
                    base.strClientMsg += MessageHelper.GetMessage("01_03110200_004");
                return;
            }

            // 將服務器端生成的文檔，下載到本地。
            string nowDate = DateTime.Now.ToString("yyyyMMdd");
            string fileName = "";
            if (this.radlSearchType.Items[0].Selected && this.radlSearchType.Items[1].Selected ||
               !this.radlSearchType.Items[0].Selected && !this.radlSearchType.Items[1].Selected)
            {
                fileName = BaseHelper.GetShowText("01_03110200_018") + nowDate + ".xls";
            }
            else if (this.radlSearchType.Items[0].Selected)
            {
                fileName = BaseHelper.GetShowText("01_03110200_019") + nowDate + ".xls";
            }
            else if (this.radlSearchType.Items[1].Selected)
            {
                fileName = BaseHelper.GetShowText("01_03110200_020") + nowDate + ".xls";
            }

            // 顯示提示訊息：匯出到Excel文檔資料成功
            this.Session["ServerFile"] = serverPathFile;
            this.Session["ClientFile"] = fileName;
            // string urlString = @"ClientMsgShow('" + MessageHelper.GetMessage("01_03110200_005") + "');";
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("01_03110200_005") + "' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            base.strClientMsg += BaseHelper.ClientMsgShow("01_03110200_004");
        }
    }

    /// <summary>
    /// 頁導航
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }

    /// <summary>
    /// CustGridView行綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbR02Record_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ////  查詢結果的columns[8]+ ":"+ columns[9],若為“：“，顯示”系統內無該錯誤代碼對應訊息“
            //if (e.Row.Cells[9].Text.Trim().Length == 0 && e.Row.Cells[10].Text.Trim().Length == 0)
            //{
            //    e.Row.Cells[9].Text = "系統內無該錯誤代碼對應訊息";
            //}
            //else
            //{
            //    e.Row.Cells[9].Text = e.Row.Cells[9].Text.Trim() + ":" + e.Row.Cells[10].Text.Trim();
            //}

            // 序號 = 當前頁號 * 每頁行數 + 頁內順序號 + 1
            e.Row.Cells[0].Text = Convert.ToString((this.gpList.CurrentPageIndex-1) * this.gpList.PageSize + e.Row.RowIndex + 1);
        }
    }
    #endregion event

    #region function

    /// <summary>
    /// 從Show.xml取漢字，設置畫面控件的Text
    /// </summary>
    private void ShowControlsText()
    {
        this.gvpbR02Record.Columns[0].HeaderText = BaseHelper.GetShowText("01_03110200_008");// 序號
        this.gvpbR02Record.Columns[1].HeaderText = BaseHelper.GetShowText("01_03110200_009");// 收件編號
        this.gvpbR02Record.Columns[2].HeaderText = BaseHelper.GetShowText("01_03110200_010");// 收受行(核印行)
        this.gvpbR02Record.Columns[3].HeaderText = BaseHelper.GetShowText("01_03110200_011");// 收受行名稱(核印行)
        this.gvpbR02Record.Columns[4].HeaderText = BaseHelper.GetShowText("01_03110200_012");// 委繳戶統編\身分證字號
        this.gvpbR02Record.Columns[5].HeaderText = BaseHelper.GetShowText("01_03110200_013");// 委繳戶帳號
        this.gvpbR02Record.Columns[6].HeaderText = BaseHelper.GetShowText("01_03110200_014");// 持卡人ID
        this.gvpbR02Record.Columns[7].HeaderText = BaseHelper.GetShowText("01_03110200_015");// 申請類別
        this.gvpbR02Record.Columns[8].HeaderText = BaseHelper.GetShowText("01_03110200_016");// 成功/失敗
        this.gvpbR02Record.Columns[9].HeaderText = BaseHelper.GetShowText("01_03110200_017");// 回覆訊息

        this.radlSearchType.Items[0].Text = BaseHelper.GetShowText("01_03110200_003");// 成功
        this.radlSearchType.Items[1].Text = BaseHelper.GetShowText("01_03110200_004");// 失敗

        // 設置每頁顯示記錄最大條數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gvpbR02Record.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
    }

    /// <summary>
    /// 綁定GridView數據源
    /// </summary>
    /// <param name="dirValues"></param>
    private void BindGridView()
    {
        try
        {
            int intTotolCount = 0;
            string uploadDate = this.txtSendFileDate.Text.Trim().Replace("/", "");
            string sReplyDate = this.txtSReplyDate.Text.Trim().Replace("/", "");
            string eReplyDate = this.txtEReplyDate.Text.Trim().Replace("/", "");
            string postRtnMsg = this.dropPostRtnMsg.SelectedValue;
            string reportType = string.Empty;
            // S.成功 F.失敗 A.成功+失敗
            if (this.radlSearchType.Items[0].Selected)
            {
                reportType = (this.radlSearchType.Items[1].Selected) ? "A" : "S";
            }
            else
            {
                reportType = (this.radlSearchType.Items[1].Selected) ? "F" : "A";
            }

            // R02授權成功/失敗報表PostOffice_Temp
            DataSet dstSearchResult = CSIPKeyInGUI.BusinessRules_new.BRPostOffice_Temp.SearchR02Record(uploadDate, sReplyDate, eReplyDate, reportType, postRtnMsg, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref intTotolCount);

            if (dstSearchResult == null)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_03110200_002");
                this.gpList.RecordCount = 0;
                this.gvpbR02Record.DataSource = null;
                this.gvpbR02Record.DataBind();
            }
            else
            {
                this.gpList.RecordCount = intTotolCount;
                this.gvpbR02Record.DataSource = dstSearchResult.Tables[0];
                this.gvpbR02Record.DataBind();

                if (intTotolCount == 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_03110200_001");
                }
                else
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_03110200_003");
                }
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            base.strClientMsg += MessageHelper.GetMessage("01_03110200_002");
        }
    }

    #endregion function
}
