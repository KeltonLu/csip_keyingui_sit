
//******************************************************************
//* 作    者：
//* 功能說明：
//* 創建日期：
//* 修改紀錄：
//* <author>            <time>            <TaskID>                <desc>
//* Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
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
using Framework.Common.Message;
using Framework.Data.OM.Collections;
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using Framework.Common.Utility;
using Framework.WebControls;

public partial class P010407000001 : PageBase
{
    #region event
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            setShowID();
            setrbTypeEnable(false);
            fillDateView("");
        }
    }

    protected void rbFunction_P_CheckedChanged(object sender, EventArgs e)
    {
        setrbTypeEnable(false);
        fillDateView("");
    }

    protected void rbFunction_C_CheckedChanged(object sender, EventArgs e)
    {
        setrbTypeEnable(true);
        fillDateView("");
    }

    protected void rbFunction_A_CheckedChanged(object sender, EventArgs e)
    {
        setrbTypeEnable(true);
        fillDateView("");
    }

    protected void rbType_Redeem_CheckedChanged(object sender, EventArgs e)
    {
        fillDateView("");
    }
    protected void rbType_Award_CheckedChanged(object sender, EventArgs e)
    {
        fillDateView("");
    }

    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        fillDateView("");
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        ButControl("A", "");
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        fillDateView(txtCode.Text);
    }

    protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[0].Text = BaseHelper.GetShowText("01_04070000_009");
            e.Row.Cells[1].Text = BaseHelper.GetShowText("01_04070000_010");
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            ((LinkButton)e.Row.Cells[2].Controls[0]).Attributes.Add("OnClick", "return confirm('是否確定刪除？');");
        }
    }

    protected void gvList_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        ButControl("D", ((CustGridView)sender).Rows[e.RowIndex].Cells[0].Text);
    }
    #endregion

    #region method
    private void setShowID()
    {
        rbFunction_P.Text = BaseHelper.GetShowText("01_04070000_002");
        rbFunction_C.Text = BaseHelper.GetShowText("01_04070000_003");
        rbFunction_A.Text = BaseHelper.GetShowText("01_04070000_004");

        rbType_Redeem.Text = BaseHelper.GetShowText("01_04070000_007");
        rbType_Award.Text = BaseHelper.GetShowText("01_04070000_008");

        //* 設置每頁顯示記錄最大筆數
        gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        gvList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
    }

    private void setrbTypeEnable(bool bState)
    {
        if (!bState)
        {
            rbType_Redeem.Checked = !bState;
            rbType_Award.Checked = bState;
            txtCode.MaxLength = 5;
            txtMEMO.MaxLength = 50;
        }
        else
        {
            txtCode.MaxLength = 3;
            txtMEMO.MaxLength = 30;
        }

        rbType_Award.Enabled = bState;
        rbType_Redeem.Enabled = bState;

        txtCode.Text = "";
        txtMEMO.Text = "";
    }

    private void fillDateView(string strCode)
    {
        string strType = "P";
        int iTotalCount = 0;

        if (rbFunction_A.Checked)
        {
            strType = "A";
        }
        else if (rbFunction_C.Checked)
        {
            strType = "C";
        }

        switch (strType.ToUpper())
        {
            case "C":
                if (rbType_Redeem.Checked)
                {

                    gvList.DataSource = BRCardTypeList_Redeem.Select(gpList.CurrentPageIndex, gpList.PageSize, strCode, ref iTotalCount);

                }
                else
                {
                    gvList.DataSource = BRCardTypeList_Award.Select(gpList.CurrentPageIndex, gpList.PageSize, strCode, ref iTotalCount);
                }
                break;
            case "A":
                if (rbType_Redeem.Checked)
                {
                    gvList.DataSource = BRACCUMTYPEList_Redeem.Select(gpList.CurrentPageIndex, gpList.PageSize, strCode, ref iTotalCount);
                }
                else
                {
                    gvList.DataSource = BRACCUMTYPEList_Award.Select(gpList.CurrentPageIndex, gpList.PageSize, strCode, ref iTotalCount);
                }
                break;
            default:
                if ("" == strCode)
                {
                    gvList.DataSource = BRProgramList.Search("", gpList.CurrentPageIndex, gpList.PageSize);
                }
                else
                {
                    gvList.DataSource = BRProgramList.Select(strCode);
                }
                iTotalCount = ((EntitySet<EntityProgramList>)gvList.DataSource).TotalCount;
                break;
        }

        gpList.RecordCount = iTotalCount;
        gvList.DataBind();
    }

    private void ButControl(string strCType, string strCode)
    {
        string strType = "P";
        string strMsg = "";

        if (rbFunction_A.Checked)
        {
            strType = "A";
        }
        else if (rbFunction_C.Checked)
        {
            strType = "C";
        }

        switch (strType.ToUpper())
        {
            case "C":
                if (rbType_Redeem.Checked)
                {
                    if ("A" == strCType.ToUpper()) { BRCardTypeList_Redeem.Add(txtCode.Text, txtMEMO.Text, ref strMsg); }
                    if ("D" == strCType.ToUpper()) { BRCardTypeList_Redeem.Remove(strCode); }
                }
                else
                {
                    if ("A" == strCType.ToUpper()) { BRCardTypeList_Award.Add(txtCode.Text, txtMEMO.Text, ref strMsg); }
                    if ("D" == strCType.ToUpper()) { BRCardTypeList_Award.Remove(strCode); }
                }
                break;
            case "A":
                if (rbType_Redeem.Checked)
                {
                    if ("A" == strCType.ToUpper()) { BRACCUMTYPEList_Redeem.Add(txtCode.Text, txtMEMO.Text, ref strMsg); }
                    if ("D" == strCType.ToUpper()) { BRACCUMTYPEList_Redeem.Remove(strCode); }
                }
                else
                {
                    if ("A" == strCType.ToUpper()) { BRACCUMTYPEList_Award.Add(txtCode.Text, txtMEMO.Text, ref strMsg); }
                    if ("D" == strCType.ToUpper()) { BRACCUMTYPEList_Award.Remove(strCode); }
                }
                break;
            default:
                if ("A" == strCType.ToUpper()) { BRProgramList.Add(txtCode.Text, txtMEMO.Text, ref strMsg); }
                if ("D" == strCType.ToUpper()) { BRProgramList.Remove(strCode); }
                break;
        }

        if ("" == strMsg.Trim())
        {
            fillDateView("");
        }
        else
        {
            base.strAlertMsg = MessageHelper.GetMessage(strMsg);
        }

    }
    #endregion








}
