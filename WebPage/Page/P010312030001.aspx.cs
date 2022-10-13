//******************************************************************
//*  作    者：Ares_Jack
//*  功能說明：EDDA案件資料查詢
//*  創建日期：2022/10/06
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Data;
using System.Web.UI.WebControls;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Message;
using Framework.WebControls;
using Framework.Common.Logging;
using Framework.Common.Utility;
using System.Data.SqlClient;
using CSIPCommonModel.BusinessRules;
using CSIPNewInvoice.EntityLayer_new;

public partial class P010312030001 : PageBase
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
            // 拋檔日期
            this.txtBatchDateStart.Text = DateTime.Now.ToString("yyyyMMdd");
            this.txtBatchDateEnd.Text = DateTime.Now.ToString("yyyyMMdd");
            this.txtBatchDateStart.Focus();
            this.gpList.RecordCount = 0;
            this.gpList.Visible = false;
            this.gvpbR02Record.DataSource = null;
            this.gvpbR02Record.DataBind();
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
        // 匯出到Excel
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
            if (!checkInputDate(txtBatchDateStart.Text, txtBatchDateEnd.Text))
            {
                base.strAlertMsg = MessageHelper.GetMessage("01_03120100_012");
                base.strClientMsg = MessageHelper.GetMessage("01_03120100_012");
                return;
            }

            string msgID = "";
            string serverPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
            string agentName = ((EntityAGENT_INFO)this.Session["Agent"]).agent_name;
            string Cus_ID = this.txtCus_ID.Text.Trim(); //客戶ID
            string BatchDateStart = this.txtBatchDateStart.Text.Trim().Replace("/", ""); //批次起日
            string BatchDateEnd = this.txtBatchDateEnd.Text.Trim().Replace("/", ""); //批次迄日
            string ComparisonStatus = string.Empty; //資料比對狀態
            if (radComparisonStatus_0.Checked)
                ComparisonStatus = "0";
            if (radComparisonStatus_1.Checked)
                ComparisonStatus = "1";
            if (radComparisonStatus_2.Checked)
                ComparisonStatus = "2";
            if (radComparisonStatus_3.Checked)
                ComparisonStatus = "3";

            // 服務器端，生成Excel文檔
            if (!CSIPKeyInGUI.BusinessRules_new.BRExcel_File.CreateExcelFile_EDDACase(Cus_ID, ComparisonStatus, BatchDateStart, BatchDateEnd, agentName, ref serverPathFile, ref msgID))
            {
                if (msgID != "")
                    base.strClientMsg += MessageHelper.GetMessage(msgID);
                else
                    base.strClientMsg += MessageHelper.GetMessage("01_03120300_004");
                return;
            }

            // 將服務器端生成的文檔，下載到本地。
            string nowDate = DateTime.Now.ToString("yyyyMMdd");
            string fileName = BaseHelper.GetShowText("01_03120300_001") + nowDate + ".xls";


            // 顯示提示訊息：匯出到Excel文檔資料成功
            this.Session["ServerFile"] = serverPathFile;
            this.Session["ClientFile"] = fileName;
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("01_03120300_005") + "' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);
        }
        catch (Exception ex)
        {
            Logging.Log(ex.ToString(), LogState.Error, LogLayer.UI);
            base.strClientMsg += BaseHelper.ClientMsgShow("01_03120300_004");
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

    #endregion event

    #region function

    /// <summary>
    /// 從Show.xml取漢字，設置畫面控件的Text
    /// </summary>
    private void ShowControlsText()
    {
        this.gvpbR02Record.Columns[0].HeaderText = BaseHelper.GetShowText("01_03120300_008");// 批次日期
        this.gvpbR02Record.Columns[1].HeaderText = BaseHelper.GetShowText("01_03120300_009");// 資料比對狀態
        this.gvpbR02Record.Columns[2].HeaderText = BaseHelper.GetShowText("01_03120300_010");// 上傳主機時間
        this.gvpbR02Record.Columns[3].HeaderText = BaseHelper.GetShowText("01_03120300_011");// 交易序號(ACH)
        this.gvpbR02Record.Columns[4].HeaderText = BaseHelper.GetShowText("01_03120300_012");// 交易代號(EDDA)
        this.gvpbR02Record.Columns[5].HeaderText = BaseHelper.GetShowText("01_03120300_013");// 授權編號
        this.gvpbR02Record.Columns[6].HeaderText = BaseHelper.GetShowText("01_03120300_014");// 用戶號碼
        this.gvpbR02Record.Columns[7].HeaderText = BaseHelper.GetShowText("01_03120300_015");// 新增或取消
        this.gvpbR02Record.Columns[8].HeaderText = BaseHelper.GetShowText("01_03120300_016");// 申請之交易日期
        this.gvpbR02Record.Columns[9].HeaderText = BaseHelper.GetShowText("01_03120300_017");// 他行行庫代碼
        this.gvpbR02Record.Columns[10].HeaderText = BaseHelper.GetShowText("01_03120300_018");// 他行銀行帳號
        this.gvpbR02Record.Columns[11].HeaderText = BaseHelper.GetShowText("01_03120300_019");// 繳款方式
        this.gvpbR02Record.Columns[12].HeaderText = BaseHelper.GetShowText("01_03120300_020");// 回復訊息
        this.gvpbR02Record.Columns[13].HeaderText = BaseHelper.GetShowText("01_03120300_021");// 自扣者ID
        this.gvpbR02Record.Columns[14].HeaderText = BaseHelper.GetShowText("01_03120300_022");// 推廣通路代碼
        this.gvpbR02Record.Columns[15].HeaderText = BaseHelper.GetShowText("01_03120300_023");// 推廣單位代碼
        this.gvpbR02Record.Columns[16].HeaderText = BaseHelper.GetShowText("01_03120300_024");// 推廣員員編

        // 設置每頁顯示記錄最大條數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gvpbR02Record.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
    }

    /// <summary>
    /// 綁定GridView數據源
    /// </summary>
    /// <param name="dirValues"></param>
    private void BindGridView()
    {
        try
        {
            //起訖日範圍超過30天
            if (!checkInputDate(txtBatchDateStart.Text, txtBatchDateEnd.Text))
            {
                base.strAlertMsg = MessageHelper.GetMessage("01_03120100_012");
                base.strClientMsg = MessageHelper.GetMessage("01_03120100_012");
                return;
            }

            int intTotolCount = 0;
            string Cus_ID = this.txtCus_ID.Text.Trim(); //客戶ID
            string BatchDateStart = this.txtBatchDateStart.Text.Trim().Replace("/", ""); //批次起日
            string BatchDateEnd = this.txtBatchDateEnd.Text.Trim().Replace("/", ""); //批次迄日
            string ComparisonStatus = string.Empty; //資料比對狀態
            if (radComparisonStatus_0.Checked)
                ComparisonStatus = "0";
            if (radComparisonStatus_1.Checked)
                ComparisonStatus = "1";
            if (radComparisonStatus_2.Checked)
                ComparisonStatus = "2";
            if (radComparisonStatus_3.Checked)
                ComparisonStatus = "3";
            if (radComparisonStatus_4.Checked)
                ComparisonStatus = string.Empty; ;

            string sqlText = @"SELECT BatchDate,
                                    CASE
	                                    WHEN ComparisonStatus IN('0') THEN '待比對' 
	                                    WHEN ComparisonStatus IN('1') THEN '正常' 
	                                    WHEN ComparisonStatus IN('2') THEN '缺少網銀資料' 
	                                    WHEN ComparisonStatus IN('3') THEN '網銀異常資料' 
	                                    ELSE '' 
	                                END AS ComparisonStatus,
	                                    UploadTime, Deal_S_No, Deal_No, AuthCode, Cus_ID, Apply_Type, ApplyDate, AccNoBank, AccNo, 
                                    CASE
	                                    WHEN PayWay IN('0') THEN '繳全額' 
	                                    WHEN PayWay IN('1') THEN '繳最低額' 
	                                    ELSE '' 
	                                END AS PayWay, 
                                        Reply_Info, AccID, SalesChannel, SalesUnit, SalesEmpoNo 
                                    FROM 
                                        EDDA_Auto_Pay 
                                    WHERE (BatchDate BETWEEN @StartDate AND @EndDate) ";
            string sqlOrderBy = "ORDER BY BatchDate";
            string sqlWhere = string.Empty;
            if (!string.IsNullOrEmpty(ComparisonStatus))
                sqlWhere += @"AND ComparisonStatus = @ComparisonStatus ";
            if (!string.IsNullOrEmpty(Cus_ID))
                sqlWhere += @"AND Cus_ID = @Cus_ID ";

            SqlCommand sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlText + sqlWhere + sqlOrderBy };
            sqlComm.Parameters.Add(new SqlParameter("@Cus_ID", txtCus_ID.Text)); //客戶ID
            sqlComm.Parameters.Add(new SqlParameter("@ComparisonStatus", ComparisonStatus)); //資料比對狀態
            sqlComm.Parameters.Add(new SqlParameter("@StartDate", txtBatchDateStart.Text)); //收檔日期
            sqlComm.Parameters.Add(new SqlParameter("@EndDate", txtBatchDateEnd.Text)); //收檔日期
            DataSet dstSearchResult = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref intTotolCount, "Connection_System");

            if (dstSearchResult == null)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_03120300_002");
                this.gpList.RecordCount = 0;
                this.gvpbR02Record.DataSource = null;
                this.gvpbR02Record.DataBind();
            }
            else
            {
                this.gpList.RecordCount = intTotolCount;
                this.gvpbR02Record.DataSource = dstSearchResult.Tables[0];
                this.gvpbR02Record.DataBind();

                if (intTotolCount == 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_03120300_001");
                }
                else
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_03120300_003");
                }
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp.ToString(), LogState.Error, LogLayer.UI) ;
            base.strClientMsg += MessageHelper.GetMessage("01_03120300_002");
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
        catch (Exception ex)
        {
            Logging.Log(ex.ToString(), LogState.Error, LogLayer.UI);
            return false;
        }
    }

    #endregion function
}
