//******************************************************************
//*  作    者：占偉林(James)
//*  功能說明：報表-R02授權成功/失敗報表
//*  創建日期：2009/12/07
//*  修改紀錄：
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Text;
using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using Framework.Common.Logging;
using Framework.Common.Utility;

public partial class P010305020001 : PageBase
{
    #region event

    /// 作者 占偉林

    /// 創建日期：2009/12/07
    /// 修改日期：2009/12/07 
    /// <summary>
    /// 畫面裝載時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //* 綁定GridView列頭訊息
            ShowControlsText();
            string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
            strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
            //* 【首錄起迄日】起
            this.txtInputDateStart.Text = strYYYYMMDD;
            //* 【首錄起迄日】迄
            this.txtInputDateEnd.Text = strYYYYMMDD;
            //* 設置光標
            this.txtInputDateStart.Focus();
            this.gpList.RecordCount = 0;
            this.gpList.Visible = false;
            this.gvpbR02Record.DataSource = null;
            this.gvpbR02Record.DataBind();
        }
        base.strHostMsg += "";
    }

    ///// 作者 占偉林

    ///// 創建日期：2009/12/07
    ///// 修改日期：2009/12/07 
    /// <summary>
    /// 點選畫面【查詢】按鈕時的處理

    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        Dictionary<string,string> dirValues = new Dictionary<string,string>();

        //* 首錄起迄日
        dirValues.Add("txtInputDateStart", this.txtInputDateStart.Text.Trim());
        dirValues.Add("txtInputDateEnd", this.txtInputDateEnd.Text.Trim());

        //* 成功
        if (this.radlSearchType.Items[0].Selected)
            dirValues.Add("Success", "1");
        else
            dirValues.Add("Success", "0");
        //* 失敗
        if (this.radlSearchType.Items[1].Selected)
            dirValues.Add("Fault", "1");
        else
            dirValues.Add("Fault", "0");

        //* 將查詢條件保存到ViewState中
        this.ViewState["dirValues"] = dirValues;
        this.gpList.Visible = true;
        this.gpList.CurrentPageIndex = 0;
        BindGridView((Dictionary<string,string>)this.ViewState["dirValues"]);
    }

    ///// 作者 占偉林

    ///// 創建日期：2009/12/07
    ///// 修改日期：2009/12/07 
    /// <summary>
    /// 點選畫面【列印】按鈕時的處理

    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        Dictionary<string, string> dirValues = new Dictionary<string, string>();

        //* 首錄起迄日
        dirValues.Add("txtInputDateStart", this.txtInputDateStart.Text.Trim());
        dirValues.Add("txtInputDateEnd", this.txtInputDateEnd.Text.Trim());

        //* 成功
        if (this.radlSearchType.Items[0].Selected)
            dirValues.Add("Success", "1");
        else
            dirValues.Add("Success", "0");
        //* 失敗
        if (this.radlSearchType.Items[1].Selected)
            dirValues.Add("Fault", "1");
        else
            dirValues.Add("Fault", "0");

        //* 匯出到Excel.
        OutputExcel(dirValues);
    }

    /// <summary>
    /// 將查詢結果匯出到Excel文檔。

    /// </summary>
    protected void OutputExcel(Dictionary<string,string> dirValues)
    {
        try
        {
            string strMsgID = "";
            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
            //* 服務器端，生成Excel文檔
            if (!BRExcel_File.CreateExcelFile_R02(dirValues,
                            ((EntityAGENT_INFO)this.Session["Agent"]).agent_name,
                            ref strServerPathFile,
                            ref strMsgID))
            {
                if (strMsgID != "")
                    base.strClientMsg += MessageHelper.GetMessage(strMsgID);
                else
                    base.strClientMsg += MessageHelper.GetMessage("01_03050200_004");
                return;
            }

            //* 將服務器端生成的文檔，下載到本地。
            string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
            strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
            string strFileName = "";
            if (this.radlSearchType.Items[0].Selected && this.radlSearchType.Items[1].Selected ||
               !this.radlSearchType.Items[0].Selected && !this.radlSearchType.Items[1].Selected)
            {
                strFileName = BaseHelper.GetShowText("01_03050200_018") + strYYYYMMDD + ".xls";
            }
            else if (this.radlSearchType.Items[0].Selected)
            {
                strFileName = BaseHelper.GetShowText("01_03050200_019") + strYYYYMMDD + ".xls";
            }
            else if (this.radlSearchType.Items[1].Selected)
            {
                strFileName = BaseHelper.GetShowText("01_03050200_020") + strYYYYMMDD + ".xls";
            }

            //* 顯示提示訊息：匯出到Excel文檔資料成功
            this.Session["ServerFile"] = strServerPathFile;
            this.Session["ClientFile"] = strFileName;
            // string urlString = @"ClientMsgShow('" + MessageHelper.GetMessage("01_03050200_005") + "');";
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("01_03050200_005") + "' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);
        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            base.strClientMsg += BaseHelper.ClientMsgShow("01_03050200_004");
        }
    }


    /// 作者 占偉林

    /// 創建日期：2009/12/07
    /// 修改日期：2009/12/07 
    /// <summary>
    /// 頁導航

    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView((Dictionary<string,string>)this.ViewState["dirValues"]);
    }

    /// 作者 占偉林

    /// 創建日期：2009/12/07 
    /// 修改日期：2009/12/07 
    /// <summary>
    /// CustGridView行綁定

    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbR02Record_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //*  查詢結果的columns[8]+ ":"+ columns[9],若為“：“，顯示”系統內無該錯誤代碼對應訊息“

            if (e.Row.Cells[9].Text.Trim().Length == 0 && e.Row.Cells[10].Text.Trim().Length == 0)
            {
                e.Row.Cells[9].Text = "系統內無該錯誤代碼對應訊息";
            }
            else
            {
                e.Row.Cells[9].Text = e.Row.Cells[9].Text.Trim() + ":" + e.Row.Cells[10].Text.Trim();
            }

            //* 序號 = 當前頁號 * 每頁行數 + 頁內順序號 + 1
            e.Row.Cells[0].Text = Convert.ToString((this.gpList.CurrentPageIndex-1) * this.gpList.PageSize + e.Row.RowIndex + 1);
        }
    }
    #endregion event

    #region function
    /// 作者 占偉林

    /// 創建日期：2009/12/07
    /// 修改日期：2009/12/07 
    /// <summary>
    /// 從Show.xml取漢字，設置畫面控件的Text
    /// </summary>
    private void ShowControlsText()
    {
        this.gvpbR02Record.Columns[0].HeaderText = BaseHelper.GetShowText("01_03050200_008");
        this.gvpbR02Record.Columns[1].HeaderText = BaseHelper.GetShowText("01_03050200_009");
        this.gvpbR02Record.Columns[2].HeaderText = BaseHelper.GetShowText("01_03050200_010");
        this.gvpbR02Record.Columns[3].HeaderText = BaseHelper.GetShowText("01_03050200_011");
        this.gvpbR02Record.Columns[4].HeaderText = BaseHelper.GetShowText("01_03050200_012");
        this.gvpbR02Record.Columns[5].HeaderText = BaseHelper.GetShowText("01_03050200_013");
        this.gvpbR02Record.Columns[6].HeaderText = BaseHelper.GetShowText("01_03050200_014");
        this.gvpbR02Record.Columns[7].HeaderText = BaseHelper.GetShowText("01_03050200_015");
        this.gvpbR02Record.Columns[8].HeaderText = BaseHelper.GetShowText("01_03050200_016");
        this.gvpbR02Record.Columns[9].HeaderText = BaseHelper.GetShowText("01_03050200_017");

        this.radlSearchType.Items[0].Text = BaseHelper.GetShowText("01_03050200_003");
        this.radlSearchType.Items[1].Text = BaseHelper.GetShowText("01_03050200_004");

        //* 設置每頁顯示記錄最大條數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gvpbR02Record.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
    }

    /// 作者 占偉林

    /// 創建日期：2009/12/07
    /// 修改日期：2009/12/07 
    /// <summary>
    /// 綁定GridView數據源

    /// </summary>
    private void BindGridView(Dictionary<string, string> dirValues)
    {
        try
        {
            int intTotolCount = 0;
            DataSet dstSearchResult = (DataSet)BROTHER_BANK_TEMP.SearchR02Record(dirValues, 
                    this.gpList.CurrentPageIndex, this.gpList.PageSize, ref intTotolCount);
            if (dstSearchResult == null)
            {
                //* 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage("01_03050200_002");
                this.gpList.RecordCount = 0;
                this.gvpbR02Record.DataSource = null;
                this.gvpbR02Record.DataBind();
            }
            else
            {
                this.gpList.RecordCount = intTotolCount;
                this.gvpbR02Record.DataSource = dstSearchResult.Tables[0];
                this.gvpbR02Record.DataBind();
                //* 顯示端末訊息
                if (intTotolCount == 0)
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_03050200_001");
                }
                else
                {
                    base.strClientMsg += MessageHelper.GetMessage("01_03050200_003");
                }
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("01_03050200_002");
        }
    }

    #endregion function
}
