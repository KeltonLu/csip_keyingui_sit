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
using System.Globalization;
using CSIPKeyInGUI.BusinessRules;
using Framework.Common.Message;
using Framework.Common.Logging;
using Framework.Common.Utility;
using Framework.Common.JavaScript;

public partial class P010307000001 : PageBase
{
    #region event
    /// <summary>
    /// 修改紀錄:2021/03/08_Ares_Stanley-查詢單一日期改為查詢日期區間
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //* 設置每頁顯示記錄最大條數
            this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
            this.gvpbRedeem.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());

            this.txtReceiveDateStart.Text = DateTime.Now.ToString("yyyyMMdd");
            this.txtReceiveDateEnd.Text = DateTime.Now.ToString("yyyyMMdd");
            this.txtReceiveDateStart.Focus();
            this.gpList.RecordCount = 0;
            this.gpList.Visible = false;


            this.gvpbRedeem.DataSource = null;
            this.gvpbRedeem.DataBind();
            this.gvpbRedeem.Visible = false;
        }
    }

    protected void gvpbRedeem_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            TableCellCollection tcHeader = e.Row.Cells;
            tcHeader.Clear();

            #region first row
            tcHeader.Add(new TableHeaderCell());
            tcHeader[0].RowSpan = 2;
            tcHeader[0].Text = BaseHelper.GetShowText("01_03070000_006");   //收件編號

            tcHeader.Add(new TableHeaderCell());
            tcHeader[1].RowSpan = 2;
            tcHeader[1].Text = BaseHelper.GetShowText("01_03070000_008");   //作業類型

            tcHeader.Add(new TableHeaderCell());
            tcHeader[2].RowSpan = 2;
            tcHeader[2].Text = BaseHelper.GetShowText("01_03070000_009");   //MSG-SEQ

            tcHeader.Add(new TableHeaderCell());
            tcHeader[3].RowSpan = 2;
            tcHeader[3].Text = BaseHelper.GetShowText("01_03070000_010");   //MSG-ERR

            tcHeader.Add(new TableHeaderCell());
            tcHeader[4].RowSpan = 2;
            tcHeader[4].Text = BaseHelper.GetShowText("01_03070000_011");   //ORG

            tcHeader.Add(new TableHeaderCell());
            tcHeader[5].RowSpan = 2;
            tcHeader[5].Text = BaseHelper.GetShowText("01_03070000_012");   //MERCHANT

            tcHeader.Add(new TableHeaderCell());
            tcHeader[6].RowSpan = 2;
            tcHeader[6].Text = BaseHelper.GetShowText("01_03070000_013");   //PROD-CODE

            tcHeader.Add(new TableHeaderCell());
            tcHeader[7].RowSpan = 2;
            tcHeader[7].Text = BaseHelper.GetShowText("01_03070000_014");   //CARD-TYPE

            tcHeader.Add(new TableHeaderCell());
            tcHeader[8].RowSpan = 2;
            tcHeader[8].Text = BaseHelper.GetShowText("01_03070000_015");   //PROGRAM

            tcHeader.Add(new TableHeaderCell());
            tcHeader[9].RowSpan = 2;
            tcHeader[9].Text = BaseHelper.GetShowText("01_03070000_016");   //本行分攤比例%

            tcHeader.Add(new TableHeaderCell());
            tcHeader[10].ColumnSpan = 3;
            tcHeader[10].Text = BaseHelper.GetShowText("01_03070000_017");   //長期活動

            tcHeader.Add(new TableHeaderCell());
            tcHeader[11].ColumnSpan = 7;
            tcHeader[11].Text = BaseHelper.GetShowText("01_03070000_021");   //短期活動

            tcHeader.Add(new TableHeaderCell());
            tcHeader[12].ColumnSpan = 6;
            tcHeader[12].Text = BaseHelper.GetShowText("01_03070000_026") + @"</th></tr><tr>";   //生日活動

            #endregion first row

            #region second row
            //長期活動
            tcHeader.Add(new TableHeaderCell());
            tcHeader[13].Text = BaseHelper.GetShowText("01_03070000_018");   //長期活動 - 折抵上限

            tcHeader.Add(new TableHeaderCell());
            tcHeader[14].Text = BaseHelper.GetShowText("01_03070000_019");   //長期活動 - 點數/金額抵用比率

            tcHeader.Add(new TableHeaderCell());
            tcHeader[15].Text = BaseHelper.GetShowText("01_03070000_020");   //長期活動 - 可折抵金額

            //短期活動
            tcHeader.Add(new TableHeaderCell());
            tcHeader[16].Text = BaseHelper.GetShowText("01_03070000_022");   //短期活動 - 類型

            tcHeader.Add(new TableHeaderCell());
            tcHeader[17].Text = BaseHelper.GetShowText("01_03070000_023");   //短期活動 - 活動起日

            tcHeader.Add(new TableHeaderCell());
            tcHeader[18].Text = BaseHelper.GetShowText("01_03070000_024");   //短期活動 - 活動迄日

            tcHeader.Add(new TableHeaderCell());
            tcHeader[19].Text = BaseHelper.GetShowText("01_03070000_025");   //短期活動 - 指定時間之短期促銷

            tcHeader.Add(new TableHeaderCell());
            tcHeader[20].Text = BaseHelper.GetShowText("01_03070000_018");   //短期活動 - 折抵上限

            tcHeader.Add(new TableHeaderCell());
            tcHeader[21].Text = BaseHelper.GetShowText("01_03070000_019");   //短期活動 - 點數/金額抵用比率

            tcHeader.Add(new TableHeaderCell());
            tcHeader[22].Text = BaseHelper.GetShowText("01_03070000_020");   //短期活動 - 可折抵金額

            //生日活動
            tcHeader.Add(new TableHeaderCell());
            tcHeader[23].Text = BaseHelper.GetShowText("01_03070000_022");   //生日活動 - 類型

            tcHeader.Add(new TableHeaderCell());
            tcHeader[24].Text = BaseHelper.GetShowText("01_03070000_023");   //生日活動 - 活動起日

            tcHeader.Add(new TableHeaderCell());
            tcHeader[25].Text = BaseHelper.GetShowText("01_03070000_024");   //生日活動 - 活動迄日

            tcHeader.Add(new TableHeaderCell());
            tcHeader[26].Text = BaseHelper.GetShowText("01_03070000_018");   //生日活動 - 折抵上限

            tcHeader.Add(new TableHeaderCell());
            tcHeader[27].Text = BaseHelper.GetShowText("01_03070000_019");   //生日活動 - 點數/金額抵用比率

            tcHeader.Add(new TableHeaderCell());
            tcHeader[28].Text = BaseHelper.GetShowText("01_03070000_020") + @"</th></tr><tr>";   //生日活動 - 可折抵金額

            for (int i = 0; i < 29; i++)
            {
                tcHeader[i].CssClass = "Grid_Header";
                tcHeader[i].Wrap = false;
            }
            #endregion second row

        }
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        OutputExcel();
        BindGridView();
    }

    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbRedeem.Visible = true;
        this.gpList.Visible = true;
        this.gpList.CurrentPageIndex = 0;
        BindGridView();
    }

    #endregion event

    #region method

    /// <summary>
    /// 綁定GridView數據源
    /// 修改紀錄:2021/03/08_Ares_Stanley-查詢單一日期改為查詢日期區間
    /// </summary>
    private void BindGridView()
    {
        int intTotolCount = 0;
        string strRECEIVENUMBER = "";
        string strReceiveDateStart = "";
        string strReceiveDateEnd = "";
        string strMERCHANT = "";
        bool withDATE = false;

        REF_Condition(ref strRECEIVENUMBER, ref strReceiveDateStart, ref strReceiveDateEnd, ref strMERCHANT, ref withDATE);
        if (withDATE)
        {
            if (!dateRangeCheck(strReceiveDateStart, strReceiveDateEnd))
            {
                return;
            }
        }

        
        gvpbRedeem.DataSource = BRRedeem3270.GetDetailReport(strRECEIVENUMBER, strReceiveDateStart, strReceiveDateEnd, strMERCHANT, gpList.CurrentPageIndex, gpList.PageSize, ref intTotolCount, withDATE);
        gpList.RecordCount = intTotolCount;
        gvpbRedeem.DataBind();

        if (0 == intTotolCount)
        {
            gvpbRedeem.Visible = false;
        }

    }
    /// <summary>
    /// 檢測日期不可大於90天
    /// </summary>
    /// <param name="strReceiveDateStart"></param>
    /// <param name="strReceiveDateEnd"></param>
    /// <returns></returns>
    private bool dateRangeCheck(string strReceiveDateStart, string strReceiveDateEnd)
    {
        var dtStart = DateTime.ParseExact(this.txtReceiveDateStart.Text, "yyyyMMdd", CultureInfo.InvariantCulture);
        var dtEnd = DateTime.ParseExact(this.txtReceiveDateEnd.Text, "yyyyMMdd", CultureInfo.InvariantCulture);
        int dtRange = new TimeSpan(dtEnd.Ticks - dtStart.Ticks).Days;
        if (dtRange > 90)
        {
            jsBuilder.RegScript(this, jsBuilder.GetAlert("日期區間不可以大於90天！"));
            gvpbRedeem.Visible = false;
            gpList.Visible = false;
            return false;
        }
        return true;
    }
    /// <summary>
    /// 修改紀錄:2021/03/08_Ares_Stanley-查詢單一日期改為查詢日期區間
    /// </summary>
    /// <param name="strRECEIVENUMBER"></param>
    /// <param name="strReceiveDateStart"></param>
    /// <param name="strReceiveDateEnd"></param>
    /// <param name="strMERCHANT"></param>
    /// <param name="withDATE"></param>
    private void REF_Condition(ref string strRECEIVENUMBER, ref string strReceiveDateStart, ref string strReceiveDateEnd, ref string strMERCHANT, ref bool withDATE)
    {
        if ("" == txtReceiveDateStart.Text.Trim())
        {
            strReceiveDateStart = "%";
        }
        else
        {
            strReceiveDateStart = txtReceiveDateStart.Text.Trim();
        }

        if ("" == txtReceiveDateEnd.Text.Trim())
        {
            strReceiveDateEnd = "%";
        }
        else
        {
            strReceiveDateEnd = txtReceiveDateEnd.Text.Trim();
        }

        if (txtReceiveDateStart.Text != "" && txtReceiveDateEnd.Text != "")
        {
            withDATE = true;
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
            strMERCHANT = "%";
        }
        else
        {
            strMERCHANT = txtMERCHANTNO.Text.Trim();
        }
    }

    /// <summary>
    /// 功能： Redeem 交易結果明細表產出
    /// 修改日期: 2020/12/07_Ares_Stanley-變更下載檔名; 2021/03/08_Ares_Stanley-查詢單一日期改為查詢日期區間
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
            bool withDATE = false;
            string strMERCHANT = "";

            REF_Condition(ref strRECEIVENUMBER, ref strReceiveDateStart, ref strReceiveDateEnd, ref strMERCHANT, ref withDATE);
            if (withDATE)
            {
                if (!dateRangeCheck(strReceiveDateStart, strReceiveDateEnd))
                {
                    return;
                }
            }

            if (!BRExcel_File.CreateExcelFile_DetailRedeem(strRECEIVENUMBER, strReceiveDateStart, strReceiveDateEnd, strMERCHANT, ref strServerPathFile, ref strMsgID, withDATE))
            {
                if (strMsgID != "")
                    base.strClientMsg += MessageHelper.GetMessage(strMsgID);
                else
                    base.strClientMsg += MessageHelper.GetMessage("01_03080000_001");
                return;
            }

            //* 將服務器端生成的文檔，下載到本地。
            string strFileName = (BaseHelper.GetShowText("01_03070000_001") + DateTime.Now.ToString("yyyyMMdd") + ".xls").Replace(" ","");
            //* 顯示提示訊息：匯出到Excel文檔資料成功
            this.Session["ServerFile"] = strServerPathFile;
            this.Session["ClientFile"] = strFileName;
            // string urlString = @"ClientMsgShow('" + MessageHelper.GetMessage("01_03080000_002") + "');";
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("01_03080000_002") + "' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);

        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            base.strClientMsg += BaseHelper.ClientMsgShow("01_03080000_001");
        }
    }

    #endregion method

}
