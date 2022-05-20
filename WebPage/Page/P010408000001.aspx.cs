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
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using Framework.Data.OM.Collections;
using Framework.Data.OM;
using Framework.WebControls;
using System.Collections.Generic;
using Framework.Common.Message;
using Framework.Common.Utility;

public partial class P010408000001 : PageBase
{
    #region event
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            setShowID();
            fillGridView("");
            CPSet.Visible = false;
        }

    }

    protected void rbType_Redeem_CheckedChanged(object sender, EventArgs e)
    {
        fillGridView("");
        CPSet.Visible = false;
        ViewState["dicACCUMGT"] = null;
    }

    protected void rbType_Award_CheckedChanged(object sender, EventArgs e)
    {
        fillGridView("");
        CPSet.Visible = false;
        ViewState["dicACCUMGT"] = null;
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        fillGridView(txtACODE.Text.Trim());
        CPSet.Visible = false;
    }

    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        fillGridView("");
    }

    protected void gpSet_PageChanged(object src, PageChangedEventArgs e)
    {
        SaveCheck();
        gpSet.CurrentPageIndex = e.NewPageIndex;
        fillGridView_Set("");
        if (rbType_Redeem.Checked)
        {
            CheckBoxSet_R(null);
        }
        else
        {
            CheckBoxSet_A(null);
        }

    }

    protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[0].Text = BaseHelper.GetShowText("01_04080000_008");
            e.Row.Cells[1].Text = BaseHelper.GetShowText("01_04080000_009");
        }
    }

    protected void gvSet_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[1].Text = BaseHelper.GetShowText("01_04080000_008");
            e.Row.Cells[2].Text = BaseHelper.GetShowText("01_04080000_009");
        }
    }

    protected void gvList_RowEditing(object sender, GridViewEditEventArgs e)
    {
        string strACode = ((CustGridView)sender).Rows[e.NewEditIndex].Cells[0].Text.Trim();

        fillGridView(strACode);

        CPSet.Visible = true;
        fillGridView_Set(strACode);
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (null == ViewState["dicACCUMGT"])
        {
            return;
        }

        SaveCheck();

        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic = (Dictionary<string, string>)ViewState["dicACCUMGT"];

        string strACode = gvList.Rows[0].Cells[0].Text.Trim();

        if (rbType_Redeem.Checked)
        {
            if (BRACCUMGroupTable_Redeem.Insert(CreateEntitySet_R(dic,strACode),strACode))
            {
                strAlertMsg = MessageHelper.GetMessage("01_04080000_001");
            }
            else
            {
                strAlertMsg = MessageHelper.GetMessage("01_04080000_002");
            }
            
        }
        else
        {
            if (BRACCUMGroupTable_Award.Insert(CreateEntitySet_A(dic,strACode),strACode))
            {
                strAlertMsg = MessageHelper.GetMessage("01_04080000_001");
            }
            else
            {
                strAlertMsg = MessageHelper.GetMessage("01_04080000_002");
            }
        }

        CPSet.Visible = false;
        fillGridView("");
    }
    #endregion

    #region method
    private void setShowID()
    {
        rbType_Redeem.Text = BaseHelper.GetShowText("01_04080000_006");
        rbType_Award.Text = BaseHelper.GetShowText("01_04080000_007");

        //* 設置每頁顯示記錄最大筆數
        gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        gvList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());

        gpSet.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        gvSet.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
    }

    private void fillGridView(string strACCUCode)
    {
        int iTotalCount = 0;

        if (rbType_Redeem.Checked)
        {
            gvList.DataSource = BRACCUMTYPEList_Redeem.Select(gpList.CurrentPageIndex, gpList.PageSize, strACCUCode, ref iTotalCount);
        }
        else
        {
            gvList.DataSource = BRACCUMTYPEList_Award.Select(gpList.CurrentPageIndex, gpList.PageSize, strACCUCode, ref iTotalCount);
        }

        gpList.RecordCount = iTotalCount;
        gvList.DataBind();
    }

    private void fillGridView_Set(string strACCUCode)
    {
        int iTotalCount = 0;

        if (rbType_Redeem.Checked)
        {
            gvSet.DataSource = BRCardTypeList_Redeem.Select(gpSet.CurrentPageIndex, gpSet.PageSize, "", ref iTotalCount);
        }
        else
        {
            gvSet.DataSource = BRCardTypeList_Award.Select(gpSet.CurrentPageIndex, gpSet.PageSize, "", ref iTotalCount);
        }

        gpSet.RecordCount = iTotalCount;
        gvSet.DataBind();

        if ("" != strACCUCode.Trim())
        {
            if (rbType_Redeem.Checked)
            {
                CheckBoxSet_R(BRACCUMGroupTable_Redeem.Select(strACCUCode));
            }
            else
            {
                CheckBoxSet_A(BRACCUMGroupTable_Award.Select(strACCUCode));
            }
        }

    }

    private void CheckBoxSet_R(EntitySet<EntityACCUMGroupTable_Redeem> esACCUMGT_R)
    {
        if (!CPSet.Visible)
        {
            return;
        }

        Dictionary<string, string> dicR = new Dictionary<string, string>();

        if (null == esACCUMGT_R)
        {
            dicR = (Dictionary<string, string>)ViewState["dicACCUMGT"];
        }
        else
        {
            for (int j = 0; j < esACCUMGT_R.Count; j++)
            {
                dicR.Add(esACCUMGT_R.GetEntity(j).Card_CODE.Trim(), esACCUMGT_R.GetEntity(j).Card_CODE.Trim());
            }
        }


        for (int i = 0; i < gvSet.Rows.Count; i++)
        {
            for (int j = 0; j < dicR.Count; j++)
            {
                if (dicR.ContainsKey(gvSet.Rows[i].Cells[1].Text.Trim()))
                {
                    ((CustCheckBox)gvSet.Rows[i].Cells[0].Controls[1]).Checked = true;
                    break;
                }
            }
        }

        ViewState["dicACCUMGT"] = dicR;

    }

    private void CheckBoxSet_A(EntitySet<EntityACCUMGroupTable_Award> esACCUMGT_A)
    {
        if (!CPSet.Visible)
        {
            return;
        }

        Dictionary<string, string> dicA = new Dictionary<string, string>();

        if (null == esACCUMGT_A)
        {
            dicA = (Dictionary<string, string>)ViewState["dicACCUMGT"];
        }
        else
        {
            for (int j = 0; j < esACCUMGT_A.Count; j++)
            {
                dicA.Add(esACCUMGT_A.GetEntity(j).Card_CODE.Trim(), esACCUMGT_A.GetEntity(j).Card_CODE.Trim());
            }
        }


        for (int i = 0; i < gvSet.Rows.Count; i++)
        {
            for (int j = 0; j < dicA.Count; j++)
            {
                if (dicA.ContainsKey(gvSet.Rows[i].Cells[1].Text.Trim()))
                {
                    ((CustCheckBox)gvSet.Rows[i].Cells[0].Controls[1]).Checked = true;
                    break;
                }
            }
        }

        ViewState["dicACCUMGT"] = dicA;

    }

    private void SaveCheck()
    {
        if (null == ViewState["dicACCUMGT"])
        {
            return;
        }

        string strACode = gvList.Rows[0].Cells[0].Text.Trim();

        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic = (Dictionary<string, string>)ViewState["dicACCUMGT"];

        for (int i = 0; i < gvSet.Rows.Count; i++)
        {
            if (((CustCheckBox)gvSet.Rows[i].Cells[0].Controls[1]).Checked)
            {
                if (!dic.ContainsKey(gvSet.Rows[i].Cells[1].Text.Trim()))
                {
                    dic.Add(gvSet.Rows[i].Cells[1].Text.Trim(), gvSet.Rows[i].Cells[1].Text.Trim());
                }
            }
            else
            {
                if (dic.ContainsKey(gvSet.Rows[i].Cells[1].Text.Trim()))
                {
                    dic.Remove(gvSet.Rows[i].Cells[1].Text.Trim());
                }
            }
        }

    }

    private EntitySet<EntityACCUMGroupTable_Redeem> CreateEntitySet_R(Dictionary<string, string> dic, string strACCUCode)
    {
        EntitySet<EntityACCUMGroupTable_Redeem> esACCUMGT_R = new EntitySet<EntityACCUMGroupTable_Redeem>();

        foreach (KeyValuePair<string,string> kvp in dic)
        {
            EntityACCUMGroupTable_Redeem eACCUMGT_R = new EntityACCUMGroupTable_Redeem();
            eACCUMGT_R.ACCU_CODE = strACCUCode.Trim();
            eACCUMGT_R.Card_CODE = kvp.Key.Trim();

            esACCUMGT_R.Add(eACCUMGT_R);
        }

        return esACCUMGT_R;
    }

    private EntitySet<EntityACCUMGroupTable_Award> CreateEntitySet_A(Dictionary<string, string> dic, string strACCUCode)
    {
        EntitySet<EntityACCUMGroupTable_Award> esACCUMGT_A = new EntitySet<EntityACCUMGroupTable_Award>();

        foreach (KeyValuePair<string, string> kvp in dic)
        {
            EntityACCUMGroupTable_Award eACCUMGT_A = new EntityACCUMGroupTable_Award();
            eACCUMGT_A.ACCU_CODE = strACCUCode.Trim();
            eACCUMGT_A.Card_CODE = kvp.Key.Trim();

            esACCUMGT_A.Add(eACCUMGT_A);
        }

        return esACCUMGT_A;
    }
    #endregion








}
