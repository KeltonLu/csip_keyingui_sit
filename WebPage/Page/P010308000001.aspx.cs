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
using Framework.Common.Message;
using Framework.Common.Logging;
using Framework.Common.Utility;

public partial class P010308000001 : PageBase
{
    #region event
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //* 設置每頁顯示記錄最大條數
            this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
            this.gvpbAward.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());

            this.txtReceiveDate.Text = DateTime.Now.ToString("yyyyMMdd");
            this.txtReceiveDate.Focus();
            this.gpList.RecordCount = 0;
            this.gpList.Visible = false;

            this.gvpbAward.DataSource = null;
            this.gvpbAward.DataBind();
            this.gvpbAward.Visible = false;
        }
    }

    protected void gvpbAward_RowDataBound(object sender, GridViewRowEventArgs e)
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
            tcHeader[5].Text = BaseHelper.GetShowText("01_03070000_027");   //PROG-NO

            tcHeader.Add(new TableHeaderCell());
            tcHeader[6].RowSpan = 2;
            tcHeader[6].Text = BaseHelper.GetShowText("01_03070000_028");   //PARTNER-NO

            tcHeader.Add(new TableHeaderCell());
            tcHeader[7].RowSpan = 2;
            tcHeader[7].Text = BaseHelper.GetShowText("01_03070000_014");   //CARD-TYPE

            tcHeader.Add(new TableHeaderCell());
            tcHeader[8].RowSpan = 2;
            tcHeader[8].Text = BaseHelper.GetShowText("01_03070000_029");   //TC-CODE(+)

            tcHeader.Add(new TableHeaderCell());
            tcHeader[9].RowSpan = 2;
            tcHeader[9].Text = BaseHelper.GetShowText("01_03070000_030");   //TC-CODE(-)

            tcHeader.Add(new TableHeaderCell());
            tcHeader[10].ColumnSpan = 10;
            tcHeader[10].Text = BaseHelper.GetShowText("01_03070000_031");   //MCC CODE

            tcHeader.Add(new TableHeaderCell());
            tcHeader[11].RowSpan = 2;
            tcHeader[11].Text = BaseHelper.GetShowText("01_03070000_042");   //消費地區FLAG

            tcHeader.Add(new TableHeaderCell());
            tcHeader[12].ColumnSpan = 10;
            tcHeader[12].Text = BaseHelper.GetShowText("01_03070000_043");   //消費地區代碼

            tcHeader.Add(new TableHeaderCell());
            tcHeader[13].ColumnSpan = 9;
            tcHeader[13].Text = BaseHelper.GetShowText("01_03070000_044");   //長期活動-正卡

            tcHeader.Add(new TableHeaderCell());
            tcHeader[14].ColumnSpan = 9;
            tcHeader[14].Text = BaseHelper.GetShowText("01_03070000_054");   //長期活動-附卡

            tcHeader.Add(new TableHeaderCell());
            tcHeader[15].ColumnSpan = 11;
            tcHeader[15].Text = BaseHelper.GetShowText("01_03070000_055");   //促銷活動-短期活動

            tcHeader.Add(new TableHeaderCell());
            tcHeader[16].ColumnSpan = 11;
            tcHeader[16].Text = BaseHelper.GetShowText("01_03070000_056") + @"</th></tr><tr>";   //促銷活動-生日活動

            #endregion first row

            #region second row
            //MCC CODE
            tcHeader.Add(new TableHeaderCell());
            tcHeader[17].Text = BaseHelper.GetShowText("01_03070000_032");   //MCC CODE - 1

            tcHeader.Add(new TableHeaderCell());
            tcHeader[18].Text = BaseHelper.GetShowText("01_03070000_033");   //MCC CODE - 2

            tcHeader.Add(new TableHeaderCell());
            tcHeader[19].Text = BaseHelper.GetShowText("01_03070000_034");   //MCC CODE - 3

            tcHeader.Add(new TableHeaderCell());
            tcHeader[20].Text = BaseHelper.GetShowText("01_03070000_035");   //MCC CODE - 4

            tcHeader.Add(new TableHeaderCell());
            tcHeader[21].Text = BaseHelper.GetShowText("01_03070000_036");   //MCC CODE - 5

            tcHeader.Add(new TableHeaderCell());
            tcHeader[22].Text = BaseHelper.GetShowText("01_03070000_037");   //MCC CODE - 6

            tcHeader.Add(new TableHeaderCell());
            tcHeader[23].Text = BaseHelper.GetShowText("01_03070000_038");   //MCC CODE - 7

            tcHeader.Add(new TableHeaderCell());
            tcHeader[24].Text = BaseHelper.GetShowText("01_03070000_039");   //MCC CODE - 8

            tcHeader.Add(new TableHeaderCell());
            tcHeader[25].Text = BaseHelper.GetShowText("01_03070000_040");   //MCC CODE - 9

            tcHeader.Add(new TableHeaderCell());
            tcHeader[26].Text = BaseHelper.GetShowText("01_03070000_041");   //MCC CODE - 10

            //消費地區代碼
            tcHeader.Add(new TableHeaderCell());
            tcHeader[27].Text = BaseHelper.GetShowText("01_03070000_032");   //消費地區代碼 - 1

            tcHeader.Add(new TableHeaderCell());
            tcHeader[28].Text = BaseHelper.GetShowText("01_03070000_033");   //消費地區代碼 - 2

            tcHeader.Add(new TableHeaderCell());
            tcHeader[29].Text = BaseHelper.GetShowText("01_03070000_034");   //消費地區代碼 - 3

            tcHeader.Add(new TableHeaderCell());
            tcHeader[30].Text = BaseHelper.GetShowText("01_03070000_035");   //消費地區代碼 - 4

            tcHeader.Add(new TableHeaderCell());
            tcHeader[31].Text = BaseHelper.GetShowText("01_03070000_036");   //消費地區代碼 - 5

            tcHeader.Add(new TableHeaderCell());
            tcHeader[32].Text = BaseHelper.GetShowText("01_03070000_037");   //消費地區代碼 - 6

            tcHeader.Add(new TableHeaderCell());
            tcHeader[33].Text = BaseHelper.GetShowText("01_03070000_038");   //消費地區代碼 - 7

            tcHeader.Add(new TableHeaderCell());
            tcHeader[34].Text = BaseHelper.GetShowText("01_03070000_039");   //消費地區代碼 - 8

            tcHeader.Add(new TableHeaderCell());
            tcHeader[35].Text = BaseHelper.GetShowText("01_03070000_040");   //消費地區代碼 - 9

            tcHeader.Add(new TableHeaderCell());
            tcHeader[36].Text = BaseHelper.GetShowText("01_03070000_041");   //消費地區代碼 - 10

            //長期活動-正卡
            tcHeader.Add(new TableHeaderCell());
            tcHeader[37].Text = BaseHelper.GetShowText("01_03070000_045");   //長期活動-正卡 - 給點方式

            tcHeader.Add(new TableHeaderCell());
            tcHeader[38].Text = BaseHelper.GetShowText("01_03070000_046");   //長期活動-正卡 - AMT1

            tcHeader.Add(new TableHeaderCell());
            tcHeader[39].Text = BaseHelper.GetShowText("01_03070000_047");   //長期活動-正卡 - RATE1

            tcHeader.Add(new TableHeaderCell());
            tcHeader[40].Text = BaseHelper.GetShowText("01_03070000_048");   //長期活動-正卡 - AMT2

            tcHeader.Add(new TableHeaderCell());
            tcHeader[41].Text = BaseHelper.GetShowText("01_03070000_049");   //長期活動-正卡 - RATE2

            tcHeader.Add(new TableHeaderCell());
            tcHeader[42].Text = BaseHelper.GetShowText("01_03070000_050");   //長期活動-正卡 - AMT3

            tcHeader.Add(new TableHeaderCell());
            tcHeader[43].Text = BaseHelper.GetShowText("01_03070000_051");   //長期活動-正卡 - RATE3

            tcHeader.Add(new TableHeaderCell());
            tcHeader[44].Text = BaseHelper.GetShowText("01_03070000_052");   //長期活動-正卡 - AMT4

            tcHeader.Add(new TableHeaderCell());
            tcHeader[45].Text = BaseHelper.GetShowText("01_03070000_053");   //長期活動-正卡 - RATE4

            //長期活動-附卡
            tcHeader.Add(new TableHeaderCell());
            tcHeader[46].Text = BaseHelper.GetShowText("01_03070000_045");   //長期活動-附卡 - 給點方式

            tcHeader.Add(new TableHeaderCell());
            tcHeader[47].Text = BaseHelper.GetShowText("01_03070000_046");   //長期活動-附卡 - AMT1

            tcHeader.Add(new TableHeaderCell());
            tcHeader[48].Text = BaseHelper.GetShowText("01_03070000_047");   //長期活動-附卡 - RATE1

            tcHeader.Add(new TableHeaderCell());
            tcHeader[49].Text = BaseHelper.GetShowText("01_03070000_048");   //長期活動-附卡 - AMT2

            tcHeader.Add(new TableHeaderCell());
            tcHeader[50].Text = BaseHelper.GetShowText("01_03070000_049");   //長期活動-附卡 - RATE2

            tcHeader.Add(new TableHeaderCell());
            tcHeader[51].Text = BaseHelper.GetShowText("01_03070000_050");   //長期活動-附卡 - AMT3

            tcHeader.Add(new TableHeaderCell());
            tcHeader[52].Text = BaseHelper.GetShowText("01_03070000_051");   //長期活動-附卡 - RATE3

            tcHeader.Add(new TableHeaderCell());
            tcHeader[53].Text = BaseHelper.GetShowText("01_03070000_052");   //長期活動-附卡 - AMT4

            tcHeader.Add(new TableHeaderCell());
            tcHeader[54].Text = BaseHelper.GetShowText("01_03070000_053");   //長期活動-附卡 - RATE4

            //促銷活動-短期活動
            tcHeader.Add(new TableHeaderCell());
            tcHeader[55].Text = BaseHelper.GetShowText("01_03070000_023");   //促銷活動-短期活動 - 活動起日

            tcHeader.Add(new TableHeaderCell());
            tcHeader[56].Text = BaseHelper.GetShowText("01_03070000_024");   //促銷活動-短期活動 - 活動迄日

            tcHeader.Add(new TableHeaderCell());
            tcHeader[57].Text = BaseHelper.GetShowText("01_03070000_045");   //促銷活動-短期活動 - 給點方式

            tcHeader.Add(new TableHeaderCell());
            tcHeader[58].Text = BaseHelper.GetShowText("01_03070000_046");   //促銷活動-短期活動 - AMT1

            tcHeader.Add(new TableHeaderCell());
            tcHeader[59].Text = BaseHelper.GetShowText("01_03070000_047");   //促銷活動-短期活動 - RATE1

            tcHeader.Add(new TableHeaderCell());
            tcHeader[60].Text = BaseHelper.GetShowText("01_03070000_048");   //促銷活動-短期活動 - AMT2

            tcHeader.Add(new TableHeaderCell());
            tcHeader[61].Text = BaseHelper.GetShowText("01_03070000_049");   //促銷活動-短期活動 - RATE2

            tcHeader.Add(new TableHeaderCell());
            tcHeader[62].Text = BaseHelper.GetShowText("01_03070000_050");   //促銷活動-短期活動 - AMT3

            tcHeader.Add(new TableHeaderCell());
            tcHeader[63].Text = BaseHelper.GetShowText("01_03070000_051");   //促銷活動-短期活動 - RATE3

            tcHeader.Add(new TableHeaderCell());
            tcHeader[64].Text = BaseHelper.GetShowText("01_03070000_052");   //促銷活動-短期活動 - AMT4

            tcHeader.Add(new TableHeaderCell());
            tcHeader[65].Text = BaseHelper.GetShowText("01_03070000_053");   //促銷活動-短期活動 - RATE4

            //促銷活動-生日活動
            tcHeader.Add(new TableHeaderCell());
            tcHeader[66].Text = BaseHelper.GetShowText("01_03070000_023");   //促銷活動-生日活動 - 活動起日

            tcHeader.Add(new TableHeaderCell());
            tcHeader[67].Text = BaseHelper.GetShowText("01_03070000_024");   //促銷活動-生日活動 - 活動迄日

            tcHeader.Add(new TableHeaderCell());
            tcHeader[68].Text = BaseHelper.GetShowText("01_03070000_045");   //促銷活動-生日活動 - 給點方式

            tcHeader.Add(new TableHeaderCell());
            tcHeader[69].Text = BaseHelper.GetShowText("01_03070000_046");   //促銷活動-生日活動 - AMT1

            tcHeader.Add(new TableHeaderCell());
            tcHeader[70].Text = BaseHelper.GetShowText("01_03070000_047");   //促銷活動-生日活動 - RATE1

            tcHeader.Add(new TableHeaderCell());
            tcHeader[71].Text = BaseHelper.GetShowText("01_03070000_048");   //促銷活動-生日活動 - AMT2

            tcHeader.Add(new TableHeaderCell());
            tcHeader[72].Text = BaseHelper.GetShowText("01_03070000_049");   //促銷活動-生日活動 - RATE2

            tcHeader.Add(new TableHeaderCell());
            tcHeader[73].Text = BaseHelper.GetShowText("01_03070000_050");   //促銷活動-生日活動 - AMT3

            tcHeader.Add(new TableHeaderCell());
            tcHeader[74].Text = BaseHelper.GetShowText("01_03070000_051");   //促銷活動-生日活動 - RATE3

            tcHeader.Add(new TableHeaderCell());
            tcHeader[75].Text = BaseHelper.GetShowText("01_03070000_052");   //促銷活動-生日活動 - AMT4

            tcHeader.Add(new TableHeaderCell());
            tcHeader[76].Text = BaseHelper.GetShowText("01_03070000_053") + @"</th></tr><tr>";   //促銷活動-生日活動 - RATE4

            for (int i = 0; i < 77; i++)
            {
                tcHeader[i].CssClass = "Grid_Header";
                tcHeader[i].Wrap = false;
            }
            #endregion second row

        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbAward.Visible = true;
        this.gpList.Visible = true;
        this.gpList.CurrentPageIndex = 0;
        BindGridView();
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

    #endregion event

    #region method
    /// <summary>
    /// 綁定GridView數據源

    /// </summary>
    private void BindGridView()
    {
        int intTotolCount = 0;
        string strRECEIVENUMBER = "";
        string strReceiveDate = "";

        REF_Condition(ref strRECEIVENUMBER, ref strReceiveDate);

        gvpbAward.DataSource = BRAwardSet3270.GetDetailReport(strRECEIVENUMBER, strReceiveDate, gpList.CurrentPageIndex, gpList.PageSize, ref intTotolCount);
        gpList.RecordCount = intTotolCount;
        gvpbAward.DataBind();

        if (0 == intTotolCount)
        {
            gvpbAward.Visible = false;
        }
    }

    private void REF_Condition(ref string strRECEIVENUMBER, ref string strReceiveDate)
    {
        if ("" == txtReceiveDate.Text.Trim())
        {
            strReceiveDate = "%";
        }
        else
        {
            strReceiveDate = txtReceiveDate.Text.Trim();
        }

        if ("" == txtReceiveNumber.Text.Trim())
        {
            strRECEIVENUMBER = "%";
        }
        else
        {
            strRECEIVENUMBER = txtReceiveNumber.Text.Trim();
        }
    }

    /// <summary>
    /// 功能： Award 交易結果明細表
    /// 修改日期: 2020/12/07_Ares_Stanley-變更下載檔名
    /// </summary>
    protected void OutputExcel()
    {
        try
        {
            string strMsgID = "";
            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
            string strRECEIVENUMBER = "";
            string strReceiveDate = "";

            REF_Condition(ref strRECEIVENUMBER, ref strReceiveDate);

            if (!BRExcel_File.CreateExcelFile_DetailAward(strRECEIVENUMBER, strReceiveDate, ref strServerPathFile, ref strMsgID))
            {
                if (strMsgID != "")
                    base.strClientMsg += MessageHelper.GetMessage(strMsgID);
                else
                    base.strClientMsg += MessageHelper.GetMessage("01_03070000_001");
                return;
            }

            //* 將服務器端生成的文檔，下載到本地。
            string strFileName = (BaseHelper.GetShowText("01_03070000_057") + DateTime.Now.ToString("yyyyMMdd") + ".xls").Replace(" ","");
            //* 顯示提示訊息：匯出到Excel文檔資料成功
            this.Session["ServerFile"] = strServerPathFile;
            this.Session["ClientFile"] = strFileName;
            // string urlString = @"ClientMsgShow('" + MessageHelper.GetMessage("01_03070000_002") + "');";
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("01_03070000_002") + "' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);

        }
        catch (Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            base.strClientMsg += BaseHelper.ClientMsgShow("01_03070000_001");
        }
    }
    #endregion method
}
