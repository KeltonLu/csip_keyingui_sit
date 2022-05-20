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
using System.IO;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Message;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Utility;

public partial class P010212000001 : PageBase
{
    #region 變數區
    private EntityAGENT_INFO eAgentInfo;//*記錄登陸Session訊息
    private structPageInfo sPageInfo;//*記錄網頁訊息

    #endregion
    #region event
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ShowControlsText();
            this.dpDateStart.Text = DateTime.Today.ToString("yyyy/MM/dd");
            this.dpDateEnd.Text = DateTime.Today.ToString("yyyy/MM/dd");
            this.gpList.RecordCount = 0;
            this.gpList.Visible = false;
            this.gvList.DataSource = null;
            this.gvList.DataBind();
        }
        base.strHostMsg += "";
        base.strClientMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];//*記錄網頁訊息
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Account_Nbr = CustTextBox1.Text; //卡號

        //20200109-修改SOC存入條件
        //log.Statement_Text = string.Format("AC_NO:{0}", log.Account_Nbr); //查詢條件內容: 用 | 區隔,要看文件確認
        log.Statement_Text = string.Format("CUSTOMER_ID:{0}|AC_NO:{1}|BRANCH_ID:{2}|ROLE_ID:{3}", log.Customer_Id, log.Account_Nbr, log.Branch_Nbr, log.Role_Id); //查詢條件內容: 用 | 區隔

        BRL_AP_LOG.Add(log);
        //------------------------------------------------------
        this.gpList.CurrentPageIndex = 0;
        BindGridView();
        gpList.Visible = true;
    }

    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }

    protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells[7].Text != "" || e.Row.Cells[7].Text != null)
            {
                switch (e.Row.Cells[7].Text)
                {
                    case "Y":
                        e.Row.Cells[7].Text = BaseHelper.GetShowText("01_0212000001_017");
                        break;
                    case "E":
                        e.Row.Cells[7].Text = BaseHelper.GetShowText("01_0212000001_018");
                        break;
                    case "D":
                        e.Row.Cells[7].Text = "刪除";
                        break;
                    case "N":
                        e.Row.Cells[7].Text = "未處理";
                        break;
                }
            }
            e.Row.Cells[1].Text = DateTime.Parse(e.Row.Cells[1].Text).ToString("yyyy/MM/dd");
            e.Row.Cells[9].Text = DateTime.Parse(e.Row.Cells[9].Text).ToString("yyyy/MM/dd HH:mm:ss");
        }
    }
    #endregion event

    #region method
    private void ShowControlsText()
    {
        this.gvList.Columns[0].HeaderText = BaseHelper.GetShowText("01_0212000001_005");
        this.gvList.Columns[1].HeaderText = BaseHelper.GetShowText("01_0212000001_006");
        this.gvList.Columns[2].HeaderText = BaseHelper.GetShowText("01_0212000001_007");
        this.gvList.Columns[3].HeaderText = BaseHelper.GetShowText("01_0212000001_025");
        this.gvList.Columns[4].HeaderText = BaseHelper.GetShowText("01_0212000001_009");
        this.gvList.Columns[5].HeaderText = BaseHelper.GetShowText("01_0212000001_010");
        this.gvList.Columns[6].HeaderText = BaseHelper.GetShowText("01_0212000001_012");
        this.gvList.Columns[7].HeaderText = BaseHelper.GetShowText("01_0212000001_011");
        this.gvList.Columns[8].HeaderText = BaseHelper.GetShowText("01_0212000001_026");
        this.gvList.Columns[9].HeaderText = BaseHelper.GetShowText("01_0212000001_013");

        //* 設置每頁顯示記錄最大條數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gvList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
    }

    private void BindGridView()
    {
        int iTotalCount = 0;
        int scount = 0;
        int fcount = 0;

        string strDateStart = this.dpDateStart.Text.Replace("/", "");
        string strDateEnd = this.dpDateEnd.Text.Replace("/", "");

        DataTable dtBalance = (DataTable)BRBalance_Trans.SelectLOG(strDateStart.Trim(), strDateEnd.Trim(),
            CustTextBox1.Text.Trim(), CustTextBox2.Text.Trim(), CDDList1.SelectedValue.Trim(),
            this.cddl_Upload.SelectedValue.Trim(), gpList.CurrentPageIndex, gpList.PageSize, ref iTotalCount);

        if (dtBalance != null && dtBalance.Rows.Count > 0)
        {
            this.ctxt_total.Text = iTotalCount.ToString();
            gvList.DataSource = dtBalance;
            gpList.RecordCount = iTotalCount;
            gvList.DataBind();

            dtBalance = (DataTable)BRBalance_Trans.SelectLOG(strDateStart.Trim(), strDateEnd.Trim(),
                CustTextBox1.Text.Trim(), CustTextBox2.Text.Trim(), CDDList1.SelectedValue.Trim(),
                this.cddl_Upload.SelectedValue.Trim());

            if (dtBalance != null && dtBalance.Rows.Count > 0)
            {
                for (int i = 0; i < dtBalance.Rows.Count; i++)
                {
                    if (dtBalance.Rows[i]["Process_Flag"].ToString().Trim().ToUpper() == "Y")
                        scount++;

                    if (dtBalance.Rows[i]["Process_Flag"].ToString().Trim().ToUpper() == "E")
                        fcount++;
                }

                this.ctxt_success.Text = scount.ToString();
                this.ctxt_faile.Text = fcount.ToString();
            }
        }
        else
        {
            this.ctxt_total.Text = "0";
            this.ctxt_success.Text = "0";
            this.ctxt_faile.Text = "0";
            gvList.DataSource = null;
            gpList.RecordCount = 0;
            gvList.DataBind();
        }


    }
    #endregion method

    protected void btnClear_Click(object sender, EventArgs e)
    {
        CustTextBox1.Text = null;
        CustTextBox2.Text = null;
        dpDateStart.Text = null;
        dpDateEnd.Text = null;
        CDDList1.SelectedIndex = 0;
    }

    /// <summary>
    /// 修改日期: 2020/12/04_Ares_Stanley-修正陣列長度不足錯誤, 無範本NPOI報表產出;
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void cbtn_Excel_Click(object sender, EventArgs e)
    {
        string sDay = this.dpDateStart.Text.Replace("/", "");
        string eDay = this.dpDateEnd.Text.Replace("/", "");
        DataTable dt = (DataTable)BRBalance_Trans.SelectLOG(sDay, eDay, CustTextBox1.Text.Trim(),
            CustTextBox2.Text.Trim(), CDDList1.SelectedValue.Trim(), this.cddl_Upload.SelectedValue.Trim());

        if (dt.Rows.Count > 0)
        {
            // string guids = "";
            string[] guidArr = new string[] { };
            DataSet ds = new DataSet();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                System.Array.Resize(ref guidArr, guidArr.Length + 1);
                guidArr[i] = dt.Rows[i]["Newid"].ToString();
            }

            ds = (DataSet)BRBalance_Trans.BindNewidsForExcel(guidArr);

            string templateFileName = AppDomain.CurrentDomain.BaseDirectory +
                                      UtilHelper.GetAppSettings("ReportTemplate") + "Balance_Trans.xls";

            //if (System.IO.File.Exists(templateFileName))
            //{
            string path = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
            BRExcel_File.CheckDirectory(ref path);
            string filename = "/Balance_Trans_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls";
            BRAuto_pay_status bps = new BRAuto_pay_status();

            bps.Excel(ds, templateFileName, path, filename, this.dpDateStart.Text, this.dpDateEnd.Text,
                this.ctxt_total.Text, this.ctxt_success.Text, this.ctxt_faile.Text);

            #region 下載
            FileInfo file = new FileInfo(path + filename);
            this.Session["ServerFile"] = path + filename;
            this.Session["ClientFile"] = filename;
            // string urlString = @"ClientMsgShow('Balance_Trans');";
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: 'Balance_Trans' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);
            #endregion
            //}
        }
        else
        {
            // 您是否遺漏查詢，結果清單無資料可匯出
            MessageHelper.ShowMessage(this.UpdatePanel1, "01_02110001_005");
            return;
        }
    }

}
