//******************************************************************
//*  作    者：占偉林(James)
//*  功能說明：報表-作業量統計表
//*  創建日期：2009/11/11
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

public partial class P010302000001 : PageBase
{
    #region event

    /// 作者 占偉林
    /// 創建日期：2009/11/11
    /// 修改日期：2009/11/11 
    /// <summary>
    /// 畫面裝載時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        // 綁定GridView表頭訊息
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
    
    ///// 作者 占偉林
    ///// 創建日期：2009/11/11
    ///// 修改日期：2009/11/11 
    /// <summary>
    /// 點選畫面【列印】按鈕時的處理
    /// 修改紀錄:2021/01/18_Ares_Stanley-變更報表產出為NPOI
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        OutputExcel();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.gpList.Visible = true;
        this.gpList.CurrentPageIndex = 0;
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
        string strMsgID = "";
        int iTotalCount = 0;
        gvpbNewItem.DataSource = BRStatisticsRpt.SearchRPTData(this.dtpSearchStart.Text.Trim(), this.dtpSearchEnd.Text.Trim(), ref strMsgID, gpList.CurrentPageIndex, gpList.PageSize, ref iTotalCount);
        gpList.RecordCount = iTotalCount;
        gvpbNewItem.DataBind();
    }
    private void ShowControlsText()
    {
        this.gvpbNewItem.Columns[0].HeaderText = BaseHelper.GetShowText("01_03020000_005");
        this.gvpbNewItem.Columns[1].HeaderText = BaseHelper.GetShowText("01_03020000_006");
        this.gvpbNewItem.Columns[2].HeaderText = BaseHelper.GetShowText("01_03020000_007");
        this.gvpbNewItem.Columns[3].HeaderText = BaseHelper.GetShowText("01_03020000_008");
        this.gvpbNewItem.Columns[4].HeaderText = BaseHelper.GetShowText("01_03020000_009");
        // 每頁顯示最大筆數
        this.gpList.PageSize= int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gvpbNewItem.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
    }
    #endregion method

    #region function
    protected void OutputExcel()
    {
        try
        {
            StringBuilder sbRegScriptF = new StringBuilder("");
            this.Title = BaseHelper.GetShowText("01_03020000_001");
            string strSearchStart = "";
            string strSearchEnd = "";
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
                sbRegScriptF.Append("alert('" + MessageHelper.GetMessage("01_03020000_001") + "');");
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
                sbRegScriptF.Append("alert('" + MessageHelper.GetMessage("01_03020000_001") + "');");
                sbRegScriptF.Append("window.close();");
                this.sbRegScript.Append(sbRegScriptF.ToString());
                return;
            }
            #endregion 從畫面取值

            #region 報表寫入資料
            string strMsgID = "";

            DataTable dtblSearchResult = BRStatisticsRpt.SearchRPTData(strSearchStart, strSearchEnd, ref strMsgID);
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
                    string strRPTPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "newitem_sum.xls";
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
                    // 建立表頭儲存格
                    for (int row = 2; row < 5; row++)
                    {
                        sheet.GetRow(row).CreateCell(0).CellStyle = titleFormat;
                    }
                    sheet.GetRow(2).GetCell(0).SetCellValue("列印日期：" + DateTime.Now.ToString("yyyyMMdd"));
                    sheet.GetRow(3).GetCell(0).SetCellValue("列印經辦：" + ((EntityAGENT_INFO)System.Web.HttpContext.Current.Session["Agent"]).agent_name);
                    sheet.GetRow(4).GetCell(0).SetCellValue("起訖日期：" + strSearchStart.Replace("/", "") + "-" + strSearchEnd.Replace("/", ""));
                    #endregion 表頭

                    #region 表格內容
                    for (int i = 0; i < dtblSearchResult.Rows.Count; i++)
                    {
                        int rowSum_12key = 0;
                        // 建立新列
                        sheet.CreateRow(sheet.LastRowNum + 1);
                        // 建立儲存格
                        for (int col = 0; col < 5; col++)
                        {
                            sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle=contentFormat;
                        }
                        // 作業項目
                        sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue(dtblSearchResult.Rows[i]["trans_name2"].ToString());
                        // 業務項目
                        sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(dtblSearchResult.Rows[i]["trans_name"].ToString());
                        // 一KEY作業量
                        sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue(dtblSearchResult.Rows[i]["trans_1keysum"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(2).StringCellValue.ToString()))
                            rowSum_12key += int.Parse(sheet.GetRow(sheet.LastRowNum).GetCell(2).StringCellValue.ToString());
                        // 二KEY作業量
                        sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(dtblSearchResult.Rows[i]["trans_sum"].ToString());
                        if (!string.IsNullOrEmpty(sheet.GetRow(sheet.LastRowNum).GetCell(3).StringCellValue.ToString()))
                            rowSum_12key += int.Parse(sheet.GetRow(sheet.LastRowNum).GetCell(3).StringCellValue.ToString());
                        // 合計
                        sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(rowSum_12key.ToString("N0"));
                    }
                    #endregion 表格內容

                    #region 表尾
                    // 轉換表格資料格式
                    for (int row = 7; row < sheet.LastRowNum + 1; row++)
                    {
                        for (int col = 2; col < 4; col++)
                        {
                            if (!string.IsNullOrEmpty(sheet.GetRow(row).GetCell(col).StringCellValue))
                            {
                                sheet.GetRow(row).GetCell(col).SetCellValue(int.Parse(sheet.GetRow(row).GetCell(col).StringCellValue.ToString()));
                            }
                            else
                            {
                                sheet.GetRow(row).GetCell(col).SetCellValue(0);
                            }
                        }
                    }
                    // 建立合計列
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    // 建立儲存格
                    for (int col = 0; col < 5; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = contentFormat;
                    }
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("合計");
                    sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellFormula(string.Format("SUM(C8:C{0})", sheet.LastRowNum));
                    sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellType(CellType.Formula);
                    sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellFormula(string.Format("SUM(D8:D{0})", sheet.LastRowNum));

                    // 主管/組長/經辦
                    sheet.CreateRow(sheet.LastRowNum + 2);
                    for (int col = 0; col < 5; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = titleFormat;
                    }
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("主管：　　　　　　　組長：　　　　　　　經辦：");
                    #endregion 表尾


                    //* 保存文件到程序运行目录下
                    strServerPathFile = strServerPathFile + @"\NewItem_Sum_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    FileStream fs1 = new FileStream(strServerPathFile, FileMode.Create);
                    wb.Write(fs1);
                    fs1.Close();
                    fs.Close();


                    base.strHostMsg += "";
                    base.strClientMsg += "";

                    //* 顯示提示訊息：匯出到Excel文檔資料成功
                    string strFileName = "作業量統計報表.xls";
                    this.Session["ServerFile"] = strServerPathFile;
                    this.Session["ClientFile"] = strFileName;
                    string urlString = @"location.href='DownLoadFile.aspx';";
                    base.sbRegScript.Append(urlString);
                }
                else
                {
                    strMsgID = "01_03020000_003";
                    sbRegScriptF.Append("alert('" + MessageHelper.GetMessage(strMsgID) + "');");
                    sbRegScriptF.Append("window.close();");
                    this.sbRegScript.Append(sbRegScriptF.ToString());
                }

            }

            #endregion 報表寫入資料


        }
        catch (Exception exp)
        {
            StringBuilder sbRegScriptF = new StringBuilder("");
            BRCompareRpt.SaveLog(exp);
            sbRegScriptF.Append("alert('" + MessageHelper.GetMessage("01_03020000_004") + "');");
            sbRegScriptF.Append("window.close();");
            this.sbRegScript.Append(sbRegScriptF.ToString());
        }
    }
    #endregion function
}
