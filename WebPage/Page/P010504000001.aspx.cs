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
    private static EntityAGENT_INFO eAgentInfo;

    /// <summary>
    /// Client訊息
    /// </summary>
    private static string ClientMsg;

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

        eAgentInfo = (EntityAGENT_INFO)Session["Agent"]; // Session變數集合
    }

    /// <summary>
    /// 資料列綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvFUNCTION_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CustLinkButton btnModify = (CustLinkButton)e.Row.FindControl("btnModify");
            btnModify.Text = BaseHelper.GetShowText("01_05040000_002");
        }
    }

    /// <summary>
    /// 點擊資料列里的修改
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvFUNCTION_RowSelecting(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            LinkButton linkButton = e.CommandSource as LinkButton;
            GridViewRow row = linkButton.NamingContainer as GridViewRow;
            int idx = row.RowIndex;

            txtPostOfficeRtnInfoSeq.Text = grvFUNCTION.Rows[idx].Cells[0].Text;
            txtRtnType.Text = grvFUNCTION.Rows[idx].Cells[1].Text;
            txtPostRtnCode.Text = grvFUNCTION.Rows[idx].Cells[2].Text;
            txtPostRtnMsg.Text = grvFUNCTION.Rows[idx].Cells[3].Text;

            btnADD.Enabled = false;
            btnOK.Enabled = true;
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }
    }

    /// <summary>
    /// GridView換頁
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }

    /// <summary>
    /// 添加(核印代碼)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnADD_Click(object sender, EventArgs e)
    {
        string rtnType = txtRtnType.Text.Trim();
        string postRtnCode = txtPostRtnCode.Text.Trim();
        string postRtnMsg = txtPostRtnMsg.Text.Trim();

        if (string.IsNullOrEmpty(rtnType) || string.IsNullOrEmpty(postRtnCode) ||
            string.IsNullOrEmpty(postRtnMsg))
        {
            sbRegScript.Append("alert('代碼或訊息不能是空白!');");
            return;
        }
        
        string sql = @"INSERT INTO PostOffice_Rtn_Info (RtnType, PostRtnCode, PostRtnMsg, Creator, CreateDate)
                        VALUES (@RtnType, @PostRtnCode, @PostRtnMsg, @Creator, GETDATE())";
        
        try
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandText = sql;
            sqlCommand.CommandType = CommandType.Text;

            SqlParameter paramRtnType = new SqlParameter("@RtnType", rtnType);
            sqlCommand.Parameters.Add(paramRtnType);
            SqlParameter paramPostRtnCode = new SqlParameter("@PostRtnCode", postRtnCode);
            sqlCommand.Parameters.Add(paramPostRtnCode);
            SqlParameter paramPostRtnMsg = new SqlParameter("@PostRtnMsg", postRtnMsg);
            sqlCommand.Parameters.Add(paramPostRtnMsg);
            SqlParameter paramCreator = new SqlParameter("@Creator", eAgentInfo.agent_id);
            sqlCommand.Parameters.Add(paramCreator);

            if (BRBase<Entity_SP>.Update(sqlCommand, "Connection_System"))
            {
                // 新增成功重新查詢
                BindGridView();
                
                // 清空控制項內容
                CleanInputControl();
                
                // 解鎖控制項
                btnADD.Enabled = true;
                btnOK.Enabled = false;
                ClientMsg = MessageHelper.GetMessage("01_05040000_001");
                sbRegScript.Append("alert('" + ClientMsg + "');");
            }
            else
            {
                ClientMsg = MessageHelper.GetMessage("01_05040000_002");
                sbRegScript.Append("alert('" + ClientMsg + "');");
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }
    }

    /// <summary>
    /// 確定(修改核印代碼)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        string postOfficeRtnInfoSeq = txtPostOfficeRtnInfoSeq.Text.Trim();
        string rtnType = txtRtnType.Text.Trim();
        string postRtnCode = txtPostRtnCode.Text.Trim();
        string postRtnMsg = txtPostRtnMsg.Text.Trim();

        if (string.IsNullOrEmpty(rtnType) || string.IsNullOrEmpty(postRtnCode) ||
            string.IsNullOrEmpty(postRtnMsg))
        {
            sbRegScript.Append("alert('代碼或訊息不能是空白!');");
            return;
        }
        
        string sql = @"UPDATE PostOffice_Rtn_Info SET RtnType = @RtnType, PostRtnCode = @PostRtnCode, 
                        PostRtnMsg = @PostRtnMsg, Modifier = @Modifier, ModifierDate = GETDATE()
                        WHERE PostOfficeRtnInfoSeq = @PostOfficeRtnInfoSeq";
        
        try
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandText = sql;
            sqlCommand.CommandType = CommandType.Text;

            SqlParameter paramSeq = new SqlParameter("@PostOfficeRtnInfoSeq", postOfficeRtnInfoSeq);
            sqlCommand.Parameters.Add(paramSeq);
            SqlParameter paramRtnType = new SqlParameter("@RtnType", rtnType);
            sqlCommand.Parameters.Add(paramRtnType);
            SqlParameter paramPostRtnCode = new SqlParameter("@PostRtnCode", postRtnCode);
            sqlCommand.Parameters.Add(paramPostRtnCode);
            SqlParameter paramPostRtnMsg = new SqlParameter("@PostRtnMsg", postRtnMsg);
            sqlCommand.Parameters.Add(paramPostRtnMsg);
            SqlParameter paramModifier = new SqlParameter("@Modifier", eAgentInfo.agent_id);
            sqlCommand.Parameters.Add(paramModifier);

            if (BRBase<Entity_SP>.Update(sqlCommand, "Connection_System"))
            {
                // 更新成功重新查詢
                BindGridView();
                
                // 清空控制項內容
                CleanInputControl();
                
                // 解鎖控制項
                btnADD.Enabled = true;
                btnOK.Enabled = false;
                ClientMsg = MessageHelper.GetMessage("01_05040000_003");
                sbRegScript.Append("alert('" + ClientMsg + "');");
            }
            else
            {
                ClientMsg = MessageHelper.GetMessage("01_05040000_004");
                sbRegScript.Append("alert('" + ClientMsg + "');");
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }
    }
    
    /// <summary>
    /// 清空控制項內容
    /// </summary>
    private void CleanInputControl()
    {
        txtPostOfficeRtnInfoSeq.Text = string.Empty;
        txtRtnType.Text = string.Empty;
        txtPostRtnCode.Text = string.Empty;
        txtPostRtnMsg.Text = string.Empty;
        
        btnADD.Enabled = true;
        btnOK.Enabled = false;
    }

    /// <summary>
    /// GridView表頭訊息綁定
    /// </summary>
    private void Show()
    {
        grvFUNCTION.Columns[0].HeaderText = string.Empty; // 流水號不顯示
        grvFUNCTION.Columns[1].HeaderText = BaseHelper.GetShowText("01_05040000_007");
        grvFUNCTION.Columns[2].HeaderText = BaseHelper.GetShowText("01_05040000_008");
        grvFUNCTION.Columns[3].HeaderText = BaseHelper.GetShowText("01_05040000_009");
        grvFUNCTION.Columns[4].HeaderText = BaseHelper.GetShowText("01_05040000_010");
    }

    /// <summary>
    /// GridView資料綁定
    /// </summary>
    private void BindGridView()
    {
        try
        {
            var sql = @"SELECT PostOfficeRtnInfoSeq, RtnType, PostRtnCode, PostRtnMsg FROM PostOffice_Rtn_Info";
            SqlCommand sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sql };
            DataSet ds = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                gpList.Visible = true;
                grvFUNCTION.Visible = true;
                gpList.RecordCount = ds.Tables[0].Rows.Count;
                grvFUNCTION.DataSource = ds.Tables[0];
                grvFUNCTION.DataBind();
            }
            else
            {
                gpList.Visible = false;
                grvFUNCTION.Visible = false;
                gpList.RecordCount = 0;
                grvFUNCTION.DataSource = null;
                grvFUNCTION.DataBind();
            }
        }
        catch (Exception ex)
        {
            jsBuilder.RegScript(UpdatePanel1, BaseHelper.ClientMsgShow("00_00000000_000"));
        }
    }

    /// <summary>
    /// 點擊取消
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        // 清空控制項內容
        CleanInputControl();
    }
}
