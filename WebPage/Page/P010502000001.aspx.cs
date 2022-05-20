//******************************************************************
//*  作    者：蘇洺葳
//*  功能說明：自訂參數維護
//*  創建日期：2018/05/16
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
using System.Text.RegularExpressions;
using Framework.Common.Utility;
using Framework.Common.Message;
using Framework.Common.JavaScript;
using Framework.Common.Logging;
using Framework.WebControls;
using Framework.Data.OM.Collections;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BusinessRules;

public partial class P010502000001 : PageBase
{

    /// <summary>
    /// Session變數集合
    /// </summary>
    private static EntityAGENT_INFO eAgentInfo;

    /// <summary>
    /// Client訊息
    /// </summary>
    private static string ClientMsg;

    /// <summary>
    /// 使用類別編號集合
    /// </summary>
    private static DataTable dtblType;

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = BaseHelper.GetShowText("00_01040000_000");
        if (!Page.IsPostBack)
        {
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(""));

            Show();
            txtElementName.Focus();
            btnOK.Enabled = false;
            IntBusinessType();
            this.BindGridView();
        }

        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
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
            //使用CommandArgument記錄
            CustLinkButton LbtnModify = (CustLinkButton)e.Row.FindControl("lbtnModify");
            LbtnModify.Text = BaseHelper.GetShowText("01_05020000_016");
        }
    }

    /// <summary>
    /// 點擊資料列里的修改
    /// 修改紀錄:2021/05/07_Ares_Stanley-修正修改資料文字欄位會變成編輯欄位
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvFUNCTION_RowSelecting(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            string code = string.Empty;
            string name = string.Empty;
            LinkButton lb = e.CommandSource as LinkButton;
            GridViewRow row = lb.NamingContainer as GridViewRow;
            Int32 idx = row.RowIndex;
            // TextBox
            this.txtElementCode.Text = this.grvFUNCTION.Rows[idx].Cells[0].Text;
            this.txtElementName.Text = this.grvFUNCTION.Rows[idx].Cells[1].Text;
            this.txtElementID.Text = this.grvFUNCTION.Rows[idx].Cells[2].Text;
            this.txtSequence.Text = this.grvFUNCTION.Rows[idx].Cells[3].Text;
            this.txtCheckFlag.Text = this.grvFUNCTION.Rows[idx].Cells[5].Text;
            this.txtValueLength.Text = this.grvFUNCTION.Rows[idx].Cells[6].Text;
            this.txtDefaultValue.Text = this.grvFUNCTION.Rows[idx].Cells[7].Text.Replace("&nbsp;", "");
            this.txtRemark.Text = this.grvFUNCTION.Rows[idx].Cells[9].Text.Replace("&nbsp;", "");
            // CheckBox
            this.chkIsRequired.Checked = this.grvFUNCTION.Rows[idx].Cells[4].Text == "True" ? true : false;

            #region 使用類別編號

            string[] propertyCodes = this.grvFUNCTION.Rows[idx].Cells[8].Text.Split('|');

            // 重置使用類別編號CheckBox
            IniChkBusinessType();

            // 目前修改的自訂參數屬於哪些使用類別編號
            foreach (string strCode in propertyCodes)
            {
                for (int i = 0; i < chkBusinessType.Items.Count; i++)
                {
                    if (strCode == chkBusinessType.Items[i].Text)
                        chkBusinessType.Items[i].Selected = true;
                }
            }

            #endregion

            // 鎖定修改時無法變更的控制項
            this.txtElementID.Enabled = false;
            this.txtValueLength.Enabled = false;

            this.btnADD.Enabled = false;
            this.btnOK.Enabled = true;
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
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView();
    }

    /// <summary>
    /// 添加(新增自訂參數)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnADD_Click(object sender, EventArgs e)
    {
        try
        {
            string strMsgID = string.Empty;

            // 控制項ID檢核
            if (BRFORM_COLUMN.IsRepeat(this.txtElementID.Text))
            {
                // 控制項ID重複
                ClientMsg = MessageHelper.GetMessage("01_05020000_006");
                base.sbRegScript.Append("alert('" + ClientMsg + "');");
                return;
            }

            EntityFORM_COLUMN eFormColumn = new EntityFORM_COLUMN();
            // 取得控制項輸入內容
            GetInputValue(ref eFormColumn);

            // 使用類別編號檢核
            if (string.IsNullOrEmpty(eFormColumn.PROPERTY_CODE))
            {
                ClientMsg = MessageHelper.GetMessage("01_05020000_008");
                base.sbRegScript.Append("alert('" + ClientMsg + "');");
                return;
            }

            if (!BRFORM_COLUMN.Add(eFormColumn, ref strMsgID))
            {
                // 更新不成功則在端末顯示
                ClientMsg = MessageHelper.GetMessage(strMsgID);
                base.sbRegScript.Append("alert('" + ClientMsg + "');");
                return;
            }
            else
            {
                // 更新成功
                this.gpList.CurrentPageIndex = 1;
                BindGridView();

                // 清空控制項內容
                IniElement();

                // 解鎖控制項
                this.txtElementID.Enabled = true;
                this.txtValueLength.Enabled = true;
                this.btnADD.Enabled = true;
                this.btnOK.Enabled = false;
                ClientMsg = MessageHelper.GetMessage(strMsgID);
                base.sbRegScript.Append("alert('" + ClientMsg + "');");
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }
    }

    /// <summary>
    /// 確定(修改自訂參數)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        try
        {
            string strMsgID = string.Empty;
            EntityFORM_COLUMN eFormColumn = new EntityFORM_COLUMN();
            // 取得控制項輸入內容
            GetInputValue(ref eFormColumn);

            // 使用類別編號檢核
            if (string.IsNullOrEmpty(eFormColumn.PROPERTY_CODE))
            {
                ClientMsg = MessageHelper.GetMessage("01_05020000_008");
                base.sbRegScript.Append("alert('" + ClientMsg + "');");
                return;
            }

            if (!BRFORM_COLUMN.Update(eFormColumn, this.txtElementCode.Text))
            {
                // 更新不成功則在端末顯示
                ClientMsg = MessageHelper.GetMessage("01_05020000_002");
                base.sbRegScript.Append("alert('" + ClientMsg + "');");
                return;
            }
            else
            {
                // 更新成功
                this.gpList.CurrentPageIndex = 1;
                BindGridView();

                // 清空控制項內容
                IniElement();

                // 解鎖控制項
                this.txtElementID.Enabled = true;
                this.txtValueLength.Enabled = true;
                this.btnADD.Enabled = true;
                this.btnOK.Enabled = false;
                ClientMsg = MessageHelper.GetMessage("01_05020000_001");
                base.sbRegScript.Append("alert('" + ClientMsg + "');");
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
        }
    }

    /// <summary>
    /// 初始化使用類別編號CheckBox
    /// </summary>
    private void IntBusinessType()
    {
        try
        {
            dtblType = new DataTable();
            if (!CSIPCommonModel.BusinessRules_new.BRM_PROPERTY_CODE.GetProperty("01", "41", null, ref dtblType))
            {
                // 初始化使用類別編號失敗
                ClientMsg = MessageHelper.GetMessage("01_05020000_006");
                base.sbRegScript.Append("alert('" + ClientMsg + "');");
            }

            chkBusinessType.DataSource = dtblType;
            this.chkBusinessType.DataTextField = "PROPERTY_NAME";
            this.chkBusinessType.DataValueField = "PROPERTY_CODE";
            this.chkBusinessType.DataBind();
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
    }

    /// <summary>
    /// 取得控制項輸入內容
    /// </summary>
    /// <param name="eFormColumn"></param>
    private void GetInputValue(ref EntityFORM_COLUMN eFormColumn)
    {
        eFormColumn.ELEMENT_CODE = this.txtElementCode.Text;
        eFormColumn.ELEMENT_NAME = this.txtElementName.Text;
        eFormColumn.ELEMENT_ID = this.txtElementID.Text;
        eFormColumn.SEQUENCE = int.Parse(this.txtSequence.Text);
        eFormColumn.IS_REQUIRED = chkIsRequired.Checked == true ? true : false;
        eFormColumn.CHECK_FLAG = int.Parse(this.txtCheckFlag.Text);
        eFormColumn.VALUE_LENGTH = string.IsNullOrEmpty(this.txtValueLength.Text) ? 50 : int.Parse(this.txtValueLength.Text);
        eFormColumn.DEFAULT_VALUE = this.txtDefaultValue.Text;
        eFormColumn.REMARK = this.txtRemark.Text;
        eFormColumn.USER_ID = eAgentInfo.agent_id.ToString().Trim();
        eFormColumn.MOD_DATE = DateTime.Now.ToString("yyyyMMdd");

        string propertyCode = "";

        // 使用類別編號
        for (int i = 0; i < chkBusinessType.Items.Count; i++)
        {
            if (chkBusinessType.Items[i].Selected)
            {
                if (!string.IsNullOrEmpty(propertyCode)) propertyCode += "|";
                propertyCode += chkBusinessType.Items[i].Value;
            }
        }

        eFormColumn.PROPERTY_CODE = propertyCode;
    }

    /// <summary>
    /// 清空控制項內容
    /// </summary>
    private void IniElement()
    {
        // TextBox
        this.txtElementName.Text = "";
        this.txtElementID.Text = "";
        this.txtSequence.Text = "";
        this.txtCheckFlag.Text = "";
        this.txtValueLength.Text = "";
        this.txtDefaultValue.Text = "";
        this.txtRemark.Text = "";
        // CheckBox
        this.chkIsRequired.Checked = false;
        IniChkBusinessType();
    }

    /// <summary>
    /// 重置使用類別編號CheckBox
    /// </summary>
    private void IniChkBusinessType()
    {
        for (int i = 0; i < chkBusinessType.Items.Count; i++)
        {
            chkBusinessType.Items[i].Selected = false;
        }
    }

    /// <summary>
    /// GridView表頭訊息綁定
    /// </summary>
    private void Show()
    {
        grvFUNCTION.Columns[0].HeaderText = BaseHelper.GetShowText("01_05020000_002");
        grvFUNCTION.Columns[1].HeaderText = BaseHelper.GetShowText("01_05020000_003");
        grvFUNCTION.Columns[2].HeaderText = BaseHelper.GetShowText("01_05020000_004");
        grvFUNCTION.Columns[3].HeaderText = BaseHelper.GetShowText("01_05020000_005");
        grvFUNCTION.Columns[4].HeaderText = BaseHelper.GetShowText("01_05020000_006");
        grvFUNCTION.Columns[5].HeaderText = BaseHelper.GetShowText("01_05020000_007");
        grvFUNCTION.Columns[6].HeaderText = BaseHelper.GetShowText("01_05020000_008");
        grvFUNCTION.Columns[7].HeaderText = BaseHelper.GetShowText("01_05020000_009");
        grvFUNCTION.Columns[8].HeaderText = BaseHelper.GetShowText("01_05020000_010");
        grvFUNCTION.Columns[9].HeaderText = BaseHelper.GetShowText("01_05020000_011");
        grvFUNCTION.Columns[10].HeaderText = BaseHelper.GetShowText("01_05020000_012");
    }

    /// <summary>
    /// GridView資料綁定
    /// </summary>
    private void BindGridView()
    {
        string strMsgID = string.Empty;
        EntitySet<EntityFORM_COLUMN> eFormColumnSet = null;

        try
        {
            eFormColumnSet = BRFORM_COLUMN.Search("", this.gpList.CurrentPageIndex, this.gpList.PageSize);
            this.gpList.RecordCount = eFormColumnSet.TotalCount;
            this.grvFUNCTION.DataSource = eFormColumnSet;
            this.grvFUNCTION.DataBind();

            // 自動填入控制項代碼
            this.txtElementCode.Text = (eFormColumnSet.TotalCount + 1).ToString().PadLeft(4, '0');
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.BusinessRule);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("00_00000000_000"));
        }
    }
}
