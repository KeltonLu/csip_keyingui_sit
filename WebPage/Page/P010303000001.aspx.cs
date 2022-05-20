//******************************************************************
//*  作    者：占偉林(James)
//*  功能說明：報表-經辦作業量統計
//*  創建日期：2009/11/12
//*  修改記錄：2021/01/15_Ares_Stanley-新增NPOI

//*<author>            <time>            <TaskID>                <desc>
//Area_Luke            2021/01/21        20200031-CSIP EOS      新增GridView查詢功能
//*******************************************************************
using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Text;
using CSIPKeyInGUI.BusinessRules;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Message;
using Framework.Common.Logging;
using Framework.Common.Utility;
using System.IO;
using Framework.Common.JavaScript;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

public partial class P010303000001 : PageBase
{
    #region event

    /// 作者 占偉林
    /// 創建日期：2009/11/12
    /// 修改日期：2009/11/12 
    /// <summary>
    /// 畫面裝載時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
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

        SetGridViewTitle();
    }
    
    ///// 作者 占偉林
    ///// 創建日期：2009/11/12
    ///// 修改日期：2009/11/12 
    /// <summary>
    /// 點選畫面【列印】按鈕時的處理
    /// 修改紀錄:2021/01/15_Ares_Stanley-變更報表產出方式為NPOI
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        OutputExcel();
    }
    #endregion event
    #region function
    /// <summary>
    /// 經辦作業量統計報表產出-NPOI
    /// 創建日期:2021/01/15_Ares_Stanley
    /// 修改日期:2021/01/18_Ares_Stanley-增加catch, 空值檢核
    /// </summary>
    protected void OutputExcel()
    {
        string parseErrorMsg = "";
        try
        {
            StringBuilder sbRegScriptF = new StringBuilder("");
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
            #endregion 從畫面取值

            // 查詢報表資料
            int count = 0;
            DataTable dtblSearchResult = BRUserStatisticsRpt.SearchRPTData(strSearchStart, strSearchEnd, ref strMsgID,ref count);
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
                    string strRPTPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "newwork_list.xls";
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
                        for (int col = 0; col < 38; col++)
                        {
                            sheet.GetRow(sheet.LastRowNum).CreateCell(col);
                            sheet.GetRow(sheet.LastRowNum).GetCell(col).CellStyle = contentFormat;
                        }
                        // 經辦姓名
                        sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue(dtblSearchResult.Rows[i]["user_name"].ToString());
                        // 地址
                        sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(dtblSearchResult.Rows[i]["A01"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(1).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(1).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "A01");
                        sum_2key_row += parseResult;
                        // 姓名生日
                        sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue(dtblSearchResult.Rows[i]["A04"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(2).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(2).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "A04");
                        sum_2key_row += parseResult;
                        // 其他
                        sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(dtblSearchResult.Rows[i]["A11"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(3).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(3).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "A11");
                        sum_2key_row += parseResult;
                        // 族群碼
                        sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(dtblSearchResult.Rows[i]["A06"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(4).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(4).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "A06");
                        sum_2key_row += parseResult;
                        // 他行自扣1key
                        sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue(dtblSearchResult.Rows[i]["A13"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(5).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(5).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "A13");
                        sum_1key_row += parseResult;
                        // 他行自扣2key
                        sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellValue(dtblSearchResult.Rows[i]["A14"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(6).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(6).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "A14");
                        sum_2key_row += parseResult;
                        // 中信及郵局1key
                        sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(dtblSearchResult.Rows[i]["A15"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(7).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(7).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "A15");
                        sum_1key_row += parseResult;
                        // 中信及郵局2key
                        sheet.GetRow(sheet.LastRowNum).GetCell(8).SetCellValue(dtblSearchResult.Rows[i]["A16"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(8).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(8).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "A16");
                        sum_2key_row += parseResult;
                        // 訊息/更正單1key
                        sheet.GetRow(sheet.LastRowNum).GetCell(9).SetCellValue(dtblSearchResult.Rows[i]["A17"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(9).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(9).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "A17");
                        sum_1key_row += parseResult;
                        // 訊息/更正單2key
                        sheet.GetRow(sheet.LastRowNum).GetCell(10).SetCellValue(dtblSearchResult.Rows[i]["A18"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(10).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(10).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "A18");
                        sum_2key_row += parseResult;
                        // 註銷
                        sheet.GetRow(sheet.LastRowNum).GetCell(11).SetCellValue(dtblSearchResult.Rows[i]["B01"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(11).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(11).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "B01");
                        sum_2key_row += parseResult;
                        // 狀況碼
                        sheet.GetRow(sheet.LastRowNum).GetCell(12).SetCellValue(dtblSearchResult.Rows[i]["B05"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(12).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(12).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "B05");
                        sum_2key_row += parseResult;
                        // 優惠碼
                        sheet.GetRow(sheet.LastRowNum).GetCell(13).SetCellValue(dtblSearchResult.Rows[i]["B04"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(13).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(13).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "B04");
                        sum_2key_row += parseResult;
                        // 繳款異動
                        sheet.GetRow(sheet.LastRowNum).GetCell(14).SetCellValue(dtblSearchResult.Rows[i]["B02"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(14).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(14).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "B02");
                        sum_2key_row += parseResult;
                        // 繳款評等
                        sheet.GetRow(sheet.LastRowNum).GetCell(15).SetCellValue(dtblSearchResult.Rows[i]["B17"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(15).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(15).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "B17");
                        sum_2key_row += parseResult;
                        // 毀補轉一卡通1key
                        sheet.GetRow(sheet.LastRowNum).GetCell(16).SetCellValue(dtblSearchResult.Rows[i]["B03"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(16).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(16).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "B03");
                        sum_1key_row += parseResult;
                        // 毀補轉一卡通2key
                        sheet.GetRow(sheet.LastRowNum).GetCell(17).SetCellValue(dtblSearchResult.Rows[i]["B18"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(17).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(17).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "B18");
                        sum_2key_row += parseResult;
                        // 毀補1key
                        sheet.GetRow(sheet.LastRowNum).GetCell(18).SetCellValue(dtblSearchResult.Rows[i]["B09"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(18).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(18).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "B09");
                        sum_1key_row += parseResult;
                        // 毀補2key
                        sheet.GetRow(sheet.LastRowNum).GetCell(19).SetCellValue(dtblSearchResult.Rows[i]["B10"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(19).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(19).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "B10");
                        sum_2key_row += parseResult;
                        // 掛補1key
                        sheet.GetRow(sheet.LastRowNum).GetCell(20).SetCellValue(dtblSearchResult.Rows[i]["B13"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(20).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(20).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "B13");
                        sum_1key_row += parseResult;
                        // 掛補2key
                        sheet.GetRow(sheet.LastRowNum).GetCell(21).SetCellValue(dtblSearchResult.Rows[i]["B14"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(21).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(21).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "B14");
                        sum_2key_row += parseResult;
                        // 新增異動1key
                        sheet.GetRow(sheet.LastRowNum).GetCell(22).SetCellValue(dtblSearchResult.Rows[i]["B11"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(22).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(22).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "B11");
                        sum_1key_row += parseResult;
                        // 新增異動2key
                        sheet.GetRow(sheet.LastRowNum).GetCell(23).SetCellValue(dtblSearchResult.Rows[i]["B12"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(23).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(23).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "B12");
                        sum_2key_row += parseResult;
                        // 解除管制1key
                        sheet.GetRow(sheet.LastRowNum).GetCell(24).SetCellValue(dtblSearchResult.Rows[i]["B15"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(24).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(24).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "B15");
                        sum_1key_row += parseResult;
                        // 解除管制2key
                        sheet.GetRow(sheet.LastRowNum).GetCell(25).SetCellValue(dtblSearchResult.Rows[i]["B16"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(25).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(25).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "B16");
                        sum_2key_row += parseResult;
                        // 資料異動1key
                        sheet.GetRow(sheet.LastRowNum).GetCell(26).SetCellValue(dtblSearchResult.Rows[i]["D01"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(26).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(26).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D01");
                        sum_1key_row += parseResult;
                        // 資料異動2key
                        sheet.GetRow(sheet.LastRowNum).GetCell(27).SetCellValue(dtblSearchResult.Rows[i]["D02"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(27).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(27).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D02");
                        sum_2key_row += parseResult;
                        // 費率1key
                        sheet.GetRow(sheet.LastRowNum).GetCell(28).SetCellValue(dtblSearchResult.Rows[i]["D03"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(28).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(28).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D03");
                        sum_1key_row += parseResult;
                        // 費率2key
                        sheet.GetRow(sheet.LastRowNum).GetCell(29).SetCellValue(dtblSearchResult.Rows[i]["D04"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(29).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(29).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D04");
                        sum_2key_row += parseResult;
                        // 帳號1key
                        sheet.GetRow(sheet.LastRowNum).GetCell(30).SetCellValue(dtblSearchResult.Rows[i]["D05"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(30).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(30).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D05");
                        sum_1key_row += parseResult;
                        // 帳號2key
                        sheet.GetRow(sheet.LastRowNum).GetCell(31).SetCellValue(dtblSearchResult.Rows[i]["D06"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(31).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(31).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D06");
                        sum_2key_row += parseResult;
                        // 解約1key
                        sheet.GetRow(sheet.LastRowNum).GetCell(32).SetCellValue(dtblSearchResult.Rows[i]["D07"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(32).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(32).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D07");
                        sum_1key_row += parseResult;
                        // 解約2key
                        sheet.GetRow(sheet.LastRowNum).GetCell(33).SetCellValue(dtblSearchResult.Rows[i]["D08"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(33).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(33).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D08");
                        sum_2key_row += parseResult;
                        // 機器1key
                        sheet.GetRow(sheet.LastRowNum).GetCell(34).SetCellValue(dtblSearchResult.Rows[i]["D09"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(34).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(34).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D09");
                        sum_1key_row += parseResult;
                        // 機器2key
                        sheet.GetRow(sheet.LastRowNum).GetCell(35).SetCellValue(dtblSearchResult.Rows[i]["D10"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(35).StringCellValue))
                            parseStatus = Int32.TryParse(sheet.GetRow(sheet.LastRowNum).GetCell(35).StringCellValue.ToString(), out parseResult);
                        if (!parseStatus)
                            parseErrorMsg += string.Format("第 {0} 筆資料，{1} 欄位資料轉換失敗，請檢查。", i + 1, "D10");
                        sum_2key_row += parseResult;
                        // 合計1key (加總所有1key)
                        sheet.GetRow(sheet.LastRowNum).GetCell(36).SetCellValue(sum_1key_row.ToString()); //20210611_Ares_Stanley移除ToString千分位
                        // 合計2key (加總所有2key跟其他)
                        sheet.GetRow(sheet.LastRowNum).GetCell(37).SetCellValue(sum_2key_row.ToString()); //20210611_Ares_Stanley移除ToString千分位
                        #endregion 表格內容
                    }

                    #region 表尾
                    string common_formula = "SUM({0}10:{0}{1})";
                    // 轉換資料為數值型態
                    for (int row = 9; row < sheet.LastRowNum + 1; row++)
                    {
                        for (int col = 1; col < 38; col++)
                        {
                            if(!string.IsNullOrEmpty(sheet.GetRow(row).GetCell(col).StringCellValue.ToString()))
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

                    // 建立空白列
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int col = 0; col < 38; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = contentFormat;
                    }

                    // 建立合計列1key
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int col = 0; col < 38; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = contentFormat;
                    }
                    // 合計列1key
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("合計一KEY");
                    // 他行自扣1key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellFormula(string.Format(common_formula, "F", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 5, 6));
                    // 中信及郵局1key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellFormula(string.Format(common_formula, "H", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 7, 8));
                    // 訊息/更正單1key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(9).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(9).SetCellFormula(string.Format(common_formula, "J", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 9, 10));
                    // 毀補轉一卡通1key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(16).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(16).SetCellFormula(string.Format(common_formula, "Q", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 16, 17));
                    //毀補1key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(18).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(18).SetCellFormula(string.Format(common_formula, "S", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 18, 19));
                    // 掛補1key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(20).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(20).SetCellFormula(string.Format(common_formula, "U", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 20, 21));
                    // 新增異動1key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(22).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(22).SetCellFormula(string.Format(common_formula, "W", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 22, 23));
                    // 解除管制1key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(24).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(24).SetCellFormula(string.Format(common_formula, "Y", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 24, 25));
                    // 資料異動1key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(26).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(26).SetCellFormula(string.Format(common_formula, "AA", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 26, 27));
                    // 費率1key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(28).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(28).SetCellFormula(string.Format(common_formula, "AC", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 28, 29));
                    // 帳號1key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(30).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(30).SetCellFormula(string.Format(common_formula, "AE", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 30, 31));
                    // 解約1key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(32).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(32).SetCellFormula(string.Format(common_formula, "AG", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 32, 33));
                    // 機器1key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(34).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(34).SetCellFormula(string.Format(common_formula, "AI", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 34, 35));
                    // 合計1key儲存格合併
                    //sheet.GetRow(sheet.LastRowNum).GetCell(36).SetCellType(CellType.Formula);
                    //sheet.GetRow(sheet.LastRowNum).GetCell(36).SetCellFormula(string.Format(common_formula, "AK", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 36, 37));

                    // 建立合計列2key
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int col = 0; col < 38; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = contentFormat;
                    }
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("合計二KEY");
                    // 地址欄總和
                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(1).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(1).SetCellFormula(string.Format(common_formula, "B", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 1, sheet.LastRowNum, 1, 1));
                    // 姓名生日欄總和
                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(2).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(2).SetCellFormula(string.Format(common_formula, "C", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 1, sheet.LastRowNum, 2, 2));
                    // 其他欄總和
                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(3).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(3).SetCellFormula(string.Format(common_formula, "D", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 1, sheet.LastRowNum, 3, 3));
                    // 族群碼欄總和
                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(4).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(4).SetCellFormula(string.Format(common_formula, "E", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 1, sheet.LastRowNum, 4, 4));
                    // 他行自扣2key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellFormula(string.Format(common_formula, "G", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 5, 6));
                    // 中信及郵局2key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellFormula(string.Format(common_formula, "I", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 7, 8));
                    // 訊息/更正單2key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(9).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(9).SetCellFormula(string.Format(common_formula, "K", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 9, 10));
                    // 註銷欄總和
                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(11).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(11).SetCellFormula(string.Format(common_formula, "L", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 1, sheet.LastRowNum, 11, 11));
                    // 狀況碼欄總和
                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(12).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(12).SetCellFormula(string.Format(common_formula, "M", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 1, sheet.LastRowNum, 12, 12));
                    // 優惠碼欄總和
                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(13).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(13).SetCellFormula(string.Format(common_formula, "N", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 1, sheet.LastRowNum, 13, 13));
                    // 繳款異動欄總和
                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(14).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(14).SetCellFormula(string.Format(common_formula, "O", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 1, sheet.LastRowNum, 14, 14));
                    // 繳款評等欄總和
                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(15).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(15).SetCellFormula(string.Format(common_formula, "P", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 1, sheet.LastRowNum, 15, 15));
                    // 毀補轉一卡通2key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(16).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(16).SetCellFormula(string.Format(common_formula, "R", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 16, 17));
                    // 毀補2key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(18).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(18).SetCellFormula(string.Format(common_formula, "T", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 18, 19));
                    // 掛補2key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(20).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(20).SetCellFormula(string.Format(common_formula, "V", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 20, 21));
                    // 新增異動2key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(22).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(22).SetCellFormula(string.Format(common_formula, "X", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 22, 23));
                    // 解除管制2key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(24).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(24).SetCellFormula(string.Format(common_formula, "Z", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 24, 25));
                    // 資料異動2key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(26).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(26).SetCellFormula(string.Format(common_formula, "AB", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 26, 27));
                    // 費率2key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(28).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(28).SetCellFormula(string.Format(common_formula, "AD", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 28, 29));
                    // 帳號2key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(30).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(30).SetCellFormula(string.Format(common_formula, "AF", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 30, 31));
                    // 解約2key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(32).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(32).SetCellFormula(string.Format(common_formula, "AH", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 32, 33));
                    // 機器2key總和
                    sheet.GetRow(sheet.LastRowNum).GetCell(34).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(34).SetCellFormula(string.Format(common_formula, "AJ", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 34, 35));
                    // 合計2key儲存格合併
                    //sheet.GetRow(sheet.LastRowNum).GetCell(36).SetCellType(CellType.Formula);
                    //sheet.GetRow(sheet.LastRowNum).GetCell(36).SetCellFormula(string.Format(common_formula, "AL", dtblSearchResult.Rows.Count + 9));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 36, 37));

                    // 建立主管/組長/經辦列
                    sheet.CreateRow(sheet.LastRowNum + 2);
                    for (int col = 0; col < 38; col++)
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
                    strServerPathFile = strServerPathFile + @"\NewWork_List_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    FileStream fs1 = new FileStream(strServerPathFile, FileMode.Create);
                    wb.Write(fs1);
                    fs1.Close();
                    fs.Close();

                    base.strHostMsg += "";
                    base.strClientMsg += "";

                    //* 顯示提示訊息：匯出到Excel文檔資料成功
                    string strFileName = "經辦作業量統計.xls";
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
        catch(Exception exp)
        {
            StringBuilder sbRegScriptF = new StringBuilder("");
            BRCompareRpt.SaveLog(exp);
            sbRegScriptF.Append("alert('" + MessageHelper.GetMessage("01_03030000_004") + "');");
            sbRegScriptF.Append("window.close();");
            this.sbRegScript.Append(sbRegScriptF.ToString());
        }
        finally
        {
            if (!string.IsNullOrEmpty(parseErrorMsg))
            {
                Logging.Log(parseErrorMsg, LogLayer.UI);
            }
        }

    }

    #region 共用NPOI
    /// <summary>
    /// 修改紀錄:2021/01/15_Ares_Stanley-新增共用NPOI
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="wb"></param>
    /// <param name="start"></param>
    /// <param name="sheetName"></param>
    private static void ExportExcelForNPOI(DataTable dt, ref HSSFWorkbook wb, Int32 start, String sheetName, int cellStart = 0)
    {
        try
        {
            HSSFCellStyle cs = (HSSFCellStyle)wb.CreateCellStyle();
            cs.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            cs.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            cs.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            cs.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            //啟動多行文字
            cs.WrapText = true;
            //文字置中
            cs.VerticalAlignment = VerticalAlignment.Center;
            cs.Alignment = HorizontalAlignment.Center;
            //將儲存格內容設定為文字
            cs.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");

            HSSFFont font1 = (HSSFFont)wb.CreateFont();
            //字體尺寸
            font1.FontHeightInPoints = 12;
            font1.FontName = "新細明體";
            cs.SetFont(font1);

            if (dt != null && dt.Rows.Count != 0)
            {
                int count = start;
                ISheet sheet = wb.GetSheet(sheetName);
                int cols = dt.Columns.Count;
                foreach (DataRow dr in dt.Rows)
                {
                    int cell = cellStart;
                    IRow row = (IRow)sheet.CreateRow(count);
                    row.CreateCell(0).SetCellValue(count.ToString());
                    for (int i = 0; i < cols; i++)
                    {
                        row.CreateCell(cell).SetCellValue(dr[i].ToString());
                        row.GetCell(cell).CellStyle = cs;
                        cell++;
                    }
                    count++;
                }
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            throw;
        }
    }
    #endregion

    #endregion function



    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:業務新增查詢切換頁需求功能
    /// 作    者:Ares Luke
    /// 創建時間:2021/01/21
    /// </summary>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:查詢事件
    /// 作    者:Ares Luke
    /// 創建時間:2021/01/21
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        BindGridView();
    }


    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:綁定GridView
    /// 作    者:Ares Luke
    /// 創建時間:2021/01/21
    /// </summary>
    private void BindGridView()
    {
        string strSearchStart = "";
        string strSearchEnd = "";

        if (chkCond( ref strSearchStart, ref strSearchEnd))
        {
            try
            {
                DataTable dt = new DataTable();
                Int32 count = 0;
                Boolean result = BRUserStatisticsRpt.SearchGripData(strSearchStart, strSearchEnd, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref count, ref dt);
                //* 查詢成功
                if (result)
                {
                    DtStatistics(ref dt);

                    this.gpList.Visible = true;
                    this.gpList.RecordCount = count;
                    this.grvUserView.Visible = true;
                    this.grvUserView.DataSource = dt;
                    this.grvUserView.DataBind();
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("01_03030000_005"));
                }
                //* 查詢不成功
                else
                {
                    this.gpList.RecordCount = 0;
                    this.grvUserView.DataSource = null;
                    this.grvUserView.DataBind();
                    this.gpList.Visible = false;
                    this.grvUserView.Visible = false;
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("01_03030000_006"));
                }
            }
            catch (Exception exp)
            {
                Logging.Log(exp, LogLayer.UI);
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("01_03100000_012"));

            }

        }
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:檢核條件並回傳查詢值
    /// 作    者:Ares Luke
    /// 創建時間:2021/01/21
    /// </summary>
    /// <param name="strSearchStart"></param>
    /// <param name="strSearchEnd"></param>
    /// <returns></returns>
    private Boolean chkCond(ref string strSearchStart, ref string strSearchEnd)
    {
        string strMsgId = string.Empty;
        if (!CheckCondition(ref strMsgId))
        {
            MessageHelper.ShowMessage(this, strMsgId);
            return false;
        }
        try
        {
            //* 區間起
            strSearchStart = this.dtpSearchStart.Text.Trim();
            //* 區間迄
            strSearchEnd = this.dtpSearchEnd.Text.Trim();
        }
        catch (Exception exp)
        {
            Logging.Log(exp);
            MessageHelper.ShowMessage(this, "01_03030000_006");
            return false;
        }

        return true;
    }


    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:檢核條件
    /// 作    者:Ares Luke
    /// 創建時間:2021/01/21
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    private bool CheckCondition(ref string strMsgID)
    {

        //* 區間起日期沒有輸入
        if (string.IsNullOrEmpty(this.dtpSearchStart.Text))
        {
            strMsgID = "01_03030000_001";
            dtpSearchStart.Focus();
            return false;
        }

        //* 區間迄日期沒有輸入
        if (string.IsNullOrEmpty(this.dtpSearchEnd.Text))
        {
            strMsgID = "01_03030000_001";
            dtpSearchEnd.Focus();
            return false;
        }
        return true;
    }


    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:設置動態GridView標題
    /// 作    者:Ares Luke
    /// 創建時間:2021/01/21
    /// </summary>
    private void SetGridViewTitle()
    {
        this.grvUserView.Columns.Clear();

        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_010"), DataField = "user_name" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_011"), DataField = "A01" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_012"), DataField = "A04" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_013"), DataField = "A11" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_014"), DataField = "A06" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_015"), DataField = "A13" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_016"), DataField = "A14" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_017"), DataField = "A15" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_018"), DataField = "A16" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_019"), DataField = "A17" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_020"), DataField = "A18" });

        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_021"), DataField = "B01" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_022"), DataField = "B05" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_023"), DataField = "B04" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_024"), DataField = "B02" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_025"), DataField = "B17" });

        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_026"), DataField = "B03" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_027"), DataField = "B18" });

        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_028"), DataField = "B09" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_029"), DataField = "B10" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_030"), DataField = "B13" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_031"), DataField = "B14" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_032"), DataField = "B11" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_033"), DataField = "B12" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_034"), DataField = "B15" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_035"), DataField = "B16" });

        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_036"), DataField = "D01" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_037"), DataField = "D02" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_038"), DataField = "D03" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_039"), DataField = "D04" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_040"), DataField = "D05" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_041"), DataField = "D06" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_042"), DataField = "D07" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_043"), DataField = "D08" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_044"), DataField = "D09" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_045"), DataField = "D10" });

        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_046"), DataField = "TOTAL1" });
        this.grvUserView.Columns.Add(new BoundField { HeaderText = BaseHelper.GetShowText("01_03030000_047"), DataField = "TOTAL2" });

        for (int i = 0; i < this.grvUserView.Columns.Count; i++)
        {
            this.grvUserView.Columns[i].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            this.grvUserView.Columns[i].ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        }
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:查詢結果統計功能
    /// 作    者:Ares Luke
    /// 創建時間:2021/01/21 
    /// </summary>
    private void DtStatistics(ref DataTable dt)
    {
        dt.Columns.Add("TOTAL1");
        dt.Columns.Add("TOTAL2");

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            int sum_1key_row = 0;
            int sum_2key_row = 0;


            // 地址
            sum_2key_row += int.Parse(dt.Rows[i]["A01"].ToString());
            // 姓名生日
            sum_2key_row += int.Parse(dt.Rows[i]["A04"].ToString());
            // 其他
            sum_2key_row += int.Parse(dt.Rows[i]["A11"].ToString());
            // 族群碼
            sum_2key_row += int.Parse(dt.Rows[i]["A06"].ToString());
            // 他行自扣1key
            sum_1key_row += int.Parse(dt.Rows[i]["A13"].ToString());
            // 他行自扣2key
            sum_2key_row += int.Parse(dt.Rows[i]["A14"].ToString());
            // 中信及郵局1key
            sum_1key_row += int.Parse(dt.Rows[i]["A15"].ToString());
            // 中信及郵局2key
            sum_2key_row += int.Parse(dt.Rows[i]["A16"].ToString());
            // 訊息/更正單1key
            sum_1key_row += int.Parse(dt.Rows[i]["A17"].ToString());
            // 訊息/更正單2key
            sum_2key_row += int.Parse(dt.Rows[i]["A18"].ToString());
            // 註銷
            sum_2key_row += int.Parse(dt.Rows[i]["B01"].ToString());
            // 狀況碼
            sum_2key_row += int.Parse(dt.Rows[i]["B05"].ToString());
            // 優惠碼
            sum_2key_row += int.Parse(dt.Rows[i]["B04"].ToString());
            // 繳款異動
            sum_2key_row += int.Parse(dt.Rows[i]["B02"].ToString());
            // 繳款評等
            sum_2key_row += int.Parse(dt.Rows[i]["B17"].ToString());
            // 毀補轉一卡通1key
            sum_1key_row += int.Parse(dt.Rows[i]["B03"].ToString());
            // 毀補轉一卡通2key
            sum_2key_row += int.Parse(dt.Rows[i]["B18"].ToString());
            // 毀補1key
            sum_1key_row += int.Parse(dt.Rows[i]["B09"].ToString());
            // 毀補2key
            sum_2key_row += int.Parse(dt.Rows[i]["B10"].ToString());
            // 掛補1key
            sum_1key_row += int.Parse(dt.Rows[i]["B13"].ToString());
            // 掛補2key
            sum_2key_row += int.Parse(dt.Rows[i]["B14"].ToString());
            // 新增異動1key
            sum_1key_row += int.Parse(dt.Rows[i]["B11"].ToString());
            // 新增異動2key
            sum_2key_row += int.Parse(dt.Rows[i]["B12"].ToString());
            // 解除管制1key
            sum_1key_row += int.Parse(dt.Rows[i]["B15"].ToString());
            // 解除管制2key
            sum_2key_row += int.Parse(dt.Rows[i]["B16"].ToString());
            // 資料異動1key
            sum_1key_row += int.Parse(dt.Rows[i]["D01"].ToString());
            // 資料異動2key
            sum_2key_row += int.Parse(dt.Rows[i]["D02"].ToString());
            // 費率1key
            sum_1key_row += int.Parse(dt.Rows[i]["D03"].ToString());
            // 費率2key
            sum_2key_row += int.Parse(dt.Rows[i]["D04"].ToString());
            // 帳號1key
            sum_1key_row += int.Parse(dt.Rows[i]["D05"].ToString());
            // 帳號2key
            sum_2key_row += int.Parse(dt.Rows[i]["D06"].ToString());
            // 解約1key
            sum_1key_row += int.Parse(dt.Rows[i]["D07"].ToString());
            // 解約2key
            sum_2key_row += int.Parse(dt.Rows[i]["D08"].ToString());
            // 機器1key
            sum_1key_row += int.Parse(dt.Rows[i]["D09"].ToString());
            // 機器2key
            sum_2key_row += int.Parse(dt.Rows[i]["D10"].ToString());


            dt.Rows[i]["TOTAL1"] = sum_1key_row;
            dt.Rows[i]["TOTAL2"] = sum_2key_row;

        }
    }
}
