//******************************************************************
//*  作    者：Ares_Jack
//*  功能說明：EDDA R12授權成功/失敗報表
//*  創建日期：2022/10/05
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Message;
using Framework.WebControls;
using Framework.Common.Logging;
using Framework.Common.Utility;
using CSIPCommonModel.BusinessRules;
using CSIPNewInvoice.EntityLayer_new;
using Framework.Common.JavaScript;

public partial class P010312020001 : PageBase
{
    #region event

    /// <summary>
    /// 畫面裝載時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 綁定GridView列頭訊息
            ShowControlsText();
            // 批次日期
            txtBatchDateStart.Text = DateTime.Now.ToString("yyyyMMdd");
            txtBatchDateEnd.Text = DateTime.Now.ToString("yyyyMMdd");
            txtBatchDateStart.Focus();
            gpList.RecordCount = 0;
            gpList.Visible = false;
            gvpbR02Record.DataSource = null;
            gvpbR02Record.DataBind();
            
            // 核印代碼下拉選單
            RtnCodeDropDownList.Items.Add(new ListItem { Text = @"全部", Value = string.Empty});
            DataTable dt = GetRtnCodeList();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var listItem = new ListItem
                {
                    Value = dt.Rows[i][0].ToString(),
                    Text = dt.Rows[i][1].ToString()
                };
                RtnCodeDropDownList.Items.Add(listItem);
            }
        }

        base.strHostMsg += "";
    }

    /// <summary>
    /// 取得EDDA核印代碼
    /// </summary>
    /// <returns></returns>
    private DataTable GetRtnCodeList()
    {
        DataTable rtnDataTable = new DataTable();
        try
        {
            var sql = @"SELECT EddaRtnCode, EddaRtnMsg FROM EDDA_Rtn_Info ORDER BY EddaRtnCode";
            SqlCommand sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sql };
            DataSet ds = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, "Connection_System");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                rtnDataTable = ds.Tables[0];
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            jsBuilder.RegScript(UpdatePanel1, BaseHelper.ClientMsgShow("00_00000000_000"));
        }
        
        return rtnDataTable;
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gpList.Visible = true;
        gpList.CurrentPageIndex = 0;
        BindGridView();
    }

    /// <summary>
    /// 列印
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        // 匯出到Excel
        OutputExcel();
    }

    /// <summary>
    /// 將查詢結果匯出到Excel文檔
    /// </summary>
    private void OutputExcel()
    {
        try
        {
            //起訖日範圍超過30天
            if (!CheckInputDate(txtBatchDateStart.Text, txtBatchDateEnd.Text))
            {
                base.strAlertMsg = MessageHelper.GetMessage("01_03120100_012");
                base.strClientMsg = MessageHelper.GetMessage("01_03120100_012");
                return;
            }

            string msgID = "";
            string serverPathFile = Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
            string agentName = ((EntityAGENT_INFO)Session["Agent"]).agent_name;
            string BatchDateStart = txtBatchDateStart.Text.Trim().Replace("/", "");
            string BatchDateEnd = txtBatchDateEnd.Text.Trim().Replace("/", "");
            string rtnCode = RtnCodeDropDownList.SelectedValue;
            string reportType = string.Empty;
            bool successChecked = radlSearchType.Items[0].Selected;
            bool failChecked = radlSearchType.Items[1].Selected;
            // S.成功 F.失敗 A.成功+失敗
            if (radlSearchType.Items[0].Selected)
            {
                reportType = radlSearchType.Items[1].Selected ? "A" : "S";
            }
            else
            {
                reportType = radlSearchType.Items[1].Selected ? "F" : "A";
            }

            // 服務器端，生成Excel文檔
            if (!CSIPKeyInGUI.BusinessRules_new.BRExcel_File.CreateExcelFile_EDDAR12(BatchDateStart, BatchDateEnd, reportType, rtnCode, agentName, ref serverPathFile, ref msgID, successChecked, failChecked))
            {
                if (msgID != "")
                    base.strClientMsg += MessageHelper.GetMessage(msgID);
                else
                    base.strClientMsg += MessageHelper.GetMessage("01_03120200_004");
                return;
            }

            // 將服務器端生成的文檔，下載到本地。
            string nowDate = DateTime.Now.ToString("yyyyMMdd");
            string fileName = "";
            if (radlSearchType.Items[0].Selected && radlSearchType.Items[1].Selected ||
               !radlSearchType.Items[0].Selected && !radlSearchType.Items[1].Selected)
            {
                fileName = BaseHelper.GetShowText("01_03120200_018") + nowDate + ".xls";
            }
            else if (radlSearchType.Items[0].Selected)
            {
                fileName = BaseHelper.GetShowText("01_03120200_019") + nowDate + ".xls";
            }
            else if (radlSearchType.Items[1].Selected)
            {
                fileName = BaseHelper.GetShowText("01_03120200_020") + nowDate + ".xls";
            }

            // 顯示提示訊息：匯出到Excel文檔資料成功
            Session["ServerFile"] = serverPathFile;
            Session["ClientFile"] = fileName;
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("01_03120200_005") + "' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);
        }
        catch (Exception ex)
        {
            Logging.Log(ex.ToString(), LogState.Error, LogLayer.UI);
            base.strClientMsg += BaseHelper.ClientMsgShow("01_03120200_004");
        }
    }

    /// <summary>
    /// 頁導航
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }

    /// <summary>
    /// CustGridView行綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbR02Record_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            // 序號 = 當前頁號 * 每頁行數 + 頁內順序號 + 1
            e.Row.Cells[0].Text = Convert.ToString((gpList.CurrentPageIndex-1) * gpList.PageSize + e.Row.RowIndex + 1);
        }
    }
    #endregion event

    #region function

    /// <summary>
    /// 從Show.xml取漢字，設置畫面控件的Text
    /// </summary>
    private void ShowControlsText()
    {
        gvpbR02Record.Columns[0].HeaderText = BaseHelper.GetShowText("01_03120200_008");// 序號
        gvpbR02Record.Columns[1].HeaderText = BaseHelper.GetShowText("01_03120200_006");// 批次日期
        gvpbR02Record.Columns[2].HeaderText = BaseHelper.GetShowText("01_03120200_009");// 收件編號
        gvpbR02Record.Columns[3].HeaderText = BaseHelper.GetShowText("01_03120200_010");// 收受行(核印行)
        gvpbR02Record.Columns[4].HeaderText = BaseHelper.GetShowText("01_03120200_011");// 收受行名稱(核印行)
        gvpbR02Record.Columns[5].HeaderText = BaseHelper.GetShowText("01_03120200_012");// 委繳戶統編\身分證字號
        gvpbR02Record.Columns[6].HeaderText = BaseHelper.GetShowText("01_03120200_013");// 委繳戶帳號
        gvpbR02Record.Columns[7].HeaderText = BaseHelper.GetShowText("01_03120200_014");// 持卡人ID
        gvpbR02Record.Columns[8].HeaderText = BaseHelper.GetShowText("01_03120200_015");// 申請類別
        gvpbR02Record.Columns[9].HeaderText = BaseHelper.GetShowText("01_03120200_016");// 成功/失敗
        gvpbR02Record.Columns[10].HeaderText = BaseHelper.GetShowText("01_03120200_017");// 回覆訊息
        gvpbR02Record.Columns[11].HeaderText = BaseHelper.GetShowText("01_03120200_021");// 核印失敗是否上送主機

        radlSearchType.Items[0].Text = BaseHelper.GetShowText("01_03120200_003");// 成功
        radlSearchType.Items[1].Text = BaseHelper.GetShowText("01_03120200_004");// 失敗

        // 設置每頁顯示記錄最大條數
        gpList.PageSize = 10;
        gvpbR02Record.PageSize = 10;
    }

    /// <summary>
    /// 綁定GridView數據源
    /// </summary>
    private void BindGridView()
    {
        try
        {
            //起訖日範圍超過30天
            if (!CheckInputDate(txtBatchDateStart.Text, txtBatchDateEnd.Text))
            {
                base.strAlertMsg = MessageHelper.GetMessage("01_03120100_012");
                base.strClientMsg = MessageHelper.GetMessage("01_03120100_012");
                return;
            }
            int totalCount = 0;
            string rtnCode = RtnCodeDropDownList.SelectedValue;
            string reportType;
            // S.成功 F.失敗 A.成功+失敗
            // S.成功 F.失敗 A.成功+失敗
            if (radlSearchType.Items[0].Selected)
            {
                reportType = radlSearchType.Items[1].Selected ? "A" : "S";
            }
            else
            {
                reportType = radlSearchType.Items[1].Selected ? "F" : "A";
            }

            DataTable dt = CSIPKeyInGUI.BusinessRules_new.BRExcel_File.GetDataEddaAr12(txtBatchDateStart.Text, txtBatchDateEnd.Text, reportType, rtnCode, ref totalCount,
                gpList.CurrentPageIndex, gpList.PageSize);

            if (dt == null)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_03120200_002");
                gpList.RecordCount = 0;
                gvpbR02Record.DataSource = null;
                gvpbR02Record.DataBind();
            }
            else
            {
                gpList.RecordCount = totalCount;
                gvpbR02Record.DataSource = dt;
                gvpbR02Record.DataBind();

                if (totalCount == 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_03120200_001");
                }
                else
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_03120200_003");
                }
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp.ToString(), LogState.Error, LogLayer.UI) ;
            base.strClientMsg += MessageHelper.GetMessage("01_03120200_002");
        }
    }

    /// <summary>
    /// 檢核起訖日範圍
    /// </summary>
    /// <param name="batchDateStart"></param>
    /// <param name="batchDateEnd"></param>
    /// <returns></returns>
    private bool CheckInputDate(string batchDateStart, string batchDateEnd)
    {
        try
        {
            batchDateStart = batchDateStart.Substring(0, 4) + "/" + batchDateStart.Substring(4, 2) + "/" + batchDateStart.Substring(6, 2);
            batchDateEnd = batchDateEnd.Substring(0, 4) + "/" + batchDateEnd.Substring(4, 2) + "/" + batchDateEnd.Substring(6, 2);

            DateTime dtBatchDateStart = Convert.ToDateTime(batchDateStart);
            DateTime dtBatchDateEnd = Convert.ToDateTime(batchDateEnd);

            TimeSpan ts = dtBatchDateEnd.Subtract(dtBatchDateStart); //兩時間天數相減
            double dayCount = ts.Days; //相距天數

            return !(dayCount > 30);
        }
        catch (Exception ex)
        {
            Logging.Log(ex.ToString(), LogState.Error, LogLayer.UI);
            return false;
        }
    }

    #endregion function
}
