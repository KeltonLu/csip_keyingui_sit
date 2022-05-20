//******************************************************************
//*  作    者：占偉林(James)
//*  功能說明：報表-特店維護經辦作業量統計
//*  創建日期：2009/11/17
//*  修改記錄：2021/01/18_Ares_Stanley-新增NPOI
//*<author>            <time>            <TaskID>                <desc>
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

public partial class P010304000001 : PageBase
{
    #region event

    /// 作者 占偉林
    /// 創建日期：2009/11/17
    /// 修改日期：2009/11/17 
    /// <summary>
    /// 畫面裝載時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //* 一次鍵檔
            this.radKey1.Text = BaseHelper.GetShowText("01_03040000_003");
            //* 二次鍵檔
            this.radKey2.Text = BaseHelper.GetShowText("01_03040000_004");
            //* 【區間】起
            this.dtpSearchStart.Text = DateTime.Now.ToString("yyyy/MM/dd");
            //* 【區間】迄
            this.dtpSearchEnd.Text = DateTime.Now.ToString("yyyy/MM/dd");
            //* 設置光標
            this.dtpSearchStart.Focus();
            //* 選中‘一次鍵檔’Radio Button
            this.radKey1.Checked = true;
        }
        base.strHostMsg += "";
        this.sbRegScript.Append("loadSetFocus();");
    }
    
    ///// 作者 占偉林
    ///// 創建日期：2009/11/17
    ///// 修改日期：2009/11/17 
    /// <summary>
    /// 點選畫面【列印】按鈕時的處理    /// 修改紀錄:2021/01/18_Ares_Stanley-變更報表產出為NPOI
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        OutputExcel();
    }
    /// <summary>
    /// 點選查詢時的處理
    /// 建立時間:2021/01/20_Ares_Stanley
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.gvbpShop.Visible = true;
        this.gpList.Visible = true;
        // 綁定GridView表頭訊息
        ShowControlsText();

        BindGridView();
    }
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }
    #endregion event
    #region method
    private void BindGridView()
    {
        int intTotalCount = 0;
        string strMsgID = "";
        string strType = "";
        if (this.radKey1.Checked == true)
        {
            strType = "1";
        }
        else
        {
            strType = "2";
        }
        this.gvbpShop.DataSource = BRShopUserRpt.SearchRPTData_Search(this.dtpSearchStart.Text, this.dtpSearchEnd.Text, strType, ref strMsgID, gpList.CurrentPageIndex, gpList.PageSize, ref intTotalCount);
        gpList.RecordCount = intTotalCount;
        gvbpShop.DataBind();
    }
    private void ShowControlsText()
    {
        this.gvbpShop.Columns.Clear();
        // 商店代號
        BoundField shop_id = new BoundField();
        shop_id.HeaderText = BaseHelper.GetShowText("01_03040000_007");
        shop_id.DataField = "shop_id";
        // 商店名稱
        BoundField shop_name = new BoundField();
        shop_name.HeaderText = BaseHelper.GetShowText("01_03040000_008");
        shop_name.DataField = "shop_name";
        // 經辦姓名
        BoundField user_name = new BoundField();
        user_name.HeaderText = BaseHelper.GetShowText("01_03040000_009");
        user_name.DataField = "user_name";
        // 作業日期
        BoundField write_date = new BoundField();
        write_date.HeaderText = BaseHelper.GetShowText("01_03040000_010");
        write_date.DataField = "write_date";
        // 複合欄位
        BoundField mdate = new BoundField();
        if (this.radKey1.Checked == true)
        {
            // 1key 銀行名稱 / 分行名稱 / 戶 名 / 檢 碼 / 帳 號 / 解約代號 / 解約日期 / 解約原因碼
            mdate.HeaderText = BaseHelper.GetShowText("01_03040000_011");
            mdate.DataField = "mdate_1key";
        }
        else
        {
            // 2key 異動時間 / 異動欄位 / 異動前內容 / 異動後內容
            mdate.HeaderText = BaseHelper.GetShowText("01_03040000_012");
            mdate.DataField = "mdate_2key";
        }
        this.gvbpShop.Columns.Add(shop_id);
        this.gvbpShop.Columns.Add(shop_name);
        this.gvbpShop.Columns.Add(user_name);
        this.gvbpShop.Columns.Add(write_date);
        this.gvbpShop.Columns.Add(mdate);
        #region 設定欄寬
        this.gvbpShop.Columns[0].HeaderStyle.Width = new Unit(10, UnitType.Percentage);
        this.gvbpShop.Columns[0].ItemStyle.Width = new Unit(10, UnitType.Percentage);
        this.gvbpShop.Columns[1].HeaderStyle.Width = new Unit(15, UnitType.Percentage);
        this.gvbpShop.Columns[1].ItemStyle.Width = new Unit(15, UnitType.Percentage);
        this.gvbpShop.Columns[2].HeaderStyle.Width = new Unit(10, UnitType.Percentage);
        this.gvbpShop.Columns[2].ItemStyle.Width = new Unit(10, UnitType.Percentage);
        this.gvbpShop.Columns[3].HeaderStyle.Width = new Unit(10, UnitType.Percentage);
        this.gvbpShop.Columns[3].ItemStyle.Width = new Unit(10, UnitType.Percentage);
        this.gvbpShop.Columns[4].HeaderStyle.Width = new Unit(55, UnitType.Percentage);
        this.gvbpShop.Columns[4].ItemStyle.Width = new Unit(55, UnitType.Percentage);
        #endregion
        for (int i = 0; i < this.gvbpShop.Columns.Count; i++)
        {
            this.gvbpShop.Columns[i].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            this.gvbpShop.Columns[i].ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            this.gvbpShop.Columns[i].HeaderStyle.CssClass = "whiteSpaceNormal";
            this.gvbpShop.Columns[i].ItemStyle.CssClass = "whiteSpaceNormal";
        }
    }
    #endregion method
    #region function
    /// <summary>
    /// 特店維護經辦作業量統計_報表產出-NPOI; 2021/02/02_Ares_Stanley-報表調整
    /// 建立時間:2021/01/18_Ares_Stanley
    /// 修改紀錄:2021/02/18_Ares_Stanley-調整mdate寬度, 調整報表小計; 2021/03/08_Ares_Stanley-調整合計欄位合併儲存格
    /// </summary>
    protected void OutputExcel()
    {
        try
        {
            StringBuilder sbRegScriptF = new StringBuilder("");
            this.Title = BaseHelper.GetShowText("01_03040000_001");
            string strSearchStart = "";
            string strSearchEnd = "";
            string strType = "";
            string strMsgID = "";
            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());

            # region 從畫面取值
            //* 區間起日期沒有輸入
            if (!string.IsNullOrEmpty(this.dtpSearchStart.Text))
            {
                //* 區間起
                strSearchStart = this.dtpSearchStart.Text;
            }
            else
            {
                sbRegScriptF.Append("alert('" + MessageHelper.GetMessage("01_03040000_001") + "');");
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
                sbRegScriptF.Append("alert('" + MessageHelper.GetMessage("01_03040000_001") + "');");
                sbRegScriptF.Append("window.close();");
                this.sbRegScript.Append(sbRegScriptF.ToString());
                return;
            }

            //* 一次鍵檔/二次鍵檔
            if (this.radKey1.Checked)
            {
                strType = "1";
            }
            else if (this.radKey2.Checked)
            {
                strType = "2";
            }
            else
            {
                strType = "";
            }
            #endregion 從畫面取值

            DataTable dtblSearchResult = BRShopUserRpt.SearchRPTData(strSearchStart, strSearchEnd, strType, ref strMsgID);
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
                    string strRPTPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "shop.xls";
                    FileStream fs = new FileStream(strRPTPathFile, FileMode.Open);
                    HSSFWorkbook wb = new HSSFWorkbook(fs);
                    ISheet sheet = wb.GetSheet("工作表1");
                    int groupCount = 1;
                    int totalCount = 0;
                    int rowStart = 7;
                    string individualSum = "SUMPRODUCT(1/COUNTIF(A{0}:A{1},A{0}:A{1}))";
                    string totalSum = "SUM(F{0}:F{1})";

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

                    #region 表格內容格式-靠左對齊
                    HSSFCellStyle contentFormat_left = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
                    contentFormat_left.VerticalAlignment = VerticalAlignment.Center; //垂直置中
                    contentFormat_left.Alignment = HorizontalAlignment.Left; //水平置左
                    contentFormat_left.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##0"); //將儲存格內容設定為文字
                    contentFormat_left.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin; // 儲存格框線
                    contentFormat_left.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    contentFormat_left.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    contentFormat_left.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

                    HSSFFont contentFont_left = (HSSFFont)wb.CreateFont(); //建立文字樣式
                    contentFont_left.FontHeightInPoints = 12; //字體大小
                    contentFont_left.FontName = "新細明體"; //字型
                    contentFormat_left.SetFont(contentFont_left); //設定儲存格的文字樣式
                    #endregion

                    #region 表頭
                    for (int row = 1; row < 4; row++)
                    {
                        sheet.GetRow(row).GetCell(0).CellStyle = titleFormat;
                    }
                    sheet.GetRow(1).GetCell(0).SetCellValue("起迄日期：" + strSearchStart.Replace("/", "") + "-" + strSearchEnd.Replace("/", ""));
                    sheet.GetRow(2).GetCell(0).SetCellValue("列印經辦：" + ((EntityAGENT_INFO)System.Web.HttpContext.Current.Session["Agent"]).agent_name);
                    sheet.GetRow(3).GetCell(0).SetCellValue("列印日期：" + DateTime.Now.ToString("yyyyMMdd"));
                    // 標題/mdate
                    if (strType == "1")
                    {
                        sheet.GetRow(0).GetCell(0).SetCellValue("特店維護經辦工作量統計（一次鍵檔）");
                        sheet.GetRow(5).GetCell(4).SetCellValue("銀行名稱 / 分行名稱 / 戶 名 / 檢 碼 / 帳 號 / 解約代號 / 解約日期 / 解約原因碼");
                    }
                    else if (strType == "2")
                    {
                        sheet.GetRow(0).GetCell(0).SetCellValue("特店維護經辦工作量統計（二次鍵檔）");
                        sheet.GetRow(5).GetCell(4).SetCellValue("異動時間 / 異動欄位 / 異動前內容 / 異動後內容");
                    }
                    else
                    {
                        return;
                    }

                    #endregion 表頭

                    #region 表身
                    // 建立第一筆資料列
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int col = 0; col < 6; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = contentFormat;
                    }
                    // 商店代號
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue(dtblSearchResult.Rows[0]["shop_id"].ToString());
                    // 商店名稱
                    sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(dtblSearchResult.Rows[0]["shop_name"].ToString());
                    // 經辦姓名
                    sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue(dtblSearchResult.Rows[0]["user_name"].ToString());
                    // 作業日期
                    sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(dtblSearchResult.Rows[0]["write_date"].ToString());
                    // @mdate
                    if (strType == "1")
                    {
                        sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(dtblSearchResult.Rows[0]["bank_name"].ToString() + " / " + dtblSearchResult.Rows[0]["branch_name"].ToString() + " / " + dtblSearchResult.Rows[0]["account"].ToString() + " / " + dtblSearchResult.Rows[0]["check_num"].ToString() + " / " + dtblSearchResult.Rows[0]["account1"].ToString() + " / " + dtblSearchResult.Rows[0]["account2"].ToString() + " / " + dtblSearchResult.Rows[0]["cancel_code1"].ToString() + " / " + dtblSearchResult.Rows[0]["cancel_date"].ToString() + " / " + dtblSearchResult.Rows[0]["cancel_code2"].ToString());
                    }
                    else
                    {
                        sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(dtblSearchResult.Rows[0]["mod_time"].ToString() + " / " + dtblSearchResult.Rows[0]["field_name"].ToString() + " / " + dtblSearchResult.Rows[0]["before"].ToString() + " / " + dtblSearchResult.Rows[0]["after"].ToString());
                    }
                    // 合計
                    //sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue(dtblSearchResult.Rows[0]["num"].ToString());
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue("");

                    // 其餘資料
                    for (int i = 1; i < dtblSearchResult.Rows.Count; i++)
                    {
                        // 建立新資料列
                        sheet.CreateRow(sheet.LastRowNum + 1);
                        for (int col = 0; col < 6; col++)
                        {
                            sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = contentFormat;
                        }
                        // 同群組資料
                        if (dtblSearchResult.Rows[i]["user_name"].ToString() == dtblSearchResult.Rows[i - 1]["user_name"].ToString())
                        {
                            if(dtblSearchResult.Rows[i]["shop_id"].ToString() != dtblSearchResult.Rows[i - 1]["shop_id"].ToString())
                            {
                                groupCount += 1;
                            }
                            // 商店代號
                            sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue(dtblSearchResult.Rows[i]["shop_id"].ToString());
                            // 商店名稱
                            sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(dtblSearchResult.Rows[i]["shop_name"].ToString());
                            // 經辦姓名
                            sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue(dtblSearchResult.Rows[i]["user_name"].ToString());
                            // 作業日期
                            sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(dtblSearchResult.Rows[i]["write_date"].ToString());
                            // @mdate
                            if (strType == "1")
                            {
                                sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(dtblSearchResult.Rows[i]["bank_name"].ToString() + " / " + dtblSearchResult.Rows[i]["branch_name"].ToString() + " / " + dtblSearchResult.Rows[i]["account"].ToString() + " / " + dtblSearchResult.Rows[i]["check_num"].ToString() + " / " + dtblSearchResult.Rows[i]["account1"].ToString() + " / " + dtblSearchResult.Rows[i]["account2"].ToString() + " / " + dtblSearchResult.Rows[i]["cancel_code1"].ToString() + " / " + dtblSearchResult.Rows[i]["cancel_date"].ToString() + " / " + dtblSearchResult.Rows[i]["cancel_code2"].ToString());
                            }
                            else
                            {
                                sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(dtblSearchResult.Rows[i]["mod_time"].ToString() + " / " + dtblSearchResult.Rows[i]["field_name"].ToString() + " / " + dtblSearchResult.Rows[i]["before"].ToString() + " / " + dtblSearchResult.Rows[i]["after"].ToString());
                            }
                            //@mdate寬度
                            if (strType == "1")
                            {
                                sheet.SetColumnWidth(4, 80 * 256);
                            }else
                            {
                                sheet.SetColumnWidth(4, 50 * 256);
                            }
                            // 合計
                            //sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue(dtblSearchResult.Rows[i]["num"].ToString());
                            sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue("");
                        }
                        else //不同群組資料
                        {
                            for (int col = 0; col < 5; col++)
                            {
                                sheet.GetRow(sheet.LastRowNum).GetCell(col).CellStyle = contentFormat_left;
                            }
                            sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("個人合計：");
                            sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellType(CellType.Formula);
                            sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellFormula(string.Format(individualSum, rowStart, sheet.LastRowNum));
                            sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 0, 4));
                            totalCount += groupCount;
                            rowStart = sheet.LastRowNum + 2;
                            // 建立新資料列
                            sheet.CreateRow(sheet.LastRowNum + 1);
                            for (int col = 0; col < 6; col++)
                            {
                                sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = contentFormat;
                            }
                            // 商店代號
                            sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue(dtblSearchResult.Rows[i]["shop_id"].ToString());
                            // 商店名稱
                            sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(dtblSearchResult.Rows[i]["shop_name"].ToString());
                            // 經辦姓名
                            sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue(dtblSearchResult.Rows[i]["user_name"].ToString());
                            // 作業日期
                            sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(dtblSearchResult.Rows[i]["write_date"].ToString());
                            // @mdate
                            if (strType == "1")
                            {
                                sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(dtblSearchResult.Rows[i]["bank_name"].ToString() + " / " + dtblSearchResult.Rows[i]["branch_name"].ToString() + " / " + dtblSearchResult.Rows[i]["account"].ToString() + " / " + dtblSearchResult.Rows[i]["check_num"].ToString() + " / " + dtblSearchResult.Rows[i]["account1"].ToString() + " / " + dtblSearchResult.Rows[i]["account2"].ToString() + " / " + dtblSearchResult.Rows[i]["cancel_code1"].ToString() + " / " + dtblSearchResult.Rows[i]["cancel_date"].ToString() + " / " + dtblSearchResult.Rows[i]["cancel_code2"].ToString());
                            }
                            else
                            {
                                sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(dtblSearchResult.Rows[i]["mod_time"].ToString() + " / " + dtblSearchResult.Rows[i]["field_name"].ToString() + " / " + dtblSearchResult.Rows[i]["before"].ToString() + " / " + dtblSearchResult.Rows[i]["after"].ToString());
                            }
                            // 合計
                            //sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue(dtblSearchResult.Rows[i]["num"].ToString());
                            sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue("");
                        }
                    }
                    for (int row = 5; row < sheet.LastRowNum + 1; row++)
                    {
                        sheet.GetRow(row).GetCell(4).CellStyle = contentFormat_left;
                    }
                    #endregion 表身

                    #region 表尾
                    // 建立最後一個個人合計資料列
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int col = 0; col < 6; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = contentFormat;
                    }
                    for (int col = 0; col < 5; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = contentFormat_left;
                    }
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("個人合計：");
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellFormula(string.Format(individualSum, rowStart, sheet.LastRowNum));
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 0, 4));

                    // 建立全體合計資料列
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int col = 0; col < 6; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = contentFormat;
                    }
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("全體合計：");
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellFormula(string.Format(totalSum, 7, sheet.LastRowNum));
                    for (int col = 0; col < 5; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).GetCell(col).CellStyle = contentFormat_left;
                    }
                    //合併儲存格
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 0, 4));
                    // 主管/組長/經辦
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int col = 0; col < 6; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = titleFormat;
                    }
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("主管：　　　　　　　組長：　　　　　　　經辦：");
                    #endregion 表尾

                    //公式預計算
                    //HSSFFormulaEvaluator.EvaluateAllFormulaCells(wb);

                    //* 保存文件到程序运行目录下
                    strServerPathFile = strServerPathFile + @"\Shop_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    FileStream fs1 = new FileStream(strServerPathFile, FileMode.Create);
                    wb.Write(fs1);
                    fs1.Close();
                    fs.Close();

                    base.strHostMsg += "";
                    base.strClientMsg += "";

                    //* 顯示提示訊息：匯出到Excel文檔資料成功
                    string strFileName = "";
                    if (strType == "1")
                    {
                        strFileName = "特店維護經辦作業量統計(一次鍵檔).xls";
                    }
                    else if(strType == "2")
                    {
                        strFileName = "特店維護經辦作業量統計(二次鍵檔).xls";
                    }
                    else
                    {
                        return;
                    }
                    this.Session["ServerFile"] = strServerPathFile;
                    this.Session["ClientFile"] = strFileName;
                    string urlString = @"location.href='DownLoadFile.aspx';";
                    base.sbRegScript.Append(urlString);
                }
                else
                {
                    strMsgID = "01_03040000_003";
                    sbRegScriptF.Append("alert('" + MessageHelper.GetMessage(strMsgID) + "');");
                    sbRegScriptF.Append("window.close();");
                    this.sbRegScript.Append(sbRegScriptF.ToString());
                }
            }

        }
        catch (Exception exp)
        {
            StringBuilder sbRegScriptF = new StringBuilder("");
            BRCompareRpt.SaveLog(exp);
            sbRegScriptF.Append("alert('" + MessageHelper.GetMessage("01_03040000_004") + "');");
            sbRegScriptF.Append("window.close();");
            this.sbRegScript.Append(sbRegScriptF.ToString());
        }
    }
    #endregion
}
