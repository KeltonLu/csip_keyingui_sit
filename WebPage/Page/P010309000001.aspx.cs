//******************************************************************
//* 作    者：
//* 功能說明：
//* 創建日期：
//* 修改紀錄：2021/02/01_Ares_Stanley-新增Common.JavaScript
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
using Framework.Common.Message;
using Framework.Common.Logging;
using Framework.Common.Utility;
using Framework.Common.JavaScript;

public partial class P010309000001 : PageBase
{
    #region event
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //* 設置每頁顯示記錄最大條數
            this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
            this.gvpbDetail.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());

            this.dtpSearchStart.Text = DateTime.Now.ToString("yyyy/MM/dd");
            this.dtpSearchEnd.Text = DateTime.Now.ToString("yyyy/MM/dd");
            this.dtpSearchStart .Focus();
            this.gpList.RecordCount = 0;
            this.gpList.Visible = false;


            this.gvpbDetail.DataSource = null;
            this.gvpbDetail.DataBind();
            this.gvpbDetail.Visible = false;
        }
    }

    protected void gvpbDetail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[0].Text = BaseHelper.GetShowText("01_03090000_002");   //收件編號

            e.Row.Cells[1].Text = BaseHelper.GetShowText("01_03090000_003");   //作業種類

            e.Row.Cells[2].Text = BaseHelper.GetShowText("01_03090000_004");   //收件時間

            e.Row.Cells[3].Text = BaseHelper.GetShowText("01_03090000_005");   //一KEY經辦

            e.Row.Cells[4].Text = BaseHelper.GetShowText("01_03090000_006");   //提交時間 - 一KEY經辦

            e.Row.Cells[5].Text = BaseHelper.GetShowText("01_03090000_007");   //二KEY經辦

            e.Row.Cells[6].Text = BaseHelper.GetShowText("01_03090000_006");   //提交時間 - 二KEY經辦

            e.Row.Cells[7].Text = BaseHelper.GetShowText("01_03090000_008");   //比對結果是否一致
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells[0].Text.ToUpper().Trim().StartsWith("RC") || e.Row.Cells[0].Text.ToUpper().Trim().StartsWith("AU"))
            {
                e.Row.Cells[1].Text = "C";
            }
            else
            {
                e.Row.Cells[1].Text = "A";
            }
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbDetail.Visible = true;
        this.gpList.Visible = true;
        this.gpList.CurrentPageIndex = 0;
        BindGridView();
    }
    /// <summary>
    /// 修改紀錄:2021/02/03_Ares_Stanley-增加條件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        string strRECEIVENUMBER = "";
        string strReceiveDateStart = "";
        string strReceiveDateEnd = "";
        string strMERCHANT = "";

        REF_Condition(ref strRECEIVENUMBER, ref strReceiveDateStart, ref strReceiveDateEnd, ref strMERCHANT);
        if (!dateRangeCheck(strReceiveDateStart, strReceiveDateEnd))
        {
            return;
        }

        OutputExcel();
    }

    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }
    #endregion event

    #region method

    /// <summary>
    /// 綁定GridView數據源
    /// 修改紀錄:2021/02/01_Ares_Stanley-收件日期調整為區間
    /// </summary>
    private void BindGridView()
    {
        int intTotolCount = 0;
        string strRECEIVENUMBER = "";
        string strReceiveDateStart = "";
        string strReceiveDateEnd = "";
        string strMERCHANT = "";

        REF_Condition(ref strRECEIVENUMBER, ref strReceiveDateStart, ref strReceiveDateEnd, ref strMERCHANT);
        if (!dateRangeCheck(strReceiveDateStart, strReceiveDateEnd))
        {
            return;
        }

        gvpbDetail.DataSource = BRRedeemSet.GetFileDetail(strRECEIVENUMBER, strReceiveDateStart, strReceiveDateEnd, strMERCHANT, gpList.CurrentPageIndex, gpList.PageSize, ref intTotolCount);
        gpList.RecordCount = intTotolCount;
        gvpbDetail.DataBind();

        if (0 == intTotolCount)
        {
            gvpbDetail.Visible = false;
        }

    }
    /// <summary>
    /// 檢查日期區間是否大於31天
    /// </summary>
    /// <param name="strReceiveDateStart"></param>
    /// <param name="strReceiveDateEnd"></param>
    /// <returns></returns>
    private bool dateRangeCheck(string strReceiveDateStart, string strReceiveDateEnd)
    {
        if (strReceiveDateEnd == "" || strReceiveDateStart == "")
        {
            jsBuilder.RegScript(this, jsBuilder.GetAlert("起/訖日不得為空"));
            gvpbDetail.Visible = false;
            gpList.Visible = false;
            return false;
        }
        if (int.Parse(strReceiveDateEnd) < int.Parse(strReceiveDateStart))
        {
            jsBuilder.RegScript(this, jsBuilder.GetAlert("結束日期不可小於起始日期！"));
            gvpbDetail.Visible = false;
            gpList.Visible = false;
            return false;
        }
        var dtStart = DateTime.Parse(this.dtpSearchStart.Text);
        var dtEnd = DateTime.Parse(this.dtpSearchEnd.Text);
        int dtRange = new TimeSpan(dtEnd.Ticks - dtStart.Ticks).Days;
        if (dtRange > 30)
        {
            jsBuilder.RegScript(this, jsBuilder.GetAlert("日期區間不可以大於31天！"));
            gvpbDetail.Visible = false;
            gpList.Visible = false;
            return false;
        }
        return true;
    }
    /// <summary>
    /// 修改紀錄:2021/02/01_Ares_Stanley-收件日期調整為區間
    /// </summary>
    /// <param name="strRECEIVENUMBER"></param>
    /// <param name="strReceiveDateStart"></param>
    /// <param name="strReceiveDateEnd"></param>
    /// <param name="strMERCHANT"></param>
    private void REF_Condition(ref string strRECEIVENUMBER, ref string strReceiveDateStart, ref string strReceiveDateEnd, ref string strMERCHANT)
    {
        if ("" == dtpSearchStart.Text.Trim())
        {
            strReceiveDateStart = "";
        }
        else
        {
            strReceiveDateStart = dtpSearchStart.Text.Replace("/", "").Trim();
        }

        if("" == dtpSearchEnd.Text.Trim())
        {
            strReceiveDateEnd = "";
        }
        else
        {
            strReceiveDateEnd = dtpSearchEnd.Text.Replace("/", "").Trim();
        }

        if ("" == txtReceiveNumber.Text.Trim())
        {
            strRECEIVENUMBER = "%";
        }
        else
        {
            strRECEIVENUMBER = txtReceiveNumber.Text.Trim();
        }

        if ("" == txtMERCHANTNO.Text.Trim())
        {
            strMERCHANT = "";
        }
        else
        {
            strMERCHANT = txtMERCHANTNO.Text.Trim();
        }


    }
    /// <summary>
    /// 修改紀錄:2021/02/01_Ares_Stanley-收件日期調整為區間
    /// </summary>
    protected void OutputExcel()
    {
        try
        {
            string strMsgID = "";
            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
            string strRECEIVENUMBER = "";
            string strReceiveDateStart = "";
            string strReceiveDateEnd = "";
            string strMERCHANT = "";

            REF_Condition(ref strRECEIVENUMBER, ref strReceiveDateStart, ref strReceiveDateEnd, ref strMERCHANT);

            if (!BRExcel_File.CreateExcelFile_FileDetail(strRECEIVENUMBER, strReceiveDateStart, strReceiveDateEnd, strMERCHANT, ref strServerPathFile, ref strMsgID))
            {
                if (strMsgID != "")
                    base.strClientMsg += MessageHelper.GetMessage(strMsgID);
                else
                    base.strClientMsg += MessageHelper.GetMessage("01_03090000_001");
                return;
            }

            //* 將服務器端生成的文檔，下載到本地。
            string strFileName = BaseHelper.GetShowText("01_03090000_001") + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            //* 顯示提示訊息：匯出到Excel文檔資料成功
            this.Session["ServerFile"] = strServerPathFile;
            this.Session["ClientFile"] = strFileName;
            // string urlString = @"ClientMsgShow('" + MessageHelper.GetMessage("01_03090000_002") + "');";
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("01_03090000_002") + "' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);

        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            base.strClientMsg += BaseHelper.ClientMsgShow("01_03090000_001");
        }
    }
    #endregion method


}
