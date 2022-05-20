//******************************************************************
//*  作    者：
//*  功能說明：報表-人工簽單
//*  創建日期：2014/07/31
//*  修改記錄：2021/01/12_Ares_Stanley-新增NPOI
//*<author>            <time>            <TaskID>                <desc>
//*Ares_Luke           2021/01/20        20200031-CSIP EOS       需求新增查詢GridView
//*Ares_Stanley        2021/04/22                                移除正確值欄位非金額時的千分位符號
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

public partial class P010310000001 : PageBase
{
    #region event

    /// <summary>
    /// 畫面裝載時的處理
    /// 修改紀錄: 2021/01/12_Ares_Stanley-增加ProperyName儲存
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
            //* 綁定報表種類DropDownList
            BindDropDownList();
            //* 設置光標
            this.dtpSearchStart.Focus();
        }

        if (!string.IsNullOrEmpty(this.dropProperty.SelectedItem.Text))
        {
            this.Session["PropertyName"] = this.dropProperty.SelectedItem.Text;
        }

        base.strHostMsg += "";
        this.sbRegScript.Append("loadSetFocus();");

        SetGridViewTitle();

    }

    /// <summary>
    /// 點選畫面【列印】按鈕時的處理
    /// 修改紀錄: 2021/01/12_Ares_Stanley-變更報表產出方式為NPOI; 2021/02/19_Ares_Stanley-列印報表後重新綁定數據
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        string strProperty = "";
        string strSearchStart = "";
        string strSearchEnd = "";

        if (chkCond(ref strProperty, ref strSearchStart,ref strSearchEnd))
        {
            OutputExcel(strProperty, strSearchStart, strSearchEnd);
        }
        //* 顯示報表
        this.Session["PropertyName"] = this.dropProperty.SelectedItem.Text;
        //重新綁定查詢數據
        BindGridView();
    }

    #endregion event

    #region function

    /// <summary>
    /// 綁定報表種類DropDownList控件
    /// </summary>
    private void BindDropDownList()
    {
        try
        {
            //* 讀取公共屬性
            DataTable dtblProperty = null;
            if (!BaseHelper.GetCommonProperty("01", "AS_REPORT", ref dtblProperty))
            {
                //* 顯示端末訊息
                base.strClientMsg += MessageHelper.GetMessage("01_03100000_001");
            }
            else
            {
                //* 將取得的【報表種類】訊息顯示到報表種類DropDownList控件
                this.dropProperty.Items.Clear();
                this.dropProperty.DataSource = dtblProperty;
                this.dropProperty.DataTextField = EntityM_PROPERTY_CODE.M_PROPERTY_NAME;
                this.dropProperty.DataValueField = EntityM_PROPERTY_CODE.M_PROPERTY;
                this.dropProperty.DataBind();
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("01_03100000_001");
        }
    }

    /// <summary>
    /// 創建日期:2021/01/12_Ares_Stanley
    /// 功能說明:人工簽單報表Excel產出-NPOI
    /// 修改紀錄:2021/01/26_Ares_Stanley-調整正確率計算; 2021/03/08_Ares_Stanley-查無資料時列印空報表; 2021/08/17_Ares_Stanley-卡號隱碼
    /// </summary>
    private void OutputExcel(string strProperty, string strSearchStart, string strSearchEnd)
    {
        StringBuilder sbRegScriptF = new StringBuilder("");
        this.Title = BaseHelper.GetShowText("01_03100000_001");
        string strPropertyName = this.Session["PropertyName"].ToString();
        string strMsgID = "";
        string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());

        // 檢查目錄，並刪除以前的文檔資料
        CSIPKeyInGUI.BusinessRules_new.BRExcel_File.CheckDirectory(ref strServerPathFile);

        //因DB格式為Datetime，結束日期需帶上時間才能取到當日資料
        //2021/03/19_Ares_Stanley-結束日期的時間改自欄位檢核時加入
        DataTable dtblSearchResult =
            BRASRpt.SearchRPTData(strProperty, strSearchStart, strSearchEnd, ref strMsgID);

        if (null == dtblSearchResult)
        {
            //* 取報表數據不成功
            sbRegScriptF.Append("alert('" + MessageHelper.GetMessage(strMsgID) + "');");
            sbRegScriptF.Append("window.close();");
            this.sbRegScript.Append(sbRegScriptF.ToString());
        }
        else if (dtblSearchResult.Rows.Count == 0)
        {
            sbRegScriptF.Append("alert('" + MessageHelper.GetMessage(strMsgID) + "');");
            sbRegScriptF.Append("window.close();");
            this.sbRegScript.Append(sbRegScriptF.ToString());
            if (chkCond(ref strProperty, ref strSearchStart, ref strSearchEnd))
            {
                OutputEmptyExcel(strProperty, strSearchStart, strSearchEnd);
            }
        }
        else if (dtblSearchResult.Rows.Count != 0)
        {
            if (dtblSearchResult.Rows.Count > 0)
            {
                string strRPTPathFile = "";
                strRPTPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") +
                                 strProperty + ".xls";
                FileStream fs = new FileStream(strRPTPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);

                ISheet sheet = wb.GetSheet("工作表1");
                int totalGeneralCount = 0;
                int totalInstCount = 0;
                int totalRow = 0;
                int totalErrorCount = 0;
                double percentRow = 0;

                int totalGeneralCount_key1 = 0;
                int totalGeneralCount_key2 = 0;
                int totalInstCount_key1 = 0;
                int totalInstCount_key2 = 0;

                #region 儲存格文字格式

                HSSFCellStyle contentFormat = (HSSFCellStyle) wb.CreateCellStyle(); //建立文字格式
                contentFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
                contentFormat.Alignment = HorizontalAlignment.Center; //水平置中
                contentFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字
                contentFormat.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin; // 儲存格框線
                contentFormat.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                contentFormat.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                contentFormat.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

                HSSFFont contentFont = (HSSFFont) wb.CreateFont(); //建立文字樣式
                contentFont.FontHeightInPoints = 12; //字體大小
                contentFont.FontName = "新細明體"; //字型
                //contentFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold; //粗體
                contentFormat.SetFont(contentFont); //設定儲存格的文字樣式

                HSSFCellStyle titleFormat = (HSSFCellStyle) wb.CreateCellStyle(); //建立文字格式
                titleFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
                titleFormat.Alignment = HorizontalAlignment.Left; //水平置中
                titleFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字

                HSSFFont titleFont = (HSSFFont) wb.CreateFont(); //建立文字樣式
                titleFont.FontHeightInPoints = 12; //字體大小
                titleFont.FontName = "新細明體"; //字型
                //titleFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold; //粗體
                titleFormat.SetFont(titleFont); //設定儲存格的文字樣式

                #endregion

                switch (strProperty)
                {
                    case "rptAS_Work":

                        #region 寫入資料-人工簽單作業量報表

                        for (int i = 0; i < dtblSearchResult.Rows.Count; i++)
                        {
                            int generalCount = 0;
                            int instCount = 0;
                            int errorCount = 0;
                            int totalColumn = 0;
                            double percentColumn = 0;

                            sheet.CreateRow(sheet.LastRowNum + 1);
                            // 鍵檔日期
                            if (!string.IsNullOrEmpty(dtblSearchResult.Rows[i]["KeyinDate"].ToString()))
                            {
                                sheet.GetRow(sheet.LastRowNum).CreateCell(0)
                                    .SetCellValue(dtblSearchResult.Rows[i]["KeyinDate"].ToString());
                                sheet.GetRow(sheet.LastRowNum).GetCell(0).CellStyle = contentFormat;
                            }
                            else
                            {
                                sheet.GetRow(sheet.LastRowNum).CreateCell(0).SetCellValue("");
                                sheet.GetRow(sheet.LastRowNum).GetCell(0).CellStyle = contentFormat;
                            }

                            // 請款簽單(一般簽單)
                            if (!string.IsNullOrEmpty(dtblSearchResult.Rows[i]["Adjust_Count_general"].ToString()))
                            {
                                sheet.GetRow(sheet.LastRowNum).CreateCell(1).SetCellValue(int
                                    .Parse(dtblSearchResult.Rows[i]["Adjust_Count_general"].ToString()).ToString("N0"));
                                sheet.GetRow(sheet.LastRowNum).GetCell(1).CellStyle = contentFormat;
                                generalCount += int.Parse(dtblSearchResult.Rows[i]["Adjust_Count_general"].ToString());
                                totalGeneralCount += generalCount;
                            }
                            else
                            {
                                sheet.GetRow(sheet.LastRowNum).CreateCell(1).SetCellValue(0);
                                sheet.GetRow(sheet.LastRowNum).GetCell(1).CellStyle = contentFormat;
                            }

                            // 分期訂購單(分期簽單)
                            if (!string.IsNullOrEmpty(dtblSearchResult.Rows[i]["Adjust_Count_inst"].ToString()))
                            {
                                sheet.GetRow(sheet.LastRowNum).CreateCell(2).SetCellValue(int
                                    .Parse(dtblSearchResult.Rows[i]["Adjust_Count_inst"].ToString()).ToString("N0"));
                                instCount += int.Parse(dtblSearchResult.Rows[i]["Adjust_Count_inst"].ToString());
                                totalInstCount += instCount;
                                sheet.GetRow(sheet.LastRowNum).GetCell(2).CellStyle = contentFormat;
                            }
                            else
                            {
                                sheet.GetRow(sheet.LastRowNum).CreateCell(2).SetCellValue(0);
                                sheet.GetRow(sheet.LastRowNum).GetCell(2).CellStyle = contentFormat;
                            }

                            // 合計欄
                            totalColumn = generalCount + instCount;
                            totalRow += totalColumn;
                            sheet.GetRow(sheet.LastRowNum).CreateCell(3).SetCellValue(totalColumn.ToString("N0"));
                            sheet.GetRow(sheet.LastRowNum).GetCell(3).CellStyle = contentFormat;
                            // 錯誤筆數
                            if (!string.IsNullOrEmpty(dtblSearchResult.Rows[i]["Err_Count"].ToString()))
                            {
                                sheet.GetRow(sheet.LastRowNum).CreateCell(4).SetCellValue(
                                    int.Parse(dtblSearchResult.Rows[i]["Err_Count"].ToString()).ToString("N0"));
                                errorCount += int.Parse(dtblSearchResult.Rows[i]["Err_Count"].ToString());
                                totalErrorCount += errorCount;
                                sheet.GetRow(sheet.LastRowNum).GetCell(4).CellStyle = contentFormat;
                            }
                            else
                            {
                                sheet.GetRow(sheet.LastRowNum).CreateCell(4).SetCellValue(0);
                                sheet.GetRow(sheet.LastRowNum).GetCell(4).CellStyle = contentFormat;
                            }

                            // 正確率
                            if (errorCount != 0 && totalColumn != 0)
                            {
                                percentColumn = Convert.ToDouble(errorCount) / Convert.ToDouble(totalColumn);
                            }
                            else if (errorCount != 0 && totalColumn == 0)
                            {
                                percentColumn = 0;
                            }
                            else if (errorCount == 0 && totalColumn != 0)
                            {
                                percentColumn = 1;
                            }

                            sheet.GetRow(sheet.LastRowNum).CreateCell(5).SetCellValue(percentColumn.ToString("0.00%"));
                            sheet.GetRow(sheet.LastRowNum).GetCell(5).CellStyle = contentFormat;
                            // 備註
                            sheet.GetRow(sheet.LastRowNum).CreateCell(6).SetCellValue("");
                            sheet.GetRow(sheet.LastRowNum).GetCell(6).CellStyle = contentFormat;
                        }

                        //* 起訖日期 OR 鍵檔日期
                        if (strProperty == "rptAS_ErrDetail" || strProperty == "rptAS_Capacity")
                        {
                            sheet.GetRow(3).GetCell(0).SetCellValue("鍵檔日期：");
                            sheet.GetRow(3).CreateCell(1)
                                .SetCellValue(strSearchStart.Replace("/", "") + "-" + strSearchEnd.Replace("/", ""));
                            sheet.GetRow(3).GetCell(1).CellStyle = titleFormat;
                        }
                        else
                        {
                            sheet.GetRow(3).GetCell(0).SetCellValue("起迄日期：");
                            sheet.GetRow(3).CreateCell(1)
                                .SetCellValue(strSearchStart.Replace("/", "") + "-" + strSearchEnd.Replace("/", ""));
                            sheet.GetRow(3).GetCell(1).CellStyle = titleFormat;
                        }

                        //* 列印經辦
                        sheet.GetRow(2).CreateCell(1)
                            .SetCellValue(((EntityAGENT_INFO) System.Web.HttpContext.Current.Session["Agent"])
                                .agent_name);
                        sheet.GetRow(2).GetCell(1).CellStyle = titleFormat;
                        //* 列印日期
                        sheet.GetRow(1).CreateCell(1).SetCellValue(DateTime.Now.ToString("yyyyMMdd"));
                        sheet.GetRow(1).GetCell(1).CellStyle = titleFormat;

                        sheet.CreateRow(sheet.LastRowNum + 1);
                        // 尾列合計
                        sheet.GetRow(sheet.LastRowNum).CreateCell(0).SetCellValue("合計");
                        sheet.GetRow(sheet.LastRowNum).GetCell(0).CellStyle = contentFormat;
                        // 請款簽單(一般簽單)
                        sheet.GetRow(sheet.LastRowNum).CreateCell(1).SetCellValue(totalGeneralCount.ToString("N0"));
                        sheet.GetRow(sheet.LastRowNum).GetCell(1).CellStyle = contentFormat;
                        // 分期訂購單(分期簽單)
                        sheet.GetRow(sheet.LastRowNum).CreateCell(2).SetCellValue(totalInstCount.ToString("N0"));
                        sheet.GetRow(sheet.LastRowNum).GetCell(2).CellStyle = contentFormat;
                        // 合計
                        sheet.GetRow(sheet.LastRowNum).CreateCell(3).SetCellValue(totalRow.ToString("N0"));
                        sheet.GetRow(sheet.LastRowNum).GetCell(3).CellStyle = contentFormat;
                        // 錯誤筆數
                        sheet.GetRow(sheet.LastRowNum).CreateCell(4).SetCellValue(totalErrorCount.ToString("N0"));
                        sheet.GetRow(sheet.LastRowNum).GetCell(4).CellStyle = contentFormat;
                        // 正確率
                        if (totalErrorCount != 0 && totalRow != 0)
                        {
                            percentRow = Convert.ToDouble(totalErrorCount) / Convert.ToDouble(totalRow);
                        }
                        else if (totalErrorCount != 0 && totalRow == 0)
                        {
                            percentRow = 0;
                        }
                        else if (totalErrorCount == 0 && totalRow != 0)
                        {
                            percentRow = 1;
                        }

                        sheet.GetRow(sheet.LastRowNum).CreateCell(5).SetCellValue(percentRow.ToString("0.00%"));
                        sheet.GetRow(sheet.LastRowNum).GetCell(5).CellStyle = contentFormat;
                        //備註
                        sheet.GetRow(sheet.LastRowNum).CreateCell(6).SetCellValue("");
                        sheet.GetRow(sheet.LastRowNum).GetCell(6).CellStyle = contentFormat;

                        // 表尾說明
                        sheet.CreateRow(sheet.LastRowNum + 2);
                        sheet.GetRow(sheet.LastRowNum).CreateCell(0).SetCellValue("主管：");
                        sheet.GetRow(sheet.LastRowNum).CreateCell(5).SetCellValue("製表：");
                        sheet.CreateRow(sheet.LastRowNum + 2).CreateCell(0).SetCellValue("說明：");
                        sheet.CreateRow(sheet.LastRowNum + 1).CreateCell(0)
                            .SetCellValue("1.此「請款簽單正確率一覽表」包含分期訂購單及請款簽單二類人工簽單鍵檔之正確性合併計算。");
                        sheet.CreateRow(sheet.LastRowNum + 1).CreateCell(0)
                            .SetCellValue("2.每月平均鍵檔正確率標準需於99.95%以上(含99.95%)。");

                        #endregion

                        break;
                    case "rptAS_ErrDetail":

                        #region 寫入資料-鍵檔錯誤明細

                        ExportExcelForNPOI(dtblSearchResult, ref wb, 6, "工作表1", 1);
                        // 新增序號欄
                        for (int row = 6; row < sheet.LastRowNum + 1; row++)
                        {
                            sheet.GetRow(row).CreateCell(0).SetCellValue((row - 5).ToString("N0"));
                            sheet.GetRow(row).GetCell(0).CellStyle = contentFormat;
                        }

                        // 變更金額、正確值資料格式
                        for (int row = 6; row < sheet.LastRowNum + 1; row++)
                        {
                            sheet.GetRow(row).GetCell(9)
                                .SetCellValue(int.Parse(sheet.GetRow(row).GetCell(9).StringCellValue).ToString("N0"));
                        }

                        for (int row = 6; row < sheet.LastRowNum + 1; row++)
                        {
                            if (sheet.GetRow(row).GetCell(15).StringCellValue == "金額/分期總價" || sheet.GetRow(row).GetCell(15).StringCellValue == "請退款")
                            {
                                if (sheet.GetRow(row).GetCell(16).StringCellValue != null && sheet.GetRow(row).GetCell(16).StringCellValue != "")
                                {
                                    sheet.GetRow(row).GetCell(16).SetCellValue(Int64.Parse(sheet.GetRow(row).GetCell(16).StringCellValue).ToString("N0"));
                                }
                            }
                        }

                        // 20210819_Ares_Stanley-交易卡號隱碼
                        for (int row = 6; row < sheet.LastRowNum + 1; row++)
                        {
                            sheet.GetRow(row).GetCell(6).SetCellValue(BRExcel_File.addHideCode(sheet.GetRow(row).GetCell(6).StringCellValue));
                        }

                        // 建立空白尾列
                        sheet.CreateRow(sheet.LastRowNum + 1);
                        for (int col = 0; col < 18; col++)
                        {
                            sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = contentFormat;
                        }

                        //* 起訖日期 OR 鍵檔日期
                        if (strProperty == "rptAS_ErrDetail" || strProperty == "rptAS_Capacity")
                        {
                            sheet.GetRow(3).GetCell(0).SetCellValue("鍵檔日期：");
                            sheet.GetRow(3).CreateCell(1)
                                .SetCellValue(strSearchStart.Replace("/", "") + "-" + strSearchEnd.Replace("/", ""));
                            sheet.GetRow(3).GetCell(1).CellStyle = titleFormat;
                        }
                        else
                        {
                            sheet.GetRow(3).GetCell(0).SetCellValue("起迄日期：");
                            sheet.GetRow(3).CreateCell(1)
                                .SetCellValue(strSearchStart.Replace("/", "") + "-" + strSearchEnd.Replace("/", ""));
                            sheet.GetRow(3).GetCell(1).CellStyle = titleFormat;
                        }

                        //* 列印經辦
                        sheet.GetRow(2).CreateCell(1)
                            .SetCellValue(((EntityAGENT_INFO) System.Web.HttpContext.Current.Session["Agent"])
                                .agent_name);
                        sheet.GetRow(2).GetCell(1).CellStyle = titleFormat;
                        //* 列印日期
                        sheet.GetRow(1).CreateCell(1).SetCellValue(DateTime.Now.ToString("yyyyMMdd"));
                        sheet.GetRow(1).GetCell(1).CellStyle = titleFormat;

                        #endregion

                        break;
                    case "rptAS_Capacity":

                        #region 寫入資料-人員鍵檔產能報表

                        for (int i = 0; i < dtblSearchResult.Rows.Count; i++)
                        {
                            int generalCount_key1 = 0;
                            int generalCount_key2 = 0;
                            int instCount_key1 = 0;
                            int instCount_key2 = 0;
                            int totalColumn = 0;

                            sheet.CreateRow(sheet.LastRowNum + 1);
                            // 人員姓名
                            if (!string.IsNullOrEmpty(dtblSearchResult.Rows[i]["Create_User"].ToString()))
                            {
                                sheet.GetRow(sheet.LastRowNum).CreateCell(0)
                                    .SetCellValue(dtblSearchResult.Rows[i]["Create_User"].ToString());
                                sheet.GetRow(sheet.LastRowNum).GetCell(0).CellStyle = contentFormat;
                            }
                            else
                            {
                                sheet.GetRow(sheet.LastRowNum).CreateCell(0).SetCellValue("");
                                sheet.GetRow(sheet.LastRowNum).GetCell(0).CellStyle = contentFormat;
                            }

                            // 請款簽單_一KEY
                            if (!string.IsNullOrEmpty(dtblSearchResult.Rows[i]["general_1key"].ToString()))
                            {
                                sheet.GetRow(sheet.LastRowNum).CreateCell(1).SetCellValue(int
                                    .Parse(dtblSearchResult.Rows[i]["general_1key"].ToString()).ToString("N0"));
                                sheet.GetRow(sheet.LastRowNum).GetCell(1).CellStyle = contentFormat;
                                generalCount_key1 += int.Parse(dtblSearchResult.Rows[i]["general_1key"].ToString());
                                totalGeneralCount_key1 += generalCount_key1;
                            }
                            else
                            {
                                sheet.GetRow(sheet.LastRowNum).CreateCell(1).SetCellValue(0);
                                sheet.GetRow(sheet.LastRowNum).GetCell(1).CellStyle = contentFormat;
                            }

                            // 請款簽單_二KEY
                            if (!string.IsNullOrEmpty(dtblSearchResult.Rows[i]["general_2key"].ToString()))
                            {
                                sheet.GetRow(sheet.LastRowNum).CreateCell(2).SetCellValue(int
                                    .Parse(dtblSearchResult.Rows[i]["general_2key"].ToString()).ToString("N0"));
                                sheet.GetRow(sheet.LastRowNum).GetCell(2).CellStyle = contentFormat;
                                generalCount_key2 += int.Parse(dtblSearchResult.Rows[i]["general_2key"].ToString());
                                totalGeneralCount_key2 += generalCount_key2;
                            }
                            else
                            {
                                sheet.GetRow(sheet.LastRowNum).CreateCell(2).SetCellValue(0);
                                sheet.GetRow(sheet.LastRowNum).GetCell(2).CellStyle = contentFormat;
                            }

                            // 分期訂購單_一KEY
                            if (!string.IsNullOrEmpty(dtblSearchResult.Rows[i]["inst_1key"].ToString()))
                            {
                                sheet.GetRow(sheet.LastRowNum).CreateCell(3).SetCellValue(
                                    int.Parse(dtblSearchResult.Rows[i]["inst_1key"].ToString()).ToString("N0"));
                                sheet.GetRow(sheet.LastRowNum).GetCell(3).CellStyle = contentFormat;
                                instCount_key1 += int.Parse(dtblSearchResult.Rows[i]["inst_1key"].ToString());
                                totalInstCount_key1 += instCount_key1;
                            }
                            else
                            {
                                sheet.GetRow(sheet.LastRowNum).CreateCell(3).SetCellValue(0);
                                sheet.GetRow(sheet.LastRowNum).GetCell(3).CellStyle = contentFormat;
                            }

                            // 分期訂購單_二KEY
                            if (!string.IsNullOrEmpty(dtblSearchResult.Rows[i]["inst_2key"].ToString()))
                            {
                                sheet.GetRow(sheet.LastRowNum).CreateCell(4).SetCellValue(
                                    int.Parse(dtblSearchResult.Rows[i]["inst_2key"].ToString()).ToString("N0"));
                                sheet.GetRow(sheet.LastRowNum).GetCell(4).CellStyle = contentFormat;
                                instCount_key2 += int.Parse(dtblSearchResult.Rows[i]["inst_2key"].ToString());
                                totalInstCount_key2 += instCount_key2;
                            }
                            else
                            {
                                sheet.GetRow(sheet.LastRowNum).CreateCell(4).SetCellValue(0);
                                sheet.GetRow(sheet.LastRowNum).GetCell(4).CellStyle = contentFormat;
                            }

                            // 合計欄
                            totalColumn = generalCount_key1 + generalCount_key2 + instCount_key1 + instCount_key2;
                            totalRow += totalColumn;
                            sheet.GetRow(sheet.LastRowNum).CreateCell(5).SetCellValue(totalColumn.ToString("N0"));
                            sheet.GetRow(sheet.LastRowNum).GetCell(5).CellStyle = contentFormat;
                            sheet.GetRow(sheet.LastRowNum).CreateCell(6).CellStyle = contentFormat;
                            sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 5,
                                6)); // 合併儲存格
                        }

                        //* 起訖日期 OR 鍵檔日期
                        if (strProperty == "rptAS_ErrDetail" || strProperty == "rptAS_Capacity")
                        {
                            sheet.GetRow(3).GetCell(0).SetCellValue("鍵檔日期：");
                            sheet.GetRow(3).CreateCell(1)
                                .SetCellValue(strSearchStart.Replace("/", "") + "-" + strSearchEnd.Replace("/", ""));
                            sheet.GetRow(3).GetCell(1).CellStyle = titleFormat;
                        }
                        else
                        {
                            sheet.GetRow(3).GetCell(0).SetCellValue("起迄日期：");
                            sheet.GetRow(3).CreateCell(1)
                                .SetCellValue(strSearchStart.Replace("/", "") + "-" + strSearchEnd.Replace("/", ""));
                            sheet.GetRow(3).GetCell(1).CellStyle = titleFormat;
                        }

                        //* 列印經辦
                        sheet.GetRow(2).CreateCell(1)
                            .SetCellValue(((EntityAGENT_INFO) System.Web.HttpContext.Current.Session["Agent"])
                                .agent_name);
                        sheet.GetRow(2).GetCell(1).CellStyle = titleFormat;
                        //* 列印日期
                        sheet.GetRow(1).CreateCell(1).SetCellValue(DateTime.Now.ToString("yyyyMMdd"));
                        sheet.GetRow(1).GetCell(1).CellStyle = titleFormat;

                        sheet.CreateRow(sheet.LastRowNum + 1);
                        // 尾列合計
                        sheet.GetRow(sheet.LastRowNum).CreateCell(0).SetCellValue("合計");
                        sheet.GetRow(sheet.LastRowNum).GetCell(0).CellStyle = contentFormat;
                        // 請款簽單_一KEY
                        sheet.GetRow(sheet.LastRowNum).CreateCell(1)
                            .SetCellValue(totalGeneralCount_key1.ToString("N0"));
                        sheet.GetRow(sheet.LastRowNum).GetCell(1).CellStyle = contentFormat;
                        // 請款簽單_二KEY
                        sheet.GetRow(sheet.LastRowNum).CreateCell(2)
                            .SetCellValue(totalGeneralCount_key2.ToString("N0"));
                        sheet.GetRow(sheet.LastRowNum).GetCell(2).CellStyle = contentFormat;
                        // 分期訂購單_一KEY
                        sheet.GetRow(sheet.LastRowNum).CreateCell(3).SetCellValue(totalInstCount_key1.ToString("N0"));
                        sheet.GetRow(sheet.LastRowNum).GetCell(3).CellStyle = contentFormat;
                        // 分期訂購單_二KEY
                        sheet.GetRow(sheet.LastRowNum).CreateCell(4).SetCellValue(totalInstCount_key2.ToString("N0"));
                        sheet.GetRow(sheet.LastRowNum).GetCell(4).CellStyle = contentFormat;
                        // 合計
                        sheet.GetRow(sheet.LastRowNum).CreateCell(5).SetCellValue(totalRow.ToString("N0"));
                        sheet.GetRow(sheet.LastRowNum).GetCell(5).CellStyle = contentFormat;
                        sheet.GetRow(sheet.LastRowNum).CreateCell(6).CellStyle = contentFormat;
                        sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 5, 6)); // 合併儲存格

                        #endregion

                        break;
                    case "rptAS_RejectDetail":

                        #region 寫入資料-人工簽單每日剔退明細

                        ExportExcelForNPOI(dtblSearchResult, ref wb, 7, "工作表1", 1);
                        // 新增序號欄
                        for (int row = 7; row < sheet.LastRowNum + 1; row++)
                        {
                            sheet.GetRow(row).CreateCell(0).SetCellValue((row - 6).ToString("N0"));
                            sheet.GetRow(row).GetCell(0).CellStyle = contentFormat;
                        }

                        for (int row = 7; row < sheet.LastRowNum + 1; row++)
                        {
                            sheet.GetRow(row).GetCell(4)
                                .SetCellValue(int.Parse(sheet.GetRow(row).GetCell(4).StringCellValue).ToString("N0"));
                        }

                        //卡號隱碼
                        for (int row = 7; row < sheet.LastRowNum + 1; row++)
                        {
                            sheet.GetRow(row).GetCell(3).SetCellValue(BRExcel_File.addHideCode(sheet.GetRow(row).GetCell(3).StringCellValue));
                        }

                        // 建立空白尾列
                        sheet.CreateRow(sheet.LastRowNum + 1);
                        for (int col = 0; col < 6; col++)
                        {
                            sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = contentFormat;
                        }

                        //* 起訖日期 OR 鍵檔日期
                        if (strProperty == "rptAS_ErrDetail" || strProperty == "rptAS_Capacity")
                        {
                            sheet.GetRow(3).GetCell(0).SetCellValue("鍵檔日期：");
                            sheet.GetRow(3).CreateCell(1)
                                .SetCellValue(strSearchStart.Replace("/", "") + "-" + strSearchEnd.Replace("/", ""));
                            sheet.GetRow(3).GetCell(1).CellStyle = titleFormat;
                        }
                        else
                        {
                            sheet.GetRow(3).GetCell(0).SetCellValue("起迄日期：");
                            sheet.GetRow(3).CreateCell(1)
                                .SetCellValue(strSearchStart.Replace("/", "") + "-" + strSearchEnd.Replace("/", ""));
                            sheet.GetRow(3).GetCell(1).CellStyle = titleFormat;
                        }

                        //* 列印經辦
                        sheet.GetRow(2).CreateCell(1)
                            .SetCellValue(((EntityAGENT_INFO) System.Web.HttpContext.Current.Session["Agent"])
                                .agent_name);
                        sheet.GetRow(2).GetCell(1).CellStyle = titleFormat;
                        //* 列印日期
                        sheet.GetRow(1).CreateCell(1).SetCellValue(DateTime.Now.ToString("yyyyMMdd"));
                        sheet.GetRow(1).GetCell(1).CellStyle = titleFormat;

                        #endregion

                        break;
                }

                strServerPathFile = strServerPathFile + @"\" + strProperty + "_" +
                                    DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strServerPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();
                //* 將服務器端生成的文檔，下載到本地。
                string strYYYYMMDD = "000" +
                                     CSIPCommonModel.BaseItem.Function.MinGuoDate7length(
                                         DateTime.Now.ToString("yyyyMMdd"));
                strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
                string strFileName = strPropertyName.Trim().Replace("/", "") + strYYYYMMDD + ".xls";

                //* 顯示提示訊息：匯出到Excel文檔資料成功
                this.Session["ServerFile"] = strServerPathFile;
                this.Session["ClientFile"] = strFileName;
                // string urlString = @"ClientMsgShow('" + MessageHelper.GetMessage("01_01030400_003") + "');";
                string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" +
                                   MessageHelper.GetMessage("01_01030400_003") + "' }, '*');";
                urlString += @"location.href='DownLoadFile.aspx';";
                base.sbRegScript.Append(urlString);

                base.strHostMsg += "";
                base.strClientMsg += "";
            }
            else
            {
                strMsgID = "01_03100000_005";
                sbRegScriptF.Append("alert('" + MessageHelper.GetMessage(strMsgID) + "');");
                sbRegScriptF.Append("window.close();");
                this.sbRegScript.Append(sbRegScriptF.ToString());
            }
        }
    }
    /// <summary>
    /// 建立日期:2021/02/19_Ares_Stanley-查無資料時列印空範本並印出查無資料; 2021/03/08_Ares_Stanley-代入空報表日期、經辦
    /// </summary>
    /// <param name="strProperty"></param>
    /// <param name="strSearchStart"></param>
    /// <param name="strSearchEnd"></param>
    private void OutputEmptyExcel(string strProperty, string strSearchStart, string strSearchEnd)
    {
        try
        {
            string strPropertyName = this.Session["PropertyName"].ToString();
            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
            // 檢查目錄，並刪除以前的文檔資料
            CSIPKeyInGUI.BusinessRules_new.BRExcel_File.CheckDirectory(ref strServerPathFile);
            string strRPTPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") +
                     strProperty + ".xls";
            FileStream fs = new FileStream(strRPTPathFile, FileMode.Open);
            HSSFWorkbook wb = new HSSFWorkbook(fs);
            ISheet sheet = wb.GetSheet("工作表1");
            #region 文字格式
            HSSFCellStyle contentFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
            contentFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
            contentFormat.Alignment = HorizontalAlignment.Center; //水平置中
            contentFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字
            contentFormat.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin; // 儲存格框線
            contentFormat.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            contentFormat.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            contentFormat.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

            HSSFFont contentFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
            contentFont.FontHeightInPoints = 12; //字體大小
            contentFont.FontName = "新細明體"; //字型
            contentFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold; //粗體
            contentFormat.SetFont(contentFont); //設定儲存格的文字樣式
            #endregion
            #region 儲存格文字格式
            HSSFCellStyle titleFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
            titleFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
            titleFormat.Alignment = HorizontalAlignment.Left; //水平置中
            titleFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字

            HSSFFont titleFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
            titleFont.FontHeightInPoints = 12; //字體大小
            titleFont.FontName = "新細明體"; //字型
                                         //titleFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold; //粗體
            titleFormat.SetFont(titleFont); //設定儲存格的文字樣式
            #endregion

            sheet.CreateRow(sheet.LastRowNum + 1);
            switch (strProperty)
            {
                case "rptAS_Work":
                    #region 人工簽單作業量報表
                    //* 起訖日期 OR 鍵檔日期
                    if (strProperty == "rptAS_ErrDetail" || strProperty == "rptAS_Capacity")
                    {
                        sheet.GetRow(3).GetCell(0).SetCellValue("鍵檔日期：");
                        sheet.GetRow(3).CreateCell(1)
                            .SetCellValue(strSearchStart.Replace("/", "") + "-" + strSearchEnd.Replace("/", ""));
                        sheet.GetRow(3).GetCell(1).CellStyle = titleFormat;
                    }
                    else
                    {
                        sheet.GetRow(3).GetCell(0).SetCellValue("起迄日期：");
                        sheet.GetRow(3).CreateCell(1)
                            .SetCellValue(strSearchStart.Replace("/", "") + "-" + strSearchEnd.Replace("/", ""));
                        sheet.GetRow(3).GetCell(1).CellStyle = titleFormat;
                    }

                    //* 列印經辦
                    sheet.GetRow(2).CreateCell(1)
                        .SetCellValue(((EntityAGENT_INFO)System.Web.HttpContext.Current.Session["Agent"])
                            .agent_name);
                    sheet.GetRow(2).GetCell(1).CellStyle = titleFormat;
                    //* 列印日期
                    sheet.GetRow(1).CreateCell(1).SetCellValue(DateTime.Now.ToString("yyyyMMdd"));
                    sheet.GetRow(1).GetCell(1).CellStyle = titleFormat;
                    for (int col = 0; col < 7; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col);
                        sheet.GetRow(sheet.LastRowNum).GetCell(col).CellStyle = contentFormat;
                    }
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 0, 6));
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("查無資料,查詢結束!");
                    break;
                #endregion

                case "rptAS_ErrDetail":
                    #region 鍵檔錯誤明細
                    //* 起訖日期 OR 鍵檔日期
                    if (strProperty == "rptAS_ErrDetail" || strProperty == "rptAS_Capacity")
                    {
                        sheet.GetRow(3).GetCell(0).SetCellValue("鍵檔日期：");
                        sheet.GetRow(3).CreateCell(1)
                            .SetCellValue(strSearchStart.Replace("/", "") + "-" + strSearchEnd.Replace("/", ""));
                        sheet.GetRow(3).GetCell(1).CellStyle = titleFormat;
                    }
                    else
                    {
                        sheet.GetRow(3).GetCell(0).SetCellValue("起迄日期：");
                        sheet.GetRow(3).CreateCell(1)
                            .SetCellValue(strSearchStart.Replace("/", "") + "-" + strSearchEnd.Replace("/", ""));
                        sheet.GetRow(3).GetCell(1).CellStyle = titleFormat;
                    }

                    //* 列印經辦
                    sheet.GetRow(2).CreateCell(1)
                        .SetCellValue(((EntityAGENT_INFO)System.Web.HttpContext.Current.Session["Agent"])
                            .agent_name);
                    sheet.GetRow(2).GetCell(1).CellStyle = titleFormat;
                    //* 列印日期
                    sheet.GetRow(1).CreateCell(1).SetCellValue(DateTime.Now.ToString("yyyyMMdd"));
                    sheet.GetRow(1).GetCell(1).CellStyle = titleFormat;
                    for (int col = 0; col < 18; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col);
                        sheet.GetRow(sheet.LastRowNum).GetCell(col).CellStyle = contentFormat;
                    }
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 0, 17));
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("查無資料,查詢結束!");
                    break;
                #endregion

                case "rptAS_Capacity":
                    #region 人員鍵檔產能報表
                    //* 起訖日期 OR 鍵檔日期
                    if (strProperty == "rptAS_ErrDetail" || strProperty == "rptAS_Capacity")
                    {
                        sheet.GetRow(3).GetCell(0).SetCellValue("鍵檔日期：");
                        sheet.GetRow(3).CreateCell(1)
                            .SetCellValue(strSearchStart.Replace("/", "") + "-" + strSearchEnd.Replace("/", ""));
                        sheet.GetRow(3).GetCell(1).CellStyle = titleFormat;
                    }
                    else
                    {
                        sheet.GetRow(3).GetCell(0).SetCellValue("起迄日期：");
                        sheet.GetRow(3).CreateCell(1)
                            .SetCellValue(strSearchStart.Replace("/", "") + "-" + strSearchEnd.Replace("/", ""));
                        sheet.GetRow(3).GetCell(1).CellStyle = titleFormat;
                    }

                    //* 列印經辦
                    sheet.GetRow(2).CreateCell(1)
                        .SetCellValue(((EntityAGENT_INFO)System.Web.HttpContext.Current.Session["Agent"])
                            .agent_name);
                    sheet.GetRow(2).GetCell(1).CellStyle = titleFormat;
                    //* 列印日期
                    sheet.GetRow(1).CreateCell(1).SetCellValue(DateTime.Now.ToString("yyyyMMdd"));
                    sheet.GetRow(1).GetCell(1).CellStyle = titleFormat;
                    for (int col = 0; col < 7; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col);
                        sheet.GetRow(sheet.LastRowNum).GetCell(col).CellStyle = contentFormat;
                    }
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 0, 6));
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("查無資料,查詢結束!");
                    break;
                #endregion

                case "rptAS_RejectDetail":
                    #region 人工簽單每日剔退明細
                    //* 起訖日期 OR 鍵檔日期
                    if (strProperty == "rptAS_ErrDetail" || strProperty == "rptAS_Capacity")
                    {
                        sheet.GetRow(3).GetCell(0).SetCellValue("鍵檔日期：");
                        sheet.GetRow(3).CreateCell(1)
                            .SetCellValue(strSearchStart.Replace("/", "") + "-" + strSearchEnd.Replace("/", ""));
                        sheet.GetRow(3).GetCell(1).CellStyle = titleFormat;
                    }
                    else
                    {
                        sheet.GetRow(3).GetCell(0).SetCellValue("起迄日期：");
                        sheet.GetRow(3).CreateCell(1)
                            .SetCellValue(strSearchStart.Replace("/", "") + "-" + strSearchEnd.Replace("/", ""));
                        sheet.GetRow(3).GetCell(1).CellStyle = titleFormat;
                    }

                    //* 列印經辦
                    sheet.GetRow(2).CreateCell(1)
                        .SetCellValue(((EntityAGENT_INFO)System.Web.HttpContext.Current.Session["Agent"])
                            .agent_name);
                    sheet.GetRow(2).GetCell(1).CellStyle = titleFormat;
                    //* 列印日期
                    sheet.GetRow(1).CreateCell(1).SetCellValue(DateTime.Now.ToString("yyyyMMdd"));
                    sheet.GetRow(1).GetCell(1).CellStyle = titleFormat;
                    for (int col = 0; col < 6; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col);
                        sheet.GetRow(sheet.LastRowNum).GetCell(col).CellStyle = contentFormat;
                    }
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 0, 5));
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("查無資料,查詢結束!");
                    break;
                    #endregion
            }
            strServerPathFile = strServerPathFile + @"\" + strProperty + "_" +
                    DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            FileStream fs1 = new FileStream(strServerPathFile, FileMode.Create);
            wb.Write(fs1);
            fs1.Close();
            fs.Close();
            //* 將服務器端生成的文檔，下載到本地。
            string strYYYYMMDD = "000" +
                                 CSIPCommonModel.BaseItem.Function.MinGuoDate7length(
                                     DateTime.Now.ToString("yyyyMMdd"));
            strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
            string strFileName = strPropertyName.Trim().Replace("/", "") + strYYYYMMDD + ".xls";

            //* 顯示提示訊息：匯出到Excel文檔資料成功
            this.Session["ServerFile"] = strServerPathFile;
            this.Session["ClientFile"] = strFileName;
            // string urlString = @"ClientMsgShow('" + MessageHelper.GetMessage("01_01030400_003") + "');";
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" +
                               MessageHelper.GetMessage("01_01030400_003") + "' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);

            base.strHostMsg += "";
            base.strClientMsg += "";
        }
        catch(Exception e)
        {
            Logging.Log(e);
            throw;
        }
        

    }

    #region 共用NPOI

    /// <summary>
    /// 修改紀錄:2021/01/12_Ares_Stanley-新增共用NPOI
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="wb"></param>
    /// <param name="start"></param>
    /// <param name="sheetName"></param>
    private static void ExportExcelForNPOI(DataTable dt, ref HSSFWorkbook wb, Int32 start, String sheetName,
        int cellStart)
    {
        try
        {
            HSSFCellStyle cs = (HSSFCellStyle) wb.CreateCellStyle();
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

            HSSFFont font1 = (HSSFFont) wb.CreateFont();
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
                    IRow row = (IRow) sheet.CreateRow(count);
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

    /// <summary>
    /// 修改紀錄: 2021/01/12_Ares_Stanley-新增共用NPOI
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="wb"></param>
    /// <param name="start"></param>
    /// <param name="delColumn"></param>
    /// <param name="sheetName"></param>
    private static void ExportExcelForNPOI_filter(DataTable dt, ref HSSFWorkbook wb, Int32 start, Int32 delColumn,
        String sheetName)
    {
        try
        {
            HSSFCellStyle cs = (HSSFCellStyle) wb.CreateCellStyle();
            cs.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            cs.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            cs.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            cs.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

            //啟動多行文字
            cs.WrapText = true;
            //文字置中
            cs.VerticalAlignment = VerticalAlignment.Center;
            cs.Alignment = HorizontalAlignment.Center;
            cs.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字
            HSSFFont font1 = (HSSFFont) wb.CreateFont();
            //字體尺寸
            font1.FontHeightInPoints = 12;
            font1.FontName = "新細明體";
            cs.SetFont(font1);

            if (dt != null && dt.Rows.Count != 0)
            {
                int count = start;
                ISheet sheet = wb.GetSheet(sheetName);
                int cols = dt.Columns.Count - delColumn;
                foreach (DataRow dr in dt.Rows)
                {
                    int cell = 0;
                    IRow row = (IRow) sheet.CreateRow(count);
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
    /// 創建時間:2021/01/20
    /// </summary>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }

    /// <summary>
    /// 功能說明:查詢事件
    /// 作    者:Ares Luke
    /// 創建時間:2021/01/20
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
    /// 創建時間:2021/01/20 
    /// </summary>
    private void BindGridView()
    {
        string strProperty = "";
        string strSearchStart = "";
        string strSearchEnd = "";

        if (chkCond(ref strProperty, ref strSearchStart, ref strSearchEnd))
        {
            try
            {
                DataTable dt = new DataTable();
                Int32 count = 0;
                Boolean result = BRASRpt.SearchGripData(
                    strProperty, strSearchStart, strSearchEnd
                    , this.gpList.CurrentPageIndex, this.gpList.PageSize, ref count, ref dt);
                //* 查詢成功
                if (result)
                {
                    DtStatistics(strProperty, ref dt);

                    this.gpList.Visible = true;
                    this.gpList.RecordCount = count;
                    this.grvUserView.Visible = true;
                    this.grvUserView.DataSource = dt;
                    this.grvUserView.DataBind();
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("01_03100000_013"));
                }
                //* 查詢不成功
                else
                {
                    this.gpList.RecordCount = 0;
                    this.grvUserView.DataSource = null;
                    this.grvUserView.DataBind();
                    this.gpList.Visible = false;
                    this.grvUserView.Visible = false;
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("01_03100000_012"));
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
    /// 創建時間:2021/01/20 
    /// </summary>
    /// <param name="strProperty"></param>
    /// <param name="strSearchStart"></param>
    /// <param name="strSearchEnd"></param>
    /// <returns></returns>
    private Boolean chkCond(ref string strProperty, ref string strSearchStart, ref string strSearchEnd)
    {
        string strMsgId = string.Empty;
        if (!CheckCondition(ref strMsgId))
        {
            MessageHelper.ShowMessage(this, strMsgId);
            return false;
        }

        try
        {
            strProperty = this.dropProperty.Text;
            //* 區間起
            strSearchStart = this.dtpSearchStart.Text.Trim().Replace("/", "");
            //* 區間迄
            //2021/03/19_Ares_Stanley-加入訖日時間
            strSearchEnd = this.dtpSearchEnd.Text.Trim().Replace("/", "") + " 23:59:59";
        }
        catch (Exception exp)
        {
            Logging.Log(exp);
            MessageHelper.ShowMessage(this, "01_03100000_011");
            return false;
        }

        return true;
    }


    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:檢核條件
    /// 作    者:Ares Luke
    /// 創建時間:2021/01/20 
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    private bool CheckCondition(ref string strMsgID)
    {
        //　起日
        if (string.IsNullOrWhiteSpace(this.dtpSearchStart.Text.Trim()))
        {
            strMsgID = "01_03100000_008";
            dtpSearchStart.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(this.dtpSearchEnd.Text.Trim()))
        {
            strMsgID = "01_03100000_008";
            dtpSearchEnd.Focus();
            return false;
        }


        if (DateTime.Compare(DateTime.Parse(dtpSearchStart.Text), DateTime.Parse(dtpSearchEnd.Text)) > 0)
        {
            strMsgID = "01_03100000_009";
            dtpSearchStart.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(this.dropProperty.Text.Trim()))
        {
            strMsgID = "01_03100000_010";
            dtpSearchStart.Focus();
            return false;
        }

        return true;
    }


    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:報表種類監聽事件
    /// 作    者:Ares Luke
    /// 創建時間:2021/01/20 
    /// </summary>
    protected void dropProperty_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetGridViewTitle();

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        this.grvUserView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
    }

    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:設置動態GridView標題
    /// 作    者:Ares Luke
    /// 創建時間:2021/01/20 
    /// </summary>
    private void SetGridViewTitle()
    {
        this.grvUserView.Columns.Clear();

        this.gpList.RecordCount = 0;
        this.grvUserView.DataSource = null;
        this.grvUserView.DataBind();
        this.gpList.Visible = false;
        this.grvUserView.Visible = false;


        if (dropProperty.SelectedValue == "rptAS_Work")
        {
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_006"), DataField = "KeyinDate"});
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_007"), DataField = "format_Adjust_Count_general"});
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_008"), DataField = "format_Adjust_Count_inst" });
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_009"), DataField = "format_totalColumn" });
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_010"), DataField = "format_Err_Count" });
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_011"), DataField = "format_percentColumn" });
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_012"), DataField = "note"});
        }
        else if (dropProperty.SelectedValue == "rptAS_ErrDetail")
        {
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_020"), DataField = "KeyinDate"});
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_021"), DataField = "Batch_Date"});
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_022"), DataField = "Sign_Type"});
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_023"), DataField = "Batch_NO"});
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_024"), DataField = "Shop_ID"});
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_025"), DataField = "Card_No"});
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_026"), DataField = "Tran_Date"});
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_027"), DataField = "Auth_Code"});
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_028"), DataField = "format_AMT" });
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_029"), DataField = "Receipt_Type"});
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_030"), DataField = "Product_Type"});
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_031"), DataField = "Installment_Periods"});
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_032"), DataField = "1Key_user"});
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_033"), DataField = "2Key_user"});
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_034"), DataField = "Error_Column"});
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_035"), DataField = "format_Correct_Value" });
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_036"), DataField = "Reflect_Source"});
        }
        else if (dropProperty.SelectedValue == "rptAS_Capacity")
        {
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_040"), DataField = "Create_User"});
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_041"), DataField = "format_general_1key" });
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_042"), DataField = "format_general_2key" });
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_043"), DataField = "format_inst_1key" });
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_044"), DataField = "format_inst_2key" });
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_045"), DataField = "format_totalColumn" });
        }
        else if (dropProperty.SelectedValue == "rptAS_RejectDetail")
        {
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_050"), DataField = "Batch_Date"});
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_051"), DataField = "Compare"});
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_052"), DataField = "Card_No"});
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_053"), DataField = "format_AMT"});
            this.grvUserView.Columns.Add(new BoundField
                {HeaderText = BaseHelper.GetShowText("01_03100000_054"), DataField = "Reject_Reason"});
        }
    }


    /// <summary>
    /// 專案代號:20200031-CSIP EOS
    /// 功能說明:查詢結果統計功能
    /// 作    者:Ares Luke
    /// 創建時間:2021/01/20 
    /// 修改紀錄:2021/01/26_Ares_Stanley-調整正確率計算
    /// </summary>

    private void DtStatistics(string strProperty, ref DataTable dt)
    {
        switch (strProperty)
        {
            case "rptAS_Work":

                #region 人工簽單作業量報表

                dt.Columns.Add("format_Adjust_Count_general");
                dt.Columns.Add("format_Adjust_Count_inst");
                dt.Columns.Add("format_totalColumn");
                dt.Columns.Add("format_percentColumn");
                dt.Columns.Add("format_Err_Count");

                dt.Columns.Add("note");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    // 請款簽單(一般簽單)
                    int generalCount = int.Parse(dt.Rows[i]["Adjust_Count_general"].ToString());
                    dt.Rows[i]["format_Adjust_Count_general"] = generalCount.ToString("N0");
                    // 分期訂購單(分期簽單)
                    int instCount = int.Parse(dt.Rows[i]["Adjust_Count_inst"].ToString());
                    dt.Rows[i]["format_Adjust_Count_inst"] = instCount.ToString("N0");
                    // 合計欄
                    int totalColumn = generalCount + instCount;
                    dt.Rows[i]["format_totalColumn"] = totalColumn.ToString("N0");
                    // 錯誤筆數
                    int errorCount = int.Parse(dt.Rows[i]["Err_Count"].ToString());
                    dt.Rows[i]["format_Err_Count"] = errorCount.ToString("N0");
                    // 正確率
                    double percentColumn = 0;
                    if (errorCount != 0 && totalColumn != 0)
                    {
                        percentColumn = Convert.ToDouble(errorCount) / Convert.ToDouble(totalColumn);
                    }
                    else if (errorCount != 0 && totalColumn == 0)
                    {
                        percentColumn = 0;
                    }
                    else if (errorCount == 0 && totalColumn != 0)
                    {
                        percentColumn = 1;
                    }

                    string percentColumnStr = percentColumn.ToString("0.00%");
                    dt.Rows[i]["format_percentColumn"] = percentColumnStr;
                    dt.Rows[i]["note"] = "";
                }

                #endregion

                break;

            case "rptAS_ErrDetail":

                #region 寫入資料-鍵檔錯誤明細
                dt.Columns.Add("format_AMT", typeof(string));
                dt.Columns.Add("format_Correct_Value", typeof(string));

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i]["format_AMT"] = Int64.Parse(dt.Rows[i]["AMT"].ToString()).ToString("N0");
                    if(dt.Rows[i]["Error_Column"].ToString() == "金額/分期總價"|| dt.Rows[i]["Error_Column"].ToString() == "請退款")
                    {
                        if (dt.Rows[i]["Correct_Value"].ToString() != null && dt.Rows[i]["Correct_Value"].ToString() != "")
                            dt.Rows[i]["format_Correct_Value"] = Int64.Parse(dt.Rows[i]["Correct_Value"].ToString()).ToString("N0");
                    }
                    else
                    {
                        if (dt.Rows[i]["Correct_Value"].ToString() != null && dt.Rows[i]["Correct_Value"].ToString() != "")
                            dt.Rows[i]["format_Correct_Value"] = dt.Rows[i]["Correct_Value"].ToString();
                    }
                }

                #endregion

                break;
            case "rptAS_Capacity":

                #region 寫入資料-人員鍵檔產能報表

                dt.Columns.Add("format_general_1key", typeof(string));
                dt.Columns.Add("format_general_2key", typeof(string));
                dt.Columns.Add("format_inst_1key", typeof(string));
                dt.Columns.Add("format_inst_2key", typeof(string));
                dt.Columns.Add("format_totalColumn", typeof(string));
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    // 請款簽單_一KEY
                    int generalCount_key1 = int.Parse(dt.Rows[i]["general_1key"].ToString());
                    dt.Rows[i]["format_general_1key"] = generalCount_key1.ToString("N0");

                    // 請款簽單_二KEY
                    int generalCount_key2 = int.Parse(dt.Rows[i]["general_2key"].ToString());
                    dt.Rows[i]["format_general_2key"] = generalCount_key2.ToString("N0");

                    // 分期訂購單_一KEY
                    int instCount_key1 = int.Parse(dt.Rows[i]["inst_1key"].ToString());
                    dt.Rows[i]["format_inst_1key"] = instCount_key1.ToString("N0");

                    // 分期訂購單_二KEY
                    int instCount_key2 = int.Parse(dt.Rows[i]["inst_2key"].ToString());
                    dt.Rows[i]["format_inst_2key"] = instCount_key2.ToString("N0");

                    // 合計欄
                    int totalColumn = generalCount_key1 + generalCount_key2 + instCount_key1 + instCount_key2;
                    dt.Rows[i]["format_totalColumn"] = totalColumn.ToString("N0");
                }

                #endregion

                break;
            case "rptAS_RejectDetail":

                #region 寫入資料-人工簽單每日剔退明細

                dt.Columns.Add("format_AMT", typeof(string));
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    // 剔退金額
                    int AMT = int.Parse(dt.Rows[i]["AMT"].ToString());
                    dt.Rows[i]["format_AMT"] = AMT.ToString("N0");
                }

                #endregion

                break;
        }

        
        for (int i = 0; i < this.grvUserView.Columns.Count; i++)
        {
            this.grvUserView.Columns[i].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            this.grvUserView.Columns[i].ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            this.grvUserView.Columns[i].HeaderStyle.CssClass = "whiteSpaceNormal";
            this.grvUserView.Columns[i].ItemStyle.CssClass = "whiteSpaceNormal";
        }
    }
}