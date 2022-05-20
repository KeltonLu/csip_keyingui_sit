//******************************************************************
//*  作    者：
//*  功能說明：請款簽單明細表
//*  創建日期：2014/08/08
//*  修改紀錄：
//*  <author>            <time>            <TaskID>                <desc>
//*  Ares Luke          2020/11/19         20200031-CSIP EOS       調整取web.config加解密參數
//*  Ares Stanley      2021/04/12                                  修改ACQ IP、PORT web.config變數
//*  Ares Stanley      2021/04/15                                  調整EXCHANGEFILE路徑取得方法
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
//using CSIPKeyInGUI.BusinessRules;
using CSIPKeyInGUI.EntityLayer;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using Framework.Common.Logging;
using Framework.Common.Utility;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BaseItem;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Net;
using System.Collections.Generic;
using Framework.Common.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.HSSF.EventUserModel.DummyRecord;
using NPOI.XSSF.UserModel.Charts;
public partial class P010107010001 : PageBase
{
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息

    #region event

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            //* 綁定GridView列頭訊息
            ShowControlsText();
            SetPageSize();
            this.gpList.RecordCount = 0;
            this.gpList.Visible = false;
            this.gvpbSearchRecord.DataSource = null;
            this.gvpbSearchRecord.DataBind();
        }
        base.strHostMsg += "";
        base.strClientMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"];
        sPageInfo = (structPageInfo)this.Session["PageInfo"];
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {

        this.gpList.CurrentPageIndex = 0;
        BindGridView();
        this.gpList.Visible = true;
    }

    protected void gvpbSearchRecord_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            foreach (Control c in e.Row.Cells[2].Controls)
            {
                if (c.GetType().Equals(typeof(HyperLink)))
                {
                    HyperLink hl = (HyperLink)c;
                    string strParameters = "?BatchDate=" + RedirectHelper.GetEncryptParam(this.Session["BatchDate"].ToString())
                                        + "&ReceiveBatch=" + RedirectHelper.GetEncryptParam(this.Session["ReceiveBatch"].ToString())
                                        + "&BatchNO=" + RedirectHelper.GetEncryptParam(e.Row.Cells[1].Text)
                                        + "&ShopID=" + RedirectHelper.GetEncryptParam(hl.Text)
                                        + "&SignType=" + RedirectHelper.GetEncryptParam(e.Row.Cells[0].Text == "一般簽單" ? "1" : "2");
                    hl.NavigateUrl = "javascript:void window.open('P010107010002.aspx" + strParameters + "','_blank','width=1000,height=600,resizable=yes')";
                }
            }
            //收件-總金額 負數顯示紅字
            for (int i = 4; i < 15; i++)
            {
                if ((e.Row.Cells[i].Text.Equals("&nbsp;") ? 0 : Convert.ToDecimal(e.Row.Cells[i].Text)) < 0)
                {
                    e.Row.Cells[i].ForeColor = Color.Red;
                }
            }
        }
    }

    protected void gpList_PageChanged(object src, PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView();
    }

    #region 產出檔案
    //產生請款簽單明細表
    protected void btnExportASDetail_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
        string strBatchDate = this.dpBatchDate.Text;
        string strReceiveBatch = this.txtReceiveBatch.Text;
        //保留舊方法，調整命名空間
        if (CSIPKeyInGUI.BusinessRules.BRASExport.ExportASDetail(strBatchDate, strReceiveBatch, ref strServerPathFile, ref strMsgID))
        {
            //* 顯示提示訊息：匯出到Excel文檔資料成功
            string strFileName = "請款簽單明細表-人工.xls";
            this.Session["ServerFile"] = strServerPathFile;
            this.Session["ClientFile"] = strFileName;
            string urlString = @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);
        }
        //* 顯示產檔結果訊息
        MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
        //* 顯示端末訊息
        base.strClientMsg += MessageHelper.GetMessage(strMsgID);
    }
    /// <summary>
    /// 產生一般簽單高收檔
    /// 修改紀錄:2021/02/03_Ares_Stanley-註解Excel, 變更產出為TXT
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //產生一般簽單高收檔(Sign_Type = 1)
    protected void btnExportASgene_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
        string oriFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
        string strBatchDate = this.dpBatchDate.Text;
        string strReceiveBatch = this.txtReceiveBatch.Text;
        string strFileName1 = "";
        #region OLD_CODE
        //if (CSIPKeyInGUI.BusinessRules.BRASExport.ExportAS_ACQ(strBatchDate, strReceiveBatch, "1", ref strServerPathFile, ref strMsgID))
        //{
        //    //西元年轉民國年 yyyMMdd 共7碼，不足左邊補0     //保留舊方法，調整命名空間
        //    string strBatchDate_TW = CSIPKeyInGUI.BusinessRules.BRASExport.ToTWDateString(DateTime.Parse(strBatchDate));
        //    //* 一般簽單高收檔匯出檔案的檔名logic 為編批日期YYYMMDD(民國年)+收件批次+”A4”.xls
        //    string strFileName = strBatchDate_TW + strReceiveBatch + "A4.xls";

        //    //* 顯示提示訊息：匯出到Excel文檔資料成功
        //    this.Session["ServerFile"] = strServerPathFile;
        //    this.Session["ClientFile"] = strFileName;
        //    string urlString = @"location.href='DownLoadFile.aspx';";
        //    base.sbRegScript.Append(urlString);
        //}
        #endregion
        #region NEW_CODE
        //檢查目錄
        CSIPKeyInGUI.BusinessRules.BRExcel_File.CheckDirectory(ref strServerPathFile);
        CSIPKeyInGUI.BusinessRules.BRExcel_File.CheckDirectory(ref oriFile);
        if (BRASExport.ExportAS_ACQNEW_NPOI(strBatchDate, strReceiveBatch, "1", ref oriFile, ref strMsgID))
        {
            //西元年轉民國年 yyyMMdd 共7碼，不足左邊補0
            string strBatchDate_TW = BRASExport.ToTWDateString(DateTime.Parse(strBatchDate));
            //* 一般簽單高收檔匯出檔案的檔名logic 為編批日期YYYMMDD(民國年)+收件批次+”A4”.xls
            string strFileName_temp = strBatchDate_TW + strReceiveBatch + "A4_Temp.txt";
            strFileName1 = strBatchDate_TW + strReceiveBatch + "A4.txt";
            //轉換檔名及內容
            ConvertXlsToTxt(oriFile, Path.Combine(strServerPathFile, strFileName_temp));
            ACQConv(Path.Combine(strServerPathFile, strFileName_temp), Path.Combine(strServerPathFile, strFileName1));
            File.Delete(oriFile); //刪除excel檔
            //* 顯示提示訊息：匯出到Excel文檔資料成功
            this.Session["ServerFile"] = Path.Combine(strServerPathFile, strFileName1);
            this.Session["ClientFile"] = strFileName1;
            string urlString = @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);
        }
        #endregion

        //* 顯示產檔結果訊息
        MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
        //* 顯示端末訊息
        base.strClientMsg += MessageHelper.GetMessage(strMsgID);
    }

    //產生分期簽單高收檔(Sign_Type = 2)
    protected void btnExportASinst_Click(object sender, EventArgs e)
    {
        string strMsgID = string.Empty;
        string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
        string oriFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
        string strBatchDate = this.dpBatchDate.Text;
        string strReceiveBatch = this.txtReceiveBatch.Text;
        string strFileName2 = "";
        #region OLD_CODE
        ////保留舊方法，調整命名空間
        //if (CSIPKeyInGUI.BusinessRules.BRASExport.ExportAS_ACQ(strBatchDate, strReceiveBatch, "2", ref strServerPathFile, ref strMsgID))
        //{
        //    //西元年轉民國年 yyyMMdd 共7碼，不足左邊補0 //保留舊方法，調整命名空間
        //    string strBatchDate_TW = CSIPKeyInGUI.BusinessRules.BRASExport.ToTWDateString(DateTime.Parse(strBatchDate));
        //    //* 分期簽單高收檔匯出檔案的檔名logic 為”P”+編批日期YYYMMDD(民國年)+收件批次+”A4”.xls
        //    string strFileName = "P" + strBatchDate_TW + strReceiveBatch + "A4.xls";

        //    //* 顯示提示訊息：匯出到Excel文檔資料成功
        //    this.Session["ServerFile"] = strServerPathFile;
        //    this.Session["ClientFile"] = strFileName;
        //    string urlString = @"location.href='DownLoadFile.aspx';";
        //    base.sbRegScript.Append(urlString);
        //}
        #endregion
        #region NEW_CODE
        //檢查目錄
        CSIPKeyInGUI.BusinessRules.BRExcel_File.CheckDirectory(ref strServerPathFile);
        CSIPKeyInGUI.BusinessRules.BRExcel_File.CheckDirectory(ref oriFile);
        if (BRASExport.ExportAS_ACQNEW_NPOI(strBatchDate, strReceiveBatch, "2", ref oriFile, ref strMsgID))
        {
            //西元年轉民國年 yyyMMdd 共7碼，不足左邊補0
            string strBatchDate_TW = BRASExport.ToTWDateString(DateTime.Parse(strBatchDate));
            //* 一般簽單高收檔匯出檔案的檔名logic 為編批日期YYYMMDD(民國年)+收件批次+”A4”.xls
            string strFileName_temp = "P" + strBatchDate_TW + strReceiveBatch + "A4_Temp.txt";
            strFileName2 = "P" + strBatchDate_TW + strReceiveBatch + "A4.txt";
            //轉換檔名及內容
            ConvertXlsToTxt(oriFile, Path.Combine(strServerPathFile, strFileName_temp));
            ACQConv(Path.Combine(strServerPathFile, strFileName_temp), Path.Combine(strServerPathFile, strFileName2));
            File.Delete(oriFile); //刪除excel檔
            //* 顯示提示訊息：匯出到Excel文檔資料成功
            this.Session["ServerFile"] = Path.Combine(strServerPathFile, strFileName2);
            this.Session["ClientFile"] = strFileName2;
            string urlString = @"location.href='DownLoadFile.aspx';";
            base.sbRegScript.Append(urlString);
        }
        #endregion
        //* 顯示產檔結果訊息
        MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
        //* 顯示端末訊息
        base.strClientMsg += MessageHelper.GetMessage(strMsgID);
    }
    #endregion 產出檔案
    #endregion event

    #region function
    /// <summary>
    /// 從Show.xml取漢字，設置畫面控件的Text
    /// </summary>
    private void ShowControlsText()
    {
        this.gvpbSearchRecord.Columns[0].HeaderText = BaseHelper.GetShowText("01_01070100_007");
        this.gvpbSearchRecord.Columns[1].HeaderText = BaseHelper.GetShowText("01_01070100_008");
        this.gvpbSearchRecord.Columns[2].HeaderText = BaseHelper.GetShowText("01_01070100_009");
        this.gvpbSearchRecord.Columns[3].HeaderText = BaseHelper.GetShowText("01_01070100_010");
        this.gvpbSearchRecord.Columns[4].HeaderText = BaseHelper.GetShowText("01_01070100_011");
        this.gvpbSearchRecord.Columns[5].HeaderText = BaseHelper.GetShowText("01_01070100_012");
        this.gvpbSearchRecord.Columns[6].HeaderText = BaseHelper.GetShowText("01_01070100_013");
        this.gvpbSearchRecord.Columns[7].HeaderText = BaseHelper.GetShowText("01_01070100_014");
        this.gvpbSearchRecord.Columns[8].HeaderText = BaseHelper.GetShowText("01_01070100_015");
        this.gvpbSearchRecord.Columns[9].HeaderText = BaseHelper.GetShowText("01_01070100_016");
        this.gvpbSearchRecord.Columns[10].HeaderText = BaseHelper.GetShowText("01_01070100_017");
        this.gvpbSearchRecord.Columns[11].HeaderText = BaseHelper.GetShowText("01_01070100_018");
        this.gvpbSearchRecord.Columns[12].HeaderText = BaseHelper.GetShowText("01_01070100_019");
        this.gvpbSearchRecord.Columns[13].HeaderText = BaseHelper.GetShowText("01_01070100_020");
        this.gvpbSearchRecord.Columns[14].HeaderText = BaseHelper.GetShowText("01_01070100_021");
        this.gvpbSearchRecord.Columns[15].HeaderText = BaseHelper.GetShowText("01_01070100_022");
        this.gvpbSearchRecord.Columns[16].HeaderText = BaseHelper.GetShowText("01_01070100_023");
        this.gvpbSearchRecord.Columns[17].HeaderText = BaseHelper.GetShowText("01_01070100_024");
        this.gvpbSearchRecord.Columns[18].HeaderText = BaseHelper.GetShowText("01_01070100_025");
    }

    /// <summary>
    /// 從web.config取PageSize，設置畫面Gridview的PageSize
    /// </summary>
    private void SetPageSize()
    {
        //* 設置每頁顯示記錄最大條數
        int iPageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gpList.PageSize = iPageSize;
        this.gvpbSearchRecord.PageSize = iPageSize;
    }

    /// <summary>
    /// 綁定GridView數據源
    /// 修改紀錄: 2021/01/20_Ares_Stanley-設定查詢結果欄位css
    /// </summary>
    private void BindGridView()
    {
        try
        {
            string strMsgID = string.Empty;
            int intTotolCount = 0;

            string strBatchDate = this.dpBatchDate.Text.Replace("/", "");
            string strReceiveBatch = txtReceiveBatch.Text;

            //* 編批日期及收件批次存入Session，於查詢明細使用
            this.Session["BatchDate"] = strBatchDate;
            this.Session["ReceiveBatch"] = strReceiveBatch;

            //* 因每個分頁加入合計列，於查詢及換頁時需先重置PageSize
            SetPageSize();
            //保留舊方法，調整命名空間
            DataTable dt = CSIPKeyInGUI.BusinessRules.BRASExport.SearchASData(strBatchDate, strReceiveBatch, ref strMsgID, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref intTotolCount);
            if (dt == null)
            {
                this.gpList.RecordCount = 0;
                this.gvpbSearchRecord.DataSource = null;
                this.gvpbSearchRecord.DataBind();
            }
            else
            {
                #region 加入合計列  ※20140924 取消合計列
                //DataRow dr = dt.NewRow();
                //dr[0] = "合計";
                //dr[1] = DBNull.Value;
                //dr[2] = DBNull.Value;
                //dr[3] = dt.Compute("Sum(Receive_Total_Count)", "");
                //dr[4] = dt.Compute("Sum(Receive_Total_AMT)", "");
                //dr[5] = dt.Compute("Sum(Keyin_Success_Count_40)", "");
                //dr[6] = dt.Compute("Sum(Keyin_Success_AMT_40)", "");
                //dr[7] = dt.Compute("Sum(Keyin_Success_Count_41)", "");
                //dr[8] = dt.Compute("Sum(Keyin_Success_AMT_41)", "");
                //dr[9] = dt.Compute("Sum(Keyin_Reject_Count_40)", "");
                //dr[10] = dt.Compute("Sum(Keyin_Reject_AMT_40)", "");
                //dr[11] = dt.Compute("Sum(Keyin_Reject_Count_41)", "");
                //dr[12] = dt.Compute("Sum(Keyin_Reject_AMT_41)", "");
                //dr[13] = dt.Compute("Sum(Adjust_Count)", "");
                //dr[14] = dt.Compute("Sum(Adjust_AMT)", "");
                //dr[15] = DBNull.Value;
                //dr[16] = DBNull.Value;
                //dr[17] = DBNull.Value;
                //dr[18] = DBNull.Value;
                //dt.Rows.Add(dr);
                //int iPageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString()) + 1;
                //this.gpList.PageSize = iPageSize;
                //this.gvpbSearchRecord.PageSize = iPageSize;
                #endregion 加入合計列

                this.gpList.RecordCount = intTotolCount;
                this.gvpbSearchRecord.DataSource = dt;
                this.gvpbSearchRecord.DataBind();
            }
            for (int i = 0; i < this.gvpbSearchRecord.Columns.Count; i++)
            {
                this.gvpbSearchRecord.Columns[i].HeaderStyle.CssClass = "whiteSpaceNormal";
                this.gvpbSearchRecord.Columns[i].ItemStyle.CssClass = "whiteSpaceNormal";
            }
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage(strMsgID);
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage("01_01070100_001");
        }
    }
    protected void btnToACQ_Click(object sender, EventArgs e)
    {
        string strBatchDate = this.dpBatchDate.Text;
        string strReceiveBatch = this.txtReceiveBatch.Text;
        string strMsgID = string.Empty;
        string strFileName_temp = "";
        //調整輸出到交換目錄
        string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("exchangePath"));
        string strExchangeFile = this.Server.MapPath(UtilHelper.GetAppSettings("exchangePath"));
        string oriFile = this.Server.MapPath(UtilHelper.GetAppSettings("exchangePath"));
        // Logging.SaveLog(ELogLayer.UI, "輸出路徑" + strExchangeFile);
        Logging.Log("輸出路徑" + strExchangeFile,LogLayer.UI);

        //取遠端筆數及金額
        string strACQRtn = getRmtData(this.dpBatchDate.Text, txtReceiveBatch.Text);
        //本地筆數及金額

        DataTable dtLocal = BRASExport.SearchExportCheck(strBatchDate, strReceiveBatch, ref strMsgID);
        if (strACQRtn != "")
        {
            string[] rRtn = strACQRtn.Split(':');
            //檢核是否相符
            string RCnt = rRtn[0];
            string RAmt = rRtn[1];

            string LCnt = dtLocal.Rows[0][0].ToString();
            string LAmt = dtLocal.Rows[0][1].ToString();

            if (RCnt != LCnt || RAmt != LAmt)
            {
                string strCompare = string.Format("筆數及金額不符!\\r\\n ACQ筆數: {0}  \\t 本地筆數:{1} \\r\\n ACQ金額:{2} \\t 本地金額:{3} \\r\\n 匯出作業中止"

                    , RCnt, LCnt, RAmt, LAmt
                     );

                base.strAlertMsg = strCompare;
                return;
            }
        }
        //相符合則往下，不符合提示訊息


        string strFileName1 = "";
        string strFileName2 = "";
        //先產生一般簽單高收檔(Sign_Type = 1)
       
        // Logging.SaveLog(ELogLayer.UI, "讀取路徑設定");
        Logging.Log("讀取路徑設定", LogLayer.UI);
        
       
        if (!Directory.Exists(strExchangeFile))
        {
            Directory.CreateDirectory(strExchangeFile);
        }
        else
        {
            //先執行HOUSTKEEPING
            DropFiles(strExchangeFile, "7");
        }


        List<string> fList = new List<string>();

        //增加計算金額筆數
        if (BRASExport.ExportAS_ACQNEW_NPOI(strBatchDate, strReceiveBatch, "1", ref oriFile , ref strMsgID))
        {
            //西元年轉民國年 yyyMMdd 共7碼，不足左邊補0
            string strBatchDate_TW = BRASExport.ToTWDateString(DateTime.Parse(strBatchDate));
            //* 一般簽單高收檔匯出檔案的檔名logic 為編批日期YYYMMDD(民國年)+收件批次+”A4”.xls
            //* _Temp為xls轉換出的CSV檔(NPOI無法直接轉換為CSV)
            strFileName_temp = strBatchDate_TW + strReceiveBatch + "A4_Temp.txt";
            strFileName1 = strBatchDate_TW + strReceiveBatch + "A4.txt";
            //轉換檔名及內容
            ConvertXlsToTxt(oriFile, Path.Combine(strServerPathFile, strFileName_temp));
            ACQConv(Path.Combine(strServerPathFile, strFileName_temp), Path.Combine(strExchangeFile, strFileName1));
            File.Delete(oriFile); //刪除excel檔
            // Logging.SaveLog(ELogLayer.UI, "一般高收輸出檔名 :" + strFileName1);
            Logging.Log("一般高收輸出檔名 :" + strFileName1, LogLayer.UI);
            fList.Add( Path.Combine(strExchangeFile, strFileName1));
        }

        strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("exchangePath"));
        oriFile = this.Server.MapPath(UtilHelper.GetAppSettings("exchangePath"));
        //產生分期簽單高收檔(Sign_Type = 2)
        //增加計算金額筆數
        if (BRASExport.ExportAS_ACQNEW_NPOI(strBatchDate, strReceiveBatch, "2", ref oriFile, ref strMsgID))
        {
            //西元年轉民國年 yyyMMdd 共7碼，不足左邊補0
            string strBatchDate_TW = BRASExport.ToTWDateString(DateTime.Parse(strBatchDate));
            //* 分期簽單高收檔匯出檔案的檔名logic 為”P”+編批日期YYYMMDD(民國年)+收件批次+”A4”
            //* _Temp為xls轉換出的CSV檔(NPOI無法直接轉換為CSV)
            strFileName_temp = "P" + strBatchDate_TW + strReceiveBatch + "A4_Temp.txt";
            strFileName2 = "P" + strBatchDate_TW + strReceiveBatch + "A4.txt";
            ConvertXlsToTxt(oriFile, Path.Combine(strServerPathFile, strFileName_temp));
            ACQConv(Path.Combine(strServerPathFile, strFileName_temp), Path.Combine(strExchangeFile, strFileName2));
            File.Delete(oriFile); //刪除excel檔
            // Logging.SaveLog(ELogLayer.UI, "分期高收輸出檔名 :" + strFileName2);
            Logging.Log("分期高收輸出檔名 :" + strFileName2, LogLayer.UI);
            fList.Add( Path.Combine(strExchangeFile, strFileName2));
        }
        //增加檢核，若有檔案才送出，反之則提示無檔案匯出

        if (!string.IsNullOrEmpty(strFileName1) || !string.IsNullOrEmpty(strFileName2))
        {
            strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("exchangePath"));
            string destNy = strServerPathFile + "/" + "fromKEYIN.zip";    //壓縮後路徑及檔名
            if (File.Exists(destNy))
            {
                File.Delete(destNy);
            }

            //CompressToZip.ZipFile(oriName, zipPath);
            string zipKey = UtilHelper.GetAppSettings("CrossZipKey").ToString();

            CompressToZip.Zip(destNy, fList.ToArray(), "", zipKey, CompressToZip.CompressLevel.Level6);

            string result = PostToACQ(strBatchDate, strFileName1, strFileName2, destNy);
            string msgTmp = "<script type='text/javascript' language='JavaScript'>alert('{0}');</script>";
            msgTmp = string.Format(msgTmp, result);
            base.strAlertMsg = result;
            // Logging.SaveLog(ELogLayer.UI, "有檔案傳送至ACQ");
            Logging.Log("有檔案傳送至ACQ", LogLayer.UI);
            //傳送成功，刪除壓縮檔
            File.Delete(destNy);
        }
        else
        {
            // Logging.SaveLog(ELogLayer.UI, "無檔案傳送至ACQ");
            Logging.Log("無檔案傳送至ACQ", LogLayer.UI);
            strMsgID = "01_01070100_002";
            //* 顯示產檔結果訊息
            MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
            //* 顯示端末訊息
            base.strClientMsg += MessageHelper.GetMessage(strMsgID);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="strBatchDate">批次日期</param>
    /// <param name="strFileName1">一般檔案名</param>
    /// <param name="strFileName2">分期檔案名</param>
    /// <param name="destNy">壓縮檔名</param>
    /// <param name="cnt1">一般檔案筆數及金額</param>
    /// <param name="cnt2">分期檔案筆數及金額</param>
    /// <returns></returns>
    private string PostToACQ(string strBatchDate, string strFileName1, string strFileName2, string destNy)
    {
        string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("exchangePath"));
        ////產生的檔案路徑

        // //傳給ACQ的資料 編批  + 一般檔名 + 分期檔名 
        string paramACQ = this.txtReceiveBatch.Text + "|{0}|{1}|{2}";
        paramACQ = string.Format(paramACQ, strFileName1, strFileName2, eAgentInfo.agent_id);


        // //先解析DOMAIN，不然會有問題
        string sTemp = "http://{0}:{1}/page/CrossKEYIN.ashx";
        string serverName = UtilHelper.GetAppSettings("SERVER_ACQ");
        string serverPort = UtilHelper.GetAppSettings("SERVERPORT_ACQ");
        sTemp = string.Format(sTemp, serverName, serverPort);
        // Logging.SaveLog(ELogLayer.UI, "傳送至ACQ 網址 " + sTemp);
        Logging.Log("傳送至ACQ 網址 " + sTemp, LogLayer.UI);
        //建立參數物件，準備上傳
        string sDate = strBatchDate;
        string SAct = "Import|" + paramACQ;
        string sSource = "KeyinGui";     
        string hexKey = UtilHelper.GetAppSettings("CrossHasKey").ToString();
        List<FormItemModel> formItems = new List<FormItemModel>();
        //有檔名，才填入
        //if (!string.IsNullOrEmpty(strFileName1))
        //{
        //    formItems.Add(new FormItemModel("FILE1", "", strFileName1, new FileStream(Path.Combine(strServerPathFile, strFileName1), FileMode.Open, FileAccess.Read)));
        //}
        //else
        //{
        //    formItems.Add(new FormItemModel("FILE1", "", "", null));
        //}
        ////有檔名，才填入
        //if (!string.IsNullOrEmpty(strFileName2))
        //{

        //    formItems.Add(new FormItemModel("FILE2", "", strFileName2, new FileStream(Path.Combine(strServerPathFile, strFileName2), FileMode.Open, FileAccess.Read)));
        //}
        //else
        //{
        //    formItems.Add(new FormItemModel("FILE2", "", "", null));
        //}

        if (!string.IsNullOrEmpty(strFileName1))
        {
            formItems.Add(new FormItemModel("FILE1", "", strFileName1, new FileStream(destNy, FileMode.Open, FileAccess.Read)));
        }
        else
        {
            formItems.Add(new FormItemModel("FILE1", "", strFileName2, new FileStream(destNy, FileMode.Open, FileAccess.Read)));
        }
            

        formItems.Add(new FormItemModel("sDate", sDate, "", null));
        formItems.Add(new FormItemModel("Act", SAct, "", null));
        formItems.Add(new FormItemModel("Source", sSource, "", null));
        formItems.Add(new FormItemModel("hexKey", hexKey, "", null));
        string CompKey = sSource + sDate + SAct + hexKey;
        string checkSum = ENCSHA1(CompKey);
        formItems.Add(new FormItemModel("checkSum", checkSum, "", null));

        string result = PostForm(sTemp, formItems);
        // sTemp = string.Format(sTemp, serverName, serverPort, sDate, SAct, checkSum, sSource);
        // string strURL = "window.open('" + sTemp + "','','height=320,width=300,top=300,left=300,status=no,toolbar=no, menubar=no,location=no');";
        // base.sbRegScript.Append(strURL);

        return result;

    }
    /// <summary>
    /// 以編批日期取得ACQ總筆數及金額
    /// </summary>
    /// <param name="BatDate"></param>
    /// <returns></returns>
    private string getRmtData(string strBatchDate,string BatcnNo)
    {
        // //先解析DOMAIN，不然會有問題
        string sTemp = "http://{0}:{1}/page/CrossKEYIN.ashx";
        string serverName = UtilHelper.GetAppSettings("SERVER_ACQ");
        string serverPort = UtilHelper.GetAppSettings("SERVERPORT_ACQ");
        sTemp = string.Format(sTemp, serverName, serverPort);
        // Logging.SaveLog(ELogLayer.UI, "傳送至ACQ 網址 " + sTemp);
        Logging.Log("傳送至ACQ 網址 " + sTemp, LogLayer.UI);
        //建立參數物件，準備上傳
        string sDate = strBatchDate;
        string SAct = "Check|" + strBatchDate + "|" + BatcnNo;
        string sSource = "KeyinGui"; 
        string hexKey = UtilHelper.GetAppSettings("CrossHasKey").ToString();
        List<FormItemModel> formItems = new List<FormItemModel>();  
        formItems.Add(new FormItemModel("sDate", sDate, "", null));
        formItems.Add(new FormItemModel("Act", SAct, "", null));
        formItems.Add(new FormItemModel("Source", sSource, "", null));
        formItems.Add(new FormItemModel("hexKey", hexKey, "", null));
        string CompKey = sSource + sDate + SAct + hexKey;
        string checkSum = ENCSHA1(CompKey);
        formItems.Add(new FormItemModel("checkSum", checkSum, "", null));    
        string result = PostForm(sTemp, formItems);
        return result;
    }
    /// <summary>
    /// 轉換檔案內容及改檔名
    /// </summary>
    /// <param name="f1"></param>
    /// <param name="f2"></param>
    private void ACQConv(string f1, string f2)
    {
        StringBuilder sb = new StringBuilder();
        string[] AllTEXT = File.ReadAllLines(f1, Encoding.Default);

        foreach (string tempStr in AllTEXT)
        {
            int x = 0;
            string result = "";
            string warpSrt = tempStr;
            while (true)
            {
                //防止無限迴圈，最多跑1000次
                x++;
                //如果第一輪就沒有，那就是整行帶入  若已處理過，則將剩下的帶入
                if (warpSrt.IndexOf("\"") == -1)
                {
                    result += warpSrt;
                    break;
                }
                //找到第一個"
                int strInx = warpSrt.IndexOf("\"");
                //剪掉第一段，放到輸出
                result += warpSrt.Substring(0, strInx + 1);
                //將剩下的放回暫存
                warpSrt = warpSrt.Substring(strInx + 1);
                //找到第二個
                int endInx = warpSrt.IndexOf("\"");
                result += warpSrt.Substring(0, endInx).Replace(",", "，") + "\"";
                //將剩下的放回暫存
                warpSrt = warpSrt.Substring(endInx + 1);

                if (x > 1000)
                {
                    break;
                }
            }

            //先把,換TAB
            result = result.Replace(",", "	");
            //再把全形，換回半形
            result = result.Replace("，", ",");
            sb.AppendLine(result);

        }


        File.WriteAllText(f2, sb.ToString(), Encoding.Default);
        File.Delete(f1);
    }
    /// <summary>
    /// 建立日期:2021/02/03_Ares_Stanley-轉換xls為CSV檔案格式(因為NPOI無法直接存成CSV)
    /// </summary>
    /// <param name="oriFilePath"></param>
    /// <param name="aftFilePath"></param>
    protected void ConvertXlsToTxt(string oriFilePath, string aftFilePath)
    {
        StringBuilder sb = new StringBuilder();
        HSSFWorkbook contentwb;
        FileStream fs = new FileStream(oriFilePath, FileMode.Open);
        contentwb = new HSSFWorkbook(fs);
        ISheet sheet = contentwb.GetSheet("工作表1");
        for (int row = 0; row < sheet.LastRowNum + 1; row++)
        {
            string result = "";
            for (int col = 0; col < 11; col++)
            {
                switch (sheet.GetRow(row).GetCell(col).CellType)
                {
                    case CellType.String:
                       result += sheet.GetRow(row).GetCell(col).StringCellValue + ",";
                        break;
                    case CellType.Numeric:
                        result += sheet.GetRow(row).GetCell(col).NumericCellValue.ToString() + ",";
                        break;
                    case CellType.Blank:
                        result += ",";
                        break;
                    case CellType.Formula:
                        result += sheet.GetRow(row).GetCell(col).NumericCellValue.ToString() + ",";
                        break;
                    default:
                        result += ",";
                        break;
                }
            }
            sb.AppendLine(result);
        }
        FileStream fs2 = File.Create(aftFilePath);
        fs2.Close();
        File.WriteAllText(aftFilePath, sb.ToString(), Encoding.Default);
    }
    /// <summary>
    /// 依傳入路徑，檔案建立日期，超過7天直接刪除
    /// </summary>
    /// <param name="expDay"></param>
    public void DropFiles(string sPath, string expDay)
    {
        int delDay = 7;
        int.TryParse(expDay, out delDay);
        string relDelDt = DateTime.Today.AddDays(delDay * -1).ToString("yyyyMMdd");
        long delDt = 0;
        long.TryParse(relDelDt, out delDt);
        string[] fireColl = Directory.GetFiles(sPath);
        foreach (string firName in fireColl)
        {
            FileInfo drInf = new FileInfo(firName);
            long drName = 0;
            long.TryParse(drInf.CreationTime.ToString("yyyyMMdd"), out drName);
            //已逾期，刪除
            if (drName < delDt)
            {
                drInf.Delete();
            }

        }

    }
    /// <summary>
    /// 產生檢查碼
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private string ENCSHA1(string input)
    {
        SHA1 sha1 = new SHA1CryptoServiceProvider();
        string resultSha1 = Convert.ToBase64String(sha1.ComputeHash(Encoding.Default.GetBytes(input)));
        return resultSha1;
    }
    #endregion function

    /// <summary>  
    /// 使用Post方法获取字符串结果  
    /// </summary>  
    /// <param name="url"></param>  
    /// <param name="formItems">Post表单内容</param>  
    /// <param name="cookieContainer"></param>  
    /// <param name="timeOut">默认20秒</param>  
    /// <param name="encoding">响应内容的编码类型（默认utf-8）</param>  
    /// <returns></returns>  
    public static string PostForm(string url, List<FormItemModel> formItems)
    {
        HttpWebRequest request = null;
        //HTTPSQ请求  

        request = WebRequest.Create(url) as HttpWebRequest;
        request.ProtocolVersion = HttpVersion.Version10;
        #region 初始化请求对象
        request.Method = "POST";
        request.Timeout = 60000; //一分鐘
        //request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
        request.KeepAlive = true;
        #endregion

        string boundary = "----" + DateTime.Now.Ticks.ToString("x");//分隔符  
        request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
        //请求流  
        Stream postStream = new MemoryStream();
        #region 处理Form表单请求内容
        //是否用Form上传文件  

        //文件数据模板  
        string fileFormdataTemplate =
            "\r\n--" + boundary +
            "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"" +
            "\r\nContent-Type: multipart/form-data" +
            "\r\n\r\n";
        //文本数据模板  
        string dataFormdataTemplate =
            "\r\n--" + boundary +
            "\r\nContent-Disposition: form-data; name=\"{0}\"" +
            "\r\n\r\n{1}";
        foreach (FormItemModel item in formItems)
        {
            string formdata = null;
            if (item.IsFile)
            {
                //上传文件  
                formdata = string.Format(
                    fileFormdataTemplate,
                    item.Key, //表单键  
                    item.FileName);
            }
            else
            {
                //上传文本  
                formdata = string.Format(
                    dataFormdataTemplate,
                    item.Key,
                    item.Value);
            }

            //统一处理  
            byte[] formdataBytes = null;
            //第一行不需要换行  
            if (postStream.Length == 0)
                formdataBytes = Encoding.UTF8.GetBytes(formdata.Substring(2, formdata.Length - 2));
            else
                formdataBytes = Encoding.UTF8.GetBytes(formdata);
            postStream.Write(formdataBytes, 0, formdataBytes.Length);

            //写入文件内容  
            if (item.FileContent != null && item.FileContent.Length > 0)
            {
                using (Stream stream = item.FileContent)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = 0;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        postStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }
        //结尾  
        byte[] footer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
        postStream.Write(footer, 0, footer.Length);
        #endregion

        request.ContentLength = postStream.Length;

        #region 输入二进制流
        if (postStream != null)
        {
            postStream.Position = 0;
            //直接写入流  
            Stream requestStream = request.GetRequestStream();

            byte[] buffer = new byte[1024];
            int bytesRead = 0;
            while ((bytesRead = postStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                requestStream.Write(buffer, 0, bytesRead);
            }

            ////debug  
            //postStream.Seek(0, SeekOrigin.Begin);  
            //StreamReader sr = new StreamReader(postStream);  
            //var postStr = sr.ReadToEnd();  
            postStream.Close();//关闭文件访问  
        }
        #endregion

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        using (Stream responseStream = response.GetResponseStream())
        {
            using (StreamReader myStreamReader = new StreamReader(responseStream, Encoding.UTF8))
            {
                string retString = myStreamReader.ReadToEnd();
                // Logging.SaveLog(ELogLayer.UI, "傳送至ACQ 回應訊息" + retString);
                Logging.Log("傳送至ACQ 回應訊息" + retString,LogLayer.UI);
                
                return retString;
            }
        }
    }



    //上傳檔案的模組
    public class FormItemModel
    {
        public FormItemModel(string inKey, string inValue, string inFilename, Stream inFileCntent)
        {
            _Key = inKey;
            _Value = inValue;
            _FileName = inFilename;
            _FileContent = inFileCntent;
        }
        private string _Key = "";
        private string _Value = "";

        private string _FileName = "";
        private Stream _FileContent = new MemoryStream();
        /// <summary>  
        /// 表单键，request["key"]  
        /// </summary>  
        public string Key
        {
            set
            {
                _Key = value;
            }
            get
            {
                return _Key;
            }
        }
        /// <summary>  
        /// 表单值,上传文件时忽略，request["key"].value  
        /// </summary>  
        public string Value
        {
            set
            {
                _Value = value;
            }
            get
            {
                return _Value;
            }
        }
        /// <summary>  
        /// 是否是文件  
        /// </summary>  
        public bool IsFile
        {
            get
            {
                if (FileContent == null || FileContent.Length == 0)
                    return false;

                if (FileContent != null && FileContent.Length > 0 && string.IsNullOrEmpty(FileName))
                    throw new Exception("上传文件时 FileName 属性值不能为空");
                return true;
            }
        }
        /// <summary>  
        /// 上传的文件名  
        /// </summary>  
        public string FileName
        {
            set
            {
                _FileName = value;
            }
            get
            {
                return _FileName;
            }
        }
        /// <summary>  
        /// 上传的文件内容  
        /// </summary>  
        public Stream FileContent
        {
            set
            {
                _FileContent = value;
            }
            get
            {
                return _FileContent;
            }
        }
    }
}
