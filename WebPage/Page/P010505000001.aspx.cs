//******************************************************************
//*  作    者：Ares.jhun
//*  功能說明：EDDA核印代碼管理
//*  創建日期：2022/10/25
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************

using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Framework.Common.Message;
using Framework.Common.JavaScript;
using Framework.Common.Logging;
using Framework.WebControls;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BusinessRules;
using CSIPNewInvoice.EntityLayer_new;

public partial class P010505000001 : PageBase
{
    /// <summary>
    /// Session變數集合
    /// </summary>
    private static EntityAGENT_INFO _eAgentInfo;

    /// <summary>
    /// Client訊息
    /// </summary>
    private static string _clientMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = BaseHelper.GetShowText("00_01040000_000");
        if (!Page.IsPostBack)
        {
            jsBuilder.RegScript(UpdatePanel1, BaseHelper.ClientMsgShow(""));

            Show();
            btnOK.Enabled = false;
            BindGridView();
        }

        _eAgentInfo = (EntityAGENT_INFO)Session["Agent"]; // Session變數集合
    }

    /// <summary>
    /// GridView表頭訊息綁定
    /// </summary>
    private void Show()
    {
        gridView.Columns[0].HeaderText = string.Empty; // 流水號不顯示
        gridView.Columns[1].HeaderText = BaseHelper.GetShowText("01_05050000_008");
        gridView.Columns[2].HeaderText = BaseHelper.GetShowText("01_05050000_009");
        gridView.Columns[3].HeaderText = BaseHelper.GetShowText("01_05050000_010");
        gridView.Columns[4].HeaderText = BaseHelper.GetShowText("01_05050000_011");
        gridView.Columns[5].HeaderText = BaseHelper.GetShowText("01_05050000_007");
    }
    
    /// <summary>
    /// 資料列綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void GridViewRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.DataRow) return;
        
        CustLinkButton btnModify = (CustLinkButton)e.Row.FindControl("btnModify");
        btnModify.Text = BaseHelper.GetShowText("01_05050000_002");
        btnModify.Attributes.Add("onclick", "window.scrollTo(0, 0)");
        CustLinkButton btnDelete = (CustLinkButton)e.Row.FindControl("btnDelete");
        btnDelete.Text = BaseHelper.GetShowText("01_05050000_003");
        var eddaRtnCode = e.Row.Cells[1].Text.Replace("&nbsp;", string.Empty);
        btnDelete.Attributes.Add("onclick", "return confirm('" + MessageHelper.GetMessage("01_05050000_007") + "【" + eddaRtnCode + "】')");
    }

    /// <summary>
    /// 點擊資料列里的修改
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void GridViewRowSelect(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            LinkButton linkButton = e.CommandSource as LinkButton;
            GridViewRow row = linkButton.NamingContainer as GridViewRow;
            int idx = row.RowIndex;

            // 將資料帶入畫面
            txtEddaRtnInfoSeq.Text = gridView.Rows[idx].Cells[0].Text.Replace("&nbsp;", string.Empty);
            txtEddaRtnCode.Text = gridView.Rows[idx].Cells[1].Text.Replace("&nbsp;", string.Empty);
            txtEddaRtnMsg.Text = gridView.Rows[idx].Cells[2].Text.Replace("&nbsp;", string.Empty);
            var selectItem = NeedSendHostList.Items.FindByValue(gridView.Rows[idx].Cells[3].Text.Replace("&nbsp;", string.Empty));
            NeedSendHostList.SelectedIndex = NeedSendHostList.Items.IndexOf(selectItem);
            txtSendHostMsg.Text = gridView.Rows[idx].Cells[4].Text.Replace("&nbsp;", string.Empty);

            // 不能修改回覆訊息(EddaRtnMsg)
            txtEddaRtnMsg.Enabled = false;
            
            btnADD.Enabled = false;
            btnOK.Enabled = true;

            // 將要修改的代碼存至Session
            Session["EddaRtnCode"] = txtEddaRtnCode.Text;
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
    }

    /// <summary>
    /// 點擊資料列里的刪除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void GridViewRowDelete(object sender, GridViewDeleteEventArgs e)
    {
        try
        {
            var eddaRtnInfoSeq = e.Values[0].ToString();

            var sql = @"DELETE FROM EDDA_Rtn_Info WHERE EddaRtnInfoSeq = @EddaRtnInfoSeq";

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandText = sql;
            sqlCommand.CommandType = CommandType.Text;

            sqlCommand.Parameters.Add(new SqlParameter("@EddaRtnInfoSeq", eddaRtnInfoSeq));

            if (BRBase<Entity_SP>.Delete(sqlCommand, "Connection_System"))
            {
                // 刪除成功重新查詢
                BindGridView();

                // 清空控制項內容
                CleanInputControl();

                _clientMsg = MessageHelper.GetMessage("01_05050000_005");
                sbRegScript.Append("alert('" + _clientMsg + "');");
            }
            else
            {
                _clientMsg = MessageHelper.GetMessage("01_05050000_006");
                sbRegScript.Append("alert('" + _clientMsg + "');");
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
    }

    /// <summary>
    /// GridView換頁
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void GridViewPageChanged(object src, PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }

    /// <summary>
    /// 添加(核印代碼)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void BtnAddClick(object sender, EventArgs e)
    {
        var eddaRtnCode = txtEddaRtnCode.Text.Trim();
        var eddaRtnMsg = txtEddaRtnMsg.Text.Trim();
        var needSendHost = NeedSendHostList.SelectedValue;
        var sendHostMsg = txtSendHostMsg.Text.Trim();
        
        if (string.IsNullOrEmpty(eddaRtnCode) || string.IsNullOrEmpty(eddaRtnMsg))
        {
            _clientMsg = MessageHelper.GetMessage("01_05050000_008");
            sbRegScript.Append("alert('" + _clientMsg + "');");
            return;
        }
        
        // 檢查代碼是否已經存在
        if (CheckIsDuplicates(eddaRtnCode, false))
        {
            _clientMsg = MessageHelper.GetMessage("01_05050000_009");
            sbRegScript.Append("alert('" + _clientMsg + "');");
            return;
        }
        
        var sql = @"INSERT INTO EDDA_Rtn_Info (EddaRtnCode, EddaRtnMsg, NeedSendHost, SendHostMsg, Creator, CreateDate)
                        VALUES (@EddaRtnCode, @EddaRtnMsg, @NeedSendHost, @SendHostMsg, @Creator, GETDATE())";
        
        try
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandText = sql;
            sqlCommand.CommandType = CommandType.Text;
        
            sqlCommand.Parameters.Add(new SqlParameter("@EddaRtnCode", eddaRtnCode));
            sqlCommand.Parameters.Add(new SqlParameter("@EddaRtnMsg", eddaRtnMsg));
            sqlCommand.Parameters.Add(new SqlParameter("@NeedSendHost", needSendHost));
            sqlCommand.Parameters.Add(new SqlParameter("@SendHostMsg", sendHostMsg));
            sqlCommand.Parameters.Add(new SqlParameter("@Creator", _eAgentInfo.agent_id));
        
            if (BRBase<Entity_SP>.Update(sqlCommand, "Connection_System"))
            {
                // 新增成功重新查詢
                BindGridView();
        
                // 清空控制項內容
                CleanInputControl();
        
                _clientMsg = MessageHelper.GetMessage("01_05050000_001");
                sbRegScript.Append("alert('" + _clientMsg + "');");
            }
            else
            {
                _clientMsg = MessageHelper.GetMessage("01_05050000_002");
                sbRegScript.Append("alert('" + _clientMsg + "');");
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
    }

    /// <summary>
    /// 確定(修改核印代碼)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void BtnOkClick(object sender, EventArgs e)
    {
        var eddaRtnInfoSeq = txtEddaRtnInfoSeq.Text.Trim();
        var eddaRtnCode = txtEddaRtnCode.Text.Trim();
        var eddaRtnMsg = txtEddaRtnMsg.Text.Trim();
        var needSendHost = NeedSendHostList.SelectedValue;
        var sendHostMsg = txtSendHostMsg.Text.Trim();
        
        if (string.IsNullOrEmpty(eddaRtnCode) || string.IsNullOrEmpty(eddaRtnMsg))
        {
            _clientMsg = MessageHelper.GetMessage("01_05050000_008");
            sbRegScript.Append("alert('" + _clientMsg + "');");
            return;
        }
        
        // 檢查代碼是否已經存在
        if (CheckIsDuplicates(eddaRtnCode))
        {
            _clientMsg = MessageHelper.GetMessage("01_05050000_009");
            sbRegScript.Append("alert('" + _clientMsg + "');");
            return;
        }
        
        var sql = @"UPDATE EDDA_Rtn_Info SET EddaRtnCode = @EddaRtnCode, EddaRtnMsg = @EddaRtnMsg, 
                        NeedSendHost = @NeedSendHost, SendHostMsg = @SendHostMsg, Modifier = @Modifier, ModifierDate = GETDATE()
                        WHERE EddaRtnInfoSeq = @EddaRtnInfoSeq";
        
        try
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandText = sql;
            sqlCommand.CommandType = CommandType.Text;
            
            sqlCommand.Parameters.Add(new SqlParameter("@EddaRtnInfoSeq", eddaRtnInfoSeq));
            sqlCommand.Parameters.Add(new SqlParameter("@EddaRtnCode", eddaRtnCode));
            sqlCommand.Parameters.Add(new SqlParameter("@EddaRtnMsg", eddaRtnMsg));
            sqlCommand.Parameters.Add(new SqlParameter("@NeedSendHost", needSendHost));
            sqlCommand.Parameters.Add(new SqlParameter("@SendHostMsg", sendHostMsg));
            sqlCommand.Parameters.Add(new SqlParameter("@Modifier", _eAgentInfo.agent_id));
        
            if (BRBase<Entity_SP>.Update(sqlCommand, "Connection_System"))
            {
                // 更新成功重新查詢
                BindGridView();
        
                // 清空控制項內容
                CleanInputControl();
        
                // 解鎖控制項
                btnADD.Enabled = true;
                btnOK.Enabled = false;
                _clientMsg = MessageHelper.GetMessage("01_05050000_003");
                sbRegScript.Append("alert('" + _clientMsg + "');");
            }
            else
            {
                _clientMsg = MessageHelper.GetMessage("01_05050000_004");
                sbRegScript.Append("alert('" + _clientMsg + "');");
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
    }

    /// <summary>
    /// 清空控制項內容
    /// </summary>
    private void CleanInputControl()
    {
        txtEddaRtnInfoSeq.Text = string.Empty;
        txtEddaRtnCode.Text = string.Empty;
        txtEddaRtnMsg.Text = string.Empty;
        txtSendHostMsg.Text = string.Empty;
        NeedSendHostList.SelectedIndex = 0;

        txtEddaRtnMsg.Enabled = true;
        btnADD.Enabled = true;
        btnOK.Enabled = false;
    }

    /// <summary>
    /// GridView資料綁定
    /// </summary>
    private void BindGridView()
    {
        try
        {
            var sql = @"SELECT EddaRtnInfoSeq, EddaRtnCode, EddaRtnMsg, NeedSendHost, SendHostMsg FROM EDDA_Rtn_Info ORDER BY EddaRtnCode";
            SqlCommand sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sql };
            DataSet ds = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                gpList.Visible = true;
                gridView.Visible = true;
                gpList.RecordCount = ds.Tables[0].Rows.Count;
                gridView.DataSource = ds.Tables[0];
                gridView.DataBind();
            }
            else
            {
                gpList.Visible = false;
                gridView.Visible = false;
                gpList.RecordCount = 0;
                gridView.DataSource = null;
                gridView.DataBind();
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            jsBuilder.RegScript(UpdatePanel1, BaseHelper.ClientMsgShow("00_00000000_000"));
        }
    }

    /// <summary>
    /// 點擊取消
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    protected void BtnCancelClick(object sender, EventArgs e)
    {
        // 清空控制項內容
        CleanInputControl();
    }

    /// <summary>
    /// 確認代碼是否重覆
    /// </summary>
    /// <param name="eddaRtnCode">核印代碼</param>
    /// <param name="isUpdate">是否為修改</param>
    /// <returns></returns>
    private bool CheckIsDuplicates(string eddaRtnCode, bool isUpdate = true)
    {
        var sql = @"SELECT * FROM EDDA_Rtn_Info WHERE EddaRtnCode = @EddaRtnCode";
        
        try
        {
            var sqlCommand = new SqlCommand { CommandType = CommandType.Text, CommandText = sql };

            sqlCommand.Parameters.Add(new SqlParameter("@EddaRtnCode", eddaRtnCode));

            var ds = BRBase<Entity_SP>.SearchOnDataSet(sqlCommand, "Connection_System");
            if (ds == null || ds.Tables[0].Rows.Count == 0) return false;
            
            var count = ds.Tables[0].Rows.Count;
            if (!isUpdate) return count > 0;
            
            // 修改
            var oldEddaRtnCode = (string)Session["EddaRtnCode"];
            return !oldEddaRtnCode.Equals(eddaRtnCode);
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            jsBuilder.RegScript(UpdatePanel1, BaseHelper.ClientMsgShow("00_00000000_000"));
            return true;
        }
    }
}