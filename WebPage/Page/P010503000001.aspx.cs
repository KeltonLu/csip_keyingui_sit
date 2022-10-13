//******************************************************************
//*  作    者：Ares.jhun
//*  功能說明：法金核印代碼管理
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

public partial class P010503000001 : PageBase
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
            btnModify.Text = BaseHelper.GetShowText("01_05030000_002");
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

            txtAchRtnInfoSeq.Text = grvFUNCTION.Rows[idx].Cells[0].Text;
            txtAchRtnCode.Text = grvFUNCTION.Rows[idx].Cells[1].Text;
            txtAchRtnMsg.Text = grvFUNCTION.Rows[idx].Cells[2].Text;
            txtEddaRtnCode.Text = grvFUNCTION.Rows[idx].Cells[3].Text;
            txtEddaRtnMsg.Text = grvFUNCTION.Rows[idx].Cells[4].Text;

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
        string achRtnCode = txtAchRtnCode.Text.Trim();
        string achRtnMsg = txtAchRtnMsg.Text.Trim();
        string eddaRtnCode = txtEddaRtnCode.Text.Trim();
        string eddaRtnMsg = txtEddaRtnMsg.Text.Trim();

        if (string.IsNullOrEmpty(achRtnCode) || string.IsNullOrEmpty(achRtnMsg) ||
            string.IsNullOrEmpty(eddaRtnCode) || string.IsNullOrEmpty(eddaRtnMsg))
        {
            sbRegScript.Append("alert('代碼或訊息不能是空白!');");
            return;
        }
        
        string sql = @"INSERT INTO Ach_Rtn_Info (Ach_Rtn_Code, Ach_Rtn_Msg, EDDA_Rtn_Code, EDDA_Rtn_Msg, Creator, CreateDate)
                        VALUES (@Ach_Rtn_Code, @Ach_Rtn_Msg, @EDDA_Rtn_Code, @EDDA_Rtn_Msg, @Creator, GETDATE())";
        
        try
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandText = sql;
            sqlCommand.CommandType = CommandType.Text;

            SqlParameter paramAchRtnCode = new SqlParameter("@Ach_Rtn_Code", achRtnCode);
            sqlCommand.Parameters.Add(paramAchRtnCode);
            SqlParameter paramAchRtnMsg = new SqlParameter("@Ach_Rtn_Msg", achRtnMsg);
            sqlCommand.Parameters.Add(paramAchRtnMsg);
            SqlParameter paramEddaRtnCode = new SqlParameter("@EDDA_Rtn_Code", eddaRtnCode);
            sqlCommand.Parameters.Add(paramEddaRtnCode);
            SqlParameter paramEddaRtnMsg = new SqlParameter("@EDDA_Rtn_Msg", eddaRtnMsg);
            sqlCommand.Parameters.Add(paramEddaRtnMsg);
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
                ClientMsg = MessageHelper.GetMessage("01_05030000_001");
                sbRegScript.Append("alert('" + ClientMsg + "');");
            }
            else
            {
                ClientMsg = MessageHelper.GetMessage("01_05030000_002");
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
        string achRtnInfoSeq = txtAchRtnInfoSeq.Text.Trim();
        string achRtnCode = txtAchRtnCode.Text.Trim();
        string achRtnMsg = txtAchRtnMsg.Text.Trim();
        string eddaRtnCode = txtEddaRtnCode.Text.Trim();
        string eddaRtnMsg = txtEddaRtnMsg.Text.Trim();

        if (string.IsNullOrEmpty(achRtnCode) || string.IsNullOrEmpty(achRtnMsg) ||
            string.IsNullOrEmpty(eddaRtnCode) || string.IsNullOrEmpty(eddaRtnMsg))
        {
            sbRegScript.Append("alert('代碼或訊息不能是空白!');");
            return;
        }
        
        string sql = @"UPDATE Ach_Rtn_Info SET Ach_Rtn_Code = @Ach_Rtn_Code, Ach_Rtn_Msg = @Ach_Rtn_Msg, 
                        EDDA_Rtn_Msg = @EDDA_Rtn_Msg, EDDA_Rtn_Code = @EDDA_Rtn_Code, Modifier = @Modifier, 
                        ModifierDate = GETDATE()
                        WHERE AchRtnInfoSeq = @AchRtnInfoSeq";
        
        try
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandText = sql;
            sqlCommand.CommandType = CommandType.Text;

            SqlParameter paramSeq = new SqlParameter("@AchRtnInfoSeq", achRtnInfoSeq);
            sqlCommand.Parameters.Add(paramSeq);
            SqlParameter paramAchRtnCode = new SqlParameter("@Ach_Rtn_Code", achRtnCode);
            sqlCommand.Parameters.Add(paramAchRtnCode);
            SqlParameter paramAchRtnMsg = new SqlParameter("@Ach_Rtn_Msg", achRtnMsg);
            sqlCommand.Parameters.Add(paramAchRtnMsg);
            SqlParameter paramEddaRtnCode = new SqlParameter("@EDDA_Rtn_Code", eddaRtnCode);
            sqlCommand.Parameters.Add(paramEddaRtnCode);
            SqlParameter paramEddaRtnMsg = new SqlParameter("@EDDA_Rtn_Msg", eddaRtnMsg);
            sqlCommand.Parameters.Add(paramEddaRtnMsg);
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
                ClientMsg = MessageHelper.GetMessage("01_05030000_003");
                sbRegScript.Append("alert('" + ClientMsg + "');");
            }
            else
            {
                ClientMsg = MessageHelper.GetMessage("01_05030000_004");
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
        txtAchRtnInfoSeq.Text = string.Empty;
        txtAchRtnCode.Text = string.Empty;
        txtAchRtnMsg.Text = string.Empty;
        txtEddaRtnCode.Text = string.Empty;
        txtEddaRtnMsg.Text = string.Empty;
        
        btnADD.Enabled = true;
        btnOK.Enabled = false;
    }

    /// <summary>
    /// GridView表頭訊息綁定
    /// </summary>
    private void Show()
    {
        grvFUNCTION.Columns[0].HeaderText = string.Empty; // 流水號不顯示
        grvFUNCTION.Columns[1].HeaderText = BaseHelper.GetShowText("01_05030000_007");
        grvFUNCTION.Columns[2].HeaderText = BaseHelper.GetShowText("01_05030000_008");
        grvFUNCTION.Columns[3].HeaderText = BaseHelper.GetShowText("01_05030000_009");
        grvFUNCTION.Columns[4].HeaderText = BaseHelper.GetShowText("01_05030000_010");
        grvFUNCTION.Columns[5].HeaderText = BaseHelper.GetShowText("01_05030000_011");
    }

    /// <summary>
    /// GridView資料綁定
    /// </summary>
    private void BindGridView()
    {
        try
        {
            var sql = @"SELECT AchRtnInfoSeq, Ach_Rtn_Code, Ach_Rtn_Msg, EDDA_Rtn_Code, EDDA_Rtn_Msg FROM Ach_Rtn_Info";
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
