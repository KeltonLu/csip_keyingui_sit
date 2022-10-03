using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.EntityLayer_new;
using CSIPKeyInGUI.BusinessRules_new;
using Framework.Common.JavaScript;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using System.Data;
using CSIPKeyInGUI.BusinessRules;

public partial class Page_P000001100001 : System.Web.UI.Page
{
    private EntitySet<EntityPostOffice_CodeType> esPostOfficeCodeType = null;
    #region Event
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            ddlCodeTypeBind();
            ddlCodeType.SelectedValue = "1";
            EnableControls(EAction.Add);
        }
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        this.gpList.CurrentPageIndex = 1;
        BindGridView(this.txtCodeID.Text, this.txtCodeName.Text, this.txtCodeEnName.Text, this.txtDescription.Text);
        this.ViewState["SearchCode_ID"] = this.txtCodeID.Text;
        this.ViewState["SearchCode_Name"] = this.txtCodeName.Text;
        this.ViewState["SearchCode_EnName"] = this.txtCodeEnName.Text;
        this.ViewState["SearchDescription"] = this.txtDescription.Text;//20211206_Ares_Jack_增加描述
        //* 將畫面設置為添加狀態
        EnableControls(EAction.Add);
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        int orderBy = 0;
        if (!Validation(ddlCodeType.SelectedValue, "", false, out orderBy))
        {
            return;
        }
        string strMsg = string.Empty;
        EntityPostOffice_CodeType insertObj = EntityPostOfficeCodeTypeInitializers(txtCodeID.Text, txtCodeName.Text, txtCodeEnName.Text, orderBy, chkIsValid.Checked, txtDescription.Text);
        if (BRPostOffice_CodeType.Add(insertObj, ref strMsg))
        {
            CleanInput();
            this.BindGridView("", "", "", "");
        }

        jsBuilder.RegScript(this.UpdatePanel1, jsBuilder.GetAlert(strMsg));
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        int orderBy = 0;
        if (!Validation(ddlCodeType.SelectedValue, hidID.Value, true, out orderBy))
        {
            return;
        }
        string strMsg = string.Empty;
        EntityPostOffice_CodeType updateObj = EntityPostOfficeCodeTypeInitializers(txtCodeID.Text, txtCodeName.Text, txtCodeEnName.Text, orderBy, chkIsValid.Checked, txtDescription.Text);
        if (BRPostOffice_CodeType.Update(updateObj, ref strMsg))
        {
            EnableControls(EAction.Add);
            CleanInput();
            this.BindGridView("", "", "", "");
        }
        jsBuilder.RegScript(this.UpdatePanel1, jsBuilder.GetAlert(strMsg));
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (!CheckRadio())
        {
            return;
        }
        string strMsg = string.Empty;
        if (BRPostOffice_CodeType.Delete(ddlCodeType.SelectedValue, hidID.Value, ref strMsg))
        {
            EnableControls(EAction.Add);
            this.gpList.CurrentPageIndex = 1;
            this.BindGridView("", "", "", "");
            CleanInput();
        }
        jsBuilder.RegScript(this.UpdatePanel1, jsBuilder.GetAlert(strMsg));
    }

    /// <summary>
    /// Cancel Button Click Event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        EnableControls(EAction.Add);
        CleanInput();
        //將RadioButton設置為非選中狀態
        if (this.gvpbCodeInfo.Rows.Count > 0)
        {
            for (int intLoop = 0; intLoop < this.gvpbCodeInfo.Rows.Count; intLoop++)
            {
                HtmlInputRadioButton radCode = (HtmlInputRadioButton)this.gvpbCodeInfo.Rows[intLoop].FindControl("radCode");
                radCode.Checked = false;
            }
        }
    }

    /// <summary>
    /// UpdateCode Button Click Event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdateCode_Click(object sender, EventArgs e)
    {
        int intRowIndex = 0;
        for (int intIndex = 0; intIndex < this.gvpbCodeInfo.Rows.Count; intIndex++)
        {
            HtmlInputRadioButton radCode = (HtmlInputRadioButton)this.gvpbCodeInfo.Rows[intIndex].FindControl("radCode");
            if (radCode.Checked)
            {
                intRowIndex = intIndex;
                break;
            }
        }
        //將畫面設置為修改、刪除狀態
        EnableControls(EAction.Edit);
        this.hidID.Value = this.gvpbCodeInfo.Rows[intRowIndex].Cells[1].Text;
        this.txtCodeID.Text = this.gvpbCodeInfo.Rows[intRowIndex].Cells[2].Text;
        //20220601_Ares_Jack_ &nbsp; 改為空值
        string chName = this.gvpbCodeInfo.Rows[intRowIndex].Cells[3].Text;
        this.txtCodeName.Text = chName.Equals("&nbsp;") ? "" : chName;

        string enName = this.gvpbCodeInfo.Rows[intRowIndex].Cells[4].Text;
        this.txtCodeEnName.Text = enName.Equals("&nbsp;") ? "" : enName;
        this.txtOrderBy.Text = this.gvpbCodeInfo.Rows[intRowIndex].Cells[5].Text;
        this.txtDescription.Text = this.gvpbCodeInfo.Rows[intRowIndex].Cells[6].Text;//20211206_Ares_Jack_增加描述
        this.chkIsValid.Checked = StringToBool(this.gvpbCodeInfo.Rows[intRowIndex].Cells[7].Text);
    }

    protected void gvpbCodeInfo_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HtmlInputRadioButton radRole = (HtmlInputRadioButton)e.Row.FindControl("radCode");
            radRole.Checked = false;
            if (e.Row.Cells[7].Text == "False")
            {
                e.Row.Cells[7].Text = "N";
            }
            else
            {
                e.Row.Cells[7].Text = "Y";
            }
        }
    }

    protected void gpList_PageChanged(object sender, PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        string searchCode_ID = this.ViewState["SearchCode_ID"] == null ? "" : this.ViewState["SearchCode_ID"].ToString();
        string searchCode_Name = this.ViewState["SearchCode_Name"] == null ? "" : this.ViewState["SearchCode_Name"].ToString();
        string searchCode_EnName = this.ViewState["SearchCode_EnName"] == null ? "" : this.ViewState["SearchCode_EnName"].ToString();
        string searchDescription = this.ViewState["SearchDescription"] == null ? "" : this.ViewState["SearchDescription"].ToString();//20211206_Ares_Jack_增加描述
        this.BindGridView(searchCode_ID, searchCode_Name, searchCode_EnName, searchDescription);
        //*將畫面設置為添加狀態
        EnableControls(EAction.Add);
        CleanInput();
    }
    #endregion

    #region others Method
    /// <summary>
    /// 根據狀態設置畫面控件的enabled或disable
    /// </summary>
    /// <param name="enumStep"></param>
    private void EnableControls(EAction enumStep)
    {
        if (enumStep == EAction.Add)
        {
            this.ddlCodeType.Enabled = true;
            this.txtCodeName.Enabled = true;
            this.txtDescription.Enabled = true;

            this.btnAdd.Enabled = true;
            this.btnDelete.Enabled = false;
            this.btnUpdate.Enabled = false;
            this.btnCancel.Enabled = false;
        }
        else if (enumStep == EAction.Edit)
        {
            this.ddlCodeType.Enabled = false;
            this.txtCodeName.Enabled = true;
            this.txtDescription.Enabled = true;

            this.btnAdd.Enabled = false;
            this.btnDelete.Enabled = true;
            this.btnUpdate.Enabled = true;
            this.btnCancel.Enabled = true;
        }
    }

    /// <summary>
    /// 維護類型選單
    /// </summary>
    private void ddlCodeTypeBind()
    {
        //20191213-RQ-2019-030155-002需求，提供變動性參數化設定
        //ddlCodeType.Items.Add(new ListItem("國籍", "1"));
        //ddlCodeType.Items.Add(new ListItem("法律形式", "2"));
        //ddlCodeType.Items.Add(new ListItem("行業別", "3"));

        DataTable dtCodeType = new DataTable();
        dtCodeType = BRPostOffice_CodeInfo.GetCodeInfoType();
        ddlCodeType.DataTextField = "TYPE_NAME";
        ddlCodeType.DataValueField = "TYPE";
        ddlCodeType.DataSource = dtCodeType;
        ddlCodeType.DataBind();
    }

    private bool StringToBool(string str)
    {
        bool result = false;
        if (!str.Equals("N"))
        {
            result = true;
        }
        return result;
    }

    //20200031-CSIP EOS Ares Luke 修改日期:2021/03/11 修改說明:白箱報告修正SQL Injection
    private string GetFilterCondition(string strCode_ID, string strCodeName, string strCodeEnName, string strDescription)
    {
        SqlHelper sqlHelper = new SqlHelper();
        sqlHelper.AddCondition(EntityPostOffice_CodeType.M_TYPE, Operator.Equal, DataTypeUtils.String, this.ddlCodeType.SelectedValue);

        if (!string.IsNullOrEmpty(strCode_ID) && !string.IsNullOrEmpty(strCode_ID.Trim()))
        {
            sqlHelper.AddCondition(EntityPostOffice_CodeType.M_CODE_ID, Operator.Equal, DataTypeUtils.String, BRCommon.EncodeForSQL(strCode_ID));
        }
        if (!string.IsNullOrEmpty(strCodeName) && !string.IsNullOrEmpty(strCodeName.Trim()))
        {
            sqlHelper.AddCondition(EntityPostOffice_CodeType.M_CODE_NAME, Operator.Equal, DataTypeUtils.String, BRCommon.EncodeForSQL(strCodeName));
        }
        if (!string.IsNullOrEmpty(strCodeEnName) && !string.IsNullOrEmpty(strCodeEnName.Trim()))
        {
            sqlHelper.AddCondition(EntityPostOffice_CodeType.M_CODE_EN_NAME, Operator.Equal, DataTypeUtils.String, BRCommon.EncodeForSQL(strCodeEnName));
        }
        if (!string.IsNullOrEmpty(strDescription) && !string.IsNullOrEmpty(strDescription.Trim()))
        {
            sqlHelper.AddCondition(EntityPostOffice_CodeType.M_DESCRIPTION, Operator.Equal, DataTypeUtils.String, BRCommon.EncodeForSQL(strDescription));
        }

        return sqlHelper.GetFilterCondition();
    }

    private void BindGridView(string strCode_ID, string strCodeName, string strCodeEnName, string strDescription)
    {
        this.gvpbCodeInfo.Columns[1].Visible = true; //ID參數
        esPostOfficeCodeType = BRPostOffice_CodeType.Search(this.GetFilterCondition(strCode_ID, strCodeName, strCodeEnName, strDescription), this.gpList.CurrentPageIndex, this.gpList.PageSize);
        try
        {
            this.gpList.RecordCount = esPostOfficeCodeType.TotalCount;
            this.gvpbCodeInfo.DataSource = esPostOfficeCodeType;
            this.gvpbCodeInfo.DataBind();
            this.gvpbCodeInfo.Columns[1].Visible = false;//ID隱藏
        }
        catch
        {
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("00_00000000_000"));
        }
    }

    private void CleanInput()
    {
        this.txtCodeID.Text = string.Empty;
        this.txtCodeName.Text = string.Empty;
        this.hidID.Value = string.Empty;
        this.txtCodeEnName.Text = string.Empty;
        this.txtOrderBy.Text = string.Empty;
        this.chkIsValid.Checked = false;
        this.txtDescription.Text = string.Empty;//20211206_Ares_Jack_增加描述
    }

    private bool Validation(string codeType, string id, bool checkRadio, out int orderBy)
    {
        bool result = true;
        if (checkRadio)
        {
            CheckRadio();
        }
        if (string.IsNullOrEmpty(txtCodeID.Text.Trim()))
        {
            jsBuilder.RegScript(this.UpdatePanel1, jsBuilder.GetAlert("請輸入代號"));
            txtCodeID.Focus();
            result = false;
        }
        if (codeType.Trim() != "21")//20220527_Ares_Jack_高風險行職業組合 不檢核中文名稱
        {
            if (string.IsNullOrEmpty(txtCodeName.Text.Trim()))
            {
                jsBuilder.RegScript(this.UpdatePanel1, jsBuilder.GetAlert("請輸入中文名稱"));
                txtCodeName.Focus();
                result = false;
            }
        }
        
        if (!int.TryParse(txtOrderBy.Text, out orderBy))
        {
            jsBuilder.RegScript(this.UpdatePanel1, jsBuilder.GetAlert("順序請輸入數字"));
            txtOrderBy.Focus();
            result = false;
        }
        //20211206_Ares_Jack_增加描述
        if (codeType == "3" || codeType == "16")//類型是 行業編號, 職稱編號
        {
            if (string.IsNullOrEmpty(txtDescription.Text.Trim()))
            {
                jsBuilder.RegScript(this.UpdatePanel1, jsBuilder.GetAlert("請於描述欄位輸入對應大項編號"));
                txtDescription.Focus();
                result = false;
            }
        }
        if (BRPostOffice_CodeType.CheckRepeat(codeType, id, txtCodeID.Text.Trim()))
        {
            jsBuilder.RegScript(this.UpdatePanel1, jsBuilder.GetAlert("已存在相同代號，請輸入代號查詢後進行編輯更新"));
            txtCodeID.Focus();
            result = false;
        }
        return result;
    }

    private EntityPostOffice_CodeType EntityPostOfficeCodeTypeInitializers(string code_ID, string code_NAME, string code_EN_NAME, int orderBy, bool isValid, string Description)
    {
        EntityPostOffice_CodeType ePOCT = new EntityPostOffice_CodeType();
        if (!string.IsNullOrEmpty(hidID.Value))
            ePOCT.ID = Convert.ToInt32(hidID.Value);
        ePOCT.TYPE = ddlCodeType.SelectedValue;
        ePOCT.CODE_ID = code_ID;
        ePOCT.CODE_NAME = code_NAME;
        ePOCT.CODE_EN_NAME = code_EN_NAME;
        ePOCT.ORDERBY = orderBy;
        ePOCT.DESCRIPTION = string.Empty;
        ePOCT.IsValid = isValid;
        ePOCT.DESCRIPTION = Description;
        return ePOCT;
    }

    private bool CheckRadio()
    {
        bool blnSelected = false;
        for (int intLoop = 0; intLoop < this.gvpbCodeInfo.Rows.Count; intLoop++)
        {
            HtmlInputRadioButton radRole = (HtmlInputRadioButton)this.gvpbCodeInfo.Rows[intLoop].FindControl("radCode");
            if (radRole.Checked)
            {
                blnSelected = true;
                break;
            }
        }
        if (!blnSelected)
        {
            jsBuilder.RegScript(this.UpdatePanel1, jsBuilder.GetAlert("畫面必須勾選一筆"));
        }
        return blnSelected;
    }


    #endregion

}