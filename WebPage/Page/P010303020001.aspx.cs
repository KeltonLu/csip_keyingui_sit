// 修改紀錄: 2021/01/18_Ares_Stanley_新增NPOI
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
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.HSSF.EventUserModel.DummyRecord;
using NPOI.XSSF.UserModel.Charts;

public partial class P010303020001 : PageBase
{
    #region event

    /// 創建日期：2012/06/25
    /// 修改日期：2009/06/25
    /// <summary>
    /// 畫面裝載時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ShowControlsText();
        if (!IsPostBack)
        {
            //* 【區間】起
            this.dtpSearchStart.Text = DateTime.Now.ToString("yyyy/MM/dd");
            //* 【區間】迄
            this.dtpSearchEnd.Text = DateTime.Now.ToString("yyyy/MM/dd");
            //* 設置光標
            this.dtpSearchStart.Focus();
        }
        base.strHostMsg += "";
        this.sbRegScript.Append("loadSetFocus();");
    }


    /// 創建日期：2012/06/25
    /// 修改日期：2009/06/25; 2021/01/18_Ares_Stanley-變更報表產出方式為NPOI
    /// <summary>
    /// 點選畫面【列印】按鈕時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        OutputExcel();
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.gvpbnewworkReqAppro.Visible = true;
        this.gpList.Visible = true;
        ShowControlsText();
        BindGridView();
    }
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }
    #endregion event
    #region method
    /// <summary>
    /// 建立時間:2021/01/21_Ares_Stanley
    /// 修改紀錄:2021/02/01_Ares_Stanley-移除請款加批、延伸性作業/請款加批、6001會員附加服務
    /// 綁定查詢資料表頭
    /// </summary>
    private void ShowControlsText()
    {
        this.gvpbnewworkReqAppro.Columns[0].HeaderText =BaseHelper.GetShowText("01_03030000_010");
        this.gvpbnewworkReqAppro.Columns[1].HeaderText =BaseHelper.GetShowText("01_03030000_036");
        this.gvpbnewworkReqAppro.Columns[2].HeaderText =BaseHelper.GetShowText("01_03030000_037");
        this.gvpbnewworkReqAppro.Columns[3].HeaderText =BaseHelper.GetShowText("01_03030000_038");
        this.gvpbnewworkReqAppro.Columns[4].HeaderText =BaseHelper.GetShowText("01_03030000_039");
        this.gvpbnewworkReqAppro.Columns[5].HeaderText =BaseHelper.GetShowText("01_03030000_040");
        this.gvpbnewworkReqAppro.Columns[6].HeaderText =BaseHelper.GetShowText("01_03030000_041");
        this.gvpbnewworkReqAppro.Columns[7].HeaderText =BaseHelper.GetShowText("01_03030000_042");
        this.gvpbnewworkReqAppro.Columns[8].HeaderText =BaseHelper.GetShowText("01_03030000_043");
        this.gvpbnewworkReqAppro.Columns[9].HeaderText =BaseHelper.GetShowText("01_03030000_044");
        this.gvpbnewworkReqAppro.Columns[10].HeaderText =BaseHelper.GetShowText("01_03030000_045");
        this.gvpbnewworkReqAppro.Columns[11].HeaderText =BaseHelper.GetShowText("01_03030000_052");
        this.gvpbnewworkReqAppro.Columns[12].HeaderText =BaseHelper.GetShowText("01_03030000_053");
        this.gvpbnewworkReqAppro.Columns[13].HeaderText =BaseHelper.GetShowText("01_03030000_056");
        this.gvpbnewworkReqAppro.Columns[14].HeaderText =BaseHelper.GetShowText("01_03030000_057");
        this.gvpbnewworkReqAppro.Columns[15].HeaderText =BaseHelper.GetShowText("01_03030000_046");
        this.gvpbnewworkReqAppro.Columns[16].HeaderText =BaseHelper.GetShowText("01_03030000_047");
    }
    /// <summary>
    /// 建立時間:2021/01/21_Ares_Stanley
    /// </summary>
    private void BindGridView()
    {
        int intTotalCount = 0;
        string strMsgID = "";
        gvpbnewworkReqAppro.DataSource = BRReqApproStatisticsRpt.SearchRPTData_Search(this.dtpSearchStart.Text.Trim(), this.dtpSearchEnd.Text.Trim(), ref strMsgID, gpList.CurrentPageIndex, gpList.PageSize, ref intTotalCount);
        gpList.RecordCount = intTotalCount;
        gvpbnewworkReqAppro.DataBind();
        //for (int i = 0; i < this.gvpbnewworkReqAppro.Columns.Count; i++)
        //{
        //    this.gvpbnewworkReqAppro.Columns[i].HeaderStyle.CssClass = "whiteSpaceNormal";
        //    this.gvpbnewworkReqAppro.Columns[i].ItemStyle.CssClass = "whiteSpaceNormal";
        //}
    }
    #endregion method
    #region function
    /// <summary>
    /// 建立時間:2021/01/18_Ares_Stanley
    /// 產出NPOI報表
    /// 修改紀錄:2021/01/21_Ares_Stanley-調整合計; 2021/02/01_Ares_Stanley-移除請款加批、延伸性作業/請款加批、6001會員附加服務
    /// </summary>
    protected void OutputExcel()
    {
        string parseErrorMsg = "";
        try
        {
            StringBuilder sbRegScriptF = new StringBuilder("");
            this.Title = BaseHelper.GetShowText("01_03030000_001");
            string strSearchStart = "";
            string strSearchEnd = "";
            string strMsgID = "";
            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
            Int32 parseResult = 0;
            bool parseStatus = false;

            #region 從畫面取值
            //* 區間起日期沒有輸入
            if (!string.IsNullOrEmpty(this.dtpSearchStart.Text))
            {
                //* 區間起
                strSearchStart = this.dtpSearchStart.Text;
            }
            else
            {
                sbRegScriptF.Append("alert('" + MessageHelper.GetMessage("01_03030000_001") + "');");
                sbRegScriptF.Append("window.close();");
                this.sbRegScript.Append(sbRegScriptF.ToString());
                return;
            }

            //* 區間迄日期沒有輸入
            if (!string.IsNullOrEmpty(this.dtpSearchEnd.Text))
            {
                //* 區間迄
                strSearchEnd = this.dtpSearchEnd.Text;
            }
            else
            {
                sbRegScriptF.Append("alert('" + MessageHelper.GetMessage("01_03030000_001") + "');");
                sbRegScriptF.Append("window.close();");
                this.sbRegScript.Append(sbRegScriptF.ToString());
                return;
            }
            #endregion

            DataTable dtblSearchResult = BRReqApproStatisticsRpt.SearchRPTData(strSearchStart, strSearchEnd, ref strMsgID);
            BRExcel_File.CheckDirectory(ref strServerPathFile);

            if (null == dtblSearchResult)
            {
                //* 取報表數據不成功
                sbRegScriptF.Append("alert('" + MessageHelper.GetMessage(strMsgID) + "');");
                sbRegScriptF.Append("window.close();");
                this.sbRegScript.Append(sbRegScriptF.ToString());
            }
            else
            {
                if (dtblSearchResult.Rows.Count > 0)
                {
                    string strRPTPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "newworkReqAppro_list.xls";
                    FileStream fs = new FileStream(strRPTPathFile, FileMode.Open);
                    HSSFWorkbook wb = new HSSFWorkbook(fs);
                    ISheet sheet = wb.GetSheet("工作表1");

                    #region 資料靠左對齊
                    HSSFCellStyle titleFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
                    titleFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
                    titleFormat.Alignment = HorizontalAlignment.Left; //水平置中
                    titleFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字

                    HSSFFont titleFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                    titleFont.FontHeightInPoints = 12; //字體大小
                    titleFont.FontName = "新細明體"; //字型
                    titleFormat.SetFont(titleFont); //設定儲存格的文字樣式
                    #endregion 資料靠左對齊

                    #region 表格內容格式
                    HSSFCellStyle contentFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
                    contentFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
                    contentFormat.Alignment = HorizontalAlignment.Center; //水平置中
                    contentFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##0"); //將儲存格內容設定為文字
                    contentFormat.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin; // 儲存格框線
                    contentFormat.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    contentFormat.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    contentFormat.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

                    HSSFFont contentFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                    contentFont.FontHeightInPoints = 12; //字體大小
                    contentFont.FontName = "新細明體"; //字型
                    contentFormat.SetFont(contentFont); //設定儲存格的文字樣式
                    #endregion 表格內容格式

                    #region 表頭
                    //* 起訖日期
                    sheet.GetRow(1).CreateCell(1).SetCellValue(strSearchStart.Replace("/", "") + "-" + strSearchEnd.Replace("/", ""));
                    sheet.GetRow(1).GetCell(1).CellStyle = titleFormat;
                    //* 列印經辦
                    sheet.GetRow(2).CreateCell(1).SetCellValue(((EntityAGENT_INFO)System.Web.HttpContext.Current.Session["Agent"]).agent_name);
                    sheet.GetRow(2).GetCell(1).CellStyle = titleFormat;
                    //* 列印日期
                    sheet.GetRow(3).CreateCell(1).SetCellValue(DateTime.Now.ToString("yyyyMMdd"));
                    sheet.GetRow(3).GetCell(1).CellStyle = titleFormat;
                    #endregion 表頭

                    for (int i = 0; i < dtblSearchResult.Rows.Count; i++)
                    {
                        int sum_1key_row = 0;
                        int sum_2key_row = 0;

                        #region 表格內容
                        // 建立新列
                        sheet.CreateRow(sheet.LastRowNum + 1);
                        // 建立儲存格
                        for (int col = 0; col < 17; col++)
                        {
                            sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = contentFormat;
                        }
                        // 經辦姓名
                        sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue(dtblSearchResult.Rows[i]["user_name"].ToString());
                        // 資料異動1key
                        sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(dtblSearchResult.Rows[i]["D01"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(1).StringCellValue))
                        {
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(1).StringCellValue.ToString(), out parseResult);
                            if (!parseStatus)
                                parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D01");
                            sum_1key_row += parseResult;
                        }
                        // 資料異動2key
                        sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue(dtblSearchResult.Rows[i]["D02"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(2).StringCellValue))
                        {
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(2).StringCellValue.ToString(), out parseResult);
                            if (!parseStatus)
                                parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D02");
                            sum_2key_row += parseResult;
                        }
                        // 費率1key
                        sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(dtblSearchResult.Rows[i]["D03"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(3).StringCellValue))
                        {
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(3).StringCellValue.ToString(), out parseResult);
                            if (!parseStatus)
                                parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D03");
                            sum_1key_row += parseResult;
                        }
                        // 費率2key
                        sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(dtblSearchResult.Rows[i]["D04"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(4).StringCellValue))
                        {
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(4).StringCellValue.ToString(), out parseResult);
                            if (!parseStatus)
                                parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D04");
                            sum_2key_row += parseResult;
                        }
                        // 帳號1key
                        sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue(dtblSearchResult.Rows[i]["D05"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(5).StringCellValue))
                        {
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(5).StringCellValue.ToString(), out parseResult);
                            if (!parseStatus)
                                parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D05");
                            sum_1key_row += parseResult;
                        }
                        // 帳號2key
                        sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellValue(dtblSearchResult.Rows[i]["D06"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(6).StringCellValue))
                        {
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(6).StringCellValue.ToString(), out parseResult);
                            if (!parseStatus)
                                parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D06");
                            sum_2key_row += parseResult;
                        }
                        // 解約1key
                        sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(dtblSearchResult.Rows[i]["D07"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(7).StringCellValue))
                        {
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(7).StringCellValue.ToString(), out parseResult);
                            if (!parseStatus)
                                parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D07");
                            sum_1key_row += parseResult;
                        }
                        // 解約2key
                        sheet.GetRow(sheet.LastRowNum).GetCell(8).SetCellValue(dtblSearchResult.Rows[i]["D08"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(8).StringCellValue))
                        {
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(8).StringCellValue.ToString(), out parseResult);
                            if (!parseStatus)
                                parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D08");
                            sum_2key_row += parseResult;
                        }
                        // 機器1key
                        sheet.GetRow(sheet.LastRowNum).GetCell(9).SetCellValue(dtblSearchResult.Rows[i]["D09"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(9).StringCellValue))
                        {
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(9).StringCellValue.ToString(), out parseResult);
                            if (!parseStatus)
                                parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D09");
                            sum_1key_row += parseResult;
                        }
                        // 機器2key
                        sheet.GetRow(sheet.LastRowNum).GetCell(10).SetCellValue(dtblSearchResult.Rows[i]["D10"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(10).StringCellValue))
                        {
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(10).StringCellValue.ToString(), out parseResult);
                            if (!parseStatus)
                                parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D10");
                            sum_2key_row += parseResult;
                        }
                        // 6001 1key
                        sheet.GetRow(sheet.LastRowNum).GetCell(11).SetCellValue(dtblSearchResult.Rows[i]["D11"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(11).StringCellValue))
                        {
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(11).StringCellValue.ToString(), out parseResult);
                            if (!parseStatus)
                                parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D11");
                            sum_1key_row += parseResult;
                        }
                        // 6001 2key
                        sheet.GetRow(sheet.LastRowNum).GetCell(12).SetCellValue(dtblSearchResult.Rows[i]["D12"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(12).StringCellValue))
                        {
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(12).StringCellValue.ToString(), out parseResult);
                            if (!parseStatus)
                                parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D12");
                            sum_2key_row += parseResult;
                        }
                        // PCAM1key
                        sheet.GetRow(sheet.LastRowNum).GetCell(13).SetCellValue(dtblSearchResult.Rows[i]["D13"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(13).StringCellValue))
                        {
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(13).StringCellValue.ToString(), out parseResult);
                            if (!parseStatus)
                                parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D13");
                            sum_1key_row += parseResult;
                        }
                        // PCAM2key
                        sheet.GetRow(sheet.LastRowNum).GetCell(14).SetCellValue(dtblSearchResult.Rows[i]["D14"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(14).StringCellValue))
                        {
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(14).StringCellValue.ToString(), out parseResult);
                            if (!parseStatus)
                                parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D14");
                            sum_2key_row += parseResult;
                        }
                        // 合計1key
                        sheet.GetRow(sheet.LastRowNum).GetCell(15).SetCellValue(sum_1key_row.ToString()); //20210611_Ares_Stanley移除ToString千分位
                        // 合計2key
                        sheet.GetRow(sheet.LastRowNum).GetCell(16).SetCellValue(sum_2key_row.ToString()); //20210611_Ares_Stanley移除ToString千分位
                        #endregion 表格內容
                    }

                    #region 表尾
                    string common_formula = "SUM({0}10:{0}{1})";
                    // 轉換資料為數值型態
                    for (int row = 9; row < sheet.LastRowNum + 1; row++)
                    {
                        for (int col = 1; col < 17; col++)
                        {
                            if (!string.IsNullOrEmpty(sheet.GetRow(row).GetCell(col).StringCellValue.ToString()))
                            {
                                parseStatus = Int32.TryParse(sheet.GetRow(row).GetCell(col).StringCellValue, out parseResult);
                                if (!parseStatus)
                                    parseErrorMsg += string.Format("資料轉換為數值型態失敗，請檢查第 {0} 列第 {1} 欄。", row + 1, col + 1);
                                sheet.GetRow(row).GetCell(col).SetCellValue(parseResult);
                            }
                            else
                            {
                                sheet.GetRow(row).GetCell(col).SetCellValue(0);
                            }
                        }
                    }
                    // 建立合計列1key
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int col = 0; col < 17; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = contentFormat;
                    }
                    // 合計列1key
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("合計一KEY");
                    // 資料異動1key
                    sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellFormula(string.Format(common_formula, "B", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 1, 2));
                    // 費率1key
                    sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellFormula(string.Format(common_formula, "D", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 3, 4));
                    // 帳號1key
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellFormula(string.Format(common_formula, "F", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 5, 6));
                    // 解約1key
                    sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellFormula(string.Format(common_formula, "H", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 7, 8));
                    // 機器1key
                    sheet.GetRow(sheet.LastRowNum).GetCell(9).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(9).SetCellFormula(string.Format(common_formula, "J", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 9, 10));
                    // 6001 1key
                    sheet.GetRow(sheet.LastRowNum).GetCell(11).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(11).SetCellFormula(string.Format(common_formula, "L", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 11, 12));
                    // PCAM 1key
                    sheet.GetRow(sheet.LastRowNum).GetCell(13).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(13).SetCellFormula(string.Format(common_formula, "N", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 13, 14));
                    //合計1key
                    //sheet.GetRow(sheet.LastRowNum).GetCell(21).SetCellType(CellType.Formula);
                    //sheet.GetRow(sheet.LastRowNum).GetCell(21).SetCellFormula(string.Format(common_formula, "V", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 15, 16));

                    // 建立合計列2key
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int col = 0; col < 17; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = contentFormat;
                    }
                    // 合計列2key //2021/04/07_Ares_Stanley-修正錯字
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("合計二KEY");
                    // 資料異動2key
                    sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellFormula(string.Format(common_formula, "C", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 1, 2));
                    // 費率2key
                    sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellFormula(string.Format(common_formula, "E", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 3, 4));
                    // 帳號2key
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellFormula(string.Format(common_formula, "G", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 5, 6));
                    // 解約2key
                    sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellFormula(string.Format(common_formula, "I", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 7, 8));
                    // 機器2key
                    sheet.GetRow(sheet.LastRowNum).GetCell(9).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(9).SetCellFormula(string.Format(common_formula, "K", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 9, 10));
                    // 6001 2key
                    sheet.GetRow(sheet.LastRowNum).GetCell(11).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(11).SetCellFormula(string.Format(common_formula, "M", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 11, 12));
                    // PCAM 2key
                    sheet.GetRow(sheet.LastRowNum).GetCell(13).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(13).SetCellFormula(string.Format(common_formula, "O", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 13, 14));
                    //合計2key
                    //sheet.GetRow(sheet.LastRowNum).GetCell(21).SetCellType(CellType.Formula);
                    //sheet.GetRow(sheet.LastRowNum).GetCell(21).SetCellFormula(string.Format(common_formula, "W", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 15, 16));



                    // 建立主管/組長/經辦列
                    sheet.CreateRow(sheet.LastRowNum + 2);
                    for (int col = 0; col < 17; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = titleFormat;
                    }
                    // 主管
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("主管：");
                    // 組長
                    sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellValue("組長：");
                    // 經辦
                    sheet.GetRow(sheet.LastRowNum).GetCell(12).SetCellValue("經辦：");
                    #endregion 表尾

                    //2021/03/09_Ares_Stanley-報表公式預計算
                    HSSFFormulaEvaluator.EvaluateAllFormulaCells(wb);

                    //* 保存文件到程序运行目录下
                    strServerPathFile = strServerPathFile + @"\NewWorkReqAppro_List_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    FileStream fs1 = new FileStream(strServerPathFile, FileMode.Create);
                    wb.Write(fs1);
                    fs1.Close();
                    fs.Close();

                    base.strHostMsg += "";
                    base.strClientMsg += "";

                    //* 顯示提示訊息：匯出到Excel文檔資料成功
                    string strFileName = "經辦作業量統計(特店資料).xls";
                    this.Session["ServerFile"] = strServerPathFile;
                    this.Session["ClientFile"] = strFileName;
                    string urlString = @"location.href='DownLoadFile.aspx';";
                    base.sbRegScript.Append(urlString);
                }
                else
                {
                    strMsgID = "01_03030000_003";
                    sbRegScriptF.Append("alert('" + MessageHelper.GetMessage(strMsgID) + "');");
                    sbRegScriptF.Append("window.close();");
                    this.sbRegScript.Append(sbRegScriptF.ToString());
                }

            }
        }
        catch ( Exception exp)
        {
            StringBuilder sbRegScriptF = new StringBuilder("");
            BRCompareRpt.SaveLog(exp);
            sbRegScriptF.Append("alert('" + MessageHelper.GetMessage("01_03030000_004") + "');");
            sbRegScriptF.Append("window.close();");
            this.sbRegScript.Append(sbRegScriptF.ToString());
        }
    }

    #endregion function
}
