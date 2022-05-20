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
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Message;
using Framework.Common.Logging;
using Framework.Common.Utility;

public partial class P010306000001 : PageBase
{
    #region event

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //* 綁定GridView列頭訊息
            ShowControlsText();

            this.txtInputDateStart.Text = DateTime.Now.ToString("yyyyMMdd");
            this.txtInputDateEnd.Text = DateTime.Now.ToString("yyyyMMdd");
            this.txtInputDateStart.Focus();
            this.gpList.RecordCount = 0;
            this.gpList.Visible = false;
            this.gvpbP02Record.DataSource = null;
            this.gvpbP02Record.DataBind();
        }
        base.strHostMsg += "";
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.gpList.Visible = true;
        this.gpList.CurrentPageIndex = 0;
        BindGridView();
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        OutputExcel();
    }

    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }
    #endregion event



    #region method
    private void ShowControlsText()
    {
        this.gvpbP02Record.Columns[0].HeaderText = BaseHelper.GetShowText("01_03060000_005");
        this.gvpbP02Record.Columns[1].HeaderText = BaseHelper.GetShowText("01_03060000_006");
        this.gvpbP02Record.Columns[2].HeaderText = BaseHelper.GetShowText("01_03060000_007");
        this.gvpbP02Record.Columns[3].HeaderText = BaseHelper.GetShowText("01_03060000_008");
        this.gvpbP02Record.Columns[4].HeaderText = BaseHelper.GetShowText("01_03060000_009");
        this.gvpbP02Record.Columns[5].HeaderText = BaseHelper.GetShowText("01_03060000_010");
        this.gvpbP02Record.Columns[6].HeaderText = BaseHelper.GetShowText("01_03060000_011");
        this.gvpbP02Record.Columns[7].HeaderText = BaseHelper.GetShowText("01_03060000_012");
        this.gvpbP02Record.Columns[8].HeaderText = BaseHelper.GetShowText("01_03060000_013");

        //* 設置每頁顯示記錄最大條數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gvpbP02Record.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
    }

    /// <summary>
    /// 綁定GridView數據源

    /// </summary>
    private void BindGridView()
    {
        int intTotolCount = 0;
        gvpbP02Record.DataSource = BRRedeemSet.SelectReport(txtInputDateStart.Text.Trim(), txtInputDateEnd.Text.Trim(), gpList.CurrentPageIndex, gpList.PageSize, ref intTotolCount);
        gpList.RecordCount = intTotolCount;
        gvpbP02Record.DataBind();
    }

    protected void OutputExcel()
    {
        try
        {
            string strMsgID = "";
            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());

            if (!BRExcel_File.CreateExcelFile_PointSummary(txtInputDateStart.Text.Trim(),txtInputDateEnd.Text.Trim(),ref strServerPathFile,ref strMsgID))
            {
                if (strMsgID != "")
                    base.strClientMsg += MessageHelper.GetMessage(strMsgID);
                else
                    base.strClientMsg += MessageHelper.GetMessage("01_03060000_001");
                return;
            }

            //* 將服務器端生成的文檔，下載到本地。
            string strFileName = BaseHelper.GetShowText("01_03060000_001") + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            //* 顯示提示訊息：匯出到Excel文檔資料成功
            this.Session["ServerFile"] = strServerPathFile;
            this.Session["ClientFile"] = strFileName;
            // string urlString = @"ClientMsgShow('" + MessageHelper.GetMessage("01_03060000_002") + "');";
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("01_03060000_002") + "' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);

        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            base.strClientMsg += BaseHelper.ClientMsgShow("01_03060000_001");
        }
    }
    #endregion method


}
