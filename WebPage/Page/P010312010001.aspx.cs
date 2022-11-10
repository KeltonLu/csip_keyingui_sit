//******************************************************************
//*  作    者：Ares_Jack
//*  功能說明：EDDA 授權扣款資料清單
//*  創建日期：2022/10/05
//*  修改紀錄：
//*  <author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Data;
using System.Web.UI.WebControls;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Message;
using Framework.WebControls;
using Framework.Common.Logging;
using Framework.Common.Utility;
using CSIPCommonModel.BusinessRules;
using System.Data.SqlClient;
using CSIPNewInvoice.EntityLayer_new;

public partial class P010312010001 : PageBase
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
            //收檔日期
            this.txtDownloadFileDateStart.Text = DateTime.Now.ToString("yyyyMMdd");
            this.txtDownloadFileDateEnd.Text = DateTime.Now.ToString("yyyyMMdd");
            this.txtDownloadFileDateStart.Focus();
            this.gpList.RecordCount = 0;
            this.gpList.Visible = false;
            this.gvpbEDDARecord.DataSource = null;
            this.gvpbEDDARecord.DataBind();
        }

        base.strHostMsg += "";
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.gpList.Visible = true;
        this.gpList.CurrentPageIndex = 0;
        BindGridView();
    }

    /// <summary>
    /// 列印
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        // 匯出到Excel.
        OutputExcel();
    }


    /// <summary>
    /// 將查詢結果匯出到Excel文檔
    /// </summary>
    protected void OutputExcel()
    {
        try
        {
            //起訖日範圍超過30天
            if (!checkInputDate(txtDownloadFileDateStart.Text, txtDownloadFileDateEnd.Text))
            {
                base.strAlertMsg = MessageHelper.GetMessage("01_03120100_012");
                base.strClientMsg = MessageHelper.GetMessage("01_03120100_012");
                return;
            }
            string msgID = "";
            string serverPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
            string agentName = ((EntityAGENT_INFO)this.Session["Agent"]).agent_name;
            string sendFileDateSatrt = this.txtDownloadFileDateStart.Text.Trim().Replace("/", "");
            string sendFileDateEnd = this.txtDownloadFileDateEnd.Text.Trim().Replace("/", "");
            // 服務器端，生成Excel文檔
            if (!CSIPKeyInGUI.BusinessRules_new.BRExcel_File.CreateExcelFile_EDDA(sendFileDateSatrt, sendFileDateEnd, agentName, ref serverPathFile, ref msgID))
            {
                if (msgID != "")
                    base.strClientMsg += MessageHelper.GetMessage(msgID);
                else
                    base.strClientMsg += MessageHelper.GetMessage("01_03120100_004");
                return;
            }

            // 將服務器端生成的文檔，下載到本地。
            string nowDate = DateTime.Now.ToString("yyyyMMdd");
            string fileName = BaseHelper.GetShowText("01_03120100_001") + nowDate + ".xls";

            // 顯示提示訊息：匯出到Excel文檔資料成功
            this.Session["ServerFile"] = serverPathFile;
            this.Session["ClientFile"] = fileName;
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("01_03120100_005") + "' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);
        }
        catch (Exception ex)
        {
            Logging.Log(ex.ToString(), LogState.Error, LogLayer.UI);
            base.strClientMsg += MessageHelper.GetMessage("01_03120100_004");
        }
    }

    /// <summary>
    /// 頁導航
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }

    /// <summary>
    /// CustGridView行綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbEDDARecord_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            // 序號 = 當前頁號 * 每頁行數 + 頁內順序號 + 1
            e.Row.Cells[0].Text = Convert.ToString((this.gpList.CurrentPageIndex - 1) * this.gpList.PageSize + e.Row.RowIndex + 1);
        }
    }

    #endregion event

    #region function

    /// <summary>
    /// 從Show.xml取漢字，設置畫面控件的Text
    /// </summary>
    private void ShowControlsText()
    {
        this.gvpbEDDARecord.Columns[0].HeaderText = BaseHelper.GetShowText("01_03120100_004");// 序號
        this.gvpbEDDARecord.Columns[1].HeaderText = BaseHelper.GetShowText("01_03120100_005");// 授權序號
        this.gvpbEDDARecord.Columns[2].HeaderText = BaseHelper.GetShowText("01_03120100_006");// 收受行(核印行)
        this.gvpbEDDARecord.Columns[3].HeaderText = BaseHelper.GetShowText("01_03120100_007");// 委繳戶統編\身分證字號
        this.gvpbEDDARecord.Columns[4].HeaderText = BaseHelper.GetShowText("01_03120100_008");// 委繳戶帳號
        this.gvpbEDDARecord.Columns[5].HeaderText = BaseHelper.GetShowText("01_03120100_009");// 申請類別
        this.gvpbEDDARecord.Columns[6].HeaderText = BaseHelper.GetShowText("01_03120100_010");// 處理註記
        this.gvpbEDDARecord.Columns[7].HeaderText = BaseHelper.GetShowText("01_03120100_011");// 收檔日期
        this.gvpbEDDARecord.Columns[8].HeaderText = BaseHelper.GetShowText("01_03120100_012");// EDDA回覆碼
        this.gvpbEDDARecord.Columns[9].HeaderText = BaseHelper.GetShowText("01_03120100_013");// 扣繳方式
        this.gvpbEDDARecord.Columns[10].HeaderText = BaseHelper.GetShowText("01_03120100_014");// 申請通路
        this.gvpbEDDARecord.Columns[11].HeaderText = BaseHelper.GetShowText("01_03120100_015");// 申請時間

        // 設置每頁顯示記錄最大條數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gvpbEDDARecord.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
    }

    /// <summary>
    /// 綁定GridView數據源
    /// </summary>
    private void BindGridView()
    {
        try
        {
            //起訖日範圍超過30天
            if (!checkInputDate(txtDownloadFileDateStart.Text, txtDownloadFileDateEnd.Text))
            {
                base.strAlertMsg = MessageHelper.GetMessage("01_03120100_012");
                base.strClientMsg = MessageHelper.GetMessage("01_03120100_012");
                return;
            }

            int intTotolCount = 0;
            string sqlText = @"
            SELECT AuthCode,
                   Other_Bank_Code_L,
                   Cus_ID,
                   Other_Bank_Acc_No,
                   Apply_Type,
                   CASE
                       WHEN UploadFlag = '0' THEN N'待上傳'
                       WHEN UploadFlag = '1' THEN N'已上傳'
                       WHEN UploadFlag = '2' THEN N'其他核印失敗集作人工處理'
                       ELSE ''
                       END AS UploadFlag,
                   Reply_Info,
                   CASE
                       WHEN PayWay = '0' THEN N'繳全額'
                       WHEN PayWay = '1' THEN N'繳最低額'
                       ELSE ''
                       END AS PayWay,
                   SalesChannel,
                   ApplyDate,
                   BatchDate
            FROM EDDA_Auto_Pay
            WHERE BatchDate BETWEEN @BatchDateStart AND @BatchDateEnd";
            
            SqlCommand sqlComm = new SqlCommand{ CommandType = CommandType.Text, CommandText = sqlText };
            sqlComm.Parameters.Add(new SqlParameter("@BatchDateStart", txtDownloadFileDateStart.Text)); //批次起日
            sqlComm.Parameters.Add(new SqlParameter("@BatchDateEnd", txtDownloadFileDateEnd.Text)); //批次迄日
            DataSet dstSearchResult = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref intTotolCount, "Connection_System");
            
            if (dstSearchResult == null)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_03120100_002");
                this.gpList.RecordCount = 0;
                this.gvpbEDDARecord.DataSource = null;
                this.gvpbEDDARecord.DataBind();
            }
            else
            {
                this.gpList.RecordCount = intTotolCount;
                this.gvpbEDDARecord.DataSource = dstSearchResult.Tables[0];
                this.gvpbEDDARecord.DataBind();

                if (intTotolCount == 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_03120100_001");
                }
                else
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_03120100_003");
                }
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp.ToString(), LogState.Error, LogLayer.UI);
            base.strClientMsg += MessageHelper.GetMessage("01_03120100_002");
        }
    }

    /// <summary>
    /// 檢核起訖日範圍
    /// </summary>
    /// <param name="BatchDateStart"></param>
    /// <param name="BatchDateEnd"></param>
    /// <returns></returns>
    private bool checkInputDate(string BatchDateStart, string BatchDateEnd)
    {
        try
        {
            BatchDateStart = BatchDateStart.Substring(0, 4) + "/" + BatchDateStart.Substring(4, 2) + "/" + BatchDateStart.Substring(6, 2);
            BatchDateEnd = BatchDateEnd.Substring(0, 4) + "/" + BatchDateEnd.Substring(4, 2) + "/" + BatchDateEnd.Substring(6, 2);

            DateTime dtBatchDateStart = Convert.ToDateTime(BatchDateStart);
            DateTime dtBatchDateEnd = Convert.ToDateTime(BatchDateEnd);

            TimeSpan ts = dtBatchDateEnd.Subtract(dtBatchDateStart); //兩時間天數相減
            double dayCount = ts.Days; //相距天數

            if (dayCount > 30)
                return false;
            else
                return true;
        }
        catch(Exception ex)
        {
            Logging.Log(ex.ToString(), LogState.Error, LogLayer.UI);
            return false;
        }
    }

    #endregion function
}
