//******************************************************************
//*  作    者：Ares_Jack
//*  功能說明：EDDA R12授權成功/失敗報表
//*  創建日期：2022/10/05
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
            this.txtBatchDateStart.Text = DateTime.Now.ToString("yyyyMMdd");
            this.txtBatchDateEnd.Text = DateTime.Now.ToString("yyyyMMdd");
            this.txtBatchDateStart.Focus();
            this.gpList.RecordCount = 0;
            this.gpList.Visible = false;
            this.gvpbR02Record.DataSource = null;
            this.gvpbR02Record.DataBind();

            DataTable dtblPropertyKey = new DataTable();
            BRM_PROPERTY_KEY.GetProperty("01", "EddaReplyInfo", ref dtblPropertyKey);
            this.dropPostRtnMsg.Items.Clear();
            ListItem listItem = new ListItem();
            listItem.Text = "";
            listItem.Value = "";
            this.dropPostRtnMsg.Items.Add(listItem);
            if (dtblPropertyKey != null && dtblPropertyKey.Rows.Count > 0)
            {
                for (int i = 0; i < dtblPropertyKey.Rows.Count; i++)
                {
                    listItem = new ListItem();
                    listItem.Text = dtblPropertyKey.Rows[i][1].ToString();
                    listItem.Value = dtblPropertyKey.Rows[i][0].ToString();
                    this.dropPostRtnMsg.Items.Add(listItem);
                }
            }
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
            string BatchDateStart = this.txtBatchDateStart.Text.Trim().Replace("/", "");
            string BatchDateEnd = this.txtBatchDateEnd.Text.Trim().Replace("/", "");
            string postRtnMsg = this.dropPostRtnMsg.SelectedValue;
            string reportType = string.Empty;
            bool successChecked = this.radlSearchType.Items[0].Selected;
            bool failChecked = this.radlSearchType.Items[1].Selected;
            // S.成功 F.失敗 A.成功+失敗
            if (this.radlSearchType.Items[0].Selected)
            {
                reportType = (this.radlSearchType.Items[1].Selected) ? "A" : "S";
            }
            else
            {
                reportType = (this.radlSearchType.Items[1].Selected) ? "F" : "A";
            }

            // 服務器端，生成Excel文檔
            if (!CSIPKeyInGUI.BusinessRules_new.BRExcel_File.CreateExcelFile_EDDAR12(BatchDateStart, BatchDateEnd, reportType, postRtnMsg, agentName, ref serverPathFile, ref msgID, successChecked, failChecked))
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
            if (this.radlSearchType.Items[0].Selected && this.radlSearchType.Items[1].Selected ||
               !this.radlSearchType.Items[0].Selected && !this.radlSearchType.Items[1].Selected)
            {
                fileName = BaseHelper.GetShowText("01_03120200_018") + nowDate + ".xls";
            }
            else if (this.radlSearchType.Items[0].Selected)
            {
                fileName = BaseHelper.GetShowText("01_03120200_019") + nowDate + ".xls";
            }
            else if (this.radlSearchType.Items[1].Selected)
            {
                fileName = BaseHelper.GetShowText("01_03120200_020") + nowDate + ".xls";
            }

            // 顯示提示訊息：匯出到Excel文檔資料成功
            this.Session["ServerFile"] = serverPathFile;
            this.Session["ClientFile"] = fileName;
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
        this.gpList.CurrentPageIndex = e.NewPageIndex;
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
            e.Row.Cells[0].Text = Convert.ToString((this.gpList.CurrentPageIndex-1) * this.gpList.PageSize + e.Row.RowIndex + 1);
        }
    }
    #endregion event

    #region function

    /// <summary>
    /// 從Show.xml取漢字，設置畫面控件的Text
    /// </summary>
    private void ShowControlsText()
    {
        this.gvpbR02Record.Columns[0].HeaderText = BaseHelper.GetShowText("01_03120200_008");// 序號
        this.gvpbR02Record.Columns[1].HeaderText = BaseHelper.GetShowText("01_03120200_006");// 批次日期
        this.gvpbR02Record.Columns[2].HeaderText = BaseHelper.GetShowText("01_03120200_009");// 收件編號
        this.gvpbR02Record.Columns[3].HeaderText = BaseHelper.GetShowText("01_03120200_010");// 收受行(核印行)
        this.gvpbR02Record.Columns[4].HeaderText = BaseHelper.GetShowText("01_03120200_011");// 收受行名稱(核印行)
        this.gvpbR02Record.Columns[5].HeaderText = BaseHelper.GetShowText("01_03120200_012");// 委繳戶統編\身分證字號
        this.gvpbR02Record.Columns[6].HeaderText = BaseHelper.GetShowText("01_03120200_013");// 委繳戶帳號
        this.gvpbR02Record.Columns[7].HeaderText = BaseHelper.GetShowText("01_03120200_014");// 持卡人ID
        this.gvpbR02Record.Columns[8].HeaderText = BaseHelper.GetShowText("01_03120200_015");// 申請類別
        this.gvpbR02Record.Columns[9].HeaderText = BaseHelper.GetShowText("01_03120200_016");// 成功/失敗
        this.gvpbR02Record.Columns[10].HeaderText = BaseHelper.GetShowText("01_03120200_017");// 回覆訊息

        this.radlSearchType.Items[0].Text = BaseHelper.GetShowText("01_03120200_003");// 成功
        this.radlSearchType.Items[1].Text = BaseHelper.GetShowText("01_03120200_004");// 失敗

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
            string uploadDate = this.txtBatchDateStart.Text.Trim().Replace("/", "");
            string postRtnMsg = this.dropPostRtnMsg.SelectedValue;
            string reportType = string.Empty;
            // S.成功 F.失敗 A.成功+失敗
            if (this.radlSearchType.Items[0].Selected)
            {
                reportType = (this.radlSearchType.Items[1].Selected) ? "A" : "S";
            }
            else
            {
                reportType = (this.radlSearchType.Items[1].Selected) ? "F" : "A";
            }


            string sqlText = string.Format(@"SELECT 
                                            eap.BatchDate, 
                                            eap.AuthCode, 
                                            eap.Other_Bank_Code_L, 
                                            mpc.BankName, 
                                            eap.Other_Bank_Cus_ID, 
                                            eap.Other_Bank_Acc_No,
                                            eap.Cus_ID, 
                                            eap.Apply_Type,
                                            CASE WHEN eap.Reply_Info IN ('A0', 'A4') THEN N'成功' ElSE N'失敗' END AS Status,
                                            mpc2.PROPERTY_NAME AS ReplyInfoName
                                            FROM EDDA_Auto_Pay eap
                                            LEFT JOIN 
                                            (
		                                        SELECT
			                                        bankl.property_code AS BankCodeS,
			                                        bankl.property_name AS BankCodeL,
			                                        bankn.property_name AS BankName 
		                                        FROM
			                                        ( SELECT property_code, property_name FROM {0}.dbo.m_property_code WHERE function_key = '01' AND property_key = '16' ) AS bankl,
			                                        ( SELECT property_code, property_name FROM {0}.dbo.m_property_code WHERE function_key = '01' AND property_key = '17' ) AS bankn 
		                                        WHERE
			                                        bankl.property_code= bankn.property_code 
	                                        ) AS mpc ON eap.Other_Bank_Code_L= mpc.BankCodeL
	
                                            LEFT JOIN {0}.dbo.M_PROPERTY_CODE mpc2 ON mpc2.PROPERTY_CODE = eap.Reply_Info AND mpc2.FUNCTION_KEY = '01' AND mpc2.PROPERTY_KEY = 'EddaReplyInfo'
                                            WHERE BatchDate BETWEEN @BatchDateStart AND @BatchDateEnd ", UtilHelper.GetAppSettings("DB_CSIP"));

            string sqlWhere = "";


            if (postRtnMsg.Length == 2)
            {
                sqlWhere = " AND mpc2.PROPERTY_CODE = @postRtnMsg ";
            }

            // 成功/失敗條件
            string condition = (reportType == "S") ? "AND eap.Reply_Info IN ('A0', 'A4') " : ((reportType == "F") ? "AND eap.Reply_Info NOT IN ('A0', 'A4') " : "");

            SqlCommand sqlComm = new SqlCommand { CommandType = CommandType.Text, CommandText = sqlText + sqlWhere + condition };
            sqlComm.Parameters.Add(new SqlParameter("@BatchDateStart", txtBatchDateStart.Text)); //批次起日
            sqlComm.Parameters.Add(new SqlParameter("@BatchDateEnd", txtBatchDateEnd.Text)); //批次迄日
            if (postRtnMsg.Length > 0)
            {
                sqlComm.Parameters.Add(new SqlParameter("@postRtnMsg", postRtnMsg));
            }
            DataSet dstSearchResult = BRBase<Entity_SP>.SearchOnDataSet(sqlComm, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref intTotolCount, "Connection_System");

            if (dstSearchResult == null)
            {
                base.strClientMsg += MessageHelper.GetMessage("01_03120200_002");
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
