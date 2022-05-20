//******************************************************************
//*  作    者：占偉林(James)
//*  功能說明：系統維護-ACH屬性設定
//*  創建日期：2009/10/26
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
using CSIPCommonModel.BaseItem;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using Framework.Common.Logging;
using Framework.Common.Utility;

public partial class P010205000001 : PageBase
{
    #region event
    /// 作者 占偉林
    /// 創建日期：2009/10/26
    /// 修改日期：2009/10/26 
    /// <summary>
    /// 畫面裝載時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //* 綁定GridView列頭訊息
            ShowControlsText();
            //* 綁定布告欄訊息
            this.ViewState["DateSend"] = "";//* 送檔日
            this.ViewState["DateInput"] = "";//* 首錄日
            this.ViewState["DateReceive"] = "";//* 收檔日
            this.ViewState["type"] = "";
            BindGridView(this.ViewState["type"].ToString());
        }
        base.strHostMsg += "";
    }

    /// 作者 占偉林
    /// 創建日期：2009/10/26
    /// 修改日期：2009/10/26 
    /// <summary>
    /// CustGridView行綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbACH_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //* 已送P02
            if (e.Row.Cells[3].Text == "Y")
                e.Row.Cells[3].Text = "Y";
            else
                e.Row.Cells[3].Text = "N";

            //* 回貼主機
            if (e.Row.Cells[4].Text.Trim() != "")
            {
                if (Convert.ToInt32(e.Row.Cells[4].Text) > 0)
                {
                    e.Row.Cells[4].Text = "Y";
                }
                else
                {
                    e.Row.Cells[4].Text = "N";
                }
            }
            else
                e.Row.Cells[4].Text = "Y";

            //* 總筆數
            if (e.Row.Cells[5].Text.Trim() != "")
                e.Row.Cells[5].Text = Convert.ToInt32(e.Row.Cells[5].Text.Trim()).ToString("N0");
        }
    }

    /// 作者 占偉林
    /// 創建日期：2009/10/26
    /// 修改日期：2009/10/26 
    /// <summary>
    /// 頁導航
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView(this.ViewState["type"].ToString());
    }

    /// 作者 占偉林
    /// 創建日期：2009/10/26
    /// 修改日期：2009/10/26 
    /// <summary>
    /// 點選【新增】按鈕時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        //* 取畫面的【送檔日】、【首錄日】、【收檔日】
        string strDateSend = this.txtDateSend.Text.Trim();
        string strDateInput = this.txtDateInput.Text.Trim();
        string strDateReceive = this.txtDateReceive.Text.Trim();

        string strMsgID = "";
        string strRtnMsg = "";
        if (BRBATCH.Add(strDateSend, strDateInput, strDateReceive, ref strMsgID, ref strRtnMsg))
        {
            //* 記錄添加成功，顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("01_04030000_005");
            this.ViewState["DateSend"] = "";//* 送檔日
            this.ViewState["DateInput"] = "";//* 首錄日
            this.ViewState["DateReceive"] = "";//* 收檔日
            this.ViewState["type"] = "";
            BindGridView(this.ViewState["type"].ToString());
        }
        else
        {
            //* 記錄添加失敗，顯示端末訊息
            if (strMsgID == "01_04030000_003" && strRtnMsg.Length!=0)
            {
                base.strClientMsg += strRtnMsg;
            }
            else
            {
                base.strClientMsg += MessageHelper.GetMessage(strMsgID);
            }
            
        }
    }

    /// 作者 占偉林
    /// 創建日期：2009/10/26
    /// 修改日期：2009/10/26 
    /// <summary>
    /// 點選【查詢】按鈕時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        //* 取畫面的【送檔日】、【首錄日】、【收檔日】
        string strDateSend = this.txtDateSend.Text.Trim();
        string strDateInput = this.txtDateInput.Text.Trim();
        string strDateReceive = this.txtDateReceive.Text.Trim();

        this.ViewState["DateSend"] = strDateSend;
        this.ViewState["DateInput"] = strDateInput;
        this.ViewState["DateReceive"] = strDateReceive;
        this.gpList.CurrentPageIndex = 0;
        this.ViewState["type"] = "search";
        BindGridView(this.ViewState["type"].ToString());
    }

    /// 作者 占偉林
    /// 創建日期：2009/10/26
    /// 修改日期：2009/10/26 
    /// <summary>
    /// 點選【Reset】按鈕時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        //* 取畫面的【首錄日】
        string strDateInput = this.txtDateInput.Text.Trim();

        string strMsgID = "";
        if (BRBATCH.Reset(strDateInput, ref strMsgID))
        {
            //* 資料Reset成功，顯示端末訊息
            base.strClientMsg += string.Format(MessageHelper.GetMessage("01_04030000_011"), this.txtDateInput.Text.Trim());
            this.ViewState["DateSend"] = "";//* 送檔日
            this.ViewState["DateInput"] = "";//* 首錄日
            this.ViewState["DateReceive"] = "";//* 收檔日
            this.ViewState["type"] = "";
            BindGridView(this.ViewState["type"].ToString());
        }
        else
        {
            //* 資料Reset失敗，顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage(strMsgID);
        }
    }
    #endregion event

    #region function
    /// 作者 占偉林
    /// 創建日期：2009/10/26
    /// 修改日期：2009/10/26 
    /// <summary>
    /// 綁定GridView數據源
    /// </summary>
    private void BindGridView(string strType)
    {
        try
        {
            string strMsgID = "";
            int intTotolCount = 0;
            DataTable dtblBatch = null;
            dtblBatch = (DataTable)BRBATCH.SearchBatch(this.ViewState["DateSend"].ToString(), this.ViewState["DateInput"].ToString(),
                        this.ViewState["DateReceive"].ToString(), this.gpList.CurrentPageIndex, this.gpList.PageSize, ref intTotolCount, ref strMsgID);
            if (dtblBatch == null)
            {
                //* 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage(strMsgID);
                this.gpList.RecordCount = 0;
                this.gvpbACH.DataSource = null;
                this.gvpbACH.DataBind();
            }
            else
            {
                this.gpList.RecordCount = intTotolCount;
                this.gvpbACH.DataSource = dtblBatch;
                this.gvpbACH.DataBind();
                //* 顯示端末訊息
                if (strType == "search" && intTotolCount==0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_04030000_012");
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtDateInput"));
                }
                else
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_04030000_002");
                }
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("01_04030000_001");
        }
    }

    /// 作者 占偉林
    /// 創建日期：2009/10/26
    /// 修改日期：2009/10/26 
    /// <summary>
    /// 從Show.xml取漢字，設置畫面控件的Text
    /// </summary>
    private void ShowControlsText()
    {
        this.gvpbACH.Columns[0].HeaderText = BaseHelper.GetShowText("01_04030000_008");
        this.gvpbACH.Columns[1].HeaderText = BaseHelper.GetShowText("01_04030000_009");
        this.gvpbACH.Columns[2].HeaderText = BaseHelper.GetShowText("01_04030000_010");
        this.gvpbACH.Columns[3].HeaderText = BaseHelper.GetShowText("01_04030000_011");
        this.gvpbACH.Columns[4].HeaderText = BaseHelper.GetShowText("01_04030000_012");
        this.gvpbACH.Columns[5].HeaderText = BaseHelper.GetShowText("01_04030000_013");
        //* 設置每頁顯示記錄最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gvpbACH.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
    }
    #endregion function
}
