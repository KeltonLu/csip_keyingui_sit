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
using Framework.Common.Utility;

public partial class P010202000001 : PageBase
{
    #region event
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ShowControlsText();
            this.gpList.RecordCount = 0;
            this.gpList.Visible = false;
            this.gvList.DataSource = null;
            this.gvList.DataBind();
            txtInputDateStart.Text = DateTime.Now.ToString("yyyyMMdd");
            txtInputDateEnd.Text = DateTime.Now.ToString("yyyyMMdd");
        }
        base.strHostMsg += "";
        base.strClientMsg += "";
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.gpList.CurrentPageIndex = 0;
        BindGridView();
        gpList.Visible = true;
    }

    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }

    protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        string strNow = DateTime.Now.ToString("yyyyMMdd");

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if ("Y" == e.Row.Cells[2].Text.Trim().ToUpper())
            {
                e.Row.Cells[2].Text = BaseHelper.GetShowText("01_02020000_007");    //成功
            }
            else
            {
                if (e.Row.Cells[1].Text.Trim() == strNow)
                {
                    if (int.Parse(DateTime.Now.ToString("HHmmss")) <　170000)
                    {
                        e.Row.Cells[2].Text = BaseHelper.GetShowText("01_02020000_009");    //未完成
                        return;
                    }
                }

                e.Row.Cells[2].Text = BaseHelper.GetShowText("01_02020000_008");    //失敗
            }
        }
    }
    #endregion event

    #region method
    /// <summary>
    /// 從Show.xml取漢字，設置畫面控件的Text
    /// </summary>
    private void ShowControlsText()
    {
        this.gvList.Columns[0].HeaderText = BaseHelper.GetShowText("01_02020000_004");
        this.gvList.Columns[1].HeaderText = BaseHelper.GetShowText("01_02020000_005");
        this.gvList.Columns[2].HeaderText = BaseHelper.GetShowText("01_02020000_006");

        //* 設置每頁顯示記錄最大條數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gvList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
    }

    private void BindGridView()
    {
        int iTotalCount = 0;

        gvList.DataSource = BRRedeemUpload.SelectLOG(txtInputDateStart.Text.Trim(), txtInputDateEnd.Text.Trim(), txtReceiveNumber.Text.Trim(), gpList.CurrentPageIndex, gpList.PageSize, ref iTotalCount);
        gpList.RecordCount = iTotalCount;
        gvList.DataBind();

    }
    #endregion method


}
