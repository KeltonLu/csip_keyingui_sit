﻿//******************************************************************
//*  作    者：占偉林(James)
//*  功能說明：系統維護-公布欄查詢
//*  創建日期：2009/10/16
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

public partial class P010401000001 : PageBase
{
    #region event
    /// 作者 占偉林
    /// 創建日期：2009/10/16
    /// 修改日期：2009/10/16 
    /// <summary>
    /// 畫面裝載時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            //* 綁定GridView列頭訊息
            ShowControlsText();
            //* 綁定布告欄訊息
            BindGridView();
        }
        base.strHostMsg += "";
    }

    /// 作者 占偉林
    /// 創建日期：2009/10/16
    /// 修改日期：2009/10/16 
    /// <summary>
    /// 從Show.xml取漢字，設置畫面控件的Text
    /// </summary>
    private void ShowControlsText()
    {
        this.gvpbBoard.Columns[0].HeaderText = BaseHelper.GetShowText("01_04010000_002");
        this.gvpbBoard.Columns[1].HeaderText = BaseHelper.GetShowText("01_04010000_003");
        this.gvpbBoard.Columns[2].HeaderText = BaseHelper.GetShowText("01_04010000_004");
        this.gvpbBoard.Columns[3].HeaderText = BaseHelper.GetShowText("01_04010000_005");
        this.gvpbBoard.Columns[4].HeaderText = BaseHelper.GetShowText("01_04010000_006");

        //* 設置每頁顯示記錄最大條數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSizeG").ToString());
        this.gvpbBoard.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSizeG").ToString());
    }

    /// 作者 占偉林
    /// 創建日期：2009/10/16
    /// 修改日期：2009/10/16 
    /// <summary>
    /// CustGridView行綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbBoard_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //* 訊息狀態
            if (e.Row.Cells[3].Text == "Y")
                e.Row.Cells[3].Text = BaseHelper.GetShowText("01_04010000_007");
            else
                e.Row.Cells[3].Text = "";
            //* 發送時間
            if (e.Row.Cells[4].Text != "" && e.Row.Cells[4].Text.Length >= 12)
                e.Row.Cells[4].Text = e.Row.Cells[4].Text.Substring(0, 12);
            else
                e.Row.Cells[4].Text = "";

        }
    }

    /// 作者 占偉林
    /// 創建日期：2009/10/16
    /// 修改日期：2009/10/16 
    /// <summary>
    /// 頁導航
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView();
    }
    #endregion event

    #region function
    /// 作者 占偉林
    /// 創建日期：2009/10/16
    /// 修改日期：2009/10/16 
    /// <summary>
    /// 綁定GridView數據源
    /// </summary>
    private void BindGridView()
    {
        try
        {
            string strMsgID = "";
            int intTotolCount = 0;
            DataTable dtblBoard = (DataTable)BRBoard.SearchBoard(this.gpList.CurrentPageIndex, this.gpList.PageSize, ref intTotolCount, ref strMsgID);
            if (dtblBoard == null && strMsgID != "")
            {
                //* 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage(strMsgID);
                this.gpList.RecordCount = 0;
                this.gvpbBoard.DataSource = null;
                this.gvpbBoard.DataBind();
            }
            else
            {
                this.gpList.RecordCount = intTotolCount;
                this.gvpbBoard.DataSource = dtblBoard;
                this.gvpbBoard.DataBind();
                //* 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage("01_04010000_001");
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("01_04010000_002");
        }
    }
    #endregion function
}
