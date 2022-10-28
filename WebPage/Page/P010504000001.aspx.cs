//******************************************************************
//*  作    者：Ares.jhun
//*  功能說明：郵局核印代碼管理
//*  創建日期：2022/10/03
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

public partial class P010504000001 : PageBase
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
        gridView.Columns[1].HeaderText = string.Empty; // 類別代號不顯示
        gridView.Columns[2].HeaderText = BaseHelper.GetShowText("01_05030000_008");
        gridView.Columns[3].HeaderText = BaseHelper.GetShowText("01_05030000_009");
        gridView.Columns[4].HeaderText = BaseHelper.GetShowText("01_05030000_010");
        gridView.Columns[5].HeaderText = BaseHelper.GetShowText("01_05030000_011");
        gridView.Columns[6].HeaderText = BaseHelper.GetShowText("01_05030000_012");
        gridView.Columns[7].HeaderText = BaseHelper.GetShowText("01_05030000_007");
    }

    /// <summary>
    /// 資料列綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void GridViewRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CustLinkButton btnModify = (CustLinkButton)e.Row.FindControl("btnModify");
            btnModify.Text = BaseHelper.GetShowText("01_05040000_002");
            CustLinkButton btnDelete = (CustLinkButton)e.Row.FindControl("btnDelete");
            btnDelete.Text = BaseHelper.GetShowText("01_05040000_003");
            var rtnCode = e.Row.Cells[3].Text.Replace("&nbsp;", string.Empty);
            btnDelete.Attributes.Add("onclick", "return confirm('" + MessageHelper.GetMessage("01_05040000_007") +　"【" + rtnCode + "】')");
        }
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
            txtPostOfficeRtnInfoSeq.Text = gridView.Rows[idx].Cells[0].Text.Replace("&nbsp;", string.Empty);
            var rtnTypeItem = RtnTypeList.Items.FindByValue(gridView.Rows[idx].Cells[1].Text.Replace("&nbsp;", string.Empty));
            RtnTypeList.SelectedIndex = RtnTypeList.Items.IndexOf(rtnTypeItem);
            txtPostRtnCode.Text = gridView.Rows[idx].Cells[3].Text.Replace("&nbsp;", string.Empty);
            txtPostRtnMsg.Text = gridView.Rows[idx].Cells[4].Text.Replace("&nbsp;", string.Empty);
            var needSendHostItem = NeedSendHostList.Items.FindByValue(gridView.Rows[idx].Cells[5].Text.Replace("&nbsp;", string.Empty));
            NeedSendHostList.SelectedIndex = NeedSendHostList.Items.IndexOf(needSendHostItem);
            txtSendHostMsg.Text = gridView.Rows[idx].Cells[6].Text.Replace("&nbsp;", string.Empty);
            
            // 不能修改回覆訊息(PostRtnMsg)
            txtPostRtnMsg.Enabled = false;

            btnADD.Enabled = false;
            btnOK.Enabled = true;

            // 將要修改的代碼存至Session
            Session["RtnType"] = rtnTypeItem.Value;
            Session["PostRtnCode"] = txtPostRtnCode.Text;
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
            var postOfficeRtnInfoSeq = e.Values[0].ToString();

            var sql = @"DELETE FROM PostOffice_Rtn_Info WHERE PostOfficeRtnInfoSeq = @PostOfficeRtnInfoSeq";

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandText = sql;
            sqlCommand.CommandType = CommandType.Text;

            sqlCommand.Parameters.Add(new SqlParameter("@PostOfficeRtnInfoSeq", postOfficeRtnInfoSeq));

            if (BRBase<Entity_SP>.Delete(sqlCommand, "Connection_System"))
            {
                // 刪除成功重新查詢
                BindGridView();

                // 清空控制項內容
                CleanInputControl();

                _clientMsg = MessageHelper.GetMessage("01_05040000_005");
                sbRegScript.Append("alert('" + _clientMsg + "');");
            }
            else
            {
                _clientMsg = MessageHelper.GetMessage("01_05040000_006");
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
        var rtnType = RtnTypeList.SelectedValue;
        var postRtnCode = txtPostRtnCode.Text.Trim();
        var postRtnMsg = txtPostRtnMsg.Text.Trim();
        var needSendHost = NeedSendHostList.SelectedValue;
        var sendHostMsg = txtSendHostMsg.Text.Trim();

        if (string.IsNullOrEmpty(rtnType) || string.IsNullOrEmpty(postRtnCode))
        {
            _clientMsg = MessageHelper.GetMessage("01_05040000_008");
            sbRegScript.Append("alert('" + _clientMsg + "');");
            return;
        }

        // 檢查代碼是否已經存在
        if (CheckIsDuplicates(rtnType, postRtnCode, false))
        {
            _clientMsg = MessageHelper.GetMessage("01_05040000_009");
            sbRegScript.Append("alert('" + _clientMsg + "');");
            return;
        }

        var sql = @"INSERT INTO PostOffice_Rtn_Info (RtnType, PostRtnCode, PostRtnMsg, NeedSendHost, SendHostMsg, Creator, CreateDate)
                        VALUES (@RtnType, @PostRtnCode, @PostRtnMsg, @NeedSendHost, @SendHostMsg, @Creator, GETDATE())";

        try
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandText = sql;
            sqlCommand.CommandType = CommandType.Text;

            sqlCommand.Parameters.Add(new SqlParameter("@RtnType", rtnType));
            sqlCommand.Parameters.Add(new SqlParameter("@PostRtnCode", postRtnCode));
            sqlCommand.Parameters.Add(new SqlParameter("@PostRtnMsg", postRtnMsg));
            sqlCommand.Parameters.Add(new SqlParameter("@NeedSendHost", needSendHost));
            sqlCommand.Parameters.Add(new SqlParameter("@SendHostMsg", sendHostMsg));
            sqlCommand.Parameters.Add(new SqlParameter("@Creator", _eAgentInfo.agent_id));

            if (BRBase<Entity_SP>.Update(sqlCommand, "Connection_System"))
            {
                // 新增成功重新查詢
                BindGridView();

                // 清空控制項內容
                CleanInputControl();

                _clientMsg = MessageHelper.GetMessage("01_05040000_001");
                sbRegScript.Append("alert('" + _clientMsg + "');");
            }
            else
            {
                _clientMsg = MessageHelper.GetMessage("01_05040000_002");
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
        var postOfficeRtnInfoSeq = txtPostOfficeRtnInfoSeq.Text.Trim();
        var rtnType = RtnTypeList.SelectedValue;
        var postRtnCode = txtPostRtnCode.Text.Trim();
        var postRtnMsg = txtPostRtnMsg.Text.Trim();
        var needSendHost = NeedSendHostList.SelectedValue;
        var sendHostMsg = txtSendHostMsg.Text.Trim();

        if (string.IsNullOrEmpty(rtnType) || string.IsNullOrEmpty(postRtnCode) ||
            string.IsNullOrEmpty(postRtnMsg))
        {
            _clientMsg = MessageHelper.GetMessage("01_05040000_008");
            sbRegScript.Append("alert('" + _clientMsg + "');");
            return;
        }

        // 檢查代碼是否已經存在
        if (CheckIsDuplicates(rtnType, postRtnCode))
        {
            _clientMsg = MessageHelper.GetMessage("01_05040000_009");
            sbRegScript.Append("alert('" + _clientMsg + "');");
            return;
        }

        string sql = @"UPDATE PostOffice_Rtn_Info SET RtnType = @RtnType, PostRtnCode = @PostRtnCode, PostRtnMsg = @PostRtnMsg, 
                               NeedSendHost = @NeedSendHost, SendHostMsg = @SendHostMsg, Modifier = @Modifier, ModifierDate = GETDATE()
                        WHERE PostOfficeRtnInfoSeq = @PostOfficeRtnInfoSeq";

        try
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandText = sql;
            sqlCommand.CommandType = CommandType.Text;

            sqlCommand.Parameters.Add(new SqlParameter("@PostOfficeRtnInfoSeq", postOfficeRtnInfoSeq));
            sqlCommand.Parameters.Add(new SqlParameter("@RtnType", rtnType));
            sqlCommand.Parameters.Add(new SqlParameter("@PostRtnCode", postRtnCode));
            sqlCommand.Parameters.Add(new SqlParameter("@PostRtnMsg", postRtnMsg));
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
                _clientMsg = MessageHelper.GetMessage("01_05040000_003");
                sbRegScript.Append("alert('" + _clientMsg + "');");
            }
            else
            {
                _clientMsg = MessageHelper.GetMessage("01_05040000_004");
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
        txtPostOfficeRtnInfoSeq.Text = string.Empty;
        txtPostRtnCode.Text = string.Empty;
        txtPostRtnMsg.Text = string.Empty;
        txtSendHostMsg.Text = string.Empty;
        RtnTypeList.SelectedIndex = 0;
        NeedSendHostList.SelectedIndex = 0;

        txtPostRtnMsg.Enabled = true;
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
            var sql = @"SELECT PostOfficeRtnInfoSeq, RtnType, PostRtnCode, PostRtnMsg, NeedSendHost, SendHostMsg,
                        CASE RtnType WHEN '1' THEN '狀況代號' WHEN '2' THEN '核對註記' ELSE '' END AS RtnTypeName
                        FROM PostOffice_Rtn_Info WHERE RtnType IN ('1', '2') ORDER BY RtnType, PostRtnCode";
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
    /// <param name="rtnType">分類</param>
    /// <param name="postRtnCode">郵局狀況代碼或核印註記</param>
    /// <param name="isUpdate">是否為修改</param>
    /// <returns></returns>
    private bool CheckIsDuplicates(string rtnType, string postRtnCode, bool isUpdate = true)
    {
        var sql = @"SELECT * FROM PostOffice_Rtn_Info WHERE RtnType = @RtnType AND PostRtnCode = @PostRtnCode";
        
        try
        {
            SqlCommand sqlCommand = new SqlCommand { CommandType = CommandType.Text, CommandText = sql };

            sqlCommand.Parameters.Add(new SqlParameter("@RtnType", rtnType));
            sqlCommand.Parameters.Add(new SqlParameter("@PostRtnCode", postRtnCode));

            DataSet ds = BRBase<Entity_SP>.SearchOnDataSet(sqlCommand, "Connection_System");
            if (ds == null || ds.Tables[0].Rows.Count == 0) return false;
            
            var count = ds.Tables[0].Rows.Count;
            if (!isUpdate) return count > 0;

            // 修改
            var oldRtnType = (string)Session["RtnType"];
            var oldPostRtnCode = (string)Session["PostRtnCode"];
            if (oldRtnType.Equals(rtnType) && oldPostRtnCode.Equals(postRtnCode))
            {
                return false;
            }

            return count > 0;
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            jsBuilder.RegScript(UpdatePanel1, BaseHelper.ClientMsgShow("00_00000000_000"));
            return true;
        }
    }
}