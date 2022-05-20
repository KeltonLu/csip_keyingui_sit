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

public partial class P010801120001 : PageBase
{
    #region 變數區
    /// <summary>
    /// Session變數集合
    /// </summary>
    //private Entity_NoteLog thisEntity_NoteLog;

    private EntityAGENT_INFO eAgentInfo;
    public String Table_HtmlValue = String.Empty;

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
                ddlSearch_USER_STATUS.SelectedIndex = 0;
                Entity_AML_AGENT_SETTING objSearch = new Entity_AML_AGENT_SETTING();
                objSearch.USER_STATUS = ddlSearch_USER_STATUS.SelectedValue;
                Search(objSearch,false);
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
        int iSuccessCount = 0;
        Boolean blSuccess = false;
        string urlString = String.Empty;
        try
        {
            String strMsg = CheckInput();

            if (!String.IsNullOrEmpty(strMsg))
            {
                urlString = @"alert('" + strMsg + "');";
                base.sbRegScript.Append(urlString);
            }
            else
            {
                if (!String.IsNullOrEmpty(hidRowCount.Value) && Convert.ToInt32(hidRowCount.Value) > 0)
                {
                    int DataRowConut = Convert.ToInt32(hidRowCount.Value);
                    List<Entity_AML_AGENT_SETTING> listAML_AGENT_SETTING = new List<Entity_AML_AGENT_SETTING>();

                    for (int i = 0; i < DataRowConut; i++)
                    {
                        TextBox txtDataRow = (TextBox)this.GridView1.Rows[i].FindControl("txtASSIGN_RATE");
                        DropDownList ddlDataRow = (DropDownList)this.GridView1.Rows[i].FindControl("ddlUSER_STATUS");

                        Entity_AML_AGENT_SETTING loopObj = new Entity_AML_AGENT_SETTING();
                        loopObj.USER_ID = this.GridView1.DataKeys[i]["USER_ID"].ToString();
                        loopObj.USER_NAME = this.GridView1.DataKeys[i]["USER_NAME"].ToString();
                        loopObj.USER_STATUS = ddlDataRow.SelectedValue;
                        loopObj.ADD_DATE = DateTime.Now;
                        loopObj.UPDATE_DATE = DateTime.Now;

                        if (eAgentInfo != null)
                        {
                            loopObj.ADD_USER_ID = eAgentInfo.agent_id;
                            loopObj.ADD_USER_NAME = eAgentInfo.agent_name;
                            loopObj.MODI_USER_ID = eAgentInfo.agent_id;
                            loopObj.MODI_USER_NAME = eAgentInfo.agent_name;
                        }

                        if (hidUSER_STATUS.Value == "1" && ddlDataRow.SelectedValue == "1")
                        {
                            if (txtDataRow != null && !String.IsNullOrEmpty(txtDataRow.Text))
                            {
                                loopObj.ASSIGN_RATE = Convert.ToInt32(txtDataRow.Text);
                                //iRatioSum += loopObj.ASSIGN_RATE;
                            }
                        }
                        else if (hidUSER_STATUS.Value == "0")
                        {
                            loopObj.ASSIGN_RATE = 0;

                            if (ddlDataRow.SelectedValue == hidUSER_STATUS.Value)
                                continue;
                        }

                        listAML_AGENT_SETTING.Add(loopObj);
                    }

                    blSuccess = AML_AGENT_SETTING.UPDATE_Obj_List(listAML_AGENT_SETTING, ref iSuccessCount);

                    if (listAML_AGENT_SETTING != null && listAML_AGENT_SETTING.Count > 0)
                    {
                        if (blSuccess)
                            urlString = @"alert('成功異動:" + iSuccessCount.ToString() + "筆');";
                    }
                    else
                        urlString = @"alert('無資料異動');";

                    base.sbRegScript.Append(urlString);

                    Entity_AML_AGENT_SETTING objSearch = new Entity_AML_AGENT_SETTING();
                    objSearch.USER_STATUS = hidUSER_STATUS.Value;
                    Search(objSearch,true);
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
    }

    //protected void btnCancel_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //    }
    //    catch (Exception ex)
    //    {
    //        Logging.Log(ex, LogLayer.UI);
    //    }
    //}

    protected void btnSearch_Click(object sender, EventArgs e)
    {
    }

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            switch (e.Row.RowType)
            {
                case DataControlRowType.Header:
                    e.Row.CssClass = "Grid_Header";
                    e.Row.Cells[0].Text = WebHelper.GetShowText("01_01080112_010");//流水號
                    e.Row.Cells[1].Text = WebHelper.GetShowText("01_01080112_001");//處理經辦
                    e.Row.Cells[2].Text = WebHelper.GetShowText("01_01080112_002");//派案比例
                    e.Row.Cells[3].Text = WebHelper.GetShowText("01_01080112_007");//狀態

                    //將不必要欄位隱藏
                    if (e.Row.Cells.Count > 5)
                    {
                        for (int i = 4; i < e.Row.Cells.Count; i++)
                        {
                            e.Row.Cells[i].Visible = false;
                            //e.Row.Cells[0].Visible = true;
                            //e.Row.Cells[e.Row.Cells.Count - 1].Visible = true;
                        }
                    }

                    break;
                case DataControlRowType.DataRow:
                    TextBox txtDataRow = (TextBox)e.Row.FindControl("txtASSIGN_RATE");
                    DropDownList ddlDataRow = (DropDownList)e.Row.FindControl("ddlUSER_STATUS");

                    String strUSER_ID = this.GridView1.DataKeys[e.Row.RowIndex]["USER_ID"].ToString();
                    String strUSER_NAME = this.GridView1.DataKeys[e.Row.RowIndex]["USER_NAME"].ToString();
                    String strASSIGN_RATE = this.GridView1.DataKeys[e.Row.RowIndex]["ASSIGN_RATE"].ToString();
                    //String strNew_STATUS = this.GridView1.DataKeys[e.Row.RowIndex]["New_STATUS"].ToString();
                    String strUSER_STATUS = this.GridView1.DataKeys[e.Row.RowIndex]["USER_STATUS"].ToString();

                    ddlDataRow.SelectedValue = strUSER_STATUS;

                    if (strUSER_STATUS == "0")
                    {
                        txtDataRow.Text = "0";
                        txtDataRow.Enabled = false;
                    }
                    else
                    {
                        txtDataRow.Text = strASSIGN_RATE;
                    }

                    if (e.Row.RowIndex % 2 == 0)
                        e.Row.CssClass = "Grid_AlternatingItem";
                    else
                        e.Row.CssClass = "Grid_Item";

                    if (e.Row.Cells.Count > 5)
                    {
                        for (int i = 4; i < e.Row.Cells.Count; i++)
                        {
                            e.Row.Cells[i].Visible = false;
                        }
                    }

                    break;
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
    }
    
    protected void ddlSearch_USER_STATUS_SelectedIndexChanged(object sender, EventArgs e)
    {
        Entity_AML_AGENT_SETTING objSearch = new Entity_AML_AGENT_SETTING();
        objSearch.USER_STATUS = ddlSearch_USER_STATUS.SelectedValue;
        Search(objSearch,false);
    }
    #endregion

    #region 方法區

    private void Search(Entity_AML_AGENT_SETTING objSearch, Boolean isModify)
    {
        try
        {
            DataTable dtGridViewSource = new DataTable();
            AML_AGENT_SETTING.GetAML_AGENT_SETTING_WebListDataTable_V2(objSearch, "CSIP0120", "", ref dtGridViewSource);
            hidUSER_STATUS.Value = ddlSearch_USER_STATUS.SelectedValue;

            if (dtGridViewSource != null && dtGridViewSource.Rows.Count > 0)
            {
                hidRowCount.Value = dtGridViewSource.Rows.Count.ToString();

                GridView1.DataSource = dtGridViewSource;
                GridView1.DataKeyNames = new String[] { "USER_ID", "USER_NAME", "ASSIGN_RATE", "USER_STATUS" };
                GridView1.DataBind();
                GridView1.Visible = true;
            }
            else
            {
                GridView1.Visible = false;

                if (!isModify)
                {
                    string urlString = @"alert('查無資料');";
                    base.sbRegScript.Append(urlString);
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
    }

    private String CheckInput()
    {
        String strMsg = String.Empty;
        String strTempMsg1 = String.Empty;
        String strTempMsg2 = String.Empty;
        try
        {
            strTempMsg1 = WebHelper.GetShowText("01_01080112_012");
            strTempMsg2 = WebHelper.GetShowText("01_01080112_013");
            if (!String.IsNullOrEmpty(hidRowCount.Value) && Convert.ToInt32(hidRowCount.Value) > 0)
            {
                int iRatioSum = 0;
                int DataRowConut = Convert.ToInt32(hidRowCount.Value);
                List<Entity_AML_AGENT_SETTING> listAML_AGENT_SETTING = new List<Entity_AML_AGENT_SETTING>();

                for (int i = 0; i < DataRowConut; i++)
                {
                    TextBox txtDataRow = (TextBox)this.GridView1.Rows[i].FindControl("txtASSIGN_RATE");
                    DropDownList ddlDataRow = (DropDownList)this.GridView1.Rows[i].FindControl("ddlUSER_STATUS");

                    if (IsNumeric(txtDataRow.Text) == false)
                    {
                        if (!String.IsNullOrEmpty(strMsg))
                            strMsg += "\\n";
                        strMsg += strTempMsg1.Replace("[Row]", i.ToString());
                    }
                    else if (hidUSER_STATUS.Value == "1" && ddlDataRow.SelectedValue == "1")
                    {
                        if (txtDataRow != null && !String.IsNullOrEmpty(txtDataRow.Text))
                        {
                            iRatioSum += Convert.ToInt32(txtDataRow.Text);
                        }
                    }
                }

                if (hidUSER_STATUS.Value == "1" && iRatioSum != 100 && iRatioSum != 0 && String.IsNullOrEmpty(strMsg))
                {
                    strMsg = strTempMsg2 + iRatioSum.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
        }
        return strMsg;
    }

    public bool IsNumeric(String strNumber)
    {
        Regex NumberPattern = new Regex("[^0-9]");
        return !NumberPattern.IsMatch(strNumber);
    }
    #endregion
}
