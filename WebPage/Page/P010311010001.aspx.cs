//******************************************************************
//*  作    者：蘇洺葳
//*  功能說明：郵局報表-郵局授權扣款資料清單
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

public partial class P010311010001 : PageBase
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
            this.gvpbACHRecord.DataSource = null;
            this.gvpbACHRecord.DataBind();
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
        // 匯出到Excel.
        OutputExcel();
    }

    /// <summary>
    /// 補跑批次
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReRunBatch_Click(object sender, EventArgs e)
    {
        bool isSendFileDate = CheckSendFileDate();
        string endTime = this.txtSendFileDate.Text;
        bool result = false;

        if (isSendFileDate)
        {
            result = new BatchJob_SendToPost().ExecuteManual("SendToPost_Manual", endTime);

            if (result)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_03110100_006");
            }
            else
            {
                base.strClientMsg += MessageHelper.GetMessage("01_03110100_007");
            }

        }
    }

    /// <summary>
    /// 補抓檔案
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReGetFile_Click(object sender, EventArgs e)
    {
        string endTime = this.txtSendFileDate.Text;
        string replyDate = this.txtReplyFileDate.Text;

        if (replyDate.Length != 8)
        {
            base.strClientMsg += MessageHelper.GetMessage("01_03110100_010");
            return;
        }

        DateTime dt = new DateTime();
        bool isDateTime = DateTime.TryParse(replyDate.Substring(0, 4) + "-" + replyDate.Substring(4, 2) + "-" + replyDate.Substring(6, 2), out dt);
        bool result = false;

        if (!isDateTime)
        {
            base.strClientMsg += MessageHelper.GetMessage("01_03110100_011");
        }
        else
        {
            EntityAGENT_INFO eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];

            result = new BatchJob_PostReply().ExecuteManual("PostReply_Manual", replyDate, eAgentInfo);

            if (result)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_03110100_008");
            }
            else
            {
                base.strClientMsg += MessageHelper.GetMessage("01_03110100_009");
            }

        }
    }

    /// <summary>
    /// 將查詢結果匯出到Excel文檔
    /// </summary>
    protected void OutputExcel()
    {
        try
        {
            string msgID = "";
            string serverPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
            string agentName = ((EntityAGENT_INFO)this.Session["Agent"]).agent_name;
            string sendFileDate = this.txtSendFileDate.Text.Trim().Replace("/", "");
            // 服務器端，生成Excel文檔
            if (!CSIPKeyInGUI.BusinessRules_new.BRExcel_File.CreateExcelFile_ACH(sendFileDate, agentName, ref serverPathFile, ref msgID))
            {
                if (msgID != "")
                    base.strClientMsg += MessageHelper.GetMessage(msgID);
                else
                    base.strClientMsg += MessageHelper.GetMessage("01_03110100_004");
                return;
            }

            // 將服務器端生成的文檔，下載到本地。
            string nowDate = DateTime.Now.ToString("yyyyMMdd");
            string fileName = BaseHelper.GetShowText("01_03110100_001") + nowDate + ".xls";

            // 顯示提示訊息：匯出到Excel文檔資料成功
            this.Session["ServerFile"] = serverPathFile;
            this.Session["ClientFile"] = fileName;
            // string urlString = @"ClientMsgShow('" + MessageHelper.GetMessage("01_03110100_005") + "');";
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("01_03110100_005") + "' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            base.strClientMsg += MessageHelper.GetMessage("01_03110100_004");
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
    protected void gvpbACHRecord_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            // 序號 = 當前頁號 * 每頁行數 + 頁內順序號 + 1
            e.Row.Cells[0].Text = Convert.ToString((this.gpList.CurrentPageIndex - 1) * this.gpList.PageSize + e.Row.RowIndex + 1);
        }
    }

    #endregion event

    #region function

    /// <summary>
    /// 從Show.xml取漢字，設置畫面控件的Text
    /// </summary>
    private void ShowControlsText()
    {
        this.gvpbACHRecord.Columns[0].HeaderText = BaseHelper.GetShowText("01_03110100_006");// 序號
        this.gvpbACHRecord.Columns[1].HeaderText = BaseHelper.GetShowText("01_03110100_007");// 收件編號
        this.gvpbACHRecord.Columns[2].HeaderText = BaseHelper.GetShowText("01_03110100_008");// 收受行(核印行)
        this.gvpbACHRecord.Columns[3].HeaderText = BaseHelper.GetShowText("01_03110100_009");// 委繳戶統編\身分證字號
        this.gvpbACHRecord.Columns[4].HeaderText = BaseHelper.GetShowText("01_03110100_010");// 委繳戶帳號
        this.gvpbACHRecord.Columns[5].HeaderText = BaseHelper.GetShowText("01_03110100_011");// 申請類別
        this.gvpbACHRecord.Columns[6].HeaderText = BaseHelper.GetShowText("01_03110100_012");// 處理註記
        this.gvpbACHRecord.Columns[7].HeaderText = BaseHelper.GetShowText("01_03110100_013");// 回覆日期
        this.gvpbACHRecord.Columns[8].HeaderText = BaseHelper.GetShowText("01_03110100_014");// 郵局回覆碼
        this.gvpbACHRecord.Columns[9].HeaderText = BaseHelper.GetShowText("01_03110100_015");// 生效碼【Y/N】

        // 設置每頁顯示記錄最大條數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gvpbACHRecord.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
    }

    /// <summary>
    /// 綁定GridView數據源
    /// </summary>
    private void BindGridView()
    {
        try
        {
            int intTotolCount = 0;
            string uploadDate = this.txtSendFileDate.Text.Trim().Replace("/", "");
            // 取得郵局授權扣款資料清單PostOffice_Temp
            DataSet dstSearchResult = CSIPKeyInGUI.BusinessRules_new.BRPostOffice_Temp.SearchACHRecord(uploadDate, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref intTotolCount);

            if (dstSearchResult == null)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_03110100_002");
                this.gpList.RecordCount = 0;
                this.gvpbACHRecord.DataSource = null;
                this.gvpbACHRecord.DataBind();
            }
            else
            {
                this.gpList.RecordCount = intTotolCount;
                this.gvpbACHRecord.DataSource = dstSearchResult.Tables[0];
                this.gvpbACHRecord.DataBind();

                if (intTotolCount == 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_03110100_001");
                }
                else
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_03110100_003");
                }
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            base.strClientMsg += MessageHelper.GetMessage("01_03110100_002");
        }
    }

    private bool CheckSendFileDate()
    {
        string endTime = this.txtSendFileDate.Text;
        DateTime dt = new DateTime();

        if (endTime == "")
        {
            return false;
        }

        if (endTime.Length != 8)
        {
            return false;
        }

        return DateTime.TryParse(endTime.Substring(0, 4) + "-" + endTime.Substring(4, 2) + "-" + endTime.Substring(6, 2), out dt);
    }
    #endregion function
}
