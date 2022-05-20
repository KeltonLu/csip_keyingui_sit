//******************************************************************
//*  作    者：
//*  功能說明：請款簽單明細表-另開新視窗顯示明細資料
//*  創建日期：2014/08/11
//*  修改紀錄：
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
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
using System.Drawing;
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using Framework.Common.Logging;
using Framework.Common.Utility;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BaseItem;

public partial class P010107010002 : PageBase
{
    string strBatchDate = string.Empty;
    string strReceiveBatch = string.Empty;
    string strBatchNO = string.Empty;
    string strShopID = string.Empty;
    string strSignType = string.Empty;

    #region event
    protected void Page_Load(object sender, EventArgs e)
    {
        # region 取從上一畫面傳過來的參數
        //* 編批日期
        if (!string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "BatchDate")))
        {
            strBatchDate = RedirectHelper.GetDecryptString(this.Page, "BatchDate");
        }
        //* 收件批次
        if (!string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "ReceiveBatch")))
        {
            strReceiveBatch = RedirectHelper.GetDecryptString(this.Page, "ReceiveBatch");
        }
        //* 批號
        if (!string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "BatchNO")))
        {
            strBatchNO = RedirectHelper.GetDecryptString(this.Page, "BatchNO");
        }
        //* 商店代號
        if (!string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "ShopID")))
        {
            strShopID = RedirectHelper.GetDecryptString(this.Page, "ShopID");
        }
        //* 簽單類別
        if (!string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "SignType")))
        {
            strSignType = RedirectHelper.GetDecryptString(this.Page, "SignType");
        }
        #endregion

        this.Title = "編批日期:" + strBatchDate + " | 收件批次:" + strReceiveBatch + " | 批號:" + strBatchNO + " | 商店代號:" + strShopID + " | 簽單類別:" + (strSignType == "1" ? "一般簽單" : "分期簽單");

        if (!IsPostBack)
        {
            //* 綁定GridView列頭訊息
            ShowControlsText();
            this.gpList.RecordCount = 0;
            this.gpList.Visible = false;
            this.gvpbSearchRecord.DataSource = null;
            this.gvpbSearchRecord.DataBind();
        }

        this.gpList.CurrentPageIndex = 0;
        BindGridView();
        this.gpList.Visible = true;
    }

    protected void gpList_PageChanged(object src, PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView();
    }

    protected void gvpbSearchRecord_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells[7].Text == "40")
                e.Row.Cells[7].Text = e.Row.Cells[7].Text + "請款";
            if (e.Row.Cells[7].Text == "41")
                e.Row.Cells[7].Text = e.Row.Cells[7].Text + "退款";
            if (e.Row.Cells[8].Text == "0")
                e.Row.Cells[8].Text = "正常件";
            if (e.Row.Cells[8].Text == "1")
                e.Row.Cells[8].Text = "踢退件";
        }
    }
    #endregion event

    #region function
    /// <summary>
    /// 從Show.xml取漢字，設置畫面控件的Text
    /// </summary>
    private void ShowControlsText()
    {
        this.gvpbSearchRecord.Columns[0].HeaderText = BaseHelper.GetShowText("01_01070100_026");
        this.gvpbSearchRecord.Columns[1].HeaderText = BaseHelper.GetShowText("01_01070100_027");
        this.gvpbSearchRecord.Columns[2].HeaderText = BaseHelper.GetShowText("01_01070100_028");
        this.gvpbSearchRecord.Columns[3].HeaderText = BaseHelper.GetShowText("01_01070100_029");
        this.gvpbSearchRecord.Columns[4].HeaderText = BaseHelper.GetShowText("01_01070100_030");
        this.gvpbSearchRecord.Columns[5].HeaderText = BaseHelper.GetShowText("01_01070100_031");
        this.gvpbSearchRecord.Columns[6].HeaderText = BaseHelper.GetShowText("01_01070100_032");
        this.gvpbSearchRecord.Columns[7].HeaderText = BaseHelper.GetShowText("01_01070100_033");
        this.gvpbSearchRecord.Columns[8].HeaderText = BaseHelper.GetShowText("01_01070100_034");
        this.gvpbSearchRecord.Columns[9].HeaderText = BaseHelper.GetShowText("01_01070100_035");
        this.gvpbSearchRecord.Columns[10].HeaderText = BaseHelper.GetShowText("01_01070100_036");
        this.gvpbSearchRecord.Columns[11].HeaderText = BaseHelper.GetShowText("01_01070100_025");

        //* 設置每頁顯示記錄最大條數
        int iPageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gpList.PageSize = iPageSize;
        this.gvpbSearchRecord.PageSize = iPageSize;
    }

    /// <summary>
    /// 綁定GridView數據源
    /// </summary>
    private void BindGridView()
    {
        try
        {
            string strMsgID = string.Empty;
            int intTotolCount = 0;

            DataTable dt = BRASExport.SearchASData_Detail(strBatchDate, strReceiveBatch, strBatchNO, strShopID, strSignType, ref strMsgID, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref intTotolCount);
            if (dt == null)
            {
                this.gpList.RecordCount = 0;
                this.gvpbSearchRecord.DataSource = null;
                this.gvpbSearchRecord.DataBind();
            }
            else
            {
                this.gpList.RecordCount = intTotolCount;
                this.gvpbSearchRecord.DataSource = dt;
                this.gvpbSearchRecord.DataBind();
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //* 顯示查詢失敗訊息
            MessageHelper.ShowMessage(this.UpdatePanel1, "01_01070100_001");
        }
    }
    #endregion function
}
