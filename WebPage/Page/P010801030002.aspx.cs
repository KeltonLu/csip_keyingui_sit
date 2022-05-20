// *****************************************************************
//   作    者：Ray
//   功能說明：NameCheck 明細資料
//   創建日期：2020/04/24-RQ-2019-030155-005
//   修改記錄：
// <author>            <time>            <TaskID>                <desc>
// ******************************************************************

using System;
using System.Data;
using CSIPKeyInGUI.BusinessRules_new;
using System.Web.UI.WebControls;
public partial class Page_P010801030002 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            showeditDate();
        }
    }

    private void showeditDate()
    {
        AML_SessionState sessionOBJ = (AML_SessionState)Session["P010801000001_SESSION"];

        DataTable Dt = new DataTable();
        BRPNAMECHECKLOG.getNameChecklog_Detail(sessionOBJ, ref Dt);
        grvNameCheckD.DataSource = Dt;
        grvNameCheckD.DataBind();
    }

    protected void grvNameCheckD_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grvNameCheckD.PageIndex = e.NewPageIndex;
        showeditDate();
    }

    protected void grvNameCheckD_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!e.Row.Cells[7].Text.Trim().Equals("") && !e.Row.Cells[7].Text.Trim().Equals("&nbsp;"))
            {
                e.Row.BackColor = System.Drawing.Color.FromArgb(255, 255, 230);//針對MatchedResult底色變色
            }
        }
    }

    /// <summary>
    /// 返回按鈕
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// 修改紀錄: 20210426_Ares_Luke-調整頁面轉向方法(避免造成瀏覽器雙開被剔除)-新增返回按鈕
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        //20210426_Ares_Luke-調整頁面轉向方法(避免造成瀏覽器雙開被剔除)
        //20210603_Ares_Rick-視窗跳轉調整 避免雙開
        string lastPage = Session["P010801030001_Last"].ToString();
        //取得上一頁
        if (lastPage != "")
        {
            Response.Redirect(lastPage, false);
        }
        else
        {
            Response.Redirect("P010801030001.aspx", false);
        }

    }


}

