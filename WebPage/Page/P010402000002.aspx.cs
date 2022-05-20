//******************************************************************
//*  作    者：占偉林(James)
//*  功能說明：系統維護-公布欄查詢
//*  創建日期：2009/10/16
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
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
using CSIPCommonModel.EntityLayer;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using Framework.Common.Logging;
using Framework.Common.Utility;

public partial class P010402000002 : PageBase
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
        }
        //* 顯示端末主機訊息
        base.strHostMsg += "";
    }

    /// 作者 占偉林
    /// 創建日期：2009/10/16
    /// 修改日期：2009/10/16 
    /// <summary>
    /// 檔【訊息狀態-否】狀態改變時
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void rabImportant_status_no_CheckedChanged(object sender, EventArgs e)
    {
        if (this.rabImportant_status_no.Checked)
            this.rabImportant_status_yes.Checked = false;
        else
            this.rabImportant_status_yes.Checked = true;
    }

    /// 作者 占偉林
    /// 創建日期：2009/10/16
    /// 修改日期：2009/10/16 
    /// <summary>
    /// 檔【訊息狀態-是】狀態改變時
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void rabImportant_status_yes_CheckedChanged(object sender, EventArgs e)
    {
        if (this.rabImportant_status_yes.Checked)
            this.rabImportant_status_no.Checked = false;
        else
            this.rabImportant_status_no.Checked = true;
    }

    /// 作者 占偉林
    /// 創建日期：2009/10/16
    /// 修改日期：2009/10/16 
    /// <summary>
    /// 從Show.xml取漢字，設置畫面控件的Text
    /// </summary>
    private void ShowControlsText()
    {
        //* 生效狀態
        this.radlStatus.Items[0].Text = BaseHelper.GetShowText("01_04020000_024");
        this.radlStatus.Items[1].Text = BaseHelper.GetShowText("01_04020000_025");
        //* 訊息狀態
        this.rabImportant_status_yes.Text = BaseHelper.GetShowText("01_04020000_024");
        this.rabImportant_status_no.Text = BaseHelper.GetShowText("01_04020000_025");

        this.ViewState["type"] = Request.QueryString["type"].ToString();        
        if (Request.QueryString["type"].ToString() == "edit")
        {
            this.ViewState["code"] = RedirectHelper.GetDecryptString(this.Page, "code");
            this.hidCode.Value = this.ViewState["code"].ToString();
            //* 修改時
            this.btnAddUpdate.Text = BaseHelper.GetShowText("01_04020000_021");
            this.lblTitle.Text = BaseHelper.GetShowText("01_04020000_014");
            this.btnDelete.Visible = true;
            //* 綁定布告欄訊息
            BindBoardInfo();
        }
        else
        {
            //* 添加時
            this.ViewState["code"] = "";
            this.hidCode.Value = "";
            this.radlStatus.Items[0].Selected = true;
            this.rabImportant_status_no.Checked = true;
            this.rabImportant_status_yes.Checked = false;
            this.btnAddUpdate.Text = BaseHelper.GetShowText("01_04020000_020");
            this.lblTitle.Text = BaseHelper.GetShowText("01_04020000_013");
            this.btnDelete.Visible = false;
            //* 顯示端末客戶訊息
            base.strClientMsg += "";
        }

        txtSubject.Focus();
    }

    /// 作者 占偉林
    /// 創建日期：2009/10/16
    /// 修改日期：2009/10/16 
    /// <summary>
    /// 返回
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReturn_Click(object sender, EventArgs e)
    {
        //20210412_Ares_Stanley-調整轉向方法
        Response.Redirect("~/Page/P010402000001.aspx", false);
    }

    /// 作者 占偉林
    /// 創建日期：2009/10/16
    /// 修改日期：2009/10/16 
    /// <summary>
    /// 點選【刪除】按鈕時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (BRBoard.DeleteBoardByCode(this.ViewState["code"].ToString()))
        {
            //* 顯示端末訊息  //20210412_Ares_Stanley-調整轉向方法
            Response.Redirect("~/Page/P010402000001.aspx?ClientMsgID=" + RedirectHelper.GetEncryptParam("01_04020000_005"), false);
        }
        else
        {
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("01_04020000_006");
        }
    }

    /// 作者 占偉林
    /// 創建日期：2009/10/16
    /// 修改日期：2009/10/16 
    /// <summary>
    /// 點選畫面【添加/修改】按鈕時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAddUpdate_Click(object sender, EventArgs e)
    {
        string strMsgID = "";

        if (this.ViewState["code"].ToString() != "")
        {
            #region 修改公布欄訊息
            //* 修改
            EntityBoard eBoard = new EntityBoard();
            eBoard.code = this.ViewState["code"].ToString();
            //* 生效狀態
            if (this.radlStatus.Items[0].Selected)
                eBoard.board_status = "Y";
            else
                eBoard.board_status = "N";
            //* 訊息狀態
            if (this.rabImportant_status_yes.Checked)
                eBoard.important_status = "Y";
            else
                eBoard.important_status = "N";
            //* 作業項目
            eBoard.subject = this.txtSubject.Text.Trim();
            //* 登入者
            //eBoard.user_id = ((EntityAGENT_INFO)this.Session["Agent"]).agent_id;
            //* 訊息內容
            eBoard.content = this.txtContent.Text;

            //* 更新當前畫面訊息
            if (BRBoard.Update(eBoard, ref strMsgID))
                //20210412_Ares_Stanley-調整轉向方法
                Response.Redirect("~/Page/P010402000001.aspx?ClientMsgID=" + RedirectHelper.GetEncryptParam(strMsgID), false);
            else
                base.strClientMsg += MessageHelper.GetMessage(strMsgID);
            #endregion 修改公布欄訊息
        }
        else
        {
            #region 添加
            //* 添加
            EntityBoard eBoard = new EntityBoard();
            eBoard.code = DateTime.Now.ToString("yyyyMMddHHmmss");
            //* 生效狀態
            if (this.radlStatus.Items[0].Selected)
                eBoard.board_status = "Y";
            else
                eBoard.board_status = "N";
            //* 訊息狀態
            if (this.rabImportant_status_yes.Checked)
                eBoard.important_status = "Y";
            else
                eBoard.important_status = "N";
            //* 作業項目
            eBoard.subject = this.txtSubject.Text.Trim();
            //* 登入者
            eBoard.user_id = ((EntityAGENT_INFO)this.Session["Agent"]).agent_id;
            //* 訊息內容
            eBoard.content = this.txtContent.Text;
            
            //* 添加頁面訊息
            if (BRBoard.Add(eBoard, ref strMsgID))
                //20210412_Ares_Stanley-調整轉向方法
                Response.Redirect("~/Page/P010402000001.aspx?ClientMsgID=" + RedirectHelper.GetEncryptParam(strMsgID), false);
            else
                base.strClientMsg += MessageHelper.GetMessage(strMsgID);
            #endregion 添加
        }
    }

    /// 作者 占偉林
    /// 創建日期：2009/10/16
    /// 修改日期：2009/10/16 
    /// <summary>
    /// 修改公布欄訊息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnHiden_Click(object sender, EventArgs e)
    {
        #region 修改公布欄訊息
        string strMsgID = "";
        //* 修改
        EntityBoard eBoard = new EntityBoard();
        eBoard.code = this.ViewState["code"].ToString();
        //* 生效狀態
        if (this.radlStatus.Items[0].Selected)
            eBoard.board_status = "Y";
        else
            eBoard.board_status = "N";
        //* 訊息狀態
        if (this.rabImportant_status_yes.Checked)
            eBoard.important_status = "Y";
        else
            eBoard.important_status = "N";
        //* 作業項目
        eBoard.subject = this.txtSubject.Text.Trim();
        //* 登入者
        eBoard.user_id = ((EntityAGENT_INFO)this.Session["Agent"]).agent_id;
        //* 訊息內容
        eBoard.content = this.txtContent.Text;

        //* 更新當前畫面訊息
        BRBoard.Update(eBoard, ref strMsgID);
        base.strClientMsg += MessageHelper.GetMessage(strMsgID);
        //20210412_Ares_Stanley-調整轉向方法
        Response.Redirect("~/Page/P010402000001.aspx", false);
        #endregion 修改公布欄訊息
    }
    #endregion event

    #region function
    /// 作者 占偉林
    /// 創建日期：2009/10/16
    /// 修改日期：2009/10/16 
    /// <summary>
    /// 綁定布告欄訊息
    /// </summary>
    private void BindBoardInfo()
    {
        try
        {
            //* 以CODE取公布欄訊息，返回EntitySet
            EntitySet<EntityBoard> esBoard = BRBoard.SearchByCode(this.ViewState["code"].ToString());
            if (esBoard == null)
            {
                //* 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage("01_04020000_001");
            }
            else
            {
                //* 如果沒有查詢到記錄
                if (esBoard.TotalCount == 0)
                {
                    //* 顯示端末訊息
                    base.strClientMsg += MessageHelper.GetMessage("01_04020000_007");
                    //* 刪除按鈕不可見
                    this.btnDelete.Visible = false;
                    return;
                }

                //* 生效狀態
                if (esBoard.GetEntity(0).board_status=="Y")
                {
                    this.radlStatus.Items[0].Selected=true;
                    this.radlStatus.Items[1].Selected=false;
                }else
                {
                    this.radlStatus.Items[0].Selected=false;
                    this.radlStatus.Items[1].Selected=true;
                }
                //* 作業項目
                this.txtSubject.Text = esBoard.GetEntity(0).subject;
                //* 訊息狀態
                if (esBoard.GetEntity(0).important_status=="Y")
                {
                    this.rabImportant_status_yes.Checked = true;
                    this.rabImportant_status_no.Checked = false;
                }else
                {
                    this.rabImportant_status_yes.Checked = false;
                    this.rabImportant_status_no.Checked = true;
                }
                //* 訊息內容
                this.txtContent.Text = esBoard.GetEntity(0).content;
                //* 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage("01_04020000_002");
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("01_04020000_001");
        }
    }
    #endregion function

    
}
