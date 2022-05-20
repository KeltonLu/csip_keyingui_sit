//******************************************************************
//*  作    者：趙呂梁  

//*  功能說明：卡人資料異動-異動姓名(生日)

//*  創建日期：2009/10/06
//*  修改記錄：
//*  2010/12/21   chaoma      RQ-2010-005537-000      增設(□P4及□P4D)作業選項及【自動翻譯】按鈕 

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
using CSIPCommonModel.EntityLayer;
using CSIPKeyInGUI.EntityLayer;
using Framework.Common.Message;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Logging;
using Framework.Common.Utility;
using CSIPKeyInGUI.EntityLayer_new;
using CSIPNewInvoice.EntityLayer_new;
using System.Collections.Generic;
using System.Text.RegularExpressions;//導入命名空間(正則表達式)

public partial class P010801130001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    //private Entity_NoteLog thisEntity_NoteLog;

    private EntityAGENT_INFO eAgentInfo;
    public String Table_HtmlValue = String.Empty;
    public int intPageSize = 15;

    #endregion

    #region 事件區
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];// Session變數集合
            string userRolls = eAgentInfo.roles;

            if (!userRolls.Contains("CSIP0124"))
            {
                sbRegScript.Append("alert('無此功能權限');top.location.href=top.location.href;");
            }

            if (!IsPostBack)
            {
                InitPage();
                //*設置GridView分頁顯示的行數
                gpList.PageSize = intPageSize;
                GridView1.PageSize = intPageSize;
                gpList.Visible = false;
                GridView1.Visible = false;

                btnSelectAll.Visible = false;
                btnClearAll.Visible = false;
                pnlText.Visible = false;

                //ddlSearch_USER_STATUS.SelectedIndex = 0;
                //Entity_AML_AGENT_SETTING objSearch = new Entity_AML_AGENT_SETTING();
                //objSearch.USER_STATUS = ddlSearch_USER_STATUS.SelectedValue;

                //Search(objSearch);
            }
            base.strClientMsg = "";
            base.strHostMsg = "";
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        Int32 iSuccessCount = 0;
        Int32 iDataRowCount = intPageSize;
        Boolean blSuccess = false;
        List<EntityAML_HQ_Work> listEntityAML_HQ_Work = new List<EntityAML_HQ_Work>();
        List<Entity_NoteLog> listEntity_NoteLog = new List<Entity_NoteLog>();
        String strID_List = String.Empty;
        string urlString = String.Empty;

        try
        {


            if (ddlNew_CaseOwner_User.SelectedItem != null && !String.IsNullOrEmpty(ddlNew_CaseOwner_User.SelectedItem.Value))
            {
                if (ViewState["DataBind"] != null)
                {
                    DataTable dtblResult = (DataTable)ViewState["DataBind"];
                    if (dtblResult != null && dtblResult.Rows.Count > 0)
                    {
                        if (dtblResult.Rows.Count < intPageSize)
                            iDataRowCount = dtblResult.Rows.Count;

                        for (int i = 0; i < iDataRowCount; i++)
                        {
                            CheckBox chbSelect = (CheckBox)this.GridView1.Rows[i].FindControl("chbSelect");
                            int iID = 0;
                            //String strRowNum = this.GridView1.DataKeys[i]["tempRowNum"].ToString();
                            String strCASE_NO = this.GridView1.DataKeys[i]["CASE_NO"].ToString();
                            //String strHCOP_HEADQUATERS_CORP_NO = this.GridView1.DataKeys[i]["HCOP_HEADQUATERS_CORP_NO"].ToString();
                            String strUSER_ID = this.GridView1.DataKeys[i]["USER_ID"].ToString();
                            String strUSER_NAME = this.GridView1.DataKeys[i]["USER_NAME"].ToString();

                            if (!String.IsNullOrEmpty(this.GridView1.DataKeys[i]["ID"].ToString()) && IsNumeric(this.GridView1.DataKeys[i]["ID"].ToString()))
                                iID = Convert.ToInt32(this.GridView1.DataKeys[i]["ID"].ToString());

                            if (chbSelect.Checked == true && iID > 0)
                            {
                                EntityAML_HQ_Work loopObj_M = new EntityAML_HQ_Work();
                                loopObj_M.ID = this.GridView1.DataKeys[i]["ID"].ToString();
                                loopObj_M.CASE_NO = strCASE_NO;
                                loopObj_M.CaseOwner_User = ddlNew_CaseOwner_User.SelectedValue;
                                listEntityAML_HQ_Work.Add(loopObj_M);

                                Entity_NoteLog loopObj = new Entity_NoteLog();
                                loopObj.NL_CASE_NO = strCASE_NO;
                                loopObj.NL_Type = "ChangeCaseOwner";
                                loopObj.NL_User = eAgentInfo.agent_id;
                                loopObj.NL_Value = "案件轉派:由" + strUSER_NAME + "(" + strUSER_ID + ") 變更為 " + ddlNew_CaseOwner_User.SelectedItem.Text;
                                loopObj.NL_DateTime = DateTime.Now;
                                loopObj.NL_ShowFlag = "1";
                                loopObj.NL_SecondKey = "";
                                listEntity_NoteLog.Add(loopObj);
                            }
                        }

                        blSuccess = BRAML_HQ_Work.UPDATE_Obj_CaseOwner_User(listEntityAML_HQ_Work, ddlNew_CaseOwner_User.SelectedItem.Value, listEntity_NoteLog, ref iSuccessCount);

                        if (blSuccess)
                        {
                            urlString = @"alert('" + WebHelper.GetShowText("01_01080113_009") + "');";
                            base.sbRegScript.Append(urlString);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DataTable dtGridViewSource = new DataTable();
        try
        {
            //將分頁回到第一頁
            this.gpList.CurrentPageIndex = 1;

            EntityAML_HQ_Work objSearch = new EntityAML_HQ_Work();

            if (!String.IsNullOrEmpty(txtCASE_NO.Text))
                objSearch.CASE_NO = txtCASE_NO.Text;

            if (!String.IsNullOrEmpty(txtHCOP_HEADQUATERS_CORP_NO.Text))
                objSearch.HCOP_HEADQUATERS_CORP_NO = txtHCOP_HEADQUATERS_CORP_NO.Text;

            if (!String.IsNullOrEmpty(txtCaseOwner_User.Text) && txtCaseOwner_User.Text != "請選擇")
            {
                if (txtCaseOwner_User.Text.IndexOf("(") > 0 && txtCaseOwner_User.Text.IndexOf(")") > 0)
                {
                    String strID = String.Empty;
                    strID = txtCaseOwner_User.Text.Substring(txtCaseOwner_User.Text.IndexOf("(") + 1, txtCaseOwner_User.Text.IndexOf(")") - txtCaseOwner_User.Text.IndexOf("(") - 1);
                    objSearch.CaseOwner_User = strID;
                }
                else
                {
                    objSearch.CaseOwner_User = txtCaseOwner_User.Text;
                }
            }

            BRAML_HQ_Work.GetAML_HQ_Work_ForP010801130001(objSearch, "CSIP0120", "", ref dtGridViewSource);

            if (dtGridViewSource != null && dtGridViewSource.Rows.Count > 0)
            {
                ViewState["DataBind"] = dtGridViewSource;
                BindGridView();
            }
            else
            {
                ViewState["DataBind"] = new DataTable();
                BindGridView();
            }

            //{
            //    string urlString = @"alert('查無資料');";
            //    base.sbRegScript.Append(urlString);

            //    this.gpList.Visible = false;
            //}
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
    }

    #endregion

    #region GridView相關

    /// <summary>
    /// 逐行判斷
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            switch (e.Row.RowType)
            {
                case DataControlRowType.Header:
                    e.Row.CssClass = "Grid_Header";
                    e.Row.Cells[1].Text = WebHelper.GetShowText("01_01080113_006");//流水號
                    e.Row.Cells[2].Text = WebHelper.GetShowText("01_01080113_001");//案件編號
                    e.Row.Cells[3].Text = WebHelper.GetShowText("01_01080113_002");//統一編號
                    e.Row.Cells[4].Text = WebHelper.GetShowText("01_01080113_010");//登記名稱
                    e.Row.Cells[5].Text = WebHelper.GetShowText("01_01080113_003");//經辦人員

                    break;
                case DataControlRowType.DataRow:
                    CheckBox chbSelect = (CheckBox)e.Row.FindControl("chbSelect");

                    //String strRowNum = this.GridView1.DataKeys[e.Row.RowIndex]["tempRowNum"].ToString();
                    //String strID = this.GridView1.DataKeys[e.Row.RowIndex]["ID"].ToString();
                    //String strCASE_NO = this.GridView1.DataKeys[e.Row.RowIndex]["CASE_NO"].ToString();
                    //String strHCOP_HEADQUATERS_CORP_NO = this.GridView1.DataKeys[e.Row.RowIndex]["HCOP_HEADQUATERS_CORP_NO"].ToString();
                    //String strUSER_ID = this.GridView1.DataKeys[e.Row.RowIndex]["USER_ID"].ToString();
                    //String strUSER_NAME = this.GridView1.DataKeys[e.Row.RowIndex]["USER_NAME"].ToString();

                    chbSelect.Checked = false;

                    if (e.Row.RowIndex % 2 == 0)
                        e.Row.CssClass = "Grid_AlternatingItem";
                    else
                        e.Row.CssClass = "Grid_Item";

                    break;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
    }

    /// <summary>
    /// 綁定資料
    /// </summary>
    private void BindGridView()
    {
        try
        {
            if (ViewState["DataBind"] != null)
            {
                DataTable dtblResult = (DataTable)ViewState["DataBind"];
                if (dtblResult != null && dtblResult.Rows.Count > 0)
                {
                    this.gpList.Visible = true;
                    this.GridView1.Visible = true;
                    this.gpList.RecordCount = dtblResult.Rows.Count;
                    this.GridView1.DataSource = CommonFunction.Pagination(dtblResult, this.gpList.CurrentPageIndex, this.gpList.PageSize);
                    GridView1.DataKeyNames = new String[] { "tempRowNum", "ID", "CASE_NO", "HCOP_HEADQUATERS_CORP_NO", "USER_ID", "USER_NAME" };
                    this.GridView1.DataBind();

                    btnSelectAll.Visible = true;
                    btnClearAll.Visible = true;
                    pnlText.Visible = true;
                }
                else
                {
                    this.gpList.Visible = true;
                    this.GridView1.Visible = true;
                    this.gpList.RecordCount = 0;
                    this.GridView1.DataSource = CommonFunction.Pagination(dtblResult, this.gpList.CurrentPageIndex, this.gpList.PageSize);
                    this.GridView1.DataBind();

                    btnSelectAll.Visible = false;
                    btnClearAll.Visible = false;
                    pnlText.Visible = false;
                }
            }
        }
        catch (Exception ex)
        {
            //base.strClientMsg += MessageHelper.GetMessage("00_00000000_000");
            Logging.Log(ex, LogLayer.UI);
        }
    }

    #endregion

    #region 方法區

    private void InitPage()
    {
        DataTable dtDDL_DataSource = new DataTable();
        DataTable dtDDL_DataSource_New = new DataTable();
        try
        {
            //綁定所有的經辦人員(含已失效 或 派案比例為0的人員)
            AML_AGENT_SETTING.GetAML_AGENT_SETTING_WebListDataTable_V2(new Entity_AML_AGENT_SETTING(), "CSIP0120", "", ref dtDDL_DataSource);
            if (dtDDL_DataSource != null && dtDDL_DataSource.Rows.Count > 0)
            {
                ddlCaseOwner_User.DataSource = dtDDL_DataSource;
                ddlCaseOwner_User.DataTextField = "Show_NAME";
                ddlCaseOwner_User.DataValueField = "USER_ID";
                ddlCaseOwner_User.DataBind();

                ddlCaseOwner_User.Items.Insert(0, "請選擇");
                txtCaseOwner_User.Text = "請選擇";
            }

            //綁定所有的經辦人員(含派案比例為0的人員)
            Entity_AML_AGENT_SETTING objEntity_AML_AGENT_SETTING = new Entity_AML_AGENT_SETTING();
            objEntity_AML_AGENT_SETTING.USER_STATUS = "1";
            AML_AGENT_SETTING.GetAML_AGENT_SETTING_WebListDataTable_V2(objEntity_AML_AGENT_SETTING, "CSIP0120", "", ref dtDDL_DataSource_New);
            if (dtDDL_DataSource_New != null && dtDDL_DataSource_New.Rows.Count > 0)
            {
                ddlNew_CaseOwner_User.DataSource = dtDDL_DataSource_New;
                ddlNew_CaseOwner_User.DataTextField = "Show_NAME";
                ddlNew_CaseOwner_User.DataValueField = "USER_ID";
                ddlNew_CaseOwner_User.DataBind();

                ddlNew_CaseOwner_User.Items.Insert(0, "請選擇");
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
        finally
        {
            if (dtDDL_DataSource != null)
            {
                dtDDL_DataSource.Clear();
                dtDDL_DataSource.Dispose();
            }
        }
    }

    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }

    /// <summary>
    /// 判斷是否為正整數
    /// </summary>
    /// <param name="strNumber"></param>
    /// <returns></returns>
    public bool IsNumeric(String strNumber)
    {
        Regex NumberPattern = new Regex("[^0-9]");
        return !NumberPattern.IsMatch(strNumber);
    }

    #endregion
}
